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
    #region 常量

    private const string DefaultFontResourcePath = "Fonts & Materials/DialogueChinese SoftPixel SDF";

    #endregion

    #region 序列化字段

    [Header("组件引用")]
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private TMP_FontAsset fontAsset;

    [Header("气泡布局")]
    [SerializeField] private Vector3 bubbleLocalOffset = new Vector3(0f, 1.1f, 0f);
    [SerializeField] private Vector3 bubbleLocalScale = new Vector3(0.01f, 0.01f, 0.01f);
    [SerializeField] private Vector2 bubblePadding = new Vector2(24f, 16f);
    [SerializeField] private float maxTextWidth = 220f;
    [SerializeField] private int sortingOrderOffset = 20;

    [Header("气泡样式")]
    [SerializeField] private Color bubbleColor = new Color(0.13f, 0.13f, 0.13f, 0.92f);
    [SerializeField] private Color textColor = new Color(0.97f, 0.97f, 0.97f, 1f);
    [SerializeField] private float fontSize = 24f;

    [Header("默认文本池")]
    [SerializeField] private string[] selfTalkLines =
    {
        "先在这边看看。",
        "今天还挺安静。",
        "休息一下再走。",
        "嗯 这边风有点舒服。",
        "不知道大家在忙什么。"
    };

    [Header("调试")]
    [SerializeField] private bool showDebugLog = false;

    #endregion

    #region 私有字段

    private Canvas _canvas;
    private RectTransform _canvasRect;
    private RectTransform _panelRect;
    private Image _panelImage;
    private TextMeshProUGUI _bubbleText;
    private Coroutine _hideCoroutine;

    #endregion

    #region Unity生命周期

    private void Reset()
    {
        CacheComponents();
    }

    private void Awake()
    {
        CacheComponents();
        EnsureBubbleUi();
        HideImmediate();
    }

    private void LateUpdate()
    {
        if (_canvas == null)
        {
            return;
        }

        SyncSorting();
    }

    private void OnValidate()
    {
        maxTextWidth = Mathf.Max(60f, maxTextWidth);
        fontSize = Mathf.Max(8f, fontSize);
        bubblePadding.x = Mathf.Max(0f, bubblePadding.x);
        bubblePadding.y = Mathf.Max(0f, bubblePadding.y);

        if (!Application.isPlaying)
        {
            CacheComponents();
        }
    }

    private void OnDisable()
    {
        HideBubble();
    }

    #endregion

    #region 公共方法

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
        if (_canvas == null || _bubbleText == null || _panelRect == null)
        {
            return false;
        }

        _canvas.gameObject.SetActive(true);
        _bubbleText.text = content.Trim();
        _bubbleText.color = textColor;
        _bubbleText.fontSize = fontSize;
        _panelImage.color = bubbleColor;

        Vector2 preferredSize = _bubbleText.GetPreferredValues(_bubbleText.text, maxTextWidth, 0f);
        preferredSize.x = Mathf.Min(maxTextWidth, Mathf.Max(80f, preferredSize.x));
        preferredSize.y = Mathf.Max(fontSize + 8f, preferredSize.y);

        _bubbleText.rectTransform.sizeDelta = preferredSize;
        _panelRect.sizeDelta = preferredSize + bubblePadding;
        _canvasRect.sizeDelta = _panelRect.sizeDelta;

        SyncSorting();

        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
        }

        if (duration > 0f)
        {
            _hideCoroutine = StartCoroutine(HideAfterSeconds(duration));
        }
        else
        {
            _hideCoroutine = null;
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

        HideImmediate();
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

    #endregion

    #region 私有方法

    private void CacheComponents()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
        }

        if (fontAsset == null)
        {
            fontAsset = Resources.Load<TMP_FontAsset>(DefaultFontResourcePath);
        }
    }

    private void EnsureBubbleUi()
    {
        if (_canvas != null && _bubbleText != null && _panelRect != null)
        {
            return;
        }

        TMP_FontAsset resolvedFont = ResolveFontAsset();
        if (resolvedFont == null)
        {
            Debug.LogWarning($"[NPCBubblePresenter] {name} 未找到可用 TMP 字体资源，气泡将不会创建。", this);
            return;
        }

        GameObject canvasObject = new GameObject("NPCBubbleCanvas", typeof(RectTransform), typeof(Canvas));
        canvasObject.transform.SetParent(transform, false);
        canvasObject.transform.localPosition = bubbleLocalOffset;
        canvasObject.transform.localRotation = Quaternion.identity;
        canvasObject.transform.localScale = bubbleLocalScale;

        _canvas = canvasObject.GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;
        _canvas.overrideSorting = true;

        _canvasRect = canvasObject.GetComponent<RectTransform>();
        _canvasRect.pivot = new Vector2(0.5f, 0.5f);
        _canvasRect.sizeDelta = new Vector2(maxTextWidth + bubblePadding.x, fontSize + bubblePadding.y);

        GameObject panelObject = new GameObject("BubblePanel", typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(canvasObject.transform, false);

        _panelRect = panelObject.GetComponent<RectTransform>();
        _panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        _panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        _panelRect.pivot = new Vector2(0.5f, 0.5f);
        _panelRect.anchoredPosition = Vector2.zero;

        _panelImage = panelObject.GetComponent<Image>();
        _panelImage.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        _panelImage.type = Image.Type.Sliced;
        _panelImage.raycastTarget = false;
        _panelImage.color = bubbleColor;

        GameObject textObject = new GameObject("BubbleText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(panelObject.transform, false);

        _bubbleText = textObject.GetComponent<TextMeshProUGUI>();
        _bubbleText.font = resolvedFont;
        _bubbleText.fontSize = fontSize;
        _bubbleText.color = textColor;
        _bubbleText.alignment = TextAlignmentOptions.Center;
        _bubbleText.textWrappingMode = TextWrappingModes.Normal;
        _bubbleText.overflowMode = TextOverflowModes.Overflow;
        _bubbleText.raycastTarget = false;

        RectTransform textRect = _bubbleText.rectTransform;
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(maxTextWidth, fontSize + 10f);

        SyncSorting();
    }

    private TMP_FontAsset ResolveFontAsset()
    {
        if (fontAsset != null)
        {
            return fontAsset;
        }

        fontAsset = Resources.Load<TMP_FontAsset>(DefaultFontResourcePath);
        if (fontAsset != null)
        {
            return fontAsset;
        }

        return TMP_Settings.defaultFontAsset;
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
        if (_canvas != null)
        {
            _canvas.gameObject.SetActive(false);
        }
    }

    private IEnumerator HideAfterSeconds(float duration)
    {
        yield return new WaitForSeconds(Mathf.Max(0.1f, duration));
        _hideCoroutine = null;
        HideImmediate();
    }

    #endregion
}
