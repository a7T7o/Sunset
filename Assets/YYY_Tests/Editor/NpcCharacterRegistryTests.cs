using System;
using System.IO;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

[TestFixture]
public class NpcCharacterRegistryTests
{
    private const string RegistryAssetPath = "Assets/Resources/Story/NpcCharacterRegistry.asset";
    private const string DialogueAssetRoot = "Assets/111_Data/Story/Dialogue";
    private const string NpcContentRoot = "Assets/111_Data/NPC";
    private const string CrowdManifestAssetPath = "Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset";
    private const string HandPortraitFolderPath = "Assets/Sprites/NPC_Hand";
    private const string DialogueUiPath = "Assets/YYY_Scripts/Story/UI/DialogueUI.cs";
    private const string RelationshipPanelPath = "Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs";
    private const string HandPortraitAutoSyncPath = "Assets/Editor/NPC/NpcCharacterRegistryHandPortraitAutoSync.cs";
    private const string RegistryTypeName = "Sunset.Story.NpcCharacterRegistry";
    private const string DialogueSequenceTypeName = "Sunset.Story.DialogueSequenceSO";
    private const string CrowdManifestTypeName = "Sunset.Story.SpringDay1NpcCrowdManifest";
    private const string DialogueContentProfileTypeName = "NPCDialogueContentProfile";
    private const string HandPortraitAutoSyncTypeName = "Sunset.Editor.Npc.NpcCharacterRegistryHandPortraitAutoSync";

    private static readonly string[] ExpectedNpcRoster =
    {
        "001", "002", "003",
        "101", "102", "103", "104",
        "201", "202", "203",
        "301"
    };

    private static readonly string[] IgnoredDialogueSpeakers =
    {
        string.Empty
    };

    [Test]
    public void Registry_ShouldCoverCurrentNpcRoster_AndExposeThemToRelationshipPanel()
    {
        object registry = LoadRegistryOrFail();

        for (int index = 0; index < ExpectedNpcRoster.Length; index++)
        {
            string npcId = ExpectedNpcRoster[index];
            Assert.That(TryInvokeRegistryEntryMethod(registry, "TryGetEntryByNpcId", npcId, out object entry), Is.True, $"主表缺少 npcId={npcId}");
            Assert.That(entry, Is.Not.Null, $"npcId={npcId} 没有解析到条目。");
            Assert.That((bool)ReadMemberValue(entry, "showInRelationshipPanel"), Is.True, $"npcId={npcId} 还没进关系页主表。");
            Assert.That(InvokeEntryMethod(entry, "ResolveRelationshipPortrait") as Sprite, Is.Not.Null, $"npcId={npcId} 缺关系页 portrait。");
        }
    }

    [Test]
    public void Registry_ShouldResolveCoreSpeakerAliases_ToStableNpcIds()
    {
        object registry = LoadRegistryOrFail();

        AssertAlias(registry, "村长", "001");
        AssertAlias(registry, "马库斯", "001");
        AssertAlias(registry, "艾拉", "002");
        AssertAlias(registry, "卡尔", "003");
        AssertAlias(registry, "旅人", "000");
        AssertAlias(registry, "陌生旅人", "000");
        AssertAlias(registry, "村民", "101");
        AssertAlias(registry, "小孩", "103");
        AssertAlias(registry, "小米", "103");
        AssertAlias(registry, "饭馆村民", "203");
        AssertAlias(registry, "老杰克", "102");
        AssertAlias(registry, "老乔治", "104");
        AssertAlias(registry, "老汤姆", "301");
    }

    [Test]
    public void Registry_ShouldProvideDialoguePortraits_ForFormalAndCrowdFallbackSpeakers()
    {
        object registry = LoadRegistryOrFail();

        AssertPortrait(registry, "村长");
        AssertPortrait(registry, "艾拉");
        AssertPortrait(registry, "卡尔");
        AssertPortrait(registry, "旅人");
        AssertPortrait(registry, "陌生旅人");
        AssertPortrait(registry, "村民");
        AssertPortrait(registry, "小孩");
        AssertPortrait(registry, "饭馆村民");
    }

