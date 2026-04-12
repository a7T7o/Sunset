# 2026-04-06｜spring-day1｜Town 协作线最终收尾阶段清单

这份不是 `Town` 线程的全部任务单。

这份只列 `Day1 最终收尾阶段` 里，`Town` 继续帮 `day1` 扛的那部分内容。

---

## 开始前统一口径

### 允许按需使用 subagent

允许按需使用 `subagent`，但优先用于：

1. scene-side 只读审计
2. 窄口 live 取证
3. 独立 scene-side 子域整理

有问题先回 `spring-day1` 主控台，没有问题就继续下一部分。

### 这份清单的边界

`Town` 这轮不该回吞：

1. `day1` 当前 active 的 director 主刀
2. `CrowdDirector` 主逻辑
3. `UI` 玩家面问题
4. `NPC` 内容层与 formal contract

---

## 当前已核实的真实起点

已核实 [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 里真实存在：

1. `Town_Day1Carriers`
2. `Town_Day1Residents`
3. `EnterVillageCrowdRoot`
4. `KidLook_01`
5. `DinnerBackgroundRoot`
6. `NightWitness_01`
7. `DailyStand_01`
8. `ResidentSlot_*`
9. `DirectorReady_*`
10. `BackstageSlot_*`

所以当前 `Town` 不再是“还没建”的阶段，而是“要继续稳成 Day1 可持续吃的 scene-side 承接层”。

---

## 第一部分：继续把 Day1 可用的 resident scene-side 基线钉死

### 当前必须继续守住的不是空泛边界，而是可用基线

`Town` 现在对 `Day1` 最有价值的交付，是：

1. 哪些 resident root / slot / anchor 已经是正式可吃的
2. 哪些只是 working tree broader dirty 的一部分
3. `day1` 当前到底应该信哪份基线

### 目标

让 `day1` 后续在吃 `Town` 时，不会再发生：

1. 把 broader dirty 当正式基线
2. 把局部可用 scene-side contract 误判成整份 Town fully ready

---

## 第二部分：继续为 Day1 resident runtime 提供窄口、可落的 scene-side 承接

### 当前优先 anchor

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `NightWitness_01`

站住后继续：

4. `DinnerBackgroundRoot`
5. `DailyStand_01`
6. `DailyStand_02`
7. `DailyStand_03`

### Town 当前真正该继续扛的内容

1. 这些 anchor / slot 在 scene 里的承接稳定性
2. resident root / director-ready / backstage 三层语义不要混
3. `day1` 一旦在具体 anchor 上撞 live 问题，Town 可以窄口接刀 scene-side 纠偏

### 这部分的完成定义

不是“Town 讲清楚了”，而是：

1. `day1` 吃这些 anchor 时更稳
2. resident 常驻化不再被 scene-side 空位或混乱层级拖住

---

## 第三部分：把和 Day1 直接相关的 mixed-scene 干扰继续缩小

当前 `Town` 的剩余问题，已经不再主要是 resident 根层没搭。

所以 Town 继续推进时，应优先做的是：

1. 继续分离与 `Day1` 直接有关的 resident / anchor / slot 子域
2. 不让无关的 farmland / camera / manager broader dirty 干扰 `Day1` 判断
3. 在不误吞整份 `Town.unity` 的前提下，为 `Day1` 留出更稳定的 scene-side 承接面

---

## 第四部分：继续把 Town 对 Day1 的“可代接工作面”说清，但只说能真接的

`Town` 继续回给 `day1` 的，不该再是泛泛的“Town 可以支持导演层”。

而应该继续落成：

1. 现在 `day1` 可以继续自己吃的内容
2. 现在 `Town` 可以代接的 scene-side 子域
3. 哪些必须等 `day1` 正式回球后再接
4. 一旦接刀，第一刀具体该改什么

---

## 第五部分：最终交回 day1 时只交什么

`Town` 最终交回 `day1` 的，应该是：

1. 可被当前 Day1 吃的 scene-side resident 基线
2. 窄口 anchor / slot / root 承接真值
3. 与 Day1 直接相关的 mixed-scene 风险说明
4. 当前第一真实 blocker
5. 什么时候该正式把某个 scene-side 触点交回 Town

---

## 当前不该回吞的内容

1. 不要回吞 `day1` 的 active director / StageBook / deployment 主刀
2. 不要回吞 `NPC` resident 内容矩阵
3. 不要回吞 `UI` 玩家面结果
4. 不要把 `Town` 包装成“整份 scene 已 fully ready，可无脑使用”
