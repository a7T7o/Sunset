using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Play Mode 下的一次性导航 live 验证器。
/// 使用显式状态机而不是协程，避免 MCP 会话里首个 yield 后失去推进。
/// </summary>
public class NavigationLiveValidationRunner : MonoBehaviour
{
    private const string LogPrefix = "[NavValidation]";
#if UNITY_EDITOR
    private const string PendingActionEditorPrefKey = "Sunset.NavigationLiveValidation.PendingAction";
    private const string PendingActionFileRelativePath = "Library/NavigationLiveValidation/pending_action.txt";
    private const bool ForceFullPackAutorunForCurrentFix = false;
#endif
    private const float ScenarioTimeout = 6.5f;
    private const int SetupSettleFrames = 2;
    private const int HeartbeatFrameInterval = 45;
    private const int InitialScenarioBootstrapFrames = 2;
    private const int IgnoreRaycastLayer = 2;
    private const float PushContactBuffer = 0.04f;
    private const float PushForwardDotThreshold = 0.25f;
    private const float MaxAllowedNpcPushDisplacement = 0.08f;
    private const float PlayerPointArrivalCenterTolerance = 0.35f;
    private const float EndpointNpcArrivalTolerancePadding = 0.35f;
    private const int EndpointNpcArrivalMaxActionChanges = 12;
    private const int ManagedRoamSeedStart = 4101;
    private const int ManagedRoamMaxSeedAttempts = 10;
    private const float ManagedRoamShortPauseDuration = 0.05f;
    private const float ManagedRoamMinimumMoveDistance = 1.4f;
    private const float ManagedRoamAcceptEastDelta = 1.2f;
    private const float ManagedRoamAcceptVerticalTolerance = 1.15f;
    private const float ManagedRoamBlockerClearance = 0.06f;
    private const float ManagedRoamBlockerSideOffset = 0.34f;
    private const float ManagedRoamPersistentAnchorOffset = 0.18f;
    private const float ManagedRoamReleaseHoldTime = 0.2f;
    private const float ManagedRoamPersistentInterruptTimeout = 9.2f;
    private const float ManagedRoamPersistentStuckCheckInterval = 0.18f;
    private const float ManagedRoamPersistentStuckDistanceThreshold = 0.3f;
    private const int ManagedRoamPersistentMaxStuckRecoveries = 1;
    private static bool s_previousRunInBackground;
    private static bool s_runInBackgroundOverridden;

    private enum ScenarioKind
    {
        None = 0,
        PlayerAvoidsMovingNpc = 1,
        NpcAvoidsPlayer = 2,
        NpcNpcCrossing = 3,
        RealInputPlayerAvoidsMovingNpc = 4,
        RealInputPlayerSingleNpcNear = 5,
        RealInputPlayerCrowdPass = 6,
        RealInputPlayerGroundPointMatrix = 7,
        RealInputPlayerEndpointNpcOccupied = 8,
        NpcRoamRecoverWindow = 9,
        NpcRoamPersistentBlockInterrupt = 10,
        RealInputPlayerPassableCorridor = 11,
        RealInputPlayerStaticNpcWall = 12
    }

    private enum ClickProbeMode
    {
        SuppressedNpcInteractions = 0,
        RawRightClick = 1
    }

    private readonly struct GroundPointMatrixCase
    {
        public readonly string Id;
        public readonly string DistanceBand;
        public readonly string DirectionLabel;
        public readonly Vector2 Start;
        public readonly Vector2 Target;

        public GroundPointMatrixCase(string id, string distanceBand, string directionLabel, Vector2 start, Vector2 target)
        {
            Id = id;
            DistanceBand = distanceBand;
            DirectionLabel = directionLabel;
            Start = start;
            Target = target;
        }
    }

    private readonly struct PackScenarioSpec
    {
        public readonly ScenarioKind Scenario;
        public readonly ClickProbeMode ClickMode;
        public readonly string ResultName;

        public PackScenarioSpec(ScenarioKind scenario, ClickProbeMode clickMode, string resultName)
        {
            Scenario = scenario;
            ClickMode = clickMode;
            ResultName = resultName;
        }
    }

    private static readonly GroundPointMatrixCase[] GroundPointMatrixCases =
    {
        new GroundPointMatrixCase("Short-East", "Short", "East", new Vector2(-9.15f, 4.45f), new Vector2(-7.75f, 4.45f)),
        new GroundPointMatrixCase("Short-West", "Short", "West", new Vector2(-6.20f, 4.45f), new Vector2(-7.60f, 4.45f)),
        new GroundPointMatrixCase("Short-NorthEast", "Short", "NorthEast", new Vector2(-9.15f, 4.45f), new Vector2(-8.05f, 5.35f)),
        new GroundPointMatrixCase("Medium-East", "Medium", "East", new Vector2(-9.15f, 4.45f), new Vector2(-5.80f, 4.45f)),
        new GroundPointMatrixCase("Medium-West", "Medium", "West", new Vector2(-5.80f, 4.45f), new Vector2(-9.15f, 4.45f)),
        new GroundPointMatrixCase("Medium-NorthEast", "Medium", "NorthEast", new Vector2(-9.15f, 4.45f), new Vector2(-6.30f, 6.10f))
    };

    private bool isRunning;
    private bool runSingleScenarioOnly;
    private ScenarioKind currentScenario;
    private int settleFramesRemaining;
    private float scenarioTimer;
    private int nextHeartbeatFrame;
    private int queuedScenarioStartFrames;
    private float laneReferenceY;
    private float combinedRadius;
    private float minClearance;
    private float blockOnsetEdgeDistance;
    private float maxPlayerLateralOffset;
    private float maxNpcLateralOffset;
    private int playerDirectionFlipCount;
    private float crowdStallDuration;
    private bool playerReached;
    private bool npcReached;
    private bool npcAReached;
    private bool npcBReached;
    private bool npcMoveIssued;
    private bool npcAMoveIssued;
    private bool npcBMoveIssued;
    private bool playerClickIssued;
    private bool pushObservationInitialized;
    private bool blockOnsetRecorded;
    private bool clickPendingAutoInteractionAfterIssue;
    private Vector2 playerTarget;
    private Vector2 npcTarget;
    private Vector2 npcATarget;
    private Vector2 npcBTarget;
    private Vector2 previousPlayerPos;
    private Vector2 previousNpcPos;
    private Vector2 previousPlayerMotion;
    private float npcPushDisplacement;
    private int npcPushContactFrames;
    private int playerPathMoveFrames;
    private int playerDetourMoveFrames;
    private int playerBlockedInputFrames;
    private int playerHardStopFrames;
    private int playerActionChangeCount;
    private int groundPointCaseIndex;
    private int groundPointReachedCases;
    private int groundPointAccurateCenterCases;
    private int groundPointPositiveCenterBiasCases;
    private float groundPointCaseStartTimer;
    private float groundPointMaxColliderDistance;
    private readonly List<string> groundPointCaseDetails = new List<string>(8);
    private PlayerAutoNavigator activePlayerNavigator;
    private GameInputManager activeGameInputManager;
    private NPCMovementBundle activeNpcA;
    private NPCMovementBundle activeNpcB;
    private NPCMovementBundle activeNpcC;
    private ScenarioKind queuedScenario;
    private ClickProbeMode currentClickProbeMode = ClickProbeMode.SuppressedNpcInteractions;
    private string clickPendingAutoInteractionTargetName = "none";
    private string lastObservedPlayerAction = string.Empty;
    private bool roamSeedAccepted;
    private bool roamDetourCreated;
    private bool roamReleaseSucceeded;
    private bool roamBlockerReleased;
    private int roamSeedCursor;
    private int roamSeedAttempts;
    private int roamAcceptedSeed;
    private float roamAcceptedAtTime;
    private Vector2 roamNpcStartPosition;
    private Vector2 roamAcceptedDestination;
    private readonly Dictionary<int, NpcRoamControllerManagedTuningSnapshot> managedRoamControllerDefaults =
        new Dictionary<int, NpcRoamControllerManagedTuningSnapshot>();

    private readonly List<ScenarioResult> scenarioResults = new List<ScenarioResult>();
    private readonly List<PackScenarioSpec> activePackScenarios = new List<PackScenarioSpec>(12);
    private string activePackName = string.Empty;
    private string currentScenarioResultName = string.Empty;
    private int activePackScenarioIndex = -1;

    private struct ScenarioResult
    {
        public string Name;
        public bool Passed;
        public string Details;
    }

    public static NavigationLiveValidationRunner BeginOrRestart()
    {
        NavigationLiveValidationRunner runner = GetOrCreateRunner();
        runner.RunAll();
        return runner;
    }

    public static NavigationLiveValidationRunner BeginFinalAcceptancePack()
    {
        NavigationLiveValidationRunner runner = GetOrCreateRunner();
        runner.RunFinalAcceptancePack();
        return runner;
    }

    public static void SetupPlayerAvoidsMovingNpcProbe()
    {
        GetOrCreateRunner().RunSingleSetup(ScenarioKind.PlayerAvoidsMovingNpc);
    }

    public static void RunRealInputPlayerAvoidsMovingNpcProbe()
    {
        GetOrCreateRunner().RunSingleSetup(
            ScenarioKind.RealInputPlayerAvoidsMovingNpc,
            ClickProbeMode.SuppressedNpcInteractions);
    }

    public static void RunRawRealInputPlayerAvoidsMovingNpcProbe()
    {
        GetOrCreateRunner().RunSingleSetup(
            ScenarioKind.RealInputPlayerAvoidsMovingNpc,
            ClickProbeMode.RawRightClick);
    }

    public static void RunRealInputPlayerSingleNpcNearProbe()
    {
        GetOrCreateRunner().RunSingleSetup(
            ScenarioKind.RealInputPlayerSingleNpcNear,
            ClickProbeMode.SuppressedNpcInteractions);
    }

    public static void RunRawRealInputPlayerSingleNpcNearProbe()
    {
        GetOrCreateRunner().RunSingleSetup(
            ScenarioKind.RealInputPlayerSingleNpcNear,
            ClickProbeMode.RawRightClick);
    }

    public static void RunRealInputPlayerCrowdPassProbe()
    {
        RunLegacyRealInputPlayerCrowdBlockedWallStressProbe();
    }

    public static void RunRawRealInputPlayerCrowdPassProbe()
    {
        RunLegacyRawRealInputPlayerCrowdBlockedWallStressProbe();
    }

    public static void RunLegacyRealInputPlayerCrowdBlockedWallStressProbe()
    {
        GetOrCreateRunner().RunSingleSetup(
            ScenarioKind.RealInputPlayerCrowdPass,
            ClickProbeMode.SuppressedNpcInteractions);
    }

    public static void RunLegacyRawRealInputPlayerCrowdBlockedWallStressProbe()
    {
        GetOrCreateRunner().RunSingleSetup(
            ScenarioKind.RealInputPlayerCrowdPass,
            ClickProbeMode.RawRightClick);
    }

    public static void RunRealInputPlayerPassableCorridorProbe()
    {
        GetOrCreateRunner().RunSingleSetup(
            ScenarioKind.RealInputPlayerPassableCorridor,
            ClickProbeMode.SuppressedNpcInteractions);
    }

    public static void RunRealInputPlayerStaticNpcWallProbe()
    {
        GetOrCreateRunner().RunSingleSetup(
            ScenarioKind.RealInputPlayerStaticNpcWall,
            ClickProbeMode.SuppressedNpcInteractions);
    }

    public static void RunRealInputGroundPointMatrixProbe()
    {
        GetOrCreateRunner().RunSingleSetup(
            ScenarioKind.RealInputPlayerGroundPointMatrix,
            ClickProbeMode.SuppressedNpcInteractions);
    }

    public static void RunRawRealInputGroundPointMatrixProbe()
    {
        GetOrCreateRunner().RunSingleSetup(
            ScenarioKind.RealInputPlayerGroundPointMatrix,
            ClickProbeMode.RawRightClick);
    }

    public static void RunRealInputPlayerEndpointNpcOccupiedProbe()
    {
        GetOrCreateRunner().RunSingleSetup(
            ScenarioKind.RealInputPlayerEndpointNpcOccupied,
            ClickProbeMode.SuppressedNpcInteractions);
    }

    public static void RunRawRealInputPlayerEndpointNpcOccupiedProbe()
    {
        GetOrCreateRunner().RunSingleSetup(
            ScenarioKind.RealInputPlayerEndpointNpcOccupied,
            ClickProbeMode.RawRightClick);
    }

    public static void SetupNpcAvoidsPlayerProbe()
    {
        GetOrCreateRunner().RunSingleSetup(ScenarioKind.NpcAvoidsPlayer);
    }

    public static void SetupNpcNpcCrossingProbe()
    {
        GetOrCreateRunner().RunSingleSetup(ScenarioKind.NpcNpcCrossing);
    }

    public static void RunNpcRoamRecoverWindowProbe()
    {
        GetOrCreateRunner().RunSingleSetup(ScenarioKind.NpcRoamRecoverWindow);
    }

