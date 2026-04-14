using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[TestFixture]
public class SpringDay1DirectorStagingTests
{
    private readonly List<UnityEngine.Object> createdObjects = new();
    private readonly List<string> temporaryScenePaths = new();

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
        ResetStaticField(ResolveTypeOrFail("TimeManager"), "instance");
        CleanupTemporaryScenes();
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
    public void StageBook_ShouldPreferExactNpcCueBeforeSharedAnchorAndDutyFallback()
    {
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type dutyType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdSceneDuty");
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "201");
        SetField(entry, "semanticAnchorIds", new[] { "Crowd_201_01" });
        Array dutyArray = Array.CreateInstance(dutyType, 1);
        dutyArray.SetValue(ParseEnum(dutyType, "EnterVillagePostEntryCrowd"), 0);
        SetField(entry, "sceneDuties", dutyArray);

        Type cueType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorActorCue");
        object crowdCue101 = Activator.CreateInstance(cueType);
        SetField(crowdCue101, "cueId", "enter-crowd-101");
        SetField(crowdCue101, "npcId", "101");
        SetField(crowdCue101, "semanticAnchorId", "EnterVillageCrowdRoot");
        SetField(crowdCue101, "duty", ParseEnum(dutyType, "EnterVillagePostEntryCrowd"));

        object crowdCue201 = Activator.CreateInstance(cueType);
        SetField(crowdCue201, "cueId", "enter-crowd-201");
        SetField(crowdCue201, "npcId", "201");
        SetField(crowdCue201, "semanticAnchorId", "Crowd_201_01");
        SetField(crowdCue201, "duty", ParseEnum(dutyType, "EnterVillagePostEntryCrowd"));

        Type beatEntryType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorBeatEntry");
        object beatEntry = Activator.CreateInstance(beatEntryType);
        SetField(beatEntry, "beatKey", "EnterVillage_PostEntry");
        Array cueArray = Array.CreateInstance(cueType, 2);
        cueArray.SetValue(crowdCue101, 0);
        cueArray.SetValue(crowdCue201, 1);
        SetField(beatEntry, "actorCues", cueArray);

        Type bookType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStageBook");
        object book = Activator.CreateInstance(bookType);
        Array beatArray = Array.CreateInstance(beatEntryType, 1);
        beatArray.SetValue(beatEntry, 0);
        SetField(book, "beats", beatArray);

        object resolvedCue = InvokeInstance(book, "TryResolveCue", "EnterVillage_PostEntry", entry);

