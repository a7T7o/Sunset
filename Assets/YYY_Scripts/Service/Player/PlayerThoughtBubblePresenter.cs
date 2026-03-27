using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlayerThoughtBubblePresenter : MonoBehaviour
{
    private static readonly Vector3 NpcBubbleLocalOffset = new Vector3(0f, 1.46f, 0f);
    private static readonly Vector3 NpcBubbleLocalScale = new Vector3(0.01f, 0.01f, 0.01f);
    private static readonly Vector2 NpcBubblePadding = new Vector2(82f, 42f);
    private static readonly Vector2 NpcTextSafePadding = new Vector2(24f, 22f);
    private static readonly Vector2 NpcTailSize = new Vector2(34f, 24f);
    private static readonly Vector2 NpcShadowOffset = new Vector2(3f, -5f);
    private static readonly Color PlayerBubbleBorderColor = new Color(0.35f, 0.46f, 0.42f, 1f);
    private static readonly Color PlayerBubbleFillColor = new Color(0.87f, 0.91f, 0.88f, 0.96f);
    private static readonly Color PlayerBubbleShadowColor = new Color(0.06f, 0.13f, 0.11f, 0.28f);
    private static readonly Color PlayerTextColor = new Color(0.12f, 0.16f, 0.14f, 1f);
    private static readonly Color PlayerTextOutlineColor = new Color(0.95f, 0.93f, 0.89f, 0.92f);

    private static readonly string[] PreferredFontResourcePaths =
    {
        "Fonts & Materials/DialogueChinese Pixel SDF",
        "Fonts & Materials/DialogueChinese SDF",
        "Fonts & Materials/DialogueChinese SoftPixel SDF"
    };

    private static Sprite sRuntimeBubbleSprite;
    private static Texture2D sRuntimeBubbleTexture;
    private static Sprite sRuntimeTailSprite;
    private static Texture2D sRuntimeTailTexture;

    [Header("组件引用")]
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private TMP_FontAsset fontAsset;

    [Header("气泡布局")]
    [SerializeField] private Vector3 bubbleLocalOffset = new Vector3(0f, 1.46f, 0f);
    [SerializeField] private Vector3 bubbleLocalScale = new Vector3(0.01f, 0.01f, 0.01f);
    [SerializeField] private Vector2 bubblePadding = new Vector2(82f, 42f);
    [SerializeField] private float maxTextWidth = 290f;
    [SerializeField] private float minAdaptiveTextWidth = 64f;
    [SerializeField] private int preferredCharactersPerLine = 10;
    [SerializeField] private Vector2 textSafePadding = new Vector2(24f, 22f);
    [SerializeField] private float textVerticalOffset = -10f;
    [SerializeField] private float borderThickness = 6f;
    [SerializeField] private Vector2 tailSize = new Vector2(34f, 24f);
    [SerializeField] private float tailYOffset = -28f;
    [SerializeField] private Vector2 shadowOffset = new Vector2(3f, -5f);
    [SerializeField] private int sortingOrderOffset = 20;
    [SerializeField] private float minBubbleHeight = 1.24f;
    [SerializeField] private float bubbleGapAboveRenderer = 0.02f;
    [SerializeField] private float visibleFloatAmplitude = 0.0034f;
    [SerializeField] private float visibleFloatFrequency = 0.8f;
    [SerializeField] private float tailBobAmplitude = 22f;
    [SerializeField] private float tailBobFrequency = 0.85f;

    [Header("气泡样式")]
    [SerializeField] private Color bubbleBorderColor = new Color(0.35f, 0.46f, 0.42f, 1f);
    [SerializeField] private Color bubbleFillColor = new Color(0.87f, 0.91f, 0.88f, 0.96f);
    [SerializeField] private Color bubbleShadowColor = new Color(0.06f, 0.13f, 0.11f, 0.28f);
    [SerializeField] private Color textColor = new Color(0.12f, 0.16f, 0.14f, 1f);
    [SerializeField] private Color textOutlineColor = new Color(0.95f, 0.93f, 0.89f, 0.92f);
    [SerializeField] private float fontSize = 32f;
    [SerializeField] private float textOutlineWidth = 0.18f;
    [SerializeField] private float showDuration = 0.14f;
    [SerializeField] private float hideDuration = 0.1f;
    [SerializeField] private float showScaleOvershoot = 0.05f;

    private Canvas canvasRoot;
    private CanvasGroup canvasGroup;
    private RectTransform canvasRect;
    private RectTransform bubbleRoot;
    private RectTransform shadowBodyRect;
    private RectTransform shadowTailRect;
    private RectTransform borderBodyRect;
    private RectTransform borderTailRect;
    private RectTransform fillBodyRect;
    private RectTransform fillTailRect;
    private Image shadowBodyImage;
    private Image shadowTailImage;
    private Image borderBodyImage;
    private Image borderTailImage;
    private Image fillBodyImage;
    private Image fillTailImage;
    private TextMeshProUGUI bubbleText;
    private Coroutine hideCoroutine;
    private Coroutine visibilityCoroutine;
    private float lowestVisibleLocalY;
    private Vector2 shadowTailBasePosition;
    private Vector2 borderTailBasePosition;
    private Vector2 fillTailBasePosition;

    public bool IsVisible => canvasRoot != null && canvasRoot.gameObject.activeSelf;
    public event Action Hidden;

    private void Reset()
    {
        CacheComponents();
        ApplyPlayerBubbleStylePreset();
    }

    private void Awake()
    {
        CacheComponents();
        ApplyPlayerBubbleStylePreset();
        EnsureUi();
        HideImmediate();
    }

    private void LateUpdate()
    {
        if (canvasRoot == null)
        {
            return;
        }

        SyncCanvasTransform();
        ApplyTailBob();
        SyncSorting();
    }

    private void OnDisable()
    {
        HideImmediate();
    }

    private void OnValidate()
    {
        CacheComponents();
        ApplyPlayerBubbleStylePreset();
    }

    public void ShowText(string content, float totalDuration, bool restartFadeIn)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        EnsureUi();
        if (canvasRoot == null || bubbleText == null)
        {
            return;
        }

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        if (visibilityCoroutine != null)
        {
            StopCoroutine(visibilityCoroutine);
            visibilityCoroutine = null;
        }

        canvasRoot.gameObject.SetActive(true);
        bubbleText.text = FormatBubbleText(content.Trim());
        UpdateStyleVisuals();
        UpdateLayout();
        SyncCanvasTransform();
        SyncSorting();

        if (restartFadeIn || canvasGroup == null || canvasGroup.alpha < 0.99f)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            if (bubbleRoot != null)
            {
                bubbleRoot.localScale = GetHiddenScale();
            }

            StartVisibilityAnimation(visible: true, deactivateAfter: false);
        }
        else
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            if (bubbleRoot != null)
            {
                bubbleRoot.localScale = Vector3.one;
            }
        }

        if (totalDuration > 0f)
        {
            hideCoroutine = StartCoroutine(HideAfterSeconds(totalDuration));
        }
    }

    public void HideImmediate()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        if (visibilityCoroutine != null)
        {
            StopCoroutine(visibilityCoroutine);
            visibilityCoroutine = null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (bubbleRoot != null)
        {
            bubbleRoot.localScale = GetHiddenScale();
        }

        if (canvasRoot != null)
        {
            canvasRoot.gameObject.SetActive(false);
        }

        Hidden?.Invoke();
    }

    private void HideBubble()
    {
        if (!Application.isPlaying || canvasRoot == null || !canvasRoot.gameObject.activeSelf)
        {
            HideImmediate();
            return;
        }

        StartVisibilityAnimation(visible: false, deactivateAfter: true);
    }

    private void CacheComponents()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (fontAsset == null)
        {
            fontAsset = TryLoadPreferredFontAsset();
        }
    }

    private void ApplyPlayerBubbleStylePreset()
    {
        bubbleLocalOffset = NpcBubbleLocalOffset;
        bubbleLocalScale = NpcBubbleLocalScale;
        bubblePadding = NpcBubblePadding;
        maxTextWidth = 290f;
        minAdaptiveTextWidth = 64f;
        preferredCharactersPerLine = 10;
        textSafePadding = NpcTextSafePadding;
        textVerticalOffset = -10f;
        borderThickness = 6f;
        tailSize = NpcTailSize;
        tailYOffset = -28f;
        shadowOffset = NpcShadowOffset;
        sortingOrderOffset = 20;
        minBubbleHeight = 1.24f;
        bubbleGapAboveRenderer = 0.02f;
        visibleFloatAmplitude = 0.0034f;
        visibleFloatFrequency = 0.8f;
        tailBobAmplitude = 22f;
        tailBobFrequency = 0.85f;
        bubbleBorderColor = PlayerBubbleBorderColor;
        bubbleFillColor = PlayerBubbleFillColor;
        bubbleShadowColor = PlayerBubbleShadowColor;
        textColor = PlayerTextColor;
        textOutlineColor = PlayerTextOutlineColor;
        fontSize = 32f;
        textOutlineWidth = 0.18f;
        showDuration = 0.14f;
        hideDuration = 0.1f;
        showScaleOvershoot = 0.05f;
    }

    private void EnsureUi()
    {
        if (canvasRoot != null && bubbleText != null)
        {
            return;
        }

        TMP_FontAsset resolvedFont = ResolveFontAsset();
        if (resolvedFont == null)
        {
            Debug.LogWarning($"[PlayerThoughtBubblePresenter] {name} 未找到可用 TMP 字体资源，气泡将不会创建。", this);
            return;
        }

        GameObject canvasObject = new GameObject(
            "PlayerThoughtBubble",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasGroup));
        canvasObject.transform.SetParent(transform, false);

        canvasRoot = canvasObject.GetComponent<Canvas>();
        canvasRoot.renderMode = RenderMode.WorldSpace;
        canvasRoot.overrideSorting = true;

        canvasGroup = canvasObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasRect = canvasObject.GetComponent<RectTransform>();
        canvasRect.pivot = new Vector2(0.5f, 0.5f);
        canvasRect.anchorMin = new Vector2(0.5f, 0.5f);
        canvasRect.anchorMax = new Vector2(0.5f, 0.5f);

        GameObject rootObject = new GameObject("BubbleRoot", typeof(RectTransform));
        rootObject.transform.SetParent(canvasObject.transform, false);
        bubbleRoot = rootObject.GetComponent<RectTransform>();
        bubbleRoot.anchorMin = new Vector2(0.5f, 0.5f);
        bubbleRoot.anchorMax = new Vector2(0.5f, 0.5f);
        bubbleRoot.pivot = new Vector2(0.5f, 0.5f);

        shadowBodyImage = CreateImage(
            bubbleRoot,
            "ShadowBody",
            GetOrCreateRuntimeBubbleSprite(),
            Image.Type.Sliced,
            bubbleShadowColor,
            out shadowBodyRect);

        shadowTailImage = CreateImage(
            bubbleRoot,
            "ShadowTail",
            GetOrCreateRuntimeTailSprite(),
            Image.Type.Simple,
            bubbleShadowColor,
            out shadowTailRect);

        borderBodyImage = CreateImage(
            bubbleRoot,
            "BorderBody",
            GetOrCreateRuntimeBubbleSprite(),
            Image.Type.Sliced,
            bubbleBorderColor,
            out borderBodyRect);

        borderTailImage = CreateImage(
            bubbleRoot,
            "BorderTail",
            GetOrCreateRuntimeTailSprite(),
            Image.Type.Simple,
            bubbleBorderColor,
            out borderTailRect);

        fillBodyImage = CreateImage(
            bubbleRoot,
            "FillBody",
            GetOrCreateRuntimeBubbleSprite(),
            Image.Type.Sliced,
            bubbleFillColor,
            out fillBodyRect);

        fillTailImage = CreateImage(
            bubbleRoot,
            "FillTail",
            GetOrCreateRuntimeTailSprite(),
            Image.Type.Simple,
            bubbleFillColor,
            out fillTailRect);

        GameObject textObject = new GameObject("BubbleText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(fillBodyRect, false);

        bubbleText = textObject.GetComponent<TextMeshProUGUI>();
        bubbleText.font = resolvedFont;
        bubbleText.alignment = TextAlignmentOptions.Center;
        bubbleText.textWrappingMode = TextWrappingModes.Normal;
        bubbleText.overflowMode = TextOverflowModes.Overflow;
        bubbleText.raycastTarget = false;
        bubbleText.enableAutoSizing = false;
        bubbleText.extraPadding = true;
        bubbleText.characterSpacing = 1.25f;
        bubbleText.lineSpacing = -5f;
        bubbleText.outlineColor = textOutlineColor;
        bubbleText.outlineWidth = textOutlineWidth;

        RectTransform textRect = bubbleText.rectTransform;
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
        if (bubbleText == null)
        {
            return;
        }

        shadowBodyImage.color = bubbleShadowColor;
        shadowTailImage.color = bubbleShadowColor;
        borderBodyImage.color = bubbleBorderColor;
        borderTailImage.color = bubbleBorderColor;
        fillBodyImage.color = bubbleFillColor;
        fillTailImage.color = bubbleFillColor;

        bubbleText.fontSize = fontSize;
        bubbleText.color = textColor;
        bubbleText.outlineColor = textOutlineColor;
        bubbleText.outlineWidth = textOutlineWidth;
    }

    private void UpdateLayout()
    {
        if (bubbleText == null || canvasRect == null || bubbleRoot == null)
        {
            return;
        }

        string content = bubbleText.text ?? string.Empty;
        AnalyzeBubbleText(content, out int visibleCharacterCount, out int longestLineCharacterCount);

        float constrainedTextWidth = EstimateAdaptiveTextWidth(longestLineCharacterCount);
        Vector2 preferredSize = bubbleText.GetPreferredValues(content, constrainedTextWidth, 0f);
        preferredSize.x = Mathf.Max(minAdaptiveTextWidth, preferredSize.x);
        preferredSize.y = Mathf.Max(fontSize + 16f, preferredSize.y);

        Vector2 textBoxSize = preferredSize + textSafePadding;
        float shapeBias = Mathf.InverseLerp(preferredCharactersPerLine, preferredCharactersPerLine * 5f, visibleCharacterCount);
        float lineWidthBias = Mathf.InverseLerp(1f, preferredCharactersPerLine, longestLineCharacterCount);
        float horizontalPadding = Mathf.Lerp(42f, bubblePadding.x, shapeBias);
        float verticalPadding = bubblePadding.y + Mathf.Lerp(0f, 12f, shapeBias);
        Vector2 bodySize = textBoxSize + new Vector2(horizontalPadding, verticalPadding) + new Vector2(lineWidthBias * 10f, shapeBias * 18f);
        Vector2 fillBodySize = new Vector2(
            Mathf.Max(60f, bodySize.x - (borderThickness * 2f)),
            Mathf.Max(fontSize + 18f, bodySize.y - (borderThickness * 2f)));
        Vector2 fillTailSize = new Vector2(
            Mathf.Max(8f, tailSize.x - (borderThickness * 1.6f)),
            Mathf.Max(6f, tailSize.y - (borderThickness * 1.2f)));
        Vector2 textRectSize = new Vector2(
            Mathf.Max(68f, fillBodySize.x - (textSafePadding.x * 2f)),
            Mathf.Max(fontSize + 14f, fillBodySize.y - (textSafePadding.y * 2f)));

        float bodyCenterY = Mathf.Max(tailSize.y * 0.72f, 10f);
        float tailCenterY = bodyCenterY - (bodySize.y * 0.54f) - (tailSize.y * 0.06f) + tailYOffset;

        Vector2 bodyPosition = new Vector2(0f, bodyCenterY);
        Vector2 tailPosition = new Vector2(0f, tailCenterY);
        Vector2 shadowBodyPosition = bodyPosition + shadowOffset;
        Vector2 shadowTailPosition = tailPosition + shadowOffset;
        Vector2 fillTailPosition = tailPosition + (Vector2.up * 0.75f);

        shadowTailBasePosition = shadowTailPosition;
        borderTailBasePosition = tailPosition;
        fillTailBasePosition = fillTailPosition;

        SetRect(shadowBodyRect, bodySize, shadowBodyPosition);
        SetRect(shadowTailRect, tailSize, shadowTailBasePosition);
        SetRect(borderBodyRect, bodySize, bodyPosition);
        SetRect(borderTailRect, tailSize, borderTailBasePosition);
        SetRect(fillBodyRect, fillBodySize, bodyPosition);
        SetRect(fillTailRect, fillTailSize, fillTailBasePosition);
        SetRect(bubbleText.rectTransform, textRectSize, bodyPosition + (Vector2.up * textVerticalOffset));

        lowestVisibleLocalY = Mathf.Min(
            bodyPosition.y - (bodySize.y * 0.5f),
            tailPosition.y - (tailSize.y * 0.5f));

        float canvasWidth = bodySize.x + Mathf.Abs(shadowOffset.x) + 18f;
        float canvasHeight = bodySize.y + tailSize.y + Mathf.Abs(tailYOffset) + Mathf.Abs(shadowOffset.y) + 20f;
        Vector2 canvasSize = new Vector2(canvasWidth, canvasHeight);

        canvasRect.sizeDelta = canvasSize;
        bubbleRoot.sizeDelta = canvasSize;
        ApplyTailBob();
    }

    private void SyncCanvasTransform()
    {
        if (canvasRoot == null)
        {
            return;
        }

        Transform canvasTransform = canvasRoot.transform;
        canvasTransform.localPosition = GetResolvedBubbleLocalOffset();
        canvasTransform.localRotation = Quaternion.identity;
        canvasTransform.localScale = bubbleLocalScale;
    }

    private void SyncSorting()
    {
        if (canvasRoot == null)
        {
            return;
        }

        if (targetRenderer != null)
        {
            canvasRoot.sortingLayerID = targetRenderer.sortingLayerID;
            canvasRoot.sortingOrder = targetRenderer.sortingOrder + sortingOrderOffset;
        }
        else
        {
            canvasRoot.sortingLayerName = "Default";
            canvasRoot.sortingOrder = sortingOrderOffset;
        }
    }

    private void StartVisibilityAnimation(bool visible, bool deactivateAfter)
    {
        if (canvasRoot == null || canvasGroup == null || bubbleRoot == null)
        {
            return;
        }

        if (visibilityCoroutine != null)
        {
            StopCoroutine(visibilityCoroutine);
        }

        visibilityCoroutine = StartCoroutine(AnimateVisibility(visible, deactivateAfter));
    }

    private IEnumerator AnimateVisibility(bool visible, bool deactivateAfter)
    {
        float duration = visible ? showDuration : hideDuration;
        float startAlpha = canvasGroup.alpha;
        float endAlpha = visible ? 1f : 0f;
        Vector3 startScale = bubbleRoot.localScale;
        Vector3 endScale = visible ? Vector3.one : GetHiddenScale();

        if (duration <= 0.001f)
        {
            canvasGroup.alpha = endAlpha;
            bubbleRoot.localScale = endScale;
            visibilityCoroutine = null;

            if (!visible && deactivateAfter)
            {
                canvasRoot.gameObject.SetActive(false);
                Hidden?.Invoke();
            }

            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, eased);
            if (visible)
            {
                float overshootT = eased + (Mathf.Sin(t * Mathf.PI) * showScaleOvershoot);
                bubbleRoot.localScale = Vector3.LerpUnclamped(startScale, endScale, overshootT);
            }
            else
            {
                bubbleRoot.localScale = Vector3.Lerp(startScale, endScale, eased);
            }

            yield return null;
        }

        canvasGroup.alpha = endAlpha;
        bubbleRoot.localScale = endScale;
        visibilityCoroutine = null;

        if (!visible && deactivateAfter)
        {
            canvasRoot.gameObject.SetActive(false);
            Hidden?.Invoke();
        }
    }

    private IEnumerator HideAfterSeconds(float duration)
    {
        yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, duration));
        hideCoroutine = null;
        HideBubble();
    }

    private Vector3 GetResolvedBubbleLocalOffset()
    {
        Vector3 resolvedOffset = bubbleLocalOffset;
        resolvedOffset.y = Mathf.Max(resolvedOffset.y, minBubbleHeight);

        if (targetRenderer != null)
        {
            Bounds rendererBounds = targetRenderer.bounds;
            Vector3 rendererTopLocal =
                transform.InverseTransformPoint(rendererBounds.center + (Vector3.up * rendererBounds.extents.y));
            float lowestVisibleWorldY = lowestVisibleLocalY * bubbleLocalScale.y;
            resolvedOffset.y = Mathf.Max(
                resolvedOffset.y,
                rendererTopLocal.y + bubbleGapAboveRenderer - lowestVisibleWorldY);
        }

        if (Application.isPlaying && IsVisible)
        {
            float floatOffset =
                Mathf.Sin(Time.unscaledTime * visibleFloatFrequency * Mathf.PI * 2f) * visibleFloatAmplitude;
            resolvedOffset.y += floatOffset;
        }

        return resolvedOffset;
    }

    private void ApplyTailBob()
    {
        if (shadowTailRect == null || borderTailRect == null || fillTailRect == null)
        {
            return;
        }

        float tailBobOffset = 0f;
        if (Application.isPlaying && IsVisible)
        {
            float bobPhase = (Mathf.Sin(Time.unscaledTime * tailBobFrequency * Mathf.PI * 2f) * 0.5f) + 0.5f;
            tailBobOffset = bobPhase * tailBobAmplitude;
        }

        Vector2 bobOffset = Vector2.up * tailBobOffset;
        shadowTailRect.anchoredPosition = shadowTailBasePosition + bobOffset;
        borderTailRect.anchoredPosition = borderTailBasePosition + bobOffset;
        fillTailRect.anchoredPosition = fillTailBasePosition + bobOffset;
    }

    private float EstimateAdaptiveTextWidth(int longestLineCharacterCount)
    {
        if (longestLineCharacterCount <= 0)
        {
            return minAdaptiveTextWidth;
        }

        float widthRatio = Mathf.InverseLerp(1f, preferredCharactersPerLine, longestLineCharacterCount);
        return Mathf.Lerp(minAdaptiveTextWidth, maxTextWidth, widthRatio);
    }

    private string FormatBubbleText(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder(content.Length + 8);
        int lineCharacterCount = 0;

        for (int index = 0; index < content.Length; index++)
        {
            char current = content[index];
            if (current == '\r')
            {
                continue;
            }

            if (current == '\n')
            {
                if (builder.Length > 0 && builder[builder.Length - 1] != '\n')
                {
                    builder.Append('\n');
                }

                lineCharacterCount = 0;
                continue;
            }

            if (lineCharacterCount >= preferredCharactersPerLine)
            {
                builder.Append('\n');
                lineCharacterCount = 0;
            }

            builder.Append(current);
            if (!char.IsWhiteSpace(current))
            {
                lineCharacterCount++;
            }
        }

        return builder.ToString();
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

    private static void AnalyzeBubbleText(string content, out int visibleCharacterCount, out int longestLineCharacterCount)
    {
        visibleCharacterCount = 0;
        longestLineCharacterCount = 0;

        if (string.IsNullOrEmpty(content))
        {
            return;
        }

        int currentLineCharacterCount = 0;
        for (int index = 0; index < content.Length; index++)
        {
            char current = content[index];
            if (current == '\r')
            {
                continue;
            }

            if (current == '\n')
            {
                longestLineCharacterCount = Mathf.Max(longestLineCharacterCount, currentLineCharacterCount);
                currentLineCharacterCount = 0;
                continue;
            }

            if (!char.IsWhiteSpace(current))
            {
                visibleCharacterCount++;
                currentLineCharacterCount++;
            }
        }

        longestLineCharacterCount = Mathf.Max(longestLineCharacterCount, currentLineCharacterCount);
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
        if (sRuntimeBubbleSprite != null)
        {
            return sRuntimeBubbleSprite;
        }

        const int width = 32;
        const int height = 24;
        const int radius = 6;

        sRuntimeBubbleTexture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false)
        {
            name = "PlayerBubbleRuntimeTexture",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool inside = IsInsideRoundedRect(x, y, width, height, radius);
                sRuntimeBubbleTexture.SetPixel(x, y, inside ? Color.white : Color.clear);
            }
        }

        sRuntimeBubbleTexture.Apply(updateMipmaps: false, makeNoLongerReadable: true);

        sRuntimeBubbleSprite = Sprite.Create(
            sRuntimeBubbleTexture,
            new Rect(0f, 0f, width, height),
            new Vector2(0.5f, 0.5f),
            16f,
            0u,
            SpriteMeshType.FullRect,
            new Vector4(radius, radius, radius, radius));
        sRuntimeBubbleSprite.name = "PlayerBubbleRuntimeSprite";
        sRuntimeBubbleSprite.hideFlags = HideFlags.HideAndDontSave;
        return sRuntimeBubbleSprite;
    }

    private static Sprite GetOrCreateRuntimeTailSprite()
    {
        if (sRuntimeTailSprite != null)
        {
            return sRuntimeTailSprite;
        }

        const int width = 18;
        const int height = 12;

        sRuntimeTailTexture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false)
        {
            name = "PlayerBubbleTailRuntimeTexture",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };

        float centerX = (width - 1) * 0.5f;
        for (int y = 0; y < height; y++)
        {
            float normalized = y / Mathf.Max(1f, height - 1f);
            float halfWidth = Mathf.SmoothStep(0f, (width - 1) * 0.5f, normalized);
            for (int x = 0; x < width; x++)
            {
                bool inside = Mathf.Abs(x - centerX) <= halfWidth;
                sRuntimeTailTexture.SetPixel(x, y, inside ? Color.white : Color.clear);
            }
        }

        sRuntimeTailTexture.Apply(updateMipmaps: false, makeNoLongerReadable: true);

        sRuntimeTailSprite = Sprite.Create(
            sRuntimeTailTexture,
            new Rect(0f, 0f, width, height),
            new Vector2(0.5f, 1f),
            16f);
        sRuntimeTailSprite.name = "PlayerBubbleTailRuntimeSprite";
        sRuntimeTailSprite.hideFlags = HideFlags.HideAndDontSave;
        return sRuntimeTailSprite;
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

    private static Vector3 GetHiddenScale()
    {
        return new Vector3(0.84f, 0.78f, 1f);
    }
}
