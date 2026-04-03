using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using System.Reflection;
#endif
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 专门验证“普通地面点导航是否按玩家实际占位中心到点”的轻量级 live runner。
/// 与 NavigationLiveValidationRunner 分离，避免和动态矩阵主线互踩。
/// </summary>
public class NavigationStaticPointValidationRunner : MonoBehaviour
{
    private const string LogPrefix = "[NavStaticValidation]";
#if UNITY_EDITOR
    private const string PendingActionEditorPrefKey = "Sunset.NavigationStaticValidation.PendingAction";
    private const string LiveValidationPendingActionEditorPrefKey = "Sunset.NavigationLiveValidation.PendingAction";
    private static readonly FieldInfo LiveValidationIsRunningField =
        typeof(NavigationLiveValidationRunner).GetField("isRunning", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo LiveValidationCurrentScenarioField =
        typeof(NavigationLiveValidationRunner).GetField("currentScenario", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo LiveValidationQueuedScenarioField =
        typeof(NavigationLiveValidationRunner).GetField("queuedScenario", BindingFlags.Instance | BindingFlags.NonPublic);
#endif
    private const string PendingLaunchMarkerRelativePath = "Library/NavStaticPointValidation.pending";
    private const float ScenarioTimeout = 5f;
    private const int StopSettleFrames = 8;
    private const float StopVelocityThreshold = 0.03f;
    private const float AllowedCenterError = 0.2f;
    private const float PathProbeMaxLengthRatio = 1.12f;
    private const float PathProbeMaxLateralDeviation = 0.18f;
    private static readonly Vector2 ValidationStartCenter = new Vector2(-8.16f, 7.38f);
    private static readonly Vector2[] CandidateOffsets =
    {
        new Vector2(1.6f, 0f),
        new Vector2(0f, 1.6f),
        new Vector2(1.15f, 1.15f),
        new Vector2(-1.4f, 0.2f)
    };

    private readonly List<ValidationCase> acceptedCases = new List<ValidationCase>(CandidateOffsets.Length);
    private readonly List<CaseResult> results = new List<CaseResult>(CandidateOffsets.Length);
    private readonly List<Vector2> pathProbeBuffer = new List<Vector2>(16);

    private PlayerAutoNavigator playerNavigator;
    private GameInputManager inputManager;
    private Rigidbody2D playerRigidbody;
    private Collider2D playerCollider;
    private NavGrid2D navGrid;
    private NavigationLiveValidationRunner conflictingLiveValidationRunner;

    private bool isRunning;
    private int currentCaseIndex = -1;
    private float scenarioTimer;
    private int settledFrames;
    private bool clickIssued;
    private Vector2 currentTarget;
    private string currentCaseName = string.Empty;
    private Vector2 runStartCenter;
    private Vector2 runStartActorPosition;

    private struct CaseResult
    {
        public string Name;
        public bool Passed;
        public string Details;
    }

    private struct ValidationCase
    {
        public Vector2 DesiredOffset;
        public Vector2 Target;
    }

    public static NavigationStaticPointValidationRunner BeginOrRestart()
    {
        NavigationStaticPointValidationRunner runner = FindFirstObjectByType<NavigationStaticPointValidationRunner>();
        if (runner == null)
        {
            GameObject go = new GameObject("NavigationStaticPointValidationRunner");
            runner = go.AddComponent<NavigationStaticPointValidationRunner>();
        }

        runner.StartRun();
        return runner;
    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoRunFromEditorLaunchRequest()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        bool launchedFromEditorPrefs = EditorPrefs.HasKey(PendingActionEditorPrefKey);
        if (launchedFromEditorPrefs)
        {
            string action = EditorPrefs.GetString(PendingActionEditorPrefKey, string.Empty);
            EditorPrefs.DeleteKey(PendingActionEditorPrefKey);
            if (action != "RunStaticPointValidation")
            {
                return;
            }

            if (HasConflictingLiveValidationPendingAction(out string liveAction))
            {
                Debug.LogWarning($"{LogPrefix} skipped_editor_pref_launch_due_to_live_validation_pending action={liveAction}");
                return;
            }

            Debug.Log($"{LogPrefix} runtime_launch_request={action}");
            BeginOrRestart();
            return;
        }

        if (HasConflictingLiveValidationPendingAction(out string pendingLiveAction))
        {
            if (ConsumePendingLaunchMarker())
            {
                Debug.LogWarning($"{LogPrefix} skipped_marker_launch_due_to_live_validation_pending action={pendingLiveAction}");
            }

            return;
        }

        if (!ConsumePendingLaunchMarker())
        {
            return;
        }

        Debug.Log($"{LogPrefix} runtime_launch_request=MarkerFile");
        BeginOrRestart();
    }
#endif

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnDisable()
    {
        if (!isRunning)
        {
            return;
        }

        Debug.LogWarning(
            $"{LogPrefix} runner_disabled isRunning={isRunning} caseIndex={currentCaseIndex} caseName={currentCaseName} elapsed={scenarioTimer:F2}");
    }

    private void OnDestroy()
    {
        if (!isRunning)
        {
            return;
        }

        Debug.LogWarning(
            $"{LogPrefix} runner_destroyed isRunning={isRunning} caseIndex={currentCaseIndex} caseName={currentCaseName} elapsed={scenarioTimer:F2}");
    }

    private void Update()
    {
        if (!Application.isPlaying || !isRunning)
        {
            return;
        }

        if (!EnsureBindings())
        {
            AbortRun("missing_runtime_bindings");
            return;
        }

        if (HasConflictingLiveValidationRunner())
        {
            AbortRun("conflicting_navigation_live_validation_runner");
            return;
        }

        if (currentCaseIndex < 0)
        {
            BuildAcceptedOffsets();
            if (acceptedCases.Count == 0)
            {
                AbortRun("no_valid_ground_click_cases");
                return;
            }

            StartNextCase();
            return;
        }

        TickCurrentCase();
    }

    private void StartRun()
    {
        isRunning = true;
        currentCaseIndex = -1;
        scenarioTimer = 0f;
        settledFrames = 0;
        clickIssued = false;
        currentTarget = Vector2.zero;
        currentCaseName = string.Empty;
        runStartCenter = ValidationStartCenter;
        runStartActorPosition = Vector2.zero;
        acceptedCases.Clear();
        results.Clear();

        if (!EnsureBindings())
        {
            AbortRun("missing_runtime_bindings");
            return;
        }

        Debug.Log($"{LogPrefix} runner_started");
    }

    private bool EnsureBindings()
    {
        if (playerNavigator == null)
        {
            playerNavigator = FindFirstObjectByType<PlayerAutoNavigator>();
        }

        if (inputManager == null)
        {
            inputManager = FindFirstObjectByType<GameInputManager>();
        }

        if (playerNavigator == null || inputManager == null)
        {
            return false;
        }

        // Scene reload / backup restore can swap the navigator instance while stale component refs survive.
        // Always rebind Rigidbody2D / Collider2D from the current navigator so reset math stays coherent.
        Rigidbody2D navigatorRigidbody = playerNavigator.GetComponent<Rigidbody2D>();
        if (playerRigidbody != navigatorRigidbody)
        {
            playerRigidbody = navigatorRigidbody;
        }

        Collider2D navigatorCollider = playerNavigator.GetComponent<Collider2D>();
        if (playerCollider != navigatorCollider)
        {
            playerCollider = navigatorCollider;
        }

        if (navGrid == null)
        {
            navGrid = FindFirstObjectByType<NavGrid2D>();
        }

        if (conflictingLiveValidationRunner == null)
        {
            conflictingLiveValidationRunner = FindFirstObjectByType<NavigationLiveValidationRunner>();
        }

        return playerRigidbody != null;
    }

    private void BuildAcceptedOffsets()
    {
        acceptedCases.Clear();
        Vector2 origin = runStartCenter;
        for (int i = 0; i < CandidateOffsets.Length; i++)
        {
            Vector2 desiredOffset = CandidateOffsets[i];
            if (TryResolveGroundClickTarget(origin, desiredOffset, out Vector2 acceptedTarget, out string reason))
            {
                acceptedCases.Add(new ValidationCase
                {
                    DesiredOffset = desiredOffset,
                    Target = acceptedTarget
                });
                Debug.Log($"{LogPrefix} candidate_accept offset={desiredOffset} target={acceptedTarget}");
            }
            else
            {
                Debug.Log($"{LogPrefix} candidate_reject offset={desiredOffset} reason={reason}");
            }
        }

        Debug.Log($"{LogPrefix} accepted_case_count={acceptedCases.Count}");
    }

    private void StartNextCase()
    {
        currentCaseIndex++;
        if (currentCaseIndex >= acceptedCases.Count)
        {
            FinishRun();
            return;
        }

        scenarioTimer = 0f;
        settledFrames = 0;
        clickIssued = false;
        ResetPlayerToRunStart();

        Vector2 origin = GetPlayerCenter();
        ValidationCase validationCase = acceptedCases[currentCaseIndex];
        Vector2 desiredOffset = validationCase.DesiredOffset;
        currentCaseName = $"StaticPointCase{currentCaseIndex + 1}";
        currentTarget = validationCase.Target;

        clickIssued = inputManager.DebugIssueAutoNavClick(currentTarget);
        PlayerAutoNavigatorDebugSnapshot playerDebug = PlayerAutoNavigatorDebugCompat.Capture(playerNavigator);
        Debug.Log(
            $"{LogPrefix} case_start name={currentCaseName} clickIssued={clickIssued} origin={origin} target={currentTarget} desiredOffset={desiredOffset} " +
            $"navTarget={playerDebug.PathRequestDestination}");
    }

    private void TickCurrentCase()
    {
        scenarioTimer += Time.unscaledDeltaTime;

        bool navigatorActive = playerNavigator != null && playerNavigator.IsActive;
        float speed = playerRigidbody != null ? playerRigidbody.linearVelocity.magnitude : 0f;
        float centerDistance = Vector2.Distance(GetPlayerCenter(), currentTarget);
        bool centerReached = centerDistance <= AllowedCenterError;
        bool stopped = speed <= StopVelocityThreshold;

        if ((centerReached || !navigatorActive) && stopped)
        {
            settledFrames++;
        }
        else
        {
            settledFrames = 0;
        }

        if (settledFrames >= StopSettleFrames)
        {
            CompleteCurrentCase(false);
            return;
        }

        if (scenarioTimer >= ScenarioTimeout)
        {
            CompleteCurrentCase(!(centerReached && stopped));
        }
    }

    private void ResetPlayerToRunStart()
    {
        if (playerNavigator != null)
        {
            playerNavigator.ForceCancel();
        }

        // Bootstrap systems can still shift the player's collider/rigidbody offset after StartRun().
        // Recalculate the actor reset point per case so the reported origin stays anchored to ValidationStartCenter.
        runStartActorPosition = GetActorPositionForCenter(runStartCenter);

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.position = runStartActorPosition;
        }

        if (playerNavigator != null)
        {
            Transform actor = playerNavigator.transform;
            actor.position = new Vector3(runStartActorPosition.x, runStartActorPosition.y, actor.position.z);
        }

        Physics2D.SyncTransforms();
    }

    private Vector2 GetActorPositionForCenter(Vector2 centerPosition)
    {
        Vector2 currentActorPosition = playerRigidbody != null
            ? playerRigidbody.position
            : (playerNavigator != null ? (Vector2)playerNavigator.transform.position : centerPosition);
        Vector2 centerOffset = GetPlayerCenter() - currentActorPosition;
        return centerPosition - centerOffset;
    }

    private void CompleteCurrentCase(bool timedOut)
    {
        PlayerAutoNavigatorDebugSnapshot playerDebug = PlayerAutoNavigatorDebugCompat.Capture(playerNavigator);
        Vector2 transformPos = playerNavigator != null ? playerDebug.TransformPosition : Vector2.zero;
        Vector2 rigidbodyPos = playerNavigator != null ? playerDebug.RigidbodyPosition : transformPos;
        Vector2 centerPos = playerNavigator != null ? playerDebug.ColliderCenter : rigidbodyPos;

        Vector2 transformDelta = transformPos - currentTarget;
        Vector2 rigidbodyDelta = rigidbodyPos - currentTarget;
        Vector2 centerDelta = centerPos - currentTarget;

        float transformDistance = transformDelta.magnitude;
        float rigidbodyDistance = rigidbodyDelta.magnitude;
        float centerDistance = centerDelta.magnitude;
        bool passed = clickIssued && !timedOut && centerDistance <= AllowedCenterError;

        string details =
            $"clickIssued={clickIssued}, timeout={scenarioTimer:F2}, target={currentTarget}, " +
            $"transform={transformPos}, transformDelta={transformDelta}, transformDistance={transformDistance:F3}, " +
            $"rigidbody={rigidbodyPos}, rigidbodyDelta={rigidbodyDelta}, rigidbodyDistance={rigidbodyDistance:F3}, " +
            $"center={centerPos}, centerDelta={centerDelta}, centerDistance={centerDistance:F3}, " +
            $"pathRequest={playerDebug.PathRequestDestination}, resolved={playerDebug.ResolvedPathDestination}";

        results.Add(new CaseResult
        {
            Name = currentCaseName,
            Passed = passed,
            Details = details
        });

        Debug.Log(
            $"{LogPrefix} case_end name={currentCaseName} pass={passed} " +
            $"centerDistance={centerDistance:F3} rigidbodyDistance={rigidbodyDistance:F3} transformDistance={transformDistance:F3} " +
            $"target={currentTarget} centerDelta={centerDelta} rigidbodyDelta={rigidbodyDelta} transformDelta={transformDelta}");

        StartNextCase();
    }

    private bool TryResolveGroundClickTarget(Vector2 origin, Vector2 desiredOffset, out Vector2 target, out string reason)
    {
        target = origin + desiredOffset;
        reason = string.Empty;

        if (navGrid != null && !navGrid.IsWalkable(target, playerCollider))
        {
            if (!navGrid.TryFindNearestWalkable(target, out target, playerCollider))
            {
                reason = "no_walkable_target";
                return false;
            }
        }

        if (Vector2.Distance(origin, target) <= 0.9f)
        {
            reason = "target_too_close";
            return false;
        }

        if (ContainsInteractable(target))
        {
            reason = "interactable_overlap";
            return false;
        }

        if (navGrid != null)
        {
            pathProbeBuffer.Clear();
            if (!navGrid.TryFindPath(origin, target, pathProbeBuffer))
            {
                reason = "path_probe_failed";
                return false;
            }

            if (!IsPathProbeOpenGround(origin, target, out reason))
            {
                return false;
            }
        }

        return true;
    }

    private static bool ContainsInteractable(Vector2 target)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(target);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null)
            {
                continue;
            }

