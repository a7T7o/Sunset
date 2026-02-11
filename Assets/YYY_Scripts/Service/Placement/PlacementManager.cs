using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using FarmGame.Data;
using FarmGame.Events;

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
    [SerializeField] private AudioClip placeSuccessSound;
    [SerializeField] private AudioClip placeFailSound;
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.8f;
    
    [Header("━━━━ 特效 ━━━━")]
    [SerializeField] private GameObject placeEffectPrefab;
    
    [Header("━━━━ 调试 ━━━━")]
    [SerializeField] private bool showDebugInfo = true; // 开启调试以排查问题
    
    #endregion
    
    #region 私有字段
    
    private PlacementValidator validator;
    private ItemData currentPlacementItem;
    private int currentItemQuality;
    private PlacementState currentState = PlacementState.Idle;
    private Camera mainCamera;
    private List<CellState> currentCellStates = new List<CellState>();
    
    // 放置历史
    private List<PlacementHistoryEntry> placementHistory = new List<PlacementHistoryEntry>();
    private const int MAX_HISTORY_SIZE = 10;
    
    // ★ 背包联动相关
    private InventoryService inventoryService;
    private HotbarSelectionService hotbarSelection;
    private PackagePanelTabsUI packageTabs;
    private PlacementSnapshot currentSnapshot;
    
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
            inventoryService = FindFirstObjectByType<InventoryService>();
        if (hotbarSelection == null)
            hotbarSelection = FindFirstObjectByType<HotbarSelectionService>();
        if (packageTabs == null)
            packageTabs = FindFirstObjectByType<PackagePanelTabsUI>(FindObjectsInactive.Include);
        
        // ★ 订阅手持物品切换事件
        if (hotbarSelection != null)
            hotbarSelection.OnSelectedChanged += OnHotbarSelectionChanged;
        
        // ★ 订阅背包槽位变化事件（用于检测物品被扣除）
        if (inventoryService != null)
            inventoryService.OnSlotChanged += OnInventorySlotChanged;
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
            if (placementPreview != null && placementPreview.gameObject.activeSelf)
            {
                placementPreview.gameObject.SetActive(false);
                if (showDebugInfo)
                    Debug.Log($"<color=yellow>[PlacementManagerV3] 背包打开，隐藏预览</color>");
            }
            
            // 🔥 Bug E 修复：面板打开 = 暂停，不中断
            // 不调用 HandleInterrupt()，保持 Locked/Navigating 状态
            // 关闭面板后自动恢复
            return;
        }
        else
        {
            // 背包关闭时恢复预览
            if (placementPreview != null && !placementPreview.gameObject.activeSelf && currentState != PlacementState.Idle)
            {
                placementPreview.gameObject.SetActive(true);
                if (showDebugInfo)
                    Debug.Log($"<color=green>[PlacementManagerV3] 背包关闭，恢复预览显示</color>");
            }
        }
        
        // ★ 在 Locked/Navigating 状态下检测中断条件
        if (currentState == PlacementState.Locked || currentState == PlacementState.Navigating)
        {
            if (CheckInterruptConditions())
            {
                HandleInterrupt();
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
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
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
        if (item == null || !item.isPlaceable)
            return;
        
        // ★ SeedData 不通过放置系统处理，由农田系统管理
        if (item is SeedData)
        {
            if (showDebugInfo)
                Debug.Log($"<color=yellow>[PlacementManagerV3] SeedData 不通过放置系统处理，请使用农田系统种植: {item.itemName}</color>");
            return;
        }
        
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
        if (currentPlacementItem == null || hotbarSelection == null)
            return PlacementSnapshot.Invalid;
        
        return new PlacementSnapshot
        {
            itemId = currentPlacementItem.itemID,
            quality = currentItemQuality,
            slotIndex = hotbarSelection.selectedIndex,
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
    
    /// <summary>
    /// 更新预览位置和状态
    /// </summary>
    private void UpdatePreview()
    {
        if (placementPreview == null || currentPlacementItem == null) return;
        
        // 获取鼠标世界坐标
        Vector3 mousePos = GetMouseWorldPosition();
        
        // 更新预览位置（锁定状态下不会更新）
        placementPreview.UpdatePosition(mousePos);
        
        // 同步 Sorting Layer
        if (playerTransform != null)
        {
            string sortingLayerName = PlacementLayerDetector.GetPlayerSortingLayer(playerTransform);
            placementPreview.UpdateSortingLayer(sortingLayerName);
        }
        
        // 验证格子状态
        Vector3 previewPos = placementPreview.GetPreviewPosition();
        Vector2Int gridSize = placementPreview.GridSize;
        
        // ★ 根据物品类型选择验证方法
        if (currentPlacementItem is SaplingData saplingData)
        {
            // 树苗使用专用验证（包含无碰撞体树苗检测）
            var saplingState = validator.ValidateSaplingPlacement(saplingData, previewPos, playerTransform);
            currentCellStates = new List<CellState> { saplingState };
        }
        else
        {
            // 其他物品使用通用验证
            currentCellStates = validator.ValidateCells(previewPos, gridSize, playerTransform);
        }
        
        placementPreview.UpdateCellStates(currentCellStates);
        
        if (showDebugInfo)
        {
            bool allValid = validator.AreAllCellsValid(currentCellStates);
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
            if (validator.AreAllCellsValid(currentCellStates))
            {
                if (showDebugInfo)
                    Debug.Log($"<color=green>[PlacementManagerV3] 所有格子有效，锁定位置</color>");
                LockPreviewPosition();
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
            Vector3 cellCenter = PlacementGridCalculator.GetCellCenter(mousePos);
            var newCellStates = validator.ValidateCells(cellCenter, placementPreview.GridSize, playerTransform);
            
            if (validator.AreAllCellsValid(newCellStates))
            {
                if (showDebugInfo)
                    Debug.Log($"<color=cyan>[PlacementManagerV3] 导航中点击新位置，重新导航</color>");
                
                // 取消当前导航
                navigator.CancelNavigation();
                
                // ★ 清除旧快照
                currentSnapshot = PlacementSnapshot.Invalid;
                
                // 解锁并更新到新位置
                placementPreview.UnlockPosition();
                placementPreview.ForceUpdatePosition(mousePos);
                currentCellStates = newCellStates;
                placementPreview.UpdateCellStates(currentCellStates);
                
                // 锁定新位置（会创建新快照）
                LockPreviewPosition();
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
    private void LockPreviewPosition()
    {
        if (placementPreview == null) return;
        
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
        
        if (showDebugInfo)
        {
            Debug.Log($"<color=cyan>[PlacementManagerV3] LockPreviewPosition: playerBounds={playerBounds}, previewBounds={previewBounds}</color>");
        }
        
        if (navigator.IsAlreadyNearTarget(playerBounds, previewBounds))
        {
            // 已经在附近，直接放置
            if (showDebugInfo)
                Debug.Log($"<color=green>[PlacementManagerV3] 玩家已在目标附近，直接放置</color>");
            
            ExecutePlacement();
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
        
        // 计算导航目标点
        Vector3 targetPos = navigator.CalculateNavigationTarget(playerBounds, previewBounds);
        
        // 开始导航
        navigator.StartNavigation(targetPos, previewBounds);
        ChangeState(PlacementState.Navigating);
        
        if (showDebugInfo)
            Debug.Log($"<color=cyan>[PlacementManagerV3] 开始导航到: {targetPos}</color>");
    }
    
    /// <summary>
    /// 导航到达回调
    /// </summary>
    private void OnNavigationReached()
    {
        if (showDebugInfo)
            Debug.Log($"<color=green>[PlacementManagerV3] 到达目标，执行放置</color>");
        
        ExecutePlacement();
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
        
        // ★ 验证快照有效性
        if (!ValidateSnapshot())
        {
            if (showDebugInfo)
                Debug.Log($"<color=red>[PlacementManagerV3] 快照验证失败，取消放置</color>");
            isExecutingPlacement = false;
            HandleInterrupt();
            return;
        }
        
        Vector3 position = placementPreview.LockedPosition;
        
        // 检查 PlaceableItemData 的自定义验证
        if (currentPlacementItem is PlaceableItemData placeableItem)
        {
            if (!placeableItem.CanPlaceAt(position))
            {
                PlaySound(placeFailSound);
                isExecutingPlacement = false;
                HandleInterrupt();
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
        
        if (showDebugInfo)
            Debug.Log($"<color=cyan>[PlacementManagerV3] 保存物品数据: id={savedItemId}, name={savedItemName}, type={savedPlacementType}</color>");
        
        // 实例化预制体
        GameObject placedObject = InstantiatePlacementPrefab(position);
        if (placedObject == null)
        {
            PlaySound(placeFailSound);
            isExecutingPlacement = false;
            HandleInterrupt();
            return;
        }
        
        if (showDebugInfo)
            Debug.Log($"<color=cyan>[PlacementManagerV3] 预制体实例化成功: {placedObject.name}</color>");
        
        // 获取目标 Layer
        int targetLayer = PlacementLayerDetector.GetLayerAtPosition(position);
        
        // 同步 Layer
        SyncLayerToPlacedObject(placedObject, targetLayer);
        
        // 设置排序 Order
        SetSortingOrder(placedObject, position);
        
        // ★ 扣除背包物品（使用快照数据）
        // 注意：这一步可能触发 HotbarSelectionService.OnSlotChanged，进而调用 ExitPlacementMode
        // 但因为我们已经保存了数据，所以不会出错
        if (showDebugInfo)
            Debug.Log($"<color=yellow>[PlacementManagerV3] 准备扣除物品，slotIndex={currentSnapshot.slotIndex}</color>");
        
        if (!DeductItemFromInventory())
        {
            // 扣除失败，销毁已实例化的物品
            Destroy(placedObject);
            PlaySound(placeFailSound);
            if (showDebugInfo)
                Debug.Log($"<color=red>[PlacementManagerV3] 背包扣除失败，取消放置</color>");
            isExecutingPlacement = false;
            HandleInterrupt();
            return;
        }
        
        if (showDebugInfo)
            Debug.Log($"<color=green>[PlacementManagerV3] 背包扣除成功</color>");
        
        // ★ 使用保存的数据创建事件数据（不再依赖 currentPlacementItem）
        var eventData = new PlacementEventData(
            position,
            savedItemData,  // 使用保存的数据
            placedObject,
            savedPlacementType  // 使用保存的数据
        );
        
        // ★ 使用保存的数据添加到历史
        AddToHistoryWithSavedData(eventData, savedItemId, savedQuality);
        
        // 播放音效和特效
        PlaySound(placeSuccessSound);
        PlayPlaceEffect(position);
        
        // 广播事件
        OnItemPlaced?.Invoke(eventData);
        
        // ★ 树苗特殊处理（使用保存的数据）
        if (savedPlacementType == PlacementType.Sapling)
        {
            HandleSaplingPlacementWithSavedData(position, placedObject, savedItemData);
        }
        
        if (showDebugInfo)
            Debug.Log($"<color=green>[PlacementManagerV3] 放置成功: {savedItemName}</color>");
        
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
            
            currentSnapshot = PlacementSnapshot.Invalid; // 清除旧快照
            if (placementPreview != null)
            {
                placementPreview.UnlockPosition();
            }
            ChangeState(PlacementState.Preview);
        }
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
    private void HandleSaplingPlacementWithSavedData(Vector3 position, GameObject treeObject, ItemData savedItemData)
    {
        var saplingData = savedItemData as SaplingData;
        if (saplingData == null) return;
        
        var treeController = treeObject.GetComponentInChildren<TreeController>();
        if (treeController != null)
        {
            // 🔥 锐评022：显式初始化新树木，生成 GUID 并注册
            treeController.InitializeAsNewTree();
            
            treeController.SetStage(0);
            
            var saplingEvent = new SaplingPlantedEventData(
                position,
                saplingData,
                treeObject,
                treeController
            );
            OnSaplingPlanted?.Invoke(saplingEvent);
        }
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
        
        return Instantiate(currentPlacementItem.placementPrefab, placementPosition, Quaternion.identity);
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
    /// 处理树苗放置
    /// </summary>
    private void HandleSaplingPlacement(Vector3 position, GameObject treeObject)
    {
        var saplingData = currentPlacementItem as SaplingData;
        if (saplingData == null) return;
        
        var treeController = treeObject.GetComponentInChildren<TreeController>();
        if (treeController != null)
        {
            treeController.SetStage(0);
            
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
    
    private void PlayPlaceEffect(Vector3 position)
    {
        if (placeEffectPrefab == null) return;
        var effect = Instantiate(placeEffectPrefab, position, Quaternion.identity);
        Destroy(effect, 2f);
    }
    
    #endregion
}
