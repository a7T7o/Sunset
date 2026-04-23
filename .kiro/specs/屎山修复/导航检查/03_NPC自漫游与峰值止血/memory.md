# 03 - NPC 自漫游与峰值止血

## 模块概述
- 承接 NPC roam、avoidance、卡顿、峰值 spike、driver 过重这一条线。

## 当前稳定结论
- 当前主因已压向 `NPCAutoRoamController` 的坏 case 循环，而不是 shared traversal kernel 本身
- `NPCMotionController` 朝向应尽量只认最终提交移动向量，不要持续被观察速度污染
- 当前这条线应继续按：
  - demo 级止血
  - 全修收薄
  两段式推进

## 当前恢复点
- 后续 NPC 避让失效、roam spike、坏序列卡顿，统一先归这里
- scene contract 和 persistent 重绑不再混写到本卷

## 2026-04-10 新增审计结论
- 用户新给的 profiler 图里，峰值仍显示为 `NPCAutoRoamController.Update() -> Physics2D.OverlapCircleAll -> GC.Alloc 15.3MB`，但这条链与当前仓库 live 源码不一致。
- 当前仓库全文检索 `Physics2D.OverlapCircleAll`，只剩：
  - `ChestDropHandler.cs`
  - `ChestController.cs`
  - `PlayerAutoNavigator.cs`
  不在当前 NPC roam 主链里。
- 因此这批 profiler 图更像旧二进制 / 旧采样现场，不能直接拿来证明“当前 stopgap 代码已经按当前源码重新复现”。
- 但用户描述的“NPC 一直往左走、朝向却左右来回翻”是真实体验问题，而且和 profiler 旧链不是同一个问题。
- 当前更像是朝向链硬伤：
  - `NPCAutoRoamController.ResolveFacingDirectionForMoveCommand(...)` 仍优先吃 `desiredDirection`
  - `NPCMotionController.ResolveFacingVelocity(...)` 又优先信任 `_externalFacingDirection`
  - 结果就是移动实体还能继续往左，动画朝向却会被原始 waypoint / move command 抖动反复改写。
- 下一刀若继续本问题域，优先只修“朝向跟最终提交速度 / 最终有效移动决策”这一刀，再用 fresh current-binary live 证据复判；不要再把 profiler 旧链和当前朝向抖动混成一个问题。

## 2026-04-10 代码审核补记
- 现有新补丁已经把 `NPCAutoRoamController` 下发给 `NPCMotionController` 的朝向改成了“优先认本帧最终提交 velocity”，这条 auto-roam 局部修法方向是对的。
- 但当前仍不能宣称“NPC 朝向链已彻底闭环”，因为还有多条运行时链路继续直接调用 `NPCMotionController.SetFacingDirection(...)`，会绕开这条 auto-roam 修法。
- 当前最危险的残留点在：
  - `SpringDay1NpcCrowdDirector.cs:1696-1707`
  - 这里在 `roamController.StartRoam()` 之后，立刻 `StopMotion()` 并 `SetFacingDirection(state.BaseFacing)`；说明 free-roam 的 facing ownership 还不是唯一来源。
- 同类直接覆写还存在于 `SpringDay1NpcCrowdDirector.cs:537-538, 1281, 1794, 1853`，以及 `SpringDay1Director.cs:5188-5190` 这类“速度和朝向分两个 API 喂”的脚本导演链。
- 因此当前正确判断应是：
  - auto-roam 自己那条朝向 bug 代码上已被对位修正
  - 但 Town/day1 的 NPC 运行时仍不是单一朝向 owner，真实体验仍可能继续出现“脸被别的链抢写”

## 2026-04-10 认知同步结论
- 用户当前真正不满意的，不只是“NPC 脸左右翻”，而是：
  - NPC 连静态世界都走不明白
  - 玩家不会撞的树、围栏、封闭区，NPC 却会撞、会顶、会把坏 case 拖成 storm
- 对“这是不是性能与功能天然冲突”的当前判断：
  - 基本同意“不是天然冲突，而是 NPC 导航实现没收平”
  - shared traversal kernel 仍是统一的，但 `NavigationTraversalCore` 明确只负责底层 traversal contract，不负责“谁给目标、什么时候算失败、什么时候该放弃”
  - 真正没收平的是 NPC caller-level contract / owner / roam domain
- 当前至少要拆 4 层：
  1. 朝向 owner：Town/day1 存在多个运行时链直接写 `SetFacingDirection(...)`
  2. 静态世界可达性 / 不可达点处理：NPC autonomous roam 会随机采样目标并反复 `TryBeginMove / TryRebuildPath`
  3. 封闭区域 / 养殖区 roam domain 约束：NPC 的 `activityRadius + homeAnchor` 不是语义化 roam domain，本身允许坏目标被反复抽到
  4. 坏 case storm / 峰值：当上面 2、3 层没收住时，才会进入 rebuild / blocked advance storm
- 当前对“玩家和 NPC 是否两套 contract”的判断：
  - 底层 traversal kernel 基本是一套
  - 但 caller-level contract 明显不是一套
  - 玩家是显式目标 + 明确 stuck cancel / 完成语义；NPC 是 autonomous sample + 局部重建 + 弱 domain 约束
- 下一轮若继续，不该先去泛改性能，也不该先重打朝向 patch；最值钱的第一验证目标是：
  - 找一个 `anchor` 靠近树/围栏/封闭区的 NPC，验证它在 free-roam 时到底是不是持续抽到了语义上不该去、或静态上不可达的目标点
  - 再确认它进入 bad case 时，是哪条 owner / rebuild 链继续接管了它

## 2026-04-10 第二轮窄验证 prompt 已落
- `spring-day1` 已完成对第一轮认知同步回执的审核，判断为：
  - 可以继续
  - 但还不能直接进施工
  - 必须先补一轮更窄的 live 验证
- 当前新增的关键审核意见：
  - 不能只盯 `roam target / owner / rebuild`
  - 还必须单独核 `NPC vs Player` 在 `clearance / avoidance / blocker` 层是否存在 caller-level contract 或参数差异
- 已落第二轮 prompt：
  - [2026-04-10_导航线程_NPC静态导航坏case第二轮窄验证prompt_64.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-10_导航线程_NPC静态导航坏case第二轮窄验证prompt_64.md)
- 这轮 prompt 的唯一目标：
  - 用两个代表性坏 case，把第一失真点钉死在 `坏目标采样 / walkable 解析 / clearance-avoidance 契约 / 外部 owner 接管` 之一
- 当前恢复点：
  - 等导航线程按 `prompt_64` 回第二轮窄验证回执
  - 再决定是否发“第一刀真修” prompt

