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

## 2026-04-15｜导航健康首刀：autonomous roam 失败先软着陆/换点，不再直接回坏循环

### 用户目标
- 用户明确要求先把前面的 own 内容提交，然后直接落地做“导航健康”本身，而不是继续停在分析。

### 本轮实际做成了什么
1. 已先完成 own checkpoint 提交：
   - 本地 commit：`658efdff`
   - 提交内容包含：
     - `Package A` 代码与测试
     - facade consumer surface 改口
     - `导航V3` 工作区 / 父层 / 线程 memory
2. 我随后重新开了 `nav-health-stuck-recovery-and-badpoint-fix` slice，只盯当前真实 residual：
   - `StuckRecoveryFailed`
   - `blocked advance / 坏点 residual`
3. 已在 `NPCAutoRoamController.cs` 落下这刀核心执行修复：
   - 当 `autonomous roam` 已经接近目标，但最后几步因为坏点/静态阻塞失败时，不再继续硬撞或重复中断，而是走 `AutonomousSoftArrival`
   - 当 `autonomous roam` 在 moving 中出现 `PathClearedWhileMoving / WaypointMissingWhileMoving / blocked advance / stuck recovery failed` 时，会先尝试：
     1. `soft arrival`
     2. 失败目标记坏点
     3. 立即重选新 roam 目标
   - 不再第一时间直接掉回原来的失败循环
4. 已补测试覆盖：
   - `NPCAutoRoamController_ShouldSoftArrive_WhenAutonomousFailureHappensNearDestination`
   - `NPCAutoRoamController_ShouldNotSoftArrive_WhenAutonomousFailureIsStillFarFromDestination`
5. 已做 fresh 代码验证：
   - `validate_script NPCAutoRoamController.cs` => `errors=0 / warnings=1`
     - 唯一 warning 仍是旧的 `Update()` 字符串拼接 GC 提示
   - `validate_script NavigationAvoidanceRulesTests.cs` => `errors=0 / warnings=0`
   - `NavigationAvoidanceRulesTests` => `30/30 passed`
   - `git diff --check`（本轮两文件）通过

### fresh live 结果
1. 我直接在 `Town` 里做了两轮短窗 live 回归：
   - 进入 `Play`
   - 强跳 `FreeTime`
   - 跑 `Tools/Sunset/NPC/Run Roam Spike Stopgap Probe`
   - 每轮 6 秒后立刻停
2. 第一轮 live：
   - `phase=FreeTime`
   - `beat=FreeTime_NightWitness`
   - resident probe 里 `101/102/103/104/201/202/203` 全部：
     - `isRoaming=true`
     - `isMoving=true`
     - `blockedAdvanceFrames=0`
     - `consecutivePathBuildFailures=0`
     - `lastMoveSkipReason=AdvanceConfirmed`
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

### 当前判断
1. 这刀已经把之前最刺眼的 live 病灶压掉了：
   - `StuckRecoveryFailed` 没再冒头
   - `blockedAdvanceFrames` 从先前的 `417` 压到两轮 `0`
   - `topSkipReasons` 不再出现 `Stuck`
2. 这说明当前导航 own 至少在这条 true free-time 短窗里，已经从“坏点+不会自救”回到“能持续前进”的健康态。
3. 这轮最值钱的不是把参数再调大，而是把失败处理语义从：
   - `失败 -> 中断/重试 -> 回原坏循环`
   改成：
   - `失败 -> 近点软着陆 或 记坏点后立即换点`

### 当前恢复点
- 本轮已再次 `Park-Slice`，当前 thread-state = `PARKED`。
- 下一轮如果继续，不该立刻再扩大改面，而应优先做：
  1. 把这刀代码 + memory 一起提交
  2. 若用户继续终验，再按同样 free-time live 口径做更长窗口或指定坏点复测

## 2026-04-16｜导航健康继续推进：先压实 repeat soak，再收 registry 全表扫描性能债

### 用户目标
- 用户明确要求我在后面无人监管时继续往下做，能测就测，测不了就继续做清单内容。
- 这轮我按这个要求继续推进，没有停在“昨天已经绿过一次”的阶段。

### 当前主线
- 仍是导航 own 的“导航健康 + 性能兼顾”。
- 这轮子任务分两段：
  1. 先把 free-time 短窗健康信号压成重复独立样本
  2. 再把代码里仍然存在的 registry 级全表扫描性能债收掉

### 本轮实际做成了什么
1. 先做了 3 次独立 `Town + Play + Force FreeTime + 6秒 spike probe + Stop` 验证：
   - Run1：`avgFrameMs=2.73 / roamNpcCount=18 / maxBlockedAdvanceFrames=0 / maxConsecutivePathBuildFailures=0`
   - Run2：`avgFrameMs=2.69 / roamNpcCount=25 / maxBlockedAdvanceFrames=0 / maxConsecutivePathBuildFailures=0`
   - Run3：`avgFrameMs=2.48 / roamNpcCount=25 / maxBlockedAdvanceFrames=0 / maxConsecutivePathBuildFailures=0`
   - 三轮共同点：
     - `maxBlockedNpcCount=0`
     - `blockedAdvanceStopgapSamples=0`
     - `passiveAvoidanceStopgapSamples=0`
     - `stuckCancelStopgapSamples=0`
     - `topSkipReasons=AdvanceConfirmed`
2. 随后我回到代码层，只收一个不漂的性能点：
   - [NavigationAgentRegistry.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs)
   - 把 `RegisteredUnits` 的 active 过滤从“每次调用都重新全表扫”改成：
     - `每帧最多重建一次 active cache`
     - `GetNearbySnapshots(...)` 和 `GetRegisteredUnits<T>(...)` 都复用这层 cache
     - 仍保留“同帧里 controller disable 后下次调用要能刷新”的语义
3. 为了防止 cache 把语义做坏，我补了新测试：
   - [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs)
   - 新增：
     - `NavigationAgentRegistry_ShouldRefreshCachedUnits_WhenControllerDisablesWithinSameFrame`
4. 在跑全类回归时，我顺手抓到一条旧语义噪音：
   - `TryFinishAutonomousRoamSoftArrival(...)` 会先写 `AutonomousSoftArrival:*`
   - 但 `FinishMoveCycle -> ResetMovementRecovery()` 又把它冲回 `AdvanceConfirmed`
   - 这会让 soft-arrival 的调试信号丢掉
5. 我做了最小修正：
   - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
   - 在 `FinishMoveCycle(...)` 之后把 `AutonomousSoftArrival:*` 写回，保留真实失败恢复语义，不改行为流
6. 代码/测试 fresh 结果：
   - direct `validate_script`
     - `NPCAutoRoamController.cs` => `errors=0 warnings=1`
     - `NavigationAgentRegistry.cs` => `errors=0 warnings=0`
     - `NavigationAvoidanceRulesTests.cs` => `errors=0 warnings=0`
   - `NavigationAvoidanceRulesTests` 全类回归：`33/33 passed`
7. 代码改完后，我又补了 3 次独立 free-time 短窗 live：
   - Run1：`avgFrameMs=0.75 / roamNpcCount=25 / maxBlockedAdvanceFrames=0 / maxConsecutivePathBuildFailures=0`
   - Run2：`avgFrameMs=0.78 / roamNpcCount=25 / maxBlockedAdvanceFrames=0 / maxConsecutivePathBuildFailures=0`
   - Run3：`avgFrameMs=0.81 / roamNpcCount=25 / maxBlockedAdvanceFrames=0 / maxConsecutivePathBuildFailures=0`
   - 三轮共同点仍然都是：
     - `maxBlockedNpcCount=0`
     - `blockedAdvanceStopgapSamples=0`
     - `passiveAvoidanceStopgapSamples=0`
     - `stuckCancelStopgapSamples=0`
     - `topSkipReasons=AdvanceConfirmed`
8. fresh Unity/console 收尾：
   - `python scripts/sunset_mcp.py status` => `Town.unity / Edit Mode`
   - `python scripts/sunset_mcp.py errors --count 20` => `errors=0 warnings=0`

### 关键判断
1. 当前导航 own 现在不只是“短窗能跑通”，而是：
   - `坏点/blocked/stuck` 继续稳定为 0
   - 同时我又收掉了一层真实的 registry 性能债
2. 这轮最值钱的地方不是“再绿一次”，而是：
   - 证明当前没有必要为了性能再去牺牲执行质量
   - 也没有必要在没有坏证据时继续盲改大面
3. 当前更准确的口径是：
   - `Package D / Package F` 的 live 信号已经非常强
   - 但执行清单里仍不能说“所有 package 都形式化完工”

### 验证结果
- direct `validate_script`
  - `NPCAutoRoamController.cs` => `errors=0 warnings=1`
  - `NavigationAgentRegistry.cs` => `errors=0 warnings=0`
  - `NavigationAvoidanceRulesTests.cs` => `errors=0 warnings=0`
