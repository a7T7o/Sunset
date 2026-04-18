using System;
using System.IO;
using NUnit.Framework;

[TestFixture]
public class SpringDay1DialogueProgressionTests
{
    private static readonly string ProjectRoot = Directory.GetCurrentDirectory();
    private static readonly string FirstDialoguePath = Path.Combine(ProjectRoot, "Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset");
    private static readonly string FollowupDialoguePath = Path.Combine(ProjectRoot, "Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue_Followup.asset");
    private static readonly string FollowupMetaPath = Path.Combine(ProjectRoot, "Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue_Followup.asset.meta");
    private static readonly string VillageGateDialoguePath = Path.Combine(ProjectRoot, "Assets/111_Data/Story/Dialogue/SpringDay1_VillageGate.asset");
    private static readonly string VillageGateMetaPath = Path.Combine(ProjectRoot, "Assets/111_Data/Story/Dialogue/SpringDay1_VillageGate.asset.meta");
    private static readonly string HouseArrivalDialoguePath = Path.Combine(ProjectRoot, "Assets/111_Data/Story/Dialogue/SpringDay1_HouseArrival.asset");
    private static readonly string HouseArrivalMetaPath = Path.Combine(ProjectRoot, "Assets/111_Data/Story/Dialogue/SpringDay1_HouseArrival.asset.meta");
    private static readonly string DebugMenuPath = Path.Combine(ProjectRoot, "Assets/Editor/Story/DialogueDebugMenu.cs");
    private static readonly string DirectorStagingWindowPath = Path.Combine(ProjectRoot, "Assets/Editor/Story/SpringDay1DirectorStagingWindow.cs");
    private static readonly string InteractablePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs");
    private static readonly string InformalInteractablePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs");
    private static readonly string NpcDialogueContentProfilePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Data/NPCDialogueContentProfile.cs");
    private static readonly string PlayerNpcChatSessionServicePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs");
    private static readonly string DialogueUiPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/UI/DialogueUI.cs");
    private static readonly string DialogueManagerPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Managers/DialogueManager.cs");
    private static readonly string DirectorPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs");
    private static readonly string CrowdDirectorPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs");
    private static readonly string DirectorStagingRuntimePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs");
    private static readonly string WorldHintBubblePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs");
    private static readonly string NpcWorldHintBubblePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/UI/NpcWorldHintBubble.cs");
    private static readonly string WorkbenchInteractablePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs");
    private static readonly string WorkbenchOverlayPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs");
    private static readonly string WorkbenchSceneBinderPath = Path.Combine(ProjectRoot, "Assets/Editor/Story/SpringDay1WorkbenchSceneBinder.cs");
    private static readonly string WorkbenchRecipeAxePath = Path.Combine(ProjectRoot, "Assets/Resources/Story/SpringDay1Workbench/Recipe_9100_Axe_0.asset");
    private static readonly string WorkbenchRecipeHoePath = Path.Combine(ProjectRoot, "Assets/Resources/Story/SpringDay1Workbench/Recipe_9101_Hoe_0.asset");
    private static readonly string WorkbenchRecipePickaxePath = Path.Combine(ProjectRoot, "Assets/Resources/Story/SpringDay1Workbench/Recipe_9102_Pickaxe_0.asset");
    private static readonly string BedInteractablePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Interaction/SpringDay1BedInteractable.cs");
    private static readonly string ProximityServicePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs");
    private static readonly string BedSceneBinderPath = Path.Combine(ProjectRoot, "Assets/Editor/Story/SpringDay1BedSceneBinder.cs");
    private static readonly string SceneTransitionTriggerPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs");
    private static readonly string PromptOverlayPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs");
    private static readonly string UiLayerUtilityPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs");
    private static readonly string SpringUiEvidenceRuntimePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/UI/SpringUiEvidenceCaptureRuntime.cs");
    private static readonly string SpringUiEvidenceMenuPath = Path.Combine(ProjectRoot, "Assets/Editor/Story/SpringUiEvidenceMenu.cs");
    private static readonly string NearbyFeedbackPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/PlayerNpcNearbyFeedbackService.cs");
    private static readonly string ChatSessionServicePath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs");
    private static readonly string AutoRoamControllerPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs");
    private static readonly string TownAnchorContractPath = Path.Combine(ProjectRoot, "Assets/Resources/Story/SpringDay1/Directing/SpringDay1TownAnchorContract.json");
    private static readonly string EnergySystemPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/EnergySystem.cs");
    private static readonly string HealthSystemPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/HealthSystem.cs");
    private static readonly string PlayerMovementPath = Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/PlayerMovement.cs");

    [Test]
    public void FirstDialogueAsset_ContainsDecodeAndPhaseAdvanceConfig()
    {
        string assetText = File.ReadAllText(FirstDialoguePath);

        StringAssert.Contains("sequenceId: spring-day1-first", assetText, "首段对话 sequenceId 应稳定");
        StringAssert.Contains("markLanguageDecodedOnComplete: 1", assetText, "首段对话完成后应解码语言");
        StringAssert.Contains("advanceStoryPhaseOnComplete: 0", assetText, "矿洞口醒来段不应在第一拍就直接切进 EnterVillage");
        StringAssert.Contains("nextStoryPhase: 0", assetText, "矿洞口醒来段不应在第一拍就推进到 EnterVillage");
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
        StringAssert.Contains("骷髅", assetText, "后续对话应把矿洞口危险感补回当前主线");
        StringAssert.Contains("markLanguageDecodedOnComplete: 0", assetText, "后续对话不应重复触发语言解码");
        StringAssert.Contains("advanceStoryPhaseOnComplete: 1", assetText, "撤离收束后应正式推进到 EnterVillage");
        StringAssert.Contains("nextStoryPhase: 20", assetText, "撤离收束后应正式推进到 EnterVillage");
        StringAssert.Contains("followupSequence: {fileID: 11400000, guid: 848b3d9bebdc4efebbafa3f500912bc4, type: 2}", assetText, "撤离段后应自动续播进村围观段");
        StringAssert.Contains("guid: c9dbc72325f747bbaf6250d6374ec586", metaText, "后续对话 meta GUID 应与首段引用一致");
    }

    [Test]
    public void EnterVillageDialogueAssets_ExistAndGateHouseArrivalToPrimaryRuntime()
    {
        string villageGateText = File.ReadAllText(VillageGateDialoguePath);
        string villageGateMetaText = File.ReadAllText(VillageGateMetaPath);
        string houseArrivalText = File.ReadAllText(HouseArrivalDialoguePath);
        string houseArrivalMetaText = File.ReadAllText(HouseArrivalMetaPath);

        StringAssert.Contains("sequenceId: spring-day1-village-gate", villageGateText, "进村围观段 sequenceId 应稳定");
        StringAssert.Contains("speakerName: 村民", villageGateText, "进村围观段应出现村民视线");
        StringAssert.Contains("speakerName: 小孩", villageGateText, "进村围观段应出现小孩视线");
        StringAssert.Contains("followupSequence: {fileID: 0}", villageGateText, "Town 围观段不应再直接自动续播 Primary 承接；承接应改由 director 在切场后接管。");
        StringAssert.Contains("guid: 848b3d9bebdc4efebbafa3f500912bc4", villageGateMetaText, "进村围观段 meta GUID 应稳定");

        StringAssert.Contains("sequenceId: spring-day1-house-arrival", houseArrivalText, "Primary 承接段 sequenceId 应稳定");
        StringAssert.Contains("艾拉", houseArrivalText, "Primary 承接段应交代艾拉会接手疗伤");
        StringAssert.Contains("先在这边站稳", houseArrivalText, "Primary 承接段应先把玩家重新站稳");
        StringAssert.Contains("followupSequence: {fileID: 0}", houseArrivalText, "Primary 承接段应成为进入疗伤前的最后一拍");
        StringAssert.Contains("guid: 8ecb8bb90c564b2289fec30b42625533", houseArrivalMetaText, "Primary 承接段 meta GUID 应稳定");
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
        StringAssert.Contains("enableProximityKeyInteraction", scriptText, "NPC 应支持近距 E 键交互");
        StringAssert.Contains("SpringDay1ProximityInteractionService.ReportCandidate(", scriptText, "NPC formal 对话的 E 键触发应接入 Day1 统一近身仲裁服务。");
        StringAssert.Contains("SpringDay1WorldHintBubble", scriptText, "NPC 应复用统一的 E 提示气泡");
        StringAssert.Contains("HideIfExists(transform)", scriptText, "NPC 退场时回收提示气泡不应反向创建新的运行时 UI");
        StringAssert.Contains("autoRoamController.StopRoam()", scriptText, "对话开始时应冻结 NPC 漫游");
        StringAssert.Contains("autoRoamController.StartRoam()", scriptText, "对话结束后应恢复 NPC 漫游");
        StringAssert.Contains("ShouldIgnoreDialogueEndEvent()", scriptText, "连续对话时 NPC 不应因旧 End 事件提前恢复漫游");
        StringAssert.Contains("HasConsumedFormalDialogue", scriptText, "NPC formal 交互应显式判断该段剧情是否已经被消费。");
        StringAssert.Contains("public enum NPCFormalDialogueState", scriptText, "NPC formal 交互应公开最小 formal 状态枚举，供 day1 / probe 直接读真实状态。");
        StringAssert.Contains("GetFormalDialogueStateForCurrentStory()", scriptText, "NPC formal 交互应公开当前 formal 状态只读口。");
        StringAssert.Contains("HasConsumedFormalDialogueForCurrentStory()", scriptText, "NPC formal 交互应公开“formal 是否已消费”的只读判断。");
        StringAssert.Contains("WillYieldToInformalResident()", scriptText, "NPC formal 交互应公开“当前是否已让位给 informal/resident”的只读判断。");
        StringAssert.DoesNotContain("return resolvedFollowup;", scriptText, "formal 对话消费后不应再把 followup 当成可重复点开的正式剧情。");
        StringAssert.Contains("return null;", scriptText, "formal 对话消费后应彻底让出正式交互入口，回落给闲聊 / resident 内容。");
    }

