using TMPro;
using UnityEngine;
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

        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese SDF",
            "Fonts & Materials/DialogueChinese SoftPixel SDF"
        };

        private static InteractionHintOverlay s_instance;
        private static Sprite s_backplateSprite;
        private static Texture2D s_backplateTexture;

        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;
        [SerializeField] private RectTransform cardRect;
        [SerializeField] private RectTransform keyPlateRect;
        [SerializeField] private Image cardImage;
        [SerializeField] private Image keyPlateImage;
        [SerializeField] private Image accentLineImage;
        [SerializeField] private TextMeshProUGUI keyText;
        [SerializeField] private TextMeshProUGUI captionText;
        [SerializeField] private TextMeshProUGUI detailText;

        private TMP_FontAsset _fontAsset;
        private bool _visible;

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

        public static void EnsureRuntime()
        {
            if (s_instance != null)
            {
                return;
            }

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
            s_instance.BuildUi();
            s_instance.HideImmediate();
        }

        public static void HideIfExists()
        {
            if (s_instance == null)
            {
                return;
            }

            s_instance.Hide();
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
        }

        private void OnDestroy()
        {
            if (s_instance == this)
            {
                s_instance = null;
            }
        }

        public void ShowPrompt(string keyLabel, string caption, string detail = "")
        {
            EnsureBuilt();
            if (!InteractionHintDisplaySettings.AreHintsVisible)
            {
                HideImmediate();
                return;
            }

            keyText.text = string.IsNullOrWhiteSpace(keyLabel) ? string.Empty : keyLabel.Trim();
            captionText.text = string.IsNullOrWhiteSpace(caption) ? "可交互" : caption.Trim();
            detailText.text = detail ?? string.Empty;
            ApplyContentLayout();

            _visible = true;
            overlayCanvas.gameObject.SetActive(true);
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void Hide()
        {
            _visible = false;
            HideImmediate();
        }

        private void HideImmediate()
        {
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

        private void HandleHintVisibilityChanged(bool visible)
        {
            if (!visible)
            {
                HideImmediate();
                return;
            }

            if (_visible)
            {
                overlayCanvas.gameObject.SetActive(true);
                canvasGroup.alpha = 1f;
            }
        }

        private void BuildUi()
        {
            rootRect = transform as RectTransform;
            overlayCanvas = GetComponent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
            _fontAsset = ResolveFontAsset();

            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.overrideSorting = true;
            overlayCanvas.sortingOrder = 164;
            overlayCanvas.pixelPerfect = true;

            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            cardRect = CreateRect(transform, "HintCard");
            cardRect.anchorMin = new Vector2(0f, 0f);
            cardRect.anchorMax = new Vector2(0f, 0f);
            cardRect.pivot = new Vector2(0f, 0f);
            cardRect.anchoredPosition = new Vector2(22f, 18f);
            cardRect.sizeDelta = new Vector2(DetailCardWidth, DetailCardHeight);

            cardImage = cardRect.gameObject.AddComponent<Image>();
            cardImage.sprite = GetOrCreateBackplateSprite();
            cardImage.type = Image.Type.Sliced;
            cardImage.color = new Color(0.08f, 0.11f, 0.16f, 0.95f);
            cardImage.raycastTarget = false;
            Outline outline = cardRect.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 0.82f, 0.45f, 0.16f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
            outline.useGraphicAlpha = true;
            Shadow shadow = cardRect.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.24f);
            shadow.effectDistance = new Vector2(0f, -4f);
            shadow.useGraphicAlpha = true;

            keyPlateRect = CreateRect(cardRect, "KeyPlate");
            keyPlateRect.anchorMin = new Vector2(0f, 0.5f);
            keyPlateRect.anchorMax = new Vector2(0f, 0.5f);
            keyPlateRect.pivot = new Vector2(0f, 0.5f);
            keyPlateRect.anchoredPosition = new Vector2(12f, 0f);
            keyPlateRect.sizeDelta = new Vector2(38f, 38f);
            keyPlateImage = keyPlateRect.gameObject.AddComponent<Image>();
            keyPlateImage.sprite = GetOrCreateBackplateSprite();
            keyPlateImage.type = Image.Type.Sliced;
            keyPlateImage.color = new Color(0.98f, 0.82f, 0.36f, 0.98f);
            keyPlateImage.raycastTarget = false;

            keyText = CreateText(keyPlateRect, "KeyText", "E", 19f, new Color(0.22f, 0.14f, 0.05f, 1f), TextAlignmentOptions.Center);
            keyText.fontStyle = FontStyles.Bold;
            RectTransform keyTextRect = keyText.rectTransform;
            keyTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            keyTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            keyTextRect.pivot = new Vector2(0.5f, 0.5f);
            keyTextRect.anchoredPosition = Vector2.zero;
            keyTextRect.sizeDelta = new Vector2(26f, 24f);

            accentLineImage = CreateRect(cardRect, "AccentLine").gameObject.AddComponent<Image>();
            accentLineImage.color = new Color(0.98f, 0.70f, 0.31f, 0.92f);
            accentLineImage.raycastTarget = false;
            RectTransform accentRect = accentLineImage.rectTransform;
            accentRect.anchorMin = new Vector2(0f, 0.5f);
            accentRect.anchorMax = new Vector2(0f, 0.5f);
            accentRect.pivot = new Vector2(0f, 0.5f);
            accentRect.anchoredPosition = new Vector2(58f, 0f);
            accentRect.sizeDelta = new Vector2(4f, 38f);

            captionText = CreateText(cardRect, "CaptionText", "可交互", 15f, new Color(0.98f, 0.96f, 0.91f, 1f), TextAlignmentOptions.Left);
            captionText.fontStyle = FontStyles.Bold;
            captionText.textWrappingMode = TextWrappingModes.Normal;
            captionText.overflowMode = TextOverflowModes.Overflow;
            RectTransform captionRect = captionText.rectTransform;
            captionRect.anchorMin = new Vector2(0f, 1f);
            captionRect.anchorMax = new Vector2(1f, 1f);
            captionRect.pivot = new Vector2(0f, 1f);
            captionRect.offsetMin = new Vector2(70f, -30f);
            captionRect.offsetMax = new Vector2(-12f, -10f);

            detailText = CreateText(cardRect, "DetailText", string.Empty, 11.5f, new Color(0.80f, 0.88f, 0.96f, 0.96f), TextAlignmentOptions.Left);
            detailText.textWrappingMode = TextWrappingModes.Normal;
            detailText.overflowMode = TextOverflowModes.Overflow;
            RectTransform detailRect = detailText.rectTransform;
            detailRect.anchorMin = new Vector2(0f, 0f);
            detailRect.anchorMax = new Vector2(1f, 0f);
            detailRect.pivot = new Vector2(0f, 0f);
            detailRect.offsetMin = new Vector2(70f, 12f);
            detailRect.offsetMax = new Vector2(-12f, 30f);
            detailText.gameObject.SetActive(false);
        }

        private void EnsureBuilt()
        {
            if (rootRect != null
                && cardRect != null
                && keyPlateRect != null
                && keyText != null
                && captionText != null
                && detailText != null)
            {
                return;
            }

            BuildUi();
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
        }

        private TMP_FontAsset ResolveFontAsset()
        {
            for (int index = 0; index < PreferredFontResourcePaths.Length; index++)
            {
                TMP_FontAsset candidate = Resources.Load<TMP_FontAsset>(PreferredFontResourcePaths[index]);
                if (IsFontAssetUsable(candidate))
                {
                    return candidate;
                }
            }

            TMP_FontAsset defaultFont = TMP_Settings.defaultFontAsset;
            return IsFontAssetUsable(defaultFont) ? defaultFont : null;
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