- EditMode：
  - `NavigationAvoidanceRulesTests` => `33/33 passed`
- free-time live（代码改后 3 次独立 short-window）：
  - `avgFrameMs=0.75 / 0.78 / 0.81`
  - `maxBlockedAdvanceFrames=0`
  - `maxConsecutivePathBuildFailures=0`
  - `topSkipReasons=AdvanceConfirmed`
- fresh `status / errors`：
  - `Town.unity / Edit Mode`
  - `errors=0 warnings=0`

### 状态与恢复点
- 这轮现已合法再次 `Park-Slice`，当前 thread-state=`PARKED`。
- 如果继续下一轮，最合理顺序是：
  1. 先把这轮代码 + memory + own checkpoint 收口
  2. 再决定是继续做更重 targeted probe，还是停给用户做体感终验

## 会话 6 - 2026-04-16（导航健康继续施工：坏点合同 + near-target 自救 + static block 记忆）

**用户需求**：
> 用户要求我别再只说“局部好了”，而是继续把导航 own 真正往“健康导航”推进；这轮上下文已经明确：性能不能再炸，同时坏点、贴墙抽搐、同一小段路反复重试也要继续收。

**当前主线目标**：
- 在已经压下 `NPCAutoRoamController.Update()` 正常推进性能爆点之后，继续收导航 own 剩余两根：
  1. `坏点合同`
  2. `不会自救`

**这轮完成**：
1. 继续沿当前 `ACTIVE` slice 施工，没有回吞 Day1 phase，也没有重开新包。
2. 在 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 落了 3 个真正对准症状的修复：
   - autonomous roam 终点新增 `comfort clearance`，不再只靠 bare-footprint 勉强通过；
   - autonomous roam 邻域探测开始吃 body margin，不再忽略 NPC 自身体积；
   - `blocked advance` 近目标时先走 `soft arrival`，避免“已经够近还在原地抖”；
   - autonomous 在静态阻塞重规划前，先记住当前坏点，避免下一次又回挑同一个臭坑。
3. 在 [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs) 新增 3 条回归：
   - `NPCAutoRoamController_ShouldSoftArriveBlockedAdvance_WhenAutonomousFailureHappensNearDestination`
   - `NPCAutoRoamController_ShouldRememberStaticBlockedDestination_BeforeAutonomousRetarget`
   - `NPCAutoRoamController_ShouldRejectAutonomousRoamBadPoint_WhenGapOnlyFitsBareFootprint`
4. 跑了两层验证：
   - `NavigationAvoidanceRulesTests` 全类从 `42` 提升到 `43`，并 `43/43 passed`
   - Town live 6 秒 probe 复跑 2 次：
     - Run1：`roamNpcCount=16 / avgFrameMs=0.89 / maxFrameMs=272.38 / maxBlockedAdvanceFrames=0`
     - Run2：`roamNpcCount=16 / avgFrameMs=19.82 / maxFrameMs=348.60 / maxBlockedAdvanceFrames=0`
   - 两次共同点：
     - `maxBlockedNpcCount=0`
     - `maxConsecutivePathBuildFailures=0`
     - `blockedAdvanceStopgapSamples=0`
     - `stuckCancelStopgapSamples=0`
     - `topSkipReasons=AdvanceConfirmed`
5. fresh 收尾：
   - `python scripts/sunset_mcp.py errors --count 20` => `errors=0 warnings=0`
   - Unity 已退回 `Edit Mode`

**关键判断**：
- 这轮最关键的变化不是“又补了几个参数”，而是导航 own 终于开始把“不要选刚好能塞进去的臭点”和“够近就别死抖”写成硬合同。
- 当前最有价值的结论是：
  - `Town` 现在没有重新炸回旧的性能深坑；
  - 同时坏点 acceptance 和 static-block retarget 也收紧了一层。

**验证结果**：
- `结构 / checkpoint`：成立
- `targeted probe / 局部验证`：成立
- `真实入口体验`：仍待用户最终体感

**遗留问题 / 恢复点**：
- 当前不能直接宣称“导航彻底完工”；
- 但下一轮最该做的，已经不再是继续盲磨性能，而是：
  1. 如果用户还有 fresh 坏样本，就按样本继续打 residual
  2. 如果没有新的坏证据，就停给用户做 true runtime 体感验收
## 2026-04-16 切 Town 启动风暴根因复盘（只读分析）
- 用户主线：查清“primary 完成后进入 Town、晚饭前转场黑屏久且卡爆”到底是不是导航 own，为什么前几轮一直没有真正解决。
- 本轮子任务：把用户这批 profiler 图结论与当前 `Day1 / NPCAutoRoamController` 代码重新对位，不直接施工。
- 已完成：
  - 确认这批样本不是之前压过的“free-time 稳态热点”，而是 `primary -> Town` 切场瞬间的启动风暴。
  - 确认热点主烧仍在 `NPCAutoRoamController.Update()`，不是 `Day1 Update` 本体；但 `Day1` 仍是触发器。
  - 确认 `SpringDay1NpcCrowdDirector` 在 Town 白天窗口会主动把 resident 重新放回 autonomy：
    - `ShouldYieldDaytimeResidentsToAutonomy()` / `YieldDaytimeResidentsToAutonomy()`
    - `ReleaseResidentToAutonomousRoam(...)`
    - `ReleasedFromDirectorCue && NeedsResidentReset` 分支仍会落到 release/resume roam
  - 确认导航 own 的高成本链主要有两段：
    - 启动段：`StartRoam/ResumeAutonomousRoam -> TryBeginMove()`，同帧做采样、候选筛选、建路、路径 acceptance
    - 移动段：`TickMoving() -> AdjustDirectionByStaticColliders() -> ProbeStaticObstacleHits()`，坏状态下持续放大 physics probe
  - 确认当前限流仍偏“单 NPC 本地限流”，不是“切场 herd 全局限流”：
    - `TryAcquirePathBuildBudget()` 只限制单 NPC 同帧重复建路
    - 不能阻止 Town 切场时多名 resident 同时进入重建路/重采样
  - 确认 `NavigationAgentRegistry` 已替掉旧的全场 `FindObjectsByType`，但当前仍会在每个 controller 侧做 per-frame buffer 获取和后续全表扫描；比旧实现好，但 herd-start 仍会一起放大。
- 关键判断：
  - 这次不是“老冻结原样回来”，也不是“Day1 Update 自己烧满 CPU”。
  - 更像 `Day1/切场释放事件` 在 Town 入场瞬间把部分 resident 推回 autonomous roam，随后导航在同一时间窗里把 `TryBeginMove + 静态 steering probe + blocked recovery` 整套高成本链顶满。
  - 之前几轮没真正收掉，是因为前面主要收的是“自由活动稳态窗口”的热点，不是“切 Town 首几帧”的 herd-start 热点。
  - 用户说的三根仍成立，而且这次切场样本里三根是叠在一起的：
    - 坏点还会被选中
    - 静态导航 probe 仍然重
    - 卡住自救仍然太晚、太贵
- 对 Day1 边界的结论：
  - `Day1` 有触发责任，但这批图上主烧的执行成本仍是导航 own。
  - 当前不能把锅全甩给 Day1，也不能把 Day1 摘干净。
- 下一步恢复点：
  - 如果继续施工，导航唯一优先刀应从“切场启动风暴”下手，而不是继续只磨 free-time 稳态参数。
  - 方向应是：Town 入场/释放后的首帧短窗降载、坏启动输入 cheap-fail、以及对 `102/103` 这类单坏 resident 放大器做定点拦截。
## 2026-04-16 只读分析：`primary -> Town` 最快复现与取证链
- 当前主线：在不改代码的前提下，找出现有工程里最快、最可靠的 `primary -> Town / scene-entry / Town 切场卡顿` 复现与取证链。
- 本轮子任务：检查 `Assets/Editor/Story/*`、`Assets/Editor/Town/*`、`Assets/YYY_Scripts/Service/Navigation/*` 的菜单、artifact 落点与 probe 覆盖面。
- 已完成：
  - 确认 `Town` 侧 3 个菜单本质上都是编辑态静态契约探针：
    - `TownSceneEntryContractMenu`
    - `TownSceneRuntimeAnchorReadinessMenu`
    - `TownScenePlayerFacingContractMenu`
  - 确认它们统一把结果写到 `Library/CodexEditorCommands/*.json`，适合做“进场前静态排雷”，不适合直接复现切场卡顿。
  - 确认最贴近真实 `primary -> Town` live 链的，是 `Story` 侧这组菜单：
    - `SpringDay1LiveSnapshotArtifactMenu`
    - `SpringDay1ActorRuntimeProbeMenu`
    - `SpringDay1ResidentControlProbeMenu`
  - 确认 `SpringDay1NativeFreshRestartMenu` 只能快速重开到 Town 原生开局，适合复现 Town 开局，不等于真实 `Primary -> Town` 转场。
  - 确认 `NavigationLiveValidationMenu/Runner` 主要服务玩家导航案例，产物以 Console `[NavValidation]` 日志为主，缺少专门针对 `Town` scene-entry herd-start 的文件级 artifact。
