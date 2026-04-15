# 导航V3 线程记忆

## 2026-04-15｜导航现状只读复核

### 用户目标
- 用户要求我先不要直接干活，先把导航现状彻底查清。
- 本轮子任务是：结合导航总交接、导航检查工作区、用户给别的线程的 prompt/回复、当前代码和 live artifact，重新判断导航现在到底卡在哪。
- 这轮服务的主线仍是导航修复，不是另起新题。

### 已完成事项
1. 做了 Sunset 前置核查；本轮是只读分析，已明确暂不跑 `Begin-Slice`。
2. 确认当前 `导航V3` 线程目录和 `导航V3` 工作区目录都还是空壳，真正持续更新的 live 材料仍在：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\`
3. 回读了：
   - 导航总交接文档
   - `导航检查/memory.md`
   - `03_NPC自漫游与峰值止血/memory.md`
   - `prompt_68`
4. 对位了当前核心代码：
   - `NPCAutoRoamController.cs`
   - `NPCMotionController.cs`
   - `NavigationTraversalCore.cs`
   - `NavigationLocalAvoidanceSolver.cs`
   - `NavigationAgentRegistry.cs`
   - `SpringDay1NpcCrowdDirector.cs`
5. 补了 fresh CLI / artifact 证据：
   - `validate_script` => `assessment=external_red / owned_errors=0 / external_errors=1`
   - 当前外部阻断是 `DialogueChinese V2 SDF.asset` importer inconsistent，不是导航 own compile red
   - `npc-roam-spike-stopgap-probe.json` 最新仍显示：
     - `avgFrameMs=31.46`
     - `maxFrameMs=102.88`
     - `Stuck=21`
     - `SharedAvoidance=9`
     - `MoveCommandNoProgress=2`

### 关键决策
- 当前最该沿用的总判断是用户压出来的三主根：
  1. `坏点`
  2. `静态导航垃圾`
  3. `不会自救`
- 现在不能再说“只剩 Day1”或“只剩导航参数”。
- 正确边界是：
  - `Day1` 仍有越权问题
  - 但导航 own 也确实没过线
- 如果后续继续施工，优先顺序固定为：
  1. `destination hard contract`
  2. `path-level body clearance`
  3. `完整脱困闭环`

### 当前判断
- 当前系统已经不是“完全不会走”，但玩家可感知体验仍明显未过线。
- 目标点 acceptance 比以前严格了一些，step constraint 也开始吃 body-clearance margin，但系统整体仍保留“先抽到坏点 / 先让路径成立，再靠后处理纠偏”的旧 DNA。
- blocked/stuck 现在更多是“减少炸锅”，不是“稳定脱困”。

### 涉及路径
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V3\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-15_导航线程总交接文档_own问题_边界语义_历史修改_后续接手.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\03_NPC自漫游与峰值止血\memory.md`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCMotionController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationTraversalCore.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationAgentRegistry.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1NpcCrowdDirector.cs`
- `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\npc-roam-spike-stopgap-probe.json`

### 验证结果
- `结构 / checkpoint`：成立
- `targeted probe / 局部验证`：成立
- `真实入口体验`：未过线

### 遗留问题 / 下一步
- 下一步如果仍停在分析阶段，就继续补硬证据，重点区分：
  1. 坏点 acceptance 问题
  2. 路径成立层 clearance 问题
  3. 自救闭环缺口
- 下一步如果进入真实施工，必须先按上面的三主根排序开刀，再决定是否跑 `Begin-Slice`。

## 2026-04-15｜Day1-V3 弱引导同步后补记

### 用户目标
- 用户要求我先完整读完 `Day1-V3` 给导航的弱引导 prompt 和两份正文。
- 本轮子任务不是替 Day1 设计 phase，而是只从导航 own 的位置回答：执行层现在分裂成几套 contract、哪些该合并、会不会伤到 Day1 staged contract、唯一下一刀是什么。

### 已完成事项
1. 已完整阅读：
   - `2026-04-15_给导航_Day1-V3语义同步与执行边界弱引导prompt_01.md`
   - `2026-04-15_Day1-V3_Day1现存问题总览与整体施工总表.md`
   - `2026-04-15_Day1-V3_给导航_语义同步与执行边界contract.md`
2. 已继续对位导航现码，确认当前导航 own 内部至少仍分裂为：
   - `autonomous roam`
   - `resident scripted move`
   - `formal navigation debug/home-return`
   - `plain debug/manual fallback`
3. 已额外确认：`Day1 staged playback` 还有一条完全绕开导航的 authored 直推链，当前在 `SpringDay1DirectorStaging.cs` 里直接 `transform.position += step`。

### 关键决策
- 我认同“只要发生真实位移，原则上应优先走统一导航执行合同”这条边界。
- 但我不认同现在就把 `opening / dinner staged playback` 整体吞进当前 free-roam contract。
- 当前更值钱的统一方向，是先把导航自己内部 3 套 point-to-point 执行口径统一：
  1. `resident scripted move`
  2. `formal navigation debug/home-return`
  3. `plain debug/manual fallback`
- `autonomous roam` 暂时继续作为“目标来源合同”，不先跟 staged playback 顶层语义强并。

### 当前方向调整
- 上一轮排序是：
  1. `destination hard contract`
  2. `path-level body clearance`
  3. `完整脱困闭环`
- 这轮调整后更准确的排序是：
  1. `统一 point-to-point navigation travel contract`
  2. `destination hard contract`
  3. `path-level body clearance`
  4. `完整脱困闭环`

### 当前判断
- 如果不先统一导航 own 内部 point-to-point travel contract，后面再做坏点合同和 clearance，仍会被 `debug / fallback / scripted move` 旁路继续污染。
- 当前 staged playback 仍应保留独立顶层合同，因为它还带有 authored cue、5 秒 timeout、超时 snap、dialogue freeze 这些 Day1 own 语义。

### 遗留问题 / 下一步
- 如果继续只读，下一步应继续把 point-to-point contract 的入口、退出、失败处理、和 NPC facade 消费面再压细。
- 如果进入真实施工，导航自己的唯一第一刀应先统一 point-to-point travel contract，而不是先回吞 Day1 staged playback。

## 2026-04-15｜导航V3 正式执行清单已立住

### 用户目标
- 用户明确要求我先给自己立一份详细的当前清单，把语义、方向，以及导航修复后续的全流程内容全部定义好。
- 这轮仍服务导航修复主线，不是进入真实施工。

### 已完成事项
1. 复核并确认当前正式执行表已落盘：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V3\2026-04-15_导航V3_导航修复当前执行方向与详细清单.md`
2. 确认这份执行表已经把下面几层写死：
   - `Day1 / 导航 / NPC body` 边界
   - 当前 movement contract 分裂图
   - 当前唯一施工顺序
   - 每个 package 的目标、范围、完成标准、停步条件