## 2026-04-10 prompt_64 第二轮窄验证结论
- 本轮继续严格只读，未进真实施工，未跑 `Begin-Slice`；线程状态继续保持 `PARKED`。
- 这轮已把两个代表性坏 case 的“第一失真点”缩到可直接支撑下一轮第一刀：
  1. `Town` 居民代表：`101`
     - scene 里当前实例位置约 `(-7.65, 14.88)`，但绑定的 `101_HomeAnchor` 在 `(28.5, 39.1)`。
     - `NPCAutoRoamController.TryBeginMove()` 会先用 `GetRoamCenter()`，而 `GetRoamCenter()` 又优先吃 `homeAnchor`。
     - 这意味着它 free-roam 的随机采样圆心一开始就是错的；第一失真点发生在 `坏目标采样 / 错 roam center`，早于 `walkable / rebuild / avoidance`。
  2. 封闭区/养殖区代表：`小动物` 组里的 `Baby Chicken Yellow / Chicken Red`
     - 这些 prefab 的 `homeAnchor` 为空，`GetRoamCenter()` 会退回初始 `transform.position`，只剩一个 `activityRadius=1.6` 的圆形 roam 域。
     - scene 里 `TraversalBlockManager2D` 已把 `fence / border / tree / rock` 作为静态阻挡绑定进 `NavGrid`，但并没有给小动物提供“围栏内语义域”。
     - 结果是采样点可以合法落到“碰 fence / fence 外侧最近 walkable”这一类语义上不该去的位置；第一失真点仍是 `坏目标采样`，`TryResolveOccupiableDestination -> TryFindNearestWalkable` 只是后续把坏目标改写成了“物理上可走但语义上不该去”的点。
- 这两个坏 case 不是同一个立即触发点，但属于同一大根：
  - 共同根：`NPC free-roam 没有语义化 roam domain，目标先抽歪，再把后面的 walkable / rebuild / storm 链全部激活`
  - 居民侧是 `homeAnchor / staging slot` 错配
  - 小动物侧是 `封闭区没有 domain，只剩圆形半径`
- `NPC vs Player` 的 `clearance / avoidance / blocker` 差异本轮也已单独核出，结论是“差异真实存在，但不是这两个 static bad case 的第一失真点”：
  - `NPC`：`ShouldAvoid()` 几乎避一切非静态单位，`GetAvoidanceRadius()` 是 capped shell，且只有 `state == Moving` 才参与 local avoidance。
  - `Player`：只对 `NPC / Enemy` 避让，`GetAvoidanceRadius()` 是 `playerRadius + contact shell`，并且额外带有 passive NPC blocker keep/cancel/detour 语义。
  - 所以这层更像“坏 case 放大器”，不是“第一颗倒下的骨牌”。
- 当前最值钱的第一刀，不该先修朝向 owner，也不该先泛修 spike；应该先只修 `NPC autonomous roam 的目标域 contract`：
  - 居民：当前 live owner/staging 下，采样圆心不能继续直接吃错位的 `homeAnchor`
  - 小动物：采样候选不能越出围栏/养殖区语义域，再交给 `TryResolveOccupiableDestination`

## 2026-04-10 NPC autonomous roam 目标域 contract 第一刀已落地
- 本轮已从只读切进真实施工，并已跑 `Begin-Slice -> Park-Slice`；当前状态重新停回 `PARKED`。
- 实际只改了 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)，没有扩到玩家链、scene、director 或 NavGrid。
- 这刀落了两件事：
  1. `错位 homeAnchor` 不再无脑充当 free-roam 圆心：
     - 新增了“anchor 是否离当前 NPC 过远”的判定；
     - 对像 `101 / 201` 这种当前实例位置和 scene 里 `*_HomeAnchor` 明显错位的对象，roam center 会退回当前在场位置，而不是继续围着远处锚点抽点。
  2. `自动漫游采样` 不再接受“大幅纠偏后的最近可走点”：
     - `TryBeginMove()` 现在先走新的 autonomous-roam 目标解析；
     - 只有原采样点本身可占用，或只发生很小幅度修正时，才允许进入建路；
     - 像围栏外、边界外、fence 边硬修正回来的点，会直接被丢弃，不再进入 bad case build/rebuild。
- 这刀的意图和边界：
  - 修的是 `NPC autonomous roam 目标域 contract`
  - 不碰 `facing owner`
  - 不碰 `shared avoidance` 规则
  - 不碰 `PlayerAutoNavigator`
- 当前最小代码验证：
  - `validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
    返回 `assessment=unity_validation_pending owned_errors=0 external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 通过
  - 当前没有代码层 owned red，但 Unity/MCP fresh runtime 证据这轮没拿到，因为本机 `127.0.0.1:8888/mcp` 基线未连通

## 2026-04-10 NPC完好系统收口（第三刀）
- 用户这轮把目标重新钉死为：
  - `NPC` 要走路正常
  - 朝向正常
  - 不撞墙硬顶
  - 且不能再出现性能炸弹
- 本轮真实施工只继续压在两份运行时代码：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
  - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
- 本轮明确没有触碰：
  - `SpringDay1NpcCrowdDirector.cs`
  - `SpringDay1Director.cs`
  - 玩家导航链
- 这轮实际新增了两层收口：
  1. `NPCMotionController`
     - 新增 `movingFacingAlignmentThreshold`
     - locomotion 期间，`SetFacingDirection(...)` 只有在与当前真实移动足够对齐时才允许生效
     - `_externalFacingDirection` 与 observed movement 明显偏离时，朝向直接回退到 observed movement
     - 目的：把“边走边被别的链扯脸”压掉，不再左走右脸、一步三回头
  2. `NPCAutoRoamController`
     - 对 `ConstrainedZeroAdvance / MoveCommandOscillation / MoveCommandNoProgress / ZeroStep`
       这类“静态坏 case 且没有动态 blocker”的情况，连续 2 次同点命中后直接放弃本次 move cycle
     - 不再继续进入 rebuild / hard-push 循环
     - 目的：让 NPC 在撞墙或抽到坏目标时更快放弃，避免 wall-thrash 和性能放大
- fresh 代码层验证：
  - `validate_script Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs --count 20 --output-limit 5`
    => `assessment=no_red / owned_errors=0 / external_errors=0`
  - `validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
    => `assessment=no_red / owned_errors=0 / external_errors=0`
  - `validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs --count 20 --output-limit 5`
    => `assessment=no_red / owned_errors=0 / external_errors=0`
  - `git diff --check` 针对两脚本通过
- fresh Town live 证据：
  - 用 `Tools/Sunset/NPC/Run Roam Spike Stopgap Probe`
  - 先静置 10 秒，再采样 6 秒
  - 当前结果：
    - `scene=Town`
    - `npcCount=25`
    - `roamNpcCount=16`
    - `avgFrameMs=0.887`
    - `maxFrameMs=15.256`
    - `maxGcAllocBytes=37197`
    - `maxBlockedNpcCount=0`
    - `maxBlockedAdvanceFrames=0`
    - `maxConsecutivePathBuildFailures=0`
  - 当前这组证据说明：
    - Town 稳态下没看到 `blocked/stuck/rebuild storm`
    - 没看到新的 GC 炸点
    - 本轮没有把 NPC 再修回性能炸弹
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑（本轮不是 sync 收口）`
  - `Park-Slice=已跑`
  - 当前 `PARKED`

