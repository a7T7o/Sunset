using System;
using System.Collections.Generic;
using FarmGame.Data.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

/// <summary>
/// 云朵阴影管理器
/// 管理云朵阴影的生成、移动、销毁
/// 
/// 核心机制：
/// 1. 云朵从移动方向的起始边缘生成
/// 2. 云朵移动到终点边缘后销毁并回收到对象池
/// 3. 云朵之间保持最小间距，避免重叠
/// 4. 严格控制云朵数量，不超过 maxClouds
/// </summary>
[ExecuteAlways]
public class CloudShadowManager : MonoBehaviour
{
    public enum WeatherState { Sunny, PartlyCloudy, Overcast, Rain, Snow }

    public enum AreaSizeMode
    {
        Manual,         // 手动设置区域大小
        FromNavGrid,    // 从 NavGrid2D 获取
        AutoDetect      // 自动检测 Tilemap 边界
    }

    [Header("Enable")] 
    [SerializeField] private bool enableCloudShadows = true;

    [Header("Appearance")]
    [Range(0f, 1f)]
    [SerializeField] private float intensity = 0.3f;
    [Range(0f, 1f)]
    [SerializeField] private float density = 0.6f;
    [Tooltip("云朵缩放范围 - 增大差异让云朵大小更随机")]
    [SerializeField] private Vector2 scaleRange = new Vector2(0.4f, 2.5f);
    [Tooltip("Sprites used for cloud shadows (grayscale/alpha). If empty, manager remains idle.")]
    [SerializeField] private Sprite[] cloudSprites;
    [Tooltip("Optional material (Multiply recommended). If null, Sprite default material is used.")]
    [SerializeField] private Material cloudMaterial;

    [Header("Movement")]
    [SerializeField] private Vector2 direction = new Vector2(1f, 0f);
    [SerializeField, Range(0f, 5f)] private float speed = 0.4f;

    [Header("Area")]
    [Tooltip("区域大小获取模式")]
    [SerializeField] private AreaSizeMode areaSizeMode = AreaSizeMode.Manual;
    [Tooltip("Cloud simulation area size centered at manager's transform.")]
    [SerializeField] private Vector2 areaSize = new Vector2(40f, 24f);
    [Tooltip("自动检测时使用的世界层级名称")]
    [SerializeField] private string[] worldLayerNames = new string[] { "LAYER 1", "LAYER 2", "LAYER 3" };
    [Tooltip("边界扩展（留出余量）")]
    [SerializeField] private float boundsPadding = 5f;

    [Header("Sorting")]
    [SerializeField] private string sortingLayerName = "CloudShadow";
    [SerializeField] private int sortingOrder = 0;

    [Header("Anti-Overlap")]
    [Tooltip("云朵之间的最小间距（中心点距离）")]
    [SerializeField] private float minCloudSpacing = 5f;
    [Tooltip("生成时尝试找到不重叠位置的最大次数")]
    [SerializeField] private int maxSpawnAttempts = 15;
    [Tooltip("生成冷却时间（秒），避免短时间内生成过多云朵")]
    [SerializeField] private float spawnCooldown = 0.5f;

    [Header("Weather Gate (manual)")]
    [SerializeField] private bool useWeatherGate = false;
    [SerializeField] private WeatherState currentWeather = WeatherState.Sunny;
    [SerializeField] private bool enableInSunny = true;
    [SerializeField] private bool enableInPartlyCloudy = true;
    [SerializeField] private bool enableInOvercast = false;
    [SerializeField] private bool enableInRain = false;
    [SerializeField] private bool enableInSnow = false;

    [Header("Seed & Preview")]
    [SerializeField] private int seed = 12345;
    [SerializeField] private bool randomizeOnStart = true;
    [SerializeField] private bool previewInEditor = false;

    [Header("Limits")]
    [SerializeField, Range(1, 20)] private int maxClouds = 6;

    [Header("Debug")]
    [SerializeField] private bool enableDebug = false;

    private const float EntryBandMinDepth = 3f;
    private const float EntryBandMaxDepth = 12f;
    private const float SpawnEdgeOffset = 0.5f;
    private const float ExitDespawnMargin = 1f;
    private const string RuntimeCloudObjectName = "CloudShadow";
    private const HideFlags RuntimeCloudHideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;

    private System.Random rng;
    private float lastSpawnTime = -999f;
    private float lastSpawnFailureLogTime = -999f;

    private struct Cloud
    {
        public Transform transform;
        public SpriteRenderer sr;
        public float halfWidth;
        public float halfHeight;
        public int id; // 用于调试
        public int spriteIndex;
        public float scale;
        public Vector3 lastPosition;
        public float lifetime;
        public float stuckTime;
    }

    private struct CloudRuntimeState
    {
        public Vector3 position;
        public int spriteIndex;
        public float scale;
        public int id;
        public float lifetime;
        public float stuckTime;
    }

    private sealed class SceneRuntimeState
    {
        public int seed;
        public int cloudIdCounter;
        public float lastSpawnTime;
        public float lastSpawnFailureLogTime;
        public int consecutiveSpawnFailures;
        public List<CloudRuntimeState> clouds = new List<CloudRuntimeState>();
    }

    private readonly List<Cloud> active = new List<Cloud>();
    private readonly Stack<GameObject> pool = new Stack<GameObject>();
    private static readonly Dictionary<string, SceneRuntimeState> runtimeStateByManager = new Dictionary<string, SceneRuntimeState>();
    private bool initialized;
    private int cloudIdCounter = 0;
    private int consecutiveSpawnFailures = 0;
    private const float CloudLifetimeSlack = 1.6f;
    private const float CloudMinExpectedTravelPerSecond = 0.04f;
    private const float CloudStuckTimeout = 1.35f;

