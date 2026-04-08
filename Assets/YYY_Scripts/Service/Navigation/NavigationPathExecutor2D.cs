using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// S4 共享路径执行层。
/// 统一维护路径状态、路径推进、override waypoint、卡住检测与基础重规划入口。
/// </summary>
public static class NavigationPathExecutor2D
{
    [Serializable]
    public sealed class ExecutionState
    {
        public List<Vector2> Path { get; } = new List<Vector2>();
        public int PathIndex { get; set; }
        public bool HasDestination { get; set; }
        public Vector2 Destination { get; set; }
        public Vector2 LastProgressCheckPosition { get; set; }
        public float LastProgressCheckTime { get; set; }
        public float LastProgressDistance { get; set; }
        public int StuckRetryCount { get; set; }
        public float LastRepathTime { get; set; }
        public float LastRecoveryTime { get; set; }
        public float LastRecoveryDistance { get; set; }
        public bool LastRecoverySucceeded { get; set; }
        public bool HasOverrideWaypoint { get; set; }
        public Vector2 OverrideWaypoint { get; set; }
        public int OverrideWaypointOwnerId { get; set; }
        public int LastDetourOwnerId { get; set; }
        public Vector2 LastDetourPoint { get; set; }
        public float LastDetourCreateTime { get; set; }
        public float LastDetourClearTime { get; set; }
        public float LastDetourRecoveryTime { get; set; }
        public bool LastDetourRecoverySucceeded { get; set; }
    }

    public readonly struct BuildPathResult
    {
        public readonly bool Success;
        public readonly string FailureReason;
        public readonly Vector2 ActualStart;
        public readonly Vector2 ActualDestination;
        public readonly int PathCount;

        public BuildPathResult(
            bool success,
            string failureReason,
            Vector2 actualStart,
            Vector2 actualDestination,
            int pathCount)
        {
            Success = success;
            FailureReason = failureReason;
            ActualStart = actualStart;
            ActualDestination = actualDestination;
            PathCount = pathCount;
        }
    }

    public readonly struct WaypointResult
    {
        public readonly bool HasWaypoint;
        public readonly bool ReachedPathEnd;
        public readonly bool ClearedOverrideWaypoint;
        public readonly bool UsingOverrideWaypoint;
        public readonly bool IsLastPathWaypoint;
        public readonly Vector2 Waypoint;
        public readonly float DistanceToWaypoint;

        public WaypointResult(
            bool hasWaypoint,
            bool reachedPathEnd,
            bool clearedOverrideWaypoint,
            bool usingOverrideWaypoint,
            bool isLastPathWaypoint,
            Vector2 waypoint,
            float distanceToWaypoint)
        {
            HasWaypoint = hasWaypoint;
            ReachedPathEnd = reachedPathEnd;
            ClearedOverrideWaypoint = clearedOverrideWaypoint;
            UsingOverrideWaypoint = usingOverrideWaypoint;
            IsLastPathWaypoint = isLastPathWaypoint;
            Waypoint = waypoint;
            DistanceToWaypoint = distanceToWaypoint;
        }
    }

    public readonly struct ProgressCheckResult
    {
        public readonly bool Checked;
        public readonly bool IsStuck;
        public readonly bool ShouldRebuild;
        public readonly bool ShouldCancel;
        public readonly int RetryCount;
        public readonly float MovedDistance;

        public ProgressCheckResult(
            bool checkedThisFrame,
            bool isStuck,
            bool shouldRebuild,
            bool shouldCancel,
            int retryCount,
            float movedDistance)
        {
            Checked = checkedThisFrame;
            IsStuck = isStuck;
            ShouldRebuild = shouldRebuild;
            ShouldCancel = shouldCancel;
            RetryCount = retryCount;
            MovedDistance = movedDistance;
        }
    }

