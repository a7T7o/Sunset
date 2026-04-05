using System.Collections;
using FarmGame.Data;
using FarmGame.Data.Core;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 物品详情悬浮框
/// 鼠标悬浮在物品槽位上时显示物品信息
/// </summary>
public class ItemTooltip : MonoBehaviour
{
    private const float MinimumShowDelay = 1f;
    private const float FadeDurationSeconds = 0.3f;
    private static ItemTooltip _instance;

    #region 单例

    public static ItemTooltip Instance => EnsureInstance();

    #endregion

    #region 序列化字段

    [Header("UI 引用")]
    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text itemNameText = null;
    [SerializeField] private Text descriptionText = null;
    [SerializeField] private Text priceText = null;
    [SerializeField] private Image qualityIcon = null;

    [Header("显示设置")]
    [SerializeField] private Vector2 offset = new Vector2(22f, -24f);
    [SerializeField] private float showDelay = 1f;

    #endregion

    #region 私有字段

    private struct TooltipVisualData
    {
        public string title;
        public Color titleColor;
        public string description;
        public string price;
        public bool showPrice;
        public bool showQualityIcon;
        public Color qualityColor;
    }

    private Canvas _parentCanvas;
    private Camera _uiCamera;
    private bool _isShowing;
    private RectTransform _followBoundsRect;
    private ItemStack _currentItem;
    private ItemDatabase _database;
    private int _currentAmount;
    private InventoryItem _currentRuntimeItem;
    private Coroutine _pendingShowCoroutine;
    private Coroutine _fadeCoroutine;
    private TooltipVisualData _pendingVisualData;

    #endregion

    #region Unity 生命周期

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
        if (_isShowing && gameObject.activeSelf && canvasGroup != null && canvasGroup.alpha > 0.001f)
        {
            FollowMouse();
        }

        if (_isShowing && ShouldSuppressBecauseOfInteraction())
        {
            Hide();
        }

