using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC 自动漫游控制器。
/// 负责随机选点、路径跟随、短停/长停节奏，以及长停时的自言自语或附近聊天。
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(NPCMotionController))]
public class NPCAutoRoamController : MonoBehaviour, INavigationUnit
{
    private const float AmbientChatRetryDelay = 0.9f;
    private const float AmbientChatRetryMinRemainingTime = 1.25f;
    private const int AmbientChatMaxRetryCount = 3;

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

    [Header("组件引用")]
    [SerializeField] private NPCMotionController motionController;
    [SerializeField] private NPCBubblePresenter bubblePresenter;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private NavGrid2D navGrid;
    [SerializeField] private Transform homeAnchor;
    [SerializeField] private NPCRoamProfile roamProfile;

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

    [Header("卡住恢复")]
    [SerializeField] private float stuckCheckInterval = 0.3f;
    [SerializeField] private float stuckDistanceThreshold = 0.08f;
    [SerializeField] private int maxStuckRecoveries = 3;

    [Header("调试")]
    [SerializeField] private bool showDebugLog = false;
    [SerializeField] private bool drawDebugPath = true;

    [Header("共享动态避让")]
    [SerializeField] private float avoidanceRadius = 0.6f;
    [SerializeField] private int avoidancePriority = 50;
    [SerializeField] private float sharedAvoidanceLookAhead = 0.8f;
    [SerializeField] private float sharedAvoidanceRepathCooldown = 0.45f;

    private readonly List<Vector2> path = new List<Vector2>();
    private readonly List<NavigationAgentSnapshot> nearbyNavigationAgents = new List<NavigationAgentSnapshot>(12);

