using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FarmGame.Data;
using FarmGame.World;

[CustomEditor(typeof(ChestController))]
public class ChestControllerEditor : Editor
{
    private SerializedProperty storageDataProp;
    private SerializedProperty authoringSlotsProp;

    private ItemDatabase itemDatabase;
    private readonly Dictionary<int, ItemData> itemCache = new Dictionary<int, ItemData>();

    private GUIStyle headerStyle;
    private GUIStyle boxStyle;
    private GUIStyle itemBoxStyle;
    private GUIStyle dropZoneStyle;
    private GUIStyle metaStyle;
    private bool stylesInitialized;

    private bool authoringFoldout = true;
    private Vector2 scrollPosition;
    private int dragTargetIndex = -1;
    private int dragSourceIndex = -1;
    private int dragOverIndex = -1;
    private bool isDraggingItem;

    private void OnEnable()
    {
        storageDataProp = serializedObject.FindProperty("storageData");
        authoringSlotsProp = serializedObject.FindProperty("_authoringSlots");
        RefreshItemDatabase();
    }

    public override void OnInspectorGUI()
    {
        InitStyles();
        serializedObject.Update();

        DrawScriptField();

        if (targets.Length == 1)
        {
            DrawAuthoringSection((ChestController)target);
            EditorGUILayout.Space(8f);
        }
        else
        {
            EditorGUILayout.HelpBox("箱子预设内容暂不支持多选批量编辑，请单独选中一个箱子。", MessageType.Info);
        }

        DrawPropertiesExcluding(serializedObject, "m_Script", "_authoringSlots");
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawScriptField()
    {
        using (new EditorGUI.DisabledScope(true))
        {
            MonoScript script = MonoScript.FromMonoBehaviour((ChestController)target);
            EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
        }
    }

    private void DrawAuthoringSection(ChestController controller)
    {
        authoringFoldout = EditorGUILayout.Foldout(authoringFoldout, "箱子预设内容", true);
        if (!authoringFoldout)
        {
            return;
        }

        int capacity = GetCapacity();
        int configuredCount = CountConfiguredEntries();
        int availableSlots = capacity > 0 ? Mathf.Max(0, capacity - CountDistinctSlots()) : -1;

        using (new EditorGUILayout.VerticalScope(boxStyle))
        {
            EditorGUILayout.LabelField("默认内容编辑器", headerStyle);
            EditorGUILayout.LabelField("一个箱子只有一个默认内容组。这里配置的是场景默认内容，不会覆盖正式读档结果。", metaStyle);

            if (itemDatabase == null)
            {
                EditorGUILayout.HelpBox("没找到 MasterItemDatabase。仍可手填物品 ID，但批量选择和名称提示会受限。", MessageType.Warning);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField($"容量：{(capacity > 0 ? capacity.ToString() : "未绑定")}", metaStyle, GUILayout.Width(84f));
                EditorGUILayout.LabelField($"已配置：{configuredCount}", metaStyle, GUILayout.Width(82f));
                EditorGUILayout.LabelField(capacity > 0 ? $"剩余：{availableSlots}" : "剩余：不限制", metaStyle, GUILayout.Width(82f));
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("刷新物品库", GUILayout.Width(88f)))
                {
                    RefreshItemDatabase();
                }
            }

            EditorGUILayout.Space(4f);
            DrawToolbar(controller, capacity);
            EditorGUILayout.Space(4f);

            if (authoringSlotsProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox("当前还没有默认内容。可以点“新增槽位”，也可以把 ItemData 直接拖到下面的投放区。", MessageType.None);
            }
            else
            {
                float scrollHeight = Mathf.Clamp(authoringSlotsProp.arraySize * 54f + 16f, 132f, 322f);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MinHeight(scrollHeight), GUILayout.MaxHeight(scrollHeight));
                DrawAuthoringEntries(capacity);
                EditorGUILayout.EndScrollView();
            }

            DrawDropZone(capacity);
            DrawWarnings(capacity);
        }
    }

    private void DrawToolbar(ChestController controller, int capacity)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("新增槽位", GUILayout.Height(24f)))
            {
                AddEmptyEntry(capacity);
            }

            if (GUILayout.Button("批量选择添加", GUILayout.Height(24f)))
            {
                ChestAuthoringBatchSelectWindow.ShowWindow(authoringSlotsProp, serializedObject, capacity);
            }

            if (GUILayout.Button("排序并清理", GUILayout.Height(24f)))
            {
                SortAndNormalizeSlots(controller);
            }

            if (GUILayout.Button("清空全部", GUILayout.Height(24f)))
            {
                if (EditorUtility.DisplayDialog("清空箱子预设", "确定要清空这个箱子的全部默认内容吗？", "清空", "取消"))
                {
                    Undo.RecordObject(controller, "Clear Chest Authoring Slots");
                    controller.ClearAuthoringSlots();
                    EditorUtility.SetDirty(controller);
                    serializedObject.Update();
                }
            }
        }
    }

    private void DrawAuthoringEntries(int capacity)
    {
        for (int i = 0; i < authoringSlotsProp.arraySize; i++)
        {
            if (DrawAuthoringEntry(i, capacity))
            {
                return;
            }
        }
    }

    private bool DrawAuthoringEntry(int itemIndex, int capacity)
    {
        SerializedProperty slotProp = authoringSlotsProp.GetArrayElementAtIndex(itemIndex);
        SerializedProperty slotIndexProp = slotProp.FindPropertyRelative("slotIndex");
        SerializedProperty itemIdProp = slotProp.FindPropertyRelative("itemId");
        SerializedProperty qualityProp = slotProp.FindPropertyRelative("quality");
        SerializedProperty amountProp = slotProp.FindPropertyRelative("amount");

        Rect rowRect = EditorGUILayout.BeginVertical(itemBoxStyle);

        bool isDragTarget = isDraggingItem && dragOverIndex == itemIndex;
        if (isDragTarget)
        {
            EditorGUI.DrawRect(rowRect, new Color(0.27f, 0.55f, 0.30f, 0.32f));
        }
        else if (itemIndex % 2 == 0)
        {
            EditorGUI.DrawRect(rowRect, new Color(0f, 0f, 0f, 0.05f));
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            Rect dragHandleRect = GUILayoutUtility.GetRect(18f, 22f, GUILayout.Width(18f));
            EditorGUIUtility.AddCursorRect(dragHandleRect, MouseCursor.Pan);
            GUI.Label(dragHandleRect, "≡", EditorStyles.centeredGreyMiniLabel);
            HandleEntryDrag(dragHandleRect, itemIndex);

            GUILayout.Label($"{itemIndex + 1}.", GUILayout.Width(22f));

            GUILayout.Label("槽位", GUILayout.Width(28f));
            slotIndexProp.intValue = Mathf.Max(0, EditorGUILayout.IntField(slotIndexProp.intValue, GUILayout.Width(42f)));

            ItemData resolvedItem = ResolveItemData(itemIdProp.intValue);
            if (resolvedItem != null && resolvedItem.icon != null)
            {
                Rect iconRect = GUILayoutUtility.GetRect(24f, 24f, GUILayout.Width(24f), GUILayout.Height(24f));
                GUI.DrawTexture(iconRect, resolvedItem.icon.texture, ScaleMode.ScaleToFit);
            }
            else
            {
                GUILayout.Space(24f);
            }

            EditorGUI.BeginChangeCheck();
            ItemData selectedItem = (ItemData)EditorGUILayout.ObjectField(resolvedItem, typeof(ItemData), false, GUILayout.MinWidth(120f));
            if (EditorGUI.EndChangeCheck())
            {
                itemIdProp.intValue = selectedItem != null ? selectedItem.itemID : -1;
                if (selectedItem != null && amountProp.intValue <= 0)
                {
                    amountProp.intValue = 1;
                }
            }

            GUILayout.Label("品质", GUILayout.Width(28f));
            qualityProp.intValue = Mathf.Clamp(EditorGUILayout.IntField(qualityProp.intValue, GUILayout.Width(34f)), 0, 4);

            GUILayout.Label("×", GUILayout.Width(10f));
            amountProp.intValue = Mathf.Max(1, EditorGUILayout.IntField(amountProp.intValue, GUILayout.Width(46f)));

            if (GUILayout.Button("⋮", GUILayout.Width(22f)))
            {
                ShowEntryContextMenu(itemIndex, capacity);
            }

            if (GUILayout.Button("×", GUILayout.Width(22f)))
            {
                authoringSlotsProp.DeleteArrayElementAtIndex(itemIndex);
                EditorGUILayout.EndVertical();
                GUIUtility.ExitGUI();
                return true;
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("ID", GUILayout.Width(16f));
            itemIdProp.intValue = Mathf.Max(-1, EditorGUILayout.IntField(itemIdProp.intValue, GUILayout.Width(64f)));
            GUILayout.Space(4f);
            EditorGUILayout.LabelField(BuildItemSummary(itemIdProp.intValue), metaStyle);
        }

        string warning = BuildEntryWarning(slotIndexProp.intValue, itemIdProp.intValue, amountProp.intValue, capacity);
        if (!string.IsNullOrWhiteSpace(warning))
        {
            EditorGUILayout.HelpBox(warning, MessageType.Warning);
        }

        EditorGUILayout.EndVertical();
        HandleEntryDragTarget(rowRect, itemIndex);
        return false;
    }

    private void DrawDropZone(int capacity)
    {
        Rect dropRect = GUILayoutUtility.GetRect(0f, 42f, GUILayout.ExpandWidth(true));
        bool isActiveTarget = dragTargetIndex >= 0;
        Color zoneColor = isActiveTarget
            ? new Color(0.32f, 0.54f, 0.32f, 0.42f)
            : new Color(0.22f, 0.22f, 0.22f, 0.18f);

        EditorGUI.DrawRect(dropRect, zoneColor);
        Handles.color = isActiveTarget ? new Color(0.31f, 0.58f, 0.31f, 0.95f) : new Color(0.44f, 0.44f, 0.44f, 0.85f);
        Handles.DrawSolidRectangleWithOutline(dropRect, Color.clear, Handles.color);
        GUI.Label(dropRect, isActiveTarget ? "松开以把物品加入这个箱子的默认内容" : "把 ItemData 拖到这里可直接加入箱子默认内容", dropZoneStyle);

        HandleExternalDragAndDrop(dropRect, capacity);
    }

    private void HandleExternalDragAndDrop(Rect dropRect, int capacity)
    {
        Event evt = Event.current;
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropRect.Contains(evt.mousePosition))
                {
                    dragTargetIndex = -1;
                    return;
                }

                List<ItemData> draggedItems = CollectDraggedItems();
                if (draggedItems.Count == 0)
                {
                    dragTargetIndex = -1;
                    return;
                }

                dragTargetIndex = 0;
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    int addedCount = AddItemsToAuthoring(draggedItems, capacity);
                    int skippedCount = draggedItems.Count - addedCount;
                    if (addedCount > 0)
                    {
                        Debug.Log($"[ChestControllerEditor] 已向箱子默认内容添加 {addedCount} 个物品");
                    }

                    if (skippedCount > 0)
                    {
                        EditorUtility.DisplayDialog("箱子容量不足", $"有 {skippedCount} 个物品因为箱子容量不足而未加入默认内容。", "知道了");
                    }

                    dragTargetIndex = -1;
                    GUI.changed = true;
                }

                evt.Use();
                Repaint();
                break;

            case EventType.DragExited:
                dragTargetIndex = -1;
                Repaint();
                break;
        }
    }

    private List<ItemData> CollectDraggedItems()
    {
        var items = new List<ItemData>();
        foreach (Object obj in DragAndDrop.objectReferences)
        {
            if (obj is ItemData itemData)
            {
                items.Add(itemData);
            }
        }

        return items;
    }

    private int AddItemsToAuthoring(IReadOnlyList<ItemData> items, int capacity)
    {
        int addedCount = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (AddItemEntry(items[i], capacity))
            {
                addedCount++;
            }
        }

        return addedCount;
    }

    private bool AddItemEntry(ItemData item, int capacity)
    {
        if (item == null)
        {
            return false;
        }

        int nextSlot = GetSuggestedSlotIndex();
        if (capacity > 0 && nextSlot >= capacity)
        {
            return false;
        }

        int newIndex = authoringSlotsProp.arraySize;
        authoringSlotsProp.InsertArrayElementAtIndex(newIndex);
        SerializedProperty slotProp = authoringSlotsProp.GetArrayElementAtIndex(newIndex);
        slotProp.FindPropertyRelative("slotIndex").intValue = nextSlot;
        slotProp.FindPropertyRelative("itemId").intValue = item.itemID;
        slotProp.FindPropertyRelative("quality").intValue = 0;
        slotProp.FindPropertyRelative("amount").intValue = 1;
        slotProp.FindPropertyRelative("instanceId").stringValue = string.Empty;
        slotProp.FindPropertyRelative("currentDurability").intValue = -1;
        slotProp.FindPropertyRelative("maxDurability").intValue = -1;
        slotProp.FindPropertyRelative("properties").ClearArray();
        return true;
    }

    private void AddEmptyEntry(int capacity)
    {
        int nextSlot = GetSuggestedSlotIndex();
        if (capacity > 0 && nextSlot >= capacity)
        {
            EditorUtility.DisplayDialog("箱子容量已满", "当前默认内容已经占满所有槽位，无法继续新增。", "知道了");
            return;
        }

        int newIndex = authoringSlotsProp.arraySize;
        authoringSlotsProp.InsertArrayElementAtIndex(newIndex);
        SerializedProperty slotProp = authoringSlotsProp.GetArrayElementAtIndex(newIndex);
        slotProp.FindPropertyRelative("slotIndex").intValue = nextSlot;
        slotProp.FindPropertyRelative("itemId").intValue = -1;
        slotProp.FindPropertyRelative("quality").intValue = 0;
        slotProp.FindPropertyRelative("amount").intValue = 1;
        slotProp.FindPropertyRelative("instanceId").stringValue = string.Empty;
        slotProp.FindPropertyRelative("currentDurability").intValue = -1;
        slotProp.FindPropertyRelative("maxDurability").intValue = -1;
        slotProp.FindPropertyRelative("properties").ClearArray();
    }

    private void ShowEntryContextMenu(int itemIndex, int capacity)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("复制当前条目"), false, () =>
        {
            DuplicateEntry(itemIndex, capacity);
            serializedObject.ApplyModifiedProperties();
        });
        menu.AddSeparator(string.Empty);

        if (itemIndex > 0)
        {
            menu.AddItem(new GUIContent("上移"), false, () =>
            {
                authoringSlotsProp.MoveArrayElement(itemIndex, itemIndex - 1);
                serializedObject.ApplyModifiedProperties();
            });
            menu.AddItem(new GUIContent("移到顶部"), false, () =>
            {
                authoringSlotsProp.MoveArrayElement(itemIndex, 0);
                serializedObject.ApplyModifiedProperties();
            });
        }
        else
        {
            menu.AddDisabledItem(new GUIContent("上移"));
            menu.AddDisabledItem(new GUIContent("移到顶部"));
        }

        if (itemIndex < authoringSlotsProp.arraySize - 1)
        {
            menu.AddItem(new GUIContent("下移"), false, () =>
            {
                authoringSlotsProp.MoveArrayElement(itemIndex, itemIndex + 1);
                serializedObject.ApplyModifiedProperties();
            });
            menu.AddItem(new GUIContent("移到底部"), false, () =>
            {
                authoringSlotsProp.MoveArrayElement(itemIndex, authoringSlotsProp.arraySize - 1);
                serializedObject.ApplyModifiedProperties();
            });
        }
        else
        {
            menu.AddDisabledItem(new GUIContent("下移"));
            menu.AddDisabledItem(new GUIContent("移到底部"));
        }

        menu.AddSeparator(string.Empty);
        menu.AddItem(new GUIContent("删除"), false, () =>
        {
            authoringSlotsProp.DeleteArrayElementAtIndex(itemIndex);
            serializedObject.ApplyModifiedProperties();
        });
        menu.ShowAsContext();
    }

    private void DuplicateEntry(int itemIndex, int capacity)
    {
        SerializedProperty sourceProp = authoringSlotsProp.GetArrayElementAtIndex(itemIndex);
        int nextSlot = GetSuggestedSlotIndex();
        if (capacity > 0 && nextSlot >= capacity)
        {
            EditorUtility.DisplayDialog("箱子容量已满", "复制失败：当前默认内容已经占满所有槽位。", "知道了");
            return;
        }

        int newIndex = itemIndex + 1;
        authoringSlotsProp.InsertArrayElementAtIndex(newIndex);
        SerializedProperty newProp = authoringSlotsProp.GetArrayElementAtIndex(newIndex);
        newProp.FindPropertyRelative("slotIndex").intValue = nextSlot;
        newProp.FindPropertyRelative("itemId").intValue = sourceProp.FindPropertyRelative("itemId").intValue;
        newProp.FindPropertyRelative("quality").intValue = sourceProp.FindPropertyRelative("quality").intValue;
        newProp.FindPropertyRelative("amount").intValue = sourceProp.FindPropertyRelative("amount").intValue;
        newProp.FindPropertyRelative("instanceId").stringValue = string.Empty;
        newProp.FindPropertyRelative("currentDurability").intValue = -1;
        newProp.FindPropertyRelative("maxDurability").intValue = -1;
        newProp.FindPropertyRelative("properties").ClearArray();
    }

    private void SortAndNormalizeSlots(ChestController controller)
    {
        serializedObject.ApplyModifiedProperties();
        Undo.RecordObject(controller, "Sort Chest Authoring Slots");
        controller.SetAuthoringSlotsFromEditor(controller.GetAuthoringSlotsSnapshot());
        EditorUtility.SetDirty(controller);
        serializedObject.Update();
    }

    private void HandleEntryDrag(Rect handleRect, int itemIndex)
    {
        Event evt = Event.current;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        switch (evt.type)
        {
            case EventType.MouseDown:
                if (handleRect.Contains(evt.mousePosition) && evt.button == 0)
                {
                    GUIUtility.hotControl = controlId;
                    isDraggingItem = true;
                    dragSourceIndex = itemIndex;
                    dragOverIndex = itemIndex;
                    evt.Use();
                }
                break;

            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlId)
                {
                    evt.Use();
                    Repaint();
                }
                break;

            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlId)
                {
                    GUIUtility.hotControl = 0;
                    if (isDraggingItem && dragSourceIndex >= 0 && dragOverIndex >= 0 && dragSourceIndex != dragOverIndex)
                    {
                        authoringSlotsProp.MoveArrayElement(dragSourceIndex, dragOverIndex);
                        serializedObject.ApplyModifiedProperties();
                    }

                    ResetDragState();
                    evt.Use();
                    Repaint();
                }
                break;
        }
    }

    private void HandleEntryDragTarget(Rect rowRect, int itemIndex)
    {
        if (!isDraggingItem)
        {
            return;
        }

        Event evt = Event.current;
        if ((evt.type == EventType.MouseDrag || evt.type == EventType.Repaint) && rowRect.Contains(evt.mousePosition))
        {
            dragOverIndex = itemIndex;
        }
    }

    private void ResetDragState()
    {
        isDraggingItem = false;
        dragSourceIndex = -1;
        dragOverIndex = -1;
        dragTargetIndex = -1;
    }

    private void DrawWarnings(int capacity)
    {
        var duplicateSlots = new List<int>();
        var seenSlots = new HashSet<int>();
        int unresolvedItems = 0;
        int outOfRangeSlots = 0;
        int overflowStacks = 0;

        for (int i = 0; i < authoringSlotsProp.arraySize; i++)
        {
            SerializedProperty slotProp = authoringSlotsProp.GetArrayElementAtIndex(i);
            int slotIndex = slotProp.FindPropertyRelative("slotIndex").intValue;
            int itemId = slotProp.FindPropertyRelative("itemId").intValue;
            int amount = slotProp.FindPropertyRelative("amount").intValue;

            if (!seenSlots.Add(slotIndex) && !duplicateSlots.Contains(slotIndex))
            {
                duplicateSlots.Add(slotIndex);
            }

            if (capacity > 0 && slotIndex >= capacity)
            {
                outOfRangeSlots++;
            }

            ItemData itemData = ResolveItemData(itemId);
            if (itemId >= 0 && itemData == null)
            {
                unresolvedItems++;
            }
            else if (itemData != null && amount > itemData.maxStackSize)
            {
                overflowStacks++;
            }
        }

        if (duplicateSlots.Count > 0)
        {
            EditorGUILayout.HelpBox($"存在重复槽位：{string.Join(", ", duplicateSlots)}。真正应用时会按最后一条覆盖前面的同槽位内容。", MessageType.Warning);
        }

        if (outOfRangeSlots > 0 || unresolvedItems > 0 || overflowStacks > 0)
        {
            string summary =
                $"越界槽位：{outOfRangeSlots}  条，未知物品：{unresolvedItems}  条，超最大堆叠：{overflowStacks}  条。";
            EditorGUILayout.HelpBox(summary, MessageType.Info);
        }
    }

    private int CountConfiguredEntries()
    {
        int count = 0;
        for (int i = 0; i < authoringSlotsProp.arraySize; i++)
        {
            SerializedProperty slotProp = authoringSlotsProp.GetArrayElementAtIndex(i);
            int itemId = slotProp.FindPropertyRelative("itemId").intValue;
            int amount = slotProp.FindPropertyRelative("amount").intValue;
            if (itemId >= 0 && amount > 0)
            {
                count++;
            }
        }

        return count;
    }

    private int CountDistinctSlots()
    {
        var slots = new HashSet<int>();
        for (int i = 0; i < authoringSlotsProp.arraySize; i++)
        {
            SerializedProperty slotProp = authoringSlotsProp.GetArrayElementAtIndex(i);
            slots.Add(Mathf.Max(0, slotProp.FindPropertyRelative("slotIndex").intValue));
        }

        return slots.Count;
    }

    private int GetSuggestedSlotIndex()
    {
        var usedSlots = new HashSet<int>();
        for (int i = 0; i < authoringSlotsProp.arraySize; i++)
        {
            SerializedProperty slotProp = authoringSlotsProp.GetArrayElementAtIndex(i);
            usedSlots.Add(Mathf.Max(0, slotProp.FindPropertyRelative("slotIndex").intValue));
        }

        int candidate = 0;
        while (usedSlots.Contains(candidate))
        {
            candidate++;
        }

        return candidate;
    }

    private string BuildItemSummary(int itemId)
    {
        if (itemId < 0)
        {
            return "未选择物品";
        }

        ItemData itemData = ResolveItemData(itemId);
        if (itemData == null)
        {
            return $"未知物品 ID {itemId}";
        }

        return $"{itemData.itemName}  (ID={itemData.itemID}, 最大堆叠={itemData.maxStackSize})";
    }

    private string BuildEntryWarning(int slotIndex, int itemId, int amount, int capacity)
    {
        if (capacity > 0 && slotIndex >= capacity)
        {
            return $"槽位 {slotIndex} 超出当前容量 {capacity}，运行时会忽略这条默认内容。";
        }

        ItemData itemData = ResolveItemData(itemId);
        if (itemId >= 0 && itemData == null)
        {
            return $"当前找不到物品 ID {itemId} 对应的 ItemData，请检查物品库或手填是否正确。";
        }

        if (itemData != null && amount > itemData.maxStackSize)
        {
            return $"{itemData.itemName} 的最大堆叠是 {itemData.maxStackSize}，当前数量 {amount} 会超出正常堆叠。";
        }

        return string.Empty;
    }

    private ItemData ResolveItemData(int itemId)
    {
        if (itemId < 0)
        {
            return null;
        }

        if (itemCache.TryGetValue(itemId, out ItemData itemData))
        {
            return itemData;
        }

        if (itemDatabase == null)
        {
            return null;
        }

        RefreshItemDatabase();
        itemCache.TryGetValue(itemId, out itemData);
        return itemData;
    }

    private void RefreshItemDatabase()
    {
        itemDatabase = Resources.Load<ItemDatabase>("Data/Database/MasterItemDatabase");
        if (itemDatabase == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:ItemDatabase MasterItemDatabase");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                itemDatabase = AssetDatabase.LoadAssetAtPath<ItemDatabase>(path);
            }
        }

        itemCache.Clear();
        if (itemDatabase == null || itemDatabase.allItems == null)
        {
            return;
        }

        foreach (ItemData item in itemDatabase.allItems)
        {
            if (item == null || itemCache.ContainsKey(item.itemID))
            {
                continue;
            }

            itemCache.Add(item.itemID, item);
        }
    }

    private int GetCapacity()
    {
        StorageData storageData = storageDataProp != null ? storageDataProp.objectReferenceValue as StorageData : null;
        return storageData != null ? Mathf.Max(0, storageData.storageCapacity) : 0;
    }

    private void InitStyles()
    {
        if (stylesInitialized)
        {
            return;
        }

        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleLeft
        };

        boxStyle = new GUIStyle("box")
        {
            padding = new RectOffset(10, 10, 10, 10),
            margin = new RectOffset(0, 0, 4, 4)
        };

        itemBoxStyle = new GUIStyle("box")
        {
            padding = new RectOffset(6, 6, 5, 5),
            margin = new RectOffset(0, 0, 2, 2)
        };

        dropZoneStyle = new GUIStyle("box")
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Italic,
            normal = { textColor = new Color(0.58f, 0.58f, 0.58f) }
        };

        metaStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            wordWrap = true
        };

        stylesInitialized = true;
    }
}