        if (_isShowing && !IsPointerWithinFollowBounds())
        {
            Hide();
        }
    }

    #endregion

    #region 公共方法

    public void SetDatabase(ItemDatabase database)
    {
        _database = database;
    }

    public void Show(ItemStack item, int amount = 1)
    {
        Show(item, null, amount, null);
    }

    public void Show(ItemStack item, InventoryItem runtimeItem, int amount = 1)
    {
        Show(item, runtimeItem, amount, null);
    }

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

        var itemData = _database.GetItemByID(item.itemId);
        Show(itemData, item, runtimeItem, amount, sourceTransform);
    }

    public void Show(ItemData itemData, ItemStack item, InventoryItem runtimeItem = null, int amount = 1)
    {
        Show(itemData, item, runtimeItem, amount, null);
    }

    public void Show(ItemData itemData, ItemStack item, InventoryItem runtimeItem, int amount, Transform sourceTransform)
    {
        if (itemData == null || item.IsEmpty)
        {
            Hide();
            return;
        }

        _currentItem = item;
        _currentAmount = amount;
        _currentRuntimeItem = runtimeItem;
        EnsureAttachedToPreferredCanvas(sourceTransform);
        _followBoundsRect = ResolveFollowBoundsRect(sourceTransform);

        int totalPrice = itemData.GetSellPriceWithQuality((ItemQuality)item.quality) * amount;
        QueueShow(new TooltipVisualData
        {
            title = itemData.itemName,
            titleColor = GetQualityColor((ItemQuality)item.quality),
            description = ItemTooltipTextBuilder.Build(itemData, runtimeItem),
            price = amount > 1 ? $"总价值: {totalPrice} 金币 ({amount}个)" : $"价值: {totalPrice} 金币",
            showPrice = totalPrice > 0,
            showQualityIcon = qualityIcon != null && item.quality > 0,
            qualityColor = itemData.GetQualityStarColor((ItemQuality)item.quality)
        });
    }

    public void ShowCustom(string title, string body)
    {
        ShowCustom(title, body, null);
    }

    public void ShowCustom(string title, string body, Transform sourceTransform)
    {
        EnsureAttachedToPreferredCanvas(sourceTransform);
        _followBoundsRect = ResolveFollowBoundsRect(sourceTransform);
        QueueShow(new TooltipVisualData
        {
            title = title,
            titleColor = Color.white,
            description = body,
            price = string.Empty,
            showPrice = false,
            showQualityIcon = false,
            qualityColor = Color.white
        });
    }

    public void Hide()
    {
        if (_pendingShowCoroutine != null)
        {
            StopCoroutine(_pendingShowCoroutine);
            _pendingShowCoroutine = null;
        }

        _isShowing = false;
        _currentItem = ItemStack.Empty;
        _currentAmount = 0;
        _currentRuntimeItem = null;
        _followBoundsRect = null;

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
        if (tooltipRect == null || _parentCanvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentCanvas.transform as RectTransform,
            Input.mousePosition,
            _uiCamera,
            out Vector2 localPoint
        );

        if (_followBoundsRect != null)
        {
            localPoint = ClampLocalPointToFollowBounds(localPoint);
        }

        RectTransform canvasRect = _parentCanvas.transform as RectTransform;
        float halfWidth = tooltipRect.rect.width * 0.5f;
        float halfHeight = tooltipRect.rect.height * 0.5f;

        float rightX = localPoint.x + offset.x;
        float leftX = localPoint.x - offset.x;
        float downY = localPoint.y + offset.y;
        float upY = localPoint.y - offset.y;

        Vector2 targetPos = new Vector2(rightX, downY);
        float maxX = canvasRect.rect.width * 0.5f - halfWidth;
        float minX = -canvasRect.rect.width * 0.5f + halfWidth;
        float maxY = canvasRect.rect.height * 0.5f - halfHeight;
        float minY = -canvasRect.rect.height * 0.5f + halfHeight;

        if (targetPos.x > maxX)
        {
            targetPos.x = leftX;
        }

        if (targetPos.y < minY)
        {
            targetPos.y = upY;
        }

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        tooltipRect.anchoredPosition = targetPos;
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

        return CreateRuntimeInstance();
    }

    #endregion

    #region 辅助方法

    private void InitializeRuntimeReferences()
    {
        if (tooltipRect == null)
        {
            tooltipRect = GetComponent<RectTransform>();
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        _parentCanvas = GetComponentInParent<Canvas>();
        if (_parentCanvas != null && _parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            _uiCamera = _parentCanvas.worldCamera;
        }

        TryFindUiReferences();
        if (!HasUsableUi())
        {
            BuildRuntimeUi();
            TryFindUiReferences();
        }

        EnsureDatabaseReference();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        gameObject.SetActive(false);
    }

    private bool HasUsableUi()
    {
        return itemNameText != null &&
               descriptionText != null &&
               priceText != null;
    }

    private void TryFindUiReferences()
    {
        if (itemNameText == null)
        {
            Transform titleTransform = transform.Find("HeaderRow/ItemName");
            if (titleTransform != null)
            {
                itemNameText = titleTransform.GetComponent<Text>();
            }
        }

        if (descriptionText == null)
        {
            Transform descriptionTransform = transform.Find("Description");
            if (descriptionTransform != null)
            {
                descriptionText = descriptionTransform.GetComponent<Text>();
            }
        }

        if (priceText == null)
        {
            Transform priceTransform = transform.Find("Price");
            if (priceTransform != null)
            {
                priceText = priceTransform.GetComponent<Text>();
            }
        }

        if (qualityIcon == null)
        {
            Transform qualityTransform = transform.Find("HeaderRow/QualityIcon");
            if (qualityTransform != null)
            {
                qualityIcon = qualityTransform.GetComponent<Image>();
            }
        }
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

    private static ItemTooltip CreateRuntimeInstance()
    {
        Canvas parentCanvas = ResolvePreferredParentCanvas(null);
        if (parentCanvas == null)
        {
            Debug.LogWarning("[ItemTooltip] 场景里没有 Canvas，无法创建运行时 Tooltip。");
            return null;
        }

        GameObject root = new GameObject(
            "RuntimeItemTooltip",
            typeof(RectTransform),
            typeof(CanvasGroup),
            typeof(Image),
            typeof(VerticalLayoutGroup),
            typeof(ContentSizeFitter));
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.SetParent(parentCanvas.transform, false);
        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.sizeDelta = new Vector2(168f, 64f);

        Image background = root.GetComponent<Image>();
        background.color = new Color(0.25f, 0.18f, 0.09f, 0.96f);
        background.raycastTarget = false;

        VerticalLayoutGroup layout = root.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(8, 8, 7, 7);
        layout.spacing = 3f;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = root.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ItemTooltip tooltip = root.AddComponent<ItemTooltip>();
        tooltip.tooltipRect = rootRect;
        tooltip.canvasGroup = root.GetComponent<CanvasGroup>();
        tooltip.BuildRuntimeUi();
        tooltip.InitializeRuntimeReferences();
        return tooltip;
    }

    private void EnsureAttachedToPreferredCanvas(Transform sourceTransform)
    {
        Canvas preferredCanvas = ResolvePreferredParentCanvas(sourceTransform);
        if (preferredCanvas == null)
        {
            return;
        }

        if (transform.parent != preferredCanvas.transform)
        {
            transform.SetParent(preferredCanvas.transform, false);
        }

        _parentCanvas = preferredCanvas;
        _uiCamera = _parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : _parentCanvas.worldCamera;
        transform.SetAsLastSibling();
    }

    private RectTransform ResolveFollowBoundsRect(Transform sourceTransform)
    {
        if (sourceTransform == null)
        {
            return null;
        }

        return sourceTransform as RectTransform ??
               sourceTransform.GetComponent<RectTransform>();
    }

    private Vector2 ClampLocalPointToFollowBounds(Vector2 localPoint)
    {
        if (_followBoundsRect == null || _parentCanvas == null)
        {
            return localPoint;
        }

        RectTransform canvasRect = _parentCanvas.transform as RectTransform;
        if (canvasRect == null)
        {
            return localPoint;
        }

        Vector3[] worldCorners = new Vector3[4];
        _followBoundsRect.GetWorldCorners(worldCorners);

        Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        for (int index = 0; index < worldCorners.Length; index++)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                RectTransformUtility.WorldToScreenPoint(_uiCamera, worldCorners[index]),
                _uiCamera,
                out Vector2 cornerLocalPoint);
            min = Vector2.Min(min, cornerLocalPoint);
            max = Vector2.Max(max, cornerLocalPoint);
        }

        localPoint.x = Mathf.Clamp(localPoint.x, min.x, max.x);
        localPoint.y = Mathf.Clamp(localPoint.y, min.y, max.y);
        return localPoint;
    }

    private bool IsPointerWithinFollowBounds()
    {
        if (_followBoundsRect == null)
        {
            return true;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(_followBoundsRect, Input.mousePosition, _uiCamera);
    }

    private static Canvas ResolvePreferredParentCanvas(Transform preferredContext)
    {
        Canvas contextCanvas = preferredContext != null
            ? preferredContext.GetComponentInParent<Canvas>(true)
            : null;
        if (contextCanvas != null && contextCanvas.isRootCanvas && contextCanvas.gameObject.activeInHierarchy && contextCanvas.enabled)
        {
            return contextCanvas;
        }

        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Canvas bestCanvas = null;
        int bestScore = int.MinValue;

        for (int index = 0; index < canvases.Length; index++)
        {
            Canvas canvas = canvases[index];
            if (canvas == null)
            {
                continue;
            }

            int score = 0;
            if (canvas.gameObject.activeInHierarchy && canvas.enabled)
            {
                score += 1000;
            }

            if (canvas.isRootCanvas)
            {
                score += 100;
            }

            if (canvas == contextCanvas)
            {
                score += 500;
            }

            switch (canvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    score += 40;
                    break;
                case RenderMode.ScreenSpaceCamera:
                    score += 20;
                    break;
            }

            if (score > bestScore)
            {
                bestCanvas = canvas;
                bestScore = score;
            }
        }

        return bestCanvas;
    }

    private void BuildRuntimeUi()
    {
        if (HasUsableUi())
        {
            return;
        }

        Font tooltipFont = LoadPreferredFont();

        RectTransform headerRow = CreateUiChild("HeaderRow", transform, typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
        HorizontalLayoutGroup headerLayout = headerRow.GetComponent<HorizontalLayoutGroup>();
        headerLayout.spacing = 6f;
        headerLayout.childAlignment = TextAnchor.MiddleLeft;
        headerLayout.childControlHeight = true;
        headerLayout.childControlWidth = false;
        headerLayout.childForceExpandHeight = false;
        headerLayout.childForceExpandWidth = false;

        ContentSizeFitter headerFitter = headerRow.GetComponent<ContentSizeFitter>();
        headerFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        headerFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        RectTransform qualityRect = CreateUiChild("QualityIcon", headerRow);
        qualityRect.anchorMin = new Vector2(0f, 0.5f);
        qualityRect.anchorMax = new Vector2(0f, 0.5f);
        qualityRect.pivot = new Vector2(0.5f, 0.5f);
        qualityRect.sizeDelta = new Vector2(14f, 14f);
        LayoutElement qualityLayout = qualityRect.gameObject.AddComponent<LayoutElement>();
        qualityLayout.preferredWidth = 14f;
        qualityLayout.preferredHeight = 14f;
        qualityIcon = qualityRect.gameObject.AddComponent<Image>();
        qualityIcon.raycastTarget = false;
        qualityIcon.enabled = true;
        qualityIcon.gameObject.SetActive(false);

        itemNameText = CreateText("ItemName", tooltipFont, 14, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(1f, 0.96f, 0.86f, 1f), headerRow);
        descriptionText = CreateText("Description", tooltipFont, 11, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.97f, 0.92f, 0.84f, 1f), transform);
        priceText = CreateText("Price", tooltipFont, 10, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(1f, 0.82f, 0.38f, 1f), transform);

        descriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        descriptionText.verticalOverflow = VerticalWrapMode.Overflow;
        LayoutElement descriptionLayout = descriptionText.gameObject.AddComponent<LayoutElement>();
        descriptionLayout.preferredWidth = 144f;

        Outline outline = gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0.98f, 0.79f, 0.39f, 0.92f);
        outline.effectDistance = new Vector2(1f, -1f);

        Shadow shadow = gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0.09f, 0.05f, 0.02f, 0.24f);
        shadow.effectDistance = new Vector2(2f, -2f);
    }

    private static RectTransform CreateUiChild(string name, Transform parent, params System.Type[] extraComponents)
    {
        var componentTypes = new System.Type[extraComponents.Length + 1];
        componentTypes[0] = typeof(RectTransform);
        for (int i = 0; i < extraComponents.Length; i++)
        {
            componentTypes[i + 1] = extraComponents[i];
        }

        GameObject child = new GameObject(name, componentTypes);
        RectTransform rect = child.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        return rect;
    }

    private static Text CreateText(string name, Font font, int size, FontStyle style, TextAnchor anchor, Color color, Transform parent)
    {
        RectTransform rect = CreateUiChild(name, parent);
        Text text = rect.gameObject.AddComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = anchor;
        text.color = color;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        return text;
    }

    private Color GetQualityColor(ItemQuality quality)
    {
        return quality switch
        {
            ItemQuality.Normal => Color.white,
            ItemQuality.Rare => new Color(0.3f, 0.7f, 1f),
            ItemQuality.Epic => new Color(0.6f, 0.2f, 0.8f),
            ItemQuality.Legendary => new Color(1f, 0.8f, 0.2f),
            _ => Color.white
        };
    }

    private void QueueShow(TooltipVisualData visualData)
    {
        if (_pendingShowCoroutine != null)
        {
            StopCoroutine(_pendingShowCoroutine);
            _pendingShowCoroutine = null;
        }

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        _pendingVisualData = visualData;
        _isShowing = false;
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        transform.SetAsLastSibling();

        _pendingShowCoroutine = StartCoroutine(ShowAfterDelayRoutine());
    }

    private IEnumerator ShowAfterDelayRoutine()
    {
        float delay = Mathf.Max(MinimumShowDelay, showDelay);
        if (delay > 0f)
        {
            yield return new WaitForSecondsRealtime(delay);
        }

        if (ShouldSuppressBecauseOfInteraction())
        {
            _pendingShowCoroutine = null;
            Hide();
            yield break;
        }

        if (!IsPointerWithinFollowBounds())
        {
            _pendingShowCoroutine = null;
            Hide();
            yield break;
        }

        ApplyVisual(_pendingVisualData);
        _isShowing = true;
        gameObject.SetActive(true);
        FollowMouse();
        StartFade(1f, false);

        _pendingShowCoroutine = null;
    }

    private void StartFade(float targetAlpha, bool deactivateOnComplete)
    {
        if (canvasGroup == null)
        {
            if (deactivateOnComplete)
            {
                gameObject.SetActive(false);
            }
            return;
        }

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha, deactivateOnComplete));
    }

    private IEnumerator FadeRoutine(float targetAlpha, bool deactivateOnComplete)
    {
        float startAlpha = canvasGroup.alpha;
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (Mathf.Approximately(startAlpha, targetAlpha))
        {
            canvasGroup.alpha = targetAlpha;
            if (deactivateOnComplete && targetAlpha <= 0.001f)
            {
                gameObject.SetActive(false);
            }

            _fadeCoroutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < FadeDurationSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / FadeDurationSeconds);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        if (deactivateOnComplete && targetAlpha <= 0.001f)
        {
            gameObject.SetActive(false);
        }

        _fadeCoroutine = null;
    }

    private static bool ShouldSuppressBecauseOfInteraction()
    {
        if (SlotDragContext.IsDragging)
        {
            return true;
        }

        InventoryInteractionManager interactionManager = InventoryInteractionManager.Instance;
        if (interactionManager != null && interactionManager.IsHolding)
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
            Font editorFont = AssetDatabase.LoadAssetAtPath<Font>(preferredPaths[index]);
            if (editorFont != null)
            {
                return editorFont;
            }
        }
#endif

        return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    private void ApplyVisual(TooltipVisualData visualData)
    {
        if (itemNameText != null)
        {
            itemNameText.text = visualData.title;
            itemNameText.color = visualData.titleColor;
        }

        if (descriptionText != null)
        {
            descriptionText.text = visualData.description;
        }

        if (priceText != null)
        {
            priceText.text = visualData.price;
            priceText.gameObject.SetActive(visualData.showPrice);
        }

        if (qualityIcon != null)
        {
            qualityIcon.color = visualData.qualityColor;
            qualityIcon.gameObject.SetActive(visualData.showQualityIcon);
        }

        if (tooltipRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);
        }
    }

    #endregion
}