    [Test]
    public void Registry_ShouldUseHandPortrait_AsCanonicalPortrait_WhenAvailable()
    {
        RunHandPortraitAutoSyncOrFail();
        object registry = LoadRegistryOrFail();
        string[] expectedHandPortraitRoster = GetExpectedHandPortraitRosterFromFolder();
        Assert.That(expectedHandPortraitRoster.Length, Is.GreaterThan(0), "NPC_Hand 目录里还没有可同步头像。");

        for (int index = 0; index < expectedHandPortraitRoster.Length; index++)
        {
            string npcId = expectedHandPortraitRoster[index];
            Assert.That(TryInvokeRegistryEntryMethod(registry, "TryGetEntryByNpcId", npcId, out object entry), Is.True, $"主表缺少 npcId={npcId}");
            Sprite handPortrait = ReadMemberValue(entry, "handPortrait") as Sprite;
            Assert.That(handPortrait, Is.Not.Null, $"npcId={npcId} 还没挂 handPortrait。");
            Assert.That(InvokeEntryMethod(entry, "ResolveDialoguePortrait") as Sprite, Is.SameAs(handPortrait), $"npcId={npcId} 的对白头像没有优先吃 handPortrait。");
            Assert.That(InvokeEntryMethod(entry, "ResolveRelationshipPortrait") as Sprite, Is.SameAs(handPortrait), $"npcId={npcId} 的关系页头像没有优先吃 handPortrait。");
        }
    }

    [Test]
    public void DialogueUi_AndRelationshipPanel_ShouldConsumeRegistry()
    {
        string dialogueUiSource = File.ReadAllText(DialogueUiPath);
        StringAssert.Contains("ResolveRegistryDialoguePortrait", dialogueUiSource, "DialogueUI 还没把正式对白头像接回统一主表。");
        StringAssert.Contains("NpcCharacterRegistry.LoadRuntime()", dialogueUiSource, "DialogueUI 还没加载统一主表。");
        StringAssert.Contains("ResolveSpecialDialoguePortrait", dialogueUiSource, "DialogueUI 还没把玩家/旁白头像单独接到 000 号 portrait。");
        StringAssert.Contains("ResolveEntryPortraitByNpcId(PlayerPortraitNpcId)", dialogueUiSource, "DialogueUI 还没把玩家/旁白头像回到 000 号条目。");

        string relationshipPanelSource = File.ReadAllText(RelationshipPanelPath);
        StringAssert.Contains("NpcCharacterRegistry.LoadRuntime()", relationshipPanelSource, "关系页还没开始吃统一主表。");
        StringAssert.Contains("BuildRegistryBackedEntry", relationshipPanelSource, "关系页还没把身份主表和 manifest 语义拆开。");

        string autoSyncSource = File.ReadAllText(HandPortraitAutoSyncPath);
        StringAssert.Contains("AssetPostprocessor", autoSyncSource, "NPC_Hand 真源自动同步器还没落地。");
        StringAssert.Contains("HandPortraitFolderPath", autoSyncSource, "自动同步器还没把 NPC_Hand 当成头像真源。");
    }

