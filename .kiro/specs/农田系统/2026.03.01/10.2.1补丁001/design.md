# 10.2.1补丁001 设计方案

版本: v1.0  
日期: 2026-03-10  
状态: 待用户审核

## 1. 设计目标

在不偏离当前仓库既有架构的前提下，为 10.2.1 提供一套“先修边界、再补统一事实源”的最小可执行设计，解决以下问题：

1. 统一 placeable 的导航目标、到达判定、放置执行判定。
2. 把作物占位并入放置链与树成长链。
3. 把 `V` 开启后的 `Hoe` 收敛到纯施工语义。
4. 用“1+8 + 0.75/1/1.5”统一检测口径，避免视觉与逻辑分叉。
5. 在本轮实现里同时兜住 `IsOnFarmland()`、楼层语义、导航中改点位验证退化这三个补充风险。

## 2. 总体策略

### 2.1 不重构大架构，优先做三条统一

本轮建议坚持最小兼容修复，不新增新的大管理器，优先做以下三条统一：

1. 统一占位事实源。
2. 统一 placeable 成功语义。
3. 统一施工模式语义边界。

### 2.2 继续保留现有职责分层

- `GameInputManager`：继续负责输入分流与 `V` 授权。
- `HotbarSelectionService`：继续负责热栏切换与 placeable 进入条件。
- `PlacementManager`：继续负责 placeable 状态机。
- `PlacementNavigator` / `PlayerAutoNavigator`：继续负责移动与接近。
- `FarmTileManager`：提升为农田占位事实源。
- `TreeController`：保留成长执行器，但成长阻挡需要读取农田占位事实。

## 3. 设计一：placeable 成功语义统一

### 3.1 目标

确保以下四个动作共享同一套事实链：

1. 初始点位验证。
2. 锁定后的导航目标计算。
3. 到达判定。
4. 最终放置前的重验。

### 3.2 设计方案

#### 方案 A：在 `PlacementManager` 内引入“锁定快照 + 同源重验”

保留当前 `PlacementSnapshot` 思路，但补齐以下约束：

1. 锁定时保存“当前物品类型 + 目标 cellCenter + gridSize + 专用验证类型”。
2. 导航完成后，不直接盲目 `ExecutePlacement()`，而是对锁定快照做一次同源重验。
3. 导航中重新点位时，不允许退回裸 `ValidateCells()`，而是回到“按当前 item 类型选择专用验证”的统一入口。

#### 方案 B：让到达判定与放置判定都围绕同一份 `previewBounds`

保留 `ClosestPoint` 思路，但要把最终执行前的“近到可以放”判断也绑定到同一份 `previewBounds` 与同一份锁定快照。

### 3.3 推荐落地

推荐方案：

1. 在 `PlacementManager` 内增加一个统一入口，例如“重新验证当前锁定请求”。
2. 初始锁定和导航中改点位都走这一个入口。
3. `OnNavigationReached()` 只负责触发这次重验，只有重验通过才执行放置。

### 3.4 预期收益

- 消除“导航认为到了 / 放置却认为没到”与“改点位后验证变宽松”的双重问题。
- 第一项问题可以在现有状态机内完成修复，不需要重写导航系统。

## 4. 设计二：统一作物占位事实源

### 4.1 目标

让放置链、树成长链、树苗禁止种在耕地上等语义，都能读取同一份农田占位事实。

### 4.2 设计方案

推荐把 `FarmTileData.cropController` 正式提升为统一占位事实源，并由 `FarmTileManager` 暴露查询接口。

建议新增或补齐以下查询能力：

1. 指定楼层、指定 `cellPos` 是否存在作物占位。
2. 指定世界坐标对应的农田格是否存在作物占位。
3. 指定一个 AABB / 目标点位周围是否被作物占位阻挡。

### 4.3 推荐接口形态

可选的最小接口方向：

```csharp
bool HasCropOccupant(int layerIndex, Vector3Int cellPos);
bool HasCropOccupantAtWorld(int layerIndex, Vector3 worldPos);
bool HasCropOccupantWithinFootprint(int layerIndex, Vector3 center, Vector2 size);
```

这里不要求最终函数名完全一致，重点是把作物占位从 `CropController` 私有细节，提升为 `FarmTileManager` 的显式查询能力。

### 4.4 接入点

1. `PlacementValidator.HasObstacle()`：placeable 检测时并入作物占位。
2. `PlacementValidator.HasFarmingObstacle()`：需要农田链统一避让作物时，也并入同一事实源。
3. `PlacementValidator.IsOnFarmland()`：不再写 `TODO + false`，而是基于 `FarmTileManager` 的 tile / layer 数据返回真实结果。
4. `TreeController` 成长检测：在四方向阻挡之外，补一层对作物占位的读取。

## 5. 设计三：施工模式下 `Hoe` 的语义隔离

### 5.1 目标

`V` 开启后，`Hoe` 只承担施工语义，不再顺手清作物和挖树苗。

### 5.2 设计方案

#### 阶段一：队列语义隔离

在 `GameInputManager.TryEnqueueFarmTool()` 处做第一层隔离：

- `V` 开启且当前工具是 `Hoe` 时，禁止再根据 `farmPreview.HasCrop` 自动切成 `RemoveCrop`。
- 如果命中作物格，可选择：
  1. 直接判定为施工无效；
  2. 或保留为红色预览，不允许入队。

推荐选择第 1 种语义，即“施工模式下作物格不可施工”，这样更符合用户对隔离的要求。

#### 阶段二：世界命中隔离

需要在锄头普通命中链与施工链之间再加一道闸门：

