# 导航V3 - 开发记忆

## 模块概述
- 本工作区当前用途不是直接施工，而是作为“导航总交接后的接手壳 + 现状复核入口”。
- 当前真正持续更新的 live 导航材料仍集中在 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\`，本工作区负责单独沉淀 `导航V3` 线程自己的判断、恢复点和后续接手顺序。

## 当前状态
- **完成度**：5%
- **最后更新**：2026-04-15
- **状态**：只读分析完成，待决定是否进入真实施工

## 会话 1 - 2026-04-15（导航现状复核）

**用户需求**：
> 先不要直接干活；先彻底查清导航现状，结合交接文档、聊天记录、当前代码和 live artifact，告诉我你的思考、剩余需求、任务清单和下一步方向，要求列点清晰、言简意赅、全部说人话。

**当前主线目标**：
- 先把导航 own 的真实现状重新钉死，再决定后续是否继续施工。

**这轮完成**：
1. 确认 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航V3\` 与 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V3\` 当前都还是空壳；真正持续更新的 live 材料仍在 `导航检查`。
2. 回读并对齐：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-15_导航线程总交接文档_own问题_边界语义_历史修改_后续接手.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\03_NPC自漫游与峰值止血\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-15_给导航_Day1解耦后return-home与free-roam执行合同唯一主刀prompt_68.md`
3. 对位当前核心代码：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCMotionController.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationTraversalCore.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationAgentRegistry.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1NpcCrowdDirector.cs`
4. 新增 fresh 证据：
   - `python scripts/sunset_mcp.py validate_script ...` 返回 `assessment=external_red / owned_errors=0 / external_errors=1`
   - 当前外部阻断是 `DialogueChinese V2 SDF.asset` importer inconsistent，不是导航 own compile red
   - `python scripts/sunset_mcp.py status` 显示当前 Unity 在 `Town.unity / Edit Mode`，bridge baseline 可连，但 `ready_for_tools=false(stale_status)`
   - `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\npc-roam-spike-stopgap-probe.json` 当前最新样本仍显示：
     - `avgFrameMs=31.46`
     - `maxFrameMs=102.88`
     - `Stuck=21`
     - `SharedAvoidance=9`
     - `MoveCommandNoProgress=2`
5. 形成 `导航V3` 当前独立判断：
   - 用户压的 3 个主根 `坏点 / 静态导航垃圾 / 不会自救` 成立
   - Day1 越权仍是真问题，但导航 own 也远未过线
   - 当前 live 不能再说“只剩 Day1”或“只剩导航参数”

**关键判断**：
- 当前第一层问题是目标点合同仍然不够硬；系统还保留着“先抽到可能坏点，再尝试修正/约束”的旧 DNA。
- 当前第二层问题是身体宽度只在 `destination acceptance / next-step constraint` 部分推进了，但还没彻底推进到“路径成立层”。
- 当前第三层问题是 blocked/stuck 仍主要停在“止血式快放弃”，还不是完整脱困闭环。
- `Day1` 当前仍深碰 resident runtime body；但把 `CrowdDirector` 关掉后 Town resident 能 roam、只是回家逻辑缺失，这说明边界问题和导航 own 问题是并存，不是互斥。

**涉及文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-15_导航线程总交接文档_own问题_边界语义_历史修改_后续接手.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\03_NPC自漫游与峰值止血\memory.md`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCMotionController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationTraversalCore.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationAgentRegistry.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1NpcCrowdDirector.cs`
- `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\npc-roam-spike-stopgap-probe.json`

**验证结果**：
- `结构 / checkpoint`：成立
- `targeted probe / 局部验证`：成立
- `真实入口体验`：未过线

**遗留问题 / 恢复点**：
- 如果后续继续施工，优先顺序应固定为：
  1. `destination hard contract`
  2. `path-level body clearance`
  3. `完整脱困闭环`
- 不要再先磨零散参数，也不要把 `Day1 / Navigation / NPC body` 混成一刀。

## 会话 2 - 2026-04-15（Day1-V3 语义同步后的导航执行合同复判）

**用户需求**：
> 先完整读完 `Day1-V3` 给导航的弱引导 prompt 和它引用的两份正文，然后只站在导航 own 的位置深度思考：导航执行层还分裂成几套 movement contract、哪些该合并、会不会伤到 Day1 staged contract、唯一下一刀是什么，并把方向调整、所有问题和剩余未做清楚列出来。

**当前主线目标**：
- 不回吞 Day1 phase，只从导航 own 角度重判执行合同分裂现状和合并顺序。

**这轮完成**：
1. 完整阅读并对齐：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\102-owner冻结与受控重构\2026-04-15_给导航_Day1-V3语义同步与执行边界弱引导prompt_01.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\102-owner冻结与受控重构\2026-04-15_Day1-V3_Day1现存问题总览与整体施工总表.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\102-owner冻结与受控重构\2026-04-15_Day1-V3_给导航_语义同步与执行边界contract.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\102-owner冻结与受控重构\memory.md`
2. 回到导航现码继续对位 `movement contract` 分叉：
   - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
   - [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs)
