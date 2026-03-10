# 10.2.2补丁002 审查分析

版本: v1.0  
日期: 2026-03-10  
状态: 待用户审核

## 1. 本轮审查结论摘要

本轮重新审视后，已经可以明确确认以下结论：

1. `10.2.1` 的主要偏差不是某个 if 写错，而是把三条不同验证链误当成了一条：
   - 普通 `PlaceableItemData`
   - `SeedData`
   - `SaplingData`
2. 普通 placeable 的预览体系本来就是“原始格子检测 + 红绿方框”，并且天然支持逐格红判定；如果把“禁放耕地”挂到物品级别的 `CanPlaceAt()`，只会得到整组全红，不能表达“只有部分格子踩了耕地”。
3. 种子虽然接入了放置系统，但它的业务语义从来不是“普通家具放置”；它是“播种”，核心依据应是 `FarmTileData.CanPlant()` 和季节等农田格事实。
4. 树苗虽然也走放置系统，但它本来就有自己的专用验证链，包含冬季、耕地、水域、树距和 Stage0 成长边距等专用规则。
5. `FarmToolPreview` 的 `1.5 x 1.5 footprint` 是农具施工链的规则，不是普通 placeable 预览的统一规则。

## 2. 项目原始设计与当前实现

### 2.1 `V` 是统一授权，不是统一验证

当前代码已经把 `V` 固定为统一授权开关：

- `GameInputManager.cs:235-245`：`V` 切换 `IsPlacementMode`。
- `HotbarSelectionService.cs:124-144`：只有 `IsPlacementMode == true` 时，手持 `isPlaceable` 物品才会进入 `PlacementManager.EnterPlacementMode(...)`。
- `10.2.0改进001/纠正001.md` 也明确写了：`V` 统一授权农具链与 placeable 链，但二者执行器不同。

这说明：

1. `V` 负责“能不能进入这条链”。
2. 但“进入后怎么判绿/判红”仍取决于各自的验证系统。

### 2.2 `PlacementPreview` 的原始职责是标准红/绿格子预览

代码证据：

- `PlacementPreview.cs:329-344`：`UpdateCellStates(List<CellState>)` 会逐格更新红/绿状态。
- `PlacementGridCell.cs:78-94`：单个格子通过 `SetValid(true/false)` 切换颜色。
- `PlacementPreview.cs:501-508`：物品 sprite 只根据 `isAllValid` 做整体 tint，不负责细分每格原因。

这说明：

1. placeable 预览本来就有逐格红判定能力。
2. 如果我们想表达“箱子 2x1 里有 1 格踩耕地、另 1 格没踩”，正确入口是 `CellState` 逐格失效，而不是整物品布尔失败。

### 2.3 普通 placeable 走的是“原始格子检测”

代码证据：

- `PlacementManager.cs:571-597`：`ValidateCurrentPlacementAt(...)` 对普通物品走 `validator.ValidateCells(...)`。
- `PlacementValidator.cs:55-103`：`ValidateCells(...)` 会逐格调用 `ValidateSingleCell(...)`。
- `PlacementValidator.cs:82-103`：单格只检查三件事：楼层、障碍物、水域。

同时，普通 `PlaceableItemData` 默认没有专用放置规则：

- `PlaceableItemData.cs:35-38`：`CanPlaceAt(Vector3 position)` 默认直接返回 `true`。
- 全项目当前只有 `SaplingData.cs:35-43` 重写了 `CanPlaceAt(...)`；`StorageData`、`WorkstationData`、`InteractiveDisplayData`、`SimpleEventData` 都没有重写。

这说明：

1. 普通箱子/工作台等并没有自己的“禁耕地”规则。
2. 如果后续要加“普通 placeable 不能压耕地”，必须从逐格验证层补进去。

### 2.4 种子和树苗从来就是特殊链路

代码证据：

- `PlacementManager.cs:577-586`：`SeedData` 走 `ValidateSeedPlacement(...)`。
- `PlacementManager.cs:580`：`SaplingData` 走 `ValidateSaplingPlacement(...)`。
- `PlacementManager.cs:895-899`：执行时 `SeedData` 还会单独走 `ExecuteSeedPlacement(...)`。

这说明：

1. 种子不是“普通 placeable + 特效”。
2. 树苗也不是“普通 placeable + 树 prefab”。
3. 这两类特殊物在预览上共用 `PlacementPreview`，但在验证与执行上一直都是专用分流。

### 2.5 种子接入放置系统后的原始设计，并不是“取消方框”

前序文档已经有明确结论：

