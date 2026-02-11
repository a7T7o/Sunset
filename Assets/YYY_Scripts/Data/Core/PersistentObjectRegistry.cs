using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmGame.Data.Core
{
    /// <summary>
    /// 持久化对象注册中心
    /// 
    /// 管理场景中所有需要存档的对象。
    /// 使用单例模式，跨场景持久化。
    /// 
    /// 职责：
    /// - 注册/注销持久化对象
    /// - 根据 GUID 查找对象
    /// - 提供遍历接口供 SaveManager 使用
    /// </summary>
    public class PersistentObjectRegistry : MonoBehaviour, IPersistentObjectRegistry
    {
        #region 单例
        
        private static PersistentObjectRegistry _instance;
        private static bool _isQuitting = false;  // 🔥 防止退出时创建新实例
        
        public static PersistentObjectRegistry Instance
        {
            get
            {
                // 🔥 如果正在退出，不要创建新实例
                if (_isQuitting)
                {
                    return _instance;  // 可能为 null，调用者需要处理
                }
                
                if (_instance == null)
                {
                    // 尝试查找现有实例
                    _instance = FindFirstObjectByType<PersistentObjectRegistry>();
                    
                    // 如果没有，创建新实例
                    if (_instance == null)
                    {
                        var go = new GameObject("[PersistentObjectRegistry]");
                        _instance = go.AddComponent<PersistentObjectRegistry>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
        
        #endregion
        
        #region 字段
        
        /// <summary>
        /// 已注册的持久化对象（GUID -> 对象）
        /// </summary>
        private Dictionary<string, IPersistentObject> _registry = new Dictionary<string, IPersistentObject>();
        
        /// <summary>
        /// 按类型分组的对象（用于快速查询）
        /// </summary>
        private Dictionary<string, HashSet<IPersistentObject>> _byType = new Dictionary<string, HashSet<IPersistentObject>>();
        
        [Header("调试")]
        [SerializeField] private bool showDebugInfo = false;
        
        #endregion
        
        #region 属性
        
        /// <summary>
        /// 已注册对象数量
        /// </summary>
        public int Count => _registry.Count;
        
        #endregion
        
        #region Unity 生命周期
        
        private void Awake()
        {
            // 单例检查
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (showDebugInfo)
                Debug.Log("[PersistentObjectRegistry] 初始化完成");
        }
        
        private void OnApplicationQuit()
        {
            // 🔥 标记正在退出，防止在 OnDestroy 期间创建新实例
            _isQuitting = true;
        }
        
        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
        
        #endregion
        
        #region IPersistentObjectRegistry 实现
        
        /// <summary>
        /// 注册持久化对象
        /// </summary>
        public void Register(IPersistentObject obj)
        {
            if (obj == null) return;
            
            string guid = obj.PersistentId;
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogWarning($"[PersistentObjectRegistry] 对象 {obj.ObjectType} 没有有效的 PersistentId");
                return;
            }
            
            // 检查重复注册
            if (_registry.ContainsKey(guid))
            {
                if (_registry[guid] == obj)
                {
                    // 同一对象重复注册，忽略
                    return;
                }
                
                Debug.LogWarning($"[PersistentObjectRegistry] GUID 冲突: {guid}, 类型: {obj.ObjectType}");
                // 覆盖旧对象
            }
            
            _registry[guid] = obj;
            
            // 按类型分组
            string objType = obj.ObjectType;
            if (!_byType.ContainsKey(objType))
            {
                _byType[objType] = new HashSet<IPersistentObject>();
            }
            _byType[objType].Add(obj);
            
            if (showDebugInfo)
                Debug.Log($"[PersistentObjectRegistry] 注册: {objType}, GUID: {guid}, 总数: {_registry.Count}");
        }
        
        /// <summary>
        /// 尝试注册持久化对象（ID 冲突自愈机制）
        /// </summary>
        /// <returns>true 表示注册成功，false 表示 ID 已被其他对象占用</returns>
        public bool TryRegister(IPersistentObject obj)
        {
            if (obj == null) return false;
            
            string guid = obj.PersistentId;
            if (string.IsNullOrEmpty(guid))
            {
                // ID 为空，需要调用者生成新 ID
                return false;
            }
            
            // 检查 ID 是否已被占用
            if (_registry.TryGetValue(guid, out var existing))
            {
                if (existing == obj)
                {
                    // 同一对象重复注册，视为成功
                    return true;
                }
                
                // ID 被其他对象占用，返回 false
                // 调用者应该重新生成 ID 并再次注册
                return false;
            }
            
            // ID 未被占用，执行注册
            _registry[guid] = obj;
            
            // 按类型分组
            string objType = obj.ObjectType;
            if (!_byType.ContainsKey(objType))
            {
                _byType[objType] = new HashSet<IPersistentObject>();
            }
            _byType[objType].Add(obj);
            
            if (showDebugInfo)
                Debug.Log($"[PersistentObjectRegistry] TryRegister 成功: {objType}, GUID: {guid}, 总数: {_registry.Count}");
            
            return true;
        }
        
        /// <summary>
        /// 注销持久化对象
        /// </summary>
        public void Unregister(IPersistentObject obj)
        {
            if (obj == null) return;
            
            string guid = obj.PersistentId;
            if (string.IsNullOrEmpty(guid)) return;
            
            if (_registry.Remove(guid))
            {
                // 从类型分组中移除
                string objType = obj.ObjectType;
                if (_byType.ContainsKey(objType))
                {
                    _byType[objType].Remove(obj);
                }
                
                if (showDebugInfo)
                    Debug.Log($"[PersistentObjectRegistry] 注销: {objType}, GUID: {guid}, 剩余: {_registry.Count}");
            }
        }
        
        /// <summary>
        /// 根据 GUID 查找对象
        /// </summary>
        public IPersistentObject FindByGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return null;
            
            _registry.TryGetValue(guid, out var obj);
            return obj;
        }
        
        /// <summary>
        /// 获取所有持久化对象
        /// </summary>
        public IEnumerable<IPersistentObject> GetAll()
        {
            return _registry.Values;
        }
        
        /// <summary>
        /// 获取指定类型的所有对象
        /// </summary>
        public IEnumerable<T> GetAllOfType<T>() where T : IPersistentObject
        {
            return _registry.Values.OfType<T>();
        }
        
        #endregion
        
        #region 扩展方法
        
        /// <summary>
        /// 获取指定类型标识的所有对象
        /// </summary>
        public IEnumerable<IPersistentObject> GetAllByObjectType(string objectType)
        {
            if (_byType.TryGetValue(objectType, out var set))
            {
                return set;
            }
            return Enumerable.Empty<IPersistentObject>();
        }
        
        /// <summary>
        /// 检查 GUID 是否已注册
        /// </summary>
        public bool IsRegistered(string guid)
        {
            return !string.IsNullOrEmpty(guid) && _registry.ContainsKey(guid);
        }
        
        /// <summary>
        /// 清空所有注册（场景切换时调用）
        /// ⚠️ 警告：在"原地读档"模式下，绝对不要调用此方法！
        /// 原地读档应使用 PruneStaleRecords() 代替
        /// </summary>
        public void Clear()
        {
            _registry.Clear();
            _byType.Clear();
            
            if (showDebugInfo)
                Debug.Log("[PersistentObjectRegistry] 已清空所有注册");
        }
        
        /// <summary>
        /// 清理空引用（已销毁的对象）
        /// 🔥 锐评010 指令：只移除 Value 为 null 的键值对，不清空所有
        /// 用于"原地读档"模式，保留活着的对象引用
        /// </summary>
        public void PruneStaleRecords()
        {
            // 收集所有 Value 为 null 的键（对象已被 Destroy）
            var keysToRemove = _registry
                .Where(kvp => kvp.Value == null || kvp.Value.Equals(null))
                .Select(kvp => kvp.Key)
                .ToList();
            
            // 移除空引用
            foreach (var key in keysToRemove)
            {
                _registry.Remove(key);
            }
            
            // 同时清理 _byType 中的空引用
            foreach (var typeSet in _byType.Values)
            {
                typeSet.RemoveWhere(obj => obj == null || obj.Equals(null));
            }
            
            if (showDebugInfo && keysToRemove.Count > 0)
                Debug.Log($"[PersistentObjectRegistry] PruneStaleRecords: 清理了 {keysToRemove.Count} 个空引用");
        }
        
        /// <summary>
        /// 获取所有需要保存的对象
        /// </summary>
        public IEnumerable<IPersistentObject> GetAllSaveable()
        {
            return _registry.Values.Where(obj => obj.ShouldSave);
        }
        
        /// <summary>
        /// 收集所有对象的存档数据
        /// </summary>
        public List<WorldObjectSaveData> CollectAllSaveData()
        {
            var result = new List<WorldObjectSaveData>();
            
            foreach (var obj in GetAllSaveable())
            {
                try
                {
                    var data = obj.Save();
                    if (data != null)
                    {
                        result.Add(data);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[PersistentObjectRegistry] 保存对象失败: {obj.ObjectType}, GUID: {obj.PersistentId}, 错误: {e.Message}");
                }
            }
            
            if (showDebugInfo)
                Debug.Log($"[PersistentObjectRegistry] 收集存档数据: {result.Count} 个对象");
            
            return result;
        }
        
        /// <summary>
        /// 恢复所有对象的状态（含反向修剪和动态重建）
        /// 🔥 P2-1 修复：实现反向修剪逻辑，防止已删除物体"复活"
        /// 🔥 锐评011 指令：添加 GUID 匹配率统计
        /// 🔥 动态对象重建：找不到 GUID 时尝试重建
        /// 🔥 P0 任务 1.4：清理 StoneDebris 临时碎片效果
        /// </summary>
        public void RestoreAllFromSaveData(List<WorldObjectSaveData> dataList)
        {
            if (dataList == null) return;
            
            // 🔥 P0 任务 1.4：清理所有 StoneDebris（临时碎片效果）
            // StoneDebris 是石头被挖掉时产生的临时视觉效果，不是持久化对象
            // 加载存档时需要清理，避免无限累积
            CleanupStoneDebris();
            
            // 🔥 锐评011 指令：GUID 匹配率统计
            int matchCount = 0;
            foreach (var data in dataList)
            {
                if (_registry.ContainsKey(data.guid)) matchCount++;
            }
            Debug.Log($"[Registry] 存档匹配率: {matchCount}/{dataList.Count}。如果为 0，说明 GUID 全错，必须重启游戏生成新档。");
            Debug.Log($"[Registry] 当前 Registry 中有 {_registry.Count} 个对象");
            
            // 🔥 Step 1: 构建存档快照 - 收集存档中的所有 GUID
            var savedGuids = new HashSet<string>(dataList.Select(d => d.guid));
            
            // 🔥 Step 2: 快照当前场景 - 获取 _registry.Keys 的副本（避免遍历时修改集合）
            var currentRegistryKeys = new List<string>(_registry.Keys);
            
            // 🔥 Step 3: 修剪 (Pruning) - 场景中有但存档中没有 = 已删除
            int pruned = 0;
            foreach (var guid in currentRegistryKeys)
            {
                if (!savedGuids.Contains(guid))
                {
                    // 存档中没有这个对象 → 说明玩家把它删了（砍树/挖箱子）
                    if (_registry.TryGetValue(guid, out var obj) && obj != null)
                    {
                        if (obj is MonoBehaviour mb && mb != null)
                        {
                            if (showDebugInfo)
                                Debug.Log($"[PersistentObjectRegistry] 反向修剪: {obj.ObjectType}, GUID: {obj.PersistentId}");
                            
                            // 🔥 P0 修复：区分动态对象和静态对象
                            // 动态对象（掉落物）：销毁
                            // 静态对象（石头、树木）：禁用
                            if (obj is WorldItemPickup)
                            {
                                // 掉落物是动态生成的，应该销毁
                                Destroy(mb.gameObject);
                                if (showDebugInfo)
                                    Debug.Log($"[PersistentObjectRegistry] 销毁掉落物: GUID: {obj.PersistentId}");
                            }
                            else
                            {
                                // 静态对象使用 SetActive(false)
                                mb.gameObject.SetActive(false);
                            }
                            pruned++;
                        }
                    }
                }
            }
            
            // 🔥 Step 4: 恢复 (Restoring) - 遍历存档数据进行 Load()
            int restored = 0;
            int notFound = 0;
            int reconstructed = 0;  // 新增：重建计数
            
            foreach (var data in dataList)
            {
                var obj = FindByGuid(data.guid);
                
                if (obj != null)
                {
                    // 找到对象，直接恢复
                    try
                    {
                        obj.Load(data);
                        restored++;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[PersistentObjectRegistry] 恢复对象失败: {data.objectType}, GUID: {data.guid}, 错误: {e.Message}");
                    }
                }
                else
                {
                    // 🔥 新增：尝试重建动态对象
                    if (DynamicObjectFactory.IsInitialized)
                    {
                        var reconstructedObj = DynamicObjectFactory.TryReconstruct(data);
                        if (reconstructedObj != null)
                        {
                            try
                            {
                                // 加载数据
                                reconstructedObj.Load(data);
                                reconstructed++;
                                
                                // 🛡️ 封印三：防闪烁 - Load 完成后再启用对象
                                if (reconstructedObj is MonoBehaviour mb && mb != null)
                                {
                                    // 获取根物体（对于树木是父物体）
                                    var rootGo = mb.transform.parent != null ? mb.transform.parent.gameObject : mb.gameObject;
                                    rootGo.SetActive(true);
                                }
                                
                                if (showDebugInfo)
                                    Debug.Log($"[PersistentObjectRegistry] 重建对象成功: {data.objectType}, GUID: {data.guid}");
                            }
                            catch (Exception e)
                            {
                                Debug.LogError($"[PersistentObjectRegistry] 重建对象后恢复失败: {data.objectType}, GUID: {data.guid}, 错误: {e.Message}");
                            }
                        }
                        else
                        {
                            notFound++;
                            if (showDebugInfo)
                                Debug.LogWarning($"[PersistentObjectRegistry] 找不到对象且无法重建: {data.objectType}, GUID: {data.guid}");
                        }
                    }
                    else
                    {
                        notFound++;
                        if (showDebugInfo)
                            Debug.LogWarning($"[PersistentObjectRegistry] 找不到对象（DynamicObjectFactory 未初始化）: {data.objectType}, GUID: {data.guid}");
                    }
                }
            }
            
            if (showDebugInfo)
                Debug.Log($"[PersistentObjectRegistry] 恢复完成: 成功 {restored}, 重建 {reconstructed}, 未找到 {notFound}, 修剪 {pruned}");
        }
        
        #endregion
        
        #region 辅助方法
        
        /// <summary>
        /// 🔥 P0 任务 1.4：清理所有 StoneDebris（临时碎片效果）
        /// StoneDebris 是石头被挖掉时产生的临时视觉效果，命名格式为 "StoneDebris_X"
        /// 这些对象不是持久化对象，加载存档时需要清理
        /// </summary>
        private void CleanupStoneDebris()
        {
            // 查找所有名称以 "StoneDebris_" 开头的对象
            var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            int cleanedCount = 0;
            
            foreach (var obj in allObjects)
            {
                if (obj != null && obj.name.StartsWith("StoneDebris_"))
                {
                    Destroy(obj);
                    cleanedCount++;
                }
            }
            
            if (cleanedCount > 0 && showDebugInfo)
                Debug.Log($"[PersistentObjectRegistry] 清理了 {cleanedCount} 个 StoneDebris 临时碎片");
        }
        
        #endregion
        
        #region 调试
        
#if UNITY_EDITOR
        [ContextMenu("打印所有注册对象")]
        private void DebugPrintAll()
        {
            Debug.Log($"[PersistentObjectRegistry] 已注册对象: {_registry.Count}");
            foreach (var kvp in _registry)
            {
                Debug.Log($"  - {kvp.Value.ObjectType}: {kvp.Key}");
            }
        }
        
        [ContextMenu("按类型统计")]
        private void DebugPrintByType()
        {
            Debug.Log($"[PersistentObjectRegistry] 按类型统计:");
            foreach (var kvp in _byType)
            {
                Debug.Log($"  - {kvp.Key}: {kvp.Value.Count}");
            }
        }
#endif
        
        #endregion
    }
}
