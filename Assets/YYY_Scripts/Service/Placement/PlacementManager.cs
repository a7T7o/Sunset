using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using FarmGame.Data;
using FarmGame.Events;
using FarmGame.Farm;
using UnityEngine.SceneManagement;

/// <summary>
/// 放置管理器
/// 核心变化：取消"放置范围"概念，改为"点击锁定 + 走过去放置"
///
/// 状态机：
/// - Idle: 空闲
/// - Preview: 预览跟随鼠标
/// - Locked: 位置锁定
/// - Navigating: 导航中
/// </summary>
public class PlacementManager : MonoBehaviour
{
    private const bool EnableSaplingPlacementProfiling = false;
    private const double SaplingPlacementProfileLogThresholdMs = 20d;

    private static bool SupportsPlacementMode(ItemData item)
    {
        return item != null && (item.isPlaceable || item is SeedData);
    }

    #region 状态枚举

    public enum PlacementState
    {
        Idle,       // 空闲
        Preview,    // 预览跟随鼠标
        Locked,     // 位置锁定
        Navigating  // 导航中
    }

    #endregion

    #region 单例

    public static PlacementManager Instance { get; private set; }

    #endregion

    #region 事件

    public static event Action<PlacementEventData> OnItemPlaced;
    public static event Action<SaplingPlantedEventData> OnSaplingPlanted;
    public static event Action<bool> OnPlacementModeChanged;

    #endregion

    #region 序列化字段

    [Header("━━━━ 组件引用 ━━━━")]
    [SerializeField] private PlacementPreview placementPreview;
    [SerializeField] private PlacementNavigator navigator;
    [SerializeField] private Transform playerTransform;

    [Header("━━━━ 配置 ━━━━")]
    [SerializeField] private string[] obstacleTags = new string[] { "Tree", "Rock", "Building", "Player" };
    [SerializeField] private bool enableLayerCheck = true;

    [Header("━━━━ 排序设置 ━━━━")]
    [SerializeField] private int sortingOrderMultiplier = 100;
    [SerializeField] private int sortingOrderOffset = 0;

    [Header("━━━━ 音效 ━━━━")]
    [SerializeField] private AudioClip placeSuccessSound = null;
    [SerializeField] private AudioClip placeFailSound = null;
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.8f;

    [Header("━━━━ 特效 ━━━━")]
    [SerializeField] private GameObject placeEffectPrefab = null;

    [Header("━━━━ 调试 ━━━━")]
    [SerializeField] private bool showDebugInfo = false;

    #endregion

    #region 私有字段

    private PlacementValidator validator;
    private ItemData currentPlacementItem;
    private int currentItemQuality;
    private PlacementState currentState = PlacementState.Idle;
    private Camera mainCamera;
    private List<CellState> currentCellStates = new List<CellState>();
    private bool hasImmediateOccupiedHoldState;
    private int lastValidationFrame = -1;
    private Vector2Int lastValidatedAnchorCell;
    private ItemData lastValidatedItem;
    private bool lastValidationResult;
    private bool holdPreviewUntilMouseMoves;
    private Vector3 holdPreviewWorldPosition;
    private Vector3 holdPreviewMouseScreenPosition;
    private bool holdPreviewPlayerDominantCellValid;
    private Vector2Int holdPreviewPlayerDominantCell;
    private Vector2 holdPreviewPlayerCenter;
    private bool hasPendingLockedPlacementCommit;
    private int pendingLockedPlacementCommitFrame = -1;
    private bool hasLastNavigationTarget;
    private Vector3 lastNavigationTarget;
    private int cachedScenePropsParentSceneHandle = -1;
    private readonly Dictionary<string, Transform> cachedScenePropsParents = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> cachedMissingScenePropsParents = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private const float ResumePreviewMouseMoveThreshold = 1f;
    private const float ResumePreviewPlayerMoveThreshold = 0.08f;
    // 连放顺延只认本轮刚刚落下的格子，避免把世界里其他已有占位也误当成连放源。
    private bool adjacentContinuationSourceValid;
    private Vector2Int adjacentContinuationSourceCell;
    private const float AdjacentIntentEdgeBandWidth = 0.1f;
    private const float DirectPlacementDominantCellThreshold = 0.6f;

    // 放置历史
    private List<PlacementHistoryEntry> placementHistory = new List<PlacementHistoryEntry>();
    private const int MAX_HISTORY_SIZE = 10;

    // ★ 背包联动相关
    private InventoryService inventoryService;
    private HotbarSelectionService hotbarSelection;
    private PackagePanelTabsUI packageTabs;
    private PlacementSnapshot currentSnapshot;
    private bool wasPanelOpen;

    private static double BeginRealtimeProfileSample()
    {
        return Time.realtimeSinceStartupAsDouble;
    }

    private static bool ShouldProfileSaplingPlacement(ItemData itemData, PlacementType placementType)
    {
        return EnableSaplingPlacementProfiling &&
               Application.isPlaying &&
               (placementType == PlacementType.Sapling || itemData is SaplingData);
    }

    private static void LogSaplingPlacementProfileIfSlow(
        string stage,
        double startedAt,
        Vector3 position,
        string extra = null)
    {
        double elapsedMs = (Time.realtimeSinceStartupAsDouble - startedAt) * 1000d;
        if (elapsedMs < SaplingPlacementProfileLogThresholdMs)
        {
            return;
        }

        string extraSuffix = string.IsNullOrEmpty(extra) ? string.Empty : $" {extra}";
        Debug.LogWarning(
            $"[PlacementSaplingProfile] {stage} took {elapsedMs:F2}ms pos={position}{extraSuffix}");
    }

    #endregion

    #region 放置快照结构体

    /// <summary>
    /// 放置快照 - 记录点击锁定时的物品信息
    /// </summary>
    private struct PlacementSnapshot
    {
        public int itemId;           // 物品 ID
        public int quality;          // 物品品质
        public int slotIndex;        // 背包槽位索引
        public Vector3 lockedPosition; // 锁定的放置位置
        public bool isValid;         // 快照是否有效

        public static PlacementSnapshot Invalid => new PlacementSnapshot { isValid = false };
    }

    #endregion

    #region 属性

    public PlacementState CurrentState => currentState;
    public bool IsPlacementMode => currentState != PlacementState.Idle;
    public ItemData CurrentPlacementItem => currentPlacementItem;
    public bool HasProtectedHeldSession => currentState == PlacementState.Locked || currentState == PlacementState.Navigating || isExecutingPlacement;

    #endregion

    #region Unity 生命周期

    private void Awake()
    {
        // 单例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 初始化验证器
        validator = new PlacementValidator();
        validator.SetObstacleTags(obstacleTags);
        validator.SetEnableLayerCheck(enableLayerCheck);
        validator.SetDebugMode(showDebugInfo);

        mainCamera = Camera.main;
    }

    private void Start()
    {
        // 查找玩家
        if (playerTransform == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("[PlacementManager] 未找到 Player 标签的物体！");
            }
        }

        // 创建预览组件
        if (placementPreview == null)
        {
            GameObject previewObj = new GameObject("PlacementPreview");
            previewObj.transform.SetParent(transform);
            placementPreview = previewObj.AddComponent<PlacementPreview>();
        }

        // 创建导航器
        if (navigator == null)
        {
            GameObject navObj = new GameObject("PlacementNavigator");
            navObj.transform.SetParent(transform);
            navigator = navObj.AddComponent<PlacementNavigator>();
        }

        // 初始化导航器
        navigator.Initialize(playerTransform);
        navigator.OnReachedTarget += OnNavigationReached;
        navigator.OnNavigationCancelled += OnNavigationCancelled;

        // ★ 查找背包相关服务
        if (inventoryService == null)
            inventoryService = PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()
                ?? FindFirstObjectByType<InventoryService>(FindObjectsInactive.Include);
        if (hotbarSelection == null)
            hotbarSelection = PersistentPlayerSceneBridge.GetPreferredRuntimeHotbarSelectionService()
                ?? FindFirstObjectByType<HotbarSelectionService>(FindObjectsInactive.Include);
        if (packageTabs == null)
            packageTabs = PersistentPlayerSceneBridge.GetPreferredRuntimePackageTabs()
                ?? FindFirstObjectByType<PackagePanelTabsUI>(FindObjectsInactive.Include);

        // ★ 订阅手持物品切换事件
        if (hotbarSelection != null)
            hotbarSelection.OnSelectedChanged += OnHotbarSelectionChanged;

