using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC 自动漫游控制器。
/// 负责随机选点、路径跟随、短停/长停节奏与长停气泡触发。
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(NPCMotionController))]
public class NPCAutoRoamController : MonoBehaviour
{
    #region 枚举

    private enum RoamState
    {
        Inactive = 0,
        ShortPause = 1,
        Moving = 2,
        LongPause = 3
    }

    #endregion

    #region 序列化字段

    [Header("组件引用")]
    [SerializeField] private NPCMotionController motionController;
    [SerializeField] private NPCBubblePresenter bubblePresenter;
    [SerializeField] private NavGrid2D navGrid;
    [SerializeField] private Transform homeAnchor;

    [Header("基础开关")]
    [SerializeField] private bool autoStart = true;

    [Header("活动范围")]
    [SerializeField] private float activityRadius = 3f;
    [SerializeField] private float minimumMoveDistance = 0.6f;
    [SerializeField] private int pathSampleAttempts = 12;

    [Header("短暂停留")]
    [SerializeField] private float shortPauseMin = 0.5f;
    [SerializeField] private float shortPauseMax = 3f;
    [SerializeField] private int shortPauseCountMin = 3;
    [SerializeField] private int shortPauseCountMax = 5;

    [Header("长停留")]
    [SerializeField] private float longPauseMin = 3f;
    [SerializeField] private float longPauseMax = 6f;

    [Header("路径跟随")]
    [SerializeField] private float waypointTolerance = 0.08f;
    [SerializeField] private float destinationTolerance = 0.12f;

    [Header("调试")]
    [SerializeField] private bool showDebugLog = false;
    [SerializeField] private bool drawDebugPath = true;

    #endregion

    #region 私有字段

    private readonly List<Vector2> _path = new List<Vector2>();

    private RoamState _state = RoamState.Inactive;
    private Vector2 _homePosition;
    private Vector2 _currentDestination;
    private float _stateTimer;
    private int _currentPathIndex;
    private int _completedShortPauseCount;
    private int _longPauseTriggerCount;
    private bool _warnedMissingNavGrid;

    #endregion

    #region 公共属性

    public bool IsRoaming => _state != RoamState.Inactive;
    public bool IsMoving => _state == RoamState.Moving;

    #endregion

    #region Unity生命周期

    private void Reset()
    {
        CacheComponents();
    }

    private void Awake()
    {
        CacheComponents();
        _homePosition = transform.position;
    }

    private void Start()
    {
        ResetLongPauseTriggerCount();

        if (autoStart)
        {
            StartRoam();
        }
        else if (motionController != null)
        {
            motionController.StopMotion();
        }
    }

    private void Update()
    {
        switch (_state)
        {
            case RoamState.ShortPause:
                TickShortPause();
                break;

            case RoamState.Moving:
                TickMoving();
                break;

            case RoamState.LongPause:
                TickLongPause();
                break;
        }
    }

    private void OnDisable()
    {
        StopRoam();
    }

    private void OnValidate()
    {
        activityRadius = Mathf.Max(0.5f, activityRadius);
        minimumMoveDistance = Mathf.Clamp(minimumMoveDistance, 0.1f, activityRadius);
        pathSampleAttempts = Mathf.Max(1, pathSampleAttempts);

        shortPauseMin = Mathf.Max(0.05f, shortPauseMin);
        shortPauseMax = Mathf.Max(shortPauseMin, shortPauseMax);
        shortPauseCountMin = Mathf.Max(1, shortPauseCountMin);
        shortPauseCountMax = Mathf.Max(shortPauseCountMin, shortPauseCountMax);

        longPauseMin = Mathf.Max(0.1f, longPauseMin);
        longPauseMax = Mathf.Max(longPauseMin, longPauseMax);

        waypointTolerance = Mathf.Max(0.01f, waypointTolerance);
        destinationTolerance = Mathf.Max(waypointTolerance, destinationTolerance);

        if (!Application.isPlaying)
        {
            CacheComponents();
        }
    }

    #endregion

    #region 公共方法

    public void StartRoam()
    {
        CacheComponents();

        _homePosition = transform.position;
        _warnedMissingNavGrid = false;
        _completedShortPauseCount = 0;
        ResetLongPauseTriggerCount();
        EnterShortPause(false);
    }

