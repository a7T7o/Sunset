# 2026-04-02-导航检查V2-强制塌缩compile-gate并在同窗完成PAN大刀闭环-33

先给这轮裁定：

1. 我接受你上一轮 **不是空口编 blocker**：
   - 当前本机 `Editor.log` 里确实存在你回执里报的这两组 `CS0103`
   - `InventorySlotUI.cs`
   - `ToolbarSlotUI.cs`
2. 但我 **不接受** 你拿这条 gate 直接停成“等外部清掉再说”。
3. 原因很具体：
   - 当前工作树里，这两个文件里 `TickStatusBarFade()` / `ApplyStatusBarAlpha()` 的方法本体已经真实存在；
   - 所以你上一轮回执缺的不是“报出了 blocker”，而是 **没有把 blocker 的当前真相塌缩清楚**：
     - 这是旧红还没被一次 fresh recompile 盖掉？
     - 还是当前文件虽然有方法，但 Unity 仍然在最新 compile 上继续红？
4. 因此这轮不准再交“blocker checkpoint 就停”的小回卡。
5. 这轮我要你做一刀 **更大的**：
   - 先把 compile gate 真伪塌缩掉；
   - gate 一旦不是当前活 blocker，就 **不要停车**；
   - 直接在同一个窗口里把 `PlayerAutoNavigator.cs` 的 endpoint + crowd + 右键停位可视真值一起打到这轮能打到的最大闭环。

---

## 一、当前已接受的基线

这些继续接受，不准回漂：

1. `-25` 继续只算 `carried partial checkpoint`
2. `-29` 继续只算 `carried partial checkpoint`
3. dedicated endpoint fake green 已剔掉
4. dedicated endpoint menu/toolchain 已接回
5. `-31` 的唯一 active write target 继续是：
   - `PlayerAutoNavigator.cs`
6. 用户最新真人反馈仍有效：
   - 右键导航整体体验还是胡闹
   - 玩家停位仍有“落在点击点上方”的体感
   - NPC/玩家共路或双 NPC 通道仍会漂、卡、徘徊

这几条都不准再被“局部 probe 绿了”偷换掉。

---

## 二、这轮唯一主刀

这轮是一个 **大刀单事务**，顺序固定，不准拆成两个小停车：

1. **先塌缩 compile gate 真伪**
2. **如果 gate 不再是当前活 blocker，直接继续同窗完成 PAN runtime 大闭环**

所谓“大闭环”，在这轮里具体是：

- dedicated endpoint
- crowd / 双近距 NPC 通道
- ground / 右键停位可视真值

都要在同一轮里一起推进，而不是只拿其中一条做 partial checkpoint。

---

## 三、这轮第一步：必须先把 compile gate 真相压实

### A. 你必须先做的动作

1. clear console
2. request compile
3. wait 到 compile 完结
4. 读取 **当前这一轮最新** console / `Editor.log`

### B. 你必须回答清楚的 4 个问题

1. 当前最新 compile 还报不报：
   - `InventorySlotUI.cs`
   - `ToolbarSlotUI.cs`
   这两组 `CS0103`
2. 如果还报，**当前工作树明明已经有方法本体，为什么最新 compile 还会继续报缺方法？**
3. 如果不再报，这条 gate 就必须立刻降级成：
   - `stale / cleared blocker`
4. 无论结果如何，都必须给：
   - 最新 console 行
   - 最新 `Editor.log` 关键片段
   - 当前这两个 UI 文件的 git dirty 事实

### C. 这一关的硬规则

#### 允许

- 只读 `InventorySlotUI.cs`
- 只读 `ToolbarSlotUI.cs`
- 只读 console / `Editor.log`

#### 不允许

- 直接把这两个 UI 文件收进你的 write scope
- 因为它们不在你这轮 own 白名单里

### D. 这一步的停步条件

只有两种：

1. **compile gate 仍然在最新 recompile 里稳定重现**
2. **compile gate 已塌缩，不再是当前活 blocker**

除此之外，不准停。

---

## 四、如果 gate 已塌缩：这轮不准停车，直接继续同窗大刀闭环

一旦这条 gate 在当前 fresh recompile 中不再成立，你就必须直接进入下面这段，不准回“这轮先报 gate truth”。

### A. 先补 endpoint / crowd 失败瞬间真值

至少 fresh：

1. `EndpointNpcOccupied raw ×3`
2. `Crowd raw ×3`
3. `Ground raw matrix ×1`

每条 endpoint / crowd 都必须补：

1. `Requested`
2. `Resolved`
3. `Transform`
4. `Rigidbody`
5. `Collider`
6. `Foot`
7. `IsActive`
8. `pathCount`
9. `pathIndex`
10. `DebugLastNavigationAction`

endpoint 另补：

11. `outcome`
12. `endpointContractSatisfied`
13. `playerCenterDistance`
14. `playerFootDistance`

crowd 另补：

11. `crowdStallDuration`
12. `directionFlips`
13. `playerCenterDistance`

### B. 然后直接在 PAN 同簇补代码

只允许回：

1. `TryFinalizeArrival(...)`
2. `ShouldDeferActiveDetourPointArrival(...)`
3. `TryHoldBlockedPointArrival(...)`
4. `TryHoldPostAvoidancePointArrival(...) / ShouldHoldPostAvoidancePointArrival(...)`
5. `TryGetPointArrivalNpcBlocker(...)`
6. `HasReachedArrivalPoint(...)`

只有当 fresh 证据明确证明测量语义是第一责任点，才允许克制触及：

