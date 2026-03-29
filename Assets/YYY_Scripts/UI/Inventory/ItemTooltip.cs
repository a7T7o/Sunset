using System.Collections;
using FarmGame.Data;
using FarmGame.Data.Core;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物品详情悬浮框
/// 鼠标悬浮在物品槽位上时显示物品信息
/// </summary>
public class ItemTooltip : MonoBehaviour
{
    private const float MinimumShowDelay = 0.6f;
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
    [SerializeField] private Vector2 offset = new Vector2(15f, -15f);
    [SerializeField] private float showDelay = 0.6f;
    [SerializeField] private float fadeSpeed = 10f;

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
    private ItemStack _currentItem;
    private ItemDatabase _database;
    private int _currentAmount;
    private InventoryItem _currentRuntimeItem;
    private Coroutine _pendingShowCoroutine;
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
        if (_isShowing)
        {
            FollowMouse();

            if (canvasGroup != null && canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, fadeSpeed * Time.unscaledDeltaTime);
            }
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
        Show(item, null, amount);
    }

    public void Show(ItemStack item, InventoryItem runtimeItem, int amount = 1)
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
        Show(itemData, item, runtimeItem, amount);
    }

    public void Show(ItemData itemData, ItemStack item, InventoryItem runtimeItem = null, int amount = 1)
    {
        if (itemData == null || item.IsEmpty)
        {
            Hide();
            return;
        }

        _currentItem = item;
        _currentAmount = amount;
        _currentRuntimeItem = runtimeItem;

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
        gameObject.SetActive(false);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
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

        Vector2 targetPos = localPoint + offset;
        RectTransform canvasRect = _parentCanvas.transform as RectTransform;
        float halfWidth = tooltipRect.rect.width * 0.5f;
        float halfHeight = tooltipRect.rect.height * 0.5f;

        float maxX = canvasRect.rect.width * 0.5f - halfWidth;
        float minX = -canvasRect.rect.width * 0.5f + halfWidth;
        float maxY = canvasRect.rect.height * 0.5f - halfHeight;
        float minY = -canvasRect.rect.height * 0.5f + halfHeight;

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
        Canvas parentCanvas = FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
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
        rootRect.sizeDelta = new Vector2(280f, 100f);

        Image background = root.GetComponent<Image>();
        background.color = new Color(0.08f, 0.10f, 0.12f, 0.96f);
        background.raycastTarget = false;

        VerticalLayoutGroup layout = root.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(18, 18, 14, 14);
        layout.spacing = 8f;
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

    private void BuildRuntimeUi()
    {
        if (HasUsableUi())
        {
            return;
        }

        Font builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        RectTransform headerRow = CreateUiChild("HeaderRow", transform, typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
        HorizontalLayoutGroup headerLayout = headerRow.GetComponent<HorizontalLayoutGroup>();
        headerLayout.spacing = 8f;
        headerLayout.childAlignment = TextAnchor.MiddleLeft;
        headerLayout.childControlHeight = true;
        headerLayout.childControlWidth = false;
        headerLayout.childForceExpandHeight = false;
        headerLayout.childForceExpandWidth = false;

        ContentSizeFitter headerFitter = headerRow.GetComponent<ContentSizeFitter>();
        headerFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        headerFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        itemNameText = CreateText("ItemName", builtinFont, 22, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.98f, 0.98f, 0.95f, 1f), headerRow);
        descriptionText = CreateText("Description", builtinFont, 18, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.83f, 0.88f, 0.90f, 0.98f), transform);
        priceText = CreateText("Price", builtinFont, 17, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.94f, 0.84f, 0.48f, 1f), transform);

        descriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        descriptionText.verticalOverflow = VerticalWrapMode.Overflow;
        LayoutElement descriptionLayout = descriptionText.gameObject.AddComponent<LayoutElement>();
        descriptionLayout.preferredWidth = 280f;

        Outline outline = gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.42f);
        outline.effectDistance = new Vector2(1f, -1f);

        Shadow shadow = gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.22f);
        shadow.effectDistance = new Vector2(3f, -3f);
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

        _pendingShowCoroutine = StartCoroutine(ShowAfterDelayRoutine());
    }

    private IEnumerator ShowAfterDelayRoutine()
    {
        float delay = Mathf.Max(MinimumShowDelay, showDelay);
        if (delay > 0f)
        {
            yield return new WaitForSecondsRealtime(delay);
        }

        ApplyVisual(_pendingVisualData);
        _isShowing = true;
        gameObject.SetActive(true);
        FollowMouse();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        _pendingShowCoroutine = null;
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
