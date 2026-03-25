using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlayerThoughtBubblePresenter : MonoBehaviour
{
    private static readonly string[] PreferredFontResourcePaths =
    {
        "Fonts & Materials/DialogueChinese Pixel SDF",
        "Fonts & Materials/DialogueChinese SDF",
        "Fonts & Materials/DialogueChinese SoftPixel SDF"
    };

    private static Sprite sSolidSprite;
    private static Texture2D sSolidTexture;

    [Header("组件引用")]
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private TMP_FontAsset fontAsset;

    [Header("布局")]
    [SerializeField] private Vector3 bubbleLocalOffset = new Vector3(0f, 1.68f, 0f);
    [SerializeField] private Vector3 bubbleLocalScale = new Vector3(0.01f, 0.01f, 0.01f);
    [SerializeField] private Vector2 bubblePadding = new Vector2(54f, 30f);
    [SerializeField] private float maxTextWidth = 270f;
    [SerializeField] private float minBubbleHeight = 74f;
    [SerializeField] private float borderThickness = 4f;
    [SerializeField] private Vector2 tailSize = new Vector2(20f, 20f);
    [SerializeField] private float tailYOffset = -18f;
    [SerializeField] private int sortingOrderOffset = 30;

    [Header("样式")]
    [SerializeField] private Color bubbleShadowColor = new Color(0.03f, 0.05f, 0.08f, 0.26f);
    [SerializeField] private Color bubbleBorderColor = new Color(0.48f, 0.78f, 0.92f, 1f);
    [SerializeField] private Color bubbleFillColor = new Color(0.10f, 0.15f, 0.24f, 0.96f);
    [SerializeField] private Color textColor = new Color(0.95f, 0.98f, 1f, 1f);
    [SerializeField] private Color textOutlineColor = new Color(0.02f, 0.05f, 0.10f, 0.95f);
    [SerializeField] private float fontSize = 28f;
    [SerializeField] private float textOutlineWidth = 0.18f;
    [SerializeField] private float fadeDuration = 0.5f;

    private Canvas canvasRoot;
    private CanvasGroup canvasGroup;
    private RectTransform canvasRect;
    private RectTransform shadowBodyRect;
    private RectTransform shadowTailRect;
    private RectTransform borderBodyRect;
    private RectTransform borderTailRect;
    private RectTransform fillBodyRect;
    private RectTransform fillTailRect;
    private TextMeshProUGUI bubbleText;
    private Coroutine lifecycleCoroutine;

    public bool IsVisible => canvasRoot != null && canvasRoot.gameObject.activeSelf;
    public event System.Action Hidden;

    private void Awake()
    {
        CacheComponents();
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
        SyncSorting();
    }

    public void ShowText(string content, float totalDuration, bool restartFadeIn)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        EnsureUi();
        if (canvasRoot == null || bubbleText == null || canvasGroup == null)
        {
            return;
        }

        bubbleText.text = content.Trim();
        UpdateStyleVisuals();
        UpdateLayout();
        SyncCanvasTransform();
        SyncSorting();

        if (lifecycleCoroutine != null)
        {
            StopCoroutine(lifecycleCoroutine);
        }

        canvasRoot.gameObject.SetActive(true);
        lifecycleCoroutine = StartCoroutine(RunLifecycle(totalDuration, restartFadeIn));
    }

    public void HideImmediate()
    {
        if (lifecycleCoroutine != null)
        {
            StopCoroutine(lifecycleCoroutine);
            lifecycleCoroutine = null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (canvasRoot != null)
        {
            canvasRoot.gameObject.SetActive(false);
        }

        Hidden?.Invoke();
    }

    private IEnumerator RunLifecycle(float totalDuration, bool restartFadeIn)
    {
        float hideDuration = Mathf.Max(0.01f, fadeDuration);
        float showDuration = restartFadeIn ? hideDuration : 0f;
        float visibleDuration = Mathf.Max(0f, totalDuration - showDuration - hideDuration);

        if (restartFadeIn)
        {
            yield return FadeCanvas(0f, 1f, showDuration);
        }
        else
        {
            canvasGroup.alpha = 1f;
        }

        if (visibleDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(visibleDuration);
        }

        yield return FadeCanvas(canvasGroup.alpha, 0f, hideDuration);
        canvasRoot.gameObject.SetActive(false);
        lifecycleCoroutine = null;
        Hidden?.Invoke();
    }

    private IEnumerator FadeCanvas(float from, float to, float duration)
    {
        if (canvasGroup == null)
        {
            yield break;
        }

        if (duration <= 0.001f)
        {
            canvasGroup.alpha = to;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        canvasGroup.alpha = to;
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

    private void EnsureUi()
    {
        if (canvasRoot != null)
        {
            return;
        }

        CacheComponents();

        GameObject canvasGo = new GameObject("PlayerThoughtBubble");
        canvasGo.transform.SetParent(transform, false);

        canvasRect = canvasGo.AddComponent<RectTransform>();
        canvasRoot = canvasGo.AddComponent<Canvas>();
        canvasRoot.renderMode = RenderMode.WorldSpace;
        canvasRoot.overrideSorting = true;

        canvasGroup = canvasGo.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        CreateLayer(canvasGo.transform, "Shadow", bubbleShadowColor, out shadowBodyRect, out shadowTailRect, new Vector2(3f, -5f));
        CreateLayer(canvasGo.transform, "Border", bubbleBorderColor, out borderBodyRect, out borderTailRect, Vector2.zero);
        CreateLayer(canvasGo.transform, "Fill", bubbleFillColor, out fillBodyRect, out fillTailRect, Vector2.zero);

        GameObject textGo = new GameObject("Text");
        textGo.transform.SetParent(canvasGo.transform, false);
        RectTransform textRect = textGo.AddComponent<RectTransform>();
        bubbleText = textGo.AddComponent<TextMeshProUGUI>();
        bubbleText.raycastTarget = false;
        bubbleText.alignment = TextAlignmentOptions.Center;
        bubbleText.textWrappingMode = TextWrappingModes.Normal;
        bubbleText.overflowMode = TextOverflowModes.Overflow;
        bubbleText.font = fontAsset;
        bubbleText.fontSize = fontSize;
        bubbleText.color = textColor;
        bubbleText.outlineColor = textOutlineColor;
        bubbleText.outlineWidth = textOutlineWidth;
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);

        UpdateStyleVisuals();
        UpdateLayout();
        SyncCanvasTransform();
        SyncSorting();
    }

    private void CreateLayer(Transform parent, string layerName, Color color, out RectTransform bodyRect, out RectTransform tailRect, Vector2 bodyOffset)
    {
        GameObject bodyGo = new GameObject(layerName + "Body");
        bodyGo.transform.SetParent(parent, false);
        bodyRect = bodyGo.AddComponent<RectTransform>();
        Image bodyImage = bodyGo.AddComponent<Image>();
        bodyImage.sprite = GetSolidSprite();
        bodyImage.type = Image.Type.Sliced;
        bodyImage.color = color;
        bodyImage.raycastTarget = false;
        bodyRect.anchorMin = new Vector2(0.5f, 0.5f);
        bodyRect.anchorMax = new Vector2(0.5f, 0.5f);
        bodyRect.pivot = new Vector2(0.5f, 0.5f);
        bodyRect.anchoredPosition = bodyOffset;

        GameObject tailGo = new GameObject(layerName + "Tail");
        tailGo.transform.SetParent(parent, false);
        tailRect = tailGo.AddComponent<RectTransform>();
        Image tailImage = tailGo.AddComponent<Image>();
        tailImage.sprite = GetSolidSprite();
        tailImage.color = color;
        tailImage.raycastTarget = false;
        tailRect.anchorMin = new Vector2(0.5f, 0.5f);
        tailRect.anchorMax = new Vector2(0.5f, 0.5f);
        tailRect.pivot = new Vector2(0.5f, 0.5f);
        tailRect.localRotation = Quaternion.Euler(0f, 0f, 45f);
        tailRect.anchoredPosition = bodyOffset;
    }

    private void UpdateStyleVisuals()
    {
        if (bubbleText == null)
        {
            return;
        }

        bubbleText.font = fontAsset != null ? fontAsset : bubbleText.font;
        bubbleText.fontSize = fontSize;
        bubbleText.color = textColor;
        bubbleText.outlineColor = textOutlineColor;
        bubbleText.outlineWidth = textOutlineWidth;
    }

    private void UpdateLayout()
    {
        if (bubbleText == null || fillBodyRect == null || borderBodyRect == null || shadowBodyRect == null)
        {
            return;
        }

        Vector2 preferred = bubbleText.GetPreferredValues(bubbleText.text, maxTextWidth, 0f);
        float width = Mathf.Min(maxTextWidth, preferred.x) + bubblePadding.x;
        float height = Mathf.Max(minBubbleHeight, preferred.y + bubblePadding.y);
        Vector2 fillSize = new Vector2(width, height);
        Vector2 borderSize = fillSize + Vector2.one * borderThickness * 2f;
        Vector2 shadowSize = borderSize + Vector2.one * 2f;

        fillBodyRect.sizeDelta = fillSize;
        borderBodyRect.sizeDelta = borderSize;
        shadowBodyRect.sizeDelta = shadowSize;

        fillTailRect.sizeDelta = tailSize;
        borderTailRect.sizeDelta = tailSize + Vector2.one * borderThickness;
        shadowTailRect.sizeDelta = tailSize + Vector2.one * (borderThickness + 2f);

        fillBodyRect.anchoredPosition = Vector2.zero;
        borderBodyRect.anchoredPosition = Vector2.zero;
        shadowBodyRect.anchoredPosition = new Vector2(3f, -5f);

        Vector2 tailPos = new Vector2(0f, -height * 0.5f + tailYOffset);
        fillTailRect.anchoredPosition = tailPos;
        borderTailRect.anchoredPosition = tailPos;
        shadowTailRect.anchoredPosition = tailPos + new Vector2(3f, -5f);

        RectTransform textRect = bubbleText.rectTransform;
        textRect.sizeDelta = new Vector2(width - bubblePadding.x, height - bubblePadding.y);
        textRect.anchoredPosition = new Vector2(0f, 4f);
    }

    private void SyncCanvasTransform()
    {
        if (canvasRect == null)
        {
            return;
        }

        canvasRect.localPosition = bubbleLocalOffset;
        canvasRect.localScale = bubbleLocalScale;

        if (targetRenderer != null)
        {
            Bounds bounds = targetRenderer.bounds;
            Vector3 worldPos = bounds.center + new Vector3(0f, bounds.extents.y, 0f) + bubbleLocalOffset;
            canvasRect.position = worldPos;
        }
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

    private TMP_FontAsset TryLoadPreferredFontAsset()
    {
        foreach (string path in PreferredFontResourcePaths)
        {
            TMP_FontAsset asset = Resources.Load<TMP_FontAsset>(path);
            if (asset != null)
            {
                return asset;
            }
        }

        return TMP_Settings.defaultFontAsset;
    }

    private static Sprite GetSolidSprite()
    {
        if (sSolidSprite != null)
        {
            return sSolidSprite;
        }

        sSolidTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        sSolidTexture.SetPixel(0, 0, Color.white);
        sSolidTexture.Apply();
        sSolidTexture.wrapMode = TextureWrapMode.Clamp;
        sSolidSprite = Sprite.Create(sSolidTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 100f);
        return sSolidSprite;
    }
}
