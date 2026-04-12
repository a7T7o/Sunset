using System;
using UnityEngine;
using FarmGame.Data;

public class HotbarSelectionService : MonoBehaviour
{
    public int selectedIndex = 0; // 0..11
    public int selectedInventoryIndex = 0; // 0..35，允许背包区作为真实手持来源
    public event Action<int> OnSelectedChanged;

    [Header("装备系统引用")]
    [SerializeField] private PlayerToolController playerToolController;
    [SerializeField] private InventoryService inventory;

    private ItemDatabase database;

    private static bool SupportsPlacementMode(ItemData itemData)
    {
        return itemData != null && (itemData.isPlaceable || itemData is SeedData);
    }

    void Awake()
    {
        ResolveReferences();
    }

    void OnEnable()
    {
        BindInventoryEvents();
    }

    void OnDisable()
    {
        UnbindInventoryEvents();
    }

    void Start()
    {
        // 游戏开始时装备当前选中槽位的工具
        EquipCurrentTool();
    }

    /// <summary>
    /// 当背包槽位变化时检查是否需要更新当前装备
    /// 处理场景：拾取物品到当前选中槽位时自动装备
    /// </summary>
    private void OnSlotChanged(int slotIndex)
    {
        // 只有当变化的槽位是当前选中的槽位时才更新装备
        if (slotIndex == selectedIndex)
        {
            EquipCurrentTool();
        }
    }

    /// <summary>
    /// 当快捷栏槽位变化时检查是否需要更新当前装备
    /// </summary>
    private void OnHotbarSlotChanged(int hotbarIndex)
    {
        // 只有当变化的快捷栏槽位是当前选中的槽位时才更新装备
        if (hotbarIndex == selectedIndex)
        {
            EquipCurrentTool();
        }
    }

    public void SelectIndex(int index)
    {
        int clamped = Mathf.Clamp(index, 0, InventoryService.HotbarWidth - 1);
        if (clamped == selectedIndex && selectedInventoryIndex == clamped) return;
        if (!CanApplySelectionChange(clamped)) return;
        RestoreSelection(clamped);
    }

    public void SelectInventoryIndex(int index)
    {
        ResolveReferences();

        int maxIndex = inventory != null ? Mathf.Max(0, inventory.Size - 1) : InventoryService.DefaultInventorySize - 1;
        int clamped = Mathf.Clamp(index, 0, maxIndex);
        if (clamped < InventoryService.HotbarWidth)
        {
            SelectIndex(clamped);
            return;
        }

        selectedInventoryIndex = clamped;
    }

    public void SelectNext()
    {
        int next = (selectedIndex + 1) % InventoryService.HotbarWidth;
        SelectIndex(next);
    }

    public void SelectPrev()
    {
        int prev = (selectedIndex - 1 + InventoryService.HotbarWidth) % InventoryService.HotbarWidth;
        SelectIndex(prev);
    }

    private bool CanApplySelectionChange(int requestedIndex)
    {
        var inputManager = GameInputManager.Instance;
        return inputManager == null || inputManager.TryPrepareHotbarSelectionChange(requestedIndex);
    }

    public void RebindRuntimeReferences(PlayerToolController runtimePlayerToolController, InventoryService runtimeInventory)
    {
        UnbindInventoryEvents();

        if (runtimePlayerToolController != null)
        {
            playerToolController = runtimePlayerToolController;
        }

        if (runtimeInventory != null)
        {
            inventory = runtimeInventory;
        }

        ResolveReferences();

        if (isActiveAndEnabled)
        {
            BindInventoryEvents();
        }
    }

    public void RestoreSelection(int index, bool invokeEvent = true)
    {
        RestoreSelectionState(index, index, invokeEvent);
    }

    public void RestoreSelectionState(int hotbarIndex, int inventoryIndex, bool invokeEvent = true)
    {
        selectedIndex = Mathf.Clamp(hotbarIndex, 0, InventoryService.HotbarWidth - 1);
        selectedInventoryIndex = NormalizeInventoryIndex(inventoryIndex, selectedIndex);
        EquipCurrentTool();

        if (invokeEvent)
        {
            OnSelectedChanged?.Invoke(selectedIndex);
        }
    }

    public void ReassertCurrentSelection(bool collapseInventorySelectionToHotbar = false, bool invokeEvent = true)
    {
        int inventoryIndex = collapseInventorySelectionToHotbar ? selectedIndex : selectedInventoryIndex;
        RestoreSelectionState(selectedIndex, inventoryIndex, invokeEvent);
    }

