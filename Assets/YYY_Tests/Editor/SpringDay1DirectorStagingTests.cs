using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class SpringDay1DirectorStagingTests
{
    private readonly List<UnityEngine.Object> createdObjects = new();

    [TearDown]
    public void TearDown()
    {
        for (int index = createdObjects.Count - 1; index >= 0; index--)
        {
            if (createdObjects[index] != null)
            {
                UnityEngine.Object.DestroyImmediate(createdObjects[index]);
            }
        }

        createdObjects.Clear();
        ResetStaticField(ResolveTypeOrFail("Sunset.Story.SpringDay1Director"), "_instance");
        ResetStaticField(ResolveTypeOrFail("Sunset.Story.StoryManager"), "_instance");
    }

    [Test]
    public void StageBook_ShouldResolveCueBySemanticAnchor()
    {
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "103");
        SetField(entry, "semanticAnchorIds", new[] { "KidLook_01" });

        Type cueType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorActorCue");
        object cue = Activator.CreateInstance(cueType);
        SetField(cue, "cueId", "kid-look");
        SetField(cue, "semanticAnchorId", "KidLook_01");

        Type beatEntryType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorBeatEntry");
        object beatEntry = Activator.CreateInstance(beatEntryType);
        SetField(beatEntry, "beatKey", "EnterVillage_PostEntry");
        Array cueArray = Array.CreateInstance(cueType, 1);
        cueArray.SetValue(cue, 0);
        SetField(beatEntry, "actorCues", cueArray);

        Type bookType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStageBook");
        object book = Activator.CreateInstance(bookType);
        Array beatArray = Array.CreateInstance(beatEntryType, 1);
        beatArray.SetValue(beatEntry, 0);
        SetField(book, "beats", beatArray);

        object resolvedCue = InvokeInstance(book, "TryResolveCue", "EnterVillage_PostEntry", entry);

        Assert.That(resolvedCue, Is.Not.Null);
        Assert.That(GetPropertyValue(resolvedCue, "StableKey"), Is.EqualTo("kid-look"));
    }

    [Test]
    public void StagingPlayback_ShouldPlaceNpcAtCustomStartAndExposeCueIdentity()
    {
        GameObject npc = Track(new GameObject("NPC_Staging"));
        GameObject homeAnchor = Track(new GameObject("NPC_Staging_Home"));
        Type playbackType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingPlayback");
        Type motionControllerType = ResolveTypeOrFail("NPCMotionController");
        Component playback = npc.AddComponent(playbackType);
        npc.AddComponent(motionControllerType);

        Type cueType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorActorCue");
        object cue = Activator.CreateInstance(cueType);
        SetField(cue, "cueId", "enter-start");
        SetField(cue, "keepCurrentSpawnPosition", false);
        SetField(cue, "startPosition", new Vector2(3.5f, -1.25f));
        SetField(cue, "facing", Vector2.left);
        SetField(cue, "path", Array.CreateInstance(ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorPathPoint"), 0));

        InvokeInstance(playback, "ApplyCue", "EnterVillage_PostEntry", cue, homeAnchor.transform);

        Assert.That(npc.transform.position.x, Is.EqualTo(3.5f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(-1.25f).Within(0.001f));
        Assert.That(homeAnchor.transform.position.x, Is.EqualTo(3.5f).Within(0.001f));
        Assert.That(homeAnchor.transform.position.y, Is.EqualTo(-1.25f).Within(0.001f));
        Assert.That(GetPropertyValue(playback, "CurrentCueKey"), Is.EqualTo("enter-start"));
        Assert.That(GetPropertyValue(playback, "CurrentBeatKey"), Is.EqualTo("EnterVillage_PostEntry"));
    }

    [Test]
    public void StagingPlayback_ReapplyingSameCue_ShouldNotSnapNpcBackToStart()
    {
        GameObject npc = Track(new GameObject("NPC_Staging_Reapply"));
        Type playbackType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingPlayback");
        Type motionControllerType = ResolveTypeOrFail("NPCMotionController");
        Component playback = npc.AddComponent(playbackType);
        npc.AddComponent(motionControllerType);

        Type cueType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorActorCue");
        object cue = Activator.CreateInstance(cueType);
        SetField(cue, "cueId", "steady-cue");
        SetField(cue, "keepCurrentSpawnPosition", false);
        SetField(cue, "startPosition", new Vector2(1.5f, 2f));
        SetField(cue, "facing", Vector2.right);
        SetField(cue, "path", Array.CreateInstance(ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorPathPoint"), 0));

        InvokeInstance(playback, "ApplyCue", "FreeTime_NightWitness", cue, null);
        npc.transform.position = new Vector3(4.25f, -3.5f, 0f);

        InvokeInstance(playback, "ApplyCue", "FreeTime_NightWitness", cue, null);

        Assert.That(npc.transform.position.x, Is.EqualTo(4.25f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(-3.5f).Within(0.001f));
    }

    [Test]
    public void Director_ShouldExposeFreeTimeAndDayEndBeatKeys()
    {
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type directorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_Test")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("Director_Test")).AddComponent(directorType);

        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(directorType, "_instance", director);

        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FreeTime"), false);
        Assert.That(InvokeInstance(director, "GetCurrentBeatKey"), Is.EqualTo("FreeTime_NightWitness"));

        SetField(director, "_dayEnded", true);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "DayEnd"), false);
        Assert.That(InvokeInstance(director, "GetCurrentBeatKey"), Is.EqualTo("DailyStand_Preview"));
    }

    [Test]
    public void StageBook_ShouldContainCapturedAbsolutePositionsForKeyDirectorCues()
    {
        Type databaseType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingDatabase");
        object bookAsset = InvokeStatic(databaseType, "Load", true);
        Assert.That(bookAsset, Is.Not.Null, "未能通过导演数据库加载 SpringDay1DirectorStageBook。");

        AssertCueCaptured(bookAsset, "EnterVillage_PostEntry", "enter-crowd-101");
        AssertCueCaptured(bookAsset, "DinnerConflict_Table", "dinner-bg-203");
        AssertCueCaptured(bookAsset, "FreeTime_NightWitness", "night-witness-102");
        AssertCueCaptured(bookAsset, "DailyStand_Preview", "daily-201");
    }

    [Test]
    public void StageBook_ShouldContainMultiPointPathsForRehearsedDirectorTargets()
    {
        Type databaseType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingDatabase");
        object bookAsset = InvokeStatic(databaseType, "Load", true);
        Assert.That(bookAsset, Is.Not.Null, "未能通过导演数据库加载 SpringDay1DirectorStageBook。");

        AssertCuePathCountAtLeast(bookAsset, "EnterVillage_PostEntry", "enter-crowd-101", 3);
        AssertCuePathCountAtLeast(bookAsset, "EnterVillage_PostEntry", "enter-kid-103", 3);
        AssertCuePathCountAtLeast(bookAsset, "FreeTime_NightWitness", "night-witness-102", 3);
        AssertCuePathCountAtLeast(bookAsset, "DinnerConflict_Table", "dinner-bg-203", 4);
        AssertCuePathCountAtLeast(bookAsset, "DinnerConflict_Table", "dinner-bg-104", 4);
        AssertCuePathCountAtLeast(bookAsset, "DinnerConflict_Table", "dinner-bg-201", 4);
        AssertCuePathCountAtLeast(bookAsset, "DinnerConflict_Table", "dinner-bg-202", 4);
    }

    [Test]
    public void NpcTakeover_ShouldDisableRoamAndInteractionsUntilRelease()
    {
        GameObject npc = Track(new GameObject("NPC_Takeover"));
        Type roamType = ResolveTypeOrFail("NPCAutoRoamController");
        Type informalType = ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable");
        Type dialogueType = ResolveTypeOrFail("Sunset.Story.NPCDialogueInteractable");
        Type takeoverType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorNpcTakeover");

        Behaviour roam = (Behaviour)npc.AddComponent(roamType);
        Behaviour informal = (Behaviour)npc.AddComponent(informalType);
        Behaviour dialogue = (Behaviour)npc.AddComponent(dialogueType);
        Component takeover = npc.AddComponent(takeoverType);

        Assert.That(roam.enabled, Is.True);
        Assert.That(informal.enabled, Is.True);
        Assert.That(dialogue.enabled, Is.True);

        InvokeInstance(takeover, "Acquire");

        Assert.That(roam.enabled, Is.False);
        Assert.That(informal.enabled, Is.False);
        Assert.That(dialogue.enabled, Is.False);

        InvokeInstance(takeover, "Release");

        Assert.That(roam.enabled, Is.True);
        Assert.That(informal.enabled, Is.True);
        Assert.That(dialogue.enabled, Is.True);
    }

    [Test]
    public void PlayerRehearsalLock_ShouldDisablePlayerMotionUntilRelease()
    {
        GameObject player = Track(new GameObject("Player_Test"));
        player.AddComponent<Rigidbody2D>();
        player.AddComponent<BoxCollider2D>();
        Behaviour movement = (Behaviour)player.AddComponent(ResolveTypeOrFail("PlayerMovement"));
        Behaviour controller = (Behaviour)player.AddComponent(ResolveTypeOrFail("PlayerController"));
        Behaviour navigator = (Behaviour)player.AddComponent(ResolveTypeOrFail("PlayerAutoNavigator"));

        GameObject directorTool = Track(new GameObject("Director_RehearsalLock"));
        Component lockComponent = directorTool.AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorPlayerRehearsalLock"));

        Assert.That(movement.enabled, Is.True);
        Assert.That(controller.enabled, Is.True);
        Assert.That(navigator.enabled, Is.True);

        InvokeInstance(lockComponent, "Acquire");

        Assert.That(movement.enabled, Is.False);
        Assert.That(controller.enabled, Is.False);
        Assert.That(navigator.enabled, Is.False);

        InvokeInstance(lockComponent, "Release");

        Assert.That(movement.enabled, Is.True);
        Assert.That(controller.enabled, Is.True);
        Assert.That(navigator.enabled, Is.True);
    }

    private T Track<T>(T target) where T : UnityEngine.Object
    {
        createdObjects.Add(target);
        return target;
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

    private static object ParseEnum(Type enumType, string name)
    {
        return Enum.Parse(enumType, name);
    }

    private static object InvokeStatic(Type targetType, string methodName, params object[] args)
    {
        MethodInfo method = targetType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到静态方法: {targetType.Name}.{methodName}");
        return method.Invoke(null, args);
    }

    private static void AssertCueCaptured(object bookAsset, string beatKey, string cueId)
    {
        object beat = InvokeInstance(bookAsset, "FindBeat", beatKey);
        Assert.That(beat, Is.Not.Null, $"StageBook 缺少 beat: {beatKey}");

        object cue = FindCueById(beat, cueId);
        Assert.That(cue, Is.Not.Null, $"Beat {beatKey} 缺少 cue: {cueId}");

        Assert.That((bool)GetFieldValue(cue, "keepCurrentSpawnPosition"), Is.False, $"{cueId} 不应继续依赖当前出生位。");
        Assert.That((bool)GetFieldValue(cue, "pathPointsAreOffsets"), Is.False, $"{cueId} 应该已经是绝对落位，不应继续保存 offset。");

        Vector2 startPosition = (Vector2)GetFieldValue(cue, "startPosition");
        Assert.That(startPosition.sqrMagnitude, Is.GreaterThan(0.25f), $"{cueId} 的绝对起点仍然过近原点，像是没吃到 live capture。");
    }

    private static void AssertCuePathCountAtLeast(object bookAsset, string beatKey, string cueId, int minCount)
    {
        object beat = InvokeInstance(bookAsset, "FindBeat", beatKey);
        Assert.That(beat, Is.Not.Null, $"StageBook 缺少 beat: {beatKey}");

        object cue = FindCueById(beat, cueId);
        Assert.That(cue, Is.Not.Null, $"Beat {beatKey} 缺少 cue: {cueId}");

        Array path = (Array)GetFieldValue(cue, "path");
        Assert.That(path, Is.Not.Null, $"{cueId} 缺少 path。");
        Assert.That(path.Length, Is.GreaterThanOrEqualTo(minCount), $"{cueId} 的 path 仍然过薄，像是还没完成导演排练写回。");
    }

    private static object FindCueById(object beat, string cueId)
    {
        Array cues = (Array)GetFieldValue(beat, "actorCues");
        if (cues == null)
        {
            return null;
        }

        foreach (object cue in cues)
        {
            if (cue == null)
            {
                continue;
            }

            string currentCueId = GetFieldValue(cue, "cueId") as string;
            if (string.Equals(currentCueId, cueId, StringComparison.OrdinalIgnoreCase))
            {
                return cue;
            }
        }

        return null;
    }

    private static object InvokeInstance(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return method.Invoke(target, args);
    }

    private static object GetPropertyValue(object target, string propertyName)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(property, Is.Not.Null, $"未找到属性: {target.GetType().Name}.{propertyName}");
        return property.GetValue(target);
    }

    private static object GetFieldValue(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        return field.GetValue(target);
    }

    private static void SetField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private static void SetStaticField(Type type, string fieldName, object value)
    {
        FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到静态字段: {type.Name}.{fieldName}");
        field.SetValue(null, value);
    }

    private static void ResetStaticField(Type type, string fieldName)
    {
        SetStaticField(type, fieldName, null);
    }
}