3. 确认这份表与 `Day1-V3` 边界一致：
   - 不回吞 `Day1 phase`
   - 不先吞 `opening / dinner staged playback`
   - 第一刀仍是 `统一 point-to-point navigation travel contract`

### 关键决策
- 从现在起，这份执行表就是 `导航V3` 的 live baseline。
- 后续导航继续推进，默认按这份表开包，不再回到“症状驱动散修”。
- 当前正式排序固定为：
  1. `统一 point-to-point navigation travel contract`
  2. `destination hard contract`
  3. `path-level body clearance`
  4. `完整脱困闭环`
  5. `NPC facade / Day1 call-site 收口`
  6. `live 终验`

### 当前判断
- 这轮做成的是“正式执行入口已立住”，不是“导航已修完”。
- 当前站住的是结构层和施工顺序，玩家体验仍未过线。
- 下一轮真正开工时，最该防的错法仍是：
  - 回吞 Day1 phase
  - 继续散修参数
  - 跳过 Package A 直接追症状

### 验证结果
- `结构 / checkpoint`：成立
- `targeted probe / 局部验证`：成立
- `真实入口体验`：未过线

### 状态与恢复点
- 当前仍是只读收口；未进入真实施工。
- thread-state 继续保持 `PARKED`。
- 如果下一轮继续只读，优先围绕执行表继续补 point-to-point contract 入口图、失败图和 facade 消费面。
- 如果下一轮进入真实施工，先 `Begin-Slice`，然后只开 `Package A｜统一 point-to-point navigation travel contract`。

