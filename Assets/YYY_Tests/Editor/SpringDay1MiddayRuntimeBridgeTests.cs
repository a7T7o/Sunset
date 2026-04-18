using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[TestFixture]
public class SpringDay1MiddayRuntimeBridgeTests
{
    private readonly List<GameObject> createdObjects = new();
    private string temporaryPrimaryScenePath;

    [TearDown]
    public void TearDown()
    {
        CleanupTemporaryPrimaryScene();

        for (int index = createdObjects.Count - 1; index >= 0; index--)
        {
            if (createdObjects[index] != null)
            {
                UnityEngine.Object.DestroyImmediate(createdObjects[index]);
            }
        }

        createdObjects.Clear();
        ResetStaticField(ResolveTypeOrFail("Sunset.Story.SpringDay1Director"), "_instance");
        ResetStaticField(ResolveTypeOrFail("Sunset.Story.StoryManager"), "_instance");
        ResetStaticField(ResolveTypeOrFail("Sunset.Story.DialogueManager"), "<Instance>k__BackingField");
        ResetStaticField(ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay"), "_instance");
        ResetStaticField(ResolveTypeOrFail("TimeManager"), "instance");
        ResetStaticField(ResolveTypeOrFail("EnergySystem"), "<Instance>k__BackingField");
        ResetStaticField(ResolveTypeOrFail("HealthSystem"), "_instance");
        ClearStaticCollection(ResolveTypeOrFail("Sunset.Story.DialogueManager"), "RuntimeCompletedSequenceIds");
    }

    [Test]
    public void Director_ShouldPreferAuthoredDialogueAssetsForMiddayPhases()
    {
        RuntimeContext context = CreateRuntimeContext();

        UnityEngine.Object healing = InvokePrivateMethod(context.director, "BuildHealingSequence") as UnityEngine.Object;
        UnityEngine.Object workbench = InvokePrivateMethod(context.director, "BuildWorkbenchSequence") as UnityEngine.Object;
        UnityEngine.Object dinner = InvokePrivateMethod(context.director, "BuildDinnerSequence") as UnityEngine.Object;
        UnityEngine.Object reminder = InvokePrivateMethod(context.director, "BuildReminderSequence") as UnityEngine.Object;
        UnityEngine.Object freeTime = InvokePrivateMethod(context.director, "BuildFreeTimeIntroSequence") as UnityEngine.Object;

        Assert.That(healing, Is.Not.Null);
        Assert.That(workbench, Is.Not.Null);
        Assert.That(dinner, Is.Not.Null);
        Assert.That(reminder, Is.Not.Null);
        Assert.That(freeTime, Is.Not.Null);

        Assert.That(healing.name, Is.EqualTo("SpringDay1_Healing"));
        Assert.That(workbench.name, Is.EqualTo("SpringDay1_WorkbenchRecall"));
        Assert.That(dinner.name, Is.EqualTo("SpringDay1_DinnerConflict"));
        Assert.That(reminder.name, Is.EqualTo("SpringDay1_ReturnReminder"));
        Assert.That(freeTime.name, Is.EqualTo("SpringDay1_FreeTimeOpening"));
    }

    [Test]
    public void HealingCompletion_ShouldAdvanceIntoWorkbenchFlashback()
    {
        RuntimeContext context = CreateRuntimeContext();
        object healingPhase = ParseEnum(context.storyPhaseType, "HealingAndHP");
        object workbenchPhase = ParseEnum(context.storyPhaseType, "WorkbenchFlashback");
        InvokeInstance(context.storyManager, "ResetState", healingPhase, false);

        object completedEvent = CreateCompletedEvent("spring-day1-healing", healingPhase);
        InvokePrivateMethod(context.director, "HandleDialogueSequenceCompleted", completedEvent);

        Assert.That(GetPropertyValue(context.storyManager, "CurrentPhase"), Is.EqualTo(workbenchPhase), "疗伤对白收束后应进入工作台闪回阶段。");
        Assert.That((string)InvokeInstance(context.director, "GetCurrentTaskLabel"), Is.EqualTo("0.0.4 工作台闪回"));
        Assert.That((string)InvokeInstance(context.director, "GetCurrentFocusTextForTests"), Is.EqualTo("靠近 Anvil_0，按 E 打开工作台。"));
    }

    [Test]
    public void WorkbenchCompletion_ShouldAdvanceIntoFarmingTutorial()
    {
        RuntimeContext context = CreateRuntimeContext();
        object workbenchPhase = ParseEnum(context.storyPhaseType, "WorkbenchFlashback");
        object farmingPhase = ParseEnum(context.storyPhaseType, "FarmingTutorial");
        InvokeInstance(context.storyManager, "ResetState", workbenchPhase, false);

        InvokePrivateMethod(
            context.director,
            "HandleDialogueSequenceCompleted",
            CreateCompletedEvent("spring-day1-workbench", workbenchPhase));

        Assert.That(GetPropertyValue(context.storyManager, "CurrentPhase"), Is.EqualTo(farmingPhase), "工作台回忆收束后应进入农田教学。");
        Assert.That((string)InvokeInstance(context.director, "GetCurrentTaskLabel"), Is.EqualTo("0.0.5 农田/砍树教学"));
        Assert.That((string)InvokeInstance(context.director, "GetCurrentFocusTextForTests"), Is.EqualTo("按 V 开启放置模式，再用锄头开垦一格土地。"));
    }

    [Test]
    public void FarmingTutorialCompletion_ShouldAwaitChiefWrapBeforeExploreWindow()
    {
        RuntimeContext context = CreateRuntimeContext();
        object farmingPhase = ParseEnum(context.storyPhaseType, "FarmingTutorial");
        InvokeInstance(context.storyManager, "ResetState", farmingPhase, false);

        SetPrivateField(context.director, "_farmingTutorialTrackingInitialized", true);
        SetPrivateField(context.director, "_tillObjectiveCompleted", true);
        SetPrivateField(context.director, "_plantObjectiveCompleted", true);
        SetPrivateField(context.director, "_waterObjectiveCompleted", true);
        SetPrivateField(context.director, "_woodObjectiveCompleted", true);
        SetPrivateField(context.director, "_craftedCount", 1);

        InvokePrivateMethod(context.director, "TickFarmingTutorial");

        Assert.That(GetPropertyValue(context.storyManager, "CurrentPhase"), Is.EqualTo(farmingPhase), "农田教学收束后不应立刻跳晚饭，而应先等和村长收口。");
        Assert.That((string)InvokeInstance(context.director, "GetCurrentTaskLabel"), Is.EqualTo("0.0.5 农田教学收口"));
        Assert.That((string)InvokeInstance(context.director, "GetCurrentFocusTextForTests"), Is.EqualTo("先去和村长说一声，把白天这套活收住，傍晚的自由活动才会正式打开。"));
        Assert.That((string)InvokeInstance(context.director, "GetCurrentProgressLabel"), Is.EqualTo("农田与工作台目标已完成，等待和村长收口"));
        Assert.That((bool)InvokeInstance(context.director, "IsDinnerDialoguePendingStart"), Is.False, "和村长收口前不应伪装成晚饭 formal 已 ready。");
    }

    [Test]
    public void DinnerAndReminderPhases_ShouldKeepWorkbenchAvailableAfterUnlock()
    {
        RuntimeContext context = CreateRuntimeContext();
        object dinnerPhase = ParseEnum(context.storyPhaseType, "DinnerConflict");
        object reminderPhase = ParseEnum(context.storyPhaseType, "ReturnAndReminder");

        InvokeInstance(context.storyManager, "ResetState", dinnerPhase, false);
        bool dinnerCanCraft = InvokeWorkbenchCraftPermission(context.director, out string dinnerBlocker);

        Assert.That(dinnerCanCraft, Is.True, "完成 0.0.4 进入 0.0.5 之后，工作台不应再被晚餐冲突重新锁回去。");
        Assert.That(dinnerBlocker, Is.EqualTo(string.Empty));
        Assert.That((bool)InvokeInstance(context.director, "ShouldExposeWorkbenchInteraction"), Is.True, "完成 0.0.4 进入 0.0.5 之后，工作台交互提示不应再被正式剧情整体禁用。");

        InvokeInstance(context.storyManager, "ResetState", reminderPhase, false);
        bool reminderCanCraft = InvokeWorkbenchCraftPermission(context.director, out string reminderBlocker);

        Assert.That(reminderCanCraft, Is.True, "完成 0.0.4 进入 0.0.5 之后，工作台不应再被归途提醒重新锁回去。");
        Assert.That(reminderBlocker, Is.EqualTo(string.Empty));
        Assert.That((bool)InvokeInstance(context.director, "ShouldExposeWorkbenchInteraction"), Is.True, "完成 0.0.4 进入 0.0.5 之后，工作台交互提示不应再被归途提醒整体禁用。");
    }

    [Test]
    public void DinnerAndReminderCompletion_ShouldBridgeIntoFreeTime()
    {
        RuntimeContext context = CreateRuntimeContext();
        object dinnerPhase = ParseEnum(context.storyPhaseType, "DinnerConflict");
        object reminderPhase = ParseEnum(context.storyPhaseType, "ReturnAndReminder");
        object freeTimePhase = ParseEnum(context.storyPhaseType, "FreeTime");
        InvokeInstance(context.storyManager, "ResetState", dinnerPhase, false);

        InvokePrivateMethod(
            context.director,
            "HandleDialogueSequenceCompleted",
            CreateCompletedEvent("spring-day1-dinner", dinnerPhase));

        Assert.That(GetPropertyValue(context.storyManager, "CurrentPhase"), Is.EqualTo(reminderPhase), "晚餐对白收束后应进入归途提醒。");

        InvokePrivateMethod(
            context.director,
            "HandleDialogueSequenceCompleted",
            CreateCompletedEvent("spring-day1-reminder", reminderPhase));

        Assert.That(GetPropertyValue(context.storyManager, "CurrentPhase"), Is.EqualTo(freeTimePhase), "归途提醒收束后应进入自由时段。");
        Assert.That((string)InvokeInstance(context.director, "GetCurrentTaskLabel"), Is.EqualTo("0.0.6 自由时段"));
        Assert.That((string)InvokeInstance(context.director, "GetCurrentProgressLabel"), Is.EqualTo("等待自由时段见闻接管"));
        Assert.That(GetPrivateField<bool>(context.director, "_freeTimeEntered"), Is.True, "进入自由时段后应显式标记 free-time 已开启。");
        Assert.That(GetPrivateField<bool>(context.director, "_freeTimeIntroQueued"), Is.True, "进入自由时段后应排队夜间见闻包。");
        Assert.That((bool)InvokeInstance(context.director, "IsSleepInteractionAvailable"), Is.True, "按当前语义，19:30 一进入自由时段，床就应立刻可用；夜间见闻不再阻止玩家直接睡觉收束。");
    }

    [Test]
    public void OneShotSummary_ShouldBackfillPhaseImpliedCompletedSequencesAfterDialogueRuntimeReset()
    {
        RuntimeContext context = CreateRuntimeContext();
        Type dialogueManagerType = ResolveTypeOrFail("Sunset.Story.DialogueManager");
        Component dialogueManager = Track(new GameObject("DialogueManager_Test")).AddComponent(dialogueManagerType);
        SetStaticField(dialogueManagerType, "<Instance>k__BackingField", dialogueManager);

        object dayEndPhase = ParseEnum(context.storyPhaseType, "DayEnd");
        InvokeInstance(context.storyManager, "ResetState", dayEndPhase, true);
        InvokeInstance(
            dialogueManager,
            "ReplaceCompletedSequenceIds",
            (object)new[] { "spring-day1-dinner", "spring-day1-reminder", "spring-day1-free-time-opening" });

        string summary = (string)InvokeInstance(context.director, "GetOneShotProgressSummary");

        StringAssert.Contains("healing=True", summary, "进入 DayEnd 后，疗伤 formal 不应因跨场景 runtime reset 被误判成未消费。");
        StringAssert.Contains("workbench=True", summary, "进入 DayEnd 后，工作台 formal 不应因跨场景 runtime reset 被误判成未消费。");
        StringAssert.Contains("dinner=True", summary, "晚餐 formal 自身也应保持已消费。");
        StringAssert.Contains("reminder=True", summary, "归途提醒 formal 自身也应保持已消费。");
        StringAssert.Contains("freeTimeIntro=True", summary, "自由时段 intro 在 DayEnd 不应再掉回未消费。");

        Assert.That(InvokeInstance(dialogueManager, "HasCompletedSequence", "spring-day1-healing"), Is.EqualTo(true));
        Assert.That(InvokeInstance(dialogueManager, "HasCompletedSequence", "spring-day1-workbench"), Is.EqualTo(true));
    }

    private RuntimeContext CreateRuntimeContext()
    {
        EnsureTestPrimarySceneActive();

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        InvokeStatic(promptOverlayType, "EnsureRuntime");
        Component promptOverlay = GetStaticPropertyValue(promptOverlayType, "Instance") as Component;
        if (promptOverlay != null)
        {
            Track(promptOverlay.gameObject);
        }

        Type directorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Type storyManagerType = ResolveTypeOrFail("Sunset.Story.StoryManager");
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Type timeManagerType = ResolveTypeOrFail("TimeManager");
        Type energySystemType = ResolveTypeOrFail("EnergySystem");
        Type healthSystemType = ResolveTypeOrFail("HealthSystem");
        Type playerMovementType = ResolveTypeOrFail("PlayerMovement");

        Component director = Track(new GameObject("SpringDay1Director_Test")).AddComponent(directorType);
        Component storyManager = Track(new GameObject("StoryManager_Test")).AddComponent(storyManagerType);
        Component timeManager = Track(new GameObject("TimeManager_Test")).AddComponent(timeManagerType);
        Component energySystem = Track(new GameObject("EnergySystem_Test")).AddComponent(energySystemType);
        Component healthSystem = Track(new GameObject("HealthSystem_Test")).AddComponent(healthSystemType);

        GameObject playerGo = Track(new GameObject("PlayerMovement_Test"));
        playerGo.AddComponent<Rigidbody2D>();
        playerGo.AddComponent<BoxCollider2D>();
        Component playerMovement = playerGo.AddComponent(playerMovementType);

        SetStaticField(directorType, "_instance", director);
        SetStaticField(storyManagerType, "_instance", storyManager);
        SetStaticField(timeManagerType, "instance", timeManager);
        SetStaticField(energySystemType, "<Instance>k__BackingField", energySystem);
        SetStaticField(healthSystemType, "_instance", healthSystem);

        SetPrivateField(timeManager, "showDebugInfo", false);
        SetPrivateField(timeManager, "enableHourEvent", false);
        SetPrivateField(timeManager, "enableMinuteEvent", false);
        SetPrivateField(timeManager, "enableDayEvent", false);
        SetPrivateField(timeManager, "enableYearEvent", false);
        SetPrivateField(timeManager, "enableSeasonChangeEvent", false);

        SetPrivateField(energySystem, "maxEnergy", 200);
        SetPrivateField(energySystem, "currentEnergy", 200);
        SetPrivateField(energySystem, "showDebugInfo", false);

        SetPrivateField(healthSystem, "maxHealth", 100);
        SetPrivateField(healthSystem, "currentHealth", 100);
        SetPrivateField(healthSystem, "showDebugInfo", false);

        SetPrivateField(director, "_playerMovement", playerMovement);

        return new RuntimeContext(director, storyManager, storyPhaseType);
    }

    private GameObject Track(GameObject gameObject)
    {
        createdObjects.Add(gameObject);
        return gameObject;
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

    private static object ParseEnum(Type enumType, string name)
    {
        return Enum.Parse(enumType, name);
    }

    private static void InvokeStatic(Type type, string methodName, params object[] args)
    {
        MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到静态方法: {type.Name}.{methodName}");
        method.Invoke(null, args);
    }

    private static object InvokeInstance(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return method.Invoke(target, args);
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

    private static object GetPropertyValue(object target, string propertyName)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到属性: {target.GetType().Name}.{propertyName}");
        return property.GetValue(target);
    }

    private static object GetStaticPropertyValue(Type type, string propertyName)
    {
        PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到静态属性: {type.Name}.{propertyName}");
        return property.GetValue(null);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private static T GetPrivateField<T>(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        return (T)field.GetValue(target);
    }

    private static void SetStaticField(Type type, string fieldName, object value)
    {
        FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到静态字段: {type.Name}.{fieldName}");
        field.SetValue(null, value);
    }

    private static void ResetStaticField(Type type, string fieldName)
    {
        FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(null, null);
        }
    }

    private static void ClearStaticCollection(Type type, string fieldName)
    {
        FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        object value = field?.GetValue(null);
        MethodInfo clearMethod = value?.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
        clearMethod?.Invoke(value, null);
    }

    private static Type ResolveTypeOrFail(string fullName)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int index = 0; index < assemblies.Length; index++)
        {
            Type resolved = assemblies[index].GetType(fullName, throwOnError: false);
            if (resolved != null)
            {
                return resolved;
            }
        }

        Assert.Fail($"缺少类型：{fullName}");
        return null;
    }

