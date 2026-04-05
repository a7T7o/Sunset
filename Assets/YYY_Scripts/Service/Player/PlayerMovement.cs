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

    [Header("剧情运行时修正")]
    [SerializeField, Range(0.1f, 2f)] private float runtimeSpeedMultiplier = 1f;

    [Header("导航边界约束")]
    [SerializeField] private NavGrid2D navGrid;
    [SerializeField] private bool autoFindNavGridIfMissing = true;
    [SerializeField] private bool enforceNavGridBounds = false;
    [SerializeField] private bool allowTraversalOverridePhysicsSoftPass = true;

    [Header("脚底导航采样")]
    [SerializeField, Min(0f)] private float navigationFootProbeVerticalInset = 0.08f;
    [SerializeField, Min(0f)] private float navigationFootProbeSideInset = 0.05f;
    [SerializeField, Min(0f)] private float navigationFootProbeExtraRadius = 0.02f;

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
    private bool hasLoggedMissingNavGridWarning = false;
    private Collider2D[] traversalSoftPassBlockers = new Collider2D[0];
    private bool isTraversalSoftPassActive = false;
    
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
        EnsureNavGridReference(logIfMissing: false);
        
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

    void OnDisable()
    {
        ClearTraversalSoftPassState();
    }

    void OnDestroy()
    {
        ClearTraversalSoftPassState();
    }
    
    private void UpdateMovement()
    {
        if (rb == null)
        {
            return;
        }

        UpdateTraversalSoftPassState();

        float currentSpeed = (isShiftHeld ? RunSpeed : WalkSpeed) * runtimeSpeedMultiplier;
        Vector2 desiredVelocity = Vector2.ClampMagnitude(movementInput, 1f) * currentSpeed;

        if (hasBlockedNavigationConstraint)
        {
            ApplyBlockedNavigationVelocity(desiredVelocity, currentSpeed);
            if (enforceNavGridBounds)
            {
                rb.linearVelocity = ConstrainVelocityToNavigationBounds(rb.linearVelocity);
            }
            return;
        }

        ResetRuntimeMotionModifiers();
        rb.linearVelocity = enforceNavGridBounds
            ? ConstrainVelocityToNavigationBounds(desiredVelocity)
            : desiredVelocity;
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

    public void SetRuntimeSpeedMultiplier(float multiplier)
    {
        runtimeSpeedMultiplier = Mathf.Clamp(multiplier, 0.1f, 2f);
    }

    public void ResetRuntimeSpeedMultiplier()
    {
        runtimeSpeedMultiplier = 1f;
    }

    public float GetRuntimeSpeedMultiplier()
    {
        return runtimeSpeedMultiplier;
    }

    public void SetNavGrid(NavGrid2D navigationGrid)
    {
        navGrid = navigationGrid;
        hasLoggedMissingNavGridWarning = false;
    }

    public void SetNavGridBoundsEnforcement(bool enabled)
    {
        enforceNavGridBounds = enabled;
    }

    public void SetTraversalSoftPassBlockers(Collider2D[] colliders, bool enabled = true)
    {
        if (isTraversalSoftPassActive)
        {
            ApplyTraversalSoftPass(traversalSoftPassBlockers, false);
            isTraversalSoftPassActive = false;
        }

        allowTraversalOverridePhysicsSoftPass = enabled;
        traversalSoftPassBlockers = NavigationTraversalCore.NormalizeColliderArray(colliders);
        UpdateTraversalSoftPassState();
    }

    public bool HasNavGridReference()
    {
        return navGrid != null;
    }

    public bool TryResolveNavGrid(bool logIfMissing = true)
    {
        return EnsureNavGridReference(logIfMissing);
    }

    public NavGrid2D GetNavGrid()
    {
        return navGrid;
    }

    public bool CanOccupyPosition(Vector2 worldCenter)
    {
        return CanOccupyNavigationPoint(worldCenter);
    }

    public void GetNavigationProbePoints(
        Vector2 worldCenter,
        out Vector2 centerProbe,
        out Vector2 leftProbe,
        out Vector2 rightProbe)
    {
        NavigationTraversalCore.ProbePoints probePoints = NavigationTraversalCore.GetNavigationProbePoints(GetTraversalContract(), worldCenter);
        centerProbe = probePoints.Center;
        leftProbe = probePoints.Left;
        rightProbe = probePoints.Right;
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

    private Vector2 ConstrainVelocityToNavigationBounds(Vector2 desiredVelocity)
    {
        if (desiredVelocity.sqrMagnitude <= 0.0001f)
        {
            return Vector2.zero;
        }

        if (!EnsureNavGridReference(logIfMissing: true))
        {
            return desiredVelocity;
        }

        Vector2 currentCenter = GetCurrentCenter();
        float stepDuration = Mathf.Max(Time.fixedDeltaTime, 0.02f);
        return NavigationTraversalCore.ConstrainVelocityToNavigationBounds(
            GetTraversalContract(),
            currentCenter,
            desiredVelocity,
            stepDuration);
    }

    private Vector2 ConstrainVelocityByAxes(Vector2 currentCenter, Vector2 desiredVelocity, float stepDuration, bool tryXFirst)
    {
        Vector2 constrainedVelocity = Vector2.zero;
        Vector2 simulatedCenter = currentCenter;

        TryApplyAxisVelocity(ref constrainedVelocity, ref simulatedCenter, desiredVelocity, stepDuration, applyX: tryXFirst);
        TryApplyAxisVelocity(ref constrainedVelocity, ref simulatedCenter, desiredVelocity, stepDuration, applyX: !tryXFirst);

        return constrainedVelocity;
    }

    private void TryApplyAxisVelocity(
        ref Vector2 constrainedVelocity,
        ref Vector2 simulatedCenter,
        Vector2 desiredVelocity,
        float stepDuration,
        bool applyX)
    {
        float axisVelocity = applyX ? desiredVelocity.x : desiredVelocity.y;
        if (Mathf.Abs(axisVelocity) <= 0.0001f)
        {
            return;
        }

        Vector2 axisOffset = applyX
            ? new Vector2(axisVelocity * stepDuration, 0f)
            : new Vector2(0f, axisVelocity * stepDuration);

        if (!CanOccupyNavigationPoint(simulatedCenter + axisOffset))
        {
            return;
        }

        simulatedCenter += axisOffset;
        if (applyX)
        {
            constrainedVelocity.x = axisVelocity;
        }
        else
        {
            constrainedVelocity.y = axisVelocity;
        }
    }

    private bool CanOccupyNavigationPoint(Vector2 worldCenter)
    {
        if (!EnsureNavGridReference(logIfMissing: false))
        {
            return true;
        }

        return NavigationTraversalCore.CanOccupyNavigationPoint(GetTraversalContract(), worldCenter);
    }

    private void UpdateTraversalSoftPassState()
    {
        bool shouldSoftPass = ShouldEnableTraversalSoftPass();
        if (shouldSoftPass == isTraversalSoftPassActive)
        {
            return;
        }

        ApplyTraversalSoftPass(traversalSoftPassBlockers, shouldSoftPass);
        isTraversalSoftPassActive = shouldSoftPass;
    }

    private bool ShouldEnableTraversalSoftPass()
    {
        if (!EnsureNavGridReference(logIfMissing: false))
        {
            return false;
        }

        return NavigationTraversalCore.ShouldEnableTraversalSoftPass(
            GetTraversalContract(),
            GetCurrentCenter(),
            allowTraversalOverridePhysicsSoftPass,
            traversalSoftPassBlockers);
    }

    private NavigationTraversalCore.Contract GetTraversalContract()
    {
        return new NavigationTraversalCore.Contract(
            navGrid,
            playerCollider,
            navigationFootProbeVerticalInset,
            navigationFootProbeSideInset,
            navigationFootProbeExtraRadius);
    }

    private void ApplyTraversalSoftPass(Collider2D[] colliders, bool shouldIgnore)
    {
        if (playerCollider == null || colliders == null)
        {
            return;
        }

        for (int index = 0; index < colliders.Length; index++)
        {
            Collider2D blocker = colliders[index];
            if (blocker == null || blocker == playerCollider)
            {
                continue;
            }

            Physics2D.IgnoreCollision(playerCollider, blocker, shouldIgnore);
        }
    }

    private void ClearTraversalSoftPassState()
    {
        if (!isTraversalSoftPassActive)
        {
            return;
        }

        ApplyTraversalSoftPass(traversalSoftPassBlockers, false);
        isTraversalSoftPassActive = false;
    }

    private static Collider2D[] NormalizeColliderArray(Collider2D[] source)
    {
        if (source == null || source.Length == 0)
        {
            return new Collider2D[0];
        }

        var normalized = new System.Collections.Generic.List<Collider2D>(source.Length);
        for (int index = 0; index < source.Length; index++)
        {
            Collider2D collider = source[index];
            if (collider == null || normalized.Contains(collider))
            {
                continue;
            }

            normalized.Add(collider);
        }

        return normalized.ToArray();
    }

    private Vector2 GetFootProbeCenter(Vector2 worldCenter)
    {
        if (playerCollider == null)
        {
            return worldCenter;
        }

        Bounds bounds = playerCollider.bounds;
        Vector2 currentCenter = bounds.center;
        float footProbeInset = Mathf.Clamp(navigationFootProbeVerticalInset, 0f, bounds.size.y * 0.45f);
        float footProbeY = bounds.min.y + footProbeInset;
        return worldCenter + new Vector2(0f, footProbeY - currentCenter.y);
    }

    private float GetLateralFootProbeOffset()
    {
        if (playerCollider == null)
        {
            return 0f;
        }

        Bounds bounds = playerCollider.bounds;
        float sideInset = Mathf.Clamp(navigationFootProbeSideInset, 0f, bounds.extents.x);
        return Mathf.Max(0.02f, bounds.extents.x - sideInset);
    }

    private void GetTraversalSupportProbePoints(
        Vector2 worldCenter,
        out Vector2 centerProbe,
        out Vector2 leftProbe,
        out Vector2 rightProbe)
    {
        centerProbe = GetFootProbeCenter(worldCenter);
        float lateralOffset = GetLateralFootProbeOffset() * 0.72f;
        if (lateralOffset <= 0.03f)
        {
            leftProbe = centerProbe;
            rightProbe = centerProbe;
            return;
        }

        leftProbe = centerProbe + Vector2.left * lateralOffset;
        rightProbe = centerProbe + Vector2.right * lateralOffset;
    }

    private float GetTraversalSupportQueryRadius(float navigationQueryRadius)
    {
        return Mathf.Max(0.03f, navigationQueryRadius * 0.85f);
    }

    private bool IsTraversalBridgeCenterSupported(Vector2 footProbeCenter, float navigationQueryRadius)
    {
        float supportRadius = GetTraversalSupportQueryRadius(navigationQueryRadius);
        return navGrid != null && navGrid.HasWalkableOverrideAt(footProbeCenter, supportRadius);
    }

    private float GetNavigationPointQueryRadius()
    {
        float fallbackRadius = Mathf.Max(0.04f, navigationFootProbeSideInset + navigationFootProbeExtraRadius);
        if (!EnsureNavGridReference(logIfMissing: false))
        {
            return fallbackRadius;
        }

        float maxRadius = navGrid.GetAgentRadius();
        if (playerCollider == null)
        {
            return Mathf.Min(maxRadius, fallbackRadius);
        }

        Bounds bounds = playerCollider.bounds;
        float sideRadius = Mathf.Max(0.03f, navigationFootProbeSideInset + navigationFootProbeExtraRadius);
        float verticalRadius = Mathf.Max(0.03f, navigationFootProbeVerticalInset + navigationFootProbeExtraRadius);
        float clampedRadius = Mathf.Min(Mathf.Min(bounds.extents.x, bounds.extents.y), Mathf.Max(sideRadius, verticalRadius));
        return Mathf.Min(maxRadius, clampedRadius);
    }

    private bool EnsureNavGridReference(bool logIfMissing)
    {
        if (navGrid == null && autoFindNavGridIfMissing)
        {
            navGrid = FindFirstObjectByType<NavGrid2D>();
        }

        if (navGrid != null)
        {
            hasLoggedMissingNavGridWarning = false;
            return true;
        }

        if (logIfMissing && !hasLoggedMissingNavGridWarning)
        {
            Debug.LogWarning($"[PlayerMovement] {name} 未接到 NavGrid2D，当前不会做 traversal / blocking 边界拦截。", this);
            hasLoggedMissingNavGridWarning = true;
        }

        return false;
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
