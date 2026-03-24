using System.IO;
using NUnit.Framework;

[TestFixture]
public class SpringDay1DialogueProgressionTests
{
    private static readonly string ProjectRoot = Directory.GetCurrentDirectory();
    private static readonly string FirstDialoguePath = Path.Combine(ProjectRoot, "Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset");
    private static readonly string FollowupDialoguePath = Path.Combine(ProjectRoot, "Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue_Followup.asset");
    private static readonly string FollowupMetaPath = Path.Combine(ProjectRoot, "Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue_Followup.asset.meta");
    private static readonly string DebugMenuPath = Path.Combine(ProjectRoot, "Assets/Editor/Story/DialogueDebugMenu.cs");
    private static readonly string InteractablePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs");
    private static readonly string DialogueUiPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/UI/DialogueUI.cs");
    private static readonly string DialogueManagerPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Managers/DialogueManager.cs");
    private static readonly string DirectorPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs");
    private static readonly string WorkbenchInteractablePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs");
    private static readonly string WorkbenchOverlayPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs");
    private static readonly string WorkbenchSceneBinderPath = Path.Combine(ProjectRoot, "Assets/Editor/Story/SpringDay1WorkbenchSceneBinder.cs");
    private static readonly string WorkbenchRecipeAxePath = Path.Combine(ProjectRoot, "Assets/Resources/Story/SpringDay1Workbench/Recipe_9100_Axe_0.asset");
    private static readonly string WorkbenchRecipeHoePath = Path.Combine(ProjectRoot, "Assets/Resources/Story/SpringDay1Workbench/Recipe_9101_Hoe_0.asset");
    private static readonly string WorkbenchRecipePickaxePath = Path.Combine(ProjectRoot, "Assets/Resources/Story/SpringDay1Workbench/Recipe_9102_Pickaxe_0.asset");
    private static readonly string BedInteractablePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Interaction/SpringDay1BedInteractable.cs");
    private static readonly string BedSceneBinderPath = Path.Combine(ProjectRoot, "Assets/Editor/Story/SpringDay1BedSceneBinder.cs");
    private static readonly string PromptOverlayPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs");
    private static readonly string EnergySystemPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/EnergySystem.cs");
    private static readonly string HealthSystemPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/HealthSystem.cs");
    private static readonly string PlayerMovementPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/PlayerMovement.cs");

    [Test]
    public void FirstDialogueAsset_ContainsDecodeAndPhaseAdvanceConfig()
    {
        string assetText = File.ReadAllText(FirstDialoguePath);

        StringAssert.Contains("sequenceId: spring-day1-first", assetText, "首段对话 sequenceId 应稳定");
        StringAssert.Contains("markLanguageDecodedOnComplete: 1", assetText, "首段对话完成后应解码语言");
        StringAssert.Contains("advanceStoryPhaseOnComplete: 1", assetText, "首段对话完成后应推进剧情阶段");
        StringAssert.Contains("nextStoryPhase: 20", assetText, "首段对话完成后应推进到 EnterVillage");
        StringAssert.Contains("followupSequence: {fileID: 11400000, guid: c9dbc72325f747bbaf6250d6374ec586, type: 2}", assetText, "首段对话应指向 follow-up 资源");
    }

    [Test]
    public void FollowupDialogueAsset_ExistsAndContainsPlayableContent()
    {
        string assetText = File.ReadAllText(FollowupDialoguePath);
        string metaText = File.ReadAllText(FollowupMetaPath);

        StringAssert.Contains("sequenceId: spring-day1-first-followup", assetText, "后续对话 sequenceId 应稳定");
        StringAssert.Contains("speakerName: 村长", assetText, "后续对话应包含村长节点");
        StringAssert.Contains("speakerName: 旅人", assetText, "后续对话应包含旅人节点");
        StringAssert.Contains("markLanguageDecodedOnComplete: 0", assetText, "后续对话不应重复触发语言解码");
        StringAssert.Contains("advanceStoryPhaseOnComplete: 0", assetText, "后续对话不应重复推进剧情阶段");
        StringAssert.Contains("guid: c9dbc72325f747bbaf6250d6374ec586", metaText, "后续对话 meta GUID 应与首段引用一致");
    }

