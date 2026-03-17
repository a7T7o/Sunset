# 1.0.2纠正001 - 设计文档

## 1. 设计目标

本轮设计要解决的不是单个 bug，而是给后续实现一套不会继续漂移的统一口径：

1. 统一“当前正在使用中的手持内容”语义。
2. 重新定义 UI 打开后的世界冻结边界。
3. 统一队列 / 导航 / 预览 / 执行 / 中断的状态机。
4. 统一 Toolbar / 背包的拒绝反馈口径。
5. 给“透明幽灵作物”建立一条可执行的排查和修正顺序。

## 2. 核心设计原则

### 2.1 不再用“自动农具 helper”代替“活跃手持流程”

后续实现必须引入一个统一概念，这里先用设计名词表示为：

`ActiveHeldSession`

它表达的是：
- 当前由哪个槽位发起
- 当前是哪类手持内容
- 当前处于什么阶段
- 是否允许被 UI 中的槽位交换破坏
- 是否允许继续世界输入

这个会话不是农田专属概念，它是更高层的“当前手持流程所有权”。

### 2.2 UI 打开 = 世界冻结，不是半暂停

UI 打开后的统一口径应为：

1. 世界输入冻结
   - 不再接受新的世界点击
   - 不再接受新的队列创建
   - 不再接受新的右键导航改点
2. 背包内部仍可整理
   - 但被 `ActiveHeldSession` 占用的源槽位始终不可交换
3. 当前正在执行中的动作可以自然收尾
   - 但 UI 里的任何交换/切换尝试都不能让它“被中断”或“被补做完毕”
4. 自动链的推进暂停
   - 当前动作结束后不继续自动推进下一项，直到 UI 关闭并重新验证恢复条件

### 2.3 交换限制是“拒绝”，不是“顺手中断”

后续实现必须明确区分两类语义：

1. `Reject`
   - 交换请求被拒绝
   - 只播放失败反馈
   - 不改变当前活跃手持流程
2. `Interrupt`
   - 当前流程被明确中断
   - 只允许由真正的中断源触发，比如玩家移动、显式取消、模式切换

背包交换尝试属于 `Reject`，不是 `Interrupt`。

## 3. 统一状态模型

### 3.1 状态分层

后续状态机建议拆成两层，而不是继续把所有状态塞在单一布尔量里。

#### A. 手持会话层

- `None`
- `Queued`
- `Navigating`
- `Executing`
- `Finishing`

#### B. UI 覆盖层

- `Normal`
- `UIFrozen`

`UIFrozen` 不是新的业务动作，而是覆盖在当前手持会话层之上的暂停/冻结态。

### 3.2 状态转换口径

#### 正常链

- `Preview -> Queued`
- `Queued -> Navigating`
- `Queued -> Executing`
- `Navigating -> Executing`
- `Executing -> Finishing`
- `Finishing -> Preview/None`

#### UI 链

- `Queued/Navigating/Executing/Finishing + UI 打开 -> UIFrozen`
- `UIFrozen + UI 关闭 -> 回到原会话层，并重新验证是否还能恢复`

#### 中断链

- `Queued/Navigating -> Interrupted`
  - `WASD`
  - 右键导航覆盖
  - 显式取消
  - 模式切换成功
- `Executing`
  - 默认不被 UI 交换打断
  - 仅允许自然结束后进入收尾

### 3.3 右键与 WASD 的统一语义

后续实现里，右键不能再被当成“另一条无关导航命令”。

右键应当先经过统一的“移动意图/中断意图”判定：
- 如果当前有农田自动链，右键和 `WASD` 应共享同一套中断收尾。
- 只有完成这一步，才能继续进入普通右键导航。

## 4. 槽位交换与反馈设计

### 4.1 槽位所有权

`ActiveHeldSession` 需要稳定记录：
- `sourceSlotIndex`
- `sourceContainerType`
- `itemKind`
- `currentPhase`