    [Test]
    public void InformalChat_ExplicitlyYieldsToDialogueCandidateOnSameNpc()
    {
        string informalText = File.ReadAllText(Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs"));
        string proximityServiceText = File.ReadAllText(ProximityServicePath);
        string sessionServiceText = File.ReadAllText(ChatSessionServicePath);

        StringAssert.Contains("dialogueInteractable", informalText, "闲聊交互应显式识别同体剧情对话组件，避免未来手挂双组件时又回到谁先抢到谁算。");
        StringAssert.Contains("NpcInteractionPriorityPolicy.ShouldSuppressInformalInteractionForCurrentStory", informalText, "闲聊交互应把“同 NPC 上剧情对话优先”写成稳定规则，而不是只靠口头约定。");
        StringAssert.Contains("NpcInteractionPriorityPolicy.ShouldSuppressInformalInteractionForCurrentStory(", informalText, "闲聊交互应在同体 formal 仍可接管时主动让位，而不是再靠旧的 CanInteract 直连判断。");
        StringAssert.Contains("ShouldUseResidentPromptTone()", informalText, "formal 已让位给 resident 后，闲聊交互应显式暴露回落语义给玩家面提示壳。");
        StringAssert.Contains("日常交流", sessionServiceText, "formal 已消费后，玩家面 idle 提示不应继续一律显示“闲聊”。");
        StringAssert.Contains("按 E 聊聊近况", sessionServiceText, "formal 已消费后，玩家面 idle detail 应切到 resident 近况语义。");
        StringAssert.Contains("candidate.CanTriggerNow != current.CanTriggerNow", proximityServiceText, "统一仲裁服务应优先保留当前真正可触发的目标，避免更近 teaser 挡住真正能按的交互。");
    }

    [Test]
    public void InformalChat_UsesPhaseAwareResidentFallbackAfterFormalWasConsumed()
    {
        string informalText = File.ReadAllText(InformalInteractablePath);
        string contentProfileText = File.ReadAllText(NpcDialogueContentProfilePath);
        string sessionServiceText = File.ReadAllText(PlayerNpcChatSessionServicePath);

        StringAssert.Contains("phaseInformalChatSets", contentProfileText, "NPC 对话内容资产应序列化按剧情阶段分流的 resident fallback 集合。");
        StringAssert.Contains("GetInformalConversationBundles(", contentProfileText, "NPC 对话内容资产应提供 phase-aware 的闲聊 bundle 解析口。");
        StringAssert.Contains("TryGetPhaseInformalChatSet", contentProfileText, "NPC 对话内容资产应能按 StoryPhase 读取 phase fallback set。");
        StringAssert.Contains("ResolveConversationBundles(relationshipStage, ResolveCurrentStoryPhase())", informalText, "NPC 闲聊交互应显式把当前 StoryPhase 传给内容解析层。");
        StringAssert.Contains("ResolveCurrentStoryPhase()", sessionServiceText, "玩家与 NPC 的闲聊会话在起聊时应读取当前 StoryPhase。");
        StringAssert.Contains("ResolveConversationBundles(relationshipStage, storyPhase)", sessionServiceText, "玩家与 NPC 的闲聊会话应在 formal 让位后按当前 phase 选择 resident fallback，而不是只掉回默认 bundle。");
    }

    [Test]
    public void PlayerNpcNearbyFeedback_UsesPhaseAwareResidentNearbyLines()
    {
        string contentProfileText = File.ReadAllText(NpcDialogueContentProfilePath);
        string nearbyServiceText = File.ReadAllText(NearbyFeedbackPath);
        string bootstrapText = File.ReadAllText(Path.Combine(ProjectRoot, "Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs"));

        StringAssert.Contains("phaseNearbyLines", contentProfileText, "NPC 对话内容资产应序列化按剧情阶段分流的 resident nearby 集合。");
        StringAssert.Contains("TryGetPhaseNearbySet", contentProfileText, "NPC 对话内容资产应能按 StoryPhase 读取 phase nearby set。");
        StringAssert.Contains("GetPlayerNearbyLines(relationshipStage, storyPhase)", nearbyServiceText, "玩家靠近 NPC 的轻反馈应把当前 StoryPhase 传给 nearby line 解析层。");
        StringAssert.Contains("ResolveCurrentStoryPhase()", nearbyServiceText, "玩家靠近 NPC 的轻反馈应在探测时读取当前 StoryPhase。");
        StringAssert.Contains("BuildPhaseNearbyPayloads()", bootstrapText, "crowd bootstrap 源头应直接生成 phase-aware resident nearby lines。");
    }

    [Test]
    public void NpcAutoRoamSelfTalk_UsesPhaseAwareResidentLines()
    {
        string contentProfileText = File.ReadAllText(NpcDialogueContentProfilePath);
        string roamProfileText = File.ReadAllText(Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Data/NPCRoamProfile.cs"));
        string autoRoamText = File.ReadAllText(AutoRoamControllerPath);
        string bootstrapText = File.ReadAllText(Path.Combine(ProjectRoot, "Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs"));

        StringAssert.Contains("phaseSelfTalkLines", contentProfileText, "NPC 对话内容资产应序列化按剧情阶段分流的 resident selfTalk 集合。");
        StringAssert.Contains("TryGetPhaseSelfTalkSet", contentProfileText, "NPC 对话内容资产应能按 StoryPhase 读取 phase selfTalk set。");
        StringAssert.Contains("GetSelfTalkLines(StoryPhase storyPhase)", roamProfileText, "漫游配置应提供 phase-aware selfTalk 解析口。");
        StringAssert.Contains("ResolveCurrentStoryPhase()", autoRoamText, "NPC 长停自语应在运行时读取当前 StoryPhase。");
        StringAssert.Contains("roamProfile.GetSelfTalkLines(currentPhase)", autoRoamText, "NPC 长停自语应优先吃当前 phase 的 resident selfTalk lines。");
        StringAssert.Contains("TryShowResidentSelfTalk", autoRoamText, "NPC 长停自语应通过单独 helper 统一走 resident selfTalk 入口。");
        StringAssert.Contains("BuildPhaseSelfTalkPayloads()", bootstrapText, "crowd bootstrap 源头应直接生成 phase-aware resident selfTalk lines。");
    }

    [Test]
    public void CrowdBootstrap_UsesPhaseAwareWalkAwayReactionsAfterFormalConsumed()
    {
        string bootstrapText = File.ReadAllText(Path.Combine(ProjectRoot, "Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs"));

        StringAssert.Contains("BuildPhaseWalkAwayReactionPayload", bootstrapText, "crowd bootstrap 源头应能给 phase resident fallback 补 phase-aware walkAwayReaction。");
        StringAssert.Contains("walkAwayReaction ??= BuildPhaseWalkAwayReactionPayload", bootstrapText, "phase resident fallback 应优先吃 phase-specific walkAwayReaction，而不是一律退回默认模板。");
        StringAssert.Contains("101-enter-village-walkaway", bootstrapText, "phase-aware walkAwayReaction 应至少给关键 resident beat 落下真实 cue。");
        StringAssert.Contains("102-free-time-walkaway", bootstrapText, "当前保留的夜段 resident fallback 也应按 phase 变味。");
    }

    [Test]
    public void Director_UsesCompletedDialogueSequencesAsOneShotRecoveryGuards()
    {
        string directorText = File.ReadAllText(DirectorPath);

        StringAssert.Contains("ShouldQueueDialogueSequence", directorText, "Day1 导演应把正式对白排队改成统一 one-shot 守门，而不是只靠局部布尔位。");
        StringAssert.Contains("TryRecoverConsumedSequenceProgression", directorText, "Day1 导演应在主循环里主动把已消费的正式对白恢复成后续剧情阶段，避免局部状态漂移时重播。");
        StringAssert.Contains("HasCompletedDialogueSequence(DinnerSequenceId)", directorText, "晚餐冲突已消费后，导演层不应再次把 Dinner formal 当成新对白重播。");
        StringAssert.Contains("HasCompletedDialogueSequence(ReminderSequenceId)", directorText, "归途提醒已消费后，导演层不应再次把 Reminder formal 当成新对白重播。");
        StringAssert.Contains("HasCompletedDialogueSequence(FreeTimeIntroSequenceId)", directorText, "自由时段见闻已消费后，导演层应直接回到自由活动，不应再次正式接管。");
        StringAssert.Contains("CompleteFreeTimeIntro();", directorText, "自由时段 intro 已消费后，导演层应统一通过单一收口方法回落到正式已完成态。");
        StringAssert.Contains("HasCompletedDialogueSequence(WorkbenchSequenceId)", directorText, "工作台回忆已消费后，再次打开工作台不应重播 formal，而应直接回到 FarmingTutorial。");
        StringAssert.Contains("GetCurrentResidentBeatConsumptionSummary()", directorText, "Day1 导演应公开当前 beat 的 resident consumption 摘要，便于最终整合与 live 验证。");
        StringAssert.Contains("GetOneShotProgressSummary()", directorText, "Day1 导演应公开正式剧情 one-shot 消费摘要，便于 live 验证已消费桥段不会重播。");
        StringAssert.Contains("TryBeginTownHouseLead()", directorText, "Town 围观收束后，导演不应只留一句提示，而应进入真实的村长带路态。");
        StringAssert.Contains("GetTownHouseLeadSummary()", directorText, "Day1 导演应公开当前进村带路态摘要，便于 live 验证 chief/target/距离是否已经真接上。");
        StringAssert.Contains("SceneTransitionTrigger2D", directorText, "进村后半段不应再靠二次聊天推进，而应真消费 Town 里的切场触发物。");
    }

    [Test]
    public void CrowdDirector_ConsumesBeatSnapshotAndPrefersSceneResidentRoots()
    {
        string crowdDirectorText = File.ReadAllText(CrowdDirectorPath);
        string stagingRuntimeText = File.ReadAllText(DirectorStagingRuntimePath);

        StringAssert.Contains("BuildBeatConsumptionSnapshot(currentBeatKey)", crowdDirectorText, "CrowdDirector 应在 runtime sync 时真正吃 beat consumption snapshot，而不是只让 helper 挂在 manifest 里闲置。");
        StringAssert.Contains("BuildBeatConsumptionSummary", crowdDirectorText, "CrowdDirector 的 runtime summary 应显式报出当前 beat consumption 的四类分布，便于 day1 直接判断导演消费已经吃到哪层。");
        StringAssert.Contains("ResolveBeatConsumptionRole", crowdDirectorText, "CrowdDirector 应显式把 beat consumption role 映射到 resident parent 选择。");
        StringAssert.Contains("TryResolveBeatPresence", crowdDirectorText, "CrowdDirector 应显式把 beat presence 读回 runtime active 判定。");
        StringAssert.Contains("EnsureSceneLookupCache(scene)", crowdDirectorText, "CrowdDirector 应优先缓存并读取 scene 已有对象，不应永远自造运行时空根。");
        StringAssert.Contains("FindSceneRoot(string rootName, Scene scene)", crowdDirectorText, "CrowdDirector 应保留 scene root 查询入口，后续 root/anchor 清扫不能退回盲造。");
        StringAssert.Contains("TryBindSceneResident", crowdDirectorText, "CrowdDirector 应优先绑定 scene 里已有的 resident，而不是继续默认 runtime 生人。");
        StringAssert.DoesNotContain("Instantiate(entry.prefab", crowdDirectorText, "CrowdDirector 不应再默认 runtime Instantiate resident prefab。");
        StringAssert.DoesNotContain("_RuntimeHomeAnchor", crowdDirectorText, "CrowdDirector 不应再默认 runtime 创建 HomeAnchor 替身。");
        StringAssert.Contains("TownSceneName", crowdDirectorText, "Town 现在已经进入 day1 crowd runtime 支持面，不应继续只绑在 Primary 代理场景。");
        StringAssert.Contains("IsSupportedRuntimeScene", crowdDirectorText, "CrowdDirector 应显式识别 Primary + Town 两个 runtime scene。");
        StringAssert.Contains("BackstagePressure", crowdDirectorText, "CrowdDirector 应把 backstagePressure 也纳入 resident backstage parent 判断。");
        StringAssert.Contains("builder.Append(\"|beat=\")", crowdDirectorText, "CrowdDirector 的 runtime summary 应显式带出当前 beat。");
        StringAssert.Contains("builder.Append(\"@group=\")", crowdDirectorText, "CrowdDirector 的 runtime summary 应显式带出 resident parent/group，便于追当前是不是常驻、takeover-ready 还是 backstage。");
        StringAssert.Contains("builder.Append(\"@owned=\")", crowdDirectorText, "CrowdDirector 的 runtime summary 应显式带出当前 resident 是 scene-owned 还是 runtime-owned。");
        StringAssert.Contains("builder.Append(\"@home=\")", crowdDirectorText, "CrowdDirector 的 runtime summary 应显式带出当前 home anchor 是 scene 现成、runtime 替身还是缺失。");
        StringAssert.Contains("builder.Append(\"@cue=\")", crowdDirectorText, "CrowdDirector 的 runtime summary 应显式带出当前 staging cue。");
        StringAssert.Contains("builder.Append(\"@role=\")", crowdDirectorText, "CrowdDirector 的 runtime summary 应显式带出当前 beat consumption role。");
        StringAssert.Contains("builder.Append(\"@presence=\")", crowdDirectorText, "CrowdDirector 的 runtime summary 应显式带出当前 beat presenceLevel。");
        StringAssert.Contains("builder.Append(\"|missing=\")", crowdDirectorText, "CrowdDirector 的 runtime summary 应显式带出当前还没绑定上的 resident，便于把 scene-side blocker 钉死。");
        StringAssert.Contains("public static bool IsBeatCueSettled", crowdDirectorText, "CrowdDirector 应公开最小 settled 判断，供 director 在正式对白前等 crowd 站定。");
        StringAssert.DoesNotContain("state.HomeAnchor.transform.position =", crowdDirectorText, "CrowdDirector 不应再把 scene HomeAnchor 跟着 resident 一起拖走。");
        StringAssert.Contains("if (!roamController.IsRoaming)", crowdDirectorText, "CrowdDirector 把 resident 从导演接管态退回 baseline 后，应显式判断是否已停漫游。");
        StringAssert.Contains("roamController.StartRoam();", crowdDirectorText, "CrowdDirector 把 resident 退回 baseline 后，应主动恢复漫游，不然原生 resident 会站桩。");
        StringAssert.DoesNotContain("_homeAnchor.position =", stagingRuntimeText, "导演回放不应再在 cue 过程中改写 scene HomeAnchor。");
        StringAssert.Contains("TryResolveCueReferenceStartPosition", stagingRuntimeText, "导演回放应支持“当前 resident 起跑 + 语义锚做目标参考”的 cue 解析。");
        StringAssert.Contains("HasSettledCurrentCue()", stagingRuntimeText, "导演回放应公开当前 cue 是否已站定，供正式对白等待走位收束。");
    }

    [Test]
    public void DialogueDebugMenu_ContainsOpeningResetAndSkipToggleSafetyGuards()
    {
        string debugMenuText = File.ReadAllText(DebugMenuPath);
        string directorText = File.ReadAllText(DirectorPath);

        StringAssert.Contains("Toggle Skip To Workbench 0.0.5", debugMenuText, "调试菜单应显式暴露工作台直跳开关，避免残留 EditorPrefs 只能靠猜。");
        StringAssert.Contains("Reset Spring Day1 To Opening", debugMenuText, "调试菜单应显式提供一键回开场入口。");
        StringAssert.Contains("StoryProgressPersistenceService.ResetToOpeningRuntimeState();", debugMenuText, "调试菜单的一键回开场应直接调用持久态重置入口。");
        StringAssert.Contains("EditorPrefs.SetBool(SpringDay1Director.DebugWorkbenchSkipEditorPrefKey, false);", debugMenuText, "一键回开场时应顺手清掉工作台直跳 EditorPrefs，避免再次 Play 又被带偏。");
        StringAssert.Contains("ConsumeDebugWorkbenchSkipEditorPref", directorText, "SpringDay1Director 应把编辑器直跳开关当一次性消费，而不是长期污染正常开场。");
        StringAssert.Contains("EditorPrefs.SetBool(DebugWorkbenchSkipEditorPrefKey, false);", directorText, "SpringDay1Director 消费完编辑器直跳后应主动清掉 EditorPrefs。");
    }

    [Test]
    public void DirectorStagingWindow_PersistsDraftAcrossDomainReloadUntilJsonSave()
    {
        string stagingWindowText = File.ReadAllText(DirectorStagingWindowPath);

        StringAssert.Contains("DraftEditorPrefKey", stagingWindowText, "导演摆位窗口应有单独的草稿持久化 key，避免重编译后未保存内容直接丢失。");
        StringAssert.Contains("LoadBookOrDraft()", stagingWindowText, "导演摆位窗口启用时应先尝试恢复草稿，而不是每次只重载正式 JSON。");
        StringAssert.Contains("TryRestoreDraftState()", stagingWindowText, "导演摆位窗口应显式尝试恢复上次未保存的草稿。");
        StringAssert.Contains("PersistDraftState()", stagingWindowText, "导演摆位窗口字段变动后应自动把当前草稿写入持久化层。");
        StringAssert.Contains("PersistDraftStateIfDirty()", stagingWindowText, "导演摆位窗口关闭时应只回写未保存草稿，不能在保存 JSON 后又把旧草稿写回来。");
        StringAssert.Contains("_isDraftDirty = false;", stagingWindowText, "导演摆位窗口在保存 JSON 或重载正式稿后应清掉草稿脏态。");
        StringAssert.Contains("_isDraftDirty = true;", stagingWindowText, "导演摆位窗口恢复旧草稿或继续编辑后应保持脏态，防止编译后未保存内容丢失。");
        StringAssert.Contains("DeleteDraftState();", stagingWindowText, "导演摆位窗口在正式保存 JSON 或手动重载后应清掉旧草稿，避免反复覆盖。");
        StringAssert.Contains("已恢复草稿", stagingWindowText, "导演摆位窗口恢复未保存草稿后应给出最小提示，不应让用户以为自己白填了。");
        StringAssert.Contains("EditorPrefs.SetString(DraftEditorPrefKey", stagingWindowText, "导演摆位窗口的草稿应真正写进 EditorPrefs，而不是只留在内存。");
        StringAssert.Contains("JsonUtility.ToJson(draft)", stagingWindowText, "导演摆位窗口应序列化整份草稿，而不是只记当前下拉索引。");
        StringAssert.Contains("TryResolveDraftCueForPreview", stagingWindowText, "导演摆位窗口的整批预演应优先吃当前草稿，而不是只读已落盘 JSON。");
        StringAssert.DoesNotContain("ResolveDraftPreviewFallbackBeatKey", stagingWindowText, "导演摆位窗口不应再把 dinner/reminder 缺 cue 偷偷回退到 opening cue。");
    }

    [Test]
    public void CrowdDirector_ShouldFenceDeprecatedSemanticAnchorsOutOfRuntimeSpawn()
    {
        string crowdDirectorText = File.ReadAllText(CrowdDirectorPath);

        StringAssert.Contains("IsDeprecatedRuntimeSemanticAnchor", crowdDirectorText, "CrowdDirector 应显式把旧 semantic anchor 关在 runtime spawn 真值之外。");
        StringAssert.Contains("EnterVillageCrowdRoot", crowdDirectorText, "opening 旧 crowd root 不应继续当 ResolveSpawnPoint 真值。");
        StringAssert.Contains("DinnerBackgroundRoot", crowdDirectorText, "dinner 旧 background root 不应继续当 ResolveSpawnPoint 真值。");
        StringAssert.Contains("NightWitness_01", crowdDirectorText, "夜间旧见闻锚点必须进入 deprecated fence。");
        StringAssert.Contains("DailyStand_", crowdDirectorText, "次日预示锚点必须进入 deprecated fence。");
    }

    [Test]
    public void DialogueUi_ContainsBottomTestStatusText()
    {
        string uiText = File.ReadAllText(DialogueUiPath);
        string managerText = File.ReadAllText(DialogueManagerPath);
        string directorText = File.ReadAllText(DirectorPath);

        StringAssert.Contains("TestStatusText", uiText, "DialogueUI 应创建测试状态文本");
        StringAssert.Contains("测试对话:", uiText, "状态文本应显示当前测试对话编号");
        StringAssert.Contains("KeyCode.Space", uiText, "DialogueUI 的键盘推进键应改为空格");
        StringAssert.Contains("NormalizeAdvanceInputSettings()", uiText, "DialogueUI 应在运行时把旧场景里残留的 T 键序列化值强制回正为空格。");
        StringAssert.Contains("advanceKey = KeyCode.Space;", uiText, "DialogueUI 应主动把剧情推进键回正为空格。");
        StringAssert.Contains("enablePointerClickAdvance = false;", uiText, "DialogueUI 不应再让旧场景序列化值把鼠标点击推进偷偷带回来。");
        StringAssert.Contains("摁空格键继续", uiText, "DialogueUI 的继续提示应直接回到纯文案，不再额外造按钮样式");
        StringAssert.Contains("Keyboard.current.spaceKey.wasPressedThisFrame", uiText, "DialogueUI 的 continue submit 链应只认空格，不应再让回车推进剧情。");
        StringAssert.DoesNotContain("Keyboard.current.enterKey.wasPressedThisFrame", uiText, "DialogueUI 的 continue submit 链不应再接受主回车。");
        StringAssert.DoesNotContain("Keyboard.current.numpadEnterKey.wasPressedThisFrame", uiText, "DialogueUI 的 continue submit 链不应再接受小键盘回车。");
        StringAssert.DoesNotContain("Input.GetKeyDown(KeyCode.Return)", uiText, "DialogueUI 的旧输入系统 submit 链不应再接受主回车。");
        StringAssert.DoesNotContain("Input.GetKeyDown(KeyCode.KeypadEnter)", uiText, "DialogueUI 的旧输入系统 submit 链不应再接受小键盘回车。");
        StringAssert.DoesNotContain("ContinueKeyChipDisplayText", uiText, "DialogueUI 不应再单独生成空格键帽节点。");
        StringAssert.DoesNotContain("ContinueHoverRelay", uiText, "DialogueUI 不应再为了继续提示额外扩写 hover 中继。");
        StringAssert.Contains("FadeNonDialogueUi", uiText, "DialogueUI 应在对话期间隐藏其他 UI");
        StringAssert.Contains("otherUiFadeOutDuration", uiText, "DialogueUI 应提供其他 UI 的淡出配置");
        StringAssert.Contains("ShouldIgnoreDialogueEndEvent", uiText, "DialogueUI 应忽略旧对话的 End 事件，避免连续剧情时误恢复 UI");
        StringAssert.Contains("ApplyFontStyle(innerMonologueFontKey, speakerNameFontKey);", uiText, "DialogueUI 的正式对话主文本应统一回到独白那套像素字体。");
        StringAssert.Contains("_nonDialogueUiSnapshots.Count > 0", uiText, "DialogueUI 应复用首次快照，避免连续剧情时把隐藏态误记成原始态");
        StringAssert.Contains("CurrentSequenceId", managerText, "DialogueManager 应暴露当前对话编号");
        StringAssert.Contains("GetCurrentTaskLabel()", directorText, "导演层应提供当前任务标签");
        StringAssert.Contains("GetCurrentProgressLabel()", directorText, "导演层应提供当前任务进度");
        StringAssert.Contains("0.0.2 进村/安置", directorText, "导演层任务标签应把 EnterVillage 区分为进村与安置阶段");
        StringAssert.Contains("debugSkipDirectToWorkbenchPhase05", directorText, "SpringDay1Director 应提供直跳到 0.0.5 工作台可用态的调试开关。");
        StringAssert.Contains("TryApplyDebugWorkbenchSkip()", directorText, "SpringDay1Director 应在运行时尝试应用工作台直跳调试开关。");
        StringAssert.Contains("调试直跳已开启：直接进入 0.0.5，返回工作台完成一次基础制作。", directorText, "工作台直跳调试开关触发后应给出明确提示。");
    }

    [Test]
    public void PromptOverlay_GeneratedPageHeight_ShouldUseVisibleSectionStackInsteadOfWholeContentPreferredHeight()
    {
        string promptText = File.ReadAllText(PromptOverlayPath);

        StringAssert.Contains("ApplyGeneratedPageSectionVisibility", promptText, "PromptOverlay 应先把空的 subtitle/focus/footer 区块关掉，再参与高度计算。");
        StringAssert.Contains("CalculateVisibleLayoutStackHeight", promptText, "PromptOverlay 生成页高度应按真实可见区块逐段求和。");
        StringAssert.Contains("page.root.gameObject.activeSelf", promptText, "PromptOverlay 外层深色壳体高度不应继续吃到隐藏背页的旧高度。");
        StringAssert.DoesNotContain("LayoutUtility.GetPreferredHeight(page.contentRoot) + 28f", promptText, "PromptOverlay 不应再直接拿整块 contentRoot 的 preferredHeight 当正式高度。");
    }

    [Test]
    public void PromptOverlay_LegacyLayout_ShouldHideEmptySubtitleAndDividerInsteadOfKeepingBlankHeight()
    {
        string promptText = File.ReadAllText(PromptOverlayPath);

        StringAssert.Contains("page.subtitleRoot.gameObject.SetActive(hasSubtitle)", promptText, "legacy prompt 页在 subtitle 为空时应直接隐藏 subtitle 壳体，不应继续占高度。");
        StringAssert.Contains("page.headerDividerRoot.gameObject.SetActive(hasDivider)", promptText, "legacy prompt 页在正文为空时应一起隐藏 divider，避免只剩黑底空洞。");
        StringAssert.Contains("bool hasDivider = page.headerDividerRoot != null && (hasSubtitle || hasTasks || hasFocus || hasFooter);", promptText, "legacy prompt 页的 divider 只应在真实正文存在时出现。");
        StringAssert.Contains("GetLegacyPageShellInset(page)", promptText, "legacy prompt 页高度应把 page 自己的壳体边距算回去，FooterText 不应再掉到 page 外。");
        StringAssert.Contains("NormalizeLegacyPageRoot(page);", promptText, "legacy prompt 页在复用 prefab 时应先把 page 根从 stretch 壳体回正成真正的固定页面，否则 page 会被父壳高度再叠一层。");
        StringAssert.Contains("ResolveLegacySectionRoot", promptText, "legacy prompt prefab 里 Subtitle/Focus/Footer 直接挂在 page 根上时，布局根节点应回到对应文本自身，不能把整张 page 当成 footer 壳。");
        StringAssert.Contains("ResolveLegacySectionRoot(pageRoot, \"FooterText\", page.footerText)", promptText, "legacy prompt 页的 FooterText 绑定应直接锚到 FooterText，而不是错误回退到 page 根节点。");
        StringAssert.Contains("Mathf.Abs(anchorMin.y - anchorMax.y)", promptText, "legacy prompt 页的 top 对齐应区分纵向是否拉伸，不能再把 FooterText 这类底锚点节点写到 page 外。");
        StringAssert.Contains("GetLegacyMinimumPageHeight(page)", promptText, "legacy prompt 页应保留 page 自己的基础页高，不能再缩成一团。");
        StringAssert.Contains("ResolveLegacyShellMinimumHeight", promptText, "legacy prompt 外层黑壳高度应以 page 的基础长度为底，不应重新塌回极小高度。");
        StringAssert.Contains("LegacyTaskToBottomBandMinGap", promptText, "legacy prompt 页的 TaskList 与底部 Focus/Footer 区之间应保留最小间距。");
        StringAssert.Contains("LegacyFocusFooterGap", promptText, "legacy prompt 页的 FocusRibbon 与 FooterText 相对距离应固定，不应再各自漂移。");
        StringAssert.Contains("LegacyBottomPadding", promptText, "legacy prompt 页底部保留距离应固定，Focus/Footer 应整体贴近 page 底边。");
        StringAssert.DoesNotContain("Mathf.Max(currentTop + 8f, ResolveLegacyVisibleHeight(page))", promptText, "legacy prompt 页高度不应再吃旧 rect 的残高，而应直接跟当前内容走。");
    }

    [Test]
    public void NpcWorldHintBubble_UsesTriangleIndicatorInsteadOfBubbleBackplateForArrow()
    {
        string bubbleText = File.ReadAllText(NpcWorldHintBubblePath);

        StringAssert.Contains("arrowImage.sprite = GetOrCreateIndicatorSprite();", bubbleText, "NPC 世界提示的倒三角应使用独立三角 sprite，不应再拿气泡底板冒充。");
        StringAssert.Contains("arrowImage.type = Image.Type.Simple;", bubbleText, "NPC 世界提示的倒三角应保持简单三角贴图，不应再走九宫格方块。");
        StringAssert.DoesNotContain("arrowImage.sprite = GetOrCreateBackgroundSprite();", bubbleText, "NPC 世界提示的倒三角不应再直接复用背景气泡 sprite。");
    }

    [Test]
    public void UiLayerUtility_RemainsPublicForSharedEditorValidationMenus()
    {
        string utilityText = File.ReadAllText(UiLayerUtilityPath);

        StringAssert.Contains("public static class SpringDay1UiLayerUtility", utilityText, "UiLayerUtility 应保持 public，避免 Editor 验证菜单访问时再引入编译阻断");
        StringAssert.Contains("IsBlockingPageUiOpen()", utilityText, "UiLayerUtility 应继续暴露页面级 UI 阻挡判断");
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
        StringAssert.Contains("IsLowEnergyWarningActive", energyText, "EnergySystem 应暴露当前低精力 warning 状态，便于 Day1 live 快照验收");
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
        string hintBubbleText = File.ReadAllText(WorldHintBubblePath);

        StringAssert.Contains("EventBus.Subscribe<DialogueStartEvent>", overlayText, "PromptOverlay 应监听对话开始");
        StringAssert.Contains("EventBus.Subscribe<DialogueEndEvent>", overlayText, "PromptOverlay 应监听对话结束");
        StringAssert.Contains("_suppressWhileDialogueActive", overlayText, "PromptOverlay 应在对话期间禁止再次显示");
        StringAssert.Contains("FadeCanvasGroup(0f, false)", overlayText, "PromptOverlay 应在对话开始时立即压低可见度");
        StringAssert.Contains("_queuedPromptText", overlayText, "PromptOverlay 应缓存对话前后的待显示提示");
        StringAssert.Contains("WaitAndRevealQueuedPrompt", overlayText, "PromptOverlay 应在对话框完全收起后再恢复提示");
        StringAssert.Contains("SpringDay1UiLayerUtility.IsBlockingPageUiOpen()", overlayText, "PromptOverlay 应在页面级 UI 打开时主动降级隐藏");
        StringAssert.Contains("BuildCurrentViewState", overlayText, "PromptOverlay 应根据导演层任务状态构建正式任务页内容");
        StringAssert.Contains("DisplaySignature", overlayText, "PromptOverlay 应区分结构变化与实时数值变化，避免任务页被高频进度刷新打抖");
        StringAssert.Contains("ApplyPendingStateWithoutTransition", overlayText, "PromptOverlay 应支持实时进度原位刷新，而不是每次都走任务页转场");
        StringAssert.Contains("TransitionToPendingState", overlayText, "PromptOverlay 应在任务变化时执行逐条过渡");
        StringAssert.Contains("PlayPageFlip", overlayText, "PromptOverlay 应支持任务页翻页特效");
        StringAssert.Contains("completionStepDuration", overlayText, "PromptOverlay 应显式配置逐条完成动画时长");
        StringAssert.Contains("postDialogueResumeDelay", overlayText, "PromptOverlay 应支持对话结束后的缓冲淡入延迟");
        StringAssert.Contains("ShouldIgnoreDialogueEndEvent()", overlayText, "连续对话时 PromptOverlay 不应因旧 End 事件提前恢复");
        StringAssert.Contains("ShouldIgnoreDialogueEndEvent()", hintBubbleText, "连续对话时世界提示气泡不应因旧 End 事件提前恢复");
        StringAssert.Contains("CurrentCaptionText", hintBubbleText, "世界提示气泡应暴露当前标题，便于 Day1 运行态快照读取正式提示内容");
        StringAssert.Contains("CurrentDetailText", hintBubbleText, "世界提示气泡应暴露当前明细，便于 Day1 运行态快照读取正式提示内容");
        StringAssert.Contains("IsVisible", hintBubbleText, "世界提示气泡应暴露当前可见态，避免运行态快照只能猜测提示是否正在显示");
    }

    [Test]
    public void ProximityService_TaskPriorityCopy_ShouldOnlyApplyWhileFormalDialogueIsStillAvailable()
    {
        string proximityText = File.ReadAllText(ProximityServicePath);
        int priorityCheckIndex = proximityText.IndexOf("candidate.Priority != current.Priority", System.StringComparison.Ordinal);
        int distanceCheckIndex = proximityText.IndexOf("candidate.BoundaryDistance < current.BoundaryDistance", System.StringComparison.Ordinal);

        StringAssert.Contains("GetFormalDialogueStateForCurrentStory() != NPCFormalDialogueState.Available", proximityText, "formal 已消费后，玩家面提示壳不应继续伪装成“进入任务”的正式剧情入口。");
        StringAssert.Contains("ResolveNpcDialogueInteractable", proximityText, "玩家面提示壳在同 NPC 存在 formal + informal 时应显式读取 formal 交互组件。");
        StringAssert.Contains("TaskPriorityOverlayCaption", proximityText, "玩家面提示壳仍应保留 formal 未消费时的正式任务 copy。");
        Assert.That(priorityCheckIndex, Is.GreaterThanOrEqualTo(0), "近身仲裁服务应继续保留显式 priority 比较。");
        Assert.That(distanceCheckIndex, Is.GreaterThanOrEqualTo(0), "近身仲裁服务应继续保留距离兜底比较。");
        Assert.That(priorityCheckIndex, Is.LessThan(distanceCheckIndex), "formal / informal / resident 抢占时，应先比语义 priority，再比距离，不应让更近的闲聊先抢走 formal。");
        StringAssert.DoesNotContain("LooksLikeGenericDialoguePrompt", proximityText, "formal 提示壳不应再只靠固定串命中才回正成“进入任务”。");
    }

    [Test]
    public void PlayerNpcNearbyFeedback_SuppressesAmbientNpcBubbleDuringDialogue()
    {
        string serviceText = File.ReadAllText(NearbyFeedbackPath);

        StringAssert.Contains("EventBus.Subscribe<DialogueStartEvent>", serviceText, "玩家靠近 NPC 的轻反馈应监听正式对话开始");
        StringAssert.Contains("EventBus.Subscribe<DialogueEndEvent>", serviceText, "玩家靠近 NPC 的轻反馈应监听正式对话结束");
        StringAssert.Contains("DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive", serviceText, "轻反馈应主动复核正式对话占用态");
        StringAssert.Contains("ShouldSuppressNearbyFeedbackForCurrentStory()", serviceText, "玩家靠近 NPC 的轻反馈应统一通过单一守门判断是否该压住 resident nearby。");
        StringAssert.Contains("NpcInteractionPriorityPolicy.ShouldSuppressAmbientBubbleForCurrentStory()", serviceText, "轻反馈应与环境气泡优先级口径保持一致，只在当前真的有 formal takeover 时压 nearby。");
        StringAssert.Contains("HideActiveNearbyBubble();", serviceText, "正式对话接管时，应主动回收此前留下的日常 NPC 气泡");
        StringAssert.Contains("return suppressWhileDialogueActive ||", serviceText, "轻反馈应先看正式对话占用，再结合 formal prompt takeover 决定是否压 resident nearby。");
        StringAssert.Contains("public string DebugSummary", serviceText, "玩家靠近 NPC 的轻反馈应公开最小调试摘要，便于 day1 直接把 resident nearby 承接吃回 live snapshot。");
        StringAssert.Contains("nearbySuppressed=", serviceText, "轻反馈调试摘要应显式记录当前 resident nearby 是否被 formal takeover 压住。");
        StringAssert.Contains("dialogueSuppressed=", serviceText, "轻反馈调试摘要应区分“正在正式对话中”与“formal prompt 正占焦点”。");
        StringAssert.Contains("activeNpc=", serviceText, "轻反馈调试摘要应显式记录当前冒泡的 NPC。");
        StringAssert.Contains("lastNpc=", serviceText, "轻反馈调试摘要应显式记录最近一次成功触发的 resident nearby 来源。");
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
        StringAssert.Contains("等待 StoryManager 初始化", runnerText, "运行态验收器的玩家面与技术面摘要在 StoryManager 尚未就位时也应给出稳定占位，不应先空引用");
        StringAssert.Contains("TryTriggerNpcDialogue()", runnerText, "运行态验收器应支持触发 NPC001 对话");
        StringAssert.Contains("TryTriggerWorkbenchInteraction()", runnerText, "运行态验收器应支持触发工作台交互");
        StringAssert.Contains("TryAdvanceFarmingTutorialValidationStep()", runnerText, "运行态验收器应支持模拟农田教学最小推进，便于继续验证 EP 与 low-energy warning");
        StringAssert.Contains("TryTriggerRestInteraction()", runnerText, "运行态验收器应支持触发回住处休息");
        StringAssert.Contains("CloseBlockingPageUiForValidation()", runnerText, "运行态验收器在触发 NPC / 工作台 / 休息交互前应先收掉挡路的页面级 UI");
        StringAssert.Contains("packageTabs.ShowPanel(false);", runnerText, "运行态验收器应能在验证入口自动收起 PackagePanel，避免开场交互被页面 UI 挡死");
        StringAssert.Contains("BoxPanelUI.ActiveInstance.Close();", runnerText, "运行态验收器应能在验证入口自动关闭箱子 UI，避免回住处或工作台验收被旧页面状态挡住");
        StringAssert.Contains("ShouldAllowNpcValidationFallback()", runnerText, "运行态验收器应对开场 NPC 交互保留验证专用距离兜底");
        StringAssert.Contains("已通过验收入口脚本触发 NPC 对话", runnerText, "当玩家初始站位不够近时，开场 NPC 验收入口应允许一次仅验证用的脚本触发");
        StringAssert.Contains("GetValidationHealingNextAction()", runnerText, "疗伤桥改成玩家靠近艾拉后，运行态验收器也应改成显式给出这一步的建议。");
        StringAssert.Contains("GetValidationFreeTimeNextAction()", runnerText, "运行态验收器应支持给出自由时段的夜间推进建议");
        StringAssert.Contains("TryAdvanceFreeTimeValidationStep()", runnerText, "运行态验收器应支持模拟夜间推进与两点规则收束");
        StringAssert.Contains("EnsureValidationRunInBackground()", runnerText, "运行态验收器初始化时应主动接管后台运行，避免失焦时自动演出停摆");
        StringAssert.Contains("Application.runInBackground = true;", runnerText, "运行态验收器应在需要时临时开启后台运行");
        StringAssert.Contains("RestoreValidationRunInBackground()", runnerText, "运行态验收器销毁时应恢复原始后台运行配置");
        StringAssert.Contains("DialogueManager dialogueManager = DialogueManager.Instance;", runnerText, "运行态排队播对白时应每帧重新解析 DialogueManager，避免 live 切换瞬间空引用");
        StringAssert.Contains("if (dialogueManager == null)", runnerText, "运行态排队播对白时应容忍 DialogueManager 短暂缺席");
        StringAssert.Contains("已通过验收入口脚本触发工作台回忆", runnerText, "运行态验收器在工作台闪回阶段应支持一次脚本交互兜底，避免因玩家站位影响继续验收");
        StringAssert.Contains("执行 Step，模拟第一格开垦并验证 EP 首次出现。", runnerText, "运行态验收器应给出 FarmingTutorial 的最小验收推进提示");
        StringAssert.Contains("执行 Step，模拟完成浇水并压到 low-energy warning 阈值。", runnerText, "运行态验收器应把 low-energy warning 也纳入 farming validation 提示");
        StringAssert.Contains("followupPending=", runnerText, "运行态验收快照应显式记录首段 follow-up 是否仍待完成");
        StringAssert.Contains("workbenchAwaiting=", runnerText, "运行态验收快照应显式记录工作台闪回是否仍等待首次交互");
        StringAssert.Contains("dinnerPending=", runnerText, "运行态验收快照应显式记录晚餐对白是否仍待接管");
        StringAssert.Contains("reminderPending=", runnerText, "运行态验收快照应显式记录归途提醒对白是否仍待接管");
        StringAssert.Contains("sleepReady=", runnerText, "运行态验收快照应显式记录自由时段是否已允许睡觉收束");
        StringAssert.Contains("nightPressure=", runnerText, "运行态验收快照应显式记录自由时段当前的夜间压力等级");
        StringAssert.Contains("AppendPair(\"HealingBridge\"", runnerText, "运行态验收快照应显式记录疗伤桥是否还在等玩家靠近艾拉。");
        StringAssert.Contains("WorldHint", runnerText, "运行态验收快照应显式记录当前近身提示焦点，便于核对唯一提示与唯一 E 仲裁");
        StringAssert.Contains("PlayerFacing", runnerText, "运行态验收快照应额外提供面向玩家的阶段摘要，而不只是技术状态拼接");
        StringAssert.Contains("已模拟推进到夜里 10 点", runnerText, "运行态验收器应支持把自由时段直接推进到 22:00，并验证回住处压力开始增强");
        StringAssert.Contains("已模拟推进到午夜", runnerText, "运行态验收器应支持把自由时段继续推进到午夜，验证夜深提醒升级");
        StringAssert.Contains("已模拟推进到凌晨一点", runnerText, "运行态验收器应支持把自由时段继续推进到凌晨一点，验证最终催促");
        StringAssert.Contains("验收入口：已模拟两点规则触发，Day1 应进入结束态。", runnerText, "运行态验收器应保留两点规则的最终收束入口，便于继续验证 DayEnd");
        StringAssert.Contains("nightPressure={director.GetFreeTimePressureState()}", runnerText, "运行态验收快照应通过导演实时状态写出当前夜间压力等级，而不是再依赖写死的 final-call 字面量。");
        StringAssert.Contains("FreeTimePressureTier.FinalCall => \"final-call\"", runnerText, "导演层仍应保留凌晨一点的 final-call 压力态，避免玩家面快照无法分辨最终催促。");
        StringAssert.Contains("warn=", runnerText, "运行态验收快照应显式记录当前低精力 warning 是否生效");
        StringAssert.Contains("AppendPair(\"Move\"", runnerText, "运行态验收快照应显式记录当前移动倍率，便于验证低精力减速");
        StringAssert.Contains("AppendPair(\"BeatConsumption\"", runnerText, "运行态验收快照应显式记录当前 beat 的 resident consumption 摘要，便于核对 director 对 NPC 常驻消费的真实承接。");
        StringAssert.Contains("AppendPair(\"OneShot\"", runnerText, "运行态验收快照应显式记录正式剧情 one-shot 消费状态，便于核对已消费桥段不会重复接管。");
        StringAssert.Contains("BuildNpcSummary", runnerText, "运行态验收快照应把 NPC formal/resident 回落状态收进统一摘要，便于核对正式剧情是否已经让位给闲聊。");
        StringAssert.Contains("BuildNpcPromptSummary", runnerText, "运行态验收快照应把 NPC 提示壳是否已回落 resident 语义收进统一摘要。");
        StringAssert.Contains("BuildNpcNearbySummary", runnerText, "运行态验收快照应把 resident nearby 轻反馈收进统一摘要，便于核对 formal phase 是否已压住自动冒泡。");
        StringAssert.Contains("BuildWorkbenchUiSummary", runnerText, "运行态验收快照应把工作台左列 runtime 壳状态收进统一摘要，避免只能靠截图猜 UI 是否恢复。");
        StringAssert.Contains("AppendPair(\"NpcPrompt\"", runnerText, "运行态验收快照应显式记录 NPC prompt 当前是否已经回落 resident 文案。");
        StringAssert.Contains("AppendPair(\"NpcNearby\"", runnerText, "运行态验收快照应显式记录 NPC nearby 轻反馈当前状态。");
        StringAssert.Contains("AppendPair(\"WorkbenchUi\"", runnerText, "运行态验收快照应显式记录工作台左列 runtime 壳恢复状态。");
        StringAssert.Contains("formal=", runnerText, "运行态验收快照应显式记录 NPC 当前 formal 状态。");
        StringAssert.Contains("yieldResident=", runnerText, "运行态验收快照应显式记录 NPC 是否已正式让位给 resident/闲聊内容。");
        StringAssert.Contains("residentTone=", runnerText, "运行态验收快照应显式记录 NPC prompt 是否已经切回 resident 语义。");
        StringAssert.Contains("activeNpc=", runnerText, "运行态验收快照应显式记录当前 resident nearby 正在冒泡的 NPC。");
        StringAssert.Contains("GetRuntimeRecipeShellSummary()", runnerText, "运行态验收快照应通过工作台浮层的 runtime summary 带出当前左列选中的配方壳状态。");
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
        string uiLayerUtilityText = File.ReadAllText(UiLayerUtilityPath);
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
        StringAssert.Contains("SpringDay1ProximityInteractionService.ReportCandidate", interactableText, "工作台近身提示与 E 触发应走 Day1 统一近身仲裁服务，而不是每个对象各自抢键");
        StringAssert.Contains("TryHandleWorkbenchTestInteraction", interactableText, "工作台在没有正式制作面板时应走 Day1 测试兜底");
        StringAssert.Contains("HideIfExists(transform)", interactableText, "工作台回收世界提示气泡时不应为了 Hide 反向创建新的运行时 UI");
        StringAssert.Contains("preferStoryWorkbenchOverlay", interactableText, "工作台交互应支持优先打开 Day1 专用制作浮层");
        StringAssert.Contains("ResolveWorkbenchOverlay", interactableText, "工作台交互应自动解析 Day1 工作台浮层");
        StringAssert.Contains("overlay.Toggle(transform, context?.PlayerTransform, craftingService, station, overlayAutoCloseDistance)", interactableText, "工作台交互应把玩家位置和自动关闭距离传给 Day1 浮层");
        StringAssert.Contains("interactionDistance = 0.5f", interactableText, "工作台交互距离应收口到 0.5 米");
        StringAssert.Contains("overlayAutoCloseDistance = 1.5f", interactableText, "工作台 UI 打开后离开 1.5 米应自动关闭");
        StringAssert.Contains("GetVisualBounds()", interactableText, "工作台交互应暴露视觉边界供悬浮 UI 与提示气泡复用");
        StringAssert.Contains("TryGetClosestVisualPoint", interactableText, "工作台交互应优先基于视觉轮廓计算最近交互点");
        StringAssert.Contains("StoryProgressPersistenceService.IsWorkbenchHintConsumed()", interactableText, "工作台首次 E 提示应读取统一剧情持久态，而不是散落 PlayerPrefs 直写。");
        StringAssert.Contains("StoryProgressPersistenceService.MarkWorkbenchHintConsumed()", interactableText, "工作台首次 E 提示消费后应写入统一剧情持久态。");
        StringAssert.Contains("RecipeResourceFolder", overlayText, "Day1 工作台浮层应从正式 RecipeData 资源目录读取配方");
        StringAssert.Contains("Resources.LoadAll<RecipeData>", overlayText, "Day1 工作台浮层不应再运行时伪造 RecipeData");
        StringAssert.Contains("selectedMaterialsText", overlayText, "Day1 工作台浮层应提供右侧详情区");
        StringAssert.Contains("quantitySlider", overlayText, "Day1 工作台浮层应提供数量滑条");
        StringAssert.Contains("decreaseButton", overlayText, "Day1 工作台浮层应提供数量减号按钮");
        StringAssert.Contains("increaseButton", overlayText, "Day1 工作台浮层应提供数量加号按钮");
        StringAssert.Contains("_displayBelow", overlayText, "Day1 工作台浮层应支持工作台上/下两个显示方向");
        StringAssert.Contains("ApplyDisplayDirection", overlayText, "Day1 工作台浮层应显式切换上/下两种悬浮方向");
        StringAssert.Contains("SpringDay1UiLayerUtility.TryProjectWorldToCanvas(", overlayText, "Day1 工作台浮层应使用真实世界投影坐标把工作台位置投到 UI 画布上。");
        StringAssert.Contains("GetUiEventCamera(Canvas canvas)", uiLayerUtilityText, "Day1 UI 层工具应显式区分 UI 事件相机。");
        StringAssert.Contains("GetWorldProjectionCamera(Canvas canvas = null)", uiLayerUtilityText, "Day1 UI 层工具应显式区分世界投影相机。");
        StringAssert.Contains("pointerRect", overlayText, "Day1 工作台浮层应带有指向工作台的悬浮指针");
        StringAssert.Contains("GetDisplayDecisionSamplePoint", overlayText, "Day1 工作台浮层应根据玩家相对工作台的位置决定上/下显示方向。");
        StringAssert.Contains("ShouldDisplayBelow(Vector2 playerPosition)", overlayText, "Day1 工作台浮层应保留明确的上/下显示判定逻辑。");
        StringAssert.Contains("GetCurrentAutoHideDistance()", overlayText, "Day1 工作台浮层在制作进行中应收紧离台自动关闭距离。");
        StringAssert.Contains("UpdateFloatingProgressVisibility()", overlayText, "Day1 工作台浮层应显式维护离台小进度卡的显隐。");
        StringAssert.Contains("RepositionFloatingProgressCards(", overlayText, "Day1 工作台浮层应持续把离台小进度卡对准工作台。");
        StringAssert.Contains("BuildStageHint", overlayText, "Day1 工作台浮层应保留阶段提示生成入口。");
        StringAssert.Contains("UpdateProgressLabel", overlayText, "Day1 工作台浮层应统一刷新主进度标签与制作态文案。");
        StringAssert.Contains("PushDirectorCraftProgress", overlayText, "Day1 工作台浮层应把制作进度同步回导演层提示系统。");
        StringAssert.Contains("制作完成", overlayText, "Day1 工作台浮层应对完成产物给出明确反馈。");
        StringAssert.Contains("中断制作", overlayText, "Day1 工作台浮层应明确提示当前按钮已切到中断制作语义。");
        StringAssert.Contains("SetWorkbenchAnimating(true)", overlayText, "Day1 工作台浮层应驱动工作台进入明确的工作动画状态。");
        StringAssert.Contains("RecipeColumn", overlayText, "Day1 工作台浮层应保留左侧滚动配方列");
        StringAssert.Contains("DetailColumn", overlayText, "Day1 工作台浮层应保留右侧详情列");
        StringAssert.Contains("QuantityControls", overlayText, "Day1 工作台浮层应保留底部数量调节区");
        StringAssert.Contains("BuildRowSummary", overlayText, "Day1 工作台浮层左列摘要应由正式配方信息生成");
        StringAssert.Contains("recipe.craftingTime", overlayText, "Day1 工作台浮层应读取正式配方里的制作耗时");
        StringAssert.Contains("SetWorkbenchAnimating", overlayText, "Day1 工作台浮层应尝试驱动工作台动画状态");
        StringAssert.Contains("UpdateProgressLabel(GetSelectedRecipe())", overlayText, "工作台制作中的逐帧刷新应只重绘当前选中的 recipe，不应再拿 active recipe 覆盖右侧详情区。");
        StringAssert.Contains("SpringDay1UiLayerUtility.IsBlockingPageUiOpen()", overlayText, "Day1 工作台浮层应在页面级 UI 打开时主动退场");
        StringAssert.Contains("AdjustLegacyDetailLayoutToFitCurrentContent();", overlayText, "Day1 工作台 prefab 详情壳在每次刷新时都应按当前文本重新排版，不能只恢复旧基线。");
        StringAssert.Contains("if (UsesPrefabDetailShell())", overlayText, "Day1 工作台正式 prefab 壳不应再被 legacy fallback 重排逻辑拉裂。");
        StringAssert.Contains("NormalizePrefabDetailShellGeometry()", overlayText, "Day1 工作台正式 prefab 壳至少应把标题/简介这些坏掉的直接文本几何拉回可见范围。");
        StringAssert.Contains("Mathf.Abs(anchorMin.y - anchorMax.y)", overlayText, "Day1 工作台左列 Name/Summary 的 top 对齐应区分纵向是否拉伸，不能再把横向拉伸文本写出 row 外。");
        StringAssert.Contains("FindCloneableRecipeRowTemplate()", overlayText, "Day1 工作台左列重建时应优先保留 prefab row 模板，不应直接退回临时生成壳。");
        StringAssert.Contains("NeedsRecipeRowHardRecovery()", overlayText, "Day1 工作台左列如果 runtime 仍然只剩空壳，应进入更硬的可见性恢复，而不是继续沿用坏 row。");
        StringAssert.Contains("RebuildRecipeRowsFromScratch(forceRuntimePrefabStyle: true);", overlayText, "Day1 工作台左列一旦进入恢复链，就应直接切到 runtime prefab-style row，避免继续复制空白模板。");
        StringAssert.Contains("ForceRuntimeRecipeRowsIfNeeded()", overlayText, "Day1 工作台左列不应继续赌坏 row 自己恢复，而应主动切到可读的 runtime row。");
        StringAssert.Contains("GetRuntimeRecipeShellSummary()", overlayText, "Day1 工作台应公开最小 runtime 壳摘要，便于 day1 验收快照直接读左列恢复真值。");
        StringAssert.Contains("!HasGeneratedRecipeRowChain()", overlayText, "Day1 工作台左列如果还不是稳定生成式 row，也应强制重建，不再继续沿用旧手工模板。");
        StringAssert.Contains("IsRectReasonablyInsideViewport(row.name.rectTransform, row.rect)", overlayText, "Day1 工作台左列文本如果还被挤出 row 外，也应被视为坏壳并强制恢复。");
        StringAssert.Contains("HorizontalLayoutGroup rowLayout", overlayText, "Day1 工作台左列 runtime row 应使用稳定生成式布局，不应再只靠脆弱的手工坐标。");
        StringAssert.Contains("rowLayout.childControlWidth = true", overlayText, "Day1 工作台左列 runtime row 必须真实给子项分配宽度，不然 icon 和文字会被压成空壳。");
        StringAssert.Contains("rowLayout.childControlHeight = true", overlayText, "Day1 工作台左列 runtime row 必须真实给子项分配高度，不然 icon 卡片和文本列会继续拿不到尺寸。");
        StringAssert.Contains("generatedLayout.childControlWidth = true", overlayText, "Day1 工作台运行时复用已有生成式 row 时，也必须把横向布局控制重新拉回真值。");
        StringAssert.Contains("generatedLayout.childControlHeight = true", overlayText, "Day1 工作台运行时复用已有生成式 row 时，也必须把纵向布局控制重新拉回真值。");
        StringAssert.Contains("SanitizeRecipeRowImage(background, preserveAspect: false);", overlayText, "Day1 工作台左列 row 若被错误材质或错误 source image 污染，应先把背景图状态清回正式壳。");
        StringAssert.Contains("SanitizeRecipeRowImage(accentImage, preserveAspect: false);", overlayText, "Day1 工作台左列 accent 若被错误材质或错误 source image 污染，也应被拉回正式纯色条。");
        StringAssert.Contains("SanitizeRecipeRowImage(iconCardImage, preserveAspect: false);", overlayText, "Day1 工作台左列 iconCard 若被污染，也应回到纯色卡片壳。");
        StringAssert.Contains("SanitizeRecipeRowImage(iconImage, preserveAspect: true);", overlayText, "Day1 工作台左列真正的配方 icon 应保留 sprite，但不应继续挂错材质。");
        StringAssert.Contains("NormalizeGeneratedRecipeRowGeometry(rowRect, accentRect, iconCardRect, textColumnRect);", overlayText, "Day1 工作台生成式 row 应显式把 Accent/IconCard/TextColumn/文本 rect 拉回稳定几何。");
        StringAssert.Contains("HasInvalidRecipeRowGraphicMaterial(row.background)", overlayText, "Day1 工作台左列如果背景图仍残留错误材质或坏图形状态，应被视为坏壳并强制恢复。");
        StringAssert.Contains("HasInvalidRecipeRowGraphicMaterial(row.accent)", overlayText, "Day1 工作台左列 accent 如果被错误材质或坏图形状态污染，应被视为坏壳并强制恢复。");
        StringAssert.Contains("HasInvalidRecipeRowGraphicMaterial(row.icon)", overlayText, "Day1 工作台左列真正 icon 如果挂上错材质或坏图形状态，也应被 recovery 链识别出来。");
        StringAssert.Contains("else if (CanInterruptActiveEntry(entry))", overlayText, "Day1 工作台在制作中未选择追加数量时，应把中断语义交给当前 recipe 的进度条状态机。");
        StringAssert.Contains("craftButton.gameObject.SetActive(showCraftButton);", overlayText, "Day1 工作台在未选择数量时应让 CraftButton 退场，由进度条承担领取/中断/取消排队语义。");
        StringAssert.Contains("if (isCrafting && _selectedQuantity <= 0)", overlayText, "Day1 工作台点击 CraftButton 时，应在制作中把“未选数量”解释成中断。");
        StringAssert.Contains("return GetCraftHoverLabel();", overlayText, "Day1 工作台在 hover CraftButton 时，应直接给出追加/中断的玩家语义，而不是继续空标签。");
        StringAssert.Contains("SetTopStretchRect(nameRect", overlayText, "Day1 工作台右侧标题恢复时应连带把左右边距一起收正，不能只改字号。");
        StringAssert.Contains("LooksLikeInternalRecipeName", overlayText, "Day1 工作台玩家面名称不应继续直接显示内部 recipeID 样式名字。");
        StringAssert.Contains("BuildPlayerFacingInternalName", overlayText, "Day1 工作台玩家面名称在 itemName 和 recipeName 都是内部 ID 时，也应回退到真正面向玩家的工具名。");
        StringAssert.Contains("ApplyDetailColumnFontTuning()", overlayText, "Day1 工作台右侧详情列字体应统一再抬一点，避免正式面显得过小发虚。");
        StringAssert.Contains("NotifyWorkbenchCraftProgress", directorText, "Day1 导演应接收工作台制作中的正式进度同步");
        StringAssert.Contains("BuildWorkbenchCraftProgressText", directorText, "Day1 导演应能把制作中状态映射到任务页/任务进度");
        StringAssert.Contains("工作台制作中", directorText, "Day1 导演的任务描述应在制作中明确体现工作态");
        StringAssert.DoesNotContain("E 打开，超出 1.5 米自动收起", overlayText, "正式工作台 UI 不应再出现测试说明文案");
        StringAssert.DoesNotContain("工作台 UI 只响应鼠标左键", overlayText, "正式工作台 UI 不应在面板里显示调试提示语");
        StringAssert.Contains("recipeID: 9100", axeRecipeText, "Axe_0 配方资源应存在且使用固定 recipeID");
        StringAssert.Contains("recipeName: Axe_0", axeRecipeText, "Axe_0 配方资源应稳定");
        StringAssert.Contains("itemID: 3200", axeRecipeText, "Axe_0 配方资源应使用木材");
        StringAssert.Contains("craftingTime: 5", axeRecipeText, "Axe_0 配方资源应配置当前正式制作耗时。");
        StringAssert.Contains("recipeID: 9101", hoeRecipeText, "Hoe_0 配方资源应存在且使用固定 recipeID");
        StringAssert.Contains("recipeName: Hoe_0", hoeRecipeText, "Hoe_0 配方资源应稳定");
        StringAssert.Contains("craftingTime: 5", hoeRecipeText, "Hoe_0 配方资源应配置当前正式制作耗时。");
        StringAssert.Contains("recipeID: 9102", pickaxeRecipeText, "Pickaxe_0 配方资源应存在且使用固定 recipeID");
        StringAssert.Contains("itemID: 3200", pickaxeRecipeText, "Pickaxe_0 配方资源应改成和 Day1 木斧一致的木料需求。");
        StringAssert.DoesNotContain("itemID: 3201", pickaxeRecipeText, "Pickaxe_0 配方资源不应再保留旧石料需求。");
        StringAssert.Contains("craftingTime: 5", pickaxeRecipeText, "Pickaxe_0 配方资源应配置当前正式制作耗时。");
        StringAssert.Contains("InitializeOnLoad", binderText, "编辑器恢复器应在 Unity 重新编译后自动生效");
        StringAssert.Contains("Anvil_0", binderText, "编辑器恢复器应优先识别 Anvil_0");
        StringAssert.Contains("Undo.AddComponent<CraftingStationInteractable>", binderText, "编辑器恢复器应能自动补挂工作台交互脚本");
    }

    [Test]
    public void UiEvidenceCapture_ContainsDialogueHintAndWorkbenchTruth()
    {
        string evidenceRuntimeText = File.ReadAllText(SpringUiEvidenceRuntimePath);
        string evidenceMenuText = File.ReadAllText(SpringUiEvidenceMenuPath);

        StringAssert.Contains("BuildDialogueSnapshot", evidenceRuntimeText, "Spring UI 证据侧车应直接记录 DialogueUI 当前正式面状态，便于 day1 判断 continue、字体和头像是否真实过线。");
        StringAssert.Contains("BuildInteractionHintSnapshot", evidenceRuntimeText, "Spring UI 证据侧车应直接记录左下角提示壳当前文案与可见状态，便于核对 formal/resident 是否已经正确回落。");
        StringAssert.Contains("runtimeShellSummary = overlay.GetRuntimeRecipeShellSummary()", evidenceRuntimeText, "Workbench 证据侧车应直接带出 runtime shell 摘要，不能只留矩形几何让人继续猜左列是不是空壳。");
        StringAssert.Contains("Play Dialogue + Capture Spring UI Evidence", evidenceMenuText, "Spring UI 调试菜单应提供 DialogueUI 一键抓图入口，避免每次手工拼接对话再抓图。");
        StringAssert.Contains("CaptureCurrentSpringUiEvidence(\"dialogue\")", evidenceMenuText, "DialogueUI 抓图入口应直接产出最终 GameView 证据，而不是只停在触发对话。");
    }

    [Test]
    public void DirectorAndBedBridge_ContainLateDayRuntimeHooks()
    {
        string directorText = File.ReadAllText(DirectorPath);
        string bedInteractableText = File.ReadAllText(BedInteractablePath);
        string proximityServiceText = File.ReadAllText(ProximityServicePath);
        string bedBinderText = File.ReadAllText(BedSceneBinderPath);
        string sceneTransitionText = File.ReadAllText(SceneTransitionTriggerPath);

        StringAssert.Contains("StoryTimePauseSource", directorText, "导演层应显式接管脚本阶段的时间暂停来源");
        StringAssert.Contains("SyncStoryTimePauseState", directorText, "导演层应在阶段切换时同步时间暂停状态");
        StringAssert.Contains("lowEnergyMoveSpeedMultiplier", directorText, "导演层应支持低精力减速");
        StringAssert.Contains("TryTriggerSleepFromBed", directorText, "导演层应提供床交互的 DayEnd 桥接入口");
        StringAssert.Contains("TimeManager.OnHourChanged += HandleHourChanged;", directorText, "导演层应监听小时变化，把自由时段压力做成渐进式状态");
        StringAssert.Contains("TimeManager.OnHourChanged -= HandleHourChanged;", directorText, "导演层停用时应解除小时监听");
        StringAssert.Contains("PreferredBedObjectNames", directorText, "导演层应保留床对象候选名");
        StringAssert.Contains("PreferredRestProxyObjectNames", directorText, "导演层应支持住处入口作为睡觉兜底承载物");
        StringAssert.Contains("House 1_2", directorText, "导演层应优先识别当前主场景里的住处入口对象");
        StringAssert.Contains("EnsureRestInteractableCollider", directorText, "导演层应在缺少碰撞器时自动补交互碰撞体");
        StringAssert.Contains("runtimeCollider.isTrigger = true", directorText, "住处入口兜底碰撞器应使用 Trigger，避免阻挡玩家");
        StringAssert.Contains("FindFirstObjectByType<TimeManager>(FindObjectsInactive.Include)", directorText, "导演层在退场释放时间暂停时不应反向创建新的 TimeManager");
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
        StringAssert.Contains("IsFirstFollowupPending()", directorText, "导演层应暴露首段 follow-up 是否仍待完成的判断，供运行态验收入口复用");
        StringAssert.Contains("IsWorkbenchFlashbackAwaitingInteraction()", directorText, "导演层应暴露工作台闪回是否仍等待首次交互，避免验收入口误反触");
        StringAssert.Contains("IsDinnerDialoguePendingStart()", directorText, "导演层应暴露晚餐对白是否仍待接管，供运行态验收入口复用");
        StringAssert.Contains("IsReminderDialoguePendingStart()", directorText, "导演层应暴露归途提醒对白是否仍待接管，供运行态验收入口复用");
        StringAssert.Contains("IsSleepInteractionAvailable()", directorText, "导演层应暴露自由时段是否已允许睡觉收束");
        StringAssert.Contains("GetRestInteractionHint", directorText, "导演层应能根据夜间压力动态收紧回住处/睡觉交互提示");
        StringAssert.Contains("GetRestInteractionDetail", directorText, "导演层应能根据夜间压力动态收紧回屋休息的细节提示");
        StringAssert.Contains("GetCurrentWorldHintSummary", directorText, "导演层应能输出当前世界提示焦点，便于继续做玩家视角整体验证");
        StringAssert.Contains("BuildPlayerFacingStatusSummary", directorText, "导演层应能把当前阶段、当前焦点和当前任务摘要压成面向玩家的一句话总结");
        StringAssert.Contains("GetFreeTimePressureState()", directorText, "导演层应暴露自由时段当前夜间压力等级");
        StringAssert.Contains("storyManager == null", directorText, "导演层对 Day1 状态公开口应先处理 StoryManager 尚未就位的空场景，避免验收入口或调试快照先空引用");
        StringAssert.Contains("if (_freeTimeEntered && !_dayEnded)", directorText, "导演层应只在 Day1 自由时段真正结束时接管睡眠收束");
        StringAssert.Contains("StoryManager.Instance.SetPhase(StoryPhase.DayEnd);", directorText, "导演层在接到睡眠事件后应显式把 Day1 切到 DayEnd");
        StringAssert.Contains("EnergySystem.Instance.FullRestore();", directorText, "DayEnd 收束时应恢复精力，避免次日承接脏状态");
        StringAssert.Contains("ApplyLowEnergyMovementPenalty(false);", directorText, "DayEnd 收束时应撤销低精力减速");
        StringAssert.Contains("SpringDay1PromptOverlay.Instance.Show(DayEndPromptText)", directorText, "DayEnd 收束时应给出明确的日终提示");
        StringAssert.Contains("SceneManager.LoadScene(HomeSceneName, LoadSceneMode.Single);", directorText, "超过两点的强制收束应把玩家送回 Home 床边，而不是留在别的场景里结束。");
        StringAssert.Contains("StoryPhase.FreeTime", bedInteractableText, "床交互应只在自由时段开放");
        StringAssert.Contains("TimeManager.Instance.Sleep()", bedInteractableText, "床交互应能直接触发睡觉");
        StringAssert.Contains("director.GetRestInteractionHint(interactionHint)", bedInteractableText, "床交互提示应随夜间压力动态收紧");
        StringAssert.Contains("ResolveRestInteractionDetail", bedInteractableText, "床交互的近身提示细节应通过兼容桥接读取导演层的夜间压力文案，避免被底座签名漂移卡死整刀编译");
        StringAssert.Contains("SpringDay1UiLayerUtility.IsBlockingPageUiOpen()", bedInteractableText, "床/回屋休息也应受页面级 UI 阻挡，不允许在背包或箱子打开时误触发");
        StringAssert.Contains("SpringDay1ProximityInteractionService.ReportCandidate", bedInteractableText, "床/回屋休息的近身提示与 E 触发应接入 Day1 统一近身仲裁服务");
        StringAssert.Contains("GetBoundaryDistance", bedInteractableText, "床/回屋休息应显式按包络线距离判断近身交互边界");
        StringAssert.Contains("TryTriggerRestInteraction()", directorText, "导演层的验收入口应能主动触发休息承载物");
        StringAssert.Contains("ShouldAllowRestValidationFallback()", directorText, "自由时段验收入口应允许一次休息交互的距离兜底，避免站位把验收卡死");
        StringAssert.Contains("已通过验收入口脚本触发休息交互", directorText, "自由时段休息验收入口应明确报出距离兜底已经触发");
        StringAssert.Contains("if (playerTransform == null)", directorText, "导演层构造验收交互上下文时应在玩家对象缺失时返回空上下文，而不是伪造 0,0 位置");
        StringAssert.Contains("ShouldReplaceCandidate", proximityServiceText, "Day1 统一近身仲裁服务应显式定义候选替换规则，避免多个近身目标同时争抢提示和 E 键");
        StringAssert.Contains("CurrentFocusSummary", proximityServiceText, "Day1 统一近身仲裁服务应暴露当前焦点摘要，便于导演层和验收入口读取");
        StringAssert.Contains("ResolveOverlayPromptContent", proximityServiceText, "Day1 统一近身仲裁服务应在 teaser 态输出中性靠近提示，而不是继续冒充可立即按键。");
        StringAssert.Contains("Input.GetKeyDown(_currentCandidate.InteractionKey)", proximityServiceText, "Day1 统一近身仲裁服务应只让当前焦点目标消费这次 E 键");
        StringAssert.Contains("if (_instance == this)", proximityServiceText, "Day1 统一近身仲裁服务在销毁时应主动回收静态实例，避免切场景后残留旧引用");
        StringAssert.Contains("InitializeOnLoad", bedBinderText, "床位编辑器恢复器应在 Unity 重新编译后自动生效");
        StringAssert.Contains("Undo.AddComponent<SpringDay1BedInteractable>", bedBinderText, "床位编辑器恢复器应能自动补挂床交互脚本");
        StringAssert.Contains("if (Application.isPlaying)", sceneTransitionText, "SceneTransitionRunner 在 EditMode/测试环境下不应无条件调用 DontDestroyOnLoad。");
        StringAssert.Contains("DontDestroyOnLoad(runnerObject);", sceneTransitionText, "SceneTransitionRunner 在 PlayMode 下仍应保留持久 runner。");
        StringAssert.Contains("DontDestroyOnLoad(gameObject);", sceneTransitionText, "SceneTransitionRunner 的 Awake 也应只在 PlayMode 下保留持久对象。");
    }

    [Test]
    public void DialogueChain_AutoPlaysFollowupBeforeHealing()
    {
        string managerText = File.ReadAllText(DialogueManagerPath);
        string directorText = File.ReadAllText(DirectorPath);

        StringAssert.Contains("ResolveFollowupSequence", managerText, "DialogueManager 应把 follow-up 资源解析成正式自动续播链");
        StringAssert.Contains("PlayDialogue(followupSequence)", managerText, "DialogueManager 应在首段完成后自动续播 follow-up");
        StringAssert.Contains("FirstFollowupSequenceId", directorText, "导演层应显式识别首段 follow-up 的完成节点");
        StringAssert.Contains("if (HasPlayableNodes(evt.FollowupSequence))", directorText, "首段完成时若还要续播 follow-up，导演层不应立刻抢跑疗伤");
        StringAssert.Contains("VillageGateSequenceId", directorText, "导演层应显式识别进村围观段的完成节点");
        StringAssert.Contains("HouseArrivalSequenceId", directorText, "导演层应显式识别闲置小屋安置段的完成节点");
        StringAssert.Contains("evt.SequenceId == FirstFollowupSequenceId", directorText, "导演层应在 follow-up 收束后再进入疗伤");
        StringAssert.Contains("evt.SequenceId == VillageGateSequenceId", directorText, "导演层应在进村围观段收束时改成等待 Primary 承接，而不是直接抢跑疗伤");
        StringAssert.Contains("evt.SequenceId == HouseArrivalSequenceId", directorText, "导演层应在闲置小屋安置段收束后再进入疗伤");
        StringAssert.Contains("if (!IsDialogueChainStillActive() && !IsFirstFollowupPending())", directorText, "EnterVillage 相位变化不应在进村安置链仍待完成时提前启动疗伤");
        StringAssert.Contains("TickTownSceneFlow()", directorText, "导演层应显式支持 Town 开场 runtime，不应继续只在 Primary 驱动 Day1。");
        StringAssert.Contains("TryAdoptTownOpeningState()", directorText, "导演层进入 Town 开场时应把旧 CrashAndMeet opening 口径切到新的进村 opening。");
        StringAssert.Contains("TryHandleTownEnterVillageFlow()", directorText, "导演层应在 Town 场景主动接进进村围观段。");
        StringAssert.Contains("SpringDay1NpcCrowdDirector.IsBeatCueSettled(SpringDay1DirectorBeatKeys.EnterVillagePostEntry)", directorText, "进村围观对白前应先等待 crowd cue 真正站定。");
        StringAssert.Contains("TryQueuePrimaryHouseArrival()", directorText, "导演层应在切到 Primary 后再接旧屋安置，不应继续靠 VillageGate 资产直接串过去。");
        StringAssert.Contains("BuildVillageGateSequence()", directorText, "导演层应显式能构造/读取进村围观段。");
        StringAssert.Contains("BuildHouseArrivalSequence()", directorText, "导演层应显式能构造/读取旧屋安置段。");
        StringAssert.Contains("IsStoryRuntimeSceneActive()", directorText, "导演层应显式识别 Town + Primary 两个 Day1 runtime scene。");
        StringAssert.Contains("TownSceneName", directorText, "导演层应显式识别 Town 作为 Day1 开场场景。");
        StringAssert.Contains("进村围观进行中", directorText, "导演层当前进度文案应识别进村围观段");
        StringAssert.Contains("村里承接对白进行中", directorText, "导演层当前进度文案应识别 Primary 承接段");
        StringAssert.Contains("在村里这边先站稳", directorText, "导演层任务卡文案应明确 Primary 承接这一拍");
        StringAssert.Contains("Town 会自动接进围观；围观收束后切到 Primary，再等承接对白自动接上。", directorText, "运行态验收器应改成符合 Town 开场 -> Primary 承接的新进村链。");
        StringAssert.Contains("进村承接已收束，等待疗伤段启动。", directorText, "运行态验收器在进村承接链已完成后不应继续建议反复触发 NPC001");
        StringAssert.Contains("工作台回忆进行中", directorText, "导演层当前进度文案应能识别工作台回忆正在播出");
        StringAssert.Contains("工作台已打开，等待回忆收束", directorText, "导演层当前进度文案应区分工作台已打开但回忆未完全结束");
        StringAssert.Contains("等待工作台回忆完整播完。", directorText, "导演层焦点提示应区分工作台已打开后的等待态");
        StringAssert.Contains("工作台已打开，等待工作台回忆自动播出。", directorText, "运行态验收器不应在工作台已打开后继续反向触发工作台交互");
        StringAssert.Contains("晚餐对白进行中", directorText, "导演层当前进度文案应能识别晚餐对白正在播出");
        StringAssert.Contains("等待晚餐对白接管", directorText, "导演层当前进度文案应能识别晚餐对白已排队但尚未接管");
        StringAssert.Contains("天已经晚了，先回去吃点东西。", directorText, "白天教学收束后应给出进入晚餐的正式桥接提示");
        StringAssert.Contains("SpringDay1NpcCrowdDirector.IsBeatCueSettled(SpringDay1DirectorBeatKeys.DinnerConflictTable)", directorText, "晚饭对白前也应先等待 dinner crowd cue 真正站定。");
        StringAssert.Contains("归途提醒对白进行中", directorText, "导演层当前进度文案应能识别归途提醒对白正在播出");
        StringAssert.Contains("等待归途提醒对白接管", directorText, "导演层当前进度文案应能识别归途提醒对白已排队但尚未接管");
        StringAssert.Contains("夜色越来越深了，别在外面逗留太久。", directorText, "晚餐收束后应给出归途提醒的桥接提示");
        StringAssert.Contains("先听一圈村里夜里的动静，再决定什么时候回屋。", directorText, "自由时段刚开启时应先给出夜间见闻桥接，而不是立刻放开睡觉收束。");
        StringAssert.Contains("自由时段见闻会先接管，先别急着立刻睡。", directorText, "自由时段 formal 见闻未收束前，焦点提示应明确先让夜间见闻接管。");
        StringAssert.Contains("听完村里夜间见闻", directorText, "自由时段 intro pending 时，任务卡应先要求听完夜间见闻。");
        StringAssert.Contains("自由时段见闻尚未收束，先让这段夜里的动静完整接管。", directorText, "自由时段验收入口在 intro pending 时应先引导收完夜间见闻。");
        StringAssert.DoesNotContain("先把夜里的见闻听完，再决定今晚还做不做别的。", directorText, "完成 0.0.4 进入 0.0.5 后，工作台不应再被自由时段 formal 见闻整体锁回去。");
        StringAssert.DoesNotContain("先完成一次浇水，再回来制作。", directorText, "完成 0.0.4 进入 0.0.5 后，工作台制作不应再被教学步骤重新锁回去。");
        StringAssert.DoesNotContain("晚餐冲突这段得先过完，别在这时回头做活。", directorText, "完成 0.0.4 进入 0.0.5 后，工作台制作不应再被晚餐冲突整体锁回去。");
        StringAssert.Contains("GetPlayerFacingWorkbenchRecipeName", directorText, "导演层工作台进度文案应把内部 recipe 名映射回玩家面名称。");
        StringAssert.Contains("_freeTimeIntroCompleted", directorText, "导演层应显式记录自由时段 intro 是否已收束，避免夜间压力和睡觉交互提前开放。");
        StringAssert.Contains("夜深了，最好尽快回住处休息", directorText, "自由时段进度文案应随着夜深程度逐步收紧");
        StringAssert.Contains("快到凌晨两点了，必须立刻回去睡觉", directorText, "自由时段最终阶段应明确给出两点规则压力");
        StringAssert.Contains("已经过了午夜，先回住处睡觉再说。", directorText, "自由时段焦点提示应在午夜后进一步收紧");
        StringAssert.Contains("快到凌晨两点了，再不睡就会直接昏睡过去。", directorText, "自由时段应在最终阶段给出明确的两点规则催促");
        StringAssert.Contains("晚餐对白已排队，等待接管。", directorText, "运行态验收器应区分晚餐对白待接管状态");
        StringAssert.Contains("归途提醒对白已排队，等待接管。", directorText, "运行态验收器应区分归途提醒对白待接管状态");
        StringAssert.Contains("执行 Step，模拟推进到夜里 10 点并验证回住处压力。", directorText, "自由时段验收入口应支持先推进到夜里 10 点");
        StringAssert.Contains("执行 Step，模拟推进到凌晨一点并验证最终催促。", directorText, "自由时段验收入口应支持推进到凌晨一点验证最终催促");
        StringAssert.Contains("验收入口：已模拟两点规则触发，Day1 应进入结束态。", directorText, "自由时段验收入口应支持直接验证两点规则收束");
        StringAssert.Contains("return \"回住处休息，或继续执行 Step 验证两点规则收束。\";", directorText, "free-time 验收入口在 final-call 后应明确只剩回屋睡觉或继续验证两点规则收束");
    }

    [Test]
    public void HealingAndEnergyPacing_RemainsBoundToDay1FormalSequence()
    {
        string directorText = File.ReadAllText(DirectorPath);

        StringAssert.Contains("healthSystem.SetHealthState(initialHealth, maxHealth);", directorText, "进入疗伤段时应先设定 Day1 初始 HP");
        StringAssert.Contains("healthSystem.SetVisible(false);", directorText, "HP 条应在疗伤演出前保持隐藏");
        StringAssert.Contains("EnergySystem.Instance.SetVisible(false);", directorText, "疗伤段开始时不应提前露出 EP");
        StringAssert.Contains("HealthSystem.Instance.PlayRevealAndAnimateTo(", directorText, "疗伤段应通过正式渐显回血演出拉出 HP");
        StringAssert.Contains("if (!_staminaRevealed && _tillObjectiveCompleted)", directorText, "EP 首次出现应绑定到第一格开垦完成");
        StringAssert.Contains("EnergySystem.Instance.SetEnergyState(initialEnergy, maxEnergy);", directorText, "EP 首次出现时应写入 Day1 正式初值");
        StringAssert.Contains("EnergySystem.Instance.PlayRevealAndAnimateTo(initialEnergy, initialEnergy, maxEnergy, energyRevealDuration, 0f);", directorText, "EP 应通过正式 reveal 动画出现");
        StringAssert.Contains("ResyncLowEnergyState(false);", directorText, "剧情 phase 切换时应重新对齐 low-energy warning 与移动惩罚，避免晚餐入口状态脱节");
        StringAssert.Contains("private void ResyncLowEnergyState(bool allowPrompt)", directorText, "导演层应提供当前精力状态的显式重同步入口");
        StringAssert.Contains("SetLowEnergyWarningVisual(shouldWarn);", directorText, "低精力 warning 应有正式视觉表现");
        StringAssert.Contains("ApplyLowEnergyMovementPenalty(shouldWarn);", directorText, "低精力时应带移动惩罚");
        StringAssert.Contains("精力过低，先休息或吃点东西。", directorText, "低精力 warning 应有明确提示文案");
        StringAssert.Contains("TryAdvanceFarmingTutorialValidationStep()", directorText, "Day1 导演应支持在验收入口里模拟 farming tutorial 最小推进");
        StringAssert.Contains("ApplyValidationEnergyState(lowEnergyWarningThreshold);", directorText, "验收入口应能把 EP 精确压到 low-energy warning 阈值，便于 live 取证");
    }
}
