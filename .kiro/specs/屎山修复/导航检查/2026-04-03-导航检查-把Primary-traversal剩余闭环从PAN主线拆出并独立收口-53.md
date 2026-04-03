# 导航检查：把 Primary traversal 剩余闭环从 PAN crowd 主线里拆出来，独立收口

## 一、先接受新的三方分工

这轮开始前，新的分工已经变了，必须先同时接受下面 3 条事实：

1. **用户已认可当前 PAN / crowd 导航版本**
   - 当前玩家右键导航版本已经被用户明确认可
   - `PassableCorridor / StaticNpcWall` 的 targeted probe 红面从现在起只再算后续 polish 诊断项
   - 不再作为当前版本“不可用”的主事实

2. **`导航检查V2` 只收 runner/menu 最终验收交接**
   - 它不再继续修 `PlayerAutoNavigator.cs`
   - 它这轮只负责：
     - `final acceptance pack`
     - 最终验收交接
     - sync / own-root 判定
     - 并且必须和 `Primary traversal` 独立分账

3. **工具-V1（`019d4d18-bb5d-7a71-b621-5d1e2319d778`）只保留 3 个脚本契约**
   - 它当前 exact-own 只剩：
     - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
     - `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
   - 它不是 scene writer
   - 不是 binder owner
   - 不是通用工具 owner
   - 不准默认让它继续替你写 `Primary.unity / Town.unity / PrimaryTraversalSceneBinder.cs`

所以你这轮的身份已经重新变成：

**父线程 `导航检查` 主刀一个新的独立切片：`Primary traversal` 剩余 scene integration / live closure。**

---

## 二、这轮唯一主线

把用户最初那组 `Primary` 现场诉求里，仍未闭环、且本质属于 traversal / blocking / navigation scene integration 的部分，从你当前的 `PAN crowd runtime` 主线里**单独拆出来**，作为一个新的可见切片接盘。

这轮唯一主刀只收下面 3 个 still-open 问题：

1. 玩家不能穿过 `Props`
2. 玩家和 NPC 不能进入 `Water`
3. 玩家不能走出 tilemap 外

这轮**不是**继续修：

- `PlayerAutoNavigator.cs` crowd runtime
- `PassableCorridor / StaticNpcWall`
- `final acceptance pack`
- `runner/menu`

---

## 三、这轮明确禁止

1. 不准再把这轮写成：
   - 继续修整个导航系统
   - 继续修 PAN crowd
   - 继续追 final pack 全绿

2. 不准再把这轮和下面两条混报：
   - `PAN crowd runtime` 主线
   - `V2 runner/menu final acceptance` 并行验收线

3. 不准去碰 `V2` own：
   - `Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs`
   - `Assets/YYY_Scripts/Service/Navigation/Editor/NavigationLiveValidationMenu.cs`

4. 不准去偷写工具-V1 own：
   - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
   - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
   - `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
   除非你已经 fresh 压实这轮 blocker 就是脚本契约缺口，并且回执里必须精确点名缺哪一条契约、为什么不是 scene integration 问题。

5. 不准把：
   - `GroundRawMatrix`
   - 或 `center-only` 结构绿
   写成：
   - `右键停位偏上已关闭`

6. 不准把镜头边界混进本轮：
   - `镜头不能拍到场景外` 不在这轮主刀里

---

## 四、允许的 scope

你这轮允许做的只有下面这些：

1. `Primary traversal` 的 scene integration / runtime closure
2. 围绕 `Primary` 当前现场的 fresh live 取证
3. 如确有必要，接盘：
   - `Primary.unity`
   - `PrimaryTraversalSceneBinder.cs`
   - 与这 3 个阻挡问题直接相关的最小 scene wiring / live support
4. 如确有必要，接盘与这 3 个问题直接相关的最小 navigation runtime glue

你这轮禁止做的仍然是：

1. crowd runtime 大修
2. runner/menu 验收线
3. 通用工具
4. `Town.unity`
5. 镜头边界
6. UI / Prompt / 其他系统顺手修

---

## 五、你必须依赖的现有契约基线

