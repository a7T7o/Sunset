using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 第一版共享局部避让求解器。
/// 采用优先级 + 前向探测 + 简单 steering / 侧绕混合机制。
/// </summary>
public static class NavigationLocalAvoidanceSolver
{
    public readonly struct AvoidanceResult
    {
        public readonly Vector2 AdjustedDirection;
        public readonly bool HasBlockingAgent;
        public readonly bool ShouldRepath;
        public readonly int BlockingAgentId;
        public readonly float BlockingDistance;

        public AvoidanceResult(Vector2 adjustedDirection, bool hasBlockingAgent, bool shouldRepath, int blockingAgentId, float blockingDistance)
        {
            AdjustedDirection = adjustedDirection;
            HasBlockingAgent = hasBlockingAgent;
            ShouldRepath = shouldRepath;
            BlockingAgentId = blockingAgentId;
            BlockingDistance = blockingDistance;
        }
    }

    public static AvoidanceResult Solve(
        in NavigationAgentSnapshot self,
        Vector2 desiredDirection,
        float lookAheadDistance,
        IList<NavigationAgentSnapshot> nearbyAgents)
    {
        if (!self.IsValid || desiredDirection.sqrMagnitude < 0.0001f || nearbyAgents == null || nearbyAgents.Count == 0)
        {
            return new AvoidanceResult(desiredDirection, false, false, 0, float.PositiveInfinity);
        }

        Vector2 desired = desiredDirection.normalized;
        Vector2 avoidance = Vector2.zero;
        int blockingAgentId = 0;
        float nearestBlockingDistance = float.PositiveInfinity;
        bool shouldRepath = false;

        for (int i = 0; i < nearbyAgents.Count; i++)
        {
            NavigationAgentSnapshot other = nearbyAgents[i];
            if (!NavigationAvoidanceRules.ShouldConsiderForLocalAvoidance(self, other))
            {
                continue;
            }

            Vector2 offset = other.Position - self.Position;
            float forwardDistance = Vector2.Dot(offset, desired);
            if (forwardDistance < 0f || forwardDistance > lookAheadDistance)
            {
                continue;
            }

            Vector2 lateral = offset - desired * forwardDistance;
            float lateralDistance = lateral.magnitude;
            float interactionRadius = NavigationAvoidanceRules.GetInteractionRadius(self, other);
            if (lateralDistance > interactionRadius)
            {
                continue;
            }

            bool shouldYield = NavigationAvoidanceRules.ShouldYield(self, other);
            bool treatAsBlockingObstacle = NavigationAvoidanceRules.ShouldTreatAsBlockingObstacle(other);

            float forwardFactor = 1f - Mathf.Clamp01(forwardDistance / Mathf.Max(lookAheadDistance, 0.001f));
            float lateralFactor = 1f - Mathf.Clamp01(lateralDistance / Mathf.Max(interactionRadius, 0.001f));
            float weight = forwardFactor * lateralFactor;

            Vector2 sidestepAxis = Vector2.Perpendicular(desired).normalized;
            float side = Mathf.Sign(Vector2.Dot(sidestepAxis, offset));
            if (Mathf.Approximately(side, 0f))
            {
                side = (other.InstanceId & 1) == 0 ? 1f : -1f;
            }

            Vector2 sidestep = sidestepAxis * -side;
            avoidance += sidestep * weight * (shouldYield ? 1.25f : 0.65f);

            if (treatAsBlockingObstacle || shouldYield)
            {
                if (forwardDistance < nearestBlockingDistance)
                {
                    nearestBlockingDistance = forwardDistance;
                    blockingAgentId = other.InstanceId;
                }

                if (treatAsBlockingObstacle && forwardDistance <= interactionRadius * 1.2f)
                {
                    shouldRepath = true;
                }
            }
        }

        Vector2 adjustedDirection = desired;
        if (avoidance.sqrMagnitude > 0.0001f)
        {
            adjustedDirection = (desired + avoidance).normalized;
        }

        bool hasBlockingAgent = blockingAgentId != 0;
        return new AvoidanceResult(adjustedDirection, hasBlockingAgent, shouldRepath, blockingAgentId, nearestBlockingDistance);
    }
}
