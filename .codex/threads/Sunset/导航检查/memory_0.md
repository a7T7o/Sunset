# 导航检查 - 线程活跃记忆

> 2026-04-10 起，旧线程母卷已归档到 [memory_1.md](D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/导航检查/memory_1.md)。本卷只保留当前线程角色和恢复点。

## 线程定位
- 线程名称：`导航检查`
- 线程作用：导航父线程 / 审稿位 / 窄域问题复盘入口

## 当前主线
- 当前导航问题域已拆成 3 条：
  - `01_场景Traversal配置与桥水`
  - `02_跨场景Persistent重绑`
  - `03_NPC自漫游与峰值止血`

## 当前稳定判断
- 该线程不再把所有导航问题都写成一条长卷
- 后续父线程只保留：
  - 总判断
  - 分域路由
  - 审核/续工边界

## 当前恢复点
- 查旧 V2 审稿、旧 prompt、旧全局警匪定责材料时，看 `memory_1.md`
- 查当前 live 问题入口时，先看对应问题域 memory

## 2026-04-10 只读续记
- 本轮未进真实施工，未跑 `Begin-Slice`；只做了 `03_NPC自漫游与峰值止血` 的读图 + 代码对齐审计。
- 新稳定判断：
  - 用户新给的 profiler 图仍显示 `NPCAutoRoamController.Update -> Physics2D.OverlapCircleAll`，这与当前仓库 NPC roam live 源码对不上，优先按旧 binary / 旧采样看待。
  - 用户反馈的“持续往左移动但朝向左右翻”则是当前 live 真问题，且更像 `NPCAutoRoamController` 给 `NPCMotionController` 的 facing source 仍吃原始 desiredDirection，而不是最终提交速度。
- 如果下一轮继续这条线，先只砍“朝向跟最终有效移动决策”这一刀；不要再把 profiler 旧链和当前朝向抖动混报。

## 2026-04-10 审核补记
- 已复核最新代码：`NPCAutoRoamController` 当前确实把 auto-roam 朝向改成了优先跟最终提交 velocity，这一刀不是空口。
- 但这还不是“全 NPC 运行时朝向闭环”：
  - `SpringDay1NpcCrowdDirector.cs:1696-1707` 在 `StartRoam()` 后仍直接 `StopMotion() + SetFacingDirection(state.BaseFacing)`
  - 另外还有多处 director/crowd 运行时直接写 `SetFacingDirection(...)`
- 所以当前最准确的审稿结论是：
  - 这刀修正了 auto-roam 自身的一个真根因
  - 但当前项目里仍存在多个外部朝向 owner，会继续造成体验不稳定

## 2026-04-10 认知同步补记
- 本轮继续保持只读，未进真实施工。
- 当前线程的新稳定判断：
  - 用户真正不满意的是 NPC 静态世界导航能力没有对齐玩家，而不是单独某个朝向 bug
  - 底层 shared traversal kernel 不是主锅；更像 NPC caller-level contract / owner / roam domain 没收平
  - 当前最该验证的不是“还有没有旧 profiler spike”，而是“坏 NPC 是否持续抽到不该去/去不到的目标点，以及进入坏 case 后是谁继续接管它”

## 2026-04-10 prompt_64 只读续记
- 本轮仍未进真实施工，未跑 `Begin-Slice`；严格按 `prompt_64` 做第二轮窄验证。
- 新稳定判断已经压到可直接发第一刀施工 prompt：
  1. `Town` 居民代表 `101` 的第一失真点，不在 `walkable / rebuild / avoidance`，而在 `坏目标采样`：
     - 当前实例位置约 `(-7.65, 14.88)`，但绑定 `101_HomeAnchor` 在 `(28.5, 39.1)`。
     - `NPCAutoRoamController.GetRoamCenter()` 会优先吃 `homeAnchor`，所以 free-roam 的采样圆心从第一步就错了。
  2. `小动物` 组里的 `Baby Chicken Yellow / Chicken Red` 的第一失真点，同样先在 `坏目标采样`：
     - prefab 没有 `homeAnchor`，只会围绕出生点做 raw 半径采样。
     - scene 虽把 `fence / border / tree / rock` 绑定成静态阻挡，但没有围栏内语义域；于是采样点可以先落到 fence 边或 fence 外侧，再被 `TryResolveOccupiableDestination / TryFindNearestWalkable` 改写成“物理上可走、语义上不该去”的点。
- 所以这两个 case 的共同大根是：
  - `NPC autonomous roam 的目标域 contract 没有语义化`
  - 不是“先到 avoidance 才坏”
  - 也不是“必须先归咎于 owner 抢写”
- 但 `NPC vs Player` 的 `clearance / avoidance / blocker` 差异也已实锤：
  - 这层能解释为什么 NPC 更容易把 bad sample 放大成乱顶 / rebuild / storm
  - 但本轮判断它不是这两个 static bad case 的第一失真点
- 当前下一轮最值钱的第一刀：
  - 只修 `NPC autonomous roam 的目标域 contract`
  - 不顺手扩成 `facing owner`、`统一重构` 或 `泛性能止血`

## 2026-04-10 第一刀施工续记
- 本轮后半段已从只读转入真实施工，并已自行跑：
  - `Begin-Slice=已跑`
  - `Park-Slice=已跑`
- 实际只改：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
- 我在这一刀里收了两件事：
  1. roam center 不再直接无脑吃错位 `homeAnchor`
  2. autonomous roam 采样点如果要被大幅纠偏成另一个最近可走点，直接丢弃，不再进入建路
- 代码层验证：
  - `validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
    => `unity_validation_pending / owned_errors=0 / external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 通过
- 当前最准确状态：
  - 代码层这刀已落地且未见 owned red
  - Unity fresh live runtime 证据仍待补，因为本机 MCP endpoint 当前拒连

## 2026-04-10 续工：NPC倒走/摆头快修
- 用户新主诉：`NPC 自漫游出现倒着走 + 朝向左右翻`；剧情/回 anchor 链路相对正常。
- 本轮先只读快验，后真实施工：
  - 先跑 Town live quick probe（菜单 `Tools/Sunset/NPC/Run Roam Spike Stopgap Probe`）
  - 探针结果显示无路径重建风暴，问题聚焦到朝向链
- 本轮真实施工仅改：
  - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
- 具体修复：
  1. `SetFacingDirection` 增加移动驱动保护：外部速度存在时，拒绝与移动方向相反的强制朝向。
  2. `ResolveFacingVelocity` 增加反向纠偏：当 `externalVelocity` 与 `observedVelocity` 明显反向（dot <= -0.35）时，朝向改跟真实位移。
- fresh 代码闸门：
  - `validate_script Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs --count 20 --output-limit 5`
    => `assessment=no_red / owned_errors=0 / external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs` 通过
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑`
  - `Park-Slice=已跑`
  - 当前 `PARKED`

## 2026-04-11 read-only：Town 常驻 NPC 自漫游坏相分层钉死
- 当前主线仍然是：
  - `Town` 常驻 NPC autonomous/free-roam 的避障回退 + 朝向乱飘
- 本轮子任务：
  - 不再继续改导航代码
  - 先按 `prompt_66` 把第一分叉点钉死：哪些坏相属于导航 own，哪些已经落到外部 owner
- 本轮只读做成的事实：
  1. `101~301` 这组常驻居民属于 `SpringDay1NpcCrowdManifest`
  2. `001~003` 不在 crowd owner 链里
  3. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L1730) `ApplyResidentBaseline(...)`
     - 在居民已交回 roam 后仍执行 `motionController.StopMotion()` + `SetFacingDirection(state.BaseFacing)`
  4. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L1841) `TickResidentReturnHome(...)`
     - 也在持续写 `SetFacingDirection(step)`
  5. `py -3 scripts/sunset_mcp.py status`
     - fresh console 命中：
       - `104 -> facingSource=SpringDay1NpcCrowdDirector.ApplyResidentBaseline`
       - `103/202 -> facingSource=SpringDay1NpcCrowdDirector.TickResidentReturnHome`
       - 鸡/牛的 mismatch 来源仍是 `NPCAutoRoamController.ApplyMoveVelocity`
- 这轮核心判断：
  - `Town` 里最严重的常驻居民朝向乱飘，第一真 blocker 已经落到 `SpringDay1NpcCrowdDirector`
  - `001~003` 的避障回退与小动物的 face lag 仍留在导航 own / motion own，不该继续和 crowd owner 混报
- 额外验证事实：
  - 我尝试用 `NpcRoamSpikeStopgapProbe` 跑 Town live
  - 但 Play 起场景被 guard 导向了 `Primary`
  - 因此本轮没有 fresh Town 体验证据，只拿到了 owner/log 层硬证据
- 当前阶段：
  - 线程保持 `PARKED`
  - 这轮没有进入新的真实施工，所以没有再跑新的 `Begin-Slice / Ready-To-Sync`
  - 当前恢复点是：
    1. 若继续留在导航线程，只剩 `001~003` 的避障回退与小动物 face lag 可继续窄验
    2. 若要收 `101~301` 的 severe 摆头/倒走，需转给 `SpringDay1NpcCrowdDirector` owner

## 2026-04-10 续工：NPC完好系统收口
- 用户把目标重新钉死为：给回一个“完好的 NPC 系统”，并且必须保证不要再出现性能炸弹。
- 本轮真实施工继续只改：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
  - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
- 我刻意没有去碰当前其它线程已 dirty 的：
  - `SpringDay1NpcCrowdDirector.cs`
  - `SpringDay1Director.cs`
- 本轮新增两层核心修法：
  1. `NPCMotionController`
     - 新增移动期朝向对齐阈值；
     - locomotion 期间，misaligned `SetFacingDirection(...)` 直接忽略；
     - `_externalFacingDirection` 与真实位移偏离过大时，朝向直接回 observed movement。
  2. `NPCAutoRoamController`
     - 对静态坏 case（`ConstrainedZeroAdvance / MoveCommandOscillation / MoveCommandNoProgress / ZeroStep`）新增更早熔断；
     - 同点连续命中 2 次且无动态 blocker 时，不再 rebuild，直接结束本次 move cycle。
- fresh 代码闸门：
  - `validate_script NPCAutoRoamController.cs + NPCMotionController.cs`
    => `assessment=no_red / owned_errors=0 / external_errors=0`
  - `git diff --check` 针对两脚本通过
- fresh Town 稳态 live：
  - 先静置 10 秒，再跑 `Tools/Sunset/NPC/Run Roam Spike Stopgap Probe`
  - 结果：
    - `avgFrameMs=0.887`
    - `maxFrameMs=15.256`
    - `maxGcAllocBytes=37197`
    - `maxBlockedNpcCount=0`
    - `maxConsecutivePathBuildFailures=0`
- 当前判断：
  - 这轮已经把“视觉鬼畜”与“静态坏 case 性能放大”同时往下压了一层
  - 仍未闭环的只剩用户现场视觉体验终验
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑`
  - `Park-Slice=已跑`
  - 当前 `PARKED`

