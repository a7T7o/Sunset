using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC 自动漫游控制器。
/// 负责随机选点、路径跟随、短停/长停节奏，以及长停时的自言自语或附近聊天。
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(NPCMotionController))]
public class NPCAutoRoamController : MonoBehaviour
{
    private enum RoamState
    {
        Inactive = 0,
        ShortPause = 1,
        Moving = 2,
        LongPause = 3
    }

    [Header("组件引用")]
    [SerializeField] private NPCMotionController motionController;
    [SerializeField] private NPCBubblePresenter bubblePresenter;
    [SerializeField] private NavGrid2D navGrid;
    [SerializeField] private Transform homeAnchor;
    [SerializeField] private NPCRoamProfile roamProfile;

    [Header("基础开关")]
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool applyProfileOnAwake = true;

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

    [Header("环境聊天")]
    [SerializeField] private bool enableAmbientChat = true;
    [SerializeField] private float ambientChatRadius = 2.8f;
    [SerializeField, Range(0f, 1f)] private float ambientChatChance = 0.75f;
    [SerializeField] private float ambientChatResponseDelay = 0.85f;
    [SerializeField] private string[] chatInitiatorLines =
    {
        "今天这边挺安静的。",
        "你也在这边歇一会儿啊。",
        "这附近风吹着还挺舒服。",
        "等会儿再继续忙吧。"
    };
    [SerializeField] private string[] chatResponderLines =
    {
        "是啊，先缓一缓。",
        "我也刚走到这儿。",
        "这边待着还挺舒服的。",
        "好，等下再去转转。"
    };

    [Header("路径跟随")]
    [SerializeField] private float waypointTolerance = 0.08f;
    [SerializeField] private float destinationTolerance = 0.12f;

    [Header("卡住恢复")]
    [SerializeField] private float stuckCheckInterval = 0.3f;
    [SerializeField] private float stuckDistanceThreshold = 0.08f;
    [SerializeField] private int maxStuckRecoveries = 3;

    [Header("调试")]
    [SerializeField] private bool showDebugLog = false;
    [SerializeField] private bool drawDebugPath = true;

    private readonly List<Vector2> path = new List<Vector2>();

    private RoamState state = RoamState.Inactive;
    private Vector2 homePosition;
    private Vector2 currentDestination;
    private float stateTimer;
    private int currentPathIndex;
    private int completedShortPauseCount;
    private int longPauseTriggerCount;
    private bool warnedMissingNavGrid;
    private Coroutine ambientChatCoroutine;
    private NPCAutoRoamController chatPartner;
    private string lastAmbientDecision = string.Empty;
    private Vector2 lastProgressCheckPosition;
    private float lastProgressCheckTime;
    private float lastProgressDistance;
    private int currentStuckRecoveryCount;

    public bool IsRoaming => state != RoamState.Inactive;
    public bool IsMoving => state == RoamState.Moving;
    public bool IsInAmbientChat => chatPartner != null;
    public string ChatPartnerName => chatPartner != null ? chatPartner.name : string.Empty;
    public string LastAmbientDecision => lastAmbientDecision;
    public string DebugState => state.ToString();
    public float DebugStateTimer => stateTimer;
    public int CompletedShortPauseCount => completedShortPauseCount;
    public int LongPauseTriggerCount => longPauseTriggerCount;
    public int CurrentStuckRecoveryCount => currentStuckRecoveryCount;
    public float DebugLastProgressDistance => lastProgressDistance;
    public NPCRoamProfile RoamProfile => roamProfile;

    private void Reset()
    {
        CacheComponents();
    }

    private void Awake()
    {
        CacheComponents();

        if (applyProfileOnAwake)
        {
            ApplyProfile();
        }

        homePosition = transform.position;
    }

