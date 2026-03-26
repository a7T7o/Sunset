# 002-prompt-11

这轮我先把你的最新回执定性清楚：

1. 我接受你已经交出了第二个真实退壳 checkpoint。
2. 但你不能停在“`stuck / repath` 已迁出”这个位置上开始摇摆下一刀。

你这轮做出来的东西，我认：

- `CheckAndHandleStuck()`
- `EvaluateStuck(...)`
- `repath cooldown`
- 恢复结果写回

已经开始从：

- [PlayerAutoNavigator.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs)
- [NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)

继续往：

- [NavigationPathExecutor2D.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs)

集中。

这件事我接受。

但下一句我也钉死：

> 你下一刀不准在 `arrival / cancel / path-end` 和 `detour create / clear / recover` 之间继续摇摆。  
> 这轮唯一主刀固定为：**`detour lifecycle` 真实退壳。**

---

## 一、当前已接受的基线

当前导航线基线固定为：

- `S0`：部分完成
- `S1 / S2 / S4`：部分完成
- `S3 / S5 / S6`：未完成

当前已接受的真实 checkpoint 有两个：

1. `BuildPath / RebuildPath / ActualDestination / 路径后处理` 开始退壳
2. `stuck / repath / 恢复入口` 开始退壳

所以这轮正确起点不是再解释你已经退了哪两刀，而是：

> 继续拆下一组最重、最影响动态避让闭环、并且仍然明显留在控制器里的私有 detour 生命周期。

---

## 二、这轮唯一主刀

### 这轮主刀固定为：

> 把玩家 / NPC 控制器里仍然私养的  
> `detour create / clear / recover`  
> 往共享 [NavigationPathExecutor2D.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs) 继续迁，形成统一 detour lifecycle。

具体盯这几段：

- 玩家侧：
  - `HandleSharedTrafficDecision()` 里的 detour 相关决策与收尾
  - `TryCreateDynamicDetour()`
- NPC 侧：
  - `TryCreateDynamicDetour()`
  - `ClearDynamicDetourIfNeeded()`
  - 与 detour 恢复原路径直接相关的链路

## 就打 detour lifecycle 这一刀。

---

## 三、为什么这轮必须先打 detour lifecycle

因为你现在残留的那些 old fallback 里，最影响“导航看起来像不像还隔着大圆壳”的，不是 `arrival/cancel/path-end`，而是：

- detour 是谁创建的
- detour 是谁清的
- 清掉后怎么恢复原路径
- override waypoint / 临时目标 / 原目标回切到底由谁 hold

如果这组责任还各自留在控制器里，那共享执行层就还不是这条动态避让链的真 owner。

所以这轮不准再跳去更像“收尾清洁工”的 `arrival / cancel / path-end`。

---

## 四、这轮最少要交出的硬结果

这轮最少要交出的，不是“又把某个 if 改成调共享方法”。

而是：

## 共享执行层开始真正拥有 detour 生命周期

至少要开始把这些东西拔出来：

1. detour 创建入口
2. detour 生效期间的核心状态
3. detour 清理入口
4. detour 结束后恢复原路径 / 原目标的入口
5. 需要时对 override waypoint / 临时目标的写回与释放

你这轮不能只让控制器继续自己决定何时建 detour、何时清 detour，只是最后调用一下共享层。

那仍然是 wrapper，不是真 owner。

---

## 五、这轮允许的 scope

### 允许集中在：

- [NavigationPathExecutor2D.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs)
- [PlayerAutoNavigator.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs)
- [NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
- 必要时：
  - [NavigationAvoidanceRulesTests.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs)
  - 与共享执行层 detour 生命周期直接相关的最小测试文件

### 这轮不允许：

1. 又漂回 solver 参数调优
2. 又漂回讲“先裁决、后求解”的大方向但代码不动
3. 又提前切去 `arrival / cancel / path-end`
4. 又拿 external blocker 当停车位
5. 又去碰 `Primary.unity`

---

## 六、这轮完成定义

只有满足下面任一条，你才算这轮有效完成：

### 结局 A：detour lifecycle 真实退壳

你要明确证明：

1. 共享执行层现在多持有了哪些 detour 状态
2. 玩家控制器少了哪些 detour 私有责任
3. NPC 控制器少了哪些 detour 私有责任
4. 当前 detour create / clear / recover 已不再由两边各自偷偷养完整逻辑

### 结局 B：detour lifecycle 退壳完成后，再顺势推进一小步 `arrival / cancel / path-end`

前提只有一个：

> 先把 detour lifecycle 这刀做实。

---

## 七、这轮仍不要求 fresh live 先行，但也不准拿它当挡箭牌

我继续接受：

- 当前 fresh compile / fresh live 可能仍受外部 blocker 影响

但你只能保留：

- `external_blocker_note`

它不能再占主叙事。

## 只有在下面两条都成立时，你才允许停在“等验证”：

1. detour lifecycle 这一整簇已经真实迁出
2. 你当前下一个具体代码动作，客观上必须依赖 fresh compile / fresh live 才能决定

否则不准停。

---

## 八、如果 blocker 中途解除了

如果这轮执行过程中，external blocker 消失，compile / live 恢复：

那你不能只停在静态验证。

至少补：

1. fresh compile / tests
2. 至少一条最相关 live：
   - `PlayerAvoidsMovingNpc`

如果够条件，再补：

- `NpcAvoidsPlayer`
- `NpcNpcCrossing`

但这是 detour lifecycle 做实后的后补，不是前置条件。

---

## 九、下一次回执必须正面回答的问题

### 1. 这轮具体由共享执行层多接手了哪些 detour 状态 / 入口

### 2. `PlayerAutoNavigator` 具体少了哪几段 detour 私有责任

### 3. `NPCAutoRoamController` 具体少了哪几段 detour 私有责任

### 4. 当前还有哪些 old fallback / private loop 仍然没退

### 5. 你这轮到底是 wrapper 还是 detour lifecycle 的真 owner 迁移

### 6. 如果你这轮仍然停下，你下一个具体代码动作是什么，为什么它客观上必须等 fresh compile / live

---

## 十、最后一条

你现在这条线已经不是“证明共享层能接一两个 helper”了。

你现在要证明的是：

> 玩家和 NPC 的动态 detour 生命周期，  
> 到底能不能从“两个控制器各养各的私货”  
> 继续往“共享执行层居中 owning”这条路上真正走下去。

所以这轮别再摇摆，直接打穿这一刀。
