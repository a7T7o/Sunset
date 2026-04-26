using System;
using System.Text;
using Sunset.Story;
using UnityEngine;

/// <summary>
/// NPC 对话内容资产。
/// 将角色内容与漫游参数拆开，便于后续继续扩展玩家轻响应和两两相遇矩阵。
/// </summary>
[CreateAssetMenu(fileName = "NPC_DialogueContentProfile", menuName = "Sunset/NPC/Dialogue Content Profile", order = 221)]
public class NPCDialogueContentProfile : ScriptableObject
{
    [Serializable]
    public sealed class PairDialogueSet
    {
        [SerializeField] private string partnerNpcId;
        [SerializeField] private string[] initiatorLines = Array.Empty<string>();
        [SerializeField] private string[] responderLines = Array.Empty<string>();

        public string PartnerNpcId => NPCDialogueContentProfile.NormalizeNpcId(partnerNpcId);
        public string[] InitiatorLines => initiatorLines ?? Array.Empty<string>();
        public string[] ResponderLines => responderLines ?? Array.Empty<string>();

        public void Sanitize()
        {
            partnerNpcId = NPCDialogueContentProfile.NormalizeNpcId(partnerNpcId);
            initiatorLines ??= Array.Empty<string>();
            responderLines ??= Array.Empty<string>();
        }
    }

    [Serializable]
    public sealed class RelationshipStageNearbySet
    {
        [SerializeField] private NPCRelationshipStage relationshipStage = NPCRelationshipStage.Stranger;
        [SerializeField] private string[] playerNearbyLines = Array.Empty<string>();

        public NPCRelationshipStage RelationshipStage => NPCRelationshipStageUtility.Sanitize(relationshipStage);
        public string[] PlayerNearbyLines => playerNearbyLines ?? Array.Empty<string>();

        public void Sanitize()
        {
            relationshipStage = NPCRelationshipStageUtility.Sanitize(relationshipStage);
            playerNearbyLines ??= Array.Empty<string>();
        }
    }

    [Serializable]
    public sealed class PhaseNearbySet
    {
        [SerializeField] private StoryPhase storyPhase = StoryPhase.None;
        [SerializeField] private string[] playerNearbyLines = Array.Empty<string>();

        public StoryPhase StoryPhase => storyPhase;
        public string[] PlayerNearbyLines => playerNearbyLines ?? Array.Empty<string>();

        public void Sanitize()
        {
            playerNearbyLines ??= Array.Empty<string>();
        }
    }

    [Serializable]
    public sealed class PhaseSelfTalkSet
    {
        [SerializeField] private StoryPhase storyPhase = StoryPhase.None;
        [SerializeField] private string[] selfTalkLines = Array.Empty<string>();

        public StoryPhase StoryPhase => storyPhase;
        public string[] SelfTalkLines => selfTalkLines ?? Array.Empty<string>();

        public void Sanitize()
        {
            selfTalkLines ??= Array.Empty<string>();
        }
    }

    [Serializable]
    public sealed class InformalChatExchange
    {
        [SerializeField] private string exchangeId = string.Empty;
        [SerializeField] private string[] playerLines = Array.Empty<string>();
        [SerializeField] private string[] npcReplyLines = Array.Empty<string>();
        [SerializeField] private float npcReplyDelayMin = 0.55f;
        [SerializeField] private float npcReplyDelayMax = 0.9f;

        public string ExchangeId => exchangeId ?? string.Empty;
        public string[] PlayerLines => playerLines ?? Array.Empty<string>();
        public string[] NpcReplyLines => npcReplyLines ?? Array.Empty<string>();
        public float NpcReplyDelayMin => Mathf.Max(0f, npcReplyDelayMin);
        public float NpcReplyDelayMax => Mathf.Max(NpcReplyDelayMin, npcReplyDelayMax);

        public void Sanitize()
        {
            exchangeId ??= string.Empty;
            playerLines ??= Array.Empty<string>();
            npcReplyLines ??= Array.Empty<string>();
            npcReplyDelayMin = Mathf.Max(0f, npcReplyDelayMin);
            npcReplyDelayMax = Mathf.Max(npcReplyDelayMin, npcReplyDelayMax);
        }
    }

    [Serializable]
    public sealed class InformalConversationBundle
    {
        [SerializeField] private string bundleId = string.Empty;
        [SerializeField] private InformalChatExchange[] exchanges = Array.Empty<InformalChatExchange>();
        [SerializeField] private int completionRelationshipDelta = 1;

        public string BundleId => bundleId ?? string.Empty;
        public InformalChatExchange[] Exchanges => exchanges ?? Array.Empty<InformalChatExchange>();
        public int CompletionRelationshipDelta => Mathf.Clamp(completionRelationshipDelta, -3, 3);

        public void Sanitize()
        {
            bundleId ??= string.Empty;
            exchanges ??= Array.Empty<InformalChatExchange>();
            completionRelationshipDelta = Mathf.Clamp(completionRelationshipDelta, -3, 3);

            for (int index = 0; index < exchanges.Length; index++)
            {
                exchanges[index]?.Sanitize();
            }
        }

