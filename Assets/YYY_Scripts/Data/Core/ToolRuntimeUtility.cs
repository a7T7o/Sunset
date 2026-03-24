using UnityEngine;

namespace FarmGame.Data.Core
{
    /// <summary>
    /// 统一处理工具实例化、耐久初始化与一次成功/有效使用的运行时消耗。
    /// </summary>
    public static class ToolRuntimeUtility
    {
        private const int DefaultDurabilityCost = 1;

        public static InventoryItem CreateRuntimeItem(global::FarmGame.Data.ItemDatabase database, int itemId, int quality, int amount)
        {
            global::FarmGame.Data.ItemData itemData = database != null ? database.GetItemByID(itemId) : null;
            return CreateRuntimeItem(itemData, itemId, quality, amount);
        }

        public static InventoryItem CreateRuntimeItem(global::FarmGame.Data.ItemData itemData, int quality, int amount)
        {
            int itemId = itemData != null ? itemData.itemID : -1;
            return CreateRuntimeItem(itemData, itemId, quality, amount);
        }

        public static InventoryItem NormalizeInventoryItem(InventoryItem item, global::FarmGame.Data.ItemDatabase database)
        {
            if (item == null || item.IsEmpty)
            {
                return item;
            }

            global::FarmGame.Data.ItemData itemData = database != null ? database.GetItemByID(item.ItemId) : null;
            EnsureRuntimeState(item, itemData);
            return item;
        }

        public static bool EnsureRuntimeState(InventoryItem item, global::FarmGame.Data.ItemDatabase database)
        {
            if (item == null || item.IsEmpty)
            {
                return false;
            }

            global::FarmGame.Data.ItemData itemData = database != null ? database.GetItemByID(item.ItemId) : null;
            return EnsureRuntimeState(item, itemData);
        }

        public static bool EnsureRuntimeState(InventoryItem item, global::FarmGame.Data.ItemData itemData)
        {
            if (item == null || item.IsEmpty || itemData is not global::FarmGame.Data.ToolData toolData || !UsesDurability(toolData))
            {
                return false;
            }

            if (item.MaxDurability > 0 && item.CurrentDurability >= 0)
            {
                return false;
            }

            item.SetDurability(GetMaxDurability(toolData));
            return true;
        }

        public static bool TryConsumeHeldToolUse(
            global::InventoryService inventory,
            global::HotbarSelectionService hotbarSelection,
            global::FarmGame.Data.ItemDatabase database,
            global::FarmGame.Data.ToolData toolData,
            string context)
        {
            if (toolData == null)
            {
                return false;
            }

            inventory ??= Object.FindFirstObjectByType<global::InventoryService>();
            hotbarSelection ??= Object.FindFirstObjectByType<global::HotbarSelectionService>();
            database ??= inventory != null ? inventory.Database : null;

            if (inventory == null || hotbarSelection == null)
            {
                Debug.LogWarning($"[ToolRuntime] {context}: 缺少 Inventory/Hotbar 服务，无法提交工具运行时消耗");
                return false;
            }

            int slotIndex = Mathf.Clamp(hotbarSelection.selectedIndex, 0, InventoryService.HotbarWidth - 1);
            ItemStack slotStack = inventory.GetSlot(slotIndex);
            if (slotStack.IsEmpty)
            {
                Debug.LogWarning($"[ToolRuntime] {context}: 当前快捷栏槽位为空，无法提交工具运行时消耗");
                return false;
            }

            if (slotStack.itemId != toolData.itemID)
            {
                Debug.LogWarning($"[ToolRuntime] {context}: 当前槽位物品({slotStack.itemId})与工具({toolData.itemID})不一致，跳过提交");
                return false;
            }

            InventoryItem runtimeItem = inventory.GetInventoryItem(slotIndex);
            if (runtimeItem == null || runtimeItem.IsEmpty)
            {
                runtimeItem = CreateRuntimeItem(database, slotStack.itemId, slotStack.quality, slotStack.amount);
                inventory.SetInventoryItem(slotIndex, runtimeItem);
            }
            else if (EnsureRuntimeState(runtimeItem, toolData))
            {
                inventory.RefreshSlot(slotIndex);
            }

            int energyCost = Mathf.Max(0, toolData.energyCost);
            global::EnergySystem energySystem = global::EnergySystem.Instance;
            if (energySystem != null && !energySystem.TryConsumeEnergy(energyCost))
            {
                Debug.LogWarning($"[ToolRuntime] {context}: 精力不足，{toolData.itemName} 本次使用未提交");
                return false;
            }

            int durabilityCost = UsesDurability(toolData) ? Mathf.Max(0, GetDurabilityCost(toolData)) : 0;
            if (runtimeItem != null && runtimeItem.HasDurability && durabilityCost > 0)
            {
                runtimeItem.UseDurability(durabilityCost);
            }

            inventory.RefreshSlot(slotIndex);

            if (runtimeItem != null && runtimeItem.HasDurability)
            {
                Debug.Log($"[ToolRuntime] {context}: {toolData.itemName} 精力-{energyCost}，当前精力 {energySystem?.CurrentEnergy ?? 0}/{energySystem?.MaxEnergy ?? 0}；耐久-{durabilityCost}，当前耐久 {runtimeItem.CurrentDurability}/{runtimeItem.MaxDurability}");
            }
            else
            {
                Debug.Log($"[ToolRuntime] {context}: {toolData.itemName} 精力-{energyCost}，当前精力 {energySystem?.CurrentEnergy ?? 0}/{energySystem?.MaxEnergy ?? 0}；无耐久度链路");
            }

            return true;
        }

        public static bool UsesDurability(global::FarmGame.Data.ToolData toolData)
        {
            return toolData != null && (toolData.hasDurability || toolData.maxDurability > 0);
        }

        public static int GetDurabilityCost(global::FarmGame.Data.ToolData toolData)
        {
            if (toolData == null)
            {
                return DefaultDurabilityCost;
            }

            return Mathf.Max(0, toolData.durabilityCost <= 0 ? DefaultDurabilityCost : toolData.durabilityCost);
        }

        public static int GetMaxDurability(global::FarmGame.Data.ToolData toolData)
        {
            if (toolData == null)
            {
                return 1;
            }

            return Mathf.Max(1, toolData.maxDurability);
        }

        private static InventoryItem CreateRuntimeItem(global::FarmGame.Data.ItemData itemData, int itemId, int quality, int amount)
        {
            if (itemId < 0 || amount <= 0)
            {
                return InventoryItem.Empty;
            }

            if (itemData is global::FarmGame.Data.ToolData toolData && UsesDurability(toolData))
            {
                return new InventoryItem(itemId, quality, amount, GetMaxDurability(toolData));
            }

            return new InventoryItem(itemId, quality, amount);
        }
    }
}
