using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// NPC 头顶气泡表现器。
/// 使用独立世界空间 UI，不接入全屏 DialogueUI。
/// </summary>
[DisallowMultipleComponent]
public class NPCBubblePresenter : MonoBehaviour
{
    private const int CurrentStyleVersion = 2;

    private static readonly string[] PreferredFontResourcePaths =
    {
        "Fonts & Materials/DialogueChinese Pixel SDF",
        "Fonts & Materials/DialogueChinese SDF",
        "Fonts & Materials/DialogueChinese SoftPixel SDF"
    };

    private static readonly Vector2 LegacyBubblePadding = new Vector2(24f, 16f);
    private static readonly Color LegacyBubbleColor = new Color(0.13f, 0.13f, 0.13f, 0.92f);
    private static readonly Color LegacyTextColor = new Color(0.97f, 0.97f, 0.97f, 1f);

    private static Sprite s_runtimeBubbleSprite;
    private static Texture2D s_runtimeBubbleTexture;
    private static Sprite s_runtimeTailSprite;
    private static Texture2D s_runtimeTailTexture;

    [Header("组件引用")]
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private TMP_FontAsset fontAsset;

    [Header("气泡布局")]
    [SerializeField] private Vector3 bubbleLocalOffset = new Vector3(0f, 1.1f, 0f);
    [SerializeField] private Vector3 bubbleLocalScale = new Vector3(0.01f, 0.01f, 0.01f);
    [SerializeField] private Vector2 bubblePadding = new Vector2(28f, 18f);
    [SerializeField] private float maxTextWidth = 228f;
    [SerializeField] private float borderThickness = 6f;
    [SerializeField] private Vector2 tailSize = new Vector2(22f, 12f);
    [SerializeField] private float tailYOffset = -6f;
    [SerializeField] private Vector2 shadowOffset = new Vector2(4f, -5f);
    [SerializeField] private int sortingOrderOffset = 20;

    [Header("气泡样式")]
    [SerializeField] private Color bubbleBorderColor = new Color(0.92f, 0.79f, 0.56f, 1f);
    [SerializeField] private Color bubbleColor = new Color(0.10f, 0.12f, 0.16f, 0.96f);
    [SerializeField] private Color bubbleShadowColor = new Color(0.01f, 0.02f, 0.04f, 0.34f);
    [SerializeField] private Color textColor = new Color(0.98f, 0.95f, 0.90f, 1f);
    [SerializeField] private Color textOutlineColor = new Color(0.05f, 0.06f, 0.09f, 0.96f);
    [SerializeField] private float fontSize = 22f;
    [SerializeField] private float textOutlineWidth = 0.18f;
    [SerializeField] private float showDuration = 0.12f;
    [SerializeField] private float hideDuration = 0.10f;

    [Header("默认文本池")]
    [SerializeField] private string[] selfTalkLines =
    {
        "先在这边看看。",
        "今天还挺安静。",
        "休息一下再走。",
        "嗯，这边风有点舒服。",
        "不知道大家在忙什么。"
    };

    [Header("调试")]
    [SerializeField] private bool showDebugLog = false;
    [SerializeField, HideInInspector] private int styleVersion;

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _canvasRect;
    private RectTransform _bubbleRoot;
    private RectTransform _shadowBodyRect;
    private RectTransform _shadowTailRect;
    private RectTransform _borderBodyRect;
    private RectTransform _borderTailRect;
    private RectTransform _fillBodyRect;
    private RectTransform _fillTailRect;
    private Image _shadowBodyImage;
    private Image _shadowTailImage;
    private Image _borderBodyImage;
    private Image _borderTailImage;
    private Image _fillBodyImage;
    private Image _fillTailImage;
    private TextMeshProUGUI _bubbleText;
    private Coroutine _hideCoroutine;
    private Coroutine _visibilityCoroutine;

