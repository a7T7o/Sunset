# NPC - 线程活跃记忆

> 2026-04-10 起，旧线程母卷已归档到 [memory_1.md](D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/NPC/memory_1.md)。本卷只保留当前线程角色和恢复点。

## 线程定位
- 线程名称：`NPC`
- 线程作用：NPC 主线的分流执行入口

## 当前主线
- resident / 关系页 / 头像真源：
  - [2.1.0_resident与关系页收口](D:/Unity/Unity_learning/Sunset/.kiro/specs/NPC/2.1.0_resident与关系页收口/memory.md)
- 自漫游 / 避让 / motion：
  - [2.2.0_自漫游与避让收口](D:/Unity/Unity_learning/Sunset/.kiro/specs/NPC/2.2.0_自漫游与避让收口/memory.md)

## 当前恢复点
- 查旧全量过程、历史回执、共享根清扫材料时看 `memory_1.md`
- 查当前问题时先按内容层 / 行为层分流，不再回到单大卷

## 2026-04-10｜只读审稿补记：导航线程当前说法可采纳一半，但不能混题
- 当前主线目标：
  - 用户要求我直接判断 NPC 导航 / 朝向多轮返工里，到底哪些结论可信、哪些在混题，以及 NPC 线程现在最该提供什么建设性帮助。
- 本轮子任务：
  - 只读回看 `导航检查` 最新 memory 与相关代码；
  - 不进真实施工，不跑 `Begin-Slice`；
  - 重点审两件事：
    1. `scene continuity`
    2. `free-roam 摇摆/摆头鬼畜`
- 本轮稳定结论：
  1. `scene continuity`：
     - 导航线程方向大体正确；
     - 应接现有 `PersistentPlayerSceneBridge / NpcResidentRuntimeContract / SpringDay1NpcCrowdDirector resident snapshot`，不是另起一套完全平行大桥。
  2. `摇摆/摆头鬼畜`：
     - 不是同一个根因；
     - 当前仓库里仍有多处外部 `SetFacingDirection(...)` 写入：
       - `SpringDay1NpcCrowdDirector.cs`
       - `NPCInformalChatInteractable.cs`
       - `NPCAutoRoamController.cs`
     - 因此“导航线程修了 auto-roam 一刀”不等于全项目已经止住用户看到的摇摆。
  3. 责任边界：
     - 这件事和 NPC 线程有关；
     - 但当前不是 NPC 单独独占锅，已经是 NPC + Day1 crowd/director + chat 的 shared/mixed 面。
- 当前恢复点：
  - 如果用户让我继续真实施工，最值钱的第一刀不是立刻扩 `scene continuity bridge`；
  - 而是先在 `NPCMotionController.SetFacingDirection(...)` 做窄口调用源追踪，把“谁在抢脸”钉成 live 证据，再决定 contract 应该收在哪一层。

## 2026-04-10｜只读审稿补记：历史“会走会避让但卡”不能整包回滚
- 当前主线目标：
  - 用户要求我进一步回答：导航线程当前到底在修什么，为什么问题久修不住，以及“历史版本导航还能不能恢复”。
- 本轮子任务：
  - 只读看 `git log / git diff / 当前 dirty`；
  - 不进真实施工，不碰导航线程现场；
  - 重点审：
    1. 最近高风险提交窗口
    2. 当前导航线程真正改动域
    3. `NPC roam` 是否已被其它 owner 语义冻结
- 本轮稳定结论：
  1. 当前导航线程不只是改 `NavigationTraversalCore`，同时还在改：
     - `NPCAutoRoamController`
     - `NPCMotionController`
     - `NavGrid2D`
     - `TraversalBlockManager2D`
     - `PlayerAutoNavigator`
  2. 这说明当前不是单纯“底层导航核坏了”，而是导航 / NPC roam / 玩家入口一起在动。
  3. `NPCAutoRoamController` 当前已经带显式 `resident scripted control` 栈：
     - `AcquireResidentScriptedControl`
     - `ReleaseResidentScriptedControl`
     - `HaltResidentScriptedMovement`
     - `ShouldSuspendResidentRuntime`
     - 这层如果 release / resume 语义不稳，就会直接把 NPC 卡成“不卡但不走”。
  4. 历史版本不能直接整包回滚：
     - `17708cba` 把 player/NPC traversal 合并；
     - `4c736a7a` 又在上面叠了 runtime harden；
     - 当前 Day1 resident/director 还在消费新的控制语义；
     - 所以全回滚会把现在整个 shared contract 一起扯坏。
