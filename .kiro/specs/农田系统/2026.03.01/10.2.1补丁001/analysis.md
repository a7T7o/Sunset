# 10.2.1补丁001 审查分析

版本: v1.0  
日期: 2026-03-10  
状态: 待用户审核

## 1. 审查结论摘要

基于当前仓库代码，五个问题均有明确代码证据，不是用户误报。

本轮核实后的总体判断如下：

1. placeable 的“导航到达”与“放置执行”至少存在两层完成语义，且导航中改点位时会退回通用验证，构成第一项问题的核心证据。
2. 放置系统当前没有把 `FarmTileData.cropController` 纳入统一障碍判断，“箱子可压作物”是真漏洞。
3. `V` 虽然已经统一授权 placeable 与农田工具，但 `Hoe` 在施工模式中仍会因 `farmPreview.HasCrop` 进入 `RemoveCrop`，也仍可能打到树苗并触发挖出。
4. 第四点必须写成“1+8 视觉结构下的检测尺度统一（0.75 / 1 / 1.5）”，这是当前系统真实结构；继续写“严格 3x3”会误导实现。
5. 树苗成长阻挡链只看 `Tree / Rock / Building`，未并入作物占位，用户第五点成立。

另外还有三个必须同步记录的补充风险：

1. 树苗“不能种在耕地上”在当前代码中并未真正落地，因为 `IsOnFarmland()` 仍直接返回 `false`。
2. 农田链当前的楼层判断仍是假实现，`GetCurrentLayerIndex()` 恒返回 `0`。
3. placeable 在 `Navigating` 状态重新点位时会丢失 seed/sapling 专用验证，存在二次绕过风险。

## 2. 审查范围与事实源

### 2.1 主要事实源

