## 当前已接受基线

1. `存档系统` 线程正在主刀 `restart / 剧情 / Home` 后的背包、toolbar、箱子、sort、input、scene-rebind 连续性收口，当前已实际落刀：
   - `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
   - `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
   - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
   - `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`
   - `Assets/YYY_Scripts/Service/Inventory/InventorySortService.cs`
   - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
2. 你这边当前 active slice 是 `toolbar固定槽位图标丢失修复`，并且 live state 里把：
   - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
   - `Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs`
   - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
   都登记进了 owned paths。
3. 这里已经发生真实撞面：`PersistentPlayerSceneBridge.cs` 现在不能再由两条线程同时继续施工。

## 当前唯一主刀

只收这一刀：

**把 `toolbar` 固定槽位 `index 4 / 8` 丢图标问题继续压实并修完，但作用域只允许留在 `ToolbarUI / ToolbarSlotUI` 自身。**

## 允许的 scope

你这轮只允许改：

- `Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs`
- `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`

只读允许看：

- `Assets/222_Prefabs/UI/0_Main/ToolBar.prefab`
- `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
- `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`

## 明确禁止的漂移

这轮不要再碰：

- `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
- `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
- `Assets/YYY_Scripts/Service/Inventory/InventorySortService.cs`
- `save / load / restart / restore` 语义
- `PersistentPlayerSceneBridge` 的 scene-rebind / inventory rebind / sort rebind / world-state capture
- `farm world persistence`、`Primary 离场世界态`、`存档格式`

如果你最终判断“必须改 bridge 才能修好 toolbar 图标”，**不要直接改**；请停在边界判断，把需要的最小 contract 写进回执。

## 完成定义

你这轮完成，必须同时满足：

1. `toolbar 4 / 8` 固定槽位丢图标问题在代码层被明确收口。
2. 修法优先落在：
   - 槽位绑定顺序
   - 运行时订阅 / 退订
   - `Refresh()` 触发链
   - rebind 后 toolbar 自身刷新一致性
3. 不引入对 `PersistentPlayerSceneBridge` 的新写入依赖。
4. 至少补一份你自己能站住的验证证据：
   - `validate_script` 或等价 compile 证据
   - `git diff --check` 针对你 own 的 toolbar 文件
5. 如果你只钉到根因但还不能安全落刀，也要明确报实“停在哪个最小 contract 上”，不要把它说成已修好。

## thread-state 要求

你当前 active slice 把 `PersistentPlayerSceneBridge.cs` 也登记成 owned 了，这一轮继续施工前先把现场收窄：

1. 先把当前旧 slice `Park-Slice`
2. 再重新 `Begin-Slice`
3. 新 slice 只登记：
   - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
   - `Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs`

不要再把 `PersistentPlayerSceneBridge.cs` 挂在这轮 owned paths 里。

## 固定回执格式

请按下面顺序回：

### A. 用户可读汇报层

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### B. 技术审计层

- changed_paths
- 是否触碰 `PersistentPlayerSceneBridge.cs`
- code_self_check
- pre_sync_validation
- 当前是否可直接提交到 `main`
- blocker_or_checkpoint
- 一句话摘要

### C. 如果需要我这边接 contract

只在你判断“非改 bridge 不可”时追加：

- 你需要 `存档系统` 线程提供的最小 contract 是什么
- 为什么 `ToolbarUI / ToolbarSlotUI` 自身无法闭环
- 你建议我落在哪个方法 / 入口点，而不是泛说“bridge 也要改”