    void OnEnable()
    {
        NormalizeResidualCloudChildren();
        EnsureRng();
        if (Application.isPlaying)
        {
            InitializeIfNeeded();
        }
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.update -= EditorUpdate;
        UnityEditor.EditorApplication.update += EditorUpdate;
        #endif
    }

void OnDisable()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.update -= EditorUpdate;
        #endif
        SaveRuntimeState();
        ClearCloudsForInactiveState();
    }

    void Start()
    {
        if (Application.isPlaying)
        {
            InitializeIfNeeded();
            
            // 订阅天气系统事件
            if (useWeatherGate)
            {
                WeatherSystem.OnWeatherChanged += OnWeatherChanged;
                
                // 初始化天气状态
                if (WeatherSystem.Instance != null)
                {
                    UpdateWeatherState(WeatherSystem.Instance.GetCurrentWeather());
                }
            }
        }
    }

    void OnDestroy()
    {
        if (useWeatherGate)
        {
            WeatherSystem.OnWeatherChanged -= OnWeatherChanged;
        }
    }
    
    /// <summary>
    /// 天气变化回调
    /// </summary>
    private void OnWeatherChanged(WeatherSystem.Weather weather)
    {
        UpdateWeatherState(weather);
    }
    
    /// <summary>
    /// 根据天气更新云影状态
    /// </summary>
    private void UpdateWeatherState(WeatherSystem.Weather weather)
    {
        // 将 WeatherSystem.Weather 映射到 CloudShadowManager.WeatherState
        WeatherState state = weather switch
        {
            WeatherSystem.Weather.Sunny => WeatherState.Sunny,
            WeatherSystem.Weather.Rainy => WeatherState.Rain,
            WeatherSystem.Weather.Withering => WeatherState.Overcast, // 枯萎天视为阴天
            _ => WeatherState.Sunny
        };

        SetWeatherState(state);

        Debug.Log($"<color=cyan>[CloudShadowManager] 天气变化: {weather} → {state}, 云影启用: {IsWeatherAllowed(state)}</color>");
    }

    void Update()
    {
        if (!Application.isPlaying) return;
        SimulateStep(Time.deltaTime);
    }

    public void SimulateStep(float dt)
    {
        if (ShouldClearCloudsForCurrentState())
        {
            ClearCloudsForInactiveState();
            return;
        }

        InitializeIfNeeded();

        int target = GetTargetCloudCount();

        Vector2 dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : new Vector2(1f, 0f);
        float spd = Mathf.Max(0f, speed);

        Rect area = GetWorldAreaRect();
        
        // 第一步：清理无效云朵和超出边界的云朵
        CleanupInvalidClouds(area, dir);
        
        // 第二步：移动所有云朵
        MoveClouds(area, dir, spd, dt);

        // 移动后再次清理，避免高速时云朵在边界外多挂一帧。
        CleanupInvalidClouds(area, dir);
        
        // 第三步：如果数量超过目标，移除多余的
        while (active.Count > target)
        {
            DespawnLast();
        }

        // 第四步：如果数量不足则尝试补云；高负载时允许一次补多朵，并在卡住时强制腾位。
        float currentTime = Time.time;
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            currentTime = (float)UnityEditor.EditorApplication.timeSinceStartup;
        }
        #endif
        TryFillCloudPopulation(area, dir, target, currentTime);

        if (enableDebug)
        {
            Debug.Log($"[CloudShadow] Active: {active.Count}/{target}, Pool: {pool.Count}");
        }
    }

    private int GetTargetCloudCount()
    {
        return Mathf.Clamp(Mathf.RoundToInt(density * maxClouds), 0, maxClouds);
    }

    private void TryFillCloudPopulation(Rect area, Vector2 dir, int target, float currentTime)
    {
        if (target <= 0 || active.Count >= target)
        {
            consecutiveSpawnFailures = 0;
            return;
        }

        if ((currentTime - lastSpawnTime) < spawnCooldown)
        {
            return;
        }

        int spawnBudget = Mathf.Clamp(target - active.Count, 1, 4);
        int spawnedThisStep = 0;

        for (int attempt = 0; attempt < spawnBudget && active.Count < target; attempt++)
        {
            if (TrySpawnOneAtEdge(area, dir))
            {
                spawnedThisStep++;
            }
        }

        if (spawnedThisStep > 0)
        {
            consecutiveSpawnFailures = 0;
            lastSpawnTime = currentTime;
            return;
        }

        consecutiveSpawnFailures++;
        TryLogSpawnStall(area, target, currentTime);

        if (consecutiveSpawnFailures < 3 || active.Count == 0)
        {
            return;
        }
    }

    private bool ShouldClearCloudsForCurrentState()
    {
        if (!enableCloudShadows) return true;
        if (useWeatherGate && !IsWeatherAllowed(currentWeather)) return true;
        if (!HasConfiguredCloudSprite()) return true;
        return density <= 0f || maxClouds <= 0;
    }

    private bool HasConfiguredCloudSprite()
    {
        if (cloudSprites == null || cloudSprites.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < cloudSprites.Length; i++)
        {
            if (cloudSprites[i] != null)
            {
                return true;
            }
        }

        return false;
    }

private void ClearCloudsForInactiveState()
    {
        if (!Application.isPlaying && this == null)
        {
            initialized = false;
            cloudIdCounter = 0;
            lastSpawnTime = -999f;
            lastSpawnFailureLogTime = -999f;
            consecutiveSpawnFailures = 0;
            active.Clear();
            pool.Clear();
            return;
        }

        if (Application.isPlaying)
        {
            DespawnAll();
            NormalizeResidualCloudChildren();
        }
        #if UNITY_EDITOR
        else
        {
            DestroyEditorCloudObjects();
        }
        #endif

        initialized = false;
        cloudIdCounter = 0;
        lastSpawnTime = -999f;
        lastSpawnFailureLogTime = -999f;
        consecutiveSpawnFailures = 0;
    }

    #if UNITY_EDITOR
