using System.Collections;
using System.Collections.Generic;
using Sunset.Story;
using UnityEngine;

/// <summary>
/// NPC 自动漫游控制器。
/// 负责随机选点、路径跟随、短停/长停节奏，以及长停时的自言自语或附近聊天。
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(NPCMotionController))]
public class NPCAutoRoamController : MonoBehaviour, INavigationUnit
{
    private static readonly NavigationPathExecutor2D.DetourCandidateSpec[] SharedAvoidanceDetourCandidateSpecs =
    {
        new NavigationPathExecutor2D.DetourCandidateSpec(false, 1.2f, 0.4f),
        new NavigationPathExecutor2D.DetourCandidateSpec(false, 1.45f, 0.75f),
        new NavigationPathExecutor2D.DetourCandidateSpec(true, 1.35f, 0.65f),
        new NavigationPathExecutor2D.DetourCandidateSpec(true, 1.6f, 0.95f),
    };

    private const float AmbientChatRetryDelay = 0.9f;
    private const float AmbientChatRetryMinRemainingTime = 1.25f;
    private const int AmbientChatMaxRetryCount = 3;
    private const float AmbientBubbleRetryDelay = 0.08f;
    private const int AmbientBubbleMaxShowAttempts = 3;
    private const float SHARED_DETOUR_MIN_ACTIVE_DURATION = 0.35f;
    private const int SHARED_DETOUR_RELEASE_STABLE_FRAMES = 3;
    private const float SHARED_DETOUR_POST_RELEASE_RECOVERY_WINDOW = 0.22f;
    private const float BLOCKED_ADVANCE_RECOVERY_COOLDOWN = 0.18f;
    private const float BLOCKED_ADVANCE_SAME_POSITION_RADIUS = 0.18f;
    private const float REBUILD_REQUEST_DEDUPE_WINDOW_SECONDS = 0.12f;
    private const float REBUILD_REQUEST_DESTINATION_RADIUS = 0.28f;
    private const int BLOCKED_ADVANCE_REROUTE_MIN_FRAMES = 3;
    private const int BLOCKED_ADVANCE_STATIC_ABORT_MIN_FRAMES = 2;
    private const int AUTONOMOUS_STATIC_ABORT_MIN_FRAMES = 4;
    private const int FORMAL_NAVIGATION_BLOCKED_ABORT_MIN_FRAMES = 8;
    private const int BLOCKED_ADVANCE_LONG_PAUSE_MIN_FRAMES = 6;
    private const float MOVE_COMMAND_NO_PROGRESS_RADIUS = 0.015f;
    private const float MOVE_COMMAND_NO_PROGRESS_GRACE_SECONDS = 0.06f;
    private const float MOVE_COMMAND_OSCILLATION_SAME_POSITION_RADIUS = 0.14f;
    private const float MOVE_COMMAND_OSCILLATION_FLIP_DOT_THRESHOLD = 0.15f;
    private const int MOVE_COMMAND_OSCILLATION_MIN_FLIPS = 2;
    private const float MOVE_DIRECTION_STABILIZE_SAME_POSITION_RADIUS = 0.16f;
    private const float MOVE_DIRECTION_STABILIZE_FLIP_DOT_THRESHOLD = 0.05f;
    private const float PHYSICS_HEAVY_DECISION_REUSE_SECONDS = 0.02f;
    private const float SCRIPTED_MOVE_DECISION_REUSE_SECONDS = 0.08f;
    private const float NPC_BAD_CASE_STOP_DECISION_REUSE_SECONDS = 0.12f;
    private const float CONSTRAINED_ADVANCE_FALLBACK_EXTRA_DISTANCE = 0.18f;
    private const int TERMINAL_STUCK_LONG_PAUSE_MIN_COUNT = 2;
    private const float TERMINAL_STUCK_SAME_POSITION_RADIUS = 0.2f;
    private const float AVOIDANCE_STUCK_RESET_MAX_MOVE_SCALE = 0.32f;
    private const int PASSIVE_NPC_BLOCKER_REROUTE_MIN_SIGHTINGS = 3;
    private const float PASSIVE_NPC_BLOCKER_REROUTE_MAX_DISTANCE = 1.15f;
    private const float PASSIVE_NPC_BLOCKER_REROUTE_MAX_CLEARANCE = 0.32f;
    private const float FRAME_REUSE_WAYPOINT_DISTANCE_PADDING = 0.04f;
    private const float PATH_BUILD_FAILURE_BASE_COOLDOWN = 0.08f;
    private const float PATH_BUILD_FAILURE_MAX_COOLDOWN = 0.48f;
    private const int PATH_BUILD_FAILURE_BACKOFF_STEP_CAP = 3;
    private const float HOME_ANCHOR_MISMATCH_MIN_DISTANCE = 2.25f;
    private const float HOME_ANCHOR_MISMATCH_PADDING = 0.5f;
    private const float AUTONOMOUS_ROAM_DESTINATION_CORRECTION_MIN = 0.22f;
    private const float AUTONOMOUS_ROAM_DESTINATION_CORRECTION_MAX = 0.65f;
    private const float AUTONOMOUS_ROAM_SOFT_ARRIVAL_DISTANCE_MIN = 0.22f;
    private const float AUTONOMOUS_ROAM_SOFT_ARRIVAL_DISTANCE_MAX = 0.52f;
    private const float AUTONOMOUS_ROAM_PATH_DETOUR_RATIO_LIMIT = 1.8f;
    private const float AUTONOMOUS_ROAM_PATH_DETOUR_MIN_EXTRA_DISTANCE = 0.45f;
    private const float AUTONOMOUS_ROAM_PATH_ROAM_RADIUS_MARGIN = 0.4f;
    private const float AUTONOMOUS_ROAM_NEIGHBOR_CLEARANCE_RADIUS_SCALE = 0.7f;
    private const float AUTONOMOUS_ROAM_NEIGHBOR_CLEARANCE_MIN_RADIUS = 0.16f;
    private const float AUTONOMOUS_ROAM_NEIGHBOR_CLEARANCE_MAX_RADIUS = 0.48f;
    private const int AUTONOMOUS_ROAM_NEIGHBOR_CLEARANCE_MIN_OPEN_PROBES = 2;
    private const float AUTONOMOUS_ROAM_BODY_CLEARANCE_EXTRA_RADIUS_SCALE = 0.38f;
    private const float AUTONOMOUS_ROAM_BODY_CLEARANCE_MIN_EXTRA_RADIUS = 0.05f;
    private const float AUTONOMOUS_ROAM_BODY_CLEARANCE_MAX_EXTRA_RADIUS = 0.34f;
    private const float AUTONOMOUS_ROAM_PATH_CLEARANCE_SAMPLE_SPACING = 0.18f;
    private const float AUTONOMOUS_ROAM_SAFE_CENTER_SEARCH_RADIUS_SCALE = 0.18f;
    private const float AUTONOMOUS_ROAM_SAFE_CENTER_MIN_SEARCH_RADIUS = 0.22f;
    private const float AUTONOMOUS_ROAM_SAFE_CENTER_MAX_SEARCH_RADIUS = 1.1f;
    private const int AUTONOMOUS_ROAM_SAFE_CENTER_RING_COUNT = 3;
    private const int AUTONOMOUS_ROAM_STATIC_RECOVERY_SAMPLE_BUDGET = 6;
    private const float AUTONOMOUS_STATIC_STEERING_REPULSE_GAIN = 0.82f;
    private const float AUTONOMOUS_STATIC_STEERING_MAX_ANGLE = 58f;
    private const float AUTONOMOUS_ROAM_DESTINATION_AGENT_CLEARANCE_RADIUS_SCALE = 0.92f;
    private const float AUTONOMOUS_ROAM_DESTINATION_AGENT_CLEARANCE_MIN_RADIUS = 0.26f;
    private const float AUTONOMOUS_ROAM_DESTINATION_AGENT_CLEARANCE_MAX_RADIUS = 0.72f;
    private const float RELEASED_MOVEMENT_DESTINATION_CORRECTION_MIN = 0.24f;
    private const float RELEASED_MOVEMENT_DESTINATION_CORRECTION_MAX = 0.95f;
    private const float RELEASED_MOVEMENT_NEIGHBOR_CLEARANCE_RADIUS_SCALE = 0.82f;
    private const float RELEASED_MOVEMENT_NEIGHBOR_CLEARANCE_MIN_RADIUS = 0.18f;
    private const float RELEASED_MOVEMENT_NEIGHBOR_CLEARANCE_MAX_RADIUS = 0.56f;
    private const int RELEASED_MOVEMENT_NEIGHBOR_CLEARANCE_MIN_OPEN_PROBES = 2;
    private const float RELEASED_MOVEMENT_PATH_CLEARANCE_SAMPLE_SPACING = 0.16f;
    private const int STATIC_RECOVERY_DETOUR_MIN_BLOCKED_FRAMES = 2;
    private const float STATIC_RECOVERY_DETOUR_MIN_DISTANCE = 0.24f;
    private const float STATIC_RECOVERY_DETOUR_MAX_DISTANCE = 0.96f;
    private const float STATIC_BLOCKED_DESTINATION_RETRY_COOLDOWN = 1.2f;
    private const float STATIC_BLOCKED_DESTINATION_RETRY_RADIUS = 0.68f;
    private const int SHARED_AVOIDANCE_DYNAMIC_ABORT_MIN_FRAMES = 10;
    private const int SHARED_AVOIDANCE_DYNAMIC_ABORT_MIN_SIGHTINGS = 3;
    private const float SHARED_AVOIDANCE_DYNAMIC_ABORT_MAX_DISTANCE = 1.28f;
    private const float SHARED_AVOIDANCE_DYNAMIC_ABORT_MAX_CLEARANCE = 0.28f;
    private const float FORMAL_NAVIGATION_RESTART_SETTLE_SECONDS = 0.45f;
    private const float FORMAL_NAVIGATION_RESTART_SETTLE_RADIUS = 0.65f;
    private const float RELEASED_STEP_BODY_CLEARANCE_SIDE_INSET_SCALE = 0.18f;
    private const float RELEASED_STEP_BODY_CLEARANCE_VERTICAL_INSET_SCALE = 0.35f;

    private enum RoamState
    {
        Inactive = 0,
        ShortPause = 1,
        Moving = 2,
        LongPause = 3
    }

    private enum AmbientChatAttemptResult
    {
        Success = 0,
        Disabled = 1,
        BubblePresenterMissing = 2,
        InitiatorLinesMissing = 3,
        RadiusInvalid = 4,
        ChanceMissed = 5,
        NoNearbyPartner = 6,
        PartnerRejected = 7
    }

    public enum RoamMoveInterruptionReason
    {
        None = 0,
        SharedAvoidanceRecovered = 1,
        SharedAvoidanceRepathFailed = 2,
        StuckCancel = 3,
        StuckRecoveryFailed = 4
    }

    public enum RoamMoveInterruptionBlockerKind
    {
        None = 0,
        Player = 1,
        NPC = 2,
        Enemy = 3,
        StaticObstacle = 4,
        Unknown = 5
    }

    public readonly struct RoamMoveInterruptionSnapshot
    {
        public readonly RoamMoveInterruptionReason Reason;
        public readonly RoamMoveInterruptionBlockerKind BlockerKind;
        public readonly int BlockerId;
        public readonly Vector2 RequestedDestination;
        public readonly Vector2 ActiveDestination;
        public readonly Vector2 CurrentPosition;
        public readonly Vector2 BlockerPosition;
        public readonly bool HasBlockerPosition;
        public readonly float TriggerTime;
        public readonly string Trigger;

        public RoamMoveInterruptionSnapshot(
            RoamMoveInterruptionReason reason,
            RoamMoveInterruptionBlockerKind blockerKind,
            int blockerId,
            Vector2 requestedDestination,
            Vector2 activeDestination,
            Vector2 currentPosition,
            Vector2 blockerPosition,
            bool hasBlockerPosition,
            float triggerTime,
            string trigger)
        {
            Reason = reason;
            BlockerKind = blockerKind;
            BlockerId = blockerId;
            RequestedDestination = requestedDestination;
            ActiveDestination = activeDestination;
            CurrentPosition = currentPosition;
            BlockerPosition = blockerPosition;
            HasBlockerPosition = hasBlockerPosition;
            TriggerTime = triggerTime;
            Trigger = trigger;
        }
    }

    [Header("组件引用")]
    [SerializeField] private NPCMotionController motionController;
    [SerializeField] private NPCBubblePresenter bubblePresenter;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private NavGrid2D navGrid;
    [SerializeField] private Transform homeAnchor;
#pragma warning disable CS0649 // Assigned by Unity serialization.
    [SerializeField] private NPCRoamProfile roamProfile;
#pragma warning restore CS0649

    [Header("基础开关")]
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool applyProfileOnAwake = true;

    [Header("活动范围")]
    [SerializeField] private float activityRadius = 3f;
    [SerializeField] private float minimumMoveDistance = 0.6f;
    [SerializeField] private int pathSampleAttempts = 12;

    [Header("短暂停留")]
    [SerializeField] private float shortPauseMin = 0.5f;
    [SerializeField] private float shortPauseMax = 3f;
    [SerializeField] private int shortPauseCountMin = 3;
    [SerializeField] private int shortPauseCountMax = 5;

    [Header("长停留")]
    [SerializeField] private float longPauseMin = 3f;
    [SerializeField] private float longPauseMax = 6f;

    [Header("环境聊天")]
    [SerializeField] private bool enableAmbientChat = true;
    [SerializeField] private float ambientChatRadius = 3.8f;
    [SerializeField, Range(0f, 1f)] private float ambientChatChance = 0.75f;
    [SerializeField] private float ambientChatResponseDelay = 0.85f;
    [SerializeField] private string[] chatInitiatorLines =
    {
        "今天这边挺安静的。",
        "你也在这边歇一会儿啊。",
        "这附近风吹着还挺舒服。",
        "等会儿再继续忙吧。"
    };
    [SerializeField] private string[] chatResponderLines =
    {
        "是啊，先缓一缓。",
        "我也刚走到这儿。",
        "这边待着还挺舒服的。",
        "好，等下再去转转。"
    };

    [Header("路径跟随")]
    [SerializeField] private float waypointTolerance = 0.08f;
    [SerializeField] private float destinationTolerance = 0.12f;

    [Header("Traversal 契约")]
    [SerializeField] private bool enforceNavGridBounds = true;
    [SerializeField] private bool allowTraversalOverridePhysicsSoftPass = true;
    [SerializeField, Min(0f)] private float navigationFootProbeVerticalInset = 0.08f;
    [SerializeField, Min(0f)] private float navigationFootProbeSideInset = 0.05f;
    [SerializeField, Min(0f)] private float navigationFootProbeExtraRadius = 0.02f;

    [Header("卡住恢复")]
    [SerializeField] private float stuckCheckInterval = 0.3f;
    [SerializeField] private float stuckDistanceThreshold = 0.08f;
    [SerializeField] private int maxStuckRecoveries = 3;

    [Header("调试")]
    [SerializeField] private bool showDebugLog = false;
#if UNITY_EDITOR
    [SerializeField] private bool drawDebugPath = true;
#endif

    [Header("共享动态避让")]
    [SerializeField] private float avoidanceRadius = 0.6f;
    [SerializeField] private int avoidancePriority = 50;
    [SerializeField] private float sharedAvoidanceLookAhead = 0.8f;
    [SerializeField] private float sharedAvoidanceRepathCooldown = 0.45f;

    private readonly List<NavigationAgentSnapshot> nearbyNavigationAgents = new List<NavigationAgentSnapshot>(12);
    private readonly List<NPCAutoRoamController> registeredRoamControllers = new List<NPCAutoRoamController>(24);
    private readonly List<AutonomousRoamCandidate> autonomousRoamCandidates = new List<AutonomousRoamCandidate>(16);
    private readonly NavigationPathExecutor2D.ExecutionState navigationExecution = new NavigationPathExecutor2D.ExecutionState();
    private ContactFilter2D staticObstacleProbeFilter = new ContactFilter2D().NoFilter();
    private Collider2D[] staticObstacleProbeBuffer = new Collider2D[12];
    private int registeredRoamControllersFrame = -1;

    private readonly struct BlockedRecoveryState
    {
        public readonly int BlockedAdvanceFrames;
        public readonly Vector2 LastBlockedAdvancePosition;
        public readonly int ConsecutiveTerminalStuckCount;
        public readonly Vector2 LastTerminalStuckPosition;

        public BlockedRecoveryState(
            int blockedAdvanceFrames,
            Vector2 lastBlockedAdvancePosition,
            int consecutiveTerminalStuckCount,
            Vector2 lastTerminalStuckPosition)
        {
            BlockedAdvanceFrames = blockedAdvanceFrames;
            LastBlockedAdvancePosition = lastBlockedAdvancePosition;
            ConsecutiveTerminalStuckCount = consecutiveTerminalStuckCount;
            LastTerminalStuckPosition = lastTerminalStuckPosition;
        }
    }

    private readonly struct AutonomousRoamCandidate
    {
        public readonly Vector2 Destination;
        public readonly float Score;

        public AutonomousRoamCandidate(Vector2 destination, float score)
        {
            Destination = destination;
            Score = score;
        }
    }

    private enum DestinationAcceptanceContract
    {
        AutonomousRoam = 0,
        ReleasedMovement = 1
    }

    private enum PointToPointTravelContract
    {
        None = 0,
        PlainDebug = 1,
        AutonomousDirected = 2,
        ResidentScripted = 3,
        FormalNavigation = 4
    }

    private RoamState state = RoamState.Inactive;
    private Vector2 homePosition;
    private float stateTimer;
    private int completedShortPauseCount;
    private int longPauseTriggerCount;
    private bool warnedMissingNavGrid;
    private Coroutine ambientChatCoroutine;
    private NPCAutoRoamController chatPartner;
    private string lastAmbientDecision = string.Empty;
    private bool ambientChatRetryPending;
    private float ambientChatRetryAtTime;
    private int ambientChatRetryCount;
    private bool debugMoveActive;
    private PointToPointTravelContract activePointToPointTravelContract = PointToPointTravelContract.None;
    private readonly List<string> residentScriptedControlOwners = new List<string>(2);
    private Collider2D navigationCollider;
    private Collider2D[] traversalSoftPassBlockers = new Collider2D[0];
    private bool isTraversalSoftPassActive;
    private float lastSharedAvoidanceRepathTime = float.NegativeInfinity;
    private int lastBlockingAgentId;
    private int blockingAgentSightings;
    private int lastSharedAvoidanceDebugFrame = -999;
    private float lastSharedAvoidanceReleaseTime = float.NegativeInfinity;
    private int sharedAvoidanceNoBlockerFrames;
    private int sharedAvoidanceBlockingFrames;
    private int sharedAvoidanceDetourCreateCount;
    private int sharedAvoidanceReleaseAttemptCount;
    private int sharedAvoidanceReleaseSuccessCount;
    private Vector2 requestedDestination;
    private bool hasRequestedDestination;
    private bool hasRoamInterruption;
    private RoamMoveInterruptionSnapshot lastRoamInterruption;
    private float lastBlockedAdvanceRecoveryTime = float.NegativeInfinity;
    private int blockedAdvanceFrames;
    private Vector2 lastBlockedAdvancePosition;
    private float lastRebuildRequestTime = float.NegativeInfinity;
    private Vector2 lastRebuildRequestPosition;
    private Vector2 lastRebuildRequestDestination;
    private bool hasPendingMoveCommandProgressCheck;
    private Vector2 pendingMoveCommandOrigin;
    private float pendingMoveCommandIssuedAt = float.NegativeInfinity;
    private Vector2 lastIssuedMoveDirection;
    private Vector2 lastIssuedMovePosition;
    private int consecutiveMoveCommandDirectionFlips;
    private int lastTraversalSoftPassUpdateFrame = -1;
    private int lastHeavyMoveDecisionFrame = -1;
    private float lastHeavyMoveDecisionTime = float.NegativeInfinity;
    private bool hasCachedMoveDecisionVelocity;
    private Vector2 cachedMoveDecisionVelocity;
    private Vector2 cachedMoveFacingDirection;
    private int lastPathBuildAttemptFrame = -1;
    private int consecutivePathBuildFailures;
    private float nextPathBuildAllowedTime = float.NegativeInfinity;
    private int consecutiveTerminalStuckCount;
    private Vector2 lastTerminalStuckPosition;
    private string lastMoveSkipReason = "None";
    private string residentScriptedControlOwnerKey = string.Empty;
    private bool resumeRoamAfterResidentScriptedControl;
    private bool residentScriptedMovePaused;
    private float lastFormalNavigationArrivalTime = float.NegativeInfinity;
    private Vector2 lastFormalNavigationArrivalPosition;
    private float lastStaticBlockedDestinationTime = float.NegativeInfinity;
    private Vector2 lastStaticBlockedDestination;
    private Vector2 lastStaticBlockedPosition;

    private List<Vector2> path => navigationExecution.Path;
    private Vector2 currentDestination { get => navigationExecution.Destination; set => navigationExecution.Destination = value; }
    private int currentPathIndex { get => navigationExecution.PathIndex; set => navigationExecution.PathIndex = value; }
    private Vector2 lastProgressCheckPosition { get => navigationExecution.LastProgressCheckPosition; set => navigationExecution.LastProgressCheckPosition = value; }
    private float lastProgressCheckTime { get => navigationExecution.LastProgressCheckTime; set => navigationExecution.LastProgressCheckTime = value; }
    private float lastProgressDistance { get => navigationExecution.LastProgressDistance; set => navigationExecution.LastProgressDistance = value; }
    private int currentStuckRecoveryCount { get => navigationExecution.StuckRetryCount; set => navigationExecution.StuckRetryCount = value; }
    private bool hasDynamicDetour => navigationExecution.HasOverrideWaypoint;

    public bool IsRoaming => state != RoamState.Inactive;
    public bool IsMoving => state == RoamState.Moving;
    public bool IsInAmbientChat => chatPartner != null;
    public bool IsResidentScriptedControlActive => residentScriptedControlOwners.Count > 0;
    public bool IsResidentScriptedMovePaused => residentScriptedMovePaused;
    public bool IsResidentScriptedMoveActive => IsResidentScriptedControlActive && HasActivePointToPointTravelContract() && state == RoamState.Moving && !residentScriptedMovePaused;
    public string ResidentScriptedControlOwnerKey => residentScriptedControlOwnerKey ?? string.Empty;
    public string ResidentStableKey => ResolveNpcId();
    public bool ResumeRoamWhenResidentControlReleases => resumeRoamAfterResidentScriptedControl;
    public bool IsNativeResidentRuntimeCandidate => gameObject.scene.IsValid() && !string.IsNullOrWhiteSpace(ResidentStableKey);
    public string ChatPartnerName => chatPartner != null ? chatPartner.name : string.Empty;
    public string LastAmbientDecision => lastAmbientDecision;
    public string DebugState => state.ToString();
    public float DebugStateTimer => stateTimer;
    public int CompletedShortPauseCount => completedShortPauseCount;
    public int LongPauseTriggerCount => longPauseTriggerCount;
    public int CurrentStuckRecoveryCount => currentStuckRecoveryCount;
    public float DebugLastProgressDistance => lastProgressDistance;
    public string DebugLastMoveSkipReason => lastMoveSkipReason;
    public int DebugBlockedAdvanceFrames => blockedAdvanceFrames;
    public int DebugConsecutivePathBuildFailures => consecutivePathBuildFailures;
    public float DebugPathBuildCooldownRemaining => Mathf.Max(0f, nextPathBuildAllowedTime - Time.time);
    public float DebugPendingMoveCommandAge =>
        hasPendingMoveCommandProgressCheck && pendingMoveCommandIssuedAt > float.NegativeInfinity
            ? Time.time - pendingMoveCommandIssuedAt
            : -1f;
    public NPCRoamProfile RoamProfile => roamProfile;
    public bool DebugHasSharedAvoidanceDetour => navigationExecution.HasOverrideWaypoint;
    public int DebugSharedAvoidanceNoBlockerFrames => sharedAvoidanceNoBlockerFrames;
    public int DebugSharedAvoidanceBlockingFrames => sharedAvoidanceBlockingFrames;
    public int DebugSharedAvoidanceDetourCreateCount => sharedAvoidanceDetourCreateCount;
    public int DebugSharedAvoidanceReleaseAttemptCount => sharedAvoidanceReleaseAttemptCount;
    public int DebugSharedAvoidanceReleaseSuccessCount => sharedAvoidanceReleaseSuccessCount;
    public int DebugLastDetourOwnerId => navigationExecution.LastDetourOwnerId;
    public int DebugOverrideWaypointOwnerId => navigationExecution.OverrideWaypointOwnerId;
    public bool DebugLastDetourRecoverySucceeded => navigationExecution.LastDetourRecoverySucceeded;
    public bool DebugHasRoamInterruption => hasRoamInterruption;
    public string DebugLastRoamInterruptionReason => lastRoamInterruption.Reason.ToString();
    public string DebugLastRoamInterruptionBlockerKind => lastRoamInterruption.BlockerKind.ToString();
    public int DebugLastRoamInterruptionBlockerId => lastRoamInterruption.BlockerId;
    public Vector2 DebugLastRoamInterruptionRequestedDestination => lastRoamInterruption.RequestedDestination;
    public Vector2 DebugLastRoamInterruptionActiveDestination => lastRoamInterruption.ActiveDestination;
    public Vector2 DebugLastRoamInterruptionCurrentPosition => lastRoamInterruption.CurrentPosition;
    public Vector2 DebugLastRoamInterruptionBlockerPosition => lastRoamInterruption.BlockerPosition;
    public bool DebugLastRoamInterruptionHasBlockerPosition => lastRoamInterruption.HasBlockerPosition;
    public float DebugLastRoamInterruptionTime => lastRoamInterruption.TriggerTime;
    public string DebugLastRoamInterruptionTrigger => lastRoamInterruption.Trigger;
    public RoamMoveInterruptionSnapshot DebugLastRoamInterruption => lastRoamInterruption;
    public event System.Action<RoamMoveInterruptionSnapshot> RoamMoveInterrupted;
    public NavigationUnitType GetUnitType() => NavigationUnitType.NPC;
    public Vector2 GetPosition() => GetNavigationCenter();
    public float GetColliderRadius()
    {
        return navigationCollider != null
            ? Mathf.Max(navigationCollider.bounds.extents.x, navigationCollider.bounds.extents.y)
            : 0.25f;
    }
    public bool ShouldAvoid(INavigationUnit other)
    {
        return other != null && other.GetUnitType() != NavigationUnitType.StaticObstacle;
    }
    public float GetAvoidanceRadius()
    {
        float colliderRadius = GetColliderRadius();
        float cappedShell = Mathf.Min(avoidanceRadius, colliderRadius + 0.12f);
        return Mathf.Max(colliderRadius, cappedShell);
    }
    public Vector2 GetCurrentVelocity()
    {
        if (motionController != null)
        {
            Vector2 currentVelocity = motionController.CurrentVelocity;
            if (currentVelocity.sqrMagnitude > 0.0001f)
            {
                return currentVelocity;
            }

            if (rb != null && rb.linearVelocity.sqrMagnitude > 0.0001f)
            {
                return rb.linearVelocity;
            }

            // Shared avoidance should prefer real motion first. Falling back to reported intent
            // keeps scripted moves working, but prevents crowded NPCs from advertising a stale
            // commanded velocity when their body is actually being constrained by contact.
            currentVelocity = motionController.ReportedVelocity;
            if (currentVelocity.sqrMagnitude > 0.0001f)
            {
                return currentVelocity;
            }
        }

        return rb != null ? rb.linearVelocity : Vector2.zero;
    }
    public int GetAvoidancePriority() => avoidancePriority;
    public bool IsCurrentlyMoving() => state == RoamState.Moving && GetCurrentVelocity().sqrMagnitude > 0.0001f;
    public bool IsNavigationSleeping() => state != RoamState.Moving || GetCurrentVelocity().sqrMagnitude <= 0.0001f;
    public bool ParticipatesInLocalAvoidance() => state == RoamState.Moving;
    public Transform HomeAnchor => homeAnchor;
    public float ActivityRadius => activityRadius;
    public float MinimumMoveDistance => minimumMoveDistance;
    public int PathSampleAttempts => pathSampleAttempts;
    public bool AutoStartEnabled => autoStart;
    public bool ApplyProfileOnAwakeEnabled => applyProfileOnAwake;
    public int DebugPathPointCount => path.Count;
    public Vector2 DebugRoamCenter => GetRoamCenter();

