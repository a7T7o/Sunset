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