    private void Start()
    {
        ResetLongPauseTriggerCount();
        ResetMovementRecovery(transform.position, resetCounter: true);

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
        switch (state)
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

        ambientChatRadius = Mathf.Max(0f, ambientChatRadius);
        ambientChatResponseDelay = Mathf.Max(0f, ambientChatResponseDelay);

        waypointTolerance = Mathf.Max(0.01f, waypointTolerance);
        destinationTolerance = Mathf.Max(waypointTolerance, destinationTolerance);

        stuckCheckInterval = Mathf.Max(0.1f, stuckCheckInterval);
        stuckDistanceThreshold = Mathf.Max(0.01f, stuckDistanceThreshold);
        maxStuckRecoveries = Mathf.Max(1, maxStuckRecoveries);

        if (!Application.isPlaying)
        {
            CacheComponents();

            if (applyProfileOnAwake)
            {
                ApplyProfile();
            }
        }
    }

    public void StartRoam()
    {
        CacheComponents();

        homePosition = transform.position;
        warnedMissingNavGrid = false;
        completedShortPauseCount = 0;
        lastAmbientDecision = string.Empty;
        ResetLongPauseTriggerCount();
        ResetMovementRecovery(transform.position, resetCounter: true);
        EnterShortPause(false);
    }

    public void StopRoam()
    {
        BreakAmbientChatLink(hideBubble: true);
        state = RoamState.Inactive;
        stateTimer = 0f;
        currentPathIndex = 0;
        path.Clear();
        ResetMovementRecovery(transform.position, resetCounter: true);

        if (motionController != null)
        {
            motionController.StopMotion();
        }

        if (bubblePresenter != null)
        {
            bubblePresenter.HideBubble();
        }
    }

