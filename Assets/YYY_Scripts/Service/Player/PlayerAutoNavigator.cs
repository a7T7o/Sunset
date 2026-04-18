using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自动导航（右键点击）- v5.3 ClosestPoint 统一版
/// 
/// 核心改进：
/// 1. 斜向移动时固定朝向为左/右，避免摇头
/// 2. 路径平滑处理，减少崎岖路线
/// 3. 详细的卡顿诊断输出
/// 4. 视线优化跳过中间路径点
/// 5. 🔥 v4.0：使用 ClosestPoint 计算距离，与 GameInputManager 统一
/// </summary>
public class PlayerAutoNavigator : MonoBehaviour, INavigationUnit
{
    private static readonly NavigationPathExecutor2D.DetourCandidateSpec[] SharedDynamicDetourCandidateSpecs =
    {
        new NavigationPathExecutor2D.DetourCandidateSpec(false, 1.25f, 0.45f),
        new NavigationPathExecutor2D.DetourCandidateSpec(false, 1.5f, 0.8f),
        new NavigationPathExecutor2D.DetourCandidateSpec(true, 1.4f, 0.7f),
        new NavigationPathExecutor2D.DetourCandidateSpec(true, 1.65f, 1.0f),
    };

    [Header("引用")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Transform player;
    [SerializeField] private NavGrid2D navGrid;

    [Header("停止距离")]
    [SerializeField, Range(0.05f, 1f)] private float stopDistance = 0.2f;
    [SerializeField, Range(0.05f, 0.5f)] private float waypointTolerance = 0.15f;

    [Header("目标点去抖")]
    [SerializeField, Range(0.1f, 1f)] private float destinationChangeThreshold = 0.3f;

    [Header("障碍物检测")]
    [SerializeField] private LayerMask losObstacleMask;
    [SerializeField] private string[] losObstacleTags = new string[0];

    [Header("路径优化")]
    [Tooltip("启用视线优化（跳过可直达的路径点）")]
    [SerializeField] private bool enableLineOfSightOptimization = true;
    [Tooltip("视线检测安全边距")]
    [SerializeField, Range(0.1f, 0.5f)] private float losSafetyMargin = 0.2f;

    [Header("调试")]
    [SerializeField] private bool showPathGizmos = true;
    [SerializeField] private bool enableDetailedDebug = false;

    [Header("动态导航障碍")]
    [SerializeField, Min(0.05f)] private float dynamicObstaclePadding = 0.15f;
    [SerializeField, Min(0.1f)] private float dynamicObstacleRepathCooldown = 0.45f;
    [SerializeField, Range(0.01f, 1f)] private float dynamicObstacleVelocityThreshold = 0.05f;
    [SerializeField, Min(8)] private int obstacleProbeBufferSize = 16;
    [SerializeField, Min(0.2f)] private float sharedAvoidanceLookAhead = 0.8f;
    [SerializeField] private int avoidancePriority = 100;

    // 私有字段
    private Collider2D playerCollider;
    private Rigidbody2D playerRigidbody;
    private float playerRadius;
    private bool active;
    private Vector3 targetPoint;
    private Transform targetTransform;
    private float followStopRadius = 0.6f;
    private bool runWhileNavigating;
    private readonly NavigationPathExecutor2D.ExecutionState navigationExecution = new NavigationPathExecutor2D.ExecutionState();
    private const float STUCK_THRESHOLD = 0.1f;
    private const float STUCK_CHECK_INTERVAL = 0.3f;
    private const int MAX_STUCK_RETRIES = 3;
    private const float DYNAMIC_DETOUR_MIN_ACTIVE_DURATION = 0.35f;
    private const int PASSIVE_NPC_BLOCKER_REPATH_MIN_SIGHTINGS = 24;
    private const float PASSIVE_NPC_BLOCKER_REPATH_CENTER_DISTANCE = 0.72f;
    private const float PASSIVE_NPC_CROWD_REPATH_CENTER_DISTANCE = 1.05f;
    private const int PASSIVE_NPC_CROWD_REPATH_MIN_BLOCKERS = 2;
    private const float PASSIVE_NPC_CROWD_REPATH_BACKWARD_TOLERANCE = 0.05f;
    private const float PASSIVE_NPC_CROWD_REPATH_LATERAL_DISTANCE = 0.78f;
    private const float PASSIVE_NPC_CROWD_DETOUR_KEEP_CENTER_DISTANCE = 2.35f;
    private const float PASSIVE_NPC_CROWD_DETOUR_KEEP_DESTINATION_DISTANCE = 0.85f;
    private const float PASSIVE_NPC_CROWD_DETOUR_KEEP_LATERAL_DISTANCE = 1.35f;
    private const float PASSIVE_NPC_CROWD_FORWARD_DETOUR_BLOCKER_FORWARD_SCALE = 1.45f;
    private const float PASSIVE_NPC_CROWD_FORWARD_DETOUR_BLOCKER_SIDESTEP_SCALE = 1.05f;
    private const float PASSIVE_NPC_CROWD_FORWARD_DETOUR_PLAYER_FORWARD_SCALE = 1.7f;
    private const float PASSIVE_NPC_CROWD_FORWARD_DETOUR_PLAYER_SIDESTEP_SCALE = 0.85f;
    private const int PASSIVE_NPC_WALL_MIN_BLOCKERS = 3;
    private const float PASSIVE_NPC_WALL_LOOKAHEAD_DISTANCE = 3.3f;
    private const float PASSIVE_NPC_WALL_MAX_FORWARD_SPAN = 1.05f;
    private const float PASSIVE_NPC_WALL_MAX_LATERAL_SPAN = 0.95f;
    private const float PASSIVE_NPC_WALL_TARGET_MAX_LATERAL_DISTANCE = 0.7f;
    private const int PASSIVE_NPC_WALL_CANCEL_MIN_SIGHTINGS = 4;
    private const float PASSIVE_NPC_WALL_CANCEL_BLOCKING_DISTANCE = 1.15f;
    private const float PASSIVE_NPC_WALL_CANCEL_MAX_MOVE_SCALE = 0.45f;
    private const int PASSIVE_NPC_SINGLE_STOP_JITTER_REPATH_MIN_SIGHTINGS = 4;
    private const float PASSIVE_NPC_SINGLE_STOP_JITTER_REPATH_DISTANCE = 0.52f;
    private const float PASSIVE_NPC_SINGLE_STOP_JITTER_CLEARANCE = -0.02f;
    private const float PASSIVE_NPC_SINGLE_STOP_JITTER_HARDSTOP_SOFTEN_CLEARANCE = -0.12f;
    private const int PASSIVE_NPC_SINGLE_PATHMOVE_REPATH_MIN_SIGHTINGS = 4;
    private const float PASSIVE_NPC_SINGLE_PATHMOVE_REPATH_BLOCKING_DISTANCE = 1.08f;
    private const float PASSIVE_NPC_SINGLE_PATHMOVE_REPATH_CENTER_DISTANCE = 1.02f;
    private const float PASSIVE_NPC_SINGLE_PATHMOVE_REPATH_MIN_MOVE_SCALE = 0.72f;
    private const float PASSIVE_NPC_SINGLE_PATHMOVE_REPATH_EARLY_BLOCKING_DISTANCE = 1.8f;
    private const float PASSIVE_NPC_SINGLE_PATHMOVE_REPATH_EARLY_CENTER_DISTANCE = 1.95f;
    private const float PASSIVE_NPC_CLOSE_CONSTRAINT_EXTRA_RADIUS = 0.05f;
    private const float PASSIVE_NPC_CLOSE_CONSTRAINT_MIN_FORWARD = 0.38f;
    private const float PASSIVE_NPC_CLOSE_CONSTRAINT_RELAX_CLEARANCE = 0.04f;
    private const float PASSIVE_NPC_CLOSE_CONSTRAINT_RECOVERY_SPEED = 0.42f;
    private const float PASSIVE_NPC_BLOCKED_INPUT_FORWARD = 0.48f;
    private const float PASSIVE_NPC_BLOCKED_INPUT_CLEARANCE = 0.018f;
    private const float PASSIVE_NPC_CLOSE_CONSTRAINT_SOFT_OVERLAP_CLEARANCE = -0.05f;
    private const float PASSIVE_NPC_CLOSE_CONSTRAINT_SOFT_OVERLAP_SPEED = 0.26f;
    private const float PASSIVE_NPC_CLOSE_CONSTRAINT_SOFT_OVERLAP_BLEND = 0.2f;
    private const float POINT_ARRIVAL_BLOCKER_RADIUS_PADDING = 0.08f;
    private const float POINT_ARRIVAL_BLOCKER_SETTLE_PADDING = 0.08f;
    private const float POINT_ARRIVAL_BLOCKER_REPATH_SUPPRESS_PADDING = 0.45f;
    private const float POST_DETOUR_POINT_ARRIVAL_SETTLE_DURATION = 0.2f;
    private const float POST_AVOIDANCE_POINT_ARRIVAL_HOLD_DURATION = 0.35f;
    private const float POST_AVOIDANCE_POINT_ARRIVAL_SETTLE_SPEED = 0.04f;
    private const float SAME_FRAME_PATH_REBUILD_START_THRESHOLD = 0.02f;
    private const float SAME_FRAME_PATH_REBUILD_DESTINATION_THRESHOLD = 0.02f;

    // 到达回调
    private System.Action _onArrivedCallback;
    
    // 🔥 P0-1：navToken 机制 - 隔离不同导航请求
    private int _navToken = 0;
    private int _currentToken = 0;

    // 调试信息
    private List<string> debugLogs = new List<string>();
    private Collider2D[] _obstacleProbeBuffer;
    private ContactFilter2D _obstacleProbeFilter;
    private float _lastDynamicObstacleRepathTime = float.NegativeInfinity;
    private int _lastDynamicObstacleId;
    private int _dynamicObstacleSightings;
    private readonly List<NavigationAgentSnapshot> _nearbyNavigationAgents = new List<NavigationAgentSnapshot>(12);
    private int _lastSharedAvoidanceDebugFrame = -999;
    private string _debugLastNavigationAction = "Idle";
    private float _debugLastMoveScale = 1f;
    private bool _debugLastAvoidanceShouldRepath;
    private bool _debugLastCloseConstraintApplied;
    private bool _debugLastCloseConstraintHardBlocked;
    private float _debugLastCloseConstraintClearance = float.PositiveInfinity;
    private float _debugLastCloseConstraintForwardInto;
    private float _debugLastBlockingDistance = float.PositiveInfinity;
    private bool _debugLastHasNpcBlocker;
    private bool _debugLastHasPassiveNpcBlocker;
    private float _pointArrivalCompletionHoldStartTime = float.NegativeInfinity;
    private Vector2 _pointArrivalCompletionHoldDestination;
    private int _lastPointArrivalGuardLogFrame = -999;
    private bool _hasLastBuildSignature;
    private int _lastBuildFrame = -1;
    private Vector2 _lastBuildStartPosition;
    private Vector2 _lastBuildRequestedDestination;
    private Transform _lastBuildTargetTransform;

    private List<Vector2> path => navigationExecution.Path;
    private int pathIndex { get => navigationExecution.PathIndex; set => navigationExecution.PathIndex = value; }
    private Vector2 lastCheckPosition { get => navigationExecution.LastProgressCheckPosition; set => navigationExecution.LastProgressCheckPosition = value; }
    private float lastCheckTime { get => navigationExecution.LastProgressCheckTime; set => navigationExecution.LastProgressCheckTime = value; }
    private int stuckRetryCount { get => navigationExecution.StuckRetryCount; set => navigationExecution.StuckRetryCount = value; }
    private bool _hasDynamicDetour => navigationExecution.HasOverrideWaypoint;

    public bool IsActive => active;
    public NavigationUnitType GetUnitType() => NavigationUnitType.Player;
    public Vector2 GetPosition() => GetPlayerPosition();
    public float GetColliderRadius() => playerRadius;
    public Vector2 GetCurrentVelocity() => playerRigidbody != null ? playerRigidbody.linearVelocity : Vector2.zero;
    public int GetAvoidancePriority() => avoidancePriority;
    public bool DebugHasDynamicDetour => navigationExecution.HasOverrideWaypoint;
    public int DebugDynamicDetourOwnerId => navigationExecution.OverrideWaypointOwnerId;
    public int DebugPathPointCount => path.Count;
    public int DebugPathIndex => pathIndex;
    public Vector2 DebugCurrentPathWaypoint =>
        pathIndex >= 0 && pathIndex < path.Count
            ? path[pathIndex]
            : Vector2.zero;
    public int DebugLastDynamicObstacleId => _lastDynamicObstacleId;
    public int DebugDynamicObstacleSightings => _dynamicObstacleSightings;
    public string DebugLastNavigationAction => _debugLastNavigationAction;
    public float DebugLastMoveScale => _debugLastMoveScale;
    public bool DebugLastAvoidanceShouldRepath => _debugLastAvoidanceShouldRepath;
    public bool DebugLastCloseConstraintApplied => _debugLastCloseConstraintApplied;
    public bool DebugLastCloseConstraintHardBlocked => _debugLastCloseConstraintHardBlocked;
    public float DebugLastCloseConstraintClearance => _debugLastCloseConstraintClearance;
    public float DebugLastCloseConstraintForwardInto => _debugLastCloseConstraintForwardInto;
    public float DebugLastBlockingDistance => _debugLastBlockingDistance;
    public bool DebugLastHasNpcBlocker => _debugLastHasNpcBlocker;
    public bool DebugLastHasPassiveNpcBlocker => _debugLastHasPassiveNpcBlocker;
    public Vector2 DebugTransformPosition => player != null ? (Vector2)player.position : Vector2.zero;
    public Vector2 DebugRigidbodyPosition => playerRigidbody != null ? playerRigidbody.position : DebugTransformPosition;
    public Vector2 DebugColliderCenter => playerCollider != null ? (Vector2)playerCollider.bounds.center : DebugRigidbodyPosition;
    public Vector2 DebugNavigationPositionOffset => DebugColliderCenter - DebugTransformPosition;
    public Vector2 DebugPathRequestDestination => GetPathRequestDestination();
    public Vector2 DebugResolvedPathDestination => GetResolvedPathDestination();
    public bool IsCurrentlyMoving()
    {
        float velocityThreshold = dynamicObstacleVelocityThreshold * dynamicObstacleVelocityThreshold;
        return GetCurrentVelocity().sqrMagnitude >= velocityThreshold;
    }
    public bool IsNavigationSleeping() => !active && !IsCurrentlyMoving();
    public bool ParticipatesInLocalAvoidance() => true;
    public bool ShouldAvoid(INavigationUnit other)
    {
        if (other == null) return false;

        NavigationUnitType otherType = other.GetUnitType();
        return otherType == NavigationUnitType.NPC || otherType == NavigationUnitType.Enemy;
    }
    public float GetAvoidanceRadius() => playerRadius + GetContactShellPadding();

    void Awake()
    {
        if (player == null) player = transform;
        if (movement == null) movement = GetComponent<PlayerMovement>();
        if (navGrid == null) navGrid = FindFirstObjectByType<NavGrid2D>();
        _obstacleProbeBuffer = new Collider2D[Mathf.Max(8, obstacleProbeBufferSize)];
        _obstacleProbeFilter = new ContactFilter2D().NoFilter();
        playerRigidbody = GetComponent<Rigidbody2D>();
        
        playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null) playerCollider = GetComponentInChildren<Collider2D>();
        
        if (playerCollider != null)
        {
            playerRadius = Mathf.Max(playerCollider.bounds.extents.x, playerCollider.bounds.extents.y);
            if (navGrid != null) navGrid.SetAgentRadius(playerRadius, rebuild: false);
        }
        else
        {
            playerRadius = 0.25f;
        }
    }

