using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using FarmGame.Data;
using FarmGame.Data.Core;
using FarmGame.UI;
using FarmGame.World;
using FarmGame.Farm;

// ===== 10.1.1 补丁002：FIFO 操作队列类型定义 =====

/// <summary>农田操作类型</summary>
public enum FarmActionType { Till, Water, PlantSeed, Harvest, RemoveCrop }

/// <summary>操作请求（值类型，轻量）</summary>
public struct FarmActionRequest
{
    public FarmActionType type;
    public Vector3Int cellPos;        // 目标格子坐标
    public int layerIndex;            // 目标层级索引
    public Vector3 worldPos;          // 目标世界坐标（格子中心，用于导航和距离判断）
    public CropController targetCrop; // 仅 Harvest 类型使用，其他类型为 null
    public int puddleVariant;         // 🔴 V3 模块L（CP-L1）：浇水预分配水渍变体索引（-1=未分配）
}

public class GameInputManager : MonoBehaviour
{
    [SerializeField, HideInInspector] private PlayerMovement playerMovement;
    [SerializeField, HideInInspector] private PlayerInteraction playerInteraction;
    [SerializeField, HideInInspector] private PlayerToolController playerToolController;
    [SerializeField, HideInInspector] private PlayerAutoNavigator autoNavigator;

    [SerializeField, HideInInspector] private InventoryService inventory;
    [SerializeField, HideInInspector] private HotbarSelectionService hotbarSelection;
    [SerializeField, HideInInspector] private PackagePanelTabsUI packageTabs;
    
    private ItemDatabase database; // 从 InventoryService 获取

    [SerializeField] private bool useAxisForMovement = false;
    [SerializeField, HideInInspector] private Camera worldCamera;
    [Header("交互设置")]
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private string[] interactableTags = new string[0];
    [SerializeField] private bool blockNavOverUI = false;
    [SerializeField, Range(0f, 1.5f)] private float navClickDeadzone = 0.3f; // 以玩家为圆心的点击死区
    [SerializeField, Range(0.05f, 0.5f)] private float navClickCooldown = 0.15f; // 导航点击间隔，防抖
    [SerializeField, Range(0.2f, 2f)] private float minNavDistance = 0.5f; // 最小导航距离，防止连续点击同一位置
    [Header("农田工具设置")]
    [Tooltip("农田工具（锄头、水壶）的最大使用距离")]
    [SerializeField, Range(0.5f, 3f)] private float farmToolReach = 1.5f;
    [Header("调试开关")]
    [SerializeField, HideInInspector] private TimeManagerDebugger timeDebugger = null;
    [SerializeField] private bool enableTimeDebugKeys = false;
    [Header("UI自动激活")]
    [SerializeField] private bool autoActivateUIRoot = true;
    [SerializeField] private string uiRootName = "UI";

    private GameObject uiRootCache;
    private bool packageTabsInitialized = false;

    #region 阶段1：对话输入锁（最小接入）
    private bool _inputEnabled = true;
    public bool IsInputEnabledForDebug => _inputEnabled;

    public void SetInputEnabled(bool enabled)
    {
        _inputEnabled = enabled;
        if (!enabled)
        {
            if (playerMovement != null) playerMovement.SetMovementInput(Vector2.zero, false);
        }
    }

    private void OnEnable()
    {
        Sunset.Events.EventBus.Subscribe<Sunset.Events.DialogueStartEvent>(OnDialogueStart, owner: this);
        Sunset.Events.EventBus.Subscribe<Sunset.Events.DialogueEndEvent>(OnDialogueEnd, owner: this);
    }

    private void OnDisable()
    {
        Sunset.Events.EventBus.UnsubscribeAll(this);
    }

    private void OnDialogueStart(Sunset.Events.DialogueStartEvent _)
    {
        SetInputEnabled(false);
    }

    private void OnDialogueEnd(Sunset.Events.DialogueEndEvent _)
    {
        SetInputEnabled(true);
    }
    #endregion

    private static GameInputManager s_instance;
    /// <summary>公开单例访问（供 PlayerInteraction 回调使用）</summary>
    public static GameInputManager Instance => s_instance;
    private float lastNavClickTime = -1f;
    private Vector3 lastNavClickPos = Vector3.zero;
    
    // 🔥 9.0.5 扩展：农田导航状态机
    private enum FarmNavState 
    { 
        Idle,       // 空闲，无预览（手持非农具）
        Preview,    // 预览跟随鼠标（手持农具/种子）
        Locked,     // 预览锁定在目标位置（点击后，判断距离前）
        Navigating, // 正在导航，预览保持锁定
        Executing   // 正在执行动作，预览保持锁定
    }
    
    private FarmNavState _farmNavState = FarmNavState.Idle;
    private System.Action _farmNavigationAction = null;
    // 🔴 补丁005：_cachedSeedData 已移除（种子不再走 FIFO 导航）
    private Coroutine _farmingNavigationCoroutine = null;
    private Vector3 _farmNavigationTarget = Vector3.zero;
    private bool _hasPausedFarmNavigation = false;
    private Vector3 _pausedFarmNavigationTarget = Vector3.zero;
    private System.Action _pausedFarmNavigationAction = null;
    
    // 🔥 9.0.5 新增：执行保护标志
    private bool _isExecutingFarming = false;
    
    // 🔥 10.1.0 新增：输入缓存（动画期间暂存玩家输入，动画结束后消费）
    // ⚠️ 10.1.1补丁002 废弃：已被 FIFO 队列（_farmActionQueue）替代，保留字段避免编译错误
    // 🔥 9.0.4 新增：农田操作快照（防止"种瓜得豆"）
    private struct FarmingSnapshot
    {
        public int itemId;      // 物品 ID
        public int slotIndex;   // 槽位索引
        public int count;       // 数量
        public bool isValid;    // 快照是否有效
        
        public static FarmingSnapshot Invalid => new FarmingSnapshot { isValid = false };
        
        public static FarmingSnapshot Create(int itemId, int slotIndex, int count)
        {
            return new FarmingSnapshot
            {
                itemId = itemId,
                slotIndex = slotIndex,
                count = count,
                isValid = true
            };
        }
    }
    
    private FarmingSnapshot _farmingSnapshot = FarmingSnapshot.Invalid;

    // ===== 10.1.1 补丁002：FIFO 操作队列 =====
    private Queue<FarmActionRequest> _farmActionQueue = new();
    private HashSet<(int layerIndex, Vector3Int cellPos)> _queuedPositions = new();
    private bool _isProcessingQueue = false;    // 队列正在执行中（有操作在处理）
    private bool _isQueuePaused = false;        // 队列暂停（面板打开时）
    private CropController _currentHarvestTarget = null;   // 当前正在收获的作物
    private FarmActionRequest _currentProcessingRequest;    // 当前正在处理的请求
    private bool _wasUIOpen = false;                        // 面板暂停/恢复：上一帧 UI 状态
    private bool _hasUIFrozenProtectedHeldSlot = false;
    private int _uiFrozenProtectedHeldSlotIndex = -1;

    // ===== 补丁003 模块C：延迟 tile 更新 =====
    private FarmActionRequest? _pendingTileUpdate = null;
    private bool _tileUpdateTriggered = false;
    
    [Header("动画帧触发")]
    [SerializeField] private float tileUpdateTriggerProgress = 0.5f;  // 动画50%进度触发

    // ===== 10.2.0 V键放置模式状态机 =====
    /// <summary>
    /// 放置模式开关（V键切换）
    /// true = 允许农田操作和放置操作
    /// false = 只允许常规工具打击（除草、打怪、砍树）
    /// </summary>
    public bool IsPlacementMode { get; private set; } = false;  // 默认关闭

