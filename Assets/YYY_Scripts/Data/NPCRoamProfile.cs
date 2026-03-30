using System;
using UnityEngine;

/// <summary>
/// NPC 漫游配置资产。
/// 统一管理移动节奏、动画速度、卡住恢复，以及 NPC 的内容资产映射。
/// </summary>
[CreateAssetMenu(fileName = "NPC_RoamProfile", menuName = "Sunset/NPC/Roam Profile", order = 220)]
public class NPCRoamProfile : ScriptableObject
{
    [Header("移动与动画")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float idleAnimationSpeed = 1f;
    [SerializeField] private float moveAnimationSpeed = 1f;

    [Header("活动范围")]
    [SerializeField] private float activityRadius = 3f;
    [SerializeField] private float minimumMoveDistance = 0.6f;
    [SerializeField] private int pathSampleAttempts = 12;

    [Header("短暂停留")]
    [SerializeField] private float shortPauseMin = 0.5f;
    [SerializeField] private float shortPauseMax = 3f;
    [SerializeField] private int shortPauseCountMin = 3;
    [SerializeField] private int shortPauseCountMax = 5;

    [Header("长停留")]
    [SerializeField] private float longPauseMin = 3f;
    [SerializeField] private float longPauseMax = 6f;

    [Header("卡住恢复")]
    [SerializeField] private float stuckCheckInterval = 0.3f;
    [SerializeField] private float stuckDistanceThreshold = 0.08f;
    [SerializeField] private int maxStuckRecoveries = 3;

    [Header("环境聊天")]
    [SerializeField] private bool enableAmbientChat = true;
    [SerializeField] private float ambientChatRadius = 3.8f;
    [SerializeField, Range(0f, 1f)] private float ambientChatChance = 0.75f;
    [SerializeField] private float ambientChatResponseDelay = 0.85f;

    [Header("内容资产")]
    [SerializeField] private NPCDialogueContentProfile dialogueContentProfile = null;

    [Header("兼容旧自言自语文案")]
    [SerializeField] private string[] selfTalkLines =
    {
        "先在这边看看。",
        "今天还挺安静。",
        "休息一下再走。",
        "嗯，这边风有点舒服。",
        "不知道大家在忙什么。"
    };

    [Header("兼容旧发起聊天文案")]
    [SerializeField] private string[] chatInitiatorLines =
    {
        "今天这边挺安静的。",
        "你也在这边歇一会儿啊。",
        "这附近风吹着还挺舒服。",
        "等会儿再继续忙吧。"
    };

    [Header("兼容旧回应聊天文案")]
    [SerializeField] private string[] chatResponderLines =
    {
        "是啊，先缓一缓。",
        "我也刚走到这儿。",
        "这边待着还挺舒服的。",
        "好，等下再去转转。"
    };

    public float MoveSpeed => moveSpeed;
    public float IdleAnimationSpeed => idleAnimationSpeed;
    public float MoveAnimationSpeed => moveAnimationSpeed;

    public float ActivityRadius => activityRadius;
    public float MinimumMoveDistance => minimumMoveDistance;
    public int PathSampleAttempts => pathSampleAttempts;

    public float ShortPauseMin => shortPauseMin;
    public float ShortPauseMax => shortPauseMax;
    public int ShortPauseCountMin => shortPauseCountMin;
    public int ShortPauseCountMax => shortPauseCountMax;

    public float LongPauseMin => longPauseMin;
    public float LongPauseMax => longPauseMax;

    public float StuckCheckInterval => stuckCheckInterval;
    public float StuckDistanceThreshold => stuckDistanceThreshold;
    public int MaxStuckRecoveries => maxStuckRecoveries;

    public bool EnableAmbientChat => enableAmbientChat;
    public float AmbientChatRadius => ambientChatRadius;
    public float AmbientChatChance => ambientChatChance;
    public float AmbientChatResponseDelay => ambientChatResponseDelay;

    public NPCDialogueContentProfile DialogueContentProfile => dialogueContentProfile;
    public string NpcId => ResolveNpcId(name);
    public string[] SelfTalkLines =>
        dialogueContentProfile != null && dialogueContentProfile.HasSelfTalkContent
            ? dialogueContentProfile.SelfTalkLines
            : selfTalkLines;
    public string[] PlayerNearbyLines =>
        dialogueContentProfile != null
            ? dialogueContentProfile.PlayerNearbyLines
            : Array.Empty<string>();
    public string[] ChatInitiatorLines =>
        dialogueContentProfile != null && dialogueContentProfile.HasAmbientChatInitiatorContent
            ? dialogueContentProfile.DefaultChatInitiatorLines
            : chatInitiatorLines;
    public string[] ChatResponderLines =>
        dialogueContentProfile != null && dialogueContentProfile.HasAmbientChatResponderContent
            ? dialogueContentProfile.DefaultChatResponderLines
            : chatResponderLines;
    public bool HasAmbientChatInitiatorContent =>
        dialogueContentProfile != null
            ? dialogueContentProfile.HasAmbientChatInitiatorContent
            : HasAnyLines(chatInitiatorLines);
    public bool HasAmbientChatResponderContent =>
        dialogueContentProfile != null
            ? dialogueContentProfile.HasAmbientChatResponderContent
            : HasAnyLines(chatResponderLines);
    public bool HasInformalConversationContent =>
        dialogueContentProfile != null && dialogueContentProfile.HasInformalConversationContent;

    public string ResolveNpcId(string fallbackName)
    {
        return dialogueContentProfile != null
            ? dialogueContentProfile.ResolveNpcId(fallbackName)
            : NPCDialogueContentProfile.NormalizeNpcId(fallbackName);
    }

    public string[] GetAmbientChatLines(string partnerNpcId, bool initiator)
    {
        if (dialogueContentProfile != null)
        {
            string[] contentLines = dialogueContentProfile.GetAmbientChatLines(partnerNpcId, initiator);
            if (HasAnyLines(contentLines))
            {
                return contentLines;
            }
        }

        return initiator ? chatInitiatorLines : chatResponderLines;
    }

    public string[] GetPlayerNearbyLines(NPCRelationshipStage relationshipStage)
    {
        if (dialogueContentProfile != null)
        {
            string[] contentLines = dialogueContentProfile.GetPlayerNearbyLines(relationshipStage);
            if (HasAnyLines(contentLines))
            {
                return contentLines;
            }
        }

        return PlayerNearbyLines;
    }

    public NPCDialogueContentProfile.InformalConversationBundle[] GetInformalConversationBundles(NPCRelationshipStage relationshipStage)
    {
        return dialogueContentProfile != null
            ? dialogueContentProfile.GetInformalConversationBundles(relationshipStage)
            : Array.Empty<NPCDialogueContentProfile.InformalConversationBundle>();
    }

    public NPCDialogueContentProfile.InformalChatInterruptReaction GetWalkAwayReaction(NPCRelationshipStage relationshipStage)
    {
        return dialogueContentProfile != null
            ? dialogueContentProfile.GetWalkAwayReaction(relationshipStage)
            : null;
    }

    public NPCDialogueContentProfile.InformalChatInterruptReaction GetInterruptReaction(
        NPCRelationshipStage relationshipStage,
        NPCInformalChatLeaveCause leaveCause,
        NPCInformalChatLeavePhase leavePhase)
    {
        return dialogueContentProfile != null
            ? dialogueContentProfile.GetInterruptReaction(relationshipStage, leaveCause, leavePhase)
            : null;
    }

    public NPCDialogueContentProfile.InformalChatResumeIntro GetResumeIntro(
        NPCRelationshipStage relationshipStage,
        NPCInformalChatLeaveCause leaveCause,
        NPCInformalChatLeavePhase leavePhase)
    {
        return dialogueContentProfile != null
            ? dialogueContentProfile.GetResumeIntro(relationshipStage, leaveCause, leavePhase)
            : null;
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        idleAnimationSpeed = Mathf.Max(0.1f, idleAnimationSpeed);
        moveAnimationSpeed = Mathf.Max(0.1f, moveAnimationSpeed);

        activityRadius = Mathf.Max(0.5f, activityRadius);
        minimumMoveDistance = Mathf.Clamp(minimumMoveDistance, 0.1f, activityRadius);
        pathSampleAttempts = Mathf.Max(1, pathSampleAttempts);

        shortPauseMin = Mathf.Max(0.05f, shortPauseMin);
        shortPauseMax = Mathf.Max(shortPauseMin, shortPauseMax);
        shortPauseCountMin = Mathf.Max(1, shortPauseCountMin);
        shortPauseCountMax = Mathf.Max(shortPauseCountMin, shortPauseCountMax);

        longPauseMin = Mathf.Max(0.1f, longPauseMin);
        longPauseMax = Mathf.Max(longPauseMin, longPauseMax);

        stuckCheckInterval = Mathf.Max(0.1f, stuckCheckInterval);
        stuckDistanceThreshold = Mathf.Max(0.01f, stuckDistanceThreshold);
        maxStuckRecoveries = Mathf.Max(1, maxStuckRecoveries);

        ambientChatRadius = Mathf.Max(0f, ambientChatRadius);
        ambientChatResponseDelay = Mathf.Max(0f, ambientChatResponseDelay);

        selfTalkLines ??= Array.Empty<string>();
        chatInitiatorLines ??= Array.Empty<string>();
        chatResponderLines ??= Array.Empty<string>();
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
}
