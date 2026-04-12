using UnityEngine;
using UnityEngine.UI;
using FarmGame.Data;

/// <summary>
/// 背包面板 UI - 简化版，与 ToolbarUI 保持一致的设计
/// </summary>
public class InventoryPanelUI : MonoBehaviour
{
    public static InventoryPanelUI ActiveVisibleInstance { get; private set; }

    [Header("Services & DB")]
    [SerializeField] private InventoryService inventory;
    [SerializeField] private EquipmentService equipment;
    [SerializeField] private ItemDatabase database;
    [SerializeField] private HotbarSelectionService selection;

    [Header("Layout")]
    [SerializeField] private Transform upParent;   // 36格：0..35，其中0..11为Hotbar映射
    [SerializeField] private Transform downParent; // 6格装备栏：0..5

    [Header("Limits")] 
    [SerializeField] private int upCount = 36;  // ★ 背包有 36 格（0-35）
    [SerializeField] private int downCount = 6;

    private int selectedInventoryIndex = -1;
    private int selectedEquipmentIndex = -1;
    private bool followHotbarSelection = true;
    private bool slotBindingsReady;
    private InventoryService boundInventory;
    private EquipmentService boundEquipment;
    private ItemDatabase boundDatabase;
    private HotbarSelectionService boundSelection;

    void Awake()
    {
        if (database == null && inventory != null)
        {
            database = inventory.Database;
        }
    }
    
    /// <summary>
    /// Rule: P1-1 背包刷新 - 每次面板激活时强制刷新
    /// </summary>
    void OnEnable()
    {
        ActiveVisibleInstance = this;
        EnsureBuilt();

        if (selection != null)
        {
            selection.OnSelectedChanged -= HandleHotbarSelectionChanged;
            selection.OnSelectedChanged += HandleHotbarSelectionChanged;
        }
        followHotbarSelection = true;
        SyncSelectionFromHotbar();
        RefreshSelectionOnly();
    }

    void OnDisable()
    {
        if (ReferenceEquals(ActiveVisibleInstance, this))
        {
            ActiveVisibleInstance = null;
        }

        if (selection != null)
        {
            selection.OnSelectedChanged -= HandleHotbarSelectionChanged;
        }
    }

    public void BuildUpSlots()
    {
        if (upParent == null)
        {
            Debug.LogError("[InventoryPanelUI] BuildUpSlots: upParent 为 null!");
            return;
        }
        if (inventory == null)
        {
            Debug.LogError("[InventoryPanelUI] BuildUpSlots: inventory 为 null!");
            return;
        }
        if (database == null)
        {
            Debug.LogError("[InventoryPanelUI] BuildUpSlots: database 为 null!");
            return;
        }
        
        int n = Mathf.Min(upCount, upParent.childCount);
        for (int i = 0; i < n; i++)
        {
            var child = upParent.GetChild(i);
            var slot = child.GetComponent<InventorySlotUI>();
            if (slot == null) slot = child.gameObject.AddComponent<InventorySlotUI>();
            bool isHotbar = i < InventoryService.HotbarWidth;
            slot.Bind(inventory, equipment, database, selection, this, null, i, isHotbar);
        }
    }

    public void BuildDownSlots()
    {
        if (downParent == null || equipment == null) return;
        int n = Mathf.Min(downCount, downParent.childCount);
        for (int i = 0; i < n; i++)
        {
            var child = downParent.GetChild(i);
            var slot = child.GetComponent<EquipmentSlotUI>();
            if (slot == null) slot = child.gameObject.AddComponent<EquipmentSlotUI>();
            slot.Bind(equipment, inventory, database, this, i);
        }
    }

    public void ConfigureRuntimeContext(
        InventoryService inventoryService,
        EquipmentService equipmentService,
        ItemDatabase itemDatabase,
        HotbarSelectionService hotbarSelection)
    {
        inventory = inventoryService;
        equipment = equipmentService;
        database = itemDatabase != null
            ? itemDatabase
            : inventoryService != null ? inventoryService.Database : database;
        selection = hotbarSelection;

        if (isActiveAndEnabled)
        {
            EnsureBuilt();
        }
    }

    // 在面板首次从未激活→激活时调用，确保所有格子已构建与绑定
    public void EnsureBuilt()
    {
        ResolveRuntimeContextIfMissing();

        // 持久 UI 在空场景切换时允许短暂等待服务重绑，不要把过渡态打成红错。
        if (inventory == null || database == null)
        {
            return;
        }

        bool needsRebind =
            !slotBindingsReady ||
            !ReferenceEquals(boundInventory, inventory) ||
            !ReferenceEquals(boundEquipment, equipment) ||
            !ReferenceEquals(boundDatabase, database) ||
            !ReferenceEquals(boundSelection, selection);

        if (needsRebind)
        {
            BuildUpSlots();
            BuildDownSlots();
            slotBindingsReady = true;
            boundInventory = inventory;
            boundEquipment = equipment;
            boundDatabase = database;
            boundSelection = selection;
            return;
        }

        RefreshDataOnly();
    }

    public void RefreshAll()
    {
        RefreshDataOnly();
        RefreshSelectionOnly();
    }

