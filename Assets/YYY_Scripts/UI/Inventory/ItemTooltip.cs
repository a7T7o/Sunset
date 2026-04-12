using System.Collections;
using FarmGame.Data;
using FarmGame.Data.Core;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemTooltip : MonoBehaviour
{
    private const float MinimumShowDelay = 1f;
    private const float FadeDurationSeconds = 0.3f;
    private const float TooltipWidth = 184f;
    private const float TooltipMinHeight = 74f;
    private const float PanelBorder = 3f;
    private const float ContentWidth = 156f;
    private const float ContentPaddingX = 10f;
    private const float ContentPaddingTop = 10f;
    private const float ContentPaddingBottom = 9f;

    private static readonly Color FrameColor = new Color(0.28f, 0.18f, 0.10f, 0.98f);
    private static readonly Color PanelColor = new Color(0.95f, 0.91f, 0.80f, 0.99f);
    private static readonly Color HeaderColor = new Color(0.42f, 0.29f, 0.15f, 1f);
    private static readonly Color DividerColor = new Color(0.55f, 0.40f, 0.20f, 0.95f);
    private static readonly Color TitleBaseColor = new Color(0.99f, 0.97f, 0.92f, 1f);
    private static readonly Color DescriptionColor = new Color(0.18f, 0.11f, 0.04f, 1f);
    private static readonly Color StatusColor = new Color(0.24f, 0.18f, 0.07f, 1f);
    private static readonly Color PriceColor = new Color(0.27f, 0.18f, 0.04f, 1f);

    private static ItemTooltip _instance;

    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private RectTransform headerRoot;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text statusText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text priceText;
    [SerializeField] private Image qualityIcon;
    [SerializeField] private Image frameImage;
    [SerializeField] private Image panelImage;
    [SerializeField] private Image headerImage;
    [SerializeField] private Image dividerImage;
    [SerializeField] private Vector2 offset = new Vector2(18f, -18f);
    [SerializeField] private float showDelay = 1f;

    private struct VisualData
    {
        public string Title;
        public Color TitleColor;
        public string Status;
        public string Description;
        public string Price;
        public bool ShowStatus;
        public bool ShowDescription;
        public bool ShowPrice;
        public bool ShowQualityIcon;
        public Color QualityColor;
    }

    private Canvas _parentCanvas;
    private Camera _uiCamera;
    private ItemDatabase _database;
    private RectTransform _sourceRect;
    private RectTransform _movementRect;
    private Transform _currentSourceTransform;
    private bool _hasSourceConstraint;
    private bool _isShowing;
    private Coroutine _pendingShowCoroutine;
    private Coroutine _fadeCoroutine;
    private VisualData _pendingVisual;

    public static ItemTooltip Instance => EnsureInstance();

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        InitializeRuntimeReferences();
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    void Update()
    {
        if ((_isShowing || _pendingShowCoroutine != null) && (ShouldSuppressBecauseOfInteraction() || !IsPointerWithinSourceRect()))
        {
            Hide();
            return;
        }

        if (_isShowing && gameObject.activeSelf && canvasGroup != null && canvasGroup.alpha > 0.001f)
        {
            FollowMouse();
        }
    }

    public void SetDatabase(ItemDatabase database)
    {
        _database = database;
    }

    public void Show(ItemStack item, int amount = 1) => Show(item, null, amount, null);
    public void Show(ItemStack item, InventoryItem runtimeItem, int amount = 1) => Show(item, runtimeItem, amount, null);

    public void Show(ItemStack item, InventoryItem runtimeItem, int amount, Transform sourceTransform)
    {
        if (item.IsEmpty)
        {
            Hide();
            return;
        }

        EnsureDatabaseReference();
        if (_database == null)
        {
            Hide();
            return;
        }

        Show(_database.GetItemByID(item.itemId), item, runtimeItem, amount, sourceTransform);
    }

    public void Show(ItemData itemData, ItemStack item, InventoryItem runtimeItem = null, int amount = 1) =>
        Show(itemData, item, runtimeItem, amount, null);

    public void Show(ItemData itemData, ItemStack item, InventoryItem runtimeItem, int amount, Transform sourceTransform)
    {
        if (itemData == null || item.IsEmpty)
        {
            Hide();
            return;
        }

        EnsureAttachedToPreferredCanvas(sourceTransform);

        string status = ItemTooltipTextBuilder.BuildRuntimeStatusText(itemData, runtimeItem);
        string description = ItemTooltipTextBuilder.BuildDescriptionText(itemData);
        int totalPrice = itemData.GetSellPriceWithQuality((ItemQuality)item.quality) * amount;

        QueueShow(new VisualData
        {
            Title = itemData.itemName,
            TitleColor = GetQualityColor((ItemQuality)item.quality),
            Status = status,
            Description = description,
            Price = amount > 1 ? $"总价值  {totalPrice}G  ({amount})" : $"价值  {totalPrice}G",
            ShowStatus = !string.IsNullOrWhiteSpace(status),
            ShowDescription = !string.IsNullOrWhiteSpace(description),
            ShowPrice = totalPrice > 0,
            ShowQualityIcon = qualityIcon != null && item.quality > 0,
            QualityColor = itemData.GetQualityStarColor((ItemQuality)item.quality)
        }, sourceTransform);
    }

    public void ShowCustom(string title, string body) => ShowCustom(title, body, null);

    public void ShowCustom(string title, string body, Transform sourceTransform)
    {
        EnsureAttachedToPreferredCanvas(sourceTransform);
        QueueShow(new VisualData
        {
            Title = title,
            TitleColor = TitleBaseColor,
            Status = string.Empty,
            Description = body,
            Price = string.Empty,
            ShowStatus = false,
            ShowDescription = !string.IsNullOrWhiteSpace(body),
            ShowPrice = false,
            ShowQualityIcon = false,
            QualityColor = Color.white
        }, sourceTransform);
    }

    public void Hide()
    {
        if (_pendingShowCoroutine != null)
        {
            StopCoroutine(_pendingShowCoroutine);
            _pendingShowCoroutine = null;
        }

        _isShowing = false;
        _currentSourceTransform = null;
        _sourceRect = null;
        _movementRect = null;
        _hasSourceConstraint = false;

        if (!gameObject.activeSelf)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
            return;
        }

        StartFade(0f, true);
    }

    public void FollowMouse()
    {
        if (tooltipRect == null || _parentCanvas == null)
        {
            return;
        }

        RectTransform canvasRect = _parentCanvas.transform as RectTransform;
        if (canvasRect == null)
        {
            return;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            ClampScreenPositionToCanvas(Input.mousePosition),
            _uiCamera,
            out Vector2 localPoint);

        Rect bounds = TryGetBoundsRect(_movementRect, canvasRect, out Rect movementBounds) ? movementBounds : canvasRect.rect;
        localPoint.x = Mathf.Clamp(localPoint.x, bounds.xMin, bounds.xMax);
        localPoint.y = Mathf.Clamp(localPoint.y, bounds.yMin, bounds.yMax);

        float halfWidth = tooltipRect.rect.width * 0.5f;
        float halfHeight = tooltipRect.rect.height * 0.5f;
        Vector2 target = new Vector2(localPoint.x + offset.x, localPoint.y + offset.y);

        if (target.x + halfWidth > bounds.xMax)
        {
            target.x = localPoint.x - offset.x;
        }

        if (target.y - halfHeight < bounds.yMin)
        {
            target.y = localPoint.y - offset.y;
        }

        target.x = Mathf.Clamp(target.x, bounds.xMin + halfWidth, bounds.xMax - halfWidth);
        target.y = Mathf.Clamp(target.y, bounds.yMin + halfHeight, bounds.yMax - halfHeight);
        tooltipRect.anchoredPosition = target;
    }

    public static ItemTooltip EnsureInstance()
    {
        if (_instance != null)
        {
            return _instance;
        }

        _instance = FindFirstObjectByType<ItemTooltip>(FindObjectsInactive.Include);
        if (_instance != null)
        {
            _instance.InitializeRuntimeReferences();
            return _instance;
        }

        Canvas parentCanvas = ResolvePreferredParentCanvas(null);
        if (parentCanvas == null)
        {
            return null;
        }

        GameObject root = new GameObject("RuntimeItemTooltip", typeof(RectTransform), typeof(CanvasGroup));
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.SetParent(parentCanvas.transform, false);
        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);

        _instance = root.AddComponent<ItemTooltip>();
        _instance.tooltipRect = rootRect;
        _instance.canvasGroup = root.GetComponent<CanvasGroup>();
        _instance.InitializeRuntimeReferences();
        return _instance;
    }

    private void InitializeRuntimeReferences()
    {
        tooltipRect ??= GetComponent<RectTransform>();
        canvasGroup ??= GetComponent<CanvasGroup>();
        _parentCanvas = GetComponentInParent<Canvas>();
        _uiCamera = _parentCanvas != null && _parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? _parentCanvas.worldCamera
            : null;

        TryFindUiReferences();
        EnsureShell();
        ApplyTheme();
        EnsureDatabaseReference();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        gameObject.SetActive(false);
    }

    private void TryFindUiReferences()
    {
        contentRoot ??= transform.Find("ContentRoot") as RectTransform;
        headerRoot ??= transform.Find("ContentRoot/HeaderRow") as RectTransform;
        if (headerRoot == null)
        {
            headerRoot = transform.Find("HeaderRow") as RectTransform;
        }

        itemNameText ??= (transform.Find("ContentRoot/HeaderRow/ItemName") ?? transform.Find("HeaderRow/ItemName") ?? transform.Find("ItemName"))?.GetComponent<Text>();
        statusText ??= transform.Find("ContentRoot/Status")?.GetComponent<Text>();
        descriptionText ??= (transform.Find("ContentRoot/Description") ?? transform.Find("Description"))?.GetComponent<Text>();
        priceText ??= (transform.Find("ContentRoot/Price") ?? transform.Find("Price"))?.GetComponent<Text>();
        qualityIcon ??= (transform.Find("ContentRoot/HeaderRow/QualityIcon") ?? transform.Find("HeaderRow/QualityIcon") ?? transform.Find("QualityIcon"))?.GetComponent<Image>();
        panelImage ??= (transform.Find("InnerPanel") ?? transform.Find("Background"))?.GetComponent<Image>();
        dividerImage ??= transform.Find("ContentRoot/Divider")?.GetComponent<Image>();
        frameImage ??= GetComponent<Image>();
    }

    private void EnsureShell()
    {
        frameImage ??= gameObject.AddComponent<Image>();
        frameImage.raycastTarget = false;

        panelImage = EnsureImage("InnerPanel", panelImage, transform);
        panelImage.rectTransform.anchorMin = Vector2.zero;
        panelImage.rectTransform.anchorMax = Vector2.one;
        panelImage.rectTransform.offsetMin = new Vector2(PanelBorder, PanelBorder);
        panelImage.rectTransform.offsetMax = new Vector2(-PanelBorder, -PanelBorder);
        panelImage.rectTransform.SetAsFirstSibling();

        contentRoot = EnsureRect("ContentRoot", contentRoot, panelImage.transform, typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        contentRoot.anchorMin = new Vector2(0f, 1f);
        contentRoot.anchorMax = new Vector2(0f, 1f);
        contentRoot.pivot = new Vector2(0f, 1f);
        contentRoot.anchoredPosition = new Vector2(ContentPaddingX, -ContentPaddingTop);
        contentRoot.sizeDelta = new Vector2(ContentWidth, 0f);

        var contentLayout = contentRoot.GetComponent<VerticalLayoutGroup>();
        contentLayout.padding = new RectOffset(0, 0, 0, 0);
        contentLayout.spacing = 5;
        contentLayout.childAlignment = TextAnchor.UpperLeft;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = true;
        contentLayout.childForceExpandWidth = false;
        contentLayout.childForceExpandHeight = false;

        var contentFitter = contentRoot.GetComponent<ContentSizeFitter>();
        contentFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        headerRoot = EnsureRect("HeaderRow", headerRoot, contentRoot.transform, typeof(Image), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
        headerImage = headerRoot.GetComponent<Image>();
        headerImage.raycastTarget = false;

        var headerLayout = headerRoot.GetComponent<HorizontalLayoutGroup>();
        headerLayout.padding = new RectOffset(8, 8, 5, 5);
        headerLayout.spacing = 6;
        headerLayout.childAlignment = TextAnchor.MiddleLeft;
        headerLayout.childControlWidth = false;
        headerLayout.childControlHeight = true;
        headerLayout.childForceExpandWidth = false;
        headerLayout.childForceExpandHeight = false;

        var headerFitter = headerRoot.GetComponent<ContentSizeFitter>();
        headerFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        headerFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        qualityIcon = EnsureImage("QualityIcon", qualityIcon, headerRoot.transform);
        qualityIcon.raycastTarget = false;
        qualityIcon.rectTransform.sizeDelta = new Vector2(12f, 12f);
        var qualityLayout = qualityIcon.GetComponent<LayoutElement>() ?? qualityIcon.gameObject.AddComponent<LayoutElement>();
        qualityLayout.preferredWidth = 12f;
        qualityLayout.preferredHeight = 12f;

        itemNameText = EnsureText("ItemName", itemNameText, headerRoot.transform);
        var titleLayout = itemNameText.GetComponent<LayoutElement>() ?? itemNameText.gameObject.AddComponent<LayoutElement>();
        titleLayout.preferredWidth = ContentWidth - 24f;
        titleLayout.flexibleWidth = 1f;

        dividerImage = EnsureImage("Divider", dividerImage, contentRoot.transform);
        dividerImage.raycastTarget = false;
        var dividerLayout = dividerImage.GetComponent<LayoutElement>() ?? dividerImage.gameObject.AddComponent<LayoutElement>();
        dividerLayout.preferredWidth = ContentWidth;
        dividerLayout.preferredHeight = 2f;

        statusText = EnsureText("Status", statusText, contentRoot.transform);
        descriptionText = EnsureText("Description", descriptionText, contentRoot.transform);
        priceText = EnsureText("Price", priceText, contentRoot.transform);
        ConfigureContentText(statusText);
        ConfigureContentText(descriptionText);
        ConfigureContentText(priceText);

        headerRoot.SetSiblingIndex(0);
        dividerImage.transform.SetSiblingIndex(1);
        statusText.transform.SetSiblingIndex(2);
        descriptionText.transform.SetSiblingIndex(3);
        priceText.transform.SetSiblingIndex(4);
    }

    private static Image EnsureImage(string name, Image current, Transform parent)
    {
        if (current == null)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                current = existing.GetComponent<Image>();
            }
        }

        if (current == null)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            current = go.GetComponent<Image>();
        }
        else if (current.transform.parent != parent)
        {
            current.transform.SetParent(parent, false);
        }

        return current;
    }

    private static RectTransform EnsureRect(string name, RectTransform current, Transform parent, params System.Type[] extraTypes)
    {
        if (current == null)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                current = existing as RectTransform;
            }
        }

        if (current == null)
        {
            var types = new System.Type[extraTypes.Length + 1];
            types[0] = typeof(RectTransform);
            for (int index = 0; index < extraTypes.Length; index++)
            {
                types[index + 1] = extraTypes[index];
            }

            GameObject go = new GameObject(name, types);
            current = go.GetComponent<RectTransform>();
            current.SetParent(parent, false);
        }
        else if (current.transform.parent != parent)
        {
            current.SetParent(parent, false);
        }

        for (int index = 0; index < extraTypes.Length; index++)
        {
            if (current.GetComponent(extraTypes[index]) == null)
            {
                current.gameObject.AddComponent(extraTypes[index]);
            }
        }

        return current;
    }

    private static Text EnsureText(string name, Text current, Transform parent)
    {
        if (current == null)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                current = existing.GetComponent<Text>();
            }
        }

        if (current == null)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            current = go.GetComponent<Text>();
        }
        else if (current.transform.parent != parent)
        {
            current.transform.SetParent(parent, false);
        }

        return current;
    }

    private void ConfigureContentText(Text text)
    {
        var layout = text.GetComponent<LayoutElement>() ?? text.gameObject.AddComponent<LayoutElement>();
        layout.preferredWidth = ContentWidth;
    }

    private void ApplyTheme()
    {
        Font font = LoadPreferredFont();

        tooltipRect.sizeDelta = new Vector2(TooltipWidth, TooltipMinHeight);
        frameImage.color = FrameColor;
        panelImage.color = PanelColor;
        headerImage.color = HeaderColor;
        dividerImage.color = DividerColor;

        ApplyTextTheme(itemNameText, font, 15, FontStyle.Bold, TextAnchor.MiddleLeft, TitleBaseColor);
        ApplyTextTheme(statusText, font, 12, FontStyle.Bold, TextAnchor.MiddleLeft, StatusColor);
        ApplyTextTheme(descriptionText, font, 12, FontStyle.Bold, TextAnchor.UpperLeft, DescriptionColor);
        ApplyTextTheme(priceText, font, 12, FontStyle.Bold, TextAnchor.MiddleRight, PriceColor);
    }

    private static void ApplyTextTheme(Text text, Font font, int size, FontStyle style, TextAnchor anchor, Color color)
    {
        text.font = font;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = anchor;
        text.color = color;
        text.supportRichText = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
    }

    private void EnsureDatabaseReference()
    {
        if (_database != null)
        {
            return;
        }

        InventoryService inventory = FindFirstObjectByType<InventoryService>(FindObjectsInactive.Include);
        if (inventory != null)
        {
            _database = inventory.Database;
        }
    }

    private void EnsureAttachedToPreferredCanvas(Transform sourceTransform)
    {
        Canvas canvas = ResolvePreferredParentCanvas(sourceTransform);
        if (canvas == null)
        {
            return;
        }

        if (transform.parent != canvas.transform)
        {
            transform.SetParent(canvas.transform, false);
        }

        _parentCanvas = canvas;
        _uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }

    private static Canvas ResolvePreferredParentCanvas(Transform preferredContext)
    {
        Canvas contextCanvas = preferredContext != null ? preferredContext.GetComponentInParent<Canvas>(true) : null;
        if (contextCanvas != null && contextCanvas.isRootCanvas && contextCanvas.gameObject.activeInHierarchy && contextCanvas.enabled)
        {
            return contextCanvas;
        }

        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Canvas best = null;
        int bestScore = int.MinValue;
        for (int index = 0; index < canvases.Length; index++)
        {
            Canvas canvas = canvases[index];
            int score = 0;
            if (canvas.gameObject.activeInHierarchy && canvas.enabled) score += 1000;
            if (canvas.isRootCanvas) score += 100;
            if (canvas == contextCanvas) score += 500;
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) score += 40;
            if (score > bestScore)
            {
                best = canvas;
                bestScore = score;
            }
        }

        return best;
    }

    private void QueueShow(VisualData visual, Transform sourceTransform)
    {
        bool sameSource = _currentSourceTransform == sourceTransform;
        bool alreadyQueued = _pendingShowCoroutine != null || _isShowing;

        _pendingVisual = visual;
        _currentSourceTransform = sourceTransform;
        _hasSourceConstraint = sourceTransform != null;
        _sourceRect = sourceTransform as RectTransform ?? sourceTransform?.GetComponent<RectTransform>();
        _movementRect = ResolveMovementRect(sourceTransform);

        if (sameSource && alreadyQueued)
        {
            if (_isShowing)
            {
                if (!VisualEquals(_pendingVisual, visual))
                {
                    ApplyVisual(visual);
                }

                if (canvasGroup != null && canvasGroup.alpha < 0.999f)
                {
                    StartFade(1f, false);
                }

                FollowMouse();
            }
            return;
        }

        if (_pendingShowCoroutine != null)
        {
            StopCoroutine(_pendingShowCoroutine);
        }

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        _isShowing = false;
        transform.SetAsLastSibling();
        _pendingShowCoroutine = StartCoroutine(ShowAfterDelayRoutine());
    }

    private RectTransform ResolveMovementRect(Transform sourceTransform)
    {
        Transform current = sourceTransform;
        RectTransform fallback = null;
        while (current != null)
        {
            if (current is RectTransform rect && current != sourceTransform)
            {
                fallback ??= rect;
                if (current.GetComponent<LayoutGroup>() != null ||
                    current.GetComponent<ScrollRect>() != null ||
                    current.GetComponent<Mask>() != null ||
                    current.GetComponent<RectMask2D>() != null ||
                    current.GetComponent<InventoryPanelUI>() != null ||
                    current.GetComponent<FarmGame.UI.BoxPanelUI>() != null ||
                    current.GetComponent<PackagePanelTabsUI>() != null)
                {
                    return rect;
                }
            }

            if (_parentCanvas != null && current == _parentCanvas.transform)
            {
                break;
            }

            current = current.parent;
        }

        return fallback;
    }

    private IEnumerator ShowAfterDelayRoutine()
    {
        yield return new WaitForSecondsRealtime(Mathf.Max(MinimumShowDelay, showDelay));

        if (ShouldSuppressBecauseOfInteraction() || !IsPointerWithinSourceRect())
        {
            _pendingShowCoroutine = null;
            Hide();
            yield break;
        }

        ApplyVisual(_pendingVisual);
        _isShowing = true;
        gameObject.SetActive(true);
        FollowMouse();
        StartFade(1f, false);
        _pendingShowCoroutine = null;
    }

    private void ApplyVisual(VisualData visual)
    {
        itemNameText.text = visual.Title;
        itemNameText.color = visual.TitleColor;

        statusText.text = visual.Status;
        statusText.gameObject.SetActive(visual.ShowStatus);

        descriptionText.text = visual.Description;
        descriptionText.gameObject.SetActive(visual.ShowDescription);

        priceText.text = visual.Price;
        priceText.gameObject.SetActive(visual.ShowPrice);

        qualityIcon.color = visual.QualityColor;
        qualityIcon.gameObject.SetActive(visual.ShowQualityIcon);

        dividerImage.gameObject.SetActive(visual.ShowStatus || visual.ShowDescription || visual.ShowPrice);

        LayoutRebuilder.ForceRebuildLayoutImmediate(headerRoot);
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot);
        float preferredHeight = Mathf.Max(LayoutUtility.GetPreferredHeight(contentRoot), 44f);
        contentRoot.sizeDelta = new Vector2(ContentWidth, preferredHeight);
        tooltipRect.sizeDelta = new Vector2(
            TooltipWidth,
            Mathf.Max(TooltipMinHeight, preferredHeight + ContentPaddingTop + ContentPaddingBottom + PanelBorder * 2f));
    }

    private void StartFade(float targetAlpha, bool deactivateOnComplete)
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha, deactivateOnComplete));
    }

    private IEnumerator FadeRoutine(float targetAlpha, bool deactivateOnComplete)
    {
        float startAlpha = canvasGroup != null ? canvasGroup.alpha : 0f;
        float elapsed = 0f;
        while (elapsed < FadeDurationSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, Mathf.Clamp01(elapsed / FadeDurationSeconds));
            }
            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = targetAlpha;
        }

        if (deactivateOnComplete && targetAlpha <= 0.001f)
        {
            gameObject.SetActive(false);
        }

        _fadeCoroutine = null;
    }

    private static bool VisualEquals(VisualData left, VisualData right)
    {
        return left.Title == right.Title &&
               left.TitleColor == right.TitleColor &&
               left.Status == right.Status &&
               left.Description == right.Description &&
               left.Price == right.Price &&
               left.ShowStatus == right.ShowStatus &&
               left.ShowDescription == right.ShowDescription &&
               left.ShowPrice == right.ShowPrice &&
               left.ShowQualityIcon == right.ShowQualityIcon &&
               left.QualityColor == right.QualityColor;
    }

    private bool IsPointerWithinSourceRect()
    {
        if (!_hasSourceConstraint)
        {
            return true;
        }

        if (_currentSourceTransform == null || _sourceRect == null)
        {
            return false;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(_sourceRect, Input.mousePosition, _uiCamera);
    }

    private Vector2 ClampScreenPositionToCanvas(Vector2 screenPosition)
    {
        if (_parentCanvas == null || _parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay || _uiCamera == null)
        {
            return new Vector2(
                Mathf.Clamp(screenPosition.x, 0f, Screen.width),
                Mathf.Clamp(screenPosition.y, 0f, Screen.height));
        }

        Rect canvasRect = _uiCamera.pixelRect;
        return new Vector2(
            Mathf.Clamp(screenPosition.x, canvasRect.xMin, canvasRect.xMax),
            Mathf.Clamp(screenPosition.y, canvasRect.yMin, canvasRect.yMax));
    }

    private static bool TryGetBoundsRect(RectTransform target, RectTransform canvasRect, out Rect rect)
    {
        rect = default;
        if (target == null || canvasRect == null)
        {
            return false;
        }

        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(canvasRect, target);
        rect = Rect.MinMaxRect(bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y);
        return true;
    }

    private static bool ShouldSuppressBecauseOfInteraction()
    {
        if (SlotDragContext.IsDragging)
        {
            return true;
        }

        InventoryInteractionManager manager = InventoryInteractionManager.Instance;
        if (manager != null && manager.IsHolding)
        {
            return true;
        }

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            return true;
        }

        return Input.GetKey(KeyCode.LeftShift) ||
               Input.GetKey(KeyCode.RightShift) ||
               Input.GetKey(KeyCode.LeftControl) ||
               Input.GetKey(KeyCode.RightControl);
    }

    private static Font LoadPreferredFont()
    {
#if UNITY_EDITOR
        string[] preferredPaths =
        {
            "Assets/111_Data/UI/Fonts/Dialogue/PixelAlt/WenQuanYi Bitmap Song 14px.ttf",
            "Assets/111_Data/UI/Fonts/Dialogue/Pixel/fusion-pixel-10px-monospaced-zh_hans.ttf",
            "Assets/111_Data/UI/Fonts/Dialogue/simhei.ttf"
        };

        for (int index = 0; index < preferredPaths.Length; index++)
        {
            Font font = AssetDatabase.LoadAssetAtPath<Font>(preferredPaths[index]);
            if (font != null)
            {
                return font;
            }
        }
#endif

        return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    private Color GetQualityColor(ItemQuality quality)
    {
        return quality switch
        {
            ItemQuality.Normal => TitleBaseColor,
            ItemQuality.Rare => new Color(0.72f, 0.88f, 1f, 1f),
            ItemQuality.Epic => new Color(0.90f, 0.77f, 1f, 1f),
            ItemQuality.Legendary => new Color(1f, 0.94f, 0.67f, 1f),
            _ => TitleBaseColor
        };
    }
}
