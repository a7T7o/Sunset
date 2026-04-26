using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using FarmGame.Data;
using FarmGame.Data.Core;

/// <summary>
/// 背包整理服务
/// </summary>
public class InventorySortService : MonoBehaviour
{
    [SerializeField] private InventoryService inventory;
    [SerializeField] private ItemDatabase database;
    private const int SortStartIndex = 0;

    void Awake()
    {
        ResolveRuntimeContext();
    }

    public void RebindRuntimeContext(InventoryService runtimeInventory, ItemDatabase runtimeDatabase = null)
    {
        if (runtimeInventory != null)
        {
            inventory = runtimeInventory;
        }

        if (runtimeDatabase != null)
        {
            database = runtimeDatabase;
        }
        else if (inventory != null)
        {
            database = inventory.Database;
        }
    }

    private void ResolveRuntimeContext()
    {
        InventoryService preferredRuntimeInventory = PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService();
        if (preferredRuntimeInventory != null)
        {
            inventory = preferredRuntimeInventory;
        }
        else if (inventory == null)
        {
            inventory = PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()
                ?? FindFirstObjectByType<InventoryService>(FindObjectsInactive.Include);
        }

        if (inventory != null)
        {
            database = inventory.Database;
        }
    }

    /// <summary>
    /// 整理背包（合并 + 排序）
    /// 只整理 Up 区域（0-35），不影响装备栏
    /// </summary>
    public void SortInventory()
    {
        ResolveRuntimeContext();

        if (inventory == null || database == null)
        {
            Debug.LogWarning("[InventorySortService] inventory 或 database 为 null");
            return;
        }

        // 1. 收集整包物品；玩家当前裁定是“背包 sort 必须按整包统一整理”，
        // 不能再只整理背包下半区，留下第一行像没参与排序一样。
        var runtimeItems = new List<InventoryItem>();
        for (int i = SortStartIndex; i < inventory.Capacity; i++)
        {
            var runtimeItem = inventory.GetInventoryItem(i);
            if (runtimeItem != null && !runtimeItem.IsEmpty)
            {
                runtimeItems.Add(runtimeItem);
                inventory.SetInventoryItem(i, null);
            }
        }

        // 2. 合并仅允许堆叠的普通物品，保留工具/动态实例的运行时状态
        var items = MergeRuntimeItems(runtimeItems);

        // 3. 按优先级排序
        items = items.OrderBy(s => GetPriority(s))
                     .ThenBy(s => s.ItemId)
                     .ThenBy(s => s.Quality)
                     .ToList();

        // 4. 放回槽位
        int slotIndex = SortStartIndex;
        foreach (var stack in items)
        {
            if (slotIndex >= inventory.Capacity) break;
            inventory.SetInventoryItem(slotIndex++, stack);
        }

        while (slotIndex < inventory.Capacity)
        {
            inventory.SetInventoryItem(slotIndex++, null);
        }

        Debug.Log($"[InventorySortService] 整理完成: {items.Count} 种物品");
    }

    /// <summary>
    /// 合并相同物品
    /// </summary>
    private List<InventoryItem> MergeRuntimeItems(List<InventoryItem> items)
    {
        var groupedAmounts = new Dictionary<(int id, int quality), int>();
        var result = new List<InventoryItem>();

        foreach (var item in items)
        {
            if (item == null || item.IsEmpty)
            {
                continue;
            }

            if (item.HasDurability || item.HasDynamicProperties)
            {
                result.Add(item);
                continue;
            }

            var key = (item.ItemId, item.Quality);
            groupedAmounts[key] = groupedAmounts.TryGetValue(key, out int existingAmount)
                ? existingAmount + item.Amount
                : item.Amount;
        }

        foreach (var kvp in groupedAmounts)
        {
            int remaining = kvp.Value;
            int maxStack = database != null ? Mathf.Max(1, database.GetItemByID(kvp.Key.id)?.maxStackSize ?? 99) : 99;

            while (remaining > 0)
            {
                int amount = Mathf.Min(remaining, maxStack);
                result.Add(ToolRuntimeUtility.CreateRuntimeItem(database, kvp.Key.id, kvp.Key.quality, amount));
                remaining -= amount;
            }
        }

        return result;
    }

    /// <summary>
    /// 获取物品排序优先级（数字越小越靠前）
    /// </summary>
    private int GetPriority(InventoryItem stack)
    {
        var itemData = database?.GetItemByID(stack.ItemId);
        if (itemData == null) return 999;

        // 工具 > 武器 > 可放置 > 种子 > 消耗品 > 材料 > 其他
        if (itemData is ToolData) return 0;
        if (itemData is WeaponData) return 1;
        if (itemData.isPlaceable) return 2;
        if (itemData is SeedData) return 3;
        if (itemData.category == ItemCategory.Consumable) return 4;
        if (itemData.category == ItemCategory.Material) return 5;

        return 6;
    }
}
