using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FarmGame.Combat;
using FarmGame.Data;
using FarmGame.Data.Core;
using FarmGame.Farm;
using FarmGame.Utils;
using TMPro;
using UnityEngine;

/// <summary>
/// 1.0.4 / 008 农田工具运行时与反馈链 live 验收 Runner。
/// 只在 Play Mode 中使用，只负责采集 4 组场景证据，不承载业务逻辑。
/// </summary>
public class FarmRuntimeLiveValidationRunner : MonoBehaviour
{
    private enum ValidationScope
    {
        All,
        HoverOnly
    }

    private const string LogPrefix = "[FarmRuntimeLive]";
    private const int HoeSlot = 0;
    private const int WaterSlot = 1;
    private const int LowAxeSlot = 2;
    private const int AltLowAxeSlot = 3;
    private const int HighAxeSlot = 4;
    private const float ScenarioPollInterval = 0.05f;
    private const float BubbleNoRefreshProbeDelay = 4.6f;
    private const float BubbleNoRefreshTailProbeDelay = 1.1f;
    private const float RecoveredBubbleProbeDelay = 3.2f;

    private static readonly MethodInfo ExecuteTillSoilMethod =
        typeof(GameInputManager).GetMethod("ExecuteTillSoil", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo ExecuteWaterTileMethod =
        typeof(GameInputManager).GetMethod("ExecuteWaterTile", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo PlacementModeSetter =
        typeof(GameInputManager).GetProperty("IsPlacementMode", BindingFlags.Instance | BindingFlags.Public)?.GetSetMethod(true);

    private static readonly FieldInfo BubbleTextField =
        typeof(PlayerThoughtBubblePresenter).GetField("bubbleText", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo IsOccludingField =
        typeof(OcclusionTransparency).GetField("isOccluding", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo CooldownUntilField =
        typeof(TreeController).GetField("s_insufficientAxeCooldownUntil", BindingFlags.Static | BindingFlags.NonPublic);

    private static readonly FieldInfo RegisteredOccludersField =
        typeof(OcclusionManager).GetField("registeredOccluders", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo OccludableTagsField =
        typeof(OcclusionManager).GetField("occludableTags", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo PreviewBoundsField =
        typeof(OcclusionManager).GetField("previewBounds", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo PreviewOccludingField =
        typeof(OcclusionManager).GetField("previewOccluding", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo ToolUseCommittedField =
        typeof(PlayerInteraction).GetField("toolUseCommittedForCurrentAction", BindingFlags.Instance | BindingFlags.NonPublic);

    private static Sprite sSolidSprite;
    private static Texture2D sSolidTexture;

    private readonly List<UnityEngine.Object> spawnedArtifacts = new();
    private readonly List<string> scenarioLogs = new();

    private Coroutine runCoroutine;
    private ValidationScope validationScope = ValidationScope.All;

    private GameInputManager gameInputManager;
    private InventoryService inventoryService;
    private HotbarSelectionService hotbarSelection;
    private ItemDatabase database;
    private EnergySystem energySystem;
    private PlayerInteraction playerInteraction;
    private PlayerToolController playerToolController;
    private Transform playerTransform;
    private GameObject playerRoot;
    private SpriteRenderer playerSpriteRenderer;
    private FarmTileManager farmTileManager;
    private FarmToolPreview farmToolPreview;
    private OcclusionManager occlusionManager;
    private PlayerToolFeedbackService playerFeedbackService;
    private PlayerThoughtBubblePresenter bubblePresenter;
    private Camera mainCamera;

    private ToolData hoeTool;
    private ToolData wateringTool;
    private ToolData lowAxeTool;
    private ToolData altLowAxeTool;
    private ToolData highAxeTool;

    private FarmCellCandidate hoeCandidateA;
    private FarmCellCandidate hoeCandidateB;
    private bool hasHoeCandidates;

    private struct FarmCellCandidate
    {
        public int LayerIndex;
        public Vector3Int CellPos;
        public Vector3 WorldPosition;
    }

    public static FarmRuntimeLiveValidationRunner BeginOrRestart()
    {
        return BeginOrRestart(ValidationScope.All);
    }

    public static FarmRuntimeLiveValidationRunner BeginHoverOnly()
    {
        return BeginOrRestart(ValidationScope.HoverOnly);
    }

    private static FarmRuntimeLiveValidationRunner BeginOrRestart(ValidationScope scope)
    {
        FarmRuntimeLiveValidationRunner runner = FindFirstObjectByType<FarmRuntimeLiveValidationRunner>();
        if (runner == null)
        {
            GameObject go = new GameObject("FarmRuntimeLiveValidationRunner");
            runner = go.AddComponent<FarmRuntimeLiveValidationRunner>();
        }

        runner.StartRun(scope);
        return runner;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void StartRun(ValidationScope scope)
    {
        if (runCoroutine != null)
        {
            StopCoroutine(runCoroutine);
        }

        validationScope = scope;
        runCoroutine = StartCoroutine(RunAllScenarios());
    }

    private IEnumerator RunAllScenarios()
    {
        scenarioLogs.Clear();
        CleanupArtifacts();

        if (!ResolveReferences(out string reason))
        {
            Debug.LogError($"{LogPrefix} resolve_failed=true details={reason}");
            runCoroutine = null;
            yield break;
        }

        SetPlacementMode(true);
        HideBubbleImmediate();

        Debug.Log($"{LogPrefix} runner_started scene={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");

        if (validationScope == ValidationScope.All)
        {
            yield return RunHoeRuntimeScenario();
            yield return RunWaterRuntimeScenario();
            yield return RunTreeFeedbackScenario();
        }

        yield return RunHoverOcclusionScenario();

        bool allPassed = true;
        foreach (string log in scenarioLogs)
        {
            Debug.Log(log);
            allPassed &= log.IndexOf("passed=true", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        HideBubbleImmediate();
        farmToolPreview?.Hide();
        SetPlacementMode(false);

        Debug.Log($"{LogPrefix} all_completed=true all_passed={allPassed} scenario_count={scenarioLogs.Count}");
        runCoroutine = null;
    }

    private IEnumerator RunHoeRuntimeScenario()
    {
        const string scenarioName = "hoe-runtime-chain";

        if (!TryFindTillableCells(2, out List<FarmCellCandidate> candidates, out string reason))
        {
            LogScenario(scenarioName, false, $"candidate_search_failed=true details={reason}");
            yield break;
        }

        hoeCandidateA = candidates[0];
        hoeCandidateB = candidates[1];
        hasHoeCandidates = true;

        InventoryItem hoeItem = ToolRuntimeUtility.CreateRuntimeItem(hoeTool, 0, 1);
        hoeItem.SetDurability(2, 2);
        yield return EquipToolSlot(HoeSlot, hoeTool, hoeItem);

        int energyBeforeFirst = energySystem.CurrentEnergy;
        ResetPlayerToolCommitGate();
        bool firstExecute = InvokeExecuteTillSoil(hoeCandidateA.LayerIndex, hoeCandidateA.CellPos);
        yield return WaitFrames(2);

        InventoryItem firstRuntime = inventoryService.GetInventoryItem(HoeSlot);
        FarmTileData firstTile = farmTileManager.GetTileData(hoeCandidateA.LayerIndex, hoeCandidateA.CellPos);
        bool firstTileCreated = firstTile != null && firstTile.isTilled;
        bool firstEnergyConsumed = energySystem.CurrentEnergy == energyBeforeFirst - hoeTool.energyCost;
        bool firstDurabilityConsumed = firstRuntime != null && firstRuntime.CurrentDurability == 1;

        HideBubbleImmediate();
        int feedbackSoundBeforeBreak = playerFeedbackService != null ? playerFeedbackService.FeedbackSoundDispatchCount : 0;
        int burstBeforeBreak = CountFeedbackBursts();
        int energyBeforeBreak = energySystem.CurrentEnergy;

        ResetPlayerToolCommitGate();
        bool secondExecute = InvokeExecuteTillSoil(hoeCandidateB.LayerIndex, hoeCandidateB.CellPos);
        yield return WaitFrames(3);

        FarmTileData secondTile = farmTileManager.GetTileData(hoeCandidateB.LayerIndex, hoeCandidateB.CellPos);
        bool secondTileCreated = secondTile != null && secondTile.isTilled;
        bool secondEnergyConsumed = energySystem.CurrentEnergy == energyBeforeBreak - hoeTool.energyCost;
        bool slotCleared = inventoryService.GetSlot(HoeSlot).IsEmpty && inventoryService.GetInventoryItem(HoeSlot) == null;
        bool breakBubbleVisible = bubblePresenter != null && bubblePresenter.IsVisible;
        string breakBubbleText = GetBubbleText();
        bool breakBurstTriggered = CountFeedbackBursts() > burstBeforeBreak;
        bool breakAudioTriggered = playerFeedbackService != null &&
                                  playerFeedbackService.FeedbackSoundDispatchCount > feedbackSoundBeforeBreak;
        bool breakBubbleMatched = IsToolBrokenLine(breakBubbleText);

        bool passed =
            firstExecute &&
            firstTileCreated &&
            firstEnergyConsumed &&
            firstDurabilityConsumed &&
            secondExecute &&
            secondTileCreated &&
            secondEnergyConsumed &&
            slotCleared &&
            breakBubbleVisible &&
            breakBubbleMatched &&
            breakBurstTriggered &&
            breakAudioTriggered;

        LogScenario(
            scenarioName,
            passed,
            $"firstExecute={firstExecute} firstTileCreated={firstTileCreated} firstEnergyConsumed={firstEnergyConsumed} firstDurabilityConsumed={firstDurabilityConsumed} " +
            $"secondExecute={secondExecute} secondTileCreated={secondTileCreated} secondEnergyConsumed={secondEnergyConsumed} slotCleared={slotCleared} " +
            $"breakBubbleVisible={breakBubbleVisible} breakBubbleMatched={breakBubbleMatched} breakBurstTriggered={breakBurstTriggered} breakAudioTriggered={breakAudioTriggered} " +
            $"breakBubbleText=\"{SanitizeLogValue(breakBubbleText)}\"");
    }

    private IEnumerator RunWaterRuntimeScenario()
    {
        const string scenarioName = "watering-runtime-chain";

        if (!hasHoeCandidates)
        {
            if (!TryFindTillableCells(2, out List<FarmCellCandidate> fallbackCells, out string findReason))
            {
                LogScenario(scenarioName, false, $"setup_failed=true details={findReason}");
                yield break;
            }

            hoeCandidateA = fallbackCells[0];
            hoeCandidateB = fallbackCells[1];
            hasHoeCandidates = true;
        }

        EnsureTilledCell(hoeCandidateA);
        EnsureTilledCell(hoeCandidateB);
        ResetWateredState(hoeCandidateA);
        ResetWateredState(hoeCandidateB);

        int maxWater = ToolRuntimeUtility.GetWaterCapacity(wateringTool);
        int waterUseCost = ToolRuntimeUtility.GetWaterUseCost(wateringTool);
        InventoryItem wateringItem = ToolRuntimeUtility.CreateRuntimeItem(wateringTool, 0, 1);
        wateringItem.SetProperty(ToolRuntimeUtility.WaterMaxPropertyKey, maxWater);
        wateringItem.SetProperty(ToolRuntimeUtility.WaterCurrentPropertyKey, waterUseCost);
        yield return EquipToolSlot(WaterSlot, wateringTool, wateringItem);

        int energyBeforeSuccess = energySystem.CurrentEnergy;
        ResetPlayerToolCommitGate();
        bool waterExecute = InvokeExecuteWaterTile(hoeCandidateA.LayerIndex, hoeCandidateA.CellPos, 0);
        yield return WaitFrames(2);

        InventoryItem wateredItem = inventoryService.GetInventoryItem(WaterSlot);
        FarmTileData wateredTile = farmTileManager.GetTileData(hoeCandidateA.LayerIndex, hoeCandidateA.CellPos);
        int remainingWater = wateredItem != null ? ToolRuntimeUtility.GetCurrentWater(wateredItem, wateringTool) : -1;
        bool waterStateApplied = wateredTile != null && wateredTile.wateredToday;
        bool waterConsumed = remainingWater == 0;
        bool energyConsumed = energySystem.CurrentEnergy == energyBeforeSuccess - wateringTool.energyCost;
        string tooltipText = ItemTooltipTextBuilder.Build(wateringTool, wateredItem);
        bool tooltipShowsWater = tooltipText.Contains($"当前水量: {remainingWater}/{maxWater}");

        HideBubbleImmediate();
        ResetWateredState(hoeCandidateB);
        int energyBeforeEmpty = energySystem.CurrentEnergy;
        int feedbackSoundBeforeEmpty = playerFeedbackService != null ? playerFeedbackService.FeedbackSoundDispatchCount : 0;
        int burstBeforeEmpty = CountFeedbackBursts();

        ResetPlayerToolCommitGate();
        bool emptyExecute = InvokeExecuteWaterTile(hoeCandidateB.LayerIndex, hoeCandidateB.CellPos, 1);
        yield return WaitFrames(3);

        FarmTileData emptyTile = farmTileManager.GetTileData(hoeCandidateB.LayerIndex, hoeCandidateB.CellPos);
        bool emptyPrevented = !emptyExecute;
        bool emptyDidNotWater = emptyTile != null && !emptyTile.wateredToday;
        bool emptyDidNotConsumeEnergy = energySystem.CurrentEnergy == energyBeforeEmpty;
        bool emptyBubbleVisible = bubblePresenter != null && bubblePresenter.IsVisible;
        string emptyBubbleText = GetBubbleText();
        bool emptyBubbleMatched = IsWaterEmptyLine(emptyBubbleText);
        bool emptyBurstTriggered = CountFeedbackBursts() > burstBeforeEmpty;
        bool emptyAudioTriggered = playerFeedbackService != null &&
                                  playerFeedbackService.FeedbackSoundDispatchCount > feedbackSoundBeforeEmpty;

        bool passed =
            waterExecute &&
            waterStateApplied &&
            waterConsumed &&
            energyConsumed &&
            tooltipShowsWater &&
            emptyPrevented &&
            emptyDidNotWater &&
            emptyDidNotConsumeEnergy &&
            emptyBubbleVisible &&
            emptyBubbleMatched &&
            emptyBurstTriggered &&
            emptyAudioTriggered;

        LogScenario(
            scenarioName,
            passed,
            $"waterExecute={waterExecute} waterStateApplied={waterStateApplied} waterConsumed={waterConsumed} energyConsumed={energyConsumed} tooltipShowsWater={tooltipShowsWater} " +
            $"emptyPrevented={emptyPrevented} emptyDidNotWater={emptyDidNotWater} emptyDidNotConsumeEnergy={emptyDidNotConsumeEnergy} emptyBubbleVisible={emptyBubbleVisible} " +
            $"emptyBubbleMatched={emptyBubbleMatched} emptyBurstTriggered={emptyBurstTriggered} emptyAudioTriggered={emptyAudioTriggered} " +
            $"remainingWater={remainingWater}/{maxWater} tooltip=\"{SanitizeLogValue(tooltipText)}\" emptyBubbleText=\"{SanitizeLogValue(emptyBubbleText)}\"");
    }

    private IEnumerator RunTreeFeedbackScenario()
    {
        const string scenarioName = "high-tree-feedback-chain";

        TreeController firstTree = CreateScenarioTree("FarmRuntimeTreeProbe_A", playerTransform.position + new Vector3(2.8f, 0f, 0f));
        TreeController secondTree = CreateScenarioTree("FarmRuntimeTreeProbe_B", playerTransform.position + new Vector3(4.6f, 0f, 0f));

        if (firstTree == null || secondTree == null)
        {
            LogScenario(scenarioName, false, "tree_clone_failed=true");
            yield break;
        }

        yield return WaitFrames(3);
        SetTreeCooldown(-1f);
        HideBubbleImmediate();

        int firstTreeHealth = firstTree.GetCurrentHealth();
        int energyBeforeFirstFail = energySystem.CurrentEnergy;
        yield return EquipToolSlot(LowAxeSlot, lowAxeTool, ToolRuntimeUtility.CreateRuntimeItem(lowAxeTool, 0, 1));
        ResetPlayerToolCommitGate();
        firstTree.OnHit(CreateToolHitContext(lowAxeTool, firstTree));
        yield return WaitFrames(3);

        float cooldownAfterFirstFail = GetTreeCooldown();
        bool firstFailNoDamage = firstTree.GetCurrentHealth() == firstTreeHealth;
        bool firstFailNoEnergy = energySystem.CurrentEnergy == energyBeforeFirstFail;
        bool firstFailBubbleVisible = bubblePresenter != null && bubblePresenter.IsVisible;
        string firstFailBubbleText = GetBubbleText();
        bool firstFailBubbleMatched = IsAxeFailLine(firstFailBubbleText);
        bool cooldownStarted = cooldownAfterFirstFail > Time.time + 25f;

        yield return new WaitForSeconds(BubbleNoRefreshProbeDelay);

        int energyBeforeSecondFail = energySystem.CurrentEnergy;
        yield return EquipToolSlot(AltLowAxeSlot, altLowAxeTool, ToolRuntimeUtility.CreateRuntimeItem(altLowAxeTool, 0, 1));
        ResetPlayerToolCommitGate();
        firstTree.OnHit(CreateToolHitContext(altLowAxeTool, firstTree));
        yield return WaitFrames(2);

        float cooldownAfterSecondFail = GetTreeCooldown();
        bool secondFailNoDamage = firstTree.GetCurrentHealth() == firstTreeHealth;
        bool secondFailNoEnergy = energySystem.CurrentEnergy == energyBeforeSecondFail;
        bool cooldownNotResetByAltLow = Mathf.Abs(cooldownAfterSecondFail - cooldownAfterFirstFail) < 0.25f;

        yield return new WaitForSeconds(BubbleNoRefreshTailProbeDelay);
        bool failureBubbleExpiredWithoutRefresh = bubblePresenter == null || !bubblePresenter.IsVisible;

        HideBubbleImmediate();
        yield return EquipToolSlot(LowAxeSlot, lowAxeTool, ToolRuntimeUtility.CreateRuntimeItem(lowAxeTool, 0, 1));
        ResetPlayerToolCommitGate();
        secondTree.OnHit(CreateToolHitContext(lowAxeTool, secondTree));
        yield return WaitFrames(2);

        bool transitionBubbleVisible = bubblePresenter != null && bubblePresenter.IsVisible;
        string transitionBubbleText = GetBubbleText();
        bool transitionStartedFromFailure = transitionBubbleVisible && IsAxeFailLine(transitionBubbleText);
        float cooldownBeforeHighSuccess = GetTreeCooldown();
        int secondTreeHealthBeforeHigh = secondTree.GetCurrentHealth();
        int energyBeforeHighSuccess = energySystem.CurrentEnergy;

        yield return EquipToolSlot(HighAxeSlot, highAxeTool, ToolRuntimeUtility.CreateRuntimeItem(highAxeTool, 0, 1));
        ResetPlayerToolCommitGate();
        secondTree.OnHit(CreateToolHitContext(highAxeTool, secondTree));
        yield return WaitFrames(3);

        string recoveredBubbleText = GetBubbleText();
        bool recoveredBubbleVisible = bubblePresenter != null && bubblePresenter.IsVisible;
        bool recoveredBubbleMatched = recoveredBubbleText.Contains("还是这把斧头锋利！");
        bool highSuccessDamagedTree = secondTree.GetCurrentHealth() < secondTreeHealthBeforeHigh;
        bool highSuccessConsumedEnergy = energySystem.CurrentEnergy == energyBeforeHighSuccess - highAxeTool.energyCost;
        bool cooldownNotResetByHighSuccess = Mathf.Abs(GetTreeCooldown() - cooldownBeforeHighSuccess) < 0.25f;

        yield return new WaitForSeconds(RecoveredBubbleProbeDelay);
        bool recoveredBubbleExpired = bubblePresenter == null || !bubblePresenter.IsVisible;

        bool passed =
            firstFailNoDamage &&
            firstFailNoEnergy &&
            firstFailBubbleVisible &&
            firstFailBubbleMatched &&
            cooldownStarted &&
            secondFailNoDamage &&
            secondFailNoEnergy &&
            cooldownNotResetByAltLow &&
            failureBubbleExpiredWithoutRefresh &&
            transitionStartedFromFailure &&
            highSuccessDamagedTree &&
            highSuccessConsumedEnergy &&
            recoveredBubbleVisible &&
            recoveredBubbleMatched &&
            cooldownNotResetByHighSuccess &&
            recoveredBubbleExpired;

        LogScenario(
            scenarioName,
            passed,
            $"firstFailNoDamage={firstFailNoDamage} firstFailNoEnergy={firstFailNoEnergy} firstFailBubbleVisible={firstFailBubbleVisible} firstFailBubbleMatched={firstFailBubbleMatched} " +
            $"cooldownStarted={cooldownStarted} secondFailNoDamage={secondFailNoDamage} secondFailNoEnergy={secondFailNoEnergy} cooldownNotResetByAltLow={cooldownNotResetByAltLow} " +
            $"failureBubbleExpiredWithoutRefresh={failureBubbleExpiredWithoutRefresh} transitionStartedFromFailure={transitionStartedFromFailure} " +
            $"highSuccessDamagedTree={highSuccessDamagedTree} highSuccessConsumedEnergy={highSuccessConsumedEnergy} recoveredBubbleVisible={recoveredBubbleVisible} " +
            $"recoveredBubbleMatched={recoveredBubbleMatched} cooldownNotResetByHighSuccess={cooldownNotResetByHighSuccess} recoveredBubbleExpired={recoveredBubbleExpired} " +
            $"firstFailBubbleText=\"{SanitizeLogValue(firstFailBubbleText)}\" recoveredBubbleText=\"{SanitizeLogValue(recoveredBubbleText)}\"");
    }

    private IEnumerator RunHoverOcclusionScenario()
    {
        const string scenarioName = "hover-occlusion-chain";

        if (!TryFindTillableCells(1, out List<FarmCellCandidate> candidates, out string reason))
        {
            LogScenario(scenarioName, false, $"candidate_search_failed=true details={reason}");
            yield break;
        }

        FarmCellCandidate candidate = candidates[0];
        bool restoreGameInput = gameInputManager != null && gameInputManager.enabled;

        // 009：这轮只验证 FarmToolPreview -> OcclusionManager 的 hover 提交链。
        // GameInputManager 的每帧鼠标预览会在 menu 触发的 live 场景里立刻覆盖/隐藏手动注入的 preview，
        // 先临时停掉这一层，避免把 editor 焦点噪音误判成 hover 链本身失败。
        if (gameInputManager != null)
        {
            gameInputManager.enabled = false;
        }

        LayerTilemaps tilemaps = farmTileManager.GetLayerTilemaps(candidate.LayerIndex);
        Vector3 cellSize = tilemaps != null && tilemaps.groundTilemap != null
            ? tilemaps.groundTilemap.layoutGrid.cellSize
            : Vector3.one;

        Vector3 sidePosition = tilemaps != null
            ? tilemaps.GetCellCenterWorld(candidate.CellPos + Vector3Int.right)
            : candidate.WorldPosition + Vector3.right;
        Vector3 centerPosition = candidate.WorldPosition;
        Vector3 sideOccluderScale = new Vector3(Mathf.Max(0.2f, Mathf.Abs(cellSize.x) * 0.35f), Mathf.Max(0.2f, Mathf.Abs(cellSize.y) * 0.35f), 1f);
        Vector3 centerOccluderScale = new Vector3(Mathf.Max(0.4f, Mathf.Abs(cellSize.x) * 0.9f), Mathf.Max(0.4f, Mathf.Abs(cellSize.y) * 0.9f), 1f);

        OcclusionTransparency sideOccluder = CreatePreviewOccluder("FarmRuntimeSideOccluder", sidePosition, sideOccluderScale);
        yield return WaitFrames(2);
        farmToolPreview.UpdateHoePreview(candidate.LayerIndex, candidate.CellPos, playerTransform, 99f);
        yield return WaitFrames(3);

        bool sideStayedOpaque = sideOccluder != null && !ReadOccludingState(sideOccluder);

        OcclusionTransparency centerOccluder = CreatePreviewOccluder("FarmRuntimeCenterOccluder", centerPosition, centerOccluderScale);
        yield return WaitFrames(2);
        farmToolPreview.UpdateHoePreview(candidate.LayerIndex, candidate.CellPos, playerTransform, 99f);
        yield return WaitFrames(3);

        bool centerBecameTransparent = centerOccluder != null && ReadOccludingState(centerOccluder);
        Bounds? activePreviewBounds = ReadPreviewBounds();
        Bounds centerBounds = centerOccluder != null ? centerOccluder.GetBounds() : default;
        bool centerBoundsIntersected = centerOccluder != null &&
                                       activePreviewBounds.HasValue &&
                                       centerBounds.Intersects(activePreviewBounds.Value);
        bool centerTrackedByManager = centerOccluder != null && PreviewOccludingContains(centerOccluder);

        farmToolPreview.Hide();
        yield return WaitFrames(3);

        bool centerRecovered = centerOccluder != null && !ReadOccludingState(centerOccluder);
        bool passed = sideStayedOpaque && centerBecameTransparent && centerRecovered;

        if (gameInputManager != null)
        {
            gameInputManager.enabled = restoreGameInput;
        }

        LogScenario(
            scenarioName,
            passed,
            $"sideStayedOpaque={sideStayedOpaque} centerBecameTransparent={centerBecameTransparent} centerRecovered={centerRecovered} " +
            $"centerBoundsIntersected={centerBoundsIntersected} centerTrackedByManager={centerTrackedByManager} " +
            $"previewBounds=\"{SanitizeBounds(activePreviewBounds)}\" centerBounds=\"{SanitizeBounds(centerBounds)}\"");
    }

    private bool ResolveReferences(out string reason)
    {
        gameInputManager = FindFirstObjectByType<GameInputManager>();
        inventoryService = FindFirstObjectByType<InventoryService>();
        hotbarSelection = FindFirstObjectByType<HotbarSelectionService>();
        energySystem = EnergySystem.Instance;
        playerInteraction = FindFirstObjectByType<PlayerInteraction>();
        playerRoot = playerInteraction != null
            ? playerInteraction.gameObject
            : FindFirstObjectByType<PlayerMovement>()?.gameObject;
        playerTransform = playerRoot != null ? playerRoot.transform : null;
        playerToolController = playerRoot != null ? playerRoot.GetComponent<PlayerToolController>() : null;
        playerSpriteRenderer = playerRoot != null ? playerRoot.GetComponentInChildren<SpriteRenderer>() : null;
        database = inventoryService != null ? inventoryService.Database : null;
        farmTileManager = FarmTileManager.Instance;
        farmToolPreview = FarmToolPreview.Instance;
        occlusionManager = OcclusionManager.Instance;
        playerFeedbackService = PlayerToolFeedbackService.ResolveForPlayer(playerRoot);
        bubblePresenter = playerFeedbackService != null ? playerFeedbackService.GetComponent<PlayerThoughtBubblePresenter>() : null;
        mainCamera = Camera.main;

        if (gameInputManager == null ||
            inventoryService == null ||
            hotbarSelection == null ||
            energySystem == null ||
            playerRoot == null ||
            playerToolController == null ||
            playerTransform == null ||
            database == null ||
            farmTileManager == null ||
            farmToolPreview == null ||
            occlusionManager == null ||
            mainCamera == null)
        {
            reason =
                $"missing_refs gim={gameInputManager != null} inv={inventoryService != null} hotbar={hotbarSelection != null} " +
                $"energy={energySystem != null} player={playerRoot != null} toolController={playerToolController != null} " +
                $"db={database != null} farmTile={farmTileManager != null} preview={farmToolPreview != null} occlusion={occlusionManager != null} cam={mainCamera != null}";
            return false;
        }

        List<ToolData> tools = database.GetAllTools();
        hoeTool = FindTool(tools, tool => tool.toolType == ToolType.Hoe && ToolRuntimeUtility.UsesDurability(tool))
            ?? FindTool(tools, tool => tool.toolType == ToolType.Hoe);
        wateringTool = FindTool(tools, tool => tool.toolType == ToolType.WateringCan);
        lowAxeTool = FindTool(tools, tool => tool.toolType == ToolType.Axe && tool.GetMaterialTierValue() == 0)
            ?? FindTool(tools, tool => tool.toolType == ToolType.Axe && tool.GetMaterialTierValue() < 3);
        altLowAxeTool = FindTool(
            tools,
            tool => tool.toolType == ToolType.Axe &&
                    tool.GetMaterialTierValue() < 3 &&
                    lowAxeTool != null &&
                    tool.itemID != lowAxeTool.itemID)
            ?? lowAxeTool;
        highAxeTool = FindTool(tools, tool => tool.toolType == ToolType.Axe && tool.GetMaterialTierValue() >= 3);

        if (hoeTool == null || wateringTool == null || lowAxeTool == null || altLowAxeTool == null || highAxeTool == null)
        {
            reason =
                $"missing_tools hoe={hoeTool != null} water={wateringTool != null} lowAxe={lowAxeTool != null} " +
                $"altLowAxe={altLowAxeTool != null} highAxe={highAxeTool != null}";
            return false;
        }

        if (ExecuteTillSoilMethod == null || ExecuteWaterTileMethod == null)
        {
            reason = $"missing_methods till={ExecuteTillSoilMethod != null} water={ExecuteWaterTileMethod != null}";
            return false;
        }

        reason = null;
        return true;
    }

    private static ToolData FindTool(List<ToolData> tools, Predicate<ToolData> predicate)
    {
        if (tools == null)
        {
            return null;
        }

        foreach (ToolData tool in tools)
        {
            if (tool != null && predicate(tool))
            {
                return tool;
            }
        }

        return null;
    }

    private IEnumerator EquipToolSlot(int slotIndex, ToolData toolData, InventoryItem runtimeItem)
    {
        if (toolData == null)
        {
            yield break;
        }

        InventoryItem item = runtimeItem ?? ToolRuntimeUtility.CreateRuntimeItem(toolData, 0, 1);
        inventoryService.SetSlot(slotIndex, new ItemStack(toolData.itemID, 0, Mathf.Max(1, item.Amount)));
        inventoryService.SetInventoryItem(slotIndex, item);
        inventoryService.RefreshSlot(slotIndex);
        hotbarSelection.SelectIndex(slotIndex);

        yield return WaitFrames(2);
        inventoryService.RefreshSlot(slotIndex);
        yield return WaitFrames(2);
    }

    private bool InvokeExecuteTillSoil(int layerIndex, Vector3Int cellPos)
    {
        object result = ExecuteTillSoilMethod.Invoke(gameInputManager, new object[] { layerIndex, cellPos });
        return result is bool succeeded && succeeded;
    }

    private bool InvokeExecuteWaterTile(int layerIndex, Vector3Int cellPos, int puddleVariant)
    {
        object result = ExecuteWaterTileMethod.Invoke(gameInputManager, new object[] { layerIndex, cellPos, puddleVariant });
        return result is bool succeeded && succeeded;
    }

    private bool TryFindTillableCells(int requiredCount, out List<FarmCellCandidate> candidates, out string reason)
    {
        candidates = new List<FarmCellCandidate>();
        reason = null;

        int layerIndex = farmTileManager.GetCurrentLayerIndex(playerTransform.position);
        LayerTilemaps tilemaps = farmTileManager.GetLayerTilemaps(layerIndex);
        if (tilemaps == null || tilemaps.groundTilemap == null)
        {
            reason = $"tilemaps_missing layer={layerIndex}";
            return false;
        }

        Vector3Int playerCell = tilemaps.WorldToCell(playerTransform.position);
        HashSet<Vector3Int> visited = new();

        for (int radius = 0; radius <= 10 && candidates.Count < requiredCount; radius++)
        {
            for (int x = -radius; x <= radius && candidates.Count < requiredCount; x++)
            {
                for (int y = -radius; y <= radius && candidates.Count < requiredCount; y++)
                {
                    if (radius > 0 && Mathf.Abs(x) != radius && Mathf.Abs(y) != radius)
                    {
                        continue;
                    }

                    Vector3Int cellPos = playerCell + new Vector3Int(x, y, 0);
                    if (!visited.Add(cellPos))
                    {
                        continue;
                    }

                    if (!tilemaps.groundTilemap.HasTile(cellPos))
                    {
                        continue;
                    }

                    Vector3 worldPos = tilemaps.GetCellCenterWorld(cellPos);
                    if (!IsWorldPointVisible(worldPos))
                    {
                        continue;
                    }

                    if (!farmTileManager.CanTillAt(layerIndex, cellPos))
                    {
                        continue;
                    }

                    candidates.Add(new FarmCellCandidate
                    {
                        LayerIndex = layerIndex,
                        CellPos = cellPos,
                        WorldPosition = worldPos
                    });
                }
            }
        }

        if (candidates.Count < requiredCount)
        {
            reason = $"not_enough_cells found={candidates.Count} required={requiredCount} layer={layerIndex}";
            return false;
        }

        return true;
    }

    private bool IsWorldPointVisible(Vector3 worldPosition)
    {
        Vector3 viewport = mainCamera.WorldToViewportPoint(worldPosition);
        return viewport.z > 0f && viewport.x >= 0.05f && viewport.x <= 0.95f && viewport.y >= 0.05f && viewport.y <= 0.95f;
    }

    private void EnsureTilledCell(FarmCellCandidate candidate)
    {
        if (farmTileManager.GetTileData(candidate.LayerIndex, candidate.CellPos)?.isTilled == true)
        {
            return;
        }

        farmTileManager.CreateTile(candidate.LayerIndex, candidate.CellPos);
    }

    private void ResetWateredState(FarmCellCandidate candidate)
    {
        FarmTileData tileData = farmTileManager.GetTileData(candidate.LayerIndex, candidate.CellPos);
        if (tileData == null)
        {
            return;
        }

        tileData.wateredToday = false;
        tileData.wateredYesterday = false;
        tileData.waterTime = -1f;
        tileData.moistureState = SoilMoistureState.Dry;
    }

    private TreeController CreateScenarioTree(string name, Vector3 position)
    {
        TreeController sourceTree = FindFirstObjectByType<TreeController>();
        if (sourceTree == null)
        {
            return null;
        }

        GameObject sourceRoot = sourceTree.transform.parent != null ? sourceTree.transform.parent.gameObject : sourceTree.gameObject;
        GameObject clone = Instantiate(sourceRoot, position, sourceRoot.transform.rotation);
        clone.name = name;
        spawnedArtifacts.Add(clone);

        TreeController clonedTree = clone.GetComponent<TreeController>() ?? clone.GetComponentInChildren<TreeController>(true);
        if (clonedTree == null)
        {
            return null;
        }

        clonedTree.InitializeAsNewTree();
        clonedTree.SetStage(5);
        return clonedTree;
    }

    private ToolHitContext CreateToolHitContext(ToolData toolData, TreeController tree)
    {
        Vector2 hitPoint = tree != null ? tree.GetPosition() : playerTransform.position;
        return new ToolHitContext
        {
            toolItemId = toolData.itemID,
            toolQuality = 0,
            toolType = toolData.toolType,
            actionState = 0,
            hitPoint = hitPoint,
            hitDir = Vector2.right,
            attacker = playerRoot,
            baseDamage = 1f,
            frameIndex = 0
        };
    }

    private OcclusionTransparency CreatePreviewOccluder(string name, Vector3 position, Vector3 scale)
    {
        GameObject go = new GameObject(name);
        go.transform.position = position;
        go.transform.localScale = scale;

        string existingTag = ResolveRegisteredOccluderTag();
        if (!string.IsNullOrEmpty(existingTag) && existingTag != "Untagged")
        {
            go.tag = existingTag;
        }

        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = GetSolidSprite();
        renderer.color = new Color(1f, 1f, 1f, 0.95f);

        if (playerSpriteRenderer != null)
        {
            renderer.sortingLayerName = playerSpriteRenderer.sortingLayerName;
            renderer.sortingOrder = playerSpriteRenderer.sortingOrder + 20;
        }

        go.AddComponent<BoxCollider2D>();
        OcclusionTransparency occluder = go.AddComponent<OcclusionTransparency>();
        occlusionManager.RegisterOccluder(occluder);
        spawnedArtifacts.Add(go);
        return occluder;
    }

    private string ResolveRegisteredOccluderTag()
    {
        if (RegisteredOccludersField?.GetValue(occlusionManager) is not HashSet<OcclusionTransparency> occluders)
        {
            return ResolveFallbackOccluderTag();
        }

        foreach (OcclusionTransparency occluder in occluders)
        {
            if (occluder != null && occluder.gameObject != null && !string.IsNullOrEmpty(occluder.gameObject.tag))
            {
                return occluder.gameObject.tag;
            }
        }

        return ResolveFallbackOccluderTag();
    }

    private string ResolveFallbackOccluderTag()
    {
        if (OccludableTagsField?.GetValue(occlusionManager) is string[] tags && tags.Length > 0)
        {
            return tags[0];
        }

        return null;
    }

    private static Sprite GetSolidSprite()
    {
        if (sSolidSprite != null)
        {
            return sSolidSprite;
        }

        sSolidTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        sSolidTexture.SetPixel(0, 0, Color.white);
        sSolidTexture.Apply();
        sSolidTexture.wrapMode = TextureWrapMode.Clamp;
        sSolidSprite = Sprite.Create(sSolidTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        return sSolidSprite;
    }

    private void SetPlacementMode(bool enabled)
    {
        PlacementModeSetter?.Invoke(gameInputManager, new object[] { enabled });
    }

    private void SetTreeCooldown(float value)
    {
        CooldownUntilField?.SetValue(null, value);
    }

    private float GetTreeCooldown()
    {
        return CooldownUntilField?.GetValue(null) is float value ? value : -1f;
    }

    private bool ReadOccludingState(OcclusionTransparency occluder)
    {
        return IsOccludingField?.GetValue(occluder) is bool value && value;
    }

    private Bounds? ReadPreviewBounds()
    {
        object value = PreviewBoundsField?.GetValue(occlusionManager);
        if (value is Bounds bounds)
        {
            return bounds;
        }

        return null;
    }

    private bool PreviewOccludingContains(OcclusionTransparency occluder)
    {
        if (PreviewOccludingField?.GetValue(occlusionManager) is not HashSet<OcclusionTransparency> previewOccluding)
        {
            return false;
        }

        foreach (OcclusionTransparency candidate in previewOccluding)
        {
            if (candidate == occluder)
            {
                return true;
            }
        }

        return false;
    }

    private void HideBubbleImmediate()
    {
        bubblePresenter?.HideImmediate();
    }

    private void ResetPlayerToolCommitGate()
    {
        if (playerInteraction != null && ToolUseCommittedField != null)
        {
            ToolUseCommittedField.SetValue(playerInteraction, false);
        }
    }

    private string GetBubbleText()
    {
        if (bubblePresenter == null || BubbleTextField?.GetValue(bubblePresenter) is not TextMeshProUGUI bubbleText)
        {
            return string.Empty;
        }

        return bubbleText.text ?? string.Empty;
    }

    private int CountAudioSources()
    {
        return FindObjectsByType<AudioSource>(FindObjectsSortMode.None).Length;
    }

    private int CountFeedbackBursts()
    {
        int count = 0;
        foreach (ParticleSystem particleSystem in FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None))
        {
            if (particleSystem != null && particleSystem.gameObject.name.StartsWith("PlayerToolFeedbackBurst", StringComparison.Ordinal))
            {
                count++;
            }
        }

        return count;
    }

    private void CleanupArtifacts()
    {
        for (int i = spawnedArtifacts.Count - 1; i >= 0; i--)
        {
            UnityEngine.Object artifact = spawnedArtifacts[i];
            if (artifact == null)
            {
                continue;
            }

            if (Application.isPlaying)
            {
                Destroy(artifact);
            }
            else
            {
                DestroyImmediate(artifact);
            }
        }

        spawnedArtifacts.Clear();
    }

    private void LogScenario(string scenarioName, bool passed, string details)
    {
        scenarioLogs.Add($"{LogPrefix} scenario={scenarioName} passed={passed} details={details}");
    }

    private static string SanitizeLogValue(string value)
    {
        return string.IsNullOrEmpty(value)
            ? string.Empty
            : value.Replace("\r", " ").Replace("\n", " ").Replace("\"", "'");
    }

    private static string SanitizeBounds(Bounds? bounds)
    {
        return bounds.HasValue ? SanitizeBounds(bounds.Value) : "null";
    }

    private static string SanitizeBounds(Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;
        return $"c=({center.x:F2},{center.y:F2},{center.z:F2}) s=({size.x:F2},{size.y:F2},{size.z:F2})";
    }

    private static bool IsToolBrokenLine(string text)
    {
        return text == "这么不耐用吗？" ||
               text == "看来得换个好点的了。" ||
               text == "哎哟，搞什么鬼？" ||
               text == "诶，好吧…" ||
               text == "旧的不去新的不来！" ||
               text == "碎碎平安~";
    }

    private static bool IsWaterEmptyLine(string text)
    {
        return text == "没水了，先去装满再来吧。" ||
               text == "壶里已经一滴都不剩了。" ||
               text == "先补点水，再继续浇。";
    }

    private static bool IsAxeFailLine(string text)
    {
        return text == "这棵树我现在还砍不动吗？" ||
               text == "看来还得换把更锋利的斧头。" ||
               text == "这棵树现在还不是我能动的。";
    }

    private static IEnumerator WaitFrames(int frameCount)
    {
        for (int i = 0; i < frameCount; i++)
        {
            yield return null;
        }
    }
}