            if (hit.GetComponent<IInteractable>() != null || hit.GetComponentInParent<IInteractable>() != null)
            {
                return true;
            }
        }

        return false;
    }

    private Vector2 GetPlayerCenter()
    {
        if (playerNavigator != null)
        {
            return PlayerAutoNavigatorDebugCompat.Capture(playerNavigator).ColliderCenter;
        }

        if (playerCollider != null)
        {
            return playerCollider.bounds.center;
        }

        if (playerRigidbody != null)
        {
            return playerRigidbody.position;
        }

        return Vector2.zero;
    }

    private bool IsPathProbeOpenGround(Vector2 origin, Vector2 target, out string reason)
    {
        reason = string.Empty;
        if (pathProbeBuffer.Count <= 1)
        {
            return true;
        }

        float directDistance = Vector2.Distance(origin, target);
        if (directDistance <= 0.001f)
        {
            return true;
        }

        float pathLength = 0f;
        float maxLateralDeviation = 0f;
        Vector2 previous = pathProbeBuffer[0];
        for (int i = 1; i < pathProbeBuffer.Count; i++)
        {
            Vector2 current = pathProbeBuffer[i];
            pathLength += Vector2.Distance(previous, current);
            maxLateralDeviation = Mathf.Max(
                maxLateralDeviation,
                DistancePointToSegment(current, origin, target));
            previous = current;
        }

        float lengthRatio = pathLength / directDistance;
        if (lengthRatio > PathProbeMaxLengthRatio || maxLateralDeviation > PathProbeMaxLateralDeviation)
        {
            reason =
                $"path_probe_not_open_ground lengthRatio={lengthRatio:F3} maxLateralDeviation={maxLateralDeviation:F3} pathCount={pathProbeBuffer.Count}";
            return false;
        }

        return true;
    }

    private bool HasConflictingLiveValidationRunner()
    {
        if (conflictingLiveValidationRunner == null)
        {
            conflictingLiveValidationRunner = FindFirstObjectByType<NavigationLiveValidationRunner>();
        }

        if (conflictingLiveValidationRunner == null)
        {
            return false;
        }

        if (!IsLiveValidationRunnerBusy(conflictingLiveValidationRunner))
        {
            return false;
        }

        Debug.LogWarning(
            $"{LogPrefix} conflicting_live_validation_runner name={conflictingLiveValidationRunner.name} active={conflictingLiveValidationRunner.isActiveAndEnabled}");
        return true;
    }