## 2026-04-10 续工：NPC鬼畜朝向诊断输出
- 用户最新判断非常关键：
  - 剧情走位和回 anchor 路上基本正常
  - 真正鬼畜的是 free-roam / 自漫游
- 因此本轮没有继续盲修功能，而是先给 `NPCMotionController` 补运行态证据链：
  - `SetFacingDirection / SetExternalVelocity / SetExternalFacingDirection`
    现在都自动记录调用源（通过 caller info）
  - 当 `observedDir != appliedDir` 连续出现时，会输出：
    - `observedDir`
    - `appliedDir`
    - `observedVelocity`
    - `facingVelocity`
    - `externalVelocity`
    - `externalFacing`
    - `facingSource`
    - `externalVelocitySource`
    - `externalFacingSource`
    - `roamState / moveSkip / blockedFrames`
- 日志前缀：
  - `[NPCFacingMismatch]`
- fresh 验证：
  - `validate_script Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs --count 20 --output-limit 5`
    => `assessment=no_red / owned_errors=0 / external_errors=0`
  - Town 下 fresh 跑 12 秒，当前没有打出 `[NPCFacingMismatch]`
- 当前判断：
  - 诊断输出已挂好
  - 但用户描述的坏 case 没在这次 Town 短窗口里复现
  - 下一次只要在 `Primary` 或更长 free-roam 现场复现，Console 会直接给出真正写入朝向的 owner

## 2026-04-10 续工：一步一回头坏相补抓与朝向噪声止损
- 用户把当前最关键坏相钉死为：
  - `NPC` 并没有往回走
  - 而是持续同向位移时，脸在左右乱翻
- 我本轮重新开 slice，只改：
  - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
- 具体落地：
  1. `[NPCFacingMismatch]` 从“只抓连续 mismatch”升级成“也抓 burst mismatch”，不再漏掉间歇性左右翻脸。
  2. `ResolveFacingVelocity(...)` 对 observed-facing override 增加确认窗口；短暂一帧噪声不再立刻改脸。
  3. `showDebugLog` 现在会直接输出 `ObservedDir / AppliedDir / ExternalFacing / FacingSource / ExternalVelocitySource / ExternalFacingSource`。
- fresh 自检：
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs --count 20 --output-limit 5`
    => `assessment=no_red / owned_errors=0 / external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs` 通过
  - fresh console = `0 error / 0 warning`
- 这轮主判断：
  - 之前那版诊断确实可能漏掉用户说的“一步一回头”
  - 这轮已经把“抓不到”和“一帧噪声就改脸”这两个漏洞一起补上
- 当前恢复点：
  - 下一步优先让用户在真实 free-roam 坏 case 下看 Console
  - 如果仍翻脸，就直接按日志里的 owner 继续砍，不再回到泛猜阶段
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑`
  - `Park-Slice=已跑`
  - 当前 `PARKED`

## 2026-04-11 续工：resident scripted control 冻结链 exact fix
- 用户先批准按 prompt_65 开工，但随后补充了新的主现场：
  - `Town`
  - `001~003` 走路基本稳，但避障回退
  - 其它常驻 NPC / 小动物主要坏在 autonomous / free-roam
- 我先按 prompt_65 的 scope 做 exact 定责，结论是：
  - `resident scripted control` 这条链里确实有一个 own bug
  - 但它不是 Town 自漫游主痛点的全部答案