private void DestroyEditorCloudObjects()
    {
        if (this == null)
        {
            active.Clear();
            pool.Clear();
            return;
        }

        var toDestroy = new HashSet<GameObject>();

        foreach (var cloud in active)
        {
            if (cloud.transform != null)
            {
                toDestroy.Add(cloud.transform.gameObject);
            }
        }

        foreach (var pooledObject in pool)
        {
            if (pooledObject != null)
            {
                toDestroy.Add(pooledObject);
            }
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child != null && child.name == "CloudShadow")
            {
                toDestroy.Add(child.gameObject);
            }
        }

        foreach (var go in toDestroy)
        {
            if (go != null)
            {
                DestroyImmediate(go);
            }
        }

        active.Clear();
        pool.Clear();
    }
    #endif
    
    /// <summary>
    /// 清理无效和超出边界的云朵
    /// </summary>
    private void CleanupInvalidClouds(Rect area, Vector2 dir)
    {
        for (int i = active.Count - 1; i >= 0; i--)
        {
            Cloud c = active[i];
            
            // 检查 Transform 是否有效
            if (c.transform == null)
            {
                active.RemoveAt(i);
                continue;
            }
            
            Vector3 p = c.transform.position;
            
            // 计算销毁边界（云朵完全离开区域后销毁）
            bool outOfBounds = false;
            
            if (dir.x > 0f && p.x > area.xMax + c.halfWidth + ExitDespawnMargin) outOfBounds = true;
            if (dir.x < 0f && p.x < area.xMin - c.halfWidth - ExitDespawnMargin) outOfBounds = true;
            if (dir.y > 0f && p.y > area.yMax + c.halfHeight + ExitDespawnMargin) outOfBounds = true;
            if (dir.y < 0f && p.y < area.yMin - c.halfHeight - ExitDespawnMargin) outOfBounds = true;
            
            if (outOfBounds)
            {
                if (enableDebug)
                {
                    Debug.Log($"[CloudShadow] 销毁云朵 #{c.id} (超出边界)");
                }
                DespawnAt(i);
            }
        }
    }
    
    /// <summary>
    /// 移动所有云朵
    /// </summary>
    private void MoveClouds(Rect area, Vector2 dir, float spd, float dt)
    {
        for (int i = 0; i < active.Count; i++)
        {
            Cloud c = active[i];
            if (c.transform == null) continue;
            
            Vector3 previousPosition = c.transform.position;
            Vector3 p = previousPosition;
            p += (Vector3)(dir * spd * dt);
            c.transform.position = p;
            c.lifetime += Mathf.Max(0f, dt);

            float movedDistance = Vector3.Distance(previousPosition, p);
            float minimumExpectedMove = Mathf.Max(0.002f, CloudMinExpectedTravelPerSecond * Mathf.Max(0.1f, dt));
            if (spd > 0.01f && movedDistance <= minimumExpectedMove)
            {
                c.stuckTime += Mathf.Max(0f, dt);
            }
            else
            {
                c.stuckTime = 0f;
            }

            c.lastPosition = p;

            if (ShouldForceRecycleCloud(c, area, dir, spd))
            {
                if (enableDebug)
                {
                    Debug.Log($"[CloudShadow] 强制回收云朵 #{c.id} (lifetime={c.lifetime:F2}, stuck={c.stuckTime:F2})");
                }

                DespawnAt(i);
                i--;
                continue;
            }

            active[i] = c;
        }
    }

    private bool ShouldForceRecycleCloud(Cloud cloud, Rect area, Vector2 dir, float spd)
    {
        if (cloud.transform == null)
        {
            return true;
        }

        if (spd <= 0.01f)
        {
            return false;
        }

        float traversalDistance = GetTraversalDistance(area, dir, cloud.halfWidth, cloud.halfHeight);
        float expectedLifetime = traversalDistance <= 0.1f
            ? 3f
            : Mathf.Max(3f, traversalDistance / Mathf.Max(0.1f, spd));

        if (cloud.lifetime > expectedLifetime * CloudLifetimeSlack)
        {
            return true;
        }

        if (cloud.stuckTime > CloudStuckTimeout)
        {
            return true;
        }

        return false;
    }

    private void InitializeIfNeeded()
    {
        if (initialized) return;
        initialized = true;
        NormalizeResidualCloudChildren();
        
        // 根据模式自动获取区域大小
        UpdateAreaSizeFromMode();

        if (Application.isPlaying && TryRestoreRuntimeState())
        {
            return;
        }

        if (randomizeOnStart) seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        EnsureRng();

        // 初始化时沿完整穿场路径分布云朵
        RebuildClouds();
    }
    
    /// <summary>
    /// 根据当前模式更新区域大小
    /// </summary>
    public void UpdateAreaSizeFromMode()
    {
        switch (areaSizeMode)
        {
            case AreaSizeMode.FromNavGrid:
                UpdateAreaSizeFromNavGrid();
                break;
            case AreaSizeMode.AutoDetect:
                DetectWorldBounds();
                break;
            // Manual 模式不做任何处理
        }
    }
    
    /// <summary>
    /// 从 NavGrid2D 获取区域大小
    /// </summary>
    private void UpdateAreaSizeFromNavGrid()
    {
        NavGrid2D navGrid = FindFirstObjectByType<NavGrid2D>();
        if (navGrid != null)
        {
            // 使用反射获取 NavGrid2D 的 worldSize（因为是私有字段）
            var worldSizeField = typeof(NavGrid2D).GetField("worldSize", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var worldOriginField = typeof(NavGrid2D).GetField("worldOrigin", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (worldSizeField != null && worldOriginField != null)
            {
                Vector2 navWorldSize = (Vector2)worldSizeField.GetValue(navGrid);
                Vector2 navWorldOrigin = (Vector2)worldOriginField.GetValue(navGrid);
                
                areaSize = navWorldSize + Vector2.one * boundsPadding * 2f;
                
                // 将 CloudShadowManager 移动到世界中心
                Vector3 worldCenter = new Vector3(
                    navWorldOrigin.x + navWorldSize.x * 0.5f,
                    navWorldOrigin.y + navWorldSize.y * 0.5f,
                    transform.position.z
                );
                transform.position = worldCenter;
                
                Debug.Log($"<color=cyan>[CloudShadowManager] 从 NavGrid2D 获取区域: Size={areaSize}, Center={worldCenter}</color>");
            }
        }
        else
        {
            Debug.LogWarning("[CloudShadowManager] 未找到 NavGrid2D，使用手动设置的区域大小");
        }
    }
    
    /// <summary>
    /// 自动检测世界边界（基于 Tilemap）
    /// </summary>
    private void DetectWorldBounds()
    {
        Bounds totalBounds = new Bounds(Vector3.zero, Vector3.zero);
        bool boundsInitialized = false;
        
        // 查找所有指定层级下的 Tilemap
        foreach (string layerName in worldLayerNames)
        {
            GameObject layerObj = GameObject.Find(layerName);
            if (layerObj == null) continue;
            
            Tilemap[] tilemaps = layerObj.GetComponentsInChildren<Tilemap>();
            foreach (Tilemap tilemap in tilemaps)
            {
                tilemap.CompressBounds();
                BoundsInt tilemapBounds = tilemap.cellBounds;
                
                if (tilemapBounds.size.x == 0 || tilemapBounds.size.y == 0) continue;
                
                Vector3 worldMin = tilemap.transform.TransformPoint(tilemap.CellToLocal(tilemapBounds.min));
                Vector3 worldMax = tilemap.transform.TransformPoint(tilemap.CellToLocal(tilemapBounds.max));
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
        
        if (boundsInitialized)
        {
            // 添加边界扩展
            totalBounds.Expand(boundsPadding * 2f);
            
            areaSize = new Vector2(totalBounds.size.x, totalBounds.size.y);
            
            // 将 CloudShadowManager 移动到世界中心
            Vector3 worldCenter = new Vector3(totalBounds.center.x, totalBounds.center.y, transform.position.z);
            transform.position = worldCenter;
            
            Debug.Log($"<color=cyan>[CloudShadowManager] 自动检测区域: Size={areaSize}, Center={worldCenter}</color>");
        }
        else
        {
            Debug.LogWarning("[CloudShadowManager] 未找到有效的 Tilemap，使用手动设置的区域大小");
        }
    }

    /// <summary>
    /// 尝试在移动方向的起始边缘生成新云朵
    /// </summary>
    /// <returns>是否成功生成</returns>
    private bool TrySpawnOneAtEdge(Rect area, Vector2 dir)
    {
        // 严格检查数量限制
        if (active.Count >= maxClouds) return false;
        
        int spriteIndex = PickSpriteIndex();
        if (spriteIndex < 0) return false;
        Sprite sprite = cloudSprites[spriteIndex];
        
        // 先计算缩放和尺寸
        Vector2 sanitizedScaleRange = GetSanitizedScaleRange();
        float scale = Mathf.Lerp(sanitizedScaleRange.x, sanitizedScaleRange.y, Next01());
        
        // 估算云朵尺寸（基于 sprite 原始尺寸和缩放）
        float estimatedHalfW = (sprite.bounds.extents.x * scale);
        float estimatedHalfH = (sprite.bounds.extents.y * scale);
        
        // 尝试找到不重叠的位置
        Vector3? position = TryFindNonOverlappingEdgePosition(area, dir, estimatedHalfW, estimatedHalfH);
        
        if (!position.HasValue)
        {
            return false;
        }
        
        return CreateCloudInstance(spriteIndex, sprite, scale, position.Value);
    }
    
    /// <summary>
    /// 初始化时在整个区域内随机分布云朵
    /// </summary>
    private bool TrySpawnOneRandom(Rect area)
    {
        if (active.Count >= maxClouds) return false;
        
        int spriteIndex = PickSpriteIndex();
        if (spriteIndex < 0) return false;
        Sprite sprite = cloudSprites[spriteIndex];
        
        Vector2 sanitizedScaleRange = GetSanitizedScaleRange();
        float scale = Mathf.Lerp(sanitizedScaleRange.x, sanitizedScaleRange.y, Next01());
        float estimatedHalfW = sprite.bounds.extents.x * scale;
        float estimatedHalfH = sprite.bounds.extents.y * scale;
        
        // 尝试找到不重叠的位置
        Vector3? position = TryFindNonOverlappingAreaPosition(area, estimatedHalfW, estimatedHalfH);
        
        if (!position.HasValue) return false;
        
        return CreateCloudInstance(spriteIndex, sprite, scale, position.Value);
    }

    private bool TrySpawnOneAlongFlow(Rect area, Vector2 dir)
    {
        if (active.Count >= maxClouds) return false;

        int spriteIndex = PickSpriteIndex();
        if (spriteIndex < 0) return false;
        Sprite sprite = cloudSprites[spriteIndex];

        Vector2 sanitizedScaleRange = GetSanitizedScaleRange();
        float scale = Mathf.Lerp(sanitizedScaleRange.x, sanitizedScaleRange.y, Next01());
        float estimatedHalfW = sprite.bounds.extents.x * scale;
        float estimatedHalfH = sprite.bounds.extents.y * scale;

        float traversalDistance = GetTraversalDistance(area, dir, estimatedHalfW, estimatedHalfH);
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector3? edgePosition = TryFindNonOverlappingEdgePosition(area, dir, estimatedHalfW, estimatedHalfH);
            if (!edgePosition.HasValue)
            {
                return false;
            }

            float progress = Mathf.Lerp(0f, traversalDistance, Next01());
            Vector3 candidate = edgePosition.Value + (Vector3)(dir * progress);

            if (IsOverlappingWithExisting(candidate, estimatedHalfW, estimatedHalfH))
            {
                continue;
            }

            return CreateCloudInstance(spriteIndex, sprite, scale, candidate);
        }

        return false;
    }

    /// <summary>
    /// 获取或创建云朵对象
    /// </summary>
    private GameObject GetOrCreateCloudObject()
    {
        while (pool.Count > 0)
        {
            GameObject go = pool.Pop();
            if (go == null)
            {
                continue;
            }

            PrepareRecoveredCloudObject(go);
            go.SetActive(true);
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null) go.AddComponent<SpriteRenderer>();
            go.transform.SetParent(transform, false);
            return go;
        }

        GameObject fresh = new GameObject(RuntimeCloudObjectName);
        ConfigureFreshCloudObject(fresh);
        fresh.transform.SetParent(transform, false);
        fresh.AddComponent<SpriteRenderer>();
        return fresh;
    }

    private bool CreateCloudInstance(int spriteIndex, Sprite sprite, float scale, Vector3 position, int forcedCloudId = -1, float lifetime = 0f, float stuckTime = 0f)
    {
        GameObject go = GetOrCreateCloudObject();
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
        if (cloudMaterial != null) sr.sharedMaterial = cloudMaterial;

        int sortingId = SortingLayer.NameToID(sortingLayerName);
        if (sortingId != 0 || sortingLayerName == "Default")
        {
            sr.sortingLayerID = sortingId;
            sr.sortingOrder = sortingOrder;
        }

        go.transform.localScale = new Vector3(scale, scale, 1f);
        go.transform.position = position;

        Bounds b = sr.bounds;
        int cloudId = forcedCloudId >= 0 ? forcedCloudId : cloudIdCounter++;
        if (forcedCloudId >= 0)
        {
            cloudIdCounter = Mathf.Max(cloudIdCounter, forcedCloudId + 1);
        }
        var c = new Cloud
        {
            transform = go.transform,
            sr = sr,
            halfWidth = b.extents.x,
            halfHeight = b.extents.y,
            id = cloudId,
            spriteIndex = spriteIndex,
            scale = scale,
            lastPosition = position,
            lifetime = lifetime,
            stuckTime = stuckTime
        };

        Color col = sr.color;
        col.a = intensity;
        sr.color = col;

        active.Add(c);

        if (enableDebug)
        {
            Debug.Log($"[CloudShadow] 生成云朵 #{cloudId} at {position}, scale={scale:F2}, 当前数量: {active.Count}");
        }

        return true;
    }
    
    /// <summary>
    /// 尝试在边缘找到不重叠的位置
    /// </summary>
    /// <returns>找到的位置，如果找不到返回 null</returns>
    private Vector3? TryFindNonOverlappingEdgePosition(Rect area, Vector2 dir, float halfW, float halfH)
    {
        // 计算生成边缘位置（在区域外一点点）
        float left = area.xMin - halfW - SpawnEdgeOffset;
        float right = area.xMax + halfW + SpawnEdgeOffset;
        float bottom = area.yMin - halfH - SpawnEdgeOffset;
        float top = area.yMax + halfH + SpawnEdgeOffset;

        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector3 p = Vector3.zero;
            float relaxedSpacing = GetSpawnSpacingMultiplier(attempt);
            
            // 根据移动方向决定生成边缘
            if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            {
                // 水平移动：在左边或右边生成
                p.x = dir.x > 0f ? left : right;
                p.y = Mathf.Lerp(area.yMin + halfH, area.yMax - halfH, Next01());
            }
            else
            {
                // 垂直移动：在上边或下边生成
                p.y = dir.y > 0f ? bottom : top;
                p.x = Mathf.Lerp(area.xMin + halfW, area.xMax - halfW, Next01());
            }
            
            if (!IsOverlappingWithExisting(p, halfW, halfH, relaxedSpacing))
            {
                return p;
            }
        }
        
        return null; // 找不到合适位置
    }

    private Vector3? TryFindNonOverlappingEntryBandPosition(Rect area, Vector2 dir, float halfW, float halfH)
    {
        float traversalDistance = GetTraversalDistance(area, dir, halfW, halfH);
        float bandDepth = GetEntryBandDepth(traversalDistance);

        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            float relaxedSpacing = GetSpawnSpacingMultiplier(attempt);
            Vector3 p = Vector3.zero;

            if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            {
                float minX = dir.x > 0f ? area.xMin + halfW : area.xMax - halfW - bandDepth;
                float maxX = dir.x > 0f ? area.xMin + halfW + bandDepth : area.xMax - halfW;
                p.x = Mathf.Lerp(minX, maxX, Next01());
                p.y = Mathf.Lerp(area.yMin + halfH, area.yMax - halfH, Next01());
            }
            else
            {
                float minY = dir.y > 0f ? area.yMin + halfH : area.yMax - halfH - bandDepth;
                float maxY = dir.y > 0f ? area.yMin + halfH + bandDepth : area.yMax - halfH;
                p.x = Mathf.Lerp(area.xMin + halfW, area.xMax - halfW, Next01());
                p.y = Mathf.Lerp(minY, maxY, Next01());
            }

            if (!IsOverlappingWithExisting(p, halfW, halfH, relaxedSpacing))
            {
                return p;
            }
        }

        return null;
    }
    
    /// <summary>
    /// 尝试在整个区域内找到不重叠的位置
    /// </summary>
    private Vector3? TryFindNonOverlappingAreaPosition(Rect area, float halfW, float halfH)
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            float relaxedSpacing = GetSpawnSpacingMultiplier(attempt);
            Vector3 p = new Vector3(
                Mathf.Lerp(area.xMin + halfW, area.xMax - halfW, Next01()),
                Mathf.Lerp(area.yMin + halfH, area.yMax - halfH, Next01()),
                0f
            );
            
            if (!IsOverlappingWithExisting(p, halfW, halfH, relaxedSpacing))
            {
                return p;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// 检查位置是否与现有云朵重叠
    /// 使用简单的圆形距离检测
    /// </summary>
    private bool IsOverlappingWithExisting(Vector3 position, float halfW, float halfH, float spacingMultiplier = 1f)
    {
        float myRadius = Mathf.Max(halfW, halfH);
        
        foreach (var cloud in active)
        {
            if (cloud.transform == null) continue;
            
            float otherRadius = Mathf.Max(cloud.halfWidth, cloud.halfHeight);
            float dist = Vector2.Distance(position, cloud.transform.position);
            float minDist = Mathf.Max(0f, minCloudSpacing * spacingMultiplier) + myRadius + otherRadius;
            
            if (dist < minDist)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 销毁指定索引的云朵
    /// </summary>
    private void DespawnAt(int index)
    {
        if (index < 0 || index >= active.Count) return;
        var c = active[index];
        if (c.transform != null)
        {
            var go = c.transform.gameObject;
            go.SetActive(false);
            go.transform.SetParent(transform, false);
            pool.Push(go);
        }
        active.RemoveAt(index);
    }

    private void DespawnLast()
    {
        if (active.Count == 0) return;
        DespawnAt(active.Count - 1);
    }

    private void DespawnAll()
    {
        for (int i = active.Count - 1; i >= 0; i--)
        {
            var c = active[i];
            if (c.transform != null)
            {
                var go = c.transform.gameObject;
                go.SetActive(false);
                go.transform.SetParent(transform, false);
                pool.Push(go);
            }
        }
        active.Clear();
    }

    private void NormalizeResidualCloudChildren()
    {
        if (transform == null)
        {
            return;
        }

        HashSet<Transform> trackedTransforms = new HashSet<Transform>();
        for (int i = 0; i < active.Count; i++)
        {
            if (active[i].transform != null)
            {
                trackedTransforms.Add(active[i].transform);
            }
        }

        HashSet<GameObject> pooledObjects = new HashSet<GameObject>();
        foreach (GameObject pooled in pool)
        {
            if (pooled != null)
            {
                pooledObjects.Add(pooled);
            }
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child == null || child.name != RuntimeCloudObjectName)
            {
                continue;
            }

            GameObject go = child.gameObject;
            PrepareRecoveredCloudObject(go);

            if (trackedTransforms.Contains(child) || pooledObjects.Contains(go))
            {
                continue;
            }

            go.SetActive(false);
            pool.Push(go);
            pooledObjects.Add(go);
        }
    }

    private void PrepareRecoveredCloudObject(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        go.name = RuntimeCloudObjectName;
    }

    private void ConfigureFreshCloudObject(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        go.name = RuntimeCloudObjectName;
        go.hideFlags = RuntimeCloudHideFlags;
    }

    private int PickSpriteIndex()
    {
        if (!HasConfiguredCloudSprite()) return -1;

        int startIndex = Mathf.Clamp((int)(Next01() * cloudSprites.Length), 0, cloudSprites.Length - 1);
        for (int offset = 0; offset < cloudSprites.Length; offset++)
        {
            int index = (startIndex + offset) % cloudSprites.Length;
            if (cloudSprites[index] != null)
            {
                return index;
            }
        }

        return -1;
    }

    private Rect GetWorldAreaRect()
    {
        Vector3 c = transform.position;
        float w = Mathf.Max(0.1f, areaSize.x);
        float h = Mathf.Max(0.1f, areaSize.y);
        return new Rect(c.x - w * 0.5f, c.y - h * 0.5f, w, h);
    }

    private bool IsWeatherAllowed(WeatherState s)
    {
        switch (s)
        {
            case WeatherState.Sunny: return enableInSunny;
            case WeatherState.PartlyCloudy: return enableInPartlyCloudy;
            case WeatherState.Overcast: return enableInOvercast;
            case WeatherState.Rain: return enableInRain;
            case WeatherState.Snow: return enableInSnow;
        }
        return true;
    }

    private void EnsureRng()
    {
        if (rng == null) rng = new System.Random(seed);
    }

    private float Next01()
    {
        if (rng == null) EnsureRng();
        return (float)rng.NextDouble();
    }

    private Vector2 GetSanitizedScaleRange()
    {
        float minScale = Mathf.Max(0.05f, Mathf.Min(scaleRange.x, scaleRange.y));
        float maxScale = Mathf.Max(minScale, Mathf.Max(scaleRange.x, scaleRange.y));
        return new Vector2(minScale, maxScale);
    }

    private float GetSpawnSpacingMultiplier(int attempt)
    {
        if (maxSpawnAttempts <= 1)
        {
            return 1f;
        }

        float t = attempt / (float)(maxSpawnAttempts - 1);
        return Mathf.Lerp(1f, 0.35f, t);
    }

    private float GetTraversalDistance(Rect area, Vector2 dir, float halfW, float halfH)
    {
        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
        {
            return Mathf.Max(0f, area.width + (halfW * 2f) + SpawnEdgeOffset + ExitDespawnMargin);
        }

        return Mathf.Max(0f, area.height + (halfH * 2f) + SpawnEdgeOffset + ExitDespawnMargin);
    }

    private float GetEntryBandDepth(float traversalDistance)
    {
        if (traversalDistance <= 0f)
        {
            return 0f;
        }

        return Mathf.Clamp(traversalDistance * 0.22f, EntryBandMinDepth, EntryBandMaxDepth);
    }

    private void TryLogSpawnStall(Rect area, int target, float currentTime)
    {
        if (!enableDebug)
        {
            return;
        }

        if ((currentTime - lastSpawnFailureLogTime) < 1.5f)
        {
            return;
        }

        lastSpawnFailureLogTime = currentTime;
        Debug.Log($"[CloudShadow] 补云受阻: Active={active.Count}/{target}, Area={area.size}, Spacing={minCloudSpacing:F2}, MaxAttempts={maxSpawnAttempts}");
    }

    public void RandomizeSeed()
    {
        seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        rng = new System.Random(seed);
    }

    public void SetWeatherState(WeatherState s)
    {
        currentWeather = s;
    }

    public void EditorRebuildNow()
    {
        initialized = false;
        RemoveRuntimeState();
        InitializeIfNeeded();
    }
    
    /// <summary>
    /// 重建所有云朵（清除后重新生成）
    /// </summary>
    private void RebuildClouds()
    {
        NormalizeResidualCloudChildren();
        DespawnAll();
        cloudIdCounter = 0;
        consecutiveSpawnFailures = 0;
        lastSpawnTime = -999f;
        lastSpawnFailureLogTime = -999f;
        
        Rect area = GetWorldAreaRect();
        int target = GetTargetCloudCount();
        Vector2 dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : new Vector2(1f, 0f);
        
        int spawned = 0;
        int maxAttempts = Mathf.Max(target * Mathf.Max(3, maxSpawnAttempts), maxSpawnAttempts); // 防止无限循环
        int attempts = 0;
        
        while (spawned < target && attempts < maxAttempts)
        {
            if (TrySpawnOneAlongFlow(area, dir))
            {
                spawned++;
            }
            attempts++;
        }
        
        if (enableDebug)
        {
            Debug.Log($"[CloudShadow] 初始化完成: 生成 {spawned}/{target} 个云朵");
        }
    }

    private bool TryRestoreRuntimeState()
    {
        if (!runtimeStateByManager.TryGetValue(GetRuntimeStateKey(), out SceneRuntimeState state))
        {
            return false;
        }

        seed = state.seed;
        rng = new System.Random(seed);
        cloudIdCounter = state.cloudIdCounter;
        lastSpawnTime = state.lastSpawnTime;
        lastSpawnFailureLogTime = state.lastSpawnFailureLogTime;
        consecutiveSpawnFailures = state.consecutiveSpawnFailures;

        NormalizeResidualCloudChildren();
        DespawnAll();

        int restoredCount = 0;
        for (int i = 0; i < state.clouds.Count; i++)
        {
            CloudRuntimeState cloudState = state.clouds[i];
            if (cloudState.spriteIndex < 0 || cloudState.spriteIndex >= cloudSprites.Length)
            {
                continue;
            }

            Sprite sprite = cloudSprites[cloudState.spriteIndex];
            if (sprite == null)
            {
                continue;
            }

            if (CreateCloudInstance(cloudState.spriteIndex, sprite, cloudState.scale, cloudState.position, cloudState.id, cloudState.lifetime, cloudState.stuckTime))
            {
                restoredCount++;
            }
        }

        if (enableDebug)
        {
            Debug.Log($"[CloudShadow] 恢复场景缓存: {restoredCount}/{state.clouds.Count} 个云朵");
        }

        return true;
    }

    private void SaveRuntimeState()
    {
        if (!Application.isPlaying || !initialized)
        {
            return;
        }

        SceneRuntimeState state = new SceneRuntimeState
        {
            seed = seed,
            cloudIdCounter = cloudIdCounter,
            lastSpawnTime = lastSpawnTime,
            lastSpawnFailureLogTime = lastSpawnFailureLogTime,
            consecutiveSpawnFailures = consecutiveSpawnFailures
        };

        for (int i = 0; i < active.Count; i++)
        {
            Cloud cloud = active[i];
            if (cloud.transform == null || cloud.sr == null)
            {
                continue;
            }

            state.clouds.Add(new CloudRuntimeState
            {
                position = cloud.transform.position,
                spriteIndex = cloud.spriteIndex,
                scale = cloud.scale,
                id = cloud.id,
                lifetime = cloud.lifetime,
                stuckTime = cloud.stuckTime
            });
        }

        runtimeStateByManager[GetRuntimeStateKey()] = state;
    }

    private void RemoveRuntimeState()
    {
        runtimeStateByManager.Remove(GetRuntimeStateKey());
    }

    public static List<CloudShadowSceneSaveData> ExportPersistentSaveData()
    {
        List<CloudShadowSceneSaveData> result = new List<CloudShadowSceneSaveData>();
        Dictionary<string, CloudShadowSceneSaveData> byKey = new Dictionary<string, CloudShadowSceneSaveData>(StringComparer.Ordinal);

        foreach (KeyValuePair<string, SceneRuntimeState> pair in runtimeStateByManager)
        {
            CloudShadowSceneSaveData serialized = SerializeSceneRuntimeState(pair.Key, pair.Value);
            if (serialized == null)
            {
                continue;
            }

            byKey[pair.Key] = serialized;
        }

        CloudShadowManager[] managers = FindObjectsByType<CloudShadowManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int index = 0; index < managers.Length; index++)
        {
            CloudShadowManager manager = managers[index];
            if (manager == null)
            {
                continue;
            }

            manager.SaveRuntimeState();
            string runtimeKey = manager.GetRuntimeStateKey();
            if (!runtimeStateByManager.TryGetValue(runtimeKey, out SceneRuntimeState state))
            {
                continue;
            }

            CloudShadowSceneSaveData serialized = SerializeSceneRuntimeState(runtimeKey, state);
            if (serialized == null)
            {
                continue;
            }

            byKey[runtimeKey] = serialized;
        }

        foreach (CloudShadowSceneSaveData state in byKey.Values)
        {
            result.Add(state);
        }

        return result;
    }

    public static void ImportPersistentSaveData(List<CloudShadowSceneSaveData> serializedStates)
    {
        runtimeStateByManager.Clear();

        if (serializedStates != null)
        {
            for (int index = 0; index < serializedStates.Count; index++)
            {
                CloudShadowSceneSaveData serializedState = serializedStates[index];
                if (serializedState == null || string.IsNullOrWhiteSpace(serializedState.sceneKey) || string.IsNullOrWhiteSpace(serializedState.managerPath))
                {
                    continue;
                }

                string runtimeKey = BuildRuntimeStateKey(serializedState.sceneKey, serializedState.managerPath);
                runtimeStateByManager[runtimeKey] = DeserializeSceneRuntimeState(serializedState);
            }
        }

        CloudShadowManager[] managers = FindObjectsByType<CloudShadowManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int index = 0; index < managers.Length; index++)
        {
            CloudShadowManager manager = managers[index];
            if (manager == null)
            {
                continue;
            }

            manager.ApplyImportedPersistentState();
        }
    }

    private string GetRuntimeStateKey()
    {
        Scene scene = gameObject.scene;
        string sceneKey = string.IsNullOrEmpty(scene.path) ? scene.name : scene.path;
        return BuildRuntimeStateKey(sceneKey, GetHierarchyPath(transform));
    }

    private static string GetHierarchyPath(Transform target)
    {
        if (target == null)
        {
            return string.Empty;
        }

        List<string> segments = new List<string>();
        Transform current = target;
        while (current != null)
        {
            segments.Add(current.name);
            current = current.parent;
        }

        segments.Reverse();
        return string.Join("/", segments);
    }

    private void ApplyImportedPersistentState()
    {
        string runtimeKey = GetRuntimeStateKey();
        bool hasImportedState = runtimeStateByManager.ContainsKey(runtimeKey);

        initialized = false;
        if (!hasImportedState)
        {
            ClearCloudsForInactiveState();
            InitializeIfNeeded();
            return;
        }

        InitializeIfNeeded();
    }

    private static CloudShadowSceneSaveData SerializeSceneRuntimeState(string runtimeKey, SceneRuntimeState state)
    {
        if (state == null || string.IsNullOrWhiteSpace(runtimeKey))
        {
            return null;
        }

        ParseRuntimeStateKey(runtimeKey, out string sceneKey, out string managerPath);
        if (string.IsNullOrWhiteSpace(sceneKey) || string.IsNullOrWhiteSpace(managerPath))
        {
            return null;
        }

        CloudShadowSceneSaveData serialized = new CloudShadowSceneSaveData
        {
            sceneKey = sceneKey,
            managerPath = managerPath,
            seed = state.seed,
            cloudIdCounter = state.cloudIdCounter,
            lastSpawnTime = state.lastSpawnTime,
            lastSpawnFailureLogTime = state.lastSpawnFailureLogTime,
            consecutiveSpawnFailures = state.consecutiveSpawnFailures
        };

        for (int index = 0; index < state.clouds.Count; index++)
        {
            CloudRuntimeState cloud = state.clouds[index];
            serialized.clouds.Add(new CloudShadowEntrySaveData
            {
                positionX = cloud.position.x,
                positionY = cloud.position.y,
                positionZ = cloud.position.z,
                spriteIndex = cloud.spriteIndex,
                scale = cloud.scale,
                id = cloud.id,
                lifetime = cloud.lifetime,
                stuckTime = cloud.stuckTime
            });
        }

        return serialized;
    }

    private static SceneRuntimeState DeserializeSceneRuntimeState(CloudShadowSceneSaveData serializedState)
    {
        SceneRuntimeState runtimeState = new SceneRuntimeState
        {
            seed = serializedState.seed,
            cloudIdCounter = serializedState.cloudIdCounter,
            lastSpawnTime = serializedState.lastSpawnTime,
            lastSpawnFailureLogTime = serializedState.lastSpawnFailureLogTime,
            consecutiveSpawnFailures = serializedState.consecutiveSpawnFailures
        };

        if (serializedState.clouds == null)
        {
            return runtimeState;
        }

        for (int index = 0; index < serializedState.clouds.Count; index++)
        {
            CloudShadowEntrySaveData cloud = serializedState.clouds[index];
            if (cloud == null)
            {
                continue;
            }

            runtimeState.clouds.Add(new CloudRuntimeState
            {
                position = new Vector3(cloud.positionX, cloud.positionY, cloud.positionZ),
                spriteIndex = cloud.spriteIndex,
                scale = cloud.scale,
                id = cloud.id,
                lifetime = cloud.lifetime,
                stuckTime = cloud.stuckTime
            });
        }

        return runtimeState;
    }

    private static string BuildRuntimeStateKey(string sceneKey, string managerPath)
    {
        return sceneKey + "::" + managerPath;
    }

    private static void ParseRuntimeStateKey(string runtimeKey, out string sceneKey, out string managerPath)
    {
        sceneKey = string.Empty;
        managerPath = string.Empty;

        if (string.IsNullOrWhiteSpace(runtimeKey))
        {
            return;
        }

        int splitIndex = runtimeKey.IndexOf("::", StringComparison.Ordinal);
        if (splitIndex < 0)
        {
            return;
        }

        sceneKey = runtimeKey.Substring(0, splitIndex);
        managerPath = runtimeKey.Substring(splitIndex + 2);
    }

    public void EditorDespawnAll()
    {
        ClearCloudsForInactiveState();
    }

    #if UNITY_EDITOR
    private double lastEditorTime;
    private void EditorUpdate()
    {
        if (Application.isPlaying) return;
        if (!previewInEditor)
        {
            ClearCloudsForInactiveState();
            return;
        }
        double now = UnityEditor.EditorApplication.timeSinceStartup;
        double dt = now - lastEditorTime;
        lastEditorTime = now;
        SimulateStep((float)Mathf.Clamp((float)dt, 0f, 0.1f));
    }
    #endif

    void OnDrawGizmosSelected()
    {
        Rect r = GetWorldAreaRect();
        Gizmos.color = new Color(1f, 1f, 0f, 0.35f);
        Gizmos.DrawWireCube(new Vector3(r.center.x, r.center.y, 0f), new Vector3(r.width, r.height, 0f));

        // Draw movement direction arrow
        Vector2 dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : new Vector2(1f, 0f);
        float len = Mathf.Min(r.width, r.height) * 0.3f;
        Vector3 start = new Vector3(r.center.x, r.center.y, 0f);
        Vector3 end = start + (Vector3)(dir * len);
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.9f);
        Gizmos.DrawLine(start, end);
        // arrow head
        Vector3 right = Quaternion.Euler(0, 0, 150f) * (Vector3)(dir * (len * 0.2f));
        Vector3 left = Quaternion.Euler(0, 0, -150f) * (Vector3)(dir * (len * 0.2f));
        Gizmos.DrawLine(end, end + right);
        Gizmos.DrawLine(end, end + left);
    }
}