    [Test]
    public void DialogueDebugMenu_LogsStoryPhaseAndLanguageDecoded()
    {
        string scriptText = File.ReadAllText(DebugMenuPath);

        StringAssert.Contains("StoryPhase", scriptText, "调试日志应输出剧情阶段");
        StringAssert.Contains("LanguageDecoded", scriptText, "调试日志应输出语言解码状态");
        StringAssert.Contains("HP=", scriptText, "调试日志应输出当前生命值");
        StringAssert.Contains("EP=", scriptText, "调试日志应输出当前精力值");
    }

    [Test]
    public void NpcDialogueInteractable_ContainsMinimalNpcDialogueOccupation()
    {
        string scriptText = File.ReadAllText(InteractablePath);

        StringAssert.Contains("freezeRoamDuringDialogue", scriptText, "当前交互 NPC 的对话冻结开关应存在");
        StringAssert.Contains("facePlayerOnInteract", scriptText, "对话开始时面向玩家的开关应存在");
        StringAssert.Contains("EventBus.Subscribe<DialogueEndEvent>", scriptText, "应监听对话结束事件以恢复 NPC");
        StringAssert.Contains("autoRoamController.StopRoam()", scriptText, "对话开始时应冻结 NPC 漫游");
        StringAssert.Contains("autoRoamController.StartRoam()", scriptText, "对话结束后应恢复 NPC 漫游");
    }

    [Test]
    public void DialogueUi_ContainsBottomTestStatusText()
    {
        string uiText = File.ReadAllText(DialogueUiPath);
        string managerText = File.ReadAllText(DialogueManagerPath);
        string directorText = File.ReadAllText(DirectorPath);

        StringAssert.Contains("TestStatusText", uiText, "DialogueUI 应创建测试状态文本");
        StringAssert.Contains("测试对话:", uiText, "状态文本应显示当前测试对话编号");
        StringAssert.Contains("KeyCode.T", uiText, "DialogueUI 的键盘推进键应改为 T");
        StringAssert.Contains("FadeNonDialogueUi", uiText, "DialogueUI 应在对话期间隐藏其他 UI");
        StringAssert.Contains("otherUiFadeOutDuration", uiText, "DialogueUI 应提供其他 UI 的淡出配置");
        StringAssert.Contains("ShouldIgnoreDialogueEndEvent", uiText, "DialogueUI 应忽略旧对话的 End 事件，避免连续剧情时误恢复 UI");
        StringAssert.Contains("_nonDialogueUiSnapshots.Count > 0", uiText, "DialogueUI 应复用首次快照，避免连续剧情时把隐藏态误记成原始态");
        StringAssert.Contains("CurrentSequenceId", managerText, "DialogueManager 应暴露当前对话编号");
        StringAssert.Contains("GetCurrentTaskLabel()", directorText, "导演层应提供当前任务标签");
        StringAssert.Contains("GetCurrentProgressLabel()", directorText, "导演层应提供当前任务进度");
    }

    [Test]
    public void StatusSystems_ContainStoryFacingMethods()
    {
        string energyText = File.ReadAllText(EnergySystemPath);
        string healthText = File.ReadAllText(HealthSystemPath);
        string movementText = File.ReadAllText(PlayerMovementPath);

        StringAssert.Contains("SetVisible(bool visible)", energyText, "EnergySystem 应支持剧情控制显隐");
        StringAssert.Contains("SetEnergyState(int current, int max)", energyText, "EnergySystem 应支持剧情设定精力值");
        StringAssert.Contains("PlayRestoreAnimation", energyText, "EnergySystem 应支持晚餐回血动画");
        StringAssert.Contains("SetLowEnergyWarningVisual", energyText, "EnergySystem 应支持低精力红色警示表现");
        StringAssert.Contains("SetHealthState(int current, int max)", healthText, "HealthSystem 应支持剧情设定生命值");
        StringAssert.Contains("PlayRevealAndAnimateTo", healthText, "HealthSystem 应支持血条渐显并缓慢回血");
        StringAssert.Contains("CanvasGroup", healthText, "HealthSystem 应通过 CanvasGroup 做显隐过渡");
        StringAssert.Contains("TryFindHealthSlider()", healthText, "HealthSystem 应能自动绑定 HP UI");
        StringAssert.Contains("SetRuntimeSpeedMultiplier", movementText, "PlayerMovement 应支持剧情临时速度修正");
        StringAssert.Contains("ResetRuntimeSpeedMultiplier", movementText, "PlayerMovement 应支持恢复默认移动速度");
    }