    public bool IsBubbleVisible => _canvas != null && _canvas.gameObject.activeSelf;
    public string CurrentBubbleText => IsBubbleVisible && _bubbleText != null ? _bubbleText.text : string.Empty;
    public int SelfTalkLineCount => selfTalkLines != null ? selfTalkLines.Length : 0;

    private void Reset()
    {
        CacheComponents();
        ApplyCurrentStylePreset();
    }

    private void Awake()
    {
        CacheComponents();
        UpgradeLegacyStyleIfNeeded();
        EnsureBubbleUi();
        HideImmediate();
    }

    private void LateUpdate()
    {
        if (_canvas == null)
        {
            return;
        }

        SyncCanvasTransform();
        SyncSorting();
    }

    private void OnValidate()
    {
        CacheComponents();
        UpgradeLegacyStyleIfNeeded();

        maxTextWidth = Mathf.Max(80f, maxTextWidth);
        fontSize = Mathf.Max(10f, fontSize);
        borderThickness = Mathf.Max(2f, borderThickness);
        tailSize.x = Mathf.Max(12f, tailSize.x);
        tailSize.y = Mathf.Max(8f, tailSize.y);
        bubblePadding.x = Mathf.Max(16f, bubblePadding.x);
        bubblePadding.y = Mathf.Max(10f, bubblePadding.y);
        showDuration = Mathf.Max(0.01f, showDuration);
        hideDuration = Mathf.Max(0.01f, hideDuration);

        if (_canvas != null)
        {
            SyncCanvasTransform();
            UpdateStyleVisuals();
            UpdateLayout();
        }
    }

    private void OnDisable()
    {
        HideImmediate();
    }

    public void ApplyProfile(NPCRoamProfile profile)
    {
        if (profile == null)
        {
            return;
        }

        selfTalkLines = CopyLines(profile.SelfTalkLines);
    }

    public bool ShowRandomSelfTalk(float duration = -1f)
    {
        if (selfTalkLines == null || selfTalkLines.Length == 0)
        {
            return false;
        }

        string content = selfTalkLines[Random.Range(0, selfTalkLines.Length)];
        return ShowText(content, duration);
    }

