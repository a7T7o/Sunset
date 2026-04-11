using Sunset.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class SpringDay1StatusOverlay : MonoBehaviour
    {
        public enum Channel
        {
            Health,
            Energy
        }

        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese V2 SDF",
            "Fonts & Materials/DialogueChinese BitmapSong SDF",
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese SoftPixel SDF",
            "Fonts & Materials/DialogueChinese SDF"
        };

        private static readonly Color HealthFrameColor = new(0.86f, 0.47f, 0.42f, 0.96f);
        private static readonly Color HealthFillColor = new(0.92f, 0.57f, 0.52f, 0.98f);
        private static readonly Color EnergyFrameColor = new(0.34f, 0.63f, 0.9f, 0.96f);
        private static readonly Color EnergyFillColor = new(0.46f, 0.79f, 0.96f, 0.98f);

        private static SpringDay1StatusOverlay _instance;

        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private CanvasScaler overlayScaler;
        [SerializeField] private CanvasGroup rootCanvasGroup;
        [SerializeField] private RectTransform rootRect;
        [SerializeField] private SectionRefs healthSection;
        [SerializeField] private SectionRefs energySection;

        private TMP_FontAsset _fontAsset;
        private bool _suppressWhileDialogueActive;

        public static SpringDay1StatusOverlay Instance
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
                nameof(SpringDay1StatusOverlay),
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasGroup));

            if (parent != null)
            {
                root.transform.SetParent(parent, false);
            }

            _instance = root.AddComponent<SpringDay1StatusOverlay>();
            _instance.BuildUi();
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

        public bool IsSectionVisible(Channel channel)
        {
            return GetSection(channel).visible;
        }

        public void SetSectionVisible(Channel channel, bool visible)
        {
            EnsureBuilt();
            SectionRefs section = GetSection(channel);
            section.visible = visible;
            ApplySectionState(section);
        }

        public void SetSectionAlpha(Channel channel, float alpha)
        {
            EnsureBuilt();
            SectionRefs section = GetSection(channel);
            section.alpha = Mathf.Clamp01(alpha);
            ApplySectionState(section);
        }

        public void SetFillColor(Channel channel, Color color)
        {
            EnsureBuilt();
            SectionRefs section = GetSection(channel);
            if (section.fillImage != null)
            {
                section.fillImage.color = color;
            }
        }

        public void SetValues(Channel channel, int current, int max)
        {
            EnsureBuilt();
            SectionRefs section = GetSection(channel);
            int clampedMax = Mathf.Max(1, max);
            int clampedCurrent = Mathf.Clamp(current, 0, clampedMax);

            if (section.valueText != null)
            {
                section.valueText.text = $"{clampedCurrent}/{clampedMax}";
            }

            if (section.fillImage != null)
            {
                section.fillImage.fillAmount = clampedMax > 0 ? (float)clampedCurrent / clampedMax : 0f;
            }
        }

        private void BuildUi()
        {
            rootRect = transform as RectTransform;
            overlayCanvas = GetComponent<Canvas>();
            overlayScaler = GetComponent<CanvasScaler>();
            if (overlayScaler == null)
            {
                overlayScaler = gameObject.AddComponent<CanvasScaler>();
            }
            rootCanvasGroup = GetComponent<CanvasGroup>();
            _fontAsset = ResolveFont();

            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.overrideSorting = true;
            overlayCanvas.sortingOrder = 145;
            overlayCanvas.pixelPerfect = true;

            overlayScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            overlayScaler.referenceResolution = new Vector2(1980f, 1080f);
            overlayScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            overlayScaler.matchWidthOrHeight = 1f;
            overlayScaler.referencePixelsPerUnit = 16f;

            rootCanvasGroup.interactable = false;
            rootCanvasGroup.blocksRaycasts = false;

            rootRect.anchorMin = new Vector2(1f, 1f);
            rootRect.anchorMax = new Vector2(1f, 1f);
            rootRect.pivot = new Vector2(1f, 1f);
            rootRect.anchoredPosition = new Vector2(-24f, -26f);
            rootRect.sizeDelta = new Vector2(236f, 118f);

            healthSection = CreateSection("HealthCard", new Vector2(0f, 0f), "HP", HealthFrameColor, HealthFillColor);
            energySection = CreateSection("EnergyCard", new Vector2(0f, -56f), "EP", EnergyFrameColor, EnergyFillColor);

            healthSection.visible = false;
            energySection.visible = false;
            ApplySectionState(healthSection);
            ApplySectionState(energySection);
        }

        private SectionRefs CreateSection(string name, Vector2 anchoredPosition, string shortLabel, Color frameColor, Color fillColor)
        {
            RectTransform cardRect = CreateRect(transform, name);
            cardRect.anchorMin = new Vector2(1f, 1f);
            cardRect.anchorMax = new Vector2(1f, 1f);
            cardRect.pivot = new Vector2(1f, 1f);
            cardRect.anchoredPosition = anchoredPosition;
            cardRect.sizeDelta = new Vector2(228f, 48f);

            Image background = cardRect.gameObject.AddComponent<Image>();
            background.color = new Color(0.08f, 0.1f, 0.15f, 0.82f);
            background.raycastTarget = false;

            Outline outline = cardRect.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.08f);
            outline.effectDistance = new Vector2(1f, -1f);
            outline.useGraphicAlpha = true;

            CanvasGroup canvasGroup = cardRect.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            RectTransform accentRect = CreateRect(cardRect, "AccentFrame");
            accentRect.anchorMin = new Vector2(0f, 0f);
            accentRect.anchorMax = new Vector2(1f, 1f);
            accentRect.offsetMin = new Vector2(2f, 2f);
            accentRect.offsetMax = new Vector2(-2f, -2f);
            Image accentImage = accentRect.gameObject.AddComponent<Image>();
            accentImage.color = new Color(frameColor.r, frameColor.g, frameColor.b, 0.14f);
            accentImage.raycastTarget = false;

            RectTransform labelTagRect = CreateRect(cardRect, "Tag");
            labelTagRect.anchorMin = new Vector2(0f, 1f);
            labelTagRect.anchorMax = new Vector2(0f, 1f);
            labelTagRect.pivot = new Vector2(0f, 1f);
            labelTagRect.anchoredPosition = new Vector2(10f, -8f);
            labelTagRect.sizeDelta = new Vector2(36f, 16f);
            Image labelTagImage = labelTagRect.gameObject.AddComponent<Image>();
            labelTagImage.color = new Color(frameColor.r, frameColor.g, frameColor.b, 0.24f);
            labelTagImage.raycastTarget = false;

            TextMeshProUGUI labelText = CreateText(labelTagRect, "TagText", shortLabel, 10f, new Color(0.96f, 0.97f, 1f, 1f), TextAlignmentOptions.Center);
            StretchRect(labelText.rectTransform);

            TextMeshProUGUI valueText = CreateText(cardRect, "ValueText", "0/0", 12f, new Color(0.94f, 0.97f, 1f, 1f), TextAlignmentOptions.Right);
            RectTransform valueRect = valueText.rectTransform;
            valueRect.anchorMin = new Vector2(1f, 1f);
            valueRect.anchorMax = new Vector2(1f, 1f);
            valueRect.pivot = new Vector2(1f, 1f);
            valueRect.anchoredPosition = new Vector2(-10f, -9f);
            valueRect.sizeDelta = new Vector2(80f, 18f);

            RectTransform barFrameRect = CreateRect(cardRect, "BarFrame");
            barFrameRect.anchorMin = new Vector2(0f, 0f);
            barFrameRect.anchorMax = new Vector2(1f, 0f);
            barFrameRect.pivot = new Vector2(0.5f, 0f);
            barFrameRect.offsetMin = new Vector2(10f, 10f);
            barFrameRect.offsetMax = new Vector2(-10f, 28f);
            Image barFrameImage = barFrameRect.gameObject.AddComponent<Image>();
            barFrameImage.color = new Color(0.04f, 0.05f, 0.08f, 0.94f);
            barFrameImage.raycastTarget = false;

            RectTransform barGlowRect = CreateRect(barFrameRect, "BarGlow");
            barGlowRect.anchorMin = new Vector2(0f, 0f);
            barGlowRect.anchorMax = new Vector2(1f, 1f);
            barGlowRect.offsetMin = new Vector2(1f, 1f);
            barGlowRect.offsetMax = new Vector2(-1f, -1f);
            Image barGlowImage = barGlowRect.gameObject.AddComponent<Image>();
            barGlowImage.color = new Color(frameColor.r, frameColor.g, frameColor.b, 0.18f);
            barGlowImage.raycastTarget = false;

            RectTransform fillRect = CreateRect(barFrameRect, "Fill");
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(1f, 1f);
            fillRect.offsetMin = new Vector2(1f, 1f);
            fillRect.offsetMax = new Vector2(-1f, -1f);
            Image fillImage = fillRect.gameObject.AddComponent<Image>();
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = 0;
            fillImage.fillAmount = 0f;
            fillImage.color = fillColor;
            fillImage.raycastTarget = false;

            return new SectionRefs
            {
                root = cardRect,
                canvasGroup = canvasGroup,
                valueText = valueText,
                fillImage = fillImage,
                defaultFillColor = fillColor,
                visible = false,
                alpha = 1f
            };
        }

        private void EnsureBuilt()
        {
            if (rootRect == null || healthSection == null || healthSection.root == null || energySection == null || energySection.root == null)
            {
                BuildUi();
            }
        }

        private SectionRefs GetSection(Channel channel)
        {
            return channel == Channel.Health ? healthSection : energySection;
        }

        private void ApplySectionState(SectionRefs section)
        {
            if (section == null || section.canvasGroup == null)
            {
                return;
            }

            float alpha = section.visible && !_suppressWhileDialogueActive ? section.alpha : 0f;
            section.canvasGroup.alpha = alpha;
            section.canvasGroup.interactable = false;
            section.canvasGroup.blocksRaycasts = false;

            if (section.fillImage != null && section.fillImage.color.a <= 0.001f)
            {
                section.fillImage.color = section.defaultFillColor;
            }
        }

        private void OnDialogueStart(DialogueStartEvent _)
        {
            _suppressWhileDialogueActive = true;
            ApplySectionState(healthSection);
            ApplySectionState(energySection);
        }

        private void OnDialogueEnd(DialogueEndEvent _)
        {
            _suppressWhileDialogueActive = false;
            ApplySectionState(healthSection);
            ApplySectionState(energySection);
        }

        private TMP_FontAsset ResolveFont()
        {
            for (int index = 0; index < PreferredFontResourcePaths.Length; index++)
            {
                TMP_FontAsset candidate = Resources.Load<TMP_FontAsset>(PreferredFontResourcePaths[index]);
                if (candidate != null)
                {
                    return candidate;
                }
            }

            return TMP_Settings.defaultFontAsset;
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

        private static void StretchRect(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static Transform ResolveParent()
        {
            GameObject uiRoot = GameObject.Find("UI");
            if (uiRoot != null)
            {
                Canvas canvas = uiRoot.GetComponent<Canvas>() ?? uiRoot.GetComponentInChildren<Canvas>(true);
                return canvas != null ? canvas.transform : uiRoot.transform;
            }

            Canvas fallback = Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
            return fallback != null ? fallback.transform : null;
        }

        [System.Serializable]
        private sealed class SectionRefs
        {
            public RectTransform root;
            public CanvasGroup canvasGroup;
            public TextMeshProUGUI valueText;
            public Image fillImage;
            public Color defaultFillColor;
            public bool visible;
            public float alpha = 1f;
        }
    }
}
