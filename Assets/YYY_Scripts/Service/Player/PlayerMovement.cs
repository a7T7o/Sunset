using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("组件引用")]
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private PlayerAnimController animController;
    private PlayerInteraction playerInteraction;
    private SprintStateManager sprintManager;
    private float defaultLinearDamping;

    [Header("移动速度")]
    public float RunSpeed = 4f;
    public float WalkSpeed = 2f;

    [Header("导航阻挡修正")]
    [SerializeField, Min(0f)] private float blockedNavigationDamping = 12f;
    [SerializeField, Range(0.05f, 0.6f)] private float blockedNavigationMaxSpeedFactor = 0.25f;
    [SerializeField, Min(0f)] private float blockedNavigationMinSpeed = 0.08f;
    [SerializeField, Min(0f)] private float blockedNavigationSpeedBuffer = 0.05f;
    [SerializeField, Min(0f)] private float blockedNavigationForwardClearanceSlack = 0.04f;
    
    private Vector2 movementInput;
    private bool isShiftHeld = false;
    private bool hasBlockedNavigationConstraint = false;
    private Vector2 blockedNavigationBlockerPosition = Vector2.zero;
    private float blockedNavigationClearance = float.PositiveInfinity;
    
    // 当前朝向（始终根据输入更新，不受动作影响）
    private PlayerAnimController.AnimDirection currentFacingDirection = PlayerAnimController.AnimDirection.Down;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            playerCollider = GetComponentInChildren<Collider2D>();
        }
        animController = GetComponent<PlayerAnimController>();
        playerInteraction = GetComponent<PlayerInteraction>();
        defaultLinearDamping = rb != null ? rb.linearDamping : 0f;
        
        // 获取疾跑状态管理器（如果不存在则创建）
        sprintManager = SprintStateManager.Instance;
        if (sprintManager == null)
        {
            GameObject managerObj = new GameObject("SprintStateManager");
            sprintManager = managerObj.AddComponent<SprintStateManager>();
        }
    }

    void Update()
    {
        // 如果正在执行动作，禁止移动和朝向更新
        if (playerInteraction != null && playerInteraction.IsPerformingAction())
        {
            ClearBlockedNavigationConstraint();
            ResetRuntimeMotionModifiers();
            rb.linearVelocity = Vector2.zero;
            // 动作期间不更新朝向，方向输入由 ToolActionLockManager 缓存
            return;
        }
        
        // 非动作状态：正常更新朝向
        UpdateFacingDirection();

        // 正常移动和动画
        UpdateMovement();
        UpdateAnimation();
    }
    
    private void UpdateMovement()
    {
        if (rb == null)
        {
            return;
        }

        float currentSpeed = isShiftHeld ? RunSpeed : WalkSpeed;
        Vector2 desiredVelocity = Vector2.ClampMagnitude(movementInput, 1f) * currentSpeed;

        if (hasBlockedNavigationConstraint)
        {
            ApplyBlockedNavigationVelocity(desiredVelocity, currentSpeed);
            return;
        }

        ResetRuntimeMotionModifiers();
        rb.linearVelocity = desiredVelocity;
    }

    private void UpdateAnimation()
    {
        if (animController == null) return;
        
        // 获取当前方向
        PlayerAnimController.AnimDirection direction = movementInput.magnitude < 0.01f 
            ? animController.GetCurrentDirection() 
            : GetDirection();
        
        bool shouldFlip = (direction == PlayerAnimController.AnimDirection.Left);
        
        // 检查是否在Carry状态
        bool isCarrying = playerInteraction != null && playerInteraction.IsCarrying();
        
        if (isCarrying)
        {
            // Carry状态的动画
            if (movementInput.magnitude < 0.01f)
            {
                animController.PlayCarry(PlayerAnimController.CarryState.Idle, direction, shouldFlip);
            }
            else if (isShiftHeld)
            {
                animController.PlayCarry(PlayerAnimController.CarryState.Run, direction, shouldFlip);
            }
            else
            {
                animController.PlayCarry(PlayerAnimController.CarryState.Walk, direction, shouldFlip);
            }
        }
        else
        {
            // 普通移动动画
            if (movementInput.magnitude < 0.01f)
            {
                animController.PlayIdle(direction, shouldFlip);
            }
            else if (isShiftHeld)
            {
                animController.PlayRun(direction, shouldFlip);
            }
            else
            {
                animController.PlayWalk(direction, shouldFlip);
            }
        }
    }

    private PlayerAnimController.AnimDirection GetDirection()
    {
        if (Mathf.Abs(movementInput.y) > Mathf.Abs(movementInput.x))
        {
            return movementInput.y > 0 ? 
                PlayerAnimController.AnimDirection.Up : 
                PlayerAnimController.AnimDirection.Down;
        }
        else
        {
            return movementInput.x > 0 ? 
                PlayerAnimController.AnimDirection.Right : 
                PlayerAnimController.AnimDirection.Left;
        }
    }

    public void SetMovementInput(Vector2 input, bool isShifted)
    {
        SetMovementInput(input, isShifted, null);
    }

    public void SetNavigationInput(Vector2 input, bool isShifted, Vector2? facingDirection = null)
    {
        ApplyMovementInput(input, isShifted, facingDirection);
        ClearBlockedNavigationConstraint();
    }

    public void SetBlockedNavigationInput(
        Vector2 input,
        bool isShifted,
        Vector2 blockerPosition,
        float clearance,
        Vector2? facingDirection = null)
    {
        ApplyMovementInput(input, isShifted, facingDirection);
        hasBlockedNavigationConstraint = true;
        blockedNavigationBlockerPosition = blockerPosition;
        blockedNavigationClearance = clearance;
    }

    /// <summary>
    /// 设置移动输入，可选指定朝向方向
    /// </summary>
    /// <param name="input">移动输入</param>
    /// <param name="isShifted">是否疾跑</param>
    /// <param name="facingDirection">可选的朝向方向（用于斜向移动时固定朝向）</param>
    public void SetMovementInput(Vector2 input, bool isShifted, Vector2? facingDirection)
    {
        ApplyMovementInput(input, isShifted, facingDirection);
        ClearBlockedNavigationConstraint();
    }

    private void ApplyMovementInput(Vector2 input, bool isShifted, Vector2? facingDirection)
    {
        movementInput = input;
        
        // ✅ 从 SprintStateManager 获取统一的疾跑状态
        if (sprintManager != null)
        {
            // 通知 SprintStateManager 移动状态
            bool hasInput = input.magnitude > 0.01f;
            sprintManager.OnMovementInput(hasInput);
            
            isShiftHeld = sprintManager.IsSprinting();
        }
        else
        {
            isShiftHeld = isShifted;
        }
        
        // 🔥 如果指定了朝向方向，使用指定的朝向
        if (facingDirection.HasValue && facingDirection.Value.sqrMagnitude > 0.01f)
        {
            SetFacingDirection(facingDirection.Value);
        }
    }

    private void UpdateFacingDirection()
    {
        // 有输入就更新朝向（不管是否在动作中）
        if (movementInput.magnitude > 0.01f)
        {
            currentFacingDirection = GetDirection();
        }
    }
    
    public PlayerAnimController.AnimDirection GetCurrentFacingDirection()
    {
        return currentFacingDirection;
    }
    
    /// <summary>
    /// 设置玩家朝向（用于缓存方向应用）
    /// </summary>
    /// <param name="direction">方向向量</param>
    public void SetFacingDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;
        
        // 将方向向量转换为 AnimDirection
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            currentFacingDirection = direction.y > 0 
                ? PlayerAnimController.AnimDirection.Up 
                : PlayerAnimController.AnimDirection.Down;
        }
        else
        {
            currentFacingDirection = direction.x > 0 
                ? PlayerAnimController.AnimDirection.Right 
                : PlayerAnimController.AnimDirection.Left;
        }
    }

    public void StopMovement()
    {
        movementInput = Vector2.zero;
        ClearBlockedNavigationConstraint();
        ResetRuntimeMotionModifiers();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void ApplyBlockedNavigationVelocity(Vector2 desiredVelocity, float currentSpeed)
    {
        Vector2 constrainedVelocity = desiredVelocity;
        Vector2 currentCenter = GetCurrentCenter();
        Vector2 toBlocker = blockedNavigationBlockerPosition - currentCenter;

        if (toBlocker.sqrMagnitude > 0.0001f)
        {
            Vector2 blockerNormal = toBlocker.normalized;
            float inwardSpeed = Vector2.Dot(constrainedVelocity, blockerNormal);
            float inwardAllowance = blockedNavigationClearance > blockedNavigationForwardClearanceSlack
                ? inwardSpeed * 0.15f
                : 0f;

            if (inwardSpeed > inwardAllowance)
            {
                constrainedVelocity -= blockerNormal * (inwardSpeed - inwardAllowance);
            }
        }

        float speedCapCeiling = Mathf.Max(blockedNavigationMinSpeed, currentSpeed * blockedNavigationMaxSpeedFactor);
        float maxBlockedSpeed = Mathf.Clamp(
            constrainedVelocity.magnitude + blockedNavigationSpeedBuffer,
            blockedNavigationMinSpeed,
            speedCapCeiling);

        Vector2 finalVelocity = Vector2.ClampMagnitude(constrainedVelocity, maxBlockedSpeed);
        if (toBlocker.sqrMagnitude > 0.0001f)
        {
            Vector2 blockerNormal = toBlocker.normalized;
            float finalInwardSpeed = Vector2.Dot(finalVelocity, blockerNormal);
            if (finalInwardSpeed > 0f)
            {
                finalVelocity -= blockerNormal * finalInwardSpeed;
            }
        }

        rb.linearDamping = Mathf.Max(defaultLinearDamping, blockedNavigationDamping);
        rb.linearVelocity = finalVelocity;
    }

    private void ClearBlockedNavigationConstraint()
    {
        hasBlockedNavigationConstraint = false;
        blockedNavigationBlockerPosition = Vector2.zero;
        blockedNavigationClearance = float.PositiveInfinity;
    }

    private void ResetRuntimeMotionModifiers()
    {
        if (rb != null && !Mathf.Approximately(rb.linearDamping, defaultLinearDamping))
        {
            rb.linearDamping = defaultLinearDamping;
        }
    }

    private Vector2 GetCurrentCenter()
    {
        if (playerCollider != null)
        {
            return playerCollider.bounds.center;
        }

        if (rb != null)
        {
            return rb.position;
        }

        return transform.position;
    }
}
