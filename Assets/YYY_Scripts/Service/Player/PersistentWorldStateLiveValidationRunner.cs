using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using FarmGame.Data;
using FarmGame.Farm;
using FarmGame.World;
using Sunset.Story;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// P0-C world-state 主链 live 验证 Runner。
/// 只在 Play Mode 中使用，用来收 Primary / Home / Town 的箱子与农地 continuity 证据。
/// </summary>
public sealed class PersistentWorldStateLiveValidationRunner : MonoBehaviour
{
    private const string LogPrefix = "[WorldStateLive]";
    private const string PrimarySceneName = "Primary";
    private const string HomeSceneName = "Home";
    private const string TownSceneName = "Town";
    private const string PrimaryHomeEntryAnchorName = "PrimaryHomeEntryAnchor";
    private const string HomeEntryAnchorName = "HomeEntryAnchor";
    private const string TownOpeningEntryAnchorName = "TownPlayerEntryAnchor";
    private const int RequiredFarmCandidateCount = 2;
    private const float SceneTransitionTimeout = 30f;
    private const float SceneStateRestoreTimeout = 10f;
    private const float PollInterval = 0.05f;
    private const float WatchdogStallTimeout = 18f;

    [Serializable]
    private sealed class ValidationReport
    {
        public string timestamp = string.Empty;
        public string startScene = string.Empty;
        public bool allPassed;
        public List<ScenarioReport> scenarios = new List<ScenarioReport>();
    }

    [Serializable]
    private sealed class ScenarioReport
    {
        public string name = string.Empty;
        public bool passed;
        public string details = string.Empty;
    }

    private struct FarmCandidate
    {
        public int LayerIndex;
        public Vector3Int CellPosition;
        public Vector3 WorldPosition;
    }

    private Coroutine runCoroutine;
    private Coroutine watchdogCoroutine;
    private readonly ValidationReport report = new ValidationReport();
    private readonly object reportSync = new object();
    private Timer realtimeWatchdogTimer;

    private ItemDatabase database;
    private TimeManager timeManager;
    private PlayerMovement playerMovement;

    private string primaryChestGuid = string.Empty;
    private int primaryChestSlotIndex = -1;
    private ItemStack markerStackFirst;
    private ItemStack markerStackSecond;
    private ItemStack homeChestMarkerStack;
    private FarmCandidate firstFarmCandidate;
    private FarmCandidate secondFarmCandidate;
    private string homeChestGuid = string.Empty;
    private int homeChestSlotIndex = -1;
    private string firstCropGuid = string.Empty;
    private string secondCropGuid = string.Empty;
    private AsyncOperation pendingSceneLoadOperation;
    private string lastSceneLoadedName = string.Empty;
    private string lastActiveSceneName = string.Empty;
    private float lastProgressRealtime;
    private long lastProgressUtcTicks;
    private string currentScenarioName = string.Empty;
    private string currentProgressPhase = string.Empty;
    private bool finalized;

