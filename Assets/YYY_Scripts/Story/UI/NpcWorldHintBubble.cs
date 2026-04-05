using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class NpcWorldHintBubble : MonoBehaviour
    {
        private struct HintRequest
        {
            public Transform AnchorTarget;
            public string KeyLabel;
            public string Caption;
            public string Detail;
            public float BoundaryDistance;
            public int Priority;
            public bool ForceFocus;
        }

        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese SDF",
            "Fonts & Materials/DialogueChinese SoftPixel SDF"
        };

        private const string FontCoverageProbeText = "进入任务再靠近一些按E对话";

        private static NpcWorldHintBubble s_instance;
        private static Sprite s_backgroundSprite;
        private static Texture2D s_backgroundTexture;
        private static HintRequest s_pendingRequest;
        private static bool s_hasPendingRequest;
        private static int s_pendingRequestFrame = -1;

        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;
        [SerializeField] private RectTransform bubbleRect;
        [SerializeField] private RectTransform arrowRect;
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

            Transform parent = ResolveParent();
            GameObject root = new GameObject(
                nameof(NpcWorldHintBubble),
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasGroup));

            if (parent != null)
            {
                root.transform.SetParent(parent, false);
            }

            s_instance = root.AddComponent<NpcWorldHintBubble>();
            s_instance.BuildUi();
            s_instance.Hide();
        }

        public static void RequestShow(
            Transform anchorTarget,
            string keyLabel,
            string caption,
            string detail,
            float boundaryDistance,
            int priority,
            bool forceFocus = false)
        {
            if (anchorTarget == null)
            {
                return;
            }

            EnsureRuntime();
            if (!s_instance.gameObject.activeSelf)
            {
                s_instance.gameObject.SetActive(true);
            }

            HintRequest candidate = new HintRequest
            {
                AnchorTarget = anchorTarget,
                KeyLabel = string.IsNullOrWhiteSpace(keyLabel) ? "E" : keyLabel.Trim(),
                Caption = string.IsNullOrWhiteSpace(caption) ? "交互" : caption.Trim(),
                Detail = detail ?? string.Empty,
                BoundaryDistance = Mathf.Max(0f, boundaryDistance),
                Priority = priority,
                ForceFocus = forceFocus
            };

            if (s_pendingRequestFrame != Time.frameCount || !s_hasPendingRequest)
            {
                s_pendingRequest = candidate;
                s_hasPendingRequest = true;
                s_pendingRequestFrame = Time.frameCount;
                return;
            }

            if (ShouldReplaceRequest(candidate, s_pendingRequest))
            {
                s_pendingRequest = candidate;
            }
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
            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen() || (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive))
            {
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                }

                return;
            }

            if (s_pendingRequestFrame == Time.frameCount && s_hasPendingRequest)
            {
                ApplyPendingRequest(s_pendingRequest);
                s_hasPendingRequest = false;
            }
            else if (_visible)
            {
                Hide();
                return;
            }

            if (!_visible || _anchorTarget == null)
            {
                return;
            }

            if (canvasGroup != null && canvasGroup.alpha < 0.999f)
            {
                canvasGroup.alpha = 1f;
            }

            Reposition();
        }

        public void Show(Transform anchorTarget, string keyLabel, string caption, string detail = "")
        {
            RequestShow(anchorTarget, keyLabel, caption, detail, boundaryDistance: 0f, priority: 0);
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

            if (overlayCanvas != null)
            {
                overlayCanvas.gameObject.SetActive(false);
            }
        }

        private void ApplyPendingRequest(HintRequest request)
        {
            EnsureBuilt();
            _anchorTarget = request.AnchorTarget;
            _visible = true;

            keyText.text = request.KeyLabel;
            captionText.text = request.Caption;
            detailText.text = request.Detail;
            EnsureTextReadable(keyText);
            EnsureTextReadable(captionText);
            EnsureTextReadable(detailText);
            detailText.gameObject.SetActive(!string.IsNullOrWhiteSpace(request.Detail));
            bubbleRect.sizeDelta = string.IsNullOrWhiteSpace(request.Detail)
                ? new Vector2(168f, 56f)
                : new Vector2(194f, 72f);

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            overlayCanvas.gameObject.SetActive(true);
            Reposition();
        }

        private void BuildUi()
        {
            rootRect = transform as RectTransform;
            overlayCanvas = GetComponent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
            _fontAsset = ResolveFont();

            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.overrideSorting = true;
            overlayCanvas.sortingOrder = 166;
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
            bubbleRect.sizeDelta = new Vector2(194f, 72f);
            bubbleBackground = bubbleRect.gameObject.AddComponent<Image>();
            bubbleBackground.sprite = GetOrCreateBackgroundSprite();
            bubbleBackground.type = Image.Type.Sliced;
            bubbleBackground.color = new Color(0.09f, 0.13f, 0.18f, 0.97f);
            bubbleBackground.raycastTarget = false;
            ApplyOutline(bubbleRect.gameObject.AddComponent<Outline>(), new Color(1f, 0.82f, 0.45f, 0.16f), new Vector2(1.5f, -1.5f));
            ApplyShadow(bubbleRect.gameObject.AddComponent<Shadow>(), new Color(0f, 0f, 0f, 0.26f), new Vector2(0f, -4f));

            RectTransform keyPlateRect = CreateRect(bubbleRect, "KeyPlate");
            keyPlateRect.anchorMin = new Vector2(0f, 0.5f);
            keyPlateRect.anchorMax = new Vector2(0f, 0.5f);
            keyPlateRect.pivot = new Vector2(0f, 0.5f);
            keyPlateRect.anchoredPosition = new Vector2(10f, 0f);
            keyPlateRect.sizeDelta = new Vector2(36f, 36f);
            keyPlateImage = keyPlateRect.gameObject.AddComponent<Image>();
            keyPlateImage.sprite = GetOrCreateBackgroundSprite();
            keyPlateImage.type = Image.Type.Sliced;
            keyPlateImage.color = new Color(0.98f, 0.82f, 0.36f, 0.98f);
            keyPlateImage.raycastTarget = false;
            ApplyOutline(keyPlateRect.gameObject.AddComponent<Outline>(), new Color(1f, 1f, 1f, 0.08f), new Vector2(1f, -1f));

            keyText = CreateText(keyPlateRect, "KeyText", "E", 19f, new Color(0.22f, 0.14f, 0.05f, 1f), TextAlignmentOptions.Center);
            keyText.fontStyle = FontStyles.Bold;
            RectTransform keyTextRect = keyText.rectTransform;
            keyTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            keyTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            keyTextRect.pivot = new Vector2(0.5f, 0.5f);
            keyTextRect.anchoredPosition = Vector2.zero;
            keyTextRect.sizeDelta = new Vector2(28f, 26f);

            accentLineImage = CreateRect(bubbleRect, "AccentLine").gameObject.AddComponent<Image>();
            accentLineImage.color = new Color(0.99f, 0.70f, 0.31f, 0.92f);
            accentLineImage.raycastTarget = false;
            RectTransform accentRect = accentLineImage.rectTransform;
            accentRect.anchorMin = new Vector2(0f, 0.5f);
            accentRect.anchorMax = new Vector2(0f, 0.5f);
            accentRect.pivot = new Vector2(0f, 0.5f);
            accentRect.anchoredPosition = new Vector2(54f, 0f);
            accentRect.sizeDelta = new Vector2(4f, 34f);

            captionText = CreateText(
                bubbleRect,
                "CaptionText",
                "交互",
                15f,
                new Color(0.98f, 0.96f, 0.91f, 1f),
                TextAlignmentOptions.Left);
            captionText.fontStyle = FontStyles.Bold;
            RectTransform captionRect = captionText.rectTransform;
            captionRect.anchorMin = new Vector2(0f, 1f);
            captionRect.anchorMax = new Vector2(1f, 1f);
            captionRect.pivot = new Vector2(0f, 1f);
            captionRect.offsetMin = new Vector2(66f, -28f);
            captionRect.offsetMax = new Vector2(-12f, -8f);

            detailText = CreateText(
                bubbleRect,
                "DetailText",
                string.Empty,
                11.5f,
                new Color(0.80f, 0.88f, 0.96f, 0.96f),
                TextAlignmentOptions.Left);
            RectTransform detailRect = detailText.rectTransform;
            detailRect.anchorMin = new Vector2(0f, 0f);
            detailRect.anchorMax = new Vector2(1f, 0f);
            detailRect.pivot = new Vector2(0f, 0f);
            detailRect.offsetMin = new Vector2(66f, 10f);
            detailRect.offsetMax = new Vector2(-12f, 30f);
            detailText.gameObject.SetActive(false);

            arrowRect = CreateRect(transform, "Arrow");
            arrowRect.anchorMin = new Vector2(0.5f, 0.5f);
            arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
            arrowRect.pivot = new Vector2(0.5f, 1f);
            arrowRect.sizeDelta = new Vector2(18f, 18f);
            arrowRect.localRotation = Quaternion.Euler(0f, 0f, 45f);
            arrowImage = arrowRect.gameObject.AddComponent<Image>();
            arrowImage.sprite = GetOrCreateBackgroundSprite();
            arrowImage.type = Image.Type.Sliced;
            arrowImage.color = bubbleBackground.color;
            arrowImage.raycastTarget = false;
            ApplyOutline(arrowRect.gameObject.AddComponent<Outline>(), new Color(1f, 0.82f, 0.45f, 0.14f), new Vector2(1f, -1f));
        }

        private void EnsureBuilt()
        {
            if (bubbleRect == null || arrowRect == null || keyText == null || captionText == null || detailText == null)
            {
                BuildUi();
            }
        }

        private void Reposition()
        {
            Camera worldCamera = SpringDay1UiLayerUtility.GetWorldProjectionCamera(overlayCanvas);
            if (worldCamera == null)
            {
                return;
            }

            Bounds bounds = ResolveBounds();
            Vector3 worldAnchor = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(worldCamera, worldAnchor);
            screenPoint.y += 38f + Mathf.Sin(Time.unscaledTime * 2.6f) * 2.4f;

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
            arrowRect.anchoredPosition = localPoint + new Vector2(0f, -8f);
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

        private static bool ShouldReplaceRequest(HintRequest candidate, HintRequest current)
        {
            if (candidate.ForceFocus != current.ForceFocus)
            {
                return candidate.ForceFocus;
            }

            if (!Mathf.Approximately(candidate.BoundaryDistance, current.BoundaryDistance))
            {
                return candidate.BoundaryDistance < current.BoundaryDistance;
            }

            if (candidate.Priority != current.Priority)
            {
                return candidate.Priority > current.Priority;
            }

            int candidateId = candidate.AnchorTarget != null ? candidate.AnchorTarget.GetInstanceID() : int.MaxValue;
            int currentId = current.AnchorTarget != null ? current.AnchorTarget.GetInstanceID() : int.MaxValue;
            return candidateId < currentId;
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

        private static void ApplyOutline(Outline outline, Color color, Vector2 effectDistance)
        {
            outline.effectColor = color;
            outline.effectDistance = effectDistance;
            outline.useGraphicAlpha = true;
        }

        private static void ApplyShadow(Shadow shadow, Color color, Vector2 effectDistance)
        {
            shadow.effectColor = color;
            shadow.effectDistance = effectDistance;
            shadow.useGraphicAlpha = true;
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
