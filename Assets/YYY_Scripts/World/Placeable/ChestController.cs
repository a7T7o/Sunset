using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FarmGame.Data;
using FarmGame.Data.Core;
using FarmGame.Combat;
using FarmGame.UI;

namespace FarmGame.World
{
    /// <summary>
    /// 箱子控制器 - 管理箱子世界物体的所有交互逻辑
    /// 包括：受击、推动、上锁、解锁、打开、Sprite状态管理
    /// 实现 IResourceNode 接口以与工具攻击系统集成
    /// 实现 IInteractable 接口以支持统一的交互系统
    /// 实现 IPersistentObject 接口以支持存档系统
    /// </summary>
    public class ChestController : MonoBehaviour, IResourceNode, IInteractable, IPersistentObject
    {
        private static readonly List<ChestController> s_activeInstances = new List<ChestController>();

        #region 序列化字段

        [Header("=== 持久化配置 ===")]
        [SerializeField, Tooltip("对象唯一 ID（自动生成，勿手动修改）")]
        private string _persistentId;

        [SerializeField, Tooltip("是否在编辑器中预生成 ID")]
        private bool _preGenerateId = true;

        [Header("=== 数据引用 ===")]
        [Tooltip("关联的 StorageData")]
        [SerializeField] private StorageData storageData;

        [Header("=== Sprite 配置 ===")]
        [Tooltip("未锁关闭状态的 Sprite")]
        [SerializeField] private Sprite spriteUnlockedClosed;

        [Tooltip("未锁打开状态的 Sprite")]
        [SerializeField] private Sprite spriteUnlockedOpen;

        [Tooltip("上锁关闭状态的 Sprite")]
        [SerializeField] private Sprite spriteLockedClosed;

        [Tooltip("上锁打开状态的 Sprite")]
        [SerializeField] private Sprite spriteLockedOpen;

        [Header("=== 交互设置 ===")]
        [Tooltip("玩家到箱子边界的允许开启距离")]
        [SerializeField, Min(0.35f)] private float interactionDistance = 0.9f;

        [Header("=== 来源与归属 ===")]
        [Tooltip("箱子来源（玩家制作/野外生成）")]
        [SerializeField] private ChestOrigin origin = ChestOrigin.PlayerCrafted;

        [Tooltip("是否曾经被上过锁（上过锁的箱子不能再次上锁）")]
        [SerializeField] private bool hasBeenLocked = false;

        [Header("=== 运行时状态 ===")]
        [SerializeField] private int currentHealth;
        [SerializeField] private ChestOwnership ownership = ChestOwnership.Player;
        [SerializeField] private bool isLocked = false;

        [Header("=== 推动设置 ===")]
        [Tooltip("推动距离（单位）")]
        [SerializeField] private float pushDistance = 1f;

        [Tooltip("推动动画总时长")]
        [SerializeField] private float pushDuration = 0.5f;

        [Tooltip("推动跳跃高度")]
        [SerializeField] private float pushJumpHeight = 0.3f;

        [Tooltip("碰撞检测半径")]
        [SerializeField] private float collisionCheckRadius = 0.4f;

        [Header("=== 抖动效果 ===")]
        [Tooltip("抖动幅度")]
        [SerializeField] private float shakeIntensity = 0.05f;

        [Tooltip("抖动持续时间")]
        [SerializeField] private float shakeDuration = 0.15f;

        [Header("=== 调试 ===")]
        [SerializeField] private bool showDebugInfo = false;

        #endregion

        #region 私有字段

        private ChestInventory _inventory;

        /// <summary>
        /// V2 库存（支持 InventoryItem）
        /// </summary>
        private ChestInventoryV2 _inventoryV2;
        private bool _isSyncingInventoryBridge;

        private bool _isPushing = false;
        private bool _isShaking = false;
        private bool _isOpen = false;
        private Collider2D _collider;
        private PolygonCollider2D _polyCollider;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _originalPosition;
        private Bounds _lastNavObstacleBounds;
        private bool _hasLastNavObstacleBounds;

        // 🔥 修正 Ⅲ：底部对齐锚点
        private Vector3 _anchorWorldPos;
        private bool _anchorInitialized = false;

        // 🔥 缓存引用（性能优化）
        private PackagePanelTabsUI _cachedPackagePanel;
        private Canvas _cachedCanvas;

        #endregion

        #region 属性

        public static IReadOnlyList<ChestController> ActiveInstances => s_activeInstances;

        public StorageData StorageData => storageData;
        public int CurrentHealth => currentHealth;
        public ChestOwnership Ownership => ownership;
        public bool IsLocked => isLocked;

        /// <summary>
        /// 箱子库存（新接口，推荐使用）
        /// </summary>
        public ChestInventory Inventory => _inventory;

        /// <summary>
        /// 当前运行时优先使用的箱子容器。
        /// </summary>
        public IItemContainer RuntimeInventory => (IItemContainer)_inventoryV2 ?? _inventory;

        /// <summary>
        /// V2 库存（支持 InventoryItem，用于存档）
        /// </summary>
        public ChestInventoryV2 InventoryV2 => _inventoryV2;

        /// <summary>
        /// 兼容旧接口：获取所有内容物
        /// </summary>
        public ItemStack[] Contents => _inventoryV2?.GetAllSlots() ?? _inventory?.GetAllSlots() ?? System.Array.Empty<ItemStack>();

        public bool IsPushing => _isPushing;
        public bool IsOpen => _isOpen;
        public ChestOrigin Origin => origin;
        public bool HasBeenLocked => hasBeenLocked;

        /// <summary>
        /// 是否为空
        /// `InventoryV2` 已是 authoritative runtime source，旧库存只保留为兼容镜像。
        /// </summary>
        public bool IsEmpty => _inventoryV2?.IsEmpty ?? _inventory?.IsEmpty ?? true;

        #endregion

        #region 数据同步