- `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
- `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementNavigator.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs`
- `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
- `Assets/YYY_Scripts/Farm/FarmTileManager.cs`
- `Assets/YYY_Scripts/Farm/CropController.cs`
- `Assets/YYY_Scripts/Farm/FarmlandBorderManager.cs`
- `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
- `Assets/YYY_Scripts/Controller/TreeController.cs`

### 2.2 文档事实边界

- 旧 handoff 提到的 `History/2026.03.07-Claude-Cli-历史会话交接/worktree-agent-ae6398ac.md` 当前仓库里不存在，因此不作为事实源。
- `10.2.0改进001/纠正001.md` 只用于确认 `V` 的既有正确语义，不替代当前代码事实。

## 3. 当前系统基线

### 3.1 `V` 已经是统一授权入口

证据：

- `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs:174`：`IsPlacementMode` 默认 `false`。
- `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs:235-245`：按 `V` 切换，关闭时隐藏农田预览，并同步当前手持 placeable 的模式。
- `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs:124-144`：只有 `IsPlacementMode == true` 时，placeable 才进入 `PlacementManager.EnterPlacementMode(...)`。

结论：

- 10.2.1 不是重新发明 `V`，而是修补 `V` 已经接管后的语义断层。

### 3.2 作物占位的真实事实源已经存在

证据：

- `Assets/YYY_Scripts/Farm/CropController.cs:585-596`：作物销毁时会清理 `tileData.cropController`。
- `Assets/YYY_Scripts/Farm/CropController.cs:625-629`、`Assets/YYY_Scripts/Farm/CropController.cs:645-651`：作物初始化时会把自己写回 `tileData.cropController`。

结论：

- “作物是否占位”并不是仓库里没有数据，而是放置链与树成长链都没有统一读取这个事实源。

## 4. 问题一：放置导航与放置成功判定脱钩

### 4.1 代码证据

#### 证据 A：placeable 左键锁定后，先切状态再决定是否导航

- `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs:633-691`
- `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs:728-767`

关键信息：

- `OnLeftClick()` 在 `Preview` 状态下只要 `validator.AreAllCellsValid(currentCellStates)` 返回真，就会进入 `LockPreviewPosition()`。
- `LockPreviewPosition()` 中如果 `navigator.IsAlreadyNearTarget(...)` 为真，就直接 `ExecutePlacement()`；否则开始导航。

#### 证据 B：导航目标点使用 `ClosestPoint` 边界点

- `Assets/YYY_Scripts/Service/Placement/PlacementNavigator.cs:123-160`

关键信息：

- `CalculateNavigationTarget()` 以 `previewBounds.ClosestPoint(playerCenter)` 为导航目标。
- 如果玩家中心已在 `previewBounds` 内部，会改找最近边界点，但仍保持“边界最近点”语义。

#### 证据 C：导航到达又是另一层阈值语义

- `Assets/YYY_Scripts/Service/Placement/PlacementNavigator.cs:212-220`
- `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs:827-845`
- `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs:892-896`

关键信息：

- `PlacementNavigator.CheckReached()` 用“玩家中心到预览边缘的距离是否小于触发阈值”作为到达。
- `PlayerAutoNavigator` 自己也有一套“到目标最近点 / followStopRadius” 的接近语义。

#### 证据 D：导航中改点位会退回通用验证

- `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs:657-683`

关键信息：

- `Navigating` 状态点击新位置时，重新验证调用的是 `validator.ValidateCells(cellCenter, placementPreview.GridSize, playerTransform)`。
- 这里没有沿用 `SeedData` 或 `SaplingData` 的专用验证分支。

### 4.2 审查判断

第一项问题成立，而且不是单点 bug，而是三类语义叠加后的系统性断层：

1. `PlacementManager` 有自己的“锁定即进入放置流程”语义。
2. `PlacementNavigator` 有自己的“到边界即算到达”语义。
3. `PlayerAutoNavigator` 还有一层“接近到最近点即可交互”的停止语义。

再加上导航中改点位时验证链回退成通用 `ValidateCells()`，这意味着：

- 初始锁定可能是专用验证。
- 中途改点可能退化成通用验证。
- 到达成功与最终执行又不一定共享完全同一套条件。

### 4.3 对主线的影响

- 这是当前主线里最容易引发“原地打转 / 到了不放 / 没到却放”的根因之一。
- 后续实现不能只调一个距离阈值，必须把“导航目标、到达判断、最终执行、改点重验”统一到同一套事实链。

## 5. 问题二：放置系统漏检作物阻挡

### 5.1 代码证据

#### 证据 A：placeable 的 `HasObstacle()` 只看碰撞体、树苗、箱子

- `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs:139-165`

关键信息：

- `HasObstacle()` 先用 `OverlapBoxAll(cellCenter, 0.9 x 0.9)` 看标签。
- 然后只额外检查 `HasTreeAtPosition(cellCenter, 0.5f)` 和 `HasChestAtPosition(cellCenter, 0.5f)`。
- 没有任何作物占位检查。

#### 证据 B：农田障碍检查同样不读取作物占位

- `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs:180-221`

关键信息：

- `HasFarmingObstacle()` 读取 `FarmTileManager.FarmingObstacleCheckRadius`、标签白名单与黑名单。
- 之后仍只补树苗和箱子检测。
- 同样没有把 `FarmTileData.cropController` 纳入判断。

#### 证据 C：作物占位明明存在

- `Assets/YYY_Scripts/Farm/CropController.cs:585-596`
- `Assets/YYY_Scripts/Farm/CropController.cs:625-629`
- `Assets/YYY_Scripts/Farm/CropController.cs:645-651`

### 5.2 审查判断

第二项问题完全成立：

- 当前系统已经有作物占位事实。
- 但 placeable 的障碍验证没有接入这份事实。
- 因此“箱子可压作物”不是边缘漏判，而是当前链路天然没有这条约束。

### 5.3 对主线的影响

- 后续实现必须建立“统一占位查询”，而不是继续把作物当作农田子系统内部细节。
- 否则后面即使修了箱子，树苗成长、施工工具等仍会各自重复漏判。

## 6. 问题三：放置模式下 `Hoe` 仍混入破坏/清作物语义

### 6.1 代码证据

#### 证据 A：`V` 开启后 `Hoe` 的入队仍会转成 `RemoveCrop`

- `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs:2604-2611`

关键信息：

- `TryEnqueueFarmTool()` 里，`Hoe` 的 `FarmActionType` 不是固定 `Till`。
- 当 `farmPreview.HasCrop` 为真时，会直接选 `FarmActionType.RemoveCrop`。

#### 证据 B：树苗被锄头命中仍会直接挖出

- `Assets/YYY_Scripts/Controller/TreeController.cs:790-817`
- `Assets/YYY_Scripts/Controller/TreeController.cs:898-910`

关键信息：

- `TreeController.OnHit()` 中，只要是阶段 0 树苗且工具匹配，就直接 `HandleSaplingDigOut(ctx)`。
- 当前没有看到“施工模式下禁止这条命中链”的保护。

### 6.2 审查判断

第三项问题成立，而且要分成两个层面理解：

1. 农田队列层：`Hoe` 在施工模式里仍可因 `HasCrop` 自动切成 `RemoveCrop`。
2. 世界命中层：只要锄头动画还能命中树苗，树苗仍可走 `HandleSaplingDigOut()`。

这说明当前 `V` 只是统一了授权入口，但没有把“施工语义”和“破坏语义”真正隔离干净。

### 6.3 对主线的影响

- 后续实现不能只改队列类型，还要一起审输入分流和命中分流。
- 否则会出现“队列不再清作物，但锄头动画依旧挖树苗”的半修状态。

## 7. 问题四：必须采用“1+8 + 0.75/1/1.5”口径

### 7.1 代码证据

#### 证据 A：边界管理只处理中心周围 8 邻位

- `Assets/YYY_Scripts/Farm/FarmlandBorderManager.cs:193-205`

关键信息：

- `UpdateBordersAround()` 循环 `dx=-1..1`、`dy=-1..1`，但 `dx == 0 && dy == 0` 时直接跳过中心。
- 这说明边界结构天然是“中心格 + 周围 8 邻位”的视觉组织，而不是单纯“把 9 格一锅端”。

#### 证据 B：农田障碍检测整体尺度是 `1.5`

- `Assets/YYY_Scripts/Farm/FarmTileManager.cs:37-38`
- `Assets/YYY_Scripts/Farm/FarmTileManager.cs:54-57`

关键信息：

- `farmingObstacleCheckRadius = 1.5f`。
- 当前这个值被作为农田障碍检测的整体尺度暴露给外部。

#### 证据 C：树与箱子的 AABB 半边尺度是 `0.75`

- `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs:248`
- `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs:276`

关键信息：

- 静态树和箱子的无碰撞体检测都用 `0.75f` 作为半边尺度。
- 这与 `1.5f` 的整体盒子尺度是一一对应关系。

#### 证据 D：未耕地不可交互时确实显示红色 `1+8` 预览

- `Assets/YYY_Scripts/Farm/FarmToolPreview.cs:618-726`
- `Assets/YYY_Scripts/Farm/FarmToolPreview.cs:753-769`

关键信息：

- 未耕地但不可交互时，`FarmToolPreview` 会走红色 `1+8` tile 预览分支。
- shader 叠色也区分了绿色可交互与红色不可交互。

### 7.2 审查判断

第四项的正确写法必须是：

> 1+8 视觉结构下的检测尺度统一（0.75 / 1 / 1.5）

不能继续写“严格 3x3”的原因：

1. 当前视觉组织是中心格 + 8 邻边界，不是单独强调九宫格填充。
2. 当前逻辑里真正落地的是三层尺度关系，而不是只有一个“3x3 开关”。

### 7.3 额外注意

- `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs:189` 的注释把 `OverlapBoxAll` 第二参数写成了 half extents，但当前实际代码语义是把 `1.5` 当完整盒子尺寸在用，注释口径容易误导后续实现。

## 8. 问题五：树苗成长未并入作物占位

### 8.1 代码证据

#### 证据 A：成长阻挡标签只有树、石头、建筑

- `Assets/YYY_Scripts/Controller/TreeController.cs:95`

#### 证据 B：成长判断只走自己的四方向阻挡检测

- `Assets/YYY_Scripts/Controller/TreeController.cs:559-569`
- `Assets/YYY_Scripts/Controller/TreeController.cs:578-623`
- `Assets/YYY_Scripts/Controller/TreeController.cs:632-640`

关键信息：

- `CanGrowToNextStage()` 只依赖 `CheckGrowthMargin(...)`。
- `CheckGrowthMargin(...)` 逐方向调用 `HasObstacleInDirection(...)`。
- 当前成长阻挡源仍是标签碰撞语义，没有接入作物占位。

### 8.2 审查判断

第五项问题成立：

- 树苗成长链是独立的成长空间检测链。
- 农作物占位事实没有并入这条链。
- 即使 placeable 放置链后续修好了作物占位，树成长若不一起修，仍会形成另一条独立漏洞。

## 9. 三个补充风险

### 9.1 `IsOnFarmland()` 仍未落地

证据：

- `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs:335-339`
- `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs:363-367`

判断：

- 目前树苗专用验证虽然写了“检查是否在耕地上”，但实际永远返回 `false`。
- 因此“树苗不能种在耕地上”在当前仓库里并未真正成立。

### 9.2 `GetCurrentLayerIndex()` 恒返回 `0`

证据：

- `Assets/YYY_Scripts/Farm/FarmTileManager.cs:218-223`

判断：

- 现阶段多层楼层语义在农田链里仍是假实现。
- 任何依赖当前层的耕地 / 作物 / 成长判断，都有被错误固定到 0 层的风险。

### 9.3 导航中改点位丢失专用验证

证据：

- `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs:657-683`

判断：

- 这是本轮额外发现的高风险点。
- 即使初始进入时用的是 `SeedData` / `SaplingData` 专用验证，中途改点位也可能被 `ValidateCells()` 放宽。

## 10. 本轮审查的总体结论

10.2.1 需要的不是“继续补几个 if”，而是一轮围绕统一占位事实源、统一到达语义、统一施工边界的定向修补。

当前最关键的三个技术焦点是：

1. 统一 placeable 的导航目标、到达判定、最终执行和改点重验。
2. 把 `FarmTileData.cropController` 提升为放置链和树成长链都能读取的统一占位事实。
3. 把 `V` 开启后的 `Hoe` 彻底收敛为施工语义，不再混入清作物与挖树苗。

## 11. 本轮未执行项

- 未修改任何业务代码。
- 未运行 Unity 编译或测试。
- 未修改任何场景、Prefab、ScriptableObject、Inspector。

## 12. 问题四最终确认：统一到 `1.5 x 1.5` footprint

### 12.1 可以这样抽象，而且应该这样抽象

本轮进一步确认后，第四点可以正式落成一句话：

> 农田交互的统一判断基线，就是“基于放置系统格心坐标的 `1.5 x 1.5` 方框 footprint”。

这里的三层口径关系应理解为：

- `1`：逻辑中心格。
- `1+8`：视觉上由中心格驱动出来的邻域结构。
- `1.5 x 1.5`：这一整套交互足迹在连续空间中的物理检测盒。
- `0.75`：同一检测盒的 half extent。

因此用户说“我的需求就是这个 1.5 边长方框”，和当前审查结论是一致的。

### 12.2 当前代码坐标是否已经统一

结论：**大部分已经统一到放置系统格心坐标，但还没有百分之百收干净。**

证据如下：

1. `PlacementGridCalculator.GetCellCenter(...)` 明确把格心定义为 `Floor(pos) + 0.5`，见 `Assets/YYY_Scripts/Service/Placement/PlacementGridCalculator.cs:18-25`。
2. `GameInputManager.UpdatePreviews()` 先把鼠标世界坐标对齐到 `PlacementGridCalculator.GetCellCenter(rawWorldPos)`，见 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs:348`。
3. 农田工具预览接着把这个 `alignedPos` 再转换成 `tilemaps.WorldToCell(alignedPos)` 后传给 `FarmToolPreview`，见 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs:436-448`。
4. `FarmToolPreview` 在核心判定前，会通过 `GetCellCenterWorld(layerIndex, cellPos)` 还原格子中心，再把这个中心交给 `PlacementValidator.HasFarmingObstacle(cellCenter)`，见 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs:410`、`Assets/YYY_Scripts/Farm/FarmToolPreview.cs:828`。
5. 正常路径下，`GetCellCenterWorld(...)` 会直接走 `tilemaps.GetCellCenterWorld(cellPos)`，见 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs:1491-1496`，这说明农田预览主路径已经和格心坐标对齐。

### 12.3 当前还没完全统一的地方

仍有一个回退路径口径不够干净：

- `FarmToolPreview.GetCellCenterWorld(...)` 在 fallback 分支里写的是 `PlacementGridCalculator.GetCellCenter(new Vector3(cellPos.x + 0.5f, cellPos.y + 0.5f, 0))`，见 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs:1500`。

这个写法在当前整数格场景下通常还能落回正确格心，但它的表达口径已经不再干净，因为：

1. 统一抽象应是“cell index -> 格心”的单一步骤。
2. 这里却写成了“先加 0.5，再交给格心计算器再算一次”。

所以最终判断是：

- **方向上已经统一**：主路径确实围绕放置系统格心和 `1.5 x 1.5` footprint。
- **表达上还没完全统一**：至少这个 fallback 仍需要在实现阶段清理成同一套格心口径。
