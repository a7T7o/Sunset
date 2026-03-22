using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using FarmGame.Data;
using FarmGame.Data.Core;

/// <summary>
/// 背包交互管理器 - 修复版 v2
/// 核心修改：
/// 1. OnSlotPointerDown 处理所有点击逻辑（包括 Held 状态下的放置）
/// 2. PointerUp 不触发任何放置逻辑
/// 3. Shift/Ctrl 拿起后等待再次点击来放置
/// </summary>
public class InventoryInteractionManager : MonoBehaviour
{
    public static InventoryInteractionManager Instance { get; private set; }
    
    [Header("服务引用")]
    [SerializeField] private InventoryService inventory;
    [SerializeField] private EquipmentService equipment;
    [SerializeField] private ItemDatabase database;
    
    [Header("UI 引用")]
    [SerializeField] private HeldItemDisplay heldDisplay;
    [SerializeField] private RectTransform panelRect;        // PackagePanel（用于旧逻辑兼容）
    [SerializeField] private RectTransform mainRect;         // Main 区域（背包主区域）
    [SerializeField] private RectTransform topRect;          // Top 区域（Tab 栏）
    [SerializeField] private RectTransform trashCanRect;     // 垃圾桶区域
    [SerializeField] private InventorySlotUI[] inventorySlots;  // 36 个背包槽位
    [SerializeField] private EquipmentSlotUI[] equipmentSlots;  // 6 个装备槽位
    
    [Header("按钮引用")]
    [SerializeField] private Button sortButton;              // 整理按钮
    [SerializeField] private InventorySortService sortService; // 整理服务
    
    [Header("配置")]
    [SerializeField] private float ctrlPickupRate = 3.5f;
    [SerializeField] private float dropCooldown = 5f;
    
    [Header("边界检测")]
    [Tooltip("背包实际可见区域（用于丢弃判定）。如果不配置，则使用 Main + Top 区域")]
    [SerializeField] private RectTransform inventoryBoundsRect;  // 背包实际可见区域
    
    // 常量：拖拽目标索引
    private const int DROP_TARGET_NONE = -2;   // 无目标槽位
    private const int DROP_TARGET_TRASH = -1;  // 垃圾桶
    
    // 状态机
    private enum State { Idle, HeldByShift, HeldByCtrl, Dragging }
    private State currentState = State.Idle;
    
    // 拿取数据
    private ItemStack heldItem;
    private InventoryItem heldRuntimeItem;
    private int sourceIndex = -1;
    private bool sourceIsEquip = false;
    
    // 拖拽目标
    private int dropTargetIndex = DROP_TARGET_NONE;
    private bool dropTargetIsEquip = false;
    
    // Ctrl 长按
    private Coroutine ctrlCoroutine;
    
    public bool IsHolding => currentState != State.Idle;
    
    /// <summary>
    /// 获取当前 Held 物品（供外部使用）
    /// </summary>
    public ItemStack GetHeldItem() => heldItem;
    
    /// <summary>
    /// 获取源槽位索引（供外部使用）
    /// </summary>
    public int GetSourceIndex() => sourceIndex;
    
    /// <summary>
    /// 获取源是否为装备槽位（供外部使用）
    /// </summary>
    public bool GetSourceIsEquip() => sourceIsEquip;
    
    /// <summary>
    /// 清空 Held 状态（供外部使用）
    /// </summary>
    public void ClearHeldState()
    {
        if (ctrlCoroutine != null) { StopCoroutine(ctrlCoroutine); ctrlCoroutine = null; }
        ResetState();
    }

    #region 🔥 P0-3：统一 Held 图标接口
    
    /// <summary>
    /// 显示 Held 图标（统一入口）
    /// </summary>
    public void ShowHeldIcon(int itemId, int amount, Sprite icon)
    {
        heldDisplay?.Show(itemId, amount, icon);
    }
    
