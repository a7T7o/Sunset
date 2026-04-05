using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家/NPC 共用的 traversal contract 核心：
/// 统一脚底 probe、bridge support、bounds constraint、occupancy 判定和 soft-pass 触发口径。
/// 不负责“谁给目标”或“什么时候算完成”。
/// </summary>
public static class NavigationTraversalCore
{
    public readonly struct Contract
    {
        public readonly NavGrid2D NavGrid;
        public readonly Collider2D NavigationCollider;
        public readonly float FootProbeVerticalInset;
        public readonly float FootProbeSideInset;
        public readonly float FootProbeExtraRadius;

        public Contract(
            NavGrid2D navGrid,
            Collider2D navigationCollider,
            float footProbeVerticalInset,
            float footProbeSideInset,
            float footProbeExtraRadius)
        {
            NavGrid = navGrid;
            NavigationCollider = navigationCollider;
            FootProbeVerticalInset = footProbeVerticalInset;
            FootProbeSideInset = footProbeSideInset;
            FootProbeExtraRadius = footProbeExtraRadius;
        }
    }

    public readonly struct ProbePoints
    {
        public readonly Vector2 Center;
        public readonly Vector2 Left;
        public readonly Vector2 Right;

        public ProbePoints(Vector2 center, Vector2 left, Vector2 right)
        {
            Center = center;
            Left = left;
            Right = right;
        }
    }

    public static Collider2D[] NormalizeColliderArray(Collider2D[] source)
    {
        if (source == null || source.Length == 0)
        {
            return new Collider2D[0];
        }

        List<Collider2D> normalized = new List<Collider2D>(source.Length);
        for (int index = 0; index < source.Length; index++)
        {
            Collider2D collider = source[index];
            if (collider == null || normalized.Contains(collider))
            {
                continue;
            }

            normalized.Add(collider);
        }

        return normalized.ToArray();
    }

    public static ProbePoints GetNavigationProbePoints(in Contract contract, Vector2 worldCenter)
    {
        Vector2 centerProbe = GetFootProbeCenter(contract, worldCenter);
        float lateralOffset = GetLateralFootProbeOffset(contract);
        if (lateralOffset <= 0.03f)
        {
            return new ProbePoints(centerProbe, centerProbe, centerProbe);
        }

        return new ProbePoints(
            centerProbe,
            centerProbe + Vector2.left * lateralOffset,
            centerProbe + Vector2.right * lateralOffset);
    }

    public static bool CanOccupyNavigationPoint(in Contract contract, Vector2 worldCenter)
    {
        if (contract.NavGrid == null)
        {
            return true;
        }

        float queryRadius = GetNavigationPointQueryRadius(contract);
        ProbePoints probePoints = GetNavigationProbePoints(contract, worldCenter);
        bool centerWalkable = contract.NavGrid.IsWalkable(probePoints.Center, queryRadius, contract.NavigationCollider);
        if (!centerWalkable)
        {
            return false;
        }

        bool leftIsDistinct = (probePoints.Left - probePoints.Center).sqrMagnitude > 0.0009f;
        bool rightIsDistinct = (probePoints.Right - probePoints.Center).sqrMagnitude > 0.0009f;
        if (!leftIsDistinct && !rightIsDistinct)
        {
            return true;
        }

        bool leftWalkable = !leftIsDistinct || contract.NavGrid.IsWalkable(probePoints.Left, queryRadius, contract.NavigationCollider);
        bool rightWalkable = !rightIsDistinct || contract.NavGrid.IsWalkable(probePoints.Right, queryRadius, contract.NavigationCollider);
        if (IsTraversalBridgeCenterSupported(contract, probePoints.Center, queryRadius))
        {
            return leftWalkable || rightWalkable;
        }

        return leftWalkable && rightWalkable;
    }

