using Sunset.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class SpringDay1WorldHintBubble : MonoBehaviour
    {
        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese V2 SDF",
            "Fonts & Materials/DialogueChinese BitmapSong SDF",
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese SoftPixel SDF",
            "Fonts & Materials/DialogueChinese SDF"
        };

        private static SpringDay1WorldHintBubble _instance;

        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;
        [SerializeField] private RectTransform bubbleRect;
        [SerializeField] private RectTransform arrowRect;
        [SerializeField] private TextMeshProUGUI keyText;
        [SerializeField] private TextMeshProUGUI captionText;

        private TMP_FontAsset _fontAsset;
        private Transform _anchorTarget;
        private bool _visible;
        private bool _suppressWhileDialogueActive;

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

            Reposition();
        }

        public void Show(Transform anchorTarget, string keyLabel = "E", string caption = "")
        {
            if (anchorTarget == null)
            {
                Hide();
                return;
            }

            EnsureBuilt();
            _anchorTarget = anchorTarget;
            _visible = true;
            keyText.text = string.IsNullOrWhiteSpace(keyLabel) ? "E" : keyLabel;
            captionText.text = caption ?? string.Empty;
            captionText.gameObject.SetActive(!string.IsNullOrWhiteSpace(caption));
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

        private void BuildUi()
        {
            rootRect = transform as RectTransform;
            overlayCanvas = GetComponent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
            _fontAsset = ResolveFont();

            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.overrideSorting = true;
            overlayCanvas.sortingOrder = 176;
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
            bubbleRect.sizeDelta = new Vector2(86f, 64f);

            Image background = bubbleRect.gameObject.AddComponent<Image>();
            background.color = new Color(0.1f, 0.11f, 0.16f, 0.92f);
            background.raycastTarget = false;
            Outline outline = bubbleRect.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.1f);
            outline.effectDistance = new Vector2(1f, -1f);
            outline.useGraphicAlpha = true;

            keyText = CreateText(bubbleRect, "KeyText", "E", 24f, new Color(0.98f, 0.88f, 0.56f, 1f), TextAlignmentOptions.Center);
            RectTransform keyRect = keyText.rectTransform;
            keyRect.anchorMin = new Vector2(0.5f, 0.5f);
            keyRect.anchorMax = new Vector2(0.5f, 0.5f);
            keyRect.pivot = new Vector2(0.5f, 0.5f);
            keyRect.anchoredPosition = new Vector2(0f, 8f);
            keyRect.sizeDelta = new Vector2(52f, 28f);

            captionText = CreateText(bubbleRect, "CaptionText", string.Empty, 10f, new Color(0.92f, 0.95f, 1f, 0.92f), TextAlignmentOptions.Center);
            RectTransform captionRect = captionText.rectTransform;
            captionRect.anchorMin = new Vector2(0.5f, 0f);
            captionRect.anchorMax = new Vector2(0.5f, 0f);
            captionRect.pivot = new Vector2(0.5f, 0f);
            captionRect.anchoredPosition = new Vector2(0f, 6f);
            captionRect.sizeDelta = new Vector2(70f, 14f);

            arrowRect = CreateRect(transform, "Arrow");
            arrowRect.anchorMin = new Vector2(0.5f, 0.5f);
            arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
            arrowRect.pivot = new Vector2(0.5f, 1f);
            arrowRect.sizeDelta = new Vector2(14f, 14f);
            arrowRect.localRotation = Quaternion.Euler(0f, 0f, 45f);
            Image arrowImage = arrowRect.gameObject.AddComponent<Image>();
            arrowImage.color = new Color(0.1f, 0.11f, 0.16f, 0.92f);
            arrowImage.raycastTarget = false;
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
            Camera camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            Bounds bounds = ResolveBounds();
            Vector3 worldAnchor = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldAnchor);
            screenPoint.y += 36f + Mathf.Sin(Time.unscaledTime * 3.2f) * 2.5f;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRect, screenPoint, null, out Vector2 localPoint))
            {
                return;
            }

            bubbleRect.anchoredPosition = localPoint;
            arrowRect.anchoredPosition = localPoint + new Vector2(0f, -4f);
        }

        private Bounds ResolveBounds()
        {
            CraftingStationInteractable interactable = _anchorTarget != null ? _anchorTarget.GetComponent<CraftingStationInteractable>() : null;
            if (interactable != null)
            {
                return interactable.GetCombinedBounds();
            }

            Collider2D collider2D = _anchorTarget != null ? (_anchorTarget.GetComponent<Collider2D>() ?? _anchorTarget.GetComponentInChildren<Collider2D>()) : null;
            if (collider2D != null)
            {
                return collider2D.bounds;
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

        private static RectTransform CreateRect(Transform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
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
    }
}