    void OnEnable()
    {
        NavigationAgentRegistry.Register(this);
    }

    void OnDisable()
    {
        NavigationAgentRegistry.Unregister(this);
    }

    void Update()
    {
        if (!active || movement == null || player == null) return;

        // 🔥 修复：跟随模式下，每帧重新计算最近接近点（目标可能移动，玩家位置也在变）
        if (targetTransform != null)
        {
            targetPoint = CalculateClosestApproachPoint(targetTransform);
        }

        UpdateSprintState();

        if (CheckAndHandleStuck()) return;

        ExecuteNavigation();
    }

    public void BindRuntimeSceneReferences(PlayerMovement boundMovement, Transform boundPlayer, NavGrid2D boundNavGrid)
    {
        movement = boundMovement != null ? boundMovement : GetComponent<PlayerMovement>();
        player = boundPlayer != null ? boundPlayer : transform;
        navGrid = boundNavGrid;
        playerRigidbody = player != null ? player.GetComponent<Rigidbody2D>() : GetComponent<Rigidbody2D>();
        playerCollider = player != null ? player.GetComponent<Collider2D>() : GetComponent<Collider2D>();
        if (playerCollider == null && player != null)
        {
            playerCollider = player.GetComponentInChildren<Collider2D>();
        }

        if (playerCollider != null)
        {
            playerRadius = Mathf.Max(playerCollider.bounds.extents.x, playerCollider.bounds.extents.y);
            if (navGrid != null)
            {
                navGrid.SetAgentRadius(playerRadius, rebuild: false);
            }
        }

        _cachedTargetTransform = null;
        _cachedTargetCollider = null;
        ForceCancel();
    }

    public void SetDestination(Vector3 worldPos)
    {
        if (active && Vector3.Distance(worldPos, targetPoint) < destinationChangeThreshold) return;

        targetTransform = null;
        targetPoint = worldPos;
        active = true;

        if (SprintStateManager.Instance != null)
            runWhileNavigating = SprintStateManager.Instance.ShouldNavigationSprint();

        BuildPath();
        ResetStuckDetection();
    }

    public void FollowTarget(Transform t, float stopRadius)
    {
        FollowTarget(t, stopRadius, null);
    }

    /// <summary>
    /// 跟随目标并在到达后执行回调
    /// </summary>
    /// <param name="t">目标 Transform</param>
    /// <param name="stopRadius">停止距离</param>
    /// <param name="onArrived">到达后的回调</param>
    public void FollowTarget(Transform t, float stopRadius, System.Action onArrived)
    {
        // 🔥 P0-1：先强制取消旧导航（不触发回调）
        ForceCancel();
        
        // 🔥 P0-1：递增 token，隔离新导航请求
        _navToken++;
        _currentToken = _navToken;
        
        // 清除缓存
        _cachedTargetTransform = null;
        _cachedTargetCollider = null;
        
        targetTransform = t;
        
        // 🔥 修复：计算玩家到目标的最近接近点，而不是目标中心
        targetPoint = CalculateClosestApproachPoint(t);
        
        // 🔥 修复：根据目标 Collider 大小动态调整停止半径，确保足够近
        followStopRadius = CalculateOptimalStopRadius(t, stopRadius);
        
        _onArrivedCallback = onArrived;
        active = true;

        if (SprintStateManager.Instance != null)
            runWhileNavigating = SprintStateManager.Instance.ShouldNavigationSprint();

        // 🔥 v4.0：关键日志 - 使用 ClosestPoint 计算距离
        Vector2 playerStart = GetPlayerPosition();
        Vector2 targetClosest = GetTargetAnchorPoint(t);
        float distToTarget = Vector2.Distance(playerStart, targetClosest);
        
        var collider = _cachedTargetCollider;
        if (collider != null)
        {
            Bounds bounds = collider.bounds;
            if (enableDetailedDebug)
            {
                Debug.Log($"<color=green>[Nav] 开始导航: 目标={t.name}</color>\n" +
                          $"  玩家起点: {playerStart}\n" +
                          $"  目标最近点(ClosestPoint): {targetClosest}, Collider大小: {(Vector2)bounds.size}\n" +
                          $"  当前距离: {distToTarget:F2}, 停止半径: {followStopRadius:F2}");
            }
        }
        else
        {
            if (enableDetailedDebug)
            {
                Debug.Log($"<color=green>[Nav] 开始导航: 目标={t.name}（无Collider）</color>\n" +
                          $"  玩家起点: {playerStart}\n" +
                          $"  目标位置: {(Vector2)t.position}\n" +
                          $"  停止半径: {followStopRadius:F2}");
            }
        }

        BuildPath();
        ResetStuckDetection();
    }

    /// <summary>
    /// 取消导航（不触发回调）- 用于卡顿、路径失败等
    /// 🔥 P0-1：Cancel 不再执行回调
    /// </summary>
    public void Cancel()
    {
        ResetNavigationState();
        // 🔥 P0-1：不执行回调
    }
    
    /// <summary>
    /// 强制取消导航（不触发回调）- 用于切换目标、玩家移动打断
    /// 🔥 P0-1：使旧 token 失效
    /// </summary>
    public void ForceCancel()
    {
        _navToken++;  // 使旧 token 失效
        ResetNavigationState();
    }
    
    /// <summary>
    /// 🔥 v4.0：正常到达时调用（唯一允许执行回调的入口）
    /// </summary>
    private void CompleteArrival()
    {
        // 🔥 v4.0：关键日志 - 使用 ClosestPoint 计算距离
        Vector2 playerFinal = GetPlayerPosition();
        string targetName = targetTransform != null ? targetTransform.name : "无目标";
        
        // 计算到目标最近点的距离（与 GameInputManager 一致）
        float distToTarget = 0f;
        if (targetTransform != null)
        {
            Vector2 targetClosest = GetTargetAnchorPoint(targetTransform);
            distToTarget = Vector2.Distance(playerFinal, targetClosest);
        }
        else
        {
            distToTarget = Vector2.Distance(playerFinal, targetPoint);
        }
        
        if (enableDetailedDebug)
        {
            Debug.Log($"<color=cyan>[Nav] 导航完成: 目标={(targetTransform != null ? targetName : "点导航")}</color>\n" +
                      $"  玩家最终位置: {playerFinal}\n" +
                      $"  导航解析终点: {GetResolvedPathDestination()}\n" +
                      $"  请求终点: {(Vector2)targetPoint}\n" +
                      $"  Transform位置: {DebugTransformPosition}\n" +
                      $"  Collider中心: {DebugColliderCenter}\n" +
                      $"  到目标最近点距离: {distToTarget:F2}\n" +
                      $"  停止半径: {followStopRadius:F2}");
        }
        
        // 只有 token 匹配才执行回调
        if (_currentToken == _navToken && _onArrivedCallback != null)
        {
            var callback = _onArrivedCallback;
            ResetNavigationState();
            callback.Invoke();
        }
        else
        {
            ResetNavigationState();
        }
    }
    
    /// <summary>
    /// 🔥 P0-1：重置导航状态（公共逻辑提取）
    /// </summary>
    private void ResetNavigationState()
    {
        active = false;
        targetTransform = null;
        _cachedTargetTransform = null;
        _cachedTargetCollider = null;
        NavigationPathExecutor2D.Clear(navigationExecution);
        _lastDynamicObstacleId = 0;
        _dynamicObstacleSightings = 0;
        runWhileNavigating = false;
        _onArrivedCallback = null;
        _hasLastBuildSignature = false;
        _lastBuildFrame = -1;
        _lastBuildTargetTransform = null;
        ResetAvoidanceDebugState("Inactive");
        ResetPointArrivalCompletionHold();
        if (movement != null) movement.SetMovementInput(Vector2.zero, false);
    }

    public void ToggleRunWhileNavigating() { }

    public void SetRunWhileNavigating(bool run) { runWhileNavigating = run; }

    private void ExecuteNavigation()
    {
        Vector2 playerPos = GetPlayerPosition();

        if (path.Count == 0 && !_hasDynamicDetour)
        {
            BuildPath();
            if (path.Count == 0) 
            { 
                if (HasReachedArrivalPoint(playerPos))
                {
                    CompleteArrival();
                }
                else if (TryHoldBlockedPointArrival(playerPos, "BuildPath path=0"))
                {
                    return;
                }
                else
                {
                    // 无法到达，取消导航
                    Cancel();
                }
                return; 
            }
        }

        // 视线优化：尝试跳过中间路径点
        if (!_hasDynamicDetour && enableLineOfSightOptimization)
        {
            TrySkipWaypoints(playerPos);
        }

        NavigationPathExecutor2D.WaypointResult waypointState = NavigationPathExecutor2D.EvaluateWaypoint(
            navigationExecution,
            playerPos,
            waypointTolerance,
            targetTransform != null ? waypointTolerance : GetFinalStopDistance(),
            1.25f);

        if (waypointState.ClearedOverrideWaypoint)
        {
            ResetAvoidanceDebugState();
            return;
        }

        if (TryFinalizeArrival(playerPos))
        {
            return;
        }

        if (waypointState.ReachedPathEnd)
        {
            if (TryFinalizeArrival(playerPos))
            {
                return;
            }

            AddDebugLog(
                $"ReachedPathEnd 但尚未接近真实终点，重建路径 => Player={playerPos}, Resolved={GetResolvedPathDestination()}, Requested={(Vector2)targetPoint}");
            BuildPath();
            ResetAvoidanceDebugState();
            if (path.Count == 0 && !HasReachedArrivalPoint(playerPos))
            {
                if (TryHoldBlockedPointArrival(playerPos, "ReachedPathEnd 重建后 path=0"))
                {
                    return;
                }

                Cancel();
            }
            return;
        }

        if (!waypointState.HasWaypoint)
        {
            if (TryHoldBlockedPointArrival(playerPos, "Waypoint 缺失"))
            {
                return;
            }

            Cancel();
            return;
        }

        Vector2 waypoint = waypointState.Waypoint;
        Vector2 toWaypoint = waypoint - playerPos;
        float distance = waypointState.DistanceToWaypoint;
        bool isLastWaypoint = !waypointState.UsingOverrideWaypoint && waypointState.IsLastPathWaypoint;

        Vector2 moveDir = toWaypoint.normalized;
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance = SolveSharedDynamicAvoidance(playerPos, moveDir);
        bool hasNpcBlocker = IsNpcBlocker(avoidance.BlockingAgentId, out NavigationAgentSnapshot npcBlocker);
        bool hasPassiveNpcBlocker = hasNpcBlocker && !npcBlocker.IsCurrentlyMoving;
        if (HandleSharedDynamicBlocker(playerPos, moveDir, avoidance))
        {
            return;
        }

        moveDir = avoidance.AdjustedDirection.sqrMagnitude > 0.0001f ? avoidance.AdjustedDirection : moveDir;
        float moveScale = Mathf.Clamp01(avoidance.SpeedScale);
        float? measuredClearance = TryMeasureBlockingClearance(playerPos, avoidance, hasNpcBlocker, npcBlocker);

        NavigationLocalAvoidanceSolver.CloseRangeConstraintResult closeRangeConstraint =
            NavigationLocalAvoidanceSolver.ApplyCloseRangeConstraint(
                playerPos,
                moveDir,
                moveScale,
                GetColliderRadius(),
                GetContactShellPadding(),
                avoidance,
                measuredClearance);

        closeRangeConstraint = MaybeRelaxPassiveNpcCloseConstraint(
            closeRangeConstraint,
            moveDir,
            moveScale,
            avoidance,
            hasPassiveNpcBlocker);

        _debugLastAvoidanceShouldRepath = avoidance.ShouldRepath;
        _debugLastCloseConstraintApplied = closeRangeConstraint.Applied;
        _debugLastCloseConstraintHardBlocked = closeRangeConstraint.HardBlocked;
        _debugLastCloseConstraintClearance = closeRangeConstraint.Clearance;
        _debugLastCloseConstraintForwardInto = closeRangeConstraint.ForwardIntoBlocker;
        _debugLastBlockingDistance = avoidance.HasBlockingAgent ? avoidance.BlockingDistance : float.PositiveInfinity;
        _debugLastHasNpcBlocker = hasNpcBlocker;
        _debugLastHasPassiveNpcBlocker = hasPassiveNpcBlocker;

        if (ShouldResetShortRangeAvoidanceProgress(hasPassiveNpcBlocker, avoidance, closeRangeConstraint))
        {
            NavigationPathExecutor2D.ResetProgress(navigationExecution, playerPos, true);
        }

        if (closeRangeConstraint.Applied)
        {
            moveDir = closeRangeConstraint.ConstrainedDirection.sqrMagnitude > 0.0001f
                ? closeRangeConstraint.ConstrainedDirection
                : moveDir;
            moveScale = Mathf.Clamp01(closeRangeConstraint.SpeedScale);

            if (ShouldApplyPassiveNpcCloseConstraintMoveFloor(hasPassiveNpcBlocker, avoidance, closeRangeConstraint))
            {
                moveScale = Mathf.Max(moveScale, GetPassiveNpcCloseConstraintMoveFloor(closeRangeConstraint));
            }
        }

        moveDir = AdjustDirectionByColliders(playerPos, moveDir);
        _debugLastMoveScale = moveScale;

        if (closeRangeConstraint.HardBlocked || moveScale <= 0.001f)
        {
            _debugLastNavigationAction = "HardStop";
            MaybeLogSharedAvoidance(playerPos, waypoint, avoidance, closeRangeConstraint, moveScale, "HardStop");
            ForceImmediateMovementStop();
            return;
        }

        Vector2 facingDir = GetFacingDirection(moveDir);
        string action = _hasDynamicDetour ? "DetourMove" : "PathMove";
        MaybeLogSharedAvoidance(playerPos, waypoint, avoidance, closeRangeConstraint, moveScale, action);

        if (ShouldUseBlockedNavigationInput(avoidance, closeRangeConstraint, moveScale))
        {
            _debugLastNavigationAction = "BlockedInput";
            movement.SetBlockedNavigationInput(
                moveDir * moveScale,
                runWhileNavigating,
                avoidance.BlockingAgentPosition,
                closeRangeConstraint.Clearance,
                facingDir);
            return;
        }

        _debugLastNavigationAction = action;
        movement.SetNavigationInput(moveDir * moveScale, runWhileNavigating, facingDir);
    }

