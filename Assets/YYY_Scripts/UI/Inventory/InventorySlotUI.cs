using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FarmGame.Data;
using FarmGame.Data.Core;

/// <summary>
/// 背包槽位 UI - 基础版本
/// 只负责显示物品图标和数量
/// 实现基础的点击功能（选中槽位）
/// 与 ToolbarSlotUI 保持一致的简单设计
///
/// V2 新增：耐久度条显示
/// </summary>
public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text amountText;
    [SerializeField] private Image selectedOverlay;

    // 🔥 V2 新增：耐久度条
    private Image _durabilityBar;
    private Image _durabilityBarBg;
    private float _statusBarAlpha;
    private float _statusBarTargetAlpha;
    private bool _statusBarHasData;
    private const float StatusBarFadeDuration = 0.3f;
    private static readonly Dictionary<int, List<InventorySlotUI>> RegisteredSlots = new Dictionary<int, List<InventorySlotUI>>();

    // 🔥 新增：支持 IItemContainer 接口
    private IItemContainer container;
    private InventoryService inventory;
    private EquipmentService equipment;
    private ItemDatabase database;
    private HotbarSelectionService hotbarSelection;
    private InventoryPanelUI inventoryPanel;
    private FarmGame.UI.BoxPanelUI boxPanel;
    private int index;
    private bool isHotbar;
    private bool isHovered;
    private RectTransform _rectTransform;
    private Vector2 _restAnchoredPosition;
    private Coroutine _rejectShakeCoroutine;
    private const float RejectShakeDuration = 0.18f;
    private const float RejectShakeDistance = 8f;

    /// <summary>
    /// 槽位索引（供外部查询）
    /// </summary>
    public int Index => index;

    /// <summary>
    /// 当前绑定的容器（供外部查询）
    /// </summary>
    public IItemContainer Container => container;

    #region Unity 生命周期
    void Awake()
    {
        _rectTransform = transform as RectTransform;
        CaptureRestAnchoredPosition();

        if (toggle == null) toggle = GetComponent<Toggle>();
        if (toggle != null)
        {
            var navigation = toggle.navigation;
            navigation.mode = Navigation.Mode.None;
            toggle.navigation = navigation;
        }

        // ★ 与 ToolbarSlotUI 保持一致：查找或创建 Icon
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
                iconImage.enabled = false;
            }
        }

        // ★ 与 ToolbarSlotUI 保持一致：查找或创建 Amount
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

        hotbarSelection = FindFirstObjectByType<HotbarSelectionService>();
        inventoryPanel = GetComponentInParent<InventoryPanelUI>(true);
        boxPanel = GetComponentInParent<FarmGame.UI.BoxPanelUI>(true);

        // 🔥 V2 新增：创建耐久度条
        CreateDurabilityBar();

        // ★ 方案 D：自动添加 Interaction 组件
        // 注意：仅关闭 Toggle 自带视觉过渡，避免它和自定义 reject shake 互相打架
        var interaction = gameObject.GetComponent<InventorySlotInteraction>();
        if (interaction == null)
        {
            interaction = gameObject.AddComponent<InventorySlotInteraction>();
        }
        interaction.Bind(this, false);
    }

    void OnEnable()
    {
        CaptureRestAnchoredPosition();
        // 🔥 修复 Ⅱ：只订阅事件，不自动刷新
        // 刷新由外部调用 Bind/BindContainer 时触发
        if (container != null)
        {
            container.OnSlotChanged += OnSlotChanged;
        }
        else if (inventory != null)
        {
            inventory.OnSlotChanged += OnSlotChanged;
        }
        if (isHotbar && hotbarSelection != null)
        {
            hotbarSelection.OnSelectedChanged -= OnHotbarSelectionChanged;
            hotbarSelection.OnSelectedChanged += OnHotbarSelectionChanged;
            RefreshSelection();
        }
        RegisterSlot();
        // 移除 Refresh()，避免使用旧绑定数据
    }

    void Update()
    {
        UpdateStatusBarVisibility();
        TickStatusBarFade();
    }

    void OnDisable()
    {
        UnregisterSlot();
        if (isHotbar && hotbarSelection != null)
        {
            hotbarSelection.OnSelectedChanged -= OnHotbarSelectionChanged;
        }
        if (container != null)
        {
            container.OnSlotChanged -= OnSlotChanged;
        }
        else if (inventory != null)
        {
            inventory.OnSlotChanged -= OnSlotChanged;
        }
    }
    #endregion

    #region 绑定和刷新

    /// <summary>
    /// 绑定到 InventoryService（原有方法，保持兼容）
    /// </summary>
    public void Bind(InventoryService inv, EquipmentService equip, ItemDatabase db, int slotIndex, bool hotbar)
    {
        // 清理旧绑定
        UnbindEvents();
        UnregisterSlot();

        container = inv; // InventoryService 实现了 IItemContainer
        inventory = inv;
        equipment = equip;
        database = db;
        if (hotbarSelection == null) hotbarSelection = FindFirstObjectByType<HotbarSelectionService>();
        inventoryPanel = GetComponentInParent<InventoryPanelUI>(true);
        boxPanel = GetComponentInParent<FarmGame.UI.BoxPanelUI>(true);
        index = slotIndex;
        isHotbar = hotbar;

        if (isActiveAndEnabled)
        {
            if (inventory != null)
            {
                inventory.OnSlotChanged += OnSlotChanged;
            }
            if (isHotbar && hotbarSelection != null)
            {
                hotbarSelection.OnSelectedChanged -= OnHotbarSelectionChanged;
                hotbarSelection.OnSelectedChanged += OnHotbarSelectionChanged;
            }
            RegisterSlot();
            Refresh();
            RefreshSelection();
        }
    }

    /// <summary>
    /// 🔥 新增：绑定到 IItemContainer（支持 ChestInventory）
    /// </summary>
    public void BindContainer(IItemContainer cont, int slotIndex)
    {
        // 清理旧绑定
        UnbindEvents();
        UnregisterSlot();

        // 🔥 修复 Ⅰ：强制清空显示，避免显示旧数据
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
        if (amountText != null)
        {
            amountText.text = "";
        }

        container = cont;
        inventory = cont as InventoryService; // 如果是 InventoryService，保留引用
        equipment = null;
        database = cont?.Database;
        inventoryPanel = GetComponentInParent<InventoryPanelUI>(true);
        boxPanel = GetComponentInParent<FarmGame.UI.BoxPanelUI>(true);
        index = slotIndex;
        isHotbar = false;

        if (isActiveAndEnabled)
        {
            if (container != null)
            {
                container.OnSlotChanged += OnSlotChanged;
            }
            RegisterSlot();
            Refresh();
            RefreshSelection();
        }
    }

    private void ApplySelectionVisual(bool isSelected)
    {
        if (selectedOverlay != null)
        {
            selectedOverlay.enabled = isSelected;
        }
    }

    /// <summary>
    /// 清理事件绑定
    /// </summary>
    private void UnbindEvents()
    {
        if (container != null)
        {
            container.OnSlotChanged -= OnSlotChanged;
        }
        else if (inventory != null)
        {
            inventory.OnSlotChanged -= OnSlotChanged;
        }
    }

    void OnSlotChanged(int idx)
    {
        if (idx == index) Refresh();
    }

    private void OnHotbarSelectionChanged(int selectedIndex)
    {
        RefreshSelection();
    }

    public void RefreshSelection()
    {
        bool isSelected = false;

        if (boxPanel != null && boxPanel.IsOpen)
        {
            if (container is ChestInventory || container is ChestInventoryV2)
            {
                isSelected = boxPanel.IsChestSlotSelected(index);
            }
            else if (container is InventoryService)
            {
                isSelected = boxPanel.IsInventorySlotSelected(index);
            }
        }
        else if (inventoryPanel != null && inventoryPanel.gameObject.activeInHierarchy)
        {
            isSelected = inventoryPanel.IsInventorySlotSelected(index);
        }
        else if (isHotbar && hotbarSelection != null)
        {
            isSelected = hotbarSelection.selectedIndex == index;
        }
        else if (toggle != null)
        {
            isSelected = toggle.isOn;
        }

        if (toggle != null)
        {
#if UNITY_2021_2_OR_NEWER
            toggle.SetIsOnWithoutNotify(isSelected);
#else
            toggle.isOn = isSelected;
#endif
        }

        ApplySelectionVisual(isSelected);
        UpdateStatusBarVisibility();
    }

    public void Refresh()
    {
        if (container == null || database == null)
        {
            return;
        }

        var s = container.GetSlot(index);

        if (s.IsEmpty)
        {
            if (iconImage != null) UIItemIconScaler.SetIconWithAutoScale(iconImage, null, null);
            if (amountText != null) amountText.text = "";
            // 隐藏耐久度条
            UpdateDurabilityBar(null, null);
        }
        else
        {
            var data = database.GetItemByID(s.itemId);
            if (iconImage != null)
            {
                UIItemIconScaler.SetIconWithAutoScale(iconImage, data?.GetBagSprite(), data);
            }
            if (amountText != null)
            {
                amountText.text = s.amount > 1 ? s.amount.ToString() : "";
            }

            // 🔥 V2 新增：更新耐久度条
            // 尝试获取 InventoryItem 以读取耐久度
            InventoryItem invItem = null;
            if (inventory != null)
            {
                invItem = inventory.GetInventoryItem(index);
            }
            UpdateDurabilityBar(invItem, data);
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

    #region 耐久度条

    /// <summary>
    /// 创建耐久度条 UI（代码动态生成，无需美术资源）
    /// Rule: P2-1 耐久度条样式 - 距离底部 6px，贴着 4px 边框，加 1px 黑色描边
    /// </summary>
    private void CreateDurabilityBar()
    {
        // 检查是否已存在
        var existing = transform.Find("DurabilityBar");
        if (existing != null)
        {
            _durabilityBar = existing.GetComponent<Image>();
            var bgTransform = transform.Find("DurabilityBarBg");
            if (bgTransform != null) _durabilityBarBg = bgTransform.GetComponent<Image>();
            return;
        }

        // 🔥 P2-1：计算位置参数
        // 槽位边框 4px，耐久度条距离底部 6px
        // 使用像素偏移而非锚点百分比，确保精确定位
        float borderPx = 4f;
        float bottomPx = 6f;
        float barHeight = 4f; // 耐久度条高度

        // 创建背景条（深灰色 + 1px 黑色描边效果）
        var bgGo = new GameObject("DurabilityBarBg");
        bgGo.transform.SetParent(transform, false);
        _durabilityBarBg = bgGo.AddComponent<Image>();
        _durabilityBarBg.color = new Color(0.1f, 0.1f, 0.1f, 1f); // 黑色描边背景
        _durabilityBarBg.raycastTarget = false;

        var bgRt = (RectTransform)_durabilityBarBg.transform;
        // 使用绝对定位：左右贴着边框，底部距离 6px
        bgRt.anchorMin = new Vector2(0, 0);
        bgRt.anchorMax = new Vector2(1, 0);
        bgRt.pivot = new Vector2(0.5f, 0);
        // offsetMin.x = 左边距, offsetMin.y = 底部距离
        // offsetMax.x = -右边距, offsetMax.y = 底部距离 + 高度
        bgRt.offsetMin = new Vector2(borderPx, bottomPx - 1f); // -1 是描边
        bgRt.offsetMax = new Vector2(-borderPx, bottomPx + barHeight + 1f); // +1 是描边

        // 创建前景条（绿色）
        var barGo = new GameObject("DurabilityBar");
        barGo.transform.SetParent(transform, false);
        _durabilityBar = barGo.AddComponent<Image>();
        _durabilityBar.color = new Color(0.2f, 0.8f, 0.2f, 1f); // 绿色
        _durabilityBar.raycastTarget = false;

        var barRt = (RectTransform)_durabilityBar.transform;
        barRt.anchorMin = new Vector2(0, 0);
        barRt.anchorMax = new Vector2(1, 0);
        barRt.pivot = new Vector2(0, 0); // 左下角对齐，方便缩放
        // 前景条比背景条小 1px（描边效果）
        barRt.offsetMin = new Vector2(borderPx + 1f, bottomPx);
        barRt.offsetMax = new Vector2(-borderPx - 1f, bottomPx + barHeight);

        // 默认隐藏
        _durabilityBarBg.enabled = false;
        _durabilityBar.enabled = false;
        ApplyStatusBarAlpha(0f);
    }

    /// <summary>
    /// 更新耐久度条显示
    /// Rule: P0-2 BoxUI 交互 - 支持从 IItemContainer 获取 InventoryItem
    /// Rule: P2-1 耐久度条样式 - 使用像素偏移控制宽度
    /// </summary>
    private void UpdateDurabilityBar(InventoryItem item, ItemData itemData)
    {
        if (_durabilityBar == null || _durabilityBarBg == null) return;

        // 🔥 修复：如果 item 为 null，尝试从 container 获取
        if (item == null && container != null)
        {
            // 尝试从 ChestInventoryV2 获取
            if (container is ChestInventoryV2 chestInv)
            {
                item = chestInv.GetItem(index);
            }
            // 尝试从 InventoryService 获取
            else if (container is InventoryService invService)
            {
                item = invService.GetInventoryItem(index);
            }
        }

        if (!ToolRuntimeUtility.TryGetToolStatusRatio(item, itemData, out float percent, out bool usesWater))
        {
            _statusBarHasData = false;
            _statusBarTargetAlpha = 0f;
            if (_statusBarAlpha <= 0.001f)
            {
                _durabilityBarBg.enabled = false;
                _durabilityBar.enabled = false;
            }
            return;
        }

        bool shouldShow = ShouldShowStatusBar();
        _statusBarHasData = true;
        _statusBarTargetAlpha = shouldShow ? 1f : 0f;
        _durabilityBarBg.enabled = true;
        _durabilityBar.enabled = true;

        // 🔥 P2-1：使用像素偏移控制宽度
        var rt = (RectTransform)_durabilityBar.transform;
        var bgRt = (RectTransform)_durabilityBarBg.transform;

        // 获取背景条的实际宽度（减去描边）
        float bgWidth = bgRt.rect.width - 2f; // 左右各 1px 描边
        float barWidth = bgWidth * percent;

        // 更新前景条的右边界
        // offsetMax.x 是相对于右锚点的偏移，负值表示向左收缩
        float borderPx = 4f;
        float rightOffset = -borderPx - 1f - (bgWidth - barWidth);
        rt.offsetMax = new Vector2(rightOffset, rt.offsetMax.y);

        // 根据耐久度百分比改变颜色
        // 100%-50%: 绿色 -> 黄色
        // 50%-0%: 黄色 -> 红色
        Color barColor;
        if (usesWater)
        {
            barColor = Color.Lerp(new Color(0.06f, 0.42f, 0.92f, 1f), new Color(0.30f, 0.86f, 0.98f, 1f), percent);
        }
        else if (percent > 0.5f)
        {
            // 绿色到黄色
            float t = (percent - 0.5f) * 2f;
            barColor = Color.Lerp(Color.yellow, new Color(0.2f, 0.8f, 0.2f), t);
        }
        else
        {
            // 黄色到红色
            float t = percent * 2f;
            barColor = Color.Lerp(Color.red, Color.yellow, t);
        }
        _durabilityBar.color = barColor;
        ApplyStatusBarAlpha(_statusBarAlpha);
    }

    private void UpdateStatusBarVisibility()
    {
        if (container == null || database == null)
        {
            return;
        }

        ItemData itemData = null;
        InventoryItem runtimeItem = null;

        var stack = container.GetSlot(index);
        if (!stack.IsEmpty)
        {
            itemData = database.GetItemByID(stack.itemId);
            if (container is InventoryService invService)
            {
                runtimeItem = invService.GetInventoryItem(index);
            }
            else if (container is ChestInventoryV2 chestInventory)
            {
                runtimeItem = chestInventory.GetItem(index);
            }
        }

        UpdateDurabilityBar(runtimeItem, itemData);
    }

    private bool ShouldShowStatusBar()
    {
        if (inventoryPanel != null && inventoryPanel.gameObject.activeInHierarchy)
        {
            return true;
        }

        if (isHovered)
        {
            return true;
        }

        return isHotbar &&
               hotbarSelection != null &&
               (hotbarSelection.selectedIndex == index || ToolRuntimeUtility.WasSlotUsedRecently(index));
    }

    private void TickStatusBarFade()
    {
        if (_durabilityBar == null || _durabilityBarBg == null)
        {
            return;
        }

        float targetAlpha = _statusBarHasData ? _statusBarTargetAlpha : 0f;
        if (Mathf.Approximately(_statusBarAlpha, targetAlpha))
        {
            if (targetAlpha <= 0.001f && !_statusBarHasData)
            {
                _durabilityBarBg.enabled = false;
                _durabilityBar.enabled = false;
            }
            return;
        }

        float fadeStep = StatusBarFadeDuration <= 0.0001f
            ? 1f
            : Time.unscaledDeltaTime / StatusBarFadeDuration;
        _statusBarAlpha = Mathf.MoveTowards(_statusBarAlpha, targetAlpha, fadeStep);
        ApplyStatusBarAlpha(_statusBarAlpha);

        if (_statusBarAlpha <= 0.001f && targetAlpha <= 0.001f)
        {
            _durabilityBarBg.enabled = false;
            _durabilityBar.enabled = false;
        }
    }

    private void ApplyStatusBarAlpha(float alpha)
    {
        if (_durabilityBarBg == null || _durabilityBar == null)
        {
            return;
        }

        Color backgroundColor = _durabilityBarBg.color;
        backgroundColor.a = alpha;
        _durabilityBarBg.color = backgroundColor;

        Color barColor = _durabilityBar.color;
        barColor.a = alpha;
        _durabilityBar.color = barColor;
    }

    #endregion
    #endregion

    #region 点击事件
    /// <summary>
    /// 基础点击功能 - 仅用于测试和选中槽位
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 🔥 P1：移除高频调用的日志输出（符合日志规范）
            // Toggle 会自动管理选中状态，不需要手动切换
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    /// <summary>
    /// 选中此槽位（设置 Toggle.isOn = true）
    /// </summary>
    public void Select()
    {
        if (boxPanel != null && boxPanel.IsOpen)
        {
            if (container is ChestInventory || container is ChestInventoryV2)
            {
                boxPanel.SetSelectedChestIndex(index);
                return;
            }

            if (container is InventoryService)
            {
                boxPanel.SetSelectedInventoryIndex(index, isHotbar);
                return;
            }
        }

        if (container is InventoryService &&
            inventoryPanel != null &&
            inventoryPanel.gameObject.activeInHierarchy)
        {
            inventoryPanel.SetSelectedInventoryIndex(index, isHotbar);
            return;
        }

        if (toggle != null)
        {
            toggle.isOn = true;
        }
        else
        {
            ApplySelectionVisual(true);
        }
    }

    /// <summary>
    /// 取消选中此槽位（设置 Toggle.isOn = false）
    /// </summary>
    public void Deselect()
    {
        if (toggle != null)
        {
            toggle.isOn = false;
        }
        else
        {
            ApplySelectionVisual(false);
        }
    }

    public void ClearSelectionState()
    {
        if (boxPanel != null && boxPanel.IsOpen)
        {
            if (container is ChestInventory || container is ChestInventoryV2)
            {
                boxPanel.ClearUpSelections();
                return;
            }

            if (container is InventoryService)
            {
                boxPanel.ClearDownSelections();
                return;
            }
        }

        if (container is InventoryService &&
            inventoryPanel != null &&
            inventoryPanel.gameObject.activeInHierarchy)
        {
            inventoryPanel.ClearUpSelection();
            return;
        }

        Deselect();
    }

    public void PlayRejectShake()
    {
        CaptureRestAnchoredPosition();
        if (_rectTransform == null)
        {
            return;
        }

        if (_rejectShakeCoroutine != null)
        {
            StopCoroutine(_rejectShakeCoroutine);
            _rectTransform.anchoredPosition = _restAnchoredPosition;
        }

        _rejectShakeCoroutine = StartCoroutine(RejectShakeCoroutine());
    }

    public static void PlayRejectShakeAt(int slotIndex)
    {
        if (!RegisteredSlots.TryGetValue(slotIndex, out var slots) || slots == null)
        {
            return;
        }

        foreach (var slot in slots)
        {
            if (slot != null && slot.isActiveAndEnabled)
            {
                slot.PlayRejectShake();
            }
        }
    }

    private void CaptureRestAnchoredPosition()
    {
        if (_rectTransform == null)
        {
            _rectTransform = transform as RectTransform;
        }

        if (_rectTransform != null && _rejectShakeCoroutine == null)
        {
            _restAnchoredPosition = _rectTransform.anchoredPosition;
        }
    }

    private void RegisterSlot()
    {
        if (container is InventoryService && index >= 0)
        {
            if (!RegisteredSlots.TryGetValue(index, out var slots) || slots == null)
            {
                slots = new List<InventorySlotUI>();
                RegisteredSlots[index] = slots;
            }

            if (!slots.Contains(this))
            {
                slots.Add(this);
            }
        }
    }

    private void UnregisterSlot()
    {
        if (index < 0 || !RegisteredSlots.TryGetValue(index, out var slots) || slots == null)
        {
            return;
        }

        slots.Remove(this);
        if (slots.Count == 0)
        {
            RegisteredSlots.Remove(index);
        }
    }

    private IEnumerator RejectShakeCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < RejectShakeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = elapsed / RejectShakeDuration;
            float damping = 1f - progress;
            float offset = Mathf.Sin(progress * Mathf.PI * 6f) * RejectShakeDistance * damping;
            _rectTransform.anchoredPosition = _restAnchoredPosition + new Vector2(offset, 0f);
            yield return null;
        }

        _rectTransform.anchoredPosition = _restAnchoredPosition;
        _rejectShakeCoroutine = null;
    }
    #endregion
}
