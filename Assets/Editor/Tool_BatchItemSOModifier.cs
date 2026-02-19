using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FarmGame.Data;

/// <summary>
/// 批量修改物品 SO 工具（增强版）
/// 基于 SerializedProperty 反射，自动发现所有序列化字段
/// 计算选中 SO 的最近公共祖先（LCA）类型，只显示共有属性
/// 
/// 功能：
/// - 自动跟随 Project 窗口选择
/// - 反射发现所有序列化字段（无需硬编码）
/// - LCA 类型计算，只显示属性交集
/// - 勾选框控制修改（未勾选保持原值）
/// - Header 分组 + 折叠
/// - 全选/全不选
/// - 修改后自动同步数据库
/// 
/// 菜单：Tools/📝 批量修改物品 SO
/// </summary>
public class Tool_BatchItemSOModifier : EditorWindow
{
    #region 内部类

    /// <summary>属性条目</summary>
    private class PropertyEntry
    {
        public string propertyPath;
        public string displayName;
        public string headerGroup;
        public bool isEnabled;
        public SerializedPropertyType propertyType;
    }

    /// <summary>Header 分组</summary>
    private class HeaderGroup
    {
        public string name;
        public bool isFolded;
        public List<PropertyEntry> properties = new List<PropertyEntry>();
    }

    #endregion

    #region 字段

    private Vector2 scrollPos;
    private Vector2 soListScrollPos;

    // 选中的 SO
    private List<ItemData> selectedItems = new List<ItemData>();

    // LCA 类型
    private Type lcaType;

    // 模板 SerializedObject（用第一个 SO 创建，用于渲染编辑器）
    private SerializedObject templateSO;

    // 属性分组列表
    private List<HeaderGroup> headerGroups = new List<HeaderGroup>();

    // 排除的属性路径
    private static readonly HashSet<string> ExcludedPaths = new HashSet<string>
    {
        "m_Script", "m_Name", "m_ObjectHideFlags",
        "itemID", "itemName", "icon"
    };

    #endregion

    [MenuItem("Tools/📝 批量修改物品 SO")]
    public static void ShowWindow()
    {
        var window = GetWindow<Tool_BatchItemSOModifier>("批量修改物品SO");
        window.minSize = new Vector2(500, 600);
        window.Show();
    }

    private void OnEnable()
    {
        RefreshSelection();
        Selection.selectionChanged += OnSelectionChanged;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged()
    {
        RefreshSelection();
        Repaint();
    }

    #region 选择刷新 + LCA

    private void RefreshSelection()
    {
        selectedItems.Clear();
        lcaType = null;
        templateSO = null;
        headerGroups.Clear();

        foreach (var obj in Selection.objects)
        {
            if (obj is ItemData item && !selectedItems.Contains(item))
                selectedItems.Add(item);
        }

        if (selectedItems.Count == 0) return;

        // 按名称排序
        selectedItems = selectedItems.OrderBy(i => i.itemName).ToList();

        // 计算 LCA
        lcaType = ComputeLCA(selectedItems.Select(i => i.GetType()));

        // 创建模板 SerializedObject
        templateSO = new SerializedObject(selectedItems[0]);

        // 构建属性列表
        try
        {
            BuildPropertyList();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[BatchSOModifier] 构建属性列表失败: {e}");
            headerGroups.Clear();
        }
    }

    /// <summary>
    /// 计算最近公共祖先类型（LCA）
    /// 对每个类型构建到 ItemData 的继承链，找到所有链共有的最深类型
    /// </summary>
    private Type ComputeLCA(IEnumerable<Type> types)
    {
        List<List<Type>> chains = new List<List<Type>>();

        foreach (var t in types)
        {
            var chain = new List<Type>();
            var current = t;
            while (current != null && typeof(ItemData).IsAssignableFrom(current))
            {
                chain.Insert(0, current); // 从 ItemData 开始
                current = current.BaseType;
            }
            chains.Add(chain);
        }

        if (chains.Count == 0) return typeof(ItemData);

        // 从根（ItemData）开始，找到所有链共有的最深类型
        Type lca = typeof(ItemData);
        int minLen = chains.Min(c => c.Count);

        for (int i = 0; i < minLen; i++)
        {
            Type candidate = chains[0][i];
            if (chains.All(c => c[i] == candidate))
                lca = candidate;
            else
                break;
        }

        return lca;
    }

    #endregion

    #region 构建属性列表

    /// <summary>
    /// 遍历模板 SO 的所有 SerializedProperty，过滤并按 Header 分组
    /// </summary>
    private void BuildPropertyList()
    {
        headerGroups.Clear();
        if (templateSO == null || lcaType == null) return;

        templateSO.Update();

        // 收集 LCA 类型及其父类中声明的字段名（用于过滤）
        var allowedFields = GetDeclaredFieldNames(lcaType);

        // 收集带 [Obsolete] 的字段名
        var obsoleteFields = GetObsoleteFieldNames(lcaType);

        // 收集字段的 Header 信息
        var fieldHeaders = GetFieldHeaders(lcaType);

        string currentHeader = "=== 通用属性 ===";
        var currentGroup = GetOrCreateGroup(currentHeader);

        var iterator = templateSO.GetIterator();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;
            string path = iterator.propertyPath;

            // 排除
            if (ExcludedPaths.Contains(path)) continue;
            if (path.Contains(".")) continue; // 跳过嵌套子属性
            if (obsoleteFields.Contains(path)) continue;

            // 只保留 LCA 及其父类中声明的字段
            if (!allowedFields.Contains(path)) continue;

            // 检查 Header
            if (fieldHeaders.TryGetValue(path, out string header))
            {
                currentHeader = header;
                currentGroup = GetOrCreateGroup(currentHeader);
            }

            var entry = new PropertyEntry
            {
                propertyPath = path,
                displayName = iterator.displayName,
                headerGroup = currentHeader,
                isEnabled = false,
                propertyType = iterator.propertyType
            };

            currentGroup.properties.Add(entry);
        }

        // 移除空分组
        headerGroups.RemoveAll(g => g.properties.Count == 0);
    }