    /// <summary>
    /// 🔥 获取朝向方向 - 斜向移动时固定为左或右
    /// </summary>
    private Vector2 GetFacingDirection(Vector2 moveDir)
    {
        // 如果主要是水平移动（|x| > |y|），使用移动方向
        if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
        {
            return moveDir;
        }
        
        // 如果主要是垂直移动，也使用移动方向
        if (Mathf.Abs(moveDir.y) > Mathf.Abs(moveDir.x) * 1.5f)
        {
            return moveDir;
        }
        
        // 斜向移动时，固定朝向为左或右
        // 根据 X 分量决定朝向
        if (moveDir.x > 0)
        {
            return Vector2.right; // 朝右
        }
        else if (moveDir.x < 0)
        {
            return Vector2.left; // 朝左
        }
        
        return moveDir;
    }

    /// <summary>
    /// 视线优化：尝试跳过可直达的中间路径点
    /// </summary>
    private void TrySkipWaypoints(Vector2 playerPos)
    {
        // 从当前路径点往后找，看能否直接到达更远的点
        for (int i = path.Count - 1; i > pathIndex; i--)
        {
            if (HasLineOfSight(playerPos, path[i]))
            {
                if (enableDetailedDebug && i > pathIndex + 1)
                {
                    Debug.Log($"<color=cyan>[Nav] 视线优化：跳过 {i - pathIndex - 1} 个路径点</color>");
                }
                pathIndex = i;
                break;
            }
        }
    }

    /// <summary>
    /// 视线检测：检查两点之间是否有障碍物
    /// 🔥 Unity 6 优化：使用 CircleCast 进行连续体积检测（考虑玩家半径）
    /// 🔥 策略：宁可误杀，不可放过 - 默认所有非 Trigger 的碰撞体都是障碍物
    /// 🔥 集成 NavGrid 检测，确保路径上所有点都是可走的
    /// </summary>
    private bool HasLineOfSight(Vector2 from, Vector2 to)
    {
        Vector2 direction = to - from;
        float distance = direction.magnitude;
        if (distance < 0.1f) return true;
        
        // 🔥 第一步：检查 NavGrid 可走性（关键！）
        // 这可以检测到水域、悬崖等没有物理碰撞体但不可走的区域
        if (navGrid != null)
        {
            int sampleCount = Mathf.Max(5, Mathf.CeilToInt(distance / 0.3f));
            for (int i = 1; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                Vector2 samplePoint = Vector2.Lerp(from, to, t);
                
                if (!navGrid.IsWalkable(samplePoint))
                {
                    _lastLOSBlocked = true;
                    return false;
                }
            }
        }
        
        // 🔥 第二步：使用 CircleCast 进行连续体积检测（考虑玩家半径）
        // CircleCast 会以一个"带半径的圆"扫过去，比 Linecast 更准确
        float checkRadius = Mathf.Max(0.05f, (playerRadius + losSafetyMargin) * 0.8f);
        
        // 使用配置的 LayerMask，如果没配置则检测所有层
        int layerMask = losObstacleMask.value != 0 ? losObstacleMask.value : Physics2D.DefaultRaycastLayers;
        
        RaycastHit2D hit = Physics2D.CircleCast(
            from,                       // 起点
            checkRadius,                // 检测半径（考虑玩家体积）
            direction.normalized,       // 方向
            distance,                   // 距离
            layerMask                   // 层级遮罩
        );
        
        // 如果没有碰撞，视线通畅
        if (hit.collider == null)
        {
            _lastLOSBlocked = false;
            return true;
        }
        
        // 跳过玩家自己
        if (IsPlayerCollider(hit.collider))
        {
            _lastLOSBlocked = false;
            return true;
        }
        
        // 检查是否是障碍物
        bool isObstacle = IsObstacle(hit.collider);
        
        if (isObstacle)
        {
            if (enableDetailedDebug)
            {
                Debug.Log($"<color=red>[Nav] 视线被阻挡: {hit.collider.name}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}, Tag: {hit.collider.tag}</color>");
            }
            _lastLOSHit = hit;
            _lastLOSBlocked = true;
            return false;
        }
        
        // 没有找到障碍物，视线通畅
        _lastLOSBlocked = false;
        return true;
    }
    
    // 🔥 调试可视化：记录最后一次视线检测的结果
    private RaycastHit2D _lastLOSHit;
    private bool _lastLOSBlocked;

    private void MoveDirectly(Vector2 playerPos)
    {
        Vector2 toTarget = (Vector2)targetPoint - playerPos;
        if (toTarget.magnitude <= GetFinalStopDistance()) { CompleteArrival(); return; }  // 🔥 P0-1：使用 CompleteArrival
        
        Vector2 moveDir = toTarget.normalized;
        Vector2 facingDir = GetFacingDirection(moveDir);
        movement.SetNavigationInput(moveDir, runWhileNavigating, facingDir);
    }

    private void BuildPath()
    {
        Vector2 playerPosition = GetPlayerPosition();
        Vector2 requestedDestination = GetPathRequestDestination();
        if (ShouldSkipSameFramePathRebuild(playerPosition, requestedDestination))
        {
            return;
        }

        debugLogs.Clear();
        string pathSummary = $"开始寻路: 起点={playerPosition}, 终点={targetPoint}" +
            (targetTransform != null ? $", 目标={targetTransform.name}" : string.Empty);
        AddDebugLog(pathSummary);

        if (targetTransform == null && Vector2.Distance(requestedDestination, (Vector2)targetPoint) > 0.01f)
        {
            AddDebugLog($"点导航目标换算: 点击点={(Vector2)targetPoint}, 导航中心终点={requestedDestination}");
        }

        NavigationPathExecutor2D.BuildPathResult buildResult = NavigationPathExecutor2D.TryRefreshPath(
            navigationExecution,
            navGrid,
            playerPosition,
            requestedDestination,
            HasLineOfSight,
            playerPosition,
            waypointTolerance,
            AddDebugLog);

        if (!buildResult.Success)
        {
            LogFullDebugInfo(buildResult.FailureReason);
            return;
        }

        if (targetTransform == null && Vector2.Distance((Vector2)targetPoint, buildResult.ActualDestination) > 0.01f)
        {
            AddDebugLog($"终点被调整: 请求={(Vector2)targetPoint}, 实际={buildResult.ActualDestination}");
        }
        
        if (enableDetailedDebug)
        {
            Debug.Log($"<color=green>[Nav] 路径构建成功：{path.Count} 个路径点</color>");
        }

        _hasLastBuildSignature = true;
        _lastBuildFrame = Time.frameCount;
        _lastBuildStartPosition = playerPosition;
        _lastBuildRequestedDestination = requestedDestination;
        _lastBuildTargetTransform = targetTransform;
    }

    private bool CheckAndHandleStuck()
    {
        Vector2 currentPos = GetPlayerPosition();
        if (IsDynamicDetourProtected(Time.time))
        {
            return false;
        }

        if (ShouldSuppressShortRangeAvoidanceStuckCheck(currentPos))
        {
            NavigationPathExecutor2D.ResetProgress(navigationExecution, currentPos, true);
            return false;
        }

        NavigationPathExecutor2D.ProgressCheckResult progress = NavigationPathExecutor2D.EvaluateStuck(
            navigationExecution,
            currentPos,
            Time.time,
            STUCK_CHECK_INTERVAL,
            STUCK_THRESHOLD,
            MAX_STUCK_RETRIES);

        if (!progress.Checked || !progress.IsStuck)
        {
            return false;
        }

        AddDebugLog($"检测到卡顿 ({progress.RetryCount}/{MAX_STUCK_RETRIES})，移动距离={progress.MovedDistance:F3}m");

        if (progress.ShouldCancel)
        {
            LogFullDebugInfo($"卡顿 {progress.RetryCount} 次后取消导航");
            Debug.LogWarning($"<color=red>[Nav] 卡顿 {progress.RetryCount} 次，取消导航</color>");
            Cancel();
            return true;
        }

        Debug.Log($"<color=yellow>[Nav] 检测到卡顿（{progress.RetryCount}/{MAX_STUCK_RETRIES}），重建路径</color>");
        BuildPath();

        if (path.Count == 0)
        {
            LogFullDebugInfo("重建路径失败");
            Cancel();
            return true;
        }

        return false;
    }

    private bool ShouldSuppressShortRangeAvoidanceStuckCheck(Vector2 currentPos)
    {
        if (_hasDynamicDetour ||
            !_debugLastHasPassiveNpcBlocker ||
            !_debugLastAvoidanceShouldRepath ||
            _debugLastCloseConstraintHardBlocked ||
            _debugLastMoveScale > 0.45f ||
            (_debugLastNavigationAction != "BlockedInput" && !_debugLastCloseConstraintApplied))
        {
            return false;
        }

        if (_debugLastBlockingDistance <= PASSIVE_NPC_BLOCKER_REPATH_CENTER_DISTANCE + 0.2f)
        {
            return _debugLastCloseConstraintClearance > PASSIVE_NPC_CLOSE_CONSTRAINT_SOFT_OVERLAP_CLEARANCE;
        }

        Vector2 currentWaypoint =
            pathIndex >= 0 && pathIndex < path.Count
                ? path[pathIndex]
                : GetResolvedPathDestination();

        return _debugLastCloseConstraintClearance > -0.01f ||
            Vector2.Distance(currentPos, currentWaypoint) <= PASSIVE_NPC_BLOCKER_REPATH_CENTER_DISTANCE + 0.2f;
    }

    private void ResetStuckDetection()
    {
        NavigationPathExecutor2D.ResetProgress(navigationExecution, GetPlayerPosition(), true);
        debugLogs.Clear();
        _lastDynamicObstacleId = 0;
        _dynamicObstacleSightings = 0;
        ResetAvoidanceDebugState("Idle");
        ResetPointArrivalCompletionHold();
    }

    private void ResetAvoidanceDebugState(string action = null)
    {
        if (!string.IsNullOrEmpty(action))
        {
            _debugLastNavigationAction = action;
        }
        _debugLastMoveScale = 1f;
        _debugLastAvoidanceShouldRepath = false;
        _debugLastCloseConstraintApplied = false;
        _debugLastCloseConstraintHardBlocked = false;
        _debugLastCloseConstraintClearance = float.PositiveInfinity;
        _debugLastCloseConstraintForwardInto = 0f;
        _debugLastBlockingDistance = float.PositiveInfinity;
        _debugLastHasNpcBlocker = false;
        _debugLastHasPassiveNpcBlocker = false;
    }

    private void ResetPointArrivalCompletionHold()
    {
        _pointArrivalCompletionHoldStartTime = float.NegativeInfinity;
        _pointArrivalCompletionHoldDestination = Vector2.zero;
    }