    private void EnsureTestPrimarySceneActive()
    {
        if (SceneManager.GetActiveScene().name == "Primary")
        {
            return;
        }

        string tempSceneDirectory = Path.Combine("Temp", "CodexEditModeScenes");
        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), tempSceneDirectory));
        temporaryPrimaryScenePath = Path.Combine(tempSceneDirectory, "Primary.unity").Replace("\\", "/");

        Scene tempScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        bool saved = EditorSceneManager.SaveScene(tempScene, temporaryPrimaryScenePath, saveAsCopy: false);
        Assert.That(saved, Is.True, "应能创建用于 midday bridge 验证的临时 Primary 场景。");
        Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("Primary"), "midday bridge 验证应在名为 Primary 的场景上下文内执行。");
    }

    private void CleanupTemporaryPrimaryScene()
    {
        if (string.IsNullOrWhiteSpace(temporaryPrimaryScenePath))
        {
            return;
        }

        if (SceneManager.GetActiveScene().path == temporaryPrimaryScenePath)
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        string absoluteScenePath = Path.Combine(Directory.GetCurrentDirectory(), temporaryPrimaryScenePath);
        if (File.Exists(absoluteScenePath))
        {
            File.Delete(absoluteScenePath);
        }

        string metaPath = $"{absoluteScenePath}.meta";
        if (File.Exists(metaPath))
        {
            File.Delete(metaPath);
        }

        temporaryPrimaryScenePath = null;
    }

    private readonly struct RuntimeContext
    {
        public RuntimeContext(Component director, Component storyManager, Type storyPhaseType)
        {
            this.director = director;
            this.storyManager = storyManager;
            this.storyPhaseType = storyPhaseType;
        }

        public Component director { get; }
        public Component storyManager { get; }
        public Type storyPhaseType { get; }
    }
}
