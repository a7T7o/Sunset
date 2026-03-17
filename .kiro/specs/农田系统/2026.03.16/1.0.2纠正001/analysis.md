# 1.0.2纠正001 - 分析文档

## 1. 文档目的

本轮不是继续给 `1.0.1` 叠 if，而是先把用户指出的交互口径、代码事实、Unity 现场快照统一成一份可审核证据链。

本文件只回答三件事：
1. 当前问题到底是不是用户描述的那样。
2. 已经证实的根因链在哪里。
3. 后续实现应该收口到什么统一口径，而不是继续各处零散补丁。

## 2. 本轮现场快照

### 2.1 Git / 工作区现场

- 工作目录：`D:\Unity\Unity_learning\Sunset`
- 当前分支：`codex/npc-main-recover-001`
- 当前 `HEAD`：`8aed637f`
- 说明：这不是旧的 `main` / `farm` 快照，本轮结论全部以这里的实时回读为准。

### 2.2 Unity / MCP 现场

- MCP 当前可用。
- 活动场景：`Assets/000_Scenes/Primary.unity`
- 当前 Console 未见新的 farm 相关红编译或 warning。
- 当前 Console 只读可见的非 farm 噪音为两条 `There are no audio listeners in the scene...`。
- 当前属于编辑态快照，没有用户问题发生当下的 live 场景，因此“透明幽灵作物”本轮不能写成已现场复现。

### 2.3 总体结论

当前问题的核心不是“少了几个入口判断”，而是下面三条基础建模都不成立：

1. 系统没有统一建模“当前正在使用中的手持内容”。
2. UI 打开后被实现成“局部暂停队列”，而不是“世界交互冻结 + 当前活跃手持流程不可交换”。
3. 农田预览/队列/执行预览的追踪键太弱，只用 `cellPos`，无法稳定表达多来源、多状态、多楼层的所有权。

这三条导致了用户看到的整串现象：交换限制失真、右键与 WASD 语义不一致、预览残留、状态错乱、偶发透明幽灵态。

## 3. 核心证据总览

### 3.1 “当前正在使用中的手持内容”没有统一语义

- `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs:2654-2685`
  - `HasActiveAutomatedFarmToolOperation()` 只在 `IsPlacementMode == true`、当前选中槽位是 `Hoe/WateringCan`、并且处于队列/导航/执行态时才返回 `true`。
- `Assets/YYY_Scripts/Service/Player/PlayerInteraction.cs:205-269`
  - 真实的动作锁仍在 `isPerformingAction` / `currentAction`。
  - 农具长按分支会在队列为空时重新 `TryEnqueueFromCurrentInput()`，然后继续 `OnFarmActionAnimationComplete()`。

结论：
- 当前系统同时存在“动作锁”和“自动农具队列锁”两套状态。
- `1.0.1` 只补了后者，没有上提到“所有当前正在使用、不会被 UI 切换打断的手持流程”。

### 3.2 背包交换限制只补在入口，没有补到底层落点

- `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs:133-145`
  - `TryRejectAutomatedFarmToolInventoryMove()` 只是把请求转交给 `GameInputManager.TryRejectActiveFarmToolInventoryMove(...)`。
- `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs:167-205`
  - 修饰键交换只在 `OnPointerDown` 入口拦。
- `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs:223-245`
  - 拖拽只在 `OnBeginDrag` 入口拦。
- `Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs:521-590`
  - 真正的 `ExecutePlacement(...)` 里没有任何“当前正在使用中的手持内容不可交换”的底层统一校验。

结论：
- 当前保护是 UI 入口补丁，不是底层约束。
- 一旦流程绕过入口检查，后续 Held / 放置 / 交换仍然会成功落地。

### 3.3 当前槽位内容一旦变化，会立刻触发装备刷新

- `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs:58-88`
  - `OnSlotChanged()`、`OnHotbarSlotChanged()`、`SelectIndex()` 都会立即调用 `EquipCurrentTool()`。

结论：
- 只要底层让当前选中槽位被换掉，玩家手上的装备表现会立刻刷新。
- 这和 `PlayerInteraction` 仍在执行中的动作锁并不天然同步。