    public static string LatestReportPath => BuildReportPath();
    public static string AutostartCommandPath => BuildAutostartCommandPath();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoStartFromCommandFile()
    {
        string commandPath = BuildAutostartCommandPath();
        if (!File.Exists(commandPath))
        {
            return;
        }

        try
        {
            string command = File.ReadAllText(commandPath).Trim();
            File.Delete(commandPath);
            if (!string.Equals(command, "run", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"{LogPrefix} autostart_read_failed path={commandPath} message={exception.Message}");
            return;
        }

        BeginOrRestart();
    }

    public static PersistentWorldStateLiveValidationRunner BeginOrRestart()
    {
        PersistentWorldStateLiveValidationRunner runner =
            FindFirstObjectByType<PersistentWorldStateLiveValidationRunner>(FindObjectsInactive.Include);
        if (runner == null)
        {
            GameObject go = new GameObject(nameof(PersistentWorldStateLiveValidationRunner));
            runner = go.AddComponent<PersistentWorldStateLiveValidationRunner>();
        }

        runner.StartRun();
        return runner;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log($"{LogPrefix} awake instance={GetInstanceID()} parent={(transform.parent != null ? transform.parent.name : "null")} scene={gameObject.scene.name} active={gameObject.activeInHierarchy}");
    }

    private void Start()
    {
        if (runCoroutine == null)
        {
            StartRun();
        }
    }

    private void Update()
    {
        if (finalized)
        {
            return;
        }

        if (Time.realtimeSinceStartup - lastProgressRealtime > WatchdogStallTimeout)
        {
            TriggerWatchdogTimeout();
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.activeSceneChanged -= HandleActiveSceneChanged;
        FinalizeRunIfInterrupted("runner_disabled");
        Debug.Log($"{LogPrefix} disabled activeScene={SceneManager.GetActiveScene().name}");
    }

    private void OnDestroy()
    {
        FinalizeRunIfInterrupted("runner_destroyed");
        Debug.Log($"{LogPrefix} destroyed activeScene={SceneManager.GetActiveScene().name}");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        SceneManager.activeSceneChanged += HandleActiveSceneChanged;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        lastSceneLoadedName = scene.name;
        TouchProgress("scene_loaded", $"scene={scene.name} mode={mode}");
        Debug.Log($"{LogPrefix} scene_loaded instance={GetInstanceID()} loaded={scene.name} active={SceneManager.GetActiveScene().name} mode={mode} activeSelf={gameObject.activeInHierarchy}");

    }

    private void HandleActiveSceneChanged(Scene previous, Scene next)
    {
        lastActiveSceneName = next.name;
        TouchProgress("active_scene_changed", $"from={previous.name} to={next.name}");
        Debug.Log($"{LogPrefix} active_scene_changed instance={GetInstanceID()} from={previous.name} to={next.name} activeSelf={gameObject.activeInHierarchy}");
    }

    private void StartRun()
    {
        if (runCoroutine != null)
        {
            StopCoroutine(runCoroutine);
        }

        if (watchdogCoroutine != null)
        {
            StopCoroutine(watchdogCoroutine);
        }

        DisposeRealtimeWatchdogTimer();

        finalized = false;
        currentScenarioName = string.Empty;
        TouchProgress("run_started", $"activeScene={SceneManager.GetActiveScene().name}");
        Debug.Log($"{LogPrefix} run_started instance={GetInstanceID()} activeScene={SceneManager.GetActiveScene().name} hadExistingCoroutine={(runCoroutine != null)}");
        watchdogCoroutine = StartCoroutine(WatchdogRun());
        realtimeWatchdogTimer = new Timer(HandleRealtimeWatchdogTick, null, 1000, 1000);
        runCoroutine = StartCoroutine(RunValidation());
    }

    private IEnumerator RunValidation()
    {
        report.timestamp = DateTime.Now.ToString("O");
        report.startScene = SceneManager.GetActiveScene().name;
        report.scenarios.Clear();
        report.allPassed = false;

        if (!ResolveRuntimeReferences(out string resolveReason))
        {
            FinishScenario("bootstrap", false, resolveReason);
            FinalizeRun();
            yield break;
        }

        if (!string.Equals(SceneManager.GetActiveScene().name, PrimarySceneName, StringComparison.Ordinal))
        {
            FinishScenario("bootstrap", false, $"active_scene={SceneManager.GetActiveScene().name} expected={PrimarySceneName}");
            FinalizeRun();
            yield break;
        }

        yield return WaitForBootstrapTargets();
        if (!LastScenarioPassed())
        {
            FinalizeRun();
            yield break;
        }

        TouchProgress("bootstrap_completed", SceneManager.GetActiveScene().name);
        Debug.Log($"{LogPrefix} bootstrap_completed active={SceneManager.GetActiveScene().name}");
        TouchProgress("scenario_chain_dispatch", SceneManager.GetActiveScene().name);
        yield return RunScenarioSequence();
    }

    private IEnumerator RunScenarioSequence()
    {
        TouchProgress("scenario_chain_started", SceneManager.GetActiveScene().name);

        yield return RunPrimaryHomePrimaryScenario();
        if (!LastScenarioPassed())
        {
            FinalizeRun();
            yield break;
        }

        yield return RunPrimaryTownPrimaryScenario();
        if (!LastScenarioPassed())
        {
            FinalizeRun();
            yield break;
        }

        yield return RunHomePrimaryHomeChestScenario();

        bool allPassed = true;
        for (int index = 0; index < report.scenarios.Count; index++)
        {
            ScenarioReport scenario = report.scenarios[index];
            allPassed &= scenario != null && scenario.passed;
        }

        report.allPassed = allPassed;
        FinalizeRun();
    }

    private IEnumerator RunNestedEnumerator(IEnumerator routine)
    {
        if (routine == null)
        {
            yield break;
        }

        Stack<IEnumerator> stack = new Stack<IEnumerator>();
        stack.Push(routine);

        while (stack.Count > 0)
        {
            IEnumerator active = stack.Peek();
            bool hasNext;
            object current = null;

            try
            {
                hasNext = active.MoveNext();
                if (hasNext)
                {
                    current = active.Current;
                }
            }
            catch (Exception exception)
            {
                string scenarioName = string.IsNullOrWhiteSpace(currentScenarioName)
                    ? "runner-exception"
                    : currentScenarioName;
                FinishScenario(
                    scenarioName,
                    false,
                    $"exception={exception.GetType().Name}:{exception.Message}|phase={currentProgressPhase}|active={SceneManager.GetActiveScene().name}");
                report.allPassed = false;
                FinalizeRun();
                yield break;
            }

            if (!hasNext)
            {
                stack.Pop();
                continue;
            }

            if (current is IEnumerator nested)
            {
                stack.Push(nested);
                continue;
            }

            yield return current;
        }
    }

    private IEnumerator RunPrimaryHomePrimaryScenario()
    {
        const string scenarioName = "primary-home-primary";
        currentScenarioName = scenarioName;
        TouchProgress("scenario_start", scenarioName);
        Debug.Log($"{LogPrefix} scenario_start={scenarioName}");
        TouchProgress("prepare_primary_state", scenarioName);

        if (!TryPreparePrimaryState(firstFarmCandidate, markerStackFirst, out firstCropGuid, out string prepareReason))
        {
            FinishScenario(scenarioName, false, prepareReason);
            yield break;
        }

        Debug.Log($"{LogPrefix} primary_state_prepared scenario={scenarioName} chest={FormatPrimaryChestExpectation(markerStackFirst)} cell={FormatCell(firstFarmCandidate)} cropGuid={firstCropGuid}");

        if (!TryStartSceneTransition(HomeSceneName, out string transitionReason))
        {
            FinishScenario(scenarioName, false, transitionReason);
            yield break;
        }

        Debug.Log($"{LogPrefix} transition_started scenario={scenarioName} target={HomeSceneName}");

        yield return WaitForScene(HomeSceneName, SceneTransitionTimeout, scenarioName);
        if (!string.Equals(SceneManager.GetActiveScene().name, HomeSceneName, StringComparison.Ordinal))
        {
            yield break;
        }

        if (!TryStartSceneTransition(PrimarySceneName, out transitionReason))
        {
            FinishScenario(scenarioName, false, transitionReason);
            yield break;
        }

        Debug.Log($"{LogPrefix} transition_started scenario={scenarioName} target={PrimarySceneName}");

        yield return WaitForScene(PrimarySceneName, SceneTransitionTimeout, scenarioName);
        if (!string.Equals(SceneManager.GetActiveScene().name, PrimarySceneName, StringComparison.Ordinal))
        {
            yield break;
        }

        yield return WaitForPrimaryState(
            scenarioName,
            () => DoesPrimaryStateMatch(markerStackFirst, firstFarmCandidate, firstCropGuid, includeSecondCandidate: false),
            $"chest={FormatPrimaryChestExpectation(markerStackFirst)} firstCell={FormatCell(firstFarmCandidate)} cropGuid={firstCropGuid}",
            () => DescribePrimaryStateMismatch(markerStackFirst, includeSecondCandidate: false));
    }

    private IEnumerator RunHomePrimaryHomeChestScenario()
    {
        const string scenarioName = "home-primary-home-chest";
        currentScenarioName = scenarioName;
        TouchProgress("scenario_start", scenarioName);
        Debug.Log($"{LogPrefix} scenario_start={scenarioName}");

        if (!TryStartSceneTransition(HomeSceneName, out string transitionReason))
        {
            FinishScenario(scenarioName, false, transitionReason);
            yield break;
        }

        Debug.Log($"{LogPrefix} transition_started scenario={scenarioName} target={HomeSceneName}");

        yield return WaitForScene(HomeSceneName, SceneTransitionTimeout, scenarioName);
        if (!string.Equals(SceneManager.GetActiveScene().name, HomeSceneName, StringComparison.Ordinal))
        {
            yield break;
        }

        if (!TryResolveSceneChest(HomeSceneName, out ChestController chest, out string resolveReason))
        {
            FinishScenario(scenarioName, false, resolveReason);
            yield break;
        }

        homeChestGuid = chest.PersistentId;
        homeChestSlotIndex = Mathf.Max(0, chest.RuntimeInventory.Capacity - 1);
        chest.SetSlot(homeChestSlotIndex, homeChestMarkerStack);

        if (!TryStartSceneTransition(PrimarySceneName, out transitionReason))
        {
            FinishScenario(scenarioName, false, transitionReason);
            yield break;
        }

        Debug.Log($"{LogPrefix} transition_started scenario={scenarioName} target={PrimarySceneName}");

        yield return WaitForScene(PrimarySceneName, SceneTransitionTimeout, scenarioName);
        if (!string.Equals(SceneManager.GetActiveScene().name, PrimarySceneName, StringComparison.Ordinal))
        {
            yield break;
        }

        if (!TryStartSceneTransition(HomeSceneName, out transitionReason))
        {
            FinishScenario(scenarioName, false, transitionReason);
            yield break;
        }

        Debug.Log($"{LogPrefix} transition_started scenario={scenarioName} target={HomeSceneName}");

        yield return WaitForScene(HomeSceneName, SceneTransitionTimeout, scenarioName);
        if (!string.Equals(SceneManager.GetActiveScene().name, HomeSceneName, StringComparison.Ordinal))
        {
            yield break;
        }

        yield return WaitForSceneState(
            scenarioName,
            () => DoesSceneChestStateMatch(HomeSceneName, homeChestGuid, homeChestSlotIndex, homeChestMarkerStack),
            $"scene={HomeSceneName} chest={FormatItemStack(homeChestMarkerStack)} guid={homeChestGuid}");
    }

    private IEnumerator RunPrimaryTownPrimaryScenario()
    {
        const string scenarioName = "primary-town-primary";
        currentScenarioName = scenarioName;
        TouchProgress("scenario_start", scenarioName);
        Debug.Log($"{LogPrefix} scenario_start={scenarioName}");
        TouchProgress("prepare_primary_state", scenarioName);

        if (!TryPreparePrimaryState(secondFarmCandidate, markerStackSecond, out secondCropGuid, out string prepareReason))
        {
            FinishScenario(scenarioName, false, prepareReason);
            yield break;
        }

        Debug.Log($"{LogPrefix} primary_state_prepared scenario={scenarioName} chest={FormatPrimaryChestExpectation(markerStackSecond)} cell={FormatCell(secondFarmCandidate)} cropGuid={secondCropGuid}");

        if (!TryStartSceneTransition(TownSceneName, out string transitionReason))
        {
            FinishScenario(scenarioName, false, transitionReason);
            yield break;
        }

        Debug.Log($"{LogPrefix} transition_started scenario={scenarioName} target={TownSceneName}");

        yield return WaitForScene(TownSceneName, SceneTransitionTimeout, scenarioName);
        if (!string.Equals(SceneManager.GetActiveScene().name, TownSceneName, StringComparison.Ordinal))
        {
            yield break;
        }

        if (!TryStartSceneTransition(PrimarySceneName, out transitionReason))
        {
            FinishScenario(scenarioName, false, transitionReason);
            yield break;
        }

        Debug.Log($"{LogPrefix} transition_started scenario={scenarioName} target={PrimarySceneName}");

        yield return WaitForScene(PrimarySceneName, SceneTransitionTimeout, scenarioName);
        if (!string.Equals(SceneManager.GetActiveScene().name, PrimarySceneName, StringComparison.Ordinal))
        {
            yield break;
        }

        yield return WaitForPrimaryState(
            scenarioName,
            () => DoesPrimaryStateMatch(markerStackSecond, firstFarmCandidate, firstCropGuid, includeSecondCandidate: true),
            $"chest={FormatPrimaryChestExpectation(markerStackSecond)} firstCell={FormatCell(firstFarmCandidate)} secondCell={FormatCell(secondFarmCandidate)} secondCropGuid={secondCropGuid}",
            () => DescribePrimaryStateMismatch(markerStackSecond, includeSecondCandidate: true));
    }

    private bool ResolveRuntimeReferences(out string reason)
    {
        database = AssetLocator.LoadItemDatabase();
        timeManager = TimeManager.Instance ?? FindFirstObjectByType<TimeManager>(FindObjectsInactive.Include);
        playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);

        if (database == null)
        {
            reason = "item_database_missing=true";
            return false;
        }

        reason = string.Empty;
        return true;
    }

    private IEnumerator WaitForBootstrapTargets()
    {
        float deadline = Time.realtimeSinceStartup + 8f;
        while (Time.realtimeSinceStartup < deadline)
        {
            bool referencesReady = ResolveRuntimeReferences(out string resolveReason);
            string targetReason = string.Empty;
            bool targetsReady = referencesReady && TryResolvePrimaryTargets(out targetReason);
            if (referencesReady && targetsReady)
            {
                FinishScenario("bootstrap", true, "targets_ready=true");
                yield break;
            }

            string lastReason = !string.IsNullOrWhiteSpace(resolveReason) ? resolveReason : targetReason;
            if (Time.frameCount % 30 == 0)
            {
                Debug.Log($"{LogPrefix} bootstrap_waiting reason={lastReason}");
            }

            yield return null;
        }

        string failureReason = ResolveRuntimeReferences(out string finalResolveReason)
            ? (TryResolvePrimaryTargets(out string finalTargetReason) ? "unknown_bootstrap_failure" : finalTargetReason)
            : finalResolveReason;
        FinishScenario("bootstrap", false, failureReason);
    }

    private bool TryResolvePrimaryTargets(out string reason)
    {
        if (!TryResolveMarkerSeed(out SeedData markerSeed, out reason))
        {
            return false;
        }

        markerStackFirst = new ItemStack(markerSeed.itemID, 0, 3);
        markerStackSecond = new ItemStack(markerSeed.itemID, 0, 5);
        homeChestMarkerStack = new ItemStack(markerSeed.itemID, 0, 7);

        if (!TryResolvePrimaryChest(out ChestController chest, out string chestResolveReason))
        {
            Debug.LogWarning($"{LogPrefix} primary_chest_optional_bootstrap_skip reason={chestResolveReason}");
            primaryChestGuid = string.Empty;
            primaryChestSlotIndex = -1;
        }
        else
        {
            primaryChestGuid = chest.PersistentId;
            primaryChestSlotIndex = Mathf.Max(0, chest.RuntimeInventory.Capacity - 1);
        }

        if (!TryFindPrimaryFarmCandidates(RequiredFarmCandidateCount, out List<FarmCandidate> candidates, out reason))
        {
            return false;
        }

        firstFarmCandidate = candidates[0];
        secondFarmCandidate = candidates[1];
        return true;
    }

    private bool TryResolvePrimaryChest(out ChestController chest, out string reason)
    {
        return TryResolveSceneChest(PrimarySceneName, out chest, out reason);
    }

    private bool TryResolveSceneChest(string sceneName, out ChestController chest, out string reason)
    {
        chest = null;
        reason = $"{sceneName.ToLowerInvariant()}_chest_missing=true";

        ChestController[] chests = FindObjectsByType<ChestController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int index = 0; index < chests.Length; index++)
        {
            ChestController candidate = chests[index];
            if (candidate == null
                || candidate.RuntimeInventory == null
                || !string.Equals(candidate.gameObject.scene.name, sceneName, StringComparison.Ordinal))
            {
                continue;
            }

            chest = candidate;
            reason = string.Empty;
            return true;
        }

        return false;
    }

