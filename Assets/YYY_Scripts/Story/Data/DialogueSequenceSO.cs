using System.Collections.Generic;
using UnityEngine;

namespace Sunset.Story
{
    [CreateAssetMenu(fileName = "NewDialogueSequence", menuName = "Story/Dialogue Sequence")]
    public class DialogueSequenceSO : ScriptableObject
    {
        [Header("对话元信息")]
        public string sequenceId;

        [Header("节点列表（按顺序播放）")]
        public List<DialogueNode> nodes = new();

        [Header("序列配置")]
        public bool canSkip = true;

        [Tooltip("字/秒")]
        public float defaultTypingSpeed = 30f;

        [Header("Progression")]
        [Tooltip("Unlock readable language after this sequence completes.")]
        public bool markLanguageDecodedOnComplete = false;

        [Tooltip("Optional follow-up sequence to use after this sequence has been completed.")]
        public DialogueSequenceSO followupSequence;
    }
}