## 2026-04-10 鬼畜朝向诊断输出补口
- 用户明确指出：
  - 剧情走位、剧情结束后回 anchor 的路上基本正常
  - 真正鬼畜的是 free-roam / 自漫游阶段
- 本轮没有再扩修功能，而是给 [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs) 补了运行态诊断输出：
  1. `SetFacingDirection / SetExternalVelocity / SetExternalFacingDirection` 现在都会自动记录最后写入源；
  2. 当 `observedDir != appliedDir` 连续出现时，会输出：
     - `observedDir`
     - `appliedDir`
     - `observedVelocity`
     - `facingVelocity`
     - `externalVelocity`
     - `externalFacing`
     - `facingSource`
     - `externalVelocitySource`
     - `externalFacingSource`
     - 以及当前 `roamState / moveSkip / blockedFrames`
- 日志前缀：
  - `[NPCFacingMismatch]`
- current live 结果：
  - Town 内 fresh 跑了 12 秒，当前没有打出 `[NPCFacingMismatch]`
  - 这说明：
    - 诊断已经挂好并可用
    - 但用户描述的坏 case 没在这次 Town 短窗口内复现
- 当前下一步恢复点：
  - 一旦用户在 `Primary` 或特定 free-roam 场景里再次复现“明明往左走但脸乱翻”，Console 现在会直接给出真正写入源，不再靠猜

## 2026-04-10 鬼畜朝向诊断增强与瞬时噪声止损
- 用户这轮把问题重新钉得更具体：
  - 不是“NPC 会往回走”
  - 而是“NPC 明明持续往左走，但脸会左右来回翻”
- 本轮真实施工只改：
  - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
- 这刀补了两件事：
  1. 诊断升级：
     - 之前 `[NPCFacingMismatch]` 只抓“连续多帧都错”的情况；
     - 现在额外能抓短窗口 burst mismatch，不再漏掉“一步一回头”这种间歇性翻脸。
  2. 朝向止损：
     - 当 `observedVelocity` 与 `externalFacing` 只是一两帧短暂打架时，不再立刻跟随瞬时观测改脸；
     - 只有同一冲突方向持续超过确认窗口，才允许 observed movement 抢回朝向。
- 额外补的 trace：
  - `showDebugLog` 打开时，日志会直接输出：
    - `ObservedDir`
    - `AppliedDir`
    - `ExternalFacing`
    - `FacingSource`
    - `ExternalVelocitySource`
    - `ExternalFacingSource`
- fresh 代码层验证：
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs --count 20 --output-limit 5`
    => `assessment=no_red / owned_errors=0 / external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs` 通过
- fresh Unity 状态：
  - `Town.unity / Edit Mode / ready_for_tools=true`
  - 当前 console fresh 读取 `error_count=0 / warning_count=0`
- 当前判断：
  - 这刀的价值不是“已经宣布鬼畜彻底消失”，而是：
    1. 先把最容易制造左右乱翻的一帧噪声改脸收住
    2. 再把真正剩余的 owner 冲突抓得出来
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑`
  - `Park-Slice=已跑`
  - 当前 `PARKED`

