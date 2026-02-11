using UnityEngine;
using UnityEditor;
using FarmGame.Data.Core;
using System.Linq;

/// <summary>
/// PrefabDatabase 自动扫描器
/// 监听预制体文件变化，自动触发扫描
/// </summary>
public class PrefabDatabaseAutoScanner : AssetPostprocessor
{
    /// <summary>
    /// PrefabDatabase 资产路径
    /// </summary>
    private const string DatabasePath = "Assets/111_Data/Database/PrefabDatabase.asset";
    
    /// <summary>
    /// 监控的文件夹
    /// </summary>
    private static readonly string[] WatchedFolders = new[]
    {
        "Assets/222_Prefabs/Tree",
        "Assets/222_Prefabs/Rock",
        "Assets/222_Prefabs/Box",
        "Assets/222_Prefabs/WorldItems"
    };
    
    /// <summary>
    /// 是否启用自动扫描
    /// </summary>
    private static bool _autoScanEnabled = true;
    
    /// <summary>
    /// 资产变化回调
    /// </summary>
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        if (!_autoScanEnabled) return;
        
        // 检查是否有预制体变化
        bool prefabChanged = CheckPrefabChanged(importedAssets) ||
                            CheckPrefabChanged(deletedAssets) ||
                            CheckPrefabChanged(movedAssets) ||
                            CheckPrefabChanged(movedFromAssetPaths);
        
        if (prefabChanged)
        {
            // 延迟扫描，避免在资产导入过程中修改
            EditorApplication.delayCall += TriggerScan;
        }
    }
    
    /// <summary>
    /// 检查是否有预制体变化
    /// </summary>
    private static bool CheckPrefabChanged(string[] assetPaths)
    {
        if (assetPaths == null || assetPaths.Length == 0) return false;
        
        foreach (var path in assetPaths)
        {
            // 检查是否为预制体
            if (!path.EndsWith(".prefab")) continue;
            
            // 检查是否在监控的文件夹内
            foreach (var folder in WatchedFolders)
            {
                if (path.StartsWith(folder))
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 触发扫描
    /// </summary>
    private static void TriggerScan()
    {
        var database = AssetDatabase.LoadAssetAtPath<PrefabDatabase>(DatabasePath);
        
        if (database == null)
        {
            // 数据库不存在，不自动创建
            return;
        }
        
        database.ScanPrefabs();
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        
        Debug.Log("[PrefabDatabaseAutoScanner] 检测到预制体变化，已自动扫描");
    }
    
    /// <summary>
    /// 菜单：启用/禁用自动扫描
    /// </summary>
    [MenuItem("FarmGame/PrefabDatabase/启用自动扫描", true)]
    private static bool ValidateEnableAutoScan()
    {
        Menu.SetChecked("FarmGame/PrefabDatabase/启用自动扫描", _autoScanEnabled);
        return true;
    }
    
    [MenuItem("FarmGame/PrefabDatabase/启用自动扫描")]
    private static void ToggleAutoScan()
    {
        _autoScanEnabled = !_autoScanEnabled;
        Debug.Log($"[PrefabDatabaseAutoScanner] 自动扫描已{(_autoScanEnabled ? "启用" : "禁用")}");
    }
    
    /// <summary>
    /// 菜单：手动扫描
    /// </summary>
    [MenuItem("FarmGame/PrefabDatabase/手动扫描")]
    private static void ManualScan()
    {
        var database = AssetDatabase.LoadAssetAtPath<PrefabDatabase>(DatabasePath);
        
        if (database == null)
        {
            // 创建数据库
            database = ScriptableObject.CreateInstance<PrefabDatabase>();
            
            // 确保目录存在
            string directory = System.IO.Path.GetDirectoryName(DatabasePath);
            if (!AssetDatabase.IsValidFolder(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
                AssetDatabase.Refresh();
            }
            
            AssetDatabase.CreateAsset(database, DatabasePath);
            Debug.Log($"[PrefabDatabaseAutoScanner] 已创建 PrefabDatabase: {DatabasePath}");
        }
        
        database.ScanPrefabs();
        database.AddDefaultAliases();
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        
        // 选中数据库
        Selection.activeObject = database;
        EditorGUIUtility.PingObject(database);
        
        Debug.Log($"[PrefabDatabaseAutoScanner] 手动扫描完成，共 {database.EntryCount} 个预制体");
    }
    
    /// <summary>
    /// 菜单：创建 PrefabDatabase
    /// </summary>
    [MenuItem("FarmGame/PrefabDatabase/创建 PrefabDatabase")]
    private static void CreateDatabase()
    {
        if (AssetDatabase.LoadAssetAtPath<PrefabDatabase>(DatabasePath) != null)
        {
            Debug.LogWarning($"[PrefabDatabaseAutoScanner] PrefabDatabase 已存在: {DatabasePath}");
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<PrefabDatabase>(DatabasePath);
            return;
        }
        
        var database = ScriptableObject.CreateInstance<PrefabDatabase>();
        
        // 确保目录存在
        string directory = System.IO.Path.GetDirectoryName(DatabasePath);
        if (!AssetDatabase.IsValidFolder(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
            AssetDatabase.Refresh();
        }
        
        AssetDatabase.CreateAsset(database, DatabasePath);
        
        // 扫描并添加默认别名
        database.ScanPrefabs();
        database.AddDefaultAliases();
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        
        Selection.activeObject = database;
        EditorGUIUtility.PingObject(database);
        
        Debug.Log($"[PrefabDatabaseAutoScanner] 已创建 PrefabDatabase: {DatabasePath}");
    }
}