- exact own bug：
  - [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs#L529) Acquire resident control
  - [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs#L534) 直接把 `roamController.enabled = false`
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L543) `OnDisable()` 旧逻辑会 `StopRoam()`
  - `StopRoam()` 会清掉 `resumeRoamAfterResidentScriptedControl`
  - 导致 release 后可能不再恢复 free roam
- 本轮真实施工只改：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
- 最小修补：
  - `OnDisable()` 在 resident scripted control 持有期间改走 `HaltResidentScriptedMovement()`，不再清空 release 后恢复 roam 的意图
- fresh 自检：
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
    => `assessment=no_red`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 通过
- 这轮新的主判断：
  - prompt_65 不是完全错，它确实打中了一个 own 漏洞
  - 但用户当前真正最痛的主线已经转成 Town 常驻 NPC 的 `避障回退 + 朝向乱飘`
- 已补下一轮 prompt：
  - [2026-04-11_导航线程_Town自漫游避障回退与朝向乱飘收口prompt_66.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-11_导航线程_Town自漫游避障回退与朝向乱飘收口prompt_66.md)
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑`
  - `Park-Slice=已跑`
  - 当前 `PARKED`

## 2026-04-11 续工：Town 自漫游避障回退与朝向乱飘同根修补
- 用户进一步补充：
  - `001~003` 走路基本稳，但避障没了
  - 其它常驻 NPC 路径对、脸乱飘
  - 小动物也疑似坏
- 我这轮继续只改：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
- 当前新的主判断：
  - 这不是“避障”和“朝向”两条完全独立的线
  - 更像 shared avoidance 看到的是“意图速度”不是“现实速度”，导致：
    1. 交通判断不准，避障回退
    2. 接触/挤压增多，朝向层开始乱飘
- 本轮补了两处：
  1. `GetCurrentVelocity()`：
     - 优先 `motionController.CurrentVelocity`
     - 再 `rb.linearVelocity`
     - 最后才 `ReportedVelocity`
  2. `IsNavigationSleeping()`：
     - 不再只看 `state`
     - 现实速度几乎为 0 也视作 sleeping / blocker
- fresh 验证：
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
    => `assessment=no_red`
  - `git diff --check` 通过
- 还没闭环的点：
  - 这轮没拿 fresh Town live 体验证据
  - 小动物是否完全同根，我目前只到“高度可疑同根”
- 当前阶段：
  - own 侧已经补了两处现实层根因：
    1. resident scripted control 的 disable/release 漏洞
    2. shared avoidance snapshot 的现实速度语义漏洞
  - 但 Town 体验层仍待下一次 fresh live 看结果
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑`
  - `Park-Slice=已跑`
  - 当前 `PARKED`

## 2026-04-11 真实施工：Town resident crowd owner 改走 resident scripted control
- 当前主线恢复点：
  - 用户批准“这一轮直接全部做完”，但明确不要整仓回滚，要保留当前玩家 traversal/bridge/water/bounds 与性能版稳定性。
  - 本轮第一正手是 `SpringDay1NpcCrowdDirector`，不是再去动玩家 navigation core。
- 本轮子任务：
  - 把 `Town` 常驻 resident 的 crowd owner 冲突真正落地修掉。
- 实际施工：
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
  - 新增 `ResidentScriptedControlOwnerKey`
  - 新增 `PrepareResidentRoamController / AcquireResidentDirectorControl / ReleaseResidentDirectorControl`
  - 改了：
    - `ApplyResidentBaseline(...)`
    - `ApplyResidentRuntimeSnapshotToState(...)`
    - `ResetStateToBasePose(...)`
    - `SnapResidentsToHomeAnchorsInternal(...)`
    - `TryBeginResidentReturnHome(...)`
    - `FinishResidentReturnHome(...)`
    - `CancelResidentReturnHome(...)`
    - `ApplyResidentNightRestState(...)`
- 施工判断：
  - baseline free-roam 不再反复 `StopMotion + SetFacingDirection(BaseFacing)` 抢姿态
  - resident return-home / night-rest / reset / snapshot 现在都有显式 owner acquire/release
  - 也就是把 `Town` 常驻 resident 的导演控制权重新并回 `NPCAutoRoamController` 既有 contract
- fresh 验证：
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs --count 20 --output-limit 5`
    - `owned_errors=0`
    - `assessment=external_red`
    - external blocker = `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs` 通过
  - `Town Runtime Anchor Readiness Probe` => `completed`
  - `Town Player-Facing Contract Probe` => `attention`，仅入口第一屏群像视野提示，不是当前 blocker
- live 阻塞：
  - 已把编辑器切到 `Town` 并尝试 Play
  - Play 没能稳定进入；当前仍被外部 `SpringDay1LateDayRuntimeTests.cs` 编译红挡住
  - 所以这轮还没有 fresh Town resident 体验层闭环
- 当前恢复点：
  - crowd owner severe 已经真正下刀
  - 下一步只剩：等外部 compile blocker 清掉后，直接跑 Town fresh live，决定 `001~003` 避障回退/小动物 face lag 是否还要补第二刀

## 2026-04-12 只读续查：Town / Day1 “NPC 不动 / 像冻结”更像 Day1 caller 持有 resident scripted control，不是导航 own 丢了 resume
- 当前主线目标：
  - 回答用户“到底出了什么问题”，把 `Town` 当前那批人类 NPC 站住不动、以及 console 里 `NPCFacingMismatch` warning 的语义分开
  - 本轮仍是只读分析，不进入真实施工
- 本轮新结论：
  1. `NPCAutoRoamController` 自己存在完整的 `AcquireResidentScriptedControl -> ReleaseResidentScriptedControl -> StartRoam()` 恢复路径。
     - `ReleaseResidentScriptedControl(...)` 在 owner 清空、`resumeRoamAfterResidentScriptedControl` 为真、且不在对白暂停时，会直接 `StartRoam()`
     - `Update()` 里也有同条件的二次兜底
  2. 因此，Town 当前“人类居民像冻结”不优先像导航 own 没恢复，而更像上层剧情仍在持有、或反复重拿 resident scripted control。
  3. `[NPCFacingMismatch]` 来自 `NPCMotionController.EmitFacingMismatchDiagnostics(...)`，是编辑器诊断 warning，不是“NPC 现实完全不动”的直接根因。
- 当前最像的 caller 分层：
  - `001/002`：
    - 被 `SpringDay1NpcCrowdDirector.ShouldDeferToStoryEscortDirector(...)` 特判
    - 在 `EnterVillage / HealingAndHP / WorkbenchFlashback / FarmingTutorial / DinnerConflict / ReturnAndReminder` 阶段直接走 `AcquireResidentDirectorControl(..., false)`
    - 这组本来就归剧情 escort，不是普通 free-roam
  - `101~301` 常驻居民：
    - 更像被 `SpringDay1NpcCrowdDirector.ApplyStagingCue(...)` + `SpringDay1DirectorStagingPlayback` 持有
    - `ApplyCue()` 若 `cue.suspendRoam == true`，会通过 `SpringDay1DirectorNpcTakeover.Acquire()` 抢 resident control
    - 只有 `ClearCue()/Release()` 真发生后，才会回到 `ApplyResidentBaseline(...)` 的 release / return-home / roam 恢复链
- 和用户截图的对齐判断：
  - 截图里的文案能对上 `SpringDay1Director` 里 `StoryPhase.EnterVillage` 的提示分支
  - 所以当前更像 `EnterVillage / HouseArrival` 还没走完，Town 居民整片停位先应视为剧情 staging，而不是直接判“导航系统死了”
- 当前最值钱的下一步（如果继续只读）：
  - 不再追 `NPCFacingMismatch`
  - 只去钉死当前 live 那一屏到底是：
    1. 正常 `EnterVillage` staging
    2. 还是 beat/cue 已该结束，但 `ApplyStagingCue`/`DirectorNpcTakeover.Release()`/`ApplyResidentBaseline` 没真正放人
- 这轮报实：
  - 未改代码
  - 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`，因为本轮停留在只读分析
  - 当前判断已越过“还没定位”；现在更像 Day1 caller 责任链，而不是导航 own first-truth

## 2026-04-12 提交审计：导航检查线程当前无 repo 内 own changes 可合法 sync
- 用户要求我把“自己能提交的都提交掉”
- 本轮按当前 thread-state 规则做了最小收口核对：
  - `Begin-Slice` 已跑，白名单只登记：
    - `.kiro/specs/屎山修复/导航检查/memory.md`
    - `.codex/threads/Sunset/导航检查/memory_0.md`
  - 之后对这两个 own roots 跑了：
    - `git status --short -- <own roots>`
    - `git diff -- <own roots>`
    - `git log -- <own roots>`
- 结论：
  - 当前这两个 repo 内 own roots 没有待提交差异
  - 因此这轮不存在“我自己能合法归仓但还没提”的 repo 改动
  - 仓库里剩余海量 dirty/untracked 仍主要在 `Assets` / `ProjectSettings` / 其他线程范围，不属于导航检查线程可安全吞并的白名单
- 额外现场：
  - 我尝试自测 Town/NPC live 时，Play 首先被外部 compile red 挡住
  - 第一外部 blocker 现在落在 `spring-day1/UI` 的 [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
  - 所以当前不是“导航这边有东西没提交”，而是“导航 live 还被别的线程红面挡着”
- 收口：
  - 本轮未做空提交
  - 因为没有进入真实 sync，未跑 `Ready-To-Sync`
  - 已跑 `Park-Slice`

## 2026-04-13 Town 只读监测：其他居民不是没接避让，而是和 001~003 处在不同 owner/roam 包装层
- 用户这轮要求：
  - `mcp` 已挂好，只读获取控制台和当前对象状态，回答“为什么只有 001~003 的避让明显，其他 NPC 像没接上一样”
- 这轮执行：
  1. 读取当前 `Town` scene / editor state / console
  2. 读取 `001 / 002 / 003 / 101 / 103 / 201` 的 GameObject 与组件详情
  3. 对照 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)、[SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)、[SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs)
- 压实的证据：
  1. 当前 `Town` console 没有新的导航/避让报错，最新只剩普通 `CloudShadowManager` log。
  2. `Town` 里共有 `25` 个 `NPCAutoRoamController`，明确不是只有 `001~003` 接了 roam/avoidance。
  3. `001 / 002 / 003 / 101 / 103 / 201` 都挂着：
     - `NPCAutoRoamController`
     - `NPCMotionController`
  4. `101 / 103 / 201` 的 `avoidanceRadius / priority / lookAhead / repathCooldown` 与 `001~003` 一致，所以不能判成“其他居民没接避让”。
  5. 真正的结构差异：
     - `001~003` 在 `NPCs/*`
     - `101~301` 在 `SCENE/Town_Day1Residents/Resident_DefaultPresent/*`
     - 并额外挂 `SpringDay1DirectorNpcTakeover`
     - `101` 的 `NPCMotionController.DebugLastFacingWriteSource` 还记录到了 `SpringDay1NpcCrowdDirector.ConfigureBoundNpc`
  6. roam profile 也不同：
     - `001~003 activityRadius` 约 `1.55 ~ 1.85`
     - `101 / 103 / 201 activityRadius = 3.0`
     - 所以后面那批更容易采到更远、更差的目标点
- 当前判断：
  - 第一结论：不是“没接给其他 NPC”
  - 更像是“同一套 roam/avoidance 核已经接上了，但 Town resident / Day1 crowd owner 这一层只包在后面那批居民上，再叠加更大的 roam 半径，所以表现和 001~003 明显分层”
  - 也就是说，这轮最值钱的只读结论是：问题更像 `owner + roam target/state` 分层，不像 `avoidance wiring` 缺失
- 证据边界：
  - 我收尾时 Unity 已回到 `Town` Edit 模式，所以这轮最终不是新的 unpaused movement proof
  - 但对“是不是根本没接给其他 NPC”这个问题，当前证据已经足够回答：`不是`
- 本轮报实：
  - 全程只读，未改代码
  - 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`，因为没有进入真实施工

## 2026-04-13 day1 fresh 同步接入后，静态障碍与冻结问题已能分锅
- 用户补充了 day1 最新同步：
  - [2026-04-12_给导航_002后TownResident冻结根因与当前修复同步.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/2026-04-12_给导航_002后TownResident冻结根因与当前修复同步.md)
  - 核心结论是：`002` 后 `Town resident` 整片冻结已被 fresh probe 钉到 `SpringDay1NpcCrowdDirector` 的 runtime registry 恢复漏口，并已补 Town resident rebind 恢复刀；用户 live 已口头反馈“npc 可以走了”
- 结合这轮继续只读得到的新判断：
  1. `Town resident` 冻结与“树/房屋静态导航算进碰撞体内部”不是同一个锅。
  2. 对静态障碍，当前第一真问题不是 tag 没挂。
     - 我抽查的树/房屋子物体已有 `PolygonCollider2D`
     - [NavGrid2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs#L2287) `HasAnyTag(...)` 也会沿父链查 tag
  3. 更准确地说，当前坏相更像 shared traversal core 的 clearance 口径过小：
     - NPC `navigationFootProbeVerticalInset / SideInset / ExtraRadius = 0.08 / 0.05 / 0.02`
     - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs#L520-L534) 把 `CanOccupyNavigationPoint(...)` 查询半径压到约 `0.07~0.10`
     - 但 NPC 动态避让半径仍是 `0.6`
     - 于是路径/占位是在用“脚底小探针”判可走，不是在用真实身体 clearance 判可走
  4. 同时 `Town` 当前仍处于 `TraversalBlockManager2D` 的显式阻挡源模式：
     - 显式数组存在
     - `autoCollectSceneBlockingColliders = false`
     - 所以 tag 不是万能入口；真正决定 NavGrid 阻挡的是 collider/source 是否被真实命中或显式纳入
  5. 小动物/NPC 卡在小区域后“不动”的问题，也不是完全没有恢复逻辑：
     - roam 当前已有 `MoveCommandNoProgress / CheckAndHandleStuck / blockedAdvance -> rebuild / reroute` 骨架
     - 只是对静态窄 clearance 场景恢复太保守，太容易退成 stop/pause
- 当前线程判断：
  - `spring-day1` 那边的 resident freeze 先不再归我这边泛修
  - 导航 own 这边真正该接的，是：
    - static clearance 判定太小
    - stuck/blocked 低成本重规划不够积极
- 本轮报实：
  - 只读分析，未改代码
  - 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`，因为没有进入真实施工

## 2026-04-13 真实施工：开始修静态障碍 clearance 与低成本 reroute
- 用户明确批准：
  - “我认为你的思考深度到位了，请你直接开始落地修复”
- 本轮已进入真实施工：
  - `Begin-Slice` 已跑
  - `CurrentSlice = navigation-static-clearance-and-lowcost-reroute`
  - 白名单：
    - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs)
    - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
    - 以及本线程/工作区 memory
- 这轮实际改动：
  1. `NavigationTraversalCore.CanOccupyNavigationPoint(...)`
     - 补 expanded static clearance 二次校验
     - 让静态 occupancy 不再只按极小脚底半径判定
  2. `NPCAutoRoamController`
     - `TryHandleBlockedAdvance(...)` / `CheckAndHandleStuck(...)` / `TryHandlePassiveAvoidanceHardStop(...)`
     - 都改成优先尝试 `TryBeginMove(...)` fresh reroute，再 fallback 到 stopgap
     - 新增 `AUTONOMOUS_STATIC_ABORT_MIN_FRAMES = 6`
- 当前验证：
  - `validate_script NavigationTraversalCore.cs` => `errors=0 warnings=0`
  - `validate_script NPCAutoRoamController.cs` => `errors=0`，仅 1 条旧 GC warning
  - `refresh_unity(scope=scripts, compile=request, wait_for_ready=true)` 已跑
  - fresh `read_console(error+warning)` => `0`
- 当前判断：
  - 这刀已经真正落地，不再是分析稿
  - 但还欠一层 live roam 体验复测，尤其是：
    - 树/屋角贴边是否明显缓解
    - 动物/普通 NPC 在小区域互卡后是否更愿意 fresh reroute 而不是直接站住
- 当前 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：待执行
  - `Park-Slice`：未跑

## 2026-04-13 施工收口：代码 clean，sync 被 own-root 历史脏改阻断，线程已停车
- 本轮最终结果：
  1. 代码修复已落地：
     - `NavigationTraversalCore` expanded static clearance
     - `NPCAutoRoamController` blocked/stuck 优先 fresh reroute
  2. 最小验证已过：
     - `validate_script` 通过
     - Unity fresh console 空
  3. `Ready-To-Sync` 结果：
     - `BLOCKED`
     - 原因不是本轮脚本编译红
     - 而是 thread-state 把这轮 own roots 解析成：
       - `Assets/YYY_Scripts/Controller/NPC`
       - `Assets/YYY_Scripts/Service/Navigation`
       - 这两个根下本来就还有别的历史 dirty/untracked
  4. 因此我没有伪装成“已可直接提交”，而是立即跑了 `Park-Slice`
- 当前 live 状态：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：已跑，`BLOCKED`
  - `Park-Slice`：已跑
  - 状态：`PARKED`

## 2026-04-11 只读分析：朝向已基本收住后，Town 当前“顶墙 / 避让像没了”并不等于静态导航消失
- 用户最新目标：
  - 要我彻底讲清：现在朝向已经对了，但 NPC 还会撞墙、避让像没了；到底是静态导航不存在，还是别的层出了问题；以及下一刀能不能做到“历史可用体验 + 当前性能稳定”。