## 4. 逐问题取证

### 问题一：UI 打开时，“正在使用中的手持内容”为什么没有被统一建模

#### 现象

用户期望的口径是：
- UI 打开后，世界交互整体冻结。
- 允许背包内部整理。
- 但凡属于“当前正在使用、不会被 UI 切换打断的手持内容/动作/流程”，都始终不能在背包里交换。

当前实现没有做到这一点。

#### 已证实事实

- `GameInputManager.cs:249-263`
  - UI 打开时只做 `_isQueuePaused = true` 和 `CancelCurrentNavigation()`。
  - UI 关闭时只在 `队列>0 && !_isExecutingFarming` 时恢复 `ProcessNextAction()`。
- `GameInputManager.cs:2654-2685`
  - 交换限制只覆盖“放置模式下 + 当前手持 Hoe/WateringCan + 队列/导航/执行态”。
- `PlayerInteraction.cs:205-269`
  - 真正的动作执行与长按续播仍受 `isPerformingAction/currentAction` 驱动。

#### 当前推断

当前实现把“交换限制”错误地绑在了“自动农具队列”上，而不是绑在“当前活跃手持流程”上。

这会漏掉两类情况：
1. 动画已经在播、动作已经在执行，但外层 helper 不认为自己是“自动农具队列”。
2. 后续 placeable、食物、消耗品等流程接入时，会继续绕开这一层判断。

#### 推荐修正口径

统一上提为：

`当前正在使用中的手持内容 = 当前被系统认定为仍在执行、仍拥有源槽位、且不允许通过 UI 切换打断的那条手持流程。`

它至少要覆盖：
- 动画播放中的工具动作
- 队列中的自动农具动作
- 导航到达前的待执行动作
- 正在进行的 placeable 放置
- 后续食物 / 消耗品 / 其他“手持后不可在 UI 里偷换”的流程

#### 后续实现需验证项

- UI 打开时，动画可继续自然结束，但不能因为 UI 中的交换尝试而变成“强制中断”或“提前完成”。
- 当前活跃手持流程结束前，对应源槽位始终不可交换。

### 问题二：为什么会出现“第一次拒绝、第二次可换、关 UI 后原动作继续完成”

#### 现象

用户当前看到的是：
- 第一次交换尝试会触发失败反馈。
- 再次交换却能成功。
- 关闭 UI 后，原本正在进行的锄地/农具动作还会继续或完成。

#### 已证实事实

- `InventorySlotInteraction.cs:167-205`
  - 修饰键交换只在入口拦截一次。
- `InventorySlotInteraction.cs:223-245`
  - 拖拽只在开始拖拽入口拦截。
- `InventoryInteractionManager.cs:521-590`
  - `ExecutePlacement(...)` 没有二次校验，会直接交换源槽位与目标槽位。
- `HotbarSelectionService.cs:58-88`
  - 当前槽位数据一变就会立刻刷新装备。
- `GameInputManager.cs:2688-2701`
  - `InterruptAutomatedFarmToolOperation()` 会隐藏预览、清队列、清 hotbar cache。
  - 但如果 `isExecuting == true`，它不会 `CancelFarmingNavigation()`，也不会 `ForceUnlock()`。
- `PlayerInteraction.cs:205-269`
  - 农具动作完成时仍会继续走 `OnFarmActionAnimationComplete()` / `ClearActionQueue()` / `ApplyCachedHotbarSwitch()`。

#### 当前推断

根因链很清晰：
1. 第一次交换被入口补丁拦住，所以出现正确的抖动和失败音效。
2. 但真正的 Held / 放置 / 落点交换没有底层兜底，后续路径可以绕过入口补丁。
3. 槽位一旦真的被换掉，`HotbarSelectionService` 会立刻重刷装备。
4. 与此同时，`PlayerInteraction` 里的旧动作仍然处于自己的完成回调链中，所以会继续把原动作走完。

#### 推荐修正口径