    public bool ShowText(string content, float duration = -1f)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        EnsureBubbleUi();
        if (_canvas == null || _bubbleText == null || _fillBodyRect == null)
        {
            return false;
        }

        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
            _hideCoroutine = null;
        }

        _canvas.gameObject.SetActive(true);
        SyncCanvasTransform();
        SyncSorting();

        _bubbleText.text = content.Trim();
        UpdateStyleVisuals();
        UpdateLayout();
        StartVisibilityAnimation(visible: true, deactivateAfter: false);

        if (duration > 0f)
        {
            _hideCoroutine = StartCoroutine(HideAfterSeconds(duration));
        }

        if (showDebugLog)
        {
            Debug.Log($"<color=cyan>[NPCBubblePresenter]</color> {name} => {content}", this);
        }

        return true;
    }

    public void HideBubble()
    {
        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
            _hideCoroutine = null;
        }

        if (!Application.isPlaying || _canvas == null || !_canvas.gameObject.activeSelf)
        {
            HideImmediate();
            return;
        }

        StartVisibilityAnimation(visible: false, deactivateAfter: true);
    }

    [ContextMenu("调试/显示随机自言自语")]
    public void DebugShowRandomSelfTalk()
    {
        if (Application.isPlaying)
        {
            ShowRandomSelfTalk(2.5f);
        }
    }

    [ContextMenu("调试/隐藏气泡")]
    public void DebugHideBubble()
    {
        HideBubble();
    }

    private void CacheComponents()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
        }

        if (fontAsset == null)
        {
            fontAsset = TryLoadPreferredFontAsset();
        }
    }

    private void UpgradeLegacyStyleIfNeeded()
    {
        if (styleVersion >= CurrentStyleVersion)
        {
            return;
        }

        if (styleVersion == 0 && LooksLikeLegacyStyle())
        {
            ApplyCurrentStylePreset();
        }

        styleVersion = CurrentStyleVersion;
    }

    private bool LooksLikeLegacyStyle()
    {
        return Approximately(bubblePadding, LegacyBubblePadding) &&
               ColorApproximately(bubbleColor, LegacyBubbleColor) &&
               ColorApproximately(textColor, LegacyTextColor) &&
               Mathf.Approximately(fontSize, 24f);
    }

    private void ApplyCurrentStylePreset()
    {
        bubblePadding = new Vector2(28f, 18f);
        bubbleBorderColor = new Color(0.92f, 0.79f, 0.56f, 1f);
        bubbleColor = new Color(0.10f, 0.12f, 0.16f, 0.96f);
        bubbleShadowColor = new Color(0.01f, 0.02f, 0.04f, 0.34f);
        textColor = new Color(0.98f, 0.95f, 0.90f, 1f);
        textOutlineColor = new Color(0.05f, 0.06f, 0.09f, 0.96f);
        fontSize = 22f;
        textOutlineWidth = 0.18f;
        borderThickness = 6f;
        tailSize = new Vector2(22f, 12f);
        tailYOffset = -6f;
        shadowOffset = new Vector2(4f, -5f);
        showDuration = 0.12f;
        hideDuration = 0.10f;
    }

    private void EnsureBubbleUi()
    {
        if (_canvas != null && _bubbleText != null)
        {
            return;
        }

        TMP_FontAsset resolvedFont = ResolveFontAsset();
        if (resolvedFont == null)
        {
            Debug.LogWarning($"[NPCBubblePresenter] {name} 未找到可用 TMP 字体资源，气泡将不会创建。", this);
            return;
        }

        GameObject canvasObject = new GameObject(
            "NPCBubbleCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasGroup));
        canvasObject.transform.SetParent(transform, false);

        _canvas = canvasObject.GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;
        _canvas.overrideSorting = true;

        _canvasGroup = canvasObject.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        _canvasRect = canvasObject.GetComponent<RectTransform>();
        _canvasRect.pivot = new Vector2(0.5f, 0.5f);
        _canvasRect.anchorMin = new Vector2(0.5f, 0.5f);
        _canvasRect.anchorMax = new Vector2(0.5f, 0.5f);

        GameObject rootObject = new GameObject("BubbleRoot", typeof(RectTransform));
        rootObject.transform.SetParent(canvasObject.transform, false);
        _bubbleRoot = rootObject.GetComponent<RectTransform>();
        _bubbleRoot.anchorMin = new Vector2(0.5f, 0.5f);
        _bubbleRoot.anchorMax = new Vector2(0.5f, 0.5f);
        _bubbleRoot.pivot = new Vector2(0.5f, 0.5f);

        _shadowBodyImage = CreateImage(
            _bubbleRoot,
            "ShadowBody",
            GetOrCreateRuntimeBubbleSprite(),
            Image.Type.Sliced,
            bubbleShadowColor,
            out _shadowBodyRect);

        _shadowTailImage = CreateImage(
            _bubbleRoot,
            "ShadowTail",
            GetOrCreateRuntimeTailSprite(),
            Image.Type.Simple,
            bubbleShadowColor,
            out _shadowTailRect);

        _borderBodyImage = CreateImage(
            _bubbleRoot,
            "BorderBody",
            GetOrCreateRuntimeBubbleSprite(),
            Image.Type.Sliced,
            bubbleBorderColor,
            out _borderBodyRect);

        _borderTailImage = CreateImage(
            _bubbleRoot,
            "BorderTail",
            GetOrCreateRuntimeTailSprite(),
            Image.Type.Simple,
            bubbleBorderColor,
            out _borderTailRect);

        _fillBodyImage = CreateImage(
            _bubbleRoot,
            "FillBody",
            GetOrCreateRuntimeBubbleSprite(),
            Image.Type.Sliced,
            bubbleColor,
            out _fillBodyRect);

        _fillTailImage = CreateImage(
            _bubbleRoot,
            "FillTail",
            GetOrCreateRuntimeTailSprite(),
            Image.Type.Simple,
            bubbleColor,
            out _fillTailRect);

        GameObject textObject = new GameObject("BubbleText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(_fillBodyRect, false);

        _bubbleText = textObject.GetComponent<TextMeshProUGUI>();
        _bubbleText.font = resolvedFont;
        _bubbleText.alignment = TextAlignmentOptions.Center;
        _bubbleText.textWrappingMode = TextWrappingModes.Normal;
        _bubbleText.overflowMode = TextOverflowModes.Overflow;
        _bubbleText.raycastTarget = false;
        _bubbleText.enableAutoSizing = false;
        _bubbleText.extraPadding = true;
        _bubbleText.characterSpacing = 1.25f;
        _bubbleText.lineSpacing = -5f;
        _bubbleText.outlineColor = textOutlineColor;
        _bubbleText.outlineWidth = textOutlineWidth;

        RectTransform textRect = _bubbleText.rectTransform;
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);

        UpdateStyleVisuals();
        UpdateLayout();
        SyncCanvasTransform();
        SyncSorting();
    }

    private void UpdateStyleVisuals()
    {
        if (_bubbleText == null)
        {
            return;
        }

        _shadowBodyImage.color = bubbleShadowColor;
        _shadowTailImage.color = bubbleShadowColor;
        _borderBodyImage.color = bubbleBorderColor;
        _borderTailImage.color = bubbleBorderColor;
        _fillBodyImage.color = bubbleColor;
        _fillTailImage.color = bubbleColor;

        _bubbleText.fontSize = fontSize;
        _bubbleText.color = textColor;
        _bubbleText.outlineColor = textOutlineColor;
        _bubbleText.outlineWidth = textOutlineWidth;
    }

    private void UpdateLayout()
    {
        if (_bubbleText == null)
        {
            return;
        }

        Vector2 preferredSize = _bubbleText.GetPreferredValues(_bubbleText.text, maxTextWidth, 0f);
        preferredSize.x = Mathf.Min(maxTextWidth, Mathf.Max(84f, preferredSize.x));
        preferredSize.y = Mathf.Max(fontSize + 10f, preferredSize.y);

        Vector2 bodySize = preferredSize + bubblePadding;
        Vector2 fillBodySize = new Vector2(
            Mathf.Max(52f, bodySize.x - (borderThickness * 2f)),
            Mathf.Max(fontSize + 12f, bodySize.y - (borderThickness * 2f)));
        Vector2 fillTailSize = new Vector2(
            Mathf.Max(8f, tailSize.x - (borderThickness * 1.6f)),
            Mathf.Max(6f, tailSize.y - (borderThickness * 1.2f)));

        float bodyCenterY = Mathf.Max(tailSize.y * 0.45f, 6f);
        float tailCenterY = bodyCenterY - (bodySize.y * 0.5f) - (tailSize.y * 0.18f) + tailYOffset;

        Vector2 bodyPosition = new Vector2(0f, bodyCenterY);
        Vector2 tailPosition = new Vector2(0f, tailCenterY);
        Vector2 shadowBodyPosition = bodyPosition + shadowOffset;
        Vector2 shadowTailPosition = tailPosition + shadowOffset;

        SetRect(_shadowBodyRect, bodySize, shadowBodyPosition);
        SetRect(_shadowTailRect, tailSize, shadowTailPosition);
        SetRect(_borderBodyRect, bodySize, bodyPosition);
        SetRect(_borderTailRect, tailSize, tailPosition);
        SetRect(_fillBodyRect, fillBodySize, bodyPosition);
        SetRect(_fillTailRect, fillTailSize, tailPosition + Vector2.up);
        SetRect(_bubbleText.rectTransform, preferredSize, bodyPosition);

        float canvasWidth = bodySize.x + Mathf.Abs(shadowOffset.x) + 10f;
        float canvasHeight = bodySize.y + tailSize.y + Mathf.Abs(tailYOffset) + Mathf.Abs(shadowOffset.y) + 12f;
        Vector2 canvasSize = new Vector2(canvasWidth, canvasHeight);

        _canvasRect.sizeDelta = canvasSize;
        _bubbleRoot.sizeDelta = canvasSize;
    }

    private void SyncCanvasTransform()
    {
        if (_canvas == null)
        {
            return;
        }

        Transform canvasTransform = _canvas.transform;
        canvasTransform.localPosition = bubbleLocalOffset;
        canvasTransform.localRotation = Quaternion.identity;
        canvasTransform.localScale = bubbleLocalScale;
    }

    private void SyncSorting()
    {
        if (_canvas == null)
        {
            return;
        }

        if (targetRenderer != null)
        {
            _canvas.sortingLayerID = targetRenderer.sortingLayerID;
            _canvas.sortingOrder = targetRenderer.sortingOrder + sortingOrderOffset;
        }
        else
        {
            _canvas.sortingOrder = sortingOrderOffset;
        }
    }

    private void HideImmediate()
    {
        if (_visibilityCoroutine != null)
        {
            StopCoroutine(_visibilityCoroutine);
            _visibilityCoroutine = null;
        }

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
        }

        if (_bubbleRoot != null)
        {
            _bubbleRoot.localScale = new Vector3(0.88f, 0.88f, 1f);
        }

        if (_canvas != null)
        {
            _canvas.gameObject.SetActive(false);
        }
    }

    private void StartVisibilityAnimation(bool visible, bool deactivateAfter)
    {
        if (_canvas == null || _canvasGroup == null || _bubbleRoot == null)
        {
            return;
        }

        if (_visibilityCoroutine != null)
        {
            StopCoroutine(_visibilityCoroutine);
        }

        _visibilityCoroutine = StartCoroutine(AnimateVisibility(visible, deactivateAfter));
    }

    private IEnumerator AnimateVisibility(bool visible, bool deactivateAfter)
    {
        float duration = visible ? showDuration : hideDuration;
        float startAlpha = _canvasGroup.alpha;
        float endAlpha = visible ? 1f : 0f;
        Vector3 startScale = _bubbleRoot.localScale;
        Vector3 endScale = visible ? Vector3.one : new Vector3(0.88f, 0.88f, 1f);

        if (duration <= 0.001f)
        {
            _canvasGroup.alpha = endAlpha;
            _bubbleRoot.localScale = endScale;
            _visibilityCoroutine = null;

            if (!visible && deactivateAfter)
            {
                _canvas.gameObject.SetActive(false);
            }

            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = 1f - Mathf.Pow(1f - t, 3f);

            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            _bubbleRoot.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        _canvasGroup.alpha = endAlpha;
        _bubbleRoot.localScale = endScale;
        _visibilityCoroutine = null;

        if (!visible && deactivateAfter)
        {
            _canvas.gameObject.SetActive(false);
        }
    }

    private IEnumerator HideAfterSeconds(float duration)
    {
        yield return new WaitForSeconds(Mathf.Max(0.1f, duration));
        _hideCoroutine = null;
        HideBubble();
    }

    private TMP_FontAsset ResolveFontAsset()
    {
        if (fontAsset != null)
        {
            return fontAsset;
        }

        fontAsset = TryLoadPreferredFontAsset();
        if (fontAsset != null)
        {
            return fontAsset;
        }

        return TMP_Settings.defaultFontAsset;
    }

    private TMP_FontAsset TryLoadPreferredFontAsset()
    {
        for (int index = 0; index < PreferredFontResourcePaths.Length; index++)
        {
            TMP_FontAsset candidate = Resources.Load<TMP_FontAsset>(PreferredFontResourcePaths[index]);
            if (candidate != null)
            {
                return candidate;
            }
        }

        return null;
    }

    private static Image CreateImage(
        Transform parent,
        string objectName,
        Sprite sprite,
        Image.Type imageType,
        Color color,
        out RectTransform rectTransform)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);

        Image image = imageObject.GetComponent<Image>();
        image.sprite = sprite;
        image.type = imageType;
        image.color = color;
        image.raycastTarget = false;

        rectTransform = image.rectTransform;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        return image;
    }

    private static Sprite GetOrCreateRuntimeBubbleSprite()
    {
        if (s_runtimeBubbleSprite != null)
        {
            return s_runtimeBubbleSprite;
        }

        const int width = 32;
        const int height = 24;
        const int radius = 6;

        s_runtimeBubbleTexture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false)
        {
            name = "NPCBubbleRuntimeTexture",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool inside = IsInsideRoundedRect(x, y, width, height, radius);
                s_runtimeBubbleTexture.SetPixel(x, y, inside ? Color.white : Color.clear);
            }
        }

        s_runtimeBubbleTexture.Apply(updateMipmaps: false, makeNoLongerReadable: true);

        s_runtimeBubbleSprite = Sprite.Create(
            s_runtimeBubbleTexture,
            new Rect(0f, 0f, width, height),
            new Vector2(0.5f, 0.5f),
            16f,
            0u,
            SpriteMeshType.FullRect,
            new Vector4(radius, radius, radius, radius));
        s_runtimeBubbleSprite.name = "NPCBubbleRuntimeSprite";
        s_runtimeBubbleSprite.hideFlags = HideFlags.HideAndDontSave;
        return s_runtimeBubbleSprite;
    }

    private static Sprite GetOrCreateRuntimeTailSprite()
    {
        if (s_runtimeTailSprite != null)
        {
            return s_runtimeTailSprite;
        }

        const int width = 18;
        const int height = 12;

        s_runtimeTailTexture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false)
        {
            name = "NPCBubbleTailRuntimeTexture",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };

        float centerX = (width - 1) * 0.5f;
        for (int y = 0; y < height; y++)
        {
            float normalized = y / Mathf.Max(1f, height - 1f);
            float halfWidth = Mathf.Lerp((width - 1) * 0.5f, 0f, normalized);
            for (int x = 0; x < width; x++)
            {
                bool inside = Mathf.Abs(x - centerX) <= halfWidth;
                s_runtimeTailTexture.SetPixel(x, y, inside ? Color.white : Color.clear);
            }
        }

        s_runtimeTailTexture.Apply(updateMipmaps: false, makeNoLongerReadable: true);

        s_runtimeTailSprite = Sprite.Create(
            s_runtimeTailTexture,
            new Rect(0f, 0f, width, height),
            new Vector2(0.5f, 1f),
            16f);
        s_runtimeTailSprite.name = "NPCBubbleTailRuntimeSprite";
        s_runtimeTailSprite.hideFlags = HideFlags.HideAndDontSave;
        return s_runtimeTailSprite;
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

    private static void SetRect(RectTransform rect, Vector2 size, Vector2 anchoredPosition)
    {
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;
        rect.localRotation = Quaternion.identity;
    }

    private static bool Approximately(Vector2 left, Vector2 right)
    {
        return Mathf.Approximately(left.x, right.x) && Mathf.Approximately(left.y, right.y);
    }

    private static bool ColorApproximately(Color left, Color right)
    {
        return Mathf.Approximately(left.r, right.r) &&
               Mathf.Approximately(left.g, right.g) &&
               Mathf.Approximately(left.b, right.b) &&
               Mathf.Approximately(left.a, right.a);
    }

    private static string[] CopyLines(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            return System.Array.Empty<string>();
        }

        string[] copy = new string[lines.Length];
        for (int index = 0; index < lines.Length; index++)
        {
            copy[index] = lines[index];
        }

        return copy;
    }
}
