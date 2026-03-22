# 1.0.3 基础 UI 与交互统一改进 - 设计方案

## 设计原则

1. 不重造已有 runtime 能力，优先把现有底层正确接到 UI。
2. 先统一入口，再丰富内容，最后补实例态与闭环。
3. 尽量沿用现有槽位和容器体系，不另起一套平行 UI 框架。
4. 新增代码以最小 helper 为上限，避免把 `1.0.3` 做成大重构。

## 一、Tooltip 总体设计

### 1.1 角色分工

- `ItemTooltip.cs`
  负责 Tooltip 面板本身的显示、定位、淡入淡出和字段渲染。

- 槽位交互入口
  负责在悬浮进入 / 离开时，向 `ItemTooltip` 传递“当前展示上下文”。

- Tooltip 文本拼装层
  负责把静态配置与实例态信息拼成最终展示文本。

### 1.2 入口层设计

当前代码里没有悬浮入口，因此第一步是把 Tooltip 入口接到真实槽位交互链。

优先接入的文件：

- `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
  - 为背包槽位、箱子槽位、装备槽位补 `IPointerEnterHandler / IPointerExitHandler`。
  - 由它统一解析当前槽位绑定的容器、索引、是否装备位。

- `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
  - Toolbar 不走 `InventorySlotInteraction`，需要单独补悬浮入口。

必要时补读并接入：

- `Assets/YYY_Scripts/UI/Inventory/EquipmentSlotUI.cs`
- `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`

### 1.3 数据载荷设计

不建议继续让 `ItemTooltip.Show()` 只接受 `ItemStack`。推荐调整为“既能吃静态数据，也能吃实例态上下文”的形式。

首选方案：

- 保持 `ItemTooltip` 仍是 UI 组件；
- 新增一个轻量上下文模型，至少包含：
  - `ItemStack stack`
  - `ItemData itemData`
  - `InventoryItem runtimeItem`
  - `int slotIndex`
  - `bool isHotbar`
  - `bool isEquipment`
  - `bool isChestSlot`

如果实现时发现 `ItemTooltip.cs` 文本拼装过重，允许新增一个 helper：

- `Assets/YYY_Scripts/UI/Inventory/ItemTooltipTextBuilder.cs`

当前判断不需要新建额外文件夹；一个 helper 足够。

## 二、Tooltip 内容分层

### 2.1 静态层

先统一静态 Tooltip 出口：

- `ItemTooltip.cs` 不再直接显示 `itemData.description`。
- 统一改为消费 `itemData.GetTooltipText()`。

这样可以立刻让现有这些子类的说明能力生效：

- `SeedData`
- `ToolData`
- `FoodData`
- `PotionData`
- `SaplingData`
- `EquipmentData`

### 2.2 实例态层

在静态层之上追加实例态信息块，优先覆盖两类：

#### A. 工具 / 武器实例耐久

来源：

- `InventoryItem.CurrentDurability`
- `InventoryItem.MaxDurability`
- `InventoryItem.DurabilityPercent`

展示要求：

- Tooltip 文本与现有耐久条口径一致。
- 不与静态配置混写成假数据。

#### B. 种子袋实例信息

来源：

- `SeedBagHelper`
- `InventoryService.OnDayChanged(...)`
- 运行时种子袋实例状态

展示要求：

- 是否已开袋
- 剩余种子数量
- 剩余保质期天数 / 是否已过期

### 2.3 术语层

所有 Tooltip 和确认弹窗统一使用“精力”作为玩家行动资源主口径；如代码内部仍保留 `stamina` / `vitality` 命名，UI 展示层仍需对外统一。

## 三、实例态来源接线设计

### 3.1 背包 / Toolbar

- `InventorySlotUI`、`ToolbarSlotUI` 当前都已经能从 `InventoryService` 拿到 `InventoryItem`。
- 因此这两条链是实例态 Tooltip 的第一落点。

### 3.2 箱子

- `InventorySlotUI.BindContainer(...)` 已支持 `IItemContainer`。
- 后续要确认箱子容器实际能否提供实例态物品；若不能，先降级显示静态 Tooltip，不伪造实例态。

### 3.3 装备位

- `EquipmentSlotUI` 当前只拿 `ItemStack`。
- 若装备系统暂时没有实例态结构，第一轮可先接静态 Tooltip，实例态留到确认装备数据结构后再补。

## 四、掉落链闭环设计

目标文件：

- `Assets/YYY_Scripts/UI/Utility/ItemDropHelper.cs`

当前问题：

- 只支持 `ItemStack`
- 不支持 `InventoryItem`

设计口径：

1. 新增支持实例态掉落的入口。
2. 掉落物生成时带上实例态属性。
3. 再拾取时恢复实例态，而不是回退成普通静态堆叠物。

这部分是 Tooltip 真正稳定成立的必要前置之一。

## 五、食物 / 药水使用效果接线设计

目标文件：

- `Assets/YYY_Scripts/UI/Inventory/ItemUseConfirmDialog.cs`
- `Assets/YYY_Scripts/Service/Player/EnergySystem.cs`

必要时补读：

- `Assets/YYY_Scripts/Service/Player/PlayerInteraction.cs`
- `Assets/YYY_Scripts/Data/Items/FoodData.cs`
- `Assets/YYY_Scripts/Data/Items/PotionData.cs`

设计口径：

1. `ItemUseConfirmDialog` 不再只记录日志。
2. 确认使用后，应直接调用玩家状态系统接口。
3. 对外呈现与 Tooltip、确认弹窗文案保持同一口径。

## 六、推荐落地顺序

### 第一步

接上 Tooltip 入口，至少让背包 / Toolbar / 箱子 / 装备位在悬浮时能稳定触发。

### 第二步

把 `ItemTooltip` 的静态文案统一改为 `GetTooltipText()`。

### 第三步

补实例态 Tooltip 载荷，优先覆盖工具耐久和种子袋信息。

### 第四步

补 `ItemDropHelper` 的实例态掉落链。

### 第五步

让食物 / 药水确认使用后真正修改玩家状态。

### 第六步

统一精力相关术语、Tooltip 文案、确认对话框口径。

## 七、预期修改文件清单

高优先级：

- `Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs`
- `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
- `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
- `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`

第二批：

- `Assets/YYY_Scripts/UI/Inventory/EquipmentSlotUI.cs`
- `Assets/YYY_Scripts/UI/Utility/ItemDropHelper.cs`
- `Assets/YYY_Scripts/UI/Inventory/ItemUseConfirmDialog.cs`

按需补充：

- `Assets/YYY_Scripts/UI/Inventory/ItemTooltipTextBuilder.cs`
- `Assets/YYY_Scripts/Service/Inventory/InventoryService.cs`
- `Assets/YYY_Scripts/Service/Player/EnergySystem.cs`

## 八、当前结论

`1.0.3` 的实现不应从“新造系统”开始，而应从“把现有系统正确接通”开始。真正的关键不是写多少新逻辑，而是把 Tooltip、实例态、掉落链、使用效果和术语这五条线整理成一条连续的用户体验链。