- “拒绝交换”不能再兼任“打断当前流程”。
- 底层交换必须在真正 `ExecutePlacement(...)` 前再次校验源槽位是否被活跃手持流程占用。
- 活跃手持流程若不允许被 UI 打断，则无论第几次交换尝试，结果都必须稳定为“拒绝，不交换，不完成，不中断”。

#### 后续实现需验证项

- 连续多次尝试交换同一活跃槽位，结果必须完全一致。
- 关闭 UI 后，不会因为 UI 中发生过尝试交换而出现原动作“补做完毕”。

### 问题三：为什么滚轮拒绝抖错槽位

#### 现象

用户要求：
- 被拒绝时，抖动的是“当前正在使用的工具/槽位”。
- 不是“滚轮即将切过去的目标槽位”。

#### 已证实事实

- `GameInputManager.cs:2709-2718`
  - `TryRejectActiveFarmToolSwitch(int requestedIndex)` 调用 `ToolbarSlotUI.PlayRejectShakeAt(requestedIndex)`。
- `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs:442-446`
  - `PlayRejectShakeAt(int hotbarIndex)` 会精确抖指定的槽位。

#### 当前推断

当前逻辑把“被拒绝的请求目标”当成了“当前活跃流程的反馈目标”，因此抖错了对象。

#### 推荐修正口径

- 拒绝反馈必须锚定“当前正在使用的源槽位”。
- 背包打开时，Toolbar 作为背包第一行的镜像语义，需要同步抖动同一索引。
- 背包关闭时只抖 Toolbar 即可，不需要反向驱动背包 UI。

#### 后续实现需验证项

- 滚轮、数字键、快捷键切换全部抖当前活跃槽位。
- 背包打开时，从背包里尝试交换活跃槽位，应同时看到背包格与 Toolbar 对应格的反馈。

### 问题四：为什么打开背包会中断导航但保留预览，之后还能点出队列却不移动

#### 现象

用户当前看到的是：
- 创建耕地队列后开始导航。
- 中途打开背包，导航中断。
- 预览残留。
- 之后还能继续点出新的预览/队列，但玩家不再移动。

#### 已证实事实

- `GameInputManager.cs:249-263`
  - UI 打开时只做 `CancelCurrentNavigation()`，不清队列，不重置 `_farmNavState`，不清预览。
- `GameInputManager.cs:1960-1975`
  - `CancelCurrentNavigation()` 只停止导航协程和 `autoNavigator`，只把 `_farmNavigationAction` 置空。
- `GameInputManager.cs:301-308`
  - UI 打开时 `UpdatePreviews()` 直接 `return`，不会主动隐藏旧预览。
- `PlacementManager.cs:209-222`
  - placeable 侧 UI 打开时也是“隐藏预览但保持 `Locked/Navigating` 状态”，定义为暂停，不中断。

#### 当前推断

当前系统把“UI 打开”实现成了一个非常轻量的暂停：
- 停掉导航。
- 暂停取下一个队列动作。
- 但不真正收束当前状态机。

这会留下三个悬空状态：
1. 视觉上残留的预览。
2. 数据上仍未被清空的队列。
3. 逻辑上没有真正退出的导航/锁定状态。

所以用户后续还能制造新预览，但角色不会恢复成一条干净、可继续移动或可继续导航的流程。

#### 推荐修正口径

- UI 打开应进入“世界冻结态”，而不是“只停协程的轻暂停”。
- 冻结态必须显式定义：
  - 哪些世界输入被禁止
  - 哪些预览只隐藏、不清除
  - 哪些队列推进被暂停
  - 哪些状态在 UI 关闭后可以恢复，哪些必须重新验证

#### 后续实现需验证项

- 打开 UI 后，不能继续创建新的世界队列。
- 关闭 UI 后，只能恢复到一个自洽状态，不能留下“预览还在 / 队列还在 / 导航没了”的组合态。

### 问题五：为什么 WASD 能清理，右键导航却不能

#### 现象

用户已经验证到：
- `WASD` 能正确清掉一批残留。
- 右键导航不能承担同样的“移动打断”职责。

#### 已证实事实

