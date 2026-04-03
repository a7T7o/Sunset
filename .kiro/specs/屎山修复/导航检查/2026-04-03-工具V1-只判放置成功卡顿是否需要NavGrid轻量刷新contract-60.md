# 工具-V1：只判“放置成功卡顿”是否需要新的 NavGrid 轻量刷新 contract

先接受当前已经被用户最新反馈压实的事实：

1. 当前不是继续修整条导航主链。
2. 当前也不是继续追 `Primary traversal`。
3. 用户最新确认后，放置相关卡顿已经进一步收窄：
   - 现在主要剩下的是“放置成功那一下会明显卡”。
   - 不是“导航开始走路整体又坏了”。
4. 农田线已经自己复盘过一次，当前怀疑点更像：
   - placeable / sapling 放置成功后，误用了 `NavGrid2D` 的重型全量刷新入口。

---

## 一、这轮唯一主刀

这轮只做一件事：

**只读判断：当前“放置成功时卡顿”到底是不是因为业务侧正在把 placeable 成功后的变化，错误地接到了 `NavGrid2D.RefreshGrid()` 这条重型 contract；以及如果是，工具-V1是否应该提供一个更轻的 runtime refresh contract。**

这轮不是：

1. 继续修 `Primary.unity`
2. 继续修 `PrimaryTraversalSceneBinder.cs`
3. 继续回头修 `PlayerAutoNavigator` 走位
4. 继续扩大到 crowd / transition / water / props
5. 直接替农田线吞业务修复

---

## 二、你必须先消费的现有事实

### 1. 导航当前 own 边界

你当前仍按 `-59` 保持 `PARKED`，只保留这三脚本的精确响应权：

1. `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
2. `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
3. `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`

当前不要把父线程 `Primary traversal`、也不要把 `PlayerAutoNavigator.cs` 带回本轮。

### 2. 农田线这边已经自己压实的代码证据

当前最硬的现场证据有三组：

1. `ChestController.cs`
   - `Start()` 里放置后会 `StartCoroutine(RequestNavGridRefreshDelayed())`
   - 下一帧 `RequestNavGridRefresh()` 直接 `NavGrid2D.OnRequestGridRefresh?.Invoke()`
   - 命中位置：
     - `Assets/YYY_Scripts/World/Placeable/ChestController.cs:615`
     - `Assets/YYY_Scripts/World/Placeable/ChestController.cs:624`
     - `Assets/YYY_Scripts/World/Placeable/ChestController.cs:632`
2. `NavGrid2D.cs`
   - `OnRequestGridRefresh` 当前直接订到 `RefreshGrid()`
   - `RefreshGrid()` 当前仍是 `RefreshExplicitObstacleSources() + RebuildGrid()`
   - 命中位置：
     - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs:61`
     - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs:111`
3. `TreeController.cs`
   - 农田线已经切掉了一部分明显不该刷整图的情况：
   - `Stage 0` 无碰撞阶段现在通过 `ShouldSyncColliderShapeForCurrentPresentation()` 避免误刷整张 NavGrid
   - 命中位置：
     - `Assets/YYY_Scripts/Controller/TreeController.cs:469`
     - `Assets/YYY_Scripts/Controller/TreeController.cs:1921`

### 3. 农田线历史上确实碰过的导航触点

这次也请你知晓，避免你误以为我们完全没碰过导航接线：

1. 我这条农田线历史上碰过：
   - `GameInputManager.cs`
   - `PlayerAutoNavigator.cs`
2. 主要改动语义是：
   - 箱子改成 `pending auto interaction`
   - 走近到位后再补交互
   - 箱子 stop radius 收紧
3. 但这轮“放置成功才卡”不是在这组触点上新增出来的；
4. 农田线这次为卡顿最新真正动过的，只是：
   - `PlacementManager.cs`
   - `TreeController.cs`
5. 这轮没有重开：
   - `NavGrid2D.cs`
   - `PlayerAutoNavigator.cs`
   - `Primary.unity`
   - `PrimaryTraversalSceneBinder.cs`

---

## 三、你这轮允许做什么

### 允许

1. 只读检查 `NavGrid2D.cs` 当前 `RefreshGrid()` 的 contract 粒度
2. 只读判断：
   - 现有 contract 已足够，只是调用方用错了
   - 还是现有 contract 过粗，确实缺一个轻量 runtime refresh 入口
3. 如果你判断存在 contract gap，只允许给出一个最小接口面设计

### 禁止

1. 不准直接 reopen 写代码
2. 不准扩回 `Primary traversal`
3. 不准把 `PlayerAutoNavigator.cs` 带回本轮
4. 不准把“placeable 成功卡顿”泛化成“整个导航都要重做”
5. 不准把调用方 misuse 也硬甩成导航侧 owner 问题

---

## 四、你这轮要回答的唯一问题

请只回答下面这个问题：

**对“placeable / chest / sapling 放置成功后会触发一次明显卡顿”这件事，当前更正确的责任判断是：**

1. `NavGrid2D` 现有 contract 足够，业务侧不该在这类 runtime 放置上直接打 `RefreshGrid()`，所以应由农田线继续收口；
2. 还是 `NavGrid2D` 现有 contract 过粗，工具-V1应该最小新增一个轻量 contract，专门承接这种 placeable 成功后的动态障碍刷新；
3. 如果是第 2 类，这个 contract 最小应该长什么样，且为什么不能只靠调用方继续绕开。

---

## 五、完成定义

### 形态 A：无需你 reopen

同时满足：

1. 你明确判断当前 `NavGrid2D` 现有 contract 已经足够；
2. 你明确指出：
   - 哪些调用点属于 misuse
   - 为什么不需要新接口
3. 你明确建议农田线应继续在哪个调用点收口。

### 形态 B：需要你后续最小 reopen

同时满足：

1. 你明确判断当前确实存在 contract gap；
2. 你能精确点名：
   - 文件
   - 入口
   - 缺的是哪类轻量 contract
3. 你能说清：
   - 为什么这不是 scene / binder / 调用方单独可解的问题
   - 为什么继续用 `RefreshGrid()` 必然会把这类 runtime 放置拖成重型全量刷新

---

## 六、固定回执格式

### A1. 用户可读汇报层

必须逐项写：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### A2. 用户补充层

必须额外显式回答：

1. 你的最终判断属于哪一类：
   - `现有 contract 足够，调用方 misuse`
   - `现有 contract 过粗，需要轻量 contract`
2. 如果是 misuse，请精确点名第一责任调用点
3. 如果是 contract gap，请精确点名最小接口面
4. 这件事和 `PlayerAutoNavigator` 是否有关
5. 农田线历史上碰过 `GameInputManager / PlayerAutoNavigator` 这件事，是否会改变你这轮责任判断

### B. 技术审计层

至少补：

1. changed_paths
2. 是否仍保持 `PARKED`
3. 本轮是否只读
4. 你引用了哪些代码证据
5. blocker_or_checkpoint
6. 一句话摘要

---

## 七、一句话总结

**这轮不是叫你回来接“导航修复大包”，而是请你只判断：当前 placeable 放置成功卡顿，到底是业务侧误用了 `NavGrid2D.RefreshGrid()`，还是你这边确实还欠一个更轻的 runtime refresh contract。**