- 当前恢复点：
  - 如果继续分析，最值钱的是继续只读审 `resident scripted control` 的 owner/release 调用链；
  - 如果以后再允许我真实施工，我才会去做最窄证据钉死或最小恢复方案，不会越过导航线程去抢改。

## 2026-04-10｜补记：导航回执当前只站住“显示层增强”，没站住“现实层已解”
- 用户关键纠正：
  - `现实不是显示`
- 本轮更明确的线程判断：
  1. 导航这次对 `NPCMotionController` 的处理，最多只说明：
     - 日志更细
     - burst 式翻脸更容易抓
     - 朝向对瞬时噪声不再那么敏感
  2. 这不等于它已经回答：
     - 谁让 NPC 进入“不走/冻结”
     - 谁在拿住 roam 控制权不放
     - 为什么历史“会走但卡”退化成现在“不卡但傻”
  3. 当前我对“现实层”的最稳判断仍是：
     - `resident scripted control` 冻结/释放语义
     - 多 owner `StartRoam / StopRoam / SetFacingDirection`
     - stop-loss / harden 提前把坏态压成 freeze

## 2026-04-10｜已给导航线程起单刀 prompt：只追现实层冻结链
- 当前主线目标：
  - 用户认可应给导航线程发新 prompt，但要求这次不要再围绕显示层兜圈，而是直接把现实层问题钉死。
- 本轮子任务：
  - 进入真实施工前已跑 `Begin-Slice`；
  - 只新建一份 prompt 文件，不做代码修改，不做 sync；
  - 起刀后已跑 `Park-Slice` 收回现场。
- 本轮完成：
  - 已新建 prompt：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-10_导航线程_现实层冻结链与resident-scripted-control追责prompt_65.md`
  - prompt 核心裁定：
    1. 不再把 `NPCMotionController` 显示层增强当主刀；
    2. 只追 `NPCAutoRoamController` 的 `resident scripted control` acquire / halt / release / resume 现实链；
    3. 如果第一真问题落在 Day1 caller，就报 exact caller chain，不越权代修。
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑（本轮未进入 sync）`
  - `Park-Slice=已跑`
  - 当前 `PARKED`
- 当前恢复点：
  - 用户现在可以直接转发这份 prompt 给导航线程；
  - 我这轮不再继续扩写，等待导航线程按新刀口回复。

## 2026-04-11｜只读考古补记：4/3 可用版是“用户认可窗口”，真实代码基线应拆成 handoff truth / 实际 SHA / 后续退化 三层
- 当前主线目标：
  - 用户追问：之前让导航“彻底提交”的那个可用版本到底在哪里、有没有 memory 和真实提交记录，以及之前和现在到底差在哪，方便后续更快回退和修复。
- 本轮子任务：
  - 只读回看导航线程 handoff 文档、memory 和 git 历史；
  - 不进真实施工，不跑 `Begin-Slice`。
- 本轮稳定结论：
  1. 4/3 的“可用版本”是存在的，但在导航线程里首先被记录成：
     - `用户已认可当前导航版本`
     - `settled 的是 handoff，不是 sync`
     - `v2-handoff-complete-but-sync-still-blocked`
     所以它不是“4/3 当天新增了一个独立 runtime sync SHA”，而是“用户真实体验认可窗口 + 之前已有提交组成的代码树”。
  2. 真正构成那版代码底子的关键提交是：
     - `acfc7f27`：`NavigationLiveValidationRunner.cs` + `NavigationLocalAvoidanceSolver.cs`
     - `6a6db43e`：`PlayerAutoNavigator.cs`
     - `cc0bc7f8`：早期 `NPCAutoRoamController.cs` 小改
  3. 如果要找一个最方便直接 `checkout/diff` 的整仓基线，当前最实用的是：
     - `343c3910`
     - 它仍处在 `7e06c2e6` 之前，属于 accepted-window 后、NPC/static-audit 大扩张前的快照。
  4. 当前坏相真正的分水岭是：
     - `7e06c2e6`
     - 然后继续叠加：
       - `7cd57279`
       - `263f4ed0`
       - `bf386811`
       - `c29a80a2`
       - `592705f8`
       - `767d389b`
       - `aeaea9a0`
       - `17708cba`
       - `4c736a7a`
  5. “之前为什么可以”：
     - 被用户认可的核心是玩家导航入口体验；
     - 那时 NPC free-roam/shared traversal/resident scripted control 还没被后面这些大改深度缠到一起。
  6. “现在为什么不可以”：
     - 4/4 之后大量共享底层和 NPC roam 改动叠加；
     - 当前 NPC 问题不是单一导航核坏了，而是：
       - shared traversal core
       - NPC 自己的 stuck/recovery/hard-stop
       - resident scripted control / Day1 caller
       三层互相影响。