    public void ApplyProfile()
    {
        if (roamProfile == null)
        {
            return;
        }

        CacheComponents();

        activityRadius = roamProfile.ActivityRadius;
        minimumMoveDistance = roamProfile.MinimumMoveDistance;
        pathSampleAttempts = roamProfile.PathSampleAttempts;

        shortPauseMin = roamProfile.ShortPauseMin;
        shortPauseMax = roamProfile.ShortPauseMax;
        shortPauseCountMin = roamProfile.ShortPauseCountMin;
        shortPauseCountMax = roamProfile.ShortPauseCountMax;

        longPauseMin = roamProfile.LongPauseMin;
        longPauseMax = roamProfile.LongPauseMax;

        stuckCheckInterval = roamProfile.StuckCheckInterval;
        stuckDistanceThreshold = roamProfile.StuckDistanceThreshold;
        maxStuckRecoveries = roamProfile.MaxStuckRecoveries;

        enableAmbientChat = roamProfile.EnableAmbientChat;
        ambientChatRadius = roamProfile.AmbientChatRadius;
        ambientChatChance = roamProfile.AmbientChatChance;
        ambientChatResponseDelay = roamProfile.AmbientChatResponseDelay;

        chatInitiatorLines = CopyLines(roamProfile.ChatInitiatorLines);
        chatResponderLines = CopyLines(roamProfile.ChatResponderLines);

        if (motionController != null)
        {
            motionController.MoveSpeed = roamProfile.MoveSpeed;
        }

        NPCAnimController animController = GetComponent<NPCAnimController>();
        if (animController != null)
        {
            animController.SetAnimationSpeed(roamProfile.IdleAnimationSpeed, roamProfile.MoveAnimationSpeed);
        }

        if (bubblePresenter != null)
        {
            bubblePresenter.ApplyProfile(roamProfile);
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

    [ContextMenu("调试/尝试附近聊天")]
    public void DebugTryAmbientChat()
    {
        if (Application.isPlaying)
        {
            TryStartAmbientChat(Random.Range(longPauseMin, longPauseMax));
        }
    }

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
        stateTimer -= Time.deltaTime;
        if (stateTimer > 0f)
        {
            return;
        }

        if (completedShortPauseCount >= longPauseTriggerCount)
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

        if (path.Count == 0)
        {
            EnterShortPause(true);
            return;
        }

        Vector2 currentPosition = transform.position;

        while (currentPathIndex < path.Count &&
               Vector2.Distance(currentPosition, path[currentPathIndex]) <= waypointTolerance)
        {
            currentPathIndex++;
        }

        if (currentPathIndex >= path.Count ||
            Vector2.Distance(currentPosition, currentDestination) <= destinationTolerance)
        {
            EnterShortPause(true);
            return;
        }

        if (CheckAndHandleStuck(currentPosition))
        {
            return;
        }

        Vector2 waypoint = path[currentPathIndex];
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
        stateTimer -= Time.deltaTime;
        if (stateTimer > 0f)
        {
            return;
        }

        BreakAmbientChatLink(hideBubble: true);
        completedShortPauseCount = 0;
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

        path.Clear();
        currentPathIndex = 0;

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

            if (!navGrid.TryFindPath(currentPosition, walkableDestination, path) || path.Count == 0)
            {
                path.Clear();
                continue;
            }

            currentDestination = walkableDestination;
            state = RoamState.Moving;
            stateTimer = 0f;
            ResetMovementRecovery(currentPosition, resetCounter: true);

            if (bubblePresenter != null)
            {
                bubblePresenter.HideBubble();
            }

            if (showDebugLog)
            {
                Debug.Log(
                    $"<color=green>[NPCAutoRoamController]</color> {name} 开始移动 => Destination={currentDestination}, PathCount={path.Count}",
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

    private bool CheckAndHandleStuck(Vector2 currentPosition)
    {
        if (stuckCheckInterval <= 0f || Time.time - lastProgressCheckTime < stuckCheckInterval)
        {
            return false;
        }

        lastProgressDistance = Vector2.Distance(currentPosition, lastProgressCheckPosition);
        lastProgressCheckPosition = currentPosition;
        lastProgressCheckTime = Time.time;

        if (lastProgressDistance >= stuckDistanceThreshold)
        {
            currentStuckRecoveryCount = 0;
            return false;
        }

        currentStuckRecoveryCount++;

        if (showDebugLog)
        {
            Debug.LogWarning(
                $"[NPCAutoRoamController] {name} 检测到卡住 ({currentStuckRecoveryCount}/{maxStuckRecoveries})，最近位移 {lastProgressDistance:F3}m",
                this);
        }

        if (currentStuckRecoveryCount >= maxStuckRecoveries)
        {
            EnterShortPause(false);
            return true;
        }

        if (TryRebuildPath(currentPosition))
        {
            return false;
        }

        if (TryBeginMove())
        {
            return true;
        }

        EnterShortPause(false);
        return true;
    }

    private bool TryRebuildPath(Vector2 currentPosition)
    {
        if (!TryResolveNavGrid())
        {
            return false;
        }

        path.Clear();
        currentPathIndex = 0;

        if (!navGrid.TryFindPath(currentPosition, currentDestination, path) || path.Count == 0)
        {
            path.Clear();
            return false;
        }

        ResetMovementRecovery(currentPosition, resetCounter: false);

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=yellow>[NPCAutoRoamController]</color> {name} 卡住后重建路径 => Destination={currentDestination}, PathCount={path.Count}",
                this);
        }

        return true;
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
            warnedMissingNavGrid = false;
            return true;
        }

        if (!warnedMissingNavGrid)
        {
            warnedMissingNavGrid = true;
            Debug.LogWarning($"[NPCAutoRoamController] {name} 未找到 NavGrid2D，自动漫游暂时不会启动。", this);
        }

        return false;
    }

    private void EnterShortPause(bool countTowardLongPause)
    {
        BreakAmbientChatLink(hideBubble: true);
        state = RoamState.ShortPause;
        stateTimer = Random.Range(shortPauseMin, shortPauseMax);
        path.Clear();
        currentPathIndex = 0;
        ResetMovementRecovery(transform.position, resetCounter: true);

        if (countTowardLongPause)
        {
            completedShortPauseCount++;
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
                $"<color=yellow>[NPCAutoRoamController]</color> {name} 进入短停 => Timer={stateTimer:F2}s, Count={completedShortPauseCount}/{longPauseTriggerCount}",
                this);
        }
    }