    public static void RunNpcRoamPersistentBlockInterruptProbe()
    {
        GetOrCreateRunner().RunSingleSetup(ScenarioKind.NpcRoamPersistentBlockInterrupt);
    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoRunFromEditorLaunchRequest()
    {
        if (ForceFullPackAutorunForCurrentFix && Application.isPlaying)
        {
            Debug.Log($"{LogPrefix} forced_autorun_full_pack");
            BeginFinalAcceptancePack();
            return;
        }

        if (!Application.isPlaying)
        {
            return;
        }

        string action = string.Empty;
        if (EditorPrefs.HasKey(PendingActionEditorPrefKey))
        {
            action = EditorPrefs.GetString(PendingActionEditorPrefKey, string.Empty);
            EditorPrefs.DeleteKey(PendingActionEditorPrefKey);
        }
        else
        {
            action = ConsumePendingActionFile();
        }

        if (string.IsNullOrEmpty(action))
        {
            return;
        }

        Debug.Log($"{LogPrefix} runtime_launch_request={action}");

        switch (action)
        {
            case "RunAll":
                BeginOrRestart();
                break;

            case "RunFinalAcceptancePack":
                BeginFinalAcceptancePack();
                break;

            case "SetupPlayerAvoidsMovingNpc":
                SetupPlayerAvoidsMovingNpcProbe();
                break;

            case "RunRealInputPush":
                RunRealInputPlayerAvoidsMovingNpcProbe();
                break;

            case "RunRawRealInputPush":
                RunRawRealInputPlayerAvoidsMovingNpcProbe();
                break;

            case "RunRealInputSingleNpcNear":
                RunRealInputPlayerSingleNpcNearProbe();
                break;

            case "RunRawRealInputSingleNpcNear":
                RunRawRealInputPlayerSingleNpcNearProbe();
                break;

            case "RunRealInputCrowd":
                RunLegacyRealInputPlayerCrowdBlockedWallStressProbe();
                break;

            case "RunRawRealInputCrowd":
                RunLegacyRawRealInputPlayerCrowdBlockedWallStressProbe();
                break;

            case "RunRealInputGroundPointMatrix":
                RunRealInputGroundPointMatrixProbe();
                break;

            case "RunRawRealInputGroundPointMatrix":
                RunRawRealInputGroundPointMatrixProbe();
                break;

            case "RunRealInputEndpointNpcOccupied":
                RunRealInputPlayerEndpointNpcOccupiedProbe();
                break;

            case "RunRawRealInputEndpointNpcOccupied":
                RunRawRealInputPlayerEndpointNpcOccupiedProbe();
                break;

            case "RunRealInputPassableCorridor":
                RunRealInputPlayerPassableCorridorProbe();
                break;

            case "RunRealInputStaticNpcWall":
                RunRealInputPlayerStaticNpcWallProbe();
                break;

            case "SetupNpcAvoidsPlayer":
                SetupNpcAvoidsPlayerProbe();
                break;

            case "SetupNpcNpcCrossing":
                SetupNpcNpcCrossingProbe();
                break;

            case "RunNpcRoamRecoverWindow":
                RunNpcRoamRecoverWindowProbe();
                break;

            case "RunNpcRoamPersistentBlockInterrupt":
                RunNpcRoamPersistentBlockInterruptProbe();
                break;

            default:
                Debug.LogWarning($"{LogPrefix} unknown_runtime_launch_request={action}");
                break;
        }
    }
#endif

#if UNITY_EDITOR
    private static string ConsumePendingActionFile()
    {
        string path = GetPendingActionFilePath();
        if (!File.Exists(path))
        {
            return string.Empty;
        }

        string action = File.ReadAllText(path).Trim();
        File.Delete(path);
        return action;
    }

    private static string GetPendingActionFilePath()
    {
        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        return string.IsNullOrEmpty(projectRoot)
            ? PendingActionFileRelativePath
            : Path.Combine(projectRoot, PendingActionFileRelativePath);
    }
#endif

    public void RunAll()
    {
        EnsureRunInBackground();
        Time.timeScale = 1f;
        isRunning = true;
        runSingleScenarioOnly = false;
        currentClickProbeMode = ClickProbeMode.SuppressedNpcInteractions;
        scenarioResults.Clear();
        ResetObservationState();
        ClearActivePackState();
        Debug.Log($"{LogPrefix} runner_started");
        QueueScenarioStart(ScenarioKind.RealInputPlayerAvoidsMovingNpc, InitialScenarioBootstrapFrames, "run_all");
    }

    private void RunFinalAcceptancePack()
    {
        EnsureRunInBackground();
        Time.timeScale = 1f;
        isRunning = true;
        runSingleScenarioOnly = false;
        scenarioResults.Clear();
        ResetObservationState();
        BuildFinalAcceptancePackScenarios();

        Debug.Log(
            $"{LogPrefix} final_acceptance_pack_started name={activePackName} " +
            $"caseCount={activePackScenarios.Count}");

        if (!QueueNextPackScenario("final_acceptance_pack"))
        {
            AbortRun("scenario=FinalAcceptancePack empty_pack");
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private static NavigationLiveValidationRunner GetOrCreateRunner()
    {
        NavigationLiveValidationRunner runner = FindFirstObjectByType<NavigationLiveValidationRunner>();
        if (runner == null)
        {
            GameObject go = new GameObject("NavigationLiveValidationRunner");
            runner = go.AddComponent<NavigationLiveValidationRunner>();
        }

        return runner;
    }

    private void Update()
    {
        if (!Application.isPlaying || !isRunning)
        {
            return;
        }

        if (currentScenario == ScenarioKind.None)
        {
            if (TryStartQueuedScenario())
            {
                return;
            }

            return;
        }

        if (!EnsureScenarioRuntimeBindings())
        {
            return;
        }

        if (settleFramesRemaining > 0)
        {
            settleFramesRemaining--;
            if (settleFramesRemaining == 0)
            {
                Debug.Log($"{LogPrefix} scenario_observe_start={currentScenario} frame={Time.frameCount} time={Time.time:F2} unscaledTime={Time.unscaledTime:F2}");
            }
            return;
        }

        scenarioTimer += Time.unscaledDeltaTime;
        MaybeLogHeartbeat();

        switch (currentScenario)
        {
            case ScenarioKind.PlayerAvoidsMovingNpc:
                TickPlayerAvoidsMovingNpc();
                break;

            case ScenarioKind.NpcAvoidsPlayer:
                TickNpcAvoidsPlayer();
                break;

            case ScenarioKind.NpcNpcCrossing:
                TickNpcNpcCrossing();
                break;

            case ScenarioKind.RealInputPlayerAvoidsMovingNpc:
                TickRealInputPlayerAvoidsMovingNpc();
                break;

            case ScenarioKind.RealInputPlayerSingleNpcNear:
                TickRealInputPlayerSingleNpcNear();
                break;

            case ScenarioKind.RealInputPlayerCrowdPass:
                TickRealInputPlayerCrowdPass();
                break;

            case ScenarioKind.RealInputPlayerGroundPointMatrix:
                TickRealInputGroundPointMatrix();
                break;

            case ScenarioKind.RealInputPlayerEndpointNpcOccupied:
                TickRealInputPlayerEndpointNpcOccupied();
                break;

            case ScenarioKind.RealInputPlayerPassableCorridor:
                TickRealInputPlayerPassableCorridor();
                break;

            case ScenarioKind.RealInputPlayerStaticNpcWall:
                TickRealInputPlayerStaticNpcWall();
                break;

            case ScenarioKind.NpcRoamRecoverWindow:
                TickNpcRoamRecoverWindow();
                break;

            case ScenarioKind.NpcRoamPersistentBlockInterrupt:
                TickNpcRoamPersistentBlockInterrupt();
                break;
        }
    }

    private void OnDisable()
    {
        Debug.Log($"{LogPrefix} runner_disabled");
    }

    private void OnDestroy()
    {
        RestoreRunInBackground();
        Debug.Log($"{LogPrefix} runner_destroyed");
    }

    private void StartScenario(ScenarioKind scenarioKind)
    {
        ResetObservationState();
        currentScenario = scenarioKind;
        nextHeartbeatFrame = Time.frameCount + HeartbeatFrameInterval;

        switch (scenarioKind)
        {
            case ScenarioKind.PlayerAvoidsMovingNpc:
                SetupPlayerAvoidsMovingNpc();
                break;

            case ScenarioKind.NpcAvoidsPlayer:
                SetupNpcAvoidsPlayer();
                break;

            case ScenarioKind.NpcNpcCrossing:
                SetupNpcNpcCrossing();
                break;

            case ScenarioKind.RealInputPlayerAvoidsMovingNpc:
                SetupRealInputPlayerAvoidsMovingNpc();
                break;

            case ScenarioKind.RealInputPlayerSingleNpcNear:
                SetupRealInputPlayerSingleNpcNear();
                break;

            case ScenarioKind.RealInputPlayerCrowdPass:
                SetupRealInputPlayerCrowdPass();
                break;

            case ScenarioKind.RealInputPlayerGroundPointMatrix:
                SetupRealInputGroundPointMatrix();
                break;

            case ScenarioKind.RealInputPlayerEndpointNpcOccupied:
                SetupRealInputPlayerEndpointNpcOccupied();
                break;

            case ScenarioKind.RealInputPlayerPassableCorridor:
                SetupRealInputPlayerPassableCorridor();
                break;

            case ScenarioKind.RealInputPlayerStaticNpcWall:
                SetupRealInputPlayerStaticNpcWall();
                break;

            case ScenarioKind.NpcRoamRecoverWindow:
                SetupNpcRoamRecoverWindow();
                break;

            case ScenarioKind.NpcRoamPersistentBlockInterrupt:
                SetupNpcRoamPersistentBlockInterrupt();
                break;
        }
    }

    private void RunSingleSetup(ScenarioKind scenarioKind)
    {
        RunSingleSetup(scenarioKind, ClickProbeMode.SuppressedNpcInteractions);
    }

    private void RunSingleSetup(ScenarioKind scenarioKind, ClickProbeMode clickProbeMode)
    {
        EnsureRunInBackground();
        Time.timeScale = 1f;
        isRunning = true;
        runSingleScenarioOnly = true;
        currentClickProbeMode = clickProbeMode;
        scenarioResults.Clear();
        ResetObservationState();
        ClearActivePackState();
        QueueScenarioStart(scenarioKind, InitialScenarioBootstrapFrames, "run_single");
    }

    private void BuildFinalAcceptancePackScenarios()
    {
        activePackScenarios.Clear();
        activePackName = "FinalPlayerNavigationAcceptancePack";
        activePackScenarioIndex = -1;

        AddPackScenario(ScenarioKind.RealInputPlayerPassableCorridor, ClickProbeMode.SuppressedNpcInteractions, "PassableCorridor#1");
        AddPackScenario(ScenarioKind.RealInputPlayerPassableCorridor, ClickProbeMode.SuppressedNpcInteractions, "PassableCorridor#2");
        AddPackScenario(ScenarioKind.RealInputPlayerPassableCorridor, ClickProbeMode.SuppressedNpcInteractions, "PassableCorridor#3");

        AddPackScenario(ScenarioKind.RealInputPlayerStaticNpcWall, ClickProbeMode.SuppressedNpcInteractions, "StaticNpcWall#1");
        AddPackScenario(ScenarioKind.RealInputPlayerStaticNpcWall, ClickProbeMode.SuppressedNpcInteractions, "StaticNpcWall#2");
        AddPackScenario(ScenarioKind.RealInputPlayerStaticNpcWall, ClickProbeMode.SuppressedNpcInteractions, "StaticNpcWall#3");

        AddPackScenario(ScenarioKind.RealInputPlayerEndpointNpcOccupied, ClickProbeMode.SuppressedNpcInteractions, "EndpointNpcOccupied#1");
        AddPackScenario(ScenarioKind.RealInputPlayerGroundPointMatrix, ClickProbeMode.RawRightClick, "GroundRawMatrix#1");
        AddPackScenario(ScenarioKind.RealInputPlayerSingleNpcNear, ClickProbeMode.SuppressedNpcInteractions, "SingleNpcNear#1");
        AddPackScenario(ScenarioKind.RealInputPlayerAvoidsMovingNpc, ClickProbeMode.SuppressedNpcInteractions, "MovingNpc#1");
        AddPackScenario(ScenarioKind.NpcAvoidsPlayer, ClickProbeMode.SuppressedNpcInteractions, "NpcAvoidsPlayer#1");
        AddPackScenario(ScenarioKind.NpcNpcCrossing, ClickProbeMode.SuppressedNpcInteractions, "NpcNpcCrossing#1");
    }

    private void AddPackScenario(ScenarioKind scenarioKind, ClickProbeMode clickProbeMode, string resultName)
    {
        activePackScenarios.Add(new PackScenarioSpec(scenarioKind, clickProbeMode, resultName));
    }

    private bool HasActivePack()
    {
        return activePackScenarios.Count > 0;
    }

    private bool QueueNextPackScenario(string source)
    {
        if (!HasActivePack())
        {
            return false;
        }

        int nextIndex = activePackScenarioIndex + 1;
        if (nextIndex >= activePackScenarios.Count)
        {
            return false;
        }

        activePackScenarioIndex = nextIndex;
        PackScenarioSpec spec = activePackScenarios[activePackScenarioIndex];
        currentClickProbeMode = spec.ClickMode;
        currentScenarioResultName = spec.ResultName;

        Debug.Log(
            $"{LogPrefix} final_acceptance_pack_case={spec.ResultName} scenario={spec.Scenario} " +
            $"clickMode={spec.ClickMode} index={activePackScenarioIndex + 1}/{activePackScenarios.Count} " +
            $"source={source}");

        QueueScenarioStart(spec.Scenario, InitialScenarioBootstrapFrames, source);
        return true;
    }

    private void ClearActivePackState()
    {
        activePackScenarios.Clear();
        activePackName = string.Empty;
        currentScenarioResultName = string.Empty;
        activePackScenarioIndex = -1;
    }

    private void QueueScenarioStart(ScenarioKind scenarioKind, int delayFrames, string source)
    {
        queuedScenario = scenarioKind;
        queuedScenarioStartFrames = Mathf.Max(0, delayFrames);
        currentScenario = ScenarioKind.None;
        Debug.Log(
            $"{LogPrefix} scenario_queued={scenarioKind} source={source} " +
            $"delayFrames={queuedScenarioStartFrames} clickMode={currentClickProbeMode} frame={Time.frameCount}");
    }

    private bool TryStartQueuedScenario()
    {
        if (queuedScenario == ScenarioKind.None)
        {
            return false;
        }

        if (queuedScenarioStartFrames > 0)
        {
            queuedScenarioStartFrames--;
            return true;
        }

        ScenarioKind scenarioToStart = queuedScenario;
        queuedScenario = ScenarioKind.None;
        queuedScenarioStartFrames = 0;
        StartScenario(scenarioToStart);
        return true;
    }

    private static void EnsureRunInBackground()
    {
        if (s_runInBackgroundOverridden)
        {
            return;
        }

        s_previousRunInBackground = Application.runInBackground;
        Application.runInBackground = true;
        s_runInBackgroundOverridden = true;
        Debug.Log($"{LogPrefix} runInBackground_enabled previous={s_previousRunInBackground}");
    }

    private static void RestoreRunInBackground()
    {
        if (!s_runInBackgroundOverridden)
        {
            return;
        }

        Application.runInBackground = s_previousRunInBackground;
        s_runInBackgroundOverridden = false;
        Debug.Log($"{LogPrefix} runInBackground_restored value={Application.runInBackground}");
    }

    private void SetupPlayerAvoidsMovingNpc()
    {
        Debug.Log($"{LogPrefix} scenario_start=PlayerAvoidsMovingNpc");
        if (!TryResolveActors(out activePlayerNavigator, out activeNpcA, out activeNpcB, out activeNpcC))
        {
            AbortRun("scenario=PlayerAvoidsMovingNpc actors_missing");
            return;
        }

        PrepareScene(activePlayerNavigator, activeNpcA, activeNpcB, activeNpcC);

        const float playerLaneY = 4.45f;
        Vector2 playerStart = new Vector2(-9.15f, playerLaneY);
        playerTarget = new Vector2(-5.25f, playerLaneY);
        Vector2 npcStart = new Vector2(-7.30f, 3.60f);
        npcTarget = new Vector2(-7.30f, 5.35f);

        Collider2D playerCollider = activePlayerNavigator.GetComponent<Collider2D>();
        PlaceActor(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>(), playerStart);
        PlaceNpc(activeNpcA, npcStart);
        PlaceNpc(activeNpcB, new Vector2(1.80f, 9.60f));
        PlaceNpc(activeNpcC, new Vector2(3.80f, 9.95f));

        laneReferenceY = GetActorMeasurePosition(activePlayerNavigator.transform, playerCollider).y;
        combinedRadius = GetActorRadius(playerCollider) + GetActorRadius(activeNpcA.Collider);
        npcMoveIssued = activeNpcA.Controller.DebugMoveTo(npcTarget);
        activePlayerNavigator.SetDestination(playerTarget);
        settleFramesRemaining = SetupSettleFrames;

        Debug.Log(
            $"{LogPrefix} scenario_setup=PlayerAvoidsMovingNpc npcMoveIssued={npcMoveIssued} " +
            $"playerActive={activePlayerNavigator.IsActive} npcMoving={activeNpcA.Controller.IsMoving} " +
            $"playerPos={(Vector2)activePlayerNavigator.transform.position} npcPos={(Vector2)activeNpcA.Transform.position}");
    }

    private void SetupNpcAvoidsPlayer()
    {
        Debug.Log($"{LogPrefix} scenario_start=NpcAvoidsPlayer");
        if (!TryResolveActors(out activePlayerNavigator, out activeNpcA, out activeNpcB, out activeNpcC))
        {
            AbortRun("scenario=NpcAvoidsPlayer actors_missing");
            return;
        }

        PrepareScene(activePlayerNavigator, activeNpcA, activeNpcB, activeNpcC);

        const float laneY = 4.45f;
        Vector2 playerStart = new Vector2(-7.25f, laneY);
        Vector2 npcStart = new Vector2(-9.05f, laneY);
        npcTarget = new Vector2(-5.25f, laneY);

        Collider2D playerCollider = activePlayerNavigator.GetComponent<Collider2D>();
        PlaceActor(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>(), playerStart);
        PlaceNpc(activeNpcA, npcStart);
        PlaceNpc(activeNpcB, new Vector2(1.80f, 9.60f));
        PlaceNpc(activeNpcC, new Vector2(3.80f, 9.95f));

        laneReferenceY = GetActorMeasurePosition(activeNpcA.Transform, activeNpcA.Collider).y;
        combinedRadius = GetActorRadius(playerCollider) + GetActorRadius(activeNpcA.Collider);
        npcMoveIssued = activeNpcA.Controller.DebugMoveTo(npcTarget);
        settleFramesRemaining = SetupSettleFrames;

        Debug.Log(
            $"{LogPrefix} scenario_setup=NpcAvoidsPlayer npcMoveIssued={npcMoveIssued} " +
            $"npcMoving={activeNpcA.Controller.IsMoving} playerPos={(Vector2)activePlayerNavigator.transform.position} " +
            $"npcPos={(Vector2)activeNpcA.Transform.position}");
    }

    private void SetupNpcNpcCrossing()
    {
        Debug.Log($"{LogPrefix} scenario_start=NpcNpcCrossing");
        if (!TryResolveActors(out activePlayerNavigator, out activeNpcA, out activeNpcB, out activeNpcC))
        {
            AbortRun("scenario=NpcNpcCrossing actors_missing");
            return;
        }

        PrepareScene(activePlayerNavigator, activeNpcA, activeNpcB, activeNpcC);

        const float laneY = 4.45f;
        Vector2 npcAStart = new Vector2(-9.05f, laneY);
        npcATarget = new Vector2(-5.25f, laneY);
        Vector2 npcBStart = new Vector2(-5.25f, laneY);
        npcBTarget = new Vector2(-9.05f, laneY);

        PlaceActor(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>(), new Vector2(-2.0f, 8.0f));
        PlaceNpc(activeNpcA, npcAStart);
        PlaceNpc(activeNpcB, npcBStart);
        PlaceNpc(activeNpcC, new Vector2(-1.5f, 8.5f));

        laneReferenceY = GetActorMeasurePosition(activeNpcA.Transform, activeNpcA.Collider).y;
        combinedRadius = GetActorRadius(activeNpcA.Collider) + GetActorRadius(activeNpcB.Collider);
        npcAMoveIssued = activeNpcA.Controller.DebugMoveTo(npcATarget);
        npcBMoveIssued = activeNpcB.Controller.DebugMoveTo(npcBTarget);
        settleFramesRemaining = SetupSettleFrames;

        Debug.Log(
            $"{LogPrefix} scenario_setup=NpcNpcCrossing npcAMoveIssued={npcAMoveIssued} npcBMoveIssued={npcBMoveIssued} " +
            $"npcAPos={(Vector2)activeNpcA.Transform.position} npcBPos={(Vector2)activeNpcB.Transform.position}");
    }

    private void SetupNpcRoamRecoverWindow()
    {
        SetupManagedNpcRoamScenario(persistentBlockers: false, "NpcRoamRecoverWindow");
    }

    private void SetupNpcRoamPersistentBlockInterrupt()
    {
        SetupManagedNpcRoamScenario(persistentBlockers: true, "NpcRoamPersistentBlockInterrupt");
    }

    private void SetupManagedNpcRoamScenario(bool persistentBlockers, string scenarioName)
    {
        Debug.Log($"{LogPrefix} scenario_start={scenarioName}");
        if (!TryResolveActors(out activePlayerNavigator, out activeNpcA, out activeNpcB, out activeNpcC))
        {
            AbortRun($"scenario={scenarioName} actors_missing");
            return;
        }

        PrepareScene(activePlayerNavigator, activeNpcA, activeNpcB, activeNpcC);

        const float laneY = 4.45f;
        roamNpcStartPosition = new Vector2(-9.05f, laneY);

        PlaceActor(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>(), new Vector2(-2.0f, 8.0f));
        PlaceNpc(activeNpcA, roamNpcStartPosition);
        StopAndParkNpc(activeNpcB, GetManagedRoamBlockerParkingPosition(1));
        StopAndParkNpc(activeNpcC, GetManagedRoamBlockerParkingPosition(2));

        ConfigureManagedRoamController(activeNpcA.Controller);
        ConfigureManagedRoamController(activeNpcB.Controller);
        ConfigureManagedRoamController(activeNpcC.Controller);
        ConfigureManagedRoamPrimaryController(activeNpcA.Controller, persistentBlockers);

        BeginNextManagedRoamAttempt();
        settleFramesRemaining = SetupSettleFrames;
    }

    private void SetupRealInputPlayerAvoidsMovingNpc()
    {
        Debug.Log($"{LogPrefix} scenario_start=RealInputPlayerAvoidsMovingNpc");
        if (!TryResolveActors(out activePlayerNavigator, out activeNpcA, out activeNpcB, out activeNpcC))
        {
            AbortRun("scenario=RealInputPlayerAvoidsMovingNpc actors_missing");
            return;
        }

        activeGameInputManager = FindFirstObjectByType<GameInputManager>();
        if (activeGameInputManager == null)
        {
            AbortRun("scenario=RealInputPlayerAvoidsMovingNpc game_input_missing");
            return;
        }

        PrepareScene(activePlayerNavigator, activeNpcA, activeNpcB, activeNpcC);

        const float playerLaneY = 4.45f;
        Vector2 playerStart = new Vector2(-9.15f, playerLaneY);
        playerTarget = new Vector2(-5.25f, playerLaneY);
        Vector2 npcStart = new Vector2(-7.30f, 3.60f);
        npcTarget = new Vector2(-7.30f, 5.35f);

        Collider2D playerCollider = activePlayerNavigator.GetComponent<Collider2D>();
        PlaceActor(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>(), playerStart);
        PlaceNpc(activeNpcA, npcStart);
        PlaceNpc(activeNpcB, new Vector2(1.80f, 9.60f));
        PlaceNpc(activeNpcC, new Vector2(3.80f, 9.95f));

        laneReferenceY = GetActorFootPosition(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>()).y;
        combinedRadius = GetActorFootprintRadius(playerCollider) + GetActorFootprintRadius(activeNpcA.Collider);
        npcMoveIssued = activeNpcA.Controller.DebugMoveTo(npcTarget);
        playerClickIssued = TryIssueAutoNavClick(
            activeGameInputManager,
            playerTarget,
            currentClickProbeMode,
            activeNpcA,
            activeNpcB,
            activeNpcC);
        CapturePendingAutoInteractionState(activeGameInputManager);
        previousPlayerPos = GetActorFootPosition(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>());
        previousNpcPos = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        pushObservationInitialized = true;
        settleFramesRemaining = SetupSettleFrames;

        Debug.Log(
            $"{LogPrefix} scenario_setup=RealInputPlayerAvoidsMovingNpc npcMoveIssued={npcMoveIssued} " +
            $"playerClickIssued={playerClickIssued} playerActive={activePlayerNavigator.IsActive} " +
            $"npcMoving={activeNpcA.Controller.IsMoving} playerPos={(Vector2)activePlayerNavigator.transform.position} " +
                $"npcPos={(Vector2)activeNpcA.Transform.position} {BuildClickDispatchSummary()}");
    }

    private void SetupRealInputPlayerSingleNpcNear()
    {
        Debug.Log($"{LogPrefix} scenario_start=RealInputPlayerSingleNpcNear");
        if (!TryResolveActors(out activePlayerNavigator, out activeNpcA, out activeNpcB, out activeNpcC))
        {
            AbortRun("scenario=RealInputPlayerSingleNpcNear actors_missing");
            return;
        }

        activeGameInputManager = FindFirstObjectByType<GameInputManager>();
        if (activeGameInputManager == null)
        {
            AbortRun("scenario=RealInputPlayerSingleNpcNear game_input_missing");
            return;
        }

        PrepareScene(activePlayerNavigator, activeNpcA, activeNpcB, activeNpcC);

        Vector2 playerStart = new Vector2(-9.15f, 4.45f);
        playerTarget = new Vector2(-5.80f, 4.45f);
        Vector2 npcStart = new Vector2(-7.30f, 4.45f);

        Collider2D playerCollider = activePlayerNavigator.GetComponent<Collider2D>();
        PlaceActor(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>(), playerStart);
        PlaceNpc(activeNpcA, npcStart);
        PlaceNpc(activeNpcB, new Vector2(-3.5f, 8.0f));
        PlaceNpc(activeNpcC, new Vector2(-1.5f, 8.5f));

        laneReferenceY = playerStart.y;
        combinedRadius = GetActorFootprintRadius(playerCollider) + GetActorFootprintRadius(activeNpcA.Collider);
        playerClickIssued = TryIssueAutoNavClick(
            activeGameInputManager,
            playerTarget,
            currentClickProbeMode,
            activeNpcA,
            activeNpcB,
            activeNpcC);
        CapturePendingAutoInteractionState(activeGameInputManager);
        previousPlayerPos = GetActorFootPosition(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>());
        previousNpcPos = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        pushObservationInitialized = true;
        settleFramesRemaining = SetupSettleFrames;

        Debug.Log(
            $"{LogPrefix} scenario_setup=RealInputPlayerSingleNpcNear playerClickIssued={playerClickIssued} " +
            $"playerActive={activePlayerNavigator.IsActive} playerPos={(Vector2)activePlayerNavigator.transform.position} " +
            $"npcPos={(Vector2)activeNpcA.Transform.position} {BuildClickDispatchSummary()}");
    }

    private bool TrySetupRealInputPlayerCrowdScenario(
        string scenarioName,
        Vector2 playerStart,
        Vector2 target,
        Vector2 npcAStart,
        Vector2 npcBStart,
        Vector2 npcCStart)
    {
        Debug.Log($"{LogPrefix} scenario_start={scenarioName}");
        if (!TryResolveActors(out activePlayerNavigator, out activeNpcA, out activeNpcB, out activeNpcC))
        {
            AbortRun($"scenario={scenarioName} actors_missing");
            return false;
        }

        activeGameInputManager = FindFirstObjectByType<GameInputManager>();
        if (activeGameInputManager == null)
        {
            AbortRun($"scenario={scenarioName} game_input_missing");
            return false;
        }

        PrepareScene(activePlayerNavigator, activeNpcA, activeNpcB, activeNpcC);

        playerTarget = target;

        PlaceActor(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>(), playerStart);
        PlaceNpc(activeNpcA, npcAStart);
        PlaceNpc(activeNpcB, npcBStart);
        PlaceNpc(activeNpcC, npcCStart);

        laneReferenceY = playerStart.y;
        combinedRadius =
            GetActorFootprintRadius(activeNpcA.Collider) +
            Mathf.Max(GetActorFootprintRadius(activeNpcB.Collider), GetActorFootprintRadius(activeNpcC.Collider));
        playerClickIssued = TryIssueAutoNavClick(
            activeGameInputManager,
            playerTarget,
            currentClickProbeMode,
            activeNpcA,
            activeNpcB,
            activeNpcC);
        CapturePendingAutoInteractionState(activeGameInputManager);
        previousPlayerPos = GetActorFootPosition(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>());
        previousPlayerMotion = Vector2.zero;
        settleFramesRemaining = SetupSettleFrames;

        Debug.Log(
            $"{LogPrefix} scenario_setup={scenarioName} playerClickIssued={playerClickIssued} " +
            $"playerPos={(Vector2)activePlayerNavigator.transform.position} npcAPos={(Vector2)activeNpcA.Transform.position} " +
            $"npcBPos={(Vector2)activeNpcB.Transform.position} npcCPos={(Vector2)activeNpcC.Transform.position} " +
            $"{BuildClickDispatchSummary()}");
        return true;
    }

    private void SetupRealInputPlayerCrowdPass()
    {
        TrySetupRealInputPlayerCrowdScenario(
            "LegacyRealInputCrowdBlockedWallStress",
            new Vector2(-9.15f, 4.45f),
            new Vector2(-4.95f, 4.45f),
            new Vector2(-7.45f, 4.95f),
            new Vector2(-7.05f, 4.08f),
            new Vector2(-6.62f, 4.88f));
    }

    private void SetupRealInputPlayerPassableCorridor()
    {
        TrySetupRealInputPlayerCrowdScenario(
            "RealInputPlayerPassableCorridor",
            new Vector2(-9.15f, 4.45f),
            new Vector2(-4.95f, 4.45f),
            new Vector2(-7.45f, 5.08f),
            new Vector2(-7.08f, 3.80f),
            new Vector2(-5.95f, 5.16f));
    }

    private void SetupRealInputPlayerStaticNpcWall()
    {
        TrySetupRealInputPlayerCrowdScenario(
            "RealInputPlayerStaticNpcWall",
            new Vector2(-9.15f, 4.45f),
            new Vector2(-4.95f, 4.45f),
            new Vector2(-7.45f, 4.95f),
            new Vector2(-7.05f, 4.08f),
            new Vector2(-6.62f, 4.88f));
    }

    private void SetupRealInputGroundPointMatrix()
    {
        Debug.Log($"{LogPrefix} scenario_start=RealInputPlayerGroundPointMatrix");
        if (!TryResolveActors(out activePlayerNavigator, out activeNpcA, out activeNpcB, out activeNpcC))
        {
            AbortRun("scenario=RealInputPlayerGroundPointMatrix actors_missing");
            return;
        }

        activeGameInputManager = FindFirstObjectByType<GameInputManager>();
        if (activeGameInputManager == null)
        {
            AbortRun("scenario=RealInputPlayerGroundPointMatrix game_input_missing");
            return;
        }

        PrepareScene(activePlayerNavigator, activeNpcA, activeNpcB, activeNpcC);
        PlaceNpc(activeNpcA, new Vector2(-3.5f, 8.0f));
        PlaceNpc(activeNpcB, new Vector2(-1.5f, 8.5f));
        PlaceNpc(activeNpcC, new Vector2(-0.5f, 9.0f));

        groundPointCaseIndex = -1;
        groundPointReachedCases = 0;
        groundPointAccurateCenterCases = 0;
        groundPointPositiveCenterBiasCases = 0;
        groundPointMaxColliderDistance = 0f;
        groundPointCaseDetails.Clear();

        StartNextGroundPointMatrixCase();
    }

    private void SetupRealInputPlayerEndpointNpcOccupied()
    {
        Debug.Log($"{LogPrefix} scenario_start=RealInputPlayerEndpointNpcOccupied");
        if (!TryResolveActors(out activePlayerNavigator, out activeNpcA, out activeNpcB, out activeNpcC))
        {
            AbortRun("scenario=RealInputPlayerEndpointNpcOccupied actors_missing");
            return;
        }

        activeGameInputManager = FindFirstObjectByType<GameInputManager>();
        if (activeGameInputManager == null)
        {
            AbortRun("scenario=RealInputPlayerEndpointNpcOccupied game_input_missing");
            return;
        }

        PrepareScene(activePlayerNavigator, activeNpcA, activeNpcB, activeNpcC);

        Vector2 playerStart = new Vector2(-9.15f, 4.45f);
        Vector2 npcStart = new Vector2(-5.92f, 4.45f);
        playerTarget = new Vector2(-5.54f, 4.45f);

        Collider2D playerCollider = activePlayerNavigator.GetComponent<Collider2D>();
        PlaceActor(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>(), playerStart);
        PlaceNpc(activeNpcA, npcStart);
        PlaceNpc(activeNpcB, new Vector2(1.80f, 9.60f));
        PlaceNpc(activeNpcC, new Vector2(3.80f, 9.95f));

        laneReferenceY = playerStart.y;
        combinedRadius = GetActorFootprintRadius(playerCollider) + GetActorFootprintRadius(activeNpcA.Collider);
        playerClickIssued = TryIssueAutoNavClick(
            activeGameInputManager,
            playerTarget,
            currentClickProbeMode,
            activeNpcA,
            activeNpcB,
            activeNpcC);
        CapturePendingAutoInteractionState(activeGameInputManager);
        previousPlayerPos = GetActorFootPosition(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>());
        previousNpcPos = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        previousPlayerMotion = Vector2.zero;
        pushObservationInitialized = true;
        settleFramesRemaining = SetupSettleFrames;

        Debug.Log(
            $"{LogPrefix} scenario_setup=RealInputPlayerEndpointNpcOccupied playerClickIssued={playerClickIssued} " +
            $"playerPos={(Vector2)activePlayerNavigator.transform.position} npcPos={(Vector2)activeNpcA.Transform.position} " +
            $"target={playerTarget} combinedRadius={combinedRadius:F3} {BuildClickDispatchSummary()}");
    }

    private bool TryIssueAutoNavClick(
        GameInputManager inputManager,
        Vector2 world,
        ClickProbeMode clickProbeMode,
        params NPCMovementBundle[] interactableSuppressedNpcs)
    {
        if (inputManager == null)
        {
            return false;
        }

        List<LayerSnapshot> layerSnapshots = null;
        if (clickProbeMode == ClickProbeMode.SuppressedNpcInteractions)
        {
            ClearPendingAutoInteraction(inputManager);
            layerSnapshots = CaptureActorLayers(interactableSuppressedNpcs);
        }

        try
        {
            MethodInfo debugIssueMethod = inputManager.GetType().GetMethod(
                "DebugIssueAutoNavClick",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                binder: null,
                types: new[] { typeof(Vector2) },
                modifiers: null);

            if (debugIssueMethod != null)
            {
                object result = debugIssueMethod.Invoke(inputManager, new object[] { world });
                return result is bool issued ? issued : result != null;
            }

            if (activePlayerNavigator != null)
            {
                activePlayerNavigator.SetDestination(world);
                return true;
            }

            return false;
        }
        finally
        {
            RestoreActorLayers(layerSnapshots);
        }
    }

    private static void ClearPendingAutoInteraction(GameInputManager inputManager)
    {
        MethodInfo clearPendingMethod = inputManager.GetType().GetMethod(
            "ClearPendingAutoInteraction",
            BindingFlags.Instance | BindingFlags.NonPublic);

        clearPendingMethod?.Invoke(inputManager, null);
    }

    private static List<LayerSnapshot> CaptureActorLayers(params NPCMovementBundle[] actors)
    {
        List<LayerSnapshot> snapshots = new List<LayerSnapshot>();
        if (actors == null || actors.Length == 0)
        {
            return snapshots;
        }

        HashSet<int> visited = new HashSet<int>();
        for (int actorIndex = 0; actorIndex < actors.Length; actorIndex++)
        {
            Transform root = actors[actorIndex].Transform;
            if (root == null)
            {
                continue;
            }

            Transform[] hierarchy = root.GetComponentsInChildren<Transform>(true);
            for (int nodeIndex = 0; nodeIndex < hierarchy.Length; nodeIndex++)
            {
                GameObject node = hierarchy[nodeIndex].gameObject;
                if (!visited.Add(node.GetInstanceID()))
                {
                    continue;
                }

                snapshots.Add(new LayerSnapshot(node, node.layer));
                node.layer = IgnoreRaycastLayer;
            }
        }

        return snapshots;
    }

    private static void RestoreActorLayers(List<LayerSnapshot> snapshots)
    {
        if (snapshots == null)
        {
            return;
        }

        for (int index = snapshots.Count - 1; index >= 0; index--)
        {
            LayerSnapshot snapshot = snapshots[index];
            if (snapshot.GameObject != null)
            {
                snapshot.GameObject.layer = snapshot.Layer;
            }
        }
    }

    private void CapturePendingAutoInteractionState(GameInputManager inputManager)
    {
        clickPendingAutoInteractionAfterIssue = false;
        clickPendingAutoInteractionTargetName = "none";

        if (inputManager == null)
        {
            return;
        }

        FieldInfo pendingInteractableField = inputManager.GetType().GetField(
            "_pendingAutoInteractable",
            BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo pendingTargetField = inputManager.GetType().GetField(
            "_pendingAutoInteractTarget",
            BindingFlags.Instance | BindingFlags.NonPublic);

        object pendingInteractable = pendingInteractableField?.GetValue(inputManager);
        Transform pendingTarget = pendingTargetField?.GetValue(inputManager) as Transform;
        clickPendingAutoInteractionAfterIssue = pendingInteractable != null || pendingTarget != null;
        if (pendingTarget != null)
        {
            clickPendingAutoInteractionTargetName = pendingTarget.name;
        }
    }

    private string BuildClickDispatchSummary()
    {
        return $"clickMode={currentClickProbeMode} " +
               $"pendingAutoInteractionAfterClick={clickPendingAutoInteractionAfterIssue} " +
               $"pendingAutoInteractionTarget={clickPendingAutoInteractionTargetName}";
    }

    private void ObservePlayerNavigationAction()
    {
        if (activePlayerNavigator == null)
        {
            return;
        }

        PlayerAutoNavigatorDebugSnapshot playerDebug = PlayerAutoNavigatorDebugCompat.Capture(activePlayerNavigator);
        string action = string.IsNullOrEmpty(playerDebug.NavigationAction)
            ? "Unknown"
            : playerDebug.NavigationAction;

        switch (action)
        {
            case "PathMove":
                playerPathMoveFrames++;
                break;

            case "DetourMove":
                playerDetourMoveFrames++;
                break;

            case "BlockedInput":
                playerBlockedInputFrames++;
                break;

            case "HardStop":
                playerHardStopFrames++;
                break;
        }

        if (string.IsNullOrEmpty(lastObservedPlayerAction))
        {
            lastObservedPlayerAction = action;
            return;
        }

        if (!string.Equals(lastObservedPlayerAction, action, System.StringComparison.Ordinal))
        {
            playerActionChangeCount++;
            lastObservedPlayerAction = action;
        }
    }

    private string BuildPlayerActionSummary()
    {
        return $"pathMoveFrames={playerPathMoveFrames}, detourMoveFrames={playerDetourMoveFrames}, " +
               $"blockedInputFrames={playerBlockedInputFrames}, hardStopFrames={playerHardStopFrames}, " +
               $"actionChanges={playerActionChangeCount}, lastAction={lastObservedPlayerAction}";
    }

    private void TickPlayerAvoidsMovingNpc()
    {
        Rigidbody2D playerRb = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Rigidbody2D>() : null;
        Vector2 playerPos = GetActorFootPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerRb);
        Vector2 npcPos = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        Vector2 playerTransformPos = GetActorTransformPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null);
        Vector2 npcTransformPos = GetActorTransformPosition(activeNpcA.Transform);
        float clearance = GetActorEdgeClearance(
            activePlayerNavigator != null ? activePlayerNavigator.transform : null,
            playerRb,
            activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Collider2D>() : null,
            activeNpcA.Transform,
            activeNpcA.Rigidbody,
            activeNpcA.Collider);

        minClearance = Mathf.Min(minClearance, clearance);
        maxPlayerLateralOffset = Mathf.Max(maxPlayerLateralOffset, Mathf.Abs(playerPos.y - laneReferenceY));
        playerReached = !activePlayerNavigator.IsActive && Vector2.Distance(playerTransformPos, playerTarget) <= 0.7f;
        npcReached = !activeNpcA.Controller.IsMoving && Vector2.Distance(npcTransformPos, npcTarget) <= 0.3f;

        if (playerReached && npcReached)
        {
            bool passed = npcMoveIssued && minClearance > -0.08f;
            CompleteScenario(
                "PlayerAvoidsMovingNpc",
                passed,
                $"npcMoveIssued={npcMoveIssued}, playerReached={playerReached}, npcReached={npcReached}, " +
                $"minClearance={minClearance:F3}, maxPlayerLateralOffset={maxPlayerLateralOffset:F3}, timeout={scenarioTimer:F2}",
                $"minClearance={minClearance:F3} playerReached={playerReached} npcReached={npcReached}");
            return;
        }

        if (scenarioTimer >= ScenarioTimeout)
        {
            CompleteScenario(
                "PlayerAvoidsMovingNpc",
                false,
                $"npcMoveIssued={npcMoveIssued}, playerReached={playerReached}, npcReached={npcReached}, " +
                $"minClearance={minClearance:F3}, maxPlayerLateralOffset={maxPlayerLateralOffset:F3}, timeout={scenarioTimer:F2}",
                $"timeout={scenarioTimer:F2} minClearance={minClearance:F3} playerReached={playerReached} npcReached={npcReached}");
        }
    }

    private void TickNpcAvoidsPlayer()
    {
        Rigidbody2D playerRb = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Rigidbody2D>() : null;
        Vector2 playerPos = GetActorFootPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerRb);
        Vector2 npcPos = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        Vector2 npcTransformPos = GetActorTransformPosition(activeNpcA.Transform);
        string npcAvoidanceDebug =
            activeNpcA.Controller != null
                ? $"detourActive={activeNpcA.Controller.DebugHasSharedAvoidanceDetour}, " +
                  $"overrideOwner={activeNpcA.Controller.DebugOverrideWaypointOwnerId}, " +
                  $"lastDetourOwner={activeNpcA.Controller.DebugLastDetourOwnerId}, " +
                  $"detourCreates={activeNpcA.Controller.DebugSharedAvoidanceDetourCreateCount}, " +
                  $"releaseAttempts={activeNpcA.Controller.DebugSharedAvoidanceReleaseAttemptCount}, " +
                  $"releaseSuccesses={activeNpcA.Controller.DebugSharedAvoidanceReleaseSuccessCount}, " +
                  $"noBlockerFrames={activeNpcA.Controller.DebugSharedAvoidanceNoBlockerFrames}, " +
                  $"blockingFrames={activeNpcA.Controller.DebugSharedAvoidanceBlockingFrames}, " +
                  $"recoveryOk={activeNpcA.Controller.DebugLastDetourRecoverySucceeded}"
                : "npcAvoidance=missing";
        string npcAvoidanceSummary =
            activeNpcA.Controller != null
                ? $"detourActive={activeNpcA.Controller.DebugHasSharedAvoidanceDetour} " +
                  $"detourCreates={activeNpcA.Controller.DebugSharedAvoidanceDetourCreateCount} " +
                  $"releaseAttempts={activeNpcA.Controller.DebugSharedAvoidanceReleaseAttemptCount} " +
                  $"releaseSuccesses={activeNpcA.Controller.DebugSharedAvoidanceReleaseSuccessCount} " +
                  $"noBlockerFrames={activeNpcA.Controller.DebugSharedAvoidanceNoBlockerFrames} " +
                  $"blockingFrames={activeNpcA.Controller.DebugSharedAvoidanceBlockingFrames} " +
                  $"recoveryOk={activeNpcA.Controller.DebugLastDetourRecoverySucceeded}"
                : "npcAvoidance=missing";
        float clearance = GetActorEdgeClearance(
            activePlayerNavigator != null ? activePlayerNavigator.transform : null,
            playerRb,
            activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Collider2D>() : null,
            activeNpcA.Transform,
            activeNpcA.Rigidbody,
            activeNpcA.Collider);

        minClearance = Mathf.Min(minClearance, clearance);
        maxNpcLateralOffset = Mathf.Max(maxNpcLateralOffset, Mathf.Abs(npcPos.y - laneReferenceY));
        npcReached = !activeNpcA.Controller.IsMoving && Vector2.Distance(npcTransformPos, npcTarget) <= 0.3f;

        if (npcReached)
        {
            bool passed = npcMoveIssued && minClearance > -0.08f;
            CompleteScenario(
                "NpcAvoidsPlayer",
                passed,
                $"npcMoveIssued={npcMoveIssued}, npcReached={npcReached}, " +
                $"minClearance={minClearance:F3}, maxNpcLateralOffset={maxNpcLateralOffset:F3}, timeout={scenarioTimer:F2}, " +
                npcAvoidanceDebug,
                $"minClearance={minClearance:F3} npcReached={npcReached} {npcAvoidanceSummary}");
            return;
        }

        if (scenarioTimer >= ScenarioTimeout)
        {
            CompleteScenario(
                "NpcAvoidsPlayer",
                false,
                $"npcMoveIssued={npcMoveIssued}, npcReached={npcReached}, " +
                $"minClearance={minClearance:F3}, maxNpcLateralOffset={maxNpcLateralOffset:F3}, timeout={scenarioTimer:F2}, " +
                npcAvoidanceDebug,
                $"timeout={scenarioTimer:F2} minClearance={minClearance:F3} npcReached={npcReached} {npcAvoidanceSummary}");
        }
    }

    private void TickNpcNpcCrossing()
    {
        Vector2 npcAPos = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        Vector2 npcBPos = GetActorFootPosition(activeNpcB.Transform, activeNpcB.Rigidbody);
        Vector2 npcATransformPos = GetActorTransformPosition(activeNpcA.Transform);
        Vector2 npcBTransformPos = GetActorTransformPosition(activeNpcB.Transform);
        float clearance = GetActorEdgeClearance(
            activeNpcA.Transform,
            activeNpcA.Rigidbody,
            activeNpcA.Collider,
            activeNpcB.Transform,
            activeNpcB.Rigidbody,
            activeNpcB.Collider);

        minClearance = Mathf.Min(minClearance, clearance);
        npcAReached = !activeNpcA.Controller.IsMoving && Vector2.Distance(npcATransformPos, npcATarget) <= 0.3f;
        npcBReached = !activeNpcB.Controller.IsMoving && Vector2.Distance(npcBTransformPos, npcBTarget) <= 0.3f;

        if (npcAReached && npcBReached)
        {
            bool passed = npcAMoveIssued && npcBMoveIssued && minClearance > -0.08f;
            CompleteScenario(
                "NpcNpcCrossing",
                passed,
                $"npcAMoveIssued={npcAMoveIssued}, npcBMoveIssued={npcBMoveIssued}, " +
                $"npcAReached={npcAReached}, npcBReached={npcBReached}, minClearance={minClearance:F3}, timeout={scenarioTimer:F2}",
                $"minClearance={minClearance:F3} npcAReached={npcAReached} npcBReached={npcBReached}");
            return;
        }

        if (scenarioTimer >= ScenarioTimeout)
        {
            CompleteScenario(
                "NpcNpcCrossing",
                false,
                $"npcAMoveIssued={npcAMoveIssued}, npcBMoveIssued={npcBMoveIssued}, " +
                $"npcAReached={npcAReached}, npcBReached={npcBReached}, minClearance={minClearance:F3}, timeout={scenarioTimer:F2}",
                $"timeout={scenarioTimer:F2} minClearance={minClearance:F3} npcAReached={npcAReached} npcBReached={npcBReached}");
        }
    }

    private void TickNpcRoamRecoverWindow()
    {
        TickManagedNpcRoam(expectInterruption: false);
    }

    private void TickNpcRoamPersistentBlockInterrupt()
    {
        TickManagedNpcRoam(expectInterruption: true);
    }

    private void TickManagedNpcRoam(bool expectInterruption)
    {
        if (activeNpcA.Controller == null)
        {
            AbortRun($"scenario={currentScenario} npcA_missing");
            return;
        }

        if (!roamSeedAccepted)
        {
            TryAcceptManagedRoamSeed();
            if (!roamSeedAccepted)
            {
                if (scenarioTimer >= ScenarioTimeout)
                {
                    CompleteScenario(
                        currentScenario.ToString(),
                        false,
                        $"seedAccepted={roamSeedAccepted}, seedAttempts={roamSeedAttempts}, timeout={scenarioTimer:F2}",
                        $"seedAccepted={roamSeedAccepted} seedAttempts={roamSeedAttempts}");
                }

                return;
            }
        }

        UpdateManagedRoamObservation(activeNpcA.Controller);
        UpdateManagedRoamBlockers(expectInterruption);

        if (expectInterruption)
        {
            if (GetManagedRoamInterruptionFlag())
            {
                CompleteScenario(
                    "NpcRoamPersistentBlockInterrupt",
                    true,
                    BuildManagedRoamInterruptionDetails(),
                    BuildManagedRoamInterruptionSummary());
                return;
            }

            if (scenarioTimer >= ManagedRoamPersistentInterruptTimeout)
            {
                CompleteScenario(
                    "NpcRoamPersistentBlockInterrupt",
                    false,
                    BuildManagedRoamTimeoutDetails(expectInterruption: true),
                    $"timeout={scenarioTimer:F2} detourCreates={activeNpcA.Controller.DebugSharedAvoidanceDetourCreateCount} " +
                    $"releaseSuccesses={activeNpcA.Controller.DebugSharedAvoidanceReleaseSuccessCount} interruption={GetManagedRoamInterruptionFlag()}");
            }

            return;
        }

        if (GetManagedRoamInterruptionFlag())
        {
            CompleteScenario(
                "NpcRoamRecoverWindow",
                false,
                BuildManagedRoamInterruptionDetails(),
                BuildManagedRoamInterruptionSummary());
            return;
        }

        if (!activeNpcA.Controller.IsMoving && activeNpcA.Controller.CompletedShortPauseCount > 0)
        {
            bool passed = roamDetourCreated && roamReleaseSucceeded;
            CompleteScenario(
                "NpcRoamRecoverWindow",
                passed,
                BuildManagedRoamRecoveryDetails(),
                $"detourCreates={activeNpcA.Controller.DebugSharedAvoidanceDetourCreateCount} " +
                $"releaseSuccesses={activeNpcA.Controller.DebugSharedAvoidanceReleaseSuccessCount} " +
                $"completedShortPauses={activeNpcA.Controller.CompletedShortPauseCount} interruption={GetManagedRoamInterruptionFlag()}");
            return;
        }

        if (scenarioTimer >= ScenarioTimeout)
        {
            CompleteScenario(
                "NpcRoamRecoverWindow",
                false,
                BuildManagedRoamTimeoutDetails(expectInterruption: false),
                $"timeout={scenarioTimer:F2} detourCreates={activeNpcA.Controller.DebugSharedAvoidanceDetourCreateCount} " +
                $"releaseSuccesses={activeNpcA.Controller.DebugSharedAvoidanceReleaseSuccessCount} " +
                $"completedShortPauses={activeNpcA.Controller.CompletedShortPauseCount}");
        }
    }

    private void TryAcceptManagedRoamSeed()
    {
        if (activeNpcA.Controller == null || !activeNpcA.Controller.IsMoving)
        {
            return;
        }

        Vector2 candidateDestination = NpcRoamControllerDebugCompat.GetVector2(
            activeNpcA.Controller,
            "currentDestination",
            Vector2.zero);
        if (IsManagedRoamTargetAccepted(candidateDestination))
        {
            roamSeedAccepted = true;
            roamAcceptedSeed = roamSeedCursor - 1;
            roamAcceptedAtTime = scenarioTimer;
            roamAcceptedDestination = candidateDestination;
            Debug.Log(
                $"{LogPrefix} roam_target_accepted scenario={currentScenario} seed={roamAcceptedSeed} " +
                $"target={roamAcceptedDestination} start={roamNpcStartPosition}");
            return;
        }

        if (roamSeedAttempts >= ManagedRoamMaxSeedAttempts)
        {
            return;
        }

        BeginNextManagedRoamAttempt();
    }

    private bool IsManagedRoamTargetAccepted(Vector2 destination)
    {
        if (destination == Vector2.zero)
        {
            return false;
        }

        return destination.x - roamNpcStartPosition.x >= ManagedRoamAcceptEastDelta &&
               Mathf.Abs(destination.y - roamNpcStartPosition.y) <= ManagedRoamAcceptVerticalTolerance;
    }

    private void BeginNextManagedRoamAttempt()
    {
        if (activeNpcA.Controller == null)
        {
            return;
        }

        StopAndParkNpc(activeNpcA, roamNpcStartPosition);
        StopAndParkNpc(activeNpcB, GetManagedRoamBlockerParkingPosition(1));
        StopAndParkNpc(activeNpcC, GetManagedRoamBlockerParkingPosition(2));
        activeNpcA.Controller.SyncHomeAnchorToCurrentPosition();

        roamDetourCreated = false;
        roamReleaseSucceeded = false;
        roamBlockerReleased = false;
        Random.InitState(roamSeedCursor);
        roamSeedAttempts++;
        Debug.Log($"{LogPrefix} roam_seed_attempt scenario={currentScenario} seed={roamSeedCursor} attempt={roamSeedAttempts}");
        roamSeedCursor++;
        activeNpcA.Controller.StartRoam();
    }

    private void UpdateManagedRoamObservation(NPCAutoRoamController controller)
    {
        if (controller == null)
        {
            return;
        }

        roamDetourCreated |= controller.DebugSharedAvoidanceDetourCreateCount > 0 || controller.DebugHasSharedAvoidanceDetour;
        roamReleaseSucceeded |= controller.DebugSharedAvoidanceReleaseSuccessCount > 0;
    }

    private void UpdateManagedRoamBlockers(bool expectInterruption)
    {
        if (activeNpcA.Controller == null || activeNpcB.Controller == null)
        {
            return;
        }

        Vector2 npcPosition = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        Vector2 forward = roamAcceptedDestination - npcPosition;
        if (forward.sqrMagnitude < 0.0001f)
        {
            forward = roamAcceptedDestination - roamNpcStartPosition;
        }

        if (forward.sqrMagnitude < 0.0001f)
        {
            forward = Vector2.right;
        }

        forward.Normalize();
        Vector2 side = new Vector2(-forward.y, forward.x);

        if (!expectInterruption && !roamBlockerReleased)
        {
            PositionManagedBlocker(
                activeNpcB,
                npcPosition + forward * GetManagedRoamBlockerAheadDistance(activeNpcB));
            StopAndParkNpc(activeNpcC, GetManagedRoamBlockerParkingPosition(2));
            if (roamDetourCreated && scenarioTimer - roamAcceptedAtTime >= ManagedRoamReleaseHoldTime)
            {
                roamBlockerReleased = true;
                StopAndParkNpc(activeNpcB, GetManagedRoamBlockerParkingPosition(1));
            }

            return;
        }

        if (!expectInterruption)
        {
            StopAndParkNpc(activeNpcB, GetManagedRoamBlockerParkingPosition(1));
            StopAndParkNpc(activeNpcC, GetManagedRoamBlockerParkingPosition(2));
            return;
        }

        Vector2 persistentAnchor = GetManagedRoamPersistentAnchor();
        PositionManagedBlocker(
            activeNpcB,
            persistentAnchor + side * ManagedRoamBlockerSideOffset);
        PositionManagedBlocker(
            activeNpcC,
            persistentAnchor - side * ManagedRoamBlockerSideOffset);
    }

    private float GetManagedRoamBlockerAheadDistance(NPCMovementBundle blocker)
    {
        return
            GetActorFootprintRadius(activeNpcA.Collider) +
            GetActorFootprintRadius(blocker.Collider) +
            ManagedRoamBlockerClearance;
    }

    private Vector2 GetManagedRoamPersistentAnchor()
    {
        Vector2 forward = roamAcceptedDestination - roamNpcStartPosition;
        if (forward.sqrMagnitude < 0.0001f)
        {
            forward = Vector2.right;
        }

        return roamNpcStartPosition +
               forward.normalized * (GetManagedRoamBlockerAheadDistance(activeNpcB) + ManagedRoamPersistentAnchorOffset);
    }

    private void PositionManagedBlocker(NPCMovementBundle npc, Vector2 position)
    {
        if (npc.Controller == null)
        {
            return;
        }

        StopAndParkNpc(npc, position);
    }

    private static void StopAndParkNpc(NPCMovementBundle npc, Vector2 position)
    {
        if (npc.Controller != null)
        {
            npc.Controller.StopRoam();
        }

        if (npc.Transform != null)
        {
            PlaceNpc(npc, position);
        }
    }

    private static Vector2 GetManagedRoamBlockerParkingPosition(int blockerIndex)
    {
        return blockerIndex switch
        {
            1 => new Vector2(-3.5f, 8.0f),
            2 => new Vector2(-1.5f, 8.5f),
            _ => new Vector2(-0.5f, 9.0f)
        };
    }

    private static void ConfigureManagedRoamController(NPCAutoRoamController controller)
    {
        if (controller == null)
        {
            return;
        }

        NpcRoamControllerDebugCompat.TrySetFloat(controller, "shortPauseMin", ManagedRoamShortPauseDuration);
        NpcRoamControllerDebugCompat.TrySetFloat(controller, "shortPauseMax", ManagedRoamShortPauseDuration);
        NpcRoamControllerDebugCompat.TrySetFloat(controller, "minimumMoveDistance", ManagedRoamMinimumMoveDistance);
        NpcRoamControllerDebugCompat.TrySetBool(controller, "enableAmbientChat", false);
        NpcRoamControllerDebugCompat.TrySetBool(controller, "showDebugLog", true);
    }

    private static void ConfigureManagedRoamPrimaryController(NPCAutoRoamController controller, bool persistentBlockers)
    {
        if (controller == null || !persistentBlockers)
        {
            return;
        }

        NpcRoamControllerDebugCompat.TrySetFloat(controller, "stuckCheckInterval", ManagedRoamPersistentStuckCheckInterval);
        NpcRoamControllerDebugCompat.TrySetFloat(controller, "stuckDistanceThreshold", ManagedRoamPersistentStuckDistanceThreshold);
        NpcRoamControllerDebugCompat.TrySetInt(controller, "maxStuckRecoveries", ManagedRoamPersistentMaxStuckRecoveries);
    }

    private bool GetManagedRoamInterruptionFlag()
    {
        return NpcRoamControllerDebugCompat.GetBool(activeNpcA.Controller, "DebugHasRoamInterruption", false);
    }

    private string GetManagedRoamInterruptionReason()
    {
        return NpcRoamControllerDebugCompat.GetString(activeNpcA.Controller, "DebugLastRoamInterruptionReason", "n/a");
    }

    private string GetManagedRoamInterruptionTrigger()
    {
        return NpcRoamControllerDebugCompat.GetString(activeNpcA.Controller, "DebugLastRoamInterruptionTrigger", "n/a");
    }

    private string GetManagedRoamInterruptionBlockerKind()
    {
        return NpcRoamControllerDebugCompat.GetString(activeNpcA.Controller, "DebugLastRoamInterruptionBlockerKind", "n/a");
    }

    private int GetManagedRoamInterruptionBlockerId()
    {
        return NpcRoamControllerDebugCompat.GetInt(activeNpcA.Controller, "DebugLastRoamInterruptionBlockerId", -1);
    }

    private Vector2 GetManagedRoamInterruptionRequestedDestination()
    {
        return NpcRoamControllerDebugCompat.GetVector2(activeNpcA.Controller, "DebugLastRoamInterruptionRequestedDestination", Vector2.zero);
    }

    private Vector2 GetManagedRoamInterruptionActiveDestination()
    {
        return NpcRoamControllerDebugCompat.GetVector2(activeNpcA.Controller, "DebugLastRoamInterruptionActiveDestination", Vector2.zero);
    }

    private Vector2 GetManagedRoamInterruptionCurrentPosition()
    {
        return NpcRoamControllerDebugCompat.GetVector2(activeNpcA.Controller, "DebugLastRoamInterruptionCurrentPosition", Vector2.zero);
    }

    private bool TryGetManagedRoamInterruptionBlockerPosition(out Vector2 blockerPosition)
    {
        blockerPosition = NpcRoamControllerDebugCompat.GetVector2(
            activeNpcA.Controller,
            "DebugLastRoamInterruptionBlockerPosition",
            Vector2.zero);
        return NpcRoamControllerDebugCompat.GetBool(
            activeNpcA.Controller,
            "DebugLastRoamInterruptionHasBlockerPosition",
            false);
    }

    private string BuildManagedRoamRecoveryDetails()
    {
        return
            $"seed={roamAcceptedSeed}, target={roamAcceptedDestination}, " +
            $"detourCreates={activeNpcA.Controller.DebugSharedAvoidanceDetourCreateCount}, " +
            $"releaseAttempts={activeNpcA.Controller.DebugSharedAvoidanceReleaseAttemptCount}, " +
            $"releaseSuccesses={activeNpcA.Controller.DebugSharedAvoidanceReleaseSuccessCount}, " +
            $"completedShortPauses={activeNpcA.Controller.CompletedShortPauseCount}, " +
            $"interruption={GetManagedRoamInterruptionFlag()}, " +
            $"state={activeNpcA.Controller.DebugState}, timeout={scenarioTimer:F2}";
    }

    private string BuildManagedRoamTimeoutDetails(bool expectInterruption)
    {
        return
            $"seed={roamAcceptedSeed}, target={roamAcceptedDestination}, seedAccepted={roamSeedAccepted}, " +
            $"detourCreates={activeNpcA.Controller.DebugSharedAvoidanceDetourCreateCount}, " +
            $"releaseAttempts={activeNpcA.Controller.DebugSharedAvoidanceReleaseAttemptCount}, " +
            $"releaseSuccesses={activeNpcA.Controller.DebugSharedAvoidanceReleaseSuccessCount}, " +
            $"completedShortPauses={activeNpcA.Controller.CompletedShortPauseCount}, " +
            $"interruption={GetManagedRoamInterruptionFlag()}, " +
            $"expectInterruption={expectInterruption}, timeout={scenarioTimer:F2}";
    }

    private string BuildManagedRoamInterruptionDetails()
    {
        string blockerPosition = TryGetManagedRoamInterruptionBlockerPosition(out Vector2 blockerValue)
            ? blockerValue.ToString()
            : "n/a";

        return
            $"seed={roamAcceptedSeed}, target={roamAcceptedDestination}, " +
            $"detourCreates={activeNpcA.Controller.DebugSharedAvoidanceDetourCreateCount}, " +
            $"releaseAttempts={activeNpcA.Controller.DebugSharedAvoidanceReleaseAttemptCount}, " +
            $"releaseSuccesses={activeNpcA.Controller.DebugSharedAvoidanceReleaseSuccessCount}, " +
            $"reason={GetManagedRoamInterruptionReason()}, " +
            $"trigger={GetManagedRoamInterruptionTrigger()}, " +
            $"blockerKind={GetManagedRoamInterruptionBlockerKind()}, " +
            $"blockerId={GetManagedRoamInterruptionBlockerId()}, " +
            $"requested={GetManagedRoamInterruptionRequestedDestination()}, " +
            $"active={GetManagedRoamInterruptionActiveDestination()}, " +
            $"current={GetManagedRoamInterruptionCurrentPosition()}, " +
            $"blocker={blockerPosition}, " +
            $"timeout={scenarioTimer:F2}";
    }

    private string BuildManagedRoamInterruptionSummary()
    {
        return
            $"reason={GetManagedRoamInterruptionReason()} " +
            $"trigger={GetManagedRoamInterruptionTrigger()} " +
            $"blockerKind={GetManagedRoamInterruptionBlockerKind()} " +
            $"blockerId={GetManagedRoamInterruptionBlockerId()}";
    }

    private void TickRealInputPlayerAvoidsMovingNpc()
    {
        Rigidbody2D playerRb = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Rigidbody2D>() : null;
        Collider2D playerCollider = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Collider2D>() : null;
        Vector2 playerPos = GetActorFootPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerRb);
        Vector2 npcPos = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        Vector2 playerTransformPos = GetActorTransformPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null);
        Vector2 playerCenterPos = GetActorMeasurePosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerCollider);
        Vector2 npcTransformPos = GetActorTransformPosition(activeNpcA.Transform);
        float playerTransformDistance = Vector2.Distance(playerTransformPos, playerTarget);
        float playerRigidbodyDistance = Vector2.Distance(playerPos, playerTarget);
        float playerCenterDistance = Vector2.Distance(playerCenterPos, playerTarget);
        float clearance = GetActorEdgeClearance(
            activePlayerNavigator != null ? activePlayerNavigator.transform : null,
            playerRb,
            playerCollider,
            activeNpcA.Transform,
            activeNpcA.Rigidbody,
            activeNpcA.Collider);

        minClearance = Mathf.Min(minClearance, clearance);
        maxPlayerLateralOffset = Mathf.Max(maxPlayerLateralOffset, Mathf.Abs(playerPos.y - laneReferenceY));
        UpdateNpcPushMetrics(playerPos, npcPos, clearance);
        ObservePlayerNavigationAction();
        UpdateBlockOnsetMetric(playerPos, clearance, playerTarget);

        playerReached = activePlayerNavigator != null &&
            !activePlayerNavigator.IsActive &&
            playerCenterDistance <= PlayerPointArrivalCenterTolerance;
        npcReached = !activeNpcA.Controller.IsMoving && Vector2.Distance(npcTransformPos, npcTarget) <= 0.3f;

        if (playerReached && npcReached)
        {
            float onsetDistance = blockOnsetRecorded ? blockOnsetEdgeDistance : float.PositiveInfinity;
            bool passed =
                playerClickIssued &&
                npcMoveIssued &&
                minClearance > -0.08f &&
                npcPushDisplacement <= MaxAllowedNpcPushDisplacement;
            CompleteScenario(
                "RealInputPlayerAvoidsMovingNpc",
                passed,
                $"playerClickIssued={playerClickIssued}, npcMoveIssued={npcMoveIssued}, " +
                $"playerReached={playerReached}, npcReached={npcReached}, minClearance={minClearance:F3}, " +
                $"playerTransformDistance={playerTransformDistance:F3}, playerRigidbodyDistance={playerRigidbodyDistance:F3}, playerCenterDistance={playerCenterDistance:F3}, " +
                $"blockOnsetEdgeDistance={(float.IsPositiveInfinity(onsetDistance) ? "n/a" : onsetDistance.ToString("F3"))}, " +
                $"maxPlayerLateralOffset={maxPlayerLateralOffset:F3}, npcPushDisplacement={npcPushDisplacement:F3}, " +
                $"pushContactFrames={npcPushContactFrames}, timeout={scenarioTimer:F2}, " +
                $"{BuildClickDispatchSummary()}, {BuildPlayerActionSummary()}",
                $"minClearance={minClearance:F3} playerCenterDistance={playerCenterDistance:F3} blockOnsetEdgeDistance={(float.IsPositiveInfinity(onsetDistance) ? "n/a" : onsetDistance.ToString("F3"))} pushDisplacement={npcPushDisplacement:F3} " +
                $"playerReached={playerReached} npcReached={npcReached}");
            return;
        }

        if (scenarioTimer >= ScenarioTimeout)
        {
            float onsetDistance = blockOnsetRecorded ? blockOnsetEdgeDistance : float.PositiveInfinity;
            CompleteScenario(
                "RealInputPlayerAvoidsMovingNpc",
                false,
                $"playerClickIssued={playerClickIssued}, npcMoveIssued={npcMoveIssued}, " +
                $"playerReached={playerReached}, npcReached={npcReached}, minClearance={minClearance:F3}, " +
                $"playerTransformDistance={playerTransformDistance:F3}, playerRigidbodyDistance={playerRigidbodyDistance:F3}, playerCenterDistance={playerCenterDistance:F3}, " +
                $"blockOnsetEdgeDistance={(float.IsPositiveInfinity(onsetDistance) ? "n/a" : onsetDistance.ToString("F3"))}, " +
                $"maxPlayerLateralOffset={maxPlayerLateralOffset:F3}, npcPushDisplacement={npcPushDisplacement:F3}, " +
                $"pushContactFrames={npcPushContactFrames}, timeout={scenarioTimer:F2}, " +
                $"{BuildClickDispatchSummary()}, {BuildPlayerActionSummary()}",
                $"timeout={scenarioTimer:F2} minClearance={minClearance:F3} blockOnsetEdgeDistance={(float.IsPositiveInfinity(onsetDistance) ? "n/a" : onsetDistance.ToString("F3"))} " +
                $"playerCenterDistance={playerCenterDistance:F3} pushDisplacement={npcPushDisplacement:F3} playerReached={playerReached} npcReached={npcReached}");
        }
    }

    private void TickRealInputPlayerSingleNpcNear()
    {
        Rigidbody2D playerRb = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Rigidbody2D>() : null;
        Collider2D playerCollider = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Collider2D>() : null;
        Vector2 playerPos = GetActorFootPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerRb);
        Vector2 npcPos = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        Vector2 playerTransformPos = GetActorTransformPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null);
        Vector2 playerCenterPos = GetActorMeasurePosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerCollider);
        float playerTransformDistance = Vector2.Distance(playerTransformPos, playerTarget);
        float playerRigidbodyDistance = Vector2.Distance(playerPos, playerTarget);
        float playerCenterDistance = Vector2.Distance(playerCenterPos, playerTarget);
        float edgeClearance = GetActorEdgeClearance(
            activePlayerNavigator != null ? activePlayerNavigator.transform : null,
            playerRb,
            playerCollider,
            activeNpcA.Transform,
            activeNpcA.Rigidbody,
            activeNpcA.Collider);

        minClearance = Mathf.Min(minClearance, edgeClearance);
        maxPlayerLateralOffset = Mathf.Max(maxPlayerLateralOffset, Mathf.Abs(playerPos.y - laneReferenceY));
        UpdateNpcPushMetrics(playerPos, npcPos, edgeClearance);
        ObservePlayerNavigationAction();
        playerReached = activePlayerNavigator != null &&
            !activePlayerNavigator.IsActive &&
            playerCenterDistance <= PlayerPointArrivalCenterTolerance;

        UpdateBlockOnsetMetric(playerPos, edgeClearance, playerTarget);

        if (playerReached)
        {
            float onsetDistance = blockOnsetRecorded ? blockOnsetEdgeDistance : 0f;
            bool passed =
                playerClickIssued &&
                minClearance > -0.06f &&
                onsetDistance <= 0.18f;
            CompleteScenario(
                "RealInputPlayerSingleNpcNear",
                passed,
                $"playerClickIssued={playerClickIssued}, playerReached={playerReached}, minEdgeClearance={minClearance:F3}, " +
                $"playerTransformDistance={playerTransformDistance:F3}, playerRigidbodyDistance={playerRigidbodyDistance:F3}, playerCenterDistance={playerCenterDistance:F3}, " +
                $"blockOnsetEdgeDistance={onsetDistance:F3}, maxPlayerLateralOffset={maxPlayerLateralOffset:F3}, " +
                $"npcPushDisplacement={npcPushDisplacement:F3}, pushContactFrames={npcPushContactFrames}, timeout={scenarioTimer:F2}, " +
                $"{BuildClickDispatchSummary()}, {BuildPlayerActionSummary()}",
                $"minEdgeClearance={minClearance:F3} playerCenterDistance={playerCenterDistance:F3} blockOnsetEdgeDistance={onsetDistance:F3} " +
                $"npcPushDisplacement={npcPushDisplacement:F3} playerReached={playerReached}");
            return;
        }

        if (scenarioTimer >= ScenarioTimeout)
        {
            float onsetDistance = blockOnsetRecorded ? blockOnsetEdgeDistance : float.PositiveInfinity;
            CompleteScenario(
                "RealInputPlayerSingleNpcNear",
                false,
                $"playerClickIssued={playerClickIssued}, playerReached={playerReached}, minEdgeClearance={minClearance:F3}, " +
                $"playerTransformDistance={playerTransformDistance:F3}, playerRigidbodyDistance={playerRigidbodyDistance:F3}, playerCenterDistance={playerCenterDistance:F3}, " +
                $"blockOnsetEdgeDistance={(float.IsPositiveInfinity(onsetDistance) ? "n/a" : onsetDistance.ToString("F3"))}, " +
                $"maxPlayerLateralOffset={maxPlayerLateralOffset:F3}, npcPushDisplacement={npcPushDisplacement:F3}, " +
                $"pushContactFrames={npcPushContactFrames}, timeout={scenarioTimer:F2}, " +
                $"{BuildClickDispatchSummary()}, {BuildPlayerActionSummary()}",
                $"timeout={scenarioTimer:F2} minEdgeClearance={minClearance:F3} " +
                $"playerCenterDistance={playerCenterDistance:F3} blockOnsetEdgeDistance={(float.IsPositiveInfinity(onsetDistance) ? "n/a" : onsetDistance.ToString("F3"))} " +
                $"npcPushDisplacement={npcPushDisplacement:F3} playerReached={playerReached}");
        }
    }

    private void TickRealInputPlayerCrowdPass()
    {
        const string scenarioName = "LegacyRealInputCrowdBlockedWallStress";
        Rigidbody2D playerRb = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Rigidbody2D>() : null;
        Collider2D playerCollider = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Collider2D>() : null;
        Vector2 playerPos = GetActorFootPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerRb);
        Vector2 playerTransformPos = GetActorTransformPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null);
        Vector2 playerCenterPos = GetActorMeasurePosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerCollider);
        float playerTransformDistance = Vector2.Distance(playerTransformPos, playerTarget);
        float playerRigidbodyDistance = Vector2.Distance(playerPos, playerTarget);
        float playerCenterDistance = Vector2.Distance(playerCenterPos, playerTarget);
        float nearestEdgeClearance = Mathf.Min(
            GetActorEdgeClearance(
                activePlayerNavigator != null ? activePlayerNavigator.transform : null,
                playerRb,
                playerCollider,
                activeNpcA.Transform,
                activeNpcA.Rigidbody,
                activeNpcA.Collider),
            Mathf.Min(
                GetActorEdgeClearance(
                    activePlayerNavigator != null ? activePlayerNavigator.transform : null,
                    playerRb,
                    playerCollider,
                    activeNpcB.Transform,
                    activeNpcB.Rigidbody,
                    activeNpcB.Collider),
                GetActorEdgeClearance(
                    activePlayerNavigator != null ? activePlayerNavigator.transform : null,
                    playerRb,
                    playerCollider,
                    activeNpcC.Transform,
                    activeNpcC.Rigidbody,
                    activeNpcC.Collider)));

        minClearance = Mathf.Min(minClearance, nearestEdgeClearance);
        ObservePlayerNavigationAction();
        playerReached = activePlayerNavigator != null &&
            !activePlayerNavigator.IsActive &&
            playerCenterDistance <= PlayerPointArrivalCenterTolerance;

        UpdateCrowdOscillationMetrics(playerPos, playerTarget);

        if (playerReached)
        {
            bool passed =
                playerClickIssued &&
                minClearance > -0.08f &&
                playerDirectionFlipCount <= 2 &&
                crowdStallDuration <= 0.45f;
            CompleteScenario(
                scenarioName,
                passed,
                $"category=legacy-blocked-wall-stress, playerClickIssued={playerClickIssued}, playerReached={playerReached}, minEdgeClearance={minClearance:F3}, " +
                $"playerTransformDistance={playerTransformDistance:F3}, playerRigidbodyDistance={playerRigidbodyDistance:F3}, playerCenterDistance={playerCenterDistance:F3}, " +
                $"directionFlips={playerDirectionFlipCount}, crowdStallDuration={crowdStallDuration:F3}, timeout={scenarioTimer:F2}, " +
                $"{BuildClickDispatchSummary()}, {BuildPlayerActionSummary()}",
                $"category=legacy-blocked-wall-stress minEdgeClearance={minClearance:F3} playerCenterDistance={playerCenterDistance:F3} directionFlips={playerDirectionFlipCount} " +
                $"crowdStallDuration={crowdStallDuration:F3} playerReached={playerReached}");
            return;
        }

        if (scenarioTimer >= ScenarioTimeout)
        {
            CompleteScenario(
                scenarioName,
                false,
                $"category=legacy-blocked-wall-stress, playerClickIssued={playerClickIssued}, playerReached={playerReached}, minEdgeClearance={minClearance:F3}, " +
                $"playerTransformDistance={playerTransformDistance:F3}, playerRigidbodyDistance={playerRigidbodyDistance:F3}, playerCenterDistance={playerCenterDistance:F3}, " +
                $"directionFlips={playerDirectionFlipCount}, crowdStallDuration={crowdStallDuration:F3}, timeout={scenarioTimer:F2}, " +
                $"{BuildClickDispatchSummary()}, {BuildPlayerActionSummary()}",
                $"category=legacy-blocked-wall-stress timeout={scenarioTimer:F2} minEdgeClearance={minClearance:F3} playerCenterDistance={playerCenterDistance:F3} " +
                $"directionFlips={playerDirectionFlipCount} crowdStallDuration={crowdStallDuration:F3} playerReached={playerReached}");
        }
    }

    private static string ClassifyPassableCorridorOutcome(
        bool interactionHijack,
        bool reachedCorridorTarget,
        bool bulldoze,
        bool oscillation,
        bool slowCrawl,
        bool timedOut)
    {
        if (interactionHijack)
        {
            return "InteractionHijack";
        }

        if (bulldoze)
        {
            return "Bulldoze";
        }

        if (oscillation)
        {
            return "Oscillation";
        }

        if (slowCrawl)
        {
            return "SlowCrawl";
        }

        if (reachedCorridorTarget)
        {
            return "ReachedPassableCorridor";
        }

        return timedOut ? "TimedOutBeforeCorridorArrival" : "CorridorInProgress";
    }

    private void TickRealInputPlayerPassableCorridor()
    {
        const string scenarioName = "RealInputPlayerPassableCorridor";
        const int passableCorridorMaxDirectionFlips = 2;
        const int passableCorridorMaxActionChanges = 12;
        const float passableCorridorMaxStallDuration = 0.45f;

        Rigidbody2D playerRb = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Rigidbody2D>() : null;
        Collider2D playerCollider = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Collider2D>() : null;
        Vector2 playerPos = GetActorFootPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerRb);
        Vector2 playerTransformPos = GetActorTransformPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null);
        Vector2 playerCenterPos = GetActorMeasurePosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerCollider);
        float playerTransformDistance = Vector2.Distance(playerTransformPos, playerTarget);
        float playerRigidbodyDistance = Vector2.Distance(playerPos, playerTarget);
        float playerCenterDistance = Vector2.Distance(playerCenterPos, playerTarget);
        float nearestEdgeClearance = Mathf.Min(
            GetActorEdgeClearance(
                activePlayerNavigator != null ? activePlayerNavigator.transform : null,
                playerRb,
                playerCollider,
                activeNpcA.Transform,
                activeNpcA.Rigidbody,
                activeNpcA.Collider),
            Mathf.Min(
                GetActorEdgeClearance(
                    activePlayerNavigator != null ? activePlayerNavigator.transform : null,
                    playerRb,
                    playerCollider,
                    activeNpcB.Transform,
                    activeNpcB.Rigidbody,
                    activeNpcB.Collider),
                GetActorEdgeClearance(
                    activePlayerNavigator != null ? activePlayerNavigator.transform : null,
                    playerRb,
                    playerCollider,
                    activeNpcC.Transform,
                    activeNpcC.Rigidbody,
                    activeNpcC.Collider)));

        minClearance = Mathf.Min(minClearance, nearestEdgeClearance);
        ObservePlayerNavigationAction();
        UpdateCrowdOscillationMetrics(playerPos, playerTarget);

        bool navigatorInactive = activePlayerNavigator != null && !activePlayerNavigator.IsActive;
        bool reachedCorridorTarget = navigatorInactive &&
            playerCenterDistance <= PlayerPointArrivalCenterTolerance;
        bool interactionHijack = clickPendingAutoInteractionAfterIssue;
        bool caseValid = playerClickIssued && !interactionHijack;
        bool bulldoze = minClearance <= -0.06f;
        bool oscillation = playerDirectionFlipCount > passableCorridorMaxDirectionFlips ||
            playerActionChangeCount > passableCorridorMaxActionChanges;
        bool slowCrawl = crowdStallDuration > passableCorridorMaxStallDuration;
        bool timedOut = scenarioTimer >= ScenarioTimeout;
        string outcome = ClassifyPassableCorridorOutcome(
            interactionHijack,
            reachedCorridorTarget,
            bulldoze,
            oscillation,
            slowCrawl,
            timedOut);

        if (reachedCorridorTarget || timedOut)
        {
            bool passed = caseValid && outcome == "ReachedPassableCorridor";
            CompleteScenario(
                scenarioName,
                passed,
                $"playerClickIssued={playerClickIssued}, caseValid={caseValid}, outcome={outcome}, " +
                $"playerReached={reachedCorridorTarget}, minEdgeClearance={minClearance:F3}, " +
                $"playerTransformDistance={playerTransformDistance:F3}, playerRigidbodyDistance={playerRigidbodyDistance:F3}, " +
                $"playerCenterDistance={playerCenterDistance:F3}, directionFlips={playerDirectionFlipCount}, " +
                $"actionChanges={playerActionChangeCount}, crowdStallDuration={crowdStallDuration:F3}, timeout={scenarioTimer:F2}, " +
                $"{BuildClickDispatchSummary()}, {BuildPlayerActionSummary()}",
                $"pass={passed} caseValid={caseValid} outcome={outcome} minEdgeClearance={minClearance:F3} " +
                $"playerCenterDistance={playerCenterDistance:F3} directionFlips={playerDirectionFlipCount} " +
                $"actionChanges={playerActionChangeCount} crowdStallDuration={crowdStallDuration:F3}");
        }
    }

    private static string ClassifyStaticNpcWallOutcome(
        bool interactionHijack,
        bool reachedBlockedTarget,
        bool bulldoze,
        bool oscillation,
        bool stableHoldBeforeWall,
        bool inactiveWithoutMeaningfulHold,
        bool timedOut,
        bool meaningfulProgress)
    {
        if (interactionHijack)
        {
            return "InteractionHijack";
        }

        if (reachedBlockedTarget)
        {
            return "ReachedBlockedTarget";
        }

        if (bulldoze)
        {
            return "Bulldoze";
        }

        if (oscillation)
        {
            return "Oscillation";
        }

        if (stableHoldBeforeWall)
        {
            return "StableHoldBeforeWall";
        }

        if (inactiveWithoutMeaningfulHold)
        {
            return meaningfulProgress ? "InactiveOutsideWallContract" : "NoMeaningfulBlockedResponse";
        }

        return timedOut ? "TimedOutActiveWallLinger" : "WallApproachInProgress";
    }

    private void TickRealInputPlayerStaticNpcWall()
    {
        const string scenarioName = "RealInputPlayerStaticNpcWall";
        const float staticNpcWallStartX = -9.15f;
        const float staticNpcWallMinForwardProgress = 0.85f;
        const float staticNpcWallFrontBuffer = 0.10f;
        const float staticNpcWallSafeMinClearance = -0.04f;
        const float staticNpcWallMaxStallDuration = 0.65f;
        const int staticNpcWallMaxDirectionFlips = 2;
        const int staticNpcWallMaxActionChanges = 12;

        Rigidbody2D playerRb = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Rigidbody2D>() : null;
        Collider2D playerCollider = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Collider2D>() : null;
        Vector2 playerPos = GetActorFootPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerRb);
        Vector2 playerTransformPos = GetActorTransformPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null);
        Vector2 playerCenterPos = GetActorMeasurePosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerCollider);
        float playerTransformDistance = Vector2.Distance(playerTransformPos, playerTarget);
        float playerRigidbodyDistance = Vector2.Distance(playerPos, playerTarget);
        float playerCenterDistance = Vector2.Distance(playerCenterPos, playerTarget);
        float nearestEdgeClearance = Mathf.Min(
            GetActorEdgeClearance(
                activePlayerNavigator != null ? activePlayerNavigator.transform : null,
                playerRb,
                playerCollider,
                activeNpcA.Transform,
                activeNpcA.Rigidbody,
                activeNpcA.Collider),
            Mathf.Min(
                GetActorEdgeClearance(
                    activePlayerNavigator != null ? activePlayerNavigator.transform : null,
                    playerRb,
                    playerCollider,
                    activeNpcB.Transform,
                    activeNpcB.Rigidbody,
                    activeNpcB.Collider),
                GetActorEdgeClearance(
                    activePlayerNavigator != null ? activePlayerNavigator.transform : null,
                    playerRb,
                    playerCollider,
                    activeNpcC.Transform,
                    activeNpcC.Rigidbody,
                    activeNpcC.Collider)));
        float wallFrontX = Mathf.Min(
            activeNpcA.Transform.position.x,
            Mathf.Min(activeNpcB.Transform.position.x, activeNpcC.Transform.position.x));

        minClearance = Mathf.Min(minClearance, nearestEdgeClearance);
        ObservePlayerNavigationAction();
        UpdateCrowdOscillationMetrics(playerPos, playerTarget);
        UpdateBlockOnsetMetric(playerPos, nearestEdgeClearance, playerTarget);

        bool navigatorInactive = activePlayerNavigator != null && !activePlayerNavigator.IsActive;
        bool reachedBlockedTarget = navigatorInactive &&
            playerCenterDistance <= PlayerPointArrivalCenterTolerance;
        float forwardProgress = playerPos.x - staticNpcWallStartX;
        bool meaningfulProgress = forwardProgress >= staticNpcWallMinForwardProgress;
        bool stillBeforeWall = playerPos.x <= wallFrontX - staticNpcWallFrontBuffer;
        bool interactionHijack = clickPendingAutoInteractionAfterIssue;
        bool caseValid = playerClickIssued && !interactionHijack;
        bool bulldoze = minClearance <= -0.06f;
        bool oscillation = playerDirectionFlipCount > staticNpcWallMaxDirectionFlips ||
            playerActionChangeCount > staticNpcWallMaxActionChanges ||
            crowdStallDuration > staticNpcWallMaxStallDuration;
        bool stableHoldBeforeWall = navigatorInactive &&
            !reachedBlockedTarget &&
            meaningfulProgress &&
            stillBeforeWall &&
            minClearance > staticNpcWallSafeMinClearance &&
            !oscillation;
        bool inactiveWithoutMeaningfulHold = navigatorInactive &&
            !reachedBlockedTarget &&
            !stableHoldBeforeWall;
        bool timedOut = scenarioTimer >= ScenarioTimeout;
        string outcome = ClassifyStaticNpcWallOutcome(
            interactionHijack,
            reachedBlockedTarget,
            bulldoze,
            oscillation,
            stableHoldBeforeWall,
            inactiveWithoutMeaningfulHold,
            timedOut,
            meaningfulProgress);

        if (reachedBlockedTarget || stableHoldBeforeWall || inactiveWithoutMeaningfulHold || timedOut)
        {
            bool passed = caseValid && outcome == "StableHoldBeforeWall";
            CompleteScenario(
                scenarioName,
                passed,
                $"playerClickIssued={playerClickIssued}, caseValid={caseValid}, outcome={outcome}, " +
                $"playerReached={reachedBlockedTarget}, stableHoldBeforeWall={stableHoldBeforeWall}, " +
                $"meaningfulProgress={meaningfulProgress}, forwardProgress={forwardProgress:F3}, wallFrontX={wallFrontX:F3}, " +
                $"playerTransformDistance={playerTransformDistance:F3}, playerRigidbodyDistance={playerRigidbodyDistance:F3}, " +
                $"playerCenterDistance={playerCenterDistance:F3}, minEdgeClearance={minClearance:F3}, " +
                $"directionFlips={playerDirectionFlipCount}, actionChanges={playerActionChangeCount}, " +
                $"crowdStallDuration={crowdStallDuration:F3}, timeout={scenarioTimer:F2}, " +
                $"{BuildClickDispatchSummary()}, {BuildPlayerActionSummary()}",
                $"pass={passed} caseValid={caseValid} outcome={outcome} forwardProgress={forwardProgress:F3} " +
                $"wallFrontX={wallFrontX:F3} playerCenterDistance={playerCenterDistance:F3} " +
                $"minEdgeClearance={minClearance:F3} directionFlips={playerDirectionFlipCount} " +
                $"actionChanges={playerActionChangeCount} crowdStallDuration={crowdStallDuration:F3}");
        }
    }

    private void TickRealInputGroundPointMatrix()
    {
        Rigidbody2D playerRb = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Rigidbody2D>() : null;
        Collider2D playerCollider = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Collider2D>() : null;
        Vector2 transformPos = GetActorTransformPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null);
        Vector2 rigidbodyPos = GetActorFootPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerRb);
        Vector2 colliderCenter = GetActorMeasurePosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerCollider);

        if (activePlayerNavigator != null && !activePlayerNavigator.IsActive)
        {
            CompleteGroundPointMatrixCase(false, transformPos, rigidbodyPos, colliderCenter);
            return;
        }

        if (scenarioTimer - groundPointCaseStartTimer >= ScenarioTimeout)
        {
            CompleteGroundPointMatrixCase(true, transformPos, rigidbodyPos, colliderCenter);
        }
    }

    private static string ClassifyEndpointNpcOccupiedOutcome(
        bool interactionHijack,
        bool endpointContractSatisfied,
        bool bulldoze,
        bool oscillation,
        bool stableHoldOutsideEndpoint)
    {
        if (interactionHijack)
        {
            return "InteractionHijack";
        }

        if (endpointContractSatisfied)
        {
            return "ReachedClickPoint";
        }

        if (bulldoze)
        {
            return "Bulldoze";
        }

        if (oscillation)
        {
            return "Oscillation";
        }

        if (stableHoldOutsideEndpoint)
        {
            return "StableHoldOutsideOccupiedEndpoint";
        }

        return "Linger";
    }

    private void CompleteEndpointNpcOccupiedScenario(
        bool passed,
        bool caseValid,
        bool endpointContractSatisfied,
        string outcome,
        float playerTransformDistance,
        float playerFootDistance,
        float playerCenterDistance,
        float endpointTolerance,
        float minEdgeClearance)
    {
        CompleteScenario(
            "RealInputPlayerEndpointNpcOccupied",
            passed,
            $"playerClickIssued={playerClickIssued}, caseValid={caseValid}, endpointContractSatisfied={endpointContractSatisfied}, outcome={outcome}, " +
            $"playerReached={playerReached}, minEdgeClearance={minEdgeClearance:F3}, " +
            $"playerTransformDistance={playerTransformDistance:F3}, playerFootDistance={playerFootDistance:F3}, " +
            $"playerCenterDistance={playerCenterDistance:F3}, endpointTolerance={endpointTolerance:F3}, " +
            $"directionFlips={playerDirectionFlipCount}, blockedInputFrames={playerBlockedInputFrames}, " +
            $"detourMoveFrames={playerDetourMoveFrames}, actionChanges={playerActionChangeCount}, crowdStallDuration={crowdStallDuration:F3}, " +
            $"npcPushDisplacement={npcPushDisplacement:F3}, pushContactFrames={npcPushContactFrames}, timeout={scenarioTimer:F2}, " +
            $"{BuildClickDispatchSummary()}, {BuildPlayerActionSummary()}",
            $"pass={passed} caseValid={caseValid} outcome={outcome} playerCenterDistance={playerCenterDistance:F3} " +
            $"playerFootDistance={playerFootDistance:F3} endpointTolerance={endpointTolerance:F3} " +
            $"pendingAutoInteractionAfterClick={clickPendingAutoInteractionAfterIssue} npcPushDisplacement={npcPushDisplacement:F3} " +
            $"directionFlips={playerDirectionFlipCount} blockedInputFrames={playerBlockedInputFrames} " +
            $"detourMoveFrames={playerDetourMoveFrames} actionChanges={playerActionChangeCount} " +
            $"endpointContractSatisfied={endpointContractSatisfied}");
    }

    private void TickRealInputPlayerEndpointNpcOccupied()
    {
        const int endpointArrivalMaxDirectionFlips = 2;
        Rigidbody2D playerRb = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Rigidbody2D>() : null;
        Collider2D playerCollider = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Collider2D>() : null;
        Vector2 playerPos = GetActorFootPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerRb);
        Vector2 npcPos = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        Vector2 playerTransformPos = GetActorTransformPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null);
        Vector2 playerCenterPos = GetActorMeasurePosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerCollider);
        float playerTransformDistance = Vector2.Distance(playerTransformPos, playerTarget);
        float playerRigidbodyDistance = Vector2.Distance(playerPos, playerTarget);
        float playerCenterDistance = Vector2.Distance(playerCenterPos, playerTarget);
        float endpointArrivalTolerance = combinedRadius + EndpointNpcArrivalTolerancePadding;
        float edgeClearance = GetActorEdgeClearance(
            activePlayerNavigator != null ? activePlayerNavigator.transform : null,
            playerRb,
            playerCollider,
            activeNpcA.Transform,
            activeNpcA.Rigidbody,
            activeNpcA.Collider);

        minClearance = Mathf.Min(minClearance, edgeClearance);
        maxPlayerLateralOffset = Mathf.Max(maxPlayerLateralOffset, Mathf.Abs(playerPos.y - laneReferenceY));
        UpdateNpcPushMetrics(playerPos, npcPos, edgeClearance);
        ObservePlayerNavigationAction();
        UpdateCrowdOscillationMetrics(playerPos, playerTarget);
        UpdateBlockOnsetMetric(playerPos, edgeClearance, playerTarget);

        bool navigatorInactive = activePlayerNavigator != null && !activePlayerNavigator.IsActive;
        bool endpointContractSatisfied = navigatorInactive &&
            playerCenterDistance <= PlayerPointArrivalCenterTolerance;
        bool stableHoldOutsideEndpoint = navigatorInactive &&
            !endpointContractSatisfied &&
            playerRigidbodyDistance <= endpointArrivalTolerance;
        bool interactionHijack = clickPendingAutoInteractionAfterIssue;
        bool caseValid = playerClickIssued && !interactionHijack;
        bool bulldoze = minClearance <= -0.06f || npcPushDisplacement > MaxAllowedNpcPushDisplacement;
        bool oscillation = playerDirectionFlipCount > endpointArrivalMaxDirectionFlips ||
            playerActionChangeCount > EndpointNpcArrivalMaxActionChanges;
        string outcome = ClassifyEndpointNpcOccupiedOutcome(
            interactionHijack,
            endpointContractSatisfied,
            bulldoze,
            oscillation,
            stableHoldOutsideEndpoint);

        playerReached = endpointContractSatisfied;

        if (endpointContractSatisfied || stableHoldOutsideEndpoint)
        {
            bool passed = caseValid && outcome == "ReachedClickPoint";
            CompleteEndpointNpcOccupiedScenario(
                passed,
                caseValid,
                endpointContractSatisfied,
                outcome,
                playerTransformDistance,
                playerRigidbodyDistance,
                playerCenterDistance,
                endpointArrivalTolerance,
                minClearance);
            return;
        }

        if (scenarioTimer >= ScenarioTimeout)
        {
            CompleteEndpointNpcOccupiedScenario(
                false,
                caseValid,
                endpointContractSatisfied,
                outcome,
                playerTransformDistance,
                playerRigidbodyDistance,
                playerCenterDistance,
                endpointArrivalTolerance,
                minClearance);
        }
    }

    private void CompleteScenario(string scenarioName, bool passed, string details, string summary)
    {
        string effectiveScenarioName = string.IsNullOrEmpty(currentScenarioResultName)
            ? scenarioName
            : currentScenarioResultName;

        scenarioResults.Add(new ScenarioResult
        {
            Name = effectiveScenarioName,
            Passed = passed,
            Details = details
        });

        if (effectiveScenarioName == scenarioName)
        {
            Debug.Log($"{LogPrefix} scenario_end={effectiveScenarioName} pass={passed} {summary}");
        }
        else
        {
            Debug.Log(
                $"{LogPrefix} scenario_end={effectiveScenarioName} pass={passed} " +
                $"baseScenario={scenarioName} {summary}");
        }

        currentScenarioResultName = string.Empty;

        if (runSingleScenarioOnly)
        {
            FinishRun();
            return;
        }

        if (HasActivePack())
        {
            if (QueueNextPackScenario("final_acceptance_pack_next"))
            {
                return;
            }

            FinishRun();
            return;
        }

        switch (currentScenario)
        {
            case ScenarioKind.RealInputPlayerAvoidsMovingNpc:
                StartScenario(ScenarioKind.RealInputPlayerSingleNpcNear);
                break;

            case ScenarioKind.RealInputPlayerSingleNpcNear:
                StartScenario(ScenarioKind.RealInputPlayerEndpointNpcOccupied);
                break;

            case ScenarioKind.RealInputPlayerEndpointNpcOccupied:
                StartScenario(ScenarioKind.RealInputPlayerPassableCorridor);
                break;

            case ScenarioKind.RealInputPlayerCrowdPass:
            case ScenarioKind.RealInputPlayerPassableCorridor:
                StartScenario(ScenarioKind.NpcAvoidsPlayer);
                break;

            case ScenarioKind.NpcAvoidsPlayer:
                StartScenario(ScenarioKind.NpcNpcCrossing);
                break;

            case ScenarioKind.NpcNpcCrossing:
                FinishRun();
                break;
        }
    }

    private void FinishRun()
    {
        RestoreManagedRoamController(activeNpcA.Controller);
        RestoreManagedRoamController(activeNpcB.Controller);
        RestoreManagedRoamController(activeNpcC.Controller);

        bool allPassed = true;
        for (int index = 0; index < scenarioResults.Count; index++)
        {
            ScenarioResult result = scenarioResults[index];
            allPassed &= result.Passed;
            Debug.Log($"{LogPrefix} scenario={result.Name} pass={result.Passed} details={result.Details}");
        }

        if (!string.IsNullOrEmpty(activePackName))
        {
            Debug.Log(
                $"{LogPrefix} final_acceptance_pack_completed name={activePackName} " +
                $"allPassed={allPassed} caseCount={scenarioResults.Count}");
        }

        Debug.Log($"{LogPrefix} all_completed={allPassed} scenario_count={scenarioResults.Count}");
        isRunning = false;
        runSingleScenarioOnly = false;
        currentScenario = ScenarioKind.None;
        queuedScenario = ScenarioKind.None;
        queuedScenarioStartFrames = 0;
        ClearActivePackState();
    }

    private void AbortRun(string reason)
    {
        RestoreManagedRoamController(activeNpcA.Controller);
        RestoreManagedRoamController(activeNpcB.Controller);
        RestoreManagedRoamController(activeNpcC.Controller);

        Debug.LogError($"{LogPrefix} abort reason={reason}");
        isRunning = false;
        runSingleScenarioOnly = false;
        currentScenario = ScenarioKind.None;
        queuedScenario = ScenarioKind.None;
        queuedScenarioStartFrames = 0;
        ClearActivePackState();
    }

    private void ResetObservationState()
    {
        settleFramesRemaining = 0;
        scenarioTimer = 0f;
        laneReferenceY = 0f;
        combinedRadius = 0f;
        minClearance = float.PositiveInfinity;
        blockOnsetEdgeDistance = float.PositiveInfinity;
        maxPlayerLateralOffset = 0f;
        maxNpcLateralOffset = 0f;
        playerDirectionFlipCount = 0;
        crowdStallDuration = 0f;
        playerReached = false;
        npcReached = false;
        npcAReached = false;
        npcBReached = false;
        npcMoveIssued = false;
        npcAMoveIssued = false;
        npcBMoveIssued = false;
        playerTarget = Vector2.zero;
        npcTarget = Vector2.zero;
        npcATarget = Vector2.zero;
        npcBTarget = Vector2.zero;
        activePlayerNavigator = null;
        activeGameInputManager = null;
        activeNpcA = default;
        activeNpcB = default;
        activeNpcC = default;
        queuedScenario = ScenarioKind.None;
        queuedScenarioStartFrames = 0;
        playerClickIssued = false;
        pushObservationInitialized = false;
        blockOnsetRecorded = false;
        clickPendingAutoInteractionAfterIssue = false;
        clickPendingAutoInteractionTargetName = "none";
        previousPlayerPos = Vector2.zero;
        previousNpcPos = Vector2.zero;
        previousPlayerMotion = Vector2.zero;
        npcPushDisplacement = 0f;
        npcPushContactFrames = 0;
        playerPathMoveFrames = 0;
        playerDetourMoveFrames = 0;
        playerBlockedInputFrames = 0;
        playerHardStopFrames = 0;
        playerActionChangeCount = 0;
        lastObservedPlayerAction = string.Empty;
        groundPointCaseIndex = -1;
        groundPointReachedCases = 0;
        groundPointAccurateCenterCases = 0;
        groundPointPositiveCenterBiasCases = 0;
        groundPointCaseStartTimer = 0f;
        groundPointMaxColliderDistance = 0f;
        groundPointCaseDetails.Clear();
        roamSeedAccepted = false;
        roamDetourCreated = false;
        roamReleaseSucceeded = false;
        roamBlockerReleased = false;
        roamSeedCursor = ManagedRoamSeedStart;
        roamSeedAttempts = 0;
        roamAcceptedSeed = 0;
        roamAcceptedAtTime = 0f;
        roamNpcStartPosition = Vector2.zero;
        roamAcceptedDestination = Vector2.zero;
    }

    private bool EnsureScenarioRuntimeBindings()
    {
        bool needsPlayer = activePlayerNavigator == null;
        bool needsNpcA = activeNpcA.Controller == null;
        bool needsNpcB = currentScenario == ScenarioKind.NpcNpcCrossing ||
            currentScenario == ScenarioKind.RealInputPlayerCrowdPass ||
            currentScenario == ScenarioKind.RealInputPlayerPassableCorridor ||
            currentScenario == ScenarioKind.RealInputPlayerStaticNpcWall ||
            currentScenario == ScenarioKind.RealInputPlayerEndpointNpcOccupied ||
            currentScenario == ScenarioKind.NpcRoamRecoverWindow ||
            currentScenario == ScenarioKind.NpcRoamPersistentBlockInterrupt
            ? activeNpcB.Controller == null
            : false;
        bool needsNpcC = currentScenario == ScenarioKind.RealInputPlayerCrowdPass ||
            currentScenario == ScenarioKind.RealInputPlayerPassableCorridor ||
            currentScenario == ScenarioKind.RealInputPlayerStaticNpcWall ||
            currentScenario == ScenarioKind.NpcRoamPersistentBlockInterrupt
            ? activeNpcC.Controller == null
            : false;

        if (needsPlayer || needsNpcA || needsNpcB || needsNpcC)
        {
            if (!TryResolveActors(out activePlayerNavigator, out activeNpcA, out activeNpcB, out activeNpcC))
            {
                AbortRun($"scenario={currentScenario} actors_lost_during_runtime");
                return false;
            }

            Debug.Log($"{LogPrefix} runtime_bindings_refreshed scenario={currentScenario}");
        }

        bool needsGameInput =
            (currentScenario == ScenarioKind.RealInputPlayerAvoidsMovingNpc ||
             currentScenario == ScenarioKind.RealInputPlayerSingleNpcNear ||
             currentScenario == ScenarioKind.RealInputPlayerCrowdPass ||
             currentScenario == ScenarioKind.RealInputPlayerPassableCorridor ||
             currentScenario == ScenarioKind.RealInputPlayerStaticNpcWall ||
             currentScenario == ScenarioKind.RealInputPlayerEndpointNpcOccupied ||
             currentScenario == ScenarioKind.RealInputPlayerGroundPointMatrix) &&
            activeGameInputManager == null;

        if (needsGameInput)
        {
            activeGameInputManager = FindFirstObjectByType<GameInputManager>();
            if (activeGameInputManager == null)
            {
                AbortRun($"scenario={currentScenario} game_input_lost_during_runtime");
                return false;
            }

            Debug.Log($"{LogPrefix} runtime_game_input_rebound scenario={currentScenario}");
        }

        return true;
    }

    private void MaybeLogHeartbeat()
    {
        if (Time.frameCount < nextHeartbeatFrame)
        {
            return;
        }

        nextHeartbeatFrame = Time.frameCount + HeartbeatFrameInterval;
        Vector2 playerPos = GetActorFootPosition(
            activePlayerNavigator != null ? activePlayerNavigator.transform : null,
            activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Rigidbody2D>() : null);
        Vector2 npcAPos = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        Vector2 npcBPos = GetActorFootPosition(activeNpcB.Transform, activeNpcB.Rigidbody);
        string playerDebug = string.Empty;
        if ((currentScenario == ScenarioKind.RealInputPlayerSingleNpcNear ||
             currentScenario == ScenarioKind.RealInputPlayerEndpointNpcOccupied ||
             currentScenario == ScenarioKind.RealInputPlayerCrowdPass ||
             currentScenario == ScenarioKind.RealInputPlayerPassableCorridor ||
             currentScenario == ScenarioKind.RealInputPlayerStaticNpcWall ||
             currentScenario == ScenarioKind.RealInputPlayerGroundPointMatrix) &&
            activePlayerNavigator != null)
        {
            PlayerAutoNavigatorDebugSnapshot navigatorDebug = PlayerAutoNavigatorDebugCompat.Capture(activePlayerNavigator);
            playerDebug =
                $" clickMode={currentClickProbeMode} pendingAutoInteraction={clickPendingAutoInteractionAfterIssue} " +
                $"pendingTarget={clickPendingAutoInteractionTargetName}" +
                $" pathCount={navigatorDebug.PathPointCount} pathIndex={navigatorDebug.PathIndex} " +
                $"waypoint={navigatorDebug.CurrentPathWaypoint} detour={navigatorDebug.HasDynamicDetour} " +
                $"detourOwner={navigatorDebug.DynamicDetourOwnerId} blocker={navigatorDebug.LastDynamicObstacleId} " +
                $"blockerSightings={navigatorDebug.DynamicObstacleSightings} action={navigatorDebug.NavigationAction} " +
                $"moveScale={navigatorDebug.MoveScale:F2} avoidRepath={navigatorDebug.AvoidanceShouldRepath} " +
                $"blockDist={navigatorDebug.BlockingDistance:F3} " +
                $"npcBlocker={navigatorDebug.HasNpcBlocker} " +
                $"passiveNpcBlocker={navigatorDebug.HasPassiveNpcBlocker} " +
                $"closeApplied={navigatorDebug.CloseConstraintApplied} " +
                $"closeHard={navigatorDebug.CloseConstraintHardBlocked} " +
                $"closeClearance={navigatorDebug.CloseConstraintClearance:F3} " +
                $"closeForward={navigatorDebug.CloseConstraintForwardInto:F3} " +
                $"{BuildPlayerActionSummary()}";
        }

        Debug.Log(
            $"{LogPrefix} heartbeat scenario={currentScenario} timer={scenarioTimer:F2} time={Time.time:F2} " +
            $"unscaledTime={Time.unscaledTime:F2} delta={Time.deltaTime:F3} unscaledDelta={Time.unscaledDeltaTime:F3} " +
            $"timeScale={Time.timeScale:F2} playerPos={playerPos} npcAPos={npcAPos} npcBPos={npcBPos} " +
            $"playerActive={(activePlayerNavigator != null && activePlayerNavigator.IsActive)} " +
            $"npcAState={(activeNpcA.Controller != null ? activeNpcA.Controller.DebugState : "null")} " +
            $"npcBState={(activeNpcB.Controller != null ? activeNpcB.Controller.DebugState : "null")}" +
            playerDebug);
    }

    private bool TryResolveActors(
        out PlayerAutoNavigator playerNavigator,
        out NPCMovementBundle npcA,
        out NPCMovementBundle npcB,
        out NPCMovementBundle npcC)
    {
        playerNavigator = FindFirstObjectByType<PlayerAutoNavigator>();
        npcA = default;
        npcB = default;
        npcC = default;

        if (playerNavigator == null)
        {
            Debug.LogError($"{LogPrefix} player navigator not found");
            return false;
        }

        NPCAutoRoamController[] controllers = FindObjectsByType<NPCAutoRoamController>(FindObjectsSortMode.None);
        Dictionary<string, NPCMovementBundle> map = new Dictionary<string, NPCMovementBundle>();
        for (int index = 0; index < controllers.Length; index++)
        {
            NPCAutoRoamController controller = controllers[index];
            map[controller.name] = new NPCMovementBundle(controller);
        }

        if (!map.TryGetValue("001", out npcA) || !map.TryGetValue("002", out npcB) || !map.TryGetValue("003", out npcC))
        {
            Debug.LogError($"{LogPrefix} npc actors 001/002/003 not found");
            return false;
        }

        return true;
    }

    private void PrepareScene(
        PlayerAutoNavigator playerNavigator,
        NPCMovementBundle npcA,
        NPCMovementBundle npcB,
        NPCMovementBundle npcC)
    {
        playerNavigator.ForceCancel();

        Rigidbody2D playerRb = playerNavigator.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        NPCMovementBundle[] npcs = { npcA, npcB, npcC };
        for (int index = 0; index < npcs.Length; index++)
        {
            RestoreManagedRoamController(npcs[index].Controller);
            npcs[index].Controller.StopRoam();
            if (npcs[index].Rigidbody != null)
            {
                npcs[index].Rigidbody.linearVelocity = Vector2.zero;
            }
        }
    }

    private void RestoreManagedRoamController(NPCAutoRoamController controller)
    {
        if (controller == null)
        {
            return;
        }

        int controllerId = controller.GetInstanceID();
        if (!managedRoamControllerDefaults.TryGetValue(controllerId, out NpcRoamControllerManagedTuningSnapshot snapshot))
        {
            managedRoamControllerDefaults[controllerId] = NpcRoamControllerManagedTuningSnapshot.Capture(controller);
            return;
        }

        snapshot.Apply(controller);
    }

    private static void PlaceNpc(NPCMovementBundle npc, Vector2 position)
    {
        PlaceActor(npc.Transform, npc.Rigidbody, position);
    }

    private static void PlaceActor(Transform actor, Rigidbody2D rb, Vector2 position)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = position;
        }

        actor.position = new Vector3(position.x, position.y, actor.position.z);
        NPCMotionController motionController = actor.GetComponent<NPCMotionController>();
        motionController?.ResetMotionObservation();
        Physics2D.SyncTransforms();
    }

    private static float GetActorRadius(Collider2D collider)
    {
        if (collider == null)
        {
            return 0.25f;
        }

        return Mathf.Max(collider.bounds.extents.x, collider.bounds.extents.y);
    }

    private static Vector2 GetActorMeasurePosition(Transform transform, Collider2D collider)
    {
        if (collider != null)
        {
            return collider.bounds.center;
        }

        return transform != null ? (Vector2)transform.position : Vector2.zero;
    }

    private static Vector2 GetActorFootPosition(Transform transform, Rigidbody2D rigidbody)
    {
        if (rigidbody != null)
        {
            return rigidbody.position;
        }

        return transform != null ? (Vector2)transform.position : Vector2.zero;
    }

    private static float GetActorFootprintRadius(Collider2D collider)
    {
        if (collider == null)
        {
            return 0.25f;
        }

        return Mathf.Max(0.01f, collider.bounds.extents.x);
    }

    private static float GetActorEdgeClearance(
        Transform transformA,
        Rigidbody2D rigidbodyA,
        Collider2D colliderA,
        Transform transformB,
        Rigidbody2D rigidbodyB,
        Collider2D colliderB)
    {
        if (colliderA != null &&
            colliderB != null &&
            colliderA.enabled &&
            colliderB.enabled &&
            colliderA.gameObject.activeInHierarchy &&
            colliderB.gameObject.activeInHierarchy)
        {
            ColliderDistance2D distance = colliderA.Distance(colliderB);
            if (distance.isValid)
            {
                return distance.distance;
            }
        }

        return Vector2.Distance(
                   GetActorFootPosition(transformA, rigidbodyA),
                   GetActorFootPosition(transformB, rigidbodyB)) -
               (GetActorFootprintRadius(colliderA) + GetActorFootprintRadius(colliderB));
    }

    private void UpdateBlockOnsetMetric(Vector2 playerPos, float edgeClearance, Vector2 target)
    {
        if (blockOnsetRecorded)
        {
            previousPlayerPos = playerPos;
            return;
        }

        Vector2 playerDelta = playerPos - previousPlayerPos;
        previousPlayerPos = playerPos;
        PlayerAutoNavigatorDebugSnapshot playerDebug = PlayerAutoNavigatorDebugCompat.Capture(activePlayerNavigator);
        string navigationAction = playerDebug.NavigationAction;
        float navigationMoveScale = playerDebug.MoveScale;
        bool closeApplied = playerDebug.CloseConstraintApplied;
        bool hardBlocked = playerDebug.CloseConstraintHardBlocked;
        bool hasMeaningfulMotionSample = playerDelta.sqrMagnitude > 0.0004f;
        bool hasRealSlowdownSignal =
            navigationAction == "HardStop" ||
            navigationAction == "BlockedInput" ||
            hardBlocked ||
            closeApplied ||
            navigationMoveScale < 0.95f;

        float distanceToTarget = Vector2.Distance(playerPos, target);
        if (distanceToTarget <= 0.9f || edgeClearance <= 0.02f)
        {
            return;
        }

        Vector2 toTarget = target - playerPos;
        if (toTarget.sqrMagnitude < 0.0001f)
        {
            return;
        }

        float forwardProgress = Vector2.Dot(playerDelta, toTarget.normalized);
        if (!hasMeaningfulMotionSample && !hasRealSlowdownSignal)
        {
            return;
        }

        if (forwardProgress <= 0.01f)
        {
            blockOnsetRecorded = true;
            blockOnsetEdgeDistance = edgeClearance;
            Debug.Log(
                $"{LogPrefix} block_onset edgeClearance={edgeClearance:F3} playerPos={playerPos} " +
                $"playerDelta={playerDelta} target={target} " +
                $"playerActive={(activePlayerNavigator != null && activePlayerNavigator.IsActive)} " +
                $"action={(activePlayerNavigator != null ? navigationAction : "null")} " +
                $"moveScale={(activePlayerNavigator != null ? navigationMoveScale.ToString("F2") : "n/a")} " +
                $"blockDist={(activePlayerNavigator != null ? playerDebug.BlockingDistance.ToString("F3") : "n/a")} " +
                $"npcBlocker={(activePlayerNavigator != null && playerDebug.HasNpcBlocker)} " +
                $"passiveNpcBlocker={(activePlayerNavigator != null && playerDebug.HasPassiveNpcBlocker)} " +
                $"closeApplied={closeApplied} " +
                $"closeHard={hardBlocked} " +
                $"closeClearance={(activePlayerNavigator != null ? playerDebug.CloseConstraintClearance.ToString("F3") : "n/a")} " +
                $"closeForward={(activePlayerNavigator != null ? playerDebug.CloseConstraintForwardInto.ToString("F3") : "n/a")}");
        }
    }

    private void UpdateCrowdOscillationMetrics(Vector2 playerPos, Vector2 target)
    {
        Vector2 playerDelta = playerPos - previousPlayerPos;
        previousPlayerPos = playerPos;

        if (playerDelta.sqrMagnitude > 0.0001f)
        {
            if (previousPlayerMotion.sqrMagnitude > 0.0001f &&
                Vector2.Dot(previousPlayerMotion.normalized, playerDelta.normalized) < -0.25f)
            {
                playerDirectionFlipCount++;
            }

            previousPlayerMotion = playerDelta;
        }

        float distanceToTarget = Vector2.Distance(playerPos, target);
        if (distanceToTarget > 0.9f && playerDelta.magnitude <= 0.012f)
        {
            crowdStallDuration += Time.unscaledDeltaTime;
        }
    }

    private void UpdateNpcPushMetrics(Vector2 playerPos, Vector2 npcPos, float clearance)
    {
        if (!pushObservationInitialized)
        {
            previousPlayerPos = playerPos;
            previousNpcPos = npcPos;
            pushObservationInitialized = true;
            return;
        }

        Vector2 playerDelta = playerPos - previousPlayerPos;
        Vector2 npcDelta = npcPos - previousNpcPos;
        float contactThreshold = PushContactBuffer;
        bool isNearContact = clearance <= contactThreshold;

        if (isNearContact && playerDelta.sqrMagnitude > 0.0001f)
        {
            Vector2 playerForward = playerDelta.normalized;
            Vector2 toNpc = npcPos - playerPos;
            if (toNpc.sqrMagnitude > 0.0001f &&
                Vector2.Dot(playerForward, toNpc.normalized) >= PushForwardDotThreshold)
            {
                float projectedNpcDisplacement = Mathf.Max(0f, Vector2.Dot(npcDelta, playerForward));
                if (projectedNpcDisplacement > 0f)
                {
                    npcPushDisplacement += projectedNpcDisplacement;
                    npcPushContactFrames++;
                }
            }
        }

        previousPlayerPos = playerPos;
        previousNpcPos = npcPos;
    }

    private static Vector2 GetActorTransformPosition(Transform transform)
    {
        return transform != null ? (Vector2)transform.position : Vector2.zero;
    }

    private void StartNextGroundPointMatrixCase()
    {
        groundPointCaseIndex++;
        if (groundPointCaseIndex >= GroundPointMatrixCases.Length)
        {
            CompleteGroundPointMatrixScenario();
            return;
        }

        GroundPointMatrixCase testCase = GroundPointMatrixCases[groundPointCaseIndex];
        PlaceActor(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>(), testCase.Start);
        playerTarget = testCase.Target;
        playerClickIssued = TryIssueAutoNavClick(
            activeGameInputManager,
            playerTarget,
            currentClickProbeMode,
            activeNpcA,
            activeNpcB,
            activeNpcC);
        CapturePendingAutoInteractionState(activeGameInputManager);
        previousPlayerPos = GetActorFootPosition(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>());
        groundPointCaseStartTimer = scenarioTimer;
        settleFramesRemaining = SetupSettleFrames;

        Debug.Log(
            $"{LogPrefix} ground_point_case_start clickMode={currentClickProbeMode} case={testCase.Id} " +
            $"band={testCase.DistanceBand} direction={testCase.DirectionLabel} start={testCase.Start} click={testCase.Target} " +
            $"playerClickIssued={playerClickIssued} pendingAutoInteractionAfterClick={clickPendingAutoInteractionAfterIssue}");
    }

    private void CompleteGroundPointMatrixCase(bool timedOut, Vector2 transformPos, Vector2 rigidbodyPos, Vector2 colliderCenter)
    {
        GroundPointMatrixCase testCase = GroundPointMatrixCases[groundPointCaseIndex];
        PlayerAutoNavigatorDebugSnapshot playerDebug = PlayerAutoNavigatorDebugCompat.Capture(activePlayerNavigator);
        Vector2 transformDelta = transformPos - testCase.Target;
        Vector2 rigidbodyDelta = rigidbodyPos - testCase.Target;
        Vector2 colliderDelta = colliderCenter - testCase.Target;
        float transformDistance = transformDelta.magnitude;
        float rigidbodyDistance = rigidbodyDelta.magnitude;
        float colliderDistance = colliderDelta.magnitude;
        float caseElapsed = scenarioTimer - groundPointCaseStartTimer;
        bool navigatorReached = !timedOut && activePlayerNavigator != null && !activePlayerNavigator.IsActive;
        bool colliderAccurate = colliderDistance <= 0.35f;

        if (navigatorReached)
        {
            groundPointReachedCases++;
        }

        if (colliderAccurate)
        {
            groundPointAccurateCenterCases++;
        }

        if (colliderDelta.y > 0.2f)
        {
            groundPointPositiveCenterBiasCases++;
        }

        groundPointMaxColliderDistance = Mathf.Max(groundPointMaxColliderDistance, colliderDistance);

        string detail =
            $"case={testCase.Id}, band={testCase.DistanceBand}, direction={testCase.DirectionLabel}, timedOut={timedOut}, " +
            $"click={testCase.Target}, transform={transformPos}, rigidbody={rigidbodyPos}, colliderCenter={colliderCenter}, " +
            $"transformDelta={transformDelta}, transformDistance={transformDistance:F3}, " +
            $"rigidbodyDelta={rigidbodyDelta}, rigidbodyDistance={rigidbodyDistance:F3}, " +
            $"colliderDelta={colliderDelta}, colliderDistance={colliderDistance:F3}, stopTime={caseElapsed:F3}, " +
            $"pathRequest={(activePlayerNavigator != null ? playerDebug.PathRequestDestination.ToString() : "n/a")}, " +
            $"resolvedDestination={(activePlayerNavigator != null ? playerDebug.ResolvedPathDestination.ToString() : "n/a")}, " +
            $"navigationOffset={(activePlayerNavigator != null ? playerDebug.NavigationPositionOffset.ToString() : "n/a")}";
        groundPointCaseDetails.Add(detail);

        Debug.Log(
            $"{LogPrefix} ground_point_case_end clickMode={currentClickProbeMode} {detail} " +
            $"navigatorReached={navigatorReached} colliderAccurate={colliderAccurate} " +
            $"pendingAutoInteractionAfterClick={clickPendingAutoInteractionAfterIssue}");

        StartNextGroundPointMatrixCase();
    }

    private void CompleteGroundPointMatrixScenario()
    {
        bool passed =
            groundPointReachedCases == GroundPointMatrixCases.Length &&
            groundPointAccurateCenterCases == GroundPointMatrixCases.Length;
        string details =
            $"clickMode={currentClickProbeMode}, cases={GroundPointMatrixCases.Length}, reachedCases={groundPointReachedCases}, " +
            $"accurateCenterCases={groundPointAccurateCenterCases}, positiveCenterBiasCases={groundPointPositiveCenterBiasCases}, " +
            $"maxColliderDistance={groundPointMaxColliderDistance:F3}, caseDetails=[{string.Join(" || ", groundPointCaseDetails)}]";
        string summary =
            $"clickMode={currentClickProbeMode} reachedCases={groundPointReachedCases}/{GroundPointMatrixCases.Length} " +
            $"accurateCenterCases={groundPointAccurateCenterCases}/{GroundPointMatrixCases.Length} " +
            $"positiveCenterBiasCases={groundPointPositiveCenterBiasCases}/{GroundPointMatrixCases.Length} " +
            $"maxColliderDistance={groundPointMaxColliderDistance:F3}";
        CompleteScenario("RealInputPlayerGroundPointMatrix", passed, details, summary);
    }

    private readonly struct NPCMovementBundle
    {
        public readonly NPCAutoRoamController Controller;
        public readonly Transform Transform;
        public readonly Rigidbody2D Rigidbody;
        public readonly Collider2D Collider;

        public NPCMovementBundle(NPCAutoRoamController controller)
        {
            Controller = controller;
            Transform = controller != null ? controller.transform : null;
            Rigidbody = controller != null ? controller.GetComponent<Rigidbody2D>() : null;
            Collider = controller != null ? controller.GetComponent<Collider2D>() : null;
        }
    }

    private readonly struct LayerSnapshot
    {
        public readonly GameObject GameObject;
        public readonly int Layer;

        public LayerSnapshot(GameObject gameObject, int layer)
        {
            GameObject = gameObject;
            Layer = layer;
        }
    }
}

