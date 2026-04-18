# 2026-04-14｜给 spring-day1｜Day1 三线程 owner 边界重划与打包前唯一主刀 prompt v4

## 当前已接受的基线

1. `UI` 不再是你 own 的施工面。玩家可见的任务清单、bridge prompt、PromptOverlay、层级、位置、观感与 `TimeManagerDebugger +/-` 由 `UI` 线程 own。
2. `导航` 不再替你兜 Day1 的剧情相位、opening staged 走位、resident release、`20:00/21:00/26:00` 的 Day1 语义。导航只 own 非剧情态 roam / 动物 / 静态障碍执行契约。
3. 你 own 的不是“某一个晚饭 case”，而是 `Day1 自己的 authored 导演 contract`：
   - 开场必要剧情 actor 在对白出现前一刻就位
   - scripted walk 按 authored `起点 -> 终点` 执行
   - 最多等待 `5` 秒
   - `5` 秒内未就位，只对必要剧情 actor 瞬移到终点
   - 然后立刻开始剧情
   - 剧情结束后原地退场
   - 回到各自正确的 anchor / baseline
4. 用户已经明确说过：这套流程很久以前就是正确且通过过的。你这轮的目标是恢复这条历史正确流程，不是发明新的全局控制器。
5. 现有 scene authored 点位是权威真值。`进村围观 / 起点 / 终点 / 001终点 / 002终点 / 003终点` 这类对象如果已经存在，代码必须消费它们，而不是继续吃 hard fallback。

## 当前唯一主刀

只收这一刀：

`恢复 Day1 自己的 authored staging / release / first-night state machine contract，让 opening 到第一夜结束重新回到历史正确行为；同时把给 UI 与导航的边界写死，不再让别人替 Day1 猜语义。`

## 这轮只允许的 scope

### 允许直接主刀的文件