    public static bool ShouldEnableTraversalSoftPass(
        in Contract contract,
        Vector2 worldCenter,
        bool allowTraversalOverridePhysicsSoftPass,
        Collider2D[] traversalSoftPassBlockers)
    {
        if (!allowTraversalOverridePhysicsSoftPass ||
            contract.NavGrid == null ||
            contract.NavigationCollider == null ||
            traversalSoftPassBlockers == null ||
            traversalSoftPassBlockers.Length == 0)
        {
            return false;
        }

        float queryRadius = GetTraversalSupportQueryRadius(GetNavigationPointQueryRadius(contract));
        ProbePoints probePoints = GetTraversalSupportProbePoints(contract, worldCenter);
        bool leftIsDistinct = (probePoints.Left - probePoints.Center).sqrMagnitude > 0.0009f;
        bool rightIsDistinct = (probePoints.Right - probePoints.Center).sqrMagnitude > 0.0009f;
        if (!contract.NavGrid.HasWalkableOverrideAt(probePoints.Center, queryRadius))
        {
            return false;
        }

        bool leftSupported = !leftIsDistinct || contract.NavGrid.HasWalkableOverrideAt(probePoints.Left, queryRadius);
        bool rightSupported = !rightIsDistinct || contract.NavGrid.HasWalkableOverrideAt(probePoints.Right, queryRadius);
        if (!leftIsDistinct && !rightIsDistinct)
        {
            return true;
        }

        return leftSupported || rightSupported;
    }

    public static bool TryResolveOccupiableDestination(
        in Contract contract,
        Vector2 sampledDestination,
        out Vector2 resolvedDestination)
    {
        resolvedDestination = ClampToWorldBounds(contract, sampledDestination);
        if (CanOccupyNavigationPoint(contract, resolvedDestination))
        {
            return true;
        }

        if (contract.NavGrid == null)
        {
            return false;
        }

        if (!contract.NavGrid.TryFindNearestWalkable(resolvedDestination, out Vector2 walkableDestination, contract.NavigationCollider))
        {
            return false;
        }

        if (!CanOccupyNavigationPoint(contract, walkableDestination))
        {
            return false;
        }

        resolvedDestination = walkableDestination;
        return true;
    }

    public static Vector2 ConstrainVelocityToNavigationBounds(
        in Contract contract,
        Vector2 currentCenter,
        Vector2 desiredVelocity,
        float stepDuration)
    {
        if (desiredVelocity.sqrMagnitude <= 0.0001f)
        {
            return Vector2.zero;
        }

        if (contract.NavGrid == null)
        {
            return desiredVelocity;
        }

        Vector2 projectedOffset = desiredVelocity * Mathf.Max(stepDuration, 0.0001f);
        if (CanOccupyNavigationPoint(contract, currentCenter + projectedOffset))
        {
            return desiredVelocity;
        }

        bool prioritizeX = Mathf.Abs(desiredVelocity.x) >= Mathf.Abs(desiredVelocity.y);
        return prioritizeX
            ? ConstrainVelocityByAxes(contract, currentCenter, desiredVelocity, stepDuration, tryXFirst: true)
            : ConstrainVelocityByAxes(contract, currentCenter, desiredVelocity, stepDuration, tryXFirst: false);
    }

    public static Vector2 ConstrainNextPositionToNavigationBounds(
        in Contract contract,
        Vector2 currentPosition,
        Vector2 desiredNextPosition,
        float fallbackExtraDistance = 0f)
    {
        Vector2 desiredOffset = desiredNextPosition - currentPosition;
        if (desiredOffset.sqrMagnitude <= 0.0001f)
        {
            return currentPosition;
        }

        if (contract.NavGrid == null)
        {
            return desiredNextPosition;
        }

        if (CanOccupyNavigationPoint(contract, desiredNextPosition))
        {
            return desiredNextPosition;
        }

        bool prioritizeX = Mathf.Abs(desiredOffset.x) >= Mathf.Abs(desiredOffset.y);
        Vector2 constrainedPosition = prioritizeX
            ? ConstrainPositionByAxes(contract, currentPosition, desiredOffset, tryXFirst: true)
            : ConstrainPositionByAxes(contract, currentPosition, desiredOffset, tryXFirst: false);
        if ((constrainedPosition - currentPosition).sqrMagnitude > 0.0001f)
        {
            return constrainedPosition;
        }

        if (fallbackExtraDistance > 0f &&
            TryResolveConstrainedFallbackPosition(contract, currentPosition, desiredNextPosition, desiredOffset, fallbackExtraDistance, out Vector2 fallbackPosition))
        {
            return fallbackPosition;
        }

        return currentPosition;
    }