7. `GetPlayerPosition()`
8. `GetResolvedPathDestination()`

但如果你碰 7/8，必须同时解释：

- 为什么这不是再把 `center / transform / foot` 混成一锅
- 为什么这不会再制造“结构绿了但用户可视停位仍怪”的假关闭

### C. 这轮真正要收的不是“小真因”，而是这三个东西一起收

1. dedicated endpoint 不再稳定 lingering
2. crowd / 双 NPC 通道不再稳定 lingering / slow-crawl / direction flip 卡死
3. `Ground` 的结构真值和用户可视停位真值不再互相打架

也就是说，这轮不是只回答“PAN 哪个分支在吃窗口”，而是要 **在同一刀里尽量把这个簇打到用户能感到不再胡闹**。

---

## 五、这轮 completion 定义，步子明确放大

### 最优完成

如果你这轮能做到，目标是：

1. `EndpointNpcOccupied raw ×3`
   - 不再全部 lingering
   - 不再停在 `playerCenterDistance ≈ 1.0`
2. `Crowd raw ×3`
   - 不再全部 `pass=False`
   - `crowdStallDuration` 明显下到不再灾难级
   - `directionFlips` 不再维持徘徊卡顿签名
3. `Ground raw matrix ×1`
   - 不再只有 `Collider.center` 结构绿
   - 你必须能直接报：
     - `用户可视停位已关闭`
     - 或
     - `仍未关闭`
4. guardrails 守住：
   - `SingleNpcNear raw ×2`
   - `MovingNpc raw ×1`
   - `NpcAvoidsPlayer ×1`
   - `NpcNpcCrossing ×1`

### 次优完成

如果这轮还不能全绿，至少也必须同时做到：

1. compile gate 真伪已塌缩清楚
2. PAN 第一责任分支已用 fresh 证据钉死
3. 至少完成一刀真实 PAN 修改
4. endpoint / crowd / ground 三条里，至少两条出现了用户可感知改善，不再只是“真因更清楚了”

我这轮不再接受：

- 只有 blocker truth
- 只有 fresh signature
- 没有 runtime 改刀

这种小停车。

---

## 六、这轮最小 fresh matrix（gate 清掉后必须跑）

1. `EndpointNpcOccupied raw ×3`
2. `Crowd raw ×3`
3. `Ground raw matrix ×1`
4. `SingleNpcNear raw ×2`
5. `MovingNpc raw ×1`
6. `NpcAvoidsPlayer ×1`
7. `NpcNpcCrossing ×1`

如果出现 hijack，再补：

8. `EndpointNpcOccupied suppressed ×1`

不准再用旧 run 顶这轮。

---

## 七、这轮固定回执格式

### A1. 用户可读汇报层

固定 6 项，顺序不得改：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

并且你这轮必须额外显式加一句：

- 这轮我为什么没有再停在小 checkpoint，而是继续把刀做大

### B. 技术审计层

至少逐项回答：

1. 当前在改什么
2. `-25` 当前如何定性；必须直接写 `carried partial checkpoint`
3. `-29` 当前如何定性；必须直接写 `carried partial checkpoint`
4. 当前最新 compile gate 真相是什么
5. 最新 console / `Editor.log` 里这条 gate 到底是：
   - `active`
   - `stale`
   - `cleared`
6. 如果 gate 仍 active，为什么当前文件明明已有方法本体却仍报 `CS0103`
7. 如果 gate 已 cleared，你从哪个时间点开始继续 PAN live
8. 这轮 fresh 先钉出来的第一真实 runtime 失败签名是什么
9. 它具体落在哪个 PAN 分支 / 条件上
10. 这轮具体改了 `PlayerAutoNavigator.cs` 的哪几个分支 / 条件
11. 它从哪个旧退出条件，改成了哪个新保持 / 恢复条件
12. `Ground raw matrix ×1` 结果
13. ground 的：
   - `Requested / Resolved / Transform / Rigidbody / Collider / Foot`
   - 以及你对“用户可视停位是否仍偏上”的直接裁定
14. `SingleNpcNear raw ×2` 结果
15. `MovingNpc raw ×1` 结果
16. `EndpointNpcOccupied raw ×3` 结果
17. `Crowd raw ×3` 结果
18. `NpcAvoidsPlayer ×1 / NpcNpcCrossing ×1` 结果
19. 如果仍 fail，新的第一责任点是什么
20. 当前是否还能把“static / 右键停位偏上”写成已关闭；如果不能，直接写 `不能`
21. changed_paths
22. 当前 own 路径是否 clean
23. blocker_or_checkpoint
24. 一句话摘要
25. 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
26. 如果没跑，原因是什么
27. 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住

---

## 八、这轮停发边界

这轮 **只有两种** 合法收口：

### 1. 真正的大 checkpoint

- compile gate 已塌缩
- PAN 已有真实改刀
- endpoint / crowd / ground 至少有两条出现用户可感知改善

### 2. 真正不可绕开的活 blocker

- 最新 recompile 仍稳定红
- 且你已经给出足够证据证明这不是 stale gate
- 且因此 Unity 无法进入 Play

除了这两种，别再给我第三种“小 blocker 回卡”。

[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 旧线程不用废弃重开；但最晚必须在“下一次真实继续施工前”补这次 `Begin-Slice`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：
- 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 如果还没跑，原因是什么
- 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住

这不是建议，是当前 live 规则的一部分。除非这轮始终停留在只读分析，否则不要跳过。
