using System.Collections;
using Sunset.Events;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Font Settings")]
        [SerializeField] private DialogueFontLibrarySO fontLibrary;
        [SerializeField] private string defaultFontKey = "default";
        [SerializeField] private string speakerNameFontKey = "speaker_name";
        [SerializeField] private string innerMonologueFontKey = "inner_monologue";
        [SerializeField] private string garbledFontKey = "garbled";

        [Header("Transition Settings")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.2f;

        [Header("Advance Settings")]
        [SerializeField] private bool enableAnyKeyAdvance = true;
        [SerializeField] private bool enablePointerClickAdvance = true;
        [SerializeField] private float anyKeyAdvanceDebounce = 0.15f;

        [Header("Portrait Settings")]
        [SerializeField] private bool usePlaceholderPortraitWhenMissing = true;
        [SerializeField] private Color placeholderPortraitColor = new Color(0.55f, 0.55f, 0.55f, 1f);
        #endregion

        #region Private Fields
        private DialogueManager _dialogueManager;
        private Coroutine _fadeCoroutine;
        private bool _isVisible;
        private float _advanceInputReadyTime = float.PositiveInfinity;
        private float _originalBackgroundAlpha = 1.0f;
        private float _dialogueBaseFontSize;
        private float _speakerBaseFontSize;
        private float _dialogueBaseLineSpacing;
        private Color _dialogueBaseColor = Color.white;
        private FontStyles _dialogueBaseFontStyle = FontStyles.Normal;
        private Sprite _placeholderPortraitSprite;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            ResolveReferences();
            EnsureCanvasGroup();
            ResolvePortraitTarget();
            ConfigureInteractionSurfaces();
            CacheBasePresentation();
            InitializeHiddenState();
            _dialogueManager = DialogueManager.Instance;
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

            InitializeHiddenState();
        }

        private void OnDisable()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }

            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(OnContinueClicked);
            }

            EventBus.UnsubscribeAll(this);
        }

        private void OnDestroy()
        {
            if (_placeholderPortraitSprite != null)
            {
                Destroy(_placeholderPortraitSprite.texture);
                Destroy(_placeholderPortraitSprite);
                _placeholderPortraitSprite = null;
            }
        }

        private void Update()
        {
            if (_dialogueManager == null)
            {
                _dialogueManager = DialogueManager.Instance;
            }

            if (_isVisible && _dialogueManager != null && dialogueText != null)
            {
                if (dialogueText.text != _dialogueManager.CurrentTypedText)
                {
                    dialogueText.text = _dialogueManager.CurrentTypedText;
                }
            }

            if (!_isVisible ||
                !enableAnyKeyAdvance ||
                Time.unscaledTime < _advanceInputReadyTime ||
                _dialogueManager == null ||
                !_dialogueManager.IsDialogueActive)
            {
                return;
            }

            if (IsAdvanceInputPressed())
            {
                _advanceInputReadyTime = Time.unscaledTime + 0.05f;
                OnContinueClicked();
            }
        }
        #endregion

        #region Public Methods
        public bool IsVisible => _isVisible;
        public string CurrentSpeakerName => speakerNameText != null ? speakerNameText.text : string.Empty;
        public string CurrentDialogueText => dialogueText != null ? dialogueText.text : string.Empty;
        public bool IsSpeakerVisible => speakerNameText != null && speakerNameText.gameObject.activeInHierarchy;
        public bool IsPortraitVisible => portraitImage != null && portraitImage.enabled && portraitImage.gameObject.activeInHierarchy;
        public string CurrentPortraitSpriteName => portraitImage != null && portraitImage.sprite != null ? portraitImage.sprite.name : string.Empty;
        public Color CurrentPortraitColor => portraitImage != null ? portraitImage.color : Color.clear;
        public bool IsContinueButtonInteractable => continueButton != null && continueButton.interactable;
        public float CurrentCanvasAlpha => canvasGroup != null ? canvasGroup.alpha : 0f;
        public bool IsCanvasInteractable => canvasGroup != null && canvasGroup.interactable;
        public bool IsCanvasBlockingRaycasts => canvasGroup != null && canvasGroup.blocksRaycasts;
        public string CurrentDialogueFontName => dialogueText != null && dialogueText.font != null ? dialogueText.font.name : string.Empty;
        public string CurrentSpeakerFontName => speakerNameText != null && speakerNameText.font != null ? speakerNameText.font.name : string.Empty;
        public FontStyles CurrentDialogueFontStyle => dialogueText != null ? dialogueText.fontStyle : FontStyles.Normal;

        public void ApplyFontStyle(string fontStyleKey)
        {
            ApplyFontStyle(fontStyleKey, speakerNameFontKey);
        }
        #endregion

        #region Event Handlers
        private void OnDialogueStart(DialogueStartEvent _)
        {
            _isVisible = true;
            _advanceInputReadyTime = Time.unscaledTime + Mathf.Max(0f, anyKeyAdvanceDebounce);
            StartVisibilityTransition(true);
        }

        private void OnDialogueNodeChanged(DialogueNodeChangedEvent eventData)
        {
            if (eventData.Node == null)
            {
                return;
            }

            RestoreDefaultPresentation();
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
                    speakerNameText.text = string.Empty;
                }

                if (portraitImage != null)
                {
                    portraitImage.gameObject.SetActive(false);
                }
            }
            else
            {
                if (speakerNameText != null)
                {
                    speakerNameText.gameObject.SetActive(true);
                    speakerNameText.text = eventData.Node.speakerName;
                }

                ApplyPortrait(eventData.Node.speakerPortrait);
            }
        }

        private void OnDialogueEnd(DialogueEndEvent _)
        {
            _isVisible = false;
            _advanceInputReadyTime = float.PositiveInfinity;
            StartVisibilityTransition(false);
        }
        #endregion

        #region Private Methods
        private void ResolveReferences()
        {
            if (root == null)
            {
                Transform panelTransform = FindChild("DialoguePanel");
                if (panelTransform != null)
                {
                    root = panelTransform.gameObject;
                }
            }

            if (speakerNameText == null)
            {
                Transform speakerTransform = FindChild("SpeakerNameText");
                if (speakerTransform != null)
                {
                    speakerNameText = speakerTransform.GetComponent<TextMeshProUGUI>();
                }
            }

            if (dialogueText == null)
            {
                Transform dialogueTransform = FindChild("DialogueText");
                if (dialogueTransform != null)
                {
                    dialogueText = dialogueTransform.GetComponent<TextMeshProUGUI>();
                }
            }

            if (continueButton == null)
            {
                Transform buttonTransform = FindChild("ContinueButton");
                if (buttonTransform != null)
                {
                    continueButton = buttonTransform.GetComponent<Button>();
                }
            }

            if (backgroundImage == null)
            {
                Transform backgroundTransform = FindChild("DialoguePanel/Background") ?? FindChild("Background");
                if (backgroundTransform != null)
                {
                    backgroundImage = backgroundTransform.GetComponent<Image>();
                }
            }

            if (portraitImage == null)
            {
                Transform portraitTransform = FindChild("Avatar/Icon") ?? FindChild("PortraitImage") ?? FindChild("Avatar");
                if (portraitTransform != null)
                {
                    portraitImage = EnsureImage(portraitTransform);
                }
            }
        }

        private void ResolvePortraitTarget()
        {
            Transform iconTransform = FindChild("Avatar/Icon");
            if (iconTransform != null)
            {
                portraitImage = EnsureImage(iconTransform);
            }
            else if (portraitImage != null)
            {
                portraitImage = EnsureImage(portraitImage.transform);
            }
        }

        private void EnsureCanvasGroup()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void ConfigureInteractionSurfaces()
        {
            SetRaycastTarget(backgroundImage, false);
            SetRaycastTarget(speakerNameText, false);
            SetRaycastTarget(dialogueText, false);
            SetRaycastTarget(portraitImage, false);

            Transform avatarTransform = FindChild("Avatar");
            if (avatarTransform != null)
            {
                SetRaycastTarget(avatarTransform.GetComponent<Image>(), false);
            }

            if (continueButton != null)
            {
                if (continueButton.targetGraphic != null)
                {
                    continueButton.targetGraphic.raycastTarget = true;
                }

                TextMeshProUGUI buttonLabel = continueButton.GetComponentInChildren<TextMeshProUGUI>(true);
                if (buttonLabel != null && buttonLabel.transform != continueButton.transform)
                {
                    buttonLabel.raycastTarget = false;
                }
            }
        }

        private void CacheBasePresentation()
        {
            if (backgroundImage != null)
            {
                _originalBackgroundAlpha = backgroundImage.color.a;
            }

            if (dialogueText != null)
            {
                _dialogueBaseFontSize = dialogueText.fontSize;
                _dialogueBaseLineSpacing = dialogueText.lineSpacing;
                _dialogueBaseColor = dialogueText.color;
                _dialogueBaseFontStyle = dialogueText.fontStyle;
            }

            if (speakerNameText != null)
            {
                _speakerBaseFontSize = speakerNameText.fontSize;
            }
        }

        private void InitializeHiddenState()
        {
            SetCanvasState(0f, false, false);
            ClearContinueButtonSelection();
            ClearVisibleContent();
        }

        private void RestoreDefaultPresentation()
        {
            if (dialogueText != null)
            {
                dialogueText.fontStyle = _dialogueBaseFontStyle;
                dialogueText.color = _dialogueBaseColor;
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
            }

            if (portraitImage != null)
            {
                portraitImage.gameObject.SetActive(true);
            }
        }

        private void ClearVisibleContent()
        {
            if (dialogueText != null)
            {
                dialogueText.text = string.Empty;
                dialogueText.fontStyle = _dialogueBaseFontStyle;
                dialogueText.color = _dialogueBaseColor;
            }

            if (speakerNameText != null)
            {
                speakerNameText.text = string.Empty;
                speakerNameText.gameObject.SetActive(false);
            }

            if (portraitImage != null)
            {
                portraitImage.sprite = null;
                portraitImage.color = Color.white;
                portraitImage.gameObject.SetActive(false);
            }
        }

        private void StartVisibilityTransition(bool visible)
        {
            if (canvasGroup == null)
            {
                EnsureCanvasGroup();
            }

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }

            float from = canvasGroup != null ? canvasGroup.alpha : (visible ? 0f : 1f);
            float to = visible ? 1f : 0f;
            float duration = visible ? Mathf.Max(0f, fadeInDuration) : Mathf.Max(0f, fadeOutDuration);

            _fadeCoroutine = StartCoroutine(FadeCanvas(from, to, duration, visible));
        }

        private IEnumerator FadeCanvas(float from, float to, float duration, bool finalVisible)
        {
            if (canvasGroup == null)
            {
                yield break;
            }

            SetCanvasState(from, finalVisible, finalVisible);
            UpdateContinueButtonSelection(finalVisible);

            if (duration <= 0f)
            {
                SetCanvasState(to, finalVisible, finalVisible);
                UpdateContinueButtonSelection(finalVisible);
                if (!finalVisible)
                {
                    ClearVisibleContent();
                }
                _fadeCoroutine = null;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float normalized = Mathf.Clamp01(elapsed / duration);
                float eased = finalVisible
                    ? 1f - ((1f - normalized) * (1f - normalized))
                    : normalized * normalized;

                SetCanvasState(Mathf.Lerp(from, to, eased), finalVisible, finalVisible);

                if (normalized >= 1f)
                {
                    break;
                }

                yield return null;
            }

            SetCanvasState(to, finalVisible, finalVisible);
            UpdateContinueButtonSelection(finalVisible);
            if (!finalVisible)
            {
                ClearVisibleContent();
            }

            _fadeCoroutine = null;
        }

        private void SetCanvasState(float alpha, bool interactable, bool blockRaycasts)
        {
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = alpha;
            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = blockRaycasts;

            if (continueButton != null)
            {
                continueButton.interactable = interactable;
            }
        }

        private void ApplyPortrait(Sprite speakerPortrait)
        {
            if (portraitImage == null)
            {
                return;
            }

            Sprite resolvedPortrait = speakerPortrait;
            Color resolvedColor = Color.white;

            if (resolvedPortrait == null && usePlaceholderPortraitWhenMissing)
            {
                resolvedPortrait = GetOrCreatePlaceholderPortrait();
                resolvedColor = placeholderPortraitColor;
            }

            portraitImage.sprite = resolvedPortrait;
            portraitImage.color = resolvedColor;
            portraitImage.preserveAspect = true;
            portraitImage.enabled = resolvedPortrait != null;
            portraitImage.gameObject.SetActive(resolvedPortrait != null);
        }

        private Sprite GetOrCreatePlaceholderPortrait()
        {
            if (_placeholderPortraitSprite != null)
            {
                return _placeholderPortraitSprite;
            }

            const int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[size * size];
            for (int index = 0; index < pixels.Length; index++)
            {
                pixels[index] = Color.white;
            }

            texture.SetPixels(pixels);
            texture.Apply();

            _placeholderPortraitSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f));
            _placeholderPortraitSprite.name = "DialoguePlaceholderPortrait";
            return _placeholderPortraitSprite;
        }

        private void OnContinueClicked()
        {
            if (_dialogueManager == null)
            {
                _dialogueManager = DialogueManager.Instance;
            }

            _advanceInputReadyTime = Time.unscaledTime + 0.05f;
            _dialogueManager?.AdvanceDialogue();
        }

        private bool IsAdvanceInputPressed()
        {
            if (enablePointerClickAdvance && IsPointerClickAdvancePressed())
            {
                return true;
            }

            return IsKeyboardAdvancePressed();
        }

        private bool IsPointerClickAdvancePressed()
        {
            if (!WasPrimaryPointerPressedThisFrame())
            {
                return false;
            }

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }

            return true;
        }

        private bool IsKeyboardAdvancePressed()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                return true;
            }
