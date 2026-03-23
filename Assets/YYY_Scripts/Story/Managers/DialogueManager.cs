using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sunset.Events;
using UnityEngine;

namespace Sunset.Story
{
    public class DialogueManager : MonoBehaviour
    {
        #region Static Members
        private const string DialoguePauseSource = "Dialogue";
        public static DialogueManager Instance { get; private set; }
        #endregion

        #region Serialized Fields
        [SerializeField] private bool useInspectorTypingSpeedOverride = false;
        [SerializeField] private float inspectorCharsPerSecond = 30f;
        #endregion

        #region Private Fields
        private readonly StringBuilder _textBuilder = new StringBuilder(512);
        private readonly HashSet<string> _completedSequenceIds = new HashSet<string>();
        private Coroutine _typingCoroutine;

        private DialogueSequenceSO _currentSequence;
        private int _currentNodeIndex = -1;
        private bool _isNodeTyping = false;
        private bool _advanceRequested = false;
        #endregion

        #region Public Properties
        public bool IsDialogueActive { get; private set; }
        public string CurrentTypedText { get; private set; } = string.Empty;
        public DialogueNode CurrentNode { get; private set; }
        public bool IsNodeTyping => _isNodeTyping;
        public string CurrentSequenceId => _currentSequence != null ? _currentSequence.sequenceId : string.Empty;
        public int CurrentNodeIndex => _currentNodeIndex;
        public int CurrentNodeCount => _currentSequence != null && _currentSequence.nodes != null ? _currentSequence.nodes.Count : 0;

        public bool IsLanguageDecoded
        {
            get => StoryManager.Instance.IsLanguageDecoded;
            set => StoryManager.Instance.SetLanguageDecoded(value);
        }
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        #endregion

        #region Public Methods
        public void PlayDialogue(DialogueSequenceSO sequence)
        {
            if (sequence == null || sequence.nodes == null || sequence.nodes.Count == 0)
            {
                Debug.LogError("[DialogueManager] PlayDialogue: sequence is null or empty");
                return;
            }

            StopDialogue();

            _currentSequence = sequence;
            _currentNodeIndex = -1;
            IsDialogueActive = true;

            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.PauseTime(DialoguePauseSource);
            }

            EventBus.Publish(new DialogueStartEvent());
            AdvanceDialogue();
        }

        public void AdvanceDialogue()
        {
            if (!IsDialogueActive)
            {
                return;
            }

            if (_isNodeTyping)
            {
                string targetText = ResolveNodeDisplayText(CurrentNode);
                if (!string.IsNullOrEmpty(targetText) && CurrentTypedText == targetText)
                {
                    StopTyping();
                }
                else
                {
                    _advanceRequested = true;
                    return;
                }
            }

            _currentNodeIndex++;

            if (_currentSequence == null || _currentSequence.nodes == null || _currentNodeIndex >= _currentSequence.nodes.Count)
            {
                CompleteCurrentSequence();
                return;
            }

            CurrentNode = _currentSequence.nodes[_currentNodeIndex];
            CurrentTypedText = string.Empty;

            EventBus.Publish(new DialogueNodeChangedEvent
            {
                SequenceId = _currentSequence.sequenceId,
                NodeIndex = _currentNodeIndex,
                Node = CurrentNode
            });

            StartTypingCurrentNode();
        }

        public void CompleteCurrentNodeImmediately()
        {
            if (!IsDialogueActive || !_isNodeTyping || CurrentNode == null)
            {
                return;
            }

            StopTyping();
            CurrentTypedText = ResolveNodeDisplayText(CurrentNode);
        }

        public void ForceCompleteOrAdvance()
        {
            if (!IsDialogueActive)
            {
                return;
            }

            if (_isNodeTyping)
            {
                CompleteCurrentNodeImmediately();
                return;
            }

            AdvanceDialogue();
        }

        public bool HasCompletedSequence(string sequenceId)
        {
            return !string.IsNullOrWhiteSpace(sequenceId) && _completedSequenceIds.Contains(sequenceId);
        }

        public void StopDialogue()
        {
            StopDialogueInternal(false, null, false, StoryManager.Instance.CurrentPhase, StoryManager.Instance.CurrentPhase, false);
        }
        #endregion

        #region Private Methods
        private void CompleteCurrentSequence()
        {
            DialogueSequenceSO completedSequence = _currentSequence;
            StoryManager storyManager = StoryManager.Instance;

            if (completedSequence == null)
            {
                StopDialogueInternal(false, null, false, storyManager.CurrentPhase, storyManager.CurrentPhase, false);
                return;
            }

            if (!string.IsNullOrWhiteSpace(completedSequence.sequenceId))
            {
                _completedSequenceIds.Add(completedSequence.sequenceId);
            }

            StoryPhase previousPhase = storyManager.CurrentPhase;
            bool wasDecodedBefore = storyManager.IsLanguageDecoded;

            if (completedSequence.markLanguageDecodedOnComplete)
            {
                storyManager.SetLanguageDecoded(true);
            }

            bool storyPhaseChanged = false;
            if (completedSequence.advanceStoryPhaseOnComplete && completedSequence.nextStoryPhase != StoryPhase.None)
            {
                storyPhaseChanged = storyManager.SetPhase(completedSequence.nextStoryPhase);
            }

            bool languageDecodedChanged = storyManager.IsLanguageDecoded != wasDecodedBefore;

            StopDialogueInternal(
                true,
                completedSequence,
                languageDecodedChanged,
                previousPhase,
                storyManager.CurrentPhase,
                storyPhaseChanged);
        }