- `10.1.5补丁005/tasks.md`：种子被正式并入放置系统。
- `10.1.5补丁005/memory.md`：用户在验收中明确要求“种子预览方框不该隐藏，和箱子/树苗一样继续保留方框 + 实物预览”。
- `PlacementPreview.cs:190-221`：当前代码也保留了这一点，种子继续走标准 `Show(item)`，只是覆盖 sprite 为 crop 第一阶段。

这说明：

1. 种子的“特殊”在验证语义，不在于另搞一套预览系统。
2. 正确方向不是让种子脱离 `PlacementPreview`，而是让它的验证保持“播种语义”。

## 3. 10.2.1 的偏差点

### 3.1 偏差一：把农田施工链规则上提成了 placeable 通用口径

`10.2.1` 的主线聚焦于：

- `FarmToolPreview.UpdateHoePreview()` 的 `1.5 x 1.5 footprint`
- `PlacementValidator.HasFarmingObstacle(...)`
- 农田作物占位与 `Hoe/WateringCan` 的施工隔离

这些内容对农具施工链是成立的，但它们不等于普通 placeable 的原始放置规则。

用户这轮纠偏的重点正是：

1. 箱子的问题是“普通 placeable 预览必须按格子严格标红”。
2. 不是“所有 isPlaceable 都按农田施工 footprint 统一判断”。

### 3.2 偏差二：把“普通 placeable 禁耕地”误扩大到了全部可放置物

从当前代码结构看，`SeedData`、`SaplingData`、普通 `PlaceableItemData` 的验证入口本来就是分开的。

因此，“除了种子之外都不能落在耕地上”这句话，不能直接翻译成：

1. 所有 `isPlaceable` 都加一个统一的耕地封禁。
2. 然后再给种子打例外补丁。

真正正确的理解应该是：

1. 普通 placeable：新增逐格耕地禁放。
2. 种子：继续走播种语义。
3. 树苗：继续走树苗专用语义。

### 3.3 偏差三：如果把禁耕地规则挂到 `CanPlaceAt()`，预览会天然失真

当前 `PlacementManager.ValidateCurrentPlacementAt(...)` 的物品级回退逻辑是：

- 先跑 `ValidateCells(...)`
- 如果所有格都绿，再检查 `placeableItem.CanPlaceAt(previewPosition)`
- 一旦 `CanPlaceAt()` 返回 false，就把所有格子都改成无效

代码证据：

- `PlacementManager.cs:589-597`

这意味着：

1. `CanPlaceAt()` 只适合表达“整组位置整体无效”。
2. 不适合表达“2x1 箱子只有右半格踩了耕地，因此只把右半格标红”。

所以用户这轮批评“不可交互应该是哪些格子不可放置才标红”是正确的：

- 普通 placeable 的耕地禁放必须进入逐格 `CellState` 验证层。
- 不能继续挂在 `CanPlaceAt()` 或执行层兜底。

### 3.4 偏差四：种子当前仍被多余地拉进了农田障碍规则

当前 `ValidateSeedPlacement(...)` 的实现是：

1. 先解析 `FarmTileData`
2. 检查 `tileData.CanPlant()`
3. 再额外调用 `HasFarmingObstacle(cellCenter, layerIndex, includeCropOccupant: false)`
4. 再做季节校验

代码证据：

- `PlacementValidator.cs:661-692`

从用户这轮纠偏看，这里多出来的农田障碍校验已经超出了“播种语义”的中心需求。因为用户要求的核心是：

1. 耕地本身就是播种合法性的主事实源。
2. 普通 placeable 根本不该落在耕地上，因此种子不该再被普通 placeable 的世界障碍逻辑反向影响。

### 3.5 偏差五：树苗规则不能被普通 placeable 方案吞掉

当前树苗链仍保留大量专用判断：

- `ValidateSaplingPlacement()` 检查冬季。
- `IsOnFarmland()` 检查不能种在耕地上。
- `HasTreeWithinDistance()` 检查与其他树的 Stage0 距离。

代码证据：

- `PlacementValidator.cs:351-397`

因此树苗虽然最后也只显示一个 1x1 方框，但其验证来源不是“普通箱子那套原始格子检测”。

## 4. 对 10.2.2 的稳定结论

基于以上代码事实与前序文档，本轮可以稳定确认：

1. `PlacementPreview` 不需要重写成农田 ghost 体系，它本来就足够表达普通 placeable 的逐格红判定。
2. 普通 placeable 的耕地禁放必须做成逐格验证，不得放到整物品级别兜底。
3. `SeedData` 继续走 `PlacementPreview`，但验证语义必须收窄到“播种”本身。
4. `SaplingData` 继续走 `PlacementPreview`，但保留树苗专用验证，不并入普通 placeable 规则。
5. `FarmToolPreview` 的 `1.5 x 1.5 footprint` 仍然只属于农具施工链。