- 当前恢复点：
  - 如果后续继续只读支持用户调度，我现在最该拿来做回退/修复对照的双基线是：
    - `343c3910`：accepted-window 单一整仓快照
    - `7e06c2e6`：大规模共享/NPC 扩张起点
  - 如果后续允许我继续分析，我下一步会优先只做：
    - `343c3910..HEAD` 在 `NPCAutoRoamController / NavGrid2D / PlayerMovement / NPCMotionController / NavigationTraversalCore / TraversalBlockManager2D` 的精确责任差异表。

## 2026-04-13｜真实归仓补记：NpcCharacterRegistry 已作为 NPC 当前唯一稳定小包提交到 main
- 当前主线目标：
  - 用户要求我按 NPC 自己的历史边界，把现在真能合法提交的内容先提交掉，不继续扩功能。
- 本轮子任务：
  - 先对白名单最小候选做真实 `preflight` 和最小 CLI 验证；
  - 如果过线，就执行 `Ready-To-Sync -> sync`；
  - 然后把这次归仓结果结算进 child/parent/thread memory。
- 本轮稳定结论：
  1. 旧 docs/thread memory 层本身没有新的可提交增量；当前真能独立提的一刀，是 `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs(.meta)`。
  2. 这包已真实通过：
     - `preflight=True`
     - `validate_script assessment=no_red`
     - `owned_errors=0`
     - `external_errors=0`
     - `unity_red_check=pass`
  3. 这包已真实提交并推送：
     - SHA：`66dadf93cdd9e3b29d67162b192804daec9757ac`
     - message：`2026.04.13_NPC_01`
  4. 当前仍不能被我一起提交的大包，主要还是卡在：
     - mixed roots
     - same-root remaining dirty
     - shared / foreign blocker
     而不是“我没去提”。
- thread-state：
  - `Begin-Slice=已跑`
    - `npc-own-clean-sync`
    - `npc-character-registry-sync`
    - `npc-registry-sync-memory-settlement`
  - `Ready-To-Sync=已跑`
    - 用于 `NpcCharacterRegistry.cs(.meta)` 真同步
  - `Park-Slice=已跑`
    - `npc-character-registry-sync-complete`
- 当前恢复点：
  - resident / 关系页 / 头像真源线里，第一个已经回到 `main` 的稳定代码入口，现在就是 `NpcCharacterRegistry`；
  - 如果用户继续要求“把能提的都提掉”，下一步不该再盲试大包，而是继续只找这种能被证明 `preflight=True` 的最小白名单切片。

## 2026-04-14｜只读补记：当前 build fail 的第一真 blocker 不在 NPC own，NPC 线只命中 warning
- 当前主线目标：
  - 用户贴出 Player build 日志，要求我只拆跟 `NPC` 线有关的问题，讲清“我的问题是什么”和怎么修。
- 本轮子任务：
  - 只读核对 `DayNightManager.cs`、`NPCMotionController.cs` 与同批 warning 文件；
  - 不进真实施工，不跑 `Begin-Slice`。
