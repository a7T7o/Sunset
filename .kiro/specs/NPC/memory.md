# NPC - 活跃入口记忆

> 2026-04-10 起，旧根母卷已归档到 [memory_0.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/NPC/memory_0.md)。本卷只保留 NPC 主线分流与恢复点。

## 当前定位
- 本工作区不再把 resident、关系页、头像真源、自漫游、避让恢复继续堆回一条根卷。

## 当前状态
- **最后校正**：2026-04-10
- **状态**：活跃卷已重建
- **当前活跃阶段**：
  - `2.1.0_resident与关系页收口`
  - `2.2.0_自漫游与避让收口`

## 当前稳定结论
- NPC 当前真正活着的是两条线：
  1. resident 化、`NPC_Hand` 真源、关系页与人物简介结构
  2. 自漫游、避让响应、障碍自动收集与 motion 收薄

## 当前恢复点
- 关系页、头像、resident、人物介绍问题进 `2.1.0`
- roam、avoidance、motion、obstacle 问题进 `2.2.0`
- 查旧阶段全量过程时，再看 `memory_0.md` 与 `2.0.0进一步落地/memory_0.md`

## 2026-04-13｜NPC 当前可归仓内容先收成最小 resident checkpoint
- 用户目标：
  - 用户要求我按 NPC 自己的历史边界，把现在真能合法提交的内容先提交掉，不继续扩功能。
- 本轮完成：
  - 已把 resident / 关系页线里当前唯一稳定可拆出的最小代码包提交到 `main`：
    - `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs`
    - `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs.meta`
  - 提交 SHA：
    - `66dadf93cdd9e3b29d67162b192804daec9757ac`
- 当前阶段判断：
  - NPC 线程现在不是“完全没有可提交内容”，但也远没到“大包都能一起交”的状态；
  - 当前能安全归仓的是孤立且编译闸门通过的 `NpcCharacterRegistry`；
  - 其余 resident / bubble / runtime / tests / assets 仍被 mixed roots、remaining dirty、shared blocker 卡住。
- 当前恢复点：
  - 如果后续继续做“能提的就提”，优先找这种：
    - own root 干净
    - `preflight=True`
    - CLI `validate_script` 至少不是 `own_red`
    的最小白名单切片；
  - 不再把大块 mixed roots 当成“顺手一起提”的候选。

## 2026-04-14｜Player build 日志拆责补记
- 用户目标：
  - 用户要求我从打包日志里只拆跟 `NPC` 线有关的问题，并把“我的问题”和解决方案分开说清楚。
- 本轮稳定结论：
  1. 当前打包真正失败的 fatal error 是 `DayNightManager` 的 editor-only 方法调用泄漏，不是 `NPC` own build blocker。
  2. `NPC` 线这次在日志里真正命中的，是 `NPCMotionController` 里的 4 条 editor-diagnostic warning；
     - 它们会脏日志；
     - 但不会单独导致 build fail。
- 当前恢复点：
  - 后续如果用户让我继续修打包：
    1. 先修 `DayNightManager` fatal error；
    2. 再顺手收 `NPCMotionController` 这批 player-warning。

## 2026-04-14｜NPC 自己的 warning 已按最小补丁收口
- 用户目标：
  - 用户要求我先修自己这条线的 warning，但不能碰高危逻辑，风险不明就停。
- 本轮完成：
  - 已只改 `NPCMotionController.cs`；
  - 已把 4 个仅供 editor 诊断的 mismatch 字段收进 `UNITY_EDITOR`；
  - 已把 `ResetFacingMismatchDiagnostics()` 改成 player 下空操作。
- 当前阶段判断：
  - 这条 `NPC` own warning 刀已经做完；
  - 当前剩余阻塞已经不是 `NPCMotionController` 自己，而是外部 `DayNightOverlay / DayNightManager` 红面。
- 当前恢复点：
  - 如果用户继续要求我修打包，这条 NPC warning 不需要再反复返工；
  - 下一步该转去处理外部 render/day-night 问题。

