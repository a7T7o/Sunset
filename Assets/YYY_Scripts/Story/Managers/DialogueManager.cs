using System.Collections;
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

        [field: SerializeField]
        [field: Tooltip("临时状态：玩家是否已解锁当前语言。未来由全局变量系统接管。")]
        public bool IsLanguageDecoded { get; set; } = false;
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
                StopDialogue();
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

        public void StopDialogue()
        {
            StopTyping();

            if (!IsDialogueActive)
            {
                return;
            }

            IsDialogueActive = false;
            _currentSequence = null;
            _currentNodeIndex = -1;
            CurrentNode = null;
            CurrentTypedText = string.Empty;

            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.ResumeTime(DialoguePauseSource);
            }

            EventBus.Publish(new DialogueEndEvent());
        }
        #endregion

        #region Private Methods
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