    [Test]
    public void PromptOverlay_SuppressesItselfDuringDialogue()
    {
        string overlayText = File.ReadAllText(PromptOverlayPath);

        StringAssert.Contains("EventBus.Subscribe<DialogueStartEvent>", overlayText, "PromptOverlay 应监听对话开始");
        StringAssert.Contains("EventBus.Subscribe<DialogueEndEvent>", overlayText, "PromptOverlay 应监听对话结束");
        StringAssert.Contains("_suppressWhileDialogueActive", overlayText, "PromptOverlay 应在对话期间禁止再次显示");
        StringAssert.Contains("FadeCanvasGroup(0f, false)", overlayText, "PromptOverlay 应在对话开始时立即压低可见度");
        StringAssert.Contains("_queuedPromptText", overlayText, "PromptOverlay 应缓存对话前后的待显示提示");
        StringAssert.Contains("WaitAndRevealQueuedPrompt", overlayText, "PromptOverlay 应在对话框完全收起后再恢复提示");
        StringAssert.Contains("CurrentCanvasAlpha", overlayText, "PromptOverlay 应等 DialogueUI 视觉层彻底隐藏后再显示");
        StringAssert.Contains("postDialogueResumeDelay", overlayText, "PromptOverlay 应支持对话结束后的缓冲淡入延迟");
    }

    [Test]
    public void LiveValidationRunner_ProvidesRepeatableAcceptanceHooks()
    {
        string runnerText = File.ReadAllText(DirectorPath);
        string debugMenuText = File.ReadAllText(DebugMenuPath);

        StringAssert.Contains("BootstrapRuntime()", runnerText, "运行态验收器应支持一键补齐最小运行时依赖");
        StringAssert.Contains("BuildSnapshot(string label = null)", runnerText, "运行态验收器应能生成结构化快照");
        StringAssert.Contains("GetRecommendedNextAction()", runnerText, "运行态验收器应给出当前推荐下一步");
        StringAssert.Contains("TriggerRecommendedAction()", runnerText, "运行态验收器应能触发最小验收动作");
        StringAssert.Contains("TryTriggerNpcDialogue()", runnerText, "运行态验收器应支持触发 NPC001 对话");
        StringAssert.Contains("TryTriggerWorkbenchInteraction()", runnerText, "运行态验收器应支持触发工作台交互");
        StringAssert.Contains("TryTriggerRestInteraction()", runnerText, "运行态验收器应支持触发回住处休息");
        StringAssert.Contains("Bootstrap Spring Day1 Validation", debugMenuText, "调试菜单应暴露 Day1 验收入口初始化命令");
        StringAssert.Contains("Log Spring Day1 Validation Snapshot", debugMenuText, "调试菜单应暴露 Day1 验收快照命令");
        StringAssert.Contains("Step Spring Day1 Validation", debugMenuText, "调试菜单应暴露 Day1 验收单步推进命令");
    }

