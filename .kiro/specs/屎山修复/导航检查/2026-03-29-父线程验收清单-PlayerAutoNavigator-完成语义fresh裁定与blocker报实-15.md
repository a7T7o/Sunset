# 2026-03-29-父线程验收清单-PlayerAutoNavigator-完成语义fresh裁定与blocker报实-15

这轮父线程不再审“结构补口有没有落下”，  
因为这一点在 `-14` 回执里已经基本成立。  

这轮父线程要审的是：

1. blocker 是否报实
2. current compile 状态是否真实
3. `PlayerAutoNavigator` 当前完成语义补口有没有拿到 fresh live 裁定

---

## 一、当前固定基线

父线程先固定这 4 条：

1. 当前主刀仍然只允许在 `PlayerAutoNavigator.cs`
2. 当前最像第一责任点的旧链仍是：
   - `HasReachedArrivalPoint() -> CompleteArrival() -> ResetNavigationState()`
3. crowd 挤住与终点 NPC 反复避让，当前仍只允许先判是否同属这条完成语义链
4. `validate_script` 通过不等于 compile clean；compile blocker 必须报当前最新事实

---

## 二、Scope Gate

### Gate P0：只准留在同一刀

允许：

1. `PlayerAutoNavigator.cs`
2. 导航检查 memory / 文档

如果它又碰了：

1. `PromptOverlay`
2. `WorkbenchCraftingOverlay`
3. `NavigationLocalAvoidanceSolver.cs`
4. `NavigationPathExecutor2D.cs`
5. `NavigationLiveValidationRunner.cs`
6. `NavigationStaticPointValidationRunner.cs`
7. `NPCAutoRoamController.cs`

默认先判 scope 漂移。

---

## 三、Blocker Truth Gate

### Gate P1：blocker 必须是“当前最新 blocker”

父线程必须看到：

1. 这轮当前 compile / console 状态
2. 如果 compile blocker 存在：
   - 精确文件
   - 精确行号
   - 精确报错文本
3. 是否属于它 own 路径

如果它继续复述旧 blocker，  
但父线程现场已核到脚本级 errors=0 / console 无该错，  
直接判 blocker 报实失真。

---

## 四、Fresh Gate

### Gate P2：compile clean 后必须有 fresh live

如果它已经 compile clean，  
父线程下一轮必须看到：

1. `SingleNpcNear raw ×2`
2. `MovingNpc raw ×1`
3. `Crowd raw ×1`
4. `终点有 NPC 停留的最接近场景 ×1`
5. `NpcAvoidsPlayer ×1`
6. `NpcNpcCrossing ×1`

如果 compile clean 但 still 没有 fresh live，  
直接判这轮未完成。

### Gate P3：仍要审“导航完成 -> Inactive/pathCount=0”签名

父线程要直接看到：

1. 这条签名现在还在不在
2. single 是否还会“先完成、再失活”
3. crowd 是否也共享这条签名
4. 终点 NPC 反复避让是否已经拿到 dedicated 映射

---

## 五、结果 Gate

### Gate P4：如果 still fail，责任点必须更窄

如果 fresh 仍 fail，  
父线程要看到：

1. 新的第一责任点仍在
   - `TryFinalizeArrival`
   - `ShouldDeferActiveDetourPointArrival`
   - `ShouldHoldPostAvoidancePointArrival`
   这一簇
2. 它没有回漂去 `Cancel()` 大口径
3. 它能明确指出 still fail 是哪一条 old fallback 在漏

### Gate P5：NPC 护栏仍要站住

下一轮至少要看到：

1. `NpcAvoidsPlayer ×1 pass`
2. `NpcNpcCrossing ×1 pass`

NPC 两条掉红就判回归。

---

## 六、父线程审查顺序

固定按这个顺序看：

1. 先看 blocker 是否报实
2. 再看 compile 是否 clean
3. 再看是否真的拿到 fresh live
4. 再看 `导航完成 -> Inactive/pathCount=0` 是否还在
5. 再看 crowd / 终点 NPC 是否同链
6. 最后才看 `changed_paths / dirty / checkpoint`

---

## 七、当前阶段判断

这轮最准确的父线程判断固定为：

1. `-14` 的结构补口已经落下，但尚未拿到足够 fresh 裁定
2. 当前最需要防的不是“它又漂到新架构”，而是“它拿旧 blocker 顶账，不把 fresh compile/live 做实”
3. 只要还没拿到 fresh compile + fresh live，这条 `PlayerAutoNavigator` 完成语义线就还不能 claim 有效推进完成
