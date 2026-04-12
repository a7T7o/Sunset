using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 轻量 2D 网格寻路（A*）
/// - 通过 Physics2D 圆形探测判定阻挡
/// - 世界→网格映射可在 Inspector 设置
/// </summary>
public class NavGrid2D : MonoBehaviour
{
    private static readonly bool EnableNavGridRefreshProfiling = false;
    private const double RefreshProfileLogThresholdMs = 20d;

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
    [Header("显式障碍 Tilemap")]
    [SerializeField] private bool autoDetectObstacleTilemapsByName = false;
    [SerializeField] private string[] obstacleTilemapNameKeywords = new string[0];
    [SerializeField] private Tilemap[] explicitObstacleTilemaps = new Tilemap[0];
    [Header("显式障碍碰撞体")]
    [SerializeField] private Collider2D[] explicitObstacleColliders = new Collider2D[0];
    [Header("Traversal 软穿越障碍 Tilemap")]
    [SerializeField] private Tilemap[] explicitSoftPassObstacleTilemaps = new Tilemap[0];
    [Header("Traversal 软穿越障碍碰撞体")]
    [SerializeField] private Collider2D[] explicitSoftPassObstacleColliders = new Collider2D[0];
    [Header("显式可走覆盖 Tilemap")]
    [SerializeField] private Tilemap[] explicitWalkableOverrideTilemaps = new Tilemap[0];
    [Header("显式可走覆盖碰撞体")]
    [SerializeField] private Collider2D[] explicitWalkableOverrideColliders = new Collider2D[0];
    [Header("可走覆盖支撑检测")]
    [SerializeField, Min(0.001f)] private float walkableOverrideSupportRadius = 0.08f;
    [Header("调试")]
#pragma warning disable CS0414 // Used by editor gizmo drawing.
    [SerializeField] private bool showDebugGizmos = true;
#pragma warning restore CS0414
    [SerializeField] private bool logObstacleDetection = false;
    [SerializeField] private bool scheduleDelayedRuntimeRebuild = false;

    private int gridW, gridH;
    private bool[,] walkable;
    private static NavGrid2D s_instance;
    private static readonly Dictionary<int, TileSpriteGeometry> s_tileSpriteGeometryCache = new Dictionary<int, TileSpriteGeometry>();
    private static readonly Dictionary<int, bool> s_dynamicNavigationColliderIgnoreCache = new Dictionary<int, bool>();
    private static readonly List<MonoBehaviour> s_navigationBehaviourBuffer = new List<MonoBehaviour>(8);
    private static int s_lastSyncedPhysicsFrame = -1;
    private const int NearestWalkableVerificationCandidateLimit = 8;
    private const float NearestWalkableCacheSeconds = 0.08f;

    // 🔥 Unity 6 优化：预分配碰撞体缓存数组，避免 GC 分配
    private Collider2D[] _colliderCache = new Collider2D[10];
    private Collider2D[] _walkableOverrideColliderCache = new Collider2D[6];
    private readonly Vector2Int[] _nearestWalkableVerificationCandidates = new Vector2Int[NearestWalkableVerificationCandidateLimit];
    private readonly float[] _nearestWalkableVerificationDistances = new float[NearestWalkableVerificationCandidateLimit];
    private readonly int[] _nearestWalkableVerificationPriorities = new int[NearestWalkableVerificationCandidateLimit];
    private readonly Dictionary<DynamicWalkableQueryKey, bool> _dynamicWalkableQueryCache = new Dictionary<DynamicWalkableQueryKey, bool>(256);
    private readonly Dictionary<NearestWalkableQueryKey, NearestWalkableQueryResult> _nearestWalkableQueryCache = new Dictionary<NearestWalkableQueryKey, NearestWalkableQueryResult>(128);
    private readonly Dictionary<PathCacheKey, Vector2[]> _pathQueryCache = new Dictionary<PathCacheKey, Vector2[]>(128);
    private ContactFilter2D _obstacleFilter;
    private double _lastFullRebuildRealtime = double.NegativeInfinity;
    private double _startupDelayedRebuildScheduledAt = double.NegativeInfinity;
    private int _lastRebuildFrame = -1;
    private int _runtimeQueryCacheFrame = -1;
    private int _gridVersion = 0;

    // 公共事件：外部可调用以通知网格需要刷新
    public static System.Action OnRequestGridRefresh;
    public int LastRebuildFrame => _lastRebuildFrame;

    private sealed class TileSpriteGeometry
    {
        public TileSpriteGeometry(Vector2[][] paths, Vector2[] meshVertices, ushort[] meshTriangles)
        {
            Paths = paths ?? new Vector2[0][];
            MeshVertices = meshVertices ?? new Vector2[0];
            MeshTriangles = meshTriangles ?? new ushort[0];
        }

        public Vector2[][] Paths { get; }
        public Vector2[] MeshVertices { get; }
        public ushort[] MeshTriangles { get; }
        public bool HasPaths => Paths.Length > 0;
        public bool HasMesh => MeshVertices.Length >= 3 && MeshTriangles.Length >= 3;
    }