## 2026-04-11 resident scripted control 冻结链 exact fix
- 这轮先按 `prompt_65` 只查 `resident scripted control` 的 acquire / halt / release / resume 现实链。
- 静态结论已钉死：
  - [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs#L529) 先 `AcquireResidentScriptedControl(...)`
  - 然后 [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs#L534) 直接 `_roamController.enabled = false`
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L543) `OnDisable()` 旧逻辑会走 `StopRoam()`
  - `StopRoam()` 又会清掉 `resumeRoamAfterResidentScriptedControl`
  - 最后 [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs#L583) `ReleaseResidentScriptedControl(..., _roamWasRoaming)` 发生后，也可能不再恢复 free roam
- 本轮实际修补：
  - `OnDisable()` 在 `IsResidentScriptedControlActive` 时不再走 `StopRoam()`
  - 改为走 `HaltResidentScriptedMovement()`，保留 release 后恢复 roam 的意图
- fresh 验证：
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
    => `assessment=no_red`

## 2026-04-13｜NPC 功能回补与性能收敛并行：自漫游 acceptance / 朝向 owner / resident 基准朝向抢写三层同轮落地

- 当前主线目标：
  - 在不回退玩家 traversal 语义、不把全局物理查询重新拉重的前提下，把 NPC 自漫游恢复到“取点不贴墙、朝向跟真实位移、resident 不再被 baseFacing 抢回去”的可打包状态。
- 本轮真实施工：
  1. [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
     - `TryBeginMove()` 现在重新区分：
       - `普通自漫游`：走 `TryResolveAutonomousRoamDestination(...)` + `IsAutonomousRoamBuiltPathAcceptable(...)`
       - `debug / resident scripted control`：仍走便宜的 `TryResolveOccupiableDestination(...)`
     - 作用：
       - 把我之前为了止血峰值退掉的“坏目标修正 / 邻域净空 / 离谱绕路过滤”只在自漫游层接回
       - 不把 shared traversal / scripted move 再拉回重查询
  2. [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
     - `ResolveFacingVelocity(...)` 现在优先认：
       1. `observedVelocity`
       2. `recent rb.linearVelocity`
       3. 才退回 `externalFacing / externalVelocity`
     - 作用：
       - 当 NPC 明明正在往左/右/上/下移动时，朝向优先跟真实位移，不再被旧 intent 或外部 facing 拖反
       - 保留原有低速/无观测位移时的 external fallback
  3. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - 新增 `ApplyFacingIfIdle(...)`
     - `snapshot restore / initialFacing / ResetStateToBasePose / baseline fallback / snap-home / finish-return-home / night-rest`
       这几条基准朝向写入，现在都会先确认：
       - 当前实例不在 roam moving
       - `NPCMotionController` 当前没有真实/报告速度
     - 作用：
       - resident 还在移动时，导演层不再硬写 `BaseFacing`
       - 收口到“谁驱动移动，谁拥有朝向”
  4. [SpringDay1LateDayRuntimeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs)
     - 只补了一个测试编译尾巴：把 `context.playerMovement` 显式转成 `Component`
     - 目的只是恢复 fresh compile / console 验证，不改运行时逻辑
- 本轮验证：
  - 原生脚本校验：
    - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) `errors=0`
    - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs) `errors=0`
    - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) `errors=0`
    - [SpringDay1LateDayRuntimeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs) `errors=0`
  - `git diff --check` 对以上 4 个文件通过
  - fresh compile / console：
    - Unity refresh 后，`read_console(error|warning)` 返回 `0`
  - 最小 live：
    - 当前已打开的 `Primary` 里短跑约 `6s`
    - 未刷出 `NPCFacingMismatch` 或导航红错
    - 只剩 1 条外部 TMP 字体 warning：`LiberationSans SDF (Runtime)` static atlas
- 当前判断：
  - 这轮已经把“功能体验回补”和“性能不回炸”的平衡点重新拉回来了：
    - 自漫游重新会拒绝明显贴墙/离谱路径
    - 朝向重新优先跟真实移动
    - resident 基准朝向不再和 roam movement 抢 owner
  - 但 `Town` 常驻居民的大场景体验，仍建议用户再做一轮人工终验；我这轮 live 只在当前脏场景安全口径下跑了 `Primary`，没有切走用户现场

## 2026-04-11 Town resident owner 接线落地：baseline / return-home / night-rest 改走 resident scripted control
- 用户本轮批准直接落地，要求保留当前玩家 traversal/bridge/water/bounds 与性能 hardening，不做整仓回退。
- 本轮实际改动：
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
  - 新增 `ResidentScriptedControlOwnerKey` 与 `Prepare/Acquire/ReleaseResidentDirectorControl(...)`
  - 把以下 resident 常驻链改成显式 owner 接线，而不是继续裸写 roam/motion：
    - `ApplyResidentBaseline(...)`
    - `TryBeginResidentReturnHome(...)`
    - `FinishResidentReturnHome(...)`
    - `CancelResidentReturnHome(...)`
    - `ApplyResidentNightRestState(...)`
    - `ResetStateToBasePose(...)`
    - `ApplyResidentRuntimeSnapshotToState(...)`
- 当前最关键修补点：
  - baseline free-roam 不再每轮 `StopMotion() + SetFacingDirection(BaseFacing)` 抢回姿态权
  - return-home / night-rest / reset / snapshot restore 现在会显式 `Acquire/ReleaseResidentScriptedControl`
  - 也就是把 `Town` 常驻 resident 的导演层控制权，真正接回 `NPCAutoRoamController` 现成的 resident runtime contract
- fresh 代码层验证：
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs --count 20 --output-limit 5`
    - `owned_errors=0`
    - `assessment=external_red`
    - 外部 blocker 落在 `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs` 通过
- fresh Town 只读 probe：
  - `Tools/Sunset/Scene/Run Town Runtime Anchor Readiness Probe`
    - `status=completed`
    - 说明 `Town` resident/backstage/director-ready scene-side 承接位与 HomeAnchor 配置位是站住的
  - `Tools/Sunset/Scene/Run Town Player-Facing Contract Probe`
    - `status=attention`
    - 唯一 attention：`EnterVillageCrowdRoot` 不在第一屏视野；不是当前 resident owner/导航 blocker
- live 验证报实：
  - 已用 `SpringDay1NativeFreshRestart` 把编辑器切到 `Town`
  - 尝试进入 Play 取 fresh resident live
  - 但 Play 没能稳定进入；当前 blocker 仍是外部 `SpringDay1LateDayRuntimeTests.cs` 编译红
  - 因此这轮拿到了：
    - owner 代码层落地
    - Town scene-side probe 通过
    - 但没有拿到 fresh Town 体验层闭环
- 当前阶段：
  - `Town` 常驻 resident severe 摆头/倒走的 owner 冲突已真正下刀
  - 体验层还欠最后一轮 live，当前被外部测试红阻断

## 2026-04-11 Town 常驻居民 read-only 收口：坏相已分成 owner 干扰层与导航 own 残留层
- 本轮只读，没有继续落 stopgap 代码。
- 新增最关键结论：
  1. `101~301` 不是“纯导航 roam NPC”，而是 `SpringDay1NpcCrowdDirector` 管的 resident crowd
  2. 这组人的 severe 朝向乱飘，已经被 fresh console 直接定责到 crowd director
  3. `001~003` 不在 crowd manifest，因此“能走稳但避障回退”不能再和 crowd owner 混报成同一个 exact blocker
- exact 证据：
  - [SpringDay1NpcCrowdManifest.asset](D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset) 只包含 `101~301`
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L1730)
    - `ApplyResidentBaseline(...)` 在居民已经交回 roam 后仍执行 `motionController.StopMotion()` 和 `SetFacingDirection(state.BaseFacing)`
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L1841)
    - `TickResidentReturnHome(...)` 也在直接写 `SetFacingDirection(step)`
  - `py -3 scripts/sunset_mcp.py status`
    - fresh console 直接命中：
      - `104 -> facingSource=SpringDay1NpcCrowdDirector.ApplyResidentBaseline`
      - `103/202 -> facingSource=SpringDay1NpcCrowdDirector.TickResidentReturnHome`
- 对小动物：
  - fresh console 里鸡/牛的 mismatch 来源仍是 `NPCAutoRoamController.ApplyMoveVelocity`
  - 这说明小动物的坏相更像 motion/facing 合成层，不等于 crowd owner
- 对验证层的新增报实：
  - 尝试用 `NpcRoamSpikeStopgapProbe` 取 Town live
  - 但 Play 起场景被 guard 导到了 `Primary`
  - 本轮 probe 报告只得到 `scene=Primary / npcCount=2 / topSkipReason=AdvanceConfirmed`
  - 不能拿这份 probe 冒充 `Town` 常驻居民 fresh 体验证据
- 当前收口判断：
  - `Town` 常驻居民 severe 摆头/倒走` 的第一真 blocker 已落到 `SpringDay1NpcCrowdDirector`
  - `001~003` 的避障回退和小动物的 face lag 还需要导航线继续窄验，但已经不该再与 crowd owner 混为一谈
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 通过
- 这轮额外重新锚定用户主线：
  - 用户现场已明确：Town 常驻 NPC 的 `避障回退 + 朝向乱飘` 才是下一条主主线
  - resident freeze 漏洞已经修掉，但不能偷换成“NPC 已整体恢复”
- 下一轮已生成新 prompt：
  - [2026-04-11_导航线程_Town自漫游避障回退与朝向乱飘收口prompt_66.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-11_导航线程_Town自漫游避障回退与朝向乱飘收口prompt_66.md)

## 2026-04-11 Town 自漫游：sleeping 语义与现实速度绑定
- 为了不让 shared avoidance 把“状态上还在 Moving、现实里几乎没动”的 NPC 继续当成正常动态体，本轮再补了一个同根小口：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L403)
- 具体改动：
  - `IsNavigationSleeping()` 现在不是单纯 `state != Moving`
  - 而是 `state != RoamState.Moving || GetCurrentVelocity().sqrMagnitude <= 0.0001f`
- 同轮还补了：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L374)
  - `GetCurrentVelocity()` 改为优先返回 `motionController.CurrentVelocity` 与 `rb.linearVelocity`
  - 只有缺真实速度时才退回 `ReportedVelocity`
- 作用：
  - 被卡住/几乎不动的 roaming NPC，现在更容易被其它 NPC 当作临时 blocker 看待
  - 这和上一个“优先现实速度”补口一起，构成同一条交通语义修补
- fresh 验证：
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
    => `assessment=no_red`
## 2026-04-14 11:34 prompt_67 续读：Day1 放人后 valid roam 样本已分账，当前 first gap 继续收窄到“静态真值 + 非剧情态执行契约”
- 本轮严格先读：
  - [2026-04-14_导航线程_Day1边界重划后非剧情态roam静态契约收口prompt_67.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-14_导航线程_Day1边界重划后非剧情态roam静态契约收口prompt_67.md)
  - 当前导航线程 memory / 工作区 memory