    [Test]
    public void Registry_ShouldResolveEveryAuthoredDialogueSpeaker_ToStableNpcIdentity()
    {
        object registry = LoadRegistryOrFail();
        Type dialogueSequenceType = ResolveTypeOrFail(DialogueSequenceTypeName);
        string[] dialogueGuids = AssetDatabase.FindAssets("t:DialogueSequenceSO", new[] { DialogueAssetRoot });
        Assert.That(dialogueGuids.Length, Is.GreaterThan(0), "没找到 authored dialogue 资产。");

        for (int index = 0; index < dialogueGuids.Length; index++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(dialogueGuids[index]);
            object sequence = AssetDatabase.LoadAssetAtPath(assetPath, dialogueSequenceType);
            Assert.That(sequence, Is.Not.Null, $"未能加载 dialogue asset: {assetPath}");

            IList nodes = ReadMemberValue(sequence, "nodes") as IList;
            Assert.That(nodes, Is.Not.Null, $"{assetPath} 的 nodes 读取失败。");

            for (int nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
            {
                object node = nodes[nodeIndex];
                string speakerName = (ReadMemberValue(node, "speakerName") as string ?? string.Empty).Trim();
                if (ShouldIgnoreDialogueSpeaker(speakerName))
                {
                    continue;
                }

                Assert.That(
                    TryInvokeRegistryEntryMethod(registry, "TryResolveSpeaker", speakerName, out object entry),
                    Is.True,
                    $"{assetPath} 第 {nodeIndex + 1} 句 speaker={speakerName} 没回到统一主表。");
                Assert.That(entry, Is.Not.Null, $"{assetPath} 第 {nodeIndex + 1} 句 speaker={speakerName} 没拿到 registry 条目。");
                Assert.That((string)ReadMemberValue(entry, "npcId"), Is.Not.Empty, $"{assetPath} 第 {nodeIndex + 1} 句 speaker={speakerName} 没绑定稳定 npcId。");
                AssertPortrait(registry, speakerName);
            }
        }
    }

    [Test]
    public void Registry_ShouldStayInSync_WithNpcDialogueContentAssets_AndCrowdManifest()
    {
        object registry = LoadRegistryOrFail();
        Type contentProfileType = ResolveTypeOrFail(DialogueContentProfileTypeName);
        string[] profileGuids = AssetDatabase.FindAssets("t:NPCDialogueContentProfile", new[] { NpcContentRoot });
        Assert.That(profileGuids.Length, Is.GreaterThan(0), "没找到 NPC dialogue content 资产。");

        for (int index = 0; index < profileGuids.Length; index++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(profileGuids[index]);
            object profile = AssetDatabase.LoadAssetAtPath(assetPath, contentProfileType);
            Assert.That(profile, Is.Not.Null, $"未能加载 NPC dialogue profile: {assetPath}");

            string npcId = (ReadMemberValue(profile, "NpcId") as string ?? string.Empty).Trim();
            Assert.That(npcId, Is.Not.Empty, $"{assetPath} 没有稳定 npcId。");
            Assert.That(
                TryInvokeRegistryEntryMethod(registry, "TryGetEntryByNpcId", npcId, out object entry),
                Is.True,
                $"{assetPath} 的 npcId={npcId} 还没进统一主表。");
            Assert.That(entry, Is.Not.Null, $"{assetPath} 的 npcId={npcId} 没拿到 registry 条目。");
        }

        object manifest = AssetDatabase.LoadAssetAtPath(CrowdManifestAssetPath, ResolveTypeOrFail(CrowdManifestTypeName));
        Assert.That(manifest, Is.Not.Null, "未找到 crowd manifest 资产。");

        Array manifestEntries = ReadMemberValue(manifest, "Entries") as Array;
        Assert.That(manifestEntries, Is.Not.Null, "crowd manifest Entries 读取失败。");
        for (int index = 0; index < manifestEntries.Length; index++)
        {
            object manifestEntry = manifestEntries.GetValue(index);
            string npcId = (ReadMemberValue(manifestEntry, "npcId") as string ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(npcId))
            {
                continue;
            }

            Assert.That(
                TryInvokeRegistryEntryMethod(registry, "TryGetEntryByNpcId", npcId, out object entry),
                Is.True,
                $"crowd manifest npcId={npcId} 还没回到统一主表。");
            Assert.That(entry, Is.Not.Null, $"crowd manifest npcId={npcId} 没拿到 registry 条目。");
        }
    }

    private static object LoadRegistryOrFail()
    {
        Type registryType = ResolveTypeOrFail(RegistryTypeName);
        object registry = AssetDatabase.LoadAssetAtPath(RegistryAssetPath, registryType);
        Assert.That(registry, Is.Not.Null, "未找到统一人物主表资产。");
        return registry;
    }

    private static void RunHandPortraitAutoSyncOrFail()
    {
        Type syncType = ResolveTypeOrFail(HandPortraitAutoSyncTypeName);
        MethodInfo method = syncType.GetMethod("TrySyncRegistryAsset", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, "没有找到 NPC_Hand 自动同步入口。");
        method.Invoke(null, null);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void AssertAlias(object registry, string speakerName, string expectedNpcId)
    {
        Assert.That(TryInvokeRegistryEntryMethod(registry, "TryResolveSpeaker", speakerName, out object entry), Is.True, $"别名 {speakerName} 没解析到主表。");
        Assert.That(entry, Is.Not.Null, $"别名 {speakerName} 没拿到条目。");
        Assert.That((string)ReadMemberValue(entry, "npcId"), Is.EqualTo(expectedNpcId), $"别名 {speakerName} 没回到预期 npcId。");
    }

    private static void AssertPortrait(object registry, string speakerName)
    {
        object[] args = { speakerName, null };
        bool resolved = (bool)ResolveTypeOrFail(RegistryTypeName)
            .GetMethod("TryResolveDialoguePortrait", BindingFlags.Instance | BindingFlags.Public)
            .Invoke(registry, args);
        Assert.That(resolved, Is.True, $"{speakerName} 还没拿到对白 portrait。");
        Assert.That(args[1] as Sprite, Is.Not.Null, $"{speakerName} 的对白 portrait 仍为空。");
    }

    private static bool TryInvokeRegistryEntryMethod(object registry, string methodName, string input, out object entry)
    {
        object[] args = { input, null };
        bool resolved = (bool)ResolveTypeOrFail(RegistryTypeName)
            .GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public)
            .Invoke(registry, args);
        entry = args[1];
        return resolved;
    }

    private static bool ShouldIgnoreDialogueSpeaker(string speakerName)
    {
        for (int index = 0; index < IgnoredDialogueSpeakers.Length; index++)
        {
            if (string.Equals(speakerName, IgnoredDialogueSpeakers[index], StringComparison.Ordinal))
            {
                return true;
            }
        }

        return string.IsNullOrWhiteSpace(speakerName);
    }

    private static object InvokeEntryMethod(object entry, string methodName)
    {
        Assert.That(entry, Is.Not.Null);
        MethodInfo method = entry.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(method, Is.Not.Null, $"未找到方法: {entry.GetType().FullName}.{methodName}");
        return method.Invoke(entry, null);
    }

    private static string[] GetExpectedHandPortraitRosterFromFolder()
    {
        string[] spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { HandPortraitFolderPath });
        Assert.That(spriteGuids, Is.Not.Null);

        string[] npcIds = new string[spriteGuids.Length];
        int count = 0;
        for (int index = 0; index < spriteGuids.Length; index++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(spriteGuids[index]);
            string npcId = Path.GetFileNameWithoutExtension(assetPath)?.Trim();
            if (string.IsNullOrWhiteSpace(npcId))
            {
                continue;
            }

            npcIds[count++] = npcId;
        }

        Array.Resize(ref npcIds, count);
        Array.Sort(npcIds, StringComparer.Ordinal);
        return npcIds;
    }

    private static object ReadMemberValue(object target, string memberName)
    {
        Assert.That(target, Is.Not.Null);

        FieldInfo field = target.GetType().GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null)
        {
            return field.GetValue(target);
        }

        PropertyInfo property = target.GetType().GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null)
        {
            return property.GetValue(target);
        }

        Assert.Fail($"未找到成员: {target.GetType().FullName}.{memberName}");
        return null;
    }

    private static Type ResolveTypeOrFail(string fullName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(fullName, throwOnError: false);
            if (type != null)
            {
                return type;
            }
        }

        Assert.Fail($"未找到类型: {fullName}");
        return null;
    }
}
