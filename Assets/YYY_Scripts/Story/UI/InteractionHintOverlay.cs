using System.Collections;
using System.Collections.Generic;
using FarmGame.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class InteractionHintOverlay : MonoBehaviour
    {
        private const float DetailCardWidth = 284f;
        private const float DetailCardHeight = 74f;
        private const float CompactCardWidth = 252f;
        private const float CompactCardHeight = 60f;
        private const float BaseCardX = 22f;
        private const float BaseCardY = 18f;
        private const float CardStackGap = 8f;

        private const float StatusCardWidth = 312f;
        private const float StatusCardCompactHeight = 68f;
        private const float StatusCardDetailHeight = 80f;
        private const float StatusFadeDuration = 0.25f;
        private const float StatusHoldDuration = 2f;
        private const float ContextCardWidth = 220f;
        private const float ContextCardMinHeight = 154f;
        private const float ContextCardRowHeight = 17f;
        private const float ContextCardRowGap = 3f;
        private const float ContextCardRightInset = 22f;
        private const float ContextCardCenterOffsetY = 4f;
        private const float ContextCardInnerLeft = 14f;
        private const float ContextCardInnerRight = 12f;
        private const float ContextCardTopInset = 12f;
        private const float ContextCardBottomInset = 10f;
        private const int MaxContextRowCount = 7;
        private const KeyCode ContextHintDismissKey = KeyCode.Backspace;
        private const float ContextBoundaryFocusStartViewportThreshold = 0.25f;
        private const float ContextBoundaryFocusFullViewportThreshold = 0.18f;
        private const float ContextBoundaryFocusMinAlpha = 0.02f;
        private const float ContextBoundaryFocusBlendSpeed = 18f;
        private const float ContextBoundaryFocusHardFadePressure = 0.58f;

        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese SDF",
            "Fonts & Materials/DialogueChinese SoftPixel SDF"
        };

        private const string FontCoverageProbeText = "工作台进入任务按E开始制作";

        private enum ContextHintGroup
        {
            None,
            Gameplay,
            Package
        }

        private struct ContextHintEntry
        {
            public string KeyLabel;
            public string Description;

            public ContextHintEntry(string keyLabel, string description)
            {
                KeyLabel = keyLabel;
                Description = description;
            }
        }

        private sealed class ContextHintRowRefs
        {
            public RectTransform Root;
            public RectTransform KeyPlateRect;
            public Image KeyPlateImage;
            public TextMeshProUGUI KeyText;
            public TextMeshProUGUI DescriptionText;
        }

        private static InteractionHintOverlay s_instance;
        private static Sprite s_backplateSprite;
        private static Texture2D s_backplateTexture;

        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private CanvasScaler overlayScaler;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;
        [SerializeField] private RectTransform cardRect;
        [SerializeField] private CanvasGroup interactionCardCanvasGroup;
        [SerializeField] private RectTransform keyPlateRect;
        [SerializeField] private Image cardImage;
        [SerializeField] private Image keyPlateImage;
        [SerializeField] private Image accentLineImage;
        [SerializeField] private TextMeshProUGUI keyText;
        [SerializeField] private TextMeshProUGUI captionText;
        [SerializeField] private TextMeshProUGUI detailText;
        [SerializeField] private RectTransform statusCardRect;
        [SerializeField] private CanvasGroup statusCardCanvasGroup;
        [SerializeField] private Image statusCardImage;
        [SerializeField] private Image statusAccentLineImage;
        [SerializeField] private Image statusTagImage;
        [SerializeField] private TextMeshProUGUI statusTagText;
        [SerializeField] private TextMeshProUGUI statusTitleText;
        [SerializeField] private TextMeshProUGUI statusDetailText;
        [SerializeField] private RectTransform contextCardRect;
        [SerializeField] private CanvasGroup contextCardCanvasGroup;
        [SerializeField] private Image contextCardImage;
        [SerializeField] private Image contextAccentLineImage;
        [SerializeField] private Image contextTagImage;
        [SerializeField] private TextMeshProUGUI contextTagText;
        [SerializeField] private TextMeshProUGUI contextTitleText;
        [SerializeField] private TextMeshProUGUI contextDetailText;
        [SerializeField] private RectTransform contextRowsRoot;
        [SerializeField] private TextMeshProUGUI contextFooterText;

        private TMP_FontAsset _fontAsset;
        private bool _visible;
        private bool _statusVisible;
        private bool _contextVisible;
        private bool _hasPlacementModeStateSample;
        private bool _lastPlacementModeState;
        private string _lastGuidanceSignature = string.Empty;
        private string _consumedGuidanceSignature = string.Empty;
        private Coroutine _statusVisibilityCoroutine;
        private GameInputManager _gameInputManager;
        private PlayerMovement _playerMovement;
        private SpringDay1Director _day1Director;
        private PackagePanelTabsUI _packageTabs;
        private SpringDay1WorkbenchCraftingOverlay _workbenchOverlay;
        private readonly List<ContextHintRowRefs> _contextRows = new();
        private readonly List<ContextHintEntry> _contextHintEntries = new();
        private ContextHintGroup _activeContextHintGroup = ContextHintGroup.None;
        private bool _gameplayContextDismissed;
        private bool _packageContextDismissed;
        private float _contextRequestedAlpha;
        private float _contextBoundaryAlpha = 1f;

        public bool IsVisible =>
            _visible &&
            overlayCanvas != null &&
            overlayCanvas.gameObject.activeSelf &&
            canvasGroup != null &&
            canvasGroup.alpha > 0.001f;
        public string CurrentKeyLabel => keyText != null ? keyText.text : string.Empty;
        public string CurrentCaptionText => captionText != null ? captionText.text : string.Empty;
        public string CurrentDetailText => detailText != null ? detailText.text : string.Empty;

        public static InteractionHintOverlay Instance
        {
            get
            {
                if (s_instance == null)
                {
                    EnsureRuntime();
                }

                return s_instance;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapRuntime()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            EnsureRuntime();
        }

        public static void EnsureRuntime()
        {
            bool adoptedOrCreated = false;
            if (s_instance == null)
            {
                s_instance = ResolvePrimaryRuntimeInstance();
                if (s_instance == null)
                {
                    Transform parent = SpringDay1UiLayerUtility.ResolveUiParent();
                    GameObject root = new GameObject(
                        nameof(InteractionHintOverlay),
                        typeof(RectTransform),
                        typeof(Canvas),
                        typeof(CanvasGroup));

                    if (parent != null)
                    {
                        root.transform.SetParent(parent, false);
                    }

                    s_instance = root.AddComponent<InteractionHintOverlay>();
                }

                adoptedOrCreated = true;
            }

            if (s_instance == null)
            {
                return;
            }

            s_instance.BuildUi();
            if (!s_instance.gameObject.activeSelf)
            {
                s_instance.gameObject.SetActive(true);
            }

            RetireDuplicateRuntimeInstances(s_instance);
            if (adoptedOrCreated)
            {
                s_instance.HideAllImmediate();
            }
        }

        public static void HideIfExists()
        {
            if (s_instance == null)
            {
                return;
            }

            s_instance.Hide();
        }

        private static InteractionHintOverlay ResolvePrimaryRuntimeInstance()
        {
            InteractionHintOverlay[] instances = FindObjectsByType<InteractionHintOverlay>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            if (instances == null || instances.Length == 0)
            {
                return null;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            InteractionHintOverlay best = null;
            int bestScore = int.MinValue;
            for (int index = 0; index < instances.Length; index++)
            {
                InteractionHintOverlay candidate = instances[index];
                if (candidate == null)
                {
                    continue;
                }

                int score = ScoreRuntimeInstance(candidate, activeScene);
                if (score > bestScore)
                {
                    best = candidate;
                    bestScore = score;
                }
            }

            return best;
        }

        private static int ScoreRuntimeInstance(InteractionHintOverlay candidate, Scene activeScene)
        {
            int score = 0;
            if (candidate == null)
            {
                return score;
            }

            GameObject candidateObject = candidate.gameObject;
            if (candidateObject.scene == activeScene)
            {
                score += 1000;
            }

            if (!candidateObject.activeSelf)
            {
                score += 120;
            }

            if (candidate.transform.parent != null)
            {
                score += 80;
            }

            if (candidate.cardRect != null)
            {
                score += 40;
            }

            if (candidate.statusCardRect != null)
            {
                score += 20;
            }

            return score;
        }

        private static void RetireDuplicateRuntimeInstances(InteractionHintOverlay primary)
        {
            InteractionHintOverlay[] instances = FindObjectsByType<InteractionHintOverlay>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            for (int index = 0; index < instances.Length; index++)
            {
                InteractionHintOverlay candidate = instances[index];
                if (candidate == null || candidate == primary)
                {
                    continue;
                }

                Destroy(candidate.gameObject);
            }
        }

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
        }

        private void OnEnable()
        {
            InteractionHintDisplaySettings.VisibilityChanged -= HandleHintVisibilityChanged;
            InteractionHintDisplaySettings.VisibilityChanged += HandleHintVisibilityChanged;
        }

        private void OnDisable()
        {
            InteractionHintDisplaySettings.VisibilityChanged -= HandleHintVisibilityChanged;
            StopStatusVisibilityCoroutine();
        }

        private void OnDestroy()
        {
            if (s_instance == this)
            {
                s_instance = null;
            }
        }

        private void LateUpdate()
        {
            SyncPlacementModeStatusCard();
            SyncContextHintCard();
            UpdateContextBoundaryFade();
        }

        public void ShowPrompt(string keyLabel, string caption, string detail = "")
        {
            EnsureBuilt();
            if (!InteractionHintDisplaySettings.AreHintsVisible)
            {
                HideAllImmediate();
                return;
            }

            keyText.text = string.IsNullOrWhiteSpace(keyLabel) ? string.Empty : keyLabel.Trim();
            captionText.text = NormalizeCaptionCopy(caption);
            detailText.text = NormalizeDetailCopy(detail, captionText.text, keyText.text);
            EnsureTextReadable(keyText);
            EnsureTextReadable(captionText);
            EnsureTextReadable(detailText);
            ApplyContentLayout();

            _visible = true;
            SetInteractionCardAlpha(1f);
            RefreshCardStackLayout();
            RefreshOverlayVisibility();
        }

        public void Hide()
        {
            _visible = false;
            SetInteractionCardAlpha(0f);
            RefreshCardStackLayout();
            RefreshOverlayVisibility();
        }

        private void HideAllImmediate()
        {
            _visible = false;
            _statusVisible = false;
            _contextVisible = false;
            _activeContextHintGroup = ContextHintGroup.None;
            _contextRequestedAlpha = 0f;
            _contextBoundaryAlpha = 1f;
            StopStatusVisibilityCoroutine();
            SetInteractionCardAlpha(0f);
            SetStatusCardAlpha(0f);
            SetContextCardAlpha(0f);
            RefreshOverlayVisibility();
        }

        private void HandleHintVisibilityChanged(bool visible)
        {
            if (!visible)
            {
                HideAllImmediate();
                return;
            }

            if (_visible || _statusVisible)
            {
                RefreshCardStackLayout();
                RefreshOverlayVisibility();
            }
        }

        private void BuildUi()
        {
            rootRect = GetComponent<RectTransform>();
            if (rootRect == null)
            {
                rootRect = gameObject.AddComponent<RectTransform>();
            }

            overlayCanvas = GetComponent<Canvas>();
            if (overlayCanvas == null)
            {
                overlayCanvas = gameObject.AddComponent<Canvas>();
            }

            overlayScaler = GetComponent<CanvasScaler>();
            if (overlayScaler == null)
            {
                overlayScaler = gameObject.AddComponent<CanvasScaler>();
            }

            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            _fontAsset = ResolveFontAsset();

            ConfigureOverlayRoot();

            RebindExistingInteractionElements();
            EnsureInteractionCardScaffold();
            ConfigureInteractionCardVisuals();

            RebindExistingStatusElements();
            EnsureStatusCardScaffold();
            ConfigureStatusCardVisuals();

            RebindExistingContextElements();
            EnsureContextCardScaffold();
            ConfigureContextCardVisuals();

            SetInteractionCardAlpha(_visible ? 1f : 0f);
            SetStatusCardAlpha(_statusVisible ? 1f : 0f);
            SetContextCardAlpha(_contextVisible ? 1f : 0f);
            RefreshCardStackLayout();
        }

        private void EnsureBuilt()
        {
            if (rootRect != null
                && overlayCanvas != null
                && overlayScaler != null
                && canvasGroup != null
                && cardRect != null
                && interactionCardCanvasGroup != null
                && cardImage != null
                && keyPlateRect != null
                && keyPlateImage != null
                && accentLineImage != null
                && keyText != null
                && captionText != null
                && detailText != null
                && statusCardRect != null
                && statusCardCanvasGroup != null
                && statusCardImage != null
                && statusAccentLineImage != null
                && statusTagImage != null
                && statusTagText != null
                && statusTitleText != null
                && statusDetailText != null
                && contextCardRect != null
                && contextCardCanvasGroup != null
                && contextCardImage != null
                && contextAccentLineImage != null
                && contextTagImage != null
                && contextTagText != null
                && contextTitleText != null
                && contextDetailText != null
                && contextRowsRoot != null
                && contextFooterText != null
                && _contextRows.Count >= MaxContextRowCount)
            {
                return;
            }

            BuildUi();
        }

        private void ConfigureOverlayRoot()
        {
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.overrideSorting = true;
            overlayCanvas.sortingOrder = 164;
            overlayCanvas.pixelPerfect = true;

            overlayScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            overlayScaler.referenceResolution = new Vector2(1980f, 1080f);
            overlayScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            overlayScaler.matchWidthOrHeight = 1f;
            overlayScaler.referencePixelsPerUnit = 16f;

            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;
        }

        private void RebindExistingInteractionElements()
        {
            cardRect = cardRect != null ? cardRect : ResolveUniqueRect(transform, "HintCard");
            if (cardRect == null)
            {
                return;
            }

            interactionCardCanvasGroup = interactionCardCanvasGroup != null
                ? interactionCardCanvasGroup
                : GetOrAddComponent<CanvasGroup>(cardRect.gameObject);
            cardImage = cardImage != null ? cardImage : GetOrAddComponent<Image>(cardRect.gameObject);

            keyPlateRect = keyPlateRect != null ? keyPlateRect : ResolveUniqueRect(cardRect, "KeyPlate");
            if (keyPlateRect != null)
            {
                keyPlateImage = keyPlateImage != null ? keyPlateImage : GetOrAddComponent<Image>(keyPlateRect.gameObject);
                keyText = keyText != null ? keyText : ResolveUniqueText(keyPlateRect, "KeyText");
            }

            accentLineImage = accentLineImage != null ? accentLineImage : ResolveUniqueImage(cardRect, "AccentLine");
            captionText = captionText != null ? captionText : ResolveUniqueText(cardRect, "CaptionText");
            detailText = detailText != null ? detailText : ResolveUniqueText(cardRect, "DetailText");
        }

        private void EnsureInteractionCardScaffold()
        {
            if (cardRect == null)
            {
                cardRect = CreateRect(transform, "HintCard");
            }

            interactionCardCanvasGroup = GetOrAddComponent<CanvasGroup>(cardRect.gameObject);
            cardImage = GetOrAddComponent<Image>(cardRect.gameObject);

            if (keyPlateRect == null)
            {
                keyPlateRect = CreateRect(cardRect, "KeyPlate");
            }

            keyPlateImage = GetOrAddComponent<Image>(keyPlateRect.gameObject);
            if (keyText == null)
            {
                keyText = CreateText(keyPlateRect, "KeyText", "E", 19f, new Color(0.22f, 0.14f, 0.05f, 1f), TextAlignmentOptions.Center);
            }

            if (accentLineImage == null)
            {
                accentLineImage = CreateRect(cardRect, "AccentLine").gameObject.AddComponent<Image>();
            }

            if (captionText == null)
            {
                captionText = CreateText(cardRect, "CaptionText", "可交互", 15f, new Color(0.98f, 0.96f, 0.91f, 1f), TextAlignmentOptions.Left);
            }

            if (detailText == null)
            {
                detailText = CreateText(cardRect, "DetailText", string.Empty, 11.5f, new Color(0.80f, 0.88f, 0.96f, 0.96f), TextAlignmentOptions.Left);
            }
        }

        private void ConfigureInteractionCardVisuals()
        {
            if (cardRect == null || keyPlateRect == null || accentLineImage == null || captionText == null || detailText == null || keyText == null)
            {
                return;
            }

            interactionCardCanvasGroup.interactable = false;
            interactionCardCanvasGroup.blocksRaycasts = false;

            cardRect.anchorMin = new Vector2(0f, 0f);
            cardRect.anchorMax = new Vector2(0f, 0f);
            cardRect.pivot = new Vector2(0f, 0f);
            cardRect.anchoredPosition = new Vector2(BaseCardX, BaseCardY);

            cardImage.sprite = GetOrCreateBackplateSprite();
            cardImage.type = Image.Type.Sliced;
            cardImage.color = new Color(0.08f, 0.11f, 0.16f, 0.95f);
            cardImage.raycastTarget = false;

            Outline outline = GetOrAddComponent<Outline>(cardRect.gameObject);
            outline.effectColor = new Color(1f, 0.82f, 0.45f, 0.16f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
            outline.useGraphicAlpha = true;

            Shadow shadow = GetOrAddComponent<Shadow>(cardRect.gameObject);
            shadow.effectColor = new Color(0f, 0f, 0f, 0.24f);
            shadow.effectDistance = new Vector2(0f, -4f);
            shadow.useGraphicAlpha = true;

            keyPlateRect.anchorMin = new Vector2(0f, 0.5f);
            keyPlateRect.anchorMax = new Vector2(0f, 0.5f);
            keyPlateRect.pivot = new Vector2(0f, 0.5f);
            keyPlateRect.anchoredPosition = new Vector2(12f, 0f);
            keyPlateRect.sizeDelta = new Vector2(38f, 38f);

            keyPlateImage.sprite = GetOrCreateBackplateSprite();
            keyPlateImage.type = Image.Type.Sliced;
            keyPlateImage.color = new Color(0.98f, 0.82f, 0.36f, 0.98f);
            keyPlateImage.raycastTarget = false;

            keyText.font = _fontAsset;
            if (_fontAsset != null && _fontAsset.material != null)
            {
                keyText.fontSharedMaterial = _fontAsset.material;
            }

            keyText.fontSize = 19f;
            keyText.color = new Color(0.22f, 0.14f, 0.05f, 1f);
            keyText.alignment = TextAlignmentOptions.Center;
            keyText.fontStyle = FontStyles.Bold;
            keyText.textWrappingMode = TextWrappingModes.NoWrap;
            keyText.overflowMode = TextOverflowModes.Ellipsis;
            keyText.raycastTarget = false;
            RectTransform keyTextRect = keyText.rectTransform;
            keyTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            keyTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            keyTextRect.pivot = new Vector2(0.5f, 0.5f);
            keyTextRect.anchoredPosition = Vector2.zero;
            keyTextRect.sizeDelta = new Vector2(26f, 24f);

            RectTransform accentRect = accentLineImage.rectTransform;
            accentRect.anchorMin = new Vector2(0f, 0.5f);
            accentRect.anchorMax = new Vector2(0f, 0.5f);
            accentRect.pivot = new Vector2(0f, 0.5f);
            accentRect.anchoredPosition = new Vector2(58f, 0f);
            accentRect.sizeDelta = new Vector2(4f, 38f);
            accentLineImage.raycastTarget = false;

            captionText.font = _fontAsset;
            if (_fontAsset != null && _fontAsset.material != null)
            {
                captionText.fontSharedMaterial = _fontAsset.material;
            }

            captionText.fontSize = 15f;
            captionText.color = new Color(0.98f, 0.96f, 0.91f, 1f);
            captionText.alignment = TextAlignmentOptions.Left;
            captionText.fontStyle = FontStyles.Bold;
            captionText.textWrappingMode = TextWrappingModes.Normal;
            captionText.overflowMode = TextOverflowModes.Overflow;
            captionText.raycastTarget = false;
            RectTransform captionRect = captionText.rectTransform;
            captionRect.anchorMin = new Vector2(0f, 1f);
            captionRect.anchorMax = new Vector2(1f, 1f);
            captionRect.pivot = new Vector2(0f, 1f);

            detailText.font = _fontAsset;
            if (_fontAsset != null && _fontAsset.material != null)
            {
                detailText.fontSharedMaterial = _fontAsset.material;
            }

            detailText.fontSize = 11.5f;
            detailText.color = new Color(0.80f, 0.88f, 0.96f, 0.96f);
            detailText.alignment = TextAlignmentOptions.Left;
            detailText.textWrappingMode = TextWrappingModes.Normal;
            detailText.overflowMode = TextOverflowModes.Overflow;
            detailText.raycastTarget = false;
            RectTransform detailRect = detailText.rectTransform;
            detailRect.anchorMin = new Vector2(0f, 0f);
            detailRect.anchorMax = new Vector2(1f, 0f);
            detailRect.pivot = new Vector2(0f, 0f);

            ApplyContentLayout();
        }

        private void RebindExistingStatusElements()
        {
            statusCardRect = statusCardRect != null ? statusCardRect : ResolveUniqueRect(transform, "PlacementStatusCard");
            if (statusCardRect == null)
            {
                return;
            }

            statusCardCanvasGroup = statusCardCanvasGroup != null
                ? statusCardCanvasGroup
                : GetOrAddComponent<CanvasGroup>(statusCardRect.gameObject);
            statusCardImage = statusCardImage != null ? statusCardImage : GetOrAddComponent<Image>(statusCardRect.gameObject);
            statusAccentLineImage = statusAccentLineImage != null ? statusAccentLineImage : ResolveUniqueImage(statusCardRect, "StatusAccentLine");
            statusTagImage = statusTagImage != null ? statusTagImage : ResolveUniqueImage(statusCardRect, "StatusTag");
            if (statusTagImage != null)
            {
                statusTagText = statusTagText != null ? statusTagText : ResolveUniqueText(statusTagImage.rectTransform, "StatusTagText");
            }

            statusTitleText = statusTitleText != null ? statusTitleText : ResolveUniqueText(statusCardRect, "StatusTitleText");
            statusDetailText = statusDetailText != null ? statusDetailText : ResolveUniqueText(statusCardRect, "StatusDetailText");
        }

        private void RebindExistingContextElements()
        {
            contextCardRect = contextCardRect != null ? contextCardRect : ResolveUniqueRect(transform, "ContextHintCard");
            if (contextCardRect == null)
            {
                return;
            }

            contextCardCanvasGroup = contextCardCanvasGroup != null
                ? contextCardCanvasGroup
                : GetOrAddComponent<CanvasGroup>(contextCardRect.gameObject);
            contextCardImage = contextCardImage != null ? contextCardImage : GetOrAddComponent<Image>(contextCardRect.gameObject);
            contextAccentLineImage = contextAccentLineImage != null ? contextAccentLineImage : ResolveUniqueImage(contextCardRect, "ContextAccentLine");
            contextTagImage = contextTagImage != null ? contextTagImage : ResolveUniqueImage(contextCardRect, "ContextTag");
            if (contextTagImage != null)
            {
                contextTagText = contextTagText != null ? contextTagText : ResolveUniqueText(contextTagImage.rectTransform, "ContextTagText");
            }

            contextTitleText = contextTitleText != null ? contextTitleText : ResolveUniqueText(contextCardRect, "ContextTitleText");
            contextDetailText = contextDetailText != null ? contextDetailText : ResolveUniqueText(contextCardRect, "ContextDetailText");
            contextRowsRoot = contextRowsRoot != null ? contextRowsRoot : ResolveUniqueRect(contextCardRect, "ContextRows");
            contextFooterText = contextFooterText != null ? contextFooterText : ResolveUniqueText(contextCardRect, "ContextFooterText");

            _contextRows.Clear();
            if (contextRowsRoot == null)
            {
                return;
            }

            for (int index = 0; index < MaxContextRowCount; index++)
            {
                RectTransform rowRect = ResolveUniqueRect(contextRowsRoot, $"ContextRow_{index}");
                if (rowRect == null)
                {
                    continue;
                }

                RectTransform keyPlateRect = ResolveUniqueRect(rowRect, "KeyPlate");
                TextMeshProUGUI keyLabel = keyPlateRect != null ? ResolveUniqueText(keyPlateRect, "KeyText") : null;
                TextMeshProUGUI description = ResolveUniqueText(rowRect, "DescriptionText");
                _contextRows.Add(new ContextHintRowRefs
                {
                    Root = rowRect,
                    KeyPlateRect = keyPlateRect,
                    KeyPlateImage = keyPlateRect != null ? GetOrAddComponent<Image>(keyPlateRect.gameObject) : null,
                    KeyText = keyLabel,
                    DescriptionText = description
                });
            }
        }

        private void EnsureStatusCardScaffold()
        {
            if (statusCardRect == null)
            {
                statusCardRect = CreateRect(transform, "PlacementStatusCard");
            }

            statusCardCanvasGroup = GetOrAddComponent<CanvasGroup>(statusCardRect.gameObject);
            statusCardImage = GetOrAddComponent<Image>(statusCardRect.gameObject);

            if (statusAccentLineImage == null)
            {
                statusAccentLineImage = CreateRect(statusCardRect, "StatusAccentLine").gameObject.AddComponent<Image>();
            }

            if (statusTagImage == null)
            {
                statusTagImage = CreateRect(statusCardRect, "StatusTag").gameObject.AddComponent<Image>();
            }

            if (statusTagText == null)
            {
                statusTagText = CreateText(statusTagImage.rectTransform, "StatusTagText", "状态", 8.5f, new Color(0.96f, 0.95f, 0.9f, 1f), TextAlignmentOptions.Center);
            }

            if (statusTitleText == null)
            {
                statusTitleText = CreateText(statusCardRect, "StatusTitleText", "放置模式", 13.5f, new Color(0.98f, 0.96f, 0.92f, 1f), TextAlignmentOptions.Left);
            }

            if (statusDetailText == null)
            {
                statusDetailText = CreateText(statusCardRect, "StatusDetailText", string.Empty, 10.5f, new Color(0.78f, 0.87f, 0.95f, 0.96f), TextAlignmentOptions.Left);
            }
        }

        private void EnsureContextCardScaffold()
        {
            if (contextCardRect == null)
            {
                contextCardRect = CreateRect(transform, "ContextHintCard");
            }

            contextCardCanvasGroup = GetOrAddComponent<CanvasGroup>(contextCardRect.gameObject);
            contextCardImage = GetOrAddComponent<Image>(contextCardRect.gameObject);

            if (contextAccentLineImage == null)
            {
                contextAccentLineImage = CreateRect(contextCardRect, "ContextAccentLine").gameObject.AddComponent<Image>();
            }

            if (contextTagImage == null)
            {
                contextTagImage = CreateRect(contextCardRect, "ContextTag").gameObject.AddComponent<Image>();
            }

            if (contextTagText == null)
            {
                contextTagText = CreateText(contextTagImage.rectTransform, "ContextTagText", "玩法", 8.5f, new Color(0.96f, 0.95f, 0.9f, 1f), TextAlignmentOptions.Center);
            }

            if (contextTitleText == null)
            {
                contextTitleText = CreateText(contextCardRect, "ContextTitleText", "当前操作", 14.5f, new Color(0.98f, 0.96f, 0.92f, 1f), TextAlignmentOptions.Left);
            }

            if (contextDetailText == null)
            {
                contextDetailText = CreateText(contextCardRect, "ContextDetailText", "只显示当前会用到的操作。", 10.5f, new Color(0.78f, 0.87f, 0.95f, 0.96f), TextAlignmentOptions.Left);
            }

            if (contextRowsRoot == null)
            {
                contextRowsRoot = CreateRect(contextCardRect, "ContextRows");
            }

            if (contextFooterText == null)
            {
                contextFooterText = CreateText(contextCardRect, "ContextFooterText", "退格关闭这组提示", 10f, new Color(0.72f, 0.79f, 0.87f, 0.92f), TextAlignmentOptions.Left);
            }

            for (int index = _contextRows.Count; index < MaxContextRowCount; index++)
            {
                RectTransform rowRect = CreateRect(contextRowsRoot, $"ContextRow_{index}");
                RectTransform keyPlateRect = CreateRect(rowRect, "KeyPlate");
                Image keyPlateImage = keyPlateRect.gameObject.AddComponent<Image>();
                TextMeshProUGUI keyLabel = CreateText(keyPlateRect, "KeyText", string.Empty, 10.5f, new Color(0.16f, 0.12f, 0.05f, 1f), TextAlignmentOptions.Center);
                TextMeshProUGUI description = CreateText(rowRect, "DescriptionText", string.Empty, 11.5f, new Color(0.98f, 0.96f, 0.92f, 1f), TextAlignmentOptions.Left);
                _contextRows.Add(new ContextHintRowRefs
                {
                    Root = rowRect,
                    KeyPlateRect = keyPlateRect,
                    KeyPlateImage = keyPlateImage,
                    KeyText = keyLabel,
                    DescriptionText = description
                });
            }
        }

        private void ConfigureStatusCardVisuals()
        {
            if (statusCardRect == null
                || statusCardCanvasGroup == null
                || statusCardImage == null
                || statusAccentLineImage == null
                || statusTagImage == null
                || statusTagText == null
                || statusTitleText == null
                || statusDetailText == null)
            {
                return;
            }

            statusCardCanvasGroup.interactable = false;
            statusCardCanvasGroup.blocksRaycasts = false;

            statusCardRect.anchorMin = new Vector2(0f, 0f);
            statusCardRect.anchorMax = new Vector2(0f, 0f);
            statusCardRect.pivot = new Vector2(0f, 0f);
            statusCardRect.anchoredPosition = new Vector2(BaseCardX, BaseCardY);

            statusCardImage.sprite = GetOrCreateBackplateSprite();
            statusCardImage.type = Image.Type.Sliced;
            statusCardImage.color = new Color(0.11f, 0.15f, 0.19f, 0.95f);
            statusCardImage.raycastTarget = false;

            Outline statusOutline = GetOrAddComponent<Outline>(statusCardRect.gameObject);
            statusOutline.effectColor = new Color(0.92f, 0.84f, 0.48f, 0.12f);
            statusOutline.effectDistance = new Vector2(1.25f, -1.25f);
            statusOutline.useGraphicAlpha = true;

            Shadow statusShadow = GetOrAddComponent<Shadow>(statusCardRect.gameObject);
            statusShadow.effectColor = new Color(0f, 0f, 0f, 0.22f);
            statusShadow.effectDistance = new Vector2(0f, -3f);
            statusShadow.useGraphicAlpha = true;

            RectTransform statusAccentRect = statusAccentLineImage.rectTransform;
            statusAccentRect.anchorMin = new Vector2(0f, 0.5f);
            statusAccentRect.anchorMax = new Vector2(0f, 0.5f);
            statusAccentRect.pivot = new Vector2(0f, 0.5f);
            statusAccentRect.anchoredPosition = new Vector2(12f, 0f);
            statusAccentRect.sizeDelta = new Vector2(4f, 47.08f);
            statusAccentLineImage.raycastTarget = false;

            RectTransform statusTagRect = statusTagImage.rectTransform;
            statusTagRect.anchorMin = new Vector2(0f, 1f);
            statusTagRect.anchorMax = new Vector2(0f, 1f);
            statusTagRect.pivot = new Vector2(0f, 1f);
            statusTagRect.anchoredPosition = new Vector2(29f, -16f);
            statusTagRect.sizeDelta = new Vector2(44f, 18f);
            statusTagImage.sprite = GetOrCreateBackplateSprite();
            statusTagImage.type = Image.Type.Sliced;
            statusTagImage.raycastTarget = false;

            statusTagText.font = _fontAsset;
            if (_fontAsset != null && _fontAsset.material != null)
            {
                statusTagText.fontSharedMaterial = _fontAsset.material;
            }

            statusTagText.fontSize = 10.25f;
            statusTagText.color = new Color(0.96f, 0.95f, 0.9f, 1f);
            statusTagText.alignment = TextAlignmentOptions.Center;
            statusTagText.fontStyle = FontStyles.Bold;
            statusTagText.textWrappingMode = TextWrappingModes.NoWrap;
            statusTagText.overflowMode = TextOverflowModes.Ellipsis;
            statusTagText.raycastTarget = false;
            RectTransform statusTagTextRect = statusTagText.rectTransform;
            statusTagTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            statusTagTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            statusTagTextRect.pivot = new Vector2(0.5f, 0.5f);
            statusTagTextRect.anchoredPosition = Vector2.zero;
            statusTagTextRect.sizeDelta = new Vector2(34f, 12f);

            statusTitleText.font = _fontAsset;
            if (_fontAsset != null && _fontAsset.material != null)
            {
                statusTitleText.fontSharedMaterial = _fontAsset.material;
            }

            statusTitleText.fontSize = 15.5f;
            statusTitleText.color = new Color(0.98f, 0.96f, 0.92f, 1f);
            statusTitleText.alignment = TextAlignmentOptions.Left;
            statusTitleText.fontStyle = FontStyles.Bold;
            statusTitleText.textWrappingMode = TextWrappingModes.Normal;
            statusTitleText.overflowMode = TextOverflowModes.Overflow;
            statusTitleText.raycastTarget = false;
            RectTransform statusTitleRect = statusTitleText.rectTransform;
            statusTitleRect.anchorMin = new Vector2(0f, 1f);
            statusTitleRect.anchorMax = new Vector2(1f, 1f);
            statusTitleRect.pivot = new Vector2(0f, 1f);

            statusDetailText.font = _fontAsset;
            if (_fontAsset != null && _fontAsset.material != null)
            {
                statusDetailText.fontSharedMaterial = _fontAsset.material;
            }

            statusDetailText.fontSize = 11.5f;
            statusDetailText.color = new Color(0.78f, 0.87f, 0.95f, 0.96f);
            statusDetailText.alignment = TextAlignmentOptions.Left;
            statusDetailText.textWrappingMode = TextWrappingModes.Normal;
            statusDetailText.overflowMode = TextOverflowModes.Overflow;
            statusDetailText.raycastTarget = false;
            RectTransform statusDetailRect = statusDetailText.rectTransform;
            statusDetailRect.anchorMin = new Vector2(0f, 0f);
            statusDetailRect.anchorMax = new Vector2(1f, 0f);
            statusDetailRect.pivot = new Vector2(0f, 0f);

            statusTagImage.rectTransform.SetAsLastSibling();
            statusDetailText.gameObject.SetActive(!string.IsNullOrWhiteSpace(statusDetailText.text));
            ApplyStatusLayout();
        }

        private void ConfigureContextCardVisuals()
        {
            if (contextCardRect == null
                || contextCardCanvasGroup == null
                || contextCardImage == null
                || contextAccentLineImage == null
                || contextTagImage == null
                || contextTagText == null
                || contextTitleText == null
                || contextDetailText == null
                || contextRowsRoot == null
                || contextFooterText == null)
            {
                return;
            }

            contextCardCanvasGroup.interactable = false;
            contextCardCanvasGroup.blocksRaycasts = false;

            contextCardRect.anchorMin = new Vector2(1f, 0.5f);
            contextCardRect.anchorMax = new Vector2(1f, 0.5f);
            contextCardRect.pivot = new Vector2(1f, 0.5f);
            contextCardRect.anchoredPosition = new Vector2(-ContextCardRightInset, ContextCardCenterOffsetY);
            contextCardRect.sizeDelta = new Vector2(ContextCardWidth, ContextCardMinHeight);

            contextCardImage.sprite = GetOrCreateBackplateSprite();
            contextCardImage.type = Image.Type.Sliced;
            contextCardImage.color = new Color(0.11f, 0.14f, 0.19f, 0.92f);
            contextCardImage.raycastTarget = false;

            Outline contextOutline = GetOrAddComponent<Outline>(contextCardRect.gameObject);
            contextOutline.effectColor = new Color(0.72f, 0.82f, 0.94f, 0.035f);
            contextOutline.effectDistance = new Vector2(0.8f, -0.8f);
            contextOutline.useGraphicAlpha = true;

            Shadow contextShadow = GetOrAddComponent<Shadow>(contextCardRect.gameObject);
            contextShadow.effectColor = new Color(0f, 0f, 0f, 0.14f);
            contextShadow.effectDistance = new Vector2(0f, -2f);
            contextShadow.useGraphicAlpha = true;

            RectTransform accentRect = contextAccentLineImage.rectTransform;
            accentRect.anchorMin = new Vector2(0f, 1f);
            accentRect.anchorMax = new Vector2(0f, 1f);
            accentRect.pivot = new Vector2(0f, 1f);
            accentRect.anchoredPosition = new Vector2(10f, -ContextCardTopInset);
            accentRect.sizeDelta = new Vector2(3f, 22f);
            contextAccentLineImage.raycastTarget = false;

            RectTransform tagRect = contextTagImage.rectTransform;
            tagRect.anchorMin = new Vector2(0f, 1f);
            tagRect.anchorMax = new Vector2(0f, 1f);
            tagRect.pivot = new Vector2(0f, 1f);
            tagRect.anchoredPosition = new Vector2(18f, -ContextCardTopInset);
            tagRect.sizeDelta = new Vector2(36f, 14f);
            contextTagImage.sprite = GetOrCreateBackplateSprite();
            contextTagImage.type = Image.Type.Sliced;
            contextTagImage.raycastTarget = false;

            contextTagText.font = _fontAsset;
            if (_fontAsset != null && _fontAsset.material != null)
            {
                contextTagText.fontSharedMaterial = _fontAsset.material;
            }

            contextTagText.fontSize = 8f;
            contextTagText.color = new Color(0.96f, 0.95f, 0.9f, 1f);
            contextTagText.alignment = TextAlignmentOptions.Center;
            contextTagText.fontStyle = FontStyles.Bold;
            contextTagText.textWrappingMode = TextWrappingModes.NoWrap;
            contextTagText.overflowMode = TextOverflowModes.Ellipsis;
            contextTagText.raycastTarget = false;
            RectTransform contextTagTextRect = contextTagText.rectTransform;
            contextTagTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            contextTagTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            contextTagTextRect.pivot = new Vector2(0.5f, 0.5f);
            contextTagTextRect.anchoredPosition = Vector2.zero;
            contextTagTextRect.sizeDelta = new Vector2(26f, 10f);

            contextTitleText.font = _fontAsset;
            if (_fontAsset != null && _fontAsset.material != null)
            {
                contextTitleText.fontSharedMaterial = _fontAsset.material;
            }

            contextTitleText.fontSize = 12f;
            contextTitleText.color = new Color(0.98f, 0.96f, 0.92f, 1f);
            contextTitleText.alignment = TextAlignmentOptions.Left;
            contextTitleText.fontStyle = FontStyles.Bold;
            contextTitleText.textWrappingMode = TextWrappingModes.NoWrap;
            contextTitleText.overflowMode = TextOverflowModes.Ellipsis;
            contextTitleText.raycastTarget = false;

            contextDetailText.font = _fontAsset;
            if (_fontAsset != null && _fontAsset.material != null)
            {
                contextDetailText.fontSharedMaterial = _fontAsset.material;
            }

            contextDetailText.fontSize = 8.75f;
            contextDetailText.color = new Color(0.78f, 0.87f, 0.95f, 0.96f);
            contextDetailText.alignment = TextAlignmentOptions.Left;
            contextDetailText.textWrappingMode = TextWrappingModes.Normal;
            contextDetailText.overflowMode = TextOverflowModes.Overflow;
            contextDetailText.raycastTarget = false;

            contextRowsRoot.anchorMin = new Vector2(0f, 1f);
            contextRowsRoot.anchorMax = new Vector2(0f, 1f);
            contextRowsRoot.pivot = new Vector2(0f, 1f);
            contextRowsRoot.anchoredPosition = new Vector2(0f, 0f);
            contextRowsRoot.sizeDelta = new Vector2(ContextCardWidth, 0f);

            contextFooterText.font = _fontAsset;
            if (_fontAsset != null && _fontAsset.material != null)
            {
                contextFooterText.fontSharedMaterial = _fontAsset.material;
            }

            contextFooterText.fontSize = 8f;
            contextFooterText.color = new Color(0.72f, 0.79f, 0.87f, 0.72f);
            contextFooterText.alignment = TextAlignmentOptions.Right;
            contextFooterText.textWrappingMode = TextWrappingModes.NoWrap;
            contextFooterText.overflowMode = TextOverflowModes.Ellipsis;
            contextFooterText.raycastTarget = false;

            for (int index = 0; index < _contextRows.Count; index++)
            {
                ContextHintRowRefs row = _contextRows[index];
                if (row == null || row.Root == null || row.KeyPlateRect == null || row.KeyPlateImage == null || row.KeyText == null || row.DescriptionText == null)
                {
                    continue;
                }

                row.Root.anchorMin = new Vector2(0f, 1f);
                row.Root.anchorMax = new Vector2(0f, 1f);
                row.Root.pivot = new Vector2(0f, 1f);
                row.Root.sizeDelta = new Vector2(ContextCardWidth - ContextCardInnerLeft - ContextCardInnerRight, ContextCardRowHeight);

                row.KeyPlateRect.anchorMin = new Vector2(0f, 0.5f);
                row.KeyPlateRect.anchorMax = new Vector2(0f, 0.5f);
                row.KeyPlateRect.pivot = new Vector2(0f, 0.5f);
                row.KeyPlateRect.sizeDelta = new Vector2(56f, 16f);
                row.KeyPlateImage.sprite = GetOrCreateBackplateSprite();
                row.KeyPlateImage.type = Image.Type.Sliced;
                row.KeyPlateImage.color = new Color(0.90f, 0.75f, 0.33f, 0.96f);
                row.KeyPlateImage.raycastTarget = false;

                row.KeyText.font = _fontAsset;
                if (_fontAsset != null && _fontAsset.material != null)
                {
                    row.KeyText.fontSharedMaterial = _fontAsset.material;
                }

                row.KeyText.fontSize = 8.75f;
                row.KeyText.color = new Color(0.16f, 0.12f, 0.05f, 1f);
                row.KeyText.alignment = TextAlignmentOptions.Center;
                row.KeyText.fontStyle = FontStyles.Bold;
                row.KeyText.textWrappingMode = TextWrappingModes.NoWrap;
                row.KeyText.overflowMode = TextOverflowModes.Ellipsis;
                row.KeyText.raycastTarget = false;
                RectTransform keyTextRect = row.KeyText.rectTransform;
                keyTextRect.anchorMin = new Vector2(0.5f, 0.5f);
                keyTextRect.anchorMax = new Vector2(0.5f, 0.5f);
                keyTextRect.pivot = new Vector2(0.5f, 0.5f);
                keyTextRect.anchoredPosition = Vector2.zero;
                keyTextRect.sizeDelta = new Vector2(48f, 11f);

                row.DescriptionText.font = _fontAsset;
                if (_fontAsset != null && _fontAsset.material != null)
                {
                    row.DescriptionText.fontSharedMaterial = _fontAsset.material;
                }

                row.DescriptionText.fontSize = 9.75f;
                row.DescriptionText.color = new Color(0.98f, 0.96f, 0.92f, 1f);
                row.DescriptionText.alignment = TextAlignmentOptions.Left;
                row.DescriptionText.textWrappingMode = TextWrappingModes.NoWrap;
                row.DescriptionText.overflowMode = TextOverflowModes.Ellipsis;
                row.DescriptionText.raycastTarget = false;
            }

            ApplyContextHintCardLayout();
        }

        private void ApplyContentLayout()
        {
            bool hasDetail = !string.IsNullOrWhiteSpace(detailText != null ? detailText.text : string.Empty);
            bool hasKeyLabel = !string.IsNullOrWhiteSpace(keyText != null ? keyText.text : string.Empty);

            if (detailText != null)
            {
                detailText.gameObject.SetActive(hasDetail);
            }

            if (keyPlateRect != null)
            {
                keyPlateRect.gameObject.SetActive(hasKeyLabel);
            }

            float leftInset = hasKeyLabel ? 70f : 18f;
            if (cardRect != null)
            {
                cardRect.sizeDelta = hasDetail
                    ? new Vector2(DetailCardWidth, DetailCardHeight)
                    : new Vector2(CompactCardWidth, CompactCardHeight);
            }

            if (accentLineImage != null)
            {
                accentLineImage.color = hasKeyLabel
                    ? new Color(0.98f, 0.70f, 0.31f, 0.92f)
                    : new Color(0.72f, 0.82f, 0.94f, 0.72f);
            }

            if (captionText != null)
            {
                RectTransform captionRect = captionText.rectTransform;
                captionRect.offsetMin = new Vector2(leftInset, hasDetail ? -30f : -20f);
                captionRect.offsetMax = new Vector2(-12f, -10f);
            }

            if (detailText != null)
            {
                detailText.color = hasDetail
                    ? new Color(0.80f, 0.88f, 0.96f, 0.96f)
                    : detailText.color;
                RectTransform detailRect = detailText.rectTransform;
                detailRect.offsetMin = new Vector2(leftInset, 12f);
                detailRect.offsetMax = new Vector2(-12f, hasDetail ? 30f : 22f);
            }

            RefreshCardStackLayout();
        }

        private void SyncPlacementModeStatusCard()
        {
            EnsureBuilt();

            if (!InteractionHintDisplaySettings.AreHintsVisible)
            {
                HidePlacementStatusImmediate();
                return;
            }

            bool blocked = IsPlacementStatusBlocked();
            bool hasInput = TryResolveGameInputManager(out GameInputManager inputManager);
            bool hasDirector = TryResolveDay1Director(out SpringDay1Director day1Director);
            bool placementModeEnabled = hasInput && inputManager.IsPlacementMode;
            string guidanceText = hasDirector && day1Director.ShouldShowPlacementModeGuidance()
                ? day1Director.GetPlacementModeGuidanceText()
                : string.Empty;
            string guidanceSignature = string.IsNullOrWhiteSpace(guidanceText) ? string.Empty : guidanceText.Trim();

            if (!_hasPlacementModeStateSample)
            {
                _lastPlacementModeState = placementModeEnabled;
                _hasPlacementModeStateSample = true;
            }

            if (!string.Equals(_lastGuidanceSignature, guidanceSignature))
            {
                _lastGuidanceSignature = guidanceSignature;
                _consumedGuidanceSignature = string.Empty;
            }

            if (blocked)
            {
                HidePlacementStatusImmediate();
                _lastPlacementModeState = placementModeEnabled;
                return;
            }

            if (_lastPlacementModeState != placementModeEnabled)
            {
                string toggleDetail = placementModeEnabled
                    ? "农田/播种/浇水输入已开启，按 V 关闭。"
                    : "农田/播种/浇水输入已关闭，按 V 开启。";
                ShowPlacementStatus(
                    placementModeEnabled ? "放置模式已开启" : "放置模式已关闭",
                    toggleDetail,
                    placementModeEnabled ? new Color(0.43f, 0.82f, 0.58f, 0.96f) : new Color(0.82f, 0.59f, 0.42f, 0.96f),
                    placementModeEnabled ? new Color(0.18f, 0.28f, 0.18f, 0.92f) : new Color(0.26f, 0.19f, 0.15f, 0.92f));
                if (!string.IsNullOrWhiteSpace(guidanceSignature))
                {
                    _consumedGuidanceSignature = guidanceSignature;
                }

                _lastPlacementModeState = placementModeEnabled;
                return;
            }

            if (!placementModeEnabled
                && !string.IsNullOrWhiteSpace(guidanceSignature)
                && !string.Equals(_consumedGuidanceSignature, guidanceSignature))
            {
                ShowPlacementStatus(
                    "放置模式提示",
                    guidanceText,
                    new Color(0.96f, 0.78f, 0.34f, 0.96f),
                    new Color(0.24f, 0.20f, 0.12f, 0.94f));
                _consumedGuidanceSignature = guidanceSignature;
            }

            _lastPlacementModeState = placementModeEnabled;
        }

        private bool TryResolveGameInputManager(out GameInputManager gameInputManager)
        {
            if (_gameInputManager == null)
            {
                _gameInputManager = FindFirstObjectByType<GameInputManager>(FindObjectsInactive.Include);
            }

            gameInputManager = _gameInputManager;
            return gameInputManager != null;
        }

        private bool TryResolveDay1Director(out SpringDay1Director day1Director)
        {
            if (_day1Director == null)
            {
                _day1Director = SpringDay1Director.Instance != null
                    ? SpringDay1Director.Instance
                    : FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);
            }

            day1Director = _day1Director;
            return day1Director != null;
        }

        private bool IsPlacementStatusBlocked()
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return true;
            }

            return SpringDay1UiLayerUtility.IsBlockingPageUiOpen();
        }

        private void ShowPlacementStatus(string title, string detail, Color accentColor, Color surfaceColor)
        {
            EnsureBuilt();

            statusTitleText.text = string.IsNullOrWhiteSpace(title) ? "放置模式" : title.Trim();
            statusDetailText.text = string.IsNullOrWhiteSpace(detail) ? string.Empty : detail.Trim();
            statusCardImage.color = surfaceColor;
            statusAccentLineImage.color = accentColor;
            statusTagImage.color = new Color(accentColor.r, accentColor.g, accentColor.b, 0.2f);
            statusTagText.color = new Color(0.96f, 0.95f, 0.9f, 1f);
            statusTitleText.color = new Color(0.98f, 0.96f, 0.92f, 1f);
            statusDetailText.color = new Color(0.78f, 0.87f, 0.95f, 0.96f);
            statusDetailText.gameObject.SetActive(!string.IsNullOrWhiteSpace(statusDetailText.text));

            EnsureTextReadable(statusTitleText);
            EnsureTextReadable(statusDetailText);
            ApplyStatusLayout();

            bool keepVisible = statusCardCanvasGroup != null && statusCardCanvasGroup.alpha > 0.85f;
            StopStatusVisibilityCoroutine();
            _statusVisibilityCoroutine = StartCoroutine(PlayStatusCardVisibility(keepVisible));
        }

        private void ApplyStatusLayout()
        {
            bool hasDetail = !string.IsNullOrWhiteSpace(statusDetailText != null ? statusDetailText.text : string.Empty);
            if (statusCardRect != null)
            {
                statusCardRect.sizeDelta = new Vector2(StatusCardWidth, hasDetail ? StatusCardDetailHeight : StatusCardCompactHeight);
            }

            if (statusTitleText != null)
            {
                RectTransform titleRect = statusTitleText.rectTransform;
                titleRect.offsetMin = new Vector2(88f, hasDetail ? -36f : -44f);
                titleRect.offsetMax = new Vector2(-18f, hasDetail ? -12f : -18f);
            }

            if (statusDetailText != null)
            {
                RectTransform detailRect = statusDetailText.rectTransform;
                detailRect.offsetMin = new Vector2(hasDetail ? 29.8f : 22f, 12f);
                detailRect.offsetMax = new Vector2(hasDetail ? -10.2f : -18f, hasDetail ? 34f : 24f);
            }

            RefreshCardStackLayout();
        }

        private void SyncContextHintCard()
        {
            EnsureBuilt();

            ContextHintGroup targetGroup = ResolveCurrentContextHintGroup();
            bool contextChanged = _activeContextHintGroup != targetGroup;
            if (Input.GetKeyDown(ContextHintDismissKey) && targetGroup != ContextHintGroup.None)
            {
                SetContextDismissed(targetGroup, true);
            }

            if (targetGroup == ContextHintGroup.None || IsContextDismissed(targetGroup))
            {
                HideContextHintImmediate();
                return;
            }

            if (contextChanged || targetGroup == ContextHintGroup.Gameplay || !_contextVisible)
            {
                ApplyContextHintCard(targetGroup);
            }

            _activeContextHintGroup = targetGroup;
            _contextVisible = true;
            SetContextCardAlpha(1f);
            RefreshOverlayVisibility();
        }

        private ContextHintGroup ResolveCurrentContextHintGroup()
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return ContextHintGroup.None;
            }

            if (BoxPanelUI.ActiveInstance != null && BoxPanelUI.ActiveInstance.IsOpen)
            {
                return ContextHintGroup.None;
            }

            if (_packageTabs == null)
            {
                _packageTabs = FindFirstObjectByType<PackagePanelTabsUI>(FindObjectsInactive.Include);
            }

            if (_packageTabs != null && _packageTabs.IsPanelOpen())
            {
                return ContextHintGroup.Package;
            }

            if (_workbenchOverlay == null)
            {
                _workbenchOverlay = FindFirstObjectByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include);
            }

            if (_workbenchOverlay != null && _workbenchOverlay.IsVisible)
            {
                return ContextHintGroup.None;
            }

            return ContextHintGroup.Gameplay;
        }

        private void ApplyContextHintCard(ContextHintGroup group)
        {
            _contextHintEntries.Clear();

            bool placementModeEnabled = TryResolveGameInputManager(out GameInputManager inputManager) && inputManager.IsPlacementMode;
            Color accentColor;
            Color surfaceColor;
            switch (group)
            {
                case ContextHintGroup.Package:
                    contextTagText.text = "背包";
                    contextTitleText.text = "常用操作";
                    contextDetailText.text = string.Empty;
                    accentColor = new Color(0.50f, 0.76f, 0.92f, 0.96f);
                    surfaceColor = new Color(0.08f, 0.12f, 0.18f, 0.95f);
                    _contextHintEntries.Add(new ContextHintEntry("左键", "选中 / 拖拽"));
                    _contextHintEntries.Add(new ContextHintEntry("Shift+左", "二分"));
                    _contextHintEntries.Add(new ContextHintEntry("Ctrl+左", "单取"));
                    _contextHintEntries.Add(new ContextHintEntry("B/M/L/O", "切页"));
                    _contextHintEntries.Add(new ContextHintEntry("Tab", "物品页 / 收起"));
                    _contextHintEntries.Add(new ContextHintEntry("Esc", "设置页"));
                    break;

                default:
                    contextTagText.text = "玩法";
                    contextTitleText.text = "常用操作";
                    contextDetailText.text = placementModeEnabled
                        ? string.Empty
                        : string.Empty;
                    accentColor = new Color(0.96f, 0.74f, 0.34f, 0.96f);
                    surfaceColor = new Color(0.09f, 0.13f, 0.18f, 0.95f);
                    _contextHintEntries.Add(new ContextHintEntry("右键", "导航"));
                    _contextHintEntries.Add(new ContextHintEntry("左键", placementModeEnabled ? "使用 / 放置" : "使用"));
                    _contextHintEntries.Add(new ContextHintEntry("E", "交互"));
                    _contextHintEntries.Add(new ContextHintEntry("Shift", "加速"));
                    _contextHintEntries.Add(new ContextHintEntry("Tab", "背包"));
                    _contextHintEntries.Add(new ContextHintEntry("V", "放置"));
                    _contextHintEntries.Add(new ContextHintEntry("1~5/滚轮", "切换手持"));
                    break;
            }

            contextCardImage.color = surfaceColor;
            contextAccentLineImage.color = accentColor;
            contextTagImage.color = new Color(accentColor.r, accentColor.g, accentColor.b, 0.22f);
            contextFooterText.text = "退格关闭";

            EnsureTextReadable(contextTagText);
            EnsureTextReadable(contextTitleText);
            EnsureTextReadable(contextDetailText);
            EnsureTextReadable(contextFooterText);
            contextDetailText.gameObject.SetActive(!string.IsNullOrWhiteSpace(contextDetailText.text));

            for (int index = 0; index < _contextRows.Count; index++)
            {
                ContextHintRowRefs row = _contextRows[index];
                bool shouldShow = index < _contextHintEntries.Count;
                if (row?.Root == null || row.KeyText == null || row.DescriptionText == null || row.KeyPlateImage == null || row.KeyPlateRect == null)
                {
                    continue;
                }

                row.Root.gameObject.SetActive(shouldShow);
                if (!shouldShow)
                {
                    continue;
                }

                ContextHintEntry entry = _contextHintEntries[index];
                row.KeyText.text = entry.KeyLabel;
                row.DescriptionText.text = entry.Description;
                row.KeyPlateImage.color = group == ContextHintGroup.Package
                    ? new Color(0.67f, 0.84f, 0.98f, 0.98f)
                    : new Color(0.98f, 0.81f, 0.36f, 0.98f);
                row.KeyText.color = group == ContextHintGroup.Package
                    ? new Color(0.09f, 0.16f, 0.26f, 1f)
                    : new Color(0.16f, 0.12f, 0.05f, 1f);
                EnsureTextReadable(row.KeyText);
                EnsureTextReadable(row.DescriptionText);
            }

            ApplyContextHintCardLayout();
        }

        private void ApplyContextHintCardLayout()
        {
            if (contextCardRect == null || contextRowsRoot == null || contextTitleText == null || contextDetailText == null || contextFooterText == null)
            {
                return;
            }

            float contentWidth = ContextCardWidth - ContextCardInnerLeft - ContextCardInnerRight;
            bool hasDetail = contextDetailText != null && !string.IsNullOrWhiteSpace(contextDetailText.text);
            float titleTop = 28f;
            RectTransform titleRect = contextTitleText.rectTransform;
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(0f, 1f);
            titleRect.pivot = new Vector2(0f, 1f);
            titleRect.anchoredPosition = new Vector2(18f, -titleTop);
            titleRect.sizeDelta = new Vector2(contentWidth - 4f, 16f);

            RectTransform detailRect = contextDetailText.rectTransform;
            detailRect.anchorMin = new Vector2(0f, 1f);
            detailRect.anchorMax = new Vector2(0f, 1f);
            detailRect.pivot = new Vector2(0f, 1f);
            detailRect.anchoredPosition = new Vector2(18f, -44f);
            detailRect.sizeDelta = new Vector2(contentWidth - 6f, 16f);

            float rowsTop = hasDetail ? 68f : 48f;
            float rowWidth = contentWidth;
            float keyColumnWidth = 48f;
            for (int index = 0; index < _contextRows.Count; index++)
            {
                ContextHintRowRefs row = _contextRows[index];
                if (row?.Root == null || !row.Root.gameObject.activeSelf || row.KeyText == null)
                {
                    continue;
                }

                string keyLabel = row.KeyText.text ?? string.Empty;
                keyColumnWidth = Mathf.Max(keyColumnWidth, Mathf.Clamp(16f + (keyLabel.Length * 5.8f), 48f, 72f));
            }

            int visibleRowCount = 0;
            for (int index = 0; index < _contextRows.Count; index++)
            {
                ContextHintRowRefs row = _contextRows[index];
                if (row?.Root == null || !row.Root.gameObject.activeSelf)
                {
                    continue;
                }

                float y = rowsTop + visibleRowCount * (ContextCardRowHeight + ContextCardRowGap);
                row.Root.anchoredPosition = new Vector2(ContextCardInnerLeft, -y);
                row.Root.sizeDelta = new Vector2(rowWidth, ContextCardRowHeight);

                row.KeyPlateRect.anchoredPosition = new Vector2(0f, -ContextCardRowHeight * 0.5f);
                row.KeyPlateRect.sizeDelta = new Vector2(keyColumnWidth, 16f);
                RectTransform keyTextRect = row.KeyText.rectTransform;
                keyTextRect.sizeDelta = new Vector2(Mathf.Max(keyColumnWidth - 8f, 24f), 11f);

                RectTransform descriptionRect = row.DescriptionText.rectTransform;
                descriptionRect.anchorMin = new Vector2(0f, 0.5f);
                descriptionRect.anchorMax = new Vector2(1f, 0.5f);
                descriptionRect.pivot = new Vector2(0f, 0.5f);
                descriptionRect.offsetMin = new Vector2(keyColumnWidth + 8f, -8f);
                descriptionRect.offsetMax = new Vector2(0f, 8f);

                visibleRowCount++;
            }

            float footerY = rowsTop + visibleRowCount * (ContextCardRowHeight + ContextCardRowGap) + 5f;
            RectTransform footerRect = contextFooterText.rectTransform;
            footerRect.anchorMin = new Vector2(1f, 1f);
            footerRect.anchorMax = new Vector2(1f, 1f);
            footerRect.pivot = new Vector2(1f, 1f);
            footerRect.anchoredPosition = new Vector2(-ContextCardInnerRight, -footerY);
            footerRect.sizeDelta = new Vector2(contentWidth, 10f);

            float desiredHeight = Mathf.Max(
                ContextCardMinHeight,
                footerY + 10f + ContextCardBottomInset);
            contextCardRect.sizeDelta = new Vector2(ContextCardWidth, desiredHeight);
        }

        private IEnumerator PlayStatusCardVisibility(bool keepVisible)
        {
            _statusVisible = true;
            RefreshCardStackLayout();
            RefreshOverlayVisibility();

            if (!keepVisible)
            {
                yield return FadeStatusCard(statusCardCanvasGroup != null ? statusCardCanvasGroup.alpha : 0f, 1f, StatusFadeDuration);
            }
            else
            {
                SetStatusCardAlpha(1f);
            }

            float holdRemaining = StatusHoldDuration;
            while (holdRemaining > 0f)
            {
                holdRemaining -= Time.unscaledDeltaTime;
                yield return null;
            }

            yield return FadeStatusCard(statusCardCanvasGroup != null ? statusCardCanvasGroup.alpha : 1f, 0f, StatusFadeDuration);
            _statusVisible = false;
            SetStatusCardAlpha(0f);
            RefreshCardStackLayout();
            RefreshOverlayVisibility();
            _statusVisibilityCoroutine = null;
        }

        private IEnumerator FadeStatusCard(float from, float to, float duration)
        {
            if (duration <= 0f)
            {
                SetStatusCardAlpha(to);
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                SetStatusCardAlpha(Mathf.Lerp(from, to, t));
                yield return null;
            }

            SetStatusCardAlpha(to);
        }

        private void HidePlacementStatusImmediate()
        {
            _statusVisible = false;
            StopStatusVisibilityCoroutine();
            SetStatusCardAlpha(0f);
            RefreshCardStackLayout();
            RefreshOverlayVisibility();
        }

        private void HideContextHintImmediate()
        {
            _contextVisible = false;
            _activeContextHintGroup = ContextHintGroup.None;
            _contextBoundaryAlpha = 1f;
            SetContextCardAlpha(0f);
            RefreshOverlayVisibility();
        }

        private void StopStatusVisibilityCoroutine()
        {
            if (_statusVisibilityCoroutine == null)
            {
                return;
            }

            StopCoroutine(_statusVisibilityCoroutine);
            _statusVisibilityCoroutine = null;
        }

        private void RefreshCardStackLayout()
        {
            if (cardRect != null)
            {
                cardRect.anchoredPosition = new Vector2(BaseCardX, BaseCardY);
            }

            if (statusCardRect != null)
            {
                float baseY = BaseCardY;
                if (_visible && cardRect != null)
                {
                    baseY += cardRect.sizeDelta.y + CardStackGap;
                }

                statusCardRect.anchoredPosition = new Vector2(BaseCardX, baseY);
            }
        }

        private void RefreshOverlayVisibility()
        {
            bool anyVisible = _visible || _statusVisible || _contextVisible;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = anyVisible ? 1f : 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            if (overlayCanvas != null)
            {
                if (!overlayCanvas.gameObject.activeSelf)
                {
                    overlayCanvas.gameObject.SetActive(true);
                }
            }
        }

        private void SetInteractionCardAlpha(float alpha)
        {
            if (interactionCardCanvasGroup == null)
            {
                return;
            }

            interactionCardCanvasGroup.alpha = alpha;
            interactionCardCanvasGroup.interactable = false;
            interactionCardCanvasGroup.blocksRaycasts = false;
            cardRect.gameObject.SetActive(alpha > 0.001f);
        }

        private void SetStatusCardAlpha(float alpha)
        {
            if (statusCardCanvasGroup == null)
            {
                return;
            }

            statusCardCanvasGroup.alpha = alpha;
            statusCardCanvasGroup.interactable = false;
            statusCardCanvasGroup.blocksRaycasts = false;
            statusCardRect.gameObject.SetActive(alpha > 0.001f);
        }

        private void SetContextCardAlpha(float alpha)
        {
            _contextRequestedAlpha = alpha;
            ApplyContextCardAlpha();
        }

        private void ApplyContextCardAlpha()
        {
            if (contextCardCanvasGroup == null || contextCardRect == null)
            {
                return;
            }

            float finalAlpha = _contextRequestedAlpha * _contextBoundaryAlpha;
            contextCardCanvasGroup.alpha = finalAlpha;
            bool shouldBlockRaycasts = _contextRequestedAlpha > 0.001f && finalAlpha > 0.95f;
            contextCardCanvasGroup.interactable = shouldBlockRaycasts;
            contextCardCanvasGroup.blocksRaycasts = shouldBlockRaycasts;
            contextCardRect.gameObject.SetActive(_contextRequestedAlpha > 0.001f);
        }

        private void UpdateContextBoundaryFade()
        {
            if (contextCardCanvasGroup == null || contextCardRect == null)
            {
                return;
            }

            float targetAlpha = ResolveContextBoundaryTargetAlpha();
            _contextBoundaryAlpha = Mathf.MoveTowards(
                _contextBoundaryAlpha,
                targetAlpha,
                Time.unscaledDeltaTime * ContextBoundaryFocusBlendSpeed);
            ApplyContextCardAlpha();
        }

        private float ResolveContextBoundaryTargetAlpha()
        {
            if (!_contextVisible || _contextRequestedAlpha <= 0.001f || ShouldKeepContextCardFullyVisible())
            {
                return 1f;
            }

            if (!TryResolvePlayerViewport(out Vector3 playerViewport))
            {
                return 1f;
            }

            float edgePressure = Mathf.Clamp01(ResolveUpperEdgePressure(playerViewport.x));
            if (edgePressure >= ContextBoundaryFocusHardFadePressure)
            {
                return ContextBoundaryFocusMinAlpha;
            }

            float easedPressure = 1f - Mathf.Pow(1f - edgePressure, 5f);
            return Mathf.Lerp(1f, ContextBoundaryFocusMinAlpha, easedPressure);
        }

        private bool ShouldKeepContextCardFullyVisible()
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return true;
            }

            if (_packageTabs != null && _packageTabs.IsPanelOpen())
            {
                return true;
            }

            return false;
        }

        private bool TryResolvePlayerViewport(out Vector3 viewport)
        {
            viewport = default;
            Transform playerTransform = ResolvePlayerTransform();
            Camera worldCamera = SpringDay1UiLayerUtility.GetWorldProjectionCamera(overlayCanvas);
            if (playerTransform == null || worldCamera == null)
            {
                return false;
            }

            viewport = worldCamera.WorldToViewportPoint(playerTransform.position);
            return float.IsFinite(viewport.x)
                && float.IsFinite(viewport.y)
                && float.IsFinite(viewport.z)
                && viewport.z > 0f;
        }

        private Transform ResolvePlayerTransform()
        {
            if (_playerMovement == null)
            {
                _playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            }

            return _playerMovement != null ? _playerMovement.transform : null;
        }

        private static float ResolveUpperEdgePressure(float viewportValue)
        {
            return Mathf.InverseLerp(
                1f - ContextBoundaryFocusStartViewportThreshold,
                1f - ContextBoundaryFocusFullViewportThreshold,
                viewportValue);
        }

        private bool IsContextDismissed(ContextHintGroup group)
        {
            return group switch
            {
                ContextHintGroup.Gameplay => _gameplayContextDismissed,
                ContextHintGroup.Package => _packageContextDismissed,
                _ => false
            };
        }

        private void SetContextDismissed(ContextHintGroup group, bool dismissed)
        {
            switch (group)
            {
                case ContextHintGroup.Gameplay:
                    _gameplayContextDismissed = dismissed;
                    break;
                case ContextHintGroup.Package:
                    _packageContextDismissed = dismissed;
                    break;
            }
        }

        private TMP_FontAsset ResolveFontAsset()
        {
            return DialogueChineseFontRuntimeBootstrap.ResolveBestFontForText(
                FontCoverageProbeText,
                _fontAsset,
                FontCoverageProbeText);
        }

        private static string NormalizeCaptionCopy(string caption)
        {
            return string.IsNullOrWhiteSpace(caption) ? "可交互" : caption.Trim();
        }

        private static string NormalizeDetailCopy(string detail, string caption, string keyLabel)
        {
            string normalizedDetail = string.IsNullOrWhiteSpace(detail) ? string.Empty : detail.Trim();
            if (!string.IsNullOrWhiteSpace(normalizedDetail))
            {
                return normalizedDetail;
            }

            if (string.IsNullOrWhiteSpace(caption))
            {
                return string.IsNullOrWhiteSpace(keyLabel) ? string.Empty : "按键可用，等待交互说明恢复。";
            }

            if (caption.Contains("工作台"))
            {
                return "打开工作台，查看配方、材料与制作进度。";
            }

            if (caption.Contains("任务"))
            {
                return "优先进入当前任务相关交互。";
            }

            if (caption.Contains("对话") || caption.Contains("交谈"))
            {
                return "进入当前对话。";
            }

            return string.IsNullOrWhiteSpace(keyLabel) ? string.Empty : "按键可用，等待交互说明恢复。";
        }

        private void EnsureTextReadable(TextMeshProUGUI text)
        {
            if (text == null)
            {
                return;
            }

            if (!text.gameObject.activeSelf)
            {
                text.gameObject.SetActive(true);
            }

            if (!text.enabled)
            {
                text.enabled = true;
            }

            if (!CanFontRenderText(text.font, text.text))
            {
                if (_fontAsset == null)
                {
                    _fontAsset = ResolveFontAsset();
                }

                TMP_FontAsset resolvedFont = DialogueChineseFontRuntimeBootstrap.ResolveBestFontForText(
                    text.text,
                    _fontAsset,
                    FontCoverageProbeText);
                if (resolvedFont != null)
                {
                    _fontAsset = resolvedFont;
                    text.font = resolvedFont;
                    if (resolvedFont.material != null)
                    {
                        text.fontSharedMaterial = resolvedFont.material;
                    }
                }
            }

            if (text.color.a < 0.98f)
            {
                Color color = text.color;
                color.a = 1f;
                text.color = color;
            }

            text.ForceMeshUpdate();
        }

        private static bool CanFontRenderText(TMP_FontAsset fontAsset, string currentText)
        {
            return DialogueChineseFontRuntimeBootstrap.CanRenderText(
                fontAsset,
                currentText,
                FontCoverageProbeText);
        }

        private static string GetFontProbeText(string currentText)
        {
            if (string.IsNullOrWhiteSpace(currentText))
            {
                return FontCoverageProbeText;
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

                if (insideTag)
                {
                    if (current == '>')
                    {
                        insideTag = false;
                    }

                    continue;
                }

                if (!char.IsControl(current))
                {
                    builder.Append(current);
                }
            }

            return builder.Length > 0 ? builder.ToString() : FontCoverageProbeText;
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

        private static T GetOrAddComponent<T>(GameObject target) where T : Component
        {
            if (target == null)
            {
                return null;
            }

            T component = target.GetComponent<T>();
            if (component == null)
            {
                component = target.AddComponent<T>();
            }

            return component;
        }

        private static RectTransform ResolveUniqueRect(Transform searchRoot, string name)
        {
            Transform child = ResolveUniqueChild(searchRoot, name);
            return child as RectTransform;
        }

        private static Image ResolveUniqueImage(Transform searchRoot, string name)
        {
            Transform child = ResolveUniqueChild(searchRoot, name);
            return child != null ? GetOrAddComponent<Image>(child.gameObject) : null;
        }

        private static TextMeshProUGUI ResolveUniqueText(Transform searchRoot, string name)
        {
            Transform child = ResolveUniqueChild(searchRoot, name);
            return child != null ? GetOrAddComponent<TextMeshProUGUI>(child.gameObject) : null;
        }

        private static Transform ResolveUniqueChild(Transform searchRoot, string name)
        {
            if (searchRoot == null || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            Transform[] descendants = searchRoot.GetComponentsInChildren<Transform>(true);
            Transform keep = null;
            for (int index = 0; index < descendants.Length; index++)
            {
                Transform candidate = descendants[index];
                if (candidate == null
                    || candidate == searchRoot
                    || !string.Equals(candidate.name, name, System.StringComparison.Ordinal))
                {
                    continue;
                }

                if (keep == null)
                {
                    keep = candidate;
                    continue;
                }

                Destroy(candidate.gameObject);
            }

            return keep;
        }

        private TextMeshProUGUI CreateText(Transform parent, string name, string text, float fontSize, Color color, TextAlignmentOptions alignment)
        {
            RectTransform rect = CreateRect(parent, name);
            TextMeshProUGUI textComponent = rect.gameObject.AddComponent<TextMeshProUGUI>();
            textComponent.font = _fontAsset;
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.color = color;
            textComponent.alignment = alignment;
            textComponent.textWrappingMode = TextWrappingModes.NoWrap;
            textComponent.overflowMode = TextOverflowModes.Ellipsis;
            textComponent.raycastTarget = false;
            return textComponent;
        }

        private static RectTransform CreateRect(Transform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private static Sprite GetOrCreateBackplateSprite()
        {
            if (s_backplateSprite != null)
            {
                return s_backplateSprite;
            }

            const int width = 24;
            const int height = 16;
            const int radius = 4;

            s_backplateTexture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false)
            {
                name = "InteractionHintOverlayRuntimeTexture",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool inside = IsInsideRoundedRect(x, y, width, height, radius);
                    s_backplateTexture.SetPixel(x, y, inside ? Color.white : Color.clear);
                }
            }

            s_backplateTexture.Apply(updateMipmaps: false, makeNoLongerReadable: true);
            s_backplateSprite = Sprite.Create(
                s_backplateTexture,
                new Rect(0f, 0f, width, height),
                new Vector2(0.5f, 0.5f),
                24f,
                0u,
                SpriteMeshType.FullRect,
                new Vector4(radius, radius, radius, radius));
            s_backplateSprite.name = "InteractionHintOverlayRuntimeSprite";
            s_backplateSprite.hideFlags = HideFlags.HideAndDontSave;
            return s_backplateSprite;
        }

        private static bool IsInsideRoundedRect(int x, int y, int width, int height, int radius)
        {
            int left = radius;
            int right = width - radius - 1;
            int bottom = radius;
            int top = height - radius - 1;

            if (x >= left && x <= right)
            {
                return true;
            }

            if (y >= bottom && y <= top)
            {
                return true;
            }

            Vector2 point = new Vector2(x, y);
            Vector2[] centers =
            {
                new Vector2(left, bottom),
                new Vector2(left, top),
                new Vector2(right, bottom),
                new Vector2(right, top)
            };

            float radiusSqr = radius * radius;
            for (int index = 0; index < centers.Length; index++)
            {
                if ((point - centers[index]).sqrMagnitude <= radiusSqr)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