    public readonly struct StuckRecoveryResult
    {
        public readonly bool Checked;
        public readonly bool IsStuck;
        public readonly bool ShouldCancel;
        public readonly bool RepathAttempted;
        public readonly bool RepathSucceeded;
        public readonly bool RepathOnCooldown;
        public readonly int RetryCount;
        public readonly float MovedDistance;
        public readonly string FailureReason;

        public StuckRecoveryResult(
            bool checkedThisFrame,
            bool isStuck,
            bool shouldCancel,
            bool repathAttempted,
            bool repathSucceeded,
            bool repathOnCooldown,
            int retryCount,
            float movedDistance,
            string failureReason)
        {
            Checked = checkedThisFrame;
            IsStuck = isStuck;
            ShouldCancel = shouldCancel;
            RepathAttempted = repathAttempted;
            RepathSucceeded = repathSucceeded;
            RepathOnCooldown = repathOnCooldown;
            RetryCount = retryCount;
            MovedDistance = movedDistance;
            FailureReason = failureReason;
        }
    }

    public readonly struct DetourCandidateSpec
    {
        public readonly bool UseBlockingAgentAnchor;
        public readonly float SidestepScale;
        public readonly float SeparationScale;

        public DetourCandidateSpec(bool useBlockingAgentAnchor, float sidestepScale, float separationScale)
        {
            UseBlockingAgentAnchor = useBlockingAgentAnchor;
            SidestepScale = sidestepScale;
            SeparationScale = separationScale;
        }
    }

    public readonly struct DetourLifecycleResult
    {
        public readonly bool Created;
        public readonly bool Cleared;
        public readonly bool Recovered;
        public readonly bool HasActiveDetour;
        public readonly bool ShouldKeepCurrentDetour;
        public readonly int OwnerId;
        public readonly Vector2 DetourPoint;
        public readonly string FailureReason;

        public DetourLifecycleResult(
            bool created,
            bool cleared,
            bool recovered,
            bool hasActiveDetour,
            bool shouldKeepCurrentDetour,
            int ownerId,
            Vector2 detourPoint,
            string failureReason)
        {
            Created = created;
            Cleared = cleared;
            Recovered = recovered;
            HasActiveDetour = hasActiveDetour;
            ShouldKeepCurrentDetour = shouldKeepCurrentDetour;
            OwnerId = ownerId;
            DetourPoint = detourPoint;
            FailureReason = failureReason;
        }
    }

    public static void Clear(ExecutionState state, bool clearDestination = true)
    {
        if (state == null)
        {
            return;
        }

        state.Path.Clear();
        state.PathIndex = 0;
        state.LastRepathTime = float.NegativeInfinity;
        state.LastRecoveryTime = float.NegativeInfinity;
        state.LastRecoveryDistance = 0f;
        state.LastRecoverySucceeded = false;
        state.LastProgressDistance = 0f;
        state.StuckRetryCount = 0;
        state.HasOverrideWaypoint = false;
        state.OverrideWaypoint = Vector2.zero;
        state.OverrideWaypointOwnerId = 0;
        state.LastDetourOwnerId = 0;
        state.LastDetourPoint = Vector2.zero;
        state.LastDetourCreateTime = float.NegativeInfinity;
        state.LastDetourClearTime = float.NegativeInfinity;
        state.LastDetourRecoveryTime = float.NegativeInfinity;
        state.LastDetourRecoverySucceeded = false;

        if (clearDestination)
        {
            state.HasDestination = false;
            state.Destination = Vector2.zero;
        }
    }

    public static void ResetProgress(ExecutionState state, Vector2 currentPosition, bool resetCounter)
    {
        if (state == null)
        {
            return;
        }

        ResetProgressState(state, currentPosition, Time.time, resetCounter);
    }