    private void MaybeLogPointArrivalGuard(string reason, Vector2 playerPos)
    {
        if (!enableDetailedDebug || Time.frameCount - _lastPointArrivalGuardLogFrame < 15)
        {
            return;
        }

        _lastPointArrivalGuardLogFrame = Time.frameCount;
        Debug.Log(
            $"<color=yellow>[Nav] 点导航完成保护: {reason}</color>\n" +
            $"  Player={playerPos}\n" +
            $"  Resolved={GetResolvedPathDestination()}\n" +
            $"  Requested={(Vector2)targetPoint}\n" +
            $"  Transform={DebugTransformPosition}\n" +
            $"  Collider={DebugColliderCenter}\n" +
            $"  PathIndex={pathIndex}/{path.Count}\n" +
            $"  HasDetour={_hasDynamicDetour}\n" +
            $"  LastDetourClear={navigationExecution.LastDetourClearTime:F2}\n" +
            $"  LastDetourRecover={navigationExecution.LastDetourRecoveryTime:F2}\n" +
            $"  Velocity={GetCurrentVelocity()}");
    }

    private bool TryFinalizeArrival(Vector2 playerPos)
    {
        if (!HasReachedArrivalPoint(playerPos))
        {
            ResetPointArrivalCompletionHold();
            return false;
        }

        if (ShouldDeferActiveDetourPointArrival(playerPos))
        {
            return false;
        }

        if (TryHoldPostAvoidancePointArrival(playerPos))
        {
            return true;
        }

        ResetPointArrivalCompletionHold();
        CompleteArrival();
        return true;
    }

    private bool ShouldDeferActiveDetourPointArrival(Vector2 playerPos)
    {
        if (targetTransform != null || !_hasDynamicDetour)
        {
            return false;
        }

        MaybeLogPointArrivalGuard("仍处于 detour waypoint，先不执行点导航完成", playerPos);
        return true;
    }

    private bool TryHoldBlockedPointArrival(Vector2 playerPos, string source)
    {
        if (!ShouldHoldBlockedPointArrival(playerPos, out NavigationAgentSnapshot blocker, out string reason))
        {
            return false;
        }

        _debugLastNavigationAction = "BlockedInput";
        _debugLastMoveScale = 0f;
        _debugLastHasNpcBlocker = true;
        _debugLastHasPassiveNpcBlocker = !blocker.IsCurrentlyMoving;
        _debugLastBlockingDistance = Vector2.Distance(playerPos, blocker.Position);
        AddDebugLog($"{source}：{reason}");
        ForceImmediateMovementStop();
        return true;
    }

    private bool TryHoldPostAvoidancePointArrival(Vector2 playerPos)
    {
        if (!ShouldHoldPostAvoidancePointArrival(playerPos, out string reason))
        {
            return false;
        }

        _debugLastNavigationAction = "BlockedInput";
        _debugLastMoveScale = 0f;
        AddDebugLog(reason);
        MaybeLogPointArrivalGuard(reason, playerPos);
        ForceImmediateMovementStop();
        return true;
    }

    private bool ShouldHoldBlockedPointArrival(
        Vector2 playerPos,
        out NavigationAgentSnapshot blocker,
        out string reason)
    {
        blocker = default;
        reason = null;

        if (targetTransform != null)
        {
            return false;
        }

        Vector2 resolvedDestination = GetResolvedPathDestination();
        if (TryGetPointArrivalNpcBlocker(resolvedDestination, out blocker))
        {
            if (IsWithinPointArrivalBlockerSettleWindow(playerPos, blocker, resolvedDestination))
            {
                reason =
                    $"点导航已进入终点 blocker keep 窗口，继续保持导航 => Blocker={blocker.InstanceId}, " +
                    $"Dest={resolvedDestination}, Npc={blocker.Position}, Player={playerPos}";
                return true;
            }

            reason =
                $"点导航终点附近仍有 NPC 占位，暂不完成/取消 => Blocker={blocker.InstanceId}, Dest={resolvedDestination}, Npc={blocker.Position}";
            return true;
        }

        if (!_hasDynamicDetour &&
            navigationExecution.LastDetourClearTime > float.NegativeInfinity &&
            Time.time - navigationExecution.LastDetourClearTime < POST_DETOUR_POINT_ARRIVAL_SETTLE_DURATION)
        {
            reason =
                $"detour 刚释放，保留一个短 settle 窗口后再决定是否完成 => ClearedAt={navigationExecution.LastDetourClearTime:F2}";
            return true;
        }

        return false;
    }

    private bool ShouldHoldPostAvoidancePointArrival(
        Vector2 playerPos,
        out string reason)
    {
        reason = null;

        if (targetTransform != null)
        {
            ResetPointArrivalCompletionHold();
            return false;
        }

        float currentTime = Time.time;
        bool hasRecentDetourClear =
            navigationExecution.LastDetourClearTime > float.NegativeInfinity &&
            currentTime - navigationExecution.LastDetourClearTime < POST_AVOIDANCE_POINT_ARRIVAL_HOLD_DURATION;
        bool hasRecentDetourRecovery =
            navigationExecution.LastDetourRecoverySucceeded &&
            navigationExecution.LastDetourRecoveryTime > float.NegativeInfinity &&
            currentTime - navigationExecution.LastDetourRecoveryTime < POST_AVOIDANCE_POINT_ARRIVAL_HOLD_DURATION;

        if (!hasRecentDetourClear && !hasRecentDetourRecovery)
        {
            ResetPointArrivalCompletionHold();
            return false;
        }

        float latestAvoidanceEventTime = hasRecentDetourClear
            ? navigationExecution.LastDetourClearTime
            : float.NegativeInfinity;
        if (hasRecentDetourRecovery)
        {
            latestAvoidanceEventTime = Mathf.Max(latestAvoidanceEventTime, navigationExecution.LastDetourRecoveryTime);
        }

        Vector2 resolvedDestination = GetResolvedPathDestination();
        if (_pointArrivalCompletionHoldStartTime <= float.NegativeInfinity ||
            Vector2.Distance(_pointArrivalCompletionHoldDestination, resolvedDestination) > 0.05f)
        {
            _pointArrivalCompletionHoldStartTime = currentTime;
            _pointArrivalCompletionHoldDestination = resolvedDestination;
        }

        float holdDuration = currentTime - _pointArrivalCompletionHoldStartTime;
        float currentSpeed = GetCurrentVelocity().magnitude;
        float avoidanceEventAge =
            latestAvoidanceEventTime > float.NegativeInfinity
                ? currentTime - latestAvoidanceEventTime
                : holdDuration;
        if (avoidanceEventAge >= POST_AVOIDANCE_POINT_ARRIVAL_HOLD_DURATION &&
            currentSpeed <= POST_AVOIDANCE_POINT_ARRIVAL_SETTLE_SPEED)
        {
            return false;
        }

        reason =
            $"detour/recover 后延迟点导航完成，等待 settle => Hold={holdDuration:F2}, EventAge={avoidanceEventAge:F2}, Speed={currentSpeed:F2}, " +
            $"Resolved={resolvedDestination}, LastClear={navigationExecution.LastDetourClearTime:F2}, " +
            $"LastRecover={navigationExecution.LastDetourRecoveryTime:F2}, Player={playerPos}";
        return true;
    }

    private bool TryGetPointArrivalNpcBlocker(
        Vector2 destination,
        out NavigationAgentSnapshot blocker)
    {
        float queryRadius =
            GetColliderRadius() +
            GetPointArrivalBlockerRadiusUpperBound() +
            GetContactShellPadding() +
            POINT_ARRIVAL_BLOCKER_RADIUS_PADDING;

        blocker = default;
        float bestDestinationDistance = float.PositiveInfinity;
        NavigationAgentRegistry.GetNearbySnapshots(this, destination, queryRadius, _nearbyNavigationAgents);
        for (int i = 0; i < _nearbyNavigationAgents.Count; i++)
        {
            NavigationAgentSnapshot candidate = _nearbyNavigationAgents[i];
            if (candidate.UnitType != NavigationUnitType.NPC)
            {
                continue;
            }

            float destinationDistance = Vector2.Distance(candidate.Position, destination);
            if (destinationDistance > GetPointArrivalBlockerOccupyDistance(candidate))
            {
                continue;
            }

            if (destinationDistance < bestDestinationDistance)
            {
                bestDestinationDistance = destinationDistance;
                blocker = candidate;
            }
        }

        return blocker.InstanceId != 0;
    }

    private float? TryMeasureBlockingClearance(
        Vector2 playerPos,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        bool hasNpcBlocker,
        NavigationAgentSnapshot npcBlocker)
    {
        if (!avoidance.HasBlockingAgent)
        {
            return null;
        }

        float effectiveBlockerRadius = hasNpcBlocker
            ? Mathf.Min(
                npcBlocker.ColliderRadius,
                GetColliderRadius() + PASSIVE_NPC_CLOSE_CONSTRAINT_EXTRA_RADIUS)
            : Mathf.Max(avoidance.BlockingAgentRadius, 0.01f);

        return Vector2.Distance(playerPos, avoidance.BlockingAgentPosition) -
            (GetColliderRadius() + Mathf.Max(effectiveBlockerRadius, 0.01f));
    }

    private static bool ShouldResetShortRangeAvoidanceProgress(
        bool hasPassiveNpcBlocker,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        NavigationLocalAvoidanceSolver.CloseRangeConstraintResult closeRangeConstraint)
    {
        return hasPassiveNpcBlocker &&
            avoidance.ShouldRepath &&
            avoidance.BlockingDistance <= PASSIVE_NPC_BLOCKER_REPATH_CENTER_DISTANCE &&
            closeRangeConstraint.Applied &&
            !closeRangeConstraint.HardBlocked;
    }

    private static bool ShouldApplyPassiveNpcCloseConstraintMoveFloor(
        bool hasPassiveNpcBlocker,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        NavigationLocalAvoidanceSolver.CloseRangeConstraintResult closeRangeConstraint)
    {
        return hasPassiveNpcBlocker &&
            avoidance.HasBlockingAgent &&
            avoidance.BlockingDistance <= PASSIVE_NPC_BLOCKER_REPATH_CENTER_DISTANCE &&
            !closeRangeConstraint.HardBlocked &&
            closeRangeConstraint.Clearance > 0f;
    }

    private Vector2 AdjustDirectionByColliders(Vector2 pos, Vector2 desiredDir)
    {
        if (playerCollider == null) return desiredDir;

        // 多点前瞻采样
        float[] aheadDistances = runWhileNavigating 
            ? new float[] { 0.15f, 0.35f, 0.6f }
            : new float[] { 0.1f, 0.25f, 0.45f };
        
        float clearance = GetAvoidanceRadius();
        Vector2 totalRepulse = Vector2.zero;
        int obstacleCount = 0;
        
        foreach (float ahead in aheadDistances)
        {
            Vector2 probe = pos + desiredDir * ahead;
            int hitCount = ProbeObstacleHits(probe, clearance);
            float weight = 1f / (ahead + 0.1f);
            
            for (int i = 0; i < hitCount; i++)
            {
                var hit = _obstacleProbeBuffer[i];
                if (TryGetNavigationUnit(hit, out _)) continue;
                if (IsPlayerCollider(hit) || !IsObstacle(hit)) continue;
                obstacleCount++;
                
                Vector2 closest = hit.ClosestPoint(probe);
                Vector2 away = probe - closest;
                float dist = away.magnitude;
                
                if (dist < 0.01f)
                {
                    away = probe - (Vector2)hit.bounds.center;
                    dist = away.magnitude;
                }
                
                if (dist > 0.01f)
                {
                    float repulseStrength = 1f / (dist * dist + 0.1f);
                    totalRepulse += away.normalized * repulseStrength * weight;
                }
            }
        }

        // 如果没有障碍物，直接返回期望方向
        if (obstacleCount == 0 || totalRepulse.sqrMagnitude < 0.0001f) return desiredDir;

        // 计算调整后的方向
        Vector2 adjusted = (desiredDir + totalRepulse * 0.6f).normalized;
        
        // 限制最大偏转角度为 45 度
        float angle = Vector2.SignedAngle(desiredDir, adjusted);
        if (Mathf.Abs(angle) > 45f)
        {
            adjusted = RotateVector(desiredDir, Mathf.Sign(angle) * 45f);
        }

        return adjusted.sqrMagnitude > 0.001f ? adjusted : desiredDir;
    }

    private NavigationLocalAvoidanceSolver.AvoidanceResult SolveSharedDynamicAvoidance(Vector2 playerPos, Vector2 desiredDir)
    {
        NavigationAgentSnapshot self = NavigationAgentSnapshot.FromUnit(this);
        if (!self.IsValid)
        {
            return new NavigationLocalAvoidanceSolver.AvoidanceResult(desiredDir, 1f, false, false, 0, float.PositiveInfinity, Vector2.zero, 0f, Vector2.zero);
        }

        float desiredSpeed = GetRequestedMoveSpeed();
        float lookAhead = NavigationLocalAvoidanceSolver.GetRecommendedLookAhead(self, sharedAvoidanceLookAhead, desiredSpeed);
        NavigationAgentRegistry.GetNearbySnapshots(this, playerPos, lookAhead, _nearbyNavigationAgents);
        return NavigationLocalAvoidanceSolver.Solve(self, desiredDir, lookAhead, _nearbyNavigationAgents);
    }

