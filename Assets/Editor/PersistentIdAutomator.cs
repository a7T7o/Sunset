using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using FarmGame.Data.Core;

/// <summary>
/// 持久化 ID 自动化守门员
/// 
/// 功能：
/// - 监听场景保存事件，自动修复缺失的 GUID
/// - 检测并修复重复的 GUID（Ctrl+D 复制导致）
/// - 支持所有 IPersistentObject 实现（TreeController, StoneController, ChestController 等）
/// 
/// 设计原则：
/// - 无感：只要按 Ctrl+S 保存场景，自动工作
/// - 零操作：不需要手动菜单或配置
/// - 安全：只在 Editor 模式下运行，不影响运行时
/// </summary>
[InitializeOnLoad]
public static class PersistentIdAutomator
{
    // 支持的字段名（不同组件可能使用不同命名）
    private static readonly string[] PersistentIdFieldNames = { "persistentId", "_persistentId" };
    
    static PersistentIdAutomator()
    {
        EditorSceneManager.sceneSaving += OnSceneSaving;
    }
    
    /// <summary>
    /// 场景保存时的回调
    /// </summary>
    private static void OnSceneSaving(Scene scene, string path)
    {
        // 运行时不处理
        if (Application.isPlaying) return;
        
        // 扫描并修复 GUID
        var result = ScanAndFixGuids(scene);
        
        // 输出日志（仅当有修复时）
        if (result.EmptyFixed > 0 || result.DuplicatesFixed > 0)
        {
            Debug.Log($"<color=green>[PersistentIdAutomator]</color> 场景 '{scene.name}' 已修复 {result.EmptyFixed} 个空 GUID，{result.DuplicatesFixed} 个重复 GUID");
        }
    }
    
    /// <summary>
    /// 扫描并修复场景中的 GUID 问题
    /// </summary>
    private static ScanResult ScanAndFixGuids(Scene scene)
    {
        var result = new ScanResult();
        var seenGuids = new Dictionary<string, MonoBehaviour>(); // GUID -> 第一个拥有者
        
        // 🔥 关键：只扫描传入的 scene，不扫描其他打开的场景
        var rootObjects = scene.GetRootGameObjects();
        
        foreach (var root in rootObjects)
        {
            // 递归查找所有 IPersistentObject 组件
            var persistentObjects = root.GetComponentsInChildren<MonoBehaviour>(true);
            
            foreach (var obj in persistentObjects)
            {
                // 检查是否实现 IPersistentObject
                if (!(obj is IPersistentObject)) continue;
                
                result.TotalScanned++;
                
                // 尝试获取并修复 GUID
                ProcessPersistentObject(obj, seenGuids, result);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// 处理单个持久化对象
    /// </summary>
    private static void ProcessPersistentObject(MonoBehaviour obj, Dictionary<string, MonoBehaviour> seenGuids, ScanResult result)
    {
        SerializedObject so = new SerializedObject(obj);
        SerializedProperty guidProp = FindGuidProperty(so);
        
        if (guidProp == null)
        {
            // 找不到 GUID 字段，记录警告
            result.Warnings.Add($"无法找到 GUID 字段: {GetObjectPath(obj)}");
            return;
        }
        
        string currentGuid = guidProp.stringValue;
        
        // 情况1：GUID 为空，需要生成
        if (string.IsNullOrEmpty(currentGuid))
        {
            string newGuid = System.Guid.NewGuid().ToString();
            guidProp.stringValue = newGuid;
            so.ApplyModifiedPropertiesWithoutUndo();
            
            seenGuids[newGuid] = obj;
            result.EmptyFixed++;
            return;
        }
        
        // 情况2：GUID 重复（Ctrl+D 复制导致）
        if (seenGuids.TryGetValue(currentGuid, out var existingObj))
        {
            // 保留第一个，为当前对象生成新 GUID
            string newGuid = System.Guid.NewGuid().ToString();
            guidProp.stringValue = newGuid;
            so.ApplyModifiedPropertiesWithoutUndo();
            
            seenGuids[newGuid] = obj;
            result.DuplicatesFixed++;
            
            Debug.LogWarning($"<color=yellow>[PersistentIdAutomator]</color> 检测到重复 GUID，已修复: {GetObjectPath(obj)} (原 GUID 属于 {GetObjectPath(existingObj)})");
            return;
        }
        
        // 情况3：GUID 正常，记录到已见列表
        seenGuids[currentGuid] = obj;
    }
    
    /// <summary>
    /// 查找 GUID 属性（支持多种字段名）
    /// </summary>
    private static SerializedProperty FindGuidProperty(SerializedObject so)
    {
        foreach (var fieldName in PersistentIdFieldNames)
        {
            var prop = so.FindProperty(fieldName);
            if (prop != null && prop.propertyType == SerializedPropertyType.String)
            {
                return prop;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 获取对象的层级路径（用于日志）
    /// </summary>
    private static string GetObjectPath(MonoBehaviour obj)
    {
        if (obj == null) return "(null)";
        
        var path = obj.gameObject.name;
        var parent = obj.transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return $"{path} ({obj.GetType().Name})";
    }
    
    /// <summary>
    /// 扫描结果
    /// </summary>
    private class ScanResult
    {
        public int TotalScanned = 0;
        public int EmptyFixed = 0;
        public int DuplicatesFixed = 0;
        public List<string> Warnings = new List<string>();
    }
}
