using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FarmGame.Data;
using FarmGame.Data.Core;

/// <summary>
/// 装备槽位 UI - 基础版本
/// 只负责显示装备图标和数量
/// 实现基础的点击功能
/// 与 InventorySlotUI 保持一致的简单设计
/// </summary>
public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text amountText;
    [SerializeField] private Image selectedOverlay;

    private Image durabilityBar;
    private Image durabilityBarBg;
    private float statusBarAlpha;
    private float statusBarTargetAlpha;
    private bool statusBarHasData;
    private bool isHovered;
    private const float StatusBarFadeDuration = 0.3f;

    private EquipmentService equipment;
    private InventoryService inventory;
    private ItemDatabase database;
    private InventoryPanelUI inventoryPanel;
    private int index; // 0..5

    /// <summary>
    /// 槽位索引（供外部查询）
    /// </summary>
    public int Index => index;

    public EquipmentService Equipment => equipment;
    public ItemDatabase Database => database;

    public ItemStack GetCurrentStack()
    {
        return equipment != null ? equipment.GetEquip(index) : ItemStack.Empty;
    }

    public InventoryItem GetCurrentRuntimeItem()
    {
        return equipment != null ? equipment.GetEquipItem(index) : null;
    }

    #region Unity 生命周期
    void Awake()
    {
        if (toggle == null) toggle = GetComponent<Toggle>();
        if (toggle != null)
        {
            var navigation = toggle.navigation;
            navigation.mode = Navigation.Mode.None;
            toggle.navigation = navigation;
        }

        // ★ 关键：确保槽位本身有可以接收射线的 Image
        var bgImage = GetComponent<Image>();
        if (bgImage != null)
        {
            bgImage.raycastTarget = true;
        }
        
        if (iconImage == null)
        {
            var t = transform.Find("Icon");
            if (t != null)
            {
                iconImage = t.GetComponent<Image>();
            }
            else
            {
                // 自动创建 Icon
                var go = new GameObject("Icon");
                go.transform.SetParent(transform, false);
                iconImage = go.AddComponent<Image>();
                iconImage.raycastTarget = false;
                var rt = (RectTransform)iconImage.transform;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                iconImage.enabled = false;
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
                // 自动创建 Amount
                var go = new GameObject("Amount");
                go.transform.SetParent(transform, false);
                amountText = go.AddComponent<Text>();
                amountText.raycastTarget = false;
                amountText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                amountText.fontSize = 18;
                amountText.fontStyle = FontStyle.BoldAndItalic;
                amountText.color = Color.black;
                amountText.alignment = TextAnchor.LowerRight;
                amountText.text = "";
                var rt = (RectTransform)amountText.transform;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.offsetMin = new Vector2(21.2356f, 0f);
                rt.offsetMax = new Vector2(-3.8808f, -41.568f);
            }
        }

        if (selectedOverlay == null)
        {
            var t = transform.Find("Selected");
            if (t != null) selectedOverlay = t.GetComponent<Image>();
        }

        // ★ 方案 D：自动添加 Interaction 组件
        var interaction = gameObject.GetComponent<InventorySlotInteraction>();
        if (interaction == null)
        {
            interaction = gameObject.AddComponent<InventorySlotInteraction>();
        }
        interaction.Bind(this, true);

        CreateDurabilityBar();
    }

    void OnEnable()
    {
        if (equipment != null) equipment.OnEquipSlotChanged += HandleEquipChanged;
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        Refresh();
        RefreshSelection();
    }

    void Update()
    {
        UpdateStatusBarVisibility();
        TickStatusBarFade();
    }

    void OnDisable()
    {
        if (equipment != null) equipment.OnEquipSlotChanged -= HandleEquipChanged;
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
    #endregion

    #region 绑定和刷新
    public void Bind(EquipmentService equip, InventoryService inv, ItemDatabase db, int equipIndex)
    {
        Bind(equip, inv, db, null, equipIndex);
    }

    public void Bind(
        EquipmentService equip,
        InventoryService inv,
        ItemDatabase db,
        InventoryPanelUI ownerInventoryPanel,
        int equipIndex)
    {
        if (equipment != null)
        {
            equipment.OnEquipSlotChanged -= HandleEquipChanged;
        }

        equipment = equip;
        inventory = inv;
        database = db;
        inventoryPanel = ownerInventoryPanel != null
            ? ownerInventoryPanel
            : GetComponentInParent<InventoryPanelUI>(true);
        index = equipIndex;
        if (isActiveAndEnabled)
        {
            if (equipment != null)
            {
                equipment.OnEquipSlotChanged -= HandleEquipChanged;
                equipment.OnEquipSlotChanged += HandleEquipChanged;
            }

            Refresh();
            RefreshSelection();
        }
        else
        {
            Refresh();
        }
    }

    void HandleEquipChanged(int changed)
    {
        if (changed == index)
        {
            Refresh();
            RefreshSelection();
        }
    }

    public void Refresh()
    {
        if (equipment == null || database == null) return;
        var s = equipment.GetEquip(index);
        if (s.IsEmpty)
        {
            if (iconImage != null) UIItemIconScaler.SetIconWithAutoScale(iconImage, null, null);
            if (amountText != null) amountText.text = "";
            UpdateDurabilityBar(null, null);
            return;
        }
        var data = database.GetItemByID(s.itemId);
        if (iconImage != null)
        {
            UIItemIconScaler.SetIconWithAutoScale(iconImage, data?.GetBagSprite(), data);
        }
        if (amountText != null)
        {
            amountText.text = s.amount > 1 ? s.amount.ToString() : "";
        }

        UpdateDurabilityBar(equipment.GetEquipItem(index), data);
    }

    public void RefreshSelection()
    {
        bool isSelected = ResolveSelectionState();

        if (toggle != null)
        {
#if UNITY_2021_2_OR_NEWER
            toggle.SetIsOnWithoutNotify(isSelected);
#else
            toggle.isOn = isSelected;
#endif
        }

        if (selectedOverlay != null)
        {
            selectedOverlay.enabled = isSelected;
        }
    }

    public void Select()
    {
        var resolvedPanel = ResolveInventoryPanel();
        if (resolvedPanel != null)
        {
            resolvedPanel.SetSelectedEquipmentIndex(index);
            return;
        }

        if (toggle != null)
        {
            toggle.isOn = true;
        }
        else if (selectedOverlay != null)
        {
            selectedOverlay.enabled = true;
        }
    }

    public void ClearSelectionState()
    {
        var resolvedPanel = ResolveInventoryPanel();
        if (resolvedPanel != null)
        {
            resolvedPanel.ClearDownSelectionState();
            return;
        }

        if (toggle != null)
        {
            toggle.isOn = false;
        }
        else if (selectedOverlay != null)
        {
            selectedOverlay.enabled = false;
        }
    }

    public void SetHovered(bool hovered)
    {
        if (isHovered == hovered)
        {
            return;
        }

        isHovered = hovered;
        UpdateStatusBarVisibility();
    }

    private InventoryPanelUI ResolveInventoryPanel()
    {
        if (inventoryPanel == null || !inventoryPanel.gameObject)
        {
            inventoryPanel = GetComponentInParent<InventoryPanelUI>(true);
        }

        if (inventoryPanel == null)
        {
            inventoryPanel = InventoryPanelUI.ActiveVisibleInstance;
        }

        return inventoryPanel;
    }

    private bool ResolveSelectionState()
    {
        var resolvedPanel = ResolveInventoryPanel();
        if (resolvedPanel != null)
        {
            return resolvedPanel.IsEquipmentSlotSelected(index);
        }

        return toggle != null && toggle.isOn;
    }

    private void OnToggleValueChanged(bool isOn)
    {
        bool expectedSelection = ResolveSelectionState();
        if (isOn != expectedSelection)
        {
            RefreshSelection();
        }
    }

    private void CreateDurabilityBar()
    {
        if (durabilityBar != null && durabilityBarBg != null)
        {
            return;
        }

        var existingBar = transform.Find("DurabilityBar");
        if (existingBar != null)
        {
            durabilityBar = existingBar.GetComponent<Image>();
            var existingBg = transform.Find("DurabilityBarBg");
            if (existingBg != null)
            {
                durabilityBarBg = existingBg.GetComponent<Image>();
            }

            return;
        }

        const float borderPx = 4f;
        const float bottomPx = 6f;
        const float barHeight = 4f;

        var bgGo = new GameObject("DurabilityBarBg");
        bgGo.transform.SetParent(transform, false);
        durabilityBarBg = bgGo.AddComponent<Image>();
        durabilityBarBg.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        durabilityBarBg.raycastTarget = false;

        var bgRt = (RectTransform)durabilityBarBg.transform;
        bgRt.anchorMin = new Vector2(0f, 0f);
        bgRt.anchorMax = new Vector2(1f, 0f);
        bgRt.pivot = new Vector2(0.5f, 0f);
        bgRt.offsetMin = new Vector2(borderPx, bottomPx - 1f);
        bgRt.offsetMax = new Vector2(-borderPx, bottomPx + barHeight + 1f);

        var barGo = new GameObject("DurabilityBar");
        barGo.transform.SetParent(transform, false);
        durabilityBar = barGo.AddComponent<Image>();
        durabilityBar.color = new Color(0.2f, 0.8f, 0.2f, 1f);
        durabilityBar.raycastTarget = false;

        var barRt = (RectTransform)durabilityBar.transform;
        barRt.anchorMin = new Vector2(0f, 0f);
        barRt.anchorMax = new Vector2(1f, 0f);
        barRt.pivot = new Vector2(0f, 0f);
        barRt.offsetMin = new Vector2(borderPx + 1f, bottomPx);
        barRt.offsetMax = new Vector2(-borderPx - 1f, bottomPx + barHeight);

        durabilityBarBg.enabled = false;
        durabilityBar.enabled = false;
        ApplyStatusBarAlpha(0f);
    }

    private void UpdateDurabilityBar(InventoryItem item, ItemData itemData)
    {
        if (durabilityBar == null || durabilityBarBg == null)
        {
            return;
        }

        if (!ToolRuntimeUtility.TryGetToolStatusRatio(item, itemData, out float percent, out bool usesWater))
        {
            statusBarHasData = false;
            statusBarTargetAlpha = 0f;
            if (statusBarAlpha <= 0.001f)
            {
                durabilityBarBg.enabled = false;
                durabilityBar.enabled = false;
            }
            return;
        }

        bool shouldShow = ShouldShowStatusBar();
        statusBarHasData = true;
        statusBarTargetAlpha = shouldShow ? 1f : 0f;

        durabilityBarBg.enabled = true;
        durabilityBar.enabled = true;

        float widthPercent = Mathf.Clamp01(percent);
        var barRt = (RectTransform)durabilityBar.transform;
        float maxWidth = 56f - 2f;
        barRt.sizeDelta = new Vector2(maxWidth * widthPercent, barRt.sizeDelta.y);

        Color barColor;
        if (usesWater)
        {
            barColor = Color.Lerp(new Color(0.23f, 0.44f, 0.98f, 1f), new Color(0.29f, 0.86f, 1f, 1f), widthPercent);
        }
        else if (percent > 0.5f)
        {
            float t = (percent - 0.5f) * 2f;
            barColor = Color.Lerp(Color.yellow, Color.green, t);
        }
        else
        {
            float t = percent * 2f;
            barColor = Color.Lerp(Color.red, Color.yellow, t);
        }

        durabilityBar.color = barColor;
        ApplyStatusBarAlpha(statusBarAlpha);
    }

    private void UpdateStatusBarVisibility()
    {
        if (equipment == null || database == null)
        {
            return;
        }

        ItemData itemData = null;
        InventoryItem runtimeItem = null;
        var stack = equipment.GetEquip(index);
        if (!stack.IsEmpty)
        {
            itemData = database.GetItemByID(stack.itemId);
            runtimeItem = equipment.GetEquipItem(index);
        }

        UpdateDurabilityBar(runtimeItem, itemData);
    }

    private bool ShouldShowStatusBar()
    {
        if (inventoryPanel != null && inventoryPanel.gameObject.activeInHierarchy)
        {
            return true;
        }

        return isHovered;
    }

    private void TickStatusBarFade()
    {
        if (durabilityBar == null || durabilityBarBg == null)
        {
            return;
        }

        float targetAlpha = statusBarHasData ? statusBarTargetAlpha : 0f;
        if (Mathf.Approximately(statusBarAlpha, targetAlpha))
        {
            if (targetAlpha <= 0.001f && !statusBarHasData)
            {
                durabilityBarBg.enabled = false;
                durabilityBar.enabled = false;
            }

            return;
        }

        float fadeStep = StatusBarFadeDuration <= 0.0001f
            ? 1f
            : Time.unscaledDeltaTime / StatusBarFadeDuration;
        statusBarAlpha = Mathf.MoveTowards(statusBarAlpha, targetAlpha, fadeStep);
        ApplyStatusBarAlpha(statusBarAlpha);

        if (statusBarAlpha <= 0.001f && targetAlpha <= 0.001f)
        {
            durabilityBarBg.enabled = false;
            durabilityBar.enabled = false;
        }
    }

    private void ApplyStatusBarAlpha(float alpha)
    {
        if (durabilityBarBg == null || durabilityBar == null)
        {
            return;
        }

        Color backgroundColor = durabilityBarBg.color;
        backgroundColor.a = alpha;
        durabilityBarBg.color = backgroundColor;

        Color barColor = durabilityBar.color;
        barColor.a = alpha;
        durabilityBar.color = barColor;
    }
    #endregion
    
    #region 点击事件
    /// <summary>
    /// 基础点击功能 - 仅用于测试
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 装备槽位的真实点击入口统一由 InventorySlotInteraction 处理。
    }
    #endregion
}