## 2026-04-14｜warning 刀尾验补记
- 已补跑：
  - `check-skill-trigger-log-health.ps1`：`Health: ok`、`Canonical-Duplicate-Groups: 0`
  - `validate_script Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
- 结论：
  1. `NPCMotionController.cs` 当前没有新的 own error / own warning；
  2. CLI assessment 这次落在 `unity_validation_pending`，根因是 Unity `stale_status`，不是这把 warning 补丁引入的新红；
  3. 本轮用户指定要先收的 4 条 `CS0414` 已不再是当前主阻塞。

## 2026-04-15｜只读剖腹补记：NPC 当前本体已可拆成 5 层，但 locomotion owner 仍未收成单一真值
- 用户目标：
  - 用户要求我把 `NPC` 自己这条线的五脏六腑彻底讲清楚，作为给 `Day1` 和 `导航` 直接使用的参照，而不是继续泛讲“是谁的锅”。
- 本轮方式：
  - 只读分析；
  - 不进真实施工；
  - 不跑 `Begin-Slice`，当前继续 `PARKED`。
- 当前稳定结论：
  1. `NPC` 本体现在已经不是一个脚本，而是至少 5 层：
     - 身份主表层：`NpcCharacterRegistry`
     - 内容语义层：`NPCDialogueContentProfile` + `NPCRoamProfile`
     - 运动/漫游层：`NPCMotionController` + `NPCAutoRoamController`
     - 会话/气泡层：`NPCInformalChatInteractable` + `PlayerNpcChatSessionService` + `NPCBubblePresenter` + `PlayerThoughtBubblePresenter`
     - resident/runtime bridge 层：`NpcResidentRuntimeContract` + `NpcResidentRuntimeSnapshot`
  2. 这 5 层里，当前最健康的是：
     - 身份主表 / 头像真源 / 关系页消费
     - 基础内容资产映射
  3. 当前最病的是：
     - locomotion owner
     - resident 释放后的回家/回 roam 生命周期
     - 原因不是 body 缺脚本，而是 `Day1 crowd` 和 `导航/NPC locomotion` 还在同时写同一段运行时真值。
  4. 当前最该给 `Day1` 的边界是：
     - 只决定 `谁被接管 / 何时释放 / 去哪 / 哪个 beat 可消费`
     - 不该继续深改 roam lifecycle 与 scripted/debug move fallback
  5. 当前最该给 `导航` 的边界是：
     - 收 `NPCAutoRoamController` 及其 shared traversal 契约
     - 尤其是 resident return-home / release-to-roam / debug move 这些非纯 free-roam 路径
     - 不该再只盯 free-roam 表面参数
- 当前恢复点：
  - 下次如果继续做“责任图 -> 施工口令”这条线，不该再从症状讲起；
  - 应直接按这 5 层拆成：
    1. `NPC own`
    2. `Day1 consume only`
    3. `导航/locomotion own`
    4. `shared contract`
    5. `绝对禁止再跨写`

## 2026-04-15｜只读补记：NPC locomotion 对外合同面草案
- 用户目标：
  - 用户要求我不要回吞 `Day1` 或 `导航`，而是只把 `NPC locomotion` 的对外调用合同收干净，列清哪些口子现在太脏、未来外部还能合法调什么。
- 本轮方式：
  - 只读分析；
  - 不进真实施工；
  - 不跑 `Begin-Slice`，当前继续 `PARKED`。
- 当前稳定结论：
  1. 现在真正危险的不是 `Day1` phase 语义本身，而是外部线程能直接组合这些 public 口：
     - `StartRoam`
     - `RestartRoamFromCurrentContext`
     - `StopRoam`
     - `HaltResidentScriptedMovement`
     - `AcquireResidentScriptedControl`
     - `DriveResidentScriptedMoveTo`
     - `PauseResidentScriptedMovement`
     - `ResumeResidentScriptedMovement`
     - `ReleaseResidentScriptedControl`
     - `ClearResidentScriptedControl`
     - `ApplyProfile`
     - `SetHomeAnchor`
     - `RefreshRoamCenterFromCurrentContext`
     - `DebugMoveTo`
     - 以及 `NPCMotionController` 的 `SetFacingDirection / SetExternalVelocity / SetExternalFacingDirection`
  2. 这些口里，真正该保留给外部的不是“内部状态机原语”，而是更小的合同语义：
     - `AcquireStoryControl`
     - `ReleaseStoryControl`
     - `RequestStageTravel(start,end,timeout)`
     - `RequestReturnToAnchor(anchor)`
     - `SnapToTarget(target)`
     - `BeginAutonomousTravel(target)`
     - `BeginReturnHome(target)`
     - `ResumeAutonomousRoam`
     - `AbortAndReplan`
  3. 现状最脏的例子已经很明确：
     - `SpringDay1NpcCrowdDirector` 现在还在内部拼装 `SetHomeAnchor + ApplyProfile + RefreshRoamCenter + StopRoam + RestartRoam + DebugMoveTo`
     - 这就是当前 API 面已经脏掉的直接证据
  4. 夜间合同也不该继续分散在两套 Day1 私房逻辑里：
     - `SpringDay1NpcCrowdDirector` 里有 resident 的 `20:00/21:00/9:00`
     - `SpringDay1Director` 里还有 story actor 的 night-rest 调度
     - 后续应统一落到 locomotion contract 消费，而不是继续让两边各写一套时段逻辑
- 当前恢复点：
  - 如果下一轮继续，不该再问“谁来修回家路”这种泛问题；
  - 直接进入：
    1. public 高危口分级
    2. external-safe facade
    3. internal/runtime/debug 标签
    4. Day1 / 导航各自允许触碰的最小集合

## 2026-04-15｜按固定回执格式复述：Day1 解耦后 resident 状态合同与外部调用面收口
- 用户目标：
  - 用户要求我严格按 `2026-04-15_给NPC_Day1解耦后resident状态合同与外部调用面收口prompt_01.md` 的固定格式回卡。
- 本轮方式：
  - 只读分析；
  - 不进真实施工；
  - 不跑 `Begin-Slice`，当前继续 `PARKED`。
- 当前稳定结论：
  1. 上一轮草案方向与本次 prompt 新基线一致；
  2. 这次只需要把结论按固定的 `A1 / A2 / B + 四张表` 重新压实，不需要新增代码判断；
  3. 当前最核心的收口目标仍然是：禁止外部线程继续直接组合 `StopRoam / RestartRoam / DebugMoveTo / ApplyProfile` 改内部状态机。
- 当前恢复点：
  - 如果后续继续推进代码刀，下一步就该把这份固定回卡继续压成“现有 public 口 -> 目标 facade -> 调用迁移建议”的执行稿。

## 2026-04-15｜弱引导补记：NPC own 的 facade 主导权与唯一下一刀
- 用户目标：
  - 用户要求我读取 `Day1-V3` 的弱引导同步，只回答 `NPC own` 的部分：
    - facade 是否仍应由 NPC 主导
    - 哪些高危 public 写口必须先 internal-only
    - Day1 最先该停止碰哪些低级 API
    - 最小可落地 facade 面
    - 我自己的唯一下一刀
- 本轮方式：
  - 只读分析；
  - 不进真实施工；
  - 不跑 `Begin-Slice`，当前继续 `PARKED`。
- 当前稳定结论：
  1. facade 仍应由 `NPC` 线程主导落地；
     - 因为 facade 本体就是 `NPCAutoRoamController / NPCMotionController` 的对外合同面；
     - 让 `Day1` 自己继续吞这层，只会继续形成“剧情线程顺手深碰身体”的旧坏模式。
  2. 第一批必须优先 internal-only 的，不是所有 public，而是最容易让外部直接拼内部状态机的那组：
     - `StopRoam`
     - `RestartRoamFromCurrentContext`
     - `ApplyProfile`
     - `SetHomeAnchor`
     - `RefreshRoamCenterFromCurrentContext`
     - `DebugMoveTo`
     - `NPCMotionController.SetFacingDirection`
     - `NPCMotionController.SetExternalVelocity`
     - `NPCMotionController.SetExternalFacingDirection`
  3. `Day1` 最先应该停止碰的低级 API，就是上面这批；
     - 其中最危险的是 `SetHomeAnchor + ApplyProfile + RefreshRoamCenter + StopRoam/RestartRoam + DebugMoveTo`
  4. 最小可落地 facade 面仍应保持语义级，而不是换名字的低级原语：
     - `AcquireStoryControl`
     - `ReleaseStoryControl`
     - `RequestStageTravel`
     - `RequestReturnToAnchor`
     - `RequestReturnHome`
     - `SnapToTarget`
  5. 我自己的唯一下一刀应是：
     - 先把“现有 public 方法 -> 目标 facade -> 调用方迁移建议”压成正式执行表；
     - 先让 `Day1` 能立刻停止深碰身体，再谈真正代码迁移。
- 当前恢复点：
  - 如果继续推进，最合理的下一步不是先动实现，而是先把 API 迁移表写死，作为 `NPC` 主导的 facade 落地蓝图。

## 2026-04-15｜补记：已收到新版 NPC prompt，目标改成外部调用面收口而非继续吞 Day1 / 导航语义
- 这轮新增 prompt：
  - [2026-04-15_给NPC_Day1解耦后resident状态合同与外部调用面收口prompt_01.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/NPC/2026-04-15_给NPC_Day1解耦后resident状态合同与外部调用面收口prompt_01.md)
- 新 prompt 固定的重点：
  1. NPC own 的不是 opening / dinner 的剧情相位；
  2. NPC own 的是 locomotion 身体对外合同面；
  3. 需要把高危 public 写口、最小 facade、以及 Day1 / Navigation 的合法调用边界正式列清。
- 当前恢复点：
  1. 后续如果 NPC 线程继续，不应再泛讲“谁的锅”；
  2. 直接按 `prompt_01` 输出高危口、最小合同面、方法分级和合法触点集合。

## 2026-04-15｜只读复核：SpringDay1Director / SpringDay1NpcCrowdDirector 现存 NPC 高危触点复盘
- 用户目标：
  - 只读检查 `SpringDay1Director.cs` 与 `SpringDay1NpcCrowdDirector.cs`，确认 NPC 回执点名的高危 API 触点还剩哪些，并区分哪些只是这轮允许保留的 staging/release 合同触点。
- 本轮方式：
  - 只读分析；
  - 不改代码；
  - 不跑 `Begin-Slice`，当前仍是 `PARKED` 语义。
- 当前稳定结论：
  1. 名单内仍残留的高危触点主要是 `SetHomeAnchor`、`ApplyProfile`、`StopRoam`、`AcquireResidentScriptedControl`、`ReleaseResidentScriptedControl`，其中 `RefreshRoamCenterFromCurrentContext`、`RestartRoamFromCurrentContext`、`DebugMoveTo` 在这两个文件里本轮未命中。
  2. `SpringDay1NpcCrowdDirector` 里 `AcquireResidentDirectorControl / ReleaseResidentDirectorControl` 这一层仍可视为 staging/release 合同包装口，但它内部还在直达 `AcquireResidentScriptedControl / ReleaseResidentScriptedControl`，说明合同口还没从底层高危 API 真正隔离出来。
  3. `SpringDay1Director` 里直接命中的 `AcquireResidentScriptedControl / ReleaseResidentScriptedControl / StopRoam / SetHomeAnchor` 仍需警惕；这些不是纯粹的 `CrowdDirector` 包装口，而是 Day1 侧仍在直接碰 locomotion 内核。
  4. 本轮还看到了名单外的新越界点：两边都还在直接写 `NPCMotionController`，不只是 `StopMotion`，还包括 `SetFacingDirection`，而 `SpringDay1Director` 还额外直接写了 `SetExternalVelocity`；此外两边还直接碰 `StartRoam`、`DriveResidentScriptedMoveTo`、`Halt/Pause/ResumeResidentScriptedMovement` 这类更深的 resident movement 口。
- 当前恢复点：
  - 如果下一轮继续 NPC 合同收口，不该再只盯旧名单；
  - 应把 `NPCMotionController` 直接写口、`StartRoam`、`DriveResidentScriptedMoveTo`、以及 scripted movement 暂停/恢复链一起纳入外部调用面清单。

## 2026-04-15｜真实施工：NPC locomotion facade 第一刀已落地
- 当前主线：
  - 按 `2026-04-15_给NPC_Day1解耦后resident状态合同与外部调用面收口prompt_01.md`，把 `NPC locomotion` 的对外调用面真正落成代码，不再停在只读清单。
- 本轮已完成：
  1. 在 `NPCAutoRoamController` 里新增了一组语义级 façade：
     - `AcquireStoryControl`
     - `ReleaseStoryControl`
     - `RequestStageTravel`
     - `RequestReturnToAnchor`
     - `RequestReturnHome`
     - `SnapToTarget`
     - `BeginAutonomousTravel`
     - `BeginReturnHome`
     - `ResumeAutonomousRoam`
     - `AbortAndReplan`
     - `BindResidentHomeAnchor`
     - `SyncRuntimeProfileFromAsset`
     - `ApplyIdleFacing`
  2. 把最危险的 resident scripted 低级口真正收回控制器内部：
     - `AcquireResidentScriptedControl`
     - `ReleaseResidentScriptedControl`
     - `DriveResidentScriptedMoveTo`
     - `PauseResidentScriptedMovement`
     - `ResumeResidentScriptedMovement`
     - `HaltResidentScriptedMovement`
     - `SetHomeAnchor`
     - `ApplyProfile`
     - `RefreshRoamCenterFromCurrentContext`
  3. 新增 `NpcLocomotionSurfaceAttribute` 与 `NpcLocomotionSurfaceScope`，把 façade / runtime-only / debug-only 直接标进源码。
  4. 在 `NPCMotionController` 补了更高层的身体口：
     - `ApplyIdleFacing`
     - `ApplyDirectedMotion`
     并把旧的 `SetFacingDirection / SetExternalVelocity / SetExternalFacingDirection` 标成 runtime-only。
  5. 已把 `Day1` 侧最脏的直调迁到 façade：
     - `SpringDay1NpcCrowdDirector.cs`
     - `SpringDay1Director.cs`
     - `SpringDay1DirectorStaging.cs`
  6. 已把 NPC own 的正式/非正式聊天冻结逻辑迁到 façade：
     - `NPCDialogueInteractable.cs`
     - `NPCInformalChatInteractable.cs`
  7. 新增反射型 EditMode 守卫测试：
     - `NpcLocomotionFacadeSurfaceTests.cs`
     - 负责钉死“哪些 façade 必须公开、哪些低级 resident scripted 口必须不再 public、哪些方法必须带 surface 标记”
- 关键判断：
  1. 这轮已经不是“提建议”，而是 `NPC` 线程真的把 locomotion 身体对外合同面收回到自己手里了。
  2. `Day1` 现在可以继续合法碰的，已经被收成 façade 语义口；它不该再直接摸 resident scripted 内核。
  3. `NavigationLiveValidationRunner` 继续保留 `DebugMoveTo / StopRoam / SyncHomeAnchorToCurrentPosition` 这一类 debug/runtime 用口，避免把 probe 工具也误伤成业务 contract。
- 验证：
  - `manage_script validate`
    - `NPCAutoRoamController.cs`：`errors=0`
    - `NPCMotionController.cs`：`errors=0`
    - `SpringDay1Director.cs`：`errors=0`
    - `SpringDay1NpcCrowdDirector.cs`：`errors=0`
    - `SpringDay1DirectorStaging.cs`：`errors=0`
    - `NPCInformalChatInteractable.cs`：`errors=0`
    - `NPCDialogueInteractable.cs`：`errors=0`
    - `NpcLocomotionFacadeSurfaceTests.cs`：`errors=0`
  - `errors`：fresh console `0 error / 0 warning`
  - `git diff --check`：本轮 own 文件 clean
  - Unity EditMode targeted test：
    - `run_tests(test_names=NpcLocomotionFacadeSurfaceTests...)` 两次都返回 `total=0`
    - 当前只能诚实记成“Unity test filter 没真正发现 case”，不能包装成测试已过
- 当前恢复点：
  1. 如果下一轮继续 NPC own：
     - 优先把这套 façade contract 写回给 `Day1` / `导航`，让他们按新口消费；
     - 然后再决定是否继续把 `StopRoam / StartRoam` 这类 runtime-only 口再收窄一层。
  2. 如果这轮先停：
     - 当前代码层 contract 已成立；
     - 真正还没拿到的，只剩 Unity test filter 层面的可运行证据，不是编译红错。

## 2026-04-16｜只读审计：Town 自由时段群体 roam 热点判定
- 当前主线目标：
  - 用户要求在不改文件的前提下，只读审计 `NPCAutoRoamController.cs`，判断 Town 自由时段大量 NPC 同时 roam 时最可能的性能热点链、低风险优化点，以及 `UpdateTraversalSoftPassStateOncePerFrame / TryHandleSharedAvoidance / AdjustDirectionByStaticColliders / NavigationAgentRegistry.GetNearbySnapshots / ambient bubble` 的优先级。
- 本轮子任务：
  - 只读核对 `Update / FixedUpdate / TickMoving`；
  - 只读展开 `TryHandleSharedAvoidance`、`AdjustDirectionByStaticColliders`、`UpdateTraversalSoftPassStateOncePerFrame`；
  - 交叉核对 `NavigationAgentRegistry`、`NavigationLocalAvoidanceSolver`、`NPCBubblePresenter`、`NpcInteractionPriorityPolicy`。
- 本轮稳定结论：
  1. 最大热点链已收敛为：
     - `TickMoving -> TryHandleSharedAvoidance -> NavigationAgentRegistry.GetNearbySnapshots -> NavigationLocalAvoidanceSolver.Solve / ApplyCloseRangeConstraint`
     - 这里最伤的不是单个分支判断，而是 registry 只缓存 active unit 列表、不缓存 snapshot；每个 moving NPC 每次查询都会重新 `FromUnit(...)` 全量活跃代理，再被 solver 二次线性扫描，Town 拥挤时近似放大成 `O(N^2)`。
  2. 第二热点链是：
     - `TickMoving -> AdjustDirectionByStaticColliders -> ProbeStaticObstacleHits`
     - 当前一次重决策最多做 3 次 `Physics2D.OverlapCircle`，并对每个 hit 再跑 `ClosestPoint / bounds.center / repulse`，这比 soft-pass 更像会直接打出卡顿峰值的物理查询链。
  3. `UpdateTraversalSoftPassStateOncePerFrame` 不是第一嫌疑：
     - 它已经有 `same-frame / 位移 / 0.08s` 节流；
     - 真正执行时主要是 nav-grid override 支撑探针，不像共享避让和静态 steering 那样会在拥挤 roam 中形成高放大倍数。
  4. ambient bubble 链不是 roam 主热点，但有一个真实的背景成本：
     - `TryStartAmbientChat / FindAmbientChatPartner` 只在长停或 retry 触发，频率低于移动链；
     - 更值得盯的是 `NPCBubblePresenter.LateUpdate`，气泡 UI 一旦创建，隐藏态也会继续每帧 `SyncCanvasTransform + ApplyTailBob + SyncSorting`，大量 NPC 都进过长停后会形成持续 UI 开销。
  5. 当前最值得动的低风险优化方向：
     - `NavigationAgentRegistry.GetNearbySnapshots` 增加 per-frame snapshot cache，直接砍掉重复 `FromUnit(...)`；
     - `AdjustDirectionByStaticColliders` 改成逐级 probe，先近距命中再扩到中/远 probe；
     - 第三刀按现场二选一：如果卡顿主要发生在拥挤移动，先收 `UpdateTraversalSoftPassStateOncePerFrame` 的 idle 轮询；如果 Town 自由时段大量气泡常亮/刚亮过，先收 `NPCBubblePresenter.LateUpdate` 的 hidden 早退。
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationAgentRegistry.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NpcInteractionPriorityPolicy.cs`
- 验证结果：
  - 只做静态代码审计；
  - 通过 `rg / Get-Content` 核对调用链；
  - 当前仓库位于 `main@8450530e`；
  - 未运行 Unity / MCP / 测试；
  - 未执行 `Begin-Slice / Ready-To-Sync / Park-Slice`，因为本轮始终停留在只读分析。