- 关键判断：
  - 现在最适合的真实复现链，不是 `Tools/Sunset/Navigation/*`，而是：
    1. 用 `Town` 静态 probe 先排结构断点
    2. 进入 `Primary` Play 现场
    3. 用 `Request Spring Day1 Town Lead Transition Artifact` 真触发 Town 转场
    4. 入 Town 后立刻写 `Actor Runtime Probe`
    5. 需要 owner 分层时再补 `Resident Control Probe`
  - 对焦导航 own 最强的现成 probe 是 `SpringDay1ActorRuntimeProbeMenu`，因为它直接写出 `NPCAutoRoamController` 的 `DebugState / lastMoveSkipReason / blockedAdvanceFrames / consecutivePathBuildFailures / sharedAvoidanceBlockingFrames`。
  - 最容易混进 Day1 scene-entry 一次性成本的是：
    - `SpringDay1LiveSnapshotArtifactMenu`，因为它本身就围绕 phase/beat/transition/bootstrap
    - `SpringDay1ResidentControlProbeMenu`，因为它直接读 `SpringDay1NpcCrowdDirector` 的 resident control/runtime summary
- 恢复点：
  - 后续若要做 live 取证，应优先沿用这条链，而不是再拿玩家导航 live validation 代替 Town 转场现场。
## 2026-04-16 导航继续施工：herd-start 放行口前移到 pause 层
- 用户主线：继续把导航 own 往“健康导航”推进；重点仍是 `primary -> Town / scene-entry / batch release` 这类短窗 herd-start，不回吞 Day1 owner。
- 本轮子任务：
  1. 吸收 Day1 对导航方案“方向对但包太大”的审核
  2. 用 fresh live 和静态 probe 重验 Town 现场
  3. 只补一刀导航 own：把 herd-level 启动闸口从 `TryBeginMove()` 前移到 pause 层
- 本轮实际完成：
  1. 重新对位 Day1 审核后，确认当前现码里已经存在一层 herd-start 治理：
     - `AUTONOMOUS_ROAM_STARTUP_WINDOW_SECONDS`
     - `TryAcquireAutonomousStartupPathBuildBudget()`
     - `TryAcquireAutonomousStaticProbeBudget()`
     - `HasAutonomousRoamStartupDestinationClearance(...)`
     但闸口仍偏后，主要卡在 `TryBeginMove()` 内部。
  2. 继续在 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 落下更窄的一刀：
     - 新增 `AUTONOMOUS_ROAM_STARTUP_GLOBAL_RESUME_PERMIT_BUDGET=4`
     - 新增 `s_autonomousStartupResumePermitFrame / s_autonomousStartupResumePermitsThisFrame`
     - 新增 `TryAcquireAutonomousResumePermit()`
     - 新增 `TryDeferAutonomousResumeFromPauseState()`
     - `TickShortPause()` / `TickLongPause()` 现在会先过 herd-level resume permit，再决定是否真正进入 `TryBeginMove()`
     - 超预算时不再让更多 NPC 同帧一起挤进完整取样/建路链，而是先记 `AutonomousResumePermitDeferred` 并走 fast-retry short pause
  3. 在 [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs) 补了 2 条回归：
     - `NPCAutoRoamController_ShouldDeferAutonomousResumeFromShortPause_WhenResumePermitBudgetIsExhausted`
     - `NPCAutoRoamController_ShouldDeferAutonomousResumeFromLongPause_WhenResumePermitBudgetIsExhausted`
  4. fresh live / 静态证据：
     - `Town + EnterVillage_PostEntry + resident control probe` 证实当前确实处在 crowd beat，`101~203` 仍由 `SpringDay1NpcCrowdDirector` 持有
     - 同窗口 `npc-roam-spike-stopgap-probe.json` 结果已是绿样本：
       - `avgFrameMs=0.54`
       - `maxFrameMs=13.23`
       - `maxGcAllocBytes=21247`
       - `maxBlockedAdvanceFrames=0`
       - `topSkipReasons=AdvanceConfirmed`
     - `Force FreeTime` 强跳样本被我判定为污染样本：
       - `phase=FreeTime`
       - 但 `101~203` 仍持续被 crowd director 持有
       - 不适合拿来判断导航 own 是否过线
     - 编辑态 Town probe：
       - `town-entry-contract-probe.json` => `completed`
       - `town-runtime-anchor-readiness-probe.json` => `completed`
       - `town-player-facing-contract-probe.json` => `attention`
         - attention 原因：`EnterVillageCrowdRoot` 不在玩家进 Town 第一屏里
         - 这不是导航 own，而是 player-facing / scene-side 入口呈现问题
