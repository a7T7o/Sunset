using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using FarmGame.Data;
using FarmGame.World;

namespace FarmGame.UI
{
    /// <summary>
    /// 箱子UI面板 - 管理箱子交互界面
    /// 
    /// 🔴 重要：本组件只做数据绑定，不生成/销毁任何槽位！
    /// 槽位已在预制体中预置好（Up_00~Up_XX, Down_00~Down_XX）
    /// 
    /// 结构：
    /// - Up：绑定 ChestInventory（显示箱子格子）
    /// - Down：绑定 InventoryService（显示玩家背包格子）
    /// </summary>
    public class BoxPanelUI : MonoBehaviour
    {
        #region 序列化字段

        [Header("=== 容器区域 ===")]
        [Tooltip("箱子格子容器（Up）")]
        [SerializeField] private Transform upGridParent;

        [Tooltip("背包格子容器（Down）")]
        [SerializeField] private Transform downGridParent;

        [Header("=== 功能按钮 ===")]
        [Tooltip("整理箱子按钮")]
        [SerializeField] private Button btnSortUp;

        [Tooltip("整理背包按钮")]
        [SerializeField] private Button btnSortDown;

        [Tooltip("垃圾桶按钮")]
        [SerializeField] private Button btnTrashCan;

        [Header("=== 调试 ===")]
        [SerializeField] private bool showDebugInfo = false;

        #endregion

        #region 私有字段

        private ChestController _currentChest;
        private List<InventorySlotUI> _chestSlots = new List<InventorySlotUI>();
        private List<InventorySlotUI> _inventorySlots = new List<InventorySlotUI>();
        private InventoryService _inventoryService;
        private EquipmentService _equipmentService;
        private ItemDatabase _database;
        private bool _isOpen = false;
        
        // 🔥 缓存引用（性能优化）
        private InventorySortService _cachedSortService;
        private HeldItemDisplay _cachedHeldDisplay;
        
        // 🔥 日志去重标志（实例级别）
        private bool _hasLoggedBindFailure = false;

        // 当前活跃的 BoxPanelUI 实例（用于互斥管理）
        private static BoxPanelUI _activeInstance;

        #endregion

        #region 属性

        /// <summary>
        /// 当前打开的箱子
        /// </summary>
        public ChestController CurrentChest => _currentChest;

        /// <summary>
        /// 面板是否打开
        /// </summary>
        public bool IsOpen => _isOpen;

        /// <summary>
        /// 当前活跃的 BoxPanelUI 实例
        /// </summary>
        public static BoxPanelUI ActiveInstance => _activeInstance;
        
        /// <summary>
        /// 缓存的 InventorySortService
        /// </summary>
        private InventorySortService CachedSortService
        {
            get
            {
                if (_cachedSortService == null)
                    _cachedSortService = FindFirstObjectByType<InventorySortService>();
                return _cachedSortService;
            }
        }
        
        /// <summary>
        /// 缓存的 HeldItemDisplay
        /// </summary>
        private HeldItemDisplay CachedHeldDisplay
        {
            get
            {
                if (_cachedHeldDisplay == null)
                    _cachedHeldDisplay = FindFirstObjectByType<HeldItemDisplay>();
                return _cachedHeldDisplay;
            }
        }

        #endregion

        #region Unity 生命周期

        private void Awake()
        {
            TryAutoLocate();
            CollectSlots();
        }

