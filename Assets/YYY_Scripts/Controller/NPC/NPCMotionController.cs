using System.IO;
using System.Runtime.CompilerServices;
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
    [SerializeField] private float externalFacingMinHoldSeconds = 0.22f;
    [SerializeField] private float oppositeFacingConfirmSeconds = 0.18f;
    [SerializeField, Range(0f, 25f)] private float facingBoundaryHysteresisDegrees = 12f;
    [SerializeField, Range(-1f, 1f)] private float movingFacingAlignmentThreshold = 0.55f;
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
    private Vector2 _externalFacingDirection;
    private bool _hasExternalVelocity;
    private bool _hasExternalFacingDirection;
    private float _lastObservedMovementTime = float.NegativeInfinity;
    private float _lastFacingDirectionChangeTime = float.NegativeInfinity;
    private float _pendingFacingDirectionSince = float.NegativeInfinity;
    private NPCAnimController.NPCAnimDirection _lastSmoothedDirection = NPCAnimController.NPCAnimDirection.Down;
    private NPCAnimController.NPCAnimDirection? _pendingFacingDirection;
    private NPCAnimController.NPCAnimDirection? _pendingObservedFacingOverrideDirection;
    private float _pendingObservedFacingOverrideSince = float.NegativeInfinity;
#if UNITY_EDITOR
    private int _facingMismatchFrameCount;
    private int _facingMismatchBurstCount;
    private float _lastFacingMismatchLogTime = float.NegativeInfinity;
    private float _lastFacingMismatchSeenTime = float.NegativeInfinity;
#endif
    private string _lastFacingWriteSource = "none";
    private string _lastExternalVelocitySource = "none";
    private string _lastExternalFacingSource = "none";

#if UNITY_EDITOR
    private const int FacingMismatchLogMinFrames = 3;
    private const float FacingMismatchLogCooldownSeconds = 0.35f;
    private static bool s_emitFacingMismatchDiagnosticsInEditor = false;