    [Test]
    public void WorkbenchInteraction_ContainsRuntimeBindingBridge()
    {
        string directorText = File.ReadAllText(DirectorPath);
        string interactableText = File.ReadAllText(WorkbenchInteractablePath);
        string overlayText = File.ReadAllText(WorkbenchOverlayPath);
        string binderText = File.ReadAllText(WorkbenchSceneBinderPath);
        string axeRecipeText = File.ReadAllText(WorkbenchRecipeAxePath);
        string hoeRecipeText = File.ReadAllText(WorkbenchRecipeHoePath);
        string pickaxeRecipeText = File.ReadAllText(WorkbenchRecipePickaxePath);

        StringAssert.Contains("PreferredWorkbenchObjectNames", directorText, "Day1 导演应保留工作台候选名列表");
        StringAssert.Contains("Anvil_0", directorText, "Day1 导演应优先识别 Anvil_0 作为当前工作台承载物");
        StringAssert.Contains("NotifyCraftingStationOpened", directorText, "Day1 导演应支持工作台直接触发剧情桥接");
        StringAssert.Contains("RefreshCraftingServiceSubscription", directorText, "Day1 导演应在动态创建 CraftingService 后补挂制作成功事件");
        StringAssert.Contains("CraftingStation.Workbench", interactableText, "工作台交互脚本应默认绑定到 Workbench 站点");
        StringAssert.Contains("panel.Open(station)", interactableText, "如果制作面板存在，工作台交互应尝试直接打开");
        StringAssert.Contains("NotifyCraftingStationOpened", interactableText, "工作台交互应把触发结果回传给 Day1 导演");
        StringAssert.Contains("KeyCode.E", interactableText, "工作台应支持测试用 E 键近距交互");
        StringAssert.Contains("Input.GetKeyDown(proximityInteractionKey)", interactableText, "工作台测试交互应由脚本自行检测 E 键");
        StringAssert.Contains("TryHandleWorkbenchTestInteraction", interactableText, "工作台在没有正式制作面板时应走 Day1 测试兜底");
        StringAssert.Contains("preferStoryWorkbenchOverlay", interactableText, "工作台交互应支持优先打开 Day1 专用制作浮层");
        StringAssert.Contains("ResolveWorkbenchOverlay", interactableText, "工作台交互应自动解析 Day1 工作台浮层");
        StringAssert.Contains("overlay.Toggle(transform, context?.PlayerTransform, craftingService, station, overlayAutoCloseDistance)", interactableText, "工作台交互应把玩家位置和自动关闭距离传给 Day1 浮层");
        StringAssert.Contains("interactionDistance = 0.5f", interactableText, "工作台交互距离应收口到 0.5 米");
        StringAssert.Contains("overlayAutoCloseDistance = 1.5f", interactableText, "工作台 UI 打开后离开 1.5 米应自动关闭");
        StringAssert.Contains("RecipeResourceFolder", overlayText, "Day1 工作台浮层应从正式 RecipeData 资源目录读取配方");
        StringAssert.Contains("Resources.LoadAll<RecipeData>", overlayText, "Day1 工作台浮层不应再运行时伪造 RecipeData");
        StringAssert.Contains("selectedMaterialsText", overlayText, "Day1 工作台浮层应提供右侧详情区");
        StringAssert.Contains("quantitySlider", overlayText, "Day1 工作台浮层应提供数量滑条");
        StringAssert.Contains("decreaseButton", overlayText, "Day1 工作台浮层应提供数量减号按钮");
        StringAssert.Contains("increaseButton", overlayText, "Day1 工作台浮层应提供数量加号按钮");
        StringAssert.Contains("GetClosestInteractionPoint", overlayText, "Day1 工作台浮层应根据玩家距离自动关闭");
        StringAssert.Contains("_displayBelow", overlayText, "Day1 工作台浮层应支持工作台上/下两个显示方向");
        StringAssert.Contains("ApplyDisplayDirection", overlayText, "Day1 工作台浮层应显式切换上/下两种悬浮方向");
        StringAssert.Contains("pointerRect", overlayText, "Day1 工作台浮层应带有指向工作台的悬浮指针");
        StringAssert.Contains("RecipeColumn", overlayText, "Day1 工作台浮层应保留左侧滚动配方列");
        StringAssert.Contains("DetailColumn", overlayText, "Day1 工作台浮层应保留右侧详情列");
        StringAssert.Contains("QuantityControls", overlayText, "Day1 工作台浮层应保留底部数量调节区");
        StringAssert.DoesNotContain("E 打开，超出 1.5 米自动收起", overlayText, "正式工作台 UI 不应再出现测试说明文案");
        StringAssert.DoesNotContain("工作台 UI 只响应鼠标左键", overlayText, "正式工作台 UI 不应在面板里显示调试提示语");
        StringAssert.Contains("recipeID: 9100", axeRecipeText, "Axe_0 配方资源应存在且使用固定 recipeID");
        StringAssert.Contains("recipeName: Axe_0", axeRecipeText, "Axe_0 配方资源应稳定");
        StringAssert.Contains("itemID: 3200", axeRecipeText, "Axe_0 配方资源应使用木材");
        StringAssert.Contains("recipeID: 9101", hoeRecipeText, "Hoe_0 配方资源应存在且使用固定 recipeID");
        StringAssert.Contains("recipeName: Hoe_0", hoeRecipeText, "Hoe_0 配方资源应稳定");
        StringAssert.Contains("recipeID: 9102", pickaxeRecipeText, "Pickaxe_0 配方资源应存在且使用固定 recipeID");
        StringAssert.Contains("itemID: 3201", pickaxeRecipeText, "Pickaxe_0 配方资源应包含石料需求");
        StringAssert.Contains("InitializeOnLoad", binderText, "编辑器恢复器应在 Unity 重新编译后自动生效");
        StringAssert.Contains("Anvil_0", binderText, "编辑器恢复器应优先识别 Anvil_0");
        StringAssert.Contains("Undo.AddComponent<CraftingStationInteractable>", binderText, "编辑器恢复器应能自动补挂工作台交互脚本");
    }

