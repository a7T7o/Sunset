using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Play Mode 下的一次性导航 live 验证器。
/// 使用显式状态机而不是协程，避免 MCP 会话里首个 yield 后失去推进。
/// </summary>
public class NavigationLiveValidationRunner : MonoBehaviour
{
    private const string LogPrefix = "[NavValidation]";
    private const float ScenarioTimeout = 6.5f;
    private const int SetupSettleFrames = 2;
    private const int HeartbeatFrameInterval = 45;

    private enum ScenarioKind
    {
        None = 0,
        PlayerAvoidsMovingNpc = 1,
        NpcAvoidsPlayer = 2,
        NpcNpcCrossing = 3
    }

    private bool isRunning;
    private ScenarioKind currentScenario;
    private int settleFramesRemaining;
    private float scenarioTimer;
    private int nextHeartbeatFrame;
    private float laneReferenceY;
    private float combinedRadius;
    private float minClearance;
    private float maxPlayerLateralOffset;
    private float maxNpcLateralOffset;
    private bool playerReached;
    private bool npcReached;
    private bool npcAReached;
    private bool npcBReached;
    private bool npcMoveIssued;
    private bool npcAMoveIssued;
    private bool npcBMoveIssued;
    private Vector2 playerTarget;
    private Vector2 npcTarget;
    private Vector2 npcATarget;
    private Vector2 npcBTarget;
    private PlayerAutoNavigator activePlayerNavigator;
    private NPCMovementBundle activeNpcA;
    private NPCMovementBundle activeNpcB;
    private NPCMovementBundle activeNpcC;

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

    public static void SetupNpcAvoidsPlayerProbe()
    {
        GetOrCreateRunner().RunSingleSetup(ScenarioKind.NpcAvoidsPlayer);
    }

    public static void SetupNpcNpcCrossingProbe()
    {
        GetOrCreateRunner().RunSingleSetup(ScenarioKind.NpcNpcCrossing);
    }

