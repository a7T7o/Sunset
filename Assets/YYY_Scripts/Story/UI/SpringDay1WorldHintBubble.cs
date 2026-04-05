using Sunset.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class SpringDay1WorldHintBubble : MonoBehaviour
    {
        public enum HintVisualKind
        {
            Interaction,
            Tutorial
        }

        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese SoftPixel SDF",
            "Fonts & Materials/DialogueChinese SDF"
        };

        private const string FontCoverageProbeText = "工作台再靠近一些按E打开";

        private const float ActionableWidth = 194f;
        private const float PassiveWidth = 166f;
        private const float MinActionableHeight = 72f;
        private const float MinPassiveHeight = 54f;

        private static SpringDay1WorldHintBubble _instance;
        private static Sprite _backplateSprite;
        private static Texture2D _backplateTexture;
        private static Sprite _indicatorSprite;
        private static Texture2D _indicatorTexture;

        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;
        [SerializeField] private RectTransform bubbleRect;
        [SerializeField] private RectTransform arrowRect;
        [SerializeField] private RectTransform keyPlateRect;
        [SerializeField] private Image bubbleBackground;
        [SerializeField] private Image arrowImage;
        [SerializeField] private Image keyPlateImage;
        [SerializeField] private Image accentLineImage;
        [SerializeField] private TextMeshProUGUI keyText;
        [SerializeField] private TextMeshProUGUI captionText;
        [SerializeField] private TextMeshProUGUI detailText;

        private TMP_FontAsset _fontAsset;
        private Transform _anchorTarget;
        private bool _visible;
        private bool _suppressWhileDialogueActive;
        private HintVisualKind _visualKind;
        private bool _isActionable;

        public Transform CurrentAnchorTarget => _anchorTarget;
        public bool IsVisible => _visible && canvasGroup != null && canvasGroup.alpha > 0.001f;
        public string CurrentKeyLabel => keyText != null ? keyText.text : string.Empty;
        public string CurrentCaptionText => captionText != null ? captionText.text : string.Empty;
        public string CurrentDetailText => detailText != null ? detailText.text : string.Empty;
        public bool CurrentIsActionable => _isActionable;

        public static SpringDay1WorldHintBubble Instance
        {
            get
            {
                if (_instance == null)
                {
                    EnsureRuntime();
                }

                return _instance;
            }
        }

        public static void EnsureRuntime()
        {
            if (_instance != null)
            {
                return;
            }

            Transform parent = ResolveParent();
            GameObject root = new GameObject(
                nameof(SpringDay1WorldHintBubble),
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasGroup));

            if (parent != null)
            {
                root.transform.SetParent(parent, false);
            }

            _instance = root.AddComponent<SpringDay1WorldHintBubble>();
            _instance.BuildUi();
            _instance.Hide();
        }

        public static void HideIfExists(Transform anchorTarget = null)
        {
            if (_instance == null)
            {
                return;
            }

            if (anchorTarget == null || _instance._anchorTarget == anchorTarget)
            {
                _instance.Hide();
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<DialogueStartEvent>(OnDialogueStart, owner: this);
            EventBus.Subscribe<DialogueEndEvent>(OnDialogueEnd, owner: this);
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeAll(this);
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void LateUpdate()
        {
            if (!_visible || _anchorTarget == null || _suppressWhileDialogueActive)
            {
                return;
            }

            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen())
            {
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                }

                return;
            }

            if (canvasGroup != null && canvasGroup.alpha < 0.999f)
            {
                canvasGroup.alpha = 1f;
            }

            Reposition();
        }

        public void Show(
            Transform anchorTarget,
            string keyLabel = "E",
            string caption = "",
            string detail = "",
            HintVisualKind visualKind = HintVisualKind.Interaction)
        {
            if (anchorTarget == null)
            {
                Hide();
                return;
            }

            EnsureBuilt();
            _anchorTarget = anchorTarget;
            _visible = true;
            _visualKind = visualKind;
            _isActionable = !string.IsNullOrWhiteSpace(keyLabel);

            keyText.text = string.IsNullOrWhiteSpace(keyLabel) ? string.Empty : keyLabel.Trim();
            captionText.text = string.IsNullOrWhiteSpace(caption) ? "交互" : caption.Trim();
            detailText.text = detail ?? string.Empty;
            EnsureTextReadable(keyText);
            EnsureTextReadable(captionText);
            EnsureTextReadable(detailText);

            ApplyContentLayout();
            ApplyVisualStyle();

            if (overlayCanvas != null)
            {
                overlayCanvas.gameObject.SetActive(true);
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            Reposition();
        }

        public void Hide()
        {
            _visible = false;
            _anchorTarget = null;
            _isActionable = false;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            if (overlayCanvas != null)
            {
                overlayCanvas.gameObject.SetActive(false);
            }
        }

        public void Hide(Transform anchorTarget)
        {
            if (anchorTarget != null && _anchorTarget != anchorTarget)
            {
                return;
            }

            Hide();
        }

        private void BuildUi()
        {
            rootRect = transform as RectTransform;
            overlayCanvas = GetComponent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
            _fontAsset = ResolveFont();

            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.overrideSorting = true;
            overlayCanvas.sortingOrder = 154;
            overlayCanvas.pixelPerfect = true;

            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            bubbleRect = CreateRect(transform, "Backplate");
            bubbleRect.anchorMin = new Vector2(0.5f, 0.5f);
            bubbleRect.anchorMax = new Vector2(0.5f, 0.5f);
            bubbleRect.pivot = new Vector2(0.5f, 0f);
            bubbleRect.sizeDelta = Vector2.one;
            bubbleBackground = bubbleRect.gameObject.AddComponent<Image>();
            bubbleBackground.sprite = GetOrCreateBackplateSprite();
            bubbleBackground.type = Image.Type.Sliced;
            bubbleBackground.raycastTarget = false;
            bubbleBackground.enabled = false;

            arrowRect = CreateRect(transform, "Indicator");
            arrowRect.anchorMin = new Vector2(0.5f, 0.5f);
            arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
            arrowRect.pivot = new Vector2(0.5f, 1f);
            arrowRect.sizeDelta = new Vector2(10f, 7f);
            arrowImage = arrowRect.gameObject.AddComponent<Image>();
            arrowImage.sprite = GetOrCreateIndicatorSprite();
            arrowImage.type = Image.Type.Simple;
            arrowImage.raycastTarget = false;
            ApplyShadow(arrowRect.gameObject.AddComponent<Shadow>(), new Color(0f, 0f, 0f, 0.18f), new Vector2(0f, -1f));

            keyPlateRect = CreateRect(bubbleRect, "KeyPlate");
            keyPlateRect.anchorMin = new Vector2(0f, 0.5f);
            keyPlateRect.anchorMax = new Vector2(0f, 0.5f);
            keyPlateRect.pivot = new Vector2(0f, 0.5f);
            keyPlateRect.anchoredPosition = new Vector2(10f, 0f);
            keyPlateRect.sizeDelta = new Vector2(34f, 34f);
            keyPlateImage = keyPlateRect.gameObject.AddComponent<Image>();
            keyPlateImage.sprite = GetOrCreateBackplateSprite();
            keyPlateImage.type = Image.Type.Sliced;
            keyPlateImage.raycastTarget = false;

            keyText = CreateText(keyPlateRect, "KeyText", "E", 17f);
            keyText.fontStyle = FontStyles.Bold;
            keyText.alignment = TextAlignmentOptions.Center;
            keyText.color = new Color(0.24f, 0.16f, 0.06f, 1f);
            RectTransform keyTextRect = keyText.rectTransform;
            keyTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            keyTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            keyTextRect.pivot = new Vector2(0.5f, 0.5f);
            keyTextRect.anchoredPosition = Vector2.zero;
            keyTextRect.sizeDelta = new Vector2(26f, 22f);

            accentLineImage = CreateRect(bubbleRect, "AccentLine").gameObject.AddComponent<Image>();
            accentLineImage.raycastTarget = false;
            RectTransform accentRect = accentLineImage.rectTransform;
            accentRect.anchorMin = new Vector2(0f, 0.5f);
            accentRect.anchorMax = new Vector2(0f, 0.5f);
            accentRect.pivot = new Vector2(0f, 0.5f);
            accentRect.anchoredPosition = new Vector2(52f, 0f);
            accentRect.sizeDelta = new Vector2(3f, 32f);

            captionText = CreateText(bubbleRect, "CaptionText", "交互", 14f);
            captionText.fontStyle = FontStyles.Bold;
            captionText.alignment = TextAlignmentOptions.Left;
            captionText.color = new Color(0.98f, 0.96f, 0.91f, 1f);
            RectTransform captionRect = captionText.rectTransform;
            captionRect.anchorMin = new Vector2(0f, 1f);
            captionRect.anchorMax = new Vector2(1f, 1f);
            captionRect.pivot = new Vector2(0f, 1f);
            captionRect.offsetMin = new Vector2(62f, -26f);
            captionRect.offsetMax = new Vector2(-12f, -8f);

            detailText = CreateText(bubbleRect, "DetailText", string.Empty, 10.5f);
            detailText.alignment = TextAlignmentOptions.Left;
            detailText.color = new Color(0.84f, 0.90f, 0.97f, 0.98f);
            detailText.textWrappingMode = TextWrappingModes.Normal;
            detailText.overflowMode = TextOverflowModes.Overflow;
            RectTransform detailRect = detailText.rectTransform;
            detailRect.anchorMin = new Vector2(0f, 0f);
            detailRect.anchorMax = new Vector2(1f, 0f);
            detailRect.pivot = new Vector2(0f, 0f);
            detailRect.offsetMin = new Vector2(62f, 10f);
            detailRect.offsetMax = new Vector2(-12f, 24f);
            detailText.gameObject.SetActive(false);
        }

        private void EnsureBuilt()
        {
            if (bubbleRect == null
                || arrowRect == null
                || keyPlateRect == null
                || bubbleBackground == null
                || arrowImage == null
                || keyPlateImage == null
                || accentLineImage == null
                || keyText == null
                || captionText == null
                || detailText == null)
            {
                BuildUi();
            }
        }

        private void Reposition()
        {
            Camera camera = SpringDay1UiLayerUtility.GetWorldProjectionCamera(overlayCanvas);
            if (camera == null)
            {
                return;
            }

            Bounds bounds = ResolveBounds();
            Vector3 worldAnchor = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldAnchor);
            screenPoint.y += 9f + Mathf.Sin(Time.unscaledTime * 2f) * 0.35f;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rootRect,
                    screenPoint,
                    SpringDay1UiLayerUtility.GetUiEventCamera(overlayCanvas),
                    out Vector2 localPoint))
            {
                return;
            }

            localPoint = SpringDay1UiLayerUtility.SnapToCanvasPixel(overlayCanvas, localPoint);
            bubbleRect.anchoredPosition = localPoint;
            arrowRect.anchoredPosition = localPoint + new Vector2(0f, -6f);
        }

        private Bounds ResolveBounds()
        {
            if (SpringDay1UiLayerUtility.TryGetPresentationBounds(_anchorTarget, out Bounds bounds))
            {
                return bounds;
            }

            return new Bounds(_anchorTarget != null ? _anchorTarget.position : Vector3.zero, Vector3.one);
        }

        private TMP_FontAsset ResolveFont()
        {
            for (int index = 0; index < PreferredFontResourcePaths.Length; index++)
            {
                TMP_FontAsset candidate = Resources.Load<TMP_FontAsset>(PreferredFontResourcePaths[index]);
                if (CanFontRenderText(candidate, FontCoverageProbeText))
                {
                    return candidate;
                }
            }

            TMP_FontAsset defaultFont = TMP_Settings.defaultFontAsset;
            return CanFontRenderText(defaultFont, FontCoverageProbeText) ? defaultFont : null;
        }

        private void EnsureTextReadable(TextMeshProUGUI text)
        {
            if (text == null)
            {
                return;
            }

            if (!text.enabled)
            {
                text.enabled = true;
            }

            if (!CanFontRenderText(text.font, text.text))
            {
                if (_fontAsset == null)
                {
                    _fontAsset = ResolveFont();
                }

                if (_fontAsset != null)
                {
                    text.font = _fontAsset;
                    if (_fontAsset.material != null)
                    {
                        text.fontSharedMaterial = _fontAsset.material;
                    }
                }
            }

            text.ForceMeshUpdate();
        }

        private static bool CanFontRenderText(TMP_FontAsset fontAsset, string currentText)
        {
            if (!IsFontAssetUsable(fontAsset))
            {
                return false;
            }

            string probeText = GetFontProbeText(currentText);
            return string.IsNullOrEmpty(probeText) || fontAsset.HasCharacters(probeText);
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

        private void OnDialogueStart(DialogueStartEvent _)
        {
            _suppressWhileDialogueActive = true;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }

        private void OnDialogueEnd(DialogueEndEvent _)
        {
            if (ShouldIgnoreDialogueEndEvent())
            {
                return;
            }

            _suppressWhileDialogueActive = false;
            if (_visible && canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }

        private static bool ShouldIgnoreDialogueEndEvent()
        {
            DialogueManager manager = DialogueManager.Instance;
            return manager != null && manager.IsDialogueActive;
        }

        private void ApplyVisualStyle()
        {
            bool showKeyPlate = keyPlateRect != null && keyPlateRect.gameObject.activeSelf;
            Color backplateColor = _visualKind == HintVisualKind.Tutorial
                ? (_isActionable ? new Color(0.17f, 0.13f, 0.06f, 0.96f) : new Color(0.23f, 0.18f, 0.08f, 0.86f))
                : (_isActionable ? new Color(0.09f, 0.13f, 0.18f, 0.95f) : new Color(0.11f, 0.14f, 0.18f, 0.84f));
            Color arrowColor = _visualKind == HintVisualKind.Tutorial
                ? (_isActionable ? new Color(0.98f, 0.79f, 0.30f, 0.98f) : new Color(0.94f, 0.84f, 0.56f, 0.86f))
                : (_isActionable ? new Color(0.96f, 0.92f, 0.82f, 0.96f) : new Color(0.84f, 0.89f, 0.95f, 0.84f));
            Color keyPlateColor = _visualKind == HintVisualKind.Tutorial
                ? new Color(0.98f, 0.82f, 0.34f, 0.99f)
                : new Color(0.99f, 0.84f, 0.40f, 0.98f);
            Color accentColor = _visualKind == HintVisualKind.Tutorial
                ? (_isActionable ? new Color(0.99f, 0.76f, 0.26f, 0.96f) : new Color(0.90f, 0.71f, 0.38f, 0.72f))
                : (_isActionable ? new Color(0.99f, 0.68f, 0.28f, 0.92f) : new Color(0.70f, 0.82f, 0.95f, 0.62f));

            if (bubbleBackground != null)
            {
                bubbleBackground.enabled = true;
                bubbleBackground.color = backplateColor;
            }

            if (arrowImage != null)
            {
                arrowImage.color = arrowColor;
            }

            if (keyPlateImage != null)
            {
                keyPlateImage.enabled = showKeyPlate;
                keyPlateImage.color = keyPlateColor;
            }

            if (accentLineImage != null)
            {
                accentLineImage.enabled = true;
                accentLineImage.color = accentColor;
            }
        }

        private void ApplyContentLayout()
        {
            bool hasKeyLabel = !string.IsNullOrWhiteSpace(keyText != null ? keyText.text : string.Empty);
            bool hasDetail = !string.IsNullOrWhiteSpace(detailText != null ? detailText.text : string.Empty);
            float cardWidth = hasKeyLabel ? ActionableWidth : PassiveWidth;
            float leftInset = hasKeyLabel ? 62f : 18f;
            float rightInset = 12f;
            float usableWidth = Mathf.Max(64f, cardWidth - leftInset - rightInset);
            float captionHeight = MeasureTextHeight(captionText, usableWidth, 18f);
            float detailHeight = hasDetail ? MeasureTextHeight(detailText, usableWidth, 12f) : 0f;
            float cardHeight = hasKeyLabel
                ? Mathf.Max(MinActionableHeight, 16f + captionHeight + (hasDetail ? detailHeight + 10f : 0f))
                : Mathf.Max(MinPassiveHeight, 14f + captionHeight + (hasDetail ? detailHeight + 8f : 0f));

            if (bubbleRect != null)
            {
                bubbleRect.gameObject.SetActive(true);
                bubbleRect.sizeDelta = new Vector2(cardWidth, cardHeight);
            }

            if (captionText != null)
            {
                captionText.gameObject.SetActive(true);
                RectTransform captionRect = captionText.rectTransform;
                captionRect.offsetMin = new Vector2(leftInset, -(hasDetail ? 28f : 20f));
                captionRect.offsetMax = new Vector2(-rightInset, -8f);
            }

            if (detailText != null)
            {
                detailText.gameObject.SetActive(hasDetail);
                RectTransform detailRect = detailText.rectTransform;
                detailRect.offsetMin = new Vector2(leftInset, 10f);
                detailRect.offsetMax = new Vector2(-rightInset, hasKeyLabel ? 30f : 24f);
            }

            if (keyPlateRect != null)
            {
                keyPlateRect.gameObject.SetActive(hasKeyLabel);
                keyPlateRect.anchoredPosition = new Vector2(10f, 0f);
                keyPlateRect.sizeDelta = new Vector2(34f, 34f);
            }

            if (accentLineImage != null)
            {
                accentLineImage.gameObject.SetActive(true);
                RectTransform accentRect = accentLineImage.rectTransform;
                accentRect.anchoredPosition = new Vector2(hasKeyLabel ? 52f : 12f, 0f);
                accentRect.sizeDelta = new Vector2(hasKeyLabel ? 3f : 2f, Mathf.Max(26f, cardHeight - 20f));
            }
        }

        private float MeasureTextHeight(TextMeshProUGUI text, float width, float minHeight)
        {
            if (text == null)
            {
                return minHeight;
            }

            Vector2 preferred = text.GetPreferredValues(text.text, Mathf.Max(1f, width), 0f);
            return Mathf.Max(minHeight, Mathf.Ceil(preferred.y));
        }

        private static RectTransform CreateRect(Transform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private TextMeshProUGUI CreateText(Transform parent, string name, string text, float fontSize)
        {
            RectTransform rect = CreateRect(parent, name);
            TextMeshProUGUI textComponent = rect.gameObject.AddComponent<TextMeshProUGUI>();
            textComponent.font = _fontAsset;
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.textWrappingMode = TextWrappingModes.NoWrap;
            textComponent.overflowMode = TextOverflowModes.Ellipsis;
            textComponent.raycastTarget = false;
            return textComponent;
        }

        private static Transform ResolveParent()
        {
            return SpringDay1UiLayerUtility.ResolveUiParent();
        }

        private static void ApplyShadow(Shadow shadow, Color color, Vector2 distance)
        {
            shadow.effectColor = color;
            shadow.effectDistance = distance;
            shadow.useGraphicAlpha = true;
        }

        private static Sprite GetOrCreateBackplateSprite()
        {
            if (_backplateSprite != null)
            {
                return _backplateSprite;
            }

            const int width = 24;
            const int height = 16;
            const int radius = 4;

            _backplateTexture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false)
            {
                name = "SpringDay1HintBackplateRuntimeTexture",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool inside = IsInsideRoundedRect(x, y, width, height, radius);
                    _backplateTexture.SetPixel(x, y, inside ? Color.white : Color.clear);
                }
            }

            _backplateTexture.Apply(updateMipmaps: false, makeNoLongerReadable: true);
            _backplateSprite = Sprite.Create(
                _backplateTexture,
                new Rect(0f, 0f, width, height),
                new Vector2(0.5f, 0.5f),
                24f,
                0u,
                SpriteMeshType.FullRect,
                new Vector4(radius, radius, radius, radius));
            _backplateSprite.name = "SpringDay1HintBackplateRuntimeSprite";
            _backplateSprite.hideFlags = HideFlags.HideAndDontSave;
            return _backplateSprite;
        }

        private static Sprite GetOrCreateIndicatorSprite()
        {
            if (_indicatorSprite != null)
            {
                return _indicatorSprite;
            }

            const int width = 20;
            const int height = 14;

            _indicatorTexture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false)
            {
                name = "SpringDay1HintIndicatorRuntimeTexture",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };

            float centerX = (width - 1) * 0.5f;
            for (int y = 0; y < height; y++)
            {
                float normalized = 1f - (y / Mathf.Max(1f, height - 1f));
                float halfWidth = Mathf.Lerp(0f, centerX, normalized);
                for (int x = 0; x < width; x++)
                {
                    bool inside = Mathf.Abs(x - centerX) <= halfWidth;
                    _indicatorTexture.SetPixel(x, y, inside ? Color.white : Color.clear);
                }
            }

            _indicatorTexture.Apply(updateMipmaps: false, makeNoLongerReadable: true);
            _indicatorSprite = Sprite.Create(
                _indicatorTexture,
                new Rect(0f, 0f, width, height),
                new Vector2(0.5f, 0.5f),
                24f);
            _indicatorSprite.name = "SpringDay1HintIndicatorRuntimeSprite";
            _indicatorSprite.hideFlags = HideFlags.HideAndDontSave;
            return _indicatorSprite;
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