    public int GetResolvedPlacementSlotIndex(
        InventoryService inventoryOverride = null,
        ItemDatabase databaseOverride = null,
        ItemData expectedItem = null,
        int expectedQuality = 0)
    {
        ResolveReferences();

        InventoryService resolvedInventory = inventoryOverride ?? inventory;
        ItemDatabase resolvedDatabase = databaseOverride ?? database ?? resolvedInventory?.Database;
        int maxIndex = resolvedInventory != null ? Mathf.Max(0, resolvedInventory.Size - 1) : InventoryService.DefaultInventorySize - 1;

        int preferredIndex = Mathf.Clamp(selectedInventoryIndex, 0, maxIndex);
        if (SlotMatchesPlacementItem(resolvedInventory, resolvedDatabase, preferredIndex, expectedItem, expectedQuality))
        {
            return preferredIndex;
        }

        int hotbarIndex = Mathf.Clamp(selectedIndex, 0, InventoryService.HotbarWidth - 1);
        if (SlotMatchesPlacementItem(resolvedInventory, resolvedDatabase, hotbarIndex, expectedItem, expectedQuality))
        {
            return hotbarIndex;
        }

        return hotbarIndex;
    }

    private void ResolveReferences()
    {
        if (playerToolController == null)
        {
            playerToolController = FindFirstObjectByType<PlayerToolController>();
        }

        if (inventory == null)
        {
            inventory = FindFirstObjectByType<InventoryService>();
        }

        database = inventory != null ? inventory.Database : null;
    }

    private int NormalizeInventoryIndex(int inventoryIndex, int fallbackIndex)
    {
        int maxIndex = inventory != null
            ? Mathf.Max(0, inventory.Size - 1)
            : InventoryService.DefaultInventorySize - 1;

        if (inventoryIndex < 0)
        {
            return Mathf.Clamp(fallbackIndex, 0, maxIndex);
        }

        return Mathf.Clamp(inventoryIndex, 0, maxIndex);
    }

    private void BindInventoryEvents()
    {
        ResolveReferences();
        if (inventory == null)
        {
            return;
        }

        inventory.OnSlotChanged -= OnSlotChanged;
        inventory.OnSlotChanged += OnSlotChanged;
        inventory.OnHotbarSlotChanged -= OnHotbarSlotChanged;
        inventory.OnHotbarSlotChanged += OnHotbarSlotChanged;
    }

    private void UnbindInventoryEvents()
    {
        if (inventory == null)
        {
            return;
        }

        inventory.OnSlotChanged -= OnSlotChanged;
        inventory.OnHotbarSlotChanged -= OnHotbarSlotChanged;
    }

    private static bool SlotMatchesPlacementItem(
        InventoryService inventoryService,
        ItemDatabase itemDatabase,
        int slotIndex,
        ItemData expectedItem,
        int expectedQuality)
    {
        if (inventoryService == null ||
            itemDatabase == null ||
            slotIndex < 0 ||
            slotIndex >= inventoryService.Size)
        {
            return false;
        }

        ItemStack slot = inventoryService.GetSlot(slotIndex);
        if (slot.IsEmpty)
        {
            return false;
        }

        ItemData slotItem = itemDatabase.GetItemByID(slot.itemId);
        if (!SupportsPlacementMode(slotItem))
        {
            return false;
        }

        if (expectedItem == null)
        {
            return true;
        }

        return slot.itemId == expectedItem.itemID && slot.quality == expectedQuality;
    }

    /// <summary>
    /// 装备当前选中槽位的工具
    /// </summary>
    private void EquipCurrentTool()
    {
        ResolveReferences();
        if (playerToolController == null || inventory == null || database == null)
            return;

        var slot = inventory.GetSlot(selectedIndex);
        
        // 空槽位时清除当前装备并退出放置模式
        if (slot.IsEmpty)
        {
            playerToolController.UnequipCurrent();
            ExitPlacementModeIfActive();
            return;
        }

        var itemData = database.GetItemByID(slot.itemId);
        if (itemData == null) return;

        // ★ 检查是否是可放置物品
        if (SupportsPlacementMode(itemData))
        {
            playerToolController.UnequipCurrent();

            var inputManager = GameInputManager.Instance;
            if (inputManager == null || !inputManager.IsPlacementMode)
            {
                ExitPlacementModeIfActive();
                return;
            }

            if (PlacementManager.Instance != null)
            {
                PlacementManager.Instance.EnterPlacementMode(itemData, slot.quality);
            }
            else
            {
                Debug.LogError("[HotbarSelectionService] 没有找到 PlacementManager！");
            }
            return;
        }

        // 非放置物品，退出放置模式
        ExitPlacementModeIfActive();

        // 每个品质的工具都是独立 ID，直接使用 itemID 匹配动画
        if (itemData is ToolData toolData)
            playerToolController.EquipToolData(toolData);
        else if (itemData is WeaponData weaponData)
            playerToolController.EquipWeaponData(weaponData);
    }

    /// <summary>
    /// 如果处于放置模式则退出
    /// </summary>
    private void ExitPlacementModeIfActive()
    {
        if (PlacementManager.Instance != null && PlacementManager.Instance.IsPlacementMode)
        {
            PlacementManager.Instance.ExitPlacementMode();
        }
    }
}
