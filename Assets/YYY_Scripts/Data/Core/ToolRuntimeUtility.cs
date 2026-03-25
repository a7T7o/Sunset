using UnityEngine;

namespace FarmGame.Data.Core
{
    public enum ToolUseFailureReason
    {
        None,
        ToolMissing,
        ServicesMissing,
        SlotEmpty,
        SlotMismatch,
        InsufficientEnergy,
        EmptyWateringCan
    }

    public readonly struct ToolUseCommitResult
    {
        public ToolUseCommitResult(
            bool succeeded,
            ToolUseFailureReason failureReason,
            string context,
            int slotIndex,
            int energyCost,
            int durabilityCost,
            int remainingDurability,
            bool toolBroken,
            bool toolRemoved,
            int waterCost,
            int remainingWater,
            int maxWater)
        {
            Succeeded = succeeded;
            FailureReason = failureReason;
            Context = context;
            SlotIndex = slotIndex;
            EnergyCost = energyCost;
            DurabilityCost = durabilityCost;
            RemainingDurability = remainingDurability;
            ToolBroken = toolBroken;
            ToolRemoved = toolRemoved;
            WaterCost = waterCost;
            RemainingWater = remainingWater;
            MaxWater = maxWater;
        }

        public bool Succeeded { get; }
        public ToolUseFailureReason FailureReason { get; }
        public string Context { get; }
        public int SlotIndex { get; }
        public int EnergyCost { get; }
        public int DurabilityCost { get; }
        public int RemainingDurability { get; }
        public bool ToolBroken { get; }
        public bool ToolRemoved { get; }
        public int WaterCost { get; }
        public int RemainingWater { get; }
        public int MaxWater { get; }

        public static ToolUseCommitResult Failure(ToolUseFailureReason reason, string context, int slotIndex = -1)
        {
            return new ToolUseCommitResult(false, reason, context, slotIndex, 0, 0, -1, false, false, 0, -1, -1);
        }

        public static ToolUseCommitResult Success(
            string context,
            int slotIndex,
            int energyCost,
            int durabilityCost,
            int remainingDurability,
            bool toolBroken,
            bool toolRemoved,
            int waterCost,
            int remainingWater,
            int maxWater)
        {
            return new ToolUseCommitResult(true, ToolUseFailureReason.None, context, slotIndex, energyCost, durabilityCost, remainingDurability, toolBroken, toolRemoved, waterCost, remainingWater, maxWater);
        }
    }

    /// <summary>
    /// 统一处理工具实例化、耐久初始化、水量初始化与一次成功/有效使用的运行时消耗。
    /// </summary>
    public static class ToolRuntimeUtility
    {
        private const int DefaultDurabilityCost = 1;
        private const int DefaultWaterCapacity = 100;
        private const int DefaultWaterUseCost = 1;

        public const string WaterCurrentPropertyKey = "watering_current";
        public const string WaterMaxPropertyKey = "watering_max";

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
            if (item == null || item.IsEmpty || itemData is not global::FarmGame.Data.ToolData toolData)
            {
                return false;
            }

            bool changed = false;

            if (UsesWater(toolData))
            {
                if (item.HasDurability)
                {
                    item.ClearDurability();
                    changed = true;
                }

                if (EnsureWaterState(item, toolData))
                {
                    changed = true;
                }

                return changed;
            }

            if (UsesDurability(toolData) && (item.MaxDurability <= 0 || item.CurrentDurability < 0))
            {
                item.SetDurability(GetMaxDurability(toolData));
                changed = true;
            }

            return changed;
        }

        public static bool TryConsumeHeldToolUse(
            global::InventoryService inventory,
            global::HotbarSelectionService hotbarSelection,
            global::FarmGame.Data.ItemDatabase database,
            global::FarmGame.Data.ToolData toolData,
            string context)
        {
            return TryConsumeHeldToolUseDetailed(inventory, hotbarSelection, database, toolData, context).Succeeded;
        }