3. 当前导航 own 的新判断已压成一张更清楚的边界图：
   - 导航 own 内部至少仍分裂为 4 套 movement contract：
     1. `autonomous roam contract`
     2. `resident scripted move contract`
     3. `formal navigation debug contract`
     4. `plain debug/manual fallback contract`
   - 在导航 own 之外，还存在 1 套完全绕开导航的 `Day1 staged playback` 直推链。
4. 重新校正“统一方向”的口径：
   - 我认同“只要发生真实位移，原则上应优先走统一导航执行合同”这条边界；
   - 但我不主张这一步直接把 `opening / dinner` 的 authored staged playback 整体吞进当前 free-roam contract。

**关键判断**：
- 当前最值钱的统一方向，不是“先把所有位移都塞进同一个大状态机”，而是先把：
  - `resident scripted move`
  - `formal navigation debug/home-return`
  - `plain debug/manual fallback`
  这 3 套已经属于导航 own 的 point-to-point 执行合同，统一成一套正式 `point-to-point navigation travel contract`。
- `autonomous roam` 暂时不应与 staged contract 强行合并；它应继续是“目标来源”不同的一层，而不是继续和 point-to-point travel 共用一堆半 debug 半 fallback 的执行口径。
- `Day1 staged playback` 当前仍应保留独立顶层合同，因为它还承担：
  - authored cue path
  - 5 秒 timeout
  - 超时 snap
  - dialogue freeze
  这套导演语义；但它后续可逐步改成“语义仍独立，底层执行能力向统一 travel contract 借力”。

**当前方向调整**：
1. 上一轮我把第一优先级放在 `destination hard contract -> path-level body clearance -> 完整脱困闭环`。
2. 这轮在读完 Day1-V3 contract 后，我认为这条排序仍成立，但前面要再补一个更靠前的结构动作：
   - 先统一导航 own 内部 `point-to-point travel contract`
3. 也就是说当前更准确的顺序变成：
   1. `统一 point-to-point navigation travel contract`
   2. `destination hard contract`
   3. `path-level body clearance`
   4. `完整脱困闭环`

**为什么这样调整**：
- 因为如果 `return-home / release-after-story / fallback/debug move` 继续分裂成多套执行口径，后面再硬化坏点和 clearance，也还是会被旁路合同重新污染。

**遗留问题 / 恢复点**：
- 后续如果进入真实施工，导航自己的唯一第一刀应先统一 `point-to-point travel contract`，而不是先回吞 Day1 staged playback。
- 当前仍未做 fresh live 终证；这轮判断站住的是结构层与边界层，不是玩家体验已过线。

## 会话 3 - 2026-04-15（导航V3 正式执行清单立项）

**用户需求**：
> 导航修复不是一轮能完成；希望我先给自己立一份详细的当前清单，把语义、方向和这次导航修复的全流程内容全部定义好，作为后续持续推进的正式落地点。

**当前主线目标**：
- 把导航V3 后续施工从“聊天分析”升级成“固定执行表”，让下一轮不再散修、不再反复重判顺序。