## 2026-04-15｜Package A 首刀已真实开工并合法停车

### 用户目标
- 用户要求我不要再停在分析层，而是直接按执行表持续施工。
- 本轮子任务是：正式开启 `Package A｜统一 point-to-point navigation travel contract` 的第一段实现。

### 已完成事项
1. 已按 Sunset 规则真实进场并收口：
   - 开工前已跑 `Begin-Slice`
   - 收尾前已跑 `Park-Slice`
   - 当前 thread-state 已回到 `PARKED`
2. 已在 `NPCAutoRoamController.cs` 落下首批结构统一：
   - 新增显式 `PointToPointTravelContract`
   - `RequestStageTravel / RequestReturnHome / BeginAutonomousTravel / BeginReturnHome / DebugMoveTo` 已统一走 `BeginPathDirectedMove(...)`
   - `RequestStageTravel` => `ResidentScripted`
   - `RequestReturnHome / BeginReturnHome` => `FormalNavigation`
   - `BeginAutonomousTravel` => `AutonomousDirected`
   - `DebugMoveTo` => `PlainDebug`
3. 已把 recoverable 的 point-to-point contract 收进统一恢复分支：
   - `AutonomousDirected / ResidentScripted / FormalNavigation` 共用 stuck recover
   - 同上 3 类共用 blocked-advance recover
   - `PlainDebug` 继续保留 debug-only bypass 语义
4. 已补一层执行统一：
   - recoverable point-to-point contract 统一吃静态障碍 steering
   - `PlainDebug` 不吃
5. 已同步改测试：
   - `NavigationAvoidanceRulesTests.cs` 中旧的 `debugMoveUsesFormalNavigationContract` / 旧推断方法反射口已换到新枚举

### 关键决策
- 这轮做的是 `Package A` 第一段结构统一，不是去做坏点硬合同或 path clearance。
- 当前仍坚持：
  - 不回吞 Day1 phase
  - 不越包到 `Package B`
  - 不把 `PlainDebug` 再当正式 locomotion contract

### 验证结果
- `validate_script`：
  - `NPCAutoRoamController.cs` => `assessment=unity_validation_pending / owned_errors=0 / external_errors=0`
  - `NavigationAvoidanceRulesTests.cs` => `assessment=unity_validation_pending / owned_errors=0 / external_errors=0`
  - 当前没到 `no_red` 的主因仍是 Unity `stale_status`，不是这轮 own compile red
- targeted EditMode tests：`2/2 passed`
  - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldTreatStoryOwnedFormalReturn_AsResidentScriptedMove`
  - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldKeepPlainDebugBypass_ButEnableRecoverablePointToPointStaticSteering`
- `git diff --check`（仅本轮文件）通过

### 当前判断
- `Package A` 已真正开工，且第一段结构统一已经落地。
- 但 Package A 还没收完，更别说整个导航修复过线。
- 当前最值钱的增量是：point-to-point contract 终于不再只靠 `debug/formal/story` 这套隐式布尔拼装状态来表达。

### 下一步恢复点
- 继续 Package A 时，优先清剩余仍靠 `debugMoveActive / IsResidentScriptedControlActive` 硬判断的旁路分支。
- 再确认 point-to-point contract 的终止 / abort / restart 语义是否还带旧 debug 味。
- 完成这些之后，才考虑 Package A 内部的 facade / call-site 收口；仍不越到 `Package B`。

## 2026-04-15｜Package A 第二段已完成并重新停车

### 用户目标
- 用户让我先吸收 NPC 回执，然后自行调整导航 own 的边界口径，再继续往后做。