- 本轮稳定结论：
  1. 当前真正卡住打包的是：
     - `Assets/YYY_Scripts/Service/Rendering/DayNightManager.cs:208`
     - `Start()` 调用了 `EditorRefreshNow()`；
     - 但 `EditorRefreshNow()` 定义在 `#if UNITY_EDITOR` 内；
     - 这是 rendering/day-night 线的 fatal compile error，不是 `NPC` 线当前第一 blocker。
  2. `NPC` 线在这份日志里真正相关的是：
     - `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
     - 4 条 `CS0414` warning
     - 本质是 editor-only facing mismatch diagnostics 字段在 Player build 里只写不读。
  3. 这些 `NPCMotionController` warning 会脏 build 日志，但不会单独导致 Player build fail。
- 当前恢复点：
  - 如果后续继续修包：
    1. 先清 `DayNightManager` 的 editor-only 调用泄漏；
    2. 再收 `NPCMotionController` 这批 player-warning；
  - 不要把当前 packaging fail 误判成“NPC 导航/漫游线又把打包打炸了”。

## 2026-04-14｜真实施工补记：已按最小风险修掉 `NPCMotionController` 自己的 4 条 warning
- 当前主线目标：
  - 用户要求我先修属于 `NPC` 自己的 warning，而且必须是最小、安全、不改原有高危逻辑的补丁。
- 本轮子任务：
  - 真实施工前已跑 `Begin-Slice`；
  - 只改 `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`；
  - 只收 `CS0414` 那 4 个 editor-diagnostic 字段 warning。
- 本轮完成：
  1. 已把以下字段收进 `#if UNITY_EDITOR`：
     - `_facingMismatchFrameCount`
     - `_facingMismatchBurstCount`
     - `_lastFacingMismatchLogTime`
     - `_lastFacingMismatchSeenTime`
  2. 已把 `ResetFacingMismatchDiagnostics()` 改成：
     - editor 下正常清空
     - player 下空操作
  3. 没改：
     - move / stop / roam / facing 判定
     - `NPCAutoRoamController`
     - 任何 shared render/day-night 逻辑
- 验证：
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`：通过
  - `validate_script NPCMotionController.cs`：
    - `owned_errors=0`
    - 当前 assessment=`external_red`
    - 外部 blocker 来自 `DayNightOverlay.cs`
    - `manage_script validate` 只剩 2 条旧式泛 warning，不再是这 4 条字段 warning
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑（本轮未进入 sync）`
  - `Park-Slice=已跑`
  - 当前 `PARKED`
- 当前恢复点：
  - `NPC` 自己这 4 条 warning 现在已经按最小补丁收掉；
  - 继续修包时，该转去处理外部 render/day-night 红面，不该再回头把这条 NPC warning 当主阻塞。

## 2026-04-14｜warning 补丁尾验完成
- 本轮补记：
  - 已补跑 `check-skill-trigger-log-health.ps1`，结果 `Health: ok`、`Canonical-Duplicate-Groups: 0`；
  - 已再次跑 `validate_script Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`。
- 最新验证结论：
  1. `NPCMotionController.cs` 当前 `owned_errors=0`、`current_warnings=0`；
  2. CLI assessment 这次是 `unity_validation_pending`，原因不是脚本自身出错，而是 Unity 当前 `stale_status`，不构成新的 own red；
  3. `manage_script validate` 仅剩 2 条旧式泛提示：
     - `Consider using FixedUpdate() for Rigidbody operations`
     - `String concatenation in Update() can cause garbage collection issues`
     这两条不是本次用户要求先清掉的 `CS0414` 字段 warning。
- 当前恢复点：
  - 这刀“只修我自己的 4 条 warning”已经完成并补齐审计；
  - 如果后续继续追整包 warning / build blocker，优先级不该再落回这 4 条，而该转去外部 render/day-night 面。

## 2026-04-15｜只读剖腹：NPC 本体结构、当前病灶与对外边界
- 当前主线目标：
  - 用户要求我把 `NPC` 线程自身剖开讲清楚，给 `Day1` 和 `导航` 一份能直接使用的本体参照，不继续停留在“谁像在抢锅”的层面。
- 本轮子任务：
  - 只读核对 `NpcCharacterRegistry / NPCDialogueContentProfile / NPCRoamProfile / NPCMotionController / NPCAutoRoamController / NPCInformalChatInteractable / PlayerNpcChatSessionService / NpcResidentRuntimeContract / SpringDay1NpcCrowdManifest / SpringDay1NpcCrowdDirector / DialogueUI / PackageNpcRelationshipPanel / PersistentPlayerSceneBridge / StoryProgressPersistenceService`；
  - 不进真实施工，不跑 `Begin-Slice`。
