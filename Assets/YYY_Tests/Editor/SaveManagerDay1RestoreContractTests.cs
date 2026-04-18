using System.IO;
using NUnit.Framework;

[TestFixture]
public class SaveManagerDay1RestoreContractTests
{
    private static readonly string ProjectRoot =
        Directory.GetParent(UnityEngine.Application.dataPath)?.FullName ?? Directory.GetCurrentDirectory();

    private const string SaveManagerPath = "Assets/YYY_Scripts/Data/Core/SaveManager.cs";
    private const string SaveDataDtosPath = "Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs";
    private const string PersistentBridgePath = "Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs";
    private const string StoryProgressPersistencePath = "Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs";
    private const string SpringDay1DirectorPath = "Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs";

    [Test]
    public void RestoreEntry_ShouldResetKnownTransientUiAndPauseSources()
    {
        string text = File.ReadAllText(SaveManagerPath);

        StringAssert.Contains("ResetTransientRuntimeForRestore(\"读档前恢复入口清理\")", text);
        StringAssert.Contains("ResetTransientRuntimeForRestore(\"新开局重建默认运行态前恢复入口清理\")", text);
        StringAssert.Contains("ClosePackageAndBoxUiForRestore();", text);
        StringAssert.Contains("CloseWorkbenchOverlayForRestore();", text);
        StringAssert.Contains("HideTransientOverlayUiForRestore();", text);
        StringAssert.Contains("ResetKnownTimePauseSourcesForRestore();", text);
        StringAssert.Contains("timeManager.ResumeTime(DialoguePauseSource);", text);
        StringAssert.Contains("timeManager.ResumeTime(StoryTimePauseSource);", text);
        StringAssert.Contains("timeManager.SetPaused(false);", text);
        StringAssert.Contains("GameInputManager.ForceResetPlacementRuntime(reason);", text);
        StringAssert.DoesNotContain("CheckPositionNextFrame", text);
    }

    [Test]
    public void LoadEntry_ShouldRespectStoryLoadBlockers()
    {
        string saveManagerText = File.ReadAllText(SaveManagerPath);
        string storyProgressText = File.ReadAllText(StoryProgressPersistencePath);
        string directorText = File.ReadAllText(SpringDay1DirectorPath);

        StringAssert.Contains("CanExecutePlayerLoadAction(out string blockerReason)", saveManagerText);
        StringAssert.Contains("CanLoadStoryProgressNow(out blockerReason)", saveManagerText);
        StringAssert.Contains("TryInvokeStoryProgressPersistenceMethod(\"CanLoadNow\"", saveManagerText,
            "F9 / 普通读档入口都必须先走剧情读档 blocker，而不是任由导演态中途切档。");
        StringAssert.Contains("public static bool CanLoadNow(out string blockerReason)", storyProgressText);
        StringAssert.Contains("director.TryGetStorySaveLoadBlockReason(out string storyBlockReason)", storyProgressText);
        StringAssert.Contains("public bool TryGetStorySaveLoadBlockReason(out string blockerReason)", directorText,
            "Day1 导演层必须自己报出“剧情接管中禁止读档”的权威判断，不能只靠对话框是否已出现。");
    }

    [Test]
    public void LoadAndSaveEntries_ShouldRespectSceneRestoreWindows()
    {
        string saveManagerText = File.ReadAllText(SaveManagerPath);
        string bridgeText = File.ReadAllText(PersistentBridgePath);

        StringAssert.Contains("_nativeFreshRestartInProgress || _sceneSwitchLoadInProgress", saveManagerText);
        StringAssert.Contains("PersistentPlayerSceneBridge.IsSceneWorldRestoreInProgress()", saveManagerText);
        StringAssert.Contains("当前场景仍在恢复世界状态，请稍候再保存。", saveManagerText);
        StringAssert.Contains("当前场景仍在恢复世界状态，请稍候再读取存档。", saveManagerText);
        StringAssert.Contains("public static bool IsSceneWorldRestoreInProgress()", bridgeText);
    }

