# 10.2.2补丁002 设计方案

版本: v1.0  
日期: 2026-03-10  
状态: 待用户审核

## 1. 设计目标

`10.2.2补丁002` 的设计目标不是重写放置系统，而是在保持现有架构的前提下，把以下四条链重新拆清楚：

1. 普通 placeable 的原始格子放置链。
2. 种子的播种链。
3. 树苗的专用放置链。
4. `Hoe / WateringCan` 的农田施工链。

## 2. 设计总原则

### 2.1 保留现有系统分工，不重做管理器

本轮继续保留：

- `GameInputManager`：统一授权、输入分流。
- `PlacementManager`：placeable 状态机与执行。
- `PlacementPreview`：placeable 的标准格子预览。
- `FarmToolPreview`：农具施工预览。
- `PlacementValidator`：不同物品类型的验证分发。

### 2.2 修正重点不是“谁能进入放置模式”，而是“进入后按哪条验证链判红”

`V` 的统一授权语义不变，本轮不再讨论“是不是由 V 控制”。

本轮真正要修的是：

1. 普通 placeable 的耕地禁放要进逐格红判定。
2. 种子不能被普通 placeable 的禁耕地规则带死。
3. 树苗继续保留专用验证，不被普通 placeable 规则吞掉。

## 3. 四类交互的正式拆分

### 3.1 A 类：普通 placeable

范围：

- `StorageData`
- `WorkstationData`
- `InteractiveDisplayData`
- `SimpleEventData`
- 以及后续所有未定义专用验证链的常规 `PlaceableItemData`

正式规则：

1. 使用 `PlacementPreview` 标准预览。
2. 使用 `ValidateCells(...)` 逐格验证。
3. 在逐格验证层新增“已耕地格不可落普通 placeable”的规则。
4. 某一格踩中已耕地，该格标红；其他未踩中的格维持原判定结果。
5. 只要有任一格红，就整组位置不可放，但预览不能因此丢失逐格信息。

### 3.2 B 类：种子

范围：

- `SeedData`

正式规则：

1. 继续使用 `PlacementPreview` 标准预览，不回退到旧种子专用 ghost 系统。
2. 验证入口继续是 `ValidateSeedPlacement(...)`。
3. 验证语义聚焦在：
   - 当前格是否存在耕地数据；
   - `FarmTileData.CanPlant()` 是否为真；
   - 季节是否匹配。
4. 不把普通 placeable 的“禁放耕地”规则套到种子上。
5. 不把农具施工链的 `HasFarmingObstacle(...)` 当作种子的主要合法性来源。

设计理由：

1. 种子不是“家具放置”，而是“播种”。
2. 既然耕地上原则上不允许其他普通 placeable 合法存在，种子的合法性应由农田格状态决定，而不是被世界放置障碍二次覆盖。

### 3.3 C 类：树苗

范围：

- `SaplingData`

正式规则：

1. 继续使用 `PlacementPreview` 标准预览。
2. 验证入口继续是 `ValidateSaplingPlacement(...)`。
3. 继续保留以下专用规则：
   - 冬季禁种；
   - 耕地禁种；
   - 水域禁种；
   - 树距 / Stage0 成长边距限制；
   - 既有障碍物检查。
4. 树苗不并入普通 placeable 的耕地逐格禁放方案。

设计理由：

1. 树苗的业务语义本质是“树木系统入口”，而不是普通家具放置。
2. 用户已经明确把树苗视为和种子一样的“特殊可放置物”。

### 3.4 D 类：农具施工

范围：

- `Hoe`
- `WateringCan`

正式规则：

1. 继续使用 `FarmToolPreview`。
2. 继续保留 `1.5 x 1.5 footprint`、`1+8` 边界视觉、`0.75` half extent 这套施工口径。
3. 不再把这套施工口径向普通 placeable 泛化。

## 4. 预览层与验证层的正确联动方式

### 4.1 普通 placeable 的禁耕地必须在 `CellState` 层表达

当前 `PlacementPreview` 已经有逐格红/绿显示能力，因此不需要新建视觉系统。

正确联动方式应为：

1. 先算出当前物品占用的所有格子。
2. 对每个格子分别判断：
   - 原有 Layer / Obstacle / Water 条件；
   - 新增“该格是否已耕地且本物品属于普通 placeable”。
3. 将逐格结果回传给 `PlacementPreview.UpdateCellStates(...)`。

### 4.2 不把普通 placeable 的禁耕地放进 `CanPlaceAt()`

原因：

1. `CanPlaceAt()` 是整物品级布尔口径。
2. 当前 `PlacementManager` 在 `CanPlaceAt == false` 时会把所有格子一起改成无效。
3. 这与用户要求的“哪些格子不可放置才标红”冲突。

因此：

- `CanPlaceAt()` 只保留给真正的整组规则；
- 普通 placeable 的耕地禁放必须进逐格验证层。

### 4.3 种子与树苗仍由专用验证返回最终布尔

因为种子和树苗都是 1x1，且它们的失败原因本来就不是“多格里某格踩到了耕地”这种普通家具语义，所以继续走单条 `CellState` 即可。

## 5. 10.2.2 推荐修补方向

### 5.1 推荐方向一：显式增加“普通 placeable”判别

需要明确一件事：

- `isPlaceable` 不等于“普通 placeable”。

因此后续实现建议引入清晰分流：

1. `SeedData`：特殊。
2. `SaplingData`：特殊。
3. 其他 `PlaceableItemData`：普通 placeable。

### 5.2 推荐方向二：为普通 placeable 增加逐格耕地禁放判定

可以在逐格验证阶段新增一条判断：

```text
若当前物品属于普通 placeable，且该占用格对应 FarmTileData.isTilled == true，
则该格无效。
```

这条规则只影响普通 placeable，不影响：

1. 种子；
2. 树苗；
3. 农具施工链。

### 5.3 推荐方向三：收窄种子验证口径

后续实现建议把种子验证重新收敛到农田格事实源：

1. `TryResolveTileAtWorld(...)`
2. `tileData != null`
3. `tileData.CanPlant()`
4. 季节匹配

如无额外代码事实证明需要，暂不把普通 placeable / 农具施工链的障碍逻辑继续叠到种子上。

### 5.4 推荐方向四：树苗维持专用验证，不参与普通 placeable 修补

树苗后续若要进一步收口，应继续围绕：

1. `ValidateSaplingPlacement()`
2. `IsOnFarmland()`
3. `HasTreeWithinDistance()`
4. `TreeController` 的成长距离参数

不应被 10.2.2 的普通 placeable 修补顺手改坏。

## 6. 对用户截图场景的正式解释

用户截图展示的是一个典型的普通 placeable 边界问题：

1. 箱子属于普通 placeable。
2. 它的预览本来应该通过占用格逐格判红。
3. 如果箱子 2x1 的一部分压在耕地上，正确反馈不是：
   - 继续整体绿色；
   - 或整组无差别全红；
4. 正确反馈应是：
   - 踩到耕地的那部分格子红；
   - 未踩到耕地的那部分格子保留原判定结果。

这正是 `10.2.2` 与 `10.2.1` 的关键差异。

## 7. 本轮设计不做的事

1. 不改 `V` 的统一授权语义。
2. 不把 placeable 预览改成 `FarmToolPreview` 的 tile ghost 体系。
3. 不新建新的总管理器。
4. 不直接修改代码，等用户先审核本轮文档。
