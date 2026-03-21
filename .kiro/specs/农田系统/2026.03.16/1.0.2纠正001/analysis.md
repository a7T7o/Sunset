# 1.0.2纠正001 - 分析文档

## 1. 文档目的

本文件只记录当前 `main` live 现场已经证实的事实，不再沿用旧 cleanroom / 污染分支的现场口径。

它回答三件事：
1. 当前 `main` working tree 里已经有哪些 `1.0.2` 实现。
2. 哪些验证已经完成，哪些仍然受共享问题阻塞。
3. 当前这条线距离“可直接给用户在 `main` 验收”还差什么。

## 2. 当前现场快照

### 2.1 Git / 工作目录

- 工作目录：`D:\Unity\Unity_learning\Sunset`
- 当前分支：`main`
- 当前 `HEAD`：`8ac0fb5d0db0714f9879ed12885aefc056a03624`
- 现场事实：
  - shared root live Git 当前并不干净；
  - `.kiro/locks/shared-root-branch-occupancy.md` 仍写着 `main + neutral`，但这只是旧占用记录，没有反映当前 working tree dirty。

### 2.2 当前 farm 相关 dirty 范围

当前 working tree 中真正属于本轮农田交互升级的代码 dirty 为：

- `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
- `Assets/YYY_Scripts/Farm/CropController.cs`
- `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
- `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementNavigator.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementPreview.cs`
- `Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs`
- `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
- `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
- `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`

说明：
- 当前 shared root 里还存在多组 unrelated dirty；
- 本轮 Git 收口必须严格白名单，不能把治理线或其他业务线的改动混进来。

### 2.3 编译与 Unity/MCP 现场

已证实：
- `Assembly-CSharp.rsp` 运行时代码独立编译通过，结果为 `0 error / 0 warning`。
- `git diff --check` 针对上述 farm 白名单文件返回空，说明当前补丁没有 diff 级别的格式错误。

已证实但不属于 farm 专属阻断：
- `Assembly-CSharp-Editor.rsp` 仍失败：
  - `Assets\Editor\NPCPrefabGeneratorTool.cs(789,43): error CS0246: NPCAutoRoamController`
- 这条错误属于共享 NPC Editor 代码问题，不是 farm runtime 闭环缺口。

已证实但当前不可作为 Unity live 验收依据：
- MCP 读场景 / 读 Console 目前返回 HTML 网关页：
  - `Sub2API - AI API Gateway`
- 这说明当前不是项目脚本报错，而是 MCP 通道失效，不能把它误写成“Unity 现场已验收失败”。

## 3. 当前代码已落地的 1.0.2 实现

### 3.1 活跃手持槽位保护链已落地

`GameInputManager.cs` 当前已经补入以下关键点：

- UI 冻结快照：
  - `_hasUIFrozenProtectedHeldSlot`
  - `_uiFrozenProtectedHeldSlotIndex`
  - `CaptureProtectedHeldSlotForUIFreeze()`
  - `ClearProtectedHeldSlotForUIFreeze()`
- UI 打开时的导航暂停与恢复：
  - `PauseCurrentNavigationForUI()`
  - `ResumePausedFarmNavigationAfterUI()`
- 活跃槽位保护判断：
  - `TryGetRuntimeProtectedHeldSlotIndex(...)`
  - `TryGetProtectedHeldSlotIndex(...)`
- 拒绝反馈：
  - `PlayProtectedHeldRejectFeedback(...)`
  - 统一同时触发 `ToolbarSlotUI.PlayRejectShakeAt(...)` 和 `InventorySlotUI.PlayRejectShakeAt(...)`

这说明当前实现已经不再只靠“UI 入口补一个 if”，而是把“当前被占用的手持槽位”抽成了运行时统一语义。

### 3.2 背包交换保护已经下沉到底层落点

`InventoryInteractionManager.cs` 当前新增了：

- `TryRejectProtectedHeldInventoryPickup(...)`
- `TryRejectProtectedHeldInventoryMutation(...)`
- `TryRejectProtectedHeldInventoryReshuffle()`
- `OnSlotBeginDrag(...)` 起手拒绝
- `HandleIdleClick(...)` 的 `Shift / Ctrl` 拒绝
- `ExecutePlacement(...)` 前的二次权威校验
- `OnSortButtonClick()` 整理拒绝
- `QuickEquip(...)` 前的保护校验

`InventorySlotInteraction.cs` 当前也新增了：

- `TryRejectAutomatedFarmToolInventoryMove()`
- `TryRejectProtectedHeldMutation(...)`
- 在 `OnPointerDown / OnBeginDrag` 等多个入口对库存槽位保护进行转发

结论：
- “第一次拒绝、第二次还能换”的旧漏洞已经不是单纯停留在 UI 入口；
- 当前真正的落点交换也会再次校验活跃槽位是否被占用。

### 3.3 Toolbar / Hotbar 的反馈与切换口径已收口

`HotbarSelectionService.cs` 当前新增：
- `CanApplySelectionChange(...)`
- `SelectIndex(...)` 在应用选中前会先调用 `GameInputManager.TryPrepareHotbarSelectionChange(...)`

`ToolbarSlotUI.cs` 当前新增或调整：
- Toolbar 点击先检查背包/箱子是否打开，再决定是否允许继续
- 之后才调用 `TryRejectActiveFarmToolSwitch(index)`