**这轮完成**：
1. 确认并保留一份正式执行表：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V3\2026-04-15_导航V3_导航修复当前执行方向与详细清单.md`
2. 这份文件已明确写死：
   - 当前语义冻结
   - Day1 / 导航 / NPC body 边界
   - movement contract 分裂图
   - 当前唯一施工顺序
   - 每个 package 的范围、完成标准、停步条件
3. 重新确认这份执行表与 `Day1-V3` 的弱引导边界一致：
   - 不回吞 `Day1 phase`
   - 不先吞 `opening / dinner staged playback`
   - 导航第一刀仍是 `统一 point-to-point navigation travel contract`

**关键决策**：
- 从现在起，这份执行表就是 `导航V3` 的 live baseline。
- 后续导航继续推进时，默认先按这份表开包，不再回到“哪又抽了就补哪”的散修模式。
- 当前正式排序继续固定为：
  1. `统一 point-to-point navigation travel contract`
  2. `destination hard contract`
  3. `path-level body clearance`
  4. `完整脱困闭环`
  5. `NPC facade / Day1 call-site 收口`
  6. `live 终验`

**验证结果**：
- `结构 / checkpoint`：成立
- `targeted probe / 局部验证`：成立
- `真实入口体验`：仍未过线

**遗留问题 / 恢复点**：
- 当前还没有进入真实施工；thread-state 仍保持 `PARKED`。
- 下一轮如果继续只读，优先围绕这份执行表继续补入口图、失败处理图和 facade 消费面。
- 下一轮如果进入真实施工，必须先 `Begin-Slice`，然后只开 `Package A｜统一 point-to-point navigation travel contract`。

## 会话 4 - 2026-04-15（Package A 第一段真实施工）

**用户需求**：
> 彻底开工，按照执行清单一直往下做。

**当前主线目标**：
- 正式进入 `Package A｜统一 point-to-point navigation travel contract`，先把 scripted / return-home / debug/manual fallback 的执行真相合并，不碰 Day1 phase。

**这轮完成**：
1. 已真实进入施工，并按 Sunset 规则跑：
   - `Begin-Slice`
   - 收尾再 `Park-Slice`
2. 已在 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 落下 Package A 第一段结构统一：
   - 新增显式 `PointToPointTravelContract` 枚举，替代旧的 `debugMoveUsesFormalNavigationContract` 隐式拼装状态。
   - `RequestStageTravel / RequestReturnHome / BeginAutonomousTravel / BeginReturnHome / DebugMoveTo` 现在都显式带 contract 进入同一个 `BeginPathDirectedMove(...)` 真相入口。
   - `RequestStageTravel` 明确走 `ResidentScripted`。
   - `RequestReturnHome` 与 `BeginReturnHome` 明确走 `FormalNavigation`。
   - `BeginAutonomousTravel` 明确走 `AutonomousDirected`。
   - `DebugMoveTo` 明确保留为 `PlainDebug`。
3. 已把 recoverable 的 point-to-point contract 往统一执行层收一层：
   - `AutonomousDirected / ResidentScripted / FormalNavigation` 现在共用 recoverable stuck 分支。
   - 同上 3 类也共用 recoverable blocked-advance 分支。
   - `PlainDebug` 仍保留 debug-only 的 bypass 语义，不再冒充正式执行真相。
4. 已补一层轻行为统一：
   - recoverable point-to-point contract 现在统一吃静态障碍 steering；
   - `PlainDebug` 继续不吃这层。
5. 已同步改测试：
   - [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs)
   - 旧的 `debugMoveUsesFormalNavigationContract` / 旧推断方法反射口已换到新枚举 contract。

**关键决策**：
- Package A 当前先做的是“显式 contract + 统一入口 + 统一 recoverable 分支”，不是直接跳去做 destination hard contract 或 path clearance。
- `PlainDebug` 继续保留为 debug-only 例外口；真正的 scripted / return-home / autonomous directed 现在开始共用同一套 contract 身份。

**验证结果**：
- `validate_script`：
  - `NPCAutoRoamController.cs` => `assessment=unity_validation_pending / owned_errors=0 / external_errors=0`
  - `NavigationAvoidanceRulesTests.cs` => `assessment=unity_validation_pending / owned_errors=0 / external_errors=0`
  - 当前没拿到 `no_red` 的原因仍是 Unity `stale_status`，不是这轮 own compile red。
- EditMode targeted tests：`2/2 passed`
  - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldTreatStoryOwnedFormalReturn_AsResidentScriptedMove`
  - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldKeepPlainDebugBypass_ButEnableRecoverablePointToPointStaticSteering`
- `git diff --check`：
  - 仅针对本轮 2 个文件执行，结果通过。

**当前阶段判断**：
- `Package A` 已开工，但还没做完。
- 这轮完成的是“执行合同显式化 + 第一层 recoverable 统一”，还没进入 `destination hard contract`。

**遗留问题 / 恢复点**：
- 下一轮继续 Package A 时，优先补：
  1. 继续清点并收剩余仍靠 `debugMoveActive / IsResidentScriptedControlActive` 硬判断的旁路分支。
  2. 再确认 point-to-point contract 的终止 / abort / restart 语义是否还有漏网的旧 debug 味。
  3. 然后再考虑是否进入 Package A 的下一段 facade / call-site 收口，仍不越到 Package B。
- 当前 thread-state 已合法停回 `PARKED`，适合从这个恢复点继续接。

## 会话 5 - 2026-04-15（Package A 第二段真实施工：按 NPC 回执继续收内部合同）

**用户需求**：
> NPC 已给出新的回执；让我自行调整导航 own 判断，然后继续把自己的内容往后做。

**当前主线目标**：
- 不改 Day1 phase，只根据 NPC facade 已落地这件事，把导航内部剩余的 point-to-point / autonomous roam 分流继续收干净。

**这轮完成**：
1. 先核对了 NPC 回执和现码，确认：
   - facade 确实存在；
   - resident scripted 低级写口确实已经收进 private；
   - 但 Day1 仍残留少量 `StartRoam()` 直调点，因此当前更准确口径应是：
     - `NPC facade 基本落成`
     - `Day1 call-site 仍有少量尾项`
2. 继续在 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 内部推进 `Package A`：
   - 新增 `UsesAutonomousRoamExecutionContract()`，把“autonomous roam contract”从旧布尔判断里抽出来。
   - `TryBeginMove / sample budget / recovery sampling / deadlock break / blocked abort / retarget / roam interruption` 等内部判断，进一步改成显式 contract helper，而不是继续散落的 `debugMoveActive / IsResidentScriptedControlActive` 组合。
   - `TryHandleBlockedAdvance` 与 detour recovery 相关分流继续按 contract 语义收口。
3. 再统一一层执行质量：
   - `ShouldUseReleasedMovementBodyClearanceExecutionContract()` 现在不再只给 `FormalNavigation`；
   - `AutonomousDirected / ResidentScripted / FormalNavigation` 这 3 类 recoverable point-to-point contract 都开始共用 released body clearance 执行合同；
   - `PlainDebug` 仍排除在外。
4. 补了新测试：
   - [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs)
   - 新增 `NPCAutoRoamController_ShouldEnableReleasedBodyClearance_ForRecoverablePointToPointContracts`

**关键决策**：
- NPC 回执没有推翻我当前方向，只让我把边界再收窄：
  - 导航 own 继续只收执行质量；
  - 不再把 story-facing facade 当成我要主消费的业务合同。
- 这轮没有去碰 Day1 phase，也没有去替 Day1 改 prompt。

**验证结果**：
- `validate_script`：
  - `NPCAutoRoamController.cs` => `assessment=unity_validation_pending / owned_errors=0 / external_errors=0`
  - `NavigationAvoidanceRulesTests.cs` => `assessment=unity_validation_pending / owned_errors=0 / external_errors=0`
  - 当前仍没拿到 `no_red` 的主因是 Unity `stale_status`，不是本轮 own red。
- EditMode targeted tests：`3/3 passed`
  - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldTreatStoryOwnedFormalReturn_AsResidentScriptedMove`
  - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldKeepPlainDebugBypass_ButEnableRecoverablePointToPointStaticSteering`
  - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldEnableReleasedBodyClearance_ForRecoverablePointToPointContracts`