    public static Vector2 ClampToWorldBounds(in Contract contract, Vector2 destination)
    {
        return contract.NavGrid != null
            ? contract.NavGrid.ClampToWorldBounds(destination)
            : destination;
    }

    private static Vector2 ConstrainVelocityByAxes(
        in Contract contract,
        Vector2 currentCenter,
        Vector2 desiredVelocity,
        float stepDuration,
        bool tryXFirst)
    {
        Vector2 constrainedVelocity = Vector2.zero;
        Vector2 simulatedCenter = currentCenter;

        TryApplyAxisVelocity(contract, ref constrainedVelocity, ref simulatedCenter, desiredVelocity, stepDuration, applyX: tryXFirst);
        TryApplyAxisVelocity(contract, ref constrainedVelocity, ref simulatedCenter, desiredVelocity, stepDuration, applyX: !tryXFirst);

        return constrainedVelocity;
    }

    private static void TryApplyAxisVelocity(
        in Contract contract,
        ref Vector2 constrainedVelocity,
        ref Vector2 simulatedCenter,
        Vector2 desiredVelocity,
        float stepDuration,
        bool applyX)
    {
        float axisVelocity = applyX ? desiredVelocity.x : desiredVelocity.y;
        if (Mathf.Abs(axisVelocity) <= 0.0001f)
        {
            return;
        }

        Vector2 axisOffset = applyX
            ? new Vector2(axisVelocity * stepDuration, 0f)
            : new Vector2(0f, axisVelocity * stepDuration);

        if (!CanOccupyNavigationPoint(contract, simulatedCenter + axisOffset))
        {
            return;
        }

        simulatedCenter += axisOffset;
        if (applyX)
        {
            constrainedVelocity.x = axisVelocity;
        }
        else
        {
            constrainedVelocity.y = axisVelocity;
        }
    }

    private static Vector2 ConstrainPositionByAxes(
        in Contract contract,
        Vector2 currentPosition,
        Vector2 desiredOffset,
        bool tryXFirst)
    {
        Vector2 constrainedPosition = currentPosition;
        TryApplyAxisOffset(contract, ref constrainedPosition, desiredOffset, applyX: tryXFirst);
        TryApplyAxisOffset(contract, ref constrainedPosition, desiredOffset, applyX: !tryXFirst);
        return constrainedPosition;
    }

    private static void TryApplyAxisOffset(
        in Contract contract,
        ref Vector2 constrainedPosition,
        Vector2 desiredOffset,
        bool applyX)
    {
        float axisOffset = applyX ? desiredOffset.x : desiredOffset.y;
        if (Mathf.Abs(axisOffset) <= 0.0001f)
        {
            return;
        }

        Vector2 offset = applyX
            ? new Vector2(axisOffset, 0f)
            : new Vector2(0f, axisOffset);
        Vector2 candidatePosition = constrainedPosition + offset;
        if (!CanOccupyNavigationPoint(contract, candidatePosition))
        {
            return;
        }

        constrainedPosition = candidatePosition;
    }

