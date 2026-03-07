using Sunset.Events;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sunset.Story
{
    /// <summary>警告：此脚本必须挂载在常驻活跃的 Canvas 根节点上，严禁挂载于可能被 SetActive(false) 的 UI 面板自身，以防止事件总线永久退订。</summary>
    public class DialogueUI : MonoBehaviour
    {
        #region 序列化字段
        [Header("UI引用")]
        [SerializeField] private GameObject root;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button continueButton;
        [SerializeField] private Image portraitImage;
        [SerializeField] private Image backgroundImage;
        #endregion

        #region 私有字段
        private DialogueManager _dialogueManager;
        private bool _isVisible = false;
        private float _originalBackgroundAlpha = 1.0f;
        #endregion

        #region Unity生命周期
        private void Awake()
        {
            ResolveValidationSceneReferences();
            _dialogueManager = DialogueManager.Instance;

            if (backgroundImage != null)
            {
                _originalBackgroundAlpha = backgroundImage.color.a;
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<DialogueStartEvent>(OnDialogueStart, owner: this);
            EventBus.Subscribe<DialogueNodeChangedEvent>(OnDialogueNodeChanged, owner: this);
            EventBus.Subscribe<DialogueEndEvent>(OnDialogueEnd, owner: this);

            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueClicked);
            }

            SetVisible(false);
        }

        private void OnDisable()
        {
            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(OnContinueClicked);
            }

            EventBus.UnsubscribeAll(this);
        }

        private void Update()
        {
            if (!_isVisible) return;

            if (_dialogueManager == null)
            {
                _dialogueManager = DialogueManager.Instance;
                if (_dialogueManager == null) return;
            }

            if (dialogueText != null)
            {
                dialogueText.text = _dialogueManager.CurrentTypedText;
            }
        }
        #endregion

        #region 事件回调
        private void OnDialogueStart(DialogueStartEvent _)
        {
            SetVisible(true);
        }

        private void OnDialogueNodeChanged(DialogueNodeChangedEvent e)
        {
            if (e.Node == null) return;

            if (e.Node.isInnerMonologue)
            {
                // 内心独白：斜体、浅灰色、隐藏名字与头像、背景半透明
                if (dialogueText != null)
                {
                    dialogueText.fontStyle = FontStyles.Italic;
                    dialogueText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                }

                if (backgroundImage != null)
                {
                    Color bgColor = backgroundImage.color;
                    bgColor.a = 0.5f;
                    backgroundImage.color = bgColor;
                }

                if (speakerNameText != null) speakerNameText.gameObject.SetActive(false);
                if (portraitImage != null) portraitImage.gameObject.SetActive(false);
            }
            else
            {
                // 普通对话：正常字体、白色、显示名字与头像、恢复原始背景透明度
                if (dialogueText != null)
                {
                    dialogueText.fontStyle = FontStyles.Normal;
                    dialogueText.color = Color.white;
                }

                if (backgroundImage != null)
                {
                    Color bgColor = backgroundImage.color;
                    bgColor.a = _originalBackgroundAlpha;
                    backgroundImage.color = bgColor;
                }

                if (speakerNameText != null)
                {
                    speakerNameText.gameObject.SetActive(true);
                    speakerNameText.text = e.Node.speakerName;
                }

                if (portraitImage != null)
                {
                    portraitImage.gameObject.SetActive(true);
                    portraitImage.sprite = e.Node.speakerPortrait;
                    portraitImage.gameObject.SetActive(portraitImage.sprite != null);
                }
            }
        }

        private void OnDialogueEnd(DialogueEndEvent _)
        {
            SetVisible(false);
        }
        #endregion

        #region UI
        private void SetVisible(bool visible)
        {
            _isVisible = visible;
            if (root != null)
            {
                root.SetActive(visible);
            }
        }

        private void OnContinueClicked()
        {
            if (_dialogueManager == null)
            {
                _dialogueManager = DialogueManager.Instance;
            }

            _dialogueManager?.AdvanceDialogue();
        }

        private void ResolveValidationSceneReferences()
        {
            if (SceneManager.GetActiveScene().name != "DialogueValidation")
            {
                return;
            }

            if (root == null)
            {
                Transform panelTransform = transform.Find("DialoguePanel");
                if (panelTransform != null)
                {
                    root = panelTransform.gameObject;
                }
            }

            if (speakerNameText == null)
            {
                Transform speakerTransform = transform.Find("SpeakerNameText");
                if (speakerTransform != null)
                {
                    speakerNameText = speakerTransform.GetComponent<TextMeshProUGUI>();
                }
            }

            if (dialogueText == null)
            {
                Transform dialogueTransform = transform.Find("DialogueText");
                if (dialogueTransform != null)
                {
                    dialogueText = dialogueTransform.GetComponent<TextMeshProUGUI>();
                }
            }

            if (continueButton == null)
            {
                Transform buttonTransform = transform.Find("ContinueButton");
                if (buttonTransform != null)
                {
                    continueButton = buttonTransform.GetComponent<Button>();
                }
            }

            if (portraitImage == null)
            {
                Transform portraitTransform = transform.Find("PortraitImage");
                if (portraitTransform != null)
                {
                    portraitImage = portraitTransform.GetComponent<Image>();
                }
            }

            if (backgroundImage == null)
            {
                Transform backgroundTransform = transform.Find("Background");
                if (backgroundTransform != null)
                {
                    backgroundImage = backgroundTransform.GetComponent<Image>();
                }
            }
        }
        #endregion
    }
}
