using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using FarmGame.Data.Core;

/// <summary>
/// 持久化 ID 验证Tools
/// 
/// 提供手动验证和修复功能，作为自动化守门员的补充。
/// </summary>
public static class PersistentIdValidator
{
    // 支持的字段名
    private static readonly string[] PersistentIdFieldNames = { "persistentId", "_persistentId" };
    
    #region 菜单命令
    
    [MenuItem("Tools/持久化系统/验证当前场景 GUID", false, 100)]
    public static void ValidateCurrentScene()
    {
        var scene = SceneManager.GetActiveScene();
        var report = GenerateReport(scene);
        ShowReport(report);
    }
    
    [MenuItem("Tools/持久化系统/修复当前场景 GUID", false, 101)]
    public static void FixCurrentScene()
    {
        var scene = SceneManager.GetActiveScene();
        var result = FixScene(scene);
        
        if (result.EmptyFixed > 0 || result.DuplicatesFixed > 0)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorUtility.DisplayDialog(
                "修复完成",
                $"已修复 {result.EmptyFixed} 个空 GUID，{result.DuplicatesFixed} 个重复 GUID。\n\n请记得保存场景 (Ctrl+S)！",
                "确定"
            );
        }
        else
        {
            EditorUtility.DisplayDialog(
                "验证通过",
                "当前场景所有 GUID 均正常，无需修复。",
                "确定"
            );
        }
    }
    
    [MenuItem("Tools/持久化系统/验证所有场景 GUID", false, 200)]
    public static void ValidateAllScenes()
    {
        var scenePaths = GetAllScenePaths();
        var allIssues = new List<string>();
        int totalEmpty = 0;
        int totalDuplicates = 0;
        
        foreach (var path in scenePaths)
        {
            // 打开场景（不保存当前场景的修改）
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            var report = GenerateReport(scene);
            
            if (report.EmptyGuids > 0 || report.DuplicateGuids > 0)
            {
                allIssues.Add($"• {scene.name}: {report.EmptyGuids} 空, {report.DuplicateGuids} 重复");
                totalEmpty += report.EmptyGuids;
                totalDuplicates += report.DuplicateGuids;
            }
        }
        
        if (allIssues.Count > 0)
        {
            EditorUtility.DisplayDialog(
                "验证结果",
                $"发现问题的场景：\n\n{string.Join("\n", allIssues)}\n\n总计：{totalEmpty} 空 GUID，{totalDuplicates} 重复 GUID\n\n请逐个打开并保存这些场景以自动修复。",
                "确定"
            );
        }
        else
        {
            EditorUtility.DisplayDialog(
                "验证通过",
                $"已检查 {scenePaths.Length} 个场景，所有 GUID 均正常。",
                "确定"
            );
        }
    }
    
    #endregion
    
    #region 核心逻辑
    
    /// <summary>
    /// 生成验证报告
    /// </summary>
    private static ValidationReport GenerateReport(Scene scene)
    {
        var report = new ValidationReport
        {
            SceneName = scene.name,
            Issues = new List<ObjectIssue>()
        };
        
        var seenGuids = new Dictionary<string, MonoBehaviour>();
        var rootObjects = scene.GetRootGameObjects();
        
        foreach (var root in rootObjects)
        {
            var persistentObjects = root.GetComponentsInChildren<MonoBehaviour>(true);
            
            foreach (var obj in persistentObjects)
            {
                if (!(obj is IPersistentObject)) continue;
                
                report.TotalObjects++;
                
                SerializedObject so = new SerializedObject(obj);
                SerializedProperty guidProp = FindGuidProperty(so);
                
                if (guidProp == null) continue;
                
                string currentGuid = guidProp.stringValue;
                
                if (string.IsNullOrEmpty(currentGuid))
                {
                    report.EmptyGuids++;
                    report.Issues.Add(new ObjectIssue
                    {
                        ObjectPath = GetObjectPath(obj),
                        ComponentType = obj.GetType().Name,
                        Issue = IssueType.Empty
                    });
                }
                else if (seenGuids.ContainsKey(currentGuid))
                {
                    report.DuplicateGuids++;
                    report.Issues.Add(new ObjectIssue
                    {
                        ObjectPath = GetObjectPath(obj),
                        ComponentType = obj.GetType().Name,
                        CurrentGuid = currentGuid,
                        Issue = IssueType.Duplicate
                    });
                }
                else
                {
                    seenGuids[currentGuid] = obj;
                }
            }
        }
        
        return report;
    }
    
    /// <summary>
    /// 修复场景中的 GUID 问题
    /// </summary>
    private static FixResult FixScene(Scene scene)
    {
        var result = new FixResult();
        var seenGuids = new Dictionary<string, MonoBehaviour>();
        var rootObjects = scene.GetRootGameObjects();
        
        foreach (var root in rootObjects)
        {
            var persistentObjects = root.GetComponentsInChildren<MonoBehaviour>(true);
            
            foreach (var obj in persistentObjects)
            {
                if (!(obj is IPersistentObject)) continue;
                
                SerializedObject so = new SerializedObject(obj);
                SerializedProperty guidProp = FindGuidProperty(so);
                
                if (guidProp == null) continue;
                
                string currentGuid = guidProp.stringValue;
                
                if (string.IsNullOrEmpty(currentGuid))
                {
                    string newGuid = System.Guid.NewGuid().ToString();
                    guidProp.stringValue = newGuid;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    seenGuids[newGuid] = obj;
                    result.EmptyFixed++;
                }
                else if (seenGuids.ContainsKey(currentGuid))
                {
                    string newGuid = System.Guid.NewGuid().ToString();
                    guidProp.stringValue = newGuid;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    seenGuids[newGuid] = obj;
                    result.DuplicatesFixed++;
                }
                else
                {
                    seenGuids[currentGuid] = obj;
                }
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// 显示验证报告
    /// </summary>
    private static void ShowReport(ValidationReport report)
    {
        if (report.EmptyGuids == 0 && report.DuplicateGuids == 0)
        {
            EditorUtility.DisplayDialog(
                "验证通过",
                $"场景 '{report.SceneName}' 共有 {report.TotalObjects} 个持久化对象，所有 GUID 均正常。",
                "确定"
            );
        }
        else
        {
            var issueDetails = string.Join("\n", report.Issues.Take(10).Select(i => 
                $"• [{i.Issue}] {i.ObjectPath}"
            ));
            
            if (report.Issues.Count > 10)
            {
                issueDetails += $"\n... 还有 {report.Issues.Count - 10} 个问题";
            }
            
            EditorUtility.DisplayDialog(
                "发现问题",
                $"场景 '{report.SceneName}' 共有 {report.TotalObjects} 个持久化对象\n\n" +
                $"空 GUID: {report.EmptyGuids}\n" +
                $"重复 GUID: {report.DuplicateGuids}\n\n" +
                $"问题对象：\n{issueDetails}\n\n" +
                $"使用 '修复当前场景 GUID' 命令进行修复。",
                "确定"
            );
        }
    }
    
    /// <summary>
    /// 获取 Build Settings 中的所有场景路径
    /// </summary>
    private static string[] GetAllScenePaths()
    {
        return EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .Where(p => !string.IsNullOrEmpty(p))
            .ToArray();
    }
    
    #endregion
    
    #region 辅助方法
    
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
        
        return path;
    }
    
    #endregion
    
    #region 数据结构
    
    private class ValidationReport
    {
        public string SceneName;
        public int TotalObjects;
        public int EmptyGuids;
        public int DuplicateGuids;
        public List<ObjectIssue> Issues;
    }
    
    private class ObjectIssue
    {
        public string ObjectPath;
        public string ComponentType;
        public string CurrentGuid;
        public IssueType Issue;
    }
    
    private enum IssueType
    {
        Empty,
        Duplicate
    }
    
    private class FixResult
    {
        public int EmptyFixed;
        public int DuplicatesFixed;
    }
    
    #endregion
}
