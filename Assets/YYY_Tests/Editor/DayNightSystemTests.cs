using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 昼夜光影系统 EditMode 回归测试
/// 重点保护真实行为链，而不是复制实现逻辑。
/// </summary>
[TestFixture]
public class DayNightSystemTests
{
    private const string DayNightManagerAssemblyQualifiedName = "DayNightManager, Assembly-CSharp";
    private const string DayNightConfigAssemblyQualifiedName = "DayNightConfig, Assembly-CSharp";
    private const string TimeManagerAssemblyQualifiedName = "TimeManager, Assembly-CSharp";
    private const string SeasonEnumAssemblyQualifiedName = "SeasonManager+Season, Assembly-CSharp";
    private const string DayNightConfigCreatorAssemblyQualifiedName = "DayNightConfigCreator, Assembly-CSharp-Editor";

    private readonly List<UnityEngine.Object> createdObjects = new List<UnityEngine.Object>();

    [TearDown]
    public void TearDown()
    {
        for (int i = createdObjects.Count - 1; i >= 0; i--)
        {
            if (createdObjects[i] != null)
            {
                UnityEngine.Object.DestroyImmediate(createdObjects[i]);
            }
        }

        createdObjects.Clear();
        SetStaticField(ResolveType(TimeManagerAssemblyQualifiedName), "instance", null);
        SetStaticField(ResolveType(DayNightManagerAssemblyQualifiedName), "instance", null);
    }

    [Test]
    public void ApplyWeatherTint_SunnyTint_ReturnsBaseColor()
    {
        Color baseColor = new Color(0.28f, 0.30f, 0.34f, 1f);
        Color result = InvokeStatic<Color>(
            ResolveType(DayNightManagerAssemblyQualifiedName),
            "ApplyWeatherTint",
            baseColor,
            Color.white,
            0.6f);

        Assert.That(result.r, Is.EqualTo(baseColor.r).Within(0.0001f));
        Assert.That(result.g, Is.EqualTo(baseColor.g).Within(0.0001f));
        Assert.That(result.b, Is.EqualTo(baseColor.b).Within(0.0001f));
    }

    [Test]
    public void ApplyWeatherTint_RainyTint_ShouldNeverBrightenDarkBaseColor()
    {
        Color baseColor = new Color(0.30f, 0.28f, 0.35f, 1f);
        Color rainyTint = new Color(0.55f, 0.55f, 0.60f, 1f);

        Color result = InvokeStatic<Color>(
            ResolveType(DayNightManagerAssemblyQualifiedName),
            "ApplyWeatherTint",
            baseColor,
            rainyTint,
            0.5f);

        Assert.LessOrEqual(result.r, baseColor.r + 0.0001f, "雨天 tint 不应把暗场 R 通道抬亮");
        Assert.LessOrEqual(result.g, baseColor.g + 0.0001f, "雨天 tint 不应把暗场 G 通道抬亮");
        Assert.LessOrEqual(result.b, baseColor.b + 0.0001f, "雨天 tint 不应把暗场 B 通道抬亮");
    }

    [Test]
    public void TimeManager_Sleep_ShouldEmitMorningHourAndMinuteEvents()
    {
        Type timeManagerType = ResolveType(TimeManagerAssemblyQualifiedName);
        Component timeManager = CreateComponent("TimeManagerTest", timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "showDebugInfo", false);

        object springSeason = Enum.Parse(ResolveType(SeasonEnumAssemblyQualifiedName), "Spring");
        SetField(timeManager, "currentYear", 1);
        SetField(timeManager, "currentSeason", springSeason);
        SetField(timeManager, "currentDay", 1);
        SetField(timeManager, "currentHour", 25);
        SetField(timeManager, "currentMinute", 50);

        int observedHour = -1;
        (int hour, int minute) observedMinute = (-1, -1);
        Action<int> hourHandler = hour => observedHour = hour;
        Action<int, int> minuteHandler = (hour, minute) => observedMinute = (hour, minute);

        EventInfo hourEvent = timeManagerType.GetEvent("OnHourChanged", BindingFlags.Public | BindingFlags.Static);
        EventInfo minuteEvent = timeManagerType.GetEvent("OnMinuteChanged", BindingFlags.Public | BindingFlags.Static);
        hourEvent.AddEventHandler(null, hourHandler);
        minuteEvent.AddEventHandler(null, minuteHandler);

        try
        {
            InvokeInstanceMethod(timeManager, "Sleep");
        }
        finally
        {
            hourEvent.RemoveEventHandler(null, hourHandler);
            minuteEvent.RemoveEventHandler(null, minuteHandler);
        }

        Assert.That(observedHour, Is.EqualTo(6), "Sleep 后应补发早上 06:00 的小时事件");
        Assert.That(observedMinute.hour, Is.EqualTo(6), "Sleep 后应补发早上 06:00 的分钟事件（小时部分）");
        Assert.That(observedMinute.minute, Is.EqualTo(0), "Sleep 后应补发早上 06:00 的分钟事件（分钟部分）");
    }