- fresh 验证结果：
  - `git diff --check`（仅这轮 2 个文件）通过
  - `manage_script validate`
    - `NPCAutoRoamController.cs` => `errors=0 warnings=1`
    - `NavigationAvoidanceRulesTests.cs` => `errors=0 warnings=0`
  - 但 Unity fresh console 当前存在 external red：
    - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs(2053) CS0120`
    - 这不是导航 own，却会阻断我对“最新这刀是否已在 Unity 内编进去”的 fresh compile / live 终证
- 当前判断：
  - 导航 own 现在不该再被表述成“整套没治理”，因为：
    - Town 开局 `EnterVillage` 现场已经是绿样本
    - herd-start 预算已存在
    - 这轮又把放行口前移到 `TickShortPause()/TickLongPause()`
  - 当前更准确的口径是：
    - `导航 own 的 scene-entry herd-start 刀又收硬了一层`
    - `Town scene-side 结构基本站住`
    - `最新导航补丁的 live 终证被外部 compile red 卡住`
- 当前恢复点：
  1. 当前 thread-state 已合法 `PARKED`
  2. 后续若 external red 清掉，第一件事不是再改代码，而是：
     - fresh compile
     - 重跑 `Town + EnterVillage_PostEntry` spike probe
     - 如果能切到 `Primary -> Town` 真转场，再按 `TownLeadTransitionArtifact -> ActorRuntimeProbe` 复抓一轮
  3. 若 fresh live 仍坏，再继续考虑 residual；若 fresh live 仍绿，就不该继续盲改导航大面
## 2026-04-16 导航继续施工：autonomous 坏区记忆落地
- 用户主线：继续只收导航 own，不回吞 Day1；这轮目标是把“知道这里坏了却还在隔壁坏点继续选”的问题收成代码。
- 本轮子任务：
  1. 先复抓 `Town + EnterVillage_PostEntry`
  2. 再只补一刀 `autonomous bad-zone memory`
  3. 最后用 `Force FreeTime` live 验证这刀没有把漫游重新炸坏
- 本轮实际完成：
  1. fresh `Town + EnterVillage_PostEntry` 先拿到一条异常 spike 样本：
     - `avgFrameMs=98.30 / sampleCount=62`
     - 但同链立刻复抓一轮变回：
       - `avgFrameMs=0.68 / maxFrameMs=12.63 / maxBlockedAdvanceFrames=0`
     - 同时 actor / resident probe 都证实：
       - crowd resident 仍被 `SpringDay1NpcCrowdDirector` 持有
       - 这批 resident 当时没有进入 autonomous roam
     - 所以这条 `98ms` 我没有拿来直接改方向，而是判成“异常样本，需继续复抓”。
  2. 继续在 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 落下一刀窄修复：
     - 保留原有 `lastStaticBlockedDestination` 点记忆
     - 新增局部 `bad-zone` 记忆：`lastStaticBlockedZoneCenter / lastStaticBlockedZoneRadius`
     - `RememberStaticBlockedDestination(...)` 不再只记单点，而会把 `currentPosition -> destination` 之间的坏区一起记住
     - `IsRecentlyBlockedAutonomousDestination(...)` 现在不仅拒收原坏点，也会拒收坏区中心附近的候选
     - `EvaluateAutonomousRoamDestinationScore(...)` 现在会给坏区边缘候选降分，不是非黑即白地只看最后一个点
  3. 在 [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs) 新增 2 条回归：
     - `NPCAutoRoamController_ShouldRejectAutonomousBadZoneCandidate_AfterStaticBlockedDestination`
     - `NPCAutoRoamController_ShouldPenalizeAutonomousBadZoneCandidate_AfterStaticBlockedDestination`
  4. 代码层验证：
     - `manage_script validate NPCAutoRoamController` => `errors=0 warnings=1`
       - 唯一 warning 仍是旧的 `Update()` 字符串拼接 GC 提示
     - `manage_script validate NavigationAvoidanceRulesTests` => `errors=0 warnings=0`
     - `git diff --check -- NPCAutoRoamController.cs NavigationAvoidanceRulesTests.cs` => 通过
  5. live 验证：
     - `Force Spring Day1 FreeTime Validation Jump`
     - `Write Spring Day1 Actor Runtime Probe Artifact`
     - `Run Roam Spike Stopgap Probe`
     - fresh 结果：
       - `phase=FreeTime / beat=FreeTime_NightWitness`
       - `101/102/103/104/201/202` 已进入 `autonomous`
       - `roamControllerEnabled=true`
       - `isRoaming=true`
       - `roamState=ShortPause`
       - `blockedAdvanceFrames=0`
       - `consecutivePathBuildFailures=0`
       - `topSkipReasons=AdvanceConfirmed`
       - spike probe：`roamNpcCount=24 / avgFrameMs=0.77 / maxFrameMs=16.66 / maxBlockedAdvanceFrames=0 / maxConsecutivePathBuildFailures=0`
  6. fresh console 收尾：
     - `python scripts/sunset_mcp.py errors --count 20 --output-limit 5`
     - 结果 `errors=0 warnings=0`
- 当前判断：
  1. 这轮最值钱的不是“又补一个 detour”，而是把坏点记忆从单点升级成了坏区。
  2. 它直接对准用户一直骂的那种体感：
     - 不要在同一个坏坑周围轮流选点
     - 不要卡在贴墙/贴围栏附近一直短段重试
  3. 当前 fresh free-time live 没有显示出新的 `blocked/stuck/pathBuildFailure` 回潮，所以这刀至少没有把导航健康重新打坏。
- 当前还没做的：
  1. 这轮没有在当前 Unity 会话里把 `NavigationAvoidanceRulesTests` 真正通过 `TestRunnerApi` 跑起来。
     - 现有工程没有给这组测试现成菜单入口
     - 我这轮没有为了跑 2 条回归再额外扩一个新 editor menu
     - 当前采取的是：`manage_script validate + git diff --check + live probe`
  2. 这轮仍没有拿到 `Primary -> Town` 真转场样本，因为当前还没有现成菜单把活动场景切到 `Primary`
- 恢复点：
  1. 当前 thread-state 已合法 `PARKED`
  2. 若后续继续，导航 own 更值得优先看的 residual 已经不是 herd-start，而是：
     - 坏区之外的几何死角
     - 4 向邻域探针不足的凹角/窄口
     - static recovery 虽会自救但跳出距离仍不够大的场景
## 2026-04-16 导航V3 warm live 收口
- 当前主线：
  - 继续做导航 own 的 live 收口与复核，不再新增代码面。
- 本轮实际做成：
  1. Town free-time `Run Roam Spike Stopgap Probe` 连跑 3 轮。
     - 第 1 轮（cold / warm-up）：`avgFrameMs=1.31 / maxFrameMs=120.53 / maxGcAllocBytes=7210631`
     - 第 2 轮（warm）：`avgFrameMs=0.923 / maxFrameMs=3.44 / maxGcAllocBytes=115566`
     - 第 3 轮（warm）：`avgFrameMs=0.824 / maxFrameMs=12.92 / maxGcAllocBytes=522473`
     - 三轮共同点：
       - `maxBlockedNpcCount=0`
       - `maxBlockedAdvanceFrames=0`
       - `maxConsecutivePathBuildFailures=0`
       - `blockedAdvanceStopgapSamples=0`
       - `passiveAvoidanceStopgapSamples=0`
       - `stuckCancelStopgapSamples=0`
       - `topSkipReasons=AdvanceConfirmed`
  2. 重新跑 `Tools/Sunset/Navigation/Run Avoidance Rules Tests`，结果 `3/3 passed`。
  3. 直接 Unity `validate_script` 复核：
     - `NPCAutoRoamController.cs` => `errors=0 / warnings=1`
     - `NavigationAgentRegistry.cs` => `errors=0 / warnings=0`
     - `NavigationAvoidanceRulesTests.cs` => `errors=0 / warnings=0`
  4. CLI `errors` => `0 / 0`
     - `status` 显示 baseline pass、bridge active、Unity 已回到 `Edit Mode`
- 关键判断：
  1. 第一轮 120ms / 7.2MB GC 尖峰更像 warm-up 噪音，不像稳定回潮。
  2. 后两轮已经回到低位，blocked/stuck 继续全零，说明当前导航 own 的 free-time live 健康态是 repeatably stable。
  3. 这轮不再需要继续盲改行为；下一步更适合做更长 soak 或用户体感终验。
- 当前恢复点：
  - 保持 `PARKED`。
  - 若用户再贴 fresh 坏样本，再按样本继续打 residual；否则优先进入验收而不是扩写。
## 2026-04-16｜真实施工：static near-wall hard-stop + replan 已落地

### 用户目标
- 用户要求我不要再停在方向判断，直接把我认定最准确的一刀落进代码。
- 本轮子任务是：只收 `static near-wall hard-stop + replan`，不扩回 Day1 phase，也不重做整套导航。

### 已完成事项
1. 真实进场前尝试 `Begin-Slice`，但脚本报实 `导航V3` 现场仍是 `ACTIVE`，所以这轮沿旧 slice 继续施工，没有重复开新切片。
2. 在 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 落下这一刀：
   - 新增 `AUTONOMOUS_STATIC_STEERING_SOFT_MAX_ANGLE = 22f`
   - 新增 `AUTONOMOUS_STATIC_NEAR_WALL_HARD_STOP_FRONTAL_PRESSURE = 0.22f`
   - 新增 `AUTONOMOUS_STATIC_NEAR_WALL_HARD_STOP_MAX_ANGLE = 24f`
   - 新增 `AUTONOMOUS_STATIC_NEAR_WALL_HARD_STOP_CONFIRM_FRAMES = 2`
   - `TickMoving()` 现在在静态 steering 后会走 `ShouldTriggerStaticNearWallHardStop()`
   - 满足条件时不再继续喂 velocity，而是：
     - `motionController.StopMotion()`
     - `TryHandleBlockedAdvance(..., "StaticNearWallHardStop")`
   - 新 reason 已并入 static blocked recovery / abort / bad-zone remember 链
3. 这刀没有动：
   - `NPCMotionController.cs`
   - `Day1` 任何文件
4. 在 [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs) 收测试：
   - 旧的 `ShouldBiasSideways_WhenStaticObstacleSitsDirectlyAhead()` 改成：
     - `ShouldTriggerStaticNearWallHardStop_WhenObstacleSitsDirectlyAhead()`
   - 新增：
     - `ShouldKeepSoftSteering_WhenStaticObstacleIsOffsetFromPath()`

### 关键决策
1. 这轮没有继续动 roam 选点硬筛，因为现码里那部分已经不是零。
2. 我把主刀压回了 movement execution：
   - 正前方高压静态障碍 => 停
   - 侧偏轻压静态障碍 => 允许小角度软绕
3. 这正对应用户最在意的“墙边疯狂转向”坏相，而不是继续做性能止血或 Day1 owner 分析。

### 验证结果
- `validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
  - `assessment=no_red`
  - `owned_errors=0`
  - `external_errors=0`
- `validate_script Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs --count 20 --output-limit 5`
  - `assessment=no_red`
  - `owned_errors=0`
  - `external_errors=0`
- `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs`
  - 通过
- targeted EditMode tests：
  - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldTriggerStaticNearWallHardStop_WhenObstacleSitsDirectlyAhead`
  - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldKeepSoftSteering_WhenStaticObstacleIsOffsetFromPath`
  - 结果 `2/2 passed`
- 现成菜单 `Tools/Sunset/Navigation/Run Avoidance Rules Tests`
  - 结果文件 `TestResults.xml` 显示 `3/3 passed`
- fresh console：
  - `errors=0 warnings=0`
  - 仅残留 Unity TestRunner 保存结果文本，不算 blocking red
- `Park-Slice`
  - 已执行
  - 当前 thread-state=`PARKED`

### 当前判断
1. 这刀已经真实落地，而且不是半截。
2. 当前代码层和 targeted test 层都站住了。
3. 但这轮还没有 fresh live 重新抓玩家体感，所以当前只能报：
   - `结构 / checkpoint`：成立
   - `targeted probe / 局部验证`：成立
   - `真实入口体验`：待重新验证

### 当前恢复点
1. 如果继续下一轮，优先做 fresh live，看墙边疯狂转向是否明显消失。
2. 若仍有 residual，再看是否需要补 `NPCMotionController` 的 replan 窗口显式朝向冻结；当前这轮先不扩。
## 2026-04-16｜重新锚定：最准确的一刀已压到“静态近壁硬停重规划”

### 用户目标
- 用户明确要求我不要再给泛方向，而要基于现码重新锚定导航 own 的唯一主刀、精确阈值和落地路径。
- 本轮子任务是只读分析，不进入真实施工；已在聊天里报实本轮不跑 `Begin-Slice`。

