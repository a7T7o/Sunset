using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using FarmGame.Data;

public class ChestAuthoringBatchSelectWindow : EditorWindow
{
    private static SerializedProperty targetSlotsProp;
    private static SerializedObject targetSerializedObject;
    private static int targetCapacity;

    private readonly List<ItemData> allItems = new List<ItemData>();
    private readonly HashSet<ItemData> selectedItems = new HashSet<ItemData>();

    private string searchFilter = string.Empty;
    private int categoryFilter = -1;
    private string[] categoryNames = new string[0];
    private Vector2 scrollPosition;

    private GUIStyle itemButtonStyle;
    private GUIStyle selectedButtonStyle;
    private bool stylesInitialized;

    public static void ShowWindow(SerializedProperty slotsProp, SerializedObject serializedObject, int capacity)
    {
        targetSlotsProp = slotsProp;
        targetSerializedObject = serializedObject;
        targetCapacity = capacity;

        ChestAuthoringBatchSelectWindow window = GetWindow<ChestAuthoringBatchSelectWindow>(true, "批量选择箱子默认物品", true);
        window.minSize = new Vector2(520f, 420f);
        window.LoadAllItems();
        window.Show();
    }

    private void LoadAllItems()
    {
        allItems.Clear();
        selectedItems.Clear();

        string[] guids = AssetDatabase.FindAssets("t:ItemData");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if (item != null)
            {
                allItems.Add(item);
            }
        }

