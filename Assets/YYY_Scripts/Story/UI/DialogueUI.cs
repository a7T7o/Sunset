using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private DialogueFontLibrarySO fontLibrary = null;
        [SerializeField] private string defaultFontKey = "default";
        [SerializeField] private string speakerNameFontKey = "speaker_name";
        [SerializeField] private string innerMonologueFontKey = "inner_monologue";
        [SerializeField] private string garbledFontKey = "garbled";

        [Header("Transition Settings")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.2f;

        [Header("Advance Settings")]
        [SerializeField] private bool enableAnyKeyAdvance = true;
        [SerializeField] private KeyCode advanceKey = KeyCode.T;
        [SerializeField] private bool enablePointerClickAdvance = false;
        [SerializeField] private float anyKeyAdvanceDebounce = 0.15f;

        [Header("Immersion Settings")]
        [SerializeField] private float otherUiFadeOutDuration = 0.12f;
        [SerializeField] private float otherUiFadeInDuration = 0.18f;

        [Header("Portrait Settings")]
        [SerializeField] private bool usePlaceholderPortraitWhenMissing = true;
        [SerializeField] private Color placeholderPortraitColor = new Color(0.55f, 0.55f, 0.55f, 1f);

        [Header("Test Status")]
        [SerializeField] private bool showTestStatus = true;
        [SerializeField] private Image testStatusBackground;
        [SerializeField] private TextMeshProUGUI testStatusText;
        #endregion

        #region Private Fields
        private DialogueManager _dialogueManager;
        private Coroutine _fadeCoroutine;
        private bool _isVisible;
        private float _advanceInputReadyTime = float.PositiveInfinity;
        private int _lastAdvanceFrame = -1;
        private float _originalBackgroundAlpha = 1.0f;
        private float _dialogueBaseFontSize;
        private float _speakerBaseFontSize;
        private float _dialogueBaseLineSpacing;
        private Color _dialogueBaseColor = Color.white;
        private FontStyles _dialogueBaseFontStyle = FontStyles.Normal;
        private float _testStatusBaseFontSize;
        private RectTransform _testStatusContainer;
        private Sprite _placeholderPortraitSprite;
        private readonly List<NonDialogueUiSnapshot> _nonDialogueUiSnapshots = new();
        #endregion

        #region Nested Types
        private sealed class NonDialogueUiSnapshot
        {
            public GameObject TargetObject;
            public CanvasGroup CanvasGroup;
            public bool WasActive;
            public float OriginalAlpha;
            public bool OriginalInteractable;
            public bool OriginalBlocksRaycasts;
        }
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            ResolveReferences();
            EnsureCanvasGroup();
            ResolvePortraitTarget();
            EnsureTestStatusText();
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

            RestoreNonDialogueUiImmediate();

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

                UpdateTestStatusText();
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
                RequestAdvanceDialogue();
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
        public string CurrentTestStatus => testStatusText != null ? testStatusText.text : string.Empty;

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
                root = gameObject;
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
                Transform portraitTransform = FindPortraitTransform();
                if (portraitTransform != null)
                {
                    portraitImage = EnsureImage(portraitTransform);
                }
            }
        }

        private void EnsureTestStatusText()
        {
            if (testStatusText != null)
            {
                return;
            }

            Transform existingContainer = FindChild("TestStatusBar");
            if (existingContainer != null)
            {
                _testStatusContainer = existingContainer as RectTransform;
                testStatusBackground = existingContainer.GetComponent<Image>();
                Transform existingText = existingContainer.Find("TestStatusText");
                if (existingText != null)
                {
                    testStatusText = existingText.GetComponent<TextMeshProUGUI>();
                }
                return;
            }

            Transform existing = FindChild("TestStatusText");
            if (existing != null)
            {
                testStatusText = existing.GetComponent<TextMeshProUGUI>();
                return;
            }

            Transform parent = backgroundImage != null ? backgroundImage.transform : transform;
            GameObject containerObject = new GameObject("TestStatusBar", typeof(RectTransform), typeof(Image));
            containerObject.transform.SetParent(parent, false);
            _testStatusContainer = containerObject.GetComponent<RectTransform>();
            testStatusBackground = containerObject.GetComponent<Image>();
            testStatusBackground.color = new Color(0.22f, 0.16f, 0.12f, 0.92f);
            testStatusBackground.raycastTarget = false;

            _testStatusContainer.anchorMin = new Vector2(0.5f, 0f);
            _testStatusContainer.anchorMax = new Vector2(0.5f, 0f);
            _testStatusContainer.pivot = new Vector2(0.5f, 0f);
            _testStatusContainer.anchoredPosition = new Vector2(0f, 18f);
            _testStatusContainer.sizeDelta = new Vector2(980f, 28f);

            GameObject textObject = new GameObject("TestStatusText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(containerObject.transform, false);

            testStatusText = textObject.GetComponent<TextMeshProUGUI>();
            testStatusText.fontSize = 17f;
            testStatusText.alignment = TextAlignmentOptions.Center;
            testStatusText.color = new Color(0.98f, 0.96f, 0.9f, 1f);
            testStatusText.textWrappingMode = TextWrappingModes.NoWrap;
            testStatusText.overflowMode = TextOverflowModes.Ellipsis;
            testStatusText.raycastTarget = false;

            if (dialogueText != null && dialogueText.font != null)
            {
                testStatusText.font = dialogueText.font;
            }

            RectTransform textRect = testStatusText.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(14f, 2f);
            textRect.offsetMax = new Vector2(-14f, -2f);
        }

        private void ResolvePortraitTarget()
        {
            Transform iconTransform = FindPortraitTransform();
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
            GameObject canvasRoot = ResolveCanvasStateTarget();
            if (canvasGroup != null && canvasGroup.gameObject == canvasRoot)
            {
                return;
            }

            canvasGroup = canvasRoot.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = canvasRoot.AddComponent<CanvasGroup>();
            }
        }

        private GameObject ResolveCanvasStateTarget()
        {
            if (root == null)
            {
                return gameObject;
            }

            if (root == gameObject)
            {
                return gameObject;
            }

            Transform rootTransform = root.transform;
            if (!rootTransform.IsChildOf(transform))
            {
                return root;
            }

            bool coversSpeaker = IsInScope(rootTransform, speakerNameText != null ? speakerNameText.transform : null);
            bool coversDialogue = IsInScope(rootTransform, dialogueText != null ? dialogueText.transform : null);
            bool coversButton = IsInScope(rootTransform, continueButton != null ? continueButton.transform : null);
            bool coversPortrait = IsInScope(rootTransform, portraitImage != null ? portraitImage.transform : null);

            return coversSpeaker && coversDialogue && coversButton && coversPortrait
                ? root
                : gameObject;
        }

        private void ConfigureInteractionSurfaces()
        {
            SetRaycastTarget(backgroundImage, false);
            SetRaycastTarget(speakerNameText, false);
            SetRaycastTarget(dialogueText, false);
            SetRaycastTarget(portraitImage, false);

            Transform avatarTransform = FindChild("头像") ?? FindChild("Avatar");
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

            if (testStatusText != null)
            {
                _testStatusBaseFontSize = testStatusText.fontSize;
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

            if (testStatusText != null)
            {
                testStatusText.text = string.Empty;
                testStatusText.gameObject.SetActive(false);
            }

            if (_testStatusContainer != null)
            {
                _testStatusContainer.gameObject.SetActive(false);
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

            _fadeCoroutine = StartCoroutine(TransitionDialoguePresentation(visible));
        }

        private IEnumerator TransitionDialoguePresentation(bool visible)
        {
            if (visible)
            {
                CaptureNonDialogueUiSnapshots();
                yield return FadeNonDialogueUi(false, Mathf.Max(0f, otherUiFadeOutDuration));
                yield return FadeDialogueCanvas(0f, 1f, Mathf.Max(0f, fadeInDuration), true);
            }
            else
            {
                float from = canvasGroup != null ? canvasGroup.alpha : 1f;
                yield return FadeDialogueCanvas(from, 0f, Mathf.Max(0f, fadeOutDuration), false);
                yield return FadeNonDialogueUi(true, Mathf.Max(0f, otherUiFadeInDuration));
            }

            _fadeCoroutine = null;
        }

        private IEnumerator FadeDialogueCanvas(float from, float to, float duration, bool finalVisible)
        {
            if (canvasGroup == null)
            {
                yield break;
            }

            SetCanvasState(from, false, false);
            UpdateContinueButtonSelection(false);

            if (duration <= 0f)
            {
                SetCanvasState(to, finalVisible, finalVisible);
                UpdateContinueButtonSelection(finalVisible);
                if (!finalVisible)
                {
                    ClearVisibleContent();
                }
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

                SetCanvasState(Mathf.Lerp(from, to, eased), false, false);

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
            RequestAdvanceDialogue();
        }

        private void RequestAdvanceDialogue()
        {
            if (_dialogueManager == null)
            {
                _dialogueManager = DialogueManager.Instance;
            }

            if (_dialogueManager == null || _lastAdvanceFrame == Time.frameCount)
            {
                return;
            }

            _lastAdvanceFrame = Time.frameCount;
            _advanceInputReadyTime = Time.unscaledTime + 0.05f;
            _dialogueManager.AdvanceDialogue();
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
            if (!enableAnyKeyAdvance)
            {
                return false;
            }

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null &&
                System.Enum.TryParse(advanceKey.ToString(), true, out Key keyboardKey) &&
                Keyboard.current[keyboardKey] != null &&
                Keyboard.current[keyboardKey].wasPressedThisFrame)
            {
                return true;
            }
#endif

            return Input.GetKeyDown(advanceKey);
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

        private bool IsSubmitKeyRoutedToContinueButton()
        {
            return continueButton != null &&
                   EventSystem.current != null &&
                   EventSystem.current.currentSelectedGameObject == continueButton.gameObject &&
                   WasSubmitAdvancePressedThisFrame();
        }

        private bool WasSubmitAdvancePressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                return Keyboard.current.enterKey.wasPressedThisFrame ||
                       Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
                       Keyboard.current.spaceKey.wasPressedThisFrame;
            }
#endif

            return Input.GetKeyDown(KeyCode.Return) ||
                   Input.GetKeyDown(KeyCode.KeypadEnter) ||
                   Input.GetKeyDown(KeyCode.Space);
        }

        private Transform FindChild(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            Transform rootTransform = root != null ? root.transform : null;
            Transform found = FindInScope(rootTransform, path) ?? FindInScope(transform, path);
            if (found != null)
            {
                return found;
            }

            if (!path.Contains("/"))
            {
                found = FindDescendantByName(rootTransform, path);
                if (found != null)
                {
                    return found;
                }

                if (rootTransform != transform)
                {
                    return FindDescendantByName(transform, path);
                }
            }

            return null;
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

        private Transform FindPortraitTransform()
        {
            return FindChild("头像/Icon") ??
                   FindChild("Avatar/Icon") ??
                   FindChild("PortraitImage") ??
                   FindChild("头像") ??
                   FindChild("Avatar");
        }

        private static Transform FindInScope(Transform scope, string path)
        {
            return scope == null ? null : scope.Find(path);
        }

        private static Transform FindDescendantByName(Transform scope, string name)
        {
            if (scope == null)
            {
                return null;
            }

            for (int childIndex = 0; childIndex < scope.childCount; childIndex++)
            {
                Transform child = scope.GetChild(childIndex);
                if (child.name == name)
                {
                    return child;
                }

                Transform nestedChild = FindDescendantByName(child, name);
                if (nestedChild != null)
                {
                    return nestedChild;
                }
            }

            return null;
        }

        private static bool IsInScope(Transform scope, Transform target)
        {
            return target == null || target == scope || target.IsChildOf(scope);
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
            ApplyFontToText(testStatusText, defaultFontKey, _testStatusBaseFontSize, 0f);
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

        private void UpdateTestStatusText()
        {
            if (!showTestStatus || testStatusText == null || _dialogueManager == null || !_dialogueManager.IsDialogueActive)
            {
                if (testStatusText != null)
                {
                    testStatusText.gameObject.SetActive(false);
                }

                if (_testStatusContainer != null)
                {
                    _testStatusContainer.gameObject.SetActive(false);
                }

                return;
            }

            SpringDay1Director day1Director = SpringDay1Director.Instance;
            string sequenceId = string.IsNullOrWhiteSpace(_dialogueManager.CurrentSequenceId) ? "unknown" : _dialogueManager.CurrentSequenceId;
            int displayIndex = Mathf.Clamp(_dialogueManager.CurrentNodeIndex + 1, 0, Mathf.Max(1, _dialogueManager.CurrentNodeCount));
            int totalCount = Mathf.Max(1, _dialogueManager.CurrentNodeCount);
            string taskLabel = day1Director != null ? day1Director.GetCurrentTaskLabel() : "n/a";
            string progressLabel = day1Director != null ? day1Director.GetCurrentProgressLabel() : "n/a";

            if (_testStatusContainer != null)
            {
                _testStatusContainer.gameObject.SetActive(true);
            }

            testStatusText.gameObject.SetActive(true);
            testStatusText.text = $"测试对话: {sequenceId} [{displayIndex}/{totalCount}]  ·  阶段: {taskLabel}  ·  进度: {progressLabel}";
        }

        private void CaptureNonDialogueUiSnapshots()
        {
            _nonDialogueUiSnapshots.Clear();

            Transform siblingParent = ResolveNonDialogueUiParent();
            Transform dialogueRoot = ResolveCanvasStateTarget().transform;
            if (siblingParent == null)
            {
                return;
            }

            for (int childIndex = 0; childIndex < siblingParent.childCount; childIndex++)
            {
                Transform child = siblingParent.GetChild(childIndex);
                if (!ShouldManageAsNonDialogueUi(child, dialogueRoot))
                {
                    continue;
                }

                CanvasGroup siblingCanvasGroup = child.GetComponent<CanvasGroup>();
                if (siblingCanvasGroup == null)
                {
                    siblingCanvasGroup = child.gameObject.AddComponent<CanvasGroup>();
                }

                _nonDialogueUiSnapshots.Add(new NonDialogueUiSnapshot
                {
                    TargetObject = child.gameObject,
                    CanvasGroup = siblingCanvasGroup,
                    WasActive = child.gameObject.activeSelf,
                    OriginalAlpha = siblingCanvasGroup.alpha,
                    OriginalInteractable = siblingCanvasGroup.interactable,
                    OriginalBlocksRaycasts = siblingCanvasGroup.blocksRaycasts
                });
            }
        }

        private IEnumerator FadeNonDialogueUi(bool visible, float duration)
        {
            if (_nonDialogueUiSnapshots.Count == 0)
            {
                yield break;
            }

            List<float> startAlphas = new List<float>(_nonDialogueUiSnapshots.Count);
            List<float> targetAlphas = new List<float>(_nonDialogueUiSnapshots.Count);

            for (int index = 0; index < _nonDialogueUiSnapshots.Count; index++)
            {
                NonDialogueUiSnapshot snapshot = _nonDialogueUiSnapshots[index];
                if (snapshot == null || snapshot.CanvasGroup == null || snapshot.TargetObject == null)
                {
                    startAlphas.Add(0f);
                    targetAlphas.Add(0f);
                    continue;
                }

                if (visible)
                {
                    if (!snapshot.WasActive)
                    {
                        startAlphas.Add(0f);
                        targetAlphas.Add(0f);
                        continue;
                    }

                    snapshot.TargetObject.SetActive(true);
                    snapshot.CanvasGroup.alpha = 0f;
                    snapshot.CanvasGroup.interactable = false;
                    snapshot.CanvasGroup.blocksRaycasts = false;
                    startAlphas.Add(0f);
                    targetAlphas.Add(snapshot.OriginalAlpha);
                }
                else
                {
                    if (!snapshot.TargetObject.activeSelf && snapshot.WasActive)
                    {
                        snapshot.TargetObject.SetActive(true);
                    }

                    snapshot.CanvasGroup.interactable = false;
                    snapshot.CanvasGroup.blocksRaycasts = false;
                    startAlphas.Add(snapshot.CanvasGroup.alpha);
                    targetAlphas.Add(0f);
                }
            }

            if (duration <= 0f)
            {
                for (int index = 0; index < _nonDialogueUiSnapshots.Count; index++)
                {
                    ApplyNonDialogueUiSnapshot(_nonDialogueUiSnapshots[index], targetAlphas[index], visible);
                }

                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float normalized = Mathf.Clamp01(elapsed / duration);
                float eased = visible
                    ? 1f - ((1f - normalized) * (1f - normalized))
                    : normalized * normalized;

                for (int index = 0; index < _nonDialogueUiSnapshots.Count; index++)
                {
                    NonDialogueUiSnapshot snapshot = _nonDialogueUiSnapshots[index];
                    if (snapshot == null || snapshot.CanvasGroup == null)
                    {
                        continue;
                    }

                    snapshot.CanvasGroup.alpha = Mathf.Lerp(startAlphas[index], targetAlphas[index], eased);
                }

                if (normalized >= 1f)
                {
                    break;
                }

                yield return null;
            }

            for (int index = 0; index < _nonDialogueUiSnapshots.Count; index++)
            {
                ApplyNonDialogueUiSnapshot(_nonDialogueUiSnapshots[index], targetAlphas[index], visible);
            }
        }

        private void ApplyNonDialogueUiSnapshot(NonDialogueUiSnapshot snapshot, float alpha, bool visible)
        {
            if (snapshot == null || snapshot.TargetObject == null || snapshot.CanvasGroup == null)
            {
                return;
            }

            if (!visible)
            {
                snapshot.CanvasGroup.alpha = 0f;
                snapshot.CanvasGroup.interactable = false;
                snapshot.CanvasGroup.blocksRaycasts = false;
                if (snapshot.WasActive)
                {
                    snapshot.TargetObject.SetActive(false);
                }
                return;
            }

            if (!snapshot.WasActive)
            {
                snapshot.TargetObject.SetActive(false);
                snapshot.CanvasGroup.alpha = snapshot.OriginalAlpha;
                snapshot.CanvasGroup.interactable = snapshot.OriginalInteractable;
                snapshot.CanvasGroup.blocksRaycasts = snapshot.OriginalBlocksRaycasts;
                return;
            }

            snapshot.TargetObject.SetActive(true);
            snapshot.CanvasGroup.alpha = alpha;
            snapshot.CanvasGroup.interactable = snapshot.OriginalInteractable;
            snapshot.CanvasGroup.blocksRaycasts = snapshot.OriginalBlocksRaycasts;
        }

        private void RestoreNonDialogueUiImmediate()
        {
            if (_nonDialogueUiSnapshots.Count == 0)
            {
                return;
            }

            for (int index = 0; index < _nonDialogueUiSnapshots.Count; index++)
            {
                NonDialogueUiSnapshot snapshot = _nonDialogueUiSnapshots[index];
                if (snapshot == null || snapshot.TargetObject == null || snapshot.CanvasGroup == null)
                {
                    continue;
                }

                snapshot.TargetObject.SetActive(snapshot.WasActive);
                snapshot.CanvasGroup.alpha = snapshot.OriginalAlpha;
                snapshot.CanvasGroup.interactable = snapshot.OriginalInteractable;
                snapshot.CanvasGroup.blocksRaycasts = snapshot.OriginalBlocksRaycasts;
            }

            _nonDialogueUiSnapshots.Clear();
        }

        private Transform ResolveNonDialogueUiParent()
        {
            GameObject canvasStateTarget = ResolveCanvasStateTarget();
            return canvasStateTarget != null ? canvasStateTarget.transform.parent : null;
        }

        private static bool ShouldManageAsNonDialogueUi(Transform candidate, Transform dialogueRoot)
        {
            if (candidate == null || candidate == dialogueRoot)
            {
                return false;
            }

            return candidate.GetComponent<SpringDay1PromptOverlay>() == null;
        }
        #endregion
    }
}