### 已完成事项
1. 完整回读并对位：
   - `Day1-V3` 弱引导 prompt
   - `Day1-V3` 总表
   - `Day1-V3` 给导航的边界 contract
2. 重新核对当前导航现码：
   - `NPCAutoRoamController.cs`
   - `NPCMotionController.cs`
   - `NavigationAgentRegistry.cs`
   - `NavigationAvoidanceRulesTests.cs`
3. 重新确认当前 roam 选点现实：
   - autonomous candidate 不是裸奔，已经有 `occupiable / neighborhood / body / comfort / agent / bad-zone` 六层硬筛
   - 但 moving 阶段仍保留 `AdjustDirectionByStaticColliders(...)` 的大角度软 steer
4. 重新确认当前最刺眼坏相与代码完全对位：
   - `AUTONOMOUS_STATIC_STEERING_MAX_ANGLE = 58f`
   - `frontalPressure > 0.2f` 时会额外加侧绕偏置
   - 这正对应用户肉眼看到的“快撞墙/撞墙时疯狂转向、疯狂改朝向”
5. 重新核对代表性 NPC prefab 身体尺寸：
   - `001/101/203` 的 `BoxCollider2D size = 0.88 x 0.68`
   - 对应当前人体 NPC `GetColliderRadius() ≈ 0.44`

### 关键决策
1. 当前最准确的结论不是“roam 选点完全没硬筛”。
2. 当前最准确的结论是：
   - 目标点 acceptance 已经有硬筛
   - 但行进阶段仍在靠大角度软 steering 救火
   - 连测试也还在保护这条旧行为
3. 因此下一刀不该继续泛砍 `坏点 / static / 自救` 三包，也不该改成“全场静态碰撞体总表扫描”。
4. 唯一最值钱的一刀应改成：
   - `静态近壁时不再继续软拧方向`
   - `直接硬停 + 立即重规划`
   - `并在重规划窗口冻结朝向`

### 现码阈值锚点
1. 人体 NPC 当前尺寸推导：
   - `collider radius ≈ 0.44`
   - `contact shell = 0.05`
   - `autonomous body extra ≈ 0.167`
   - `destination comfort extra ≈ 0.259`
2. 因此当前“静态舒适圈”的总量级大约已经在：
   - `0.44 + 0.259 + 0.05 ≈ 0.75`
3. 当前 static steering 的探测半径实际也在这个量级：
   - `clearanceRadius ≈ 0.737`
4. 所以下一刀不该拍脑袋写 `3`，而该继续锚在 NPC 自身体积推出来的 `0.72 ~ 0.78` 局部硬阈值。

### 当前判断
1. 当前唯一主刀：
   - `把静态障碍处理从软 steer 主导，改成硬停重规划主导`
2. 当前最该动的文件：
   - `NPCAutoRoamController.cs`
   - 次刀才可能碰 `NPCMotionController.cs`
3. 当前最该改的测试不是 avoidance solver，而是：
   - `NavigationAvoidanceRulesTests.cs` 里那条仍在保护“正前方静态障碍要侧偏”的测试

### 当前恢复点
1. 如果继续真实施工，先只开这一刀，不扩面。
2. 落地顺序应固定为：
   - 收紧 static near-wall hard-stop 触发
   - 改成 stop + replan
   - 给 replan 窗口补朝向冻结
   - 重写/替换保护旧 soft-steer 行为的测试
## 2026-04-16｜导航健康第一刀落地：主到达合同 + 中远距选点 + 恢复停顿窗

### 用户目标
- 用户这轮明确要求不要再停在“局部 steering 调参”，而要把导航修成更简单、看起来更正常、又不炸性能的基础漫游机制。
- 当前主线仍然是导航 own，不回吞 Day1 phase；Day1 语义只作为边界参考。

### 本轮实际做成
1. 在 `NPCAutoRoamController.cs` 把“身体进终点壳层就算到”升成主到达合同：
   - `TickMoving()` 不再只认 `destinationTolerance=0.12`
   - autonomous roam 与 navigation-own 的 `AutonomousDirected/FormalNavigation` 现在走 body-shell arrival
   - `ResidentScripted` 仍不吃这条宽松到达，避免伤 Day1 staged contract
2. 把 autonomous roam 选点改成“优先中远距离，失败再放宽”：
   - 新增 preferred minimum travel distance
   - 当前 crowd NPC 默认量级从原本 `0.6` 的近点偏好，提升到约 `1.5+` 的中远点偏好
   - 先采中远点；实在没有，再回退到旧下限，不把自己锁死
3. 给 autonomous roam 补了恢复停顿窗：
   - 静态近壁 hard-stop 或重复 blocked 后，不再同帧/短帧立即反向重试
   - 现在会先进入 `ShortPause` 型 recovery hold，大约 `0.42s`，停一下、稳住朝向、再重新取样
   - passive avoidance 的硬阻塞也接进了同类 hold，减少互卡后马上乱翻方向
4. 把 `MOVE_COMMAND_NO_PROGRESS_GRACE_SECONDS` 从 `0.06` 放宽到 `0.12`，降低“刚发出去就判没动”的过早误判
5. 在 `NavigationAvoidanceRulesTests.cs` 补和改了这轮新合同测试：
   - autonomous 主到达合同
   - formal navigation 也可吃 body-shell arrival
   - resident scripted 不可误吃 body-shell arrival
   - autonomous 中远距选点偏好
   - static blocked 后进入 recovery hold
   - 同时保留并复跑旧的 soft-arrival / bad-zone / gap / static recovery 护栏

### 验证结果
1. 代码层：
   - `git diff --check` 通过
   - `validate_script NPCAutoRoamController.cs` => `assessment=no_red`
   - `validate_script NavigationAvoidanceRulesTests.cs` => `assessment=no_red`
2. targeted EditMode tests：
   - 新合同首批 `8/8 passed`
   - 旧护栏回归 `8/8 passed`
3. live：
   - Town 场景进入 PlayMode
   - `Force Spring Day1 FreeTime Validation Jump`
   - `Write Spring Day1 Actor Runtime Probe Artifact`
   - `Run Roam Spike Stopgap Probe`
   - 结果：
     - `phase=FreeTime`
     - `beat=FreeTime_NightWitness`
     - `clock=07:30 PM`
     - `roamNpcCount=24`
     - `avgFrameMs=0.666`
     - `maxFrameMs=14.50`
     - `maxGcAllocBytes=156238`
     - `maxBlockedNpcCount=0`
     - `maxBlockedAdvanceFrames=0`
     - `maxConsecutivePathBuildFailures=0`
     - `topSkipReasons=AdvanceConfirmed`
   - fresh console：`errors=0 warnings=0`
4. 收尾：
   - 已退出 PlayMode
   - 当前 Unity 回到 `Edit Mode`
   - 当前 thread-state=`PARKED`

### 当前判断
1. 这轮真正改掉的，不是某个绕路角度，而是 3 套基础合同：
   - 到达合同
   - 选点合同
   - 脱困合同
2. 现在最关键的改善是：
   - “已经靠近目的地但一直不算到”被主合同直接放宽了
   - “原地挑近点小碎步”被中远距优先压住了
   - “卡住后立刻反向乱试”被 recovery hold 切断了
3. 这轮 live 没再看到 blocked/pathBuildFailure/spike 复发，所以这刀至少没有拿性能换行为
4. `203` 在这次 free-time probe 里仍被 `SpringDay1NpcCrowdDirector` staging 持有；这条不是导航 own 的新问题，而是 Day1 现状边界

### 还没做完的
1. 这轮还没有拿到“用户肉眼最终体感已过线”的结论
2. 这轮 live 探针证明了：
   - 没有 blocked/stuck/path build 回潮
   - free-time 运行态健康
   但它不能替代你对“看起来是否自然”的终判
3. 若后续还有 residual，最可能剩在：
   - 凹角/窄口的局部几何死角
   - 个别 shared avoidance 的视觉朝向细节
   - 某些 Day1 特殊 staging 个体继续不走这条合同

### 恢复点
1. 下一轮若继续导航 own，优先看 fresh 坏样本是否还集中在：
   - 凹角/窄口
   - shared avoidance 互卡视觉
2. 若用户实测主坏相已经大幅缓解，导航 own 更该停给用户验体验，而不是继续盲修
## 2026-04-16｜用户“时间预算/避让超时/近终点直接算到”思路对位补记

### 用户新增思路
- 用户提出：NPC 速度与目标距离已知，可以引入“预期到达时间 + 避让暂停计时 + 超时换点 + 近终点久拖则直接算到”的更简单漫游语义。

### 我这轮重新对位后的判断
1. 我认可这个思路的方向，但不认同把它当导航主合同重写。
2. 当前最该保留为主合同的，仍然是已经落地的 3 件事：
   - `body-shell arrival`
   - `中远距优先选点`
   - `blocked 后 recovery hold，再重取样`