        /// <summary>
        /// 将 legacy `ChestInventory` 的变更桥接到 authoritative `ChestInventoryV2`。
        /// 旧库存只作为兼容入口保留，bridge 时必须静默写入，避免事件互相反写。
        /// </summary>
        private void SyncInventoryToV2()
        {
            if (_inventory == null || _inventoryV2 == null) return;

            var slots = _inventory.GetAllSlots();
            List<int> changedSlots = null;
            for (int i = 0; i < _inventoryV2.Capacity; i++)
            {
                var stack = i < slots.Length ? slots[i] : ItemStack.Empty;
                if (LegacyStackMatchesRuntimeItem(stack, _inventoryV2.GetItem(i)))
                    continue;

                changedSlots ??= new List<int>();

                if (stack.IsEmpty)
                {
                    _inventoryV2.ClearItemSilently(i);
                }
                else
                {
                    _inventoryV2.SetSlotSilently(i, stack);
                }

                changedSlots.Add(i);
            }

            if (changedSlots == null || changedSlots.Count == 0)
                return;

            foreach (int index in changedSlots)
            {
                _inventoryV2.NotifySlotChanged(index);
            }

            _inventoryV2.NotifyInventoryChanged();

            if (showDebugInfo)
                Debug.Log($"[ChestController] SyncInventoryToV2: 已将 legacy mirror 同步到 authoritative V2，变更槽位数={changedSlots.Count}");
        }

        /// <summary>
        /// 将 authoritative `ChestInventoryV2` 同步回 legacy `ChestInventory` 兼容镜像。
        /// 仅用于旧接口读取，不允许再次触发双向事件回写。
        /// </summary>
        private void SyncV2ToInventory()
        {
            if (_inventory == null || _inventoryV2 == null) return;

            List<int> changedSlots = null;
            int syncCount = Mathf.Min(_inventoryV2.Capacity, _inventory.Capacity);
            for (int i = 0; i < syncCount; i++)
            {
                var item = _inventoryV2.GetItem(i);
                var desiredStack = item == null || item.IsEmpty ? ItemStack.Empty : item.ToItemStack();
                var currentStack = _inventory.GetSlot(i);
                if (ItemStacksEqual(currentStack, desiredStack))
                    continue;

                changedSlots ??= new List<int>();

                if (item == null || item.IsEmpty)
                {
                    _inventory.ClearSlotSilently(i);
                }
                else
                {
                    _inventory.SetSlotSilently(i, desiredStack);
                }

                changedSlots.Add(i);
            }

            for (int i = syncCount; i < _inventory.Capacity; i++)
            {
                var currentStack = _inventory.GetSlot(i);
                if (currentStack.IsEmpty)
                    continue;

                changedSlots ??= new List<int>();
                _inventory.ClearSlotSilently(i);
                changedSlots.Add(i);
            }

            if (changedSlots == null || changedSlots.Count == 0)
                return;

            foreach (int index in changedSlots)
            {
                _inventory.NotifySlotChanged(index);
            }

            _inventory.NotifyInventoryChanged();

            if (showDebugInfo)
                Debug.Log($"[ChestController] SyncV2ToInventory: 已刷新 legacy mirror，变更槽位数={changedSlots.Count}");
        }

        private bool ItemStacksEqual(ItemStack a, ItemStack b)
        {
            if (a.IsEmpty && b.IsEmpty)
                return true;

            return a.itemId == b.itemId &&
                   a.quality == b.quality &&
                   a.amount == b.amount;
        }

        private bool LegacyStackMatchesRuntimeItem(ItemStack legacyStack, InventoryItem runtimeItem)
        {
            if (legacyStack.IsEmpty)
                return runtimeItem == null || runtimeItem.IsEmpty;

            if (runtimeItem == null || runtimeItem.IsEmpty)
                return false;

            return runtimeItem.ItemId == legacyStack.itemId &&
                   runtimeItem.Quality == legacyStack.quality &&
                   runtimeItem.Amount == legacyStack.amount;
        }

        #endregion

        #region IPersistentObject 接口实现

        /// <summary>
        /// 对象唯一标识符
        /// </summary>
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

        /// <summary>
        /// 🔥 P1 任务 9.2：设置持久化 ID（仅供 DynamicObjectFactory 加载时使用）
        /// </summary>
        public void SetPersistentIdForLoad(string guid)
        {
            if (!string.IsNullOrEmpty(guid))
            {
                _persistentId = guid;
            }
        }

        /// <summary>
        /// 对象类型标识
        /// </summary>
        public string ObjectType => "Chest";

        /// <summary>
        /// 是否应该被保存
        /// </summary>
        public bool ShouldSave => gameObject.activeInHierarchy;

        /// <summary>
        /// 保存对象状态
        /// Rule: P0-1 箱子存档 - 保存前同步 _inventory 到 _inventoryV2
        /// 🔥 P1 任务 9：保存 prefabId 用于动态重建
        /// 🔥 3.7.5：使用世界预制体名称（storagePrefab.name）而非 StorageData 名称
        /// </summary>
        public WorldObjectSaveData Save()
        {
            var data = new WorldObjectSaveData
            {
                guid = PersistentId,
                objectType = ObjectType,
                sceneName = gameObject.scene.name,
                isActive = gameObject.activeSelf,
                layer = 1, // TODO: 从父物体获取楼层
                // 🔥 3.7.5：使用世界预制体名称
                prefabId = GetWorldPrefabName()
            };

            // 设置位置
            data.SetPosition(transform.position);
            data.rotationZ = transform.eulerAngles.z;

            // 创建箱子特有数据
            var chestData = new ChestSaveData
            {
                capacity = storageData != null ? storageData.storageCapacity : 20,
                isLocked = isLocked,
                customName = storageData?.itemName
            };

            // 保存库存数据（优先使用 V2）
            if (_inventoryV2 != null)
            {
                chestData.slots = _inventoryV2.ToSaveData();
            }
            else if (_inventory != null)
            {
                // 兼容旧库存
                chestData.slots = new List<InventorySlotSaveData>();
                var slots = _inventory.GetAllSlots();
                for (int i = 0; i < slots.Length; i++)
                {
                    chestData.slots.Add(new InventorySlotSaveData
                    {
                        slotIndex = i,
                        itemId = slots[i].itemId,
                        quality = slots[i].quality,
                        amount = slots[i].amount
                    });
                }
            }

            // 序列化为 JSON
            data.genericData = JsonUtility.ToJson(chestData);

            // 🔴 保存渲染层级参数（Sorting Layer + Order in Layer）
            if (_spriteRenderer != null)
            {
                data.SetSortingLayer(_spriteRenderer);
            }

            if (showDebugInfo)
                Debug.Log($"[ChestController] Save: GUID={PersistentId}, prefabId={data.prefabId}, sortingLayer={data.sortingLayerName}, sortingOrder={data.sortingOrder}");

            return data;
        }