    /// <summary>获取类型及其所有父类（到 ItemData）中声明的字段名</summary>
    private HashSet<string> GetDeclaredFieldNames(Type type)
    {
        var names = new HashSet<string>();
        var current = type;
        while (current != null && typeof(ItemData).IsAssignableFrom(current))
        {
            var fields = current.GetFields(
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var f in fields)
                names.Add(f.Name);
            current = current.BaseType;
        }
        return names;
    }

    /// <summary>获取带 [Obsolete] 特性的字段名</summary>
    private HashSet<string> GetObsoleteFieldNames(Type type)
    {
        var names = new HashSet<string>();
        var current = type;
        while (current != null && typeof(ItemData).IsAssignableFrom(current))
        {
            var fields = current.GetFields(
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var f in fields)
            {
                if (f.GetCustomAttribute<ObsoleteAttribute>() != null)
                    names.Add(f.Name);
            }
            current = current.BaseType;
        }
        return names;
    }

    /// <summary>获取字段到 Header 文本的映射</summary>
    private Dictionary<string, string> GetFieldHeaders(Type type)
    {
        var headers = new Dictionary<string, string>();
        var current = type;

        // 从基类到子类收集，子类覆盖
        var typeChain = new List<Type>();
        while (current != null && typeof(ItemData).IsAssignableFrom(current))
        {
            typeChain.Insert(0, current);
            current = current.BaseType;
        }

        foreach (var t in typeChain)
        {
            var fields = t.GetFields(
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var f in fields)
            {
                var headerAttrs = f.GetCustomAttributes<HeaderAttribute>().ToArray();
                if (headerAttrs.Length > 0)
                    headers[f.Name] = headerAttrs[headerAttrs.Length - 1].header;
            }
        }

        return headers;
    }

    private HeaderGroup GetOrCreateGroup(string name)
    {
        var group = headerGroups.Find(g => g.name == name);
        if (group == null)
        {
            group = new HeaderGroup { name = name, isFolded = false };
            headerGroups.Add(group);
        }
        return group;
    }

    #endregion

    #region OnGUI

    private void OnGUI()
    {
        DrawHeader();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        try
        {
            DrawSOSelection();
            DrawSeparator();

            if (selectedItems.Count > 0 && headerGroups.Count > 0)
            {
                DrawSelectButtons();
                DrawSeparator();
                DrawPropertyGroups();
                DrawSeparator();
                DrawApplyButton();
            }
        }
        catch (System.Exception e)
        {
            EditorGUILayout.HelpBox($"绘制出错: {e.Message}", MessageType.Error);
            Debug.LogException(e);
        }

        EditorGUILayout.EndScrollView();
    }

