# 2026-04-15｜给 NPC｜Day1 解耦后 resident 状态合同与外部调用面收口 prompt - 01

## 当前唯一主刀

只收这一刀：

`把 NPC locomotion 的对外调用合同面收干净，不再让 Day1、导航或别的线程直接拼装 NPC 内部状态机。`

这不是让你回吞 Day1 phase，也不是让你回吞导航执行策略。

## 你必须接受的当前新基线

### 1. opening / dinner 的 staged contract 不是你 own

1. opening 和 dinner 的：
   - 起点
   - 终点
   - 5 秒等待
   - 超时 snap
   - 对白期间冻结
   - 对话结束 release
   是 Day1 own。
2. 你不该替 Day1 重写这些剧情相位语义。

### 2. opening / dinner 结束后的 resident movement quality 不是 Day1 own

1. resident 回 anchor/home
2. 回到 anchor 后恢复 roam
3. free-roam 的稳定执行质量

这些执行质量未来主要归导航 / locomotion own。

### 3. 你 own 的是“身体对外合同面”

1. 外部线程到底允许调什么。
2. 哪些 public 写口现在太脏。
3. 哪些现有方法只能 internal-only。
4. 哪些应该变成更小、更安全的 facade。

## 当前用户最新裁定

1. `003` opening 后不应继续特殊化，应与 `101~203` 同合同。
2. `20:00~26:00` 是全日通用夜间合同，不应继续在 Day1 与 NPC 内各写一套私房逻辑。
3. Day1 只负责：
   - 接管
   - 放手
   - 告知 anchor/home
4. NPC 收到“剧情演完了”后，应自己切 resident state。
5. 导航只负责把这条路走好。

## 这轮你必须列清楚的高危 public 写口

至少覆盖这些：

1. `StartRoam`
2. `RestartRoamFromCurrentContext`
3. `StopRoam`
4. `HaltResidentScriptedMovement`
5. `AcquireResidentScriptedControl`
6. `DriveResidentScriptedMoveTo`
7. `PauseResidentScriptedMovement`
8. `ResumeResidentScriptedMovement`
9. `ReleaseResidentScriptedControl`
10. `ClearResidentScriptedControl`
11. `ApplyProfile`
12. `SetHomeAnchor`
13. `RefreshRoamCenterFromCurrentContext`
14. `DebugMoveTo`
15. `NPCMotionController.SetFacingDirection`
16. `NPCMotionController.SetExternalVelocity`
17. `NPCMotionController.SetExternalFacingDirection`
18. `NPCMotionController.StopMotion`

你要明确说清：

1. 哪些必须 internal-only。
2. 哪些只能 runtime-only。
3. 哪些只能 debug-only。
4. 哪些可以保留，但只能作为 facade 内部原语，不该继续裸露给业务线程。

## 你要给出的目标合同

### Day1 未来只该能说

1. `AcquireStoryControl`
2. `ReleaseStoryControl`
3. `RequestStageTravel(start,end,timeout)`
4. `RequestReturnToAnchor(anchor)`
5. `SnapToTarget(target)`

### Navigation 未来只该能说

1. `BeginAutonomousTravel(target)`
2. `BeginReturnHome(target)`
3. `ResumeAutonomousRoam`
4. `AbortAndReplan`

### 外部线程绝对不该继续做

1. 直接组合内部 lifecycle
2. 把 `StopRoam + RestartRoam + DebugMoveTo + ApplyProfile` 拼成业务逻辑
3. 直接抢 `NPCMotionController` 的速度与朝向真值

## 你这轮必须给我的 4 张表

### 1. 当前 NPC locomotion 对外高危口清单

要求：

1. 方法名
2. 文件
3. 为什么危险
4. 当前谁在滥用
5. 继续裸露的风险

### 2. 你建议保留的最小外部合同面

要求：

1. facade 名
2. 给谁用
3. 负责什么语义
4. 明确不负责什么

### 3. 现有方法分级

必须分成：

1. `internal-only`
2. `runtime-only`
3. `debug-only`
4. `保留但只允许 facade 内部消费`

### 4. 合同落地后各线程还能合法碰什么

必须明确：

1. `Day1` 还能合法碰什么
2. `Day1` 不该再合法碰什么
3. `Navigation` 还能合法碰什么
4. `Navigation` 不该再合法碰什么

## 绝对红线

1. 不回吞 Day1 phase / beat 语义。
2. 不回吞导航执行质量策略。
3. 不允许继续让外部线程任意组合 `StopRoam / RestartRoam / DebugMoveTo / ApplyProfile` 去修改内部状态机。
4. 不准拿“现在能跑”来掩盖 API 面已经脏了。
5. 不准把 `003` 再做成特例来迁就 Day1 错位。

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

1. 当前最脏的对外口到底是哪几个。
2. 哪些口如果不收，后面还会继续反复污染 Day1 / 导航。
3. 你这轮最核心的判断。
4. 你为什么认为这个判断成立。
5. 你这轮最薄弱、最可能看错的点。
6. 自评。

### B 技术审计层

至少包含：

- 当前在改什么
- changed_paths
- exact public/api surface inventory
- facade 草案
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
