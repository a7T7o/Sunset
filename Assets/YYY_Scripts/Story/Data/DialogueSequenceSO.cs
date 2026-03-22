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

        [Header("剧情推进")]
        [Tooltip("该序列完成后是否解码语言。")]
        public bool markLanguageDecodedOnComplete = false;

        [Tooltip("该序列完成后是否推进剧情阶段。")]
        public bool advanceStoryPhaseOnComplete = false;

        [Tooltip("完成后切换到的剧情阶段。")]
        public StoryPhase nextStoryPhase = StoryPhase.None;

        [Tooltip("该序列完成后默认切换到的后续对话。")]
        public DialogueSequenceSO followupSequence;
    }
}
