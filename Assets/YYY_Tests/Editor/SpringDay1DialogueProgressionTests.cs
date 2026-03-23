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
    private static readonly string WorkbenchSceneBinderPath = Path.Combine(ProjectRoot, "Assets/Editor/Story/SpringDay1WorkbenchSceneBinder.cs");
    private static readonly string PromptOverlayPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs");
    private static readonly string EnergySystemPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/EnergySystem.cs");
    private static readonly string HealthSystemPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/HealthSystem.cs");

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
        StringAssert.Contains("CurrentSequenceId", managerText, "DialogueManager 应暴露当前对话编号");
        StringAssert.Contains("GetCurrentTaskLabel()", directorText, "导演层应提供当前任务标签");
        StringAssert.Contains("GetCurrentProgressLabel()", directorText, "导演层应提供当前任务进度");
    }

    [Test]
    public void StatusSystems_ContainStoryFacingMethods()
    {
        string energyText = File.ReadAllText(EnergySystemPath);
        string healthText = File.ReadAllText(HealthSystemPath);

        StringAssert.Contains("SetVisible(bool visible)", energyText, "EnergySystem 应支持剧情控制显隐");
        StringAssert.Contains("SetEnergyState(int current, int max)", energyText, "EnergySystem 应支持剧情设定精力值");
        StringAssert.Contains("SetHealthState(int current, int max)", healthText, "HealthSystem 应支持剧情设定生命值");
        StringAssert.Contains("PlayRevealAndAnimateTo", healthText, "HealthSystem 应支持血条渐显并缓慢回血");
        StringAssert.Contains("CanvasGroup", healthText, "HealthSystem 应通过 CanvasGroup 做显隐过渡");
        StringAssert.Contains("TryFindHealthSlider()", healthText, "HealthSystem 应能自动绑定 HP UI");
    }

    [Test]
    public void PromptOverlay_SuppressesItselfDuringDialogue()
    {
        string overlayText = File.ReadAllText(PromptOverlayPath);

        StringAssert.Contains("EventBus.Subscribe<DialogueStartEvent>", overlayText, "PromptOverlay 应监听对话开始");
        StringAssert.Contains("EventBus.Subscribe<DialogueEndEvent>", overlayText, "PromptOverlay 应监听对话结束");
        StringAssert.Contains("_suppressWhileDialogueActive", overlayText, "PromptOverlay 应在对话期间禁止再次显示");
        StringAssert.Contains("Hide();", overlayText, "PromptOverlay 应在对话开始时立即隐藏");
    }

    [Test]
    public void WorkbenchInteraction_ContainsRuntimeBindingBridge()
    {
        string directorText = File.ReadAllText(DirectorPath);
        string interactableText = File.ReadAllText(WorkbenchInteractablePath);
        string binderText = File.ReadAllText(WorkbenchSceneBinderPath);

        StringAssert.Contains("PreferredWorkbenchObjectNames", directorText, "Day1 导演应保留工作台候选名列表");
        StringAssert.Contains("Anvil_0", directorText, "Day1 导演应优先识别 Anvil_0 作为当前工作台承载物");
        StringAssert.Contains("NotifyCraftingStationOpened", directorText, "Day1 导演应支持工作台直接触发剧情桥接");
        StringAssert.Contains("CraftingStation.Workbench", interactableText, "工作台交互脚本应默认绑定到 Workbench 站点");
        StringAssert.Contains("panel.Open(station)", interactableText, "如果制作面板存在，工作台交互应尝试直接打开");
        StringAssert.Contains("NotifyCraftingStationOpened", interactableText, "工作台交互应把触发结果回传给 Day1 导演");
        StringAssert.Contains("InitializeOnLoad", binderText, "编辑器恢复器应在 Unity 重新编译后自动生效");
        StringAssert.Contains("Anvil_0", binderText, "编辑器恢复器应优先识别 Anvil_0");
        StringAssert.Contains("Undo.AddComponent<CraftingStationInteractable>", binderText, "编辑器恢复器应能自动补挂工作台交互脚本");
    }
}