    [Test]
    public void TimeManager_Sleep_ShouldRefreshDayNightCachedProgressImmediately()
    {
        Type timeManagerType = ResolveType(TimeManagerAssemblyQualifiedName);
        Component timeManager = CreateComponent("TimeManagerTest", timeManagerType);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetField(timeManager, "showDebugInfo", false);

        object springSeason = Enum.Parse(ResolveType(SeasonEnumAssemblyQualifiedName), "Spring");
        SetField(timeManager, "currentYear", 1);
        SetField(timeManager, "currentSeason", springSeason);
        SetField(timeManager, "currentDay", 1);
        SetField(timeManager, "currentHour", 25);
        SetField(timeManager, "currentMinute", 50);

        Type dayNightManagerType = ResolveType(DayNightManagerAssemblyQualifiedName);
        Component dayNightManager = CreateComponent("DayNightManagerTest", dayNightManagerType);
        SetStaticField(dayNightManagerType, "instance", dayNightManager);
        SetField(dayNightManager, "showDebugInfo", false);
        SetField(dayNightManager, "config", CreateDayNightConfigAsset());
        InvokeOptionalInstanceMethod(dayNightManager, "Start");

        InvokeInstanceMethod(timeManager, "Sleep");

        float cachedDayProgress = (float)GetField(dayNightManager, "cachedDayProgress");
        bool isTimeJumpTransitioning = (bool)GetField(dayNightManager, "isTimeJumpTransitioning");

        Assert.That(cachedDayProgress, Is.EqualTo(0f).Within(0.0001f), "Sleep 后 DayNightManager 应立即看到次日 06:00 的进度");
        Assert.That(isTimeJumpTransitioning, Is.True, "Sleep 后仍应保留夜色 → 晨光的时间跳跃过渡");
    }

    [Test]
    public void DayNightConfigCreator_SpringGradient_ShouldKeepDaylightPlateauFromNoonToFourPm()
    {
        Type creatorType = ResolveType(DayNightConfigCreatorAssemblyQualifiedName);
        MethodInfo method = creatorType.GetMethod("CreateSpringGradient", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method, "未找到 DayNightConfigCreator.CreateSpringGradient");

        Gradient gradient = method.Invoke(null, null) as Gradient;
        Assert.IsNotNull(gradient, "CreateSpringGradient 应返回有效 Gradient");

        float noonBrightness = GetBrightness(gradient.Evaluate(0.30f));
        float afternoonBrightness = GetBrightness(gradient.Evaluate(0.50f));

        Assert.That(noonBrightness - afternoonBrightness, Is.LessThanOrEqualTo(0.05f), "12:00 到 16:00 不应过早明显下坠");
    }

    private ScriptableObject CreateDayNightConfigAsset()
    {
        Type configType = ResolveType(DayNightConfigAssemblyQualifiedName);
        ScriptableObject config = ScriptableObject.CreateInstance(configType);
        createdObjects.Add(config);

        Gradient neutralGradient = CreateGradient(
            new Color(0.95f, 0.95f, 0.95f, 1f),
            new Color(0.95f, 0.95f, 0.95f, 1f));

        SetField(config, "springGradient", neutralGradient);
        SetField(config, "summerGradient", neutralGradient);
        SetField(config, "autumnGradient", neutralGradient);
        SetField(config, "winterGradient", neutralGradient);
        SetField(config, "rainyTint", new Color(0.55f, 0.55f, 0.60f, 1f));
        SetField(config, "witheringTint", new Color(0.85f, 0.75f, 0.45f, 1f));
        SetField(config, "weatherTintStrength", 0.5f);
        SetField(config, "weatherTransitionDuration", 1.5f);
        SetField(config, "timeJumpTransitionDuration", 0.8f);
        SetField(config, "seasonTransitionDuration", 2f);
        SetField(config, "overlayStrengthWithURP", 0.35f);
        SetField(config, "overlayStrengthWithoutURP", 0.92f);

        return config;
    }

    private Gradient CreateGradient(Color startColor, Color endColor)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(startColor, 0f),
                new GradientColorKey(endColor, 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            });
        return gradient;
    }

    private float GetBrightness(Color color)
    {
        return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
    }

    private Component CreateComponent(string name, Type componentType)
    {
        GameObject go = new GameObject(name);
        createdObjects.Add(go);
        Component component = go.AddComponent(componentType);
        Assert.IsNotNull(component, $"未能创建组件: {componentType.FullName}");
        return component;
    }

    private static T InvokeStatic<T>(Type type, string methodName, params object[] args)
    {
        MethodInfo method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method, $"未找到静态方法: {methodName}");
        return (T)method.Invoke(null, args);
    }

    private static void InvokeInstanceMethod(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, $"未找到实例方法: {methodName}");
        method.Invoke(target, args);
    }

    private static void InvokeOptionalInstanceMethod(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
        {
            method.Invoke(target, null);
        }
    }

    private static object GetField(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        Assert.IsNotNull(field, $"未找到字段: {fieldName}");
        return field.GetValue(target);
    }

    private static void SetField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        Assert.IsNotNull(field, $"未找到字段: {fieldName}");
        field.SetValue(target, value);
    }

    private static void SetStaticField(Type type, string fieldName, object value)
    {
        FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(field, $"未找到静态字段: {type.FullName}.{fieldName}");
        field.SetValue(null, value);
    }

    private static Type ResolveType(string assemblyQualifiedName)
    {
        Type type = Type.GetType(assemblyQualifiedName);
        Assert.IsNotNull(type, $"未能解析类型: {assemblyQualifiedName}");
        return type;
    }
}
