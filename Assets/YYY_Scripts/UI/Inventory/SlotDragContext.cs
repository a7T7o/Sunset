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
    /// 源容器是否为箱子
    /// </summary>
    public static bool IsSourceChest => SourceContainer is ChestInventory || SourceContainer is ChestInventoryV2;

    /// <summary>
    /// 源容器是否为背包
    /// </summary>
    public static bool IsSourceInventory => SourceContainer is InventoryService;

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
        InventoryItem runtimeItem = null)
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

        // 尝试返回物品到源槽位
        var currentSlot = SourceContainer.GetSlot(SourceSlotIndex);
        if (currentSlot.IsEmpty)
        {
            RestoreToSource();
        }
        else if (currentSlot.CanStackWith(DraggedItem))
        {
            var merged = currentSlot;
            merged.amount += DraggedItem.amount;
            SourceContainer.SetSlot(SourceSlotIndex, merged);
        }
        else
        {
            // 源槽位被占用且不能堆叠，物品丢失（理论上不应该发生）
            Debug.LogWarning($"[SlotDragContext] Cancel: 无法返回物品到源槽位，物品丢失！");
        }

        // 🔥 P1：移除日志输出（符合日志规范）
        End();
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

    #endregion
}
