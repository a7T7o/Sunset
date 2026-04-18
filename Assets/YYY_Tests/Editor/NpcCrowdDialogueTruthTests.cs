using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;

[TestFixture]
public class NpcCrowdDialogueTruthTests
{
    private const string CrowdRoot = "Assets/111_Data/NPC/SpringDay1Crowd";

    private static readonly CrowdTruthEntry[] ExpectedCrowdTruthTable =
    {
        new CrowdTruthEntry($"{CrowdRoot}/NPC_101_LedgerScribeDialogueContent.asset", "101", "ScribeCrowd", "103", "104", "203"),
        new CrowdTruthEntry($"{CrowdRoot}/NPC_102_HunterDialogueContent.asset", "102", "HunterClue"),
        new CrowdTruthEntry($"{CrowdRoot}/NPC_103_ErrandBoyDialogueContent.asset", "103", "WitnessBoy", "101", "104", "203"),
        new CrowdTruthEntry($"{CrowdRoot}/NPC_104_CarpenterDialogueContent.asset", "104", "CarpenterCrowd", "101", "103", "203"),
        new CrowdTruthEntry($"{CrowdRoot}/NPC_201_SeamstressDialogueContent.asset", "201", "MendingCrowd", "202"),
        new CrowdTruthEntry($"{CrowdRoot}/NPC_202_FloristDialogueContent.asset", "202", "HerbCrowd", "201"),
        new CrowdTruthEntry($"{CrowdRoot}/NPC_203_CanteenKeeperDialogueContent.asset", "203", "DinnerCrowd", "101", "103", "104")
    };

    private static readonly string[] BannedFormalRoleTokens =
    {
        "VillageChief",
        "VillageDaughter",
        "Research"
    };

    [Test]
    public void SpringDay1CrowdProfiles_ShouldStayOnNeutralizedTruthTable()
    {
        foreach (CrowdTruthEntry entry in ExpectedCrowdTruthTable)
        {
            object profile = LoadProfile(entry.AssetPath);
            string profileName = GetUnityObjectName(profile);
            object defaultWalkAwayReaction = GetPublicProperty<object>(profile, "DefaultWalkAwayReaction");

            Assert.That(InvokeInstance<string>(profile, "ResolveNpcId", string.Empty), Is.EqualTo(entry.NpcId), $"{entry.AssetPath} 的 npcId 漂了。");
            StringAssert.Contains(entry.NeutralToken, profileName, $"{entry.AssetPath} 没守住当前群众/线索位命名。");
            Assert.That(GetPublicProperty<bool>(profile, "HasInformalConversationContent"), Is.True, $"{entry.AssetPath} 缺正式允许的非正式闲聊包。");
            Assert.That(GetPublicProperty<bool>(profile, "HasAmbientChatInitiatorContent"), Is.True, $"{entry.AssetPath} 缺环境发起气泡。");
            Assert.That(GetPublicProperty<bool>(profile, "HasAmbientChatResponderContent"), Is.True, $"{entry.AssetPath} 缺环境回应气泡。");
            Assert.That(defaultWalkAwayReaction, Is.Not.Null, $"{entry.AssetPath} 缺离开反应。");
            Assert.That(InvokeInstance<bool>(defaultWalkAwayReaction, "HasAnyContent"), Is.True, $"{entry.AssetPath} 的离开反应是空的。");

            for (int index = 0; index < BannedFormalRoleTokens.Length; index++)
            {
                StringAssert.DoesNotContain(BannedFormalRoleTokens[index], profileName, $"{entry.AssetPath} 又开始冒用 Day1 正式主角色命名。");
            }
        }
    }

    [Test]
    public void SpringDay1CrowdPairDialogue_ShouldMatchCurrentTruthMatrix()
    {
        foreach (CrowdTruthEntry entry in ExpectedCrowdTruthTable)
        {
            object profile = LoadProfile(entry.AssetPath);
            Array pairSets = GetPublicProperty<Array>(profile, "PairDialogueSets");
            string[] actualPartners = pairSets
                .Cast<object>()
                .Where(pairSet => pairSet != null)
                .Select(pairSet => GetPublicProperty<string>(pairSet, "PartnerNpcId"))
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .OrderBy(id => id)
                .ToArray();

            Assert.That(actualPartners, Is.EqualTo(entry.PairPartners.OrderBy(id => id).ToArray()), $"{entry.AssetPath} 的 pair partner 矩阵不符合当前真值。");

            for (int index = 0; index < pairSets.Length; index++)
            {
                object pairSet = pairSets.GetValue(index);
                Assert.That(pairSet, Is.Not.Null, $"{entry.AssetPath} 存在空的 pair set。");
                Assert.That(GetPublicProperty<string[]>(pairSet, "InitiatorLines").Any(line => !string.IsNullOrWhiteSpace(line)), Is.True, $"{entry.AssetPath} 的 pair initiator 为空。");
                Assert.That(GetPublicProperty<string[]>(pairSet, "ResponderLines").Any(line => !string.IsNullOrWhiteSpace(line)), Is.True, $"{entry.AssetPath} 的 pair responder 为空。");
            }
        }
    }

    private static object LoadProfile(string assetPath)
    {
        object profile = AssetDatabase.LoadAssetAtPath(assetPath, ResolveTypeOrFail("NPCDialogueContentProfile"));
        Assert.That(profile, Is.Not.Null, $"未找到 NPC crowd 资产: {assetPath}");
        return profile;
    }

    private static string GetUnityObjectName(object target)
    {
        PropertyInfo property = target.GetType().GetProperty("name", BindingFlags.Instance | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到 name 属性: {target.GetType().Name}");
        return property.GetValue(target) as string ?? string.Empty;
    }

    private static T GetPublicProperty<T>(object target, string propertyName)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到属性: {target.GetType().Name}.{propertyName}");
        return (T)property.GetValue(target);
    }

    private static T InvokeInstance<T>(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return (T)method.Invoke(target, args);
    }

    private static Type ResolveTypeOrFail(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type resolvedType = TryResolveType(assembly, typeName);
            if (resolvedType != null)
            {
                return resolvedType;
            }
        }

        Assert.Fail($"未找到类型: {typeName}");
        return null;
    }

    private static Type TryResolveType(Assembly assembly, string typeName)
    {
        Type direct = assembly.GetType(typeName, false);
        if (direct != null)
        {
            return direct;
        }

        try
        {
            return assembly
                .GetTypes()
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal) ||
                                        string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types
                .Where(type => type != null)
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal) ||
                                        string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
    }

    private readonly struct CrowdTruthEntry
    {
        public CrowdTruthEntry(string assetPath, string npcId, string neutralToken, params string[] pairPartners)
        {
            AssetPath = assetPath;
            NpcId = npcId;
            NeutralToken = neutralToken;
            PairPartners = pairPartners ?? Array.Empty<string>();
        }

        public string AssetPath { get; }
        public string NpcId { get; }
        public string NeutralToken { get; }
        public string[] PairPartners { get; }
    }
}