    private void EnterLongPause()
    {
        BreakAmbientChatLink(hideBubble: true);
        state = RoamState.LongPause;
        stateTimer = Random.Range(longPauseMin, longPauseMax);
        path.Clear();
        currentPathIndex = 0;
        ResetMovementRecovery(transform.position, resetCounter: true);

        if (motionController != null)
        {
            motionController.StopMotion();
        }

        if (!TryStartAmbientChat(stateTimer) && bubblePresenter != null)
        {
            bubblePresenter.ShowRandomSelfTalk(Mathf.Max(1f, stateTimer - 0.15f));
        }

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=cyan>[NPCAutoRoamController]</color> {name} 进入长停 => Timer={stateTimer:F2}s",
                this);
        }
    }

    private Vector2 GetRoamCenter()
    {
        return homeAnchor != null ? (Vector2)homeAnchor.position : homePosition;
    }

    private bool TryStartAmbientChat(float duration)
    {
        if (!enableAmbientChat)
        {
            lastAmbientDecision = "ambient chat disabled";
            return false;
        }

        if (bubblePresenter == null)
        {
            lastAmbientDecision = "bubble presenter missing";
            return false;
        }

        if (!HasBubbleLines(chatInitiatorLines))
        {
            lastAmbientDecision = "initiator lines missing";
            return false;
        }

        if (ambientChatRadius <= 0f)
        {
            lastAmbientDecision = "ambient radius <= 0";
            return false;
        }

        if (Random.value > ambientChatChance)
        {
            lastAmbientDecision = "ambient chat roll missed";
            return false;
        }

        NPCAutoRoamController partner = FindAmbientChatPartner();
        if (partner == null || !partner.TryJoinAmbientChat(this, duration))
        {
            lastAmbientDecision = partner == null
                ? "no nearby partner"
                : $"partner rejected: {partner.name}";
            return false;
        }

        chatPartner = partner;
        lastAmbientDecision = $"chatting with {partner.name}";
        FaceToward(partner.transform.position - transform.position);
        partner.FaceToward(transform.position - partner.transform.position);

        StartAmbientChatRoutine(chatInitiatorLines, delay: 0f, duration);
        partner.StartAmbientChatRoutine(partner.chatResponderLines, ambientChatResponseDelay, duration);

        if (showDebugLog)
        {
            Debug.Log(
                $"<color=magenta>[NPCAutoRoamController]</color> {name} 与 {partner.name} 开始聊天 => Duration={duration:F2}s",
                this);
        }

        return true;
    }

    private NPCAutoRoamController FindAmbientChatPartner()
    {
        float bestDistanceSqr = ambientChatRadius * ambientChatRadius;
        NPCAutoRoamController bestPartner = null;
        NPCAutoRoamController[] controllers = FindObjectsByType<NPCAutoRoamController>(FindObjectsSortMode.None);
        for (int index = 0; index < controllers.Length; index++)
        {
            NPCAutoRoamController candidate = controllers[index];
            if (!CanUseAsAmbientChatPartner(candidate))
            {
                continue;
            }

            float distanceSqr = (candidate.transform.position - transform.position).sqrMagnitude;
            if (distanceSqr > bestDistanceSqr)
            {
                continue;
            }

            bestDistanceSqr = distanceSqr;
            bestPartner = candidate;
        }

        return bestPartner;
    }

    private bool CanUseAsAmbientChatPartner(NPCAutoRoamController candidate)
    {
        return candidate != null && candidate.CanJoinAmbientChat(this);
    }

    private bool CanJoinAmbientChat(NPCAutoRoamController requester)
    {
        if (requester == null || requester == this)
        {
            return false;
        }

        if (!isActiveAndEnabled || !gameObject.activeInHierarchy)
        {
            return false;
        }

        if (!enableAmbientChat ||
            bubblePresenter == null ||
            !HasBubbleLines(chatResponderLines) ||
            chatPartner != null)
        {
            return false;
        }

        return state is RoamState.ShortPause or RoamState.LongPause;
    }

    private bool TryJoinAmbientChat(NPCAutoRoamController requester, float duration)
    {
        if (!CanJoinAmbientChat(requester))
        {
            return false;
        }

        BreakAmbientChatLink(hideBubble: true);
        chatPartner = requester;
        lastAmbientDecision = $"joined chat with {requester.name}";
        state = RoamState.LongPause;
        stateTimer = Mathf.Max(duration, ambientChatResponseDelay + 1f);
        path.Clear();
        currentPathIndex = 0;
        ResetMovementRecovery(transform.position, resetCounter: true);

        if (motionController != null)
        {
            motionController.StopMotion();
        }

        return true;
    }

    private void StartAmbientChatRoutine(string[] lines, float delay, float duration)
    {
        StopAmbientChatRoutine();
        if (bubblePresenter == null || !HasBubbleLines(lines))
        {
            return;
        }

        ambientChatCoroutine = StartCoroutine(PlayAmbientChatBubble(lines, delay, duration));
    }

    private void StopAmbientChatRoutine()
    {
        if (ambientChatCoroutine == null)
        {
            return;
        }

        StopCoroutine(ambientChatCoroutine);
        ambientChatCoroutine = null;
    }

    private IEnumerator PlayAmbientChatBubble(string[] lines, float delay, float duration)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        ambientChatCoroutine = null;
        if (!isActiveAndEnabled || bubblePresenter == null || !HasBubbleLines(lines))
        {
            yield break;
        }

        float bubbleDuration = Mathf.Clamp(duration * 0.45f, 1.2f, 2.4f);
        bubblePresenter.ShowText(lines[Random.Range(0, lines.Length)], bubbleDuration);
    }

    private void BreakAmbientChatLink(bool hideBubble)
    {
        StopAmbientChatRoutine();

        if (hideBubble && bubblePresenter != null)
        {
            bubblePresenter.HideBubble();
        }

        NPCAutoRoamController partner = chatPartner;
        chatPartner = null;

        if (partner != null && partner.chatPartner == this)
        {
            partner.chatPartner = null;
            partner.StopAmbientChatRoutine();
            if (hideBubble && partner.bubblePresenter != null)
            {
                partner.bubblePresenter.HideBubble();
            }
        }
    }

    private void FaceToward(Vector2 direction)
    {
        if (motionController == null || direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        motionController.SetFacingDirection(direction);
    }

    private static bool HasBubbleLines(string[] lines)
    {
        if (lines == null)
        {
            return false;
        }

        for (int index = 0; index < lines.Length; index++)
        {
            if (!string.IsNullOrWhiteSpace(lines[index]))
            {
                return true;
            }
        }

        return false;
    }

    private void ResetLongPauseTriggerCount()
    {
        longPauseTriggerCount = Random.Range(shortPauseCountMin, shortPauseCountMax + 1);
    }

    private void ResetMovementRecovery(Vector2 currentPosition, bool resetCounter)
    {
        lastProgressCheckPosition = currentPosition;
        lastProgressCheckTime = Time.time;
        lastProgressDistance = 0f;

        if (resetCounter)
        {
            currentStuckRecoveryCount = 0;
        }
    }

    private static string[] CopyLines(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            return System.Array.Empty<string>();
        }

        string[] copy = new string[lines.Length];
        for (int index = 0; index < lines.Length; index++)
        {
            copy[index] = lines[index];
        }

        return copy;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector2 roamCenter = homeAnchor != null ? (Vector2)homeAnchor.position : (Vector2)transform.position;

        Gizmos.color = new Color(0f, 0.8f, 1f, 0.35f);
        Gizmos.DrawWireSphere(roamCenter, activityRadius);

        if (!drawDebugPath || path.Count == 0)
        {
            return;
        }

        Gizmos.color = Color.green;
        Vector3 previous = transform.position;
        for (int index = 0; index < path.Count; index++)
        {
            Vector3 point = new Vector3(path[index].x, path[index].y, transform.position.z);
            Gizmos.DrawLine(previous, point);
            Gizmos.DrawSphere(point, 0.05f);
            previous = point;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            new Vector3(currentDestination.x, currentDestination.y, transform.position.z),
            destinationTolerance);
    }
#endif
}