        Assert.That(resolvedCue, Is.Not.Null);
        Assert.That(GetPropertyValue(resolvedCue, "StableKey"), Is.EqualTo("enter-crowd-201"));
    }

    [Test]
    public void StagingPlayback_ShouldPlaceNpcAtCustomStartAndExposeCueIdentity()
    {
        GameObject npc = Track(new GameObject("NPC_Staging"));
        GameObject homeAnchor = Track(new GameObject("NPC_Staging_Home"));
        homeAnchor.transform.position = new Vector3(-7.25f, 9.5f, 0f);
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
        Assert.That(homeAnchor.transform.position.x, Is.EqualTo(-7.25f).Within(0.001f));
        Assert.That(homeAnchor.transform.position.y, Is.EqualTo(9.5f).Within(0.001f));
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
    public void StagingPlayback_ForceRestart_ShouldSnapNpcBackToStartForManualPreview()
    {
        GameObject npc = Track(new GameObject("NPC_Staging_ForceRestart"));
        Type playbackType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingPlayback");
        Type motionControllerType = ResolveTypeOrFail("NPCMotionController");
        Component playback = npc.AddComponent(playbackType);
        npc.AddComponent(motionControllerType);

        Type cueType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorActorCue");
        object cue = Activator.CreateInstance(cueType);
        SetField(cue, "cueId", "force-restart-cue");
        SetField(cue, "keepCurrentSpawnPosition", false);
        SetField(cue, "startPosition", new Vector2(-2.75f, 6.5f));
        SetField(cue, "facing", Vector2.up);
        SetField(cue, "path", Array.CreateInstance(ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorPathPoint"), 0));

        InvokeInstance(playback, "ApplyCue", "EnterVillage_PostEntry", cue, null);
        npc.transform.position = new Vector3(4.25f, -3.5f, 0f);

        InvokeInstance(playback, "ApplyCue", "EnterVillage_PostEntry", cue, null, true, true);

        Assert.That(npc.transform.position.x, Is.EqualTo(-2.75f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(6.5f).Within(0.001f));
        Assert.That((bool)GetPropertyValue(playback, "IsManualPreviewLocked"), Is.True);
    }

    [Test]
    public void StagingPlayback_ForceSnapToCueEnd_ShouldFinishCueImmediately()
    {
        GameObject npc = Track(new GameObject("NPC_Staging_ForceSnap"));
        Type playbackType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingPlayback");
        Type motionControllerType = ResolveTypeOrFail("NPCMotionController");
        Component playback = npc.AddComponent(playbackType);
        npc.AddComponent(motionControllerType);

        Type cueType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorActorCue");
        object cue = Activator.CreateInstance(cueType);
        SetField(cue, "cueId", "force-snap-cue");
        SetField(cue, "keepCurrentSpawnPosition", false);
        SetField(cue, "startPosition", new Vector2(1f, 2f));
        SetField(cue, "facing", Vector2.right);

        Type pathPointType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorPathPoint");
        object finalPoint = Activator.CreateInstance(pathPointType);
        SetField(finalPoint, "position", new Vector2(4.5f, -3.25f));
        SetField(finalPoint, "facing", Vector2.left);
        SetField(finalPoint, "holdSeconds", 0f);
        Array path = Array.CreateInstance(pathPointType, 1);
        path.SetValue(finalPoint, 0);
        SetField(cue, "path", path);

        InvokeInstance(playback, "ApplyCue", "EnterVillage_PostEntry", cue, null);

        bool snapped = (bool)InvokeInstance(playback, "ForceSnapToCueEnd");

        Assert.That(snapped, Is.True);
        Assert.That(npc.transform.position.x, Is.EqualTo(4.5f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(-3.25f).Within(0.001f));
        Assert.That((bool)InvokeInstance(playback, "HasSettledCurrentCue"), Is.True);
    }

    [Test]
    public void StagingPlayback_ShouldUseSemanticAnchorAsCueStartWhenConfigured()
    {
        GameObject anchor = Track(new GameObject("DinnerBackgroundRoot"));
        anchor.transform.position = new Vector3(-17.2f, 8.6f, 0f);

        GameObject npc = Track(new GameObject("NPC_Staging_SemanticStart"));
        GameObject homeAnchor = Track(new GameObject("NPC_Staging_SemanticStart_Home"));
        homeAnchor.transform.position = new Vector3(4.2f, -2.1f, 0f);
        Type playbackType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingPlayback");
        Type motionControllerType = ResolveTypeOrFail("NPCMotionController");
        Component playback = npc.AddComponent(playbackType);
        npc.AddComponent(motionControllerType);

        Type cueType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorActorCue");
        object cue = Activator.CreateInstance(cueType);
        SetField(cue, "cueId", "semantic-start");
        SetField(cue, "semanticAnchorId", "DinnerBackgroundRoot");
        SetField(cue, "keepCurrentSpawnPosition", false);
        SetField(cue, "useSemanticAnchorAsStart", true);
        SetField(cue, "startPosition", new Vector2(99f, 99f));
        SetField(cue, "path", Array.CreateInstance(ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorPathPoint"), 0));

        InvokeInstance(playback, "ApplyCue", "DinnerConflict_Table", cue, homeAnchor.transform);

        Assert.That(npc.transform.position.x, Is.EqualTo(-17.2f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(8.6f).Within(0.001f));
        Assert.That(homeAnchor.transform.position.x, Is.EqualTo(4.2f).Within(0.001f));
        Assert.That(homeAnchor.transform.position.y, Is.EqualTo(-2.1f).Within(0.001f));
    }

    [Test]
    public void StagingPlayback_ShouldSupportSemanticAnchorOffsetStart()
    {
        GameObject anchor = Track(new GameObject("NightWitness_01"));
        anchor.transform.position = new Vector3(12f, 7f, 0f);

        GameObject npc = Track(new GameObject("NPC_Staging_SemanticOffset"));
        Type playbackType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingPlayback");
        Type motionControllerType = ResolveTypeOrFail("NPCMotionController");
        Component playback = npc.AddComponent(playbackType);
        npc.AddComponent(motionControllerType);

        Type cueType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorActorCue");
        object cue = Activator.CreateInstance(cueType);
        SetField(cue, "cueId", "semantic-offset");
        SetField(cue, "semanticAnchorId", "NightWitness_01");
        SetField(cue, "keepCurrentSpawnPosition", false);
        SetField(cue, "useSemanticAnchorAsStart", true);
        SetField(cue, "startPositionIsSemanticAnchorOffset", true);
        SetField(cue, "startPosition", new Vector2(0.5f, -0.25f));
        SetField(cue, "path", Array.CreateInstance(ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorPathPoint"), 0));

        InvokeInstance(playback, "ApplyCue", "FreeTime_NightWitness", cue, null);

        Assert.That(npc.transform.position.x, Is.EqualTo(12.5f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(6.75f).Within(0.001f));
    }

    [Test]
    public void StagingPlayback_ShouldRebaseLegacyAbsolutePathAroundSemanticAnchorStart()
    {
        GameObject anchor = Track(new GameObject("EnterVillageCrowdRoot"));
        anchor.transform.position = new Vector3(20f, 10f, 0f);

        GameObject npc = Track(new GameObject("NPC_Staging_SemanticPath"));
        Type playbackType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingPlayback");
        Type motionControllerType = ResolveTypeOrFail("NPCMotionController");
        Component playback = npc.AddComponent(playbackType);
        npc.AddComponent(motionControllerType);

        Type cueType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorActorCue");
        object cue = Activator.CreateInstance(cueType);
        SetField(cue, "cueId", "semantic-path");
        SetField(cue, "semanticAnchorId", "EnterVillageCrowdRoot");
        SetField(cue, "keepCurrentSpawnPosition", false);
        SetField(cue, "useSemanticAnchorAsStart", true);
        SetField(cue, "startPosition", new Vector2(5f, 4f));

        Type pathPointType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorPathPoint");
        object point = Activator.CreateInstance(pathPointType);
        SetField(point, "position", new Vector2(6.5f, 4.25f));
        Array path = Array.CreateInstance(pathPointType, 1);
        path.SetValue(point, 0);
        SetField(cue, "path", path);

        InvokeInstance(playback, "ApplyCue", "EnterVillage_PostEntry", cue, null);
        Vector3 targetPosition = (Vector3)InvokeInstance(playback, "ResolveTargetPosition", point);

        Assert.That(npc.transform.position.x, Is.EqualTo(20f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(10f).Within(0.001f));
        Assert.That(targetPosition.x, Is.EqualTo(21.5f).Within(0.001f));
        Assert.That(targetPosition.y, Is.EqualTo(10.25f).Within(0.001f));
    }

    [Test]
    public void StagingPlayback_ShouldUseSemanticTargetsWithoutMovingHomeAnchorWhenKeepingCurrentSpawn()
    {
        GameObject anchor = Track(new GameObject("EnterVillageCrowdRoot"));
        anchor.transform.position = new Vector3(20f, 10f, 0f);

        GameObject npc = Track(new GameObject("NPC_Staging_CurrentSpawnSemanticTarget"));
        npc.transform.position = new Vector3(5f, 1f, 0f);
        GameObject homeAnchor = Track(new GameObject("NPC_Staging_CurrentSpawnSemanticTarget_Home"));
        homeAnchor.transform.position = new Vector3(1.5f, 2.5f, 0f);

        Type playbackType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingPlayback");
        Type motionControllerType = ResolveTypeOrFail("NPCMotionController");
        Component playback = npc.AddComponent(playbackType);
        npc.AddComponent(motionControllerType);

        Type cueType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorActorCue");
        Type pathPointType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorPathPoint");
        object cue = Activator.CreateInstance(cueType);
        object point = Activator.CreateInstance(pathPointType);
        Array path = Array.CreateInstance(pathPointType, 1);

        SetField(cue, "cueId", "current-spawn-semantic-target");
        SetField(cue, "semanticAnchorId", "EnterVillageCrowdRoot");
        SetField(cue, "keepCurrentSpawnPosition", true);
        SetField(cue, "useSemanticAnchorAsStart", true);
        SetField(cue, "startPositionIsSemanticAnchorOffset", true);
        SetField(cue, "startPosition", Vector2.zero);
        SetField(point, "position", new Vector2(1.5f, -0.5f));
        path.SetValue(point, 0);
        SetField(cue, "path", path);

        InvokeInstance(playback, "ApplyCue", "EnterVillage_PostEntry", cue, homeAnchor.transform);
        Vector3 targetPosition = (Vector3)InvokeInstance(playback, "ResolveTargetPosition", point);

        Assert.That(npc.transform.position.x, Is.EqualTo(5f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(1f).Within(0.001f));
        Assert.That(homeAnchor.transform.position.x, Is.EqualTo(1.5f).Within(0.001f));
        Assert.That(homeAnchor.transform.position.y, Is.EqualTo(2.5f).Within(0.001f));
        Assert.That(targetPosition.x, Is.EqualTo(21.5f).Within(0.001f));
        Assert.That(targetPosition.y, Is.EqualTo(9.5f).Within(0.001f));
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
    public void StageBook_ShouldContainCapturedAbsolutePositionsForTownOpeningCrowdCues()
    {
        Type databaseType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingDatabase");
        object bookAsset = InvokeStatic(databaseType, "Load", true);
        Assert.That(bookAsset, Is.Not.Null, "未能通过导演数据库加载 SpringDay1DirectorStageBook。");

        AssertCueCaptured(bookAsset, "EnterVillage_PostEntry", "101");
        AssertCueCaptured(bookAsset, "EnterVillage_PostEntry", "103");
        AssertCueCaptured(bookAsset, "EnterVillage_PostEntry", "104");
        AssertCueCaptured(bookAsset, "EnterVillage_PostEntry", "201");
        AssertCueCaptured(bookAsset, "EnterVillage_PostEntry", "202");
        AssertCueCaptured(bookAsset, "EnterVillage_PostEntry", "203");
        AssertCueCaptured(bookAsset, "EnterVillage_PostEntry", "102");
    }

    [Test]
    public void StageBook_ShouldContainCapturedAbsolutePositionsForNonOpeningDirectorCues()
    {
        Type databaseType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingDatabase");
        object bookAsset = InvokeStatic(databaseType, "Load", true);
        Assert.That(bookAsset, Is.Not.Null, "未能通过导演数据库加载 SpringDay1DirectorStageBook。");

        AssertCueCaptured(bookAsset, "DinnerConflict_Table", "dinner-bg-203");
        AssertCueCaptured(bookAsset, "FreeTime_NightWitness", "night-witness-102");
        AssertCueCaptured(bookAsset, "DinnerConflict_Table", "dinner-bg-201");
    }

    [Test]
    public void StageBook_ShouldContainMultiPointPathsForRehearsedDirectorTargets()
    {
        Type databaseType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingDatabase");
        object bookAsset = InvokeStatic(databaseType, "Load", true);
        Assert.That(bookAsset, Is.Not.Null, "未能通过导演数据库加载 SpringDay1DirectorStageBook。");

        AssertCuePathProgression(bookAsset, "EnterVillage_PostEntry", "101", 1, 1);
        AssertCuePathProgression(bookAsset, "EnterVillage_PostEntry", "103", 1, 1);
        AssertCuePathProgression(bookAsset, "EnterVillage_PostEntry", "104", 1, 1);
        AssertCuePathProgression(bookAsset, "EnterVillage_PostEntry", "201", 1, 1);
        AssertCuePathProgression(bookAsset, "EnterVillage_PostEntry", "202", 1, 1);
        AssertCuePathProgression(bookAsset, "EnterVillage_PostEntry", "203", 1, 1);
        AssertCuePathProgression(bookAsset, "EnterVillage_PostEntry", "102", 1, 1);
        AssertCuePathProgression(bookAsset, "FreeTime_NightWitness", "night-witness-102", 3, 2);

        AssertCuePathProgression(bookAsset, "DinnerConflict_Table", "dinner-bg-203", 4, 3);
        AssertCuePathProgression(bookAsset, "DinnerConflict_Table", "dinner-bg-104", 4, 3);
        AssertCuePathProgression(bookAsset, "DinnerConflict_Table", "dinner-bg-201", 4, 3);
        AssertCuePathProgression(bookAsset, "DinnerConflict_Table", "dinner-bg-202", 4, 3);
    }

    [Test]
    public void StageBook_ShouldMarkMigratedTownCuesToUseSemanticAnchorStarts()
    {
        Type databaseType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingDatabase");
        object bookAsset = InvokeStatic(databaseType, "Load", true);
        Assert.That(bookAsset, Is.Not.Null, "未能通过导演数据库加载 SpringDay1DirectorStageBook。");

        AssertCueUsesSemanticAnchorStart(bookAsset, "ReturnAndReminder_WalkBack", "reminder-bg-203");
        AssertCueUsesSemanticAnchorStart(bookAsset, "ReturnAndReminder_WalkBack", "reminder-bg-201");
        AssertCueUsesSemanticAnchorStart(bookAsset, "DailyStand_Preview", "daily-101");
        AssertCueUsesSemanticAnchorStart(bookAsset, "DailyStand_Preview", "daily-103");
        AssertCueUsesSemanticAnchorStart(bookAsset, "DailyStand_Preview", "daily-102");
        AssertCueUsesSemanticAnchorStart(bookAsset, "DailyStand_Preview", "daily-104");
        AssertCueUsesSemanticAnchorStart(bookAsset, "DailyStand_Preview", "daily-201");
        AssertCueUsesSemanticAnchorStart(bookAsset, "DailyStand_Preview", "daily-203");
    }

    [Test]
    public void CrowdDirector_ShouldPreferSemanticAnchorBeforeLegacyAnchor()
    {
        GameObject semanticAnchor = Track(new GameObject("EnterVillageCrowdRoot"));
        semanticAnchor.transform.position = new Vector3(6.5f, -4.25f, 0f);

        GameObject legacyAnchor = Track(new GameObject("001"));
        legacyAnchor.transform.position = new Vector3(1.25f, 2.75f, 0f);

        Component director = Track(new GameObject("CrowdDirector_Test")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "101");
        SetField(entry, "semanticAnchorIds", new[] { "EnterVillageCrowdRoot" });
        SetField(entry, "anchorObjectName", "001");
        SetField(entry, "spawnOffset", Vector2.zero);
        SetField(entry, "fallbackWorldPosition", Vector2.zero);

        object resolution = InvokeInstance(director, "ResolveSpawnPoint", entry, "EnterVillage_PostEntry");
        Vector3 position = (Vector3)GetPropertyValue(resolution, "Position");
        string anchorName = (string)GetPropertyValue(resolution, "AnchorName");
        bool usedFallback = (bool)GetPropertyValue(resolution, "UsedFallback");

        Assert.That(usedFallback, Is.False);
        Assert.That(anchorName, Is.EqualTo("EnterVillageCrowdRoot"));
        Assert.That(position.x, Is.EqualTo(6.5f).Within(0.001f));
        Assert.That(position.y, Is.EqualTo(-4.25f).Within(0.001f));
    }

    [Test]
    public void CrowdDirector_ShouldFallBackToTownAnchorContractWhenSemanticAnchorIsNotInLoadedScene()
    {
        Component director = Track(new GameObject("CrowdDirector_Test")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        AssertTownContractResolution(director, "102", "NightWitness_01", "FreeTime_NightWitness", new Vector2(-6.4f, 18.7f));
    }

    [Test]
    public void CrowdDirector_ShouldResolveSemanticAnchorFromDirectorReadyAliasBeforeTownContract()
    {
        GameObject directorReadyAnchor = Track(new GameObject("DirectorReady_EnterVillageCrowdRoot"));
        directorReadyAnchor.transform.position = new Vector3(-31.2f, 14.6f, 0f);

        Component director = Track(new GameObject("CrowdDirector_DirectorReadyAlias")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "101");
        SetField(entry, "semanticAnchorIds", new[] { "EnterVillageCrowdRoot" });
        SetField(entry, "anchorObjectName", string.Empty);
        SetField(entry, "spawnOffset", Vector2.zero);
        SetField(entry, "fallbackWorldPosition", Vector2.zero);

        object resolution = InvokeInstance(director, "ResolveSpawnPoint", entry, "EnterVillage_PostEntry");
        Vector3 position = (Vector3)GetPropertyValue(resolution, "Position");
        string anchorName = (string)GetPropertyValue(resolution, "AnchorName");
        bool usedFallback = (bool)GetPropertyValue(resolution, "UsedFallback");

        Assert.That(usedFallback, Is.False);
        Assert.That(anchorName, Is.EqualTo("DirectorReady_EnterVillageCrowdRoot"));
        Assert.That(position.x, Is.EqualTo(-31.2f).Within(0.001f));
        Assert.That(position.y, Is.EqualTo(14.6f).Within(0.001f));
    }

    [Test]
    public void CrowdDirector_ShouldPreferCurrentSceneResidentStartForVillageCrowdCueWhenMarkersAreMissing()
    {
        Type directorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type cueType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorActorCue");

        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "101");

        object cue = Activator.CreateInstance(cueType);
        SetField(cue, "cueId", "legacy-enter-crowd");
        SetField(cue, "keepCurrentSpawnPosition", false);
        SetField(cue, "useSemanticAnchorAsStart", false);
        SetField(cue, "startPosition", new Vector2(-4.71f, 16.65f));
        SetField(cue, "path", Array.CreateInstance(ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorPathPoint"), 0));

        object resolvedCue = InvokeStatic(directorType, "ResolveRuntimeCueOverride", "EnterVillage_PostEntry", entry, cue);

        Assert.That(resolvedCue, Is.Not.Null);
        Assert.That((bool)GetFieldValue(resolvedCue, "keepCurrentSpawnPosition"), Is.True);
        Assert.That((bool)GetFieldValue(resolvedCue, "useSemanticAnchorAsStart"), Is.False);
        Assert.That((Vector2)GetFieldValue(resolvedCue, "startPosition"), Is.EqualTo(new Vector2(-4.71f, 16.65f)));
    }

    [Test]
    public void CrowdDirector_ShouldResolveDailyStandTownContractAnchorsWhenSceneAnchorIsMissing()
    {
        Component director = Track(new GameObject("CrowdDirector_TownContract")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));

        AssertTownContractResolution(director, "103", "DailyStand_02", "DailyStand_Preview", new Vector2(-9.4f, 1.5f));
        AssertTownContractResolution(director, "102", "DailyStand_03", "DailyStand_Preview", new Vector2(-3.6f, 4.1f));
    }

    [Test]
    public void CrowdDirector_ShouldBindExistingResidentRootsWithoutProvisioningRuntimeOnes()
    {
        Scene tempScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject residentRoot = Track(new GameObject("Town_Day1Residents"));
        GameObject residentDefault = Track(new GameObject("Resident_DefaultPresent"));
        GameObject residentTakeover = Track(new GameObject("Resident_DirectorTakeoverReady"));
        GameObject residentBackstage = Track(new GameObject("Resident_BackstagePresent"));
        GameObject carrierRoot = Track(new GameObject("Town_Day1Carriers"));
        SceneManager.MoveGameObjectToScene(residentRoot, tempScene);
        SceneManager.MoveGameObjectToScene(residentDefault, tempScene);
        SceneManager.MoveGameObjectToScene(residentTakeover, tempScene);
        SceneManager.MoveGameObjectToScene(residentBackstage, tempScene);
        SceneManager.MoveGameObjectToScene(carrierRoot, tempScene);
        residentDefault.transform.SetParent(residentRoot.transform, false);
        residentTakeover.transform.SetParent(residentRoot.transform, false);
        residentBackstage.transform.SetParent(residentRoot.transform, false);

        Component director = Track(new GameObject("CrowdDirector_Test")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        InvokeInstance(director, "EnsureSceneRoot", tempScene);

        Assert.That(GameObject.Find("SpringDay1NpcCrowdRuntimeRoot"), Is.Null, "CrowdDirector 不应再在 scene 中自造 runtime root。");
        Assert.That(GetFieldValue(director, "_residentRoot"), Is.EqualTo(residentRoot));
        Assert.That(GetFieldValue(director, "_residentDefaultRoot"), Is.EqualTo(residentDefault));
        Assert.That(GetFieldValue(director, "_residentTakeoverReadyRoot"), Is.EqualTo(residentTakeover));
        Assert.That(GetFieldValue(director, "_residentBackstageRoot"), Is.EqualTo(residentBackstage));
        Assert.That(GetFieldValue(director, "_carrierRoot"), Is.EqualTo(carrierRoot));
    }

    [Test]
    public void CrowdDirector_ShouldBindSceneResidentInTownSceneWithoutRuntimeSpawn()
    {
        Scene townScene = CreateNamedTestScene("Town");
        Component director = Track(new GameObject("CrowdDirector_TownScene")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject residentRoot = Track(new GameObject("Town_Day1Residents"));
        GameObject residentDefault = Track(new GameObject("Resident_DefaultPresent"));
        GameObject resident = Track(new GameObject("103"));
        GameObject homeAnchor = Track(new GameObject("103_HomeAnchor"));
        SceneManager.MoveGameObjectToScene(residentRoot, townScene);
        SceneManager.MoveGameObjectToScene(residentDefault, townScene);
        SceneManager.MoveGameObjectToScene(resident, townScene);
        SceneManager.MoveGameObjectToScene(homeAnchor, townScene);
        residentDefault.transform.SetParent(residentRoot.transform, false);
        resident.transform.position = new Vector3(-9.4f, 1.5f, 0f);
        homeAnchor.transform.position = resident.transform.position;
        InvokeInstance(director, "EnsureSceneRoot", townScene);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type beatSnapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionSnapshot");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");

        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "103");
        SetField(entry, "prefab", resident);
        SetField(entry, "semanticAnchorIds", new[] { "DailyStand_02" });
        SetField(entry, "anchorObjectName", string.Empty);
        SetField(entry, "spawnOffset", Vector2.zero);
        SetField(entry, "fallbackWorldPosition", Vector2.zero);
        SetField(entry, "initialFacing", Vector2.left);
        SetField(entry, "residentBaseline", Enum.Parse(residentBaselineType, "DaytimeResident"));

        object beatConsumption = Activator.CreateInstance(beatSnapshotType);
        object state = InvokeInstance(director, "GetOrCreateState", entry, "DailyStand_Preview", beatConsumption);

        Assert.That(state, Is.Not.Null, "Town 已有原生 resident 时，Day1 应直接绑定 scene resident。");

        GameObject instance = (GameObject)GetFieldValue(state, "Instance");
        GameObject boundHomeAnchor = (GameObject)GetFieldValue(state, "HomeAnchor");
        Assert.That(instance, Is.Not.Null);
        Assert.That(boundHomeAnchor, Is.Not.Null);
        Assert.That(instance.scene.name, Is.EqualTo("Town"));
        Assert.That(boundHomeAnchor.scene.name, Is.EqualTo("Town"));
        Assert.That(instance, Is.EqualTo(resident), "不应再 runtime Instantiate 一个新 resident。");
        Assert.That(boundHomeAnchor, Is.EqualTo(homeAnchor), "不应再 runtime 创建 HomeAnchor 替身。");
        Assert.That(instance.transform.position.x, Is.EqualTo(-9.4f).Within(0.001f));
        Assert.That(instance.transform.position.y, Is.EqualTo(1.5f).Within(0.001f));
        Assert.That((string)GetFieldValue(state, "ResolvedAnchorName"), Is.EqualTo("103"));
        Assert.That((bool)GetFieldValue(state, "OwnsInstance"), Is.False);
        Assert.That((bool)GetFieldValue(state, "OwnsHomeAnchor"), Is.False);
        Assert.That(GameObject.Find("SpringDay1NpcCrowdRuntimeRoot"), Is.Null);
    }

    [Test]
    public void CrowdDirector_ShouldCaptureAndReapplyResidentRuntimeSnapshot()
    {
        Scene townScene = CreateNamedTestScene("Town");
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Component director = Track(new GameObject("CrowdDirector_Snapshot")).AddComponent(crowdDirectorType);
        GameObject residentRoot = Track(new GameObject("Town_Day1Residents"));
        GameObject residentDefault = Track(new GameObject("Resident_DefaultPresent"));
        GameObject resident = Track(new GameObject("103"));
        GameObject homeAnchor = Track(new GameObject("103_HomeAnchor"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));

        SceneManager.MoveGameObjectToScene(residentRoot, townScene);
        SceneManager.MoveGameObjectToScene(residentDefault, townScene);
        SceneManager.MoveGameObjectToScene(resident, townScene);
        SceneManager.MoveGameObjectToScene(homeAnchor, townScene);
        residentDefault.transform.SetParent(residentRoot.transform, false);
        resident.transform.SetParent(residentDefault.transform, true);
        resident.transform.position = new Vector3(8.25f, -2.75f, 0f);
        homeAnchor.transform.position = new Vector3(2f, 1f, 0f);
        InvokeInstance(director, "EnsureSceneRoot", townScene);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type beatSnapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionSnapshot");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "103");
        SetField(entry, "prefab", resident);
        SetField(entry, "semanticAnchorIds", new[] { "DailyStand_02" });
        SetField(entry, "anchorObjectName", string.Empty);
        SetField(entry, "spawnOffset", Vector2.zero);
        SetField(entry, "fallbackWorldPosition", Vector2.zero);
        SetField(entry, "initialFacing", Vector2.left);
        SetField(entry, "residentBaseline", Enum.Parse(residentBaselineType, "DaytimeResident"));

        object beatConsumption = Activator.CreateInstance(beatSnapshotType);
        object state = InvokeInstance(director, "GetOrCreateState", entry, "DailyStand_Preview", beatConsumption);
        SetField(state, "ResidentGroupKey", "Resident_DefaultPresent");
        SetField(state, "ResolvedAnchorName", "resident:Resident_DefaultPresent:DailyStand_Preview");

        System.Collections.IList snapshots = (System.Collections.IList)InvokeStatic(crowdDirectorType, "CaptureResidentRuntimeSnapshots");
        Assert.That(snapshots, Is.Not.Null);
        Assert.That(snapshots.Count, Is.EqualTo(1));

        object snapshot = snapshots[0];
        Assert.That(GetFieldValue(snapshot, "npcId"), Is.EqualTo("103"));
        Assert.That(GetFieldValue(snapshot, "sceneName"), Is.EqualTo("Town"));
        Assert.That(GetFieldValue(snapshot, "residentGroupKey"), Is.EqualTo("Resident_DefaultPresent"));

        resident.transform.position = new Vector3(-9.4f, 1.5f, 0f);
        SetField(state, "ResidentGroupKey", string.Empty);
        SetField(state, "ResolvedAnchorName", "stale-anchor");

        InvokeStatic(crowdDirectorType, "ApplyResidentRuntimeSnapshots", snapshots);

        Assert.That(resident.transform.position.x, Is.EqualTo(8.25f).Within(0.001f));
        Assert.That(resident.transform.position.y, Is.EqualTo(-2.75f).Within(0.001f));
        Assert.That((string)GetFieldValue(state, "ResidentGroupKey"), Is.EqualTo("Resident_DefaultPresent"));
        Assert.That((string)GetFieldValue(state, "ResolvedAnchorName"), Is.EqualTo("resident:Resident_DefaultPresent:DailyStand_Preview"));
    }

    [Test]
    public void CrowdDirector_ShouldNotPersistStoryDrivenReturnHomeSnapshotBeforeFreeTime()
    {
        Scene townScene = CreateNamedTestScene("Town");
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Component storyManager = Track(new GameObject("StoryManager_Snapshot")).AddComponent(storyManagerType);
        Component springDirector = Track(new GameObject("SpringDay1Director_Snapshot")).AddComponent(springDirectorType);
        Component crowdDirector = Track(new GameObject("CrowdDirector_StoryReturnSnapshot")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", springDirector);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "EnterVillage"), false);
        SetField(springDirector, "_townHouseLeadStarted", true);

        GameObject residentRoot = Track(new GameObject("Town_Day1Residents"));
        GameObject residentDefault = Track(new GameObject("Resident_DefaultPresent"));
        GameObject resident = Track(new GameObject("103"));
        GameObject homeAnchor = Track(new GameObject("103_HomeAnchor"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        SceneManager.MoveGameObjectToScene(residentRoot, townScene);
        SceneManager.MoveGameObjectToScene(residentDefault, townScene);
        SceneManager.MoveGameObjectToScene(resident, townScene);
        SceneManager.MoveGameObjectToScene(homeAnchor, townScene);
        residentDefault.transform.SetParent(residentRoot.transform, false);
        resident.transform.SetParent(residentDefault.transform, true);
        resident.transform.position = new Vector3(8.25f, -2.75f, 0f);
        homeAnchor.transform.position = new Vector3(2f, 1f, 0f);
        InvokeInstance(crowdDirector, "EnsureSceneRoot", townScene);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type beatSnapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionSnapshot");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "103");
        SetField(entry, "prefab", resident);
        SetField(entry, "semanticAnchorIds", new[] { "KidLook_01" });
        SetField(entry, "anchorObjectName", string.Empty);
        SetField(entry, "spawnOffset", Vector2.zero);
        SetField(entry, "fallbackWorldPosition", Vector2.zero);
        SetField(entry, "initialFacing", Vector2.left);
        SetField(entry, "residentBaseline", Enum.Parse(residentBaselineType, "DaytimeResident"));

        object beatConsumption = Activator.CreateInstance(beatSnapshotType);
        object state = InvokeInstance(crowdDirector, "GetOrCreateState", entry, "EnterVillage_HouseArrival", beatConsumption);
        SetField(state, "ResidentGroupKey", "Resident_DefaultPresent");
        SetField(state, "ResolvedAnchorName", "resident:Resident_DefaultPresent:EnterVillage_HouseArrival:return-home");
        SetField(state, "BaseResolvedAnchorName", "KidLook_01");
        SetField(state, "BaseFacing", Vector2.left);
        SetField(state, "IsReturningHome", true);
        SetField(state, "ResumeRoamAfterReturn", true);
        InvokeStatic(crowdDirectorType, "AcquireResidentDirectorControl", state, true);
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.True);

        System.Collections.IList snapshots = (System.Collections.IList)InvokeStatic(crowdDirectorType, "CaptureResidentRuntimeSnapshots");
        Assert.That(snapshots, Is.Not.Null);
        Assert.That(snapshots.Count, Is.EqualTo(1));
        Assert.That((bool)GetFieldValue(snapshots[0], "isReturningHome"), Is.False,
            "EnterVillage/HouseArrival 这种白天剧情散场态不应跨场景持久化成 return-home。");
    }

    [Test]
    public void CrowdDirector_ShouldIgnoreStaleStoryDrivenReturnHomeSnapshotBeforeFreeTime()
    {
        Scene townScene = CreateNamedTestScene("Town");
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Component storyManager = Track(new GameObject("StoryManager_SnapshotRestore")).AddComponent(storyManagerType);
        Component springDirector = Track(new GameObject("SpringDay1Director_SnapshotRestore")).AddComponent(springDirectorType);
        Component crowdDirector = Track(new GameObject("CrowdDirector_StoryReturnRestore")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", springDirector);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "EnterVillage"), false);
        SetField(springDirector, "_townHouseLeadStarted", true);

        GameObject residentRoot = Track(new GameObject("Town_Day1Residents"));
        GameObject residentDefault = Track(new GameObject("Resident_DefaultPresent"));
        GameObject resident = Track(new GameObject("103"));
        GameObject homeAnchor = Track(new GameObject("103_HomeAnchor"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        SceneManager.MoveGameObjectToScene(residentRoot, townScene);
        SceneManager.MoveGameObjectToScene(residentDefault, townScene);
        SceneManager.MoveGameObjectToScene(resident, townScene);
        SceneManager.MoveGameObjectToScene(homeAnchor, townScene);
        residentDefault.transform.SetParent(residentRoot.transform, false);
        resident.transform.SetParent(residentDefault.transform, true);
        resident.transform.position = new Vector3(8.25f, -2.75f, 0f);
        homeAnchor.transform.position = new Vector3(2f, 1f, 0f);
        InvokeInstance(crowdDirector, "EnsureSceneRoot", townScene);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type beatSnapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionSnapshot");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "103");
        SetField(entry, "prefab", resident);
        SetField(entry, "semanticAnchorIds", new[] { "KidLook_01" });
        SetField(entry, "anchorObjectName", string.Empty);
        SetField(entry, "spawnOffset", Vector2.zero);
        SetField(entry, "fallbackWorldPosition", Vector2.zero);
        SetField(entry, "initialFacing", Vector2.left);
        SetField(entry, "residentBaseline", Enum.Parse(residentBaselineType, "DaytimeResident"));

        object beatConsumption = Activator.CreateInstance(beatSnapshotType);
        object state = InvokeInstance(crowdDirector, "GetOrCreateState", entry, "EnterVillage_HouseArrival", beatConsumption);
        SetField(state, "ResidentGroupKey", "Resident_DefaultPresent");
        SetField(state, "ResolvedAnchorName", "resident:Resident_DefaultPresent:EnterVillage_HouseArrival");
        SetField(state, "BaseResolvedAnchorName", "KidLook_01");
        SetField(state, "BaseFacing", Vector2.left);

        Type snapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+ResidentRuntimeSnapshotEntry");
        object staleSnapshot = Activator.CreateInstance(snapshotType);
        SetField(staleSnapshot, "npcId", "103");
        SetField(staleSnapshot, "sceneName", "Town");
        SetField(staleSnapshot, "residentGroupKey", "Resident_DefaultPresent");
        SetField(staleSnapshot, "resolvedAnchorName", "resident:Resident_DefaultPresent:EnterVillage_HouseArrival:return-home");
        SetField(staleSnapshot, "positionX", 8.25f);
        SetField(staleSnapshot, "positionY", -2.75f);
        SetField(staleSnapshot, "facingX", -1f);
        SetField(staleSnapshot, "facingY", 0f);
        SetField(staleSnapshot, "isActive", true);
        SetField(staleSnapshot, "isReturningHome", true);
        SetField(staleSnapshot, "underDirectorCue", false);

        System.Collections.IList snapshots = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(snapshotType));
        snapshots.Add(staleSnapshot);
        InvokeStatic(crowdDirectorType, "ApplyResidentRuntimeSnapshots", snapshots);

        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.False,
            "旧档或旧缓存里的白天 return-home 不应再把 resident 恢复成冻结散场态。");
        Assert.That((bool)GetFieldValue(state, "NeedsResidentReset"), Is.False);
        Assert.That((bool)GetFieldValue(state, "ReleasedFromDirectorCue"), Is.False);
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.False,
            "清洗掉 stale return-home 后，resident 不应继续被导演 scripted control 持有。");
    }

    [Test]
    public void CrowdDirector_ShouldTreatDayResidentsAsAlreadyAroundBeforeEnterVillage()
    {
        Component director = Track(new GameObject("CrowdDirector_Test")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type beatSnapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionSnapshot");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");

        object dayResident = Activator.CreateInstance(entryType);
        SetField(dayResident, "residentBaseline", Enum.Parse(residentBaselineType, "DaytimeResident"));

        object nightResident = Activator.CreateInstance(entryType);
        SetField(nightResident, "residentBaseline", Enum.Parse(residentBaselineType, "NightResident"));
        object beatConsumption = Activator.CreateInstance(beatSnapshotType);

        bool daytimeVisible = (bool)InvokeInstance(
            director,
            "ShouldKeepResidentActive",
            dayResident,
            Enum.Parse(storyPhaseType, "CrashAndMeet"),
            string.Empty,
            beatConsumption);

        bool nightVisible = (bool)InvokeInstance(
            director,
            "ShouldKeepResidentActive",
            nightResident,
            Enum.Parse(storyPhaseType, "CrashAndMeet"),
            string.Empty,
            beatConsumption);

        Assert.That(daytimeVisible, Is.True, "白天常驻居民在进村前应已算存在。");
        Assert.That(nightVisible, Is.False, "夜间居民在进村前应先留在 backstage / 隐身承接。");
    }

    [Test]
    public void CrowdDirector_ShouldSuppressEnterVillagePostEntryCrowdCueWhenTownHouseLeadPending()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type dialogueManagerType = ResolveTypeOrFail("Sunset.Story.DialogueManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_EnterVillageLead")).AddComponent(storyManagerType);
        Component springDirector = Track(new GameObject("SpringDay1Director_EnterVillageLead")).AddComponent(springDirectorType);
        Component dialogueManager = Track(new GameObject("DialogueManager_EnterVillageLead")).AddComponent(dialogueManagerType);
        Track(new GameObject("CrowdDirector_EnterVillageLead")).AddComponent(crowdDirectorType);

        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", springDirector);
        SetStaticField(dialogueManagerType, "<Instance>k__BackingField", dialogueManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "EnterVillage"), false);
        AddCompletedSequenceId(dialogueManager, "spring-day1-village-gate");

        bool shouldSuppress = (bool)InvokeStatic(
            crowdDirectorType,
            "ShouldSuppressEnterVillageCrowdCueForTownHouseLead",
            "EnterVillage_PostEntry");

        bool shouldKeepOtherBeat = (bool)InvokeStatic(
            crowdDirectorType,
            "ShouldSuppressEnterVillageCrowdCueForTownHouseLead",
            "EnterVillage_HouseArrival");

        Assert.That(shouldSuppress, Is.True, "村长开始引路后，EnterVillage_PostEntry 那套围观 crowd cue 应放人，不该继续把常驻居民扣成群演。");
        Assert.That(shouldKeepOtherBeat, Is.False, "抑制条件只应卡 EnterVillage_PostEntry，不应误伤其它 beat。");
    }

    [Test]
    public void CrowdDirector_ShouldReleaseEnterVillagePostEntryCueOnlyAfterTownHouseLeadActuallyStarts()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type dialogueManagerType = ResolveTypeOrFail("Sunset.Story.DialogueManager");
        Type dialogueSequenceType = ResolveTypeOrFail("Sunset.Story.DialogueSequenceSO");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_EnterVillageCueWindow")).AddComponent(storyManagerType);
        Component springDirector = Track(new GameObject("SpringDay1Director_EnterVillageCueWindow")).AddComponent(springDirectorType);
        Component dialogueManager = Track(new GameObject("DialogueManager_EnterVillageCueWindow")).AddComponent(dialogueManagerType);
        Track(new GameObject("CrowdDirector_EnterVillageCueWindow")).AddComponent(crowdDirectorType);

        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", springDirector);
        SetStaticField(dialogueManagerType, "<Instance>k__BackingField", dialogueManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "EnterVillage"), false);

        bool suppressBeforeDialogue = (bool)InvokeStatic(
            crowdDirectorType,
            "ShouldSuppressEnterVillageCrowdCueForTownHouseLead",
            "EnterVillage_PostEntry");

        ScriptableObject activeSequence = Track(ScriptableObject.CreateInstance(dialogueSequenceType) as ScriptableObject);
        SetField(activeSequence, "sequenceId", "spring-day1-village-gate");
        SetField(dialogueManager, "_currentSequence", activeSequence);
        SetField(dialogueManager, "<IsDialogueActive>k__BackingField", true);

        bool suppressDuringDialogue = (bool)InvokeStatic(
            crowdDirectorType,
            "ShouldSuppressEnterVillageCrowdCueForTownHouseLead",
            "EnterVillage_PostEntry");

        SetField(dialogueManager, "_currentSequence", null);
        SetField(dialogueManager, "<IsDialogueActive>k__BackingField", false);
        SetField(springDirector, "_townHouseLeadWaitingForPlayer", true);

        bool suppressAfterLeadStarts = (bool)InvokeStatic(
            crowdDirectorType,
            "ShouldSuppressEnterVillageCrowdCueForTownHouseLead",
            "EnterVillage_PostEntry");

        Assert.That(suppressBeforeDialogue, Is.False, "VillageGate 正式对白前，EnterVillage_PostEntry 仍应继续托住 opening cue，不该提前放人。");
        Assert.That(suppressDuringDialogue, Is.False, "VillageGate 对白进行中也不该提前 suppress 掉 EnterVillage_PostEntry。");
        Assert.That(suppressAfterLeadStarts, Is.True, "只有 Town house lead 真正启动后，EnterVillage_PostEntry 才该释放 resident。");
    }

    [Test]
    public void CrowdDirector_ShouldNotResnapResidentToBasePositionOnRepeatedBaselineSync()
    {
        Component director = Track(new GameObject("CrowdDirector_Test")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject runtimeRoot = Track(new GameObject("SpringDay1NpcCrowdRuntimeRoot"));
        GameObject residentRoot = Track(new GameObject("Town_Day1Residents"));
        GameObject defaultRoot = Track(new GameObject("Resident_DefaultPresent"));

        residentRoot.transform.SetParent(runtimeRoot.transform, false);
        defaultRoot.transform.SetParent(residentRoot.transform, false);

        SetField(director, "_sceneRoot", runtimeRoot);
        SetField(director, "_residentRoot", residentRoot);
        SetField(director, "_residentDefaultRoot", defaultRoot);

        GameObject npc = Track(new GameObject("ResidentNpc"));
        GameObject homeAnchor = Track(new GameObject("ResidentNpc_HomeAnchor"));
        npc.transform.SetParent(defaultRoot.transform, true);
        homeAnchor.transform.SetParent(defaultRoot.transform, true);
        npc.transform.position = new Vector3(8.75f, -3.5f, 0f);
        homeAnchor.transform.position = npc.transform.position;

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BasePosition", new Vector3(1.5f, 2.25f, 0f));
        SetField(state, "BaseFacing", Vector2.down);
        SetField(state, "ResidentGroupKey", "Resident_DefaultPresent");
        SetField(state, "NeedsResidentReset", false);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type beatSnapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionSnapshot");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");
        object entry = Activator.CreateInstance(entryType);
        object beatConsumption = Activator.CreateInstance(beatSnapshotType);
        SetField(entry, "residentBaseline", Enum.Parse(residentBaselineType, "DaytimeResident"));

        InvokeInstance(
            director,
            "ApplyResidentBaseline",
            entry,
            state,
            Enum.Parse(storyPhaseType, "FreeTime"),
            string.Empty,
            beatConsumption);

        Assert.That(npc.transform.position.x, Is.EqualTo(8.75f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(-3.5f).Within(0.001f));
        Assert.That(homeAnchor.transform.position.x, Is.EqualTo(8.75f).Within(0.001f));
        Assert.That(homeAnchor.transform.position.y, Is.EqualTo(-3.5f).Within(0.001f));
        Assert.That(npc.transform.parent, Is.EqualTo(defaultRoot.transform));
        Assert.That((bool)GetFieldValue(state, "NeedsResidentReset"), Is.False);
    }

    [Test]
    public void CrowdDirector_ShouldReleaseResidentBackToBasePoseAfterCueReleaseBeforeFreeTime()
    {
        Component director = Track(new GameObject("CrowdDirector_DaytimeBaselineRelease")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject npc = Track(new GameObject("Resident_DaytimeRelease"));
        GameObject homeAnchor = Track(new GameObject("Resident_DaytimeRelease_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        npc.transform.position = new Vector3(6.5f, -2.25f, 0f);
        homeAnchor.transform.position = new Vector3(14.5f, -6.75f, 0f);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_Returning_HomeAnchor");
        SetField(state, "BasePosition", new Vector3(2.5f, 4.75f, 0f));
        SetField(state, "BaseFacing", Vector2.left);
        SetField(state, "NeedsResidentReset", true);
        SetField(state, "ReleasedFromDirectorCue", true);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type beatSnapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionSnapshot");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");
        object entry = Activator.CreateInstance(entryType);
        object beatConsumption = Activator.CreateInstance(beatSnapshotType);
        SetField(entry, "residentBaseline", Enum.Parse(residentBaselineType, "DaytimeResident"));
        SetField(entry, "semanticAnchorIds", new[] { "EnterVillageCrowdRoot", "DailyStand_02" });

        InvokeInstance(
            director,
            "ApplyResidentBaseline",
            entry,
            state,
            Enum.Parse(storyPhaseType, "EnterVillage"),
            "EnterVillage_HouseArrival",
            beatConsumption);

        Assert.That(npc.transform.position.x, Is.EqualTo(2.5f).Within(0.01f));
        Assert.That(npc.transform.position.y, Is.EqualTo(4.75f).Within(0.01f));
        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.False, "白天围观收束后不应立刻送 resident 回家。");
        Assert.That((bool)GetFieldValue(state, "NeedsResidentReset"), Is.False);
        Assert.That((bool)GetFieldValue(state, "ReleasedFromDirectorCue"), Is.False);
        Assert.That(((Vector3)GetFieldValue(state, "BasePosition")).x, Is.EqualTo(2.5f).Within(0.01f));
        Assert.That(((Vector3)GetFieldValue(state, "BasePosition")).y, Is.EqualTo(4.75f).Within(0.01f));
        Assert.That((string)GetFieldValue(state, "BaseResolvedAnchorName"), Is.EqualTo("Resident_Returning_HomeAnchor"));
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.False, "白天 baseline 恢复后，导演不应继续持有 resident scripted control。");

        Vector2 roamCenterAfterRelease = (Vector2)GetPropertyValue(roamController, "DebugRoamCenter");
        Assert.That(roamCenterAfterRelease.x, Is.EqualTo(2.5f).Within(0.01f));
        Assert.That(roamCenterAfterRelease.y, Is.EqualTo(4.75f).Within(0.01f));
        Assert.That(GetPropertyValue(roamController, "DebugState").ToString(), Is.Not.EqualTo("LongPause"),
            "白天 resident 从导演 cue 放人后，不该整排停在 LongPause 里像被继续卡住。");

        InvokeInstance(
            director,
            "ApplyResidentBaseline",
            entry,
            state,
            Enum.Parse(storyPhaseType, "EnterVillage"),
            "EnterVillage_HouseArrival",
            beatConsumption);

        Vector2 roamCenterAfterRepeatSync = (Vector2)GetPropertyValue(roamController, "DebugRoamCenter");
        Assert.That(roamCenterAfterRepeatSync.x, Is.EqualTo(2.5f).Within(0.01f),
            "重复 SyncCrowd 后，白天 resident 的 roam center 应继续留在自己的 daytime base 域，而不是跳去统一聚堆点。");
        Assert.That(roamCenterAfterRepeatSync.y, Is.EqualTo(4.75f).Within(0.01f));
    }

    [Test]
    public void CrowdDirector_ShouldKeepResidentsRoamingBeforeTwentyDuringFreeTime()
    {
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Component timeManager = Track(new GameObject("TimeManager_FreeTimeBeforeTwenty")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 19);
        SetField(timeManager, "currentMinute", 30);

        Component director = Track(new GameObject("CrowdDirector_FreeTimeReturnHome")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject npc = Track(new GameObject("Resident_FreeTimeReturning"));
        GameObject homeAnchor = Track(new GameObject("Resident_FreeTimeReturning_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        npc.transform.position = new Vector3(6.5f, -2.25f, 0f);
        homeAnchor.transform.position = new Vector3(1.5f, 0.75f, 0f);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_FreeTimeReturning_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.left);
        SetField(state, "NeedsResidentReset", true);
        SetField(state, "ReleasedFromDirectorCue", true);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type beatSnapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionSnapshot");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");
        object entry = Activator.CreateInstance(entryType);
        object beatConsumption = Activator.CreateInstance(beatSnapshotType);
        SetField(entry, "residentBaseline", Enum.Parse(residentBaselineType, "DaytimeResident"));

        InvokeInstance(
            director,
            "ApplyResidentBaseline",
            entry,
            state,
            Enum.Parse(storyPhaseType, "FreeTime"),
            string.Empty,
            beatConsumption);

        Assert.That(npc.transform.position.x, Is.EqualTo(6.5f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(-2.25f).Within(0.001f));
        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.False, "19:30 的自由时段仍应先让居民正常活动，不能提前进回家链。");
        Assert.That((bool)GetFieldValue(state, "NeedsResidentReset"), Is.False);
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.False);
        StringAssert.DoesNotContain("return-home", (string)GetFieldValue(state, "ResolvedAnchorName"));
    }

    [Test]
    public void CrowdDirector_ShouldQueueReturnHomeAtTwentyDuringFreeTime()
    {
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Component timeManager = Track(new GameObject("TimeManager_FreeTimeAtTwenty")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 20);
        SetField(timeManager, "currentMinute", 0);

        Component director = Track(new GameObject("CrowdDirector_FreeTimeReturnHome_AtTwenty")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject npc = Track(new GameObject("Resident_FreeTimeReturning_AtTwenty"));
        GameObject homeAnchor = Track(new GameObject("Resident_FreeTimeReturning_AtTwenty_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        npc.transform.position = new Vector3(6.5f, -2.25f, 0f);
        homeAnchor.transform.position = new Vector3(1.5f, 0.75f, 0f);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_FreeTimeReturning_AtTwenty_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.left);
        SetField(state, "NeedsResidentReset", true);
        SetField(state, "ReleasedFromDirectorCue", true);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type beatSnapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionSnapshot");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");
        object entry = Activator.CreateInstance(entryType);
        object beatConsumption = Activator.CreateInstance(beatSnapshotType);
        SetField(entry, "residentBaseline", Enum.Parse(residentBaselineType, "DaytimeResident"));

        InvokeInstance(
            director,
            "ApplyResidentBaseline",
            entry,
            state,
            Enum.Parse(storyPhaseType, "FreeTime"),
            string.Empty,
            beatConsumption);

        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.True, "20:00 之后再释放 cue，resident 才应进入回家散场。");
        Assert.That((bool)GetFieldValue(state, "NeedsResidentReset"), Is.True);
        StringAssert.Contains("return-home", (string)GetFieldValue(state, "ResolvedAnchorName"));
    }

    [Test]
    public void CrowdDirector_ShouldResumeRoamAfterReturnHomeCompletes()
    {
        Component director = Track(new GameObject("CrowdDirector_ReturnHomeFinish")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject npc = Track(new GameObject("Resident_ReturnComplete"));
        GameObject homeAnchor = Track(new GameObject("Resident_ReturnComplete_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        npc.transform.position = new Vector3(1.96f, 0.02f, 0f);
        homeAnchor.transform.position = new Vector3(2f, 0f, 0f);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_ReturnComplete_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.up);
        SetField(state, "NeedsResidentReset", true);
        SetField(state, "ReleasedFromDirectorCue", true);
        SetField(state, "IsReturningHome", true);
        SetField(state, "ResumeRoamAfterReturn", true);

        InvokeInstance(director, "TickResidentReturnHome", state, 1f);

        Assert.That(npc.transform.position.x, Is.EqualTo(2f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(0f).Within(0.001f));
        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.False);
        Assert.That((bool)GetFieldValue(state, "NeedsResidentReset"), Is.False);
        Assert.That((bool)GetFieldValue(state, "ReleasedFromDirectorCue"), Is.False);
        Assert.That((bool)GetPropertyValue(roamController, "IsRoaming"), Is.True, "回到 HomeAnchor 后应重新进入 roam。");
    }

    [Test]
    public void CrowdDirector_ShouldFallbackToStepReturnHomeWhenRoamControllerCannotStartPath()
    {
        Component director = Track(new GameObject("CrowdDirector_ReturnHomePathing")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject npc = Track(new GameObject("Resident_ReturnPathing"));
        GameObject homeAnchor = Track(new GameObject("Resident_ReturnPathing_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        npc.transform.position = new Vector3(-6f, 1.5f, 0f);
        homeAnchor.transform.position = new Vector3(6f, 1.5f, 0f);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_ReturnPathing_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.right);
        SetField(state, "NeedsResidentReset", true);
        SetField(state, "ReleasedFromDirectorCue", true);
        SetField(state, "IsReturningHome", true);
        SetField(state, "ResumeRoamAfterReturn", true);

        Vector3 startPosition = npc.transform.position;
        InvokeInstance(director, "TickResidentReturnHome", state, 1f);

        Assert.That(npc.transform.position, Is.Not.EqualTo(startPosition), "如果 scripted move 连路径都起不来，resident 至少要继续朝 homeAnchor 退场，不能原地冻住。");
        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.True, "未到 HomeAnchor 前，return-home 语义应保持。");
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.True, "即使这次路径还没真正跑起来，也应保持 scripted control 接管态。");
    }

    [Test]
    public void CrowdDirector_ClockSchedule_ShouldStartReturnAtTwentyAndRestAtTwentyOne()
    {
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Component timeManager = Track(new GameObject("TimeManager_Test")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 20);
        SetField(timeManager, "currentMinute", 0);

        Component director = Track(new GameObject("CrowdDirector_ClockSchedule")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        Assert.That((bool)InvokeInstance(director, "ShouldResidentsReturnHomeByClock"), Is.True, "20:00 之后，居民应先开始自主回 anchor。");
        Assert.That((bool)InvokeInstance(director, "ShouldResidentsRestByClock"), Is.False, "20:00 还不应直接进入强制 resting。");

        SetField(timeManager, "currentHour", 21);
        Assert.That((bool)InvokeInstance(director, "ShouldResidentsReturnHomeByClock"), Is.False);
        Assert.That((bool)InvokeInstance(director, "ShouldResidentsRestByClock"), Is.True, "21:00 之后仍未到家的居民应进入强制 resting/snap 语义。");
    }

    [Test]
    public void CrowdDirector_FreeTimeBeforeTwenty_ShouldReleaseNonPriorityResidentsToDefaultPresent()
    {
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Component timeManager = Track(new GameObject("TimeManager_FreeTimeRelease")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 19);
        SetField(timeManager, "currentMinute", 30);

        Component director = Track(new GameObject("CrowdDirector_FreeTimeRelease")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject runtimeRoot = Track(new GameObject("SpringDay1NpcCrowdRuntimeRoot"));
        GameObject residentRoot = Track(new GameObject("Town_Day1Residents"));
        GameObject defaultRoot = Track(new GameObject("Resident_DefaultPresent"));
        GameObject takeoverRoot = Track(new GameObject("Resident_DirectorTakeoverReady"));
        GameObject backstageRoot = Track(new GameObject("Resident_BackstagePresent"));
        residentRoot.transform.SetParent(runtimeRoot.transform, false);
        defaultRoot.transform.SetParent(residentRoot.transform, false);
        takeoverRoot.transform.SetParent(residentRoot.transform, false);
        backstageRoot.transform.SetParent(residentRoot.transform, false);
        SetField(director, "_sceneRoot", runtimeRoot);
        SetField(director, "_residentRoot", residentRoot);
        SetField(director, "_residentDefaultRoot", defaultRoot);
        SetField(director, "_residentTakeoverReadyRoot", takeoverRoot);
        SetField(director, "_residentBackstageRoot", backstageRoot);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type beatSnapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionSnapshot");
        Type beatEntryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionEntry");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "201");

        object beatEntry = Activator.CreateInstance(beatEntryType);
        SetField(beatEntry, "npcId", "201");
        Array traceEntries = Array.CreateInstance(beatEntryType, 1);
        traceEntries.SetValue(beatEntry, 0);

        object beatSnapshot = Activator.CreateInstance(beatSnapshotType);
        SetField(beatSnapshot, "trace", traceEntries);

        Transform resolvedParent = InvokeInstance(
            director,
            "ResolveResidentParent",
            entry,
            Enum.Parse(storyPhaseType, "FreeTime"),
            "FreeTime_NightWitness",
            beatSnapshot) as Transform;

        Assert.That(resolvedParent, Is.EqualTo(defaultRoot.transform), "19:30 的自由时段里，普通 resident 应回到 default-present，而不是继续被扣在 backstage。");
    }

    [Test]
    public void CrowdDirector_FreeTimeBeforeTwenty_ShouldKeepPriorityWitnessInTakeoverReady()
    {
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Component timeManager = Track(new GameObject("TimeManager_FreeTimePriority")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 19);
        SetField(timeManager, "currentMinute", 30);

        Component director = Track(new GameObject("CrowdDirector_FreeTimePriority")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject runtimeRoot = Track(new GameObject("SpringDay1NpcCrowdRuntimeRoot"));
        GameObject residentRoot = Track(new GameObject("Town_Day1Residents"));
        GameObject defaultRoot = Track(new GameObject("Resident_DefaultPresent"));
        GameObject takeoverRoot = Track(new GameObject("Resident_DirectorTakeoverReady"));
        GameObject backstageRoot = Track(new GameObject("Resident_BackstagePresent"));
        residentRoot.transform.SetParent(runtimeRoot.transform, false);
        defaultRoot.transform.SetParent(residentRoot.transform, false);
        takeoverRoot.transform.SetParent(residentRoot.transform, false);
        backstageRoot.transform.SetParent(residentRoot.transform, false);
        SetField(director, "_sceneRoot", runtimeRoot);
        SetField(director, "_residentRoot", residentRoot);
        SetField(director, "_residentDefaultRoot", defaultRoot);
        SetField(director, "_residentTakeoverReadyRoot", takeoverRoot);
        SetField(director, "_residentBackstageRoot", backstageRoot);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type beatSnapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionSnapshot");
        Type beatEntryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionEntry");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "102");

        object beatEntry = Activator.CreateInstance(beatEntryType);
        SetField(beatEntry, "npcId", "102");
        Array priorityEntries = Array.CreateInstance(beatEntryType, 1);
        priorityEntries.SetValue(beatEntry, 0);

        object beatSnapshot = Activator.CreateInstance(beatSnapshotType);
        SetField(beatSnapshot, "priority", priorityEntries);

        Transform resolvedParent = InvokeInstance(
            director,
            "ResolveResidentParent",
            entry,
            Enum.Parse(storyPhaseType, "FreeTime"),
            "FreeTime_NightWitness",
            beatSnapshot) as Transform;

        Assert.That(resolvedParent, Is.EqualTo(takeoverRoot.transform), "真正的 night witness actor 仍应保留在 takeover-ready 语义，不跟普通 resident 一起放掉。");
    }

    [Test]
    public void StagingPlayback_ShouldKeepManualPreviewLockUntilClear()
    {
        GameObject npc = Track(new GameObject("NPC_ManualPreview"));
        Type playbackType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingPlayback");
        Type motionControllerType = ResolveTypeOrFail("NPCMotionController");
        Component playback = npc.AddComponent(playbackType);
        npc.AddComponent(motionControllerType);

        Type cueType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorActorCue");
        object cue = Activator.CreateInstance(cueType);
        SetField(cue, "cueId", "manual-preview");
        SetField(cue, "keepCurrentSpawnPosition", true);
        SetField(cue, "suspendRoam", true);
        SetField(cue, "path", Array.CreateInstance(ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorPathPoint"), 0));

        InvokeInstance(playback, "ApplyCue", "EnterVillage_PostEntry", cue, null, true);

        Assert.That((bool)GetPropertyValue(playback, "IsManualPreviewLocked"), Is.True);
        Assert.That((string)GetPropertyValue(playback, "CurrentCueKey"), Is.EqualTo("manual-preview"));

        InvokeInstance(playback, "ClearCue");

        Assert.That((bool)GetPropertyValue(playback, "IsManualPreviewLocked"), Is.False);
        Assert.That((string)GetPropertyValue(playback, "CurrentCueKey"), Is.Empty);
    }

    [Test]
    public void StagingPlayback_ShouldDriveRoamControllerInsteadOfHardPushingTransformDuringCueMotion()
    {
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        GameObject navGridObject = Track(new GameObject("NavGrid_Staging_RuntimePathing"));
        GameObject npc = Track(new GameObject("NPC_Staging_RuntimePathing"));
        GameObject homeAnchor = Track(new GameObject("NPC_Staging_RuntimePathing_Home"));
        npc.transform.position = new Vector3(0.5f, 0.5f, 0f);
        homeAnchor.transform.position = new Vector3(-3f, 2f, 0f);

        Type playbackType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingPlayback");
        Type roamControllerType = ResolveTypeOrFail("NPCAutoRoamController");

        Component navGrid = navGridObject.AddComponent(navGridType);
        InvokeInstance(navGrid, "Awake");
        SetField(navGrid, "autoDetectWorldBounds", false);
        SetField(navGrid, "worldOrigin", new Vector2(-4f, -4f));
        SetField(navGrid, "worldSize", new Vector2(8f, 8f));
        SetField(navGrid, "cellSize", 0.5f);
        SetField(navGrid, "probeRadius", 0.5f);
        LayerMask noObstacleMask = 0;
        SetField(navGrid, "obstacleMask", noObstacleMask);
        SetField(navGrid, "obstacleTags", Array.Empty<string>());
        InvokeInstance(navGrid, "RebuildGrid");

        Component playback = npc.AddComponent(playbackType);
        Behaviour roam = (Behaviour)npc.AddComponent(roamControllerType);

        Type cueType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorActorCue");
        Type pathPointType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorPathPoint");
        object cue = Activator.CreateInstance(cueType);
        object point = Activator.CreateInstance(pathPointType);
        Array path = Array.CreateInstance(pathPointType, 1);

        SetField(cue, "cueId", "runtime-pathing");
        SetField(cue, "keepCurrentSpawnPosition", true);
        SetField(cue, "suspendRoam", true);
        SetField(cue, "initialHoldSeconds", 0f);
        SetField(cue, "moveSpeed", 1.4f);
        SetField(point, "position", new Vector2(2.5f, 0.5f));
        path.SetValue(point, 0);
        SetField(cue, "path", path);

        InvokeInstance(playback, "ApplyCue", "EnterVillage_PostEntry", cue, homeAnchor.transform);
        Vector3 startPosition = npc.transform.position;
        InvokeInstance(playback, "Update");

        Assert.That(npc.transform.position, Is.EqualTo(startPosition), "有 roam controller 时，runtime cue 不应再直接硬推 transform。");
        Assert.That(roam.enabled, Is.True, "playback 应把 roam controller 重新拉回 enabled，让 scripted move 真正进入导航合同。");
        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedControlActive"), Is.True);
        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedMoveActive"), Is.True);
        Assert.That((bool)GetFieldValue(roam, "hasRequestedDestination"), Is.True);
        Assert.That((Vector2)GetFieldValue(roam, "requestedDestination"), Is.EqualTo(new Vector2(2.5f, 0.5f)));
    }

    [Test]
    public void CrowdDirector_ShouldDeferToManualPreviewLockedPlayback()
    {
        Component director = Track(new GameObject("CrowdDirector_ManualPreview")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject npc = Track(new GameObject("101"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component playback = npc.AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingPlayback"));

        Type cueType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorActorCue");
        object manualCue = Activator.CreateInstance(cueType);
        SetField(manualCue, "cueId", "manual-preview");
        SetField(manualCue, "keepCurrentSpawnPosition", true);
        SetField(manualCue, "suspendRoam", true);
        SetField(manualCue, "path", Array.CreateInstance(ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorPathPoint"), 0));
        InvokeInstance(playback, "ApplyCue", "EnterVillage_PostEntry", manualCue, null, true);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "BaseResolvedAnchorName", "101_HomeAnchor");

        bool held = (bool)InvokeStatic(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"), "TryHoldForManualPreview", state);

        Assert.That(held, Is.True);
        Assert.That((string)GetFieldValue(state, "ResolvedAnchorName"), Is.EqualTo("manual-preview:manual-preview"));
        Assert.That((bool)GetPropertyValue(playback, "IsManualPreviewLocked"), Is.True);
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
        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedControlActive"), Is.True);
        Assert.That((string)GetPropertyValue(roam, "ResidentScriptedControlOwnerKey"), Is.EqualTo("spring-day1-director"));

        InvokeInstance(takeover, "Release");

        Assert.That(roam.enabled, Is.True);
        Assert.That(informal.enabled, Is.True);
        Assert.That(dialogue.enabled, Is.True);
        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedControlActive"), Is.False);
    }

    [Test]
    public void ResidentScriptedControl_ReacquireSameOwnerShouldRemainIdempotent()
    {
        GameObject npc = Track(new GameObject("NPC_IdempotentTakeover"));
        Behaviour roam = (Behaviour)npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        InvokeInstance(roam, "AcquireResidentScriptedControl", "spring-day1-director", true);
        InvokeInstance(roam, "AcquireResidentScriptedControl", "spring-day1-director", true);
        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedControlActive"), Is.True);

        InvokeInstance(roam, "ReleaseResidentScriptedControl", "spring-day1-director", false);
        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedControlActive"), Is.False, "同一个 owner 重复接管不应堆叠成多层 owner 栈。");
    }

    [Test]
    public void EscortWait_ShouldUseResumeHysteresisBeforeLeavingWaitState()
    {
        GameObject runtime = Track(new GameObject("SpringDay1Director_Runtime"));
        Component director = runtime.AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1Director"));

        bool shouldEnterWait = (bool)InvokeInstance(director, "ShouldEscortWaitForPlayer", 2f, 7.2f, false);
        bool shouldKeepWaiting = (bool)InvokeInstance(director, "ShouldEscortWaitForPlayer", 2f, 6.1f, true);
        bool shouldResumeEscort = (bool)InvokeInstance(director, "ShouldEscortWaitForPlayer", 2f, 5.6f, true);

        Assert.That(shouldEnterWait, Is.True, "玩家明显掉队时应进入等待。");
        Assert.That(shouldKeepWaiting, Is.True, "刚回到阈值附近时应继续等待，避免停走抖动。");
        Assert.That(shouldResumeEscort, Is.False, "只有真正跟近后才应退出等待。");
    }

    [Test]
    public void EscortTransition_ShouldRequireChiefAndCompanionReady()
    {
        GameObject runtime = Track(new GameObject("SpringDay1Director_Runtime"));
        Component director = runtime.AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1Director"));
        GameObject chief = Track(new GameObject("001"));
        GameObject companion = Track(new GameObject("002"));
        Vector3 targetPosition = new Vector3(8f, 3.5f, 0f);
        chief.transform.position = targetPosition;

        Vector3 companionTarget = (Vector3)InvokeInstance(
            director,
            "BuildEscortCompanionTarget",
            chief.transform.position,
            targetPosition);
        companion.transform.position = companionTarget + new Vector3(2.4f, 0f, 0f);

        bool shouldBlockTransition = (bool)InvokeInstance(
            director,
            "AreEscortActorsReadyForTransition",
            chief.transform,
            companion.transform,
            targetPosition,
            0.65f);

        companion.transform.position = companionTarget;

        bool shouldAllowTransition = (bool)InvokeInstance(
            director,
            "AreEscortActorsReadyForTransition",
            chief.transform,
            companion.transform,
            targetPosition,
            0.65f);

        Assert.That(shouldBlockTransition, Is.False, "同伴还没跟到位时，不应让 Town/回村转场提前切过去。");
        Assert.That(shouldAllowTransition, Is.True, "001/002 都贴近目标后，才允许继续转场。");
    }

    [Test]
    public void TryDriveEscortActor_ShouldNotNudgeWhenResidentScriptedMoveCannotPath()
    {
        GameObject runtime = Track(new GameObject("SpringDay1Director_Runtime"));
        Component director = runtime.AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1Director"));
        GameObject npc = Track(new GameObject("001"));
        Behaviour roam = (Behaviour)npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        Vector3 startPosition = npc.transform.position;
        bool moved = (bool)InvokeInstance(
            director,
            "TryDriveEscortActor",
            npc.transform,
            roam,
            new Vector3(4f, 0f, 0f),
            0.65f);

        Assert.That(moved, Is.False, "没有有效路径时，导演不应再偷偷回退到硬推位移。");
        Assert.That(npc.transform.position, Is.EqualTo(startPosition));
        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedControlActive"), Is.True, "即使这次没发出路径，也应保持 director 的接管态。");
    }

    [Test]
    public void ResidentScriptedControl_EndDebugMoveShouldPreserveResumeRoamFlag()
    {
        GameObject npc = Track(new GameObject("NPC_ScriptedMoveResume"));
        Behaviour roam = (Behaviour)npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");

        InvokeInstance(roam, "StartRoam");
        InvokeInstance(roam, "AcquireResidentScriptedControl", "spring-day1-director", true);
        SetField(roam, "debugMoveActive", true);
        SetField(roam, "state", ParseEnum(roamStateType, "Moving"));

        InvokeInstance(roam, "EndDebugMove", false);

        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedControlActive"), Is.True, "结束 scripted move 后不应顺手把 resident control 释放掉。");
        Assert.That((bool)GetPropertyValue(roam, "ResumeRoamWhenResidentControlReleases"), Is.True, "结束 scripted move 后仍应保留恢复 roam 的意图。");

        InvokeInstance(roam, "ReleaseResidentScriptedControl", "spring-day1-director", true);

        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedControlActive"), Is.False);
        Assert.That((bool)GetPropertyValue(roam, "IsRoaming"), Is.True, "导演放手后，resident 应能按原 contract 恢复 roam。");
    }

    [Test]
    public void ResidentScriptedControl_DebugMoveShouldStillParticipateInSharedAvoidance()
    {
        GameObject npc = Track(new GameObject("NPC_ScriptedMoveAvoidance"));
        Behaviour roam = (Behaviour)npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        InvokeInstance(roam, "AcquireResidentScriptedControl", "spring-day1-director", true);
        SetField(roam, "debugMoveActive", true);

        bool bypassWhileScripted = (bool)InvokeInstance(roam, "ShouldBypassSharedAvoidanceForCurrentMove");
        Assert.That(bypassWhileScripted, Is.False, "resident scripted move 也应继续参与 shared avoidance，而不是直接绕开人群避让。");

        InvokeInstance(roam, "ReleaseResidentScriptedControl", "spring-day1-director", false);
        SetField(roam, "debugMoveActive", true);

        bool bypassPlainDebugMove = (bool)InvokeInstance(roam, "ShouldBypassSharedAvoidanceForCurrentMove");
        Assert.That(bypassPlainDebugMove, Is.True, "纯 debug move 仍可保留旧的绕开 shared avoidance 口径。");
    }

    [Test]
    public void ResidentScriptedControl_StaticBlockedAdvanceShouldAllowEarlyAbort()
    {
        GameObject npc = Track(new GameObject("NPC_ScriptedMoveBlockedAbort"));
        Behaviour roam = (Behaviour)npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        InvokeInstance(roam, "AcquireResidentScriptedControl", "spring-day1-director", true);
        SetField(roam, "debugMoveActive", true);
        SetField(roam, "blockedAdvanceFrames", 2);
        SetField(roam, "lastBlockingAgentId", 0);
        SetField(roam, "sharedAvoidanceBlockingFrames", 0);

        bool scriptedShouldAbort = (bool)InvokeInstance(roam, "ShouldAbortBlockedAdvanceWithoutRebuild", "ConstrainedZeroAdvance");
        Assert.That(scriptedShouldAbort, Is.True, "resident scripted move 命中静态零推进坏点时，也应允许早停退出，避免在树边/角落持续 storm。");

        InvokeInstance(roam, "ReleaseResidentScriptedControl", "spring-day1-director", false);
        SetField(roam, "debugMoveActive", true);
        SetField(roam, "blockedAdvanceFrames", 2);
        SetField(roam, "lastBlockingAgentId", 0);
        SetField(roam, "sharedAvoidanceBlockingFrames", 0);

        bool plainDebugShouldAbort = (bool)InvokeInstance(roam, "ShouldAbortBlockedAdvanceWithoutRebuild", "ConstrainedZeroAdvance");
        Assert.That(plainDebugShouldAbort, Is.False, "非 resident scripted 的纯 debug move 仍保留旧口径，不在这里顺手改语义。");
    }

    [Test]
    public void ResidentScriptedControl_PauseAndResumeShouldPreserveScriptedMove()
    {
        GameObject npc = Track(new GameObject("NPC_ScriptedMovePause"));
        Behaviour roam = (Behaviour)npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        Vector2 requestedDestination = new Vector2(4f, 1.5f);

        InvokeInstance(roam, "AcquireResidentScriptedControl", "spring-day1-director", true);
        SetField(roam, "debugMoveActive", true);
        SetField(roam, "residentScriptedMovePaused", false);
        SetField(roam, "state", ParseEnum(roamStateType, "Moving"));
        SetField(roam, "requestedDestination", requestedDestination);
        SetField(roam, "hasRequestedDestination", true);

        InvokeInstance(roam, "PauseResidentScriptedMovement");

        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedControlActive"), Is.True);
        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedMovePaused"), Is.True, "等待玩家时应只暂停现有 scripted move，而不是把整条导演路径清空。");
        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedMoveActive"), Is.False);
        Assert.That((Vector2)GetFieldValue(roam, "requestedDestination"), Is.EqualTo(requestedDestination));
        Assert.That((bool)GetFieldValue(roam, "hasRequestedDestination"), Is.True);

        InvokeInstance(roam, "ResumeResidentScriptedMovement");

        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedMovePaused"), Is.False);
        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedMoveActive"), Is.True, "玩家重新跟上后，应继续沿用原 scripted move 往前走。");
    }

    [Test]
    public void RehearsalDriver_ShouldPauseExistingPlaybackUntilDisabled()
    {
        GameObject npc = Track(new GameObject("NPC_RehearsalPlayback"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Behaviour playback = (Behaviour)npc.AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingPlayback"));

        Assert.That(playback.enabled, Is.True);

        Component driver = npc.AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingRehearsalDriver"));
        if (playback.enabled)
        {
            InvokeInstance(driver, "OnEnable");
        }

        Assert.That(playback.enabled, Is.False, "进入排练时，应先暂停已有 runtime playback，避免和手动导演抢控制权。");

        InvokeInstance(driver, "OnDisable");
        UnityEngine.Object.DestroyImmediate(driver);

        Assert.That(playback.enabled, Is.True, "停止排练后，应恢复原有 playback 状态。");
    }

    [Test]
    public void CrowdDirector_ShouldHoldForActiveRehearsalDriver()
    {
        GameObject npc = Track(new GameObject("NPC_RehearsalHold"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component driver = npc.AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingRehearsalDriver"));

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "BaseResolvedAnchorName", "101_HomeAnchor");
        SetField(state, "AppliedCueKey", "enter-crowd-101");

        bool held = (bool)InvokeStatic(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"), "TryHoldForRehearsal", state);

        Assert.That(held, Is.True);
        Assert.That((string)GetFieldValue(state, "ResolvedAnchorName"), Is.EqualTo("rehearsal:enter-crowd-101"));

        UnityEngine.Object.DestroyImmediate(driver);

        bool heldAfterDisable = (bool)InvokeStatic(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"), "TryHoldForRehearsal", state);
        Assert.That(heldAfterDisable, Is.False);
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

    private static void AssertTownContractResolution(Component director, string npcId, string semanticAnchorId, string beatKey, Vector2 expectedPosition)
    {
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", npcId);
        SetField(entry, "semanticAnchorIds", new[] { semanticAnchorId });
        SetField(entry, "anchorObjectName", string.Empty);
        SetField(entry, "spawnOffset", Vector2.zero);
        SetField(entry, "fallbackWorldPosition", Vector2.zero);

        object resolution = InvokeInstance(director, "ResolveSpawnPoint", entry, beatKey);
        Vector3 position = (Vector3)GetPropertyValue(resolution, "Position");
        string anchorName = (string)GetPropertyValue(resolution, "AnchorName");
        bool usedFallback = (bool)GetPropertyValue(resolution, "UsedFallback");

        Assert.That(usedFallback, Is.False);
        Assert.That(anchorName, Is.EqualTo(semanticAnchorId));
        Assert.That(position.x, Is.EqualTo(expectedPosition.x).Within(0.001f));
        Assert.That(position.y, Is.EqualTo(expectedPosition.y).Within(0.001f));
    }

    private Scene CreateNamedTestScene(string sceneName)
    {
        string tempSceneDirectory = Path.Combine("Assets", "__CodexEditModeScenes", Guid.NewGuid().ToString("N")).Replace("\\", "/");
        string tempSceneDirectoryAbsolute = Path.Combine(Directory.GetCurrentDirectory(), tempSceneDirectory.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(tempSceneDirectoryAbsolute);
        string scenePath = $"{tempSceneDirectory}/{sceneName}.unity";

        Scene tempScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        bool saved = EditorSceneManager.SaveScene(tempScene, scenePath, saveAsCopy: false);
        Assert.That(saved, Is.True, $"应能创建用于 {sceneName} runtime 验证的临时场景。");
        Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(sceneName));
        temporaryScenePaths.Add(scenePath);
        return tempScene;
    }

    private void CleanupTemporaryScenes()
    {
        for (int index = temporaryScenePaths.Count - 1; index >= 0; index--)
        {
            string scenePath = temporaryScenePaths[index];
            if (string.IsNullOrWhiteSpace(scenePath))
            {
                continue;
            }

            if (SceneManager.GetActiveScene().path == scenePath)
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }

            string absoluteScenePath = Path.Combine(Directory.GetCurrentDirectory(), scenePath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(absoluteScenePath))
            {
                File.Delete(absoluteScenePath);
            }

            string metaPath = $"{absoluteScenePath}.meta";
            if (File.Exists(metaPath))
            {
                File.Delete(metaPath);
            }
        }

        temporaryScenePaths.Clear();
        AssetDatabase.Refresh();
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
        Assert.That((bool)GetFieldValue(cue, "useSemanticAnchorAsStart"), Is.False, $"{cueId} 既然已经按场景点位落成绝对坐标，就不应继续保留语义锚点起点。");
        Assert.That((bool)GetFieldValue(cue, "startPositionIsSemanticAnchorOffset"), Is.False, $"{cueId} 的起点不应再被当成语义锚点偏移量。");
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

    private static void AssertCueUsesSemanticAnchorStart(object bookAsset, string beatKey, string cueId)
    {
        object beat = InvokeInstance(bookAsset, "FindBeat", beatKey);
        Assert.That(beat, Is.Not.Null, $"StageBook 缺少 beat: {beatKey}");

        object cue = FindCueById(beat, cueId);
        Assert.That(cue, Is.Not.Null, $"Beat {beatKey} 缺少 cue: {cueId}");

        Assert.That((bool)GetFieldValue(cue, "keepCurrentSpawnPosition"), Is.False, $"{cueId} 不应再继续依赖当前出生位。");
        Assert.That((bool)GetFieldValue(cue, "useSemanticAnchorAsStart"), Is.True, $"{cueId} 应已改成由语义锚点驱动起点。");
        Assert.That((bool)GetFieldValue(cue, "startPositionIsSemanticAnchorOffset"), Is.True, $"{cueId} 应保存成锚点相对偏移。");
    }

    private static object FindCueById(object beat, string cueId)
    {
        Array cues = (Array)GetFieldValue(beat, "actorCues");
        if (cues == null)
        {
            return null;
        }

        string normalizedNpcId = ExtractTrailingNpcId(cueId);
        foreach (object cue in cues)
        {
            if (cue == null)
            {
                continue;
            }

            string currentCueId = GetFieldValue(cue, "cueId") as string;
            string currentNpcId = GetFieldValue(cue, "npcId") as string;
            if (string.Equals(currentCueId, cueId, StringComparison.OrdinalIgnoreCase)
                || string.Equals(currentNpcId, cueId, StringComparison.OrdinalIgnoreCase)
                || (!string.IsNullOrWhiteSpace(normalizedNpcId)
                    && string.Equals(currentNpcId, normalizedNpcId, StringComparison.OrdinalIgnoreCase)))
            {
                return cue;
            }
        }

        return null;
    }

    private static string ExtractTrailingNpcId(string cueId)
    {
        if (string.IsNullOrWhiteSpace(cueId))
        {
            return string.Empty;
        }

        int end = cueId.Length - 1;
        while (end >= 0 && !char.IsDigit(cueId[end]))
        {
            end--;
        }

        if (end < 0)
        {
            return string.Empty;
        }

        int start = end;
        while (start >= 0 && char.IsDigit(cueId[start]))
        {
            start--;
        }

        return cueId.Substring(start + 1, end - start);
    }

    private static void AssertCuePathProgression(object bookAsset, string beatKey, string cueId, int minimumPathPoints, int minimumUniquePositions)
    {
        object beat = InvokeInstance(bookAsset, "FindBeat", beatKey);
        Assert.That(beat, Is.Not.Null, $"StageBook 缺少 beat: {beatKey}");

        object cue = FindCueById(beat, cueId);
        Assert.That(cue, Is.Not.Null, $"Beat {beatKey} 缺少 cue: {cueId}");

        Array path = (Array)GetFieldValue(cue, "path");
        Assert.That(path, Is.Not.Null, $"{cueId} 的 path 不应为空。");
        Assert.That(path.Length, Is.GreaterThanOrEqualTo(minimumPathPoints), $"{cueId} 的 path 点数不足。");

        HashSet<string> uniquePositions = new HashSet<string>(StringComparer.Ordinal);
        foreach (object point in path)
        {
            if (point == null)
            {
                continue;
            }

            Vector2 position = (Vector2)GetFieldValue(point, "position");
            uniquePositions.Add($"{position.x:F3},{position.y:F3}");
        }

        Assert.That(uniquePositions.Count, Is.GreaterThanOrEqualTo(minimumUniquePositions), $"{cueId} 仍像空轨迹，独立位置点不足。");
    }

    private static object InvokeInstance(object target, string methodName, params object[] args)
    {
        MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo method = null;
        for (int index = 0; index < methods.Length; index++)
        {
            MethodInfo candidate = methods[index];
            if (!string.Equals(candidate.Name, methodName, StringComparison.Ordinal))
            {
                continue;
            }

            ParameterInfo[] parameters = candidate.GetParameters();
            if (parameters.Length != args.Length)
            {
                continue;
            }

            bool matches = true;
            for (int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                object argument = args[parameterIndex];
                Type parameterType = parameters[parameterIndex].ParameterType;
                if (argument == null)
                {
                    if (parameterType.IsValueType && Nullable.GetUnderlyingType(parameterType) == null)
                    {
                        matches = false;
                        break;
                    }

                    continue;
                }

                Type argumentType = argument.GetType();
                if (!parameterType.IsAssignableFrom(argumentType))
                {
                    matches = false;
                    break;
                }
            }

            if (!matches)
            {
                continue;
            }

            method = candidate;
            break;
        }

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

    private static void AddCompletedSequenceId(object target, string sequenceId)
    {
        FieldInfo field = target.GetType().GetField("_completedSequenceIds", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}._completedSequenceIds");
        if (field.GetValue(target) is HashSet<string> ids && !string.IsNullOrWhiteSpace(sequenceId))
        {
            ids.Add(sequenceId);
        }
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