    private bool HandleSharedDynamicBlocker(
        Vector2 playerPos,
        Vector2 desiredDirection,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance)
    {
        if (!avoidance.HasBlockingAgent)
        {
            _lastDynamicObstacleId = 0;
            _dynamicObstacleSightings = 0;
            if (ShouldCancelPassiveNpcWallPointNavigationWithoutActiveBlocker(playerPos))
            {
                AddDebugLog("静止 NPC 墙前取消点导航，detour 不再继续绕到被堵目标后方");
                Cancel();
                ForceImmediateMovementStop();
                return true;
            }

            if (TryReleaseDynamicDetour(playerPos, Time.time))
            {
                ResetAvoidanceDebugState();
            }

            return false;
        }

        if (_lastDynamicObstacleId == avoidance.BlockingAgentId)
        {
            _dynamicObstacleSightings++;
        }
        else
        {
            _lastDynamicObstacleId = avoidance.BlockingAgentId;
            _dynamicObstacleSightings = 1;
        }

        if (TryReleaseDynamicDetourForSwitchedBlocker(playerPos, avoidance, Time.time))
        {
            _lastDynamicObstacleRepathTime = Time.time;
            ResetAvoidanceDebugState();
            return false;
        }

        if (ShouldCancelPassiveNpcWallPointNavigation(playerPos, desiredDirection, avoidance))
        {
            AddDebugLog(
                $"静止 NPC 墙前取消点导航，避免继续绕/挤到被堵目标后方 => AgentId={avoidance.BlockingAgentId}, " +
                $"BlockDist={Mathf.Min(avoidance.BlockingDistance, _debugLastBlockingDistance):F2}, Sightings={_dynamicObstacleSightings}");
            Cancel();
            ForceImmediateMovementStop();
            return true;
        }

        bool shouldEscalatePassivePathMove = ShouldBreakSinglePassiveNpcPathMoveBulldoze(
            playerPos,
            avoidance);

        if (!avoidance.ShouldRepath && !shouldEscalatePassivePathMove)
        {
            return false;
        }

        if (ShouldDeferPassiveNpcBlockerRepath(playerPos, desiredDirection, avoidance))
        {
            return false;
        }

        if (Time.time - _lastDynamicObstacleRepathTime < dynamicObstacleRepathCooldown)
        {
            return false;
        }

        _lastDynamicObstacleRepathTime = Time.time;
        _dynamicObstacleSightings = 0;
        NavigationPathExecutor2D.ResetProgress(navigationExecution, playerPos, true);

        if (shouldEscalatePassivePathMove && !avoidance.ShouldRepath)
        {
            AddDebugLog(
                $"共享动态代理进入 passive PathMove 推土机升级 => AgentId={avoidance.BlockingAgentId}, BlockDist={avoidance.BlockingDistance:F2}, Sightings={_dynamicObstacleSightings}");
        }

        if (TryCreateDynamicDetour(playerPos, avoidance))
        {
            AddDebugLog($"共享动态代理持续阻挡，切入临时绕行点 => AgentId={avoidance.BlockingAgentId}");
            ResetAvoidanceDebugState();
            ForceImmediateMovementStop();
            return true;
        }

        AddDebugLog($"共享动态代理持续阻挡，重建路径 => AgentId={avoidance.BlockingAgentId}");
        BuildPath();
        ResetAvoidanceDebugState();
        ForceImmediateMovementStop();
        return true;
    }

    private bool TryReleaseDynamicDetourForSwitchedBlocker(
        Vector2 playerPos,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        float currentTime)
    {
        if (!_hasDynamicDetour)
        {
            return false;
        }

        int detourOwnerId = navigationExecution.OverrideWaypointOwnerId;
        if (detourOwnerId == 0 || detourOwnerId == avoidance.BlockingAgentId)
        {
            return false;
        }

        if (IsPassiveNpcBlocker(avoidance.BlockingAgentId, out _) &&
            ShouldKeepPassiveNpcCrowdDetour(playerPos))
        {
            return false;
        }

        NavigationPathExecutor2D.DetourLifecycleResult detour = NavigationPathExecutor2D.TryClearDetourAndRecover(
            navigationExecution,
            navGrid,
            playerPos,
            GetResolvedPathDestination(),
            0,
            currentTime,
            rebuildPath: false,
            minimumDetourActiveDuration: DYNAMIC_DETOUR_MIN_ACTIVE_DURATION,
            recoveryCooldown: 0f,
            hasLineOfSight: null,
            cleanupReferencePosition: null,
            cleanupWaypointTolerance: 0f,
            log: AddDebugLog,
            ignoredCollider: playerCollider);

        if (detour.ShouldKeepCurrentDetour || detour.HasActiveDetour)
        {
            return false;
        }

        if (detour.Cleared || detour.Recovered)
        {
            _dynamicObstacleSightings = 0;
            AddDebugLog(
                $"共享动态代理切换 blocker，释放旧 detour => OldOwner={detourOwnerId}, CurrentBlocker={avoidance.BlockingAgentId}");
            return true;
        }

        return false;
    }

    private bool ShouldDeferPassiveNpcBlockerRepath(
        Vector2 playerPos,
        Vector2 desiredDirection,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance)
    {
        if (_hasDynamicDetour ||
            !TryGetNearbyNavigationAgentSnapshot(avoidance.BlockingAgentId, out NavigationAgentSnapshot blocker) ||
            blocker.UnitType != NavigationUnitType.NPC ||
            blocker.IsCurrentlyMoving)
        {
            return false;
        }

        if (ShouldTreatPassiveNpcWallAsBlockedTarget(playerPos, blocker))
        {
            return true;
        }

        if (ShouldSuppressPassiveNpcArrivalRepath(playerPos, blocker))
        {
            return true;
        }

        if (CountNearbyPassiveNpcBlockers(
                playerPos,
                desiredDirection,
                PASSIVE_NPC_CROWD_REPATH_CENTER_DISTANCE) >=
            PASSIVE_NPC_CROWD_REPATH_MIN_BLOCKERS)
        {
            return false;
        }

        if (ShouldBreakSinglePassiveNpcStopJitter(avoidance) ||
            ShouldBreakSinglePassiveNpcPathMoveBulldoze(playerPos, avoidance, blocker))
        {
            return false;
        }

        float centerDistance = Vector2.Distance(playerPos, blocker.Position);
        return centerDistance > PASSIVE_NPC_BLOCKER_REPATH_CENTER_DISTANCE ||
            _dynamicObstacleSightings < PASSIVE_NPC_BLOCKER_REPATH_MIN_SIGHTINGS;
    }

    private bool ShouldBreakSinglePassiveNpcStopJitter(
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance)
    {
        if (_hasDynamicDetour ||
            !avoidance.HasBlockingAgent ||
            !_debugLastHasPassiveNpcBlocker ||
            _lastDynamicObstacleId != avoidance.BlockingAgentId ||
            _dynamicObstacleSightings < PASSIVE_NPC_SINGLE_STOP_JITTER_REPATH_MIN_SIGHTINGS)
        {
            return false;
        }

        if (_debugLastNavigationAction != "HardStop" &&
            _debugLastNavigationAction != "BlockedInput")
        {
            return false;
        }

        if (!_debugLastCloseConstraintApplied && !_debugLastCloseConstraintHardBlocked)
        {
            return false;
        }

        if (_debugLastCloseConstraintClearance > PASSIVE_NPC_SINGLE_STOP_JITTER_CLEARANCE)
        {
            return false;
        }

        if (_debugLastNavigationAction == "HardStop" || _debugLastCloseConstraintHardBlocked)
        {
            return true;
        }

        return Mathf.Min(avoidance.BlockingDistance, _debugLastBlockingDistance) <=
            PASSIVE_NPC_SINGLE_STOP_JITTER_REPATH_DISTANCE;
    }

    private bool ShouldBreakSinglePassiveNpcPathMoveBulldoze(
        Vector2 playerPos,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        NavigationAgentSnapshot blocker = default)
    {
        if (blocker.InstanceId == 0 &&
            (!TryGetNearbyNavigationAgentSnapshot(avoidance.BlockingAgentId, out blocker) ||
             blocker.UnitType != NavigationUnitType.NPC ||
             blocker.IsCurrentlyMoving))
        {
            return false;
        }

        if (_hasDynamicDetour ||
            !avoidance.HasBlockingAgent ||
            _dynamicObstacleSightings < PASSIVE_NPC_SINGLE_PATHMOVE_REPATH_MIN_SIGHTINGS)
        {
            return false;
        }

        if (CountNearbyPassiveNpcBlockers(
                playerPos,
                avoidance.AdjustedDirection.sqrMagnitude > 0.0001f
                    ? avoidance.AdjustedDirection
                    : (blocker.Position - playerPos),
                PASSIVE_NPC_CROWD_REPATH_CENTER_DISTANCE) >=
            PASSIVE_NPC_CROWD_REPATH_MIN_BLOCKERS)
        {
            return false;
        }

        float centerDistance = Vector2.Distance(playerPos, blocker.Position);
        float blockingDistance = Mathf.Min(avoidance.BlockingDistance, _debugLastBlockingDistance);
        bool sustainedActivePush =
            (_debugLastNavigationAction == "PathMove" || _debugLastNavigationAction == "DetourMove") &&
            Mathf.Max(_debugLastMoveScale, avoidance.SpeedScale) >= PASSIVE_NPC_SINGLE_PATHMOVE_REPATH_MIN_MOVE_SCALE;

        if (centerDistance > PASSIVE_NPC_SINGLE_PATHMOVE_REPATH_CENTER_DISTANCE &&
            blockingDistance > PASSIVE_NPC_SINGLE_PATHMOVE_REPATH_BLOCKING_DISTANCE)
        {
            if (!sustainedActivePush ||
                centerDistance > PASSIVE_NPC_SINGLE_PATHMOVE_REPATH_EARLY_CENTER_DISTANCE ||
                blockingDistance > PASSIVE_NPC_SINGLE_PATHMOVE_REPATH_EARLY_BLOCKING_DISTANCE)
            {
                return false;
            }
        }

        if (_debugLastMoveScale < PASSIVE_NPC_SINGLE_PATHMOVE_REPATH_MIN_MOVE_SCALE &&
            avoidance.SpeedScale < PASSIVE_NPC_SINGLE_PATHMOVE_REPATH_MIN_MOVE_SCALE)
        {
            return false;
        }

        return true;
    }

    private int CountNearbyPassiveNpcBlockers(
        Vector2 playerPos,
        Vector2 desiredDirection,
        float maxCenterDistance)
    {
        return CountNearbyPassiveNpcBlockers(
            playerPos,
            desiredDirection,
            maxCenterDistance,
            PASSIVE_NPC_CROWD_REPATH_LATERAL_DISTANCE);
    }

    private int CountNearbyPassiveNpcBlockers(
        Vector2 playerPos,
        Vector2 desiredDirection,
        float maxCenterDistance,
        float maxLateralDistance)
    {
        int count = 0;
        float maxCenterDistanceSqr = maxCenterDistance * maxCenterDistance;
        Vector2 forward = desiredDirection.sqrMagnitude > 0.0001f
            ? desiredDirection.normalized
            : Vector2.right;

        for (int i = 0; i < _nearbyNavigationAgents.Count; i++)
        {
            NavigationAgentSnapshot candidate = _nearbyNavigationAgents[i];
            if (candidate.UnitType != NavigationUnitType.NPC || candidate.IsCurrentlyMoving)
            {
                continue;
            }

            Vector2 toCandidate = candidate.Position - playerPos;
            if (toCandidate.sqrMagnitude > maxCenterDistanceSqr)
            {
                continue;
            }

            float forwardDistance = Vector2.Dot(toCandidate, forward);
            if (forwardDistance < -PASSIVE_NPC_CROWD_REPATH_BACKWARD_TOLERANCE)
            {
                continue;
            }

            Vector2 lateral = toCandidate - forward * forwardDistance;
            if (lateral.magnitude > maxLateralDistance)
            {
                continue;
            }

            count++;
        }

        return count;
    }

    private bool IsPassiveNpcWallFormation(
        Vector2 playerPos,
        Vector2 desiredDirection,
        NavigationAgentSnapshot primaryBlocker)
    {
        if (primaryBlocker.InstanceId == 0 ||
            primaryBlocker.UnitType != NavigationUnitType.NPC ||
            primaryBlocker.IsCurrentlyMoving)
        {
            return false;
        }

        int blockerCount = 0;
        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        float minY = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;
        for (int i = 0; i < _nearbyNavigationAgents.Count; i++)
        {
            NavigationAgentSnapshot candidate = _nearbyNavigationAgents[i];
            if (candidate.UnitType != NavigationUnitType.NPC ||
                candidate.IsCurrentlyMoving)
            {
                continue;
            }

            if (Vector2.Distance(candidate.Position, primaryBlocker.Position) >
                PASSIVE_NPC_WALL_LOOKAHEAD_DISTANCE)
            {
                continue;
            }

            blockerCount++;
            minX = Mathf.Min(minX, candidate.Position.x);
            maxX = Mathf.Max(maxX, candidate.Position.x);
            minY = Mathf.Min(minY, candidate.Position.y);
            maxY = Mathf.Max(maxY, candidate.Position.y);
        }

        if (blockerCount < PASSIVE_NPC_WALL_MIN_BLOCKERS)
        {
            return false;
        }

        return (maxX - minX) <= PASSIVE_NPC_WALL_MAX_FORWARD_SPAN &&
            (maxY - minY) <= PASSIVE_NPC_WALL_MAX_LATERAL_SPAN;
    }