### 已完成事项
1. 我已核对 NPC 回执与现码，确认：
   - facade 基本已落地；
   - resident scripted 低级写口已收进 private；
   - 但 Day1 仍残留少量 `StartRoam()` 直调点，因此我把判断调整成：
     - `NPC facade 基本成立`
     - `Day1 call-site 仍有少量尾项`
2. 我继续在 `NPCAutoRoamController.cs` 里推进 `Package A`：
   - 新增 `UsesAutonomousRoamExecutionContract()` helper
   - 把一批残余的旧布尔分流收成显式 contract helper
   - 继续把 autonomous roam 与 point-to-point 的内部执行分流拉开
3. 我又统一了一层执行质量：
   - `AutonomousDirected / ResidentScripted / FormalNavigation` 现在共同吃 released body clearance 执行合同
   - `PlainDebug` 继续保持 debug-only 例外口
4. 我补了一条新的测试：
   - `NPCAutoRoamController_ShouldEnableReleasedBodyClearance_ForRecoverablePointToPointContracts`

### 关键决策
- NPC 回执没有要求我改主方向，只要求我把“导航 own 的对外口径”收窄。
- 我这轮没有回改 Day1 phase，也没有去碰 NPC facade 设计本身。
- 当前导航仍只做执行层质量统一。

### 验证结果
- `validate_script`
  - `NPCAutoRoamController.cs` => `assessment=unity_validation_pending / owned_errors=0 / external_errors=0`
  - `NavigationAvoidanceRulesTests.cs` => `assessment=unity_validation_pending / owned_errors=0 / external_errors=0`
- EditMode targeted tests：`3/3 passed`
  - `NPCAutoRoamController_ShouldTreatStoryOwnedFormalReturn_AsResidentScriptedMove`
  - `NPCAutoRoamController_ShouldKeepPlainDebugBypass_ButEnableRecoverablePointToPointStaticSteering`
  - `NPCAutoRoamController_ShouldEnableReleasedBodyClearance_ForRecoverablePointToPointContracts`
- `git diff --check`（仅本轮文件）通过

### 当前判断
- `Package A` 还没完成，但最粗的内部合同分流已经又收干净一层。
- 当前剩余更像：
  - 少量旁路残项
  - facade / call-site 尾项
  - 然后才是 `Package B`

### 状态与恢复点
- 这轮已再次 `Park-Slice`，当前 thread-state 是 `PARKED`。
- 下一轮继续时，先看：
  1. 是否还有值得继续在导航 own 内部清的残余旁路
  2. 如果没有，就进入 `Package A` 的 facade / call-site 收口判断

## 2026-04-15｜Package A 第三段已完成并重新停车

### 用户目标
- 用户继续要求我沿当前清单往下做，不要回到纯分析；因此我继续留在 `Package A`，专收导航 own 的验证/探针消费面。

### 当前主线
- `Package A｜统一 point-to-point navigation travel contract`
- 本轮子任务：
  - 把导航 own 的 validation / probe 工具从旧的 `DebugMoveTo / StartRoam / StopRoam` 改口到新 facade
- 这一步服务于主线的原因：
  - 如果导航自己的验证工具还在直调旧口，那么内部 contract 清得再干净，也会被自己的工具继续回流污染

### 已完成事项
1. 核清 live 状态时发现 `Show-Active-Ownership` 里 `导航V3` 实际仍是 `ACTIVE`；因此这轮没有重复开新 slice，而是沿当前 slice 继续施工，收尾再重新 `Park-Slice`。
2. 在 `Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs` 完成导航 own live validation 改口：
   - `DebugMoveTo` -> `BeginAutonomousTravel`
   - `StartRoam` -> `ResumeAutonomousRoam`
   - `StopRoam + parking/reset` -> `SnapToTarget`
3. 在 `Assets/Editor/NPC/CodexNpcTraversalAcceptanceProbeMenu.cs` 完成 acceptance probe 改口：
   - `StopRoam` -> `SnapToTarget`
   - `DebugMoveTo` -> `BeginAutonomousTravel`
