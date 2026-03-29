# 2026-03-29-导航检查V2-PlayerAutoNavigator-passive静态NPC-blocker响应链-13

上一轮“高保真测试矩阵已完成”这个 checkpoint 我接受，  
但我不接受你把它外推成“已经接近收口”。  

你现在要接受的不是“继续做更多测试”，  
而是：测试已经把 runtime 第一责任点重新钉死，  
下一刀必须直接落到 `PlayerAutoNavigator.cs` 里 passive/static NPC blocker 的失效响应链。

---

## 一、当前已接受基线

### 1. ground / 静态点基线

这一轮先固定为：

1. 普通地面点导航 = 玩家实际占位中心语义
2. 跟随交互目标 = `ClosestPoint + stopRadius`
3. 这两套不得混用
4. ground 契约与静态验证工具不是你这轮主刀

### 2. 高保真矩阵基线

你这轮正式交出的高保真矩阵，我接受这些事实：

1. `SingleNpcNear raw ×3 + suppressed ×1`
   - 稳定 `pass=False`
   - `playerReached=False`
   - `npcPushDisplacement≈2.286 ~ 2.298`
   - `detourMoveFrames=0`
   - `hardStopFrames=0`
   - `actionChanges=1`
   - 这不是误触 NPC 交互，而是稳定推土机坏相
2. `MovingNpc raw ×3`
   - 稳定 fail
   - 当前形态是 onset 偏早 + 侧偏过大 + 到点失败
3. `Crowd raw ×3`
   - 稳定 fail
   - 当前形态是长时间主路径蹭行 + 未到点
4. `NpcAvoidsPlayer ×2` 与 `NpcNpcCrossing ×2`
   - 当前仍是绿护栏

### 3. 当前最窄第一责任点

当前第一责任点已经不是：

- ground 锚点契约
- 静态 runner / menu
- solver 大方向
- moving / crowd 的泛调

而是：

`PlayerAutoNavigator.cs` 里 passive/static NPC blocker 命中后，  
玩家仍停留在 `PathMove` 主路径，  
没有进入有效 detour / 停让 / blocker 升级，  
最终把 NPC 顶走且自己没到点。

---

## 二、这轮唯一主刀

只打这一刀：

锁定 `PlayerAutoNavigator.cs` 中这条响应失效链：

1. `HandleSharedDynamicBlocker(...)`
2. `ShouldDeferPassiveNpcBlockerRepath(...)`
3. `ShouldBreakSinglePassiveNpcStopJitter(...)`
4. 以及与 `PathMove / BlockedInput / HardStop / detour` 进入条件直接相邻的分支

你这轮要解决的不是“指标还可以更漂亮”，  
而是：

当 passive/static NPC blocker 命中时，  
玩家不能再稳定地继续 `PathMove` 把 NPC 顶走。  

你必须让这条链真正进入一个有效响应：

1. detour 生效
2. `BlockedInput / HardStop` 后触发明确升级
3. blocker 明确升级成 rebuild / detour / 恢复入口

三者至少要有一个在 runtime 上成为真实有效路径。  
不允许再停留在“`ShouldRepath=True` 但响应链把它吃掉”。

---

## 三、允许的 scope

这轮只允许动：

1. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
2. 你自己的文档 / memory / 报告

这轮默认不允许动：

1. `NavigationStaticPointValidationRunner.cs`
2. `NavigationStaticPointValidationMenu.cs`
3. `NavigationLiveValidationRunner.cs`
4. `NavigationLiveValidationMenu.cs`
5. `NavigationLocalAvoidanceSolver.cs`
6. `NPCAutoRoamController.cs`
7. ground 契约相关触点

如果你发现自己非改第 3~7 项不可，  
这轮先停在“blocker 报实”，  
不要偷偷扩刀。

---

## 四、这轮禁止漂移

1. 不准再把“高保真矩阵完成”包装成“体验已接近收口”
2. 不准回去继续讲 solver 大方向
3. 不准回去讲 `TrafficArbiter / MotionCommand / DynamicNavigationAgent`
4. 不准再碰 ground / static point 契约与静态验证工具
5. 不准把 moving / crowd 的 fail 当成这轮主刀
6. 不准通过改测试口径来把推土机坏相解释掉
7. 不准再用 `SingleNpcNear` 的旧 pass 或局部 suppress 结果顶账

---

## 五、完成定义

这轮只有两种可接受结局。

### 结局 A：单刀有效完成

你要明确证明：

1. `SingleNpcNear raw` fresh `2~3` 次里，稳定推土机签名已经被打破：
   - 不再是稳定 `detourMoveFrames=0 + actionChanges=1 + playerReached=False + npcPushDisplacement≈2.29`
2. passive/static blocker 命中后，玩家已进入一个真实有效响应分支：
   - `detour`
   - 或 `BlockedInput / HardStop` 后明确升级
   - 或明确的 blocker 升级入口
3. 至少补 4 条最小护栏：
   - `MovingNpc raw ×1`
   - `Crowd raw ×1`
   - `NpcAvoidsPlayer ×1`
   - `NpcNpcCrossing ×1`
4. 护栏没有被这刀带坏

### 结局 B：live 仍未过，但失败形态继续压窄

前提是你必须明确证明：

1. 当前已经不再是“稳定 `PathMove` 推土机”
2. 你能指出是哪个 `if` / 哪段分支仍在吃掉有效响应
3. 新的第一责任点比现在更窄，而且还留在同一条 `PlayerAutoNavigator` 响应链里
4. 护栏没有被这刀带坏

如果只是“我又多讲清了一点结构”或者“我又多测了几场”，  
这轮不算完成。

---

## 六、验证纪律

1. 只保留：
   - `SingleNpcNear raw` fresh `2~3`
   - `MovingNpc raw ×1`
   - `Crowd raw ×1`
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
2. 这轮具体改了 `PlayerAutoNavigator.cs` 的哪几个分支 / 条件
3. 这轮让 passive/static NPC blocker 从哪个旧分支退出，进入了哪个新响应分支
4. 现在哪个条件不再允许它继续稳定 `PathMove`
5. `SingleNpcNear raw` 最新 fresh 结果
6. `MovingNpc raw ×1 / Crowd raw ×1 / NpcAvoidsPlayer ×1 / NpcNpcCrossing ×1` 结果
7. 如果仍 fail，新的第一责任点是什么
8. 当前仍残留哪些 old fallback / private loop
9. changed_paths
10. 当前 own 路径是否 clean
11. blocker_or_checkpoint
12. 一句话摘要

---

## 八、一句话提醒

你这轮不再负责证明“导航有多复杂”，  
你只负责把 `PlayerAutoNavigator` 面对 static/passive NPC 时  
“该响应却没响应、继续推着走”的这条链打穿。
