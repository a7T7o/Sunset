using UnityEngine;
using FarmGame.Data.Core;

/// <summary>
/// 槽位拖拽上下文 - 跨容器拖拽支持
///
/// 职责：
/// 1. 记录拖拽开始时的源容器和槽位信息
/// 2. 提供静态访问，让 Drop 目标能获取拖拽来源
/// 3. 支持 InventoryService 和 ChestInventory 两种容器
///
/// 使用流程：
/// 1. BeginDrag 时调用 SlotDragContext.Begin()
/// 2. Drop 时通过 SlotDragContext.Current 获取来源信息
/// 3. EndDrag 时调用 SlotDragContext.End()
/// </summary>
public static class SlotDragContext
{
    public enum ModifierHoldMode
    {
        None = 0,
        Shift = 1,
        Ctrl = 2
    }

    #region 拖拽状态

    /// <summary>
    /// 是否正在拖拽
    /// </summary>
    public static bool IsDragging { get; private set; }

    /// <summary>
    /// 源容器（IItemContainer 接口）
    /// </summary>
    public static IItemContainer SourceContainer { get; private set; }

    /// <summary>
    /// 源槽位索引
    /// </summary>
    public static int SourceSlotIndex { get; private set; } = -1;

    /// <summary>
    /// 拖拽的物品
    /// </summary>
    public static ItemStack DraggedItem { get; private set; }
    public static InventoryItem DraggedRuntimeItem { get; private set; }

    /// <summary>
    /// 🔥 新增：源槽位 UI 引用（用于跨区域放置时取消选中）
    /// </summary>
    public static InventorySlotUI SourceSlotUI { get; private set; }

    /// <summary>
    /// 当前修饰键 held 模式。
    /// 仅箱子侧 Shift/Ctrl 连续拿取会写入该值。
    /// </summary>
    public static ModifierHoldMode HoldMode { get; private set; }

    /// <summary>
    /// 当前箱子 held 的 owner 槽位。
    /// </summary>
    public static InventorySlotInteraction ActiveOwner { get; private set; }

    /// <summary>
    /// 源容器是否为箱子
    /// </summary>
    public static bool IsSourceChest => SourceContainer is ChestInventory || SourceContainer is ChestInventoryV2;

    /// <summary>
    /// 源容器是否为背包
    /// </summary>
    public static bool IsSourceInventory => SourceContainer is InventoryService;

    public static bool IsHeldByShift => IsDragging && HoldMode == ModifierHoldMode.Shift;
    public static bool IsHeldByCtrl => IsDragging && HoldMode == ModifierHoldMode.Ctrl;
    public static bool IsModifierHeld => IsHeldByShift || IsHeldByCtrl;

    #endregion

    #region 公共方法

    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="container">源容器</param>
    /// <param name="slotIndex">源槽位索引</param>
    /// <param name="item">拖拽的物品</param>
    /// <param name="slotUI">源槽位 UI（可选，用于跨区域放置时取消选中）</param>
    public static void Begin(
        IItemContainer container,
        int slotIndex,
        ItemStack item,
        InventorySlotUI slotUI = null,
        InventoryItem runtimeItem = null,
        ModifierHoldMode holdMode = ModifierHoldMode.None,
        InventorySlotInteraction owner = null)
    {
        // 🔥 互斥检查：如果 Manager 正在持有物品，拒绝开始拖拽
        if (InventoryInteractionManager.Instance != null &&
            InventoryInteractionManager.Instance.IsHolding)
        {
            Debug.LogWarning("[SlotDragContext] InventoryInteractionManager 正在持有物品，无法开始拖拽");
            return;
        }

        IsDragging = true;
        SourceContainer = container;
        SourceSlotIndex = slotIndex;
        DraggedItem = item;
        DraggedRuntimeItem = runtimeItem;
        SourceSlotUI = slotUI;
        HoldMode = holdMode;
        ActiveOwner = owner;
        SourceSlotUI?.Select();
        // 🔥 P1：移除日志输出（符合日志规范）
    }

