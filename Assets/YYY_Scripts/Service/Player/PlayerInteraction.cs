using UnityEngine;
using FarmGame.Data;
using FarmGame.Data.Core;

/// <summary>
/// 玩家交互控制器
/// 
/// 核心逻辑：
/// 1. 动画播放期间：锁定toolbar和移动，缓存方向和hotbar输入
/// 2. 动画完成时：
///    - 先应用缓存的方向（更新朝向数据）
///    - 如果继续长按：用新方向播放下一个动画
///    - 如果松开：解锁，应用hotbar缓存，清空所有缓存
/// 3. 松开后：当前动画必须播放完毕
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    #region 组件引用
    private PlayerAnimController animController;
    private PlayerMovement playerMovement;
    private PlayerToolController toolController;
    private EnergySystem energySystem;
    private ToolActionLockManager lockManager;
    private LayerAnimSync layerAnimSync;
    #endregion

    #region 动画状态
    private bool isPerformingAction = false;
    private bool isCarrying = false;
    private PlayerAnimController.AnimState currentAction;
    private float actionStartTime;
    public bool enableLegacyInput = false;
    private bool toolUseCommittedForCurrentAction = false;
    
    // 当前操作的工具数据（用于精力消耗判定）
    private ToolData pendingToolData;
    
    [Header("调试")]
    [SerializeField] private bool enableDebugLog = false;
    #endregion
    
    void Awake()
    {
        animController = GetComponent<PlayerAnimController>();
        playerMovement = GetComponent<PlayerMovement>();
        toolController = GetComponent<PlayerToolController>();
        energySystem = EnergySystem.Instance;
        layerAnimSync = GetComponentInChildren<LayerAnimSync>();
    }

    void Start()
    {
        if (energySystem == null)
            energySystem = EnergySystem.Instance;
        lockManager = ToolActionLockManager.Instance;
    }

    void Update()
    {
        if (isPerformingAction)
        {
            // 简单判断：动画是否完成
            if (animController.IsAnimationFinished())
            {
                OnActionComplete();
            }
            return;
        }

        if (enableLegacyInput)
            CheckInputs();
    }

    private void CheckInputs()
    {
        if (Input.GetKey(KeyCode.G)) { PerformAction(PlayerAnimController.AnimState.Death); return; }
        if (Input.GetKey(KeyCode.E)) { PerformAction(PlayerAnimController.AnimState.Collect); return; }
        if (Input.GetMouseButton(1)) { PerformAction(PlayerAnimController.AnimState.Hit); return; }
        if (Input.GetKey(KeyCode.Alpha1)) { PerformAction(PlayerAnimController.AnimState.Crush); return; }
        if (Input.GetKey(KeyCode.Alpha2)) { PerformAction(PlayerAnimController.AnimState.Slice); return; }
        if (Input.GetKey(KeyCode.Alpha3)) { PerformAction(PlayerAnimController.AnimState.Watering); return; }
        if (Input.GetKey(KeyCode.Alpha4)) { PerformAction(PlayerAnimController.AnimState.Fish); return; }
    }


    /// <summary>
    /// 执行动作（首次触发）
    /// </summary>
    private void PerformAction(PlayerAnimController.AnimState action)
    {
        if (isPerformingAction) return;

        pendingToolData = toolController?.CurrentToolData;

        if (pendingToolData != null && energySystem != null)
        {
            if (!energySystem.HasEnoughEnergy(pendingToolData.energyCost))
            {
                if (enableDebugLog)
                    Debug.Log($"<color=yellow>[PlayerInteraction] 精力不足</color>");
                return;
            }
        }

        if (lockManager == null)
            lockManager = ToolActionLockManager.Instance;

        // 首次动作：不应用缓存，使用当前朝向
        StartAction(action);
    }

    /// <summary>
    /// 开始动作（内部方法）
    /// </summary>
    /// <param name="action">动作类型</param>
    /// <param name="isContinuation">是否是连续动作（长按继续）</param>
    private void StartAction(PlayerAnimController.AnimState action, bool isContinuation = false)
    {
        isPerformingAction = true;
        currentAction = action;
        actionStartTime = Time.time;
        toolUseCommittedForCurrentAction = false;

        lockManager?.BeginAction();
        
        // ★ 允许工具显示（解除强制隐藏）
        layerAnimSync?.AllowToolShow();

        if (playerMovement != null)
            playerMovement.StopMovement();

        // 获取当前朝向
        PlayerAnimController.AnimDirection direction = playerMovement != null 
            ? playerMovement.GetCurrentFacingDirection() 
            : PlayerAnimController.AnimDirection.Down;
        
        bool shouldFlip = (direction == PlayerAnimController.AnimDirection.Left);

        if (enableDebugLog)
            Debug.Log($"<color=cyan>[PlayerInteraction] 开始动作: {action}, 方向: {direction}, 连续: {isContinuation}</color>");

        if (isContinuation)
        {
            // 连续动作：只有方向变化才切换动画
            animController?.ForcePlayAnimation(action, direction, shouldFlip);
        }
        else
        {
            // 首次动作：正常播放
            animController?.PlayAnimation(action, direction, shouldFlip);
        }
        
        // 立即开始计时
        animController?.StartAnimationTracking();
    }

    public void RequestAction(PlayerAnimController.AnimState action)
    {
        PerformAction(action);
    }

    public void OnToolActionSuccess()
    {
        CommitCurrentToolUse(pendingToolData, $"PlayerInteraction/{currentAction}");
    }

    public void OnToolActionSuccess(ToolData tool)
    {
        CommitCurrentToolUse(tool, $"PlayerInteraction/{currentAction}");
    }

    /// <summary>
    /// 动作完成时的处理
    /// 
    /// 关键逻辑：
    /// 1. 先应用缓存的方向（更新朝向数据，不播放动画）
    /// 2. 检查是否继续长按
    /// 3. 如果继续：用新朝向开始下一个动画
    /// 4. 如果不继续：解锁，应用hotbar缓存，清空所有缓存
    /// </summary>
    private void OnActionComplete()
    {
        if (lockManager == null)
            lockManager = ToolActionLockManager.Instance;

        // ===== 🔴 Collect 专用分支（10.1.1补丁002：V3 漏洞修补）=====
        // 必须在 shouldContinue 判断之前拦截，因为 IsToolAction(Collect) = false
        if (currentAction == PlayerAnimController.AnimState.Collect)
        {
            animController?.StopAnimationTracking();
            lockManager?.EndAction(false);
            lockManager?.ClearAllCache();
            isPerformingAction = false;
            GameInputManager.Instance?.OnCollectAnimationComplete();
            return; // 不进入后续任何分支
        }

        if (currentAction == PlayerAnimController.AnimState.Death)
            isCarrying = false;

        // ★ 关键：先应用缓存的方向（只更新朝向数据，不播放动画）
        ApplyCachedDirectionToFacing();

        // 检查是否继续长按
        bool isCurrentlyHolding = Input.GetMouseButton(0);
        bool shouldContinue = isCurrentlyHolding && IsToolAction(currentAction);
        
        var actionToRepeat = currentAction;
        
        if (shouldContinue)
        {
            if (enableDebugLog)
                Debug.Log($"<color=yellow>[PlayerInteraction] 长按继续</color>");
            
            // 🔥 10.1.1 方案B：区分农田工具和通用工具的长按行为
            var gimContinue = GameInputManager.Instance;
            bool isFarmTool = gimContinue != null && gimContinue.IsHoldingFarmTool();
            
            if (isFarmTool)
            {
                // 🔴 补丁003 模块E（CP-E2/CP-E3）：农田工具长按分支改造
                // 统一清理顺序：StopTracking → isPerformingAction=false → EndAction → ClearAllCache
                animController?.StopAnimationTracking();
                isPerformingAction = false;
                lockManager?.EndAction(false);
                lockManager?.ClearAllCache();
                
                // 🔴 P2：长按分支 — 队列为空时重新入队当前鼠标位置
                if (gimContinue.IsQueueEmpty())
                {
                    gimContinue.TryEnqueueFromCurrentInput();
                }
                gimContinue.OnFarmActionAnimationComplete();
            }
            else
            {
                // 🔥 通用工具（镐子/斧头等）：保持原有行为，先播动画再继续
                animController?.StopAnimationTracking();
                lockManager?.EndAction(true);
                StartAction(actionToRepeat, true);
            }
        }
        else
        {
            if (enableDebugLog)
                Debug.Log($"<color=green>[PlayerInteraction] 动作结束</color>");
            
            // 🔴 补丁003 模块E（CP-E1/CP-E3）：松开分支时序修复
            // 统一清理顺序：ForceHideTool → StopTracking → isPerformingAction=false → EndAction → 回调 → ApplyCachedHotbarSwitch → ClearAllCache
            layerAnimSync?.ForceHideTool();
            animController?.StopAnimationTracking();
            isPerformingAction = false;  // 🔴 P3修复：必须在 OnFarmActionAnimationComplete 之前，否则 ProcessNextAction 被守卫拦截
            lockManager?.EndAction(false);
            
            var gimRelease = GameInputManager.Instance;
            if (gimRelease != null)
            {
                if (lockManager != null && lockManager.HasCachedHotbarInput)
                {
                    // 动画期间切换了工具栏 → 清空整个队列（CP-3）
                    gimRelease.ClearActionQueue();
                }
                else
                {
                    // 松开鼠标 → 通知队列取下一个（CP-18）
                    gimRelease.OnFarmActionAnimationComplete();
                }
            }
            
            ApplyCachedHotbarSwitch();
            lockManager?.ClearAllCache();
        }
    }
    
    /// <summary>
    /// 应用缓存的方向到朝向数据（不播放动画）
    /// </summary>
    private void ApplyCachedDirectionToFacing()
    {
        if (lockManager == null) return;
        
        Vector2? cachedDir = lockManager.ConsumeDirectionCache();
        if (cachedDir.HasValue && playerMovement != null)
        {
            if (enableDebugLog)
                Debug.Log($"<color=lime>[PlayerInteraction] 应用缓存方向: {cachedDir.Value}</color>");
            
            playerMovement.SetFacingDirection(cachedDir.Value);
        }
    }
    
    private void ApplyCachedHotbarSwitch()
    {
        if (lockManager == null) return;
        
        int cachedIndex = lockManager.ConsumeHotbarCache();
        if (cachedIndex >= 0)
        {
            if (enableDebugLog)
                Debug.Log($"<color=lime>[PlayerInteraction] 应用缓存 hotbar: {cachedIndex}</color>");
            
            var hotbarSelection = FindFirstObjectByType<HotbarSelectionService>();
            hotbarSelection?.SelectIndex(cachedIndex);
        }
    }
    
    private bool IsToolAction(PlayerAnimController.AnimState action)
    {
        return action == PlayerAnimController.AnimState.Slice ||
               action == PlayerAnimController.AnimState.Crush ||
               action == PlayerAnimController.AnimState.Pierce ||
               action == PlayerAnimController.AnimState.Watering;
    }

    private void CommitCurrentToolUse(ToolData tool, string context)
    {
        if (toolUseCommittedForCurrentAction || tool == null)
        {
            return;
        }

        if (ToolRuntimeUtility.TryConsumeHeldToolUse(null, null, null, tool, context))
        {
            toolUseCommittedForCurrentAction = true;
        }
    }

    public bool IsCarrying() => isCarrying;
    public bool IsPerformingAction() => isPerformingAction;
    public PlayerAnimController.AnimState GetCurrentAction() => currentAction;
    
    /// <summary>
    /// 获取当前动画进度 (0-1)。转发到 PlayerAnimController。
    /// 补丁003 模块C：供 GameInputManager 延迟执行机制查询动画进度。
    /// </summary>
    public float GetAnimationProgress()
    {
        return animController != null ? animController.GetAnimationProgress() : 0f;
    }
}