- 本轮稳定结论：
  1. `NPC` 自己现在的核心器官可拆成 5 层：
     - 身份主表层：`NpcCharacterRegistry`
     - 内容语义层：`NPCDialogueContentProfile` + `NPCRoamProfile`
     - 运动/漫游层：`NPCMotionController` + `NPCAutoRoamController`
     - 会话/气泡层：`NPCInformalChatInteractable` + `PlayerNpcChatSessionService` + `NPCBubblePresenter` + `PlayerThoughtBubblePresenter`
     - resident/runtime bridge 层：`NpcResidentRuntimeContract` + `NpcResidentRuntimeSnapshot`
  2. 外部消费口已经很明确：
     - 正式对白头像：`DialogueUI`
     - 关系页：`PackageNpcRelationshipPanel`
     - 关系持久化：`StoryProgressPersistenceService`
     - 切场 native resident snapshot：`PersistentPlayerSceneBridge`
     - Day1 resident runtime 消费：`SpringDay1NpcCrowdDirector`
  3. 当前最健康的是身份/内容/头像/关系链；
     - 当前最不健康的是 locomotion owner 与 resident release/return-home 合同。
  4. 当前最大问题不是“缺功能”，而是“同一个 NPC 没有单一 locomotion owner”：
     - `Day1 crowd` 仍在深碰 resident release/home-return/roam restart
     - `NPCAutoRoamController` 同时背着 free-roam、scripted control、debug move、shared avoidance
     - `NPCMotionController` 又在最后一层兜动画与朝向
  5. 因此后续不该再把 `NPC` 整条线当成一坨一起修；
     - 必须按器官和 owner 拆。
- 当前恢复点：
  - 如果下轮继续给 `Day1` / `导航` 对齐，不该再从“主诉现象”讲起；
  - 直接用这张拆层图去落：
    - `NPC own`
    - `Day1 consume-only`
    - `导航 own`
    - `shared contract`
    - `禁止再跨写`

## 2026-04-15｜只读大整理：NPC locomotion 对外合同草案
- 当前主线目标：
  - 用户要求我只收 `NPC locomotion` 的对外合同草案，不回吞 `Day1` phase 语义，也不回吞导航执行策略。
- 本轮子任务：
  - 只读核对 `NPCAutoRoamController` 的 public 口、`NPCMotionController` 的 public 写口、`SpringDay1NpcCrowdDirector` 对这些口的当前调用方式、以及 `SpringDay1Director` 的夜间 story actor 调度；
  - 不进真实施工，不跑 `Begin-Slice`。
- 本轮稳定结论：
  1. `NPCAutoRoamController` 当前对外高危口太多，且粒度过低；
     - 外部线程现在能直接拼装内部 lifecycle
     - 这就是 contract 面失控的根因
  2. 最危险的一组口是：
     - `StartRoam / RestartRoamFromCurrentContext / StopRoam`
     - `AcquireResidentScriptedControl / DriveResidentScriptedMoveTo / PauseResidentScriptedMovement / ResumeResidentScriptedMovement / ReleaseResidentScriptedControl / ClearResidentScriptedControl`
     - `ApplyProfile / SetHomeAnchor / RefreshRoamCenterFromCurrentContext`
     - `DebugMoveTo`
     - `NPCMotionController.SetFacingDirection / SetExternalVelocity / SetExternalFacingDirection`
  3. 这些口里，只有少数应继续给外部保留“语义级 facade”；
     - 大多数都该降成 `internal-only / runtime-only / debug-only`
  4. 当前 `Day1 crowd` 已经在直接组合这些低级口；
     - 所以后续最该收的不是“调参数”，而是 public 面缩口
  5. 夜间合同目前也还是双头：
     - resident 夜间时段写在 `SpringDay1NpcCrowdDirector`
     - story actor 夜间时段写在 `SpringDay1Director`
     - 这两套都不该继续各自私写，应统一消费 locomotion contract
- 当前恢复点：
  - 如果继续往下做，下一刀应直接把这份草案压成一张面向 `Day1 / 导航` 的合法接口表，而不是继续做症状层争论。

## 2026-04-15｜固定格式回卡补记：按 prompt 01 输出合同面回执
- 当前主线目标：
  - 用户要求我严格按 prompt 01 的固定格式输出 `NPC locomotion` 合同面回执。
- 本轮子任务：
  - 复核 prompt 里的硬要求；
  - 沿用上一轮只读结论；
  - 以 `A1 / A2 / B` 与四张表输出，不新增代码判断。
- 本轮稳定结论：
  1. 当前回卡的核心内容没有变：
     - 高危 public 写口清单
     - 最小 facade
     - `internal-only / runtime-only / debug-only / facade 内部原语`
     - `Day1 / Navigation` 各自合法触碰范围
  2. 本轮仍然是只读；
     - 线程状态继续 `PARKED`
     - 不存在新的代码/scene 改动
- 当前恢复点：
  - 如果下一刀继续深入，就该从“回卡”进入“执行表”，把 public 面迁移方案真正落细。

