# 2026-03-29-父线程验收清单-PlayerAutoNavigator-完成语义fresh复核与中心语义收口-16

这轮父线程审的不是“它有没有继续留在 `PlayerAutoNavigator.cs`”。

那一点在上一轮已经基本成立。

这轮父线程要审的是 4 件事：

1. 它有没有停止拿旧 blocker 顶账
2. 它有没有真的跑 fresh compile + fresh live
3. 它有没有把“普通地面点完成语义”和“跟随目标完成语义”继续混用这件事，压成实锤或排除
4. 如果 still fail，它有没有在同一个文件里把责任点继续压窄，而不是再开新簇

---

## 一、当前固定基线

父线程先固定这 6 条：

1. 动态主刀仍只允许在 `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
2. 当前已接受的结构热区仍是：
   - `TryFinalizeArrival(...)`
   - `ShouldDeferActiveDetourPointArrival(...)`
   - `TryHoldPostAvoidancePointArrival(...)`
   - `ShouldHoldPostAvoidancePointArrival(...)`
   - `HasReachedArrivalPoint(...)`
   - `GetPlayerPosition(...)`
3. 用户已明确接受的契约是：
   - 普通地面点导航 = 玩家实际占位中心语义
   - 跟随交互目标 = `ClosestPoint + stopRadius`
4. 当前不能 claim 的仍是：
   - crowd 体验已过
   - 终点 NPC 反复避让已过
   - 普通点导航到点精度已过
5. `SpringUiEvidenceMenu.cs` 旧 blocker 已失效，不允许再拿来停车
6. 只要没有 fresh live，这轮就还不能 claim 体验推进

---

## 二、Scope Gate

### Gate P0：仍必须留在同一刀

允许：

1. `PlayerAutoNavigator.cs`
2. 它自己的 memory / 文档

如果它碰了下面任一代码文件，先判 scope 漂移：

1. `NavigationLocalAvoidanceSolver.cs`
2. `NavigationPathExecutor2D.cs`
3. `NavigationLiveValidationRunner.cs`
4. `NavigationLiveValidationMenu.cs`
5. `NavigationStaticPointValidationRunner.cs`
6. `NPCAutoRoamController.cs`
7. 任何 Overlay / UI / Scene / Prefab

---

## 三、Blocker Truth Gate

### Gate P1：不允许旧 blocker 顶账

父线程必须看到：

1. 这轮 fresh compile / console 状态
2. 如果 compile red：
   - 精确文件
   - 精确行号
   - 精确报错文本
   - 是否 own 路径
3. 它必须明确表述：
   - `SpringUiEvidenceMenu.cs` 已不是当前 blocker

如果它继续复述旧 blocker，
直接判 blocker 报实失真。

---

## 四、Fresh Gate

### Gate P2：compile clean 后必须进入最小 fresh live

如果它 compile clean，
父线程必须看到：

1. `SingleNpcNear raw ×2`
2. `MovingNpc raw ×1`
3. `Crowd raw ×1`
4. `终点有 NPC 停留的最接近场景 ×1`
5. `NpcAvoidsPlayer ×1`
6. `NpcNpcCrossing ×1`

如果 compile clean 但没有这组 fresh，
直接判未完成。

### Gate P3：如果做了最小补口，必须同矩阵复核

如果它这轮又改了 `PlayerAutoNavigator.cs`，
父线程必须继续看到：

1. 它只改了完成语义相关热区
2. 它没有改第二个主刀
3. 它用同一组最小矩阵重跑了 1 轮 fresh

---

## 五、语义 Gate

### Gate P4：父线程必须看到“中心语义”被直接回答

父线程这轮一定要看到：

1. 第一条仍 fail 的 case 的
   - `Requested`
   - `Resolved`
   - `Transform`
   - `Collider`
2. 它是否还存在：
   - `Collider` 已进入完成阈值
   - 但玩家可见占位仍没真正到点击点 / 终点窗口
3. 它有没有明确区分：
   - 普通地面点完成语义
   - 跟随交互目标完成语义

如果它继续只报 frame 指标，
但不回答“语义到底有没有混”，
这轮视为没有把第一责任点压窄。

---

## 六、结果 Gate

### Gate P5：继续审 `导航完成 -> Inactive/pathCount=0`

父线程要看到：

1. 这条签名现在还在不在
2. single 是否仍会先完成再失活
3. crowd 是否仍共享这条签名
4. 终点 NPC 反复避让是否已拿到 dedicated 映射

### Gate P6：NPC 护栏仍要站住

下一轮至少要看到：

1. `NpcAvoidsPlayer ×1 pass`
2. `NpcNpcCrossing ×1 pass`

NPC 护栏掉红就判回归。

### Gate P7：不能把 targeted probe 说成体验过线

即使它这轮 probe 全过，
父线程也只允许它 claim：

1. `结构 / checkpoint`
2. `targeted probe / 局部验证`

不允许它直接 claim 用户真实体验已经过线，
除非用户自己手测确认。

---

## 七、父线程审查顺序

固定按这个顺序看：

1. 先看它有没有停止拿旧 blocker 顶账
2. 再看 compile 是否 clean
3. 再看是否真的拿到 fresh live
4. 再看是否直答了“中心语义是否混用”
5. 再看 `导航完成 -> Inactive/pathCount=0` 是否还在
6. 再看 crowd / 终点 NPC 是否同链
7. 最后才看 `changed_paths / dirty / checkpoint`

---

## 八、当前阶段判断

这轮父线程判断固定为：

1. `-15` 最新回执只能算历史 checkpoint，不能再拿当前 blocker 停车
2. 动态线现在该做的不是继续解释结构补口，而是把 fresh compile / fresh live 真做出来
3. 如果 still fail，当前最值钱的继续压窄方向是：
   - `HasReachedArrivalPoint(...)`
   - `GetPlayerPosition(...)`
   - 普通点 vs 跟随目标 的完成语义分离
4. 在它真正给出这轮 fresh 结果前，父线程不能接受任何“已经进入收口后半段”式口号
