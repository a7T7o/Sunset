using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class SpringDay1OpeningRuntimeBridgeTests
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
        ResetStaticField(ResolveTypeOrFail("Sunset.Story.SpringDay1LiveValidationRunner"), "_instance");
        ResetStaticField(ResolveTypeOrFail("Sunset.Story.DialogueManager"), "<Instance>k__BackingField");
        ResetStaticField(ResolveTypeOrFail("TimeManager"), "instance");
        ResetStaticField(ResolveTypeOrFail("EnergySystem"), "<Instance>k__BackingField");
        ResetStaticField(ResolveTypeOrFail("HealthSystem"), "_instance");
    }

    [UnityTest]
    public IEnumerator HouseArrivalCompletion_ShouldBridgeIntoHealingAndHp()
    {
        EnsureTestPrimarySceneActive();
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object enterVillagePhase = ParseEnum(context.storyPhaseType, "EnterVillage");
        object healingPhase = ParseEnum(context.storyPhaseType, "HealingAndHP");
        InvokeInstance(context.storyManager, "ResetState", enterVillagePhase, false);

        Type completedEventType = ResolveTypeOrFail("Sunset.Events.DialogueSequenceCompletedEvent");
        object completedEvent = Activator.CreateInstance(completedEventType);
        SetWritableMember(completedEvent, "SequenceId", "spring-day1-house-arrival");
        SetWritableMember(completedEvent, "FollowupSequence", null);
        SetWritableMember(completedEvent, "CurrentPhase", enterVillagePhase);

        InvokeInstance(context.director, "HandleDialogueSequenceCompleted", completedEvent);

        Assert.That(GetPropertyValue(context.storyManager, "CurrentPhase"), Is.EqualTo(healingPhase), "Primary 承接段收束后应自动接到疗伤阶段。");
        Assert.That((string)InvokeInstance(context.director, "GetCurrentTaskLabel"), Is.EqualTo("0.0.3 疗伤/血条"));
        Assert.That((string)InvokeInstance(context.director, "GetCurrentProgressLabel"), Is.EqualTo("先走到艾拉身边，让她接手疗伤"));
        Assert.That((string)InvokeInstance(context.director, "GetCurrentFocusTextForTests"), Is.EqualTo("先走到艾拉身边，再让疗伤对白和 HP 卡片接管。"));
        Assert.That((bool)GetPropertyValue(context.healthSystem, "IsVisible"), Is.False, "疗伤开场前应先隐藏 HP，等演出自己显现。");
        Assert.That((bool)GetPropertyValue(context.energySystem, "IsVisible"), Is.False, "疗伤阶段不应提前亮出精力条。");
    }

    [UnityTest]
    public IEnumerator LiveValidationRunner_ShouldRecommendOpeningActionsForCrashAndEnterVillage()
    {
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        Type runnerType = ResolveTypeOrFail("Sunset.Story.SpringDay1LiveValidationRunner");
        Component runner = Track(new GameObject("SpringDay1LiveValidationRunner_Test")).AddComponent(runnerType);

        object crashPhase = ParseEnum(context.storyPhaseType, "CrashAndMeet");
        object enterVillagePhase = ParseEnum(context.storyPhaseType, "EnterVillage");

        InvokeInstance(context.storyManager, "ResetState", crashPhase, false);
        Assert.That((string)InvokeInstance(runner, "GetRecommendedNextAction"), Is.EqualTo("触发 NPC001 首段对话，接住醒来、危险感与撤离链。"));

        InvokeInstance(context.storyManager, "ResetState", enterVillagePhase, false);
        Assert.That(
            (string)InvokeInstance(runner, "GetRecommendedNextAction"),
            Is.EqualTo("Town 会自动接进围观；围观收束后切到 Primary，再等承接对白自动接上。"),
            "进入村庄后，验收入口应明确提示当前是 Town 围观 -> Primary 承接的双场景接力。");
    }

    [Test]
    public void GenericTownTransitionTrigger_ShouldFallbackToPrimaryHomeEntryAnchor()
    {
        EnsureTestSceneActive("Town");

        GameObject triggerObject = Track(new GameObject("SceneTransitionTrigger"));
        triggerObject.AddComponent<BoxCollider2D>().isTrigger = true;
        Type transitionTriggerType = ResolveTypeOrFail("Sunset.Story.SceneTransitionTrigger2D");
        Component transitionTrigger = triggerObject.AddComponent(transitionTriggerType);
        InvokeInstance(transitionTrigger, "SetTargetScene", "Primary", "Assets/000_Scenes/Primary.unity", string.Empty);

        Assert.That(GetPropertyValue(transitionTrigger, "TargetEntryAnchorName"), Is.EqualTo("PrimaryHomeEntryAnchor"));
    }

    [Test]
    public void GenericPrimaryTransitionTrigger_ShouldFallbackToTownOpeningEntryAnchor()
    {
        EnsureTestSceneActive("Primary");

        GameObject triggerObject = Track(new GameObject("SceneTransitionTrigger"));
        triggerObject.AddComponent<BoxCollider2D>().isTrigger = true;
        Type transitionTriggerType = ResolveTypeOrFail("Sunset.Story.SceneTransitionTrigger2D");
        Component transitionTrigger = triggerObject.AddComponent(transitionTriggerType);
        InvokeInstance(transitionTrigger, "SetTargetScene", "Town", "Assets/000_Scenes/Town.unity", string.Empty);

        Assert.That(GetPropertyValue(transitionTrigger, "TargetEntryAnchorName"), Is.EqualTo("TownPlayerEntryAnchor"));
    }

    [Test]
    public void PersistentPlayerSceneBridge_ShouldTreatPipeSeparatedEntryNamesAsAliases()
    {
        Type bridgeType = ResolveTypeOrFail("PersistentPlayerSceneBridge");
        MethodInfo matchMethod = bridgeType.GetMethod(
            "DoesEntryAnchorNameMatch",
            BindingFlags.Static | BindingFlags.NonPublic);

        Assert.That(matchMethod, Is.Not.Null, "PersistentPlayerSceneBridge 应支持 entry anchor alias 匹配，避免 scene 命名微调时直接掉回默认 Player 点。");
        Assert.That((bool)matchMethod.Invoke(null, new object[] { "TownPlayerEntryAnchor", "村民汇聚点|TownPlayerEntryAnchor" }), Is.True);
        Assert.That((bool)matchMethod.Invoke(null, new object[] { "PrimaryHomeEntryAnchor", "村民汇聚点|TownPlayerEntryAnchor" }), Is.False);
    }

    [UnityTest]
    public IEnumerator TownOpening_ShouldAdoptCrashPhaseIntoEnterVillageBeat()
    {
        EnsureTestSceneActive("Town");
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object crashPhase = ParseEnum(context.storyPhaseType, "CrashAndMeet");
        object enterVillagePhase = ParseEnum(context.storyPhaseType, "EnterVillage");
        InvokeInstance(context.storyManager, "ResetState", crashPhase, false);

        InvokeInstance(context.director, "TryAdoptTownOpeningState");

        Assert.That(GetPropertyValue(context.storyManager, "CurrentPhase"), Is.EqualTo(enterVillagePhase), "Town 开场时，导演应把旧的 CrashAndMeet 自动收口为 EnterVillage。");
        Assert.That((string)InvokeInstance(context.director, "GetCurrentBeatKey"), Is.EqualTo("EnterVillage_PostEntry"), "Town 开场改口后，当前 beat 应直接落到进村围观段。");
    }

    [UnityTest]
    public IEnumerator TownEnterVillageFlow_ShouldEnsureNpcCrowdDirectorRuntime()
    {
        EnsureTestSceneActive("Town");
        RuntimeContext context = CreateRuntimeContext();
        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        yield return null;

        ResetStaticField(crowdDirectorType, "_instance");
        object enterVillagePhase = ParseEnum(context.storyPhaseType, "EnterVillage");
        InvokeInstance(context.storyManager, "ResetState", enterVillagePhase, false);

        InvokeInstance(context.director, "Update");

        Component ensuredCrowdDirector = UnityEngine.Object.FindFirstObjectByType(crowdDirectorType, FindObjectsInactive.Include) as Component;
        Assert.That(ensuredCrowdDirector, Is.Not.Null, "Town 开场进到 EnterVillage 后，导演应自愈确保 SpringDay1NpcCrowdDirector 存在，否则 101~203 会只剩场景壳体、不会恢复 roam。");
    }

    [UnityTest]
    public IEnumerator TownVillageGatePreparation_ShouldAlignStoryActorsBeforeDialogueStarts()
    {
        EnsureTestSceneActive("Town");
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        GameObject townOpening = Track(new GameObject("进村围观"));
        GameObject startGroup = Track(new GameObject("起点"));
        GameObject endGroup = Track(new GameObject("终点"));
        startGroup.transform.SetParent(townOpening.transform, false);
        endGroup.transform.SetParent(townOpening.transform, false);

        GameObject chiefStart = Track(new GameObject("001起点"));
        chiefStart.transform.SetParent(startGroup.transform, false);
        chiefStart.transform.position = new Vector3(-11.60f, 15.26f, 0f);

        GameObject companionStart = Track(new GameObject("002起点"));
        companionStart.transform.SetParent(startGroup.transform, false);
        companionStart.transform.position = new Vector3(-10.40f, 14.88f, 0f);

        GameObject chiefPoint = Track(new GameObject("001终点"));
        chiefPoint.transform.SetParent(endGroup.transform, false);
        chiefPoint.transform.position = new Vector3(-12.55f, 14.52f, 0f);

        GameObject companionPoint = Track(new GameObject("002终点"));
        companionPoint.transform.SetParent(endGroup.transform, false);
        companionPoint.transform.position = new Vector3(-10.91f, 16.86f, 0f);

        GameObject chief = TrackNpcActorStub("001");
        GameObject companion = TrackNpcActorStub("002");
        chief.transform.position = new Vector3(8f, 8f, 0f);
        companion.transform.position = new Vector3(9f, 9f, 0f);

        bool prepared = (bool)InvokeInstance(context.director, "TryPrepareTownVillageGateActors", true);

        Assert.That(prepared, Is.True, "Town 开场对白前，导演应能把剧情 actor 拉到围观点位。");
        Assert.That(chief.transform.position.x, Is.EqualTo(-12.55f).Within(0.01f));
        Assert.That(chief.transform.position.y, Is.EqualTo(14.52f).Within(0.01f));
        Assert.That(companion.transform.position.x, Is.EqualTo(-10.91f).Within(0.01f));
        Assert.That(companion.transform.position.y, Is.EqualTo(16.86f).Within(0.01f));
    }

    [UnityTest]
    public IEnumerator TownVillageGatePreparation_ShouldFallbackToStageBookCueWhenSceneLayoutObjectsAreMissing()
    {
        EnsureTestSceneActive("Town");
        yield return null;

        GameObject chief = TrackNpcActorStub("001");
        GameObject companion = TrackNpcActorStub("002");
        chief.transform.position = new Vector3(8f, 8f, 0f);
        companion.transform.position = new Vector3(9f, 9f, 0f);

        Vector3 chiefTarget = ResolveTownVillageGateActorTarget("001", chief.transform);
        Vector3 companionTarget = ResolveTownVillageGateActorTarget("002", companion.transform);

        InvokeReframeStoryActor(chief.transform, chiefTarget);
        InvokeReframeStoryActor(companion.transform, companionTarget);

        Assert.That(chief.transform.position.x, Is.EqualTo(-12.55f).Within(0.01f));
        Assert.That(chief.transform.position.y, Is.EqualTo(14.52f).Within(0.01f));
        Assert.That(companion.transform.position.x, Is.EqualTo(-10.91f).Within(0.01f));
        Assert.That(companion.transform.position.y, Is.EqualTo(16.86f).Within(0.01f));
    }

    [UnityTest]
    public IEnumerator TownVillageGatePreparation_ShouldUseSceneEndMarkersWhenLegacyEndGroupIsMissing()
    {
        EnsureTestSceneActive("Town");
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        GameObject townOpening = Track(new GameObject("进村围观"));
        GameObject startGroup = Track(new GameObject("起点"));
        GameObject customGroup = Track(new GameObject("自定义层"));
        startGroup.transform.SetParent(townOpening.transform, false);
        customGroup.transform.SetParent(townOpening.transform, false);

        GameObject chiefStart = Track(new GameObject("001起点"));
        chiefStart.transform.SetParent(startGroup.transform, false);
        chiefStart.transform.position = new Vector3(-12f, 11.6f, 0f);

        GameObject companionStart = Track(new GameObject("002起点"));
        companionStart.transform.SetParent(startGroup.transform, false);
        companionStart.transform.position = new Vector3(-11.1f, 12.2f, 0f);

        GameObject chiefPoint = Track(new GameObject("001终点"));
        chiefPoint.transform.SetParent(customGroup.transform, false);
        chiefPoint.transform.position = new Vector3(-30.4f, 11.2f, 0f);

        GameObject companionPoint = Track(new GameObject("002终点"));
        companionPoint.transform.SetParent(customGroup.transform, false);
        companionPoint.transform.position = new Vector3(-28.9f, 12.7f, 0f);

        GameObject chief = TrackNpcActorStub("001");
        GameObject companion = TrackNpcActorStub("002");
        chief.transform.position = new Vector3(8f, 8f, 0f);
        companion.transform.position = new Vector3(9f, 9f, 0f);

        bool prepared = (bool)InvokeInstance(context.director, "TryPrepareTownVillageGateActors", true);

        Assert.That(prepared, Is.True, "只要 scene 里已有 001/002 的 authored 终点，导演就应直接吃这些点位，不应掉回 stagebook/hard fallback。");
        Assert.That(chief.transform.position.x, Is.EqualTo(-30.4f).Within(0.01f));
        Assert.That(chief.transform.position.y, Is.EqualTo(11.2f).Within(0.01f));
        Assert.That(companion.transform.position.x, Is.EqualTo(-28.9f).Within(0.01f));
        Assert.That(companion.transform.position.y, Is.EqualTo(12.7f).Within(0.01f));
    }

    [UnityTest]
    public IEnumerator TownVillageGatePreparation_ShouldIgnoreMissingThirdResidentMarkerBecause003RunsThroughCrowdRuntime()
    {
        EnsureTestSceneActive("Town");
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        GameObject townOpening = Track(new GameObject("进村围观"));
        GameObject startGroup = Track(new GameObject("起点"));
        GameObject customGroup = Track(new GameObject("自定义层"));
        startGroup.transform.SetParent(townOpening.transform, false);
        customGroup.transform.SetParent(townOpening.transform, false);

        GameObject chiefStart = Track(new GameObject("001起点"));
        chiefStart.transform.SetParent(startGroup.transform, false);
        chiefStart.transform.position = new Vector3(-12f, 11.6f, 0f);

        GameObject companionStart = Track(new GameObject("002起点"));
        companionStart.transform.SetParent(startGroup.transform, false);
        companionStart.transform.position = new Vector3(-11.1f, 12.2f, 0f);

        GameObject chiefPoint = Track(new GameObject("001终点"));
        chiefPoint.transform.SetParent(customGroup.transform, false);
        chiefPoint.transform.position = new Vector3(-30.4f, 11.2f, 0f);

        GameObject companionPoint = Track(new GameObject("002终点"));
        companionPoint.transform.SetParent(customGroup.transform, false);
        companionPoint.transform.position = new Vector3(-28.9f, 12.7f, 0f);

        TrackNpcActorStub("001");
        TrackNpcActorStub("002");
        GameObject thirdResident = TrackNpcActorStub("003");
        Vector3 thirdResidentStart = new Vector3(17f, 3f, 0f);
        thirdResident.transform.position = thirdResidentStart;

        bool prepared = (bool)InvokeInstance(context.director, "TryPrepareTownVillageGateActors", true);

        Assert.That(prepared, Is.True, "003 已不再走 opening 的导演私链；缺少 003 marker 不应再阻塞 001/002 的 village gate staged contract。");
        Assert.That(thirdResident.transform.position, Is.EqualTo(thirdResidentStart),
            "003 现在应由 crowd runtime 处理；导演 opening 准备链不应再私自改写它的位置。");
    }

    [UnityTest]
    public IEnumerator TownVillageGateDialogueActive_ShouldKeepStoryActorsAligned()
    {
        EnsureTestSceneActive("Town");
        RuntimeContext context = CreateRuntimeContext();
        Type dialogueManagerType = ResolveTypeOrFail("Sunset.Story.DialogueManager");
        Component dialogueManager = Track(new GameObject("DialogueManager_TownVillageGateAlign")).AddComponent(dialogueManagerType);
        SetStaticField(dialogueManagerType, "<Instance>k__BackingField", dialogueManager);
        yield return null;

        GameObject townOpening = Track(new GameObject("进村围观"));
        GameObject startGroup = Track(new GameObject("起点"));
        GameObject endGroup = Track(new GameObject("终点"));
        startGroup.transform.SetParent(townOpening.transform, false);
        endGroup.transform.SetParent(townOpening.transform, false);

        GameObject chiefStart = Track(new GameObject("001起点"));
        chiefStart.transform.SetParent(startGroup.transform, false);
        chiefStart.transform.position = new Vector3(-11.60f, 15.26f, 0f);

        GameObject companionStart = Track(new GameObject("002起点"));
        companionStart.transform.SetParent(startGroup.transform, false);
        companionStart.transform.position = new Vector3(-10.40f, 14.88f, 0f);

        GameObject chiefPoint = Track(new GameObject("001终点"));
        chiefPoint.transform.SetParent(endGroup.transform, false);
        chiefPoint.transform.position = new Vector3(-12.55f, 14.52f, 0f);

        GameObject companionPoint = Track(new GameObject("002终点"));
        companionPoint.transform.SetParent(endGroup.transform, false);
        companionPoint.transform.position = new Vector3(-10.91f, 16.86f, 0f);

        GameObject chief = TrackNpcActorStub("001");
        GameObject companion = TrackNpcActorStub("002");
        chief.transform.position = new Vector3(6f, 6f, 0f);
        companion.transform.position = new Vector3(7f, 7f, 0f);

        ScriptableObject sequence = ScriptableObject.CreateInstance(ResolveTypeOrFail("Sunset.Story.DialogueSequenceSO")) as ScriptableObject;
        SetWritableMember(sequence, "sequenceId", "spring-day1-village-gate");
        SetPrivateField(dialogueManager, "_currentSequence", sequence);
        SetPrivateField(dialogueManager, "<IsDialogueActive>k__BackingField", true);

        object enterVillagePhase = ParseEnum(context.storyPhaseType, "EnterVillage");
        InvokeInstance(context.storyManager, "ResetState", enterVillagePhase, false);

        InvokeInstance(context.director, "TickTownSceneFlow");

        Assert.That(chief.transform.position.x, Is.EqualTo(-12.55f).Within(0.01f));
        Assert.That(chief.transform.position.y, Is.EqualTo(14.52f).Within(0.01f));
        Assert.That(companion.transform.position.x, Is.EqualTo(-10.91f).Within(0.01f));
        Assert.That(companion.transform.position.y, Is.EqualTo(16.86f).Within(0.01f));
    }

    [UnityTest]
    public IEnumerator EnterVillageCrowdCue_ShouldStayActiveUntilHouseLeadActuallyStarts()
    {
        EnsureTestSceneActive("Town");
        RuntimeContext context = CreateRuntimeContext();
        yield return null;

        object enterVillagePhase = ParseEnum(context.storyPhaseType, "EnterVillage");
        InvokeInstance(context.storyManager, "ResetState", enterVillagePhase, false);

        Type crowdDirectorType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdDirector");
        MethodInfo suppressMethod = crowdDirectorType.GetMethod(
            "ShouldSuppressEnterVillageCrowdCueForTownHouseLead",
            BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(suppressMethod, Is.Not.Null, "围观 cue 的释放条件必须可被回归测试直接钉住。");

        bool suppressedBeforeLead = (bool)suppressMethod.Invoke(null, new object[] { "EnterVillage_PostEntry" });
        Assert.That(suppressedBeforeLead, Is.False, "VillageGate 正在准备/播放时，EnterVillage crowd cue 不能提前被放掉，否则 ordinary resident 会直接跳终点。");

        SetPrivateField(context.director, "_townHouseLeadStarted", true);
        bool suppressedAfterLead = (bool)suppressMethod.Invoke(null, new object[] { "EnterVillage_PostEntry" });
        Assert.That(suppressedAfterLead, Is.True, "只有 house lead 真正开始后，EnterVillage crowd cue 才应释放。");
    }

    [UnityTest]
    public IEnumerator VillageGateCompletionInTown_ShouldPromoteChiefLeadState()
    {
        EnsureTestSceneActive("Town");
        RuntimeContext context = CreateRuntimeContext();
        Type dialogueManagerType = ResolveTypeOrFail("Sunset.Story.DialogueManager");
        Component dialogueManager = Track(new GameObject("DialogueManager_Test")).AddComponent(dialogueManagerType);
        SetStaticField(dialogueManagerType, "<Instance>k__BackingField", dialogueManager);
        yield return null;

        TrackNpcActorStub("001");
        GameObject triggerObject = Track(new GameObject("SceneTransitionTrigger"));
        triggerObject.transform.position = new Vector3(20f, 20f, 0f);
        BoxCollider2D triggerCollider = triggerObject.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        Type transitionTriggerType = ResolveTypeOrFail("Sunset.Story.SceneTransitionTrigger2D");
        Component transitionTrigger = triggerObject.AddComponent(transitionTriggerType);
        InvokeInstance(transitionTrigger, "SetTargetScene", "Primary", "Assets/000_Scenes/Primary.unity", string.Empty);

        object enterVillagePhase = ParseEnum(context.storyPhaseType, "EnterVillage");
        InvokeInstance(context.storyManager, "ResetState", enterVillagePhase, false);
        AddCompletedSequenceId(dialogueManager, "spring-day1-village-gate");

        Type completedEventType = ResolveTypeOrFail("Sunset.Events.DialogueSequenceCompletedEvent");
        object completedEvent = Activator.CreateInstance(completedEventType);
        SetWritableMember(completedEvent, "SequenceId", "spring-day1-village-gate");
        SetWritableMember(completedEvent, "FollowupSequence", null);
        SetWritableMember(completedEvent, "CurrentPhase", enterVillagePhase);

        InvokeInstance(context.director, "HandleDialogueSequenceCompleted", completedEvent);

        Assert.That((bool)InvokeInstance(context.director, "IsTownHouseLeadPending"), Is.True, "Town 围观收束后，导演应进入村长带路态。");
        StringAssert.Contains("chief=001", (string)InvokeInstance(context.director, "GetTownHouseLeadSummary"));
        StringAssert.Contains("target=SceneTransitionTrigger", (string)InvokeInstance(context.director, "GetTownHouseLeadSummary"));
        Assert.That((string)InvokeInstance(context.director, "GetCurrentProgressLabel"), Is.EqualTo("村口围观已过，村长正带着你往住处走"));
    }

    [UnityTest]
    public IEnumerator VillageGateWhileActiveInTown_ShouldKeepPostEntryBeatUntilDialogueCompletes()
    {
        EnsureTestSceneActive("Town");
        RuntimeContext context = CreateRuntimeContext();
        Type dialogueManagerType = ResolveTypeOrFail("Sunset.Story.DialogueManager");
        Component dialogueManager = Track(new GameObject("DialogueManager_Test")).AddComponent(dialogueManagerType);
        SetStaticField(dialogueManagerType, "<Instance>k__BackingField", dialogueManager);
        yield return null;

        object enterVillagePhase = ParseEnum(context.storyPhaseType, "EnterVillage");
        InvokeInstance(context.storyManager, "ResetState", enterVillagePhase, false);
        SetPrivateField(context.director, "_villageGateSequencePlayed", true);

        Type sequenceType = ResolveTypeOrFail("Sunset.Story.DialogueSequenceSO");
        Type dialogueNodeType = ResolveTypeOrFail("Sunset.Story.DialogueNode");
        ScriptableObject sequence = ScriptableObject.CreateInstance(sequenceType) as ScriptableObject;
        Assert.That(sequence, Is.Not.Null, "未能创建测试对话序列。");
        object dialogueNode = Activator.CreateInstance(dialogueNodeType);
        Assert.That(dialogueNode, Is.Not.Null, "未能创建测试对白节点。");
        SetWritableMember(dialogueNode, "speakerName", "村民");
        SetWritableMember(dialogueNode, "text", "围观还在继续");
        IList nodes = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(dialogueNodeType));
        nodes.Add(dialogueNode);
        SetWritableMember(sequence, "sequenceId", "spring-day1-village-gate");
        SetWritableMember(sequence, "nodes", nodes);
        SetWritableMember(sequence, "defaultTypingSpeed", 0f);
        InvokeInstance(dialogueManager, "PlayDialogue", sequence);
        yield return null;

        Assert.That((string)InvokeInstance(context.director, "GetCurrentBeatKey"), Is.EqualTo("EnterVillage_PostEntry"), "VillageGate 对话还没播完时，导演仍应继续消费围观 beat。");
        Assert.That((bool)InvokeInstance(context.director, "IsTownHouseLeadPending"), Is.False, "VillageGate 还在播时，不应提前进入村长带路态。");
    }

    private RuntimeContext CreateRuntimeContext()
    {
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

        return new RuntimeContext(director, storyManager, timeManager, energySystem, healthSystem, storyPhaseType);
    }

    private GameObject Track(GameObject gameObject)
    {
        createdObjects.Add(gameObject);
        return gameObject;
    }

    private GameObject TrackNpcActorStub(string name)
    {
        GameObject actor = Track(new GameObject(name));
        actor.AddComponent(ResolveTypeOrFail("Sunset.Story.NPCDialogueInteractable"));
        return actor;
    }

    private static object ParseEnum(Type enumType, string name)
    {
        return Enum.Parse(enumType, name);
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

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private static void AddCompletedSequenceId(object target, string sequenceId)
    {
        FieldInfo field = target.GetType().GetField("_completedSequenceIds", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}._completedSequenceIds");
        if (field.GetValue(target) is HashSet<string> ids && !string.IsNullOrWhiteSpace(sequenceId))
        {
            ids.Add(sequenceId);
        }
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

    private static object InvokeInstance(object target, string methodName, params object[] args)
    {
        MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo method = null;
        object[] finalArgs = null;
        int providedCount = args != null ? args.Length : 0;

        for (int methodIndex = 0; methodIndex < methods.Length; methodIndex++)
        {
            MethodInfo candidate = methods[methodIndex];
            if (!string.Equals(candidate.Name, methodName, StringComparison.Ordinal))
            {
                continue;
            }

            ParameterInfo[] parameters = candidate.GetParameters();
            if (providedCount > parameters.Length)
            {
                continue;
            }

            object[] candidateArgs = new object[parameters.Length];
            bool matches = true;
            for (int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                if (parameterIndex < providedCount)
                {
                    object argument = args[parameterIndex];
                    Type parameterType = parameters[parameterIndex].ParameterType;
                    if (argument == null)
                    {
                        if (parameterType.IsValueType && Nullable.GetUnderlyingType(parameterType) == null)
                        {
                            matches = false;
                            break;
                        }

                        candidateArgs[parameterIndex] = null;
                        continue;
                    }

                    if (!parameterType.IsAssignableFrom(argument.GetType()))
                    {
                        matches = false;
                        break;
                    }

                    candidateArgs[parameterIndex] = argument;
                    continue;
                }

                if (!parameters[parameterIndex].IsOptional)
                {
                    matches = false;
                    break;
                }

                candidateArgs[parameterIndex] = Type.Missing;
            }

            if (!matches)
            {
                continue;
            }

            method = candidate;
            finalArgs = candidateArgs;
            break;
        }

        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return method.Invoke(target, finalArgs);
    }

    private static Vector3 ResolveTownVillageGateActorTarget(string npcId, Transform actor)
    {
        Type directorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        MethodInfo method = directorType.GetMethod("TryResolveTownVillageGateActorTarget", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, "SpringDay1Director 应提供 Town VillageGate actor fallback 解析。");

        object[] args = { npcId, actor, null, null };
        object result = method.Invoke(null, args);
        Assert.That(result, Is.EqualTo(true), $"应能为 {npcId} 解出 Town VillageGate fallback 点位。");
        return (Vector3)args[3];
    }

    private static void InvokeReframeStoryActor(Transform actor, Vector3 targetPosition)
    {
        Type directorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        MethodInfo method = directorType.GetMethod("ReframeStoryActor", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, "SpringDay1Director 应保留 story actor 立即就位入口。");
        method.Invoke(null, new object[] { actor, targetPosition, Vector2.zero });
    }

    private static object GetPropertyValue(object target, string propertyName)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到属性: {target.GetType().Name}.{propertyName}");
        return property.GetValue(target);
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
        EnsureTestSceneActive("Primary");
    }

    private void EnsureTestSceneActive(string sceneName)
    {
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            return;
        }

        string tempSceneDirectory = Path.Combine("Temp", "CodexEditModeScenes");
        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), tempSceneDirectory));
        temporaryPrimaryScenePath = Path.Combine(tempSceneDirectory, $"{sceneName}.unity").Replace("\\", "/");

        Scene tempScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        bool saved = EditorSceneManager.SaveScene(tempScene, temporaryPrimaryScenePath, saveAsCopy: false);
        Assert.That(saved, Is.True, $"应能创建用于 opening bridge 验证的临时 {sceneName} 场景。");
        Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(sceneName), $"opening bridge 验证应在名为 {sceneName} 的场景上下文内执行。");
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
        public RuntimeContext(
            Component director,
            Component storyManager,
            Component timeManager,
            Component energySystem,
            Component healthSystem,
            Type storyPhaseType)
        {
            this.director = director;
            this.storyManager = storyManager;
            this.timeManager = timeManager;
            this.energySystem = energySystem;
            this.healthSystem = healthSystem;
            this.storyPhaseType = storyPhaseType;
        }

        public Component director { get; }
        public Component storyManager { get; }
        public Component timeManager { get; }
        public Component energySystem { get; }
        public Component healthSystem { get; }
        public Type storyPhaseType { get; }
    }
}
