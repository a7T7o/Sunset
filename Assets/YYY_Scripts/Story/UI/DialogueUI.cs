using Sunset.Events;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sunset.Story
{
    public class DialogueUI : MonoBehaviour
    {
        #region Serialized Fields
        [Header("UI References")]
        [SerializeField] private GameObject root;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button continueButton;
        [SerializeField] private Image portraitImage;
        [SerializeField] private Image backgroundImage;

        [Header("Font Settings")]
        [SerializeField] private DialogueFontLibrarySO fontLibrary;
        [SerializeField] private string defaultFontKey = "default";
        [SerializeField] private string speakerNameFontKey = "speaker_name";
        [SerializeField] private string innerMonologueFontKey = "inner_monologue";
        [SerializeField] private string garbledFontKey = "garbled";
        #endregion

        #region Private Fields
        private DialogueManager _dialogueManager;
        private bool _isVisible;
        private float _originalBackgroundAlpha = 1.0f;
        private float _dialogueBaseFontSize;
        private float _speakerBaseFontSize;
        private float _dialogueBaseLineSpacing;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            ResolveValidationSceneReferences();
            _dialogueManager = DialogueManager.Instance;

            if (backgroundImage != null)
            {
                _originalBackgroundAlpha = backgroundImage.color.a;
            }

            if (dialogueText != null)
            {
                _dialogueBaseFontSize = dialogueText.fontSize;
                _dialogueBaseLineSpacing = dialogueText.lineSpacing;
            }

            if (speakerNameText != null)
            {
                _speakerBaseFontSize = speakerNameText.fontSize;
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
            if (!_isVisible)
            {
                return;
            }

            if (_dialogueManager == null)
            {
                _dialogueManager = DialogueManager.Instance;
                if (_dialogueManager == null)
                {
                    return;
                }
            }

            if (dialogueText != null)
            {
                dialogueText.text = _dialogueManager.CurrentTypedText;
            }
        }
        #endregion

        #region Public Methods
        public void ApplyFontStyle(string fontStyleKey)
        {
            ApplyFontStyle(fontStyleKey, speakerNameFontKey);
        }
        #endregion

        #region Event Handlers
        private void OnDialogueStart(DialogueStartEvent _)
        {
            SetVisible(true);
        }

        private void OnDialogueNodeChanged(DialogueNodeChangedEvent eventData)
        {
            if (eventData.Node == null)
            {
                return;
            }

            ApplyFontPresetForNode(eventData.Node);

            if (eventData.Node.isInnerMonologue)
            {
                if (dialogueText != null)
                {
                    dialogueText.fontStyle = FontStyles.Italic;
                    dialogueText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                }

                if (backgroundImage != null)
                {
                    Color backgroundColor = backgroundImage.color;
                    backgroundColor.a = 0.5f;
                    backgroundImage.color = backgroundColor;
                }

                if (speakerNameText != null)
                {
                    speakerNameText.gameObject.SetActive(false);
                }

                if (portraitImage != null)
                {
                    portraitImage.gameObject.SetActive(false);
                }
            }
            else
            {
                if (dialogueText != null)
                {
                    dialogueText.fontStyle = FontStyles.Normal;
                    dialogueText.color = Color.white;
                }

                if (backgroundImage != null)
                {
                    Color backgroundColor = backgroundImage.color;
                    backgroundColor.a = _originalBackgroundAlpha;
                    backgroundImage.color = backgroundColor;
                }

                if (speakerNameText != null)
                {
                    speakerNameText.gameObject.SetActive(true);
                    speakerNameText.text = eventData.Node.speakerName;
                }

                if (portraitImage != null)
                {
                    portraitImage.gameObject.SetActive(true);
                    portraitImage.sprite = eventData.Node.speakerPortrait;
                    portraitImage.gameObject.SetActive(portraitImage.sprite != null);
                }
            }
        }

        private void OnDialogueEnd(DialogueEndEvent _)
        {
            SetVisible(false);
        }
        #endregion

        #region Private Methods
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

        private void ApplyFontPresetForNode(DialogueNode node)
        {
            if (node == null)
            {
                ApplyFontStyle(defaultFontKey, speakerNameFontKey);
                return;
            }

            if (!string.IsNullOrWhiteSpace(node.fontStyleKey))
            {
                ApplyFontStyle(node.fontStyleKey, node.fontStyleKey);
                return;
            }

            if (node.isGarbled && _dialogueManager != null && !_dialogueManager.IsLanguageDecoded)
            {
                ApplyFontStyle(garbledFontKey, speakerNameFontKey);
                return;
            }

            if (node.isInnerMonologue)
            {
                ApplyFontStyle(innerMonologueFontKey, speakerNameFontKey);
                return;
            }

            ApplyFontStyle(defaultFontKey, speakerNameFontKey);
        }

        private void ApplyFontStyle(string dialogueFontKey, string speakerFontKey)
        {
            if (fontLibrary == null)
            {
                return;
            }

            ApplyFontToText(dialogueText, dialogueFontKey, _dialogueBaseFontSize, _dialogueBaseLineSpacing);
            ApplyFontToText(speakerNameText, speakerFontKey, _speakerBaseFontSize, 0f);
        }

        private void ApplyFontToText(TextMeshProUGUI target, string key, float baseFontSize, float baseLineSpacing)
        {
            if (target == null)
            {
                return;
            }

            if (!fontLibrary.TryGetEntry(key, out DialogueFontLibrarySO.FontEntry entry) || entry == null)
            {
                return;
            }

            if (entry.fontAsset != null)
            {
                target.font = entry.fontAsset;
            }

            if (baseFontSize > 0f)
            {
                target.fontSize = baseFontSize + entry.fontSizeOffset;
            }

            target.lineSpacing = baseLineSpacing + entry.lineSpacingOffset;
        }
        #endregion
    }
}
