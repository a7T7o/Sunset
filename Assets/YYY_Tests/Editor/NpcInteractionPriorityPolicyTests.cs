using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UObject = UnityEngine.Object;

[TestFixture]
public class NpcInteractionPriorityPolicyTests
{
    private static readonly string[] PhaseNames =
    {
        "None",
        "CrashAndMeet",
        "EnterVillage",
        "HealingAndHP",
        "WorkbenchFlashback",
        "FarmingTutorial",
        "DinnerConflict",
        "ReturnAndReminder",
        "FreeTime",
        "DayEnd"
    };

    private static readonly bool[] ExpectedSuppression =
    {
        false,
        true,
        true,
        true,
        true,
        true,
        true,
        true,
        false,
        false
    };

    private GameObject storyObject;
    private GameObject playerObject;
    private GameObject npcObject;
    private GameObject dialogueObject;

    [TearDown]
    public void TearDown()
    {
        Type policyType = ResolveTypeOrFail("Sunset.Story.NpcInteractionPriorityPolicy");
        InvokeStatic<object>(policyType, "SetEditorValidationPhaseOverride", (object)null);
        SetPrivateStaticField(ResolveTypeOrFail("Sunset.Story.StoryManager"), "_instance", null);
        SetPrivateStaticField(ResolveTypeOrFail("Sunset.Story.SpringDay1Director"), "_instance", null);
        SetStaticProperty(ResolveTypeOrFail("Sunset.Story.DialogueManager"), "Instance", null);

        if (npcObject != null)
        {
            UObject.DestroyImmediate(npcObject);
        }

        if (playerObject != null)
        {
            UObject.DestroyImmediate(playerObject);
        }

        if (storyObject != null)
        {
            UObject.DestroyImmediate(storyObject);
        }

        if (dialogueObject != null)
        {
            UObject.DestroyImmediate(dialogueObject);
        }
    }

    public static IEnumerable<TestCaseData> StoryPhaseSuppressionCases()
    {
        for (int index = 0; index < PhaseNames.Length; index++)
        {
            yield return new TestCaseData(PhaseNames[index], ExpectedSuppression[index]);
        }
    }