- 本轮只读结论：
  1. `Town` 当前不是“没有静态导航”。
     - `NavGrid2D + TraversalBlockManager2D + NavigationTraversalCore` 这条静态 traversal contract 仍在
     - `shared avoidance / local avoidance` 也仍在，但它本来就只负责动态单位，不负责静态墙体
  2. 当前更像是“静态阻挡 source / occupiable target / 自漫游半径”三层没有重新完全对齐。
  3. 001~003 看起来更稳，不是因为它们还有另一套避障系统；而是它们的 `activityRadius` 明显更小（1.55~1.85），比 101~301 那组常驻（普遍 3.0）更少采到坏目标。
  4. 小动物沿用同一套 autonomous roam / shared avoidance 栈，只是半径不同，所以大概率同根，不是全新问题。
- 这轮压实的新证据：
  - [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 当前确实还挂着：
    - `blockingTilemaps = Layer 1 - Wall + 4 组 composite tilemap source`
    - `blockingColliders = Layer 1 - Props 系列 + Layer 1 - Farmland_Water + 4 组 composite`
  - 但 [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs#L465) 的逻辑决定了：
    - 只要这些显式数组非空，scene 就进入 allow-list 模式
    - 之后新增静态 collider/tilemap 不会靠 auto collect 自动接入
  - 同时 `Town Runtime Anchor Readiness Probe` 只证明 anchor/slot/home-anchor 存在且在 bounds 内，**没有**证明这些点对 `CanOccupyNavigationPoint(...)` 真可站立
- 当前最像的第一真问题：
  - `HomeAnchor / DailyStand / ResidentSlot / roam center` 里有一部分点贴墙或压在 blocked edge 邻域
  - 半径更大的 resident/autonomous roam 更容易采到这些点
  - shared avoidance 对墙无能为力，所以最后表现成“方向对，但一直顶墙 / 像没有避让”
- 历史线重新确认：
  - 你记得的“NPC 避障有过够用窗口”不是错觉
  - memory 里保留着 2026-03-29 的用户正反馈：`好了非常多、到了可以用的地步`
  - 所以下一刀不是重发明系统，而是把那个“够用窗口”的体验重新和当前性能止血版对齐
- 下一步恢复点：
  - 保留当前性能 hardening，不回滚
  - 第一刀先做 `Town` 问题 NPC / 小动物的 `occupiable target` 验证
  - 如果 target 非法，再做最小目标修正与 roam 采样收紧
  - 只有这两层都站住后，才继续追 dynamic avoidance 是否还要补一刀
## 2026-04-13 导航检查线程只读自查：当前主线已切回“导航问题总分层”，不是继续盲修
- 用户最新明确要求：
  - 暂不继续施工，先给出一份完整自查：现在导航还没做好的问题有哪些、用户真正需要的导航是什么、下一步解决清单是什么
- 本轮前置核查：
  - 已按 `skills-governor` 做 Sunset 手工等价启动核查
  - 已联动 `user-readable-progress-report` 与 `delivery-self-review-gate`
  - 因任务涉及截图/体验判断，又补走了 `preference-preflight-gate` 的手工等价流程并读取 `global-preference-profile.md`
  - 本轮保持只读，未跑 `Begin-Slice`
- 本轮新增稳定结论：
  1. 当前不能再说成“导航全坏”，必须拆成 3 类：
     - `spring-day1 owner` 的 resident crowd registry 恢复漏口
     - `导航 own` 的静态 clearance / blocker source / 坏目标采样
     - `高密度 crowd/动物` 的 reactive avoidance 不够
  2. 导航 own 这边已修掉一部分，但还没有闭环：
     - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs#L148-L200) 已加 expanded static clearance
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1638-L1708) 与 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L3482-L3633) 已把 blocked/stuck 恢复顺序调成优先 fresh reroute
     - 但 `autonomous roam` 坏目标采样与 `Town` blocker source 审计还没做
  3. `Town` 当前不是“tag 挂了就一定自动进静态导航”：
     - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs#L465-L532) 证明显式 source 一旦存在，就会压过 auto collect
  4. 当前用户要的目标导航，我压成一句话就是：
     - 玩家 / NPC / 动物都共用一套 traversal contract；不会钻碰撞体、不会贴障碍硬挤；卡住会低成本重规划；高密度时宁可稳妥绕开或短暂停，也不要鬼畜、站死、来回抖
  5. 当前最值钱的下一刀排序：
     - `Phase 1` 保留已落性能 hardening，不回滚
     - `Phase 2` 收 autonomous roam sampling 到同一可达域/enclosure
     - `Phase 3` 做 `Town` blocker source scene audit，确认显式名单覆盖完整
     - `Phase 4` 若仍有密集互卡，再补更强 jam breaker
- 证据边界：
  - 用户之前贴的 temp 图片路径这轮已失效，无法重新打开
  - 但基于现有代码、memory、day1 fresh 同步和用户口头现场描述，以上分层已足够支撑决策
- 本轮报实：
  - 只读分析，未改代码
  - 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
  - 当前线程 live 状态应继续视为 `PARKED`

## 2026-04-13 导航检查线程恢复真实施工：enclosure 过滤 + Town blocker source
- 用户后续继续放行，并明确：
  - 允许我彻底开始
  - 允许使用 subagent
  - 当前优先级最高
- 本轮我实际做了什么：
  1. 跑了 `Begin-Slice`
     - slice = `navigation-enclosure-and-town-blocker-hardening`
  2. 并行开了 2 个 explorer：
     - 一个只读钉 `Town` blocker source 为什么漏新增 tilemap/composite collider
     - 一个只读钉 autonomous roam 坏目标采样的第一失真点
  3. 自己落代码：
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1552) 新增 built-path detour/enclosure 过滤
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L2339) 新增 destination 邻域 clearance 过滤
     - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs#L494) / [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs#L523) 改成 manual + auto collect 合流
  4. 用 Unity MCP 读取 live `Town` 场景中的 `TraversalBlockManager2D`，确认 `autoCollectSceneBlockingColliders` 已切成 `true` 并保存回 [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity#L180421)
- 当前验证：
  - `validate_script`：
    - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) `errors=0`，仅 1 条旧 GC warning
    - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs) `errors=0 warnings=0`
  - Unity fresh console：
    - 无新的导航红错
    - Play/Stop 短检后仅 2 条外部 warning：MCP transport / TMP runtime atlas
