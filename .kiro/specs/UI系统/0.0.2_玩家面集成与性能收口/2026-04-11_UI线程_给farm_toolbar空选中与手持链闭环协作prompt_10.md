当前已接受基线：

1. `PromptOverlay / 任务清单` 当前 owner 已切回 `spring-day1`，这轮不要借机回漂到 task list。
2. `toolbar` 当前真实问题不是“样式没亮”，而是会进入“没有任何槽位被选中”的坏状态：
   - 用户实机症状：
     - 底部热栏有时会进入没有任何内容被选中的状态
     - 这时点击 toolbar 槽位没反应
     - 切一次放置模式或做别的操作后，才可能恢复
3. UI 线程已经碰过这条链上的共享触点，这些改动现在是 live 事实，不要直接回退：
   - [GameInputManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs)
     - 已接入 `TryApplyHotbarSelectionChange(int)`
     - 已有 `ResetPlacementRuntimeState(...)`
     - 已有 `ForceResetPlacementRuntime(...)`
     - 世界左键只认 live placement session
   - [ToolbarSlotUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs)
     - 点击切槽已改走 `GameInputManager.TryApplyHotbarSelectionChange(...)`
   - [SaveManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs)
     - 读档 / fresh start 前统一 `ForceResetPlacementRuntime(...)`

当前唯一主刀：

把 `toolbar / hand-held / placement` 的真实 selection 状态机收平，堵住“空选中死态”这条 runtime 漏洞。

重点不是改 UI 外观，而是保证：
- 点击
- 滚轮
- 数字键
- 放置模式切换
- 读档 / fresh start
- 切场 / rebind

这些入口最后都落到同一个稳定 selection 真值，不再出现“视觉态、手持态、实际 selection”三者脱钩。

允许 scope：

- [GameInputManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs)
- [HotbarSelectionService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs)
- [ToolbarSlotUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs)
- [ToolbarUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs)
- [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
- 必要时最小补测试

明确禁止：

- 不要回漂到 `PromptOverlay`
- 不要碰 `InteractionHintOverlay / 状态提示卡`
- 不要扩到 `Workbench / NPC / Town / task list`
- 不要重做 toolbar 美术和表现
- 不要把点击链和滚轮链继续保留为两套各自恢复的真值

必须先查清的点：

1. `HotbarSelectionService.selectedIndex / selectedInventoryIndex / RestoreSelection(...)` 是否会在某些入口后留下非法或空态。
2. `OnPointerClick -> TryApplyHotbarSelectionChange -> SelectIndex -> RestoreSelection` 这条链上，是否有某个 reject / early-return 让 UI 选中态和真实 selection 分离。
3. `PersistentPlayerSceneBridge` 的 hotbar snapshot / restore，是否会在切场或 rebind 后把 selection 还原到一个“视觉不为空、真实为空”或“视觉为空、真实也没恢复”的死态。
4. `placement reset` 相关逻辑，是否会把“当前手持 / 当前预览 / 当前 selection”清了一半，留下半失效现场。
5. 如果“空手”在这个项目里是合法态，就必须给它明确稳定语义；如果不是合法态，就必须自动恢复到一个稳定有效槽位，不能停在坏死中间态。

完成定义：

1. 热栏不再出现“无任何槽位被选中且点击失效”的坏状态。
2. 点击 / 滚轮 / 数字键 对同一槽位的结果完全一致：
   - UI 选中态一致
   - 手持内容一致
   - runtime selection 真值一致
3. 放置模式切换、读档、fresh start、切场、scene rebind 后，不会留下 preview / selection / hand-held 脱钩。
4. 如果当前没有合法手持项，系统行为也必须是明确可恢复的，不允许用户靠“再切一次模式”碰运气恢复。
5. 全程 no-red。

固定回执格式：

- 当前主线
- 这轮实际做成了什么
- 现在还没做成什么
- 当前阶段
- 下一步只做什么
- 需要用户现在做什么
- changed_paths
- touched_touchpoints
- no-red 证据
- 当前 own / external blocker

thread-state：

- 如果这轮从只读进入真实施工，先跑 `Begin-Slice`
- 第一次准备 sync 前跑 `Ready-To-Sync`
- 如果本轮先停或让位，跑 `Park-Slice`