#if UNITY_EDITOR
    private static bool IsLiveValidationRunnerBusy(NavigationLiveValidationRunner runner)
    {
        if (runner == null)
        {
            return false;
        }

        try
        {
            bool isRunning = LiveValidationIsRunningField != null && (bool)LiveValidationIsRunningField.GetValue(runner);
            int currentScenario = LiveValidationCurrentScenarioField != null
                ? System.Convert.ToInt32(LiveValidationCurrentScenarioField.GetValue(runner))
                : 0;
            int queuedScenario = LiveValidationQueuedScenarioField != null
                ? System.Convert.ToInt32(LiveValidationQueuedScenarioField.GetValue(runner))
                : 0;
            return isRunning || currentScenario != 0 || queuedScenario != 0;
        }
        catch
        {
            return runner.isActiveAndEnabled;
        }
    }
#else
    private static bool IsLiveValidationRunnerBusy(NavigationLiveValidationRunner runner)
    {
        return runner != null && runner.isActiveAndEnabled;
    }
#endif

    private static float DistancePointToSegment(Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
    {
        Vector2 segment = segmentEnd - segmentStart;
        float segmentLengthSqr = segment.sqrMagnitude;
        if (segmentLengthSqr <= 0.0001f)
        {
            return Vector2.Distance(point, segmentStart);
        }

        float t = Mathf.Clamp01(Vector2.Dot(point - segmentStart, segment) / segmentLengthSqr);
        Vector2 projection = segmentStart + segment * t;
        return Vector2.Distance(point, projection);
    }

    private void FinishRun()
    {
        isRunning = false;

        int passCount = 0;
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].Passed)
            {
                passCount++;
            }
        }

        Debug.Log($"{LogPrefix} all_completed={(passCount == results.Count)} passCount={passCount} caseCount={results.Count}");
        for (int i = 0; i < results.Count; i++)
        {
            CaseResult result = results[i];
            Debug.Log($"{LogPrefix} summary name={result.Name} pass={result.Passed} {result.Details}");
        }

