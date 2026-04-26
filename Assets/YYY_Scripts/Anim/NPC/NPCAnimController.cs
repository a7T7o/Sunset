using UnityEngine;

/// <summary>
/// NPC 动画控制器。
/// 只维护 Idle / Move 两种状态，并负责左右翻转桥接。
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class NPCAnimController : MonoBehaviour
{
    #region 枚举

    public enum NPCAnimState
    {
        Idle = 0,
        Move = 1
    }

    public enum NPCAnimDirection
    {
        Down = 0,
        Up = 1,
        Right = 2,
        Left = 3
    }

    #endregion

    #region 序列化字段

    [Header("组件引用")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("动画速度")]
    [SerializeField, Range(0.1f, 3f)] private float idleAnimationSpeed = 1f;
    [SerializeField, Range(0.1f, 3f)] private float moveAnimationSpeed = 1f;

    [Header("朝向映射")]
    [SerializeField] private bool useCustomFacingVisualMap = false;
    [SerializeField, Min(0)] private int animatorDirectionWhenFacingDown = 0;
    [SerializeField, Min(0)] private int animatorDirectionWhenFacingUp = 1;
    [SerializeField, Min(0)] private int animatorDirectionWhenFacingRight = 2;
    [SerializeField, Min(0)] private int animatorDirectionWhenFacingLeft = 2;
    [SerializeField] private bool flipXWhenFacingDown = false;
    [SerializeField] private bool flipXWhenFacingUp = false;
    [SerializeField] private bool flipXWhenFacingRight = false;
    [SerializeField] private bool flipXWhenFacingLeft = true;

    [Header("调试")]
    [SerializeField] private bool showDebugLog = false;

    #endregion

    #region 私有字段

    private NPCAnimState _currentState = NPCAnimState.Idle;
    private NPCAnimDirection _currentDirection = NPCAnimDirection.Down;
    private bool _isFlipped;
    private bool _warnedMissingController;

    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int DirectionHash = Animator.StringToHash("Direction");

    private readonly struct FacingVisualConfig
    {
        public readonly int AnimatorDirection;
        public readonly bool FlipX;

        public FacingVisualConfig(int animatorDirection, bool flipX)
        {
            AnimatorDirection = animatorDirection;
            FlipX = flipX;
        }
    }

    #endregion

    #region 公共属性

    public NPCAnimState CurrentState => _currentState;
    public NPCAnimDirection CurrentDirection => _currentDirection;
    public bool IsFlipped => _isFlipped;
    public float IdleAnimationSpeed => idleAnimationSpeed;
    public float MoveAnimationSpeed => moveAnimationSpeed;

    #endregion

    #region Unity生命周期

    private void Reset()
    {
        CacheComponents();
    }

    private void Awake()
    {
        CacheComponents();
        ApplyVisualState(force: true);
    }

    private void OnValidate()
    {
        idleAnimationSpeed = Mathf.Max(0.1f, idleAnimationSpeed);
        moveAnimationSpeed = Mathf.Max(0.1f, moveAnimationSpeed);
        animatorDirectionWhenFacingDown = Mathf.Max(0, animatorDirectionWhenFacingDown);
        animatorDirectionWhenFacingUp = Mathf.Max(0, animatorDirectionWhenFacingUp);
        animatorDirectionWhenFacingRight = Mathf.Max(0, animatorDirectionWhenFacingRight);
        animatorDirectionWhenFacingLeft = Mathf.Max(0, animatorDirectionWhenFacingLeft);

        if (!Application.isPlaying)
        {
            CacheComponents();
        }
    }

    #endregion

    #region 公共方法

    public void SetState(NPCAnimState state, bool force = false)
    {
        if (!force && _currentState == state)
        {
            return;
        }

        _currentState = state;
        ApplyVisualState(force);
    }

    public void SetDirection(NPCAnimDirection direction, bool force = false)
    {
        if (!force && _currentDirection == direction)
        {
            return;
        }

        _currentDirection = direction;
        ApplyVisualState(force);
    }

    public void PlayIdle(NPCAnimDirection direction, bool force = false)
    {
        if (!force && _currentState == NPCAnimState.Idle && _currentDirection == direction)
        {
            return;
        }

        _currentState = NPCAnimState.Idle;
        _currentDirection = direction;
        ApplyVisualState(force);
    }

    public void PlayMove(NPCAnimDirection direction, bool force = false)
    {
        if (!force && _currentState == NPCAnimState.Move && _currentDirection == direction)
        {
            return;
        }

        _currentState = NPCAnimState.Move;
        _currentDirection = direction;
        ApplyVisualState(force);
    }

    public void ApplyMovementVisual(Vector2 movement)
    {
        if (movement.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        SetDirection(ConvertVectorToDirection(movement));
    }

    public void SetAnimationSpeed(float idleSpeed, float moveSpeed)
    {
        idleAnimationSpeed = Mathf.Max(0.1f, idleSpeed);
        moveAnimationSpeed = Mathf.Max(0.1f, moveSpeed);
        ApplyAnimatorSpeed();
    }

    [ContextMenu("调试/Idle 朝下")]
    public void DebugIdleDown()
    {
        PlayIdle(NPCAnimDirection.Down, force: true);
    }

    [ContextMenu("调试/Move 朝下")]
    public void DebugMoveDown()
    {
        PlayMove(NPCAnimDirection.Down, force: true);
    }

    #endregion

    #region 私有方法

    private void CacheComponents()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void ApplyVisualState(bool force)
    {
        if (animator == null || spriteRenderer == null)
        {
            CacheComponents();
        }

        if (animator == null || spriteRenderer == null)
        {
            return;
        }

        FacingVisualConfig visualConfig = ResolveFacingVisualConfig(_currentDirection);
        bool shouldFlip = visualConfig.FlipX;
        if (force || _isFlipped != shouldFlip)
        {
            _isFlipped = shouldFlip;
            spriteRenderer.flipX = shouldFlip;
        }

        ApplyAnimatorSpeed();

        if (!animator.isActiveAndEnabled || !gameObject.activeInHierarchy || !animator.isInitialized)
        {
            return;
        }

        if (!HasRuntimeController())
        {
            if (!_warnedMissingController)
            {
                _warnedMissingController = true;
                Debug.LogWarning(
                    $"[NPCAnimController] {name} 的 Animator 尚未绑定 RuntimeAnimatorController，先跳过参数驱动。",
                    this);
            }

            return;
        }

        _warnedMissingController = false;
        animator.SetInteger(StateHash, (int)_currentState);
        animator.SetInteger(DirectionHash, visualConfig.AnimatorDirection);

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=cyan>[NPCAnimController]</color> {name} => State={_currentState}, Direction={_currentDirection}, AnimatorDirection={visualConfig.AnimatorDirection}, Flip={_isFlipped}, Speed={animator.speed:F2}",
                this);
        }
    }

    private void ApplyAnimatorSpeed()
    {
        if (animator == null)
        {
            return;
        }

        animator.speed = _currentState == NPCAnimState.Move
            ? moveAnimationSpeed
            : idleAnimationSpeed;
    }

    private bool HasRuntimeController()
    {
        return animator != null && animator.runtimeAnimatorController != null;
    }

    private FacingVisualConfig ResolveFacingVisualConfig(NPCAnimDirection direction)
    {
        if (!useCustomFacingVisualMap)
        {
            return direction switch
            {
                NPCAnimDirection.Down => new FacingVisualConfig(0, false),
                NPCAnimDirection.Up => new FacingVisualConfig(1, false),
                NPCAnimDirection.Right => new FacingVisualConfig(2, false),
                NPCAnimDirection.Left => new FacingVisualConfig(2, true),
                _ => new FacingVisualConfig(0, false)
            };
        }

        return direction switch
        {
            NPCAnimDirection.Down => new FacingVisualConfig(animatorDirectionWhenFacingDown, flipXWhenFacingDown),
            NPCAnimDirection.Up => new FacingVisualConfig(animatorDirectionWhenFacingUp, flipXWhenFacingUp),
            NPCAnimDirection.Right => new FacingVisualConfig(animatorDirectionWhenFacingRight, flipXWhenFacingRight),
            NPCAnimDirection.Left => new FacingVisualConfig(animatorDirectionWhenFacingLeft, flipXWhenFacingLeft),
            _ => new FacingVisualConfig(animatorDirectionWhenFacingDown, flipXWhenFacingDown)
        };
    }

    private NPCAnimDirection ConvertVectorToDirection(Vector2 movement)
    {
        if (Mathf.Abs(movement.y) > Mathf.Abs(movement.x))
        {
            return movement.y >= 0f ? NPCAnimDirection.Up : NPCAnimDirection.Down;
        }

        return movement.x >= 0f ? NPCAnimDirection.Right : NPCAnimDirection.Left;
    }

    #endregion
}