4. 在 `Assets/YYY_Tests/Editor/NpcLocomotionFacadeSurfaceTests.cs` 补了 source test：
   - 钉死这两个导航 own 工具文件不再直调 `DebugMoveTo / StartRoam / StopRoam`
   - 同时修正项目根路径解析，避免测试环境目录变化导致误报

### 验证结果
- direct MCP `validate_script`
  - `NavigationLiveValidationRunner.cs` => `errors=0 / warnings=2`
  - `CodexNpcTraversalAcceptanceProbeMenu.cs` => `errors=0 / warnings=0`
  - `NpcLocomotionFacadeSurfaceTests.cs` => `errors=0 / warnings=0`
- EditMode targeted tests
  - `NpcLocomotionFacadeSurfaceTests` => `5/5 passed`
- CLI `errors`
  - `errors=0 / warnings=0`
  - 测试结果保存日志出现一条 `Exception` 型文本，但它不是项目红错
- `git diff --check`（仅本轮 3 个文件）通过
- 额外噪音
  - CLI `validate_script` 对其中 2 个目标再次撞到 `CodexCodeGuard returned no JSON` / `stale_status`
  - 所以这轮 compile-first 证据不是完美闭环，但没有 own compile red 迹象

### 关键判断
- 这轮最核心的推进不是“修了导航算法”，而是“把导航 own 自己的工具消费面从旧合同切走了”。
- 这使 `Package A` 从“内部 contract 清洗”继续推进到“consumer surface 收口”阶段。
- 这仍然不是 `Package A` 已完成，更不是导航体验已过线。

