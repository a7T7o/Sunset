using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.TestTools;

[TestFixture]
public class SpringDay1LateDayRuntimeTests
{
    private readonly List<GameObject> createdObjects = new();
    private string temporaryScenePath;

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
        CleanupTemporaryScene();
        TrySetStaticField("Sunset.Story.SpringDay1Director", "_instance", null);
        TrySetStaticField("Sunset.Story.StoryManager", "_instance", null);
        TrySetStaticField("Sunset.Story.SpringDay1PromptOverlay", "_instance", null);
        TrySetStaticField("Sunset.Story.SpringDay1WorkbenchCraftingOverlay", "_instance", null);
        TrySetStaticField("Sunset.Story.SceneTransitionRunner", "_instance", null);
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

        SetPrivateField(context.director, "_freeTimeEntered", true);
        SetPrivateField(context.director, "_freeTimeIntroCompleted", true);
        SetPrivateField(context.director, "_staminaRevealed", true);
        InvokeInstance(context.storyManager, "ResetState", freeTimePhase, false);
        SetTimeWithoutSystems(context, 21, 0);
        InvokeInstance(context.energySystem, "SetVisible", true);
        InvokeInstance(context.energySystem, "SetEnergyState", 10, 200);
        InvokePrivateMethod(context.director, "HandleEnergyChanged", 10, 200);

        Assert.That((bool)GetPropertyValue(context.energySystem, "IsLowEnergyWarningActive"), Is.True, "自由时段低精力起手应仍保持 warning");
        Assert.That((float)InvokeInstance(context.playerMovement, "GetRuntimeSpeedMultiplier"), Is.EqualTo(0.8f).Within(0.001f), "低精力 warning 应继续带减速");
        Assert.That((string)InvokeInstance(context.director, "GetValidationFreeTimeNextAction"), Is.EqualTo("执行 Step，模拟推进到夜里 10 点并验证回住处压力。"));

        SetTimeWithoutSystems(context, 22, 0);
        InvokePrivateMethod(context.director, "HandleHourChanged", 22);
        Assert.That((string)InvokeInstance(context.director, "GetFreeTimePressureState"), Is.EqualTo("night"));
        Assert.That((string)InvokeInstance(context.director, "GetValidationFreeTimeNextAction"), Is.EqualTo("执行 Step，模拟推进到午夜并验证夜深提醒。"));

        SetTimeWithoutSystems(context, 24, 0);
        InvokePrivateMethod(context.director, "HandleHourChanged", 24);
        Assert.That((string)InvokeInstance(context.director, "GetFreeTimePressureState"), Is.EqualTo("midnight"));

        SetTimeWithoutSystems(context, 25, 0);
        InvokePrivateMethod(context.director, "HandleHourChanged", 25);
        Assert.That((string)InvokeInstance(context.director, "GetFreeTimePressureState"), Is.EqualTo("final-call"));
        Assert.That((string)InvokeInstance(context.director, "GetValidationFreeTimeNextAction"), Is.EqualTo("回住处休息，或继续执行 Step 验证两点规则收束。"));

        SetTimeWithoutSystems(context, 26, 0);
        bool triggered = (bool)InvokeInstance(context.director, "TryTriggerSleepFromBed");
        InvokePrivateMethod(context.director, "HandleSleep");

        Assert.That(triggered, Is.True, "自由时段 final-call 之后应允许通过回住处交互收束到 DayEnd。");
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

