using UnityEngine;
using FarmGame.Data;

public class InventoryPanelUI : MonoBehaviour
{
    [Header("Services & DB")]
    [SerializeField] private InventoryService inventory;
    [SerializeField] private EquipmentService equipment;
    [SerializeField] private ItemDatabase database;
    [SerializeField] private HotbarSelectionService selection;

    [Header("Layout")]
    [SerializeField] private Transform upParent;   // 36格：0..35，其中0..11为Hotbar映射
    [SerializeField] private Transform downParent; // 6格装备栏：0..5

    [Header("Limits")] 
    [SerializeField] private int upCount = 36;
    [SerializeField] private int downCount = 6;

    void Awake()
    {
        if (inventory == null) inventory = FindFirstObjectByType<InventoryService>();
        if (equipment == null) equipment = FindFirstObjectByType<EquipmentService>();
        if (database == null) database = FindFirstObjectByType<ItemDatabase>();
        if (selection == null) selection = FindFirstObjectByType<HotbarSelectionService>();
    }

    void Start()
    {
        BuildUpSlots();
        BuildDownSlots();
    }

    void OnEnable()
    {
        // 不再在每次页面激活时重置选择，避免在主面板未关闭的情况下切页导致选择丢失
    }

    public void BuildUpSlots()
    {
        if (upParent == null || inventory == null) return;
        int n = Mathf.Min(upCount, upParent.childCount);
        for (int i = 0; i < n; i++)
        {
            var child = upParent.GetChild(i);
            var slot = child.GetComponent<InventorySlotUI>();
            if (slot == null) slot = child.gameObject.AddComponent<InventorySlotUI>();
            bool isHotbar = i < InventoryService.HotbarWidth;
            slot.Bind(inventory, equipment, database, i, isHotbar);
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
            slot.Bind(equipment, inventory, database, i);
        }
    }

    // 在面板首次从未激活→激活时调用，确保所有格子已构建与绑定
    public void EnsureBuilt()
    {
        BuildUpSlots();
        BuildDownSlots();
        RefreshAll();
    }

    public void RefreshAll()
    {
        // 触发一次全量刷新（依赖各自的Refresh）
        if (upParent != null)
        {
            int n = Mathf.Min(upCount, upParent.childCount);
            for (int i = 0; i < n; i++)
            {
                var slot = upParent.GetChild(i).GetComponent<InventorySlotUI>();
                if (slot != null) slot.Refresh();
            }
        }
        if (downParent != null)
        {
            int n = Mathf.Min(downCount, downParent.childCount);
            for (int i = 0; i < n; i++)
            {
                var slot = downParent.GetChild(i).GetComponent<EquipmentSlotUI>();
                if (slot != null) slot.Refresh();
            }
        }
    }

    private void TryApplyHotbarSelectionToUp()
    {
        if (selection == null || upParent == null) return;
        int idx = Mathf.Clamp(selection.selectedIndex, 0, Mathf.Min(InventoryService.HotbarWidth, upParent.childCount) - 1);
        for (int i = 0; i < Mathf.Min(upCount, upParent.childCount); i++)
        {
            var tg = upParent.GetChild(i).GetComponent<UnityEngine.UI.Toggle>();
            if (tg != null) tg.isOn = (i == idx);
        }
    }

    private void ClearDownSelection()
    {
        if (downParent == null) return;
        for (int i = 0; i < Mathf.Min(downCount, downParent.childCount); i++)
        {
            var tg = downParent.GetChild(i).GetComponent<UnityEngine.UI.Toggle>();
            if (tg != null) tg.isOn = false;
        }
    }

    // 由 PackagePanelTabsUI 在“主面板从关闭→打开”时调用
    public void ResetSelectionsOnPanelOpen()
    {
        TryApplyHotbarSelectionToUp();
        ClearDownSelection();
    }
}
