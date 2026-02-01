using System;
using System.Collections.Generic;
using UnityEngine;
using FarmGame.Data;

[DefaultExecutionOrder(-10)]
public class InventoryBootstrap : MonoBehaviour
{
    #region 数据结构
    
    [Serializable]
    public struct BootItem
    {
        public ItemData item;
        [Range(0, 4)] public int quality;
        [Min(1)] public int amount;
    }

    [Serializable]
    public class BootItemList
    {
        public string name = "新列表";
        public bool enabled = true;
        public bool foldout = true;
        public List<BootItem> items = new List<BootItem>();
        
        public BootItemList() { }
        
        public BootItemList(string name)
        {
            this.name = name;
        }
    }
    
    #endregion

    #region 序列化字段
    
    [Header("引用")]
    [SerializeField, HideInInspector] private InventoryService inventory;

    [Header("启动注入")]
    [Tooltip("编辑器中运行时自动注入")]
    [SerializeField] private bool runOnStart = false;
    [Tooltip("构建后的游戏中自动注入（Build 专用）")]
    #pragma warning disable CS0414
    [SerializeField] private bool runOnBuild = true;
    #pragma warning restore CS0414
    [SerializeField] private bool clearInventoryFirst = false;
    
    [Header("物品列表")]
    [SerializeField] private List<BootItemList> itemLists = new List<BootItemList>();
    
    // 旧版兼容字段（自动迁移后清空）
    [SerializeField, HideInInspector] private List<BootItem> items = new List<BootItem>();
    [SerializeField, HideInInspector] private bool migrated = false;
    
    #endregion

    #region 公共属性
    
    public List<BootItemList> ItemLists => itemLists;
    public bool ClearInventoryFirst { get => clearInventoryFirst; set => clearInventoryFirst = value; }
    public bool RunOnStart { get => runOnStart; set => runOnStart = value; }
    
    #endregion

    #region Unity 生命周期
    
    void OnValidate()
    {
        MigrateLegacyData();
    }
    
    void Start()
    {
        MigrateLegacyData();
        
        bool shouldRun = false;
        #if UNITY_EDITOR
        shouldRun = runOnStart;
        #else
        shouldRun = runOnBuild;
        #endif
        
        if (shouldRun)
        {
            StartCoroutine(DelayedApply());
        }
    }
    
    private System.Collections.IEnumerator DelayedApply()
    {
        yield return null;
        Apply();
    }
    
    #endregion

    #region 数据迁移
    
    /// <summary>
    /// 将旧版单列表数据迁移到新的多列表结构
    /// </summary>
    public void MigrateLegacyData()
    {
        if (migrated || items == null || items.Count == 0) return;
        
        // 创建一个新列表来存放旧数据
        var legacyList = new BootItemList("旧版物品");
        legacyList.items = new List<BootItem>(items);
        legacyList.enabled = true;
        
        // 添加到列表开头
        if (itemLists == null) itemLists = new List<BootItemList>();
        itemLists.Insert(0, legacyList);
        
        // 清空旧数据并标记已迁移
        items.Clear();
        migrated = true;
        
        Debug.Log($"<color=yellow>[InventoryBootstrap] 已迁移 {legacyList.items.Count} 个旧版物品到新列表结构</color>");
    }
    
    #endregion

    #region 核心方法
    
    /// <summary>
    /// 获取所有启用列表中的物品
    /// </summary>
    public List<BootItem> GetAllEnabledItems()
    {
        var result = new List<BootItem>();
        if (itemLists == null) return result;
        
        foreach (var list in itemLists)
        {
            if (list != null && list.enabled && list.items != null)
            {
                result.AddRange(list.items);
            }
        }
        return result;
    }

    [ContextMenu("Apply Now")] 
    public void Apply()
    {
        Debug.Log("<color=cyan>[InventoryBootstrap] Apply() 开始执行</color>");
        
        // 1. 获取 InventoryService
        if (inventory == null) inventory = FindFirstObjectByType<InventoryService>();
        if (inventory == null)
        {
            Debug.LogError("[InventoryBootstrap] 找不到 InventoryService！");
            return;
        }
        
        // 2. 清空背包（如果需要）
        if (clearInventoryFirst)
        {
            Debug.Log("[InventoryBootstrap] 清空背包...");
            for (int i = 0; i < inventory.Size; i++) inventory.ClearSlot(i);
        }

        // 3. 获取所有启用的物品
        var allItems = GetAllEnabledItems();
        
        // 4. 注入物品（🔥 锐评019：日志降噪，只输出汇总信息）
        int addedCount = 0;
        int skippedCount = 0;
        int totalItems = allItems.Count;
        
        foreach (var b in allItems)
        {
            if (b.item == null) 
            {
                skippedCount++;
                continue;
            }
            
            int id = b.item.itemID;
            int remaining = inventory.AddItem(id, b.quality, b.amount);
            
            if (remaining == 0)
            {
                addedCount++;
            }
            else if (remaining < b.amount)
            {
                addedCount++;
            }
            else
            {
                skippedCount++;
            }
        }
        
        // 🔥 锐评019：只输出汇总信息，不逐条输出
        string resultColor = skippedCount > 0 ? "yellow" : "green";
        Debug.Log($"<color={resultColor}>[InventoryBootstrap] 完成！成功添加 {addedCount}/{totalItems} 个物品" +
                  (skippedCount > 0 ? $"，跳过 {skippedCount} 个" : "") + "</color>");
    }
    
    /// <summary>
    /// 添加新的物品列表
    /// </summary>
    public BootItemList AddNewList(string name = "新列表")
    {
        if (itemLists == null) itemLists = new List<BootItemList>();
        var newList = new BootItemList(name);
        itemLists.Add(newList);
        return newList;
    }
    
    /// <summary>
    /// 删除指定索引的列表
    /// </summary>
    public void RemoveList(int index)
    {
        if (itemLists != null && index >= 0 && index < itemLists.Count)
        {
            itemLists.RemoveAt(index);
        }
    }
    
    #endregion
}
