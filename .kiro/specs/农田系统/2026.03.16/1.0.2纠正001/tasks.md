# 1.0.2纠正001 - 任务清单

## A. 本轮已完成的只读取证

- [x] 1. 回读工作区规则、父/子 memory、线程 memory，并确认当前主线已切换到 `1.0.2纠正001` 的只读分析阶段。
- [x] 2. 复核当前 Git 现场：
  - 工作目录 `D:\Unity\Unity_learning\Sunset`
  - 分支 `codex/npc-main-recover-001`
  - `HEAD=8aed637f`
- [x] 3. 回读 UI / 背包 / Toolbar / Hotbar / 手持刷新链：
  - `GameInputManager.cs`
  - `PlayerInteraction.cs`
  - `InventoryInteractionManager.cs`
  - `InventorySlotInteraction.cs`
  - `HotbarSelectionService.cs`
  - `ToolbarSlotUI.cs`
- [x] 4. 回读农田队列 / 导航 / 预览 / 中断链：
  - `GameInputManager.cs`
  - `FarmToolPreview.cs`
  - `PlacementManager.cs`
- [x] 5. 回读种植显示链：
  - `PlacementPreview.cs`
  - `PlacementManager.cs`
  - `DynamicSortingOrder.cs`
  - `CropController.cs`
- [x] 6. 使用 Unity MCP 做只读取证：
  - 活动场景为 `Primary`
  - MCP 可用
  - Console 当前无新的 farm 红编译 / warning
- [x] 7. 将用户指出的问题整理成“现象 / 已证实事实 / 当前推断 / 推荐修正口径 / 后续验证项”。
- [x] 8. 补齐 `analysis.md`、`design.md`、`tasks.md` 首版正文。
- [x] 9. 同步子工作区、父工作区、农田系统总 memory、线程 memory。

## B. 审核已视为通过（用户默许）

- [x] 1. 认可“当前正在使用中的手持内容”为本轮统一核心语义。
- [x] 2. 认可“UI 打开 = 世界冻结，而不是轻量暂停”的设计口径。
- [x] 3. 认可“背包交换尝试属于 Reject，不属于 Interrupt”的设计口径。
- [x] 4. 认可“右键导航必须纳入移动中断语义”的设计口径。
- [x] 5. 接受“透明幽灵作物目前只能写成嫌疑链，不写成已复现根因”的写实口径。

## C. 已完成实现任务

- [x] 1. 在手持层补入统一的受保护手持槽位语义，替代当前“自动农具 helper”式判断。
- [x] 2. 把背包 / Toolbar 的交换保护下沉到真正的交换落点，收口“第一次拒绝、第二次还能换”。
- [x] 3. 重构 UI 打开后的冻结逻辑，明确：
  - 当前执行动作可否自然收尾
  - 队列何时暂停
  - 关闭 UI 后如何恢复
- [x] 4. 统一 `WASD` 与右键导航的中断/覆盖语义。
- [x] 5. 重构 `FarmToolPreview` 的预览所有权键与清理逻辑，解决跨层执行预览残留。
- [x] 6. 对种植显示链补做代码侧修正，优先从预览残留与执行预览所有权错位收口。
- [x] 7. 补上受保护手持槽位的拖拽起手拒绝，避免普通拖拽绕过背包 / Toolbar 的交换保护。

## D. 待用户场景验收的闭环

- [ ] 1. 背包打开期间，活跃手持槽位始终不可交换，多次尝试结果一致。
- [ ] 2. 拒绝反馈抖动的是当前活跃槽位，不是目标槽位。
- [ ] 3. 背包打开中断导航后，不再留下“预览还在 / 队列还在 / 人不动”的错乱态。
- [ ] 4. 右键导航与 `WASD` 对农田自动链的收尾一致。
- [ ] 5. 高频点击 + 持续移动不再制造不可清理残留。
- [ ] 6. 透明幽灵作物问题要么被复现并修正，要么被明确排除并留下证据。

## E. 提交前验证与收尾

- [x] 1. 通过 Unity MCP 复核当前活动场景为 `Primary`。
- [x] 2. 清空 Console 后触发 Unity 编译，当前未读到新的 farm `error / warning`。
- [x] 3. 使用 Unity Roslyn 独立编译 `Assembly-CSharp.rsp`，结果 `0 error / 0 warning`。
- [x] 4. 对本线白名单文件执行 `git diff --check`，无内容级错误，仅剩 CRLF 归一化提示。
- [x] 5. 同步子工作区 / 父工作区 / 农田系统总记忆 / 线程记忆。
- [ ] 6. 执行本线白名单 Git 收尾并生成可一步回退的 checkpoint。