    [Test]
    public void RestoreEntry_ShouldCloseInventoryDragAndDay1TransientShells()
    {
        string text = File.ReadAllText(SaveManagerPath);

        StringAssert.Contains("InventoryInteractionManager.Instance", text);
        StringAssert.Contains("interactionManager?.Cancel();", text);
        StringAssert.Contains("InventorySlotInteraction.ResetActiveChestHeldState();", text);
        StringAssert.Contains("ItemTooltip.Instance?.Hide();", text);
        StringAssert.Contains("ItemUseConfirmDialog.Instance?.Hide();", text);
        StringAssert.Contains("promptOverlay.SetExternalVisibilityBlock(false);", text);
        StringAssert.Contains("promptOverlay.Hide();", text);
        StringAssert.Contains("director?.HideTaskListBridgePrompt();", text);
        StringAssert.Contains("InteractionHintOverlay.HideIfExists();", text);
        StringAssert.Contains("NpcWorldHintBubble.HideIfExists();", text);
        StringAssert.Contains("SpringDay1WorldHintBubble.HideIfExists();", text);
        StringAssert.Contains("playerThoughtBubbles[index]?.HideImmediate();", text);
        StringAssert.Contains("npcBubbles[index]?.HideImmediateBubble();", text);
        StringAssert.Contains("RestoreHotbarSelectionForLoadedPlayer(data);", text);
    }

    [Test]
    public void CollectFullSaveData_ShouldKeepOffSceneWorldStateOutOfWorldObjects()
    {
        string text = File.ReadAllText(SaveManagerPath);

        StringAssert.Contains("正式存档当前只收“当前已加载 scene 内”的持久对象。", text);
        StringAssert.Contains("不能把 off-scene world state 粗暴并进 worldObjects。", text);
        StringAssert.Contains("saveData.worldObjects = PersistentObjectRegistry.Instance.CollectAllSaveData();", text);
        StringAssert.Contains("saveData.offSceneWorldSnapshots = PersistentPlayerSceneBridge.ExportOffSceneWorldSnapshotsForSave();", text);
        StringAssert.Contains("PersistentPlayerSceneBridge.ImportOffSceneWorldSnapshotsFromSave(saveData.offSceneWorldSnapshots);", text);
    }

    [Test]
    public void SaveDataDto_ShouldDeclareOffSceneWorldSnapshotContainer()
    {
        string dtoText = File.ReadAllText(SaveDataDtosPath);

        StringAssert.Contains("public List<SceneWorldSnapshotSaveData> offSceneWorldSnapshots;", dtoText);
        StringAssert.Contains("offSceneWorldSnapshots = new List<SceneWorldSnapshotSaveData>();", dtoText);
        StringAssert.Contains("public class SceneWorldSnapshotSaveData", dtoText);
    }

    [Test]
    public void PlayerSaveData_ShouldCarryRuntimeSelectionState()
    {
        string dtoText = File.ReadAllText(SaveDataDtosPath);

        StringAssert.Contains("public int selectedHotbarSlot = 0;", dtoText);
        StringAssert.Contains("public int selectedInventoryIndex = 0;", dtoText,
            "正式读档不能只靠 bridge 会话内快照；至少要把当前快捷栏/背包选中态写入存档。");
    }

    [Test]
    public void SaveManager_ShouldPersistAndRestoreHotbarSelectionFromPlayerSaveData()
    {
        string text = File.ReadAllText(SaveManagerPath);

        StringAssert.Contains("data.selectedHotbarSlot = hotbarSelection.selectedIndex;", text);
        StringAssert.Contains("data.selectedInventoryIndex = hotbarSelection.selectedInventoryIndex;", text);
        StringAssert.Contains("RestoreHotbarSelectionForLoadedPlayer(data);", text);
        StringAssert.Contains("hotbarSelection.RestoreSelectionState(restoredHotbarIndex, restoredInventoryIndex);", text,
            "读档后应优先回到存档里的 runtime 选中态，而不是只靠当前会话残留。");
    }