    private RoamState state = RoamState.Inactive;
    private Vector2 homePosition;
    private Vector2 currentDestination;
    private float stateTimer;
    private int currentPathIndex;
    private int completedShortPauseCount;
    private int longPauseTriggerCount;
    private bool warnedMissingNavGrid;
    private Coroutine ambientChatCoroutine;
    private NPCAutoRoamController chatPartner;
    private string lastAmbientDecision = string.Empty;
    private bool ambientChatRetryPending;
    private float ambientChatRetryAtTime;
    private int ambientChatRetryCount;
    private Vector2 lastProgressCheckPosition;
    private float lastProgressCheckTime;
    private float lastProgressDistance;
    private int currentStuckRecoveryCount;
    private Collider2D navigationCollider;
    private float lastSharedAvoidanceRepathTime = float.NegativeInfinity;
    private int lastBlockingAgentId;
    private int blockingAgentSightings;
    private int lastSharedAvoidanceDebugFrame = -999;

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
    public NPCRoamProfile RoamProfile => roamProfile;
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
    public float GetAvoidanceRadius() => Mathf.Max(avoidanceRadius, GetColliderRadius());
    public Vector2 GetCurrentVelocity()
    {
        if (motionController != null)
        {
            return motionController.CurrentVelocity;
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

    private void Reset()
    {
        CacheComponents();
    }

    private void Awake()
    {
        CacheComponents();

        if (applyProfileOnAwake)
        {
            ApplyProfile();
        }

        homePosition = transform.position;
    }

    private void OnEnable()
    {
        NavigationAgentRegistry.Register(this);
    }

    private void Start()
    {
        ResetLongPauseTriggerCount();
        ResetMovementRecovery(transform.position, resetCounter: true);

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
        if (state == RoamState.Moving && rb != null)
        {
            TickMoving(Time.fixedDeltaTime);
        }
    }

    private void OnDisable()
    {
        NavigationAgentRegistry.Unregister(this);
        StopRoam();
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

    public void StartRoam()
    {
        CacheComponents();

        homePosition = transform.position;
        warnedMissingNavGrid = false;
        completedShortPauseCount = 0;
        lastAmbientDecision = string.Empty;
        ResetLongPauseTriggerCount();
        ResetMovementRecovery(transform.position, resetCounter: true);
        EnterShortPause(false);
    }

    public void StopRoam()
    {
        BreakAmbientChatLink(hideBubble: true);
        state = RoamState.Inactive;
        stateTimer = 0f;
        currentPathIndex = 0;
        path.Clear();
        ResetMovementRecovery(transform.position, resetCounter: true);

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
            return;
        }

        if (path.Count == 0)
        {
            EnterShortPause(true);
            return;
        }

        Vector2 currentPosition = rb != null ? rb.position : (Vector2)transform.position;
        Vector2 avoidancePosition = GetNavigationCenter();

        while (currentPathIndex < path.Count &&
               Vector2.Distance(currentPosition, path[currentPathIndex]) <= waypointTolerance)
        {
            currentPathIndex++;
        }

        if (currentPathIndex >= path.Count ||
            Vector2.Distance(currentPosition, currentDestination) <= destinationTolerance)
        {
            EnterShortPause(true);
            return;
        }

        if (CheckAndHandleStuck(currentPosition))
        {
            return;
        }

        Vector2 waypoint = path[currentPathIndex];
        Vector2 toWaypoint = waypoint - currentPosition;
        float distance = toWaypoint.magnitude;
        float moveSpeed = Mathf.Max(0f, motionController.MoveSpeed);

        if (distance <= 0.0001f || moveSpeed <= 0f)
        {
            motionController.StopMotion();
            return;
        }

        Vector2 moveDirection = toWaypoint / distance;
        if (TryHandleSharedAvoidance(currentPosition, avoidancePosition, waypoint, moveDirection, out Vector2 adjustedDirection, out float moveScale))
        {
            return;
        }

        float step = moveSpeed * Mathf.Clamp01(moveScale) * Mathf.Max(deltaTime, 0.0001f);
        if (step <= 0.0001f)
        {
            motionController.StopMotion();
            return;
        }

        Vector2 nextPosition = distance <= step
            ? waypoint
            : currentPosition + adjustedDirection * step;

        float safeDeltaTime = Mathf.Max(deltaTime, 0.0001f);
        Vector2 velocity = (nextPosition - currentPosition) / safeDeltaTime;

        motionController.SetExternalVelocity(velocity);
        if (rb != null)
        {
            rb.MovePosition(nextPosition);
        }
        else
        {
            transform.position = new Vector3(nextPosition.x, nextPosition.y, transform.position.z);
        }
    }

    private void TickLongPause()
    {
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

    private bool TryBeginMove()
    {
        if (!TryResolveNavGrid())
        {
            return false;
        }

        path.Clear();
        currentPathIndex = 0;

        Vector2 currentPosition = transform.position;
        Vector2 roamCenter = GetRoamCenter();

        for (int attempt = 0; attempt < pathSampleAttempts; attempt++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * activityRadius;
            Vector2 sampledDestination = roamCenter + randomOffset;

            if (Vector2.Distance(currentPosition, sampledDestination) < minimumMoveDistance)
            {
                continue;
            }

            Vector2 walkableDestination = sampledDestination;
            if (!navGrid.IsWalkable(sampledDestination) &&
                !navGrid.TryFindNearestWalkable(sampledDestination, out walkableDestination))
            {
                continue;
            }

            if (Vector2.Distance(currentPosition, walkableDestination) < minimumMoveDistance)
            {
                continue;
            }

            if (!navGrid.TryFindPath(currentPosition, walkableDestination, path) || path.Count == 0)
            {
                path.Clear();
                continue;
            }

            currentDestination = walkableDestination;
            state = RoamState.Moving;
            stateTimer = 0f;
            ResetMovementRecovery(currentPosition, resetCounter: true);

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
        if (stuckCheckInterval <= 0f || Time.time - lastProgressCheckTime < stuckCheckInterval)
        {
            return false;
        }

        lastProgressDistance = Vector2.Distance(currentPosition, lastProgressCheckPosition);
        lastProgressCheckPosition = currentPosition;
        lastProgressCheckTime = Time.time;

        if (lastProgressDistance >= stuckDistanceThreshold)
        {
            currentStuckRecoveryCount = 0;
            return false;
        }

        currentStuckRecoveryCount++;

        if (showDebugLog)
        {
            Debug.LogWarning(
                $"[NPCAutoRoamController] {name} 检测到卡住 ({currentStuckRecoveryCount}/{maxStuckRecoveries})，最近位移 {lastProgressDistance:F3}m",
                this);
        }

        if (currentStuckRecoveryCount >= maxStuckRecoveries)
        {
            EnterShortPause(false);
            return true;
        }

        if (TryRebuildPath(currentPosition))
        {
            return false;
        }

        if (TryBeginMove())
        {
            return true;
        }

        EnterShortPause(false);
        return true;
    }

    private bool TryRebuildPath(Vector2 currentPosition)
    {
        if (!TryResolveNavGrid())
        {
            return false;
        }

        path.Clear();
        currentPathIndex = 0;

        if (!navGrid.TryFindPath(currentPosition, currentDestination, path) || path.Count == 0)
        {
            path.Clear();
            return false;
        }

        ResetMovementRecovery(currentPosition, resetCounter: false);

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=yellow>[NPCAutoRoamController]</color> {name} 卡住后重建路径 => Destination={currentDestination}, PathCount={path.Count}",
                this);
        }

        return true;
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

        if (!avoidance.HasBlockingAgent)
        {
            lastBlockingAgentId = 0;
            blockingAgentSightings = 0;
            adjustedDirection = avoidance.AdjustedDirection.sqrMagnitude > 0.0001f
                ? avoidance.AdjustedDirection
                : desiredDirection;
            speedScale = Mathf.Clamp01(avoidance.SpeedScale);
            return false;
        }

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
                GetAvoidanceRadius(),
                0.05f,
                avoidance);

        if (closeRangeConstraint.Applied)
        {
            adjustedDirection = closeRangeConstraint.ConstrainedDirection.sqrMagnitude > 0.0001f
                ? closeRangeConstraint.ConstrainedDirection
                : adjustedDirection;
            speedScale = Mathf.Clamp01(closeRangeConstraint.SpeedScale);
        }

        MaybeLogSharedAvoidance(navigationPosition, waypoint, avoidance, closeRangeConstraint, speedScale, "Move");

        if (!avoidance.ShouldRepath)
        {
            if (closeRangeConstraint.HardBlocked && speedScale <= 0.025f)
            {
                StopForSharedAvoidance(navigationPosition, avoidance, closeRangeConstraint, "HardStop");
                return true;
            }

            return false;
        }

        if (Time.time - lastSharedAvoidanceRepathTime < sharedAvoidanceRepathCooldown)
        {
            if (closeRangeConstraint.HardBlocked && speedScale <= 0.025f)
            {
                StopForSharedAvoidance(navigationPosition, avoidance, closeRangeConstraint, "HardStop");
                return true;
            }

            return false;
        }

        lastSharedAvoidanceRepathTime = Time.time;
        blockingAgentSightings = 0;
        StopForSharedAvoidance(navigationPosition, avoidance, closeRangeConstraint, "Repath");

        if (TryRebuildPath(movementPosition))
        {
            return true;
        }

        return false;
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
        return navigationCollider != null
            ? (Vector2)navigationCollider.bounds.center
            : (rb != null ? rb.position : (Vector2)transform.position);
    }

    private bool TryResolveNavGrid()
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

        if (!warnedMissingNavGrid)
        {
            warnedMissingNavGrid = true;
            Debug.LogWarning($"[NPCAutoRoamController] {name} 未找到 NavGrid2D，自动漫游暂时不会启动。", this);
        }

        return false;
    }