    private readonly struct DynamicWalkableQueryKey : System.IEquatable<DynamicWalkableQueryKey>
    {
        public DynamicWalkableQueryKey(Vector2 worldPos, float radius, int ignoredColliderId)
        {
            WorldPos = worldPos;
            Radius = radius;
            IgnoredColliderId = ignoredColliderId;
        }

        public Vector2 WorldPos { get; }
        public float Radius { get; }
        public int IgnoredColliderId { get; }

        public bool Equals(DynamicWalkableQueryKey other)
        {
            return WorldPos.Equals(other.WorldPos) &&
                   Radius.Equals(other.Radius) &&
                   IgnoredColliderId == other.IgnoredColliderId;
        }

        public override bool Equals(object obj)
        {
            return obj is DynamicWalkableQueryKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 31) + WorldPos.GetHashCode();
                hash = (hash * 31) + Radius.GetHashCode();
                hash = (hash * 31) + IgnoredColliderId;
                return hash;
            }
        }
    }

    private readonly struct NearestWalkableQueryKey : System.IEquatable<NearestWalkableQueryKey>
    {
        public NearestWalkableQueryKey(int gx, int gy, int ignoredColliderId)
        {
            Gx = gx;
            Gy = gy;
            IgnoredColliderId = ignoredColliderId;
        }

        public int Gx { get; }
        public int Gy { get; }
        public int IgnoredColliderId { get; }

        public bool Equals(NearestWalkableQueryKey other)
        {
            return Gx == other.Gx &&
                   Gy == other.Gy &&
                   IgnoredColliderId == other.IgnoredColliderId;
        }

        public override bool Equals(object obj)
        {
            return obj is NearestWalkableQueryKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 31) + Gx;
                hash = (hash * 31) + Gy;
                hash = (hash * 31) + IgnoredColliderId;
                return hash;
            }
        }
    }

    private readonly struct NearestWalkableQueryResult
    {
        public NearestWalkableQueryResult(bool success, Vector2 nearestWorld, float cachedAtTime)
        {
            Success = success;
            NearestWorld = nearestWorld;
            CachedAtTime = cachedAtTime;
        }

        public bool Success { get; }
        public Vector2 NearestWorld { get; }
        public float CachedAtTime { get; }
    }

    private readonly struct PathCacheKey : System.IEquatable<PathCacheKey>
    {
        public PathCacheKey(int startX, int startY, int targetX, int targetY, int gridVersion)
        {
            StartX = startX;
            StartY = startY;
            TargetX = targetX;
            TargetY = targetY;
            GridVersion = gridVersion;
        }

        public int StartX { get; }
        public int StartY { get; }
        public int TargetX { get; }
        public int TargetY { get; }
        public int GridVersion { get; }

        public bool Equals(PathCacheKey other)
        {
            return StartX == other.StartX &&
                   StartY == other.StartY &&
                   TargetX == other.TargetX &&
                   TargetY == other.TargetY &&
                   GridVersion == other.GridVersion;
        }

        public override bool Equals(object obj)
        {
            return obj is PathCacheKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 31) + StartX;
                hash = (hash * 31) + StartY;
                hash = (hash * 31) + TargetX;
                hash = (hash * 31) + TargetY;
                hash = (hash * 31) + GridVersion;
                return hash;
            }
        }
    }

    void Awake()
    {
        s_instance = this;
        s_dynamicNavigationColliderIgnoreCache.Clear();
        s_lastSyncedPhysicsFrame = -1;
        ValidateParameters();
        RefreshExplicitObstacleSources();
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
        RefreshExplicitObstacleSources();
        if (!Application.isEditor
            && Application.isPlaying
            && HasActiveTraversalBootstrapManager())
        {
            return;
        }

        RebuildGrid();
    }

    void OnValidate()
    {
        // 编辑器中修改参数时自动校验
        ValidateParameters();
        obstacleTilemapNameKeywords = NormalizeKeywordArray(obstacleTilemapNameKeywords);
        RefreshExplicitObstacleSources();
    }

    void Start()
    {
        // 旧逻辑会在启用后再强打一轮整图重建，demo 首次进场代价过高。
        if (scheduleDelayedRuntimeRebuild)
        {
            _startupDelayedRebuildScheduledAt = Time.realtimeSinceStartupAsDouble;
            Invoke(nameof(DelayedRuntimeRebuildIfStillNeeded), 0.5f);
        }
    }

    void OnDestroy()
    {
        if (s_instance == this) s_instance = null;
        s_dynamicNavigationColliderIgnoreCache.Clear();
        s_lastSyncedPhysicsFrame = -1;
        InvalidateRuntimeQueryCaches();
        OnRequestGridRefresh -= RefreshGrid;
    }

    /// <summary>
    /// 手动刷新网格（供外部调用，解决运行时障碍物变化导致的导航失效）
    /// </summary>
    public void RefreshGrid()
    {
        double profileStart = Time.realtimeSinceStartupAsDouble;
        RefreshExplicitObstacleSources();
        RebuildGrid();
        LogRefreshProfileIfSlow("RefreshGrid(full)", profileStart);
    }

    public static bool TryRequestLocalRefresh(
        Bounds worldBounds,
        bool refreshExplicitSources = false,
        float extraPadding = -1f)
    {
        if (s_instance == null || !s_instance.isActiveAndEnabled)
        {
            return false;
        }

        return s_instance.RefreshGrid(worldBounds, refreshExplicitSources, extraPadding);
    }

    public bool RefreshGrid(
        Bounds worldBounds,
        bool refreshExplicitSources = false,
        float extraPadding = -1f)
    {
        double profileStart = Time.realtimeSinceStartupAsDouble;
        if (!isActiveAndEnabled)
        {
            return false;
        }

        if (refreshExplicitSources)
        {
            RefreshExplicitObstacleSources();
        }

        if (!TryNormalizeRuntimeRefreshBounds(worldBounds, extraPadding, out Bounds refreshBounds))
        {
            RebuildGrid();
            LogRefreshProfileIfSlow("RefreshGrid(fallback-rebuild)", profileStart);
            return true;
        }

        EnsurePhysicsTransformsSyncedOncePerFrame();

        if (!CanRefreshBoundsInPlace(refreshBounds))
        {
            RebuildGrid();
            LogRefreshProfileIfSlow(
                "RefreshGrid(rebuild-bounds)",
                profileStart,
                $"bounds={refreshBounds}");
            return true;
        }

        RefreshGridRegion(refreshBounds);
        LogRefreshProfileIfSlow(
            "RefreshGrid(local)",
            profileStart,
            $"bounds={refreshBounds}");
        return true;
    }

    public bool SetBoundsPadding(float padding, bool rebuildImmediately = false)
    {
        float sanitizedPadding = Mathf.Max(0f, padding);
        if (Mathf.Approximately(boundsPadding, sanitizedPadding))
        {
            return false;
        }

        boundsPadding = sanitizedPadding;
        if (rebuildImmediately && isActiveAndEnabled)
        {
            RebuildGrid();
        }

        return true;
    }

    public bool SetAutoDetectWorldBounds(bool enabled, bool rebuildImmediately = false)
    {
        if (autoDetectWorldBounds == enabled)
        {
            return false;
        }

        autoDetectWorldBounds = enabled;
        if (rebuildImmediately && isActiveAndEnabled)
        {
            RebuildGrid();
        }

        return true;
    }

    public bool SetWorldBounds(Bounds bounds, bool rebuildImmediately = false)
    {
        Vector2 sanitizedOrigin = new Vector2(bounds.min.x, bounds.min.y);
        Vector2 sanitizedSize = new Vector2(
            Mathf.Max(bounds.size.x, cellSize),
            Mathf.Max(bounds.size.y, cellSize));

        bool originChanged =
            !Mathf.Approximately(worldOrigin.x, sanitizedOrigin.x) ||
            !Mathf.Approximately(worldOrigin.y, sanitizedOrigin.y);
        bool sizeChanged =
            !Mathf.Approximately(worldSize.x, sanitizedSize.x) ||
            !Mathf.Approximately(worldSize.y, sanitizedSize.y);
        bool autoDetectChanged = autoDetectWorldBounds;
        if (!originChanged && !sizeChanged && !autoDetectChanged)
        {
            return false;
        }

        autoDetectWorldBounds = false;
        worldOrigin = sanitizedOrigin;
        worldSize = sanitizedSize;
        if (rebuildImmediately && isActiveAndEnabled)
        {
            RebuildGrid();
        }

        return true;
    }

    public bool ConfigureExplicitObstacleSources(
        Collider2D[] colliders,
        Tilemap[] tilemaps,
        bool rebuildImmediately = false)
    {
        Collider2D[] normalizedColliders = NormalizeObjectArray(colliders);
        Tilemap[] normalizedTilemaps = NormalizeObjectArray(tilemaps);

        bool collidersChanged = !AreReferenceArraysEqual(explicitObstacleColliders, normalizedColliders);
        bool tilemapsChanged = !AreReferenceArraysEqual(explicitObstacleTilemaps, normalizedTilemaps);
        if (!collidersChanged && !tilemapsChanged)
        {
            return false;
        }

        explicitObstacleColliders = normalizedColliders;
        explicitObstacleTilemaps = normalizedTilemaps;

        if (rebuildImmediately && isActiveAndEnabled)
        {
            RebuildGrid();
        }

        return true;
    }

    public bool ConfigureTraversalSoftPassSources(
        Collider2D[] colliders,
        Tilemap[] tilemaps,
        bool rebuildImmediately = false)
    {
        Collider2D[] normalizedColliders = NormalizeObjectArray(colliders);
        Tilemap[] normalizedTilemaps = NormalizeObjectArray(tilemaps);

        bool collidersChanged = !AreReferenceArraysEqual(explicitSoftPassObstacleColliders, normalizedColliders);
        bool tilemapsChanged = !AreReferenceArraysEqual(explicitSoftPassObstacleTilemaps, normalizedTilemaps);
        if (!collidersChanged && !tilemapsChanged)
        {
            return false;
        }

        explicitSoftPassObstacleColliders = normalizedColliders;
        explicitSoftPassObstacleTilemaps = normalizedTilemaps;

        if (rebuildImmediately && isActiveAndEnabled)
        {
            RebuildGrid();
        }

        return true;
    }

    public bool ConfigureExplicitWalkableOverrideSources(
        Collider2D[] colliders,
        Tilemap[] tilemaps,
        bool rebuildImmediately = false)
    {
        Collider2D[] normalizedColliders = NormalizeObjectArray(colliders);
        Tilemap[] normalizedTilemaps = NormalizeObjectArray(tilemaps);

        bool collidersChanged = !AreReferenceArraysEqual(explicitWalkableOverrideColliders, normalizedColliders);
        bool tilemapsChanged = !AreReferenceArraysEqual(explicitWalkableOverrideTilemaps, normalizedTilemaps);
        if (!collidersChanged && !tilemapsChanged)
        {
            return false;
        }

        explicitWalkableOverrideColliders = normalizedColliders;
        explicitWalkableOverrideTilemaps = normalizedTilemaps;

        if (rebuildImmediately && isActiveAndEnabled)
        {
            RebuildGrid();
        }

        return true;
    }

    public bool SetObstacleTilemapAutoDetection(
        bool enabled,
        string[] keywords = null,
        bool rebuildImmediately = false)
    {
        string[] normalizedKeywords = keywords == null
            ? NormalizeKeywordArray(obstacleTilemapNameKeywords)
            : NormalizeKeywordArray(keywords);

        bool autoDetectChanged = autoDetectObstacleTilemapsByName != enabled;
        bool keywordsChanged = !AreStringArraysEqual(obstacleTilemapNameKeywords, normalizedKeywords);
        if (!autoDetectChanged && !keywordsChanged)
        {
            return false;
        }

        autoDetectObstacleTilemapsByName = enabled;
        obstacleTilemapNameKeywords = normalizedKeywords;
        RefreshExplicitObstacleSources();

        if (rebuildImmediately && isActiveAndEnabled)
        {
            RebuildGrid();
        }

        return true;
    }

    public Tilemap[] GetExplicitObstacleTilemaps()
    {
        return NormalizeObjectArray(explicitObstacleTilemaps);
    }

    public Collider2D[] GetExplicitObstacleColliders()
    {
        return NormalizeObjectArray(explicitObstacleColliders);
    }

    public Tilemap[] GetExplicitWalkableOverrideTilemaps()
    {
        return NormalizeObjectArray(explicitWalkableOverrideTilemaps);
    }

    public Collider2D[] GetExplicitWalkableOverrideColliders()
    {
        return NormalizeObjectArray(explicitWalkableOverrideColliders);
    }

    public bool HasWalkableOverrideAt(Vector2 worldPos, float radius = -1f)
    {
        if (HasAnyExplicitWalkableOverrideTileAt(worldPos))
        {
            return true;
        }

        float effectiveRadius = GetWalkableOverrideSupportRadius(radius);

        if (HasExplicitWalkableOverrideTilemapHit(worldPos, effectiveRadius))
        {
            return true;
        }

        return HasExplicitWalkableOverrideColliderHit(worldPos, effectiveRadius, null);
    }

    public Bounds GetWorldBounds()
    {
        return new Bounds(worldOrigin + worldSize * 0.5f, worldSize);
    }

    public bool IsWithinWorldBounds(Vector2 worldPos)
    {
        if (worldSize.x <= 0f || worldSize.y <= 0f)
        {
            return false;
        }

        float localX = worldPos.x - worldOrigin.x;
        float localY = worldPos.y - worldOrigin.y;
        return localX >= 0f &&
               localY >= 0f &&
               localX < worldSize.x &&
               localY < worldSize.y;
    }

    public Vector2 ClampToWorldBounds(Vector2 worldPos)
    {
        if (worldSize.x <= 0f || worldSize.y <= 0f)
        {
            return worldPos;
        }

        float epsilon = Mathf.Min(cellSize * 0.1f, 0.001f);
        float maxX = worldOrigin.x + Mathf.Max(0f, worldSize.x - epsilon);
        float maxY = worldOrigin.y + Mathf.Max(0f, worldSize.y - epsilon);
        return new Vector2(
            Mathf.Clamp(worldPos.x, worldOrigin.x, maxX),
            Mathf.Clamp(worldPos.y, worldOrigin.y, maxY));
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
        double profileStart = Time.realtimeSinceStartupAsDouble;
        InvalidateRuntimeQueryCaches();
        _gridVersion++;
        // 🔥 关键修复：同步物理系统的 Transform 变化
        // 动态障碍物（如树木成长、箱子放置）修改碰撞体后，
        // Physics2D 内部缓存可能未更新，导致 OverlapCircle 检测到旧数据
        EnsurePhysicsTransformsSyncedOncePerFrame();
        
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
                bool blocked = EvaluateBlockedStateForCell(x, y);
                walkable[x, y] = !blocked;
                if (blocked) obstacleCount++;
            }
        }
        
        if (logObstacleDetection)
        {
            Debug.Log($"[NavGrid2D] 网格重建完毕: {gridW}x{gridH}={gridW*gridH} 单元，障碍物={obstacleCount}");
        }

        _lastRebuildFrame = Time.frameCount;
        LogRefreshProfileIfSlow(
            "RebuildGrid",
            profileStart,
            $"grid={gridW}x{gridH} cells={gridW * gridH}");
        _lastFullRebuildRealtime = Time.realtimeSinceStartupAsDouble;
    }

    private bool HasActiveTraversalBootstrapManager()
    {
        TraversalBlockManager2D[] managers = FindObjectsByType<TraversalBlockManager2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int index = 0; index < managers.Length; index++)
        {
            TraversalBlockManager2D manager = managers[index];
            if (manager == null
                || manager.gameObject.scene != gameObject.scene
                || !manager.gameObject.activeInHierarchy
                || !manager.enabled
                || !manager.AppliesConfigurationOnAwake)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private void DelayedRuntimeRebuildIfStillNeeded()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (_lastFullRebuildRealtime > _startupDelayedRebuildScheduledAt + 0.05d)
        {
            return;
        }

        RebuildGrid();
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
        float walkableOverrideRadius = GetWalkableOverrideSupportRadius(radius);
        bool hasWalkableOverride = HasAnyExplicitWalkableOverrideTileAt(worldPos) ||
                                   HasExplicitWalkableOverrideTilemapHit(worldPos, walkableOverrideRadius);

        int hitCount = Physics2D.OverlapCircle(worldPos, radius, _obstacleFilter, _colliderCache);
        if (hitCount == _colliderCache.Length)
        {
            System.Array.Resize(ref _colliderCache, _colliderCache.Length * 2);
            hitCount = Physics2D.OverlapCircle(worldPos, radius, _obstacleFilter, _colliderCache);
        }

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hitCollider = _colliderCache[i];
            if (ShouldIgnoreCollider(hitCollider, ignoredCollider))
            {
                continue;
            }

            if (ShouldIgnoreDynamicNavigationCollider(hitCollider))
            {
                continue;
            }

            if (MatchesExplicitWalkableOverrideCollider(hitCollider))
            {
                hasWalkableOverride = true;
            }
        }

        if (!hasWalkableOverride && HasExplicitObstacleTilemapHit(worldPos, radius))
        {
            return true;
        }

        if (!hasWalkableOverride && HasExplicitSoftPassObstacleTilemapHit(worldPos, radius))
        {
            return true;
        }

        bool hasSoftPassHit = false;
        bool shouldCheckTags = obstacleTags != null && obstacleTags.Length > 0;
        bool shouldCheckLayerMask = obstacleMask.value != 0;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hitCollider = _colliderCache[i];
            if (ShouldIgnoreCollider(hitCollider, ignoredCollider))
            {
                continue;
            }

            if (ShouldIgnoreDynamicNavigationCollider(hitCollider))
            {
                continue;
            }

            if (MatchesExplicitObstacleCollider(hitCollider))
            {
                if (!hasWalkableOverride)
                {
                    return true;
                }

                continue;
            }

            if (!hasSoftPassHit && MatchesExplicitSoftPassCollider(hitCollider))
            {
                if (!hasWalkableOverride)
                {
                    hasSoftPassHit = true;
                }

                continue;
            }

            if (shouldCheckTags)
            {
                Transform hitTransform = hitCollider.transform;
                if (!hitTransform.name.Contains("(Clone)") &&
                    !hitTransform.name.Contains("Pickup") &&
                    HasAnyTag(hitTransform, obstacleTags))
                {
                    return true;
                }
            }

            if (shouldCheckLayerMask &&
                ((1 << hitCollider.gameObject.layer) & obstacleMask.value) != 0)
            {
                return true;
            }
        }

        if (!hasWalkableOverride && hasSoftPassHit)
        {
            return true;
        }

        return false;
    }

    private bool EvaluateBlockedStateForCell(int x, int y)
    {
        Vector2 worldPos = GridToWorldCenter(x, y);

        bool blocked = IsPointBlocked(worldPos, probeRadius);
        if (!blocked && strictCornerCutting)
        {
            float offset = cellSize * 0.35f;
            blocked = IsPointBlocked(worldPos + new Vector2(offset, offset), probeRadius * 0.7f) ||
                      IsPointBlocked(worldPos + new Vector2(-offset, offset), probeRadius * 0.7f) ||
                      IsPointBlocked(worldPos + new Vector2(offset, -offset), probeRadius * 0.7f) ||
                      IsPointBlocked(worldPos + new Vector2(-offset, -offset), probeRadius * 0.7f);
        }

        return blocked;
    }

    private bool TryNormalizeRuntimeRefreshBounds(
        Bounds worldBounds,
        float extraPadding,
        out Bounds normalizedBounds)
    {
        normalizedBounds = worldBounds;

        if (worldBounds.size.x < 0f || worldBounds.size.y < 0f)
        {
            return false;
        }

        Vector3 size = normalizedBounds.size;
        size.x = Mathf.Max(size.x, cellSize);
        size.y = Mathf.Max(size.y, cellSize);
        size.z = Mathf.Max(size.z, 0f);
        normalizedBounds.size = size;

        float padding = extraPadding >= 0f
            ? extraPadding
            : Mathf.Max(probeRadius + cellSize, cellSize * 1.5f);
        normalizedBounds.Expand(new Vector3(padding * 2f, padding * 2f, 0f));
        return normalizedBounds.size.x > 0f && normalizedBounds.size.y > 0f;
    }

    private bool CanRefreshBoundsInPlace(Bounds refreshBounds)
    {
        if (walkable == null ||
            gridW <= 0 ||
            gridH <= 0 ||
            walkable.GetLength(0) != gridW ||
            walkable.GetLength(1) != gridH)
        {
            return false;
        }

        if (worldSize.x <= 0f || worldSize.y <= 0f)
        {
            return false;
        }

        float maxX = worldOrigin.x + worldSize.x;
        float maxY = worldOrigin.y + worldSize.y;
        return refreshBounds.max.x >= worldOrigin.x &&
               refreshBounds.max.y >= worldOrigin.y &&
               refreshBounds.min.x <= maxX &&
               refreshBounds.min.y <= maxY;
    }

    private void RefreshGridRegion(Bounds refreshBounds)
    {
        double profileStart = Time.realtimeSinceStartupAsDouble;
        InvalidateRuntimeQueryCaches();
        _gridVersion++;
        int minX = Mathf.Clamp(
            Mathf.FloorToInt((refreshBounds.min.x - worldOrigin.x) / cellSize),
            0,
            gridW - 1);
        int maxX = Mathf.Clamp(
            Mathf.FloorToInt((refreshBounds.max.x - worldOrigin.x) / cellSize),
            0,
            gridW - 1);
        int minY = Mathf.Clamp(
            Mathf.FloorToInt((refreshBounds.min.y - worldOrigin.y) / cellSize),
            0,
            gridH - 1);
        int maxY = Mathf.Clamp(
            Mathf.FloorToInt((refreshBounds.max.y - worldOrigin.y) / cellSize),
            0,
            gridH - 1);

        if (minX > maxX || minY > maxY)
        {
            return;
        }

        int refreshedCellCount = 0;
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                walkable[x, y] = !EvaluateBlockedStateForCell(x, y);
                refreshedCellCount++;
            }
        }

        if (logObstacleDetection)
        {
            Debug.Log(
                $"[NavGrid2D] 局部刷新完成: x={minX}-{maxX}, y={minY}-{maxY}, cells={refreshedCellCount}");
        }

        LogRefreshProfileIfSlow(
            "RefreshGridRegion",
            profileStart,
            $"cells={refreshedCellCount} bounds={refreshBounds}");
    }

    private static void LogRefreshProfileIfSlow(string stage, double startedAt, string extra = null)
    {
        if (!EnableNavGridRefreshProfiling)
        {
            return;
        }

        double elapsedMs = (Time.realtimeSinceStartupAsDouble - startedAt) * 1000d;
        if (elapsedMs < RefreshProfileLogThresholdMs)
        {
            return;
        }

        string extraSuffix = string.IsNullOrEmpty(extra) ? string.Empty : $" {extra}";
        Debug.LogWarning($"[NavGridProfile] {stage} took {elapsedMs:F2}ms{extraSuffix}");
    }

    private static void EnsurePhysicsTransformsSyncedOncePerFrame()
    {
        if (s_lastSyncedPhysicsFrame == Time.frameCount)
        {
            return;
        }

        Physics2D.SyncTransforms();
        s_lastSyncedPhysicsFrame = Time.frameCount;
    }

    private void EnsureRuntimeQueryCachesCurrentFrame()
    {
        if (_runtimeQueryCacheFrame == Time.frameCount)
        {
            return;
        }

        _runtimeQueryCacheFrame = Time.frameCount;
        _dynamicWalkableQueryCache.Clear();
    }

    private void InvalidateRuntimeQueryCaches()
    {
        _runtimeQueryCacheFrame = -1;
        _dynamicWalkableQueryCache.Clear();
        _nearestWalkableQueryCache.Clear();
        _pathQueryCache.Clear();
    }

    private bool HasExplicitWalkableOverrideTilemapHit(Vector2 worldPos, float radius)
    {
        return HasAnyTileGeometryHit(explicitWalkableOverrideTilemaps, worldPos, radius);
    }

    private float GetWalkableOverrideSupportRadius(float requestedRadius = -1f)
    {
        float fallbackRadius = requestedRadius >= 0f
            ? Mathf.Max(0.001f, requestedRadius)
            : Mathf.Max(0.001f, probeRadius);
        return Mathf.Clamp(walkableOverrideSupportRadius, 0.001f, fallbackRadius);
    }

    private bool HasAnyExplicitWalkableOverrideTileAt(Vector2 worldPos)
    {
        if (explicitWalkableOverrideTilemaps == null)
        {
            return false;
        }

        for (int i = 0; i < explicitWalkableOverrideTilemaps.Length; i++)
        {
            Tilemap tilemap = explicitWalkableOverrideTilemaps[i];
            if (tilemap == null || !tilemap.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (tilemap.HasTile(tilemap.WorldToCell(worldPos)))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasExplicitObstacleTilemapHit(Vector2 worldPos, float radius)
    {
        return HasAnyTileGeometryHit(explicitObstacleTilemaps, worldPos, radius);
    }

    private bool HasExplicitSoftPassObstacleTilemapHit(Vector2 worldPos, float radius)
    {
        return HasAnyTileGeometryHit(explicitSoftPassObstacleTilemaps, worldPos, radius);
    }

    private bool HasAnyExplicitObstacleTileAt(Vector2 worldPos)
    {
        if (explicitObstacleTilemaps == null)
        {
            return false;
        }

        for (int i = 0; i < explicitObstacleTilemaps.Length; i++)
        {
            Tilemap tilemap = explicitObstacleTilemaps[i];
            if (tilemap == null || !tilemap.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (tilemap.HasTile(tilemap.WorldToCell(worldPos)))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasAnyTileGeometryHit(Tilemap[] tilemaps, Vector2 worldPos, float radius)
    {
        if (tilemaps == null || tilemaps.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < tilemaps.Length; i++)
        {
            Tilemap tilemap = tilemaps[i];
            if (tilemap == null || !tilemap.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (TilemapHasAnyTileGeometryHit(tilemap, worldPos, radius, cellSize))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TilemapHasAnyTileGeometryHit(Tilemap tilemap, Vector2 worldPos, float radius, float cellSize)
    {
        float searchExtent = Mathf.Max(radius, cellSize) + Mathf.Max(tilemap.cellSize.x, tilemap.cellSize.y);
        Vector3Int minCell = tilemap.WorldToCell(new Vector3(worldPos.x - searchExtent, worldPos.y - searchExtent, 0f));
        Vector3Int maxCell = tilemap.WorldToCell(new Vector3(worldPos.x + searchExtent, worldPos.y + searchExtent, 0f));

        int minX = Mathf.Min(minCell.x, maxCell.x);
        int maxX = Mathf.Max(minCell.x, maxCell.x);
        int minY = Mathf.Min(minCell.y, maxCell.y);
        int maxY = Mathf.Max(minCell.y, maxCell.y);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (!tilemap.HasTile(cell))
                {
                    continue;
                }

                if (TileCellIntersectsCircle(tilemap, cell, worldPos, radius))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool TileCellIntersectsCircle(Tilemap tilemap, Vector3Int cell, Vector2 worldPos, float radius)
    {
        TileSpriteGeometry geometry = GetTileSpriteGeometry(tilemap.GetSprite(cell));
        if (geometry.HasPaths)
        {
            Vector2[][] worldPaths = TransformTilePathsToWorld(tilemap, cell, geometry.Paths);
            for (int index = 0; index < worldPaths.Length; index++)
            {
                if (PolygonIntersectsCircle(worldPaths[index], worldPos, radius))
                {
                    return true;
                }
            }

            return false;
        }

        if (geometry.HasMesh)
        {
            return SpriteMeshIntersectsCircle(tilemap, cell, geometry, worldPos, radius);
        }

        Vector2[] worldRect = BuildTileFallbackRect(tilemap, cell);
        return PolygonIntersectsCircle(worldRect, worldPos, radius);
    }

    private static TileSpriteGeometry GetTileSpriteGeometry(Sprite sprite)
    {
        if (sprite == null)
        {
            return new TileSpriteGeometry(new Vector2[0][], new Vector2[0], new ushort[0]);
        }

        int spriteId = sprite.GetInstanceID();
        if (s_tileSpriteGeometryCache.TryGetValue(spriteId, out TileSpriteGeometry cachedGeometry))
        {
            return cachedGeometry;
        }

        List<Vector2[]> paths = new List<Vector2[]>();
        int physicsShapeCount = sprite.GetPhysicsShapeCount();
        if (physicsShapeCount > 0)
        {
            List<Vector2> points = new List<Vector2>(16);
            for (int index = 0; index < physicsShapeCount; index++)
            {
                points.Clear();
                sprite.GetPhysicsShape(index, points);
                if (points.Count >= 3)
                {
                    paths.Add(points.ToArray());
                }
            }
        }

        Vector2[] meshVertices = sprite.vertices ?? new Vector2[0];
        ushort[] meshTriangles = sprite.triangles ?? new ushort[0];

        TileSpriteGeometry geometry = new TileSpriteGeometry(paths.ToArray(), meshVertices, meshTriangles);
        s_tileSpriteGeometryCache[spriteId] = geometry;
        return geometry;
    }

    private static bool SpriteMeshIntersectsCircle(
        Tilemap tilemap,
        Vector3Int cell,
        TileSpriteGeometry geometry,
        Vector2 worldPos,
        float radius)
    {
        ushort[] triangles = geometry.MeshTriangles;
        Vector2[] vertices = geometry.MeshVertices;
        for (int triangleIndex = 0; triangleIndex + 2 < triangles.Length; triangleIndex += 3)
        {
            int aIndex = triangles[triangleIndex];
            int bIndex = triangles[triangleIndex + 1];
            int cIndex = triangles[triangleIndex + 2];
            if (aIndex < 0 || aIndex >= vertices.Length ||
                bIndex < 0 || bIndex >= vertices.Length ||
                cIndex < 0 || cIndex >= vertices.Length)
            {
                continue;
            }

            Vector2 a = TransformTileLocalPointToWorld(tilemap, cell, vertices[aIndex]);
            Vector2 b = TransformTileLocalPointToWorld(tilemap, cell, vertices[bIndex]);
            Vector2 c = TransformTileLocalPointToWorld(tilemap, cell, vertices[cIndex]);
            if (TriangleIntersectsCircle(a, b, c, worldPos, radius))
            {
                return true;
            }
        }

        return false;
    }

    private static Vector2[][] TransformTilePathsToWorld(Tilemap tilemap, Vector3Int cell, Vector2[][] localPaths)
    {
        Vector2[][] worldPaths = new Vector2[localPaths.Length][];
        for (int pathIndex = 0; pathIndex < localPaths.Length; pathIndex++)
        {
            Vector2[] localPath = localPaths[pathIndex];
            Vector2[] worldPath = new Vector2[localPath.Length];
            for (int pointIndex = 0; pointIndex < localPath.Length; pointIndex++)
            {
                worldPath[pointIndex] = TransformTileLocalPointToWorld(tilemap, cell, localPath[pointIndex]);
            }

            worldPaths[pathIndex] = worldPath;
        }

        return worldPaths;
    }

    private static Vector2[] BuildTileFallbackRect(Tilemap tilemap, Vector3Int cell)
    {
        Sprite sprite = tilemap.GetSprite(cell);
        if (sprite != null)
        {
            Bounds spriteBounds = sprite.bounds;
            Vector2[] rect = new Vector2[4];
            rect[0] = TransformTileLocalPointToWorld(tilemap, cell, new Vector2(spriteBounds.min.x, spriteBounds.min.y));
            rect[1] = TransformTileLocalPointToWorld(tilemap, cell, new Vector2(spriteBounds.min.x, spriteBounds.max.y));
            rect[2] = TransformTileLocalPointToWorld(tilemap, cell, new Vector2(spriteBounds.max.x, spriteBounds.max.y));
            rect[3] = TransformTileLocalPointToWorld(tilemap, cell, new Vector2(spriteBounds.max.x, spriteBounds.min.y));
            return rect;
        }

        Vector3 worldMin = tilemap.CellToWorld(cell);
        Vector3 cellSize = tilemap.cellSize;
        return new[]
        {
            (Vector2)worldMin,
            (Vector2)(worldMin + new Vector3(0f, cellSize.y, 0f)),
            (Vector2)(worldMin + new Vector3(cellSize.x, cellSize.y, 0f)),
            (Vector2)(worldMin + new Vector3(cellSize.x, 0f, 0f))
        };
    }

    private static Vector2 TransformTileLocalPointToWorld(Tilemap tilemap, Vector3Int cell, Vector2 localPoint)
    {
        Vector3 cellCenterLocal = tilemap.transform.InverseTransformPoint(tilemap.GetCellCenterWorld(cell));
        Matrix4x4 tileTransform = tilemap.GetTransformMatrix(cell);
        Vector3 tileLocalPoint = cellCenterLocal + tileTransform.MultiplyPoint3x4(localPoint);
        return tilemap.transform.TransformPoint(tileLocalPoint);
    }

    private static bool PolygonIntersectsCircle(Vector2[] polygon, Vector2 point, float radius)
    {
        if (polygon == null || polygon.Length < 3)
        {
            return false;
        }

        if (IsPointInsidePolygon(point, polygon))
        {
            return true;
        }

        float radiusSqr = Mathf.Max(0.0001f, radius) * Mathf.Max(0.0001f, radius);
        for (int index = 0; index < polygon.Length; index++)
        {
            Vector2 start = polygon[index];
            Vector2 end = polygon[(index + 1) % polygon.Length];
            if (DistancePointToSegmentSquared(point, start, end) <= radiusSqr)
            {
                return true;
            }
        }

        return false;
    }

    private static bool TriangleIntersectsCircle(
        Vector2 a,
        Vector2 b,
        Vector2 c,
        Vector2 point,
        float radius)
    {
        if (IsPointInsideTriangle(point, a, b, c))
        {
            return true;
        }

        float radiusSqr = Mathf.Max(0.0001f, radius) * Mathf.Max(0.0001f, radius);
        return DistancePointToSegmentSquared(point, a, b) <= radiusSqr ||
               DistancePointToSegmentSquared(point, b, c) <= radiusSqr ||
               DistancePointToSegmentSquared(point, c, a) <= radiusSqr;
    }

    private static bool IsPointInsideTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
    {
        float denominator = ((b.y - c.y) * (a.x - c.x)) + ((c.x - b.x) * (a.y - c.y));
        if (Mathf.Abs(denominator) <= 0.00001f)
        {
            return false;
        }

        float alpha = (((b.y - c.y) * (point.x - c.x)) + ((c.x - b.x) * (point.y - c.y))) / denominator;
        float beta = (((c.y - a.y) * (point.x - c.x)) + ((a.x - c.x) * (point.y - c.y))) / denominator;
        float gamma = 1f - alpha - beta;
        return alpha >= 0f && beta >= 0f && gamma >= 0f;
    }

    private static bool IsPointInsidePolygon(Vector2 point, Vector2[] polygon)
    {
        bool isInside = false;
        for (int current = 0, previous = polygon.Length - 1; current < polygon.Length; previous = current++)
        {
            Vector2 currentPoint = polygon[current];
            Vector2 previousPoint = polygon[previous];
            bool intersects = ((currentPoint.y > point.y) != (previousPoint.y > point.y)) &&
                              (point.x < (previousPoint.x - currentPoint.x) * (point.y - currentPoint.y) /
                               Mathf.Max(previousPoint.y - currentPoint.y, 0.00001f) + currentPoint.x);
            if (intersects)
            {
                isInside = !isInside;
            }
        }

        return isInside;
    }

    private static float DistancePointToSegmentSquared(Vector2 point, Vector2 start, Vector2 end)
    {
        Vector2 segment = end - start;
        float segmentLengthSqr = segment.sqrMagnitude;
        if (segmentLengthSqr <= 0.000001f)
        {
            return (point - start).sqrMagnitude;
        }

        float t = Mathf.Clamp01(Vector2.Dot(point - start, segment) / segmentLengthSqr);
        Vector2 closest = start + segment * t;
        return (point - closest).sqrMagnitude;
    }

    private bool HasExplicitObstacleHit(int hitCount, Collider2D ignoredCollider)
    {
        if (explicitObstacleColliders == null || explicitObstacleColliders.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hitCollider = _colliderCache[i];
            if (ShouldIgnoreCollider(hitCollider, ignoredCollider))
            {
                continue;
            }

            if (ShouldIgnoreDynamicNavigationCollider(hitCollider))
            {
                continue;
            }

            if (MatchesExplicitObstacleCollider(hitCollider))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasExplicitSoftPassObstacleHit(int hitCount, Collider2D ignoredCollider)
    {
        if (explicitSoftPassObstacleColliders == null || explicitSoftPassObstacleColliders.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hitCollider = _colliderCache[i];
            if (ShouldIgnoreCollider(hitCollider, ignoredCollider))
            {
                continue;
            }

            if (ShouldIgnoreDynamicNavigationCollider(hitCollider))
            {
                continue;
            }

            for (int obstacleIndex = 0; obstacleIndex < explicitSoftPassObstacleColliders.Length; obstacleIndex++)
            {
                if (explicitSoftPassObstacleColliders[obstacleIndex] == hitCollider)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool MatchesExplicitSoftPassCollider(Collider2D hitCollider)
    {
        if (hitCollider == null ||
            explicitSoftPassObstacleColliders == null ||
            explicitSoftPassObstacleColliders.Length == 0)
        {
            return false;
        }

        for (int index = 0; index < explicitSoftPassObstacleColliders.Length; index++)
        {
            if (explicitSoftPassObstacleColliders[index] == hitCollider)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasExplicitWalkableOverrideColliderHit(Vector2 worldPos, float radius, Collider2D ignoredCollider)
    {
        if (explicitWalkableOverrideColliders == null || explicitWalkableOverrideColliders.Length == 0)
        {
            return false;
        }

        int hitCount = Physics2D.OverlapCircle(worldPos, radius, _obstacleFilter, _walkableOverrideColliderCache);
        if (hitCount == _walkableOverrideColliderCache.Length)
        {
            System.Array.Resize(ref _walkableOverrideColliderCache, _walkableOverrideColliderCache.Length * 2);
            hitCount = Physics2D.OverlapCircle(worldPos, radius, _obstacleFilter, _walkableOverrideColliderCache);
        }

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hitCollider = _walkableOverrideColliderCache[i];
            if (ShouldIgnoreCollider(hitCollider, ignoredCollider))
            {
                continue;
            }

            if (ShouldIgnoreDynamicNavigationCollider(hitCollider))
            {
                continue;
            }

            if (MatchesExplicitWalkableOverrideCollider(hitCollider))
            {
                return true;
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

        PathCacheKey cacheKey = new PathCacheKey(sx, sy, tx, ty, _gridVersion);
        if (_pathQueryCache.TryGetValue(cacheKey, out Vector2[] cachedPath))
        {
            AppendCachedPath(cachedPath, outPath);
            return cachedPath.Length > 0;
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

        if (found == null)
        {
            CachePathQuery(cacheKey, outPath);
            return false;
        }

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

        CachePathQuery(cacheKey, outPath);
        return true;
    }

    private void AppendCachedPath(Vector2[] cachedPath, List<Vector2> outPath)
    {
        if (cachedPath == null || cachedPath.Length == 0)
        {
            return;
        }

        for (int index = 0; index < cachedPath.Length; index++)
        {
            outPath.Add(cachedPath[index]);
        }
    }

    private void CachePathQuery(PathCacheKey cacheKey, List<Vector2> pathPoints)
    {
        if (_pathQueryCache.Count >= 256)
        {
            _pathQueryCache.Clear();
        }

        _pathQueryCache[cacheKey] = pathPoints.Count == 0
            ? System.Array.Empty<Vector2>()
            : pathPoints.ToArray();
    }

    /// <summary>
    /// 检查世界坐标是否可走（公共接口）
    /// </summary>
    public bool IsWalkable(Vector2 worldPos, Collider2D ignoredCollider = null)
    {
        return IsWalkable(worldPos, probeRadius, ignoredCollider);
    }

    public bool IsWalkable(Vector2 worldPos, float queryRadius, Collider2D ignoredCollider = null)
    {
        float effectiveRadius = queryRadius > 0f ? queryRadius : probeRadius;
        if (!IsWithinWorldBounds(worldPos))
        {
            return false;
        }

        if (ignoredCollider != null || !Mathf.Approximately(effectiveRadius, probeRadius))
        {
            EnsureRuntimeQueryCachesCurrentFrame();
            DynamicWalkableQueryKey cacheKey = new DynamicWalkableQueryKey(
                worldPos,
                effectiveRadius,
                ignoredCollider != null ? ignoredCollider.GetInstanceID() : 0);
            if (_dynamicWalkableQueryCache.TryGetValue(cacheKey, out bool cachedWalkable))
            {
                return cachedWalkable;
            }

            EnsurePhysicsTransformsSyncedOncePerFrame();
            bool isWalkable = !IsPointBlocked(worldPos, effectiveRadius, ignoredCollider);
            _dynamicWalkableQueryCache[cacheKey] = isWalkable;
            return isWalkable;
        }

        if (!WorldToGrid(worldPos, out int gx, out int gy))
            return false;

        return IsWalkable(gx, gy);
    }

    public bool TryFindNearestWalkable(Vector2 world, out Vector2 nearestWorld, Collider2D ignoredCollider = null)
    {
        nearestWorld = world;
        if (!WorldToGrid(world, out int gx, out int gy)) return false;

        if (ignoredCollider != null)
        {
            EnsureRuntimeQueryCachesCurrentFrame();
            NearestWalkableQueryKey cacheKey = new NearestWalkableQueryKey(gx, gy, ignoredCollider.GetInstanceID());
            if (_nearestWalkableQueryCache.TryGetValue(cacheKey, out NearestWalkableQueryResult cachedResult))
            {
                float cachedAge = Time.time - cachedResult.CachedAtTime;
                if (cachedAge >= 0f && cachedAge <= NearestWalkableCacheSeconds)
                {
                    nearestWorld = cachedResult.NearestWorld;
                    return cachedResult.Success;
                }
            }

            EnsurePhysicsTransformsSyncedOncePerFrame();
            bool success = false;
            Vector2 cachedNearestWorld = world;
            if (FindNearestWalkableImproved(gx, gy, 30, out int cachedNx, out int cachedNy, ignoredCollider))
            {
                cachedNearestWorld = GridToWorldCenter(cachedNx, cachedNy);
                success = true;
            }

            if (_nearestWalkableQueryCache.Count >= 256)
            {
                _nearestWalkableQueryCache.Clear();
            }

            _nearestWalkableQueryCache[cacheKey] = new NearestWalkableQueryResult(success, cachedNearestWorld, Time.time);
            nearestWorld = cachedNearestWorld;
            return success;
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

        if (ignoredCollider != null &&
            FindNearestWalkableWithIgnoredCollider(gx, gy, maxRange, ignoredCollider, out nx, out ny))
        {
            return true;
        }
        
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

    private bool FindNearestWalkableWithIgnoredCollider(
        int gx,
        int gy,
        int maxRange,
        Collider2D ignoredCollider,
        out int nx,
        out int ny)
    {
        nx = gx;
        ny = gy;
        int candidateCount = 0;
        float bestGridDistSq = float.MaxValue;

        for (int r = 1; r <= maxRange; r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    if (System.Math.Abs(dx) != r && System.Math.Abs(dy) != r)
                    {
                        continue;
                    }

                    int x = gx + dx;
                    int y = gy + dy;
                    if (!InBounds(x, y) || !IsWalkable(x, y))
                    {
                        continue;
                    }

                    float distSq = dx * dx + dy * dy;
                    int priority = GetDirectionPriority(dx, dy);
                    InsertNearestWalkableVerificationCandidate(x, y, distSq, priority, ref candidateCount);
                    bestGridDistSq = Mathf.Min(bestGridDistSq, distSq);
                }
            }

            if (candidateCount > 0 && r > Mathf.Sqrt(bestGridDistSq) + 1f)
            {
                break;
            }
        }

        for (int index = 0; index < candidateCount; index++)
        {
            Vector2Int candidate = _nearestWalkableVerificationCandidates[index];
            if (!IsPointBlocked(GridToWorldCenter(candidate.x, candidate.y), probeRadius, ignoredCollider))
            {
                nx = candidate.x;
                ny = candidate.y;
                return true;
            }
        }

        return false;
    }

    private void InsertNearestWalkableVerificationCandidate(
        int x,
        int y,
        float distSq,
        int priority,
        ref int candidateCount)
    {
        int limit = _nearestWalkableVerificationCandidates.Length;
        int usedCount = Mathf.Min(candidateCount, limit);
        if (usedCount >= limit &&
            !IsCandidateBetter(
                distSq,
                priority,
                _nearestWalkableVerificationDistances[limit - 1],
                _nearestWalkableVerificationPriorities[limit - 1]))
        {
            return;
        }

        int insertIndex = usedCount;
        while (insertIndex > 0 &&
               IsCandidateBetter(
                   distSq,
                   priority,
                   _nearestWalkableVerificationDistances[insertIndex - 1],
                   _nearestWalkableVerificationPriorities[insertIndex - 1]))
        {
            if (insertIndex < limit)
            {
                _nearestWalkableVerificationCandidates[insertIndex] = _nearestWalkableVerificationCandidates[insertIndex - 1];
                _nearestWalkableVerificationDistances[insertIndex] = _nearestWalkableVerificationDistances[insertIndex - 1];
                _nearestWalkableVerificationPriorities[insertIndex] = _nearestWalkableVerificationPriorities[insertIndex - 1];
            }

            insertIndex--;
        }

        if (insertIndex >= limit)
        {
            return;
        }

        _nearestWalkableVerificationCandidates[insertIndex] = new Vector2Int(x, y);
        _nearestWalkableVerificationDistances[insertIndex] = distSq;
        _nearestWalkableVerificationPriorities[insertIndex] = priority;
        candidateCount = Mathf.Min(candidateCount + 1, limit);
    }

    private static bool IsCandidateBetter(float distSq, int priority, float otherDistSq, int otherPriority)
    {
        if (distSq < otherDistSq - 0.01f)
        {
            return true;
        }

        return Mathf.Abs(distSq - otherDistSq) < 0.01f && priority < otherPriority;
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

        if (!IsWalkable(x, y))
        {
            return false;
        }

        if (ignoredCollider == null)
        {
            return true;
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

        int colliderId = hitCollider.GetInstanceID();
        if (s_dynamicNavigationColliderIgnoreCache.TryGetValue(colliderId, out bool cachedResult))
        {
            return cachedResult;
        }

        Transform current = hitCollider.transform;
        while (current != null)
        {
            s_navigationBehaviourBuffer.Clear();
            current.GetComponents<MonoBehaviour>(s_navigationBehaviourBuffer);
            for (int index = 0; index < s_navigationBehaviourBuffer.Count; index++)
            {
                if (s_navigationBehaviourBuffer[index] is INavigationUnit navigationUnit &&
                    navigationUnit.GetUnitType() != NavigationUnitType.StaticObstacle)
                {
                    CacheDynamicNavigationColliderIgnore(colliderId, true);
                    s_navigationBehaviourBuffer.Clear();
                    return true;
                }
            }

            if (current == current.root)
            {
                break;
            }

            current = current.parent;
        }

        CacheDynamicNavigationColliderIgnore(colliderId, false);
        s_navigationBehaviourBuffer.Clear();
        return false;
    }

    private static void CacheDynamicNavigationColliderIgnore(int colliderId, bool shouldIgnore)
    {
        if (s_dynamicNavigationColliderIgnoreCache.Count >= 4096)
        {
            s_dynamicNavigationColliderIgnoreCache.Clear();
        }

        s_dynamicNavigationColliderIgnoreCache[colliderId] = shouldIgnore;
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

    private bool MatchesExplicitObstacleCollider(Collider2D hitCollider)
    {
        if (hitCollider == null || explicitObstacleColliders == null)
        {
            return false;
        }

        for (int i = 0; i < explicitObstacleColliders.Length; i++)
        {
            Collider2D explicitCollider = explicitObstacleColliders[i];
            if (explicitCollider == null)
            {
                continue;
            }

            if (hitCollider == explicitCollider)
            {
                return true;
            }

            if (explicitCollider.attachedRigidbody != null &&
                hitCollider.attachedRigidbody != null &&
                explicitCollider.attachedRigidbody == hitCollider.attachedRigidbody)
            {
                return true;
            }

            if (hitCollider.transform.IsChildOf(explicitCollider.transform) ||
                explicitCollider.transform.IsChildOf(hitCollider.transform))
            {
                return true;
            }
        }

        return false;
    }

    private bool MatchesExplicitWalkableOverrideCollider(Collider2D hitCollider)
    {
        if (hitCollider == null || explicitWalkableOverrideColliders == null)
        {
            return false;
        }

        for (int i = 0; i < explicitWalkableOverrideColliders.Length; i++)
        {
            Collider2D explicitCollider = explicitWalkableOverrideColliders[i];
            if (explicitCollider == null)
            {
                continue;
            }

            if (hitCollider == explicitCollider)
            {
                return true;
            }

            if (explicitCollider.attachedRigidbody != null &&
                hitCollider.attachedRigidbody != null &&
                explicitCollider.attachedRigidbody == hitCollider.attachedRigidbody)
            {
                return true;
            }

            if (hitCollider.transform.IsChildOf(explicitCollider.transform) ||
                explicitCollider.transform.IsChildOf(hitCollider.transform))
            {
                return true;
            }
        }

        return false;
    }

    private void RefreshExplicitObstacleSources()
    {
        List<Tilemap> tilemaps = new List<Tilemap>();
        if (explicitObstacleTilemaps != null)
        {
            for (int i = 0; i < explicitObstacleTilemaps.Length; i++)
            {
                AddUniqueObject(tilemaps, explicitObstacleTilemaps[i]);
            }
        }

        if (autoDetectObstacleTilemapsByName)
        {
            Tilemap[] discoveredTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
            for (int i = 0; i < discoveredTilemaps.Length; i++)
            {
                Tilemap tilemap = discoveredTilemaps[i];
                if (tilemap == null || !IsInWorldLayers(tilemap.transform))
                {
                    continue;
                }

                if (IsExplicitObstacleTilemap(tilemap, obstacleTilemapNameKeywords))
                {
                    AddUniqueObject(tilemaps, tilemap);
                }
            }
        }

        explicitObstacleTilemaps = tilemaps.ToArray();

        List<Collider2D> colliders = new List<Collider2D>();
        if (explicitObstacleColliders != null)
        {
            for (int i = 0; i < explicitObstacleColliders.Length; i++)
            {
                AddUniqueObject(colliders, explicitObstacleColliders[i]);
            }
        }

        for (int i = 0; i < explicitObstacleTilemaps.Length; i++)
        {
            Tilemap tilemap = explicitObstacleTilemaps[i];
            if (tilemap == null)
            {
                continue;
            }

            AddUniqueObject(colliders, tilemap.GetComponent<Collider2D>());
        }

        explicitObstacleColliders = colliders.ToArray();

        List<Tilemap> walkableOverrideTilemaps = new List<Tilemap>();
        if (explicitWalkableOverrideTilemaps != null)
        {
            for (int i = 0; i < explicitWalkableOverrideTilemaps.Length; i++)
            {
                AddUniqueObject(walkableOverrideTilemaps, explicitWalkableOverrideTilemaps[i]);
            }
        }

        explicitWalkableOverrideTilemaps = walkableOverrideTilemaps.ToArray();

        List<Collider2D> walkableOverrideColliders = new List<Collider2D>();
        if (explicitWalkableOverrideColliders != null)
        {
            for (int i = 0; i < explicitWalkableOverrideColliders.Length; i++)
            {
                AddUniqueObject(walkableOverrideColliders, explicitWalkableOverrideColliders[i]);
            }
        }

        for (int i = 0; i < explicitWalkableOverrideTilemaps.Length; i++)
        {
            Tilemap tilemap = explicitWalkableOverrideTilemaps[i];
            if (tilemap == null)
            {
                continue;
            }

            AddUniqueObject(walkableOverrideColliders, tilemap.GetComponent<Collider2D>());
        }

        explicitWalkableOverrideColliders = walkableOverrideColliders.ToArray();
    }

    private static bool IsExplicitObstacleTilemap(Tilemap tilemap, string[] keywords)
    {
        if (tilemap == null || !TilemapHasAnyTiles(tilemap))
        {
            return false;
        }

        if (keywords == null || keywords.Length == 0)
        {
            return false;
        }

        string lowered = tilemap.name.ToLowerInvariant();
        for (int i = 0; i < keywords.Length; i++)
        {
            string keyword = keywords[i];
            if (string.IsNullOrWhiteSpace(keyword))
            {
                continue;
            }

            if (lowered.Contains(keyword))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TilemapHasAnyTiles(Tilemap tilemap)
    {
        if (tilemap == null)
        {
            return false;
        }

        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int position in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(position))
            {
                return true;
            }
        }

        return false;
    }

    private static T[] NormalizeObjectArray<T>(T[] source) where T : Object
    {
        List<T> results = new List<T>();
        if (source == null)
        {
            return results.ToArray();
        }

        for (int i = 0; i < source.Length; i++)
        {
            AddUniqueObject(results, source[i]);
        }

        return results.ToArray();
    }

    private static string[] NormalizeKeywordArray(string[] source)
    {
        List<string> results = new List<string>();
        if (source == null)
        {
            return results.ToArray();
        }

        for (int i = 0; i < source.Length; i++)
        {
            string keyword = source[i];
            if (string.IsNullOrWhiteSpace(keyword))
            {
                continue;
            }

            string normalized = keyword.Trim().ToLowerInvariant();
            if (results.Contains(normalized))
            {
                continue;
            }

            results.Add(normalized);
        }

        return results.ToArray();
    }

    private static void AddUniqueObject<T>(List<T> list, T item) where T : Object
    {
        if (item == null)
        {
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == item)
            {
                return;
            }
        }

        list.Add(item);
    }

    private static bool AreReferenceArraysEqual<T>(T[] left, T[] right) where T : Object
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left == null || right == null || left.Length != right.Length)
        {
            return false;
        }

        for (int i = 0; i < left.Length; i++)
        {
            if (left[i] != right[i])
            {
                return false;
            }
        }

        return true;
    }

    private static bool AreStringArraysEqual(string[] left, string[] right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left == null || right == null || left.Length != right.Length)
        {
            return false;
        }

        for (int i = 0; i < left.Length; i++)
        {
            if (!string.Equals(left[i], right[i], System.StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
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
