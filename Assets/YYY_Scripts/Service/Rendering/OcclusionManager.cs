using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 遮挡方向枚举
/// </summary>
public enum OcclusionDirection
{
    Top,    // 树在玩家上方
    Bottom, // 树在玩家下方
    Left,   // 树在玩家左侧
    Right   // 树在玩家右侧
}

/// <summary>
/// 预览遮挡来源枚举。
/// 放在已跟踪的 rendering 主文件里，避免未归仓的新文件单独参与编译可见性时把 preview 链卡红。
/// </summary>
public enum PreviewOcclusionSource
{
    Generic = 0,
    FarmTool = 1,
    PlaceablePlacement = 2
}

/// <summary>
/// 标签遮挡参数配置
/// </summary>
[System.Serializable]
public class TagOcclusionParams
{
    [Tooltip("标签名称")]
    public string tag = "Tree";

    [Tooltip("遮挡时的透明度")]
    [Range(0f, 1f)]
    public float occludedAlpha = 0.3f;

    [Tooltip("渐变速度")]
    [Range(1f, 20f)]
    public float fadeSpeed = 8f;
}

/// <summary>
/// 遮挡管理器：管理所有可遮挡物体，检测玩家被遮挡情况
/// 核心逻辑：玩家 Collider 中心被遮挡 → 遮挡物透明
/// </summary>
public class OcclusionManager : MonoBehaviour
{
    private const float FarmToolPreviewHoverExpand = 0.4f;
    private const float PlaceablePreviewHoverExpand = 0.14f;

    [Header("玩家引用")]
    [Tooltip("玩家Transform（自动查找Player标签）")]
    [SerializeField] private Transform player;

    [Tooltip("玩家的SpriteRenderer（用于bounds检测）")]
    [SerializeField] private SpriteRenderer playerSprite;

    [Tooltip("玩家的Collider2D（用于获取中心点）")]
    [SerializeField] private Collider2D playerCollider;

    [Tooltip("玩家的DynamicSortingOrder组件")]
    [SerializeField] private DynamicSortingOrder playerSorting;

    [Header("检测设置")]
    [Tooltip("检测半径（只检测玩家周围此范围内的物体）")]
    [SerializeField, Range(1f, 20f)] private float detectionRadius = 8f;

    [Tooltip("检测间隔（秒）- 避免每帧检测")]
    [SerializeField, Range(0.05f, 0.5f)] private float detectionInterval = 0.1f;

    [Header("透明度设置（全局）")]
    [Tooltip("遮挡时的目标透明度（全局默认值）")]
    [SerializeField, Range(0f, 1f)] private float globalOccludedAlpha = 0.3f;

    [Tooltip("透明度渐变速度（全局默认值）")]
    [SerializeField, Range(1f, 20f)] private float globalFadeSpeed = 8f;

    [Header("标签自定义参数")]
    [Tooltip("启用标签自定义参数（不同标签可以有不同的透明度）")]
    [SerializeField] private bool useTagCustomParams = false;

    [Tooltip("标签自定义参数列表")]
    [SerializeField] private TagOcclusionParams[] tagParams = new TagOcclusionParams[]
    {
        new TagOcclusionParams { tag = "Tree", occludedAlpha = 0.3f, fadeSpeed = 8f },
        new TagOcclusionParams { tag = "Building", occludedAlpha = 0.4f, fadeSpeed = 10f },
        new TagOcclusionParams { tag = "Rock", occludedAlpha = 0.5f, fadeSpeed = 6f }
    };


    [Header("过滤设置")]
    [Tooltip("启用标签过滤（只检测指定标签的物体）")]
    [SerializeField] private bool useTagFilter = true;

    [Tooltip("可遮挡的标签列表")]
    [SerializeField] private string[] occludableTags = new string[] { "Tree", "Building", "Rock" };

    [Tooltip("只检测同一Sorting Layer的物体")]
    [SerializeField] private bool sameSortingLayerOnly = true;

    [Header("遮挡占比过滤")]
    [Tooltip("启用遮挡占比过滤（只有被遮挡很多才触发透明）")]
    [SerializeField] private bool useOcclusionRatioFilter = true;

    [Tooltip("最小遮挡占比阈值（0-1）- 玩家被遮挡的面积占玩家总面积的比例")]
    [Range(0f, 1f)]
    [SerializeField] private float minOcclusionRatio = 0.4f;

    [Header("树林整体透明")]
    [Tooltip("启用树林整体透明（进入树林时整片树木都透明）")]
    [SerializeField] private bool enableForestTransparency = true;

    [Tooltip("树根连通距离（米）- 两棵树的种植点距离小于此值才算连通")]
    [SerializeField, Range(1f, 5f)] private float rootConnectionDistance = 2.5f;

    [Tooltip("最大搜索深度（防止性能问题）- 限制最多搜索多少棵树")]
    [SerializeField, Range(5, 100)] private int maxForestSearchDepth = 50;

    [Tooltip("最大搜索半径（米）- 超出此范围的树木不会被包含")]
    [SerializeField, Range(5f, 30f)] private float maxForestSearchRadius = 15f;

    [Header("调试")]
    [SerializeField] private bool showDebugGizmos = false;
    [SerializeField] private bool enableDetailedDebug = false;

    private float lastDebugLogTime = 0f;
    private const float DEBUG_LOG_INTERVAL = 1f;

    // 单例
    private static OcclusionManager instance;
    public static OcclusionManager Instance => instance;

    // 注册的可遮挡物体
    private HashSet<OcclusionTransparency> registeredOccluders = new HashSet<OcclusionTransparency>();

    // 当前正在遮挡玩家的物体
    private HashSet<OcclusionTransparency> currentlyOccluding = new HashSet<OcclusionTransparency>();
    private HashSet<OcclusionTransparency> previousOccluding = new HashSet<OcclusionTransparency>();

    // 树林缓存
    private HashSet<OcclusionTransparency> currentForest = new HashSet<OcclusionTransparency>();
    private Bounds currentForestBounds;
    private OcclusionTransparency lastOccludingTree;

    // ✅ 树林边界缓存（凸包）
    private List<Vector2> currentForestHull = new List<Vector2>();
    private bool forestHullValid = false;

    // ✅ 边缘遮挡模式
    [Header("边缘遮挡设置")]
    [Tooltip("启用智能边缘遮挡（边缘只透明单树，内部透明整林）")]
    [SerializeField] private bool enableSmartEdgeOcclusion = true;

    // ✅ 砍伐高亮管理
    private OcclusionTransparency currentChoppingTree;
    private float lastChopTime;
    private const float CHOPPING_TIMEOUT = 3f;  // 3秒超时

