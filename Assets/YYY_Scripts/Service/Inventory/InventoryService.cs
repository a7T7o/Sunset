using System;
using System.Collections.Generic;
using UnityEngine;
using FarmGame.Data;
using FarmGame.Data.Core;

/// <summary>
/// 运行时背包服务 - V2 重构版
/// 
/// 核心改进：
/// - 内部使用 PlayerInventoryData（基于 InventoryItem）存储
/// - 实现 IPersistentObject 接口，支持存档/读档
/// - 保留所有旧接口签名，兼容现有 UI
/// - 对外暴露 ItemStack 接口，内部使用 InventoryItem
/// 
/// 设计原则：
/// - 数据以 InventoryItem 为准
/// - UI 显示时转换为 ItemStack（只在显示那一瞬间）
/// - 保存时直接序列化 InventoryItem
/// </summary>
public class InventoryService : MonoBehaviour, IItemContainer, IPersistentObject
{
    public const int DefaultInventorySize = 36; // 3行 * 12列
    public const int HotbarWidth = 12;          // 第一行 12 格，滚轮循环范围
    public const int HotbarDirectSelectCount = 5; // 数字键仅直选前 5 格

    [Header("数据库")]
    [SerializeField] private ItemDatabase database;

    [Header("容量")]
    [SerializeField] private int inventorySize = DefaultInventorySize;
    
    [Header("持久化配置")]
    [SerializeField, Tooltip("对象唯一 ID（自动生成）")]
    private string _persistentId;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    // 🔥 核心改变：使用 PlayerInventoryData 替代 ItemStack[]
    private PlayerInventoryData _inventoryData;
    
    // 兼容旧代码：保留 slots 字段用于序列化迁移
    [SerializeField, HideInInspector] 
    private ItemStack[] _legacySlots;

    // 事件
    public event Action OnInventoryChanged;
    public event Action<int> OnSlotChanged;
    public event Action<int> OnHotbarSlotChanged; // index: 0..11

    public int Size => inventorySize;
    public ItemDatabase Database => database;

    // IItemContainer 接口实现
    public int Capacity => inventorySize;
    
    #region IPersistentObject 实现
    
    public string PersistentId
    {
        get
        {
            if (string.IsNullOrEmpty(_persistentId))
            {
                _persistentId = System.Guid.NewGuid().ToString();
            }
            return _persistentId;
        }
    }
    
    public string ObjectType => "PlayerInventory";
    
    public bool ShouldSave => gameObject.activeInHierarchy;
    
    public WorldObjectSaveData Save()
    {
        var data = new WorldObjectSaveData
        {
            guid = PersistentId,
            objectType = ObjectType,
            sceneName = gameObject.scene.name,
            isActive = gameObject.activeSelf
        };
        
        // 将背包数据序列化为 JSON 存入 genericData
        var inventoryData = _inventoryData.ToSaveData();
        data.genericData = JsonUtility.ToJson(inventoryData);
        
        if (showDebugInfo)
            Debug.Log($"[InventoryService] Save: {inventoryData.slots?.Count ?? 0} 个槽位");
        
        return data;
    }
    
    public void Load(WorldObjectSaveData data)
    {
        if (data == null || string.IsNullOrEmpty(data.genericData)) return;
        
        try
        {
            var inventoryData = JsonUtility.FromJson<InventorySaveData>(data.genericData);
            _inventoryData.LoadFromSaveData(inventoryData);
            
            if (showDebugInfo)
                Debug.Log($"[InventoryService] Load: {inventoryData.slots?.Count ?? 0} 个槽位");
        }
        catch (Exception e)
        {
            Debug.LogError($"[InventoryService] Load 失败: {e.Message}");
        }
    }
    
    #endregion

    #region Unity 生命周期
    
    void Awake()
    {
        if (inventorySize <= 0) inventorySize = DefaultInventorySize;
        
        // 初始化新的数据核心
        _inventoryData = new PlayerInventoryData(inventorySize, database);
        
        // 订阅内部事件，转发到外部
        _inventoryData.OnSlotChanged += HandleInternalSlotChanged;
        _inventoryData.OnInventoryChanged += HandleInternalInventoryChanged;
        
        // 迁移旧数据（如果有）
        MigrateLegacyData();
    }
    
    void Start()
    {
        // 注册到持久化注册中心
        if (PersistentObjectRegistry.Instance != null)
        {
            PersistentObjectRegistry.Instance.Register(this);
        }
        
        // 订阅日变化事件（用于种子袋保质期检查）
        TimeManager.OnDayChanged += OnDayChanged;
    }
    