只要会话还活着，这个源槽位就视为“被占用”。

### 4.2 背包 / Toolbar 的统一反馈口径

#### 背包关闭时

- 只抖 Toolbar 当前活跃槽位。
- 不抖目标槽位。

#### 背包打开时

- 背包里的实际交互格需要抖。
- Toolbar 作为背包第一行的镜像语义，也要抖同一索引。
- Toolbar 在背包打开时不负责交互，只负责反馈同步。

### 4.3 底层保护落点

后续实现必须保留入口快速拒绝，但真正的权威保护要放到底层落点：

- `InventorySlotInteraction`：只负责尽早拦截和触发反馈
- `InventoryInteractionManager.ExecutePlacement(...)`：做最终权威校验
- `HotbarSelectionService`：在应用选中/装备刷新前再次确认当前槽位是否允许变更

只有这样，才能彻底消除“第一次拒绝、第二次还能换”的漏洞。

## 5. 预览 / 队列 / 执行预览设计

### 5.1 所有权追踪必须升级

当前只用 `cellPos` 追踪不够。

后续设计建议至少升级为：

`PreviewKey = (layerIndex, cellPos, previewKind, ownerToken)`

其中：
- `layerIndex` 解决多楼层混淆
- `previewKind` 区分实时预览 / 队列预览 / 执行预览
- `ownerToken` 区分高频点击下的不同会话所有权

### 5.2 三种清理语义必须分开

不能再让一个 `Hide()` 或 `Cancel...()` 同时承担多种含义。

后续建议拆成：

1. `HideRealtimePreview`
   - 只隐藏鼠标跟随预览
2. `FreezeSessionVisuals`
   - UI 打开时隐藏实时视觉，但保留恢复快照
3. `ClearQueuedPreviews`
   - 清理所有队列拥有的预览
4. `PromoteToExecuting`
   - 队列预览提升为执行预览
5. `RemoveExecuting`
   - 执行完成后回收该执行实例的预览

### 5.3 高频点击边界

高频点击 + 持续移动场景下，后续实现必须保证：
- 同一格的旧 owner 不会覆盖新 owner
- 旧 owner 清理时不会误删新 owner 的 tile
- `UI open / mode toggle / movement interrupt` 都能通过 ownerToken 做精确回收

## 6. 透明幽灵作物的修正设计

### 6.1 排查顺序

后续实现不要先碰 `CropController.color`，而是按下面顺序排：

1. 复现当帧检查 `PlacementPreview/ItemPreview` 是否仍存活
2. 检查真实 crop root、`CropController` 宿主、实际 `SpriteRenderer` 宿主是否一致
3. 检查 `DynamicSortingOrder` 是否挂在真正持有 `SpriteRenderer` 的对象上
4. 检查是否存在第二个残留 renderer 叠在上层
5. 最后才检查材质 / shader / alpha 写入

### 6.2 最小设计约束

后续修正必须保证：
- 预览 renderer 和真实作物 renderer 生命周期严格分离
- 排序脚本只挂在真正的显示宿主上
- 复现不到时不强行修改 `CropController` 主色逻辑

## 7. 后续实现建议范围

审核通过后，最小实现建议会落在下面几组文件：

- 会话语义与中断统一：`GameInputManager.cs`、`PlayerInteraction.cs`
- 背包/Toolbar 底层保护：`InventoryInteractionManager.cs`、`InventorySlotInteraction.cs`、`HotbarSelectionService.cs`、`ToolbarSlotUI.cs`
- 预览所有权与清理：`FarmToolPreview.cs`
- 种植显示链：`PlacementManager.cs`、`PlacementPreview.cs`、`DynamicSortingOrder.cs`

## 8. 本轮设计结论

`1.0.2纠正001` 的正确实现方向不是“继续修农田判断”，而是先把“活跃手持流程”“UI 冻结边界”“预览所有权”这三个基础概念统一起来。

在用户审核通过前，本工作区不进入任何业务代码修改。
