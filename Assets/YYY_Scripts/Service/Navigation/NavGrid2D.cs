using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 轻量 2D 网格寻路（A*）
/// - 通过 Physics2D 圆形探测判定阻挡
/// - 世界→网格映射可在 Inspector 设置
/// </summary>
public class NavGrid2D : MonoBehaviour
{
    [Header("网格设置")]
    [SerializeField] private Vector2 worldOrigin = Vector2.zero;
    [SerializeField] private Vector2 worldSize = new Vector2(50, 50);
    [SerializeField, Min(0.05f)] private float cellSize = 0.5f;  // 单元格大小(Unity单位)，16px游戏中0.5=8像素
    [SerializeField, Range(0.05f, 100f)] private float probeRadius = 0.5f;  // 障碍物探测半径，必须≥障碍物碰撞体半径！16px游戏中0.5=8像素圆
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private bool eightDirections = true;
    [SerializeField] private bool strictCornerCutting = true;  // 严格的对角线检测，防止穿墙
    [Header("世界边界设置")]
    [SerializeField] private bool autoDetectWorldBounds = true;  // 自动检测世界边界（基于Tilemap+场景物体）
    [SerializeField] private string[] worldLayerNames = new string[] { "LAYER 1", "LAYER 2", "LAYER 3" };  // 世界层级名称
    [SerializeField] private float boundsPadding = 5f;  // 边界扩展（留出余量）
    [Header("障碍物标签(可多选)")]
    [SerializeField] private string[] obstacleTags = new string[0];
    [Header("调试")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private bool logObstacleDetection = false;

    private int gridW, gridH;
    private bool[,] walkable;
    private static NavGrid2D s_instance;

    // 🔥 Unity 6 优化：预分配碰撞体缓存数组，避免 GC 分配
    private Collider2D[] _colliderCache = new Collider2D[10];
    private ContactFilter2D _obstacleFilter;

    // 公共事件：外部可调用以通知网格需要刷新
    public static System.Action OnRequestGridRefresh;

    void Awake()
    {
        s_instance = this;
        ValidateParameters();
        _obstacleFilter = new ContactFilter2D().NoFilter();
        
        // 订阅外部刷新请求
        OnRequestGridRefresh += RefreshGrid;
    }

    /// <summary>
    /// 设置代理（玩家）半径为网格探测半径，支持运行时同步。
    /// </summary>
    public void SetAgentRadius(float radius, bool rebuild = true)
    {
        probeRadius = Mathf.Clamp(radius, 0.05f, 100f);
        if (rebuild) RebuildGrid();
    }

    /// <summary>
    /// 获取当前的网格探测半径。
    /// </summary>
    public float GetAgentRadius()
    {
        return probeRadius;
    }

    void OnEnable()
    {
        // 组件启用时立即重建网格
        RebuildGrid();
    }

    void OnValidate()
    {
        // 编辑器中修改参数时自动校验
        ValidateParameters();
    }

    void Start()
    {
        // Start时延迟刷新，确保所有动态障碍物已生成
        Invoke(nameof(RebuildGrid), 0.5f);
    }

    void OnDestroy()
    {
        if (s_instance == this) s_instance = null;
        OnRequestGridRefresh -= RefreshGrid;
    }

    /// <summary>
    /// 手动刷新网格（供外部调用，解决运行时障碍物变化导致的导航失效）
    /// </summary>
    public void RefreshGrid()
    {
        RebuildGrid();
    }

    private void ValidateParameters()
    {
        // 允许更小的探测半径以支持狭窄通道（与代理碰撞体等宽）
        const float MinProbe = 0.05f;
        const float MaxProbe = 100f;
        
        if (probeRadius < MinProbe)
        {
            probeRadius = MinProbe;
        }
        else if (probeRadius > MaxProbe)
        {
            probeRadius = MaxProbe;
        }
        
        if (cellSize < 0.05f)
        {
            Debug.LogWarning($"[NavGrid2D] cellSize={cellSize} 过小，已重置为 0.5");
            cellSize = 0.5f;
        }
    }

    public void RebuildGrid()
    {
        // 🔥 关键修复：同步物理系统的 Transform 变化
        // 动态障碍物（如树木成长、箱子放置）修改碰撞体后，
        // Physics2D 内部缓存可能未更新，导致 OverlapCircle 检测到旧数据
        Physics2D.SyncTransforms();
        
        // 自动检测世界边界
        if (autoDetectWorldBounds)
        {
            DetectWorldBounds();
        }
        gridW = Mathf.Max(1, Mathf.RoundToInt(worldSize.x / cellSize));
        gridH = Mathf.Max(1, Mathf.RoundToInt(worldSize.y / cellSize));
        walkable = new bool[gridW, gridH];

        int obstacleCount = 0;
        for (int x = 0; x < gridW; x++)
        {
            for (int y = 0; y < gridH; y++)
            {
                Vector2 w = GridToWorldCenter(x, y);
                
                // 多点采样提高精度：中心 + 4角
                bool blocked = IsPointBlocked(w, probeRadius);
                if (!blocked && strictCornerCutting)
                {
                    // 额外检查格子四角，防止大型障碍物漏检
                    float offset = cellSize * 0.35f;
                    blocked = IsPointBlocked(w + new Vector2(offset, offset), probeRadius * 0.7f) ||
                              IsPointBlocked(w + new Vector2(-offset, offset), probeRadius * 0.7f) ||
                              IsPointBlocked(w + new Vector2(offset, -offset), probeRadius * 0.7f) ||
                              IsPointBlocked(w + new Vector2(-offset, -offset), probeRadius * 0.7f);
                }
                
                walkable[x, y] = !blocked;
                if (blocked) obstacleCount++;
            }
        }
        
        if (logObstacleDetection)
        {
            Debug.Log($"[NavGrid2D] 网格重建完毕: {gridW}x{gridH}={gridW*gridH} 单元，障碍物={obstacleCount}");
        }
    }

    /// <summary>
    /// 自动检测世界边界（基于Tilemap和场景物体）
    /// </summary>
    private void DetectWorldBounds()
    {
        Bounds totalBounds = new Bounds(Vector3.zero, Vector3.zero);
        bool boundsInitialized = false;

        // 1. 检测所有Tilemap的边界
        var tilemaps = FindObjectsByType<UnityEngine.Tilemaps.Tilemap>(FindObjectsSortMode.None);
        foreach (var tilemap in tilemaps)
        {
            // 过滤：只包含LAYER 1/2/3下的Tilemap
            if (!IsInWorldLayers(tilemap.transform))
                continue;

            if (tilemap.cellBounds.size.x > 0 && tilemap.cellBounds.size.y > 0)
            {
                Bounds tilemapBounds = tilemap.localBounds;
                Vector3 worldMin = tilemap.transform.TransformPoint(tilemapBounds.min);
                Vector3 worldMax = tilemap.transform.TransformPoint(tilemapBounds.max);
                Bounds worldBounds = new Bounds();
                worldBounds.SetMinMax(worldMin, worldMax);

                if (!boundsInitialized)
                {
                    totalBounds = worldBounds;
                    boundsInitialized = true;
                }
                else
                {
                    totalBounds.Encapsulate(worldBounds);
                }
            }
        }

        // 2. 检测所有Collider2D的边界（作为补充）
        var colliders = FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
        foreach (var col in colliders)
        {
            if (!IsInWorldLayers(col.transform))
                continue;

            // 跳过玩家和临时物体
            if (col.CompareTag("Player") || col.name.Contains("(Clone)") || col.name.Contains("Pickup"))
                continue;

            if (!boundsInitialized)
            {
                totalBounds = col.bounds;
                boundsInitialized = true;
            }
            else
            {
                totalBounds.Encapsulate(col.bounds);
            }
        }

        if (boundsInitialized)
        {
            // 扩展边界（留出余量）
            totalBounds.Expand(boundsPadding * 2f);

            worldOrigin = new Vector2(totalBounds.min.x, totalBounds.min.y);
            worldSize = new Vector2(totalBounds.size.x, totalBounds.size.y);

            if (logObstacleDetection)
            {
                Debug.Log($"[NavGrid2D] 自动检测世界边界: Origin={worldOrigin}, Size={worldSize}");
            }
        }
        else
        {
            Debug.LogWarning("[NavGrid2D] 未能检测到任何Tilemap或Collider，使用默认边界");
        }
    }

    /// <summary>
    /// 检查物体是否在世界层级下（LAYER 1/2/3）
    /// </summary>
    private bool IsInWorldLayers(Transform t)
    {
        if (worldLayerNames == null || worldLayerNames.Length == 0)
            return true;

        Transform current = t;
        while (current != null)
        {
            foreach (var layerName in worldLayerNames)
            {
                if (current.name == layerName)
                    return true;
            }
            current = current.parent;
        }
        return false;
    }

    private bool IsPointBlocked(Vector2 worldPos, float radius, Collider2D ignoredCollider = null)
    {
        int hitCount = Physics2D.OverlapCircle(worldPos, radius, _obstacleFilter, _colliderCache);
        if (hitCount == _colliderCache.Length)
        {
            System.Array.Resize(ref _colliderCache, _colliderCache.Length * 2);
            hitCount = Physics2D.OverlapCircle(worldPos, radius, _obstacleFilter, _colliderCache);
        }
        
        // 先检查标签
        if (obstacleTags != null && obstacleTags.Length > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hitCollider = _colliderCache[i];
                if (ShouldIgnoreCollider(hitCollider, ignoredCollider))
                    continue;

                if (ShouldIgnoreDynamicNavigationCollider(hitCollider))
                    continue;

                var hitTransform = hitCollider.transform;
                // 跳过生成的物体
                if (hitTransform.name.Contains("(Clone)") || hitTransform.name.Contains("Pickup"))
                    continue;
                
                // 检查标签（包括父级）
                if (HasAnyTag(hitTransform, obstacleTags))
                {
                    return true;
                }
            }
        }
        
        // 如果标签没检测到，再用LayerMask检测
        if (obstacleMask.value != 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hitCollider = _colliderCache[i];
                if (ShouldIgnoreCollider(hitCollider, ignoredCollider))
                    continue;

                if (ShouldIgnoreDynamicNavigationCollider(hitCollider))
                    continue;

                if (((1 << hitCollider.gameObject.layer) & obstacleMask.value) != 0)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    public bool TryFindPath(Vector2 startWorld, Vector2 endWorld, List<Vector2> outPath)
    {
        outPath.Clear();
        if (!WorldToGrid(startWorld, out int sx, out int sy)) return false;
        if (!WorldToGrid(endWorld, out int tx, out int ty)) return false;

        // ✅ 改进：使用智能起始点查找，考虑目标方向
        if (!IsWalkable(sx, sy))
        {
            if (!FindSmartStartPoint(startWorld, endWorld, sx, sy, out sx, out sy)) return false;
        }
        if (!IsWalkable(tx, ty))
        {
            if (!FindNearestWalkable(tx, ty, 6, out tx, out ty)) return false;
        }

        var open = new List<Node>();
        var closed = new bool[gridW, gridH];
        var parent = new Vector2Int[gridW, gridH];
        for (int i = 0; i < gridW; i++)
            for (int j = 0; j < gridH; j++) parent[i, j] = new Vector2Int(-1, -1);

        Node start = new Node(sx, sy, 0, Heuristic(sx, sy, tx, ty));
        open.Add(start);

        Node found = null;
        while (open.Count > 0)
        {
            open.Sort((a, b) => a.f.CompareTo(b.f));
            Node cur = open[0];
            open.RemoveAt(0);
            if (closed[cur.x, cur.y]) continue;
            closed[cur.x, cur.y] = true;

            if (cur.x == tx && cur.y == ty)
            {
                found = cur;
                break;
            }

            foreach (var n in Neighbors(cur.x, cur.y))
            {
                if (!IsWalkable(n.x, n.y) || closed[n.x, n.y]) continue;
                int ng = cur.g + Cost(cur.x, cur.y, n.x, n.y);
                int nh = Heuristic(n.x, n.y, tx, ty);
                Node nn = new Node(n.x, n.y, ng, nh);
                // 如果更优或未在 open 中，加入
                bool better = true;
                for (int i = 0; i < open.Count; i++)
                {
                    if (open[i].x == nn.x && open[i].y == nn.y)
                    {
                        if (open[i].f <= nn.f) better = false;
                        else open.RemoveAt(i);
                        break;
                    }
                }
                if (better)
                {
                    open.Add(nn);
                    parent[nn.x, nn.y] = new Vector2Int(cur.x, cur.y);
                }
            }
        }

        if (found == null) return false;

        // 回溯
        var stack = new Stack<Vector2Int>();
        Vector2Int p = new Vector2Int(found.x, found.y);
        stack.Push(p);
        while (parent[p.x, p.y].x >= 0)
        {
            p = parent[p.x, p.y];
            stack.Push(p);
        }

        while (stack.Count > 0)
        {
            var g = stack.Pop();
            outPath.Add(GridToWorldCenter(g.x, g.y));
        }
        return true;
    }

    /// <summary>
    /// 检查世界坐标是否可走（公共接口）
    /// </summary>
    public bool IsWalkable(Vector2 worldPos, Collider2D ignoredCollider = null)
    {
        if (!WorldToGrid(worldPos, out int gx, out int gy))
            return false;

        if (ignoredCollider != null)
        {
            Physics2D.SyncTransforms();
            return !IsPointBlocked(worldPos, probeRadius, ignoredCollider);
        }

        return IsWalkable(gx, gy);
    }

    public bool TryFindNearestWalkable(Vector2 world, out Vector2 nearestWorld, Collider2D ignoredCollider = null)
    {
        nearestWorld = world;
        if (!WorldToGrid(world, out int gx, out int gy)) return false;

        if (ignoredCollider != null)
        {
            Physics2D.SyncTransforms();
        }
        
        // 🔥 使用更大的搜索范围，确保能找到可走点
        // 并且找到真正最近的点，而不是第一个找到的点
        if (FindNearestWalkableImproved(gx, gy, 30, out int nx, out int ny, ignoredCollider))
        {
            nearestWorld = GridToWorldCenter(nx, ny);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 智能起始点查找：考虑目标方向，优先选择朝向目标的可走点
    /// 🔥 解决玩家紧贴障碍物时起始点选择不当的问题
    /// </summary>
    private bool FindSmartStartPoint(Vector2 playerWorldPos, Vector2 targetWorldPos, int gx, int gy, out int nx, out int ny)
    {
        nx = gx; ny = gy;
        
        // 如果当前位置可走，直接返回
        if (InBounds(gx, gy) && IsWalkable(gx, gy)) return true;
        
        // 计算玩家到目标的方向向量
        Vector2 toTarget = (targetWorldPos - playerWorldPos).normalized;
        
        // 在玩家周围搜索可走点，使用评分系统
        float bestScore = float.MinValue;
        bool found = false;
        
        // 🔥 搜索范围：从小到大，找到就停止
        for (int r = 1; r <= 10; r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    // 只检查当前半径圆圈上的点
                    if (System.Math.Abs(dx) != r && System.Math.Abs(dy) != r) continue;
                    
                    int x = gx + dx;
                    int y = gy + dy;
                    
                    if (InBounds(x, y) && IsWalkable(x, y))
                    {
                        Vector2 candidateWorld = GridToWorldCenter(x, y);
                        Vector2 toCandidate = (candidateWorld - playerWorldPos).normalized;
                        
                        // 🔥 评分系统：
                        // 1. 方向得分：与目标方向的点积 [-1, 1]，越接近目标方向越好
                        float directionScore = Vector2.Dot(toCandidate, toTarget);
                        
                        // 2. 距离得分：距离越近越好
                        float distSq = dx * dx + dy * dy;
                        float distanceScore = 1f / (1f + Mathf.Sqrt(distSq));
                        
                        // 3. 综合评分：方向权重 70%，距离权重 30%
                        float score = directionScore * 7f + distanceScore * 3f;
                        
                        if (score > bestScore)
                        {
                            bestScore = score;
                            nx = x;
                            ny = y;
                            found = true;
                        }
                    }
                }
            }
            
            // 🔥 找到就停止，不继续扩大搜索范围
            if (found) break;
        }
        
        return found;
    }
    
    /// <summary>
    /// 改进的最近可走点查找：返回欧几里得距离最近的点
    /// 🔥 如果有多个距离相等的点，优先选择：下 > 右 > 上 > 左（确保稳定性）
    /// </summary>
    private bool FindNearestWalkableImproved(int gx, int gy, int maxRange, out int nx, out int ny, Collider2D ignoredCollider = null)
    {
        nx = gx; ny = gy;
        if (IsGridWalkableForQuery(gx, gy, ignoredCollider)) return true;
        
        // 🔥 在搜索范围内找到欧几里得距离最近的可走点
        float bestDistSq = float.MaxValue;
        bool found = false;
        
        // 🔥 优先级：用于距离相等时的选择（确保稳定性）
        int bestPriority = int.MaxValue;
        
        // 使用螺旋搜索：从内向外扩展
        for (int r = 1; r <= maxRange; r++)
        {
            // 遍历当前半径圆圈上的所有点
            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    // 只检查当前半径圆圈上的点（切比雪夫距离 = r）
                    if (System.Math.Abs(dx) != r && System.Math.Abs(dy) != r) continue;
                    
                    int x = gx + dx;
                    int y = gy + dy;
                    
                    if (IsGridWalkableForQuery(x, y, ignoredCollider))
                    {
                        // 计算欧几里得距离的平方（避免开方运算）
                        float distSq = dx * dx + dy * dy;
                        
                        // 🔥 计算方向优先级：下(0) > 右(1) > 上(2) > 左(3) > 斜向(4+)
                        // 这确保了相同距离时总是选择相同方向
                        int priority = GetDirectionPriority(dx, dy);
                        
                        // 🔥 更新最佳点：距离更近，或距离相同但优先级更高
                        bool shouldUpdate = false;
                        if (distSq < bestDistSq - 0.01f)  // 距离明显更近
                        {
                            shouldUpdate = true;
                        }
                        else if (Mathf.Abs(distSq - bestDistSq) < 0.01f && priority < bestPriority)  // 距离相等但优先级更高
                        {
                            shouldUpdate = true;
                        }
                        
                        if (shouldUpdate)
                        {
                            bestDistSq = distSq;
                            bestPriority = priority;
                            nx = x;
                            ny = y;
                            found = true;
                        }
                    }
                }
            }
            
            // 🔥 优化：如果已经在当前半径找到点，后续半径只可能更远，可以提前结束
            if (found && r > Mathf.Sqrt(bestDistSq) + 1)
            {
                break;  // 当前半径已经大于最佳距离+1，后续不可能更近
            }
        }
        
        return found;
    }
    
