using Sunset.Story;

namespace Sunset.Events
{
    public class DialogueStartEvent : IGameEvent { }

    public class DialogueEndEvent : IGameEvent
    {
        public string SequenceId { get; set; }
        public DialogueSequenceSO Sequence { get; set; }
        public bool WasCompleted { get; set; }
        public bool LanguageDecoded { get; set; }
        public bool LanguageDecodedChanged { get; set; }
        public StoryPhase PreviousPhase { get; set; }
        public StoryPhase CurrentPhase { get; set; }
        public bool StoryPhaseChanged { get; set; }
    }

    public class DialogueSequenceCompletedEvent : IGameEvent
    {
        public string SequenceId { get; set; }
        public DialogueSequenceSO Sequence { get; set; }
        public DialogueSequenceSO FollowupSequence { get; set; }
        public bool LanguageDecoded { get; set; }
        public bool LanguageDecodedChanged { get; set; }
        public StoryPhase PreviousPhase { get; set; }
        public StoryPhase CurrentPhase { get; set; }
        public bool StoryPhaseChanged { get; set; }
    }

    public class DialogueNodeChangedEvent : IGameEvent
    {
        public string SequenceId { get; set; }
        public int NodeIndex { get; set; }
        public DialogueNode Node { get; set; }
    }

    public class StoryPhaseChangedEvent : IGameEvent
    {
        public StoryPhase PreviousPhase { get; set; }
        public StoryPhase CurrentPhase { get; set; }
    }
}
