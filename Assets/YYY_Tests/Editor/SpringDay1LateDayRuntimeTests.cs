using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

[TestFixture]
public class SpringDay1LateDayRuntimeTests
{
    private readonly List<GameObject> createdObjects = new();

    [TearDown]
    public void TearDown()
    {
        for (int index = createdObjects.Count - 1; index >= 0; index--)
        {
            if (createdObjects[index] != null)
            {
                UnityEngine.Object.DestroyImmediate(createdObjects[index]);
            }
        }

        createdObjects.Clear();
        TrySetStaticField("Sunset.Story.SpringDay1Director", "_instance", null);
        TrySetStaticField("Sunset.Story.StoryManager", "_instance", null);
        TrySetStaticField("Sunset.Story.SpringDay1PromptOverlay", "_instance", null);
        TrySetStaticField("Sunset.Story.SpringDay1WorkbenchCraftingOverlay", "_instance", null);
        TrySetStaticField("TimeManager", "instance", null);
        TrySetStaticField("EnergySystem", "<Instance>k__BackingField", null);
    }

    [UnityTest]
    public IEnumerator FreeTimeValidationStep_AdvancesFromFinalCallToDayEnd()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object freeTimePhase = ParseEnum(context.storyPhaseType, "FreeTime");
        object dayEndPhase = ParseEnum(context.storyPhaseType, "DayEnd");
        object springSeason = ParseEnum(context.seasonType, "Spring");

        SetPrivateField(context.director, "_freeTimeEntered", true);
        SetPrivateField(context.director, "_staminaRevealed", true);
        InvokeInstance(context.storyManager, "ResetState", freeTimePhase, false);
        InvokeInstance(context.timeManager, "SetTime", 1, springSeason, 1, 21, 0);
        InvokeInstance(context.energySystem, "SetVisible", true);
        InvokeInstance(context.energySystem, "SetEnergyState", 10, 200);
        InvokePrivateMethod(context.director, "HandleEnergyChanged", 10, 200);

        Assert.That((bool)GetPropertyValue(context.energySystem, "IsLowEnergyWarningActive"), Is.True, "自由时段低精力起手应仍保持 warning");
        Assert.That((float)InvokeInstance(context.playerMovement, "GetRuntimeSpeedMultiplier"), Is.EqualTo(0.8f).Within(0.001f), "低精力 warning 应继续带减速");
        Assert.That((string)InvokeInstance(context.director, "GetValidationFreeTimeNextAction"), Is.EqualTo("执行 Step，模拟推进到夜里 10 点并验证回住处压力。"));

        string nightStep = (string)InvokeInstance(context.director, "TryAdvanceFreeTimeValidationStep");
        Assert.That(nightStep, Is.EqualTo("验收入口：已模拟推进到夜里 10 点，回住处压力应已增强。"));
        Assert.That((string)InvokeInstance(context.director, "GetFreeTimePressureState"), Is.EqualTo("night"));
        Assert.That((string)InvokeInstance(context.director, "GetValidationFreeTimeNextAction"), Is.EqualTo("执行 Step，模拟推进到午夜并验证夜深提醒。"));

        string midnightStep = (string)InvokeInstance(context.director, "TryAdvanceFreeTimeValidationStep");
        Assert.That(midnightStep, Is.EqualTo("验收入口：已模拟推进到午夜，夜深提醒应已触发。"));
        Assert.That((string)InvokeInstance(context.director, "GetFreeTimePressureState"), Is.EqualTo("midnight"));

        string finalCallStep = (string)InvokeInstance(context.director, "TryAdvanceFreeTimeValidationStep");
        Assert.That(finalCallStep, Is.EqualTo("验收入口：已模拟推进到凌晨一点，最终催促应已触发。"));
        Assert.That((string)InvokeInstance(context.director, "GetFreeTimePressureState"), Is.EqualTo("final-call"));
        Assert.That((string)InvokeInstance(context.director, "GetValidationFreeTimeNextAction"), Is.EqualTo("回住处休息，或继续执行 Step 验证两点规则收束。"));