- 本轮没有继续改代码，最终已执行：
  - `Park-Slice=已跑`
  - 当前 live 状态=`PARKED`
- 这轮新压实的 5 个事实：
  1. `001/002/003` 已经是 prompt_67 认可的 valid roam 样本：
     - `spring-day1-actor-runtime-probe.json` 里这 3 个当前都是 `roamControllerEnabled=true`、`isRoaming=true`、`scriptedControlActive=false`
  2. `101~203` 当前仍不是 valid roam 样本：
     - 同一份 probe 明确显示它们仍是 `scriptedControlOwnerKey=spring-day1-director`
     - `roamControllerEnabled=false`
     - `roamState=Inactive`
     - 所以这批人这一拍仍应继续算 `day1 owner`，不能回灌成导航 core 现场
  3. 动物和普通 NPC 当前确实同根走 `NPCAutoRoamController + NPCMotionController`
     - `Baby Chicken Yellow.prefab`
     - `Chicken Red.prefab`
     - `Female/Male Cow Brown.prefab`
     - 都直接挂同一套 roam/motion 组件
  4. 我前一拍补进 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 的 `AdjustDirectionByStaticColliders(...)` 现在是 valid contract 内补口，不是漂到 Day1：
     - 仅在 `!debugMoveActive && !IsResidentScriptedControlActive` 时生效
     - 所以只会打在非剧情态 roam，不会碰 scripted/staging
  5. `Town.unity` 当前的静态配置还留着一个真实 gap：
     - `walkableOverrideTilemaps=[]`
     - `walkableOverrideColliders=[]`
     - 也就是 Town 这张图当前没有把“桥面可走 / 水面不可走”的 walkable override 显式挂出来
- 本轮额外核掉的误判：
  - “building/tree/fence collider 因为挂得太深，`TraversalBlockManager2D` 只看 6 层父链所以漏收”这一猜测，至少对典型房屋 prefab 不是现成铁证：
    - `Assets/222_Prefabs/House/House 1.prefab` 的 Building collider 只在第 2 层
- 当前最稳的判断：
  - prompt_67 下，第一分账已经站稳：
    - `101~203` 仍由 `day1 owner` 持有，不是这轮导航样本
    - `001~003 + 动物` 才是这轮该看的非剧情态 roam 样本
  - 导航 own 这边现在最像的真实缺口，不再是“又一次性能 storm”，而是：
    1. 静态 baked truth 要继续和 Town 当前真实配置对齐
    2. 非剧情态 roam 执行层要继续证明它真的像玩家一样尊重静态 collider，而不只是“多了一层止血 steering”
- 当前恢复点：
  1. 下轮如果继续 real fix，只能继续留在 prompt_67 白名单：
     - `NPCAutoRoamController.cs`
     - `TraversalBlockManager2D.cs`
     - 如有必要再扩到 `NPCMotionController / NavigationLocalAvoidanceSolver / NavigationAvoidanceRules / NavigationAgentRegistry`
  2. 不要再拿 `101~203` 当 valid roam 例子
  3. 如果要动 scene，只能以 “Town 的 bridge/water walkable override 缺口” 这种 exact blocker 为前提做最小核对，不准回吞 day1 逻辑

## 2026-04-14 12:15 prompt_67 真实施工续记：NPC 非剧情态静态避让与早退脱困刀已落，性能 fresh 过但体验终证仍待 valid roam 窗口
- 当前主线仍是：
  - 只收 `Day1 已放人后` NPC / 动物非剧情态 roam 的静态执行契约
  - 不碰 `SpringDay1Director / SpringDay1NpcCrowdDirector` own
- 本轮已执行真实施工：
  - `Begin-Slice=已在本轮前半执行`
  - 本轮实际修改文件仍只落在：
    - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
- 这刀的实际补口分两类：
  1. `静态避让更像玩家`
     - autonomous roam 的邻域 clearance 半径、body clearance、path clearance 采样密度都提高
     - `AdjustDirectionByStaticColliders(...)` 的 repulse 更强，转向上限收紧
     - 目标是减少贴房、贴树、围栏边磨行走
  2. `静态卡住更早脱困`
     - autonomous roam 遇到纯静态 bad case 时，不再长时间原地短停/磨步
     - 更早触发 `TryRetargetAutonomousRoamAfterStaticBlock(...)`
     - 仍保留动态 blocker / resident scripted control 的边界，不把剧情链误伤成 roam stopgap
- fresh 代码/红面验证：
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
    - `assessment=no_red`
    - `owned_errors=0`
    - `external_errors=0`
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 5`
    - `errors=0`
    - `warnings=0`
- fresh live 证据：
  1. 当前我自己重新跑了受控短 live：
     - `PLAY -> SpringDay1ActorRuntimeProbe -> NpcRoamSpikeStopgapProbe -> STOP`
     - 已主动退回 `Edit Mode`
  2. actor probe 结果：
     - `activeScene=Town`
     - 当前 fresh 时点是 `phase=EnterVillage` / `beat=EnterVillage_PostEntry`
     - 所以这一次并不是 prompt_67 要的“完整 valid resident roam 窗口”
     - 现场里：
       - `001/002` 仍是 `scriptedControlActive=true`
       - `003` 已进入 `isRoaming=true`
     - 这说明当前这次 live 只能拿来证明“场景和 owner 状态”，不能拿来冒充最终体验终证
  3. spike probe 结果：
     - `scene=Town`
     - `npcCount=25`
     - `roamNpcCount=16`
     - `avgFrameMs=0.6868`
     - `maxFrameMs=14.6529`
     - `maxGcAllocBytes=235072`
     - `maxBlockedNpcCount=0`
     - `maxBlockedAdvanceFrames=0`
     - `maxConsecutivePathBuildFailures=0`
     - `blockedAdvanceStopgapSamples=0`
     - `passiveAvoidanceStopgapSamples=0`
     - `stuckCancelStopgapSamples=0`
  4. 因此这轮 fresh live 只证明两件事：
     - 这刀没有把 `Town` 再炸回性能 storm
     - 但这次 fresh 入口不是完整的 `Day1 已放人 resident roam` 样本，不能直接替代 prompt_67 的体验验收
- 当前最稳判断：
  - 这轮代码层已经站住到“结构成立 + 性能未回炸”
  - 但 `prompt_67` 的体验层终证，仍需要回到真正的 valid roam 时窗继续看 `001~003 + 动物`
- 收口状态：
  - 我已先跑 `Ready-To-Sync`
  - 结果被白名单预检阻断，不是因为这刀 red，而是因为当前 own roots 里还有大量别轮残留 dirty/untracked：
    - `Assets/000_Scenes`
    - `Assets/YYY_Scripts/Controller/NPC`
    - `Assets/YYY_Scripts/Service/Navigation`
  - 所以这轮不能 claim `可直接 sync / commit`
  - 随后已执行：
    - `Park-Slice`
    - 当前状态=`PARKED`
- 当前恢复点：
  1. 如果下一轮继续，只能继续沿 `prompt_67`
  2. 直接优先回到真正的 valid roam 时窗，看 `001~003 + 动物`
  3. 如果仍出现贴静态障碍/小范围磨步，再继续优先补 `NPCAutoRoamController`，不要漂回 Day1 owner

## 2026-04-15 02:34 prompt_67 真实施工续记：把 release 后 home return 的 debug move 收成 formal navigation contract，并补到家后短暂 settle
- 当前主线仍是：
  - 只收 `Day1 已 release 后` 的 `return-home / 回 anchor / 再接回 free-roam` 导航执行合同
  - 不碰 `SpringDay1Director / SpringDay1NpcCrowdDirector` 当前 ACTIVE 文件
- 本轮真实施工已落：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
  - [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs)
- 这刀实际补了 4 件事：
  1. `DebugMoveTo(homeAnchor)` 不再和 plain debug 混成一类
     - 现在会识别“目标就是 homeAnchor / homePosition 附近”的 debug move
     - 这类 move 会挂上 `formal navigation contract`
  2. formal home return 不再绕开 shared avoidance
     - `ShouldBypassSharedAvoidanceForCurrentMove()` 只保留 plain debug 的旧 bypass
  3. formal home return 现在也吃静态 steering 和重建
     - `ShouldApplyAutonomousStaticObstacleSteering()` 已对 formal home return 放行
     - `CheckAndHandleStuck()` / `TryHandleBlockedAdvance()` 遇到 formal home return 时，不再走 plain debug 的直接停掉或随机 `TryBeginMove()`
     - 改成优先对同一目标重建路径，尽量减少卡房、卡树、卡石头后乱抽新点
  4. 到家后不会立刻被 `tryImmediateMove` 再抽一条 roam
     - `RestartRoamFromCurrentContext(true)` 现在对“刚刚完成 formal home return”会给一个短暂 settle 窗口
- 本轮补的最小回归测试：
  1. homeAnchor return 会被识别为 formal navigation contract
  2. plain debug 仍 bypass shared avoidance；formal home return 不再 bypass 且会开静态 steering
  3. formal home return 完成后，immediate restart 会先 delay 一下
- fresh 代码/红面验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs --count 20 --output-limit 5`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
    - 阻断原因不是这刀 compile 红，而是 Unity `stale_status`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py errors --count 20 --output-limit 5`
    - `errors=0`
    - `warnings=0`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs`
    - 通过