#endif

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
    public string DebugLastFacingWriteSource => _lastFacingWriteSource ?? "none";
    public string DebugLastExternalVelocitySource => _lastExternalVelocitySource ?? "none";
    public string DebugLastExternalFacingSource => _lastExternalFacingSource ?? "none";

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
        Vector2 facingVelocity = ResolveFacingVelocity(velocity);
        CurrentVelocity = velocity;
        IsMoving = autoDetectMovement && facingVelocity.magnitude >= moveThreshold;

        if (IsMoving)
        {
            NPCAnimController.NPCAnimDirection targetDirection = GetDirectionFromVelocity(facingVelocity);
            targetDirection = ApplyFacingSectorHysteresis(facingVelocity, targetDirection);
            targetDirection = GetSmoothedFacingDirection(targetDirection);
            animController.PlayMove(targetDirection);
            EmitFacingMismatchDiagnostics(velocity, facingVelocity, targetDirection);
        }
        else
        {
            animController.PlayIdle(animController.CurrentDirection);
            ResetFacingMismatchDiagnostics();
        }

        if (showDebugLog && Time.frameCount % 30 == 0)
        {
            string observedDirection = velocity.sqrMagnitude >= moveThreshold * moveThreshold
                ? GetDirectionFromVelocity(velocity).ToString()
                : "Idle";
            string appliedDirection = animController != null
                ? animController.CurrentDirection.ToString()
                : "None";
            string externalFacing = _hasExternalFacingDirection
                ? GetDirectionFromVelocity(_externalFacingDirection).ToString()
                : "None";
            Debug.Log(
                $"<color=yellow>[NPCMotionController]</color> {name} => Velocity={velocity}, IsMoving={IsMoving}, " +
                $"ObservedDir={observedDirection}, AppliedDir={appliedDirection}, ExternalFacing={externalFacing}, " +
                $"MoveSpeed={moveSpeed:F2}, FacingSource={_lastFacingWriteSource}, ExternalVelocitySource={_lastExternalVelocitySource}, " +
                $"ExternalFacingSource={_lastExternalFacingSource}",
                this);
        }

        _lastPosition = transform.position;
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        moveThreshold = Mathf.Max(0.001f, moveThreshold);
        facingDirectionMinHoldSeconds = Mathf.Clamp(facingDirectionMinHoldSeconds, 0f, 0.4f);
        externalFacingMinHoldSeconds = Mathf.Clamp(externalFacingMinHoldSeconds, 0f, 0.4f);
        oppositeFacingConfirmSeconds = Mathf.Clamp(oppositeFacingConfirmSeconds, 0f, 0.4f);
        facingBoundaryHysteresisDegrees = Mathf.Clamp(facingBoundaryHysteresisDegrees, 0f, 25f);
        movingFacingAlignmentThreshold = Mathf.Clamp(movingFacingAlignmentThreshold, -1f, 1f);
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

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "外部线程如果只是想让 NPC 原地朝向某个方向，应优先走这个语义口，而不是自己拼低级 facing 写入。")]
    public void ApplyIdleFacing(
        Vector2 direction,
        [CallerMemberName] string callerMember = "",
        [CallerFilePath] string callerFile = "")
    {
        SetFacingDirection(direction, callerMember, callerFile);
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.ExternalFacade,
        Guidance = "剧情/导演/排练对 NPC 做受控位移时，应通过这个语义口同时写入速度和朝向，不要自行组合低级写口。")]
    public void ApplyDirectedMotion(
        Vector2 velocity,
        Vector2? facingDirection = null,
        [CallerMemberName] string callerMember = "",
        [CallerFilePath] string callerFile = "")
    {
        SetExternalVelocity(velocity, callerMember, callerFile);

        Vector2 effectiveFacing = facingDirection.HasValue && facingDirection.Value.sqrMagnitude > 0.0001f
            ? facingDirection.Value
            : velocity;
        if (effectiveFacing.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        SetExternalFacingDirection(effectiveFacing, callerMember, callerFile);
        SetFacingDirection(effectiveFacing, callerMember, callerFile);
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.RuntimeOnly,
        Guidance = "低级 facing 写口。外部协作线程应优先改用 ApplyIdleFacing 或更高层的 locomotion facade。")]
    public void SetFacingDirection(
        Vector2 direction,
        [CallerMemberName] string callerMember = "",
        [CallerFilePath] string callerFile = "")
    {
        if (animController == null || direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        _lastFacingWriteSource = BuildDebugSource(callerFile, callerMember);
        NPCAnimController.NPCAnimDirection facingDirection = GetDirectionFromVelocity(direction);
        if (TryGetAuthoritativeMovementVector(out Vector2 movementVector))
        {
            float alignment = Vector2.Dot(direction.normalized, movementVector.normalized);
            if (alignment < movingFacingAlignmentThreshold)
            {
                // While locomotion is active, ignore stray facing writes that diverge from the
                // actual movement. This keeps roam/story owners from making the NPC walk one way
                // while the sprite keeps snapping to another direction.
                return;
            }

            facingDirection = GetDirectionFromVelocity(movementVector);
        }

        animController.SetDirection(facingDirection, force: true);
        _lastSmoothedDirection = facingDirection;
        _lastFacingDirectionChangeTime = Time.time;
        _pendingFacingDirection = null;
        _pendingFacingDirectionSince = float.NegativeInfinity;
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.RuntimeOnly,
        Guidance = "低级速度写口。外部协作线程应优先改用 ApplyDirectedMotion。")]
    public void SetExternalVelocity(
        Vector2 velocity,
        [CallerMemberName] string callerMember = "",
        [CallerFilePath] string callerFile = "")
    {
        _lastExternalVelocitySource = BuildDebugSource(callerFile, callerMember);
        _externalVelocity = velocity;
        _hasExternalVelocity = true;

        if (!_hasExternalFacingDirection && velocity.sqrMagnitude > 0.0001f)
        {
            _externalFacingDirection = velocity.normalized;
            _hasExternalFacingDirection = true;
        }
    }

    [NpcLocomotionSurface(
        NpcLocomotionSurfaceScope.RuntimeOnly,
        Guidance = "低级外部朝向写口。外部协作线程应优先改用 ApplyDirectedMotion。")]
    public void SetExternalFacingDirection(
        Vector2 direction,
        [CallerMemberName] string callerMember = "",
        [CallerFilePath] string callerFile = "")
    {
        if (direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        _lastExternalFacingSource = BuildDebugSource(callerFile, callerMember);
        _externalFacingDirection = direction.normalized;
        _hasExternalFacingDirection = true;
    }

    public void ClearExternalVelocity()
    {
        _externalVelocity = Vector2.zero;
        _hasExternalVelocity = false;
        _externalFacingDirection = Vector2.zero;
        _hasExternalFacingDirection = false;
        ResetObservedFacingOverride();
    }

    public void StopMotion()
    {
        _externalVelocity = Vector2.zero;
        _hasExternalVelocity = false;
        _externalFacingDirection = Vector2.zero;
        _hasExternalFacingDirection = false;
        ResetObservedFacingOverride();
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
            _pendingFacingDirection = null;
            _pendingFacingDirectionSince = float.NegativeInfinity;
        }

        _lastPosition = transform.position;
        ResetFacingMismatchDiagnostics();
    }

    public void ResetMotionObservation(bool clearExternalVelocity = true)
    {
        if (clearExternalVelocity)
        {
            _externalVelocity = Vector2.zero;
            _hasExternalVelocity = false;
            _externalFacingDirection = Vector2.zero;
            _hasExternalFacingDirection = false;
            ResetObservedFacingOverride();
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

    private Vector2 ResolveFacingVelocity(Vector2 observedVelocity)
    {
        float minSpeedSqr = moveThreshold * moveThreshold;
        bool hasObservedVelocity = observedVelocity.sqrMagnitude >= minSpeedSqr;
        bool hasRigidbodyVelocity = useRigidbodyVelocity &&
            rb != null &&
            rb.linearVelocity.sqrMagnitude >= minSpeedSqr &&
            Time.time - _lastObservedMovementTime <= 0.12f;
        bool hasExternalVelocity = _hasExternalVelocity && _externalVelocity.sqrMagnitude >= minSpeedSqr;

        // 自漫游/避让的最终表现要跟真实位移走，不能继续被旧 intent 或外部朝向拖反。
        if (hasObservedVelocity)
        {
            ResetObservedFacingOverride();
            return observedVelocity;
        }

        if (hasRigidbodyVelocity)
        {
            ResetObservedFacingOverride();
            return rb.linearVelocity;
        }

        if (_hasExternalFacingDirection &&
            hasExternalVelocity)
        {
            ResetObservedFacingOverride();
            float facingSpeed = Mathf.Max(Mathf.Sqrt(minSpeedSqr), _externalVelocity.magnitude);
            return _externalFacingDirection * facingSpeed;
        }

        ResetObservedFacingOverride();

        if (hasExternalVelocity)
        {
            return _externalVelocity;
        }

        return observedVelocity;
    }

    private bool TryGetAuthoritativeMovementVector(out Vector2 movementVector)
    {
        float minSpeedSqr = moveThreshold * moveThreshold;
        if (CurrentVelocity.sqrMagnitude >= minSpeedSqr)
        {
            movementVector = CurrentVelocity;
            return true;
        }

        if (useRigidbodyVelocity &&
            rb != null &&
            rb.linearVelocity.sqrMagnitude >= minSpeedSqr &&
            Time.time - _lastObservedMovementTime <= 0.12f)
        {
            movementVector = rb.linearVelocity;
            return true;
        }

        if (_hasExternalVelocity && _externalVelocity.sqrMagnitude >= minSpeedSqr)
        {
            movementVector = _externalVelocity;
            return true;
        }

        movementVector = Vector2.zero;
        return false;
    }

    private bool ShouldPreferObservedFacing(Vector2 observedVelocity, float facingAlignment)
    {
        NPCAnimController.NPCAnimDirection observedDirection = GetDirectionFromVelocity(observedVelocity);
        NPCAnimController.NPCAnimDirection externalDirection = GetDirectionFromVelocity(_externalFacingDirection);

        if (observedDirection == externalDirection || facingAlignment >= movingFacingAlignmentThreshold)
        {
            ResetObservedFacingOverride();
            return false;
        }

        if (_pendingObservedFacingOverrideDirection != observedDirection)
        {
            _pendingObservedFacingOverrideDirection = observedDirection;
            _pendingObservedFacingOverrideSince = Time.time;
            return false;
        }

        float confirmSeconds = Mathf.Max(externalFacingMinHoldSeconds, oppositeFacingConfirmSeconds);
        if (Time.time - _pendingObservedFacingOverrideSince < confirmSeconds)
        {
            return false;
        }

        return true;
    }

    private void EmitFacingMismatchDiagnostics(
        Vector2 observedVelocity,
        Vector2 facingVelocity,
        NPCAnimController.NPCAnimDirection appliedDirection)
    {
#if UNITY_EDITOR
        if (!Application.isEditor || (!s_emitFacingMismatchDiagnosticsInEditor && !showDebugLog))
        {
            return;
        }

        float minSpeedSqr = moveThreshold * moveThreshold;
        if (observedVelocity.sqrMagnitude < minSpeedSqr)
        {
            ResetFacingMismatchDiagnostics();
            return;
        }

        NPCAnimController.NPCAnimDirection observedDirection = GetDirectionFromVelocity(observedVelocity);
        if (observedDirection == appliedDirection)
        {
            ResetFacingMismatchDiagnostics();
            return;
        }

        _facingMismatchFrameCount++;
        if (Time.time - _lastFacingMismatchSeenTime > FacingMismatchLogCooldownSeconds)
        {
            _facingMismatchBurstCount = 0;
        }

        _facingMismatchBurstCount++;
        _lastFacingMismatchSeenTime = Time.time;

        bool hasConsecutiveMismatch = _facingMismatchFrameCount >= FacingMismatchLogMinFrames;
        bool hasBurstMismatch = _facingMismatchBurstCount >= 2;
        if (!hasConsecutiveMismatch && !hasBurstMismatch)
        {
            return;
        }

        if (Time.time - _lastFacingMismatchLogTime < FacingMismatchLogCooldownSeconds)
        {
            return;
        }

        _lastFacingMismatchLogTime = Time.time;

        NPCAutoRoamController roamController = GetComponent<NPCAutoRoamController>();
        string roamSummary = roamController != null
            ? $" roamState={roamController.DebugState} moveSkip={roamController.DebugLastMoveSkipReason} blockedFrames={roamController.DebugBlockedAdvanceFrames}"
            : string.Empty;

        Debug.LogWarning(
            $"[NPCFacingMismatch] {name} observedDir={observedDirection} appliedDir={appliedDirection} " +
            $"observedVelocity={observedVelocity} facingVelocity={facingVelocity} externalVelocity={_externalVelocity} " +
            $"externalFacing={_externalFacingDirection} facingSource={_lastFacingWriteSource} " +
            $"externalVelocitySource={_lastExternalVelocitySource} externalFacingSource={_lastExternalFacingSource}{roamSummary}",
            this);
#endif
    }

    private void ResetFacingMismatchDiagnostics()
    {
#if UNITY_EDITOR
        _facingMismatchFrameCount = 0;
        _facingMismatchBurstCount = 0;
        _lastFacingMismatchSeenTime = float.NegativeInfinity;
#endif
    }

    private void ResetObservedFacingOverride()
    {
        _pendingObservedFacingOverrideDirection = null;
        _pendingObservedFacingOverrideSince = float.NegativeInfinity;
    }

    private static string BuildDebugSource(string callerFile, string callerMember)
    {
        string safeMember = string.IsNullOrWhiteSpace(callerMember) ? "unknown" : callerMember.Trim();
        string fileStem = string.IsNullOrWhiteSpace(callerFile)
            ? string.Empty
            : Path.GetFileNameWithoutExtension(callerFile);

        return string.IsNullOrWhiteSpace(fileStem)
            ? safeMember
            : $"{fileStem}.{safeMember}";
    }

    private NPCAnimController.NPCAnimDirection GetDirectionFromVelocity(Vector2 velocity)
    {
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        if (angle >= -45f && angle < 45f)
        {
            return NPCAnimController.NPCAnimDirection.Right;
        }

        if (angle >= 45f && angle < 135f)
        {
            return NPCAnimController.NPCAnimDirection.Up;
        }

        if (angle >= -135f && angle < -45f)
        {
            return NPCAnimController.NPCAnimDirection.Down;
        }

        return NPCAnimController.NPCAnimDirection.Left;
    }

    private NPCAnimController.NPCAnimDirection ApplyFacingSectorHysteresis(
        Vector2 velocity,
        NPCAnimController.NPCAnimDirection rawDirection)
    {
        if (rawDirection == _lastSmoothedDirection ||
            facingBoundaryHysteresisDegrees <= 0f ||
            velocity.sqrMagnitude < moveThreshold * moveThreshold ||
            IsOppositeFacingDirection(_lastSmoothedDirection, rawDirection))
        {
            return rawDirection;
        }

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        return IsAngleWithinFacingHoldRange(angle, _lastSmoothedDirection, facingBoundaryHysteresisDegrees)
            ? _lastSmoothedDirection
            : rawDirection;
    }

    private static bool IsAngleWithinFacingHoldRange(
        float angle,
        NPCAnimController.NPCAnimDirection direction,
        float hysteresisDegrees)
    {
        float centerAngle = GetFacingDirectionCenterAngle(direction);
        float delta = Mathf.DeltaAngle(centerAngle, angle);
        return Mathf.Abs(delta) <= 45f + hysteresisDegrees;
    }

    private static float GetFacingDirectionCenterAngle(NPCAnimController.NPCAnimDirection direction)
    {
        switch (direction)
        {
            case NPCAnimController.NPCAnimDirection.Right:
                return 0f;
            case NPCAnimController.NPCAnimDirection.Up:
                return 90f;
            case NPCAnimController.NPCAnimDirection.Left:
                return 180f;
            default:
                return -90f;
        }
    }

    private NPCAnimController.NPCAnimDirection GetSmoothedFacingDirection(
        NPCAnimController.NPCAnimDirection rawDirection)
    {
        if (ShouldBypassFacingHoldForMovement(rawDirection))
        {
            return CommitFacingDirection(rawDirection);
        }

        if (rawDirection == _lastSmoothedDirection)
        {
            _pendingFacingDirection = null;
            _pendingFacingDirectionSince = float.NegativeInfinity;
            return rawDirection;
        }

        float minHoldSeconds = _hasExternalFacingDirection
            ? Mathf.Max(facingDirectionMinHoldSeconds, externalFacingMinHoldSeconds)
            : facingDirectionMinHoldSeconds;

        if (Time.time - _lastFacingDirectionChangeTime < minHoldSeconds)
        {
            return _lastSmoothedDirection;
        }

        if (IsOppositeFacingDirection(_lastSmoothedDirection, rawDirection))
        {
            if (_pendingFacingDirection != rawDirection)
            {
                _pendingFacingDirection = rawDirection;
                _pendingFacingDirectionSince = Time.time;
                return _lastSmoothedDirection;
            }

            float confirmSeconds = Mathf.Max(minHoldSeconds, oppositeFacingConfirmSeconds);
            if (Time.time - _pendingFacingDirectionSince < confirmSeconds)
            {
                return _lastSmoothedDirection;
            }
        }
        else
        {
            _pendingFacingDirection = null;
            _pendingFacingDirectionSince = float.NegativeInfinity;
        }

        return CommitFacingDirection(rawDirection);
    }

    private bool ShouldBypassFacingHoldForMovement(NPCAnimController.NPCAnimDirection rawDirection)
    {
        if (!TryGetAuthoritativeMovementVector(out Vector2 movementVector))
        {
            return false;
        }

        float minSpeedSqr = moveThreshold * moveThreshold;
        if (movementVector.sqrMagnitude < minSpeedSqr)
        {
            return false;
        }

        return GetDirectionFromVelocity(movementVector) == rawDirection;
    }

    private NPCAnimController.NPCAnimDirection CommitFacingDirection(NPCAnimController.NPCAnimDirection direction)
    {
        _lastSmoothedDirection = direction;
        _lastFacingDirectionChangeTime = Time.time;
        _pendingFacingDirection = null;
        _pendingFacingDirectionSince = float.NegativeInfinity;
        return direction;
    }

    private static bool IsOppositeFacingDirection(
        NPCAnimController.NPCAnimDirection current,
        NPCAnimController.NPCAnimDirection candidate)
    {
        return (current == NPCAnimController.NPCAnimDirection.Left && candidate == NPCAnimController.NPCAnimDirection.Right) ||
               (current == NPCAnimController.NPCAnimDirection.Right && candidate == NPCAnimController.NPCAnimDirection.Left) ||
               (current == NPCAnimController.NPCAnimDirection.Up && candidate == NPCAnimController.NPCAnimDirection.Down) ||
               (current == NPCAnimController.NPCAnimDirection.Down && candidate == NPCAnimController.NPCAnimDirection.Up);
    }

    #endregion
}