        public static ToolUseCommitResult TryConsumeHeldToolUseDetailed(
            global::InventoryService inventory,
            global::HotbarSelectionService hotbarSelection,
            global::FarmGame.Data.ItemDatabase database,
            global::FarmGame.Data.ToolData toolData,
            string context)
        {
            if (toolData == null)
            {
                return ToolUseCommitResult.Failure(ToolUseFailureReason.ToolMissing, context);
            }

            inventory ??= Object.FindFirstObjectByType<global::InventoryService>();
            hotbarSelection ??= Object.FindFirstObjectByType<global::HotbarSelectionService>();
            database ??= inventory != null ? inventory.Database : null;

            if (inventory == null || hotbarSelection == null)
            {
                Debug.LogWarning($"[ToolRuntime] {context}: 缺少 Inventory/Hotbar 服务，无法提交工具运行时消耗");
                return ToolUseCommitResult.Failure(ToolUseFailureReason.ServicesMissing, context);
            }

            int slotIndex = Mathf.Clamp(hotbarSelection.selectedIndex, 0, global::InventoryService.HotbarWidth - 1);
            ItemStack slotStack = inventory.GetSlot(slotIndex);
            if (slotStack.IsEmpty)
            {
                Debug.LogWarning($"[ToolRuntime] {context}: 当前快捷栏槽位为空，无法提交工具运行时消耗");
                return ToolUseCommitResult.Failure(ToolUseFailureReason.SlotEmpty, context, slotIndex);
            }

            if (slotStack.itemId != toolData.itemID)
            {
                Debug.LogWarning($"[ToolRuntime] {context}: 当前槽位物品({slotStack.itemId})与工具({toolData.itemID})不一致，跳过提交");
                return ToolUseCommitResult.Failure(ToolUseFailureReason.SlotMismatch, context, slotIndex);
            }

            InventoryItem runtimeItem = inventory.GetInventoryItem(slotIndex);
            bool slotStateChanged = false;
            if (runtimeItem == null || runtimeItem.IsEmpty)
            {
                runtimeItem = CreateRuntimeItem(database, slotStack.itemId, slotStack.quality, slotStack.amount);
                inventory.SetInventoryItem(slotIndex, runtimeItem);
                slotStateChanged = true;
            }

            if (EnsureRuntimeState(runtimeItem, toolData))
            {
                slotStateChanged = true;
            }

            int maxWater = -1;
            int currentWater = -1;
            int waterCost = 0;
            if (UsesWater(toolData))
            {
                maxWater = GetWaterCapacity(toolData);
                currentWater = GetCurrentWater(runtimeItem, toolData);
                waterCost = GetWaterUseCost(toolData);

                if (currentWater < waterCost)
                {
                    if (slotStateChanged)
                    {
                        inventory.RefreshSlot(slotIndex);
                    }

                    NotifyWateringCanEmpty(toolData, slotIndex);
                    Debug.LogWarning($"[ToolRuntime] {context}: {toolData.itemName} 没水了，本次使用未提交");
                    return ToolUseCommitResult.Failure(ToolUseFailureReason.EmptyWateringCan, context, slotIndex);
                }
            }

            int energyCost = Mathf.Max(0, toolData.energyCost);
            global::EnergySystem energySystem = global::EnergySystem.Instance;
            if (energySystem != null && !energySystem.TryConsumeEnergy(energyCost))
            {
                if (slotStateChanged)
                {
                    inventory.RefreshSlot(slotIndex);
                }

                Debug.LogWarning($"[ToolRuntime] {context}: 精力不足，{toolData.itemName} 本次使用未提交");
                return ToolUseCommitResult.Failure(ToolUseFailureReason.InsufficientEnergy, context, slotIndex);
            }

            int durabilityCost = UsesDurability(toolData) ? GetDurabilityCost(toolData) : 0;
            bool toolBroken = false;
            bool toolRemoved = false;

            if (waterCost > 0)
            {
                currentWater = Mathf.Max(0, currentWater - waterCost);
                runtimeItem.SetProperty(WaterCurrentPropertyKey, currentWater);
                runtimeItem.SetProperty(WaterMaxPropertyKey, maxWater);
            }

            if (runtimeItem != null && runtimeItem.HasDurability && durabilityCost > 0)
            {
                toolBroken = runtimeItem.UseDurability(durabilityCost);
            }

            int remainingDurability = runtimeItem != null && runtimeItem.HasDurability ? runtimeItem.CurrentDurability : -1;
            if (toolBroken)
            {
                inventory.ClearSlot(slotIndex);
                toolRemoved = true;
                NotifyToolBroken(toolData, slotIndex);
            }
            else
            {
                inventory.RefreshSlot(slotIndex);
            }

            LogCommit(toolData, context, energySystem, energyCost, durabilityCost, remainingDurability, waterCost, currentWater, maxWater, toolBroken);

            return ToolUseCommitResult.Success(
                context,
                slotIndex,
                energyCost,
                durabilityCost,
                remainingDurability,
                toolBroken,
                toolRemoved,
                waterCost,
                currentWater,
                maxWater);
        }

