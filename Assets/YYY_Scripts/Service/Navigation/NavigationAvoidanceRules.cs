using UnityEngine;

/// <summary>
/// 共享动态代理避让的基础规则。
/// </summary>
public static class NavigationAvoidanceRules
{
    public static int GetDefaultPriority(NavigationUnitType unitType)
    {
        switch (unitType)
        {
            case NavigationUnitType.Player:
                return 100;
            case NavigationUnitType.Enemy:
                return 70;
            case NavigationUnitType.NPC:
                return 50;
            case NavigationUnitType.StaticObstacle:
            default:
                return 0;
        }
    }

    public static float GetInteractionRadius(in NavigationAgentSnapshot self, in NavigationAgentSnapshot other)
    {
        return self.AvoidanceRadius + other.AvoidanceRadius;
    }

    public static bool ShouldTreatAsBlockingObstacle(in NavigationAgentSnapshot other)
    {
        return other.UnitType == NavigationUnitType.StaticObstacle ||
               other.IsNavigationSleeping ||
               !other.ParticipatesInLocalAvoidance;
    }

    public static bool ShouldConsiderForLocalAvoidance(in NavigationAgentSnapshot self, in NavigationAgentSnapshot other)
    {
        if (!self.IsValid || !other.IsValid || self.InstanceId == other.InstanceId)
        {
            return false;
        }

        if (other.UnitType == NavigationUnitType.StaticObstacle)
        {
            return false;
        }

        return true;
    }

    public static bool ShouldYield(in NavigationAgentSnapshot self, in NavigationAgentSnapshot other)
    {
        if (ShouldTreatAsBlockingObstacle(other))
        {
            return true;
        }

        if (other.AvoidancePriority > self.AvoidancePriority)
        {
            return true;
        }

        if (other.AvoidancePriority < self.AvoidancePriority)
        {
            return false;
        }

        if (other.UnitType == NavigationUnitType.Player && self.UnitType != NavigationUnitType.Player)
        {
            return true;
        }

        if (self.UnitType == NavigationUnitType.Player && other.UnitType != NavigationUnitType.Player)
        {
            return false;
        }

        return self.InstanceId > other.InstanceId;
    }
}