- `GameInputManager.cs:552-569`
  - `WASD + hasActiveQueue` 时会：
    - `HideFarmToolPreview()`
    - `ClearActionQueue()`
    - 在非执行态下 `CancelFarmingNavigation()`
    - `ForceUnlock()`
- `GameInputManager.cs:940-975`
  - 右键导航在农田状态下只会先 `CancelFarmingNavigation()`。
  - 之后进入普通右键导航链。
  - 它不会同步 `HideFarmToolPreview()`，也不会 `ClearActionQueue()`。

#### 当前推断

`WASD` 被实现成“玩家主动移动 = 中断当前农田自动链”，而右键只被实现成“另一次导航命令”。

这就是为什么：
- `WASD` 能把旧的农田状态收掉。
- 右键却只是把导航层改了一下，没有把队列 / 预览 / 锁定语义当成同一件事处理。

#### 推荐修正口径

右键导航必须被提升为“移动意图”的一种，而不是独立于农田状态机之外的另一条命令链。

也就是说：
- 如果当前处于农田自动流程，右键首先要走统一中断/冻结判断。
- 只有通过这层判断后，才能进入普通右键导航。

#### 后续实现需验证项

- 农田队列存在时，右键和 `WASD` 对预览/队列/导航的收尾口径必须一致。
- UI 打开时，右键不允许再制造新的世界导航/新队列。

### 问题六：为什么“持续移动 + 高频点击”会制造大量残留并且难以清理

#### 现象

用户描述的是高压边界：
- 长按方向键持续移动。
- 同时高速点击玩家周边可耕地位置。
- 会制造很多残留。
- 后续切模式、开关 UI、解除手持都未必清得掉。

#### 已证实事实

- `FarmToolPreview.cs:189-196`
  - 当前追踪结构是：
    - `queuePreviewPositions`
    - `tillQueueTileGroups`
    - `executingTileGroups`
    - `executingWaterPositions`
  - 它们都只以 `Vector3Int cellPos` 作为主键。
- `FarmToolPreview.cs:899-920`
  - `Hide()` 只隐藏鼠标跟随预览，不清空队列预览。
- `FarmToolPreview.cs:1288-1314`
  - `ClearAllQueuePreviews()` 会清队列预览，但有意跳过执行预览。
- `FarmToolPreview.cs:1317-1379`
  - `PromoteToExecutingPreview()` / `RemoveExecutingPreview()` 仍然只按 `cellPos` 迁移和清理所有权。

#### 当前推断

当前追踪模型表达不了下面这些并发情况：
- 同一格在不同楼层
- 同一格从实时预览切到队列预览，再切到执行预览
- 高频点击下多个来源同时声称拥有同一组 tile

一旦所有权记录丢失，后续任何 `Hide()`、`ClearQueue()`、`切模式`、`关 UI` 都只能清掉“还能找到 key 的那部分”，于是就出现残留。

#### 推荐修正口径

预览/队列/执行预览的追踪键必须至少升级为：

`(layerIndex, cellPos, previewKind, ownerToken)`

并且明确区分：
- 视觉隐藏
- 队列清空
- 执行预览提升
- 执行完成回收

#### 后续实现需验证项

- 高频点击 + 持续移动下，残留必须可以被统一清理。
- 不同楼层或同一格不同状态之间不能互相吃掉所有权。

### 问题七：种植后“透明幽灵作物”的最可能根因链是什么

#### 现象

用户现场看到的是：
- 作物已经正常种下。
- 参数正常。
- 可以正常生长。
- 但外观有概率呈现出“像队列预览/预览幽灵”的半透明态。

#### 已证实事实

- `PlacementPreview.cs:83-94`
  - 预览系统会创建独立的 `ItemPreview` 子物体和 `SpriteRenderer`。
  - 其 `sortingOrder = 32000`。
- `PlacementPreview.cs:190-217`
  - 种子预览会使用 `cropPrefab` 第一阶段 sprite，并把 alpha 设为 `itemPreviewAlpha = 0.8`。