- 当前阶段判断：
  - 这轮已经把“release 后回家还是 plain debug、不吃避障、不吃重建、到家立刻再抽路”的 own 缺口收进 `NPCAutoRoamController`
  - 但当前还没有 fresh Town live 终证；只能报 `结构成立 + 代码层验证通过 + Unity live 仍被 stale_status 阻断`
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑`
  - `Park-Slice=已跑`
  - 当前状态=`PARKED`
- 当前恢复点：
  1. 外部 Unity `stale_status` 稳定后，优先回 Town fresh 看 `101/103/201/203/003` 的实际体感
  2. 如果仍出现“formal home return 结束后 Day1 手搓 fallback”，那就是 Day1 own 还在兜底，不是这轮导航 own 没接

## 2026-04-15 11:03 导航 own 真实施工：禁用 CrowdDirector 仍会互锁/撞墙后，改收 autonomous roam 的 `NPC-NPC deadlock + static obstacle steering`
- 当前主线目标：
  - 用户已把主锅重新钉回导航 own：即使禁用 `SpringDay1NpcCrowdDirector`，NPC 仍会互相卡死、撞墙、卡墙，所以这轮不再碰 Day1，只修 autonomous roam 执行质量。
- 本轮子任务：
  1. 让 `NPC-NPC` 互锁更早脱困，不再长时间互相磨停
  2. 让静态房屋/树木/石头前的侧绕更果断，不再只会正面磨墙
  3. 补最小编辑器测试，守住这轮 contract
- 本轮实际改动：
  - [NavigationLocalAvoidanceSolver.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs)
    - `NPC vs NPC` head-on / near-contact 时：
      - yielding 一侧的 sidestep 权重更高
      - yielding 一侧保留最小侧移速度，不再轻易原地冻住
      - hold-course 一侧保留更高前进速度，同时仍给微侧绕，减少“双停双磨”
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
    - `shared avoidance deadlock` 触发阈值提前：
      - `frames 18 -> 10`
      - `sightings 6 -> 3`
      - 距离/clearance 门槛放宽，纯 NPC blocker 更早认定为真互锁
    - `TryBreakAutonomousSharedAvoidanceDeadlock(...)` 现在优先创建 detour，再退回重抽新 move，不再直接等很久后随机 retarget
    - `AdjustDirectionByStaticColliders(...)` 现在：
      - 使用更大的 body-clearance probe 半径
      - 拉长前瞻探测距离
      - 当前方 repulse 明显呈“纯后退”时，增加稳定侧绕偏置，减少贴房/贴树/贴石头原地磨
    - passive NPC blocker 的 reroute 阈值也提前，减少小动物/居民互相挤住时的迟钝反应
  - [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs)
    - 新增 3 组回归：
      1. yielding NPC head-on 仍会保持侧移
      2. hold-course NPC head-on 不会跟着冻住
      3. 正前方静态障碍时，static steering 会给出明显侧偏
- fresh 验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs --count 20 --output-limit 5`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
    - Unity 当时卡在 `playmode_transition / stale_status`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name NavigationLocalAvoidanceSolver --path Assets/YYY_Scripts/Service/Navigation --level standard`
    - `status=clean`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name NPCAutoRoamController --path Assets/YYY_Scripts/Controller/NPC --level standard`
    - `status=warning`
    - 唯一 warning 仍是既有 `String concatenation in Update() can cause garbage collection issues`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name NavigationAvoidanceRulesTests --path Assets/YYY_Tests/Editor --level standard`
    - `status=clean`
  - `git diff --check -- Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs`
    - 通过
- 本轮判断：
  - 这刀已经把“纯导航 own 的 crowd deadlock 太迟脱困”和“静态障碍只会正顶不侧绕”一起往下收了一层
  - 但当前还没有 fresh Town live 终证；只能报 `代码层成立 + Unity live 仍被 stale_status 阻断`
- 当前恢复点：
  1. Unity 稳定后，优先回 Town 只看纯 autonomous roam 场景，验证：
     - 房屋/石头前是否还会长时间贴边磨
     - 小动物群是否还会互相锁死
     - 取消 `CrowdDirector` 后普通 NPC 是否仍能明显互锁
  2. 若仍复现，再继续只收导航 own：
     - `destination agent clearance`
     - `local detour candidate distance`
     - 不回漂 Day1 owner 语义

## 2026-04-15 11:44 只读复盘：牛圈局部抽搐与复杂 collider 形状问题，第一失真点已压到“逐步执行占位太窄 + 动态互锁放大”
- 当前主线目标：
  - 按 `prompt_68` 只读回答：`Day1` 已 release 后，为什么 NPC/动物在静态世界里仍会卡墙、抽搐、罚站、假 roam。
- 本轮子任务：
  - 把用户最新牛圈截图、`NPC` 回执、现有导航代码重新对齐，确认“复杂 collider 形状会不会影响 NPC 静态导航”以及“为什么玩家没事、NPC 有事”。
- 本轮实际确认的代码事实：
  1. `NPC` 不是完全没算身体大小，但热路径的逐步占位仍主要看“脚底三点”：
     - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs) `CanOccupyNavigationPoint(...)`
     - `GetNavigationProbePoints(...)`
     - `GetNavigationPointQueryRadius(...)`
     - 当前 query radius 仍被 `footProbeInset + extraRadius` 这类较窄脚底 probe 控住。
  2. `NPC` 真正移动时，`TickMoving()` 还是先：
     - `TryHandleSharedAvoidance(...)`
     - 再 `AdjustDirectionByStaticColliders(...)`
     - 再 `ConstrainNextPositionToNavigationBounds(...)`
     - 而 `ConstrainNextPositionToNavigationBounds(...)` 底层仍回到 `CanOccupyNavigationPoint(...)`
     - 所以复杂树根/屋角/围栏形状会放大坏相：脚底可过，不等于整个身体 clearance 真够。
  3. `NPC` 的动态避让半径仍偏紧：
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L404)
     - `GetAvoidanceRadius()` 现在会把 shell cap 在 `colliderRadius + 0.06f`
     - 这解释了为什么 `NPC` 更容易贴着障碍或彼此身体去磨。
  4. `Player` 的 caller 层静态 steering 明显更强：
     - [PlayerAutoNavigator.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs#L1218)
     - 玩家直接做多段 ahead probe + 更宽的 collider repulse
     - [PlayerAutoNavigator.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs#L209)
     - 玩家 avoidance 半径也没有 `NPC` 那个紧 cap。
  5. 牛这种“旁边看着没明显静态 blocker 还在上下转头抽搐”的 case，不像单纯漏收了静态 collider：
     - 更像 `destination claim / local avoidance / blocked recovery` 在近接触里来回打架
     - 再叠加逐步执行占位太窄，导致它一直判不出一个稳定脱困方向。
- 当前最稳判断：
  - “collider 形状位置不一”会影响 `NPC`，但不是用户场景做错，而是 `NPC` 这条执行链对复杂边界的 body-clearance 处理明显比玩家脆。
  - 用户截图里的牛 case 更像：
    1. `per-step occupancy / clearance` 太窄
    2. 再叠加 `pairwise local avoidance`
    3. 最后被 `blocked recovery` 放大成原地小幅抽搐
  - 这不是一句“再调大避让参数”就能彻底解决的层级。
- 当前恢复点：
  1. 后续如果继续开刀，导航 own 最值钱的一刀应继续压在：
     - `released 后 return-home / free-roam` 的逐步执行占位
     - 从脚底 probe 主导，收向 body-clearance-aware contract
  2. 不能把这轮只读结论包装成“live 体验已过线”
  3. 本轮未进真实施工，线程继续保持 `PARKED`

## 2026-04-15 12:17 真实施工：released 后 movement 的 per-step body-clearance contract 第一刀已落
- 当前主线目标：
  - 不碰 `Day1 phase`
  - 只修 `released 后 return-home / free-roam` 的逐步执行占位，让 NPC 更像玩家一样尊重静态边界和自身身体宽度。
- 本轮真实改动：
  - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs)
    - 新增 `ConstrainNextPositionToNavigationBoundsWithClearanceMargin(...)`
    - 以及配套的 axis / fallback helper
    - 作用：把“下一步位置约束”从纯脚底 probe 扩成可带 body-clearance margin 的版本
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
    - 新增 `ShouldUseReleasedMovementBodyClearanceExecutionContract()`
    - 新增 `GetReleasedMovementStepBodyClearanceExtraRadius()`
    - `ConstrainNextPositionToNavigationBounds(...)` 现在在：
      - autonomous free-roam
      - formal home return
      这两类 `released 后` movement 上，改走 clearance-aware helper
    - plain debug / resident scripted move 不吃这刀，避免误伤 staged contract
  - [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs)
    - 新增窄缝回归：
      - 旧脚底 probe 仍可判可站
      - body-clearance margin 会拒绝同一点
- 这刀为什么值钱：
  - 它直接打到了这轮只读结论里的第一失真点：
    - 热路径 per-step occupancy 仍偏脚底化
  - 同时又没把重查询塞回更大的热循环，只是在“下一步位置约束”这层加了更厚的 clearance 判定
- fresh 验证：
  - `validate_script NavigationTraversalCore + NPCAutoRoamController + NavigationAvoidanceRulesTests`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
    - 当前阻断仍是 Unity `stale_status`
  - `manage_script validate`
    - `NavigationTraversalCore=clean`
    - `NavigationAvoidanceRulesTests=clean`
    - `NPCAutoRoamController=warning`
    - 唯一 warning 仍是既有 `String concatenation in Update() can cause garbage collection issues`
  - `git diff --check -- <3 files>`
    - 通过
- 当前最准确状态：
  - 这刀代码层已站住
  - own red=0
  - 但 live 终验还没做，因为 Unity 仍被 `stale_status` 卡住
- 本轮 thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑`
  - `Park-Slice=已跑`
  - 当前 `PARKED`