    public bool TryResolveOccupiablePosition(Vector2 sampledPosition, out Vector2 resolvedPosition)
    {
        CacheComponents();
        EnsureRuntimeHomeAnchorBound();
        return TryResolveOccupiableDestination(sampledPosition, out resolvedPosition);
    }

    private void Reset()
    {
        CacheComponents();
    }

    private void Awake()
    {
        CacheComponents();
        EnsureRuntimeHomeAnchorBound();

        if (applyProfileOnAwake)
        {
            ApplyProfile();
        }

        RefreshHomePositionFromCurrentContext();
    }

    private void OnEnable()
    {
        NavigationAgentRegistry.Register(this);
    }

    private void Start()
    {
        ResetLongPauseTriggerCount();
        ResetMovementRecovery(GetNavigationCenter(), resetCounter: true);

        if (autoStart)
        {
            StartRoam();
        }
        else if (motionController != null)
        {
            motionController.StopMotion();
        }
    }

    private void Update()
    {
        if (resumeRoamAfterResidentScriptedControl &&
            !IsResidentScriptedControlActive &&
            !ShouldSuspendForDialogue() &&
            enabled &&
            isActiveAndEnabled)
        {
            resumeRoamAfterResidentScriptedControl = false;
            StartRoam();
        }

        if (ShouldSuspendResidentRuntime())
        {
            ApplyResidentRuntimeFreeze();
            return;
        }

        UpdateTraversalSoftPassStateOncePerFrame(GetNavigationCenter());

        switch (state)
        {
            case RoamState.ShortPause:
                TickShortPause();
                break;

            case RoamState.Moving:
                if (rb == null)
                {
                    TickMoving(Time.deltaTime);
                }
                break;

            case RoamState.LongPause:
                TickLongPause();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (ShouldSuspendResidentRuntime())
        {
            ApplyResidentRuntimeFreeze();
            return;
        }

        if (state == RoamState.Moving && rb != null)
        {
            TickMoving(Time.fixedDeltaTime);
        }
    }

    private bool ShouldSuspendResidentRuntime()
    {
        if (ShouldSuspendForDialogue())
        {
            return true;
        }

        return IsResidentScriptedControlActive && !IsResidentScriptedMoveActive;
    }

    private static bool ShouldSuspendForDialogue()
    {
        return DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive;
    }

    private void ApplyResidentRuntimeFreeze()
    {
        ClearMoveDecisionCache();
        BreakAmbientChatLink(hideBubble: true);

        if (motionController != null)
        {
            motionController.StopMotion();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        bool preserveScriptedPriorityBubble =
            bubblePresenter != null
            && !ShouldSuspendForDialogue()
            && IsResidentScriptedControlActive
            && !IsResidentScriptedMoveActive
            && bubblePresenter.IsConversationPriorityVisible;

        if (bubblePresenter != null && !preserveScriptedPriorityBubble)
        {
            bubblePresenter.HideBubble();
        }
    }

    private void OnDisable()
    {
        NavigationAgentRegistry.Unregister(this);
        if (IsResidentScriptedControlActive)
        {
            // Disabled-time resident freezes must preserve the "resume roam on release" intent.
            HaltResidentScriptedMovement();
        }
        else
        {
            StopRoam();
        }

        ClearTraversalSoftPassState();
    }

    private void OnDestroy()
    {
        ClearTraversalSoftPassState();
    }

    private void OnValidate()
    {
        activityRadius = Mathf.Max(0.5f, activityRadius);
        minimumMoveDistance = Mathf.Clamp(minimumMoveDistance, 0.1f, activityRadius);
        pathSampleAttempts = Mathf.Max(1, pathSampleAttempts);

        shortPauseMin = Mathf.Max(0.05f, shortPauseMin);
        shortPauseMax = Mathf.Max(shortPauseMin, shortPauseMax);
        shortPauseCountMin = Mathf.Max(1, shortPauseCountMin);
        shortPauseCountMax = Mathf.Max(shortPauseCountMin, shortPauseCountMax);

        longPauseMin = Mathf.Max(0.1f, longPauseMin);
        longPauseMax = Mathf.Max(longPauseMin, longPauseMax);

        ambientChatRadius = Mathf.Max(0f, ambientChatRadius);
        ambientChatResponseDelay = Mathf.Max(0f, ambientChatResponseDelay);

        waypointTolerance = Mathf.Max(0.01f, waypointTolerance);
        destinationTolerance = Mathf.Max(waypointTolerance, destinationTolerance);
        navigationFootProbeVerticalInset = Mathf.Max(0f, navigationFootProbeVerticalInset);
        navigationFootProbeSideInset = Mathf.Max(0f, navigationFootProbeSideInset);
        navigationFootProbeExtraRadius = Mathf.Max(0f, navigationFootProbeExtraRadius);

        stuckCheckInterval = Mathf.Max(0.1f, stuckCheckInterval);
        stuckDistanceThreshold = Mathf.Max(0.01f, stuckDistanceThreshold);
        maxStuckRecoveries = Mathf.Max(1, maxStuckRecoveries);

        if (!Application.isPlaying)
        {
            CacheComponents();

            if (applyProfileOnAwake)
            {
                ApplyProfile();
            }
        }
    }

    public void SetNavGrid(NavGrid2D navigationGrid)
    {
        navGrid = navigationGrid;
        warnedMissingNavGrid = false;
    }

    public void SetNavGridBoundsEnforcement(bool enabled)
    {
        enforceNavGridBounds = enabled;
    }

    public void SetTraversalSoftPassBlockers(Collider2D[] colliders, bool enabled = true)
    {
        if (isTraversalSoftPassActive)
        {
            ApplyTraversalSoftPass(traversalSoftPassBlockers, false);
            isTraversalSoftPassActive = false;
        }

        allowTraversalOverridePhysicsSoftPass = enabled;
        traversalSoftPassBlockers = NavigationTraversalCore.NormalizeColliderArray(colliders);
        UpdateTraversalSoftPassState(GetNavigationCenter());
    }

    public bool CanOccupyPosition(Vector2 worldCenter)
    {
        return CanOccupyNavigationPoint(worldCenter);
    }

    public void GetNavigationProbePoints(
        Vector2 worldCenter,
        out Vector2 centerProbe,
        out Vector2 leftProbe,
        out Vector2 rightProbe)
    {
        NavigationTraversalCore.ProbePoints probePoints = NavigationTraversalCore.GetNavigationProbePoints(GetTraversalContract(), worldCenter);
        centerProbe = probePoints.Center;
        leftProbe = probePoints.Left;
        rightProbe = probePoints.Right;
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.RuntimeOnly,
        Guidance = "底层 autonomous roam 入口。外部协作线程应优先使用 ResumeAutonomousRoam 或 story facade，而不是自己拼 lifecycle。")]
    public void StartRoam()
    {
        if (IsResidentScriptedControlActive)
        {
            resumeRoamAfterResidentScriptedControl = true;
            return;
        }

        CacheComponents();
        EnsureRuntimeHomeAnchorBound();

        RefreshHomePositionFromCurrentContext();
        ResetSharedAvoidanceDebugState();
        ResetRoamInterruptionState();
        warnedMissingNavGrid = false;
        completedShortPauseCount = 0;
        lastAmbientDecision = string.Empty;
        ResetLongPauseTriggerCount();
        ResetMovementRecovery(GetNavigationCenter(), resetCounter: true);
        EnterShortPause(false);
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "外部 autonomous owner 恢复 resident roam 时，应通过这个语义口，而不是直接拼 StartRoam/RestartRoam。")]
    public void ResumeAutonomousRoam(bool tryImmediateMove = false)
    {
        if (IsResidentScriptedControlActive || !enabled || !isActiveAndEnabled)
        {
            return;
        }

        if (!IsRoaming)
        {
            StartRoam();
            return;
        }

        RestartRoamFromCurrentContext(tryImmediateMove);
    }