    private bool ShouldCancelPassiveNpcWallPointNavigation(
        Vector2 playerPos,
        Vector2 desiredDirection,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance)
    {
        if (targetTransform != null ||
            !IsPassiveNpcBlocker(avoidance.BlockingAgentId, out NavigationAgentSnapshot blocker) ||
            !ShouldTreatPassiveNpcWallAsBlockedTarget(playerPos, blocker) ||
            !IsPassiveNpcWallFormation(playerPos, desiredDirection, blocker) ||
            _dynamicObstacleSightings < PASSIVE_NPC_WALL_CANCEL_MIN_SIGHTINGS)
        {
            return false;
        }

        float blockingDistance = Mathf.Min(avoidance.BlockingDistance, _debugLastBlockingDistance);
        if (blockingDistance > PASSIVE_NPC_WALL_CANCEL_BLOCKING_DISTANCE)
        {
            return false;
        }

        return avoidance.ShouldRepath ||
            _debugLastCloseConstraintHardBlocked ||
            (_debugLastCloseConstraintApplied &&
             _debugLastMoveScale <= PASSIVE_NPC_WALL_CANCEL_MAX_MOVE_SCALE);
    }

    private bool ShouldCancelPassiveNpcWallPointNavigationWithoutActiveBlocker(Vector2 playerPos)
    {
        if (!_hasDynamicDetour ||
            targetTransform != null ||
            !TryGetNearestPassiveNpcAheadTowardsDestination(
                playerPos,
                PASSIVE_NPC_WALL_LOOKAHEAD_DISTANCE,
                out NavigationAgentSnapshot blocker) ||
            !ShouldTreatPassiveNpcWallAsBlockedTarget(playerPos, blocker))
        {
            return false;
        }

        return Vector2.Distance(playerPos, blocker.Position) <=
            PASSIVE_NPC_WALL_CANCEL_BLOCKING_DISTANCE;
    }

    private bool TryGetNearbyNavigationAgentSnapshot(int instanceId, out NavigationAgentSnapshot snapshot)
    {
        for (int i = 0; i < _nearbyNavigationAgents.Count; i++)
        {
            NavigationAgentSnapshot candidate = _nearbyNavigationAgents[i];
            if (candidate.InstanceId == instanceId)
            {
                snapshot = candidate;
                return true;
            }
        }

        snapshot = default;
        return false;
    }

    private bool IsNpcBlocker(int blockingAgentId, out NavigationAgentSnapshot blocker)
    {
        if (TryGetNearbyNavigationAgentSnapshot(blockingAgentId, out blocker) &&
            blocker.UnitType == NavigationUnitType.NPC)
        {
            return true;
        }

        blocker = default;
        return false;
    }

    private bool IsPassiveNpcBlocker(int blockingAgentId, out NavigationAgentSnapshot blocker)
    {
        if (IsNpcBlocker(blockingAgentId, out blocker) &&
            !blocker.IsCurrentlyMoving)
        {
            return true;
        }

        blocker = default;
        return false;
    }

    private static NavigationLocalAvoidanceSolver.CloseRangeConstraintResult MaybeRelaxPassiveNpcCloseConstraint(
        NavigationLocalAvoidanceSolver.CloseRangeConstraintResult closeRangeConstraint,
        Vector2 desiredDirection,
        float desiredSpeedScale,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        bool hasPassiveNpcBlocker)
    {
        if (!hasPassiveNpcBlocker || !closeRangeConstraint.Applied)
        {
            return closeRangeConstraint;
        }

        if (avoidance.BlockingDistance > PASSIVE_NPC_BLOCKER_REPATH_CENTER_DISTANCE &&
            closeRangeConstraint.Clearance > PASSIVE_NPC_CLOSE_CONSTRAINT_SOFT_OVERLAP_CLEARANCE)
        {
            return new NavigationLocalAvoidanceSolver.CloseRangeConstraintResult(
                desiredDirection,
                desiredSpeedScale,
                false,
                false,
                closeRangeConstraint.Clearance,
                closeRangeConstraint.ForwardIntoBlocker);
        }

        if (closeRangeConstraint.HardBlocked)
        {
            float hardStopSoftenClearance =
                avoidance.ShouldRepath &&
                avoidance.BlockingDistance > PASSIVE_NPC_SINGLE_STOP_JITTER_REPATH_DISTANCE
                    ? PASSIVE_NPC_SINGLE_STOP_JITTER_HARDSTOP_SOFTEN_CLEARANCE
                    : PASSIVE_NPC_CLOSE_CONSTRAINT_SOFT_OVERLAP_CLEARANCE;

            if (closeRangeConstraint.Clearance <= hardStopSoftenClearance)
            {
                return closeRangeConstraint;
            }

            Vector2 softenedDirection = desiredDirection.sqrMagnitude > 0.0001f
                ? desiredDirection.normalized
                : closeRangeConstraint.ConstrainedDirection;
            if (closeRangeConstraint.ConstrainedDirection.sqrMagnitude > 0.0001f &&
                desiredDirection.sqrMagnitude > 0.0001f)
            {
                softenedDirection = Vector2.Lerp(
                    desiredDirection.normalized,
                    closeRangeConstraint.ConstrainedDirection.normalized,
                    PASSIVE_NPC_CLOSE_CONSTRAINT_SOFT_OVERLAP_BLEND).normalized;
            }

            return new NavigationLocalAvoidanceSolver.CloseRangeConstraintResult(
                softenedDirection,
                Mathf.Max(closeRangeConstraint.SpeedScale, PASSIVE_NPC_CLOSE_CONSTRAINT_SOFT_OVERLAP_SPEED),
                true,
                false,
                closeRangeConstraint.Clearance,
                closeRangeConstraint.ForwardIntoBlocker);
        }

        if (closeRangeConstraint.Clearance < 0f &&
            closeRangeConstraint.Clearance > PASSIVE_NPC_CLOSE_CONSTRAINT_SOFT_OVERLAP_CLEARANCE)
        {
            Vector2 softenedDirection = desiredDirection.sqrMagnitude > 0.0001f
                ? desiredDirection.normalized
                : closeRangeConstraint.ConstrainedDirection;
            if (closeRangeConstraint.ConstrainedDirection.sqrMagnitude > 0.0001f &&
                desiredDirection.sqrMagnitude > 0.0001f)
            {
                softenedDirection = Vector2.Lerp(
                    desiredDirection.normalized,
                    closeRangeConstraint.ConstrainedDirection.normalized,
                    PASSIVE_NPC_CLOSE_CONSTRAINT_SOFT_OVERLAP_BLEND).normalized;
            }

            return new NavigationLocalAvoidanceSolver.CloseRangeConstraintResult(
                softenedDirection,
                Mathf.Max(closeRangeConstraint.SpeedScale, PASSIVE_NPC_CLOSE_CONSTRAINT_SOFT_OVERLAP_SPEED),
                true,
                false,
                closeRangeConstraint.Clearance,
                closeRangeConstraint.ForwardIntoBlocker);
        }

        if (closeRangeConstraint.Clearance <= PASSIVE_NPC_CLOSE_CONSTRAINT_RELAX_CLEARANCE ||
            closeRangeConstraint.ForwardIntoBlocker >= PASSIVE_NPC_CLOSE_CONSTRAINT_MIN_FORWARD)
        {
            return closeRangeConstraint;
        }

        return new NavigationLocalAvoidanceSolver.CloseRangeConstraintResult(
            desiredDirection,
            desiredSpeedScale,
            false,
            false,
            closeRangeConstraint.Clearance,
            closeRangeConstraint.ForwardIntoBlocker);
    }

    private static float GetPassiveNpcCloseConstraintMoveFloor(
        NavigationLocalAvoidanceSolver.CloseRangeConstraintResult closeRangeConstraint)
    {
        float recoveryFactor = 1f - Mathf.Clamp01(
            closeRangeConstraint.ForwardIntoBlocker /
            Mathf.Max(PASSIVE_NPC_CLOSE_CONSTRAINT_MIN_FORWARD, 0.001f));

        return Mathf.Lerp(PASSIVE_NPC_CLOSE_CONSTRAINT_SOFT_OVERLAP_SPEED, PASSIVE_NPC_CLOSE_CONSTRAINT_RECOVERY_SPEED, recoveryFactor);
    }

    private bool TryCreateDynamicDetour(Vector2 playerPos, NavigationLocalAvoidanceSolver.AvoidanceResult avoidance)
    {
        if (navGrid == null)
        {
            return false;
        }

        if (IsPassiveNpcBlocker(avoidance.BlockingAgentId, out NavigationAgentSnapshot blocker) &&
            (ShouldTreatPassiveNpcWallAsBlockedTarget(playerPos, blocker) ||
             ShouldSuppressPassiveNpcArrivalRepath(playerPos, blocker)))
        {
            return false;
        }

        Vector2 separationDirection = playerPos - avoidance.BlockingAgentPosition;
        if (separationDirection.sqrMagnitude < 0.0001f)
        {
            separationDirection = -avoidance.AdjustedDirection;
        }

        if (separationDirection.sqrMagnitude < 0.0001f)
        {
            separationDirection = Vector2.left;
        }

        separationDirection.Normalize();
        Vector2 sidestepDirection = avoidance.SuggestedDetourDirection.sqrMagnitude > 0.0001f
            ? avoidance.SuggestedDetourDirection.normalized
            : Vector2.Perpendicular(separationDirection).normalized;

        float contactShellDistance =
            GetColliderRadius() +
            Mathf.Max(avoidance.BlockingAgentRadius, 0.35f) +
            GetContactShellPadding() +
            0.12f;

        NavigationPathExecutor2D.DetourLifecycleResult sharedDetour = NavigationPathExecutor2D.TryCreateDetour(
            navigationExecution,
            navGrid,
            playerPos,
            avoidance.BlockingAgentId,
            avoidance.BlockingAgentPosition,
            avoidance.AdjustedDirection,
            avoidance.SuggestedDetourDirection,
            contactShellDistance,
            0.55f,
            SharedDynamicDetourCandidateSpecs,
            Time.time);

        if (sharedDetour.Created || sharedDetour.ShouldKeepCurrentDetour)
        {
            return true;
        }

        float minimumClearanceSqr = contactShellDistance * contactShellDistance * 0.55f;

        Vector2 destinationDirection = GetResolvedPathDestination() - playerPos;
        if (destinationDirection.sqrMagnitude > 0.0001f &&
            CountNearbyPassiveNpcBlockers(
                playerPos,
                destinationDirection,
                PASSIVE_NPC_CROWD_DETOUR_KEEP_CENTER_DISTANCE,
                PASSIVE_NPC_CROWD_DETOUR_KEEP_LATERAL_DISTANCE) >=
            PASSIVE_NPC_CROWD_REPATH_MIN_BLOCKERS)
        {
            Vector2 forwardDirection = destinationDirection.normalized;
            if (TrySetDynamicDetourCandidate(
                    avoidance.BlockingAgentPosition +
                    sidestepDirection * (contactShellDistance * PASSIVE_NPC_CROWD_FORWARD_DETOUR_BLOCKER_SIDESTEP_SCALE) +
                    forwardDirection * (contactShellDistance * PASSIVE_NPC_CROWD_FORWARD_DETOUR_BLOCKER_FORWARD_SCALE),
                    avoidance,
                    minimumClearanceSqr))
            {
                return true;
            }

            if (TrySetDynamicDetourCandidate(
                    playerPos +
                    sidestepDirection * (contactShellDistance * PASSIVE_NPC_CROWD_FORWARD_DETOUR_PLAYER_SIDESTEP_SCALE) +
                    forwardDirection * (contactShellDistance * PASSIVE_NPC_CROWD_FORWARD_DETOUR_PLAYER_FORWARD_SCALE),
                    avoidance,
                    minimumClearanceSqr))
            {
                return true;
            }
        }

        if (TrySetDynamicDetourCandidate(
                playerPos + separationDirection * (contactShellDistance * 0.9f) + sidestepDirection * (contactShellDistance * 0.9f),
                avoidance,
                minimumClearanceSqr))
        {
            return true;
        }

        if (TrySetDynamicDetourCandidate(
                avoidance.BlockingAgentPosition + sidestepDirection * (contactShellDistance * 1.15f) + separationDirection * (contactShellDistance * 0.65f),
                avoidance,
                minimumClearanceSqr))
        {
            return true;
        }

        if (TrySetDynamicDetourCandidate(
                playerPos + sidestepDirection * (contactShellDistance * 1.1f),
                avoidance,
                minimumClearanceSqr))
        {
            return true;
        }

        return TrySetDynamicDetourCandidate(
            playerPos + separationDirection * contactShellDistance,
            avoidance,
            minimumClearanceSqr);
    }

    private bool TrySetDynamicDetourCandidate(
        Vector2 candidate,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        float minimumClearanceSqr)
    {
        if (navGrid == null || !navGrid.TryFindNearestWalkable(candidate, out Vector2 detourPoint))
        {
            return false;
        }

        if ((detourPoint - avoidance.BlockingAgentPosition).sqrMagnitude < minimumClearanceSqr)
        {
            return false;
        }

        NavigationPathExecutor2D.SetOverrideWaypoint(navigationExecution, detourPoint, avoidance.BlockingAgentId);
        return true;
    }

    private bool TryReleaseDynamicDetour(Vector2 playerPos, float currentTime)
    {
        if (!_hasDynamicDetour)
        {
            return false;
        }

        if (ShouldKeepPassiveNpcCrowdDetour(playerPos))
        {
            return false;
        }

        NavigationPathExecutor2D.DetourLifecycleResult detour = NavigationPathExecutor2D.TryClearDetourAndRecover(
            navigationExecution,
            navGrid,
            playerPos,
            GetResolvedPathDestination(),
            0,
            currentTime,
            rebuildPath: false,
            minimumDetourActiveDuration: DYNAMIC_DETOUR_MIN_ACTIVE_DURATION,
            recoveryCooldown: 0f,
            hasLineOfSight: null,
            cleanupReferencePosition: null,
            cleanupWaypointTolerance: 0f,
            log: AddDebugLog,
            ignoredCollider: playerCollider);

        if (detour.ShouldKeepCurrentDetour || detour.HasActiveDetour)
        {
            return false;
        }

        if (detour.Cleared || detour.Recovered)
        {
            AddDebugLog("共享动态代理短暂消失，释放 detour owner 并恢复主路径");
            return true;
        }

        return false;
    }