        private void Start()
        {
            // 获取服务引用
            _inventoryService = FindFirstObjectByType<InventoryService>();
            _equipmentService = FindFirstObjectByType<EquipmentService>();
            
            if (_inventoryService != null)
            {
                _database = _inventoryService.Database;
            }
            
            // 🔥 修复：确保 _database 不为 null
            if (_database == null)
            {
                Debug.LogError("[BoxPanelUI] Start: _database 为 null！无法初始化箱子 UI");
            }

            // 绑定按钮事件
            BindButtons();
            
            // 🔥 延迟初始化：确保 InventorySlotUI 的 Awake 已执行
            // 不在 Start 中关闭，因为可能是被 Open 调用后才激活的
            if (showDebugInfo)
            {
                Debug.Log($"[BoxPanelUI] Start: _inventoryService={_inventoryService != null}, _equipmentService={_equipmentService != null}, _database={_database != null}");
                Debug.Log($"[BoxPanelUI] Start: 箱子槽位={_chestSlots.Count}, 背包槽位={_inventorySlots.Count}");
            }
        }

        private void OnDestroy()
        {
            if (_activeInstance == this)
            {
                _activeInstance = null;
            }

            // 取消箱子事件订阅
            UnsubscribeFromChest();
        }

        #endregion

        #region 自动定位

        private void TryAutoLocate()
        {
            if (upGridParent == null)
            {
                upGridParent = FindChildByName(transform, "Up");
            }

            if (downGridParent == null)
            {
                downGridParent = FindChildByName(transform, "Down");
            }

            if (btnSortUp == null)
            {
                var t = FindChildByName(transform, "BT_Sort_Up");
                if (t != null) btnSortUp = t.GetComponent<Button>();
            }

            if (btnSortDown == null)
            {
                var t = FindChildByName(transform, "BT_Sort_Down");
                if (t != null) btnSortDown = t.GetComponent<Button>();
            }

            if (btnTrashCan == null)
            {
                var t = FindChildByName(transform, "BT_TrashCan");
                if (t != null) btnTrashCan = t.GetComponent<Button>();
            }
        }

