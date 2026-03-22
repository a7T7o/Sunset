using UnityEngine;

/// <summary>
/// 共享导航核心使用的只读代理快照。
/// </summary>
public readonly struct NavigationAgentSnapshot
{
    public readonly int InstanceId;
    public readonly NavigationUnitType UnitType;
    public readonly Vector2 Position;
    public readonly Vector2 Velocity;
    public readonly float ColliderRadius;
    public readonly float AvoidanceRadius;
    public readonly int AvoidancePriority;
    public readonly bool IsCurrentlyMoving;
    public readonly bool IsNavigationSleeping;
    public readonly bool ParticipatesInLocalAvoidance;

    public bool IsValid => InstanceId != 0;

    public NavigationAgentSnapshot(
        int instanceId,
        NavigationUnitType unitType,
        Vector2 position,
        Vector2 velocity,
        float colliderRadius,
        float avoidanceRadius,
        int avoidancePriority,
        bool isCurrentlyMoving,
        bool isNavigationSleeping,
        bool participatesInLocalAvoidance)
    {
        InstanceId = instanceId;
        UnitType = unitType;
        Position = position;
        Velocity = velocity;
        ColliderRadius = colliderRadius;
        AvoidanceRadius = avoidanceRadius;
        AvoidancePriority = avoidancePriority;
        IsCurrentlyMoving = isCurrentlyMoving;
        IsNavigationSleeping = isNavigationSleeping;
        ParticipatesInLocalAvoidance = participatesInLocalAvoidance;
    }

    public static NavigationAgentSnapshot FromUnit(INavigationUnit unit)
    {
        if (unit == null)
        {
            return default;
        }

        Component component = unit as Component;
        int instanceId = component != null ? component.GetInstanceID() : unit.GetHashCode();

        return new NavigationAgentSnapshot(
            instanceId,
            unit.GetUnitType(),
            unit.GetPosition(),
            unit.GetCurrentVelocity(),
            Mathf.Max(0.01f, unit.GetColliderRadius()),
            Mathf.Max(0.01f, unit.GetAvoidanceRadius()),
            unit.GetAvoidancePriority(),
            unit.IsCurrentlyMoving(),
            unit.IsNavigationSleeping(),
            unit.ParticipatesInLocalAvoidance());
    }
}