    /// <summary>
    /// 结束拖拽（清理状态）
    /// </summary>
    public static void End()
    {
        // 🔥 P1：移除日志输出（符合日志规范）
        IsDragging = false;
        SourceContainer = null;
        SourceSlotIndex = -1;
        DraggedItem = ItemStack.Empty;
        DraggedRuntimeItem = null;
        SourceSlotUI = null;
        HoldMode = ModifierHoldMode.None;
        ActiveOwner = null;
    }

    /// <summary>
    /// 🔥 更新拖拽物品（用于连续拿取时更新数量）
    /// 不会重新检查互斥状态，仅更新 DraggedItem
    /// </summary>
    public static void UpdateDraggedItem(ItemStack item)
    {
        if (!IsDragging) return;
        DraggedItem = item;
    }

    public static void UpdateDraggedRuntimeItem(InventoryItem runtimeItem)
    {
        if (!IsDragging) return;
        DraggedRuntimeItem = runtimeItem;
    }

    /// <summary>
    /// 取消拖拽（返回物品到源槽位）
    /// </summary>
    public static void Cancel()
    {
        if (!IsDragging || SourceContainer == null) return;

        ItemStack draggedItem = DraggedItem;
        InventoryItem draggedRuntimeItem = DraggedRuntimeItem;
        ReturnHeldToSourceContainer(SourceContainer, SourceSlotIndex, ref draggedItem, ref draggedRuntimeItem);

        // 🔥 P1：移除日志输出（符合日志规范）
        End();
    }

    public static bool IsOwnedBy(InventorySlotInteraction owner)
    {
        return owner != null && ActiveOwner == owner;
    }

    public static void ClearOwner(InventorySlotInteraction owner)
    {
        if (owner == null || ActiveOwner != owner)
        {
            return;
        }

        ActiveOwner = null;
        HoldMode = ModifierHoldMode.None;
    }

    private static void RestoreToSource()
    {
        if (SourceContainer is InventoryService inventoryService && DraggedRuntimeItem != null && !DraggedRuntimeItem.IsEmpty)
        {
            inventoryService.SetInventoryItem(SourceSlotIndex, DraggedRuntimeItem);
            return;
        }

        if (SourceContainer is ChestInventoryV2 chestInventoryV2 && DraggedRuntimeItem != null && !DraggedRuntimeItem.IsEmpty)
        {
            chestInventoryV2.SetItem(SourceSlotIndex, DraggedRuntimeItem);
            return;
        }

        SourceContainer.SetSlot(SourceSlotIndex, DraggedItem);
    }

    private static InventoryItem ResolveContainerRuntimeItem(IItemContainer container, int slotIndex)
    {
        if (container is InventoryService inventoryService)
        {
            return inventoryService.GetInventoryItem(slotIndex);
        }

        if (container is ChestInventoryV2 chestInventoryV2)
        {
            return chestInventoryV2.GetItem(slotIndex);
        }

        return null;
    }

    private static InventoryItem CreateRuntimeItemForAmount(InventoryItem runtimeItem, int amount)
    {
        if (runtimeItem == null || runtimeItem.IsEmpty || amount <= 0)
        {
            return null;
        }

        if (runtimeItem.Amount == amount)
        {
            return runtimeItem;
        }

        var clone = runtimeItem.Clone();
        clone.SetAmount(amount);
        return clone;
    }