## 2026-04-15｜弱引导同步：NPC 只回答自己 own 的 facade 判断
- 当前主线目标：
  - 用户要求我读取 `Day1-V3` 的弱引导文件后，只回答 `NPC own` 的部分，不回吞 `Day1 phase`，也不回吞导航策略。
- 本轮子任务：
  - 读取弱引导总表与给 NPC 的协作边界文件；
  - 只输出 facade 主导权、优先 internal-only 的口、Day1 最先停碰的低级 API、最小 facade 面、唯一下一刀。
- 本轮稳定结论：
  1. facade 仍应由 `NPC` 主导；
  2. 最先要 internal-only 的，是最容易让外部拼状态机的那批低级 public 写口；
  3. `Day1` 最先该停碰的，就是 `SetHomeAnchor / ApplyProfile / RefreshRoamCenter / StopRoam / RestartRoam / DebugMoveTo` 这组；
  4. 我自己的唯一下一刀，不是直接开改，而是先把 `旧 public -> 新 facade -> 调用迁移` 压成执行表。
- 当前恢复点：
  - 后续如果继续推进代码，就按这份判断进入 facade 迁移表，而不是重新争论职责边界。

## 2026-04-16｜只读审计：Town 自由时段群体 roam 性能热点
- 当前主线目标：
  - 用户要求只读审计 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`，判断 Town 自由时段大量 NPC 同时 roam 时最可能的热点链和最值得动的 1-3 处方法，不做实现。
- 本轮子任务或阻塞：
  - 子任务是性能热点定位；
  - 服务于 NPC roam 卡顿主线；
  - 本轮没有进入真实施工，因此不跑 `thread-state`，保持 `PARKED` 语义。
- 已完成事项：
  1. 只读展开了 `Update / FixedUpdate / TickMoving` 主循环。
  2. 只读核对了：
     - `TryHandleSharedAvoidance`
     - `AdjustDirectionByStaticColliders`
     - `UpdateTraversalSoftPassStateOncePerFrame`
     - `NavigationAgentRegistry.GetNearbySnapshots`
     - ambient bubble / `NPCBubblePresenter.LateUpdate`
  3. 已把热点优先级压缩成明确排序：
     - 第一：共享避让链，核心瓶颈在 registry 每次查询都重建 snapshots，再被 solver 二次扫描。
     - 第二：静态碰撞 steering 链，核心瓶颈在每次重决策最多 3 次 `OverlapCircle` + per-hit `ClosestPoint`。
     - 第三：视现场决定是 `UpdateTraversalSoftPassStateOncePerFrame` 的 idle 轮询，还是 `NPCBubblePresenter.LateUpdate` 的 hidden 背景更新。
- 关键决策：
  1. 不建议第一刀去大改 `TickMoving` 总体结构，因为它已经有同帧/跨帧 reuse 窗口，先动 registry 与静态 probe 更低风险。
  2. `UpdateTraversalSoftPassStateOncePerFrame` 已有节流，不是当前第一嫌疑。
  3. ambient bubble 真正值得警惕的不是聊天配对本身，而是气泡 UI 创建后的持续 `LateUpdate`。
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationAgentRegistry.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubblePresenter.cs`
- 验证结果：
  - 只做静态审计，未跑 Unity / MCP / profiler / tests；
  - 当前结论属于“代码结构推断成立”，不是 runtime 实测。
- 修复后恢复点：
  - 如果后续继续 NPC 性能线，直接从 `GetNearbySnapshots` 的 snapshot cache 或 `AdjustDirectionByStaticColliders` 的渐进 probe 开始；
  - 用户当前已经可以据此决定下一刀，不必再先做宽泛扫读。

## 2026-04-18｜真实施工：`NPC_Hand/202.png` 已落成对白胸像候选
- 当前主线目标：
  - resident / 关系页 / 对白头像真源线继续推进；
  - 本轮子任务是把 `Assets/Sprites/NPC/202.png` 对应角色补成一张真正可用于对白框的右向胸像，而不是继续沿用“裁切行走帧”的失败路径。
- 本轮子任务：
  - 命中 `2.1.0_resident与关系页收口`；
  - 真实施工前已跑 `Begin-Slice`；
  - 只改：
    - `Assets/Sprites/NPC_Hand/202.png`
    - `Assets/Sprites/NPC_Hand/202.png.meta`
  - 本轮不做 Unity live import / DialogueUI runtime 验证，因此不进 `Ready-To-Sync`，改走 `Park-Slice` 收尾。