        public static bool UsesDurability(global::FarmGame.Data.ToolData toolData)
        {
            return toolData != null &&
                   toolData.toolType != global::FarmGame.Data.ToolType.WateringCan &&
                   (toolData.hasDurability || toolData.maxDurability > 0);
        }

        public static bool UsesWater(global::FarmGame.Data.ToolData toolData)
        {
            return toolData != null && toolData.toolType == global::FarmGame.Data.ToolType.WateringCan;
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

        public static int GetWaterCapacity(global::FarmGame.Data.ToolData toolData)
        {
            if (toolData == null)
            {
                return DefaultWaterCapacity;
            }

            if (toolData.waterCapacity > 0)
            {
                return toolData.waterCapacity;
            }

            if (toolData.maxDurability > 0)
            {
                return toolData.maxDurability;
            }

            return DefaultWaterCapacity;
        }

        public static int GetWaterUseCost(global::FarmGame.Data.ToolData toolData)
        {
            if (toolData == null)
            {
                return DefaultWaterUseCost;
            }

            return Mathf.Max(1, toolData.waterUseCost <= 0 ? DefaultWaterUseCost : toolData.waterUseCost);
        }

        public static int GetCurrentWater(InventoryItem item, global::FarmGame.Data.ToolData toolData)
        {
            if (item == null || item.IsEmpty)
            {
                return 0;
            }

            EnsureWaterState(item, toolData);
            int maxWater = GetWaterCapacity(toolData);
            int current = item.GetPropertyInt(WaterCurrentPropertyKey, maxWater);
            return Mathf.Clamp(current, 0, maxWater);
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

        private static bool EnsureWaterState(InventoryItem item, global::FarmGame.Data.ToolData toolData)
        {
            if (item == null || item.IsEmpty || !UsesWater(toolData))
            {
                return false;
            }

            bool changed = false;
            int maxWater = GetWaterCapacity(toolData);

            int recordedMax = item.GetPropertyInt(WaterMaxPropertyKey, 0);
            if (recordedMax != maxWater)
            {
                item.SetProperty(WaterMaxPropertyKey, maxWater);
                changed = true;
            }

            if (!item.HasProperty(WaterCurrentPropertyKey))
            {
                item.SetProperty(WaterCurrentPropertyKey, maxWater);
                return true;
            }

            int currentWater = item.GetPropertyInt(WaterCurrentPropertyKey, maxWater);
            int clampedWater = Mathf.Clamp(currentWater, 0, maxWater);
            if (clampedWater != currentWater)
            {
                item.SetProperty(WaterCurrentPropertyKey, clampedWater);
                changed = true;
            }

            return changed;
        }

        private static void NotifyToolBroken(global::FarmGame.Data.ToolData toolData, int slotIndex)
        {
            global::PlayerToolFeedbackService.ResolveForPlayer(null)?.HandleToolBroken(toolData, slotIndex);
        }

        private static void NotifyWateringCanEmpty(global::FarmGame.Data.ToolData toolData, int slotIndex)
        {
            global::PlayerToolFeedbackService.ResolveForPlayer(null)?.HandleWateringCanEmpty(toolData, slotIndex);
        }

        private static void LogCommit(
            global::FarmGame.Data.ToolData toolData,
            string context,
            global::EnergySystem energySystem,
            int energyCost,
            int durabilityCost,
            int remainingDurability,
            int waterCost,
            int remainingWater,
            int maxWater,
            bool toolBroken)
        {
            string energyPart = $"精力-{energyCost}，当前精力 {energySystem?.CurrentEnergy ?? 0}/{energySystem?.MaxEnergy ?? 0}";
            string durabilityPart = durabilityCost > 0
                ? $"；耐久-{durabilityCost}，当前耐久 {remainingDurability}/{GetMaxDurability(toolData)}"
                : "；无耐久度链路";
            string waterPart = waterCost > 0
                ? $"；水量-{waterCost}，当前水量 {remainingWater}/{maxWater}"
                : string.Empty;
            string breakPart = toolBroken ? "；工具已损坏并移除" : string.Empty;

            Debug.Log($"[ToolRuntime] {context}: {toolData.itemName} {energyPart}{durabilityPart}{waterPart}{breakPart}");
        }
    }
}