    // ✅ 预览遮挡检测
    private Bounds? previewBounds = null;
    private Bounds? genericPreviewBounds = null;
    private Bounds? farmPreviewBounds = null;
    private Bounds? placeablePreviewBounds = null;
    private PreviewOcclusionSource activePreviewSource = PreviewOcclusionSource.Generic;
    private HashSet<OcclusionTransparency> previewOccluding = new HashSet<OcclusionTransparency>();

    private float lastDetectionTime = 0f;
    private string playerLayer = "";

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        RefreshRegisteredOccludersFromScene();
        RefreshPlayerBindings(forceSearch: true);
    }

    void Update()
    {
        EnsurePlayerBindings();
        if (player == null) return;

        // 按间隔检测
        if (Time.time - lastDetectionTime >= detectionInterval)
        {
            lastDetectionTime = Time.time;
            DetectOcclusion();
        }

        // ✅ 砍伐高亮超时检测
        CheckChoppingTimeout();
    }

    public void RefreshRuntimePlayerBinding(Transform runtimePlayer)
    {
        if (runtimePlayer != null)
        {
            BindPlayerObject(runtimePlayer.gameObject);
        }
        else
        {
            RefreshPlayerBindings(forceSearch: true);
        }

        RefreshRegisteredOccludersFromScene();
    }

    private void EnsurePlayerBindings()
    {
        if (HasUsablePlayerBindings())
        {
            playerLayer = ResolvePlayerLayer(playerSprite, playerSorting);

            return;
        }

        RefreshPlayerBindings(forceSearch: true);
    }

    private void RefreshPlayerBindings(bool forceSearch)
    {
        if (!forceSearch && HasUsablePlayerBindings())
        {
            return;
        }

        GameObject playerObject = ResolvePlayerObject();
        BindPlayerObject(playerObject);
    }

    private GameObject ResolvePlayerObject()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        if (playerObjects == null || playerObjects.Length == 0)
        {
            return null;
        }

        GameObject bestCandidate = null;
        int bestScore = int.MinValue;
        for (int index = 0; index < playerObjects.Length; index++)
        {
            GameObject candidate = playerObjects[index];
            int score = ScorePlayerCandidate(candidate);
            if (score > bestScore)
            {
                bestScore = score;
                bestCandidate = candidate;
            }
        }

        return bestCandidate;
    }

    private void RefreshRegisteredOccludersFromScene()
    {
        Scene managerScene = gameObject.scene;
        registeredOccluders.RemoveWhere(occluder =>
            occluder == null
            || !occluder.gameObject.scene.IsValid()
            || occluder.gameObject.scene != managerScene);

        if (!managerScene.IsValid() || !managerScene.isLoaded)
        {
            return;
        }

        OcclusionTransparency[] sceneOccluders =
            FindObjectsByType<OcclusionTransparency>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int index = 0; index < sceneOccluders.Length; index++)
        {
            OcclusionTransparency occluder = sceneOccluders[index];
            if (occluder == null
                || occluder.gameObject.scene != managerScene
                || !occluder.CanBeOccluded)
            {
                continue;
            }

            registeredOccluders.Add(occluder);
        }
    }

    private void BindPlayerObject(GameObject playerObject)
    {
        if (playerObject == null)
        {
            player = null;
            playerSprite = null;
            playerCollider = null;
            playerSorting = null;
            playerLayer = string.Empty;
            return;
        }

        player = playerObject.transform;
        playerSprite = ResolvePlayerSprite(playerObject);
        playerCollider = playerObject.GetComponent<Collider2D>()
            ?? playerObject.GetComponentInChildren<Collider2D>(true);
        playerSorting = playerObject.GetComponent<DynamicSortingOrder>()
            ?? playerObject.GetComponentInChildren<DynamicSortingOrder>(true);
        playerLayer = ResolvePlayerLayer(playerSprite, playerSorting);
    }

    private bool HasUsablePlayerBindings()
    {
        return player != null
            && player.gameObject != null
            && player.gameObject.activeInHierarchy
            && player.gameObject.CompareTag("Player")
            && (IsUsablePlayerSprite(playerSprite) || HasUsablePlayerSortingLayer(playerSorting));
    }

    private static SpriteRenderer ResolvePlayerSprite(GameObject playerObject)
    {
        if (playerObject == null)
        {
            return null;
        }

        PlayerAnimController playerAnimController = playerObject.GetComponent<PlayerAnimController>()
            ?? playerObject.GetComponentInChildren<PlayerAnimController>(true);
        if (playerAnimController != null)
        {
            SpriteRenderer authoritativeRenderer = playerAnimController.BodySpriteRenderer;
            if (IsUsablePlayerSprite(authoritativeRenderer))
            {
                return authoritativeRenderer;
            }
        }

        SpriteRenderer directRenderer = playerObject.GetComponent<SpriteRenderer>();
        if (IsUsablePlayerSprite(directRenderer))
        {
            return directRenderer;
        }

        SpriteRenderer[] renderers = playerObject.GetComponentsInChildren<SpriteRenderer>(true);
        SpriteRenderer fallbackRenderer = null;
        for (int index = 0; index < renderers.Length; index++)
        {
            SpriteRenderer renderer = renderers[index];
            if (renderer == null)
            {
                continue;
            }

            fallbackRenderer ??= renderer;
            if (IsUsablePlayerSprite(renderer))
            {
                return renderer;
            }
        }

        return fallbackRenderer;
    }

    private static int ScorePlayerCandidate(GameObject candidate)
    {
        if (candidate == null || !candidate.activeInHierarchy)
        {
            return int.MinValue;
        }

        int score = 0;

        if (candidate.GetComponent<PlayerAnimController>() != null
            || candidate.GetComponentInChildren<PlayerAnimController>(true) != null)
        {
            score += 1000;
        }

        if (candidate.GetComponent<PlayerMovement>() != null
            || candidate.GetComponentInChildren<PlayerMovement>(true) != null)
        {
            score += 500;
        }

        if (candidate.GetComponent<PlayerController>() != null
            || candidate.GetComponentInChildren<PlayerController>(true) != null)
        {
            score += 200;
        }

        SpriteRenderer spriteRenderer = ResolvePlayerSprite(candidate);
        if (IsUsablePlayerSprite(spriteRenderer))
        {
            score += 300;
        }
        else if (spriteRenderer != null)
        {
            score += 10;
        }

        DynamicSortingOrder sorting = candidate.GetComponent<DynamicSortingOrder>()
            ?? candidate.GetComponentInChildren<DynamicSortingOrder>(true);
        if (sorting != null)
        {
            score += 50;
        }

        Collider2D collider = candidate.GetComponent<Collider2D>()
            ?? candidate.GetComponentInChildren<Collider2D>(true);
        if (collider != null)
        {
            score += 25;
        }

        if (candidate.transform.parent == null)
        {
            score += 5;
        }

        return score;
    }

    private static bool IsUsablePlayerSprite(SpriteRenderer renderer)
    {
        return renderer != null
            && renderer.enabled
            && renderer.gameObject != null
            && renderer.gameObject.activeInHierarchy
            && renderer.sprite != null;
    }

    private static bool HasUsablePlayerSortingLayer(DynamicSortingOrder sorting)
    {
        return sorting != null && !string.IsNullOrWhiteSpace(sorting.GetCurrentSortingLayer());
    }

    private static string ResolvePlayerLayer(SpriteRenderer spriteRenderer, DynamicSortingOrder sorting)
    {
        if (IsUsablePlayerSprite(spriteRenderer) && !string.IsNullOrWhiteSpace(spriteRenderer.sortingLayerName))
        {
            return spriteRenderer.sortingLayerName;
        }

        if (HasUsablePlayerSortingLayer(sorting))
        {
            return sorting.GetCurrentSortingLayer();
        }

        return spriteRenderer != null ? spriteRenderer.sortingLayerName : string.Empty;
    }

    private bool ContainsPhysicalTree(HashSet<OcclusionTransparency> collection, OcclusionTransparency occluder)
    {
        if (occluder == null)
        {
            return false;
        }

        foreach (var item in collection)
        {
            if (item != null && item.SharesOcclusionRoot(occluder))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsOccluderInFrontOfPlayer(OcclusionTransparency occluder, Bounds playerFootBounds)
    {
        if (occluder == null)
        {
            return false;
        }

        float playerFootY = playerFootBounds.min.y;
        float occluderFootY = occluder.GetBounds().min.y;

        if (Mathf.Abs(playerFootY - occluderFootY) > 0.05f)
        {
            return playerFootY > occluderFootY;
        }

        if (playerSprite != null)
        {
            int occluderOrder = occluder.GetSortingOrder();
            int playerOrder = playerSprite.sortingOrder;
            if (occluderOrder != playerOrder)
            {
                return occluderOrder > playerOrder;
            }
        }

        return playerFootBounds.center.y > occluder.GetBounds().center.y + 0.01f;
    }

    private bool ForestContainsPhysicalTree(OcclusionTransparency occluder)
    {
        return ContainsPhysicalTree(currentForest, occluder);
    }

    private void SetPhysicalTreeOccluding(
        OcclusionTransparency occluder,
        bool occluding,
        float customAlpha = -1f,
        float customSpeed = -1f)
    {
        if (occluder == null)
        {
            return;
        }

        foreach (var candidate in registeredOccluders)
        {
            if (candidate == null || !candidate.SharesOcclusionRoot(occluder))
            {
                continue;
            }

            candidate.SetOccluding(occluding, customAlpha, customSpeed);
        }
    }

    private void SetPhysicalTreeChopping(
        OcclusionTransparency occluder,
        bool chopping,
        float alphaOffset = 0.25f)
    {
        if (occluder == null)
        {
            return;
        }

        foreach (var candidate in registeredOccluders)
        {
            if (candidate == null || !candidate.SharesOcclusionRoot(occluder))
            {
                continue;
            }

            candidate.SetChoppingState(chopping, alphaOffset);
        }
    }

    private Vector2 GetOccluderReferencePosition(OcclusionTransparency occluder)
    {
        if (occluder == null)
        {
            return Vector2.zero;
        }

        if (occluder.IsTreeOccluder())
        {
            return occluder.GetOcclusionRootTransform().position;
        }

        return occluder.GetBounds().center;
    }

    /// <summary>
    /// ✅ 设置当前正在砍伐的树木（砍伐高亮）
    /// 确保同时只有一棵树处于砍伐高亮状态
    /// </summary>
    /// <param name="tree">正在砍伐的树木（null 表示清除高亮）</param>
    /// <param name="alphaOffset">透明度偏移量（默认0.5，值越大越不透明）</param>
    public void SetChoppingTree(OcclusionTransparency tree, float alphaOffset = 0.5f)
    {
        // 清除之前的高亮
        if (currentChoppingTree != null && !currentChoppingTree.SharesOcclusionRoot(tree))
        {
            SetPhysicalTreeChopping(currentChoppingTree, false);
        }

        // 设置新的高亮
        currentChoppingTree = tree;
        if (tree != null)
        {
            SetPhysicalTreeChopping(tree, true, alphaOffset);
            lastChopTime = Time.time;
        }
    }

    /// <summary>
    /// ✅ 清除砍伐高亮状态
    /// </summary>
    public void ClearChoppingHighlight()
    {
        if (currentChoppingTree != null)
        {
            SetPhysicalTreeChopping(currentChoppingTree, false);
            currentChoppingTree = null;
        }
    }

    /// <summary>
    /// ✅ 检查砍伐高亮超时
    /// </summary>
    private void CheckChoppingTimeout()
    {
        if (currentChoppingTree != null && Time.time - lastChopTime > CHOPPING_TIMEOUT)
        {
            ClearChoppingHighlight();
        }
    }

    /// <summary>
    /// ✅ 获取当前正在砍伐的树木
    /// </summary>
    public OcclusionTransparency CurrentChoppingTree => currentChoppingTree;

    #region 预览遮挡检测

    /// <summary>
    /// ✅ 设置预览 Bounds（用于放置预览的遮挡检测）
    /// 当预览显示时调用此方法，传入预览的 Bounds
    /// 当预览隐藏时调用此方法，传入 null
    /// </summary>
    /// <param name="bounds">预览的 Bounds，null 表示清除预览检测</param>
    public void SetPreviewBounds(Bounds? bounds)
    {
        SetPreviewBounds(PreviewOcclusionSource.Generic, bounds);
    }

    public void SetPreviewBounds(PreviewOcclusionSource source, Bounds? bounds)
    {
        switch (source)
        {
            case PreviewOcclusionSource.FarmTool:
                farmPreviewBounds = bounds;
                break;
            case PreviewOcclusionSource.PlaceablePlacement:
                placeablePreviewBounds = bounds;
                break;
            default:
                genericPreviewBounds = bounds;
                break;
        }

        Bounds? resolvedPreviewBounds = ResolveActivePreviewBounds(out PreviewOcclusionSource resolvedSource);
        if (resolvedPreviewBounds == null && previewBounds != null)
        {
            ClearPreviewOcclusion();
        }

        previewBounds = resolvedPreviewBounds;
        activePreviewSource = resolvedSource;
        if (previewBounds != null)
        {
            DetectPreviewOcclusion();
        }
    }

    private Bounds? ResolveActivePreviewBounds(out PreviewOcclusionSource resolvedSource)
    {
        if (placeablePreviewBounds != null)
        {
            resolvedSource = PreviewOcclusionSource.PlaceablePlacement;
            return placeablePreviewBounds;
        }

        if (farmPreviewBounds != null)
        {
            resolvedSource = PreviewOcclusionSource.FarmTool;
            return farmPreviewBounds;
        }

        resolvedSource = PreviewOcclusionSource.Generic;
        return genericPreviewBounds;
    }

    /// <summary>
    /// ✅ 检测预览遮挡
    /// 对于预览：minOcclusionRatio = 0（任何遮挡都触发透明）
    /// </summary>
    private void DetectPreviewOcclusion()
    {
        if (previewBounds == null) return;

        Bounds bounds = previewBounds.Value;
        Bounds detectionBounds = ExpandPreviewBoundsForOcclusion(bounds, activePreviewSource);
        Vector2 previewCenter = detectionBounds.center;

        // 记录之前被预览遮挡的物体
        var previousPreviewOccluding = new HashSet<OcclusionTransparency>(previewOccluding);
        previewOccluding.Clear();

        // 遍历所有注册的可遮挡物体
        foreach (var occluder in registeredOccluders)
        {
            if (occluder == null || !occluder.CanBeOccluded) continue;

            Bounds occluderBounds = occluder.GetPreviewOcclusionBounds(activePreviewSource);
            float distanceToOccluderBounds = Mathf.Sqrt(occluderBounds.SqrDistance(previewCenter));
            if (distanceToOccluderBounds > detectionRadius)
            {
                continue;
            }

            // preview 来源不同，遮挡判定也必须走不同口径：
            // - FarmTool：只看更精确的本地 footprint，避免中心格之外误触发
            // - Placeable / Generic：恢复更贴近玩家视觉的包络，不再退化成“必须真实碰撞体重叠”
            // 注意：预览距离过滤现在按 occluder bounds，而不是 root 点，
            // 这样像房屋/大树这类“大物体边缘靠近，但根点较远”的情况不会被误跳过。

            // ★ 预览遮挡检测：只要 Bounds 有重叠就触发透明
            // 对于预览，minOcclusionRatio = 0（任何遮挡都触发）
            if (occluderBounds.Intersects(detectionBounds))
            {
                previewOccluding.Add(occluder);

                // 设置遮挡状态（如果还没有被玩家遮挡）
                if (!ContainsPhysicalTree(currentlyOccluding, occluder))
                {
                    SetPhysicalTreeOccluding(occluder, true);
                }
            }
        }

        // 恢复不再被预览遮挡的物体（如果也没有被玩家遮挡）
        foreach (var occluder in previousPreviewOccluding)
        {
            if (occluder != null &&
                !ContainsPhysicalTree(previewOccluding, occluder) &&
                !ContainsPhysicalTree(currentlyOccluding, occluder))
            {
                // 如果是树林中的树木，不恢复（由树林逻辑统一管理）
                if (enableForestTransparency && ForestContainsPhysicalTree(occluder))
                {
                    continue;
                }

                SetPhysicalTreeOccluding(occluder, false);
            }
        }

        if (enableDetailedDebug && previewOccluding.Count > 0)
        {
            Debug.Log($"<color=yellow>[预览遮挡] 检测到 {previewOccluding.Count} 个遮挡物</color>");
        }
    }

    private Bounds ExpandPreviewBoundsForOcclusion(Bounds sourceBounds, PreviewOcclusionSource source)
    {
        Bounds expandedBounds = sourceBounds;

        // FarmTool 仍然只认中心格，但补一圈小幅 hover 缓冲，
        // 让资源的可见遮挡面贴近中心格边缘时也能触发，而不是再次退化成必须几乎重合。
        if (source == PreviewOcclusionSource.FarmTool)
        {
            expandedBounds.Expand(new Vector3(FarmToolPreviewHoverExpand, FarmToolPreviewHoverExpand, 0.01f));
        }
        else if (source == PreviewOcclusionSource.PlaceablePlacement ||
                 source == PreviewOcclusionSource.Generic)
        {
            expandedBounds.Expand(new Vector3(PlaceablePreviewHoverExpand, PlaceablePreviewHoverExpand, 0.01f));
        }

        return expandedBounds;
    }

    /// <summary>
    /// ✅ 清除预览遮挡状态
    /// </summary>
    private void ClearPreviewOcclusion()
    {
        foreach (var occluder in previewOccluding)
        {
            if (occluder != null && !ContainsPhysicalTree(currentlyOccluding, occluder))
            {
                // 如果是树林中的树木，不恢复（由树林逻辑统一管理）
                if (enableForestTransparency && ForestContainsPhysicalTree(occluder))
                {
                    continue;
                }

                SetPhysicalTreeOccluding(occluder, false);
            }
        }
        previewOccluding.Clear();
    }

    #endregion

    /// <summary>
    /// 注册可遮挡物体
    /// </summary>
    public void RegisterOccluder(OcclusionTransparency occluder)
    {
        if (occluder != null)
        {
            registeredOccluders.Add(occluder);
        }
    }

    /// <summary>
    /// 注销可遮挡物体
    /// </summary>
    public void UnregisterOccluder(OcclusionTransparency occluder)
    {
        if (occluder != null)
        {
            registeredOccluders.Remove(occluder);
            currentlyOccluding.Remove(occluder);
            currentForest.RemoveWhere(tree => tree == null || tree.SharesOcclusionRoot(occluder));
        }
    }

    /// <summary>
    /// 获取遮挡参数（供OcclusionTransparency初始化使用）
    /// </summary>
    public void GetOcclusionParams(string tag, out float alpha, out float speed)
    {
        alpha = globalOccludedAlpha;
        speed = globalFadeSpeed;

        if (useTagCustomParams && tagParams != null)
        {
            foreach (var param in tagParams)
            {
                if (param.tag == tag)
                {
                    alpha = param.occludedAlpha;
                    speed = param.fadeSpeed;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 检测遮挡
    /// 🔥 v5.1 修复：使用 Bounds.Intersects 替代 Contains，确保外侧树也能触发遮挡
    /// </summary>
    private void DetectOcclusion()
    {
        EnsurePlayerBindings();
        if (playerCollider == null && playerSprite == null) return;

        // 获取玩家中心位置和 Bounds
        Bounds playerVisualBounds = playerSprite != null ? playerSprite.bounds : playerCollider.bounds;
        Bounds playerFootBounds = playerCollider != null ? playerCollider.bounds : playerVisualBounds;
        Vector2 playerCenterPos = playerVisualBounds.center;

        // 交换当前和上一帧的遮挡集合
        var temp = previousOccluding;
        previousOccluding = currentlyOccluding;
        currentlyOccluding = temp;
        currentlyOccluding.Clear();

        bool shouldLog = enableDetailedDebug && (Time.time - lastDebugLogTime > DEBUG_LOG_INTERVAL);
        if (shouldLog) lastDebugLogTime = Time.time;

        int checkedCount = 0;
        int skippedByDistance = 0;
        int skippedByBounds = 0;
        int occludedCount = 0;

        // 遍历所有注册的可遮挡物体
        foreach (var occluder in registeredOccluders)
        {
            if (occluder == null || !occluder.CanBeOccluded) continue;
            checkedCount++;

            // 获取遮挡物的位置（父物体位置 = 种植点）
            Vector2 occluderPos = GetOccluderReferencePosition(occluder);

            // 获取遮挡物的bounds
            Bounds occluderBounds = occluder.GetBounds();

            // 距离过滤：只检测玩家周围的物体（使用玩家中心位置）
            float distance = Vector2.Distance(playerCenterPos, occluderPos);
            if (distance > detectionRadius)
            {
                skippedByDistance++;
                continue;
            }

            // 标签过滤
            if (useTagFilter && !HasAnyTag(occluder))
            {
                continue;
            }

            // Sorting Layer过滤：只检测同一层的物体（可选）
            if (sameSortingLayerOnly && occluder.GetSortingLayerName() != playerLayer)
            {
                continue;
            }

            // 只有“物体在玩家前面”时，玩家重叠才算真实遮挡。
            // 否则站在树/房子前面时也会被误判成需要透明。
            if (!IsOccluderInFrontOfPlayer(occluder, playerFootBounds))
            {
                skippedByBounds++;
                continue;
            }

            // 在前后关系成立后，再判断玩家是否真的与可见遮挡面发生重叠。
            bool isOccluding = false;
            bool usedBoundsFallback = false;
            bool usedSampleFallback = false;
            float preciseSampleRatio = 0f;

            // 优先使用像素采样精确检测
            if (occluder.UsePixelSampling && occluder.IsTextureReadable)
            {
                bool preciseOcclusion = occluder.ContainsPointPrecise(playerCenterPos);
                bool sampledBodyOcclusion = false;
                if (!preciseOcclusion)
                {
                    preciseSampleRatio = occluder.CalculateOcclusionRatioPrecise(playerVisualBounds);
                    sampledBodyOcclusion = preciseSampleRatio > 0f;
                    usedSampleFallback = sampledBodyOcclusion;
                }

                bool boundsOcclusion = occluderBounds.Intersects(playerVisualBounds);

                // 先看中心点精确命中；如果中心点刚好踩空，再补一轮身体多点采样。
                // 只有前两层都没命中时，才最后退回到 visual bounds 重叠兜底。
                usedBoundsFallback = !preciseOcclusion && !sampledBodyOcclusion && boundsOcclusion;
                isOccluding = preciseOcclusion || sampledBodyOcclusion || usedBoundsFallback;
            }
            else
            {
                // 🔥 修复：使用 Bounds.Intersects 替代 Contains
                // 这样即使玩家中心不在树的 Bounds 内，只要有重叠就能检测到
                isOccluding = occluderBounds.Intersects(playerVisualBounds);
            }

            // ✅ 遮挡占比过滤（可选）
            // 如果启用了遮挡占比过滤，需要额外检查被遮挡的面积占比
            if (isOccluding && useOcclusionRatioFilter && !usedBoundsFallback && !usedSampleFallback)
            {
                // 计算被遮挡占比（使用精确或 Bounds 方法）
                float occlusionRatio;
                if (occluder.UsePixelSampling && occluder.IsTextureReadable)
                {
                    occlusionRatio = Mathf.Max(
                        preciseSampleRatio > 0f
                            ? preciseSampleRatio
                            : occluder.CalculateOcclusionRatioPrecise(playerVisualBounds),
                        CalculateOcclusionRatio(playerVisualBounds, occluderBounds));
                }
                else
                {
                    occlusionRatio = CalculateOcclusionRatio(playerVisualBounds, occluderBounds);
                }

                // 检查是否在树林中（如果启用了树林透明功能）
                bool isInForest = enableForestTransparency && ForestContainsPhysicalTree(occluder);

                // 只有满足以下条件之一才触发透明：
                // 1. 被遮挡占比 >= 阈值
                // 2. 在树林中
                if (occlusionRatio < minOcclusionRatio && !isInForest)
                {
                    // 被遮挡不够多，且不在树林中 → 不触发透明
                    isOccluding = false;
                }
            }

            if (isOccluding)
            {
                // 玩家 Bounds 与遮挡物 Bounds 重叠 → 遮挡成立
                currentlyOccluding.Add(occluder);

                // ✅ 根据标签获取自定义参数
                if (useTagCustomParams)
                {
                    TagOcclusionParams customParams = GetTagParams(occluder.gameObject.tag);
                    if (customParams != null)
                    {
                        SetPhysicalTreeOccluding(occluder, true, customParams.occludedAlpha, customParams.fadeSpeed);
                    }
                    else
                    {
                        SetPhysicalTreeOccluding(occluder, true);
                    }
                }
                else
                {
                    SetPhysicalTreeOccluding(occluder, true);
                }

                occludedCount++;
            }
            else
            {
                skippedByBounds++;
            }
        }

        // ✅ 树林整体透明逻辑（使用智能边缘遮挡）
        if (enableForestTransparency && currentlyOccluding.Count > 0)
        {
            // 检测玩家是否被树木遮挡
            OcclusionTransparency occludingTree = null;
            foreach (var occluder in currentlyOccluding)
            {
                if (occluder != null && occluder.IsTreeOccluder())
                {
                    occludingTree = occluder;
                    break;
                }
            }

            if (occludingTree != null)
            {
                // 玩家被树木遮挡 → 检查是否需要更新树林区域
                if (currentForest.Count == 0)
                {
                    // 首次进入树林 → 执行 Flood Fill
                    FindConnectedForest(occludingTree, playerCenterPos);
                    lastOccludingTree = occludingTree;

                    // ✅ 使用智能边缘遮挡
                    HandleForestOcclusion(occludingTree, playerCenterPos, playerVisualBounds);
                }
                else if (!ForestContainsPhysicalTree(occludingTree))
                {
                    // 玩家移动到另一片树林 → 清空缓存，重新 Flood Fill
                    ClearForestTransparency();
                    FindConnectedForest(occludingTree, playerCenterPos);
                    lastOccludingTree = occludingTree;

                    // ✅ 使用智能边缘遮挡
                    HandleForestOcclusion(occludingTree, playerCenterPos, playerVisualBounds);
                }
                else if (!currentForestBounds.Contains(playerCenterPos))
                {
                    // 玩家离开树林边界 → 检查是否还在缓存范围内
                    ClearForestTransparency();
                    FindConnectedForest(occludingTree, playerCenterPos);
                    lastOccludingTree = occludingTree;

                    // ✅ 使用智能边缘遮挡
                    HandleForestOcclusion(occludingTree, playerCenterPos, playerVisualBounds);
                }
                else
                {
                    // 玩家还在同一片树林内 → 使用缓存，但更新边缘遮挡状态
                    // ✅ 每次检测都更新边缘遮挡状态（因为玩家位置变化）
                    HandleForestOcclusion(occludingTree, playerCenterPos, playerVisualBounds);
                }
            }
            else
            {
                // 玩家没有被树木遮挡 → 清空树林缓存
                if (currentForest.Count > 0)
                {
                    ClearForestTransparency();
                }
            }
        }
        else if (currentForest.Count > 0)
        {
            // 功能关闭或没有遮挡物 → 清空树林缓存
            ClearForestTransparency();
        }

        // 恢复不再遮挡的物体（排除树林中的树木）
        foreach (var occluder in previousOccluding)
        {
            if (occluder != null && !ContainsPhysicalTree(currentlyOccluding, occluder))
            {
                // 如果是树林中的树木，不恢复（由树林逻辑统一管理）
                if (enableForestTransparency && ForestContainsPhysicalTree(occluder))
                {
                    continue;
                }

                SetPhysicalTreeOccluding(occluder, false);
            }
        }

        // 简化调试输出：每秒最多一次
        if (shouldLog)
        {
            Debug.Log($"<color=cyan>[遮挡检测] 检查:{checkedCount} | 遮挡:{occludedCount} | 树林:{currentForest.Count} | 跳过:距离={skippedByDistance},Bounds={skippedByBounds} | 玩家中心:{playerCenterPos}</color>");
        }

        // ✅ 同时更新预览遮挡检测
        if (previewBounds != null)
        {
            DetectPreviewOcclusion();
        }
    }

    /// <summary>
    /// 计算被遮挡占比（重叠面积 / 玩家面积）
    /// </summary>
    /// <param name="playerBounds">玩家的 Bounds</param>
    /// <param name="occluderBounds">遮挡物的 Bounds</param>
    /// <returns>被遮挡占比（0-1）</returns>
    private float CalculateOcclusionRatio(Bounds playerBounds, Bounds occluderBounds)
    {
        // 计算重叠区域
        float overlapMinX = Mathf.Max(playerBounds.min.x, occluderBounds.min.x);
        float overlapMaxX = Mathf.Min(playerBounds.max.x, occluderBounds.max.x);
        float overlapMinY = Mathf.Max(playerBounds.min.y, occluderBounds.min.y);
        float overlapMaxY = Mathf.Min(playerBounds.max.y, occluderBounds.max.y);

        float overlapWidth = overlapMaxX - overlapMinX;
        float overlapHeight = overlapMaxY - overlapMinY;

        // 没有重叠
        if (overlapWidth <= 0 || overlapHeight <= 0)
        {
            return 0f;
        }

        // 计算重叠面积
        float overlapArea = overlapWidth * overlapHeight;

        // 计算玩家面积
        float playerArea = playerBounds.size.x * playerBounds.size.y;

        // 避免除以零
        if (playerArea <= 0)
        {
            return 0f;
        }

        // 返回被遮挡占比
        return overlapArea / playerArea;
    }

    /// <summary>
    /// 根据标签获取自定义参数
    /// </summary>
    private TagOcclusionParams GetTagParams(string tag)
    {
        if (tagParams == null || tagParams.Length == 0) return null;

        foreach (var param in tagParams)
        {
            if (param.tag == tag)
                return param;
        }

        return null;
    }

    /// <summary>
    /// 检查物体是否有可遮挡标签
    /// </summary>
    private bool HasAnyTag(OcclusionTransparency occluder)
    {
        if (occludableTags == null || occludableTags.Length == 0) return true;

        string objTag = occluder.gameObject.tag;
        foreach (var tag in occludableTags)
        {
            if (objTag == tag) return true;
        }

        // 检查父物体标签
        Transform parent = occluder.transform.parent;
        while (parent != null)
        {
            string parentTag = parent.gameObject.tag;
            foreach (var tag in occludableTags)
            {
                if (parentTag == tag) return true;
            }
            parent = parent.parent;
        }

        return false;
    }

    /// <summary>
    /// 使用 Flood Fill 查找连通的树林区域
    /// ✅ 核心逻辑：两棵树的 Sprite Bounds 重叠或接触 = 连通
    /// </summary>
    private void FindConnectedForest(OcclusionTransparency startTree, Vector2 playerPos)
    {
        currentForest.Clear();

        Queue<OcclusionTransparency> queue = new Queue<OcclusionTransparency>();
        HashSet<Transform> visitedRoots = new HashSet<Transform>();

        queue.Enqueue(startTree);
        visitedRoots.Add(startTree.GetOcclusionRootTransform());

        int searchCount = 0;
        Bounds forestBounds = default;
        bool hasForestBounds = false;

        while (queue.Count > 0 && searchCount < maxForestSearchDepth)
        {
            OcclusionTransparency current = queue.Dequeue();
            searchCount++;

            // 距离限制：超出最大搜索半径的树木不加入
            float distanceToPlayer = Vector2.Distance(GetOccluderReferencePosition(current), playerPos);
            if (distanceToPlayer > maxForestSearchRadius)
            {
                continue;
            }

            // 加入树林
            currentForest.Add(current);
            SetPhysicalTreeOccluding(current, true);

            // 更新树林整体包围盒
            Bounds colliderBounds = current.GetColliderBounds();
            if (!hasForestBounds)
            {
                forestBounds = colliderBounds;
                hasForestBounds = true;
            }
            else
            {
                forestBounds.Encapsulate(colliderBounds.min);
                forestBounds.Encapsulate(colliderBounds.max);
            }

            // 查找相邻的树木
            foreach (var occluder in registeredOccluders)
            {
                if (occluder == null || !occluder.CanBeOccluded) continue;
                if (!occluder.IsTreeOccluder()) continue;
                if (visitedRoots.Contains(occluder.GetOcclusionRootTransform())) continue;

                // ✅ 核心判定：使用树根距离判断连通
                if (AreTreesConnected(current, occluder))
                {
                    queue.Enqueue(occluder);
                    visitedRoots.Add(occluder.GetOcclusionRootTransform());
                }
            }
        }

        // 计算树林边界（扩展一点，避免频繁重算）
        currentForestBounds = hasForestBounds ? forestBounds : new Bounds(startTree.GetOcclusionRootTransform().position, Vector3.one * 2f);

        // ✅ 计算凸包边界
        CalculateForestBoundary();

        if (enableDetailedDebug)
        {
            Debug.Log($"<color=green>[树林检测] 找到连通树木: {currentForest.Count} 棵 | 搜索次数: {searchCount} | 凸包顶点: {currentForestHull.Count}</color>");
        }
    }

    /// <summary>
    /// 判断两棵树是否连通
    /// ✅ 核心逻辑：满足以下任一条件即为连通
    /// 1. 树根距离近（种植在一起的树林）
    /// 2. 树冠 Bounds 有显著重叠（上下排树冠交叠）
    /// </summary>
    private bool AreTreesConnected(OcclusionTransparency a, OcclusionTransparency b)
    {
        // ========== 条件1：树根距离判定 ==========
        // 树根距离近 = 种植在一起 = 同一片林
        if (a == null || b == null)
        {
            return false;
        }

        if (a.SharesOcclusionRoot(b))
        {
            return false;
        }

        Vector2 rootA = GetTreeRootPosition(a);
        Vector2 rootB = GetTreeRootPosition(b);
        float rootDistance = Vector2.Distance(rootA, rootB);

        // 树根距离在连通范围内 → 直接判定为连通
        if (rootDistance <= rootConnectionDistance)
        {
            return true;
        }

        // ========== 条件2：树冠重叠判定 ==========
        // 用于处理上下两排树（树根距离远，但树冠重叠）
        Bounds boundsA = a.GetBounds();
        Bounds boundsB = b.GetBounds();

        // 计算重叠区域
        float overlapMinX = Mathf.Max(boundsA.min.x, boundsB.min.x);
        float overlapMaxX = Mathf.Min(boundsA.max.x, boundsB.max.x);
        float overlapMinY = Mathf.Max(boundsA.min.y, boundsB.min.y);
        float overlapMaxY = Mathf.Min(boundsA.max.y, boundsB.max.y);

        float overlapWidth = overlapMaxX - overlapMinX;
        float overlapHeight = overlapMaxY - overlapMinY;

        // 没有重叠
        if (overlapWidth <= 0 || overlapHeight <= 0)
        {
            return false;
        }

        // 计算重叠面积
        float overlapArea = overlapWidth * overlapHeight;

        // 计算较小树的面积
        float areaA = boundsA.size.x * boundsA.size.y;
        float areaB = boundsB.size.x * boundsB.size.y;
        float smallerArea = Mathf.Min(areaA, areaB);

        // 重叠面积占较小树面积的比例
        float overlapRatio = overlapArea / smallerArea;

        // 重叠比例超过 15% → 视为树冠交叠，判定为连通
        // 这个阈值可以防止细长树苗和远处物体误判
        return overlapRatio >= 0.15f;
    }

    /// <summary>
    /// 获取树的根部位置（种植点）
    /// </summary>
    private Vector2 GetTreeRootPosition(OcclusionTransparency tree)
    {
        // 优先使用父物体位置（树根）
        if (tree.transform.parent != null)
        {
            return tree.GetOcclusionRootTransform().position;
        }

        // 如果没有父物体，使用 Bounds 底部中心
        Bounds bounds = tree.GetBounds();
        return new Vector2(bounds.center.x, bounds.min.y);
    }

    /// <summary>
    /// 清空树林透明状态
    /// </summary>
    private void ClearForestTransparency()
    {
        foreach (var tree in currentForest)
        {
            if (tree != null)
            {
                SetPhysicalTreeOccluding(tree, false);
            }
        }

        currentForest.Clear();
        lastOccludingTree = null;

        // ✅ 清空凸包缓存
        currentForestHull.Clear();
        forestHullValid = false;
    }

    #region 树林边缘遮挡系统

    /// <summary>
    /// ✅ 计算树林边界（凸包）
    /// 使用树木的 Collider Bounds 计算凸包
    /// </summary>
    private void CalculateForestBoundary()
    {
        if (currentForest.Count < 3)
        {
            // 树木太少，无法形成有效边界
            forestHullValid = false;
            return;
        }

        // 收集所有树木的 Collider 边界
        List<Bounds> boundsList = new List<Bounds>();
        foreach (var tree in currentForest)
        {
            if (tree != null)
            {
                boundsList.Add(tree.GetColliderBounds());
            }
        }

        // 计算凸包
        currentForestHull = ConvexHullCalculator.ComputeConvexHullFromBounds(boundsList);
        forestHullValid = currentForestHull.Count >= 3;

        if (enableDetailedDebug && forestHullValid)
        {
            Debug.Log($"<color=cyan>[树林边界] 计算完成: {currentForestHull.Count} 个顶点</color>");
        }
    }

    /// <summary>
    /// ✅ 判断玩家是否在树林边界内部
    /// </summary>
    private bool IsPlayerInsideForest(Vector2 playerPos)
    {
        if (!forestHullValid || currentForestHull.Count < 3)
        {
            return false;
        }

        return ConvexHullCalculator.IsPointInsideConvexHull(playerPos, currentForestHull);
    }

    /// <summary>
    /// ✅ 获取遮挡方向（树相对于玩家的位置）
    /// </summary>
    private OcclusionDirection GetOcclusionDirection(Vector2 playerPos, Vector2 treePos)
    {
        Vector2 delta = treePos - playerPos;

        // 主要判断 Y 轴方向（上/下）
        // 因为 2D 游戏中，遮挡主要发生在玩家上方的物体
        if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x) * 0.5f)
        {
            return delta.y > 0 ? OcclusionDirection.Top : OcclusionDirection.Bottom;
        }
        else
        {
            return delta.x > 0 ? OcclusionDirection.Right : OcclusionDirection.Left;
        }
    }

    /// <summary>
    /// ✅ 判断树木是否是边界树（最外圈）
    /// 改进版：使用树木的 Sprite Bounds 中心点判断
    /// </summary>
    private bool IsBoundaryTree(OcclusionTransparency tree)
    {
        if (!forestHullValid || currentForestHull.Count < 3)
        {
            return true; // 无法判断，默认是边界树（安全回退）
        }

        // 使用树木碰撞体中心判断边界归属：既保留物理树语义，又避免把边界树误判成内侧树
        Vector2 treeReferencePosition = tree.GetColliderBounds().center;

        // 计算树根到凸包边界的距离
        float distance = ConvexHullCalculator.DistanceToConvexHull(treeReferencePosition, currentForestHull);

        // 以接近树根碰撞体半宽的阈值判定边界树，保持边界树稳定，同时减少内侧树误判
        float boundaryThreshold = 0.75f;

        bool isBoundary = Mathf.Abs(distance) < boundaryThreshold;

        if (enableDetailedDebug)
        {
            Debug.Log($"<color=gray>[边界判定] {tree.name}: 根位置距边界={distance:F2}m, 阈值={boundaryThreshold}m, 是边界树={isBoundary}</color>");
        }

        return isBoundary;
    }

    /// <summary>
    /// ✅ 处理树林遮挡（智能边缘遮挡）v5.1
    /// 核心逻辑：
    /// - 玩家在边界外被遮挡 → 只透明遮挡的树
    /// - 玩家在边界内 → 整片树林透明
    /// - 🔥 保底机制：内侧树触发遮挡 → 直接整林透明
    /// - 🔥 保底机制：多棵树 Bounds 与玩家重叠 → 直接整林透明
    /// </summary>
    private void HandleForestOcclusion(OcclusionTransparency occludingTree, Vector2 playerPos, Bounds playerBounds)
    {
        if (!enableSmartEdgeOcclusion)
        {
            // 未启用智能边缘遮挡，使用原有逻辑（整林透明）
            SetForestTransparent(true);
            return;
        }

        // 确保凸包已计算
        if (!forestHullValid)
        {
            CalculateForestBoundary();
        }

        // 🔥 保底机制1：如果凸包计算失败，直接整林透明
        if (!forestHullValid || currentForestHull.Count < 3)
        {
            SetForestTransparent(true);
            if (enableDetailedDebug)
            {
                Debug.Log($"<color=magenta>[边缘遮挡] 凸包计算失败，保底机制：整林透明</color>");
            }
            return;
        }

        // 🔥 保底机制2（修复版）：检查树林中有多少棵树的 Bounds 与玩家 Bounds 重叠
        // 不再依赖 currentlyOccluding（因为之前的 Contains 检测可能漏掉外侧树）
        // 直接遍历树林中的所有树，检查 Bounds 重叠
        int overlappingTreeCount = 0;
        List<OcclusionTransparency> overlappingTrees = new List<OcclusionTransparency>();

        foreach (var tree in currentForest)
        {
            if (tree == null) continue;

            Bounds treeBounds = tree.GetBounds();
            if (treeBounds.Intersects(playerBounds))
            {
                overlappingTreeCount++;
                overlappingTrees.Add(tree);
            }
        }

        if (overlappingTreeCount >= 2)
        {
            // 多棵树 Bounds 与玩家重叠 → 直接整林透明
            SetForestTransparent(true);

            if (enableDetailedDebug)
            {
                Debug.Log($"<color=magenta>[边缘遮挡] 多棵树 Bounds 重叠（{overlappingTreeCount}棵），保底机制：整林透明</color>");
            }
            return;
        }

        // 🔥 保底机制3：如果触发遮挡的树是内侧树（不是边界树），直接整林透明
        // 因为内侧树触发遮挡意味着玩家已经深入树林内部
        bool isBoundary = IsBoundaryTree(occludingTree);
        if (!isBoundary)
        {
            // 内侧树触发遮挡 → 直接整林透明
            SetForestTransparent(true);

            if (enableDetailedDebug)
            {
                Debug.Log($"<color=magenta>[边缘遮挡] 内侧树触发遮挡，保底机制：整林透明</color>");
            }
            return;
        }

        // 判断玩家是否在树林内部
        bool playerInside = IsPlayerInsideForest(playerPos);

        if (playerInside)
        {
            // 玩家在树林内部 → 整片树林透明
            SetForestTransparent(true);

            if (enableDetailedDebug)
            {
                Debug.Log($"<color=green>[边缘遮挡] 玩家在树林内部，整林透明</color>");
            }
        }
        else
        {
            // 玩家在树林外部，被单棵边界树遮挡 → 只透明该树
            SetSingleTreeTransparent(occludingTree);

            if (enableDetailedDebug)
            {
                OcclusionDirection direction = GetOcclusionDirection(playerPos, GetOccluderReferencePosition(occludingTree));
                Debug.Log($"<color=cyan>[边缘遮挡] 边界树 {direction} 遮挡，只透明单树</color>");
            }
        }
    }

    /// <summary>
    /// ✅ 设置整片树林透明
    /// </summary>
    private void SetForestTransparent(bool transparent)
    {
        foreach (var tree in currentForest)
        {
            if (tree != null)
            {
                SetPhysicalTreeOccluding(tree, transparent);
            }
        }
    }

    /// <summary>
    /// ✅ 只设置单棵树透明（其他树恢复）
    /// </summary>
    private void SetSingleTreeTransparent(OcclusionTransparency targetTree)
    {
        foreach (var tree in currentForest)
        {
            if (tree != null)
            {
                // 只有目标树透明，其他树恢复
                SetPhysicalTreeOccluding(tree, tree.SharesOcclusionRoot(targetTree));
            }
        }
    }

    #endregion

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos || player == null) return;

        // 绘制检测半径
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, detectionRadius);

        // 绘制当前遮挡物
        Gizmos.color = Color.red;
        foreach (var occluder in currentlyOccluding)
        {
            if (occluder != null)
            {
                Bounds bounds = occluder.GetBounds();
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }

        // 绘制树林
        Gizmos.color = Color.green;
        foreach (var tree in currentForest)
        {
            if (tree != null)
            {
                Bounds bounds = tree.GetBounds();
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }

        // 绘制树林边界
        if (currentForest.Count > 0)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(currentForestBounds.center, currentForestBounds.size);

            // ✅ 绘制凸包边界
            if (forestHullValid && currentForestHull.Count >= 3)
            {
                Gizmos.color = Color.magenta;
                for (int i = 0; i < currentForestHull.Count; i++)
                {
                    Vector2 a = currentForestHull[i];
                    Vector2 b = currentForestHull[(i + 1) % currentForestHull.Count];
                    Gizmos.DrawLine(a, b);
                }
            }
        }
    }
}