- 当前恢复点：
  1. Unity 稳定后，直接回 `Town` 验：
     - released 后贴房/贴树/贴石头磨步是否下降
     - 牛圈/动物局部抽搐是否下降
  2. 如果仍有坏相，下一刀继续只收：
     - detour candidate distance
     - destination agent clearance
     - 不回漂 `Day1`

## 2026-04-15 15:08 补记：`NavigationAvoidanceRulesTests.cs` 那 3 个编译红已不再是当前现码问题
- 用户贴出的 3 个报错：
  1. `Component.enabled`
  2. `NavigationAgentRegistry` 找不到类型
  3. `Component -> string` 参数转换失败
- 当前代码里，这 3 个点都已经落成修后形态：
  1. `inactiveController.enabled` 已改成 `((Behaviour)inactiveController).enabled = false`
  2. 测试不再直接静态引用 `NavigationAgentRegistry`，而是通过 `ResolveTypeOrFail(\"NavigationAgentRegistry\") + reflection` 取 `GetRegisteredUnits<T>()`
  3. `Assert.That(... Does.Contain(...))` 这类不稳写法，当前用的是 `IList buffer` 上的 `buffer.Contains(controllerX)` 断言，不再有 `Component -> string` 的错误重载
- fresh 代码层验证：
  - `manage_script validate`
    - `NavigationAvoidanceRulesTests=clean`
    - `NavigationAgentRegistry=clean`
    - `NPCAutoRoamController=warning only`
  - warning 只有 `NPCAutoRoamController` 一条既有 GC 提示，不是编译红
- 当前 compile-first 现场仍不干净，但脏点不在我 own 的 3 个 CS 报错：
  1. `validate_script` 会被外部 console 噪音打断：
     - 一次是 `DialogueChinese V2 SDF.asset` import inconsistent
     - 一次是多条 `The referenced script (Unknown) on this Behaviour is missing!`
  2. `compile` 命令又被 `CodexCodeGuard/dotnet timeout` 挡住，没给出新的 owned error
