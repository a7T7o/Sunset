using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 运行时共享动态导航代理注册表。
/// </summary>
public static class NavigationAgentRegistry
{
    private static readonly HashSet<INavigationUnit> RegisteredUnits = new HashSet<INavigationUnit>();
    private static readonly List<INavigationUnit> StaleUnits = new List<INavigationUnit>();
    private static readonly List<INavigationUnit> ActiveUnitsCache = new List<INavigationUnit>();
    private static int ActiveUnitsCacheFrame = -1;
    private static int ActiveUnitsCacheVersion = -1;
    private static int RegistryVersion = 0;

    public static void Register(INavigationUnit unit)
    {
        if (unit == null)
        {
            return;
        }

        RegisteredUnits.Add(unit);
        RegistryVersion++;
    }

    public static void Unregister(INavigationUnit unit)
    {
        if (unit == null)
        {
            return;
        }

        if (RegisteredUnits.Remove(unit))
        {
            RegistryVersion++;
        }
    }

    public static void GetNearbySnapshots(INavigationUnit self, Vector2 center, float radius, List<NavigationAgentSnapshot> buffer)
    {
        buffer.Clear();
        EnsureActiveUnitsCacheCurrent();
        foreach (INavigationUnit unit in ActiveUnitsCache)
        {
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
    }

    public static void GetRegisteredUnits<T>(List<T> buffer)
        where T : class, INavigationUnit
    {
        buffer.Clear();
        EnsureActiveUnitsCacheCurrent();
        foreach (INavigationUnit unit in ActiveUnitsCache)
        {
            if (unit is T typedUnit)
            {
                buffer.Add(typedUnit);
            }
        }
    }

    private static void EnsureActiveUnitsCacheCurrent()
    {
        if (ActiveUnitsCacheFrame == Time.frameCount &&
            ActiveUnitsCacheVersion == RegistryVersion &&
            !CacheHasInvalidEntries())
        {
            return;
        }

        RebuildActiveUnitsCache();
    }

    private static bool CacheHasInvalidEntries()
    {
        for (int index = 0; index < ActiveUnitsCache.Count; index++)
        {
            if (!IsUnitActive(ActiveUnitsCache[index]))
            {
                return true;
            }
        }

        return false;
    }

    private static void RebuildActiveUnitsCache()
    {
        ActiveUnitsCache.Clear();
        StaleUnits.Clear();

        foreach (INavigationUnit unit in RegisteredUnits)
        {
            if (unit == null)
            {
                StaleUnits.Add(unit);
                continue;
            }

            if (!IsUnitActive(unit))
            {
                if (!(unit is Behaviour behaviour) || !behaviour)
                {
                    StaleUnits.Add(unit);
                }

                continue;
            }

            ActiveUnitsCache.Add(unit);
        }

        for (int index = 0; index < StaleUnits.Count; index++)
        {
            RegisteredUnits.Remove(StaleUnits[index]);
        }

        ActiveUnitsCacheFrame = Time.frameCount;
        ActiveUnitsCacheVersion = RegistryVersion;
    }

    private static bool IsUnitActive(INavigationUnit unit)
    {
        if (unit == null)
        {
            return false;
        }

        if (!(unit is Behaviour behaviour))
        {
            return false;
        }

        return behaviour && behaviour.enabled && behaviour.gameObject.activeInHierarchy;
    }
}
