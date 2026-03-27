using System;
using System.Text;
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

    [Header("身份标识")]
    [SerializeField] private string npcId = string.Empty;

    [Header("单人环境气泡")]
    [SerializeField] private string[] selfTalkLines = Array.Empty<string>();

    [Header("玩家轻响应")]
    [SerializeField] private string[] playerNearbyLines = Array.Empty<string>();

    [Header("按关系阶段分流的玩家轻响应")]
    [SerializeField] private RelationshipStageNearbySet[] relationshipStageNearbyLines = Array.Empty<RelationshipStageNearbySet>();

    [Header("通用相遇发起")]
    [SerializeField] private string[] defaultChatInitiatorLines = Array.Empty<string>();

    [Header("通用相遇回应")]
    [SerializeField] private string[] defaultChatResponderLines = Array.Empty<string>();

    [Header("按搭档区分的相遇矩阵")]
    [SerializeField] private PairDialogueSet[] pairDialogueSets = Array.Empty<PairDialogueSet>();

    public string NpcId => ResolveNpcId();
    public string[] SelfTalkLines => selfTalkLines ?? Array.Empty<string>();
    public string[] PlayerNearbyLines => playerNearbyLines ?? Array.Empty<string>();
    public RelationshipStageNearbySet[] RelationshipStageNearbyLines => relationshipStageNearbyLines ?? Array.Empty<RelationshipStageNearbySet>();
    public string[] DefaultChatInitiatorLines => defaultChatInitiatorLines ?? Array.Empty<string>();
    public string[] DefaultChatResponderLines => defaultChatResponderLines ?? Array.Empty<string>();
    public PairDialogueSet[] PairDialogueSets => pairDialogueSets ?? Array.Empty<PairDialogueSet>();
    public bool HasSelfTalkContent => HasAnyLines(SelfTalkLines);
    public bool HasPlayerNearbyContent => HasAnyLines(PlayerNearbyLines) || HasAnyStageNearbyLines();
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

    public string[] GetPlayerNearbyLines(NPCRelationshipStage relationshipStage)
    {
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
        playerNearbyLines ??= Array.Empty<string>();
        relationshipStageNearbyLines ??= Array.Empty<RelationshipStageNearbySet>();
        defaultChatInitiatorLines ??= Array.Empty<string>();
        defaultChatResponderLines ??= Array.Empty<string>();
        pairDialogueSets ??= Array.Empty<PairDialogueSet>();

        for (int index = 0; index < relationshipStageNearbyLines.Length; index++)
        {
            relationshipStageNearbyLines[index]?.Sanitize();
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