- 本轮完成：
  1. 已新建 `202` 的手绘对白胸像：
     - `Assets/Sprites/NPC_Hand/202.png`
  2. 已补 Sprite 导入 `.meta`：
     - `Assets/Sprites/NPC_Hand/202.png.meta`
  3. 当前成品静态上已满足：
     - 单人
     - 右向 3/4
     - bust-up
     - 粉发 / 白头带 / 黑白女仆服
     - 纯白实底
- 已验证事实：
  - `202.png` 尺寸：
    - `512x512`
  - alpha：
    - `255~255`
  - 四角像素：
    - 全部 `(255,255,255,255)`
  - Git 当前只把这两条路径记为新增：
    - `Assets/Sprites/NPC_Hand/202.png`
    - `Assets/Sprites/NPC_Hand/202.png.meta`
- 当前没完成的事：
  - 还没进 Unity 触发 import
  - 还没确认 `NpcCharacterRegistryHandPortraitAutoSync` 是否吃到 `202`
  - 还没在 `DialogueUI` / 关系页里看实际显示
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑`
  - `Park-Slice=已跑`
  - 当前 `PARKED`
- 当前恢复点：
  - 如果下一轮继续，这条线最值得做的不是再重画，而是先补 Unity 接链验证，再决定是否需要第二版更强 Ram 神态或情绪版。

## 2026-04-18｜用户退回第一版后，`202` 已按透明底与现有资产风格重做
- 当前主线目标：
  - 主线仍是 resident / 关系页 / 对白头像真源线；
  - 本轮子任务是修正上一版 `202` 胸像被用户指出的三类问题：
    1. 没学习 `NPC_Hand` 现有内容
    2. 背景错误做成实色
    3. 图片里有坏点坏条纹
- 本轮实际做成了什么：
  1. 重新读取并对比了：
     - `Assets/Sprites/NPC_Hand/001.png`
     - `Assets/Sprites/NPC_Hand/104.png`
     - `Assets/Sprites/NPC_Hand/201.png`
     - 当前错误版 `Assets/Sprites/NPC_Hand/202.png`
  2. 重新确认现有资产规律：
     - 透明底
     - 现成完整胸像
     - `201` 是最接近的 maid 风格母版
  3. 已据此重做并覆盖：
     - `Assets/Sprites/NPC_Hand/202.png`
     - `Assets/Sprites/NPC_Hand/202.png.meta`
  4. 当前新成品静态自验为：
     - `536x700`
     - alpha `0~255`
     - 四角透明
- 当前还没做成什么：
  - 还没进 Unity 验 import
  - 还没让 `NpcCharacterRegistryHandPortraitAutoSync` 吃到 `202`
  - 还没在 `DialogueUI` / 关系页里看 runtime 显示
- 当前阶段：
  - 资产重做已完成；
  - Unity 接链验证尚未做。
- 为什么这轮这样判断：
  - 用户批评的不是“再调一点颜色”，而是方向就错了；
  - 所以这轮正确动作不是继续硬修上一版，而是先回到 `NPC_Hand` 现有真源学习，再做低风险重构。
- 自评：
  - 这版比上一版扎实得多，至少已经回到项目现有风格和透明底规范里；
  - 我最不满意的是还没补 Unity 接链验证，所以这轮只能报“资产已修正”，不能报“已经正式可用”。
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑`
  - `Park-Slice=已跑`
  - 当前 `PARKED`
- 当前恢复点：
  - 如果继续，就直接补 Unity import / registry / DialogueUI 验证；
  - 不再先开第三轮重画。

## 2026-04-23｜shared-root 历史小批次上传：`104` 头像删除不是独立批次
- 当前主线目标：
  - 用户要求这轮不要继续修 resident / roam / runtime contract；
  - 只按历史小批次口径，单独尝试 `104` 头像删除这一个最小上传切片。
- 本轮子任务：
  - 真实施工前已重新 `Begin-Slice`，切片只登记：
    - `Assets/Sprites/NPC_Hand/104.png`
    - `Assets/Sprites/NPC_Hand/104.png.meta`
  - 本轮只核引用一致性，不扩到 editor tool、runtime controller 或第二批次。