    private void EnterShortPause(bool countTowardLongPause)
    {
        BreakAmbientChatLink(hideBubble: true);
        ClearAmbientChatRetry();
        state = RoamState.ShortPause;
        stateTimer = Random.Range(shortPauseMin, shortPauseMax);
        path.Clear();
        currentPathIndex = 0;
        ResetMovementRecovery(transform.position, resetCounter: true);

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

    private void EnterLongPause()
    {
        BreakAmbientChatLink(hideBubble: true);
        ClearAmbientChatRetry();
        state = RoamState.LongPause;
        stateTimer = Random.Range(longPauseMin, longPauseMax);
        path.Clear();
        currentPathIndex = 0;
        ResetMovementRecovery(transform.position, resetCounter: true);

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

            bubblePresenter.ShowRandomSelfTalk(Mathf.Max(1f, stateTimer - 0.15f));
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

        if (!HasBubbleLines(chatInitiatorLines))
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

        StartAmbientChatRoutine(chatInitiatorLines, delay: 0f, duration);
        partner.StartAmbientChatRoutine(partner.chatResponderLines, ambientChatResponseDelay, duration);

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
            !HasBubbleLines(chatResponderLines) ||
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
        ResetMovementRecovery(transform.position, resetCounter: true);

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
        if (!isActiveAndEnabled || bubblePresenter == null || !HasBubbleLines(lines))
        {
            yield break;
        }

        float bubbleDuration = Mathf.Clamp(duration * 0.45f, 1.2f, 2.4f);
        bubblePresenter.ShowText(lines[Random.Range(0, lines.Length)], bubbleDuration);
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

    private void ResetMovementRecovery(Vector2 currentPosition, bool resetCounter)
    {
        lastProgressCheckPosition = currentPosition;
        lastProgressCheckTime = Time.time;
        lastProgressDistance = 0f;
        lastBlockingAgentId = 0;
        blockingAgentSightings = 0;

        if (resetCounter)
        {
            currentStuckRecoveryCount = 0;
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