- 当前最大风险：
  - [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 当前是 mixed scene
  - 虽然我 intended 的 scene 级改动只有 `autoCollectSceneBlockingColliders: 1`
  - 但 git diff 体量远大于一行，说明 scene 文件里还混着别的现场改动
- 当前恢复点：
  - 下一步先跑 `Ready-To-Sync`
  - 预期很可能会被 `Town.unity` mixed 现场或同根历史脏改阻断
  - 如果被阻断，正确动作是报 blocker 并 `Park-Slice`，而不是假装 clean

## 2026-04-13 只读审核：fresh profiler 已把这轮导航修法判回“顺序修错”
- 用户最新要求：
  - 不再继续修
  - 先彻查并汇报：性能为什么还炸、我修错了什么、用户到底要什么
- 这轮我重新压实的结论：
  1. `fresh profiler` 已足够说明：病灶仍在 `NPCAutoRoamController.Update()` 驱动的 NPC 自漫游热链。
  2. 我这轮确实往 hottest loop 里叠了更贵的过滤：
     - [TryBeginMove](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1552) 在 `pathSampleAttempts = 12` 的采样循环里调用 [TryResolveAutonomousRoamDestination](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L2283)
     - [HasAutonomousRoamDestinationNeighborhoodClearance](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L2339) 最多额外打 `4` 次 `CanOccupyNavigationPoint(...)`
     - [IsAutonomousRoamBuiltPathAcceptable](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L2378) 又在建路成功后补了一层 detour 检查
  3. 我还把共享 occupancy 契约加重了：
     - [NavigationTraversalCore.CanOccupyNavigationPoint](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs#L148) 现在会继续走 [HasExpandedStaticClearance](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs#L520)
     - 这说明变重的不只是 roam 选点，而是共用这条 traversal contract 的 occupancy 判定
  4. 当前 repo 明文里，导航主链仍是 non-alloc `Physics2D.OverlapCircle(..., cache)`：
     - [NavGrid2D.IsPointBlocked](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs#L811)
     - [NavGrid2D.IsWalkable](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs#L1690)
     - 显式 `OverlapCircleAll` 只在 `ChestDropHandler`、`ChestController`、`PlayerAutoNavigator`
  5. 所以 `fresh profiler` 里的 `OverlapCircleAll` 与当前 live 文本源码仍未一一钉死；当前最稳口径只能是：
     - 热点仍在 NPC 自漫游链
     - 但我还没有把 `OverlapCircleAll` 标签精确映射到当前 live 源码里的唯一调用点
  6. [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 仍是 mixed scene，`git diff --numstat` 为 `1481 / 324`；不能再把整张 scene 当成我这一轮的纯净单点成果。
- 当前总判断：
  - 这轮失败不是“方向完全错”，而是“真正 hottest loop 还没拆掉前，我又往里面叠了更贵的过滤”
  - 当前已有 `dedupe / backoff / reuse` 不是没有，而是没有挡住 `TryBeginMove` 单次坏 case 的成本
  - 如果下一轮允许继续修，第一刀必须先拆热循环，不再继续往 `TryBeginMove` 叠 correctness
- 证据边界：
  - 用户之前给的 temp 截图路径已失效，这轮无法重新打开原图
  - 当前结论属于：`结构/targeted probe 证据成立，真实体验已被 fresh profiler 否决`
- thread-state 报实：
  - 本轮只读分析，未重新 `Begin-Slice`
  - 当前 live 状态继续视为 `PARKED`

## 2026-04-13 真实施工：按 fresh profiler 结论先拆 `TryBeginMove` 热循环
- 用户在只读审核后明确放行：`落地`
- 本轮已重新进入真实施工：
  - `Begin-Slice` 已跑
  - slice = `npc-roam-hotloop-performance-fix`
- 这轮实际修改：
  1. [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1552)
     - `TryBeginMove()` 从 `TryResolveAutonomousRoamDestination + 邻域 clearance + built-path acceptable` 改回更便宜的 `TryResolveOccupiableDestination`
     - 新增 `GetAutonomousRoamSampleAttemptBudget()`：`blocked / stuck / path failure / pending progress` 时把采样预算压到 `4~6`
     - 目的：先让 bad case 下的单次 roam 选点不要再打满整轮高成本过滤
  2. [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs#L148)
     - 去掉 `CanOccupyNavigationPoint()` 里额外叠的 `expanded static clearance` 热查询
     - 目的：撤掉我上一轮加在 shared occupancy contract 上的额外成本
- 当前验证：
  - `validate_script`
    - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) => `errors=0`，仅 1 条旧 GC warning
    - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs) => `errors=0 warnings=0`
  - `git diff --check`：通过
  - Unity fresh console：`0 error / 0 warning`
- 当前判断：
  - 这刀的目的不是“最终体验恢复”，而是先把我自己新叠进去的热点成本拿掉
  - 当前状态属于：`代码层已落地，真实性能/体感仍待 live 复测`
- thread-state 恢复点：
  - 本轮尚未 `Ready-To-Sync`
  - 如果当前停止继续 live 验，应先 `Park-Slice`
  - 当前已执行 `Park-Slice`，live 状态回到 `PARKED`

## 2026-04-13 继续 live 调试：`Building` 不是没处理，fresh profiler 的 101 热点落在 resident scripted control
- 用户后续追加要求：
  - 先确保控制台调试输出足够关键
  - 再自己跑 live 并分析，不先继续乱修
- 本轮真实动作：
  1. 重新 `Begin-Slice`，slice = `npc-building-avoidance-live-debug`
  2. 在 [NavGrid2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs) 新增 `BuildBlockingDebugSummary(...)`
  3. 在 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 新增 `LogBlockedMovementDiagnostics(...)`
  4. 先误入 `Primary` 做了一拍 live，确认那边只有 `2` 个 NPC bindings，不是用户截图里的主要坏现场
  5. 再回到 `Town` 正确现场，对 `101` 和 `003` 临时挂 `showDebugLog` 做 live 取证，最后已全部关回去
- 本轮最关键的新结论：
  1. `Building` 标签不是完全没处理：
     - `Town` 的 `obstacleTags` 明确含 `Building / Buildings`
     - [NavGrid2D.HasAnyTag](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs#L2287) 会沿父链查 tag
  2. fresh profiler 里最热的 `101`，当前 live runtime 状态不是普通 roam：
     - `IsResidentScriptedControlActive = true`
     - `IsResidentScriptedMoveActive = true`
     - owner = `spring-day1-director`
  3. 这说明我之前把性能问题只盯成“自然漫游坏 sample”是不够的；至少最热样本 `101` 这一拍落在 resident scripted control 驱动的移动链上
  4. 这轮 live 窗口里，没有成功触发 `101` 的 `[NPCBlockedDebug]`：
     - 它只进入了短停/移动，没有复现顶建筑坏 case
     - 因而这轮还不能把“building 避让问题”钉死到唯一 collider/root cause
- 当前收口状态：
  - 运行已停止
  - 临时 debug scene 开关已关回去
  - `Park-Slice` 已执行
  - 当前 live 状态 = `PARKED`

## 2026-04-13 只读自我审查：当前功能问题已经比性能问题更紧急
- 当前主线目标：
  - 不是继续泛修性能，而是把 NPC 导航恢复到“农田配置尊重、不会乱撞静态障碍、resident 不再乱摆头、小动物不再集体卡死”的可打包功能状态
- 本轮子任务：
  - 只读核查用户 fresh 反馈，不改代码
- 这轮压实的关键判断：
  1. `Farmland_Border` 被纳入 obstacle，不是用户配置错，是我在 [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs#L23) 留着 `border` 自动收集关键字，且 `Town` 当前仍开着 `autoCollectSceneBlockingTilemaps / Colliders`
  2. `Building` 标签不是当前主因；`Town` 的 `obstacleTags` 其实已经含 `Building / Buildings`
  3. 我之前为止峰值回退了 `TryBeginMove()` 的 acceptance 层，导致当前 NPC 自漫游只能保证“有路/勉强可走”，不能保证“目标足够干净、路径不贴边”
  4. 用户贴的 `NPCFacingMismatch` 已直接证明 resident 一批还存在 `movement owner != facing owner`
  5. 小动物集体停滞不是“避让完全没代码”，而是静态坏目标和封闭空间坏 case 先把它们拖进 blocked / pause 循环，后面的共享避让根本发挥不出来
- 当前最重要的恢复点：
  - 下一刀必须按这个顺序修：
    1. 配置权威：先把自动收错的 `Farmland_Border` 排除
    2. 自漫游 acceptance：把便宜但有效的目标/路径筛选接回 `TryBeginMove()`
    3. owner 一致性：resident 里谁驱动移动，谁写朝向
    4. 末端缓冲：必要时再给 NPC 补玩家那种 blocked-navigation 式的最后一米阻挡缓冲
- 验证状态：
  - `静态推断成立 + 用户 live 图证强支撑`
  - `尚未修复`
- thread-state 报实：
  - 本轮只读分析，没有重新 `Begin-Slice`
  - 当前 live 状态保持 `PARKED`

## 2026-04-13 追加施工：`Farmland_Border` 自动误收已修
- 用户当轮明确纠偏：虽然大头是汇报，但 `Farmland_Border` 被自动塞进 obstacle 这一项必须本轮先修
- 本轮动作：
  - 重新 `Begin-Slice`
    - slice = `fix-farmland-border-auto-collect`
  - 只修改 [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs)
  - 完成后已再次 `Park-Slice`
- 改动内容：
  1. 默认 exclude keywords 增加 `farmland`
  2. `ShouldAutoCollectBlockingSource(...)` 增加农田可走面早退
  3. 自动收集静态障碍时，显式跳过：
     - `Farmland_Border`
     - `Farmland_Center`
  4. 保留：
     - `Farmland_Water` 仍按 `water` 走 soft-pass
     - 其余 `wall / props / fence / rock / tree` 自动阻挡不变
- 验证：
  - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs) `validate_script => errors=0 warnings=0`
  - fresh console 仍有外部 `Missing Script` 与 TMP 字体 warning，但没有看到这刀引入的新红
- 当前判断：
  - 这刀已经把用户指出的“farm 必须可走，不能被自动误收”落地修掉
  - 但这不等于 NPC 导航整体已过线；剩余主问题仍是：
    1. 自漫游 acceptance 退化
    2. resident 朝向 owner 冲突
    3. 小动物封闭区 bad case 停滞
    4. `NPCMotionController.GetSmoothedFacingDirection()` 自身也会在 `facingSource=none` 时拖错朝向

## 2026-04-13 导航检查继续 live：Town clean run 下卡顿未复现，但 FreeTime validation 仍没复现到用户那批 resident 真坏 case
- 当前主线仍是：
  - NPC 导航功能优先
  - 不能再有性能炸弹
  - 不改玩家 traversal 语义
- 本轮动作：
  1. 保持线程 `ACTIVE`，继续当前 slice `npc-live-perf-regression-triage`
  2. 先做只读 live 核查：
     - `editor/state`
     - `Town` active scene
     - console
     - `SpringDay1LatePhaseValidationMenu`
     - `SpringDay1ActorRuntimeProbeMenu`
     - `NpcRoamSpikeStopgapProbeMenu`
  3. 发现 `Town` play 中直接深读 GameObject `/components` 会触发 MCP 自己的 Animator/Playback serializer error，污染 console 和性能样本
  4. 落一刀真实代码：
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
       - `DebugMoveTo()` 接入 `TryAcquirePathBuildBudget()` + `RecordPathBuildOutcome(...)`
       - 新增 scripted move 跨帧 decision reuse（`SCRIPTED_MOVE_DECISION_REUSE_SECONDS = 0.08f`）
- 关键验证结果：
  1. `validate_script(NPCAutoRoamController)`：`errors=0`，仅 1 条旧 warning
  2. `git diff --check -- Assets/.../NPCAutoRoamController.cs`：通过
  3. fresh console（编译后）：`0 error / 0 warning`
  4. clean `Town` run + `NpcRoamSpikeStopgapProbe`：
     - `npcCount=25`
     - `roamNpcCount=18`
     - `avgFrameMs=0.90`
     - `maxFrameMs=19.51`
     - `maxGcAllocBytes=67014`
     - `maxConsecutivePathBuildFailures=0`
     - `topSkipReasons=AdvanceConfirmed`
- 这轮最重要的判断：
  1. 之前那次 `avgFrameMs≈95ms` 的 probe，不能再当干净游戏基线
     - 因为 probe 前我自己刚做过多个 `/components` 资源深读
     - MCP serializer 自己在 PlayMode 里刷了多条 Animator/Playback error
     - 所以那次样本混进了我的取证干扰
  2. 当前 patched + clean run 下，我没有再复现“Town 一进 FreeTime 就持续卡爆”
  3. 但这不等于用户真实坏 case 已全过：
     - fresh `Force Spring Day1 FreeTime Validation Jump` 后
     - `SpringDay1ActorRuntimeProbe` 仍显示 `101~203` 处于 `staging:*`
     - `roamControllerEnabled=false`
     - `scriptedControlOwnerKey=spring-day1-director`
     - 所以我这条 validation 路径还没把用户抱怨的“常驻居民真正自由动起来”的现场干净复现出来
- 本轮如果停手，下一次恢复点：
  1. 优先找更贴近用户真实路径的 Town resident free-roam / return-home 入口
  2. 不再先做 MCP `/components` 深读，避免污染性能样本
  3. 当前 patch 已经留在 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)

## 2026-04-13 导航检查只读再收敛：已经能明确回答“不能只靠 no-red 闭环，但已经知道下一刀怎么改”
- 用户最新要求：
  - 不要再讲可能性
  - 直接回答：是否能不用 live、只靠 MCP/no-red 把导航彻底做对；如果不能，为什么不能；以及下一刀精确改点是什么
- 本轮动作：
  - 手工等价执行 `skills-governor + sunset-workspace-router + user-readable-progress-report + delivery-self-review-gate`
  - 不进真实施工，维持只读
  - 重点回看：
    - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs)
    - [NavigationPathExecutor2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs)
    - [NavGrid2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs)
    - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs)
    - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
    - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
    - [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs)
    - [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity)
    - [npc-roam-spike-stopgap-probe.json](D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/npc-roam-spike-stopgap-probe.json)
    - [spring-day1-actor-runtime-probe.json](D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/spring-day1-actor-runtime-probe.json)
- 这轮稳定结论：
  1. 不能只靠 MCP/no-red 就 claim “完全符合需求”
     - no-red 只能证明 compile/console clean
     - 不能证明真实 NPC 不会撞墙、不会贴树、不会被 day1 owner 持续接管
  2. 真正的导航 core 问题现在已经压成两层：
     - `静态栅格真值不完整`
       - `TraversalBlockManager2D` 的 auto-collect 仍主要靠名字关键词；Town 当前 `sceneBlockingIncludeKeywords` 没有 `house/building`
       - `NavigationPathExecutor2D -> NavGrid2D.TryFindPath` 建路用的是 baked `walkable[,]`
       - 所以路径可能仍会规划进房屋/建筑附近
     - `运行态 occupancy contract 太窄`
       - `NavigationTraversalCore.CanOccupyNavigationPoint` 现在是脚底 center/left/right 小半径 probe
       - 足以在脚底层防穿模，但不足以保证 NPC 身体整体不贴柱/墙
  3. `101~203` 这批 resident 仍有 day1 owner 层，不是普通 roam
     - `spring-day1-actor-runtime-probe.json` 直接显示 `scriptedControlOwnerKey=spring-day1-director`
     - 所以导航 core 修完，也不能谎报“这批 resident 全部就自然恢复”
  4. 性能方向也已经更清楚：
     - 下一刀不能再把重查询塞回热循环
     - 正确位置是“静态阻挡真值修正 + 低频建路 acceptance 修正”
- 当前恢复点：
  1. 若用户放行真实施工，先改 `TraversalBlockManager2D.ShouldAutoCollectBlockingSource()`，让“用户已配 tag/layer/soft-pass/walkable override”成为静态阻挡采集权威
  2. 再改 `NPCAutoRoamController.TryBeginMove()/TryResolveAutonomousRoamDestination()/IsAutonomousRoamBuiltPathAcceptable()`，只在低频建路层补 body-clearance acceptance
  3. `NavigationTraversalCore` 只补 helper，不重做 hot loop
  4. resident owner 问题继续按 day1 caller 分责，不冒充成纯导航 core 问题
- 本轮状态：
  - 纯只读
  - 未跑 `Begin-Slice`
  - 当前合法维持 `PARKED`

## 2026-04-13 导航检查真实施工：已落地 `nav-static-truth-and-autonomous-acceptance`
- 本轮已跑 `Begin-Slice`：
  - slice=`nav-static-truth-and-autonomous-acceptance`
  - owned paths=
    - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs)
    - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
    - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs)
