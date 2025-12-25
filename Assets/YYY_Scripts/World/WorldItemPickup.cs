using UnityEngine;
using System.Collections;
using FarmGame.Data;

public class WorldItemPickup : MonoBehaviour
{
    [Header("数据")]
    [Tooltip("物品ID（-1表示未初始化，会尝试从预制体名称解析）")]
    public int itemId = -1;
    [Range(0,4)] public int quality = 0;
    [Min(1)] public int amount = 1;
    
    [Header("关联数据（可选）")]
    [Tooltip("直接关联的 ItemData，用于预制体拖入场景时自动初始化")]
    [SerializeField] private ItemData linkedItemData;

    [Header("表现")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite fallbackSprite;
    
    [Header("飞向玩家动画")]
    [SerializeField] private float flyDuration = 0.25f;
    [SerializeField] private float flyHeight = 0.3f;

    private ItemDatabase database;
    private bool _isFlying = false;
    private Coroutine _flyCoroutine;
    private bool _initialized = false;
    
    /// <summary>
    /// 是否正在飞向玩家
    /// </summary>
    public bool IsFlying => _isFlying;

    public void Init(ItemDatabase db, ItemStack stack)
    {
        database = db;
        itemId = stack.itemId;
        quality = stack.quality;
        amount = Mathf.Max(1, stack.amount);
        _initialized = true;
        ApplyVisual();
    }

    public void Init(ItemData data, int q, int amt)
    {
        if (data != null)
        {
            itemId = data.itemID;
            quality = q;
            amount = Mathf.Max(1, amt);
            linkedItemData = data;
            _initialized = true;
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            var sp = data.GetBagSprite();
            if (spriteRenderer != null && sp != null) spriteRenderer.sprite = sp;
            
            // ★ 应用显示尺寸
            ApplyDisplaySize();
        }
        else
        {
            ApplyVisual();
        }
    }
    
    /// <summary>
    /// 确保物品已初始化（用于预制体拖入场景的情况）
    /// </summary>
    private void EnsureInitialized()
    {
        if (_initialized) return;
        
        // 1. 优先使用关联的 ItemData
        if (linkedItemData != null)
        {
            itemId = linkedItemData.itemID;
            _initialized = true;
            Debug.Log($"[WorldItemPickup] 从 linkedItemData 初始化: itemId={itemId}");
            return;
        }
        
        // 2. 尝试从预制体名称解析 itemId
        // 预制体命名格式：WorldItem_{itemId}_{itemName}
        if (itemId < 0)
        {
            string objName = gameObject.name;
            // 移除 "(Clone)" 后缀
            if (objName.EndsWith("(Clone)"))
            {
                objName = objName.Substring(0, objName.Length - 7).Trim();
            }
            
            // 解析格式：WorldItem_{itemId}_{itemName}
            if (objName.StartsWith("WorldItem_"))
            {
                string[] parts = objName.Split('_');
                if (parts.Length >= 2)
                {
                    if (int.TryParse(parts[1], out int parsedId))
                    {
                        itemId = parsedId;
                        _initialized = true;
                        Debug.Log($"[WorldItemPickup] 从预制体名称解析: itemId={itemId}");
                        return;
                    }
                }
            }
        }
        
        // 3. 如果仍然无效，记录警告
        if (itemId < 0)
        {
            Debug.LogWarning($"[WorldItemPickup] 无法初始化物品 '{gameObject.name}'：itemId={itemId}，请设置 linkedItemData 或使用正确的预制体命名格式");
        }
        
        _initialized = true;
    }
    
    private void Start()
    {
        // 确保物品已初始化
        EnsureInitialized();
    }

    public void ApplyVisual()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (database == null && WorldSpawnService.Instance != null) database = WorldSpawnService.Instance.Database;
        if (database == null)
        {
            database = Resources.Load<FarmGame.Data.ItemDatabase>("Data/Database/MasterItemDatabase");
#if UNITY_EDITOR
            if (database == null)
            {
                var guids = UnityEditor.AssetDatabase.FindAssets("t:ItemDatabase MasterItemDatabase");
                if (guids != null && guids.Length > 0)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    database = UnityEditor.AssetDatabase.LoadAssetAtPath<FarmGame.Data.ItemDatabase>(path);
                }
                if (database == null)
                {
                    var any = UnityEditor.AssetDatabase.FindAssets("t:ItemDatabase");
                    if (any != null && any.Length > 0)
                    {
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(any[0]);
                        database = UnityEditor.AssetDatabase.LoadAssetAtPath<FarmGame.Data.ItemDatabase>(path);
                    }
                }
            }
#endif
        }
        if (spriteRenderer != null && database != null)
        {
            var data = database.GetItemByID(itemId);
            if (data != null)
            {
                var sp = data.GetBagSprite();
                spriteRenderer.sprite = sp != null ? sp : fallbackSprite;
            }
        }
    }

    public ItemStack GetStack() => new ItemStack(itemId, quality, amount);

    public bool TryPickup(InventoryService inventory)
    {
        if (inventory == null) return false;
        int rem = inventory.AddItem(itemId, quality, amount);
        if (rem == 0)
        {
            // 停止动画
            var dropAnim = GetComponent<WorldItemDrop>();
            if (dropAnim != null)
            {
                dropAnim.StopAnimation();
            }

            // 优先使用对象池回收
            if (WorldItemPool.Instance != null)
            {
                WorldItemPool.Instance.Despawn(this);
            }
            else
            {
                Destroy(gameObject);
            }
            return true;
        }
        amount = rem; // 未拾完，更新堆叠
        return false;
    }
    
    /// <summary>
    /// 飞向玩家动画
    /// </summary>
    /// <param name="player">玩家 Transform</param>
    /// <param name="inventory">背包服务</param>
    public void FlyToPlayer(Transform player, InventoryService inventory)
    {
        if (_isFlying) return;
        if (player == null || inventory == null) return;
        
        _isFlying = true;
        
        // 停止掉落动画
        var dropAnim = GetComponent<WorldItemDrop>();
        if (dropAnim != null)
        {
            dropAnim.StopAnimation();
        }
        
        _flyCoroutine = StartCoroutine(FlyToPlayerCoroutine(player, inventory));
    }
    
    /// <summary>
    /// 飞向玩家协程
    /// </summary>
    private IEnumerator FlyToPlayerCoroutine(Transform player, InventoryService inventory)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        
        // 获取玩家 Collider 中心作为目标点
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider == null)
            playerCollider = player.GetComponentInChildren<Collider2D>();
        
        while (elapsed < flyDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flyDuration;
            
            // 使用缓动曲线（ease out cubic）
            float easedT = 1f - Mathf.Pow(1f - t, 3f);
            
            // 获取当前目标位置（玩家可能在移动）
            Vector3 targetPos = playerCollider != null 
                ? playerCollider.bounds.center 
                : player.position;
            
            // 计算当前位置（带抛物线弧度）
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, easedT);
            
            // 添加抛物线高度
            float heightT = 4f * t * (1f - t); // 抛物线：0 -> 1 -> 0
            currentPos.y += flyHeight * heightT;
            
            transform.position = currentPos;
            
            yield return null;
        }
        
        // 动画完成，执行拾取
        _isFlying = false;
        TryPickup(inventory);
    }
    
    /// <summary>
    /// 停止飞向动画
    /// </summary>
    public void StopFlyAnimation()
    {
        if (_flyCoroutine != null)
        {
            StopCoroutine(_flyCoroutine);
            _flyCoroutine = null;
        }
        _isFlying = false;
    }

    /// <summary>
    /// 重置物品状态（用于对象池复用）
    /// </summary>
    public void Reset()
    {
        itemId = -1;
        quality = 0;
        amount = 1;
        linkedItemData = null;
        _isFlying = false;
        _initialized = false;
        if (_flyCoroutine != null)
        {
            StopCoroutine(_flyCoroutine);
            _flyCoroutine = null;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = fallbackSprite;
            // 重置缩放
            spriteRenderer.transform.localScale = Vector3.one;
        }
        // 重置阴影缩放
        var shadow = transform.Find("Shadow");
        if (shadow != null)
        {
            shadow.localScale = Vector3.one;
        }
    }
    
    /// <summary>
    /// 应用 ItemData 的显示尺寸设置
    /// 用于运行时动态生成的物品
    /// </summary>
    public void ApplyDisplaySize()
    {
        if (linkedItemData == null || !linkedItemData.useCustomDisplaySize) return;
        
        float scale = linkedItemData.GetWorldDisplayScale();
        if (Mathf.Approximately(scale, 1f)) return;
        
        // 应用到 Sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.transform.localScale = Vector3.one * scale;
        }
        
        // 同步阴影缩放
        var shadow = transform.Find("Shadow");
        if (shadow != null)
        {
            // 阴影的基础缩放已经在预制体中设置，这里只需要乘以 displayScale
            shadow.localScale *= scale;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // 延迟执行，避免在 OnValidate 中调用 SendMessage
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            // 不在 OnValidate 中调用 ApplyVisual，避免 SendMessage 错误
        };
    }
#endif
}