- `PlacementPreview.cs:501-507`
  - 无效/有效态都直接改 `itemPreviewRenderer.color.a = itemPreviewAlpha`。
- `PlacementManager.cs:1158-1186`
  - 真正种植时会实例化 `seedData.cropPrefab` 并初始化 `CropController`。
- `PlacementManager.cs:1333-1362`
  - 会调用 `ConfigureCropPlacementVisuals(...)`，然后把 `DynamicSortingOrder` 挂到 `controller.gameObject` 或 `cropObject`。
- `DynamicSortingOrder.cs:38-46`
  - 组件要求自己所在 `GameObject` 上必须有 `SpriteRenderer`，否则会报错并自禁用。
- `CropController.cs:205-210`
  - 成熟 glow 只在白色到暖白之间插值，不会做半透明。
- `CropController.cs:719-738`
  - 常态 `UpdateVisuals()` 只会设回 `Color.white` 或 `WitheredColor`。

#### 当前推断

本轮不能把问题写成“已确认根因”，但最可疑的是下面三条链：

1. 预览残留链
   - 种子预览本身就是 alpha=0.8 的独立 `SpriteRenderer`。
   - 如果预览对象没有在某个边界正确退场，就可能叠在真实作物之上，看起来像“透明幽灵作物”。
2. 排序/宿主链
   - `DynamicSortingOrder` 被挂到 `controller.gameObject` 时，前提是该对象本身就有 `SpriteRenderer`。
   - 如果真正显示 sprite 的宿主和 `CropController` 宿主不一致，可能导致排序脚本挂错对象，自禁用或排序失效。
3. 多 renderer 叠层链
   - 一个真实作物 renderer 正常，另一个残留/预览 renderer 仍在上层，视觉上就会出现“参数正常但外观看起来透明”的假象。

#### 当前不能下结论的点

- 本轮 Unity live 快照里没有现成 `CropController` 实例可直接复验。
- Console 也没有看到与这次透明态直接对应的 farm 新报错。

#### 推荐修正口径

透明幽灵作物目前必须被归类为：

`已证实症状 + 已证实若干嫌疑链 + 尚未 live 复现确认`

不能把它提前写成“就是 CropController 的颜色问题”或“就是种子预览没关”。

#### 后续实现需验证项

- 复现当帧同时抓：
  - 层级面板中的真实 crop 对象
  - `PlacementPreview/ItemPreview` 是否仍然存活
  - 每个相关 `SpriteRenderer` 的 alpha / sortingLayer / sortingOrder
  - `DynamicSortingOrder` 是否挂在真正持有 `SpriteRenderer` 的对象上

## 5. 本轮结论

### 5.1 已证实的设计性问题

1. `1.0.1` 的交换限制作用域建错了，建成了“自动农具 helper”，而不是“当前活跃手持流程”。
2. 当前交换限制只补在 UI 入口，没有补到底层实际交换落点。
3. UI 打开后的世界冻结边界没有单独建模，导致“暂停了一部分，放过了另一部分”。
4. 预览/队列/执行预览所有权追踪太弱，解释得通用户说的残留问题。

### 5.2 本轮尚未证实但已收窄的点

1. 透明幽灵作物的直接根因还没 live 复现。
2. 但它更像“预览残留 / 排序宿主错误 / 多 renderer 叠层”的显示链问题，不像 `CropController.color` 主逻辑。

## 6. 审核后实现的最小边界

本轮建议的后续实现不会从“再补某个入口”开始，而是先收口下面几类文件：

- 手持流程统一语义：`GameInputManager.cs`、`PlayerInteraction.cs`
- 背包/Toolbar 底层交换限制：`InventoryInteractionManager.cs`、`InventorySlotInteraction.cs`、`HotbarSelectionService.cs`、`ToolbarSlotUI.cs`
- 队列/导航/预览状态机：`GameInputManager.cs`、`FarmToolPreview.cs`
- 透明幽灵作物显示链：`PlacementManager.cs`、`PlacementPreview.cs`、`DynamicSortingOrder.cs`

在用户审核通过前，本工作区不进入代码修改。
