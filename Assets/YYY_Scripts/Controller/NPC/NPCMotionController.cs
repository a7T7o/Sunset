using UnityEngine;

/// <summary>
/// NPC 运动与动画桥接器。
/// 默认作为“被动观察者”工作：侦测 Rigidbody2D 或 Transform 位移，并驱动 NPCAnimController。
/// </summary>
[DisallowMultipleComponent]
public class NPCMotionController : MonoBehaviour
{
    #region 序列化字段

    [Header("组件引用")]
    [SerializeField] private NPCAnimController animController;
    [SerializeField] private Rigidbody2D rb;

    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float moveThreshold = 0.05f;
    [SerializeField] private float facingDirectionMinHoldSeconds = 0.08f;
    [SerializeField] private float navigationAcceleration = 16f;
    [SerializeField] private float navigationDeceleration = 20f;
    [SerializeField] private float navigationRecoverAcceleration = 12f;
    [SerializeField] private bool useRigidbodyVelocity = true;
    [SerializeField] private bool autoDetectMovement = true;

    [Header("默认朝向")]
    [SerializeField] private NPCAnimController.NPCAnimDirection defaultFacingDirection = NPCAnimController.NPCAnimDirection.Down;

    [Header("调试")]
    [SerializeField] private bool showDebugLog = false;
    [SerializeField] private bool enableEditorPreview = false;
    [SerializeField] private Vector2 previewMoveInput = Vector2.zero;

    #endregion

    #region 私有字段

    private Vector3 _lastPosition;
    private Vector2 _externalVelocity;
    private bool _hasExternalVelocity;
    private float _lastObservedMovementTime = float.NegativeInfinity;
    private float _lastFacingDirectionChangeTime = float.NegativeInfinity;
    private NPCAnimController.NPCAnimDirection _lastSmoothedDirection = NPCAnimController.NPCAnimDirection.Down;

    #endregion