    private bool TryResolveMarkerSeed(out SeedData seed, out string reason)
    {
        seed = null;
        reason = "marker_seed_missing=true";

        List<SeedData> seeds = database.GetAllSeeds();
        for (int index = 0; index < seeds.Count; index++)
        {
            SeedData candidate = seeds[index];
            if (candidate == null || candidate.cropPrefab == null)
            {
                continue;
            }

            seed = candidate;
            reason = string.Empty;
            return true;
        }

        return false;
    }

    private bool TryFindPrimaryFarmCandidates(int requiredCount, out List<FarmCandidate> candidates, out string reason)
    {
        candidates = new List<FarmCandidate>();
        reason = "farm_tile_manager_missing=true";

        FarmTileManager farmTileManager = FarmTileManager.Instance ?? FindFirstObjectByType<FarmTileManager>(FindObjectsInactive.Include);
        if (farmTileManager == null)
        {
            return false;
        }

        Vector3 origin = playerMovement != null ? playerMovement.transform.position : Vector3.zero;
        int layerIndex = farmTileManager.GetCurrentLayerIndex(origin);
        LayerTilemaps tilemaps = farmTileManager.GetLayerTilemaps(layerIndex);
        if (tilemaps == null || tilemaps.groundTilemap == null)
        {
            reason = $"farm_layer_missing=true layer={layerIndex}";
            return false;
        }

        Vector3Int originCell = tilemaps.WorldToCell(origin);
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        for (int radius = 0; radius <= 12 && candidates.Count < requiredCount; radius++)
        {
            for (int x = -radius; x <= radius && candidates.Count < requiredCount; x++)
            {
                for (int y = -radius; y <= radius && candidates.Count < requiredCount; y++)
                {
                    if (radius > 0 && Mathf.Abs(x) != radius && Mathf.Abs(y) != radius)
                    {
                        continue;
                    }

                    Vector3Int cell = originCell + new Vector3Int(x, y, 0);
                    if (!visited.Add(cell) || !tilemaps.groundTilemap.HasTile(cell))
                    {
                        continue;
                    }

                    if (!farmTileManager.CanTillAt(layerIndex, cell))
                    {
                        continue;
                    }

                    candidates.Add(new FarmCandidate
                    {
                        LayerIndex = layerIndex,
                        CellPosition = cell,
                        WorldPosition = tilemaps.GetCellCenterWorld(cell)
                    });
                }
            }
        }

        if (candidates.Count < requiredCount)
        {
            reason = $"farm_candidates_insufficient found={candidates.Count} required={requiredCount} layer={layerIndex}";
            return false;
        }

        reason = string.Empty;
        return true;
    }

