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

    [Header("调试")]
    [SerializeField] private bool showDebugLog = false;

    #endregion

    #region 私有字段

    private NPCAnimState _currentState = NPCAnimState.Idle;
    private NPCAnimDirection _currentDirection = NPCAnimDirection.Down;
    private bool _isFlipped;

    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int DirectionHash = Animator.StringToHash("Direction");

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
        _currentState = NPCAnimState.Idle;
        _currentDirection = direction;
        ApplyVisualState(force);
    }

    public void PlayMove(NPCAnimDirection direction, bool force = false)
    {
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

        animator.SetInteger(StateHash, (int)_currentState);
        animator.SetInteger(DirectionHash, ConvertToAnimatorDirection(_currentDirection));

        bool shouldFlip = _currentDirection == NPCAnimDirection.Left;
        if (force || _isFlipped != shouldFlip)
        {
            _isFlipped = shouldFlip;
            spriteRenderer.flipX = shouldFlip;
        }

        ApplyAnimatorSpeed();

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=cyan>[NPCAnimController]</color> {name} => State={_currentState}, Direction={_currentDirection}, Flip={_isFlipped}, Speed={animator.speed:F2}",
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

    private int ConvertToAnimatorDirection(NPCAnimDirection direction)
    {
        return direction switch
        {
            NPCAnimDirection.Down => 0,
            NPCAnimDirection.Up => 1,
            NPCAnimDirection.Right => 2,
            NPCAnimDirection.Left => 2,
            _ => 0
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