    void OnDestroy()
    {
        // 从注册中心注销
        if (PersistentObjectRegistry.Instance != null)
        {
            PersistentObjectRegistry.Instance.Unregister(this);
        }
        
        // 取消订阅
        if (_inventoryData != null)
        {
            _inventoryData.OnSlotChanged -= HandleInternalSlotChanged;
            _inventoryData.OnInventoryChanged -= HandleInternalInventoryChanged;
        }
        
        TimeManager.OnDayChanged -= OnDayChanged;
    }
    
    /// <summary>
    /// 迁移旧的 ItemStack 数据到新系统
    /// </summary>
    private void MigrateLegacyData()
    {
        if (_legacySlots != null && _legacySlots.Length > 0)
        {
            for (int i = 0; i < _legacySlots.Length && i < inventorySize; i++)
            {
                var stack = _legacySlots[i];
                if (!stack.IsEmpty)
                {
                    _inventoryData.SetSlot(i, stack);
                }
            }
            
            // 清空旧数据
            _legacySlots = null;
            
            if (showDebugInfo)
                Debug.Log("[InventoryService] 已迁移旧数据到新系统");
        }
    }
    
    private void HandleInternalSlotChanged(int index)
    {
        OnSlotChanged?.Invoke(index);
        if (index >= 0 && index < HotbarWidth)
            OnHotbarSlotChanged?.Invoke(index);
    }
    
    private void HandleInternalInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }
    
    #endregion

    #region 编辑器支持
    
