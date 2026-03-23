# 1.0.3 基础 UI 与交互统一改进 - 任务清单

## A. 本轮已完成的建档与收尾

- [x] 1. 确认当前 live 现场仍为 `D:\Unity\Unity_learning\Sunset @ main`。
- [x] 2. 复核背包 reject shake 卡顿的真实根因，并完成最小代码修复。
- [x] 3. 复核 Tooltip 当前真实状态不是“内容不足”，而是“没有真实接线”。
- [x] 4. 复核种子袋保质期、工具耐久、精力系统在 runtime 侧已存在的底层能力。
- [x] 5. 在 `2026.03.16` 下新建 `1.0.3基础UI与交互统一改进` 工作区，并落下需求 / 分析 / 设计 / 任务 / 记忆。

## B. Tooltip 入口重建

- [ ] 1. 为背包 / 箱子 / 装备槽位补悬浮进入与离开入口。
- [ ] 2. 为 Toolbar 槽位补悬浮进入与离开入口。
- [ ] 3. 梳理 Tooltip 显示 / 隐藏调用链，确认不与拖拽和点击链互相打架。

## C. Tooltip 静态出口统一

- [ ] 1. 将 `ItemTooltip.cs` 的静态描述出口从 `itemData.description` 改为统一消费 `itemData.GetTooltipText()`。
- [ ] 2. 校验种子、工具、食物、药水、树苗、装备等类型的静态 Tooltip 文案是否全部生效。

## D. Tooltip 实例态入口建立

- [ ] 1. 设计并接入 `ItemTooltip` 的实例态数据载荷。
- [ ] 2. 背包 / Toolbar 优先传入 `InventoryItem`。
- [ ] 3. 补工具耐久实例显示。
- [ ] 4. 补种子袋保质期、开袋状态、剩余种子数显示。
- [ ] 5. 对箱子 / 装备位明确区分“已有实例态”与“暂时只支持静态”的真实范围。

## E. 实例态掉落链闭环

- [ ] 1. 让 `ItemDropHelper.cs` 支持实例态掉落。
- [ ] 2. 验证掉落再拾取后，耐久等实例态数据不丢失。

## F. 食物 / 药水真实生效

- [ ] 1. 将 `ItemUseConfirmDialog.cs` 从日志占位推进到真实调用玩家状态系统。
- [ ] 2. 验证食物 / 药水使用后的精力与生命变化。

## G. 术语与文案统一

- [ ] 1. 统一 Tooltip、确认弹窗、状态展示中的“精力 / 体力 / stamina / vitality”口径。
- [ ] 2. 明确哪些内部命名可以保留，哪些用户可见文本必须统一为“精力”。

## H. 验证清单

- [ ] 1. Tooltip 在背包 / Toolbar / 箱子 / 装备位的悬浮显示稳定。
- [ ] 2. 工具实例耐久 Tooltip 与耐久条数据一致。
- [ ] 3. 种子袋实例态 Tooltip 在开袋前后都正确。
- [ ] 4. 掉落再拾取后实例态仍正确。
- [ ] 5. 食物 / 药水真实影响玩家状态。
- [ ] 6. 不引入新的 farm runtime 编译错误或 warning。

## I. 2026-03-23 第一轮实现进展
- [x] 1. `ItemTooltip.cs` 已接入统一静态文本出口，并不再只显示 `itemData.description`。
- [x] 2. `ItemTooltipTextBuilder.cs` 已建立，工具耐久与种子袋实例态信息可进入 Tooltip 文本。
- [x] 3. 背包 / 装备 / Toolbar 的 Tooltip 悬浮入口已接上，拖拽开始时会主动隐藏 Tooltip。
- [x] 4. 主要实例态拖拽 / 归位 / 掉落链已补齐，`InventoryItem` 不再在常见的换格与丢弃路径里直接丢失。
- [x] 5. `ItemDropHelper.cs` 与 `WorldItemPickup.cs` 已支持 `InventoryItem` 实例态掉落和拾取回背包。
- [x] 6. `ItemUseConfirmDialog.cs` 已将食物 / 药水的精力恢复接到 `EnergySystem`。
- [ ] 7. 玩家生命恢复仍未真正落地，当前项目内未发现可安全接入的玩家生命系统。
- [ ] 8. 箱子主 UI 仍走 `ChestInventory` 旧链，箱内实例态完整保真尚未彻底治理。
- [ ] 9. 用户现场验收与共享编译阻断清理仍待完成。
## J. 2026-03-23 第二轮收口：箱子实例态保真 + 农田预览遮挡
- [x] 1. `ChestInventory.cs` 与 `ChestInventoryV2.cs` 的 `Set/Clear/SwapOrMerge/Remove` 已统一补发 `OnInventoryChanged`，避免箱内实例态修改后 UI 不刷新。
- [x] 2. `ChestController.cs` 新增 `RuntimeInventory`，`BoxPanelUI.cs` 改为优先绑定运行时容器，箱子排序/刷新/订阅不再只认旧 `ChestInventory`。
- [x] 3. `InventorySlotInteraction.cs`、`InventoryInteractionManager.cs`、`SlotDragContext.cs` 已补齐 chest/inventory/equip/manager-held 四条链上的 runtime item 保真，避免交换或回滚时退化成静态 `ItemStack`。
- [x] 4. `FarmToolPreview.cs` 已复用 `OcclusionManager.SetPreviewBounds(Bounds?)`，仅把当前 hover 预览的 `ghostTilemap + cursorRenderer` 同步给遮挡系统，不把 queue/executing 预览并进去。
- [x] 5. `Assembly-CSharp.rsp` 已在 `D:\1_BBB_Platform\Unity\6000.0.62f1\Editor\Data\DotNetSdkRoslyn\csc.dll` 路径下再次独立编译通过。
- [ ] 6. 待用户在 Unity 场景回归：箱内实例态拖拽/交换/装备回滚是否保真，以及锄头/水壶 hover 预览在树或建筑后方时是否正确触发遮挡透明。