    public void StopRoam()
    {
        _state = RoamState.Inactive;
        _stateTimer = 0f;
        _currentPathIndex = 0;
        _path.Clear();

        if (motionController != null)
        {
            motionController.StopMotion();
        }

        if (bubblePresenter != null)
        {
            bubblePresenter.HideBubble();
        }
    }

    [ContextMenu("调试/立即开始漫游")]
    public void DebugStartRoam()
    {
        if (Application.isPlaying)
        {
            StartRoam();
        }
    }

    [ContextMenu("调试/停止漫游")]
    public void DebugStopRoam()
    {
        if (Application.isPlaying)
        {
            StopRoam();
        }
    }

    [ContextMenu("调试/立即进入长停")]
    public void DebugEnterLongPause()
    {
        if (Application.isPlaying)
        {
            EnterLongPause();
        }
    }

    #endregion

    #region 私有方法

    private void CacheComponents()
    {
        if (motionController == null)
        {
            motionController = GetComponent<NPCMotionController>();
        }

        if (bubblePresenter == null)
        {
            bubblePresenter = GetComponent<NPCBubblePresenter>();
        }

        if (navGrid == null)
        {
            navGrid = FindFirstObjectByType<NavGrid2D>();
        }
    }

    private void TickShortPause()
    {
        _stateTimer -= Time.deltaTime;
        if (_stateTimer > 0f)
        {
            return;
        }

        if (_completedShortPauseCount >= _longPauseTriggerCount)
        {
            EnterLongPause();
            return;
        }

        if (!TryBeginMove())
        {
            EnterShortPause(false);
        }
    }

    private void TickMoving()
    {
        if (motionController == null)
        {
            return;
        }

        if (_path.Count == 0)
        {
            EnterShortPause(true);
            return;
        }

        Vector2 currentPosition = transform.position;

        while (_currentPathIndex < _path.Count &&
               Vector2.Distance(currentPosition, _path[_currentPathIndex]) <= waypointTolerance)
        {
            _currentPathIndex++;
        }

        if (_currentPathIndex >= _path.Count ||
            Vector2.Distance(currentPosition, _currentDestination) <= destinationTolerance)
        {
            EnterShortPause(true);
            return;
        }

        Vector2 waypoint = _path[_currentPathIndex];
        Vector2 toWaypoint = waypoint - currentPosition;
        float distance = toWaypoint.magnitude;
        float moveSpeed = Mathf.Max(0f, motionController.MoveSpeed);
        float step = moveSpeed * Time.deltaTime;

        if (distance <= 0.0001f || step <= 0f)
        {
            motionController.StopMotion();
            return;
        }

        Vector2 nextPosition = distance <= step
            ? waypoint
            : currentPosition + (toWaypoint / distance) * step;

        float deltaTime = Mathf.Max(Time.deltaTime, 0.0001f);
        Vector2 velocity = (nextPosition - currentPosition) / deltaTime;

        motionController.SetExternalVelocity(velocity);
        transform.position = new Vector3(nextPosition.x, nextPosition.y, transform.position.z);
    }

    private void TickLongPause()
    {
        _stateTimer -= Time.deltaTime;
        if (_stateTimer > 0f)
        {
            return;
        }

        _completedShortPauseCount = 0;
        ResetLongPauseTriggerCount();

        if (bubblePresenter != null)
        {
            bubblePresenter.HideBubble();
        }

        if (!TryBeginMove())
        {
            EnterShortPause(false);
        }
    }

