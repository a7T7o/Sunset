# 1.0.1 自动农具进阶中断 - 设计文档

## 设计目标

把“自动农具队列”与“手动长按连续动作”彻底区分，拒绝逻辑只打在自动队列上。

## 状态口径

### 自动队列进行中
满足任一条件即可视为自动队列活跃：
- `_farmActionQueue.Count > 0`
- `_isProcessingQueue == true`
- `_farmNavState` 位于 `Locked / Navigating / Executing` 且当前入口来自队列链

### 手动长按连续动作
- 由 `PlayerInteraction` 的长按续播驱动
- 没有等待队列，也不是“点击锁定后自动走过去继续执行”的语义
- 不适用本次“拒绝切换/拒绝拖拽”的失败反馈链

## 方案一：切换入口统一改为“拒绝而非缓存”

### 现状
- `ToolbarSlotUI` 在锁定状态下调用 `CacheHotbarInput(index)`。
- `GameInputManager.HandleHotbarSelection()` 在锁定状态下对滚轮/数字键也走缓存。
- `PlayerInteraction.OnActionComplete()` 会消费缓存并最终切换。

### 设计
- 新增“自动农具队列是否活跃”的统一判断。
- 当该判断为真时：
  - Toolbar 点击 / 滚轮 / 数字键全部不再写入 `ToolActionLockManager` 缓存；
  - 直接触发失败反馈；
  - 然后按自动队列中断口径收束预览和等待队列。
- 当只是普通锁定或手动连续动作时，保留原有 lock cache 机制。

## 方案二：背包拖拽入口前置拒绝

### 现状
- `InventoryInteractionManager.OnSlotBeginDrag(...)` 和 `InventorySlotInteraction.OnBeginDrag(...)` 目前只检查 Held/Dragging 状态，不知道“自动队列正在使用这个工具”。

### 设计
- 在拖拽开始前判断：
  - 当前是否是自动农具队列活跃；
  - 被拖动槽位是否正是当前 hotbar 选中的工具槽位。
- 若满足，则直接拒绝：
  - 不清空源槽位
  - 不进入拖拽状态
  - 触发失败反馈

## 方案三：失败反馈

### 音效
- 优先复用现有 `AudioSource.PlayClipAtPoint(...)` 方式。
- 失败音效配置放到较少的热点脚本或统一的 UI 辅助里，避免在多个类里复制硬编码。

### 抖动
- 给 `ToolbarSlotUI` / `InventorySlotUI` 增加轻量级 UI 抖动接口，直接对目标 `RectTransform` 做短时位移。
- 拒绝切换时：
  - Toolbar 点击：抖目标 toolbar 槽位
  - 滚轮 / 数字键：抖“当前工具槽位”或目标槽位，以实际入口为准
- 拒绝拖拽时：抖源背包槽位

## A 类热文件风险

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs` 属于 A 类热文件。
- 进入实际修改前必须先查锁、获锁，再写入。
