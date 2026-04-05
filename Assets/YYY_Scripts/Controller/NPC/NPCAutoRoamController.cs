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
    private const int BLOCKED_ADVANCE_REROUTE_MIN_FRAMES = 3;
    private const int BLOCKED_ADVANCE_LONG_PAUSE_MIN_FRAMES = 6;
    private const float MOVE_COMMAND_NO_PROGRESS_RADIUS = 0.015f;
    private const float MOVE_COMMAND_NO_PROGRESS_GRACE_SECONDS = 0.06f;
    private const float MOVE_COMMAND_OSCILLATION_SAME_POSITION_RADIUS = 0.14f;
    private const float MOVE_COMMAND_OSCILLATION_FLIP_DOT_THRESHOLD = 0.15f;
    private const int MOVE_COMMAND_OSCILLATION_MIN_FLIPS = 2;
    private const float CONSTRAINED_ADVANCE_FALLBACK_EXTRA_DISTANCE = 0.18f;
    private const int TERMINAL_STUCK_LONG_PAUSE_MIN_COUNT = 2;
    private const float TERMINAL_STUCK_SAME_POSITION_RADIUS = 0.2f;
    private const float AVOIDANCE_STUCK_RESET_MAX_MOVE_SCALE = 0.32f;
    private const int PASSIVE_NPC_BLOCKER_REROUTE_MIN_SIGHTINGS = 4;
    private const float PASSIVE_NPC_BLOCKER_REROUTE_MAX_DISTANCE = 1.15f;
    private const float PASSIVE_NPC_BLOCKER_REROUTE_MAX_CLEARANCE = 0.24f;

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
    private readonly NavigationPathExecutor2D.ExecutionState navigationExecution = new NavigationPathExecutor2D.ExecutionState();

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
    private bool hasPendingMoveCommandProgressCheck;
    private Vector2 pendingMoveCommandOrigin;
    private float pendingMoveCommandIssuedAt = float.NegativeInfinity;
    private Vector2 lastIssuedMoveDirection;
    private Vector2 lastIssuedMovePosition;
    private int consecutiveMoveCommandDirectionFlips;
    private int consecutiveTerminalStuckCount;
    private Vector2 lastTerminalStuckPosition;
    private string lastMoveSkipReason = "None";

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
    public string ChatPartnerName => chatPartner != null ? chatPartner.name : string.Empty;
    public string LastAmbientDecision => lastAmbientDecision;
    public string DebugState => state.ToString();
    public float DebugStateTimer => stateTimer;
    public int CompletedShortPauseCount => completedShortPauseCount;
    public int LongPauseTriggerCount => longPauseTriggerCount;
    public int CurrentStuckRecoveryCount => currentStuckRecoveryCount;
    public float DebugLastProgressDistance => lastProgressDistance;
    public string DebugLastMoveSkipReason => lastMoveSkipReason;
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
        float cappedShell = Mathf.Min(avoidanceRadius, colliderRadius + 0.06f);
        return Mathf.Max(colliderRadius, cappedShell);
    }
    public Vector2 GetCurrentVelocity()
    {
        if (motionController != null)
        {
            Vector2 currentVelocity = motionController.ReportedVelocity;
            if (currentVelocity.sqrMagnitude > 0.0001f)
            {
                return currentVelocity;
            }

            return rb != null ? rb.linearVelocity : Vector2.zero;
        }

        return rb != null ? rb.linearVelocity : Vector2.zero;
    }
    public int GetAvoidancePriority() => avoidancePriority;
    public bool IsCurrentlyMoving() => state == RoamState.Moving && GetCurrentVelocity().sqrMagnitude > 0.0001f;
    public bool IsNavigationSleeping() => state != RoamState.Moving;
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

        homePosition = homeAnchor != null ? homeAnchor.position : transform.position;
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
        if (ShouldSuspendForDialogue())
        {
            ApplyDialogueFreeze();
            return;
        }

        UpdateTraversalSoftPassState(GetNavigationCenter());

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
        if (ShouldSuspendForDialogue())
        {
            ApplyDialogueFreeze();
            return;
        }

        if (state == RoamState.Moving && rb != null)
        {
            TickMoving(Time.fixedDeltaTime);
        }
    }

    private static bool ShouldSuspendForDialogue()
    {
        return DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive;
    }

    private void ApplyDialogueFreeze()
    {
        BreakAmbientChatLink(hideBubble: true);

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

    private void OnDisable()
    {
        NavigationAgentRegistry.Unregister(this);
        StopRoam();
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

    public void StartRoam()
    {
        CacheComponents();
        EnsureRuntimeHomeAnchorBound();

        homePosition = homeAnchor != null ? homeAnchor.position : transform.position;
        ResetSharedAvoidanceDebugState();
        ResetRoamInterruptionState();
        warnedMissingNavGrid = false;
        completedShortPauseCount = 0;
        lastAmbientDecision = string.Empty;
        ResetLongPauseTriggerCount();
        ResetMovementRecovery(GetNavigationCenter(), resetCounter: true);
        EnterShortPause(false);
    }

    public void StopRoam()
    {
        BreakAmbientChatLink(hideBubble: true);
        debugMoveActive = false;
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

    public void ApplyProfile()
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

    public void SetHomeAnchor(Transform anchor)
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
        Transform parent = transform.parent;
        if (parent != null)
        {
            Transform siblingAnchor = parent.Find(anchorName);
            if (siblingAnchor != null)
            {
                return siblingAnchor;
            }
        }

        Transform childAnchor = transform.Find(anchorName);
        if (childAnchor != null)
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
            if (sceneAnchor != null)
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

    public void SyncHomeAnchorToCurrentPosition()
    {
        if (homeAnchor != null)
        {
            homeAnchor.position = transform.position;
        }

        homePosition = transform.position;
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

    public bool DebugMoveTo(Vector2 destination)
    {
        CacheComponents();
        EnsureRuntimeHomeAnchorBound();
        if (!TryResolveNavGrid())
        {
            return false;
        }

        BreakAmbientChatLink(hideBubble: true);
        ClearAmbientChatRetry();
        if (!TryResolveOccupiableDestination(destination, out Vector2 resolvedDestination))
        {
            return false;
        }

        RememberRequestedDestination(resolvedDestination);
        ResetRoamInterruptionState();

        Vector2 currentPosition = rb != null ? rb.position : (Vector2)transform.position;
        NavigationPathExecutor2D.BuildPathResult buildResult = NavigationPathExecutor2D.TryRefreshPath(
            navigationExecution,
            navGrid,
            currentPosition,
            resolvedDestination,
            null,
            null,
            0f,
            GetPathBuildLogger("DebugMoveTo"),
            navigationCollider);

        if (!buildResult.Success || path.Count == 0)
        {
            return false;
        }

        debugMoveActive = true;
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
                $"<color=green>[NPCAutoRoamController]</color> {name} DebugMoveTo => Destination={currentDestination}, PathCount={path.Count}",
                this);
        }

        return true;
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
            lastMoveSkipReason = "MissingMotionController";
            return;
        }

        Vector2 currentPosition = rb != null ? rb.position : (Vector2)transform.position;
        if (path.Count == 0)
        {
            lastMoveSkipReason = "PathClearedWhileMoving";
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

        UpdateTraversalSoftPassState(currentPosition);
        Vector2 avoidancePosition = GetNavigationCenter();

        if (TryHandlePendingMoveCommandNoProgress(currentPosition))
        {
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
            lastMoveSkipReason = "ReachedDestination";
            FinishMoveCycle(countTowardLongPause: true, reachedDestination: true);
            return;
        }

        if (CheckAndHandleStuck(currentPosition))
        {
            lastMoveSkipReason = "Stuck";
            return;
        }

        if (!waypointState.HasWaypoint)
        {
            lastMoveSkipReason = "MissingWaypoint";
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
            lastMoveSkipReason = distance <= 0.0001f ? "ZeroDistanceToWaypoint" : "ZeroMoveSpeed";
            motionController.StopMotion();
            return;
        }

        Vector2 moveDirection = toWaypoint / distance;
        if (TryHandleSharedAvoidance(currentPosition, avoidancePosition, waypoint, moveDirection, out Vector2 adjustedDirection, out float moveScale))
        {
            lastMoveSkipReason = "SharedAvoidance";
            return;
        }

        float step = moveSpeed * Mathf.Clamp01(moveScale) * Mathf.Max(deltaTime, 0.0001f);
        if (step <= 0.0001f)
        {
            lastMoveSkipReason = "ZeroStep";
            TryHandleBlockedAdvance(currentPosition, "ZeroStep");
            return;
        }

        Vector2 nextPosition = distance <= step
            ? waypoint
            : currentPosition + adjustedDirection * step;
        nextPosition = ConstrainNextPositionToNavigationBounds(currentPosition, nextPosition);

        float safeDeltaTime = Mathf.Max(deltaTime, 0.0001f);
        Vector2 velocity = (nextPosition - currentPosition) / safeDeltaTime;
        if (velocity.sqrMagnitude <= 0.0001f)
        {
            lastMoveSkipReason = "ConstrainedZeroAdvance";
            TryHandleBlockedAdvance(currentPosition, "ConstrainedZeroAdvance");
            return;
        }

        if (ShouldTreatMoveCommandAsOscillation(currentPosition, velocity))
        {
            lastMoveSkipReason = "MoveCommandOscillation";
            TryHandleBlockedAdvance(currentPosition, "MoveCommandOscillation");
            return;
        }

        lastMoveSkipReason = "IssuingVelocity";
        MarkMoveCommandIssued(currentPosition, nextPosition);
        motionController.SetExternalVelocity(velocity);
        if (rb != null)
        {
            if (rb.bodyType == RigidbodyType2D.Dynamic)
            {
                rb.linearVelocity = velocity;
            }
            else
            {
                rb.MovePosition(nextPosition);
            }
        }
        else
        {
            transform.position = new Vector3(nextPosition.x, nextPosition.y, transform.position.z);
        }
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

        BlockedRecoveryState blockedRecoveryState = preserveBlockedRecoveryState
            ? CaptureBlockedRecoveryState()
            : default;

        debugMoveActive = false;
        NavigationPathExecutor2D.Clear(navigationExecution, clearDestination: false);

        Vector2 currentPosition = GetNavigationCenter();
        Vector2 roamCenter = GetRoamCenter();

        for (int attempt = 0; attempt < pathSampleAttempts; attempt++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * activityRadius;
            Vector2 sampledDestination = NormalizeDestinationToNavGridBounds(roamCenter + randomOffset);

            if (Vector2.Distance(currentPosition, sampledDestination) < minimumMoveDistance)
            {
                continue;
            }

            if (!TryResolveOccupiableDestination(sampledDestination, out Vector2 walkableDestination))
            {
                continue;
            }

            if (Vector2.Distance(currentPosition, walkableDestination) < minimumMoveDistance)
            {
                continue;
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
                continue;
            }

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

        if (showDebugLog)
        {
            Debug.LogWarning($"[NPCAutoRoamController] {name} 在 {pathSampleAttempts} 次采样内没有找到可用路径。", this);
        }

        return false;
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
            return false;
        }

        if (TryBeginMove(preserveBlockedRecoveryState: true))
        {
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

        Vector2 rebuildDestination = NormalizeDestinationToNavGridBounds(GetRebuildRequestedDestination());
        NavigationPathExecutor2D.BuildPathResult buildResult = NavigationPathExecutor2D.TryRefreshPath(
            navigationExecution,
            navGrid,
            currentPosition,
            rebuildDestination,
            null,
            null,
            0f,
            GetPathBuildLogger(reason),
            navigationCollider);

        if (!buildResult.Success || path.Count == 0)
        {
            return false;
        }

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

            if (debugMoveActive)
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

    private Vector2 NormalizeDestinationToNavGridBounds(Vector2 destination)
    {
        if (!TryResolveNavGrid(logIfMissing: false))
        {
            return destination;
        }

        return navGrid.ClampToWorldBounds(destination);
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
        debugMoveActive = false;
        StopRoam();

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=yellow>[NPCAutoRoamController]</color> {name} DebugMove {(reachedDestination ? "completed" : "stopped")} => Position={(rb != null ? rb.position : (Vector2)transform.position)}, Destination={currentDestination}",
                this);
        }
    }

    private void EnterLongPause()
    {
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
                bubblePresenter.ShowRandomSelfTalk(Mathf.Max(1f, stateTimer - 0.15f));
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
        return homeAnchor != null ? (Vector2)homeAnchor.position : homePosition;
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
        NPCAutoRoamController[] controllers = FindObjectsByType<NPCAutoRoamController>(FindObjectsSortMode.None);
        for (int index = 0; index < controllers.Length; index++)
        {
            NPCAutoRoamController candidate = controllers[index];
            if (!CanUseAsAmbientChatPartner(candidate))
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

        if (hideBubble && bubblePresenter != null)
        {
            bubblePresenter.HideBubble();
        }

        NPCAutoRoamController partner = chatPartner;
        chatPartner = null;

        if (partner != null && partner.chatPartner == this)
        {
            partner.chatPartner = null;
            partner.StopAmbientChatRoutine();
            if (hideBubble && partner.bubblePresenter != null)
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
        lastBlockedAdvancePosition = currentPosition;
        consecutiveTerminalStuckCount = 0;
        lastTerminalStuckPosition = currentPosition;
        ResetMoveCommandOscillationState(currentPosition);
        lastMoveSkipReason = "AdvanceConfirmed";
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
        if (debugMoveActive || state != RoamState.Moving)
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
