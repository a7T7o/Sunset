using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class ToolRuntimeFeedbackTests
{
    [Test]
    public void EnsureRuntimeState_ShouldInitializeWateringCanState_AndClearLegacyDurability()
    {
        Type toolDataType = ResolveTypeOrFail("FarmGame.Data.ToolData");
        Type toolTypeEnum = ResolveTypeOrFail("FarmGame.Data.ToolType");
        Type inventoryItemType = ResolveTypeOrFail("FarmGame.Data.Core.InventoryItem");
        Type runtimeUtilityType = ResolveTypeOrFail("FarmGame.Data.Core.ToolRuntimeUtility");

        ScriptableObject toolData = ScriptableObject.CreateInstance(toolDataType);

        try
        {
            SetFieldOrProperty(toolData, "itemID", 9103);
            SetFieldOrProperty(toolData, "toolType", Enum.Parse(toolTypeEnum, "WateringCan"));
            SetFieldOrProperty(toolData, "waterCapacity", 60);
            SetFieldOrProperty(toolData, "maxDurability", 12);
            SetFieldOrProperty(toolData, "hasDurability", true);

            object runtimeItem = Activator.CreateInstance(inventoryItemType, 9103, 0, 1, 12);
            bool changed = (bool)InvokeStatic(runtimeUtilityType, "EnsureRuntimeState", runtimeItem, toolData);

            Assert.That(changed, Is.True);
            Assert.That((int)GetFieldOrProperty(runtimeItem, "MaxDurability"), Is.EqualTo(-1));
            Assert.That((int)GetFieldOrProperty(runtimeItem, "CurrentDurability"), Is.EqualTo(-1));
            Assert.That(InvokeInstance(runtimeItem, "GetPropertyInt", "watering_max", 0), Is.EqualTo(60));
            Assert.That(InvokeInstance(runtimeItem, "GetPropertyInt", "watering_current", 0), Is.EqualTo(60));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(toolData);
        }
    }

    [Test]
    public void GetWaterCapacity_ShouldFallback_ToExplicitThenDurabilityThenDefault()
    {
        Type toolDataType = ResolveTypeOrFail("FarmGame.Data.ToolData");
        Type toolTypeEnum = ResolveTypeOrFail("FarmGame.Data.ToolType");
        Type runtimeUtilityType = ResolveTypeOrFail("FarmGame.Data.Core.ToolRuntimeUtility");

        ScriptableObject toolData = ScriptableObject.CreateInstance(toolDataType);

        try
        {
            SetFieldOrProperty(toolData, "toolType", Enum.Parse(toolTypeEnum, "WateringCan"));

            SetFieldOrProperty(toolData, "waterCapacity", 80);
            SetFieldOrProperty(toolData, "maxDurability", 25);
            Assert.That(InvokeStatic(runtimeUtilityType, "GetWaterCapacity", toolData), Is.EqualTo(80));

            SetFieldOrProperty(toolData, "waterCapacity", 0);
            SetFieldOrProperty(toolData, "maxDurability", 25);
            Assert.That(InvokeStatic(runtimeUtilityType, "GetWaterCapacity", toolData), Is.EqualTo(25));

            SetFieldOrProperty(toolData, "waterCapacity", 0);
            SetFieldOrProperty(toolData, "maxDurability", 0);
            Assert.That(InvokeStatic(runtimeUtilityType, "GetWaterCapacity", toolData), Is.EqualTo(100));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(toolData);
        }
    }

    [Test]
    public void ToolDataTooltip_ShouldUseRuntimeWaterCapacityFallback()
    {
        Type toolDataType = ResolveTypeOrFail("FarmGame.Data.ToolData");
        Type toolTypeEnum = ResolveTypeOrFail("FarmGame.Data.ToolType");

        ScriptableObject toolData = ScriptableObject.CreateInstance(toolDataType);

        try
        {
            SetFieldOrProperty(toolData, "itemName", "测试水壶");
            SetFieldOrProperty(toolData, "toolType", Enum.Parse(toolTypeEnum, "WateringCan"));
            SetFieldOrProperty(toolData, "energyCost", 2);
            SetFieldOrProperty(toolData, "waterCapacity", 0);
            SetFieldOrProperty(toolData, "maxDurability", 0);

            string tooltip = (string)InvokeInstance(toolData, "GetTooltipText");

            StringAssert.Contains("水量上限: 100", tooltip);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(toolData);
        }
    }

    private static Type ResolveTypeOrFail(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type direct = assembly.GetType(typeName);
            if (direct != null)
            {
                return direct;
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
        MethodInfo method = ResolveMethodByArguments(type, methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, args);
        Assert.IsNotNull(method, $"未找到静态方法：{type.Name}.{methodName}");
        return method.Invoke(null, args);
    }

    private static object InvokeInstance(object target, string methodName, params object[] args)
    {
        MethodInfo method = ResolveMethodByArguments(target.GetType(), methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, args);
        Assert.IsNotNull(method, $"未找到实例方法：{target.GetType().Name}.{methodName}");
        return method.Invoke(target, args);
    }

    private static object GetFieldOrProperty(object target, string name)
    {
        Type type = target.GetType();

        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null)
        {
            return property.GetValue(target);
        }

        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            return field.GetValue(target);
        }

        Assert.Fail($"未找到字段或属性：{type.Name}.{name}");
        return null;
    }

    private static void SetFieldOrProperty(object target, string name, object value)
    {
        Type type = target.GetType();

        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, value);
            return;
        }

        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null)
        {
            property.SetValue(target, value);
            return;
        }

        Assert.Fail($"未找到可写字段或属性：{type.Name}.{name}");
    }

    private static MethodInfo ResolveMethodByArguments(Type type, string methodName, BindingFlags bindingFlags, object[] args)
    {
        MethodInfo fallback = null;
        MethodInfo[] methods = type.GetMethods(bindingFlags);

        for (int index = 0; index < methods.Length; index++)
        {
            MethodInfo method = methods[index];
            if (method.Name != methodName)
            {
                continue;
            }

            fallback ??= method;

            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != args.Length)
            {
                continue;
            }

            bool match = true;
            for (int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                Type parameterType = parameters[parameterIndex].ParameterType;
                if (parameterType.IsByRef)
                {
                    parameterType = parameterType.GetElementType();
                }

                object arg = args[parameterIndex];
                if (arg == null)
                {
                    if (parameterType.IsValueType && Nullable.GetUnderlyingType(parameterType) == null)
                    {
                        match = false;
                        break;
                    }

                    continue;
                }

                if (!parameterType.IsAssignableFrom(arg.GetType()))
                {
                    match = false;
                    break;
                }
            }

            if (match)
            {
                return method;
            }
        }

        return fallback;
    }
}