- `git diff --check`（仅本轮代码/测试文件）通过。

**当前阶段判断**：
- `Package A` 还没完成，但内部合同分流已经又收了一大段。
- 当前剩余不再是最粗的“contract 身份不清”，而更接近：
  - 少量残余旁路
  - facade / call-site 尾项
  - 然后才轮到 Package B

**遗留问题 / 恢复点**：
- 下一轮继续时，优先做两件事：
  1. 只在导航 own 里继续清少量残余旁路，不越包。
  2. 再判断是否已到可以开始 `Package A` 的 facade / call-site 收口。
- Day1 侧目前我自己的判断不变：
  - 仅建议后续把少量 `StartRoam()` 残留清到 `ResumeAutonomousRoam(...)`；
  - 这不是导航当前主刀。
- 当前 thread-state 已合法停回 `PARKED`。

## 会话 6 - 2026-04-15（Package A 第三段真实施工：导航验证/探针消费面改口）

**用户需求**：
> 继续往下做。

**当前主线目标**：
- 继续留在 `Package A`，把导航 own 的 validation / probe 消费面从旧的 `DebugMoveTo / StartRoam / StopRoam` 改到新 contract，不碰 Day1 phase。

**这轮完成**：
1. 先核清 live 现场时发现：`Show-Active-Ownership` 里 `导航V3` 实际仍是 `ACTIVE`，不是我上一轮记忆里的 `PARKED`；因此这轮直接沿现有 slice 继续施工，收尾再重新 `Park-Slice`。
2. 已在 [NavigationLiveValidationRunner.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs) 把导航 own 的 live validation 消费面改口：
   - `DebugMoveTo(...)` 全部切到 `BeginAutonomousTravel(...)`
   - managed roam 重启从 `StartRoam()` 切到 `ResumeAutonomousRoam()`
   - parking / cleanup 从 `StopRoam()` 切到 `SnapToTarget(...)`