    private void ResolveRuntimeContextIfMissing()
    {
        if (inventory == null) inventory = FindFirstObjectByType<InventoryService>();
        if (equipment == null) equipment = FindFirstObjectByType<EquipmentService>();
        if (database == null && inventory != null) database = inventory.Database;
        if (selection == null) selection = FindFirstObjectByType<HotbarSelectionService>();
    }

    private void RefreshDataOnly()
    {
        if (upParent != null)
        {
            int n = Mathf.Min(upCount, upParent.childCount);
            for (int i = 0; i < n; i++)
            {
                var slot = upParent.GetChild(i).GetComponent<InventorySlotUI>();
                if (slot != null)
                {
                    slot.Refresh();
                }
            }
        }
        if (downParent != null)
        {
            int n = Mathf.Min(downCount, downParent.childCount);
            for (int i = 0; i < n; i++)
            {
                var slot = downParent.GetChild(i).GetComponent<EquipmentSlotUI>();
                if (slot != null)
                {
                    slot.Refresh();
                }
            }
        }
    }

    private void TryApplyHotbarSelectionToUp()
    {
        RefreshUpSelectionVisuals();
    }

    private void ClearDownSelection()
    {
        selectedEquipmentIndex = -1;
        RefreshDownSelectionVisuals();
    }

    // 由 PackagePanelTabsUI 在"主面板从关闭→打开"时调用
    public void ResetSelectionsOnPanelOpen()
    {
        SyncSelectionFromHotbar();
        followHotbarSelection = true;
        selectedEquipmentIndex = -1;
        RefreshSelectionOnly();
    }
    
    /// <summary>
    /// 清空背包槽位（Up 区域）的所有选中状态
    /// 供 Sort 后调用
    /// </summary>
    public void ClearUpSelection()
    {
        selectedInventoryIndex = -1;
        followHotbarSelection = false;
        RefreshUpSelectionVisuals();
    }

    public bool IsInventorySlotSelected(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex == selectedInventoryIndex;
    }

    public bool IsEquipmentSlotSelected(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex == selectedEquipmentIndex;
    }

    public void SetSelectedInventoryIndex(int slotIndex, bool syncHotbarSelection)
    {
        if (slotIndex < 0 || slotIndex >= upCount)
        {
            return;
        }

        selectedInventoryIndex = slotIndex;
        followHotbarSelection = syncHotbarSelection && slotIndex < InventoryService.HotbarWidth;

        if (selection != null)
        {
            selection.SelectInventoryIndex(slotIndex);
        }

        RefreshUpSelectionVisuals();
    }

    public void SetSelectedEquipmentIndex(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= downCount)
        {
            return;
        }

        selectedEquipmentIndex = slotIndex;
        RefreshDownSelectionVisuals();
    }

    public void ClearDownSelectionState()
    {
        ClearDownSelection();
    }

    private void SyncSelectionFromHotbar()
    {
        if (selection == null)
        {
            selectedInventoryIndex = selectedInventoryIndex >= 0 ? selectedInventoryIndex : 0;
            return;
        }

        selectedInventoryIndex = Mathf.Clamp(selection.selectedIndex, 0, Mathf.Max(0, upCount - 1));
    }

    private void HandleHotbarSelectionChanged(int selectedIndex)
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (followHotbarSelection || selectedInventoryIndex < 0)
        {
            SyncSelectionFromHotbar();
        }

        RefreshUpSelectionVisuals();
    }

    private void RefreshSelectionOnly()
    {
        RefreshUpSelectionVisuals();
        RefreshDownSelectionVisuals();
    }

    private void RefreshUpSelectionVisuals()
    {
        if (upParent == null)
        {
            return;
        }

        int n = Mathf.Min(upCount, upParent.childCount);
        for (int i = 0; i < n; i++)
        {
            var slot = upParent.GetChild(i).GetComponent<InventorySlotUI>();
            if (slot != null)
            {
                slot.RefreshSelection();
                continue;
            }

            var tg = upParent.GetChild(i).GetComponent<Toggle>();
            if (tg == null)
            {
                continue;
            }

            bool isSelected = i == selectedInventoryIndex;
#if UNITY_2021_2_OR_NEWER
            tg.SetIsOnWithoutNotify(isSelected);
#else
            tg.isOn = isSelected;
#endif
        }
    }

    private void RefreshDownSelectionVisuals()
    {
        if (downParent == null)
        {
            return;
        }

        int n = Mathf.Min(downCount, downParent.childCount);
        for (int i = 0; i < n; i++)
        {
            var slot = downParent.GetChild(i).GetComponent<EquipmentSlotUI>();
            if (slot != null)
            {
                slot.RefreshSelection();
                continue;
            }

            var tg = downParent.GetChild(i).GetComponent<Toggle>();
            if (tg == null)
            {
                continue;
            }

            bool isSelected = i == selectedEquipmentIndex;
#if UNITY_2021_2_OR_NEWER
            tg.SetIsOnWithoutNotify(isSelected);
#else
            tg.isOn = isSelected;
#endif
        }
    }
}
