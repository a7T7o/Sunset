using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FarmGame.Data;

public class ToolbarUI : MonoBehaviour
{
    [SerializeField] private InventoryService inventory;
    [SerializeField] private ItemDatabase database;
    [SerializeField] private Transform gridParent; // 包含12个 Bar_00_TG
    [SerializeField] private HotbarSelectionService selection;

    private readonly List<ToolbarSlotUI> slots = new List<ToolbarSlotUI>(InventoryService.HotbarWidth);
    private readonly List<Toggle> toggles = new List<Toggle>(InventoryService.HotbarWidth);
    private InventoryService subscribedInventory;
    private HotbarSelectionService subscribedSelection;

    void Awake()
    {
        if (gridParent == null) gridParent = transform;
        ResolveRuntimeContextIfMissing();
    }

    void Start()
    {
        ResolveRuntimeContextIfMissing();
        Build();
        // 初始同步一次选中高亮
        if (selection != null) HandleSelectedChanged(selection.selectedIndex);
    }

    public void ConfigureRuntimeContext(
        InventoryService inventoryService,
        ItemDatabase itemDatabase,
        HotbarSelectionService hotbarSelection)
    {
        if (inventoryService != null)
        {
            inventory = inventoryService;
        }

        if (itemDatabase != null)
        {
            database = itemDatabase;
        }
        else if (inventory != null)
        {
            database = inventory.Database;
        }

        if (hotbarSelection != null)
        {
            selection = hotbarSelection;
        }

        SyncInventorySubscription();
        SyncSelectionSubscription();
    }

    public void Build()
    {
        ResolveRuntimeContextIfMissing();
        slots.Clear();
        toggles.Clear();
        List<Transform> orderedChildren = new List<Transform>(gridParent.childCount);
        foreach (Transform child in gridParent)
        {
            if (!IsToolbarSlotTransform(child))
            {
                continue;
            }

            orderedChildren.Add(child);
        }

        if (orderedChildren.Count != InventoryService.HotbarWidth)
        {
            Debug.LogWarning(
                $"[ToolbarUI] Grid 子槽位数量异常: expected={InventoryService.HotbarWidth}, actual={orderedChildren.Count}. 将按 sibling 顺序绑定。",
                this);
        }

        int bindCount = Mathf.Min(orderedChildren.Count, InventoryService.HotbarWidth);
        for (int resolvedIndex = 0; resolvedIndex < bindCount; resolvedIndex++)
        {
            Transform child = orderedChildren[resolvedIndex];
            var slot = child.GetComponent<ToolbarSlotUI>();
            if (slot == null) slot = child.gameObject.AddComponent<ToolbarSlotUI>();
            slot.Bind(inventory, database, selection, resolvedIndex);
            slots.Add(slot);
            var tg = child.GetComponent<Toggle>();
            if (tg != null) toggles.Add(tg);
        }
        // 若子物体不足12个，不报错，按现有数量绑定
        // 事件将在 OnEnable 中统一注册，避免重复
    }

    private static bool IsToolbarSlotTransform(Transform child)
    {
        if (child == null)
        {
            return false;
        }

        if (child.name.StartsWith("Bar_00_TG"))
        {
            return true;
        }

        return child.GetComponent<Toggle>() != null || child.GetComponent<ToolbarSlotUI>() != null;
    }

    void OnEnable()
    {
        ResolveRuntimeContextIfMissing();
        SyncInventorySubscription();
        SyncSelectionSubscription();
        if (selection != null) HandleSelectedChanged(selection.selectedIndex);
    }

    void OnDisable()
    {
        if (subscribedInventory != null)
        {
            subscribedInventory.OnInventoryChanged -= HandleInventoryChanged;
        }

        if (subscribedSelection != null)
        {
            subscribedSelection.OnSelectedChanged -= HandleSelectedChanged;
        }

        subscribedInventory = null;
        subscribedSelection = null;
    }

    public void ForceRefresh()
    {
        ResolveRuntimeContextIfMissing();
        foreach (var s in slots) s.Refresh();
    }

    void HandleSelectedChanged(int idx)
    {
        // 仅让每个Slot更新覆盖层，不再强制改 Toggle.isOn，避免白块/双触发
        foreach (var s in slots) s.RefreshSelection();
    }

    void HandleInventoryChanged()
    {
        ForceRefresh();
        if (selection != null)
        {
            HandleSelectedChanged(selection.selectedIndex);
        }
    }

    private void ResolveRuntimeContextIfMissing()
    {
        InventoryService preferredInventory = PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService();
        if (preferredInventory != null)
        {
            inventory = preferredInventory;
        }
        else if (inventory == null)
        {
            inventory = PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()
                ?? FindFirstObjectByType<InventoryService>();
        }

        if (inventory != null)
        {
            database = inventory.Database;
        }

        HotbarSelectionService preferredSelection = PersistentPlayerSceneBridge.GetPreferredRuntimeHotbarSelectionService();
        if (preferredSelection != null)
        {
            selection = preferredSelection;
        }
        else if (selection == null)
        {
            selection = PersistentPlayerSceneBridge.GetPreferredRuntimeHotbarSelectionService()
                ?? FindFirstObjectByType<HotbarSelectionService>();
        }

        SyncInventorySubscription();
        SyncSelectionSubscription();
    }

    private void SyncInventorySubscription()
    {
        if (subscribedInventory == inventory)
        {
            return;
        }

        if (subscribedInventory != null)
        {
            subscribedInventory.OnInventoryChanged -= HandleInventoryChanged;
        }

        subscribedInventory = inventory;
        if (!isActiveAndEnabled || subscribedInventory == null)
        {
            return;
        }

        subscribedInventory.OnInventoryChanged -= HandleInventoryChanged;
        subscribedInventory.OnInventoryChanged += HandleInventoryChanged;
    }

    private void SyncSelectionSubscription()
    {
        if (subscribedSelection == selection)
        {
            return;
        }

        if (subscribedSelection != null)
        {
            subscribedSelection.OnSelectedChanged -= HandleSelectedChanged;
        }

        subscribedSelection = selection;
        if (!isActiveAndEnabled || subscribedSelection == null)
        {
            return;
        }

        subscribedSelection.OnSelectedChanged -= HandleSelectedChanged;
        subscribedSelection.OnSelectedChanged += HandleSelectedChanged;
    }
}