        SetPrivateField(context.director, "_freeTimeEntered", true);
        SetPrivateField(context.director, "_freeTimeIntroCompleted", true);
        SetPrivateField(context.director, "_staminaRevealed", true);
        InvokeInstance(context.storyManager, "ResetState", freeTimePhase, false);
        SetTimeWithoutSystems(context, 25, 0);
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
    public IEnumerator PlusHourAdvance_BeyondTwoAmAfterDay1_ShouldFallbackToHomeSleepTransition()
    {
        EnsureTestSceneActive("Town");

        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        Type debuggerType = ResolveTypeOrFail("TimeManagerDebugger");
        Component timeManagerComponent = context.timeManager as Component;
        Assert.That(timeManagerComponent, Is.Not.Null, "应能拿到测试用 TimeManager 组件。");
        Component debugger = timeManagerComponent.gameObject.AddComponent(debuggerType);

        SetTimeWithoutSystems(context, 26, 0, day: 2);
        InvokePrivateMethod(debugger, "AdvanceOneHour");
        InvokePrivateMethod(context.director, "HandleSleep");
        yield return null;

        Assert.That(InvokeInstance(context.timeManager, "GetDay"), Is.EqualTo(3), "后续天数用 + 号跨过两点时，应真的过到下一天。");
        Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("Home"), "后续天数超过两点未睡时，也应直接回到 Home 走睡觉收束。");
    }

    [UnityTest]
    public IEnumerator FreeTimePlayerFacingCopy_ShouldTightenAcrossNightPressure()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object freeTimePhase = ParseEnum(context.storyPhaseType, "FreeTime");

        SetPrivateField(context.director, "_freeTimeEntered", true);
        SetPrivateField(context.director, "_staminaRevealed", true);
        InvokeInstance(context.storyManager, "ResetState", freeTimePhase, false);
        SetTimeWithoutSystems(context, 21, 0);

        Assert.That((bool)InvokeInstance(context.director, "IsSleepInteractionAvailable"), Is.False);
        Assert.That((string)InvokeInstance(context.director, "GetCurrentFocusTextForTests"), Is.EqualTo("自由时段见闻会先接管，先别急着立刻睡。"));
        Assert.That((string)InvokeInstance(context.director, "GetValidationFreeTimeNextAction"), Is.EqualTo("自由时段见闻尚未收束，先让这段夜里的动静完整接管。"));

        object introPendingCard = InvokeInstance(context.director, "BuildPromptCardModel");
        Array introPendingItems = GetFieldValue(introPendingCard, "Items") as Array;
        Assert.That(introPendingItems, Is.Not.Null);
        Assert.That(GetPropertyValue(introPendingItems.GetValue(0), "Label"), Is.EqualTo("听完村里夜间见闻"));
        Assert.That(GetPropertyValue(introPendingItems.GetValue(0), "Detail"), Is.EqualTo("自由时段见闻已排队，先让这段夜里的动静完整接管。"));

        SetPrivateField(context.director, "_freeTimeIntroCompleted", true);

        Assert.That((bool)InvokeInstance(context.director, "IsSleepInteractionAvailable"), Is.True);
        Assert.That((string)InvokeInstance(context.director, "GetCurrentFocusTextForTests"), Is.EqualTo("今晚可以先看看夜里的村子，但最晚两点前得回住处。"));
        Assert.That((string)InvokeInstance(context.director, "GetRestInteractionHint", "睡觉"), Is.EqualTo("回屋休息"));
        Assert.That((string)InvokeInstance(context.director, "GetRestInteractionDetail", "按 E 睡觉"), Is.EqualTo("按 E 回屋休息，也可以先在村里再看看。"));

        object relaxedCard = InvokeInstance(context.director, "BuildPromptCardModel");
        Array relaxedItems = GetFieldValue(relaxedCard, "Items") as Array;
        Assert.That(relaxedItems, Is.Not.Null);
        Assert.That(GetPropertyValue(relaxedItems.GetValue(0), "Detail"), Is.EqualTo("今晚可以再看看夜里的村子，但最晚两点前必须回住处。"));
        Assert.That((string)InvokeInstance(context.director, "BuildPlayerFacingStatusSummary"), Does.Contain("今晚可以先看看夜里的村子"));

        SetTimeWithoutSystems(context, 25, 0);
        InvokePrivateMethod(context.director, "HandleHourChanged", 25);

        Assert.That((string)InvokeInstance(context.director, "GetCurrentFocusTextForTests"), Is.EqualTo("现在不要再逗留，立刻回住处睡觉。"));
        Assert.That((string)InvokeInstance(context.director, "GetRestInteractionHint", "睡觉"), Is.EqualTo("赶紧睡觉"));
        Assert.That((string)InvokeInstance(context.director, "GetRestInteractionDetail", "按 E 睡觉"), Is.EqualTo("按 E 立刻睡觉收束今天"));

        object finalCard = InvokeInstance(context.director, "BuildPromptCardModel");
        Array finalItems = GetFieldValue(finalCard, "Items") as Array;
        Assert.That(finalItems, Is.Not.Null);
        Assert.That(GetPropertyValue(finalItems.GetValue(0), "Detail"), Is.EqualTo("快到凌晨两点了，再拖会直接昏睡过去。"));
        Assert.That((string)InvokeInstance(context.director, "BuildPlayerFacingStatusSummary"), Does.Contain("快到凌晨两点了"));
    }

    [UnityTest]
    public IEnumerator DayEndPlayerFacingCopy_ShouldCarryTomorrowBurdenAndClearWorkbenchState()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object freeTimePhase = ParseEnum(context.storyPhaseType, "FreeTime");

        SetPrivateField(context.director, "_freeTimeEntered", true);
        SetPrivateField(context.director, "_freeTimeIntroCompleted", true);
        SetPrivateField(context.director, "_staminaRevealed", true);
        SetPrivateField(context.director, "_workbenchCraftingActive", true);
        SetPrivateField(context.director, "_workbenchCraftProgress", 0.42f);
        SetPrivateField(context.director, "_workbenchCraftQueueTotal", 2);
        SetPrivateField(context.director, "_workbenchCraftQueueCompleted", 1);
        SetPrivateField(context.director, "_workbenchCraftRecipeName", "WoodBox");
        InvokeInstance(context.storyManager, "ResetState", freeTimePhase, false);

        InvokePrivateMethod(context.director, "HandleSleep");

        Assert.That((string)InvokeInstance(context.director, "GetCurrentProgressLabel"), Is.EqualTo("第一夜已经熬过去，明天还得继续用做活证明自己"));
        Assert.That((string)InvokeInstance(context.director, "GetCurrentFocusTextForTests"), Is.EqualTo("这一夜总算先熬过去了。天亮以后，再让他们看清你值不值得留下。"));
        object dayEndCard = InvokeInstance(context.director, "BuildPromptCardModel");
        Assert.That(dayEndCard, Is.Not.Null);
        Assert.That(GetFieldValue(dayEndCard, "FooterText"), Is.EqualTo("第一夜已经熬过去，明天还得继续用做活证明自己"));
        Array dayEndItems = GetFieldValue(dayEndCard, "Items") as Array;
        Assert.That(dayEndItems, Is.Not.Null);
        Assert.That(GetPropertyValue(dayEndItems.GetValue(0), "Detail"), Is.EqualTo("今晚总算先熬过去了，但明天开始还得继续用下地和做活证明自己。"));
        Assert.That((string)InvokeInstance(context.director, "BuildPlayerFacingStatusSummary"), Does.Contain("值不值得留下"));
        Assert.That(GetPrivateFieldValue(context.director, "_workbenchCraftingActive"), Is.EqualTo(false));
        Assert.That(GetPrivateFieldValue(context.director, "_workbenchCraftRecipeName"), Is.EqualTo(string.Empty));
    }

    [UnityTest]
    public IEnumerator ReminderCompletion_ShouldEnterFreeTimeWithIntroPendingAndYieldWorkbenchToFormalNightIntro()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object reminderPhase = ParseEnum(context.storyPhaseType, "ReturnAndReminder");
        object freeTimePhase = ParseEnum(context.storyPhaseType, "FreeTime");
        InvokeInstance(context.storyManager, "ResetState", reminderPhase, false);

        InvokePrivateMethod(context.director, "EnterFreeTime");

        Assert.That(GetPropertyValue(context.storyManager, "CurrentPhase"), Is.EqualTo(freeTimePhase));
        NormalizeManagedTimeTargetForCurrentPhase(context, 6, 0, out int freeTimeFloorHour, out int freeTimeFloorMinute);
        Assert.That(freeTimeFloorHour, Is.EqualTo(19), "自由时段进入后，受管控时钟的最早落点应是 19:30。");
        Assert.That(freeTimeFloorMinute, Is.EqualTo(30), "自由时段进入后，受管控时钟的最早落点应是 19:30。");
        Assert.That(GetPrivateFieldValue(context.director, "_freeTimeIntroQueued"), Is.EqualTo(true));
        Assert.That(GetPrivateFieldValue(context.director, "_freeTimeIntroCompleted"), Is.EqualTo(false));
        Assert.That((bool)InvokeInstance(context.director, "IsSleepInteractionAvailable"), Is.False);
        Assert.That((string)InvokeInstance(context.director, "GetCurrentFocusTextForTests"), Is.EqualTo("自由时段见闻会先接管，先别急着立刻睡。"));
        Assert.That((string)InvokeInstance(context.director, "GetValidationFreeTimeNextAction"), Is.EqualTo("自由时段见闻尚未收束，先让这段夜里的动静完整接管。"));

        bool canCraft = InvokeWorkbenchCraftPermission(context.director, out string blockerMessage);
        Assert.That(canCraft, Is.True, "完成 0.0.4 进入 0.0.5 之后，即使夜间 formal 见闻仍 pending，工作台也不应再被重新锁回去。");
        Assert.That(blockerMessage, Is.EqualTo(string.Empty));
        Assert.That((bool)InvokeInstance(context.director, "ShouldExposeWorkbenchInteraction"), Is.True, "完成 0.0.4 进入 0.0.5 之后，工作台提示不应再被 formal 夜间见闻整体禁用。");
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
    public IEnumerator PromptOverlay_ClearsStaleManualPromptAfterPhaseChanges()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object crashPhase = ParseEnum(context.storyPhaseType, "CrashAndMeet");
        InvokeInstance(context.storyManager, "ResetState", crashPhase, false);

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null, "应能获取 PromptOverlay 运行时实例");

        SetPrivateField(promptOverlay, "_manualPromptText", "先用锄头开垦一格土地。");
        SetPrivateField(promptOverlay, "_manualPromptPhaseKey", "FarmingTutorial");
        SetPrivateField(promptOverlay, "_queuedPromptText", "先用锄头开垦一格土地。");

        object state = InvokePrivateMethod(promptOverlay, "BuildCurrentViewState");
        Assert.That(state, Is.Not.Null, "PromptOverlay 应能构建当前阶段状态。");
        Assert.That(GetFieldValue(state, "FocusText"), Is.EqualTo("靠近 NPC 并按 E 开始首段对话。"), "阶段已经回到首段时，不应继续残留农田阶段的手动提示。");
        Assert.That(GetPrivateFieldValue(promptOverlay, "_manualPromptText"), Is.EqualTo(string.Empty), "过期 manual prompt 应被自动清掉。");
        Assert.That(GetPrivateFieldValue(promptOverlay, "_queuedPromptText"), Is.EqualTo(string.Empty), "过期 manual prompt 的排队内容也应一起清掉。");
    }

    [UnityTest]
    public IEnumerator PromptOverlay_FormalTaskCard_ShouldIgnoreManualFocusOverride_WhenDirectorModelExists()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object crashPhase = ParseEnum(context.storyPhaseType, "CrashAndMeet");
        InvokeInstance(context.storyManager, "ResetState", crashPhase, false);

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null, "应能获取 PromptOverlay 运行时实例");

        SetPrivateField(promptOverlay, "_manualPromptText", "先用锄头开垦一格土地。");
        SetPrivateField(promptOverlay, "_manualPromptPhaseKey", "CrashAndMeet");
        SetPrivateField(promptOverlay, "_queuedPromptText", "先用锄头开垦一格土地。");

        object state = InvokePrivateMethod(promptOverlay, "BuildCurrentViewState");
        Assert.That(state, Is.Not.Null, "PromptOverlay 在 director 有 formal model 时仍应构建正式任务页。");
        Assert.That(
            GetFieldValue(state, "FocusText"),
            Is.EqualTo(InvokeInstance(context.director, "GetCurrentFocusTextForTests")),
            "manual bridge prompt 不应再覆盖 formal task card 的 FocusText 真值。");
    }

    [UnityTest]
    public IEnumerator PromptOverlay_DirectorBridgePrompt_ShouldRenderSeparatelyFromFormalFocus()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object crashPhase = ParseEnum(context.storyPhaseType, "CrashAndMeet");
        InvokeInstance(context.storyManager, "ResetState", crashPhase, false);
        InvokeInstance(context.director, "ShowTaskListBridgePrompt", "测试过桥提示");

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null, "应能获取 PromptOverlay 运行时实例");

        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        TextMeshProUGUI focusLabel = GetPrivateFieldValue(promptOverlay, "focusText") as TextMeshProUGUI;
        TextMeshProUGUI bridgeLabel = GetPrivateFieldValue(promptOverlay, "bridgePromptText") as TextMeshProUGUI;
        CanvasGroup bridgeGroup = GetPrivateFieldValue(promptOverlay, "bridgePromptCanvasGroup") as CanvasGroup;

        Assert.That(focusLabel, Is.Not.Null);
        Assert.That(bridgeLabel, Is.Not.Null);
        Assert.That(bridgeGroup, Is.Not.Null);
        Assert.That(focusLabel.text, Is.EqualTo(InvokeInstance(context.director, "GetCurrentFocusTextForTests")));
        Assert.That(bridgeLabel.text, Is.EqualTo("测试过桥提示"), "director bridge prompt 应落到独立显示区，而不是继续写坏 formal focus。");
        Assert.That(bridgeGroup.alpha, Is.GreaterThan(0.99f), "独立 bridge prompt 区在有提示时应可见。");
    }

    [UnityTest]
    public IEnumerator PromptOverlay_DirectorBridgePrompt_ShouldHideWhenSemanticallyRedundantWithFormalCard()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object crashPhase = ParseEnum(context.storyPhaseType, "CrashAndMeet");
        InvokeInstance(context.storyManager, "ResetState", crashPhase, false);
        InvokeInstance(context.director, "ShowTaskListBridgePrompt", "先靠近村长按 E，弄清眼前到底发生了什么。");

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null, "应能获取 PromptOverlay 运行时实例");

        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        TextMeshProUGUI bridgeLabel = GetPrivateFieldValue(promptOverlay, "bridgePromptText") as TextMeshProUGUI;
        CanvasGroup bridgeGroup = GetPrivateFieldValue(promptOverlay, "bridgePromptCanvasGroup") as CanvasGroup;

        Assert.That(bridgeLabel, Is.Not.Null);
        Assert.That(bridgeGroup, Is.Not.Null);
        Assert.That(bridgeGroup.alpha, Is.LessThan(0.01f), "语义上已被 formal task card 表达的 bridge prompt 不应继续重复显示。");
        Assert.That(bridgeLabel.text, Is.EqualTo(string.Empty));
    }

    [UnityTest]
    public IEnumerator PromptOverlay_UsesParentCanvasGovernance_WhenUiRootCanvasExists()
    {
        GameObject uiRoot = Track(new GameObject(
            "UI",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)));

        Canvas uiCanvas = uiRoot.GetComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        uiCanvas.overrideSorting = true;
        uiCanvas.sortingOrder = 37;
        uiCanvas.pixelPerfect = false;

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        InvokeStatic(promptOverlayType, "EnsureRuntime");
        yield return null;

        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null, "应能创建 PromptOverlay 运行时实例。");
        createdObjects.Add(promptOverlay.gameObject);

        Canvas overlayCanvas = promptOverlay.GetComponent<Canvas>();
        Assert.That(overlayCanvas, Is.Not.Null, "PromptOverlay 应保留自己的 Canvas 组件。");
        Assert.That(overlayCanvas.overrideSorting, Is.False, "挂在 UI 根 Canvas 下时，PromptOverlay 应回到父级基础层治理，而不是继续自抬排序。");
        Assert.That(overlayCanvas.sortingOrder, Is.EqualTo(uiCanvas.sortingOrder), "PromptOverlay 在父级 UI 根下应跟随父层排序口径。");
        Assert.That(overlayCanvas.renderMode, Is.EqualTo(uiCanvas.renderMode), "PromptOverlay 应继承父级 Canvas 的渲染模式。");
    }

    [UnityTest]
    public IEnumerator PromptOverlay_ShouldPreferBaseCanvasUnderUiRoot_InsteadOfModalPackageCanvas()
    {
        GameObject uiRoot = Track(new GameObject("UI", typeof(RectTransform)));

        GameObject baseCanvasRoot = Track(new GameObject(
            "MainCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)));
        baseCanvasRoot.transform.SetParent(uiRoot.transform, false);
        Canvas baseCanvas = baseCanvasRoot.GetComponent<Canvas>();
        baseCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        baseCanvas.overrideSorting = true;
        baseCanvas.sortingOrder = 25;

        GameObject packagePanelRoot = Track(new GameObject(
            "PackagePanel",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)));
        packagePanelRoot.transform.SetParent(uiRoot.transform, false);
        Canvas packageCanvas = packagePanelRoot.GetComponent<Canvas>();
        packageCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        packageCanvas.overrideSorting = true;
        packageCanvas.sortingOrder = 181;

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        InvokeStatic(promptOverlayType, "EnsureRuntime");
        yield return null;

        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null, "应能创建 PromptOverlay 运行时实例。");
        createdObjects.Add(promptOverlay.gameObject);

        Assert.That(promptOverlay.transform.parent, Is.EqualTo(baseCanvasRoot.transform), "UI 根下同时存在基础 Canvas 和背包模态 Canvas 时，PromptOverlay 应挂到基础层 Canvas，而不是挂到 PackagePanel 模态层。");

        promptOverlay.transform.SetParent(packagePanelRoot.transform, false);
        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        Assert.That(promptOverlay.transform.parent, Is.EqualTo(baseCanvasRoot.transform), "即使任务清单一度被挂到错误模态层，下一次运行时刷新也应自动回正到基础 Canvas。");

        Canvas overlayCanvas = promptOverlay.GetComponent<Canvas>();
        Assert.That(overlayCanvas, Is.Not.Null);
        Assert.That(overlayCanvas.overrideSorting, Is.False, "回正后任务清单应继续服从基础层父 Canvas，而不是保留独立 overrideSorting。");
        Assert.That(overlayCanvas.sortingOrder, Is.EqualTo(baseCanvas.sortingOrder), "回正后任务清单排序应跟随基础层 Canvas。");
        Assert.That(packageCanvas.sortingOrder, Is.GreaterThan(overlayCanvas.sortingOrder), "背包模态层排序应继续高于基础层任务清单。");
    }

    [UnityTest]
    public IEnumerator PromptOverlay_ShouldHideWhilePackagePanelIsOpen()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object crashPhase = ParseEnum(context.storyPhaseType, "CrashAndMeet");
        InvokeInstance(context.storyManager, "ResetState", crashPhase, false);

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null, "应能获取 PromptOverlay 运行时实例");

        CanvasGroup canvasGroup = GetPrivateFieldValue(promptOverlay, "canvasGroup") as CanvasGroup;
        Assert.That(canvasGroup, Is.Not.Null, "PromptOverlay 应持有 CanvasGroup。");

        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;
        Assert.That(canvasGroup.alpha, Is.GreaterThan(0.99f), "测试前提失败：任务清单应先处于可见态。");

        Component packageTabs = Track(new GameObject("PackageTabs_Test")).AddComponent(ResolveTypeOrFail("PackagePanelTabsUI"));
        GameObject panelRoot = Track(new GameObject("PackagePanel_Test", typeof(RectTransform)));
        panelRoot.SetActive(false);
        Transform topRoot = Track(new GameObject("Top", typeof(RectTransform))).transform;
        topRoot.SetParent(panelRoot.transform, false);
        Track(new GameObject("Top_0", typeof(RectTransform), typeof(UnityEngine.CanvasRenderer), typeof(UnityEngine.UI.Image), typeof(UnityEngine.UI.Toggle))).transform.SetParent(topRoot, false);
        Transform mainRoot = Track(new GameObject("Main", typeof(RectTransform))).transform;
        mainRoot.SetParent(panelRoot.transform, false);
        Transform pageRoot = Track(new GameObject("0_Props", typeof(RectTransform))).transform;
        pageRoot.SetParent(mainRoot, false);
        Track(new GameObject("Main", typeof(RectTransform))).transform.SetParent(pageRoot, false);

        InvokeInstance(packageTabs, "SetRoots", panelRoot, topRoot, mainRoot);
        InvokeInstance(packageTabs, "ShowPanel", true);

        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        Assert.That(canvasGroup.alpha, Is.LessThan(0.01f), "背包/包裹页面打开时，任务清单应按统一模态规则退场，避免和页面内容并排争抢视线。");

        Canvas promptCanvas = promptOverlay.GetComponent<Canvas>();
        Assert.That(promptCanvas, Is.Not.Null);
        Assert.That(promptCanvas.overrideSorting, Is.False, "任务清单应继续服从父级 UI 根排序，而不是自己抬成独立模态层。");
    }

    [UnityTest]
    public IEnumerator PromptOverlay_ShouldStayVisibleWhileWorkbenchOverlayIsVisible()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object crashPhase = ParseEnum(context.storyPhaseType, "CrashAndMeet");
        InvokeInstance(context.storyManager, "ResetState", crashPhase, false);

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null, "应能获取 PromptOverlay 运行时实例");

        CanvasGroup canvasGroup = GetPrivateFieldValue(promptOverlay, "canvasGroup") as CanvasGroup;
        Assert.That(canvasGroup, Is.Not.Null, "PromptOverlay 应持有 CanvasGroup。");

        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;
        Assert.That(canvasGroup.alpha, Is.GreaterThan(0.99f), "测试前提失败：任务清单应先处于可见态。");

        Type workbenchOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1WorkbenchCraftingOverlay");
        InvokeStatic(workbenchOverlayType, "EnsureRuntime");
        yield return null;

        Component workbenchOverlay = GetStaticPropertyValue(workbenchOverlayType, "Instance") as Component;
        Assert.That(workbenchOverlay, Is.Not.Null, "应能获取 WorkbenchOverlay 运行时实例");
        Track(workbenchOverlay.gameObject);
        SetPrivateField(workbenchOverlay, "_isVisible", true);

        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        Assert.That(canvasGroup.alpha, Is.GreaterThan(0.99f), "工作台页面打开时，任务清单不应发生透明度闪烁或被压暗。");
    }

    [UnityTest]
    public IEnumerator PromptOverlay_ManualPromptState_ShouldStillExposeReadableTaskRow()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null);

        object state = InvokePrivateMethod(promptOverlay, "BuildManualState", "测试提示");
        Assert.That(state, Is.Not.Null);

        IList items = GetFieldValue(state, "Items") as IList;
        Assert.That(items, Is.Not.Null);
        Assert.That(items.Count, Is.EqualTo(1), "手动提示态也应保留一条可读任务行，避免左侧列表空掉。");

        object row = items[0];
        Assert.That(GetFieldValue(row, "Label"), Is.EqualTo("当前提示"));
        Assert.That(GetFieldValue(row, "Detail"), Is.EqualTo("测试提示"));
    }

    [UnityTest]
    public IEnumerator PromptOverlay_Show_ShouldReactivateInactiveRuntimeInstanceBeforeStartingTransition()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null);

        promptOverlay.gameObject.SetActive(false);
        Assert.That(promptOverlay.gameObject.activeSelf, Is.False);

        Assert.DoesNotThrow(() => InvokeInstance(promptOverlay, "Show", "治疗结束后，先去工作台看看。"));
        yield return null;

        Assert.That(promptOverlay.gameObject.activeSelf, Is.True, "PromptOverlay 在被别的 UI 链临时关掉后，Show 应先自救拉起运行时对象。");
        Assert.That(GetPrivateFieldValue(promptOverlay, "_queuedPromptText"), Is.EqualTo("治疗结束后，先去工作台看看。"));
    }

    [UnityTest]
    public IEnumerator PromptOverlay_Hide_ShouldNotStartCoroutineWhenRuntimeInstanceIsInactive()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null);

        promptOverlay.gameObject.SetActive(false);
        Assert.That(promptOverlay.gameObject.activeSelf, Is.False);

        Assert.DoesNotThrow(() => InvokeInstance(promptOverlay, "Hide"));
        yield return null;
    }

    [UnityTest]
    public IEnumerator PromptOverlay_CrashAndMeet_ShouldRenderReadableCurrentTaskOnFirstFrame()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object crashPhase = ParseEnum(context.storyPhaseType, "CrashAndMeet");
        InvokeInstance(context.storyManager, "ResetState", crashPhase, false);

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null);

        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        object state = InvokePrivateMethod(promptOverlay, "BuildCurrentViewState");
        Assert.That(state, Is.Not.Null, "开局首段也应立即生成可读任务态，不能让左侧任务栏空掉。");
        Assert.That(GetFieldValue(state, "FocusText"), Is.Not.EqualTo(string.Empty), "开局首段焦点提示不应为空。");

        IList items = GetFieldValue(state, "Items") as IList;
        Assert.That(items, Is.Not.Null.And.Count.GreaterThan(0), "开局首段至少应有一条当前任务行。");

        object firstRow = items[0];
        Assert.That(GetFieldValue(firstRow, "Label"), Is.Not.EqualTo(string.Empty));
        Assert.That(GetFieldValue(firstRow, "Detail"), Is.Not.EqualTo(string.Empty));
    }

    [UnityTest]
    public IEnumerator PromptOverlay_RuntimeCanvas_ShouldBeScreenOverlayAndRenderFilledTaskTexts()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object farmingPhase = ParseEnum(context.storyPhaseType, "FarmingTutorial");
        InvokeInstance(context.storyManager, "ResetState", farmingPhase, false);

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null, "应能获取 PromptOverlay 运行时实例");

        Canvas overlayCanvas = GetPrivateFieldValue(promptOverlay, "overlayCanvas") as Canvas;
        Assert.That(overlayCanvas, Is.Not.Null, "PromptOverlay 应持有运行时 Canvas");
        Assert.That(overlayCanvas.renderMode, Is.EqualTo(RenderMode.ScreenSpaceOverlay), "PromptOverlay 运行时必须回到屏幕叠加层，不能继续沿用错误的 world-space 壳。");

        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        Component titleText = GetPrivateFieldValue(promptOverlay, "titleText") as Component;
        Component focusText = GetPrivateFieldValue(promptOverlay, "focusText") as Component;
        Assert.That(titleText, Is.Not.Null);
        Assert.That(focusText, Is.Not.Null);
        Assert.That(GetPropertyValue(titleText, "text"), Is.Not.EqualTo(string.Empty), "PromptOverlay 标题不应空白。");
        Assert.That(GetPropertyValue(focusText, "text"), Is.Not.EqualTo(string.Empty), "PromptOverlay 焦点提示不应空白。");

        object frontPage = GetPrivateFieldValue(promptOverlay, "_frontPage");
        Assert.That(frontPage, Is.Not.Null, "PromptOverlay 应保留当前前台页引用。");
        IList rows = GetFieldValue(frontPage, "rows") as IList;
        Assert.That(rows, Is.Not.Null.And.Count.GreaterThan(0), "前台页应至少绑定一条任务行。");

        object firstRow = rows[0];
        Assert.That(firstRow, Is.Not.Null);
        Component label = GetFieldValue(firstRow, "label") as Component;
        Component detail = GetFieldValue(firstRow, "detail") as Component;
        Assert.That(label, Is.Not.Null, "前台 row 应绑定标题文本组件。");
        Assert.That(detail, Is.Not.Null, "前台 row 应绑定说明文本组件。");
        Assert.That(GetPropertyValue(label, "text"), Is.Not.EqualTo(string.Empty), "PromptOverlay 前台任务行标题不应为空。");
        Assert.That(GetPropertyValue(detail, "text"), Is.Not.EqualTo(string.Empty), "PromptOverlay 前台任务行说明不应为空。");
    }

    [UnityTest]
    public IEnumerator PromptOverlay_ShouldRecoverFromDestroyedRowCanvasGroup()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object crashPhase = ParseEnum(context.storyPhaseType, "CrashAndMeet");
        InvokeInstance(context.storyManager, "ResetState", crashPhase, false);

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null, "应能获取 PromptOverlay 运行时实例");
        SetPrivateField(promptOverlay, "completionStepDuration", 1f);

        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        object frontPage = GetPrivateFieldValue(promptOverlay, "_frontPage");
        Assert.That(frontPage, Is.Not.Null);

        IList rows = GetFieldValue(frontPage, "rows") as IList;
        Assert.That(rows, Is.Not.Null.And.Count.GreaterThan(0), "前台页应至少有一条任务行。");

        object firstRow = rows[0];
        CanvasGroup staleGroup = GetFieldValue(firstRow, "group") as CanvasGroup;
        Assert.That(staleGroup, Is.Not.Null, "前台第一行初始应带有 CanvasGroup。");
        UnityEngine.Object.DestroyImmediate(staleGroup);

        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        frontPage = GetPrivateFieldValue(promptOverlay, "_frontPage");
        rows = GetFieldValue(frontPage, "rows") as IList;
        Assert.That(rows, Is.Not.Null.And.Count.GreaterThan(0));

        firstRow = rows[0];
        CanvasGroup rebuiltGroup = GetFieldValue(firstRow, "group") as CanvasGroup;
        Assert.That(rebuiltGroup, Is.Not.Null, "任务行 CanvasGroup 被销毁后，PromptOverlay 应在下一次刷新时自愈补回，而不是抛 MissingReference。");
        Assert.That(rebuiltGroup.alpha, Is.GreaterThan(0.98f), "自愈后的任务行应恢复到可见态。");
    }

    [UnityTest]
    public IEnumerator PromptOverlay_CompletionAnimation_ShouldStopTouchingDestroyedRowCanvasGroup()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object farmingPhase = ParseEnum(context.storyPhaseType, "FarmingTutorial");
        InvokeInstance(context.storyManager, "ResetState", farmingPhase, false);

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null, "应能获取 PromptOverlay 运行时实例");

        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        object frontPage = GetPrivateFieldValue(promptOverlay, "_frontPage");
        Assert.That(frontPage, Is.Not.Null);

        IList rows = GetFieldValue(frontPage, "rows") as IList;
        Assert.That(rows, Is.Not.Null.And.Count.GreaterThan(0), "前台页应至少有一条任务行。");

        object firstRow = rows[0];
        CanvasGroup currentGroup = GetFieldValue(firstRow, "group") as CanvasGroup;
        Assert.That(currentGroup, Is.Not.Null, "动画测试前的任务行应带有有效 CanvasGroup。");
        object oldState = CreatePromptRowState(promptOverlayType, "测试任务", "动画前详情", false);
        object newState = CreatePromptRowState(promptOverlayType, "测试任务", "动画后详情", true);

        IEnumerator completionAnimation = InvokePrivateMethod(promptOverlay, "AnimateRowCompletion", firstRow, oldState, newState) as IEnumerator;
        Assert.That(completionAnimation, Is.Not.Null, "应能拿到任务完成动画协程。");
        UnityEngine.Object.DestroyImmediate(currentGroup);

        Assert.DoesNotThrow(
            () => completionAnimation.MoveNext(),
            "任务完成动画进行中即使 row CanvasGroup 被销毁，也不应继续触碰 stale 引用导致 MissingReference。");

        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        frontPage = GetPrivateFieldValue(promptOverlay, "_frontPage");
        rows = GetFieldValue(frontPage, "rows") as IList;
        Assert.That(rows, Is.Not.Null.And.Count.GreaterThan(0));

        firstRow = rows[0];
        CanvasGroup rebuiltGroup = GetFieldValue(firstRow, "group") as CanvasGroup;
        Assert.That(rebuiltGroup, Is.Not.Null, "动画路径里的 stale row 也应在后续刷新时自愈补回 CanvasGroup。");
    }

    [UnityTest]
    public IEnumerator PromptOverlay_ShouldHideDuringDialogueAndRecoverAfterwards()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object crashPhase = ParseEnum(context.storyPhaseType, "CrashAndMeet");
        InvokeInstance(context.storyManager, "ResetState", crashPhase, false);

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null);

        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        CanvasGroup canvasGroup = GetPrivateFieldValue(promptOverlay, "canvasGroup") as CanvasGroup;
        Assert.That(canvasGroup, Is.Not.Null);
        Assert.That(canvasGroup.alpha, Is.GreaterThan(0.99f), "首屏任务卡在正常推进时应可见。");

        Type dialogueStartType = ResolveTypeOrFail("Sunset.Events.DialogueStartEvent");
        Type dialogueEndType = ResolveTypeOrFail("Sunset.Events.DialogueEndEvent");
        InvokePrivateMethod(promptOverlay, "OnDialogueStart", Activator.CreateInstance(dialogueStartType));
        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        Assert.That(canvasGroup.alpha, Is.LessThan(0.01f), "正式对话开始时，左侧任务卡应立即淡出隐藏。");

        InvokePrivateMethod(promptOverlay, "OnDialogueEnd", Activator.CreateInstance(dialogueEndType));
        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        Assert.That(canvasGroup.alpha, Is.GreaterThan(0.99f), "对话结束后，左侧任务卡应恢复显示。");
    }

    [UnityTest]
    public IEnumerator PromptOverlay_ShouldReplaceStaleWorldSpaceStaticInstance()
    {
        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");

        GameObject staleRoot = Track(new GameObject("PromptOverlay_Stale", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup)));
        Canvas staleCanvas = staleRoot.GetComponent<Canvas>();
        staleCanvas.renderMode = RenderMode.WorldSpace;
        Component staleOverlay = staleRoot.AddComponent(promptOverlayType);
        SetStaticField(promptOverlayType, "_instance", staleOverlay);

        InvokeStatic(promptOverlayType, "EnsureRuntime");
        yield return null;

        Component runtimeOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(runtimeOverlay, Is.Not.Null, "PromptOverlay 应能从 stale 静态实例恢复。");
        Assert.That(runtimeOverlay, Is.Not.SameAs(staleOverlay), "world-space 的旧静态实例不应继续截胡 runtime 真正使用的 PromptOverlay。");

        Canvas runtimeCanvas = GetPrivateFieldValue(runtimeOverlay, "overlayCanvas") as Canvas;
        Assert.That(runtimeCanvas, Is.Not.Null);
        Assert.That(runtimeCanvas.renderMode, Is.EqualTo(RenderMode.ScreenSpaceOverlay));
    }

    [UnityTest]
    public IEnumerator PromptOverlay_ShouldReplaceIncompleteScreenOverlayStaticInstance()
    {
        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");

        GameObject staleRoot = Track(new GameObject("PromptOverlay_Incomplete", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup)));
        Canvas staleCanvas = staleRoot.GetComponent<Canvas>();
        staleCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        Component staleOverlay = staleRoot.AddComponent(promptOverlayType);

        GameObject taskCardRoot = Track(new GameObject("TaskCardRoot", typeof(RectTransform)));
        taskCardRoot.transform.SetParent(staleRoot.transform, false);

        SetStaticField(promptOverlayType, "_instance", staleOverlay);

        InvokeStatic(promptOverlayType, "EnsureRuntime");
        yield return null;

        Component runtimeOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(runtimeOverlay, Is.Not.Null);
        Assert.That(runtimeOverlay, Is.Not.SameAs(staleOverlay), "缺关键文本链和页面节点的 screen-overlay 壳不应继续被复用。");
    }

    [UnityTest]
    public IEnumerator PromptOverlay_FarmingTutorial_ShouldOnlyRenderCurrentPrimaryTask()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object farmingPhase = ParseEnum(context.storyPhaseType, "FarmingTutorial");
        InvokeInstance(context.storyManager, "ResetState", farmingPhase, false);

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        Assert.That(promptOverlay, Is.Not.Null);

        InvokePrivateMethod(promptOverlay, "LateUpdate");
        yield return null;

        int activeTaskRows = 0;
        Component[] components = promptOverlay.GetComponentsInChildren<Component>(true);
        for (int index = 0; index < components.Length; index++)
        {
            Component component = components[index];
            if (component is not RectTransform rowRect || !rowRect.name.StartsWith("TaskRow_"))
            {
                continue;
            }

            if (rowRect.gameObject.activeSelf)
            {
                activeTaskRows++;
            }
        }

        Assert.That(activeTaskRows, Is.EqualTo(1), "前台任务卡应只显示当前主任务这一条，不再把整组教学任务一次全部摊开。");
    }

    [UnityTest]
    public IEnumerator Director_FarmingTutorialPromptCard_ShouldExposeFiveFilledObjectives()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object farmingPhase = ParseEnum(context.storyPhaseType, "FarmingTutorial");
        InvokeInstance(context.storyManager, "ResetState", farmingPhase, false);

        object cardModel = InvokeInstance(context.director, "BuildPromptCardModel");
        Assert.That(cardModel, Is.Not.Null, "导演层应能构建正式任务卡模型。");

        Array items = GetFieldValue(cardModel, "Items") as Array;
        Assert.That(items, Is.Not.Null, "任务卡 Items 不应为空。");
        Assert.That(items.Length, Is.EqualTo(5), "农田教学阶段应把 5 个正式目标都显式写进任务卡。");

        for (int index = 0; index < items.Length; index++)
        {
            object item = items.GetValue(index);
            Assert.That(item, Is.Not.Null);
            Assert.That(GetPropertyValue(item, "Label"), Is.Not.EqualTo(string.Empty), $"任务项 {index} 的标题不应为空。");
            Assert.That(GetPropertyValue(item, "Detail"), Is.Not.EqualTo(string.Empty), $"任务项 {index} 的说明不应为空。");
        }
    }

    [UnityTest]
    public IEnumerator Director_FarmingTutorial_ShouldAdvanceToCraftWhenWoodAlreadyExistsAfterTrackingStarted()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object farmingPhase = ParseEnum(context.storyPhaseType, "FarmingTutorial");
        InvokeInstance(context.storyManager, "ResetState", farmingPhase, false);

        InvokeInstance(context.director, "BuildPromptCardModel");
        SetPrivateField(context.director, "_tillObjectiveCompleted", true);
        SetPrivateField(context.director, "_plantObjectiveCompleted", true);
        SetPrivateField(context.director, "_waterObjectiveCompleted", true);

        Type inventoryType = ResolveTypeOrFail("InventoryService");
        Type itemStackType = ResolveTypeOrFail("ItemStack");
        Component inventory = Track(new GameObject("InventoryService_Test")).AddComponent(inventoryType);
        object woodStack = Activator.CreateInstance(itemStackType, 3200, 0, 3);
        InvokeInstance(inventory, "SetSlot", 0, woodStack);
        InvokePrivateMethod(context.director, "RefreshInventoryTrackingSubscription", inventory);

        object cardModel = InvokeInstance(context.director, "BuildPromptCardModel");
        Assert.That(cardModel, Is.Not.Null);
        Assert.That(GetFieldValue(cardModel, "FocusText"), Is.EqualTo("回到工作台，完成一次真正的基础制作。"), "背包里已经有足够木头时，木材目标应自动判完并直接推进到制作。");

        Array items = GetFieldValue(cardModel, "Items") as Array;
        Assert.That(items, Is.Not.Null);
        object woodItem = items.GetValue(3);
        Assert.That(GetPropertyValue(woodItem, "Completed"), Is.EqualTo(true), "木材目标应直接判完成。");
    }

    [UnityTest]
    public IEnumerator Director_FarmingTutorialPromptCard_ShouldAutoCompleteWoodObjectiveWithoutWaitingInventorySubscription()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object farmingPhase = ParseEnum(context.storyPhaseType, "FarmingTutorial");
        InvokeInstance(context.storyManager, "ResetState", farmingPhase, false);
        SetPrivateField(context.director, "_tillObjectiveCompleted", true);
        SetPrivateField(context.director, "_plantObjectiveCompleted", true);
        SetPrivateField(context.director, "_waterObjectiveCompleted", true);

        Type inventoryType = ResolveTypeOrFail("InventoryService");
        Type itemStackType = ResolveTypeOrFail("ItemStack");
        Component inventory = Track(new GameObject("InventoryService_AutoWood")).AddComponent(inventoryType);
        object woodStack = Activator.CreateInstance(itemStackType, 3200, 0, 3);
        InvokeInstance(inventory, "SetSlot", 0, woodStack);

        object cardModel = InvokeInstance(context.director, "BuildPromptCardModel");
        Assert.That(cardModel, Is.Not.Null);
        Assert.That(GetFieldValue(cardModel, "FocusText"), Is.EqualTo("回到工作台，完成一次真正的基础制作。"), "任务卡构建时应主动自校正库存进度，不应等订阅刷新后才推进。");

        Array items = GetFieldValue(cardModel, "Items") as Array;
        Assert.That(items, Is.Not.Null);
        object woodItem = items.GetValue(3);
        Assert.That(GetPropertyValue(woodItem, "Completed"), Is.EqualTo(true));
    }

    [UnityTest]
    public IEnumerator Director_TryAutoBindBedInteractable_ShouldIgnoreDoorOnlyRestProxy()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        Type bedInteractableType = ResolveTypeOrFail("Sunset.Story.SpringDay1BedInteractable");
        GameObject homeDoor = Track(new GameObject("HomeDoor", typeof(SpriteRenderer), typeof(BoxCollider2D)));
        homeDoor.tag = "Building";
        Assert.That(homeDoor.GetComponent(bedInteractableType), Is.Null);

        InvokePrivateMethod(context.director, "TryAutoBindBedInteractable");

        Assert.That(homeDoor.GetComponent(bedInteractableType), Is.Null, "只有门代理时，不应再把门口绑成床交互点。");
        Assert.That(GetPrivateFieldValue(context.director, "_boundBedInteractable"), Is.Null, "没有真实床位时，导演不应回退到门口睡觉交互。");
    }

    [UnityTest]
    public IEnumerator BeginDinnerConflict_ShouldNormalizeClockToSixPm()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        SetTimeWithoutSystems(context, 16, 45);
        InvokePrivateMethod(context.director, "BeginDinnerConflict");

        NormalizeManagedTimeTargetForCurrentPhase(context, 16, 45, out int dinnerFloorHour, out int dinnerFloorMinute);
        Assert.That(dinnerFloorHour, Is.EqualTo(18), "晚饭段的受管控时钟最早应被夹到 18:00。");
        Assert.That(dinnerFloorMinute, Is.EqualTo(0), "晚饭段的受管控时钟最早应被夹到 18:00。");
    }

    [UnityTest]
    public IEnumerator AlignTownDinnerGatheringActorsAndPlayer_ShouldPreferDinnerAreaOverVillageCrowdMarkers()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        Type npcMotionControllerType = ResolveTypeOrFail("NPCMotionController");

        GameObject crowdRoot = Track(new GameObject("EnterVillageCrowdRoot"));
        Transform chiefMarker = Track(new GameObject("001起点")).transform;
        chiefMarker.SetParent(crowdRoot.transform, false);
        chiefMarker.position = new Vector3(8.03f, 18.58f, 0f);

        Transform companionMarker = Track(new GameObject("002起点")).transform;
        companionMarker.SetParent(crowdRoot.transform, false);
        companionMarker.position = new Vector3(9.27f, 18.04f, 0f);

        GameObject dinnerAnchor = Track(new GameObject("DirectorReady_DinnerBackgroundRoot"));
        dinnerAnchor.transform.position = new Vector3(-17.2f, 8.6f, 0f);

        GameObject chief = Track(new GameObject("001"));
        chief.AddComponent(npcMotionControllerType);
        chief.transform.position = new Vector3(0f, 0f, 0f);

        GameObject companion = Track(new GameObject("002"));
        companion.AddComponent(npcMotionControllerType);
        companion.transform.position = new Vector3(0f, 0f, 0f);

        InvokePrivateMethod(context.director, "AlignTownDinnerGatheringActorsAndPlayer");

        Vector2 chiefPosition = chief.transform.position;
        Vector2 companionPosition = companion.transform.position;

        Assert.That(Vector2.Distance(chiefPosition, chiefMarker.position), Is.GreaterThan(5f), "晚饭开场时，001 不应再被重新扔回进村围观 marker。");
        Assert.That(Vector2.Distance(companionPosition, companionMarker.position), Is.GreaterThan(5f), "晚饭开场时，002 不应再被重新扔回进村围观 marker。");
        Assert.That(Vector2.Distance(chiefPosition, dinnerAnchor.transform.position), Is.LessThan(2.2f), "001 应落在晚饭区域附近，而不是继续站在围观左上角。");
        Assert.That(Vector2.Distance(companionPosition, dinnerAnchor.transform.position), Is.LessThan(3.1f), "002 应落在晚饭区域附近，而不是继续站在围观左上角。");

    }

    [UnityTest]
    public IEnumerator BeginDinnerConflict_ShouldKeepWaitingBeforeDinnerCueTimeout()
    {
        EnsureTestSceneActive("Town");

        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object dinnerPhase = ParseEnum(context.storyPhaseType, "DinnerConflict");
        InvokeInstance(context.storyManager, "ResetState", dinnerPhase, false);
        SetPrivateField(context.director, "_editorDinnerCueSettledOverride", false);

        InvokePrivateMethod(context.director, "BeginDinnerConflict");

        float waitStartedAt = (float)GetPrivateFieldValue(context.director, "_dinnerCueWaitStartedAt");
        Assert.That(waitStartedAt, Is.GreaterThanOrEqualTo(0f), "晚饭 table cue 未 settled 时，应先开始计时等待。");
        Assert.That(GetPrivateFieldValue(context.director, "_dinnerSequencePlayed"), Is.EqualTo(false), "超时前不应提前把晚饭正式对话吃掉。");
    }

    [UnityTest]
    public IEnumerator BeginDinnerConflict_ShouldForceAlignStoryActorsAfterDinnerCueTimeout()
    {
        EnsureTestSceneActive("Town");

        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        Type npcMotionControllerType = ResolveTypeOrFail("NPCMotionController");
        GameObject dinnerAnchor = Track(new GameObject("DirectorReady_DinnerBackgroundRoot"));
        dinnerAnchor.transform.position = new Vector3(-17.2f, 8.6f, 0f);

        GameObject chief = Track(new GameObject("001"));
        chief.AddComponent(npcMotionControllerType);
        chief.transform.position = new Vector3(5f, 5f, 0f);

        GameObject companion = Track(new GameObject("002"));
        companion.AddComponent(npcMotionControllerType);
        companion.transform.position = new Vector3(7f, 5f, 0f);

        object dinnerPhase = ParseEnum(context.storyPhaseType, "DinnerConflict");
        InvokeInstance(context.storyManager, "ResetState", dinnerPhase, false);
        SetPrivateField(context.director, "_editorDinnerCueSettledOverride", false);
        SetPrivateField(context.director, "dinnerCueSettleTimeout", 0f);
        SetPrivateField(context.director, "_dinnerCueWaitStartedAt", 0f);

        InvokePrivateMethod(context.director, "BeginDinnerConflict");

        Assert.That(GetPrivateFieldValue(context.director, "_dinnerSequencePlayed"), Is.EqualTo(true), "晚饭 cue 超时后，应直接开始正式剧情，不再无限等待。");
        Assert.That(GetPrivateFieldValue(context.director, "_dinnerCueWaitStartedAt"), Is.EqualTo(-1f), "超时兜底开戏后，等待计时应及时清零。");
        Assert.That(Vector2.Distance(chief.transform.position, dinnerAnchor.transform.position), Is.LessThan(2.2f), "超时后只应强制把 001 拉回晚饭区域。");
        Assert.That(Vector2.Distance(companion.transform.position, dinnerAnchor.transform.position), Is.LessThan(3.1f), "超时后只应强制把 002 拉回晚饭区域。");
    }

    [UnityTest]
    public IEnumerator WorkbenchInteractable_ShouldStayQuietBeforeWorkbenchPhase()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object crashPhase = ParseEnum(context.storyPhaseType, "CrashAndMeet");
        InvokeInstance(context.storyManager, "ResetState", crashPhase, false);

        Type workbenchType = ResolveTypeOrFail("Sunset.Story.CraftingStationInteractable");
        Component workbench = Track(new GameObject("Workbench_Test")).AddComponent(workbenchType);
        Assert.That(workbench, Is.Not.Null);

        bool canInteract = (bool)InvokeInstance(workbench, "CanInteract", null);
        Assert.That(canInteract, Is.False, "首段推进链进行时，工作台不应抢先暴露交互语义。");
    }

    [UnityTest]
    public IEnumerator Director_WorkbenchFallback_ShouldNotMarkCraftObjectiveComplete()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object farmingPhase = ParseEnum(context.storyPhaseType, "FarmingTutorial");
        object workbenchStation = ParseEnum(ResolveTypeOrFail("FarmGame.Data.CraftingStation"), "Workbench");
        InvokeInstance(context.storyManager, "ResetState", farmingPhase, false);

        SetPrivateField(context.director, "_farmingTutorialTrackingInitialized", true);
        SetPrivateField(context.director, "_tillObjectiveCompleted", true);
        SetPrivateField(context.director, "_plantObjectiveCompleted", true);
        SetPrivateField(context.director, "_waterObjectiveCompleted", true);
        SetPrivateField(context.director, "_woodObjectiveCompleted", true);
        SetPrivateField(context.director, "_craftedCount", 0);

        string result = (string)InvokeInstance(context.director, "TryHandleWorkbenchTestInteraction", workbenchStation);

        Assert.That(
            result,
            Is.EqualTo("工作台界面当前未接通，本次不会记作基础制作。等工作台真正打开后再完成这一步。"));
        Assert.That(
            (int)GetPrivateFieldValue(context.director, "_craftedCount"),
            Is.EqualTo(0),
            "workbench fallback 不应直接把基础制作记成已完成。");
        Assert.That(
            (string)InvokeInstance(context.director, "GetValidationFarmingNextAction"),
            Is.EqualTo("执行 Step，模拟一次基础制作并推进到和村长收口。"),
            "在未真实完成制作前，导演层仍应保留基础制作这一拍。");
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

        Canvas overlayCanvas = GetPrivateFieldValue(overlay, "overlayCanvas") as Canvas;
        Assert.That(overlayCanvas, Is.Not.Null, "WorkbenchOverlay 应持有运行时 Canvas");
        Assert.That(overlayCanvas.renderMode, Is.EqualTo(RenderMode.ScreenSpaceOverlay), "WorkbenchOverlay 运行时必须回到屏幕叠加层，不能继续沿用错误的 world-space 壳。");
    }

    [UnityTest]
    public IEnumerator WorkbenchOverlay_ShouldReplaceStaleWorldSpaceStaticInstance()
    {
        Type overlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1WorkbenchCraftingOverlay");

        GameObject staleRoot = Track(new GameObject("WorkbenchOverlay_Stale", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup)));
        Canvas staleCanvas = staleRoot.GetComponent<Canvas>();
        staleCanvas.renderMode = RenderMode.WorldSpace;
        Component staleOverlay = staleRoot.AddComponent(overlayType);
        SetStaticField(overlayType, "_instance", staleOverlay);

        InvokeStatic(overlayType, "EnsureRuntime");
        yield return null;

        Component runtimeOverlay = GetStaticPropertyValue(overlayType, "Instance") as Component;
        Assert.That(runtimeOverlay, Is.Not.Null, "WorkbenchOverlay 应能从 stale 静态实例恢复。");
        Assert.That(runtimeOverlay, Is.Not.SameAs(staleOverlay), "world-space 的旧静态实例不应继续截胡 runtime 真正使用的 WorkbenchOverlay。");

        Canvas runtimeCanvas = GetPrivateFieldValue(runtimeOverlay, "overlayCanvas") as Canvas;
        Assert.That(runtimeCanvas, Is.Not.Null);
        Assert.That(runtimeCanvas.renderMode, Is.EqualTo(RenderMode.ScreenSpaceOverlay));
    }

    [UnityTest]
    public IEnumerator WorkbenchOverlay_ShouldReplaceIncompleteRecipeShellStaticInstance()
    {
        Type overlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1WorkbenchCraftingOverlay");

        GameObject staleRoot = Track(new GameObject("WorkbenchOverlay_Incomplete", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup)));
        Canvas staleCanvas = staleRoot.GetComponent<Canvas>();
        staleCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        Component staleOverlay = staleRoot.AddComponent(overlayType);

        GameObject panelRoot = Track(new GameObject("PanelRoot", typeof(RectTransform)));
        panelRoot.transform.SetParent(staleRoot.transform, false);
        GameObject viewport = Track(new GameObject("Viewport", typeof(RectTransform)));
        viewport.transform.SetParent(panelRoot.transform, false);
        GameObject content = Track(new GameObject("Content", typeof(RectTransform)));
        content.transform.SetParent(viewport.transform, false);
        GameObject brokenRow = Track(new GameObject("RecipeRow_0", typeof(RectTransform), typeof(Image), typeof(Button)));
        brokenRow.transform.SetParent(content.transform, false);

        SetStaticField(overlayType, "_instance", staleOverlay);

        InvokeStatic(overlayType, "EnsureRuntime");
        yield return null;

        Component runtimeOverlay = GetStaticPropertyValue(overlayType, "Instance") as Component;
        Assert.That(runtimeOverlay, Is.Not.Null);
        Assert.That(runtimeOverlay, Is.Not.SameAs(staleOverlay), "左列 recipe 行文本链不完整的 screen-overlay 壳不应继续被复用。");
    }

    [UnityTest]
    public IEnumerator WorkbenchOverlay_ShouldReplaceManualRecipeShellStaticInstance()
    {
        Type overlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1WorkbenchCraftingOverlay");
        Type tmpType = ResolveTypeOrFail("TMPro.TextMeshProUGUI");

        GameObject staleRoot = Track(new GameObject("WorkbenchOverlay_ManualShell", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup)));
        staleRoot.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        Component staleOverlay = staleRoot.AddComponent(overlayType);

        GameObject panelRoot = Track(new GameObject("PanelRoot", typeof(RectTransform)));
        panelRoot.transform.SetParent(staleRoot.transform, false);
        GameObject viewport = Track(new GameObject("Viewport", typeof(RectTransform)));
        viewport.transform.SetParent(panelRoot.transform, false);
        GameObject content = Track(new GameObject("Content", typeof(RectTransform)));
        content.transform.SetParent(viewport.transform, false);

        GameObject row = Track(new GameObject("RecipeRow_0", typeof(RectTransform), typeof(Image), typeof(Button)));
        row.transform.SetParent(content.transform, false);
        Track(new GameObject("Accent", typeof(RectTransform), typeof(Image))).transform.SetParent(row.transform, false);
        Track(new GameObject("Icon", typeof(RectTransform), typeof(Image))).transform.SetParent(row.transform, false);
        Track(new GameObject("Name", typeof(RectTransform))).AddComponent(tmpType).transform.SetParent(row.transform, false);
        Track(new GameObject("Summary", typeof(RectTransform))).AddComponent(tmpType).transform.SetParent(row.transform, false);

        SetStaticField(overlayType, "_instance", staleOverlay);

        InvokeStatic(overlayType, "EnsureRuntime");
        yield return null;

        Component runtimeOverlay = GetStaticPropertyValue(overlayType, "Instance") as Component;
        Assert.That(runtimeOverlay, Is.Not.Null);
        Assert.That(runtimeOverlay, Is.Not.SameAs(staleOverlay), "旧 manual recipe 壳即使勉强带齐文本节点，也不应继续被 runtime 复用。");
    }

    [UnityTest]
    public IEnumerator WorkbenchOverlay_ShouldReplaceLegacyDetailManualShellStaticInstance()
    {
        Type overlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1WorkbenchCraftingOverlay");
        Type tmpType = ResolveTypeOrFail("TMPro.TextMeshProUGUI");

        GameObject staleRoot = Track(new GameObject("WorkbenchOverlay_LegacyDetail", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup)));
        staleRoot.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        Component staleOverlay = staleRoot.AddComponent(overlayType);

        GameObject panelRoot = Track(new GameObject("PanelRoot", typeof(RectTransform)));
        panelRoot.transform.SetParent(staleRoot.transform, false);
        GameObject viewport = Track(new GameObject("Viewport", typeof(RectTransform)));
        viewport.transform.SetParent(panelRoot.transform, false);
        GameObject content = Track(new GameObject("Content", typeof(RectTransform)));
        content.transform.SetParent(viewport.transform, false);
        GameObject generatedRow = Track(new GameObject("RecipeRow_0", typeof(RectTransform), typeof(Image), typeof(Button), typeof(HorizontalLayoutGroup)));
        generatedRow.transform.SetParent(content.transform, false);
        Track(new GameObject("Accent", typeof(RectTransform), typeof(Image))).transform.SetParent(generatedRow.transform, false);
        Track(new GameObject("Icon", typeof(RectTransform), typeof(Image))).transform.SetParent(generatedRow.transform, false);
        GameObject textColumn = Track(new GameObject("TextColumn", typeof(RectTransform)));
        textColumn.transform.SetParent(generatedRow.transform, false);
        Track(new GameObject("Name", typeof(RectTransform))).AddComponent(tmpType).transform.SetParent(textColumn.transform, false);
        Track(new GameObject("Summary", typeof(RectTransform))).AddComponent(tmpType).transform.SetParent(textColumn.transform, false);

        GameObject detailColumn = Track(new GameObject("DetailColumn", typeof(RectTransform)));
        detailColumn.transform.SetParent(panelRoot.transform, false);
        Track(new GameObject("SelectedName", typeof(RectTransform))).AddComponent(tmpType).transform.SetParent(detailColumn.transform, false);
        Track(new GameObject("SelectedDescription", typeof(RectTransform))).AddComponent(tmpType).transform.SetParent(detailColumn.transform, false);
        Track(new GameObject("MaterialsTitle", typeof(RectTransform))).transform.SetParent(detailColumn.transform, false);
        Track(new GameObject("MaterialsViewport", typeof(RectTransform))).transform.SetParent(detailColumn.transform, false);
        Track(new GameObject("QuantityControls", typeof(RectTransform))).transform.SetParent(detailColumn.transform, false);
        Track(new GameObject("ProgressBackground", typeof(RectTransform), typeof(Image), typeof(Button))).transform.SetParent(detailColumn.transform, false);
        GameObject craftButton = Track(new GameObject("CraftButton", typeof(RectTransform), typeof(Image), typeof(Button)));
        craftButton.transform.SetParent(detailColumn.transform, false);
        Track(new GameObject("Label", typeof(RectTransform))).AddComponent(tmpType).transform.SetParent(craftButton.transform, false);
        Track(new GameObject("QuantityValue", typeof(RectTransform))).AddComponent(tmpType).transform.SetParent(detailColumn.transform, false);
        Track(new GameObject("StageHint", typeof(RectTransform))).AddComponent(tmpType).transform.SetParent(detailColumn.transform, false);
        Track(new GameObject("Pointer", typeof(RectTransform))).transform.SetParent(panelRoot.transform, false);

        SetStaticField(overlayType, "_instance", staleOverlay);

        InvokeStatic(overlayType, "EnsureRuntime");
        yield return null;

        Component runtimeOverlay = GetStaticPropertyValue(overlayType, "Instance") as Component;
        Assert.That(runtimeOverlay, Is.Not.Null);
        Assert.That(runtimeOverlay, Is.Not.SameAs(staleOverlay), "旧 detail 手工壳不应继续被 runtime 复用，否则左列和右侧布局会继续沿用坏基线。");
    }

    [UnityTest]
    public IEnumerator WorkbenchOverlay_RuntimeRecipeRows_ShouldUseDirectPrefabStyleChildren()
    {
        Type overlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1WorkbenchCraftingOverlay");

        InvokeStatic(overlayType, "EnsureRuntime");
        Component overlay = GetStaticPropertyValue(overlayType, "Instance") as Component;
        Assert.That(overlay, Is.Not.Null);
        Track(overlay.gameObject);
        yield return null;

        RectTransform recipeContentRect = GetPrivateFieldValue(overlay, "recipeContentRect") as RectTransform;
        Assert.That(recipeContentRect, Is.Not.Null, "WorkbenchOverlay 应持有左列 recipe content。");

        bool hasVisibleRow = false;
        for (int index = 0; index < recipeContentRect.childCount; index++)
        {
            if (recipeContentRect.GetChild(index) is not RectTransform rowRect || !rowRect.name.StartsWith("RecipeRow_") || !rowRect.gameObject.activeSelf)
            {
                continue;
            }

            hasVisibleRow = true;
            HorizontalLayoutGroup rowLayout = rowRect.GetComponent<HorizontalLayoutGroup>();
            Assert.That(rowLayout, Is.Not.Null, "runtime recipe 行应回到稳定生成式布局，而不是继续挂在坏掉的手工壳上。");
            Assert.That(rowLayout.childControlWidth, Is.True, "runtime recipe 行必须真实给子项分配宽度，不然 icon 和文字会继续被压成空壳。");
            Assert.That(rowLayout.childControlHeight, Is.True, "runtime recipe 行必须真实给子项分配高度，不然 text column 和 icon card 会继续拿不到尺寸。");

            Transform textColumn = rowRect.Find("TextColumn");
            Assert.That(textColumn, Is.Not.Null, "runtime recipe 行应持有 TextColumn，统一承载名称和摘要。");

            Transform name = textColumn.Find("Name");
            Transform summary = textColumn.Find("Summary");
            Transform iconCard = rowRect.Find("IconCard");
            Assert.That(name, Is.Not.Null, "runtime recipe 行应直接持有 Name 文本节点。");
            Assert.That(summary, Is.Not.Null, "runtime recipe 行应直接持有 Summary 文本节点。");
            Assert.That(iconCard, Is.Not.Null, "runtime recipe 行应持有独立 icon card，不能再只剩背景高亮。");
            Assert.That(name.parent, Is.EqualTo(textColumn), "Name 应挂在 TextColumn 下，跟摘要保持稳定布局关系。");
            Assert.That(summary.parent, Is.EqualTo(textColumn), "Summary 应挂在 TextColumn 下，跟名称保持稳定布局关系。");

            TMPro.TextMeshProUGUI nameText = name.GetComponent<TMPro.TextMeshProUGUI>();
            TMPro.TextMeshProUGUI summaryText = summary.GetComponent<TMPro.TextMeshProUGUI>();
            Assert.That(nameText, Is.Not.Null, "Name 节点应继续使用 TMP 文本。");
            Assert.That(summaryText, Is.Not.Null, "Summary 节点应继续使用 TMP 文本。");
            Assert.That(nameText.text, Is.Not.Empty, "runtime recipe 行的名称文本应已真实填充。");
            Assert.That(summaryText.text, Is.Not.Empty, "runtime recipe 行的摘要文本应已真实填充。");
            Assert.That(((RectTransform)iconCard).rect.width, Is.GreaterThan(1f), "icon card 应拿到真实宽度。");
            Assert.That(((RectTransform)iconCard).rect.height, Is.GreaterThan(1f), "icon card 应拿到真实高度。");
            Assert.That(nameText.rectTransform.rect.width, Is.GreaterThan(1f), "名称文本应拿到真实宽度。");
            Assert.That(nameText.rectTransform.rect.height, Is.GreaterThan(1f), "名称文本应拿到真实高度。");
            Assert.That(summaryText.rectTransform.rect.width, Is.GreaterThan(1f), "摘要文本应拿到真实宽度。");
            Assert.That(summaryText.rectTransform.rect.height, Is.GreaterThan(1f), "摘要文本应拿到真实高度。");
        }

        Assert.That(hasVisibleRow, Is.True, "runtime workbench 至少应绑定出一行可见 recipe。");
    }

    [Test]
    public void WorkbenchOverlay_EnsureRecipeRowCompatibility_ShouldPreserveRecipeShellSpritesWhileClearingBadMaterials()
    {
        Type overlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1WorkbenchCraftingOverlay");

        Component overlay = Track(new GameObject("WorkbenchOverlay_Test")).AddComponent(overlayType);
        GameObject row = Track(new GameObject("RecipeRow_0", typeof(RectTransform), typeof(Image), typeof(Button), typeof(HorizontalLayoutGroup)));
        Image rowBackground = row.GetComponent<Image>();
        GameObject accent = Track(new GameObject("Accent", typeof(RectTransform), typeof(Image)));
        accent.transform.SetParent(row.transform, false);
        Image accentImage = accent.GetComponent<Image>();
        GameObject iconCard = Track(new GameObject("IconCard", typeof(RectTransform), typeof(Image)));
        iconCard.transform.SetParent(row.transform, false);
        Image iconCardImage = iconCard.GetComponent<Image>();
        GameObject icon = Track(new GameObject("Icon", typeof(RectTransform), typeof(Image)));
        icon.transform.SetParent(iconCard.transform, false);
        Image iconImage = icon.GetComponent<Image>();
        GameObject textColumn = Track(new GameObject("TextColumn", typeof(RectTransform)));
        textColumn.transform.SetParent(row.transform, false);
        Track(new GameObject("Name", typeof(RectTransform), typeof(TMPro.TextMeshProUGUI))).transform.SetParent(textColumn.transform, false);
        Track(new GameObject("Summary", typeof(RectTransform), typeof(TMPro.TextMeshProUGUI))).transform.SetParent(textColumn.transform, false);

        Sprite backgroundSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 8f, 8f), new Vector2(0.5f, 0.5f));
        Sprite accentSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 8f, 8f), new Vector2(0.5f, 0.5f));
        Sprite iconCardSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 8f, 8f), new Vector2(0.5f, 0.5f));
        Sprite iconSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 8f, 8f), new Vector2(0.5f, 0.5f));
        rowBackground.sprite = backgroundSprite;
        accentImage.sprite = accentSprite;
        iconCardImage.sprite = iconCardSprite;
        iconImage.sprite = iconSprite;

        Material badMaterial = new Material(Shader.Find("UI/Default")) { name = "Font Material" };
        rowBackground.material = badMaterial;
        accentImage.material = badMaterial;
        iconCardImage.material = badMaterial;
        iconImage.material = badMaterial;

        InvokePrivateMethod(overlay, "EnsureRecipeRowCompatibility", row.GetComponent<RectTransform>());

        Assert.That(rowBackground.sprite, Is.SameAs(backgroundSprite), "row 背景图不应在兼容修复时被一起清空。");
        Assert.That(accentImage.sprite, Is.SameAs(accentSprite), "accent 图不应在兼容修复时被一起清空。");
        Assert.That(iconCardImage.sprite, Is.SameAs(iconCardSprite), "iconCard 图不应在兼容修复时被一起清空。");
        Assert.That(iconImage.sprite, Is.SameAs(iconSprite), "真正的物品 icon 不应在兼容修复时被一起清空。");
        Assert.That(rowBackground.material, Is.Null, "错误字体材质应被清掉，而不是继续污染 row 背景。");
        Assert.That(accentImage.material, Is.Null);
        Assert.That(iconCardImage.material, Is.Null);
        Assert.That(iconImage.material, Is.Null);
    }

    [Test]
    public void WorkbenchOverlay_ResolveItem_ShouldFallbackToMasterDatabaseWhenServicesAreMissing()
    {
        Type overlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1WorkbenchCraftingOverlay");
        Type itemDatabaseType = ResolveTypeOrFail("FarmGame.Data.ItemDatabase");
        Component overlay = Track(new GameObject("WorkbenchOverlay_Test")).AddComponent(overlayType);

        ScriptableObject database = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/111_Data/Database/MasterItemDatabase.asset", itemDatabaseType) as ScriptableObject;
        Assert.That(database, Is.Not.Null, "测试前提失败：应能加载 MasterItemDatabase。");
        IList allItems = GetFieldValue(database, "allItems") as IList;
        Assert.That(allItems, Is.Not.Null.And.Not.Empty, "测试前提失败：MasterItemDatabase 应至少包含一个物品。");

        object expectedItem = null;
        for (int index = 0; index < allItems.Count; index++)
        {
            if (allItems[index] != null)
            {
                expectedItem = allItems[index];
                break;
            }
        }

        Assert.That(expectedItem, Is.Not.Null, "测试前提失败：MasterItemDatabase 中至少应有一个有效物品。");
        int expectedItemId = (int)GetFieldValue(expectedItem, "itemID");

        object resolved = InvokePrivateMethod(overlay, "ResolveItem", expectedItemId);
        Assert.That(resolved, Is.Not.Null, "当服务侧 Database 尚未绑上时，Workbench 仍应能回退到 MasterItemDatabase。");
        Assert.That((int)GetFieldValue(resolved, "itemID"), Is.EqualTo(expectedItemId));
    }

    [UnityTest]
    public IEnumerator WorkbenchOverlay_ProgressLabels_ShouldCountQueueExcludingCurrentActiveCraft()
    {
        Type overlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1WorkbenchCraftingOverlay");
        Type recipeType = ResolveTypeOrFail("FarmGame.Data.RecipeData");
        Type queueEntryType = overlayType.GetNestedType("WorkbenchQueueEntry", BindingFlags.NonPublic);
        Assert.That(queueEntryType, Is.Not.Null, "应能拿到 WorkbenchQueueEntry 运行时类型。");

        InvokeStatic(overlayType, "EnsureRuntime");
        Component overlay = GetStaticPropertyValue(overlayType, "Instance") as Component;
        Assert.That(overlay, Is.Not.Null);
        Track(overlay.gameObject);
        yield return null;

        ScriptableObject recipe = ScriptableObject.CreateInstance(recipeType);
        Coroutine runningRoutine = ((MonoBehaviour)overlay).StartCoroutine(KeepCoroutineAlive());
        GameObject anchor = Track(new GameObject("WorkbenchAnchor_Test"));
        object queueEntry = Activator.CreateInstance(queueEntryType);
        Assert.That(queueEntry, Is.Not.Null);

        SetFieldValue(queueEntry, "recipe", recipe);
        SetFieldValue(queueEntry, "recipeId", 901);
        SetFieldValue(queueEntry, "resultItemId", 901);
        SetFieldValue(queueEntry, "resultAmountPerCraft", 1);
        SetFieldValue(queueEntry, "totalCount", 1);
        SetFieldValue(queueEntry, "readyCount", 0);

        IList queueEntries = GetPrivateFieldValue(overlay, "_queueEntries") as IList;
        Assert.That(queueEntries, Is.Not.Null);
        queueEntries.Clear();
        queueEntries.Add(queueEntry);

        SetPrivateField(overlay, "_craftRoutine", runningRoutine);
        SetPrivateField(overlay, "_craftingRecipe", recipe);
        SetPrivateField(overlay, "_activeQueueEntry", queueEntry);
        SetPrivateField(overlay, "_craftQueueTotal", 1);
        SetPrivateField(overlay, "_craftQueueCompleted", 0);
        SetPrivateField(overlay, "_craftProgress", 0.35f);
        SetPrivateField(overlay, "_anchorTarget", anchor.transform);
        SetPrivateField(overlay, "_isVisible", false);

        InvokePrivateMethod(overlay, "UpdateProgressLabel", recipe);
        InvokePrivateMethod(overlay, "UpdateFloatingProgressVisibility");

        Component progressLabel = GetPrivateFieldValue(overlay, "progressLabelText") as Component;
        Component floatingLabel = GetPrivateFieldValue(overlay, "floatingProgressLabel") as Component;
        Assert.That(GetPropertyValue(progressLabel, "text"), Is.EqualTo("进度  0/1"), "当前单件已经开工时，大面板应显示已完成数量/总制作数量。");
        Assert.That(GetPropertyValue(floatingLabel, "text"), Is.EqualTo("进度  0/1"), "离台小进度条应与大面板保持同一套数量语义。");

        ((MonoBehaviour)overlay).StopCoroutine(runningRoutine);
        UnityEngine.Object.DestroyImmediate(recipe);
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

    private static object GetFieldValue(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        return field.GetValue(target);
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

    private static bool InvokeWorkbenchCraftPermission(object director, out string blockerMessage)
    {
        MethodInfo method = director.GetType().GetMethod("CanPerformWorkbenchCraft", BindingFlags.Instance | BindingFlags.Public);
        Assert.That(method, Is.Not.Null, $"未找到方法: {director.GetType().Name}.CanPerformWorkbenchCraft");

        object[] arguments = { null };
        object result = method.Invoke(director, arguments);
        blockerMessage = arguments[0] as string ?? string.Empty;
        return result is bool boolResult && boolResult;
    }

    private static object CreateCompletedEvent(string sequenceId, object currentPhase)
    {
        Type completedEventType = ResolveTypeOrFail("Sunset.Events.DialogueSequenceCompletedEvent");
        object completedEvent = Activator.CreateInstance(completedEventType);
        SetWritableMember(completedEvent, "SequenceId", sequenceId);
        SetWritableMember(completedEvent, "FollowupSequence", null);
        SetWritableMember(completedEvent, "CurrentPhase", currentPhase);
        return completedEvent;
    }

    private static void SetWritableMember(object target, string memberName, object value)
    {
        PropertyInfo property = target.GetType().GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public);
        if (property != null && property.CanWrite)
        {
            property.SetValue(target, value);
            return;
        }

        FieldInfo field = target.GetType().GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到可写成员: {target.GetType().Name}.{memberName}");
        field.SetValue(target, value);
    }

    private void EnsureTestSceneActive(string sceneName)
    {
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            return;
        }

        string tempSceneDirectory = Path.Combine("Temp", "CodexEditModeScenes", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), tempSceneDirectory));
        temporaryScenePath = Path.Combine(tempSceneDirectory, $"{sceneName}.unity").Replace("\\", "/");

        Scene tempScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        bool saved = EditorSceneManager.SaveScene(tempScene, temporaryScenePath, saveAsCopy: false);
        Assert.That(saved, Is.True, $"应能创建用于 late-day 睡眠 fallback 验证的临时 {sceneName} 场景。");
        Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(sceneName), $"late-day 睡眠 fallback 验证应在名为 {sceneName} 的场景上下文内执行。");
    }

    private void CleanupTemporaryScene()
    {
        if (string.IsNullOrWhiteSpace(temporaryScenePath))
        {
            return;
        }

        if (SceneManager.GetActiveScene().path == temporaryScenePath)
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        string absoluteScenePath = Path.Combine(Directory.GetCurrentDirectory(), temporaryScenePath);
        if (File.Exists(absoluteScenePath))
        {
            File.Delete(absoluteScenePath);
        }

        string metaPath = $"{absoluteScenePath}.meta";
        if (File.Exists(metaPath))
        {
            File.Delete(metaPath);
        }

        string absoluteDirectory = Path.GetDirectoryName(absoluteScenePath);
        if (!string.IsNullOrWhiteSpace(absoluteDirectory) &&
            Directory.Exists(absoluteDirectory) &&
            Directory.GetFileSystemEntries(absoluteDirectory).Length == 0)
        {
            Directory.Delete(absoluteDirectory);
        }

        temporaryScenePath = null;
    }

    private static void SetTimeWithoutSystems(RuntimeContext context, int hour, int minute, int day = 1)
    {
        SetPrivateField(context.timeManager, "currentYear", 1);
        SetPrivateField(context.timeManager, "currentSeason", ParseEnum(context.seasonType, "Spring"));
        SetPrivateField(context.timeManager, "currentDay", day);
        SetPrivateField(context.timeManager, "currentHour", hour);
        SetPrivateField(context.timeManager, "currentMinute", minute);
    }

    private static void NormalizeManagedTimeTargetForCurrentPhase(
        RuntimeContext context,
        int requestedHour,
        int requestedMinute,
        out int normalizedHour,
        out int normalizedMinute)
    {
        MethodInfo method = context.director.GetType().GetMethod("NormalizeManagedDay1TimeTarget", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, "应能读取导演层的分钟级 Day1 时钟夹制逻辑。");

        object managedSeason = ParseEnum(context.seasonType, "Spring");
        object currentPhase = GetPropertyValue(context.storyManager, "CurrentPhase");
        object[] arguments =
        {
            1,
            managedSeason,
            1,
            requestedHour,
            requestedMinute,
            currentPhase,
            0,
            managedSeason,
            0,
            0,
            0
        };

        method.Invoke(context.director, arguments);
        normalizedHour = (int)arguments[9];
        normalizedMinute = (int)arguments[10];
    }

    private static object CreatePromptRowState(Type promptOverlayType, string label, string detail, bool completed)
    {
        Type promptRowStateType = promptOverlayType.GetNestedType("PromptRowState", BindingFlags.NonPublic);
        Assert.That(promptRowStateType, Is.Not.Null, "应能解析 PromptOverlay 内部的 PromptRowState 类型。");

        object state = Activator.CreateInstance(promptRowStateType, nonPublic: true);
        Assert.That(state, Is.Not.Null, "应能创建 PromptRowState 测试实例。");

        SetFieldValue(state, "Label", label);
        SetFieldValue(state, "Detail", detail);
        SetFieldValue(state, "Completed", completed);
        return state;
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

    private static void SetFieldValue(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
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

    private static System.Collections.IEnumerator KeepCoroutineAlive()
    {
        while (true)
        {
            yield return null;
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
