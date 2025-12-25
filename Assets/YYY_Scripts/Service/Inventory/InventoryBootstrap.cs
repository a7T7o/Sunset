using System;
using System.Collections.Generic;
using UnityEngine;
using FarmGame.Data;

[DefaultExecutionOrder(-10)]
public class InventoryBootstrap : MonoBehaviour
{
    [Serializable]
    public struct BootItem
    {
        public ItemData item;
        [Range(0, 4)] public int quality;
        [Min(1)] public int amount;
    }

    [Header("引用")]
    [SerializeField, HideInInspector] private InventoryService inventory;

    [Header("启动注入")]
    [Tooltip("编辑器中运行时自动注入")]
    [SerializeField] private bool runOnStart = false;
    [Tooltip("构建后的游戏中自动注入（Build 专用）")]
    #pragma warning disable CS0414 // 在 #else 分支中使用
    [SerializeField] private bool runOnBuild = true;
    #pragma warning restore CS0414
    [SerializeField] private bool clearInventoryFirst = false;
    [SerializeField] private List<BootItem> items = new List<BootItem>();

    void Start()
    {
        // 根据运行环境决定是否自动注入
        bool shouldRun = false;
        #if UNITY_EDITOR
        // 编辑器中：使用 runOnStart 选项
        shouldRun = runOnStart;
        #else
        // 构建后的游戏：使用 runOnBuild 选项
        shouldRun = runOnBuild;
        #endif
        
        if (shouldRun)
        {
            // 延迟一帧执行，确保其他服务已初始化
            StartCoroutine(DelayedApply());
        }
    }
    
    private System.Collections.IEnumerator DelayedApply()
    {
        // 等待一帧，确保 InventoryService 等已初始化
        yield return null;
        Apply();
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
        
        // 2. 获取 ItemDatabase（构建版本不需要 database，直接使用 items 列表中的引用）
        // items 列表中的 ItemData 引用在构建时会被序列化保留
        
        // 3. 清空背包（如果需要）
        if (clearInventoryFirst)
        {
            Debug.Log("[InventoryBootstrap] 清空背包...");
            for (int i = 0; i < inventory.Size; i++) inventory.ClearSlot(i);
        }

        // 4. 注入物品
        int addedCount = 0;
        foreach (var b in items)
        {
            if (b.item == null) 
            {
                Debug.LogWarning("[InventoryBootstrap] 跳过空物品引用");
                continue;
            }
            int id = b.item.itemID;
            int remaining = inventory.AddItem(id, b.quality, b.amount);
            if (remaining == 0)
            {
                addedCount++;
                Debug.Log($"[InventoryBootstrap] 添加物品: {b.item.itemName} x{b.amount} (ID={id}, Quality={b.quality})");
            }
            else
            {
                Debug.LogWarning($"[InventoryBootstrap] 添加物品部分失败: {b.item.itemName}, 剩余 {remaining}");
            }
        }
        
        Debug.Log($"<color=green>[InventoryBootstrap] 完成！成功添加 {addedCount}/{items.Count} 个物品</color>");
    }
}