    [TestCaseSource(nameof(StoryPhaseSuppressionCases))]
    public void StoryPhase_ShouldDecideWhetherFormalSuppressesCasual(string phaseName, bool shouldSuppress)
    {
        Type policyType = ResolveTypeOrFail("Sunset.Story.NpcInteractionPriorityPolicy");
        object phase = ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), phaseName);

        Assert.That(
            InvokeStatic<bool>(policyType, "ShouldSuppressInformalInteraction", phase),
            Is.EqualTo(shouldSuppress),
            $"闲聊阶段门禁判断异常：{phaseName}");
    }

    [Test]
    public void InformalChatInteractable_ShouldRemainAvailableDuringFormalPriorityPhase_WhenSameNpcHasNoFormalDialogue()
    {
        CreateStoryManager("CrashAndMeet");
        Component interactable = CreateInformalInteractable();
        object context = CreateContext();

        Assert.That(InvokeInstance<bool>(interactable, "CanInteract", context), Is.True);
    }

    [Test]
    public void InformalChatInteractable_ShouldYieldOnlyWhenSameNpcFormalDialogueCanTakeOver()
    {
        Component storyManager = CreateStoryManager("CrashAndMeet");
        CreateDialogueManager();
        Component interactable = CreateInformalInteractable(withFormalDialogueCandidate: true);
        object context = CreateContext();

        Assert.That(InvokeInstance<bool>(interactable, "CanInteract", context), Is.False);

        InvokeInstance<object>(
            storyManager,
            "ResetState",
            ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), "FreeTime"),
            false);

        Assert.That(InvokeInstance<bool>(interactable, "CanInteract", context), Is.True);
    }

    [Test]
    public void InformalChatInteractable_ShouldRecoverAfterFormalDialogueWasConsumed()
    {
        CreateStoryManager("CrashAndMeet");
        Component dialogueManager = CreateDialogueManager();
        Component interactable = CreateInformalInteractable(withFormalDialogueCandidate: true);
        object context = CreateContext();

        Component dialogueInteractable = npcObject.GetComponent(ResolveTypeOrFail("Sunset.Story.NPCDialogueInteractable"));
        SetPrivateField(dialogueInteractable, "followupSequence", CreateDialogueSequence("test-formal-followup"));
        MarkSequenceCompleted(dialogueManager, "test-formal-sequence");

        Assert.That(
            InvokeInstance<bool>(dialogueInteractable, "CanInteract", context),
            Is.False,
            "正式剧情一旦消费过，就不应再让 NPCDialogueInteractable 重播 formal / followup。");
        Assert.That(
            InvokeInstance<object>(dialogueInteractable, "GetFormalDialogueStateForCurrentStory").ToString(),
            Is.EqualTo("Consumed"),
            "formal 被消费后，公开只读状态也应明确落到 Consumed。");
        Assert.That(
            InvokeInstance<bool>(dialogueInteractable, "HasConsumedFormalDialogueForCurrentStory"),
            Is.True,
            "formal 被消费后，NPCDialogueInteractable 应能直接报出已消费状态。");
        Assert.That(
            InvokeInstance<bool>(dialogueInteractable, "WillYieldToInformalResident"),
            Is.True,
            "formal 被消费后，NPCDialogueInteractable 应明确让位给 informal / resident。");
        Assert.That(
            InvokeInstance<bool>(interactable, "CanInteract", context),
            Is.True,
            "formal 消费后，同 NPC 应回落到 informal / resident 闲聊入口。");
    }

    [Test]
    public void InformalChatInteractable_ShouldPreferPhaseSpecificResidentBundle_WhenCurrentStoryPhaseMatches()
    {
        CreateStoryManager("DinnerConflict");
        Component interactable = CreateInformalInteractable();

        Type relationshipStageType = ResolveTypeOrFail("NPCRelationshipStage");
        object strangerStage = ParseEnum(relationshipStageType, "Stranger");
        object dinnerPhase = ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), "DinnerConflict");

        InjectPhaseConversationBundle("DinnerConflict", "phase-dinner-resident", "晚饭后我们继续刚才那口气。", "桌边那阵安静我还没放下。");

        Array bundles = InvokeInstance<Array>(interactable, "ResolveConversationBundles", strangerStage, dinnerPhase);
        Assert.That(bundles, Is.Not.Null);
        Assert.That(bundles.Length, Is.EqualTo(1), "当前 phase 有专属 resident bundle 时，应优先只回这一包。");

        object resolvedBundle = bundles.GetValue(0);
        Assert.That(GetPrivateField<string>(resolvedBundle, "bundleId"), Is.EqualTo("phase-dinner-resident"));
    }

    [Test]
    public void NearbyFeedback_ShouldPreferPhaseSpecificResidentLines_WhenCurrentStoryPhaseMatches()
    {
        CreateStoryManager("DayEnd");
        CreateInformalInteractable();

        Type relationshipStageType = ResolveTypeOrFail("NPCRelationshipStage");
        object strangerStage = ParseEnum(relationshipStageType, "Stranger");
        object dayEndPhase = ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), "DayEnd");

        InjectPhaseNearbyLines("DayEnd", "夜里收口后先别把脚步踩乱。", "这会儿整村都还压着声音。");

        Component roamController = npcObject.GetComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        ScriptableObject roamProfile = GetPrivateField<ScriptableObject>(roamController, "roamProfile");
        Array lines = InvokeInstance<Array>(roamProfile, "GetPlayerNearbyLines", strangerStage, dayEndPhase);

        Assert.That(lines, Is.Not.Null);
        Assert.That(lines.Length, Is.EqualTo(2), "当前 phase 有专属 nearby resident lines 时，应优先回这一组。");
        Assert.That(lines.GetValue(0), Is.EqualTo("夜里收口后先别把脚步踩乱。"));
        Assert.That(lines.GetValue(1), Is.EqualTo("这会儿整村都还压着声音。"));
    }

    [Test]
    public void InformalChatInteractable_ShouldRecoverAfterFormalPhaseAdvanceWasAlreadyReached()
    {
        CreateStoryManager("EnterVillage");
        CreateDialogueManager();
        Component interactable = CreateInformalInteractable(withFormalDialogueCandidate: true);
        object context = CreateContext();

        Component dialogueInteractable = npcObject.GetComponent(ResolveTypeOrFail("Sunset.Story.NPCDialogueInteractable"));
        ScriptableObject sequence = GetPrivateField<ScriptableObject>(dialogueInteractable, "initialSequence");
        SetPublicField(sequence, "advanceStoryPhaseOnComplete", true);
        SetPublicField(sequence, "nextStoryPhase", ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), "EnterVillage"));

        Assert.That(
            InvokeInstance<bool>(dialogueInteractable, "CanInteract", context),
            Is.False,
            "正式剧情对应阶段已经抵达后，不应再次回放 formal 推进。");
        Assert.That(
            InvokeInstance<bool>(interactable, "CanInteract", context),
            Is.True,
            "正式推进已消费后，再次交互应只回落到 informal / resident 闲聊入口。");
    }

    [Test]
    public void InformalChatInteractable_ShouldKeepCurrentConversationAliveEvenInFormalPhase()
    {
        CreateStoryManager("CrashAndMeet");
        Component interactable = CreateInformalInteractable();
        object context = CreateContext();

        Component sessionService = playerObject.AddComponent(ResolveTypeOrFail("PlayerNpcChatSessionService"));
        SetPrivateField(sessionService, "_activeInteractable", interactable);
        SetPrivateField(sessionService, "_state", ParseNestedEnum(sessionService.GetType(), "SessionState", "PlayerTyping"));

        Assert.That(InvokeInstance<bool>(interactable, "CanInteract", context), Is.True);
    }

    [Test]
    public void InformalChatInteractable_ShouldRemainAvailableDuringFreeTimeNightReturnNavigation()
    {
        CreateStoryManager("FreeTime");
        Component interactable = CreateInformalInteractable();
        object context = CreateContext();

        Type roamType = ResolveTypeOrFail("NPCAutoRoamController");
        Component roamController = npcObject.GetComponent(roamType);
        SetPrivateField(roamController, "residentScriptedControlOwners", new List<string> { "SpringDay1NpcCrowdDirector" });
        SetPrivateField(roamController, "residentScriptedControlOwnerKey", "SpringDay1NpcCrowdDirector");
        SetPrivateField(roamController, "debugMoveActive", true);
        SetPrivateField(roamController, "state", ParseNestedEnum(roamType, "RoamState", "Moving"));
        SetPrivateField(
            roamController,
            "activePointToPointTravelContract",
            ParseNestedEnum(roamType, "PointToPointTravelContract", "FormalNavigation"));

        Assert.That(
            InvokeInstance<bool>(interactable, "CanInteract", context),
            Is.True,
            "20:00 后的 free-time 回家导航不应把 NPC 闲聊整体禁掉；玩家仍应能开口。");
    }

    [Test]
    public void FarmingTutorialExploreWindow_ShouldStopSuppressingResidentInformalInteraction()
    {
        CreateStoryManager("FarmingTutorial");
        Type directorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Component director = storyObject.AddComponent(directorType);
        SetPrivateStaticField(directorType, "_instance", director);
        SetPrivateField(director, "_postTutorialExploreWindowEntered", true);

        Type policyType = ResolveTypeOrFail("Sunset.Story.NpcInteractionPriorityPolicy");
        object phase = ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), "FarmingTutorial");

        Assert.That(
            InvokeStatic<bool>(policyType, "ShouldSuppressInformalInteraction", phase),
            Is.False,
            "0.0.6 的傍晚自由活动虽然还挂在 FarmingTutorial 相位里，但交互语义已经不该继续按 formal-priority 整体压闸。");
    }

    [Test]
    public void InformalChatInteractable_ShouldRemainAvailableDuringPostTutorialExploreWindowReturnNavigation()
    {
        CreateStoryManager("FarmingTutorial");
        Type directorType = ResolveTypeOrFail("Sunset.Story.SpringDay1Director");
        Component director = storyObject.AddComponent(directorType);
        SetPrivateStaticField(directorType, "_instance", director);
        SetPrivateField(director, "_postTutorialExploreWindowEntered", true);

        Component interactable = CreateInformalInteractable();
        object context = CreateContext();

        Type roamType = ResolveTypeOrFail("NPCAutoRoamController");
        Component roamController = npcObject.GetComponent(roamType);
        SetPrivateField(roamController, "residentScriptedControlOwners", new List<string> { "SpringDay1NpcCrowdDirector" });
        SetPrivateField(roamController, "residentScriptedControlOwnerKey", "SpringDay1NpcCrowdDirector");
        SetPrivateField(roamController, "debugMoveActive", true);
        SetPrivateField(roamController, "state", ParseNestedEnum(roamType, "RoamState", "Moving"));
        SetPrivateField(
            roamController,
            "activePointToPointTravelContract",
            ParseNestedEnum(roamType, "PointToPointTravelContract", "FormalNavigation"));

        Assert.That(
            InvokeInstance<bool>(interactable, "CanInteract", context),
            Is.True,
            "0.0.6 的 explore window 里，002/003 这类普通闲聊入口不应再被 scripted formal-navigation 一刀切掉。");
    }

    [Test]
    public void FormalDialogueInteractable_ShouldFollowResidentScriptedControlContract()
    {
        Component storyManager = CreateStoryManager("DinnerConflict");
        Component dialogueManager = CreateDialogueManager();
        CreateInformalInteractable(withFormalDialogueCandidate: true);
        object context = CreateContext();

        Type roamType = ResolveTypeOrFail("NPCAutoRoamController");
        Component roamController = npcObject.GetComponent(roamType);
        Component dialogueInteractable = npcObject.GetComponent(ResolveTypeOrFail("Sunset.Story.NPCDialogueInteractable"));

        SetPrivateField(roamController, "residentScriptedControlOwners", new List<string> { "spring-day1-director" });
        SetPrivateField(roamController, "residentScriptedControlOwnerKey", "spring-day1-director");

        Assert.That(
            InvokeInstance<bool>(dialogueInteractable, "CanInteract", context),
            Is.False,
            "剧情 owner 正在持有 NPC 身体时，formal 组件不该因为保持 enabled 就误放出剧情外对话。");

        InvokeInstance<object>(
            storyManager,
            "ResetState",
            ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), "FreeTime"),
            false);
        SetPrivateField(roamController, "residentScriptedControlOwners", new List<string> { "SpringDay1NpcCrowdDirector" });
        SetPrivateField(roamController, "residentScriptedControlOwnerKey", "SpringDay1NpcCrowdDirector");
        SetPrivateField(roamController, "debugMoveActive", true);
        SetPrivateField(roamController, "state", ParseNestedEnum(roamType, "RoamState", "Moving"));
        SetPrivateField(
            roamController,
            "activePointToPointTravelContract",
            ParseNestedEnum(roamType, "PointToPointTravelContract", "FormalNavigation"));

        Assert.That(
            InvokeInstance<bool>(dialogueInteractable, "CanInteract", context),
            Is.True,
            "20:00 回家 formal-navigation 期间，NPC 仍应保持正常可交互，而不是被 scripted-control 一刀切禁掉。");

        Assert.That(dialogueManager, Is.Not.Null);
    }

    [Test]
    public void EditorValidationPhaseOverride_ShouldAllowAmbientChecksToProbePairBubbleOutsideFormalPhase()
    {
        CreateStoryManager("CrashAndMeet");
        Type policyType = ResolveTypeOrFail("Sunset.Story.NpcInteractionPriorityPolicy");
        object freeTimePhase = ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), "FreeTime");

        InvokeStatic<object>(policyType, "SetEditorValidationPhaseOverride", freeTimePhase);

        Assert.That(
            InvokeStatic<bool>(policyType, "ShouldSuppressAmbientBubbleForCurrentStory"),
            Is.False,
            "编辑器 probe 临时覆写到 FreeTime 后，不应继续把 pair ambient 当成 formal 期压掉。");
    }

    [Test]
    public void AmbientBubble_ShouldStayAvailableDuringFormalPriorityPhase_WhenNoFormalTakeoverIsActive()
    {
        CreateStoryManager("CrashAndMeet");
        playerObject = new GameObject("AmbientPolicyPlayer");
        playerObject.AddComponent(ResolveTypeOrFail("PlayerMovement"));

        Type policyType = ResolveTypeOrFail("Sunset.Story.NpcInteractionPriorityPolicy");

        Assert.That(
            InvokeStatic<bool>(policyType, "ShouldSuppressAmbientBubbleForCurrentStory"),
            Is.False,
            "formal 阶段没有当前 formal 接管时，不应把 ambient 全局闷掉。");
    }

    private Component CreateStoryManager(string phaseName)
    {
        storyObject = new GameObject("StoryManagerTestHost");
        Component storyManager = storyObject.AddComponent(ResolveTypeOrFail("Sunset.Story.StoryManager"));
        SetPrivateStaticField(storyManager.GetType(), "_instance", storyManager);
        InvokeInstance<object>(
            storyManager,
            "ResetState",
            ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), phaseName),
            false);
        return storyManager;
    }

    private Component CreateInformalInteractable(bool withFormalDialogueCandidate = false)
    {
        npcObject = new GameObject("InformalNpc");

        Component roamController = npcObject.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        Component interactable = npcObject.AddComponent(ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable"));

        ScriptableObject roamProfile = ScriptableObject.CreateInstance(ResolveTypeOrFail("NPCRoamProfile"));
        Type contentProfileType = ResolveTypeOrFail("NPCDialogueContentProfile");
        ScriptableObject contentProfile = ScriptableObject.CreateInstance(contentProfileType);

        Type exchangeType = ResolveNestedTypeOrFail(contentProfileType, "InformalChatExchange");
        object exchange = Activator.CreateInstance(exchangeType);
        SetPrivateField(exchange, "playerLines", new[] { "玩家句" });
        SetPrivateField(exchange, "npcReplyLines", new[] { "NPC句" });

        Type bundleType = ResolveNestedTypeOrFail(contentProfileType, "InformalConversationBundle");
        object bundle = Activator.CreateInstance(bundleType);
        Array exchangeArray = Array.CreateInstance(exchangeType, 1);
        exchangeArray.SetValue(exchange, 0);
        SetPrivateField(bundle, "exchanges", exchangeArray);

        Array bundleArray = Array.CreateInstance(bundleType, 1);
        bundleArray.SetValue(bundle, 0);
        SetPrivateField(contentProfile, "defaultInformalConversationBundles", bundleArray);
        SetPrivateField(roamProfile, "dialogueContentProfile", contentProfile);
        SetPrivateField(roamController, "roamProfile", roamProfile);

        if (withFormalDialogueCandidate)
        {
            Component dialogueInteractable = npcObject.AddComponent(ResolveTypeOrFail("Sunset.Story.NPCDialogueInteractable"));
            ScriptableObject sequence = CreateDialogueSequence();
            SetPrivateField(dialogueInteractable, "initialSequence", sequence);
            SetPrivateField(interactable, "dialogueInteractable", dialogueInteractable);
        }

        return interactable;
    }

    private object CreateContext()
    {
        playerObject = new GameObject("PlayerContext");

        object context = Activator.CreateInstance(ResolveProjectTypeOrFail("InteractionContext"));
        SetPublicProperty(context, "PlayerTransform", playerObject.transform);
        SetPublicProperty(context, "PlayerPosition", (Vector2)playerObject.transform.position);
        return context;
    }

    private Component CreateDialogueManager()
    {
        dialogueObject = new GameObject("DialogueManagerTestHost");
        Component dialogueManager = dialogueObject.AddComponent(ResolveTypeOrFail("Sunset.Story.DialogueManager"));
        SetStaticProperty(dialogueManager.GetType(), "Instance", dialogueManager);
        return dialogueManager;
    }

    private static ScriptableObject CreateDialogueSequence(string sequenceId = "test-formal-sequence")
    {
        Type sequenceType = ResolveTypeOrFail("Sunset.Story.DialogueSequenceSO");
        Type nodeType = ResolveTypeOrFail("Sunset.Story.DialogueNode");

        ScriptableObject sequence = ScriptableObject.CreateInstance(sequenceType);
        SetPublicField(sequence, "sequenceId", sequenceId);

        object node = Activator.CreateInstance(nodeType);
        SetPublicField(node, "speakerName", "村长");
        SetPublicField(node, "text", "正式对白");

        IList nodeList = GetPublicField<IList>(sequence, "nodes");
        nodeList.Add(node);
        return sequence;
    }

    private static void MarkSequenceCompleted(Component dialogueManager, string sequenceId)
    {
        HashSet<string> completedSequences = GetPrivateField<HashSet<string>>(dialogueManager, "_completedSequenceIds");
        completedSequences.Add(sequenceId);
    }

    private void InjectPhaseConversationBundle(string phaseName, string bundleId, string playerLine, string npcReplyLine)
    {
        Component roamController = npcObject.GetComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        ScriptableObject roamProfile = GetPrivateField<ScriptableObject>(roamController, "roamProfile");
        ScriptableObject contentProfile = GetPrivateField<ScriptableObject>(roamProfile, "dialogueContentProfile");

        Type contentProfileType = contentProfile.GetType();
        Type exchangeType = ResolveNestedTypeOrFail(contentProfileType, "InformalChatExchange");
        object exchange = Activator.CreateInstance(exchangeType);
        SetPrivateField(exchange, "playerLines", new[] { playerLine });
        SetPrivateField(exchange, "npcReplyLines", new[] { npcReplyLine });

        Type bundleType = ResolveNestedTypeOrFail(contentProfileType, "InformalConversationBundle");
        object bundle = Activator.CreateInstance(bundleType);
        Array exchangeArray = Array.CreateInstance(exchangeType, 1);
        exchangeArray.SetValue(exchange, 0);
        SetPrivateField(bundle, "bundleId", bundleId);
        SetPrivateField(bundle, "exchanges", exchangeArray);

        Type phaseSetType = ResolveNestedTypeOrFail(contentProfileType, "PhaseInformalChatSet");
        object phaseSet = Activator.CreateInstance(phaseSetType);
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        Array bundleArray = Array.CreateInstance(bundleType, 1);
        bundleArray.SetValue(bundle, 0);
        SetPrivateField(phaseSet, "storyPhase", ParseEnum(storyPhaseType, phaseName));
        SetPrivateField(phaseSet, "conversationBundles", bundleArray);

        Array phaseSetArray = Array.CreateInstance(phaseSetType, 1);
        phaseSetArray.SetValue(phaseSet, 0);
        SetPrivateField(contentProfile, "phaseInformalChatSets", phaseSetArray);
    }

    private void InjectPhaseNearbyLines(string phaseName, params string[] nearbyLines)
    {
        Component roamController = npcObject.GetComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        ScriptableObject roamProfile = GetPrivateField<ScriptableObject>(roamController, "roamProfile");
        ScriptableObject contentProfile = GetPrivateField<ScriptableObject>(roamProfile, "dialogueContentProfile");

        Type contentProfileType = contentProfile.GetType();
        Type phaseSetType = ResolveNestedTypeOrFail(contentProfileType, "PhaseNearbySet");
        object phaseSet = Activator.CreateInstance(phaseSetType);
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");

        SetPrivateField(phaseSet, "storyPhase", ParseEnum(storyPhaseType, phaseName));
        SetPrivateField(phaseSet, "playerNearbyLines", nearbyLines);

        Array phaseSetArray = Array.CreateInstance(phaseSetType, 1);
        phaseSetArray.SetValue(phaseSet, 0);
        SetPrivateField(contentProfile, "phaseNearbyLines", phaseSetArray);
    }

    private static Type ResolveTypeOrFail(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type resolvedType = TryResolveType(assembly, typeName);
            if (resolvedType != null)
            {
                return resolvedType;
            }
        }

        Assert.Fail($"未找到类型: {typeName}");
        return null;
    }

    private static Type ResolveProjectTypeOrFail(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type resolvedType = TryResolveProjectType(assembly, typeName);
            if (resolvedType != null)
            {
                return resolvedType;
            }
        }

        Assert.Fail($"未找到项目类型: {typeName}");
        return null;
    }

    private static Type TryResolveType(Assembly assembly, string typeName)
    {
        Type direct = assembly.GetType(typeName, false);
        if (direct != null)
        {
            return direct;
        }

        try
        {
            return assembly
                .GetTypes()
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal) ||
                                        string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types
                .Where(type => type != null)
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal) ||
                                        string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
    }

    private static Type TryResolveProjectType(Assembly assembly, string typeName)
    {
        try
        {
            return assembly
                .GetTypes()
                .Where(type => type != null &&
                               !string.Equals(type.Namespace, "UnityEditor", StringComparison.Ordinal))
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal) ||
                                        string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types
                .Where(type => type != null &&
                               !string.Equals(type.Namespace, "UnityEditor", StringComparison.Ordinal))
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal) ||
                                        string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
    }

    private static Type ResolveNestedTypeOrFail(Type parentType, string nestedTypeName)
    {
        Type nestedType = parentType.GetNestedType(nestedTypeName, BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(nestedType, Is.Not.Null, $"未找到嵌套类型: {parentType.Name}.{nestedTypeName}");
        return nestedType;
    }

    private static object ParseEnum(Type enumType, string value)
    {
        Assert.That(enumType, Is.Not.Null, $"未找到枚举类型: {value}");
        return Enum.Parse(enumType, value);
    }

    private static object ParseNestedEnum(Type targetType, string nestedTypeName, string value)
    {
        Type nestedType = targetType.GetNestedType(nestedTypeName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(nestedType, Is.Not.Null, $"未找到嵌套类型: {targetType.Name}.{nestedTypeName}");
        return Enum.Parse(nestedType, value);
    }

    private static T InvokeStatic<T>(Type targetType, string methodName, params object[] args)
    {
        MethodInfo method = FindMethod(
            targetType,
            methodName,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
            args);
        Assert.That(method, Is.Not.Null, $"未找到静态方法: {targetType.Name}.{methodName}");
        return (T)method.Invoke(null, args);
    }

    private static T InvokeInstance<T>(object target, string methodName, params object[] args)
    {
        MethodInfo method = FindMethod(
            target.GetType(),
            methodName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            args);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return (T)method.Invoke(target, args);
    }

    private static MethodInfo FindMethod(Type targetType, string methodName, BindingFlags bindingFlags, object[] args)
    {
        MethodInfo[] candidates = targetType
            .GetMethods(bindingFlags)
            .Where(method => string.Equals(method.Name, methodName, StringComparison.Ordinal))
            .ToArray();

        for (int methodIndex = 0; methodIndex < candidates.Length; methodIndex++)
        {
            MethodInfo method = candidates[methodIndex];
            ParameterInfo[] parameters = method.GetParameters();
            if (!ParametersMatch(parameters, args))
            {
                continue;
            }

            return method;
        }

        return null;
    }

    private static bool ParametersMatch(ParameterInfo[] parameters, object[] args)
    {
        object[] effectiveArgs = args ?? Array.Empty<object>();
        if (parameters.Length != effectiveArgs.Length)
        {
            return false;
        }

        for (int index = 0; index < parameters.Length; index++)
        {
            object argument = effectiveArgs[index];
            Type parameterType = parameters[index].ParameterType;
            if (argument == null)
            {
                if (parameterType.IsValueType && Nullable.GetUnderlyingType(parameterType) == null)
                {
                    return false;
                }

                continue;
            }

            if (!parameterType.IsInstanceOfType(argument))
            {
                return false;
            }
        }

        return true;
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

    private static void SetPrivateStaticField(Type targetType, string fieldName, object value)
    {
        FieldInfo field = targetType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到静态字段: {targetType.Name}.{fieldName}");
        field.SetValue(null, value);
    }

    private static T GetPublicField<T>(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(field, Is.Not.Null, $"未找到公开字段: {target.GetType().Name}.{fieldName}");
        return (T)field.GetValue(target);
    }

    private static void SetPublicField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(field, Is.Not.Null, $"未找到公开字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private static void SetPublicProperty(object target, string propertyName, object value)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到属性: {target.GetType().Name}.{propertyName}");
        property.SetValue(target, value);
    }

    private static void SetStaticProperty(Type targetType, string propertyName, object value)
    {
        PropertyInfo property = targetType.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到静态属性: {targetType.Name}.{propertyName}");
        property.SetValue(null, value);
    }
}