internal readonly struct PlayerAutoNavigatorDebugSnapshot
{
    public static readonly PlayerAutoNavigatorDebugSnapshot Default = new PlayerAutoNavigatorDebugSnapshot(
        pathPointCount: 0,
        pathIndex: 0,
        currentPathWaypoint: Vector2.zero,
        hasDynamicDetour: false,
        dynamicDetourOwnerId: 0,
        lastDynamicObstacleId: 0,
        dynamicObstacleSightings: 0,
        navigationAction: string.Empty,
        moveScale: 1f,
        avoidanceShouldRepath: false,
        closeConstraintApplied: false,
        closeConstraintHardBlocked: false,
        closeConstraintClearance: 0f,
        closeConstraintForwardInto: 0f,
        blockingDistance: 0f,
        hasNpcBlocker: false,
        hasPassiveNpcBlocker: false,
        transformPosition: Vector2.zero,
        rigidbodyPosition: Vector2.zero,
        colliderCenter: Vector2.zero,
        navigationPositionOffset: Vector2.zero,
        pathRequestDestination: Vector2.zero,
        resolvedPathDestination: Vector2.zero);

    public readonly int PathPointCount;
    public readonly int PathIndex;
    public readonly Vector2 CurrentPathWaypoint;
    public readonly bool HasDynamicDetour;
    public readonly int DynamicDetourOwnerId;
    public readonly int LastDynamicObstacleId;
    public readonly int DynamicObstacleSightings;
    public readonly string NavigationAction;
    public readonly float MoveScale;
    public readonly bool AvoidanceShouldRepath;
    public readonly bool CloseConstraintApplied;
    public readonly bool CloseConstraintHardBlocked;
    public readonly float CloseConstraintClearance;
    public readonly float CloseConstraintForwardInto;
    public readonly float BlockingDistance;
    public readonly bool HasNpcBlocker;
    public readonly bool HasPassiveNpcBlocker;
    public readonly Vector2 TransformPosition;
    public readonly Vector2 RigidbodyPosition;
    public readonly Vector2 ColliderCenter;
    public readonly Vector2 NavigationPositionOffset;
    public readonly Vector2 PathRequestDestination;
    public readonly Vector2 ResolvedPathDestination;

    public PlayerAutoNavigatorDebugSnapshot(
        int pathPointCount,
        int pathIndex,
        Vector2 currentPathWaypoint,
        bool hasDynamicDetour,
        int dynamicDetourOwnerId,
        int lastDynamicObstacleId,
        int dynamicObstacleSightings,
        string navigationAction,
        float moveScale,
        bool avoidanceShouldRepath,
        bool closeConstraintApplied,
        bool closeConstraintHardBlocked,
        float closeConstraintClearance,
        float closeConstraintForwardInto,
        float blockingDistance,
        bool hasNpcBlocker,
        bool hasPassiveNpcBlocker,
        Vector2 transformPosition,
        Vector2 rigidbodyPosition,
        Vector2 colliderCenter,
        Vector2 navigationPositionOffset,
        Vector2 pathRequestDestination,
        Vector2 resolvedPathDestination)
    {
        PathPointCount = pathPointCount;
        PathIndex = pathIndex;
        CurrentPathWaypoint = currentPathWaypoint;
        HasDynamicDetour = hasDynamicDetour;
        DynamicDetourOwnerId = dynamicDetourOwnerId;
        LastDynamicObstacleId = lastDynamicObstacleId;
        DynamicObstacleSightings = dynamicObstacleSightings;
        NavigationAction = navigationAction;
        MoveScale = moveScale;
        AvoidanceShouldRepath = avoidanceShouldRepath;
        CloseConstraintApplied = closeConstraintApplied;
        CloseConstraintHardBlocked = closeConstraintHardBlocked;
        CloseConstraintClearance = closeConstraintClearance;
        CloseConstraintForwardInto = closeConstraintForwardInto;
        BlockingDistance = blockingDistance;
        HasNpcBlocker = hasNpcBlocker;
        HasPassiveNpcBlocker = hasPassiveNpcBlocker;
        TransformPosition = transformPosition;
        RigidbodyPosition = rigidbodyPosition;
        ColliderCenter = colliderCenter;
        NavigationPositionOffset = navigationPositionOffset;
        PathRequestDestination = pathRequestDestination;
        ResolvedPathDestination = resolvedPathDestination;
    }
}