3. 已在 [CodexNpcTraversalAcceptanceProbeMenu.cs](D:/Unity/Unity_learning/Sunset/Assets/Editor/NPC/CodexNpcTraversalAcceptanceProbeMenu.cs) 把 acceptance probe 改到 facade：
   - probe 摆位从 `StopRoam()` 切到 `SnapToTarget(...)`
   - probe 发起移动从 `DebugMoveTo(...)` 切到 `BeginAutonomousTravel(...)`
4. 已在 [NpcLocomotionFacadeSurfaceTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NpcLocomotionFacadeSurfaceTests.cs) 补消费面边界测试：
   - 继续验证 facade/runtim-only/debug-only surface scope
   - 新增 source test，钉死这两个导航 own 工具文件不再直调 `DebugMoveTo / StartRoam / StopRoam`
   - 修正 source test 的项目根路径解析，避免 `TestDirectory` 变化时误判

**关键决策**：
- 这轮没有去改导航算法本体，也没有越到 `Package B`。
- 我把这轮定义为 `Package A` 的“consumer surface 收口”，因为它的价值是防止导航 own 的验证工具继续把旧 contract 回流成默认消费面。
- `SnapToTarget(...)` 虽然不是 navigation-facing facade，但对 probe/runtime 的瞬时摆位属于显式外部合同，比继续拼 `StopRoam + 手工挪位` 更干净。

**验证结果**：
- direct MCP `validate_script`
  - `NavigationLiveValidationRunner.cs` => `errors=0 / warnings=2`
    - warning 为现有大文件级提示：`Rigidbody 建议 FixedUpdate`、`Update 里字符串拼接`
  - `CodexNpcTraversalAcceptanceProbeMenu.cs` => `errors=0 / warnings=0`
  - `NpcLocomotionFacadeSurfaceTests.cs` => `errors=0 / warnings=0`
