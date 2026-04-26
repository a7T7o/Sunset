using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class StoryProgressPersistenceServiceTests
{
    private const string WorkbenchHintConsumedKey = "spring-day1.workbench-entry-hint-consumed";

    private readonly List<GameObject> createdObjects = new();

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

        Type relationshipServiceType = ResolveTypeOrFail("PlayerNpcRelationshipService");
        Type relationshipStageType = ResolveTypeOrFail("NPCRelationshipStage");
        object emptySnapshot = Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(typeof(string), relationshipStageType));
        InvokeStatic(relationshipServiceType, "ReplaceAllStages", emptySnapshot, true);

        PlayerPrefs.DeleteKey(WorkbenchHintConsumedKey);
        PlayerPrefs.Save();

        TrySetStaticField(ResolveTypeOrFail("Sunset.Story.StoryProgressPersistenceService"), "_instance", null);
        TrySetStaticField(ResolveTypeOrFail("Sunset.Story.StoryProgressPersistenceService"), "s_workbenchHintConsumed", false);
        TrySetStaticField(ResolveTypeOrFail("Sunset.Story.StoryManager"), "_instance", null);
        TrySetStaticField(ResolveTypeOrFail("Sunset.Story.SpringDay1Director"), "_instance", null);
        TrySetStaticField(ResolveTypeOrFail("Sunset.Story.DialogueManager"), "<Instance>k__BackingField", null);
        TrySetStaticField(ResolveTypeOrFail("HealthSystem"), "_instance", null);
        TrySetStaticField(ResolveTypeOrFail("EnergySystem"), "<Instance>k__BackingField", null);
        TrySetStaticField(ResolveTypeOrFail("PlayerNpcChatSessionService"), "<Instance>k__BackingField", null);
        TrySetStaticField(relationshipServiceType, "knownNpcIdsLoaded", false);

        ClearStaticCollection(relationshipServiceType, "CachedStages");
        ClearStaticCollection(relationshipServiceType, "KnownNpcIds");
        ClearStaticCollection(ResolveTypeOrFail("Sunset.Story.StoryProgressPersistenceService"), "WorkbenchRuntimeStateByStation");
    }

    [Test]
    public void CanSaveNow_BlocksDialogueAndNpcChat_ButAllowsWorkbenchRuntimePersistence()
    {
        RuntimeContext context = CreateRuntimeContext();
        Type persistenceServiceType = ResolveTypeOrFail("Sunset.Story.StoryProgressPersistenceService");

        SetAutoPropertyBackingField(context.dialogueManager, "IsDialogueActive", true);
        bool canSaveDuringDialogue = InvokeCanSaveNow(persistenceServiceType, out string dialogueReason);
        Assert.That(canSaveDuringDialogue, Is.False);
        StringAssert.Contains("正式对白", dialogueReason);

        SetAutoPropertyBackingField(context.dialogueManager, "IsDialogueActive", false);

        Type chatSessionType = ResolveTypeOrFail("PlayerNpcChatSessionService");
        Type informalInteractableType = ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable");
        Component chatSessionService = Track(new GameObject("ChatSessionService")).AddComponent(chatSessionType);
        Component interactable = Track(new GameObject("NpcInformalInteractable")).AddComponent(informalInteractableType);
        SetPrivateField(chatSessionService, "_activeInteractable", interactable);
        SetPrivateField(chatSessionService, "_state", ParseNestedEnum(chatSessionService, "SessionState", "WaitingNpcReply"));

        bool canSaveDuringChat = InvokeCanSaveNow(persistenceServiceType, out string chatReason);
        Assert.That(canSaveDuringChat, Is.False);
        StringAssert.Contains("NPC 闲聊", chatReason);

        SetPrivateField(chatSessionService, "_activeInteractable", null);
        SetPrivateField(chatSessionService, "_state", ParseNestedEnum(chatSessionService, "SessionState", "Idle"));
        SetPrivateField(context.director, "_workbenchCraftingActive", true);

        bool canSaveDuringCrafting = InvokeCanSaveNow(persistenceServiceType, out string craftingReason);
        Assert.That(canSaveDuringCrafting, Is.True,
            "工作台 runtime 状态现在会先 flush 进正式持久层，不应再因为正在制作而直接拦存档。");
        Assert.That(craftingReason, Is.Empty);
    }

    [Test]
    public void CanSaveNow_ShouldFlushWorkbenchRuntimeStateInsteadOfBlockingWorkbenchQueue()
    {
        string projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? Directory.GetCurrentDirectory();
        string persistenceText = File.ReadAllText(Path.Combine(
            projectRoot,
            "Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs"));

        StringAssert.Contains("SpringDay1WorkbenchCraftingOverlay.FlushRuntimeStateToPersistence();", persistenceText,
            "正式存档前应先把工作台 runtime 状态落到长期持久层，而不是继续把工作台当成 save blocker。");
        Assert.That(persistenceText, Does.Not.Contain("overlay.HasReadyWorkbenchOutputs"),
            "工作台 ready outputs 已进入正式持久化链后，不应继续靠 blocker 文本守住。");
        Assert.That(persistenceText, Does.Not.Contain("overlay.HasWorkbenchFloatingState"),
            "工作台队列状态已正式进 snapshot 后，不应继续要求用户必须先清空工作台才能保存。");
    }

    [Test]
    public void SaveLoad_RoundTripRestoresWorkbenchRuntimeStates()
    {
        RuntimeContext context = CreateRuntimeContext();
        Type persistenceServiceType = ResolveTypeOrFail("Sunset.Story.StoryProgressPersistenceService");
        Type workbenchStateType = ResolveTypeOrFail("FarmGame.Data.Core.WorkbenchRuntimeSaveData");
        Type queueEntryType = ResolveTypeOrFail("FarmGame.Data.Core.WorkbenchQueueEntrySaveData");
        const string stationKey = "Assets/000_Scenes/Home.unity|Stations/Workbench|1.000|2.000|0.000";

        object savedWorkbenchState = CreateWorkbenchRuntimeState(
            workbenchStateType,
            queueEntryType,
            stationKey,
            "Home",
            "Assets/000_Scenes/Home.unity",
            recipeId: 9102,
            resultItemId: 7,
            totalCount: 3,
            readyCount: 1,
            currentUnitProgress: 0.35f,
            hasReservedCurrentCraft: true,
            isActiveEntry: true);
        InvokeStatic(persistenceServiceType, "StoreWorkbenchRuntimeState", savedWorkbenchState);

        object savedData = InvokeInstance(context.persistenceService, "Save");
        string genericData = GetFieldValue(savedData, "genericData") as string;
        StringAssert.Contains("workbenchStates", genericData);

        InvokeStatic(persistenceServiceType, "ClearWorkbenchRuntimeStates");
        object staleWorkbenchState = CreateWorkbenchRuntimeState(
            workbenchStateType,
            queueEntryType,
            stationKey,
            "Town",
            "Assets/000_Scenes/Town.unity",
            recipeId: 1,
            resultItemId: 1,
            totalCount: 1,
            readyCount: 0,
            currentUnitProgress: 0f,
            hasReservedCurrentCraft: false,
            isActiveEntry: false);
        InvokeStatic(persistenceServiceType, "StoreWorkbenchRuntimeState", staleWorkbenchState);

        InvokeInstance(context.persistenceService, "Load", savedData);

        Assert.That(TryGetWorkbenchRuntimeState(persistenceServiceType, stationKey, out object restored), Is.True);
        Assert.That(GetFieldValue(restored, "sceneName"), Is.EqualTo("Home"));
        Assert.That(GetFieldValue(restored, "scenePath"), Is.EqualTo("Assets/000_Scenes/Home.unity"));

        System.Collections.IList restoredEntries = GetFieldValue(restored, "queueEntries") as System.Collections.IList;
        Assert.That(restoredEntries, Is.Not.Null);
        Assert.That(restoredEntries.Count, Is.EqualTo(1));
        object restoredEntry = restoredEntries[0];
        Assert.That(GetFieldValue(restoredEntry, "recipeId"), Is.EqualTo(9102));
        Assert.That(GetFieldValue(restoredEntry, "totalCount"), Is.EqualTo(3));
        Assert.That(GetFieldValue(restoredEntry, "readyCount"), Is.EqualTo(1));
        Assert.That(GetFieldValue(restoredEntry, "hasReservedCurrentCraft"), Is.EqualTo(true));
        Assert.That(GetFieldValue(restoredEntry, "isActiveEntry"), Is.EqualTo(true));
    }

    [Test]
    public void CanLoadNow_ShouldBlockQueuedFormalStoryPhaseBeforeDialogueAppears()
    {
        RuntimeContext context = CreateRuntimeContext();
        Type persistenceServiceType = ResolveTypeOrFail("Sunset.Story.StoryProgressPersistenceService");

        InvokeInstance(
            context.storyManager,
            "ResetState",
            ParseEnum(context.storyPhaseType, "WorkbenchFlashback"),
            true);
        InvokeInstance(context.dialogueManager, "ReplaceCompletedSequenceIds", (object)Array.Empty<string>());

        SetPrivateField(context.director, "_workbenchOpened", false);
        SetPrivateField(context.director, "_workbenchSequencePlayed", false);
        SetPrivateField(context.director, "_workbenchBriefingSequenceQueued", false);

        bool canLoad = InvokeCanLoadNow(persistenceServiceType, out string blockerReason);

        Assert.That(canLoad, Is.False);
        StringAssert.Contains("工作台剧情", blockerReason);
    }

    [Test]
    public void SaveLoad_RoundTripRestoresLongTermStoryStateAndClearsNpcTransientState()
    {
        RuntimeContext context = CreateRuntimeContext();

        InvokeInstance(
            context.storyManager,
            "ResetState",
            ParseEnum(context.storyPhaseType, "FarmingTutorial"),
            true);
        InvokeInstance(context.dialogueManager, "ReplaceCompletedSequenceIds", (object)new[] { "spring-day1-workbench", "spring-day1-dinner" });
        InvokeStatic(
            context.relationshipServiceType,
            "SetStage",
            "npc_alpha",
            ParseEnum(context.relationshipStageType, "Close"),
            false);
        InvokeStatic(ResolveTypeOrFail("Sunset.Story.StoryProgressPersistenceService"), "MarkWorkbenchHintConsumed");

        InvokeInstance(context.healthSystem, "SetHealthState", 73, 120);
        InvokeInstance(context.healthSystem, "SetVisible", true);
        InvokeInstance(context.energySystem, "SetEnergyState", 42, 210);
        InvokeInstance(context.energySystem, "SetVisible", true);
        InvokeInstance(context.energySystem, "SetLowEnergyWarningVisual", true);

        SetPrivateField(context.director, "_tillObjectiveCompleted", true);
        SetPrivateField(context.director, "_plantObjectiveCompleted", true);
        SetPrivateField(context.director, "_waterObjectiveCompleted", true);
        SetPrivateField(context.director, "_woodObjectiveCompleted", false);
        SetPrivateField(context.director, "_collectedWoodSinceWoodStepStart", 3);
        SetPrivateField(context.director, "_craftedCount", 2);
        SetPrivateField(context.director, "_staminaRevealed", true);

        object savedData = InvokeInstance(context.persistenceService, "Save");
        Assert.That(savedData, Is.Not.Null);
        Assert.That(GetFieldValue(savedData, "objectType"), Is.EqualTo("StoryProgressState"));

        string genericData = GetFieldValue(savedData, "genericData") as string;
        Assert.That(genericData, Is.Not.Null.And.Not.Empty);
        StringAssert.Contains("completedDialogueSequenceIds", genericData);
        StringAssert.Contains("npcRelationships", genericData);
        StringAssert.Contains("springDay1", genericData);
        StringAssert.Contains("residentRuntimeSnapshots", genericData);
        StringAssert.Contains("health", genericData);
        StringAssert.Contains("energy", genericData);

        Type chatSessionType = ResolveTypeOrFail("PlayerNpcChatSessionService");
        Type informalInteractableType = ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable");
        Type nearbyFeedbackType = ResolveTypeOrFail("PlayerNpcNearbyFeedbackService");
        Type bubblePresenterType = ResolveTypeOrFail("NPCBubblePresenter");

        Component chatSessionService = Track(new GameObject("ChatSessionService")).AddComponent(chatSessionType);
        Component interactable = Track(new GameObject("NpcInformalInteractable")).AddComponent(informalInteractableType);
        SetPrivateField(chatSessionService, "_activeInteractable", interactable);
        SetPrivateField(chatSessionService, "_state", ParseNestedEnum(chatSessionService, "SessionState", "WaitingNpcReply"));
        SetPrivateField(chatSessionService, "_pendingResumeNpcId", "npc_alpha");
        SetPrivateField(chatSessionService, "_pendingResumeExpiresAt", 999f);

        Component nearbyFeedbackService = Track(new GameObject("NearbyFeedbackService")).AddComponent(nearbyFeedbackType);
        Component bubblePresenter = Track(new GameObject("NearbyBubblePresenter")).AddComponent(bubblePresenterType);
        SetPrivateField(nearbyFeedbackService, "activeNearbyBubblePresenter", bubblePresenter);

        InvokeInstance(
            context.storyManager,
            "ResetState",
            ParseEnum(context.storyPhaseType, "CrashAndMeet"),
            false);
        InvokeInstance(context.dialogueManager, "ReplaceCompletedSequenceIds", (object)new[] { "stale-sequence" });
        InvokeStatic(
            context.relationshipServiceType,
            "SetStage",
            "npc_beta",
            ParseEnum(context.relationshipStageType, "Acquainted"),
            false);
        TrySetStaticField(ResolveTypeOrFail("Sunset.Story.StoryProgressPersistenceService"), "s_workbenchHintConsumed", false);
        InvokeInstance(context.healthSystem, "SetHealthState", 1, 10);
        InvokeInstance(context.healthSystem, "SetVisible", false);
        InvokeInstance(context.energySystem, "SetEnergyState", 5, 50);
        InvokeInstance(context.energySystem, "SetVisible", false);
        InvokeInstance(context.energySystem, "SetLowEnergyWarningVisual", false);

        SetPrivateField(context.director, "_tillObjectiveCompleted", false);
        SetPrivateField(context.director, "_plantObjectiveCompleted", false);
        SetPrivateField(context.director, "_waterObjectiveCompleted", false);
        SetPrivateField(context.director, "_woodObjectiveCompleted", false);
        SetPrivateField(context.director, "_collectedWoodSinceWoodStepStart", 0);
        SetPrivateField(context.director, "_craftedCount", 0);
        SetPrivateField(context.director, "_staminaRevealed", false);

        InvokeInstance(context.persistenceService, "Load", savedData);

        Assert.That(
            GetPropertyValue(context.storyManager, "CurrentPhase"),
            Is.EqualTo(ParseEnum(context.storyPhaseType, "FarmingTutorial")));
        Assert.That(GetPropertyValue(context.storyManager, "IsLanguageDecoded"), Is.EqualTo(true));
        Assert.That(InvokeInstance(context.dialogueManager, "HasCompletedSequence", "spring-day1-workbench"), Is.EqualTo(true));
        Assert.That(InvokeInstance(context.dialogueManager, "HasCompletedSequence", "spring-day1-dinner"), Is.EqualTo(true));
        Assert.That(InvokeInstance(context.dialogueManager, "HasCompletedSequence", "stale-sequence"), Is.EqualTo(false));

        Assert.That(
            InvokeStatic(context.relationshipServiceType, "GetStage", "npc_alpha"),
            Is.EqualTo(ParseEnum(context.relationshipStageType, "Close")));
        Assert.That(
            InvokeStatic(context.relationshipServiceType, "GetStage", "npc_beta"),
            Is.EqualTo(ParseEnum(context.relationshipStageType, "Stranger")));
        Assert.That(
            InvokeStatic(ResolveTypeOrFail("Sunset.Story.StoryProgressPersistenceService"), "IsWorkbenchHintConsumed"),
            Is.EqualTo(true));

        Assert.That(GetPropertyValue(context.healthSystem, "CurrentHealth"), Is.EqualTo(73));
        Assert.That(GetPropertyValue(context.healthSystem, "MaxHealth"), Is.EqualTo(120));
        Assert.That(GetPropertyValue(context.healthSystem, "IsVisible"), Is.EqualTo(true));
        Assert.That(GetPropertyValue(context.energySystem, "CurrentEnergy"), Is.EqualTo(42));
        Assert.That(GetPropertyValue(context.energySystem, "MaxEnergy"), Is.EqualTo(210));
        Assert.That(GetPropertyValue(context.energySystem, "IsVisible"), Is.EqualTo(true));
        Assert.That(GetPropertyValue(context.energySystem, "IsLowEnergyWarningActive"), Is.EqualTo(true));

        Assert.That(GetPrivateFieldValue(context.director, "_tillObjectiveCompleted"), Is.EqualTo(true));
        Assert.That(GetPrivateFieldValue(context.director, "_plantObjectiveCompleted"), Is.EqualTo(true));
        Assert.That(GetPrivateFieldValue(context.director, "_waterObjectiveCompleted"), Is.EqualTo(true));
        Assert.That(GetPrivateFieldValue(context.director, "_woodObjectiveCompleted"), Is.EqualTo(false));
        Assert.That(GetPrivateFieldValue(context.director, "_collectedWoodSinceWoodStepStart"), Is.EqualTo(3));
        Assert.That(GetPrivateFieldValue(context.director, "_craftedCount"), Is.EqualTo(2));
        Assert.That(GetPrivateFieldValue(context.director, "_staminaRevealed"), Is.EqualTo(true));

        Assert.That(GetPropertyValue(chatSessionService, "HasActiveConversation"), Is.EqualTo(false));
        Assert.That(GetPropertyValue(chatSessionService, "HasPendingResumeConversation"), Is.EqualTo(false));
        Assert.That(GetPropertyValue(chatSessionService, "PendingResumeNpcId"), Is.EqualTo(string.Empty));
        Assert.That(GetPrivateFieldValue(nearbyFeedbackService, "activeNearbyBubblePresenter"), Is.Null);
    }

    [Test]
    public void Load_DoesNotPromoteLateDayPrivateFlagsFromPhaseAlone()
    {
        RuntimeContext context = CreateRuntimeContext();

        InvokeInstance(
            context.storyManager,
            "ResetState",
            ParseEnum(context.storyPhaseType, "FreeTime"),
            true);
        InvokeInstance(context.dialogueManager, "ReplaceCompletedSequenceIds", (object)Array.Empty<string>());

        SetPrivateField(context.director, "_dinnerSequencePlayed", false);
        SetPrivateField(context.director, "_returnSequencePlayed", false);
        SetPrivateField(context.director, "_freeTimeEntered", false);
        SetPrivateField(context.director, "_freeTimeIntroCompleted", false);
        SetPrivateField(context.director, "_freeTimeIntroQueued", false);
        SetPrivateField(context.director, "_dayEnded", false);

        object savedData = InvokeInstance(context.persistenceService, "Save");
        Assert.That(savedData, Is.Not.Null);

        InvokeInstance(
            context.storyManager,
            "ResetState",
            ParseEnum(context.storyPhaseType, "DayEnd"),
            true);
        InvokeInstance(
            context.dialogueManager,
            "ReplaceCompletedSequenceIds",
            (object)new[] { "spring-day1-dinner", "spring-day1-reminder", "spring-day1-free-time-opening" });
        SetPrivateField(context.director, "_dinnerSequencePlayed", true);
        SetPrivateField(context.director, "_returnSequencePlayed", true);
        SetPrivateField(context.director, "_freeTimeEntered", true);
        SetPrivateField(context.director, "_freeTimeIntroCompleted", true);
        SetPrivateField(context.director, "_freeTimeIntroQueued", true);
        SetPrivateField(context.director, "_dayEnded", true);

        InvokeInstance(context.persistenceService, "Load", savedData);

        Assert.That(
            GetPropertyValue(context.storyManager, "CurrentPhase"),
            Is.EqualTo(ParseEnum(context.storyPhaseType, "FreeTime")));
        Assert.That(InvokeInstance(context.dialogueManager, "HasCompletedSequence", "spring-day1-dinner"), Is.EqualTo(false));
        Assert.That(InvokeInstance(context.dialogueManager, "HasCompletedSequence", "spring-day1-reminder"), Is.EqualTo(false));
        Assert.That(InvokeInstance(context.dialogueManager, "HasCompletedSequence", "spring-day1-free-time-opening"), Is.EqualTo(false));
        Assert.That(GetPrivateFieldValue(context.director, "_dinnerSequencePlayed"), Is.EqualTo(false));
        Assert.That(GetPrivateFieldValue(context.director, "_returnSequencePlayed"), Is.EqualTo(false));
        Assert.That(GetPrivateFieldValue(context.director, "_freeTimeIntroCompleted"), Is.EqualTo(false));
        Assert.That(GetPrivateFieldValue(context.director, "_freeTimeIntroQueued"), Is.EqualTo(false));
    }

    [Test]
    public void Load_ShouldExpireDayEndTaskCardImmediatelyAfterRestore()
    {
        RuntimeContext context = CreateRuntimeContext();

        InvokeInstance(
            context.storyManager,
            "ResetState",
            ParseEnum(context.storyPhaseType, "DayEnd"),
            true);
        InvokeInstance(context.dialogueManager, "ReplaceCompletedSequenceIds", (object)new[] { "spring-day1-dinner", "spring-day1-reminder", "spring-day1-free-time-opening" });
        SetPrivateField(context.director, "_dinnerSequencePlayed", true);
        SetPrivateField(context.director, "_returnSequencePlayed", true);
        SetPrivateField(context.director, "_freeTimeEntered", true);
        SetPrivateField(context.director, "_freeTimeIntroCompleted", true);
        SetPrivateField(context.director, "_freeTimeIntroQueued", true);
        SetPrivateField(context.director, "_dayEnded", true);
        SetPrivateField(context.director, "_dayEndTaskCardAutoHideArmed", true);
        SetPrivateField(context.director, "_dayEndTaskCardShownAt", Time.unscaledTime);

        object savedData = InvokeInstance(context.persistenceService, "Save");
        Assert.That(savedData, Is.Not.Null);

        InvokeInstance(
            context.storyManager,
            "ResetState",
            ParseEnum(context.storyPhaseType, "FreeTime"),
            false);
        SetPrivateField(context.director, "_dayEnded", false);
        SetPrivateField(context.director, "_dayEndTaskCardAutoHideArmed", false);
        SetPrivateField(context.director, "_dayEndTaskCardShownAt", -1f);

        InvokeInstance(context.persistenceService, "Load", savedData);

        Assert.That(GetPropertyValue(context.storyManager, "CurrentPhase"), Is.EqualTo(ParseEnum(context.storyPhaseType, "DayEnd")));
        Assert.That(GetPrivateFieldValue(context.director, "_dayEnded"), Is.EqualTo(true));
        Assert.That(GetPrivateFieldValue(context.director, "_dayEndTaskCardAutoHideArmed"), Is.EqualTo(true));
        Assert.That((float)GetPrivateFieldValue(context.director, "_dayEndTaskCardShownAt"), Is.LessThan(Time.unscaledTime));
        Assert.That(InvokeInstance(context.director, "BuildPromptCardModel"), Is.Null, "读档恢复 DayEnd 后，日终任务卡应立即过期，不应继续常驻左侧。");
    }

    [Test]
    public void Load_DoesNotPromoteOpeningAndMiddayPrivateFlagsFromPhaseAlone()
    {
        RuntimeContext context = CreateRuntimeContext();

        InvokeInstance(
            context.storyManager,
            "ResetState",
            ParseEnum(context.storyPhaseType, "FarmingTutorial"),
            true);
        InvokeInstance(context.dialogueManager, "ReplaceCompletedSequenceIds", (object)Array.Empty<string>());
        SetPrivateField(context.director, "_villageGateSequencePlayed", false);
        SetPrivateField(context.director, "_houseArrivalSequencePlayed", false);
        SetPrivateField(context.director, "_healingSequencePlayed", false);
        SetPrivateField(context.director, "_workbenchSequencePlayed", false);

        object savedData = InvokeInstance(context.persistenceService, "Save");
        Assert.That(savedData, Is.Not.Null);

        InvokeInstance(
            context.dialogueManager,
            "ReplaceCompletedSequenceIds",
            (object)new[]
            {
                "spring-day1-village-gate",
                "spring-day1-house-arrival",
                "spring-day1-healing",
                "spring-day1-workbench"
            });
        SetPrivateField(context.director, "_villageGateSequencePlayed", true);
        SetPrivateField(context.director, "_houseArrivalSequencePlayed", true);
        SetPrivateField(context.director, "_healingSequencePlayed", true);
        SetPrivateField(context.director, "_workbenchSequencePlayed", true);

        InvokeInstance(context.persistenceService, "Load", savedData);

        Assert.That(
            GetPropertyValue(context.storyManager, "CurrentPhase"),
            Is.EqualTo(ParseEnum(context.storyPhaseType, "FarmingTutorial")));
        Assert.That(InvokeInstance(context.dialogueManager, "HasCompletedSequence", "spring-day1-village-gate"), Is.EqualTo(false));
        Assert.That(InvokeInstance(context.dialogueManager, "HasCompletedSequence", "spring-day1-house-arrival"), Is.EqualTo(false));
        Assert.That(InvokeInstance(context.dialogueManager, "HasCompletedSequence", "spring-day1-healing"), Is.EqualTo(false));
        Assert.That(InvokeInstance(context.dialogueManager, "HasCompletedSequence", "spring-day1-workbench"), Is.EqualTo(false));
        Assert.That(GetPrivateFieldValue(context.director, "_villageGateSequencePlayed"), Is.EqualTo(false));
        Assert.That(GetPrivateFieldValue(context.director, "_houseArrivalSequencePlayed"), Is.EqualTo(false));
        Assert.That(GetPrivateFieldValue(context.director, "_healingSequencePlayed"), Is.EqualTo(false));
        Assert.That(GetPrivateFieldValue(context.director, "_workbenchSequencePlayed"), Is.EqualTo(false));
    }

    private RuntimeContext CreateRuntimeContext()
    {
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type dialogueManagerType = ResolveTypeOrFail("Sunset.Story.DialogueManager");
        Type directorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type persistenceServiceType = ResolveTypeOrFail("Sunset.Story.StoryProgressPersistenceService");
        Type healthSystemType = ResolveTypeOrFail("HealthSystem");
        Type energySystemType = ResolveTypeOrFail("EnergySystem");
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Type playerMovementType = ResolveTypeOrFail("PlayerMovement");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type relationshipServiceType = ResolveTypeOrFail("PlayerNpcRelationshipService");
        Type relationshipStageType = ResolveTypeOrFail("NPCRelationshipStage");

        Component storyManager = Track(new GameObject("StoryManager")).AddComponent(storyManagerType);
        Component dialogueManager = Track(new GameObject("DialogueManager")).AddComponent(dialogueManagerType);
        Component director = Track(new GameObject("SpringDay1Director")).AddComponent(directorType);
        Component persistenceService = Track(new GameObject("StoryProgressPersistenceService")).AddComponent(persistenceServiceType);
        Component healthSystem = Track(new GameObject("HealthSystem")).AddComponent(healthSystemType);
        Component energySystem = Track(new GameObject("EnergySystem")).AddComponent(energySystemType);
        Component timeManager = Track(new GameObject("TimeManager")).AddComponent(timeManagerType);
        TrySetStaticField(timeManagerType, "instance", timeManager);

        GameObject playerObject = Track(new GameObject("PlayerMovement"));
        playerObject.AddComponent<Rigidbody2D>();
        playerObject.AddComponent<BoxCollider2D>();
        Component playerMovement = playerObject.AddComponent(playerMovementType);
        SetPrivateField(director, "_playerMovement", playerMovement);

        return new RuntimeContext(
            storyManager,
            dialogueManager,
            director,
            persistenceService,
            healthSystem,
            energySystem,
            storyPhaseType,
            relationshipServiceType,
            relationshipStageType);
    }

    private static bool InvokeCanSaveNow(Type persistenceServiceType, out string blockerReason)
    {
        MethodInfo method = persistenceServiceType.GetMethod("CanSaveNow", BindingFlags.Static | BindingFlags.Public);
        Assert.That(method, Is.Not.Null, "未找到 StoryProgressPersistenceService.CanSaveNow");

        object[] arguments = { null };
        bool result = (bool)method.Invoke(null, arguments);
        blockerReason = arguments[0] as string ?? string.Empty;
        return result;
    }

    private static bool InvokeCanLoadNow(Type persistenceServiceType, out string blockerReason)
    {
        MethodInfo method = persistenceServiceType.GetMethod("CanLoadNow", BindingFlags.Static | BindingFlags.Public);
        Assert.That(method, Is.Not.Null, "未找到 StoryProgressPersistenceService.CanLoadNow");

        object[] arguments = { null };
        bool result = (bool)method.Invoke(null, arguments);
        blockerReason = arguments[0] as string ?? string.Empty;
        return result;
    }

    private static Type ResolveTypeOrFail(string fullName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(fullName);
            if (type != null)
            {
                return type;
            }
        }

        Assert.Fail($"未找到类型: {fullName}");
        return null;
    }

    private GameObject Track(GameObject gameObject)
    {
        createdObjects.Add(gameObject);
        return gameObject;
    }

    private T Track<T>(T component) where T : Component
    {
        createdObjects.Add(component.gameObject);
        return component;
    }

    private static object ParseEnum(Type enumType, string name)
    {
        return Enum.Parse(enumType, name);
    }

    private static object ParseNestedEnum(object target, string nestedTypeName, string memberName)
    {
        Type nestedType = target.GetType().GetNestedType(nestedTypeName, BindingFlags.NonPublic);
        Assert.That(nestedType, Is.Not.Null, $"未找到嵌套枚举: {target.GetType().Name}.{nestedTypeName}");
        return Enum.Parse(nestedType, memberName);
    }

    private static object InvokeInstance(object target, string methodName, params object[] arguments)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return method.Invoke(target, arguments);
    }

    private static object InvokeStatic(Type type, string methodName, params object[] arguments)
    {
        MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到静态方法: {type.Name}.{methodName}");
        return method.Invoke(null, arguments);
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

    private static void SetFieldValue(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private static void SetAutoPropertyBackingField(object target, string propertyName, object value)
    {
        SetPrivateField(target, $"<{propertyName}>k__BackingField", value);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private static object GetPrivateFieldValue(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        return field.GetValue(target);
    }

    private static object CreateWorkbenchRuntimeState(
        Type workbenchStateType,
        Type queueEntryType,
        string stationKey,
        string sceneName,
        string scenePath,
        int recipeId,
        int resultItemId,
        int totalCount,
        int readyCount,
        float currentUnitProgress,
        bool hasReservedCurrentCraft,
        bool isActiveEntry)
    {
        object workbenchState = Activator.CreateInstance(workbenchStateType);
        object queueEntry = Activator.CreateInstance(queueEntryType);

        SetFieldValue(queueEntry, "recipeId", recipeId);
        SetFieldValue(queueEntry, "resultItemId", resultItemId);
        SetFieldValue(queueEntry, "resultAmountPerCraft", 1);
        SetFieldValue(queueEntry, "totalCount", totalCount);
        SetFieldValue(queueEntry, "readyCount", readyCount);
        SetFieldValue(queueEntry, "currentUnitProgress", currentUnitProgress);
        SetFieldValue(queueEntry, "hasReservedCurrentCraft", hasReservedCurrentCraft);
        SetFieldValue(queueEntry, "isActiveEntry", isActiveEntry);

        System.Collections.IList queueEntries = Activator.CreateInstance(typeof(List<>).MakeGenericType(queueEntryType)) as System.Collections.IList;
        queueEntries?.Add(queueEntry);

        SetFieldValue(workbenchState, "stationKey", stationKey);
        SetFieldValue(workbenchState, "sceneName", sceneName);
        SetFieldValue(workbenchState, "scenePath", scenePath);
        SetFieldValue(workbenchState, "queueEntries", queueEntries);
        return workbenchState;
    }

    private static bool TryGetWorkbenchRuntimeState(Type persistenceServiceType, string stationKey, out object state)
    {
        MethodInfo method = persistenceServiceType.GetMethod("TryGetWorkbenchRuntimeState", BindingFlags.Static | BindingFlags.Public);
        Assert.That(method, Is.Not.Null, "未找到 StoryProgressPersistenceService.TryGetWorkbenchRuntimeState");

        object[] arguments = { stationKey, null };
        bool result = (bool)method.Invoke(null, arguments);
        state = arguments[1];
        return result;
    }

    private static void TrySetStaticField(Type type, string fieldName, object value)
    {
        FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(null, value);
        }
    }

    private static void ClearStaticCollection(Type type, string fieldName)
    {
        FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        if (field == null)
        {
            return;
        }

        object value = field.GetValue(null);
        if (value == null)
        {
            return;
        }

        MethodInfo clearMethod = value.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
        clearMethod?.Invoke(value, null);
    }

    private readonly struct RuntimeContext
    {
        public RuntimeContext(
            object storyManager,
            object dialogueManager,
            object director,
            object persistenceService,
            object healthSystem,
            object energySystem,
            Type storyPhaseType,
            Type relationshipServiceType,
            Type relationshipStageType)
        {
            this.storyManager = storyManager;
            this.dialogueManager = dialogueManager;
            this.director = director;
            this.persistenceService = persistenceService;
            this.healthSystem = healthSystem;
            this.energySystem = energySystem;
            this.storyPhaseType = storyPhaseType;
            this.relationshipServiceType = relationshipServiceType;
            this.relationshipStageType = relationshipStageType;
        }

        public object storyManager { get; }
        public object dialogueManager { get; }
        public object director { get; }
        public object persistenceService { get; }
        public object healthSystem { get; }
        public object energySystem { get; }
        public Type storyPhaseType { get; }
        public Type relationshipServiceType { get; }
        public Type relationshipStageType { get; }
    }
}
