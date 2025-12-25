using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自动导航（右键点击）- v5.2 终极优化版
/// 
/// 核心改进：
/// 1. 斜向移动时固定朝向为左/右，避免摇头
/// 2. 路径平滑处理，减少崎岖路线
/// 3. 详细的卡顿诊断输出
/// 4. 视线优化跳过中间路径点
/// </summary>
public class PlayerAutoNavigator : MonoBehaviour
{
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

    // 私有字段
    private Collider2D playerCollider;
    private float playerRadius;
    private bool active;
    private Vector3 targetPoint;
    private Transform targetTransform;
    private float followStopRadius = 0.6f;
    private bool runWhileNavigating;
    private readonly List<Vector2> path = new List<Vector2>();
    private int pathIndex;

    // 卡顿检测
    private Vector2 lastCheckPosition;
    private float lastCheckTime;
    private int stuckRetryCount;
    private const float STUCK_THRESHOLD = 0.1f;
    private const float STUCK_CHECK_INTERVAL = 0.3f;
    private const int MAX_STUCK_RETRIES = 3;

    // 调试信息
    private List<string> debugLogs = new List<string>();

    public bool IsActive => active;

    void Awake()
    {
        if (player == null) player = transform;
        if (movement == null) movement = GetComponent<PlayerMovement>();
        if (navGrid == null) navGrid = FindFirstObjectByType<NavGrid2D>();
        
        playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null) playerCollider = GetComponentInChildren<Collider2D>();
        