- CLI `errors`：
  - `errors=0 / warnings=0`
  - 仅出现一条测试结果保存日志，不是项目红错
- targeted EditMode tests：
  - `NpcLocomotionFacadeSurfaceTests` => `5/5 passed`
- `git diff --check`（仅本轮 3 个文件）通过
- 额外说明：
  - CLI `validate_script` 对其中 2 个目标再次撞到 `CodexCodeGuard returned no JSON` / `stale_status`
  - 因此这轮 Unity compile-first 证据不是完全理想闭环，但没看到 own compile red

**当前判断**：
- `Package A` 现在已经不只是在 `NPCAutoRoamController` 内部收合同，也开始把导航 own 的外部消费面收窄。
- 这轮最值钱的结果是：导航自己的验证工具不再继续把旧 debug/runtime 写口当正式 contract 示例。
- 但 `Package A` 仍未宣布收完；当前只是又收掉了一块最脏的 consumer surface。

**遗留问题 / 恢复点**：
- 下一轮继续时，优先判断：
  1. 导航 own 是否还残留别的 validation/probe/tool call-site 在继续直调旧口
  2. 如果导航 own 这条消费面已足够干净，再决定 `Package A` 是否可以正式切到收尾判断，还是还要补一小刀 facade/call-site 尾项
- 这轮收尾已重新执行 `Park-Slice`，当前 thread-state 回到 `PARKED`。

## 会话 7 - 2026-04-15（用户追问：为什么不是一轮直接从 A 到 D）

**用户需求**：
> 想知道能不能一轮内从 A 到 D 一直做完；不想看中间产物，只想要最终导航；也不理解为什么需要阶段性，希望我用人话解释清楚。

**当前主线目标**：
- 解释当前导航修复为什么要按 `A~D` 分层推进，并把“连续施工”和“阶段收口”之间的边界说清楚。

**这轮结论**：
1. 可以连续往下做，但我不能负责任地预先承诺“一轮必定从 A 到 D 全做完”。
2. 原因不是我想产出中间稿，而是 `A~D` 本身对应 4 层不同问题：
   - `A` = 统一 point-to-point 执行合同
   - `B` = 坏点硬合同
   - `C` = 路径级 body clearance
   - `D` = 完整脱困闭环
3. 这 4 层是上下游关系，不是随便拆段：
   - `A` 不稳，后面 `B/C/D` 会继续被旧入口污染
   - `B` 不稳，`C/D` 会一直吃坏目标点样本
   - `C` 不稳，`D` 会被迫一直替错误路径擦屁股
4. 所以当前最准确口径不是“非要做阶段汇报”，而是：
   - 我可以连续施工
   - 但每清完一层，必须先确认没有把下一层的样本污染掉，再继续往下砍
5. 用户不要中间产物这件事我已接受：
   - 后续我默认继续往下做，不会每小步都停下来要你拍板
   - 只有当我真的撞到 blocker，或者需要告诉你“当前还不能宣称最终导航已过线”时，才会停下来报实

**当前判断**：
- 你说“从你的视角这本来就该一轮做完”这个要求是合理的。
- 我这里需要保留的不是“阶段性汇报习惯”，而是“不能把 4 个不同病层混成一次瞎修，然后最后再说 done”。
- 也就是说：
  - `可以连续施工`
  - `不能跳过分层判断`

**恢复点**：
- 如果继续真实施工，下一轮就按这个口径执行：
  - 不给你交中间包装
  - 我自己连续往下砍
  - 但内部仍按 `A -> B -> C -> D` 判断是否已经真正站稳

## 2026-04-15｜Town live 取证补记：导航 own 代码站住，但 fresh live 被 Day1/test 残留污染

### 用户目标
- 用户要求我继续往下做，不要停在空分析；因此这轮我没有再盲改导航代码，而是先补 fresh Town live 取证，确认当前真实阻塞点到底还在不在导航 own。