- [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
- [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
- [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs)

### 允许只读核对或在“确有缺口”时最小补口的资产

- [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity)
- Day1 现有 stagebook / marker / authored route 相关资产

前提只有一个：

`只有当 runtime 明确无法解析当前 scene 已有的 authored marker，才允许做最小 marker 绑定修补；不允许借机重做整套 scene authored 点位。`

## 这轮必须恢复的 P0

### P0-1. opening authored contract

你必须直接收平下面这条链：

- [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的 `TryHandleTownEnterVillageFlow()`
- [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的 `TryPrepareTownVillageGateActors()`
- [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的 `TryResolveTownVillageGateActorTarget()`
- [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的 `TryResolveVillageCrowdMarker(...)`

硬要求是：

1. 进入 opening 剧情时，必要剧情 actor 立刻传送到 `起点`。
2. 然后按 authored route 走向 `终点`。
3. 最多等待 `5` 秒。
4. `5` 秒内还没全部到位，只对必要剧情 actor 瞬移到 `终点`。
5. 对话框出现前一刻，`001/002/003` 的位置和朝向必须已经正确。
6. 不允许再出现“对白已经开始，但剧情 actor 还在错误位置；对白结束后反而才跑去对的位置”。
7. 如果当前 scene 已经有 marker，对应准备函数不允许在 marker 缺失或解析失败时仍然 `return true` 继续往后走。
8. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 里的 hard fallback 旧坐标不准继续覆盖 authored 真值。

### P0-2. scripted 结束后的 daytime release

你必须把下面这条链收成 Day1 自己的一致策略：

- [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 `ApplyResidentBaseline(...)`
- [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 `TryReleaseResidentToDaytimeBaseline(...)`
- [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 `TryBeginResidentReturnHome(...)`
- [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 `ShouldResidentsReturnHomeByClock()`

硬要求是：

1. scripted opening / scripted dinner 一结束，resident 必须先回到白天 baseline / roam。
2. 不允许再出现“剧情后 resident 像树一样站死，到 `20:00` 才突然会回家”的坏相。
3. `003` 不能再成为游离个例；它必须吃同一套 Day1 daytime / nighttime policy。
4. `20:00` 才是“主动回 home anchor”的开始，不是“第一次被放人”的开始。
5. `21:00` 才允许对仍未到位的 resident 做强制 snap 到 home anchor。

### P0-3. Day1 第一夜时间语义

这轮统一按用户最新裁定，不再沿用旧的晚段口径：

1. `18:00` = 晚饭开始
2. `19:30` = 所有晚饭相关剧情收完，玩家恢复自由活动
3. `20:00` = resident 自发回 `home anchor`
4. `21:00` = 仍未到位者才强制 snap 到 `home anchor`
5. `26:00` = Day1 第一夜强制睡觉收束

其中 `26:00` 的 Day1 第一夜语义必须是：

1. 不是 generic 晕倒演出
2. 而是“传送回正确的 home / bed 入口位”
3. 触发正确睡觉转场
4. 直达第二天
5. 摆位正确，不允许掉到奇怪落点

## 这轮明确禁止的漂移

1. 不要再自己改玩家可见 UI 壳体、位置、布局、样式。
2. 不要再去动导航 core、`PlayerAutoNavigator`、`NavGrid2D`、共享静态障碍系统。
3. 不要把 scene authored 点位问题偷换成“再加一层全局剧情抢控制”。
4. 不要把 `20:00` 回家逻辑当成白天 release 的替代品。
5. 不要用 hardcoded fallback 坐标继续绕开已存在的 marker 体系。
6. 不要把“结构推断成立”说成“体验已经过线”。
7. 不要越权替 `UI` 或 `导航` 收他们自己的 own 问题。

## 你与另外两条线程的边界

### 你交给导航的边界

导航只在下面这个前提下接活：

`Day1 已经明确释放 scripted control，NPC 已进入非剧情态 autonomous / roam。`

在这个前提下，导航 own：

- NPC / 动物非剧情态 roam 的静态障碍执行契约
- shared static truth
- local avoidance / blocked abort / replan

不在这个前提下的 opening / dinner / resident release / clock policy，都还是你 own。

### 你交给 UI 的边界

UI own：

- 任务清单
- bridge prompt
- PromptOverlay
- modal 层级
- `TimeManagerDebugger +/-`
- re-entry 后的玩家可见 UI 重建

你 own：

- 这些 UI 的 runtime canonical state
- phase / task / bridge prompt / modal / sleep / re-entry 语义

也就是说：

`你负责真语义，UI 负责玩家可见呈现。`

## 完成定义

只有下面都站住，才算这轮真的过线：

1. fresh opening 下，`001/002/003` 在对白前一刻已经位置正确、朝向正确。
2. opening 不再出现“剧情开始时没就位，剧情结束后才跑去预设点”的倒挂。
3. scripted 结束后，resident 会正常进入 daytime baseline / roam，而不是站死到 `20:00`。
4. `20:00 -> 21:00 -> 26:00` 的 Day1 第一夜时间链符合这轮新口径。
5. `26:00` 强制睡觉会把玩家送到正确 home / bed 入口位并正确转到第二天。
6. 你已经把给导航和 UI 的 owner 边界说清，且没有再越权碰它们的 own 壳层。

## 收口纪律

1. 这轮如果形成高风险 own 改动，必须在本轮收成最小可回退 checkpoint。
2. 不允许把 opening / resident / first-night 的半成品高风险改动继续挂在 shared root 上离场。
3. 回执里要报实：
   - 当前是否已经形成可直接 `sync / commit` 的最小 checkpoint
   - 如果还没有，exact blocker 是什么
   - 当前回退时最小应该回到哪一个 own checkpoint

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

1. opening 的 authored contract 现在恢复到了哪一步
2. resident daytime release 和 `20:00/21:00` 夜间 policy 现在是否统一
3. `26:00` 第一夜强制睡觉现在是否正确
4. 你这轮最核心的判断
5. 你为什么认为这个判断成立
6. 你这轮最薄弱、最可能看错的点
7. 自评

### B 技术审计层

至少包含：

- 当前在改什么
- changed_paths
- 当前是否触碰 [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity)
- exact opening / release / sleep caller chain
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
