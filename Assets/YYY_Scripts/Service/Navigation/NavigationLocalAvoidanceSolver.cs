using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 第一版共享局部避让求解器。
/// 采用优先级 + 前向探测 + 简单 steering / 侧绕混合机制。
/// </summary>
public static class NavigationLocalAvoidanceSolver
{
    private const float NpcPeerYieldPredictedSidestepWeight = 2.55f;
    private const float NpcPeerYieldHeadOnSidestepWeight = 2.35f;
    private const float NpcPeerYieldCruiseSidestepWeight = 2.05f;
    private const float NpcPeerYieldPredictedMinSpeedScale = 0.22f;
    private const float NpcPeerYieldNearContactMinSpeedScale = 0.26f;
    private const float NpcPeerYieldCruiseMinSpeedScale = 0.3f;
    private const float NpcPeerHoldCourseNearContactSidestepWeight = 0.18f;
    private const float NpcPeerHoldCourseCruiseSidestepWeight = 0.06f;

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

    public readonly struct CloseRangeConstraintResult
    {
        public readonly Vector2 ConstrainedDirection;
        public readonly float SpeedScale;
        public readonly bool Applied;
        public readonly bool HardBlocked;
        public readonly float Clearance;
        public readonly float ForwardIntoBlocker;

        public CloseRangeConstraintResult(
            Vector2 constrainedDirection,
            float speedScale,
            bool applied,
            bool hardBlocked,
            float clearance,
            float forwardIntoBlocker)
        {
            ConstrainedDirection = constrainedDirection;
            SpeedScale = speedScale;
            Applied = applied;
            HardBlocked = hardBlocked;
            Clearance = clearance;
            ForwardIntoBlocker = forwardIntoBlocker;
        }
    }

    public static float GetRecommendedLookAhead(
        NavigationAgentSnapshot self,
        float baseLookAheadDistance,
        float desiredSpeed)
    {
        float referenceSpeed = Mathf.Max(self.Velocity.magnitude, desiredSpeed);
        float dynamicLookAhead = self.AvoidanceRadius + referenceSpeed * 0.45f;
        return Mathf.Max(baseLookAheadDistance, dynamicLookAhead);
    }

    public static CloseRangeConstraintResult ApplyCloseRangeConstraint(
        Vector2 currentPosition,
        Vector2 desiredDirection,
        float speedScale,
        float selfRadius,
        float padding,
        AvoidanceResult avoidance)
    {
        return ApplyCloseRangeConstraint(
            currentPosition,
            desiredDirection,
            speedScale,
            selfRadius,
            padding,
            avoidance,
            null);
    }

