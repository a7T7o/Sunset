using System.Collections.Generic;
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
    private const bool ForceFullPackAutorunForCurrentFix = false;
#endif
    private const float ScenarioTimeout = 6.5f;
    private const int SetupSettleFrames = 2;
    private const int HeartbeatFrameInterval = 45;
    private const int InitialScenarioBootstrapFrames = 2;
    private const float PushContactBuffer = 0.04f;
    private const float PushForwardDotThreshold = 0.25f;
    private const float MaxAllowedNpcPushDisplacement = 0.08f;
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
        RealInputPlayerCrowdPass = 6
    }

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
    private Vector2 playerTarget;
    private Vector2 npcTarget;
    private Vector2 npcATarget;
    private Vector2 npcBTarget;
    private Vector2 previousPlayerPos;
    private Vector2 previousNpcPos;
    private Vector2 previousPlayerMotion;
    private float npcPushDisplacement;
    private int npcPushContactFrames;
    private PlayerAutoNavigator activePlayerNavigator;
    private GameInputManager activeGameInputManager;
    private NPCMovementBundle activeNpcA;
    private NPCMovementBundle activeNpcB;
    private NPCMovementBundle activeNpcC;
    private ScenarioKind queuedScenario;

    private readonly List<ScenarioResult> scenarioResults = new List<ScenarioResult>();

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

    public static void SetupPlayerAvoidsMovingNpcProbe()
    {
        GetOrCreateRunner().RunSingleSetup(ScenarioKind.PlayerAvoidsMovingNpc);
    }

    public static void RunRealInputPlayerAvoidsMovingNpcProbe()
    {
        GetOrCreateRunner().RunSingleSetup(ScenarioKind.RealInputPlayerAvoidsMovingNpc);
    }

    public static void RunRealInputPlayerSingleNpcNearProbe()
    {
        GetOrCreateRunner().RunSingleSetup(ScenarioKind.RealInputPlayerSingleNpcNear);
    }

    public static void RunRealInputPlayerCrowdPassProbe()
    {
        GetOrCreateRunner().RunSingleSetup(ScenarioKind.RealInputPlayerCrowdPass);
    }

    public static void SetupNpcAvoidsPlayerProbe()
    {
        GetOrCreateRunner().RunSingleSetup(ScenarioKind.NpcAvoidsPlayer);
    }

    public static void SetupNpcNpcCrossingProbe()
    {
        GetOrCreateRunner().RunSingleSetup(ScenarioKind.NpcNpcCrossing);
    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoRunFromEditorLaunchRequest()
    {
        if (ForceFullPackAutorunForCurrentFix && Application.isPlaying)
        {
            Debug.Log($"{LogPrefix} forced_autorun_full_pack");
            BeginOrRestart();
            return;
        }

        if (!Application.isPlaying || !EditorPrefs.HasKey(PendingActionEditorPrefKey))
        {
            return;
        }

        string action = EditorPrefs.GetString(PendingActionEditorPrefKey, string.Empty);
        EditorPrefs.DeleteKey(PendingActionEditorPrefKey);
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

            case "SetupPlayerAvoidsMovingNpc":
                SetupPlayerAvoidsMovingNpcProbe();
                break;

            case "RunRealInputPush":
                RunRealInputPlayerAvoidsMovingNpcProbe();
                break;

            case "RunRealInputSingleNpcNear":
                RunRealInputPlayerSingleNpcNearProbe();
                break;

            case "RunRealInputCrowd":
                RunRealInputPlayerCrowdPassProbe();
                break;

            case "SetupNpcAvoidsPlayer":
                SetupNpcAvoidsPlayerProbe();
                break;

            case "SetupNpcNpcCrossing":
                SetupNpcNpcCrossingProbe();
                break;

            default:
                Debug.LogWarning($"{LogPrefix} unknown_runtime_launch_request={action}");
                break;
        }
    }
