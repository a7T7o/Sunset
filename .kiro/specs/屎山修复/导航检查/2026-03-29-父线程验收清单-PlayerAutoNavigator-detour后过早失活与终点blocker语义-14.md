# 2026-03-29-父线程验收清单-PlayerAutoNavigator-detour后过早失活与终点blocker语义-14

本清单只服务父线程对 `导航检查V2` 下一轮的单刀验收。  
这一轮不再审“pure PathMove 推土机有没有被打破”，  
因为这一点已经拿到有效 checkpoint。  

这一轮要审的是：

1. `PlayerAutoNavigator` 是否把 `detour/rebuild` 后的过早失活继续压窄
2. 用户刚刚手测的
   - crowd 挤在中间过不去
   - 终点有 NPC 停留时反复避让/顶撞
   是否同属这条终点前失活链

---

## 一、当前固定基线

父线程下一轮先固定这 5 条：

1. 普通地面点导航 = 玩家实际占位中心语义
2. 跟随交互目标 = `ClosestPoint + stopRadius`
3. `SingleNpcNear raw` 已经不再是旧的 pure `PathMove` 推土机，而是“会进 detour，但仍未过线”
4. 用户真实手测已确认：
   - 单个静止 NPC 体验明显好了很多，到了“可以用”的地步
   - 但 crowd 仍会挤住
   - 终点有 NPC 停留时仍会在终点附近反复避让/顶撞
5. `NpcAvoidsPlayer / NpcNpcCrossing` 当前仍是绿护栏

如果下一轮回执试图把第 3 条重新讲回“旧推土机仍是主因”，  
直接判责任点倒退。

---

## 二、Scope Gate

### Gate P0：只准这一刀

允许写入：

1. `PlayerAutoNavigator.cs`
2. 导航检查相关文档 / memory

如果下一轮又碰了下面任一项，默认先判 scope 违规：

1. `NavigationLocalAvoidanceSolver.cs`
2. `NavigationPathExecutor2D.cs`
3. `NavigationLiveValidationRunner.cs`
4. `NavigationLiveValidationMenu.cs`
5. `NavigationStaticPointValidationRunner.cs`
6. `NavigationStaticPointValidationMenu.cs`
7. `NPCAutoRoamController.cs`
8. ground 契约触点

除非它能明确证明这些改动不可回避且仍服务同一刀，  
否则这轮不接受。

---

## 三、代码链 Gate

### Gate P1：必须明确交代“谁吃掉了执行窗口”

父线程必须在回执里直接看到：

1. 它改了 `ExecuteNavigation()` 里哪几个终点前分支
2. 到底是哪一个旧分支，让 `detour/rebuild` 后在未到点时掉成：
   - `pathCount=0`
   - `Cancel()/ResetNavigationState()`
   - `DebugLastNavigationAction=Inactive`
3. 这轮用什么新条件，把这个旧退出链掰开

如果只看到“又调了几个阈值 / 指标更好了”，  
但看不到断点解释，  
直接判不合格。

### Gate P2：不能把 crowd / 终点 NPC 直接漂成新簇

父线程必须能从回执里直接看到：

1. crowd 挤住是否同属这条链
2. 终点 NPC 反复避让是否同属这条链

如果它直接把这两个问题写成“新的独立责任簇”，  
且没有先证明与当前链脱钩，  
直接判漂移。

---

## 四、结果 Gate

### Gate P3：`SingleNpcNear raw` 不能再只是“比以前好”

父线程最先看：

1. 是否仍会出现 `Inactive/pathCount=0`
2. 是否仍是未到点提前失活
3. detour 进入后是否真的留下了有效恢复窗口

下一轮要么：

1. `SingleNpcNear raw` 直接过线

要么至少满足：

1. 过早失活的触发分支已经被明确压窄
2. single 不再只是“会进 detour 然后又死掉”
3. 能明确指出新的单一责任点

### Gate P4：crowd 与终点 NPC 至少要有一条被映射清楚

这轮不要求把 crowd 和终点 NPC 两条都彻底修完，  
但至少要满足二选一：

1. 至少一条已随同当前链一起明显缓解
2. 两条都未彻底缓解，但已被明确判清是否同属这条链

如果它既没缓解，也没判清，  
说明 prompt 没落到位。

### Gate P5：最小护栏仍必须站住

下一轮至少要看到：

1. `MovingNpc raw ×1` 没被明显带坏
2. `Crowd raw ×1` 已回报
3. `NpcAvoidsPlayer ×1 pass`
4. `NpcNpcCrossing ×1 pass`

如果 NPC 两条掉红，  
直接判回归。

---

## 五、回执质量 Gate

### Gate P6：不能再 claim “接近收口”

下一轮如果出现下面任何一种说法，父线程直接降级处理：

1. “已经接近收口”
2. “已进入最后残余细节阶段”
3. “整体功能主链已经基本完成”

除非它同时拿到：

1. `SingleNpcNear raw` 真实过线
2. crowd / 终点 NPC 残余坏相至少大幅收缩
3. 护栏稳定

否则这些阶段判断都不成立。

### Gate P7：own dirty 不 clean 不能冒充收口

如果回执里的 `当前 own 路径是否 clean` 不是 `yes`，  
那它最多只能 claim：

1. 技术 checkpoint
2. 失败收缩 checkpoint

不能 claim：

1. 当前这一刀已彻底收口
2. 可以直接安全 sync

---

## 六、父线程下一轮审查顺序

固定按这个顺序看：

1. 先看 scope 有没有漂
2. 再看它到底指出了哪一个“执行窗口被吃掉”的分支
3. 再看 `SingleNpcNear raw` 是否还在未到点提前失活
4. 再看 crowd / 终点 NPC 是否被映射回同一条链
5. 再看 `MovingNpc / NPC 两条护栏`
6. 最后才看 `changed_paths / dirty / checkpoint`

---

## 七、当前阶段判断

这轮最准确的父线程判断固定为：

1. 旧推土机主故障已经明显被压掉
2. 导航线已经从“完全不能用”推进到“可用但细节和终点语义还明显不对”
3. 下一刀仍应继续留在 `PlayerAutoNavigator.cs`
4. 只要“detour 后未到点就过早失活 / 终点 blocker 语义错误”这条链没打穿，导航线就还不能说进入收尾
