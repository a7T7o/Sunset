using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

[TestFixture]
public class NpcCrowdPrefabBindingTests
{
    private const string PrefabRoot = "Assets/222_Prefabs/NPC";

    private static readonly PrefabTruthEntry[] ExpectedPrefabs =
    {
        new PrefabTruthEntry($"{PrefabRoot}/101.prefab", "101", "ScribeCrowd"),
        new PrefabTruthEntry($"{PrefabRoot}/102.prefab", "102", "HunterClue"),
        new PrefabTruthEntry($"{PrefabRoot}/103.prefab", "103", "WitnessBoy"),
        new PrefabTruthEntry($"{PrefabRoot}/104.prefab", "104", "CarpenterCrowd"),
        new PrefabTruthEntry($"{PrefabRoot}/201.prefab", "201", "MendingCrowd"),
        new PrefabTruthEntry($"{PrefabRoot}/202.prefab", "202", "HerbCrowd"),
        new PrefabTruthEntry($"{PrefabRoot}/203.prefab", "203", "DinnerCrowd"),
    };

    [Test]
    public void CrowdPrefabs_ShouldKeepNpcOwnBindingMatrixStable()
    {
        Type roamControllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type bubblePresenterType = ResolveTypeOrFail("NPCBubblePresenter");
        Type informalType = ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable");

        foreach (PrefabTruthEntry entry in ExpectedPrefabs)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.PrefabPath);
            Assert.That(prefab, Is.Not.Null, $"未找到 prefab: {entry.PrefabPath}");

            Component roamController = prefab.GetComponent(roamControllerType);
            Component bubblePresenter = prefab.GetComponent(bubblePresenterType);
            Component informalInteractable = prefab.GetComponent(informalType);

            Assert.That(roamController, Is.Not.Null, $"{entry.PrefabPath} 缺少 NPCAutoRoamController。");
            Assert.That(bubblePresenter, Is.Not.Null, $"{entry.PrefabPath} 缺少 NPCBubblePresenter。");
            Assert.That(informalInteractable, Is.Not.Null, $"{entry.PrefabPath} 缺少 NPCInformalChatInteractable。");

            UnityEngine.Object roamProfile = GetPrivateField<UnityEngine.Object>(roamController, "roamProfile");
            Assert.That(roamProfile, Is.Not.Null, $"{entry.PrefabPath} 缺少 roamProfile 绑定。");
            Assert.That(GetPublicProperty<string>(roamProfile, "NpcId"), Is.EqualTo(entry.NpcId), $"{entry.PrefabPath} 的 roamProfile npcId 漂了。");
            StringAssert.Contains(entry.NeutralToken, roamProfile.name, $"{entry.PrefabPath} 的 roamProfile 命名不再是群众口径。");

            UnityEngine.Object boundRoamController = GetPrivateField<UnityEngine.Object>(informalInteractable, "autoRoamController");
            UnityEngine.Object boundBubblePresenter = GetPrivateField<UnityEngine.Object>(informalInteractable, "bubblePresenter");

            Assert.That(boundRoamController, Is.SameAs(roamController), $"{entry.PrefabPath} 的 casual 交互没有绑定同 prefab 的 roam controller。");
            Assert.That(boundBubblePresenter, Is.SameAs(bubblePresenter), $"{entry.PrefabPath} 的 casual 交互没有绑定同 prefab 的 bubble presenter。");

            float interactionDistance = GetPrivateField<float>(informalInteractable, "interactionDistance");
            float sessionBreakDistance = GetPrivateField<float>(informalInteractable, "sessionBreakDistance");
            Assert.That(interactionDistance, Is.GreaterThan(0.8f), $"{entry.PrefabPath} 的起聊距离异常。");
            Assert.That(sessionBreakDistance, Is.GreaterThan(interactionDistance), $"{entry.PrefabPath} 的中断距离应大于起聊距离。");
        }
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

    private static T GetPrivateField<T>(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        return (T)field.GetValue(target);
    }

    private static T GetPublicProperty<T>(object target, string propertyName)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到属性: {target.GetType().Name}.{propertyName}");
        return (T)property.GetValue(target);
    }

    private readonly struct PrefabTruthEntry
    {
        public PrefabTruthEntry(string prefabPath, string npcId, string neutralToken)
        {
            PrefabPath = prefabPath;
            NpcId = npcId;
            NeutralToken = neutralToken;
        }

        public string PrefabPath { get; }
        public string NpcId { get; }
        public string NeutralToken { get; }
    }
}