        // ★ 订阅背包槽位变化事件（用于检测物品被扣除）
        if (inventoryService != null)
            inventoryService.OnSlotChanged += OnInventorySlotChanged;
    }

    public void RebindRuntimeSceneReferences(
        Transform runtimePlayerTransform,
        InventoryService runtimeInventoryService,
        HotbarSelectionService runtimeHotbarSelection,
        PackagePanelTabsUI runtimePackageTabs = null,
        Camera runtimeWorldCamera = null)
    {
        if (hotbarSelection != null)
        {
            hotbarSelection.OnSelectedChanged -= OnHotbarSelectionChanged;
        }

        if (inventoryService != null)
        {
            inventoryService.OnSlotChanged -= OnInventorySlotChanged;
        }

        playerTransform = runtimePlayerTransform != null
            ? runtimePlayerTransform
            : GameObject.FindGameObjectWithTag("Player")?.transform;

        inventoryService = runtimeInventoryService != null
            ? runtimeInventoryService
            : PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()
                ?? FindFirstObjectByType<InventoryService>(FindObjectsInactive.Include);

        hotbarSelection = runtimeHotbarSelection != null
            ? runtimeHotbarSelection
            : PersistentPlayerSceneBridge.GetPreferredRuntimeHotbarSelectionService()
                ?? FindFirstObjectByType<HotbarSelectionService>(FindObjectsInactive.Include);

        packageTabs = runtimePackageTabs != null
            ? runtimePackageTabs
            : PersistentPlayerSceneBridge.GetPreferredRuntimePackageTabs()
                ?? FindFirstObjectByType<PackagePanelTabsUI>(FindObjectsInactive.Include);

        mainCamera = runtimeWorldCamera != null ? runtimeWorldCamera : Camera.main;

        if (navigator == null)
        {
            GameObject navObj = new GameObject("PlacementNavigator");
            navObj.transform.SetParent(transform);
            navigator = navObj.AddComponent<PlacementNavigator>();
            navigator.OnReachedTarget += OnNavigationReached;
            navigator.OnNavigationCancelled += OnNavigationCancelled;
        }

        navigator.Initialize(playerTransform);

        if (hotbarSelection != null)
        {
            hotbarSelection.OnSelectedChanged += OnHotbarSelectionChanged;
        }

        if (inventoryService != null)
        {
            inventoryService.OnSlotChanged += OnInventorySlotChanged;
        }

        ClearValidationReuseCache();

        if (currentState != PlacementState.Idle && currentPlacementItem != null)
        {
            RefreshCurrentPreview();
        }
    }

    private void Update()
    {
        if (currentState == PlacementState.Idle) return;

        // 检查背包是否打开 - 如果打开则暂停预览更新
        bool isPanelOpen = false;
        if (packageTabs != null)
        {
            isPanelOpen = packageTabs.IsPanelOpen();
        }

        // 背包打开时隐藏预览
        if (isPanelOpen)
        {
            if (!wasPanelOpen && navigator != null && currentState == PlacementState.Navigating)
            {
                navigator.PauseNavigation();
            }

            if (placementPreview != null && placementPreview.gameObject.activeSelf)
            {
                placementPreview.SetVisualVisibility(false);
                if (showDebugInfo)
                    Debug.Log($"<color=yellow>[PlacementManagerV3] 背包打开，隐藏预览</color>");
            }

            // 🔥 Bug E 修复：面板打开 = 暂停，不中断
            // 不调用 HandleInterrupt()，保持 Locked/Navigating 状态
            // 关闭面板后自动恢复
            wasPanelOpen = true;
            return;
        }
        else
        {
            // 背包关闭时恢复预览
            if (wasPanelOpen && navigator != null && currentState == PlacementState.Navigating && navigator.IsPaused)
            {
                navigator.ResumeNavigation();
            }

            if (placementPreview != null && !placementPreview.gameObject.activeSelf && currentState != PlacementState.Idle)
            {
                RefreshCurrentPreview();
                if (showDebugInfo)
                    Debug.Log($"<color=green>[PlacementManagerV3] 背包关闭，恢复预览显示</color>");
            }
        }
        wasPanelOpen = false;

        // ★ 在 Locked/Navigating 状态下检测中断条件
        if (currentState == PlacementState.Locked || currentState == PlacementState.Navigating)
        {
            if (CheckInterruptConditions())
            {
                HandleInterrupt();
                return;
            }

            if (TryExecuteLockedPlacementWhenPlayerIsNear())
            {
                return;
            }
        }

        // 只在 Preview 状态更新预览（跟随鼠标 + 验证格子）
        if (currentState == PlacementState.Preview)
        {
            UpdatePreview();
        }
        // Locked 和 Navigating 状态下预览保持锁定，不需要更新位置
        // 但需要保持 Sorting Layer 同步
        else if (currentState == PlacementState.Locked || currentState == PlacementState.Navigating)
        {
            if (placementPreview != null && playerTransform != null)
            {
                string sortingLayerName = PlacementLayerDetector.GetPlayerSortingLayer(playerTransform);
                placementPreview.UpdateSortingLayer(sortingLayerName);
            }
        }

        // 检查取消
        bool rightClickCancel = Input.GetMouseButtonDown(1) &&
                                !(GameInputManager.Instance != null &&
                                  GameInputManager.Instance.ShouldPreservePlacementModeForCurrentRightClick());
        if (Input.GetKeyDown(KeyCode.Escape) || rightClickCancel)
        {
            OnRightClick();
        }

        // 检查撤销
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        {
            UndoLastPlacement();
        }
    }

    private void OnDestroy()
    {
        if (navigator != null)
        {
            navigator.OnReachedTarget -= OnNavigationReached;
            navigator.OnNavigationCancelled -= OnNavigationCancelled;
        }

        // ★ 取消订阅手持物品切换事件
        if (hotbarSelection != null)
            hotbarSelection.OnSelectedChanged -= OnHotbarSelectionChanged;

        // ★ 取消订阅背包槽位变化事件
        if (inventoryService != null)
            inventoryService.OnSlotChanged -= OnInventorySlotChanged;
    }

    #endregion

    #region 公共方法 - 模式控制

    /// <summary>
    /// 进入放置模式
    /// </summary>
    public void EnterPlacementMode(ItemData item, int quality = 0)
    {
        if (!SupportsPlacementMode(item))
            return;

        ClearValidationReuseCache();
        ClearPostPlacementPreviewHold();
        ClearAdjacentContinuationSource();
        currentPlacementItem = item;
        currentItemQuality = quality;
        ChangeState(PlacementState.Preview);

        // 显示预览
        if (placementPreview != null)
        {
            placementPreview.Show(item);
        }

        OnPlacementModeChanged?.Invoke(true);

        if (showDebugInfo)
            Debug.Log($"<color=green>[PlacementManagerV3] 进入放置模式: {item.itemName}</color>");
    }

    /// <summary>
    /// 退出放置模式
    /// </summary>
    public void ExitPlacementMode()
    {
        ClearValidationReuseCache();
        ClearPostPlacementPreviewHold();
        ClearAdjacentContinuationSource();
        currentPlacementItem = null;
        currentItemQuality = 0;
        currentSnapshot = PlacementSnapshot.Invalid; // ★ 清除快照
        ChangeState(PlacementState.Idle);

        // 隐藏预览
        if (placementPreview != null)
        {
            placementPreview.Hide();
        }

        // 取消导航
        if (navigator != null && navigator.IsNavigating)
        {
            navigator.CancelNavigation();
        }

        OnPlacementModeChanged?.Invoke(false);

        if (showDebugInfo)
            Debug.Log($"<color=yellow>[PlacementManagerV3] 退出放置模式</color>");
    }

    #endregion

    #region 中断检测和处理

    /// <summary>
    /// 手持物品切换回调
    /// </summary>
    private void OnHotbarSelectionChanged(int newIndex)
    {
        // ★ 如果正在执行放置，忽略此回调（避免扣除物品时触发的事件导致中断）
        if (isExecutingPlacement)
        {
            if (showDebugInfo)
                Debug.Log($"<color=yellow>[PlacementManagerV3] 正在执行放置，忽略手持物品切换回调</color>");
            return;
        }

        // 如果正在放置过程中（Locked 或 Navigating），中断并退出
        if (currentState == PlacementState.Locked || currentState == PlacementState.Navigating)
        {
            if (showDebugInfo)
                Debug.Log($"<color=yellow>[PlacementManagerV3] 手持物品切换，中断放置</color>");

            HandleInterrupt();
            ExitPlacementMode();
        }
    }

    /// <summary>
    /// 背包槽位变化回调（用于检测物品被外部扣除）
    /// </summary>
    private void OnInventorySlotChanged(int slotIndex)
    {
        // ★ 如果正在执行放置，忽略此回调（避免我们自己扣除物品时触发的事件导致问题）
        if (isExecutingPlacement)
        {
            if (showDebugInfo)
                Debug.Log($"<color=yellow>[PlacementManagerV3] 正在执行放置，忽略背包槽位变化回调 slotIndex={slotIndex}</color>");
            return;
        }

        // 如果不在放置模式，忽略
        if (currentState == PlacementState.Idle)
            return;

        // 如果变化的槽位不是当前快照的槽位，忽略
        if (!currentSnapshot.isValid || slotIndex != currentSnapshot.slotIndex)
            return;

        // 检查槽位物品是否还有效
        if (inventoryService != null)
        {
            var slot = inventoryService.GetSlot(slotIndex);
            if (slot.IsEmpty || slot.itemId != currentSnapshot.itemId || slot.quality != currentSnapshot.quality)
            {
                if (showDebugInfo)
                    Debug.Log($"<color=yellow>[PlacementManagerV3] 背包槽位物品变化，中断放置</color>");

                // 如果正在 Locked 或 Navigating 状态，中断
                if (currentState == PlacementState.Locked || currentState == PlacementState.Navigating)
                {
                    HandleInterrupt();
                }
            }
        }
    }

    /// <summary>
    /// 检测中断条件
    /// </summary>
    private bool CheckInterruptConditions()
    {
        // 1. 快照失效（物品被移动/使用/数量变化）
        if (!ValidateSnapshot())
        {
            if (showDebugInfo)
                Debug.Log($"<color=yellow>[PlacementManagerV3] 快照失效，中断放置</color>");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 验证快照是否仍然有效
    /// </summary>
    private bool ValidateSnapshot()
    {
        if (!currentSnapshot.isValid) return false;
        if (inventoryService == null) return false;

        var slot = inventoryService.GetSlot(currentSnapshot.slotIndex);
        if (slot.IsEmpty) return false;
        if (slot.itemId != currentSnapshot.itemId) return false;
        if (slot.quality != currentSnapshot.quality) return false;

        return true;
    }

    /// <summary>
    /// 创建放置快照
    /// </summary>
    private PlacementSnapshot CreateSnapshot()
    {
        if (currentPlacementItem == null ||
            hotbarSelection == null ||
            inventoryService == null)
            return PlacementSnapshot.Invalid;

        int sourceSlotIndex = hotbarSelection.GetResolvedPlacementSlotIndex(
            inventoryService,
            inventoryService.Database,
            currentPlacementItem,
            currentItemQuality);

        return new PlacementSnapshot
        {
            itemId = currentPlacementItem.itemID,
            quality = currentItemQuality,
            slotIndex = sourceSlotIndex,
            lockedPosition = placementPreview != null ? placementPreview.LockedPosition : Vector3.zero,
            isValid = true
        };
    }

    /// <summary>
    /// 处理中断 - 取消当前放置进程，恢复到预览跟随鼠标状态
    /// </summary>
    private void HandleInterrupt()
    {
        if (showDebugInfo)
            Debug.Log($"<color=yellow>[PlacementManagerV3] HandleInterrupt 开始, 当前状态={currentState}</color>");

        ClearValidationReuseCache();
        ClearPostPlacementPreviewHold();
        ClearAdjacentContinuationSource();
        ClearLockedPlacementCommitState();
        ClearNavigationRequestState();

        // 取消导航
        if (navigator != null && navigator.IsNavigating)
        {
            navigator.CancelNavigation();
            if (showDebugInfo)
                Debug.Log($"<color=yellow>[PlacementManagerV3] 已取消导航</color>");
        }

        // 清除快照
        currentSnapshot = PlacementSnapshot.Invalid;

        // 解锁预览，恢复跟随鼠标
        if (placementPreview != null)
        {
            placementPreview.UnlockPosition();
            // ★ 确保预览是激活的
            if (!placementPreview.gameObject.activeSelf)
            {
                placementPreview.gameObject.SetActive(true);
            }
            if (showDebugInfo)
                Debug.Log($"<color=yellow>[PlacementManagerV3] 已解锁预览位置</color>");
        }

        // 返回 Preview 状态（如果还在放置模式且有物品）
        if (currentState != PlacementState.Idle && currentPlacementItem != null)
        {
            ChangeState(PlacementState.Preview);
            if (showDebugInfo)
                Debug.Log($"<color=green>[PlacementManagerV3] 已恢复到 Preview 状态，预览将跟随鼠标</color>");
        }
        else
        {
            if (showDebugInfo)
                Debug.Log($"<color=yellow>[PlacementManagerV3] 无法恢复 Preview 状态: currentState={currentState}, currentPlacementItem={currentPlacementItem?.itemName ?? "null"}</color>");
        }
    }

    /// <summary>
    /// 🔥 Bug D 修复：外部调用的中断接口（供 GameInputManager 的 WASD 检测使用）
    /// </summary>
    public void InterruptFromExternal()
    {
        if (currentState == PlacementState.Locked || currentState == PlacementState.Navigating)
        {
            HandleInterrupt();
        }
    }

    public void HandleManualMovementWhileLocked()
    {
        if (currentState != PlacementState.Locked && currentState != PlacementState.Navigating)
        {
            return;
        }

        if (placementPreview == null)
        {
            HandleInterrupt();
            return;
        }

        Vector3 currentCandidatePosition = ResolvePreviewCandidatePosition(GetMouseWorldPosition());
        if (!IsSamePlacementCandidate(placementPreview.LockedPosition, currentCandidatePosition))
        {
            HandleInterrupt();
            return;
        }

        if (navigator != null && navigator.IsNavigating)
        {
            navigator.CancelNavigation();
            ChangeState(PlacementState.Locked);
        }
    }

    public void RefreshCurrentPreview()
    {
        if (currentState == PlacementState.Idle || currentPlacementItem == null || placementPreview == null)
            return;

        if (!placementPreview.gameObject.activeSelf ||
            placementPreview.NeedsVisualRebuildFor(currentPlacementItem))
        {
            placementPreview.RestoreVisualStateIfNeeded();

            if (!placementPreview.gameObject.activeSelf ||
                placementPreview.NeedsVisualRebuildFor(currentPlacementItem))
            {
                placementPreview.Show(currentPlacementItem);
            }
        }

        if (currentState == PlacementState.Preview)
        {
            UpdatePreview();
            return;
        }

        if (playerTransform != null)
        {
            string sortingLayerName = PlacementLayerDetector.GetPlayerSortingLayer(playerTransform);
            placementPreview.UpdateSortingLayer(sortingLayerName);
        }

        if (currentCellStates != null)
        {
            placementPreview.UpdateCellStates(currentCellStates);
        }
    }

    public void SetPreviewVisualVisibility(bool visible)
    {
        if (placementPreview == null || currentState == PlacementState.Idle)
        {
            return;
        }

        if (visible)
        {
            RefreshCurrentPreview();
            return;
        }

        placementPreview.SetVisualVisibility(visible);
    }

    private PlacementExecutionTransaction BeginPlacementTransaction(Vector3 lockedPosition)
    {
        int slotIndex = currentSnapshot.isValid ? currentSnapshot.slotIndex : -1;
        return new PlacementExecutionTransaction(currentPlacementItem, currentItemQuality, lockedPosition, slotIndex, LogPlacementTransaction);
    }

    private void LogPlacementTransaction(string message)
    {
        if (showDebugInfo)
        {
            Debug.Log($"<color=#9aa6b2>[PlacementManagerV3] {message}</color>");
        }
    }

    private void RollbackPlacementTransaction(PlacementExecutionTransaction transaction, string reason, bool restoreInventory, Action rollbackOccupancy = null)
    {
        Action restoreInventoryAction = null;
        if (restoreInventory && transaction != null)
        {
            restoreInventoryAction = () => ReturnItemToInventory(transaction.ItemId, transaction.Quality);
        }

        transaction?.Rollback(reason, restoreInventoryAction, rollbackOccupancy);
        PlaySound(placeFailSound);
        isExecutingPlacement = false;
        HandleInterrupt();
    }

    #endregion

    #region 状态机

    /// <summary>
    /// 改变状态
    /// </summary>
    private void ChangeState(PlacementState newState)
    {
        if (currentState == newState) return;

        PlacementState oldState = currentState;
        currentState = newState;

        if (showDebugInfo)
            Debug.Log($"<color=cyan>[PlacementManagerV3] 状态变化: {oldState} → {newState}</color>");
    }

    #endregion

    #region 预览更新

    private void EnsureValidatorInitialized()
    {
        if (validator != null)
        {
            return;
        }

        validator = new PlacementValidator();
        validator.SetObstacleTags(obstacleTags);
        validator.SetEnableLayerCheck(enableLayerCheck);
        validator.SetDebugMode(showDebugInfo);
    }

    private List<CellState> ValidateCurrentPlacementAt(Vector3 previewPosition)
    {
        EnsureValidatorInitialized();

        if (placementPreview == null || currentPlacementItem == null)
        {
            return new List<CellState>();
        }

        if (currentPlacementItem is SaplingData saplingData)
        {
            return new List<CellState> { validator.ValidateSaplingPlacement(saplingData, previewPosition, playerTransform) };
        }

        if (currentPlacementItem is SeedData seedData)
        {
            bool seedValid = PlacementValidator.ValidateSeedPlacement(seedData, previewPosition);
            return new List<CellState>
            {
                new CellState(Vector2Int.zero, seedValid, seedValid ? InvalidReason.None : InvalidReason.HasObstacle)
            };
        }

        var cellStates = validator.ValidateCells(previewPosition, placementPreview.GridSize, playerTransform, currentPlacementItem);
        if (currentPlacementItem is PlaceableItemData placeableItem &&
            validator.AreAllCellsValid(cellStates) &&
            !placeableItem.CanPlaceAt(previewPosition))
        {
            var invalidStates = new List<CellState>(cellStates.Count);
            foreach (var state in cellStates)
            {
                invalidStates.Add(new CellState(state.gridPosition, false, InvalidReason.HasObstacle));
            }
            return invalidStates;
        }

        return cellStates;
    }

    private bool RefreshPlacementValidationAt(Vector3 worldPosition, bool updatePreviewPosition)
    {
        EnsureValidatorInitialized();

        if (placementPreview == null)
        {
            currentCellStates = new List<CellState>();
            ClearValidationReuseCache();
            return false;
        }

        if (updatePreviewPosition)
        {
            placementPreview.ForceUpdatePosition(worldPosition);
        }

        Vector3 previewPosition = placementPreview.GetPreviewPosition();
        if (TryReuseCurrentFrameValidation(previewPosition, out bool cachedValid))
        {
            return cachedValid;
        }

        currentCellStates = ValidateCurrentPlacementAt(previewPosition);
        placementPreview.UpdateCellStates(currentCellStates);
        hasImmediateOccupiedHoldState = false;

        bool allValid = currentCellStates.Count > 0 && validator.AreAllCellsValid(currentCellStates);
        StoreValidationReuseCache(previewPosition, allValid);
        return allValid;
    }

    private bool TryExecuteLockedPlacement(bool skipValidation = false)
    {
        if (placementPreview == null)
        {
            return false;
        }

        if (!skipValidation &&
            !RefreshPlacementValidationAt(placementPreview.LockedPosition, false))
        {
            if (showDebugInfo)
                Debug.Log($"<color=yellow>[PlacementManagerV3] 锁定位置重验失败，恢复预览跟随</color>");

            PlaySound(placeFailSound);
            HandleInterrupt();
            return false;
        }

        ExecutePlacement();
        ClearLockedPlacementCommitState();
        return true;
    }

    private bool TryExecuteLockedPlacementWhenPlayerIsNear()
    {
        if (placementPreview == null ||
            currentState == PlacementState.Idle ||
            !currentSnapshot.isValid ||
            isExecutingPlacement)
        {
            return false;
        }

        if (IsLockedPlacementCommitAlreadyIssuedThisFrame())
        {
            return false;
        }

        Bounds playerBounds = GetPlayerBounds();
        Bounds previewBounds = placementPreview.GetPreviewBounds();
        if (navigator != null && !navigator.CheckReached(playerBounds, previewBounds))
        {
            return false;
        }

        if (navigator != null && navigator.IsNavigating)
        {
            navigator.CancelNavigation();
        }

        if (showDebugInfo)
        {
            Debug.Log("<color=green>[PlacementManagerV3] 玩家已进入放置有效距离，直接提交锁定放置</color>");
        }

        MarkLockedPlacementCommitIssued();
        return TryExecuteLockedPlacement();
    }

    /// <summary>
    /// 更新预览位置和状态
    /// </summary>
    private void UpdatePreview()
    {
        if (placementPreview == null || currentPlacementItem == null) return;

        Vector3 mousePos = GetMouseWorldPosition();
        Vector3 candidatePreviewPosition = ResolvePreviewCandidatePosition(mousePos);
        bool holdingAtLastPlacement = ShouldHoldPreviewAtLastPlacement(candidatePreviewPosition);

        if (holdingAtLastPlacement)
        {
            placementPreview.ForceUpdatePosition(holdPreviewWorldPosition);
        }
        else
        {
            // 更新预览位置（锁定状态下不会更新）
            placementPreview.UpdatePosition(candidatePreviewPosition);
        }

        // 同步 Sorting Layer
        if (playerTransform != null)
        {
            string sortingLayerName = PlacementLayerDetector.GetPlayerSortingLayer(playerTransform);
            placementPreview.UpdateSortingLayer(sortingLayerName);
        }

        if (holdingAtLastPlacement &&
            currentPlacementItem is SaplingData &&
            (hasImmediateOccupiedHoldState || TryApplyImmediateOccupiedHoldState(holdPreviewWorldPosition)))
        {
            return;
        }

        Vector3 previewPos = placementPreview.GetPreviewPosition();
        bool allValid = RefreshPlacementValidationAt(previewPos, false);

        if (showDebugInfo)
        {
            Debug.Log($"<color=cyan>[PlacementManagerV3] UpdatePreview: pos={previewPos}, allValid={allValid}, isSapling={currentPlacementItem is SaplingData}</color>");
        }
    }

    #endregion

    #region 输入处理

    /// <summary>
    /// 处理左键点击
    /// </summary>
    public void OnLeftClick()
    {
        // 检查是否在 UI 上
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (showDebugInfo)
            Debug.Log($"<color=cyan>[PlacementManagerV3] OnLeftClick, 当前状态: {currentState}</color>");

        if (currentState == PlacementState.Preview)
        {
            // Preview 状态：检查是否全绿，是则锁定位置
            Vector3 previewPosition = placementPreview.GetPreviewPosition();
            bool allValid = TryReuseCurrentFrameValidation(previewPosition, out bool cachedValid)
                ? cachedValid
                : RefreshPlacementValidationAt(previewPosition, false);

            if (allValid)
            {
                if (showDebugInfo)
                    Debug.Log($"<color=green>[PlacementManagerV3] 所有格子有效，锁定位置</color>");
                LockPreviewPosition(skipValidation: true);
            }
            else
            {
                if (showDebugInfo)
                    Debug.Log($"<color=red>[PlacementManagerV3] 有无效格子，无法放置</color>");
            }
        }
        else if (currentState == PlacementState.Navigating)
        {
            // Navigating 状态：点击新位置，需要先验证新位置
            Vector3 mousePos = GetMouseWorldPosition();
            Vector3 candidatePosition = ResolvePreviewCandidatePosition(mousePos);

            // 取消当前导航
            navigator.CancelNavigation();

            // ★ 清除旧快照
            currentSnapshot = PlacementSnapshot.Invalid;

            // 解锁并更新到新位置
            placementPreview.UnlockPosition();

            if (RefreshPlacementValidationAt(candidatePosition, true))
            {
                if (showDebugInfo)
                    Debug.Log($"<color=cyan>[PlacementManagerV3] 导航中点击新位置，重新导航</color>");

                // 锁定新位置（会创建新快照）
                LockPreviewPosition(skipValidation: true);
            }
            else
            {
                // 🔥 Bug G 修复：导航中点击红色位置 → 取消导航 → 恢复跟随
                if (showDebugInfo)
                    Debug.Log($"<color=yellow>[PlacementManagerV3] 导航中点击无效位置，取消导航恢复跟随</color>");
                HandleInterrupt();
            }
        }
    }

    /// <summary>
    /// 处理右键/ESC
    /// </summary>
    public void OnRightClick()
    {
        if (currentState == PlacementState.Navigating)
        {
            // 导航中：取消导航，回到 Preview 状态
            navigator.CancelNavigation();

            // ★ 清除快照
            currentSnapshot = PlacementSnapshot.Invalid;

            if (placementPreview != null)
            {
                placementPreview.UnlockPosition();
            }

            ChangeState(PlacementState.Preview);
        }
        else if (currentState == PlacementState.Preview || currentState == PlacementState.Locked)
        {
            // Preview/Locked 状态：退出放置模式
            ExitPlacementMode();
        }
    }

    #endregion

    #region 位置锁定和导航

    /// <summary>
    /// 锁定预览位置并开始导航
    /// </summary>
    private void LockPreviewPosition(bool skipValidation = false)
    {
        if (placementPreview == null) return;
        if (!skipValidation &&
            !RefreshPlacementValidationAt(placementPreview.GetPreviewPosition(), false))
        {
            if (showDebugInfo)
                Debug.Log($"<color=red>[PlacementManagerV3] 锁定前重验失败，取消本次锁定</color>");
            return;
        }

        // 锁定位置
        placementPreview.LockPosition();
        ChangeState(PlacementState.Locked);

        // ★ 创建放置快照
        currentSnapshot = CreateSnapshot();
        if (!currentSnapshot.isValid)
        {
            if (showDebugInfo)
                Debug.Log($"<color=red>[PlacementManagerV3] 创建快照失败，取消放置</color>");
            HandleInterrupt();
            return;
        }

        // 检查是否已经在目标附近
        Bounds playerBounds = GetPlayerBounds();
        Bounds previewBounds = placementPreview.GetPreviewBounds();
        Bounds visualBounds = placementPreview.GetVisualPreviewBounds();

        if (showDebugInfo)
        {
            Debug.Log($"<color=cyan>[PlacementManagerV3] LockPreviewPosition: playerBounds={playerBounds}, interactionBounds={previewBounds}, visualBounds={visualBounds}</color>");
        }

        ClearLockedPlacementCommitState();
        ClearNavigationRequestState();

        if (ShouldDirectPlaceFromPlayerNeighborhood())
        {
            if (showDebugInfo)
            {
                Debug.Log("<color=green>[PlacementManagerV3] 当前目标位于玩家主占格 3x3 内，直接放置</color>");
            }

            TryExecuteLockedPlacement(skipValidation: true);
            return;
        }

        if (navigator.IsAlreadyNearTarget(playerBounds, previewBounds))
        {
            // 已经在附近，直接放置
            if (showDebugInfo)
                Debug.Log($"<color=green>[PlacementManagerV3] 玩家已在目标附近，直接放置</color>");

            TryExecuteLockedPlacement(skipValidation: true);
        }
        else
        {
            // 需要导航
            StartNavigation();
        }
    }

    /// <summary>
    /// 开始导航
    /// </summary>
    private void StartNavigation()
    {
        if (navigator == null || placementPreview == null) return;

        Bounds playerBounds = GetPlayerBounds();
        Bounds previewBounds = placementPreview.GetPreviewBounds();
        Bounds visualBounds = placementPreview.GetVisualPreviewBounds();

        // 计算导航目标点
        Vector3 targetPos = navigator.CalculateNavigationTarget(playerBounds, previewBounds);

        if (hasLastNavigationTarget &&
            navigator.IsNavigating &&
            IsSamePlacementCandidate(targetPos, lastNavigationTarget))
        {
            if (showDebugInfo)
            {
                Debug.Log($"<color=cyan>[PlacementManagerV3] StartNavigation 跳过重复导航目标: {targetPos}</color>");
            }
            return;
        }

        // 开始导航
        lastNavigationTarget = targetPos;
        hasLastNavigationTarget = true;
        navigator.StartNavigation(targetPos, previewBounds);
        ChangeState(PlacementState.Navigating);

        if (showDebugInfo)
            Debug.Log($"<color=cyan>[PlacementManagerV3] 开始导航到: {targetPos}, interactionBounds={previewBounds}, visualBounds={visualBounds}</color>");
    }

    /// <summary>
    /// 导航到达回调
    /// </summary>
    private void OnNavigationReached()
    {
        if (showDebugInfo)
            Debug.Log($"<color=green>[PlacementManagerV3] 到达目标，执行锁定位置重验</color>");

        if (IsLockedPlacementCommitAlreadyIssuedThisFrame())
        {
            return;
        }

        MarkLockedPlacementCommitIssued();
        TryExecuteLockedPlacement();
    }

    /// <summary>
    /// 导航取消回调
    /// </summary>
    private void OnNavigationCancelled()
    {
        if (showDebugInfo)
            Debug.Log($"<color=yellow>[PlacementManagerV3] 导航已取消</color>");
    }

    #endregion

    #region 放置执行

    // ★ 放置执行中标志，防止在扣除物品时被 HotbarSelectionService 中断
    private bool isExecutingPlacement = false;

    /// <summary>
    /// 执行放置操作
    /// </summary>
    private void ExecutePlacement()
    {
        if (placementPreview == null || currentPlacementItem == null) return;

        // ★ 设置执行中标志
        isExecutingPlacement = true;

        if (showDebugInfo)
            Debug.Log($"<color=cyan>[PlacementManagerV3] ExecutePlacement 开始, item={currentPlacementItem.itemName}</color>");

        Vector3 position = placementPreview.LockedPosition;
        PlacementExecutionTransaction transaction = BeginPlacementTransaction(position);

        // ★ 验证快照有效性
        if (!ValidateSnapshot())
        {
            if (showDebugInfo)
                Debug.Log($"<color=red>[PlacementManagerV3] 快照验证失败，取消放置</color>");
            RollbackPlacementTransaction(transaction, "快照验证失败", false);
            return;
        }

        // 🔴 补丁005 B.4.2：种子专用分支路由（不走通用放置流程）
        if (currentPlacementItem is SeedData seedData)
        {
            ExecuteSeedPlacement(seedData);
            return;
        }

        // 检查 PlaceableItemData 的自定义验证
        if (currentPlacementItem is PlaceableItemData placeableItem)
        {
            if (!placeableItem.CanPlaceAt(position))
            {
                RollbackPlacementTransaction(transaction, "自定义放置验证失败", false);
                return;
            }
        }

        // ★★★ 在扣除物品之前，先保存所有需要的数据 ★★★
        // 因为扣除物品后，HotbarSelectionService 可能会触发 ExitPlacementMode，清空 currentPlacementItem
        ItemData savedItemData = currentPlacementItem;
        int savedItemId = currentPlacementItem.itemID;
        int savedQuality = currentItemQuality;
        PlacementType savedPlacementType = currentPlacementItem.placementType;
        string savedItemName = currentPlacementItem.itemName;
        bool shouldProfileSaplingPlacement = ShouldProfileSaplingPlacement(savedItemData, savedPlacementType);
        double placementTotalStart = shouldProfileSaplingPlacement ? BeginRealtimeProfileSample() : 0d;

        if (showDebugInfo)
            Debug.Log($"<color=cyan>[PlacementManagerV3] 保存物品数据: id={savedItemId}, name={savedItemName}, type={savedPlacementType}</color>");

        // 实例化预制体
        double instantiateStart = shouldProfileSaplingPlacement ? BeginRealtimeProfileSample() : 0d;
        GameObject placedObject = InstantiatePlacementPrefab(position);
        if (shouldProfileSaplingPlacement)
        {
            LogSaplingPlacementProfileIfSlow(
                "InstantiatePlacementPrefab",
                instantiateStart,
                position,
                $"prefab={(savedItemData != null && savedItemData.placementPrefab != null ? savedItemData.placementPrefab.name : "<null>")} placementType={savedPlacementType} itemType={savedItemData?.GetType().Name}");
        }

        if (placedObject == null)
        {
            RollbackPlacementTransaction(transaction, "实例化放置预制体失败", false);
            return;
        }
        transaction.MarkSpawned(placedObject);

        if (showDebugInfo)
            Debug.Log($"<color=cyan>[PlacementManagerV3] 预制体实例化成功: {placedObject.name}</color>");

        try
        {
            double visualPrepStart = shouldProfileSaplingPlacement ? BeginRealtimeProfileSample() : 0d;
            // 获取目标 Layer
            int targetLayer = PlacementLayerDetector.GetLayerAtPosition(position);

            // 同步 Layer
            SyncLayerToPlacedObject(placedObject, targetLayer);

            // 设置排序 Order
            SetSortingOrder(placedObject, position);
            transaction.MarkVisualReady();
            if (shouldProfileSaplingPlacement)
            {
                LogSaplingPlacementProfileIfSlow(
                    "PreparePlacedVisuals",
                    visualPrepStart,
                    position,
                    $"object={placedObject.name}");
            }
        }
        catch (Exception ex)
        {
            RollbackPlacementTransaction(transaction, $"放置物视觉准备失败: {ex.Message}", false);
            return;
        }

        // ★ 扣除背包物品（使用快照数据）
        // 注意：这一步可能触发 HotbarSelectionService.OnSlotChanged，进而调用 ExitPlacementMode
        // 但因为我们已经保存了数据，所以不会出错
        if (showDebugInfo)
            Debug.Log($"<color=yellow>[PlacementManagerV3] 准备扣除物品，slotIndex={currentSnapshot.slotIndex}</color>");

        double deductStart = shouldProfileSaplingPlacement ? BeginRealtimeProfileSample() : 0d;
        if (!DeductItemFromInventory())
        {
            if (showDebugInfo)
                Debug.Log($"<color=red>[PlacementManagerV3] 背包扣除失败，取消放置</color>");
            RollbackPlacementTransaction(transaction, "背包扣除失败", false);
            return;
        }
        if (shouldProfileSaplingPlacement)
        {
            LogSaplingPlacementProfileIfSlow(
                "DeductItemFromInventory",
                deductStart,
                position,
                $"slot={currentSnapshot.slotIndex}");
        }
        transaction.MarkInventoryCommitted();

        if (showDebugInfo)
            Debug.Log($"<color=green>[PlacementManagerV3] 背包扣除成功</color>");

        SaplingPlantedEventData? saplingEventData = null;
        double saplingPrepareStart = shouldProfileSaplingPlacement ? BeginRealtimeProfileSample() : 0d;
        if (savedPlacementType == PlacementType.Sapling &&
            !TryPrepareSaplingPlacement(position, placedObject, savedItemData, out saplingEventData))
        {
            RollbackPlacementTransaction(transaction, "树苗落地确认失败", true);
            return;
        }
        if (shouldProfileSaplingPlacement)
        {
            LogSaplingPlacementProfileIfSlow(
                "TryPrepareSaplingPlacement",
                saplingPrepareStart,
                position,
                $"object={placedObject.name}");
        }

        try
        {
            double commitEventStart = shouldProfileSaplingPlacement ? BeginRealtimeProfileSample() : 0d;
            // ★ 使用保存的数据创建事件数据（不再依赖 currentPlacementItem）
            var eventData = new PlacementEventData(
                position,
                savedItemData,
                placedObject,
                savedPlacementType
            );

            OnItemPlaced?.Invoke(eventData);

            if (saplingEventData.HasValue)
            {
                OnSaplingPlanted?.Invoke(saplingEventData.Value);
            }

            transaction.MarkOccupancyCommitted();
            AddToHistoryWithSavedData(eventData, savedItemId, savedQuality);
            if (shouldProfileSaplingPlacement)
            {
                LogSaplingPlacementProfileIfSlow(
                    "CommitPlacementEventsAndHistory",
                    commitEventStart,
                    position);
            }
        }
        catch (Exception ex)
        {
            RollbackPlacementTransaction(transaction, $"放置提交异常: {ex.Message}", true);
            return;
        }

        // 播放音效和特效。树苗这条链已经把重活尽量后移，
        // 这里再把纯视觉特效顺到下一帧，避免和树首帧初始化挤在同一帧峰值上。
        PlaySound(placeSuccessSound);
        if (savedPlacementType == PlacementType.Sapling)
        {
            StartCoroutine(PlayPlaceEffectNextFrame(position));
        }
        else
        {
            PlayPlaceEffect(position);
        }

        if (showDebugInfo)
            Debug.Log($"<color=green>[PlacementManagerV3] 放置成功: {savedItemName}, tx#{transaction.TransactionId}</color>");

        // ★ 检查是否还有物品（使用快照数据）
        bool hasMore = HasMoreItems();

        if (showDebugInfo)
            Debug.Log($"<color=cyan>[PlacementManagerV3] 检查剩余物品: hasMore={hasMore}</color>");

        // ★ 清除执行中标志
        isExecutingPlacement = false;

        if (!hasMore)
        {
            if (showDebugInfo)
                Debug.Log($"<color=yellow>[PlacementManagerV3] 物品用完，退出放置模式</color>");
            ExitPlacementMode();
        }
        else
        {
            // 还有物品，回到 Preview 状态
            if (showDebugInfo)
                Debug.Log($"<color=cyan>[PlacementManagerV3] 还有物品，回到 Preview 状态</color>");

            double resumePreviewStart = shouldProfileSaplingPlacement ? BeginRealtimeProfileSample() : 0d;
            ResumePreviewAfterSuccessfulPlacement();
            if (shouldProfileSaplingPlacement)
            {
                LogSaplingPlacementProfileIfSlow(
                    "ResumePreviewAfterSuccessfulPlacement",
                    resumePreviewStart,
                    position);
            }
        }

        if (shouldProfileSaplingPlacement)
        {
            LogSaplingPlacementProfileIfSlow(
                "ExecutePlacementTotal",
                placementTotalStart,
                position,
                $"stateAfter={currentState}");
        }
    }

    /// <summary>
    /// 🔴 补丁005 B.4.1：种子专用放置执行（从 GameInputManager.ExecutePlantSeed 完整迁移）
    /// 不走通用放置流程：用 cropPrefab 不是 placementPrefab，物品扣除走 SeedBagHelper，
    /// 需要 CropController.Initialize + tileData.SetCropData 农田数据绑定。
    /// </summary>
    private void ExecuteSeedPlacement(SeedData seedData)
    {
        if (placementPreview == null || seedData == null) return;

        isExecutingPlacement = true;

        Vector3 position = placementPreview.LockedPosition;

        // 获取农田系统数据
        var farmTileManager = FarmTileManager.Instance;
        if (farmTileManager == null)
        {
            if (showDebugInfo)
                Debug.Log("<color=red>[PlacementManager] ExecuteSeedPlacement: FarmTileManager 未初始化</color>");
            isExecutingPlacement = false;
            HandleInterrupt();
            return;
        }

        if (seedData.cropPrefab == null)
        {
            Debug.LogError($"[PlacementManager] 种子 {seedData.itemName} 的 cropPrefab 为空");
            isExecutingPlacement = false;
            HandleInterrupt();
            return;
        }

        // layerIndex 获取（与 FarmToolPreview.UpdateSeedPreview 同源）
        int layerIndex = farmTileManager.GetCurrentLayerIndex(position);
        var tilemaps = farmTileManager.GetLayerTilemaps(layerIndex);
        if (tilemaps == null || !tilemaps.IsValid())
        {
            if (showDebugInfo)
                Debug.Log($"<color=red>[PlacementManager] 楼层 {layerIndex} 的 Tilemap 未配置</color>");
            isExecutingPlacement = false;
            HandleInterrupt();
            return;
        }

        Vector3Int cellPos = tilemaps.WorldToCell(position);

        // 二次验证：耕地数据 + 可种植
        var tileData = farmTileManager.GetTileData(layerIndex, cellPos);
        if (tileData == null || !tileData.CanPlant())
        {
            if (showDebugInfo)
                Debug.Log($"<color=red>[PlacementManager] 无法在此位置种植: {cellPos}</color>");
            PlaySound(placeFailSound);
            isExecutingPlacement = false;
            HandleInterrupt();
            return;
        }

        // 季节检查
        var timeManager = TimeManager.Instance;
        if (timeManager != null && seedData.season != Season.AllSeason)
        {
            var currentSeason = timeManager.GetSeason();
            if ((int)seedData.season != (int)currentSeason)
            {
                if (showDebugInfo)
                    Debug.Log($"<color=red>[PlacementManager] {seedData.itemName} 不适合当前季节</color>");
                PlaySound(placeFailSound);
                isExecutingPlacement = false;
                HandleInterrupt();
                return;
            }
        }

        // 快照验证
        if (!ValidateSnapshot())
        {
            isExecutingPlacement = false;
            HandleInterrupt();
            return;
        }

        // 从种子袋消耗一颗种子（走 SeedBagHelper 保质期链路）
        int consumedSlotIndex = currentSnapshot.slotIndex;
        var seedItem = inventoryService.GetInventoryItem(consumedSlotIndex);
        if (seedItem == null || seedItem.IsEmpty)
        {
            if (showDebugInfo)
                Debug.Log($"<color=red>[PlacementManager] 背包中没有种子: {seedData.itemName}</color>");
            isExecutingPlacement = false;
            HandleInterrupt();
            return;
        }

        int currentTotalDays = timeManager?.GetTotalDaysPassed() ?? 0;

        // 自动初始化未初始化的种子袋
        if (!SeedBagHelper.IsSeedBag(seedItem))
        {
            SeedBagHelper.InitializeSeedBag(seedItem, seedData, currentTotalDays);
        }

        // 检查是否过期
        if (SeedBagHelper.IsExpired(seedItem, currentTotalDays))
        {
            if (showDebugInfo)
                Debug.Log($"<color=red>[PlacementManager] 种子袋已过期: {seedData.itemName}</color>");
            PlaySound(placeFailSound);
            isExecutingPlacement = false;
            ExitPlacementMode();
            return;
        }

        int remaining = SeedBagHelper.ConsumeSeed(seedItem, seedData, currentTotalDays);
        if (remaining < 0)
        {
            if (showDebugInfo)
                Debug.Log($"<color=red>[PlacementManager] 种子消耗失败: {seedData.itemName}</color>");
            isExecutingPlacement = false;
            HandleInterrupt();
            return;
        }

        // 种子袋用完，清除槽位
        if (remaining <= 0)
        {
            inventoryService.ClearSlot(consumedSlotIndex);
        }

        // 实例化作物
        int currentDay = timeManager?.GetTotalDaysPassed() ?? 0;
        Vector3 cropWorldPos = tilemaps.GetCellCenterWorld(cellPos);
        Transform container = tilemaps.propsContainer;

        GameObject cropObj = Instantiate(seedData.cropPrefab, cropWorldPos, Quaternion.identity, container);
        cropObj.name = $"Crop_{seedData.itemName}_{cellPos}";

        var controller = cropObj.GetComponentInChildren<CropController>();
        if (controller == null)
        {
            Debug.LogError($"[PlacementManager] 作物预制体缺少 CropController: {seedData.itemName}");
            Destroy(cropObj);

            // 退还种子
            if (seedItem != null && !seedItem.IsEmpty)
            {
                int curRemaining = SeedBagHelper.GetRemaining(seedItem);
                seedItem.SetProperty(SeedBagHelper.KEY_REMAINING, curRemaining + 1);
            }
            else
            {
                inventoryService.AddItem(seedData.itemID, 0, 1);
            }

            isExecutingPlacement = false;
            HandleInterrupt();
            return;
        }

        // 创建作物实例数据并初始化
        var instanceData = new CropInstanceData(seedData.itemID, currentDay);
        controller.Initialize(seedData, instanceData, layerIndex, cellPos);
        ConfigureCropPlacementVisuals(cropObj, controller, cropWorldPos);

        // 更新耕地数据
        tileData.SetCropData(instanceData);

        // 播放音效和特效
        PlaySound(placeSuccessSound);
        PlayPlaceEffect(position);

        if (showDebugInfo)
            Debug.Log($"<color=green>[PlacementManager] 种植成功: {seedData.itemName}, Layer={layerIndex}, Pos={cellPos}</color>");

        // 检查是否还有种子
        bool hasMore = HasMoreItems();

        isExecutingPlacement = false;

        if (!hasMore)
        {
            ExitPlacementMode();
        }
        else
        {
            // 还有种子，回到 Preview 状态
            ResumePreviewAfterSuccessfulPlacement();
        }
    }

    private void ResumePreviewAfterSuccessfulPlacement()
    {
        Vector3 lastPlacedPosition = placementPreview != null ? placementPreview.LockedPosition : Vector3.zero;
        currentSnapshot = PlacementSnapshot.Invalid;
        ClearLockedPlacementCommitState();
        ClearNavigationRequestState();

        if (placementPreview != null)
        {
            placementPreview.UnlockPosition();
        }

        ChangeState(PlacementState.Preview);

        if (placementPreview == null)
        {
            ClearPostPlacementPreviewHold();
            ClearAdjacentContinuationSource();
            return;
        }

        SetAdjacentContinuationSource(lastPlacedPosition);
        holdPreviewUntilMouseMoves = true;
        holdPreviewWorldPosition = lastPlacedPosition;
        holdPreviewMouseScreenPosition = Input.mousePosition;
        holdPreviewPlayerCenter = GetPlayerBounds().center;
        holdPreviewPlayerDominantCellValid = TryGetPlayerDominantCell(out holdPreviewPlayerDominantCell);

        if (currentPlacementItem is SaplingData &&
            TryApplyImmediateOccupiedHoldState(lastPlacedPosition))
        {
            return;
        }

        Vector3 currentCandidatePosition = ResolvePreviewCandidatePosition(GetMouseWorldPosition());
        if (IsSamePlacementCandidate(lastPlacedPosition, currentCandidatePosition))
        {
            if (TryApplyImmediateOccupiedHoldState(lastPlacedPosition))
            {
                return;
            }

            if (currentPlacementItem is SaplingData)
            {
                return;
            }

            RefreshPlacementValidationAt(lastPlacedPosition, true);
            return;
        }

        ClearPostPlacementPreviewHold();

        if (currentPlacementItem is SaplingData)
        {
            return;
        }

        RefreshPlacementValidationAt(currentCandidatePosition, true);
    }

    private bool TryApplyImmediateOccupiedHoldState(Vector3 occupiedPosition)
    {
        if (placementPreview == null ||
            currentPlacementItem == null)
        {
            return false;
        }

        List<Vector2Int> occupiedCells = PlacementGridCalculator.GetOccupiedCellIndices(
            occupiedPosition,
            placementPreview.GridSize);
        if (occupiedCells.Count == 0)
        {
            return false;
        }

        placementPreview.ForceUpdatePosition(occupiedPosition);
        currentCellStates = new List<CellState>(occupiedCells.Count);
        for (int i = 0; i < occupiedCells.Count; i++)
        {
            currentCellStates.Add(new CellState(occupiedCells[i], false, InvalidReason.HasObstacle));
        }
        placementPreview.UpdateCellStates(currentCellStates);
        hasImmediateOccupiedHoldState = true;
        ClearValidationReuseCache();
        return true;
    }

    private bool ShouldHoldPreviewAtLastPlacement(Vector3 currentCandidatePosition)
    {
        if (!holdPreviewUntilMouseMoves)
        {
            return false;
        }

        if (!IsSamePlacementCandidate(holdPreviewWorldPosition, currentCandidatePosition))
        {
            ClearPostPlacementPreviewHold();
            return false;
        }

        if ((Input.mousePosition - holdPreviewMouseScreenPosition).sqrMagnitude >
            ResumePreviewMouseMoveThreshold * ResumePreviewMouseMoveThreshold)
        {
            ClearPostPlacementPreviewHold();
            return false;
        }

        bool allowPlayerMovementRelease =
            currentPlacementItem != null &&
            placementPreview != null &&
            placementPreview.GridSize == Vector2Int.one &&
            AllowsAdjacentDirectPlacement(currentPlacementItem);

        if (allowPlayerMovementRelease)
        {
            if (holdPreviewPlayerDominantCellValid)
            {
                // dominant hold only ok..
                if (!TryGetPlayerDominantCell(out Vector2Int currentPlayerCell) ||
                    currentPlayerCell != holdPreviewPlayerDominantCell)
                {
                    ClearPostPlacementPreviewHold();
                    return false;
                }
            }
            else if ((((Vector2)GetPlayerBounds().center) - holdPreviewPlayerCenter).sqrMagnitude >
                     ResumePreviewPlayerMoveThreshold * ResumePreviewPlayerMoveThreshold)
            {
                ClearPostPlacementPreviewHold();
                return false;
            }
        }

        return true;
    }

    private void ClearPostPlacementPreviewHold()
    {
        holdPreviewUntilMouseMoves = false;
        holdPreviewPlayerDominantCellValid = false;
        hasImmediateOccupiedHoldState = false;
    }

    private bool TryReuseCurrentFrameValidation(Vector3 worldPosition, out bool allValid)
    {
        allValid = false;

        if (currentPlacementItem == null ||
            currentCellStates == null ||
            currentCellStates.Count == 0 ||
            lastValidationFrame != Time.frameCount ||
            lastValidatedItem != currentPlacementItem)
        {
            return false;
        }

        Vector2Int anchorCell = PlacementGridCalculator.GetCellIndex(worldPosition);
        if (anchorCell != lastValidatedAnchorCell)
        {
            return false;
        }

        allValid = lastValidationResult;
        return true;
    }

    private void StoreValidationReuseCache(Vector3 worldPosition, bool allValid)
    {
        lastValidationFrame = Time.frameCount;
        lastValidatedAnchorCell = PlacementGridCalculator.GetCellIndex(worldPosition);
        lastValidatedItem = currentPlacementItem;
        lastValidationResult = allValid;
    }

    private void ClearValidationReuseCache()
    {
        lastValidationFrame = -1;
        lastValidatedAnchorCell = default;
        lastValidatedItem = null;
        lastValidationResult = false;
    }

    private void ClearLockedPlacementCommitState()
    {
        hasPendingLockedPlacementCommit = false;
        pendingLockedPlacementCommitFrame = -1;
    }

    private void MarkLockedPlacementCommitIssued()
    {
        hasPendingLockedPlacementCommit = true;
        pendingLockedPlacementCommitFrame = Time.frameCount;
    }

    private bool IsLockedPlacementCommitAlreadyIssuedThisFrame()
    {
        return hasPendingLockedPlacementCommit &&
               pendingLockedPlacementCommitFrame == Time.frameCount;
    }

    private void ClearNavigationRequestState()
    {
        hasLastNavigationTarget = false;
        lastNavigationTarget = Vector3.zero;
    }

    private Vector3 ResolvePreviewCandidatePosition(Vector3 mouseWorldPosition)
    {
        Vector3 baseCandidatePosition = PlacementGridCalculator.GetCellCenter(mouseWorldPosition);
        if (holdPreviewUntilMouseMoves &&
            IsSamePlacementCandidate(baseCandidatePosition, holdPreviewWorldPosition))
        {
            return baseCandidatePosition;
        }

        if (!TryResolveAdjacentIntentBiasedCandidate(baseCandidatePosition, mouseWorldPosition, out Vector3 biasedCandidatePosition))
        {
            return baseCandidatePosition;
        }

        return biasedCandidatePosition;
    }

    private bool TryResolveAdjacentIntentBiasedCandidate(
        Vector3 baseCandidatePosition,
        Vector3 mouseWorldPosition,
        out Vector3 biasedCandidatePosition)
    {
        biasedCandidatePosition = baseCandidatePosition;

        if (currentPlacementItem == null ||
            placementPreview == null ||
            placementPreview.GridSize != Vector2Int.one ||
            !AllowsAdjacentDirectPlacement(currentPlacementItem))
        {
            return false;
        }

        EnsureValidatorInitialized();
        if (validator == null || !IsAdjacentIntentBiasSourceEligible(baseCandidatePosition))
        {
            return false;
        }

        foreach (Vector2Int direction in BuildAdjacentIntentDirections(baseCandidatePosition, mouseWorldPosition))
        {
            if (direction == Vector2Int.zero)
            {
                continue;
            }

            Vector3 candidate = new Vector3(
                baseCandidatePosition.x + direction.x,
                baseCandidatePosition.y + direction.y,
                baseCandidatePosition.z);
            if (!IsSamePlacementCandidate(candidate, baseCandidatePosition) &&
                IsAdjacentIntentBiasCandidateValid(candidate))
            {
                biasedCandidatePosition = candidate;
                return true;
            }
        }

        return false;
    }

    private bool IsAdjacentIntentBiasSourceEligible(Vector3 candidatePosition)
    {
        return adjacentContinuationSourceValid &&
               PlacementGridCalculator.GetCellIndex(candidatePosition) == adjacentContinuationSourceCell &&
               (holdPreviewUntilMouseMoves || IsAdjacentIntentBiasSourceOccupied(candidatePosition));
    }

    private bool IsAdjacentIntentBiasSourceOccupied(Vector3 candidatePosition)
    {
        if (currentPlacementItem is SaplingData)
        {
            return validator.HasTreeAtPosition(candidatePosition, 0.5f);
        }

        if (currentPlacementItem is SeedData)
        {
            var farmTileManager = FarmTileManager.Instance;
            return farmTileManager != null &&
                   farmTileManager.TryResolveTileAtWorld(candidatePosition, out int layerIndex, out Vector3Int cellPos, out _) &&
                   farmTileManager.HasCropOccupant(layerIndex, cellPos);
        }

        return false;
    }

    private bool IsAdjacentIntentBiasCandidateValid(Vector3 candidatePosition)
    {
        if (currentPlacementItem is SeedData seedData)
        {
            return PlacementValidator.ValidateSeedPlacement(seedData, candidatePosition);
        }

        if (currentPlacementItem is SaplingData)
        {
            var saplingCellState = validator.ValidateSaplingPlacement((SaplingData)currentPlacementItem, candidatePosition, playerTransform);
            return saplingCellState.isValid;
        }

        return false;
    }

    private void SetAdjacentContinuationSource(Vector3 placedPosition)
    {
        if (currentPlacementItem == null ||
            placementPreview == null ||
            placementPreview.GridSize != Vector2Int.one ||
            !AllowsAdjacentDirectPlacement(currentPlacementItem))
        {
            ClearAdjacentContinuationSource();
            return;
        }

        adjacentContinuationSourceCell = PlacementGridCalculator.GetCellIndex(placedPosition);
        adjacentContinuationSourceValid = true;
    }

    private void ClearAdjacentContinuationSource()
    {
        adjacentContinuationSourceValid = false;
    }

    private static Vector2Int[] BuildAdjacentIntentDirections(Vector3 baseCandidatePosition, Vector3 mouseWorldPosition)
    {
        TryResolveAdjacentIntentAxis(mouseWorldPosition.x, baseCandidatePosition.x, out int primaryX, out float horizontalDepth);
        TryResolveAdjacentIntentAxis(mouseWorldPosition.y, baseCandidatePosition.y, out int primaryY, out float verticalDepth);

        if (primaryX == 0 && primaryY == 0)
        {
            return Array.Empty<Vector2Int>();
        }

        var directions = new List<Vector2Int>(3);
        void AddDirection(int x, int y)
        {
            var direction = new Vector2Int(x, y);
            if (direction != Vector2Int.zero && !directions.Contains(direction))
            {
                directions.Add(direction);
            }
        }

        if (primaryX != 0 && primaryY != 0)
        {
            AddDirection(primaryX, primaryY);
            if (horizontalDepth >= verticalDepth)
            {
                AddDirection(primaryX, 0);
                AddDirection(0, primaryY);
            }
            else
            {
                AddDirection(0, primaryY);
                AddDirection(primaryX, 0);
            }
        }
        else
        {
            AddDirection(primaryX, 0);
            AddDirection(0, primaryY);
        }

        return directions.ToArray();
    }

    private static void TryResolveAdjacentIntentAxis(
        float mouseCoordinate,
        float cellCenterCoordinate,
        out int direction,
        out float edgeDepth)
    {
        direction = 0;
        edgeDepth = 0f;

        float minEdgeDistance = mouseCoordinate - (cellCenterCoordinate - 0.5f);
        if (minEdgeDistance <= AdjacentIntentEdgeBandWidth)
        {
            direction = -1;
            edgeDepth = Mathf.Clamp(AdjacentIntentEdgeBandWidth - Mathf.Max(0f, minEdgeDistance), 0f, AdjacentIntentEdgeBandWidth);
            return;
        }

        float maxEdgeDistance = (cellCenterCoordinate + 0.5f) - mouseCoordinate;
        if (maxEdgeDistance <= AdjacentIntentEdgeBandWidth)
        {
            direction = 1;
            edgeDepth = Mathf.Clamp(AdjacentIntentEdgeBandWidth - Mathf.Max(0f, maxEdgeDistance), 0f, AdjacentIntentEdgeBandWidth);
        }
    }

    private static bool IsSamePlacementCandidate(Vector3 lhs, Vector3 rhs)
    {
        return PlacementGridCalculator.GetCellIndex(lhs) == PlacementGridCalculator.GetCellIndex(rhs);
    }

    /// <summary>
    /// 使用保存的数据添加到历史（避免依赖可能被清空的 currentPlacementItem）
    /// </summary>
    private void AddToHistoryWithSavedData(PlacementEventData eventData, int itemId, int quality)
    {
        var entry = new PlacementHistoryEntry(
            eventData,
            itemId,
            quality
        );

        placementHistory.Add(entry);

        while (placementHistory.Count > MAX_HISTORY_SIZE)
        {
            placementHistory.RemoveAt(0);
        }
    }

    /// <summary>
    /// 使用保存的数据处理树苗放置（避免依赖可能被清空的 currentPlacementItem）
    /// </summary>
    private bool TryPrepareSaplingPlacement(Vector3 position, GameObject treeObject, ItemData savedItemData, out SaplingPlantedEventData? saplingEvent)
    {
        saplingEvent = null;
        Vector3 plantedCellCenter = PlacementGridCalculator.GetCellCenter(position);

        var saplingData = savedItemData as SaplingData;
        if (saplingData == null)
        {
            return true;
        }

        var treeController = treeObject.GetComponentInChildren<TreeController>();
        if (treeController == null)
        {
            LogPlacementTransaction("树苗落地确认失败：缺少 TreeController");
            return false;
        }

        // 放置树苗时先把树归一到“新树苗基态”，避免同一帧再做一轮多余的阶段重刷。
        treeController.InitializeAsNewTree();

        // 树苗成功必须马上在本地占格事实源上落到目标格；这里不再每次放置后立刻做一次全场找树重扫描。
        if (!IsPlacedSaplingReadyForOccupancy(treeController, plantedCellCenter))
        {
            LogPlacementTransaction($"树苗落地确认失败：本地占格未对齐到目标格 {plantedCellCenter}");
            return false;
        }

        saplingEvent = new SaplingPlantedEventData(
            plantedCellCenter,
            saplingData,
            treeObject,
            treeController
        );
        return true;
    }

    private static bool IsPlacedSaplingReadyForOccupancy(TreeController treeController, Vector3 plantedCellCenter)
    {
        if (treeController == null || !treeController.gameObject.activeInHierarchy)
        {
            return false;
        }

        Transform occupancyRoot = treeController.transform.parent != null
            ? treeController.transform.parent
            : treeController.transform;
        if (occupancyRoot == null || !occupancyRoot.gameObject.activeInHierarchy)
        {
            return false;
        }

        return PlacementGridCalculator.GetCellIndex(occupancyRoot.position) ==
               PlacementGridCalculator.GetCellIndex(plantedCellCenter);
    }

    /// <summary>
    /// 实例化放置预制体
    /// 使用 Collider 中心对齐：放置后 Collider 几何中心 = 格子几何中心
    ///
    /// 核心等式：预览 GridCell 几何中心 = 放置后物品 Collider 几何中心
    /// </summary>
    private GameObject InstantiatePlacementPrefab(Vector3 position)
    {
        if (currentPlacementItem.placementPrefab == null)
        {
            Debug.LogError($"[PlacementManagerV3] {currentPlacementItem.itemName} 缺少放置预制体！");
            return null;
        }

        // ★ 使用 PlacementGridCalculator.GetPlacementPosition() 计算正确的放置位置
        // 该方法会：
        // 1. 计算格子几何中心（考虑多格子偏移）
        // 2. 计算放置后 Collider 中心（考虑底部对齐）
        // 3. 返回正确的放置位置，使 Collider 中心对齐到格子几何中心
        Vector3 placementPosition = PlacementGridCalculator.GetPlacementPosition(position, currentPlacementItem.placementPrefab);

        if (showDebugInfo)
        {
            Vector2Int gridSize = PlacementGridCalculator.GetRequiredGridSizeFromPrefab(currentPlacementItem.placementPrefab);
            Vector2 finalColliderCenter = PlacementGridCalculator.GetColliderCenterAfterBottomAlign(currentPlacementItem.placementPrefab);
            float gridCenterOffsetX = (gridSize.x % 2 == 0) ? 0.5f : 0f;
            float gridCenterOffsetY = (gridSize.y % 2 == 0) ? 0.5f : 0f;
            Vector3 gridGeometricCenter = new Vector3(position.x + gridCenterOffsetX, position.y + gridCenterOffsetY, position.z);

            Debug.Log($"<color=cyan>[PlacementManagerV3] InstantiatePlacementPrefab:</color>\n" +
                      $"  mouseGridCenter={position}\n" +
                      $"  gridSize={gridSize}\n" +
                      $"  gridGeometricCenter={gridGeometricCenter}\n" +
                      $"  finalColliderCenter={finalColliderCenter}\n" +
                      $"  placementPos={placementPosition}");
        }

        Transform parent = ResolvePlacementParent(position);
        if (showDebugInfo)
        {
            Debug.Log($"<color=cyan>[PlacementManagerV3] 放置 parent 解析: {(parent != null ? GetHierarchyPath(parent) : "<scene-root>")}</color>");
        }

        return parent != null
            ? Instantiate(currentPlacementItem.placementPrefab, placementPosition, Quaternion.identity, parent)
            : Instantiate(currentPlacementItem.placementPrefab, placementPosition, Quaternion.identity);
    }

    private Transform ResolvePlacementParent(Vector3 worldPosition)
    {
        if (TryResolveFarmLayerPropsParent(worldPosition, out Transform farmPropsParent))
        {
            return farmPropsParent;
        }

        int targetLayer = PlacementLayerDetector.GetLayerAtPosition(worldPosition);
        string layerName = LayerMask.LayerToName(targetLayer);
        if (TryResolveSceneLayerPropsParent(layerName, out Transform scenePropsParent))
        {
            return scenePropsParent;
        }

        return null;
    }

    private bool TryResolveSceneLayerPropsParent(string layerName, out Transform propsParent)
    {
        propsParent = null;
        if (string.IsNullOrWhiteSpace(layerName))
        {
            return false;
        }

        string upperLayerName = layerName.ToUpperInvariant();
        string sceneRootName = upperLayerName.StartsWith("LAYER ", StringComparison.Ordinal)
            ? upperLayerName
            : layerName.Replace("Layer ", "LAYER ", StringComparison.OrdinalIgnoreCase).ToUpperInvariant();

        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.IsValid())
        {
            return false;
        }

        if (cachedScenePropsParentSceneHandle != activeScene.handle)
        {
            cachedScenePropsParents.Clear();
            cachedMissingScenePropsParents.Clear();
            cachedScenePropsParentSceneHandle = activeScene.handle;
        }

        if (cachedScenePropsParents.TryGetValue(sceneRootName, out Transform cachedPropsParent))
        {
            if (cachedPropsParent != null)
            {
                propsParent = cachedPropsParent;
                return true;
            }

            cachedScenePropsParents.Remove(sceneRootName);
        }

        if (cachedMissingScenePropsParents.Contains(sceneRootName))
        {
            return false;
        }

        foreach (GameObject rootObject in activeScene.GetRootGameObjects())
        {
            Transform layerRoot = FindChildByNameRecursive(rootObject.transform, sceneRootName);
            if (layerRoot == null)
            {
                continue;
            }

            Transform propsTransform = FindChildByNameRecursive(layerRoot, "Props");
            if (propsTransform != null)
            {
                cachedScenePropsParents[sceneRootName] = propsTransform;
                propsParent = propsTransform;
                return true;
            }
        }

        cachedMissingScenePropsParents.Add(sceneRootName);
        return false;
    }

    private bool TryResolveFarmLayerPropsParent(Vector3 worldPosition, out Transform propsParent)
    {
        propsParent = null;

        var farmTileManager = FarmTileManager.Instance;
        if (farmTileManager == null)
        {
            return false;
        }

        int layerIndex = farmTileManager.GetCurrentLayerIndex(worldPosition);
        LayerTilemaps tilemaps = farmTileManager.GetLayerTilemaps(layerIndex);
        if (tilemaps == null || tilemaps.propsContainer == null)
        {
            return false;
        }

        propsParent = tilemaps.propsContainer;
        return true;
    }

    private static Transform FindChildByNameRecursive(Transform root, string targetName)
    {
        if (root == null || string.IsNullOrWhiteSpace(targetName))
        {
            return null;
        }

        if (string.Equals(root.name, targetName, StringComparison.OrdinalIgnoreCase))
        {
            return root;
        }

        foreach (Transform child in root)
        {
            Transform match = FindChildByNameRecursive(child, targetName);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }

    private static string GetHierarchyPath(Transform transform)
    {
        if (transform == null)
        {
            return string.Empty;
        }

        var segments = new List<string>();
        Transform current = transform;
        while (current != null)
        {
            segments.Add(current.name);
            current = current.parent;
        }

        segments.Reverse();
        return string.Join("/", segments);
    }

    /// <summary>
    /// 同步 Layer 到放置物体
    /// </summary>
    private void SyncLayerToPlacedObject(GameObject placedObject, int layer)
    {
        PlacementLayerDetector.SyncLayerToAllChildren(placedObject, layer);

        string sortingLayerName = PlacementLayerDetector.GetPlayerSortingLayer(playerTransform);
        PlacementLayerDetector.SyncSortingLayerToAllRenderers(placedObject, sortingLayerName);
    }

    /// <summary>
    /// 设置排序 Order
    /// </summary>
    private void SetSortingOrder(GameObject placedObject, Vector3 position)
    {
        int order = -Mathf.RoundToInt(position.y * sortingOrderMultiplier) + sortingOrderOffset;

        var renderers = placedObject.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var renderer in renderers)
        {
            bool isShadow = renderer.gameObject.name.ToLower().Contains("shadow");
            renderer.sortingOrder = isShadow ? order - 1 : order;
        }
    }

    /// <summary>
    /// 为种植生成的作物补齐放置系统已有的图层与动态排序配置。
    /// </summary>
    private void ConfigureCropPlacementVisuals(GameObject cropObject, CropController controller, Vector3 worldPosition)
    {
        if (cropObject == null)
        {
            return;
        }

        int targetLayer = PlacementLayerDetector.GetLayerAtPosition(worldPosition);
        SyncLayerToPlacedObject(cropObject, targetLayer);
        SetSortingOrder(cropObject, worldPosition);

        GameObject sortingTarget = controller != null ? controller.gameObject : cropObject;
        EnsureDynamicSortingOnPlacedObject(sortingTarget);
    }

    private void EnsureDynamicSortingOnPlacedObject(GameObject sortingTarget)
    {
        if (sortingTarget == null)
        {
            return;
        }

        var dynamicSorting = sortingTarget.GetComponent<DynamicSortingOrder>();
        if (dynamicSorting == null)
        {
            dynamicSorting = sortingTarget.AddComponent<DynamicSortingOrder>();
        }

        dynamicSorting.sortingOrderMultiplier = sortingOrderMultiplier;
        dynamicSorting.sortingOrderOffset = sortingOrderOffset;
        dynamicSorting.useSpriteBounds = true;
        dynamicSorting.autoHandleShadow = true;
    }

    /// <summary>
    /// 处理树苗放置
    /// </summary>
    private void HandleSaplingPlacement(Vector3 position, GameObject treeObject)
    {
        var saplingData = currentPlacementItem as SaplingData;
        if (saplingData == null) return;

        var treeController = treeObject.GetComponentInChildren<TreeController>();
        if (treeController != null)
        {
            treeController.InitializeAsNewTree();

            var saplingEvent = new SaplingPlantedEventData(
                position,
                saplingData,
                treeObject,
                treeController
            );
            OnSaplingPlanted?.Invoke(saplingEvent);
        }
    }

    #endregion

    #region 撤销功能

    /// <summary>
    /// 撤销最近一次放置
    /// </summary>
    public bool UndoLastPlacement()
    {
        if (placementHistory.Count == 0)
            return false;

        var lastEntry = placementHistory[placementHistory.Count - 1];

        if (!lastEntry.CanUndo)
            return false;

        if (lastEntry.EventData.PlacedObject != null)
        {
            Destroy(lastEntry.EventData.PlacedObject);
        }

        ReturnItemToInventory(lastEntry.DeductedItemId, lastEntry.DeductedItemQuality);
        placementHistory.RemoveAt(placementHistory.Count - 1);

        if (showDebugInfo)
            Debug.Log($"<color=cyan>[PlacementManagerV3] 撤销放置成功</color>");

        return true;
    }

    private void AddToHistory(PlacementEventData eventData)
    {
        var entry = new PlacementHistoryEntry(
            eventData,
            currentPlacementItem.itemID,
            currentItemQuality
        );

        placementHistory.Add(entry);

        while (placementHistory.Count > MAX_HISTORY_SIZE)
        {
            placementHistory.RemoveAt(0);
        }
    }

    #endregion

    #region 辅助方法

    private Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    private Bounds GetPlayerBounds()
    {
        if (playerTransform == null)
            return new Bounds(Vector3.zero, Vector3.one);

        var collider = playerTransform.GetComponent<Collider2D>();
        if (collider != null)
            return collider.bounds;

        return new Bounds(playerTransform.position, Vector3.one);
    }

    private bool ShouldDirectPlaceFromPlayerNeighborhood()
    {
        if (currentPlacementItem == null ||
            placementPreview == null ||
            placementPreview.GridSize != Vector2Int.one ||
            !AllowsAdjacentDirectPlacement(currentPlacementItem))
        {
            return false;
        }

        if (!TryGetPlayerDominantCell(out Vector2Int playerCell))
        {
            return false;
        }

        Vector2Int targetCell = PlacementGridCalculator.GetCellIndex(placementPreview.GetPreviewPosition());
        return Mathf.Abs(targetCell.x - playerCell.x) <= 1 &&
               Mathf.Abs(targetCell.y - playerCell.y) <= 1;
    }

    private bool TryGetPlayerDominantCell(out Vector2Int dominantCell)
    {
        Bounds playerBounds = GetPlayerBounds();
        float totalArea = playerBounds.size.x * playerBounds.size.y;
        if (totalArea <= 0.0001f)
        {
            dominantCell = PlacementGridCalculator.GetCellIndex(playerBounds.center);
            return true;
        }

        int minX = Mathf.FloorToInt(playerBounds.min.x);
        int maxX = Mathf.FloorToInt(playerBounds.max.x - 0.0001f);
        int minY = Mathf.FloorToInt(playerBounds.min.y);
        int maxY = Mathf.FloorToInt(playerBounds.max.y - 0.0001f);

        float bestArea = 0f;
        dominantCell = PlacementGridCalculator.GetCellIndex(playerBounds.center);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                float overlapWidth = Mathf.Max(0f, Mathf.Min(playerBounds.max.x, x + 1f) - Mathf.Max(playerBounds.min.x, x));
                float overlapHeight = Mathf.Max(0f, Mathf.Min(playerBounds.max.y, y + 1f) - Mathf.Max(playerBounds.min.y, y));
                float overlapArea = overlapWidth * overlapHeight;
                if (overlapArea <= bestArea)
                {
                    continue;
                }

                bestArea = overlapArea;
                dominantCell = new Vector2Int(x, y);
            }
        }

        return bestArea / totalArea >= DirectPlacementDominantCellThreshold;
    }

    private bool AllowsAdjacentDirectPlacement(ItemData placementItem)
    {
        if (placementItem == null)
        {
            return false;
        }

        if (placementItem is SeedData)
        {
            return true;
        }

        if (placementItem is SaplingData saplingData)
        {
            return !HasBlockingColliderAtPlacement(saplingData);
        }

        return !HasBlockingColliderAtPlacement(placementItem);
    }

    private bool HasBlockingColliderAtPlacement(ItemData placementItem)
    {
        if (placementItem == null || placementItem.placementPrefab == null)
        {
            return false;
        }

        var colliders = placementItem.placementPrefab.GetComponentsInChildren<Collider2D>(true);
        foreach (var collider in colliders)
        {
            if (collider != null && collider.enabled && !collider.isTrigger)
            {
                return true;
            }
        }

        return false;
    }

    private bool DeductItemFromInventory()
    {
        // ★ 使用快照数据扣除背包物品
        if (inventoryService == null || !currentSnapshot.isValid)
        {
            if (showDebugInfo)
                Debug.Log($"<color=red>[PlacementManagerV3] DeductItemFromInventory 失败: inventoryService={inventoryService != null}, snapshot.isValid={currentSnapshot.isValid}</color>");
            return false;
        }

        bool success = inventoryService.RemoveFromSlot(currentSnapshot.slotIndex, 1);
        if (showDebugInfo)
            Debug.Log($"<color=cyan>[PlacementManagerV3] DeductItemFromInventory: slotIndex={currentSnapshot.slotIndex}, success={success}</color>");

        return success;
    }

    private bool HasMoreItems()
    {
        // ★ 使用快照数据检查剩余物品
        if (inventoryService == null || !currentSnapshot.isValid)
            return false;

        // 🔴 补丁005 B.4.3：种子袋剩余检测（走 SeedBagHelper，不是 slot.amount）
        if (currentPlacementItem is SeedData)
        {
            var seedItem = inventoryService.GetInventoryItem(currentSnapshot.slotIndex);
            return seedItem != null && !seedItem.IsEmpty
                && SeedBagHelper.GetRemaining(seedItem) > 0;
        }

        var slot = inventoryService.GetSlot(currentSnapshot.slotIndex);

        // 检查槽位是否还有相同物品
        bool hasMore = !slot.IsEmpty &&
                       slot.itemId == currentSnapshot.itemId &&
                       slot.quality == currentSnapshot.quality;

        if (showDebugInfo)
            Debug.Log($"<color=cyan>[PlacementManagerV3] HasMoreItems: slotIndex={currentSnapshot.slotIndex}, hasMore={hasMore}, amount={slot.amount}</color>");

        return hasMore;
    }

    private void ReturnItemToInventory(int itemId, int quality)
    {
        // ★ 使用 InventoryService 返还物品
        if (inventoryService == null) return;

        int remaining = inventoryService.AddItem(itemId, quality, 1);
        if (showDebugInfo)
            Debug.Log($"<color=cyan>[PlacementManagerV3] ReturnItemToInventory: itemId={itemId}, quality={quality}, remaining={remaining}</color>");
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, transform.position, soundVolume);
    }

    public void PlayFailFeedbackSound()
    {
        PlaySound(placeFailSound);
    }

    private void PlayPlaceEffect(Vector3 position)
    {
        if (placeEffectPrefab == null) return;
        var effect = Instantiate(placeEffectPrefab, position, Quaternion.identity);
        Destroy(effect, 2f);
    }

    private System.Collections.IEnumerator PlayPlaceEffectNextFrame(Vector3 position)
    {
        yield return null;
        PlayPlaceEffect(position);
    }

    #endregion
}