    private bool IsDynamicDetourProtected(float currentTime)
    {
        return _hasDynamicDetour &&
            navigationExecution.LastDetourCreateTime > float.NegativeInfinity &&
            currentTime - navigationExecution.LastDetourCreateTime < DYNAMIC_DETOUR_MIN_ACTIVE_DURATION;
    }

    private float GetRequestedMoveSpeed()
    {
        if (movement == null)
        {
            return 0f;
        }

        return runWhileNavigating ? movement.RunSpeed : movement.WalkSpeed;
    }

    private bool ShouldUseBlockedNavigationInput(
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        NavigationLocalAvoidanceSolver.CloseRangeConstraintResult closeRangeConstraint,
        float moveScale)
    {
        if (!avoidance.HasBlockingAgent)
        {
            return false;
        }

        if (!_hasDynamicDetour &&
            IsPassiveNpcBlocker(avoidance.BlockingAgentId, out _))
        {
            if (!closeRangeConstraint.HardBlocked &&
                closeRangeConstraint.Applied &&
                closeRangeConstraint.Clearance > PASSIVE_NPC_CLOSE_CONSTRAINT_SOFT_OVERLAP_CLEARANCE &&
                closeRangeConstraint.Clearance < 0f)
            {
                return false;
            }

            if (closeRangeConstraint.HardBlocked || closeRangeConstraint.Clearance <= 0f)
            {
                if (!closeRangeConstraint.HardBlocked &&
                    avoidance.BlockingDistance > PASSIVE_NPC_BLOCKER_REPATH_CENTER_DISTANCE &&
                    closeRangeConstraint.Clearance > PASSIVE_NPC_CLOSE_CONSTRAINT_SOFT_OVERLAP_CLEARANCE)
                {
                    return false;
                }

                return closeRangeConstraint.Applied || avoidance.ShouldRepath;
            }

            return closeRangeConstraint.Applied &&
                avoidance.BlockingDistance <= PASSIVE_NPC_BLOCKER_REPATH_CENTER_DISTANCE &&
                closeRangeConstraint.Clearance <= PASSIVE_NPC_BLOCKED_INPUT_CLEARANCE &&
                closeRangeConstraint.ForwardIntoBlocker >= PASSIVE_NPC_BLOCKED_INPUT_FORWARD;
        }

        if (closeRangeConstraint.Applied || avoidance.ShouldRepath)
        {
            return true;
        }

        float engageDistance =
            GetColliderRadius() +
            Mathf.Max(avoidance.BlockingAgentRadius, 0.01f) +
            GetContactShellPadding() +
            0.08f;

        return moveScale <= 0.35f && avoidance.BlockingDistance <= engageDistance;
    }

    private void ForceImmediateMovementStop()
    {
        if (movement != null)
        {
            movement.SetMovementInput(Vector2.zero, false);
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
        }
    }

    private void MaybeLogSharedAvoidance(
        Vector2 playerPos,
        Vector2 waypoint,
        NavigationLocalAvoidanceSolver.AvoidanceResult avoidance,
        NavigationLocalAvoidanceSolver.CloseRangeConstraintResult closeRangeConstraint,
        float moveScale,
        string action)
    {
        if (!enableDetailedDebug || !avoidance.HasBlockingAgent)
        {
            return;
        }

        if (Time.frameCount - _lastSharedAvoidanceDebugFrame < 8)
        {
            return;
        }

        _lastSharedAvoidanceDebugFrame = Time.frameCount;
        Debug.Log(
            $"<color=orange>[NavAvoid]</color> action={action}, agent={avoidance.BlockingAgentId}, " +
            $"playerPos={playerPos}, waypoint={waypoint}, blockerPos={avoidance.BlockingAgentPosition}, " +
            $"blockingDistance={avoidance.BlockingDistance:F2}, moveScale={moveScale:F2}, " +
            $"shouldRepath={avoidance.ShouldRepath}, detour={_hasDynamicDetour}, " +
            $"closeConstraint={closeRangeConstraint.Applied}, hardBlocked={closeRangeConstraint.HardBlocked}, " +
            $"clearance={closeRangeConstraint.Clearance:F2}, forwardInto={closeRangeConstraint.ForwardIntoBlocker:F2}, " +
            $"rbVelocity={(playerRigidbody != null ? playerRigidbody.linearVelocity : Vector2.zero)}",
            this);
    }

    private int ProbeObstacleHits(Vector2 center, float radius)
    {
        if (_obstacleProbeBuffer == null || _obstacleProbeBuffer.Length != Mathf.Max(8, obstacleProbeBufferSize))
        {
            _obstacleProbeBuffer = new Collider2D[Mathf.Max(8, obstacleProbeBufferSize)];
        }

        return Physics2D.OverlapCircle(center, radius, _obstacleProbeFilter, _obstacleProbeBuffer);
    }