    /// <summary>
    /// 隐藏 Held 图标（统一入口）
    /// </summary>
    public void HideHeldIcon()
    {
        heldDisplay?.Hide();
    }
    
    #endregion

    
    #region Unity 生命周期
    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        
        if (inventory == null) inventory = FindFirstObjectByType<InventoryService>();
        if (equipment == null) equipment = FindFirstObjectByType<EquipmentService>();
        if (database == null && inventory != null) database = inventory.Database;
        if (sortService == null) sortService = FindFirstObjectByType<InventorySortService>();
        
        // 绑定整理按钮
        if (sortButton != null)
        {
            sortButton.onClick.AddListener(OnSortButtonClick);
        }
    }
    
    void OnDestroy()
    {
        if (sortButton != null)
        {
            sortButton.onClick.RemoveListener(OnSortButtonClick);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && IsHolding)
            Cancel();
    }
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 槽位被点击（PointerDown）- 所有点击逻辑的入口
    /// </summary>
    public void OnSlotPointerDown(int index, bool isEquip)
    {
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        
        // Shift+Ctrl 同时按下忽略
        if (shift && ctrl) return;
        
        // ★ 根据当前状态分发处理
        switch (currentState)
        {
            case State.Idle:
                HandleIdleClick(index, isEquip, shift, ctrl);
                break;
            case State.HeldByShift:
            case State.HeldByCtrl:
                HandleHeldClick(index, isEquip, shift);
                break;
            case State.Dragging:
                // 拖拽中不处理点击
                break;
        }
    }
    
    /// <summary>
    /// 开始拖拽
    /// </summary>
    public void OnSlotBeginDrag(int index, bool isEquip, PointerEventData eventData)
    {
        if (currentState != State.Idle) return;
        ItemTooltip.Instance?.Hide();

        if (TryRejectProtectedHeldInventoryPickup(index, isEquip))
        {
            return;
        }
        
        ItemStack slot = GetSlot(index, isEquip);
        if (slot.IsEmpty) return;
        
        sourceIndex = index;
        sourceIsEquip = isEquip;
        heldItem = slot;
        heldRuntimeItem = GetRuntimeItem(index, isEquip);
        
        // 清空源槽位
        ClearSlot(index, isEquip);
        
        // ★ 重置拖拽目标为"无目标"
        dropTargetIndex = DROP_TARGET_NONE;
        
        currentState = State.Dragging;
        ShowHeld();
    }
    
    /// <summary>
    /// 结束拖拽
    /// </summary>
    public void OnSlotEndDrag(PointerEventData eventData)
    {
        if (currentState != State.Dragging) return;
        
        // ★ 情况 1：拖拽到垃圾桶（dropTargetIndex == DROP_TARGET_TRASH）
        if (dropTargetIndex == DROP_TARGET_TRASH)
        {
            DropItem();
            ResetState();
            return;
        }
        
        // ★ 情况 2：有目标槽位（dropTargetIndex >= 0）
        if (dropTargetIndex >= 0)
        {
            ExecutePlacement(dropTargetIndex, dropTargetIsEquip, true);
            return;
        }
        
        // ★ 情况 3：无目标槽位（dropTargetIndex == DROP_TARGET_NONE）
        // 检查是否在垃圾桶区域（通过位置检测）
        if (IsOverTrashCan(eventData.position))
        {
            DropItem();
            ResetState();
            return;
        }
        
        // 检查是否在面板外
        if (!IsInsidePanel(eventData.position))
        {
            DropItem();
            ResetState();
            return;
        }
        
        // 面板内无目标，返回原位
        ReturnToSource();
        ResetState();
    }
    
    /// <summary>
    /// 拖拽经过槽位
    /// </summary>
    public void OnSlotDrop(int index, bool isEquip)
    {
        dropTargetIndex = index;
        dropTargetIsEquip = isEquip;
    }
    
    public void Cancel()
    {
        if (ctrlCoroutine != null) { StopCoroutine(ctrlCoroutine); ctrlCoroutine = null; }
        ReturnToSource();
        ResetState();
        HideHeldIcon();  // 🔥 P0-3：确保隐藏图标
    }
    
    /// <summary>
    /// 🔥 P1+-1：物品归位逻辑（关闭 UI 时调用）
    /// 优先级：原槽位 → 背包空位 → 扔在脚下
    /// 绝对禁止销毁物品！
    /// </summary>
    public void ReturnHeldItemToInventory()
    {
        if (!IsHolding || heldItem.IsEmpty) return;
        
        // 停止 Ctrl 长按
        if (ctrlCoroutine != null) { StopCoroutine(ctrlCoroutine); ctrlCoroutine = null; }
        
        // 1. 尝试返回原槽位
        ItemStack src = GetSlot(sourceIndex, sourceIsEquip);
        if (src.IsEmpty)
        {
            SetSlot(sourceIndex, sourceIsEquip, heldItem, heldRuntimeItem);
            Debug.Log($"[InventoryInteractionManager] 物品归位：返回原槽位 {sourceIndex}");
            ResetState();
            return;
        }
        
        // 2. 原槽位有物品，尝试堆叠
        if (src.itemId == heldItem.itemId && src.quality == heldItem.quality)
        {
            var itemData = database?.GetItemByID(heldItem.itemId);
            int maxStack = itemData != null ? itemData.maxStackSize : 99;
            int total = src.amount + heldItem.amount;
            
            if (total <= maxStack)
            {
                SetSlot(sourceIndex, sourceIsEquip, new ItemStack { itemId = src.itemId, quality = src.quality, amount = total });
                Debug.Log($"[InventoryInteractionManager] 物品归位：堆叠到原槽位 {sourceIndex}");
                ResetState();
                return;
            }
            else
            {
                // 部分堆叠
                SetSlot(sourceIndex, sourceIsEquip, new ItemStack { itemId = src.itemId, quality = src.quality, amount = maxStack });
                heldItem.amount = total - maxStack;
                Debug.Log($"[InventoryInteractionManager] 物品归位：部分堆叠到原槽位，剩余 {heldItem.amount}");
            }
        }
        
        // 3. 尝试放入背包空位
        for (int i = 0; i < 36; i++)
        {
            if (inventory.GetSlot(i).IsEmpty)
            {
                SetSlot(i, false, heldItem, heldRuntimeItem);
                Debug.Log($"[InventoryInteractionManager] 物品归位：放入背包空位 {i}");
                ResetState();
                return;
            }
        }
        
        // 4. 背包满了，扔在脚下
        DropItemAtPlayerFeet();
        ResetState();
    }
    
    /// <summary>
    /// 🔥 P1+-1：在玩家脚下丢弃物品
    /// </summary>
    private void DropItemAtPlayerFeet()
    {
        if (heldItem.IsEmpty) return;
        
        Debug.Log($"[InventoryInteractionManager] 物品归位：背包已满，扔在脚下");
        if (heldRuntimeItem != null && !heldRuntimeItem.IsEmpty)
            FarmGame.UI.ItemDropHelper.DropAtPlayer(heldRuntimeItem, dropCooldown);
        else
            FarmGame.UI.ItemDropHelper.DropAtPlayer(heldItem, dropCooldown);
        heldItem = new ItemStack();
        heldRuntimeItem = null;
    }
    
    #endregion

    
    #region Idle 状态处理
    
    private void HandleIdleClick(int index, bool isEquip, bool shift, bool ctrl)
    {
        ItemStack slot = GetSlot(index, isEquip);

        if (!slot.IsEmpty && (shift || ctrl) && TryRejectProtectedHeldInventoryPickup(index, isEquip))
        {
            return;
        }
        
        if (shift && !slot.IsEmpty)
        {
            // Shift+左键：二分拿取
            ShiftPickup(index, isEquip, slot);
        }
        else if (ctrl && !slot.IsEmpty)
        {
            // Ctrl+左键：单个拿取或快速装备
            CtrlPickup(index, isEquip, slot);
        }
        else
        {
            // 🔥 致命修复 2：无修饰键单击 = 只选中槽位，不拿起物品
            // 拿起物品由 OnBeginDrag 处理（需要长按 + 拖动）
            SelectSlot(index, isEquip);
        }
    }
    
    private void ShiftPickup(int index, bool isEquip, ItemStack slot)
    {
        // 🔥 互斥检查：如果 SlotDragContext 正在拖拽，拒绝拿取
        if (SlotDragContext.IsDragging)
        {
            Debug.LogWarning("[InventoryInteractionManager] SlotDragContext 正在拖拽，无法拿起物品");
            return;
        }
        
        // ★ 向上取整：余数归手上，确保手上至少有 1 个
        int handAmount = (slot.amount + 1) / 2;
        int sourceAmount = slot.amount - handAmount;
        
        heldItem = new ItemStack { itemId = slot.itemId, quality = slot.quality, amount = handAmount };
        heldRuntimeItem = slot.amount == handAmount ? GetRuntimeItem(index, isEquip) : null;
        
        // 如果源槽位数量为 0，清空槽位
        if (sourceAmount > 0)
            SetSlot(index, isEquip, new ItemStack { itemId = slot.itemId, quality = slot.quality, amount = sourceAmount });
        else
            ClearSlot(index, isEquip);
        
        sourceIndex = index;
        sourceIsEquip = isEquip;
        currentState = State.HeldByShift;
        
        ShowHeld();
    }
    
    private void CtrlPickup(int index, bool isEquip, ItemStack slot)
    {
        // 🔥 互斥检查：如果 SlotDragContext 正在拖拽，拒绝拿取
        if (SlotDragContext.IsDragging)
        {
            Debug.LogWarning("[InventoryInteractionManager] SlotDragContext 正在拖拽，无法拿起物品");
            return;
        }
        
        // 检查快速装备
        var itemData = database?.GetItemByID(slot.itemId);
        if (itemData != null && itemData.equipmentType != EquipmentType.None && !isEquip)
        {
            QuickEquip(index, slot, itemData);
            return;
        }
        
        heldItem = new ItemStack { itemId = slot.itemId, quality = slot.quality, amount = 1 };
        heldRuntimeItem = slot.amount == 1 ? GetRuntimeItem(index, isEquip) : null;
        
        if (slot.amount > 1)
            SetSlot(index, isEquip, new ItemStack { itemId = slot.itemId, quality = slot.quality, amount = slot.amount - 1 });
        else
            ClearSlot(index, isEquip);
        
        sourceIndex = index;
        sourceIsEquip = isEquip;
        currentState = State.HeldByCtrl;
        
        ShowHeld();
        
        // 启动长按协程
        ctrlCoroutine = StartCoroutine(ContinueCtrlPickup());
    }
    
    private IEnumerator ContinueCtrlPickup()
    {
        float interval = 1f / ctrlPickupRate;
        while (true)
        {
            yield return new WaitForSeconds(interval);
            
            bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool mouse = Input.GetMouseButton(0);
            if (!ctrl || !mouse) break;
            
            ItemStack src = GetSlot(sourceIndex, sourceIsEquip);
            if (src.IsEmpty || src.itemId != heldItem.itemId) break;
            
            heldItem.amount++;
            if (src.amount > 1)
                SetSlot(sourceIndex, sourceIsEquip, new ItemStack { itemId = src.itemId, quality = src.quality, amount = src.amount - 1 });
            else
            {
                ClearSlot(sourceIndex, sourceIsEquip);
                ShowHeld();  // 🔥 修复：break 前调用 ShowHeld() 更新显示
                break;
            }
            ShowHeld();
        }
        ctrlCoroutine = null;
    }
    
    #endregion

    
    #region Held 状态处理（再次点击放置）
    
    private void HandleHeldClick(int index, bool isEquip, bool shift)
    {
        // 停止 Ctrl 长按
        if (ctrlCoroutine != null) { StopCoroutine(ctrlCoroutine); ctrlCoroutine = null; }
        
        // 点击源槽位 + Shift = 连续二分
        if (index == sourceIndex && isEquip == sourceIsEquip && shift && currentState == State.HeldByShift)
        {
            ContinueShiftSplit();
            return;
        }
        
        // 执行放置（Shift/Ctrl 模式：不同物品返回原位）
        ExecutePlacement(index, isEquip, false);
    }
    
    /// <summary>
    /// 处理 Held 状态下点击非槽位区域（由 InventoryPanelClickHandler 调用）
    /// </summary>
    public void HandleHeldClickOutside(Vector2 screenPos, bool isDropZone)
    {
        if (currentState != State.HeldByShift && currentState != State.HeldByCtrl) return;
        
        // 停止 Ctrl 长按
        if (ctrlCoroutine != null) { StopCoroutine(ctrlCoroutine); ctrlCoroutine = null; }
        
        if (isDropZone)
        {
            // 垃圾桶区域 - 丢弃
            DropItem();
            ResetState();
        }
        else if (!IsInsidePanel(screenPos))
        {
            // 面板外 - 丢弃
            DropItem();
            ResetState();
        }
        else
        {
            // 面板内非槽位区域 - 返回原位
            ReturnToSource();
            ResetState();
        }
    }
    
    private void ContinueShiftSplit()
    {
        // ★ 手上只有 1 个时，不执行二分（避免返回 0 个到源槽位）
        if (heldItem.amount <= 1) return;
        
        // ★ 向下取整返回，向上取整保留在手上
        int returnAmount = heldItem.amount / 2;
        int handAmount = heldItem.amount - returnAmount;
        
        heldItem.amount = handAmount;
        
        ItemStack src = GetSlot(sourceIndex, sourceIsEquip);
        SetSlot(sourceIndex, sourceIsEquip, new ItemStack { itemId = src.itemId, quality = src.quality, amount = src.amount + returnAmount });
        
        ShowHeld();
    }
    
    /// <summary>
    /// 执行放置逻辑
    /// </summary>
    /// <param name="allowSwap">是否允许交换（拖拽=true，Shift/Ctrl=false）</param>
    private void ExecutePlacement(int targetIndex, bool targetIsEquip, bool allowSwap)
    {
        ItemStack target = GetSlot(targetIndex, targetIsEquip);
        InventoryItem targetRuntimeItem = GetRuntimeItem(targetIndex, targetIsEquip);

        if (TryRejectProtectedHeldInventoryMutation(targetIndex, targetIsEquip))
        {
            ReturnToSource();
            ResetState();
            return;
        }
        
        // 装备槽位限制检查
        if (targetIsEquip)
        {
            var itemData = database?.GetItemByID(heldItem.itemId);
            if (itemData == null || !CanPlaceInEquipSlot(itemData, targetIndex))
            {
                ReturnToSource();
                ResetState();
                return;
            }
        }
        
        // 目标为空：直接放置
        if (target.IsEmpty)
        {
            SetSlot(targetIndex, targetIsEquip, heldItem, heldRuntimeItem);
            heldItem = new ItemStack();
            heldRuntimeItem = null;
            SelectSlot(targetIndex, targetIsEquip);  // 选中目标槽位
            ResetState();
            return;
        }
        
        // 相同物品：堆叠
        if (target.itemId == heldItem.itemId && target.quality == heldItem.quality)
        {
            var itemData = database?.GetItemByID(heldItem.itemId);
            int maxStack = itemData != null ? itemData.maxStackSize : 99;
            int total = target.amount + heldItem.amount;
            
            if (total <= maxStack)
            {
                SetSlot(targetIndex, targetIsEquip, new ItemStack { itemId = target.itemId, quality = target.quality, amount = total });
                heldItem = new ItemStack();
                heldRuntimeItem = null;
                SelectSlot(targetIndex, targetIsEquip);  // 选中目标槽位
                ResetState();
            }
            else
            {
                SetSlot(targetIndex, targetIsEquip, new ItemStack { itemId = target.itemId, quality = target.quality, amount = maxStack });
                heldItem.amount = total - maxStack;
                ShowHeld();
            }
            return;
        }
        
        // 不同物品
        if (allowSwap)
        {
            // 拖拽模式：交换（把手上物品放到目标，把目标物品放到源槽位）
            SetSlot(targetIndex, targetIsEquip, heldItem, heldRuntimeItem);
            SetSlot(sourceIndex, sourceIsEquip, target, targetRuntimeItem);
            heldItem = new ItemStack();
            heldRuntimeItem = null;
            SelectSlot(targetIndex, targetIsEquip);  // 选中目标槽位
            ResetState();
        }
        else
        {
            // Shift/Ctrl 模式：检查源槽位是否为空
            ItemStack source = GetSlot(sourceIndex, sourceIsEquip);
            
            if (source.IsEmpty)
            {
                // 源槽位为空：允许交换
                SetSlot(targetIndex, targetIsEquip, heldItem, heldRuntimeItem);
                SetSlot(sourceIndex, sourceIsEquip, target, targetRuntimeItem);
                heldItem = new ItemStack();
                heldRuntimeItem = null;
                SelectSlot(targetIndex, targetIsEquip);  // 选中目标槽位
                ResetState();
            }
            else
            {
                // 源槽位非空：返回原位
                ReturnToSource();
                ResetState();
            }
        }
    }
    
    #endregion

    
    #region 辅助方法
    
    private ItemStack GetSlot(int index, bool isEquip)
    {
        return isEquip ? equipment.GetEquip(index) : inventory.GetSlot(index);
    }

    private InventoryItem GetRuntimeItem(int index, bool isEquip)
    {
        return isEquip ? equipment.GetEquipItem(index) : inventory.GetInventoryItem(index);
    }

    private bool TryRejectProtectedHeldInventoryMutation(int targetIndex, bool targetIsEquip)
    {
        var inputManager = GameInputManager.Instance;
        if (inputManager == null || sourceIndex < 0)
        {
            return false;
        }

        return inputManager.TryRejectProtectedHeldInventoryMutation(
            sourceIndex,
            !sourceIsEquip,
            targetIndex,
            !targetIsEquip);
    }

    private bool TryRejectProtectedHeldInventoryPickup(int slotIndex, bool isEquip)
    {
        var inputManager = GameInputManager.Instance;
        return inputManager != null && inputManager.TryRejectActiveFarmToolInventoryMove(slotIndex, isEquip);
    }

    private bool TryRejectProtectedHeldInventoryReshuffle()
    {
        var inputManager = GameInputManager.Instance;
        return inputManager != null && inputManager.TryRejectProtectedHeldInventoryReshuffle();
    }
    
    private void SetSlot(int index, bool isEquip, ItemStack item)
    {
        if (isEquip) equipment.SetEquip(index, item);
        else inventory.SetSlot(index, item);
    }

    private void SetSlot(int index, bool isEquip, ItemStack item, InventoryItem runtimeItem)
    {
        if (runtimeItem != null && !runtimeItem.IsEmpty)
        {
            if (isEquip) equipment.SetEquipItem(index, runtimeItem);
            else inventory.SetInventoryItem(index, runtimeItem);
            return;
        }

        SetSlot(index, isEquip, item);
    }
    
    private void ClearSlot(int index, bool isEquip)
    {
        if (isEquip) equipment.ClearEquip(index);
        else inventory.ClearSlot(index);
    }
    
    private void ReturnToSource()
    {
        if (heldItem.IsEmpty) return;
        
        ItemStack src = GetSlot(sourceIndex, sourceIsEquip);
        if (src.IsEmpty)
        {
            SetSlot(sourceIndex, sourceIsEquip, heldItem, heldRuntimeItem);
        }
        else if (src.itemId == heldItem.itemId && src.quality == heldItem.quality)
        {
            SetSlot(sourceIndex, sourceIsEquip, new ItemStack { itemId = src.itemId, quality = src.quality, amount = src.amount + heldItem.amount });
        }
        else
        {
            // 原位被占，找空位
            for (int i = 0; i < 36; i++)
            {
                if (inventory.GetSlot(i).IsEmpty)
                {
                    SetSlot(i, false, heldItem, heldRuntimeItem);
                    heldItem = new ItemStack();
                    heldRuntimeItem = null;
                    return;
                }
            }
            DropItem();
        }
        heldItem = new ItemStack();
        heldRuntimeItem = null;
    }
    
    private void DropItem()
    {
        if (heldItem.IsEmpty)
        {
            return;
        }
        
        // 🔥 使用 ItemDropHelper 统一丢弃逻辑
        if (heldRuntimeItem != null && !heldRuntimeItem.IsEmpty)
            FarmGame.UI.ItemDropHelper.DropAtPlayer(heldRuntimeItem, dropCooldown);
        else
            FarmGame.UI.ItemDropHelper.DropAtPlayer(heldItem, dropCooldown);
        
        heldItem = new ItemStack();
        heldRuntimeItem = null;
    }
    
    private void ShowHeld()
    {
        if (heldDisplay == null) return;
        var itemData = database?.GetItemByID(heldItem.itemId);
        if (itemData != null)
            heldDisplay.Show(heldItem.itemId, heldItem.amount, itemData.GetBagSprite());
    }
    
    private void ResetState()
    {
        currentState = State.Idle;
        heldItem = new ItemStack();
        heldRuntimeItem = null;
        sourceIndex = -1;
        dropTargetIndex = DROP_TARGET_NONE;  // ★ 使用常量
        heldDisplay?.Hide();
    }
    
    private bool IsInsidePanel(Vector2 pos)
    {
        // ★ 优先使用 inventoryBoundsRect（背包实际可见区域）
        if (inventoryBoundsRect != null)
        {
            bool inside = RectTransformUtility.RectangleContainsScreenPoint(inventoryBoundsRect, pos);
            return inside;
        }
        
        // 回退：背包区域 = Main + Top（不包括 Background）
        bool insideMain = mainRect != null && RectTransformUtility.RectangleContainsScreenPoint(mainRect, pos);
        bool insideTop = topRect != null && RectTransformUtility.RectangleContainsScreenPoint(topRect, pos);
        
        // 如果 mainRect 和 topRect 都没配置，回退到 panelRect
        if (mainRect == null && topRect == null)
        {
            return panelRect != null && RectTransformUtility.RectangleContainsScreenPoint(panelRect, pos);
        }
        
        return insideMain || insideTop;
    }
    
    /// <summary>
    /// 检查是否在垃圾桶区域
    /// </summary>
    private bool IsOverTrashCan(Vector2 pos)
    {
        return trashCanRect != null && RectTransformUtility.RectangleContainsScreenPoint(trashCanRect, pos);
    }
    
    /// <summary>
    /// 🔥 公共方法：检测是否在面板边界内（供 InventorySlotInteraction 调用）
    /// </summary>
    public bool IsInsidePanelBounds(Vector2 pos)
    {
        return IsInsidePanel(pos);
    }
    
    /// <summary>
    /// 整理按钮点击
    /// </summary>
    private void OnSortButtonClick()
    {
        if (TryRejectProtectedHeldInventoryReshuffle())
        {
            return;
        }

        // 如果正在拿取物品，先取消
        if (IsHolding)
        {
            Cancel();
        }
        
        // 执行整理
        if (sortService != null)
        {
            sortService.SortInventory();
            
            // ★ 整理后清除所有选中状态
            ClearAllSelection();
        }
        else
        {
            Debug.LogWarning("[Manager] sortService 为 null，无法整理");
        }
    }
    
    /// <summary>
    /// 清除所有背包槽位的选中状态
    /// 使用 SetAllTogglesOff() 更简洁可靠
    /// </summary>
    private void ClearAllSelection()
    {
        // 方案 1：通过 InventoryPanelUI 清空（最可靠）
        var invPanel = FindFirstObjectByType<InventoryPanelUI>();
        if (invPanel != null)
        {
            invPanel.ClearUpSelection();
            return;
        }
        
        // 方案 2：通过 inventorySlots 数组获取 ToggleGroup
        if (inventorySlots != null && inventorySlots.Length > 0)
        {
            var firstSlot = inventorySlots[0];
            if (firstSlot != null)
            {
                var toggle = firstSlot.GetComponent<Toggle>();
                if (toggle != null && toggle.group != null)
                {
                    toggle.group.SetAllTogglesOff();
                    return;
                }
            }
            
            // 方案 3：回退到遍历槽位
            foreach (var slot in inventorySlots)
            {
                if (slot != null)
                {
                    slot.Deselect();
                }
            }
        }
    }
    
    /// <summary>
    /// 垃圾桶点击（丢弃手上物品）
    /// </summary>
    public void OnTrashCanClick()
    {
        if (!IsHolding) return;
        
        DropItem();
        ResetState();
    }
    
    /// <summary>
    /// 选中指定槽位（设置 Toggle.isOn = true）
    /// </summary>
    private void SelectSlot(int index, bool isEquip)
    {
        if (!isEquip && inventorySlots != null && index >= 0 && index < inventorySlots.Length)
        {
            inventorySlots[index]?.Select();
        }
        // 装备槽位暂不处理选中（没有 Toggle）
    }
    
    private void QuickEquip(int srcIndex, ItemStack item, ItemData itemData)
    {
        int targetSlot = GetEquipSlotForType(itemData.equipmentType);
        if (targetSlot < 0) return;

        var inputManager = GameInputManager.Instance;
        if (inputManager != null &&
            inputManager.TryRejectProtectedHeldInventoryMutation(srcIndex, true, targetSlot, false))
        {
            return;
        }
        
        ItemStack current = equipment.GetEquip(targetSlot);
        InventoryItem currentRuntimeItem = equipment.GetEquipItem(targetSlot);
        InventoryItem sourceRuntimeItem = inventory.GetInventoryItem(srcIndex);

        if (sourceRuntimeItem != null && !sourceRuntimeItem.IsEmpty)
            equipment.SetEquipItem(targetSlot, sourceRuntimeItem);
        else
            equipment.SetEquip(targetSlot, item);
        
        if (!current.IsEmpty)
            SetSlot(srcIndex, false, current, currentRuntimeItem);
        else
            inventory.ClearSlot(srcIndex);
    }
    
    private int GetEquipSlotForType(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Helmet: return 0;
            case EquipmentType.Pants: return 1;
            case EquipmentType.Armor: return 2;
            case EquipmentType.Shoes: return 3;
            case EquipmentType.Ring:
                if (equipment.GetEquip(4).IsEmpty) return 4;
                if (equipment.GetEquip(5).IsEmpty) return 5;
                return 4;
            default: return -1;
        }
    }
    
    private bool CanPlaceInEquipSlot(ItemData itemData, int slot)
    {
        switch (slot)
        {
            case 0: return itemData.equipmentType == EquipmentType.Helmet;
            case 1: return itemData.equipmentType == EquipmentType.Pants;
            case 2: return itemData.equipmentType == EquipmentType.Armor;
            case 3: return itemData.equipmentType == EquipmentType.Shoes;
            case 4: case 5: return itemData.equipmentType == EquipmentType.Ring;
            default: return false;
        }
    }
    
    #endregion
}
