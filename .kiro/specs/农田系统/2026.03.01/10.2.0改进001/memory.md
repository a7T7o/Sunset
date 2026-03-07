# 10.2.0改进001 工作区记忆

## 模块概述
V 键统一放置模式改进：新增 V 键作为放置模式 toggle 开关，统一控制耕地工具（锄头、水壶）和放置物品（树苗、箱子、种子）的预览和交互。

## 当前状态
- 阶段：需求分析
- 进度：已创建工作区，待完成方案分析

---

## 会话1（2026-03-03）

用户需求：新增 V 键统一控制放置模式（toggle 切换），控制范围包括放置类物品（树苗、箱子、种子）和耕地工具（锄头、水壶）。默认状态下锄头只能除草不能耕地，水壶不能浇水。切换工具不影响放置模式状态。ESC/WASD 保持现有逻辑。

AI 行动：创建工作区目录结构，尝试创建审核文档（因路径问题未成功）。

待完成：
- 创建工作区目录
- 读取相关代码文件分析当前实现
- 编写全面的方案分析和审核报告


## 会话2（2026-03-06）

用户需求：修复 10.2.0 当前核心 bug——放置模式关闭时，锄头不应整体失效；V 键只隔离耕地，非放置模式下锄头仍要保留普通工具身份，可正常挥动并可清除农作物/树苗等原有非耕地交互。

AI 行动：
- 重新核对 10.2.0 反思文档、主 memory 与 GameInputManager / PlayerInteraction / CropController 代码证据，确认根因是 `HandleUseCurrentTool()` 把 Hoe 无条件送入农田 FIFO，导致非放置模式下直接被吞。
- 修改 `GameInputManager.cs`：
  - `HandleUseCurrentTool()` 改为锄头双态分流：V 开时继续走农田入队；V 关时先尝试对鼠标下农作物入队 `RemoveCrop`，否则回退普通 `Crush` 动画请求。
  - 新增 `TryEnqueueHoeRemoveCropFromMouse()`，按玩家当前层级检测鼠标下 `CropController` 并入队清除农作物。
  - `TryEnqueueFromCurrentInput()` 同步改为遵循锄头双态，不再让 Hoe 在非放置模式误走耕地/浇水式 FIFO。
  - `IsHoldingFarmTool()` 改为 Hoe 只有在 `IsPlacementMode == true` 时才算农田工具，避免非放置模式锄头动画结束后被错误走农田收尾链。
- 尝试 Unity 重新编译与 EditMode 测试，当前 MCP 均超时，未拿到自动验证结果。

修改文件：
- Assets/YYY_Scripts/Controller/Input/GameInputManager.cs

当前状态：
- 已完成本轮核心代码修复。
- 待用户在 Unity 内手动重点验证：
  1. V 关闭时锄头可正常挥动。
  2. V 关闭时锄头可清除农作物、可挖树苗。
  3. V 开启时锄头仍只负责耕地。
  4. 长按与动画结束后不会再把非放置模式锄头误当农田工具。

## 会话3（2026-03-06）

用户需求：纠正此前对 V 键语义的错误收缩，明确 `V` 是统一的放置/施工模式总开关；`Hoe/WateringCan` 走 `GameInputManager` 农田链，`Seed/Sapling/PlaceableItem` 走 `PlacementManager` 放置链，但两条链都必须受 `V` 统一授权。要求先把纠正后的理解、需求与改进方案记录到 `纠正001.md`，再继续代码修复。

AI 行动：
- 创建并写入 `纠正001.md`，固化纠正后的正式需求、行为矩阵、设计边界、修复方向与验证重点。
- 修改 `HotbarSelectionService.cs`：切到 placeable 时先检查 `GameInputManager.Instance.IsPlacementMode`，仅在 `V` 开启时才进入 `PlacementManager.EnterPlacementMode(...)`；`V` 关闭时直接退出当前 placeable 放置模式。
- 修改 `GameInputManager.cs`：
  - `V` 切换后调用 `SyncPlaceableModeWithCurrentSelection()`，把当前手持 placeable 与 `V` 授权状态同步。
  - `SyncPlaceableModeWithCurrentSelection()` 补齐为：当前空槽/非 placeable/`V` 关闭时退出 `PlacementManager`；当前为 placeable 且 `V` 开启时进入并立即 `RefreshCurrentPreview()`。
  - `IsHoldingFarmTool()` 不再把种子当成农田工具，只允许 Hoe/WateringCan 在 `IsPlacementMode == true` 时视为农田工具。
  - `HandlePanelHotkeys()` 中 ESC 在 placeable 放置活跃时不再误开设置面板。
  - `HandleRightClickAutoNav()` 在 placeable 放置活跃时直接 return，避免右键导航抢占 `PlacementManager` 的右键退出逻辑。
- 使用 Unity MCP 重新编译，结果为 0 error、2 warning；warning 仅为既有未使用变量和已废弃字段提示。

修改文件：
- Assets/YYY_Scripts/Controller/Input/GameInputManager.cs
- Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs
- .kiro/specs/农田系统/2026.03.01/10.2.0改进001/纠正001.md

当前状态：
- 已完成本轮“V 统一授权农田链 + placeable 链”的最小兼容修复，并通过 Unity 重新编译。
- 待用户在 Unity 内手动重点验证：
  1. 开局默认 `V` 关闭时，Hoe/WateringCan/placeable 都不会进入有效施工/放置态。
  2. 当前手持 placeable 时按 `V` 开启，预览会立即出现；再按 `V` 关闭，预览会立即退出。
  3. `V` 开启时切换到 placeable 会立即进入预览；`V` 关闭时切换到 placeable 不会偷偷进入放置模式。
  4. ESC / 右键 / WASD 只处理中断当前链路，不再偷偷改写 `V` 状态。
  5. 之前已经修好的 Hoe 非放置模式普通挥动/清作物/清树苗能力保持不回退。