    #region 公共属性

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Max(0f, value);
    }

    public bool IsMoving { get; private set; }
    public Vector2 CurrentVelocity { get; private set; }
    public Vector2 ReportedVelocity => _hasExternalVelocity ? _externalVelocity : CurrentVelocity;

    #endregion

    #region Unity生命周期

    private void Reset()
    {
        CacheComponents();
    }

    private void Awake()
    {
        CacheComponents();
        _lastPosition = transform.position;
        _lastObservedMovementTime = float.NegativeInfinity;
    }

    private void Start()
    {
        if (animController != null)
        {
            animController.PlayIdle(defaultFacingDirection, force: true);
            _lastSmoothedDirection = defaultFacingDirection;
            _lastFacingDirectionChangeTime = Time.time;
        }
    }

    private void Update()
    {
        if (animController == null)
        {
            return;
        }

        Vector2 velocity = ResolveVelocity();
        CurrentVelocity = velocity;
        IsMoving = autoDetectMovement && velocity.magnitude >= moveThreshold;

        if (IsMoving)
        {
            NPCAnimController.NPCAnimDirection targetDirection = GetDirectionFromVelocity(velocity);
            targetDirection = GetSmoothedFacingDirection(targetDirection);
            animController.PlayMove(targetDirection);
        }
        else
        {
            animController.PlayIdle(animController.CurrentDirection);
        }

        if (showDebugLog && Time.frameCount % 30 == 0)
        {
            Debug.Log(
                $"<color=yellow>[NPCMotionController]</color> {name} => Velocity={velocity}, IsMoving={IsMoving}, MoveSpeed={moveSpeed:F2}",
                this);
        }

        _lastPosition = transform.position;
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        moveThreshold = Mathf.Max(0.001f, moveThreshold);
        facingDirectionMinHoldSeconds = Mathf.Clamp(facingDirectionMinHoldSeconds, 0f, 0.4f);
        navigationAcceleration = Mathf.Max(0.1f, navigationAcceleration);
        navigationDeceleration = Mathf.Max(0.1f, navigationDeceleration);
        navigationRecoverAcceleration = Mathf.Max(0.1f, navigationRecoverAcceleration);

        if (!Application.isPlaying)
        {
            CacheComponents();
        }
    }

    #endregion

    #region 公共方法

    public void SetFacingDirection(Vector2 direction)
    {
        if (animController == null || direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        NPCAnimController.NPCAnimDirection facingDirection = GetDirectionFromVelocity(direction);
        animController.SetDirection(facingDirection, force: true);
        _lastSmoothedDirection = facingDirection;
        _lastFacingDirectionChangeTime = Time.time;
    }

    public void SetExternalVelocity(Vector2 velocity)
    {
        _externalVelocity = velocity;
        _hasExternalVelocity = true;
    }

    public void ClearExternalVelocity()
    {
        _externalVelocity = Vector2.zero;
        _hasExternalVelocity = false;
    }

    public void StopMotion()
    {
        _externalVelocity = Vector2.zero;
        _hasExternalVelocity = false;
        CurrentVelocity = Vector2.zero;
        IsMoving = false;
        _lastObservedMovementTime = float.NegativeInfinity;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (animController != null)
        {
            animController.PlayIdle(animController.CurrentDirection, force: true);
            _lastSmoothedDirection = animController.CurrentDirection;
            _lastFacingDirectionChangeTime = Time.time;
        }

        _lastPosition = transform.position;
    }

    public void ResetMotionObservation(bool clearExternalVelocity = true)
    {
        if (clearExternalVelocity)
        {
            _externalVelocity = Vector2.zero;
            _hasExternalVelocity = false;
        }

        CurrentVelocity = Vector2.zero;
        IsMoving = false;
        _lastObservedMovementTime = float.NegativeInfinity;
        _lastPosition = transform.position;
    }

    [ContextMenu("调试/向右移动预览")]
    public void PreviewMoveRight()
    {
        if (Application.isPlaying)
        {
            SetExternalVelocity(Vector2.right * moveSpeed);
        }
    }

    [ContextMenu("调试/停止移动预览")]
    public void PreviewStop()
    {
        if (Application.isPlaying)
        {
            StopMotion();
        }
    }

    #endregion

    #region 私有方法

    private void CacheComponents()
    {
        if (animController == null)
        {
            animController = GetComponent<NPCAnimController>();
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private Vector2 ResolveVelocity()
    {
        if (enableEditorPreview && previewMoveInput.sqrMagnitude > 0.0001f)
        {
            return previewMoveInput.normalized * moveSpeed;
        }

        float deltaTime = Mathf.Max(Time.deltaTime, 0.0001f);
        Vector3 delta = transform.position - _lastPosition;
        Vector2 transformVelocity = new Vector2(delta.x, delta.y) / deltaTime;
        Vector2 rigidbodyVelocity = useRigidbodyVelocity && rb != null
            ? rb.linearVelocity
            : Vector2.zero;

        if (transformVelocity.sqrMagnitude >= moveThreshold * moveThreshold)
        {
            _lastObservedMovementTime = Time.time;
            return transformVelocity;
        }

        if (rigidbodyVelocity.sqrMagnitude >= moveThreshold * moveThreshold)
        {
            // Contacts can leave Rigidbody2D with a non-zero intent velocity while the visible pose is already stuck.
            if (Time.time - _lastObservedMovementTime <= 0.12f)
            {
                return rigidbodyVelocity;
            }

            return Vector2.zero;
        }

        if (_hasExternalVelocity && rb == null)
        {
            return _externalVelocity;
        }

        return transformVelocity;
    }

    private NPCAnimController.NPCAnimDirection GetDirectionFromVelocity(Vector2 velocity)
    {
        if (Mathf.Abs(velocity.y) > Mathf.Abs(velocity.x))
        {
            return velocity.y >= 0f
                ? NPCAnimController.NPCAnimDirection.Up
                : NPCAnimController.NPCAnimDirection.Down;
        }

        return velocity.x >= 0f
            ? NPCAnimController.NPCAnimDirection.Right
            : NPCAnimController.NPCAnimDirection.Left;
    }

    private NPCAnimController.NPCAnimDirection GetSmoothedFacingDirection(
        NPCAnimController.NPCAnimDirection rawDirection)
    {
        if (rawDirection == _lastSmoothedDirection)
        {
            return rawDirection;
        }

        if (Time.time - _lastFacingDirectionChangeTime < facingDirectionMinHoldSeconds)
        {
            return _lastSmoothedDirection;
        }

        _lastSmoothedDirection = rawDirection;
        _lastFacingDirectionChangeTime = Time.time;
        return rawDirection;
    }

    #endregion
}
