# 1.0.2纠正001 - 设计文档

## 1. 设计目标

当前 `main` 的实现目标不是继续叠 if，而是把这轮用户真正关心的四类语义统一下来：

1. 当前哪一个手持槽位算“被占用”。
2. UI 打开时，世界交互到底冻结到什么程度。
3. 哪些输入属于 `Reject`，哪些输入属于 `Interrupt`。
4. 农田预览、队列预览、执行预览如何在多状态之间安全回收。

## 2. 活跃手持流程设计

### 2.1 当前实现采用“受保护手持槽位”语义

本轮没有额外引入新的管理器，而是把统一语义落在 `GameInputManager` 的运行时判断上：

- `TryGetRuntimeProtectedHeldSlotIndex(...)`
- `TryGetProtectedHeldSlotIndex(...)`
- `_hasUIFrozenProtectedHeldSlot`
- `_uiFrozenProtectedHeldSlotIndex`

这套设计的实际含义是：
- 运行中的自动农具链、放置导航链等，会把当前 hotbar 槽位视作“受保护源槽位”；
- UI 打开后，即使实时状态机暂停，也会把这个源槽位快照冻结下来；
- 只要该源槽位仍属于活跃流程，就不能在背包里被交换、拖走、整理或快速装备覆盖。

### 2.2 保护范围

当前设计实际覆盖到以下两类：

1. 自动农具流程
   - 队列中
   - 导航中
   - 执行中
2. placeable 放置导航流程
   - 物品仍处于锁定 / 导航 / 待执行放置态

这里保持写实：
- 当前实现还没有上提成一个跨全部系统的通用 `ActiveHeldSession` 类；
- 但对于本轮农田交互升级，它已经足够承担“统一活跃槽位保护”的职责。

## 3. UI 冻结设计

### 3.1 UI 打开不是直接中断，而是“冻结 + 恢复”

当前设计对应到代码是：

- 打开 UI：
  - `CaptureProtectedHeldSlotForUIFreeze()`
  - `_isQueuePaused = true`
  - `HideFarmToolPreview()`
  - `PauseCurrentNavigationForUI()`
- 关闭 UI：
  - `_isQueuePaused = false`
  - `PlacementManager.RefreshCurrentPreview()`
  - `ResumePausedFarmNavigationAfterUI()`
  - `ClearProtectedHeldSlotForUIFreeze()`

这套设计的边界是：
- 世界输入冻结；
- 当前流程不因 UI 打开而直接变成“取消”；
- 关闭 UI 后优先恢复到原先的导航/预览链；
- 如果不能恢复，再继续处理剩余队列。

### 3.2 UI 冻结下允许与不允许的事

允许：
- 背包内部的普通查看与整理操作

不允许：
- 影响受保护手持槽位的交换 / 拖拽 / 快速装备 / 整理
- 新的世界点击
- 新的农田队列推进
- 新的右键导航改点

## 4. Reject 与 Interrupt 的统一口径

### 4.1 Reject

适用场景：
- 背包尝试交换活跃槽位
- 背包尝试拖走活跃槽位
- Toolbar / Hotbar 尝试切换到别的槽位，但当前流程不允许切

设计动作：
- 只播放失败反馈
- 不允许交换落地
- 不让当前活跃流程因为这次尝试而偷偷完成或被打断

当前主要落点：
- `TryRejectActiveFarmToolInventoryMove(...)`
- `TryRejectProtectedHeldInventoryMutation(...)`
- `TryRejectProtectedHeldInventoryReshuffle()`
- `TryPrepareHotbarSelectionChange(...)`

### 4.2 Interrupt

适用场景：
- `WASD` 主动移动
- 右键导航覆盖
- 显式取消

设计动作：
- 清理队列
- 收尾导航
- 清理/隐藏预览
- 解锁移动

当前主要落点：
- `HandleMovement()`
- `HandleRightClickAutoNav()`
- `InterruptAutomatedFarmToolOperation()`
- `CancelFarmingNavigation()`

## 5. 反馈设计

### 5.1 当前活跃槽位是唯一反馈锚点

当前反馈链已经统一为：

- `ToolbarSlotUI.PlayRejectShakeAt(sourceSlotIndex)`
- `InventorySlotUI.PlayRejectShakeAt(sourceSlotIndex)`
- `PlayAutomatedFarmToolRejectSound()`

这意味着：
- 背包关闭时，用户至少能看到 Toolbar 当前槽位抖动；
- 背包打开时，Toolbar 与背包槽位都能对同一索引做反馈；
- 反馈锚点不再是目标槽位。

### 5.2 Toolbar 的交互顺序

当前设计特意把 `ToolbarSlotUI.OnPointerClick(...)` 的顺序改成：

1. 先检查背包 / 箱子是否打开
2. 再判断是否命中活跃槽位拒绝

这是为了对齐用户最终口径：
- 背包打开时 Toolbar 不承担交互；
- 反馈同步交给活跃槽位的统一拒绝逻辑。

## 6. 预览所有权设计

### 6.1 当前实现采用 `PreviewCellKey(layerIndex, cellPos)`

本轮没有继续上提到 `ownerToken`，而是先把最直接的错位问题收口到跨层 key：

- `PreviewCellKey(layerIndex, cellPos)`
- `queuePreviewPositions`
- `tillQueueTileGroups`
- `executingTileGroups`
- `executingWaterPositions`

这样可以直接解决：
- 同一 `cellPos` 在不同楼层互相吃掉所有权
- 提升为执行预览后无法准确回收
- 清理时误把别层的 tile 一起清掉

### 6.2 安全清理策略

当前 safe path 的设计要点是：

1. 队列清理与执行清理分开
2. 清理时先看执行态是否仍占用
3. 执行预览回收时，先检查同层是否还有其他队列预览占用同一 tile

对应实现：
- `CollectOccupiedPreviewPositionsForLayer(...)`
- `ClearQueuedPreviewsInternal(...)`
- `PromoteToExecutingPreview(...)`
- `RemoveExecutingPreview(...)`
- `IsTileQueuedByAnotherPreview(...)`

## 7. 种植显示链设计

### 7.1 预览对象与真实作物对象分离

当前设计通过两条链保证不串台：

1. `PlacementPreview.Hide()`
   - 退场时清掉 preview sprite / color / localPosition
2. `PlacementManager.ConfigureCropPlacementVisuals(...)`
   - 种植成功后对真实作物对象补齐 layer / sorting / dynamic sorting

### 7.2 作物显示恢复

当前额外增加：
- `CropController.Initialize(...)` 恢复 `spriteRenderer.enabled / interactionTrigger.enabled`
- `CropController.Load(...)` 同样恢复

这样即使曾在旧流程里隐藏过作物视觉，也不会把“隐藏态”带到真实种植对象上。

## 8. 当前设计结论

`1.0.2纠正001` 当前在 `main` 的实际设计并不是“理想化重写”，而是：

1. 用“受保护手持槽位”替代旧的自动农具局部 helper。
2. 用“UI 冻结 + 导航恢复”替代旧的轻量暂停。
3. 用 `Reject / Interrupt` 明确区分用户的不同输入语义。
4. 用 `PreviewCellKey(layerIndex, cellPos)` 和 safe clear 路径解决现阶段残留问题。
5. 用预览退场 + 作物显示恢复的组合方式，防守幽灵透明作物。

这就是当前 `main` working tree 的真实设计口径，后续验收若还暴露边界，再在这个基础上继续加严，而不是回到分散补丁思路。
