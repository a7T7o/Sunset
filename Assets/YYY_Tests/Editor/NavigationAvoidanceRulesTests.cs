using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class NavigationAvoidanceRulesTests
{
    [Test]
    public void AutoNavigationPlayer_ShouldYield_ToMovingNpc()
    {
        Type snapshotType = ResolveTypeOrFail("NavigationAgentSnapshot");
        Type unitType = ResolveTypeOrFail("NavigationUnitType");
        Type rulesType = ResolveTypeOrFail("NavigationAvoidanceRules");

        object player = CreateSnapshot(
            snapshotType,
            unitType,
            1,
            "Player",
            Vector2.zero,
            Vector2.right,
            0.3f,
            0.6f,
            100,
            true,
            false,
            true);

        object npc = CreateSnapshot(
            snapshotType,
            unitType,
            2,
            "NPC",
            new Vector2(1f, 0f),
            Vector2.left,
            0.3f,
            0.6f,
            50,
            true,
            false,
            true);

        bool playerShouldYield = InvokeStaticBool(rulesType, "ShouldYield", player, npc);
        bool npcShouldYield = InvokeStaticBool(rulesType, "ShouldYield", npc, player);

        Assert.That(playerShouldYield, Is.True);
        Assert.That(npcShouldYield, Is.False);
    }

    [Test]
    public void Solver_ShouldProduceLateralOrBackwardBias_WhenPlayerFacesMovingNpc()
    {
        Type snapshotType = ResolveTypeOrFail("NavigationAgentSnapshot");
        Type unitType = ResolveTypeOrFail("NavigationUnitType");
        Type solverType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver");

        object player = CreateSnapshot(
            snapshotType,
            unitType,
            1,
            "Player",
            Vector2.zero,
            Vector2.right,
            0.3f,
            0.6f,
            100,
            true,
            false,
            true);

        object npc = CreateSnapshot(
            snapshotType,
            unitType,
            2,
            "NPC",
            new Vector2(0.7f, 0.05f),
            Vector2.zero,
            0.3f,
            0.6f,
            50,
            true,
            false,
            true);

        Array nearbyAgents = Array.CreateInstance(snapshotType, 1);
        nearbyAgents.SetValue(npc, 0);

        object result = InvokeStatic(solverType, "Solve", player, Vector2.right, 1f, nearbyAgents);
        Assert.IsNotNull(result);

        bool hasBlockingAgent = (bool)GetFieldOrProperty(result, "HasBlockingAgent");
        Vector2 adjustedDirection = (Vector2)GetFieldOrProperty(result, "AdjustedDirection");

        Assert.That(hasBlockingAgent, Is.True);
        Assert.That(adjustedDirection.x, Is.LessThan(0.99f));
        Assert.That(Mathf.Abs(adjustedDirection.y), Is.GreaterThan(0.01f));
    }

    [Test]
    public void CloseRangeConstraint_ShouldClampForwardPush_WhenBlockerIsTooClose()
    {
        Type solverType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver");
        Type avoidanceResultType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver+AvoidanceResult");

        object avoidance = Activator.CreateInstance(
            avoidanceResultType,
            Vector2.right,
            1f,
            true,
            false,
            2,
            0.2f,
            new Vector2(0.62f, 0f),
            0.35f,
            Vector2.up);

        object constraint = InvokeStatic(
            solverType,
            "ApplyCloseRangeConstraint",
            Vector2.zero,
            Vector2.right,
            1f,
            0.35f,
            0.05f,
            avoidance);

        Assert.IsNotNull(constraint);

        bool applied = (bool)GetFieldOrProperty(constraint, "Applied");
        bool hardBlocked = (bool)GetFieldOrProperty(constraint, "HardBlocked");
        Vector2 constrainedDirection = (Vector2)GetFieldOrProperty(constraint, "ConstrainedDirection");
        float constrainedSpeedScale = (float)GetFieldOrProperty(constraint, "SpeedScale");

        Assert.That(applied, Is.True);
        Assert.That(hardBlocked, Is.True);
        Assert.That(constrainedDirection.x, Is.LessThan(0.95f));
        Assert.That(Mathf.Abs(constrainedDirection.y), Is.GreaterThan(0.01f));
        Assert.That(constrainedSpeedScale, Is.LessThan(0.2f));
    }

    private static object CreateSnapshot(
        Type snapshotType,
        Type unitType,
        int instanceId,
        string unitTypeName,
        Vector2 position,
        Vector2 velocity,
        float colliderRadius,
        float avoidanceRadius,
        int avoidancePriority,
        bool isCurrentlyMoving,
        bool isNavigationSleeping,
        bool participatesInLocalAvoidance)
    {
        object enumValue = Enum.Parse(unitType, unitTypeName);
        return Activator.CreateInstance(
            snapshotType,
            instanceId,
            enumValue,
            position,
            velocity,
            colliderRadius,
            avoidanceRadius,
            avoidancePriority,
            isCurrentlyMoving,
            isNavigationSleeping,
            participatesInLocalAvoidance);
    }

    private static Type ResolveTypeOrFail(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(typeName);
            if (type != null)
            {
                return type;
            }

            Type[] candidates;
            try
            {
                candidates = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                candidates = ex.Types;
            }

            foreach (Type candidate in candidates)
            {
                if (candidate != null && (candidate.FullName == typeName || candidate.Name == typeName))
                {
                    return candidate;
                }
            }
        }

        Assert.Fail($"未找到类型：{typeName}");
        return null;
    }

    private static object InvokeStatic(Type type, string methodName, params object[] args)
    {
        MethodInfo method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(method, $"未找到静态方法：{type.Name}.{methodName}");
        return method.Invoke(null, args);
    }

    private static bool InvokeStaticBool(Type type, string methodName, params object[] args)
    {
        return (bool)InvokeStatic(type, methodName, args);
    }

    private static object GetFieldOrProperty(object target, string name)
    {
        Type type = target.GetType();

        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance);
        if (field != null)
        {
            return field.GetValue(target);
        }

        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
        if (property != null)
        {
            return property.GetValue(target);
        }

        Assert.Fail($"未找到字段或属性：{type.Name}.{name}");
        return null;
    }
}
