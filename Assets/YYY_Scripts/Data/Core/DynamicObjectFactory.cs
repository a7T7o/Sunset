using UnityEngine;
using System.Text.RegularExpressions;
using FarmGame.World;

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
    /// 
    /// 🛡️ 锐评020：工厂容错
    /// - 如果 prefabId 查找失败，尝试清洗后再查找
    /// 
    /// 🔥 3.7.5 重构：使用 PrefabDatabase 替代 PrefabRegistry
    /// - 支持自动扫描、智能回退、ID 别名映射
    /// </summary>
    public static class DynamicObjectFactory
    {
        #region 私有字段
        
        /// <summary>
        /// 🔥 3.7.5：使用 PrefabDatabase 替代 PrefabRegistry
        /// </summary>
        private static PrefabDatabase _database;
        
        /// <summary>
        /// 🔥 向后兼容：保留对旧 PrefabRegistry 的支持
        /// </summary>
        [System.Obsolete("使用 _database 替代")]
        private static PrefabRegistry _registry;
        
        private static bool _initialized = false;
        private static bool _showDebugInfo = true;
        
        #endregion
        
        #region 初始化
        
        /// <summary>
        /// 🔥 3.7.5：使用 PrefabDatabase 初始化（推荐）
        /// </summary>
        /// <param name="database">预制体数据库</param>
        public static void Initialize(PrefabDatabase database)
        {
            _database = database;
            _initialized = true;
            
            if (_showDebugInfo)
                Debug.Log($"[DynamicObjectFactory] 初始化完成，PrefabDatabase: {(database != null ? $"已加载 ({database.EntryCount} 个预制体)" : "为空")}");
        }
        
        /// <summary>
        /// 初始化工厂（旧接口，向后兼容）
        /// </summary>
        /// <param name="registry">预制体注册表</param>
        [System.Obsolete("使用 Initialize(PrefabDatabase) 替代")]
        public static void Initialize(PrefabRegistry registry)
        {
            #pragma warning disable CS0618
            _registry = registry;
            #pragma warning restore CS0618
            _initialized = true;
            
            if (_showDebugInfo)
                Debug.Log($"[DynamicObjectFactory] 初始化完成（旧模式），PrefabRegistry: {(registry != null ? "已加载" : "为空")}");
        }
        
        /// <summary>
        /// 检查是否已初始化
        /// </summary>
        public static bool IsInitialized => _initialized && (_database != null || 
            #pragma warning disable CS0618
            _registry != null
            #pragma warning restore CS0618
        );
        
        #endregion
        
        #region 预制体查找
        
        /// <summary>
        /// 🔥 3.7.5：统一的预制体查找方法
        /// 优先使用 PrefabDatabase，回退到 PrefabRegistry
        /// </summary>
        private static GameObject GetPrefab(string prefabId)
        {
            if (string.IsNullOrEmpty(prefabId)) return null;
            
            // 优先使用 PrefabDatabase
            if (_database != null)
            {
                return _database.GetPrefab(prefabId);
            }
            
            // 回退到 PrefabRegistry
            #pragma warning disable CS0618
            if (_registry != null)
            {
                return _registry.GetPrefab(prefabId);
            }
            #pragma warning restore CS0618
            
            return null;
        }
        
        /// <summary>
        /// 🔥 3.7.5：检查预制体是否存在
        /// </summary>
        private static bool HasPrefab(string prefabId)
        {
            if (string.IsNullOrEmpty(prefabId)) return false;
            
            if (_database != null)
            {
                return _database.HasPrefab(prefabId);
            }
            
            #pragma warning disable CS0618
            if (_registry != null)
            {
                return _registry.HasPrefab(prefabId);
            }
            #pragma warning restore CS0618
            
            return false;
        }
        
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
            
            // === 🔥 P1 任务 5：处理石头 ===
            if (data.objectType == "Stone")
            {
                return TryReconstructStone(data);
            }
            
            // === 🔥 P1 任务 9：处理箱子 ===
            if (data.objectType == "Chest")
            {
                return TryReconstructChest(data);
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
        /// 🔥 P2 任务 6.3：添加来源节点关联检查
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
            
            // 🔥 P2 任务 6.3：检查来源节点是否存在且活跃
            // 如果来源节点存在且活跃，说明资源节点被恢复了，掉落物不应该存在
            if (!string.IsNullOrEmpty(dropData.sourceNodeGuid))
            {
                if (PersistentObjectRegistry.Instance != null)
                {
                    var sourceNode = PersistentObjectRegistry.Instance.FindByGuid(dropData.sourceNodeGuid);
                    if (sourceNode != null)
                    {
                        // 检查来源节点是否活跃
                        var mb = sourceNode as MonoBehaviour;
                        if (mb != null && mb.gameObject.activeInHierarchy)
                        {
                            if (_showDebugInfo)
                                Debug.Log($"[DynamicObjectFactory] 跳过掉落物重建：来源节点 {dropData.sourceNodeGuid} 存在且活跃");
                            return null;
                        }
                    }
                }
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
        /// 🔥 3.7.5：使用统一的 GetPrefab 方法
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
            
            // 🔥 3.7.5：使用统一的 GetPrefab 方法（支持 PrefabDatabase 和 PrefabRegistry）
            var prefab = GetPrefab(prefabId);
            
            // 🛡️ 锐评020：工厂容错 - 如果查找失败，尝试清洗 prefabId 后再查找
            if (prefab == null)
            {
                // 尝试清洗 prefabId (去掉可能存在的 " (1)" 后缀)
                string cleanId = Regex.Replace(prefabId, @"\s\(\d+\)$", "");
                if (cleanId != prefabId)
                {
                    prefab = GetPrefab(cleanId);
                    if (prefab != null)
                    {
                        if (_showDebugInfo)
                            Debug.LogWarning($"[DynamicObjectFactory] 原始 ID '{prefabId}' 失败，清洗后 '{cleanId}' 成功匹配");
                    }
                }
            }
            
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
        
        #region 石头重建
        
        /// <summary>
        /// 🔥 P1 任务 5：重建石头
        /// 石头使用假死机制，通常不需要动态重建
        /// 此方法用于处理极端情况（如石头被意外销毁）
        /// 🔥 3.7.5：使用统一的 GetPrefab 方法
        /// </summary>
        private static IPersistentObject TryReconstructStone(WorldObjectSaveData data)
        {
            // 解析 StoneSaveData 进行数据验证
            StoneSaveData stoneData = null;
            if (!string.IsNullOrEmpty(data.genericData))
            {
                try
                {
                    stoneData = JsonUtility.FromJson<StoneSaveData>(data.genericData);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[DynamicObjectFactory] 石头数据解析失败: guid={data.guid}, error={e.Message}");
                    return null;
                }
            }
            
            // 数据有效性检查
            if (stoneData == null)
            {
                Debug.LogWarning($"[DynamicObjectFactory] 石头数据为空: guid={data.guid}");
                return null;
            }
            
            // 根据阶段和矿物类型确定预制体 ID
            // 石头预制体命名规范：C1, C2, C3
            string prefabId = data.prefabId;
            if (string.IsNullOrEmpty(prefabId))
            {
                // 默认使用 C1
                prefabId = "C1";
                
                if (_showDebugInfo)
                    Debug.LogWarning($"[DynamicObjectFactory] 石头 prefabId 为空，使用默认: {prefabId}");
            }
            
            // 🔥 3.7.5：使用统一的 GetPrefab 方法
            var prefab = GetPrefab(prefabId);
            
            // 如果找不到，尝试使用通用石头预制体
            if (prefab == null)
            {
                prefab = GetPrefab("C1");
                if (prefab != null && _showDebugInfo)
                    Debug.LogWarning($"[DynamicObjectFactory] 找不到预制体 {prefabId}，使用默认 C1");
            }
            
            if (prefab == null)
            {
                Debug.LogWarning($"[DynamicObjectFactory] 找不到石头预制体: {prefabId}");
                return null;
            }
            
            // 实例化（先禁用，避免闪烁）
            Vector3 position = data.GetPosition();
            var instance = Object.Instantiate(prefab, position, Quaternion.identity);
            instance.SetActive(false);
            
            // 获取 IPersistentObject 组件
            var persistentObj = instance.GetComponentInChildren<IPersistentObject>();
            if (persistentObj == null)
            {
                Debug.LogError($"[DynamicObjectFactory] 石头预制体 {prefabId} 没有 IPersistentObject 组件");
                Object.Destroy(instance);
                return null;
            }
            
            // 强制设置 GUID
            SetPersistentId(persistentObj, data.guid);
            
            // 注册到 Registry
            if (PersistentObjectRegistry.Instance != null)
            {
                PersistentObjectRegistry.Instance.Register(persistentObj);
            }
            
            if (_showDebugInfo)
                Debug.Log($"[DynamicObjectFactory] 石头重建成功: prefabId={prefabId}, guid={data.guid}, position={position}");
            
            return persistentObj;
        }
        
        #endregion
        
        #region 箱子重建
        
        /// <summary>
        /// 🔥 P1 任务 9：重建箱子
        /// 箱子被挖取后从场景移除，加载存档时需要动态重建
        /// 🔥 3.7.5：使用统一的 GetPrefab 方法，支持 ID 别名映射
        /// </summary>
        private static IPersistentObject TryReconstructChest(WorldObjectSaveData data)
        {
            // 解析 ChestSaveData 进行数据验证
            ChestSaveData chestData = null;
            if (!string.IsNullOrEmpty(data.genericData))
            {
                try
                {
                    chestData = JsonUtility.FromJson<ChestSaveData>(data.genericData);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[DynamicObjectFactory] 箱子数据解析失败: guid={data.guid}, error={e.Message}");
                    return null;
                }
            }
            
            // 确定预制体 ID
            // 🔥 3.7.5：PrefabDatabase 会自动处理 ID 别名映射
            // 例如：Storage_1400_小木箱子_0 → Box_1
            string prefabId = data.prefabId;
            if (string.IsNullOrEmpty(prefabId))
            {
                // 默认使用 Box_1
                prefabId = "Box_1";
                if (_showDebugInfo)
                    Debug.LogWarning($"[DynamicObjectFactory] 箱子 prefabId 为空，使用默认: {prefabId}");
            }
            
            // 🔥 3.7.5：使用统一的 GetPrefab 方法（支持 ID 别名映射）
            var prefab = GetPrefab(prefabId);
            
            // 如果找不到，尝试使用默认箱子预制体
            if (prefab == null)
            {
                prefab = GetPrefab("Box_1");
                if (prefab != null && _showDebugInfo)
                    Debug.LogWarning($"[DynamicObjectFactory] 找不到预制体 {prefabId}，使用默认 Box_1");
            }
            
            if (prefab == null)
            {
                Debug.LogWarning($"[DynamicObjectFactory] 找不到箱子预制体: {prefabId}");
                return null;
            }
            
            // 实例化（先禁用，避免闪烁）
            Vector3 position = data.GetPosition();
            var instance = Object.Instantiate(prefab, position, Quaternion.identity);
            instance.SetActive(false);
            
            // 获取 IPersistentObject 组件
            var persistentObj = instance.GetComponentInChildren<IPersistentObject>();
            if (persistentObj == null)
            {
                Debug.LogError($"[DynamicObjectFactory] 箱子预制体 {prefabId} 没有 IPersistentObject 组件");
                Object.Destroy(instance);
                return null;
            }
            
            // 强制设置 GUID
            SetPersistentId(persistentObj, data.guid);
            
            // 注册到 Registry
            if (PersistentObjectRegistry.Instance != null)
            {
                PersistentObjectRegistry.Instance.Register(persistentObj);
            }
            
            if (_showDebugInfo)
                Debug.Log($"[DynamicObjectFactory] 箱子重建成功: prefabId={prefabId}, guid={data.guid}, position={position}");
            
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
            else if (obj is StoneController stone)
            {
                stone.SetPersistentIdForLoad(guid);
            }
            else if (obj is FarmGame.World.ChestController chest)
            {
                chest.SetPersistentIdForLoad(guid);
            }
            // 其他类型可以在这里扩展
        }
        
        #endregion
    }
}