    public static CloseRangeConstraintResult ApplyCloseRangeConstraint(
        Vector2 currentPosition,
        Vector2 desiredDirection,
        float speedScale,
        float selfRadius,
        float padding,
        AvoidanceResult avoidance,
        float? measuredClearance)
    {
        if (!avoidance.HasBlockingAgent || desiredDirection.sqrMagnitude < 0.0001f)
        {
            return new CloseRangeConstraintResult(desiredDirection, speedScale, false, false, float.PositiveInfinity, 0f);
        }

        Vector2 toBlocker = avoidance.BlockingAgentPosition - currentPosition;
        float centerDistance = toBlocker.magnitude;
        float minimumDistance;
        float clearance;
        if (measuredClearance.HasValue && !float.IsNaN(measuredClearance.Value))
        {
            clearance = measuredClearance.Value;
            minimumDistance = Mathf.Max(0.01f, centerDistance - clearance);
        }
        else
        {
            minimumDistance =
                Mathf.Max(0.01f, selfRadius) +
                Mathf.Max(0.01f, avoidance.BlockingAgentRadius) +
                Mathf.Max(0f, padding);
            clearance = centerDistance - minimumDistance;
        }

        float engageDistance = minimumDistance + Mathf.Max(0.05f, padding);

        if (centerDistance > engageDistance)
        {
            return new CloseRangeConstraintResult(desiredDirection, speedScale, false, false, clearance, 0f);
        }

        Vector2 blockerNormal;
        if (centerDistance > 0.0001f)
        {
            blockerNormal = toBlocker / centerDistance;
        }
        else if (avoidance.SuggestedDetourDirection.sqrMagnitude > 0.0001f)
        {
            blockerNormal = -avoidance.SuggestedDetourDirection.normalized;
        }
        else
        {
            blockerNormal = Vector2.right;
        }

        Vector2 desired = desiredDirection.normalized;
        float forwardIntoBlocker = Mathf.Max(0f, Vector2.Dot(desired, blockerNormal));
        float contactFactor = 1f - Mathf.Clamp01((centerDistance - minimumDistance) / Mathf.Max(engageDistance - minimumDistance, 0.001f));
        bool insideContactShell = clearance <= 0f;
        Vector2 tangential = desired - blockerNormal * forwardIntoBlocker;
        if (tangential.sqrMagnitude < 0.0001f)
        {
            tangential = avoidance.SuggestedDetourDirection.sqrMagnitude > 0.0001f
                ? avoidance.SuggestedDetourDirection.normalized
                : Vector2.Perpendicular(blockerNormal).normalized;
        }
        else
        {
            tangential.Normalize();
        }

        Vector2 separationDirection = -blockerNormal;
        if (!insideContactShell && forwardIntoBlocker <= 0.0001f)
        {
            return new CloseRangeConstraintResult(desiredDirection, speedScale, false, false, clearance, 0f);
        }

        if (insideContactShell)
        {
            float overlapDepth = Mathf.Max(0f, -clearance);
            float overlapFactor = Mathf.Clamp01(overlapDepth / Mathf.Max(minimumDistance, 0.001f));

            Vector2 escapeDirection = separationDirection * Mathf.Lerp(1.6f, 2.4f, overlapFactor);
            float desiredEscapeAlignment = Mathf.Max(0f, Vector2.Dot(desired, separationDirection));
            if (desiredEscapeAlignment > 0.0001f)
            {
                escapeDirection += desired * Mathf.Lerp(0.18f, 0.4f, desiredEscapeAlignment);
            }

            float tangentialWeight = avoidance.SuggestedDetourDirection.sqrMagnitude > 0.0001f
                ? Mathf.Lerp(0.2f, 0.55f, 1f - overlapFactor)
                : Mathf.Lerp(0.12f, 0.35f, 1f - overlapFactor);
            escapeDirection += tangential * tangentialWeight;

            Vector2 escapeConstrainedDirection = escapeDirection.sqrMagnitude > 0.0001f
                ? escapeDirection.normalized
                : separationDirection;

            float separationAlignment = Vector2.Dot(escapeConstrainedDirection, separationDirection);
            if (separationAlignment < 0.72f)
            {
                escapeConstrainedDirection = (separationDirection * 2f + tangential * 0.35f).normalized;
            }

            float escapeMaxSpeed = Mathf.Lerp(0.18f, 0.08f, overlapFactor);
            float escapeMinSpeed = Mathf.Lerp(0.06f, 0.02f, overlapFactor);
            float escapeConstrainedSpeedScale = Mathf.Clamp(
                Mathf.Min(speedScale, escapeMaxSpeed),
                escapeMinSpeed,
                escapeMaxSpeed);

            bool escapeHardBlocked =
                forwardIntoBlocker >= 0.32f &&
                clearance <= -Mathf.Max(0.02f, padding * 0.25f);
            if (escapeHardBlocked)
            {
                escapeConstrainedSpeedScale = 0f;
            }

            return new CloseRangeConstraintResult(
                escapeConstrainedDirection,
                escapeConstrainedSpeedScale,
                true,
                escapeHardBlocked,
                clearance,
                forwardIntoBlocker);
        }

        float blend = Mathf.Clamp01(contactFactor * Mathf.Lerp(0.55f, 1f, forwardIntoBlocker));
        Vector2 constrainedBasis = tangential + separationDirection * Mathf.Lerp(0.15f, 0.4f, contactFactor);
        constrainedBasis = constrainedBasis.sqrMagnitude > 0.0001f ? constrainedBasis.normalized : tangential;
        Vector2 constrainedDirection = Vector2.Lerp(desired, constrainedBasis, blend).normalized;
        if (constrainedDirection.sqrMagnitude < 0.0001f)
        {
            constrainedDirection = constrainedBasis;
        }

        float constrainedSpeedScale = Mathf.Min(speedScale, Mathf.Lerp(speedScale, 0.05f, contactFactor));
        bool hardBlocked =
            contactFactor >= 0.92f &&
            forwardIntoBlocker >= 0.55f &&
            clearance <= Mathf.Max(0.02f, padding * 0.35f);
        if (hardBlocked)
        {
            constrainedSpeedScale = Mathf.Min(constrainedSpeedScale, 0.025f);
        }

        return new CloseRangeConstraintResult(
            constrainedDirection,
            constrainedSpeedScale,
            true,
            hardBlocked,
            clearance,
            forwardIntoBlocker);
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

            float interactionRadius = NavigationAvoidanceRules.GetInteractionRadius(self, other);
            Vector2 offset = other.Position - self.Position;
            float currentForwardDistance = Vector2.Dot(offset, desired);
            float selfSpeed = Mathf.Max(self.Velocity.magnitude, 1.5f);
            Vector2 otherVelocity =
                other.IsCurrentlyMoving && other.Velocity.sqrMagnitude > 0.0001f
                    ? other.Velocity
                    : Vector2.zero;
            float predictionHorizon = Mathf.Clamp(
                lookAheadDistance / Mathf.Max(selfSpeed + otherVelocity.magnitude, 0.001f),
                0.18f,
                0.85f);
            bool hasPredictedConflict = false;
            float timeToClosest = 0f;
            Vector2 steeringOffset = offset;
            float steeringDistance = offset.magnitude;

            if (otherVelocity.sqrMagnitude > 0.0001f)
            {
                Vector2 selfVelocityEstimate = desired * selfSpeed;
                Vector2 relativeVelocity = otherVelocity - selfVelocityEstimate;
                float relativeSpeedSqr = relativeVelocity.sqrMagnitude;
                if (relativeSpeedSqr > 0.0001f)
                {
                    float rawTimeToClosest = -Vector2.Dot(offset, relativeVelocity) / relativeSpeedSqr;
                    if (rawTimeToClosest > 0f)
                    {
                        timeToClosest = Mathf.Min(rawTimeToClosest, predictionHorizon);
                        Vector2 futureOffset = offset + relativeVelocity * timeToClosest;
                        float futureDistance = futureOffset.magnitude;
                        float predictionRadius = interactionRadius * 1.08f;
                        bool converging = Vector2.Dot(offset, relativeVelocity) < -0.01f;
                        if (converging && rawTimeToClosest <= predictionHorizon && futureDistance <= predictionRadius)
                        {
                            hasPredictedConflict = true;
                            steeringOffset = futureOffset;
                            steeringDistance = futureDistance;
                        }
                    }
                }
            }

            float forwardDistance = Vector2.Dot(steeringOffset, desired);
            if (!hasPredictedConflict && (forwardDistance < 0f || forwardDistance > lookAheadDistance))
            {
                continue;
            }

            if (hasPredictedConflict)
            {
                forwardDistance = Mathf.Clamp(forwardDistance, 0f, lookAheadDistance);
            }

            float lateralAllowance = interactionRadius * (hasPredictedConflict ? 1.15f : 1f);
            Vector2 lateral = steeringOffset - desired * forwardDistance;
            float lateralDistance = lateral.magnitude;
            if (lateralDistance > lateralAllowance)
            {
                continue;
            }

            bool treatAsBlockingObstacle = NavigationAvoidanceRules.ShouldTreatAsBlockingObstacle(other);
            bool shouldYield = NavigationAvoidanceRules.ShouldYield(self, other);
            bool yieldToDynamicAgent = shouldYield && !treatAsBlockingObstacle;
            bool sleepingBlocker = treatAsBlockingObstacle && other.IsNavigationSleeping;
            bool stationaryBlocker = treatAsBlockingObstacle && !other.IsCurrentlyMoving;
            bool playerAgainstPassiveNpcBlocker =
                self.UnitType == NavigationUnitType.Player &&
                other.UnitType == NavigationUnitType.NPC &&
                !other.IsCurrentlyMoving;

            float centerDistance = steeringDistance;
            float clearance = centerDistance - interactionRadius;
            bool nearContact = clearance <= Mathf.Max(0.08f, interactionRadius * 0.15f);
            bool predictedYieldConflict = hasPredictedConflict && yieldToDynamicAgent;
            float passiveNpcApproachFactor = 1f;
            if (playerAgainstPassiveNpcBlocker)
            {
                const float PassiveNpcSoftAvoidanceStartClearance = 0.24f;
                const float PassiveNpcSoftAvoidanceRamp = 0.14f;
                passiveNpcApproachFactor = Mathf.Clamp01(
                    (PassiveNpcSoftAvoidanceStartClearance - clearance) /
                    PassiveNpcSoftAvoidanceRamp);
            }

            float forwardFactor = 1f - Mathf.Clamp01(forwardDistance / Mathf.Max(lookAheadDistance, 0.001f));
            float lateralFactor = 1f - Mathf.Clamp01(lateralDistance / Mathf.Max(lateralAllowance, 0.001f));
            float weight = forwardFactor * lateralFactor;
            if (hasPredictedConflict)
            {
                float timeFactor = 1f - Mathf.Clamp01(timeToClosest / Mathf.Max(predictionHorizon, 0.001f));
                float predictionFactor =
                    1f - Mathf.Clamp01(centerDistance / Mathf.Max(interactionRadius * 1.08f, 0.001f));
                weight = Mathf.Max(weight, Mathf.Clamp01(0.35f + timeFactor * 0.65f + predictionFactor * 0.45f));
            }

            bool npcPeerCrossing =
                self.UnitType == NavigationUnitType.NPC &&
                other.UnitType == NavigationUnitType.NPC;
            Vector2 otherHeading =
                otherVelocity.sqrMagnitude > 0.0001f
                    ? otherVelocity.normalized
                    : Vector2.zero;
            bool headOnPeerCrossing =
                npcPeerCrossing &&
                otherHeading.sqrMagnitude > 0.0001f &&
                Vector2.Dot(desired, otherHeading) <= -0.45f;

            bool peerHoldCourse = npcPeerCrossing && !shouldYield;
            bool holdCoursePeerAwareness =
                peerHoldCourse &&
                currentForwardDistance >= 0f &&
                currentForwardDistance <= interactionRadius * 1.45f &&
                lateralDistance <= interactionRadius * 0.3f;
            Vector2 sidestepAxis = Vector2.Perpendicular(desired).normalized;
            Vector2 sidestep;
            if (headOnPeerCrossing)
            {
                float pairSide = ((self.InstanceId ^ other.InstanceId) & 1) == 0 ? 1f : -1f;
                sidestep = sidestepAxis * pairSide;
            }
            else
            {
                float side = Mathf.Sign(Vector2.Dot(sidestepAxis, steeringOffset));
                if (Mathf.Approximately(side, 0f))
                {
                    side = (other.InstanceId & 1) == 0 ? 1f : -1f;
                }

                sidestep = sidestepAxis * -side;
            }
            float sidestepWeight;
            if (yieldToDynamicAgent || treatAsBlockingObstacle)
            {
                if (yieldToDynamicAgent && npcPeerCrossing)
                {
                    sidestepWeight = predictedYieldConflict
                        ? NpcPeerYieldPredictedSidestepWeight
                        : (headOnPeerCrossing ? NpcPeerYieldHeadOnSidestepWeight : NpcPeerYieldCruiseSidestepWeight);
                }
                else
                {
                    sidestepWeight = predictedYieldConflict
                        ? 2.25f
                            : (headOnPeerCrossing
                                ? 2.05f
                                : (playerAgainstPassiveNpcBlocker
                                    ? 1.45f
                                    : (sleepingBlocker
                                        ? 2.55f
                                        : (treatAsBlockingObstacle ? 2.15f : 1.75f))));
                }
            }
            else if (nearContact)
            {
                sidestepWeight = peerHoldCourse
                    ? (holdCoursePeerAwareness ? NpcPeerHoldCourseNearContactSidestepWeight : 0.1f)
                    : (headOnPeerCrossing ? 0.08f : (npcPeerCrossing ? 0.42f : 1.1f));
            }
            else
            {
                sidestepWeight = peerHoldCourse
                    ? NpcPeerHoldCourseCruiseSidestepWeight
                    : (headOnPeerCrossing ? 0.02f : (npcPeerCrossing ? 0.18f : 0.65f));
            }

            if (playerAgainstPassiveNpcBlocker)
            {
                sidestepWeight *= passiveNpcApproachFactor;
            }

            avoidance += sidestep * weight * sidestepWeight;

            // 近距离动态阻挡时，除了侧绕还要主动减前冲，否则会继续把对方推走。
            if (yieldToDynamicAgent)
            {
                float clearanceBuffer = Mathf.Max(
                    predictedYieldConflict ? 0.22f : 0.12f,
                    interactionRadius * (predictedYieldConflict ? 0.26f : 0.14f));
                float clearanceRatio = Mathf.Clamp01(
                    (clearance - clearanceBuffer) /
                    Mathf.Max(interactionRadius * (predictedYieldConflict ? 0.95f : 0.8f), 0.001f));
                float slowDownWeight = Mathf.Lerp(
                    predictedYieldConflict ? 0.85f : 0.42f,
                    predictedYieldConflict ? 1.55f : 1.05f,
                    1f - clearanceRatio);
                avoidance += (-desired) * weight * slowDownWeight;
                float distanceSpeedScale = Mathf.Clamp01(
                    forwardDistance /
                    Mathf.Max(interactionRadius * (predictedYieldConflict ? 1.9f : 1.45f), 0.001f));
                float clearanceSpeedScale = Mathf.Lerp(predictedYieldConflict ? 0.02f : 0.08f, 1f, clearanceRatio);
                float localSpeedScale = Mathf.Min(distanceSpeedScale, clearanceSpeedScale);
                if (predictedYieldConflict)
                {
                    float timeSpeedScale = Mathf.Lerp(
                        0.02f,
                        1f,
                        Mathf.Clamp01(timeToClosest / Mathf.Max(predictionHorizon, 0.001f)));
                    localSpeedScale = Mathf.Min(localSpeedScale, timeSpeedScale);
                }

                if (npcPeerCrossing)
                {
                    float peerYieldMinSpeedScale = predictedYieldConflict
                        ? NpcPeerYieldPredictedMinSpeedScale
                        : (nearContact ? NpcPeerYieldNearContactMinSpeedScale : NpcPeerYieldCruiseMinSpeedScale);
                    localSpeedScale = Mathf.Max(localSpeedScale, peerYieldMinSpeedScale);
                }

                speedScale = Mathf.Min(speedScale, localSpeedScale);
            }
            else if (treatAsBlockingObstacle)
            {
                float blockerClearanceBuffer = Mathf.Max(
                    sleepingBlocker ? 0.12f : 0.14f,
                    interactionRadius * (sleepingBlocker ? 0.1f : 0.12f));
                float blockerClearanceRatio = Mathf.Clamp01(
                    (clearance - blockerClearanceBuffer) /
                    Mathf.Max(interactionRadius * (sleepingBlocker ? 0.65f : 0.7f), 0.001f));
                float blockerPressure = 1f - blockerClearanceRatio;
                float blockerSlowDownWeight = Mathf.Lerp(
                    playerAgainstPassiveNpcBlocker ? 0.16f : (sleepingBlocker ? 0.22f : 0.28f),
                    playerAgainstPassiveNpcBlocker ? 0.5f : (sleepingBlocker ? 0.75f : 0.9f),
                    blockerPressure);
                blockerSlowDownWeight *= passiveNpcApproachFactor;
                avoidance += (-desired) * weight * blockerSlowDownWeight;

                float blockerDistanceSpeedScale = Mathf.Clamp01(
                    forwardDistance /
                    Mathf.Max(
                        interactionRadius * (playerAgainstPassiveNpcBlocker ? 2.35f : (sleepingBlocker ? 3f : 1.6f)),
                        0.001f));
                float blockerClearanceSpeedScale = Mathf.Lerp(
                    playerAgainstPassiveNpcBlocker ? 0.42f : (sleepingBlocker ? 0.32f : 0.24f),
                    1f,
                    blockerClearanceRatio);
                float blockerSpeedScale = Mathf.Min(blockerDistanceSpeedScale, blockerClearanceSpeedScale);
                if (playerAgainstPassiveNpcBlocker)
                {
                    blockerSpeedScale = Mathf.Lerp(1f, blockerSpeedScale, passiveNpcApproachFactor);
                }

                speedScale = Mathf.Min(speedScale, blockerSpeedScale);
            }
            else if (nearContact)
            {
                float holdCourseBackoffWeight = holdCoursePeerAwareness ? 0.04f : 0.08f;
                avoidance += (-desired) * weight * (peerHoldCourse ? holdCourseBackoffWeight : (headOnPeerCrossing ? 0.08f : 0.35f));
                float nonYieldSpeedScale = Mathf.Clamp01(Mathf.Max(clearance, 0f) / Mathf.Max(interactionRadius * 0.5f, 0.001f));
                speedScale = Mathf.Min(
                    speedScale,
                    peerHoldCourse || headOnPeerCrossing
                        ? Mathf.Lerp(holdCoursePeerAwareness ? 0.82f : 0.78f, 0.96f, nonYieldSpeedScale)
                        : Mathf.Lerp(0.18f, 0.45f, nonYieldSpeedScale));
            }

            bool trackAsBlockingAgent =
                treatAsBlockingObstacle ||
                shouldYield ||
                nearContact ||
                hasPredictedConflict ||
                holdCoursePeerAwareness;
            if (trackAsBlockingAgent)
            {
                float repathForwardDistance = hasPredictedConflict
                    ? forwardDistance
                    : currentForwardDistance;
                float blockingMetric = repathForwardDistance >= 0f ? repathForwardDistance : centerDistance;
                if (blockingMetric < nearestBlockingDistance)
                {
                    nearestBlockingDistance = blockingMetric;
                    blockingAgentId = other.InstanceId;
                    blockingAgentPosition = other.Position;
                    blockingAgentRadius = Mathf.Max(other.ColliderRadius, interactionRadius - Mathf.Max(self.ColliderRadius, 0.01f));
                    detourDirection = sidestep;
                }

                float repathLaneThreshold = interactionRadius *
                    (yieldToDynamicAgent
                        ? (predictedYieldConflict ? 0.92f : 0.78f)
                        : (playerAgainstPassiveNpcBlocker
                            ? 0.56f
                            : (sleepingBlocker ? 0.62f : (stationaryBlocker ? 0.72f : (treatAsBlockingObstacle ? 0.76f : 0.55f)))));
                float repathClearanceThreshold = yieldToDynamicAgent
                    ? Mathf.Max(
                        predictedYieldConflict ? 0.32f : 0.22f,
                        interactionRadius * (predictedYieldConflict ? 0.34f : 0.24f))
                    : (treatAsBlockingObstacle
                        ? (playerAgainstPassiveNpcBlocker
                            ? Mathf.Max(0.08f, interactionRadius * 0.12f)
                            : (sleepingBlocker
                                ? Mathf.Max(0.14f, interactionRadius * 0.14f)
                                : Mathf.Max(0.2f, interactionRadius * 0.22f)))
                        : Mathf.Max(0.16f, interactionRadius * 0.2f));
                bool blockerStillOnCorridor = lateralDistance <= repathLaneThreshold;
                float blockerForwardReach = interactionRadius *
                    (yieldToDynamicAgent
                        ? (predictedYieldConflict ? 1.7f : 1.35f)
                        : (playerAgainstPassiveNpcBlocker ? 1.1f : (sleepingBlocker ? 1.85f : (stationaryBlocker ? 1.6f : 1.2f))));

                if (sleepingBlocker &&
                    !playerAgainstPassiveNpcBlocker &&
                    blockerStillOnCorridor &&
                    repathForwardDistance >= 0f &&
                    repathForwardDistance <= interactionRadius * 2.4f)
                {
                    shouldRepath = true;
                }
                else if ((treatAsBlockingObstacle || yieldToDynamicAgent) &&
                    repathForwardDistance >= 0f &&
                    repathForwardDistance <= blockerForwardReach &&
                    blockerStillOnCorridor &&
                    clearance <= repathClearanceThreshold)
                {
                    shouldRepath = true;
                }
                else if (treatAsBlockingObstacle &&
                         blockerStillOnCorridor &&
                         currentForwardDistance >= -interactionRadius * 0.05f &&
                         speedScale <= (playerAgainstPassiveNpcBlocker ? 0.44f : (sleepingBlocker ? 0.72f : 0.58f)) &&
                         clearance <= (playerAgainstPassiveNpcBlocker
                            ? Mathf.Max(0.12f, interactionRadius * 0.14f)
                            : (sleepingBlocker ? Mathf.Max(0.18f, interactionRadius * 0.18f) : Mathf.Max(0.24f, interactionRadius * 0.24f))))
                {
                    // 静止阻挡体不会自己让开；若仍被压成慢蹭，应尽快转入 detour/repath。
                    shouldRepath = true;
                }
                else if (nearContact && !peerHoldCourse)
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