这正是用户纠正过的最终口径：
- 背包打开时 Toolbar 不能承担新的交互入口；
- 背包关闭时，Toolbar 切换仍要经过活跃槽位保护。

### 3.4 右键导航与 WASD 已开始统一到“移动意图”

`GameInputManager.cs` 当前已证实：
- `HandleRightClickAutoNav()` 在进入普通右键导航前，会优先：
  - `InterruptAutomatedFarmToolOperation()`
  - 或 `CancelFarmingNavigation()`
- `HandleMovement()` 中，`WASD + hasActiveQueue` 会：
  - `HideFarmToolPreview()`
  - `ClearActionQueue()`
  - 非执行态下 `CancelFarmingNavigation()`
  - `ForceUnlock()`
- `TryEnqueueFromCurrentInput()` 在检测到手动移动输入时直接 `return`

结论：
- 右键已经不再是“完全独立于农田状态机之外的另一条导航命令”；
- 高频点击 + 持续移动场景也已经在 enqueue 入口和 movement 中断口径上双重收紧。

### 3.5 预览所有权与清理逻辑已升级到跨层键

`FarmToolPreview.cs` 当前实现已经不再只用裸 `cellPos`：

- 新增 `PreviewCellKey(layerIndex, cellPos)`
- `queuePreviewPositions`
- `tillQueueTileGroups`
- `executingTileGroups`
- `executingWaterPositions`
  都改为使用跨层 key

并且补了安全清理路径：
- `CollectOccupiedPreviewPositionsForLayer(...)`
- `PromoteToExecutingPreview(int layerIndex, Vector3Int cellPos)`
- `RemoveExecutingPreview(int layerIndex, Vector3Int cellPos)`
- `ClearQueuedPreviewsInternal(bool clearExecutingPreviews)`
- `ClearAllQueuePreviews(bool)` 顶部强制走 safe path

说明：
- 当前实现没有继续上提到 `ownerToken`；
- 但对用户本轮报告的“跨层执行预览残留”和“清不掉的残留”来说，这一版已经是当前 working tree 的实际收口方案。

### 3.6 透明幽灵作物的显示链做了三层防守

当前与“透明幽灵作物”相关的 live 修正包括：

1. `PlacementManager.cs`
   - 种植成功后调用 `ConfigureCropPlacementVisuals(...)`
   - 为作物补齐图层、sorting order 和 `DynamicSortingOrder`
2. `PlacementPreview.cs`
   - `Hide()` 会重置：
     - `lockedPosition`
     - `itemPreviewRenderer.sprite`
     - `itemPreviewRenderer.color`
     - `itemPreviewRenderer.transform.localPosition`
3. `CropController.cs`
   - `Initialize(...)` 与 `Load(...)` 都会强制恢复：
     - `spriteRenderer.enabled = true`
     - `interactionTrigger.enabled = true`

结论：
- 当前补丁不是把幽灵作物武断归因为 `SpriteRenderer.color`；
- 而是从预览退场、作物显示链、加载恢复三层做了防守性闭环。

## 4. 当前与 cleanroom 的关系

当前正确关系不是“回 cleanroom 继续开发”，而是：

1. `main` working tree 已经承接了大部分 `1.0.2` 有效代码；
2. `codex/farm-1.0.2-cleanroom001` 现在只提供历史参考；
3. `main` 当前真正缺的是：
   - `1.0.2纠正001` 正文文档
   - 当前 live 现场的记忆同步
   - 严格白名单 Git 收口

另外，当前 main working tree 相对 cleanroom 还多了几处额外补位：

- `CropController.cs`：显示/交互恢复
- `FarmToolPreview.cs`：safe clear 强制路径
- `InventoryInteractionManager.cs`：pickup 二次拒绝
- `ToolbarSlotUI.cs`：背包打开时先禁用 Toolbar 交互

这些 extra fix 都服务于用户后续真实验收，不应被机械回退到 cleanroom 版本。

## 5. 当前仍未完成的事项

### 5.1 已完成但尚未正式落盘到 main 的内容

- `1.0.2纠正001` 这套正文文档当前还没进 `main` tracked 工作树。
- 当前三层记忆里还没有把“main-only + 1.0.2 live dirty + 编译/MCP现状”正式补记完整。

### 5.2 仍待用户现场验收的闭环

以下 6 条仍然需要用户在 Unity 里做真实场景验收：

1. 背包打开期间，活跃手持槽位始终不可交换，多次尝试结果一致。
2. 抖动的是当前活跃槽位，不是目标槽位。
3. UI 打开中断导航后，不再留下“预览还在 / 队列还在 / 人不动”的错乱态。
4. 右键导航与 `WASD` 对农田自动链的收尾一致。
5. 高频点击 + 持续移动不再制造不可清理残留。
6. 幽灵透明作物在用户现场是否已经被真正消灭。

## 6. 当前结论

1. 从 runtime 代码层看，`1.0.2纠正001` 已经大部分落在 `main` working tree，不再是“历史分支里的未恢复内容”。
2. 当前真正的共享阻断不是 farm runtime，而是：
   - NPC Editor 编译错误
   - MCP 网关异常
3. 本轮最合理的下一步不是重做实现，而是：
   - 把 `1.0.2` 正文与记忆补回 `main`
   - 做白名单 Git 收口
   - 再由用户在 `main` 的 Unity 场景里做真实验收
