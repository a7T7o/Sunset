# 2026-04-15｜给 NPC｜Day1-V3 协作边界与 facade 落地 prompt

## 1. 当前同步

`Day1` 已进入 `Day1-V3`。

当前方向已经明确：

1. `Day1` 负责剧情语义
2. `NPC` 负责 resident state 与 locomotion facade
3. `导航` 负责 movement execution

这轮不是要你回吞 `Day1 phase` 或 `导航策略`，而是要你把你已经形成的 locomotion contract 草案，继续压成真正可迁移的 facade 方案。

## 2. 我为什么判断这部分应由你主导

我当前的判断是：

`NPC facade 的落地，应该由 NPC 线程主导，不应该继续由 Day1 线程越过你的边界直接改低级 locomotion API。`

原因：

1. 你已经明确识别出高危 public 写口。
2. 这些口子本来就属于 `NPCAutoRoamController / NPCMotionController` 自己的对外合同面。
3. Day1 线程继续自己吞这部分，只会再次形成“剧情线程顺手深碰身体”的坏模式。

## 3. 你的目标

这轮你只收下面这些：

1. 最小外部 facade
2. 现有危险 public 口分级
3. `Day1 / 导航` 的合法调用面
4. 迁移建议

不要回吞：

1. `Day1 phase / beat / cue`
2. `导航` 的 pathing / avoidance / replan 策略

## 4. 当前 Day1 需要什么

Day1 最终需要的，不是低级原语，而是这些语义口：

1. `AcquireStoryControl(ownerKey)`
2. `ReleaseStoryControl(ownerKey, releaseMode)`
3. `RequestStageTravel(start,end,timeout)`
4. `RequestReturnToAnchor(anchor)`
5. `RequestReturnHome(target)`
6. `SnapToTarget(target)`

导航最终需要的，不是 `phase`，而是这些执行口：

1. `BeginAutonomousTravel(target)`
2. `BeginReturnHome(target)`
3. `ResumeAutonomousRoam()`
4. `AbortAndReplan(reason)`

## 5. 你当前最该继续压实的部分

请你基于你已经形成的合同草案，继续只做这一件事：

`把“现有 public 方法 -> 目标 facade -> 调用方迁移建议”压成执行表。`

至少覆盖：

1. `AcquireResidentScriptedControl`
2. `DriveResidentScriptedMoveTo`
3. `ReleaseResidentScriptedControl`
4. `ApplyProfile`
5. `SetHomeAnchor`
6. `RefreshRoamCenterFromCurrentContext`
7. `StopRoam`
8. `RestartRoamFromCurrentContext`
9. `DebugMoveTo`
10. `NPCMotionController` 的直接写口

## 6. 你的红线

1. 不要再让外部线程直接组合 `StopRoam / RestartRoam / DebugMoveTo / ApplyProfile`。
2. 不要把 facade 做成另一套“换名字的低级 public 原语”。
3. 不要回吞 `Day1` 的剧情语义。
4. 不要回吞 `导航` 的 pathing 策略。

## 7. 你后续回执最好回答什么

1. 现有 public 口里哪些必须 internal-only
2. 哪些是 runtime-only / debug-only
3. 哪些 facade 先落，能让 `Day1` 立即停止深碰身体
4. `Day1` 与 `导航` 未来各自还能合法碰什么

## 8. 当前我的主导判断

我当前不建议让 Day1 线程自己把 NPC facade 一并吞掉。

我建议：

1. `Day1-V3` 主导语义边界和 call-site 迁移目标
2. `NPC` 主导 facade 本体与 public API 收口
3. `导航` 主导统一 movement execution contract

这样三方才不会再次越权缠回去。
