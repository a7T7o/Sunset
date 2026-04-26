using System.Collections;
using System.Collections.Generic;
using System.IO;
using FarmGame.UI;
using Sunset.Events;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Sunset.Story
{
    public class DialogueUI : MonoBehaviour
    {
        private const string PlayerPortraitNpcId = "000";
        private const string ContinueButtonDisplayText = "摁空格键继续";
        private const string ContinueButtonFontProbeText = "摁空格键继续";
        private const float MinimumDialogueTransitionDuration = 0.5f;
        private const string InnerMonologueFontResourcePath = "Fonts & Materials/DialogueChinese SoftPixel SDF";
        private const string NpcFallbackPortraitAssetPath = "Assets/Sprites/NPC/001.png";
        private const string NpcFallbackPortraitRuntimeRelativePath = "Sprites/NPC/001.png";
        private const float NpcFallbackPortraitCropWidthRatio = 0.72f;
        private const float NpcFallbackPortraitCropHeightRatio = 0.62f;

        private static readonly string[] PreferredDialogueFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese SDF",
            "Fonts & Materials/DialogueChinese SoftPixel SDF"
        };

        private static readonly string[] PreferredNpcPortraitSpriteNames =
        {
            "001_Down_1",
            "001_1",
            "001_Up_1"
        };

        private static readonly string[] PlayerSpeakerAliases =
        {
            "旅人",
            "陌生旅人",
            "玩家",
            "主角"
        };

        private static readonly string[] NarrationSpeakerAliases =
        {
            "旁白",
            "内心旁白"
        };

        #region Serialized Fields
        [Header("UI References")]
        [SerializeField] private GameObject root;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button continueButton;
        [SerializeField] private TextMeshProUGUI continueButtonLabel;
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
        [SerializeField] private KeyCode advanceKey = KeyCode.Space;
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
        private float _continueButtonBaseFontSize;
        private float _dialogueBaseLineSpacing;
        private Color _dialogueBaseColor = Color.white;
        private Color _continueButtonBaseColor = Color.white;
        private FontStyles _dialogueBaseFontStyle = FontStyles.Normal;
        private float _testStatusBaseFontSize;
        private RectTransform _testStatusContainer;
        private Sprite _placeholderPortraitSprite;
        private Sprite _fallbackNpcPortraitSprite;
        private Texture2D _fallbackNpcPortraitTexture;
        private NpcCharacterRegistry _npcCharacterRegistry;
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
            NormalizeAdvanceInputSettings();
            DialogueChineseFontRuntimeBootstrap.EnsureRuntimeFontReady();
            ResolveReferences();
            EnsureCanvasGroup();
            ResolvePortraitTarget();
            showTestStatus &= Application.isBatchMode;
            EnsureTestStatusText();
            ConfigureInteractionSurfaces();
            CacheBasePresentation();
            EnsureUsableRuntimeFonts();
            _npcCharacterRegistry = NpcCharacterRegistry.LoadRuntime();
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
            _npcCharacterRegistry = _npcCharacterRegistry != null ? _npcCharacterRegistry : NpcCharacterRegistry.LoadRuntime();
            _dialogueManager = DialogueManager.Instance ?? _dialogueManager;
            TryRecoverActiveDialogueVisibility(immediate: _dialogueManager != null && _dialogueManager.IsDialogueActive);
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

            if (_fallbackNpcPortraitSprite != null)
            {
                Destroy(_fallbackNpcPortraitSprite);
                _fallbackNpcPortraitSprite = null;
            }

            if (_fallbackNpcPortraitTexture != null)
            {
                Destroy(_fallbackNpcPortraitTexture);
                _fallbackNpcPortraitTexture = null;
            }
        }

        private void OnValidate()
        {
            NormalizeAdvanceInputSettings();
        }

        private void Update()
        {
            if (_dialogueManager == null)
            {
                _dialogueManager = DialogueManager.Instance;
            }

            if (_dialogueManager != null &&
                _dialogueManager.IsDialogueActive &&
                _fadeCoroutine == null &&
                (!_isVisible || (canvasGroup != null && canvasGroup.alpha <= 0.001f)))
            {
                TryRecoverActiveDialogueVisibility(immediate: true);
            }

            if (_isVisible && _dialogueManager != null && dialogueText != null)
            {
                if (dialogueText.text != _dialogueManager.CurrentTypedText)
                {
                    dialogueText.text = _dialogueManager.CurrentTypedText;
                    dialogueText.ForceMeshUpdate();
                }

                StabilizeAssignedFont(dialogueText, _dialogueBaseFontSize, _dialogueBaseLineSpacing);
                StabilizeAssignedFont(speakerNameText, _speakerBaseFontSize, 0f);
                EnsureContinueButtonReadable();
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
            EnsureDialogueVisualComponentsReady();
            EnsureUsableRuntimeFonts();
            EnsureContinueButtonReadable();
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
            EnsureContinueButtonReadable();

            if (eventData.Node.isInnerMonologue)
            {
                if (dialogueText != null)
                {
                    ApplyInnerMonologuePresentation();
                }

                if (speakerNameText != null)
                {
                    speakerNameText.gameObject.SetActive(false);
                    speakerNameText.text = string.Empty;
                }
                ApplyPortrait(eventData.Node);
            }
            else
            {
                if (speakerNameText != null)
                {
                    bool hasSpeakerName = !string.IsNullOrWhiteSpace(eventData.Node.speakerName);
                    speakerNameText.gameObject.SetActive(hasSpeakerName);
                    speakerNameText.text = eventData.Node.speakerName;
                }

                ApplyPortrait(eventData.Node);
            }
        }

        private void OnDialogueEnd(DialogueEndEvent _)
        {
            if (ShouldIgnoreDialogueEndEvent())
            {
                return;
            }

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

            if (continueButtonLabel == null && continueButton != null)
            {
                continueButtonLabel = continueButton.GetComponentInChildren<TextMeshProUGUI>(true);
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

                if (continueButtonLabel == null)
                {
                    continueButtonLabel = continueButton.GetComponentInChildren<TextMeshProUGUI>(true);
                }

                if (continueButtonLabel != null && continueButtonLabel.transform != continueButton.transform)
                {
                    continueButtonLabel.raycastTarget = false;
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

            if (continueButtonLabel != null)
            {
                _continueButtonBaseFontSize = continueButtonLabel.fontSize;
                _continueButtonBaseColor = continueButtonLabel.color;
            }

            if (testStatusText != null)
            {
                _testStatusBaseFontSize = testStatusText.fontSize;
            }
        }

        private void EnsureUsableRuntimeFonts()
        {
            DialogueChineseFontRuntimeBootstrap.EnsureRuntimeFontReady();
            EnsureDialogueVisualComponentsReady();
            StabilizeAssignedFont(dialogueText, _dialogueBaseFontSize, _dialogueBaseLineSpacing);
            StabilizeAssignedFont(speakerNameText, _speakerBaseFontSize, 0f);
            StabilizeAssignedFont(continueButtonLabel, _continueButtonBaseFontSize, 0f);
            StabilizeAssignedFont(testStatusText, _testStatusBaseFontSize, 0f);
            EnsureContinueButtonReadable();
        }

        private void EnsureDialogueVisualComponentsReady()
        {
            EnsureContinueButtonLabelReference();
            EnsureTextComponentReady(dialogueText, forceActive: true);
            EnsureTextComponentReady(speakerNameText, forceActive: false);
            EnsureTextComponentReady(continueButtonLabel, forceActive: continueButton != null && continueButton.gameObject.activeInHierarchy);
            EnsureTextComponentReady(testStatusText, forceActive: false);
        }

        private void EnsureContinueButtonLabelReference()
        {
            if (continueButton == null)
            {
                return;
            }

            if (!IsUsableContinueButtonLabel(continueButtonLabel))
            {
                continueButtonLabel = FindReusableContinueButtonLabel();
            }

            if (continueButtonLabel != null)
            {
                NormalizeContinueButtonLabelRect(continueButtonLabel.rectTransform);
                return;
            }

            GameObject labelObject = new GameObject("Label", typeof(RectTransform));
            labelObject.transform.SetParent(continueButton.transform, false);

            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            continueButtonLabel = labelObject.AddComponent<TextMeshProUGUI>();
            continueButtonLabel.alignment = TextAlignmentOptions.Center;
            continueButtonLabel.raycastTarget = false;
            continueButtonLabel.textWrappingMode = TextWrappingModes.NoWrap;
            continueButtonLabel.overflowMode = TextOverflowModes.Overflow;
            continueButtonLabel.color = _continueButtonBaseColor.a > 0.01f
                ? _continueButtonBaseColor
                : Color.white;
            continueButtonLabel.fontSize = _continueButtonBaseFontSize > 0.01f
                ? _continueButtonBaseFontSize
                : 16f;
            NormalizeContinueButtonLabelRect(continueButtonLabel.rectTransform);
        }

        private TextMeshProUGUI FindReusableContinueButtonLabel()
        {
            if (continueButton == null)
            {
                return null;
            }

            TextMeshProUGUI[] candidates = continueButton.GetComponentsInChildren<TextMeshProUGUI>(true);
            TextMeshProUGUI best = null;
            float bestScore = float.MinValue;
            for (int index = 0; index < candidates.Length; index++)
            {
                TextMeshProUGUI candidate = candidates[index];
                if (candidate == null || !candidate.transform.IsChildOf(continueButton.transform))
                {
                    continue;
                }

                RectTransform rect = candidate.rectTransform;
                float score = Mathf.Max(0f, rect.rect.width) * Mathf.Max(0f, rect.rect.height);
                if (candidate.gameObject.activeSelf)
                {
                    score += 1000f;
                }

                if (candidate.enabled)
                {
                    score += 600f;
                }

                if (rect.anchorMin == Vector2.zero && rect.anchorMax == Vector2.one)
                {
                    score += 450f;
                }

                if (ContainsCjk(candidate.text))
                {
                    score += 900f;
                }

                if (CanFontRenderText(candidate.font, ContinueButtonFontProbeText))
                {
                    score += 500f;
                }

                if (score > bestScore)
                {
                    best = candidate;
                    bestScore = score;
                }
            }

            return best;
        }

        private bool IsUsableContinueButtonLabel(TextMeshProUGUI candidate)
        {
            return candidate != null
                && continueButton != null
                && candidate.transform.IsChildOf(continueButton.transform);
        }

        private static void NormalizeContinueButtonLabelRect(RectTransform rect)
        {
            if (rect == null)
            {
                return;
            }
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
        }

        private static void EnsureTextComponentReady(TextMeshProUGUI target, bool forceActive)
        {
            if (target == null)
            {
                return;
            }

            if (forceActive && !target.gameObject.activeSelf)
            {
                target.gameObject.SetActive(true);
            }

            if (!target.enabled)
            {
                target.enabled = true;
            }
        }

        private void InitializeHiddenState()
        {
            SetCanvasState(0f, false, false);
            ClearContinueButtonSelection();
            ClearVisibleContent();
        }

        private void TryRecoverActiveDialogueVisibility(bool immediate)
        {
            if (_dialogueManager == null)
            {
                _dialogueManager = DialogueManager.Instance;
            }

            if (_dialogueManager == null || !_dialogueManager.IsDialogueActive)
            {
                return;
            }

            _isVisible = true;
            _advanceInputReadyTime = Time.unscaledTime + Mathf.Max(0f, anyKeyAdvanceDebounce);

            if (_dialogueManager.CurrentNode != null)
            {
                OnDialogueNodeChanged(new DialogueNodeChangedEvent
                {
                    Node = _dialogueManager.CurrentNode
                });
            }

            if (dialogueText != null)
            {
                dialogueText.text = _dialogueManager.CurrentTypedText;
                dialogueText.ForceMeshUpdate();
            }

            EnsureUsableRuntimeFonts();
            UpdateTestStatusText();

            if (immediate)
            {
                if (_fadeCoroutine != null)
                {
                    StopCoroutine(_fadeCoroutine);
                    _fadeCoroutine = null;
                }

                SetCanvasState(1f, true, true);
                UpdateContinueButtonSelection(true);
                return;
            }

            StartVisibilityTransition(true);
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
                backgroundImage.enabled = true;
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
                EnsureDialogueVisualComponentsReady();
                EnsureUsableRuntimeFonts();
                float from = canvasGroup != null ? canvasGroup.alpha : 0f;
                SetCanvasState(from, false, false);
                UpdateContinueButtonSelection(false);
                yield return FadeNonDialogueUi(false, Mathf.Max(MinimumDialogueTransitionDuration, otherUiFadeOutDuration));
                yield return FadeDialogueCanvas(from, 1f, Mathf.Max(MinimumDialogueTransitionDuration, fadeInDuration), true);
            }
            else
            {
                float from = canvasGroup != null ? canvasGroup.alpha : 1f;
                yield return FadeDialogueCanvas(from, 0f, Mathf.Max(MinimumDialogueTransitionDuration, fadeOutDuration), false);
                yield return FadeNonDialogueUi(true, Mathf.Max(MinimumDialogueTransitionDuration, otherUiFadeInDuration));
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

        private void ApplyPortrait(DialogueNode node)
        {
            if (portraitImage == null)
            {
                return;
            }

            Sprite resolvedPortrait = null;
            Color resolvedColor = Color.white;

            if (node != null)
            {
                resolvedPortrait = ResolveSpecialDialoguePortrait(node);
                if (resolvedPortrait == null)
                {
                    resolvedPortrait = ResolveRegistryDialoguePortrait(node.speakerName);
                }
                if (resolvedPortrait == null)
                {
                    resolvedPortrait = node.speakerPortrait;
                }
            }

            if (resolvedPortrait == null && usePlaceholderPortraitWhenMissing)
            {
                resolvedPortrait = GetOrCreateNpcFallbackPortrait();
                if (resolvedPortrait == null)
                {
                    resolvedPortrait = GetOrCreatePlaceholderPortrait();
                    resolvedColor = placeholderPortraitColor;
                }
            }

            portraitImage.sprite = resolvedPortrait;
            portraitImage.color = resolvedColor;
            portraitImage.preserveAspect = true;
            portraitImage.enabled = resolvedPortrait != null;
            portraitImage.gameObject.SetActive(resolvedPortrait != null);
        }

        private Sprite ResolveRegistryDialoguePortrait(string speakerName)
        {
            _npcCharacterRegistry = _npcCharacterRegistry != null ? _npcCharacterRegistry : NpcCharacterRegistry.LoadRuntime();
            if (_npcCharacterRegistry == null)
            {
                return null;
            }

            return _npcCharacterRegistry.TryResolveDialoguePortrait(speakerName, out Sprite portrait) ? portrait : null;
        }

        private Sprite ResolveSpecialDialoguePortrait(DialogueNode node)
        {
            if (node == null)
            {
                return null;
            }

            if (node.isInnerMonologue || IsPlayerSpeaker(node.speakerName) || IsNarrationSpeaker(node.speakerName))
            {
                return ResolveEntryPortraitByNpcId(PlayerPortraitNpcId);
            }

            return null;
        }

        private Sprite ResolveEntryPortraitByNpcId(string npcId)
        {
            _npcCharacterRegistry = _npcCharacterRegistry != null ? _npcCharacterRegistry : NpcCharacterRegistry.LoadRuntime();
            if (_npcCharacterRegistry == null || !_npcCharacterRegistry.TryGetEntryByNpcId(npcId, out NpcCharacterRegistry.Entry entry) || entry == null)
            {
                return null;
            }

            return entry.ResolveDialoguePortrait();
        }

        private static bool IsPlayerSpeaker(string speakerName)
        {
            return MatchesAlias(speakerName, PlayerSpeakerAliases);
        }

        private static bool IsNarrationSpeaker(string speakerName)
        {
            return MatchesAlias(speakerName, NarrationSpeakerAliases);
        }

        private static bool MatchesAlias(string rawValue, string[] aliases)
        {
            if (aliases == null || aliases.Length == 0)
            {
                return false;
            }

            string normalized = string.IsNullOrWhiteSpace(rawValue) ? string.Empty : rawValue.Trim();
            if (normalized.Length == 0)
            {
                return false;
            }

            for (int index = 0; index < aliases.Length; index++)
            {
                if (string.Equals(normalized, aliases[index], System.StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private Sprite GetOrCreateNpcFallbackPortrait()
        {
            if (_fallbackNpcPortraitSprite != null)
            {
                return _fallbackNpcPortraitSprite;
            }

#if UNITY_EDITOR
            Sprite editorSprite = LoadEditorNpcFallbackSourceSprite();
            if (editorSprite != null)
            {
                _fallbackNpcPortraitSprite = CreateNpcFallbackPortraitCrop(editorSprite.texture, editorSprite.rect, editorSprite.pixelsPerUnit);
                return _fallbackNpcPortraitSprite;
            }
#endif

            if (_fallbackNpcPortraitTexture == null)
            {
                string absolutePath = Path.Combine(Application.dataPath, NpcFallbackPortraitRuntimeRelativePath);
                if (File.Exists(absolutePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(absolutePath);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        _fallbackNpcPortraitTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false)
                        {
                            name = "DialogueNpcFallbackPortraitTexture",
                            filterMode = FilterMode.Point,
                            wrapMode = TextureWrapMode.Clamp
                        };

                        if (!_fallbackNpcPortraitTexture.LoadImage(imageBytes, markNonReadable: false))
                        {
                            Destroy(_fallbackNpcPortraitTexture);
                            _fallbackNpcPortraitTexture = null;
                        }
                    }
                }
            }

            if (_fallbackNpcPortraitTexture == null)
            {
                return null;
            }

            float frameWidth = Mathf.Min(32f, _fallbackNpcPortraitTexture.width);
            float frameHeight = Mathf.Min(32f, _fallbackNpcPortraitTexture.height);
            Rect sourceRect = new Rect(
                Mathf.Min(frameWidth, Mathf.Max(0f, _fallbackNpcPortraitTexture.width - frameWidth)),
                Mathf.Max(0f, _fallbackNpcPortraitTexture.height - frameHeight),
                frameWidth,
                frameHeight);
            _fallbackNpcPortraitSprite = CreateNpcFallbackPortraitCrop(_fallbackNpcPortraitTexture, sourceRect, 16f);
            return _fallbackNpcPortraitSprite;
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

#if UNITY_EDITOR
        private static Sprite LoadEditorNpcFallbackSourceSprite()
        {
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(NpcFallbackPortraitAssetPath);
            if (assets == null)
            {
                return null;
            }

            for (int spriteNameIndex = 0; spriteNameIndex < PreferredNpcPortraitSpriteNames.Length; spriteNameIndex++)
            {
                string preferredName = PreferredNpcPortraitSpriteNames[spriteNameIndex];
                for (int assetIndex = 0; assetIndex < assets.Length; assetIndex++)
                {
                    if (assets[assetIndex] is Sprite candidate && candidate.name == preferredName)
                    {
                        return candidate;
                    }
                }
            }

            for (int assetIndex = 0; assetIndex < assets.Length; assetIndex++)
            {
                if (assets[assetIndex] is Sprite fallbackSprite)
                {
                    return fallbackSprite;
                }
            }

            return null;
        }
#endif

        private static Sprite CreateNpcFallbackPortraitCrop(Texture2D texture, Rect sourceRect, float pixelsPerUnit)
        {
            if (texture == null || sourceRect.width <= 0f || sourceRect.height <= 0f)
            {
                return null;
            }

            float cropWidth = Mathf.Clamp(sourceRect.width * NpcFallbackPortraitCropWidthRatio, 8f, sourceRect.width);
            float cropHeight = Mathf.Clamp(sourceRect.height * NpcFallbackPortraitCropHeightRatio, 8f, sourceRect.height);
            float cropX = sourceRect.x + ((sourceRect.width - cropWidth) * 0.5f);
            float cropY = sourceRect.y + (sourceRect.height - cropHeight);
            Sprite portraitSprite = Sprite.Create(
                texture,
                new Rect(cropX, cropY, cropWidth, cropHeight),
                new Vector2(0.5f, 0.56f),
                Mathf.Max(1f, pixelsPerUnit));
            portraitSprite.name = "DialogueNpcFallbackPortrait";
            return portraitSprite;
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
                return Keyboard.current.spaceKey.wasPressedThisFrame;
            }
#endif

            return Input.GetKeyDown(KeyCode.Space);
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
                ApplyFontStyle(innerMonologueFontKey, speakerNameFontKey);
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

            ApplyFontStyle(innerMonologueFontKey, speakerNameFontKey);
        }

        private void ApplyInnerMonologuePresentation()
        {
            if (dialogueText == null)
            {
                return;
            }

            dialogueText.fontStyle = FontStyles.Normal;
            dialogueText.color = _dialogueBaseColor;
            ApplyFontToText(dialogueText, innerMonologueFontKey, _dialogueBaseFontSize, _dialogueBaseLineSpacing);

            if (!CanFontRenderText(dialogueText.font, dialogueText.text))
            {
                TMP_FontAsset monologueFont = Resources.Load<TMP_FontAsset>(InnerMonologueFontResourcePath);
                TMP_FontAsset resolvedMonologueFont = DialogueChineseFontRuntimeBootstrap.ResolveBestFontForText(
                    dialogueText.text,
                    monologueFont,
                    ContinueButtonFontProbeText);
                if (resolvedMonologueFont != null)
                {
                    dialogueText.font = resolvedMonologueFont;
                    if (resolvedMonologueFont.material != null)
                    {
                        dialogueText.fontSharedMaterial = resolvedMonologueFont.material;
                    }
                }
            }

            dialogueText.ForceMeshUpdate();
        }

        private void ApplyFontStyle(string dialogueFontKey, string speakerFontKey)
        {
            if (fontLibrary == null)
            {
                return;
            }

            ApplyFontToText(dialogueText, dialogueFontKey, _dialogueBaseFontSize, _dialogueBaseLineSpacing);
            ApplyFontToText(speakerNameText, speakerFontKey, _speakerBaseFontSize, 0f);
            ApplyFontToText(continueButtonLabel, defaultFontKey, _continueButtonBaseFontSize, 0f);
            ApplyFontToText(testStatusText, defaultFontKey, _testStatusBaseFontSize, 0f);
        }

        private void NormalizeContinueButtonCopy()
        {
            if (continueButtonLabel == null)
            {
                return;
            }

            string current = continueButtonLabel.text != null ? continueButtonLabel.text.Trim() : string.Empty;
            if (string.IsNullOrWhiteSpace(current)
                || !ContainsCjk(current)
                || current.Contains("任意键")
                || current.Contains("继续对话")
                || current.Contains("按空格")
                || string.Equals(current, "jixu", System.StringComparison.OrdinalIgnoreCase)
                || string.Equals(current, "continue", System.StringComparison.OrdinalIgnoreCase))
            {
                continueButtonLabel.text = ContinueButtonDisplayText;
                continueButtonLabel.ForceMeshUpdate();
            }
        }

        private void EnsureContinueButtonReadable()
        {
            EnsureContinueButtonLabelReference();
            if (continueButtonLabel == null)
            {
                return;
            }

            EnsureTextComponentReady(continueButtonLabel, forceActive: continueButton != null && continueButton.gameObject.activeInHierarchy);
            NormalizeContinueButtonLabelRect(continueButtonLabel.rectTransform);
            EnsureTextAncestorsVisible(continueButtonLabel, continueButton != null ? continueButton.transform : null, 0.95f);
            NormalizeContinueButtonCopy();
            StabilizeAssignedFont(continueButtonLabel, _continueButtonBaseFontSize, 0f);
            continueButtonLabel.raycastTarget = false;
            continueButtonLabel.textWrappingMode = TextWrappingModes.NoWrap;
            continueButtonLabel.overflowMode = TextOverflowModes.Overflow;

            Color targetColor = _continueButtonBaseColor.a > 0.01f
                ? _continueButtonBaseColor
                : continueButtonLabel.color;
            if (targetColor.a < 0.98f)
            {
                targetColor.a = 1f;
            }

            continueButtonLabel.color = targetColor;
            continueButtonLabel.ForceMeshUpdate();
        }

        private void NormalizeAdvanceInputSettings()
        {
            advanceKey = KeyCode.Space;
            enablePointerClickAdvance = false;
        }

        private static void EnsureTextAncestorsVisible(TextMeshProUGUI target, Transform stopAtExclusive, float minAlpha)
        {
            if (target == null)
            {
                return;
            }

            Transform current = target.transform;
            while (current != null && current != stopAtExclusive)
            {
                if (!current.gameObject.activeSelf)
                {
                    current.gameObject.SetActive(true);
                }

                if (current.TryGetComponent(out CanvasGroup canvasGroup) && canvasGroup.alpha < minAlpha)
                {
                    canvasGroup.alpha = minAlpha;
                }

                current = current.parent;
            }
        }

        private void ApplyFontToText(TextMeshProUGUI target, string key, float baseFontSize, float baseLineSpacing)
        {
            if (target == null)
            {
                return;
            }

            DialogueFontLibrarySO.FontEntry entry = null;
            if (fontLibrary != null)
            {
                fontLibrary.TryGetEntry(key, out entry);
            }

            TMP_FontAsset resolvedFont = ResolveUsableDialogueFont(entry, target.text);
            if (resolvedFont != null)
            {
                target.font = resolvedFont;
                if (resolvedFont.material != null)
                {
                    target.fontSharedMaterial = resolvedFont.material;
                }
            }

            if (baseFontSize > 0f)
            {
                target.fontSize = baseFontSize + (entry != null ? entry.fontSizeOffset : 0f);
            }

            target.lineSpacing = baseLineSpacing + (entry != null ? entry.lineSpacingOffset : 0f);
        }

        private void StabilizeAssignedFont(TextMeshProUGUI target, float baseFontSize, float baseLineSpacing)
        {
            if (target == null || !ShouldReplaceAssignedFont(target))
            {
                return;
            }

            TMP_FontAsset fallbackFont = ResolveUsableDialogueFont(null, target.text);
            if (fallbackFont == null)
            {
                return;
            }

            target.font = fallbackFont;
            if (fallbackFont.material != null)
            {
                target.fontSharedMaterial = fallbackFont.material;
            }

            if (baseFontSize > 0f)
            {
                target.fontSize = baseFontSize;
            }

            target.lineSpacing = baseLineSpacing;
            target.ForceMeshUpdate();
        }

        private static TMP_FontAsset ResolveUsableDialogueFont(DialogueFontLibrarySO.FontEntry entry, string currentText)
        {
            TMP_FontAsset preferredFont = entry != null ? entry.fontAsset : null;
            return DialogueChineseFontRuntimeBootstrap.ResolveBestFontForText(
                currentText,
                preferredFont,
                ContinueButtonFontProbeText);
        }

        private static bool ShouldReplaceAssignedFont(TextMeshProUGUI target)
        {
            return !CanFontRenderText(target.font, target.text);
        }

        private static bool CanFontRenderText(TMP_FontAsset fontAsset, string currentText)
        {
            return DialogueChineseFontRuntimeBootstrap.CanRenderText(
                fontAsset,
                currentText,
                ContinueButtonFontProbeText);
        }

        private static string GetFontProbeText(string currentText)
        {
            if (string.IsNullOrWhiteSpace(currentText))
            {
                return ContinueButtonFontProbeText;
            }

            var builder = new System.Text.StringBuilder(currentText.Length);
            bool insideTag = false;
            for (int index = 0; index < currentText.Length; index++)
            {
                char current = currentText[index];
                if (current == '<')
                {
                    insideTag = true;
                    continue;
                }

                if (current == '>')
                {
                    insideTag = false;
                    continue;
                }

                if (!insideTag && !char.IsControl(current))
                {
                    builder.Append(current);
                }
            }

            return builder.ToString().Trim();
        }

        private static bool ContainsCjk(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            for (int index = 0; index < value.Length; index++)
            {
                char current = value[index];
                if ((current >= '\u3400' && current <= '\u9FFF')
                    || (current >= '\uF900' && current <= '\uFAFF'))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsFontAssetUsable(TMP_FontAsset fontAsset)
        {
            if (fontAsset == null || fontAsset.material == null)
            {
                return false;
            }

            Texture[] atlasTextures = fontAsset.atlasTextures;
            if (atlasTextures == null || atlasTextures.Length == 0)
            {
                return false;
            }

            for (int index = 0; index < atlasTextures.Length; index++)
            {
                Texture atlasTexture = atlasTextures[index];
                if (atlasTexture != null && atlasTexture.width > 1 && atlasTexture.height > 1)
                {
                    return true;
                }
            }

            return false;
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
            if (_nonDialogueUiSnapshots.Count > 0)
            {
                return;
            }

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

        private bool ShouldIgnoreDialogueEndEvent()
        {
            if (_dialogueManager == null)
            {
                _dialogueManager = DialogueManager.Instance;
            }

            return _dialogueManager != null && _dialogueManager.IsDialogueActive;
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

            if (candidate.GetComponent<PackagePanelTabsUI>() != null
                || candidate.GetComponent<BoxPanelUI>() != null
                || candidate.GetComponent<InventoryPanelUI>() != null
                || candidate.GetComponent<PackageSaveSettingsPanel>() != null
                || candidate.GetComponent<SpringDay1PromptOverlay>() != null)
            {
                return false;
            }

            if (candidate.GetComponentInChildren<PackagePanelTabsUI>(true) != null
                || candidate.GetComponentInChildren<BoxPanelUI>(true) != null
                || candidate.GetComponentInChildren<InventoryPanelUI>(true) != null
                || candidate.GetComponentInChildren<PackageSaveSettingsPanel>(true) != null
                || candidate.GetComponentInChildren<SpringDay1PromptOverlay>(true) != null)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