    private static void SetContainerSlotPreservingRuntime(IItemContainer container, int slotIndex, ItemStack stack, InventoryItem runtimeItem)
    {
        InventoryItem adjustedRuntimeItem = CreateRuntimeItemForAmount(runtimeItem, stack.amount);
        if (container is InventoryService inventoryService && adjustedRuntimeItem != null && !adjustedRuntimeItem.IsEmpty)
        {
            inventoryService.SetInventoryItem(slotIndex, adjustedRuntimeItem);
            return;
        }

        if (container is ChestInventoryV2 chestInventoryV2 && adjustedRuntimeItem != null && !adjustedRuntimeItem.IsEmpty)
        {
            chestInventoryV2.SetItem(slotIndex, adjustedRuntimeItem);
            return;
        }

        container.SetSlot(slotIndex, stack);
    }

    private static void UpdateContainerStackAmountPreservingRuntime(
        IItemContainer container,
        int slotIndex,
        ItemStack currentStack,
        InventoryItem currentRuntimeItem,
        int newAmount)
    {
        if (newAmount <= 0)
        {
            container.ClearSlot(slotIndex);
            return;
        }

        ItemStack updatedStack = new ItemStack(currentStack.itemId, currentStack.quality, newAmount);
        SetContainerSlotPreservingRuntime(container, slotIndex, updatedStack, currentRuntimeItem);
    }

    private static void ReturnHeldToSourceContainer(
        IItemContainer container,
        int sourceIndex,
        ref ItemStack draggedItem,
        ref InventoryItem draggedRuntimeItem)
    {
        if (container == null || sourceIndex < 0 || draggedItem.IsEmpty)
        {
            return;
        }

        ItemStack sourceSlot = container.GetSlot(sourceIndex);
        if (sourceSlot.IsEmpty)
        {
            SetContainerSlotPreservingRuntime(container, sourceIndex, draggedItem, draggedRuntimeItem);
            draggedItem = ItemStack.Empty;
            draggedRuntimeItem = null;
            return;
        }

        if (sourceSlot.CanStackWith(draggedItem))
        {
            int maxStack = container.GetMaxStack(draggedItem.itemId);
            int acceptedAmount = Mathf.Min(draggedItem.amount, Mathf.Max(0, maxStack - sourceSlot.amount));
            if (acceptedAmount > 0)
            {
                InventoryItem sourceRuntimeItem = ResolveContainerRuntimeItem(container, sourceIndex);
                UpdateContainerStackAmountPreservingRuntime(
                    container,
                    sourceIndex,
                    sourceSlot,
                    sourceRuntimeItem,
                    sourceSlot.amount + acceptedAmount);
            }

            int remainingAmount = draggedItem.amount - acceptedAmount;
            if (remainingAmount <= 0)
            {
                draggedItem = ItemStack.Empty;
                draggedRuntimeItem = null;
                return;
            }

            draggedItem = new ItemStack(draggedItem.itemId, draggedItem.quality, remainingAmount);
            draggedRuntimeItem = CreateRuntimeItemForAmount(draggedRuntimeItem, remainingAmount);
        }

        for (int index = 0; index < container.Capacity; index++)
        {
            if (index == sourceIndex)
            {
                continue;
            }

            if (!container.GetSlot(index).IsEmpty)
            {
                continue;
            }

            SetContainerSlotPreservingRuntime(container, index, draggedItem, draggedRuntimeItem);
            draggedItem = ItemStack.Empty;
            draggedRuntimeItem = null;
            return;
        }

        Debug.LogWarning($"[SlotDragContext] Cancel: 回源槽位 {sourceIndex} 已被占用且容器无空位，改为掉落到玩家脚下，避免覆盖现有物品。");
        float dropCooldown = SourceContainer is ChestInventory || SourceContainer is ChestInventoryV2 ? 5f : 0.35f;
        if (draggedRuntimeItem != null && !draggedRuntimeItem.IsEmpty)
        {
            FarmGame.UI.ItemDropHelper.DropAtPlayer(draggedRuntimeItem, dropCooldown);
        }
        else
        {
            FarmGame.UI.ItemDropHelper.DropAtPlayer(draggedItem, dropCooldown);
        }
        draggedItem = ItemStack.Empty;
        draggedRuntimeItem = null;
    }

    #endregion
}
