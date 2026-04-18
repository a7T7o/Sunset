using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class NPCInformalChatInterruptMatrixTests
{
    [Test]
    public void GetInterruptReaction_ShouldPreferExactCauseAndPhaseRule()
    {
        Type dialogueContentType = ResolveTypeOrFail("NPCDialogueContentProfile");
        Type stageSetType = ResolveNestedTypeOrFail(dialogueContentType, "RelationshipStageInformalChatSet");
        Type ruleType = ResolveNestedTypeOrFail(dialogueContentType, "InformalChatInterruptRule");
        Type reactionType = ResolveNestedTypeOrFail(dialogueContentType, "InformalChatInterruptReaction");
        Type leaveCauseType = ResolveTypeOrFail("NPCInformalChatLeaveCause");
        Type leavePhaseType = ResolveTypeOrFail("NPCInformalChatLeavePhase");
        Type relationshipStageType = ResolveTypeOrFail("NPCRelationshipStage");

        ScriptableObject profile = ScriptableObject.CreateInstance(dialogueContentType);

        try
        {
            object stageSet = Activator.CreateInstance(stageSetType);
            SetPrivateField(stageSet, "relationshipStage", Enum.Parse(relationshipStageType, "Close"));
            SetPrivateField(
                stageSet,
                "walkAwayReaction",
                InvokeStatic(
                    reactionType,
                    "Create",
                    "legacy",
                    new[] { "旧离场句" },
                    new[] { "旧回应句" },
                    0,
                    0.45f));

            object exactRule = Activator.CreateInstance(ruleType);
            SetPrivateField(exactRule, "leaveCause", Enum.Parse(leaveCauseType, "DistanceGraceExceeded"));
            SetPrivateField(exactRule, "leavePhase", Enum.Parse(leavePhaseType, "PlayerSpeaking"));
            SetPrivateField(
                exactRule,
                "reaction",
                InvokeStatic(
                    reactionType,
                    "Create",
                    "exact",
                    new[] { "精确离场句" },
                    new[] { "精确回应句" },
                    0,
                    0.45f));

            Array ruleArray = Array.CreateInstance(ruleType, 1);
            ruleArray.SetValue(exactRule, 0);
            SetPrivateField(stageSet, "interruptRules", ruleArray);

            Array stageSetArray = Array.CreateInstance(stageSetType, 1);
            stageSetArray.SetValue(stageSet, 0);
            SetPrivateField(profile, "relationshipStageInformalChatSets", stageSetArray);

            object reaction = InvokeInstance(
                profile,
                "GetInterruptReaction",
                Enum.Parse(relationshipStageType, "Close"),
                Enum.Parse(leaveCauseType, "DistanceGraceExceeded"),
                Enum.Parse(leavePhaseType, "PlayerSpeaking"));

            Assert.That(GetFieldOrProperty(reaction, "ReactionCue"), Is.EqualTo("exact"));
            string[] playerExitLines = (string[])GetFieldOrProperty(reaction, "PlayerExitLines");
            Assert.That(playerExitLines[0], Is.EqualTo("精确离场句"));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(profile);
        }
    }

    [Test]
    public void GetInterruptReaction_ShouldFallbackToLegacyWalkAway_ForDistanceCause()
    {
        Type dialogueContentType = ResolveTypeOrFail("NPCDialogueContentProfile");
        Type stageSetType = ResolveNestedTypeOrFail(dialogueContentType, "RelationshipStageInformalChatSet");
        Type reactionType = ResolveNestedTypeOrFail(dialogueContentType, "InformalChatInterruptReaction");
        Type leaveCauseType = ResolveTypeOrFail("NPCInformalChatLeaveCause");
        Type leavePhaseType = ResolveTypeOrFail("NPCInformalChatLeavePhase");
        Type relationshipStageType = ResolveTypeOrFail("NPCRelationshipStage");

        ScriptableObject profile = ScriptableObject.CreateInstance(dialogueContentType);

        try
        {
            object stageSet = Activator.CreateInstance(stageSetType);
            SetPrivateField(stageSet, "relationshipStage", Enum.Parse(relationshipStageType, "Familiar"));
            SetPrivateField(
                stageSet,
                "walkAwayReaction",
                InvokeStatic(
                    reactionType,
                    "Create",
                    "legacy",
                    new[] { "老的离场句" },
                    new[] { "老的回应句" },
                    0,
                    0.45f));

            Array stageSetArray = Array.CreateInstance(stageSetType, 1);
            stageSetArray.SetValue(stageSet, 0);
            SetPrivateField(profile, "relationshipStageInformalChatSets", stageSetArray);

            object reaction = InvokeInstance(
                profile,
                "GetInterruptReaction",
                Enum.Parse(relationshipStageType, "Familiar"),
                Enum.Parse(leaveCauseType, "DistanceGraceExceeded"),
                Enum.Parse(leavePhaseType, "NpcThinking"));

            Assert.That(GetFieldOrProperty(reaction, "ReactionCue"), Is.EqualTo("legacy"));
            string[] npcReactionLines = (string[])GetFieldOrProperty(reaction, "NpcReactionLines");
            Assert.That(npcReactionLines[0], Is.EqualTo("老的回应句"));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(profile);
        }
    }

    [Test]
    public void RoamProfile_ShouldExposeInterruptReaction_FromDialogueContent()
    {
        Type dialogueContentType = ResolveTypeOrFail("NPCDialogueContentProfile");
        Type ruleType = ResolveNestedTypeOrFail(dialogueContentType, "InformalChatInterruptRule");
        Type reactionType = ResolveNestedTypeOrFail(dialogueContentType, "InformalChatInterruptReaction");
        Type roamProfileType = ResolveTypeOrFail("NPCRoamProfile");
        Type leaveCauseType = ResolveTypeOrFail("NPCInformalChatLeaveCause");
        Type leavePhaseType = ResolveTypeOrFail("NPCInformalChatLeavePhase");
        Type relationshipStageType = ResolveTypeOrFail("NPCRelationshipStage");

        ScriptableObject content = ScriptableObject.CreateInstance(dialogueContentType);
        ScriptableObject roamProfile = ScriptableObject.CreateInstance(roamProfileType);

        try
        {
            object defaultRule = Activator.CreateInstance(ruleType);
            SetPrivateField(defaultRule, "leaveCause", Enum.Parse(leaveCauseType, "BlockingUi"));
            SetPrivateField(defaultRule, "leavePhase", Enum.Parse(leavePhaseType, "Any"));
            SetPrivateField(
                defaultRule,
                "reaction",
                InvokeStatic(
                    reactionType,
                    "Create",
                    "busy",
                    new[] { "我先忙一下" },
                    new[] { "好，你先忙" },
                    0,
                    0.2f));

            Array ruleArray = Array.CreateInstance(ruleType, 1);
            ruleArray.SetValue(defaultRule, 0);
            SetPrivateField(content, "defaultInterruptRules", ruleArray);
            SetPrivateField(roamProfile, "dialogueContentProfile", content);

            object reaction = InvokeInstance(
                roamProfile,
                "GetInterruptReaction",
                Enum.Parse(relationshipStageType, "Stranger"),
                Enum.Parse(leaveCauseType, "BlockingUi"),
                Enum.Parse(leavePhaseType, "BetweenTurns"));

            Assert.That(GetFieldOrProperty(reaction, "ReactionCue"), Is.EqualTo("busy"));
            string[] playerExitLines = (string[])GetFieldOrProperty(reaction, "PlayerExitLines");
            Assert.That(playerExitLines[0], Is.EqualTo("我先忙一下"));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(content);
            UnityEngine.Object.DestroyImmediate(roamProfile);
        }
    }

    [Test]
    public void ResumeIndex_ShouldContinueFromCompletedExchangeCount_ForResumableInterrupt()
    {
        Type serviceType = ResolveTypeOrFail("PlayerNpcChatSessionService");
        Type leaveCauseType = ResolveTypeOrFail("NPCInformalChatLeaveCause");
        Type leavePhaseType = ResolveTypeOrFail("NPCInformalChatLeavePhase");

        object[] args =
        {
            Enum.Parse(leaveCauseType, "DistanceGraceExceeded"),
            Enum.Parse(leavePhaseType, "NpcThinking"),
            1,
            3,
            -1
        };

        bool result = (bool)InvokeStaticWithByRef(serviceType, "TryResolveResumeExchangeIndex", args);

        Assert.That(result, Is.True);
        Assert.That(args[4], Is.EqualTo(1));
    }

    [Test]
    public void ResumeIndex_ShouldRejectCompletionHoldInterrupt()
    {
        Type serviceType = ResolveTypeOrFail("PlayerNpcChatSessionService");
        Type leaveCauseType = ResolveTypeOrFail("NPCInformalChatLeaveCause");
        Type leavePhaseType = ResolveTypeOrFail("NPCInformalChatLeavePhase");

        object[] args =
        {
            Enum.Parse(leaveCauseType, "DistanceGraceExceeded"),
            Enum.Parse(leavePhaseType, "CompletionHold"),
            1,
            3,
            -1
        };

        bool result = (bool)InvokeStaticWithByRef(serviceType, "TryResolveResumeExchangeIndex", args);

        Assert.That(result, Is.False);
        Assert.That(args[4], Is.EqualTo(-1));
    }

    [Test]
    public void PendingResumeConsume_ShouldRejectDifferentNpcAndExpiredSnapshot()
    {
        Type serviceType = ResolveTypeOrFail("PlayerNpcChatSessionService");

        bool differentNpcResult = (bool)InvokeStatic(
            serviceType,
            "CanConsumePendingResumeSnapshot",
            "002",
            "003",
            12f,
            8f,
            1,
            3);

        bool expiredResult = (bool)InvokeStatic(
            serviceType,
            "CanConsumePendingResumeSnapshot",
            "002",
            "002",
            8f,
            12f,
            1,
            3);

        bool validResult = (bool)InvokeStatic(
            serviceType,
            "CanConsumePendingResumeSnapshot",
            "002",
            "002",
            12f,
            8f,
            1,
            3);

        Assert.That(differentNpcResult, Is.False);
        Assert.That(expiredResult, Is.False);
        Assert.That(validResult, Is.True);
    }

    [Test]
    public void ConversationTakeoverProbe_ShouldIgnoreInformalChatSelfOwner()
    {
        Type serviceType = ResolveTypeOrFail("PlayerNpcChatSessionService");
        Type roamType = ResolveTypeOrFail("NPCAutoRoamController");
        Type interactableType = ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable");

        GameObject player = new GameObject("Player_ConversationTakeoverProbe");
        GameObject npc = new GameObject("Npc_ConversationTakeoverProbe");
        try
        {
            Component service = player.AddComponent(serviceType);
            Component roamController = npc.AddComponent(roamType);
            Component interactable = npc.AddComponent(interactableType);
            SetPrivateField(interactable, "autoRoamController", roamController);
            SetPrivateField(service, "_activeInteractable", interactable);
            SetPrivateField(roamController, "residentScriptedControlOwners", new List<string> { "npc-informal-chat" });
            SetPrivateField(roamController, "residentScriptedControlOwnerKey", "npc-informal-chat");

            bool shouldCancel = (bool)InvokeInstance(service, "ShouldCancelForResidentScriptedTakeover");

            Assert.That(shouldCancel, Is.False,
                "闲聊自己占用 resident scripted control 时，不应再被 session service 当成系统 takeover 误杀。");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(npc);
            UnityEngine.Object.DestroyImmediate(player);
        }
    }

    [Test]
    public void ConversationTakeoverProbe_ShouldTreatStoryOwnerAsRealTakeover()
    {
        Type serviceType = ResolveTypeOrFail("PlayerNpcChatSessionService");
        Type roamType = ResolveTypeOrFail("NPCAutoRoamController");
        Type interactableType = ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable");

        GameObject player = new GameObject("Player_StoryTakeoverProbe");
        GameObject npc = new GameObject("Npc_StoryTakeoverProbe");
        try
        {
            Component service = player.AddComponent(serviceType);
            Component roamController = npc.AddComponent(roamType);
            Component interactable = npc.AddComponent(interactableType);
            SetPrivateField(interactable, "autoRoamController", roamController);
            SetPrivateField(service, "_activeInteractable", interactable);
            SetPrivateField(roamController, "residentScriptedControlOwners", new List<string> { "spring-day1-director" });
            SetPrivateField(roamController, "residentScriptedControlOwnerKey", "spring-day1-director");

            bool shouldCancel = (bool)InvokeInstance(service, "ShouldCancelForResidentScriptedTakeover");

            Assert.That(shouldCancel, Is.True,
                "真正的 Day1/director scripted owner 介入时，session service 仍应把它当 takeover 收掉闲聊。");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(npc);
            UnityEngine.Object.DestroyImmediate(player);
        }
    }

    [Test]
    public void FallbackDistanceReaction_ShouldBePhaseAware_WhenNpcSpeakingIsInterrupted()
    {
        Type serviceType = ResolveTypeOrFail("PlayerNpcChatSessionService");
        Type leaveCauseType = ResolveTypeOrFail("NPCInformalChatLeaveCause");
        Type leavePhaseType = ResolveTypeOrFail("NPCInformalChatLeavePhase");

        object reaction = InvokeStatic(
            serviceType,
            "CreateFallbackInterruptReaction",
            Enum.Parse(leaveCauseType, "DistanceGraceExceeded"),
            Enum.Parse(leavePhaseType, "NpcSpeaking"));

        Assert.That(GetFieldOrProperty(reaction, "ReactionCue"), Is.EqualTo("先打断一下"));
        string[] playerExitLines = (string[])GetFieldOrProperty(reaction, "PlayerExitLines");
        string[] npcReactionLines = (string[])GetFieldOrProperty(reaction, "NpcReactionLines");
        Assert.That(playerExitLines[0], Is.EqualTo("抱歉，先打断一下，我得先走开。"));
        Assert.That(npcReactionLines[0], Is.EqualTo("好吧，我先把话停在这里，等你回来。"));
    }

    [Test]
    public void ResumeIntroPlan_ShouldReturnContinuityLines_ForBlockingUiResume()
    {
        Type serviceType = ResolveTypeOrFail("PlayerNpcChatSessionService");
        Type leaveCauseType = ResolveTypeOrFail("NPCInformalChatLeaveCause");
        Type leavePhaseType = ResolveTypeOrFail("NPCInformalChatLeavePhase");

        object plan = InvokeStatic(
            serviceType,
            "CreateFallbackResumeIntroPlan",
            Enum.Parse(leaveCauseType, "BlockingUi"),
            Enum.Parse(leavePhaseType, "BetweenTurns"),
            1);

        Assert.IsNotNull(plan);
        Assert.That(GetFieldOrProperty(plan, "PlayerLine"), Is.EqualTo("刚才被别的事打断了，我们接着说。"));
        Assert.That(GetFieldOrProperty(plan, "NpcLine"), Is.EqualTo("好，我们就从刚才那儿接着往下说。"));
    }

    [Test]
    public void FallbackInterruptReaction_ShouldProvidePlayerSnapshot_ForTargetInvalid()
    {
        Type serviceType = ResolveTypeOrFail("PlayerNpcChatSessionService");
        Type leaveCauseType = ResolveTypeOrFail("NPCInformalChatLeaveCause");
        Type leavePhaseType = ResolveTypeOrFail("NPCInformalChatLeavePhase");

        object reaction = InvokeStatic(
            serviceType,
            "CreateFallbackInterruptReaction",
            Enum.Parse(leaveCauseType, "TargetInvalid"),
            Enum.Parse(leavePhaseType, "NpcThinking"));

        Assert.That(GetFieldOrProperty(reaction, "ReactionCue"), Is.EqualTo("人呢"));
        string[] playerExitLines = (string[])GetFieldOrProperty(reaction, "PlayerExitLines");
        string[] npcReactionLines = (string[])GetFieldOrProperty(reaction, "NpcReactionLines");
        Assert.That(playerExitLines[0], Is.EqualTo("欸，人怎么一下不见了？"));
        Assert.That(npcReactionLines.Length, Is.EqualTo(0));
    }

    [Test]
    public void ResumeIntroPlan_ShouldStayNull_ForDistanceResumeBeforeAnyCompletedExchange()
    {
        Type serviceType = ResolveTypeOrFail("PlayerNpcChatSessionService");
        Type leaveCauseType = ResolveTypeOrFail("NPCInformalChatLeaveCause");
        Type leavePhaseType = ResolveTypeOrFail("NPCInformalChatLeavePhase");

        object plan = InvokeStatic(
            serviceType,
            "CreateFallbackResumeIntroPlan",
            Enum.Parse(leaveCauseType, "DistanceGraceExceeded"),
            Enum.Parse(leavePhaseType, "PlayerSpeaking"),
            0);

        Assert.IsNull(plan);
    }

    [Test]
    public void ShouldSuppressResumeIntro_ShouldOnlyBlockSameNpcWithinCooldown()
    {
        Type serviceType = ResolveTypeOrFail("PlayerNpcChatSessionService");

        bool sameNpcDuringCooldown = (bool)InvokeStatic(
            serviceType,
            "ShouldSuppressResumeIntro",
            "002",
            "002",
            10f,
            6f);

        bool sameNpcAfterCooldown = (bool)InvokeStatic(
            serviceType,
            "ShouldSuppressResumeIntro",
            "002",
            "002",
            6f,
            10f);

        bool differentNpcDuringCooldown = (bool)InvokeStatic(
            serviceType,
            "ShouldSuppressResumeIntro",
            "002",
            "003",
            10f,
            6f);

        Assert.That(sameNpcDuringCooldown, Is.True);
        Assert.That(sameNpcAfterCooldown, Is.False);
        Assert.That(differentNpcDuringCooldown, Is.False);
    }

    [Test]
    public void ResolveMatchedResumeOutcome_ShouldDistinguishIntroAndSilentResume()
    {
        Type serviceType = ResolveTypeOrFail("PlayerNpcChatSessionService");

        object withIntro = InvokeStatic(
            serviceType,
            "ResolveMatchedResumeOutcome",
            true,
            false);

        object silent = InvokeStatic(
            serviceType,
            "ResolveMatchedResumeOutcome",
            false,
            false);

        object suppressed = InvokeStatic(
            serviceType,
            "ResolveMatchedResumeOutcome",
            true,
            true);

        Assert.That(withIntro.ToString(), Is.EqualTo("ResumedWithIntro"));
        Assert.That(silent.ToString(), Is.EqualTo("ResumedSilently"));
        Assert.That(suppressed.ToString(), Is.EqualTo("SuppressedByCooldown"));
    }

    [Test]
    public void ResolveUnavailablePendingResumeOutcome_ShouldDistinguishExpiredAndInvalidSnapshot()
    {
        Type serviceType = ResolveTypeOrFail("PlayerNpcChatSessionService");

        object expired = InvokeStatic(
            serviceType,
            "ResolveUnavailablePendingResumeOutcome",
            6f,
            10f);

        object invalid = InvokeStatic(
            serviceType,
            "ResolveUnavailablePendingResumeOutcome",
            -1f,
            10f);

        Assert.That(expired.ToString(), Is.EqualTo("Expired"));
        Assert.That(invalid.ToString(), Is.EqualTo("InvalidSnapshot"));
    }

    [Test]
    public void PendingResumePlan_ShouldReportDifferentNpc_WhenSnapshotBelongsToAnotherNpc()
    {
        Type serviceType = ResolveTypeOrFail("PlayerNpcChatSessionService");
        Type dialogueContentType = ResolveTypeOrFail("NPCDialogueContentProfile");
        Type bundleType = ResolveNestedTypeOrFail(dialogueContentType, "InformalConversationBundle");
        Type exchangeType = ResolveNestedTypeOrFail(dialogueContentType, "InformalChatExchange");

        GameObject host = new GameObject("ResumeOutcomeHost");
        MonoBehaviour service = (MonoBehaviour)host.AddComponent(serviceType);

        try
        {
            ScriptableObject profile = ScriptableObject.CreateInstance(dialogueContentType);
            try
            {
                object exchange = Activator.CreateInstance(exchangeType);
                SetPrivateField(exchange, "playerLines", new[] { "玩家句" });
                SetPrivateField(exchange, "npcReplyLines", new[] { "NPC句" });

                object bundle = Activator.CreateInstance(bundleType);
                Array exchangeArray = Array.CreateInstance(exchangeType, 1);
                exchangeArray.SetValue(exchange, 0);
                SetPrivateField(bundle, "exchanges", exchangeArray);

                SetPrivateField(service, "_pendingResumeBundle", bundle);
                SetPrivateField(service, "_pendingResumeNpcId", "002");
                SetPrivateField(service, "_pendingResumeExchangeIndex", 0);
                SetPrivateField(service, "_pendingResumeExpiresAt", 12f);

                object[] args =
                {
                    "003",
                    0,
                    string.Empty,
                    string.Empty,
                    null
                };

                object result = InvokeInstanceWithByRef(service, "TryResolvePendingResumePlan", args);

                Assert.IsNull(result);
                Assert.That(args[4].ToString(), Is.EqualTo("DifferentNpc"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(profile);
            }
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(host);
        }
    }

    [Test]
    public void GetResumeIntro_ShouldPreferExactCauseAndPhaseRule()
    {
        Type dialogueContentType = ResolveTypeOrFail("NPCDialogueContentProfile");
        Type stageSetType = ResolveNestedTypeOrFail(dialogueContentType, "RelationshipStageInformalChatSet");
        Type ruleType = ResolveNestedTypeOrFail(dialogueContentType, "InformalChatResumeRule");
        Type introType = ResolveNestedTypeOrFail(dialogueContentType, "InformalChatResumeIntro");
        Type leaveCauseType = ResolveTypeOrFail("NPCInformalChatLeaveCause");
        Type leavePhaseType = ResolveTypeOrFail("NPCInformalChatLeavePhase");
        Type relationshipStageType = ResolveTypeOrFail("NPCRelationshipStage");

        ScriptableObject profile = ScriptableObject.CreateInstance(dialogueContentType);

        try
        {
            object stageSet = Activator.CreateInstance(stageSetType);
            SetPrivateField(stageSet, "relationshipStage", Enum.Parse(relationshipStageType, "Close"));

            object exactRule = Activator.CreateInstance(ruleType);
            SetPrivateField(exactRule, "leaveCause", Enum.Parse(leaveCauseType, "DialogueTakeover"));
            SetPrivateField(exactRule, "leavePhase", Enum.Parse(leavePhaseType, "BetweenTurns"));
            SetPrivateField(
                exactRule,
                "resumeIntro",
                InvokeStatic(
                    introType,
                    "Create",
                    new[] { "我们把刚才那句接上吧。" },
                    new[] { "好，我还记着刚才那段。" },
                    0.18f));

            Array ruleArray = Array.CreateInstance(ruleType, 1);
            ruleArray.SetValue(exactRule, 0);
            SetPrivateField(stageSet, "resumeRules", ruleArray);

            Array stageSetArray = Array.CreateInstance(stageSetType, 1);
            stageSetArray.SetValue(stageSet, 0);
            SetPrivateField(profile, "relationshipStageInformalChatSets", stageSetArray);

            object intro = InvokeInstance(
                profile,
                "GetResumeIntro",
                Enum.Parse(relationshipStageType, "Close"),
                Enum.Parse(leaveCauseType, "DialogueTakeover"),
                Enum.Parse(leavePhaseType, "BetweenTurns"));

            string[] playerResumeLines = (string[])GetFieldOrProperty(intro, "PlayerResumeLines");
            string[] npcResumeLines = (string[])GetFieldOrProperty(intro, "NpcResumeLines");
            Assert.That(playerResumeLines[0], Is.EqualTo("我们把刚才那句接上吧。"));
            Assert.That(npcResumeLines[0], Is.EqualTo("好，我还记着刚才那段。"));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(profile);
        }
    }

    [Test]
    public void RoamProfile_ShouldExposeResumeIntro_FromDialogueContent()
    {
        Type dialogueContentType = ResolveTypeOrFail("NPCDialogueContentProfile");
        Type ruleType = ResolveNestedTypeOrFail(dialogueContentType, "InformalChatResumeRule");
        Type introType = ResolveNestedTypeOrFail(dialogueContentType, "InformalChatResumeIntro");
        Type roamProfileType = ResolveTypeOrFail("NPCRoamProfile");
        Type leaveCauseType = ResolveTypeOrFail("NPCInformalChatLeaveCause");
        Type leavePhaseType = ResolveTypeOrFail("NPCInformalChatLeavePhase");
        Type relationshipStageType = ResolveTypeOrFail("NPCRelationshipStage");

        ScriptableObject content = ScriptableObject.CreateInstance(dialogueContentType);
        ScriptableObject roamProfile = ScriptableObject.CreateInstance(roamProfileType);

        try
        {
            object defaultRule = Activator.CreateInstance(ruleType);
            SetPrivateField(defaultRule, "leaveCause", Enum.Parse(leaveCauseType, "BlockingUi"));
            SetPrivateField(defaultRule, "leavePhase", Enum.Parse(leavePhaseType, "Any"));
            SetPrivateField(
                defaultRule,
                "resumeIntro",
                InvokeStatic(
                    introType,
                    "Create",
                    new[] { "刚才忙完了，我们继续吧。" },
                    new[] { "好，我们接着刚才那段。" },
                    0.2f));

            Array ruleArray = Array.CreateInstance(ruleType, 1);
            ruleArray.SetValue(defaultRule, 0);
            SetPrivateField(content, "defaultResumeRules", ruleArray);
            SetPrivateField(roamProfile, "dialogueContentProfile", content);

            object intro = InvokeInstance(
                roamProfile,
                "GetResumeIntro",
                Enum.Parse(relationshipStageType, "Stranger"),
                Enum.Parse(leaveCauseType, "BlockingUi"),
                Enum.Parse(leavePhaseType, "BetweenTurns"));

            string[] playerResumeLines = (string[])GetFieldOrProperty(intro, "PlayerResumeLines");
            Assert.That(playerResumeLines[0], Is.EqualTo("刚才忙完了，我们继续吧。"));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(content);
            UnityEngine.Object.DestroyImmediate(roamProfile);
        }
    }

    private static Type ResolveNestedTypeOrFail(Type parentType, string nestedTypeName)
    {
        Type nestedType = parentType.GetNestedType(nestedTypeName, BindingFlags.Public | BindingFlags.NonPublic);
        Assert.IsNotNull(nestedType, $"未找到嵌套类型：{parentType.Name}+{nestedTypeName}");
        return nestedType;
    }

    private static Type ResolveTypeOrFail(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type direct = assembly.GetType(typeName);
            if (direct != null)
            {
                return direct;
            }

            Type[] candidates;
            try
            {
                candidates = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                candidates = ex.Types;
            }

            foreach (Type candidate in candidates)
            {
                if (candidate != null && (candidate.FullName == typeName || candidate.Name == typeName))
                {
                    return candidate;
                }
            }
        }

        Assert.Fail($"未找到类型：{typeName}");
        return null;
    }

    private static object InvokeStatic(Type type, string methodName, params object[] args)
    {
        MethodInfo method = ResolveMethodByArguments(type, methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, args);
        Assert.IsNotNull(method, $"未找到静态方法：{type.Name}.{methodName}");
        return method.Invoke(null, args);
    }

    private static object InvokeStaticWithByRef(Type type, string methodName, object[] args)
    {
        MethodInfo method = ResolveMethodByArguments(type, methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, args);
        Assert.IsNotNull(method, $"未找到静态方法：{type.Name}.{methodName}");
        return method.Invoke(null, args);
    }

    private static object InvokeInstance(object target, string methodName, params object[] args)
    {
        MethodInfo method = ResolveMethodByArguments(target.GetType(), methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, args);
        Assert.IsNotNull(method, $"未找到实例方法：{target.GetType().Name}.{methodName}");
        return method.Invoke(target, args);
    }

    private static object InvokeInstanceWithByRef(object target, string methodName, object[] args)
    {
        MethodInfo method = ResolveMethodByArguments(target.GetType(), methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, args);
        Assert.IsNotNull(method, $"未找到实例方法：{target.GetType().Name}.{methodName}");
        return method.Invoke(target, args);
    }

    private static object GetFieldOrProperty(object target, string name)
    {
        Type type = target.GetType();

        FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            return field.GetValue(target);
        }

        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null)
        {
            return property.GetValue(target);
        }

        Assert.Fail($"未找到字段或属性：{type.Name}.{name}");
        return null;
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"未找到字段：{target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private static MethodInfo ResolveMethodByArguments(Type type, string methodName, BindingFlags bindingFlags, object[] args)
    {
        MethodInfo fallback = null;
        MethodInfo[] methods = type.GetMethods(bindingFlags);

        for (int methodIndex = 0; methodIndex < methods.Length; methodIndex++)
        {
            MethodInfo method = methods[methodIndex];
            if (method.Name != methodName)
            {
                continue;
            }

            fallback ??= method;

            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != args.Length)
            {
                continue;
            }

            bool match = true;
            for (int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                Type parameterType = parameters[parameterIndex].ParameterType;
                if (parameterType.IsByRef)
                {
                    parameterType = parameterType.GetElementType();
                }

                object arg = args[parameterIndex];
                if (arg == null)
                {
                    if (parameterType.IsValueType && Nullable.GetUnderlyingType(parameterType) == null)
                    {
                        match = false;
                        break;
                    }

                    continue;
                }

                if (!parameterType.IsAssignableFrom(arg.GetType()))
                {
                    match = false;
                    break;
                }
            }

            if (match)
            {
                return method;
            }
        }

        return fallback;
    }
}