- 本轮真实改动：
  1. `TraversalBlockManager2D`
     - 自动静态阻挡补全不再只靠名字关键词
     - 默认关键词补入 `building / house / structure`
     - 新增按 `NavGrid` 当前 obstacle tags / obstacle mask 的语义补收静态 source
     - 保住 `farmland 不入 hard obstacle` 与 `water soft-pass`
  2. `NavigationTraversalCore`
     - 新增 `CanOccupyNavigationPointWithClearanceMargin(...)`
     - 只给低频建路 acceptance 用，不碰移动热循环
  3. `NPCAutoRoamController`
     - autonomous roam 目标点现在要求 destination body-clearance
     - autonomous roam built path 现在要求整条路径 body-clearance
     - 采样间距 `0.32f`
- 本轮 no-red 自检：
  - `validate_script Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs`
    - `assessment=external_red`
    - `owned_errors=0`
    - native `manage_script.validate=clean`
  - `validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - `assessment=external_red`
    - `owned_errors=0`
    - native `manage_script.validate=warning`
    - warning 仅为既有 GC concat 提示
  - `validate_script Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs`
    - `assessment=external_red`
    - `owned_errors=0`
    - native `manage_script.validate=clean`
  - `git diff --check -- <3 files>`：通过
- 当前 blocker / 外部现场：
  - Unity 仍在 `Primary.unity` 的 `playmode_transition / stale_status`
  - console 仍有既有 external：
    - 多条 `The referenced script (Unknown) on this Behaviour is missing!`
    - 1 条 MCP websocket warning
  - 当前没有证据显示是本轮导航改动新增
- 当前阶段：
  - 这轮代码已完成并合法 `Park-Slice`
  - 仍未做 live 终验
  - resident `101~203` 的 owner 问题仍需与 day1 分责

## 2026-04-14 导航检查 Town live 验收：导航 core 短窗稳定，但 resident owner 漏口仍在
- 本轮已跑 `Begin-Slice`：
  - slice=`town-live-acceptance-after-static-truth-fix`
- 本轮 live 动作：
  1. 从 `Town` 当前场景直接 Play
  2. 跑一次 `NpcRoamSpikeStopgapProbe`
  3. 写一次 `SpringDay1ActorRuntimeProbe`
  4. 强制跳 `FreeTime`
  5. 再写一次 actor runtime probe
  6. 再跑一次 roam spike probe
  7. 立即 Stop，退回 EditMode
- 关键 live 结果：
  - 第一轮普通入口 probe：
    - `npcCount=25`
    - `roamNpcCount=15`
    - `avgFrameMs=0.67`
    - `maxFrameMs=9.53`
    - `maxBlockedNpcCount=0`
    - `maxBlockedAdvanceFrames=0`
    - `maxConsecutivePathBuildFailures=0`
  - 第二轮 FreeTime probe：
    - `npcCount=25`
    - `roamNpcCount=17`
    - `avgFrameMs=0.62`
    - `maxFrameMs=12.32`
    - `maxBlockedNpcCount=0`
    - `maxBlockedAdvanceFrames=0`
    - `maxConsecutivePathBuildFailures=0`
  - 当前 live 没抓到导航 core 的 spike / blocked storm
  - actor probe 明确显示：
    - `phase=FreeTime`
    - `beatKey=FreeTime_NightWitness`
    - 但 `101~203` 仍 `appliedBeatKey=EnterVillage_PostEntry`
    - `scriptedControlOwnerKey=spring-day1-director`
    - `roamControllerEnabled=false`
    - `roamState=Inactive`
    - `003` 也仍被 `spring-day1-director` 挂住，未恢复自然 roam
    - `001/002` 已恢复正常 roam
- 当前结论：
  - 导航 core 这一刀在 Town 受控短窗里没有再打爆性能，也没刷新的 blocked 证据
  - 当前 live 真剩余问题更像 `day1 owner release 漏口`
  - 如果用户现场仍见到贴房/贴树，需要下一轮做更定向的坏点复现，不是现在这轮短窗已稳定复现
- 本轮收口：
  - 已 `Park-Slice`
  - 当前 live 状态：`PARKED`

## 2026-04-14 导航检查只读深挖：把“非剧情态导航病态”拆成 owner / roam / animal / player-vs-npc 四层
- 当前主线：
  - 用户要求这轮只读深挖，不许跨 owner 直接改
  - 追查范围不能只停在 `FreeTime roam`
  - 要把更多非剧情态导航病态拆清楚，再决定后续谁该修
- 本轮只读检查动作：
  1. 复核 `2026-04-10_导航检查_NPC自漫游峰值卡顿止血刀与现有节流失效复盘_62.md`
  2. fresh 重读：
     - `Library/CodexEditorCommands/spring-day1-actor-runtime-probe.json`
     - `Library/CodexEditorCommands/npc-roam-spike-stopgap-probe.json`
     - `Library/CodexEditorCommands/spring-day1-resident-control-probe.json`
  3. 对比代码链：
     - `NPCAutoRoamController`
     - `NPCMotionController`
     - `NavigationTraversalCore`
     - `NavGrid2D`
     - `TraversalBlockManager2D`
     - `PlayerAutoNavigator`
     - `NavigationLocalAvoidanceSolver`
     - `NavigationAvoidanceRules`
     - `SpringDay1NpcCrowdDirector / SpringDay1Director / SpringDay1DirectorStaging`
  4. 读 `Town.unity` 当前序列化的：
     - `NavGrid2D.obstacleTags`
     - `TraversalBlockManager2D.sceneBlockingInclude/ExcludeKeywords`
  5. 读 `global-preference-profile.md`，确保这轮不会把“静态推断成立”冒充成“体验已修好”
- 本轮新增钉死的关键判断：
  1. `101~203 + 003` 的冻结/不 roam，当前 first truth 仍是 day1 owner 未释放，不是导航 core 泛坏
  2. “剧情移动正常、自漫游病态”符合当前代码结构：
     - scripted move 走显式目标
     - autonomous roam 先随机采样目标再 acceptance
  3. NPC 与玩家并不共享完整同一层静态世界处理：
     - 玩家 caller 层仍有静态 collider steering
     - NPC 只有脚底约束 + blocked/stuck 止血，没有同级静态 steering
  4. shared avoidance 主要解决 agent-agent，不负责房屋/树/围栏这类静态场景体
  5. `Town` 的 `TraversalBlockManager2D` scene 序列化 include 仍落后于代码默认值
     - scene 里还没序列化进 `building/house/structure`
     - 当前 building 是否入 truth，受 `NavGrid obstacle tags + ancestor depth<=6` 共同影响
  6. 农田规则当前代码意图仍然正确：
     - `Farmland_Center/Border` 自动视为 walkable surface
     - `Farmland_Water` 不豁免
  7. 当前 fresh 短窗 probe 没有再打到性能 storm
     - 所以最新最高价值问题是功能契约，而不是“这版一定每次都爆卡”
- 当前最重要的问题地图：
  1. `day1 caller/owner`：resident 释放漏口
  2. `普通 Town 自漫游`：目标 acceptance 与静态执行契约失配
  3. `小动物/围栏`：同一 roam contract 在 closed domain 下更容易卡死/停滞
  4. `玩家 vs NPC`：静态障碍 steering 能力不对等
- 当前最核心的判断：
  - 现在的病态导航不是“一个总开关坏了”
  - 至少已经能拆成两类不同 owner：
    - day1 reality/owner 漏口
    - navigation roam/runtime contract 失真
  - 如果下一刀不先分账，继续混修只会再次出现“修一点坏一点”
- 本轮未做：
  - 没动代码
  - 没改 scene/prefab
  - 没开新的施工 slice
- 当前线程状态：
  - `PARKED`
  - active ownership 仍显示 `导航检查 = PARKED / town-live-acceptance-after-static-truth-fix`
- 下一轮恢复点：
  1. 若继续只读：用 `1 个普通 Town NPC + 1 个动物 case` 钉第一失真点
  2. 若允许施工：必须先决定是修 `day1 owner release`，还是修 `NPC static steering / scene truth`，不能混在一刀里
## 2026-04-14 11:34 prompt_67 续记：valid roam 样本已和 Day1 持有样本彻底分账
- 本轮先读：
  - [2026-04-14_导航线程_Day1边界重划后非剧情态roam静态契约收口prompt_67.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-14_导航线程_Day1边界重划后非剧情态roam静态契约收口prompt_67.md)
  - 当前线程 / 工作区 memory
- 本轮主线目标：
  - 只收 `Day1 已放人后` 的 NPC / 动物非剧情态 roam 静态契约
- 本轮子任务：
  - 把 valid roam 样本和仍被 Day1 持有的样本拆开
  - 复核当前导航 own 的第一真 gap 是否仍落在静态真值 / 执行契约，而不是 owner 漏释放
- 这轮实际站稳的事实：
  1. `001/002/003` = valid roam 样本
  2. `101~203` = 当前仍被 `spring-day1-director` 持有的 invalid roam 样本
  3. 动物 prefab 当前和普通 NPC 同根走 `NPCAutoRoamController + NPCMotionController`
  4. `Town.unity` 当前 `walkableOverrideTilemaps / walkableOverrideColliders` 为空，桥/水 walkable contract 仍不是显式配置态
- 这轮没有继续改 runtime 代码，只做了 read-only 收口；结束前已执行：
  - `Park-Slice`
  - 当前 live 状态=`PARKED`
- 当前恢复点：
  1. 如果下一轮继续 real fix，只能继续沿 `prompt_67` 白名单推进
  2. 不要再把 `101~203` 当成导航 own roam 样本
  3. 下轮优先看：
     - Town 的 bridge/water walkable override 是否需要最小 scene 核对
     - 当前 `NPCAutoRoamController` 的静态 steering 是否还只是止血，而非真正对齐玩家静态执行契约
## 2026-04-14 18:13 prompt67 own 修复续记：201 stale failure 与局部坏点恢复补口
- 本轮先读：
  - [2026-04-14_导航线程_Day1边界重划后非剧情态roam静态契约收口prompt_67.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-14_导航线程_Day1边界重划后非剧情态roam静态契约收口prompt_67.md)
  - 当前工作区 / 线程 memory
- 本轮主线目标：
  - 继续只收 `Day1 已放人后` 的 NPC / 动物非剧情态 roam 静态执行契约
- 本轮子任务：
  - 只修导航 own：
    1. `201` 类单点 case 的 stale `consecutivePathBuildFailures`
    2. 自漫游在局部坏点 / 窄口附近的恢复采样
- 本轮实际动作：
  - 沿现有 active slice 继续施工
  - 修改 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
    - `TryBeginMove()` 增加 autonomous recovery 的局部 safe center + 8 向 deterministic escape sampling
    - `NoteSuccessfulAdvance()` 在真实推进确认后清零 path failure/backoff
  - `validate_script` 通过：
    - `errors=0`
    - `warnings=1`
  - `git diff --check` 通过
- 本轮新增事实：
  - 当前编辑器已不在 Play Mode；17:20 的 roam probe 样本不能继续当作“现在现场”
  - 当前 Console 的唯一 red 是外部 blocker：
    - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs(2452,13): error CS0103: The name 'SyncPrimaryTownReturnGate' does not exist in the current context`
  - 因此外部红面阻断了 fresh live 复测，但没有阻断我 own 脚本的代码层自检