### 涉及文件
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLiveValidationRunner.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPC\CodexNpcTraversalAcceptanceProbeMenu.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NpcLocomotionFacadeSurfaceTests.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V3\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\memory.md`

### 状态与恢复点
- 这轮收尾已再次执行 `Park-Slice`，当前 thread-state 是 `PARKED`。
- 如果用户继续让我往下做，下一步优先：
  1. 再扫一遍导航 own 是否还有别的 validation/probe/tool call-site 在直调旧口
  2. 如果这条消费面已经足够干净，就判断 `Package A` 能不能进入收尾，而不是直接越到 `Package B`

## 2026-04-15｜已向用户解释为什么不是“直接一轮从 A 到 D”

### 用户目标
- 用户明确表示不想看中间产物，只想要最终导航；并追问为什么我还要分 `A~D` 讲阶段。

### 这轮稳定结论
1. 可以连续往下做，但不能先空口承诺“一轮必定从 A 到 D 全做完”。
2. 原因不是流程癖，而是 `A/B/C/D` 各自对应不同病层：
   - `A` = 执行合同统一
   - `B` = 坏点硬合同
   - `C` = 路径级 clearance
   - `D` = 脱困闭环
3. 它们是上下游关系，不是任意拆段：
   - `A` 不稳，后面的样本会继续被旧入口污染
   - `B` 不稳，后面会继续吃坏目标点
   - `C` 不稳，`D` 会一直替错误路径擦屁股
4. 所以后续正确口径是：
   - 对用户：默认连续施工，不交中间包装
   - 对内部：仍按 `A -> B -> C -> D` 判断是否真正站稳

### 下一步约束
- 如果用户继续让我真实施工，默认按“连续做，不频繁停；但内部逐层确认”执行。
- 只有当出现 blocker，或我还不能诚实 claim 最终导航已过线时，才需要停下来报实。

## 2026-04-15｜Town live 取证收口：当前不能诚实 claim“最终导航已过线”

### 用户目标
- 用户要求我继续推进导航 own，并希望我自己测试、自己判断，不要再只给空分析。

### 当前主线
- 继续导航 own 的 `Package A-D` 主线。
- 本轮子任务：
  - 用 Town fresh live 去验证我这轮已经落下的坏点拒收 / 静态绕行 / 自救链，到底有没有被真实 runtime 吃到

### 本轮实际做成了什么
1. 按 Sunset live 前置规则补读了：
   - `mcp-single-instance-occupancy.md`
   - `mcp-live-baseline.md`
   - `mcp-hot-zones.md`
2. 跑了 Town clean play 下的两条 managed roam probe：
   - `NpcRoamRecoverWindow`
   - `NpcRoamPersistentBlockInterrupt`
3. 两条都失败，但失败方式不是“detour/recovery 逻辑爆炸”，而是共同卡在：
   - `seedAccepted=False`
   - runtime heartbeat 持续显示 `npcAState=Inactive / npcBState=Inactive`
4. 我进一步在 Play Mode 里直读了 `001 / 002 / 003` 的运行时组件，拿到了本轮最关键的 runtime 真值：
   - 三者都 `IsResidentScriptedControlActive=true`
   - `ResidentScriptedControlOwnerKey="spring-day1-director"`
   - 这说明 Town clean play 下，这三个 resident 仍被 Day1 director 持有，当前 managed roam probe 不是 clean navigation-only baseline
5. 又确认了额外污染源：
   - `002` 的 `NPCBubbleStressTalker`
   - `startOnEnable=true`
   - `disableRoamWhileTesting=true`
   - 运行时会直接把 `roamController.enabled=false`
6. fresh 代码侧证据已补：
   - `validate_script NPCAutoRoamController.cs` => `owned_errors=0`
   - `validate_script NavigationAvoidanceRulesTests.cs` => `owned_errors=0`
   - scoped `git diff --check`（仅 own 两文件）通过
   - 清理掉 MCP 读组件引出的插件噪音后，fresh `errors` => `0 error / 0 warning`

### 关键决策
- 这轮我选择先 `Park-Slice`，不继续在导航 own 上盲改。
- 理由不是我不想继续做，而是现在继续磨导航，只会反复吃被 Day1 / 测试器污染的 Town live 样本。
- 当前最诚实的结论是：
  - 导航 own 的代码层和 targeted tests 已站住
  - Town fresh live 仍未形成可直接判导航 own 的 clean baseline

### 验证结果
- `doctor` => baseline `pass`
- `RunNpcRoamRecoverWindow` => `pass=False`, `seedAccepted=False`
- `RunNpcRoamPersistentBlockInterrupt` => `pass=False`, `seedAccepted=False`
- Play Mode runtime state：
  - `001 / 002 / 003` 全部 `IsResidentScriptedControlActive=true`
  - owner = `spring-day1-director`
- `002` 另有 `NPCBubbleStressTalker` 会直接禁用 roam controller
- CLI：
  - `validate_script` 两目标都 `owned_errors=0`
  - assessment 都仍是 `unity_validation_pending`，主因是 `stale_status / codeguard timeout-downgraded`
  - fresh `errors` 最终已清到 `0 / 0`

### 遗留问题 / 下一步
- 如果下一轮继续，最确信的下一步不是再调导航参数，而是先做这件事之一：
  1. 拿掉 Town live 的 `spring-day1-director` resident hold
  2. 拿掉 `002` 的 `NPCBubbleStressTalker` 测试残留
  3. 或换一个真正 clean 的 navigation-only runtime baseline
- 只有 baseline 干净后，Town live 才能继续拿来判我这轮导航修复到底还剩什么。

### 当前状态
- 本轮已合法执行 `Park-Slice`
- 当前 thread-state = `PARKED`
- 当前恢复点：
  - “导航代码层已站住，但 Town live 仍被 Day1/test 污染；不要继续拿 contaminated sample 逼导航 own 盲改”

## 2026-04-15｜Town free-time live 已把导航 own 真 residual 压实

### 用户目标
- 用户要求我不要停在分析，要在 `Town + opening结束后` 的 live 现场直接大量测试。
- 本轮子任务是：把“当前到底是不是 Day1 没放手”和“真正 free-time 下导航还剩什么问题”压成 runtime 真值。

### 已完成事项
1. 先在用户给的现场补抓：
   - `spring-day1-resident-control-probe.json`
   - `spring-day1-actor-runtime-probe.json`
   - `npc-roam-spike-stopgap-probe.json`
2. 先证伪了一个关键误会：
   - 用户那时给我的现场，系统真值其实还在 `EnterVillage / HouseArrival`
   - 不是普通 resident 已彻底 free-roam 的时刻
3. 因为这层误会还在，我又补跑了：
   - `Bootstrap Spring Day1 Validation`
   - `Run Runtime Targeted Probe`
   - `Force Spring Day1 FreeTime Validation Jump`
4. 真正 `FreeTime / FreeTime_NightWitness / 19:30~19:54` 下，现在已确认：
   - `001 / 002 / 101 / 103 / 104 / 201 / 202 / 203` 已进入 free-time roam / short pause / ambient chat
   - `102` 仍是 `night-witness-102` staged 特例，被 `SpringDay1NpcCrowdDirector` 持有，`roamControllerEnabled=false`
   - `003` 仍是 `night-witness-102` staged 特例，被 `spring-day1-director` 持有，`Inactive`
5. fresh free-time spike probe 结果：
   - `roamNpcCount=23`
   - `avgFrameMs=21.44ms`，后续短窗 `26.38ms`
   - `maxBlockedAdvanceFrames=417`
   - `topSkipReasons` 出现 `Stuck=49`
6. console 又直接给出关键 live 证据：
   - `[NPCAutoRoamController] 201 roam interrupted => Reason=StuckRecoveryFailed`
7. 我又直接读了 `201 / 001 / 002` 的运行态组件：
   - `201` 当前正在 roam，最近确实有 shared avoidance detour 历史，并刚刚打出过 `StuckRecoveryFailed`
   - `001 / 002` 不是冻结，而是在 free-time 下短暂停留聊天，`LastAmbientDecision=joined chat with 002 / chatting with 001`

### 关键决策
- 现在不能再把主判断写成“Day1 还没把 resident 放掉”。
- 当前更准确的口径是：
  1. `102 + 003` 仍是 Day1 staged 特例，不算导航 own 普通 resident 故障
  2. 普通 resident 和 `001 / 002` 的 free-time autonomous roam 已经接上
  3. 导航 own 现在最真实的主 residual 是：
     - `不会自救`：`201` 已出现 `StuckRecoveryFailed`
     - `坏点 / blocked advance residual`：short-window spike 仍有 `maxBlockedAdvanceFrames=417`、`Stuck=49`

### 验证结果
- `runtime phase`: `FreeTime`
- `beatKey`: `FreeTime_NightWitness`
- `resident release`：
  - 普通 resident：已 release
  - `102 / 003`：仍 staged
- `spike probe`：
  - `avgFrameMs=21.44ms / 26.38ms`
  - `maxBlockedAdvanceFrames=417`
  - `Stuck=49`
- `live console`：
  - `201 roam interrupted => StuckRecoveryFailed`

### 遗留问题 / 下一步
- 下一轮如果继续真实施工，最确信的下一步不再是“查是不是没放行”，而是：
  1. 沿 `201 -> StuckRecoveryFailed` 这条链补自救闭环
  2. 顺着 `maxBlockedAdvanceFrames=417 / Stuck=49` 收坏点与 blocked advance residual
  3. 继续把 `102 / 003` 当成 Day1 staged 特例，不要误吞成普通 resident 导航 bug

## 2026-04-15｜先提交 own checkpoint，再落导航健康首刀

### 用户目标
- 用户明确要求：
  1. 先把前面的测试数据/必要内容保留并提交
  2. 然后直接进入“导航健康”处理，不再停在分析

### 已完成事项
1. 我先把自己前面已经做完的 own 内容做成了本地 checkpoint：
   - commit：`658efdff`
   - 内容包含：
     - `Package A` 导航合同统一
     - facade consumer surface 改口
     - `导航V3` 子工作区 / 父层 / 线程 memory
2. 随后我重新开了：
   - `nav-health-stuck-recovery-and-badpoint-fix`
3. 这轮我只盯两条 true residual：
   - `StuckRecoveryFailed`
   - `blocked advance / 坏点 residual`
4. 我在 `NPCAutoRoamController.cs` 里落下的真实修复是：
   - `autonomous roam` 近目标失败时，允许 `AutonomousSoftArrival`
   - moving 中出现：
     - `PathClearedWhileMoving`
     - `WaypointMissingWhileMoving`
     - `blocked advance`
     - `stuck recovery failed`
     不再直接回原坏循环，而是先尝试：
       - 软着陆
       - 记坏点
       - 立即换点
5. 我补了两条新测试：
   - `NPCAutoRoamController_ShouldSoftArrive_WhenAutonomousFailureHappensNearDestination`
   - `NPCAutoRoamController_ShouldNotSoftArrive_WhenAutonomousFailureIsStillFarFromDestination`
6. fresh 代码验证：
   - `validate_script NPCAutoRoamController.cs` => `errors=0 / warnings=1`
   - `validate_script NavigationAvoidanceRulesTests.cs` => `errors=0 / warnings=0`
   - `NavigationAvoidanceRulesTests` => `30/30 passed`
   - `git diff --check`（本轮两文件）通过

### live 结果
1. 我直接在 `Town` 里做了两轮最小 live 回归：
   - `Play`
   - `Force Spring Day1 FreeTime Validation Jump`
   - `Run Roam Spike Stopgap Probe`
   - 每轮 6 秒后立刻停
2. 第一轮：
   - resident probe：
     - `phase=FreeTime`
     - `beat=FreeTime_NightWitness`
     - `101/102/103/104/201/202/203` 全部 `isMoving=true`
     - 以上条目全部 `blockedAdvanceFrames=0 / consecutivePathBuildFailures=0 / lastMoveSkipReason=AdvanceConfirmed`
   - spike probe：
     - `npcCount=25`
     - `roamNpcCount=25`
     - `avgFrameMs=1.24`
     - `maxBlockedNpcCount=0`
     - `maxBlockedAdvanceFrames=0`
     - `maxConsecutivePathBuildFailures=0`
     - `topSkipReasons=AdvanceConfirmed`
3. 第二轮复跑：
   - spike probe：
     - `npcCount=25`
     - `roamNpcCount=16`
     - `avgFrameMs=1.04`
     - `maxBlockedNpcCount=0`
     - `maxBlockedAdvanceFrames=0`
     - `maxConsecutivePathBuildFailures=0`
     - `topSkipReasons=AdvanceConfirmed`
4. fresh console：
   - 没有新的导航 error
   - 只剩外部字体 warning：
     - `LiberationSans SDF (Runtime)` atlas static

### 关键决策
- 这轮我最核心的判断是：
  - 当前导航 own 的关键病灶已经不再是“跑一会儿就 blocked/stuck/path failure 爆表”
  - 而是已经回到可持续前进的健康态
- 我为什么认为这个判断成立：
  1. 两轮 free-time live 短窗都把 `maxBlockedAdvanceFrames` 压到 `0`
  2. `topSkipReasons` 不再出现 `Stuck`
  3. resident probe 里关键居民都变成 `AdvanceConfirmed`
- 我这轮最薄弱的点：
  - 现在拿到的是“两轮 6 秒短窗强正信号”
  - 还不是更长时长的 soak，也不是所有指定坏点的人工逐点巡检
- 我这轮自评：
  - `8.8/10`
  - 这次不是只做结构层或静态推断，而是真的把 live 上最刺眼的 `blocked/stuck` 压下去了；没给满分，是因为终极“健康导航”还差更长窗口和用户体感终验

### 当前状态 / 恢复点
- 这轮已合法 `Park-Slice`
- 当前 thread-state = `PARKED`
- 如果继续下一轮，最稳的顺序是：
  1. 先把这刀代码 + memory 提交
  2. 再看用户是否要更长 free-time soak，或指定坏点人工复验
