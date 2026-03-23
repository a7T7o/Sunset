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
        public Vector2 Destination { get; set; }
        public Vector2 LastProgressCheckPosition { get; set; }
        public float LastProgressCheckTime { get; set; }
        public float LastProgressDistance { get; set; }
        public int StuckRetryCount { get; set; }
        public bool HasOverrideWaypoint { get; set; }
        public Vector2 OverrideWaypoint { get; set; }
        public int OverrideWaypointOwnerId { get; set; }
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

    public static void Clear(ExecutionState state, bool clearDestination = true)
    {
        if (state == null)
        {
            return;
        }

        state.Path.Clear();
        state.PathIndex = 0;
        state.LastProgressDistance = 0f;
        state.StuckRetryCount = 0;
        state.HasOverrideWaypoint = false;
        state.OverrideWaypoint = Vector2.zero;
        state.OverrideWaypointOwnerId = 0;

        if (clearDestination)
        {
            state.Destination = Vector2.zero;
        }
    }

    public static void ResetProgress(ExecutionState state, Vector2 currentPosition, bool resetCounter)
    {
        if (state == null)
        {
            return;
        }

        state.LastProgressCheckPosition = currentPosition;
        state.LastProgressCheckTime = Time.time;
        state.LastProgressDistance = 0f;
        if (resetCounter)
        {
            state.StuckRetryCount = 0;
        }
    }

    public static BuildPathResult TryBuildPath(
        ExecutionState state,
        NavGrid2D navGrid,
        Vector2 requestedStart,
        Vector2 requestedDestination,
        Action<string> log = null)
    {
        if (state == null)
        {
            return new BuildPathResult(false, "执行状态未初始化", requestedStart, requestedDestination, 0);
        }

        state.Path.Clear();
        state.PathIndex = 0;
        state.Destination = requestedDestination;

        if (navGrid == null)
        {
            log?.Invoke("NavGrid2D 未找到");
            return new BuildPathResult(false, "NavGrid2D 未找到", requestedStart, requestedDestination, 0);
        }

        Vector2 start = requestedStart;
        Vector2 destination = requestedDestination;

        log?.Invoke($"开始寻路: 起点={start}, 终点={destination}");

        if (!navGrid.IsWalkable(start))
        {
            log?.Invoke("起点不可走，尝试查找最近可走点");
            if (!navGrid.TryFindNearestWalkable(start, out start))
            {
                log?.Invoke("无法找到有效起点");
                return new BuildPathResult(false, "起点不可走且无法找到替代点", requestedStart, requestedDestination, 0);
            }

            log?.Invoke($"找到替代起点: {start}");
        }

        if (!navGrid.IsWalkable(destination))
        {
            log?.Invoke("终点不可走，尝试查找最近可走点");
            if (navGrid.TryFindNearestWalkable(destination, out Vector2 walkableDestination))
            {
                destination = walkableDestination;
                log?.Invoke($"找到替代终点: {destination}");
            }
            else
            {
                log?.Invoke("无法找到有效终点");
            }
        }

        if (!navGrid.TryFindPath(start, destination, state.Path))
        {
            log?.Invoke("A* 寻路失败");
            state.Path.Clear();
            return new BuildPathResult(false, "寻路失败", start, destination, 0);
        }

        log?.Invoke($"寻路成功: {state.Path.Count} 个路径点");
        return new BuildPathResult(true, null, start, destination, state.Path.Count);
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

    public static void SetOverrideWaypoint(ExecutionState state, Vector2 waypoint, int ownerId)
    {
        if (state == null)
        {
            return;
        }

        state.HasOverrideWaypoint = true;
        state.OverrideWaypoint = waypoint;
        state.OverrideWaypointOwnerId = ownerId;
    }

    public static void ClearOverrideWaypoint(ExecutionState state)
    {
        if (state == null)
        {
            return;
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
}