    private bool TryGetNavigationUnit(Collider2D col, out INavigationUnit navigationUnit)
    {
        navigationUnit = null;
        if (col == null)
        {
            return false;
        }

        NavigationUnitBase unitBase = col.GetComponentInParent<NavigationUnitBase>();
        if (unitBase != null)
        {
            navigationUnit = unitBase;
            return true;
        }

        PlayerAutoNavigator playerNavigator = col.GetComponentInParent<PlayerAutoNavigator>();
        if (playerNavigator != null)
        {
            navigationUnit = playerNavigator;
            return true;
        }

        NPCAutoRoamController npcNavigator = col.GetComponentInParent<NPCAutoRoamController>();
        if (npcNavigator != null)
        {
            navigationUnit = npcNavigator;
            return true;
        }

        return false;
    }

    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad) * v.x - Mathf.Sin(rad) * v.y, Mathf.Sin(rad) * v.x + Mathf.Cos(rad) * v.y);
    }

    private Vector2 GetPlayerPosition()
    {
        // 普通地面点导航与交互跟随都统一以玩家真实占位中心作为导航位置真值；
        // 两者的区别只在终点契约：
        // 1. 点导航 = 占位中心对齐点击点
        // 2. 跟随目标 = 占位中心接近 ClosestPoint + stopRadius
        if (playerCollider != null && playerCollider.enabled && playerCollider.gameObject.activeInHierarchy)
        {
            return playerCollider.bounds.center;
        }

        if (playerRigidbody != null)
        {
            return playerRigidbody.position;
        }

        return player != null ? (Vector2)player.position : Vector2.zero;
    }

    private float GetContactShellPadding()
    {
        return Mathf.Min(dynamicObstaclePadding, 0.05f);
    }

    private float GetFinalStopDistance()
    {
        return targetTransform != null ? followStopRadius : stopDistance;
    }

    private Vector2 GetPathRequestDestination()
    {
        if (targetTransform != null)
        {
            return targetPoint;
        }

        // 普通地面点导航直接以玩家实际占位中心对齐点击点；
        // 不再混用“脚底 Transform/刚体位置 + 偏移换算”语义。
        return targetPoint;
    }

    private bool HasReachedArrivalPoint(Vector2 playerPos)
    {
        if (targetTransform != null)
        {
            return IsCloseEnoughToInteract(playerPos);
        }

        Vector2 resolvedDestination = GetResolvedPathDestination();
        return Vector2.Distance(playerPos, resolvedDestination) <= GetFinalStopDistance();
    }

    private Vector2 GetResolvedPathDestination()
    {
        return NavigationPathExecutor2D.GetResolvedDestination(navigationExecution, targetPoint);
    }

    private float GetPointArrivalBlockerRadiusUpperBound()
    {
        return GetColliderRadius() + PASSIVE_NPC_CLOSE_CONSTRAINT_EXTRA_RADIUS;
    }

    private float GetPointArrivalBlockerOccupyDistance(NavigationAgentSnapshot blocker)
    {
        float candidateOccupyRadius =
            Mathf.Min(
                Mathf.Max(blocker.ColliderRadius, 0.01f),
                GetPointArrivalBlockerRadiusUpperBound());
        return
            GetColliderRadius() +
            candidateOccupyRadius +
            GetContactShellPadding() +
            POINT_ARRIVAL_BLOCKER_RADIUS_PADDING;
    }

    private bool IsPointArrivalPassiveNpcBlocker(
        NavigationAgentSnapshot blocker,
        Vector2 resolvedDestination)
    {
        return targetTransform == null &&
            blocker.InstanceId != 0 &&
            blocker.UnitType == NavigationUnitType.NPC &&
            !blocker.IsCurrentlyMoving &&
            Vector2.Distance(blocker.Position, resolvedDestination) <= GetPointArrivalBlockerOccupyDistance(blocker);
    }

    private bool IsWithinPointArrivalBlockerSettleWindow(
        Vector2 playerPos,
        NavigationAgentSnapshot blocker,
        Vector2 resolvedDestination)
    {
        if (!IsPointArrivalPassiveNpcBlocker(blocker, resolvedDestination))
        {
            return false;
        }

        float settleDistance =
            GetPointArrivalBlockerOccupyDistance(blocker) +
            GetFinalStopDistance() +
            POINT_ARRIVAL_BLOCKER_SETTLE_PADDING;
        return Vector2.Distance(playerPos, resolvedDestination) <= settleDistance;
    }

    private bool ShouldSuppressPassiveNpcArrivalRepath(
        Vector2 playerPos,
        NavigationAgentSnapshot blocker)
    {
        Vector2 resolvedDestination = GetResolvedPathDestination();
        if (!IsPointArrivalPassiveNpcBlocker(blocker, resolvedDestination))
        {
            return false;
        }

        float repathSuppressDistance =
            GetPointArrivalBlockerOccupyDistance(blocker) +
            GetFinalStopDistance() +
            POINT_ARRIVAL_BLOCKER_REPATH_SUPPRESS_PADDING;
        return Vector2.Distance(playerPos, resolvedDestination) <= repathSuppressDistance;
    }

    private bool ShouldTreatPassiveNpcWallAsBlockedTarget(
        Vector2 playerPos,
        NavigationAgentSnapshot blocker)
    {
        if (targetTransform != null ||
            blocker.InstanceId == 0)
        {
            return false;
        }

        Vector2 resolvedDestination = GetResolvedPathDestination();
        Vector2 destinationDirection = resolvedDestination - playerPos;
        if (destinationDirection.sqrMagnitude < 0.0001f)
        {
            return false;
        }

        if (!IsPassiveNpcWallFormation(playerPos, destinationDirection, blocker))
        {
            return false;
        }

        Vector2 forward = destinationDirection.normalized;
        Vector2 lateralAxis = new Vector2(-forward.y, forward.x);
        float blockerForwardDistance = Vector2.Dot(blocker.Position - playerPos, forward);
        float destinationForwardDistance = Vector2.Dot(destinationDirection, forward);
        float destinationLateralDistance = Mathf.Abs(Vector2.Dot(destinationDirection, lateralAxis));

        return blockerForwardDistance > -PASSIVE_NPC_CROWD_REPATH_BACKWARD_TOLERANCE &&
            destinationForwardDistance > blockerForwardDistance &&
            destinationLateralDistance <= PASSIVE_NPC_WALL_TARGET_MAX_LATERAL_DISTANCE;
    }

    private bool TryGetNearestPassiveNpcAheadTowardsDestination(
        Vector2 playerPos,
        float maxCenterDistance,
        out NavigationAgentSnapshot blocker)
    {
        return TryGetNearestPassiveNpcAhead(
            playerPos,
            GetResolvedPathDestination() - playerPos,
            maxCenterDistance,
            out blocker);
    }

    private bool TryGetNearestPassiveNpcAhead(
        Vector2 playerPos,
        Vector2 forwardDirection,
        float maxCenterDistance,
        out NavigationAgentSnapshot blocker)
    {
        blocker = default;
        if (forwardDirection.sqrMagnitude < 0.0001f)
        {
            return false;
        }

        Vector2 forward = forwardDirection.normalized;
        float bestForwardDistance = float.PositiveInfinity;
        float maxCenterDistanceSqr = maxCenterDistance * maxCenterDistance;
        for (int i = 0; i < _nearbyNavigationAgents.Count; i++)
        {
            NavigationAgentSnapshot candidate = _nearbyNavigationAgents[i];
            if (candidate.UnitType != NavigationUnitType.NPC ||
                candidate.IsCurrentlyMoving)
            {
                continue;
            }

            Vector2 toCandidate = candidate.Position - playerPos;
            if (toCandidate.sqrMagnitude > maxCenterDistanceSqr)
            {
                continue;
            }

            float forwardDistance = Vector2.Dot(toCandidate, forward);
            if (forwardDistance < -PASSIVE_NPC_CROWD_REPATH_BACKWARD_TOLERANCE ||
                forwardDistance >= bestForwardDistance)
            {
                continue;
            }

            blocker = candidate;
            bestForwardDistance = forwardDistance;
        }

        return blocker.InstanceId != 0;
    }

    private bool ShouldKeepPassiveNpcCrowdDetour(Vector2 playerPos)
    {
        if (!_hasDynamicDetour ||
            targetTransform != null)
        {
            return false;
        }

        Vector2 resolvedDestination = GetResolvedPathDestination();
        if (Vector2.Distance(playerPos, resolvedDestination) <=
            GetFinalStopDistance() + PASSIVE_NPC_CROWD_DETOUR_KEEP_DESTINATION_DISTANCE)
        {
            return false;
        }

        return CountNearbyPassiveNpcBlockers(
                playerPos,
                resolvedDestination - playerPos,
                PASSIVE_NPC_CROWD_DETOUR_KEEP_CENTER_DISTANCE,
                PASSIVE_NPC_CROWD_DETOUR_KEEP_LATERAL_DISTANCE) >=
            PASSIVE_NPC_CROWD_REPATH_MIN_BLOCKERS;
    }

    private void UpdateSprintState()
    {
        if (SprintStateManager.Instance != null)
        {
            SprintStateManager.Instance.OnMovementInput(true);
            runWhileNavigating = SprintStateManager.Instance.ShouldNavigationSprint();
        }
    }

    private bool IsPlayerCollider(Collider2D col)
    {
        return col == playerCollider || col.transform == player || col.transform.IsChildOf(player);
    }

    /// <summary>
    /// 判断碰撞体是否是障碍物
    /// 🔥 策略：宁可误杀，不可放过
    /// 默认所有非 Trigger 的碰撞体都是障碍物，除非明确排除
    /// </summary>
    private bool IsObstacle(Collider2D col)
    {
        // 🔥 Trigger 通常不阻挡视线（如拾取物、触发区域）
        if (col.isTrigger) return false;
        
        // 排除掉落物和临时物体
        if (col.name.Contains("(Clone)") || col.name.Contains("Pickup")) return false;
        
        // 🔥 新策略：默认所有非 Trigger 的碰撞体都是障碍物
        // 除非它在排除列表中
        
        // 如果配置了 LayerMask，检查是否在 Mask 中
        if (losObstacleMask.value != 0)
        {
            bool inMask = ((1 << col.gameObject.layer) & losObstacleMask.value) != 0;
            if (inMask) return true; // 在 Mask 中，是障碍物
        }
        
        // 如果配置了 Tags，检查是否有匹配的 Tag
        if (losObstacleTags != null && losObstacleTags.Length > 0)
        {
            bool hasTag = HasAnyTag(col.transform, losObstacleTags);
            if (hasTag) return true; // 有匹配 Tag，是障碍物
        }
        
        // 🔥 关键：如果既没有配置 Mask 也没有配置 Tags，
        // 或者物体不在配置的 Mask/Tags 中，
        // 默认也视为障碍物（宁可误杀，不可放过）
        return true;
    }

    private static bool HasAnyTag(Transform t, string[] tags)
    {
        if (t == null || tags == null) return false;
        Transform current = t;
        while (current != null)
        {
            foreach (var tag in tags)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    try { if (current.CompareTag(tag.Trim())) return true; } catch { }
                }
            }
            current = current.parent;
        }
        return false;
    }

    #region 🔥 智能目标点计算（v4.0 ClosestPoint 统一版）
    
    // 缓存目标 Collider，避免每帧 GetComponent
    private Collider2D _cachedTargetCollider;
    private Transform _cachedTargetTransform;
    
    /// <summary>
    /// 🔥 v4.0：获取目标最近点（使用 ClosestPoint）
    /// 
    /// 核心思路：
    /// 1. 使用 Collider.ClosestPoint(playerPos) 计算玩家到目标的最近点
    /// 2. 这样从任何方向接近都是最短路径，不会绕路
    /// 3. 与 GameInputManager 使用相同的距离计算方式
    /// </summary>
    private Vector2 GetTargetAnchorPoint(Transform target)
    {
        if (target == null) return transform.position;

        var chest = target.GetComponent<FarmGame.World.ChestController>() ?? target.GetComponentInParent<FarmGame.World.ChestController>();
        if (chest != null)
        {
            return chest.GetClosestInteractionPointForNavigation(GetPlayerPosition());
        }
        
        // 缓存 Collider
        if (_cachedTargetTransform != target)
        {
            _cachedTargetTransform = target;
            _cachedTargetCollider = target.GetComponent<Collider2D>();
            if (_cachedTargetCollider == null)
                _cachedTargetCollider = target.GetComponentInChildren<Collider2D>();
        }
        
        if (_cachedTargetCollider != null)
        {
            // 🔥 v4.0：使用 ClosestPoint 计算玩家到 Collider 的最近点
            Vector2 playerPos = GetPlayerPosition();
            return _cachedTargetCollider.ClosestPoint(playerPos);
        }
        
        return target.position;
    }
    
    /// <summary>
    /// 🔥 v4.0：计算玩家到目标的最近接近点
    /// 
    /// 核心思路：
    /// 1. 使用 ClosestPoint 计算玩家到 Collider 的最近点
    /// 2. 这样导航的目标与交互判断的目标一致
    /// </summary>
    private Vector3 CalculateClosestApproachPoint(Transform target)
    {
        return GetTargetAnchorPoint(target);
    }
    
    /// <summary>
    /// 根据交互距离计算最优停止半径
    /// 
    /// 核心思路：
    /// 1. 停止半径应该小于交互距离，确保到达后能触发交互
    /// 2. GameInputManager 允许 20% 容差，所以停止半径 = interactDist * 0.9
    /// 3. 这样玩家停下后，距离 < interactDist * 1.2，一定能交互
    /// </summary>
    private float CalculateOptimalStopRadius(Transform target, float defaultRadius)
    {
        // 尝试获取 IInteractable 的交互距离
        var interactable = target.GetComponent<IInteractable>();
        if (interactable != null)
        {
            float interactDist = interactable.InteractionDistance;
            float stopFactor = interactable is FarmGame.World.ChestController ? 0.72f : 0.78f;
            float minStopRadius = interactable is FarmGame.World.ChestController ? 0.45f : 0.3f;
            // 对交互目标保留更明显的“到位余量”，避免角色停得过早导致 UI 不开。
            return Mathf.Max(minStopRadius, interactDist * stopFactor);
        }
        
        // 没有 IInteractable，使用默认值
        return Mathf.Clamp(defaultRadius, 0.3f, 1.5f);
    }
    
    /// <summary>
    /// 🔥 v4.0：检查玩家是否足够近可以与目标交互
    /// 
    /// 核心思路：
    /// 1. 使用与 GameInputManager 相同的距离计算方式
    /// 2. 玩家中心 → Collider 最近点（ClosestPoint）
    /// </summary>
    private bool IsCloseEnoughToInteract(Vector2 playerPos)
    {
        Vector2 targetAnchor = GetTargetAnchorPoint(targetTransform);
        float distance = Vector2.Distance(playerPos, targetAnchor);
        return distance <= followStopRadius;
    }
    
    #endregion

    #region 调试输出

    private void AddDebugLog(string message)
    {
        if (!enableDetailedDebug)
        {
            return;
        }

        debugLogs.Add($"[{Time.frameCount}] {message}");
    }

    private bool ShouldSkipSameFramePathRebuild(Vector2 startPosition, Vector2 requestedDestination)
    {
        if (!_hasLastBuildSignature || _lastBuildFrame != Time.frameCount)
        {
            return false;
        }

        if (_lastBuildTargetTransform != targetTransform)
        {
            return false;
        }

        if (Vector2.Distance(_lastBuildStartPosition, startPosition) > SAME_FRAME_PATH_REBUILD_START_THRESHOLD)
        {
            return false;
        }

        if (Vector2.Distance(_lastBuildRequestedDestination, requestedDestination) > SAME_FRAME_PATH_REBUILD_DESTINATION_THRESHOLD)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 输出完整的调试信息
    /// </summary>
    private void LogFullDebugInfo(string reason)
    {
        if (!enableDetailedDebug) return;
        
        Vector2 playerPos = GetPlayerPosition();
        
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"<color=red>═══════════════════════════════════════</color>");
        sb.AppendLine($"<color=red>[Nav] 导航诊断报告：{reason}</color>");
        sb.AppendLine($"<color=red>═══════════════════════════════════════</color>");
        
        // 基本信息
        sb.AppendLine($"<color=yellow>【基本信息】</color>");
        sb.AppendLine($"  玩家位置: {playerPos}");
        sb.AppendLine($"  目标位置: {(Vector2)targetPoint}");
        sb.AppendLine($"  距离目标: {Vector2.Distance(playerPos, targetPoint):F2}m");
        sb.AppendLine($"  玩家半径: {playerRadius:F2}m");
        sb.AppendLine($"  是否跑步: {runWhileNavigating}");
        
        // 路径信息
        sb.AppendLine($"<color=yellow>【路径信息】</color>");
        sb.AppendLine($"  路径点数: {path.Count}");
        sb.AppendLine($"  当前索引: {pathIndex}");
        if (path.Count > 0)
        {
            sb.AppendLine($"  路径点列表:");
            for (int i = 0; i < Mathf.Min(path.Count, 10); i++)
            {
                string marker = i == pathIndex ? " ← 当前" : "";
                sb.AppendLine($"    [{i}] {path[i]}{marker}");
            }
            if (path.Count > 10)
            {
                sb.AppendLine($"    ... 还有 {path.Count - 10} 个点");
            }
        }
        
        // 网格信息
        if (navGrid != null)
        {
            sb.AppendLine($"<color=yellow>【网格信息】</color>");
            sb.AppendLine($"  起点可走: {navGrid.IsWalkable(playerPos)}");
            sb.AppendLine($"  终点可走: {navGrid.IsWalkable(targetPoint)}");
        }
        
        // 周边障碍物
        sb.AppendLine($"<color=yellow>【周边障碍物】</color>");
        var nearbyObstacles = Physics2D.OverlapCircleAll(playerPos, 2f);
        int obstacleCount = 0;
        foreach (var col in nearbyObstacles)
        {
            if (IsPlayerCollider(col)) continue;
            if (IsObstacle(col))
            {
                obstacleCount++;
                Vector2 obstaclePos = col.transform.position;
                float dist = Vector2.Distance(playerPos, obstaclePos);
                Vector2 closest = col.ClosestPoint(playerPos);
                float closestDist = Vector2.Distance(playerPos, closest);
                
                if (obstacleCount <= 8)
                {
                    sb.AppendLine($"  {obstacleCount}. {col.name}");
                    sb.AppendLine($"     位置: {obstaclePos}, 距离: {dist:F2}m, 最近点距离: {closestDist:F2}m");
                }
            }
        }
        if (obstacleCount > 8)
        {
            sb.AppendLine($"  ... 还有 {obstacleCount - 8} 个障碍物");
        }
        if (obstacleCount == 0)
        {
            sb.AppendLine($"  无障碍物");
        }
        
        // 调试日志历史
        if (debugLogs.Count > 0)
        {
            sb.AppendLine($"<color=yellow>【调试日志】</color>");
            foreach (var log in debugLogs)
            {
                sb.AppendLine($"  {log}");
            }
        }
        
        // 卡顿信息
        sb.AppendLine($"<color=yellow>【卡顿信息】</color>");
        sb.AppendLine($"  卡顿次数: {stuckRetryCount}/{MAX_STUCK_RETRIES}");
        sb.AppendLine($"  上次检测位置: {lastCheckPosition}");
        sb.AppendLine($"  当前位置: {playerPos}");
        sb.AppendLine($"  位移: {Vector2.Distance(lastCheckPosition, playerPos):F3}m");
        
        sb.AppendLine($"<color=red>═══════════════════════════════════════</color>");
        
        Debug.LogWarning(sb.ToString());
    }

    #endregion

    void OnDrawGizmos()
    {
        if (!showPathGizmos || !active || path == null || path.Count == 0) return;

        // 绘制路径线
        Gizmos.color = Color.cyan;
        for (int i = 0; i < path.Count - 1; i++) 
            Gizmos.DrawLine(path[i], path[i + 1]);

        // 绘制路径点
        for (int i = 0; i < path.Count; i++)
        {
            if (i < pathIndex)
                Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            else if (i == pathIndex)
                Gizmos.color = Color.yellow;
            else
                Gizmos.color = Color.cyan;
            
            Gizmos.DrawSphere(path[i], 0.08f);
        }

        // 绘制目标点
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPoint, 0.25f);

        // 绘制当前移动方向
        if (playerCollider != null && pathIndex < path.Count)
        {
            Vector3 playerPos = playerCollider.bounds.center;
            
            // 到路径点的方向
            Gizmos.color = Color.green;
            Gizmos.DrawLine(playerPos, path[pathIndex]);
        }
        
        // 🔥 可视化调试：绘制视线检测结果
        if (enableLineOfSightOptimization && playerCollider != null && pathIndex < path.Count)
        {
            Vector2 playerPos = playerCollider.bounds.center;
            
            // 绘制从玩家到当前路径点的视线
            if (_lastLOSBlocked && _lastLOSHit.collider != null)
            {
                // 被阻挡：红线 + 红球
                Gizmos.color = Color.red;
                Gizmos.DrawLine(playerPos, _lastLOSHit.point);
                Gizmos.DrawWireSphere(_lastLOSHit.point, 0.15f);
                Gizmos.DrawSphere(_lastLOSHit.point, 0.08f);
            }
            else
            {
                // 通畅：绿线
                Gizmos.color = Color.green;
                for (int i = pathIndex; i < path.Count; i++)
                {
                    Vector2 checkFrom = (i == pathIndex) ? playerPos : path[i - 1];
                    Vector2 checkTo = path[i];
                    
                    // 简单检查（不调用 HasLineOfSight 避免递归）
                    Gizmos.DrawLine(checkFrom, checkTo);
                }
            }
        }
    }
}