- 额外处理：
  - 已用 `CodexEditorCommandBridge` 发 `STOP`，把 Unity 从卡住的 `PlayMode transition` 退回 `EditMode`
- 当前最准确结论：
  - 你贴出来的这 3 个 `CS1061/CS0246/CS1503`，现在不是当前 owned 代码还在报的活红
  - 当前 remaining blocker 是 Unity/console 外部噪音，不是这 3 个测试代码错误复发

## 2026-04-15 16:24 补记：已完成 Town 自漫游 live probe，两轮结果都说明“还不能叫导航正确”
- 本轮类型：
  - 只读 live 验证
  - 未进入新的代码施工
  - thread-state 继续保持 `PARKED`
- live 范围：
  - 场景：`Town`
  - 状态：Unity `Play Mode`
  - 探针：`Tools/Sunset/NPC/Run Roam Spike Stopgap Probe`
  - 目的：只测 `Town` 当前 25 个 roam NPC 的真实自漫游质量，不混 `Day1` staged 语义
- 我实际跑了 2 轮同条件 6 秒探针：
  1. 第 1 轮：
     - `npcCount=25`
     - `roamNpcCount=25`
     - `avgFrameMs=28.45`
     - `maxFrameMs=101.23`
     - `maxBlockedNpcCount=1`
     - `maxBlockedAdvanceFrames=1`
     - `topSkipReasons` 里仍有：
       - `ReusedFrameMoveDecision=290`
       - `ReusedFrameStopDecision=107`
       - `Stuck=19`
  2. 第 2 轮：
     - `npcCount=25`
     - `roamNpcCount=25`
     - `avgFrameMs=31.46`
     - `maxFrameMs=102.88`
     - `maxBlockedNpcCount=3`
     - `maxBlockedAdvanceFrames=2`
     - `topSkipReasons` 里仍有：
       - `ReusedFrameMoveDecision=159`
       - `ReusedFrameStopDecision=201`
       - `Stuck=21`
       - `SharedAvoidance=9`
       - `MoveCommandNoProgress=2`
- 这轮最准确判断：
  1. 现在已经不是“全员完全坏死”。
  2. 但也绝对还不能叫“导航正确了”。
  3. 理由不是空口感觉，而是：
     - 两轮都是 `25/25` roam
     - 仍持续出现 `Stuck / SharedAvoidance / NoProgress / 重复决策复用`
     - 峰值帧时间仍在 `~101-103ms`
  4. 这说明当前还有“局部互卡 / 原地抖动 / 短距反复修正”没有收干净。
- 额外现场：
  - 探针后 fresh `errors`：
    - `errors=0`
    - `warnings=0`
  - 所以这轮 live 的坏相不是被 fresh console 红错直接解释掉的。
- 当前恢复点：
  1. 下一刀仍该只收导航 own：
     - `destination agent clearance`
     - `local avoidance / repeated move-stop decision oscillation`
     - `detour candidate distance`
  2. 不把当前这轮 targeted probe 包装成“体验已过线”

## 2026-04-15 16:42 分析补记：用户把当前导航 own 问题压成 3 个主根，这个归纳是对的，我前几轮的刀法也确实没有一次切干净
- 用户当前三条归纳：
  1. `NPC 漫游目标点经常是坏点`
  2. `NPC 静态导航计算仍然垃圾`
  3. `NPC 阻塞卡死后不会自救`
- 当前重新对账后的判断：
  - 这三条归纳成立，而且比我前几轮“按现象拆刀”更接近主根。
- 我前几轮真实改了什么：
  1. 在 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 给 autonomous roam 加了候选评分，不再完全 first-fit 选第一个点。
  2. 增加了 `GetAvoidanceRadius()` 的壳体余量，把 `colliderRadius + 0.06` 放到 `+0.12`。
  3. 在 [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs) 和 `NPCAutoRoamController` 上补了 released movement 的 body-clearance-aware next-step constraint。
  4. 给 blocked advance / oscillation / retarget / decision reuse 加了一批止血逻辑。
  5. 用 [NavigationAgentRegistry.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs) 替掉了一部分 `FindObjectsByType` 全场扫描，主要是降放大后的性能噪音。
- 这些刀为什么没有把你的 3 个主根真正收掉：
  1. `坏点` 这块，我只是把“随机抽第一个点”改成“多个候选里挑相对更好一点的点”，但还没有把“所有目标点都必须先过 body-clearance + 远离静态边界”收成统一硬合同。
  2. `静态导航垃圾` 这块，我补的是“走每一步时更尊重身体宽度”，但路径/目标本身很多地方仍然先按偏点状、偏脚底化的逻辑成立，所以 route 仍会贴树、贴桶、贴墙。
  3. `不会自救` 这块，我补的是“更早放弃坏状态、减少抖动、尽量重试”，但还没有形成真正可靠的脱困闭环，所以碰到反复无解的小死局时，仍会原地磨和抽。
- 这轮从截图重新确认到的东西：
  1. 目标点和路径点仍然经常压在静态碰撞边缘附近。
  2. 身体盒已经贴上树/桶/墙了，系统还在尝试沿原方向挤过去。
  3. 这说明当前问题不是单点参数，而是：
     - 目标点选取
     - 静态 clearance 合同
     - 脱困流程
     这三层都还没闭环。
- 我当前更准确的自我评价：
  - 我没有“失去修复能力”，但我前几轮确实在止血、降爆炸和局部修正里绕了太久，没有直接按用户这三个主根来组织施工。
  - 这不是上下文太长的借口，是我这边刀法不够直。
- 现在最该落地的真实施工方向：
  1. `坏点`
     - 把 destination acceptance 升成“统一 body-clearance-first 合同”
     - 不只是 autonomous roam 候选
     - 还要覆盖 detour 点、重规划点、最终落点
  2. `静态导航`
     - 把“身体宽度”真正推进到路径成立层，而不是只在最后一步被动裁边
     - 目标是“不再先选一条要擦着树根/桶边/墙边过的路，再靠 steering 苟过去”
  3. `自救`
     - blocked 后不再只是抖动/重试
     - 而要明确区分静态卡死和动态互卡，进入短反向、侧移采样、临时黑名单、重规划这一套真正脱困流程

## 2026-04-15 17:02 子工作区补记：导航正式总交接文档已生成
- 本卷相关内容已被收进正式总交接文档：
  - [2026-04-15_导航线程总交接文档_own问题_边界语义_历史修改_后续接手.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-15_导航线程总交接文档_own问题_边界语义_历史修改_后续接手.md)
- 这份文档覆盖了：
  1. 当前子卷的核心结论
  2. 与 `Day1 / NPC` 的边界接缝
  3. 代码地图
  4. 时间线
  5. work package 级接手建议
- 当前恢复点：
  - 若后续继续施工，优先读总交接文档，不再只靠本卷局部记忆恢复上下文
