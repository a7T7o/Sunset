# 2026-04-14｜导航线程｜Day1 边界重划后非剧情态 roam 静态契约收口 prompt - 67

## 当前已接受的基线

1. `opening` 站位、对白前强制就位、对白后 release、`18:00/19:30/20:00/21:00/26:00` 的 Day1 时间语义，全部归 `spring-day1` own。
2. 当前用户最痛的“剧情开始时站位不对、剧情结束后反而才到位、resident 到 `20:00` 前像树一样站死”，不是你来代修的面。
3. 你 own 的是：
   - NPC / 动物在 **已经离开剧情接管之后**
   - 进入 autonomous / free-roam 时
   - 如何和树、房子、围栏、水、桥、静态障碍与其它 agent 一起稳定行走
4. 这轮不能再把 `day1 owner 漏释放`、`opening scripted staging`、`晚饭 actor 未到位` 混成导航锅。

## 当前唯一主刀

只收这一刀：

`把 NPC / 动物在非剧情态 autonomous / free-roam 下的静态障碍执行契约收平，让它们在真正被 Day1 放人后，能像玩家一样服从同一套静态真相，不再出现贴房、顶树、围栏边抖动、只会互相推搡不会尊重场景的坏相。`

## 这轮允许的 scope

### 允许主刀的代码面

- [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
- [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
- [NavigationAgentRegistry.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs)
- [NavigationLocalAvoidanceSolver.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs)
- [NavigationAvoidanceRules.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationAvoidanceRules.cs)
- [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs)

### 允许在“真 blocker 命中时”最小核对的资产

- [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 里的导航静态障碍配置
- NPC / 动物 prefab 的导航相关 profile

前提只有一个：

`只允许为了确认 NPC/动物非剧情态的静态真值收集与执行契约；不允许扩成 Day1 marker、剧情点位、晚饭 staging、resident scripted control 的调查。`

## 这轮必须回答的 4 个问题

1. 为什么 NPC / 动物一旦真正进入非剧情态 roam，还会出现贴房、顶树、围栏边抖动、被玩家推着走或互相僵持？
2. 玩家的静态障碍处理为什么看起来比 NPC 稳？当前差额到底缺在哪一层？
3. 这条缺口是否同时解释了普通 NPC 和小动物的坏相？
4. 如果 Day1 已经正确 release，但 NPC 仍然病态，这个第一真问题 exact 落在你 own 的哪一个方法或 contract 上？

## 这轮明确禁止的漂移

1. 不要再碰 [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)。
2. 不要再碰 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)。
3. 不要去判 opening marker、对白前强制就位、晚饭 5 秒等待和超时瞬移。
4. 不要把“剧情态 actor 沿 authored route 正常走”偷换成“非剧情态 roam 也没问题”。
5. 不要扩成“统一导航大重构”。
6. 不要为了证明自己能抓更多日志，再把问题范围拉回 `day1 owner 漏释放`。

## 这轮的完成定义

### A. 第一真问题在导航 own

你要同时交出：

1. exact root cause
2. 最小补口
3. 最小代码层验证
4. live 入口必须是 **Day1 已放人后的非剧情态 roam / 动物 roaming**
5. 明确说明为什么这刀能改善：
   - 静态障碍尊重
   - agent 间避让
   - 卡住时的 abort / replan

### B. 第一真问题不在导航 own

你要同时交出：

1. exact owner
2. exact file
3. exact method
4. exact condition
5. 为什么这件事必须先由别的 owner 收完，你这边才值得继续动刀

## 收口纪律

1. 这轮如果形成导航 own 的高风险改动，必须在本轮收成最小可回退 checkpoint。
2. 不允许把 `NPCAutoRoamController / NavigationLocalAvoidanceSolver / TraversalBlockManager2D` 这类高风险半成品挂在 shared root 上离场。
3. 回执里要报实：
   - 当前是否已经形成可直接 `sync / commit` 的最小 checkpoint
   - 如果还没有，exact blocker 是什么
   - 当前回退时最小应该回到哪一个 own checkpoint

## 这轮必须对 Day1 的边界承认什么

1. 如果当前 live 里 NPC 还没真正进入非剧情态 roam，而是仍被 Day1 scripted control / daytime baseline / return-home 逻辑持有，那不是导航修口时机。
2. 只有在 `spring-day1` 已经把 opening scripted staging、白天 release、`20:00/21:00` policy 收平后，你的 live 样本才算有效。
3. 你可以要求 Day1 给你更干净的入口，但不能越权替它收 opening / 夜晚状态机。

## 固定回执格式

### A1 保底六点卡

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### A2 用户补充层

必须额外回答：

1. 你这轮证明了哪些坏相确实发生在“非剧情态 roam”而不是 Day1 scripted 持有期
2. 玩家与 NPC 当前静态障碍契约到底差在哪
3. 小动物是否与普通 NPC 同根
4. 你这轮最核心的判断
5. 你为什么认为这个判断成立
6. 最薄弱、最可能看错的点
7. 自评

### B 技术审计层

至少包含：

- 当前在改什么
- changed_paths
- 当前 failing scene / failing group
- exact static-truth / avoidance / facing caller chain
- code_self_check
- pre_sync_validation
- 当前是否可以直接提交到 `main`
- blocker_or_checkpoint
- 当前 own path 是否 clean
- 一句话摘要

[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：

- 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 如果还没跑，原因是什么
- 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住