    public static BuildPathResult TryBuildPath(
        ExecutionState state,
        NavGrid2D navGrid,
        Vector2 requestedStart,
        Vector2 requestedDestination,
        Action<string> log = null,
        Collider2D ignoredCollider = null)
    {
        if (state == null)
        {
            return new BuildPathResult(false, "执行状态未初始化", requestedStart, requestedDestination, 0);
        }

        if (navGrid == null)
        {
            log?.Invoke("NavGrid2D 未找到");
            return new BuildPathResult(false, "NavGrid2D 未找到", requestedStart, requestedDestination, 0);
        }

        Vector2 start = requestedStart;
        Vector2 destination = requestedDestination;

        log?.Invoke($"开始寻路: 起点={start}, 终点={destination}");

        if (!navGrid.IsWalkable(start, ignoredCollider))
        {
            log?.Invoke("起点不可走，尝试查找最近可走点");
            if (!navGrid.TryFindNearestWalkable(start, out start, ignoredCollider))
            {
                log?.Invoke("无法找到有效起点");
                return new BuildPathResult(false, "起点不可走且无法找到替代点", requestedStart, requestedDestination, 0);
            }

            log?.Invoke($"找到替代起点: {start}");
        }

        if (!navGrid.IsWalkable(destination, ignoredCollider))
        {
            log?.Invoke("终点不可走，尝试查找最近可走点");
            if (navGrid.TryFindNearestWalkable(destination, out Vector2 walkableDestination, ignoredCollider))
            {
                destination = walkableDestination;
                log?.Invoke($"找到替代终点: {destination}");
            }
            else
            {
                log?.Invoke("无法找到有效终点");
            }
        }

        List<Vector2> builtPath = new List<Vector2>(Mathf.Max(8, state.Path.Count));
        if (!navGrid.TryFindPath(start, destination, builtPath))
        {
            log?.Invoke("A* 寻路失败");
            return new BuildPathResult(false, "寻路失败", start, destination, 0);
        }

        AppendContinuousDestination(builtPath, destination, log);

        state.Path.Clear();
        state.Path.AddRange(builtPath);
        state.PathIndex = 0;
        state.HasDestination = true;
        state.Destination = destination;

        log?.Invoke($"寻路成功: {state.Path.Count} 个路径点");
        return new BuildPathResult(true, null, start, destination, state.Path.Count);
    }

    public static BuildPathResult TryRefreshPath(
        ExecutionState state,
        NavGrid2D navGrid,
        Vector2 requestedStart,
        Vector2 requestedDestination,
        Func<Vector2, Vector2, bool> hasLineOfSight = null,
        Vector2? cleanupReferencePosition = null,
        float cleanupWaypointTolerance = 0f,
        Action<string> log = null,
        Collider2D ignoredCollider = null)
    {
        BuildPathResult buildResult = TryBuildPath(
            state,
            navGrid,
            requestedStart,
            requestedDestination,
            log,
            ignoredCollider);

        if (!buildResult.Success)
        {
            return buildResult;
        }

        if (hasLineOfSight != null)
        {
            SmoothPath(state, hasLineOfSight, log);
        }

        if (cleanupReferencePosition.HasValue && cleanupWaypointTolerance > 0f)
        {
            CleanupPathBehind(state, cleanupReferencePosition.Value, cleanupWaypointTolerance);
        }

        return new BuildPathResult(
            true,
            null,
            buildResult.ActualStart,
            state.Destination,
            state.Path.Count);
    }

    public static void SmoothPath(
        ExecutionState state,
        Func<Vector2, Vector2, bool> hasLineOfSight,
        Action<string> log = null)
    {
        if (state == null || state.Path.Count < 3 || hasLineOfSight == null)
        {
            return;
        }

        List<Vector2> smoothed = new List<Vector2>(state.Path.Count) { state.Path[0] };
        int current = 0;

        while (current < state.Path.Count - 1)
        {
            int farthest = current + 1;
            for (int i = state.Path.Count - 1; i > current + 1; i--)
            {
                if (hasLineOfSight(state.Path[current], state.Path[i]))
                {
                    farthest = i;
                    break;
                }
            }

            smoothed.Add(state.Path[farthest]);
            current = farthest;
        }

        int removed = state.Path.Count - smoothed.Count;
        state.Path.Clear();
        state.Path.AddRange(smoothed);

        if (removed > 0)
        {
            log?.Invoke($"路径平滑：移除 {removed} 个冗余点");
        }
    }

    public static void CleanupPathBehind(
        ExecutionState state,
        Vector2 currentPosition,
        float waypointTolerance)
    {
        if (state == null || state.Path.Count < 2)
        {
            return;
        }

        while (state.Path.Count > 1)
        {
            Vector2 first = state.Path[0];
            if (Vector2.Distance(currentPosition, first) < waypointTolerance * 0.5f)
            {
                state.Path.RemoveAt(0);
                continue;
            }

            Vector2 second = state.Path[1];
            Vector2 pathDirection = (second - first).normalized;
            Vector2 toFirst = (first - currentPosition).normalized;
            if (Vector2.Dot(toFirst, pathDirection) < -0.2f)
            {
                state.Path.RemoveAt(0);
                continue;
            }

            break;
        }
    }