        /// <summary>
        /// 🔥 3.7.5：获取世界预制体名称
        /// 优先使用 StorageData 中配置的世界预制体名称
        /// </summary>
        private string GetWorldPrefabName()
        {
            // 1. 优先使用 StorageData 中配置的世界预制体
            if (storageData != null && storageData.storagePrefab != null)
            {
                return storageData.storagePrefab.name;
            }

            // 2. 回退：使用当前 GameObject 名称（清洗后）
            return CleanGameObjectName(gameObject.name);
        }

        /// <summary>
        /// 🔥 3.7.5：清理 GameObject 名称（去掉 Clone 等后缀）
        /// </summary>
        private string CleanGameObjectName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;

            // 去掉 "(Clone)" 后缀
            if (name.EndsWith("(Clone)"))
                name = name.Substring(0, name.Length - 7).Trim();

            // 去掉 " (1)", " (2)" 等后缀
            name = System.Text.RegularExpressions.Regex.Replace(name, @"\s\(\d+\)$", "");

            return name;
        }

        /// <summary>
        /// 加载对象状态
        /// Rule: P0-1 箱子存档 - 加载后同步 _inventoryV2 到 _inventory
        /// 🔥 修复：确保 _inventory 和 _inventoryV2 在加载前已初始化
        /// 🔥 P0-2 修复：处理空箱子情况，确保 IsEmpty 返回正确值
        /// </summary>
        public void Load(WorldObjectSaveData data)
        {
            if (data == null) return;

            // 恢复位置
            transform.position = data.GetPosition();
            transform.rotation = Quaternion.Euler(0, 0, data.rotationZ);

            // 解析箱子特有数据
            if (!string.IsNullOrEmpty(data.genericData))
            {
                var chestData = JsonUtility.FromJson<ChestSaveData>(data.genericData);
                if (chestData != null)
                {
                    isLocked = chestData.isLocked;

                    // 🔥 修复：确保 _inventory 和 _inventoryV2 已初始化
                    int capacity = chestData.capacity > 0 ? chestData.capacity : (storageData?.storageCapacity ?? 20);

                    // 🔥 锐评010 指令：添加调试日志验证 capacity
                    Debug.Log($"[Chest] Load Capacity: {capacity} (chestData.capacity={chestData.capacity}, storageData?.storageCapacity={storageData?.storageCapacity})");

                    if (_inventory == null)
                    {
                        _inventory = new ChestInventory(capacity);
                        if (showDebugInfo)
                            Debug.Log($"[ChestController] Load: 初始化 _inventory, capacity={capacity}");
                    }
                    if (_inventoryV2 == null)
                    {
                        _inventoryV2 = new ChestInventoryV2(capacity);
                        if (showDebugInfo)
                            Debug.Log($"[ChestController] Load: 初始化 _inventoryV2, capacity={capacity}");
                    }

                    // 恢复库存数据
                    if (chestData.slots != null && chestData.slots.Count > 0)
                    {
                        _inventoryV2.LoadFromSaveData(chestData.slots);
                        // 🔥 P0-1 修复：同步到 _inventory，确保 UI 显示正确
                        SyncV2ToInventory();

                        if (showDebugInfo)
                            Debug.Log($"[ChestController] Load: 恢复了 {chestData.slots.Count} 个槽位数据");
                    }
                    else
                    {
                        // 🔥 P0-2 修复：如果存档中没有槽位数据，说明箱子是空的
                        // 必须清空两个库存系统，避免残留数据
                        for (int i = 0; i < _inventoryV2.Capacity; i++)
                        {
                            _inventoryV2.ClearItemSilently(i);
                        }
                        for (int i = 0; i < _inventory.Capacity; i++)
                        {
                            _inventory.ClearSlotSilently(i);
                        }

                        if (showDebugInfo)
                            Debug.Log($"[ChestController] Load: 存档中无槽位数据，已清空两个库存系统");
                    }
                }
            }

            // 更新视觉状态
            UpdateSprite();

            // 🔴 恢复渲染层级参数（Sorting Layer + Order in Layer）
            if (_spriteRenderer != null)
            {
                data.RestoreSortingLayer(_spriteRenderer);
            }

            // 🔥 P0-2 修复：强制状态检查，验证 IsEmpty 属性返回正确值
            if (showDebugInfo)
            {
                Debug.Log($"[ChestController] Load 完成: GUID={PersistentId}, isLocked={isLocked}, sortingLayer={data.sortingLayerName}, sortingOrder={data.sortingOrder}");
                Debug.Log($"[ChestController] 状态检查: IsEmpty={IsEmpty}, _inventoryV2.IsEmpty={_inventoryV2?.IsEmpty}, _inventory.IsEmpty={_inventory?.IsEmpty}");
            }
        }

        #endregion

        #region IResourceNode 接口实现

        public string ResourceTag => "Chest";
        public bool IsDepleted => false;

        public bool CanAccept(ToolHitContext ctx) => true;

        public void OnHit(ToolHitContext ctx)
        {
            // 始终播放抖动效果
            PlayShakeEffect();

            // 非镐子工具只抖动
            if (ctx.toolType != ToolType.Pickaxe)
            {
                if (showDebugInfo)
                    Debug.Log($"[ChestController] 非镐子工具击中，只抖动: {ctx.toolType}");
                return;
            }

            // 检查是否可以被挖取或移动
            if (!CanBeMinedOrMoved())
            {
                if (showDebugInfo)
                    Debug.Log("[ChestController] 野外上锁箱子不能被挖取或移动");
                return;
            }

            // 有物品：推动
            if (!IsEmpty)
            {
                TryPush(ctx.hitDir);
                return;
            }

            // 空箱子：造成伤害
            int damage = Mathf.Max(1, Mathf.RoundToInt(ctx.baseDamage));
            currentHealth -= damage;

            if (showDebugInfo)
                Debug.Log($"[ChestController] 受到伤害: {damage}, 剩余血量: {currentHealth}");

            if (currentHealth <= 0)
                OnDestroyed();
        }