- 当 `V` 开启且当前进入的是农田施工链时，屏蔽对树苗 `HandleSaplingDigOut()` 的命中流入。
- 不建议修改 `TreeController` 的“普通挖树苗能力”，而是应该在输入 / 命中分流层面控制“施工模式下不把这次点击当成普通命中”。

### 5.3 推荐落地

推荐把“施工模式下的农田动作优先级”提到 `GameInputManager`，让同一次左键在施工模式下优先走施工验证；只有未进入施工链时，才允许下沉到普通工具命中。

这样可以同时解决：

1. `RemoveCrop` 混入施工链。
2. 锄头动画在施工模式里挖树苗。

## 6. 设计四：统一“1+8 + 0.75/1/1.5”口径

### 6.1 目标

把当前已经存在但散落在代码里的尺度关系，整理成可维护的统一口径。

### 6.2 统一口径

本轮建议显式固定以下三层语义：

1. `1`
   - 单格中心格语义。
   - 代表当前耕作 / 放置的中心格。

2. `0.75`
   - AABB 半边尺度。
   - 用于树、箱子等无碰撞体或静态补充检测。

3. `1.5`
   - 当前农田障碍检测整体盒子尺度。
   - 与 `0.75` 一一对应。

### 6.3 设计要求

1. 注释、文档、变量名不能再把 `1.5` 误解释成 half extents。
2. `FarmlandBorderManager`、`FarmToolPreview`、`PlacementValidator` 的相关口径要对齐为同一组表述。
3. 审查和实现都统一写“1+8 视觉结构”，不再写“严格 3x3”。

### 6.4 推荐实现细节

建议把这组三个尺度抽成带语义的命名常量，至少做到：

- 不再在多个文件中裸写 `0.75f`、`1.5f` 而不说明语义。
- 未来如果需要调整检测尺度，可以以统一命名常量为中心做变更。

## 7. 设计五：树苗成长并入作物占位

### 7.1 目标

让树成长空间检测不只看 `Tree / Rock / Building`，同时把农作物占位纳入阻挡来源。

### 7.2 设计方案

建议保留 `TreeController.CheckGrowthMargin()` 的四方向结构，但在 `HasObstacleInDirection(...)` 的流程中补一层“农田占位查询”。

可选方向：

1. 在 `TreeController` 内部直接调用 `FarmTileManager` 的占位查询接口。
2. 或把“成长阻挡查询”抽到更靠近服务层的位置，再由 `TreeController` 调用。

本轮推荐第 1 种最小方案，原因是：

- 改动面更小。
- 当前目标是定向修补，而不是重做树服务架构。

### 7.3 语义要求

树成长被作物阻挡时，需要和现有 `Tree / Rock / Building` 阻挡表现一致：

- 成长失败，但不直接销毁树。
- 调试信息应能区分“是作物阻挡了成长”。

## 8. 三个补充风险的设计处理

### 8.1 `IsOnFarmland()` 必须真实化

当前 `ValidateSaplingPlacement()` 已经调用了 `IsOnFarmland(position)`，但后者直接返回 `false`。

本轮设计要求：

- 不新增第二套耕地判断。
- 直接复用 `FarmTileManager` 的 tile 数据，返回真实“该位置是否已是耕地 / 可视为农田”的判断。

### 8.2 楼层语义优先“减少依赖假值”

由于 `GetCurrentLayerIndex()` 仍恒返回 `0`，本轮设计建议：

1. 能从当前已知上下文直接传入 `layerIndex` 的链路，不再回头依赖 `GetCurrentLayerIndex()` 推断。
2. 只有确实缺上下文时，才走现有默认逻辑。

这能在不立刻重做楼层系统的前提下，先减少假值污染范围。

### 8.3 导航中改点位必须保留 item-specific 验证

本轮设计要求把 `Navigating` 状态重新点位的验证升级为“按当前 item 类型回到统一验证入口”。

这条要求和“统一成功语义”必须绑定实现，不能拆开做。

## 9. 建议实施顺序

### 9.1 第一阶段：事实源与验证入口统一

1. 补 `FarmTileManager` 作物占位查询。
2. 补 `PlacementValidator.IsOnFarmland()`。
3. 把 `PlacementManager` 的初始锁定与导航中改点位统一到同一验证入口。

### 9.2 第二阶段：placeable 成功语义统一

1. `OnNavigationReached()` 改成“同源重验后再执行”。
2. 统一到达与执行使用的 `previewBounds / snapshot` 语义。

### 9.3 第三阶段：施工模式隔离

1. 修 `TryEnqueueFarmTool()` 的 `RemoveCrop` 分流。
2. 修施工模式下锄头命中树苗的问题。

### 9.4 第四阶段：树成长并入作物占位

1. 在 `TreeController` 成长检测中接入占位事实。
2. 验证成长受阻反馈与现有阻挡反馈保持一致。

## 10. 本轮设计不做的事

- 不在 10.2.1 里新增新的总管理器。
- 不重写 `PlacementManager` 整套状态机。
- 不把多层楼层系统在本轮完全做完。
- 不在当前审核未通过前抢先提交实现代码。

## 11. 问题四实现基线最终确认

本轮补充确认后，问题四的实现基线正式固定为：

1. 所有“是否可耕地”的核心判断都围绕一个 `1.5 x 1.5` footprint。
2. 这个 footprint 的锚点统一使用 `PlacementGridCalculator` 定义的格心坐标。
3. `0.75` 只是该 footprint 的 half extent 表达，不再单独作为另一套业务口径存在。
4. `1+8` 只保留为视觉与邻域拓扑表达，不再与物理检测盒争夺“谁才是主定义”。
5. 实现阶段需要顺手清理农田链中仍不够干净的 fallback 格心写法，确保“放置链坐标”和“农田链坐标”最终收束到同一来源。
