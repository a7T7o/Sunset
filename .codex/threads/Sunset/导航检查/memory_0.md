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
