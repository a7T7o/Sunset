using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 运行时共享动态导航代理注册表。
/// </summary>
public static class NavigationAgentRegistry
{
    private static readonly HashSet<INavigationUnit> RegisteredUnits = new HashSet<INavigationUnit>();
    private static readonly List<INavigationUnit> StaleUnits = new List<INavigationUnit>();

    public static void Register(INavigationUnit unit)
    {
        if (unit == null)
        {
            return;
        }

        RegisteredUnits.Add(unit);
    }

    public static void Unregister(INavigationUnit unit)
    {
        if (unit == null)
        {
            return;
        }

        RegisteredUnits.Remove(unit);
    }

    public static void GetNearbySnapshots(INavigationUnit self, Vector2 center, float radius, List<NavigationAgentSnapshot> buffer)
    {
        buffer.Clear();
        StaleUnits.Clear();

        foreach (INavigationUnit unit in RegisteredUnits)
        {
            if (unit == null)
            {
                StaleUnits.Add(unit);
                continue;
            }

            Behaviour behaviour = unit as Behaviour;
            if (behaviour == null)
            {
                continue;
            }

            if (!behaviour || !behaviour.enabled || !behaviour.gameObject.activeInHierarchy)
            {
                if (!behaviour)
                {
                    StaleUnits.Add(unit);
                }
                continue;
            }

            if (ReferenceEquals(unit, self))
            {
                continue;
            }

            NavigationAgentSnapshot snapshot = NavigationAgentSnapshot.FromUnit(unit);
            if (!snapshot.IsValid)
            {
                continue;
            }

            float queryRadius = radius + snapshot.AvoidanceRadius + snapshot.ColliderRadius;
            if ((snapshot.Position - center).sqrMagnitude <= queryRadius * queryRadius)
            {
                buffer.Add(snapshot);
            }
        }

        for (int i = 0; i < StaleUnits.Count; i++)
        {
            RegisteredUnits.Remove(StaleUnits[i]);
        }
    }

    public static void GetRegisteredUnits<T>(List<T> buffer)
        where T : class, INavigationUnit
    {
        buffer.Clear();
        StaleUnits.Clear();

        foreach (INavigationUnit unit in RegisteredUnits)
        {
            if (unit == null)
            {
                StaleUnits.Add(unit);
                continue;
            }

            Behaviour behaviour = unit as Behaviour;
            if (behaviour == null)
            {
                continue;
            }

            if (!behaviour || !behaviour.enabled || !behaviour.gameObject.activeInHierarchy)
            {
                if (!behaviour)
                {
                    StaleUnits.Add(unit);
                }

                continue;
            }

            if (unit is T typedUnit)
            {
                buffer.Add(typedUnit);
            }
        }

        for (int i = 0; i < StaleUnits.Count; i++)
        {
            RegisteredUnits.Remove(StaleUnits[i]);
        }
    }
}