#if UNITY_EDITOR
        EditorApplication.delayCall += StopPlayMode;
#endif
    }

    private void AbortRun(string reason)
    {
        isRunning = false;
        Debug.LogError($"{LogPrefix} aborted reason={reason}");
#if UNITY_EDITOR
        EditorApplication.delayCall += StopPlayMode;
#endif
    }

#if UNITY_EDITOR
    private static void StopPlayMode()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
    }

    public static void WritePendingLaunchMarker()
    {
        string markerPath = GetPendingLaunchMarkerPath();
        string markerDirectory = Path.GetDirectoryName(markerPath);
        if (!string.IsNullOrEmpty(markerDirectory))
        {
            Directory.CreateDirectory(markerDirectory);
        }

        File.WriteAllText(markerPath, "RunStaticPointValidation");
        Debug.Log($"{LogPrefix} marker_launch_written path={markerPath}");
    }

    private static bool HasConflictingLiveValidationPendingAction(out string liveAction)
    {
        if (!EditorPrefs.HasKey(LiveValidationPendingActionEditorPrefKey))
        {
            liveAction = string.Empty;
            return false;
        }

        liveAction = EditorPrefs.GetString(LiveValidationPendingActionEditorPrefKey, string.Empty);
        return !string.IsNullOrEmpty(liveAction);
    }

    private static bool ConsumePendingLaunchMarker()
    {
        string markerPath = GetPendingLaunchMarkerPath();
        if (!File.Exists(markerPath))
        {
            return false;
        }

        File.Delete(markerPath);
        return true;
    }
#else
    public static void WritePendingLaunchMarker()
    {
    }
#endif

    private static string GetPendingLaunchMarkerPath()
    {
        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        return string.IsNullOrEmpty(projectRoot)
            ? PendingLaunchMarkerRelativePath
            : Path.Combine(projectRoot, PendingLaunchMarkerRelativePath);
    }
}
