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
    private float _statusBarAlpha;
    private float _statusBarTargetAlpha;
    private bool _statusBarHasData;
    private const float StatusBarFadeDuration = 0.3f;

    private InventoryService inventory;
    private ItemDatabase database;
    private HotbarSelectionService selection;
    private InventoryService subscribedInventory;
    private HotbarSelectionService subscribedSelection;
    private int index = -1;
    private bool isHovered;
    private RectTransform _rectTransform;
    private Vector2 _restAnchoredPosition;
    private Coroutine _rejectShakeCoroutine;
    private const float RejectShakeDuration = 0.18f;
    private const float RejectShakeDistance = 8f;

    void Awake()
    {
        _rectTransform = transform as RectTransform;
        CaptureRestAnchoredPosition();
        ResolveRuntimeContextIfMissing();

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
        ResolveRuntimeContextIfMissing();
        SyncRuntimeSubscriptions();
        
        // 注册 Toggle 的 OnValueChanged 事件，用于锁定状态下拦截
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
        RefreshTooltipWhileHovered();
    }

    void OnDisable()
    {
        UnregisterSlot();
        SyncRuntimeSubscriptions(forceClear: true);
        
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
            RestoreAllRegisteredSlotVisuals(resetRejectShake: false);
        }
    }

    public void Bind(InventoryService inv, ItemDatabase db, HotbarSelectionService sel, int hotbarIndex)
    {
        inventory = inv;
        database = db;
        selection = sel;
        index = hotbarIndex;
        CaptureRestAnchoredPosition();
        ResolveRuntimeContextIfMissing();
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
        ResolveRuntimeContextIfMissing();
        if (inventory == null || database == null) return;
        var s = inventory.GetSlot(index);
        if (s.IsEmpty)
        {
            // 使用统一的缩放适配工具清除图标
            if (iconImage) UIItemIconScaler.SetIconWithAutoScale(iconImage, null, null);
            if (amountText) amountText.text = "";
            // 隐藏耐久度条
            UpdateDurabilityBar(null, null);
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
        UpdateDurabilityBar(invItem, data);
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
        ApplyStatusBarAlpha(0f);
    }
    
    /// <summary>
    /// 更新耐久度条显示
    /// Rule: P2-1 耐久度条样式 - 使用像素偏移控制宽度
    /// </summary>
    private void UpdateDurabilityBar(InventoryItem item, ItemData itemData)
    {
        if (_durabilityBar == null || _durabilityBarBg == null) return;
        
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
        
        float bgWidth = bgRt.rect.width - 2f;
        float barWidth = bgWidth * percent;
        
        float borderPx = 4f;
        float rightOffset = -borderPx - 1f - (bgWidth - barWidth);
        rt.offsetMax = new Vector2(rightOffset, rt.offsetMax.y);
        
        // 根据耐久度百分比改变颜色
        Color barColor;
        if (usesWater)
        {
            barColor = Color.Lerp(new Color(0.06f, 0.42f, 0.92f, 1f), new Color(0.30f, 0.86f, 0.98f, 1f), percent);
        }
        else if (percent > 0.5f)
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
        ApplyStatusBarAlpha(_statusBarAlpha);
    }
    
    #endregion

    public void RefreshSelection()
    {
        ResolveRuntimeContextIfMissing();
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

        UpdateStatusBarVisibility();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

            var inputManager = GameInputManager.Instance;
            if (inputManager != null)
            {
                GameInputManager.ToolbarPointerSelectionChangeResult result =
                    inputManager.TryHandleToolbarPointerSelectionChange(index);

                switch (result)
                {
                    case GameInputManager.ToolbarPointerSelectionChangeResult.Applied:
                        RefreshSelection();
                        return;

                    case GameInputManager.ToolbarPointerSelectionChangeResult.RejectedKeepCurrentSelection:
                    case GameInputManager.ToolbarPointerSelectionChangeResult.DeferredDuringLock:
                        if (selection != null)
                        {
                            selection.ReassertCurrentSelection(collapseInventorySelectionToHotbar: true, invokeEvent: true);
                        }
                        else
                        {
                            RestoreAllRegisteredSlotVisuals(resetRejectShake: false);
                        }
                        return;

                    default:
                        RestoreAllRegisteredSlotVisuals();
                        return;
                }
            }

            selection?.SelectIndex(index);
            if (selection == null || selection.selectedIndex != index)
            {
                RestoreAllRegisteredSlotVisuals();
                return;
            }

            RefreshSelection();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateStatusBarVisibility();
        TryShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateStatusBarVisibility();
        ItemTooltip.Instance?.Hide();
    }

    private void UpdateStatusBarVisibility()
    {
        ResolveRuntimeContextIfMissing();
        if (inventory == null || database == null || index < 0)
        {
            return;
        }

        var stack = inventory.GetSlot(index);
        ItemData itemData = stack.IsEmpty ? null : database.GetItemByID(stack.itemId);
        UpdateDurabilityBar(stack.IsEmpty ? null : inventory.GetInventoryItem(index), itemData);
    }

    private bool ShouldShowStatusBar()
    {
        if (isHovered)
        {
            return true;
        }

        return selection != null &&
               (selection.selectedIndex == index || ToolRuntimeUtility.WasSlotUsedRecently(index));
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

    private static bool ShouldSuppressTooltipHover()
    {
        if (SlotDragContext.IsDragging)
        {
            return true;
        }

        var interactionManager = InventoryInteractionManager.Instance;
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

    private void RefreshTooltipWhileHovered()
    {
        if (!isHovered || ShouldSuppressTooltipHover())
        {
            return;
        }

        TryShowTooltip();
    }

    private void TryShowTooltip()
    {
        if (ShouldSuppressTooltipHover())
        {
            return;
        }

        ResolveRuntimeContextIfMissing();

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

        ItemTooltip.Instance?.Show(itemData, stack, inventory.GetInventoryItem(index), stack.amount, transform);
    }
    
    /// <summary>
    /// 强制恢复 Toggle 状态到当前选中的槽位
    /// 用于锁定状态下阻止视觉变化
    /// </summary>
    private void ForceRestoreToggleState()
    {
        ResolveRuntimeContextIfMissing();
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

    private void ResetRejectShakeVisual()
    {
        CaptureRestAnchoredPosition();
        if (_rectTransform == null)
        {
            return;
        }

        if (_rejectShakeCoroutine != null)
        {
            StopCoroutine(_rejectShakeCoroutine);
            _rejectShakeCoroutine = null;
        }

        _rectTransform.anchoredPosition = _restAnchoredPosition;
    }

    private static void RestoreAllRegisteredSlotVisuals(bool resetRejectShake = true)
    {
        foreach (var pair in RegisteredSlots)
        {
            var slot = pair.Value;
            if (slot == null)
            {
                continue;
            }

            if (resetRejectShake)
            {
                slot.ResetRejectShakeVisual();
            }

            slot.ForceRestoreToggleState();
        }
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

    private void ResolveRuntimeContextIfMissing()
    {
        if (inventory == null)
        {
            inventory = FindFirstObjectByType<InventoryService>();
        }

        if (database == null && inventory != null)
        {
            database = inventory.Database;
        }

        if (selection == null)
        {
            selection = FindFirstObjectByType<HotbarSelectionService>();
        }

        SyncRuntimeSubscriptions();
    }

    private void SyncRuntimeSubscriptions(bool forceClear = false)
    {
        InventoryService desiredInventory = forceClear ? null : inventory;
        if (subscribedInventory != desiredInventory)
        {
            if (subscribedInventory != null)
            {
                subscribedInventory.OnHotbarSlotChanged -= HandleHotbarChanged;
                subscribedInventory.OnSlotChanged -= HandleAnySlotChanged;
            }

            subscribedInventory = desiredInventory;
            if (isActiveAndEnabled && subscribedInventory != null)
            {
                subscribedInventory.OnHotbarSlotChanged -= HandleHotbarChanged;
                subscribedInventory.OnHotbarSlotChanged += HandleHotbarChanged;
                subscribedInventory.OnSlotChanged -= HandleAnySlotChanged;
                subscribedInventory.OnSlotChanged += HandleAnySlotChanged;
            }
        }

        HotbarSelectionService desiredSelection = forceClear ? null : selection;
        if (subscribedSelection == desiredSelection)
        {
            return;
        }

        if (subscribedSelection != null)
        {
            subscribedSelection.OnSelectedChanged -= HandleSelectionChanged;
        }

        subscribedSelection = desiredSelection;
        if (!isActiveAndEnabled || subscribedSelection == null)
        {
            return;
        }

        subscribedSelection.OnSelectedChanged -= HandleSelectionChanged;
        subscribedSelection.OnSelectedChanged += HandleSelectionChanged;
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
