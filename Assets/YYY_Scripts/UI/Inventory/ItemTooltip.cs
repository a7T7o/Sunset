using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FarmGame.Data;
using FarmGame.Data.Core;

/// <summary>
/// 物品详情悬浮框
/// 鼠标悬浮在物品槽位上时显示物品信息
/// </summary>
public class ItemTooltip : MonoBehaviour
{
    private const float MinimumShowDelay = 0.6f;

    #region 单例

    public static ItemTooltip Instance { get; private set; }

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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (tooltipRect == null)
            tooltipRect = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        _parentCanvas = GetComponentInParent<Canvas>();
        if (_parentCanvas != null && _parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            _uiCamera = _parentCanvas.worldCamera;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
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
        if (item.IsEmpty || _database == null)
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
            showQualityIcon = item.quality > 0,
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
            canvasGroup.alpha = 0f;
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

    #endregion

    #region 辅助方法

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
        gameObject.SetActive(false);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

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
            canvasGroup.alpha = 0f;

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
    }

    #endregion
}