    public void RunAll()
    {
        Time.timeScale = 1f;
        isRunning = true;
        scenarioResults.Clear();
        ResetObservationState();
        Debug.Log($"{LogPrefix} runner_started");
        StartScenario(ScenarioKind.PlayerAvoidsMovingNpc);
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
        if (!Application.isPlaying || !isRunning || currentScenario == ScenarioKind.None)
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
        }
    }

    private void OnDisable()
    {
        Debug.Log($"{LogPrefix} runner_disabled");
    }

    private void OnDestroy()
    {
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
        }
    }

    private void RunSingleSetup(ScenarioKind scenarioKind)
    {
        Time.timeScale = 1f;
        isRunning = false;
        scenarioResults.Clear();
        StartScenario(scenarioKind);
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

        PlaceActor(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>(), playerStart);
        PlaceNpc(activeNpcA, npcStart);
        PlaceNpc(activeNpcB, new Vector2(-3.5f, 8.0f));
        PlaceNpc(activeNpcC, new Vector2(-1.5f, 8.5f));

        laneReferenceY = playerLaneY;
        combinedRadius = GetActorRadius(activePlayerNavigator.GetComponent<Collider2D>()) + GetActorRadius(activeNpcA.Collider);
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

        PlaceActor(activePlayerNavigator.transform, activePlayerNavigator.GetComponent<Rigidbody2D>(), playerStart);
        PlaceNpc(activeNpcA, npcStart);
        PlaceNpc(activeNpcB, new Vector2(-3.5f, 8.0f));
        PlaceNpc(activeNpcC, new Vector2(-1.5f, 8.5f));

        laneReferenceY = laneY;
        combinedRadius = GetActorRadius(activePlayerNavigator.GetComponent<Collider2D>()) + GetActorRadius(activeNpcA.Collider);
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

        combinedRadius = GetActorRadius(activeNpcA.Collider) + GetActorRadius(activeNpcB.Collider);
        npcAMoveIssued = activeNpcA.Controller.DebugMoveTo(npcATarget);
        npcBMoveIssued = activeNpcB.Controller.DebugMoveTo(npcBTarget);
        settleFramesRemaining = SetupSettleFrames;

        Debug.Log(
            $"{LogPrefix} scenario_setup=NpcNpcCrossing npcAMoveIssued={npcAMoveIssued} npcBMoveIssued={npcBMoveIssued} " +
            $"npcAPos={(Vector2)activeNpcA.Transform.position} npcBPos={(Vector2)activeNpcB.Transform.position}");
    }

    private void TickPlayerAvoidsMovingNpc()
    {
        Vector2 playerPos = activePlayerNavigator.transform.position;
        Vector2 npcPos = activeNpcA.Transform.position;
        float clearance = Vector2.Distance(playerPos, npcPos) - combinedRadius;

        minClearance = Mathf.Min(minClearance, clearance);
        maxPlayerLateralOffset = Mathf.Max(maxPlayerLateralOffset, Mathf.Abs(playerPos.y - laneReferenceY));
        playerReached = !activePlayerNavigator.IsActive && Vector2.Distance(playerPos, playerTarget) <= 0.7f;
        npcReached = !activeNpcA.Controller.IsMoving && Vector2.Distance(npcPos, npcTarget) <= 0.3f;

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
        Vector2 playerPos = activePlayerNavigator.transform.position;
        Vector2 npcPos = activeNpcA.Transform.position;
        float clearance = Vector2.Distance(playerPos, npcPos) - combinedRadius;

        minClearance = Mathf.Min(minClearance, clearance);
        maxNpcLateralOffset = Mathf.Max(maxNpcLateralOffset, Mathf.Abs(npcPos.y - laneReferenceY));
        npcReached = !activeNpcA.Controller.IsMoving && Vector2.Distance(npcPos, npcTarget) <= 0.3f;

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
        Vector2 npcAPos = activeNpcA.Transform.position;
        Vector2 npcBPos = activeNpcB.Transform.position;
        float clearance = Vector2.Distance(npcAPos, npcBPos) - combinedRadius;

        minClearance = Mathf.Min(minClearance, clearance);
        npcAReached = !activeNpcA.Controller.IsMoving && Vector2.Distance(npcAPos, npcATarget) <= 0.3f;
        npcBReached = !activeNpcB.Controller.IsMoving && Vector2.Distance(npcBPos, npcBTarget) <= 0.3f;

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

    private void CompleteScenario(string scenarioName, bool passed, string details, string summary)
    {
        scenarioResults.Add(new ScenarioResult
        {
            Name = scenarioName,
            Passed = passed,
            Details = details
        });

        Debug.Log($"{LogPrefix} scenario_end={scenarioName} pass={passed} {summary}");

        switch (currentScenario)
        {
            case ScenarioKind.PlayerAvoidsMovingNpc:
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
        currentScenario = ScenarioKind.None;
    }

    private void AbortRun(string reason)
    {
        Debug.LogError($"{LogPrefix} abort reason={reason}");
        isRunning = false;
        currentScenario = ScenarioKind.None;
    }

    private void ResetObservationState()
    {
        settleFramesRemaining = 0;
        scenarioTimer = 0f;
        laneReferenceY = 0f;
        combinedRadius = 0f;
        minClearance = float.PositiveInfinity;
        maxPlayerLateralOffset = 0f;
        maxNpcLateralOffset = 0f;
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
        activeNpcA = default;
        activeNpcB = default;
        activeNpcC = default;
    }

    private void MaybeLogHeartbeat()
    {
        if (Time.frameCount < nextHeartbeatFrame)
        {
            return;
        }

        nextHeartbeatFrame = Time.frameCount + HeartbeatFrameInterval;
        Vector2 playerPos = activePlayerNavigator != null ? (Vector2)activePlayerNavigator.transform.position : Vector2.zero;
        Vector2 npcAPos = activeNpcA.Transform != null ? (Vector2)activeNpcA.Transform.position : Vector2.zero;
        Vector2 npcBPos = activeNpcB.Transform != null ? (Vector2)activeNpcB.Transform.position : Vector2.zero;

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
