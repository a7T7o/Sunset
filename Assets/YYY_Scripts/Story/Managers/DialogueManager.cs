using System.Collections;
using System.Text;
using Sunset.Events;
using UnityEngine;

namespace Sunset.Story
{
    public class DialogueManager : MonoBehaviour
    {
        #region 静态成员
        public static DialogueManager Instance { get; private set; }
        #endregion

        #region 私有字段
        private readonly StringBuilder _textBuilder = new StringBuilder(512);
        private Coroutine _typingCoroutine;

        private DialogueSequenceSO _currentSequence;
        private int _currentNodeIndex = -1;
        private bool _isNodeTyping = false;
        private bool _advanceRequested = false;
        #endregion

        #region 公共属性
        public bool IsDialogueActive { get; private set; }
        public string CurrentTypedText { get; private set; } = string.Empty;
        public DialogueNode CurrentNode { get; private set; }
        [field: SerializeField]
        [field: Tooltip("临时状态：玩家是否已解锁当前语言。未来由全局变量系统接管。")]
        public bool IsLanguageDecoded { get; set; } = false;
        #endregion

        #region Unity生命周期
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        #endregion

        #region 公共方法
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

            EventBus.Publish(new DialogueStartEvent());
            AdvanceDialogue();
        }

        public void AdvanceDialogue()
        {
            if (!IsDialogueActive) return;

            if (_isNodeTyping)
            {
                _advanceRequested = true;
                return;
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

        public void StopDialogue()
        {
            StopTyping();

            if (IsDialogueActive)
            {
                IsDialogueActive = false;
                _currentSequence = null;
                _currentNodeIndex = -1;
                CurrentNode = null;
                CurrentTypedText = string.Empty;

                EventBus.Publish(new DialogueEndEvent());
            }
        }
        #endregion

        #region 私有方法
        private void StartTypingCurrentNode()
        {
            StopTyping();

            if (CurrentNode == null)
            {
                _isNodeTyping = false;
                return;
            }

            float cps = (CurrentNode.typingSpeedOverride > 0f)
                ? CurrentNode.typingSpeedOverride
                : (_currentSequence != null ? _currentSequence.defaultTypingSpeed : 30f);

            string targetText = (CurrentNode.isGarbled && !IsLanguageDecoded)
                ? CurrentNode.garbledText
                : CurrentNode.text;

            _typingCoroutine = StartCoroutine(TypeNodeText(targetText, cps));
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
                yield break;
            }

            float interval = 1f / Mathf.Max(charsPerSecond, 1f);
            var wait = new WaitForSeconds(interval);

            for (int i = 0; i < fullText.Length; i++)
            {
                if (_advanceRequested)
                {
                    _textBuilder.Clear();
                    _textBuilder.Append(fullText);
                    CurrentTypedText = _textBuilder.ToString();
                    _advanceRequested = false;
                    break;
                }

                _textBuilder.Append(fullText[i]);
                CurrentTypedText = _textBuilder.ToString();
                yield return wait;
            }

            _isNodeTyping = false;
        }
        #endregion
    }
}