3. 用户提出的“时间预算”更适合作为二级护栏，只在 residual 场景里兜底：
   - 长时间避让未恢复
   - 近终点低位移久拖
   - recovery hold 后仍持续卡在同一局部几何
4. 不适合把它升成一级主合同，原因是：
   - autonomous roam 不是恒速直线任务，中间天然包含 short pause / avoidance / detour / rebuild
   - 如果把 ETA 直接做成主判据，容易把正常停让误打成失败
   - 当前更值钱的是“先把坏样本挡在宽松到达与 cheap-fail 里”，不是再造一套全局计时状态机

### 当前 roam 选点现实
1. autonomous roam 现在不是“随便找个点先走再说”。
2. 当前真实入口已经有这些硬筛与打分：
   - `CanOccupyNavigationPoint`
   - `NeighborhoodClearance`
   - `BodyClearance`
   - `ComfortClearance`
   - `AgentClearance`
   - `RecentlyBlockedBadZone`
   - startup 短窗里的额外 clearance
   - `open-neighbor / open-corner` score
3. 因此“坏点完全没管”已经不是最准确描述；更准确的说法是：
   - 选点层已有硬筛
   - 残留风险更多落在局部凹角/窄口几何与 shared avoidance 视觉细节

### 对用户阈值想法的当前裁定
1. “周围 3 个单位不能有碰撞体”我不采纳。
2. 原因不是反对保守，而是它会远超 NPC 身体真实量级，直接把大部分 Town 漫游点都判成非法。
3. 现码当前真实身体量级仍大约是：
   - `collider radius ≈ 0.44`
   - `舒适圈总半径 ≈ 0.75`
4. 所以导航 own 当前仍应坚持 `body-driven` 阈值，而不是跳到固定 `3` 这种与身体尺寸脱节的全场硬规则。

### 当前阶段判断
1. 当前已站住：
   - `结构 / checkpoint`
   - `targeted probe / 局部验证`
   - `真实入口 live 健康`
2. 当前尚未站住：
   - `最终视觉体验`
3. 当前最合理的下一步不是再重写主合同，而是：
   - 等用户 fresh 坏样本
   - 若确有 residual，再把“时间预算型超时护栏”只补到近终点久拖或长时间避让失败这一层

### 当前状态
- 本轮未改业务代码
- 本轮未重新开 slice
- 当前 thread-state 继续保持 `PARKED`
## 2026-04-16｜性能排查补记：steady-state 导航当前不重，主风险转向 scene-entry 与全场探测链

### 用户目标
- 用户反馈“现在贼卡”，要求我先把性能大头和“不能过度优化”的位置完整排查，再给出排序。

### 本轮实际做了什么
1. 只读排查代码热点：
   - `NPCAutoRoamController.cs`
   - `SpringDay1NpcCrowdDirector.cs`
   - `SpringDay1Director.cs`
   - `NavigationAgentRegistry.cs`
2. fresh live：
   - 进入 PlayMode
   - 跳到 `FreeTime`
   - 写 actor runtime probe
   - 跑 `Run Roam Spike Stopgap Probe`
   - 读 fresh console
   - 退回 Edit Mode

### fresh live 结果
1. roam spike probe：
   - `roamNpcCount=18`
   - `avgFrameMs=0.713`
   - `maxFrameMs=16.29`
   - `maxGcAllocBytes=61313`
   - `maxBlockedNpcCount=0`
   - `maxBlockedAdvanceFrames=0`
   - `maxConsecutivePathBuildFailures=0`
2. actor probe：
   - 当前 `101~203` 这批 resident 并没有真正进入 autonomous roam
   - 都还在 `SpringDay1NpcCrowdDirector` 的 staging 持有链里
3. fresh console：
   - `errors=0 warnings=0`

### 当前判断
1. 这次 fresh live 说明：`steady-state` 导航循环在当前这个 probe 里不是主峰。
2. 但这不等于“导航没有性能风险”，因为这次 probe 里 resident 自治负载并没有 fully 打开。
3. 当前最该盯的性能大头，按风险更像是两层：
   - `Town / Day1` 入口与补绑短窗的一次性全场搜索
   - `NPCAutoRoamController` 自治时的候选点筛选、路径构建、静态探测、全体 agent 扫描
4. 因此当前不能再把“贼卡”只说成导航 steady-state，也不能把导航完全摘干净。

### 当前恢复点
1. 若继续性能修复，优先顺序应是：
   - 先砍 `scene-entry / crowd sync / resident recover` 的一次性搜索峰
   - 再砍 NPC 自治时的 `agent-clearance / static-probe / path-build` 热链
2. 当前 thread-state 继续保持 `PARKED`
## 2026-04-16｜只读审计补记：导航 own 剩余短窗 CPU 热点已收窄到少数重链

### 用户目标
- 只审 `NPCAutoRoamController.cs`，回答“在现有 startup budget、agent registry、static hard stop、arrival shell、bad zone 记忆、Day1 queue/resume herd-start 节流都已存在后，导航 own 里还剩哪些最可能造成 Day1/Town 短窗卡顿的 CPU 热点”。

### 本轮实际做了什么
1. 只读回查 `TryBeginMove`、autonomous destination acceptance、static probe、shared avoidance、ambient chat、path build budget 相关入口。
2. 把“还值得继续砍的点”压成导航 own 内部排序，不回吞 Day1 phase/beat 语义。

### 关键判断
1. 当前最可疑的导航 own 主峰已经不是旧的 `FindObjectsByType`，而是 `TryBeginMove` 内的整条重链：
   - candidate sampling
   - safe center search
   - destination acceptance
   - candidate score
   - path build
   - built-path accept
2. `static probe / overlap` 仍值得继续盯，因为 moving 态靠墙、挤墙、短窗多人并发时会重复触发。
3. `shared avoidance nearby snapshots` 仍可能贵，但优先级低于 candidate pipeline 与 static probe。
4. `ambient chat partner` 目前只算次级热点，不是下一刀最值位置。
5. `path build budget` 已经部分成立，但它没有削掉 build 之前那串昂贵 acceptance。

### 当前恢复点
1. 若导航V3 只能再收一刀，最值的是继续收窄 `TryBeginMove` 的 candidate sampling / acceptance pipeline，让坏样本更早 cheap-fail。
2. 本轮只读，无代码改动，thread-state 继续保持 `PARKED`。
## 2026-04-16｜导航V3 施工补记：Day1/Town herd-start 节流与 candidate pipeline 早停已落地

### 用户目标
- 真正优化 Day1/Town 的 NPC 短窗卡顿与拥挤，不靠冻结 resident、砍 roam、破坏 Day1 staged contract 来换性能。

### 本轮实际落地
1. `SpringDay1NpcCrowdDirector.cs`
   - `QueueResidentAutonomousRoamResume` / `GetQueuedAutonomousResumeDelay` 收成静态 helper，清掉本轮 own red。
   - resident 自治恢复继续走队列分帧，不再同窗整批 resume。
   - `MaxTownResidentRecoveriesPerTick` 从 `6` 压到 `3`，把 Town 返场/恢复 resident 的短窗摊薄。
2. `NPCAutoRoamController.cs`
   - autonomous candidate 收集新增预算与早停：
     - normal `4`
     - startup `3`
     - recovery `2`
   - recovery/random/relaxed 采样一旦收满候选预算就提前停，不再把更多候选送进完整打分/建路链。
   - `InsertAutonomousRoamCandidate(...)` 超预算会裁尾，避免候选列表继续膨胀。
3. `SpringDay1DirectorStagingTests.cs`
   - 静态 helper 测试入口改成按参数匹配的 `InvokeStatic(...)`。
   - 两条队列节流测试继续通过。