当前你必须把工具-V1交给你的 3 个脚本视为**已存在的 contract baseline**，默认先用，不要先重写：

### `NavGrid2D.cs`

1. 提供显式 obstacle tilemaps / colliders 接线能力
2. 可开关 obstacle auto-detection
3. 不再按 `Primary/Town` 名字硬编码 scene wiring

### `PlayerMovement.cs`

1. 提供 `NavGrid2D` 接线入口
2. 提供脚底采样参数
3. 缺 `NavGrid2D` 时应明确 warning，而不是 silent fallback

### `SceneTransitionTrigger2D.cs`

1. 已有 trigger + target scene path / name 的基础脚本契约
2. 但 scene 里有没有放、怎么接，不算那条线已完成

你的第一默认判断必须是：

- **先假设脚本契约已经有了**
- **优先查 scene integration / live wiring**

只有在 fresh 证据明确表明：

- scene wiring 已正确
- 但脚本契约本身仍缺入口 / 缺字段 / 缺行为

才允许把 blocker 反抛给工具-V1。

---

## 六、完成定义

### 形态 A：这轮真正闭环

同时满足：

1. 你已经把 `Primary traversal` 从 PAN crowd 主线里单独拆出来报实
2. 玩家朝 `Props` 走时，进不去
3. 玩家朝 `Water` 走时，进不去
4. 玩家朝 tilemap 外走时，被拦住
5. NPC / 自动导航也遵守同一阻挡语义
6. 你明确写清：
   - 这轮动了哪些 scene wiring / binder / scene 资源
   - 哪些属于结构成立
   - 哪些属于真实入口体验已验证

### 形态 B：这轮把 blocker 压到真责任点

同时满足：

1. 你没有越界去改工具-V1 own 的 3 个脚本
2. 你没有再把 `Primary traversal` 混报进 crowd / runner/menu
3. 你把 blocker 压实到了下面三类之一：
   - `Primary.unity` scene wiring
   - `PrimaryTraversalSceneBinder.cs` 或 live integration
   - 工具-V1 3 个脚本中的精确 contract gap
4. 如果是第 3 类，你必须精确点名：
   - 哪个文件
   - 哪个入口 / 字段 / 行为
   - 为什么现在确定不是 scene wiring 问题

---

## 七、固定回执要求

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

1. 这轮有没有把 `Primary traversal` 从 `PAN crowd runtime` 主线里单独拆出来
2. 这轮有没有碰：
   - `Primary.unity`
   - `PrimaryTraversalSceneBinder.cs`
   - `NavGrid2D.cs`
   - `PlayerMovement.cs`
   - `SceneTransitionTrigger2D.cs`
3. `Props / Water / tilemap 外边界` 现在各自处于哪一层：
   - `结构成立`
   - `targeted probe 成立`
   - `真实入口体验成立`
4. NPC / 自动导航是否也已经被同一语义拦住
5. 如果仍未闭环，第一责任点现在是：
   - scene wiring
   - binder / live integration
   - 还是脚本 contract gap

### B. 技术审计层

至少补：

1. changed_paths
2. 是否触碰 scene
3. 是否触碰 `PlayerAutoNavigator.cs`
4. 是否触碰 `NavGrid2D.cs / PlayerMovement.cs / SceneTransitionTrigger2D.cs`
5. pre_sync_validation
6. 当前 own 路径是否 clean
7. blocker_or_checkpoint
8. 一句话摘要

---

## 八、thread-state

```text
[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1
- 第一次准备 sync 前，必须先跑：
  D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1

回执里额外补 3 件事：
- 这轮是否已跑 Begin-Slice / Ready-To-Sync / Park-Slice
- 如果还没跑，原因是什么
- 当前是 ACTIVE / READY / PARKED，还是被 blocker 卡住
```

---

## 九、一句话总结

**这轮父线程不再继续修 crowd，也不再接 runner/menu；唯一主刀是把 `Primary traversal` 剩余 scene integration / live closure 从 PAN 主线里独立拆出并收口，工具-V1只做脚本 contract support，V2只做最终验收交接。**