    private bool TryPreparePrimaryState(
        FarmCandidate candidate,
        ItemStack markerStack,
        out string cropGuid,
        out string reason)
    {
        cropGuid = string.Empty;
        reason = string.Empty;

        if (!string.Equals(SceneManager.GetActiveScene().name, PrimarySceneName, StringComparison.Ordinal))
        {
            reason = $"prepare_scene_mismatch active={SceneManager.GetActiveScene().name}";
            return false;
        }

        ChestController chest = null;
        if (!string.IsNullOrWhiteSpace(primaryChestGuid))
        {
            chest = FindChestByGuid(primaryChestGuid);
            if (chest == null)
            {
                reason = $"primary_chest_rebind_failed guid={primaryChestGuid}";
                return false;
            }

            chest.SetSlot(primaryChestSlotIndex, markerStack);
        }

        FarmTileManager farmTileManager = FarmTileManager.Instance ?? FindFirstObjectByType<FarmTileManager>(FindObjectsInactive.Include);
        if (farmTileManager == null)
        {
            reason = "farm_tile_manager_missing=true";
            return false;
        }

        if (!farmTileManager.CreateTile(candidate.LayerIndex, candidate.CellPosition)
            && farmTileManager.GetTileData(candidate.LayerIndex, candidate.CellPosition) == null)
        {
            reason = $"create_tile_failed cell={FormatCell(candidate)}";
            return false;
        }

        float waterTime = ResolveWaterTime();
        if (!farmTileManager.SetWatered(candidate.LayerIndex, candidate.CellPosition, waterTime))
        {
            reason = $"set_watered_failed cell={FormatCell(candidate)} waterTime={waterTime:F2}";
            return false;
        }

        FarmTileData tileData = farmTileManager.GetTileData(candidate.LayerIndex, candidate.CellPosition);
        if (tileData == null)
        {
            reason = $"tile_data_missing_after_create cell={FormatCell(candidate)}";
            return false;
        }

        if (tileData.HasCrop())
        {
            cropGuid = tileData.cropController != null ? tileData.cropController.PersistentId : string.Empty;
            return true;
        }

        if (!TryResolveMarkerSeed(out SeedData seed, out reason))
        {
            return false;
        }

        LayerTilemaps tilemaps = farmTileManager.GetLayerTilemaps(candidate.LayerIndex);
        if (tilemaps == null)
        {
            reason = $"layer_tilemaps_missing layer={candidate.LayerIndex}";
            return false;
        }

        Transform container = tilemaps.propsContainer;
        GameObject cropRoot = Instantiate(seed.cropPrefab, candidate.WorldPosition, Quaternion.identity, container);
        cropRoot.name = $"WorldStateLiveCrop_{seed.itemName}_{candidate.CellPosition.x}_{candidate.CellPosition.y}";

        CropController cropController = cropRoot.GetComponentInChildren<CropController>();
        if (cropController == null)
        {
            Destroy(cropRoot);
            reason = $"crop_controller_missing seed={seed.itemName}";
            return false;
        }

        int currentDay = timeManager != null ? timeManager.GetTotalDaysPassed() : 0;
        CropInstanceData instanceData = new CropInstanceData(seed.itemID, currentDay);
        cropController.Initialize(seed, instanceData, candidate.LayerIndex, candidate.CellPosition);
        tileData.SetCropData(instanceData);
        cropGuid = cropController.PersistentId;

        return !string.IsNullOrWhiteSpace(cropGuid);
    }

