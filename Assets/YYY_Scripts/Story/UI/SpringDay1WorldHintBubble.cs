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
            "Fonts & Materials/DialogueChinese SDF"
        };

        private static SpringDay1WorldHintBubble _instance;

        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;
        [SerializeField] private RectTransform bubbleRect;
        [SerializeField] private RectTransform arrowRect;
        [SerializeField] private Image bubbleBackground;
        [SerializeField] private Image arrowImage;
        [SerializeField] private Image keyPlateImage;
        [SerializeField] private TextMeshProUGUI keyText;
        [SerializeField] private TextMeshProUGUI captionText;
        [SerializeField] private TextMeshProUGUI detailText;

        private TMP_FontAsset _fontAsset;
        private Transform _anchorTarget;
        private bool _visible;
        private bool _suppressWhileDialogueActive;
        private HintVisualKind _visualKind;

        public Transform CurrentAnchorTarget => _anchorTarget;

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

        public void Show(Transform anchorTarget, string keyLabel = "E", string caption = "", string detail = "", HintVisualKind visualKind = HintVisualKind.Interaction)
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
            keyText.text = string.IsNullOrWhiteSpace(keyLabel) ? "E" : keyLabel;
            captionText.text = string.IsNullOrWhiteSpace(caption) ? "交互" : caption;
            detailText.text = detail ?? string.Empty;
            detailText.gameObject.SetActive(!string.IsNullOrWhiteSpace(detail));
            bubbleRect.sizeDelta = string.IsNullOrWhiteSpace(detail) ? new Vector2(118f, 42f) : new Vector2(128f, 54f);
            ApplyVisualStyle();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            Reposition();
        }

        public void Hide()
        {
            _visible = false;
            _anchorTarget = null;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
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

            bubbleRect = CreateRect(transform, "Bubble");
            bubbleRect.anchorMin = new Vector2(0.5f, 0.5f);
            bubbleRect.anchorMax = new Vector2(0.5f, 0.5f);
            bubbleRect.pivot = new Vector2(0.5f, 0f);
            bubbleRect.sizeDelta = new Vector2(118f, 42f);

            bubbleBackground = bubbleRect.gameObject.AddComponent<Image>();
            bubbleBackground.raycastTarget = false;
            Outline outline = bubbleRect.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.45f, 0.3f, 0.15f, 0.18f);
            outline.effectDistance = new Vector2(1f, -1f);
            outline.useGraphicAlpha = true;
            Shadow shadow = bubbleRect.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.22f);
            shadow.effectDistance = new Vector2(0f, -3f);
            shadow.useGraphicAlpha = true;

            RectTransform keyPlateRect = CreateRect(bubbleRect, "KeyPlate");
            keyPlateRect.anchorMin = new Vector2(0f, 0.5f);
            keyPlateRect.anchorMax = new Vector2(0f, 0.5f);
            keyPlateRect.pivot = new Vector2(0f, 0.5f);
            keyPlateRect.anchoredPosition = new Vector2(8f, 0f);
            keyPlateRect.sizeDelta = new Vector2(28f, 28f);
            keyPlateImage = keyPlateRect.gameObject.AddComponent<Image>();
            ApplyOutline(keyPlateRect.gameObject.AddComponent<Outline>(), new Color(1f, 1f, 1f, 0.1f), new Vector2(1f, -1f));

            keyText = CreateText(keyPlateRect, "KeyText", "E", 18f, new Color(0.26f, 0.17f, 0.08f, 1f), TextAlignmentOptions.Center);
            RectTransform keyRect = keyText.rectTransform;
            keyRect.anchorMin = new Vector2(0.5f, 0.5f);
            keyRect.anchorMax = new Vector2(0.5f, 0.5f);
            keyRect.pivot = new Vector2(0.5f, 0.5f);
            keyRect.anchoredPosition = Vector2.zero;
            keyRect.sizeDelta = new Vector2(22f, 22f);

            captionText = CreateText(bubbleRect, "CaptionText", string.Empty, 12f, new Color(0.26f, 0.17f, 0.08f, 1f), TextAlignmentOptions.Left);
            RectTransform captionRect = captionText.rectTransform;
            captionRect.anchorMin = new Vector2(0f, 1f);
            captionRect.anchorMax = new Vector2(1f, 1f);
            captionRect.pivot = new Vector2(0f, 1f);
            captionRect.offsetMin = new Vector2(44f, -22f);
            captionRect.offsetMax = new Vector2(-10f, -6f);

            detailText = CreateText(bubbleRect, "DetailText", string.Empty, 10f, new Color(0.36f, 0.26f, 0.16f, 0.92f), TextAlignmentOptions.Left);
            RectTransform detailRect = detailText.rectTransform;
            detailRect.anchorMin = new Vector2(0f, 0f);
            detailRect.anchorMax = new Vector2(1f, 0f);
            detailRect.pivot = new Vector2(0f, 0f);
            detailRect.offsetMin = new Vector2(44f, 8f);
            detailRect.offsetMax = new Vector2(-10f, 24f);
            detailText.gameObject.SetActive(false);

            arrowRect = CreateRect(transform, "Arrow");
            arrowRect.anchorMin = new Vector2(0.5f, 0.5f);
            arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
            arrowRect.pivot = new Vector2(0.5f, 1f);
            arrowRect.sizeDelta = new Vector2(14f, 14f);
            arrowRect.localRotation = Quaternion.Euler(0f, 0f, 45f);
            arrowImage = arrowRect.gameObject.AddComponent<Image>();
            arrowImage.raycastTarget = false;
            ApplyVisualStyle();
        }

        private void EnsureBuilt()
        {
            if (bubbleRect == null || arrowRect == null || keyText == null || captionText == null)
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
            screenPoint.y += 28f + Mathf.Sin(Time.unscaledTime * 2.4f) * 1.8f;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRect, screenPoint, SpringDay1UiLayerUtility.GetUiEventCamera(overlayCanvas), out Vector2 localPoint))
            {
                return;
            }

            bubbleRect.anchoredPosition = localPoint;
            arrowRect.anchoredPosition = localPoint + new Vector2(0f, -4f);
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
            _suppressWhileDialogueActive = false;
            if (_visible && canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }

        private void ApplyVisualStyle()
        {
            Color bubbleColor = _visualKind == HintVisualKind.Tutorial
                ? new Color(0.96f, 0.88f, 0.68f, 0.96f)
                : new Color(0.94f, 0.88f, 0.76f, 0.94f);
            Color keyPlateColor = _visualKind == HintVisualKind.Tutorial
                ? new Color(0.98f, 0.8f, 0.42f, 0.98f)
                : new Color(0.87f, 0.71f, 0.36f, 0.98f);

            if (bubbleBackground != null)
            {
                bubbleBackground.color = bubbleColor;
            }

            if (arrowImage != null)
            {
                arrowImage.color = bubbleColor;
            }

            if (keyPlateImage != null)
            {
                keyPlateImage.color = keyPlateColor;
            }
        }

        private static void ApplyOutline(Outline outline, Color color, Vector2 distance)
        {
            outline.effectColor = color;
            outline.effectDistance = distance;
            outline.useGraphicAlpha = true;
        }

        private static RectTransform CreateRect(Transform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private static Transform ResolveParent()
        {
            return SpringDay1UiLayerUtility.ResolveUiParent();
        }
    }
}