    void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            enabled = false;
            return;
        }
        s_instance = this;

        if (playerMovement == null) playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerInteraction == null) playerInteraction = FindFirstObjectByType<PlayerInteraction>();
        if (playerToolController == null) playerToolController = FindFirstObjectByType<PlayerToolController>();
        if (autoNavigator == null) autoNavigator = FindFirstObjectByType<PlayerAutoNavigator>();

        if (inventory == null) inventory = FindFirstObjectByType<InventoryService>();
        if (hotbarSelection == null) hotbarSelection = FindFirstObjectByType<HotbarSelectionService>();
        if (packageTabs == null) packageTabs = FindFirstObjectByType<PackagePanelTabsUI>(FindObjectsInactive.Include);

        // 从 InventoryService 获取 database(ItemDatabase 是 ScriptableObject,不能用 Find)
        if (inventory != null)
            database = inventory.Database;

        if (worldCamera == null) worldCamera = Camera.main;
    }

    void OnDestroy()
    {
        if (s_instance == this)
        {
            s_instance = null;
        }
    }

    void Start()
    {
        // 运行时自动激活UI根物体
        var uiRoot = ResolveUIRoot();
        if (autoActivateUIRoot)
        {
            if (uiRoot != null && !uiRoot.activeSelf)
            {
                uiRoot.SetActive(true);
            }
            else if (uiRoot == null)
            {
                Debug.LogError($"未找到名为 '{uiRootName}' 的UI根物体！");
            }
        }

        packageTabs = EnsurePackageTabs();
        if (packageTabs == null)
        {
            Debug.LogError("PackagePanelTabsUI 仍然为 null，无法初始化面板热键！");
        }
    }

    void Update()
    {
        // ===== 10.2.0 V键放置模式切换 =====
        if (Input.GetKeyDown(KeyCode.V))
        {
            IsPlacementMode = !IsPlacementMode;

            if (!IsPlacementMode)
            {
                HideFarmToolPreview();
            }

            SyncPlaceableModeWithCurrentSelection();
        }
        
        // 🔥 9.0.4 修复：UpdatePreviews 必须在第一行，确保 WYSIWYG（所见即所得）
        UpdatePreviews();
        
        HandlePanelHotkeys();
        HandleRunToggleWhileNav();
        HandleMovement();
        HandleHotbarSelection();
        
        // 🔥 10.1.1补丁002 任务5.3：面板暂停/恢复机制
        // 在 HandleUseCurrentTool 之前检测面板状态变化
        bool uiOpen = IsAnyPanelOpen();
        if (uiOpen && !_wasUIOpen)
        {
            // 面板刚打开 → 冻结自动链推进，并隐藏实时预览
            CaptureProtectedHeldSlotForUIFreeze();
            _isQueuePaused = true;
            HideFarmToolPreview();
            PauseCurrentNavigationForUI();
        }
        else if (!uiOpen && _wasUIOpen)
        {
            // 面板刚关闭 → 恢复预览与队列推进
            _isQueuePaused = false;
            PlacementManager.Instance?.RefreshCurrentPreview();
            bool resumedPausedNavigation = ResumePausedFarmNavigationAfterUI();
            ClearProtectedHeldSlotForUIFreeze();
            if (!resumedPausedNavigation && (_farmActionQueue.Count > 0 || _isProcessingQueue) && !_isExecutingFarming)
                ProcessNextAction();
        }
        _wasUIOpen = uiOpen;
        
        // 🔴 补丁003 模块C：延迟 tile 更新 — 监听动画进度
        if (_pendingTileUpdate != null && !_tileUpdateTriggered)
        {
            float progress = playerInteraction != null ? playerInteraction.GetAnimationProgress() : 1f;
            if (progress >= tileUpdateTriggerProgress)
            {
                var req = _pendingTileUpdate.Value;
                switch (req.type)
                {
                    case FarmActionType.Till:
                        ExecuteTillSoil(req.layerIndex, req.cellPos);
                        break;
                    case FarmActionType.Water:
                        ExecuteWaterTile(req.layerIndex, req.cellPos, req.puddleVariant);
                        break;
                    case FarmActionType.RemoveCrop:
                        // 🔴 V6 模块S（CP-S4）：动画中期清除农作物
                        ExecuteRemoveCrop(req.layerIndex, req.cellPos);
                        break;
                }
                _tileUpdateTriggered = true;
                // 不清空 _pendingTileUpdate，等动画完成回调时清空
            }
        }
        
        HandleUseCurrentTool();
        HandleRightClickAutoNav();
        if (timeDebugger != null) timeDebugger.enableDebugKeys = enableTimeDebugKeys;
    }
    
    /// <summary>
    /// 🔥 新增：根据手持物品更新预览
    /// 路由到 PlacementPreview 或 FarmToolPreview
    /// </summary>
    private void UpdatePreviews()
    {
        // 🔥 9.0.5 修正：面板打开时不做任何预览操作
        // 不隐藏（保留锁定状态），不更新（面板冻结事件）
        if (IsAnyPanelOpen())
        {
            return;
        }
        
        // 鼠标在 UI 上时隐藏预览
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            HideAllPreviews();
            return;
        }
        
        // 获取手持物品
        if (inventory == null || database == null || hotbarSelection == null)
        {
            HideAllPreviews();
            return;
        }
        
        int idx = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
        var slot = inventory.GetSlot(idx);
        
        if (slot.IsEmpty)
        {
            HideAllPreviews();
            return;
        }
        
        var itemData = database.GetItemByID(slot.itemId);
        if (itemData == null)
        {
            HideAllPreviews();
            return;
        }
        
        // 获取鼠标世界坐标并对齐到格子中心
        Vector3 rawWorldPos = GetMouseWorldPosition();
        Vector3 alignedPos = PlacementGridCalculator.GetCellCenter(rawWorldPos);
        
        // 🔥 9.0.5：不再在此处检查 FarmNavState
        // 即使在 Locked/Navigating/Executing 状态，也要调用 UpdateFarmToolPreview
        // 🔴 补丁004：LockPosition 已移除，ghost 预览始终跟随鼠标
        // 实时数据（CurrentCellPos/IsValid/IsInRange）始终更新
        
        // 根据物品类型路由预览
        if (itemData is ToolData tool)
        {
            switch (tool.toolType)
            {
                case ToolType.Hoe:
                    HidePlacementPreview();
                    if (IsPlacementMode)
                    {
                        UpdateFarmToolPreview(alignedPos, true);
                    }
                    else
                    {
                        HideFarmToolPreview();
                    }
                    return;

                case ToolType.WateringCan:
                    HidePlacementPreview();
                    if (IsPlacementMode)
                    {
                        UpdateFarmToolPreview(alignedPos, false);
                    }
                    else
                    {
                        HideFarmToolPreview();
                    }
                    return;
                    
                default:
                    HideAllPreviews();
                    return;
            }
        }
        else if (itemData is SeedData)
        {
            // 🔴 补丁005：种子现在走放置系统预览，不再走 FarmToolPreview
            // HotbarSelectionService 会自动路由到 PlacementManager.EnterPlacementMode
            HideFarmToolPreview();
            return;
        }
        else if (itemData is PlaceableItemData)
        {
            HideFarmToolPreview();
            return;
        }
        else
        {
            HideAllPreviews();
        }
    }
    
    /// <summary>
    /// 更新农田工具预览
    /// 🔥 重构：传递 playerTransform 和 reach 参数
    /// </summary>
    /// <param name="alignedPos">对齐后的世界坐标（格子中心）</param>
    /// <param name="isHoe">true=锄头, false=水壶</param>
    private void UpdateFarmToolPreview(Vector3 alignedPos, bool isHoe)
    {
        // 🔥 使用 Lazy Singleton，Instance getter 会自动创建实例
        var farmPreview = FarmGame.Farm.FarmToolPreview.Instance;
        
        var farmTileManager = FarmGame.Farm.FarmTileManager.Instance;
        if (farmTileManager == null)
        {
            farmPreview.Hide();
            return;
        }
        
        // 获取楼层
        int layerIndex = farmTileManager.GetCurrentLayerIndex(alignedPos);
        var tilemaps = farmTileManager.GetLayerTilemaps(layerIndex);
        
        if (tilemaps == null)
        {
            farmPreview.Hide();
            return;
        }
        
        // 转换为格子坐标
        Vector3Int cellPos = tilemaps.WorldToCell(alignedPos);
        
        // 🔥 获取玩家 Transform
        Transform playerTransform = playerMovement != null ? playerMovement.transform : null;
        
        // 更新预览（传递 playerTransform 和 reach）
        if (isHoe)
        {
            farmPreview.UpdateHoePreview(layerIndex, cellPos, playerTransform, farmToolReach);
        }
        else
        {
            farmPreview.UpdateWateringPreview(layerIndex, cellPos, playerTransform, farmToolReach);
        }
    }
    
    
    /// <summary>
    /// 隐藏所有预览
    /// </summary>
    private void HideAllPreviews()
    {
        HideFarmToolPreview();
        HidePlacementPreview();
    }
    
    /// <summary>
    /// 隐藏农田工具预览
    /// </summary>
    private void HideFarmToolPreview()
    {
        // 🔥 使用 Lazy Singleton，直接调用 Hide
        FarmGame.Farm.FarmToolPreview.Instance.Hide();
    }
    
    /// <summary>
    /// 隐藏放置预览
    /// </summary>
    private void HidePlacementPreview()
    {
        // PlacementPreview 由 PlacementManager 管理
        // 这里只是占位，实际隐藏逻辑在 PlacementManager 中
    }

    private void SyncPlaceableModeWithCurrentSelection()
    {
        var placementManager = PlacementManager.Instance;
        bool placementActive = placementManager != null && placementManager.IsPlacementMode;

        if (inventory == null || database == null || hotbarSelection == null)
        {
            if (placementActive)
            {
                placementManager.ExitPlacementMode();
            }
            return;
        }

        int idx = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
        var slot = inventory.GetSlot(idx);
        if (slot.IsEmpty)
        {
            if (placementActive)
            {
                placementManager.ExitPlacementMode();
            }
            return;
        }

        var itemData = database.GetItemByID(slot.itemId);
        if (itemData == null || !itemData.isPlaceable || !IsPlacementMode)
        {
            if (placementActive)
            {
                placementManager.ExitPlacementMode();
            }
            return;
        }

        if (placementManager != null)
        {
            placementManager.EnterPlacementMode(itemData, slot.quality);
            placementManager.RefreshCurrentPreview();
        }
    }

    void HandleRunToggleWhileNav()
    {
        // ✅ Shift 逻辑已由 SprintStateManager 统一管理，这里不需要处理
        // 导航会自动从 SprintStateManager 获取疾跑状态
    }

    private Vector2 GetManualMovementInput()
    {
        if (useAxisForMovement)
        {
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        float x = (Input.GetKey(KeyCode.D) ? 1f : 0f) + (Input.GetKey(KeyCode.A) ? -1f : 0f);
        float y = (Input.GetKey(KeyCode.W) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f);
        return new Vector2(x, y);
    }

    void HandleMovement()
    {
        // 对话/演出输入锁
        if (!_inputEnabled)
        {
            if (playerMovement != null) playerMovement.SetMovementInput(Vector2.zero, false);
            return;
        }

        // 背包或箱子UI打开时禁用移动输入
        bool uiOpen = IsAnyPanelOpen();
        if (uiOpen)
        {
            if (playerMovement != null) playerMovement.SetMovementInput(Vector2.zero, false);
            return;
        }
        
        Vector2 input = GetManualMovementInput();
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // 🔥 10.1.1补丁002 V4：WASD 中断队列（优先级高于 lockManager）
        bool hasWASD = input.sqrMagnitude > 0.01f;
        bool hasActiveQueue = _farmActionQueue.Count > 0 || _isProcessingQueue;
        if (hasWASD && hasActiveQueue)
        {
            // 🔴 V6 模块Q（CP-Q1/Q2）：绝对锁定原则
            bool wasExecuting = _isExecutingFarming;
            HideFarmToolPreview();
            ClearActionQueue();  // 清空等待队列（模块H已保护执行状态）

            if (!wasExecuting)
            {
                // 没有动画在执行（导航途中或空闲）→ 正常取消一切
                CancelFarmingNavigation();
                ToolActionLockManager.Instance?.ForceUnlock();
            }
            // else: 动画执行中 → 只清空队列，不取消导航/不解锁移动
            // 动画完成后由 OnFarmActionAnimationComplete 回调链正常清理一切
        }

        // 检查是否处于工具动作锁定状态
        var lockManager = ToolActionLockManager.Instance;
        if (lockManager != null && lockManager.IsLocked)
        {
            // 锁定状态：缓存方向输入，不执行移动，也不传递给 PlayerMovement
            if (input.sqrMagnitude > 0.01f)
            {
                lockManager.CacheDirection(input);
            }
            // 重要：清空 PlayerMovement 的输入，防止朝向被更新
            if (playerMovement != null) playerMovement.SetMovementInput(Vector2.zero, false);
            return;
        }

        // 若自动导航激活：
        if (autoNavigator != null && autoNavigator.IsActive)
        {
            // 只要玩家有任意输入则打断导航；否则不要写入移动值，避免覆盖导航输入
            if (Mathf.Abs(input.x) > 0.01f || Mathf.Abs(input.y) > 0.01f)
            {
                autoNavigator.ForceCancel();  // 🔥 P0-1：使用 ForceCancel 替代 Cancel
                // 🔥 9.0.4：手动移动时取消农田导航
                CancelFarmingNavigation();
                // 🔴 补丁004 模块G（CP-G1）：移除 UnlockPosition（ghost 永不锁定）
                if (playerMovement != null) playerMovement.SetMovementInput(input, shift);
            }
            return;
        }
        
        // 🔥 9.0.4：即使没有自动导航，手动移动也要取消农田导航
        if (_farmNavState != FarmNavState.Idle && (Mathf.Abs(input.x) > 0.01f || Mathf.Abs(input.y) > 0.01f))
        {
            CancelFarmingNavigation();
        }
        
        // 🔥 Bug D 修复：WASD 也取消放置系统导航
        if (PlacementManager.Instance != null && 
            (PlacementManager.Instance.CurrentState == PlacementManager.PlacementState.Locked || 
             PlacementManager.Instance.CurrentState == PlacementManager.PlacementState.Navigating) &&
            (Mathf.Abs(input.x) > 0.01f || Mathf.Abs(input.y) > 0.01f))
        {
            PlacementManager.Instance.InterruptFromExternal();
        }

        // 非导航状态，正常写入移动
        if (playerMovement != null) playerMovement.SetMovementInput(input, shift);
    }

    static int s_lastScrollFrame = -1;
    static float s_lastScrollTime = -1f;
    const float ScrollCooldown = 0.08f; // 秒
    
    // 滚轮累积值（用于锁定状态下累积多次滚动）
    private int _accumulatedScrollSteps = 0;

    void HandleHotbarSelection()
    {
        // 面板打开或鼠标在UI上时，禁用滚轮切换工具栏
        bool uiOpen = IsAnyPanelOpen();
        bool pointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        
        // 检查是否处于工具动作锁定状态
        var lockManager = ToolActionLockManager.Instance;
        bool isLocked = lockManager != null && lockManager.IsLocked;
        
        float scroll = (uiOpen || pointerOverUI) ? 0f : Input.mouseScrollDelta.y;
        
        // 滚轮处理
        if (scroll != 0f)
        {
            // 防抖：同一帧只处理一次；并加时间冷却避免一次滚动触发多帧事件
            bool shouldProcess = Time.frameCount != s_lastScrollFrame && 
                                 (Time.unscaledTime - s_lastScrollTime) >= ScrollCooldown;
            
            if (shouldProcess)
            {
                s_lastScrollFrame = Time.frameCount;
                s_lastScrollTime = Time.unscaledTime;
                
                // 计算滚动步数（支持高精度滚轮）
                int scrollSteps = scroll > 0 ? -1 : 1; // 向上滚 = -1（上一个），向下滚 = +1（下一个）
                int currentIndex = hotbarSelection != null ? hotbarSelection.selectedIndex : 0;
                int targetIndex = (currentIndex + scrollSteps) % InventoryService.HotbarWidth;
                if (targetIndex < 0) targetIndex += InventoryService.HotbarWidth;

                if (TryRejectActiveFarmToolSwitch(targetIndex))
                {
                    _accumulatedScrollSteps = 0;
                    return;
                }
                
                if (isLocked)
                {
                    // 锁定状态：累积滚轮步数
                    _accumulatedScrollSteps += scrollSteps;
                    
                    // 计算目标索引（基于当前选中 + 累积步数）
                    currentIndex = hotbarSelection != null ? hotbarSelection.selectedIndex : 0;
                    targetIndex = (currentIndex + _accumulatedScrollSteps) % InventoryService.HotbarWidth;
                    if (targetIndex < 0) targetIndex += InventoryService.HotbarWidth;
                    
                    // 缓存最终目标索引
                    lockManager.CacheHotbarInput(targetIndex);
                }
                else
                {
                    // 正常切换：重置累积值
                    _accumulatedScrollSteps = 0;

                    // 统一交给 HotbarSelectionService 做最终切换与农田态收尾
                    if (scrollSteps > 0) hotbarSelection?.SelectNext();
                    else hotbarSelection?.SelectPrev();
                }
            }
        }
        
        // 解锁时重置累积值
        if (!isLocked && _accumulatedScrollSteps != 0)
        {
            _accumulatedScrollSteps = 0;
        }

        // 数字键直选仅覆盖前 5 格；滚轮仍在 12 格内循环
        // 面板打开时禁用数字键直选
        if (uiOpen) return;
        
        int keyIndex = -1;
        if (Input.GetKeyDown(KeyCode.Alpha1)) keyIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) keyIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) keyIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) keyIndex = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5)) keyIndex = 4;

        if (keyIndex >= InventoryService.HotbarDirectSelectCount)
        {
            keyIndex = -1;
        }
        
        if (keyIndex >= 0)
        {
            if (TryRejectActiveFarmToolSwitch(keyIndex))
            {
                _accumulatedScrollSteps = 0;
                return;
            }

            if (isLocked)
            {
                // 锁定状态：缓存输入（数字键直接指定索引，重置累积值）
                _accumulatedScrollSteps = 0;
                lockManager.CacheHotbarInput(keyIndex);
            }
            else
            {
                // 统一交给 HotbarSelectionService 做最终切换与农田态收尾
                hotbarSelection?.SelectIndex(keyIndex);
            }
        }
    }

    void HandlePanelHotkeys()
    {
        var tabs = EnsurePackageTabs();
        
        // 🔥 9.0.5：ESC 键特殊处理
        // 有面板打开 → 关闭面板（不取消导航）
        // 无面板打开 → 中断导航
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (BoxPanelUI.ActiveInstance != null && BoxPanelUI.ActiveInstance.IsOpen)
            {
                if (tabs != null)
                {
                    tabs.CloseBoxUI(false);
                }
                else
                {
                    BoxPanelUI.ActiveInstance.Close();
                }
                return;
            }
            
            if (tabs != null && tabs.IsPanelOpen())
            {
                tabs.OpenSettings();
                return;
            }
            
            // 🔥 9.0.5：无面板打开时，ESC 中断导航
            // 🔥 10.1.1补丁002：ESC 清空操作队列 + 取消导航（CP-3）
            // 🔴 补丁004 模块A/G：移除 UnlockPosition（ghost 永不锁定）
            HideFarmToolPreview();
            ClearActionQueue();
            CancelFarmingNavigation();
            if (_farmNavState == FarmNavState.Navigating || _farmNavState == FarmNavState.Locked)
            {
                return;
            }

            if (PlacementManager.Instance != null && PlacementManager.Instance.IsPlacementMode)
            {
                return;
            }

            if (tabs != null) tabs.OpenSettings();
            return;
        }
        
        // 🔥 9.0.5：Tab 键 — 不取消导航
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (BoxPanelUI.ActiveInstance != null && BoxPanelUI.ActiveInstance.IsOpen)
            {
                if (tabs != null)
                {
                    tabs.CloseBoxUI(true);
                }
                return;
            }
            if (tabs != null) tabs.OpenProps();
            return;
        }
        
        // 🔥 9.0.5：其他面板热键 — 不取消导航
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (tabs != null) tabs.OpenRecipes();
            return;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (tabs != null) tabs.OpenMap();
            return;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (tabs != null) tabs.OpenEx();
            return;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (tabs != null) tabs.OpenRelations();
            return;
        }
    }
    
    /// <summary>
    /// 检查是否有任何面板打开
    /// </summary>
    private bool IsAnyPanelOpen()
    {
        bool packageOpen = packageTabs != null && packageTabs.IsPanelOpen();
        bool boxOpen = BoxPanelUI.ActiveInstance != null && BoxPanelUI.ActiveInstance.IsOpen;
        return packageOpen || boxOpen;
    }
    
    /// <summary>
    /// 如果箱子面板打开则关闭
    /// </summary>
    private void CloseBoxPanelIfOpen()
    {
        if (BoxPanelUI.ActiveInstance != null && BoxPanelUI.ActiveInstance.IsOpen)
        {
            BoxPanelUI.ActiveInstance.Close();
        }
    }

    void HandleUseCurrentTool()
    {
        // 对话/演出输入锁
        if (!_inputEnabled) return;

        // 任何面板打开时禁用工具使用
        bool uiOpen = IsAnyPanelOpen();
        if (uiOpen) return;

        bool hasManualMovementInput = GetManualMovementInput().sqrMagnitude > 0.01f;
        bool hasPlacementFlow = IsPlacementMode || (PlacementManager.Instance != null && PlacementManager.Instance.IsPlacementMode);
        if (hasManualMovementInput && hasPlacementFlow)
        {
            return;
        }

        // 🔥 10.1.1补丁002：执行保护 — 执行中/动画中的点击走 FIFO 入队（替代旧 CacheFarmInput）
        // V1 漏洞修补：保护分支1（_isExecutingFarming）+ 保护分支2（isPerformingAction）合并
        if (_isExecutingFarming || (playerInteraction != null && playerInteraction.IsPerformingAction()))
        {
            if (Input.GetMouseButtonDown(0) && (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()))
            {
                // CP-11：动画期间可入队 — 先检测收获（CP-5），再检测工具/种子
                TryEnqueueFromCurrentInput();
            }
            return; // 无论是否成功入队，都 return（不穿透到下面的正常流程）
        }

        // 改为 GetMouseButton 支持长按连续使用
        bool isFirstPress = Input.GetMouseButtonDown(0);

        if (!isFirstPress) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        // 🔴 补丁003 模块F（CP-F1）：导航中/执行中的左键点击 → 统一入队
        if (_farmNavState == FarmNavState.Navigating || _farmNavState == FarmNavState.Locked)
        {
            TryEnqueueFromCurrentInput();
            return;
        }

        // ★ 检查是否处于放置模式
        if (PlacementManager.Instance != null && PlacementManager.Instance.IsPlacementMode)
        {
            PlacementManager.Instance.OnLeftClick();
            return;
        }

        if (inventory == null || database == null || hotbarSelection == null) return;

        int idx = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
        var slot = inventory.GetSlot(idx);
        if (slot.IsEmpty) return;

        var itemData = database.GetItemByID(slot.itemId);
        if (itemData == null) return;

        // 🔥 10.1.1补丁002 CP-5：收获检测（最高优先级）— 在所有工具分发之前
        if (TryDetectAndEnqueueHarvest())
            return;

        if (itemData is ToolData tool)
        {
            if (tool.toolType == ToolType.Hoe)
            {
                if (IsPlacementMode)
                {
                    TryEnqueueFarmTool(tool);
                    return;
                }

                if (TryEnqueueHoeRemoveCropFromMouse())
                    return;

                playerInteraction?.RequestAction(ResolveAction(tool.toolType));
                return;
            }

            if (tool.toolType == ToolType.WateringCan)
            {
                if (IsPlacementMode)
                {
                    TryEnqueueFarmTool(tool);
                }
                return;
            }

            // 其他工具正常处理（CP-19：镐子/斧头/武器保持原有逻辑不变）
            var toolAction = ResolveAction(tool.toolType);
            playerInteraction?.RequestAction(toolAction);
        }
        else if (itemData is SeedData)
        {
            // 🔴 补丁005：种子现在走放置系统，不再走 FIFO 入队
            // 放置模式由 HotbarSelectionService 自动路由（isPlaceable=true）
        }
        else if (itemData is WeaponData weapon)
        {
            // 根据武器的动画动作类型决定人物动画
            var action = ResolveWeaponAction(weapon.animActionType);
            playerInteraction?.RequestAction(action);
        }
    }

    void HandleRightClickAutoNav()
    {
        if (!Input.GetMouseButtonDown(1)) return;

        bool boxOpen = BoxPanelUI.ActiveInstance != null && BoxPanelUI.ActiveInstance.IsOpen;
        bool packageOpen = packageTabs != null && packageTabs.IsPanelOpen() && !boxOpen;
        if (packageOpen)
        {
            // 背包打开（非 Box 模式），禁用右键导航
            return;
        }

        if (PlacementManager.Instance != null && PlacementManager.Instance.IsPlacementMode)
        {
            return;
        }

        // 右键属于移动意图：先统一收尾农田自动链，再进入普通导航
        if (HasActiveAutomatedFarmToolOperation())
        {
            InterruptAutomatedFarmToolOperation();
        }
        else if (_farmNavState == FarmNavState.Locked || _farmNavState == FarmNavState.Navigating)
        {
            CancelFarmingNavigation();
        }

        // 🔥 P0-1 修复：Box 打开时，右键点击另一个箱子应该先关闭当前 Box，然后导航到新箱子
        // 但普通背包打开时，右键导航仍然禁用
        // blockNavOverUI 只阻挡导航，不应该阻挡面板热键
        if (blockNavOverUI && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        
        // ★ 农田系统：作物收获已迁移到 IInteractable 链路
        // CropController 实现了 IInteractable，通过 Physics2D.OverlapPointAll 检测
        // 旧的 TryHarvestCropAtMouse 已废弃
        
        if (autoNavigator == null) return;

        // 防抖：点击间隔限制
        float currentTime = Time.unscaledTime;
        if (currentTime - lastNavClickTime < navClickCooldown)
        {
            return;
        }

        var cam = worldCamera != null ? worldCamera : Camera.main;
        if (cam == null) return;
        Vector3 mouse = Input.mousePosition;
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 0f));
        world.z = 0f;

        TryHandleAutoNavWorldClick(world, boxOpen, respectClickRateLimit: true);
    }

    public bool DebugIssueAutoNavClick(Vector2 world)
    {
        bool boxOpen = BoxPanelUI.ActiveInstance != null && BoxPanelUI.ActiveInstance.IsOpen;
        return TryHandleAutoNavWorldClick(world, boxOpen, respectClickRateLimit: false);
    }

    private bool TryHandleAutoNavWorldClick(Vector2 world, bool boxOpen, bool respectClickRateLimit)
    {
        if (autoNavigator == null)
        {
            return false;
        }

        // 点击死区：靠近玩家的区域忽略导航（使用Collider中心）
        Vector2 playerCenter = Vector2.zero;
        if (playerMovement != null)
        {
            var player = playerMovement.transform;
            var col = playerMovement.GetComponent<Collider2D>();
            playerCenter = col != null ? (Vector2)col.bounds.center : (Vector2)player.position;
            if (Vector2.Distance(playerCenter, world) <= navClickDeadzone)
            {
                return false;
            }
        }

        // 防止连续点击同一位置（鬼畜问题）
        if (respectClickRateLimit &&
            autoNavigator.IsActive &&
            Vector3.Distance(world, lastNavClickPos) < minNavDistance)
        {
            // 如果已在导航且点击位置过近，忽略
            return false;
        }

        if (respectClickRateLimit)
        {
            // 更新点击记录
            lastNavClickTime = Time.unscaledTime;
            lastNavClickPos = world;
        }

        // 🔥 C3：优先使用 Sprite Bounds 检测 IResourceNode（箱子、树木等）
        // 因为这些物体的 Collider 只覆盖底部，但交互应该基于整个 Sprite
        var resourceNodes = ResourceNodeRegistry.Instance?.GetNodesInRange(world, 2f);
        if (resourceNodes != null)
        {
            foreach (var node in resourceNodes)
            {
                var bounds = node.GetBounds(); // SpriteRenderer.bounds
                if (bounds.Contains(world))
                {
                    // 点击在 Sprite 范围内，检查是否实现 IInteractable
                    var interactable = node as IInteractable;
                    if (interactable != null)
                    {
                        var nodeGO = (node as MonoBehaviour)?.gameObject;
                        if (nodeGO != null)
                        {
                            // 🔥 P0-1：如果 Box 打开，先关闭再导航
                            if (boxOpen)
                            {
                                CloseBoxPanelIfOpen();
                            }
                            HandleInteractable(interactable, nodeGO.transform, playerCenter);
                            return true;
                        }
                    }
                }
            }
        }

        // 🔥 使用通用目标选择器，收集所有 IInteractable 并按优先级排序
        var hits = Physics2D.OverlapPointAll(world);
        var candidates = new System.Collections.Generic.List<(IInteractable interactable, Transform tr, float distance)>();
        
        foreach (var h in hits)
        {
            // 忽略自身碰撞
            if (playerMovement != null && (h.transform == playerMovement.transform || h.transform.IsChildOf(playerMovement.transform)))
                continue;
            
            // 🔥 关键：从碰撞体或其父级获取 IInteractable
            var interactable = h.GetComponent<IInteractable>();
            if (interactable == null)
                interactable = h.GetComponentInParent<IInteractable>();
            
            if (interactable == null) continue;
            if (interactable is CropController) continue;  // 10.1.1补丁002：作物收获已迁移到左键
            
            float dist = Vector2.Distance(playerCenter, h.transform.position);
            // 稍微放宽范围，允许导航到目标附近
            if (dist > interactable.InteractionDistance * 2f) continue;
            
            candidates.Add((interactable, h.transform, dist));
        }
        
        // 🔥 如果有交互候选，按优先级排序选择目标
        if (candidates.Count > 0)
        {
            // 按优先级降序排序，同优先级时距离近的优先
            candidates.Sort((a, b) =>
            {
                int p = b.interactable.InteractionPriority.CompareTo(a.interactable.InteractionPriority);
                if (p != 0) return p;
                return a.distance.CompareTo(b.distance);
            });
            
            var best = candidates[0];
            
            // 🔥 P0-1：如果 Box 打开，先关闭再导航
            if (boxOpen)
            {
                CloseBoxPanelIfOpen();
            }
            HandleInteractable(best.interactable, best.tr, playerCenter);
            return true;
        }
        
        // 🔥 没有 IInteractable，检查是否有其他可跟随的目标（通过 Tag/Layer）
        Transform found = null;
        foreach (var h in hits)
        {
            if (playerMovement != null && (h.transform == playerMovement.transform || h.transform.IsChildOf(playerMovement.transform)))
                continue;
            
            bool tagMatched = interactableTags != null && interactableTags.Length > 0 && HasAnyTag(h.transform, interactableTags);
            bool layerMatched = ((1 << h.gameObject.layer) & interactableMask.value) != 0;
            if (tagMatched || layerMatched)
            {
                found = h.transform;
                break;
            }
        }

        if (found != null)
        {
            // 🔥 P0-1：如果 Box 打开，先关闭再导航
            if (boxOpen)
            {
                CloseBoxPanelIfOpen();
            }
            autoNavigator.FollowTarget(found, 0.6f);
            return true;
        }
        else
        {
            // 🔥 P0-1：纯导航（无目标）时，如果 Box 打开则禁用
            if (boxOpen)
            {
                return false; // Box 打开时不允许纯导航
            }
            autoNavigator.SetDestination(world);
            return true;
        }
    }
    
    /// <summary>
    /// 🔥 v4.0：统一处理可交互物体
    /// 使用 ClosestPoint 计算距离，确保从任何方向接近都是最短路径
    /// </summary>
    private void HandleInteractable(IInteractable interactable, Transform target, Vector2 playerCenter)
    {
        // 导航开始前取消 Held 状态
        var manager = InventoryInteractionManager.Instance;
        if (manager != null && manager.IsHolding)
        {
            manager.Cancel();
        }
        
        // 🔥 v4.0：使用 ClosestPoint 计算玩家到目标的最近距离
        Vector2 targetPos = GetTargetAnchor(target, playerCenter);
        float distance = Vector2.Distance(playerCenter, targetPos);
        float interactDist = interactable.InteractionDistance;
        
        if (showDebugInfo)
        {
            Debug.Log($"[GameInputManager] HandleInteractable: target={target.name}, distance={distance:F2}, interactDist={interactDist:F2}");
        }
        
        if (distance <= interactDist)
        {
            // 在交互距离内，直接交互
            TryInteract(interactable);
        }
        else
        {
            // 距离太远，导航到目标附近后交互
            if (autoNavigator != null)
            {
                autoNavigator.ForceCancel();
                
                autoNavigator.FollowTarget(target, interactDist * 0.8f, () =>
                {
                    // 到达后距离复核
                    TryInteractWithDistanceCheck(interactable, target);
                });
            }
        }
    }
    
    /// <summary>
    /// 🔥 v4.0：获取目标最近点（使用 ClosestPoint）
    /// 
    /// 核心思路：
    /// 1. 使用 Collider.ClosestPoint(playerPos) 计算玩家到目标的最近点
    /// 2. 这样从任何方向接近都是最短路径，不会绕路
    /// 3. 与 PlayerAutoNavigator 使用相同的距离计算方式
    /// </summary>
    private Vector2 GetTargetAnchor(Transform target, Vector2 playerPos)
    {
        // 尝试获取 Collider
        var collider = target.GetComponent<Collider2D>();
        if (collider == null)
            collider = target.GetComponentInChildren<Collider2D>();
        
        if (collider != null)
        {
            // 🔥 使用 ClosestPoint 计算玩家到 Collider 的最近点
            return collider.ClosestPoint(playerPos);
        }
        
        return target.position;
    }
    
    /// <summary>
    /// 🔥 v4.0：带距离复核的交互（使用 ClosestPoint）
    /// </summary>
    private void TryInteractWithDistanceCheck(IInteractable interactable, Transform target)
    {
        if (interactable == null || target == null) return;
        
        // 获取玩家位置
        Vector2 playerPos = GetPlayerCenter();
        
        // 🔥 v4.0：使用 ClosestPoint 计算距离
        Vector2 targetPos = GetTargetAnchor(target, playerPos);
        float distance = Vector2.Distance(playerPos, targetPos);
        float interactDist = interactable.InteractionDistance;
        
        // 允许 20% 容差
        if (distance > interactDist * 1.2f)
        {
            LogWarningOnce("DistanceTooFar", $"[GameInputManager] 距离过远，取消交互: {distance:F2} > {interactDist * 1.2f:F2}");
            return;
        }
        
        TryInteract(interactable);
    }
    
    /// <summary>
    /// 🔥 P0-1：获取玩家中心位置
    /// </summary>
    private Vector2 GetPlayerCenter()
    {
        if (playerMovement != null)
        {
            var col = playerMovement.GetComponent<Collider2D>();
            return col != null ? (Vector2)col.bounds.center : (Vector2)playerMovement.transform.position;
        }
        return Vector2.zero;
    }
    
    // 🔥 P0-1：警告去重
    private static System.Collections.Generic.HashSet<string> _loggedWarnings = new System.Collections.Generic.HashSet<string>();
    
    private void LogWarningOnce(string key, string message)
    {
        if (!_loggedWarnings.Contains(key))
        {
            _loggedWarnings.Add(key);
            Debug.LogWarning(message);
        }
    }
    
    // 🔥 P0-1：调试开关（默认关闭）
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    /// <summary>
    /// 尝试与可交互物体交互
    /// </summary>
    private void TryInteract(IInteractable interactable)
    {
        if (interactable == null) return;
        
        // 构建交互上下文
        var context = BuildInteractionContext();
        
        // 检查是否可以交互
        if (!interactable.CanInteract(context))
        {
            if (showDebugInfo)
                Debug.Log($"[GameInputManager] 当前无法交互");
            return;
        }
        
        // 执行交互
        interactable.OnInteract(context);
    }
    
    /// <summary>
    /// 构建交互上下文
    /// </summary>
    private InteractionContext BuildInteractionContext()
    {
        var context = new InteractionContext
        {
            Inventory = inventory,
            Database = database,
            Navigator = autoNavigator
        };
        
        // 获取玩家位置
        if (playerMovement != null)
        {
            var col = playerMovement.GetComponent<Collider2D>();
            context.PlayerPosition = col != null ? (Vector2)col.bounds.center : (Vector2)playerMovement.transform.position;
            context.PlayerTransform = playerMovement.transform;
        }
        
        // 获取手持物品信息
        if (inventory != null && hotbarSelection != null)
        {
            int idx = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
            var slot = inventory.GetSlot(idx);
            
            if (!slot.IsEmpty)
            {
                context.HeldItemId = slot.itemId;
                context.HeldItemQuality = slot.quality;
                context.HeldSlotIndex = idx;
            }
        }
        
        return context;
    }

    /// <summary>
    /// 根据工具类型解析对应的玩家动画状态
    /// 
    /// 映射规则：
    /// - Axe（斧头）→ Slice（挥砍）
    /// - Sickle（镰刀）→ Slice（挥砍）
    /// - Pickaxe（镐子）→ Crush（挖掘）
    /// - Hoe（锄头）→ Crush（挖掘）
    /// - WateringCan（洒水壶）→ Watering（浇水）
    /// - FishingRod（鱼竿）→ Fish（钓鱼）
    /// 
    /// 注意：Pierce（刺出）用于长剑等武器，不是工具
    /// </summary>
    PlayerAnimController.AnimState ResolveAction(ToolType type)
    {
        switch (type)
        {
            case ToolType.Axe: return PlayerAnimController.AnimState.Slice;      // 斧头 → 挥砍
            case ToolType.Sickle: return PlayerAnimController.AnimState.Slice;   // 镰刀 → 挥砍
            case ToolType.Pickaxe: return PlayerAnimController.AnimState.Crush;  // 镐子 → 挖掘
            case ToolType.Hoe: return PlayerAnimController.AnimState.Crush;      // 锄头 → 挖掘（修复：之前错误地映射到Pierce）
            case ToolType.WateringCan: return PlayerAnimController.AnimState.Watering; // 洒水壶 → 浇水
            case ToolType.FishingRod: return PlayerAnimController.AnimState.Fish;      // 鱼竿 → 钓鱼
            default: return PlayerAnimController.AnimState.Slice;
        }
    }

    /// <summary>
    /// 根据武器的动画动作类型解析对应的玩家动画状态
    /// 
    /// 映射规则：
    /// - Pierce → Pierce（刺出，长剑）
    /// - Slice → Slice（挥砍）
    /// - 其他 → Slice（默认）
    /// </summary>
    PlayerAnimController.AnimState ResolveWeaponAction(AnimActionType actionType)
    {
        switch (actionType)
        {
            case AnimActionType.Pierce: return PlayerAnimController.AnimState.Pierce;  // 刺出（长剑）
            case AnimActionType.Slice: return PlayerAnimController.AnimState.Slice;    // 挥砍
            case AnimActionType.Crush: return PlayerAnimController.AnimState.Crush;    // 挖掘（如果武器有这种类型）
            default: return PlayerAnimController.AnimState.Slice;
        }
    }

    #region 农田系统集成
    
    /// <summary>
    /// 尝试处理农田工具（锄头、水壶）
    /// </summary>
    /// <param name="tool">工具数据</param>
    /// <returns>是否已处理（true=农田工具已处理，false=非农田工具）</returns>
    private bool TryHandleFarmingTool(ToolData tool)
    {
        if (tool == null) return false;
        
        // 🔥 10.1.0 修复：不再调用 GetMouseWorldPosition()
        // 位置信息已经由实时预览更新到 FarmToolPreview
        // 直接使用 FarmToolPreview 的 CurrentCursorPos
        
        switch (tool.toolType)
        {
            case ToolType.Hoe:
                // 锄头 → 锄地（TryTillSoil 内部从 FarmToolPreview 读取位置）
                return TryTillSoil(Vector3.zero); // 参数已废弃，内部不使用
                
            case ToolType.WateringCan:
                // 水壶 → 浇水（TryWaterTile 内部从 FarmToolPreview 读取位置）
                return TryWaterTile(Vector3.zero); // 参数已废弃，内部不使用
                
            default:
                return false;
        }
    }
    
    /// <summary>
    /// 尝试锄地
    /// 🔥 9.0.4 重构：使用 FarmToolPreview 的 IsValid 和 IsInRange 进行分流
    /// - 近距离：RequestAction + Execute
    /// - 远距离：导航后执行（带快照校验）
    /// 🔥 10.2.0 新增：V键放置模式拦截
    /// </summary>
    private bool TryTillSoil(Vector3 worldPosition)
    {
        // ===== 10.2.0 V键拦截：非放置模式时禁止锄地 =====
        if (!IsPlacementMode)
        {
            return false;
        }
        
        var farmPreview = FarmGame.Farm.FarmToolPreview.Instance;
        
        // 🔥 Step 1: 检查目标是否有效（不含距离）
        if (farmPreview == null || !farmPreview.IsValid())
        {
            if (showDebugInfo)
                Debug.Log("[GameInputManager] 锄地失败：目标无效（红框状态）");
            return false;
        }
        
        // 🔥 Step 2: 获取目标位置
        Vector3 targetPos = farmPreview.CurrentCursorPos;
        int layerIndex = farmPreview.CurrentLayerIndex;
        Vector3Int cellPos = farmPreview.CurrentCellPos;
        
        // 🔴 补丁004 模块A/G：移除 LockPosition（ghost 永不锁定）
        _farmNavState = FarmNavState.Locked;
        
        // 🔥 Step 3: 距离分流
        if (farmPreview.IsInRange)
        {
            // A. 近距离 → Executing → 执行 → Preview
            _farmNavState = FarmNavState.Executing;
            _isExecutingFarming = true;
            try
            {
                if (TryStartPlayerAction(PlayerAnimController.AnimState.Crush))
                {
                    ExecuteTillSoil(layerIndex, cellPos);
                }
            }
            finally
            {
                _isExecutingFarming = false;
                _farmNavState = FarmNavState.Preview;
            }
            return true;
        }
        else
        {
            // B. 远距离 → 导航后执行
            int slotIndex = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
            var slot = inventory.GetSlot(slotIndex);
            _farmingSnapshot = FarmingSnapshot.Create(slot.itemId, slotIndex, 0);
            
            int cachedLayerIndex = layerIndex;
            Vector3Int cachedCellPos = cellPos;
            
            StartFarmingNavigation(targetPos, () =>
            {
                // 🔥 到达后强制校验
                if (!ValidateToolSnapshot())
                {
                    if (showDebugInfo)
                        Debug.Log("[GameInputManager] 锄地取消：手持物品已变化");
                    ClearSnapshot();
                    return;
                }
                
                // 🔴 补丁004：使用闭包捕获的 targetPos 做距离校验（替代已移除的 LockedWorldPos）
                Vector2 playerPos = GetPlayerCenter();
                float distToTarget = Vector2.Distance(playerPos, targetPos);
                if (distToTarget > farmToolReach)
                {
                    if (showDebugInfo)
                        Debug.Log($"[GameInputManager] 锄地取消：距离目标过远 {distToTarget:F2} > {farmToolReach:F2}");
                    ClearSnapshot();
                    return;
                }
                
                // 🔥 全部校验通过，执行动作
                if (TryStartPlayerAction(PlayerAnimController.AnimState.Crush))
                {
                    ExecuteTillSoil(cachedLayerIndex, cachedCellPos);
                }
                ClearSnapshot();
            });
            return true;
        }
    }
    
    /// <summary>
    /// 执行锄地动作（纯逻辑，不含距离检查）
    /// 🔥 10.0.1：增加枯萎未成熟作物清除逻辑
    /// </summary>
    private bool ExecuteTillSoil(int layerIndex, Vector3Int cellPos)
    {
        var farmTileManager = FarmGame.Farm.FarmTileManager.Instance;
        if (farmTileManager == null)
        {
            Debug.LogError("[GameInputManager] 锄地失败：FarmTileManager.Instance 为空！");
            return false;
        }
        
        if (!farmTileManager.CanTillAt(layerIndex, cellPos))
        {
            if (showDebugInfo)
                Debug.Log($"[GameInputManager] CanTillAt 返回 false: Layer={layerIndex}, Pos={cellPos}");
            return false;
        }
        
        bool success = farmTileManager.CreateTile(layerIndex, cellPos);

        if (success)
        {
            CommitCurrentToolUse(GetCurrentHeldToolData(), $"Farm/Till L{layerIndex} {cellPos}");
        }
        
        if (showDebugInfo)
            Debug.Log($"[GameInputManager] 锄地{(success ? "成功" : "失败")}: Layer={layerIndex}, Pos={cellPos}");
        
        return success;
    }
    /// <summary>
    /// 🔴 V6 模块S（CP-S4）：清除耕地上的农作物（任何状态）
    /// </summary>
    private bool ExecuteRemoveCrop(int layerIndex, Vector3Int cellPos)
    {
        var farmTileManager = FarmGame.Farm.FarmTileManager.Instance;
        if (farmTileManager == null) return false;

        var tileData = farmTileManager.GetTileData(layerIndex, cellPos);
        if (tileData?.cropController != null)
        {
            // 🔴 004-P3：动画中期只隐藏视觉，不销毁数据
            // 真正的 RemoveCrop 在 OnFarmActionAnimationComplete 兜底中执行
            tileData.cropController.HideCropVisuals();
            CommitCurrentToolUse(GetCurrentHeldToolData(), $"Farm/RemoveCrop L{layerIndex} {cellPos}");
            if (showDebugInfo)
                Debug.Log($"[GameInputManager] 隐藏农作物视觉: Layer={layerIndex}, Pos={cellPos}");
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 尝试浇水
    /// 🔥 9.0.4 重构：使用 FarmToolPreview 的 IsValid 和 IsInRange 进行分流
    /// - 近距离：RequestAction + Execute
    /// - 远距离：导航后执行（带快照校验）
    /// 🔥 10.2.0 新增：V键放置模式拦截
    /// </summary>
    private bool TryWaterTile(Vector3 worldPosition)
    {
        // ===== 10.2.0 V键拦截：非放置模式时禁止浇水 =====
        if (!IsPlacementMode)
        {
            return false;
        }
        
        var farmPreview = FarmGame.Farm.FarmToolPreview.Instance;
        
        // 🔥 Step 1: 检查目标是否有效
        if (farmPreview == null || !farmPreview.IsValid())
        {
            if (showDebugInfo)
            {
                // 🔥 9.0.5：输出浇水失败原因
                var reason = farmPreview != null ? farmPreview.LastWateringFailure : FarmGame.Farm.FarmToolPreview.WateringFailureReason.ManagerNull;
                Debug.Log($"[GameInputManager] 浇水失败：目标无效（{reason}）");
            }
            return false;
        }
        
        // 🔥 Step 2: 获取目标位置
        Vector3 targetPos = farmPreview.CurrentCursorPos;
        int layerIndex = farmPreview.CurrentLayerIndex;
        Vector3Int cellPos = farmPreview.CurrentCellPos;
        
        // 🔴 补丁004 模块A/G：移除 LockPosition（ghost 永不锁定）
        _farmNavState = FarmNavState.Locked;
        
        // 🔥 Step 3: 距离分流
        if (farmPreview.IsInRange)
        {
            // A. 近距离 → Executing → 执行 → Preview
            _farmNavState = FarmNavState.Executing;
            _isExecutingFarming = true;
            try
            {
                if (TryStartPlayerAction(PlayerAnimController.AnimState.Watering))
                {
                    ExecuteWaterTile(layerIndex, cellPos);
                }
            }
            finally
            {
                _isExecutingFarming = false;
                _farmNavState = FarmNavState.Preview;
            }
            return true;
        }
        else
        {
            // B. 远距离 → 导航后执行
            int slotIndex = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
            var slot = inventory.GetSlot(slotIndex);
            _farmingSnapshot = FarmingSnapshot.Create(slot.itemId, slotIndex, 0);
            
            int cachedLayerIndex = layerIndex;
            Vector3Int cachedCellPos = cellPos;
            
            StartFarmingNavigation(targetPos, () =>
            {
                if (!ValidateToolSnapshot())
                {
                    if (showDebugInfo)
                        Debug.Log("[GameInputManager] 浇水取消：手持物品已变化");
                    ClearSnapshot();
                    return;
                }
                
                // 🔴 补丁004：使用闭包捕获的 targetPos 做距离校验（替代已移除的 LockedWorldPos）
                Vector2 playerPos = GetPlayerCenter();
                float distToTarget = Vector2.Distance(playerPos, targetPos);
                if (distToTarget > farmToolReach)
                {
                    if (showDebugInfo)
                        Debug.Log($"[GameInputManager] 浇水取消：距离目标过远 {distToTarget:F2} > {farmToolReach:F2}");
                    ClearSnapshot();
                    return;
                }
                
                if (TryStartPlayerAction(PlayerAnimController.AnimState.Watering))
                {
                    ExecuteWaterTile(cachedLayerIndex, cachedCellPos);
                }
                ClearSnapshot();
            });
            return true;
        }
    }
    
    /// <summary>
    /// 执行浇水动作（纯逻辑，不含距离检查）
    /// </summary>
    private bool ExecuteWaterTile(int layerIndex, Vector3Int cellPos, int puddleVariant = -1)
    {
        var farmTileManager = FarmGame.Farm.FarmTileManager.Instance;
        if (farmTileManager == null)
        {
            if (showDebugInfo)
                Debug.Log("[GameInputManager] FarmTileManager 未初始化");
            return false;
        }
        
        float currentHour = TimeManager.Instance != null ? TimeManager.Instance.GetHour() : 0f;
        // 🔴 V3 模块L（CP-L2）：传递预分配的 puddleVariant，确保预览与实际一致
        bool success = farmTileManager.SetWatered(layerIndex, cellPos, currentHour, puddleVariant);
        
        // 🔴 V6 模块T（CP-T4）：浇水成功后通知预览系统需要在移出格子时随机新样式
        if (success)
        {
            CommitCurrentToolUse(GetCurrentHeldToolData(), $"Farm/Water L{layerIndex} {cellPos}");
            FarmToolPreview.Instance?.OnWaterExecuted(cellPos);
        }
        
        if (showDebugInfo)
            Debug.Log($"[GameInputManager] 浇水{(success ? "成功" : "失败")}: Layer={layerIndex}, Pos={cellPos}, variant={puddleVariant}");
        
        return success;
    }
    
    
    
    
    /// <summary>
    /// 获取鼠标世界坐标
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        var cam = worldCamera != null ? worldCamera : Camera.main;
        if (cam == null) return Vector3.zero;
        
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0f));
        worldPos.z = 0f;
        
        return worldPos;
    }
    
    /// <summary>
    /// [已废弃] 尝试在鼠标位置收获作物
    /// 收获统一走 IInteractable → CropController.Harvest()
    /// </summary>
    [System.Obsolete("10.X 纠正：收获统一走 IInteractable → CropController.Harvest()")]
    private bool TryHarvestCropAtMouse()
    {
        // 直接使用 FarmTileManager
        var farmTileManager = FarmGame.Farm.FarmTileManager.Instance;
        if (farmTileManager == null) return false;
        
        // 直接使用 CropManager
        var cropManager = FarmGame.Farm.CropManager.Instance;
        if (cropManager == null) return false;
        
        Vector3 worldPos = GetMouseWorldPosition();
        
        // 获取当前楼层
        int layerIndex = farmTileManager.GetCurrentLayerIndex(worldPos);
        var tilemaps = farmTileManager.GetLayerTilemaps(layerIndex);
        if (tilemaps == null || !tilemaps.IsValid())
        {
            return false;
        }
        
        // 转换为格子坐标
        Vector3Int cellPosition = tilemaps.WorldToCell(worldPos);
        
        // 获取耕地数据
        var tileData = farmTileManager.GetTileData(layerIndex, cellPosition);
        if (tileData == null || !tileData.HasCrop())
        {
            return false;
        }
        
        // 获取种子数据
        SeedData seedData = null;
        if (database != null && tileData.cropData != null)
        {
            seedData = database.GetItemByID(tileData.cropData.seedDataID) as SeedData;
        }
        
        // 尝试收获
        if (cropManager.TryHarvest(layerIndex, cellPosition, tileData, seedData, out int cropID, out int amount))
        {
            // 添加到背包
            if (cropID > 0 && amount > 0 && inventory != null)
            {
                inventory.AddItem(cropID, 0, amount);
            }
            
            if (showDebugInfo)
                Debug.Log($"[GameInputManager] 收获成功: CropID={cropID}, Amount={amount}");
            return true;
        }
        
        return false;
    }
    
    #endregion

    static bool HasAnyTag(Transform t, string[] tags)
    {
        if (t == null || tags == null) return false;
        foreach (var tag in tags)
        {
            if (!string.IsNullOrEmpty(tag) && t.CompareTag(tag)) return true;
        }
        var p = t.parent;
        while (p != null)
        {
            foreach (var tag in tags)
            {
                if (!string.IsNullOrEmpty(tag) && p.CompareTag(tag)) return true;
            }
            p = p.parent;
        }
        return false;
    }

    PackagePanelTabsUI EnsurePackageTabs()
    {
        if (packageTabs == null)
        {
            packageTabs = ResolvePackageTabs();
        }
        if (packageTabs != null && !packageTabsInitialized)
        {
            packageTabs.EnsureReady();
            packageTabsInitialized = true;
        }
        return packageTabs;
    }

    PackagePanelTabsUI ResolvePackageTabs()
    {
        var uiRoot = ResolveUIRoot();
        if (uiRoot != null)
        {
            var tabs = uiRoot.GetComponentInChildren<PackagePanelTabsUI>(true);
            if (tabs != null) return tabs;
        }
        return FindFirstObjectByType<PackagePanelTabsUI>(FindObjectsInactive.Include);
    }

    GameObject ResolveUIRoot()
    {
        if (uiRootCache != null) return uiRootCache;
        var scene = gameObject.scene;
        if (scene.IsValid())
        {
            var roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                if (roots[i].name == uiRootName)
                {
                    uiRootCache = roots[i];
                    return uiRootCache;
                }
            }
        }
        var fallback = GameObject.Find(uiRootName);
        if (fallback != null) uiRootCache = fallback;
        return uiRootCache;
    }
    
    #region 🔥 9.0.4 农田智能导航
    
    /// <summary>
    /// 启动农田工具导航
    /// </summary>
    /// <param name="targetPos">目标位置（格子中心）</param>
    /// <param name="onArrived">到达后的回调</param>
    private void StartFarmingNavigation(Vector3 targetPos, System.Action onArrived)
    {
        if (autoNavigator == null)
        {
            Debug.LogWarning("[GameInputManager] PlayerAutoNavigator 未初始化，无法导航");
            return;
        }
        
        // 🔥 9.0.5 修复：不调用 CancelFarmingNavigation()！
        // 🔴 补丁004：LockPosition 已移除，此处只清理旧协程和导航器。
        if (_farmingNavigationCoroutine != null)
        {
            StopCoroutine(_farmingNavigationCoroutine);
            _farmingNavigationCoroutine = null;
        }
        if (autoNavigator.IsActive)
        {
            autoNavigator.ForceCancel();
        }
        
        // 设置状态
        _farmNavState = FarmNavState.Navigating;
        _farmNavigationAction = onArrived;
        _farmNavigationTarget = targetPos;
        
        // 计算停止距离（略小于工具使用距离）
        float stopDistance = farmToolReach * 0.8f;
        
        // 使用 SetDestination 导航到目标点
        autoNavigator.SetDestination(targetPos);
        
        // 启动监控协程
        _farmingNavigationCoroutine = StartCoroutine(WaitForNavigationComplete(targetPos, stopDistance, onArrived));
        
        if (showDebugInfo)
            Debug.Log($"[GameInputManager] 启动农田导航: target={targetPos}, stopDist={stopDistance:F2}");
    }
    
    /// <summary>
    /// 等待导航完成的协程
    /// </summary>
    private System.Collections.IEnumerator WaitForNavigationComplete(Vector3 targetPos, float stopDistance, System.Action onArrived)
    {
        // 等待导航开始
        yield return null;
        
        // 监控导航状态
        while (autoNavigator != null && autoNavigator.IsActive && _farmNavState == FarmNavState.Navigating)
        {
            Vector2 playerPos = GetPlayerCenter();
            float distance = Vector2.Distance(playerPos, targetPos);
            
            if (distance <= stopDistance)
            {
                autoNavigator.ForceCancel();
                
                if (showDebugInfo)
                    Debug.Log($"[FarmNav] 到达目标, distance={distance:F2}");
                
                // 🔴 004-P0：协程只负责导航和调用回调，不管理 _isExecutingFarming
                // ExecuteFarmAction 全权管理执行状态的生命周期
                _farmNavState = FarmNavState.Executing;
                onArrived?.Invoke();
                
                // 回调完成后清理导航状态（执行状态由 ExecuteFarmAction → OnFarmActionAnimationComplete 管理）
                _farmNavState = IsHoldingFarmTool() ? FarmNavState.Preview : FarmNavState.Idle;
                _farmNavigationAction = null;
                _farmingNavigationCoroutine = null;
                _farmNavigationTarget = Vector3.zero;
                
                yield break;
            }
            
            yield return null;
        }
        
        // 导航结束（可能被取消或卡住）
        Vector2 finalPos = GetPlayerCenter();
        float finalDistance = Vector2.Distance(finalPos, targetPos);
        
        if (_farmNavState == FarmNavState.Navigating && finalDistance <= stopDistance * 1.2f)
        {
            if (showDebugInfo)
                Debug.Log($"[FarmNav] 导航结束但在范围内，执行回调: distance={finalDistance:F2}");
            
            // 🔴 004-P0：协程只负责导航和调用回调，不管理 _isExecutingFarming
            _farmNavState = FarmNavState.Executing;
            onArrived?.Invoke();
            
            // 回调完成后清理导航状态
            _farmNavState = IsHoldingFarmTool() ? FarmNavState.Preview : FarmNavState.Idle;
            _farmNavigationAction = null;
            _farmingNavigationCoroutine = null;
            _farmNavigationTarget = Vector3.zero;
        }
        else
        {
            if (showDebugInfo && _farmNavState == FarmNavState.Navigating)
                Debug.Log($"[FarmNav] 导航结束但距离过远: distance={finalDistance:F2}");
            
            // 🔴 补丁004 模块A/G：移除 UnlockPosition（ghost 永不锁定）
            // 🔥 10.1.1-F2：安全网 — 确保 lockManager 也解锁（防止永久卡死）
            var lockMgr = ToolActionLockManager.Instance;
            if (lockMgr != null && lockMgr.IsLocked)
            {
                lockMgr.ForceUnlock();
            }
            _farmNavState = IsHoldingFarmTool() ? FarmNavState.Preview : FarmNavState.Idle;
            _farmNavigationAction = null;
            _farmingNavigationCoroutine = null;
            _farmNavigationTarget = Vector3.zero;
        }
    }
    
    /// <summary>
    /// 取消农田导航
    /// </summary>
    private void CancelFarmingNavigation()
    {
        if (_farmNavState == FarmNavState.Idle || _farmNavState == FarmNavState.Preview) return;
        
        if (showDebugInfo)
            Debug.Log($"[FarmNav] 取消导航: state={_farmNavState}");
        
        // 停止协程
        if (_farmingNavigationCoroutine != null)
        {
            StopCoroutine(_farmingNavigationCoroutine);
            _farmingNavigationCoroutine = null;
        }
        
        // 🔴 补丁004 模块G（CP-G1）：移除 UnlockPosition（ghost 永不锁定）
        ClearPausedFarmingNavigation();
        _farmNavigationTarget = Vector3.zero;
        
        // 🔴 V6 模块Q（CP-Q3）：执行中不清除标志，不解锁
        if (_isExecutingFarming)
        {
            // 动画正在执行，只清理导航相关状态，保留执行状态
            _farmNavState = FarmNavState.Preview;
            _farmNavigationAction = null;
            ClearSnapshot();
            return;  // 不清除 _isExecutingFarming，不 ForceUnlock
        }
        
        // 🔥 9.0.5：重置状态 → 回到 Preview（而非 Idle）
        // 如果仍持有农具/种子，回到 Preview；否则回到 Idle
        _farmNavState = IsHoldingFarmTool() ? FarmNavState.Preview : FarmNavState.Idle;
        _farmNavigationAction = null;
        _isExecutingFarming = false;
        ClearSnapshot();
        
        // 🔥 10.1.1 方案E：取消导航时清除输入缓存（防止过时缓存被消费）
        // ⚠️ 10.1.1补丁002：旧缓存字段已废弃，保留赋值作为安全网
#pragma warning disable CS0612 // 已知废弃，保留作为安全网
#pragma warning restore CS0612
        
        // 🔥 10.1.1-F2：安全网 — 确保 lockManager 也解锁（防止永久卡死）
        var lockMgr = ToolActionLockManager.Instance;
        if (lockMgr != null && lockMgr.IsLocked)
        {
            lockMgr.ForceUnlock();
        }
    }

    private void ClearPausedFarmingNavigation()
    {
        _hasPausedFarmNavigation = false;
        _pausedFarmNavigationTarget = Vector3.zero;
        _pausedFarmNavigationAction = null;
    }

    private void PauseCurrentNavigationForUI()
    {
        bool hasActiveNavigation = _farmNavState == FarmNavState.Navigating ||
                                   _farmingNavigationCoroutine != null ||
                                   (autoNavigator != null && autoNavigator.IsActive);
        if (!hasActiveNavigation)
        {
            return;
        }

        _hasPausedFarmNavigation = true;
        _pausedFarmNavigationTarget = _farmNavigationTarget;
        _pausedFarmNavigationAction = _farmNavigationAction;

        if (_farmingNavigationCoroutine != null)
        {
            StopCoroutine(_farmingNavigationCoroutine);
            _farmingNavigationCoroutine = null;
        }

        if (autoNavigator != null && autoNavigator.IsActive)
        {
            autoNavigator.ForceCancel();
        }

        _farmNavigationAction = null;
        _farmNavState = FarmNavState.Locked;
    }

    private bool ResumePausedFarmNavigationAfterUI()
    {
        if (!_hasPausedFarmNavigation || _isExecutingFarming || _pausedFarmNavigationAction == null)
        {
            return false;
        }

        Vector3 target = _pausedFarmNavigationTarget;
        System.Action onArrived = _pausedFarmNavigationAction;
        ClearPausedFarmingNavigation();
        StartFarmingNavigation(target, onArrived);
        return true;
    }

    private void CaptureProtectedHeldSlotForUIFreeze()
    {
        if (TryGetRuntimeProtectedHeldSlotIndex(out int protectedSlotIndex))
        {
            _hasUIFrozenProtectedHeldSlot = true;
            _uiFrozenProtectedHeldSlotIndex = protectedSlotIndex;
            return;
        }

        ClearProtectedHeldSlotForUIFreeze();
    }

    private void ClearProtectedHeldSlotForUIFreeze()
    {
        _hasUIFrozenProtectedHeldSlot = false;
        _uiFrozenProtectedHeldSlotIndex = -1;
    }
    
    /// <summary>
    /// 🔥 10.1.1补丁002 任务5.3：轻量版导航取消（面板暂停专用）
    /// 只停止导航协程和导航器，不清空队列、不解锁预览、不重置 _farmNavState、不重置 _isExecutingFarming
    /// </summary>
    private void CancelCurrentNavigation()
    {
        // 停止导航协程
        if (_farmingNavigationCoroutine != null)
        {
            StopCoroutine(_farmingNavigationCoroutine);
            _farmingNavigationCoroutine = null;
        }
        
        // 停止导航器
        if (autoNavigator != null && autoNavigator.IsActive)
        {
            autoNavigator.ForceCancel();
        }
        
        _farmNavigationAction = null;

        bool isExecuting = _isExecutingFarming || (playerInteraction != null && playerInteraction.IsPerformingAction());
        if (!isExecuting)
        {
            _farmNavState = IsHoldingFarmTool() ? FarmNavState.Preview : FarmNavState.Idle;
            ClearSnapshot();
        }
    }
    
    /// <summary>
    /// 🔥 9.0.5 新增：检查当前是否手持农具或种子
    /// </summary>
    public bool IsHoldingFarmTool()
    {
        if (inventory == null || database == null || hotbarSelection == null) return false;

        int idx = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
        var slot = inventory.GetSlot(idx);
        if (slot.IsEmpty) return false;

        var itemData = database.GetItemByID(slot.itemId);
        if (itemData is ToolData tool)
        {
            if (tool.toolType == ToolType.Hoe)
            {
                return IsPlacementMode;
            }

            if (tool.toolType == ToolType.WateringCan)
            {
                return IsPlacementMode;
            }
        }
        return false;
    }
    
    /// <summary>
    /// 校验快照是否仍然有效（用于种子，检查数量）
    /// </summary>
    private bool ValidateSnapshot()
    {
        if (!_farmingSnapshot.isValid) return false;
        if (inventory == null || database == null) return false;
        
        var slot = inventory.GetSlot(_farmingSnapshot.slotIndex);
        
        // 校验：槽位非空 && 物品 ID 匹配 && 数量足够
        return !slot.IsEmpty && 
               slot.itemId == _farmingSnapshot.itemId && 
               slot.amount >= _farmingSnapshot.count;
    }
    
    /// <summary>
    /// 校验工具快照是否仍然有效（工具不消耗，只检查 ID）
    /// </summary>
    private bool ValidateToolSnapshot()
    {
        if (!_farmingSnapshot.isValid) return false;
        if (inventory == null || database == null) return false;
        
        var slot = inventory.GetSlot(_farmingSnapshot.slotIndex);
        
        // 工具校验：槽位非空 && 物品 ID 匹配（不检查数量，工具不消耗）
        return !slot.IsEmpty && slot.itemId == _farmingSnapshot.itemId;
    }
    
    /// <summary>
    /// 清除快照
    /// </summary>
    private void ClearSnapshot()
    {
        _farmingSnapshot = FarmingSnapshot.Invalid;
    }
    
    /// <summary>
    /// 检查当前手持物品是否是指定种子
    /// </summary>
    private bool IsHoldingSameSeed(SeedData expectedSeed)
    {
        if (inventory == null || database == null || hotbarSelection == null)
            return false;
        
        int idx = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
        var slot = inventory.GetSlot(idx);
        
        if (slot.IsEmpty) return false;
        
        var itemData = database.GetItemByID(slot.itemId);
        return itemData is SeedData seed && seed.itemID == expectedSeed.itemID;
    }

