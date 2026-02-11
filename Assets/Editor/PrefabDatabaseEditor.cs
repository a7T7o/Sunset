using UnityEngine;
using UnityEditor;
using FarmGame.Data.Core;
using System.Linq;

/// <summary>
/// PrefabDatabase 自定义编辑器
/// 提供预制体扫描、统计信息、列表显示等功能
/// </summary>
[CustomEditor(typeof(PrefabDatabase))]
public class PrefabDatabaseEditor : Editor
{
    private bool _showPrefabList = true;
    private bool _showAliasList = true;
    private Vector2 _scrollPosition;
    private string _searchFilter = "";
    
    // 分组折叠状态
    private System.Collections.Generic.Dictionary<string, bool> _folderFoldouts = 
        new System.Collections.Generic.Dictionary<string, bool>();
    
    public override void OnInspectorGUI()
    {
        var database = (PrefabDatabase)target;
        
        serializedObject.Update();
        
        // 1. 绘制文件夹配置
        DrawFolderConfig();
        
        EditorGUILayout.Space(10);
        
        // 2. 扫描按钮
        DrawScanButton(database);
        
        EditorGUILayout.Space(5);
        
        // 3. 统计信息
        DrawStatistics(database);
        
        EditorGUILayout.Space(10);
        
        // 4. 别名列表
        DrawAliasList(database);
        
        EditorGUILayout.Space(10);
        
        // 5. 预制体列表
        DrawPrefabList(database);
        
        // 6. 调试选项
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showDebugInfo"));
        
        serializedObject.ApplyModifiedProperties();
    }
    
    /// <summary>
    /// 绘制文件夹配置
    /// </summary>
    private void DrawFolderConfig()
    {
        EditorGUILayout.LabelField("预制体文件夹配置", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("prefabFolders"), true);
    }
    
    /// <summary>
    /// 绘制扫描按钮
    /// </summary>
    private void DrawScanButton(PrefabDatabase database)
    {
        EditorGUILayout.BeginHorizontal();
        
        GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
        if (GUILayout.Button("🔍 扫描预制体", GUILayout.Height(30)))
        {
            database.ScanPrefabs();
            EditorUtility.SetDirty(database);
        }
        GUI.backgroundColor = Color.white;
        
        GUI.backgroundColor = new Color(0.8f, 0.4f, 0.4f);
        if (GUILayout.Button("🗑️ 清空", GUILayout.Height(30), GUILayout.Width(60)))
        {
            if (EditorUtility.DisplayDialog("确认清空", "确定要清空所有预制体条目吗？", "确定", "取消"))
            {
                database.ClearEntries();
                EditorUtility.SetDirty(database);
            }
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndHorizontal();
    }
    
    /// <summary>
    /// 绘制统计信息
    /// </summary>
    private void DrawStatistics(PrefabDatabase database)
    {
        var entries = database.Entries;
        var aliases = database.Aliases;
        
        // 按文件夹分组统计
        var folderGroups = entries.GroupBy(e => e.folderPath).ToList();
        
        string statsText = $"已注册 {database.EntryCount} 个预制体";
        if (folderGroups.Count > 0)
        {
            statsText += $"（{folderGroups.Count} 个文件夹）";
        }
        if (aliases.Count > 0)
        {
            statsText += $"\n已配置 {aliases.Count} 个 ID 别名映射";
        }
        
        EditorGUILayout.HelpBox(statsText, MessageType.Info);
    }
    
    /// <summary>
    /// 绘制别名列表
    /// </summary>
    private void DrawAliasList(PrefabDatabase database)
    {
        _showAliasList = EditorGUILayout.Foldout(_showAliasList, $"ID 别名映射 ({database.Aliases.Count})", true);
        
        if (!_showAliasList) return;
        
        EditorGUI.indentLevel++;
        
        // 添加默认别名按钮
        if (database.Aliases.Count == 0)
        {
            if (GUILayout.Button("添加默认别名", GUILayout.Height(25)))
            {
                database.AddDefaultAliases();
                EditorUtility.SetDirty(database);
            }
        }
        
        // 显示别名列表
        EditorGUILayout.PropertyField(serializedObject.FindProperty("aliases"), true);
        
        EditorGUI.indentLevel--;
    }
    
    /// <summary>
    /// 绘制预制体列表
    /// </summary>
    private void DrawPrefabList(PrefabDatabase database)
    {
        _showPrefabList = EditorGUILayout.Foldout(_showPrefabList, $"预制体列表 ({database.EntryCount})", true);
        
        if (!_showPrefabList) return;
        
        EditorGUI.indentLevel++;
        
        // 搜索框
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("搜索:", GUILayout.Width(40));
        _searchFilter = EditorGUILayout.TextField(_searchFilter);
        if (GUILayout.Button("✕", GUILayout.Width(20)))
        {
            _searchFilter = "";
            GUI.FocusControl(null);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        
        // 按文件夹分组显示
        var entries = database.Entries;
        var filteredEntries = string.IsNullOrEmpty(_searchFilter) 
            ? entries 
            : entries.Where(e => e.name.ToLower().Contains(_searchFilter.ToLower())).ToList();
        
        var folderGroups = filteredEntries.GroupBy(e => e.folderPath).OrderBy(g => g.Key);
        
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.MaxHeight(300));
        
        foreach (var group in folderGroups)
        {
            string folderName = group.Key;
            if (!_folderFoldouts.ContainsKey(folderName))
                _folderFoldouts[folderName] = true;
            
            _folderFoldouts[folderName] = EditorGUILayout.Foldout(
                _folderFoldouts[folderName], 
                $"📁 {folderName} ({group.Count()})", 
                true
            );
            
            if (_folderFoldouts[folderName])
            {
                EditorGUI.indentLevel++;
                foreach (var entry in group.OrderBy(e => e.name))
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    // 预制体名称
                    EditorGUILayout.LabelField(entry.name, GUILayout.Width(200));
                    
                    // 预制体引用（只读）
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(entry.prefab, typeof(GameObject), false);
                    EditorGUI.EndDisabledGroup();
                    
                    // 选择按钮
                    if (GUILayout.Button("选择", GUILayout.Width(50)))
                    {
                        Selection.activeObject = entry.prefab;
                        EditorGUIUtility.PingObject(entry.prefab);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUI.indentLevel--;
    }
}
