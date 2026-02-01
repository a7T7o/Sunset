using System;
using System.IO;
using UnityEngine;

namespace FarmGame.Data.Core
{
    /// <summary>
    /// 存档管理器 (MVP 版本)
    /// 
    /// 职责：
    /// - 协调存档/读档流程
    /// - 收集全局数据（时间、玩家）
    /// - 序列化/反序列化 JSON
    /// - 文件读写
    /// 
    /// 本阶段简化：
    /// - 只做当前场景内的状态恢复（不换场景）
    /// - 使用 Unity JsonUtility（简单但有限制）
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        #region 单例
        
        private static SaveManager _instance;
        
        public static SaveManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<SaveManager>();
                    
                    if (_instance == null)
                    {
                        var go = new GameObject("[SaveManager]");
                        _instance = go.AddComponent<SaveManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
        
        #endregion
        
        #region 配置
        
        [Header("存档配置")]
        [SerializeField] private string saveFileExtension = ".json";
        [SerializeField] private string saveFolder = "Save";
        
        [Header("调试")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private bool prettyPrintJson = true;
        
        #endregion
        
        #region 属性
        
        /// <summary>
        /// 存档目录路径（Assets/Save）
        /// </summary>
        public string SaveFolderPath
        {
            get
            {
#if UNITY_EDITOR
                // 编辑器模式：使用 Assets 目录
                return Path.Combine(Application.dataPath, saveFolder);
#else
                // 打包后：使用游戏根目录
                return Path.Combine(Application.dataPath, "..", saveFolder);
#endif
            }
        }
        
        /// <summary>
        /// 当前加载的存档数据（用于调试）
        /// </summary>
        public GameSaveData CurrentSaveData { get; private set; }
        
        #endregion
        
        #region Unity 生命周期
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            
            // 🔥 修复：DontDestroyOnLoad 只对根级 GameObject 有效
            // 如果当前对象有父对象，先解除父子关系
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
            
            // 确保存档目录存在
            EnsureSaveFolderExists();
            
            // 🔥 初始化 DynamicObjectFactory（动态对象重建系统）
            InitializeDynamicObjectFactory();
            
            if (showDebugInfo)
                Debug.Log($"[SaveManager] 初始化完成，存档路径: {SaveFolderPath}");
        }
        
        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
        
        #endregion
        
        #region 核心 API
        
        /// <summary>
        /// 保存游戏
        /// </summary>
        /// <param name="slotName">存档槽名称（如 "slot1", "autosave"）</param>
        /// <returns>是否成功</returns>
        public bool SaveGame(string slotName)
        {
            if (string.IsNullOrEmpty(slotName))
            {
                Debug.LogError("[SaveManager] 存档名称不能为空");
                return false;
            }
            
            try
            {
                // 1. 创建存档数据结构
                var saveData = new GameSaveData();
                saveData.lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                
                // 2. 收集游戏时间数据
                saveData.gameTime = CollectGameTimeData();
                
                // 3. 收集玩家数据
                saveData.player = CollectPlayerData();
                
                // 4. 收集背包数据
                saveData.inventory = CollectInventoryData();
                
                // 5. 收集世界对象数据（通过 Registry）
                if (PersistentObjectRegistry.Instance != null)
                {
                    saveData.worldObjects = PersistentObjectRegistry.Instance.CollectAllSaveData();
                }
                
                // 6. 序列化为 JSON
                string json = prettyPrintJson 
                    ? JsonUtility.ToJson(saveData, true) 
                    : JsonUtility.ToJson(saveData);
                
                // 7. 写入文件
                string filePath = GetSaveFilePath(slotName);
                File.WriteAllText(filePath, json);
                
                CurrentSaveData = saveData;
                
                if (showDebugInfo)
                    Debug.Log($"[SaveManager] 保存成功: {filePath}, 世界对象: {saveData.worldObjects?.Count ?? 0}");
                
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 保存失败: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }
        
        /// <summary>
        /// 加载游戏
        /// </summary>
        /// <param name="slotName">存档槽名称</param>
        /// <returns>是否成功</returns>
        public bool LoadGame(string slotName)
        {
            if (string.IsNullOrEmpty(slotName))
            {
                Debug.LogError("[SaveManager] 存档名称不能为空");
                return false;
            }
            
            // 🔥 锐评010 修复：清理空引用，而不是清空所有
            // 原地读档模式下，Registry 是连接存档数据和场景实例的唯一桥梁，绝对不能断！
            // Clear() 会把所有活着的对象引用删除，导致后续 RestoreAllFromSaveData() 找不到对象
            if (PersistentObjectRegistry.Instance != null)
            {
                PersistentObjectRegistry.Instance.PruneStaleRecords();
                if (showDebugInfo)
                    Debug.Log("[SaveManager] 已清理 PersistentObjectRegistry 中的空引用（保留活着的对象）");
            }
            
            string filePath = GetSaveFilePath(slotName);
            
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[SaveManager] 存档文件不存在: {filePath}");
                return false;
            }
            
            try
            {
                // 1. 读取文件
                string json = File.ReadAllText(filePath);
                
                // 2. 反序列化
                var saveData = JsonUtility.FromJson<GameSaveData>(json);
                
                if (saveData == null)
                {
                    Debug.LogError("[SaveManager] 存档数据解析失败");
                    return false;
                }
                
                // 3. 恢复游戏时间
                RestoreGameTimeData(saveData.gameTime);
                
                // 4. 恢复玩家数据
                RestorePlayerData(saveData.player);
                
                // 5. 恢复背包数据
                RestoreInventoryData(saveData.inventory);
                
                // 6. 恢复世界对象数据
                if (PersistentObjectRegistry.Instance != null && saveData.worldObjects != null)
                {
                    PersistentObjectRegistry.Instance.RestoreAllFromSaveData(saveData.worldObjects);
                }
                
                CurrentSaveData = saveData;
                
                if (showDebugInfo)
                    Debug.Log($"[SaveManager] 加载成功: {filePath}, 世界对象: {saveData.worldObjects?.Count ?? 0}");
                
                // 刷新 UI（读档后立即更新显示）
                RefreshAllUI();
                
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 加载失败: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }
        
        /// <summary>
        /// 检查存档是否存在
        /// </summary>
        public bool SaveExists(string slotName)
        {
            return File.Exists(GetSaveFilePath(slotName));
        }
        
        /// <summary>
        /// 删除存档
        /// </summary>
        public bool DeleteSave(string slotName)
        {
            string filePath = GetSaveFilePath(slotName);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                if (showDebugInfo)
                    Debug.Log($"[SaveManager] 删除存档: {filePath}");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// 获取所有存档槽名称
        /// </summary>
        public string[] GetAllSaveSlots()
        {
            if (!Directory.Exists(SaveFolderPath))
                return Array.Empty<string>();
            
            var files = Directory.GetFiles(SaveFolderPath, "*" + saveFileExtension);
            var slots = new string[files.Length];
            
            for (int i = 0; i < files.Length; i++)
            {
                slots[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            
            return slots;
        }
        
        #endregion
        
        #region 数据收集
        
        /// <summary>
        /// 收集游戏时间数据
        /// Rule: P1-2 时间恢复 - 从 TimeManager 获取实际时间
        /// </summary>
        private GameTimeSaveData CollectGameTimeData()
        {
            var data = new GameTimeSaveData();
            
            // 从 TimeManager 获取数据
            if (TimeManager.Instance != null)
            {
                data.day = TimeManager.Instance.GetDay();
                data.season = (int)TimeManager.Instance.GetSeason();
                data.year = TimeManager.Instance.GetYear();
                data.hour = TimeManager.Instance.GetHour();
                data.minute = TimeManager.Instance.GetMinute();
                
                if (showDebugInfo)
                    Debug.Log($"[SaveManager] 收集时间数据: Year {data.year} Season {data.season} Day {data.day} {data.hour}:{data.minute:D2}");
            }
            else
            {
                // 回退到默认值
                data.day = 1;
                data.season = 0;
                data.year = 1;
                data.hour = 6;
                data.minute = 0;
                
                Debug.LogWarning("[SaveManager] TimeManager 未找到，使用默认时间");
            }
            
            return data;
        }
        
        /// <summary>
        /// 收集玩家数据
        /// 注意：Tool 子物体不需要排除，因为：
        /// 1. PlayerSaveData 只保存位置、场景等基础数据
        /// 2. Tool 没有实现 IPersistentObject，不会被 Registry 收集
        /// 🔥 锐评013 修复：使用 FindPlayerRoot() 确保找到真正的 Player
        /// </summary>
        private PlayerSaveData CollectPlayerData()
        {
            var data = new PlayerSaveData();
            
            // 🔥 使用 FindPlayerRoot() 而不是 FindGameObjectWithTag
            var player = FindPlayerRoot();
            if (player != null)
            {
                data.positionX = player.transform.position.x;
                data.positionY = player.transform.position.y;
                data.sceneName = player.scene.name;
                
                // Tool 子物体不需要特殊处理：
                // - 当前只保存玩家位置，不收集子物体数据
                // - Tool 是运行时动态控制的，不需要持久化
            }
            
            return data;
        }
        
        /// <summary>
        /// 收集背包数据
        /// 注意：InventoryService 现在实现了 IPersistentObject，
        /// 会通过 PersistentObjectRegistry 自动收集
        /// 这里保留方法用于兼容性，但实际数据由 Registry 收集
        /// </summary>
        private InventorySaveData CollectInventoryData()
        {
            var data = new InventorySaveData();
            
            // InventoryService 现在通过 IPersistentObject 接口保存
            // 这里只返回空数据，实际数据在 worldObjects 中
            // 保留此方法是为了兼容旧存档格式
            
            return data;
        }
        
        #endregion
        
        #region 数据恢复
        
        /// <summary>
        /// 恢复游戏时间数据
        /// Rule: P1-2 时间恢复 - 调用 TimeManager.SetTime()
        /// </summary>
        private void RestoreGameTimeData(GameTimeSaveData data)
        {
            if (data == null) return;
            
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.SetTime(
                    data.year,
                    (SeasonManager.Season)data.season,
                    data.day,
                    data.hour,
                    data.minute
                );
                
                if (showDebugInfo)
                    Debug.Log($"[SaveManager] 恢复时间: Year {data.year} Season {data.season} Day {data.day} {data.hour}:{data.minute:D2}");
            }
            else
            {
                Debug.LogWarning("[SaveManager] TimeManager 未找到，无法恢复时间");
            }
        }
        
        /// <summary>
        /// 恢复玩家数据
        /// 🔥 锐评012 修复：终极暴力复位 - 直接设置 Rigidbody2D.position + 协程验证
        /// 🔥 锐评013 修复：确保找到的是真正的 Player 根节点，而不是子物体 Tool
        /// </summary>
        private void RestorePlayerData(PlayerSaveData data)
        {
            if (data == null) return;
            
            // 🔥 锐评013 修复：FindGameObjectWithTag 可能返回 Tool（也有 Player 标签）
            // 必须确保找到的是真正的 Player 根节点（有 PlayerMovement 组件的那个）
            var player = FindPlayerRoot();
            if (player != null)
            {
                Vector3 oldPosition = player.transform.position;
                Vector3 newPosition = new Vector3(data.positionX, data.positionY, 0);
                
                // 🔥 Step 1: 暂时禁用 Animator（防止 Root Motion 或动画帧锁定位置）
                var animator = player.GetComponent<Animator>();
                bool animatorWasEnabled = false;
                if (animator != null)
                {
                    animatorWasEnabled = animator.enabled;
                    animator.enabled = false;
                }
                
                // 🔥 Step 2: 获取 Rigidbody2D 并完全控制
                var rb = player.GetComponent<Rigidbody2D>();
                bool wasSimulated = true;
                RigidbodyInterpolation2D originalInterpolation = RigidbodyInterpolation2D.None;
                RigidbodyType2D originalBodyType = RigidbodyType2D.Dynamic;
                
                if (rb != null)
                {
                    wasSimulated = rb.simulated;
                    originalInterpolation = rb.interpolation;
                    originalBodyType = rb.bodyType;
                    
                    // 🔥 关键：设置为 Kinematic，完全禁用物理模拟
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    rb.interpolation = RigidbodyInterpolation2D.None;
                    rb.simulated = false;
                    
                    // 清零速度
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    
                    // 🔥 关键：直接设置 Rigidbody2D.position（而不是 transform.position）
                    rb.position = new Vector2(data.positionX, data.positionY);
                }
                
                // 🔥 Step 3: 同时设置 Transform.position（双保险）
                player.transform.position = newPosition;
                
                // 🔥 Step 4: 强制物理引擎立即同步
                Physics2D.SyncTransforms();
                
                // 🔥 Step 5: 递归重置所有子物体的 localPosition
                RecursiveResetChildPositions(player.transform);
                
                // 🔥 Step 6: 再次强制同步
                Physics2D.SyncTransforms();
                
                // 🔥 Step 7: 恢复物理组件状态
                if (rb != null)
                {
                    rb.bodyType = originalBodyType;
                    rb.simulated = wasSimulated;
                    rb.interpolation = originalInterpolation;
                    
                    // 🔥 强制物理休眠
                    if (rb.bodyType == RigidbodyType2D.Dynamic)
                    {
                        rb.Sleep();
                    }
                    
                    // 最后一次同步
                    Physics2D.SyncTransforms();
                }
                
                // 🔥 Step 8: 恢复 Animator
                if (animator != null)
                {
                    animator.enabled = animatorWasEnabled;
                    animator.Update(0);
                }
                
                // 🔥 锐评012 指令：输出详细日志
                Debug.Log($"[SaveManager] 玩家瞬移: {oldPosition} -> {newPosition}");
                Debug.Log($"[SaveManager] 设置后 Transform.position: {player.transform.position}");
                if (rb != null)
                {
                    Debug.Log($"[SaveManager] 设置后 Rigidbody2D.position: {rb.position}");
                }
                
                var tool = player.transform.Find("Tool");
                if (tool != null)
                {
                    Debug.Log($"[SaveManager] Tool 世界坐标: {tool.position}, 本地坐标: {tool.localPosition}");
                }
                
                // 🔥 锐评012 指令：启动协程检测下一帧位置
                StartCoroutine(CheckPositionNextFrame(player, newPosition));
                
                if (showDebugInfo)
                    Debug.Log($"[SaveManager] 恢复玩家位置完成: ({data.positionX}, {data.positionY})");
            }
        }
        
        /// <summary>
        /// 递归重置所有子物体的 localPosition
        /// 🔥 锐评012 指令：确保所有层级的子物体都归零
        /// </summary>
        private void RecursiveResetChildPositions(Transform parent)
        {
            foreach (Transform child in parent)
            {
                // Tool 必须在 (0,0,0)
                if (child.name == "Tool" || child.name.Contains("Tool"))
                {
                    if (child.localPosition != Vector3.zero)
                    {
                        Debug.Log($"[SaveManager] 重置 {child.name} localPosition: {child.localPosition} -> (0,0,0)");
                        child.localPosition = Vector3.zero;
                    }
                }
                
                // 递归处理子物体的子物体
                if (child.childCount > 0)
                {
                    RecursiveResetChildPositions(child);
                }
            }
        }
        
        /// <summary>
        /// 协程：检测下一帧玩家位置是否被"内鬼"脚本修改
        /// 🔥 锐评012 指令：如果位置被改回去，说明有脚本在 Update/LateUpdate 里强制修改位置
        /// </summary>
        private System.Collections.IEnumerator CheckPositionNextFrame(GameObject player, Vector3 targetPos)
        {
            yield return null; // 等一帧
            
            if (player == null) yield break;
            
            Vector3 currentPos = player.transform.position;
            float distance = Vector3.Distance(currentPos, targetPos);
            
            if (distance > 0.1f)
            {
                Debug.LogError($"[SaveManager] ⚠️ 异常！刚移动完一帧后，玩家位置被改回了！\n" +
                    $"  目标位置: {targetPos}\n" +
                    $"  当前位置: {currentPos}\n" +
                    $"  偏移距离: {distance}\n" +
                    $"  一定有脚本在 Update/LateUpdate 里强制修改位置！");
                
                // 检查 Tool 位置
                var tool = player.transform.Find("Tool");
                if (tool != null)
                {
                    Debug.LogError($"[SaveManager] Tool 当前状态:\n" +
                        $"  世界坐标: {tool.position}\n" +
                        $"  本地坐标: {tool.localPosition}");
                }
                
                // 检查 Rigidbody2D 位置
                var rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Debug.LogError($"[SaveManager] Rigidbody2D 当前位置: {rb.position}");
                }
            }
            else
            {
                Debug.Log($"[SaveManager] ✓ 位置验证通过，玩家位置稳定在: {currentPos}");
            }
        }
        
        /// <summary>
        /// 恢复背包数据
        /// 注意：InventoryService 现在实现了 IPersistentObject，
        /// 会通过 PersistentObjectRegistry 自动恢复
        /// 这里保留方法用于兼容旧存档
        /// </summary>
        private void RestoreInventoryData(InventorySaveData data)
        {
            // InventoryService 现在通过 IPersistentObject 接口恢复
            // 这里只处理旧存档格式的兼容性
            
            if (data == null || data.slots == null || data.slots.Count == 0) return;
            
            // 如果旧存档有数据，尝试迁移到新系统
            var inventory = FindFirstObjectByType<InventoryService>();
            if (inventory != null)
            {
                foreach (var slotData in data.slots)
                {
                    if (slotData.slotIndex >= 0 && slotData.slotIndex < inventory.Size && !slotData.IsEmpty)
                    {
                        // 使用新的 InventoryItem API
                        var item = SaveDataHelper.FromSaveData(slotData);
                        inventory.SetInventoryItem(slotData.slotIndex, item);
                    }
                }
                
                if (showDebugInfo)
                    Debug.Log($"[SaveManager] 已从旧存档格式迁移背包数据");
            }
        }
        
        #endregion
        
        #region 辅助方法
        
        /// <summary>
        /// 初始化 DynamicObjectFactory（动态对象重建系统）
        /// 加载 PrefabRegistry 并初始化工厂
        /// </summary>
        private void InitializeDynamicObjectFactory()
        {
            // 尝试从 Resources 加载 PrefabRegistry
            var registry = Resources.Load<PrefabRegistry>("Data/Database/PrefabRegistry");
            
            if (registry == null)
            {
                // 尝试其他路径
                registry = Resources.Load<PrefabRegistry>("PrefabRegistry");
            }
            
#if UNITY_EDITOR
            // 编辑器模式下，尝试从 AssetDatabase 加载
            if (registry == null)
            {
                var guids = UnityEditor.AssetDatabase.FindAssets("t:PrefabRegistry");
                if (guids != null && guids.Length > 0)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    registry = UnityEditor.AssetDatabase.LoadAssetAtPath<PrefabRegistry>(path);
                    if (showDebugInfo && registry != null)
                        Debug.Log($"[SaveManager] 从 AssetDatabase 加载 PrefabRegistry: {path}");
                }
            }
#endif
            
            if (registry != null)
            {
                DynamicObjectFactory.Initialize(registry);
                if (showDebugInfo)
                    Debug.Log("[SaveManager] DynamicObjectFactory 初始化成功");
            }
            else
            {
                Debug.LogWarning("[SaveManager] 未找到 PrefabRegistry，动态对象重建功能将不可用。" +
                    "请在 Assets/111_Data/Database/ 下创建 PrefabRegistry.asset");
            }
        }
        
        /// <summary>
        /// 查找真正的 Player 根节点
        /// 🔥 锐评013 修复：场景中 Tool 子物体也有 "Player" 标签，
        /// FindGameObjectWithTag 可能返回 Tool 而不是 Player 根节点
        /// 必须通过 PlayerMovement 组件来确认是真正的 Player
        /// </summary>
        private GameObject FindPlayerRoot()
        {
            // 方法 1：通过 PlayerMovement 组件查找（最可靠）
            var playerMovement = FindFirstObjectByType<PlayerMovement>();
            if (playerMovement != null)
            {
                if (showDebugInfo)
                    Debug.Log($"[SaveManager] FindPlayerRoot: 通过 PlayerMovement 找到 Player: {playerMovement.gameObject.name}");
                return playerMovement.gameObject;
            }
            
            // 方法 2：遍历所有 Player 标签的对象，找到有 Rigidbody2D 的那个
            var allPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (var obj in allPlayers)
            {
                // 真正的 Player 根节点应该有 Rigidbody2D
                if (obj.GetComponent<Rigidbody2D>() != null)
                {
                    if (showDebugInfo)
                        Debug.Log($"[SaveManager] FindPlayerRoot: 通过 Rigidbody2D 找到 Player: {obj.name}");
                    return obj;
                }
            }
            
            // 方法 3：回退到原来的方法（不推荐，但作为最后手段）
            var fallback = GameObject.FindGameObjectWithTag("Player");
            if (fallback != null)
            {
                Debug.LogWarning($"[SaveManager] FindPlayerRoot: 使用回退方法找到: {fallback.name}，可能不是真正的 Player 根节点！");
            }
            
            return fallback;
        }
        
        /// <summary>
        /// 获取存档文件路径
        /// </summary>
        private string GetSaveFilePath(string slotName)
        {
            return Path.Combine(SaveFolderPath, slotName + saveFileExtension);
        }
        
        /// <summary>
        /// 确保存档目录存在
        /// </summary>
        private void EnsureSaveFolderExists()
        {
            if (!Directory.Exists(SaveFolderPath))
            {
                Directory.CreateDirectory(SaveFolderPath);
            }
        }
        
        /// <summary>
        /// 刷新所有 UI（读档后调用）
        /// Rule: P1-1 背包刷新 - 读档后立即刷新 UI
        /// </summary>
        private void RefreshAllUI()
        {
            // 刷新背包面板
            var inventoryPanel = FindFirstObjectByType<InventoryPanelUI>();
            if (inventoryPanel != null)
            {
                inventoryPanel.RefreshAll();
            }
            
            // 刷新工具栏
            var toolbar = FindFirstObjectByType<ToolbarUI>();
            if (toolbar != null)
            {
                toolbar.ForceRefresh();
            }
            
            if (showDebugInfo)
                Debug.Log("[SaveManager] UI 已刷新");
        }
        
        #endregion
        
        #region 调试命令
        
#if UNITY_EDITOR
        [ContextMenu("快速保存 (slot1)")]
        private void DebugQuickSave()
        {
            SaveGame("slot1");
        }
        
        [ContextMenu("快速加载 (slot1)")]
        private void DebugQuickLoad()
        {
            LoadGame("slot1");
        }
        
        [ContextMenu("打印存档路径")]
        private void DebugPrintSavePath()
        {
            Debug.Log($"[SaveManager] 存档路径: {SaveFolderPath}");
            Debug.Log($"[SaveManager] 现有存档: {string.Join(", ", GetAllSaveSlots())}");
        }
        
        [ContextMenu("打开存档目录")]
        private void DebugOpenSaveFolder()
        {
            EnsureSaveFolderExists();
            System.Diagnostics.Process.Start(SaveFolderPath);
        }
#endif
        
        #endregion
    }
}
