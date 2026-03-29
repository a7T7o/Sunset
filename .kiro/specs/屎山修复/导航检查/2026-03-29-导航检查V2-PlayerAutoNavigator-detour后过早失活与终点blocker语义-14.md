# 2026-03-29-导航检查V2-PlayerAutoNavigator-detour后过早失活与终点blocker语义-14

上一轮 `PlayerAutoNavigator.cs` 的 passive/static NPC blocker 响应链 checkpoint，  
我接受，而且我也接受用户刚刚的真实手测结论：  

1. 这轮已经不是旧的纯 `PathMove` 推土机了  
2. 玩家面对单个静止 NPC 时，已经“明显好了非常多”，到了“可以用”的地步  
3. 但这不等于已经收口  

因为用户刚刚补的现场事实同样非常硬：  

1. NPC 多时，玩家会挤在中间过不去  
2. 如果终点就有一个 NPC 停留，玩家会像一直想撞开或绕开它一样，在终点附近反复避让  
3. 细节仍然明显不到位

同时你自己这轮 fresh 也已经把新责任点说得很清楚：  

1. 旧的 pure `PathMove` 推土机签名已经被打破  
2. `detour/rebuild` 已进入  
3. 但随后会在未到点时掉成 `pathCount=0 + DebugLastNavigationAction=Inactive` 的过早失活

所以这轮不准漂去新的大责任簇。  
这轮唯一主刀固定为：

只锁 `PlayerAutoNavigator.cs` 里  
“detour/rebuild 已进入，但终点前执行窗口被过早吃掉，最终掉到 `Inactive/pathCount=0`”  
这条链。

---

## 一、当前已接受基线

### 1. 已接受的有效进展

这些我接受：

1. 旧链
   - `!avoidance.ShouldRepath -> return false -> 继续 PathMove`
   已经不再是唯一结果
2. `SingleNpcNear raw`
   已经从
   - `npcPush≈2.29`
   - `detourMoveFrames=0`
   - `actionChanges=1`
   - `playerReached=False`
   压成了
   - `npcPush=0~0.077`
   - `detourMoveFrames=14~15`
   - `actionChanges=3`
   - 但仍 `pass=False`
3. 玩家面对单个静止 NPC 的坏相，已经从“稳定顶着走”收缩成“会进 detour，但还会未到点提前失活”

### 2. 当前仍未接受的部分

这些我不接受你包装成“已经快收尾”：

1. `SingleNpcNear raw` 仍未过线
2. crowd 仍会挤在中间过不去
3. 终点有 NPC 停留时，玩家仍会在终点附近反复避让/顶撞
4. 你还没证明 crowd 与终点 NPC 这两个残余坏相是否同属同一条终点前失活链

### 3. 当前最窄第一责任点

当前第一责任点已经不是：

1. ground 锚点契约
2. 静态 runner
3. passive/static NPC 完全没响应
4. solver 大方向
5. `TrafficArbiter / MotionCommand`

而是：

`PlayerAutoNavigator.cs` 里  
`detour/rebuild` 已进入后，  
哪一段分支在玩家尚未真正到点时，  
把执行链吃回了：

1. `pathCount=0`
2. `Cancel()/ResetNavigationState()`
3. `DebugLastNavigationAction=Inactive`
4. 或等价的“终点前执行窗口被提前清空”

---

## 二、这轮唯一主刀

只打这一刀：

锁定 `PlayerAutoNavigator.cs` 中与下面节点直接相邻的终点前失活链：

1. `ExecuteNavigation()`
2. `waypointState.ClearedOverrideWaypoint`
3. `waypointState.ReachedPathEnd`
4. `!waypointState.HasWaypoint`
5. `BuildPath()`
6. `CompleteArrival()`
7. `Cancel()`
8. `ResetNavigationState()`
9. `HasReachedArrivalPoint()`
10. `GetResolvedPathDestination()`

你这轮要解决的不是“再让 single 指标漂亮一点”，  
而是：

1. detour / rebuild 已经进入后
2. 为什么玩家在未到点时还会掉成 `Inactive/pathCount=0`
3. 以及用户刚刚手测到的
   - crowd 中间挤住
   - 终点有 NPC 停留时反复避让
   到底是不是同属这条链

如果它们同属这条链，  
这轮就继续只在这条链里补口。  

如果你确认它们不属同一条链，  
也必须先把“哪一个仍属这条链、哪一个不属”明确判清，  
但不准顺势开成两个新主刀。

---

## 三、允许的 scope

这轮只允许动：

1. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
2. 你自己的文档 / memory / 报告

这轮默认不允许动：