    private bool TryStartSceneTransition(string targetSceneName, out string reason)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (!TryResolveSceneBuildIndex(targetSceneName, out int buildIndex, out string scenePath))
        {
            reason = $"transition_path_missing scene={activeScene.name} target={targetSceneName}";
            return false;
        }

        lastSceneLoadedName = string.Empty;
        lastActiveSceneName = string.Empty;
        string entryAnchor = ResolveEntryAnchorName(activeScene.name, targetSceneName);
        PersistentPlayerSceneBridge.QueueSceneEntry(targetSceneName, scenePath, entryAnchor);

        pendingSceneLoadOperation = buildIndex >= 0
            ? SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single)
            : SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Single);
        if (pendingSceneLoadOperation == null)
        {
            reason = $"transition_load_start_failed scene={activeScene.name} target={targetSceneName}";
            return false;
        }

        TouchProgress("transition_started", $"from={activeScene.name} to={targetSceneName}");
        reason = string.Empty;
        return true;
    }

    private IEnumerator WaitForScene(string expectedSceneName, float timeout, string scenarioName)
    {
        Debug.Log($"{LogPrefix} wait_scene_begin scenario={scenarioName} expected={expectedSceneName} active={SceneManager.GetActiveScene().name} pendingLoad={(pendingSceneLoadOperation == null ? "null" : "set")} instance={GetInstanceID()}");
        AsyncOperation loadOperation = pendingSceneLoadOperation;
        if (loadOperation != null)
        {
            // 直接跟住这次 Single-load 的 AsyncOperation，避免跨场景后纯轮询协程失去恢复点。
            yield return loadOperation;
        }

        float deadline = Time.realtimeSinceStartup + Mathf.Max(1f, timeout * 0.25f);
        while (Time.realtimeSinceStartup < deadline)
        {
            bool activeMatched = string.Equals(SceneManager.GetActiveScene().name, expectedSceneName, StringComparison.Ordinal);
            bool loadedMatched = string.Equals(lastSceneLoadedName, expectedSceneName, StringComparison.Ordinal);
            bool activeChangedMatched = string.Equals(lastActiveSceneName, expectedSceneName, StringComparison.Ordinal);
            if (activeMatched || loadedMatched || activeChangedMatched)
            {
                Debug.Log($"{LogPrefix} scene_reached scenario={scenarioName} scene={expectedSceneName} via={(activeMatched ? "active" : loadedMatched ? "sceneLoaded" : "activeSceneChanged")} instance={GetInstanceID()}");
                pendingSceneLoadOperation = null;
                TouchProgress("scene_reached", $"{scenarioName}:{expectedSceneName}");
                yield return WaitForSceneSettle(expectedSceneName, scenarioName);
                yield break;
            }

            yield return new WaitForSecondsRealtime(PollInterval);
        }

        FinishScenario(
            scenarioName,
            false,
            $"scene_timeout expected={expectedSceneName} active={SceneManager.GetActiveScene().name} pendingLoad={(pendingSceneLoadOperation == null ? "null" : "set")}");
    }

    private IEnumerator WaitForSceneSettle(string expectedSceneName, string scenarioName)
    {
        const int settleFrames = 3;
        for (int frame = 0; frame < settleFrames; frame++)
        {
            if (!string.Equals(SceneManager.GetActiveScene().name, expectedSceneName, StringComparison.Ordinal))
            {
                TouchProgress("scene_settle_drift", $"{scenarioName}:{expectedSceneName}->{SceneManager.GetActiveScene().name}");
                yield break;
            }

            TouchProgress("scene_settle", $"{scenarioName}:{expectedSceneName}:frame={frame + 1}");
            yield return null;
        }

        TouchProgress("scene_settled", $"{scenarioName}:{expectedSceneName}");
    }

    private IEnumerator WaitForPrimaryState(
        string scenarioName,
        Func<bool> predicate,
        string successDetails,
        Func<string> failureDetailsFactory)
    {
        return WaitForSceneState(scenarioName, predicate, successDetails, failureDetailsFactory);
    }

    private IEnumerator WaitForSceneState(
        string scenarioName,
        Func<bool> predicate,
        string successDetails,
        Func<string> failureDetailsFactory = null)
    {
        float deadline = Time.realtimeSinceStartup + SceneStateRestoreTimeout;
        while (Time.realtimeSinceStartup < deadline)
        {
            if (predicate())
            {
                FinishScenario(scenarioName, true, successDetails);
                yield break;
            }

            yield return new WaitForSecondsRealtime(PollInterval);
        }

        string failureDetails = failureDetailsFactory != null
            ? failureDetailsFactory()
            : successDetails;
        FinishScenario(scenarioName, false, $"state_timeout details={failureDetails}");
    }

    private bool DoesPrimaryStateMatch(
        ItemStack expectedChestStack,
        FarmCandidate firstCandidateToCheck,
        string firstExpectedCropGuid,
        bool includeSecondCandidate)
    {
        if (!string.IsNullOrWhiteSpace(primaryChestGuid)
            && !DoesSceneChestStateMatch(PrimarySceneName, primaryChestGuid, primaryChestSlotIndex, expectedChestStack))
        {
            return false;
        }

        if (!DoesFarmCandidateMatch(firstCandidateToCheck, firstExpectedCropGuid))
        {
            return false;
        }

        if (!includeSecondCandidate)
        {
            return true;
        }

        return DoesFarmCandidateMatch(secondFarmCandidate, secondCropGuid);
    }

    private bool DoesSceneChestStateMatch(string sceneName, string chestGuid, int slotIndex, ItemStack expectedStack)
    {
        if (string.IsNullOrWhiteSpace(chestGuid) || slotIndex < 0)
        {
            return false;
        }

        ChestController chest = FindChestByGuid(chestGuid);
        if (chest == null || !string.Equals(chest.gameObject.scene.name, sceneName, StringComparison.Ordinal))
        {
            return false;
        }

        return ItemStacksEqual(chest.GetSlot(slotIndex), expectedStack);
    }

    private bool DoesFarmCandidateMatch(FarmCandidate candidate, string expectedCropGuid)
    {
        FarmTileManager farmTileManager = FarmTileManager.Instance ?? FindFirstObjectByType<FarmTileManager>(FindObjectsInactive.Include);
        if (farmTileManager == null)
        {
            return false;
        }

        FarmTileData tileData = farmTileManager.GetTileData(candidate.LayerIndex, candidate.CellPosition);
        if (tileData == null || !tileData.isTilled || !tileData.wateredToday || tileData.cropData == null)
        {
            return false;
        }

        CropController crop = FindCropByGuid(expectedCropGuid);
        if (crop == null || !crop.gameObject.activeInHierarchy)
        {
            return false;
        }

        return tileData.cropController == crop
            || string.Equals(tileData.cropController?.PersistentId, expectedCropGuid, StringComparison.Ordinal);
    }

    private ChestController FindChestByGuid(string guid)
    {
        if (string.IsNullOrWhiteSpace(guid))
        {
            return null;
        }

        ChestController[] chests = FindObjectsByType<ChestController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int index = 0; index < chests.Length; index++)
        {
            ChestController chest = chests[index];
            if (chest != null
                && chest.gameObject.scene == SceneManager.GetActiveScene()
                && string.Equals(chest.PersistentId, guid, StringComparison.Ordinal))
            {
                return chest;
            }
        }

        return null;
    }

    private CropController FindCropByGuid(string guid)
    {
        if (string.IsNullOrWhiteSpace(guid))
        {
            return null;
        }

        CropController[] crops = FindObjectsByType<CropController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int index = 0; index < crops.Length; index++)
        {
            CropController crop = crops[index];
            if (crop != null
                && crop.gameObject.scene == SceneManager.GetActiveScene()
                && string.Equals(crop.PersistentId, guid, StringComparison.Ordinal))
            {
                return crop;
            }
        }

        return null;
    }

    private float ResolveWaterTime()
    {
        if (timeManager == null)
        {
            return 8f;
        }

        return timeManager.GetHour() + (timeManager.GetMinute() / 60f);
    }

    private void FinishScenario(string scenarioName, bool passed, string details)
    {
        ScenarioReport scenario = new ScenarioReport
        {
            name = scenarioName,
            passed = passed,
            details = details ?? string.Empty
        };

        lock (reportSync)
        {
            report.scenarios.Add(scenario);
        }
        currentScenarioName = scenarioName;
        TouchProgress("scenario_finished", $"{scenarioName}:{(passed ? "passed" : "failed")}");
        Debug.Log($"{LogPrefix} scenario={scenarioName} passed={passed} details={scenario.details}");
    }

    private void FinalizeRun()
    {
        string reportPath;
        int scenarioCount;

        lock (reportSync)
        {
            if (finalized)
            {
                return;
            }

            finalized = true;
            reportPath = BuildReportPath();
            scenarioCount = report.scenarios.Count;
            TryWriteReportUnsafe(reportPath);
        }

        DisposeRealtimeWatchdogTimer();
        if (watchdogCoroutine != null)
        {
            StopCoroutine(watchdogCoroutine);
            watchdogCoroutine = null;
        }

        Debug.Log($"{LogPrefix} finalizing report={reportPath} scenarioCount={scenarioCount}");
        Debug.Log($"{LogPrefix} all_completed={report.allPassed} scenario_count={scenarioCount} report={reportPath}");
        runCoroutine = null;
    }

    private void FinalizeRunIfInterrupted(string reason)
    {
        if (finalized)
        {
            return;
        }

        if (!HasScenarioRecord("runner-interrupted"))
        {
            FinishScenario(
                "runner-interrupted",
                false,
                $"{reason}|phase={currentProgressPhase}|active={SceneManager.GetActiveScene().name}");
        }

        report.allPassed = false;
        FinalizeRun();
    }

    private IEnumerator WatchdogRun()
    {
        while (!finalized)
        {
            if (Time.realtimeSinceStartup - lastProgressRealtime > WatchdogStallTimeout)
            {
                TriggerWatchdogTimeout();
                yield break;
            }

            yield return null;
        }
    }

    private void TriggerWatchdogTimeout()
    {
        string scenarioName = string.IsNullOrWhiteSpace(currentScenarioName)
            ? "watchdog"
            : currentScenarioName;
        string details =
            $"watchdog_timeout phase={currentProgressPhase} active={SceneManager.GetActiveScene().name} " +
            $"lastSceneLoaded={lastSceneLoadedName} lastActiveScene={lastActiveSceneName}";

        if (!HasScenarioRecord(scenarioName))
        {
            FinishScenario(scenarioName, false, details);
        }

        report.allPassed = false;
        if (runCoroutine != null)
        {
            StopCoroutine(runCoroutine);
            runCoroutine = null;
        }

        if (watchdogCoroutine != null)
        {
            StopCoroutine(watchdogCoroutine);
            watchdogCoroutine = null;
        }

        FinalizeRun();
    }

    private void HandleRealtimeWatchdogTick(object _)
    {
        long lastTicks = Interlocked.Read(ref lastProgressUtcTicks);
        if (lastTicks <= 0)
        {
            return;
        }

        if (DateTime.UtcNow.Ticks - lastTicks <= TimeSpan.FromSeconds(WatchdogStallTimeout).Ticks)
        {
            return;
        }

        string reportPath;
        lock (reportSync)
        {
            if (finalized)
            {
                return;
            }

            string scenarioName = string.IsNullOrWhiteSpace(currentScenarioName)
                ? "watchdog"
                : currentScenarioName;
            string activeSceneName = !string.IsNullOrWhiteSpace(lastActiveSceneName)
                ? lastActiveSceneName
                : !string.IsNullOrWhiteSpace(lastSceneLoadedName)
                    ? lastSceneLoadedName
                    : report.startScene;
            string details =
                $"watchdog_timeout phase={currentProgressPhase} active={activeSceneName} " +
                $"lastSceneLoaded={lastSceneLoadedName} lastActiveScene={lastActiveSceneName}";

            if (!HasScenarioRecordUnsafe(scenarioName))
            {
                report.scenarios.Add(new ScenarioReport
                {
                    name = scenarioName,
                    passed = false,
                    details = details
                });
            }

            report.allPassed = false;
            finalized = true;
            reportPath = BuildReportPath();
            TryWriteReportUnsafe(reportPath);
        }

        DisposeRealtimeWatchdogTimer();
    }

    private bool HasScenarioRecordUnsafe(string scenarioName)
    {
        for (int index = 0; index < report.scenarios.Count; index++)
        {
            ScenarioReport scenario = report.scenarios[index];
            if (scenario != null && string.Equals(scenario.name, scenarioName, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private void TryWriteReportUnsafe(string reportPath)
    {
        try
        {
            string directory = Path.GetDirectoryName(reportPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(reportPath, JsonUtility.ToJson(report, true), new UTF8Encoding(false));
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"{LogPrefix} write_report_failed path={reportPath} message={exception.Message}");
        }
    }

    private void DisposeRealtimeWatchdogTimer()
    {
        if (realtimeWatchdogTimer == null)
        {
            return;
        }

        realtimeWatchdogTimer.Dispose();
        realtimeWatchdogTimer = null;
    }

    private bool HasScenarioRecord(string scenarioName)
    {
        lock (reportSync)
        {
            return HasScenarioRecordUnsafe(scenarioName);
        }
    }

    private void TouchProgress(string phase, string details = null)
    {
        lastProgressRealtime = Time.realtimeSinceStartup;
        Interlocked.Exchange(ref lastProgressUtcTicks, DateTime.UtcNow.Ticks);
        currentProgressPhase = string.IsNullOrWhiteSpace(details)
            ? phase
            : $"{phase}|{details}";
    }

    private bool LastScenarioPassed()
    {
        if (report.scenarios.Count <= 0)
        {
            return false;
        }

        ScenarioReport last = report.scenarios[report.scenarios.Count - 1];
        return last != null && last.passed;
    }

    private static string BuildReportPath()
    {
        string root = Directory.GetParent(Application.dataPath)?.FullName ?? Application.persistentDataPath;
        return Path.Combine(root, "Library", "CodexEditorCommands", "world-state-live-validation.json");
    }

    private static string BuildAutostartCommandPath()
    {
        string root = Directory.GetParent(Application.dataPath)?.FullName ?? Application.persistentDataPath;
        return Path.Combine(root, "Library", "CodexEditorCommands", "world-state-live-validation.command");
    }

    private static bool ItemStacksEqual(ItemStack left, ItemStack right)
    {
        if (left.IsEmpty && right.IsEmpty)
        {
            return true;
        }

        return left.itemId == right.itemId
            && left.quality == right.quality
            && left.amount == right.amount;
    }

    private static string FormatItemStack(ItemStack stack)
    {
        return stack.IsEmpty
            ? "Empty"
            : $"itemId={stack.itemId},quality={stack.quality},amount={stack.amount}";
    }

    private string FormatPrimaryChestExpectation(ItemStack stack)
    {
        return string.IsNullOrWhiteSpace(primaryChestGuid)
            ? "skipped:no_primary_chest"
            : FormatItemStack(stack);
    }

    private static string FormatCell(FarmCandidate candidate)
    {
        return $"layer={candidate.LayerIndex},cell=({candidate.CellPosition.x},{candidate.CellPosition.y})";
    }

    private string DescribePrimaryStateMismatch(ItemStack expectedChestStack, bool includeSecondCandidate)
    {
        List<string> mismatches = new List<string>();

        if (!string.IsNullOrWhiteSpace(primaryChestGuid))
        {
            mismatches.Add(DescribeSceneChestMismatch(PrimarySceneName, primaryChestGuid, primaryChestSlotIndex, expectedChestStack));
        }

        mismatches.Add(DescribeFarmCandidateMismatch("first", firstFarmCandidate, firstCropGuid));
        if (includeSecondCandidate && !string.IsNullOrWhiteSpace(secondCropGuid))
        {
            mismatches.Add(DescribeFarmCandidateMismatch("second", secondFarmCandidate, secondCropGuid));
        }

        return string.Join(" | ", mismatches.FindAll(entry => !string.IsNullOrWhiteSpace(entry)));
    }

    private string DescribeSceneChestMismatch(string sceneName, string chestGuid, int slotIndex, ItemStack expectedStack)
    {
        ChestController chest = FindChestByGuid(chestGuid);
        if (chest == null)
        {
            return $"chest_missing guid={chestGuid}";
        }

        if (!string.Equals(chest.gameObject.scene.name, sceneName, StringComparison.Ordinal))
        {
            return $"chest_scene_mismatch expected={sceneName} actual={chest.gameObject.scene.name}";
        }

        ItemStack actualStack = chest.GetSlot(slotIndex);
        if (!ItemStacksEqual(actualStack, expectedStack))
        {
            return $"chest_slot_mismatch expected={FormatItemStack(expectedStack)} actual={FormatItemStack(actualStack)}";
        }

        return "chest_ok";
    }

    private string DescribeFarmCandidateMismatch(string label, FarmCandidate candidate, string expectedCropGuid)
    {
        FarmTileManager farmTileManager = FarmTileManager.Instance ?? FindFirstObjectByType<FarmTileManager>(FindObjectsInactive.Include);
        if (farmTileManager == null)
        {
            return $"{label}_farm_missing";
        }

        FarmTileData tileData = farmTileManager.GetTileData(candidate.LayerIndex, candidate.CellPosition);
        if (tileData == null)
        {
            return $"{label}_tile_missing cell={FormatCell(candidate)}";
        }

        if (!tileData.isTilled)
        {
            return $"{label}_tile_not_tilled cell={FormatCell(candidate)}";
        }

        if (!tileData.wateredToday)
        {
            return $"{label}_tile_not_watered cell={FormatCell(candidate)}";
        }

        if (tileData.cropData == null)
        {
            return $"{label}_cropdata_missing cell={FormatCell(candidate)}";
        }

        CropController crop = FindCropByGuid(expectedCropGuid);
        if (crop == null)
        {
            return $"{label}_crop_missing guid={expectedCropGuid}";
        }

        string tileCropGuid = tileData.cropController != null ? tileData.cropController.PersistentId : string.Empty;
        if (!string.Equals(tileCropGuid, expectedCropGuid, StringComparison.Ordinal))
        {
            return $"{label}_crop_guid_mismatch expected={expectedCropGuid} actual={tileCropGuid}";
        }

        return $"{label}_ok";
    }

    private static bool TryResolveSceneBuildIndex(string sceneName, out int buildIndex, out string scenePath)
    {
        buildIndex = -1;
        scenePath = string.Empty;
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            return false;
        }

        for (int index = 0; index < SceneManager.sceneCountInBuildSettings; index++)
        {
            string buildScenePath = SceneUtility.GetScenePathByBuildIndex(index);
            if (string.IsNullOrWhiteSpace(buildScenePath))
            {
                continue;
            }

            string buildSceneName = Path.GetFileNameWithoutExtension(buildScenePath);
            if (!string.Equals(buildSceneName, sceneName.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            buildIndex = index;
            scenePath = buildScenePath;
            return true;
        }

        return false;
    }

    private static string ResolveEntryAnchorName(string currentSceneName, string targetSceneName)
    {
        if (string.Equals(currentSceneName, PrimarySceneName, StringComparison.Ordinal)
            && string.Equals(targetSceneName, HomeSceneName, StringComparison.Ordinal))
        {
            return HomeEntryAnchorName;
        }

        if (string.Equals(targetSceneName, PrimarySceneName, StringComparison.Ordinal))
        {
            return PrimaryHomeEntryAnchorName;
        }

        if (string.Equals(currentSceneName, PrimarySceneName, StringComparison.Ordinal)
            && string.Equals(targetSceneName, TownSceneName, StringComparison.Ordinal))
        {
            return TownOpeningEntryAnchorName;
        }

        return string.Empty;
    }
}
