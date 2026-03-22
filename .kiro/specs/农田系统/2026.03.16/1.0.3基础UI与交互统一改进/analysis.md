# 1.0.3 基础 UI 与交互统一改进 - 现状分析

## 1. 已证实事实

### 1.1 背包 reject shake 的真实根因已经确认

当前背包 shake 发黏不是协程算法问题，而是 `InventorySlotUI.cs` 与 `ToolbarSlotUI.cs` 的 `Toggle` 配置不一致：

- `ToolbarSlotUI.cs` 早已关闭 `Toggle` 默认视觉过渡。
- `InventorySlotUI.cs` 在修复前仍保留默认 `Toggle` 视觉反馈。
- 结果是背包槽位的默认过渡和自定义 reject shake 同时生效，观感就会发黏、卡顿。

本轮已经做的最小修复是：

- `toggle.targetGraphic = null;`
- `toggle.transition = Selectable.Transition.None;`

因此这条问题已经从“待分析”转为“已收尾、待用户手感验收”。

### 1.2 Tooltip 的静态能力存在，但 UI 完全没用起来

当前项目里大量物品数据类都实现了 `GetTooltipText()`，包括：

- `SeedData`
- `ToolData`
- `FoodData`
- `PotionData`
- `SaplingData`
- `EquipmentData`

但 `ItemTooltip.cs` 的 `Show()` 最终仍直接使用 `itemData.description`。这说明现在不是“缺 Tooltip 文案生成能力”，而是“Tooltip UI 没接上已有能力”。

### 1.3 Tooltip 当前几乎没有真实交互入口

代码侧检索结果显示：

- `ItemTooltip.cs` 存在。
- 但当前没有稳定的 `ItemTooltip.Instance.Show(...)` 调用链。
- UI 侧也没有 `IPointerEnterHandler / IPointerExitHandler` 的现成悬浮入口。

这意味着 Tooltip 不是“接了一半”，而是“主体入口还没真正建立”。

### 1.4 实例态信息展示链缺失

当前很多运行时信息已经存在，但 UI 拿不到：

- `InventoryItem` 已支持实例耐久数据。
- `InventoryService.GetInventoryItem(int index)` 已提供运行时实例查询。
- `SeedBagHelper` 与 `InventoryService.OnDayChanged(...)` 已形成种子袋保质期 runtime 链。

问题在于 `ItemTooltip.Show()` 现在只拿到 `ItemStack` 和 `ItemData`，拿不到 `InventoryItem`。因此：

- 工具当前耐久
- 种子袋剩余天数
- 种子袋剩余种子数
- 开袋状态

这些信息即使底层存在，也没有展示出口。

### 1.5 掉落链对实例态仍然不闭环

`ItemDropHelper.cs` 已明确写了 TODO：当前掉落系统不支持 `InventoryItem` 实例数据。

这意味着：

- 背包 / Toolbar 当前虽然能显示耐久条，
- 但一旦涉及掉落、再拾取，实例态数据仍可能丢失，
- 所以这条链必须算进下一阶段，而不能只修 Tooltip。

### 1.6 食物 / 药水存在运行时配置，但没有真实使用效果

当前真实状态不是“食物和药水系统不存在”，而是“数据和状态系统存在，使用入口没接完”：

- `ToolData.energyCost` 已存在。
- `EnergySystem` 已存在，并且具备 `TryConsumeEnergy()`、`RestoreEnergy()` 等接口。
- `FoodData`、`PotionData` 也有恢复数值。

但 `ItemUseConfirmDialog.cs` 当前只打印日志，没有真正修改玩家状态，因此这一段必须被纳入下一阶段。

### 1.7 术语口径不统一

当前代码里并行存在几套说法：

- `EnergySystem`
- `IStaminaSystem`
- `Vitality`
- `体力`
- `精力`

如果不统一，Tooltip、确认弹窗、状态 UI 与后续说明文案都会继续冲突。

## 2. 为什么这轮要单独开 1.0.3

`1.0.2纠正001` 的核心任务是把农田交互链纠偏、收口、修正到能正确工作。当前用户提出的新内容已经明显越过了那个边界：

- 不再只是农田动作正确性；
- 而是要重建基础 UI 出口、实例态展示链和物品使用交互。

因此继续把这些内容塞回 `1.0.2` 会让范围失真，也不利于后续验收和记录。

## 3. 当前阶段的真实优先级

### P0 已完成的阶段前置收尾

- 背包 reject shake 与 Toolbar 手感不一致的问题已完成代码级收尾。

### P1 下一阶段的真正入口

- 先把 Tooltip 从“有脚本无接线”修成“有入口、有内容、有实例态”。

### P2 与 Tooltip 强耦合的闭环

- 补掉掉落链的实例态丢失。
- 让工具耐久、种子袋状态显示在整条链上都成立。

### P3 第二梯队

- 把食物 / 药水从“确认对话框 + 日志”推进到“确认后真实生效”。
- 同步清理状态 UI 的术语口径。

## 4. 风险与边界

### 4.1 风险不是算法，而是入口分散

当前物品 UI 分散在：

- `InventorySlotUI`
- `ToolbarSlotUI`
- `EquipmentSlotUI`
- `InventorySlotInteraction`
- 箱子复用的 container 槽位链

如果只在一个槽位里偷偷补 Tooltip，最后一定会造成各面板行为不一致。

### 4.2 不能把“种子保质期 UI”误写成简单文案任务

如果只改 `GetTooltipText()`：

- 静态描述能变好看；
- 但运行时种子袋状态仍然展示不出来；
- 用户看到的仍不是他真正关心的内容。

### 4.3 不能误报“精力系统还没做”

这条旧说法已经过时。真实问题是：

- runtime 已有，
- UI 出口与物品使用入口没闭合。

### 4.4 当前不应顺手扩到场景资源或大规模美术改造

本阶段的核心价值是把基础交互口径先统一，让后续任何 UI 美化、表现升级都建立在真实可用的入口之上。

## 5. 当前结论

`1.0.3` 的第一性问题不是“某个物品描述写得不够好”，而是：

1. Tooltip 目前没有真实接线。
2. Tooltip 没有实例态入口。
3. 掉落链会破坏实例态。
4. 食物 / 药水使用效果还停在日志层。
5. 术语口径没有统一。

因此后续进入实现时，必须按“入口 -> 内容 -> 实例态 -> 丢失链 -> 使用效果 -> 术语口径”的顺序推进，而不是头痛医头地补单点文案。
