using UnityEngine;
using System.Collections;
using FarmGame.Data;
using FarmGame.Data.Core;

public class WorldItemPickup : MonoBehaviour, IPersistentObject
{
    [Header("数据")]
    [Tooltip("物品ID（-1表示未初始化，会尝试从预制体名称解析）")]
    public int itemId = -1;
    [Range(0,4)] public int quality = 0;
    [Min(1)] public int amount = 1;
    
    /// <summary>
    /// 物品ID（公开属性，用于对象池管理）
    /// </summary>
    public int ItemId => itemId;
    
    [Header("持久化配置")]
    [Tooltip("对象唯一 ID（自动生成，勿手动修改）")]
    [SerializeField] private string persistentId;
    
    /// <summary>
    /// 🔥 P2 任务 6：来源资源节点的 GUID
    /// 用于关联掉落物与其来源（石头、树木等）
    /// </summary>
    [SerializeField] private string sourceNodeGuid;
    
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
    private InventoryItem runtimeItem;
    private bool _isFlying = false;
    private Coroutine _flyCoroutine;
    private bool _initialized = false;
    
    // 拾取冷却相关
    private float _pickupCooldownEndTime = 0f;
    private bool _hasLeftPickupRange = false;
    private bool _isDropCooldown = false;  // 是否为丢弃冷却（区别于生成冷却）
    
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
        runtimeItem = ToolRuntimeUtility.CreateRuntimeItem(db, stack.itemId, stack.quality, amount);
        _initialized = true;
        ApplyVisual();
    }

    public void Init(ItemDatabase db, InventoryItem item)
    {
        if (item == null || item.IsEmpty)
        {
            return;
        }

        database = db;
        runtimeItem = item;
        itemId = item.ItemId;
        quality = item.Quality;
        amount = Mathf.Max(1, item.Amount);
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
            runtimeItem = ToolRuntimeUtility.CreateRuntimeItem(data, quality, amount);
            linkedItemData = data;
            _initialized = true;
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            var sp = data.GetBagSprite();
            if (spriteRenderer != null && sp != null) spriteRenderer.sprite = sp;
            
            // ★ 应用显示尺寸（包括旋转、位置、缩放）
            ApplyDisplaySize(data);
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
        
        // 🔥 P0 修复：注册到持久化对象注册表
        // 这样反向修剪才能正确处理掉落物
        RegisterToPersistentRegistry();
    }
    
    private void OnDestroy()
    {
        // 🔥 P0 修复：从持久化对象注册表注销
        UnregisterFromPersistentRegistry();
    }
    
    /// <summary>
    /// 注册到持久化对象注册表
    /// </summary>
    private void RegisterToPersistentRegistry()
    {
        if (PersistentObjectRegistry.Instance == null) return;
        
        // 确保有 GUID
        if (string.IsNullOrEmpty(persistentId))
        {
            persistentId = System.Guid.NewGuid().ToString();
        }
        
        PersistentObjectRegistry.Instance.Register(this);
    }
    
    /// <summary>
    /// 从持久化对象注册表注销
    /// </summary>
    private void UnregisterFromPersistentRegistry()
    {
        if (PersistentObjectRegistry.Instance != null)
        {
            PersistentObjectRegistry.Instance.Unregister(this);
        }
    }

    public void ApplyVisual()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (database == null && WorldSpawnService.Instance != null) database = WorldSpawnService.Instance.Database;
        if (database == null && WorldItemPool.Instance != null) database = WorldItemPool.Instance.Database;
        if (database == null)
        {
            database = AssetLocator.LoadItemDatabase();
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

    public ItemStack GetStack() => runtimeItem != null && !runtimeItem.IsEmpty ? runtimeItem.ToItemStack() : new ItemStack(itemId, quality, amount);

    public InventoryItem GetRuntimeItem() => runtimeItem;

    public void SetRuntimeItem(InventoryItem item, ItemDatabase db = null)
    {
        if (item == null || item.IsEmpty)
        {
            runtimeItem = null;
            return;
        }

        if (db != null)
        {
            database = db;
        }

        runtimeItem = item;
        itemId = item.ItemId;
        quality = item.Quality;
        amount = Mathf.Max(1, item.Amount);
        _initialized = true;
        ApplyVisual();
    }

    public bool TryPickup(InventoryService inventory)
    {
        if (inventory == null) return false;

        bool usesRuntimeState = runtimeItem != null && !runtimeItem.IsEmpty &&
                                (runtimeItem.HasDurability || runtimeItem.HasDynamicProperties);

        if (usesRuntimeState)
        {
            bool success = inventory.AddInventoryItem(runtimeItem);
            if (success)
            {
                DespawnSelf();
                return true;
            }

            return false;
        }

        int rem = inventory.AddItem(itemId, quality, amount);
        if (rem == 0)
        {
            DespawnSelf();
            return true;
        }

        amount = rem;
        if (runtimeItem != null && !runtimeItem.IsEmpty)
        {
            runtimeItem.SetAmount(rem);
        }
        return false;
    }

    private void DespawnSelf()
    {
        var dropAnim = GetComponent<WorldItemDrop>();
        if (dropAnim != null)
        {
            dropAnim.StopAnimation();
        }

        if (WorldItemPool.Instance != null)
        {
            WorldItemPool.Instance.Despawn(this);
        }
        else
        {
            Destroy(gameObject);
        }
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
        runtimeItem = null;
        _isFlying = false;
        _initialized = false;
        _pickupCooldownEndTime = 0f;
        _hasLeftPickupRange = false;
        _isDropCooldown = false;
        if (_flyCoroutine != null)
        {
            StopCoroutine(_flyCoroutine);
            _flyCoroutine = null;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = fallbackSprite;
            // 重置 Sprite 变换
            spriteRenderer.transform.localPosition = Vector3.zero;
            spriteRenderer.transform.localRotation = Quaternion.identity;
            spriteRenderer.transform.localScale = Vector3.one;
        }
        // 重置阴影变换
        var shadow = transform.Find("Shadow");
        if (shadow != null)
        {
            shadow.localPosition = new Vector3(0f, -0.1f, 0f);
            shadow.localRotation = Quaternion.identity;
            shadow.localScale = new Vector3(0.5f, 0.3f, 1f);
        }
        // 重置整体缩放
        transform.localScale = Vector3.one;
        // 重置 Collider
        var collider = GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = 0.3f;
        }
    }
    
    #region 拾取冷却
    
    /// <summary>
    /// 设置生成冷却（资源节点掉落物使用）
    /// </summary>
    /// <param name="duration">冷却时间（秒）</param>
    public void SetSpawnCooldown(float duration)
    {
        _pickupCooldownEndTime = Time.time + duration;
        _isDropCooldown = false;
        _hasLeftPickupRange = false;
    }
    
    /// <summary>
    /// 设置丢弃冷却（玩家丢弃物品使用）
    /// </summary>
    /// <param name="duration">冷却时间（秒）</param>
    public void SetDropCooldown(float duration)
    {
        _pickupCooldownEndTime = Time.time + duration;
        _isDropCooldown = true;
        _hasLeftPickupRange = false;
    }
    
    /// <summary>
    /// 检查是否可以被拾取
    /// </summary>
    public bool CanBePickedUp()
    {
        // 如果是丢弃冷却，满足任一条件即可拾取：
        // 1. 冷却时间结束
        // 2. 玩家离开过拾取范围后重新进入
        if (_isDropCooldown)
        {
            if (_hasLeftPickupRange) return true;
            if (Time.time >= _pickupCooldownEndTime) return true;
            return false;
        }
        
        // 生成冷却只检查时间
        return Time.time >= _pickupCooldownEndTime;
    }
    
    /// <summary>
    /// 玩家离开拾取范围时调用
    /// </summary>
    public void OnPlayerExitRange()
    {
        if (_isDropCooldown && Time.time < _pickupCooldownEndTime)
        {
            _hasLeftPickupRange = true;
        }
    }
    
    /// <summary>
    /// 玩家进入拾取范围时调用
    /// </summary>
    public void OnPlayerEnterRange()
    {
        // 如果已经离开过范围，现在重新进入，可以拾取
        // 这个方法主要用于触发检测，实际判断在 CanBePickedUp 中
    }
    
    #endregion
    
    /// <summary>
    /// 应用 ItemData 的显示尺寸设置
    /// 用于运行时动态生成的物品
    /// </summary>
    public void ApplyDisplaySize()
    {
        ApplyDisplaySize(linkedItemData);
    }
    
    /// <summary>
    /// 应用指定 ItemData 的显示尺寸设置
    /// </summary>
    public void ApplyDisplaySize(ItemData itemData)
    {
        if (itemData == null) return;
        
        // 获取 Sprite 信息
        Sprite itemSprite = itemData.GetBagSprite();
        if (itemSprite == null) return;
        
        // 获取显示尺寸缩放比例
        float displayScale = itemData.GetWorldDisplayScale();
        
        // 计算 Sprite 在世界单位中的尺寸（应用显示尺寸缩放）
        float spriteWidth = (itemSprite.rect.width / itemSprite.pixelsPerUnit) * displayScale;
        float spriteHeight = (itemSprite.rect.height / itemSprite.pixelsPerUnit) * displayScale;
        
        // 世界物品旋转角度（与 WorldPrefabGeneratorTool 保持一致）
        const float SPRITE_ROTATION_Z = 45f;
        const float SHADOW_BOTTOM_OFFSET = 0.02f;
        const float WORLD_ITEM_SCALE = 0.75f;
        
        // 计算旋转后的边界框
        float rotRad = SPRITE_ROTATION_Z * Mathf.Deg2Rad;
        float cos = Mathf.Abs(Mathf.Cos(rotRad));
        float sin = Mathf.Abs(Mathf.Sin(rotRad));
        float rotatedWidth = spriteWidth * cos + spriteHeight * sin;
        float rotatedHeight = spriteWidth * sin + spriteHeight * cos;
        
        // 计算旋转后物体底部到中心的距离
        float bottomY = -rotatedHeight * 0.5f;
        
        // 应用到 Sprite
        if (spriteRenderer != null)
        {
            // Sprite 位置：底部略高于阴影中心
            float spriteY = -bottomY + SHADOW_BOTTOM_OFFSET;
            spriteRenderer.transform.localPosition = new Vector3(0f, spriteY, 0f);
            spriteRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, SPRITE_ROTATION_Z);
            spriteRenderer.transform.localScale = Vector3.one * displayScale;
        }
        
        // 同步阴影缩放和位置
        var shadow = transform.Find("Shadow");
        if (shadow != null)
        {
            shadow.localPosition = Vector3.zero;
            shadow.localRotation = Quaternion.identity;
            
            // 阴影大小（已经包含了 displayScale 的影响）
            float shadowWidth = rotatedWidth * 0.8f;
            float shadowHeight = shadowWidth * 0.5f;
            
            // 获取阴影 Sprite 的原始尺寸
            var shadowSr = shadow.GetComponent<SpriteRenderer>();
            if (shadowSr != null && shadowSr.sprite != null)
            {
                float shadowSpriteWidth = shadowSr.sprite.rect.width / shadowSr.sprite.pixelsPerUnit;
                float shadowSpriteHeight = shadowSr.sprite.rect.height / shadowSr.sprite.pixelsPerUnit;
                
                float scaleX = shadowWidth / shadowSpriteWidth;
                float scaleY = shadowHeight / shadowSpriteHeight;
                shadow.localScale = new Vector3(scaleX, scaleY, 1f);
            }
            else
            {
                shadow.localScale = new Vector3(shadowWidth, shadowHeight, 1f);
            }
        }
        
        // 更新 Collider 大小
        var collider = GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = Mathf.Max(rotatedWidth, rotatedHeight) * 0.4f;
        }
        
        // 应用整体缩放
        transform.localScale = Vector3.one * WORLD_ITEM_SCALE;
        
        Debug.Log($"[WorldItemPickup] 最终: 整体缩放={WORLD_ITEM_SCALE}, Collider半径={Mathf.Max(rotatedWidth, rotatedHeight) * 0.4f:F3}");
    }
    
    #region IPersistentObject 实现
    
    /// <summary>
    /// 对象唯一标识符（GUID）
    /// </summary>
    public string PersistentId
    {
        get
        {
            // 延迟生成 GUID
            if (string.IsNullOrEmpty(persistentId))
            {
                persistentId = System.Guid.NewGuid().ToString();
            }
            return persistentId;
        }
    }
    
    /// <summary>
    /// 对象类型标识
    /// </summary>
    public string ObjectType => "Drop";
    
    /// <summary>
    /// 是否应该被保存
    /// </summary>
    public bool ShouldSave => gameObject.activeInHierarchy && itemId >= 0 && amount > 0;
    
    /// <summary>
    /// 保存对象状态
    /// </summary>
    public WorldObjectSaveData Save()
    {
        var data = new WorldObjectSaveData
        {
            guid = PersistentId,
            objectType = ObjectType,
            sceneName = gameObject.scene.name,
            isActive = gameObject.activeSelf
        };
        
        // 保存位置
        data.SetPosition(transform.position);
        
        // 保存掉落物特有数据（使用 DropDataDTO + genericData）
        // 🛡️ 封印一：DropDataDTO 必须有 [Serializable] 特性
        var dropData = new DropDataDTO
        {
            itemId = this.itemId,
            quality = this.quality,
            amount = this.amount,
            sourceNodeGuid = this.sourceNodeGuid  // 🔥 P2 任务 6：保存来源 GUID
        };
        data.genericData = JsonUtility.ToJson(dropData);
        
        // 🔴 保存渲染层级参数（Sorting Layer + Order in Layer）
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            data.SetSortingLayer(spriteRenderer);
        }
        
        return data;
    }
    
    /// <summary>
    /// 加载对象状态
    /// </summary>
    public void Load(WorldObjectSaveData data)
    {
        if (data == null || string.IsNullOrEmpty(data.genericData)) return;
        
        // 从 genericData 反序列化掉落物数据
        var dropData = JsonUtility.FromJson<DropDataDTO>(data.genericData);
        if (dropData == null) return;
        
        // 恢复掉落物数据
        itemId = dropData.itemId;
        quality = dropData.quality;
        amount = dropData.amount;
        sourceNodeGuid = dropData.sourceNodeGuid;  // 🔥 P2 任务 6：恢复来源 GUID
        
        // 刷新视觉
        ApplyVisual();
        
        // 🔴 恢复渲染层级参数（Sorting Layer + Order in Layer）
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            data.RestoreSortingLayer(spriteRenderer);
        }
        
        Debug.Log($"[WorldItemPickup] 已从存档恢复: itemId={itemId}, quality={quality}, amount={amount}, sortingLayer={data.sortingLayerName}, sortingOrder={data.sortingOrder}");
    }
    
    /// <summary>
    /// 为存档加载设置 PersistentId（仅供 DynamicObjectFactory 调用）
    /// </summary>
    public void SetPersistentIdForLoad(string guid)
    {
        if (string.IsNullOrEmpty(guid))
        {
            Debug.LogWarning("[WorldItemPickup] SetPersistentIdForLoad: guid 为空");
            return;
        }
        
        persistentId = guid;
    }
    
    /// <summary>
    /// 🔥 P2 任务 6：来源资源节点的 GUID（只读属性）
    /// </summary>
    public string SourceNodeGuid => sourceNodeGuid;
    
    /// <summary>
    /// 🔥 P2 任务 6：设置来源资源节点的 GUID
    /// 由资源节点（石头、树木等）在生成掉落物时调用
    /// </summary>
    public void SetSourceNodeGuid(string guid)
    {
        sourceNodeGuid = guid;
    }
    
    #endregion

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