        if (playerCollider != null)
        {
            playerRadius = Mathf.Max(playerCollider.bounds.extents.x, playerCollider.bounds.extents.y);
            if (navGrid != null) navGrid.SetAgentRadius(playerRadius);
        }
        else
        {
            playerRadius = 0.25f;
        }
    }

    void Update()
    {
        if (!active || movement == null || player == null) return;

        if (targetTransform != null) targetPoint = targetTransform.position;

        UpdateSprintState();

        if (CheckAndHandleStuck()) return;

        ExecuteNavigation();
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
        targetTransform = t;
        followStopRadius = Mathf.Max(0.1f, stopRadius);
        active = true;

        if (SprintStateManager.Instance != null)
            runWhileNavigating = SprintStateManager.Instance.ShouldNavigationSprint();

        BuildPath();
        ResetStuckDetection();
    }

    public void Cancel()
    {
        active = false;
        targetTransform = null;
        path.Clear();
        pathIndex = 0;
        stuckRetryCount = 0;
        runWhileNavigating = false;
        if (movement != null) movement.SetMovementInput(Vector2.zero, false);
    }

    public void ToggleRunWhileNavigating() { }

    public void SetRunWhileNavigating(bool run) { runWhileNavigating = run; }

    private void ExecuteNavigation()
    {
        Vector2 playerPos = GetPlayerPosition();

        if (path.Count == 0)
        {
            BuildPath();
            if (path.Count == 0) { MoveDirectly(playerPos); return; }
        }

        if (pathIndex >= path.Count) { Cancel(); return; }

        // 视线优化：尝试跳过中间路径点
        if (enableLineOfSightOptimization)
        {
            TrySkipWaypoints(playerPos);
        }

        Vector2 waypoint = path[pathIndex];
        Vector2 toWaypoint = waypoint - playerPos;
        float distance = toWaypoint.magnitude;
        float stopDist = (pathIndex == path.Count - 1) ? GetFinalStopDistance() : waypointTolerance;

        if (distance <= stopDist)
        {
            if (pathIndex < path.Count - 1) pathIndex++;
            else Cancel();
            return;
        }

        // 计算移动方向
        Vector2 moveDir = toWaypoint.normalized;
        
        // 碰撞调整（只在必要时）
        moveDir = AdjustDirectionByColliders(playerPos, moveDir);
        
        // 🔥 关键：斜向移动时固定朝向为左或右
        // 这样可以避免角色摇头
        Vector2 facingDir = GetFacingDirection(moveDir);
        
        movement.SetMovementInput(moveDir, runWhileNavigating, facingDir);
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
    /// </summary>
    private bool HasLineOfSight(Vector2 from, Vector2 to)
    {
        Vector2 direction = to - from;
        float distance = direction.magnitude;
        if (distance < 0.1f) return true;
        
        float checkRadius = playerRadius + losSafetyMargin;
        int sampleCount = Mathf.Max(3, Mathf.CeilToInt(distance / 0.3f));
        
        for (int i = 0; i <= sampleCount; i++)
        {
            float t = i / (float)sampleCount;
            Vector2 point = Vector2.Lerp(from, to, t);
            
            var hits = Physics2D.OverlapCircleAll(point, checkRadius);
            foreach (var hit in hits)
            {
                if (IsPlayerCollider(hit)) continue;
                if (IsObstacle(hit)) return false;
            }
        }
        
        return true;
    }

    private void MoveDirectly(Vector2 playerPos)
    {
        Vector2 toTarget = (Vector2)targetPoint - playerPos;
        if (toTarget.magnitude <= GetFinalStopDistance()) { Cancel(); return; }
        
        Vector2 moveDir = toTarget.normalized;
        Vector2 facingDir = GetFacingDirection(moveDir);
        movement.SetMovementInput(moveDir, runWhileNavigating, facingDir);
    }

    private void BuildPath()
    {
        path.Clear();
        pathIndex = 0;
        debugLogs.Clear();
        
        if (navGrid == null) 
        {
            AddDebugLog("NavGrid2D 未找到");
            return;
        }

        Vector2 start = GetPlayerPosition();
        Vector2 end = targetPoint;

        AddDebugLog($"开始寻路: 起点={start}, 终点={end}");

        // 检查起点是否可走
        if (!navGrid.IsWalkable(start))
        {
            AddDebugLog($"起点不可走，尝试查找最近可走点");
            
            if (!navGrid.TryFindNearestWalkable(start, out Vector2 validStart))
            {
                AddDebugLog("无法找到有效起点");
                LogFullDebugInfo("起点不可走且无法找到替代点");
                return;
            }
            AddDebugLog($"找到替代起点: {validStart}");
            start = validStart;
        }

        // 检查终点是否可走
        Vector2 actualEnd = end;
        if (!navGrid.IsWalkable(end))
        {
            AddDebugLog($"终点不可走，尝试查找最近可走点");
            
            if (navGrid.TryFindNearestWalkable(end, out Vector2 nearEnd))
            {
                AddDebugLog($"找到替代终点: {nearEnd}");
                actualEnd = nearEnd;
            }
            else
            {
                AddDebugLog("无法找到有效终点");
            }
        }

        // 尝试寻路
        if (!navGrid.TryFindPath(start, actualEnd, path))
        {
            AddDebugLog($"A* 寻路失败");
            LogFullDebugInfo("寻路失败");
            return;
        }

        AddDebugLog($"寻路成功: {path.Count} 个路径点");

        // 路径平滑处理
        SmoothPath();
        
        // 清理身后路径点
        CleanupPathBehindPlayer();
        
        if (enableDetailedDebug)
        {
            Debug.Log($"<color=green>[Nav] 路径构建成功：{path.Count} 个路径点</color>");
        }
    }

    /// <summary>
    /// 路径平滑处理 - 移除不必要的中间点
    /// </summary>
    private void SmoothPath()
    {
        if (path.Count < 3) return;
        
        List<Vector2> smoothed = new List<Vector2>();
        smoothed.Add(path[0]);
        
        int current = 0;
        while (current < path.Count - 1)
        {
            // 从当前点往后找，看能直接到达哪个点
            int farthest = current + 1;
            for (int i = path.Count - 1; i > current + 1; i--)
            {
                if (HasLineOfSight(path[current], path[i]))
                {
                    farthest = i;
                    break;
                }
            }
            
            smoothed.Add(path[farthest]);
            current = farthest;
        }
        
        int removed = path.Count - smoothed.Count;
        if (removed > 0)
        {
            AddDebugLog($"路径平滑：移除 {removed} 个冗余点");
        }
        
        path.Clear();
        path.AddRange(smoothed);
    }

    private void CleanupPathBehindPlayer()
    {
        if (path.Count < 2) return;
        Vector2 playerPos = GetPlayerPosition();

        while (path.Count > 1)
        {
            Vector2 first = path[0];
            if (Vector2.Distance(playerPos, first) < waypointTolerance * 0.5f) { path.RemoveAt(0); continue; }

            Vector2 second = path[1];
            Vector2 pathDir = (second - first).normalized;
            Vector2 toFirst = (first - playerPos).normalized;
            if (Vector2.Dot(toFirst, pathDir) < -0.2f) { path.RemoveAt(0); continue; }
            break;
        }
    }

    private bool CheckAndHandleStuck()
    {
        if (Time.time - lastCheckTime < STUCK_CHECK_INTERVAL) return false;

        Vector2 currentPos = GetPlayerPosition();
        float movedDistance = Vector2.Distance(currentPos, lastCheckPosition);
        lastCheckPosition = currentPos;
        lastCheckTime = Time.time;

        if (movedDistance < STUCK_THRESHOLD)
        {
            stuckRetryCount++;
            
            AddDebugLog($"检测到卡顿 ({stuckRetryCount}/{MAX_STUCK_RETRIES})，移动距离={movedDistance:F3}m");
            
            if (stuckRetryCount >= MAX_STUCK_RETRIES)
            {
                LogFullDebugInfo($"卡顿 {stuckRetryCount} 次后取消导航");
                Debug.LogWarning($"<color=red>[Nav] 卡顿 {stuckRetryCount} 次，取消导航</color>");
                Cancel();
                return true;
            }
            
            Debug.Log($"<color=yellow>[Nav] 检测到卡顿（{stuckRetryCount}/{MAX_STUCK_RETRIES}），重建路径</color>");
            BuildPath();
            
            if (path.Count == 0) 
            { 
                LogFullDebugInfo("重建路径失败");
                Cancel(); 
                return true; 
            }
        }
        else
        {
            stuckRetryCount = 0;
        }
        return false;
    }

    private void ResetStuckDetection()
    {
        lastCheckPosition = GetPlayerPosition();
        lastCheckTime = Time.time;
        stuckRetryCount = 0;
        debugLogs.Clear();
    }

    private Vector2 AdjustDirectionByColliders(Vector2 pos, Vector2 desiredDir)
    {
        if (playerCollider == null) return desiredDir;

        // 多点前瞻采样
        float[] aheadDistances = runWhileNavigating 
            ? new float[] { 0.15f, 0.35f, 0.6f }
            : new float[] { 0.1f, 0.25f, 0.45f };
        
        float clearance = playerRadius + 0.05f;
        Vector2 totalRepulse = Vector2.zero;
        int obstacleCount = 0;
        
        foreach (float ahead in aheadDistances)
        {
            Vector2 probe = pos + desiredDir * ahead;
            var hits = Physics2D.OverlapCircleAll(probe, clearance);
            float weight = 1f / (ahead + 0.1f);
            
            foreach (var hit in hits)
            {
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

    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad) * v.x - Mathf.Sin(rad) * v.y, Mathf.Sin(rad) * v.x + Mathf.Cos(rad) * v.y);
    }

    private Vector2 GetPlayerPosition()
    {
        return playerCollider != null ? (Vector2)playerCollider.bounds.center : (Vector2)player.position;
    }

    private float GetFinalStopDistance()
    {
        return targetTransform != null ? followStopRadius : stopDistance;
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

    private bool IsObstacle(Collider2D col)
    {
        if (col.name.Contains("(Clone)") || col.name.Contains("Pickup")) return false;

        if (losObstacleTags != null && losObstacleTags.Length > 0 && HasAnyTag(col.transform, losObstacleTags))
            return true;

        if (losObstacleMask.value != 0 && ((1 << col.gameObject.layer) & losObstacleMask.value) != 0)
            return true;

        return false;
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

    #region 调试输出

    private void AddDebugLog(string message)
    {
        debugLogs.Add($"[{Time.frameCount}] {message}");
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
    }
}