    private bool TryBeginMove()
    {
        if (!TryResolveNavGrid())
        {
            return false;
        }

        _path.Clear();
        _currentPathIndex = 0;

        Vector2 currentPosition = transform.position;
        Vector2 roamCenter = GetRoamCenter();

        for (int attempt = 0; attempt < pathSampleAttempts; attempt++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * activityRadius;
            Vector2 sampledDestination = roamCenter + randomOffset;

            if (Vector2.Distance(currentPosition, sampledDestination) < minimumMoveDistance)
            {
                continue;
            }

            Vector2 walkableDestination = sampledDestination;
            if (!navGrid.IsWalkable(sampledDestination) &&
                !navGrid.TryFindNearestWalkable(sampledDestination, out walkableDestination))
            {
                continue;
            }

            if (Vector2.Distance(currentPosition, walkableDestination) < minimumMoveDistance)
            {
                continue;
            }

            if (!navGrid.TryFindPath(currentPosition, walkableDestination, _path) || _path.Count == 0)
            {
                _path.Clear();
                continue;
            }

            _currentDestination = walkableDestination;
            _state = RoamState.Moving;
            _stateTimer = 0f;

            if (bubblePresenter != null)
            {
                bubblePresenter.HideBubble();
            }

            if (showDebugLog)
            {
                Debug.Log(
                    $"<color=green>[NPCAutoRoamController]</color> {name} 开始移动 => Destination={_currentDestination}, PathCount={_path.Count}",
                    this);
            }

            return true;
        }

        if (showDebugLog)
        {
            Debug.LogWarning($"[NPCAutoRoamController] {name} 在 {pathSampleAttempts} 次采样内没有找到可用路径。", this);
        }

        return false;
    }

    private bool TryResolveNavGrid()
    {
        if (navGrid != null)
        {
            return true;
        }

        navGrid = FindFirstObjectByType<NavGrid2D>();
        if (navGrid != null)
        {
            _warnedMissingNavGrid = false;
            return true;
        }

        if (!_warnedMissingNavGrid)
        {
            _warnedMissingNavGrid = true;
            Debug.LogWarning($"[NPCAutoRoamController] {name} 未找到 NavGrid2D，自动漫游暂时不会启动。", this);
        }

        return false;
    }

    private void EnterShortPause(bool countTowardLongPause)
    {
        _state = RoamState.ShortPause;
        _stateTimer = Random.Range(shortPauseMin, shortPauseMax);
        _path.Clear();
        _currentPathIndex = 0;

        if (countTowardLongPause)
        {
            _completedShortPauseCount++;
        }

        if (motionController != null)
        {
            motionController.StopMotion();
        }

        if (bubblePresenter != null)
        {
            bubblePresenter.HideBubble();
        }

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=yellow>[NPCAutoRoamController]</color> {name} 进入短停 => Timer={_stateTimer:F2}s, Count={_completedShortPauseCount}/{_longPauseTriggerCount}",
                this);
        }
    }

    private void EnterLongPause()
    {
        _state = RoamState.LongPause;
        _stateTimer = Random.Range(longPauseMin, longPauseMax);
        _path.Clear();
        _currentPathIndex = 0;

        if (motionController != null)
        {
            motionController.StopMotion();
        }

        if (bubblePresenter != null)
        {
            bubblePresenter.ShowRandomSelfTalk(Mathf.Max(1f, _stateTimer - 0.15f));
        }

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=cyan>[NPCAutoRoamController]</color> {name} 进入长停 => Timer={_stateTimer:F2}s",
                this);
        }
    }

    private Vector2 GetRoamCenter()
    {
        if (homeAnchor != null)
        {
            return homeAnchor.position;
        }

        return _homePosition;
    }

    private void ResetLongPauseTriggerCount()
    {
        _longPauseTriggerCount = Random.Range(shortPauseCountMin, shortPauseCountMax + 1);
    }

    #endregion

    #region 编辑器方法

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector2 roamCenter = homeAnchor != null ? (Vector2)homeAnchor.position : (Vector2)transform.position;

        Gizmos.color = new Color(0f, 0.8f, 1f, 0.35f);
        Gizmos.DrawWireSphere(roamCenter, activityRadius);

        if (!drawDebugPath || _path.Count == 0)
        {
            return;
        }

        Gizmos.color = Color.green;
        Vector3 previous = transform.position;
        for (int index = 0; index < _path.Count; index++)
        {
            Vector3 point = new Vector3(_path[index].x, _path[index].y, transform.position.z);
            Gizmos.DrawLine(previous, point);
            Gizmos.DrawSphere(point, 0.05f);
            previous = point;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(_currentDestination.x, _currentDestination.y, transform.position.z), destinationTolerance);
    }
#endif

    #endregion
}