1. `NavigationLocalAvoidanceSolver.cs`
2. `NavigationPathExecutor2D.cs`
3. `NavigationLiveValidationRunner.cs`
4. `NavigationLiveValidationMenu.cs`
5. `NavigationStaticPointValidationRunner.cs`
6. `NavigationStaticPointValidationMenu.cs`
7. `NPCAutoRoamController.cs`
8. ground 契约触点

如果你发现自己非改第 2~8 项不可，  
这轮先停在 blocker 报实，  
不要偷偷扩刀。

---

## 四、这轮禁止漂移

1. 不准把“已经明显好多了”包装成“已经接近收口”
2. 不准把 crowd 直接开成新的独立总修
3. 不准把“终点有 NPC 停留时反复避让”直接漂成 arrival 总架构讨论
4. 不准回到 solver 调参
5. 不准回到 `TrafficArbiter / MotionCommand / DynamicNavigationAgent`
6. 不准再碰 ground / static point 契约与静态验证工具
7. 不准通过改 runner 口径把终点前失活解释掉
8. 不准拿“single 已经比以前好很多”顶账

---

## 五、完成定义

这轮只有两种可接受结局。

### 结局 A：同一条链真正继续前进

你要明确证明：

1. `SingleNpcNear raw` fresh `2~3` 次里，不再出现“detour/rebuild 已进入，但随后未到点掉成 `Inactive/pathCount=0`”这条新坏相
2. crowd “挤在中间过不去”与“终点 NPC 反复避让”至少有一条，被证明确实属于并已随同这条链一起缓解
3. 你能说清这轮具体是哪个终点前分支被修掉了：
   - `ClearedOverrideWaypoint`
   - `ReachedPathEnd`
   - `!HasWaypoint`
   - `BuildPath() -> pathCount=0`
   - `Cancel()/ResetNavigationState()`
   - `HasReachedArrivalPoint()` 误判
   之中的哪一个或哪一小段
4. 最小护栏没有被带坏：
   - `MovingNpc raw ×1`
   - `Crowd raw ×1`
   - `NpcAvoidsPlayer ×1`
   - `NpcNpcCrossing ×1`

### 结局 B：live 仍未过，但责任点继续压窄

前提是你必须明确证明：

1. 旧推土机不是当前主因
2. 现在真正吃掉执行窗口的，是更窄的一段终点前失活链
3. 你能指出是哪个 `if` / 哪段返回让它掉成 `Inactive/pathCount=0`
4. 你能明确回答：
   - crowd 挤住是否属于同链
   - 终点 NPC 反复避让是否属于同链
5. 护栏没有被带坏

如果只是“single 指标再微调了一点”，  
但终点前失活责任点没有更窄，  
这轮不算完成。

---

## 六、验证纪律

1. 只保留：
   - `SingleNpcNear raw` fresh `2~3`
   - `MovingNpc raw ×1`
   - `Crowd raw ×1`
   - 一个最贴近“终点有 NPC 停留”的真实右键场景或现有最接近护栏 `×1`
   - `NpcAvoidsPlayer ×1`
   - `NpcNpcCrossing ×1`
2. 不开长窗 live
3. 不重跑整包大矩阵
4. 一旦拿到足够证据，立刻 `Pause / Stop`
5. 完成后必须退回 `Edit Mode`

---

## 七、固定回执格式

只按下面格式回复，不要自由发挥：

1. 当前在改什么
2. 这轮具体改了 `PlayerAutoNavigator.cs` 的哪几个终点前分支 / 条件
3. 目前是哪一个旧分支在把 `detour/rebuild` 后的执行窗口吃成 `Inactive/pathCount=0`
4. 这轮把它从哪个旧退出条件，改成了哪个新保持/恢复条件
5. `SingleNpcNear raw` 最新 fresh 结果
6. `MovingNpc raw ×1 / Crowd raw ×1 / 终点有 NPC 停留的最接近场景 ×1 / NpcAvoidsPlayer ×1 / NpcNpcCrossing ×1` 结果
7. crowd 挤住与终点 NPC 反复避让，是否同属这条终点前失活链
8. 如果仍 fail，新的第一责任点是什么
9. 当前仍残留哪些 old fallback / private loop
10. changed_paths
11. 当前 own 路径是否 clean
12. blocker_or_checkpoint
13. 一句话摘要

---

## 八、一句话提醒

你这轮不再负责证明“现在比以前好多了”。  
你只负责把 `PlayerAutoNavigator` 里  
“已经会进 detour，但终点前又被过早吃回 Inactive/pathCount=0”  
这条链继续打穿。