### 当前主线
- 仍是导航 own 的 `Package A-D` 主线。
- 本轮子任务：
  - 用现成 `NavigationLiveValidationRunner` 的 `NpcRoamRecoverWindow / NpcRoamPersistentBlockInterrupt` 补 Town runtime 证据
  - 不回吞 Day1 phase，但要把“当前 live 为何还不能直接拿来判导航 own”钉成 runtime 真值

### 已完成事项
1. 读取并确认了 live 前置：
   - `mcp-single-instance-occupancy.md`
   - `mcp-live-baseline.md`
   - `mcp-hot-zones.md`
   - CLI `doctor` = baseline `pass`
2. 在 `Town` clean play 下实际跑了两条 live：
   - `RunNpcRoamRecoverWindow`
   - `RunNpcRoamPersistentBlockInterrupt`
3. 两条 probe 的共同结果不是“撞墙卡死”，而是都在 `seedAccepted=False` 处超时，且控制台 heartbeat 一致显示：
   - `npcAState=Inactive`
   - `npcBState=Inactive`
   - NPC 很快从 probe 摆位回到各自 runtime 位置
4. 我又在 Play Mode 里直接读了 `001 / 002 / 003` 的运行时组件真值，确认：
   - 三者 `IsResidentScriptedControlActive=true`
   - `ResidentScriptedControlOwnerKey="spring-day1-director"`
   - 说明 Town clean play 下这三个 resident 仍被 Day1 director 持有，导航 roam probe 进场前就被剧情 owner 抢回
5. 又补抓到一层测试污染：
   - `002` 身上 `NPCBubbleStressTalker`
   - `TestModeEnabled=true`
   - `StartOnEnable=true`
   - `DisableRoamInTestMode=true`
   - 其 `OnEnable()` 会直接把 `NPCAutoRoamController.enabled=false`
6. 代码侧 fresh 证据已补：
   - CLI `validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - `owned_errors=0`
     - `assessment=unity_validation_pending`
     - native validate 只有 1 条 warning：`Update()` 字符串拼接 GC 提示
   - CLI `validate_script Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs`
     - `owned_errors=0`
     - `assessment=unity_validation_pending`
   - scoped `git diff --check`（仅 own 两个文件）通过
   - fresh `errors` 在清理 MCP 读组件噪音后恢复到 `errors=0 warnings=0`

### 关键判断
1. 这轮 runtime 已经足够说明：当前 Town clean play 的 live probe 失败，不能直接归因到我刚补的坏点/静态自救代码。
2. 当前更准确的口径是：
   - 导航 own 的代码层与 targeted tests 站住了
   - Town fresh live 仍被 `spring-day1-director` resident control 污染
   - 另外 `002` 还被 `NPCBubbleStressTalker` 测试器直接禁 roam
3. 因此这轮继续在导航 own 里盲调参数，只会继续吃 contaminated sample，不会得到可信 live 结论。

### 涉及文件 / 证据点
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NavigationAvoidanceRulesTests.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLiveValidationRunner.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubbleStressTalker.cs`

### 当前恢复点
- 这轮没有继续扩写导航代码。
- 已执行 `Park-Slice`，thread-state 已回到 `PARKED`。
- 如果继续推进，最小合理顺序是：
  1. 先拿掉 Town live 里的 story/test 污染，再重新跑 managed roam probe
  2. 在 clean baseline 下再判断导航 own 是否还有真实 residual
  3. 如果 baseline 仍不干净，就不要把 Town live 现象包装成“导航 own 已败北”

## 2026-04-15｜Town live 二次收窄：真 free-time 下导航 own 现状已压实

### 用户目标
- 用户要求我在 `Town + opening结束后` 的 live 现场直接大量测试，不要只停在分析。
- 这轮子任务是：把“当前到底是不是 Day1 还没放手”“真正 free-time 下还剩哪些导航 own 问题”彻底压成 runtime 真值。

### 本轮实际做成了什么
1. 先在用户给的现场补抓：
   - `spring-day1-resident-control-probe.json`
   - `spring-day1-actor-runtime-probe.json`
   - `npc-roam-spike-stopgap-probe.json`