        private void StopDialogueInternal(
            bool wasCompleted,
            DialogueSequenceSO endingSequenceOverride,
            bool languageDecodedChanged,
            StoryPhase previousPhase,
            StoryPhase currentPhase,
            bool storyPhaseChanged)
        {
            StopTyping();

            if (!IsDialogueActive)
            {
                return;
            }

            DialogueSequenceSO endingSequence = endingSequenceOverride ?? _currentSequence;
            string endingSequenceId = endingSequence != null ? endingSequence.sequenceId : string.Empty;

            IsDialogueActive = false;
            _currentSequence = null;
            _currentNodeIndex = -1;
            CurrentNode = null;
            CurrentTypedText = string.Empty;

            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.ResumeTime(DialoguePauseSource);
            }

            if (wasCompleted && endingSequence != null)
            {
                EventBus.Publish(new DialogueSequenceCompletedEvent
                {
                    SequenceId = endingSequenceId,
                    Sequence = endingSequence,
                    FollowupSequence = endingSequence.followupSequence,
                    LanguageDecoded = StoryManager.Instance.IsLanguageDecoded,
                    LanguageDecodedChanged = languageDecodedChanged,
                    PreviousPhase = previousPhase,
                    CurrentPhase = currentPhase,
                    StoryPhaseChanged = storyPhaseChanged
                });
            }

            EventBus.Publish(new DialogueEndEvent
            {
                SequenceId = endingSequenceId,
                Sequence = endingSequence,
                WasCompleted = wasCompleted,
                LanguageDecoded = StoryManager.Instance.IsLanguageDecoded,
                LanguageDecodedChanged = languageDecodedChanged,
                PreviousPhase = previousPhase,
                CurrentPhase = currentPhase,
                StoryPhaseChanged = storyPhaseChanged
            });
        }

        private void StartTypingCurrentNode()
        {
            StopTyping();

            if (CurrentNode == null)
            {
                _isNodeTyping = false;
                return;
            }

            float charsPerSecond = ResolveTypingSpeed();
            string targetText = ResolveNodeDisplayText(CurrentNode);

            _typingCoroutine = StartCoroutine(TypeNodeText(targetText, charsPerSecond));
        }

        private string ResolveNodeDisplayText(DialogueNode node)
        {
            if (node == null)
            {
                return string.Empty;
            }

            return (node.isGarbled && !IsLanguageDecoded)
                ? node.garbledText
                : node.text;
        }

        private float ResolveTypingSpeed()
        {
            if (CurrentNode != null && CurrentNode.typingSpeedOverride > 0f)
            {
                return CurrentNode.typingSpeedOverride;
            }

            if (useInspectorTypingSpeedOverride && inspectorCharsPerSecond > 0f)
            {
                return inspectorCharsPerSecond;
            }

            return _currentSequence != null ? _currentSequence.defaultTypingSpeed : 30f;
        }

        private void StopTyping()
        {
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
                _typingCoroutine = null;
            }

            _isNodeTyping = false;
            _advanceRequested = false;
        }

        private IEnumerator TypeNodeText(string fullText, float charsPerSecond)
        {
            _isNodeTyping = true;
            _advanceRequested = false;
            _textBuilder.Clear();

            if (string.IsNullOrEmpty(fullText))
            {
                CurrentTypedText = string.Empty;
                _isNodeTyping = false;
                _typingCoroutine = null;
                yield break;
            }

            float secondsPerChar = 1f / Mathf.Max(charsPerSecond, 1f);
            float accumulatedTime = secondsPerChar;
            int nextCharacterIndex = 0;

            while (nextCharacterIndex < fullText.Length)
            {
                if (_advanceRequested)
                {
                    _textBuilder.Clear();
                    _textBuilder.Append(fullText);
                    CurrentTypedText = fullText;
                    _advanceRequested = false;
                    break;
                }

                accumulatedTime += Time.unscaledDeltaTime;
                bool hasUpdatedText = false;

                while (accumulatedTime >= secondsPerChar && nextCharacterIndex < fullText.Length)
                {
                    _textBuilder.Append(fullText[nextCharacterIndex]);
                    nextCharacterIndex++;
                    accumulatedTime -= secondsPerChar;
                    hasUpdatedText = true;
                }

                if (hasUpdatedText)
                {
                    CurrentTypedText = _textBuilder.ToString();
                }

                yield return null;
            }

            if (CurrentTypedText != fullText)
            {
                CurrentTypedText = fullText;
            }

            _isNodeTyping = false;
            _typingCoroutine = null;
        }
        #endregion
    }
}