    private static bool TryResolveConstrainedFallbackPosition(
        in Contract contract,
        Vector2 currentPosition,
        Vector2 desiredNextPosition,
        Vector2 desiredOffset,
        float fallbackExtraDistance,
        out Vector2 fallbackPosition)
    {
        fallbackPosition = currentPosition;
        if (contract.NavGrid == null)
        {
            return false;
        }

        if (!contract.NavGrid.TryFindNearestWalkable(desiredNextPosition, out Vector2 nearestWalkable, contract.NavigationCollider))
        {
            return false;
        }

        float fallbackDistance = Vector2.Distance(currentPosition, nearestWalkable);
        float maxFallbackDistance = desiredOffset.magnitude + fallbackExtraDistance;
        if (fallbackDistance <= 0.0001f || fallbackDistance > maxFallbackDistance)
        {
            return false;
        }

        if (!CanOccupyNavigationPoint(contract, nearestWalkable))
        {
            return false;
        }

        fallbackPosition = nearestWalkable;
        return true;
    }

    private static ProbePoints GetTraversalSupportProbePoints(in Contract contract, Vector2 worldCenter)
    {
        Vector2 centerProbe = GetFootProbeCenter(contract, worldCenter);
        float lateralOffset = GetLateralFootProbeOffset(contract) * 0.72f;
        if (lateralOffset <= 0.03f)
        {
            return new ProbePoints(centerProbe, centerProbe, centerProbe);
        }

        return new ProbePoints(
            centerProbe,
            centerProbe + Vector2.left * lateralOffset,
            centerProbe + Vector2.right * lateralOffset);
    }

    private static Vector2 GetFootProbeCenter(in Contract contract, Vector2 worldCenter)
    {
        if (contract.NavigationCollider == null)
        {
            return worldCenter;
        }

        Bounds bounds = contract.NavigationCollider.bounds;
        Vector2 currentCenter = bounds.center;
        float footProbeInset = Mathf.Clamp(contract.FootProbeVerticalInset, 0f, bounds.size.y * 0.45f);
        float footProbeY = bounds.min.y + footProbeInset;
        return worldCenter + new Vector2(0f, footProbeY - currentCenter.y);
    }

    private static float GetLateralFootProbeOffset(in Contract contract)
    {
        if (contract.NavigationCollider == null)
        {
            return 0f;
        }

        Bounds bounds = contract.NavigationCollider.bounds;
        float sideInset = Mathf.Clamp(contract.FootProbeSideInset, 0f, bounds.extents.x);
        return Mathf.Max(0.02f, bounds.extents.x - sideInset);
    }

    private static float GetTraversalSupportQueryRadius(float navigationQueryRadius)
    {
        return Mathf.Max(0.03f, navigationQueryRadius * 0.85f);
    }

    private static bool IsTraversalBridgeCenterSupported(in Contract contract, Vector2 footProbeCenter, float navigationQueryRadius)
    {
        float supportRadius = GetTraversalSupportQueryRadius(navigationQueryRadius);
        return contract.NavGrid != null && contract.NavGrid.HasWalkableOverrideAt(footProbeCenter, supportRadius);
    }

    private static float GetNavigationPointQueryRadius(in Contract contract)
    {
        float fallbackRadius = Mathf.Max(0.04f, contract.FootProbeSideInset + contract.FootProbeExtraRadius);
        if (contract.NavGrid == null)
        {
            return fallbackRadius;
        }

        float maxRadius = contract.NavGrid.GetAgentRadius();
        if (contract.NavigationCollider == null)
        {
            return Mathf.Min(maxRadius, fallbackRadius);
        }

        Bounds bounds = contract.NavigationCollider.bounds;
        float sideRadius = Mathf.Max(0.03f, contract.FootProbeSideInset + contract.FootProbeExtraRadius);
        float verticalRadius = Mathf.Max(0.03f, contract.FootProbeVerticalInset + contract.FootProbeExtraRadius);
        float clampedRadius = Mathf.Min(Mathf.Min(bounds.extents.x, bounds.extents.y), Mathf.Max(sideRadius, verticalRadius));
        return Mathf.Min(maxRadius, clampedRadius);
    }
}