#if LEGACY_PENDING_FARM_INPUT

    #region 10.1.0 输入缓存系统
    
    /// <summary>是否有待消费的农田输入缓存</summary>
    [System.Obsolete("10.1.1补丁002：被 FIFO 队列替代，使用 _farmActionQueue.Count > 0 代替")]
    public bool HasPendingFarmInput => _hasPendingFarmInput;
    
    /// <summary>
    /// 清除农田输入缓存并解锁预览（10.1.1 方案Q4：切换工具栏时丢弃缓存）
    /// </summary>
    [System.Obsolete("10.1.1补丁002：被 ClearActionQueue 替代")]
    /// <summary>
    /// 缓存农田输入（动画/执行期间调用，后来的覆盖前面的）
    /// </summary>
    /// <summary>
    /// 缓存农田输入（动画/执行期间调用，后来的覆盖前面的）
    /// 🔥 10.1.1 修正：缓存时锁定预览 + 刷新 1+8 GhostTilemap（修复 A-1/A-2/A-3/A-4）
    /// </summary>
    [System.Obsolete("10.1.1补丁002：被 EnqueueAction 替代")]
    private void CacheFarmInput(int itemId)
    {
        var worldPos = GetMouseWorldPosition();

        // 🔥 10.1.1 A-3：获取 ItemData 用于 ForceUpdatePreviewToPosition
        var itemData = database != null ? database.GetItemByID(itemId) : null;

        var farmPreview = FarmGame.Farm.FarmToolPreview.Instance;
        var farmTileManager = FarmGame.Farm.FarmTileManager.Instance;

        if (farmPreview != null && itemData != null && farmTileManager != null)
        {
            // 🔴 补丁004 模块A/G：移除 UnlockPosition/LockPosition（ghost 永不锁定）

            // 🔥 10.1.1 A-1：刷新完整预览（含 1+8 GhostTilemap）到缓存位置
            ForceUpdatePreviewToPosition(worldPos, itemData);
        }

        _hasPendingFarmInput = true;
        _pendingFarmWorldPos = worldPos;
        _pendingFarmItemId = itemId;
    }
    
    /// <summary>
    /// 获取当前手持物品 ID（用于缓存消费时验证一致性）
    /// </summary>
    private int GetCurrentHeldItemId()
    {
        if (inventory == null || database == null || hotbarSelection == null)
            return -1;
        
        int idx = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
        var slot = inventory.GetSlot(idx);
        return slot.IsEmpty ? -1 : slot.itemId;
    }
    
    /// <summary>
    /// 消费缓存的农田输入（由 PlayerInteraction.OnActionComplete 回调触发）
    /// </summary>
    [System.Obsolete("10.1.1补丁002：被 ProcessNextAction 替代")]
    public void ConsumePendingFarmInput()
    {
        if (!_hasPendingFarmInput) return;

        _hasPendingFarmInput = false;

        // 验证手持物品一致性（E-2：动画期间切换物品则缓存失效）
        int currentItemId = GetCurrentHeldItemId();
        if (currentItemId != _pendingFarmItemId) return;

        // 🔴 补丁004 模块A/G：移除 UnlockPosition（ghost 永不锁定）

        // 以缓存的世界坐标执行完整动作链
        ProcessFarmInputAt(_pendingFarmWorldPos);
    }
    
    /// <summary>
    /// 以指定世界坐标执行完整农田输入处理
    /// 长按时也通过此方法以当前鼠标位置执行
    /// 🔥 10.1.0 修复：先更新 FarmToolPreview 到指定位置，确保后续方法使用正确的位置
    /// </summary>
    [System.Obsolete("10.1.1补丁002：被队列内部逻辑替代")]
    public void ProcessFarmInputAt(Vector3 worldPos)
    {
        if (inventory == null || database == null || hotbarSelection == null) return;
        
        int idx = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
        var slot = inventory.GetSlot(idx);
        if (slot.IsEmpty) return;
        
        var itemData = database.GetItemByID(slot.itemId);
        if (itemData == null) return;
        
        // 🔥 10.1.0 核心修复：先将 FarmToolPreview 更新到缓存的位置
        // 这样后续 TryTillSoil/TryWaterTile/TryPlantSeed 从 Preview 读取时就是正确的位置
        ForceUpdatePreviewToPosition(worldPos, itemData);
        
        if (itemData is ToolData tool)
        {
            if (tool.toolType == ToolType.Hoe || tool.toolType == ToolType.WateringCan)
            {
                TryHandleFarmingTool(tool);
                return;
            }
        }
        else if (itemData is SeedData seedData)
        {
            // 🔴 补丁005：种子已迁移到放置系统，此分支不再执行
        }
    }
    
    /// <summary>
    /// 🔥 10.1.0 新增：强制将 FarmToolPreview 更新到指定位置
    /// 用于缓存消费时，确保 Preview 状态与缓存位置一致
    /// </summary>
    [System.Obsolete("10.1.1补丁002：队列通过 LockPosition 处理预览")]
    private void ForceUpdatePreviewToPosition(Vector3 worldPos, ItemData itemData)
    {
        var farmPreview = FarmGame.Farm.FarmToolPreview.Instance;
        var farmTileManager = FarmGame.Farm.FarmTileManager.Instance;
        if (farmPreview == null || farmTileManager == null) return;
        
        // 对齐到格子中心
        Vector3 alignedPos = PlacementGridCalculator.GetCellCenter(worldPos);
        
        // 获取楼层和格子坐标
        int layerIndex = farmTileManager.GetCurrentLayerIndex(alignedPos);
        var tilemaps = farmTileManager.GetLayerTilemaps(layerIndex);
        if (tilemaps == null) return;
        
        Vector3Int cellPos = tilemaps.WorldToCell(alignedPos);
        Transform playerTransform = playerMovement != null ? playerMovement.transform : null;
        
        // 根据物品类型更新对应的预览
        if (itemData is ToolData tool)
        {
            if (tool.toolType == ToolType.Hoe)
            {
                farmPreview.UpdateHoePreview(layerIndex, cellPos, playerTransform, farmToolReach);
            }
            else if (tool.toolType == ToolType.WateringCan)
            {
                farmPreview.UpdateWateringPreview(layerIndex, cellPos, playerTransform, farmToolReach);
            }
        }
        else if (itemData is SeedData)
        {
            // 🔴 补丁005：种子已迁移到放置系统，此分支不再需要
        }
    }
    
    #endregion
    
    #endif
    #region ===== 10.1.1 补丁002：FIFO 操作队列方法 =====
    
    /// <summary>
    /// 将农田操作请求入队。防重复：同一 (layerIndex, cellPos) 不重复入队；
    /// Harvest 额外检查同一 CropController 实例不重复。
    /// 如果队列之前为空且未暂停 → 启动 ProcessNextAction。
    /// </summary>
    private void EnqueueAction(FarmActionRequest request, Dictionary<Vector3Int, TileBase> ghostTileData = null)
    {
        // CP-2：防重复 — 同一格子不重复入队
        var key = (request.layerIndex, request.cellPos);
        if (_queuedPositions.Contains(key)) return;
        
        // Harvest 额外防重复：同一 CropController 实例不重复
        if (request.type == FarmActionType.Harvest)
        {
            foreach (var existing in _farmActionQueue)
            {
                if (existing.type == FarmActionType.Harvest && existing.targetCrop == request.targetCrop)
                    return;
            }
        }
        
        _queuedPositions.Add(key);
        _farmActionQueue.Enqueue(request);
        
        // 🔴 补丁004 模块G（CP-G4/G5）：入队成功时添加队列预览，传递 ghostTileData
        FarmToolPreview.Instance?.AddQueuePreview(request.cellPos, request.layerIndex, request.type, request.puddleVariant, ghostTileData);
        
        // 队列之前为空且未暂停且没有正在执行的操作 → 启动处理
        if (!_isProcessingQueue && !_isQueuePaused)
        {
            ProcessNextAction();
        }
    }
    
    /// <summary>
    /// 从队列取出下一个操作并执行。
    /// 暂停检查 → 队列空则结束 → 出队 → 二次验证 → 距离判断 → 近距离直接执行 / 远距离导航。
    /// </summary>
    private void ProcessNextAction()
    {
        // V5：暂停检查
        if (_isQueuePaused) return;
        
        // 队列为空 → 结束处理
        if (_farmActionQueue.Count == 0)
        {
            _isProcessingQueue = false;
            _isExecutingFarming = false;
            _queuedPositions.Clear();
            // 🔴 补丁004 模块G（CP-G1）：移除 UnlockPosition（ghost 永不锁定）
            _farmNavState = FarmNavState.Preview;
            return;
        }
        
        _isProcessingQueue = true;
        var request = _farmActionQueue.Dequeue();
        _currentProcessingRequest = request;
        
        // ===== 二次验证 =====
        switch (request.type)
        {
            // 🔴 补丁005：PlantSeed 分支已移除（种子不再走 FIFO）
            case FarmActionType.Harvest:
                // CP-7：作物可收获二次验证
                if (request.targetCrop == null || !request.targetCrop.CanInteract(null))
                {
                    _queuedPositions.Remove((request.layerIndex, request.cellPos));
                    ProcessNextAction(); // 跳过
                    return;
                }
                break;
        }
        
        // ===== 距离判断 =====
        // 🔴🔴🔴 玩家位置 = Collider 中心（最高优先级规则）
        Vector2 playerCenter = GetPlayerCenter();
        float distance = Vector2.Distance(playerCenter, request.worldPos);
        
        // 🔴 补丁004V2 模块I（CP-I1~I4）：_isExecutingFarming 和 PromoteToExecutingPreview 移到 ExecuteFarmAction
        // 导航途中只是前置行为，执行 = 动画开始的瞬间
        
        if (distance <= farmToolReach)
        {
            // 近距离：直接执行
            _farmNavState = FarmNavState.Executing;
            ExecuteFarmAction(request);
        }
        else
        {
            // 远距离：导航到目标后执行
            _farmNavState = FarmNavState.Locked;
            StartFarmingNavigation(request.worldPos, () =>
            {
                // 导航到达回调
                if (_isQueuePaused) return; // V5：面板打开期间到达，不执行
                _farmNavState = FarmNavState.Executing;
                ExecuteFarmAction(request);
            });
        }
    }

    private void AbortCurrentQueuedFarmAction(string reason)
    {
        if (showDebugInfo)
        {
            Debug.Log($"[FarmQueue] 跳过当前动作: {reason}");
        }

        _isExecutingFarming = false;
        _currentHarvestTarget = null;
        _pendingTileUpdate = null;
        _tileUpdateTriggered = false;
        _queuedPositions.Remove((_currentProcessingRequest.layerIndex, _currentProcessingRequest.cellPos));
        FarmToolPreview.Instance?.RemoveExecutingPreview(_currentProcessingRequest.layerIndex, _currentProcessingRequest.cellPos);
        ProcessNextAction();
    }
    
    /// <summary>
    /// 面向目标位置（根据 worldPos 相对于玩家 Collider 中心设置朝向）。
    /// 🔴 玩家位置 = playerCollider.bounds.center（最高优先级规则）
    /// </summary>
    private void FaceTarget(Vector3 worldPos)
    {
        if (playerMovement == null) return;
        // 🔴🔴🔴 使用 Collider 中心计算方向
        Vector2 playerCenter = GetPlayerCenter();
        Vector2 direction = (Vector2)worldPos - playerCenter;
        if (direction.sqrMagnitude > 0.001f)
            playerMovement.SetFacingDirection(direction);
    }
    
    
    /// <summary>
    /// 执行农田操作（面向目标后按类型分发）。
    /// Till/Water/Harvest 有动画，等待 OnActionComplete 回调。
    /// PlantSeed 无动画，执行后立即 ProcessNextAction。
    /// </summary>
    private void ExecuteFarmAction(FarmActionRequest request)
    {
        if (showDebugInfo)
            Debug.Log($"[FarmQueue] ExecuteFarmAction: type={request.type}, cellPos={request.cellPos}");
        
        // 🔴 补丁004V2 模块I（CP-I3）：执行 = 动画开始的瞬间，在此设置执行状态
        _isExecutingFarming = true;
        FarmToolPreview.Instance?.PromoteToExecutingPreview(request.layerIndex, request.cellPos);
        
        switch (request.type)
        {
            case FarmActionType.Till:
                FaceTarget(request.worldPos);
                if (!TryStartPlayerAction(PlayerAnimController.AnimState.Crush))
                {
                    AbortCurrentQueuedFarmAction("锄地动画未开始");
                    break;
                }
                // 🔴 补丁003 模块C：不再同帧执行，改为延迟到动画中期
                _pendingTileUpdate = request;
                _tileUpdateTriggered = false;
                // 动画完成后由 OnActionComplete → OnFarmActionAnimationComplete 回调
                break;
            
            case FarmActionType.Water:
                FaceTarget(request.worldPos);
                if (!TryStartPlayerAction(PlayerAnimController.AnimState.Watering))
                {
                    AbortCurrentQueuedFarmAction("浇水动画未开始");
                    break;
                }
                // 🔴 补丁003 模块C：不再同帧执行，改为延迟到动画中期
                _pendingTileUpdate = request;
                _tileUpdateTriggered = false;
                // 动画完成后由 OnActionComplete → OnFarmActionAnimationComplete 回调
                break;
            
            case FarmActionType.PlantSeed:
                // 🔴 补丁005：种子不再走 FIFO，此分支理论上不会被触发
                // 保留空分支防止 switch 遗漏
                Debug.LogWarning("[FarmQueue] PlantSeed 不应通过 FIFO 执行，种子已迁移到放置系统");
                _isExecutingFarming = false;
                _queuedPositions.Remove((request.layerIndex, request.cellPos));
                ProcessNextAction();
                break;
            
            case FarmActionType.RemoveCrop:
                // 🔴 V6 模块S（CP-S6）：锄头清除农作物，使用 Crush 动画（和耕地一致）
                FaceTarget(request.worldPos);
                if (!TryStartPlayerAction(PlayerAnimController.AnimState.Crush))
                {
                    AbortCurrentQueuedFarmAction("清除农作物动画未开始");
                    break;
                }
                _pendingTileUpdate = request;
                _tileUpdateTriggered = false;
                break;
            
            case FarmActionType.Harvest:
                _currentHarvestTarget = request.targetCrop;
                FaceTarget(request.worldPos);
                playerInteraction?.RequestAction(PlayerAnimController.AnimState.Collect);
                // 动画完成后由 OnActionComplete → OnCollectAnimationComplete 回调
                break;
        }
    }
    
    /// <summary>
    /// 农田工具动画完成回调（Crush/Watering）。
    /// 由 PlayerInteraction.OnActionComplete 中的农田工具分支调用。
    /// </summary>
    public void OnFarmActionAnimationComplete()
    {
        // 🔴 补丁003 模块C：兜底 — 若 tile 更新还没触发（异常情况），强制执行
        if (_pendingTileUpdate != null && !_tileUpdateTriggered)
        {
            var req = _pendingTileUpdate.Value;
            switch (req.type)
            {
                case FarmActionType.Till:
                    ExecuteTillSoil(req.layerIndex, req.cellPos);
                    break;
                case FarmActionType.Water:
                    ExecuteWaterTile(req.layerIndex, req.cellPos, req.puddleVariant);
                    break;
                case FarmActionType.RemoveCrop:
                    // 🔴 004-P3：兜底 — 如果动画中期没触发隐藏，这里直接隐藏
                    ExecuteRemoveCrop(req.layerIndex, req.cellPos);
                    break;
            }
        }
        
        // 🔴 004-P3：动画完成后，RemoveCrop 类型执行真正的数据销毁
        if (_pendingTileUpdate != null && _pendingTileUpdate.Value.type == FarmActionType.RemoveCrop)
        {
            var req = _pendingTileUpdate.Value;
            var farmTileManager = FarmGame.Farm.FarmTileManager.Instance;
            if (farmTileManager != null)
            {
                var tileData = farmTileManager.GetTileData(req.layerIndex, req.cellPos);
                if (tileData?.cropController != null)
                {
                    tileData.cropController.RemoveCrop();
                    if (showDebugInfo)
                        Debug.Log($"[GameInputManager] 动画完成，销毁农作物: Layer={req.layerIndex}, Pos={req.cellPos}");
                }
            }
        }
        
        _pendingTileUpdate = null;
        _tileUpdateTriggered = false;
        
        _isExecutingFarming = false;
        _queuedPositions.Remove((_currentProcessingRequest.layerIndex, _currentProcessingRequest.cellPos));
        
        // 🔴 补丁004 模块G（CP-G3）：执行完成时移除执行预览（替代 RemoveQueuePreview）
        FarmToolPreview.Instance?.RemoveExecutingPreview(_currentProcessingRequest.layerIndex, _currentProcessingRequest.cellPos);
        
        ProcessNextAction();
    }
    
    /// <summary>
    /// 收获动画完成回调（Collect）。
    /// 由 PlayerInteraction.OnActionComplete 中的 Collect 专用分支调用。
    /// 执行收获逻辑（走 IInteractable 接口），然后取队列下一个。
    /// </summary>
    public void OnCollectAnimationComplete()
    {
        // 执行收获逻辑（走 IInteractable 接口）
        if (_currentHarvestTarget != null && _currentHarvestTarget.CanInteract(null))
        {
            var context = BuildInteractionContext();
            _currentHarvestTarget.OnInteract(context);
        }
        
        _currentHarvestTarget = null;
        _isExecutingFarming = false;
        _queuedPositions.Remove((_currentProcessingRequest.layerIndex, _currentProcessingRequest.cellPos));
        
        // 🔴 补丁004 模块G（CP-G3）：执行完成时移除执行预览（替代 RemoveQueuePreview）
        FarmToolPreview.Instance?.RemoveExecutingPreview(_currentProcessingRequest.layerIndex, _currentProcessingRequest.cellPos);
        
        ProcessNextAction();
    }
    
    /// <summary>
    /// 清空操作队列。
    /// 用于 WASD 中断、ESC、切换快捷栏等场景。
    /// </summary>
    public void ClearActionQueue()
    {
        _farmActionQueue.Clear();
        _queuedPositions.Clear();
        _isProcessingQueue = false;

        // 🔴 补丁004V2 模块H（CP-H1~H4）：动画执行中保留执行状态
        if (!_isExecutingFarming)
        {
            // 没有动画在执行，全部清空
            _isExecutingFarming = false;
            _currentHarvestTarget = null;
            _currentProcessingRequest = default;
            _pendingTileUpdate = null;
            _tileUpdateTriggered = false;
        }
        // else: 动画执行中，保留 _pendingTileUpdate/_currentProcessingRequest/_isExecutingFarming
        // 动画完成后由 OnFarmActionAnimationComplete/OnCollectAnimationComplete 正常清理

        // 🔴 补丁003 模块D/I（CP-I3）：清理所有队列预览
        FarmToolPreview.Instance?.ClearAllQueuePreviews(!_isExecutingFarming);
        ClearPausedFarmingNavigation();
    }
    

    /// <summary>
    /// 收获检测方法：检测鼠标位置是否有可收获的作物（同层级），有则入队 Harvest。
    /// CP-5：收获最高优先级，无论手持什么物品都执行检测。
    /// CP-6：只检测玩家当前层级的作物。
    /// </summary>
    private bool ShouldSkipHarvestPriorityForPlacementTool()
    {
        if (!IsPlacementMode || inventory == null || database == null || hotbarSelection == null)
            return false;

        int idx = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
        var slot = inventory.GetSlot(idx);
        if (slot.IsEmpty) return false;

        var itemData = database.GetItemByID(slot.itemId);
        return itemData is ToolData tool &&
               (tool.toolType == ToolType.Hoe || tool.toolType == ToolType.WateringCan);
    }

    private bool TryDetectAndEnqueueHarvest()
    {
        if (ShouldSkipHarvestPriorityForPlacementTool())
            return false;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        var hits = Physics2D.OverlapPointAll(mouseWorldPos);
        if (hits == null || hits.Length == 0) return false;

        // 🔴 获取玩家当前层级索引
        var farmTileManager = FarmTileManager.Instance;
        if (farmTileManager == null) return false;
        // 🔴🔴🔴 玩家位置 = Collider 中心（最高优先级规则）
        Vector2 playerCenter = GetPlayerCenter();
        int playerLayer = farmTileManager.GetCurrentLayerIndex(playerCenter);

        foreach (var hit in hits)
        {
            // 跳过玩家自身碰撞体
            if (playerMovement != null &&
                (hit.transform == playerMovement.transform || hit.transform.IsChildOf(playerMovement.transform)))
                continue;

            // 走 IInteractable 接口检测
            var interactable = hit.GetComponent<IInteractable>();
            if (interactable == null)
                interactable = hit.GetComponentInParent<IInteractable>();

            if (interactable is CropController crop)
            {
                // CP-6：层级过滤 — 只检测玩家当前层级的作物
                if (crop.LayerIndex != playerLayer) continue;

                // 可收获检测（Mature 或 WitheredMature）
                if (!crop.CanInteract(null)) continue;

                // 构建请求并入队
                EnqueueAction(new FarmActionRequest
                {
                    type = FarmActionType.Harvest,
                    cellPos = crop.CellPos,
                    layerIndex = crop.LayerIndex,
                    worldPos = crop.GetCellCenterPosition(),
                    targetCrop = crop
                });
                return true;
            }
        }

        return false;
    }

    private bool TryEnqueueHoeRemoveCropFromMouse()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        var hits = Physics2D.OverlapPointAll(mouseWorldPos);
        if (hits == null || hits.Length == 0) return false;

        var farmTileManager = FarmTileManager.Instance;
        if (farmTileManager == null) return false;

        Vector2 playerCenter = GetPlayerCenter();
        int playerLayer = farmTileManager.GetCurrentLayerIndex(playerCenter);

        foreach (var hit in hits)
        {
            if (playerMovement != null &&
                (hit.transform == playerMovement.transform || hit.transform.IsChildOf(playerMovement.transform)))
                continue;

            var crop = hit.GetComponent<CropController>();
            if (crop == null)
                crop = hit.GetComponentInParent<CropController>();

            if (crop == null) continue;
            if (crop.LayerIndex != playerLayer) continue;

            EnqueueAction(new FarmActionRequest
            {
                type = FarmActionType.RemoveCrop,
                cellPos = crop.CellPos,
                layerIndex = crop.LayerIndex,
                worldPos = crop.GetCellCenterPosition(),
                targetCrop = crop
            });
            return true;
        }

        return false;
    }

    /// <summary>
    /// 农田工具入队方法：检查预览有效性后，将锄头/浇水操作入队。
    /// CP-9：预览无效时不入队。
    /// </summary>
    private void TryEnqueueFarmTool(ToolData tool)
    {
        var farmPreview = FarmGame.Farm.FarmToolPreview.Instance;
        if (farmPreview == null || !farmPreview.IsValid()) return; // CP-9：预览无效不入队

        var type = tool.toolType == ToolType.Hoe
            ? FarmActionType.Till
            : FarmActionType.Water;
        
        // 🔴 补丁004V3：耕地入队不再传递 ghost 数据（b 层独立计算完整预览）
        
        // 🔴 补丁004 模块G（CP-G5）：浇水入队时使用 ghost 缓存的 variant（替代当前的 Random.Range）
        int variant = -1;
        if (type == FarmActionType.Water)
        {
            variant = farmPreview.CurrentPuddleVariant;
            // 兜底：如果 ghost 没有缓存（异常情况），随机分配
            if (variant < 0)
            {
                var puddleTiles = FarmVisualManager.Instance?.GetPuddleTiles();
                int puddleCount = puddleTiles != null ? puddleTiles.Length : 3;
                variant = Random.Range(0, puddleCount);
            }
        }
        
        EnqueueAction(new FarmActionRequest
        {
            type = type,
            cellPos = farmPreview.CurrentCellPos,
            layerIndex = farmPreview.CurrentLayerIndex,
            worldPos = farmPreview.CurrentCursorPos,
            targetCrop = null,
            puddleVariant = variant
        });
        
        // 🔴 补丁005续3修复：浇水入队成功后设置标志，等待鼠标移出才随机
        if (type == FarmActionType.Water)
        {
            farmPreview.MarkWaterQueued(farmPreview.CurrentCellPos);
        }
    }

    private bool HasActiveAutomatedFarmToolOperation()
    {
        if (!IsPlacementMode || !TryGetCurrentHotbarItem(out _, out var itemData))
        {
            return false;
        }
        if (itemData is not ToolData tool)
        {
            return false;
        }

        bool isFarmTool = tool.toolType == ToolType.Hoe || tool.toolType == ToolType.WateringCan;
        if (!isFarmTool)
        {
            return false;
        }

        return _farmActionQueue.Count > 0 ||
               _isProcessingQueue ||
               _isExecutingFarming ||
               _farmNavState == FarmNavState.Locked ||
               _farmNavState == FarmNavState.Navigating ||
               _farmNavState == FarmNavState.Executing;
    }

    private bool TryGetCurrentHotbarItem(out int slotIndex, out ItemData itemData)
    {
        slotIndex = -1;
        itemData = null;

        if (inventory == null || database == null || hotbarSelection == null)
        {
            return false;
        }

        slotIndex = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
        var slot = inventory.GetSlot(slotIndex);
        if (slot.IsEmpty)
        {
            return false;
        }

        itemData = database.GetItemByID(slot.itemId);
        return itemData != null;
    }

    private bool HasActivePlacementHeldSession(ItemData itemData)
    {
        return itemData != null &&
               itemData.isPlaceable &&
               PlacementManager.Instance != null &&
               PlacementManager.Instance.HasProtectedHeldSession;
    }

    private bool HasActivePlacementNavigationSession()
    {
        if (PlacementManager.Instance == null)
        {
            return false;
        }

        return PlacementManager.Instance.HasProtectedHeldSession;
    }

    private bool HasActiveToolHeldSession(ItemData itemData)
    {
        if (itemData is not ToolData)
        {
            return false;
        }

        if (HasActiveAutomatedFarmToolOperation())
        {
            return true;
        }

        bool isPerformingAction = playerInteraction != null && playerInteraction.IsPerformingAction();
        bool isLocked = ToolActionLockManager.Instance != null && ToolActionLockManager.Instance.IsLocked;
        return isPerformingAction || isLocked;
    }

    private bool TryGetRuntimeProtectedHeldSlotIndex(out int slotIndex)
    {
        slotIndex = -1;
        if (!TryGetCurrentHotbarItem(out slotIndex, out var itemData))
        {
            return false;
        }

        return HasActivePlacementHeldSession(itemData) || HasActiveToolHeldSession(itemData);
    }

    private bool TryGetProtectedHeldSlotIndex(out int slotIndex)
    {
        if (TryGetRuntimeProtectedHeldSlotIndex(out slotIndex))
        {
            return true;
        }

        if (IsAnyPanelOpen() && _hasUIFrozenProtectedHeldSlot && _uiFrozenProtectedHeldSlotIndex >= 0)
        {
            slotIndex = _uiFrozenProtectedHeldSlotIndex;
            return true;
        }

        slotIndex = -1;
        return false;
    }

    private void InterruptAutomatedFarmToolOperation()
    {
        HideFarmToolPreview();
        ClearActionQueue();

        var lockManager = ToolActionLockManager.Instance;
        lockManager?.ClearHotbarCache();

        bool isExecuting = _isExecutingFarming || (playerInteraction != null && playerInteraction.IsPerformingAction());
        if (!isExecuting)
        {
            CancelFarmingNavigation();
            lockManager?.ForceUnlock();
        }
    }

    private void PlayAutomatedFarmToolRejectSound()
    {
        PlacementManager.Instance?.PlayFailFeedbackSound();
    }

    private void PlayProtectedHeldRejectFeedback(int sourceSlotIndex)
    {
        PlayAutomatedFarmToolRejectSound();
        ToolbarSlotUI.PlayRejectShakeAt(sourceSlotIndex);
        InventorySlotUI.PlayRejectShakeAt(sourceSlotIndex);
    }

    private bool TryStartPlayerAction(PlayerAnimController.AnimState action)
    {
        if (playerInteraction == null)
        {
            return true;
        }

        playerInteraction.RequestAction(action);
        return playerInteraction.IsPerformingAction();
    }

    private ToolData GetCurrentHeldToolData()
    {
        if (!TryGetCurrentHotbarItem(out _, out var itemData))
        {
            return null;
        }

        return itemData as ToolData;
    }

    private void CommitCurrentToolUse(ToolData toolData, string context)
    {
        if (toolData == null)
        {
            return;
        }

        if (playerInteraction != null &&
            playerInteraction.TryCommitCurrentToolActionSuccess(toolData, context))
        {
            return;
        }

        ToolRuntimeUtility.TryConsumeHeldToolUse(inventory, hotbarSelection, database, toolData, context);
    }

    public bool IsActiveHeldSlotProtected(int slotIndex, bool isEquip = false)
    {
        if (isEquip)
        {
            return false;
        }

        return TryGetProtectedHeldSlotIndex(out int protectedSlotIndex) && protectedSlotIndex == slotIndex;
    }

    public bool TryRejectActiveFarmToolSwitch(int requestedIndex)
    {
        if (!TryGetCurrentHotbarItem(out int protectedSlotIndex, out var itemData) || requestedIndex == protectedSlotIndex)
        {
            return false;
        }

        if (HasActiveAutomatedFarmToolOperation())
        {
            PlayProtectedHeldRejectFeedback(protectedSlotIndex);
            InterruptAutomatedFarmToolOperation();
            return true;
        }

        if (itemData != null && itemData.isPlaceable && HasActivePlacementNavigationSession())
        {
            PlayProtectedHeldRejectFeedback(protectedSlotIndex);
            PlacementManager.Instance?.InterruptFromExternal();
            return true;
        }

        return false;
    }

    public bool TryPrepareHotbarSelectionChange(int requestedIndex)
    {
        int clampedIndex = Mathf.Clamp(requestedIndex, 0, InventoryService.HotbarWidth - 1);
        if (hotbarSelection != null && clampedIndex == hotbarSelection.selectedIndex)
        {
            return true;
        }

        if (TryRejectActiveFarmToolSwitch(clampedIndex))
        {
            return false;
        }

        ClearActionQueue();
        CancelFarmingNavigation();
        return true;
    }

    public bool TryRejectActiveFarmToolInventoryMove(int slotIndex, bool isEquip)
    {
        if (isEquip || !TryGetProtectedHeldSlotIndex(out int protectedSlotIndex) || slotIndex != protectedSlotIndex)
        {
            return false;
        }

        PlayProtectedHeldRejectFeedback(protectedSlotIndex);
        return true;
    }

    public bool TryRejectProtectedHeldInventoryMutation(int sourceSlotIndex, bool sourceIsInventorySlot, int targetSlotIndex, bool targetIsInventorySlot)
    {
        if (!TryGetProtectedHeldSlotIndex(out int protectedSlotIndex))
        {
            return false;
        }

        bool touchesProtectedSource = sourceIsInventorySlot && sourceSlotIndex == protectedSlotIndex;
        bool touchesProtectedTarget = targetIsInventorySlot && targetSlotIndex == protectedSlotIndex;
        if (!touchesProtectedSource && !touchesProtectedTarget)
        {
            return false;
        }

        PlayProtectedHeldRejectFeedback(protectedSlotIndex);
        return true;
    }

    public bool TryRejectProtectedHeldInventoryReshuffle()
    {
        if (!TryGetProtectedHeldSlotIndex(out int protectedSlotIndex))
        {
            return false;
        }

        PlayProtectedHeldRejectFeedback(protectedSlotIndex);
        return true;
    }


    /// <summary>
    /// 队列是否为空（供 OnActionComplete 长按分支查询）— 补丁003 模块E CP-E2
    /// </summary>
    public bool IsQueueEmpty() => _farmActionQueue.Count == 0;

    /// <summary>
    /// 保护分支统一入队方法：在 _isExecutingFarming 或 isPerformingAction 期间调用。
    /// 替代旧的 CacheFarmInput，先尝试收获（CP-5 最高优先级），再尝试工具/种子。
    /// </summary>
    public void TryEnqueueFromCurrentInput()
    {
        if (IsAnyPanelOpen())
        {
            return;
        }

        if (GetManualMovementInput().sqrMagnitude > 0.01f)
        {
            return;
        }

        // 再尝试工具/种子
        if (inventory == null || database == null || hotbarSelection == null) return;
        int idx = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
        var slot = inventory.GetSlot(idx);
        if (slot.IsEmpty) return;
        var itemData = database.GetItemByID(slot.itemId);

        // CP-5：先尝试收获（最高优先级）
        if (TryDetectAndEnqueueHarvest()) return;

        if (itemData is ToolData tool)
        {
            if (tool.toolType == ToolType.Hoe)
            {
                if (IsPlacementMode)
                {
                    TryEnqueueFarmTool(tool);
                }
                else
                {
                    TryEnqueueHoeRemoveCropFromMouse();
                }
                return;
            }

            if (tool.toolType == ToolType.WateringCan)
            {
                if (IsPlacementMode)
                {
                    TryEnqueueFarmTool(tool);
                }
            }
        }
        // 🔴 补丁005：种子不再走 FIFO 入队（已迁移到放置系统）
    }
    
    #endregion
    
    #endregion
}
