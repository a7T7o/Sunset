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
        public readonly float SpeedScale;
        public readonly bool HasBlockingAgent;
        public readonly bool ShouldRepath;
        public readonly int BlockingAgentId;
        public readonly float BlockingDistance;
        public readonly Vector2 BlockingAgentPosition;
        public readonly float BlockingAgentRadius;
        public readonly Vector2 SuggestedDetourDirection;

        public AvoidanceResult(
            Vector2 adjustedDirection,
            float speedScale,
            bool hasBlockingAgent,
            bool shouldRepath,
            int blockingAgentId,
            float blockingDistance,
            Vector2 blockingAgentPosition,
            float blockingAgentRadius,
            Vector2 suggestedDetourDirection)
        {
            AdjustedDirection = adjustedDirection;
            SpeedScale = speedScale;
            HasBlockingAgent = hasBlockingAgent;
            ShouldRepath = shouldRepath;
            BlockingAgentId = blockingAgentId;
            BlockingDistance = blockingDistance;
            BlockingAgentPosition = blockingAgentPosition;
            BlockingAgentRadius = blockingAgentRadius;
            SuggestedDetourDirection = suggestedDetourDirection;
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
            return new AvoidanceResult(desiredDirection, 1f, false, false, 0, float.PositiveInfinity, Vector2.zero, 0f, Vector2.zero);
        }

        Vector2 desired = desiredDirection.normalized;
        Vector2 avoidance = Vector2.zero;
        int blockingAgentId = 0;
        float nearestBlockingDistance = float.PositiveInfinity;
        Vector2 blockingAgentPosition = Vector2.zero;
        float blockingAgentRadius = 0f;
        Vector2 detourDirection = Vector2.zero;
        bool shouldRepath = false;
        float speedScale = 1f;

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
            float sidestepWeight = shouldYield ? 1.75f : 0.65f;
            avoidance += sidestep * weight * sidestepWeight;

            // 近距离动态阻挡时，除了侧绕还要主动减前冲，否则会继续把对方推走。
            if (shouldYield)
            {
                float slowDownWeight = Mathf.Lerp(0.2f, 1f, lateralFactor);
                avoidance += (-desired) * weight * slowDownWeight;
                float localSpeedScale = Mathf.Clamp01(forwardDistance / Mathf.Max(interactionRadius * 1.6f, 0.001f));
                speedScale = Mathf.Min(speedScale, localSpeedScale);
            }

            if (treatAsBlockingObstacle || shouldYield)
            {
                if (forwardDistance < nearestBlockingDistance)
                {
                    nearestBlockingDistance = forwardDistance;
                    blockingAgentId = other.InstanceId;
                    blockingAgentPosition = other.Position;
                    blockingAgentRadius = other.AvoidanceRadius;
                    detourDirection = sidestep;
                }

                if ((treatAsBlockingObstacle || shouldYield) && forwardDistance <= interactionRadius * 1.35f)
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
        return new AvoidanceResult(
            adjustedDirection,
            speedScale,
            hasBlockingAgent,
            shouldRepath,
            blockingAgentId,
            nearestBlockingDistance,
            blockingAgentPosition,
            blockingAgentRadius,
            detourDirection);
    }
}
