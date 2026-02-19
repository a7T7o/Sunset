using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmGame.Data.Core
{
    /// <summary>
    /// 存档数据传输对象（DTOs）
    /// 
    /// 设计原则：
    /// - 纯数据类，无逻辑
    /// - 支持 JSON 序列化（Unity JsonUtility 或 Newtonsoft.Json）
    /// - 与运行时对象解耦
    /// - 版本兼容性考虑
    /// </summary>
    
    #region 游戏存档根结构
    
    /// <summary>
    /// 游戏存档根数据
    /// </summary>
    [Serializable]
    public class GameSaveData
    {
        /// <summary>存档版本号（用于兼容性处理）</summary>
        public int version = 1;
        
        /// <summary>存档创建时间</summary>
        public string createdTime;
        
        /// <summary>最后保存时间</summary>
        public string lastSaveTime;
        
        /// <summary>游戏时间数据</summary>
        public GameTimeSaveData gameTime;
        
        /// <summary>玩家数据</summary>
        public PlayerSaveData player;
        
        /// <summary>背包数据</summary>
        public InventorySaveData inventory;
        
        /// <summary>世界对象数据</summary>
        public List<WorldObjectSaveData> worldObjects;
        
        /// <summary>农田数据</summary>
        public List<FarmTileSaveData> farmTiles;
        
        public GameSaveData()
        {
            createdTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            lastSaveTime = createdTime;
            worldObjects = new List<WorldObjectSaveData>();
            farmTiles = new List<FarmTileSaveData>();
        }
    }
    
    #endregion
    
    #region 游戏时间
    
    /// <summary>
    /// 游戏时间存档数据
    /// </summary>
    [Serializable]
    public class GameTimeSaveData
    {
        /// <summary>当前天数</summary>
        public int day = 1;
        
        /// <summary>当前季节（0=春, 1=夏, 2=秋, 3=冬）</summary>
        public int season = 0;
        
        /// <summary>当前年份</summary>
        public int year = 1;
        
        /// <summary>当前小时（0-23）</summary>
        public int hour = 6;
        
        /// <summary>当前分钟（0-59）</summary>
        public int minute = 0;
    }
    
    #endregion
    
    #region 玩家数据
    
    /// <summary>
    /// 玩家存档数据
    /// </summary>
    [Serializable]
    public class PlayerSaveData
    {
        /// <summary>玩家位置 X</summary>
        public float positionX;
        
        /// <summary>玩家位置 Y</summary>
        public float positionY;
        
        /// <summary>当前场景名称</summary>
        public string sceneName;
        
        /// <summary>当前楼层</summary>
        public int currentLayer = 1;
        
        /// <summary>当前选中的快捷栏槽位</summary>
        public int selectedHotbarSlot = 0;
        
        /// <summary>金币数量</summary>
        public int gold = 0;
        
        /// <summary>当前体力</summary>
        public int stamina = 100;
        
        /// <summary>最大体力</summary>
        public int maxStamina = 100;
    }
    
    #endregion
    
    #region 背包数据
    
    /// <summary>
    /// 背包存档数据
    /// </summary>
    [Serializable]
    public class InventorySaveData
    {
        /// <summary>背包容量</summary>
        public int capacity = 36;
        
        /// <summary>所有槽位数据</summary>
        public List<InventorySlotSaveData> slots;
        
        public InventorySaveData()
        {
            slots = new List<InventorySlotSaveData>();
        }
    }
    
    /// <summary>
    /// 背包槽位存档数据
    /// </summary>
    [Serializable]
    public class InventorySlotSaveData
    {
        /// <summary>槽位索引</summary>
        public int slotIndex;
        
        /// <summary>物品定义 ID（-1 表示空）</summary>
        public int itemId = -1;
        
        /// <summary>物品品质</summary>
        public int quality = 0;
        
        /// <summary>堆叠数量</summary>
        public int amount = 0;
        
        /// <summary>物品实例 ID（用于关联动态属性）</summary>
        public string instanceId;
        
        /// <summary>当前耐久度（-1 表示无耐久度）</summary>
        public int currentDurability = -1;
        
        /// <summary>最大耐久度（-1 表示无耐久度）</summary>
        public int maxDurability = -1;
        
        /// <summary>动态属性（序列化为 JSON 字符串列表）</summary>
        public List<PropertyEntrySaveData> properties;
        
        public InventorySlotSaveData()
        {
            properties = new List<PropertyEntrySaveData>();
        }
        
        /// <summary>是否为空槽位</summary>
        public bool IsEmpty => itemId < 0 || amount <= 0;
    }
    
    /// <summary>
    /// 属性条目存档数据
    /// </summary>
    [Serializable]
    public class PropertyEntrySaveData
    {
        public string key;
        public string value;
        
        public PropertyEntrySaveData() { }
        
        public PropertyEntrySaveData(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
    
    #endregion
    
    #region 世界对象数据
    
    /// <summary>
    /// 世界对象存档数据
    /// 用于存储场景中的动态对象（箱子、作物、放置的物品等）
    /// </summary>
    [Serializable]
    public class WorldObjectSaveData
    {
        /// <summary>对象唯一 ID（GUID）</summary>
        public string guid;
        
        /// <summary>对象类型标识（用于反序列化时创建正确的对象）</summary>
        public string objectType;
        
        /// <summary>预制体路径或 ID（用于实例化）</summary>
        public string prefabId;
        
        /// <summary>所在场景名称</summary>
        public string sceneName;
        
        /// <summary>所在楼层</summary>
        public int layer = 1;
        
        /// <summary>位置 X</summary>
        public float positionX;
        
        /// <summary>位置 Y</summary>
        public float positionY;
        
        /// <summary>位置 Z</summary>
        public float positionZ;
        
        /// <summary>旋转角度（2D 游戏通常只用 Z 轴）</summary>
        public float rotationZ;
        
        /// <summary>是否激活</summary>
        public bool isActive = true;
        
        // ============================================================
        // 🔴 渲染层级参数（必须保存！不是预制体默认值，是运行时动态计算的）
        // ============================================================
        
        /// <summary>
        /// 排序图层名称（Sorting Layer）
        /// 🔴 重要：这是 SpriteRenderer 的渲染层级，不是 GameObject.layer
        /// 例如："Layer 1", "Layer 2", "Effects" 等
        /// </summary>
        public string sortingLayerName;
        
        /// <summary>
        /// 图层顺序（Order in Layer）
        /// 🔴 重要：这是同一 Sorting Layer 内的渲染顺序
        /// 通常根据 Y 坐标动态计算，如 -517
        /// </summary>
        public int sortingOrder;
        
        // ============================================================
        
        /// <summary>通用数据（JSON 字符串，存储对象特有数据）</summary>
        public string genericData;
        
        /// <summary>
        /// 设置位置
        /// </summary>
        public void SetPosition(Vector3 pos)
        {
            positionX = pos.x;
            positionY = pos.y;
            positionZ = pos.z;
        }
        
        /// <summary>
        /// 获取位置
        /// </summary>
        public Vector3 GetPosition()
        {
            return new Vector3(positionX, positionY, positionZ);
        }
        
        /// <summary>
        /// 🔴 设置渲染层级参数（从 SpriteRenderer 获取）
        /// </summary>
        public void SetSortingLayer(SpriteRenderer renderer)
        {
            if (renderer != null)
            {
                sortingLayerName = renderer.sortingLayerName;
                sortingOrder = renderer.sortingOrder;
            }
        }
        
        /// <summary>
        /// 🔴 恢复渲染层级参数（应用到 SpriteRenderer）
        /// </summary>
        public void RestoreSortingLayer(SpriteRenderer renderer)
        {
            if (renderer != null && !string.IsNullOrEmpty(sortingLayerName))
            {
                renderer.sortingLayerName = sortingLayerName;
                renderer.sortingOrder = sortingOrder;
            }
        }
    }
    
    #endregion
    
    #region 特定对象数据
    
    /// <summary>
    /// 箱子存档数据（存储在 WorldObjectSaveData.genericData 中）
    /// </summary>
    [Serializable]
    public class ChestSaveData
    {
        /// <summary>箱子容量</summary>
        public int capacity = 20;
        
        /// <summary>箱子内物品</summary>
        public List<InventorySlotSaveData> slots;
        
        /// <summary>是否锁定</summary>
        public bool isLocked = false;
        
        /// <summary>自定义名称</summary>
        public string customName;
        
        public ChestSaveData()
        {
            slots = new List<InventorySlotSaveData>();
        }
    }
    
    /// <summary>
    /// 树木存档数据（存储在 WorldObjectSaveData.genericData 中）
    /// </summary>
    [Serializable]
    public class TreeSaveData
    {
        /// <summary>生长阶段索引</summary>
        public int growthStageIndex;
        
        /// <summary>当前血量</summary>
        public int currentHealth;
        
        /// <summary>最大血量</summary>
        public int maxHealth;
        
        /// <summary>已生长天数</summary>
        public int daysGrown;
        
        /// <summary>树木状态（0=正常, 1=被砍, 2=树桩）</summary>
        public int state;
        
        // ===== 动态对象重建系统新增字段 =====
        
        /// <summary>当前季节（0=春, 1=夏, 2=秋, 3=冬）</summary>
        public int season;
        
        /// <summary>是否为树桩状态</summary>
        public bool isStump;
        
        /// <summary>树桩血量</summary>
        public int stumpHealth;
        
        /// <summary>是否已渐变到下一季节（渐变不可逆）</summary>
        public bool hasTransitionedToNextSeason;
        
        /// <summary>渐变时的植被季节</summary>
        public int transitionVegetationSeason;
    }
    
    /// <summary>
    /// 石头存档数据（存储在 WorldObjectSaveData.genericData 中）
    /// </summary>
    [Serializable]
    public class StoneSaveData
    {
        /// <summary>当前阶段（0=M1, 1=M2, 2=M3, 3=M4）</summary>
        public int stage;
        
        /// <summary>矿物类型（0=None, 1=C1铜, 2=C2铁, 3=C3金）</summary>
        public int oreType;
        
        /// <summary>矿物含量指数（0-4）</summary>
        public int oreIndex;
        
        /// <summary>当前血量</summary>
        public int currentHealth;
    }
    
    /// <summary>
    /// 掉落物存档数据（存储在 WorldObjectSaveData.genericData 中）
    /// 🛡️ 封印一：必须加上 [Serializable] 特性
    /// 用于 JsonUtility 序列化/反序列化
    /// </summary>
    [Serializable]
    public class DropDataDTO
    {
        /// <summary>物品 ID</summary>
        public int itemId;
        
        /// <summary>品质等级</summary>
        public int quality;
        
        /// <summary>数量</summary>
        public int amount;
        
        /// <summary>
        /// 🔥 P2 任务 6：来源资源节点的 GUID
        /// 用于关联掉落物与其来源（石头、树木等）
        /// 如果来源节点存在且活跃，则不恢复此掉落物
        /// </summary>
        public string sourceNodeGuid;
    }
    
    /// <summary>
    /// 农田格子存档数据
    /// </summary>
    [Serializable]
    public class FarmTileSaveData
    {
        /// <summary>格子位置 X（Tilemap 坐标）</summary>
        public int tileX;
        
        /// <summary>格子位置 Y（Tilemap 坐标）</summary>
        public int tileY;
        
        /// <summary>所在楼层</summary>
        public int layer = 1;
        
        /// <summary>土地状态（0=干燥, 1=湿润深色, 2=湿润水渍）</summary>
        public int soilState;
        
        /// <summary>是否已浇水（当天）</summary>
        public bool isWatered;
        
        // ===== 10.1.0 新增字段 =====
        
        /// <summary>昨天是否浇过水</summary>
        public bool wateredYesterday;
        
        /// <summary>浇水时间（游戏小时）</summary>
        public float waterTime;
        
        /// <summary>水渍变体索引（0-2）</summary>
        public int puddleVariant;
        
        // ===== 废弃字段（保留用于兼容旧存档）=====
        
        /// <summary>[已废弃] 种植的作物 ID - 作物数据已迁移到 CropController</summary>
        [Obsolete("作物数据已迁移到 CropController，此字段仅用于兼容旧存档")]
        public int cropId = -1;
        
        /// <summary>[已废弃] 作物生长阶段</summary>
        [Obsolete("作物数据已迁移到 CropController")]
        public int cropGrowthStage;
        
        /// <summary>[已废弃] 作物品质</summary>
        [Obsolete("作物数据已迁移到 CropController")]
        public int cropQuality;
        
        /// <summary>[已废弃] 已生长天数</summary>
        [Obsolete("作物数据已迁移到 CropController")]
        public int daysGrown;
        
        /// <summary>[已废弃] 连续未浇水天数</summary>
        [Obsolete("作物数据已迁移到 CropController")]
        public int daysWithoutWater;
    }
    
    /// <summary>
    /// 耕地列表包装器（用于 JSON 序列化）
    /// FarmTileManager 使用此类序列化所有耕地数据
    /// </summary>
    [Serializable]
    public class FarmTileListWrapper
    {
        public List<FarmTileSaveData> tiles = new List<FarmTileSaveData>();
    }
    
    /// <summary>
    /// 作物存档数据（存储在 WorldObjectSaveData.genericData 中）
    /// CropController 使用此类序列化作物状态
    /// </summary>
    [Serializable]
    public class CropSaveData
    {
        /// <summary>种子物品 ID</summary>
        public int seedId;
        
        /// <summary>当前生长阶段</summary>
        public int currentStage;
        
        /// <summary>已生长天数</summary>
        public int grownDays;
        
        /// <summary>连续未浇水天数</summary>
        public int daysWithoutWater;
        
        /// <summary>是否枯萎</summary>
        public bool isWithered;
        
        /// <summary>作物品质</summary>
        public int quality;
        
        /// <summary>已收获次数（可重复收获作物）</summary>
        public int harvestCount;
        
        /// <summary>上次收获的天数</summary>
        public int lastHarvestDay;
        
        /// <summary>成熟后经过的天数（用于过熟枯萎判断）</summary>
        public int daysSinceMature;
        
        // ===== 位置信息（用于关联耕地）=====
        
        /// <summary>所在楼层索引</summary>
        public int layerIndex;
        
        /// <summary>格子坐标 X</summary>
        public int cellX;
        
        /// <summary>格子坐标 Y</summary>
        public int cellY;
    }
    
    #endregion
    
    #region 辅助方法
    
    /// <summary>
    /// 存档数据转换辅助类
    /// </summary>
    public static class SaveDataHelper
    {
        /// <summary>
        /// 将 InventoryItem 转换为存档数据
        /// </summary>
        public static InventorySlotSaveData ToSaveData(InventoryItem item, int slotIndex)
        {
            if (item == null || item.IsEmpty)
            {
                return new InventorySlotSaveData { slotIndex = slotIndex };
            }
            
            item.PrepareForSerialization();
            
            var data = new InventorySlotSaveData
            {
                slotIndex = slotIndex,
                itemId = item.ItemId,
                quality = item.Quality,
                amount = item.Amount,
                instanceId = item.InstanceId,
                currentDurability = item.CurrentDurability,
                maxDurability = item.MaxDurability
            };
            
            // 转换动态属性
            // 注意：这里需要访问 InventoryItem 的内部属性
            // 实际实现时可能需要调整
            
            return data;
        }
        
        /// <summary>
        /// 从存档数据恢复 InventoryItem
        /// </summary>
        public static InventoryItem FromSaveData(InventorySlotSaveData data)
        {
            if (data == null || data.IsEmpty)
            {
                return InventoryItem.Empty;
            }
            
            var item = new InventoryItem(data.itemId, data.quality, data.amount);
            
            if (data.maxDurability > 0)
            {
                item.SetDurability(data.maxDurability, data.currentDurability);
            }
            
            // 恢复动态属性
            if (data.properties != null)
            {
                foreach (var prop in data.properties)
                {
                    item.SetProperty(prop.key, prop.value);
                }
            }
            
            return item;
        }
    }
    
    #endregion
}