    public static WaypointResult EvaluateWaypoint(
        ExecutionState state,
        Vector2 currentPosition,
        float waypointTolerance,
        float finalWaypointTolerance,
        float overrideToleranceMultiplier = 1.25f)
    {
        if (state == null)
        {
            return default;
        }

        if (state.HasOverrideWaypoint)
        {
            float overrideDistance = Vector2.Distance(currentPosition, state.OverrideWaypoint);
            if (overrideDistance <= waypointTolerance * overrideToleranceMultiplier)
            {
                ClearOverrideWaypoint(state);
                return new WaypointResult(false, false, true, true, false, Vector2.zero, overrideDistance);
            }

            return new WaypointResult(true, false, false, true, false, state.OverrideWaypoint, overrideDistance);
        }

        while (state.PathIndex < state.Path.Count)
        {
            bool isLastPathWaypoint = state.PathIndex == state.Path.Count - 1;
            float tolerance = isLastPathWaypoint ? Mathf.Max(waypointTolerance, finalWaypointTolerance) : waypointTolerance;
            float distance = Vector2.Distance(currentPosition, state.Path[state.PathIndex]);
            if (distance > tolerance)
            {
                return new WaypointResult(
                    true,
                    false,
                    false,
                    false,
                    isLastPathWaypoint,
                    state.Path[state.PathIndex],
                    distance);
            }

            state.PathIndex++;
        }

        return new WaypointResult(false, true, false, false, false, Vector2.zero, 0f);
    }

    public static ProgressCheckResult EvaluateStuck(
        ExecutionState state,
        Vector2 currentPosition,
        float currentTime,
        float checkInterval,
        float distanceThreshold,
        int maxRetries)
    {
        if (state == null || currentTime - state.LastProgressCheckTime < checkInterval)
        {
            return default;
        }

        float movedDistance = Vector2.Distance(currentPosition, state.LastProgressCheckPosition);
        state.LastProgressCheckPosition = currentPosition;
        state.LastProgressCheckTime = currentTime;
        state.LastProgressDistance = movedDistance;

        if (movedDistance >= distanceThreshold)
        {
            state.StuckRetryCount = 0;
            return new ProgressCheckResult(true, false, false, false, 0, movedDistance);
        }

        state.StuckRetryCount++;
        bool shouldCancel = state.StuckRetryCount >= maxRetries;
        return new ProgressCheckResult(
            true,
            true,
            !shouldCancel,
            shouldCancel,
            state.StuckRetryCount,
            movedDistance);
    }

    public static StuckRecoveryResult TryHandleStuckRecovery(
        ExecutionState state,
        NavGrid2D navGrid,
        Vector2 currentPosition,
        Vector2 rebuildDestination,
        float currentTime,
        float checkInterval,
        float distanceThreshold,
        int maxRetries,
        float repathCooldown,
        Func<Vector2, Vector2, bool> hasLineOfSight = null,
        Vector2? cleanupReferencePosition = null,
        float cleanupWaypointTolerance = 0f,
        Action<string> log = null,
        Collider2D ignoredCollider = null)
    {
        ProgressCheckResult progress = EvaluateStuck(
            state,
            currentPosition,
            currentTime,
            checkInterval,
            distanceThreshold,
            maxRetries);

        if (!progress.Checked || !progress.IsStuck)
        {
            return new StuckRecoveryResult(
                progress.Checked,
                progress.IsStuck,
                false,
                false,
                false,
                false,
                progress.RetryCount,
                progress.MovedDistance,
                null);
        }

        state.LastRecoveryTime = currentTime;
        state.LastRecoveryDistance = progress.MovedDistance;
        state.LastRecoverySucceeded = false;

        if (progress.ShouldCancel)
        {
            return new StuckRecoveryResult(
                true,
                true,
                true,
                false,
                false,
                false,
                progress.RetryCount,
                progress.MovedDistance,
                "max_retries_reached");
        }

        float safeRepathCooldown = Mathf.Max(0f, repathCooldown);
        if (safeRepathCooldown > 0f && currentTime - state.LastRepathTime < safeRepathCooldown)
        {
            return new StuckRecoveryResult(
                true,
                true,
                false,
                false,
                false,
                true,
                progress.RetryCount,
                progress.MovedDistance,
                "repath_cooldown_active");
        }

        BuildPathResult buildResult = TryRefreshPath(
            state,
            navGrid,
            currentPosition,
            rebuildDestination,
            hasLineOfSight,
            cleanupReferencePosition,
            cleanupWaypointTolerance,
            log,
            ignoredCollider);

        if (!buildResult.Success || state.Path.Count == 0)
        {
            return new StuckRecoveryResult(
                true,
                true,
                false,
                true,
                false,
                false,
                progress.RetryCount,
                progress.MovedDistance,
                buildResult.FailureReason);
        }

        state.LastRepathTime = currentTime;
        state.LastRecoverySucceeded = true;
        ResetProgressState(state, currentPosition, currentTime, false);

        return new StuckRecoveryResult(
            true,
            true,
            false,
            true,
            true,
            false,
            progress.RetryCount,
            progress.MovedDistance,
            null);
    }