    #endregion

    #region UI 绘制

    private void DrawHeader()
    {
        var style = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter
        };
        EditorGUILayout.LabelField("📝 批量修改物品 SO（增强版）", style, GUILayout.Height(30));
    }

    private void DrawSOSelection()
    {
        EditorGUILayout.LabelField("🖼️ 选中的 SO（自动跟随 Project 选择）", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (selectedItems.Count == 0)
        {
            EditorGUILayout.LabelField("⚠️ 请在 Project 窗口选择 ItemData 资产", EditorStyles.miniLabel);
        }
        else
        {
            string typeInfo = lcaType != null ? $"（共同类型: {lcaType.Name}）" : "";
            EditorGUILayout.LabelField(
                $"✓ 已选择 {selectedItems.Count} 个 SO {typeInfo}",
                EditorStyles.boldLabel);
        }

        if (GUILayout.Button("🔄 刷新", GUILayout.Width(60)))
            RefreshSelection();

        EditorGUILayout.EndHorizontal();

        if (selectedItems.Count > 0)
        {
            float listHeight = Mathf.Min(selectedItems.Count * 22 + 10, 150);
            soListScrollPos = EditorGUILayout.BeginScrollView(
                soListScrollPos, EditorStyles.helpBox, GUILayout.Height(listHeight));

            foreach (var item in selectedItems)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"[{item.itemID}] {item.itemName}", GUILayout.Width(200));
                EditorGUILayout.LabelField($"({item.GetType().Name})", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawSelectButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("✅ 全选", GUILayout.Width(80)))
        {
            foreach (var g in headerGroups)
                foreach (var p in g.properties)
                    p.isEnabled = true;
        }

        if (GUILayout.Button("❌ 全不选", GUILayout.Width(80)))
        {
            foreach (var g in headerGroups)
                foreach (var p in g.properties)
                    p.isEnabled = false;
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawPropertyGroups()
    {
        if (templateSO == null) return;
        templateSO.Update();

        foreach (var group in headerGroups)
        {
            group.isFolded = EditorGUILayout.Foldout(group.isFolded, group.name, true, EditorStyles.foldoutHeader);

            if (!group.isFolded) continue;

            EditorGUI.indentLevel++;

            foreach (var entry in group.properties)
            {
                var prop = templateSO.FindProperty(entry.propertyPath);
                if (prop == null) continue;

                EditorGUILayout.BeginHorizontal();

                // Toggle
                entry.isEnabled = EditorGUILayout.Toggle(entry.isEnabled, GUILayout.Width(20));

                // 属性编辑器
                EditorGUI.BeginDisabledGroup(!entry.isEnabled);
                EditorGUILayout.PropertyField(prop, new GUIContent(entry.displayName), true);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }

        templateSO.ApplyModifiedPropertiesWithoutUndo();
    }

    private void DrawApplyButton()
    {
        EditorGUILayout.Space(10);

        int enabledCount = headerGroups.Sum(g => g.properties.Count(p => p.isEnabled));

        GUI.enabled = selectedItems.Count > 0 && enabledCount > 0;
        GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);

        if (GUILayout.Button(
            $"🚀 应用修改到 {selectedItems.Count} 个 SO（{enabledCount} 个字段）",
            GUILayout.Height(45)))
        {
            if (EditorUtility.DisplayDialog("确认批量修改",
                $"将修改 {selectedItems.Count} 个 SO 的 {enabledCount} 个字段。\n此操作支持 Undo。",
                "确认修改", "取消"))
            {
                ApplyModifications();
            }
        }

        GUI.backgroundColor = Color.white;
        GUI.enabled = true;

        if (selectedItems.Count == 0)
            EditorGUILayout.HelpBox("请先在 Project 窗口选择 ItemData 资产", MessageType.Warning);
        else if (enabledCount == 0)
            EditorGUILayout.HelpBox("请至少勾选一个要修改的字段", MessageType.Warning);
    }

    private void DrawSeparator()
    {
        EditorGUILayout.Space(5);
        var rect = EditorGUILayout.GetControlRect(false, 2);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
        EditorGUILayout.Space(5);
    }

    #endregion

    #region 应用修改

    /// <summary>
    /// 将模板 SO 中勾选的属性值复制到所有选中的 SO
    /// </summary>
    private void ApplyModifications()
    {
        if (templateSO == null || selectedItems.Count == 0) return;

        templateSO.Update();

        // 收集启用的属性
        var enabledEntries = headerGroups
            .SelectMany(g => g.properties)
            .Where(p => p.isEnabled)
            .ToList();

        if (enabledEntries.Count == 0) return;

        int modifiedCount = 0;

        foreach (var item in selectedItems)
        {
            var targetSO = new SerializedObject(item);

            Undo.RecordObject(item, "批量修改 SO");

            foreach (var entry in enabledEntries)
            {
                var srcProp = templateSO.FindProperty(entry.propertyPath);
                var dstProp = targetSO.FindProperty(entry.propertyPath);

                if (srcProp == null || dstProp == null) continue;

                CopySerializedProperty(srcProp, dstProp);
            }

            targetSO.ApplyModifiedProperties();
            EditorUtility.SetDirty(item);
            modifiedCount++;
        }

        AssetDatabase.SaveAssets();

        // 自动同步数据库
        string syncMsg = "";
        if (modifiedCount > 0 && DatabaseSyncHelper.DatabaseExists())
        {
            int syncCount = DatabaseSyncHelper.AutoCollectAllItems();
            if (syncCount >= 0)
                syncMsg = $"\n\n✅ 数据库已自动同步（共 {syncCount} 个物品）";
        }

        EditorUtility.DisplayDialog("完成",
            $"成功修改 {modifiedCount} 个 SO（{enabledEntries.Count} 个字段）{syncMsg}",
            "确定");

        Debug.Log($"<color=green>[批量修改] ✅ 完成！修改 {modifiedCount} 个 SO，{enabledEntries.Count} 个字段</color>");
    }

    /// <summary>
    /// 按类型分支复制 SerializedProperty 的值
    /// </summary>
    private void CopySerializedProperty(SerializedProperty src, SerializedProperty dst)
    {
        if (src.propertyType != dst.propertyType) return;

        switch (src.propertyType)
        {
            case SerializedPropertyType.Integer:
                dst.intValue = src.intValue;
                break;
            case SerializedPropertyType.Boolean:
                dst.boolValue = src.boolValue;
                break;
            case SerializedPropertyType.Float:
                dst.floatValue = src.floatValue;
                break;
            case SerializedPropertyType.String:
                dst.stringValue = src.stringValue;
                break;
            case SerializedPropertyType.Enum:
                dst.enumValueIndex = src.enumValueIndex;
                break;
            case SerializedPropertyType.ObjectReference:
                dst.objectReferenceValue = src.objectReferenceValue;
                break;
            case SerializedPropertyType.Vector2:
                dst.vector2Value = src.vector2Value;
                break;
            case SerializedPropertyType.Vector2Int:
                dst.vector2IntValue = src.vector2IntValue;
                break;
            case SerializedPropertyType.Vector3:
                dst.vector3Value = src.vector3Value;
                break;
            case SerializedPropertyType.Vector3Int:
                dst.vector3IntValue = src.vector3IntValue;
                break;
            case SerializedPropertyType.Vector4:
                dst.vector4Value = src.vector4Value;
                break;
            case SerializedPropertyType.Color:
                dst.colorValue = src.colorValue;
                break;
            case SerializedPropertyType.Rect:
                dst.rectValue = src.rectValue;
                break;
            case SerializedPropertyType.Bounds:
                dst.boundsValue = src.boundsValue;
                break;
            case SerializedPropertyType.AnimationCurve:
                dst.animationCurveValue = src.animationCurveValue;
                break;
            case SerializedPropertyType.LayerMask:
                dst.intValue = src.intValue;
                break;
            default:
                // 对于复杂类型（Generic 等），尝试逐子属性复制
                if (src.hasChildren)
                {
                    var srcChild = src.Copy();
                    var dstChild = dst.Copy();
                    var srcEnd = src.GetEndProperty();

                    srcChild.NextVisible(true);
                    dstChild.NextVisible(true);

                    while (!SerializedProperty.EqualContents(srcChild, srcEnd))
                    {
                        CopySerializedProperty(srcChild, dstChild);
                        if (!srcChild.NextVisible(false)) break;
                        if (!dstChild.NextVisible(false)) break;
                    }
                }
                break;
        }
    }

    #endregion
}
