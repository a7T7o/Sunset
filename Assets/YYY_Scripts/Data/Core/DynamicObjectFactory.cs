using UnityEngine;

namespace FarmGame.Data.Core
{
    /// <summary>
    /// 动态对象工厂
    /// 负责根据存档数据重建动态对象（树苗、掉落物等）
    /// 
    /// 核心职责：
    /// - 根据 prefabId 查找预制体并实例化
    /// - 处理旧存档兼容（Legacy Fallback）
    /// - 数据有效性验证（防腐层）
    /// 
    /// 🛡️ 封印二：回退逻辑的防腐层
    /// - 在执行 Instantiate 之前，必须先校验数据有效性
    /// - health <= 0 && !isStump → 坏死数据，直接丢弃
    /// </summary>
    public static class DynamicObjectFactory
    {
        #region 私有字段
        
        private static PrefabRegistry _registry;
        private static bool _initialized = false;
        private static bool _showDebugInfo = true;
        
        #endregion
        
        #region 初始化
        
        /// <summary>
        /// 初始化工厂（在游戏启动时调用）
        /// </summary>
        /// <param name="registry">预制体注册表</param>
        public static void Initialize(PrefabRegistry registry)
        {
            _registry = registry;
            _initialized = true;
            
            if (_showDebugInfo)
                Debug.Log($"[DynamicObjectFactory] 初始化完成，PrefabRegistry: {(registry != null ? "已加载" : "为空")}");
        }
        
        /// <summary>
        /// 检查是否已初始化
        /// </summary>
        public static bool IsInitialized => _initialized && _registry != null;
        
        #endregion
        
        #region 核心重建方法
        
        /// <summary>
        /// 尝试重建动态对象
        /// 🛡️ 封印二：回退逻辑的防腐层
        /// </summary>
        /// <param name="data">存档数据</param>
        /// <returns>重建的对象，失败返回 null</returns>
        public static IPersistentObject TryReconstruct(WorldObjectSaveData data)
        {
            if (data == null)
            {
                Debug.LogWarning("[DynamicObjectFactory] TryReconstruct: data 为 null");
                return null;
            }
            
            // === 处理掉落物 ===
            if (data.objectType == "Drop")
            {
                return TryReconstructDrop(data);
            }
            
            // === 处理树木 ===
            if (data.objectType == "Tree")
            {
                return TryReconstructTree(data);
            }
            
            // 其他类型暂不支持重建
            if (_showDebugInfo)
                Debug.Log($"[DynamicObjectFactory] 不支持重建的对象类型: {data.objectType}");
            
            return null;
        }
        
        #endregion
        
        #region 掉落物重建
        
        /// <summary>
        /// 重建掉落物
        /// 使用 WorldSpawnService.SpawnById() 而非 PrefabRegistry
        /// </summary>
        private static IPersistentObject TryReconstructDrop(WorldObjectSaveData data)
        {
            // 解析 DropDataDTO
            if (string.IsNullOrEmpty(data.genericData))
            {
                Debug.LogWarning($"[DynamicObjectFactory] 掉落物数据为空: guid={data.guid}");
                return null;
            }
            
            DropDataDTO dropData;
            try
            {
                dropData = JsonUtility.FromJson<DropDataDTO>(data.genericData);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[DynamicObjectFactory] 掉落物数据解析失败: guid={data.guid}, error={e.Message}");
                return null;
            }
            
            if (dropData == null)
            {
                Debug.LogWarning($"[DynamicObjectFactory] 掉落物数据解析结果为 null: guid={data.guid}");
                return null;
            }
            
            // 数据有效性检查
            if (dropData.itemId < 0)
            {
                Debug.LogWarning($"[DynamicObjectFactory] 掉落物 itemId 无效: guid={data.guid}, itemId={dropData.itemId}");
                return null;
            }
            
            if (dropData.amount <= 0)
            {
                Debug.LogWarning($"[DynamicObjectFactory] 掉落物数量无效: guid={data.guid}, amount={dropData.amount}");
                return null;
            }
            
            // 使用 WorldSpawnService 重建
            if (WorldSpawnService.Instance == null)
            {
                Debug.LogError("[DynamicObjectFactory] WorldSpawnService.Instance 为 null，无法重建掉落物");
                return null;
            }
            
            Vector3 position = data.GetPosition();
            var pickup = WorldSpawnService.Instance.SpawnById(
                dropData.itemId,
                dropData.quality,
                dropData.amount,
                position,
                false,  // playAnimation
                false   // setSpawnCooldown
            );
            
            if (pickup == null)
            {
                Debug.LogWarning($"[DynamicObjectFactory] 掉落物生成失败: itemId={dropData.itemId}");
                return null;
            }
            
            // 获取 IPersistentObject 组件并设置 GUID
            var persistentObj = pickup.GetComponent<IPersistentObject>();
            if (persistentObj != null)
            {
                SetPersistentId(persistentObj, data.guid);
                
                if (_showDebugInfo)
                    Debug.Log($"[DynamicObjectFactory] 掉落物重建成功: itemId={dropData.itemId}, amount={dropData.amount}, guid={data.guid}");
            }
            else
            {
                Debug.LogWarning($"[DynamicObjectFactory] 掉落物没有 IPersistentObject 组件: itemId={dropData.itemId}");
            }
            
            return persistentObj;
        }
        