### 验证结果
1. 代码层：
   - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs` 通过。
   - `SpringDay1NpcCrowdDirector.cs` fresh `validate_script` 过 `no_red`。
   - `NPCAutoRoamController.cs` / `SpringDay1DirectorStagingTests.cs` 无 own red；CLI 多次 `unity_validation_pending` 主要是 `stale_status / tool missing` 类 transport 噪音，不是编译红。
   - fresh CLI console：`errors=0 warnings=0`。
2. targeted tests：
   - `SpringDay1DirectorStagingTests.CrowdDirector_QueueResidentAutonomousRoamResume_ShouldQueueInsteadOfImmediateRestart` passed
   - `SpringDay1DirectorStagingTests.CrowdDirector_TickQueuedAutonomousResumes_ShouldThrottleResumeBurst` passed
3. live：
   - `Force Spring Day1 FreeTime Validation Jump` 仍被外部运行态打断：
     - `FarmTileManager.OnHourChanged(...)` 空引用
     - 一次 UGUI `IndexOutOfRangeException`
   - 它们不是导航 own，本轮只记作 external live blocker。
   - 在不依赖 `Force FreeTime` 的 Town/opening stopgap probe 里拿到两次数据：
     - 早一轮：`roamNpcCount=15`，`avgFrameMs≈13.63`，`maxFrameMs≈26.96`，`maxGcAllocBytes≈1055877`
     - 当前一轮：`roamNpcCount=15`，`avgFrameMs≈11.93`，`maxFrameMs≈21.52`，`maxGcAllocBytes≈377207`
   - 当前 probe 没看到 `blockedAdvance/pathBuildFailure` 风暴，主要 skip reason 已转成 `AdvanceConfirmed / ReusedFrameMoveDecision / AutonomousRecoveryHold / IssuingVelocity`。

### 当前判断
1. herd-start 短窗这边，现在已经同时落了：
   - Day1 resident 恢复/释放分帧
   - autonomous resume 分帧
   - autonomous candidate 收集早停
2. live 数字有向下收，但不是完全同场 strict A/B：
   - 第二次 actor probe 的 resident 现场并不完全等于第一次
   - 所以当前可以说“导航 own 的短窗负担进一步下降了”，不能包装成“完整 Day1/Town 已最终过线”
3. 当前最明确的 external blocker 仍是：
   - `Force FreeTime` 被 `FarmTileManager` runtime NRE 打断
   - 它阻断了 fully resumed free-time 的最终 live 对照

### 当前恢复点
1. 若下一轮继续导航 own，优先级应是：
   - 等 external phase-jump blocker 清掉，再抓 fully resumed free-time 的 fresh live/profiler
   - 若导航还要再收一刀，更值的是 `static probe / overlap` 二级缓存或 `shared avoidance nearby snapshot` 复用
2. 当前本线程应转 `PARKED`，等待后续接手或用户 fresh live 指令。
3. 补记：本轮收尾已实际执行 `Park-Slice.ps1`，当前线程状态为 `PARKED`，blocker 已登记为：
   - `Force FreeTime Validation Jump -> FarmTileManager.OnHourChanged NullReferenceException`
   - validation jump 偶发 UGUI `IndexOutOfRangeException`
   - external blocker 清掉后需补 fully resumed free-time live/profiler
## 2026-04-16｜导航V3 施工补记：static probe 复用与近墙转向稳定窗已补一刀

### 用户目标
- 继续收导航 own 的 near-wall 卡顿与疯狂改朝向，不回吞 Day1 phase，也不靠冻结/砍 roam 换性能。

### 本轮实际落地
1. `NPCAutoRoamController.cs`
   - 为 obstacle static steering 新增 `adjusted-direction` 复用阈值：
     - `STATIC_STEERING_OBSTACLE_REUSE_MIN_ADJUSTED_ALIGNMENT`
   - 为近墙 steering 新增短时稳定窗：
     - `STATIC_STEERING_OBSTACLE_STABILIZE_MIN_ALIGNMENT`
     - `StabilizeObstacleStaticSteeringDirection(...)`
   - 当前效果是：
     - 当 NPC 已经沿上一帧侧绕方向在走时，不再因为“原始 probe 朝向变了”就立刻重跑整轮 static probe
     - 当同一局部墙角里新 probe 想把侧绕方向抖到另一边时，会先优先保留上一帧已经成立的侧绕方向
2. `NavigationAvoidanceRulesTests.cs`
   - 新增 `NPCAutoRoamController_ShouldReuseObstacleSteeringProbe_WhenFollowingPreviousAdjustedDirection()`
   - 用来钉住“当前朝向跟着上一次 adjusted direction 继续走时，应命中 static obstacle probe reuse”

### 验证结果
1. 代码层：
   - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs` 通过。
   - fresh CLI `errors --count 20 --output-limit 5` 返回 `errors=0 warnings=0`。
2. 自检过程：
   - 本轮中途引入过一条 own red：
     - `NavigationAvoidanceRulesTests.cs` 误用了不存在的 `Vector2EqualityComparer`
   - 已同轮修掉，之后 console 恢复 `0 error / 0 warning`。
3. tool / live：
   - `validate_script` 多次没有再报 owned error，但 Unity MCP 会话处于不稳定状态：
     - `manage_script tool missing`
     - direct `run_tests/read_console` 出现 `plugin session disconnected / ping not answered`
   - 因此这轮没拿到 fresh direct targeted test / live probe 终证。

### 当前判断
1. 这刀仍是导航 own 的 execution-quality 优化，不是 Day1 语义施工。
2. 它主要解决两件事：
   - 降低 near-wall 相邻帧重复 static probe 的概率
   - 降低同一局部几何里侧绕方向左右翻的视觉抖动
3. 当前可以诚实说：
   - 代码层无 own red
   - 新护栏已经落到位
   - 但 fresh Unity targeted test / live 还差工具链恢复后的补证

### 当前恢复点
1. 下一轮若继续导航 own，先补这轮缺的 targeted test / Town stopgap probe。
2. 若这刀 live 证据成立，再决定是否继续收 `static probe / overlap` 更深一层缓存，而不是现在继续盲扩。
## 2026-04-16 23:20｜导航V3 施工补记：`StaticNearWallHardStop` 首次 blocked 优先走 static recovery

### 当前主线目标
- 继续把导航 own 的 near-wall 卡顿和假死窗口往下收，但不回吞 Day1 phase/beat，也不靠冻结 resident、砍 roam 换性能。

### 本轮子任务
- 把 `StaticNearWallHardStop` 从“首次 blocked frame 先进入 `AutonomousRecoveryHold`”改成“先试 static recovery path，失败后才回落旧链”，并补 fresh targeted tests + live probe。

### 已完成事项
1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
   - `TryHandleBlockedAdvance(...)` 现在对 `StaticNearWallHardStop + autonomous contract` 会在首次 blocked frame 就优先尝试 `TryCreateStaticObstacleRecoveryPath(...)`。
   - recovery 成功时直接记成 `BlockedAdvanceStaticRecovery:StaticNearWallHardStop`，不再先落 `AutonomousRecoveryHold:StaticNearWallHardStop`。
2. `Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs`
   - 新增：
     - `NPCAutoRoamController_ShouldPreferStaticRecoveryPath_BeforeRecoveryHold_OnFirstStaticNearWallHardStop()`
   - 旧的 reuse 测试改成 deterministic sample：
     - 不再依赖一块会随 steering 阈值漂移的 offset-wall 几何
     - 直接钉 `TryReuseObstacleStaticSteeringProbe(...)` 的复用合同本身

