using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmGame.Data.Core
{
    /// <summary>
    /// 预制体注册表（已废弃）
    /// 用于存档系统根据 prefabId 查找预制体进行动态重建
    /// 
    /// ⚠️ 已废弃：请使用 PrefabDatabase 替代
    /// PrefabDatabase 支持：
    /// - 自动扫描预制体文件夹
    /// - 智能查找（精确匹配 → 清洗名称 → 别名映射 → 前缀回退）
    /// - ID 别名映射（旧存档兼容）
    /// 
    /// 迁移指南：
    /// 1. 创建 PrefabDatabase.asset（菜单：FarmGame/PrefabDatabase/创建 PrefabDatabase）
    /// 2. 配置预制体文件夹路径
    /// 3. 点击"扫描预制体"按钮
    /// 4. 在 PersistentManagers 中配置 PrefabDatabase 引用
    /// 5. 删除旧的 PrefabRegistry.asset
    /// </summary>
    [System.Obsolete("请使用 PrefabDatabase 替代。PrefabDatabase 支持自动扫描、智能回退、ID 别名映射。")]
    [CreateAssetMenu(fileName = "PrefabRegistry", menuName = "FarmGame/Data/PrefabRegistry (已废弃)")]
    public class PrefabRegistry : ScriptableObject
    {
        #region 内部类
        
        /// <summary>
        /// 预制体条目
        /// </summary>
        [Serializable]
        public class PrefabEntry
        {
            [Tooltip("预制体 ID（如 M1, M2, M3）")]
            public string prefabId;
            
            [Tooltip("预制体引用")]
            public GameObject prefab;
        }
        
        #endregion
        
        #region 序列化字段
        
        [Header("预制体映射")]
        [Tooltip("prefabId → Prefab 映射列表")]
        [SerializeField] private List<PrefabEntry> entries = new List<PrefabEntry>();
        
        [Header("调试")]
        [SerializeField] private bool showDebugInfo = false;
        
        #endregion
        
        #region 私有字段
        
        /// <summary>
        /// 运行时缓存（prefabId → Prefab）
        /// </summary>
        private Dictionary<string, GameObject> _cache;
        
        #endregion
        
        #region 公开方法
        
        /// <summary>
        /// 根据 prefabId 获取预制体
        /// </summary>
        /// <param name="prefabId">预制体 ID</param>
        /// <returns>预制体，找不到返回 null</returns>
        public GameObject GetPrefab(string prefabId)
        {
            if (string.IsNullOrEmpty(prefabId))
            {
                if (showDebugInfo)
                    Debug.LogWarning("[PrefabRegistry] GetPrefab: prefabId 为空");
                return null;
            }
            
            // 延迟构建缓存
            if (_cache == null)
            {
                BuildCache();
            }
            
            if (_cache.TryGetValue(prefabId, out var prefab))
            {
                return prefab;
            }
            
            if (showDebugInfo)
                Debug.LogWarning($"[PrefabRegistry] 找不到预制体: {prefabId}");
            
            return null;
        }
        
        /// <summary>
        /// 检查 prefabId 是否已注册
        /// </summary>
        public bool HasPrefab(string prefabId)
        {
            if (string.IsNullOrEmpty(prefabId)) return false;
            
            if (_cache == null)
            {
                BuildCache();
            }
            
            return _cache.ContainsKey(prefabId);
        }
        
        /// <summary>
        /// 获取所有已注册的 prefabId
        /// </summary>
        public IEnumerable<string> GetAllPrefabIds()
        {
            if (_cache == null)
            {
                BuildCache();
            }
            
            return _cache.Keys;
        }
        
        /// <summary>
        /// 强制重建缓存（编辑器修改后调用）
        /// </summary>
        public void RebuildCache()
        {
            _cache = null;
            BuildCache();
        }
        
        #endregion
        
        #region 私有方法
        
        /// <summary>
        /// 构建运行时缓存
        /// </summary>
        private void BuildCache()
        {
            _cache = new Dictionary<string, GameObject>();
            
            if (entries == null) return;
            
            foreach (var entry in entries)
            {
                if (string.IsNullOrEmpty(entry.prefabId))
                {
                    Debug.LogWarning("[PrefabRegistry] 发现空的 prefabId，已跳过");
                    continue;
                }
                
                if (entry.prefab == null)
                {
                    Debug.LogWarning($"[PrefabRegistry] prefabId '{entry.prefabId}' 的预制体为空，已跳过");
                    continue;
                }
                
                if (_cache.ContainsKey(entry.prefabId))
                {
                    Debug.LogWarning($"[PrefabRegistry] prefabId '{entry.prefabId}' 重复，使用第一个");
                    continue;
                }
                
                _cache[entry.prefabId] = entry.prefab;
            }
            
            if (showDebugInfo)
                Debug.Log($"[PrefabRegistry] 缓存构建完成，共 {_cache.Count} 个预制体");
        }
        
        #endregion
        
        #region Unity 生命周期
        
        private void OnEnable()
        {
            // 每次启用时清空缓存，确保编辑器修改后能重新加载
            _cache = null;
        }
        
        #endregion
    }
}