#if UNITY_EDITOR
    void OnValidate()
    {
        if (string.IsNullOrEmpty(_persistentId))
        {
            _persistentId = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
    
    [ContextMenu("重新生成持久化 ID")]
    private void RegeneratePersistentId()
    {
        _persistentId = System.Guid.NewGuid().ToString();
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[InventoryService] 已重新生成 ID: {_persistentId}");
    }
#endif
    
    #endregion

    public void SetDatabase(ItemDatabase db)
    {
        database = db;
        _inventoryData?.SetDatabase(db);
    }

    #region ItemStack 兼容接口（供旧 UI 使用）
    
    /// <summary>
    /// 读取槽位（返回 ItemStack，兼容旧 UI）
    /// </summary>
    public ItemStack GetSlot(int index)
    {
        if (_inventoryData == null || !InRange(index)) return ItemStack.Empty;
        return _inventoryData.GetSlot(index);
    }

    public bool TryGetSlot(int index, out ItemStack stack)
    {
        if (InRange(index))
        {
            stack = GetSlot(index);
            return true;
        }
        stack = ItemStack.Empty;
        return false;
    }

    public bool SetSlot(int index, ItemStack stack)
    {
        if (_inventoryData == null || !InRange(index)) return false;
        return _inventoryData.SetSlot(index, stack);
    }

    public void ClearSlot(int index)
    {
        if (_inventoryData == null || !InRange(index)) return;
        _inventoryData.ClearItem(index);
    }
    
    #endregion
    
    #region InventoryItem 操作（新 API）
    
    /// <summary>
    /// 获取指定槽位的 InventoryItem（新 API）
    /// </summary>
    public InventoryItem GetInventoryItem(int index)
    {
        if (_inventoryData == null || !InRange(index)) return null;
        return _inventoryData.GetItem(index);
    }
    
    /// <summary>
    /// 设置指定槽位的 InventoryItem（新 API）
    /// </summary>
    public bool SetInventoryItem(int index, InventoryItem item)
    {
        if (_inventoryData == null || !InRange(index)) return false;
        return _inventoryData.SetItem(index, item);
    }
    
    /// <summary>
    /// 添加 InventoryItem（支持动态属性）
    /// </summary>
    public bool AddInventoryItem(InventoryItem item)
    {
        if (_inventoryData == null) return false;
        return _inventoryData.AddInventoryItem(item);
    }
    
    #endregion

    #region 交换与合并
    
    public bool SwapOrMerge(int a, int b)
    {
        if (_inventoryData == null) return false;
        return _inventoryData.SwapOrMerge(a, b);
    }
    
    #endregion

    #region 添加物品
    
    /// <summary>
    /// 添加物品（优先叠加/放置在第一行）
    /// 返回未能放入的剩余数量
    /// </summary>
    public int AddItem(int itemId, int quality, int amount)
    {
        if (_inventoryData == null || amount <= 0) return amount;
        return _inventoryData.AddItem(itemId, quality, amount);
    }

    /// <summary>
    /// 检查是否可以添加指定物品（不实际添加）
    /// </summary>
    public bool CanAddItem(int itemId, int quality, int amount)
    {
        if (_inventoryData == null) return false;
        return _inventoryData.CanAddItem(itemId, quality, amount);
    }
    
    #endregion

    #region 移除物品
    
    public bool RemoveFromSlot(int index, int amount)
    {
        if (_inventoryData == null) return false;
        return _inventoryData.RemoveFromSlot(index, amount);
    }

    /// <summary>
    /// 从背包中移除指定物品
    /// </summary>
    public bool RemoveItem(int itemId, int quality, int amount)
    {
        if (_inventoryData == null) return false;
        return _inventoryData.RemoveItem(itemId, quality, amount);
    }
    
    #endregion

    #region 查询
    
    /// <summary>
    /// 检查背包中是否有足够数量的指定物品
    /// </summary>
    public bool HasItem(int itemId, int quality, int amount)
    {
        if (_inventoryData == null) return false;
        return _inventoryData.HasItem(itemId, quality, amount);
    }

    public int GetMaxStack(int itemId)
    {
        if (database == null) return 99;
        var data = database.GetItemByID(itemId);
        if (data == null) return 99;
        return Mathf.Max(1, data.maxStackSize);
    }
    
    #endregion

    #region 排序
    
    /// <summary>
    /// 排序背包（不包括 Hotbar 第一行）
    /// </summary>
    public void Sort()
    {
        if (_inventoryData == null) return;
        _inventoryData.Sort();
        
        if (showDebugInfo)
            Debug.Log($"[InventoryService] Sort 完成");
    }
    
    /// <summary>
    /// 强制刷新指定槽位的 UI（供外部调用）
    /// </summary>
    public void RefreshSlot(int index)
    {
        if (InRange(index))
        {
            OnSlotChanged?.Invoke(index);
            if (index >= 0 && index < HotbarWidth)
                OnHotbarSlotChanged?.Invoke(index);
        }
    }
    
    /// <summary>
    /// 强制刷新所有槽位的 UI（供外部调用）
    /// </summary>
    public void RefreshAll()
    {
        OnInventoryChanged?.Invoke();
    }
    
    #endregion
    
    #region 种子袋保质期检查
    
    /// <summary>
    /// 腐烂食物 ID
    /// </summary>
    private const int ROTTEN_FOOD_ID = 5999;
    
    /// <summary>
    /// 每日检查背包中的种子袋保质期
    /// </summary>
    private void OnDayChanged(int year, int day, int totalDays)
    {
        if (_inventoryData == null) return;
        
        bool changed = false;
        
        for (int i = 0; i < inventorySize; i++)
        {
            var item = _inventoryData.GetItem(i);
            if (item == null || item.IsEmpty) continue;
            
            if (!FarmGame.Farm.SeedBagHelper.IsSeedBag(item)) continue;
            
            if (FarmGame.Farm.SeedBagHelper.IsExpired(item, totalDays))
            {
                // 过期 → 替换为腐烂食物
                // ★ 关键：不设置任何动态属性，确保 properties.Count == 0，可堆叠
                var rottenItem = new InventoryItem(ROTTEN_FOOD_ID, 0, 1);
                _inventoryData.SetItem(i, rottenItem);
                changed = true;
                
                if (showDebugInfo)
                    Debug.Log($"[InventoryService] 槽位 {i} 种子袋过期，替换为腐烂食物");
            }
        }
        
        if (changed)
        {
            // 尝试合并腐烂食物
            TryMergeRottenFood();
        }
    }
    
    /// <summary>
    /// 尝试合并背包中的腐烂食物
    /// </summary>
    private void TryMergeRottenFood()
    {
        if (_inventoryData == null) return;
        
        int maxStack = GetMaxStack(ROTTEN_FOOD_ID);
        
        for (int i = 0; i < inventorySize; i++)
        {
            var itemA = _inventoryData.GetItem(i);
            if (itemA == null || itemA.IsEmpty || itemA.ItemId != ROTTEN_FOOD_ID) continue;
            
            for (int j = i + 1; j < inventorySize; j++)
            {
                var itemB = _inventoryData.GetItem(j);
                if (itemB == null || itemB.IsEmpty || itemB.ItemId != ROTTEN_FOOD_ID) continue;
                
                if (!itemA.CanStackWith(itemB)) continue;
                
                int total = itemA.Amount + itemB.Amount;
                if (total <= maxStack)
                {
                    itemA.SetAmount(total);
                    _inventoryData.ClearItem(j);
                }
                else
                {
                    itemA.SetAmount(maxStack);
                    itemB.SetAmount(total - maxStack);
                }
            }
        }
    }
    
    #endregion

    bool InRange(int i) => i >= 0 && i < inventorySize;
}
