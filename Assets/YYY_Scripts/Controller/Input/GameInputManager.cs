using UnityEngine;
using UnityEngine.EventSystems;
using FarmGame.Data;

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
    [Header("调试开关")]
    [SerializeField, HideInInspector] private TimeManagerDebugger timeDebugger;
    [SerializeField] private bool enableTimeDebugKeys = false;
    [Header("UI自动激活")]
    [SerializeField] private bool autoActivateUIRoot = true;
    [SerializeField] private string uiRootName = "UI";

    private GameObject uiRootCache;
    private bool packageTabsInitialized = false;

    private static GameInputManager s_instance;
    private float lastNavClickTime = -1f;
    private Vector3 lastNavClickPos = Vector3.zero;

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
        HandlePanelHotkeys();
        HandleRunToggleWhileNav();
        HandleMovement();
        HandleHotbarSelection();
        HandleUseCurrentTool();
        HandleRightClickAutoNav();
        if (timeDebugger != null) timeDebugger.enableDebugKeys = enableTimeDebugKeys;
    }

    void HandleRunToggleWhileNav()
    {
        // ✅ Shift 逻辑已由 SprintStateManager 统一管理，这里不需要处理
        // 导航会自动从 SprintStateManager 获取疾跑状态
    }

    void HandleMovement()
    {
        Vector2 input;
        if (useAxisForMovement)
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        else
        {
            float x = (Input.GetKey(KeyCode.D) ? 1f : 0f) + (Input.GetKey(KeyCode.A) ? -1f : 0f);
            float y = (Input.GetKey(KeyCode.W) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f);
            input = new Vector2(x, y);
        }
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

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
                autoNavigator.Cancel();
                if (playerMovement != null) playerMovement.SetMovementInput(input, shift);
            }
            return;
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
        bool uiOpen = packageTabs != null && packageTabs.IsPanelOpen();
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
                
                if (isLocked)
                {
                    // 锁定状态：累积滚轮步数
                    _accumulatedScrollSteps += scrollSteps;
                    
                    // 计算目标索引（基于当前选中 + 累积步数）
                    int currentIndex = hotbarSelection != null ? hotbarSelection.selectedIndex : 0;
                    int targetIndex = (currentIndex + _accumulatedScrollSteps) % InventoryService.HotbarWidth;
                    if (targetIndex < 0) targetIndex += InventoryService.HotbarWidth;
                    
                    // 缓存最终目标索引
                    lockManager.CacheHotbarInput(targetIndex);
                }
                else
                {
                    // 正常切换：重置累积值
                    _accumulatedScrollSteps = 0;
                    
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

        // 数字键切换
        int keyIndex = -1;
        if (Input.GetKeyDown(KeyCode.Alpha1)) keyIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) keyIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) keyIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) keyIndex = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5)) keyIndex = 4;
        
        if (keyIndex >= 0)
        {
            if (isLocked)
            {
                // 锁定状态：缓存输入（数字键直接指定索引，重置累积值）
                _accumulatedScrollSteps = 0;
                lockManager.CacheHotbarInput(keyIndex);
            }
            else
            {
                // 正常切换
                hotbarSelection?.SelectIndex(keyIndex);
            }
        }
    }

    void HandlePanelHotkeys()
    {
        var tabs = EnsurePackageTabs();
        if (tabs == null) return;

        if (Input.GetKeyDown(KeyCode.Tab)) { tabs.OpenProps(); return; }
        if (Input.GetKeyDown(KeyCode.B)) { tabs.OpenRecipes(); return; }
        if (Input.GetKeyDown(KeyCode.M)) { tabs.OpenMap(); return; }
        if (Input.GetKeyDown(KeyCode.L)) { tabs.OpenEx(); return; }
        if (Input.GetKeyDown(KeyCode.O)) { tabs.OpenRelations(); return; }
        if (Input.GetKeyDown(KeyCode.Escape)) { tabs.OpenSettings(); }
    }

    void HandleUseCurrentTool()
    {
        // 改为 GetMouseButton 支持长按连续使用
        // 但首次触发仍需要 GetMouseButtonDown，后续由 PlayerInteraction 处理连续
        bool isFirstPress = Input.GetMouseButtonDown(0);
        bool isHolding = Input.GetMouseButton(0);
        
        // 检查是否正在执行动作
        bool isPerformingAction = playerInteraction != null && playerInteraction.IsPerformingAction();
        
        // 首次按下时触发，或者动作完成后继续长按时由 PlayerInteraction 内部处理
        if (!isFirstPress)
        {
            // 非首次按下，如果正在执行动作则由 PlayerInteraction 处理连续
            return;
        }
        
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        if (inventory == null || database == null || hotbarSelection == null) return;
        
        // 如果正在执行动作，不重复触发
        if (isPerformingAction) return;

        int idx = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
        var slot = inventory.GetSlot(idx);
        if (slot.IsEmpty) return;

        var itemData = database.GetItemByID(slot.itemId);
        if (itemData == null) return;

        if (itemData is ToolData tool)
        {
            var action = ResolveAction(tool.toolType);
            playerInteraction?.RequestAction(action);
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
        // blockNavOverUI 只阻挡导航，不应该阻挡面板热键
        if (blockNavOverUI && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
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

        // 点击死区：靠近玩家的区域忽略导航（使用Collider中心）
        if (playerMovement != null)
        {
            var player = playerMovement.transform;
            var col = playerMovement.GetComponent<Collider2D>();
            Vector2 playerCenter = col != null ? (Vector2)col.bounds.center : (Vector2)player.position;
            if (Vector2.Distance(playerCenter, world) <= navClickDeadzone)
            {
                return;
            }
        }

        // 防止连续点击同一位置（鬼畜问题）
        if (autoNavigator.IsActive && Vector3.Distance(world, lastNavClickPos) < minNavDistance)
        {
            // 如果已在导航且点击位置过近，忽略
            return;
        }

        // 2D射线检测交互对象（优先按标签过滤，其次图层）
        var hits = Physics2D.OverlapPointAll(world);
        Transform found = null;
        foreach (var h in hits)
        {
            // 忽略自身碰撞
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

        // 更新点击记录
        lastNavClickTime = currentTime;
        lastNavClickPos = world;

        if (found != null) autoNavigator.FollowTarget(found, 0.6f);
        else autoNavigator.SetDestination(world);
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
}