#endif

    public void RunAll()
    {
        EnsureRunInBackground();
        Time.timeScale = 1f;
        isRunning = true;
        runSingleScenarioOnly = false;
        scenarioResults.Clear();
        ResetObservationState();
        Debug.Log($"{LogPrefix} runner_started");
        QueueScenarioStart(ScenarioKind.RealInputPlayerAvoidsMovingNpc, InitialScenarioBootstrapFrames, "run_all");
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
        }
    }

    private void RunSingleSetup(ScenarioKind scenarioKind)
    {
        EnsureRunInBackground();
        Time.timeScale = 1f;
        isRunning = true;
        runSingleScenarioOnly = true;
        scenarioResults.Clear();
        ResetObservationState();
        QueueScenarioStart(scenarioKind, InitialScenarioBootstrapFrames, "run_single");
    }

    private void QueueScenarioStart(ScenarioKind scenarioKind, int delayFrames, string source)
    {
        queuedScenario = scenarioKind;
        queuedScenarioStartFrames = Mathf.Max(0, delayFrames);
        currentScenario = ScenarioKind.None;
        Debug.Log(
            $"{LogPrefix} scenario_queued={scenarioKind} source={source} " +
            $"delayFrames={queuedScenarioStartFrames} frame={Time.frameCount}");
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
        PlaceNpc(activeNpcB, new Vector2(-3.5f, 8.0f));
        PlaceNpc(activeNpcC, new Vector2(-1.5f, 8.5f));

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
        PlaceNpc(activeNpcB, new Vector2(-3.5f, 8.0f));
        PlaceNpc(activeNpcC, new Vector2(-1.5f, 8.5f));

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
        PlaceNpc(activeNpcB, new Vector2(-3.5f, 8.0f));
        PlaceNpc(activeNpcC, new Vector2(-1.5f, 8.5f));

        laneReferenceY = GetActorFootPosition(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>()).y;
        combinedRadius = GetActorFootprintRadius(playerCollider) + GetActorFootprintRadius(activeNpcA.Collider);
        npcMoveIssued = activeNpcA.Controller.DebugMoveTo(npcTarget);
        playerClickIssued = TryIssueAutoNavClick(activeGameInputManager, playerTarget);
        previousPlayerPos = GetActorFootPosition(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>());
        previousNpcPos = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        pushObservationInitialized = true;
        settleFramesRemaining = SetupSettleFrames;

        Debug.Log(
            $"{LogPrefix} scenario_setup=RealInputPlayerAvoidsMovingNpc npcMoveIssued={npcMoveIssued} " +
            $"playerClickIssued={playerClickIssued} playerActive={activePlayerNavigator.IsActive} " +
            $"npcMoving={activeNpcA.Controller.IsMoving} playerPos={(Vector2)activePlayerNavigator.transform.position} " +
                $"npcPos={(Vector2)activeNpcA.Transform.position}");
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
        playerClickIssued = TryIssueAutoNavClick(activeGameInputManager, playerTarget);
        previousPlayerPos = GetActorFootPosition(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>());
        previousNpcPos = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        pushObservationInitialized = true;
        settleFramesRemaining = SetupSettleFrames;

        Debug.Log(
            $"{LogPrefix} scenario_setup=RealInputPlayerSingleNpcNear playerClickIssued={playerClickIssued} " +
            $"playerActive={activePlayerNavigator.IsActive} playerPos={(Vector2)activePlayerNavigator.transform.position} " +
            $"npcPos={(Vector2)activeNpcA.Transform.position}");
    }

    private void SetupRealInputPlayerCrowdPass()
    {
        Debug.Log($"{LogPrefix} scenario_start=RealInputPlayerCrowdPass");
        if (!TryResolveActors(out activePlayerNavigator, out activeNpcA, out activeNpcB, out activeNpcC))
        {
            AbortRun("scenario=RealInputPlayerCrowdPass actors_missing");
            return;
        }

        activeGameInputManager = FindFirstObjectByType<GameInputManager>();
        if (activeGameInputManager == null)
        {
            AbortRun("scenario=RealInputPlayerCrowdPass game_input_missing");
            return;
        }

        PrepareScene(activePlayerNavigator, activeNpcA, activeNpcB, activeNpcC);

        Vector2 playerStart = new Vector2(-9.15f, 4.45f);
        playerTarget = new Vector2(-4.95f, 4.45f);
        Vector2 npcAStart = new Vector2(-7.45f, 4.95f);
        Vector2 npcBStart = new Vector2(-7.05f, 4.08f);
        Vector2 npcCStart = new Vector2(-6.62f, 4.88f);

        PlaceActor(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>(), playerStart);
        PlaceNpc(activeNpcA, npcAStart);
        PlaceNpc(activeNpcB, npcBStart);
        PlaceNpc(activeNpcC, npcCStart);

        laneReferenceY = playerStart.y;
        combinedRadius =
            GetActorFootprintRadius(activeNpcA.Collider) +
            Mathf.Max(GetActorFootprintRadius(activeNpcB.Collider), GetActorFootprintRadius(activeNpcC.Collider));
        playerClickIssued = TryIssueAutoNavClick(activeGameInputManager, playerTarget);
        previousPlayerPos = GetActorFootPosition(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>());
        previousPlayerMotion = Vector2.zero;
        settleFramesRemaining = SetupSettleFrames;

        Debug.Log(
            $"{LogPrefix} scenario_setup=RealInputPlayerCrowdPass playerClickIssued={playerClickIssued} " +
            $"playerPos={(Vector2)activePlayerNavigator.transform.position} npcAPos={(Vector2)activeNpcA.Transform.position} " +
            $"npcBPos={(Vector2)activeNpcB.Transform.position} npcCPos={(Vector2)activeNpcC.Transform.position}");
    }

    private bool TryIssueAutoNavClick(GameInputManager inputManager, Vector2 world)
    {
        if (inputManager == null)
        {
            return false;
        }

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
                $"minClearance={minClearance:F3}, maxNpcLateralOffset={maxNpcLateralOffset:F3}, timeout={scenarioTimer:F2}",
                $"minClearance={minClearance:F3} npcReached={npcReached}");
            return;
        }

        if (scenarioTimer >= ScenarioTimeout)
        {
            CompleteScenario(
                "NpcAvoidsPlayer",
                false,
                $"npcMoveIssued={npcMoveIssued}, npcReached={npcReached}, " +
                $"minClearance={minClearance:F3}, maxNpcLateralOffset={maxNpcLateralOffset:F3}, timeout={scenarioTimer:F2}",
                $"timeout={scenarioTimer:F2} minClearance={minClearance:F3} npcReached={npcReached}");
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

    private void TickRealInputPlayerAvoidsMovingNpc()
    {
        Rigidbody2D playerRb = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Rigidbody2D>() : null;
        Collider2D playerCollider = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Collider2D>() : null;
        Vector2 playerPos = GetActorFootPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerRb);
        Vector2 npcPos = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        Vector2 playerTransformPos = GetActorTransformPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null);
        Vector2 npcTransformPos = GetActorTransformPosition(activeNpcA.Transform);
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

        playerReached = !activePlayerNavigator.IsActive && Vector2.Distance(playerTransformPos, playerTarget) <= 0.7f;
        npcReached = !activeNpcA.Controller.IsMoving && Vector2.Distance(npcTransformPos, npcTarget) <= 0.3f;

        if (playerReached && npcReached)
        {
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
                $"maxPlayerLateralOffset={maxPlayerLateralOffset:F3}, npcPushDisplacement={npcPushDisplacement:F3}, " +
                $"pushContactFrames={npcPushContactFrames}, timeout={scenarioTimer:F2}",
                $"minClearance={minClearance:F3} pushDisplacement={npcPushDisplacement:F3} " +
                $"playerReached={playerReached} npcReached={npcReached}");
            return;
        }

        if (scenarioTimer >= ScenarioTimeout)
        {
            CompleteScenario(
                "RealInputPlayerAvoidsMovingNpc",
                false,
                $"playerClickIssued={playerClickIssued}, npcMoveIssued={npcMoveIssued}, " +
                $"playerReached={playerReached}, npcReached={npcReached}, minClearance={minClearance:F3}, " +
                $"maxPlayerLateralOffset={maxPlayerLateralOffset:F3}, npcPushDisplacement={npcPushDisplacement:F3}, " +
                $"pushContactFrames={npcPushContactFrames}, timeout={scenarioTimer:F2}",
                $"timeout={scenarioTimer:F2} minClearance={minClearance:F3} " +
                $"pushDisplacement={npcPushDisplacement:F3} playerReached={playerReached} npcReached={npcReached}");
        }
    }

    private void TickRealInputPlayerSingleNpcNear()
    {
        Rigidbody2D playerRb = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Rigidbody2D>() : null;
        Collider2D playerCollider = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Collider2D>() : null;
        Vector2 playerPos = GetActorFootPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerRb);
        Vector2 npcPos = GetActorFootPosition(activeNpcA.Transform, activeNpcA.Rigidbody);
        Vector2 playerTransformPos = GetActorTransformPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null);
        float edgeClearance = GetActorEdgeClearance(
            activePlayerNavigator != null ? activePlayerNavigator.transform : null,
            playerRb,
            playerCollider,
            activeNpcA.Transform,
            activeNpcA.Rigidbody,
            activeNpcA.Collider);

        minClearance = Mathf.Min(minClearance, edgeClearance);
        maxPlayerLateralOffset = Mathf.Max(maxPlayerLateralOffset, Mathf.Abs(playerPos.y - laneReferenceY));
        playerReached = !activePlayerNavigator.IsActive && Vector2.Distance(playerTransformPos, playerTarget) <= 0.7f;

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
                $"blockOnsetEdgeDistance={onsetDistance:F3}, maxPlayerLateralOffset={maxPlayerLateralOffset:F3}, timeout={scenarioTimer:F2}",
                $"minEdgeClearance={minClearance:F3} blockOnsetEdgeDistance={onsetDistance:F3} playerReached={playerReached}");
            return;
        }

        if (scenarioTimer >= ScenarioTimeout)
        {
            float onsetDistance = blockOnsetRecorded ? blockOnsetEdgeDistance : float.PositiveInfinity;
            CompleteScenario(
                "RealInputPlayerSingleNpcNear",
                false,
                $"playerClickIssued={playerClickIssued}, playerReached={playerReached}, minEdgeClearance={minClearance:F3}, " +
                $"blockOnsetEdgeDistance={(float.IsPositiveInfinity(onsetDistance) ? "n/a" : onsetDistance.ToString("F3"))}, " +
                $"maxPlayerLateralOffset={maxPlayerLateralOffset:F3}, timeout={scenarioTimer:F2}",
                $"timeout={scenarioTimer:F2} minEdgeClearance={minClearance:F3} " +
                $"blockOnsetEdgeDistance={(float.IsPositiveInfinity(onsetDistance) ? "n/a" : onsetDistance.ToString("F3"))} playerReached={playerReached}");
        }
    }

    private void TickRealInputPlayerCrowdPass()
    {
        Rigidbody2D playerRb = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Rigidbody2D>() : null;
        Collider2D playerCollider = activePlayerNavigator != null ? activePlayerNavigator.GetComponent<Collider2D>() : null;
        Vector2 playerPos = GetActorFootPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null, playerRb);
        Vector2 playerTransformPos = GetActorTransformPosition(activePlayerNavigator != null ? activePlayerNavigator.transform : null);
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
        playerReached = !activePlayerNavigator.IsActive && Vector2.Distance(playerTransformPos, playerTarget) <= 0.7f;

        UpdateCrowdOscillationMetrics(playerPos, playerTarget);

        if (playerReached)
        {
            bool passed =
                playerClickIssued &&
                minClearance > -0.08f &&
                playerDirectionFlipCount <= 2 &&
                crowdStallDuration <= 0.45f;
            CompleteScenario(
                "RealInputPlayerCrowdPass",
                passed,
                $"playerClickIssued={playerClickIssued}, playerReached={playerReached}, minEdgeClearance={minClearance:F3}, " +
                $"directionFlips={playerDirectionFlipCount}, crowdStallDuration={crowdStallDuration:F3}, timeout={scenarioTimer:F2}",
                $"minEdgeClearance={minClearance:F3} directionFlips={playerDirectionFlipCount} " +
                $"crowdStallDuration={crowdStallDuration:F3} playerReached={playerReached}");
            return;
        }

        if (scenarioTimer >= ScenarioTimeout)
        {
            CompleteScenario(
                "RealInputPlayerCrowdPass",
                false,
                $"playerClickIssued={playerClickIssued}, playerReached={playerReached}, minEdgeClearance={minClearance:F3}, " +
                $"directionFlips={playerDirectionFlipCount}, crowdStallDuration={crowdStallDuration:F3}, timeout={scenarioTimer:F2}",
                $"timeout={scenarioTimer:F2} minEdgeClearance={minClearance:F3} " +
                $"directionFlips={playerDirectionFlipCount} crowdStallDuration={crowdStallDuration:F3} playerReached={playerReached}");
        }
    }

    private void CompleteScenario(string scenarioName, bool passed, string details, string summary)
    {
        scenarioResults.Add(new ScenarioResult
        {
            Name = scenarioName,
            Passed = passed,
            Details = details
        });

        Debug.Log($"{LogPrefix} scenario_end={scenarioName} pass={passed} {summary}");

        if (runSingleScenarioOnly)
        {
            FinishRun();
            return;
        }

        switch (currentScenario)
        {
            case ScenarioKind.RealInputPlayerAvoidsMovingNpc:
                StartScenario(ScenarioKind.RealInputPlayerSingleNpcNear);
                break;

            case ScenarioKind.RealInputPlayerSingleNpcNear:
                StartScenario(ScenarioKind.RealInputPlayerCrowdPass);
                break;

            case ScenarioKind.RealInputPlayerCrowdPass:
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
        bool allPassed = true;
        for (int index = 0; index < scenarioResults.Count; index++)
        {
            ScenarioResult result = scenarioResults[index];
            allPassed &= result.Passed;
            Debug.Log($"{LogPrefix} scenario={result.Name} pass={result.Passed} details={result.Details}");
        }

        Debug.Log($"{LogPrefix} all_completed={allPassed} scenario_count={scenarioResults.Count}");
        isRunning = false;
        runSingleScenarioOnly = false;
        currentScenario = ScenarioKind.None;
        queuedScenario = ScenarioKind.None;
        queuedScenarioStartFrames = 0;
    }

    private void AbortRun(string reason)
    {
        Debug.LogError($"{LogPrefix} abort reason={reason}");
        isRunning = false;
        runSingleScenarioOnly = false;
        currentScenario = ScenarioKind.None;
        queuedScenario = ScenarioKind.None;
        queuedScenarioStartFrames = 0;
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
        previousPlayerPos = Vector2.zero;
        previousNpcPos = Vector2.zero;
        previousPlayerMotion = Vector2.zero;
        npcPushDisplacement = 0f;
        npcPushContactFrames = 0;
    }

    private bool EnsureScenarioRuntimeBindings()
    {
        bool needsPlayer = activePlayerNavigator == null;
        bool needsNpcA = activeNpcA.Controller == null;
        bool needsNpcB = currentScenario == ScenarioKind.NpcNpcCrossing || currentScenario == ScenarioKind.RealInputPlayerCrowdPass
            ? activeNpcB.Controller == null
            : false;
        bool needsNpcC = currentScenario == ScenarioKind.RealInputPlayerCrowdPass
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
             currentScenario == ScenarioKind.RealInputPlayerCrowdPass) &&
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

        Debug.Log(
            $"{LogPrefix} heartbeat scenario={currentScenario} timer={scenarioTimer:F2} time={Time.time:F2} " +
            $"unscaledTime={Time.unscaledTime:F2} delta={Time.deltaTime:F3} unscaledDelta={Time.unscaledDeltaTime:F3} " +
            $"timeScale={Time.timeScale:F2} playerPos={playerPos} npcAPos={npcAPos} npcBPos={npcBPos} " +
            $"playerActive={(activePlayerNavigator != null && activePlayerNavigator.IsActive)} " +
            $"npcAState={(activeNpcA.Controller != null ? activeNpcA.Controller.DebugState : "null")} " +
            $"npcBState={(activeNpcB.Controller != null ? activeNpcB.Controller.DebugState : "null")}");
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
            npcs[index].Controller.StopRoam();
            if (npcs[index].Rigidbody != null)
            {
                npcs[index].Rigidbody.linearVelocity = Vector2.zero;
            }
        }
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
        if (forwardProgress <= 0.01f)
        {
            blockOnsetRecorded = true;
            blockOnsetEdgeDistance = edgeClearance;
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
}