    /// <summary>
    /// 获取方向优先级（确保相同距离时选择固定方向）
    /// 优先级：下(0) > 右(1) > 上(2) > 左(3) > 斜向(4+)
    /// </summary>
    private int GetDirectionPriority(int dx, int dy)
    {
        // 正下方（最优先，因为通常从障碍物下方走出）
        if (dx == 0 && dy < 0) return 0;
        // 正右方
        if (dx > 0 && dy == 0) return 1;
        // 正上方
        if (dx == 0 && dy > 0) return 2;
        // 正左方
        if (dx < 0 && dy == 0) return 3;
        
        // 斜向：按象限优先级
        if (dx > 0 && dy < 0) return 4;  // 右下
        if (dx > 0 && dy > 0) return 5;  // 右上
        if (dx < 0 && dy > 0) return 6;  // 左上
        if (dx < 0 && dy < 0) return 7;  // 左下
        
        return 8;  // 其他情况
    }
    
    private bool FindNearestWalkable(int gx, int gy, int maxRange, out int nx, out int ny)
    {
        nx = gx; ny = gy;
        if (InBounds(gx, gy) && IsWalkable(gx, gy)) return true;
        for (int r = 1; r <= maxRange; r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                int x = gx + dx;
                int y1 = gy + r;
                int y2 = gy - r;
                if (InBounds(x, y1) && IsWalkable(x, y1)) { nx = x; ny = y1; return true; }
                if (InBounds(x, y2) && IsWalkable(x, y2)) { nx = x; ny = y2; return true; }
            }
            for (int dy = -r + 1; dy <= r - 1; dy++)
            {
                int y = gy + dy;
                int x1 = gx + r;
                int x2 = gx - r;
                if (InBounds(x1, y) && IsWalkable(x1, y)) { nx = x1; ny = y; return true; }
                if (InBounds(x2, y) && IsWalkable(x2, y)) { nx = x2; ny = y; return true; }
            }
        }
        return false;
    }

    private IEnumerable<Vector2Int> Neighbors(int x, int y)
    {
        // 4-或8方向
        yield return new Vector2Int(x + 1, y);
        yield return new Vector2Int(x - 1, y);
        yield return new Vector2Int(x, y + 1);
        yield return new Vector2Int(x, y - 1);
        if (eightDirections)
        {
            foreach (var diag in DiagonalNeighbors(x, y))
                yield return diag;
        }
    }

    private IEnumerable<Vector2Int> DiagonalNeighbors(int x, int y)
    {
        if (TryMakeDiagonal(x, y, 1, 1, out var a)) yield return a;
        if (TryMakeDiagonal(x, y, 1, -1, out var b)) yield return b;
        if (TryMakeDiagonal(x, y, -1, 1, out var c)) yield return c;
        if (TryMakeDiagonal(x, y, -1, -1, out var d)) yield return d;
    }

    private bool TryMakeDiagonal(int x, int y, int dx, int dy, out Vector2Int result)
    {
        int nx = x + dx;
        int ny = y + dy;
        if (!InBounds(nx, ny)) { result = default; return false; }
        
        // 严格的corner cutting检测：两条相邻边和对角格都必须可走
        int adjX = x + dx;
        int adjY = y;
        int adjX2 = x;
        int adjY2 = y + dy;
        
        if (!IsWalkable(adjX, adjY) || !IsWalkable(adjX2, adjY2))
        {
            result = default;
            return false;
        }
        
        // 额外检查：对角格本身也必须可走
        if (!IsWalkable(nx, ny))
        {
            result = default;
            return false;
        }
        
        // 如果启用严格模式，额外检查对角线中点是否有障碍物
        if (strictCornerCutting)
        {
            Vector2 from = GridToWorldCenter(x, y);
            Vector2 to = GridToWorldCenter(nx, ny);
            Vector2 mid = (from + to) * 0.5f;
            if (IsPointBlocked(mid, probeRadius * 0.5f))
            {
                result = default;
                return false;
            }
        }
        
        result = new Vector2Int(nx, ny);
        return true;
    }

    private int Heuristic(int x1, int y1, int x2, int y2)
    {
        int dx = Mathf.Abs(x1 - x2);
        int dy = Mathf.Abs(y1 - y2);
        if (eightDirections)
        {
            int diag = Mathf.Min(dx, dy);
            int straight = dx + dy - 2 * diag;
            return 14 * diag + 10 * straight;
        }
        else
        {
            return 10 * (dx + dy);
        }
    }

    private int Cost(int x1, int y1, int x2, int y2)
    {
        int dx = Mathf.Abs(x1 - x2);
        int dy = Mathf.Abs(y1 - y2);
        return (dx + dy == 2) ? 14 : 10;
    }

    private class Node { public int x, y, g, h; public int f => g + h; public Node(int x, int y, int g, int h){this.x=x;this.y=y;this.g=g;this.h=h;} }

    public bool WorldToGrid(Vector2 world, out int gx, out int gy)
    {
        Vector2 local = world - worldOrigin;
        // ✅ 使用 FloorToInt 向下取整，确保映射到玩家所在的格子
        // 这样可以更准确地反映玩家的实际位置
        gx = Mathf.FloorToInt(local.x / cellSize);
        gy = Mathf.FloorToInt(local.y / cellSize);
        return InBounds(gx, gy);
    }

    public Vector2 GridToWorldCenter(int gx, int gy)
    {
        return worldOrigin + new Vector2((gx + 0.5f) * cellSize, (gy + 0.5f) * cellSize);
    }

    private bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < gridW && y < gridH;
    }

    private bool IsWalkable(int x, int y)
    {
        return InBounds(x, y) && walkable[x, y];
    }

    private bool IsGridWalkableForQuery(int x, int y, Collider2D ignoredCollider)
    {
        if (!InBounds(x, y))
        {
            return false;
        }

        if (ignoredCollider == null)
        {
            return IsWalkable(x, y);
        }

        return !IsPointBlocked(GridToWorldCenter(x, y), probeRadius, ignoredCollider);
    }

    private static bool ShouldIgnoreCollider(Collider2D hitCollider, Collider2D ignoredCollider)
    {
        if (hitCollider == null || ignoredCollider == null)
        {
            return false;
        }

        if (hitCollider == ignoredCollider)
        {
            return true;
        }

        if (ignoredCollider.attachedRigidbody != null &&
            hitCollider.attachedRigidbody != null &&
            hitCollider.attachedRigidbody == ignoredCollider.attachedRigidbody)
        {
            return true;
        }

        return hitCollider.transform.root == ignoredCollider.transform.root;
    }

    private static bool ShouldIgnoreDynamicNavigationCollider(Collider2D hitCollider)
    {
        if (hitCollider == null)
        {
            return false;
        }

        MonoBehaviour[] behaviours = hitCollider.GetComponentsInParent<MonoBehaviour>(includeInactive: false);
        for (int index = 0; index < behaviours.Length; index++)
        {
            if (behaviours[index] is INavigationUnit navigationUnit &&
                navigationUnit.GetUnitType() != NavigationUnitType.StaticObstacle)
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasAnyTag(Transform t, string[] tags)
    {
        if (t == null || tags == null) return false;
        
        // 检查自身和所有父级的Tag
        Transform current = t;
        while (current != null)
        {
            foreach (var tag in tags)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    // 去除空格并比较
                    string trimmedTag = tag.Trim();
                    if (current.CompareTag(trimmedTag))
                    {
                        return true;
                    }
                }
            }
            current = current.parent;
        }
        return false;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;
        
        Gizmos.color = Color.cyan;
        if (Application.isPlaying && walkable != null)
        {
            // 运行时显示网格：绿色=可走，红色=障碍物
            for (int x = 0; x < gridW; x++)
            for (int y = 0; y < gridH; y++)
            {
                Color c = walkable[x, y] ? new Color(0,1,0,0.15f) : new Color(1,0,0,0.35f);
                Gizmos.color = c;
                Vector2 p = GridToWorldCenter(x, y);
                Gizmos.DrawCube(p, Vector3.one * cellSize * 0.95f);
            }
            // 显示probeRadius范围（每5个格子显示一次避免太密）
            Gizmos.color = new Color(1, 1, 0, 0.3f);
            for (int x = 0; x < gridW; x += 5)
            for (int y = 0; y < gridH; y += 5)
            {
                Vector2 p = GridToWorldCenter(x, y);
                Gizmos.DrawWireSphere(p, probeRadius);
            }
        }
        else
        {
            // 预览边界
            Gizmos.DrawWireCube(worldOrigin + worldSize * 0.5f, worldSize);
        }
    }
#endif
}