- 本轮判断：
  - 这刀价值在于先收掉导航恢复链里最像导致：
    - `201` 持久挂 16
    - 动物 / resident 在局部坏点反复抽签不脱困
  - 它不涉及 Day1 authored/staging/release contract
- 本轮未做：
  - 没碰 Day1 文件
  - 没改 scene / prefab / profile 资产
  - 没做新的 live 结论宣称
- 当前线程状态：
  - `Park-Slice` 已跑
  - `thread-state=PARKED`
- 下一轮恢复点：
  1. 等 Day1 compile blocker 清掉后，直接用 Town fresh live 验 `201/103/动物互锁`
  2. 如果还复现，继续只看：
     - `静态 clearance`
     - `agent clearance`
     - 不再回漂 owner 分账

## 2026-04-15 11:03 真实施工续记：用户把主锅重新钉回导航 own 后，我已只收 `autonomous-roam-deadlock-and-static-obstacle-contract`
- 当前主线目标：
  - 不再碰 Day1
  - 只修“禁用 `SpringDay1NpcCrowdDirector` 后仍存在的互锁/撞墙/卡墙”
- 本轮已执行：
  - `Begin-Slice=之前已跑`
  - `Park-Slice=本轮已跑`
  - 当前状态=`PARKED`
- 本轮实际改动：
  - [NavigationLocalAvoidanceSolver.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs)
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
  - [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs)
- 这轮最核心判断：
  1. `NPC-NPC` 互锁的直接根因不是“完全没避让”，而是：
     - yielding peer 侧移不够坚决
     - hold-course peer 也被一起拖慢
     - controller 层 deadlock break 太迟
  2. `静态卡墙` 的直接根因不是“完全没静态 steering”，而是：
     - probe 半径/前瞻距离偏保守
     - frontal obstacle 时 repulse 更像后退止血，缺少稳定侧绕偏置
- 本轮落地：
  1. solver：
     - NPC peer head-on 时，yielding 一侧 sidestep 更强且保留最小侧移速度
     - hold-course 一侧保留更高前进速度，不再更容易“双停”
  2. controller：
     - `shared avoidance deadlock` 提前从 `18f/6 sightings` 收到 `10f/3 sightings`
     - deadlock 时优先 `TryCreateSharedAvoidanceDetour(...)`，再退到 `TryBeginMove(...)`
     - static steering 的 body-clearance 半径和 ahead probe 都加大
     - frontal obstacle 额外给稳定 sidestep bias
     - passive NPC blocker 的 reroute 也提前
  3. tests：
     - 新增 yielding NPC head-on
     - 新增 hold-course NPC head-on
     - 新增静态正前方障碍时的 sideways bias
- 验证：
  - `validate_script <3 files>`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
    - 阻断原因=`playmode_transition / stale_status`
  - `manage_script validate`
    - `NavigationLocalAvoidanceSolver=clean`
    - `NPCAutoRoamController=warning(既有 string concat GC 提示)`
    - `NavigationAvoidanceRulesTests=clean`
  - `git diff --check` 通过
- 当前恢复点：
  1. 下一轮优先做 `Town` 的 pure autonomous roam fresh live，只看：
     - 房屋/石头前是否仍贴边磨
     - 小动物群是否仍互锁
     - 关闭 CrowdDirector 后普通居民是否仍显著互锁
  2. 若还复现，继续只收导航 own：
     - `destination agent clearance`
     - `detour candidate distance`
     - 不再回漂 `spring-day1`

## 2026-04-15 11:44 只读续记：用户牛圈截图与 prompt_68 / NPC 回执对齐后，当前第一刀继续压向 `per-step occupancy + release 后执行合同`
- 当前主线目标：
  - 只读说清 `Day1` 已 release 后，为什么 `NPC/动物` 还会卡墙、抽搐、互锁、假 roam。
- 本轮子任务：
  - 结合用户最新牛圈截图、`prompt_68`、`NPC` 回执和现码，回答“复杂 collider 形状会不会影响 NPC 静态导航”“为什么玩家没问题而 NPC 有问题”。
- 这轮新站稳的结论：
  1. `复杂 collider 形状 / 位置不一` 会放大 `NPC` 坏相，但不是用户场景做错，而是 `NPC` 热路径的逐步执行占位仍偏脚底化。
  2. `NavigationTraversalCore.CanOccupyNavigationPoint(...)` 仍主要靠脚底 center/left/right 三点 probe；`GetNavigationPointQueryRadius(...)` 也仍是较窄 query radius。
  3. `NPCAutoRoamController.TickMoving()` 每一步推进最终仍会回落到这套 occupancy，所以会出现：
     - 脚底可过
     - 身体 clearance 不够
     - 执行层再被 static steering / local avoidance / blocked recovery 来回拉扯
     - 最终表现成贴边磨、原地小幅抽搐、上下转头
  4. `NPC` 的 `GetAvoidanceRadius()` 现在还被 cap 在 `colliderRadius + 0.06f`，这会让它比玩家更容易贴物体、贴同类。
  5. `Player` caller 层的 `AdjustDirectionByColliders(...)` 明显更强，ahead probe 更远、更宽，avoidance 半径也没有 `NPC` 那个紧 cap，所以玩家不会像当前 `NPC` 这样脆。
  6. 牛这种“旁边没明显静态 blocker 也会抽搐”的 case，更像：
     - `per-step occupancy / clearance` 偏窄
     - 叠加 `pairwise local avoidance`
     - 再被 `blocked recovery` 放大
     - 不是一句“building/tag 没吃进去”能解释完。
- 当前最核心判断：
  - 现在导航 own 最值钱的唯一下一刀，仍然不是继续撒参数，而是把 `released 后 return-home / free-roam` 的逐步执行占位，从脚底 probe 主导收成 body-clearance-aware contract。
- 当前验证层：
  - 这轮仍只站住 `结构 / targeted probe`
  - 不能宣称 `真实入口体验已过线`
- thread-state：
  - 本轮纯只读
  - `Begin-Slice / Ready-To-Sync / Park-Slice` 均未新跑
  - 当前继续按既有状态 `PARKED`

## 2026-04-15 12:17 真实施工续记：`released-movement-body-clearance-contract` 已落地并合法停车
- 当前主线目标：
  - 只修导航 own
  - 让 `released 后 return-home / free-roam` 的逐步执行占位更像玩家一样尊重 body clearance
- 本轮已执行：
  - `Begin-Slice=已跑`
  - `Park-Slice=已跑`
  - 当前状态=`PARKED`
- 本轮实际改动：
  - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs)
    - 新增 `ConstrainNextPositionToNavigationBoundsWithClearanceMargin(...)`
    - 把 next-position constraint 扩成可带 body-clearance margin 的版本
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
    - free-roam / formal home return 现在会走 clearance-aware next-position constraint
    - plain debug / resident scripted move 不吃这刀
  - [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs)
    - 新增窄缝回归，证明“脚底 probe 可过”不再等于“body clearance 也可过”
- 这轮验证：
  - `validate_script <3 files>`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
    - 阻断原因仍是 Unity `stale_status`
  - `manage_script validate`
    - `NavigationTraversalCore=clean`
    - `NavigationAvoidanceRulesTests=clean`
    - `NPCAutoRoamController=warning(既有 string concat GC 提示)`
  - `git diff --check`
    - 通过
- 这轮最核心判断：
  - 这刀已经命中了“复杂边界下脚底可过、身体 clearance 不够”的第一失真点
  - 但我还不能说体验已过，因为 Town live 终验还没跑
- 当前恢复点：
  1. Unity 稳定后直接回 Town 验：
     - released 后贴房/贴树/贴石头磨步
     - 牛圈/动物局部抽搐
  2. 如果仍有坏相，下一刀继续只收：
     - detour candidate distance
     - destination agent clearance
     - 不回漂 Day1

## 2026-04-15 15:08 续记：用户贴出的 `NavigationAvoidanceRulesTests.cs` 三个 compile error 已核为“旧红已清”，本轮不再误判成当前 owned 问题
- 当前主线目标：
  - 继续只收导航 own，不把 `Day1` 或 `NPC` 的外部脏 console 误判成导航自己还在同一组测试红里打转。
- 本轮子任务：
  - 逐项核对用户贴出的 3 个编译错误，并确认当前现码是否仍在报它们。
