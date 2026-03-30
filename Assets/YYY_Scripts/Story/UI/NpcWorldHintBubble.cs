using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class NpcWorldHintBubble : MonoBehaviour
    {
        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese SDF",
            "Fonts & Materials/DialogueChinese SoftPixel SDF"
        };

        private static NpcWorldHintBubble s_instance;
        private static Sprite s_backgroundSprite;
        private static Texture2D s_backgroundTexture;

        [SerializeField] private Canvas worldCanvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;
        [SerializeField] private RectTransform bubbleRect;
        [SerializeField] private RectTransform tailRect;
        [SerializeField] private Image bubbleBackground;
        [SerializeField] private Image tailImage;
        [SerializeField] private Image keyPlateImage;
        [SerializeField] private Image accentLineImage;
        [SerializeField] private TextMeshProUGUI keyText;
        [SerializeField] private TextMeshProUGUI captionText;
        [SerializeField] private TextMeshProUGUI detailText;

        private TMP_FontAsset _fontAsset;
        private Transform _anchorTarget;
        private SpriteRenderer _anchorRenderer;
        private bool _visible;

        public Transform CurrentAnchorTarget => _anchorTarget;

        public static NpcWorldHintBubble Instance
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

            GameObject root = new GameObject(
                nameof(NpcWorldHintBubble),
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasGroup));
            s_instance = root.AddComponent<NpcWorldHintBubble>();
            s_instance.BuildUi();
            s_instance.Hide();
        }

        public static void HideIfExists(Transform anchorTarget = null)
        {
            if (s_instance == null)
            {
                return;
            }

            if (anchorTarget == null || s_instance._anchorTarget == anchorTarget)
            {
                s_instance.Hide();
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

        private void LateUpdate()
        {
            if (!_visible || _anchorTarget == null)
            {
                return;
            }

            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen() || (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive))
            {
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                }

                return;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            Reposition();
        }

        public void Show(Transform anchorTarget, string keyLabel, string caption, string detail = "")
        {
            if (anchorTarget == null)
            {
                Hide();
                return;
            }

            EnsureBuilt();
            _anchorTarget = anchorTarget;
            _anchorRenderer = anchorTarget.GetComponentInChildren<SpriteRenderer>();
            _visible = true;

            keyText.text = string.IsNullOrWhiteSpace(keyLabel) ? "E" : keyLabel.Trim();
            captionText.text = string.IsNullOrWhiteSpace(caption) ? "交谈" : caption.Trim();
            detailText.text = detail ?? string.Empty;
            detailText.gameObject.SetActive(!string.IsNullOrWhiteSpace(detail));
            bubbleRect.sizeDelta = string.IsNullOrWhiteSpace(detail) ? new Vector2(2.06f, 0.62f) : new Vector2(2.44f, 0.82f);
            if (tailRect != null)
            {
                tailRect.anchoredPosition = new Vector2(0f, -0.10f);
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            worldCanvas.gameObject.SetActive(true);
            Reposition();
        }

        public void Hide()
        {
            _visible = false;
            _anchorTarget = null;
            _anchorRenderer = null;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            if (worldCanvas != null)
            {
                worldCanvas.gameObject.SetActive(false);
            }
        }

        private void BuildUi()
        {
            rootRect = transform as RectTransform;
            worldCanvas = GetComponent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
            _fontAsset = ResolveFont();

            worldCanvas.renderMode = RenderMode.WorldSpace;
            worldCanvas.overrideSorting = true;
            worldCanvas.pixelPerfect = false;
            rootRect.sizeDelta = new Vector2(2.8f, 1.4f);

            bubbleRect = CreateRect(rootRect, "Bubble");
            bubbleRect.anchorMin = new Vector2(0.5f, 0.5f);
            bubbleRect.anchorMax = new Vector2(0.5f, 0.5f);
            bubbleRect.pivot = new Vector2(0.5f, 0.5f);
            bubbleRect.sizeDelta = new Vector2(2.2f, 0.78f);
            bubbleBackground = bubbleRect.gameObject.AddComponent<Image>();
            bubbleBackground.sprite = GetOrCreateBackgroundSprite();
            bubbleBackground.type = Image.Type.Sliced;
            bubbleBackground.color = new Color(0.07f, 0.09f, 0.13f, 0.97f);
            bubbleBackground.raycastTarget = false;

            Outline outline = bubbleRect.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.95f, 0.77f, 0.56f, 0.26f);
            outline.effectDistance = new Vector2(1.2f, -1.2f);
            outline.useGraphicAlpha = true;

            Shadow shadow = bubbleRect.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.30f);
            shadow.effectDistance = new Vector2(0f, -2.4f);
            shadow.useGraphicAlpha = true;

            tailRect = CreateRect(bubbleRect, "Tail");
            tailRect.anchorMin = new Vector2(0.5f, 0f);
            tailRect.anchorMax = new Vector2(0.5f, 0f);
            tailRect.pivot = new Vector2(0.5f, 0.5f);
            tailRect.anchoredPosition = new Vector2(0f, -0.10f);
            tailRect.sizeDelta = new Vector2(0.22f, 0.22f);
            tailRect.localRotation = Quaternion.Euler(0f, 0f, 45f);
            tailImage = tailRect.gameObject.AddComponent<Image>();
            tailImage.sprite = GetOrCreateBackgroundSprite();
            tailImage.type = Image.Type.Sliced;
            tailImage.color = bubbleBackground.color;
            tailImage.raycastTarget = false;
            Outline tailOutline = tailRect.gameObject.AddComponent<Outline>();
            tailOutline.effectColor = outline.effectColor;
            tailOutline.effectDistance = outline.effectDistance;
            tailOutline.useGraphicAlpha = true;

            RectTransform keyPlateRect = CreateRect(bubbleRect, "KeyPlate");
            keyPlateRect.anchorMin = new Vector2(0f, 0.5f);
            keyPlateRect.anchorMax = new Vector2(0f, 0.5f);
            keyPlateRect.pivot = new Vector2(0f, 0.5f);
            keyPlateRect.anchoredPosition = new Vector2(0.16f, 0f);
            keyPlateRect.sizeDelta = new Vector2(0.48f, 0.48f);
            keyPlateImage = keyPlateRect.gameObject.AddComponent<Image>();
            keyPlateImage.sprite = GetOrCreateBackgroundSprite();
            keyPlateImage.type = Image.Type.Sliced;
            keyPlateImage.color = new Color(0.96f, 0.88f, 0.70f, 0.98f);
            keyPlateImage.raycastTarget = false;

            keyText = CreateText(keyPlateRect, "KeyText", "E", 16f, new Color(0.16f, 0.10f, 0.06f, 1f), TextAlignmentOptions.Center);
            keyText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            keyText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            keyText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            keyText.rectTransform.anchoredPosition = Vector2.zero;
            keyText.rectTransform.sizeDelta = new Vector2(0.30f, 0.24f);

            accentLineImage = CreateRect(bubbleRect, "AccentLine").gameObject.AddComponent<Image>();
            accentLineImage.color = new Color(0.95f, 0.71f, 0.40f, 0.88f);
            accentLineImage.raycastTarget = false;
            RectTransform accentRect = accentLineImage.rectTransform;
            accentRect.anchorMin = new Vector2(0f, 0.5f);
            accentRect.anchorMax = new Vector2(0f, 0.5f);
            accentRect.pivot = new Vector2(0f, 0.5f);
            accentRect.anchoredPosition = new Vector2(0.74f, 0f);
            accentRect.sizeDelta = new Vector2(0.05f, 0.40f);

            captionText = CreateText(bubbleRect, "CaptionText", "交谈", 12f, new Color(0.98f, 0.95f, 0.90f, 1f), TextAlignmentOptions.Left);
            RectTransform captionRect = captionText.rectTransform;
            captionRect.anchorMin = new Vector2(0f, 0.5f);
            captionRect.anchorMax = new Vector2(1f, 0.5f);
            captionRect.pivot = new Vector2(0f, 0.5f);
            captionRect.anchoredPosition = new Vector2(0.88f, 0.14f);
            captionRect.sizeDelta = new Vector2(-1.00f, 0.22f);

            detailText = CreateText(bubbleRect, "DetailText", string.Empty, 9.5f, new Color(0.77f, 0.84f, 0.92f, 0.92f), TextAlignmentOptions.Left);
            RectTransform detailRect = detailText.rectTransform;
            detailRect.anchorMin = new Vector2(0f, 0.5f);
            detailRect.anchorMax = new Vector2(1f, 0.5f);
            detailRect.pivot = new Vector2(0f, 0.5f);
            detailRect.anchoredPosition = new Vector2(0.88f, -0.14f);
            detailRect.sizeDelta = new Vector2(-1.00f, 0.18f);
            detailText.gameObject.SetActive(false);

            Hide();
        }

        private void EnsureBuilt()
        {
            if (bubbleRect == null || keyText == null || captionText == null || detailText == null)
            {
                BuildUi();
            }
        }

        private void Reposition()
        {
            if (_anchorTarget == null)
            {
                return;
            }

            Bounds bounds = SpringDay1UiLayerUtility.TryGetPresentationBounds(_anchorTarget, out Bounds resolvedBounds)
                ? resolvedBounds
                : new Bounds(_anchorTarget.position, Vector3.one);
            Vector3 targetPosition = new Vector3(
                bounds.center.x,
                bounds.max.y + 0.36f + (Mathf.Sin(Time.unscaledTime * 3f) * 0.026f),
                0f);

            transform.position = targetPosition;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one * 0.01f;

            if (_anchorRenderer != null)
            {
                worldCanvas.sortingLayerID = _anchorRenderer.sortingLayerID;
                worldCanvas.sortingOrder = _anchorRenderer.sortingOrder + 42;
            }
            else
            {
                worldCanvas.sortingLayerName = "Default";
                worldCanvas.sortingOrder = 42;
            }
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

        private static Sprite GetOrCreateBackgroundSprite()
        {
            if (s_backgroundSprite != null)
            {
                return s_backgroundSprite;
            }

            const int width = 48;
            const int height = 24;
            const int radius = 6;

            s_backgroundTexture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false)
            {
                name = "NpcHintBubbleRuntimeTexture",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool inside = IsInsideRoundedRect(x, y, width, height, radius);
                    s_backgroundTexture.SetPixel(x, y, inside ? Color.white : Color.clear);
                }
            }

            s_backgroundTexture.Apply(updateMipmaps: false, makeNoLongerReadable: true);
            s_backgroundSprite = Sprite.Create(
                s_backgroundTexture,
                new Rect(0f, 0f, width, height),
                new Vector2(0.5f, 0.5f),
                32f,
                0u,
                SpriteMeshType.FullRect,
                new Vector4(radius, radius, radius, radius));
            s_backgroundSprite.name = "NpcHintBubbleRuntimeSprite";
            s_backgroundSprite.hideFlags = HideFlags.HideAndDontSave;
            return s_backgroundSprite;
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
