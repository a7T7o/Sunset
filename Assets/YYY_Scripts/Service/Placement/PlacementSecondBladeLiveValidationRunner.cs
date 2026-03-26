using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FarmGame.Data;
using FarmGame.Data.Core;
using FarmGame.Events;
using FarmGame.UI;
using FarmGame.World;
using UnityEngine;
#if UNITY_EDITOR
using System.Runtime.InteropServices;
using UnityEditor;
#endif

/// <summary>
/// 第二刀放置链 live 终验 Runner。
/// 只服务 004 文档要求的 4 条场景，不承载业务逻辑。
/// </summary>
public class PlacementSecondBladeLiveValidationRunner : MonoBehaviour
{
    private const string LogPrefix = "[PlacementSecondBlade]";
    private const int ChestItemId = 1400;
    private const int SaplingItemId = 1200;
    private const int ChestHotbarSlot = 0;
    private const int SaplingHotbarSlot = 1;
    private const float ScenarioTimeout = 12f;
    private const float ScenarioPollInterval = 0.05f;
    private const float PositionTolerance = 0.15f;

    private enum CandidatePreference
    {
        RequireNavigation,
        PreferImmediate,
        Any
    }

    private enum ScenarioKind
    {
        None,
        ChestReachEnvelope,
        PreviewRefreshAfterPlacement,
        SaplingGhostOccupancy,
        ChestSaveLoadRegression
    }

    private enum RunScope
    {
        Full,
        SaplingOnly
    }

    private struct ScenarioResult
    {
        public string Name;
        public bool Passed;
        public string Details;
    }

    private struct PlacementCandidate
    {
        public Vector3 WorldPosition;
        public Bounds PreviewBounds;
        public bool RequiresNavigation;
    }

