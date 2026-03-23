# 1.0.2纠正001 - 任务清单

## A. 已完成的接管与审计

- [x] 1. 回读 `main-only` 治理 prompt、线程记忆、农田父工作区记忆与 `2026.03.16` 工作区记忆。
- [x] 2. 重新核对 live Git 现场：
  - `D:\Unity\Unity_learning\Sunset`
  - `main`
  - `HEAD=8ac0fb5d0db0714f9879ed12885aefc056a03624`
- [x] 3. 重新核对 `1.0.2` 对应的 farm dirty 范围与 cleanroom 参考差异。
- [x] 4. 重新核对共享阻断：
  - runtime 编译通过
  - Editor 编译失败属于 NPC 共享问题
  - MCP 当前为网关异常，不能冒充 Unity live 验收

## B. 已完成的代码闭环

- [x] 1. 在 `GameInputManager.cs` 中补入受保护手持槽位语义与 UI 冻结恢复链。
- [x] 2. 在 `InventoryInteractionManager.cs` / `InventorySlotInteraction.cs` 中把交换保护下沉到真正落点。
- [x] 3. 在 `HotbarSelectionService.cs` / `ToolbarSlotUI.cs` 中收口 Hotbar / Toolbar 切换与反馈顺序。
- [x] 4. 在 `FarmToolPreview.cs` 中引入 `PreviewCellKey(layerIndex, cellPos)` 并收紧 safe clear。
- [x] 5. 在 `PlacementPreview.cs` / `PlacementManager.cs` / `CropController.cs` 中补做幽灵透明作物的防守性修正。
- [x] 6. 在 `PlacementNavigator.cs` / `GameInputManager.cs` 中补齐 UI 打开时导航暂停与关闭后的恢复语义。

## C. 已完成的文档与记忆补位

- [x] 1. 把 `1.0.2纠正001` 的 `requirements.md / analysis.md / design.md / tasks.md / memory.md` 补回 `main`。
- [x] 2. 同步 `2026.03.16` 父工作区 `memory.md`。
- [x] 3. 同步农田系统根工作区 `memory.md`。
- [x] 4. 同步当前线程 `memory_0.md`。

## D. 已完成的验证

- [x] 1. `Assembly-CSharp.rsp` 运行时代码独立编译通过，结果 `0 error / 0 warning`。
- [x] 2. `git diff --check` 针对 farm 白名单文件通过。
- [x] 3. 重新确认 `Assembly-CSharp-Editor.rsp` 当前失败点来自 `NPCPrefabGeneratorTool.cs -> NPCAutoRoamController`，不属于 farm 专属缺口。
- [x] 4. 重新确认 MCP 当前返回 HTML 网关页，因此本轮不把它写成“Unity 现场验收已失败”。

## E. 待用户在 main 场景里验收

- [ ] 1. 背包打开期间，活跃手持槽位始终不可交换，多次尝试结果一致。
- [ ] 2. 滚轮、数字键 `1~5`、Toolbar 点击、背包交换的拒绝反馈都落在当前活跃槽位。
- [ ] 3. UI 打开/关闭后，不再出现“预览还在 / 队列还在 / 人不动”的错乱态。
- [ ] 4. 右键导航与 `WASD` 对农田自动链的收尾一致。
- [ ] 5. 高频点击 + 持续移动不再制造不可清理残留。
- [ ] 6. 种植后的幽灵透明作物在用户现场不再复现。

## F. 本轮 Git 收口

- [x] 1. 本轮只认 farm 白名单代码与 `1.0.2` 正文/记忆，不混入 unrelated dirty。
- [x] 2. 当前补丁已整理到可执行白名单同步的状态，后续直接在 `main` 上生成可一步回退的 checkpoint。