    [Test]
    public void EquipmentService_ShouldJoinPersistentRegistry()
    {
        string equipmentText = File.ReadAllText(Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Equipment/EquipmentService.cs"));

        StringAssert.Contains("private bool _registeredWithPersistentRegistry;", equipmentText);
        StringAssert.Contains("RegisterWithPersistentRegistry();", equipmentText);
        StringAssert.Contains("PersistentPlayerSceneBridge.GetPreferredRuntimeEquipmentService()", equipmentText);
        StringAssert.Contains("PersistentObjectRegistry registry = PersistentObjectRegistry.Instance;", equipmentText);
        StringAssert.Contains("registry.TryRegister(this);", equipmentText);
        StringAssert.Contains("UnregisterFromPersistentRegistry();", equipmentText);
    }

    [Test]
    public void ChestAndDropPayloads_ShouldCarryExtendedRuntimeState()
    {
        string dtoText = File.ReadAllText(SaveDataDtosPath);
        string chestText = File.ReadAllText(Path.Combine(ProjectRoot, "Assets/YYY_Scripts/World/Placeable/ChestController.cs"));
        string dropText = File.ReadAllText(Path.Combine(ProjectRoot, "Assets/YYY_Scripts/World/WorldItemPickup.cs"));

        StringAssert.Contains("public int version = 2;", dtoText);
        StringAssert.Contains("public int origin = -1;", dtoText);
        StringAssert.Contains("public int ownership = -1;", dtoText);
        StringAssert.Contains("public bool hasBeenLocked = false;", dtoText);
        StringAssert.Contains("public int currentHealth = -1;", dtoText);
        StringAssert.Contains("public InventorySlotSaveData runtimeItem;", dtoText);
        StringAssert.Contains("item.RestoreInstanceIdForLoad(data.instanceId);", dtoText);
        StringAssert.Contains("origin = (int)origin,", chestText);
        StringAssert.Contains("ownership = (int)ownership,", chestText);
        StringAssert.Contains("hasBeenLocked = hasBeenLocked,", chestText);
        StringAssert.Contains("currentHealth = currentHealth,", chestText);
        StringAssert.Contains("if (chestData.version >= 2)", chestText);
        StringAssert.Contains("dropData.runtimeItem != null && !dropData.runtimeItem.IsEmpty", dropText);
        StringAssert.Contains("SaveDataHelper.ToSaveData(runtimeItem, 0)", dropText);
        StringAssert.Contains("SaveDataHelper.FromSaveData(dropData.runtimeItem)", dropText);
    }

    [Test]
    public void SceneSwitchLoad_ShouldSuppressBridgeContinuityRestore_ForCurrentLoadTarget()
    {
        string text = File.ReadAllText(SaveManagerPath);

        StringAssert.Contains("PersistentPlayerSceneBridge.SuppressSceneWorldRestoreForScene(targetSceneName);", text);
        StringAssert.Contains("PersistentPlayerSceneBridge.CancelSuppressedSceneWorldRestore(targetSceneName);", text);
    }

    [Test]
    public void NativeFreshRestart_ShouldAlsoSuppressTownRestore_BeforeRestartLoad()
    {
        string text = File.ReadAllText(SaveManagerPath);

        StringAssert.Contains("PersistentPlayerSceneBridge.SuppressSceneWorldRestoreForScene(NativeFreshSceneName);", text,
            "重新开始走 Town 原生开局时，也必须先抑制 Town 的旧 world snapshot。");
        StringAssert.Contains("PersistentPlayerSceneBridge.CancelSuppressedSceneWorldRestore(NativeFreshSceneName);", text,
            "如果原生重开载入 Town 失败，必须取消这次 suppress，避免后续场景 restore 一直被吞。");
    }

    [Test]
    public void PersistentBridge_ShouldCoverChestFarmAndCrop_InWorldStateContinuity()
    {
        string bridgeText = File.ReadAllText(PersistentBridgePath);

        StringAssert.Contains("CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<FarmTileManager>(scene), snapshots, capturedGuids);", bridgeText);
        StringAssert.Contains("CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<CropController>(scene), snapshots, capturedGuids);", bridgeText);
        StringAssert.Contains("CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<ChestController>(scene), snapshots, capturedGuids);", bridgeText);
        StringAssert.Contains("CollectSceneWorldRuntimeBindings(FindComponentsInScene<FarmTileManager>(scene), bindings);", bridgeText);
        StringAssert.Contains("CollectSceneWorldRuntimeBindings(FindComponentsInScene<CropController>(scene), bindings);", bridgeText);
        StringAssert.Contains("CollectSceneWorldRuntimeBindings(FindComponentsInScene<ChestController>(scene), bindings);", bridgeText);
        StringAssert.Contains("case \"FarmTileManager\":", bridgeText);
        StringAssert.Contains("case \"Crop\":", bridgeText);
    }

    [Test]
    public void StoryProgressPersistence_ShouldRestoreCanonicalDialogueAndDay1ProgressBeforeRuntimeTakesOver()
    {
        string storyProgressText = File.ReadAllText(StoryProgressPersistencePath);

        StringAssert.Contains("StoryManager.Instance.ResetState(SanitizeStoryPhase(safeSnapshot.storyPhase), safeSnapshot.isLanguageDecoded);", storyProgressText);
        StringAssert.Contains("dialogueManager.StopDialogue();", storyProgressText,
            "恢复剧情长期态前必须先停掉旧对白，不能让 stale dialogue 壳继续挂在恢复后的 Day1 上。");
        StringAssert.Contains("dialogueManager.ReplaceCompletedSequenceIds(safeSnapshot.completedDialogueSequenceIds);", storyProgressText,
            "恢复后的 completedDialogueSequenceIds 必须以存档真值为准。");
        StringAssert.Contains("ApplySpringDay1Progress(", storyProgressText);
        StringAssert.Contains("BuildCompletedSequenceSet(safeSnapshot.completedDialogueSequenceIds)", storyProgressText,
            "Day1 恢复应显式把已消费对话集合交给 ApplySpringDay1Progress，而不是让 director 自己再按 phase 猜。");
        StringAssert.Contains("ApplySpringDay1ResidentRuntime(safeSnapshot.springDay1);", storyProgressText);
        StringAssert.Contains("ResetNpcTransientState();", storyProgressText,
            "剧情长期态恢复完后还必须清掉 NPC 闲聊/提示这类瞬时壳。");
    }

    [Test]
    public void StoryProgressPersistence_ShouldProjectCompletedSequencesIntoDay1RuntimeFlags()
    {
        string storyProgressText = File.ReadAllText(StoryProgressPersistencePath);

        StringAssert.Contains("bool hasVillageGateSequence = completedSequenceIds.Contains(SpringDay1VillageGateSequenceId);", storyProgressText);
        StringAssert.Contains("bool hasHouseArrivalSequence = completedSequenceIds.Contains(SpringDay1HouseArrivalSequenceId);", storyProgressText);
        StringAssert.Contains("bool hasHealingSequence = completedSequenceIds.Contains(SpringDay1HealingSequenceId);", storyProgressText);
        StringAssert.Contains("bool hasWorkbenchSequence = completedSequenceIds.Contains(SpringDay1WorkbenchSequenceId);", storyProgressText);
        StringAssert.Contains("bool hasDinnerSequence = completedSequenceIds.Contains(SpringDay1DinnerSequenceId);", storyProgressText);
        StringAssert.Contains("bool hasReminderSequence = completedSequenceIds.Contains(SpringDay1ReminderSequenceId);", storyProgressText);
        StringAssert.Contains("WritePrivateFieldValue(director, DirectorFieldCache, \"_villageGateSequencePlayed\", DirectorMemberFlags, hasVillageGateSequence);", storyProgressText);
        StringAssert.Contains("WritePrivateFieldValue(director, DirectorFieldCache, \"_returnSequencePlayed\", DirectorMemberFlags, hasReminderSequence);", storyProgressText);
        StringAssert.Contains("director.HideTaskListBridgePrompt();", storyProgressText);
        StringAssert.Contains("RestoreLoadedDayEndTaskCardState", storyProgressText,
            "如果存档恢复到 DayEnd，导演层还必须把日终任务卡重置为已过期态，不能让左侧任务页在第二天继续常驻。");
        StringAssert.Contains("InvokePrivateMethod(director, DirectorMethodCache, \"SyncStoryTimePauseState\", DirectorMemberFlags);", storyProgressText,
            "ApplySpringDay1Progress 最终还必须让 day1 runtime 自己重建 pause / prompt 语义。");
    }

    [Test]
    public void SpringDay1Director_ShouldRecoverFromCompletedSequencesWithoutPhaseAutocomplete()
    {
        string directorText = File.ReadAllText(SpringDay1DirectorPath);

        StringAssert.Contains("return manager.HasCompletedSequence(sequenceId);", directorText,
            "HasCompletedDialogueSequence 不应再按 phase 自动补 completed dialogue。");
        StringAssert.Contains("if (HasCompletedDialogueSequence(HealingSequenceId))", directorText);
        StringAssert.Contains("StoryManager.Instance.SetPhase(StoryPhase.WorkbenchFlashback);", directorText);
        StringAssert.Contains("if (HasCompletedDialogueSequence(WorkbenchSequenceId))", directorText);
        StringAssert.Contains("StoryManager.Instance.SetPhase(StoryPhase.FarmingTutorial);", directorText);
        StringAssert.Contains("if (HasCompletedDialogueSequence(DinnerSequenceId))", directorText);
        StringAssert.Contains("BeginReturnReminder();", directorText);
        StringAssert.Contains("if (HasCompletedDialogueSequence(ReminderSequenceId))", directorText);
        StringAssert.Contains("EnterFreeTime();", directorText);
        StringAssert.Contains("if (!_freeTimeIntroCompleted && HasCompletedDialogueSequence(FreeTimeIntroSequenceId))", directorText);
        StringAssert.Contains("CompleteFreeTimeIntro();", directorText);
        StringAssert.Contains("ShouldFallbackToDayEndWhenStoryPhaseUnavailable", directorText,
            "Day1 任务卡 fallback 不能再只靠 _dayEnded 单独续命；StoryPhase 暂空时也必须受日终卡有效窗口约束。");
    }

    [Test]
    public void SpringDay1Director_ShouldRouteSceneLoadsThroughPersistentBridge()
    {
        string directorText = File.ReadAllText(SpringDay1DirectorPath);

        StringAssert.Contains("PersistentPlayerSceneBridge.QueueSceneEntry(sceneName, string.Empty, string.Empty);", directorText,
            "Day1 runtime 切场前必须先交给 persistent bridge 记住 entry contract。");
        StringAssert.Contains("SceneManager.LoadScene(sceneName, LoadSceneMode.Single);", directorText);
    }

    [Test]
    public void SpringDay1WorkbenchEscort_ShouldFallbackToWorkbenchArea_WhenChiefDistanceLags()
    {
        string directorText = File.ReadAllText(SpringDay1DirectorPath);

        StringAssert.Contains("TryGetPlayerDistanceToEscortTarget(null, workbenchTarget, out float workbenchPlayerDistance)", directorText,
            "0.0.4 工作台桥接不能只按村长距离判断玩家是否到位；玩家已经到工作台边也应能接起 briefing。");
        StringAssert.Contains("bool canForceWorkbenchBriefing = playerNearWorkbench && !HasWorkbenchBriefingCompleted();", directorText,
            "玩家已经进入工作台触发范围时，导演层应允许强制接起 workbench briefing，避免 001/002 摆位略偏时剧情卡死。");
    }
}