        public Bounds GetBounds()
        {
            if (_spriteRenderer != null && _spriteRenderer.sprite != null)
                return _spriteRenderer.bounds;
            return new Bounds(transform.position, Vector3.one);
        }

        public Bounds GetColliderBounds()
        {
            if (_collider != null && _collider.enabled)
                return _collider.bounds;
            return GetBounds();
        }

        public Vector3 GetPosition() => transform.position;

        #endregion


        #region Unity 生命周期

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _polyCollider = GetComponent<PolygonCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            // 🔥 修正 Ⅲ：初始化底部对齐锚点
            if (!_anchorInitialized && _spriteRenderer != null)
            {
                _anchorWorldPos = GetCurrentBottomCenterWorld();
                _anchorInitialized = true;
            }
        }

        private void OnEnable()
        {
            RegisterActiveInstance(this);
        }

        private void OnDisable()
        {
            UnregisterActiveInstance(this);
        }

        private void Start()
        {
            Initialize();

            if (ResourceNodeRegistry.Instance != null)
            {
                ResourceNodeRegistry.Instance.Register(this, gameObject.GetInstanceID());
                if (showDebugInfo)
                    Debug.Log($"[ChestController] 已注册到 ResourceNodeRegistry: {gameObject.name}");
            }

            // 🔥 注册到持久化对象注册中心
            if (PersistentObjectRegistry.Instance != null)
            {
                PersistentObjectRegistry.Instance.Register(this);
                if (showDebugInfo)
                    Debug.Log($"[ChestController] 已注册到 PersistentObjectRegistry: GUID={PersistentId}");
            }

            // 🔥 关键修复：箱子放置后通知 NavGrid 刷新
            // 延迟一帧确保碰撞体已完全初始化
            StartCoroutine(RequestNavGridRefreshDelayed());
        }

        /// <summary>
        /// 延迟请求 NavGrid 刷新（确保碰撞体已初始化）
        /// </summary>
        private IEnumerator RequestNavGridRefreshDelayed()
        {
            yield return null; // 等待一帧
            RequestNavGridRefresh();
        }

        /// <summary>
        /// 请求 NavGrid 刷新（供外部调用）
        /// </summary>
        public void RequestNavGridRefresh()
        {
            bool hasRefreshBounds = TryBuildNavRefreshBounds(out Bounds refreshBounds);
            bool requested = hasRefreshBounds && NavGrid2D.TryRequestLocalRefresh(refreshBounds);
            if (!requested)
            {
                NavGrid2D.OnRequestGridRefresh?.Invoke();
            }

            UpdateLastNavObstacleBounds();
            if (showDebugInfo)
                Debug.Log($"[ChestController] 已请求 NavGrid 刷新（局部={hasRefreshBounds}）");
        }

        private void OnDestroy()
        {
            UnregisterActiveInstance(this);

            if (ResourceNodeRegistry.Instance != null)
                ResourceNodeRegistry.Instance.Unregister(gameObject.GetInstanceID());

            // 🔥 从持久化对象注册中心注销
            if (PersistentObjectRegistry.Instance != null)
                PersistentObjectRegistry.Instance.Unregister(this);

            // 🔥 P1 任务 4：取消订阅事件，避免内存泄漏
            if (_inventory != null)
                _inventory.OnInventoryChanged -= OnInventoryChangedHandler;
            if (_inventoryV2 != null)
                _inventoryV2.OnInventoryChanged -= OnInventoryV2ChangedHandler;
        }

        private static void RegisterActiveInstance(ChestController controller)
        {
            if (controller == null || s_activeInstances.Contains(controller))
            {
                return;
            }

            s_activeInstances.Add(controller);
        }

        private static void UnregisterActiveInstance(ChestController controller)
        {
            if (controller == null)
            {
                return;
            }

            s_activeInstances.Remove(controller);
        }

        #endregion

        #region 初始化

        public void Initialize()
        {
            if (storageData != null)
            {
                currentHealth = storageData.maxHealth;
                isLocked = storageData.defaultLocked;

                // 🔥 3.7.6 修复：如果 _inventory 已被 Load() 初始化并填充数据，则不重新创建
                // 问题场景：DynamicObjectFactory 重建箱子时，Load() 先于 Start() 执行
                // 如果这里无条件重新创建，会覆盖 Load() 中恢复的数据
                if (_inventory == null)
                {
                    _inventory = new ChestInventory(storageData.storageCapacity);
                    // 🔥 P1 任务 4：订阅库存变化事件，实时同步到 V2
                    _inventory.OnInventoryChanged += OnInventoryChangedHandler;

                    if (showDebugInfo)
                        Debug.Log($"[ChestController] Initialize: 创建新的 _inventory, capacity={storageData.storageCapacity}");
                }
                else
                {
                    // 🔥 确保事件订阅（Load() 中创建的 _inventory 可能没有订阅事件）
                    _inventory.OnInventoryChanged -= OnInventoryChangedHandler; // 先移除，避免重复订阅
                    _inventory.OnInventoryChanged += OnInventoryChangedHandler;

                    if (showDebugInfo)
                        Debug.Log($"[ChestController] Initialize: _inventory 已存在（来自 Load），跳过重建");
                }

                // 🔥 3.7.6 修复：同样检查 _inventoryV2
                if (_inventoryV2 == null)
                {
                    _inventoryV2 = new ChestInventoryV2(storageData.storageCapacity);

                    if (showDebugInfo)
                        Debug.Log($"[ChestController] Initialize: 创建新的 _inventoryV2, capacity={storageData.storageCapacity}");
                }
                else
                {
                    if (showDebugInfo)
                        Debug.Log($"[ChestController] Initialize: _inventoryV2 已存在（来自 Load），跳过重建");
                }

                _inventoryV2.OnInventoryChanged -= OnInventoryV2ChangedHandler;
                _inventoryV2.OnInventoryChanged += OnInventoryV2ChangedHandler;

                // 🔥 C4：添加调试日志验证每个箱子有独立的 ChestInventory 实例
                if (showDebugInfo)
                    Debug.Log($"[ChestController] 初始化完成: {storageData.itemName}, 血量={currentHealth}, 容量={storageData.storageCapacity}, instanceId={GetInstanceID()}, GUID={PersistentId}");
            }

            // 🔥 修正 Ⅳ：初始化时完整执行 Sprite → Collider → NavGrid 链路
            UpdateSprite();
            UpdateColliderShape();
            // NavGrid 刷新由 Start 中的延迟调用处理
        }

        /// <summary>
        /// 🔥 P1 任务 4：库存变化事件处理器
        /// 当 _inventory 发生变化时，自动同步到 _inventoryV2
        /// </summary>
        private void OnInventoryChangedHandler()
        {
            if (_isSyncingInventoryBridge)
                return;

            _isSyncingInventoryBridge = true;
            try
            {
                SyncInventoryToV2();
            }
            finally
            {
                _isSyncingInventoryBridge = false;
            }

            if (showDebugInfo)
                Debug.Log($"[ChestController] OnInventoryChanged: 已同步到 V2");
        }

        private void OnInventoryV2ChangedHandler()
        {
            if (_isSyncingInventoryBridge)
                return;

            _isSyncingInventoryBridge = true;
            try
            {
                SyncV2ToInventory();
            }
            finally
            {
                _isSyncingInventoryBridge = false;
            }

            if (showDebugInfo)
                Debug.Log($"[ChestController] OnInventoryV2Changed: authoritative V2 已刷新 legacy mirror");
        }

        public void Initialize(StorageData data, ChestOwnership initialOwnership = ChestOwnership.Player)
        {
            storageData = data;
            ownership = initialOwnership;
            Initialize();
        }

        /// <summary>
        /// 初始化箱子（支持设置来源）
        /// </summary>
        public void Initialize(StorageData data, ChestOrigin chestOrigin, ChestOwnership initialOwnership = ChestOwnership.Player, bool initialLocked = false)
        {
            storageData = data;
            origin = chestOrigin;
            ownership = initialOwnership;
            isLocked = initialLocked;
            hasBeenLocked = initialLocked;
            Initialize();
        }

        /// <summary>
        /// 设置物品数据库引用（供 BoxPanelUI 调用）
        /// </summary>
        public void SetDatabase(ItemDatabase database)
        {
            _inventory?.SetDatabase(database);
            _inventoryV2?.SetDatabase(database);
        }

        #endregion

        #region Sprite 管理

        public void SetOpen(bool open)
        {
            _isOpen = open;
            UpdateSpriteForState();

            // 🔥 修正 Ⅳ：状态切换后更新 Collider 和 NavGrid
            UpdateColliderShape();
            RequestNavGridRefresh();

            if (showDebugInfo)
                Debug.Log($"[ChestController] 设置打开状态: {open}, ownership={ownership}, isLocked={isLocked}");
        }

        /// <summary>
        /// 🔥 修正 Ⅲ：根据 ownership、isLocked、_isOpen 决定 Sprite，并保持底部对齐
        /// 玩家箱子：开→上锁打开，关→上锁关闭
        /// 野外箱子（已解锁）：常驻上锁打开
        /// </summary>
        public void UpdateSpriteForState()
        {
            if (_spriteRenderer == null) return;

            Sprite targetSprite = null;

            // 野外箱子且曾经被解锁过：常驻"上锁打开"样式
            if (origin == ChestOrigin.WorldSpawned && hasBeenLocked && !isLocked)
            {
                targetSprite = spriteLockedOpen;
            }
            // 玩家箱子或普通逻辑
            else
            {
                targetSprite = GetCurrentSprite();
            }

            if (targetSprite != null)
            {
                // 🔥 修正：先更新 Sprite，再执行底部对齐
                _spriteRenderer.sprite = targetSprite;
                AlignSpriteBottom();
            }
        }

        public void UpdateSprite()
        {
            UpdateSpriteForState();
        }

        public Sprite GetCurrentSprite()
        {
            if (isLocked)
                return _isOpen ? spriteLockedOpen : spriteLockedClosed;
            else
                return _isOpen ? spriteUnlockedOpen : spriteUnlockedClosed;
        }

        #region 底部对齐（修正 Ⅲ）

        /// <summary>
        /// 获取当前 Sprite 底部中心的世界坐标
        /// </summary>
        private Vector3 GetCurrentBottomCenterWorld()
        {
            if (_spriteRenderer == null || _spriteRenderer.sprite == null)
                return transform.position;

            var bounds = _spriteRenderer.bounds; // 世界空间 bounds
            return new Vector3(bounds.center.x, bounds.min.y, transform.position.z);
        }

        /// <summary>
        /// 底部对齐 - 与 TreeController 保持一致
        /// 修改子物体的 localPosition.y，使 Sprite 底部对齐到父物体位置
        /// </summary>
        private void AlignSpriteBottom()
        {
            if (_spriteRenderer == null || _spriteRenderer.sprite == null) return;

            // 使用与 TreeController 完全一致的逻辑
            Bounds spriteBounds = _spriteRenderer.sprite.bounds;
            float spriteBottomOffset = spriteBounds.min.y;

            Vector3 localPos = transform.localPosition;
            localPos.y = -spriteBottomOffset;
            transform.localPosition = localPos;

            // 更新锚点（用于后续 Sprite 切换时的相对对齐）
            _anchorWorldPos = GetCurrentBottomCenterWorld();
            _anchorInitialized = true;

            if (showDebugInfo)
                Debug.Log($"[ChestController] AlignSpriteBottom: spriteBottomOffset={spriteBottomOffset}, localPos.y={localPos.y}");
        }

        /// <summary>
        /// 应用 Sprite 并保持底部对齐（旧方法，保留兼容）
        /// </summary>
        [System.Obsolete("使用 AlignSpriteBottom() 替代")]
        private void ApplySpriteWithBottomAlign(Sprite newSprite)
        {
            if (_spriteRenderer == null || newSprite == null) return;

            // 应用新 Sprite
            _spriteRenderer.sprite = newSprite;

            // 使用统一的底部对齐方法
            AlignSpriteBottom();
        }

        #endregion

        #region Collider 更新（修正 Ⅳ）

        /// <summary>
        /// 更新 PolygonCollider2D 形状以匹配当前 Sprite
        /// </summary>
        private void UpdateColliderShape()
        {
            if (_polyCollider == null || _spriteRenderer == null || _spriteRenderer.sprite == null)
                return;

            var sprite = _spriteRenderer.sprite;
            int shapeCount = sprite.GetPhysicsShapeCount();

            if (shapeCount == 0)
            {
                _polyCollider.pathCount = 0;
                return;
            }

            _polyCollider.pathCount = shapeCount;

            var physicsShape = new System.Collections.Generic.List<Vector2>();
            for (int i = 0; i < shapeCount; i++)
            {
                physicsShape.Clear();
                sprite.GetPhysicsShape(i, physicsShape);
                _polyCollider.SetPath(i, physicsShape);
            }

            // 🔥 确保物理系统同步
            Physics2D.SyncTransforms();

            if (showDebugInfo)
                Debug.Log($"[ChestController] UpdateColliderShape: shapeCount={shapeCount}");
        }

        private bool TryGetCurrentNavObstacleBounds(out Bounds bounds)
        {
            if (_collider != null && _collider.enabled)
            {
                bounds = _collider.bounds;
                return true;
            }

            bounds = default;
            return false;
        }

        private bool TryBuildNavRefreshBounds(out Bounds bounds)
        {
            bool hasBounds = TryGetCurrentNavObstacleBounds(out bounds);
            if (_hasLastNavObstacleBounds)
            {
                if (hasBounds)
                {
                    bounds.Encapsulate(_lastNavObstacleBounds);
                }
                else
                {
                    bounds = _lastNavObstacleBounds;
                    hasBounds = true;
                }
            }

            return hasBounds;
        }

        private void UpdateLastNavObstacleBounds()
        {
            _hasLastNavObstacleBounds = TryGetCurrentNavObstacleBounds(out _lastNavObstacleBounds);
        }

        #endregion

        #endregion

        #region 抖动效果

        public void PlayShakeEffect()
        {
            if (_isShaking) return;
            StartCoroutine(ShakeCoroutine());
        }

        private IEnumerator ShakeCoroutine()
        {
            _isShaking = true;
            _originalPosition = transform.position;
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / shakeDuration;
                float currentIntensity = shakeIntensity * (1f - t);
                float offsetX = Random.Range(-currentIntensity, currentIntensity);
                float offsetY = Random.Range(-currentIntensity, currentIntensity);
                transform.position = _originalPosition + new Vector3(offsetX, offsetY, 0f);
                yield return null;
            }

            transform.position = _originalPosition;
            _isShaking = false;
        }

        #endregion

        #region 受击处理（旧接口）

        /// <summary>
        /// 尝试推动箱子
        /// </summary>
        private void TryPush(Vector2 direction)
        {
            if (_isPushing) return;

            // 标准化方向
            Vector2 pushDir = direction.normalized;
            if (pushDir.sqrMagnitude < 0.01f) return;

            // 计算目标位置
            Vector3 targetPos = transform.position + (Vector3)(pushDir * pushDistance);

            // 碰撞检测
            var hits = Physics2D.OverlapCircleAll(targetPos, collisionCheckRadius);
            foreach (var hit in hits)
            {
                if (hit.gameObject != gameObject && !hit.isTrigger)
                {
                    if (showDebugInfo)
                        Debug.Log($"[ChestController] 推动被阻挡: {hit.gameObject.name}");
                    return;
                }
            }

            StartCoroutine(PushCoroutine(targetPos));
        }

        private IEnumerator PushCoroutine(Vector3 targetPos)
        {
            _isPushing = true;
            Vector3 startPos = transform.position;
            float elapsed = 0f;

            while (elapsed < pushDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / pushDuration;

                // 水平移动
                Vector3 pos = Vector3.Lerp(startPos, targetPos, t);

                // 添加跳跃效果
                float jumpT = Mathf.Sin(t * Mathf.PI);
                pos.y += jumpT * pushJumpHeight;

                transform.position = pos;
                yield return null;
            }

            transform.position = targetPos;
            _isPushing = false;

            // 🔥 关键修复：推动完成后刷新 NavGrid
            RequestNavGridRefresh();
        }

        #endregion

        #region 锁定系统

        /// <summary>
        /// 判断箱子是否可以被挖取或移动
        /// 野外上锁箱子（无论是否已开锁）不能被挖取或移动
        /// </summary>
        public bool CanBeMinedOrMoved()
        {
            // 野外上锁箱子（hasBeenLocked=true）不能被挖取或移动
            if (origin == ChestOrigin.WorldSpawned && hasBeenLocked)
                return false;
            return true;
        }

        /// <summary>
        /// 玩家尝试上锁（消耗锁道具）
        /// </summary>
        public LockResult TryLockByPlayer()
        {
            // 已经上过锁的箱子不能再次上锁
            if (hasBeenLocked)
            {
                if (showDebugInfo)
                    Debug.Log("[ChestController] 箱子已上过锁，不能再次上锁");
                return LockResult.AlreadyLocked;
            }

            // 野外上锁箱子不能被玩家上锁
            if (origin == ChestOrigin.WorldSpawned && isLocked)
            {
                if (showDebugInfo)
                    Debug.Log("[ChestController] 野外上锁箱子不能被玩家上锁");
                return LockResult.AlreadyLocked;
            }

            isLocked = true;
            hasBeenLocked = true;

            // 野外未锁箱子上锁后变为玩家归属
            if (origin == ChestOrigin.WorldSpawned)
                ownership = ChestOwnership.Player;

            UpdateSprite();

            if (showDebugInfo)
                Debug.Log($"[ChestController] 玩家上锁成功");

            return LockResult.Success;
        }

        /// <summary>
        /// 使用钥匙尝试开锁（概率系统）
        /// </summary>
        /// <param name="keyData">钥匙数据</param>
        /// <returns>开锁结果</returns>
        public UnlockResult TryUnlockWithKey(KeyLockData keyData)
        {
            if (keyData == null)
            {
                if (showDebugInfo)
                    Debug.Log("[ChestController] 钥匙数据为空");
                return UnlockResult.MaterialMismatch;
            }

            // 确保是钥匙而不是锁
            if (keyData.keyLockType != KeyLockType.Key)
            {
                if (showDebugInfo)
                    Debug.Log("[ChestController] 不是钥匙类型");
                return UnlockResult.MaterialMismatch;
            }

            if (!isLocked)
                return UnlockResult.NotLocked;

            // 玩家自己的箱子不需要钥匙
            if (ownership == ChestOwnership.Player)
                return UnlockResult.AlreadyOwned;

            // 计算开锁概率
            float chestChance = storageData != null ? storageData.baseUnlockChance : 0.5f;
            float totalChance = keyData.unlockChance + chestChance;
            bool success = Random.value <= totalChance;

            if (showDebugInfo)
                Debug.Log($"[ChestController] 开锁尝试: 钥匙概率={keyData.unlockChance}, 箱子概率={chestChance}, 总概率={totalChance}, 结果={success}");

            if (success)
            {
                isLocked = false;
                // 野外箱子开锁后保持 World 归属，不能被挖取
                UpdateSprite();
                return UnlockResult.Success;
            }

            // 失败时钥匙会被消耗（由调用方处理）
            return UnlockResult.MaterialMismatch; // 复用枚举表示失败
        }

        /// <summary>
        /// 尝试上锁（旧接口，保留兼容）
        /// </summary>
        public LockResult TryLock(ChestMaterial lockMaterial)
        {
            if (hasBeenLocked) return LockResult.AlreadyLocked;
            if (isLocked) return LockResult.AlreadyLocked;

            if (storageData != null && storageData.chestMaterial != lockMaterial)
                return LockResult.MaterialMismatch;

            isLocked = true;
            hasBeenLocked = true;
            ownership = ChestOwnership.Locked;
            UpdateSprite();

            if (showDebugInfo)
                Debug.Log($"[ChestController] 上锁成功");

            return LockResult.Success;
        }

        /// <summary>
        /// 尝试解锁（旧接口，保留兼容）
        /// </summary>
        public UnlockResult TryUnlock(ChestMaterial keyMaterial)
        {
            if (!isLocked) return UnlockResult.NotLocked;
            if (ownership == ChestOwnership.Player) return UnlockResult.AlreadyOwned;

            if (storageData != null && storageData.chestMaterial != keyMaterial)
                return UnlockResult.MaterialMismatch;

            isLocked = false;
            // 野外箱子开锁后保持 World 归属
            if (origin != ChestOrigin.WorldSpawned)
                ownership = ChestOwnership.Player;
            UpdateSprite();

            if (showDebugInfo)
                Debug.Log($"[ChestController] 解锁成功");

            return UnlockResult.Success;
        }

        /// <summary>
        /// 尝试打开箱子
        /// 🔥 修正：玩家自己的箱子即使上锁也可以直接打开，不需要钥匙
        /// </summary>
        public OpenResult TryOpen()
        {
            // 🔥 玩家自己的箱子：即使上锁也可以直接打开
            if (ownership == ChestOwnership.Player)
            {
                SetOpen(true);
                return OpenResult.Success;
            }

            // 非玩家箱子：检查锁定状态
            if (isLocked) return OpenResult.Locked;

            SetOpen(true);
            return OpenResult.Success;
        }

        /// <summary>
        /// 设置指定槽位的物品（委托给 ChestInventory）
        /// </summary>
        public void SetSlot(int index, ItemStack stack)
        {
            if (_inventoryV2 != null)
            {
                if (stack.IsEmpty)
                {
                    _inventoryV2.ClearSlot(index);
                }
                else
                {
                    _inventoryV2.SetSlot(index, stack);
                }

                return;
            }

            _inventory?.SetSlot(index, stack);
        }

        /// <summary>
        /// 获取指定槽位的物品（委托给 ChestInventory）
        /// </summary>
        public ItemStack GetSlot(int index)
        {
            return _inventoryV2?.GetSlot(index) ?? _inventory?.GetSlot(index) ?? ItemStack.Empty;
        }

        #endregion

        #region IInteractable 接口实现

        /// <summary>
        /// 交互优先级（箱子为 50）
        /// </summary>
        public int InteractionPriority => 50;

        /// <summary>
        /// 交互距离
        /// </summary>
        public float InteractionDistance => Mathf.Max(0.35f, interactionDistance);

        /// <summary>
        /// 是否可以交互
        /// </summary>
        public bool CanInteract(InteractionContext context)
        {
            // 箱子始终可以交互（即使上锁也可以尝试开锁）
            return true;
        }

        /// <summary>
        /// 执行交互 - 核心逻辑从 GameInputManager 移到这里
        /// 🔥 修正：玩家箱子交互时不消耗钥匙
        /// </summary>
        public void OnInteract(InteractionContext context)
        {
            if (context == null) return;

            // 🔥 玩家自己的箱子：直接打开，不处理锁/钥匙逻辑
            if (ownership == ChestOwnership.Player)
            {
                OpenBoxUI();
                return;
            }

            // 检查手持物品类型（仅对非玩家箱子生效）
            if (context.Inventory != null && context.Database != null && context.HeldItemId >= 0)
            {
                var itemData = context.Database.GetItemByID(context.HeldItemId);

                // 检查是否为锁或钥匙
                if (itemData is KeyLockData keyLockData)
                {
                    if (keyLockData.keyLockType == KeyLockType.Lock)
                    {
                        // 尝试上锁
                        var result = TryLock(keyLockData.material);
                        switch (result)
                        {
                            case LockResult.Success:
                                // 消耗锁
                                context.Inventory.RemoveFromSlot(context.HeldSlotIndex, 1);
                                if (showDebugInfo)
                                    Debug.Log($"[ChestController] 上锁成功");
                                return;
                            case LockResult.AlreadyLocked:
                                if (showDebugInfo)
                                    Debug.Log($"[ChestController] 箱子已上锁");
                                // TODO: 显示UI提示
                                return;
                            case LockResult.MaterialMismatch:
                                if (showDebugInfo)
                                    Debug.Log($"[ChestController] 锁与箱子材质不匹配");
                                // TODO: 显示UI提示
                                return;
                        }
                    }
                    else if (keyLockData.keyLockType == KeyLockType.Key)
                    {
                        // 尝试解锁（仅对野外上锁箱子生效）
                        if (isLocked)
                        {
                            var result = TryUnlock(keyLockData.material);
                            switch (result)
                            {
                                case UnlockResult.Success:
                                    // 消耗钥匙
                                    context.Inventory.RemoveFromSlot(context.HeldSlotIndex, 1);
                                    if (showDebugInfo)
                                        Debug.Log($"[ChestController] 解锁成功");
                                    // 解锁后打开箱子
                                    break;
                                case UnlockResult.NotLocked:
                                    if (showDebugInfo)
                                        Debug.Log($"[ChestController] 箱子未上锁");
                                    // 直接打开
                                    break;
                                case UnlockResult.AlreadyOwned:
                                    if (showDebugInfo)
                                        Debug.Log($"[ChestController] 箱子已是玩家所有");
                                    // 直接打开
                                    break;
                                case UnlockResult.MaterialMismatch:
                                    if (showDebugInfo)
                                        Debug.Log($"[ChestController] 钥匙与箱子材质不匹配");
                                    // TODO: 显示UI提示
                                    return;
                            }
                        }
                    }
                }
            }

            // 打开箱子UI - 实例化对应的 UI Prefab
            OpenBoxUI();
        }

        /// <summary>
        /// 打开箱子 UI
        /// 🔥 修正：通过 PackagePanelTabsUI 在 PackagePanel 内部实例化 UI
        /// 🔥 优化：使用缓存引用，避免每次 Find
        /// </summary>
        private void OpenBoxUI()
        {
            // 检查是否已有打开的 BoxPanelUI
            if (BoxPanelUI.ActiveInstance != null && BoxPanelUI.ActiveInstance.IsOpen)
            {
                // 如果是同一个箱子，不重复打开
                if (BoxPanelUI.ActiveInstance.CurrentChest == this)
                {
                    if (showDebugInfo)
                        Debug.Log("[ChestController] 箱子 UI 已打开");
                    return;
                }
                // 关闭之前的 UI
                BoxPanelUI.ActiveInstance.Close();
            }

            // 检查 StorageData 是否配置了 UI Prefab
            if (storageData == null || storageData.boxUiPrefab == null)
            {
                Debug.LogError($"[ChestController] 箱子 {gameObject.name} 缺少 boxUiPrefab 配置！");
                return;
            }

            // 🔥 使用缓存引用（PackagePanelTabsUI 没有单例）
            if (_cachedPackagePanel == null)
                _cachedPackagePanel = FindFirstObjectByType<PackagePanelTabsUI>(FindObjectsInactive.Include);
            var packageTabs = _cachedPackagePanel;

            if (packageTabs != null)
            {
                var boxPanelUI = packageTabs.OpenBoxUI(storageData.boxUiPrefab);
                if (boxPanelUI != null)
                {
                    boxPanelUI.Open(this);
                    if (showDebugInfo)
                        Debug.Log($"[ChestController] 通过 PackagePanelTabsUI 打开箱子 UI: {storageData.boxUiPrefab.name}");
                    return;
                }
            }

            // 🔥 备用方案：直接在 Canvas 下实例化（不推荐）
            if (_cachedCanvas == null)
                _cachedCanvas = FindFirstObjectByType<Canvas>();

            if (_cachedCanvas == null)
            {
                Debug.LogError("[ChestController] 场景中没有 Canvas！");
                return;
            }

            var uiInstance = Instantiate(storageData.boxUiPrefab, _cachedCanvas.transform);
            var boxUI = uiInstance.GetComponent<BoxPanelUI>();

            if (boxUI == null)
            {
                Debug.LogError($"[ChestController] UI Prefab {storageData.boxUiPrefab.name} 缺少 BoxPanelUI 组件！");
                Destroy(uiInstance);
                return;
            }

            boxUI.Open(this);

            if (showDebugInfo)
                Debug.Log($"[ChestController] 直接实例化箱子 UI: {storageData.boxUiPrefab.name}");
        }

        /// <summary>
        /// 获取交互提示文本
        /// </summary>
        public string GetInteractionHint(InteractionContext context)
        {
            if (isLocked && ownership != ChestOwnership.Player)
                return "使用钥匙解锁";
            return "打开箱子";
        }

        #endregion

        #region 受击处理（旧接口 - 兼容）

        public bool OnHit(int damage, ToolType toolType, Vector2 attackerDirection)
        {
            PlayShakeEffect();

            if (toolType != ToolType.Pickaxe)
            {
                if (showDebugInfo)
                    Debug.Log($"[ChestController] 非镐子工具无法对箱子造成伤害: {toolType}");
                return false;
            }

            if (!IsEmpty)
            {
                TryPush(attackerDirection);
                return false;
            }

            currentHealth -= damage;
            if (showDebugInfo)
                Debug.Log($"[ChestController] 受到伤害: {damage}, 剩余血量: {currentHealth}");

            if (currentHealth <= 0)
            {
                OnDestroyed();
                return true;
            }
            return true;
        }

        private void OnDestroyed()
        {
            if (showDebugInfo)
                Debug.Log($"[ChestController] 箱子被销毁，生成掉落物");

            if (storageData != null && WorldSpawnService.Instance != null)
            {
                WorldSpawnService.Instance.SpawnWithAnimation(
                    storageData, 0, 1, transform.position, Vector3.up);
            }

            if (_collider != null)
            {
                _collider.enabled = false;
            }

            RequestNavGridRefresh();

            // 删除整个箱子物体（包括父物体）
            // 箱子结构：Box_1(Clone) 父物体 -> Box 子物体（ChestController 在子物体上）
            // 需要删除父物体才能完全清除箱子
            GameObject objectToDestroy = transform.parent != null ? transform.parent.gameObject : gameObject;
            Destroy(objectToDestroy);
        }

        #endregion

        #region 编辑器支持

#if UNITY_EDITOR
        /// <summary>
        /// 在编辑器中预生成 ID
        /// </summary>
        private void OnValidate()
        {
            if (_preGenerateId && string.IsNullOrEmpty(_persistentId))
            {
                _persistentId = System.Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

        /// <summary>
        /// 重新生成持久化 ID
        /// </summary>
        [ContextMenu("重新生成持久化 ID")]
        private void RegeneratePersistentId()
        {
            _persistentId = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log($"[ChestController] 已重新生成 ID: {_persistentId}");
        }
#endif

        #endregion
    }

}