### 验证结果
1. 代码层
   - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs` 通过。
   - `validate_script` 两个目标都没有 own red。
   - `NPCAutoRoamController.cs` 仍只有旧的 `Update()` 字符串拼接 GC warning。
2. targeted EditMode tests
   - 以下 5 条 fresh 通过：
     - `NPCAutoRoamController_ShouldReuseObstacleSteeringProbe_WhenFollowingPreviousAdjustedDirection`
     - `NPCAutoRoamController_ShouldTriggerStaticNearWallHardStop_WhenObstacleSitsDirectlyAhead`
     - `NPCAutoRoamController_ShouldEnterRecoveryHold_WhenStaticBlockedReasonRepeats`
     - `NPCAutoRoamController_ShouldAbortRepeatedStaticNearWallHardStop_InsteadOfReenteringRecoveryHold`
     - `NPCAutoRoamController_ShouldPreferStaticRecoveryPath_BeforeRecoveryHold_OnFirstStaticNearWallHardStop`
3. live
   - opening 窗口：
     - 条件：`Beat=EnterVillage_HouseArrival && Dialogue=idle`
     - actor probe：`phase=EnterVillage`，`beat=EnterVillage_HouseArrival`
     - stopgap probe：`avgFrameMs≈28.99`，`maxBlockedNpcCount=6`，`maxBlockedAdvanceFrames=4`
     - 关键 skip reason：
       - `BlockedAdvanceStaticRecovery:StaticNearWallHardStop=34`
       - `AutonomousRecoveryHold:StaticNearWallHardStop=15`
   - free-time 第一轮：
     - 条件：`Phase=FreeTime && Beat=FreeTime_NightWitness && Dialogue=idle`
     - stopgap probe：`avgFrameMs≈27.22`，`maxBlockedNpcCount=7`，`maxBlockedAdvanceFrames=31`
     - 有少量 `Stuck=6`
   - free-time 第二轮同窗复跑：
     - actor probe：`phase=FreeTime`，`beat=FreeTime_NightWitness`
     - stopgap probe：`avgFrameMs≈14.75`，`maxBlockedNpcCount=0`，`maxBlockedAdvanceFrames=0`
     - `topSkipReasons` 只剩 `AdvanceConfirmed`
4. fresh console
   - CLI `errors --count 20 --output-limit 5` => `errors=0 warnings=0`

### 当前判断
1. 这刀对 opening 短窗是有效的：
   - `StaticNearWallHardStop` 已经明显从“长 hold”转成“更少量的 static recovery + 少量 hold”
2. free-time steady-state 当前是健康的：
   - 第一轮更像刚跳转后的过渡样本
   - 第二轮 same-window 复跑已经回到 `0 blocked / 0 stuck`
3. 当前最准确口径是：
   - 导航 own 的 near-wall 假死链又收下一层
   - 但 opening 的视觉体验是否已经够自然，仍要看用户终验

### 当前恢复点
1. 如果后面 opening 视觉上仍让用户觉得卡或撞墙异常，下一刀优先继续收：
   - `scene-entry` 窗口里的 `BlockedAdvanceStaticRecovery:StaticNearWallHardStop` 频次
2. 不要回吞 Day1 owner；free-time steady-state 当前不值得再为导航 own 盲扩。
3. 本轮收尾已实际执行 `Park-Slice.ps1`，线程状态为 `PARKED`。

## 2026-04-16 23:59｜导航V3 自查补记：功能/性能有进展，但当前仍不是“已安全存档”
### 当前主线目标
- 在不回吞 Day1 owner 的前提下，把导航执行质量继续收窄到用户可感知更健康，同时确保提交与归档口径诚实。

### 本轮子任务
- 只读自查“功能和性能是否已经彻底同时成立”“导航相关内容是否早就提交到位”“本轮改动是否已经安全存档”“当前能不能安全分批提交”。

### 已确认事实
1. 功能/性能层：
   - 当前最准确口径仍是“opening 短窗明显改善，free-time steady-state 基本站住”。
   - 不能升级成“导航已经彻底过线”，因为 opening 视觉体验和整日 Town 体感还没有被用户终验钉死。
2. 提交层：
   - 不能说“导航相关内容在这次修改前就已经全部提交到位”。
   - 也不能说“我这次导航内容已经全部提交到位”。
   - 当前 `git status --short` 仍有大量 dirty/untracked；导航 own 相关根目录不是 clean。
3. 当前切片现场：
   - `thread-state` 文件显示 `导航V3` 当前是 `BLOCKED`
   - `current_slice = navigation-own-safe-batch-commit`
   - `owned_paths` 只写了 5 个目标文件，但 broad own roots 下面仍有大量 remaining dirty/untracked。
4. 关键代码文件当前 diff 体量很大：
   - `NPCAutoRoamController.cs` 与 `NavigationAvoidanceRulesTests.cs` 相对 `HEAD` 都不是“小而纯”的单刀 diff。
   - 这说明当前工作区里混着之前累计的导航 WIP，不能把它包装成“只剩这次新刀的待提交内容”。

### 当前判断
1. 导航 own 的最新一刀在代码层和局部 live 层是站住的，但体验层仍只能报“更健康了”，不能报“彻底好了”。
2. 当前不是“已经安全存档”的状态：
   - 没有 commit
   - 也没有通过 Sunset 当前 `Ready-To-Sync` 合法收口闸门
3. “安全分批提交”这件事当前不能直接成立：
   - 路径级分批不安全，因为 own roots 被大量额外 dirty 污染
   - 文件级分批也不天然安全，因为关键导航文件 diff 里混有之前累计 WIP

### 下一步恢复点
1. 如果后续优先处理提交安全性，先做的不是继续改导航，而是先把导航 own roots 里的 remaining dirty/untracked 清账、分流或确认 owner。
2. 只有 roots 收干净，或者用户明确接受“手工局部 Git 提交但不宣称 Sunset 合法 clean sync”，才适合继续谈分批提交。

## 2026-04-17 00:20｜真实施工：到达宽容、远点采样、static retarget 冷却已落地
### 当前主线目标
- 继续只收导航 own 的执行质量，不回吞 Day1 语义；重点改善三类玩家可见坏相：
  - 到点附近却迟迟不算到
  - 漫游老抽近点，原地短程反复
  - static block 后立刻反复重试，视觉像左右乱翻

### 本轮子任务
- 在 `NPCAutoRoamController` 里补一刀“arrival-and-stuck quality”：
  - blocked 近终点时更宽容到达
  - preferred roam 采样真正偏向远点
  - static retarget 改成短冷却后再续跑

### 已完成事项
1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
   - `GetAutonomousRoamSoftArrivalDistance()` 现在在 `blockedAdvanceFrames > 0` 时会额外放宽一点软到达半径。
   - `SampleAutonomousRoamOffset(...)` 修掉了一个关键漏洞：
     - 以前当 preferred minimum distance 已经逼近 `activityRadius` 时，分支会退回 `Random.insideUnitCircle * maxRadius`
     - 这会把“明明想抽远点”重新放回“全圆乱抽”，导致仍可能抽出很近的点
     - 现在改成明确采样 outer ring，不再回退成全圆内点
   - `TryRetargetAutonomousRoamAfterStaticBlock(...)` 不再同帧立刻 `TryBeginMove()`
     - 现在会先进入一个短冷却 pause，再重新续跑
     - 目标是减少撞墙/卡边后的来回翻方向和立刻重试
   - 同时把 autonomous roam 的 preferred travel distance ratio 往远点方向上调了一档，但没有粗暴改 `activityRadius` 语义。
2. `Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs`
   - 新增 3 条针对本轮逻辑的 targeted tests：
     - `NPCAutoRoamController_ShouldExpandSoftArrivalDistance_WhenBlockedNearDestination`
     - `NPCAutoRoamController_ShouldSampleOuterRing_WhenPreferredMinimumTravelDistanceNearlyMatchesActivityRadius`
     - `NPCAutoRoamController_ShouldPauseBeforeStaticRetargetRetry_AfterRepeatedStaticBlock`
   - 同时把 2 条已有 resume-permit tests 改成更稳定的真实合同测法：
     - 先在同一轮里打满 permit budget，再测 `TryDeferAutonomousResumeFromPauseState()`
     - 不再借 `TickShortPause/TickLongPause` 的外围状态机噪音来断言

### 验证结果
1. 代码层
   - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs` 通过。
   - `validate_script` 两个目标当前都没有 own red。
   - CLI `errors --count 20 --output-limit 5` => `errors=0 warnings=0`。
   - `validate_script` 仍会报 `unity_validation_pending`，主因是 editor `stale_status`，不是 owned compile red。
2. targeted EditMode tests
   - 以下 8 条 full-name targeted tests fresh 通过：
     - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldDeferAutonomousResumeFromShortPause_WhenResumePermitBudgetIsExhausted`
     - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldDeferAutonomousResumeFromLongPause_WhenResumePermitBudgetIsExhausted`
     - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldExpandSoftArrivalDistance_WhenBlockedNearDestination`
     - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldSampleOuterRing_WhenPreferredMinimumTravelDistanceNearlyMatchesActivityRadius`
     - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldPauseBeforeStaticRetargetRetry_AfterRepeatedStaticBlock`
     - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldSoftArriveBlockedAdvance_WhenAutonomousFailureHappensNearDestination`
     - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldPreferMediumDistanceSampling_ForAutonomousRoam`
     - `NavigationAvoidanceRulesTests.NPCAutoRoamController_ShouldPreferStaticRecoveryPath_BeforeRecoveryHold_OnFirstStaticNearWallHardStop`
3. live
   - 在 `Town / EnterVillage_PostEntry / 09:00 AM` 窗口跑了 `Tools/Sunset/NPC/Run Roam Spike Stopgap Probe`
   - artifact:
     - `Library/CodexEditorCommands/npc-roam-spike-stopgap-probe.json`
     - `Library/CodexEditorCommands/spring-day1-actor-runtime-probe.json`
   - 关键结果：
     - `npcCount=25`
     - `roamNpcCount=16`
     - `maxBlockedNpcCount=0`
     - `maxBlockedAdvanceFrames=0`
     - `maxConsecutivePathBuildFailures=0`
     - `topSkipReasons=AdvanceConfirmed`

### 当前判断
1. 这刀已经把“原地短跳 / 到点不算到 / static block 立刻重试”一起往正确方向推了一层。
2. 当前最可信口径是：
   - 结构和 targeted probe 站住了
   - Town opening 窗口的 live 也站住了
   - 但这还不是“全天所有 free-time / dinner 前后体验已经终验过线”
3. 本轮没有处理提交安全性；当前仍不能把它包装成“已正式提交”。

### 当前恢复点
1. 如果后续还有 fresh 视觉反馈说“仍有近墙疯狂转头 / 仍有很近了却不算到”，下一刀优先继续收：
   - blocked near-destination 的剩余边界
   - 以及 current destination 到 local safe center 的 handoff
2. 如果没有 fresh live 反例，导航 own 不该再盲扩回 Day1 owner。
3. 本轮已执行 `Park-Slice.ps1`，当前 thread-state=`PARKED`。
