# 002-prompt-12

这轮我先把你的最新回执定性清楚：

1. 我接受你已经交出了第三个真实退壳 checkpoint。
2. `detour lifecycle` 迁进共享 `NavigationPathExecutor2D` 这件事，我认。
3. 但我不接受你把这个 checkpoint 包装成“已经快收口了”。

因为最新单场 live 事实非常硬：

- `PlayerAvoidsMovingNpc`
- `pass=False / timeout=6.50 / minClearance=0.526 / playerReached=False / npcReached=False`

并且现场信号已经缩得很清楚：

> 玩家仍卡在 `Wait / SidePass`，  
> NPC 反复命中 `共享交通裁决: 清理 detour => Recovered=True`，  
> 说明现在真正该打的不是 detour owner 迁不迁得动，  
> 而是 **shared detour clear / recover 触发过密**。

---

## 一、当前已接受的基线

当前导航线基线继续固定为：

- `S0`：部分完成
- `S1 / S2 / S4`：部分完成
- `S3 / S5`：未完成
- `S6`：部分完成

当前已接受的真实 checkpoint 有三个：

1. `BuildPath / RebuildPath / ActualDestination / 路径后处理`
2. `stuck / repath / 恢复入口`
3. `detour create / clear / recover`

所以这轮正确起点不是再解释第三个 checkpoint 有多真实。

而是：

> 继续把 detour shared owner 从“能 clear / recover”  
> 推进到“不会因为 clear / recover 过密而把现场震荡死”。

---

## 二、这轮唯一主刀

### 这轮主刀固定为：

> 直接锁  
> [NavigationPathExecutor2D.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs)  
> 里的 `TryClearDetourAndRecover()`，以及玩家 / NPC clear 分支中的  
> **detour clear hysteresis / cooldown / owner 释放条件**。

这轮不准再漂去：

- `arrival / cancel / path-end`
- `先裁决、后求解` 大方向宣讲
- solver 参数泛调

## 就只打“detour clear / recover 过密”这一刀。

---

## 三、为什么这轮必须先打 clear hysteresis / cooldown

因为你现在的 live 已经不是：

- detour 创建失败
- detour 清不掉
- detour 恢复不了

而是：

- detour owner 已经会 clear
- clear 后也会 recover
- 但 clear/recover 太密，导致：
  - NPC 反复重建
  - 玩家一直卡在 `Wait / SidePass`
  - 单场最终 timeout

所以当前第一责任点不是“继续让共享层多接一个入口”。

而是：

> 让共享层开始有“什么时候不该立刻再 clear / recover”的节制能力。

---

## 四、这轮允许的 scope

### 允许集中在：

- [NavigationPathExecutor2D.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs)
- [PlayerAutoNavigator.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs)
- [NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
- [NavigationAvoidanceRulesTests.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs)

### 这轮不允许：

1. 漂回 `arrival / cancel / path-end`
2. 漂回 solver 参数继续泛调
3. 再去把 `NavigationTrafficArbiter` 当成本轮主刀
4. 再把 unrelated test failure 混进主叙事
5. 为了取证跑长窗 full live，把日志刷爆

---

## 五、这轮最少要交出的硬结果

这轮最少要交出的，不是“又多了几个 owner 字段”。

而是：

## shared detour clear / recover 开始有节制

至少要正面处理下面几件事里的主要部分：

1. detour clear hysteresis
2. clear cooldown
3. owner release 条件
4. clear 后多快允许再次进入 recover / rebuild
5. 玩家 / NPC clear 分支何时不该立刻再次打回 detour 流程

如果这些东西还没有开始收口，那 live 就还会继续震荡。

---

## 六、这轮完成定义

只有满足下面任一条，你才算这轮有效完成：

### 结局 A：单场 live 过线

你要明确给出：

1. `PlayerAvoidsMovingNpc` fresh live 过了
2. 当前 clear / recover 过密问题已被压住
3. 这轮具体靠什么条件生效

### 结局 B：单场 live 仍未过，但失败形态被继续压缩

前提是你必须明确证明：

1. 反复 `clear -> recover -> rebuild` 风暴已经减少
2. 失败形态已从“过密震荡”收缩到新的单一责任点
3. 新的第一责任点比现在更窄、更具体

如果只是“还是 fail，但是我又多迁了一点结构”，这轮不算完成。

---

## 七、验证纪律

这轮我把 live 纪律也钉死：

1. fresh compile / targeted tests 可以先做
2. live 只保留：
   - 单场 `PlayerAvoidsMovingNpc`
3. 一旦拿到足够证据，立刻 `Pause / Stop`
4. 完成后必须退回 `Edit Mode`
5. 不准为了“多看一点日志”继续放着跑

---

## 八、你这轮不需要再等什么

你自己已经说得很清楚：

> 不需要等 fresh compile / fresh live 才能决定下一刀。

那我就按这个口径钉死：

- 这轮不接受再停在“等 fresh compile / live”
- 这轮必须直接把刀落在 `TryClearDetourAndRecover()` 与 clear 分支节制条件上

---

## 九、下一次回执必须正面回答的问题

### 1. 这轮具体加了哪些 detour clear hysteresis / cooldown / owner release 条件

### 2. 哪些条件在共享 `NavigationPathExecutor2D` 持有，哪些还留在控制器侧

### 3. 玩家侧具体少了哪几段“过密 clear/recover”私有触发责任

### 4. NPC 侧具体少了哪几段“过密 clear/recover”私有触发责任

### 5. 单场 `PlayerAvoidsMovingNpc` 最新 fresh 结果是什么

### 6. 如果仍 fail，新的第一责任点是否已经继续收窄

### 7. 当前仍残留哪些 old fallback / private loop

---

## 十、最后一条

你现在这条线已经不是“shared owner 有没有接住 detour 生命周期”。

你现在要证明的是：

> shared owner 接住之后，  
> 到底能不能让 detour clear / recover  
> 从“会工作”  
> 变成“不会把现场震荡死”。

所以别再横跳，直接把这一刀打穿。
