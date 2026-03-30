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
    private const int CurrentStyleVersion = 8;

    private enum BubbleDisplayMode
    {
        Default = 0,
        ReactionCue = 1
    }

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
    [SerializeField] private float tailHorizontalBias = 18f;
    [SerializeField] private float tailYOffset = -28f;
    [SerializeField] private Vector2 shadowOffset = new Vector2(3f, -5f);
    [SerializeField] private int sortingOrderOffset = 20;
    [SerializeField] private float minBubbleHeight = 1.24f;
    [SerializeField] private float bubbleGapAboveRenderer = 0.02f;
    [SerializeField] private float visibleFloatAmplitude = 0.004f;
    [SerializeField] private float visibleFloatFrequency = 0.8f;
    [SerializeField] private float tailBobAmplitude = 26f;
    [SerializeField] private float tailBobFrequency = 0.85f;

    [Header("气泡样式")]
    [SerializeField] private Color bubbleBorderColor = new Color(0.92f, 0.79f, 0.56f, 1f);
    [SerializeField] private Color bubbleColor = new Color(0.10f, 0.12f, 0.16f, 0.96f);
    [SerializeField] private Color bubbleShadowColor = new Color(0.01f, 0.02f, 0.04f, 0.34f);
    [SerializeField] private Color textColor = new Color(0.98f, 0.95f, 0.90f, 1f);
    [SerializeField] private Color textOutlineColor = new Color(0.05f, 0.06f, 0.09f, 0.96f);
    [SerializeField] private float fontSize = 32f;
    [SerializeField] private float textOutlineWidth = 0.18f;
    [SerializeField] private float showDuration = 0.14f;
    [SerializeField] private float hideDuration = 0.1f;
    [SerializeField] private float showScaleOvershoot = 0.05f;

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
    private float _lowestVisibleLocalY;
    private Vector2 _shadowTailBasePosition;
    private Vector2 _borderTailBasePosition;
    private Vector2 _fillTailBasePosition;
    private Vector3 _conversationLayoutShift;
    private bool _hasConversationLayoutShift;
    private BubbleDisplayMode _displayMode;

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
        ApplyTailBob();
        SyncSorting();
    }

    private void OnValidate()
    {
        CacheComponents();
        UpgradeLegacyStyleIfNeeded();

        maxTextWidth = Mathf.Max(80f, maxTextWidth);
        minAdaptiveTextWidth = Mathf.Clamp(minAdaptiveTextWidth, 40f, maxTextWidth);
        preferredCharactersPerLine = Mathf.Clamp(preferredCharactersPerLine, 4, 20);
        textSafePadding.x = Mathf.Max(4f, textSafePadding.x);
        textSafePadding.y = Mathf.Max(4f, textSafePadding.y);
        textVerticalOffset = Mathf.Clamp(textVerticalOffset, -24f, 12f);
        fontSize = Mathf.Max(10f, fontSize);
        borderThickness = Mathf.Max(2f, borderThickness);
        tailSize.x = Mathf.Max(12f, tailSize.x);
        tailSize.y = Mathf.Max(8f, tailSize.y);
        bubblePadding.x = Mathf.Max(16f, bubblePadding.x);
        bubblePadding.y = Mathf.Max(10f, bubblePadding.y);
        minBubbleHeight = Mathf.Max(0.8f, minBubbleHeight);
        bubbleGapAboveRenderer = Mathf.Max(0f, bubbleGapAboveRenderer);
        visibleFloatAmplitude = Mathf.Clamp(visibleFloatAmplitude, 0f, 0.12f);
        visibleFloatFrequency = Mathf.Clamp(visibleFloatFrequency, 0.1f, 6f);
        tailBobAmplitude = Mathf.Clamp(tailBobAmplitude, 0f, 48f);
        tailBobFrequency = Mathf.Clamp(tailBobFrequency, 0.1f, 8f);
        showDuration = Mathf.Max(0.01f, showDuration);
        hideDuration = Mathf.Max(0.01f, hideDuration);
        showScaleOvershoot = Mathf.Clamp(showScaleOvershoot, 0f, 0.2f);

        if (_canvas != null)
        {
            UpdateStyleVisuals();
            UpdateLayout();
            SyncCanvasTransform();
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

    public bool ShowText(string content, float duration = -1f, bool restartFadeIn = true)
    {
        return ShowTextInternal(content, duration, restartFadeIn, BubbleDisplayMode.Default);
    }

    public bool ShowReactionCue(string content, float duration = -1f, bool restartFadeIn = true)
    {
        return ShowTextInternal(content, duration, restartFadeIn, BubbleDisplayMode.ReactionCue);
    }

    private bool ShowTextInternal(
        string content,
        float duration,
        bool restartFadeIn,
        BubbleDisplayMode displayMode)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        _displayMode = displayMode;
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

        _bubbleText.text = FormatBubbleText(content.Trim());
        UpdateStyleVisuals();
        UpdateLayout();
        SyncCanvasTransform();

        if (restartFadeIn || _canvasGroup == null || _canvasGroup.alpha < 0.99f)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
            }

            if (_bubbleRoot != null)
            {
                _bubbleRoot.localScale = GetHiddenScale();
            }

            StartVisibilityAnimation(visible: true, deactivateAfter: false);
        }
        else
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }

            if (_bubbleRoot != null)
            {
                _bubbleRoot.localScale = Vector3.one;
            }
        }

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

    public void SetConversationLayoutShift(Vector3 localOffsetShift)
    {
        _conversationLayoutShift = localOffsetShift;
        _hasConversationLayoutShift = true;
        SyncCanvasTransform();
    }

    public void ClearConversationLayoutShift()
    {
        if (!_hasConversationLayoutShift && _conversationLayoutShift == Vector3.zero)
        {
            return;
        }

        _conversationLayoutShift = Vector3.zero;
        _hasConversationLayoutShift = false;
        SyncCanvasTransform();
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

        if ((styleVersion == 0 && LooksLikeLegacyStyle()) || styleVersion > 0)
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
        bubbleLocalOffset = new Vector3(0.08f, 1.46f, 0f);
        bubblePadding = new Vector2(82f, 42f);
        bubbleBorderColor = new Color(0.92f, 0.79f, 0.56f, 1f);
        bubbleColor = new Color(0.10f, 0.12f, 0.16f, 0.96f);
        bubbleShadowColor = new Color(0.01f, 0.02f, 0.04f, 0.34f);
        textColor = new Color(0.98f, 0.95f, 0.90f, 1f);
        textOutlineColor = new Color(0.05f, 0.06f, 0.09f, 0.96f);
        fontSize = 32f;
        textOutlineWidth = 0.18f;
        maxTextWidth = 315f;
        minAdaptiveTextWidth = 64f;
        preferredCharactersPerLine = 10;
        textSafePadding = new Vector2(24f, 22f);
        textVerticalOffset = -10f;
        borderThickness = 6f;
        tailSize = new Vector2(34f, 24f);
        tailHorizontalBias = 18f;
        tailYOffset = -28f;
        shadowOffset = new Vector2(3f, -5f);
        minBubbleHeight = 1.24f;
        bubbleGapAboveRenderer = 0.02f;
        visibleFloatAmplitude = 0.004f;
        visibleFloatFrequency = 0.8f;
        tailBobAmplitude = 26f;
        tailBobFrequency = 0.85f;
        showDuration = 0.14f;
        hideDuration = 0.1f;
        showScaleOvershoot = 0.05f;
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

        bool isReactionCue = _displayMode == BubbleDisplayMode.ReactionCue;
        Color resolvedShadowColor = isReactionCue
            ? new Color(0.02f, 0.01f, 0.03f, 0.24f)
            : bubbleShadowColor;
        Color resolvedBorderColor = isReactionCue
            ? new Color(1f, 0.84f, 0.58f, 1f)
            : bubbleBorderColor;
        Color resolvedBubbleColor = isReactionCue
            ? new Color(0.18f, 0.13f, 0.18f, 0.96f)
            : bubbleColor;
        Color resolvedTextColor = isReactionCue
            ? new Color(0.99f, 0.92f, 0.84f, 1f)
            : textColor;
        Color resolvedOutlineColor = isReactionCue
            ? new Color(0.17f, 0.10f, 0.08f, 0.94f)
            : textOutlineColor;

        _shadowBodyImage.color = resolvedShadowColor;
        _shadowTailImage.color = resolvedShadowColor;
        _borderBodyImage.color = resolvedBorderColor;
        _borderTailImage.color = resolvedBorderColor;
        _fillBodyImage.color = resolvedBubbleColor;
        _fillTailImage.color = resolvedBubbleColor;

        _bubbleText.fontSize = isReactionCue ? fontSize - 2f : fontSize;
        _bubbleText.color = resolvedTextColor;
        _bubbleText.outlineColor = resolvedOutlineColor;
        _bubbleText.outlineWidth = isReactionCue ? textOutlineWidth * 0.82f : textOutlineWidth;
    }

    private void UpdateLayout()
    {
        if (_bubbleText == null)
        {
            return;
        }

        string bubbleText = _bubbleText.text ?? string.Empty;
        AnalyzeBubbleText(bubbleText, out int visibleCharacterCount, out int longestLineCharacterCount);
        bool isReactionCue = _displayMode == BubbleDisplayMode.ReactionCue;
        Vector2 effectiveBubblePadding = isReactionCue ? new Vector2(44f, 18f) : bubblePadding;
        Vector2 effectiveTextSafePadding = isReactionCue ? new Vector2(18f, 14f) : textSafePadding;
        float effectiveFontSize = isReactionCue ? fontSize - 2f : fontSize;
        float effectiveBorderThickness = isReactionCue ? Mathf.Max(4f, borderThickness - 1f) : borderThickness;
        Vector2 effectiveTailSize = isReactionCue ? Vector2.zero : tailSize;
        float effectiveTailYOffset = isReactionCue ? 0f : tailYOffset;
        float effectiveTailHorizontalBias = isReactionCue ? 0f : tailHorizontalBias;
        Vector2 effectiveShadowOffset = isReactionCue ? new Vector2(2f, -3f) : shadowOffset;
        float effectiveTextVerticalOffset = isReactionCue ? -4f : textVerticalOffset;

        Vector2 preferredSize = _bubbleText.GetPreferredValues(bubbleText, 10000f, 0f);
        preferredSize.x = Mathf.Max(92f, preferredSize.x);
        preferredSize.y = Mathf.Max(effectiveFontSize + 16f, preferredSize.y);

        Vector2 textBoxSize = preferredSize + effectiveTextSafePadding;
        float shapeBias = Mathf.InverseLerp(preferredCharactersPerLine, preferredCharactersPerLine * 5f, visibleCharacterCount);
        float lineWidthBias = Mathf.InverseLerp(1f, preferredCharactersPerLine, longestLineCharacterCount);
        float horizontalPadding = Mathf.Lerp(isReactionCue ? 26f : 42f, effectiveBubblePadding.x, shapeBias);
        float verticalPadding = effectiveBubblePadding.y + Mathf.Lerp(0f, isReactionCue ? 6f : 12f, shapeBias);
        Vector2 bodySize = textBoxSize + new Vector2(horizontalPadding, verticalPadding) + new Vector2(lineWidthBias * 10f, shapeBias * 18f);
        Vector2 fillBodySize = new Vector2(
            Mathf.Max(60f, bodySize.x - (effectiveBorderThickness * 2f)),
            Mathf.Max(effectiveFontSize + 18f, bodySize.y - (effectiveBorderThickness * 2f)));
        Vector2 fillTailSize = new Vector2(
            Mathf.Max(0f, effectiveTailSize.x - (effectiveBorderThickness * 1.6f)),
            Mathf.Max(0f, effectiveTailSize.y - (effectiveBorderThickness * 1.2f)));
        Vector2 textRectSize = new Vector2(
            Mathf.Max(68f, fillBodySize.x - (effectiveTextSafePadding.x * 2f)),
            Mathf.Max(effectiveFontSize + 14f, fillBodySize.y - (effectiveTextSafePadding.y * 2f)));

        float bodyCenterY = isReactionCue ? 0f : Mathf.Max(effectiveTailSize.y * 0.72f, 10f);
        float tailCenterY = bodyCenterY - (bodySize.y * 0.54f) - (effectiveTailSize.y * 0.06f) + effectiveTailYOffset;

        Vector2 bodyPosition = new Vector2(0f, bodyCenterY);
        Vector2 tailPosition = new Vector2(effectiveTailHorizontalBias, tailCenterY);
        Vector2 shadowBodyPosition = bodyPosition + effectiveShadowOffset;
        Vector2 shadowTailPosition = tailPosition + effectiveShadowOffset;
        Vector2 fillTailPosition = tailPosition + (Vector2.up * (isReactionCue ? 0f : 0.75f));

        _shadowTailBasePosition = shadowTailPosition;
        _borderTailBasePosition = tailPosition;
        _fillTailBasePosition = fillTailPosition;

        SetRect(_shadowBodyRect, bodySize, shadowBodyPosition);
        SetRect(_shadowTailRect, effectiveTailSize, _shadowTailBasePosition);
        SetRect(_borderBodyRect, bodySize, bodyPosition);
        SetRect(_borderTailRect, effectiveTailSize, _borderTailBasePosition);
        SetRect(_fillBodyRect, fillBodySize, bodyPosition);
        SetRect(_fillTailRect, fillTailSize, _fillTailBasePosition);
        SetRect(_bubbleText.rectTransform, textRectSize, bodyPosition + (Vector2.up * effectiveTextVerticalOffset));

        _lowestVisibleLocalY = Mathf.Min(
            bodyPosition.y - (bodySize.y * 0.5f),
            tailPosition.y - (effectiveTailSize.y * 0.5f));

        float canvasWidth = bodySize.x + Mathf.Abs(shadowOffset.x) + 18f;
        float canvasHeight = bodySize.y + effectiveTailSize.y + Mathf.Abs(effectiveTailYOffset) + Mathf.Abs(effectiveShadowOffset.y) + 20f;
        Vector2 canvasSize = new Vector2(canvasWidth, canvasHeight);

        _canvasRect.sizeDelta = canvasSize;
        _bubbleRoot.sizeDelta = canvasSize;
        ApplyTailBob();
    }

    private void SyncCanvasTransform()
    {
        if (_canvas == null)
        {
            return;
        }

        Transform canvasTransform = _canvas.transform;
        canvasTransform.localPosition = GetResolvedBubbleLocalOffset();
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
            _bubbleRoot.localScale = GetHiddenScale();
        }

        if (_canvas != null)
        {
            _canvas.gameObject.SetActive(false);
        }

        _displayMode = BubbleDisplayMode.Default;
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
        Vector3 endScale = visible ? Vector3.one : GetHiddenScale();

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
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, eased);
            if (visible)
            {
                float overshootT = eased + (Mathf.Sin(t * Mathf.PI) * showScaleOvershoot);
                _bubbleRoot.localScale = Vector3.LerpUnclamped(startScale, endScale, overshootT);
            }
            else
            {
                _bubbleRoot.localScale = Vector3.Lerp(startScale, endScale, eased);
            }
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

    private Vector3 GetResolvedBubbleLocalOffset()
    {
        Vector3 resolvedOffset = bubbleLocalOffset;
        resolvedOffset.y = Mathf.Max(resolvedOffset.y, minBubbleHeight);

        if (targetRenderer != null)
        {
            Bounds rendererBounds = targetRenderer.bounds;
            Vector3 rendererTopLocal =
                transform.InverseTransformPoint(rendererBounds.center + (Vector3.up * rendererBounds.extents.y));
            float lowestVisibleWorldY = _lowestVisibleLocalY * bubbleLocalScale.y;
            resolvedOffset.y = Mathf.Max(
                resolvedOffset.y,
                rendererTopLocal.y + bubbleGapAboveRenderer - lowestVisibleWorldY);
        }

        if (Application.isPlaying && IsBubbleVisible)
        {
            float floatOffset =
                Mathf.Sin(Time.unscaledTime * visibleFloatFrequency * Mathf.PI * 2f) * visibleFloatAmplitude;
            resolvedOffset.y += floatOffset;
        }

        if (_displayMode == BubbleDisplayMode.ReactionCue)
        {
            resolvedOffset.y += 0.16f;
        }

        if (_hasConversationLayoutShift)
        {
            resolvedOffset += _conversationLayoutShift;
        }

        return resolvedOffset;
    }

    private static Vector3 GetHiddenScale()
    {
        return new Vector3(0.84f, 0.78f, 1f);
    }

    private void ApplyTailBob()
    {
        if (_shadowTailRect == null || _borderTailRect == null || _fillTailRect == null)
        {
            return;
        }

        if (_displayMode == BubbleDisplayMode.ReactionCue)
        {
            _shadowTailRect.anchoredPosition = _shadowTailBasePosition;
            _borderTailRect.anchoredPosition = _borderTailBasePosition;
            _fillTailRect.anchoredPosition = _fillTailBasePosition;
            return;
        }

        float tailBobOffset = 0f;
        if (Application.isPlaying && IsBubbleVisible)
        {
            float bobPhase = (Mathf.Sin(Time.unscaledTime * tailBobFrequency * Mathf.PI * 2f) * 0.5f) + 0.5f;
            tailBobOffset = bobPhase * tailBobAmplitude;
        }

        Vector2 bobOffset = Vector2.up * tailBobOffset;
        _shadowTailRect.anchoredPosition = _shadowTailBasePosition + bobOffset;
        _borderTailRect.anchoredPosition = _borderTailBasePosition + bobOffset;
        _fillTailRect.anchoredPosition = _fillTailBasePosition + bobOffset;
    }

    private string FormatBubbleText(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return string.Empty;
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder(content.Length + 8);
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
            float halfWidth = Mathf.SmoothStep(0f, (width - 1) * 0.5f, normalized);
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