- 本轮已完成：
  1. 已确认当前工作树里 `104` 只表现为删除：
     - `D Assets/Sprites/NPC_Hand/104.png`
     - `D Assets/Sprites/NPC_Hand/104.png.meta`
  2. 已查到 `Assets/Resources/Story/NpcCharacterRegistry.asset` 的 `npcId: 104` 条目仍然保留旧头像引用：
     - `handPortrait: {fileID: 5953923354035733431, guid: c09c30ba65467fb4190d2679edee39a9, type: 3}`
  3. 这说明 `104` 删除当前不是“自然一致、可独立上传”的历史小批；
     - 一旦真提交删除，就会留下主表悬空引用。
- 当前稳定结论：
  - `104` 删除不是独立历史批次；
  - 当前引用没有自然一致；
  - 第一真实 blocker 就是：
    - `Assets/Resources/Story/NpcCharacterRegistry.asset` 里 `npcId: 104` 仍直接引用被删头像。
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑`
  - `Park-Slice=已跑`
  - 当前 `PARKED`
- 当前恢复点：
  - 这轮按规则已停车，不继续第二批；
  - 除 `104` 删除这组之外，其它 NPC 尾账先不动。

## 2026-04-24｜`104` 主表引用一致性已补平，但真实上传被 shared-root blocker 挡停
- 当前主线目标：
  - 用户要求这轮不要再继续问 `104` 删除能不能单独上传；
  - 只做一刀：先把 `npcId:104` 的主表 `handPortrait` 引用一致性收平，再把“主表 + 删除态图片”当成唯一小批做一次真实上传尝试。
- 本轮子任务：
  - 已重新 `Begin-Slice`，切片只登记：
    - `Assets/Resources/Story/NpcCharacterRegistry.asset`
    - `Assets/Sprites/NPC_Hand/104.png`
    - `Assets/Sprites/NPC_Hand/104.png.meta`
  - 未越权扩到其它 NPC 条目、editor tool 或 runtime controller。
- 本轮完成：
  1. 已把 `NpcCharacterRegistry.asset` 中 `npcId:104` 的旧头像引用清掉：
     - 从 `{fileID: 5953923354035733431, guid: c09c30ba65467fb4190d2679edee39a9, type: 3}`
     - 改成 `{fileID: 0}`
  2. 当前主表与 `104.png` 删除态已经就内容语义而言一致：
     - 主表不再引用已删图
     - `104.png` 与 `.meta` 仍保持删除态
  3. 已按规则跑了真实 `Ready-To-Sync` 尝试。
- 新的第一真实 blocker：
  - 不是 `104` 这一组自身还不一致；
  - 而是 `Ready-To-Sync` 在 shared-root 白名单逻辑里把 root 折叠成 `Assets/Resources/Story`，随后被：
    - `Assets/Resources/Story/SpringDay1Workbench/Recipe_9102_Pickaxe_0.asset`
    这个他线脏改挡住。
- 当前稳定结论：
  - `npcId:104` 的 `handPortrait` 一致性已补平；
  - 这组没有提交成功；
  - 失败原因已经变成新的 exact blocker，而不是旧的悬空引用问题。
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=已跑但被 blocker 卡住`
  - `Park-Slice=已跑`
  - 当前 `PARKED`
- 当前恢复点：
  - 这轮已按规则停车，不继续第二批；
  - 其它 NPC 尾账仍未碰。

## 2026-04-24｜`prompt_04` 复核：`Assets/Resources/Story` blocker 证据已足够，不重跑上传
- 当前主线目标：
  - 用户要求不要再改 `npcId:104` 内容本身；
  - 只处理 `Assets/Resources/Story` 目录同步 blocker，并按 exact 结论收口。
- 本轮复核：
  1. 已再次确认 `NpcCharacterRegistry.asset` 里 `npcId:104` 仍为 `handPortrait: {fileID: 0}`。
  2. 已再次确认 `104.png` 与 `104.png.meta` 当前都不存在，删除态保持成立。
  3. 已再次确认当前 shared-root 第一真实 foreign blocker 仍是：
     - `Assets/Resources/Story/SpringDay1Workbench/Recipe_9102_Pickaxe_0.asset`
  4. 已确认现有证据足够，不需要重跑同一上传尝试。
- 当前稳定结论：
  - `104` 内容一致性没有回退；
  - 当前卡点已经纯粹是 `Assets/Resources/Story` 目录级 foreign dirty；
  - 下一步应优先转给对应 foreign owner / 治理位，而不是继续由 `NPC` 重复做同一次上传。
- thread-state：
  - 本轮只读复核
  - 当前维持 `PARKED`
