using Sunset.Story;

namespace Sunset.Events
{
    // 对话开始事件（用于锁定玩家输入）
    public class DialogueStartEvent : IGameEvent { }

    // 对话结束事件（用于恢复玩家输入）
    public class DialogueEndEvent : IGameEvent
    {
        public string SequenceId { get; set; }
        public DialogueSequenceSO Sequence { get; set; }
        public bool WasCompleted { get; set; }
        public bool LanguageDecoded { get; set; }
        public bool LanguageDecodedChanged { get; set; }
    }

    public class DialogueSequenceCompletedEvent : IGameEvent
    {
        public string SequenceId { get; set; }
        public DialogueSequenceSO Sequence { get; set; }
        public DialogueSequenceSO FollowupSequence { get; set; }
        public bool LanguageDecoded { get; set; }
        public bool LanguageDecodedChanged { get; set; }
    }

    // 节点切换事件（UI/调试监听）
    public class DialogueNodeChangedEvent : IGameEvent
    {
        public string SequenceId { get; set; }
        public int NodeIndex { get; set; }
        public DialogueNode Node { get; set; }
    }
}