internal static class PlayerAutoNavigatorDebugCompat
{
    private const BindingFlags InstanceMemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly Dictionary<string, PropertyInfo> PropertyCache = new Dictionary<string, PropertyInfo>();
    private static readonly Dictionary<string, FieldInfo> FieldCache = new Dictionary<string, FieldInfo>();
    private static readonly HashSet<string> MissingMembers = new HashSet<string>();

    public static PlayerAutoNavigatorDebugSnapshot Capture(PlayerAutoNavigator navigator)
    {
        if (navigator == null)
        {
            return PlayerAutoNavigatorDebugSnapshot.Default;
        }

        Rigidbody2D rigidbody = navigator.GetComponent<Rigidbody2D>();
        Collider2D collider = navigator.GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = navigator.GetComponentInChildren<Collider2D>();
        }

        Vector2 transformPosition = navigator.transform != null ? (Vector2)navigator.transform.position : Vector2.zero;
        Vector2 rigidbodyPosition = rigidbody != null ? rigidbody.position : transformPosition;
        Vector2 colliderCenter = collider != null ? (Vector2)collider.bounds.center : rigidbodyPosition;

        return new PlayerAutoNavigatorDebugSnapshot(
            pathPointCount: GetInt(navigator, "DebugPathPointCount", 0),
            pathIndex: GetInt(navigator, "DebugPathIndex", 0),
            currentPathWaypoint: GetVector2(navigator, "DebugCurrentPathWaypoint", Vector2.zero),
            hasDynamicDetour: GetBool(navigator, "DebugHasDynamicDetour", false),
            dynamicDetourOwnerId: GetInt(navigator, "DebugDynamicDetourOwnerId", 0),
            lastDynamicObstacleId: GetInt(navigator, "DebugLastDynamicObstacleId", 0),
            dynamicObstacleSightings: GetInt(navigator, "DebugDynamicObstacleSightings", 0),
            navigationAction: GetString(navigator, "DebugLastNavigationAction", string.Empty),
            moveScale: GetFloat(navigator, "DebugLastMoveScale", 1f),
            avoidanceShouldRepath: GetBool(navigator, "DebugLastAvoidanceShouldRepath", false),
            closeConstraintApplied: GetBool(navigator, "DebugLastCloseConstraintApplied", false),
            closeConstraintHardBlocked: GetBool(navigator, "DebugLastCloseConstraintHardBlocked", false),
            closeConstraintClearance: GetFloat(navigator, "DebugLastCloseConstraintClearance", 0f),
            closeConstraintForwardInto: GetFloat(navigator, "DebugLastCloseConstraintForwardInto", 0f),
            blockingDistance: GetFloat(navigator, "DebugLastBlockingDistance", 0f),
            hasNpcBlocker: GetBool(navigator, "DebugLastHasNpcBlocker", false),
            hasPassiveNpcBlocker: GetBool(navigator, "DebugLastHasPassiveNpcBlocker", false),
            transformPosition: transformPosition,
            rigidbodyPosition: rigidbodyPosition,
            colliderCenter: colliderCenter,
            navigationPositionOffset: GetVector2(navigator, "DebugNavigationPositionOffset", colliderCenter - transformPosition),
            pathRequestDestination: GetVector2(navigator, "DebugPathRequestDestination", Vector2.zero),
            resolvedPathDestination: GetVector2(navigator, "DebugResolvedPathDestination", Vector2.zero));
    }

    private static string GetString(PlayerAutoNavigator navigator, string memberName, string fallback)
    {
        object value;
        return TryGetRawValue(navigator, memberName, out value) && value is string stringValue
            ? stringValue
            : fallback;
    }

    private static bool GetBool(PlayerAutoNavigator navigator, string memberName, bool fallback)
    {
        object value;
        return TryGetRawValue(navigator, memberName, out value) && value is bool boolValue
            ? boolValue
            : fallback;
    }

    private static int GetInt(PlayerAutoNavigator navigator, string memberName, int fallback)
    {
        object value;
        return TryGetRawValue(navigator, memberName, out value) && value is int intValue
            ? intValue
            : fallback;
    }

    private static float GetFloat(PlayerAutoNavigator navigator, string memberName, float fallback)
    {
        object value;
        return TryGetRawValue(navigator, memberName, out value) && value is float floatValue
            ? floatValue
            : fallback;
    }

    private static Vector2 GetVector2(PlayerAutoNavigator navigator, string memberName, Vector2 fallback)
    {
        object value;
        return TryGetRawValue(navigator, memberName, out value) && value is Vector2 vectorValue
            ? vectorValue
            : fallback;
    }

    private static bool TryGetRawValue(PlayerAutoNavigator navigator, string memberName, out object value)
    {
        value = null;
        if (navigator == null || string.IsNullOrEmpty(memberName))
        {
            return false;
        }

        PropertyInfo propertyInfo;
        if (PropertyCache.TryGetValue(memberName, out propertyInfo))
        {
            try
            {
                value = propertyInfo.GetValue(navigator, null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        FieldInfo fieldInfo;
        if (FieldCache.TryGetValue(memberName, out fieldInfo))
        {
            try
            {
                value = fieldInfo.GetValue(navigator);
                return true;
            }
            catch
            {
                return false;
            }
        }

        if (MissingMembers.Contains(memberName))
        {
            return false;
        }

        propertyInfo = typeof(PlayerAutoNavigator).GetProperty(memberName, InstanceMemberFlags);
        if (propertyInfo != null)
        {
            PropertyCache[memberName] = propertyInfo;
            try
            {
                value = propertyInfo.GetValue(navigator, null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        fieldInfo = typeof(PlayerAutoNavigator).GetField(memberName, InstanceMemberFlags);
        if (fieldInfo != null)
        {
            FieldCache[memberName] = fieldInfo;
            try
            {
                value = fieldInfo.GetValue(navigator);
                return true;
            }
            catch
            {
                return false;
            }
        }

        MissingMembers.Add(memberName);
        return false;
    }
}

internal readonly struct NpcRoamControllerManagedTuningSnapshot
{
    public readonly float ShortPauseMin;
    public readonly float ShortPauseMax;
    public readonly float MinimumMoveDistance;
    public readonly float StuckCheckInterval;
    public readonly float StuckDistanceThreshold;
    public readonly int MaxStuckRecoveries;
    public readonly bool EnableAmbientChat;
    public readonly bool ShowDebugLog;

    public NpcRoamControllerManagedTuningSnapshot(
        float shortPauseMin,
        float shortPauseMax,
        float minimumMoveDistance,
        float stuckCheckInterval,
        float stuckDistanceThreshold,
        int maxStuckRecoveries,
        bool enableAmbientChat,
        bool showDebugLog)
    {
        ShortPauseMin = shortPauseMin;
        ShortPauseMax = shortPauseMax;
        MinimumMoveDistance = minimumMoveDistance;
        StuckCheckInterval = stuckCheckInterval;
        StuckDistanceThreshold = stuckDistanceThreshold;
        MaxStuckRecoveries = maxStuckRecoveries;
        EnableAmbientChat = enableAmbientChat;
        ShowDebugLog = showDebugLog;
    }

    public static NpcRoamControllerManagedTuningSnapshot Capture(NPCAutoRoamController controller)
    {
        return new NpcRoamControllerManagedTuningSnapshot(
            NpcRoamControllerDebugCompat.GetFloat(controller, "shortPauseMin", 0.05f),
            NpcRoamControllerDebugCompat.GetFloat(controller, "shortPauseMax", 0.05f),
            NpcRoamControllerDebugCompat.GetFloat(controller, "minimumMoveDistance", 1.4f),
            NpcRoamControllerDebugCompat.GetFloat(controller, "stuckCheckInterval", 0.18f),
            NpcRoamControllerDebugCompat.GetFloat(controller, "stuckDistanceThreshold", 0.3f),
            NpcRoamControllerDebugCompat.GetInt(controller, "maxStuckRecoveries", 1),
            NpcRoamControllerDebugCompat.GetBool(controller, "enableAmbientChat", false),
            NpcRoamControllerDebugCompat.GetBool(controller, "showDebugLog", false));
    }

    public void Apply(NPCAutoRoamController controller)
    {
        NpcRoamControllerDebugCompat.TrySetFloat(controller, "shortPauseMin", ShortPauseMin);
        NpcRoamControllerDebugCompat.TrySetFloat(controller, "shortPauseMax", ShortPauseMax);
        NpcRoamControllerDebugCompat.TrySetFloat(controller, "minimumMoveDistance", MinimumMoveDistance);
        NpcRoamControllerDebugCompat.TrySetFloat(controller, "stuckCheckInterval", StuckCheckInterval);
        NpcRoamControllerDebugCompat.TrySetFloat(controller, "stuckDistanceThreshold", StuckDistanceThreshold);
        NpcRoamControllerDebugCompat.TrySetInt(controller, "maxStuckRecoveries", MaxStuckRecoveries);
        NpcRoamControllerDebugCompat.TrySetBool(controller, "enableAmbientChat", EnableAmbientChat);
        NpcRoamControllerDebugCompat.TrySetBool(controller, "showDebugLog", ShowDebugLog);
    }
}

internal static class NpcRoamControllerDebugCompat
{
    private const BindingFlags InstanceMemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly Dictionary<string, PropertyInfo> PropertyCache = new Dictionary<string, PropertyInfo>();
    private static readonly Dictionary<string, FieldInfo> FieldCache = new Dictionary<string, FieldInfo>();
    private static readonly HashSet<string> MissingMembers = new HashSet<string>();

    public static Vector2 GetVector2(NPCAutoRoamController controller, string memberName, Vector2 fallback)
    {
        object value;
        return TryGetRawValue(controller, memberName, out value) && value is Vector2 vectorValue
            ? vectorValue
            : fallback;
    }

    public static float GetFloat(NPCAutoRoamController controller, string memberName, float fallback)
    {
        object value;
        return TryGetRawValue(controller, memberName, out value) && value is float floatValue
            ? floatValue
            : fallback;
    }

    public static string GetString(NPCAutoRoamController controller, string memberName, string fallback)
    {
        object value;
        return TryGetRawValue(controller, memberName, out value) && value != null
            ? value.ToString()
            : fallback;
    }

    public static bool GetBool(NPCAutoRoamController controller, string memberName, bool fallback)
    {
        object value;
        return TryGetRawValue(controller, memberName, out value) && value is bool boolValue
            ? boolValue
            : fallback;
    }

    public static int GetInt(NPCAutoRoamController controller, string memberName, int fallback)
    {
        object value;
        return TryGetRawValue(controller, memberName, out value) && value is int intValue
            ? intValue
            : fallback;
    }

    public static bool TrySetFloat(NPCAutoRoamController controller, string memberName, float value)
    {
        return TrySetRawValue(controller, memberName, value);
    }

    public static bool TrySetBool(NPCAutoRoamController controller, string memberName, bool value)
    {
        return TrySetRawValue(controller, memberName, value);
    }

    public static bool TrySetInt(NPCAutoRoamController controller, string memberName, int value)
    {
        return TrySetRawValue(controller, memberName, value);
    }

    private static bool TryGetRawValue(NPCAutoRoamController controller, string memberName, out object value)
    {
        value = null;
        if (controller == null || string.IsNullOrEmpty(memberName))
        {
            return false;
        }

        PropertyInfo propertyInfo;
        if (PropertyCache.TryGetValue(memberName, out propertyInfo))
        {
            try
            {
                value = propertyInfo.GetValue(controller, null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        FieldInfo fieldInfo;
        if (FieldCache.TryGetValue(memberName, out fieldInfo))
        {
            try
            {
                value = fieldInfo.GetValue(controller);
                return true;
            }
            catch
            {
                return false;
            }
        }

        if (MissingMembers.Contains(memberName))
        {
            return false;
        }

        propertyInfo = typeof(NPCAutoRoamController).GetProperty(memberName, InstanceMemberFlags);
        if (propertyInfo != null)
        {
            PropertyCache[memberName] = propertyInfo;
            try
            {
                value = propertyInfo.GetValue(controller, null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        fieldInfo = typeof(NPCAutoRoamController).GetField(memberName, InstanceMemberFlags);
        if (fieldInfo != null)
        {
            FieldCache[memberName] = fieldInfo;
            try
            {
                value = fieldInfo.GetValue(controller);
                return true;
            }
            catch
            {
                return false;
            }
        }

        MissingMembers.Add(memberName);
        return false;
    }

    private static bool TrySetRawValue(NPCAutoRoamController controller, string memberName, object value)
    {
        if (controller == null || string.IsNullOrEmpty(memberName))
        {
            return false;
        }

        FieldInfo fieldInfo;
        if (FieldCache.TryGetValue(memberName, out fieldInfo))
        {
            try
            {
                fieldInfo.SetValue(controller, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        PropertyInfo propertyInfo;
        if (PropertyCache.TryGetValue(memberName, out propertyInfo) && propertyInfo.CanWrite)
        {
            try
            {
                propertyInfo.SetValue(controller, value, null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        if (MissingMembers.Contains(memberName))
        {
            return false;
        }

        fieldInfo = typeof(NPCAutoRoamController).GetField(memberName, InstanceMemberFlags);
        if (fieldInfo != null)
        {
            FieldCache[memberName] = fieldInfo;
            try
            {
                fieldInfo.SetValue(controller, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        propertyInfo = typeof(NPCAutoRoamController).GetProperty(memberName, InstanceMemberFlags);
        if (propertyInfo != null && propertyInfo.CanWrite)
        {
            PropertyCache[memberName] = propertyInfo;
            try
            {
                propertyInfo.SetValue(controller, value, null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        MissingMembers.Add(memberName);
        return false;
    }
}
