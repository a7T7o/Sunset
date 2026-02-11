using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FarmGame.Data.Core
{
    /// <summary>
    /// 预制体数据库
    /// 自动扫描指定文件夹下的所有预制体，支持智能查找和 ID 别名映射
    /// 
    /// 核心功能：
    /// 1. 自动扫描：配置文件夹路径，自动扫描所有预制体
    /// 2. 智能查找：精确匹配 → 清洗名称 → 别名映射 → 前缀回退
    /// 3. ID 别名映射：支持旧存档 ID 到新预制体名称的映射
    /// 4. 向后兼容：旧存档中的 prefabId 能通过回退机制找到对应预制体
    /// 
    /// 使用方式：
    /// 1. 在 Assets/111_Data/Database/ 下创建 PrefabDatabase.asset
    /// 2. 配置预制体文件夹路径
    /// 3. 点击"扫描预制体"按钮
    /// 4. 配置 ID 别名映射（可选，用于旧存档兼容）
    /// </summary>
    [CreateAssetMenu(fileName = "PrefabDatabase", menuName = "FarmGame/Data/PrefabDatabase")]
    public class PrefabDatabase : ScriptableObject
    {
        #region 内部类
        
        /// <summary>
        /// 预制体条目
        /// </summary>
        [Serializable]
        public class PrefabEntry
        {
            [Tooltip("预制体名称（作为 ID）")]
            public string name;
            
            [Tooltip("预制体引用")]
            public GameObject prefab;
            
            [Tooltip("来源文件夹（用于分组显示）")]
            public string folderPath;
        }
        
        /// <summary>
        /// ID 别名条目（旧存档兼容）
        /// </summary>
        [Serializable]
        public class AliasEntry
        {
            [Tooltip("旧存档中的 ID")]
            public string oldId;
            
            [Tooltip("新系统中的预制体名称")]
            public string newPrefabName;
            
            [Tooltip("备注说明")]
            public string note;
        }
        
        #endregion
        
        #region 序列化字段
        
        [Header("预制体文件夹")]
        [Tooltip("自动扫描这些文件夹下的所有预制体")]
        [SerializeField] private string[] prefabFolders = new string[]
        {
            "Assets/222_Prefabs/Tree",
            "Assets/222_Prefabs/Rock",
            "Assets/222_Prefabs/Box",
            "Assets/222_Prefabs/WorldItems"
        };
        
        [Header("运行时数据")]
        [SerializeField] private List<PrefabEntry> entries = new List<PrefabEntry>();
        
        [Header("ID 别名映射（旧存档兼容）")]
        [Tooltip("旧存档 ID 到新预制体名称的映射")]
        [SerializeField] private List<AliasEntry> aliases = new List<AliasEntry>();
        
        [Header("调试")]
        [SerializeField] private bool showDebugInfo = false;
        
        #endregion
        
        #region 私有字段
        
        /// <summary>
        /// 运行时缓存（name → Prefab）
        /// </summary>
        private Dictionary<string, GameObject> _cache;
        
        /// <summary>
        /// 别名缓存（oldId → newPrefabName）
        /// </summary>
        private Dictionary<string, string> _aliasCache;
        
        #endregion
        
        #region 属性
        
        /// <summary>
        /// 获取已注册预制体数量
        /// </summary>
        public int EntryCount => entries?.Count ?? 0;
        
        /// <summary>
        /// 获取预制体文件夹配置
        /// </summary>
        public string[] PrefabFolders => prefabFolders;
        
        /// <summary>
        /// 获取所有条目（只读）
        /// </summary>
        public IReadOnlyList<PrefabEntry> Entries => entries;
        
        /// <summary>
        /// 获取所有别名（只读）
        /// </summary>
        public IReadOnlyList<AliasEntry> Aliases => aliases;
        
        #endregion

        #region 公开方法
        
        /// <summary>
        /// 根据名称获取预制体（支持智能回退）
        /// </summary>
        /// <param name="prefabName">预制体名称或旧存档 ID</param>
        /// <returns>预制体，找不到返回 null</returns>
        public GameObject GetPrefab(string prefabName)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                if (showDebugInfo)
                    Debug.LogWarning("[PrefabDatabase] GetPrefab: prefabName 为空");
                return null;
            }
            
            // 延迟构建缓存
            EnsureCacheBuilt();
            
            // 1. 解析 ID（支持别名映射和前缀回退）
            string resolvedId = ResolvePrefabId(prefabName);
            
            // 2. 精确匹配
            if (_cache.TryGetValue(resolvedId, out var prefab))
            {
                return prefab;
            }
            
            // 3. 清洗名称后匹配
            string cleanName = CleanPrefabName(resolvedId);
            if (cleanName != resolvedId && _cache.TryGetValue(cleanName, out prefab))
            {
                if (showDebugInfo)
                    Debug.Log($"[PrefabDatabase] 清洗名称匹配: {prefabName} → {cleanName}");
                return prefab;
            }
            
            // 4. 所有尝试都失败
            if (showDebugInfo)
                Debug.LogWarning($"[PrefabDatabase] 找不到预制体: {prefabName} (resolved: {resolvedId})");
            
            return null;
        }
        
        /// <summary>
        /// 解析预制体 ID（支持别名映射和前缀回退）
        /// </summary>
        /// <param name="saveId">存档中的 ID</param>
        /// <returns>解析后的预制体名称</returns>
        public string ResolvePrefabId(string saveId)
        {
            if (string.IsNullOrEmpty(saveId)) return saveId;
            
            EnsureCacheBuilt();
            
            // 1. 如果数据库里直接有，直接返回
            if (_cache.ContainsKey(saveId))
            {
                return saveId;
            }
            
            // 2. 查找别名映射
            if (_aliasCache != null && _aliasCache.TryGetValue(saveId, out var newName))
            {
                if (_cache.ContainsKey(newName))
                {
                    if (showDebugInfo)
                        Debug.Log($"[PrefabDatabase] ID 别名映射: {saveId} → {newName}");
                    return newName;
                }
            }
            
            // 3. 前缀匹配回退（通用规则）
            string fallback = TryPrefixFallback(saveId);
            if (fallback != null && _cache.ContainsKey(fallback))
            {
                Debug.LogWarning($"[PrefabDatabase] 前缀回退: {saveId} → {fallback}");
                return fallback;
            }
            
            // 4. 返回原始 ID（让后续逻辑处理失败情况）
            return saveId;
        }
        
        /// <summary>
        /// 检查预制体是否存在
        /// </summary>
        public bool HasPrefab(string prefabName)
        {
            if (string.IsNullOrEmpty(prefabName)) return false;
            
            EnsureCacheBuilt();
            
            // 直接检查
            if (_cache.ContainsKey(prefabName)) return true;
            
            // 检查清洗后的名称
            string cleanName = CleanPrefabName(prefabName);
            if (cleanName != prefabName && _cache.ContainsKey(cleanName)) return true;
            
            // 检查别名
            if (_aliasCache != null && _aliasCache.TryGetValue(prefabName, out var newName))
            {
                if (_cache.ContainsKey(newName)) return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// 获取所有已注册的预制体名称
        /// </summary>
        public IEnumerable<string> GetAllPrefabNames()
        {
            EnsureCacheBuilt();
            return _cache.Keys;
        }
        
        #endregion
        
        #region 私有方法
        
        /// <summary>
        /// 确保缓存已构建
        /// </summary>
        private void EnsureCacheBuilt()
        {
            if (_cache == null)
            {
                BuildCache();
            }
            if (_aliasCache == null)
            {
                BuildAliasCache();
            }
        }
        
        /// <summary>
        /// 构建运行时缓存
        /// </summary>
        private void BuildCache()
        {
            _cache = new Dictionary<string, GameObject>();
            
            if (entries == null) return;
            
            foreach (var entry in entries)
            {
                if (string.IsNullOrEmpty(entry.name))
                {
                    Debug.LogWarning("[PrefabDatabase] 发现空的预制体名称，已跳过");
                    continue;
                }
                
                if (entry.prefab == null)
                {
                    Debug.LogWarning($"[PrefabDatabase] 预制体 '{entry.name}' 的引用为空，已跳过");
                    continue;
                }
                
                if (_cache.ContainsKey(entry.name))
                {
                    Debug.LogWarning($"[PrefabDatabase] 预制体名称 '{entry.name}' 重复，使用第一个");
                    continue;
                }
                
                _cache[entry.name] = entry.prefab;
            }
            
            if (showDebugInfo)
                Debug.Log($"[PrefabDatabase] 缓存构建完成，共 {_cache.Count} 个预制体");
        }
        
        /// <summary>
        /// 构建别名缓存
        /// </summary>
        private void BuildAliasCache()
        {
            _aliasCache = new Dictionary<string, string>();
            
            if (aliases == null) return;
            
            foreach (var alias in aliases)
            {
                if (string.IsNullOrEmpty(alias.oldId) || string.IsNullOrEmpty(alias.newPrefabName))
                {
                    continue;
                }
                
                if (_aliasCache.ContainsKey(alias.oldId))
                {
                    Debug.LogWarning($"[PrefabDatabase] 别名 '{alias.oldId}' 重复，使用第一个");
                    continue;
                }
                
                _aliasCache[alias.oldId] = alias.newPrefabName;
            }
            
            if (showDebugInfo)
                Debug.Log($"[PrefabDatabase] 别名缓存构建完成，共 {_aliasCache.Count} 个映射");
        }
        
        /// <summary>
        /// 清洗预制体名称（去掉 Clone、数字后缀等）
        /// </summary>
        private string CleanPrefabName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            
            // 去掉 "(Clone)" 后缀
            if (name.EndsWith("(Clone)"))
                name = name.Substring(0, name.Length - 7).Trim();
            
            // 去掉 " (1)", " (2)" 等后缀
            name = Regex.Replace(name, @"\s\(\d+\)$", "");
            
            return name;
        }
        
        /// <summary>
        /// 前缀匹配回退（通用规则）
        /// </summary>
        private string TryPrefixFallback(string saveId)
        {
            if (string.IsNullOrEmpty(saveId)) return null;
            
            // Storage_ 开头 → 尝试 Box_1
            if (saveId.StartsWith("Storage_"))
            {
                return "Box_1";
            }
            
            // Stone_ 开头 → 尝试 C1
            if (saveId.StartsWith("Stone_"))
            {
                return "C1";
            }
            
            // 其他规则可以在这里添加
            return null;
        }
        
        #endregion

        #region 编辑器方法
        
#if UNITY_EDITOR
        /// <summary>
        /// 扫描所有配置的文件夹
        /// </summary>
        public void ScanPrefabs()
        {
            entries.Clear();
            
            if (prefabFolders == null || prefabFolders.Length == 0)
            {
                Debug.LogWarning("[PrefabDatabase] 没有配置预制体文件夹");
                return;
            }
            
            foreach (var folder in prefabFolders)
            {
                if (string.IsNullOrEmpty(folder)) continue;
                ScanFolder(folder);
            }
            
            // 去重
            RemoveDuplicates();
            
            // 清空缓存，下次访问时重建
            _cache = null;
            _aliasCache = null;
            
            Debug.Log($"[PrefabDatabase] 扫描完成，共 {entries.Count} 个预制体");
        }
        
        /// <summary>
        /// 扫描单个文件夹
        /// </summary>
        private void ScanFolder(string folderPath)
        {
            if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogWarning($"[PrefabDatabase] 文件夹不存在: {folderPath}");
                return;
            }
            
            // 查找所有预制体
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
            
            foreach (var guid in guids)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                
                if (prefab == null) continue;
                
                // 获取相对文件夹路径
                string relativePath = System.IO.Path.GetDirectoryName(assetPath);
                if (relativePath.StartsWith(folderPath))
                {
                    relativePath = relativePath.Substring(folderPath.Length).TrimStart('/', '\\');
                }
                
                entries.Add(new PrefabEntry
                {
                    name = prefab.name,
                    prefab = prefab,
                    folderPath = string.IsNullOrEmpty(relativePath) ? folderPath : $"{folderPath}/{relativePath}"
                });
            }
        }
        
        /// <summary>
        /// 去除重复条目
        /// </summary>
        private void RemoveDuplicates()
        {
            var seen = new HashSet<string>();
            var uniqueEntries = new List<PrefabEntry>();
            
            foreach (var entry in entries)
            {
                if (string.IsNullOrEmpty(entry.name)) continue;
                
                if (seen.Contains(entry.name))
                {
                    Debug.LogWarning($"[PrefabDatabase] 发现重复预制体名称: {entry.name}，保留第一个");
                    continue;
                }
                
                seen.Add(entry.name);
                uniqueEntries.Add(entry);
            }
            
            entries = uniqueEntries;
        }
        
        /// <summary>
        /// 清空所有条目
        /// </summary>
        public void ClearEntries()
        {
            entries.Clear();
            _cache = null;
            _aliasCache = null;
        }
        
        /// <summary>
        /// 添加默认别名（首次创建时调用）
        /// </summary>
        public void AddDefaultAliases()
        {
            if (aliases == null)
                aliases = new List<AliasEntry>();
            
            // 检查是否已有别名
            if (aliases.Count > 0) return;
            
            // 添加默认别名
            aliases.Add(new AliasEntry
            {
                oldId = "Storage_1400_小木箱子_0",
                newPrefabName = "Box_1",
                note = "小木箱子 StorageData 名称 → 世界预制体"
            });
            
            aliases.Add(new AliasEntry
            {
                oldId = "Storage_1401_大木箱子_0",
                newPrefabName = "Box_2",
                note = "大木箱子 StorageData 名称 → 世界预制体"
            });
            
            aliases.Add(new AliasEntry
            {
                oldId = "Storage_1402_小铁箱子_0",
                newPrefabName = "Box_3",
                note = "小铁箱子 StorageData 名称 → 世界预制体"
            });
            
            aliases.Add(new AliasEntry
            {
                oldId = "Storage_1403_大铁箱子_0",
                newPrefabName = "Box_4",
                note = "大铁箱子 StorageData 名称 → 世界预制体"
            });
            
            aliases.Add(new AliasEntry
            {
                oldId = "Chest",
                newPrefabName = "Box_1",
                note = "默认箱子回退"
            });
            
            Debug.Log($"[PrefabDatabase] 已添加 {aliases.Count} 个默认别名");
        }
        
        /// <summary>
        /// 首次创建时初始化
        /// </summary>
        private void Reset()
        {
            // 设置默认文件夹
            prefabFolders = new string[]
            {
                "Assets/222_Prefabs/Tree",
                "Assets/222_Prefabs/Rock",
                "Assets/222_Prefabs/Box",
                "Assets/222_Prefabs/WorldItems"
            };
            
            // 添加默认别名
            AddDefaultAliases();
        }
#endif
        
        #endregion
        
        #region Unity 生命周期
        
        private void OnEnable()
        {
            // 每次启用时清空缓存，确保编辑器修改后能重新加载
            _cache = null;
            _aliasCache = null;
        }
        
        #endregion
    }
}