        allItems.Sort((left, right) => left.itemID.CompareTo(right.itemID));
        categoryNames = System.Enum.GetNames(typeof(ItemCategory));
    }

    private void InitStyles()
    {
        if (stylesInitialized)
        {
            return;
        }

        itemButtonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(8, 8, 4, 4),
            fixedHeight = 34
        };

        selectedButtonStyle = new GUIStyle(itemButtonStyle);
        selectedButtonStyle.normal.background = MakeColorTexture(new Color(0.30f, 0.49f, 0.30f, 1f));
        selectedButtonStyle.normal.textColor = Color.white;

        stylesInitialized = true;
    }

    private Texture2D MakeColorTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }

    private void OnGUI()
    {
        InitStyles();
        DrawToolbar();
        EditorGUILayout.Space(6f);
        DrawItemGrid();
        EditorGUILayout.Space(6f);
        DrawBottomButtons();
    }

    private void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            GUILayout.Label("搜索:", GUILayout.Width(36f));
            searchFilter = EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField, GUILayout.Width(160f));
            if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.Width(20f)))
            {
                searchFilter = string.Empty;
                GUI.FocusControl(null);
            }

            GUILayout.Space(8f);
            GUILayout.Label("分类:", GUILayout.Width(34f));
            string[] filterOptions = new string[categoryNames.Length + 1];
            filterOptions[0] = "全部";
            for (int i = 0; i < categoryNames.Length; i++)
            {
                filterOptions[i + 1] = categoryNames[i];
            }

            categoryFilter = EditorGUILayout.Popup(categoryFilter + 1, filterOptions, EditorStyles.toolbarPopup, GUILayout.Width(110f)) - 1;

            GUILayout.FlexibleSpace();
            GUILayout.Label($"已选: {selectedItems.Count}", EditorStyles.miniLabel);

            if (GUILayout.Button("全选", EditorStyles.toolbarButton, GUILayout.Width(40f)))
            {
                SelectAllFiltered();
            }

            if (GUILayout.Button("取消", EditorStyles.toolbarButton, GUILayout.Width(40f)))
            {
                selectedItems.Clear();
            }
        }
    }

    private void DrawItemGrid()
    {
        List<ItemData> filteredItems = GetFilteredItems();
        if (filteredItems.Count == 0)
        {
            EditorGUILayout.HelpBox("没有找到匹配的物品。", MessageType.Info);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        int columns = Mathf.Max(1, (int)((position.width - 24f) / 200f));
        int itemIndex = 0;

        while (itemIndex < filteredItems.Count)
        {
            EditorGUILayout.BeginHorizontal();
            for (int column = 0; column < columns && itemIndex < filteredItems.Count; column++)
            {
                DrawItemButton(filteredItems[itemIndex]);
                itemIndex++;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawItemButton(ItemData item)
    {
        bool isSelected = selectedItems.Contains(item);
        GUIStyle style = isSelected ? selectedButtonStyle : itemButtonStyle;

        using (new EditorGUILayout.HorizontalScope(style, GUILayout.Width(190f), GUILayout.Height(34f)))
        {
            bool toggled = EditorGUILayout.Toggle(isSelected, GUILayout.Width(18f));
            if (toggled != isSelected)
            {
                if (toggled)
                {
                    selectedItems.Add(item);
                }
                else
                {
                    selectedItems.Remove(item);
                }
            }

            if (item.icon != null)
            {
                Rect iconRect = GUILayoutUtility.GetRect(24f, 24f, GUILayout.Width(24f), GUILayout.Height(24f));
                GUI.DrawTexture(iconRect, item.icon.texture, ScaleMode.ScaleToFit);
            }
            else
            {
                GUILayout.Space(24f);
            }

            GUILayout.Label($"[{item.itemID}] {item.itemName}", GUILayout.ExpandWidth(true));
        }

        Rect hitRect = GUILayoutUtility.GetLastRect();
        if (Event.current.type == EventType.MouseDown && hitRect.Contains(Event.current.mousePosition))
        {
            if (isSelected)
            {
                selectedItems.Remove(item);
            }
            else
            {
                selectedItems.Add(item);
            }

            Event.current.Use();
            Repaint();
        }
    }

    private void DrawBottomButtons()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (targetCapacity > 0)
            {
                GUILayout.Label($"目标容量：{targetCapacity}", EditorStyles.miniLabel);
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("取消", GUILayout.Width(80f), GUILayout.Height(25f)))
            {
                Close();
            }

            using (new EditorGUI.DisabledScope(selectedItems.Count == 0))
            {
                if (GUILayout.Button($"添加 ({selectedItems.Count})", GUILayout.Width(110f), GUILayout.Height(25f)))
                {
                    AddSelectedItems();
                    Close();
                }
            }
        }
    }

    private List<ItemData> GetFilteredItems()
    {
        IEnumerable<ItemData> result = allItems;
        if (categoryFilter >= 0 && categoryFilter < categoryNames.Length)
        {
            ItemCategory targetCategory = (ItemCategory)categoryFilter;
            result = result.Where(item => item.category == targetCategory);
        }

        if (!string.IsNullOrEmpty(searchFilter))
        {
            string filter = searchFilter.ToLowerInvariant();
            result = result.Where(item => item.itemName.ToLowerInvariant().Contains(filter) || item.itemID.ToString().Contains(filter));
        }

        return result.ToList();
    }

    private void SelectAllFiltered()
    {
        foreach (ItemData item in GetFilteredItems())
        {
            selectedItems.Add(item);
        }
    }

    private void AddSelectedItems()
    {
        if (targetSlotsProp == null || targetSerializedObject == null)
        {
            return;
        }

        targetSerializedObject.Update();
        int addedCount = 0;
        int skippedCount = 0;

        foreach (ItemData item in selectedItems.OrderBy(entry => entry.itemID))
        {
            int nextSlotIndex = GetNextAvailableSlotIndex();
            if (targetCapacity > 0 && nextSlotIndex >= targetCapacity)
            {
                skippedCount++;
                continue;
            }

            int newIndex = targetSlotsProp.arraySize;
            targetSlotsProp.InsertArrayElementAtIndex(newIndex);
            SerializedProperty newSlot = targetSlotsProp.GetArrayElementAtIndex(newIndex);
            newSlot.FindPropertyRelative("slotIndex").intValue = nextSlotIndex;
            newSlot.FindPropertyRelative("itemId").intValue = item.itemID;
            newSlot.FindPropertyRelative("quality").intValue = 0;
            newSlot.FindPropertyRelative("amount").intValue = 1;
            newSlot.FindPropertyRelative("instanceId").stringValue = string.Empty;
            newSlot.FindPropertyRelative("currentDurability").intValue = -1;
            newSlot.FindPropertyRelative("maxDurability").intValue = -1;
            newSlot.FindPropertyRelative("properties").ClearArray();
            addedCount++;
        }

        targetSerializedObject.ApplyModifiedProperties();

        if (addedCount > 0)
        {
            Debug.Log($"[ChestAuthoringBatchSelectWindow] 已添加 {addedCount} 个默认物品");
        }

        if (skippedCount > 0)
        {
            EditorUtility.DisplayDialog("箱子容量不足", $"有 {skippedCount} 个物品因为箱子容量不足而未加入默认内容。", "知道了");
        }
    }

    private int GetNextAvailableSlotIndex()
    {
        var usedSlots = new HashSet<int>();
        for (int i = 0; i < targetSlotsProp.arraySize; i++)
        {
            SerializedProperty slotProp = targetSlotsProp.GetArrayElementAtIndex(i);
            usedSlots.Add(Mathf.Max(0, slotProp.FindPropertyRelative("slotIndex").intValue));
        }

        int candidate = 0;
        while (usedSlots.Contains(candidate))
        {
            candidate++;
        }

        return candidate;
    }
}
