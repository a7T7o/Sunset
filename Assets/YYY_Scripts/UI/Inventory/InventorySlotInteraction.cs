using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FarmGame.Data;
using FarmGame.Data.Core;
using FarmGame.UI;

/// <summary>
/// 背包槽位交互组件 - 方案 D（跨容器拖拽完整支持）
/// 职责：实现 Unity 原生事件接口，根据容器类型分发到不同的处理逻辑
/// 
/// 修复说明（2026-01-18 v5 - P0-2）：
/// 1. 使用 SlotDragContext 统一管理跨容器拖拽状态
/// 2. 四象限拖拽完整支持：Up→Up / Up→Down / Down→Up / Down→Down
/// 3. 箱子槽位拖拽：直接操作 ChestInventory
/// 4. 背包槽位拖拽：通过 SlotDragContext
/// 5. 跨容器拖拽：交换或堆叠
/// </summary>
public class InventorySlotInteraction : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    private enum ContainerDropOutcome
    {
        ClearHeld,
        KeepHeld
    }

    private InventorySlotUI inventorySlotUI;
    private EquipmentSlotUI equipmentSlotUI;
    private bool isEquip;
    private bool isPointerHovering;
    
    private bool isDragging = false;
    private float pressTime;
    private Vector2 pressPosition;
    
    #region 🔥 箱子槽位连续操作状态
    
    /// <summary>
    /// 箱子槽位：Ctrl 长按协程
    /// </summary>
    private Coroutine _chestCtrlCoroutine;
    
    /// <summary>
    /// 箱子槽位：Ctrl 长按拿取速率（次/秒）
    /// </summary>
    private const float CHEST_CTRL_PICKUP_RATE = 3.5f;
    
    #endregion
    
    #region 缓存引用（性能优化）
    
    private InventoryService _cachedInventoryService;
    private EquipmentService _cachedEquipmentService;
    private BoxPanelUI _cachedBoxPanel;
    private PackagePanelTabsUI _cachedPackagePanel;
    
    private InventoryService CachedInventoryService
    {
        get
        {
            if (_cachedInventoryService == null)
                _cachedInventoryService = Object.FindFirstObjectByType<InventoryService>();
            return _cachedInventoryService;
        }
    }
    
    private EquipmentService CachedEquipmentService
    {
        get
        {
            if (_cachedEquipmentService == null)
                _cachedEquipmentService = Object.FindFirstObjectByType<EquipmentService>();
            return _cachedEquipmentService;
        }
    }
    
    private BoxPanelUI CachedBoxPanel
    {
        get
        {
            // BoxPanelUI 可能被销毁重建，需要检查
            if (_cachedBoxPanel == null || !_cachedBoxPanel.gameObject)
                _cachedBoxPanel = Object.FindFirstObjectByType<BoxPanelUI>();
            return _cachedBoxPanel;
        }
    }
    
    private PackagePanelTabsUI CachedPackagePanel
    {
        get
        {
            // PackagePanelTabsUI 没有单例，直接使用缓存
            if (_cachedPackagePanel == null)
                _cachedPackagePanel = Object.FindFirstObjectByType<PackagePanelTabsUI>();
            return _cachedPackagePanel;
        }
    }
    
    #endregion
    
    private int SlotIndex
    {
        get
        {
            if (isEquip && equipmentSlotUI != null)
                return equipmentSlotUI.Index;
            if (!isEquip && inventorySlotUI != null)
                return inventorySlotUI.Index;
            return -1;
        }
    }
    
    private IItemContainer CurrentContainer
    {
        get
        {
            if (!isEquip && inventorySlotUI != null)
                return inventorySlotUI.Container;
            return null;
        }
    }

    private ItemStack ResolveCurrentStack()
    {
        if (isEquip && equipmentSlotUI != null)
        {
            return equipmentSlotUI.GetCurrentStack();
        }

        if (CurrentContainer != null && SlotIndex >= 0)
        {
            return CurrentContainer.GetSlot(SlotIndex);
        }

        return ItemStack.Empty;
    }

    private ItemData ResolveCurrentItemData(ItemStack stack)
    {
        if (stack.IsEmpty)
        {
            return null;
        }

        if (isEquip && equipmentSlotUI != null)
        {
            return equipmentSlotUI.Database != null ? equipmentSlotUI.Database.GetItemByID(stack.itemId) : null;
        }

        return CurrentContainer?.Database != null ? CurrentContainer.Database.GetItemByID(stack.itemId) : null;
    }

    private InventoryItem ResolveCurrentRuntimeItem()
    {
        if (isEquip && equipmentSlotUI != null)
        {
            return equipmentSlotUI.GetCurrentRuntimeItem();
        }

        if (CurrentContainer is InventoryService inventoryService)
        {
            return inventoryService.GetInventoryItem(SlotIndex);
        }

        if (CurrentContainer is ChestInventoryV2 chestInventoryV2)
        {
            return chestInventoryV2.GetItem(SlotIndex);
        }

        return null;
    }

    private void TryShowTooltip()
    {
        if (ShouldSuppressTooltipHover())
        {
            return;
        }

        var tooltip = ItemTooltip.Instance;
        if (tooltip == null)
        {
            return;
        }

        var stack = ResolveCurrentStack();
        if (stack.IsEmpty)
        {
            tooltip.Hide();
            return;
        }

        var itemData = ResolveCurrentItemData(stack);
        if (itemData == null)
        {
            tooltip.Hide();
            return;
        }

        tooltip.Show(itemData, stack, ResolveCurrentRuntimeItem(), stack.amount, transform);
    }

    private static bool ShouldSuppressTooltipHover()
    {
        if (SlotDragContext.IsDragging)
        {
            return true;
        }

        var interactionManager = InventoryInteractionManager.Instance;
        if (interactionManager != null && interactionManager.IsHolding)
        {
            return true;
        }

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            return true;
        }

        return Input.GetKey(KeyCode.LeftShift) ||
               Input.GetKey(KeyCode.RightShift) ||
               Input.GetKey(KeyCode.LeftControl) ||
               Input.GetKey(KeyCode.RightControl);
    }

    private InventoryItem ResolveContainerRuntimeItem(IItemContainer container, int slotIndex)
    {
        if (container is InventoryService inventoryService)
        {
            return inventoryService.GetInventoryItem(slotIndex);
        }

        if (container is ChestInventoryV2 chestInventoryV2)
        {
            return chestInventoryV2.GetItem(slotIndex);
        }

        return null;
    }

    private void SetContainerSlot(IItemContainer container, int slotIndex, ItemStack stack, InventoryItem runtimeItem = null)
    {
        if (container is InventoryService inventoryService && runtimeItem != null && !runtimeItem.IsEmpty)
        {
            inventoryService.SetInventoryItem(slotIndex, runtimeItem);
            return;
        }

        if (container is ChestInventoryV2 chestInventoryV2 && runtimeItem != null && !runtimeItem.IsEmpty)
        {
            chestInventoryV2.SetItem(slotIndex, runtimeItem);
            return;
        }

        container.SetSlot(slotIndex, stack);
    }

    private static InventoryItem CreateRuntimeItemForAmount(InventoryItem runtimeItem, int amount)
    {
        if (runtimeItem == null || runtimeItem.IsEmpty || amount <= 0)
        {
            return null;
        }

        if (runtimeItem.Amount == amount)
        {
            return runtimeItem;
        }

        var clone = runtimeItem.Clone();
        clone.SetAmount(amount);
        return clone;
    }

    private static ItemStack CreateStackWithAmount(ItemStack template, int amount)
    {
        return new ItemStack(template.itemId, template.quality, amount);
    }

    private static bool CanStackByBackpackRules(ItemStack targetSlot, ItemStack draggedItem)
    {
        return targetSlot.CanStackWith(draggedItem);
    }

    private void SetContainerSlotPreservingRuntime(IItemContainer container, int slotIndex, ItemStack stack, InventoryItem runtimeItem = null)
    {
        SetContainerSlot(container, slotIndex, stack, CreateRuntimeItemForAmount(runtimeItem, stack.amount));
    }

    private void UpdateContainerStackAmountPreservingRuntime(
        IItemContainer container,
        int slotIndex,
        ItemStack currentStack,
        InventoryItem currentRuntimeItem,
        int newAmount)
    {
        if (newAmount <= 0)
        {
            if (container is InventoryService inventoryService)
            {
                inventoryService.ClearSlot(slotIndex);
                return;
            }

            if (container is ChestInventoryV2 chestInventoryV2)
            {
                chestInventoryV2.ClearSlot(slotIndex);
                return;
            }

            container.ClearSlot(slotIndex);
            return;
        }

        ItemStack updatedStack = new ItemStack(currentStack.itemId, currentStack.quality, newAmount);
        SetContainerSlotPreservingRuntime(container, slotIndex, updatedStack, currentRuntimeItem);
    }
    
    private bool IsChestSlot => CurrentContainer is ChestInventory || CurrentContainer is ChestInventoryV2;
    private bool IsInventorySlot => CurrentContainer is InventoryService;

    private bool TryRejectAutomatedFarmToolInventoryMove()
    {
        if (!IsInventorySlot)
        {
            return false;
        }

        var inputManager = GameInputManager.Instance;
        if (inputManager == null || !inputManager.TryRejectActiveFarmToolInventoryMove(SlotIndex, isEquip))
        {
            return false;
        }

        return true;
    }

    private bool TryRejectProtectedHeldMutation(
        IItemContainer sourceContainer,
        int sourceIndex,
        bool sourceIsEquipSlot,
        IItemContainer targetContainer,
        int targetIndex,
        bool targetIsEquipSlot)
    {
        var inputManager = GameInputManager.Instance;
        if (inputManager == null)
        {
            return false;
        }

        bool sourceIsInventorySlot = !sourceIsEquipSlot && sourceContainer is InventoryService;
        bool targetIsInventorySlot = !targetIsEquipSlot && targetContainer is InventoryService;

        return inputManager.TryRejectProtectedHeldInventoryMutation(
            sourceIndex,
            sourceIsInventorySlot,
            targetIndex,
            targetIsInventorySlot);
    }
    
    public void Bind(InventorySlotUI slot, bool isEquipmentSlot)
    {
        inventorySlotUI = slot;
        equipmentSlotUI = null;
        isEquip = isEquipmentSlot;
    }
    
    public void Bind(EquipmentSlotUI slot, bool isEquipmentSlot)
    {
        equipmentSlotUI = slot;
        inventorySlotUI = null;
        isEquip = isEquipmentSlot;
    }

    
    #region Unity 原生事件接口
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        ItemTooltip.Instance?.Hide();

        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        
        pressTime = Time.time;
        pressPosition = eventData.position;
        isDragging = false;
        
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        
        // 🔥 修复 1：检测是否有 SlotDragContext 或 Manager Held 状态
        // 如果有，则处理放置逻辑
        if (SlotDragContext.IsDragging)
        {
            HandleSlotDragContextClick();
            return;
        }
        
        var manager = InventoryInteractionManager.Instance;
        if (manager != null && manager.IsHolding)
        {
            HandleManagerHeldClick();
            return;
        }
        
        // 🔥 修复 2：箱子槽位处理修饰键
        if ((shift || ctrl) && TryRejectAutomatedFarmToolInventoryMove())
        {
            return;
        }

        if (IsChestSlot)
        {
            SelectTargetSlot();
            if (shift || ctrl)
            {
                HandleChestSlotModifierClick(shift, ctrl);
            }
            return;
        }
        
        // 装备槽位或背包槽位：使用 InventoryInteractionManager
        if (isEquip || IsInventorySlot)
        {
            if (!shift && !ctrl && (inventorySlotUI != null || equipmentSlotUI != null))
            {
                SelectTargetSlot();
            }

            if (manager != null)
            {
                manager.OnSlotPointerDown(SlotIndex, isEquip, inventorySlotUI);
            }
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        // PointerUp 不做任何事
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerHovering = true;
        inventorySlotUI?.SetHovered(true);
        equipmentSlotUI?.SetHovered(true);
        TryShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerHovering = false;
        inventorySlotUI?.SetHovered(false);
        equipmentSlotUI?.SetHovered(false);
        ItemTooltip.Instance?.Hide();
    }

    void Update()
    {
        if (!isPointerHovering || ShouldSuppressTooltipHover())
        {
            return;
        }

        TryShowTooltip();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        ItemTooltip.Instance?.Hide();
        
        // 🔥 修复：如果已经在 Held 状态（通过 Shift/Ctrl 拿起），不要开始拖拽
        // 与背包区域行为保持一致：Held 状态下移动鼠标不会触发拖拽
        if (SlotDragContext.IsDragging)
        {
            return;
        }

        if (IsInventorySlot && TryRejectAutomatedFarmToolInventoryMove())
        {
            return;
        }
        
        float holdTime = Time.time - pressTime;
        float moveDistance = Vector2.Distance(eventData.position, pressPosition);
        
        if (holdTime < 0.15f && moveDistance < 5f) return;

        if (TryRejectAutomatedFarmToolInventoryMove())
        {
            return;
        }
        
        int index = SlotIndex;
        var container = CurrentContainer;
        
        // 箱子槽位拖拽
        if (IsChestSlot)
        {
            var chest = container;
            if (chest == null) return;
            
            var item = chest.GetSlot(index);
            if (item.IsEmpty) return;
            
            isDragging = true;
            SlotDragContext.Begin(chest, index, item, inventorySlotUI, ResolveContainerRuntimeItem(chest, index));  // 🔥 传入源槽位 UI
            chest.ClearSlot(index);
            ShowDragIcon(item, eventData.position);
            return;
        }
        
        // 装备槽位拖拽：使用 InventoryInteractionManager
        if (isEquip)
        {
            var manager = InventoryInteractionManager.Instance;
            if (manager != null && !manager.IsHolding)
            {
                isDragging = true;
                manager.OnSlotBeginDrag(index, isEquip, eventData, inventorySlotUI);
            }
            return;
        }
        
        // 背包槽位拖拽
        if (IsInventorySlot)
        {
            var inventory = container as InventoryService;
            if (inventory == null) return;
            
            var item = inventory.GetSlot(index);
            if (item.IsEmpty) return;
            
            var manager = InventoryInteractionManager.Instance;
            if (manager != null && manager.IsHolding) return;
            
            if (manager != null)
            {
                isDragging = true;
                manager.OnSlotBeginDrag(index, isEquip, eventData, inventorySlotUI);
            }
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            return;
        }

        InventoryInteractionManager.Instance?.SyncHeldIconToScreenPosition(eventData.position);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;
        
        if (SlotDragContext.IsDragging)
        {
            // 🔥 修复：检测垃圾桶和面板外
            if (IsOverTrashCan(eventData.position))
            {
                DropItemFromContext();
                return;
            }
            
            if (!IsInsidePanel(eventData.position))
            {
                DropItemFromContext();
                return;
            }
            
            // 没有放到有效目标，返回原位
            SlotDragContext.Cancel();
            HideDragIcon();
            ResetActiveChestHeldState();
            return;
        }
        
        var manager = InventoryInteractionManager.Instance;
        if (manager != null)
        {
            manager.OnSlotEndDrag(eventData);
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        int targetIndex = SlotIndex;
        var targetContainer = CurrentContainer;
        
        if (SlotDragContext.IsDragging)
        {
            if (HandleSlotDragContextDrop(targetIndex, targetContainer, allowKeepHeld: false) == ContainerDropOutcome.ClearHeld)
            {
                HideDragIcon();
                SlotDragContext.End();
                ResetActiveChestHeldState();
            }
            return;
        }
        
        var manager = InventoryInteractionManager.Instance;
        if (manager != null && manager.IsHolding)
        {
            // 🔥 修复：支持 Manager Held 放置到箱子槽位
            if (IsChestSlot)
            {
                HandleManagerHeldToChest(targetIndex);
                return;
            }
            manager.OnSlotDrop(targetIndex, isEquip, inventorySlotUI);
        }
    }
    
    #endregion

    
    #region SlotDragContext Drop 处理
    
    private ContainerDropOutcome HandleSlotDragContextDrop(int targetIndex, IItemContainer targetContainer, bool allowKeepHeld)
    {
        var sourceContainer = SlotDragContext.SourceContainer;
        int sourceIndex = SlotDragContext.SourceSlotIndex;
        var draggedItem = SlotDragContext.DraggedItem;
        bool targetIsEquipmentSlot = targetContainer == null && isEquip;

        if (TryRejectProtectedHeldMutation(sourceContainer, sourceIndex, false, targetContainer, targetIndex, targetIsEquipmentSlot))
        {
            SlotDragContext.Cancel();
            return ContainerDropOutcome.ClearHeld;
        }
        
        // 🔥 P0 修复：处理装备槽位（targetContainer == null && isEquip == true）
        if (targetContainer == null && isEquip)
        {
            HandleDropToEquipmentSlot(sourceContainer, sourceIndex, targetIndex, draggedItem);
            return ContainerDropOutcome.ClearHeld;
        }
        
        // 🔥 P0 修复：targetContainer 为 null 但不是装备槽位，取消操作
        if (targetContainer == null)
        {
            SlotDragContext.Cancel();
            return ContainerDropOutcome.ClearHeld;
        }
        
        bool sourceIsChest = sourceContainer is ChestInventory || sourceContainer is ChestInventoryV2;
        bool targetIsChest = targetContainer is ChestInventory || targetContainer is ChestInventoryV2;
        
        // Up → Up（箱子内拖拽）
        if (sourceIsChest && targetIsChest)
        {
            if (ReferenceEquals(sourceContainer, targetContainer))
            {
                return HandleSameContainerDrop(sourceContainer, sourceIndex, targetIndex, draggedItem, allowKeepHeld);
            }

            SlotDragContext.Cancel();
            return ContainerDropOutcome.ClearHeld;
        }
        
        // Up → Down（箱子到背包）
        if (sourceIsChest && !targetIsChest)
        {
            var chest = sourceContainer;
            var inventory = targetContainer as InventoryService;
            if (chest != null && inventory != null)
            {
                return HandleChestToInventoryDrop(chest, sourceIndex, inventory, targetIndex, draggedItem, allowKeepHeld);
            }

            SlotDragContext.Cancel();
            return ContainerDropOutcome.ClearHeld;
        }
        
        // Down → Up（背包到箱子）
        if (!sourceIsChest && targetIsChest)
        {
            var inventory = sourceContainer as InventoryService;
            var chest = targetContainer;
            if (inventory != null && chest != null)
            {
                return HandleInventoryToChestDrop(inventory, sourceIndex, chest, targetIndex, draggedItem, allowKeepHeld);
            }

            SlotDragContext.Cancel();
            return ContainerDropOutcome.ClearHeld;
        }
        
        // Down → Down（背包内拖拽）
        if (!sourceIsChest && !targetIsChest)
        {
            var inventory = sourceContainer as InventoryService;
            if (inventory != null && inventory == targetContainer as InventoryService)
            {
                return HandleSameContainerDrop(inventory, sourceIndex, targetIndex, draggedItem, allowKeepHeld);
            }
        }

        SlotDragContext.Cancel();
        return ContainerDropOutcome.ClearHeld;
    }
    
    private ContainerDropOutcome HandleSameContainerDrop(IItemContainer container, int sourceIndex, int targetIndex, ItemStack draggedItem, bool allowKeepHeld)
    {
        var draggedRuntimeItem = CreateRuntimeItemForAmount(SlotDragContext.DraggedRuntimeItem, draggedItem.amount);

        if (sourceIndex == targetIndex)
        {
            ReturnHeldToSourceContainer(container, sourceIndex, draggedItem, draggedRuntimeItem);
            SelectSourceSlot();
            return ContainerDropOutcome.ClearHeld;
        }
        
        var targetSlot = container.GetSlot(targetIndex);
        var targetRuntimeItem = CreateRuntimeItemForAmount(ResolveContainerRuntimeItem(container, targetIndex), targetSlot.amount);
        
        if (targetSlot.IsEmpty)
        {
            SetContainerSlotPreservingRuntime(container, targetIndex, draggedItem, draggedRuntimeItem);
            SelectTargetSlot();
            return ContainerDropOutcome.ClearHeld;
        }
        
        if (CanStackByBackpackRules(targetSlot, draggedItem))
        {
            int maxStack = container.GetMaxStack(draggedItem.itemId);
            int acceptedAmount = Mathf.Min(draggedItem.amount, Mathf.Max(0, maxStack - targetSlot.amount));
            
            if (acceptedAmount > 0)
            {
                UpdateContainerStackAmountPreservingRuntime(
                    container,
                    targetIndex,
                    targetSlot,
                    targetRuntimeItem,
                    targetSlot.amount + acceptedAmount);
            }

            int remainingAmount = draggedItem.amount - acceptedAmount;
            if (remainingAmount > 0)
            {
                ItemStack remainingItem = CreateStackWithAmount(draggedItem, remainingAmount);
                InventoryItem remainingRuntimeItem = CreateRuntimeItemForAmount(draggedRuntimeItem, remainingAmount);

                if (allowKeepHeld)
                {
                    SlotDragContext.UpdateDraggedItem(remainingItem);
                    SlotDragContext.UpdateDraggedRuntimeItem(remainingRuntimeItem);
                    ShowDragIcon(remainingItem, Input.mousePosition);
                    SelectTargetSlot();
                    return ContainerDropOutcome.KeepHeld;
                }

                ReturnHeldToSourceContainer(container, sourceIndex, remainingItem, remainingRuntimeItem);
            }
            SelectTargetSlot();
            return ContainerDropOutcome.ClearHeld;
        }
        
        var sourceSlot = container.GetSlot(sourceIndex);
        
        if (sourceSlot.IsEmpty)
        {
            SetContainerSlotPreservingRuntime(container, targetIndex, draggedItem, draggedRuntimeItem);
            SetContainerSlotPreservingRuntime(container, sourceIndex, targetSlot, targetRuntimeItem);
            SelectTargetSlot();
            return ContainerDropOutcome.ClearHeld;
        }

        ReturnHeldToSourceContainer(container, sourceIndex, draggedItem, draggedRuntimeItem);
        SelectSourceSlot();
        return ContainerDropOutcome.ClearHeld;
    }
    
    private ContainerDropOutcome HandleChestToInventoryDrop(IItemContainer chest, int chestIndex, InventoryService inventory, int invIndex, ItemStack draggedItem, bool allowKeepHeld)
    {
        var invSlot = inventory.GetSlot(invIndex);
        var draggedRuntimeItem = CreateRuntimeItemForAmount(SlotDragContext.DraggedRuntimeItem, draggedItem.amount);
        var inventoryRuntimeItem = CreateRuntimeItemForAmount(inventory.GetInventoryItem(invIndex), invSlot.amount);
        
        if (invSlot.IsEmpty)
        {
            SetContainerSlotPreservingRuntime(inventory, invIndex, draggedItem, draggedRuntimeItem);
            DeselectSourceSlot();
            SelectTargetSlot();
            return ContainerDropOutcome.ClearHeld;
        }
        
        if (CanStackByBackpackRules(invSlot, draggedItem))
        {
            int maxStack = inventory.GetMaxStack(draggedItem.itemId);
            int acceptedAmount = Mathf.Min(draggedItem.amount, Mathf.Max(0, maxStack - invSlot.amount));
            
            if (acceptedAmount > 0)
            {
                UpdateContainerStackAmountPreservingRuntime(
                    inventory,
                    invIndex,
                    invSlot,
                    inventoryRuntimeItem,
                    invSlot.amount + acceptedAmount);
            }

            int remainingAmount = draggedItem.amount - acceptedAmount;
            if (remainingAmount > 0)
            {
                ItemStack remainingItem = CreateStackWithAmount(draggedItem, remainingAmount);
                InventoryItem remainingRuntimeItem = CreateRuntimeItemForAmount(draggedRuntimeItem, remainingAmount);

                if (allowKeepHeld)
                {
                    SlotDragContext.UpdateDraggedItem(remainingItem);
                    SlotDragContext.UpdateDraggedRuntimeItem(remainingRuntimeItem);
                    ShowDragIcon(remainingItem, Input.mousePosition);
                    DeselectSourceSlot();
                    SelectTargetSlot();
                    return ContainerDropOutcome.KeepHeld;
                }

                ReturnHeldToSourceContainer(chest, chestIndex, remainingItem, remainingRuntimeItem);
            }
            DeselectSourceSlot();
            SelectTargetSlot();
            return ContainerDropOutcome.ClearHeld;
        }
        
        var sourceSlot = chest.GetSlot(chestIndex);
        
        if (sourceSlot.IsEmpty)
        {
            SetContainerSlotPreservingRuntime(inventory, invIndex, draggedItem, draggedRuntimeItem);
            SetContainerSlotPreservingRuntime(chest, chestIndex, invSlot, inventoryRuntimeItem);
            DeselectSourceSlot();
            SelectTargetSlot();
            return ContainerDropOutcome.ClearHeld;
        }

        ReturnHeldToSourceContainer(chest, chestIndex, draggedItem, draggedRuntimeItem);
        SelectSourceSlot();
        return ContainerDropOutcome.ClearHeld;
    }
    
    private ContainerDropOutcome HandleInventoryToChestDrop(InventoryService inventory, int invIndex, IItemContainer chest, int chestIndex, ItemStack draggedItem, bool allowKeepHeld)
    {
        var chestSlot = chest.GetSlot(chestIndex);
        var draggedRuntimeItem = CreateRuntimeItemForAmount(SlotDragContext.DraggedRuntimeItem, draggedItem.amount);
        var chestRuntimeItem = CreateRuntimeItemForAmount(ResolveContainerRuntimeItem(chest, chestIndex), chestSlot.amount);
        
        if (chestSlot.IsEmpty)
        {
            SetContainerSlotPreservingRuntime(chest, chestIndex, draggedItem, draggedRuntimeItem);
            DeselectSourceSlot();
            SelectTargetSlot();
            return ContainerDropOutcome.ClearHeld;
        }
        
        if (CanStackByBackpackRules(chestSlot, draggedItem))
        {
            int maxStack = chest.GetMaxStack(draggedItem.itemId);
            int acceptedAmount = Mathf.Min(draggedItem.amount, Mathf.Max(0, maxStack - chestSlot.amount));
            
            if (acceptedAmount > 0)
            {
                UpdateContainerStackAmountPreservingRuntime(
                    chest,
                    chestIndex,
                    chestSlot,
                    chestRuntimeItem,
                    chestSlot.amount + acceptedAmount);
            }

            int remainingAmount = draggedItem.amount - acceptedAmount;
            if (remainingAmount > 0)
            {
                ItemStack remainingItem = CreateStackWithAmount(draggedItem, remainingAmount);
                InventoryItem remainingRuntimeItem = CreateRuntimeItemForAmount(draggedRuntimeItem, remainingAmount);

                if (allowKeepHeld)
                {
                    SlotDragContext.UpdateDraggedItem(remainingItem);
                    SlotDragContext.UpdateDraggedRuntimeItem(remainingRuntimeItem);
                    ShowDragIcon(remainingItem, Input.mousePosition);
                    DeselectSourceSlot();
                    SelectTargetSlot();
                    return ContainerDropOutcome.KeepHeld;
                }

                ReturnHeldToSourceContainer(inventory, invIndex, remainingItem, remainingRuntimeItem);
            }
            DeselectSourceSlot();
            SelectTargetSlot();
            return ContainerDropOutcome.ClearHeld;
        }
        
        var sourceSlot = inventory.GetSlot(invIndex);
        
        if (sourceSlot.IsEmpty)
        {
            SetContainerSlotPreservingRuntime(chest, chestIndex, draggedItem, draggedRuntimeItem);
            SetContainerSlotPreservingRuntime(inventory, invIndex, chestSlot, chestRuntimeItem);
            DeselectSourceSlot();
            SelectTargetSlot();
            return ContainerDropOutcome.ClearHeld;
        }

        ReturnHeldToSourceContainer(inventory, invIndex, draggedItem, draggedRuntimeItem);
        SelectSourceSlot();
        return ContainerDropOutcome.ClearHeld;
    }
    
    /// <summary>
    /// 🔥 P0 修复：处理拖拽到装备槽位
    /// 核心原则：验证失败必须回滚，绝不吞噬物品
    /// </summary>
    private void HandleDropToEquipmentSlot(IItemContainer sourceContainer, int sourceIndex, int targetEquipIndex, ItemStack draggedItem)
    {
        var equipService = CachedEquipmentService;
        var invService = CachedInventoryService;
        var draggedRuntimeItem = SlotDragContext.DraggedRuntimeItem;
        
        if (equipService == null || invService == null || invService.Database == null)
        {
            // 服务不可用，回滚到源槽位
            Debug.LogWarning("[InventorySlotInteraction] 装备服务不可用，回滚物品");
            SlotDragContext.Cancel();
            return;
        }
        
        var itemData = invService.Database.GetItemByID(draggedItem.itemId);
        
        // 🔥 核心验证：检查物品是否可以装备到该槽位
        if (!equipService.CanEquipAt(targetEquipIndex, itemData))
        {
            // 验证失败，回滚到源槽位
            Debug.Log($"[InventorySlotInteraction] 物品 {itemData?.itemName} 无法装备到槽位 {targetEquipIndex}，回滚");
            SlotDragContext.Cancel();
            return;
        }
        
        // 验证通过，执行装备操作
        var currentEquip = equipService.GetEquip(targetEquipIndex);
        var currentEquipRuntimeItem = equipService.GetEquipItem(targetEquipIndex);
        
        // 设置新装备
        bool success = draggedRuntimeItem != null && !draggedRuntimeItem.IsEmpty
            ? equipService.SetEquipItem(targetEquipIndex, draggedRuntimeItem)
            : equipService.SetEquip(targetEquipIndex, draggedItem);
        if (!success)
        {
            // 设置失败，回滚
            Debug.LogWarning("[InventorySlotInteraction] SetEquip 失败，回滚物品");
            SlotDragContext.Cancel();
            return;
        }
        
        // 如果原装备槽位有物品，需要放回源位置
        if (!currentEquip.IsEmpty)
        {
            // 尝试放回源槽位
            if (sourceContainer is IItemContainer chest && sourceContainer is not InventoryService)
            {
                var sourceSlot = chest.GetSlot(sourceIndex);
                if (sourceSlot.IsEmpty)
                {
                    if (currentEquipRuntimeItem != null && !currentEquipRuntimeItem.IsEmpty)
                        SetContainerSlot(chest, sourceIndex, currentEquip, currentEquipRuntimeItem);
                    else
                        chest.SetSlot(sourceIndex, currentEquip);
                }
                else
                {
                    // 源槽位非空，尝试找空位
                    bool placed = false;
                    for (int i = 0; i < chest.Capacity; i++)
                    {
                        if (chest.GetSlot(i).IsEmpty)
                        {
                            SetContainerSlot(chest, i, currentEquip, currentEquipRuntimeItem);
                            placed = true;
                            break;
                        }
                    }
                    if (!placed)
                    {
                        // 箱子满了，放到背包
                        int remaining = invService.AddItem(currentEquip.itemId, currentEquip.quality, currentEquip.amount);
                        if (remaining > 0)
                        {
                            // 背包也满了，扔在脚下
                            if (currentEquipRuntimeItem != null && !currentEquipRuntimeItem.IsEmpty)
                                FarmGame.UI.ItemDropHelper.DropAtPlayer(currentEquipRuntimeItem);
                            else
                                FarmGame.UI.ItemDropHelper.DropAtPlayer(new ItemStack { itemId = currentEquip.itemId, quality = currentEquip.quality, amount = remaining });
                        }
                    }
                }
            }
            else if (sourceContainer is InventoryService inv)
            {
                var sourceSlot = inv.GetSlot(sourceIndex);
                if (sourceSlot.IsEmpty)
                {
                    SetContainerSlot(inv, sourceIndex, currentEquip, currentEquipRuntimeItem);
                }
                else
                {
                    // 源槽位非空，尝试找空位
                    bool added = currentEquipRuntimeItem != null && !currentEquipRuntimeItem.IsEmpty
                        ? inv.AddInventoryItem(currentEquipRuntimeItem)
                        : inv.AddItem(currentEquip.itemId, currentEquip.quality, currentEquip.amount) == 0;
                    if (!added)
                    {
                        // 背包满了，扔在脚下
                        if (currentEquipRuntimeItem != null && !currentEquipRuntimeItem.IsEmpty)
                            FarmGame.UI.ItemDropHelper.DropAtPlayer(currentEquipRuntimeItem);
                        else
                            FarmGame.UI.ItemDropHelper.DropAtPlayer(currentEquip);
                    }
                }
            }
        }
        
        Debug.Log($"[InventorySlotInteraction] 成功装备 {itemData?.itemName} 到槽位 {targetEquipIndex}");
    }
    
    #endregion

    
    #region 拖拽图标
    
    private void ShowDragIcon(ItemStack item, Vector2? initialScreenPosition = null)
    {
        // 🔥 P0-3：使用统一入口
        var manager = InventoryInteractionManager.Instance;
        if (manager == null) return;
        
        // 🔥 使用缓存引用
        var invService = CachedInventoryService;
        if (invService == null || invService.Database == null) return;
        
        var itemData = invService.Database.GetItemByID(item.itemId);
        if (itemData != null)
        {
            manager.ShowHeldIcon(item.itemId, item.amount, itemData.GetBagSprite(), initialScreenPosition);
        }
    }
    
    private void HideDragIcon()
    {
        // 🔥 P0-3：使用统一入口
        var manager = InventoryInteractionManager.Instance;
        manager?.HideHeldIcon();
    }
    
    #endregion
    
    #region 🔥 修复：新增辅助方法
    
    /// <summary>
    /// 处理箱子槽位的修饰键点击（Shift/Ctrl 拿取）
    /// </summary>
    private void HandleChestSlotModifierClick(bool shift, bool ctrl)
    {
        if (!IsChestSlot) return;
        var chest = CurrentContainer;
        if (chest == null) return;
        
        int index = SlotIndex;
        var slot = chest.GetSlot(index);
        if (slot.IsEmpty) return;
        var runtimeItem = ResolveContainerRuntimeItem(chest, index);
        
        ItemStack pickupItem;
        
        if (shift)
        {
            // Shift：二分拿取（向上取整给手上）
            int handAmount = (slot.amount + 1) / 2;
            int sourceAmount = slot.amount - handAmount;
            
            pickupItem = new ItemStack { itemId = slot.itemId, quality = slot.quality, amount = handAmount };
            
            UpdateContainerStackAmountPreservingRuntime(
                chest,
                index,
                slot,
                runtimeItem,
                sourceAmount);
            
            // 🔥 记录状态：通过 Shift 拿起
        }
        else // ctrl
        {
            // Ctrl：单个拿取
            pickupItem = new ItemStack { itemId = slot.itemId, quality = slot.quality, amount = 1 };
            
            UpdateContainerStackAmountPreservingRuntime(
                chest,
                index,
                slot,
                runtimeItem,
                slot.amount - 1);
            
            // 🔥 记录状态：通过 Ctrl 拿起
            // 🔥 启动长按协程
            if (_chestCtrlCoroutine != null)
                StopCoroutine(_chestCtrlCoroutine);
            _chestCtrlCoroutine = StartCoroutine(ContinueChestCtrlPickup(chest, index, pickupItem.itemId, pickupItem.quality));
        }
        
        // 使用 SlotDragContext 管理 Held 状态
        SlotDragContext.Begin(
            chest,
            index,
            pickupItem,
            inventorySlotUI,
            slot.amount == pickupItem.amount ? runtimeItem : null,
            shift ? SlotDragContext.ModifierHoldMode.Shift : SlotDragContext.ModifierHoldMode.Ctrl,
            this);
        ShowDragIcon(pickupItem, Input.mousePosition);
    }
    
    /// <summary>
    /// 🔥 箱子槽位：Ctrl 长按连续拿取协程
    /// </summary>
    private System.Collections.IEnumerator ContinueChestCtrlPickup(IItemContainer chest, int sourceIndex, int itemId, int quality)
    {
        float interval = 1f / CHEST_CTRL_PICKUP_RATE;
        
        while (true)
        {
            yield return new WaitForSeconds(interval);
            
            // 检查按键状态
            bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool mouse = Input.GetMouseButton(0);
            if (!ctrl || !mouse) break;
            
            // 检查 SlotDragContext 状态
            if (!SlotDragContext.IsDragging || !SlotDragContext.IsOwnedBy(this) || !SlotDragContext.IsHeldByCtrl) break;
            
            // 从源槽位继续拿取
            var src = chest.GetSlot(sourceIndex);
            var srcRuntimeItem = ResolveContainerRuntimeItem(chest, sourceIndex);
            if (src.IsEmpty || src.itemId != itemId) break;
            
            // 增加手上物品数量
            var draggedItem = SlotDragContext.DraggedItem;
            draggedItem.amount++;
            
            // 🔥 使用 UpdateDraggedItem 更新，避免互斥检查
            SlotDragContext.UpdateDraggedItem(draggedItem);
            
            // 更新源槽位
            UpdateContainerStackAmountPreservingRuntime(
                chest,
                sourceIndex,
                src,
                srcRuntimeItem,
                src.amount - 1);
            if (src.amount <= 1)
            {
                ShowDragIcon(draggedItem, Input.mousePosition);
                break;  // 源槽位空了，停止
            }
            
            ShowDragIcon(draggedItem, Input.mousePosition);
        }
        
        _chestCtrlCoroutine = null;
    }
    
    /// <summary>
    /// 处理 SlotDragContext 状态下的点击（放置到当前槽位）
    /// </summary>
    private void HandleSlotDragContextClick()
    {
        // 🔥 停止 Ctrl 长按协程
        if (_chestCtrlCoroutine != null)
        {
            StopCoroutine(_chestCtrlCoroutine);
            _chestCtrlCoroutine = null;
        }
        
        int targetIndex = SlotIndex;
        var targetContainer = CurrentContainer;
        
        if (targetContainer == null)
        {
            ResetActiveChestHeldState();
            SlotDragContext.Cancel();
            HideDragIcon();
            return;
        }
        
        // 🔥 检查是否是 Shift 连续二分场景
        // 条件：通过 Shift 拿起 + 点击源槽位 + 按住 Shift
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (SlotDragContext.IsOwnedBy(this) &&
            SlotDragContext.IsHeldByShift &&
            shift &&
            targetIndex == SlotDragContext.SourceSlotIndex && 
            targetContainer == SlotDragContext.SourceContainer)
        {
            ContinueChestShiftSplit();
            return;
        }
        
        // 正常放置逻辑
        if (HandleSlotDragContextDrop(targetIndex, targetContainer, allowKeepHeld: true) == ContainerDropOutcome.ClearHeld)
        {
            HideDragIcon();
            SlotDragContext.End();
            ResetActiveChestHeldState();
        }
    }
    
    /// <summary>
    /// 🔥 箱子槽位：Shift 连续二分
    /// </summary>
    private void ContinueChestShiftSplit()
    {
        var draggedItem = SlotDragContext.DraggedItem;
        
        // 手上只有 1 个时，不执行二分
        if (draggedItem.amount <= 1) return;
        
        // 向下取整返回，向上取整保留在手上
        int returnAmount = draggedItem.amount / 2;
        int handAmount = draggedItem.amount - returnAmount;
        
        // 更新手上物品
        draggedItem.amount = handAmount;
        
        // 更新源槽位
        var sourceContainer = SlotDragContext.SourceContainer;
        int sourceIndex = SlotDragContext.SourceSlotIndex;
        var src = sourceContainer.GetSlot(sourceIndex);
        var srcRuntimeItem = ResolveContainerRuntimeItem(sourceContainer, sourceIndex);
        
        UpdateContainerStackAmountPreservingRuntime(
            sourceContainer,
            sourceIndex,
            src,
            srcRuntimeItem,
            src.amount + returnAmount);
        
        // 🔥 使用 UpdateDraggedItem 更新，避免互斥检查
        SlotDragContext.UpdateDraggedItem(draggedItem);
        
        ShowDragIcon(draggedItem, Input.mousePosition);
    }
    
    /// <summary>
    /// 🔥 重置箱子槽位 Held 状态
    /// </summary>
    private void ResetChestHeldState()
    {
        if (_chestCtrlCoroutine != null)
        {
            StopCoroutine(_chestCtrlCoroutine);
            _chestCtrlCoroutine = null;
        }
        SlotDragContext.ClearOwner(this);
    }

    public static void ResetActiveChestHeldState()
    {
        if (SlotDragContext.ActiveOwner == null)
        {
            return;
        }

        SlotDragContext.ActiveOwner.ResetChestHeldState();
    }
    
    /// <summary>
    /// 处理 Manager Held 状态下的点击（放置到当前槽位）
    /// </summary>
    private void HandleManagerHeldClick()
    {
        var manager = InventoryInteractionManager.Instance;
        if (manager == null || !manager.IsHolding) return;
        
        // 如果是箱子槽位，处理放置
        if (IsChestSlot)
        {
            HandleManagerHeldToChest(SlotIndex);
            return;
        }
        
        // 背包槽位，使用 Manager 的标准逻辑
        manager.OnSlotPointerDown(SlotIndex, isEquip, inventorySlotUI);
    }
    
    /// <summary>
    /// 处理 Manager Held 物品放置到箱子槽位
    /// </summary>
    private void HandleManagerHeldToChest(int targetIndex)
    {
        var manager = InventoryInteractionManager.Instance;
        if (manager == null || !manager.IsHolding) return;
        
        if (!IsChestSlot) return;
        var chest = CurrentContainer;
        if (chest == null) return;
        
        var heldItem = manager.GetHeldItem();
        var heldRuntimeItem = CreateRuntimeItemForAmount(manager.GetHeldRuntimeItem(), heldItem.amount);
        if (heldItem.IsEmpty) return;
        
        var targetSlot = chest.GetSlot(targetIndex);
        var chestRuntimeItem = CreateRuntimeItemForAmount(ResolveContainerRuntimeItem(chest, targetIndex), targetSlot.amount);
        
        // 目标为空：直接放置
        if (targetSlot.IsEmpty)
        {
            SetContainerSlotPreservingRuntime(chest, targetIndex, heldItem, heldRuntimeItem);
            manager.ClearHeldSourceSelectionVisual();
            manager.ClearHeldState();
            SelectTargetSlot();
            return;
        }
        
        // 相同物品：堆叠
        if (CanStackByBackpackRules(targetSlot, heldItem))
        {
            int maxStack = chest.GetMaxStack(heldItem.itemId);
            int acceptedAmount = Mathf.Min(heldItem.amount, Mathf.Max(0, maxStack - targetSlot.amount));
            
            if (acceptedAmount > 0)
            {
                UpdateContainerStackAmountPreservingRuntime(
                    chest,
                    targetIndex,
                    targetSlot,
                    chestRuntimeItem,
                    targetSlot.amount + acceptedAmount);
            }

            int remainingAmount = heldItem.amount - acceptedAmount;
            if (remainingAmount > 0)
            {
                ItemStack remainingItem = CreateStackWithAmount(heldItem, remainingAmount);
                InventoryItem remainingRuntimeItem = CreateRuntimeItemForAmount(heldRuntimeItem, remainingAmount);
                manager.ReplaceHeldItem(remainingItem, remainingRuntimeItem);
            }
            else
            {
                manager.ClearHeldSourceSelectionVisual();
                manager.ClearHeldState();
            }

            SelectTargetSlot();
            return;
        }
        
        // 不同物品：交换
        // 把手上物品放到箱子，把箱子物品放到背包源槽位
        int sourceIndex = manager.GetSourceIndex();
        bool sourceIsEquip = manager.GetSourceIsEquip();

        if (sourceIndex < 0)
        {
            manager.ReturnHeldToSourceAndClear();
            return;
        }
        
        // 🔥 使用缓存引用
        var invService = CachedInventoryService;
        var equipService = CachedEquipmentService;
        
        if (invService == null)
        {
            manager.ReturnHeldToSourceAndClear();
            return;
        }
        
        // 检查源槽位是否为空
        ItemStack sourceSlot = sourceIsEquip 
            ? (equipService?.GetEquip(sourceIndex) ?? ItemStack.Empty)
            : invService.GetSlot(sourceIndex);

        if (sourceIsEquip && equipService == null)
        {
            manager.ReturnHeldToSourceAndClear();
            return;
        }
        
        if (sourceSlot.IsEmpty)
        {
            // 源槽位为空，可以交换
            SetContainerSlotPreservingRuntime(chest, targetIndex, heldItem, heldRuntimeItem);
            if (sourceIsEquip && equipService != null)
            {
                if (chestRuntimeItem != null && !chestRuntimeItem.IsEmpty)
                    equipService.SetEquipItem(sourceIndex, chestRuntimeItem);
                else
                    equipService.SetEquip(sourceIndex, targetSlot);
            }
            else
            {
                if (chestRuntimeItem != null && !chestRuntimeItem.IsEmpty)
                    invService.SetInventoryItem(sourceIndex, chestRuntimeItem);
                else
                    invService.SetSlot(sourceIndex, targetSlot);
            }
            manager.ClearHeldSourceSelectionVisual();
            manager.ClearHeldState();
            SelectTargetSlot();
            return;
        }

        manager.ReturnHeldToSourceAndClear();
    }
    
    /// <summary>
    /// 从 SlotDragContext 丢弃物品
    /// 🔥 使用 ItemDropHelper 统一丢弃逻辑
    /// </summary>
    private void DropItemFromContext()
    {
        if (!SlotDragContext.IsDragging) return;
        
        var item = SlotDragContext.DraggedItem;
        if (!item.IsEmpty)
        {
            // 🔥 使用 ItemDropHelper 统一丢弃逻辑
            if (SlotDragContext.DraggedRuntimeItem != null && !SlotDragContext.DraggedRuntimeItem.IsEmpty)
                FarmGame.UI.ItemDropHelper.DropAtPlayer(SlotDragContext.DraggedRuntimeItem);
            else
                FarmGame.UI.ItemDropHelper.DropAtPlayer(item);
        }
        
        SlotDragContext.End();
        HideDragIcon();
        ResetActiveChestHeldState();
    }
    
    /// <summary>
    /// 检测是否在垃圾桶区域
    /// 🔥 使用缓存引用优化性能
    /// </summary>
    private bool IsOverTrashCan(Vector2 screenPos)
    {
        // 查找 BoxPanelUI 中的垃圾桶
        var boxPanel = CachedBoxPanel;
        if (boxPanel != null)
        {
            var trashCan = boxPanel.transform.Find("BT_TrashCan");
            if (trashCan != null)
            {
                var rt = trashCan.GetComponent<RectTransform>();
                if (rt != null && RectTransformUtility.RectangleContainsScreenPoint(rt, screenPos))
                {
                    return true;
                }
            }
        }
        
        // 查找 PackagePanel 中的垃圾桶
        var packagePanel = CachedPackagePanel;
        if (packagePanel != null)
        {
            var trashCan = packagePanel.transform.Find("BT_TrashCan");
            if (trashCan == null)
            {
                // 尝试在子层级查找
                foreach (Transform t in packagePanel.GetComponentsInChildren<Transform>(true))
                {
                    if (t.name == "BT_TrashCan")
                    {
                        trashCan = t;
                        break;
                    }
                }
            }
            if (trashCan != null)
            {
                var rt = trashCan.GetComponent<RectTransform>();
                if (rt != null && RectTransformUtility.RectangleContainsScreenPoint(rt, screenPos))
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 检测是否在面板内
    /// 🔥 修复：复用 InventoryInteractionManager 的检测逻辑，避免 RectTransform 覆盖整个屏幕的问题
    /// 🔥 使用缓存引用优化性能
    /// </summary>
    private bool IsInsidePanel(Vector2 screenPos)
    {
        // 🔥 优先使用 InventoryInteractionManager 的检测方法（它使用配置好的精确区域）
        var manager = InventoryInteractionManager.Instance;
        if (manager != null)
        {
            return manager.IsInsidePanelBounds(screenPos);
        }
        
        // 回退：检测 BoxPanelUI
        var boxPanel = CachedBoxPanel;
        if (boxPanel != null && boxPanel.IsOpen)
        {
            var rt = boxPanel.GetComponent<RectTransform>();
            if (rt != null && RectTransformUtility.RectangleContainsScreenPoint(rt, screenPos))
            {
                return true;
            }
        }
        
        // 回退：检测 PackagePanel
        var packagePanel = CachedPackagePanel;
        if (packagePanel != null && packagePanel.IsPanelOpen())
        {
            var rt = packagePanel.GetComponent<RectTransform>();
            if (rt != null && RectTransformUtility.RectangleContainsScreenPoint(rt, screenPos))
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 🔥 选中状态优化：选中当前目标槽位
    /// </summary>
    private void SelectTargetSlot()
    {
        if (inventorySlotUI != null)
        {
            inventorySlotUI.Select();
            inventorySlotUI.RefreshSelection();
            return;
        }

        if (equipmentSlotUI != null)
        {
            equipmentSlotUI.Select();
            equipmentSlotUI.RefreshSelection();
        }
    }

    private void SelectSourceSlot()
    {
        var sourceSlotUI = SlotDragContext.SourceSlotUI;
        if (sourceSlotUI == null)
        {
            return;
        }

        sourceSlotUI.Select();
        sourceSlotUI.RefreshSelection();
    }
    
    /// <summary>
    /// 🔥 选中状态优化：清空源区域的所有选中状态（用于跨区域放置）
    /// 使用 SetAllTogglesOff() 更简洁可靠，性能开销可忽略
    /// </summary>
    private void DeselectSourceSlot()
    {
        var sourceSlotUI = SlotDragContext.SourceSlotUI;
        if (sourceSlotUI == null)
        {
            return;
        }

        sourceSlotUI.ClearSelectionState();
    }

    private void ReturnHeldToSourceContainer(IItemContainer container, int sourceIndex, ItemStack draggedItem, InventoryItem draggedRuntimeItem)
    {
        ItemStack sourceSlot = container.GetSlot(sourceIndex);
        if (sourceSlot.IsEmpty)
        {
            SetContainerSlotPreservingRuntime(container, sourceIndex, draggedItem, draggedRuntimeItem);
            return;
        }

        if (CanStackByBackpackRules(sourceSlot, draggedItem))
        {
            int maxStack = container.GetMaxStack(draggedItem.itemId);
            int acceptedAmount = Mathf.Min(draggedItem.amount, Mathf.Max(0, maxStack - sourceSlot.amount));
            if (acceptedAmount > 0)
            {
                InventoryItem sourceRuntimeItem = CreateRuntimeItemForAmount(
                    ResolveContainerRuntimeItem(container, sourceIndex),
                    sourceSlot.amount);
                UpdateContainerStackAmountPreservingRuntime(
                    container,
                    sourceIndex,
                    sourceSlot,
                    sourceRuntimeItem,
                    sourceSlot.amount + acceptedAmount);
            }

            int remainingAmount = draggedItem.amount - acceptedAmount;
            if (remainingAmount <= 0)
            {
                return;
            }

            draggedItem = CreateStackWithAmount(draggedItem, remainingAmount);
            draggedRuntimeItem = CreateRuntimeItemForAmount(draggedRuntimeItem, remainingAmount);
        }

        for (int index = 0; index < container.Capacity; index++)
        {
            if (index == sourceIndex)
            {
                continue;
            }

            if (container.GetSlot(index).IsEmpty)
            {
                SetContainerSlotPreservingRuntime(container, index, draggedItem, draggedRuntimeItem);
                return;
            }
        }

        Debug.LogWarning($"[InventorySlotInteraction] 回源槽位 {sourceIndex} 已被异步占用，且容器无空位，改为掉落到玩家脚下，避免覆盖现有物品。");
        if (draggedRuntimeItem != null && !draggedRuntimeItem.IsEmpty)
        {
            FarmGame.UI.ItemDropHelper.DropAtPlayer(draggedRuntimeItem);
        }
        else
        {
            FarmGame.UI.ItemDropHelper.DropAtPlayer(draggedItem);
        }
    }
    
    #endregion
}