        private Transform FindChildByName(Transform root, string name)
        {
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == name) return t;
            }
            return null;
        }

        #endregion

        #region 槽位收集（只收集，不生成）

        /// <summary>
        /// 收集预制体中已存在的槽位
        /// 🔴 绝对不生成或销毁任何槽位！
        /// 🔥 修正：只收集直接子级，避免 Up 和 Down 互相污染
        /// </summary>
        private void CollectSlots()
        {
            _chestSlots.Clear();
            _inventorySlots.Clear();

            if (upGridParent != null)
            {
                // 🔥 只收集直接子级，不递归
                foreach (Transform child in upGridParent)
                {
                    var slot = child.GetComponent<InventorySlotUI>();
                    if (slot != null)
                    {
                        _chestSlots.Add(slot);
                    }
                }
                if (showDebugInfo)
                    Debug.Log($"[BoxPanelUI] 收集到 {_chestSlots.Count} 个箱子槽位（Up 区域）");
            }

            if (downGridParent != null)
            {
                // 🔥 只收集直接子级，不递归
                foreach (Transform child in downGridParent)
                {
                    var slot = child.GetComponent<InventorySlotUI>();
                    if (slot != null)
                    {
                        _inventorySlots.Add(slot);
                    }
                }
                if (showDebugInfo)
                    Debug.Log($"[BoxPanelUI] 收集到 {_inventorySlots.Count} 个背包槽位（Down 区域）");
            }
        }

        #endregion

        #region 按钮绑定

        private void BindButtons()
        {
            if (btnSortUp != null)
            {
                btnSortUp.onClick.RemoveAllListeners();
                btnSortUp.onClick.AddListener(OnSortUpClicked);
            }

            if (btnSortDown != null)
            {
                btnSortDown.onClick.RemoveAllListeners();
                btnSortDown.onClick.AddListener(OnSortDownClicked);
            }

            if (btnTrashCan != null)
            {
                btnTrashCan.onClick.RemoveAllListeners();
                btnTrashCan.onClick.AddListener(OnTrashCanClicked);
            }
        }

        private void OnSortUpClicked()
        {
            if (_currentChest?.Inventory == null) return;
            
            if (_currentChest.RuntimeInventory != null)
                _currentChest.RuntimeInventory.Sort();
            RefreshChestSlots();  // 🔥 P0-2：排序后刷新 UI
            
            // 🔥 选中状态优化：Sort 后清空选中状态
            ClearUpSelections();
            
            if (showDebugInfo)
                Debug.Log("[BoxPanelUI] 整理箱子完成");
        }

        private void OnSortDownClicked()
        {
            if (_inventoryService == null) return;
            
            // 🔥 使用缓存引用
            var sortService = CachedSortService;
            if (sortService != null)
            {
                sortService.SortInventory();
            }
            else
            {
                // 回退到 InventoryService.Sort()
                _inventoryService.Sort();
            }
            
            RefreshInventorySlots();  // 🔥 P0-2：排序后刷新 UI
            
            // 🔥 选中状态优化：Sort 后清空选中状态
            ClearDownSelections();
            
            if (showDebugInfo)
                Debug.Log("[BoxPanelUI] 整理背包完成");
        }

        private void OnTrashCanClicked()
        {
            // 🔥 修复 2：垃圾桶逻辑
            
            // 情况 1：背包物品在手上（Manager 管辖）
            var manager = InventoryInteractionManager.Instance;
            if (manager != null && manager.IsHolding)
            {
                manager.OnTrashCanClick();
                if (showDebugInfo)
                    Debug.Log("[BoxPanelUI] 垃圾桶 - 通过 Manager 丢弃背包物品");
                return;
            }
            
            // 情况 2：箱子物品在手上（SlotDragContext 管辖）
            if (SlotDragContext.IsDragging)
            {
                DropItemFromContext();
                if (showDebugInfo)
                    Debug.Log("[BoxPanelUI] 垃圾桶 - 通过 SlotDragContext 丢弃箱子物品");
                return;
            }
            
            if (showDebugInfo)
                Debug.Log("[BoxPanelUI] 垃圾桶 - 没有物品在手上");
        }
        
        /// <summary>
        /// 🔥 从 SlotDragContext 丢弃物品
        /// 使用 ItemDropHelper 统一丢弃逻辑
        /// </summary>
        private void DropItemFromContext()
        {
            if (!SlotDragContext.IsDragging) return;
            
            var item = SlotDragContext.DraggedItem;
            if (!item.IsEmpty)
            {
                // 🔥 使用 ItemDropHelper 统一丢弃逻辑
                if (SlotDragContext.DraggedRuntimeItem != null && !SlotDragContext.DraggedRuntimeItem.IsEmpty) ItemDropHelper.DropAtPlayer(SlotDragContext.DraggedRuntimeItem); else ItemDropHelper.DropAtPlayer(item);
            }
            
            // 清空拖拽状态
            SlotDragContext.End();
            HideDragIcon();
        }
        
        /// <summary>
        /// 隐藏拖拽图标
        /// 🔥 使用缓存引用优化性能
        /// </summary>
        private void HideDragIcon()
        {
            var heldDisplay = CachedHeldDisplay;
            if (heldDisplay != null)
            {
                heldDisplay.Hide();
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 打开箱子UI
        /// </summary>
        /// <param name="chest">要打开的箱子</param>
        public void Open(ChestController chest)
        {
            if (chest == null)
            {
                Debug.LogWarning("[BoxPanelUI] 尝试打开空箱子");
                return;
            }

            // 检查箱子是否可以打开
            var result = chest.TryOpen();
            if (result != OpenResult.Success)
            {
                if (showDebugInfo)
                    Debug.Log($"[BoxPanelUI] 无法打开箱子: {result}");
                return;
            }

            _currentChest = chest;
            
            // 🔥 重置日志去重标志
            _hasLoggedBindFailure = false;

            // 🔥 P0-3 修复：防御性获取 _database
            EnsureDatabaseReference(chest);

            // 🔥 关闭其他已打开的 BoxPanelUI（互斥）
            if (_activeInstance != null && _activeInstance != this && _activeInstance._isOpen)
            {
                _activeInstance.Close();
            }
            _activeInstance = this;

            // 显示面板
            gameObject.SetActive(true);
            _isOpen = true;

            // 🔥 订阅箱子库存事件
            SubscribeToChest();

            // 刷新UI
            RefreshUI();

            // 🔥 P1：只输出一行关键日志
            Debug.Log($"[BoxPanelUI] Open: {chest.StorageData?.itemName}, capacity={chest.StorageData?.storageCapacity}");
        }

        /// <summary>
        /// 关闭箱子UI
        /// </summary>
        public void Close()
        {
            if (!_isOpen) return;

            // 🔥 P1+-1：关闭前处理手持物品（物品归位逻辑）
            ReturnHeldItemsBeforeClose();

            // 🔥 取消订阅箱子库存事件
            UnsubscribeFromChest();

            // 关闭箱子的打开状态
            if (_currentChest != null)
            {
                _currentChest.SetOpen(false);
            }

            // 隐藏面板
            gameObject.SetActive(false);
            _isOpen = false;
            _currentChest = null;

            if (_activeInstance == this)
            {
                _activeInstance = null;
            }

            // 🔥 P1：只输出一行关键日志
            if (showDebugInfo)
                Debug.Log("[BoxPanelUI] Close");
        }

        /// <summary>
        /// 刷新UI显示
        /// </summary>
        public void RefreshUI()
        {
            if (_currentChest == null) return;

            RefreshChestSlots();
            RefreshInventorySlots();
        }

        #endregion

        #region 事件订阅

        private void SubscribeToChest()
        {
            var runtimeInventory = _currentChest?.RuntimeInventory;
            if (runtimeInventory == null) return;

            runtimeInventory.OnSlotChanged += OnChestSlotChanged;
            runtimeInventory.OnInventoryChanged += OnChestInventoryChanged;

            // 🔥 修复 1：订阅 InventoryService.OnInventoryChanged
            // 这样背包整理后 BoxPanelUI 才能刷新 Down 区域
            if (_inventoryService != null)
            {
                _inventoryService.OnInventoryChanged += OnInventoryServiceChanged;
            }

            if (showDebugInfo)
                Debug.Log("[BoxPanelUI] 已订阅箱子库存事件和背包事件");
        }

        private void UnsubscribeFromChest()
        {
            var runtimeInventory = _currentChest?.RuntimeInventory;
            if (runtimeInventory != null)
            {
                runtimeInventory.OnSlotChanged -= OnChestSlotChanged;
                runtimeInventory.OnInventoryChanged -= OnChestInventoryChanged;
            }

            // 🔥 修复 1：取消订阅 InventoryService.OnInventoryChanged
            if (_inventoryService != null)
            {
                _inventoryService.OnInventoryChanged -= OnInventoryServiceChanged;
            }

            if (showDebugInfo)
                Debug.Log("[BoxPanelUI] 已取消订阅箱子库存事件和背包事件");
        }

        /// <summary>
        /// 🔥 修复 1：背包变化时刷新 Down 区域
        /// </summary>
        private void OnInventoryServiceChanged()
        {
            if (showDebugInfo)
                Debug.Log("[BoxPanelUI] OnInventoryServiceChanged - 刷新背包槽位");
            RefreshInventorySlots();
        }

        private void OnChestSlotChanged(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < _chestSlots.Count)
            {
                RefreshSingleChestSlot(slotIndex);
            }
        }

        private void OnChestInventoryChanged()
        {
            if (showDebugInfo)
                Debug.Log("[BoxPanelUI] OnChestInventoryChanged - 刷新箱子槽位");
            RefreshChestSlots();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 🔥 选中状态优化：清空箱子区域（Up）的所有选中状态
        /// </summary>
        private void ClearUpSelections()
        {
            // 方案 1：通过 ToggleGroup 清空
            if (upGridParent != null)
            {
                var toggleGroup = upGridParent.GetComponent<ToggleGroup>();
                if (toggleGroup != null)
                {
                    toggleGroup.SetAllTogglesOff();
                    return;
                }
            }
            
            // 方案 2：遍历槽位调用 Deselect
            foreach (var slot in _chestSlots)
            {
                if (slot != null)
                {
                    slot.Deselect();
                }
            }
        }

        /// <summary>
        /// 🔥 选中状态优化：清空背包区域（Down）的所有选中状态
        /// </summary>
        private void ClearDownSelections()
        {
            // 方案 1：通过 ToggleGroup 清空
            if (downGridParent != null)
            {
                var toggleGroup = downGridParent.GetComponent<ToggleGroup>();
                if (toggleGroup != null)
                {
                    toggleGroup.SetAllTogglesOff();
                    return;
                }
            }
            
            // 方案 2：遍历槽位调用 Deselect
            foreach (var slot in _inventorySlots)
            {
                if (slot != null)
                {
                    slot.Deselect();
                }
            }
        }

        /// <summary>
        /// 刷新箱子槽位（只绑定数据，不修改槽位数量）
        /// </summary>
        private void RefreshChestSlots()
        {
            var runtimeInventory = _currentChest?.RuntimeInventory;
            if (runtimeInventory == null) return;

            var inventory = runtimeInventory;
            int capacity = inventory.Capacity;

            // 🔴 只绑定数据，不修改槽位数量
            // 如果槽位数量不匹配，说明预制体配置错误，输出警告
            if (_chestSlots.Count < capacity)
            {
                Debug.LogWarning($"[BoxPanelUI] 预制体槽位数量({_chestSlots.Count})小于箱子容量({capacity})！请检查预制体配置。");
            }

            for (int i = 0; i < _chestSlots.Count; i++)
            {
                var slot = _chestSlots[i];
                if (slot == null) continue;

                if (i < capacity)
                {
                    // 有数据的槽位：显示并绑定
                    slot.gameObject.SetActive(true);
                    BindChestSlotData(slot, i);
                }
                else
                {
                    // 超出容量的槽位：隐藏
                    slot.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 刷新单个箱子槽位
        /// </summary>
        private void RefreshSingleChestSlot(int index)
        {
            if (index < 0 || index >= _chestSlots.Count) return;
            var slot = _chestSlots[index];
            if (slot == null) return;

            BindChestSlotData(slot, index);
        }

        /// <summary>
        /// 绑定箱子槽位数据
        /// 🔥 使用 InventorySlotUI.BindContainer 方法绑定 ChestInventory
        /// </summary>
        private void BindChestSlotData(InventorySlotUI slot, int index)
        {
            var runtimeInventory = _currentChest?.RuntimeInventory;
            if (runtimeInventory == null || _database == null)
            {
                // 🔥 P1：警告去重（实例级别）
                if (!_hasLoggedBindFailure)
                {
                    Debug.LogWarning($"[BoxPanelUI] BindChestSlotData 失败: chest={_currentChest != null}, inventory={runtimeInventory != null}, db={_database != null}");
                    _hasLoggedBindFailure = true;
                }
                return;
            }

            // 🔥 优先绑定到 RuntimeInventory（InventoryV2 优先）
            slot.BindContainer(runtimeInventory, index);
            
            // 🔥 P1：删除逐格日志，只在 showDebugInfo 开启时输出汇总
        }

        /// <summary>
        /// 刷新背包槽位
        /// 🔥 修正 Ⅴ：增强诊断输出，确保绑定可靠
        /// 🔥 修正 C1：Down 显示完整背包（0-35），startIndex 从 12 改为 0
        /// 🔥 修正 C2：与 InventoryPanelUI.BuildUpSlots 保持一致，传入 EquipmentService
        /// </summary>
        private void RefreshInventorySlots()
        {
            if (_inventoryService == null)
            {
                _inventoryService = FindFirstObjectByType<InventoryService>();
                if (_inventoryService != null)
                {
                    _database = _inventoryService.Database;
                }
            }
            
            if (_equipmentService == null)
            {
                _equipmentService = FindFirstObjectByType<EquipmentService>();
            }
            
            if (_inventoryService == null || _database == null)
            {
                if (showDebugInfo)
                    Debug.LogWarning($"[BoxPanelUI] RefreshInventorySlots 失败: InventoryService={_inventoryService != null}, Database={_database != null}");
                return;
            }

            // 背包槽位使用 InventorySlotUI 的标准 Bind 方法
            // 🔥 修正 C1：Down 显示完整背包（0-35），第一行与 Hotbar 同步
            int startIndex = 0;

            for (int i = 0; i < _inventorySlots.Count; i++)
            {
                var slot = _inventorySlots[i];
                if (slot == null)
                {
                    if (showDebugInfo)
                        Debug.LogWarning($"[BoxPanelUI] 背包槽位[{i}] 为 null！");
                    continue;
                }

                int actualIndex = startIndex + i;
                bool isHotbar = actualIndex < InventoryService.HotbarWidth;
                
                // 🔥 修正 C2：与 InventoryPanelUI.BuildUpSlots 保持一致
                slot.Bind(_inventoryService, _equipmentService, _database, actualIndex, isHotbar);
                
                // 🔥 致命修复 1：Bind 后必须调用 Refresh 才能更新 UI 显示
                slot.Refresh();
                slot.RefreshSelection();
            }
        }

        #endregion

        #region Database 防御性获取

        /// <summary>
        /// 🔥 P1+-1：关闭前处理手持物品（物品归位逻辑）
        /// 优先级：原槽位 → 背包空位 → 扔在脚下
        /// </summary>
        private void ReturnHeldItemsBeforeClose()
        {
            // 情况 1：背包物品在手上（Manager 管辖）
            var manager = InventoryInteractionManager.Instance;
            if (manager != null && manager.IsHolding)
            {
                manager.ReturnHeldItemToInventory();
                if (showDebugInfo)
                    Debug.Log("[BoxPanelUI] Close: 通过 Manager 归位背包物品");
            }
            
            // 情况 2：箱子物品在手上（SlotDragContext 管辖）
            if (SlotDragContext.IsDragging)
            {
                ReturnChestItemToSource();
                if (showDebugInfo)
                    Debug.Log("[BoxPanelUI] Close: 归位箱子物品");
            }
            
            // 确保隐藏拖拽图标
            HideDragIcon();
        }
        
        /// <summary>
        /// 🔥 P1+-1：将箱子物品归位
        /// 优先级：原槽位 → 箱子空位 → 背包空位 → 扔在脚下
        /// </summary>
        private void ReturnChestItemToSource()
        {
            if (!SlotDragContext.IsDragging) return;
            
            var item = SlotDragContext.DraggedItem;
            if (item.IsEmpty)
            {
                SlotDragContext.End();
                return;
            }
            
            var sourceContainer = SlotDragContext.SourceContainer;
            int sourceIndex = SlotDragContext.SourceSlotIndex;
            
            // 1. 尝试返回原槽位
            if (sourceContainer != null)
            {
                var srcSlot = sourceContainer.GetSlot(sourceIndex);
                if (srcSlot.IsEmpty)
                {
                    sourceContainer.SetSlot(sourceIndex, item);
                    SlotDragContext.End();
                    if (showDebugInfo)
                        Debug.Log($"[BoxPanelUI] 箱子物品归位：返回原槽位 {sourceIndex}");
                    return;
                }
                
                // 尝试堆叠
                if (srcSlot.CanStackWith(item))
                {
                    int maxStack = sourceContainer.GetMaxStack(item.itemId);
                    int total = srcSlot.amount + item.amount;
                    
                    if (total <= maxStack)
                    {
                        srcSlot.amount = total;
                        sourceContainer.SetSlot(sourceIndex, srcSlot);
                        SlotDragContext.End();
                        if (showDebugInfo)
                            Debug.Log($"[BoxPanelUI] 箱子物品归位：堆叠到原槽位 {sourceIndex}");
                        return;
                    }
                }
            }
            
            // 2. 尝试放入箱子空位
            if (_currentChest?.Inventory != null)
            {
                var chest = _currentChest.Inventory;
                for (int i = 0; i < chest.Capacity; i++)
                {
                    if (chest.GetSlot(i).IsEmpty)
                    {
                        chest.SetSlot(i, item);
                        SlotDragContext.End();
                        if (showDebugInfo)
                            Debug.Log($"[BoxPanelUI] 箱子物品归位：放入箱子空位 {i}");
                        return;
                    }
                }
            }
            
            // 3. 尝试放入背包空位
            if (_inventoryService != null)
            {
                for (int i = 0; i < 36; i++)
                {
                    if (_inventoryService.GetSlot(i).IsEmpty)
                    {
                        _inventoryService.SetSlot(i, item);
                        SlotDragContext.End();
                        if (showDebugInfo)
                            Debug.Log($"[BoxPanelUI] 箱子物品归位：放入背包空位 {i}");
                        return;
                    }
                }
            }
            
            // 4. 都满了，扔在脚下
            if (SlotDragContext.DraggedRuntimeItem != null && !SlotDragContext.DraggedRuntimeItem.IsEmpty) ItemDropHelper.DropAtPlayer(SlotDragContext.DraggedRuntimeItem); else ItemDropHelper.DropAtPlayer(item);
            SlotDragContext.End();
            if (showDebugInfo)
                Debug.Log("[BoxPanelUI] 箱子物品归位：扔在脚下");
        }

        /// <summary>
        /// 🔥 P0-3：防御性获取 _database
        /// 优先级：chest.Inventory.Database → _inventoryService.Database → FindFirstObjectByType
        /// </summary>
        private void EnsureDatabaseReference(ChestController chest)
        {
            // 1. 尝试从箱子库存获取
            if (_database == null && chest?.Inventory?.Database != null)
            {
                _database = chest.Inventory.Database;
                if (showDebugInfo)
                    Debug.Log("[BoxPanelUI] _database 从 chest.Inventory.Database 获取");
            }

            // 2. 尝试从 InventoryService 获取
            if (_database == null)
            {
                if (_inventoryService == null)
                {
                    _inventoryService = FindFirstObjectByType<InventoryService>();
                }
                if (_inventoryService?.Database != null)
                {
                    _database = _inventoryService.Database;
                    if (showDebugInfo)
                        Debug.Log("[BoxPanelUI] _database 从 _inventoryService.Database 获取");
                }
            }

            // 3. 最后尝试直接查找
            if (_database == null)
            {
                _database = FindFirstObjectByType<ItemDatabase>();
                if (_database != null && showDebugInfo)
                    Debug.Log("[BoxPanelUI] _database 从 FindFirstObjectByType 获取");
            }

            // 4. 如果获取成功，同步设置到箱子
            if (_database != null)
            {
                chest?.SetDatabase(_database);
            }
            else
            {
                Debug.LogError("[BoxPanelUI] 无法获取 _database！箱子 UI 将无法正常工作");
            }
        }

        #endregion

        #region 编辑器

#if UNITY_EDITOR
        [ContextMenu("自动定位引用")]
        private void DEBUG_AutoLocate()
        {
            TryAutoLocate();
            CollectSlots();
            Debug.Log($"[BoxPanelUI] upGridParent={upGridParent?.name}, downGridParent={downGridParent?.name}");
            Debug.Log($"[BoxPanelUI] 箱子槽位={_chestSlots.Count}, 背包槽位={_inventorySlots.Count}");
        }
#endif

        #endregion
    }
}