    private void RestartRoamFromCurrentContext(bool tryImmediateMove = false)
    {
        StartRoam();
        if (!tryImmediateMove
            || IsResidentScriptedControlActive
            || state != RoamState.ShortPause)
        {
            return;
        }

        if (ShouldDelayImmediateRoamRestartAfterFormalNavigation())
        {
            ConsumeFormalNavigationRoamSettle();
            return;
        }

        stateTimer = 0f;
        if (!TryBeginMove())
        {
            EnterShortPause(false);
        }
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.RuntimeOnly,
        Guidance = "低级停 roam 写口。debug/runtime 工具可以用，外部协作线程不要再拿它和其他内部原语自由组合。")]
    public void StopRoam()
    {
        resumeRoamAfterResidentScriptedControl = false;
        ClearMoveDecisionCache();
        BreakAmbientChatLink(hideBubble: true);
        debugMoveActive = false;
        activePointToPointTravelContract = PointToPointTravelContract.None;
        residentScriptedMovePaused = false;
        ClearRequestedDestination();
        ResetSharedAvoidanceDebugState();
        ResetRoamInterruptionState();
        ClearTraversalSoftPassState();
        state = RoamState.Inactive;
        stateTimer = 0f;
        currentPathIndex = 0;
        path.Clear();
        ResetMovementRecovery(GetNavigationCenter(), resetCounter: true);

        if (motionController != null)
        {
            motionController.StopMotion();
        }

        if (bubblePresenter != null)
        {
            bubblePresenter.HideBubble();
        }
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "Day1 等剧情 owner 取得 resident 身体控制权时，应从这里进入，不再直接调用 resident scripted low-level 写口。")]
    public void AcquireStoryControl(string ownerKey, bool resumeAutonomousRoamWhenReleased = true)
    {
        AcquireResidentScriptedControl(ownerKey, resumeAutonomousRoamWhenReleased);
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "Day1 等剧情 owner 释放 resident 身体控制权时，应从这里退出，不再自己拼 release + roam restart。")]
    public void ReleaseStoryControl(string ownerKey, bool resumeAutonomousRoam = true)
    {
        ReleaseResidentScriptedControl(ownerKey, resumeAutonomousRoam);
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "剧情/导演要求 resident 受控走位时应走这个语义口，不再直接组合 Acquire + Drive。")]
    public bool RequestStageTravel(
        string ownerKey,
        Vector2 stageStart,
        Vector2 stageDestination,
        float retargetTolerance = 0.35f,
        bool resumeAutonomousRoamWhenReleased = true)
    {
        return DriveResidentScriptedMoveTo(
            ownerKey,
            stageDestination,
            PointToPointTravelContract.ResidentScripted,
            resumeAutonomousRoamWhenReleased,
            retargetTolerance);
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "剧情 owner 把 resident 交还 home/anchor 时应走这里；它会同时更新 anchor 绑定并走统一的 return-home contract。")]
    public bool RequestReturnToAnchor(
        string ownerKey,
        Transform anchor,
        bool resumeAutonomousRoamWhenReleased = true,
        float retargetTolerance = 0.35f)
    {
        if (anchor == null)
        {
            return false;
        }

        BindResidentHomeAnchor(anchor);
        return RequestReturnHome(
            ownerKey,
            anchor.position,
            resumeAutonomousRoamWhenReleased,
            retargetTolerance);
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "剧情 owner 让 resident 回家/回锚点时应走这里，而不是直接 DebugMoveTo/DriveResidentScriptedMoveTo。")]
    public bool RequestReturnHome(
        string ownerKey,
        Vector2 homeDestination,
        bool resumeAutonomousRoamWhenReleased = true,
        float retargetTolerance = 0.35f)
    {
        homePosition = homeDestination;
        return DriveResidentScriptedMoveTo(
            ownerKey,
            homeDestination,
            PointToPointTravelContract.FormalNavigation,
            resumeAutonomousRoamWhenReleased,
            retargetTolerance);
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "剧情/导演想让 resident 立刻停住并保持 story owner 控制时，应走这个语义口。")]
    public void HaltStoryControlMotion(bool preserveResumeAutonomousRoamWhenReleased = true)
    {
        HaltResidentScriptedMovement(preserveResumeAutonomousRoamWhenReleased);
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "剧情/导演想让 resident 暂停受控移动时，应走这个语义口。")]
    public void PauseStoryControlMotion()
    {
        PauseResidentScriptedMovement();
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "剧情/导演恢复 resident 的受控移动时，应走这个语义口。")]
    public void ResumeStoryControlMotion()
    {
        ResumeResidentScriptedMovement();
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "剧情/导演要瞬时摆正 resident 到目标点时，应走这个语义口，而不是自己拼 Stop/Halt/Transform 写入。")]
    public void SnapToTarget(Vector2 worldPosition, Vector2? facingDirection = null)
    {
        CacheComponents();
        EnsureRuntimeHomeAnchorBound();

        ClearMoveDecisionCache();
        BreakAmbientChatLink(hideBubble: true);
        debugMoveActive = false;
        activePointToPointTravelContract = PointToPointTravelContract.None;
        residentScriptedMovePaused = false;
        ClearRequestedDestination();
        ResetSharedAvoidanceDebugState();
        ResetRoamInterruptionState();
        ClearTraversalSoftPassState();
        state = RoamState.Inactive;
        stateTimer = 0f;
        currentPathIndex = 0;
        path.Clear();
        ResetMovementRecovery(worldPosition, resetCounter: true);

        Vector3 snappedPosition = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
        transform.position = snappedPosition;
        if (rb != null)
        {
            rb.position = worldPosition;
            rb.linearVelocity = Vector2.zero;
        }

        if (motionController != null)
        {
            motionController.StopMotion();
            if (facingDirection.HasValue)
            {
                motionController.ApplyIdleFacing(facingDirection.Value);
            }
        }

        if (bubblePresenter != null)
        {
            bubblePresenter.HideBubble();
        }
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "Navigation owner 让 resident 进入一次性 autonomous directed travel 时，应走这个语义口。")]
    public bool BeginAutonomousTravel(Vector2 destination)
    {
        if (IsResidentScriptedControlActive)
        {
            return false;
        }

        return BeginPathDirectedMove(destination, PointToPointTravelContract.AutonomousDirected);
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "Navigation owner 让 resident 走正式 return-home/return-anchor 路径时，应走这个语义口。")]
    public bool BeginReturnHome(Vector2 destination)
    {
        if (IsResidentScriptedControlActive)
        {
            return false;
        }

        homePosition = destination;
        return BeginPathDirectedMove(destination, PointToPointTravelContract.FormalNavigation);
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "Navigation owner 发现当前 autonomous 路径该放弃并重新取样时，应走这个语义口。")]
    public void AbortAndReplan(bool tryImmediateMove = false)
    {
        if (IsResidentScriptedControlActive)
        {
            return;
        }

        debugMoveActive = false;
        activePointToPointTravelContract = PointToPointTravelContract.None;
        residentScriptedMovePaused = false;
        ResetRoamInterruptionState();

        if (!enabled || !isActiveAndEnabled)
        {
            StopRoam();
            return;
        }

        StartRoam();
        if (tryImmediateMove && state == RoamState.ShortPause)
        {
            stateTimer = 0f;
            if (!TryBeginMove())
            {
                EnterShortPause(false);
            }
        }
    }

    private void HaltResidentScriptedMovement(bool preserveResumeRoamWhenReleased = true)
    {
        CacheComponents();

        if (!preserveResumeRoamWhenReleased)
        {
            resumeRoamAfterResidentScriptedControl = false;
        }

        ClearMoveDecisionCache();
        BreakAmbientChatLink(hideBubble: true);
        debugMoveActive = false;
        activePointToPointTravelContract = PointToPointTravelContract.None;
        residentScriptedMovePaused = false;
        ClearRequestedDestination();
        ResetSharedAvoidanceDebugState();
        ResetRoamInterruptionState();
        ClearTraversalSoftPassState();
        state = RoamState.Inactive;
        stateTimer = 0f;
        currentPathIndex = 0;
        path.Clear();
        ResetMovementRecovery(GetNavigationCenter(), resetCounter: true);

        if (motionController != null)
        {
            motionController.StopMotion();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (bubblePresenter != null)
        {
            bubblePresenter.HideBubble();
        }
    }

    private void AcquireResidentScriptedControl(string ownerKey, bool resumeRoamWhenReleased = true)
    {
        CacheComponents();
        EnsureRuntimeHomeAnchorBound();

        string normalizedOwnerKey = NormalizeResidentScriptedControlOwner(ownerKey);
        if (residentScriptedControlOwners.Count > 0 &&
            string.Equals(residentScriptedControlOwners[residentScriptedControlOwners.Count - 1], normalizedOwnerKey, System.StringComparison.Ordinal))
        {
            if (resumeRoamWhenReleased)
            {
                resumeRoamAfterResidentScriptedControl = true;
            }

            residentScriptedControlOwnerKey = normalizedOwnerKey;
            return;
        }

        bool ownerAlreadyRegistered = residentScriptedControlOwners.Remove(normalizedOwnerKey);
        if (residentScriptedControlOwners.Count == 0)
        {
            resumeRoamAfterResidentScriptedControl = resumeRoamWhenReleased && state != RoamState.Inactive;
            ApplyResidentRuntimeFreeze();
            debugMoveActive = false;
            activePointToPointTravelContract = PointToPointTravelContract.None;
            residentScriptedMovePaused = false;
            ClearRequestedDestination();
            state = RoamState.Inactive;
            stateTimer = 0f;
            currentPathIndex = 0;
            path.Clear();
            ResetMovementRecovery(GetNavigationCenter(), resetCounter: true);
        }
        else if (resumeRoamWhenReleased)
        {
            resumeRoamAfterResidentScriptedControl = true;
        }

        if (ownerAlreadyRegistered && showDebugLog)
        {
            Debug.Log($"<color=green>[NPCAutoRoamController]</color> {name} Resident scripted control refreshed => Owner={normalizedOwnerKey}", this);
        }

        residentScriptedControlOwners.Add(normalizedOwnerKey);
        residentScriptedControlOwnerKey = normalizedOwnerKey;
    }

    private bool DriveResidentScriptedMoveTo(
        string ownerKey,
        Vector2 destination,
        PointToPointTravelContract travelContract,
        bool resumeRoamWhenReleased = true,
        float retargetTolerance = 0.35f)
    {
        CacheComponents();
        EnsureRuntimeHomeAnchorBound();

        AcquireResidentScriptedControl(ownerKey, resumeRoamWhenReleased);

        Vector2 normalizedDestination = NormalizeDestinationToNavGridBounds(destination);
        float effectiveTolerance = Mathf.Max(0.05f, retargetTolerance);
        if (debugMoveActive &&
            state == RoamState.Moving &&
            Vector2.Distance(GetRebuildRequestedDestination(), normalizedDestination) <= effectiveTolerance)
        {
            residentScriptedMovePaused = false;
            return true;
        }

        residentScriptedMovePaused = false;
        return BeginPathDirectedMove(normalizedDestination, travelContract);
    }

    private void PauseResidentScriptedMovement()
    {
        CacheComponents();

        if (!IsResidentScriptedControlActive || !debugMoveActive || state != RoamState.Moving)
        {
            return;
        }

        residentScriptedMovePaused = true;
        ApplyResidentRuntimeFreeze();
    }

    private void ResumeResidentScriptedMovement()
    {
        if (!IsResidentScriptedControlActive || !debugMoveActive || state != RoamState.Moving)
        {
            return;
        }

        residentScriptedMovePaused = false;
    }

    private void ReleaseResidentScriptedControl(string ownerKey, bool resumeResidentLogic = true)
    {
        if (residentScriptedControlOwners.Count == 0)
        {
            return;
        }

        string normalizedOwnerKey = NormalizeResidentScriptedControlOwner(ownerKey);
        if (string.IsNullOrWhiteSpace(normalizedOwnerKey) ||
            !residentScriptedControlOwners.Remove(normalizedOwnerKey))
        {
            residentScriptedControlOwners.RemoveAt(residentScriptedControlOwners.Count - 1);
        }

        residentScriptedControlOwnerKey = residentScriptedControlOwners.Count > 0
            ? residentScriptedControlOwners[residentScriptedControlOwners.Count - 1]
            : string.Empty;

        if (residentScriptedControlOwners.Count > 0)
        {
            return;
        }

        if (!resumeResidentLogic)
        {
            resumeRoamAfterResidentScriptedControl = false;
            residentScriptedMovePaused = false;
            ApplyResidentRuntimeFreeze();
            return;
        }

        if (resumeRoamAfterResidentScriptedControl &&
            !IsResidentScriptedControlActive &&
            !ShouldSuspendForDialogue() &&
            enabled &&
            isActiveAndEnabled)
        {
            resumeRoamAfterResidentScriptedControl = false;
            StartRoam();
        }
    }

    private void ClearResidentScriptedControl(bool resumeResidentLogic = false)
    {
        residentScriptedControlOwners.Clear();
        residentScriptedControlOwnerKey = string.Empty;
        if (!resumeResidentLogic)
        {
            resumeRoamAfterResidentScriptedControl = false;
            residentScriptedMovePaused = false;
            ApplyResidentRuntimeFreeze();
            return;
        }

        if (resumeRoamAfterResidentScriptedControl &&
            !IsResidentScriptedControlActive &&
            !ShouldSuspendForDialogue() &&
            enabled &&
            isActiveAndEnabled)
        {
            resumeRoamAfterResidentScriptedControl = false;
            StartRoam();
        }
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.RuntimeOnly,
        Guidance = "scene continuity / persistence runtime 使用的 resident 快照口，不是给剧情线程拼 locomotion lifecycle 用的。")]
    public NpcResidentRuntimeSnapshot CaptureResidentRuntimeSnapshot()
    {
        CacheComponents();
        EnsureRuntimeHomeAnchorBound();

        return new NpcResidentRuntimeSnapshot
        {
            stableKey = ResidentStableKey,
            sceneName = gameObject.scene.IsValid() ? gameObject.scene.name : string.Empty,
            residentGroupName = transform.parent != null ? transform.parent.name : string.Empty,
            residentGroupHierarchyPath = NpcResidentRuntimeContract.BuildHierarchyPath(transform.parent),
            homeAnchorName = homeAnchor != null ? homeAnchor.name : string.Empty,
            homeAnchorHierarchyPath = NpcResidentRuntimeContract.BuildHierarchyPath(homeAnchor),
            hasHomeAnchor = homeAnchor != null,
            homeAnchorSceneOwned = homeAnchor != null && homeAnchor.gameObject.scene.IsValid(),
            residentPosition = transform.position,
            homeAnchorPosition = homeAnchor != null ? (Vector2)homeAnchor.position : (Vector2)transform.position,
            wasRoaming = IsRoaming,
            scriptedControlActive = IsResidentScriptedControlActive,
            scriptedControlOwnerKey = ResidentScriptedControlOwnerKey,
            resumeRoamWhenReleased = resumeRoamAfterResidentScriptedControl
        };
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.RuntimeOnly,
        Guidance = "scene continuity / persistence runtime 使用的 resident 快照恢复口，不是给剧情线程拼 locomotion lifecycle 用的。")]
    public void ApplyResidentRuntimeSnapshot(NpcResidentRuntimeSnapshot snapshot, bool resumeResidentLogic = false)
    {
        if (snapshot == null)
        {
            return;
        }

        CacheComponents();
        Transform resolvedHomeAnchor = NpcResidentRuntimeContract.ResolveSceneTransform(
            gameObject.scene,
            snapshot.homeAnchorHierarchyPath,
            snapshot.homeAnchorName);

        if (resolvedHomeAnchor != null)
        {
            BindResidentHomeAnchor(resolvedHomeAnchor);
            if (snapshot.hasHomeAnchor)
            {
                resolvedHomeAnchor.position = snapshot.homeAnchorPosition;
            }
        }
        else if (snapshot.hasHomeAnchor)
        {
            homePosition = snapshot.homeAnchorPosition;
        }

        transform.position = snapshot.residentPosition;
        if (rb != null)
        {
            rb.position = snapshot.residentPosition;
            rb.linearVelocity = Vector2.zero;
        }

        if (motionController != null)
        {
            motionController.StopMotion();
        }

        residentScriptedControlOwners.Clear();
        residentScriptedControlOwnerKey = string.Empty;
        resumeRoamAfterResidentScriptedControl = snapshot.resumeRoamWhenReleased;

        if (snapshot.scriptedControlActive)
        {
            AcquireResidentScriptedControl(snapshot.scriptedControlOwnerKey, snapshot.resumeRoamWhenReleased);
            return;
        }

        bool shouldResumeRoam = snapshot.wasRoaming || snapshot.resumeRoamWhenReleased;
        if (resumeResidentLogic && shouldResumeRoam)
        {
            if (!IsResidentScriptedControlActive &&
                !ShouldSuspendForDialogue() &&
                enabled &&
                isActiveAndEnabled)
            {
                resumeRoamAfterResidentScriptedControl = false;
                StartRoam();
                return;
            }
        }

        ApplyResidentRuntimeFreeze();
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "runtime/baseline 配置如果确实要从 roamProfile 重灌，应走这个语义口，而不是外部直接碰低级 ApplyProfile。")]
    public void SyncRuntimeProfileFromAsset()
    {
        ApplyProfile();
    }

    private void ApplyProfile()
    {
        if (roamProfile == null)
        {
            return;
        }

        CacheComponents();

        activityRadius = roamProfile.ActivityRadius;
        minimumMoveDistance = roamProfile.MinimumMoveDistance;
        pathSampleAttempts = roamProfile.PathSampleAttempts;

        shortPauseMin = roamProfile.ShortPauseMin;
        shortPauseMax = roamProfile.ShortPauseMax;
        shortPauseCountMin = roamProfile.ShortPauseCountMin;
        shortPauseCountMax = roamProfile.ShortPauseCountMax;

        longPauseMin = roamProfile.LongPauseMin;
        longPauseMax = roamProfile.LongPauseMax;

        stuckCheckInterval = roamProfile.StuckCheckInterval;
        stuckDistanceThreshold = roamProfile.StuckDistanceThreshold;
        maxStuckRecoveries = roamProfile.MaxStuckRecoveries;

        enableAmbientChat = roamProfile.EnableAmbientChat;
        ambientChatRadius = roamProfile.AmbientChatRadius;
        ambientChatChance = roamProfile.AmbientChatChance;
        ambientChatResponseDelay = roamProfile.AmbientChatResponseDelay;

        chatInitiatorLines = CopyLines(roamProfile.ChatInitiatorLines);
        chatResponderLines = CopyLines(roamProfile.ChatResponderLines);

        if (motionController != null)
        {
            motionController.MoveSpeed = roamProfile.MoveSpeed;
        }

        NPCAnimController animController = GetComponent<NPCAnimController>();
        if (animController != null)
        {
            animController.SetAnimationSpeed(roamProfile.IdleAnimationSpeed, roamProfile.MoveAnimationSpeed);
        }

        if (bubblePresenter != null)
        {
            bubblePresenter.ApplyProfile(roamProfile);
        }
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "runtime/baseline 配置如果需要重绑 home anchor，应走这个语义口。剧情线程不要把它当成 locomotion lifecycle 拼装件。")]
    public void BindResidentHomeAnchor(Transform anchor)
    {
        SetHomeAnchor(anchor);
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "外部线程如果只是想在 NPC 静止时摆正朝向，应走这个语义口，不要直写 NPCMotionController 的低级 facing API。")]
    public void ApplyIdleFacing(Vector2 facing)
    {
        CacheComponents();
        if (motionController == null)
        {
            return;
        }

        motionController.ApplyIdleFacing(facing);
    }

    private void SetHomeAnchor(Transform anchor)
    {
        homeAnchor = anchor;
        homePosition = anchor != null ? anchor.position : transform.position;
    }

    private void EnsureRuntimeHomeAnchorBound()
    {
        if (!Application.isPlaying || homeAnchor != null)
        {
            return;
        }

        Transform resolvedAnchor = FindRuntimeHomeAnchorCandidate();
        if (resolvedAnchor == null)
        {
            resolvedAnchor = CreateRuntimeHomeAnchor();
        }

        if (resolvedAnchor != null)
        {
            SetHomeAnchor(resolvedAnchor);
        }
    }

    private Transform FindRuntimeHomeAnchorCandidate()
    {
        string anchorName = $"{name}_HomeAnchor";
        Vector2 currentPosition = GetNavigationCenter();
        Transform parent = transform.parent;
        if (parent != null)
        {
            Transform siblingAnchor = parent.Find(anchorName);
            if (IsPlausibleRuntimeHomeAnchorCandidate(siblingAnchor, currentPosition))
            {
                return siblingAnchor;
            }
        }

        Transform childAnchor = transform.Find(anchorName);
        if (IsPlausibleRuntimeHomeAnchorCandidate(childAnchor, currentPosition))
        {
            return childAnchor;
        }

        if (!gameObject.scene.IsValid())
        {
            return null;
        }

        GameObject[] roots = gameObject.scene.GetRootGameObjects();
        for (int index = 0; index < roots.Length; index++)
        {
            Transform sceneAnchor = FindNamedChildRecursive(roots[index].transform, anchorName);
            if (IsPlausibleRuntimeHomeAnchorCandidate(sceneAnchor, currentPosition))
            {
                return sceneAnchor;
            }
        }

        return null;
    }

    private Transform CreateRuntimeHomeAnchor()
    {
        GameObject anchorObject = new GameObject($"{name}_HomeAnchor");
        Transform anchorParent = transform.parent != null ? transform.parent : transform;
        anchorObject.transform.SetParent(anchorParent, worldPositionStays: true);
        anchorObject.transform.position = transform.position;
        anchorObject.transform.rotation = Quaternion.identity;
        return anchorObject.transform;
    }

    private static Transform FindNamedChildRecursive(Transform root, string targetName)
    {
        if (root == null)
        {
            return null;
        }

        if (string.Equals(root.name, targetName, System.StringComparison.Ordinal))
        {
            return root;
        }

        for (int index = 0; index < root.childCount; index++)
        {
            Transform childMatch = FindNamedChildRecursive(root.GetChild(index), targetName);
            if (childMatch != null)
            {
                return childMatch;
            }
        }

        return null;
    }

    private bool IsPlausibleRuntimeHomeAnchorCandidate(Transform candidate, Vector2 currentPosition)
    {
        return candidate != null && !ShouldPreferCurrentPositionAsRoamCenter(currentPosition, candidate.position);
    }

    private void RefreshHomePositionFromCurrentContext()
    {
        Vector2 currentPosition = GetNavigationCenter();
        if (homeAnchor == null)
        {
            homePosition = currentPosition;
            return;
        }

        Vector2 anchorPosition = homeAnchor.position;
        bool preferCurrentPosition = ShouldPreferCurrentPositionAsRoamCenter(currentPosition, anchorPosition);
        Vector2 preferredCenter = preferCurrentPosition ? currentPosition : anchorPosition;
        if (TryResolvePreferredRoamCenter(preferredCenter, preferCurrentPosition ? anchorPosition : currentPosition, out Vector2 resolvedRoamCenter))
        {
            homePosition = resolvedRoamCenter;
            return;
        }

        homePosition = preferCurrentPosition
            ? currentPosition
            : anchorPosition;
    }

    private static bool IsRecoverablePointToPointTravelContract(PointToPointTravelContract travelContract)
    {
        return travelContract == PointToPointTravelContract.AutonomousDirected ||
               travelContract == PointToPointTravelContract.ResidentScripted ||
               travelContract == PointToPointTravelContract.FormalNavigation;
    }

    private static bool RequiresReleasedMovementBodyClearance(PointToPointTravelContract travelContract)
    {
        return IsRecoverablePointToPointTravelContract(travelContract);
    }

    private bool HasActivePointToPointTravelContract()
    {
        return debugMoveActive && activePointToPointTravelContract != PointToPointTravelContract.None;
    }

    private bool IsActivePointToPointTravelContract(PointToPointTravelContract travelContract)
    {
        return debugMoveActive && activePointToPointTravelContract == travelContract;
    }

    private bool IsRecoverablePointToPointTravelActive()
    {
        return debugMoveActive && IsRecoverablePointToPointTravelContract(activePointToPointTravelContract);
    }

    private bool UsesAutonomousRoamExecutionContract()
    {
        return !HasActivePointToPointTravelContract() && !IsResidentScriptedControlActive;
    }

    private bool IsFormalNavigationDebugMoveActive()
    {
        return IsActivePointToPointTravelContract(PointToPointTravelContract.FormalNavigation);
    }

    private bool IsPlainDebugMoveActive()
    {
        return IsActivePointToPointTravelContract(PointToPointTravelContract.PlainDebug);
    }

    private string GetPointToPointTravelContractLabel(PointToPointTravelContract travelContract)
    {
        return $"{GetPointToPointTravelContractName(travelContract)}Move";
    }

    private string GetPointToPointTravelContractName(PointToPointTravelContract travelContract)
    {
        return travelContract switch
        {
            PointToPointTravelContract.PlainDebug => "PlainDebug",
            PointToPointTravelContract.AutonomousDirected => "AutonomousDirected",
            PointToPointTravelContract.ResidentScripted => "ResidentScripted",
            PointToPointTravelContract.FormalNavigation => "FormalNavigation",
            _ => "PointToPoint"
        };
    }

    private bool ShouldUseReleasedMovementBodyClearanceExecutionContract()
    {
        return UsesAutonomousRoamExecutionContract() || IsRecoverablePointToPointTravelActive();
    }

    private float GetReleasedMovementStepBodyClearanceExtraRadius()
    {
        return Mathf.Max(
            GetAutonomousRoamBodyClearanceExtraRadius(),
            GetContactShellPadding() * 0.85f);
    }

    private float GetReleasedMovementDestinationCorrectionLimit()
    {
        float correctionLimit = Mathf.Max(
            GetReleasedMovementStepBodyClearanceExtraRadius() * 2.25f,
            GetColliderRadius() + GetContactShellPadding(),
            minimumMoveDistance * 0.55f);
        return Mathf.Clamp(
            correctionLimit,
            RELEASED_MOVEMENT_DESTINATION_CORRECTION_MIN,
            RELEASED_MOVEMENT_DESTINATION_CORRECTION_MAX);
    }

    private float GetReleasedMovementNeighborClearanceRadius()
    {
        float desiredRadius = Mathf.Max(
            GetColliderRadius() * RELEASED_MOVEMENT_NEIGHBOR_CLEARANCE_RADIUS_SCALE,
            GetReleasedMovementStepBodyClearanceExtraRadius() * 1.2f,
            minimumMoveDistance * 0.32f);
        return Mathf.Clamp(
            desiredRadius,
            RELEASED_MOVEMENT_NEIGHBOR_CLEARANCE_MIN_RADIUS,
            RELEASED_MOVEMENT_NEIGHBOR_CLEARANCE_MAX_RADIUS);
    }

    private bool ShouldDelayImmediateRoamRestartAfterFormalNavigation()
    {
        if (lastFormalNavigationArrivalTime <= float.NegativeInfinity)
        {
            return false;
        }

        float elapsed = Time.time - lastFormalNavigationArrivalTime;
        if (elapsed < 0f || elapsed > FORMAL_NAVIGATION_RESTART_SETTLE_SECONDS)
        {
            return false;
        }

        return Vector2.Distance(GetNavigationCenter(), lastFormalNavigationArrivalPosition) <= FORMAL_NAVIGATION_RESTART_SETTLE_RADIUS;
    }

    private void ConsumeFormalNavigationRoamSettle()
    {
        lastFormalNavigationArrivalTime = float.NegativeInfinity;
    }

    private bool TryResolvePreferredRoamCenter(
        Vector2 primaryCenter,
        Vector2 secondaryCenter,
        out Vector2 resolvedRoamCenter)
    {
        if (TryResolveSafeRoamCenterNear(primaryCenter, out resolvedRoamCenter))
        {
            return true;
        }

        if (TryResolveSafeRoamCenterNear(secondaryCenter, out resolvedRoamCenter))
        {
            return true;
        }

        resolvedRoamCenter = primaryCenter;
        return false;
    }

    private bool TryResolveSafeRoamCenterNear(Vector2 preferredCenter, out Vector2 resolvedRoamCenter)
    {
        Vector2 clampedCenter = NormalizeDestinationToNavGridBounds(preferredCenter);
        if (IsAutonomousRoamCenterCandidateClear(clampedCenter))
        {
            resolvedRoamCenter = clampedCenter;
            return true;
        }

        if (TryResolveOccupiableDestination(clampedCenter, out Vector2 correctedCenter) &&
            Vector2.Distance(preferredCenter, correctedCenter) <= GetAutonomousRoamSafeCenterSearchRadius() + GetAutonomousRoamDestinationCorrectionLimit() &&
            IsAutonomousRoamCenterCandidateClear(correctedCenter))
        {
            resolvedRoamCenter = correctedCenter;
            return true;
        }

        float searchRadius = GetAutonomousRoamSafeCenterSearchRadius();
        Vector2[] searchDirections =
        {
            Vector2.right,
            Vector2.left,
            Vector2.up,
            Vector2.down,
            new Vector2(1f, 1f).normalized,
            new Vector2(-1f, 1f).normalized,
            new Vector2(1f, -1f).normalized,
            new Vector2(-1f, -1f).normalized
        };

        for (int ringIndex = 1; ringIndex <= AUTONOMOUS_ROAM_SAFE_CENTER_RING_COUNT; ringIndex++)
        {
            float ringDistance = searchRadius * (ringIndex / (float)AUTONOMOUS_ROAM_SAFE_CENTER_RING_COUNT);
            for (int directionIndex = 0; directionIndex < searchDirections.Length; directionIndex++)
            {
                Vector2 candidate = NormalizeDestinationToNavGridBounds(preferredCenter + searchDirections[directionIndex] * ringDistance);
                if (!TryResolveOccupiableDestination(candidate, out Vector2 correctedCandidate))
                {
                    continue;
                }

                if (Vector2.Distance(preferredCenter, correctedCandidate) > searchRadius + GetAutonomousRoamDestinationCorrectionLimit())
                {
                    continue;
                }

                if (IsAutonomousRoamCenterCandidateClear(correctedCandidate))
                {
                    resolvedRoamCenter = correctedCandidate;
                    return true;
                }
            }
        }

        resolvedRoamCenter = clampedCenter;
        return false;
    }

    private float GetAutonomousRoamSafeCenterSearchRadius()
    {
        float desiredRadius = Mathf.Max(
            GetColliderRadius() + GetAutonomousRoamBodyClearanceExtraRadius(),
            activityRadius * AUTONOMOUS_ROAM_SAFE_CENTER_SEARCH_RADIUS_SCALE,
            minimumMoveDistance * 0.75f);
        return Mathf.Clamp(
            desiredRadius,
            AUTONOMOUS_ROAM_SAFE_CENTER_MIN_SEARCH_RADIUS,
            AUTONOMOUS_ROAM_SAFE_CENTER_MAX_SEARCH_RADIUS);
    }

    private bool IsAutonomousRoamCenterCandidateClear(Vector2 candidatePosition)
    {
        return IsAutonomousRoamDestinationCandidateClear(candidatePosition);
    }

    private bool IsAutonomousRoamDestinationCandidateClear(Vector2 candidatePosition)
    {
        return CanOccupyNavigationPoint(candidatePosition) &&
            HasAutonomousRoamDestinationNeighborhoodClearance(candidatePosition) &&
            HasAutonomousRoamDestinationBodyClearance(candidatePosition) &&
            HasAutonomousRoamDestinationAgentClearance(candidatePosition) &&
            !IsRecentlyBlockedAutonomousDestination(candidatePosition);
    }

    private bool IsReleasedMovementDestinationCandidateClear(Vector2 candidatePosition)
    {
        return HasReleasedMovementDestinationBodyClearance(candidatePosition) &&
            HasReleasedMovementDestinationNeighborhoodClearance(candidatePosition);
    }

    private bool IsRecoverablePointToPointBuiltPathAcceptable(
        Vector2 currentPosition,
        Vector2 destination)
    {
        if (path.Count == 0 || !IsReleasedMovementDestinationCandidateClear(destination))
        {
            return false;
        }

        return HasPathBodyClearance(
            currentPosition,
            GetReleasedMovementStepBodyClearanceExtraRadius(),
            RELEASED_MOVEMENT_PATH_CLEARANCE_SAMPLE_SPACING,
            RELEASED_STEP_BODY_CLEARANCE_SIDE_INSET_SCALE,
            RELEASED_STEP_BODY_CLEARANCE_VERTICAL_INSET_SCALE);
    }

    private bool HasPathBodyClearance(
        Vector2 currentPosition,
        float extraRadius,
        float sampleSpacing,
        float sideInsetScale,
        float verticalInsetScale)
    {
        if (path.Count == 0)
        {
            return false;
        }

        Vector2 previousPoint = currentPosition;
        for (int index = 0; index < path.Count; index++)
        {
            if (!HasPathSegmentBodyClearance(
                    previousPoint,
                    path[index],
                    extraRadius,
                    sampleSpacing,
                    sideInsetScale,
                    verticalInsetScale))
            {
                return false;
            }

            previousPoint = path[index];
        }

        return true;
    }

    private bool HasPathSegmentBodyClearance(
        Vector2 start,
        Vector2 end,
        float extraRadius,
        float sampleSpacing,
        float sideInsetScale,
        float verticalInsetScale)
    {
        float segmentDistance = Vector2.Distance(start, end);
        if (segmentDistance <= 0.0001f)
        {
            return true;
        }

        int sampleCount = Mathf.Max(1, Mathf.CeilToInt(segmentDistance / Mathf.Max(0.05f, sampleSpacing)));
        for (int step = 1; step <= sampleCount; step++)
        {
            Vector2 samplePoint = Vector2.Lerp(start, end, step / (float)sampleCount);
            if (!NavigationTraversalCore.CanOccupyNavigationPointWithClearanceMargin(
                    GetTraversalContract(),
                    samplePoint,
                    extraRadius,
                    sideInsetScale,
                    verticalInsetScale))
            {
                return false;
            }
        }

        return true;
    }

    private bool ShouldPreferCurrentPositionAsRoamCenter(Vector2 currentPosition, Vector2 anchorPosition)
    {
        if (IsResidentScriptedControlActive)
        {
            return false;
        }

        float allowedAnchorDistance = Mathf.Max(
            HOME_ANCHOR_MISMATCH_MIN_DISTANCE,
            activityRadius + Mathf.Max(minimumMoveDistance, 0.5f) + GetColliderRadius() + HOME_ANCHOR_MISMATCH_PADDING);
        return Vector2.Distance(currentPosition, anchorPosition) > allowedAnchorDistance;
    }

    private static string NormalizeResidentScriptedControlOwner(string ownerKey)
    {
        string trimmed = ownerKey?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? "resident-scripted-control" : trimmed;
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.RuntimeOnly,
        Guidance = "runtime/debug 校准 home anchor 用口。不要把它当成剧情 lifecycle 拼装件。")]
    public void SyncHomeAnchorToCurrentPosition()
    {
        if (homeAnchor != null)
        {
            homeAnchor.position = transform.position;
        }

        homePosition = transform.position;
    }

    private void RefreshRoamCenterFromCurrentContext()
    {
        CacheComponents();
        EnsureRuntimeHomeAnchorBound();
        RefreshHomePositionFromCurrentContext();
    }

    [ContextMenu("调试/立即开始漫游")]
    public void DebugStartRoam()
    {
        if (Application.isPlaying)
        {
            StartRoam();
        }
    }

    [ContextMenu("调试/停止漫游")]
    public void DebugStopRoam()
    {
        if (Application.isPlaying)
        {
            StopRoam();
        }
    }

    [ContextMenu("调试/立即进入长停")]
    public void DebugEnterLongPause()
    {
        if (Application.isPlaying)
        {
            EnterLongPause();
        }
    }

    [ContextMenu("调试/尝试附近聊天")]
    public void DebugTryAmbientChat()
    {
        if (Application.isPlaying)
        {
            TryStartAmbientChat(Random.Range(longPauseMin, longPauseMax));
        }
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.DebugOnly,
        Guidance = "只给 probe / validation / 调试菜单使用。业务线程不要再拿它作为正式 locomotion contract。")]
    public bool DebugMoveTo(Vector2 destination)
    {
        return BeginPathDirectedMove(destination, PointToPointTravelContract.PlainDebug);
    }

    private bool BeginPathDirectedMove(Vector2 destination, PointToPointTravelContract travelContract)
    {
        CacheComponents();
        EnsureRuntimeHomeAnchorBound();
        if (!TryResolveNavGrid())
        {
            return false;
        }

        BreakAmbientChatLink(hideBubble: true);
        ClearAmbientChatRetry();
        if (!TryResolvePathDirectedDestination(destination, travelContract, out Vector2 resolvedDestination))
        {
            return false;
        }

        if (!TryAcquirePathBuildBudget())
        {
            return false;
        }

        RememberRequestedDestination(resolvedDestination);
        ResetRoamInterruptionState();

        Vector2 currentPosition = rb != null ? rb.position : (Vector2)transform.position;
        if (!TryBuildPathToDestination(
                currentPosition,
                resolvedDestination,
                GetPointToPointTravelContractLabel(travelContract),
                out _))
        {
            RecordPathBuildOutcome(success: false);
            return false;
        }

        if (!IsBuiltPathAcceptableForTravelContract(currentPosition, resolvedDestination, travelContract))
        {
            RecordPathBuildOutcome(success: false);
            NavigationPathExecutor2D.Clear(navigationExecution, clearDestination: false);
            return false;
        }

        RecordPathBuildOutcome(success: true);
        debugMoveActive = true;
        activePointToPointTravelContract = travelContract;
        residentScriptedMovePaused = false;
        state = RoamState.Moving;
        stateTimer = 0f;
        ResetSharedAvoidanceDebugState();
        ResetMovementRecovery(currentPosition, resetCounter: true);

        if (bubblePresenter != null)
        {
            bubblePresenter.HideBubble();
        }

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=green>[NPCAutoRoamController]</color> {name} PointToPointTravel => Contract={travelContract}, Destination={currentDestination}, PathCount={path.Count}",
                this);
        }

        return true;
    }

    private bool TryResolvePathDirectedDestination(
        Vector2 destination,
        PointToPointTravelContract travelContract,
        out Vector2 resolvedDestination)
    {
        if (RequiresReleasedMovementBodyClearance(travelContract))
        {
            return TryResolveReleasedMovementDestination(destination, out resolvedDestination);
        }

        return TryResolveOccupiableDestination(destination, out resolvedDestination);
    }

    private bool TryBuildPathToDestination(
        Vector2 startPosition,
        Vector2 destination,
        string reason,
        out NavigationPathExecutor2D.BuildPathResult buildResult)
    {
        buildResult = NavigationPathExecutor2D.TryRefreshPath(
            navigationExecution,
            navGrid,
            startPosition,
            destination,
            null,
            null,
            0f,
            GetPathBuildLogger(reason),
            navigationCollider);
        return buildResult.Success && path.Count > 0;
    }

    private void CacheComponents()
    {
        if (motionController == null)
        {
            motionController = GetComponent<NPCMotionController>();
        }

        if (bubblePresenter == null)
        {
            bubblePresenter = GetComponent<NPCBubblePresenter>();
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (navigationCollider == null)
        {
            navigationCollider = GetComponent<Collider2D>();
            if (navigationCollider == null)
            {
                navigationCollider = GetComponentInChildren<Collider2D>();
            }
        }

        if (navGrid == null)
        {
            navGrid = FindFirstObjectByType<NavGrid2D>();
        }

        if (staticObstacleProbeBuffer == null || staticObstacleProbeBuffer.Length < 12)
        {
            staticObstacleProbeBuffer = new Collider2D[12];
        }
    }

    private void TickShortPause()
    {
        if (debugMoveActive)
        {
            EndDebugMove(reachedDestination: false);
            return;
        }

        stateTimer -= Time.deltaTime;
        if (stateTimer > 0f)
        {
            return;
        }

        if (completedShortPauseCount >= longPauseTriggerCount)
        {
            EnterLongPause();
            return;
        }

        if (!TryBeginMove())
        {
            EnterShortPause(false);
        }
    }

    private void TickMoving(float deltaTime)
    {
        if (motionController == null)
        {
            ClearMoveDecisionCache();
            lastMoveSkipReason = "MissingMotionController";
            return;
        }

        Vector2 currentPosition = rb != null ? rb.position : (Vector2)transform.position;
        if (path.Count == 0)
        {
            ClearMoveDecisionCache();
            lastMoveSkipReason = "PathClearedWhileMoving";
            if (TryRecoverAutonomousRoamFailureBeforeInterrupt(currentPosition, "PathClearedWhileMoving"))
            {
                return;
            }

            if (TryInterruptRoamMove(
                    RoamMoveInterruptionReason.StuckRecoveryFailed,
                    currentPosition,
                    lastBlockingAgentId,
                    null,
                    RoamMoveInterruptionBlockerKind.None,
                    "PathClearedWhileMoving"))
            {
                return;
            }

            FinishMoveCycle(countTowardLongPause: false, reachedDestination: false);
            return;
        }

        if (CanReuseStoppedMoveDecisionThisFrame(currentPosition))
        {
            ApplyStoppedMoveDecision();
            lastMoveSkipReason = "ReusedFrameStopDecision";
            return;
        }

        UpdateTraversalSoftPassStateOncePerFrame(currentPosition);
        Vector2 avoidancePosition = GetNavigationCenter();

        if (TryHandlePendingMoveCommandNoProgress(currentPosition))
        {
            CacheStoppedMoveDecisionForCurrentFrame();
            lastMoveSkipReason = "MoveCommandNoProgress";
            return;
        }

        NavigationPathExecutor2D.WaypointResult waypointState = NavigationPathExecutor2D.EvaluateWaypoint(
            navigationExecution,
            currentPosition,
            waypointTolerance,
            destinationTolerance);

        if (waypointState.ClearedOverrideWaypoint)
        {
            if (TryReleaseSharedAvoidanceDetour(avoidancePosition, Time.time) &&
                state != RoamState.Moving)
            {
                return;
            }

            waypointState = NavigationPathExecutor2D.EvaluateWaypoint(
                navigationExecution,
                currentPosition,
                waypointTolerance,
                destinationTolerance);
        }

        if (waypointState.ReachedPathEnd ||
            Vector2.Distance(currentPosition, currentDestination) <= destinationTolerance)
        {
            ClearMoveDecisionCache();
            lastMoveSkipReason = "ReachedDestination";
            FinishMoveCycle(countTowardLongPause: true, reachedDestination: true);
            return;
        }

        if (CanReuseHeavyMoveDecisionThisFrame(waypointState, currentPosition, deltaTime))
        {
            ApplyCachedMoveVelocity(currentPosition, deltaTime);
            lastMoveSkipReason = "ReusedFrameMoveDecision";
            return;
        }

        if (CheckAndHandleStuck(currentPosition))
        {
            CacheStoppedMoveDecisionForCurrentFrame();
            lastMoveSkipReason = "Stuck";
            return;
        }

        if (!waypointState.HasWaypoint)
        {
            CacheStoppedMoveDecisionForCurrentFrame();
            lastMoveSkipReason = "MissingWaypoint";
            if (TryRecoverAutonomousRoamFailureBeforeInterrupt(currentPosition, "WaypointMissingWhileMoving"))
            {
                return;
            }

            if (TryInterruptRoamMove(
                    RoamMoveInterruptionReason.StuckRecoveryFailed,
                    currentPosition,
                    lastBlockingAgentId,
                    null,
                    RoamMoveInterruptionBlockerKind.None,
                    "WaypointMissingWhileMoving"))
            {
                return;
            }

            FinishMoveCycle(countTowardLongPause: false, reachedDestination: false);
            return;
        }

        Vector2 waypoint = waypointState.Waypoint;
        Vector2 toWaypoint = waypoint - currentPosition;
        float distance = waypointState.DistanceToWaypoint;
        float moveSpeed = Mathf.Max(0f, motionController.MoveSpeed);

        if (distance <= 0.0001f || moveSpeed <= 0f)
        {
            CacheStoppedMoveDecisionForCurrentFrame();
            lastMoveSkipReason = distance <= 0.0001f ? "ZeroDistanceToWaypoint" : "ZeroMoveSpeed";
            motionController.StopMotion();
            return;
        }

        Vector2 moveDirection = toWaypoint / distance;
        if (TryHandleSharedAvoidance(currentPosition, avoidancePosition, waypoint, moveDirection, out Vector2 adjustedDirection, out float moveScale))
        {
            CacheStoppedMoveDecisionForCurrentFrame();
            lastMoveSkipReason = "SharedAvoidance";
            return;
        }

        if (sharedAvoidanceBlockingFrames <= 0 && !hasDynamicDetour)
        {
            adjustedDirection = StabilizeMoveDirection(currentPosition, adjustedDirection);
        }
        float step = moveSpeed * Mathf.Clamp01(moveScale) * Mathf.Max(deltaTime, 0.0001f);
        if (step <= 0.0001f)
        {
            CacheStoppedMoveDecisionForCurrentFrame();
            lastMoveSkipReason = "ZeroStep";
            LogBlockedMovementDiagnostics("ZeroStep", currentPosition, waypoint, currentPosition);
            TryHandleBlockedAdvance(currentPosition, "ZeroStep");
            return;
        }

        if (ShouldApplyAutonomousStaticObstacleSteering())
        {
            adjustedDirection = AdjustDirectionByStaticColliders(currentPosition, adjustedDirection, step);
        }

        Vector2 nextPosition = distance <= step
            ? waypoint
            : currentPosition + adjustedDirection * step;
        nextPosition = ConstrainNextPositionToNavigationBounds(currentPosition, nextPosition);

        float safeDeltaTime = Mathf.Max(deltaTime, 0.0001f);
        Vector2 velocity = (nextPosition - currentPosition) / safeDeltaTime;
        if (velocity.sqrMagnitude <= 0.0001f)
        {
            CacheStoppedMoveDecisionForCurrentFrame();
            lastMoveSkipReason = "ConstrainedZeroAdvance";
            LogBlockedMovementDiagnostics("ConstrainedZeroAdvance", currentPosition, waypoint, nextPosition);
            TryHandleBlockedAdvance(currentPosition, "ConstrainedZeroAdvance");
            return;
        }

        if (ShouldTreatMoveCommandAsOscillation(currentPosition, velocity))
        {
            CacheStoppedMoveDecisionForCurrentFrame();
            lastMoveSkipReason = "MoveCommandOscillation";
            LogBlockedMovementDiagnostics("MoveCommandOscillation", currentPosition, waypoint, nextPosition);
            TryHandleBlockedAdvance(currentPosition, "MoveCommandOscillation");
            return;
        }

        Vector2 facingDirection = ResolveFacingDirectionForMoveCommand(moveDirection, adjustedDirection);
        lastMoveSkipReason = "IssuingVelocity";
        MarkMoveCommandIssued(currentPosition, nextPosition);
        ApplyMoveVelocity(currentPosition, velocity, facingDirection, deltaTime);
        CacheMoveDecisionVelocityForCurrentFrame(velocity, facingDirection);
    }

    private void TickLongPause()
    {
        if (debugMoveActive)
        {
            EndDebugMove(reachedDestination: false);
            return;
        }

        TryExecutePendingAmbientChatRetry();

        stateTimer -= Time.deltaTime;
        if (stateTimer > 0f)
        {
            return;
        }

        BreakAmbientChatLink(hideBubble: true);
        completedShortPauseCount = 0;
        ResetLongPauseTriggerCount();

        if (bubblePresenter != null)
        {
            bubblePresenter.HideBubble();
        }

        if (!TryBeginMove())
        {
            EnterShortPause(false);
        }
    }

    private bool TryBeginMove(bool preserveBlockedRecoveryState = false)
    {
        if (!TryResolveNavGrid())
        {
            return false;
        }

        if (!TryAcquirePathBuildBudget())
        {
            return false;
        }

        BlockedRecoveryState blockedRecoveryState = preserveBlockedRecoveryState
            ? CaptureBlockedRecoveryState()
            : default;

        debugMoveActive = false;
        activePointToPointTravelContract = PointToPointTravelContract.None;
        NavigationPathExecutor2D.Clear(navigationExecution, clearDestination: false);

        Vector2 currentPosition = GetNavigationCenter();
        Vector2 roamCenter = GetRoamCenter();

        bool requireAutonomousRoamAcceptance = UsesAutonomousRoamExecutionContract();
        Vector2 samplingCenter = requireAutonomousRoamAcceptance
            ? ResolveAutonomousRoamSamplingCenter(currentPosition, roamCenter)
            : roamCenter;
        int sampleAttemptBudget = GetAutonomousRoamSampleAttemptBudget();
        if (requireAutonomousRoamAcceptance)
        {
            autonomousRoamCandidates.Clear();
            EnsureRegisteredRoamControllerBufferCurrentFrame();
        }

        bool TryCommitResolvedDestination(Vector2 walkableDestination)
        {
            if (Vector2.Distance(currentPosition, walkableDestination) < minimumMoveDistance)
            {
                return false;
            }

            NavigationPathExecutor2D.BuildPathResult buildResult = NavigationPathExecutor2D.TryRefreshPath(
                navigationExecution,
                navGrid,
                currentPosition,
                walkableDestination,
                null,
                null,
                0f,
                GetPathBuildLogger("TryBeginMove"),
                navigationCollider);

            if (!buildResult.Success || path.Count == 0)
            {
                return false;
            }

            if (requireAutonomousRoamAcceptance &&
                !IsAutonomousRoamBuiltPathAcceptable(currentPosition, roamCenter, walkableDestination))
            {
                NavigationPathExecutor2D.Clear(navigationExecution, clearDestination: false);
                return false;
            }

            RecordPathBuildOutcome(success: true);
            RememberRequestedDestination(walkableDestination);
            ResetRoamInterruptionState();
            state = RoamState.Moving;
            stateTimer = 0f;
            ResetMovementRecovery(currentPosition, resetCounter: true);
            if (preserveBlockedRecoveryState)
            {
                RestoreBlockedRecoveryState(blockedRecoveryState);
            }

            if (bubblePresenter != null)
            {
                bubblePresenter.HideBubble();
            }

            if (showDebugLog)
            {
                Debug.Log(
                    $"<color=green>[NPCAutoRoamController]</color> {name} 开始移动 => Destination={currentDestination}, PathCount={path.Count}",
                    this);
            }

            return true;
        }

        bool TryCollectAutonomousRoamCandidate(Vector2 sampledDestination)
        {
            if (!IsWithinAutonomousRoamBounds(roamCenter, sampledDestination) ||
                Vector2.Distance(currentPosition, sampledDestination) < minimumMoveDistance)
            {
                return false;
            }

            if (!TryResolveAutonomousRoamDestination(
                    currentPosition,
                    roamCenter,
                    sampledDestination,
                    out Vector2 walkableDestination,
                    out float candidateScore))
            {
                return false;
            }

            InsertAutonomousRoamCandidate(walkableDestination, candidateScore);
            return true;
        }

        bool TryCommitSampledDestination(Vector2 sampledDestination)
        {
            if (requireAutonomousRoamAcceptance)
            {
                return TryCollectAutonomousRoamCandidate(sampledDestination);
            }

            if (Vector2.Distance(currentPosition, sampledDestination) < minimumMoveDistance)
            {
                return false;
            }

            if (!TryResolveOccupiableDestination(sampledDestination, out Vector2 walkableDestination))
            {
                return false;
            }

            return TryCommitResolvedDestination(walkableDestination);
        }

        if (requireAutonomousRoamAcceptance && IsAutonomousRoamRecoverySamplingActive())
        {
            float recoveryDistance = GetAutonomousRoamRecoverySampleDistance();
            Vector2[] recoveryDirections =
            {
                Vector2.right,
                Vector2.left,
                Vector2.up,
                Vector2.down,
                new Vector2(1f, 1f).normalized,
                new Vector2(-1f, 1f).normalized,
                new Vector2(1f, -1f).normalized,
                new Vector2(-1f, -1f).normalized
            };

            for (int index = 0; index < recoveryDirections.Length; index++)
            {
                Vector2 sampledDestination = NormalizeDestinationToNavGridBounds(
                    samplingCenter + recoveryDirections[index] * recoveryDistance);
                TryCommitSampledDestination(sampledDestination);
            }
        }

        for (int attempt = 0; attempt < sampleAttemptBudget; attempt++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * activityRadius;
            Vector2 sampledDestination = NormalizeDestinationToNavGridBounds(samplingCenter + randomOffset);
            if (!TryCommitSampledDestination(sampledDestination))
            {
                continue;
            }

            if (!requireAutonomousRoamAcceptance)
            {
                return true;
            }
        }

        if (requireAutonomousRoamAcceptance)
        {
            for (int index = 0; index < autonomousRoamCandidates.Count; index++)
            {
                if (TryCommitResolvedDestination(autonomousRoamCandidates[index].Destination))
                {
                    return true;
                }
            }
        }

        RecordPathBuildOutcome(success: false);
        if (showDebugLog)
        {
            string navSummary = navGrid != null
                ? navGrid.BuildBlockingDebugSummary(currentPosition, GetBlockingDebugProbeRadius(), navigationCollider)
                : "NavGridMissing";
            Debug.LogWarning(
                $"[NPCAutoRoamController] {name} 在 {sampleAttemptBudget} 次采样内没有找到可用路径。 " +
                $"state={state} blockedFrames={blockedAdvanceFrames} pathFailures={consecutivePathBuildFailures} " +
                $"current={currentPosition} roamCenter={roamCenter} nav={navSummary}",
                this);
        }

        return false;
    }

    private int GetAutonomousRoamSampleAttemptBudget()
    {
        int budget = Mathf.Max(1, pathSampleAttempts);
        if (!UsesAutonomousRoamExecutionContract())
        {
            return budget;
        }

        if (blockedAdvanceFrames > 0 || currentStuckRecoveryCount > 0 || consecutivePathBuildFailures > 0)
        {
            return Mathf.Min(budget, AUTONOMOUS_ROAM_STATIC_RECOVERY_SAMPLE_BUDGET);
        }

        if (hasPendingMoveCommandProgressCheck || sharedAvoidanceBlockingFrames > 0 || hasDynamicDetour)
        {
            return Mathf.Min(budget, 6);
        }

        return budget;
    }

    private bool IsAutonomousRoamRecoverySamplingActive()
    {
        return UsesAutonomousRoamExecutionContract() &&
            (blockedAdvanceFrames > 0 ||
             currentStuckRecoveryCount > 0 ||
             consecutivePathBuildFailures > 0);
    }

    private Vector2 ResolveAutonomousRoamSamplingCenter(Vector2 currentPosition, Vector2 roamCenter)
    {
        if (!IsAutonomousRoamRecoverySamplingActive())
        {
            return roamCenter;
        }

        if (TryResolveSafeRoamCenterNear(currentPosition, out Vector2 localSafeCenter))
        {
            return localSafeCenter;
        }

        return currentPosition;
    }

    private float GetAutonomousRoamRecoverySampleDistance()
    {
        float desiredDistance = Mathf.Max(
            minimumMoveDistance * 1.1f,
            GetColliderRadius() + GetAutonomousRoamBodyClearanceExtraRadius() + GetContactShellPadding());
        return Mathf.Clamp(
            desiredDistance,
            minimumMoveDistance,
            Mathf.Max(minimumMoveDistance, activityRadius));
    }

    private bool IsWithinAutonomousRoamBounds(Vector2 roamCenter, Vector2 candidatePosition)
    {
        return Vector2.Distance(roamCenter, candidatePosition) <=
               activityRadius + GetAutonomousRoamDestinationCorrectionLimit();
    }

    private void LogBlockedMovementDiagnostics(
        string reason,
        Vector2 currentPosition,
        Vector2 waypoint,
        Vector2 candidatePosition)
    {
        if (!showDebugLog || navGrid == null)
        {
            return;
        }

        if (blockedAdvanceFrames > 1 &&
            blockedAdvanceFrames != BLOCKED_ADVANCE_REROUTE_MIN_FRAMES &&
            blockedAdvanceFrames != BLOCKED_ADVANCE_LONG_PAUSE_MIN_FRAMES)
        {
            return;
        }

        float debugRadius = GetBlockingDebugProbeRadius();
        string currentSummary = navGrid.BuildBlockingDebugSummary(currentPosition, debugRadius, navigationCollider);
        string candidateSummary = navGrid.BuildBlockingDebugSummary(candidatePosition, debugRadius, navigationCollider);
        string destinationSummary = navGrid.BuildBlockingDebugSummary(currentDestination, debugRadius, navigationCollider);

        Debug.LogWarning(
            $"[NPCBlockedDebug] {name} reason={reason} state={state} moveSkip={lastMoveSkipReason} " +
            $"blockedFrames={blockedAdvanceFrames} current={currentPosition} waypoint={waypoint} next={candidatePosition} destination={currentDestination} " +
            $"sampleBudget={GetAutonomousRoamSampleAttemptBudget()} currentHits={currentSummary} nextHits={candidateSummary} destinationHits={destinationSummary}",
            this);
    }

    private float GetBlockingDebugProbeRadius()
    {
        float bodyRadius = Mathf.Max(0.05f, GetColliderRadius() + GetContactShellPadding());
        float traversalRadius = Mathf.Max(
            0.05f,
            navigationFootProbeVerticalInset + navigationFootProbeExtraRadius,
            navigationFootProbeSideInset + navigationFootProbeExtraRadius);
        return Mathf.Min(bodyRadius, Mathf.Max(traversalRadius, 0.08f));
    }

    private bool CheckAndHandleStuck(Vector2 currentPosition)
    {
        if (stuckCheckInterval <= 0f)
        {
            return false;
        }

        if (IsSharedAvoidanceDetourProtected(Time.time))
        {
            return false;
        }

        NavigationPathExecutor2D.ProgressCheckResult progress = NavigationPathExecutor2D.EvaluateStuck(
            navigationExecution,
            currentPosition,
            Time.time,
            stuckCheckInterval,
            stuckDistanceThreshold,
            maxStuckRecoveries);

        if (!progress.Checked || !progress.IsStuck)
        {
            return false;
        }

        if (showDebugLog)
        {
            Debug.LogWarning(
                $"[NPCAutoRoamController] {name} 检测到卡住 ({progress.RetryCount}/{maxStuckRecoveries})，最近位移 {progress.MovedDistance:F3}m",
                this);
        }

        if (IsRecoverablePointToPointTravelActive())
        {
            return TryHandleRecoverablePointToPointTravelStuck(currentPosition, progress);
        }

        if (debugMoveActive)
        {
            EndDebugMove(reachedDestination: false);
            return true;
        }

        bool preserveTrafficState = lastBlockingAgentId != 0 || sharedAvoidanceBlockingFrames > 0;
        if (TryRebuildPath(
                currentPosition,
                resetRecoveryCounter: false,
                reason: progress.ShouldCancel ? "StuckCancelRecover" : "StuckRecover",
                preserveTrafficState: preserveTrafficState,
                preserveBlockedRecoveryState: true))
        {
            return true;
        }

        if (TryBeginMove(preserveBlockedRecoveryState: true))
        {
            return true;
        }

        if (TryRecoverAutonomousRoamFailureBeforeInterrupt(
                currentPosition,
                progress.ShouldCancel ? "StuckCancel" : "StuckRecoveryFailed"))
        {
            return true;
        }

        if (progress.ShouldCancel &&
            UsesAutonomousRoamExecutionContract())
        {
            lastMoveSkipReason = "StuckCancelStopgap";
            FinishMoveCycle(countTowardLongPause: false, reachedDestination: false);
            return true;
        }

        if (progress.ShouldCancel)
        {
            if (TryHandleTerminalStuck(currentPosition))
            {
                return true;
            }

            if (TryInterruptRoamMove(
                RoamMoveInterruptionReason.StuckCancel,
                currentPosition,
                lastBlockingAgentId,
                null,
                RoamMoveInterruptionBlockerKind.None,
                "StuckCancel"))
            {
                return true;
            }

            FinishMoveCycle(countTowardLongPause: false, reachedDestination: false);
            return true;
        }

        if (TryInterruptRoamMove(
            RoamMoveInterruptionReason.StuckRecoveryFailed,
            currentPosition,
            lastBlockingAgentId,
            null,
            RoamMoveInterruptionBlockerKind.None,
            "StuckRecoveryFailed"))
        {
            return true;
        }

        FinishMoveCycle(countTowardLongPause: false, reachedDestination: false);
        return true;
    }

    private bool TryHandleRecoverablePointToPointTravelStuck(
        Vector2 currentPosition,
        NavigationPathExecutor2D.ProgressCheckResult progress)
    {
        string contractName = GetPointToPointTravelContractName(activePointToPointTravelContract);
        bool preserveTrafficState = lastBlockingAgentId != 0 || sharedAvoidanceBlockingFrames > 0;
        if (TryRebuildPath(
                currentPosition,
                resetRecoveryCounter: false,
                reason: progress.ShouldCancel ? $"{contractName}StuckCancelRecover" : $"{contractName}StuckRecover",
                preserveTrafficState: preserveTrafficState,
                preserveBlockedRecoveryState: true))
        {
            return true;
        }

        if (!progress.ShouldCancel)
        {
            return true;
        }

        lastMoveSkipReason = $"{contractName}StuckCancel";
        EndDebugMove(reachedDestination: false);
        return true;
    }

    private bool TryRebuildPath(
        Vector2 currentPosition,
        bool resetRecoveryCounter,
        string reason = "重建路径",
        bool preserveTrafficState = false,
        bool preserveBlockedRecoveryState = false)
    {
        if (!TryResolveNavGrid())
        {
            return false;
        }

        BlockedRecoveryState blockedRecoveryState = preserveBlockedRecoveryState
            ? CaptureBlockedRecoveryState()
            : default;

        if (!TryResolveRebuildRequestedDestination(out Vector2 rebuildDestination))
        {
            return false;
        }

        if (ShouldSkipRebuildPathRequest(currentPosition, rebuildDestination))
        {
            return false;
        }

        if (!TryAcquirePathBuildBudget())
        {
            return false;
        }

        RecordRebuildPathRequest(currentPosition, rebuildDestination);
        if (!TryBuildPathToDestination(
                currentPosition,
                rebuildDestination,
                reason,
                out _))
        {
            RecordPathBuildOutcome(success: false);
            return false;
        }

        if (!IsCurrentBuiltPathAcceptable(currentPosition, rebuildDestination))
        {
            RecordPathBuildOutcome(success: false);
            if (UsesAutonomousRoamExecutionContract())
            {
                RememberStaticBlockedDestination(currentPosition, rebuildDestination);
            }

            NavigationPathExecutor2D.Clear(navigationExecution, clearDestination: false);
            return false;
        }

        RecordPathBuildOutcome(success: true);
        ResetMovementRecovery(
            currentPosition,
            resetCounter: resetRecoveryCounter,
            preserveTrafficState: preserveTrafficState);
        if (preserveBlockedRecoveryState)
        {
            RestoreBlockedRecoveryState(blockedRecoveryState);
        }

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=yellow>[NPCAutoRoamController]</color> {name} {reason} => Requested={rebuildDestination}, Actual={currentDestination}, PathCount={path.Count}",
                this);
        }

        return true;
    }

    private bool TryResolveRebuildRequestedDestination(out Vector2 rebuildDestination)
    {
        rebuildDestination = NormalizeDestinationToNavGridBounds(GetRebuildRequestedDestination());
        if (!IsRecoverablePointToPointTravelActive())
        {
            return true;
        }

        return TryResolveReleasedMovementDestination(rebuildDestination, out rebuildDestination);
    }

    private bool IsCurrentBuiltPathAcceptable(Vector2 currentPosition, Vector2 destination)
    {
        if (UsesAutonomousRoamExecutionContract())
        {
            return IsAutonomousRoamBuiltPathAcceptable(currentPosition, GetRoamCenter(), destination);
        }

        if (IsRecoverablePointToPointTravelActive())
        {
            return IsRecoverablePointToPointBuiltPathAcceptable(currentPosition, destination);
        }

        return true;
    }

    private bool IsBuiltPathAcceptableForTravelContract(
        Vector2 currentPosition,
        Vector2 destination,
        PointToPointTravelContract travelContract)
    {
        if (!RequiresReleasedMovementBodyClearance(travelContract))
        {
            return true;
        }

        return IsRecoverablePointToPointBuiltPathAcceptable(currentPosition, destination);
    }

    private bool ShouldSkipRebuildPathRequest(Vector2 currentPosition, Vector2 rebuildDestination)
    {
        float elapsed = Time.time - lastRebuildRequestTime;
        if (elapsed < 0f || elapsed > REBUILD_REQUEST_DEDUPE_WINDOW_SECONDS)
        {
            return false;
        }

        bool samePosition = Vector2.Distance(currentPosition, lastRebuildRequestPosition) <= BLOCKED_ADVANCE_SAME_POSITION_RADIUS;
        if (!samePosition)
        {
            return false;
        }

        float destinationToleranceRadius = Mathf.Max(destinationTolerance + 0.05f, REBUILD_REQUEST_DESTINATION_RADIUS);
        return Vector2.Distance(rebuildDestination, lastRebuildRequestDestination) <= destinationToleranceRadius;
    }

    private void RecordRebuildPathRequest(Vector2 currentPosition, Vector2 rebuildDestination)
    {
        lastRebuildRequestTime = Time.time;
        lastRebuildRequestPosition = currentPosition;
        lastRebuildRequestDestination = rebuildDestination;
    }

    private System.Action<string> GetPathBuildLogger(string context)
    {
        if (!showDebugLog)
        {
            return null;
        }

        return message =>
            Debug.Log($"<color=orange>[NPCNavBuild]</color> {name} {context}: {message}", this);
    }

    private bool TryHandleSharedAvoidance(
        Vector2 movementPosition,
        Vector2 navigationPosition,
        Vector2 waypoint,
        Vector2 desiredDirection,
        out Vector2 adjustedDirection,
        out float speedScale)
    {
        adjustedDirection = desiredDirection;
        speedScale = 1f;
        if (ShouldBypassSharedAvoidanceForCurrentMove())
        {
            sharedAvoidanceNoBlockerFrames = 0;
            sharedAvoidanceBlockingFrames = 0;
            lastBlockingAgentId = 0;
            blockingAgentSightings = 0;
            return false;
        }

        NavigationAgentSnapshot self = NavigationAgentSnapshot.FromUnit(this);
        if (!self.IsValid || desiredDirection.sqrMagnitude < 0.0001f)
        {
            return false;
        }

        float referenceSpeed = motionController != null ? motionController.MoveSpeed : 0f;
        float lookAhead = NavigationLocalAvoidanceSolver.GetRecommendedLookAhead(self, sharedAvoidanceLookAhead, referenceSpeed);
        NavigationAgentRegistry.GetNearbySnapshots(this, navigationPosition, lookAhead, nearbyNavigationAgents);
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance = NavigationLocalAvoidanceSolver.Solve(
            self,
            desiredDirection,
            lookAhead,
            nearbyNavigationAgents);
        bool isInReleaseRecoveryWindow = IsSharedAvoidanceInReleaseRecoveryWindow(Time.time);

        if (!avoidance.HasBlockingAgent)
        {
            sharedAvoidanceNoBlockerFrames++;
            sharedAvoidanceBlockingFrames = 0;
            lastBlockingAgentId = 0;
            blockingAgentSightings = 0;
            bool shouldDelayDetourRelease = hasDynamicDetour &&
                sharedAvoidanceNoBlockerFrames < SHARED_DETOUR_RELEASE_STABLE_FRAMES;
            if (shouldDelayDetourRelease)
            {
                adjustedDirection = avoidance.AdjustedDirection.sqrMagnitude > 0.0001f
                    ? avoidance.AdjustedDirection
                    : desiredDirection;
                speedScale = Mathf.Clamp01(avoidance.SpeedScale);
                return false;
            }

            if (TryReleaseSharedAvoidanceDetour(navigationPosition, Time.time))
            {
                if (state != RoamState.Moving)
                {
                    return true;
                }

                adjustedDirection = avoidance.AdjustedDirection.sqrMagnitude > 0.0001f
                    ? avoidance.AdjustedDirection
                    : desiredDirection;
                speedScale = Mathf.Clamp01(avoidance.SpeedScale);
                return false;
            }

            adjustedDirection = avoidance.AdjustedDirection.sqrMagnitude > 0.0001f
                ? avoidance.AdjustedDirection
                : desiredDirection;
            speedScale = Mathf.Clamp01(avoidance.SpeedScale);
            return false;
        }

        sharedAvoidanceNoBlockerFrames = 0;
        sharedAvoidanceBlockingFrames++;
        if (lastBlockingAgentId == avoidance.BlockingAgentId)
        {
            blockingAgentSightings++;
        }
        else
        {
            lastBlockingAgentId = avoidance.BlockingAgentId;
            blockingAgentSightings = 1;
        }

        adjustedDirection = avoidance.AdjustedDirection.sqrMagnitude > 0.0001f
            ? avoidance.AdjustedDirection
            : desiredDirection;
        speedScale = Mathf.Clamp01(avoidance.SpeedScale);

        NavigationLocalAvoidanceSolver.CloseRangeConstraintResult closeRangeConstraint =
            NavigationLocalAvoidanceSolver.ApplyCloseRangeConstraint(
                navigationPosition,
                adjustedDirection,
                speedScale,
                GetColliderRadius(),
                GetContactShellPadding(),
                avoidance);

        if (closeRangeConstraint.Applied)
        {
            adjustedDirection = closeRangeConstraint.ConstrainedDirection.sqrMagnitude > 0.0001f
                ? closeRangeConstraint.ConstrainedDirection
                : adjustedDirection;
            speedScale = Mathf.Clamp01(closeRangeConstraint.SpeedScale);
        }

        bool dynamicAvoidanceBlocker = IsDynamicAvoidanceBlocker(avoidance);
        if (dynamicAvoidanceBlocker &&
            ShouldResetSharedAvoidanceStuckProgress(closeRangeConstraint, speedScale, avoidance.ShouldRepath))
        {
            RefreshProgressCheckpoint(movementPosition, resetCounter: true);
        }

        if (dynamicAvoidanceBlocker &&
            TryBreakAutonomousSharedAvoidanceDeadlock(
                movementPosition,
                navigationPosition,
                avoidance,
                closeRangeConstraint))
        {
            return true;
        }

        if (isInReleaseRecoveryWindow)
        {
            MaybeLogSharedAvoidance(navigationPosition, waypoint, avoidance, closeRangeConstraint, speedScale, "RecoverWindowMove");
            if (closeRangeConstraint.HardBlocked && speedScale <= 0.025f)
            {
                StopForSharedAvoidance(navigationPosition, avoidance, closeRangeConstraint, "RecoverWindowHardStop");
                if (!dynamicAvoidanceBlocker)
                {
                    return TryHandlePassiveAvoidanceHardStop(
                        movementPosition,
                        avoidance,
                        closeRangeConstraint,
                        "SharedAvoidanceRecoverWindowHardStop");
                }

                return true;
            }

            return false;
        }

        MaybeLogSharedAvoidance(navigationPosition, waypoint, avoidance, closeRangeConstraint, speedScale, "Move");

        if (!avoidance.ShouldRepath)
        {
            if (closeRangeConstraint.HardBlocked && speedScale <= 0.025f)
            {
                StopForSharedAvoidance(navigationPosition, avoidance, closeRangeConstraint, "HardStop");
                if (!dynamicAvoidanceBlocker)
                {
                    return TryHandlePassiveAvoidanceHardStop(
                        movementPosition,
                        avoidance,
                        closeRangeConstraint,
                        "SharedAvoidanceHardStop");
                }

                return true;
            }

            return false;
        }

        if (Time.time - lastSharedAvoidanceRepathTime < sharedAvoidanceRepathCooldown)
        {
            if (closeRangeConstraint.HardBlocked && speedScale <= 0.025f)
            {
                StopForSharedAvoidance(navigationPosition, avoidance, closeRangeConstraint, "HardStop");
                if (!dynamicAvoidanceBlocker)
                {
                    return TryHandlePassiveAvoidanceHardStop(
                        movementPosition,
                        avoidance,
                        closeRangeConstraint,
                        "SharedAvoidanceHardStopCooldown");
                }

                return true;
            }

            return false;
        }

        lastSharedAvoidanceRepathTime = Time.time;
        blockingAgentSightings = 0;
        StopForSharedAvoidance(navigationPosition, avoidance, closeRangeConstraint, "Repath");

        if (TryCreateSharedAvoidanceDetour(navigationPosition, adjustedDirection, avoidance))
        {
            return true;
        }

        if (TryRebuildPath(
                movementPosition,
                resetRecoveryCounter: false,
                preserveTrafficState: true,
                preserveBlockedRecoveryState: true))
        {
            return true;
        }

        if (ShouldRerouteAroundPassiveNpcBlocker(avoidance, closeRangeConstraint) &&
            TryBeginMove(preserveBlockedRecoveryState: true))
        {
            return true;
        }

        return TryInterruptRoamMove(
            RoamMoveInterruptionReason.SharedAvoidanceRepathFailed,
            movementPosition,
            avoidance.BlockingAgentId,
            avoidance.BlockingAgentPosition,
            RoamMoveInterruptionBlockerKind.None,
            "SharedAvoidanceRepathFailed",
            nearbyNavigationAgents);
    }

    private bool TryBreakAutonomousSharedAvoidanceDeadlock(
        Vector2 movementPosition,
        Vector2 navigationPosition,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        NavigationLocalAvoidanceSolver.CloseRangeConstraintResult closeRangeConstraint)
    {
        if (!UsesAutonomousRoamExecutionContract())
        {
            return false;
        }

        if (sharedAvoidanceBlockingFrames < SHARED_AVOIDANCE_DYNAMIC_ABORT_MIN_FRAMES ||
            blockingAgentSightings < SHARED_AVOIDANCE_DYNAMIC_ABORT_MIN_SIGHTINGS ||
            !avoidance.HasBlockingAgent ||
            !TryResolveAvoidanceBlockingSnapshot(avoidance, out NavigationAgentSnapshot blocker) ||
            blocker.UnitType != NavigationUnitType.NPC)
        {
            return false;
        }

        float blockerDistance = Mathf.Min(
            avoidance.BlockingDistance,
            Vector2.Distance(navigationPosition, blocker.Position));
        bool isHardStopped =
            closeRangeConstraint.HardBlocked ||
            closeRangeConstraint.SpeedScale <= 0.08f ||
            closeRangeConstraint.Clearance <= SHARED_AVOIDANCE_DYNAMIC_ABORT_MAX_CLEARANCE;
        if (!isHardStopped || blockerDistance > SHARED_AVOIDANCE_DYNAMIC_ABORT_MAX_DISTANCE)
        {
            return false;
        }

        StopForSharedAvoidance(navigationPosition, avoidance, closeRangeConstraint, "DynamicDeadlockAbort");
        sharedAvoidanceNoBlockerFrames = 0;
        sharedAvoidanceBlockingFrames = 0;
        lastBlockingAgentId = 0;
        blockingAgentSightings = 0;
        lastSharedAvoidanceRepathTime = Time.time;

        Vector2 detourDirection = avoidance.AdjustedDirection.sqrMagnitude > 0.0001f
            ? avoidance.AdjustedDirection
            : (avoidance.SuggestedDetourDirection.sqrMagnitude > 0.0001f
                ? avoidance.SuggestedDetourDirection
                : (blocker.Position - navigationPosition).normalized);
        if (TryCreateSharedAvoidanceDetour(navigationPosition, detourDirection, avoidance))
        {
            lastMoveSkipReason = "SharedAvoidanceDeadlockDetour";
            RefreshProgressCheckpoint(movementPosition, resetCounter: true);
            return true;
        }

        if (TryBeginMove(preserveBlockedRecoveryState: true))
        {
            lastMoveSkipReason = "SharedAvoidanceDeadlockRetarget";
            RefreshProgressCheckpoint(movementPosition, resetCounter: true);
            return true;
        }

        lastMoveSkipReason = "SharedAvoidanceDeadlockAbort";
        FinishMoveCycle(countTowardLongPause: false, reachedDestination: false);
        return true;
    }

    private bool TryCreateSharedAvoidanceDetour(
        Vector2 navigationPosition,
        Vector2 adjustedDirection,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance)
    {
        if (navGrid == null)
        {
            return false;
        }

        float detourDistance =
            GetColliderRadius() +
            Mathf.Max(avoidance.BlockingAgentRadius, 0.3f) +
            GetContactShellPadding() +
            0.1f;

        NavigationPathExecutor2D.DetourLifecycleResult detour = NavigationPathExecutor2D.TryCreateDetour(
            navigationExecution,
            navGrid,
            navigationPosition,
            avoidance.BlockingAgentId,
            avoidance.BlockingAgentPosition,
            adjustedDirection,
            avoidance.SuggestedDetourDirection,
            detourDistance,
            0.5f,
            SharedAvoidanceDetourCandidateSpecs,
            Time.time);

        if (detour.Created)
        {
            sharedAvoidanceDetourCreateCount++;
        }

        return detour.Created || detour.ShouldKeepCurrentDetour;
    }

    private bool TryReleaseSharedAvoidanceDetour(Vector2 navigationPosition, float currentTime)
    {
        bool hasRecoverableDetourContext = hasDynamicDetour ||
            (!navigationExecution.LastDetourRecoverySucceeded && navigationExecution.LastDetourOwnerId != 0);
        if (!hasRecoverableDetourContext)
        {
            return false;
        }

        sharedAvoidanceReleaseAttemptCount++;
        NavigationPathExecutor2D.DetourLifecycleResult detour = NavigationPathExecutor2D.TryClearDetourAndRecover(
            navigationExecution,
            navGrid,
            navigationPosition,
            GetRebuildRequestedDestination(),
            0,
            currentTime,
            rebuildPath: false,
            minimumDetourActiveDuration: SHARED_DETOUR_MIN_ACTIVE_DURATION,
            recoveryCooldown: 0f,
            hasLineOfSight: null,
            cleanupReferencePosition: null,
            cleanupWaypointTolerance: 0f,
            log: null,
            ignoredCollider: navigationCollider);

        if (detour.ShouldKeepCurrentDetour || detour.HasActiveDetour)
        {
            return false;
        }

        if (detour.Cleared || detour.Recovered)
        {
            sharedAvoidanceReleaseSuccessCount++;
            lastSharedAvoidanceReleaseTime = currentTime;
            sharedAvoidanceNoBlockerFrames = 0;
            sharedAvoidanceBlockingFrames = 0;
            lastBlockingAgentId = 0;
            blockingAgentSightings = 0;
            lastSharedAvoidanceRepathTime = currentTime;

            if (IsRecoverablePointToPointTravelActive())
            {
                if (motionController != null)
                {
                    motionController.StopMotion();
                }

                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }

                TryRebuildPath(
                    navigationPosition,
                    resetRecoveryCounter: false,
                    reason: "SharedAvoidanceRecover",
                    preserveTrafficState: true);
            }

            return true;
        }

        return false;
    }

    private void ResetSharedAvoidanceDebugState()
    {
        lastSharedAvoidanceReleaseTime = float.NegativeInfinity;
        sharedAvoidanceNoBlockerFrames = 0;
        sharedAvoidanceBlockingFrames = 0;
        sharedAvoidanceDetourCreateCount = 0;
        sharedAvoidanceReleaseAttemptCount = 0;
        sharedAvoidanceReleaseSuccessCount = 0;
    }

    private void StopForSharedAvoidance(
        Vector2 currentPosition,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        NavigationLocalAvoidanceSolver.CloseRangeConstraintResult closeRangeConstraint,
        string action)
    {
        if (motionController != null)
        {
            motionController.StopMotion();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        MaybeLogSharedAvoidance(currentPosition, currentDestination, avoidance, closeRangeConstraint, 0f, action);
    }

    private void MaybeLogSharedAvoidance(
        Vector2 currentPosition,
        Vector2 waypoint,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        NavigationLocalAvoidanceSolver.CloseRangeConstraintResult closeRangeConstraint,
        float speedScale,
        string action)
    {
        if (!showDebugLog || !avoidance.HasBlockingAgent)
        {
            return;
        }

        if (Time.frameCount - lastSharedAvoidanceDebugFrame < 10)
        {
            return;
        }

        lastSharedAvoidanceDebugFrame = Time.frameCount;
        Debug.Log(
            $"<color=yellow>[NPCAvoid]</color> {name} action={action}, agent={avoidance.BlockingAgentId}, " +
            $"state={state}, pos={currentPosition}, waypoint={waypoint}, blockerPos={avoidance.BlockingAgentPosition}, " +
            $"blockingDistance={avoidance.BlockingDistance:F2}, speedScale={speedScale:F2}, shouldRepath={avoidance.ShouldRepath}, " +
            $"clearance={closeRangeConstraint.Clearance:F2}, forwardInto={closeRangeConstraint.ForwardIntoBlocker:F2}, " +
            $"hardBlocked={closeRangeConstraint.HardBlocked}, rbPos={(rb != null ? rb.position : currentPosition)}",
            this);
    }

    private Vector2 GetNavigationCenter()
    {
        return rb != null ? rb.position : (Vector2)transform.position;
    }

    private float GetContactShellPadding()
    {
        return 0.05f;
    }

    private bool ShouldApplyAutonomousStaticObstacleSteering()
    {
        return UsesAutonomousRoamExecutionContract() || IsRecoverablePointToPointTravelActive();
    }

    private Vector2 AdjustDirectionByStaticColliders(Vector2 position, Vector2 desiredDirection, float stepDistance)
    {
        if (navigationCollider == null || desiredDirection.sqrMagnitude <= 0.0001f)
        {
            return desiredDirection;
        }

        float clearanceRadius = Mathf.Max(
            0.18f,
            Mathf.Max(
                GetAvoidanceRadius() + 0.04f,
                GetColliderRadius() + GetAutonomousRoamBodyClearanceExtraRadius() + GetContactShellPadding() + 0.08f));
        float[] aheadDistances =
        {
            Mathf.Max(0.28f, Mathf.Max(clearanceRadius * 1.0f, stepDistance * 1.7f)),
            Mathf.Max(0.52f, Mathf.Max(clearanceRadius * 1.75f, stepDistance * 2.7f)),
            Mathf.Max(0.88f, Mathf.Max(clearanceRadius * 2.45f, stepDistance * 3.5f))
        };

        Vector2 totalRepulse = Vector2.zero;
        int obstacleCount = 0;
        for (int probeIndex = 0; probeIndex < aheadDistances.Length; probeIndex++)
        {
            float aheadDistance = aheadDistances[probeIndex];
            Vector2 probe = position + desiredDirection * aheadDistance;
            int hitCount = ProbeStaticObstacleHits(probe, clearanceRadius);
            float probeWeight = 1f / (aheadDistance + 0.1f);

            for (int hitIndex = 0; hitIndex < hitCount; hitIndex++)
            {
                Collider2D hitCollider = staticObstacleProbeBuffer[hitIndex];
                if (!ShouldTreatAsStaticSceneObstacle(hitCollider))
                {
                    continue;
                }

                obstacleCount++;
                Vector2 closestPoint = hitCollider.ClosestPoint(probe);
                Vector2 away = probe - closestPoint;
                float distance = away.magnitude;
                if (distance < 0.01f)
                {
                    away = probe - (Vector2)hitCollider.bounds.center;
                    distance = away.magnitude;
                }

                if (distance <= 0.01f)
                {
                    continue;
                }

                float repulseStrength = 1f / (distance * distance + 0.08f);
                totalRepulse += away.normalized * repulseStrength * probeWeight;
            }
        }

        if (obstacleCount == 0 || totalRepulse.sqrMagnitude <= 0.0001f)
        {
            return desiredDirection;
        }

        // 当前方障碍把 repulse 压成明显“纯后退”时，给一个稳定侧绕偏置，避免贴墙原地磨。
        Vector2 repulseDirection = totalRepulse.normalized;
        float frontalPressure = Mathf.Max(0f, -Vector2.Dot(repulseDirection, desiredDirection));
        if (frontalPressure > 0.2f)
        {
            Vector2 sidestepAxis = Vector2.Perpendicular(desiredDirection).normalized;
            float sidestepSign = Mathf.Sign(Vector2.Dot(sidestepAxis, totalRepulse));
            if (Mathf.Approximately(sidestepSign, 0f))
            {
                sidestepSign = (GetInstanceID() & 1) == 0 ? 1f : -1f;
            }

            totalRepulse -= desiredDirection * Mathf.Lerp(0.18f, 0.42f, frontalPressure);
            totalRepulse += sidestepAxis * sidestepSign * Mathf.Lerp(0.35f, 0.85f, frontalPressure);
        }

        Vector2 adjustedDirection = (desiredDirection + totalRepulse * AUTONOMOUS_STATIC_STEERING_REPULSE_GAIN).normalized;
        if (adjustedDirection.sqrMagnitude <= 0.0001f)
        {
            return desiredDirection;
        }

        float angle = Vector2.SignedAngle(desiredDirection, adjustedDirection);
        if (Mathf.Abs(angle) > AUTONOMOUS_STATIC_STEERING_MAX_ANGLE)
        {
            adjustedDirection = RotateVector(desiredDirection, Mathf.Sign(angle) * AUTONOMOUS_STATIC_STEERING_MAX_ANGLE).normalized;
        }

        return adjustedDirection;
    }

    private int ProbeStaticObstacleHits(Vector2 center, float radius)
    {
        if (staticObstacleProbeBuffer == null || staticObstacleProbeBuffer.Length < 12)
        {
            staticObstacleProbeBuffer = new Collider2D[12];
        }

        int hitCount = Physics2D.OverlapCircle(center, radius, staticObstacleProbeFilter, staticObstacleProbeBuffer);
        if (hitCount == staticObstacleProbeBuffer.Length)
        {
            System.Array.Resize(ref staticObstacleProbeBuffer, staticObstacleProbeBuffer.Length * 2);
            hitCount = Physics2D.OverlapCircle(center, radius, staticObstacleProbeFilter, staticObstacleProbeBuffer);
        }

        return hitCount;
    }

    private bool ShouldTreatAsStaticSceneObstacle(Collider2D collider)
    {
        if (collider == null ||
            collider == navigationCollider ||
            collider.isTrigger)
        {
            return false;
        }

        if (collider.attachedRigidbody != null &&
            collider.attachedRigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            return false;
        }

        if (collider.name.Contains("(Clone)") || collider.name.Contains("Pickup"))
        {
            return false;
        }

        if (isTraversalSoftPassActive && IsTraversalSoftPassBlocker(collider))
        {
            return false;
        }

        if (TryGetNavigationUnit(collider, out INavigationUnit navigationUnit))
        {
            return navigationUnit.GetUnitType() == NavigationUnitType.StaticObstacle;
        }

        return true;
    }

    private bool IsTraversalSoftPassBlocker(Collider2D collider)
    {
        if (collider == null || traversalSoftPassBlockers == null || traversalSoftPassBlockers.Length == 0)
        {
            return false;
        }

        for (int index = 0; index < traversalSoftPassBlockers.Length; index++)
        {
            if (traversalSoftPassBlockers[index] == collider)
            {
                return true;
            }
        }

        return false;
    }

    private bool TryGetNavigationUnit(Collider2D collider, out INavigationUnit navigationUnit)
    {
        navigationUnit = null;
        if (collider == null)
        {
            return false;
        }

        NavigationUnitBase unitBase = collider.GetComponentInParent<NavigationUnitBase>();
        if (unitBase != null)
        {
            navigationUnit = unitBase;
            return true;
        }

        PlayerAutoNavigator playerNavigator = collider.GetComponentInParent<PlayerAutoNavigator>();
        if (playerNavigator != null)
        {
            navigationUnit = playerNavigator;
            return true;
        }

        NPCAutoRoamController npcNavigator = collider.GetComponentInParent<NPCAutoRoamController>();
        if (npcNavigator != null)
        {
            navigationUnit = npcNavigator;
            return true;
        }

        return false;
    }

    private static Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        return new Vector2(
            Mathf.Cos(radians) * vector.x - Mathf.Sin(radians) * vector.y,
            Mathf.Sin(radians) * vector.x + Mathf.Cos(radians) * vector.y);
    }

    private bool IsSharedAvoidanceDetourProtected(float currentTime)
    {
        return hasDynamicDetour &&
            navigationExecution.LastDetourCreateTime > float.NegativeInfinity &&
            currentTime - navigationExecution.LastDetourCreateTime < SHARED_DETOUR_MIN_ACTIVE_DURATION;
    }

    private bool IsSharedAvoidanceInReleaseRecoveryWindow(float currentTime)
    {
        return !hasDynamicDetour &&
            lastSharedAvoidanceReleaseTime > float.NegativeInfinity &&
            currentTime - lastSharedAvoidanceReleaseTime < SHARED_DETOUR_POST_RELEASE_RECOVERY_WINDOW;
    }

    public bool TryResolveNavGrid(bool logIfMissing = true)
    {
        if (navGrid != null)
        {
            return true;
        }

        navGrid = FindFirstObjectByType<NavGrid2D>();
        if (navGrid != null)
        {
            warnedMissingNavGrid = false;
            return true;
        }

        if (logIfMissing && !warnedMissingNavGrid)
        {
            warnedMissingNavGrid = true;
            Debug.LogWarning($"[NPCAutoRoamController] {name} 未找到 NavGrid2D，自动漫游暂时不会启动。", this);
        }

        return false;
    }

    private bool TryResolveOccupiableDestination(Vector2 sampledDestination, out Vector2 resolvedDestination)
    {
        if (!TryResolveNavGrid(logIfMissing: false))
        {
            resolvedDestination = sampledDestination;
            return false;
        }

        return NavigationTraversalCore.TryResolveOccupiableDestination(
            GetTraversalContract(),
            sampledDestination,
            out resolvedDestination);
    }

    private bool TryResolveDestinationNear(
        Vector2 sampledDestination,
        DestinationAcceptanceContract acceptanceContract,
        out Vector2 resolvedDestination)
    {
        if (!TryResolveNavGrid(logIfMissing: false))
        {
            resolvedDestination = sampledDestination;
            return false;
        }

        float extraRadius = acceptanceContract == DestinationAcceptanceContract.ReleasedMovement
            ? GetReleasedMovementStepBodyClearanceExtraRadius()
            : GetAutonomousRoamBodyClearanceExtraRadius();
        float correctionLimit = acceptanceContract == DestinationAcceptanceContract.ReleasedMovement
            ? GetReleasedMovementDestinationCorrectionLimit()
            : GetAutonomousRoamDestinationCorrectionLimit();
        float sideInsetScale = acceptanceContract == DestinationAcceptanceContract.ReleasedMovement
            ? RELEASED_STEP_BODY_CLEARANCE_SIDE_INSET_SCALE
            : 0.35f;
        float verticalInsetScale = acceptanceContract == DestinationAcceptanceContract.ReleasedMovement
            ? RELEASED_STEP_BODY_CLEARANCE_VERTICAL_INSET_SCALE
            : 0.55f;

        Vector2 clampedDestination = NormalizeDestinationToNavGridBounds(sampledDestination);
        if (IsDestinationCandidateClear(acceptanceContract, clampedDestination))
        {
            resolvedDestination = clampedDestination;
            return true;
        }

        if (NavigationTraversalCore.TryResolveOccupiableDestinationWithClearanceMargin(
                GetTraversalContract(),
                clampedDestination,
                extraRadius,
                sideInsetScale,
                verticalInsetScale,
                out Vector2 correctedDestination) &&
            Vector2.Distance(clampedDestination, correctedDestination) <= correctionLimit &&
            IsDestinationCandidateClear(acceptanceContract, correctedDestination))
        {
            resolvedDestination = correctedDestination;
            return true;
        }

        float searchRadius = correctionLimit;
        Vector2[] searchDirections =
        {
            Vector2.right,
            Vector2.left,
            Vector2.up,
            Vector2.down,
            new Vector2(1f, 1f).normalized,
            new Vector2(-1f, 1f).normalized,
            new Vector2(1f, -1f).normalized,
            new Vector2(-1f, -1f).normalized
        };

        for (int ringIndex = 1; ringIndex <= AUTONOMOUS_ROAM_SAFE_CENTER_RING_COUNT; ringIndex++)
        {
            float ringDistance = searchRadius * (ringIndex / (float)AUTONOMOUS_ROAM_SAFE_CENTER_RING_COUNT);
            for (int directionIndex = 0; directionIndex < searchDirections.Length; directionIndex++)
            {
                Vector2 candidate = NormalizeDestinationToNavGridBounds(
                    clampedDestination + searchDirections[directionIndex] * ringDistance);
                if (!NavigationTraversalCore.TryResolveOccupiableDestinationWithClearanceMargin(
                        GetTraversalContract(),
                        candidate,
                        extraRadius,
                        sideInsetScale,
                        verticalInsetScale,
                        out Vector2 correctedCandidate))
                {
                    continue;
                }

                if (Vector2.Distance(clampedDestination, correctedCandidate) > correctionLimit)
                {
                    continue;
                }

                if (IsDestinationCandidateClear(acceptanceContract, correctedCandidate))
                {
                    resolvedDestination = correctedCandidate;
                    return true;
                }
            }
        }

        resolvedDestination = clampedDestination;
        return false;
    }

    private bool TryResolveReleasedMovementDestination(Vector2 sampledDestination, out Vector2 resolvedDestination)
    {
        return TryResolveDestinationNear(
            sampledDestination,
            DestinationAcceptanceContract.ReleasedMovement,
            out resolvedDestination);
    }

    private bool TryResolveAutonomousRoamDestinationCandidate(Vector2 sampledDestination, out Vector2 resolvedDestination)
    {
        return TryResolveDestinationNear(
            sampledDestination,
            DestinationAcceptanceContract.AutonomousRoam,
            out resolvedDestination);
    }

    private bool IsDestinationCandidateClear(DestinationAcceptanceContract acceptanceContract, Vector2 candidatePosition)
    {
        switch (acceptanceContract)
        {
            case DestinationAcceptanceContract.ReleasedMovement:
                return IsReleasedMovementDestinationCandidateClear(candidatePosition);

            case DestinationAcceptanceContract.AutonomousRoam:
                return IsAutonomousRoamDestinationCandidateClear(candidatePosition);

            default:
                return CanOccupyNavigationPoint(candidatePosition);
        }
    }

    private bool TryResolveAutonomousRoamDestination(
        Vector2 currentPosition,
        Vector2 roamCenter,
        Vector2 sampledDestination,
        out Vector2 resolvedDestination,
        out float destinationScore)
    {
        resolvedDestination = sampledDestination;
        destinationScore = float.NegativeInfinity;
        if (!TryResolveAutonomousRoamDestinationCandidate(sampledDestination, out Vector2 correctedDestination))
        {
            return false;
        }

        if (!IsAutonomousRoamDestinationCorrectionAcceptable(roamCenter, sampledDestination, correctedDestination))
        {
            return false;
        }

        resolvedDestination = correctedDestination;
        destinationScore = EvaluateAutonomousRoamDestinationScore(
            currentPosition,
            roamCenter,
            sampledDestination,
            correctedDestination);
        return true;
    }

    private bool IsAutonomousRoamDestinationCorrectionAcceptable(
        Vector2 roamCenter,
        Vector2 sampledDestination,
        Vector2 correctedDestination)
    {
        float maxCorrectionDistance = GetAutonomousRoamDestinationCorrectionLimit();
        if (Vector2.Distance(sampledDestination, correctedDestination) > maxCorrectionDistance)
        {
            return false;
        }

        return Vector2.Distance(roamCenter, correctedDestination) <= activityRadius + maxCorrectionDistance;
    }

    private float GetAutonomousRoamDestinationCorrectionLimit()
    {
        float correctionLimit = Mathf.Max(
            GetColliderRadius() + GetContactShellPadding(),
            minimumMoveDistance * 0.8f,
            activityRadius * 0.18f);
        return Mathf.Clamp(
            correctionLimit,
            AUTONOMOUS_ROAM_DESTINATION_CORRECTION_MIN,
            AUTONOMOUS_ROAM_DESTINATION_CORRECTION_MAX);
    }

    private bool HasAutonomousRoamDestinationNeighborhoodClearance(Vector2 destination)
    {
        return CountAutonomousRoamOpenNeighborProbes(destination) >=
            AUTONOMOUS_ROAM_NEIGHBOR_CLEARANCE_MIN_OPEN_PROBES;
    }

    private int CountAutonomousRoamOpenNeighborProbes(Vector2 destination)
    {
        return CountOpenNeighborProbes(
            destination,
            GetAutonomousRoamNeighborClearanceRadius(),
            0f,
            0.35f,
            0.55f);
    }

    private float GetAutonomousRoamNeighborClearanceRadius()
    {
        float desiredRadius = Mathf.Max(
            GetColliderRadius() * AUTONOMOUS_ROAM_NEIGHBOR_CLEARANCE_RADIUS_SCALE,
            minimumMoveDistance * 0.35f);
        return Mathf.Clamp(
            desiredRadius,
            AUTONOMOUS_ROAM_NEIGHBOR_CLEARANCE_MIN_RADIUS,
            AUTONOMOUS_ROAM_NEIGHBOR_CLEARANCE_MAX_RADIUS);
    }

    private bool HasAutonomousRoamDestinationBodyClearance(Vector2 destination)
    {
        return HasAutonomousRoamBodyClearance(
            destination,
            GetAutonomousRoamBodyClearanceExtraRadius());
    }

    private bool HasReleasedMovementDestinationNeighborhoodClearance(Vector2 destination)
    {
        return CountOpenNeighborProbes(
                destination,
                GetReleasedMovementNeighborClearanceRadius(),
                GetReleasedMovementStepBodyClearanceExtraRadius(),
                RELEASED_STEP_BODY_CLEARANCE_SIDE_INSET_SCALE,
                RELEASED_STEP_BODY_CLEARANCE_VERTICAL_INSET_SCALE) >=
            RELEASED_MOVEMENT_NEIGHBOR_CLEARANCE_MIN_OPEN_PROBES;
    }

    private bool HasReleasedMovementDestinationBodyClearance(Vector2 destination)
    {
        return NavigationTraversalCore.CanOccupyNavigationPointWithClearanceMargin(
            GetTraversalContract(),
            destination,
            GetReleasedMovementStepBodyClearanceExtraRadius(),
            RELEASED_STEP_BODY_CLEARANCE_SIDE_INSET_SCALE,
            RELEASED_STEP_BODY_CLEARANCE_VERTICAL_INSET_SCALE);
    }

    private int CountOpenNeighborProbes(
        Vector2 destination,
        float neighborProbeRadius,
        float extraRadius,
        float sideInsetScale,
        float verticalInsetScale)
    {
        if (neighborProbeRadius <= 0.0001f)
        {
            return 4;
        }

        int openProbeCount = 0;
        Vector2[] offsets =
        {
            Vector2.right * neighborProbeRadius,
            Vector2.left * neighborProbeRadius,
            Vector2.up * neighborProbeRadius,
            Vector2.down * neighborProbeRadius
        };

        for (int index = 0; index < offsets.Length; index++)
        {
            if (!NavigationTraversalCore.CanOccupyNavigationPointWithClearanceMargin(
                    GetTraversalContract(),
                    destination + offsets[index],
                    extraRadius,
                    sideInsetScale,
                    verticalInsetScale))
            {
                continue;
            }

            openProbeCount++;
        }

        return openProbeCount;
    }

    private bool HasAutonomousRoamDestinationAgentClearance(Vector2 destination)
    {
        return GetAutonomousRoamDestinationAgentClearanceMargin(destination) >= 0f;
    }

    private float GetAutonomousRoamDestinationAgentClearanceRadius()
    {
        float desiredRadius = Mathf.Max(
            GetColliderRadius() + GetContactShellPadding(),
            Mathf.Max(0.18f, GetAvoidanceRadius() * AUTONOMOUS_ROAM_DESTINATION_AGENT_CLEARANCE_RADIUS_SCALE),
            minimumMoveDistance * 0.24f);
        return Mathf.Clamp(
            desiredRadius,
            AUTONOMOUS_ROAM_DESTINATION_AGENT_CLEARANCE_MIN_RADIUS,
            AUTONOMOUS_ROAM_DESTINATION_AGENT_CLEARANCE_MAX_RADIUS);
    }

    private bool TryGetAutonomousRoamDestinationClaim(out Vector2 destinationClaim)
    {
        destinationClaim = Vector2.zero;
        if (state == RoamState.Inactive)
        {
            return false;
        }

        if (hasRequestedDestination)
        {
            destinationClaim = requestedDestination;
            return true;
        }

        if (state == RoamState.Moving || path.Count > 0 || HasActivePointToPointTravelContract())
        {
            destinationClaim = currentDestination;
            return true;
        }

        return false;
    }

    private bool HasAutonomousRoamBodyClearance(Vector2 position, float extraRadius)
    {
        return NavigationTraversalCore.CanOccupyNavigationPointWithClearanceMargin(
            GetTraversalContract(),
            position,
            extraRadius);
    }

    private float GetAutonomousRoamDestinationAgentClearanceMargin(Vector2 destination)
    {
        EnsureRegisteredRoamControllerBufferCurrentFrame();

        float selfClaimRadius = GetAutonomousRoamDestinationAgentClearanceRadius();
        float nearestMargin = float.PositiveInfinity;
        for (int index = 0; index < registeredRoamControllers.Count; index++)
        {
            NPCAutoRoamController other = registeredRoamControllers[index];
            if (other == null ||
                ReferenceEquals(other, this) ||
                !other.gameObject.scene.IsValid() ||
                other.gameObject.scene != gameObject.scene)
            {
                continue;
            }

            float conflictRadius = selfClaimRadius + other.GetAutonomousRoamDestinationAgentClearanceRadius();
            nearestMargin = Mathf.Min(
                nearestMargin,
                Vector2.Distance(destination, other.GetNavigationCenter()) - conflictRadius);

            if (other.TryGetAutonomousRoamDestinationClaim(out Vector2 otherDestinationClaim))
            {
                nearestMargin = Mathf.Min(
                    nearestMargin,
                    Vector2.Distance(destination, otherDestinationClaim) - conflictRadius);
            }
        }

        return nearestMargin;
    }

    private bool IsRecentlyBlockedAutonomousDestination(Vector2 destination)
    {
        if (lastStaticBlockedDestinationTime <= float.NegativeInfinity)
        {
            return false;
        }

        float elapsed = Time.time - lastStaticBlockedDestinationTime;
        if (elapsed < 0f || elapsed > STATIC_BLOCKED_DESTINATION_RETRY_COOLDOWN)
        {
            return false;
        }

        float blockedPositionRadius = Mathf.Max(
            STATIC_BLOCKED_DESTINATION_RETRY_RADIUS * 0.75f,
            GetColliderRadius() + GetContactShellPadding());
        return Vector2.Distance(destination, lastStaticBlockedDestination) <= STATIC_BLOCKED_DESTINATION_RETRY_RADIUS ||
            Vector2.Distance(destination, lastStaticBlockedPosition) <= blockedPositionRadius;
    }

    private float EvaluateAutonomousRoamDestinationScore(
        Vector2 currentPosition,
        Vector2 roamCenter,
        Vector2 sampledDestination,
        Vector2 resolvedDestination)
    {
        float travelScore = Mathf.Clamp01(
            (Vector2.Distance(currentPosition, resolvedDestination) - minimumMoveDistance) /
            Mathf.Max(activityRadius, minimumMoveDistance));
        float centerScore = 1f - Mathf.Clamp01(
            Vector2.Distance(roamCenter, resolvedDestination) /
            Mathf.Max(activityRadius, 0.001f));
        float correctionPenalty = Mathf.Clamp01(
            Vector2.Distance(sampledDestination, resolvedDestination) /
            Mathf.Max(GetAutonomousRoamDestinationCorrectionLimit(), 0.001f));

        int openNeighborCount = CountAutonomousRoamOpenNeighborProbes(resolvedDestination);
        float neighborScore = openNeighborCount / 4f;

        float bodyClearanceExtraRadius = GetAutonomousRoamBodyClearanceExtraRadius();
        float bodyScore = 0f;
        if (HasAutonomousRoamBodyClearance(resolvedDestination, bodyClearanceExtraRadius * 1.35f))
        {
            bodyScore += 0.75f;
        }

        if (HasAutonomousRoamBodyClearance(resolvedDestination, bodyClearanceExtraRadius * 1.7f))
        {
            bodyScore += 0.85f;
        }

        float agentMargin = GetAutonomousRoamDestinationAgentClearanceMargin(resolvedDestination);
        float agentSpacingScore = float.IsPositiveInfinity(agentMargin)
            ? 1f
            : Mathf.Clamp01(agentMargin / Mathf.Max(0.24f, GetAutonomousRoamDestinationAgentClearanceRadius()));

        return bodyScore * 1.2f +
            neighborScore * 1.05f +
            agentSpacingScore * 1.15f +
            travelScore * 0.35f +
            centerScore * 0.15f -
            correctionPenalty * 0.4f;
    }

    private float GetAutonomousRoamBodyClearanceExtraRadius()
    {
        float desiredRadius = Mathf.Max(
            GetContactShellPadding() * 0.65f,
            GetColliderRadius() * AUTONOMOUS_ROAM_BODY_CLEARANCE_EXTRA_RADIUS_SCALE,
            minimumMoveDistance * 0.18f);
        return Mathf.Clamp(
            desiredRadius,
            AUTONOMOUS_ROAM_BODY_CLEARANCE_MIN_EXTRA_RADIUS,
            AUTONOMOUS_ROAM_BODY_CLEARANCE_MAX_EXTRA_RADIUS);
    }

    private bool HasAutonomousRoamPathBodyClearance(Vector2 currentPosition)
    {
        if (path.Count == 0)
        {
            return false;
        }

        float extraRadius = GetAutonomousRoamBodyClearanceExtraRadius();
        Vector2 previousPoint = currentPosition;
        for (int index = 0; index < path.Count; index++)
        {
            if (!HasAutonomousRoamSegmentBodyClearance(previousPoint, path[index], extraRadius))
            {
                return false;
            }

            previousPoint = path[index];
        }

        return true;
    }

    private bool HasAutonomousRoamSegmentBodyClearance(Vector2 start, Vector2 end, float extraRadius)
    {
        float segmentDistance = Vector2.Distance(start, end);
        if (segmentDistance <= 0.0001f)
        {
            return true;
        }

        int sampleCount = Mathf.Max(1, Mathf.CeilToInt(segmentDistance / AUTONOMOUS_ROAM_PATH_CLEARANCE_SAMPLE_SPACING));
        for (int step = 1; step <= sampleCount; step++)
        {
            Vector2 samplePoint = Vector2.Lerp(start, end, step / (float)sampleCount);
            if (!HasAutonomousRoamBodyClearance(samplePoint, extraRadius))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsAutonomousRoamBuiltPathAcceptable(
        Vector2 currentPosition,
        Vector2 roamCenter,
        Vector2 destination)
    {
        float directDistance = Vector2.Distance(currentPosition, destination);
        if (directDistance <= minimumMoveDistance * 1.1f)
        {
            return true;
        }

        float pathTravelDistance = ComputeCurrentPathTravelDistance(currentPosition);
        float maxExtraDistance = Mathf.Max(
            AUTONOMOUS_ROAM_PATH_DETOUR_MIN_EXTRA_DISTANCE,
            GetColliderRadius() + GetContactShellPadding(),
            minimumMoveDistance * 0.65f);
        float maxAllowedPathDistance = Mathf.Max(
            directDistance * AUTONOMOUS_ROAM_PATH_DETOUR_RATIO_LIMIT,
            directDistance + maxExtraDistance);
        if (pathTravelDistance > maxAllowedPathDistance)
        {
            return false;
        }

        float maxWaypointDistanceFromRoamCenter = activityRadius + Mathf.Max(
            AUTONOMOUS_ROAM_PATH_ROAM_RADIUS_MARGIN,
            GetColliderRadius() * 0.6f,
            minimumMoveDistance * 0.35f);
        for (int index = 0; index < path.Count; index++)
        {
            if (Vector2.Distance(roamCenter, path[index]) > maxWaypointDistanceFromRoamCenter)
            {
                return false;
            }
        }

        return HasAutonomousRoamPathBodyClearance(currentPosition);
    }

    private float ComputeCurrentPathTravelDistance(Vector2 startPosition)
    {
        float totalDistance = 0f;
        Vector2 previousPoint = startPosition;
        for (int index = 0; index < path.Count; index++)
        {
            float segmentDistance = Vector2.Distance(previousPoint, path[index]);
            if (segmentDistance <= 0.0001f)
            {
                continue;
            }

            totalDistance += segmentDistance;
            previousPoint = path[index];
        }

        return totalDistance;
    }

    private Vector2 NormalizeDestinationToNavGridBounds(Vector2 destination)
    {
        if (!TryResolveNavGrid(logIfMissing: false))
        {
            return destination;
        }

        return navGrid.ClampToWorldBounds(destination);
    }

    private void EnsureRegisteredRoamControllerBufferCurrentFrame()
    {
        if (registeredRoamControllersFrame == Time.frameCount)
        {
            return;
        }

        registeredRoamControllersFrame = Time.frameCount;
        NavigationAgentRegistry.GetRegisteredUnits(registeredRoamControllers);
    }

    private void InsertAutonomousRoamCandidate(Vector2 destination, float score)
    {
        float dedupeRadius = Mathf.Max(
            minimumMoveDistance * 0.45f,
            GetColliderRadius() + GetContactShellPadding());
        for (int index = 0; index < autonomousRoamCandidates.Count; index++)
        {
            if (Vector2.Distance(autonomousRoamCandidates[index].Destination, destination) > dedupeRadius)
            {
                continue;
            }

            if (score <= autonomousRoamCandidates[index].Score)
            {
                return;
            }

            autonomousRoamCandidates.RemoveAt(index);
            break;
        }

        AutonomousRoamCandidate candidate = new AutonomousRoamCandidate(destination, score);
        int insertIndex = 0;
        while (insertIndex < autonomousRoamCandidates.Count &&
               autonomousRoamCandidates[insertIndex].Score >= score)
        {
            insertIndex++;
        }

        autonomousRoamCandidates.Insert(insertIndex, candidate);
    }

    private bool TryResolveAvoidanceBlockingSnapshot(
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        out NavigationAgentSnapshot snapshot)
    {
        return TryResolveBlockingSnapshot(avoidance.BlockingAgentId, nearbyNavigationAgents, out snapshot) ||
               TryResolveCurrentBlockingSnapshot(avoidance.BlockingAgentId, out snapshot);
    }

    private bool ShouldRerouteAroundPassiveNpcBlocker(
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        NavigationLocalAvoidanceSolver.CloseRangeConstraintResult closeRangeConstraint)
    {
        if (!avoidance.HasBlockingAgent ||
            !TryResolveAvoidanceBlockingSnapshot(avoidance, out NavigationAgentSnapshot blocker) ||
            blocker.UnitType != NavigationUnitType.NPC ||
            blocker.IsCurrentlyMoving ||
            sharedAvoidanceBlockingFrames < PASSIVE_NPC_BLOCKER_REROUTE_MIN_SIGHTINGS)
        {
            return false;
        }

        if (closeRangeConstraint.HardBlocked)
        {
            return true;
        }

        float blockerDistance = Mathf.Min(
            avoidance.BlockingDistance,
            Vector2.Distance(GetNavigationCenter(), blocker.Position));
        return blockerDistance <= PASSIVE_NPC_BLOCKER_REROUTE_MAX_DISTANCE &&
            closeRangeConstraint.Clearance <= PASSIVE_NPC_BLOCKER_REROUTE_MAX_CLEARANCE;
    }

    private Vector2 ConstrainNextPositionToNavigationBounds(Vector2 currentPosition, Vector2 desiredNextPosition)
    {
        if (!enforceNavGridBounds)
        {
            return desiredNextPosition;
        }

        if (!TryResolveNavGrid(logIfMissing: false))
        {
            return desiredNextPosition;
        }

        if (ShouldUseReleasedMovementBodyClearanceExecutionContract())
        {
            return NavigationTraversalCore.ConstrainNextPositionToNavigationBoundsWithClearanceMargin(
                GetTraversalContract(),
                currentPosition,
                desiredNextPosition,
                CONSTRAINED_ADVANCE_FALLBACK_EXTRA_DISTANCE,
                GetReleasedMovementStepBodyClearanceExtraRadius(),
                RELEASED_STEP_BODY_CLEARANCE_SIDE_INSET_SCALE,
                RELEASED_STEP_BODY_CLEARANCE_VERTICAL_INSET_SCALE);
        }

        return NavigationTraversalCore.ConstrainNextPositionToNavigationBounds(
            GetTraversalContract(),
            currentPosition,
            desiredNextPosition,
            CONSTRAINED_ADVANCE_FALLBACK_EXTRA_DISTANCE);
    }

    private bool TryResolveConstrainedFallbackPosition(
        Vector2 currentPosition,
        Vector2 desiredNextPosition,
        Vector2 desiredOffset,
        out Vector2 fallbackPosition)
    {
        fallbackPosition = currentPosition;
        if (!TryResolveNavGrid(logIfMissing: false))
        {
            return false;
        }

        if (!navGrid.TryFindNearestWalkable(desiredNextPosition, out Vector2 nearestWalkable, navigationCollider))
        {
            return false;
        }

        float fallbackDistance = Vector2.Distance(currentPosition, nearestWalkable);
        float maxFallbackDistance = desiredOffset.magnitude + CONSTRAINED_ADVANCE_FALLBACK_EXTRA_DISTANCE;
        if (fallbackDistance <= 0.0001f || fallbackDistance > maxFallbackDistance)
        {
            return false;
        }

        if (!CanOccupyNavigationPoint(nearestWalkable))
        {
            return false;
        }

        fallbackPosition = nearestWalkable;
        return true;
    }

    private Vector2 ConstrainNextPositionByAxes(Vector2 currentPosition, Vector2 desiredOffset, bool tryXFirst)
    {
        Vector2 constrainedPosition = currentPosition;
        TryApplyAxisOffset(ref constrainedPosition, currentPosition, desiredOffset, applyX: tryXFirst);
        TryApplyAxisOffset(ref constrainedPosition, currentPosition, desiredOffset, applyX: !tryXFirst);
        return constrainedPosition;
    }

    private void TryApplyAxisOffset(ref Vector2 constrainedPosition, Vector2 currentPosition, Vector2 desiredOffset, bool applyX)
    {
        float axisOffset = applyX ? desiredOffset.x : desiredOffset.y;
        if (Mathf.Abs(axisOffset) <= 0.0001f)
        {
            return;
        }

        Vector2 offset = applyX
            ? new Vector2(axisOffset, 0f)
            : new Vector2(0f, axisOffset);
        Vector2 candidatePosition = constrainedPosition + offset;
        if (!CanOccupyNavigationPoint(candidatePosition))
        {
            return;
        }

        constrainedPosition = candidatePosition;
    }

    private bool CanOccupyNavigationPoint(Vector2 worldCenter)
    {
        if (!TryResolveNavGrid(logIfMissing: false))
        {
            return true;
        }

        return NavigationTraversalCore.CanOccupyNavigationPoint(GetTraversalContract(), worldCenter);
    }

    private void UpdateTraversalSoftPassState(Vector2 worldCenter)
    {
        bool shouldSoftPass = ShouldEnableTraversalSoftPass(worldCenter);
        if (shouldSoftPass == isTraversalSoftPassActive)
        {
            return;
        }

        ApplyTraversalSoftPass(traversalSoftPassBlockers, shouldSoftPass);
        isTraversalSoftPassActive = shouldSoftPass;
    }

    private void UpdateTraversalSoftPassStateOncePerFrame(Vector2 worldCenter)
    {
        if (lastTraversalSoftPassUpdateFrame == Time.frameCount)
        {
            return;
        }

        lastTraversalSoftPassUpdateFrame = Time.frameCount;
        UpdateTraversalSoftPassState(worldCenter);
    }

    private bool ShouldEnableTraversalSoftPass(Vector2 worldCenter)
    {
        if (!TryResolveNavGrid(logIfMissing: false))
        {
            return false;
        }

        return NavigationTraversalCore.ShouldEnableTraversalSoftPass(
            GetTraversalContract(),
            worldCenter,
            allowTraversalOverridePhysicsSoftPass,
            traversalSoftPassBlockers);
    }

    private NavigationTraversalCore.Contract GetTraversalContract()
    {
        return new NavigationTraversalCore.Contract(
            navGrid,
            navigationCollider,
            navigationFootProbeVerticalInset,
            navigationFootProbeSideInset,
            navigationFootProbeExtraRadius);
    }

    private void ApplyTraversalSoftPass(Collider2D[] colliders, bool shouldIgnore)
    {
        if (navigationCollider == null || colliders == null)
        {
            return;
        }

        for (int index = 0; index < colliders.Length; index++)
        {
            Collider2D blocker = colliders[index];
            if (blocker == null || blocker == navigationCollider)
            {
                continue;
            }

            Physics2D.IgnoreCollision(navigationCollider, blocker, shouldIgnore);
        }
    }

    private void ClearTraversalSoftPassState()
    {
        if (!isTraversalSoftPassActive)
        {
            return;
        }

        ApplyTraversalSoftPass(traversalSoftPassBlockers, false);
        isTraversalSoftPassActive = false;
    }

    private static Collider2D[] NormalizeColliderArray(Collider2D[] source)
    {
        if (source == null || source.Length == 0)
        {
            return new Collider2D[0];
        }

        List<Collider2D> normalized = new List<Collider2D>(source.Length);
        for (int index = 0; index < source.Length; index++)
        {
            Collider2D collider = source[index];
            if (collider == null || normalized.Contains(collider))
            {
                continue;
            }

            normalized.Add(collider);
        }

        return normalized.ToArray();
    }

    private Vector2 GetFootProbeCenter(Vector2 worldCenter)
    {
        if (navigationCollider == null)
        {
            return worldCenter;
        }

        Bounds bounds = navigationCollider.bounds;
        Vector2 currentCenter = bounds.center;
        float footProbeInset = Mathf.Clamp(navigationFootProbeVerticalInset, 0f, bounds.size.y * 0.45f);
        float footProbeY = bounds.min.y + footProbeInset;
        return worldCenter + new Vector2(0f, footProbeY - currentCenter.y);
    }

    private float GetLateralFootProbeOffset()
    {
        if (navigationCollider == null)
        {
            return 0f;
        }

        Bounds bounds = navigationCollider.bounds;
        float sideInset = Mathf.Clamp(navigationFootProbeSideInset, 0f, bounds.extents.x);
        return Mathf.Max(0.02f, bounds.extents.x - sideInset);
    }

    private void GetTraversalSupportProbePoints(
        Vector2 worldCenter,
        out Vector2 centerProbe,
        out Vector2 leftProbe,
        out Vector2 rightProbe)
    {
        centerProbe = GetFootProbeCenter(worldCenter);
        float lateralOffset = GetLateralFootProbeOffset() * 0.72f;
        if (lateralOffset <= 0.03f)
        {
            leftProbe = centerProbe;
            rightProbe = centerProbe;
            return;
        }

        leftProbe = centerProbe + Vector2.left * lateralOffset;
        rightProbe = centerProbe + Vector2.right * lateralOffset;
    }

    private float GetTraversalSupportQueryRadius(float navigationQueryRadius)
    {
        return Mathf.Max(0.03f, navigationQueryRadius * 0.85f);
    }

    private bool IsTraversalBridgeCenterSupported(Vector2 footProbeCenter, float navigationQueryRadius)
    {
        float supportRadius = GetTraversalSupportQueryRadius(navigationQueryRadius);
        return navGrid != null && navGrid.HasWalkableOverrideAt(footProbeCenter, supportRadius);
    }

    private float GetNavigationPointQueryRadius()
    {
        float fallbackRadius = Mathf.Max(0.04f, navigationFootProbeSideInset + navigationFootProbeExtraRadius);
        if (!TryResolveNavGrid(logIfMissing: false))
        {
            return fallbackRadius;
        }

        float maxRadius = navGrid.GetAgentRadius();
        if (navigationCollider == null)
        {
            return Mathf.Min(maxRadius, fallbackRadius);
        }

        Bounds bounds = navigationCollider.bounds;
        float sideRadius = Mathf.Max(0.03f, navigationFootProbeSideInset + navigationFootProbeExtraRadius);
        float verticalRadius = Mathf.Max(0.03f, navigationFootProbeVerticalInset + navigationFootProbeExtraRadius);
        float clampedRadius = Mathf.Min(Mathf.Min(bounds.extents.x, bounds.extents.y), Mathf.Max(sideRadius, verticalRadius));
        return Mathf.Min(maxRadius, clampedRadius);
    }

    private void EnterShortPause(bool countTowardLongPause)
    {
        ClearMoveDecisionCache();
        BreakAmbientChatLink(hideBubble: true);
        ClearAmbientChatRetry();
        ClearRequestedDestination();
        state = RoamState.ShortPause;
        stateTimer = Random.Range(shortPauseMin, shortPauseMax);
        path.Clear();
        currentPathIndex = 0;
        ResetMovementRecovery(GetNavigationCenter(), resetCounter: true);

        if (countTowardLongPause)
        {
            completedShortPauseCount++;
        }

        if (motionController != null)
        {
            motionController.StopMotion();
        }

        if (bubblePresenter != null)
        {
            bubblePresenter.HideBubble();
        }

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=yellow>[NPCAutoRoamController]</color> {name} 进入短停 => Timer={stateTimer:F2}s, Count={completedShortPauseCount}/{longPauseTriggerCount}",
                this);
        }
    }

    private void FinishMoveCycle(bool countTowardLongPause, bool reachedDestination)
    {
        if (debugMoveActive)
        {
            EndDebugMove(reachedDestination);
            return;
        }

        EnterShortPause(countTowardLongPause);
    }

    private void EndDebugMove(bool reachedDestination)
    {
        PointToPointTravelContract completedTravelContract = activePointToPointTravelContract;
        bool formalNavigationArrived = completedTravelContract == PointToPointTravelContract.FormalNavigation && reachedDestination;
        Vector2 finalPosition = GetNavigationCenter();
        debugMoveActive = false;
        activePointToPointTravelContract = PointToPointTravelContract.None;
        residentScriptedMovePaused = false;
        if (IsResidentScriptedControlActive)
        {
            HaltResidentScriptedMovement();
        }
        else
        {
            StopRoam();
        }

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=yellow>[NPCAutoRoamController]</color> {name} PointToPointTravel {(reachedDestination ? "completed" : "stopped")} => Contract={completedTravelContract}, Position={(rb != null ? rb.position : (Vector2)transform.position)}, Destination={currentDestination}",
                this);
        }

        if (formalNavigationArrived)
        {
            lastFormalNavigationArrivalTime = Time.time;
            lastFormalNavigationArrivalPosition = finalPosition;
        }
    }

    private void EnterLongPause()
    {
        ClearMoveDecisionCache();
        BreakAmbientChatLink(hideBubble: true);
        ClearAmbientChatRetry();
        ClearRequestedDestination();
        state = RoamState.LongPause;
        stateTimer = Random.Range(longPauseMin, longPauseMax);
        path.Clear();
        currentPathIndex = 0;
        ResetMovementRecovery(GetNavigationCenter(), resetCounter: true);

        if (motionController != null)
        {
            motionController.StopMotion();
        }

        if (!TryStartAmbientChat(stateTimer, out AmbientChatAttemptResult attemptResult) && bubblePresenter != null)
        {
            if (ShouldRetryAmbientChat(attemptResult))
            {
                ScheduleAmbientChatRetry(resetCounter: true);
            }

            if (!NpcInteractionPriorityPolicy.ShouldSuppressAmbientBubbleForCurrentStory())
            {
                TryShowResidentSelfTalk(Mathf.Max(1f, stateTimer - 0.15f));
            }
        }

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=cyan>[NPCAutoRoamController]</color> {name} 进入长停 => Timer={stateTimer:F2}s",
                this);
        }
    }

    private Vector2 GetRoamCenter()
    {
        EnsureRuntimeHomeAnchorBound();
        return homePosition;
    }

    private bool TryStartAmbientChat(float duration)
    {
        return TryStartAmbientChat(duration, out _);
    }

    private bool TryStartAmbientChat(float duration, out AmbientChatAttemptResult attemptResult)
    {
        attemptResult = AmbientChatAttemptResult.Disabled;

        if (!enableAmbientChat)
        {
            lastAmbientDecision = "ambient chat disabled";
            return false;
        }

        if (bubblePresenter == null)
        {
            attemptResult = AmbientChatAttemptResult.BubblePresenterMissing;
            lastAmbientDecision = "bubble presenter missing";
            return false;
        }

        if (!HasAmbientChatInitiatorContent())
        {
            attemptResult = AmbientChatAttemptResult.InitiatorLinesMissing;
            lastAmbientDecision = "initiator lines missing";
            return false;
        }

        if (ambientChatRadius <= 0f)
        {
            attemptResult = AmbientChatAttemptResult.RadiusInvalid;
            lastAmbientDecision = "ambient radius <= 0";
            return false;
        }

        if (Random.value > ambientChatChance)
        {
            attemptResult = AmbientChatAttemptResult.ChanceMissed;
            lastAmbientDecision = "ambient chat roll missed";
            return false;
        }

        NPCAutoRoamController partner = FindAmbientChatPartner();
        if (partner == null || !partner.TryJoinAmbientChat(this, duration))
        {
            attemptResult = partner == null
                ? AmbientChatAttemptResult.NoNearbyPartner
                : AmbientChatAttemptResult.PartnerRejected;
            lastAmbientDecision = partner == null
                ? "no nearby partner"
                : $"partner rejected: {partner.name}";
            return false;
        }

        chatPartner = partner;
        ClearAmbientChatRetry();
        attemptResult = AmbientChatAttemptResult.Success;
        lastAmbientDecision = $"chatting with {partner.name}";
        FaceToward(partner.transform.position - transform.position);
        partner.FaceToward(transform.position - partner.transform.position);

        string[] initiatorLines = ResolveAmbientChatLinesForPartner(partner, initiator: true);
        string[] responderLines = partner.ResolveAmbientChatLinesForPartner(this, initiator: false);

        StartAmbientChatRoutine(initiatorLines, delay: 0f, duration);
        partner.StartAmbientChatRoutine(responderLines, ambientChatResponseDelay, duration);

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=magenta>[NPCAutoRoamController]</color> {name} 与 {partner.name} 开始聊天 => Duration={duration:F2}s",
                this);
        }

        return true;
    }

    private NPCAutoRoamController FindAmbientChatPartner()
    {
        float bestDistanceSqr = ambientChatRadius * ambientChatRadius;
        NPCAutoRoamController bestPartner = null;
        EnsureRegisteredRoamControllerBufferCurrentFrame();
        for (int index = 0; index < registeredRoamControllers.Count; index++)
        {
            NPCAutoRoamController candidate = registeredRoamControllers[index];
            if (candidate == null ||
                candidate == this ||
                !candidate.gameObject.scene.IsValid() ||
                candidate.gameObject.scene != gameObject.scene ||
                !CanUseAsAmbientChatPartner(candidate))
            {
                continue;
            }

            float distanceSqr = (candidate.transform.position - transform.position).sqrMagnitude;
            if (distanceSqr > bestDistanceSqr)
            {
                continue;
            }

            bestDistanceSqr = distanceSqr;
            bestPartner = candidate;
        }

        return bestPartner;
    }

    private bool CanUseAsAmbientChatPartner(NPCAutoRoamController candidate)
    {
        return candidate != null && candidate.CanJoinAmbientChat(this);
    }

    private bool CanJoinAmbientChat(NPCAutoRoamController requester)
    {
        if (requester == null || requester == this)
        {
            return false;
        }

        if (!isActiveAndEnabled || !gameObject.activeInHierarchy)
        {
            return false;
        }

        if (!enableAmbientChat ||
            bubblePresenter == null ||
            !HasAmbientChatResponderContent() ||
            chatPartner != null)
        {
            return false;
        }

        return state is RoamState.ShortPause or RoamState.LongPause;
    }

    private bool TryJoinAmbientChat(NPCAutoRoamController requester, float duration)
    {
        if (!CanJoinAmbientChat(requester))
        {
            return false;
        }

        BreakAmbientChatLink(hideBubble: true);
        chatPartner = requester;
        lastAmbientDecision = $"joined chat with {requester.name}";
        state = RoamState.LongPause;
        stateTimer = Mathf.Max(duration, ambientChatResponseDelay + 1f);
        path.Clear();
        currentPathIndex = 0;
        ResetMovementRecovery(GetNavigationCenter(), resetCounter: true);

        if (motionController != null)
        {
            motionController.StopMotion();
        }

        return true;
    }

    private void StartAmbientChatRoutine(string[] lines, float delay, float duration)
    {
        StopAmbientChatRoutine();
        if (bubblePresenter == null || !HasBubbleLines(lines))
        {
            return;
        }

        ambientChatCoroutine = StartCoroutine(PlayAmbientChatBubble(lines, delay, duration));
    }

    private void StopAmbientChatRoutine()
    {
        if (ambientChatCoroutine == null)
        {
            return;
        }

        StopCoroutine(ambientChatCoroutine);
        ambientChatCoroutine = null;
    }

    private IEnumerator PlayAmbientChatBubble(string[] lines, float delay, float duration)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        ambientChatCoroutine = null;
        string selectedLine = SelectAmbientChatLine(lines);
        if (string.IsNullOrWhiteSpace(selectedLine))
        {
            yield break;
        }

        float bubbleDuration = Mathf.Clamp(duration * 0.45f, 1.2f, 2.4f);
        for (int attempt = 0; attempt < AmbientBubbleMaxShowAttempts; attempt++)
        {
            StoryPhase currentPhase = NpcInteractionPriorityPolicy.ResolveCurrentStoryPhase();
            if (!CanShowAmbientBubbleNow(lines, currentPhase))
            {
                yield break;
            }

            if (bubblePresenter.ShowText(selectedLine, bubbleDuration))
            {
                yield break;
            }

            if (attempt >= AmbientBubbleMaxShowAttempts - 1)
            {
                yield break;
            }

            yield return new WaitForSeconds(AmbientBubbleRetryDelay);
        }
    }

    private bool CanShowAmbientBubbleNow(string[] lines, StoryPhase currentPhase)
    {
        return isActiveAndEnabled &&
               bubblePresenter != null &&
               HasBubbleLines(lines) &&
               chatPartner != null &&
               !NpcInteractionPriorityPolicy.ShouldSuppressAmbientBubble(currentPhase);
    }

    private void BreakAmbientChatLink(bool hideBubble)
    {
        StopAmbientChatRoutine();
        ClearAmbientChatRetry();

        bool preserveOwnPriorityBubble =
            hideBubble
            && bubblePresenter != null
            && !ShouldSuspendForDialogue()
            && IsResidentScriptedControlActive
            && !IsResidentScriptedMoveActive
            && bubblePresenter.IsConversationPriorityVisible;

        if (hideBubble && bubblePresenter != null && !preserveOwnPriorityBubble)
        {
            bubblePresenter.HideBubble();
        }

        NPCAutoRoamController partner = chatPartner;
        chatPartner = null;

        if (partner != null && partner.chatPartner == this)
        {
            partner.chatPartner = null;
            partner.StopAmbientChatRoutine();
            bool preservePartnerPriorityBubble =
                hideBubble
                && partner.bubblePresenter != null
                && !ShouldSuspendForDialogue()
                && partner.IsResidentScriptedControlActive
                && !partner.IsResidentScriptedMoveActive
                && partner.bubblePresenter.IsConversationPriorityVisible;

            if (hideBubble && partner.bubblePresenter != null && !preservePartnerPriorityBubble)
            {
                partner.bubblePresenter.HideBubble();
            }
        }
    }

    private void FaceToward(Vector2 direction)
    {
        if (motionController == null || direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        motionController.SetFacingDirection(direction);
    }

    private static bool HasBubbleLines(string[] lines)
    {
        if (lines == null)
        {
            return false;
        }

        for (int index = 0; index < lines.Length; index++)
        {
            if (!string.IsNullOrWhiteSpace(lines[index]))
            {
                return true;
            }
        }

        return false;
    }

    private static string SelectAmbientChatLine(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            return string.Empty;
        }

        int startIndex = Random.Range(0, lines.Length);
        for (int offset = 0; offset < lines.Length; offset++)
        {
            string line = lines[(startIndex + offset) % lines.Length];
            if (!string.IsNullOrWhiteSpace(line))
            {
                return line.Trim();
            }
        }

        return string.Empty;
    }

    private bool TryShowResidentSelfTalk(float duration)
    {
        if (bubblePresenter == null)
        {
            return false;
        }

        StoryPhase currentPhase = NpcInteractionPriorityPolicy.ResolveCurrentStoryPhase();
        string[] contentLines = roamProfile != null
            ? roamProfile.GetSelfTalkLines(currentPhase)
            : null;

        if (HasBubbleLines(contentLines))
        {
            string selectedLine = SelectAmbientChatLine(contentLines);
            if (!string.IsNullOrWhiteSpace(selectedLine))
            {
                return bubblePresenter.ShowText(selectedLine, duration);
            }
        }

        return bubblePresenter.ShowRandomSelfTalk(duration);
    }

    private bool HasAmbientChatInitiatorContent()
    {
        return roamProfile != null
            ? roamProfile.HasAmbientChatInitiatorContent
            : HasBubbleLines(chatInitiatorLines);
    }

    private bool HasAmbientChatResponderContent()
    {
        return roamProfile != null
            ? roamProfile.HasAmbientChatResponderContent
            : HasBubbleLines(chatResponderLines);
    }

    private string[] ResolveAmbientChatLinesForPartner(NPCAutoRoamController partner, bool initiator)
    {
        if (roamProfile != null)
        {
            string partnerNpcId = partner != null ? partner.ResolveNpcId() : string.Empty;
            string[] contentLines = roamProfile.GetAmbientChatLines(partnerNpcId, initiator);
            if (HasBubbleLines(contentLines))
            {
                return CopyLines(contentLines);
            }
        }

        return initiator ? CopyLines(chatInitiatorLines) : CopyLines(chatResponderLines);
    }

    private string ResolveNpcId()
    {
        return roamProfile != null
            ? roamProfile.ResolveNpcId(name)
            : NPCDialogueContentProfile.NormalizeNpcId(name);
    }

    private void ResetLongPauseTriggerCount()
    {
        longPauseTriggerCount = Random.Range(shortPauseCountMin, shortPauseCountMax + 1);
    }

    private void ScheduleAmbientChatRetry()
    {
        ScheduleAmbientChatRetry(resetCounter: false);
    }

    private void ScheduleAmbientChatRetry(bool resetCounter)
    {
        if (resetCounter)
        {
            ambientChatRetryCount = 0;
        }

        if (!enableAmbientChat || ambientChatRetryPending || state != RoamState.LongPause)
        {
            return;
        }

        if (NpcInteractionPriorityPolicy.ShouldSuppressAmbientBubbleForCurrentStory())
        {
            return;
        }

        if (ambientChatRetryCount >= AmbientChatMaxRetryCount)
        {
            return;
        }

        if (stateTimer < AmbientChatRetryMinRemainingTime)
        {
            return;
        }

        ambientChatRetryPending = true;
        ambientChatRetryAtTime = Time.time + Mathf.Min(
            Random.Range(AmbientChatRetryDelay * 0.85f, AmbientChatRetryDelay * 1.15f),
            stateTimer * 0.35f);
    }

    private void TryExecutePendingAmbientChatRetry()
    {
        if (!ambientChatRetryPending || state != RoamState.LongPause || chatPartner != null)
        {
            return;
        }

        if (NpcInteractionPriorityPolicy.ShouldSuppressAmbientBubbleForCurrentStory())
        {
            ClearAmbientChatRetry();
            return;
        }

        if (Time.time < ambientChatRetryAtTime || stateTimer < AmbientChatRetryMinRemainingTime)
        {
            return;
        }

        ambientChatRetryPending = false;
        ambientChatRetryCount++;

        if (TryStartAmbientChat(stateTimer, out AmbientChatAttemptResult attemptResult))
        {
            return;
        }

        if (ShouldRetryAmbientChat(attemptResult))
        {
            ScheduleAmbientChatRetry();
        }
    }

    private void ClearAmbientChatRetry()
    {
        ambientChatRetryPending = false;
        ambientChatRetryAtTime = 0f;
        ambientChatRetryCount = 0;
    }

    private void NoteSuccessfulAdvance(Vector2 currentPosition)
    {
        blockedAdvanceFrames = 0;
        consecutivePathBuildFailures = 0;
        nextPathBuildAllowedTime = Time.time;
        lastBlockedAdvancePosition = currentPosition;
        consecutiveTerminalStuckCount = 0;
        lastTerminalStuckPosition = currentPosition;
        ResetMoveCommandOscillationState(currentPosition);
        lastMoveSkipReason = "AdvanceConfirmed";
    }

    private bool CanReuseStoppedMoveDecisionThisFrame(Vector2 currentPosition)
    {
        if (rb != null &&
            IsWithinStopDecisionReuseWindow())
        {
            return !hasCachedMoveDecisionVelocity &&
                Vector2.Distance(currentPosition, currentDestination) > destinationTolerance;
        }

        return lastHeavyMoveDecisionFrame == Time.frameCount &&
            !hasCachedMoveDecisionVelocity &&
            Vector2.Distance(currentPosition, currentDestination) > destinationTolerance;
    }

    private bool CanReuseHeavyMoveDecisionThisFrame(
        NavigationPathExecutor2D.WaypointResult waypointState,
        Vector2 currentPosition,
        float deltaTime)
    {
        if (!hasCachedMoveDecisionVelocity)
        {
            return false;
        }

        if (!waypointState.HasWaypoint || waypointState.ReachedPathEnd)
        {
            return false;
        }

        bool isSameFrameReuse = lastHeavyMoveDecisionFrame == Time.frameCount;
        if (!isSameFrameReuse &&
            !CanReuseStableMoveDecisionAcrossFrames(waypointState, currentPosition, deltaTime))
        {
            return false;
        }

        float maxReusableStep = cachedMoveDecisionVelocity.magnitude * Mathf.Max(deltaTime, 0.0001f);
        float reuseThreshold = Mathf.Max(destinationTolerance, maxReusableStep + FRAME_REUSE_WAYPOINT_DISTANCE_PADDING);
        if (waypointState.DistanceToWaypoint <= reuseThreshold)
        {
            return false;
        }

        // Physics-driven NPCs can hit TickMoving multiple times in the same rendered frame
        // when Unity catches up on FixedUpdate. Reusing the already computed decision keeps
        // the visible direction stable and avoids re-running the expensive avoidance/repath
        // chain several times before the frame is even presented.
        if (rb != null)
        {
            return (isSameFrameReuse
                    ? IsWithinPhysicsHeavyDecisionReuseWindow()
                    : IsWithinScriptedMoveDecisionReuseWindow()) &&
                !hasPendingMoveCommandProgressCheck &&
                blockedAdvanceFrames <= 0 &&
                sharedAvoidanceBlockingFrames <= 0 &&
                !hasDynamicDetour &&
                Vector2.Distance(currentPosition, currentDestination) > destinationTolerance;
        }

        if (hasPendingMoveCommandProgressCheck ||
            blockedAdvanceFrames > 0 ||
            sharedAvoidanceBlockingFrames > 0 ||
            hasDynamicDetour)
        {
            return false;
        }

        return Vector2.Distance(currentPosition, currentDestination) > destinationTolerance;
    }

    private bool CanReuseStableMoveDecisionAcrossFrames(
        NavigationPathExecutor2D.WaypointResult waypointState,
        Vector2 currentPosition,
        float deltaTime)
    {
        if (!HasActivePointToPointTravelContract())
        {
            return false;
        }

        if (!IsWithinScriptedMoveDecisionReuseWindow())
        {
            return false;
        }

        if (hasPendingMoveCommandProgressCheck ||
            blockedAdvanceFrames > 0 ||
            sharedAvoidanceBlockingFrames > 0 ||
            hasDynamicDetour)
        {
            return false;
        }

        Vector2 toWaypoint = waypointState.Waypoint - currentPosition;
        if (toWaypoint.sqrMagnitude <= 0.0001f)
        {
            return false;
        }

        Vector2 cachedDirection = cachedMoveDecisionVelocity.normalized;
        Vector2 waypointDirection = toWaypoint.normalized;
        if (Vector2.Dot(cachedDirection, waypointDirection) < 0.92f)
        {
            return false;
        }

        float maxReusableStep = cachedMoveDecisionVelocity.magnitude * Mathf.Max(deltaTime, 0.0001f);
        return waypointState.DistanceToWaypoint > maxReusableStep + FRAME_REUSE_WAYPOINT_DISTANCE_PADDING;
    }

    private void ApplyCachedMoveVelocity(Vector2 currentPosition, float deltaTime)
    {
        ApplyMoveVelocity(currentPosition, cachedMoveDecisionVelocity, cachedMoveFacingDirection, deltaTime);
    }

    private void ApplyStoppedMoveDecision()
    {
        if (motionController != null)
        {
            motionController.StopMotion();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void ApplyMoveVelocity(Vector2 currentPosition, Vector2 velocity, Vector2 facingDirection, float deltaTime)
    {
        Vector2 committedFacingDirection = ResolveCommittedFacingDirection(velocity, facingDirection);
        motionController.SetExternalVelocity(velocity);
        motionController.SetExternalFacingDirection(committedFacingDirection);
        if (rb != null)
        {
            if (rb.bodyType == RigidbodyType2D.Dynamic)
            {
                rb.linearVelocity = velocity;
            }
            else
            {
                Vector2 nextPosition = currentPosition + velocity * Mathf.Max(deltaTime, 0.0001f);
                rb.MovePosition(nextPosition);
            }
        }
        else
        {
            Vector2 nextPosition = currentPosition + velocity * Mathf.Max(deltaTime, 0.0001f);
            transform.position = new Vector3(nextPosition.x, nextPosition.y, transform.position.z);
        }
    }

    private Vector2 ResolveFacingDirectionForMoveCommand(Vector2 desiredDirection, Vector2 adjustedDirection)
    {
        if (desiredDirection.sqrMagnitude > 0.0001f)
        {
            return desiredDirection.normalized;
        }

        if (adjustedDirection.sqrMagnitude > 0.0001f)
        {
            return adjustedDirection.normalized;
        }

        if (lastIssuedMoveDirection.sqrMagnitude > 0.0001f)
        {
            return lastIssuedMoveDirection;
        }

        return Vector2.zero;
    }

    private static Vector2 ResolveCommittedFacingDirection(Vector2 velocity, Vector2 fallbackFacingDirection)
    {
        if (velocity.sqrMagnitude > 0.0001f)
        {
            return velocity.normalized;
        }

        if (fallbackFacingDirection.sqrMagnitude > 0.0001f)
        {
            return fallbackFacingDirection.normalized;
        }

        return Vector2.zero;
    }

    private void CacheMoveDecisionVelocityForCurrentFrame(Vector2 velocity, Vector2 facingDirection)
    {
        lastHeavyMoveDecisionFrame = Time.frameCount;
        lastHeavyMoveDecisionTime = Time.time;
        hasCachedMoveDecisionVelocity = true;
        cachedMoveDecisionVelocity = velocity;
        cachedMoveFacingDirection = ResolveCommittedFacingDirection(velocity, facingDirection);
    }

    private void CacheStoppedMoveDecisionForCurrentFrame()
    {
        lastHeavyMoveDecisionFrame = Time.frameCount;
        lastHeavyMoveDecisionTime = Time.time;
        hasCachedMoveDecisionVelocity = false;
        cachedMoveDecisionVelocity = Vector2.zero;
        cachedMoveFacingDirection = Vector2.zero;
    }

    private void ClearMoveDecisionCache()
    {
        lastHeavyMoveDecisionFrame = -1;
        lastHeavyMoveDecisionTime = float.NegativeInfinity;
        hasCachedMoveDecisionVelocity = false;
        cachedMoveDecisionVelocity = Vector2.zero;
        cachedMoveFacingDirection = Vector2.zero;
    }

    private bool IsWithinPhysicsHeavyDecisionReuseWindow()
    {
        if (lastHeavyMoveDecisionTime <= float.NegativeInfinity)
        {
            return false;
        }

        float elapsed = Time.time - lastHeavyMoveDecisionTime;
        return elapsed >= 0f && elapsed <= PHYSICS_HEAVY_DECISION_REUSE_SECONDS;
    }

    private bool IsWithinScriptedMoveDecisionReuseWindow()
    {
        if (lastHeavyMoveDecisionTime <= float.NegativeInfinity)
        {
            return false;
        }

        float elapsed = Time.time - lastHeavyMoveDecisionTime;
        return elapsed >= 0f && elapsed <= SCRIPTED_MOVE_DECISION_REUSE_SECONDS;
    }

    private bool IsWithinStopDecisionReuseWindow()
    {
        if (lastHeavyMoveDecisionTime <= float.NegativeInfinity)
        {
            return false;
        }

        float elapsed = Time.time - lastHeavyMoveDecisionTime;
        float reuseWindow = ShouldUseExtendedStopDecisionReuseWindow()
            ? NPC_BAD_CASE_STOP_DECISION_REUSE_SECONDS
            : PHYSICS_HEAVY_DECISION_REUSE_SECONDS;
        return elapsed >= 0f && elapsed <= reuseWindow;
    }

    private bool ShouldUseExtendedStopDecisionReuseWindow()
    {
        return hasPendingMoveCommandProgressCheck ||
            blockedAdvanceFrames > 0 ||
            sharedAvoidanceBlockingFrames > 0 ||
            hasDynamicDetour;
    }

    private bool TryAcquirePathBuildBudget()
    {
        if (lastPathBuildAttemptFrame == Time.frameCount)
        {
            return false;
        }

        if (Time.time < nextPathBuildAllowedTime)
        {
            return false;
        }

        lastPathBuildAttemptFrame = Time.frameCount;
        return true;
    }

    private void RecordPathBuildOutcome(bool success)
    {
        if (success)
        {
            consecutivePathBuildFailures = 0;
            nextPathBuildAllowedTime = Time.time;
            return;
        }

        consecutivePathBuildFailures = Mathf.Min(consecutivePathBuildFailures + 1, 16);
        int backoffStep = Mathf.Min(consecutivePathBuildFailures - 1, PATH_BUILD_FAILURE_BACKOFF_STEP_CAP);
        float cooldown = PATH_BUILD_FAILURE_BASE_COOLDOWN * Mathf.Pow(2f, backoffStep);
        nextPathBuildAllowedTime = Time.time + Mathf.Min(PATH_BUILD_FAILURE_MAX_COOLDOWN, cooldown);
    }

    private void RefreshProgressCheckpoint(Vector2 currentPosition, bool resetCounter)
    {
        lastProgressCheckPosition = currentPosition;
        lastProgressCheckTime = Time.time;
        lastProgressDistance = 0f;
        if (resetCounter)
        {
            currentStuckRecoveryCount = 0;
        }
    }

    private bool IsDynamicAvoidanceBlocker(NavigationLocalAvoidanceSolver.AvoidanceResult avoidance)
    {
        if (!avoidance.HasBlockingAgent ||
            !TryResolveAvoidanceBlockingSnapshot(avoidance, out NavigationAgentSnapshot blocker))
        {
            return false;
        }

        return blocker.UnitType == NavigationUnitType.Player ||
               blocker.UnitType == NavigationUnitType.Enemy ||
               blocker.IsCurrentlyMoving;
    }

    private bool ShouldResetSharedAvoidanceStuckProgress(
        NavigationLocalAvoidanceSolver.CloseRangeConstraintResult closeRangeConstraint,
        float moveScale,
        bool shouldRepath)
    {
        if (closeRangeConstraint.HardBlocked)
        {
            return false;
        }

        if (closeRangeConstraint.Applied && moveScale <= AVOIDANCE_STUCK_RESET_MAX_MOVE_SCALE)
        {
            return true;
        }

        return shouldRepath && moveScale <= 0.45f;
    }

    private bool TryHandlePassiveAvoidanceHardStop(
        Vector2 currentPosition,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        NavigationLocalAvoidanceSolver.CloseRangeConstraintResult closeRangeConstraint,
        string reason)
    {
        RecordBlockedAdvance(currentPosition);
        if (Time.time - lastBlockedAdvanceRecoveryTime >= BLOCKED_ADVANCE_RECOVERY_COOLDOWN)
        {
            lastBlockedAdvanceRecoveryTime = Time.time;
            if (TryRebuildPath(
                    currentPosition,
                    resetRecoveryCounter: false,
                    reason: $"PassiveHardStop:{reason}",
                    preserveTrafficState: true,
                    preserveBlockedRecoveryState: true))
            {
                RefreshProgressCheckpoint(currentPosition, resetCounter: true);
                return true;
            }

            if (blockedAdvanceFrames >= BLOCKED_ADVANCE_REROUTE_MIN_FRAMES &&
                ShouldRerouteAroundPassiveNpcBlocker(avoidance, closeRangeConstraint) &&
                TryBeginMove(preserveBlockedRecoveryState: true))
            {
                RefreshProgressCheckpoint(currentPosition, resetCounter: true);
                return true;
            }

            if (ShouldEarlyAbortAutonomousRoamMove())
            {
                lastMoveSkipReason = "PassiveAvoidanceStopgap";
                FinishMoveCycle(countTowardLongPause: false, reachedDestination: false);
                return true;
            }
        }

        if (blockedAdvanceFrames >= BLOCKED_ADVANCE_LONG_PAUSE_MIN_FRAMES)
        {
            if (TryInterruptRoamMove(
                    RoamMoveInterruptionReason.SharedAvoidanceRepathFailed,
                    currentPosition,
                    avoidance.BlockingAgentId,
                    avoidance.BlockingAgentPosition,
                    RoamMoveInterruptionBlockerKind.None,
                    $"PassiveHardStop:{reason}",
                    nearbyNavigationAgents))
            {
                return true;
            }

            FinishMoveCycle(countTowardLongPause: false, reachedDestination: false);
            return true;
        }

        return true;
    }

    private bool TryHandleBlockedAdvance(Vector2 currentPosition, string reason)
    {
        RecordBlockedAdvance(currentPosition);
        if (IsRecoverablePointToPointTravelActive())
        {
            return TryHandleRecoverablePointToPointTravelBlockedAdvance(currentPosition, reason);
        }

        if (blockedAdvanceFrames >= STATIC_RECOVERY_DETOUR_MIN_BLOCKED_FRAMES &&
            TryCreateStaticObstacleRecoveryPath(currentPosition, reason))
        {
            lastMoveSkipReason = $"BlockedAdvanceStaticRecovery:{reason}";
            RefreshProgressCheckpoint(currentPosition, resetCounter: true);
            return true;
        }

        if (ShouldAbortBlockedAdvanceWithoutRebuild(reason))
        {
            if (TryRetargetAutonomousRoamAfterStaticBlock(currentPosition, reason))
            {
                return true;
            }

            lastMoveSkipReason = "BlockedAdvanceStaticAbort";
            FinishMoveCycle(countTowardLongPause: false, reachedDestination: false);
            return true;
        }

        if (Time.time - lastBlockedAdvanceRecoveryTime < BLOCKED_ADVANCE_RECOVERY_COOLDOWN)
        {
            return true;
        }

        lastBlockedAdvanceRecoveryTime = Time.time;
        if (TryRebuildPath(
                currentPosition,
                resetRecoveryCounter: false,
                reason: $"BlockedAdvance:{reason}",
                preserveTrafficState: true,
                preserveBlockedRecoveryState: true))
        {
            RefreshProgressCheckpoint(currentPosition, resetCounter: true);
            return true;
        }

        if (blockedAdvanceFrames >= BLOCKED_ADVANCE_REROUTE_MIN_FRAMES &&
            TryBeginMove(preserveBlockedRecoveryState: true))
        {
            RefreshProgressCheckpoint(currentPosition, resetCounter: true);
            return true;
        }

        if (TryRecoverAutonomousRoamFailureBeforeInterrupt(currentPosition, $"BlockedAdvance:{reason}"))
        {
            return true;
        }

        if (ShouldEarlyAbortAutonomousRoamMove())
        {
            lastMoveSkipReason = "BlockedAdvanceStopgap";
            FinishMoveCycle(countTowardLongPause: false, reachedDestination: false);
            return true;
        }

        if (blockedAdvanceFrames >= BLOCKED_ADVANCE_LONG_PAUSE_MIN_FRAMES)
        {
            if (TryInterruptRoamMove(
                    RoamMoveInterruptionReason.StuckRecoveryFailed,
                    currentPosition,
                    lastBlockingAgentId,
                    null,
                    RoamMoveInterruptionBlockerKind.None,
                    $"BlockedAdvance:{reason}"))
            {
                return true;
            }

            FinishMoveCycle(countTowardLongPause: false, reachedDestination: false);
            return true;
        }

        return true;
    }

    private bool TryHandleRecoverablePointToPointTravelBlockedAdvance(Vector2 currentPosition, string reason)
    {
        string contractName = GetPointToPointTravelContractName(activePointToPointTravelContract);
        if (blockedAdvanceFrames >= STATIC_RECOVERY_DETOUR_MIN_BLOCKED_FRAMES &&
            TryCreateStaticObstacleRecoveryPath(currentPosition, reason))
        {
            lastMoveSkipReason = $"{contractName}StaticRecovery";
            RefreshProgressCheckpoint(currentPosition, resetCounter: true);
            return true;
        }

        if (Time.time - lastBlockedAdvanceRecoveryTime < BLOCKED_ADVANCE_RECOVERY_COOLDOWN)
        {
            return true;
        }

        lastBlockedAdvanceRecoveryTime = Time.time;
        if (TryRebuildPath(
                currentPosition,
                resetRecoveryCounter: false,
                reason: $"{contractName}BlockedAdvance:{reason}",
                preserveTrafficState: true,
                preserveBlockedRecoveryState: true))
        {
            RefreshProgressCheckpoint(currentPosition, resetCounter: true);
            return true;
        }

        if (blockedAdvanceFrames >= FORMAL_NAVIGATION_BLOCKED_ABORT_MIN_FRAMES)
        {
            lastMoveSkipReason = $"{contractName}BlockedAbort";
            EndDebugMove(reachedDestination: false);
            return true;
        }

        return true;
    }

    private bool ShouldBypassSharedAvoidanceForCurrentMove()
    {
        return IsPlainDebugMoveActive();
    }

    private bool ShouldAbortBlockedAdvanceWithoutRebuild(string reason)
    {
        if (HasActivePointToPointTravelContract())
        {
            return false;
        }

        int abortThreshold = BLOCKED_ADVANCE_STATIC_ABORT_MIN_FRAMES;
        if (UsesAutonomousRoamExecutionContract())
        {
            abortThreshold = AUTONOMOUS_STATIC_ABORT_MIN_FRAMES;
        }

        if (blockedAdvanceFrames < abortThreshold)
        {
            return false;
        }

        if (lastBlockingAgentId != 0 || sharedAvoidanceBlockingFrames > 0 || hasDynamicDetour)
        {
            return false;
        }

        return reason == "ConstrainedZeroAdvance" ||
               reason == "MoveCommandOscillation" ||
               reason == "MoveCommandNoProgress" ||
               reason == "ZeroStep";
    }

    private static bool IsStaticBlockedAdvanceReason(string reason)
    {
        return reason == "ConstrainedZeroAdvance" ||
               reason == "MoveCommandOscillation" ||
               reason == "MoveCommandNoProgress" ||
               reason == "ZeroStep";
    }

    private bool IsStaticBlockedAdvanceRecoveryEligible(string reason)
    {
        if (!IsStaticBlockedAdvanceReason(reason))
        {
            return false;
        }

        if (lastBlockingAgentId != 0 || sharedAvoidanceBlockingFrames > 0 || hasDynamicDetour)
        {
            return false;
        }

        return UsesAutonomousRoamExecutionContract() || IsRecoverablePointToPointTravelActive();
    }

    private void RememberStaticBlockedDestination(Vector2 currentPosition, Vector2 destination)
    {
        lastStaticBlockedDestinationTime = Time.time;
        lastStaticBlockedDestination = NormalizeDestinationToNavGridBounds(destination);
        lastStaticBlockedPosition = currentPosition;
    }

    private bool TryCreateStaticObstacleRecoveryPath(Vector2 currentPosition, string reason)
    {
        if (!TryResolveNavGrid(logIfMissing: false) ||
            !IsStaticBlockedAdvanceRecoveryEligible(reason))
        {
            return false;
        }

        bool useReleasedClearance = IsRecoverablePointToPointTravelActive();
        Vector2 rebuildDestination = NormalizeDestinationToNavGridBounds(GetRebuildRequestedDestination());
        if (useReleasedClearance &&
            !TryResolveReleasedMovementDestination(rebuildDestination, out rebuildDestination))
        {
            return false;
        }

        if (!TryResolveStaticObstacleRecoveryWaypoint(
                currentPosition,
                rebuildDestination,
                out Vector2 recoveryWaypoint))
        {
            return false;
        }

        if (!TryBuildPathToDestination(
                recoveryWaypoint,
                rebuildDestination,
                $"StaticRecovery:{reason}",
                out _))
        {
            RecordPathBuildOutcome(success: false);
            NavigationPathExecutor2D.Clear(navigationExecution, clearDestination: false);
            return false;
        }

        bool acceptable = useReleasedClearance
            ? IsRecoverablePointToPointBuiltPathAcceptable(recoveryWaypoint, rebuildDestination)
            : IsAutonomousRoamBuiltPathAcceptable(recoveryWaypoint, GetRoamCenter(), rebuildDestination);
        if (!acceptable)
        {
            RecordPathBuildOutcome(success: false);
            NavigationPathExecutor2D.Clear(navigationExecution, clearDestination: false);
            if (!useReleasedClearance)
            {
                RememberStaticBlockedDestination(currentPosition, rebuildDestination);
            }

            return false;
        }

        NavigationPathExecutor2D.SetOverrideWaypoint(navigationExecution, recoveryWaypoint, 0);
        RememberRequestedDestination(rebuildDestination);
        RecordPathBuildOutcome(success: true);

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=yellow>[NPCAutoRoamController]</color> {name} StaticRecovery => Reason={reason}, Waypoint={recoveryWaypoint}, Destination={rebuildDestination}, PathCount={path.Count}",
                this);
        }

        return true;
    }

    private bool TryResolveStaticObstacleRecoveryWaypoint(
        Vector2 currentPosition,
        Vector2 rebuildDestination,
        out Vector2 recoveryWaypoint)
    {
        recoveryWaypoint = currentPosition;
        bool useReleasedClearance = IsRecoverablePointToPointTravelActive();
        Vector2 referenceDirection = currentDestination - currentPosition;
        if (path.Count > currentPathIndex)
        {
            referenceDirection = path[currentPathIndex] - currentPosition;
        }

        if (referenceDirection.sqrMagnitude <= 0.0001f)
        {
            referenceDirection = rebuildDestination - currentPosition;
        }

        if (referenceDirection.sqrMagnitude <= 0.0001f)
        {
            referenceDirection = Vector2.right;
        }

        referenceDirection.Normalize();
        Vector2 sidestep = Vector2.Perpendicular(referenceDirection).normalized;
        Vector2 backward = -referenceDirection;
        Vector2[] recoveryDirections =
        {
            (sidestep + backward * 0.35f).normalized,
            (-sidestep + backward * 0.35f).normalized,
            sidestep,
            -sidestep,
            backward,
            (sidestep + referenceDirection * 0.18f).normalized,
            (-sidestep + referenceDirection * 0.18f).normalized
        };

        float detourDistanceStep = Mathf.Lerp(
            STATIC_RECOVERY_DETOUR_MIN_DISTANCE,
            STATIC_RECOVERY_DETOUR_MAX_DISTANCE,
            0.5f);
        float[] detourDistances =
        {
            STATIC_RECOVERY_DETOUR_MAX_DISTANCE,
            detourDistanceStep,
            STATIC_RECOVERY_DETOUR_MIN_DISTANCE
        };

        for (int distanceIndex = 0; distanceIndex < detourDistances.Length; distanceIndex++)
        {
            float detourDistance = detourDistances[distanceIndex];
            for (int directionIndex = 0; directionIndex < recoveryDirections.Length; directionIndex++)
            {
                Vector2 candidate = NormalizeDestinationToNavGridBounds(
                    currentPosition + recoveryDirections[directionIndex] * detourDistance);
                if (!TryResolveStaticObstacleRecoveryCandidate(
                        currentPosition,
                        candidate,
                        useReleasedClearance,
                        out Vector2 resolvedWaypoint))
                {
                    continue;
                }

                recoveryWaypoint = resolvedWaypoint;
                return true;
            }
        }

        return false;
    }

    private bool TryResolveStaticObstacleRecoveryCandidate(
        Vector2 currentPosition,
        Vector2 candidate,
        bool useReleasedClearance,
        out Vector2 recoveryWaypoint)
    {
        recoveryWaypoint = candidate;
        bool resolved = useReleasedClearance
            ? TryResolveReleasedMovementDestination(candidate, out recoveryWaypoint)
            : TryResolveAutonomousRoamDestinationCandidate(candidate, out recoveryWaypoint);
        if (!resolved)
        {
            return false;
        }

        float minRecoveryDistance = Mathf.Max(minimumMoveDistance * 0.45f, STATIC_RECOVERY_DETOUR_MIN_DISTANCE * 0.55f);
        if (Vector2.Distance(currentPosition, recoveryWaypoint) < minRecoveryDistance)
        {
            return false;
        }

        return HasPathSegmentBodyClearance(
            currentPosition,
            recoveryWaypoint,
            useReleasedClearance ? GetReleasedMovementStepBodyClearanceExtraRadius() : GetAutonomousRoamBodyClearanceExtraRadius(),
            useReleasedClearance ? RELEASED_MOVEMENT_PATH_CLEARANCE_SAMPLE_SPACING : AUTONOMOUS_ROAM_PATH_CLEARANCE_SAMPLE_SPACING,
            useReleasedClearance ? RELEASED_STEP_BODY_CLEARANCE_SIDE_INSET_SCALE : 0.35f,
            useReleasedClearance ? RELEASED_STEP_BODY_CLEARANCE_VERTICAL_INSET_SCALE : 0.55f);
    }

    private bool TryRetargetAutonomousRoamAfterStaticBlock(Vector2 currentPosition, string reason)
    {
        if (!UsesAutonomousRoamExecutionContract())
        {
            return false;
        }

        if (lastBlockingAgentId != 0 || sharedAvoidanceBlockingFrames > 0 || hasDynamicDetour)
        {
            return false;
        }

        RememberStaticBlockedDestination(currentPosition, GetRebuildRequestedDestination());
        if (!TryBeginMove())
        {
            return false;
        }

        lastMoveSkipReason = $"BlockedAdvanceStaticRetarget:{reason}";
        RefreshProgressCheckpoint(currentPosition, resetCounter: true);
        return true;
    }

    private bool TryRecoverAutonomousRoamFailureBeforeInterrupt(Vector2 currentPosition, string reason)
    {
        if (!UsesAutonomousRoamExecutionContract())
        {
            return false;
        }

        if (TryFinishAutonomousRoamSoftArrival(currentPosition, reason))
        {
            return true;
        }

        return TryRetargetAutonomousRoamAfterStaticBlock(currentPosition, reason);
    }

    private bool TryFinishAutonomousRoamSoftArrival(Vector2 currentPosition, string reason)
    {
        if (!CanTreatAutonomousRoamFailureAsSoftArrival(currentPosition))
        {
            return false;
        }

        RememberStaticBlockedDestination(currentPosition, GetRebuildRequestedDestination());
        FinishMoveCycle(countTowardLongPause: true, reachedDestination: true);
        lastMoveSkipReason = $"AutonomousSoftArrival:{reason}";
        return true;
    }

    private bool CanTreatAutonomousRoamFailureAsSoftArrival(Vector2 currentPosition)
    {
        if (!UsesAutonomousRoamExecutionContract() || state != RoamState.Moving)
        {
            return false;
        }

        Vector2 requestedDestination = GetRebuildRequestedDestination();
        if (Vector2.Distance(currentPosition, requestedDestination) > GetAutonomousRoamSoftArrivalDistance())
        {
            return false;
        }

        return IsWithinAutonomousRoamBounds(GetRoamCenter(), currentPosition);
    }

    private float GetAutonomousRoamSoftArrivalDistance()
    {
        float desiredDistance = Mathf.Max(
            destinationTolerance * 2.2f,
            GetColliderRadius() + GetContactShellPadding(),
            minimumMoveDistance * 0.45f);
        return Mathf.Clamp(
            desiredDistance,
            AUTONOMOUS_ROAM_SOFT_ARRIVAL_DISTANCE_MIN,
            AUTONOMOUS_ROAM_SOFT_ARRIVAL_DISTANCE_MAX);
    }

    private void RecordBlockedAdvance(Vector2 currentPosition)
    {
        if (Vector2.Distance(currentPosition, lastBlockedAdvancePosition) <= BLOCKED_ADVANCE_SAME_POSITION_RADIUS)
        {
            blockedAdvanceFrames++;
        }
        else
        {
            blockedAdvanceFrames = 1;
        }

        lastBlockedAdvancePosition = currentPosition;
        if (motionController != null)
        {
            motionController.StopMotion();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private bool TryHandlePendingMoveCommandNoProgress(Vector2 currentPosition)
    {
        if (!hasPendingMoveCommandProgressCheck)
        {
            return false;
        }

        if (Vector2.Distance(currentPosition, pendingMoveCommandOrigin) > MOVE_COMMAND_NO_PROGRESS_RADIUS)
        {
            ClearPendingMoveCommandProgressCheck();
            NoteSuccessfulAdvance(currentPosition);
            return false;
        }

        if (Time.time - pendingMoveCommandIssuedAt < MOVE_COMMAND_NO_PROGRESS_GRACE_SECONDS)
        {
            return false;
        }

        ClearPendingMoveCommandProgressCheck();
        return TryHandleBlockedAdvance(currentPosition, "MoveCommandNoProgress");
    }

    private void MarkMoveCommandIssued(Vector2 currentPosition, Vector2 nextPosition)
    {
        if (Vector2.Distance(currentPosition, nextPosition) <= MOVE_COMMAND_NO_PROGRESS_RADIUS)
        {
            ClearPendingMoveCommandProgressCheck();
            return;
        }

        hasPendingMoveCommandProgressCheck = true;
        pendingMoveCommandOrigin = currentPosition;
        pendingMoveCommandIssuedAt = Time.time;
    }

    private void ClearPendingMoveCommandProgressCheck()
    {
        hasPendingMoveCommandProgressCheck = false;
        pendingMoveCommandOrigin = Vector2.zero;
        pendingMoveCommandIssuedAt = float.NegativeInfinity;
    }

    private bool ShouldEarlyAbortAutonomousRoamMove()
    {
        return UsesAutonomousRoamExecutionContract() &&
            blockedAdvanceFrames >= BLOCKED_ADVANCE_REROUTE_MIN_FRAMES;
    }

    private bool ShouldTreatMoveCommandAsOscillation(Vector2 currentPosition, Vector2 velocity)
    {
        Vector2 moveDirection = velocity.normalized;
        bool samePosition =
            Vector2.Distance(currentPosition, lastIssuedMovePosition) <= MOVE_COMMAND_OSCILLATION_SAME_POSITION_RADIUS;
        bool flipped =
            lastIssuedMoveDirection.sqrMagnitude > 0.0001f &&
            Vector2.Dot(moveDirection, lastIssuedMoveDirection) <= MOVE_COMMAND_OSCILLATION_FLIP_DOT_THRESHOLD;

        if (samePosition && flipped)
        {
            consecutiveMoveCommandDirectionFlips++;
        }
        else
        {
            consecutiveMoveCommandDirectionFlips = 0;
        }

        lastIssuedMoveDirection = moveDirection;
        lastIssuedMovePosition = currentPosition;
        return consecutiveMoveCommandDirectionFlips >= MOVE_COMMAND_OSCILLATION_MIN_FLIPS;
    }

    private Vector2 StabilizeMoveDirection(Vector2 currentPosition, Vector2 desiredDirection)
    {
        if (rb == null ||
            desiredDirection.sqrMagnitude <= 0.0001f ||
            lastIssuedMoveDirection.sqrMagnitude <= 0.0001f)
        {
            return desiredDirection;
        }

        if (!ShouldHoldPreviousMoveDirection() ||
            Vector2.Distance(currentPosition, lastIssuedMovePosition) > MOVE_DIRECTION_STABILIZE_SAME_POSITION_RADIUS)
        {
            return desiredDirection;
        }

        float alignment = Vector2.Dot(desiredDirection, lastIssuedMoveDirection);
        if (alignment > MOVE_DIRECTION_STABILIZE_FLIP_DOT_THRESHOLD)
        {
            return desiredDirection;
        }

        return lastIssuedMoveDirection;
    }

    private bool ShouldHoldPreviousMoveDirection()
    {
        return hasPendingMoveCommandProgressCheck ||
            blockedAdvanceFrames > 0 ||
            (rb != null && IsWithinPhysicsHeavyDecisionReuseWindow());
    }

    private void ResetMoveCommandOscillationState(Vector2 currentPosition)
    {
        lastIssuedMoveDirection = Vector2.zero;
        lastIssuedMovePosition = currentPosition;
        consecutiveMoveCommandDirectionFlips = 0;
    }

    private BlockedRecoveryState CaptureBlockedRecoveryState()
    {
        return new BlockedRecoveryState(
            blockedAdvanceFrames,
            lastBlockedAdvancePosition,
            consecutiveTerminalStuckCount,
            lastTerminalStuckPosition);
    }

    private void RestoreBlockedRecoveryState(BlockedRecoveryState recoveryState)
    {
        blockedAdvanceFrames = recoveryState.BlockedAdvanceFrames;
        lastBlockedAdvancePosition = recoveryState.LastBlockedAdvancePosition;
        consecutiveTerminalStuckCount = recoveryState.ConsecutiveTerminalStuckCount;
        lastTerminalStuckPosition = recoveryState.LastTerminalStuckPosition;
    }

    private bool TryHandleTerminalStuck(Vector2 currentPosition)
    {
        if (Vector2.Distance(currentPosition, lastTerminalStuckPosition) <= TERMINAL_STUCK_SAME_POSITION_RADIUS)
        {
            consecutiveTerminalStuckCount++;
        }
        else
        {
            consecutiveTerminalStuckCount = 1;
        }

        lastTerminalStuckPosition = currentPosition;
        if (consecutiveTerminalStuckCount >= TERMINAL_STUCK_LONG_PAUSE_MIN_COUNT)
        {
            EnterLongPause();
            return true;
        }

        return false;
    }

    private void ResetMovementRecovery(
        Vector2 currentPosition,
        bool resetCounter,
        bool preserveTrafficState = false)
    {
        NavigationPathExecutor2D.ResetProgress(navigationExecution, currentPosition, resetCounter);
        lastBlockedAdvanceRecoveryTime = float.NegativeInfinity;
        ClearPendingMoveCommandProgressCheck();
        ResetMoveCommandOscillationState(currentPosition);
        NoteSuccessfulAdvance(currentPosition);
        if (resetCounter && !preserveTrafficState)
        {
            lastBlockingAgentId = 0;
            blockingAgentSightings = 0;
            sharedAvoidanceBlockingFrames = 0;
            sharedAvoidanceNoBlockerFrames = 0;
        }
    }

    private void RememberRequestedDestination(Vector2 destination)
    {
        requestedDestination = NormalizeDestinationToNavGridBounds(destination);
        hasRequestedDestination = true;
    }

    private void ClearRequestedDestination()
    {
        requestedDestination = Vector2.zero;
        hasRequestedDestination = false;
    }

    private Vector2 GetRebuildRequestedDestination()
    {
        return hasRequestedDestination ? requestedDestination : currentDestination;
    }

    private void ResetRoamInterruptionState()
    {
        hasRoamInterruption = false;
        lastRoamInterruption = default;
    }

    private bool TryInterruptRoamMove(
        RoamMoveInterruptionReason reason,
        Vector2 currentPosition,
        int blockerId,
        Vector2? blockerPosition,
        RoamMoveInterruptionBlockerKind blockerKind,
        string trigger,
        List<NavigationAgentSnapshot> blockingCandidates = null)
    {
        if (HasActivePointToPointTravelContract() || state != RoamState.Moving)
        {
            return false;
        }

        RoamMoveInterruptionBlockerKind resolvedBlockerKind = blockerKind;
        Vector2 resolvedBlockerPosition = blockerPosition ?? Vector2.zero;
        bool hasBlockerPosition = blockerPosition.HasValue;
        if (blockerId != 0)
        {
            if (TryResolveBlockingSnapshot(blockerId, blockingCandidates, out NavigationAgentSnapshot snapshot) ||
                TryResolveCurrentBlockingSnapshot(blockerId, out snapshot))
            {
                resolvedBlockerKind = ToRoamMoveInterruptionBlockerKind(snapshot.UnitType);
                if (!hasBlockerPosition)
                {
                    resolvedBlockerPosition = snapshot.Position;
                    hasBlockerPosition = true;
                }
            }
            else if (resolvedBlockerKind == RoamMoveInterruptionBlockerKind.None)
            {
                resolvedBlockerKind = RoamMoveInterruptionBlockerKind.Unknown;
            }
        }
        else
        {
            resolvedBlockerKind = RoamMoveInterruptionBlockerKind.None;
        }

        lastRoamInterruption = new RoamMoveInterruptionSnapshot(
            reason,
            resolvedBlockerKind,
            blockerId,
            hasRequestedDestination ? requestedDestination : currentDestination,
            currentDestination,
            currentPosition,
            resolvedBlockerPosition,
            hasBlockerPosition,
            Time.time,
            string.IsNullOrEmpty(trigger) ? reason.ToString() : trigger);
        hasRoamInterruption = true;

        Debug.LogWarning(
            $"[NPCAutoRoamController] {name} roam interrupted => Reason={lastRoamInterruption.Reason}, Trigger={lastRoamInterruption.Trigger}, " +
            $"BlockerKind={lastRoamInterruption.BlockerKind}, BlockerId={lastRoamInterruption.BlockerId}, " +
            $"Requested={lastRoamInterruption.RequestedDestination}, Active={lastRoamInterruption.ActiveDestination}, " +
            $"Current={lastRoamInterruption.CurrentPosition}, " +
            $"Blocker={(lastRoamInterruption.HasBlockerPosition ? lastRoamInterruption.BlockerPosition.ToString() : "n/a")}",
            this);

        NavigationPathExecutor2D.Clear(navigationExecution, clearDestination: true);
        ResetSharedAvoidanceDebugState();

        if (motionController != null)
        {
            motionController.StopMotion();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        EnterShortPause(false);
        RoamMoveInterrupted?.Invoke(lastRoamInterruption);
        return true;
    }

    private bool TryResolveCurrentBlockingSnapshot(int blockerId, out NavigationAgentSnapshot snapshot)
    {
        snapshot = default;
        if (blockerId == 0)
        {
            return false;
        }

        NavigationAgentSnapshot self = NavigationAgentSnapshot.FromUnit(this);
        if (!self.IsValid)
        {
            return false;
        }

        float referenceSpeed = motionController != null ? motionController.MoveSpeed : 0f;
        float lookAhead = NavigationLocalAvoidanceSolver.GetRecommendedLookAhead(
            self,
            sharedAvoidanceLookAhead,
            referenceSpeed);
        NavigationAgentRegistry.GetNearbySnapshots(this, GetNavigationCenter(), lookAhead, nearbyNavigationAgents);
        return TryResolveBlockingSnapshot(blockerId, nearbyNavigationAgents, out snapshot);
    }

    private static bool TryResolveBlockingSnapshot(
        int blockerId,
        List<NavigationAgentSnapshot> candidates,
        out NavigationAgentSnapshot snapshot)
    {
        snapshot = default;
        if (blockerId == 0 || candidates == null)
        {
            return false;
        }

        for (int index = 0; index < candidates.Count; index++)
        {
            if (candidates[index].InstanceId == blockerId)
            {
                snapshot = candidates[index];
                return true;
            }
        }

        return false;
    }

    private static RoamMoveInterruptionBlockerKind ToRoamMoveInterruptionBlockerKind(NavigationUnitType unitType)
    {
        switch (unitType)
        {
            case NavigationUnitType.Player:
                return RoamMoveInterruptionBlockerKind.Player;

            case NavigationUnitType.NPC:
                return RoamMoveInterruptionBlockerKind.NPC;

            case NavigationUnitType.Enemy:
                return RoamMoveInterruptionBlockerKind.Enemy;

            case NavigationUnitType.StaticObstacle:
                return RoamMoveInterruptionBlockerKind.StaticObstacle;

            default:
                return RoamMoveInterruptionBlockerKind.Unknown;
        }
    }

    private static string[] CopyLines(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            return System.Array.Empty<string>();
        }

        string[] copy = new string[lines.Length];
        for (int index = 0; index < lines.Length; index++)
        {
            copy[index] = lines[index];
        }

        return copy;
    }

    private static bool ShouldRetryAmbientChat(AmbientChatAttemptResult attemptResult)
    {
        return attemptResult == AmbientChatAttemptResult.NoNearbyPartner ||
               attemptResult == AmbientChatAttemptResult.PartnerRejected;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector2 roamCenter = homeAnchor != null ? (Vector2)homeAnchor.position : (Vector2)transform.position;

        Gizmos.color = new Color(0f, 0.8f, 1f, 0.35f);
        Gizmos.DrawWireSphere(roamCenter, activityRadius);

        if (!drawDebugPath || path.Count == 0)
        {
            return;
        }

        Gizmos.color = Color.green;
        Vector3 previous = transform.position;
        for (int index = 0; index < path.Count; index++)
        {
            Vector3 point = new Vector3(path[index].x, path[index].y, transform.position.z);
            Gizmos.DrawLine(previous, point);
            Gizmos.DrawSphere(point, 0.05f);
            previous = point;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            new Vector3(currentDestination.x, currentDestination.y, transform.position.z),
            destinationTolerance);
    }
#endif
}