    public static DetourLifecycleResult TryCreateDetour(
        ExecutionState state,
        NavGrid2D navGrid,
        Vector2 currentPosition,
        int blockingAgentId,
        Vector2 blockingAgentPosition,
        Vector2 adjustedDirection,
        Vector2 suggestedDetourDirection,
        float detourDistance,
        float minimumClearanceMultiplier,
        DetourCandidateSpec[] candidateSpecs,
        float currentTime)
    {
        if (state == null)
        {
            return new DetourLifecycleResult(false, false, false, false, false, 0, Vector2.zero, "state_missing");
        }

        if (state.HasOverrideWaypoint)
        {
            return new DetourLifecycleResult(false, false, false, true, true, state.OverrideWaypointOwnerId, state.OverrideWaypoint, "detour_already_active");
        }

        if (navGrid == null)
        {
            return new DetourLifecycleResult(false, false, false, false, false, blockingAgentId, Vector2.zero, "navgrid_missing");
        }

        if (candidateSpecs == null || candidateSpecs.Length == 0)
        {
            return new DetourLifecycleResult(false, false, false, false, false, blockingAgentId, Vector2.zero, "candidate_specs_missing");
        }

        Vector2 separationDirection = currentPosition - blockingAgentPosition;
        if (separationDirection.sqrMagnitude < 0.0001f)
        {
            separationDirection = -adjustedDirection;
        }

        if (separationDirection.sqrMagnitude < 0.0001f)
        {
            separationDirection = Vector2.left;
        }

        separationDirection.Normalize();
        Vector2 sidestepDirection = suggestedDetourDirection.sqrMagnitude > 0.0001f
            ? suggestedDetourDirection.normalized
            : Vector2.Perpendicular(separationDirection).normalized;

        float safeDetourDistance = Mathf.Max(detourDistance, 0.05f);
        float minimumClearanceSqr = safeDetourDistance * safeDetourDistance * Mathf.Max(1f, minimumClearanceMultiplier);

        for (int index = 0; index < candidateSpecs.Length; index++)
        {
            DetourCandidateSpec spec = candidateSpecs[index];
            Vector2 anchor = spec.UseBlockingAgentAnchor ? blockingAgentPosition : currentPosition;
            Vector2 candidate = anchor
                + sidestepDirection * (safeDetourDistance * spec.SidestepScale)
                + separationDirection * (safeDetourDistance * spec.SeparationScale);

            if (!TryResolveDetourCandidate(navGrid, candidate, blockingAgentPosition, minimumClearanceSqr, out Vector2 detourPoint))
            {
                continue;
            }

            SetOverrideWaypoint(state, detourPoint, blockingAgentId);
            state.LastDetourOwnerId = blockingAgentId;
            state.LastDetourPoint = detourPoint;
            state.LastDetourCreateTime = currentTime;
            state.LastDetourRecoverySucceeded = false;
            return new DetourLifecycleResult(true, false, false, true, false, blockingAgentId, detourPoint, null);
        }

        return new DetourLifecycleResult(false, false, false, false, false, blockingAgentId, Vector2.zero, "no_detour_candidate");
    }

