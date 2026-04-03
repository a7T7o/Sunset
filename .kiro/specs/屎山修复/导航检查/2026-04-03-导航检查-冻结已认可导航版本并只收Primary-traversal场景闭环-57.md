# 导航检查：冻结已认可导航版本，下一轮只收 Primary traversal 场景闭环

先接受当前已经压实的总分账：

1. 当前玩家导航版本已经被用户认可，这是最高层事实。
2. `导航检查V2` 已完成最终验收交接定性，但未完成 legal-sync。
3. 工具-V1 已被 `-55` 压回 `PARKED`：
   - 不再自己清 same-root dirty
   - 只保留三脚本的被动响应权
4. 所以你这轮真正还没做完、而且只能由你接的，已经只剩：
   - `Primary traversal` 的 scene integration / live closure

---

## 一、这轮唯一主刀

只收下面 3 件事在 `Primary` 现场的真实成立：

1. 玩家不能穿过 `Props`
2. 玩家和 NPC 不能进入 `Water`
3. 玩家不能走出 tilemap 外

这轮不是：

1. 继续修 `PAN crowd runtime`
2. 继续追 `PassableCorridor / StaticNpcWall`
3. 继续拉 `导航检查V2` 重跑 final pack
4. 继续让工具-V1自清 same-root dirty
5. 镜头边界
6. `Town` 转化

---

## 二、这轮允许的 scope

### 允许

1. `Primary.unity`
2. `Assets/Editor/Story/PrimaryTraversalSceneBinder.cs`
3. 与上面 3 个 traversal 问题直接相关的最小 scene wiring / live integration
4. 如 fresh 证据证明仅靠 scene wiring 不够，允许最小 navigation glue

### 禁止

1. 不准把 crowd/runtime 再混回这轮
2. 不准碰 runner/menu 验收线
3. 不准碰工具-V1 own 的三脚本，除非你已经 fresh 压实真是脚本 gap
4. 不准把：
   - `GroundRawMatrix`
   - 或结构层绿
   写成：
   - `右键停位偏上已关闭`
5. 不准把“accepted navigation version 已成立”和“Primary traversal 还没完”混成一个模糊总评

---

## 三、你必须先消费的现有基线

### 1. 已冻结的事实

1. 当前玩家导航版本：已被用户认可
2. `PassableCorridor / StaticNpcWall`：
   - 仍是 targeted probe 红面
   - 但不是当前版本 release blocker
3. `导航检查V2`：
   - 只再承担最终交接和 sync 判定
   - 不再回 runtime

### 2. 工具-V1当前可依赖脚本基线

当前默认先用，不要先重写：

1. `NavGrid2D.cs`
   - 已有 world bounds 读取 / clamp / in-bounds fallback
   - 已带 obstacle source / nearest-walkable 相关 runtime 影响
2. `PlayerMovement.cs`
   - 已有 nav-grid resolve / occupy / foot probe API
   - 已带真实移动边界约束相关 runtime 影响
3. `SceneTransitionTrigger2D.cs`
   - 已有统一 target 解析逻辑

所以你这轮默认先查：

1. `Primary.unity`
2. `PrimaryTraversalSceneBinder.cs`
3. scene wiring / live integration

只有当你能 fresh 压实：

1. 现场 wiring 是对的
2. binder 是对的
3. 但脚本 contract 或现有 runtime behavior 仍缺口

才允许把球精确回抛给工具-V1。

---

## 四、完成定义

### 形态 A：这轮真正闭环

同时满足：

1. 玩家朝 `Props` 走时进不去
2. 玩家朝 `Water` 走时进不去
3. 玩家朝 tilemap 外走时被拦住
4. NPC / 自动导航也遵守同一语义
5. 你明确分开报：
   - 结构成立
   - targeted probe
   - 真实入口体验

### 形态 B：这轮把 blocker 压到真责任点

同时满足：

1. 你没有把 cleanup 甩回工具-V1
2. 你没有把 crowd/runtime 混回来
3. 你把 blocker 压实到下面三类之一：
   - `Primary.unity` scene wiring
   - `PrimaryTraversalSceneBinder.cs`
   - 工具-V1三脚本中的精确 contract gap / runtime trim gap

如果是第 3 类，必须精确点名：

1. 文件
2. 入口 / 字段 / 行为
3. 是 `contract gap` 还是 `runtime trim / rollback`
4. 为什么现在确定不是 scene 面问题

---

## 五、固定回执格式

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

1. 这轮有没有碰：
   - `Primary.unity`
   - `PrimaryTraversalSceneBinder.cs`
   - `NavGrid2D.cs`
   - `PlayerMovement.cs`
   - `SceneTransitionTrigger2D.cs`
2. `Props / Water / tilemap 外边界` 现在各自处于哪一层：
   - `结构成立`
   - `targeted probe 成立`
   - `真实入口体验成立`
3. NPC / 自动导航是否也已被同一语义拦住
4. 如果仍未闭环，第一责任点是：
   - scene wiring
   - binder / live integration
   - 还是脚本 contract / runtime trim gap
5. 你有没有要求工具-V1再动；如果有，精确点名内容是什么

### B. 技术审计层

至少补：

1. changed_paths
2. 是否触碰 scene
3. 是否触碰 binder
4. 是否触碰 `NavGrid2D / PlayerMovement / SceneTransitionTrigger2D`
5. pre_sync_validation
6. 当前 own 路径是否 clean
7. blocker_or_checkpoint
8. `Begin-Slice / Ready-To-Sync / Park-Slice`
9. 一句话摘要

---

## 六、thread-state

```text
[thread-state 接线要求｜本轮强制]
你这轮如果继续真实施工，先跑：
  D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1

第一次准备 sync 前，必须跑：
  D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1

如果这轮先停、卡住、让位或不准备收口，改跑：
  D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1
```

---

## 七、一句话总结

**下一轮父线程不再回头修 crowd，也不再收 runner/menu；只冻结当前已认可导航版本，并把 `Primary traversal` 的 scene integration / live closure 真正收掉。**
