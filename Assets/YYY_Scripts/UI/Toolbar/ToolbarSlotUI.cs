using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using FarmGame.Data;
using FarmGame.Data.Core;
using FarmGame.UI;

public class ToolbarSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private static readonly Dictionary<int, ToolbarSlotUI> RegisteredSlots = new Dictionary<int, ToolbarSlotUI>();

    [SerializeField] private Image iconImage;
    [SerializeField] private Text amountText;
    [SerializeField] private Image selectedOverlay;
    [SerializeField] private Toggle toggle;
    
    // 🔥 V2 新增：耐久度条
    private Image _durabilityBar;
    private Image _durabilityBarBg;

    private InventoryService inventory;
    private ItemDatabase database;
    private HotbarSelectionService selection;
    private int index = -1;
    private RectTransform _rectTransform;
    private Vector2 _restAnchoredPosition;
    private Coroutine _rejectShakeCoroutine;
    private const float RejectShakeDuration = 0.18f;
    private const float RejectShakeDistance = 8f;

    void Awake()
    {
        _rectTransform = transform as RectTransform;
        CaptureRestAnchoredPosition();

        if (toggle == null) toggle = GetComponent<Toggle>();
        if (iconImage == null)
        {
            var t = transform.Find("Icon");
            if (t) iconImage = t.GetComponent<Image>();
            else
            {
                var go = new GameObject("Icon");
                go.transform.SetParent(transform, false);
                iconImage = go.AddComponent<Image>();
                iconImage.raycastTarget = false;
                var rt = (RectTransform)iconImage.transform;
                rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
                iconImage.enabled = false;
            }
        }
        if (amountText == null)
        {
            var t = transform.Find("Amount");
            if (t) 
            {
                amountText = t.GetComponent<Text>();
            }
            else
            {
                // ★ 自动创建 Amount（与 InventorySlotUI 保持一致）
                var go = new GameObject("Amount");
                go.transform.SetParent(transform, false);
                amountText = go.AddComponent<Text>();
                amountText.raycastTarget = false;
                
                // 字体设置
                amountText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                amountText.fontSize = 18;
                amountText.fontStyle = FontStyle.BoldAndItalic;
                amountText.color = Color.black;
                amountText.alignment = TextAnchor.LowerRight;
                amountText.text = "";
                
                var rt = (RectTransform)amountText.transform;
                // 自定义锚点（全拉伸）
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.pivot = new Vector2(0.5f, 0.5f);
                // ★ 用户指定参数：左21.2356，顶部41.568，右3.8808，底部0
                rt.offsetMin = new Vector2(21.2356f, 0f);      // left, bottom
                rt.offsetMax = new Vector2(-3.8808f, -41.568f); // -right, -top
            }
        }
        if (selectedOverlay == null)
        {
            var t = transform.Find("Selected");
            if (t) selectedOverlay = t.GetComponent<Image>();
        }

        // 让 Toggle 不影响底图着色：targetGraphic设为null，transition=None，避免Toggle自身视觉反馈
        // 选中红框完全由 selectedOverlay 控制
        if (toggle != null)
        {
            var navigation = toggle.navigation;
            navigation.mode = Navigation.Mode.None;
            toggle.navigation = navigation;
        }
        
        // 🔥 V2 新增：创建耐久度条
        CreateDurabilityBar();
    }

    void OnEnable()
    {
        CaptureRestAnchoredPosition();
        RegisterSlot();

        if (inventory != null)
        {
            inventory.OnHotbarSlotChanged += HandleHotbarChanged;
            inventory.OnSlotChanged += HandleAnySlotChanged;
        }
        if (selection != null)
        {
            selection.OnSelectedChanged -= HandleSelectionChanged;
            selection.OnSelectedChanged += HandleSelectionChanged;
        }
        
        // 注册 Toggle 的 OnValueChanged 事件，用于锁定状态下拦截
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        
        Refresh();
        RefreshSelection();
    }

    void OnDisable()
    {
        UnregisterSlot();

        if (inventory != null)
        {
            inventory.OnHotbarSlotChanged -= HandleHotbarChanged;
            inventory.OnSlotChanged -= HandleAnySlotChanged;
        }
        if (selection != null)
        {
            selection.OnSelectedChanged -= HandleSelectionChanged;
        }
        
        // 移除 Toggle 事件监听
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
    
    /// <summary>
    /// Toggle 值变化时的处理
    /// 在锁定状态下立即恢复状态，阻止视觉变化
    /// </summary>
    private void OnToggleValueChanged(bool isOn)
    {
        var lockManager = ToolActionLockManager.Instance;
        if (lockManager != null && lockManager.IsLocked)
        {
            // 锁定状态：立即恢复到正确的选中状态
            ForceRestoreToggleState();
        }
    }

    public void Bind(InventoryService inv, ItemDatabase db, HotbarSelectionService sel, int hotbarIndex)
    {
        inventory = inv;
        database = db;
        selection = sel;
        index = hotbarIndex;
        CaptureRestAnchoredPosition();
        RegisterSlot();
        if (isActiveAndEnabled)
        {
            OnDisable();
            OnEnable();
        }
        else
        {
            Refresh();
            RefreshSelection();
        }
    }

    void HandleHotbarChanged(int changedIndex)
    {
        if (changedIndex == index) Refresh();
    }

    void HandleAnySlotChanged(int changedIndex)
    {
        if (changedIndex == index) Refresh();
    }

    void HandleSelectionChanged(int idx)
    {
        RefreshSelection();
    }

    public void Refresh()
    {
        if (inventory == null || database == null) return;
        var s = inventory.GetSlot(index);
        if (s.IsEmpty)
        {
            // 使用统一的缩放适配工具清除图标
            if (iconImage) UIItemIconScaler.SetIconWithAutoScale(iconImage, null, null);
            if (amountText) amountText.text = "";
            // 隐藏耐久度条
            UpdateDurabilityBar(null);
            return;
        }
        var data = database.GetItemByID(s.itemId);
        // ✅ 使用统一的缩放适配工具设置图标（支持自定义旋转和尺寸）
        if (iconImage)
        {
            UIItemIconScaler.SetIconWithAutoScale(iconImage, data != null ? data.GetBagSprite() : null, data);
        }
        if (amountText)
        {
            amountText.text = s.amount > 1 ? s.amount.ToString() : "";
        }
        
        // 🔥 V2 新增：更新耐久度条
        var invItem = inventory.GetInventoryItem(index);
        UpdateDurabilityBar(invItem);
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
        float borderPx = 4f;
        float bottomPx = 6f;
        float barHeight = 4f;
        
        // 创建背景条（黑色描边背景）
        var bgGo = new GameObject("DurabilityBarBg");
        bgGo.transform.SetParent(transform, false);
        _durabilityBarBg = bgGo.AddComponent<Image>();
        _durabilityBarBg.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        _durabilityBarBg.raycastTarget = false;
        
        var bgRt = (RectTransform)_durabilityBarBg.transform;
        bgRt.anchorMin = new Vector2(0, 0);
        bgRt.anchorMax = new Vector2(1, 0);
        bgRt.pivot = new Vector2(0.5f, 0);
        bgRt.offsetMin = new Vector2(borderPx, bottomPx - 1f);
        bgRt.offsetMax = new Vector2(-borderPx, bottomPx + barHeight + 1f);
        
        // 创建前景条（绿色）
        var barGo = new GameObject("DurabilityBar");
        barGo.transform.SetParent(transform, false);
        _durabilityBar = barGo.AddComponent<Image>();
        _durabilityBar.color = new Color(0.2f, 0.8f, 0.2f, 1f);
        _durabilityBar.raycastTarget = false;
        
        var barRt = (RectTransform)_durabilityBar.transform;
        barRt.anchorMin = new Vector2(0, 0);
        barRt.anchorMax = new Vector2(1, 0);
        barRt.pivot = new Vector2(0, 0);
        barRt.offsetMin = new Vector2(borderPx + 1f, bottomPx);
        barRt.offsetMax = new Vector2(-borderPx - 1f, bottomPx + barHeight);
        
        // 默认隐藏
        _durabilityBarBg.enabled = false;
        _durabilityBar.enabled = false;
    }
    
    /// <summary>
    /// 更新耐久度条显示
    /// Rule: P2-1 耐久度条样式 - 使用像素偏移控制宽度
    /// </summary>
    private void UpdateDurabilityBar(InventoryItem item)
    {
        if (_durabilityBar == null || _durabilityBarBg == null) return;
        
        // 如果物品为空或没有耐久度，隐藏耐久度条
        if (item == null || !item.HasDurability)
        {
            _durabilityBarBg.enabled = false;
            _durabilityBar.enabled = false;
            return;
        }
        
        // 显示耐久度条
        _durabilityBarBg.enabled = true;
        _durabilityBar.enabled = true;
        
        // 计算耐久度百分比
        float percent = item.DurabilityPercent;
        
        // 🔥 P2-1：使用像素偏移控制宽度
        var rt = (RectTransform)_durabilityBar.transform;
        var bgRt = (RectTransform)_durabilityBarBg.transform;
        
        float bgWidth = bgRt.rect.width - 2f;
        float barWidth = bgWidth * percent;
        
        float borderPx = 4f;
        float rightOffset = -borderPx - 1f - (bgWidth - barWidth);
        rt.offsetMax = new Vector2(rightOffset, rt.offsetMax.y);
        
        // 根据耐久度百分比改变颜色
        Color barColor;
        if (percent > 0.5f)
        {
            float t = (percent - 0.5f) * 2f;
            barColor = Color.Lerp(Color.yellow, new Color(0.2f, 0.8f, 0.2f), t);
        }
        else
        {
            float t = percent * 2f;
            barColor = Color.Lerp(Color.red, Color.yellow, t);
        }
        _durabilityBar.color = barColor;
    }
    
    #endregion

    public void RefreshSelection()
    {
        bool sel = selection != null && selection.selectedIndex == index;
        // 1) 更新选中红框（overlay）
        if (selectedOverlay) selectedOverlay.enabled = sel;
        // 2) 更新Toggle.isOn状态以配合ToggleGroup联动（但不触发事件）
        if (toggle != null)
        {
#if UNITY_2021_2_OR_NEWER
            toggle.SetIsOnWithoutNotify(sel);
#else
            bool prev = toggle.isOn;
            toggle.isOn = sel;
            // 低版本无SetIsOnWithoutNotify，需要手动避免事件（但ToggleGroup会处理）
#endif
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

            // ★ 检查是否有面板打开（背包/箱子）- 被遮挡时不响应
            var packageTabs = FindFirstObjectByType<PackagePanelTabsUI>();
            if (packageTabs != null && packageTabs.IsPanelOpen()) 
            {
                ForceRestoreToggleState();
                return;
            }
            if (BoxPanelUI.ActiveInstance != null && BoxPanelUI.ActiveInstance.IsOpen) 
            {
                ForceRestoreToggleState();
                return;
            }

            if (GameInputManager.Instance != null && GameInputManager.Instance.TryRejectActiveFarmToolSwitch(index))
            {
                ForceRestoreToggleState();
                return;
            }
            
            // 检查是否处于工具动作锁定状态
            var lockManager = ToolActionLockManager.Instance;
            if (lockManager != null && lockManager.IsLocked)
            {
                // 锁定状态：缓存输入而非立即切换
                lockManager.CacheHotbarInput(index);
                
                // 重要：强制恢复 Toggle 状态，防止视觉变化
                // Toggle 的 OnValueChanged 可能已经触发，需要立即恢复
                ForceRestoreToggleState();
                return;
            }
            
            selection?.SelectIndex(index);
            RefreshSelection();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SlotDragContext.IsDragging || (InventoryInteractionManager.Instance != null && InventoryInteractionManager.Instance.IsHolding))
        {
            return;
        }

        if (inventory == null || database == null || index < 0)
        {
            return;
        }

        var stack = inventory.GetSlot(index);
        if (stack.IsEmpty)
        {
            ItemTooltip.Instance?.Hide();
            return;
        }

        var itemData = database.GetItemByID(stack.itemId);
        if (itemData == null)
        {
            ItemTooltip.Instance?.Hide();
            return;
        }

        ItemTooltip.Instance?.Show(itemData, stack, inventory.GetInventoryItem(index), stack.amount);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltip.Instance?.Hide();
    }
    
    /// <summary>
    /// 强制恢复 Toggle 状态到当前选中的槽位
    /// 用于锁定状态下阻止视觉变化
    /// </summary>
    private void ForceRestoreToggleState()
    {
        if (toggle == null || selection == null) return;
        
        bool shouldBeSelected = selection.selectedIndex == index;
        
        // 使用 SetIsOnWithoutNotify 避免触发事件
#if UNITY_2021_2_OR_NEWER
        toggle.SetIsOnWithoutNotify(shouldBeSelected);
#else
        toggle.isOn = shouldBeSelected;
#endif
        
        // 同时更新选中覆盖层
        if (selectedOverlay != null)
            selectedOverlay.enabled = shouldBeSelected;
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

    public static void PlayRejectShakeAt(int hotbarIndex)
    {
        if (RegisteredSlots.TryGetValue(hotbarIndex, out var slot) && slot != null)
        {
            slot.PlayRejectShake();
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
        if (index >= 0)
        {
            RegisteredSlots[index] = this;
        }
    }

    private void UnregisterSlot()
    {
        if (index >= 0 && RegisteredSlots.TryGetValue(index, out var slot) && slot == this)
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
}