- 当前恢复点：
  - 如果下一轮进入真实施工，最稳的首刀是 registry snapshot cache，其次是 static collider 渐进 probe；
  - 当前这轮已经足够支持用户直接在这 2-3 处里选一处下刀，不需要先改语义层。

## 2026-04-18｜resident 头像真源线新增 `202` 对话胸像
- 当前主线目标：
  - 用户要求把 `Assets/Sprites/NPC/202.png` 对应角色补成一张真正可用于对白框的右向胸像，而不是继续走“裁切行走帧放大”的失败路线。
- 本轮完成：
  - 已新增：
    - `Assets/Sprites/NPC_Hand/202.png`
    - `Assets/Sprites/NPC_Hand/202.png.meta`
  - 当前成品已站住：
    - `512x512`
    - 单人右向 bust-up
    - 粉发 / 白色头带 / 黑白女仆服
    - 纯白不透明背景
- 当前阶段判断：
  - 这轮只站住了“头像资产已落地 + 静态自验成立”；
  - 还没站住“Unity import / registry 同步 / DialogueUI runtime 显示已确认”。
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑`
  - `Park-Slice=已跑`
  - 当前 `PARKED`
- 当前恢复点：
  - resident / 关系页 / 对白头像同源线如果继续推进，下一步应先补 Unity 接链验证，而不是立刻把这张图包装成已经正式接进游戏。

## 2026-04-18｜`202` 胸像第一版被用户拒收后，已按 `NPC_Hand` 真实风格链重做
- 当前主线目标：
  - 用户明确退回第一版 `202` 胸像，要求我先学习 `Assets/Sprites/NPC_Hand` 现有内容，再重做；
  - 当前子任务是把 `202` 修回：
    - 透明底
    - 无坏点坏条纹
    - 与现有 `NPC_Hand` 风格一致
- 本轮完成：
  - 已重新比对 `001 / 104 / 201 / 202` 的：
    - 尺寸
    - alpha
    - 构图
    - 透明底口径
  - 已确认：
    - 现有 `NPC_Hand` 资产默认不是实色背景
    - `201` 是本轮最接近的 maid 胸像真源
  - 已重做并覆盖：
    - `Assets/Sprites/NPC_Hand/202.png`
    - `Assets/Sprites/NPC_Hand/202.png.meta`
  - 当前新成品已改成：
    - `536x700`
    - alpha `0~255`
    - 四角透明
- 当前阶段判断：
  - 这轮已经把用户指出的三类问题都正面修掉了：
    - 不学现有资产
    - 背景口径错误
    - 成品有坏点 / 坏条纹
  - 但我仍不把它说成“已经正式接进游戏”，因为 Unity import / registry / DialogueUI runtime 还没验证。
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑`
  - `Park-Slice=已跑`
  - 当前 `PARKED`
- 当前恢复点：
  - 如果继续 resident 头像线，下一步先补 Unity 接链验证；
  - 在此之前，不再把这张图包装成最终 runtime 已过线。
