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
    private static readonly string DirectorPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs");
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
    public void SpringDay1Director_ContainsStageThreeToSixRuntimeFlow()
    {
        string scriptText = File.ReadAllText(DirectorPath);

        StringAssert.Contains("StoryPhase.HealingAndHP", scriptText, "导演应包含 0.0.3 阶段");
        StringAssert.Contains("StoryPhase.WorkbenchFlashback", scriptText, "导演应包含 0.0.4 阶段");
        StringAssert.Contains("StoryPhase.FarmingTutorial", scriptText, "导演应包含 0.0.5 阶段");
        StringAssert.Contains("StoryPhase.DinnerConflict", scriptText, "导演应包含 0.0.6 晚餐冲突阶段");
        StringAssert.Contains("StoryPhase.ReturnAndReminder", scriptText, "导演应包含归途提醒阶段");
        StringAssert.Contains("StoryPhase.FreeTime", scriptText, "导演应包含自由时段阶段");
        StringAssert.Contains("StoryPhase.DayEnd", scriptText, "导演应包含睡觉结束阶段");
    }

    [Test]
    public void StatusSystems_ContainStoryFacingMethods()
    {
        string energyText = File.ReadAllText(EnergySystemPath);
        string healthText = File.ReadAllText(HealthSystemPath);

        StringAssert.Contains("SetVisible(bool visible)", energyText, "EnergySystem 应支持剧情控制显隐");
        StringAssert.Contains("SetEnergyState(int current, int max)", energyText, "EnergySystem 应支持剧情设定精力值");
        StringAssert.Contains("SetHealthState(int current, int max)", healthText, "HealthSystem 应支持剧情设定生命值");
        StringAssert.Contains("TryFindHealthSlider()", healthText, "HealthSystem 应能自动绑定 HP UI");
    }
}