        #endregion
        
        #region 树木重建
        
        /// <summary>
        /// 重建树木
        /// 🛡️ 封印二：在执行 Instantiate 之前，必须先校验数据有效性
        /// </summary>
        private static IPersistentObject TryReconstructTree(WorldObjectSaveData data)
        {
            // 解析 TreeSaveData 进行数据验证
            TreeSaveData treeData = null;
            if (!string.IsNullOrEmpty(data.genericData))
            {
                try
                {
                    treeData = JsonUtility.FromJson<TreeSaveData>(data.genericData);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[DynamicObjectFactory] 树木数据解析失败: guid={data.guid}, error={e.Message}");
                    return null;
                }
            }
            
            // === 🛡️ 封印二（修正版）：数据有效性检查（防腐层）===
            // 🔥 锐评019 修正：树苗 (Stage 0) 的血量设计就是 0，不能误杀
            if (treeData != null)
            {
                // 检查生长阶段有效性
                if (treeData.growthStageIndex < 0)
                {
                    if (_showDebugInfo)
                        Debug.LogWarning($"[DynamicObjectFactory] 跳过无效的树木数据（生长阶段无效）: guid={data.guid}, stage={treeData.growthStageIndex}");
                    return null;
                }
                
                // 🔥 修正：只有成长中的树 (Stage > 0) 且不是树桩 且血量<=0 时，才判定为"死树"
                // 树苗 (Stage 0) 的血量设计就是 0，是合法的
                if (treeData.growthStageIndex > 0 && !treeData.isStump && treeData.currentHealth <= 0)
                {
                    if (_showDebugInfo)
                        Debug.LogWarning($"[DynamicObjectFactory] 跳过死树数据: guid={data.guid}, stage={treeData.growthStageIndex}, health={treeData.currentHealth}");
                    return null;
                }
            }
            
            // === Legacy Fallback：旧存档 prefabId 为空 ===
            string prefabId = data.prefabId;
            if (string.IsNullOrEmpty(prefabId))
            {
                // 🛡️ 封印二：回退逻辑仅限于有效数据
                // 旧存档使用 M1 作为默认预制体进行抢救性重建
                prefabId = "M1";
                if (_showDebugInfo)
                    Debug.LogWarning($"[DynamicObjectFactory] 旧存档兼容：使用默认预制体 M1, guid={data.guid}");
            }
            
            // 检查 PrefabRegistry
            if (_registry == null)
            {
                Debug.LogError("[DynamicObjectFactory] PrefabRegistry 未初始化，无法重建树木");
                return null;
            }
            
            // 查找预制体
            var prefab = _registry.GetPrefab(prefabId);
            if (prefab == null)
            {
                Debug.LogWarning($"[DynamicObjectFactory] 找不到预制体: {prefabId}");
                return null;
            }
            
            // 实例化（先禁用，避免闪烁）
            Vector3 position = data.GetPosition();
            var instance = Object.Instantiate(prefab, position, Quaternion.identity);
            instance.SetActive(false);  // 🛡️ 封印三：防闪烁
            
            // 获取 IPersistentObject 组件
            var persistentObj = instance.GetComponentInChildren<IPersistentObject>();
            if (persistentObj == null)
            {
                Debug.LogError($"[DynamicObjectFactory] 预制体 {prefabId} 没有 IPersistentObject 组件");
                Object.Destroy(instance);
                return null;
            }
            
            // 强制设置 GUID（关键！）
            SetPersistentId(persistentObj, data.guid);
            
            // 注册到 Registry
            if (PersistentObjectRegistry.Instance != null)
            {
                PersistentObjectRegistry.Instance.Register(persistentObj);
            }
            
            if (_showDebugInfo)
                Debug.Log($"[DynamicObjectFactory] 树木重建成功: prefabId={prefabId}, guid={data.guid}, position={position}");
            
            return persistentObj;
        }
        
        #endregion
        
        #region 辅助方法
        
        /// <summary>
        /// 强制设置对象的 PersistentId
        /// </summary>
        private static void SetPersistentId(IPersistentObject obj, string guid)
        {
            if (obj == null || string.IsNullOrEmpty(guid)) return;
            
            if (obj is TreeController tree)
            {
                tree.SetPersistentIdForLoad(guid);
            }
            else if (obj is WorldItemPickup pickup)
            {
                pickup.SetPersistentIdForLoad(guid);
            }
            // 其他类型可以在这里扩展
        }
        
        #endregion
    }
}