        public bool HasPlayableExchanges()
        {
            InformalChatExchange[] candidateExchanges = Exchanges;
            for (int index = 0; index < candidateExchanges.Length; index++)
            {
                InformalChatExchange exchange = candidateExchanges[index];
                if (exchange != null && HasAnyLines(exchange.PlayerLines) && HasAnyLines(exchange.NpcReplyLines))
                {
                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public sealed class InformalChatInterruptReaction
    {
        [SerializeField] private string reactionCue = string.Empty;
        [SerializeField] private string[] playerExitLines = Array.Empty<string>();
        [SerializeField] private string[] npcReactionLines = Array.Empty<string>();
        [SerializeField] private int relationshipDelta = -1;
        [SerializeField] private float npcReactionDelay = 0.45f;

        public string ReactionCue => reactionCue ?? string.Empty;
        public string[] PlayerExitLines => playerExitLines ?? Array.Empty<string>();
        public string[] NpcReactionLines => npcReactionLines ?? Array.Empty<string>();
        public int RelationshipDelta => Mathf.Clamp(relationshipDelta, -3, 3);
        public float NpcReactionDelay => Mathf.Max(0f, npcReactionDelay);

        public void Sanitize()
        {
            reactionCue ??= string.Empty;
            playerExitLines ??= Array.Empty<string>();
            npcReactionLines ??= Array.Empty<string>();
            relationshipDelta = Mathf.Clamp(relationshipDelta, -3, 3);
            npcReactionDelay = Mathf.Max(0f, npcReactionDelay);
        }

        public bool HasAnyContent()
        {
            return HasAnyLines(PlayerExitLines) || HasAnyLines(NpcReactionLines) || !string.IsNullOrWhiteSpace(ReactionCue);
        }

        public static InformalChatInterruptReaction Create(
            string reactionCue,
            string[] playerExitLines,
            string[] npcReactionLines,
            int relationshipDelta = 0,
            float npcReactionDelay = 0.45f)
        {
            InformalChatInterruptReaction reaction = new InformalChatInterruptReaction
            {
                reactionCue = reactionCue ?? string.Empty,
                playerExitLines = playerExitLines ?? Array.Empty<string>(),
                npcReactionLines = npcReactionLines ?? Array.Empty<string>(),
                relationshipDelta = relationshipDelta,
                npcReactionDelay = npcReactionDelay
            };
            reaction.Sanitize();
            return reaction;
        }
    }

    [Serializable]
    public sealed class InformalChatInterruptRule
    {
        [SerializeField] private NPCInformalChatLeaveCause leaveCause = NPCInformalChatLeaveCause.Any;
        [SerializeField] private NPCInformalChatLeavePhase leavePhase = NPCInformalChatLeavePhase.Any;
        [SerializeField] private InformalChatInterruptReaction reaction = new InformalChatInterruptReaction();

        public NPCInformalChatLeaveCause LeaveCause => leaveCause;
        public NPCInformalChatLeavePhase LeavePhase => leavePhase;
        public InformalChatInterruptReaction Reaction => reaction;

        public void Sanitize()
        {
            reaction ??= new InformalChatInterruptReaction();
            reaction.Sanitize();
        }

        public int MatchScore(NPCInformalChatLeaveCause cause, NPCInformalChatLeavePhase phase)
        {
            if (leaveCause != NPCInformalChatLeaveCause.Any && leaveCause != cause)
            {
                return -1;
            }

            if (leavePhase != NPCInformalChatLeavePhase.Any && leavePhase != phase)
            {
                return -1;
            }

            int score = 0;
            if (leaveCause == cause)
            {
                score += 2;
            }

            if (leavePhase == phase)
            {
                score += 1;
            }

            return score;
        }
    }

    [Serializable]
    public sealed class InformalChatResumeIntro
    {
        [SerializeField] private string[] playerResumeLines = Array.Empty<string>();
        [SerializeField] private string[] npcResumeLines = Array.Empty<string>();
        [SerializeField] private float npcResumeDelay = 0.18f;

        public string[] PlayerResumeLines => playerResumeLines ?? Array.Empty<string>();
        public string[] NpcResumeLines => npcResumeLines ?? Array.Empty<string>();
        public float NpcResumeDelay => Mathf.Max(0f, npcResumeDelay);

        public void Sanitize()
        {
            playerResumeLines ??= Array.Empty<string>();
            npcResumeLines ??= Array.Empty<string>();
            npcResumeDelay = Mathf.Max(0f, npcResumeDelay);
        }

        public bool HasAnyContent()
        {
            return HasAnyLines(PlayerResumeLines) || HasAnyLines(NpcResumeLines);
        }

        public static InformalChatResumeIntro Create(
            string[] playerResumeLines,
            string[] npcResumeLines,
            float npcResumeDelay = 0.18f)
        {
            InformalChatResumeIntro intro = new InformalChatResumeIntro
            {
                playerResumeLines = playerResumeLines ?? Array.Empty<string>(),
                npcResumeLines = npcResumeLines ?? Array.Empty<string>(),
                npcResumeDelay = npcResumeDelay
            };
            intro.Sanitize();
            return intro;
        }
    }

    [Serializable]
    public sealed class InformalChatResumeRule
    {
        [SerializeField] private NPCInformalChatLeaveCause leaveCause = NPCInformalChatLeaveCause.Any;
        [SerializeField] private NPCInformalChatLeavePhase leavePhase = NPCInformalChatLeavePhase.Any;
        [SerializeField] private InformalChatResumeIntro resumeIntro = new InformalChatResumeIntro();

        public NPCInformalChatLeaveCause LeaveCause => leaveCause;
        public NPCInformalChatLeavePhase LeavePhase => leavePhase;
        public InformalChatResumeIntro ResumeIntro => resumeIntro;

        public void Sanitize()
        {
            resumeIntro ??= new InformalChatResumeIntro();
            resumeIntro.Sanitize();
        }

        public int MatchScore(NPCInformalChatLeaveCause cause, NPCInformalChatLeavePhase phase)
        {
            if (leaveCause != NPCInformalChatLeaveCause.Any && leaveCause != cause)
            {
                return -1;
            }

            if (leavePhase != NPCInformalChatLeavePhase.Any && leavePhase != phase)
            {
                return -1;
            }

            int score = 0;
            if (leaveCause == cause)
            {
                score += 2;
            }

            if (leavePhase == phase)
            {
                score += 1;
            }

            return score;
        }
    }

    [Serializable]
    public sealed class RelationshipStageInformalChatSet
    {
        [SerializeField] private NPCRelationshipStage relationshipStage = NPCRelationshipStage.Stranger;
        [SerializeField] private InformalConversationBundle[] conversationBundles = Array.Empty<InformalConversationBundle>();
        [SerializeField] private InformalChatInterruptReaction walkAwayReaction = new InformalChatInterruptReaction();
        [SerializeField] private InformalChatInterruptRule[] interruptRules = Array.Empty<InformalChatInterruptRule>();
        [SerializeField] private InformalChatResumeRule[] resumeRules = Array.Empty<InformalChatResumeRule>();

        public NPCRelationshipStage RelationshipStage => NPCRelationshipStageUtility.Sanitize(relationshipStage);
        public InformalConversationBundle[] ConversationBundles => conversationBundles ?? Array.Empty<InformalConversationBundle>();
        public InformalChatInterruptReaction WalkAwayReaction => walkAwayReaction;
        public InformalChatInterruptRule[] InterruptRules => interruptRules ?? Array.Empty<InformalChatInterruptRule>();
        public InformalChatResumeRule[] ResumeRules => resumeRules ?? Array.Empty<InformalChatResumeRule>();

        public void Sanitize()
        {
            relationshipStage = NPCRelationshipStageUtility.Sanitize(relationshipStage);
            conversationBundles ??= Array.Empty<InformalConversationBundle>();
            walkAwayReaction ??= new InformalChatInterruptReaction();
            interruptRules ??= Array.Empty<InformalChatInterruptRule>();
            resumeRules ??= Array.Empty<InformalChatResumeRule>();

            for (int index = 0; index < conversationBundles.Length; index++)
            {
                conversationBundles[index]?.Sanitize();
            }

            walkAwayReaction.Sanitize();

            for (int index = 0; index < interruptRules.Length; index++)
            {
                interruptRules[index]?.Sanitize();
            }

            for (int index = 0; index < resumeRules.Length; index++)
            {
                resumeRules[index]?.Sanitize();
            }
        }

        public bool HasConversationContent()
        {
            InformalConversationBundle[] bundles = ConversationBundles;
            for (int index = 0; index < bundles.Length; index++)
            {
                InformalConversationBundle bundle = bundles[index];
                if (bundle != null && bundle.HasPlayableExchanges())
                {
                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public sealed class PhaseInformalChatSet
    {
        [SerializeField] private StoryPhase storyPhase = StoryPhase.None;
        [SerializeField] private InformalConversationBundle[] conversationBundles = Array.Empty<InformalConversationBundle>();
        [SerializeField] private InformalChatInterruptReaction walkAwayReaction = new InformalChatInterruptReaction();
        [SerializeField] private InformalChatInterruptRule[] interruptRules = Array.Empty<InformalChatInterruptRule>();
        [SerializeField] private InformalChatResumeRule[] resumeRules = Array.Empty<InformalChatResumeRule>();

        public StoryPhase StoryPhase => storyPhase;
        public InformalConversationBundle[] ConversationBundles => conversationBundles ?? Array.Empty<InformalConversationBundle>();
        public InformalChatInterruptReaction WalkAwayReaction => walkAwayReaction;
        public InformalChatInterruptRule[] InterruptRules => interruptRules ?? Array.Empty<InformalChatInterruptRule>();
        public InformalChatResumeRule[] ResumeRules => resumeRules ?? Array.Empty<InformalChatResumeRule>();

        public void Sanitize()
        {
            conversationBundles ??= Array.Empty<InformalConversationBundle>();
            walkAwayReaction ??= new InformalChatInterruptReaction();
            interruptRules ??= Array.Empty<InformalChatInterruptRule>();
            resumeRules ??= Array.Empty<InformalChatResumeRule>();

            for (int index = 0; index < conversationBundles.Length; index++)
            {
                conversationBundles[index]?.Sanitize();
            }

            walkAwayReaction.Sanitize();

            for (int index = 0; index < interruptRules.Length; index++)
            {
                interruptRules[index]?.Sanitize();
            }

            for (int index = 0; index < resumeRules.Length; index++)
            {
                resumeRules[index]?.Sanitize();
            }
        }

        public bool HasConversationContent()
        {
            InformalConversationBundle[] bundles = ConversationBundles;
            for (int index = 0; index < bundles.Length; index++)
            {
                InformalConversationBundle bundle = bundles[index];
                if (bundle != null && bundle.HasPlayableExchanges())
                {
                    return true;
                }
            }

            return false;
        }
    }

    [Header("身份标识")]
    [SerializeField] private string npcId = string.Empty;

    [Header("单人环境气泡")]
    [SerializeField] private string[] selfTalkLines = Array.Empty<string>();

    [Header("按剧情阶段分流的居民自语")]
    [SerializeField] private PhaseSelfTalkSet[] phaseSelfTalkLines = Array.Empty<PhaseSelfTalkSet>();

    [Header("玩家轻响应")]
    [SerializeField] private string[] playerNearbyLines = Array.Empty<string>();

    [Header("按关系阶段分流的玩家轻响应")]
    [SerializeField] private RelationshipStageNearbySet[] relationshipStageNearbyLines = Array.Empty<RelationshipStageNearbySet>();

    [Header("按剧情阶段分流的居民近身反馈")]
    [SerializeField] private PhaseNearbySet[] phaseNearbyLines = Array.Empty<PhaseNearbySet>();

    [Header("默认闲聊会话")]
    [SerializeField] private InformalConversationBundle[] defaultInformalConversationBundles = Array.Empty<InformalConversationBundle>();

    [Header("按关系阶段分流的闲聊会话")]
    [SerializeField] private RelationshipStageInformalChatSet[] relationshipStageInformalChatSets = Array.Empty<RelationshipStageInformalChatSet>();

    [Header("按剧情阶段分流的居民闲聊会话")]
    [SerializeField] private PhaseInformalChatSet[] phaseInformalChatSets = Array.Empty<PhaseInformalChatSet>();

    [Header("默认聊到一半离开反应")]
    [SerializeField] private InformalChatInterruptReaction defaultWalkAwayReaction = new InformalChatInterruptReaction();

    [Header("默认中断反应矩阵")]
    [SerializeField] private InformalChatInterruptRule[] defaultInterruptRules = Array.Empty<InformalChatInterruptRule>();

    [Header("默认续聊补口矩阵")]
    [SerializeField] private InformalChatResumeRule[] defaultResumeRules = Array.Empty<InformalChatResumeRule>();

    [Header("通用相遇发起")]
    [SerializeField] private string[] defaultChatInitiatorLines = Array.Empty<string>();

    [Header("通用相遇回应")]
    [SerializeField] private string[] defaultChatResponderLines = Array.Empty<string>();

    [Header("按搭档区分的相遇矩阵")]
    [SerializeField] private PairDialogueSet[] pairDialogueSets = Array.Empty<PairDialogueSet>();

    public string NpcId => ResolveNpcId();
    public string[] SelfTalkLines => selfTalkLines ?? Array.Empty<string>();
    public PhaseSelfTalkSet[] PhaseSelfTalkLines => phaseSelfTalkLines ?? Array.Empty<PhaseSelfTalkSet>();
    public string[] PlayerNearbyLines => playerNearbyLines ?? Array.Empty<string>();
    public RelationshipStageNearbySet[] RelationshipStageNearbyLines => relationshipStageNearbyLines ?? Array.Empty<RelationshipStageNearbySet>();
    public PhaseNearbySet[] PhaseNearbyLines => phaseNearbyLines ?? Array.Empty<PhaseNearbySet>();
    public InformalConversationBundle[] DefaultInformalConversationBundles => defaultInformalConversationBundles ?? Array.Empty<InformalConversationBundle>();
    public RelationshipStageInformalChatSet[] RelationshipStageInformalChatSets => relationshipStageInformalChatSets ?? Array.Empty<RelationshipStageInformalChatSet>();
    public PhaseInformalChatSet[] PhaseInformalChatSets => phaseInformalChatSets ?? Array.Empty<PhaseInformalChatSet>();
    public InformalChatInterruptReaction DefaultWalkAwayReaction => defaultWalkAwayReaction;
    public InformalChatInterruptRule[] DefaultInterruptRules => defaultInterruptRules ?? Array.Empty<InformalChatInterruptRule>();
    public InformalChatResumeRule[] DefaultResumeRules => defaultResumeRules ?? Array.Empty<InformalChatResumeRule>();
    public string[] DefaultChatInitiatorLines => defaultChatInitiatorLines ?? Array.Empty<string>();
    public string[] DefaultChatResponderLines => defaultChatResponderLines ?? Array.Empty<string>();
    public PairDialogueSet[] PairDialogueSets => pairDialogueSets ?? Array.Empty<PairDialogueSet>();
    public bool HasSelfTalkContent => HasAnyLines(SelfTalkLines) || HasAnyPhaseSelfTalkLines();
    public bool HasPlayerNearbyContent => HasAnyLines(PlayerNearbyLines) || HasAnyStageNearbyLines() || HasAnyPhaseNearbyLines();
    public bool HasInformalConversationContent =>
        HasAnyPlayableConversationBundles(DefaultInformalConversationBundles) ||
        HasAnyStageConversationBundles() ||
        HasAnyPhaseConversationBundles();
    public bool HasAmbientChatInitiatorContent => HasAnyLines(DefaultChatInitiatorLines) || HasAnyPairLines(initiator: true);
    public bool HasAmbientChatResponderContent => HasAnyLines(DefaultChatResponderLines) || HasAnyPairLines(initiator: false);

    public string ResolveNpcId(string fallbackName = "")
    {
        string normalizedNpcId = NormalizeNpcId(npcId);
        return !string.IsNullOrEmpty(normalizedNpcId)
            ? normalizedNpcId
            : NormalizeNpcId(fallbackName);
    }

    public string[] GetAmbientChatLines(string partnerNpcId, bool initiator)
    {
        if (TryGetPairDialogueSet(partnerNpcId, out PairDialogueSet pairDialogueSet))
        {
            string[] pairLines = initiator ? pairDialogueSet.InitiatorLines : pairDialogueSet.ResponderLines;
            if (HasAnyLines(pairLines))
            {
                return pairLines;
            }
        }

        return initiator ? DefaultChatInitiatorLines : DefaultChatResponderLines;
    }

    public string[] GetSelfTalkLines(StoryPhase storyPhase)
    {
        if (TryGetPhaseSelfTalkSet(storyPhase, out PhaseSelfTalkSet phaseSet))
        {
            string[] phaseSpecificLines = phaseSet.SelfTalkLines;
            if (HasAnyLines(phaseSpecificLines))
            {
                return phaseSpecificLines;
            }
        }

        return SelfTalkLines;
    }

    public string[] GetPlayerNearbyLines(NPCRelationshipStage relationshipStage)
    {
        return GetPlayerNearbyLines(relationshipStage, StoryPhase.None);
    }

    public string[] GetPlayerNearbyLines(NPCRelationshipStage relationshipStage, StoryPhase storyPhase)
    {
        if (TryGetPhaseNearbySet(storyPhase, out PhaseNearbySet phaseLineSet))
        {
            string[] phaseSpecificLines = phaseLineSet.PlayerNearbyLines;
            if (HasAnyLines(phaseSpecificLines))
            {
                return phaseSpecificLines;
            }
        }

        if (TryGetRelationshipStageNearbySet(relationshipStage, out RelationshipStageNearbySet relationshipStageLineSet))
        {
            string[] stageSpecificLines = relationshipStageLineSet.PlayerNearbyLines;
            if (HasAnyLines(stageSpecificLines))
            {
                return stageSpecificLines;
            }
        }

        return PlayerNearbyLines;
    }

    public InformalConversationBundle[] GetInformalConversationBundles(NPCRelationshipStage relationshipStage)
    {
        return GetInformalConversationBundles(relationshipStage, StoryPhase.None);
    }

    public InformalConversationBundle[] GetInformalConversationBundles(
        NPCRelationshipStage relationshipStage,
        StoryPhase storyPhase)
    {
        if (TryGetPhaseInformalChatSet(storyPhase, out PhaseInformalChatSet phaseChatSet))
        {
            InformalConversationBundle[] phaseBundles = phaseChatSet.ConversationBundles;
            if (HasAnyPlayableConversationBundles(phaseBundles))
            {
                return phaseBundles;
            }
        }

        if (TryGetRelationshipStageInformalChatSet(relationshipStage, out RelationshipStageInformalChatSet chatSet))
        {
            InformalConversationBundle[] stageBundles = chatSet.ConversationBundles;
            if (HasAnyPlayableConversationBundles(stageBundles))
            {
                return stageBundles;
            }
        }

        return DefaultInformalConversationBundles;
    }

    public InformalChatInterruptReaction GetWalkAwayReaction(NPCRelationshipStage relationshipStage)
    {
        return GetWalkAwayReaction(relationshipStage, StoryPhase.None);
    }

    public InformalChatInterruptReaction GetWalkAwayReaction(
        NPCRelationshipStage relationshipStage,
        StoryPhase storyPhase)
    {
        if (TryGetPhaseInformalChatSet(storyPhase, out PhaseInformalChatSet phaseChatSet) &&
            phaseChatSet.WalkAwayReaction != null &&
            phaseChatSet.WalkAwayReaction.HasAnyContent())
        {
            return phaseChatSet.WalkAwayReaction;
        }

        if (TryGetRelationshipStageInformalChatSet(relationshipStage, out RelationshipStageInformalChatSet chatSet) &&
            chatSet.WalkAwayReaction != null &&
            chatSet.WalkAwayReaction.HasAnyContent())
        {
            return chatSet.WalkAwayReaction;
        }

        return DefaultWalkAwayReaction;
    }

    public InformalChatInterruptReaction GetInterruptReaction(
        NPCRelationshipStage relationshipStage,
        NPCInformalChatLeaveCause leaveCause,
        NPCInformalChatLeavePhase leavePhase)
    {
        return GetInterruptReaction(relationshipStage, StoryPhase.None, leaveCause, leavePhase);
    }

    public InformalChatInterruptReaction GetInterruptReaction(
        NPCRelationshipStage relationshipStage,
        StoryPhase storyPhase,
        NPCInformalChatLeaveCause leaveCause,
        NPCInformalChatLeavePhase leavePhase)
    {
        if (TryGetPhaseInformalChatSet(storyPhase, out PhaseInformalChatSet phaseChatSet))
        {
            InformalChatInterruptReaction phaseReaction = FindBestInterruptReaction(phaseChatSet.InterruptRules, leaveCause, leavePhase);
            if (phaseReaction != null)
            {
                return phaseReaction;
            }

            if (leaveCause == NPCInformalChatLeaveCause.DistanceGraceExceeded &&
                phaseChatSet.WalkAwayReaction != null &&
                phaseChatSet.WalkAwayReaction.HasAnyContent())
            {
                return phaseChatSet.WalkAwayReaction;
            }
        }

        if (TryGetRelationshipStageInformalChatSet(relationshipStage, out RelationshipStageInformalChatSet chatSet))
        {
            InformalChatInterruptReaction stageReaction = FindBestInterruptReaction(chatSet.InterruptRules, leaveCause, leavePhase);
            if (stageReaction != null)
            {
                return stageReaction;
            }

            if (leaveCause == NPCInformalChatLeaveCause.DistanceGraceExceeded &&
                chatSet.WalkAwayReaction != null &&
                chatSet.WalkAwayReaction.HasAnyContent())
            {
                return chatSet.WalkAwayReaction;
            }
        }

        InformalChatInterruptReaction defaultReaction = FindBestInterruptReaction(DefaultInterruptRules, leaveCause, leavePhase);
        if (defaultReaction != null)
        {
            return defaultReaction;
        }

        if (leaveCause == NPCInformalChatLeaveCause.DistanceGraceExceeded &&
            DefaultWalkAwayReaction != null &&
            DefaultWalkAwayReaction.HasAnyContent())
        {
            return DefaultWalkAwayReaction;
        }

        return null;
    }

    public InformalChatResumeIntro GetResumeIntro(
        NPCRelationshipStage relationshipStage,
        NPCInformalChatLeaveCause leaveCause,
        NPCInformalChatLeavePhase leavePhase)
    {
        return GetResumeIntro(relationshipStage, StoryPhase.None, leaveCause, leavePhase);
    }

    public InformalChatResumeIntro GetResumeIntro(
        NPCRelationshipStage relationshipStage,
        StoryPhase storyPhase,
        NPCInformalChatLeaveCause leaveCause,
        NPCInformalChatLeavePhase leavePhase)
    {
        if (TryGetPhaseInformalChatSet(storyPhase, out PhaseInformalChatSet phaseChatSet))
        {
            InformalChatResumeIntro phaseIntro = FindBestResumeIntro(phaseChatSet.ResumeRules, leaveCause, leavePhase);
            if (phaseIntro != null)
            {
                return phaseIntro;
            }
        }

        if (TryGetRelationshipStageInformalChatSet(relationshipStage, out RelationshipStageInformalChatSet chatSet))
        {
            InformalChatResumeIntro stageIntro = FindBestResumeIntro(chatSet.ResumeRules, leaveCause, leavePhase);
            if (stageIntro != null)
            {
                return stageIntro;
            }
        }

        return FindBestResumeIntro(DefaultResumeRules, leaveCause, leavePhase);
    }

    public bool TryGetPairDialogueSet(string partnerNpcId, out PairDialogueSet pairDialogueSet)
    {
        string normalizedPartnerId = NormalizeNpcId(partnerNpcId);
        PairDialogueSet[] pairSets = PairDialogueSets;
        for (int index = 0; index < pairSets.Length; index++)
        {
            PairDialogueSet candidate = pairSets[index];
            if (candidate == null)
            {
                continue;
            }

            if (string.Equals(candidate.PartnerNpcId, normalizedPartnerId, StringComparison.OrdinalIgnoreCase))
            {
                pairDialogueSet = candidate;
                return true;
            }
        }

        pairDialogueSet = null;
        return false;
    }

    public bool TryGetRelationshipStageNearbySet(NPCRelationshipStage relationshipStage, out RelationshipStageNearbySet relationshipStageLineSet)
    {
        NPCRelationshipStage sanitizedStage = NPCRelationshipStageUtility.Sanitize(relationshipStage);
        RelationshipStageNearbySet[] stageSets = RelationshipStageNearbyLines;
        for (int index = 0; index < stageSets.Length; index++)
        {
            RelationshipStageNearbySet candidate = stageSets[index];
            if (candidate == null)
            {
                continue;
            }

            if (candidate.RelationshipStage == sanitizedStage)
            {
                relationshipStageLineSet = candidate;
                return true;
            }
        }

        relationshipStageLineSet = null;
        return false;
    }

    public bool TryGetPhaseNearbySet(StoryPhase storyPhase, out PhaseNearbySet nearbySet)
    {
        if (storyPhase == StoryPhase.None)
        {
            nearbySet = null;
            return false;
        }

        PhaseNearbySet[] phaseSets = PhaseNearbyLines;
        for (int index = 0; index < phaseSets.Length; index++)
        {
            PhaseNearbySet candidate = phaseSets[index];
            if (candidate == null)
            {
                continue;
            }

            if (candidate.StoryPhase == storyPhase)
            {
                nearbySet = candidate;
                return true;
            }
        }

        nearbySet = null;
        return false;
    }

    public bool TryGetPhaseSelfTalkSet(StoryPhase storyPhase, out PhaseSelfTalkSet selfTalkSet)
    {
        if (storyPhase == StoryPhase.None)
        {
            selfTalkSet = null;
            return false;
        }

        PhaseSelfTalkSet[] phaseSets = PhaseSelfTalkLines;
        for (int index = 0; index < phaseSets.Length; index++)
        {
            PhaseSelfTalkSet candidate = phaseSets[index];
            if (candidate == null)
            {
                continue;
            }

            if (candidate.StoryPhase == storyPhase)
            {
                selfTalkSet = candidate;
                return true;
            }
        }

        selfTalkSet = null;
        return false;
    }

    public bool TryGetRelationshipStageInformalChatSet(NPCRelationshipStage relationshipStage, out RelationshipStageInformalChatSet chatSet)
    {
        NPCRelationshipStage sanitizedStage = NPCRelationshipStageUtility.Sanitize(relationshipStage);
        RelationshipStageInformalChatSet[] stageSets = RelationshipStageInformalChatSets;
        for (int index = 0; index < stageSets.Length; index++)
        {
            RelationshipStageInformalChatSet candidate = stageSets[index];
            if (candidate == null)
            {
                continue;
            }

            if (candidate.RelationshipStage == sanitizedStage)
            {
                chatSet = candidate;
                return true;
            }
        }

        chatSet = null;
        return false;
    }

    public bool TryGetPhaseInformalChatSet(StoryPhase storyPhase, out PhaseInformalChatSet chatSet)
    {
        if (storyPhase == StoryPhase.None)
        {
            chatSet = null;
            return false;
        }

        PhaseInformalChatSet[] phaseSets = PhaseInformalChatSets;
        for (int index = 0; index < phaseSets.Length; index++)
        {
            PhaseInformalChatSet candidate = phaseSets[index];
            if (candidate == null)
            {
                continue;
            }

            if (candidate.StoryPhase == storyPhase)
            {
                chatSet = candidate;
                return true;
            }
        }

        chatSet = null;
        return false;
    }

    public static string NormalizeNpcId(string rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return string.Empty;
        }

        string trimmed = rawValue.Trim();
        StringBuilder digits = new StringBuilder(trimmed.Length);
        for (int index = 0; index < trimmed.Length; index++)
        {
            char current = trimmed[index];
            if (char.IsDigit(current))
            {
                digits.Append(current);
            }
        }

        if (digits.Length > 0)
        {
            return digits.ToString();
        }

        if (trimmed.StartsWith("NPC_", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed.Substring(4).Trim();
        }

        if (trimmed.StartsWith("NPC", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed.Substring(3).TrimStart('_', ' ');
        }

        return trimmed;
    }

    private void OnValidate()
    {
        npcId = NormalizeNpcId(npcId);
        selfTalkLines ??= Array.Empty<string>();
        phaseSelfTalkLines ??= Array.Empty<PhaseSelfTalkSet>();
        playerNearbyLines ??= Array.Empty<string>();
        relationshipStageNearbyLines ??= Array.Empty<RelationshipStageNearbySet>();
        phaseNearbyLines ??= Array.Empty<PhaseNearbySet>();
        defaultInformalConversationBundles ??= Array.Empty<InformalConversationBundle>();
        relationshipStageInformalChatSets ??= Array.Empty<RelationshipStageInformalChatSet>();
        phaseInformalChatSets ??= Array.Empty<PhaseInformalChatSet>();
        defaultWalkAwayReaction ??= new InformalChatInterruptReaction();
        defaultInterruptRules ??= Array.Empty<InformalChatInterruptRule>();
        defaultResumeRules ??= Array.Empty<InformalChatResumeRule>();
        defaultChatInitiatorLines ??= Array.Empty<string>();
        defaultChatResponderLines ??= Array.Empty<string>();
        pairDialogueSets ??= Array.Empty<PairDialogueSet>();

        for (int index = 0; index < phaseSelfTalkLines.Length; index++)
        {
            phaseSelfTalkLines[index]?.Sanitize();
        }

        for (int index = 0; index < relationshipStageNearbyLines.Length; index++)
        {
            relationshipStageNearbyLines[index]?.Sanitize();
        }

        for (int index = 0; index < phaseNearbyLines.Length; index++)
        {
            phaseNearbyLines[index]?.Sanitize();
        }

        for (int index = 0; index < defaultInformalConversationBundles.Length; index++)
        {
            defaultInformalConversationBundles[index]?.Sanitize();
        }

        for (int index = 0; index < relationshipStageInformalChatSets.Length; index++)
        {
            relationshipStageInformalChatSets[index]?.Sanitize();
        }

        for (int index = 0; index < phaseInformalChatSets.Length; index++)
        {
            phaseInformalChatSets[index]?.Sanitize();
        }

        defaultWalkAwayReaction.Sanitize();

        for (int index = 0; index < defaultInterruptRules.Length; index++)
        {
            defaultInterruptRules[index]?.Sanitize();
        }

        for (int index = 0; index < defaultResumeRules.Length; index++)
        {
            defaultResumeRules[index]?.Sanitize();
        }

        for (int index = 0; index < pairDialogueSets.Length; index++)
        {
            pairDialogueSets[index]?.Sanitize();
        }
    }

    private bool HasAnyStageNearbyLines()
    {
        RelationshipStageNearbySet[] stageSets = RelationshipStageNearbyLines;
        for (int index = 0; index < stageSets.Length; index++)
        {
            RelationshipStageNearbySet candidate = stageSets[index];
            if (candidate == null)
            {
                continue;
            }

            if (HasAnyLines(candidate.PlayerNearbyLines))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasAnyPhaseSelfTalkLines()
    {
        PhaseSelfTalkSet[] phaseSets = PhaseSelfTalkLines;
        for (int index = 0; index < phaseSets.Length; index++)
        {
            PhaseSelfTalkSet candidate = phaseSets[index];
            if (candidate == null)
            {
                continue;
            }

            if (HasAnyLines(candidate.SelfTalkLines))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasAnyPhaseNearbyLines()
    {
        PhaseNearbySet[] phaseSets = PhaseNearbyLines;
        for (int index = 0; index < phaseSets.Length; index++)
        {
            PhaseNearbySet candidate = phaseSets[index];
            if (candidate == null)
            {
                continue;
            }

            if (HasAnyLines(candidate.PlayerNearbyLines))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasAnyStageConversationBundles()
    {
        RelationshipStageInformalChatSet[] stageSets = RelationshipStageInformalChatSets;
        for (int index = 0; index < stageSets.Length; index++)
        {
            RelationshipStageInformalChatSet candidate = stageSets[index];
            if (candidate != null && candidate.HasConversationContent())
            {
                return true;
            }
        }

        return false;
    }

    private bool HasAnyPhaseConversationBundles()
    {
        PhaseInformalChatSet[] phaseSets = PhaseInformalChatSets;
        for (int index = 0; index < phaseSets.Length; index++)
        {
            PhaseInformalChatSet candidate = phaseSets[index];
            if (candidate != null && candidate.HasConversationContent())
            {
                return true;
            }
        }

        return false;
    }

    private bool HasAnyPairLines(bool initiator)
    {
        PairDialogueSet[] pairSets = PairDialogueSets;
        for (int index = 0; index < pairSets.Length; index++)
        {
            PairDialogueSet candidate = pairSets[index];
            if (candidate == null)
            {
                continue;
            }

            if (HasAnyLines(initiator ? candidate.InitiatorLines : candidate.ResponderLines))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasAnyPlayableConversationBundles(InformalConversationBundle[] bundles)
    {
        if (bundles == null)
        {
            return false;
        }

        for (int index = 0; index < bundles.Length; index++)
        {
            InformalConversationBundle bundle = bundles[index];
            if (bundle != null && bundle.HasPlayableExchanges())
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasAnyLines(string[] lines)
    {
        if (lines == null)
        {
            return false;
        }

        for (int index = 0; index < lines.Length; index++)
        {
            if (!string.IsNullOrWhiteSpace(lines[index]))
            {
                return true;
            }
        }

        return false;
    }

    private static InformalChatInterruptReaction FindBestInterruptReaction(
        InformalChatInterruptRule[] rules,
        NPCInformalChatLeaveCause leaveCause,
        NPCInformalChatLeavePhase leavePhase)
    {
        if (rules == null)
        {
            return null;
        }

        InformalChatInterruptReaction bestReaction = null;
        int bestScore = -1;
        for (int index = 0; index < rules.Length; index++)
        {
            InformalChatInterruptRule rule = rules[index];
            if (rule == null || rule.Reaction == null || !rule.Reaction.HasAnyContent())
            {
                continue;
            }

            int score = rule.MatchScore(leaveCause, leavePhase);
            if (score < 0 || score < bestScore)
            {
                continue;
            }

            bestScore = score;
            bestReaction = rule.Reaction;
        }

        return bestReaction;
    }

    private static InformalChatResumeIntro FindBestResumeIntro(
        InformalChatResumeRule[] rules,
        NPCInformalChatLeaveCause leaveCause,
        NPCInformalChatLeavePhase leavePhase)
    {
        if (rules == null)
        {
            return null;
        }

        InformalChatResumeIntro bestIntro = null;
        int bestScore = -1;
        for (int index = 0; index < rules.Length; index++)
        {
            InformalChatResumeRule rule = rules[index];
            if (rule == null || rule.ResumeIntro == null || !rule.ResumeIntro.HasAnyContent())
            {
                continue;
            }

            int score = rule.MatchScore(leaveCause, leavePhase);
            if (score < 0 || score < bestScore)
            {
                continue;
            }

            bestScore = score;
            bestIntro = rule.ResumeIntro;
        }

        return bestIntro;
    }
}