    public static DetourLifecycleResult TryClearDetourAndRecover(
        ExecutionState state,
        NavGrid2D navGrid,
        Vector2 currentPosition,
        Vector2 rebuildDestination,
        int blockingAgentId,
        float currentTime,
        bool rebuildPath,
        Func<Vector2, Vector2, bool> hasLineOfSight = null,
        Vector2? cleanupReferencePosition = null,
        float cleanupWaypointTolerance = 0f,
        Action<string> log = null,
        Collider2D ignoredCollider = null)
    {
        return TryClearDetourAndRecover(
            state,
            navGrid,
            currentPosition,
            rebuildDestination,
            blockingAgentId,
            currentTime,
            rebuildPath,
            0f,
            0f,
            hasLineOfSight,
            cleanupReferencePosition,
            cleanupWaypointTolerance,
            log,
            ignoredCollider);
    }

    public static DetourLifecycleResult TryClearDetourAndRecover(
        ExecutionState state,
        NavGrid2D navGrid,
        Vector2 currentPosition,
        Vector2 rebuildDestination,
        int blockingAgentId,
        float currentTime,
        bool rebuildPath,
        float minimumDetourActiveDuration,
        float recoveryCooldown,
        Func<Vector2, Vector2, bool> hasLineOfSight = null,
        Vector2? cleanupReferencePosition = null,
        float cleanupWaypointTolerance = 0f,
        Action<string> log = null,
        Collider2D ignoredCollider = null)
    {
        if (state == null)
        {
            return new DetourLifecycleResult(false, false, false, false, false, 0, Vector2.zero, "state_missing");
        }

        if (state.HasOverrideWaypoint && blockingAgentId != 0 && state.OverrideWaypointOwnerId == blockingAgentId)
        {
            return new DetourLifecycleResult(false, false, false, true, true, state.OverrideWaypointOwnerId, state.OverrideWaypoint, "owner_still_blocking");
        }

        bool hadActiveDetour = state.HasOverrideWaypoint;
        int detourOwnerId = hadActiveDetour ? state.OverrideWaypointOwnerId : state.LastDetourOwnerId;
        Vector2 detourPoint = hadActiveDetour ? state.OverrideWaypoint : state.LastDetourPoint;
        float safeMinActiveDuration = Mathf.Max(0f, minimumDetourActiveDuration);
        float safeRecoveryCooldown = Mathf.Max(0f, recoveryCooldown);

        if (!hadActiveDetour && detourOwnerId == 0)
        {
            return new DetourLifecycleResult(false, false, false, false, false, 0, Vector2.zero, "no_detour_context");
        }

        if (hadActiveDetour &&
            safeMinActiveDuration > 0f &&
            state.LastDetourCreateTime > float.NegativeInfinity &&
            currentTime - state.LastDetourCreateTime < safeMinActiveDuration)
        {
            return new DetourLifecycleResult(
                false,
                false,
                false,
                true,
                true,
                detourOwnerId,
                detourPoint,
                "detour_clear_hysteresis");
        }

        if (safeRecoveryCooldown > 0f &&
            state.LastDetourRecoveryTime > float.NegativeInfinity &&
            currentTime - state.LastDetourRecoveryTime < safeRecoveryCooldown)
        {
            return new DetourLifecycleResult(
                false,
                false,
                false,
                hadActiveDetour,
                hadActiveDetour,
                detourOwnerId,
                detourPoint,
                hadActiveDetour
                    ? "detour_owner_release_cooldown"
                    : "detour_recovery_cooldown");
        }

        if (hadActiveDetour)
        {
            ClearOverrideWaypoint(state, currentTime);
        }

        state.LastDetourRecoveryTime = currentTime;
        state.LastDetourRecoverySucceeded = false;

        if (!rebuildPath)
        {
            state.LastDetourRecoverySucceeded = true;
            ResetProgressState(state, currentPosition, currentTime, false);
            return new DetourLifecycleResult(false, hadActiveDetour, true, false, false, detourOwnerId, detourPoint, null);
        }

        BuildPathResult buildResult = TryRefreshPath(
            state,
            navGrid,
            currentPosition,
            rebuildDestination,
            hasLineOfSight,
            cleanupReferencePosition,
            cleanupWaypointTolerance,
            log,
            ignoredCollider);

        if (!buildResult.Success || state.Path.Count == 0)
        {
            return new DetourLifecycleResult(false, hadActiveDetour, false, false, false, detourOwnerId, detourPoint, buildResult.FailureReason);
        }

        state.LastDetourRecoverySucceeded = true;
        ResetProgressState(state, currentPosition, currentTime, false);
        return new DetourLifecycleResult(false, hadActiveDetour, true, false, false, detourOwnerId, detourPoint, null);
    }