- 本轮实际确认：
  1. `Component.enabled` 错误已消失，当前测试代码明确使用 `((Behaviour)inactiveController).enabled = false`。
  2. `NavigationAgentRegistry` 直引类型错误已消失，当前测试改用 `ResolveTypeOrFail(\"NavigationAgentRegistry\")` + 反射取 `GetRegisteredUnits<T>()`。
  3. `Component -> string` 参数转换错误已消失，当前断言走 `IList buffer.Contains(controllerX)`，不再碰错误重载。
  4. `manage_script validate` fresh 结果：
     - `NavigationAvoidanceRulesTests=clean`
     - `NavigationAgentRegistry=clean`
     - `NPCAutoRoamController=warning only`
  5. `validate_script` / `compile` 仍可能失败，但当前失败源头已改判为外部噪音：
     - `DialogueChinese V2 SDF.asset` import inconsistent
     - 多条 `The referenced script (Unknown) on this Behaviour is missing!`
     - `CodexCodeGuard/dotnet timeout`
  6. 我还额外通过 `CodexEditorCommandBridge` 发了 `STOP`，把 Unity 从卡住的 `PlayMode transition` 拉回 `EditMode`，避免继续污染判断。
- 本轮关键判断：
  - 用户贴的这 3 个错误不是“导航现在还没修完”的现行铁证，而是旧测试写法留下的报错快照；当前 own 阻断已经切换成 Unity/console 外部噪音。
- 验证状态：
  - `结构成立 + 代码层验证成立`
  - `Unity compile-first 干净票` 仍未闭环，因为外部噪音还在。
- 本轮 thread-state：
  - `Begin-Slice=已跑`
  - `Park-Slice=已跑`
  - 当前状态=`PARKED`
- 当前恢复点：
  1. 如果下一轮还要追 `no-red`，先清外部 importer/missing-script 噪音，再重跑 compile-first。
  2. 如果回到导航主线体验修复，则继续看 `Town` pure autonomous roam 的 live 体验，不再回查这 3 个旧 compile 红。

## 2026-04-15 16:24 续记：已在 Unity 里完成 Town 自漫游 live 验证，结论是“没有完全坏死，但还没正确”
- 当前主线目标：
  - 只验证导航 own 的 `Town` 自漫游质量，回答“现在导航正确了吗”。
- 本轮子任务：
  - 在用户可直接观看 Unity 的前提下，跑最小 live probe，确认当前 roam 是否已过线。
- 本轮实际执行：
  1. 保持 Unity 在 `Town` 的 `Play Mode`
  2. 连续两次触发 `Tools/Sunset/NPC/Run Roam Spike Stopgap Probe`
  3. 每次 6 秒，只采当前 live roam，不切场景、不混 Day1 staging
  4. 探针后补跑 fresh `errors`
- 本轮结果：
  1. 两轮都是 `25/25` roam NPC
  2. 第 1 轮：
     - `avgFrameMs=28.45`
     - `maxFrameMs=101.23`
     - `maxBlockedNpcCount=1`
     - `Stuck=19`
  3. 第 2 轮：
     - `avgFrameMs=31.46`
     - `maxFrameMs=102.88`
     - `maxBlockedNpcCount=3`
     - `Stuck=21`
     - `SharedAvoidance=9`
     - `MoveCommandNoProgress=2`
  4. fresh console：
     - `errors=0`
     - `warnings=0`
- 本轮关键判断：
  - 现在已经不是“导航完全不工作”
  - 但也不能叫“正确”
  - 因为 live 里仍存在持续的：
    - `Stuck`
    - `SharedAvoidance`
    - `MoveCommandNoProgress`
    - `ReusedFrameMoveDecision / ReusedFrameStopDecision`
  - 这些正对应用户一直在抱怨的：
    - 原地抽搐
    - 互卡
    - 走同一小段反复重试
- 验证层：
  - `真实入口 live + targeted probe` 已拿到
  - 结论是：`未过线`
- 当前阶段：
  - 继续是导航体验修复阶段，不是收尾阶段
- 下一步只做什么：
  - 继续只收导航 own 的：
    - `destination agent clearance`
    - `local avoidance oscillation`
    - `detour candidate distance`
- 需要用户现在做什么：
  - 无
  - 这轮只是把“现在到底对不对”先测实
- 当前 thread-state：
  - 这轮仍属只读 live 验证
  - 保持 `PARKED`

## 2026-04-15 16:42 续记：用户把导航 own 问题压成 3 个根，我认同这个归纳，而且承认我前几轮没按这 3 根来组织施工
- 当前主线目标：
  - 用人话彻底说明：
    1. 我前几轮到底改了什么
    2. 为什么这些修改还没把问题修完
    3. 下一步应该怎样按正确方向落地
- 用户当前对导航的 3 条总诊断：
  1. `NPC 漫游走的都是坏点`
  2. `NPC 的静态导航计算还是垃圾`
  3. `NPC 阻塞卡死了不会自救`
- 我当前重新对账后的判断：
  - 这 3 条成立，而且比我前几轮偏“现象/止血”的拆法更准确。
- 我前几轮真实改动的本质：
  1. `坏点`：
     - 给 autonomous roam 候选加评分，不再完全 first-fit
  2. `静态导航`：
     - released movement 的 next-step constraint 开始吃 body-clearance margin
     - avoidance shell 稍微放大
  3. `自救`：
     - blocked advance / oscillation / retarget / decision reuse 止血
  4. `性能降噪`：
     - 一部分 `FindObjectsByType` 改走 registry
- 为什么这些还不够：
  1. `坏点` 仍没变成统一的 destination hard contract
  2. `静态导航` 仍主要是在“路径出来之后”补保守，不是从路径成立那一层就彻底避免贴边路线
  3. `自救` 仍主要是“更快放弃坏状态”，不是完整稳定的脱困流程
- 这轮我对自己的判断：
  - 我没有失去修复能力
  - 但我前几轮的确在止血层和体验层之间来回，刀法不够直
  - 这不是可以甩给上下文过长的借口
- 当前恢复点：
  1. 后续必须按这三个主根重排施工，不再按症状散修
  2. 当前对用户的交付应是清楚说明“我改了什么 / 为什么不够 / 下一步怎样真正落地”

## 2026-04-15 17:02 续记：用户要求导航线程直接交正式总交接文档，本轮已完成并停车
- 当前主线目标：
  - 不再给局部 prompt 或零散回卡，直接交一份能让后续接手者独立恢复上下文的正式总交接文档。
- 本轮子任务：
  - 把导航 own 的历史语义、用户裁定、Day1/NPC 边界、代码地图、历史修改、已证实与未证实事实、以及后续接手建议，一次性写进一个正式文件。
- 本轮实际做成了什么：
  1. 已新增：
     - [2026-04-15_导航线程总交接文档_own问题_边界语义_历史修改_后续接手.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-15_导航线程总交接文档_own问题_边界语义_历史修改_后续接手.md)
  2. 文档内容已覆盖：
     - 用户给 `Day1` 的 opening/dinner/free time/night 语义
     - `prompt_68` 的 Day1/Navigation 边界
     - `NPC` 线程关于 low-level public API / facade 的判断
     - 导航核心代码总地图
     - 历史修改时间线
     - 当前 3 个主根问题
     - 后续 work package 建议
     - 所有历史材料入口路径
- 当前阶段：
  - 导航线程已完成“正式交接整理”
  - 当前不处于继续施工态
- 本轮判断：
  - 这份总交接文档比继续写一个“唯一下一刀 prompt”更适合当前现场，因为问题已经涉及 `Navigation / Day1 / NPC` 三方接缝，不适合再靠单线短 prompt 传达。
- 当前最薄弱点：
  - 这轮交的是“恢复上下文与接手地图”，不是“已经修完”
- 本轮自评：
  - `8/10`
  - 该说清的都说清了，但它的价值是交接，不是完成修复
- 当前 thread-state：
  - `Begin-Slice=已跑`
  - `Park-Slice=已跑`
  - 当前状态=`PARKED`
- 当前恢复点：
  - 后续任何继续接手导航 own 的线程，应先读这份总交接文档，再决定分包施工

## 2026-04-19 只读续记：Town 空气墙排查已把主嫌压回 scene 真实 collider
- 本轮仍是只读，未进真实施工，未跑 `Begin-Slice`。
- 当前主线目标：
  - 回答 `Town` 里“看起来空白的位置撞墙”更像哪类机制，并明确哪些证据最可疑、哪些点不要误判成 `TraversalBlockManager2D`。
- 本轮子任务：
  - 只读审计 [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 与 [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs)。
- 本轮实际做成了什么：
  1. 确认 `Town` 里的 `TraversalBlockManager2D` 当前是“手动阻挡源 + 自动收集”双开态，不是空配置：
     - `blockingTilemaps`、`blockingColliders` 已显式落盘
     - `autoCollectSceneBlockingTilemaps=1`
     - `autoCollectSceneBlockingColliders=1`
  2. 确认它自己不会生成新的物理碰撞体：
     - 它只把现有 `Tilemap/Collider2D` 收集后塞给 `NavGrid2D`
     - 再把 nav-grid / bounds 约束绑定给玩家和 NPC。
  3. 确认 `Town` scene 里已经真实存在多组非 trigger 的大体量 tilemap 碰撞体：
     - `SCENE/LAYER 1/Tilemap/"基础设施"/-9970`
       - `Tilemap size = 52x20`
       - `TilemapCollider2D + CompositeCollider2D + Rigidbody2D`
       - 还带 `Building` tag
     - `-9960`
       - `10x12`
       - `TilemapCollider2D + CompositeCollider2D`
     - `-9975`
       - `6x12`
       - `TilemapCollider2D + CompositeCollider2D`
  4. 还确认 `轨道` 组 `Layer 1 - Props*` 也都是真 `TilemapCollider2D`，只是尺寸窄，容易表现成“一条不明显的挡线”。
  5. `Layer 1 - Farmland_Water` 虽然在 manager 的 `blockingColliders` 里，但当前 `Tilemap size = 0x0`，本轮不该把它当主嫌。
- 当前最核心判断：
  - 这次空气墙更像 scene 里真实存在的 `TilemapCollider2D / CompositeCollider2D` 在拦人；
  - `TraversalBlockManager2D` 更像把这些现成阻挡再同步进导航和 bounds 约束，会放大“这里不能走”的感受，但不是凭空造墙的第一责任点。
- 最薄弱点：
  - 本轮没有开 live scene 去抓玩家撞点，所以还不能把“具体哪一组 collider”压到单一 object；
  - 但静态证据已经足够把第一排查方向收敛到 `基础设施/-9970/-9960/-9975` 与 `轨道/Props_*`。
- 本轮自评：
  - `8.5/10`
  - 机制和主嫌已经收窄得比较干净，薄弱点只剩缺一张 live 撞点对位图。
- 当前恢复点：
  1. 如果后续继续只读，就先对位用户撞点所在区域与上述几组 scene collider 的空间位置
  2. 不要把第一刀误发给 `TraversalBlockManager2D.cs` owner 去盲改逻辑