2. 第一层 live 结论确认：
   - 用户口中的“opening 结束后”当时实际仍是 `EnterVillage / HouseArrival`
   - 不是 resident 全体已经自由放行的时刻
   - `001/002` 仍在 `spring-day1-director` scripted move
   - `003` 仍被 `spring-day1-director` 持有且 `Inactive`
   - `101~203` 多数已经 release 并在 roam
3. 我补跑了：
   - `Sunset/Story/Debug/Bootstrap Spring Day1 Validation`
   - `Tools/NPC/Spring Day1/Run Runtime Targeted Probe`
4. 这个 targeted probe 证明：
   - 背景推进恢复正常，之前“后台不推进”噪音已被排掉
   - 但它在 `EnterVillage` 这拍对 7 个 resident 的 informal / walk-away 全线失败
   - 所以它更像“当前相位不适合拿来验 resident free-roam”，不是导航 own 已经通过
5. 我随后直接执行：
   - `Sunset/Story/Validation/Force Spring Day1 FreeTime Validation Jump`
   - 再次抓 resident / actor / spike 三份 artifact
6. 真正 `FreeTime / FreeTime_NightWitness / 19:30~19:54` 下的 live 真值现在已经明确：
   - `001`：已 release，`IsRoaming=true`，`ShortPause`
   - `002`：已 release，`IsRoaming=true`，`ShortPause`
   - `101/103/104/201/202/203`：已 release，`IsRoaming=true`，多数处于 `ShortPause`
   - `102`：仍是 `night-witness-102` staged 特例，`roamControllerEnabled=false`，owner=`SpringDay1NpcCrowdDirector`
   - `003`：仍是 `night-witness-102` staged 特例，owner=`spring-day1-director`，`Inactive`
7. 最关键的导航 own 证据：
   - fresh spike probe：`roamNpcCount=23`
   - `avgFrameMs=21.44ms`，后续短窗又测到 `26.38ms`
   - `maxBlockedAdvanceFrames=417`
   - `topSkipReasons` 已出现 `Stuck=49`
   - console 明确出现：
     - `[NPCAutoRoamController] 201 roam interrupted => Reason=StuckRecoveryFailed`
8. 我又直接读了 `201 / 001 / 002` 当前组件：
   - `201` 正在 roam，`DebugPathPointCount=12`，最近有 shared avoidance detour create/release 历史，并且刚刚确实打出过一次 `StuckRecoveryFailed`
   - `001/002` 目前不是冻结，而是在 free-time 下短暂停留聊天，`LastAmbientDecision` 分别是 `joined chat with 002 / chatting with 001`

### 当前判断
1. 现在可以确认：
   - 当前真正 free-time 下，并不是“resident 根本没放行”
   - 也不是“Day1 还把所有人一直抓死”
2. 这轮最值钱的新结论是：
   - `102 + 003` 仍然属于 `FreeTime_NightWitness` 的 Day1 staged 特例，不应误算成导航 own 全局故障
   - 普通 resident 与 `001/002` 在 free-time 下已经能进入 autonomous roam
3. 导航 own 仍然没过线，但主问题已经收窄成更具体的两类：
   - `自救仍不稳`：`201` 已出现 `StuckRecoveryFailed`
   - `坏点 / blocked advance residual 仍在`：短窗 spike 里 `maxBlockedAdvanceFrames=417`、`Stuck=49`
4. 这也说明我之前压的三主根没有失效，只是当前 live 下最先冒头的是：
   - `不会自救`
   - 其次才是 `坏点 / blocked advance residual`

### 当前恢复点
- 这轮完成后已再次 `Park-Slice`，thread-state = `PARKED`。
- 下一轮如果继续真实施工，导航 own 最小正确顺序应改成：
  1. 先沿 `201 roam interrupted => StuckRecoveryFailed` 这条链补自救闭环
  2. 再顺着 `maxBlockedAdvanceFrames=417 / Stuck=49` 收坏点与 blocked advance residual
  3. 不把 `102 + 003` 这两个 `night-witness` staged 特例误吞进“普通 resident 导航故障”