        string dayEndStep = (string)InvokeInstance(context.director, "TryAdvanceFreeTimeValidationStep");
        InvokePrivateMethod(context.director, "HandleSleep");

        Assert.That(dayEndStep, Is.EqualTo("验收入口：已模拟两点规则触发，Day1 应进入结束态。"));
        Assert.That(GetPropertyValue(context.storyManager, "CurrentPhase"), Is.EqualTo(dayEndPhase), "两点规则收束后应进入 DayEnd");
        Assert.That(GetPropertyValue(context.energySystem, "CurrentEnergy"), Is.EqualTo(GetPropertyValue(context.energySystem, "MaxEnergy")), "DayEnd 收束后应满精力");
        Assert.That((bool)GetPropertyValue(context.energySystem, "IsLowEnergyWarningActive"), Is.False, "DayEnd 收束后不应残留低精力 warning");
        Assert.That((float)InvokeInstance(context.playerMovement, "GetRuntimeSpeedMultiplier"), Is.EqualTo(1f).Within(0.001f), "DayEnd 收束后应撤销低精力减速");
        Assert.That((bool)InvokeInstance(context.director, "IsSleepInteractionAvailable"), Is.False, "DayEnd 后不应继续开放睡觉交互");
        Assert.That((string)InvokeInstance(context.director, "GetFreeTimePressureState"), Is.EqualTo("inactive"), "DayEnd 后夜间压力态应归零");
    }

    [UnityTest]
    public IEnumerator BedBridge_EndsDayAndRestoresSystems()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object freeTimePhase = ParseEnum(context.storyPhaseType, "FreeTime");
        object dayEndPhase = ParseEnum(context.storyPhaseType, "DayEnd");
        object springSeason = ParseEnum(context.seasonType, "Spring");

        SetPrivateField(context.director, "_freeTimeEntered", true);
        SetPrivateField(context.director, "_staminaRevealed", true);
        InvokeInstance(context.storyManager, "ResetState", freeTimePhase, false);
        InvokeInstance(context.timeManager, "SetTime", 1, springSeason, 1, 25, 0);
        InvokeInstance(context.energySystem, "SetVisible", true);
        InvokeInstance(context.energySystem, "SetEnergyState", 5, 200);
        InvokePrivateMethod(context.director, "HandleEnergyChanged", 5, 200);

        bool triggered = (bool)InvokeInstance(context.director, "TryTriggerSleepFromBed");
        InvokePrivateMethod(context.director, "HandleSleep");

        Assert.That(triggered, Is.True, "自由时段床交互应触发 DayEnd 桥接");
        Assert.That(GetPropertyValue(context.storyManager, "CurrentPhase"), Is.EqualTo(dayEndPhase));
        Assert.That(GetPropertyValue(context.energySystem, "CurrentEnergy"), Is.EqualTo(GetPropertyValue(context.energySystem, "MaxEnergy")));
        Assert.That((float)InvokeInstance(context.playerMovement, "GetRuntimeSpeedMultiplier"), Is.EqualTo(1f).Within(0.001f));
    }

    [UnityTest]
    public IEnumerator PromptOverlay_RecoversWhenDisplayedStateCacheIsMissing()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null, "应能获取 PromptOverlay 运行时实例");

        SetPrivateField(promptOverlay, "_manualPromptText", "测试提示");
        SetPrivateField(promptOverlay, "_queuedPromptText", "测试提示");
        SetPrivateField(promptOverlay, "_hasDisplayedState", true);
        SetPrivateField(promptOverlay, "_displayedState", null);

        InvokePrivateMethod(promptOverlay, "LateUpdate");

        Assert.That(GetPrivateFieldValue(promptOverlay, "_displayedState"), Is.Not.Null, "PromptOverlay 应在缓存缺失时自愈重建显示态");
        Assert.That((bool)GetPrivateFieldValue(promptOverlay, "_hasDisplayedState"), Is.True, "PromptOverlay 自愈后应继续保持已显示状态");
    }

    [UnityTest]
    public IEnumerator WorkbenchOverlay_RecoversCompatibilityNodesFromPrefabShell()
    {
        Type overlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1WorkbenchCraftingOverlay");
        InvokeStatic(overlayType, "EnsureRuntime");
        Component overlay = GetStaticPropertyValue(overlayType, "Instance") as Component;
        Assert.That(overlay, Is.Not.Null, "应能创建 Day1 WorkbenchOverlay 运行时对象");
        Track(overlay.gameObject);
        yield return null;

        RectTransform recipeViewport = GetPrivateFieldValue(overlay, "recipeViewportRect") as RectTransform;
        RectTransform recipeContent = GetPrivateFieldValue(overlay, "recipeContentRect") as RectTransform;
        RectTransform materialsViewport = GetPrivateFieldValue(overlay, "materialsViewportRect") as RectTransform;
        RectTransform materialsContent = GetPrivateFieldValue(overlay, "materialsContentRect") as RectTransform;
        RectTransform floatingProgressRoot = GetPrivateFieldValue(overlay, "floatingProgressRoot") as RectTransform;
        Component floatingProgressIcon = GetPrivateFieldValue(overlay, "floatingProgressIcon") as Component;
        Component floatingProgressFillImage = GetPrivateFieldValue(overlay, "floatingProgressFillImage") as Component;
        Component floatingProgressLabel = GetPrivateFieldValue(overlay, "floatingProgressLabel") as Component;

        Assert.That(recipeViewport, Is.Not.Null, "左侧 Recipe Viewport 应完成绑定");
        Assert.That(recipeContent, Is.Not.Null, "左侧 Recipe Content 应完成绑定");
        Assert.That(recipeViewport.GetComponent<Mask>(), Is.Not.Null, "左侧 Recipe Viewport 应有 Mask");
        Assert.That(recipeViewport.GetComponent<ScrollRect>(), Is.Not.Null, "左侧 Recipe Viewport 应有 ScrollRect");

        Assert.That(materialsViewport, Is.Not.Null, "右侧 Materials Viewport 应完成绑定或补建");
        Assert.That(materialsContent, Is.Not.Null, "右侧 Materials Content 应完成绑定或补建");
        Assert.That(materialsViewport.GetComponent<Mask>(), Is.Not.Null, "右侧 Materials Viewport 应有 Mask");
        Assert.That(materialsViewport.GetComponent<ScrollRect>(), Is.Not.Null, "右侧 Materials Viewport 应有 ScrollRect");
        Assert.That(materialsContent.parent, Is.EqualTo(materialsViewport), "右侧 Materials Content 应挂在 Viewport 下面");

        Assert.That(floatingProgressRoot, Is.Not.Null, "离台小进度容器应完成绑定或补建");
        Assert.That(floatingProgressIcon, Is.Not.Null, "离台小进度应有图标");
        Assert.That(floatingProgressFillImage, Is.Not.Null, "离台小进度应有进度填充");
        Assert.That(floatingProgressLabel, Is.Not.Null, "离台小进度应有数量标签");
    }

    private RuntimeContext CreateRuntimeContext()
    {
        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Type directorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Type energySystemType = ResolveTypeOrFail("EnergySystem");
        Type playerMovementType = ResolveTypeOrFail("PlayerMovement");
        Type seasonType = ResolveTypeOrFail("SeasonManager+Season");

        InvokeStatic(promptOverlayType, "EnsureRuntime");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null, "应能创建 Day1 PromptOverlay 运行时对象");
        Track(promptOverlay.gameObject);
        SetPrivateField(promptOverlay, "fadeDuration", 0f);
        SetPrivateField(promptOverlay, "completionStepDuration", 0f);
        SetPrivateField(promptOverlay, "pageFlipDuration", 0f);
        SetPrivateField(promptOverlay, "postDialogueResumeDelay", 0f);

        Component director = Track(new GameObject("SpringDay1Director_Test")).AddComponent(directorType);
        Component storyManager = Track(new GameObject("StoryManager_Test")).AddComponent(storyManagerType);
        Component timeManager = Track(new GameObject("TimeManager_Test")).AddComponent(timeManagerType);
        Component energySystem = Track(new GameObject("EnergySystem_Test")).AddComponent(energySystemType);

        GameObject playerGo = Track(new GameObject("PlayerMovement_Test"));
        playerGo.AddComponent<Rigidbody2D>();
        playerGo.AddComponent<BoxCollider2D>();
        Component playerMovement = playerGo.AddComponent(playerMovementType);

        SetStaticField(directorType, "_instance", director);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetStaticField(energySystemType, "<Instance>k__BackingField", energySystem);
        SetStaticField(promptOverlayType, "_instance", promptOverlay);

        SetPrivateField(timeManager, "showDebugInfo", false);
        SetPrivateField(timeManager, "enableHourEvent", false);
        SetPrivateField(timeManager, "enableMinuteEvent", false);
        SetPrivateField(timeManager, "enableDayEvent", false);
        SetPrivateField(timeManager, "enableYearEvent", false);
        SetPrivateField(timeManager, "enableSeasonChangeEvent", false);

        SetPrivateField(energySystem, "maxEnergy", 200);
        SetPrivateField(energySystem, "currentEnergy", 200);
        SetPrivateField(energySystem, "showDebugInfo", false);

        SetPrivateField(director, "_playerMovement", playerMovement);

        return new RuntimeContext(director, storyManager, timeManager, energySystem, playerMovement, storyPhaseType, seasonType);
    }

    private static Type ResolveTypeOrFail(string fullName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(fullName);
            if (type != null)
            {
                return type;
            }
        }

        Assert.Fail($"未找到类型: {fullName}");
        return null;
    }

    private GameObject Track(GameObject gameObject)
    {
        createdObjects.Add(gameObject);
        return gameObject;
    }

    private static object ParseEnum(Type enumType, string name)
    {
        return Enum.Parse(enumType, name);
    }

    private static object GetPropertyValue(object target, string propertyName)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(property, Is.Not.Null, $"未找到属性: {target.GetType().Name}.{propertyName}");
        return property.GetValue(target);
    }

    private static object GetStaticPropertyValue(Type type, string propertyName)
    {
        PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(property, Is.Not.Null, $"未找到静态属性: {type.Name}.{propertyName}");
        return property.GetValue(null);
    }

    private static object InvokeInstance(object target, string methodName, params object[] arguments)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return method.Invoke(target, arguments);
    }

    private static object InvokeStatic(Type type, string methodName, params object[] arguments)
    {
        MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到静态方法: {type.Name}.{methodName}");
        return method.Invoke(null, arguments);
    }

    private static object InvokePrivateMethod(object target, string methodName, params object[] arguments)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return method.Invoke(target, arguments);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private static object GetPrivateFieldValue(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        return field.GetValue(target);
    }

    private static void SetStaticField(Type type, string fieldName, object value)
    {
        FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到静态字段: {type.Name}.{fieldName}");
        field.SetValue(null, value);
    }

    private static void TrySetStaticField(string fullTypeName, string fieldName, object value)
    {
        Type type = null;
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType(fullTypeName);
            if (type != null)
            {
                break;
            }
        }

        if (type == null)
        {
            return;
        }

        FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(null, value);
        }
    }

    private readonly struct RuntimeContext
    {
        public RuntimeContext(
            object director,
            object storyManager,
            object timeManager,
            object energySystem,
            object playerMovement,
            Type storyPhaseType,
            Type seasonType)
        {
            this.director = director;
            this.storyManager = storyManager;
            this.timeManager = timeManager;
            this.energySystem = energySystem;
            this.playerMovement = playerMovement;
            this.storyPhaseType = storyPhaseType;
            this.seasonType = seasonType;
        }

        public object director { get; }
        public object storyManager { get; }
        public object timeManager { get; }
        public object energySystem { get; }
        public object playerMovement { get; }
        public Type storyPhaseType { get; }
        public Type seasonType { get; }
    }
}
