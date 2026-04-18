using System;
using System.Collections;
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
    public void CrowdDirector_ShouldPreferLegacyAnchorWhenSemanticAnchorIsDeprecatedRuntimeRoot()
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
        Assert.That(anchorName, Is.EqualTo("001"));
        Assert.That(position.x, Is.EqualTo(1.25f).Within(0.001f));
        Assert.That(position.y, Is.EqualTo(2.75f).Within(0.001f));
    }

    [Test]
    public void CrowdDirector_ShouldIgnoreDeprecatedNightWitnessTownContractAnchorForRuntimeSpawn()
    {
        Component director = Track(new GameObject("CrowdDirector_Test")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        AssertDeprecatedTownContractAnchorIgnored(director, "102", "NightWitness_01", "FreeTime_NightWitness");
    }

    [Test]
    public void CrowdDirector_ShouldIgnoreDirectorReadyAliasForDeprecatedOpeningRuntimeRoot()
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

        Assert.That(usedFallback, Is.True);
        Assert.That(anchorName, Is.EqualTo("fallback"));
        Assert.That(position.x, Is.EqualTo(0f).Within(0.001f));
        Assert.That(position.y, Is.EqualTo(0f).Within(0.001f));
    }

    [Test]
    public void CrowdDirector_ShouldRequireSceneMarkersForVillageCrowdCue()
    {
        Type directorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        MethodInfo resolveMethod = directorType.GetMethod(
            "TryResolveStagingCueForCurrentScene",
            BindingFlags.NonPublic | BindingFlags.Static);

        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "101");

        Assert.That(resolveMethod, Is.Not.Null);

        object[] args = { "EnterVillage_PostEntry", entry, null };
        bool resolved = (bool)resolveMethod.Invoke(null, args);

        Assert.That(resolved, Is.False, "没有进村围观场景 markers 时，不应再为 village crowd 生成旧 fallback cue。");
        Assert.That(args[2], Is.Null);
    }

    [Test]
    public void CrowdDirector_ShouldIgnoreDeprecatedDailyStandTownContractAnchorsForRuntimeSpawn()
    {
        Component director = Track(new GameObject("CrowdDirector_TownContract")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));

        AssertDeprecatedTownContractAnchorIgnored(director, "103", "DailyStand_02", "DailyStand_Preview");
        AssertDeprecatedTownContractAnchorIgnored(director, "102", "DailyStand_03", "DailyStand_Preview");
    }

    [Test]
    public void CrowdDirector_ShouldBindSceneResidentInTownSceneWithoutRuntimeSpawn()
    {
        Scene townScene = CreateNamedTestScene("Town");
        Component director = Track(new GameObject("CrowdDirector_TownScene")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject residentRoot = Track(new GameObject("TestResidentRoot"));
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
        GameObject residentRoot = Track(new GameObject("TestResidentRoot"));
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
        Type sceneTransitionTriggerType = ResolveTypeOrFail("Sunset.Story.SceneTransitionTrigger2D");
        Component storyManager = Track(new GameObject("StoryManager_Snapshot")).AddComponent(storyManagerType);
        Component springDirector = Track(new GameObject("SpringDay1Director_Snapshot")).AddComponent(springDirectorType);
        Component crowdDirector = Track(new GameObject("CrowdDirector_StoryReturnSnapshot")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", springDirector);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "EnterVillage"), false);
        SetField(springDirector, "_townHouseLeadStarted", true);

        GameObject residentRoot = Track(new GameObject("TestResidentRoot"));
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

        GameObject residentRoot = Track(new GameObject("TestResidentRoot"));
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
    public void CrowdDirector_ShouldIgnoreClockOwnedReturnHomeSnapshotAtTwenty()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Type snapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+ResidentRuntimeSnapshotEntry");

        Component timeManager = Track(new GameObject("TimeManager_ClockReturnSnapshot")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 20);
        SetField(timeManager, "currentMinute", 15);

        Component director = Track(new GameObject("CrowdDirector_ClockReturnSnapshot")).AddComponent(crowdDirectorType);
        GameObject residentRoot = Track(new GameObject("TestResidentRoot"));
        GameObject residentDefault = Track(new GameObject("Resident_DefaultPresent"));
        GameObject resident = Track(new GameObject("103"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        SceneManager.MoveGameObjectToScene(residentRoot, townScene);
        SceneManager.MoveGameObjectToScene(residentDefault, townScene);
        SceneManager.MoveGameObjectToScene(resident, townScene);
        residentDefault.transform.SetParent(residentRoot.transform, false);
        resident.transform.SetParent(residentDefault.transform, true);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type beatSnapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionSnapshot");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "103");
        SetField(entry, "prefab", resident);
        SetField(entry, "semanticAnchorIds", Array.Empty<string>());
        SetField(entry, "anchorObjectName", string.Empty);
        SetField(entry, "spawnOffset", Vector2.zero);
        SetField(entry, "fallbackWorldPosition", Vector2.zero);
        SetField(entry, "initialFacing", Vector2.left);
        SetField(entry, "residentBaseline", Enum.Parse(residentBaselineType, "DaytimeResident"));

        object beatConsumption = Activator.CreateInstance(beatSnapshotType);
        object state = InvokeInstance(director, "GetOrCreateState", entry, string.Empty, beatConsumption);

        object snapshot = Activator.CreateInstance(snapshotType);
        SetField(snapshot, "npcId", "103");
        SetField(snapshot, "sceneName", "Town");
        SetField(snapshot, "residentGroupKey", "Resident_DefaultPresent");
        SetField(snapshot, "resolvedAnchorName", "return-home:ClockOwned_103");
        SetField(snapshot, "positionX", 7.5f);
        SetField(snapshot, "positionY", -3f);
        SetField(snapshot, "facingX", -1f);
        SetField(snapshot, "facingY", 0f);
        SetField(snapshot, "isActive", true);
        SetField(snapshot, "isReturningHome", true);
        SetField(snapshot, "underDirectorCue", false);

        System.Collections.IList snapshots = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(snapshotType));
        snapshots.Add(snapshot);
        InvokeStatic(crowdDirectorType, "ApplyResidentRuntimeSnapshots", snapshots);

        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.False,
            "20:00 的 schedule-owned return-home 不应再跨场景恢复成 Day1 持有的旧 owner 状态。");
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.False);
    }

    [Test]
    public void CrowdDirector_ShouldIgnoreStaleDirectorCueSnapshotDuringDaytimeAutonomyWindow()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type snapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+ResidentRuntimeSnapshotEntry");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");

        Component storyManager = Track(new GameObject("StoryManager_StaleCueSnapshot")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("CrowdDirector_StaleCueSnapshot")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FreeTime"), false);

        GameObject residentRoot = Track(new GameObject("TestResidentRoot"));
        GameObject residentDefault = Track(new GameObject("Resident_DefaultPresent"));
        GameObject resident = Track(new GameObject("103"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        GameObject homeAnchor = Track(new GameObject("103_HomeAnchor"));

        SceneManager.MoveGameObjectToScene(residentRoot, townScene);
        SceneManager.MoveGameObjectToScene(residentDefault, townScene);
        SceneManager.MoveGameObjectToScene(resident, townScene);
        SceneManager.MoveGameObjectToScene(homeAnchor, townScene);
        residentDefault.transform.SetParent(residentRoot.transform, false);
        resident.transform.SetParent(residentDefault.transform, true);
        homeAnchor.transform.position = new Vector3(2f, 1f, 0f);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type beatSnapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionSnapshot");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "103");
        SetField(entry, "prefab", resident);
        SetField(entry, "semanticAnchorIds", Array.Empty<string>());
        SetField(entry, "anchorObjectName", string.Empty);
        SetField(entry, "spawnOffset", Vector2.zero);
        SetField(entry, "fallbackWorldPosition", Vector2.zero);
        SetField(entry, "initialFacing", Vector2.left);
        SetField(entry, "residentBaseline", Enum.Parse(residentBaselineType, "DaytimeResident"));

        object beatConsumption = Activator.CreateInstance(beatSnapshotType);
        InvokeInstance(director, "GetOrCreateState", entry, string.Empty, beatConsumption);

        SetField(roamController, "state", ParseEnum(roamStateType, "ShortPause"));
        SetField(roamController, "stateTimer", 99f);

        object snapshot = Activator.CreateInstance(snapshotType);
        SetField(snapshot, "npcId", "103");
        SetField(snapshot, "sceneName", "Town");
        SetField(snapshot, "residentGroupKey", "Resident_DefaultPresent");
        SetField(snapshot, "resolvedAnchorName", "staging:stale-opening");
        SetField(snapshot, "positionX", 7.5f);
        SetField(snapshot, "positionY", -3f);
        SetField(snapshot, "facingX", -1f);
        SetField(snapshot, "facingY", 0f);
        SetField(snapshot, "isActive", true);
        SetField(snapshot, "isReturningHome", false);
        SetField(snapshot, "underDirectorCue", true);

        System.Collections.IList snapshots = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(snapshotType));
        snapshots.Add(snapshot);
        InvokeStatic(crowdDirectorType, "ApplyResidentRuntimeSnapshots", snapshots);

        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.False,
            "0.0.6 这类白天自由窗口回 Town 时，stale director cue snapshot 不应再把 resident 重新抓回导演 owner。");
        Assert.That((bool)GetPropertyValue(roamController, "IsRoaming"), Is.True);
        Assert.That((float)GetFieldValue(roamController, "stateTimer"), Is.Not.EqualTo(99f).Within(0.001f));
    }

    [Test]
    public void CrowdDirector_ShouldRecoverReleasedResidentsIntoTrackedStateAndRestartAutonomousRoam()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");

        Component storyManager = Track(new GameObject("StoryManager_RecoverReleasedResidents")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("CrowdDirector_RecoverReleasedResidents")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FreeTime"), false);

        GameObject resident = Track(new GameObject("103"));
        GameObject homeAnchor = Track(new GameObject("103_HomeAnchor"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        resident.transform.position = new Vector3(6f, -2f, 0f);
        homeAnchor.transform.position = new Vector3(2f, 1f, 0f);
        SetField(roamController, "state", ParseEnum(roamStateType, "ShortPause"));
        SetField(roamController, "stateTimer", 99f);

        SceneManager.MoveGameObjectToScene(resident, townScene);
        SceneManager.MoveGameObjectToScene(homeAnchor, townScene);

        InvokeInstance(director, "RecoverReleasedTownResidentsWithoutSpawnStates", townScene);

        IDictionary spawnStates = (IDictionary)GetFieldValue(director, "_spawnStates");
        Assert.That(spawnStates.Contains("103"), Is.True,
            "spawn-state 掉空后，CrowdDirector 仍应把场上 resident 重新纳回 Day1 视图。");
        Assert.That(GetPropertyValue(roamController, "HomeAnchor"), Is.EqualTo(homeAnchor.transform));
        object recoveredState = spawnStates["103"];
        Assert.That((bool)GetFieldValue(recoveredState, "QueuedAutonomousResume"), Is.True,
            "返场补绑 resident 后，应先进入受控的 queued autonomous resume，而不是继续留在无人接管的坏 runtime。");

        SetField(recoveredState, "NextAutonomousResumeAt", 0f);
        SetField(director, "_nextQueuedAutonomousResumeDispatchAt", 0f);
        InvokeInstance(director, "TickQueuedAutonomousResumes");

        Assert.That((bool)GetPropertyValue(roamController, "IsRoaming"), Is.True,
            "返场补绑 resident 后，CrowdDirector 应把自治漫游真正重启，而不是只补回 tracked state。");
        Assert.That((float)GetFieldValue(roamController, "stateTimer"), Is.Not.EqualTo(99f).Within(0.001f),
            "如果补绑后仍保留旧的 roam runtime 现场，NPC 会继续卡在坏态里，只能靠聊天局部重置解放。");
    }

    [Test]
    public void CrowdDirector_ShouldPreferTownSceneHomeAnchorOverForeignSceneDuplicate()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);
        Scene foreignScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_LocalAnchorPref")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("CrowdDirector_LocalAnchorPref")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FreeTime"), false);

        GameObject foreignAnchor = Track(new GameObject("103_HomeAnchor"));
        GameObject townAnchor = Track(new GameObject("103_HomeAnchor"));
        GameObject resident = Track(new GameObject("103"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        foreignAnchor.transform.position = new Vector3(99f, 99f, 0f);
        townAnchor.transform.position = new Vector3(2f, 1f, 0f);

        SceneManager.MoveGameObjectToScene(foreignAnchor, foreignScene);
        SceneManager.MoveGameObjectToScene(townAnchor, townScene);
        SceneManager.MoveGameObjectToScene(resident, townScene);

        InvokeInstance(director, "RecoverReleasedTownResidentsWithoutSpawnStates", townScene);

        Assert.That(GetPropertyValue(roamController, "HomeAnchor"), Is.EqualTo(townAnchor.transform),
            "Town 返场补绑时，应优先绑定当前 Town scene 的 homeAnchor，不能被别的 scene 同名对象污染。");
    }

    [Test]
    public void CrowdDirector_ShouldThrottleReleasedTownResidentRecoveryAcrossMultiplePasses()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type manifestType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest");
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_RecoveryThrottle")).AddComponent(storyManagerType);
        Component springDirector = Track(new GameObject("SpringDay1Director_RecoveryThrottle")).AddComponent(springDirectorType);
        Component director = Track(new GameObject("CrowdDirector_RecoveryThrottle")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", springDirector);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FreeTime"), false);

        ScriptableObject manifest = ScriptableObject.CreateInstance(manifestType);
        Track(manifest);
        string[] residentIds = { "701", "702", "703", "704", "705", "706", "707" };
        Array entryArray = Array.CreateInstance(entryType, residentIds.Length);
        for (int index = 0; index < residentIds.Length; index++)
        {
            string residentId = residentIds[index];
            object entry = Activator.CreateInstance(entryType);
            SetField(entry, "npcId", residentId);
            SetField(entry, "anchorObjectName", string.Empty);
            SetField(entry, "semanticAnchorIds", Array.Empty<string>());
            SetField(entry, "spawnOffset", Vector2.zero);
            SetField(entry, "fallbackWorldPosition", Vector2.zero);
            SetField(entry, "initialFacing", Vector2.down);
            entryArray.SetValue(entry, index);

            GameObject resident = Track(new GameObject(residentId));
            GameObject homeAnchor = Track(new GameObject($"{residentId}_HomeAnchor"));
            resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
            resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
            resident.transform.position = new Vector3(index * 1.2f, 0f, 0f);
            homeAnchor.transform.position = resident.transform.position;
            SceneManager.MoveGameObjectToScene(resident, townScene);
            SceneManager.MoveGameObjectToScene(homeAnchor, townScene);
        }

        InvokeInstance(manifest, "SetEntries", entryArray);
        SetField(director, "_manifest", manifest);

        bool hasMoreAfterFirstPass = (bool)InvokeInstance(director, "RecoverReleasedTownResidentsWithoutSpawnStates", townScene);
        IDictionary spawnStates = (IDictionary)GetFieldValue(director, "_spawnStates");

        Assert.That(spawnStates.Count, Is.EqualTo(3),
            "Town re-entry recover 第一拍应只补一个更小的批次，避免把整批 resident 同帧补绑回来。");
        Assert.That(hasMoreAfterFirstPass, Is.True);

        bool hasMoreAfterSecondPass = (bool)InvokeInstance(director, "RecoverReleasedTownResidentsWithoutSpawnStates", townScene);
        Assert.That(spawnStates.Count, Is.EqualTo(6));
        Assert.That(hasMoreAfterSecondPass, Is.True);

        bool hasMoreAfterThirdPass = (bool)InvokeInstance(director, "RecoverReleasedTownResidentsWithoutSpawnStates", townScene);

        Assert.That(spawnStates.Count, Is.EqualTo(7));
        Assert.That(hasMoreAfterThirdPass, Is.False);
    }

    [Test]
    public void CrowdDirector_ShouldRestartAutonomousRoamWhenApplyingNeutralResidentRuntimeSnapshot()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type snapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+ResidentRuntimeSnapshotEntry");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");

        Component director = Track(new GameObject("CrowdDirector_NeutralSnapshotRestart")).AddComponent(crowdDirectorType);
        GameObject residentRoot = Track(new GameObject("TestResidentRoot"));
        GameObject residentDefault = Track(new GameObject("Resident_DefaultPresent"));
        GameObject resident = Track(new GameObject("103"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        GameObject homeAnchor = Track(new GameObject("103_HomeAnchor"));

        SceneManager.MoveGameObjectToScene(residentRoot, townScene);
        SceneManager.MoveGameObjectToScene(residentDefault, townScene);
        SceneManager.MoveGameObjectToScene(resident, townScene);
        SceneManager.MoveGameObjectToScene(homeAnchor, townScene);
        residentDefault.transform.SetParent(residentRoot.transform, false);
        resident.transform.SetParent(residentDefault.transform, true);
        homeAnchor.transform.position = new Vector3(2f, 1f, 0f);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type beatSnapshotType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+BeatConsumptionSnapshot");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "103");
        SetField(entry, "prefab", resident);
        SetField(entry, "semanticAnchorIds", Array.Empty<string>());
        SetField(entry, "anchorObjectName", string.Empty);
        SetField(entry, "spawnOffset", Vector2.zero);
        SetField(entry, "fallbackWorldPosition", Vector2.zero);
        SetField(entry, "initialFacing", Vector2.left);
        SetField(entry, "residentBaseline", Enum.Parse(residentBaselineType, "DaytimeResident"));

        object beatConsumption = Activator.CreateInstance(beatSnapshotType);
        InvokeInstance(director, "GetOrCreateState", entry, string.Empty, beatConsumption);

        SetField(roamController, "state", ParseEnum(roamStateType, "ShortPause"));
        SetField(roamController, "stateTimer", 99f);

        object snapshot = Activator.CreateInstance(snapshotType);
        SetField(snapshot, "npcId", "103");
        SetField(snapshot, "sceneName", "Town");
        SetField(snapshot, "residentGroupKey", "Resident_DefaultPresent");
        SetField(snapshot, "resolvedAnchorName", "resident:Resident_DefaultPresent:free");
        SetField(snapshot, "positionX", 7.5f);
        SetField(snapshot, "positionY", -3f);
        SetField(snapshot, "facingX", -1f);
        SetField(snapshot, "facingY", 0f);
        SetField(snapshot, "isActive", true);
        SetField(snapshot, "isReturningHome", false);
        SetField(snapshot, "underDirectorCue", false);

        System.Collections.IList snapshots = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(snapshotType));
        snapshots.Add(snapshot);
        InvokeStatic(crowdDirectorType, "ApplyResidentRuntimeSnapshots", snapshots);

        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.False);
        Assert.That((bool)GetPropertyValue(roamController, "IsRoaming"), Is.True,
            "中性 crowd snapshot 恢复后，resident 应立刻回到真正自治，而不是停留在表面 roam、内部坏态。");
        Assert.That((float)GetFieldValue(roamController, "stateTimer"), Is.Not.EqualTo(99f).Within(0.001f),
            "如果 neutral snapshot 恢复后不重启 roam runtime，卡住 NPC 仍会只能靠聊天 reset 才恢复。");
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
    public void CrowdDirector_ShouldSuppressEnterVillagePostEntryCrowdCueWhenTownHouseLeadRuntimeStarts()
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
        SetField(springDirector, "_townHouseLeadWaitingForPlayer", true);

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
    public void CrowdDirector_ShouldKeepEnterVillagePostEntryCueWhileVillageGateOnlyQueued()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type dialogueManagerType = ResolveTypeOrFail("Sunset.Story.DialogueManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_EnterVillageQueued")).AddComponent(storyManagerType);
        Component springDirector = Track(new GameObject("SpringDay1Director_EnterVillageQueued")).AddComponent(springDirectorType);
        Component dialogueManager = Track(new GameObject("DialogueManager_EnterVillageQueued")).AddComponent(dialogueManagerType);
        Track(new GameObject("CrowdDirector_EnterVillageQueued")).AddComponent(crowdDirectorType);

        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", springDirector);
        SetStaticField(dialogueManagerType, "<Instance>k__BackingField", dialogueManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "EnterVillage"), false);
        SetField(springDirector, "_villageGateSequencePlayed", true);

        bool shouldSuppress = (bool)InvokeStatic(
            crowdDirectorType,
            "ShouldSuppressEnterVillageCrowdCueForTownHouseLead",
            "EnterVillage_PostEntry");

        Assert.That(shouldSuppress, Is.False, "VillageGate 只是排队但对白还没真正开始/完成时，不该提前放人；否则 resident 会先走两步再被下一次 Sync 抓回。");
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
    public void Director_ShouldKeepVillageGateActorsPlacedWhileVillageGateDialogueActive()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type dialogueManagerType = ResolveTypeOrFail("Sunset.Story.DialogueManager");
        Type dialogueSequenceType = ResolveTypeOrFail("Sunset.Story.DialogueSequenceSO");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_VillageGatePlacement")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_VillageGatePlacement")).AddComponent(springDirectorType);
        Component dialogueManager = Track(new GameObject("DialogueManager_VillageGatePlacement")).AddComponent(dialogueManagerType);

        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        SetStaticField(dialogueManagerType, "<Instance>k__BackingField", dialogueManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "EnterVillage"), false);

        SetField(director, "_villageGateSequencePlayed", true);
        SetField(director, "_townVillageGateActorsPlaced", true);

        ScriptableObject activeSequence = Track(ScriptableObject.CreateInstance(dialogueSequenceType) as ScriptableObject);
        SetField(activeSequence, "sequenceId", "spring-day1-village-gate");
        SetField(dialogueManager, "_currentSequence", activeSequence);
        SetField(dialogueManager, "<IsDialogueActive>k__BackingField", true);

        InvokeInstance(director, "TryHandleTownEnterVillageFlow");

        Assert.That((bool)GetFieldValue(director, "_townVillageGateActorsPlaced"), Is.True,
            "VillageGate 对白已经开始后，不应再把 opening story actor 当成“未摆位”并重置回起点。");
    }

    [Test]
    public void Director_ShouldPreferVillageCrowdSceneMarkersForDinnerStoryRoute()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type roamControllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Component director = Track(new GameObject("SpringDay1Director_DinnerRouteMarkers")).AddComponent(springDirectorType);

        GameObject crowdRoot = Track(new GameObject("进村围观"));
        GameObject startGroup = Track(new GameObject("起点"));
        GameObject endGroup = Track(new GameObject("终点"));
        startGroup.transform.SetParent(crowdRoot.transform, false);
        endGroup.transform.SetParent(crowdRoot.transform, false);

        GameObject chiefStart = Track(new GameObject("001起点"));
        chiefStart.transform.SetParent(startGroup.transform, false);
        chiefStart.transform.position = new Vector3(-11.60f, 15.26f, 0f);

        GameObject chiefEnd = Track(new GameObject("001终点"));
        chiefEnd.transform.SetParent(endGroup.transform, false);
        chiefEnd.transform.position = new Vector3(-12.55f, 14.52f, 0f);

        GameObject chief = Track(new GameObject("NPC001"));
        chief.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component chiefRoam = chief.AddComponent(roamControllerType);
        chief.transform.position = new Vector3(3f, 3f, 0f);

        GameObject homeAnchor = Track(new GameObject("001_HomeAnchor"));
        homeAnchor.transform.position = new Vector3(28f, 28f, 0f);

        SceneManager.MoveGameObjectToScene(crowdRoot, townScene);
        SceneManager.MoveGameObjectToScene(chief, townScene);
        SceneManager.MoveGameObjectToScene(homeAnchor, townScene);
        InvokeInstance(chiefRoam, "BindResidentHomeAnchor", homeAnchor.transform);

        MethodInfo routeMethod = springDirectorType.GetMethod(
            "TryResolveDinnerStoryActorRoute",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(routeMethod, Is.Not.Null, "晚饭 story actor 路线应可被回归测试直接钉住。");

        object[] args = { "001", chief.transform, null, null };
        bool resolved = (bool)routeMethod.Invoke(director, args);

        Assert.That(resolved, Is.True);
        Assert.That((Vector3)args[2], Is.EqualTo(chiefStart.transform.position),
            "晚饭剧情应优先沿用场景同一套起点 authored markers，而不是退回 home-anchor fallback。");
        Assert.That((Vector3)args[3], Is.EqualTo(chiefEnd.transform.position),
            "晚饭剧情应优先沿用场景同一套终点 authored markers，而不是再从别的地方乱走一遍。");
    }

    [Test]
    public void Director_ShouldResolveDinnerRouteForThirdResidentFromSceneMarkers()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type roamControllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Component director = Track(new GameObject("SpringDay1Director_DinnerThirdResident")).AddComponent(springDirectorType);

        GameObject crowdRoot = Track(new GameObject("进村围观"));
        GameObject startGroup = Track(new GameObject("起点"));
        startGroup.transform.SetParent(crowdRoot.transform, false);
        GameObject endGroup = Track(new GameObject("终点"));
        endGroup.transform.SetParent(crowdRoot.transform, false);

        GameObject thirdStart = Track(new GameObject("003起点"));
        thirdStart.transform.SetParent(startGroup.transform, false);
        thirdStart.transform.position = new Vector3(-6.2f, 12.4f, 0f);

        GameObject thirdEnd = Track(new GameObject("003终点"));
        thirdEnd.transform.SetParent(endGroup.transform, false);
        thirdEnd.transform.position = new Vector3(-4.7f, 11.3f, 0f);

        GameObject third = Track(new GameObject("NPC003"));
        third.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component thirdRoam = third.AddComponent(roamControllerType);
        third.transform.position = new Vector3(6f, 2f, 0f);

        GameObject homeAnchor = Track(new GameObject("003_HomeAnchor"));
        homeAnchor.transform.position = new Vector3(32f, 24f, 0f);

        SceneManager.MoveGameObjectToScene(crowdRoot, townScene);
        SceneManager.MoveGameObjectToScene(third, townScene);
        SceneManager.MoveGameObjectToScene(homeAnchor, townScene);
        InvokeInstance(thirdRoam, "BindResidentHomeAnchor", homeAnchor.transform);

        MethodInfo routeMethod = springDirectorType.GetMethod(
            "TryResolveDinnerStoryActorRoute",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(routeMethod, Is.Not.Null);

        object[] args = { "003", third.transform, null, null };
        bool resolved = (bool)routeMethod.Invoke(director, args);

        Assert.That(resolved, Is.True);
        Assert.That((Vector3)args[2], Is.EqualTo(thirdStart.transform.position));
        Assert.That((Vector3)args[3], Is.EqualTo(thirdEnd.transform.position));
    }

    [Test]
    public void Director_ShouldPreferGroupedVillageCrowdMarkersOverBareAnchorRoot()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type roamControllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Component director = Track(new GameObject("SpringDay1Director_DinnerGroupedMarkers")).AddComponent(springDirectorType);

        GameObject bareRoot = Track(new GameObject("EnterVillageCrowdRoot"));
        SceneManager.MoveGameObjectToScene(bareRoot, townScene);

        GameObject crowdRoot = Track(new GameObject("进村围观"));
        GameObject startGroup = Track(new GameObject("起点"));
        GameObject endGroup = Track(new GameObject("终点"));
        startGroup.transform.SetParent(crowdRoot.transform, false);
        endGroup.transform.SetParent(crowdRoot.transform, false);

        GameObject chiefStart = Track(new GameObject("001起点"));
        chiefStart.transform.SetParent(startGroup.transform, false);
        chiefStart.transform.position = new Vector3(-11.60f, 15.26f, 0f);

        GameObject chiefEnd = Track(new GameObject("001终点"));
        chiefEnd.transform.SetParent(endGroup.transform, false);
        chiefEnd.transform.position = new Vector3(-12.55f, 14.52f, 0f);

        GameObject chief = Track(new GameObject("NPC001"));
        chief.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component chiefRoam = chief.AddComponent(roamControllerType);
        chief.transform.position = new Vector3(3f, 3f, 0f);

        GameObject homeAnchor = Track(new GameObject("001_HomeAnchor"));
        homeAnchor.transform.position = new Vector3(28f, 28f, 0f);

        SceneManager.MoveGameObjectToScene(crowdRoot, townScene);
        SceneManager.MoveGameObjectToScene(chief, townScene);
        SceneManager.MoveGameObjectToScene(homeAnchor, townScene);
        InvokeInstance(chiefRoam, "BindResidentHomeAnchor", homeAnchor.transform);

        MethodInfo routeMethod = springDirectorType.GetMethod(
            "TryResolveDinnerStoryActorRoute",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(routeMethod, Is.Not.Null);

        object[] args = { "001", chief.transform, null, null };
        bool resolved = (bool)routeMethod.Invoke(director, args);

        Assert.That(resolved, Is.True);
        Assert.That((Vector3)args[2], Is.EqualTo(chiefStart.transform.position),
            "同时存在 bare EnterVillageCrowdRoot 和真正的进村围观分组时，导演应优先吃分组里的 authored 起点。");
        Assert.That((Vector3)args[3], Is.EqualTo(chiefEnd.transform.position),
            "终点也应优先取自分组 markers，而不是被 bare root 误导到 fallback。");
    }

    [Test]
    public void Director_ShouldIgnoreBareEnterVillageCrowdRootWhenDinnerMarkersAreMissing()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Component director = Track(new GameObject("SpringDay1Director_DinnerBareRootIgnored")).AddComponent(springDirectorType);

        GameObject bareRoot = Track(new GameObject("EnterVillageCrowdRoot"));
        GameObject startGroup = Track(new GameObject("起点"));
        GameObject endGroup = Track(new GameObject("终点"));
        startGroup.transform.SetParent(bareRoot.transform, false);
        endGroup.transform.SetParent(bareRoot.transform, false);

        GameObject chiefStart = Track(new GameObject("001起点"));
        chiefStart.transform.SetParent(startGroup.transform, false);
        chiefStart.transform.position = new Vector3(-11.60f, 15.26f, 0f);

        GameObject chiefEnd = Track(new GameObject("001终点"));
        chiefEnd.transform.SetParent(endGroup.transform, false);
        chiefEnd.transform.position = new Vector3(-12.55f, 14.52f, 0f);

        GameObject chief = Track(new GameObject("NPC001"));
        chief.transform.position = new Vector3(3f, 3f, 0f);

        SceneManager.MoveGameObjectToScene(bareRoot, townScene);
        SceneManager.MoveGameObjectToScene(chief, townScene);

        MethodInfo routeMethod = springDirectorType.GetMethod(
            "TryResolveDinnerStoryActorRoute",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(routeMethod, Is.Not.Null);

        object[] args = { "001", chief.transform, null, null };
        bool resolved = (bool)routeMethod.Invoke(director, args);

        Assert.That(resolved, Is.False, "bare EnterVillageCrowdRoot 不再是晚饭剧情走位真值；没有进村围观分组 markers 就不能生成路线。");
    }

    [Test]
    public void Director_ShouldRequireTownOpeningStartAndEndMarkersInGroupedRoot()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Component director = Track(new GameObject("SpringDay1Director_OpeningRequiresMarkers")).AddComponent(springDirectorType);

        GameObject crowdRoot = Track(new GameObject("进村围观"));
        GameObject endGroup = Track(new GameObject("终点"));
        endGroup.transform.SetParent(crowdRoot.transform, false);

        GameObject chiefEnd = Track(new GameObject("001终点"));
        chiefEnd.transform.SetParent(endGroup.transform, false);
        chiefEnd.transform.position = new Vector3(-12.55f, 14.52f, 0f);

        GameObject chief = Track(new GameObject("NPC001"));
        chief.transform.position = new Vector3(3f, 3f, 0f);

        SceneManager.MoveGameObjectToScene(crowdRoot, townScene);
        SceneManager.MoveGameObjectToScene(chief, townScene);

        MethodInfo routeMethod = springDirectorType.GetMethod(
            "TryResolveTownVillageGateActorRoute",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(routeMethod, Is.Not.Null);

        object[] args = { "001", chief.transform, null, true, null, null };
        bool resolved = (bool)routeMethod.Invoke(director, args);

        Assert.That(resolved, Is.False, "opening 已经存在进村围观 authored markers 时，不应在缺起点的情况下偷用终点或 stage book 兜底。");
    }

    [Test]
    public void CrowdDirector_ShouldRequireSceneMarkersForDinnerCue()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "101");
        SetField(entry, "spawnOffset", new Vector2(1.6f, -0.55f));
        SetField(entry, "initialFacing", Vector2.left);
        SetField(entry, "semanticAnchorIds", new[] { "DinnerBackgroundRoot", "DailyStand_01" });

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        MethodInfo resolveMethod = crowdDirectorType.GetMethod(
            "TryResolveStagingCueForCurrentScene",
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.That(resolveMethod, Is.Not.Null);

        object[] args = { "DinnerConflict_Table", entry, null };
        bool resolved = (bool)resolveMethod.Invoke(null, args);

        Assert.That(resolved, Is.False, "晚饭没有 scene markers 时，不应再伪造 DirectorReady/ResidentSlot 这种抽象 fallback cue。");
        Assert.That(args[2], Is.Null);
    }

    [Test]
    public void CrowdDirector_ShouldBuildDinnerCueFromSceneMarkers()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        GameObject crowdRoot = Track(new GameObject("进村围观"));
        GameObject startGroup = Track(new GameObject("起点"));
        GameObject endGroup = Track(new GameObject("终点"));
        startGroup.transform.SetParent(crowdRoot.transform, false);
        endGroup.transform.SetParent(crowdRoot.transform, false);
        SceneManager.MoveGameObjectToScene(crowdRoot, townScene);

        GameObject markerStart = Track(new GameObject("101起点"));
        markerStart.transform.SetParent(startGroup.transform, false);
        markerStart.transform.position = new Vector3(-8.4f, 12.1f, 0f);

        GameObject markerEnd = Track(new GameObject("101终点"));
        markerEnd.transform.SetParent(endGroup.transform, false);
        markerEnd.transform.position = new Vector3(-9.1f, 11.4f, 0f);

        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "101");
        SetField(entry, "spawnOffset", new Vector2(1.6f, -0.55f));
        SetField(entry, "initialFacing", Vector2.left);
        SetField(entry, "semanticAnchorIds", new[] { "DinnerBackgroundRoot", "DailyStand_01" });

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        MethodInfo resolveMethod = crowdDirectorType.GetMethod(
            "TryResolveStagingCueForCurrentScene",
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.That(resolveMethod, Is.Not.Null);

        object[] resolveArgs = { "DinnerConflict_Table", entry, null };
        bool resolved = (bool)resolveMethod.Invoke(null, resolveArgs);
        Assert.That(resolved, Is.True);

        object runtimeCue = resolveArgs[2];
        Assert.That(runtimeCue, Is.Not.Null);
        Assert.That((Vector2)GetFieldValue(runtimeCue, "startPosition"), Is.EqualTo((Vector2)markerStart.transform.position),
            "晚饭有场景起点时，cue 应直接吃 authored marker。");

        Array path = (Array)GetFieldValue(runtimeCue, "path");
        Assert.That(path, Is.Not.Null);
        Assert.That(path.Length, Is.EqualTo(1));
        object pathPoint = path.GetValue(0);
        Assert.That((Vector2)GetFieldValue(pathPoint, "position"), Is.EqualTo((Vector2)markerEnd.transform.position),
            "晚饭有场景终点时，cue 应直接走到对应 NPC 的终点 marker。");
    }

    [Test]
    public void CrowdDirector_ShouldKeepDinnerCueLockedDuringReturnReminderBlock()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type manifestType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest");
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");
        Type playbackType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingPlayback");

        Component storyManager = Track(new GameObject("StoryManager_DinnerReminderCueLock")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_DinnerReminderCueLock")).AddComponent(springDirectorType);
        Component crowdDirector = Track(new GameObject("CrowdDirector_DinnerReminderCueLock")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        SetStaticField(crowdDirectorType, "_instance", crowdDirector);

        GameObject crowdRoot = Track(new GameObject("进村围观"));
        GameObject startGroup = Track(new GameObject("起点"));
        GameObject endGroup = Track(new GameObject("终点"));
        startGroup.transform.SetParent(crowdRoot.transform, false);
        endGroup.transform.SetParent(crowdRoot.transform, false);
        SceneManager.MoveGameObjectToScene(crowdRoot, townScene);

        CreateMarker(startGroup.transform, "101起点", new Vector3(-8.4f, 12.1f, 0f));
        CreateMarker(endGroup.transform, "101终点", new Vector3(-9.1f, 11.4f, 0f));

        ScriptableObject manifest = ScriptableObject.CreateInstance(manifestType);
        Track(manifest);
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "101");
        SetField(entry, "anchorObjectName", string.Empty);
        SetField(entry, "spawnOffset", new Vector2(1.6f, -0.55f));
        SetField(entry, "fallbackWorldPosition", Vector2.zero);
        SetField(entry, "initialFacing", Vector2.left);
        SetField(entry, "semanticAnchorIds", new[] { "DinnerBackgroundRoot", "DailyStand_01" });
        SetField(entry, "residentBaseline", Enum.Parse(residentBaselineType, "DaytimeResident"));
        Array entries = Array.CreateInstance(entryType, 1);
        entries.SetValue(entry, 0);
        InvokeInstance(manifest, "SetEntries", entries);
        SetField(crowdDirector, "_manifest", manifest);

        GameObject resident = Track(new GameObject("101"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        SceneManager.MoveGameObjectToScene(resident, townScene);

        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "DinnerConflict"), false);
        SetField(crowdDirector, "_syncRequested", true);
        InvokeInstance(crowdDirector, "Update");

        IDictionary spawnStates = (IDictionary)GetFieldValue(crowdDirector, "_spawnStates");
        Assert.That(spawnStates.Contains("101"), Is.True);
        object state = spawnStates["101"];
        Component playback = resident.GetComponent(playbackType);
        Assert.That(playback, Is.Not.Null);
        Assert.That((string)GetFieldValue(state, "AppliedBeatKey"), Is.EqualTo("DinnerConflict_Table"));
        Assert.That((string)GetPropertyValue(playback, "CurrentBeatKey"), Is.EqualTo("DinnerConflict_Table"));

        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "ReturnAndReminder"), false);
        SetField(crowdDirector, "_syncRequested", true);
        InvokeInstance(crowdDirector, "Update");

        Assert.That((string)GetFieldValue(state, "AppliedBeatKey"), Is.EqualTo("DinnerConflict_Table"),
            "19:00 的 reminder block 仍应继续持有晚饭那一份 staged cue，不能在剧情块中段再换成第二套 beat。");
        Assert.That((string)GetPropertyValue(playback, "CurrentBeatKey"), Is.EqualTo("DinnerConflict_Table"),
            "进入 reminder block 后，不应把 dinner staged playback 重开成新的 beat，从而触发二次走位。");
    }

    [Test]
    public void Director_ShouldNotTreatQueuedVillageGateSequenceAsHouseLeadReady()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type dialogueManagerType = ResolveTypeOrFail("Sunset.Story.DialogueManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_VillageGatePending")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_VillageGatePending")).AddComponent(springDirectorType);
        Component dialogueManager = Track(new GameObject("DialogueManager_VillageGatePending")).AddComponent(dialogueManagerType);

        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        SetStaticField(dialogueManagerType, "<Instance>k__BackingField", dialogueManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "EnterVillage"), false);

        SetField(director, "_villageGateSequencePlayed", true);
        SetField(director, "_townVillageGateActorsPlaced", true);

        InvokeInstance(director, "TryHandleTownEnterVillageFlow");

        Assert.That((bool)InvokeInstance(director, "IsTownHouseLeadPending"), Is.False,
            "VillageGate 只是 queued 但还没真正完成时，导演不应把 opening 直接推进成 house lead。");
        Assert.That((bool)GetFieldValue(director, "_townVillageGateActorsPlaced"), Is.True,
            "对白还没真正收口前，不应把 001~003 又重置回“待摆位”。");
    }

    [Test]
    public void Director_ShouldPlaceVillageGateActorsAtAuthoredStartsBeforeDialogueCanBegin()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);
        ResetStaticField(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"), "_instance");

        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type motionControllerType = ResolveTypeOrFail("NPCMotionController");
        Type roamControllerType = ResolveTypeOrFail("NPCAutoRoamController");

        Component storyManager = Track(new GameObject("StoryManager_VillageGateStarts")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_VillageGateStarts")).AddComponent(springDirectorType);

        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "EnterVillage"), false);

        GameObject layoutRoot = Track(new GameObject("进村围观"));
        GameObject endGroup = Track(new GameObject("终点"));
        endGroup.transform.SetParent(layoutRoot.transform, false);

        GameObject chief = Track(new GameObject("001"));
        chief.AddComponent(motionControllerType);
        chief.AddComponent(roamControllerType);
        chief.transform.position = new Vector3(20f, 20f, 0f);

        GameObject companion = Track(new GameObject("002"));
        companion.AddComponent(motionControllerType);
        companion.AddComponent(roamControllerType);
        companion.transform.position = new Vector3(21f, 20f, 0f);

        GameObject thirdResident = Track(new GameObject("003"));
        thirdResident.AddComponent(motionControllerType);
        thirdResident.AddComponent(roamControllerType);
        thirdResident.transform.position = new Vector3(22f, 20f, 0f);

        CreateMarker(layoutRoot.transform, "001起点", new Vector3(-5f, 3f, 0f));
        CreateMarker(layoutRoot.transform, "002起点", new Vector3(-4f, 3.25f, 0f));
        CreateMarker(layoutRoot.transform, "003起点", new Vector3(-3f, 3.5f, 0f));
        CreateMarker(endGroup.transform, "001终点", new Vector3(2f, 0f, 0f));
        CreateMarker(endGroup.transform, "002终点", new Vector3(2.5f, 0.2f, 0f));
        CreateMarker(endGroup.transform, "003终点", new Vector3(3f, 0.4f, 0f));

        InvokeInstance(director, "TryHandleTownEnterVillageFlow");

        Assert.That((bool)GetFieldValue(director, "_townVillageGateActorsPlaced"), Is.True,
            "opening 等待 crowd cue settled 的窗口里，001~003 也应先被摆到各自起点并开始这一次 staged run。");
        Assert.That((bool)GetFieldValue(director, "_villageGateSequencePlayed"), Is.False,
            "只摆到起点并开始走位，不等于对白已经可以开。");
        Assert.That(chief.transform.position.x, Is.EqualTo(-5f).Within(0.001f));
        Assert.That(chief.transform.position.y, Is.EqualTo(3f).Within(0.001f));
        Assert.That(companion.transform.position.x, Is.EqualTo(-4f).Within(0.001f));
        Assert.That(companion.transform.position.y, Is.EqualTo(3.25f).Within(0.001f));
        Assert.That(thirdResident.transform.position.x, Is.EqualTo(-3f).Within(0.001f));
        Assert.That(thirdResident.transform.position.y, Is.EqualTo(3.5f).Within(0.001f));
    }

    [Test]
    public void CrowdDirector_ShouldKeepOpeningResidentsOnBaselineReleasePathDuringEnterVillage()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");

        Component storyManager = Track(new GameObject("StoryManager_EnterVillageYieldGuard")).AddComponent(storyManagerType);
        Component crowdDirector = Track(new GameObject("CrowdDirector_EnterVillageYieldGuard")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "EnterVillage"), false);

        GameObject npc = Track(new GameObject("Resident_EnterVillageYieldGuard"));
        GameObject homeAnchor = Track(new GameObject("Resident_EnterVillageYieldGuard_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        homeAnchor.transform.position = new Vector3(6f, -2f, 0f);

        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_EnterVillageYieldGuard_HomeAnchor");

        IDictionary spawnStates = (IDictionary)GetFieldValue(crowdDirector, "_spawnStates");
        spawnStates["101"] = state;

        bool shouldYield = (bool)InvokeInstance(crowdDirector, "ShouldYieldDaytimeResidentsToAutonomy");

        Assert.That(shouldYield, Is.False, "opening 还在 EnterVillage 时，release 必须继续走 baseline/release contract，不能短路成 direct autonomy。");
    }

    [Test]
    public void CrowdDirector_ShouldNotResnapResidentToBasePositionOnRepeatedBaselineSync()
    {
        Component director = Track(new GameObject("CrowdDirector_Test")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject defaultRoot = Track(new GameObject("Resident_DefaultPresent"));

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
    public void CrowdDirector_ShouldReleaseOpeningResidentToSharedBaselineAfterEnterVillageCrowdReleaseLatches()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_OpeningReleaseYield")).AddComponent(storyManagerType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "EnterVillage"), false);

        Component director = Track(new GameObject("CrowdDirector_DaytimeBaselineRelease")).AddComponent(crowdDirectorType);
        SetField(director, "_enterVillageCrowdReleaseLatched", true);
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
            "EnterVillage_PostEntry",
            beatConsumption);

        Assert.That(npc.transform.position.x, Is.EqualTo(6.5f).Within(0.01f));
        Assert.That(npc.transform.position.y, Is.EqualTo(-2.25f).Within(0.01f));
        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.False,
            "opening handoff release 后，Day1 不应继续把普通 resident 留在 crowd-owned return-home 生命周期里。");
        Assert.That((bool)GetFieldValue(state, "NeedsResidentReset"), Is.False);
        Assert.That((bool)GetFieldValue(state, "ReleasedFromDirectorCue"), Is.False);
        Assert.That(((Vector3)GetFieldValue(state, "BasePosition")).x, Is.EqualTo(2.5f).Within(0.01f));
        Assert.That(((Vector3)GetFieldValue(state, "BasePosition")).y, Is.EqualTo(4.75f).Within(0.01f));
        Assert.That((string)GetFieldValue(state, "BaseResolvedAnchorName"), Is.EqualTo("Resident_Returning_HomeAnchor"));
        StringAssert.Contains("baseline-release", (string)GetFieldValue(state, "ResolvedAnchorName"));
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.False,
            "opening handoff release 后，shared resident contract 应直接把 crowd owner 放掉。");
        Assert.That((bool)GetFieldValue(state, "QueuedAutonomousResume"), Is.True,
            "opening handoff release 后，ordinary resident 应进入受控自治恢复队列，而不是继续被 baseline teleport 或脚本 owner 持有。");

        InvokeInstance(
            director,
            "ApplyResidentBaseline",
            entry,
            state,
            Enum.Parse(storyPhaseType, "EnterVillage"),
            "EnterVillage_PostEntry",
            beatConsumption);

        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.False,
            "重复 SyncCrowd 后，opening handoff 已 release 的 resident 不应再被 crowd owner 重新抓回 return-home。");
        StringAssert.DoesNotContain("return-home", (string)GetFieldValue(state, "ResolvedAnchorName"));
        Assert.That(npc.transform.position.x, Is.EqualTo(6.5f).Within(0.01f));
        Assert.That(npc.transform.position.y, Is.EqualTo(-2.25f).Within(0.01f));
    }

    [Test]
    public void CrowdDirector_ShouldNotRetouchAlreadyAutonomousResidentsDuringDaytimeYield()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");

        Component storyManager = Track(new GameObject("StoryManager_DaytimeYieldAutonomy")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("CrowdDirector_DaytimeYieldAutonomy")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "HealingAndHP"), false);

        GameObject npc = Track(new GameObject("Resident_DaytimeAutonomyStable"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "BaseResolvedAnchorName", "Resident_DaytimeAutonomyStable_HomeAnchor");
        SetField(state, "ResolvedAnchorName", "baseline:Resident_DaytimeAutonomyStable_HomeAnchor");
        SetField(state, "NeedsResidentReset", false);
        SetField(state, "ReleasedFromDirectorCue", false);
        SetField(state, "IsReturningHome", false);
        SetField(state, "AppliedBeatKey", string.Empty);
        SetField(state, "AppliedCueKey", string.Empty);

        IDictionary spawnStates = (IDictionary)GetFieldValue(director, "_spawnStates");
        spawnStates["101"] = state;

        InvokeInstance(director, "YieldDaytimeResidentsToAutonomy");

        Assert.That((string)GetFieldValue(state, "ResolvedAnchorName"), Is.EqualTo("baseline:Resident_DaytimeAutonomyStable_HomeAnchor"),
            "daytime 自由态 resident 已经自治后，CrowdDirector 不应每轮 yield 再去重写它的 autonomy/release 状态。");
    }

    [Test]
    public void CrowdDirector_ShouldNotCancelResidentReturnHomeDuringDaytimeYield()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");

        Component storyManager = Track(new GameObject("StoryManager_DaytimeYieldReturnHome")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("CrowdDirector_DaytimeYieldReturnHome")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "HealingAndHP"), false);

        GameObject npc = Track(new GameObject("Resident_DaytimeReturnHomeStable"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "BaseResolvedAnchorName", "Resident_DaytimeReturnHomeStable_HomeAnchor");
        SetField(state, "ResolvedAnchorName", "return-home:Resident_DaytimeReturnHomeStable_HomeAnchor");
        SetField(state, "NeedsResidentReset", false);
        SetField(state, "ReleasedFromDirectorCue", false);
        SetField(state, "IsReturningHome", true);
        SetField(state, "ResumeRoamAfterReturn", true);
        SetField(state, "AppliedBeatKey", string.Empty);
        SetField(state, "AppliedCueKey", string.Empty);

        IDictionary spawnStates = (IDictionary)GetFieldValue(director, "_spawnStates");
        spawnStates["101"] = state;

        InvokeInstance(director, "YieldDaytimeResidentsToAutonomy");

        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.True,
            "resident 已经在回 anchor/home 时，daytime yield 不应再把它 cancel 掉。");
        Assert.That((bool)GetFieldValue(state, "ResumeRoamAfterReturn"), Is.True);
        Assert.That((string)GetFieldValue(state, "ResolvedAnchorName"), Is.EqualTo("return-home:Resident_DaytimeReturnHomeStable_HomeAnchor"));
    }

    [Test]
    public void CrowdDirector_ShouldThrottleDaytimeYieldAcrossMultipleUpdates()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");

        Component storyManager = Track(new GameObject("StoryManager_DaytimeYieldThrottle")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("CrowdDirector_DaytimeYieldThrottle")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "HealingAndHP"), false);

        IDictionary spawnStates = (IDictionary)GetFieldValue(director, "_spawnStates");
        List<object> states = new List<object>();
        for (int index = 0; index < 8; index++)
        {
            string npcId = (101 + index).ToString("000");
            GameObject npc = Track(new GameObject(npcId));
            npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
            npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
            SceneManager.MoveGameObjectToScene(npc, townScene);

            object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
            SetField(state, "Instance", npc);
            SetField(state, "BaseResolvedAnchorName", $"{npcId}_HomeAnchor");
            SetField(state, "ResolvedAnchorName", $"baseline:{npcId}_HomeAnchor");
            SetField(state, "NeedsResidentReset", true);
            SetField(state, "ReleasedFromDirectorCue", true);
            SetField(state, "IsReturningHome", false);
            SetField(state, "AppliedBeatKey", string.Empty);
            SetField(state, "AppliedCueKey", string.Empty);
            spawnStates[npcId] = state;
            states.Add(state);
        }

        InvokeInstance(director, "Update");

        int releasedAfterFirstUpdate = 0;
        for (int index = 0; index < states.Count; index++)
        {
            if (!(bool)GetFieldValue(states[index], "NeedsResidentReset"))
            {
                releasedAfterFirstUpdate++;
            }
        }

        Assert.That(releasedAfterFirstUpdate, Is.EqualTo(3),
            "daytime yield 第一拍应只释放一个小批次，避免一口气把整群 resident 同窗放回 autonomy。");
        Assert.That((bool)GetFieldValue(director, "_syncRequested"), Is.True);
        float retryDelay = (float)GetFieldValue(director, "_nextSyncAt") - Time.unscaledTime;
        Assert.That(retryDelay, Is.GreaterThan(0f).And.LessThan(0.1f),
            "如果还有待释放 resident，CrowdDirector 应在短窗内尽快续跑下一批，而不是拖回常规 0.35 秒节拍。");

        SetField(director, "_syncRequested", true);
        SetField(director, "_nextSyncAt", Time.unscaledTime - 0.01f);
        InvokeInstance(director, "Update");

        int releasedAfterSecondUpdate = 0;
        for (int index = 0; index < states.Count; index++)
        {
            if (!(bool)GetFieldValue(states[index], "NeedsResidentReset"))
            {
                releasedAfterSecondUpdate++;
            }
        }

        Assert.That(releasedAfterSecondUpdate, Is.EqualTo(6));
        Assert.That((bool)GetFieldValue(director, "_syncRequested"), Is.True);
        float secondRetryDelay = (float)GetFieldValue(director, "_nextSyncAt") - Time.unscaledTime;
        Assert.That(secondRetryDelay, Is.GreaterThan(0f).And.LessThan(0.1f),
            "还有 resident 待释放时，第二拍也应继续保持短窗 retry，而不是误判为已全部收完。");
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
    public void CrowdDirector_ShouldQueueClockReturnHomeWithoutRoamResumeAtTwenty()
    {
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Component timeManager = Track(new GameObject("TimeManager_FreeTimeAtTwenty_NoResume")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 20);
        SetField(timeManager, "currentMinute", 0);

        Component director = Track(new GameObject("CrowdDirector_FreeTimeReturnHome_NoResume")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject npc = Track(new GameObject("Resident_FreeTimeReturning_NoResume"));
        GameObject homeAnchor = Track(new GameObject("Resident_FreeTimeReturning_NoResume_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        npc.transform.position = new Vector3(6.5f, -2.25f, 0f);
        homeAnchor.transform.position = new Vector3(1.5f, 0.75f, 0f);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_FreeTimeReturning_NoResume_HomeAnchor");
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

        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.True);
        Assert.That((bool)GetFieldValue(state, "ResumeRoamAfterReturn"), Is.False,
            "20:00 的 clock-owned return-home 应是回家并停住，不应在到家后又恢复 roam。");
    }

    [Test]
    public void CrowdDirector_ShouldNotReissueReturnHomeDriveWhileScriptedMoveIsAlreadyActive()
    {
        Component director = Track(new GameObject("CrowdDirector_ReturnHomeActiveDrive")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject npc = Track(new GameObject("Resident_ReturnActiveDrive"));
        GameObject homeAnchor = Track(new GameObject("Resident_ReturnActiveDrive_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        npc.transform.position = new Vector3(-4f, 1.5f, 0f);
        homeAnchor.transform.position = new Vector3(6f, 1.5f, 0f);

        SetField(roamController, "state", ParseEnum(roamStateType, "Moving"));
        SetField(roamController, "debugMoveActive", true);
        SetField(roamController, "residentScriptedMovePaused", false);
        SetField(roamController, "residentScriptedControlOwnerKey", "SpringDay1NpcCrowdDirector");
        ((IList)GetFieldValue(roamController, "residentScriptedControlOwners")).Add("SpringDay1NpcCrowdDirector");

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_ReturnActiveDrive_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.right);
        SetField(state, "NeedsResidentReset", true);
        SetField(state, "ReleasedFromDirectorCue", true);
        SetField(state, "IsReturningHome", true);
        SetField(state, "ResumeRoamAfterReturn", true);
        SetField(state, "NextReturnHomeDriveRetryAt", -1f);

        InvokeInstance(director, "TickResidentReturnHome", state, 1f);

        Assert.That((float)GetFieldValue(state, "NextReturnHomeDriveRetryAt"), Is.EqualTo(-1f).Within(0.001f),
            "当 resident 的 scripted move 已经活着时，CrowdDirector 不应每帧重新下发同一条 return-home drive。");
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedMoveActive"), Is.True);
        StringAssert.Contains("return-home", (string)GetFieldValue(state, "ResolvedAnchorName"));
    }

    [Test]
    public void CrowdDirector_ShouldScheduleReturnHomeRetryWhenNoDriveIsActive()
    {
        Component director = Track(new GameObject("CrowdDirector_ReturnHomeRetryPending")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject npc = Track(new GameObject("Resident_ReturnHomeRetryPending"));
        GameObject homeAnchor = Track(new GameObject("Resident_ReturnHomeRetryPending_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        npc.transform.position = new Vector3(-6f, 1.5f, 0f);
        homeAnchor.transform.position = new Vector3(6f, 1.5f, 0f);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_ReturnHomeRetryPending_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.right);
        SetField(state, "NeedsResidentReset", true);
        SetField(state, "ReleasedFromDirectorCue", true);
        SetField(state, "IsReturningHome", true);
        SetField(state, "ResumeRoamAfterReturn", false);
        SetField(state, "NextReturnHomeDriveRetryAt", -1f);

        Vector3 startPosition = npc.transform.position;
        float retryBefore = Time.unscaledTime;
        InvokeInstance(director, "TickResidentReturnHome", state, 1f);

        Assert.That(npc.transform.position, Is.EqualTo(startPosition),
            "如果当前这次 still 没把回家导航起起来，Day1 不应手搓 transform fallback 替导航走路。");
        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.True,
            "没有 completion signal 前，return-home 语义本身仍应保留。");
        Assert.That((float)GetFieldValue(state, "NextReturnHomeDriveRetryAt"), Is.GreaterThan(retryBefore),
            "20:00 回家途中丢了 active drive 时，不应一直晾到 21:00；应留下短 retry 窗继续回家。");
        StringAssert.Contains("return-home-pending", (string)GetFieldValue(state, "ResolvedAnchorName"));
    }

    [Test]
    public void NpcAutoRoamController_ShouldExposeFormalNavigationArrivalOnceForMatchingOwner()
    {
        GameObject npc = Track(new GameObject("NPC_FormalNavigationArrival"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        Type travelContractType = ResolveTypeOrFail("NPCAutoRoamController+PointToPointTravelContract");

        npc.transform.position = new Vector3(4.5f, -1.25f, 0f);
        InvokeInstance(roamController, "AcquireResidentScriptedControl", "SpringDay1NpcCrowdDirector", false);
        SetField(roamController, "debugMoveActive", true);
        SetField(roamController, "activePointToPointTravelContract", ParseEnum(travelContractType, "FormalNavigation"));
        SetField(roamController, "state", ParseEnum(roamStateType, "Moving"));

        InvokeInstance(roamController, "EndDebugMove", true);

        object firstConsume = InvokeInstance(roamController, "ConsumeFormalNavigationArrival", "SpringDay1NpcCrowdDirector");
        Assert.That(firstConsume, Is.TypeOf<Vector2>(), "FormalNavigation 到达后应暴露一次可消费 arrival 信号。");
        Assert.That((Vector2)firstConsume, Is.EqualTo(new Vector2(4.5f, -1.25f)));
        Assert.That(InvokeInstance(roamController, "ConsumeFormalNavigationArrival", "SpringDay1NpcCrowdDirector"), Is.Null,
            "同一条 FormalNavigation arrival 信号只允许被消费一次。");
        Assert.That(InvokeInstance(roamController, "ConsumeFormalNavigationArrival", "spring-day1-director"), Is.Null,
            "不匹配的 ownerKey 不应拿到 Crowd 的 FormalNavigation arrival 信号。");
    }

    [Test]
    public void NpcAutoRoamController_ShouldExposeFormalNavigationRuntimeSignalsForActiveOwner()
    {
        GameObject npc = Track(new GameObject("NPC_FormalNavigationRuntimeSignals"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        Type travelContractType = ResolveTypeOrFail("NPCAutoRoamController+PointToPointTravelContract");

        npc.transform.position = new Vector3(6.25f, 2.75f, 0f);
        InvokeInstance(roamController, "AcquireResidentScriptedControl", "SpringDay1NpcCrowdDirector", false);
        SetField(roamController, "debugMoveActive", true);
        SetField(roamController, "activePointToPointTravelContract", ParseEnum(travelContractType, "FormalNavigation"));
        SetField(roamController, "state", ParseEnum(roamStateType, "Moving"));

        Assert.That((bool)InvokeInstance(roamController, "IsFormalNavigationDriveActive"), Is.True,
            "当 FormalNavigation scripted move 仍在跑时，runtime owner 应能通过语义口确认 drive 还活着。");

        MethodInfo tryConsumeArrivalMethod = roamController.GetType().GetMethod(
            "TryConsumeFormalNavigationArrival",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(tryConsumeArrivalMethod, Is.Not.Null);

        object[] beforeArrivalArgs = { null };
        Assert.That((bool)tryConsumeArrivalMethod.Invoke(roamController, beforeArrivalArgs), Is.False,
            "到达前不应提前暴露 FormalNavigation arrival 信号。");
        Assert.That((bool)InvokeInstance(roamController, "IsFormalNavigationDriveActive"), Is.True);

        InvokeInstance(roamController, "EndDebugMove", true);

        Assert.That((bool)InvokeInstance(roamController, "IsFormalNavigationDriveActive"), Is.False,
            "一旦 FormalNavigation scripted move 已结束，runtime owner 不应再把它当成 active drive。");

        object[] firstArrivalArgs = { null };
        Assert.That((bool)tryConsumeArrivalMethod.Invoke(roamController, firstArrivalArgs), Is.True,
            "runtime owner 应能直接消费一次性 FormalNavigation arrival 信号。");
        Assert.That((Vector2)firstArrivalArgs[0], Is.EqualTo(new Vector2(6.25f, 2.75f)));

        object[] secondArrivalArgs = { null };
        Assert.That((bool)tryConsumeArrivalMethod.Invoke(roamController, secondArrivalArgs), Is.False,
            "同一条 runtime arrival 信号只允许消费一次，不能让 CrowdDirector 在 tick 里重复 finish。");
    }

    [Test]
    public void CrowdDirector_ShouldFinishReturnHomeFromFormalNavigationArrivalSignal()
    {
        Component director = Track(new GameObject("CrowdDirector_ReturnHomeFormalArrival")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject npc = Track(new GameObject("Resident_ReturnFormalArrival"));
        GameObject homeAnchor = Track(new GameObject("Resident_ReturnFormalArrival_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        Type travelContractType = ResolveTypeOrFail("NPCAutoRoamController+PointToPointTravelContract");

        homeAnchor.transform.position = new Vector3(2f, 0f, 0f);
        npc.transform.position = homeAnchor.transform.position;
        InvokeInstance(roamController, "AcquireResidentScriptedControl", "SpringDay1NpcCrowdDirector", false);
        SetField(roamController, "debugMoveActive", true);
        SetField(roamController, "activePointToPointTravelContract", ParseEnum(travelContractType, "FormalNavigation"));
        SetField(roamController, "state", ParseEnum(roamStateType, "Moving"));
        InvokeInstance(roamController, "EndDebugMove", true);

        npc.transform.position = new Vector3(-4f, 1.5f, 0f);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_ReturnFormalArrival_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.right);
        SetField(state, "NeedsResidentReset", true);
        SetField(state, "ReleasedFromDirectorCue", true);
        SetField(state, "IsReturningHome", true);
        SetField(state, "ResumeRoamAfterReturn", false);

        InvokeInstance(director, "TickResidentReturnHome", state, 1f);

        Assert.That(npc.transform.position.x, Is.EqualTo(2f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(0f).Within(0.001f));
        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.False,
            "拿到 FormalNavigation arrival 信号后，CrowdDirector 应直接完成 return-home 收尾，而不是继续等 transform 距离轮询。");
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.False);
    }

    [Test]
    public void CrowdDirector_ShouldNotFallbackToManualStepReturnHomeWhenNavigationCannotStart()
    {
        Component director = Track(new GameObject("CrowdDirector_ReturnHomePathing")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject npc = Track(new GameObject("Resident_ReturnPathing"));
        GameObject homeAnchor = Track(new GameObject("Resident_ReturnPathing_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
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

        Assert.That(npc.transform.position, Is.EqualTo(startPosition), "如果导航这次没把回家路径起起来，Day1 不应再手搓 transform fallback 替导航走路。");
        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.True, "未到 HomeAnchor 前，return-home 语义应保持。");
        StringAssert.Contains("return-home", (string)GetFieldValue(state, "ResolvedAnchorName"));
    }

    [Test]
    public void CrowdDirector_ShouldReleaseSharedOwnerWhenReturnHomeCompletesWithoutRoamResume()
    {
        Component director = Track(new GameObject("CrowdDirector_ReturnHomeReleaseWithoutResume")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));
        GameObject npc = Track(new GameObject("Resident_ReturnComplete_NoResume"));
        GameObject homeAnchor = Track(new GameObject("Resident_ReturnComplete_NoResume_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        InvokeInstance(roamController, "AcquireResidentScriptedControl", "SpringDay1NpcCrowdDirector", false);
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.True);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_ReturnComplete_NoResume_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.left);
        SetField(state, "IsReturningHome", true);
        SetField(state, "ResumeRoamAfterReturn", false);
        SetField(state, "NeedsResidentReset", true);
        SetField(state, "ReleasedFromDirectorCue", true);

        InvokeInstance(director, "FinishResidentReturnHome", state);

        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.False,
            "回家合同完成但不续 roam 时，CrowdDirector 也必须把 shared owner 放掉。");
        Assert.That((bool)GetPropertyValue(roamController, "IsRoaming"), Is.False);
        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.False);
        Assert.That((bool)GetFieldValue(state, "NeedsResidentReset"), Is.False);
        Assert.That((bool)GetFieldValue(state, "ReleasedFromDirectorCue"), Is.False);
    }

    [Test]
    public void CrowdDirector_ShouldRestoreNightRestResidentsAtHomeAnchorWhenMorningReleases()
    {
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        GameObject npc = Track(new GameObject("Resident_MorningRelease"));
        GameObject homeAnchor = Track(new GameObject("Resident_MorningRelease_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        npc.transform.position = new Vector3(7f, -3f, 0f);
        homeAnchor.transform.position = new Vector3(-1.5f, 2.25f, 0f);
        npc.SetActive(false);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_MorningRelease_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.up);
        SetField(state, "IsNightResting", false);
        SetField(state, "HideWhileNightResting", false);

        InvokeStatic(crowdDirectorType, "RestoreResidentFromNightRestState", state);

        Assert.That(npc.activeSelf, Is.True, "次日释放 night-rest 时，resident 应先恢复显现。");
        Assert.That(npc.transform.position.x, Is.EqualTo(-1.5f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(2.25f).Within(0.001f),
            "次日恢复时必须先回到自己的 home anchor，不应从昨晚隐藏时的位置直接复活。");
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.False);
    }

    [Test]
    public void CrowdDirector_ShouldNotKeepNightRestResidentsUnderCrowdScriptedOwner()
    {
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        GameObject npc = Track(new GameObject("Resident_NightRest_NoCrowdOwner"));
        GameObject homeAnchor = Track(new GameObject("Resident_NightRest_NoCrowdOwner_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        npc.transform.position = new Vector3(-5f, 2f, 0f);
        homeAnchor.transform.position = new Vector3(1.5f, -1.25f, 0f);

        InvokeInstance(roamController, "AcquireResidentScriptedControl", "SpringDay1NpcCrowdDirector", false);
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.True);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_NightRest_NoCrowdOwner_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.up);
        SetField(state, "IsNightResting", true);
        SetField(state, "HideWhileNightResting", true);
        SetField(state, "IsReturningHome", true);

        InvokeStatic(crowdDirectorType, "ApplyResidentNightRestState", state);

        Assert.That(npc.transform.position.x, Is.EqualTo(1.5f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(-1.25f).Within(0.001f));
        Assert.That(npc.activeSelf, Is.False, "21:00 后的强制 night-rest 应在 snap 到 anchor 后统一隐藏。");
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.False,
            "进入 night-rest 后，CrowdDirector 不应继续深持有 resident 身体 owner。");
        StringAssert.Contains("night-rest-hidden", (string)GetFieldValue(state, "ResolvedAnchorName"));
    }

    [Test]
    public void CrowdDirector_ShouldHideResidentsAtTwentyOneEvenWhenHomeAnchorIsMissing()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        Type timeManagerType = ResolveTypeOrFail("TimeManager");

        Component timeManager = Track(new GameObject("TimeManager_NightRest_NoAnchor")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 21);
        SetField(timeManager, "currentMinute", 0);

        Component director = Track(new GameObject("CrowdDirector_NightRest_NoAnchor")).AddComponent(crowdDirectorType);
        GameObject npc = Track(new GameObject("Resident_NightRest_NoAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        npc.transform.position = new Vector3(3f, -2f, 0f);
        SceneManager.MoveGameObjectToScene(npc, townScene);

        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "BaseResolvedAnchorName", "missing-home-anchor");
        SetField(state, "BaseFacing", Vector2.left);

        IDictionary spawnStates = (IDictionary)GetFieldValue(director, "_spawnStates");
        spawnStates["resident-no-anchor"] = state;

        InvokeInstance(director, "SyncResidentNightRestSchedule");

        Assert.That((bool)GetFieldValue(state, "IsNightResting"), Is.True);
        Assert.That((bool)GetFieldValue(state, "HideWhileNightResting"), Is.True);
        Assert.That(npc.activeSelf, Is.False,
            "21:00 是全体兜底：就算没绑到 HomeAnchor，也不能继续留在场上。");
        StringAssert.Contains("night-rest-hidden", (string)GetFieldValue(state, "ResolvedAnchorName"));
    }

    [Test]
    public void CrowdDirector_ShouldNotDeferThirdResidentToStoryEscortDirectorDuringEnterVillage()
    {
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");

        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "003");

        bool shouldDefer = (bool)InvokeStatic(
            crowdDirectorType,
            "ShouldDeferToStoryEscortDirector",
            entry,
            ParseEnum(storyPhaseType, "EnterVillage"));

        Assert.That(shouldDefer, Is.False,
            "003 在 opening 里也应按 ordinary resident / crowd contract 处理，不能再被导演 story-escort 私链抓走。");
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
    public void CrowdDirector_ClockSchedule_ShouldReleaseNightRestAtSeven()
    {
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Component timeManager = Track(new GameObject("TimeManager_MorningRelease")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);

        Component director = Track(new GameObject("CrowdDirector_MorningRelease")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector"));

        SetField(timeManager, "currentHour", 6);
        SetField(timeManager, "currentMinute", 59);
        Assert.That((bool)InvokeInstance(director, "ShouldResidentsRestByClock"), Is.True, "7:00 前仍应保持夜间隐藏/休息合同。");

        SetField(timeManager, "currentHour", 7);
        SetField(timeManager, "currentMinute", 0);
        Assert.That((bool)InvokeInstance(director, "ShouldResidentsRestByClock"), Is.False, "7:00 起应结束 night-rest，允许从 anchor 恢复。");
    }

    [Test]
    public void CrowdDirector_ShouldHideResidentImmediatelyWhenAlreadyHomeAtTwenty()
    {
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Component timeManager = Track(new GameObject("TimeManager_AlreadyHomeAtTwenty")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 20);
        SetField(timeManager, "currentMinute", 0);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Component director = Track(new GameObject("CrowdDirector_AlreadyHomeAtTwenty")).AddComponent(crowdDirectorType);
        GameObject npc = Track(new GameObject("Resident_AlreadyHomeAtTwenty"));
        GameObject homeAnchor = Track(new GameObject("Resident_AlreadyHomeAtTwenty_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        npc.transform.position = new Vector3(2f, -1f, 0f);
        homeAnchor.transform.position = new Vector3(2.02f, -1.01f, 0f);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_AlreadyHomeAtTwenty_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.left);

        bool started = (bool)InvokeInstance(director, "TryBeginResidentReturnHome", state);

        Assert.That(started, Is.False, "已经在家附近时，不需要再重新发起一段 return-home drive。");
        Assert.That(npc.activeSelf, Is.False, "20:00 时已经到 anchor 附近的 resident 应直接隐藏，视作进屋睡觉。");
        Assert.That((bool)GetFieldValue(state, "HideWhileNightResting"), Is.True);
        StringAssert.Contains("night-return-hidden", (string)GetFieldValue(state, "ResolvedAnchorName"));
    }

    [Test]
    public void CrowdDirector_SnapResidentsToHomeAnchors_ShouldHideResidentsForNightRest()
    {
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");

        Component director = Track(new GameObject("CrowdDirector_SnapNightRest")).AddComponent(crowdDirectorType);
        GameObject npc = Track(new GameObject("Resident_SnapNightRest"));
        GameObject homeAnchor = Track(new GameObject("Resident_SnapNightRest_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        npc.transform.position = new Vector3(5f, -2f, 0f);
        homeAnchor.transform.position = new Vector3(-3f, 4f, 0f);

        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_SnapNightRest_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.left);

        IDictionary spawnStates = (IDictionary)GetFieldValue(director, "_spawnStates");
        spawnStates["resident-snap-night-rest"] = state;

        InvokeInstance(director, "SnapResidentsToHomeAnchorsInternal");

        Assert.That((bool)GetFieldValue(state, "IsNightResting"), Is.True);
        Assert.That((bool)GetFieldValue(state, "HideWhileNightResting"), Is.True);
        Assert.That(npc.activeSelf, Is.False, "forced day-end snap 也必须体现成“进屋睡觉”，不能只 snap 不 hide。");
        Assert.That(npc.transform.position.x, Is.EqualTo(-3f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(4f).Within(0.001f));
        StringAssert.Contains("night-rest-hidden", (string)GetFieldValue(state, "ResolvedAnchorName"));
    }

    [Test]
    public void CrowdDirector_ShouldHideResidentImmediatelyWhenNightReturnCompletesAtTwenty()
    {
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Component timeManager = Track(new GameObject("TimeManager_ReturnFinishAtTwenty")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 20);
        SetField(timeManager, "currentMinute", 0);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Component director = Track(new GameObject("CrowdDirector_ReturnFinishAtTwenty")).AddComponent(crowdDirectorType);
        GameObject npc = Track(new GameObject("Resident_ReturnFinishAtTwenty"));
        GameObject homeAnchor = Track(new GameObject("Resident_ReturnFinishAtTwenty_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        npc.transform.position = new Vector3(-4f, 3f, 0f);
        homeAnchor.transform.position = new Vector3(1.25f, -2f, 0f);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_ReturnFinishAtTwenty_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.up);
        SetField(state, "IsReturningHome", true);

        InvokeInstance(roamController, "AcquireStoryControl", "SpringDay1NpcCrowdDirector", false);
        InvokeInstance(director, "FinishResidentReturnHome", state);

        Assert.That(npc.activeSelf, Is.False, "20:00 的回家导航一旦完成，就应直接隐藏而不是站在门口。");
        Assert.That(npc.transform.position.x, Is.EqualTo(1.25f).Within(0.001f));
        Assert.That(npc.transform.position.y, Is.EqualTo(-2f).Within(0.001f));
        Assert.That((bool)GetFieldValue(state, "HideWhileNightResting"), Is.True);
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.False);
        StringAssert.Contains("night-return-hidden", (string)GetFieldValue(state, "ResolvedAnchorName"));
    }

    [Test]
    public void CrowdDirector_TryBeginResidentReturnHome_ShouldHideImmediatelyWhenWithinArrivalRadiusDuringRetryCooldown()
    {
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Component timeManager = Track(new GameObject("TimeManager_ReturnRetryAtHome")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 20);
        SetField(timeManager, "currentMinute", 0);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Component director = Track(new GameObject("CrowdDirector_ReturnRetryAtHome")).AddComponent(crowdDirectorType);
        GameObject npc = Track(new GameObject("Resident_ReturnRetryAtHome"));
        GameObject homeAnchor = Track(new GameObject("Resident_ReturnRetryAtHome_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        npc.transform.position = new Vector3(5.28f, -2.0f, 0f);
        homeAnchor.transform.position = new Vector3(5.0f, -2.0f, 0f);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_ReturnRetryAtHome_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.left);
        SetField(state, "NextReturnHomeDriveRetryAt", Time.unscaledTime + 10f);

        bool started = (bool)InvokeInstance(director, "TryBeginResidentReturnHome", state);

        Assert.That(started, Is.False, "已经进入 home arrival 半径时，不应再被 retry cooldown 拦住。");
        Assert.That(npc.activeSelf, Is.False, "20:00 时只要已经到家附近，就应立即隐藏进屋。");
        Assert.That((bool)GetFieldValue(state, "HideWhileNightResting"), Is.True);
        StringAssert.Contains("night-return-hidden", (string)GetFieldValue(state, "ResolvedAnchorName"));
    }

    [Test]
    public void CrowdDirector_TickResidentReturnHome_ShouldHideWhenResidentIsAtAnchorWithoutActiveNavigation()
    {
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Component timeManager = Track(new GameObject("TimeManager_ReturnTickAtHome")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 20);
        SetField(timeManager, "currentMinute", 0);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Component director = Track(new GameObject("CrowdDirector_ReturnTickAtHome")).AddComponent(crowdDirectorType);
        GameObject npc = Track(new GameObject("Resident_ReturnTickAtHome"));
        GameObject homeAnchor = Track(new GameObject("Resident_ReturnTickAtHome_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        npc.transform.position = new Vector3(-1.22f, 4.0f, 0f);
        homeAnchor.transform.position = new Vector3(-1.0f, 4.0f, 0f);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_ReturnTickAtHome_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.up);
        SetField(state, "IsReturningHome", true);

        InvokeInstance(director, "TickResidentReturnHome", state, 0.016f);

        Assert.That(npc.activeSelf, Is.False, "即使没有 active FormalNavigation，只要 resident 已经贴近 anchor，也应立即隐藏进屋。");
        Assert.That((bool)GetFieldValue(state, "HideWhileNightResting"), Is.True);
        StringAssert.Contains("night-return-hidden", (string)GetFieldValue(state, "ResolvedAnchorName"));
    }

    [Test]
    public void CrowdDirector_TickResidentReturnHome_ShouldHideWhenResidentIsWithinFormalArrivalTolerance()
    {
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Component timeManager = Track(new GameObject("TimeManager_ReturnTickFormalTolerance")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 20);
        SetField(timeManager, "currentMinute", 0);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Component director = Track(new GameObject("CrowdDirector_ReturnTickFormalTolerance")).AddComponent(crowdDirectorType);
        GameObject npc = Track(new GameObject("Resident_ReturnTickFormalTolerance"));
        GameObject homeAnchor = Track(new GameObject("Resident_ReturnTickFormalTolerance_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        npc.transform.position = new Vector3(-0.42f, 6.0f, 0f);
        homeAnchor.transform.position = new Vector3(0f, 6.0f, 0f);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_ReturnTickFormalTolerance_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.up);
        SetField(state, "IsReturningHome", true);

        InvokeInstance(director, "TickResidentReturnHome", state, 0.016f);

        Assert.That(npc.activeSelf, Is.False,
            "FormalNavigation 会在 body-shell arrival 半径内收尾，CrowdDirector 也必须在同一量级内完成到家隐藏，不能把 NPC 留在家门口罚站。");
        Assert.That((bool)GetFieldValue(state, "HideWhileNightResting"), Is.True);
        StringAssert.Contains("night-return-hidden", (string)GetFieldValue(state, "ResolvedAnchorName"));
    }

    [Test]
    public void CrowdDirector_TickResidentReturnHome_ShouldHideWhenCachedFormalComparableDestinationIsReached()
    {
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Component timeManager = Track(new GameObject("TimeManager_ReturnTickCachedComparable")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 20);
        SetField(timeManager, "currentMinute", 0);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Component director = Track(new GameObject("CrowdDirector_ReturnTickCachedComparable")).AddComponent(crowdDirectorType);
        GameObject npc = Track(new GameObject("Resident_ReturnTickCachedComparable"));
        GameObject homeAnchor = Track(new GameObject("Resident_ReturnTickCachedComparable_HomeAnchor"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        npc.transform.position = new Vector3(0.36f, 6.0f, 0f);
        homeAnchor.transform.position = new Vector3(1.28f, 6.0f, 0f);
        SetField(roamController, "hasCachedFormalNavigationComparableDestination", true);
        SetField(roamController, "cachedFormalNavigationRequestedDestination", (Vector2)homeAnchor.transform.position);
        SetField(roamController, "cachedFormalNavigationComparableDestination", new Vector2(0f, 6.0f));
        SetField(roamController, "lastFormalNavigationArrivalTime", Time.time);

        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_ReturnTickCachedComparable_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.up);
        SetField(state, "IsReturningHome", true);

        InvokeInstance(director, "TickResidentReturnHome", state, 0.016f);

        Assert.That(npc.activeSelf, Is.False,
            "即使 authored homeAnchor 更深、实际 FormalNavigation 只走到可达代理点，CrowdDirector 也应按这次真实导航目标完成到家隐藏。");
        Assert.That((bool)GetFieldValue(state, "HideWhileNightResting"), Is.True);
        StringAssert.Contains("night-return-hidden", (string)GetFieldValue(state, "ResolvedAnchorName"));
    }

    [Test]
    public void CrowdDirector_ShouldNotTreatTwentyOneAsReturnHomeContractWindow()
    {
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Component timeManager = Track(new GameObject("TimeManager_ReturnHomeWindow")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 21);
        SetField(timeManager, "currentMinute", 0);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Component director = Track(new GameObject("CrowdDirector_ReturnHomeWindow")).AddComponent(crowdDirectorType);

        bool shouldAllow = (bool)InvokeInstance(
            director,
            "ShouldAllowResidentReturnHome",
            ParseEnum(storyPhaseType, "FreeTime"));

        Assert.That(shouldAllow, Is.False, "21:00 之后应只剩 night-rest 语义，不应继续把 rest 当成 return-home contract 延长期。");
    }

    [Test]
    public void CrowdDirector_RuntimeEntries_ShouldIncludeThirdResidentAfterOpening()
    {
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type manifestType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest");
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component director = Track(new GameObject("CrowdDirector_ThirdResidentRuntimeEntries")).AddComponent(crowdDirectorType);
        ScriptableObject manifest = Track(ScriptableObject.CreateInstance(manifestType) as ScriptableObject);
        SetField(manifest, "entries", Array.CreateInstance(entryType, 0));

        Array openingEntries = (Array)InvokeInstance(
            director,
            "GetRuntimeEntries",
            manifest,
            ParseEnum(storyPhaseType, "EnterVillage"));
        Array healingEntries = (Array)InvokeInstance(
            director,
            "GetRuntimeEntries",
            manifest,
            ParseEnum(storyPhaseType, "HealingAndHP"));

        Assert.That(openingEntries.Length, Is.EqualTo(1),
            "003 现在在 opening 的 EnterVillage 阶段就应并入 crowd resident runtime，而不是继续悬空在导演私链外。");
        Assert.That(healingEntries.Length, Is.EqualTo(1),
            "opening 收口后，003 应并回 resident runtime，而不是继续悬空在导演私有合同外。");
        Assert.That((string)GetFieldValue(openingEntries.GetValue(0), "npcId"), Is.EqualTo("003"));
        Assert.That((string)GetFieldValue(healingEntries.GetValue(0), "npcId"), Is.EqualTo("003"));
    }

    [Test]
    public void CrowdDirector_RuntimeEntries_ShouldIncludeStoryActorsDuringFreeTimeNightContract()
    {
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type manifestType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest");
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component director = Track(new GameObject("CrowdDirector_StoryActorsNightRuntimeEntries")).AddComponent(crowdDirectorType);
        ScriptableObject manifest = Track(ScriptableObject.CreateInstance(manifestType) as ScriptableObject);
        SetField(manifest, "entries", Array.CreateInstance(entryType, 0));

        Array freeTimeEntries = (Array)InvokeInstance(
            director,
            "GetRuntimeEntries",
            manifest,
            ParseEnum(storyPhaseType, "FreeTime"));

        string[] npcIds = new string[freeTimeEntries.Length];
        for (int index = 0; index < freeTimeEntries.Length; index++)
        {
            npcIds[index] = (string)GetFieldValue(freeTimeEntries.GetValue(index), "npcId");
        }

        CollectionAssert.AreEquivalent(new[] { "001", "002", "003" }, npcIds,
            "19:30 之后进入统一夜间合同前，001/002/003 都应纳回 resident runtime，不能继续 split 成导演私链 + crowd 私链。");
    }

    [Test]
    public void CrowdDirector_RuntimeEntries_ShouldIncludeStoryActorsDuringDinnerAndReminderContract()
    {
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type manifestType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest");
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component director = Track(new GameObject("CrowdDirector_StoryActorsDinnerRuntimeEntries")).AddComponent(crowdDirectorType);
        ScriptableObject manifest = Track(ScriptableObject.CreateInstance(manifestType) as ScriptableObject);
        SetField(manifest, "entries", Array.CreateInstance(entryType, 0));

        Array dinnerEntries = (Array)InvokeInstance(
            director,
            "GetRuntimeEntries",
            manifest,
            ParseEnum(storyPhaseType, "DinnerConflict"));
        Array reminderEntries = (Array)InvokeInstance(
            director,
            "GetRuntimeEntries",
            manifest,
            ParseEnum(storyPhaseType, "ReturnAndReminder"));

        CollectionAssert.AreEquivalent(new[] { "001", "002", "003" }, ExtractNpcIds(dinnerEntries),
            "晚饭阶段也必须让 001/002/003 进入 crowd runtime；不能再让 001/002 走 director 私链、003/101~203 走 crowd 链。");
        CollectionAssert.AreEquivalent(new[] { "001", "002", "003" }, ExtractNpcIds(reminderEntries),
            "19:00 reminder 仍属于 dinner block，也必须沿用同一份 crowd runtime 合同。");
    }

    [Test]
    public void Director_ShouldNotUseTownStoryActorModeForDinnerAndReminder()
    {
        Type directorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Component director = Track(new GameObject("SpringDay1Director_DinnerNoStoryActorMode")).AddComponent(directorType);

        Assert.That((bool)InvokeInstance(director, "ShouldUseTownStoryActorMode", ParseEnum(storyPhaseType, "DinnerConflict")), Is.False,
            "DinnerConflict 的 001/002 不应再由 Director story actor policy 私有持有，应交给 crowd/stage-book 合同。");
        Assert.That((bool)InvokeInstance(director, "ShouldUseTownStoryActorMode", ParseEnum(storyPhaseType, "ReturnAndReminder")), Is.False,
            "ReturnAndReminder 仍属于晚饭 block，也不应重新回到 Director story actor 私链。");
    }

    [Test]
    public void CrowdDirector_ShouldBindUnifiedNightRuntimeStoryActorsToOwnHomeAnchors()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type manifestType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest");
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type motionControllerType = ResolveTypeOrFail("NPCMotionController");
        Type roamControllerType = ResolveTypeOrFail("NPCAutoRoamController");

        Component storyManager = Track(new GameObject("StoryManager_UnifiedNightHomeAnchors")).AddComponent(storyManagerType);
        Component crowdDirector = Track(new GameObject("CrowdDirector_UnifiedNightHomeAnchors")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FreeTime"), false);

        ScriptableObject manifest = Track(ScriptableObject.CreateInstance(manifestType) as ScriptableObject);
        SetField(manifest, "entries", Array.CreateInstance(entryType, 0));
        SetField(crowdDirector, "_manifest", manifest);

        CreateResidentWithHomeAnchor("001", new Vector3(-2f, 4f, 0f), new Vector3(-12f, 2f, 0f), motionControllerType, roamControllerType, townScene);
        CreateResidentWithHomeAnchor("002", new Vector3(-1f, 4f, 0f), new Vector3(-11f, 2f, 0f), motionControllerType, roamControllerType, townScene);
        CreateResidentWithHomeAnchor("003", new Vector3(0f, 4f, 0f), new Vector3(-10f, 2f, 0f), motionControllerType, roamControllerType, townScene);

        InvokeInstance(crowdDirector, "SyncCrowd");

        IDictionary spawnStates = (IDictionary)GetFieldValue(crowdDirector, "_spawnStates");
        Assert.That(spawnStates.Contains("001"), Is.True);
        Assert.That(spawnStates.Contains("002"), Is.True);
        Assert.That(spawnStates.Contains("003"), Is.True);
        Assert.That(((GameObject)GetFieldValue(spawnStates["001"], "HomeAnchor")).name, Is.EqualTo("001_HomeAnchor"),
            "统一夜间合同里，001 必须绑到自己的 HomeAnchor；不能再退回 NPC 本体或错误 alias。");
        Assert.That(((GameObject)GetFieldValue(spawnStates["002"], "HomeAnchor")).name, Is.EqualTo("002_HomeAnchor"),
            "统一夜间合同里，002 必须绑到自己的 HomeAnchor；不能再退回 NPC 本体或错误 alias。");
        Assert.That(((GameObject)GetFieldValue(spawnStates["003"], "HomeAnchor")).name, Is.EqualTo("003_HomeAnchor"),
            "统一夜间合同里，003 也必须和普通 resident 一样绑到自己的 HomeAnchor。");
    }

    [Test]
    public void CrowdDirector_FindSceneResidentHomeAnchor_ShouldRejectResidentBodiesAsFallback()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");

        Component crowdDirector = Track(new GameObject("CrowdDirector_HomeAnchorFallbackSafety")).AddComponent(crowdDirectorType);
        GameObject resident101 = Track(new GameObject("101"));
        SceneManager.MoveGameObjectToScene(resident101, townScene);

        GameObject resident001 = Track(new GameObject("001"));
        SceneManager.MoveGameObjectToScene(resident001, townScene);

        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "101");
        SetField(entry, "anchorObjectName", "001|NPC001");

        Transform resolved = InvokeInstance(
            crowdDirector,
            "FindSceneResidentHomeAnchor",
            entry,
            townScene,
            resident101.transform) as Transform;

        Assert.That(resolved, Is.Null,
            "夜间 home-anchor 解析不应把别的 resident 本体（如 001）错当成 101 的家。");
    }

    [Test]
    public void Director_ShouldNotForceTownStoryActorsVisibleDuringResidentNightContract()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type directorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type timeManagerType = ResolveTypeOrFail("TimeManager");

        Component storyManager = Track(new GameObject("StoryManager_TownNightVisibility")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_TownNightVisibility")).AddComponent(directorType);
        Component timeManager = Track(new GameObject("TimeManager_TownNightVisibility")).AddComponent(timeManagerType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(directorType, "_instance", director);
        SetStaticField(timeManagerType, "instance", timeManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FreeTime"), false);
        SetField(timeManager, "currentHour", 21);
        SetField(timeManager, "currentMinute", 0);

        GameObject chief = Track(new GameObject("001"));
        chief.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        chief.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        chief.SetActive(false);
        SetField(director, "_cachedChiefActor", chief.transform);

        GameObject companion = Track(new GameObject("002"));
        companion.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        companion.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        companion.SetActive(false);
        SetField(director, "_cachedCompanionActor", companion.transform);

        GameObject thirdResident = Track(new GameObject("003"));
        thirdResident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        thirdResident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        thirdResident.SetActive(false);
        SetField(director, "_cachedThirdResidentActor", thirdResident.transform);

        InvokeInstance(director, "UpdateSceneStoryNpcVisibility");

        Assert.That(chief.activeSelf, Is.False, "21:00 之后夜间显隐 owner 应让给 resident contract，Director 不应再把 001 强行拉出来。");
        Assert.That(companion.activeSelf, Is.False, "21:00 之后夜间显隐 owner 应让给 resident contract，Director 不应再把 002 强行拉出来。");
        Assert.That(thirdResident.activeSelf, Is.False, "003 进入 unified resident runtime 后，Director 在夜间窗口也不应再强拉可见。");
    }

    [Test]
    public void CrowdDirector_ShouldBindThirdResidentIntoResidentRuntimeDuringOpening()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_ThirdResidentRuntimeBind")).AddComponent(storyManagerType);
        Component crowdDirector = Track(new GameObject("CrowdDirector_ThirdResidentRuntimeBind")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "EnterVillage"), false);

        GameObject thirdResident = Track(new GameObject("003"));
        thirdResident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component roamController = thirdResident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        thirdResident.transform.position = new Vector3(-6.5f, 2.25f, 0f);
        SceneManager.MoveGameObjectToScene(thirdResident, townScene);

        GameObject homeAnchor = Track(new GameObject("003_HomeAnchor"));
        homeAnchor.transform.position = new Vector3(-8.5f, 1.25f, 0f);
        SceneManager.MoveGameObjectToScene(homeAnchor, townScene);

        InvokeInstance(crowdDirector, "SyncCrowd");

        IDictionary spawnStates = (IDictionary)GetFieldValue(crowdDirector, "_spawnStates");
        Assert.That(spawnStates.Contains("003"), Is.True,
            "003 在 opening 的 EnterVillage 阶段就应被 crowd resident runtime 正式接进来，而不是继续游离在导演私链外。");
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.False,
            "003 并入 opening crowd runtime 后，不应继续残留导演 scripted owner。");
    }

    [Test]
    public void CrowdDirector_ShouldResolveDinnerCueFromDinnerBeatInsteadOfOpeningMirror()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");

        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "203");

        MethodInfo resolveMethod = crowdDirectorType.GetMethod(
            "TryResolveStagingCueForCurrentScene",
            BindingFlags.NonPublic | BindingFlags.Static);

        object[] openingArgs = { "EnterVillage_PostEntry", entry, null };
        object[] dinnerArgs = { "DinnerConflict_Table", entry, null };
        bool openingResolved = (bool)resolveMethod.Invoke(null, openingArgs);
        bool dinnerResolved = (bool)resolveMethod.Invoke(null, dinnerArgs);

        Assert.That(openingResolved, Is.True);
        Assert.That(dinnerResolved, Is.True);
        Assert.That(GetPropertyValue(dinnerArgs[2], "StableKey"), Is.Not.EqualTo(GetPropertyValue(openingArgs[2], "StableKey")),
            "晚饭 beat 应走自己的 staged cue，不能再偷用 opening 的 crowd cue。");
    }

    [TestCase("HealingAndHP")]
    [TestCase("WorkbenchFlashback")]
    [TestCase("FarmingTutorial")]
    public void Director_ShouldKeepStoryTimeRunningDuringPrimaryPhases(string phaseName)
    {
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Component director = Track(new GameObject($"SpringDay1Director_Time_{phaseName}")).AddComponent(springDirectorType);

        bool shouldPause = (bool)InvokeInstance(
            director,
            "ShouldPauseStoryTimeForCurrentPhase",
            ParseEnum(storyPhaseType, phaseName));

        Assert.That(shouldPause, Is.False, $"Primary 内的 {phaseName} 不应冻结 Day1 时钟；正确语义是正常流逝，到 16:00 再由 guardrail 卡住。");
    }

    [Test]
    public void Director_ShouldKeepStoryTimeRunningDuringPrimaryArrivalBridge()
    {
        Scene primaryScene = CreateNamedTestScene("Primary");
        SceneManager.SetActiveScene(primaryScene);

        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_PrimaryArrivalTime")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_PrimaryArrivalTime")).AddComponent(springDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "EnterVillage"), false);

        bool shouldPause = (bool)InvokeInstance(
            director,
            "ShouldPauseStoryTimeForCurrentPhase",
            ParseEnum(storyPhaseType, "EnterVillage"));

        Assert.That(shouldPause, Is.False,
            "进入 Primary 的承接段仍属于白天推进，不应因为 phase 还挂着 EnterVillage 就把 Day1 时钟停住。");
    }

    [Test]
    public void Director_ShouldKeepPrimaryTownReturnGateClosedBeforePostTutorialExploreWindow()
    {
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_PrimaryTownGateClosed")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_PrimaryTownGateClosed")).AddComponent(springDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FarmingTutorial"), false);
        SetField(director, "_postTutorialExploreWindowEntered", false);

        bool allowReturn = (bool)InvokeInstance(director, "ShouldAllowPrimaryReturnToTown");

        Assert.That(allowReturn, Is.False,
            "Primary 任务尚未进入 post-tutorial explore window 前，不应提前放开回 Town。");
    }

    [Test]
    public void Director_ShouldAllowPrimaryTownReturnGateDuringPostTutorialExploreWindow()
    {
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_PrimaryTownGateOpen")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_PrimaryTownGateOpen")).AddComponent(springDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FarmingTutorial"), false);
        SetField(director, "_postTutorialExploreWindowEntered", true);

        bool allowReturn = (bool)InvokeInstance(director, "ShouldAllowPrimaryReturnToTown");

        Assert.That(allowReturn, Is.True,
            "进入 post-tutorial explore window 后，Primary -> Town gate 才应正式放开。");
    }

    [Test]
    public void Director_ShouldKeepPrimaryHomeEntryGateClosedBeforeHealingCompletes()
    {
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type sceneTransitionTriggerType = ResolveTypeOrFail("Sunset.Story.SceneTransitionTrigger2D");

        Scene primaryScene = CreateNamedTestScene("Primary");
        SceneManager.SetActiveScene(primaryScene);

        Component storyManager = Track(new GameObject("StoryManager_PrimaryHomeGateClosed")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_PrimaryHomeGateClosed")).AddComponent(springDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "HealingAndHP"), false);
        SetField(director, "_healingSequencePlayed", false);

        GameObject homeDoor = Track(new GameObject("PrimaryHomeDoor", typeof(BoxCollider2D)));
        Component trigger = Track(homeDoor.AddComponent(sceneTransitionTriggerType));
        InvokeInstance(trigger, "SetTargetScene", "Home", string.Empty, string.Empty);

        InvokeInstance(director, "SyncPrimaryHomeEntryGate");

        Assert.That(((Behaviour)trigger).enabled, Is.False, "疗伤对白完成前，PrimaryHomeDoor 的转场触发器应保持关闭。");
        Assert.That(homeDoor.GetComponent<BoxCollider2D>().enabled, Is.False, "疗伤对白完成前，PrimaryHomeDoor 的 Collider2D 也应一起关闭。");
    }

    [Test]
    public void Director_ShouldAllowPrimaryHomeEntryGateAfterHealingCompletes()
    {
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type sceneTransitionTriggerType = ResolveTypeOrFail("Sunset.Story.SceneTransitionTrigger2D");

        Scene primaryScene = CreateNamedTestScene("Primary");
        SceneManager.SetActiveScene(primaryScene);

        Component storyManager = Track(new GameObject("StoryManager_PrimaryHomeGateOpen")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_PrimaryHomeGateOpen")).AddComponent(springDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "WorkbenchFlashback"), false);
        SetField(director, "_healingSequencePlayed", true);

        GameObject homeDoor = Track(new GameObject("PrimaryHomeDoor", typeof(BoxCollider2D)));
        Component trigger = Track(homeDoor.AddComponent(sceneTransitionTriggerType));
        InvokeInstance(trigger, "SetTargetScene", "Home", string.Empty, string.Empty);

        InvokeInstance(director, "SyncPrimaryHomeEntryGate");

        Assert.That(((Behaviour)trigger).enabled, Is.True, "疗伤完成后，PrimaryHomeDoor 的转场触发器应重新放开。");
        Assert.That(homeDoor.GetComponent<BoxCollider2D>().enabled, Is.True, "疗伤完成后，PrimaryHomeDoor 的 Collider2D 应重新放开。");
    }

    [Test]
    public void Director_ShouldKeepPrimaryHomeEntryGateClosedUntilPhaseReallyEntersWorkbenchFlashback()
    {
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type sceneTransitionTriggerType = ResolveTypeOrFail("Sunset.Story.SceneTransitionTrigger2D");

        Scene primaryScene = CreateNamedTestScene("Primary");
        SceneManager.SetActiveScene(primaryScene);

        Component storyManager = Track(new GameObject("StoryManager_PrimaryHomeGateStillClosed")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_PrimaryHomeGateStillClosed")).AddComponent(springDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "HealingAndHP"), false);
        SetField(director, "_healingSequencePlayed", true);
        AddCompletedSequenceId(director, "day1_healing_bridge");

        GameObject homeDoor = Track(new GameObject("PrimaryHomeDoor", typeof(BoxCollider2D)));
        Component trigger = Track(homeDoor.AddComponent(sceneTransitionTriggerType));
        InvokeInstance(trigger, "SetTargetScene", "Home", string.Empty, string.Empty);

        InvokeInstance(director, "SyncPrimaryHomeEntryGate");

        Assert.That(((Behaviour)trigger).enabled, Is.False, "就算疗伤对白刚播完，只要 phase 还停在 HealingAndHP，就不应提前放开进屋门。");
        Assert.That(homeDoor.GetComponent<BoxCollider2D>().enabled, Is.False, "0.0.4 真正开始前，PrimaryHomeDoor 的 Collider2D 也必须继续保持关闭。");
    }

    [Test]
    public void Director_ShouldNotForceWorkbenchBriefingFromPlayerProximityBeforeLeaderArrives()
    {
        Component director = Track(new GameObject("SpringDay1Director_WorkbenchForceGate")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1Director"));

        bool canForceBeforeLeaderArrives = (bool)InvokeInstance(
            director,
            "CanForceWorkbenchBriefing",
            false,
            true);

        bool canForceAfterLeaderArrives = (bool)InvokeInstance(
            director,
            "CanForceWorkbenchBriefing",
            true,
            true);

        Assert.That(canForceBeforeLeaderArrives, Is.False,
            "玩家就算已经贴近工作台，只要 001 还没走到位，0.0.4 也不应被提前强开。");
        Assert.That(canForceAfterLeaderArrives, Is.True,
            "只有 leader 真正到位后，玩家靠近工作台这条宽松条件才允许补齐 002 并推进开场。");
    }

    [Test]
    public void Director_ShouldEnforceHealingSupportApproachRuntimeMinimum()
    {
        Component director = Track(new GameObject("SpringDay1Director_HealingRadiusFloor")).AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1Director"));
        SetField(director, "healingSupportApproachDistance", 0.9f);

        float effectiveDistance = (float)InvokeInstance(director, "GetEffectiveHealingSupportApproachDistance");

        Assert.That(effectiveDistance, Is.EqualTo(2.4f).Within(0.001f),
            "疗伤触发半径必须有运行时下限，不能再被旧 Inspector 小值偷偷盖回近乎贴脸的体验。");
    }

    [Test]
    public void Director_ShouldRecoverPreHealingHomeIntrusionWhenHomeSceneIsActive()
    {
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Scene homeScene = CreateNamedTestScene("Home");
        SceneManager.SetActiveScene(homeScene);

        Component storyManager = Track(new GameObject("StoryManager_HomeRecovery")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_HomeRecovery")).AddComponent(springDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "HealingAndHP"), false);
        SetField(director, "_healingSequencePlayed", false);

        bool shouldRecover = (bool)InvokeInstance(director, "ShouldRecoverPreHealingHomeIntrusion");

        Assert.That(shouldRecover, Is.True, "疗伤完成前如果坏档已经落在 Home，导演应判定需要立刻退回 Primary。");
    }

    [Test]
    public void Director_ShouldKeepConversationBubbleVisibleDuringStoryActorMode()
    {
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type bubblePresenterType = ResolveTypeOrFail("NPCBubblePresenter");

        Scene primaryScene = CreateNamedTestScene("Primary");
        SceneManager.SetActiveScene(primaryScene);

        Component director = Track(new GameObject("SpringDay1Director_BubblePolicy")).AddComponent(springDirectorType);
        GameObject actor = Track(new GameObject("PrimaryBubbleActor"));
        Component bubblePresenter = Track(actor.AddComponent(bubblePresenterType));

        GameObject canvasObject = Track(new GameObject("NPCBubbleCanvas", typeof(Canvas)));
        canvasObject.transform.SetParent(actor.transform, false);
        canvasObject.SetActive(true);
        SetField(bubblePresenter, "_canvas", canvasObject.GetComponent<Canvas>());
        FieldInfo channelPriorityField = bubblePresenterType.GetField("_channelPriority", BindingFlags.Instance | BindingFlags.NonPublic);
        channelPriorityField.SetValue(bubblePresenter, Enum.ToObject(channelPriorityField.FieldType, 10));

        Assert.That((bool)bubblePresenterType.GetProperty("IsConversationPriorityVisible").GetValue(bubblePresenter), Is.True,
            "测试前提应先成立：这个 bubble 现在处于 conversation priority 可见态。");

        InvokeInstance(
            director,
            "ApplyStoryActorRuntimePolicy",
            actor.transform,
            true,
            false,
            true);

        Assert.That(canvasObject.activeSelf, Is.True, "storyActorMode 下，conversation priority bubble 不应被导演自己每帧关掉。");
        Assert.That((bool)bubblePresenterType.GetProperty("IsBubbleVisible").GetValue(bubblePresenter), Is.True,
            "storyActorMode 下，001 的提示气泡应保持可见。");
    }

    [Test]
    public void Director_ShouldReleaseStoryActorsBackToOrdinaryNpcRulesDuringPostTutorialExploreWindowInTown()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type dialogueType = ResolveTypeOrFail("Sunset.Story.NPCDialogueInteractable");
        Type informalType = ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable");

        Component storyManager = Track(new GameObject("StoryManager_TownExploreStoryActorRelease")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_TownExploreStoryActorRelease")).AddComponent(springDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FarmingTutorial"), false);
        SetField(director, "_postTutorialExploreWindowEntered", true);

        GameObject chief = Track(new GameObject("001"));
        chief.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component chiefRoam = chief.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        Behaviour chiefDialogue = (Behaviour)chief.AddComponent(dialogueType);
        Behaviour chiefInformal = (Behaviour)chief.AddComponent(informalType);
        SceneManager.MoveGameObjectToScene(chief, townScene);

        GameObject companion = Track(new GameObject("002"));
        companion.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component companionRoam = companion.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        Behaviour companionDialogue = (Behaviour)companion.AddComponent(dialogueType);
        Behaviour companionInformal = (Behaviour)companion.AddComponent(informalType);
        SceneManager.MoveGameObjectToScene(companion, townScene);

        InvokeInstance(chiefRoam, "AcquireStoryControl", "spring-day1-director", true);
        InvokeInstance(companionRoam, "AcquireStoryControl", "spring-day1-director", true);
        chiefDialogue.enabled = false;
        chiefInformal.enabled = false;
        companionDialogue.enabled = false;
        companionInformal.enabled = false;

        InvokeInstance(director, "UpdateSceneStoryNpcVisibility");

        Assert.That(chiefDialogue.enabled, Is.True);
        Assert.That(chiefInformal.enabled, Is.True);
        Assert.That(companionDialogue.enabled, Is.True);
        Assert.That(companionInformal.enabled, Is.True);
        Assert.That((bool)GetPropertyValue(chiefRoam, "IsResidentScriptedControlActive"), Is.False);
        Assert.That((bool)GetPropertyValue(companionRoam, "IsResidentScriptedControlActive"), Is.False,
            "0.0.6 回 Town 的白天自由窗口里，001/002 也必须回到普通 NPC 规则，不能继续被 story actor runtime 扣着。");
    }

    [Test]
    public void Director_ShouldSnapStoryActorsToHomeAnchorsDuringPostTutorialExploreWindowInTown()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_TownExploreAnchorPrime")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_TownExploreAnchorPrime")).AddComponent(springDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FarmingTutorial"), false);
        SetField(director, "_postTutorialExploreWindowEntered", true);

        GameObject chiefHomeAnchor = Track(new GameObject("001_HomeAnchor"));
        chiefHomeAnchor.transform.position = new Vector3(-12f, 2f, 0f);
        SceneManager.MoveGameObjectToScene(chiefHomeAnchor, townScene);

        GameObject companionHomeAnchor = Track(new GameObject("002_HomeAnchor"));
        companionHomeAnchor.transform.position = new Vector3(-11f, 2f, 0f);
        SceneManager.MoveGameObjectToScene(companionHomeAnchor, townScene);

        GameObject chief = Track(new GameObject("001"));
        chief.transform.position = new Vector3(6f, 1.5f, 0f);
        chief.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        chief.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        SceneManager.MoveGameObjectToScene(chief, townScene);

        GameObject companion = Track(new GameObject("002"));
        companion.transform.position = new Vector3(7f, 1.5f, 0f);
        companion.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        companion.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        SceneManager.MoveGameObjectToScene(companion, townScene);

        InvokeInstance(director, "UpdateSceneStoryNpcVisibility");

        Assert.That(chief.transform.position, Is.EqualTo(chiefHomeAnchor.transform.position),
            "进入 0.0.6 后，001 回 Town 的第一拍应先落到自己的 Town anchor，不应继续停在旧 scene 原位。");
        Assert.That(companion.transform.position, Is.EqualTo(companionHomeAnchor.transform.position),
            "进入 0.0.6 后，002 回 Town 的第一拍也应先落到自己的 Town anchor，不应继续停在旧 scene 原位。");
        Assert.That((bool)GetFieldValue(director, "_postTutorialTownActorsAnchored"), Is.True,
            "Town explore window 的 anchor-first handoff 应是一拍完成的一次性对位，不应每帧反复回抓。");
    }

    [Test]
    public void Director_ShouldRestoreStoryActorDialogueRightsWhenLateDayEntersFreeTime()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type dialogueType = ResolveTypeOrFail("Sunset.Story.NPCDialogueInteractable");
        Type informalType = ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable");

        Component storyManager = Track(new GameObject("StoryManager_LateDayFreeTimeDialogueRelease")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_LateDayFreeTimeDialogueRelease")).AddComponent(springDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "DinnerConflict"), false);

        GameObject chief = Track(new GameObject("001"));
        chief.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component chiefRoam = chief.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        Behaviour chiefDialogue = (Behaviour)chief.AddComponent(dialogueType);
        Behaviour chiefInformal = (Behaviour)chief.AddComponent(informalType);
        SceneManager.MoveGameObjectToScene(chief, townScene);

        GameObject companion = Track(new GameObject("002"));
        companion.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component companionRoam = companion.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        Behaviour companionDialogue = (Behaviour)companion.AddComponent(dialogueType);
        Behaviour companionInformal = (Behaviour)companion.AddComponent(informalType);
        SceneManager.MoveGameObjectToScene(companion, townScene);

        InvokeInstance(director, "UpdateSceneStoryNpcVisibility");

        Assert.That(chiefDialogue.enabled, Is.True);
        Assert.That(chiefInformal.enabled, Is.True);
        Assert.That(companionDialogue.enabled, Is.True);
        Assert.That(companionInformal.enabled, Is.True);
        Assert.That((bool)GetPropertyValue(chiefRoam, "IsResidentScriptedControlActive"), Is.True);
        Assert.That((bool)GetPropertyValue(companionRoam, "IsResidentScriptedControlActive"), Is.True);

        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FreeTime"), false);
        InvokeInstance(director, "UpdateSceneStoryNpcVisibility");

        Assert.That(chiefDialogue.enabled, Is.True);
        Assert.That(chiefInformal.enabled, Is.True);
        Assert.That(companionDialogue.enabled, Is.True);
        Assert.That(companionInformal.enabled, Is.True);
        Assert.That((bool)GetPropertyValue(chiefRoam, "IsResidentScriptedControlActive"), Is.False);
        Assert.That((bool)GetPropertyValue(companionRoam, "IsResidentScriptedControlActive"), Is.False,
            "19:30 进入 free time 后，001/002 必须退出晚段 story actor policy，恢复剧情外交互。");
    }

    [Test]
    public void Director_ShouldExposeFreeRoamBeatDuringPostTutorialExploreWindow()
    {
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component storyManager = Track(new GameObject("StoryManager_ExploreBeat")).AddComponent(storyManagerType);
        Component director = Track(new GameObject("SpringDay1Director_ExploreBeat")).AddComponent(springDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", director);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FarmingTutorial"), false);
        SetField(director, "_postTutorialExploreWindowEntered", true);

        string beatKey = (string)InvokeInstance(director, "GetCurrentBeatKey");

        Assert.That(beatKey, Is.EqualTo("FreeTime_NightWitness"),
            "0.0.6 explore window 已进入后，不应继续把 Town resident 当成 FarmingTutorial_Fieldwork。");
    }

    [Test]
    public void Director_ResetDinnerCueWaitState_ShouldPreservePlacedStoryActors()
    {
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Component director = Track(new GameObject("SpringDay1Director_DinnerWaitReset")).AddComponent(springDirectorType);

        SetField(director, "_dinnerCueWaitStartedAt", 3f);
        SetField(director, "_dinnerStoryActorsWaitStartedAt", 4f);
        SetField(director, "_dinnerStoryActorsPlaced", true);

        InvokeInstance(director, "ResetDinnerCueWaitState");

        Assert.That((float)GetFieldValue(director, "_dinnerCueWaitStartedAt"), Is.EqualTo(-1f));
        Assert.That((float)GetFieldValue(director, "_dinnerStoryActorsWaitStartedAt"), Is.EqualTo(-1f));
        Assert.That((bool)GetFieldValue(director, "_dinnerStoryActorsPlaced"), Is.True,
            "晚饭对白排队后只应清等待计时，不应把已就位的 001/002 重置成“还没摆位”。");
    }

    [Test]
    public void CrowdDirector_ShouldResyncRecoveredResidentsBeforeSkippingDaytimeYield()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");

        Component storyManager = Track(new GameObject("StoryManager_RecoverSync")).AddComponent(storyManagerType);
        Component springDirector = Track(new GameObject("SpringDay1Director_RecoverSync")).AddComponent(springDirectorType);
        Component crowdDirector = Track(new GameObject("CrowdDirector_RecoverSync")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", springDirector);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FarmingTutorial"), false);
        SetField(springDirector, "_postTutorialExploreWindowEntered", true);

        GameObject resident = Track(new GameObject("103"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        SceneManager.MoveGameObjectToScene(resident, townScene);

        object staleState = Activator.CreateInstance(spawnStateType, nonPublic: true);
        IDictionary spawnStates = (IDictionary)GetFieldValue(crowdDirector, "_spawnStates");
        spawnStates["103"] = staleState;
        SetField(crowdDirector, "_syncRequested", true);

        InvokeInstance(crowdDirector, "Update");

        object reboundState = spawnStates["103"];
        Assert.That(GetFieldValue(reboundState, "Instance"), Is.EqualTo(resident),
            "Town re-entry 时，即使白天没有 pending release，只要 continuity 现场需要 re-sync，也必须先把 resident 重新绑定回来，而不是直接 return 掉。");
    }

    [Test]
    public void CrowdDirector_ShouldYieldAutonomyDuringNightWitnessFreeWindow()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");

        Component timeManager = Track(new GameObject("TimeManager_NightWitnessYield")).AddComponent(timeManagerType);
        Component storyManager = Track(new GameObject("StoryManager_NightWitnessYield")).AddComponent(storyManagerType);
        Component springDirector = Track(new GameObject("SpringDay1Director_NightWitnessYield")).AddComponent(springDirectorType);
        Component crowdDirector = Track(new GameObject("CrowdDirector_NightWitnessYield")).AddComponent(crowdDirectorType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", springDirector);
        SetField(timeManager, "currentHour", 19);
        SetField(timeManager, "currentMinute", 30);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FarmingTutorial"), false);
        SetField(springDirector, "_postTutorialExploreWindowEntered", true);

        GameObject resident = Track(new GameObject("102"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", resident);
        SetField(state, "AppliedBeatKey", "EnterVillage_PostEntry");

        IDictionary spawnStates = (IDictionary)GetFieldValue(crowdDirector, "_spawnStates");
        spawnStates["102"] = state;

        bool shouldYield = (bool)InvokeInstance(crowdDirector, "ShouldYieldDaytimeResidentsToAutonomy");

        Assert.That(shouldYield, Is.True,
            "进入 0.0.6 的 night-witness/free window 后，Town resident 应恢复白天自治；不能继续把 FreeTime_NightWitness 当成 crowd hold 的延期。");
    }

    [Test]
    public void CrowdDirector_ShouldStandDownDuringPostTutorialTownAutonomyWindowUntilDinnerStarts()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type manifestType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest");
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");

        Component storyManager = Track(new GameObject("StoryManager_PostTutorialTownStandDown")).AddComponent(storyManagerType);
        Component springDirector = Track(new GameObject("SpringDay1Director_PostTutorialTownStandDown")).AddComponent(springDirectorType);
        Component crowdDirector = Track(new GameObject("CrowdDirector_PostTutorialTownStandDown")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", springDirector);
        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FarmingTutorial"), false);
        SetField(springDirector, "_postTutorialExploreWindowEntered", true);

        ScriptableObject manifest = ScriptableObject.CreateInstance(manifestType);
        Track(manifest);
        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "npcId", "103");
        SetField(entry, "anchorObjectName", string.Empty);
        SetField(entry, "semanticAnchorIds", Array.Empty<string>());
        SetField(entry, "spawnOffset", Vector2.zero);
        SetField(entry, "fallbackWorldPosition", Vector2.zero);
        SetField(entry, "initialFacing", Vector2.left);
        SetField(entry, "residentBaseline", Enum.Parse(residentBaselineType, "DaytimeResident"));
        Array entryArray = Array.CreateInstance(entryType, 1);
        entryArray.SetValue(entry, 0);
        InvokeInstance(manifest, "SetEntries", entryArray);
        SetField(crowdDirector, "_manifest", manifest);

        GameObject resident = Track(new GameObject("103"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        GameObject homeAnchor = Track(new GameObject("103_HomeAnchor"));
        SceneManager.MoveGameObjectToScene(resident, townScene);
        SceneManager.MoveGameObjectToScene(homeAnchor, townScene);

        SetField(crowdDirector, "_syncRequested", true);

        InvokeInstance(crowdDirector, "Update");

        IDictionary spawnStates = (IDictionary)GetFieldValue(crowdDirector, "_spawnStates");
        Assert.That(spawnStates.Count, Is.EqualTo(0),
            "0.0.6 白天自由窗口回 Town 时，CrowdDirector 应该站开，不再为了 ordinary resident 做 recover/sync/release 风暴。");

        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "DinnerConflict"), false);
        SetField(crowdDirector, "_syncRequested", true);

        InvokeInstance(crowdDirector, "Update");

        Assert.That(spawnStates.Contains("103"), Is.True,
            "一旦正式进入晚饭块，CrowdDirector 仍应重新接回 dinner 需要的 resident runtime，而不是永久停摆。");
    }

    [Test]
    public void CrowdDirector_FreeTimeBeforeTwenty_ShouldYieldOrdinaryResidentsToAutonomy()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component timeManager = Track(new GameObject("TimeManager_FreeTimeRelease")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 19);
        SetField(timeManager, "currentMinute", 30);

        Component storyManager = Track(new GameObject("StoryManager_FreeTimeRelease")).AddComponent(storyManagerType);
        Component springDirector = Track(new GameObject("SpringDay1Director_FreeTimeRelease")).AddComponent(springDirectorType);
        Component crowdDirector = Track(new GameObject("CrowdDirector_FreeTimeRelease")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", springDirector);

        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FreeTime"), false);

        GameObject resident = Track(new GameObject("201"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        SceneManager.MoveGameObjectToScene(resident, townScene);

        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", resident);
        IDictionary spawnStates = (IDictionary)GetFieldValue(crowdDirector, "_spawnStates");
        spawnStates["201"] = state;

        bool shouldYield = (bool)InvokeInstance(crowdDirector, "ShouldYieldDaytimeResidentsToAutonomy");

        Assert.That(shouldYield, Is.True,
            "19:30 进入 free time 后，ordinary resident 应直接恢复自治，不再回旧 bucket 或 takeover-ready。");
    }

    [Test]
    public void CrowdDirector_FreeTimeBeforeTwenty_ShouldYieldWitnessResidentsToAutonomy()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type springDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        Component timeManager = Track(new GameObject("TimeManager_FreeTimeWitnessRelease")).AddComponent(timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "currentHour", 19);
        SetField(timeManager, "currentMinute", 30);

        Component storyManager = Track(new GameObject("StoryManager_FreeTimeWitnessRelease")).AddComponent(storyManagerType);
        Component springDirector = Track(new GameObject("SpringDay1Director_FreeTimeWitnessRelease")).AddComponent(springDirectorType);
        Component crowdDirector = Track(new GameObject("CrowdDirector_FreeTimeWitnessRelease")).AddComponent(crowdDirectorType);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(springDirectorType, "_instance", springDirector);

        InvokeInstance(storyManager, "ResetState", ParseEnum(storyPhaseType, "FreeTime"), false);

        GameObject resident = Track(new GameObject("102"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        SceneManager.MoveGameObjectToScene(resident, townScene);

        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", resident);
        IDictionary spawnStates = (IDictionary)GetFieldValue(crowdDirector, "_spawnStates");
        spawnStates["102"] = state;

        bool shouldYield = (bool)InvokeInstance(crowdDirector, "ShouldYieldDaytimeResidentsToAutonomy");

        Assert.That(shouldYield, Is.True,
            "19:30 之后按用户新语义是“全员自由活动”，连 night-witness 相关 resident 也不该继续留在旧 takeover bucket。");
    }

    [Test]
    public void CrowdDirector_ReleaseResidentToAutonomousRoam_ShouldRestoreDialogueRightsAfterCueRelease()
    {
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");
        Type dialogueType = ResolveTypeOrFail("Sunset.Story.NPCDialogueInteractable");
        Type informalType = ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable");

        Component director = Track(new GameObject("CrowdDirector_FreeTimeDialogueUnlock")).AddComponent(crowdDirectorType);
        GameObject resident = Track(new GameObject("201"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        Behaviour formalDialogue = (Behaviour)resident.AddComponent(dialogueType);
        Behaviour informalDialogue = (Behaviour)resident.AddComponent(informalType);

        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", resident);
        SetField(state, "AppliedCueKey", "scene-marker-201");
        SetField(state, "AppliedBeatKey", "DinnerConflict_Table");
        SetField(state, "BaseResolvedAnchorName", "201_HomeAnchor");

        InvokeStatic(crowdDirectorType, "SetResidentCueInteractionLock", state, true);

        Assert.That(formalDialogue.enabled, Is.True);
        Assert.That(informalDialogue.enabled, Is.True);
        Assert.That((bool)GetFieldValue(state, "InteractionLockOwnedByCue"), Is.True,
            "cue 锁现在只记 runtime lock owner，不应再直接把 formal / informal 组件关掉。");

        InvokeInstance(director, "ReleaseResidentToAutonomousRoam", state);

        Assert.That(formalDialogue.enabled, Is.True);
        Assert.That(informalDialogue.enabled, Is.True);
        Assert.That((bool)GetFieldValue(state, "InteractionLockOwnedByCue"), Is.False,
            "19:30 放人后，ordinary resident 的 formal / informal 交互锁必须一起退出。");
    }

    [Test]
    public void CrowdDirector_ConfigureBoundNpc_ShouldAddInformalChatWhenFormalDialogueExists()
    {
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type entryType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest+Entry");
        Type contentProfileType = ResolveTypeOrFail("NPCDialogueContentProfile");

        Component director = Track(new GameObject("CrowdDirector_AddInformalWithFormal")).AddComponent(crowdDirectorType);
        GameObject resident = Track(new GameObject("Resident_AddInformalWithFormal"));
        Component roamController = resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        resident.AddComponent(ResolveTypeOrFail("Sunset.Story.NPCDialogueInteractable"));

        ScriptableObject roamProfile = ScriptableObject.CreateInstance(ResolveTypeOrFail("NPCRoamProfile"));
        ScriptableObject contentProfile = ScriptableObject.CreateInstance(contentProfileType);
        Type exchangeType = ResolveTypeOrFail("NPCDialogueContentProfile+InformalChatExchange");
        object exchange = Activator.CreateInstance(exchangeType);
        SetField(exchange, "playerLines", new[] { "玩家句" });
        SetField(exchange, "npcReplyLines", new[] { "NPC句" });

        Type bundleType = ResolveTypeOrFail("NPCDialogueContentProfile+InformalConversationBundle");
        object bundle = Activator.CreateInstance(bundleType);
        Array exchangeArray = Array.CreateInstance(exchangeType, 1);
        exchangeArray.SetValue(exchange, 0);
        SetField(bundle, "exchanges", exchangeArray);

        Array bundleArray = Array.CreateInstance(bundleType, 1);
        bundleArray.SetValue(bundle, 0);
        SetField(contentProfile, "defaultInformalConversationBundles", bundleArray);
        SetField(roamProfile, "dialogueContentProfile", contentProfile);
        SetField(roamController, "roamProfile", roamProfile);

        object entry = Activator.CreateInstance(entryType);
        SetField(entry, "initialFacing", Vector2.down);

        InvokeInstance(director, "ConfigureBoundNpc", entry, resident, null);

        Assert.That(resident.GetComponent(ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable")), Is.Not.Null,
            "formal 组件存在不应再阻止 resident 自动补 informal 入口；否则剧情外 formal 让位后会没有居民闲聊口。");
    }

    [Test]
    public void InformalChatBootstrap_ShouldAddInformalComponentEvenWhenFormalDialogueExists()
    {
        Type informalType = ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable");
        Type contentProfileType = ResolveTypeOrFail("NPCDialogueContentProfile");

        GameObject resident = Track(new GameObject("Resident_BootstrapFormalAndInformal"));
        Component roamController = resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        resident.AddComponent(ResolveTypeOrFail("Sunset.Story.NPCDialogueInteractable"));

        ScriptableObject roamProfile = ScriptableObject.CreateInstance(ResolveTypeOrFail("NPCRoamProfile"));
        ScriptableObject contentProfile = ScriptableObject.CreateInstance(contentProfileType);
        Type exchangeType = ResolveTypeOrFail("NPCDialogueContentProfile+InformalChatExchange");
        object exchange = Activator.CreateInstance(exchangeType);
        SetField(exchange, "playerLines", new[] { "玩家句" });
        SetField(exchange, "npcReplyLines", new[] { "NPC句" });

        Type bundleType = ResolveTypeOrFail("NPCDialogueContentProfile+InformalConversationBundle");
        object bundle = Activator.CreateInstance(bundleType);
        Array exchangeArray = Array.CreateInstance(exchangeType, 1);
        exchangeArray.SetValue(exchange, 0);
        SetField(bundle, "exchanges", exchangeArray);

        Array bundleArray = Array.CreateInstance(bundleType, 1);
        bundleArray.SetValue(bundle, 0);
        SetField(contentProfile, "defaultInformalConversationBundles", bundleArray);
        SetField(roamProfile, "dialogueContentProfile", contentProfile);
        SetField(roamController, "roamProfile", roamProfile);

        InvokeStatic(informalType, "BootstrapRuntime");

        Assert.That(resident.GetComponent(informalType), Is.Not.Null,
            "像 001/002 这种既有 formal 又有居民闲聊内容的 NPC，runtime bootstrap 也必须补上 informal 入口。");
    }

    [Test]
    public void CrowdDirector_TryBeginResidentReturnHome_ShouldReleaseControlAndRetryLaterWhenDriveStartFails()
    {
        Scene townScene = CreateNamedTestScene("Town");
        SceneManager.SetActiveScene(townScene);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");

        Component director = Track(new GameObject("CrowdDirector_ReturnHomeRetry")).AddComponent(crowdDirectorType);
        SceneManager.MoveGameObjectToScene(director.gameObject, townScene);

        GameObject resident = Track(new GameObject("Resident_ReturnHomeRetry"));
        resident.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Behaviour roamController = (Behaviour)resident.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        resident.transform.position = new Vector3(-3f, 0f, 0f);
        SceneManager.MoveGameObjectToScene(resident, townScene);

        GameObject homeAnchor = Track(new GameObject("Resident_ReturnHomeRetry_HomeAnchor"));
        homeAnchor.transform.position = new Vector3(4f, 0f, 0f);
        SceneManager.MoveGameObjectToScene(homeAnchor, townScene);

        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", resident);
        SetField(state, "HomeAnchor", homeAnchor);
        SetField(state, "BaseResolvedAnchorName", "Resident_ReturnHomeRetry_HomeAnchor");
        SetField(state, "BaseFacing", Vector2.right);

        float retryBefore = Time.unscaledTime;
        bool started = (bool)InvokeInstance(director, "TryBeginResidentReturnHome", state);

        Assert.That(started, Is.False, "20:00 回家起步失败时，不应把 NPC 假装标成已经在回家。");
        Assert.That((bool)GetFieldValue(state, "IsReturningHome"), Is.False);
        Assert.That((bool)GetPropertyValue(roamController, "IsResidentScriptedControlActive"), Is.False,
            "回家起步失败后，不应把 resident 留在 scripted control 假接管态里。");
        Assert.That((float)GetFieldValue(state, "NextReturnHomeDriveRetryAt"), Is.GreaterThan(retryBefore),
            "起步失败后应留下一个短重试窗，而不是永久卡死在第一脚失败。");
        StringAssert.Contains("return-home-pending", (string)GetFieldValue(state, "ResolvedAnchorName"));
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
    public void TownHouseLeadWait_ShouldKeepCompanionClosingFormationAroundPausedChief()
    {
        GameObject runtime = Track(new GameObject("SpringDay1Director_Runtime"));
        Component director = runtime.AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1Director"));
        GameObject chief = Track(new GameObject("001"));
        GameObject companion = Track(new GameObject("002"));
        Vector3 leadTarget = new Vector3(12f, 3.5f, 0f);
        chief.transform.position = new Vector3(8f, 3.5f, 0f);

        Vector3 companionTarget = (Vector3)InvokeInstance(
            director,
            "BuildEscortCompanionTarget",
            chief.transform.position,
            leadTarget);
        companion.transform.position = companionTarget + new Vector3(-2.4f, -1.2f, 0f);
        Vector3 chiefStart = chief.transform.position;
        float startDistance = Vector2.Distance(companion.transform.position, companionTarget);

        bool moved = (bool)InvokeInstance(
            director,
            "TryDriveEscortCompanionTowardLeaderFormation",
            chief.transform,
            companion.transform,
            null,
            leadTarget,
            0.65f);

        float endDistance = Vector2.Distance(companion.transform.position, companionTarget);
        Assert.That(moved, Is.True, "001 等玩家时，002 不应一起冻结，而应继续向 001 的动态编队位补位。");
        Assert.That(chief.transform.position, Is.EqualTo(chiefStart), "补位只应移动 002，不应拖动已经停下等玩家的 001。");
        Assert.That(endDistance, Is.LessThan(startDistance), "002 应更靠近基于 001 当前位置计算出来的跟随位。");
    }

    [Test]
    public void BuildEscortCompanionTarget_ShouldMaintainExplicitFormationDistance()
    {
        GameObject runtime = Track(new GameObject("SpringDay1Director_Runtime"));
        Component director = runtime.AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1Director"));
        Vector3 leaderPosition = new Vector3(8f, 3.5f, 0f);
        Vector3 leadTarget = new Vector3(12f, 3.5f, 0f);

        Vector3 companionTarget = (Vector3)InvokeInstance(
            director,
            "BuildEscortCompanionTarget",
            leaderPosition,
            leadTarget);

        Assert.That(Vector2.Distance(leaderPosition, companionTarget), Is.EqualTo(1.5f).Within(0.001f),
            "002 的编队位要显式保持约 1.5 单位，不再靠一组过短的 side/trailing 偏移碰运气。");
    }

    [Test]
    public void HealingBridge_ShouldUsePlayerCenterAsProximityPoint()
    {
        GameObject player = Track(new GameObject("Player_HealingCenter"));
        player.transform.position = new Vector3(1.4f, -0.2f, 0f);
        player.AddComponent<BoxCollider2D>().size = new Vector2(1.2f, 2.4f);

        Vector2 interactionSamplePoint = (Vector2)InvokeStatic(
            ResolveTypeOrFail("Sunset.Story.SpringDay1UiLayerUtility"),
            "GetInteractionSamplePoint",
            player.transform);
        Vector2 healingProximityPoint = (Vector2)InvokeStatic(
            ResolveTypeOrFail("Sunset.Story.SpringDay1Director"),
            "GetHealingProximityPoint",
            player.transform);

        Assert.That(healingProximityPoint, Is.EqualTo((Vector2)player.transform.position),
            "疗伤触发应以玩家本体位置进圆判定，不应继续吃交互 sample 点。");
        Assert.That(healingProximityPoint, Is.Not.EqualTo(interactionSamplePoint),
            "这条测试要钉死：疗伤 proximity 不再复用 UI/交互 sample 点。");
    }

    [Test]
    public void HealingBridge_ShouldUseSupportTransformAsProximityAnchor()
    {
        GameObject supportNpc = Track(new GameObject("002"));
        supportNpc.transform.position = new Vector3(2.4f, -1.1f, 0f);
        BoxCollider2D collider = supportNpc.AddComponent<BoxCollider2D>();
        collider.offset = new Vector2(0.82f, 0.34f);
        collider.size = new Vector2(1.2f, 1.8f);
        Physics2D.SyncTransforms();

        Vector2 supportSamplePoint = (Vector2)InvokeStatic(
            ResolveTypeOrFail("Sunset.Story.SpringDay1Director"),
            "GetHealingSupportSamplePoint",
            supportNpc.transform);

        Assert.That(supportSamplePoint, Is.EqualTo((Vector2)supportNpc.transform.position),
            "疗伤接近判定现在应是最简单的 transform.position 圆心，不再吃 collider/presentation center。");
        Assert.That(supportSamplePoint, Is.Not.EqualTo((Vector2)collider.bounds.center),
            "这条测试要钉死：即便艾拉碰撞中心偏移，疗伤半径也只认 transform.position。");
    }

    [Test]
    public void HealingBridge_ShouldTurnCompanionToFacePlayerOnceHealingTriggers()
    {
        GameObject runtime = Track(new GameObject("SpringDay1Director_Runtime"));
        Component director = runtime.AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1Director"));
        GameObject supportNpc = Track(new GameObject("002"));
        supportNpc.transform.position = Vector3.zero;
        supportNpc.AddComponent<Animator>();
        supportNpc.AddComponent<SpriteRenderer>();
        Component anim = supportNpc.AddComponent(ResolveTypeOrFail("NPCAnimController"));
        Component motion = supportNpc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        SetField(motion, "animController", anim);

        InvokeInstance(
            director,
            "FaceHealingSupportTowardPlayer",
            supportNpc.transform,
            new Vector2(3f, 0f));

        Assert.That(GetPropertyValue(anim, "CurrentDirection").ToString(), Is.EqualTo("Right"),
            "疗伤触发后，艾拉应最小化地朝向玩家，避免继续保持旧朝向。");
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
    public void ResidentScriptedControl_ReleaseStoryControlShouldEnterMovingStateWithoutShortPause()
    {
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        GameObject navGridGo = Track(new GameObject("NavGrid_ReleaseImmediateResume"));
        GameObject npc = Track(new GameObject("NPC_ReleaseImmediateResume"));
        GameObject homeAnchor = Track(new GameObject("NPC_ReleaseImmediateResume_Home"));

        Component navGrid = navGridGo.AddComponent(navGridType);
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

        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Behaviour roam = (Behaviour)npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        BoxCollider2D npcCollider = npc.AddComponent<BoxCollider2D>();
        SetField(roam, "navGrid", navGrid);
        SetField(roam, "navigationCollider", npcCollider);
        homeAnchor.transform.position = new Vector3(2f, 0f, 0f);
        InvokeInstance(roam, "BindResidentHomeAnchor", homeAnchor.transform);
        npc.transform.position = new Vector3(0f, 0f, 0f);

        InvokeInstance(roam, "StartRoam");
        Assert.That((string)GetPropertyValue(roam, "DebugState"), Is.EqualTo("ShortPause"),
            "测试前提：自治刚启动时仍会先进入短停。");

        InvokeInstance(roam, "AcquireResidentScriptedControl", "spring-day1-director", true);
        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedControlActive"), Is.True);

        InvokeInstance(roam, "ReleaseStoryControl", "spring-day1-director", true);

        Assert.That((bool)GetPropertyValue(roam, "IsResidentScriptedControlActive"), Is.False);
        Assert.That((string)GetPropertyValue(roam, "DebugState"), Is.EqualTo("Moving"),
            "story control 释放后应直接进入移动，不应再先落到 ShortPause。");
    }

    [Test]
    public void ResidentScriptedControl_DebugMoveShouldStillParticipateInSharedAvoidance()
    {
        GameObject npc = Track(new GameObject("NPC_ScriptedMoveAvoidance"));
        Behaviour roam = (Behaviour)npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        Type pointToPointTravelType = ResolveTypeOrFail("NPCAutoRoamController+PointToPointTravelContract");

        InvokeInstance(roam, "AcquireResidentScriptedControl", "spring-day1-director", true);
        SetField(roam, "debugMoveActive", true);
        SetField(roam, "activePointToPointTravelContract", ParseEnum(pointToPointTravelType, "ResidentScripted"));

        bool bypassWhileScripted = (bool)InvokeInstance(roam, "ShouldBypassSharedAvoidanceForCurrentMove");
        Assert.That(bypassWhileScripted, Is.False, "resident scripted move 也应继续参与 shared avoidance，而不是直接绕开人群避让。");

        InvokeInstance(roam, "ReleaseResidentScriptedControl", "spring-day1-director", false);
        SetField(roam, "debugMoveActive", true);
        SetField(roam, "activePointToPointTravelContract", ParseEnum(pointToPointTravelType, "PlainDebug"));

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
        Type pointToPointTravelType = ResolveTypeOrFail("NPCAutoRoamController+PointToPointTravelContract");
        Vector2 requestedDestination = new Vector2(4f, 1.5f);

        InvokeInstance(roam, "AcquireResidentScriptedControl", "spring-day1-director", true);
        SetField(roam, "debugMoveActive", true);
        SetField(roam, "residentScriptedMovePaused", false);
        SetField(roam, "state", ParseEnum(roamStateType, "Moving"));
        SetField(roam, "activePointToPointTravelContract", ParseEnum(pointToPointTravelType, "ResidentScripted"));
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

    [Test]
    public void CrowdDirector_QueueResidentAutonomousRoamResume_ShouldQueueInsteadOfImmediateRestart()
    {
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");

        Component director = Track(new GameObject("CrowdDirector_QueuedResume")).AddComponent(crowdDirectorType);
        GameObject npc = Track(new GameObject("NPC_QueuedResume"));
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Behaviour roam = (Behaviour)npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
        SetField(state, "Instance", npc);

        InvokeStatic(crowdDirectorType, "QueueResidentAutonomousRoamResume", state, false);

        Assert.That((bool)GetFieldValue(state, "QueuedAutonomousResume"), Is.True);
        Assert.That((float)GetFieldValue(state, "NextAutonomousResumeAt"), Is.GreaterThan(0f));
        Assert.That((bool)GetPropertyValue(roam, "IsRoaming"), Is.False, "进入队列后不应同帧立刻 restart roam。");
    }

    [Test]
    public void CrowdDirector_TickQueuedAutonomousResumes_ShouldThrottleResumeBurst()
    {
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        Type spawnStateType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector+SpawnState");

        Component director = Track(new GameObject("CrowdDirector_QueuedResumeTick")).AddComponent(crowdDirectorType);
        IDictionary spawnStates = (IDictionary)GetFieldValue(director, "_spawnStates");
        List<Behaviour> roamControllers = new List<Behaviour>();
        List<object> states = new List<object>();

        for (int index = 0; index < 3; index++)
        {
            GameObject npc = Track(new GameObject($"NPC_QueuedResumeTick_{index}"));
            npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
            Behaviour roam = (Behaviour)npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
            roamControllers.Add(roam);

            object state = Activator.CreateInstance(spawnStateType, nonPublic: true);
            SetField(state, "Instance", npc);
            SetField(state, "QueuedAutonomousResume", true);
            SetField(state, "QueuedAutonomousResumeTryImmediateMove", false);
            SetField(state, "NextAutonomousResumeAt", -1f);
            spawnStates[$"npc-{index}"] = state;
            states.Add(state);
        }

        SetField(director, "_nextQueuedAutonomousResumeDispatchAt", 0f);
        InvokeInstance(director, "TickQueuedAutonomousResumes");

        int roamingCount = 0;
        for (int index = 0; index < roamControllers.Count; index++)
        {
            if ((bool)GetPropertyValue(roamControllers[index], "IsRoaming"))
            {
                roamingCount++;
            }
        }

        int queuedCount = 0;
        for (int index = 0; index < states.Count; index++)
        {
            if ((bool)GetFieldValue(states[index], "QueuedAutonomousResume"))
            {
                queuedCount++;
            }
        }

        Assert.That(roamingCount, Is.EqualTo(2), "同一 tick 内恢复自治的 resident 数量应被节流，而不是整批同窗起跑。");
        Assert.That(queuedCount, Is.EqualTo(1), "超出的 resident 应继续留在短队列里，下一小窗再恢复。");
    }

    private GameObject CreateMarker(Transform parent, string name, Vector3 position)
    {
        GameObject marker = Track(new GameObject(name));
        marker.transform.SetParent(parent, false);
        marker.transform.position = position;
        return marker;
    }

    private void CreateResidentWithHomeAnchor(
        string npcName,
        Vector3 npcPosition,
        Vector3 homeAnchorPosition,
        Type motionControllerType,
        Type roamControllerType,
        Scene scene)
    {
        GameObject npc = Track(new GameObject(npcName));
        npc.AddComponent(motionControllerType);
        npc.AddComponent(roamControllerType);
        npc.transform.position = npcPosition;
        SceneManager.MoveGameObjectToScene(npc, scene);

        GameObject homeAnchor = Track(new GameObject($"{npcName}_HomeAnchor"));
        homeAnchor.transform.position = homeAnchorPosition;
        SceneManager.MoveGameObjectToScene(homeAnchor, scene);
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

    private static void AssertDeprecatedTownContractAnchorIgnored(Component director, string npcId, string semanticAnchorId, string beatKey)
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

        Assert.That(usedFallback, Is.True, $"{semanticAnchorId} 不应再作为 late-day runtime spawn 的 Town contract 真值。");
        Assert.That(anchorName, Is.EqualTo("fallback"));
        Assert.That(position.x, Is.EqualTo(0f).Within(0.001f));
        Assert.That(position.y, Is.EqualTo(0f).Within(0.001f));
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
        MethodInfo[] methods = targetType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
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

                if (!parameterType.IsAssignableFrom(argument.GetType()))
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

        Assert.That(method, Is.Not.Null, $"未找到静态方法: {targetType.Name}.{methodName}");
        return method.Invoke(null, args);
    }

    private static string[] ExtractNpcIds(Array entries)
    {
        if (entries == null)
        {
            return Array.Empty<string>();
        }

        string[] npcIds = new string[entries.Length];
        for (int index = 0; index < entries.Length; index++)
        {
            npcIds[index] = (string)GetFieldValue(entries.GetValue(index), "npcId");
        }

        return npcIds;
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