    [Test]
    public void DirectorAndBedBridge_ContainLateDayRuntimeHooks()
    {
        string directorText = File.ReadAllText(DirectorPath);
        string bedInteractableText = File.ReadAllText(BedInteractablePath);
        string bedBinderText = File.ReadAllText(BedSceneBinderPath);

        StringAssert.Contains("StoryTimePauseSource", directorText, "导演层应显式接管脚本阶段的时间暂停来源");
        StringAssert.Contains("SyncStoryTimePauseState", directorText, "导演层应在阶段切换时同步时间暂停状态");
        StringAssert.Contains("lowEnergyMoveSpeedMultiplier", directorText, "导演层应支持低精力减速");
        StringAssert.Contains("TryTriggerSleepFromBed", directorText, "导演层应提供床交互的 DayEnd 桥接入口");
        StringAssert.Contains("PreferredBedObjectNames", directorText, "导演层应保留床对象候选名");
        StringAssert.Contains("PreferredRestProxyObjectNames", directorText, "导演层应支持住处入口作为睡觉兜底承载物");
        StringAssert.Contains("House 1_2", directorText, "导演层应优先识别当前主场景里的住处入口对象");
        StringAssert.Contains("EnsureRestInteractableCollider", directorText, "导演层应在缺少碰撞器时自动补交互碰撞体");
        StringAssert.Contains("runtimeCollider.isTrigger = true", directorText, "住处入口兜底碰撞器应使用 Trigger，避免阻挡玩家");
        StringAssert.Contains("PlayRestoreAnimation", directorText, "导演层应在晚餐阶段触发精力恢复动画");
        StringAssert.Contains("回住处休息即可结束", directorText, "自由时段提示应兼容没有床对象的场景");
        StringAssert.Contains("_tillObjectiveCompleted", directorText, "农田教学应记录开垦完成状态，避免后续回退");
        StringAssert.Contains("_plantObjectiveCompleted", directorText, "农田教学应记录播种完成状态，避免后续回退");
        StringAssert.Contains("_waterObjectiveCompleted", directorText, "农田教学应记录浇水完成状态，避免后续回退");
        StringAssert.Contains("_woodObjectiveCompleted", directorText, "农田教学应记录木材收集完成状态，避免后续回退");
        StringAssert.Contains("requiredWoodCollectedCount", directorText, "砍树目标应改为木材收集数量目标");
        StringAssert.Contains("WoodItemId = 3200", directorText, "导演层应基于木材物品 ID 统计收集进度");
        StringAssert.Contains("GetCurrentWoodCount()", directorText, "导演层应从背包读取当前木材数量");
        StringAssert.Contains("TryHandleWorkbenchTestInteraction", directorText, "导演层应支持工作台测试交互兜底");
        StringAssert.Contains("StoryPhase.FreeTime", bedInteractableText, "床交互应只在自由时段开放");
        StringAssert.Contains("TimeManager.Instance.Sleep()", bedInteractableText, "床交互应能直接触发睡觉");
        StringAssert.Contains("InitializeOnLoad", bedBinderText, "床位编辑器恢复器应在 Unity 重新编译后自动生效");
        StringAssert.Contains("Undo.AddComponent<SpringDay1BedInteractable>", bedBinderText, "床位编辑器恢复器应能自动补挂床交互脚本");
    }
}