    public static void SetOverrideWaypoint(ExecutionState state, Vector2 waypoint, int ownerId)
    {
        if (state == null)
        {
            return;
        }

        state.HasOverrideWaypoint = true;
        state.OverrideWaypoint = waypoint;
        state.OverrideWaypointOwnerId = ownerId;
        state.LastDetourOwnerId = ownerId;
        state.LastDetourPoint = waypoint;
        state.LastDetourCreateTime = Time.time;
        state.LastDetourRecoverySucceeded = false;
    }

    public static void ClearOverrideWaypoint(ExecutionState state)
    {
        ClearOverrideWaypoint(state, Time.time);
    }

    public static void ClearOverrideWaypoint(ExecutionState state, float currentTime)
    {
        if (state == null)
        {
            return;
        }

        if (state.HasOverrideWaypoint)
        {
            state.LastDetourOwnerId = state.OverrideWaypointOwnerId;
            state.LastDetourPoint = state.OverrideWaypoint;
            state.LastDetourClearTime = currentTime;
        }

        state.HasOverrideWaypoint = false;
        state.OverrideWaypoint = Vector2.zero;
        state.OverrideWaypointOwnerId = 0;
    }

    public static void ClearOverrideWaypointIfChanged(ExecutionState state, int blockingAgentId)
    {
        if (state == null || !state.HasOverrideWaypoint)
        {
            return;
        }

        if (blockingAgentId == 0 || state.OverrideWaypointOwnerId != blockingAgentId)
        {
            ClearOverrideWaypoint(state);
        }
    }

    public static Vector2 GetResolvedDestination(ExecutionState state, Vector2 fallbackDestination)
    {
        if (state == null)
        {
            return fallbackDestination;
        }

        if (state.HasDestination)
        {
            return state.Destination;
        }

        if (state.Path.Count > 0)
        {
            return state.Path[state.Path.Count - 1];
        }

        return fallbackDestination;
    }

    private static bool TryResolveDetourCandidate(
        NavGrid2D navGrid,
        Vector2 candidate,
        Vector2 blockingAgentPosition,
        float minimumClearanceSqr,
        out Vector2 detourPoint)
    {
        detourPoint = Vector2.zero;
        if (navGrid == null || !navGrid.TryFindNearestWalkable(candidate, out detourPoint))
        {
            return false;
        }

        return (detourPoint - blockingAgentPosition).sqrMagnitude >= minimumClearanceSqr;
    }

    private static void ResetProgressState(ExecutionState state, Vector2 currentPosition, float currentTime, bool resetCounter)
    {
        state.LastProgressCheckPosition = currentPosition;
        state.LastProgressCheckTime = currentTime;
        state.LastProgressDistance = 0f;
        if (resetCounter)
        {
            state.StuckRetryCount = 0;
            state.LastRepathTime = float.NegativeInfinity;
            state.LastRecoveryTime = float.NegativeInfinity;
            state.LastRecoveryDistance = 0f;
            state.LastRecoverySucceeded = false;
        }
    }

    private static void AppendContinuousDestination(List<Vector2> path, Vector2 destination, Action<string> log)
    {
        if (path == null)
        {
            return;
        }

        if (path.Count == 0)
        {
            path.Add(destination);
            log?.Invoke($"路径补终点: 直接追加真实终点 {destination}");
            return;
        }

        Vector2 lastPoint = path[path.Count - 1];
        const float destinationAppendThreshold = 0.01f;
        if (Vector2.Distance(lastPoint, destination) <= destinationAppendThreshold)
        {
            path[path.Count - 1] = destination;
            return;
        }

        path.Add(destination);
        log?.Invoke($"路径补终点: 网格末点={lastPoint}, 真实终点={destination}");
    }
}
