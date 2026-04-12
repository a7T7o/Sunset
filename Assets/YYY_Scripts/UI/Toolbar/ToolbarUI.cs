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

    public void Build()
    {
        ResolveRuntimeContextIfMissing();
        slots.Clear();
        toggles.Clear();
        List<Transform> orderedChildren = new List<Transform>(gridParent.childCount);
        foreach (Transform child in gridParent)
        {
            orderedChildren.Add(child);
        }

        orderedChildren.Sort((left, right) =>
        {
            int leftIndex = ResolveToolbarSlotIndex(left);
            int rightIndex = ResolveToolbarSlotIndex(right);
            int compare = leftIndex.CompareTo(rightIndex);
            if (compare != 0)
            {
                return compare;
            }

            return left.GetSiblingIndex().CompareTo(right.GetSiblingIndex());
        });

        int index = 0;
        foreach (Transform child in orderedChildren)
        {
            if (index >= InventoryService.HotbarWidth) break;
            var slot = child.GetComponent<ToolbarSlotUI>();
            if (slot == null) slot = child.gameObject.AddComponent<ToolbarSlotUI>();
            slot.Bind(inventory, database, selection, index);
            slots.Add(slot);
            var tg = child.GetComponent<Toggle>();
            if (tg != null) toggles.Add(tg);
            index++;
        }
        // 若子物体不足12个，不报错，按现有数量绑定
        // 事件将在 OnEnable 中统一注册，避免重复
    }

    private static int ResolveToolbarSlotIndex(Transform child)
    {
        if (child == null)
        {
            return int.MaxValue;
        }

        string childName = child.name;
        if (string.IsNullOrEmpty(childName))
        {
            return int.MaxValue - child.GetSiblingIndex();
        }

        if (!childName.StartsWith("Bar_00_TG"))
        {
            return int.MaxValue - child.GetSiblingIndex();
        }

        int openParen = childName.LastIndexOf('(');
        int closeParen = childName.LastIndexOf(')');
        if (openParen >= 0 && closeParen == childName.Length - 1 && closeParen > openParen + 1)
        {
            string cloneSuffix = childName.Substring(openParen + 1, closeParen - openParen - 1).Trim();
            if (int.TryParse(cloneSuffix, out int parsedCloneIndex))
            {
                return parsedCloneIndex;
            }
        }

        return 0;
    }

    void OnEnable()
    {
        ResolveRuntimeContextIfMissing();
        SyncSelectionSubscription();
        if (selection != null) HandleSelectedChanged(selection.selectedIndex);
    }

    void OnDisable()
    {
        if (subscribedSelection != null)
        {
            subscribedSelection.OnSelectedChanged -= HandleSelectedChanged;
        }
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

    private void ResolveRuntimeContextIfMissing()
    {
        if (inventory == null)
        {
            inventory = FindFirstObjectByType<InventoryService>();
        }

        if (database == null && inventory != null)
        {
            database = inventory.Database;
        }

        if (selection == null)
        {
            selection = FindFirstObjectByType<HotbarSelectionService>();
        }

        SyncSelectionSubscription();
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
