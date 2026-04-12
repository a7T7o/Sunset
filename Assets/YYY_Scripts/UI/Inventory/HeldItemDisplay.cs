using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 跟随鼠标显示被拿起的物品
/// 独立组件，不依赖任何 SlotUI 代码
/// </summary>
public class HeldItemDisplay : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text amountText;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    private Canvas parentCanvas;
    private RectTransform rectTransform;
    private bool hasPinnedScreenPosition;
    private Vector2 pinnedScreenPosition;
    private bool isInitialized;
    
    void Awake()
    {
        EnsureInitialized();
        ApplyHiddenVisualState();
    }

    private void EnsureInitialized()
    {
        if (isInitialized)
        {
            return;
        }

        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
            rectTransform = gameObject.AddComponent<RectTransform>();
        
        parentCanvas = GetComponentInParent<Canvas>();
        
        // 自动查找或创建子组件
        if (iconImage == null)
        {
            var t = transform.Find("Icon");
            if (t != null)
            {
                iconImage = t.GetComponent<Image>();
            }
            else
            {
                var go = new GameObject("Icon");
                go.transform.SetParent(transform, false);
                iconImage = go.AddComponent<Image>();
                iconImage.raycastTarget = false;
                var rt = (RectTransform)iconImage.transform;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
        }
        
        if (amountText == null)
        {
            var t = transform.Find("Amount");
            if (t != null)
            {
                amountText = t.GetComponent<Text>();
            }
            else
            {
                var go = new GameObject("Amount");
                go.transform.SetParent(transform, false);
                amountText = go.AddComponent<Text>();
                amountText.raycastTarget = false;
                amountText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                amountText.fontSize = 18;
                amountText.fontStyle = FontStyle.BoldAndItalic;
                amountText.color = Color.black;
                amountText.alignment = TextAnchor.LowerRight;
                var rt = (RectTransform)amountText.transform;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.offsetMin = new Vector2(21.2356f, 0f);
                rt.offsetMax = new Vector2(-3.8808f, -41.568f);
            }
        }
        
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        isInitialized = true;
    }

    private void ApplyHiddenVisualState()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }

        hasPinnedScreenPosition = false;
    }
    
    void OnEnable()
    {
        EnsureInitialized();

        // 确保有 Canvas 引用
        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();

        if (IsShowing)
        {
            FollowMouse();
        }
    }
    
    /// <summary>
    /// 显示物品
    /// </summary>
    public void Show(int itemId, int amount, Sprite icon, Vector2? initialScreenPosition = null)
    {
        EnsureInitialized();

        gameObject.SetActive(true);
        EnsureInitialized();
        
        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
            
            // 使用 UIItemIconScaler 保持样式一致（如果可用）
            if (icon != null)
            {
                UIItemIconScaler.SetIconWithAutoScale(iconImage, icon, null);
            }
        }
        
        UpdateAmount(amount);
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = false; // 不阻挡射线
        }

        Canvas.ForceUpdateCanvases();
        if (initialScreenPosition.HasValue)
        {
            SyncToScreenPosition(initialScreenPosition.Value, pinForNextFrame: true);
        }
        else
        {
            hasPinnedScreenPosition = false;
            FollowMouse();
        }
        
        if (showDebugInfo)
            Debug.Log($"[HeldItemDisplay] 显示物品: itemId={itemId}, sprite={icon?.name}, amount={amount}");
    }
    
    /// <summary>
    /// 更新数量显示
    /// </summary>
    public void UpdateAmount(int amount)
    {
        if (amountText != null)
        {
            amountText.text = amount > 1 ? amount.ToString() : "";
        }
    }
    
    /// <summary>
    /// 隐藏显示
    /// </summary>
    public void Hide()
    {
        EnsureInitialized();
        ApplyHiddenVisualState();

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 是否正在显示
    /// </summary>
    public bool IsShowing
    {
        get
        {
            EnsureInitialized();
            return gameObject.activeInHierarchy && canvasGroup != null && canvasGroup.alpha > 0.001f;
        }
    }

    public void SyncToScreenPosition(Vector2 screenPosition, bool pinForNextFrame = false)
    {
        EnsureInitialized();
        pinnedScreenPosition = screenPosition;
        hasPinnedScreenPosition = pinForNextFrame;

        if (!IsShowing)
        {
            return;
        }

        FollowMouse();
    }
    
    void Update()
    {
        if (!IsShowing) return;
        
        // 跟随鼠标
        FollowMouse();
    }
    
    private void FollowMouse()
    {
        EnsureInitialized();

        if (parentCanvas == null)
        {
            parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas == null) return;
        }
        
        if (rectTransform == null) return;

        Vector2 screenPosition = hasPinnedScreenPosition ? pinnedScreenPosition : (Vector2)Input.mousePosition;
        hasPinnedScreenPosition = false;
        screenPosition = ClampScreenPositionToCanvas(screenPosition);
        
        // 根据 Canvas 渲染模式处理
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rectTransform.position = screenPosition;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                screenPosition,
                parentCanvas.worldCamera,
                out Vector2 localPoint
            );
            rectTransform.localPosition = localPoint;
        }
    }

    private Vector2 ClampScreenPositionToCanvas(Vector2 screenPosition)
    {
        if (parentCanvas == null)
        {
            return screenPosition;
        }

        Rect screenRect = GetCanvasScreenRect();
        return new Vector2(
            Mathf.Clamp(screenPosition.x, screenRect.xMin, screenRect.xMax),
            Mathf.Clamp(screenPosition.y, screenRect.yMin, screenRect.yMax));
    }

    private Rect GetCanvasScreenRect()
    {
        if (parentCanvas == null || parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay || parentCanvas.worldCamera == null)
        {
            return new Rect(0f, 0f, Screen.width, Screen.height);
        }

        return parentCanvas.worldCamera.pixelRect;
    }
}