#endif

            if (!Input.anyKeyDown)
            {
                return false;
            }

            if (Input.GetMouseButtonDown(0) ||
                Input.GetMouseButtonDown(1) ||
                Input.GetMouseButtonDown(2))
            {
                return false;
            }

            return true;
        }

        private bool WasPrimaryPointerPressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                return true;
            }
#endif

            return Input.GetMouseButtonDown(0);
        }

        private Transform FindChild(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? null : transform.Find(path);
        }

        private Image EnsureImage(Transform targetTransform)
        {
            if (targetTransform == null)
            {
                return null;
            }

            Image image = targetTransform.GetComponent<Image>();
            if (image == null)
            {
                image = targetTransform.gameObject.AddComponent<Image>();
            }

            image.raycastTarget = false;
            image.preserveAspect = true;
            return image;
        }

        private void UpdateContinueButtonSelection(bool visible)
        {
            if (visible)
            {
                SelectContinueButton();
                return;
            }

            ClearContinueButtonSelection();
        }

        private void SelectContinueButton()
        {
            if (continueButton == null || EventSystem.current == null)
            {
                return;
            }

            EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
        }

        private void ClearContinueButtonSelection()
        {
            if (EventSystem.current == null || continueButton == null)
            {
                return;
            }

            if (EventSystem.current.currentSelectedGameObject == continueButton.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        private static void SetRaycastTarget(Graphic graphic, bool enabled)
        {
            if (graphic == null)
            {
                return;
            }

            graphic.raycastTarget = enabled;
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