    private static readonly FieldInfo PlacementPreviewField =
        typeof(PlacementManager).GetField("placementPreview", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo NavigatorField =
        typeof(PlacementManager).GetField("navigator", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo PlayerTransformField =
        typeof(PlacementManager).GetField("playerTransform", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo MainCameraField =
        typeof(PlacementManager).GetField("mainCamera", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly PropertyInfo GameInputPlacementModeProperty =
        typeof(GameInputManager).GetProperty("IsPlacementMode", BindingFlags.Instance | BindingFlags.Public);

    private static readonly MethodInfo GameInputPlacementModeSetter =
        GameInputPlacementModeProperty?.GetSetMethod(true);

    private static readonly MethodInfo SyncPlaceableModeMethod =
        typeof(GameInputManager).GetMethod("SyncPlaceableModeWithCurrentSelection", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo RefreshPlacementValidationAtMethod =
        typeof(PlacementManager).GetMethod("RefreshPlacementValidationAt", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo LockPreviewPositionMethod =
        typeof(PlacementManager).GetMethod("LockPreviewPosition", BindingFlags.Instance | BindingFlags.NonPublic);

    private readonly List<ScenarioResult> scenarioResults = new List<ScenarioResult>();
    private readonly List<UnityEngine.Object> spawnedArtifacts = new List<UnityEngine.Object>();

    private Coroutine runCoroutine;
    private PlacementManager placementManager;
    private PlacementPreview placementPreview;
    private PlacementNavigator placementNavigator;
    private InventoryService inventoryService;
    private HotbarSelectionService hotbarSelection;
    private GameInputManager gameInputManager;
    private PlayerAutoNavigator playerNavigator;
    private PackagePanelTabsUI packageTabs;
    private ItemDatabase database;
    private Transform playerTransform;
    private Collider2D playerCollider;
    private Camera mainCamera;
    private StorageData chestItemData;
    private SaplingData saplingItemData;

    private PlacementEventData? lastPlacementEvent;
    private SaplingPlantedEventData? lastSaplingEvent;
    private ChestController scenarioChest;
    private GameObject scenarioChestRoot;
    private Vector3 scenarioChestTarget;
    private bool runFailed;
    private RunScope requestedRunScope = RunScope.Full;

    public static PlacementSecondBladeLiveValidationRunner BeginOrRestart()
    {
        PlacementSecondBladeLiveValidationRunner runner = FindFirstObjectByType<PlacementSecondBladeLiveValidationRunner>();
        if (runner == null)
        {
            GameObject go = new GameObject("PlacementSecondBladeLiveValidationRunner");
            runner = go.AddComponent<PlacementSecondBladeLiveValidationRunner>();
        }

        runner.requestedRunScope = RunScope.Full;
        runner.StartRun();
        return runner;
    }

    public static PlacementSecondBladeLiveValidationRunner BeginSaplingOnly()
    {
        PlacementSecondBladeLiveValidationRunner runner = FindFirstObjectByType<PlacementSecondBladeLiveValidationRunner>();
        if (runner == null)
        {
            GameObject go = new GameObject("PlacementSecondBladeLiveValidationRunner");
            runner = go.AddComponent<PlacementSecondBladeLiveValidationRunner>();
        }

        runner.requestedRunScope = RunScope.SaplingOnly;
        runner.StartRun();
        return runner;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (runCoroutine == null)
        {
            StartRun();
        }
    }

    private void OnEnable()
    {
        PlacementManager.OnItemPlaced += HandleItemPlaced;
        PlacementManager.OnSaplingPlanted += HandleSaplingPlanted;
    }

    private void OnDisable()
    {
        PlacementManager.OnItemPlaced -= HandleItemPlaced;
        PlacementManager.OnSaplingPlanted -= HandleSaplingPlanted;
    }

    private void StartRun()
    {
        if (runCoroutine != null)
        {
            StopCoroutine(runCoroutine);
        }

        runCoroutine = StartCoroutine(RunAllScenarios());
    }

    private IEnumerator RunAllScenarios()
    {
        scenarioResults.Clear();
        spawnedArtifacts.Clear();
        lastPlacementEvent = null;
        lastSaplingEvent = null;
        scenarioChest = null;
        scenarioChestRoot = null;
        runFailed = false;

        CleanupArtifacts();

        if (!ResolveReferences(out string resolveReason))
        {
            FinishFailedRun("ResolveReferences", resolveReason);
            yield break;
        }

        Debug.Log($"{LogPrefix} runner_started scene={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name} scope={requestedRunScope}");

        if (requestedRunScope == RunScope.SaplingOnly)
        {
            yield return RunScenario(ScenarioKind.SaplingGhostOccupancy, RunSaplingGhostScenario);
            if (runFailed) yield break;

            Debug.Log($"{LogPrefix} all_completed=true scenario_count={scenarioResults.Count} scope={requestedRunScope}");
            runCoroutine = null;
            yield break;
        }

        yield return RunScenario(ScenarioKind.ChestReachEnvelope, RunChestReachEnvelopeScenario);
        if (runFailed) yield break;

        yield return RunScenario(ScenarioKind.PreviewRefreshAfterPlacement, RunPreviewRefreshScenario);
        if (runFailed) yield break;

        yield return RunScenario(ScenarioKind.SaplingGhostOccupancy, RunSaplingGhostScenario);
        if (runFailed) yield break;

        yield return RunScenario(ScenarioKind.ChestSaveLoadRegression, RunChestSaveLoadScenario);
        if (runFailed) yield break;

        Debug.Log($"{LogPrefix} all_completed=true scenario_count={scenarioResults.Count} scope={requestedRunScope}");
        runCoroutine = null;
    }

    private IEnumerator RunScenario(ScenarioKind scenarioKind, Func<IEnumerator> scenarioRoutineFactory)
    {
        Debug.Log($"{LogPrefix} scenario_start={scenarioKind}");
        yield return scenarioRoutineFactory();
    }

    private IEnumerator RunChestReachEnvelopeScenario()
    {
        if (!PreparePlacementItem(ChestHotbarSlot, ChestItemId, 2, out string prepareReason))
        {
            FinishScenario(ScenarioKind.ChestReachEnvelope, false, prepareReason);
            yield break;
        }

        if (!TryFindPlacementCandidate(CandidatePreference.RequireNavigation, out PlacementCandidate candidate, out string candidateReason))
        {
            FinishScenario(ScenarioKind.ChestReachEnvelope, false, candidateReason);
            yield break;
        }

        Vector3 playerStart = playerTransform.position;

        yield return MoveCursorAndPrimePreview(candidate.WorldPosition);
        scenarioChestTarget = placementPreview != null ? placementPreview.GetPreviewPosition() : candidate.WorldPosition;

        lastPlacementEvent = null;
        placementManager.OnLeftClick();

        yield return WaitForCondition(
            () => lastPlacementEvent.HasValue && lastPlacementEvent.Value.ItemData != null && lastPlacementEvent.Value.ItemData.itemID == ChestItemId,
            ScenarioTimeout);

        if (!lastPlacementEvent.HasValue)
        {
            FinishScenario(
                ScenarioKind.ChestReachEnvelope,
                false,
                $"timeout=true state={placementManager.CurrentState} playerPos={playerTransform.position} target={candidate.WorldPosition}");
            yield break;
        }

        scenarioChestRoot = lastPlacementEvent.Value.PlacedObject;
        scenarioChest = scenarioChestRoot != null
            ? scenarioChestRoot.GetComponentInChildren<ChestController>()
            : null;

        if (scenarioChest == null)
        {
            FinishScenario(ScenarioKind.ChestReachEnvelope, false, "placed_chest_controller_missing=true");
            yield break;
        }

        spawnedArtifacts.Add(scenarioChestRoot != null ? scenarioChestRoot : scenarioChest.gameObject);

        float playerToTargetDistance = Vector2.Distance(playerStart, playerTransform.position);
        Vector3 placedPosition = scenarioChestRoot != null ? scenarioChestRoot.transform.position : scenarioChest.transform.position;
        Transform placedParent = scenarioChestRoot != null ? scenarioChestRoot.transform.parent : scenarioChest.transform.parent;
        string parentPath = GetTransformPath(placedParent);
        bool placedNearTarget = Vector2.Distance(placedPosition, candidate.WorldPosition) <= 1.5f;
        bool parentResolved =
            placedParent != null &&
            parentPath.Contains("SCENE/LAYER", StringComparison.OrdinalIgnoreCase) &&
            parentPath.Contains("/Props", StringComparison.OrdinalIgnoreCase);
        bool passed = placedNearTarget && playerToTargetDistance > 0.2f && parentResolved;

        FinishScenario(
            ScenarioKind.ChestReachEnvelope,
            passed,
            $"placedNearTarget={placedNearTarget} parentResolved={parentResolved} parentPath={parentPath} movedDistance={playerToTargetDistance:F2} playerEnd={playerTransform.position} target={candidate.WorldPosition} state={placementManager.CurrentState}");
    }

    private IEnumerator RunPreviewRefreshScenario()
    {
        if (scenarioChest == null)
        {
            FinishScenario(ScenarioKind.PreviewRefreshAfterPlacement, false, "scenario_chest_missing=true");
            yield break;
        }

        yield return WaitFrames(6);

        if (placementManager.CurrentState != PlacementManager.PlacementState.Preview)
        {
            FinishScenario(
                ScenarioKind.PreviewRefreshAfterPlacement,
                false,
                $"expected_preview_state=true actual={placementManager.CurrentState}");
            yield break;
        }

        Vector3 previewPosition = placementPreview != null ? placementPreview.GetPreviewPosition() : Vector3.zero;
        bool stayedOnOriginalCell = Vector2.Distance(previewPosition, scenarioChestTarget) <= PositionTolerance;
        bool previewInvalid = placementPreview != null && !placementPreview.IsAllValid;
        bool passed = stayedOnOriginalCell && previewInvalid;

        FinishScenario(
            ScenarioKind.PreviewRefreshAfterPlacement,
            passed,
            $"stayedOnOriginalCell={stayedOnOriginalCell} previewInvalid={previewInvalid} previewPos={previewPosition} target={scenarioChestTarget}");

        if (placementManager.IsPlacementMode)
        {
            placementManager.OnRightClick();
            yield return WaitFrames(2);
        }
    }

    private IEnumerator RunSaplingGhostScenario()
    {
        if (!PreparePlacementItem(SaplingHotbarSlot, SaplingItemId, 2, out string prepareReason))
        {
            FinishScenario(ScenarioKind.SaplingGhostOccupancy, false, prepareReason);
            yield break;
        }

        int initialSaplingCount = FindObjectsByType<TreeController>(FindObjectsSortMode.None).Length;
        PlacementCandidate plantedCandidate = default;
        string firstPlantFailure = "sapling_first_plant_unresolved=true";
        bool firstPlantSucceeded = false;

        for (int attempt = 0; attempt < 3 && !firstPlantSucceeded; attempt++)
        {
            if (!TryFindPlacementCandidate(CandidatePreference.PreferImmediate, out PlacementCandidate candidate, out string candidateReason))
            {
                firstPlantFailure = $"attempt={attempt} {candidateReason}";
                yield return WaitFrames(2);
                continue;
            }

            Vector3 primedPreviewPosition = candidate.WorldPosition;
            bool previewReady =
                placementPreview != null &&
                InvokeRefreshPlacementValidation(candidate.WorldPosition, true);

            if (placementPreview != null)
            {
                primedPreviewPosition = placementPreview.GetPreviewPosition();
            }

            bool previewAligned = Vector2.Distance(primedPreviewPosition, candidate.WorldPosition) <= PositionTolerance;

            if (!previewReady || !previewAligned)
            {
                firstPlantFailure =
                    $"attempt={attempt} preview_not_ready={(!previewReady).ToString()} previewAligned={previewAligned.ToString()} state={placementManager.CurrentState} previewPos={primedPreviewPosition} target={candidate.WorldPosition}";
                yield return WaitFrames(2);
                continue;
            }

            lastSaplingEvent = null;
            TriggerPlacementAttempt();

            yield return WaitForCondition(
                () =>
                    (lastSaplingEvent.HasValue &&
                     lastSaplingEvent.Value.SaplingData != null &&
                     lastSaplingEvent.Value.SaplingData.itemID == SaplingItemId) ||
                    placementManager.CurrentState == PlacementManager.PlacementState.Navigating,
                2f);

            if (lastSaplingEvent.HasValue &&
                lastSaplingEvent.Value.SaplingData != null &&
                lastSaplingEvent.Value.SaplingData.itemID == SaplingItemId)
            {
                plantedCandidate = candidate;
                firstPlantSucceeded = true;
                break;
            }

            if (placementManager.CurrentState == PlacementManager.PlacementState.Navigating)
            {
                placementManager.OnRightClick();
                yield return WaitFrames(2);
                firstPlantFailure = $"attempt={attempt} unexpected_navigation=true target={candidate.WorldPosition}";
                continue;
            }

            firstPlantFailure = $"attempt={attempt} first_plant_timeout=true state={placementManager.CurrentState} target={candidate.WorldPosition}";
            yield return WaitFrames(2);
        }

        if (!firstPlantSucceeded)
        {
            FinishScenario(
                ScenarioKind.SaplingGhostOccupancy,
                false,
                firstPlantFailure);
            yield break;
        }

        if (lastSaplingEvent.Value.TreeObject != null)
        {
            spawnedArtifacts.Add(lastSaplingEvent.Value.TreeObject);
        }

        yield return WaitFrames(4);

        if (placementPreview != null)
        {
            InvokeRefreshPlacementValidation(plantedCandidate.WorldPosition, true);
        }

        lastSaplingEvent = null;
        TriggerPlacementAttempt();
        yield return WaitFrames(6);

        int finalSaplingCount = FindObjectsByType<TreeController>(FindObjectsSortMode.None).Length;
        bool secondPlantBlocked = !lastSaplingEvent.HasValue;
        bool previewInvalid = placementPreview != null && !placementPreview.IsAllValid;
        bool previewStayedOnOccupiedCell =
            placementPreview != null &&
            Vector2.Distance(placementPreview.GetPreviewPosition(), plantedCandidate.WorldPosition) <= PositionTolerance;
        bool onlyOneTreeCreated = finalSaplingCount == initialSaplingCount + 1;
        bool passed = secondPlantBlocked && previewInvalid && previewStayedOnOccupiedCell && onlyOneTreeCreated;

        FinishScenario(
            ScenarioKind.SaplingGhostOccupancy,
            passed,
            $"secondPlantBlocked={secondPlantBlocked} previewInvalid={previewInvalid} previewStayedOnOccupiedCell={previewStayedOnOccupiedCell} treeDelta={finalSaplingCount - initialSaplingCount} target={plantedCandidate.WorldPosition}");

        if (placementManager.IsPlacementMode)
        {
            placementManager.OnRightClick();
            yield return WaitFrames(2);
        }
    }

    private IEnumerator RunChestSaveLoadScenario()
    {
        if (scenarioChest == null)
        {
            FinishScenario(ScenarioKind.ChestSaveLoadRegression, false, "scenario_chest_missing=true");
            yield break;
        }

        var runtimeItem = new InventoryItem(ChestItemId, 2, 4);
        runtimeItem.SetDurability(40, 17);
        runtimeItem.SetProperty("seedRemaining", 9);
        runtimeItem.SetProperty("customName", "live-second-blade");

        scenarioChest.InventoryV2.SetItem(3, runtimeItem);
        scenarioChest.Inventory.SetSlot(5, new ItemStack(SaplingItemId, 1, 6));

        WorldObjectSaveData saveData = scenarioChest.Save();
        scenarioChest.Inventory.ClearSlot(3);
        scenarioChest.InventoryV2.ClearItem(5);

        if (scenarioChestRoot != null)
        {
            Destroy(scenarioChestRoot);
            scenarioChestRoot = null;
            scenarioChest = null;
            yield return WaitFrames(2);
        }

        GameObject restoredRoot = Instantiate(
            chestItemData.storagePrefab,
            playerTransform.position + new Vector3(2f, 0f, 0f),
            Quaternion.identity);
        spawnedArtifacts.Add(restoredRoot);

        ChestController restoredChest = restoredRoot.GetComponentInChildren<ChestController>();
        if (restoredChest == null)
        {
            FinishScenario(ScenarioKind.ChestSaveLoadRegression, false, "restored_chest_missing=true");
            yield break;
        }

        yield return WaitFrames(2);

        restoredChest.SetPersistentIdForLoad(saveData.guid);
        restoredChest.Load(saveData);
        yield return WaitFrames(2);

        InteractionContext context = new InteractionContext
        {
            PlayerPosition = playerTransform.position,
            PlayerTransform = playerTransform,
            HeldItemId = -1,
            HeldItemQuality = 0,
            HeldSlotIndex = -1,
            Inventory = inventoryService,
            Database = database,
            Navigator = playerNavigator
        };

        restoredChest.OnInteract(context);
        yield return WaitFrames(2);

        BoxPanelUI activeBoxUi = BoxPanelUI.ActiveInstance;
        bool uiOpened = activeBoxUi != null && activeBoxUi.IsOpen && activeBoxUi.CurrentChest == restoredChest;

        InventoryItem loadedRuntimeItem = restoredChest.InventoryV2.GetItem(3);
        ItemStack mirroredRuntimeStack = restoredChest.Inventory.GetSlot(3);
        InventoryItem loadedLegacySyncedItem = restoredChest.InventoryV2.GetItem(5);
        ItemStack mirroredLegacyStack = restoredChest.Inventory.GetSlot(5);

        bool runtimeRestored =
            loadedRuntimeItem != null &&
            loadedRuntimeItem.ItemId == runtimeItem.ItemId &&
            loadedRuntimeItem.Quality == runtimeItem.Quality &&
            loadedRuntimeItem.Amount == runtimeItem.Amount &&
            loadedRuntimeItem.MaxDurability == runtimeItem.MaxDurability &&
            loadedRuntimeItem.CurrentDurability == runtimeItem.CurrentDurability &&
            loadedRuntimeItem.GetProperty("customName") as string == "live-second-blade" &&
            loadedRuntimeItem.GetPropertyInt("seedRemaining", 0) == 9;

        bool mirrorRestored =
            mirroredRuntimeStack.itemId == runtimeItem.ItemId &&
            mirroredRuntimeStack.quality == runtimeItem.Quality &&
            mirroredRuntimeStack.amount == runtimeItem.Amount &&
            loadedLegacySyncedItem != null &&
            loadedLegacySyncedItem.ItemId == SaplingItemId &&
            loadedLegacySyncedItem.Quality == 1 &&
            loadedLegacySyncedItem.Amount == 6 &&
            mirroredLegacyStack.itemId == SaplingItemId &&
            mirroredLegacyStack.quality == 1 &&
            mirroredLegacyStack.amount == 6;

        bool passed = runtimeRestored && mirrorRestored && uiOpened && !restoredChest.IsEmpty;

        FinishScenario(
            ScenarioKind.ChestSaveLoadRegression,
            passed,
            $"runtimeRestored={runtimeRestored} mirrorRestored={mirrorRestored} uiOpened={uiOpened} chestIsEmpty={restoredChest.IsEmpty}");

        if (activeBoxUi != null && activeBoxUi.IsOpen)
        {
            activeBoxUi.Close();
            yield return WaitFrames(2);
        }

        if (packageTabs != null && packageTabs.IsPanelOpen())
        {
            packageTabs.ShowPanel(false);
            yield return WaitFrames(2);
        }
    }

    private void FinishScenario(ScenarioKind scenarioKind, bool passed, string details)
    {
        scenarioResults.Add(new ScenarioResult
        {
            Name = scenarioKind.ToString(),
            Passed = passed,
            Details = details
        });

        Debug.Log($"{LogPrefix} scenario_end={scenarioKind} pass={passed} details={details}");

        if (!passed)
        {
            runFailed = true;
            Debug.Log($"{LogPrefix} all_completed=false failed_scenario={scenarioKind} scenario_count={scenarioResults.Count}");
            runCoroutine = null;
        }
    }

    private void FinishFailedRun(string scenarioName, string details)
    {
        Debug.LogError($"{LogPrefix} scenario_end={scenarioName} pass=false details={details}");
        Debug.Log($"{LogPrefix} all_completed=false failed_scenario={scenarioName} scenario_count={scenarioResults.Count}");
        runCoroutine = null;
    }

    private bool ResolveReferences(out string reason)
    {
        placementManager = PlacementManager.Instance != null ? PlacementManager.Instance : FindFirstObjectByType<PlacementManager>();
        inventoryService = FindFirstObjectByType<InventoryService>();
        hotbarSelection = FindFirstObjectByType<HotbarSelectionService>();
        gameInputManager = GameInputManager.Instance != null ? GameInputManager.Instance : FindFirstObjectByType<GameInputManager>();
        playerNavigator = FindFirstObjectByType<PlayerAutoNavigator>();
        packageTabs = FindFirstObjectByType<PackagePanelTabsUI>(FindObjectsInactive.Include);

        if (placementManager == null || inventoryService == null || hotbarSelection == null)
        {
            reason = $"missing_refs placementManager={placementManager != null} inventory={inventoryService != null} hotbar={hotbarSelection != null}";
            return false;
        }

        placementPreview = PlacementPreviewField?.GetValue(placementManager) as PlacementPreview;
        placementNavigator = NavigatorField?.GetValue(placementManager) as PlacementNavigator;
        playerTransform = PlayerTransformField?.GetValue(placementManager) as Transform;
        mainCamera = MainCameraField?.GetValue(placementManager) as Camera;
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (playerTransform == null && playerNavigator != null)
        {
            playerTransform = playerNavigator.transform;
        }

        if (playerTransform != null)
        {
            playerCollider = playerTransform.GetComponent<Collider2D>();
        }

        database = inventoryService.Database;
        chestItemData = database != null ? database.GetItemByID(ChestItemId) as StorageData : null;
        saplingItemData = database != null ? database.GetItemByID(SaplingItemId) as SaplingData : null;

        if (placementPreview == null || placementNavigator == null || playerTransform == null || playerCollider == null || database == null)
        {
            reason =
                $"resolve_failed preview={placementPreview != null} navigator={placementNavigator != null} playerTransform={playerTransform != null} playerCollider={playerCollider != null} database={database != null}";
            return false;
        }

        if (chestItemData == null || saplingItemData == null)
        {
            reason = $"item_lookup_failed chest={chestItemData != null} sapling={saplingItemData != null}";
            return false;
        }

        reason = null;
        return true;
    }

    private bool PreparePlacementItem(int slotIndex, int itemId, int amount, out string reason)
    {
        if (database == null)
        {
            reason = "database_missing=true";
            return false;
        }

        ItemData itemData = database.GetItemByID(itemId);
        if (itemData == null)
        {
            reason = $"item_missing itemId={itemId}";
            return false;
        }

        if (placementManager.IsPlacementMode)
        {
            placementManager.ExitPlacementMode();
        }

        SetPlacementMode(true);
        inventoryService.SetSlot(slotIndex, new ItemStack(itemId, 0, amount));
        inventoryService.RefreshSlot(slotIndex);
        hotbarSelection.SelectIndex(slotIndex);

        if (gameInputManager != null && SyncPlaceableModeMethod != null)
        {
            SyncPlaceableModeMethod.Invoke(gameInputManager, null);
        }

        if (!placementManager.IsPlacementMode || placementManager.CurrentPlacementItem == null || placementManager.CurrentPlacementItem.itemID != itemId)
        {
            placementManager.EnterPlacementMode(itemData, 0);
        }

        reason = null;
        return true;
    }

    private bool TryFindPlacementCandidate(
        CandidatePreference preference,
        out PlacementCandidate candidate,
        out string reason)
    {
        candidate = default;
        reason = null;

        Vector3 origin = GetCandidateSearchOrigin(preference);
        float bestDistance = float.MaxValue;
        bool found = false;
        bool requireImmediate = preference == CandidatePreference.PreferImmediate;

        for (int radius = 0; radius <= 8; radius++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (radius > 0 && Mathf.Abs(x) != radius && Mathf.Abs(y) != radius)
                    {
                        continue;
                    }

                    Vector3 worldPosition = PlacementGridCalculator.GetCellCenter(origin + new Vector3(x, y, 0f));
                    if (!IsWorldPointVisible(worldPosition))
                    {
                        continue;
                    }

                    if (!InvokeRefreshPlacementValidation(worldPosition, true))
                    {
                        continue;
                    }

                    Bounds previewBounds = placementPreview.GetPreviewBounds();
                    bool requiresNavigation = !placementNavigator.IsAlreadyNearTarget(playerCollider.bounds, previewBounds);

                    if (requireImmediate && requiresNavigation)
                    {
                        continue;
                    }

                    if (preference == CandidatePreference.RequireNavigation && !requiresNavigation)
                    {
                        continue;
                    }

                    float distance = Vector2.Distance(worldPosition, playerTransform.position);
                    if (preference == CandidatePreference.RequireNavigation && distance < 1.5f)
                    {
                        continue;
                    }

                    if (preference == CandidatePreference.PreferImmediate && requiresNavigation && found)
                    {
                        continue;
                    }

                    if (!found || distance < bestDistance || (preference == CandidatePreference.PreferImmediate && !requiresNavigation))
                    {
                        candidate = new PlacementCandidate
                        {
                            WorldPosition = worldPosition,
                            PreviewBounds = previewBounds,
                            RequiresNavigation = requiresNavigation
                        };
                        bestDistance = distance;
                        found = true;

                        if (preference == CandidatePreference.PreferImmediate && !requiresNavigation)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        if (!found)
        {
            reason = requireImmediate
                ? $"immediate_candidate_not_found preference={preference}"
                : $"candidate_not_found preference={preference}";
            return false;
        }

        return true;
    }

    private IEnumerator MoveCursorAndPrimePreview(Vector3 worldPosition)
    {
        InvokeRefreshPlacementValidation(worldPosition, true);
        TryMoveCursorToWorldPoint(worldPosition);
        yield return WaitFrames(4);

        for (int attempt = 0; attempt < 3; attempt++)
        {
            InvokeRefreshPlacementValidation(worldPosition, true);
            Vector3 hoverWorld = GetMouseWorldPosition();
            Debug.Log($"{LogPrefix} hover_after_move target={worldPosition} hover={hoverWorld} attempt={attempt}");

            if (Vector2.Distance(hoverWorld, worldPosition) <= 0.75f)
            {
                break;
            }

            if (!TryNudgeCursorToWorldPoint(worldPosition, hoverWorld))
            {
                break;
            }

            yield return WaitFrames(3);
        }
    }

    private bool InvokeRefreshPlacementValidation(Vector3 worldPosition, bool updatePreviewPosition)
    {
        if (RefreshPlacementValidationAtMethod == null)
        {
            return false;
        }

        object result = RefreshPlacementValidationAtMethod.Invoke(
            placementManager,
            new object[] { worldPosition, updatePreviewPosition });

        return result is bool valid && valid;
    }

    private void TriggerPlacementAttempt()
    {
        if (LockPreviewPositionMethod != null && placementManager != null && placementManager.CurrentState == PlacementManager.PlacementState.Preview)
        {
            LockPreviewPositionMethod.Invoke(placementManager, null);
            return;
        }

        placementManager.OnLeftClick();
    }

    private void CleanupArtifacts()
    {
        if (BoxPanelUI.ActiveInstance != null && BoxPanelUI.ActiveInstance.IsOpen)
        {
            BoxPanelUI.ActiveInstance.Close();
        }

        if (packageTabs != null && packageTabs.IsPanelOpen())
        {
            packageTabs.ShowPanel(false);
        }

        SetPlacementMode(false);

        for (int i = spawnedArtifacts.Count - 1; i >= 0; i--)
        {
            if (spawnedArtifacts[i] == null)
            {
                continue;
            }

            if (Application.isPlaying)
            {
                Destroy(spawnedArtifacts[i]);
            }
            else
            {
                DestroyImmediate(spawnedArtifacts[i]);
            }
        }
    }

    private static string GetTransformPath(Transform transform)
    {
        if (transform == null)
        {
            return "<scene-root>";
        }

        var segments = new List<string>();
        Transform current = transform;
        while (current != null)
        {
            segments.Add(current.name);
            current = current.parent;
        }

        segments.Reverse();
        return string.Join("/", segments);
    }

    private IEnumerator WaitForCondition(Func<bool> predicate, float timeoutSeconds)
    {
        float elapsed = 0f;
        while (elapsed < timeoutSeconds)
        {
            if (predicate())
            {
                yield break;
            }

            elapsed += ScenarioPollInterval;
            yield return new WaitForSecondsRealtime(ScenarioPollInterval);
        }
    }

    private static IEnumerator WaitFrames(int frameCount)
    {
        for (int i = 0; i < frameCount; i++)
        {
            yield return null;
        }
    }

    private void HandleItemPlaced(PlacementEventData eventData)
    {
        lastPlacementEvent = eventData;
    }

    private void HandleSaplingPlanted(SaplingPlantedEventData eventData)
    {
        lastSaplingEvent = eventData;
    }

    private Vector3 GetCandidateSearchOrigin(CandidatePreference preference)
    {
        if (preference == CandidatePreference.PreferImmediate && playerTransform != null)
        {
            return PlacementGridCalculator.GetCellCenter(playerTransform.position);
        }

        Vector3 hoverWorld = GetMouseWorldPosition();
        if (IsWorldPointVisible(hoverWorld))
        {
            return hoverWorld;
        }

        if (mainCamera != null)
        {
            Vector3 cameraCenter = mainCamera.ScreenToWorldPoint(
                new Vector3(mainCamera.pixelWidth * 0.5f, mainCamera.pixelHeight * 0.5f, -mainCamera.transform.position.z));
            return PlacementGridCalculator.GetCellCenter(cameraCenter);
        }

        return PlacementGridCalculator.GetCellCenter(playerTransform.position);
    }

    private Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null)
        {
            return PlacementGridCalculator.GetCellCenter(playerTransform.position);
        }

        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = -mainCamera.transform.position.z;
        return PlacementGridCalculator.GetCellCenter(mainCamera.ScreenToWorldPoint(mouseScreenPosition));
    }

    private bool IsWorldPointVisible(Vector3 worldPosition)
    {
        if (mainCamera == null)
        {
            return true;
        }

        Vector3 screenPoint = mainCamera.WorldToScreenPoint(worldPosition);
        return screenPoint.z >= 0f &&
               screenPoint.x >= 0f &&
               screenPoint.x <= mainCamera.pixelWidth &&
               screenPoint.y >= 0f &&
               screenPoint.y <= mainCamera.pixelHeight;
    }

    private void SetPlacementMode(bool isActive)
    {
        if (gameInputManager == null || GameInputPlacementModeSetter == null)
        {
            return;
        }

        GameInputPlacementModeSetter.Invoke(gameInputManager, new object[] { isActive });
    }

#if UNITY_EDITOR && UNITY_STANDALONE_WIN
    [StructLayout(LayoutKind.Sequential)]
    private struct CursorPoint
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out CursorPoint point);

    private bool TryMoveCursorToWorldPoint(Vector3 worldPosition)
    {
        if (mainCamera == null)
        {
            return false;
        }

        if (!TryGetGameViewWindowMapping(out EditorWindow gameView, out Rect windowRect, out float widthScale, out float heightScale))
        {
            return false;
        }

        gameView.Focus();

        Vector3 screenPoint = mainCamera.WorldToScreenPoint(worldPosition);
        float windowSpaceX = screenPoint.x * widthScale;
        float windowSpaceY = screenPoint.y * heightScale;

        int screenX = Mathf.RoundToInt(windowRect.x + windowSpaceX);
        int screenY = Mathf.RoundToInt(windowRect.y + (windowRect.height - windowSpaceY));
        bool moved = SetCursorPos(screenX, screenY);

        Debug.Log(
            $"{LogPrefix} move_cursor world={worldPosition} gameViewScreen={screenPoint} windowSpace=({windowSpaceX:F2}, {windowSpaceY:F2}) moved={moved}");
        return moved;
    }

    private bool TryNudgeCursorToWorldPoint(Vector3 targetWorld, Vector3 currentHoverWorld)
    {
        if (mainCamera == null || !TryGetGameViewWindowMapping(out _, out _, out float widthScale, out float heightScale))
        {
            return false;
        }

        Vector3 targetScreen = mainCamera.WorldToScreenPoint(targetWorld);
        Vector3 currentScreen = mainCamera.WorldToScreenPoint(currentHoverWorld);
        Vector3 deltaScreen = targetScreen - currentScreen;

        if (!GetCursorPos(out CursorPoint currentCursor))
        {
            return false;
        }

        int nudgedX = Mathf.RoundToInt(currentCursor.X + (deltaScreen.x * widthScale));
        int nudgedY = Mathf.RoundToInt(currentCursor.Y - (deltaScreen.y * heightScale));
        bool moved = SetCursorPos(nudgedX, nudgedY);

        Debug.Log(
            $"{LogPrefix} nudge_cursor target={targetWorld} hover={currentHoverWorld} deltaScreen={deltaScreen} moved={moved}");
        return moved;
    }

    private bool TryGetGameViewWindowMapping(out EditorWindow gameView, out Rect windowRect, out float widthScale, out float heightScale)
    {
        gameView = null;
        windowRect = default;
        widthScale = 1f;
        heightScale = 1f;

        Type gameViewType = Type.GetType("UnityEditor.GameView,UnityEditor");
        if (gameViewType == null)
        {
            return false;
        }

        UnityEngine.Object[] gameViews = Resources.FindObjectsOfTypeAll(gameViewType);
        if (gameViews == null || gameViews.Length == 0)
        {
            return false;
        }

        gameView = gameViews[0] as EditorWindow;
        if (gameView == null)
        {
            return false;
        }

        windowRect = gameView.position;
        widthScale = mainCamera.pixelWidth > 0f ? windowRect.width / mainCamera.pixelWidth : 1f;
        heightScale = mainCamera.pixelHeight > 0f ? windowRect.height / mainCamera.pixelHeight : 1f;
        return true;
    }
#else
    private bool TryMoveCursorToWorldPoint(Vector3 worldPosition)
    {
        return false;
    }

    private bool TryNudgeCursorToWorldPoint(Vector3 targetWorld, Vector3 currentHoverWorld)
    {
        return false;
    }
#endif
}
