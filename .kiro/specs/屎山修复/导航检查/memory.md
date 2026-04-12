# 导航检查 - 活跃入口记忆

> 2026-04-10 起，旧超长母卷已归档到 [memory_0.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/memory_0.md)。本卷只保留问题域分流和恢复点。

## 当前定位
- 本工作区不再把所有导航问题塞进一条主卷。
- 当前导航问题必须按 3 个问题域分流：
  1. 场景 traversal 配置与桥水
  2. 跨场景 persistent 重绑
  3. NPC 自漫游与峰值止血

## 当前状态
- **最后校正**：2026-04-10
- **状态**：活跃卷已分流

## 当前活跃问题域
- [01_场景Traversal配置与桥水](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/01_场景Traversal配置与桥水/memory.md)
- [02_跨场景Persistent重绑](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/02_跨场景Persistent重绑/memory.md)
- [03_NPC自漫游与峰值止血](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/03_NPC自漫游与峰值止血/memory.md)

## 当前稳定结论
- 当前导航问题不是单一 bug，也不是“导航全推倒重来”
- shared traversal kernel 应保留
- 真正还在活的痛点已经分成 scene contract、runtime bridge、driver 过重三层

## 当前恢复点
- 后续新问题先归类，再进入对应问题域 memory
- 查旧 prompt、旧锐评、旧 V2 审稿历史时，再回看 `memory_0.md`

## 2026-04-10 补充恢复点
- `03_NPC自漫游与峰值止血` 当前新增一个必须分开的判断：
  1. profiler 里旧式 `OverlapCircleAll` 峰值链
  2. 当前 live 体验里的 NPC 朝向左右乱翻
- 前者当前更像旧 binary / 旧采样证据；后者才是当前 live 代码仍未收掉的朝向链问题。
- 新增第三层恢复点：
  - 即便 auto-roam 本体已经改成“朝向跟最终 velocity”，也不能直接等于“Town/day1 全运行时朝向链统一”
  - 后续凡是继续出现 NPC 左走右翻或类似鬼畜，必须优先检查外部 `SetFacingDirection(...)` 调用链是否还在抢 owner

## 2026-04-10 新增续工入口
- `spring-day1` 基于用户最新裁定，已给导航线程落了一份“第一轮认知同步 prompt”，不是直接施工 prompt：
  - [2026-04-10_导航线程_NPC静态导航与owner问题认知同步prompt_63.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-10_导航线程_NPC静态导航与owner问题认知同步prompt_63.md)
- 这份 prompt 的目的不是继续压 stopgap，而是先对齐 3 件事：
  1. 用户主诉并不只是朝向抽搐，而是 `NPC` 连静态世界都走不明白
  2. `性能止血` 不能继续拿来交换“正常导航能力”
  3. 当前问题很可能不只是 spike，而是 `NPC` 导航 contract / owner / 封闭 roam 域约束没有统一
- 当前恢复点：
  - 先等导航线程回这份认知同步回执
  - 再决定第二轮是否发真实施工 prompt

## 2026-04-10 认知同步回执摘要
- 当前回执核心判断已经成形：
  - 用户主诉不只是朝向抽搐，而是 NPC 静态世界 contract 没收平
  - `性能止血` 不是可以交换正常导航能力的理由
  - 当前问题应分成 `owner / 可达性与不可达处理 / roam domain / storm` 四层，而不是继续混成“只是朝向”或“只是 profiler”
- 下一轮最值钱的目标，不是再泛修，而是先验证：
  - 某个坏 NPC 是否在语义上越界的 roam domain 里持续采样坏目标
  - 以及 bad case 进入后到底是谁在持续接管 motion/facing/rebuild

## 2026-04-10 prompt_64 窄验证摘要
- `03_NPC自漫游与峰值止血` 已完成第二轮窄验证，只读结论已经足够支撑下一轮第一刀施工 prompt。
- 当前最重要的新裁定：
  - 两个代表性 bad case 的第一失真点都已落在 `坏目标采样 / roam domain contract`，不是先落在 `avoidance`，也不是先落在 `owner 抢写`。
  - 但两者的立即触发原因不同：
    1. 居民 case：`homeAnchor / staging slot` 错配，导致采样圆心一开始就是错的。
    2. 小动物/封闭区 case：没有“围栏内语义域”，只有 raw 半径圆，导致采样本身就能落到 fence 边或 fence 外侧的最近 walkable。
- `NPC vs Player` 的 `clearance / avoidance / blocker` 差异已核出为真实存在，但本轮判断它属于“坏相放大器”，不是这两个 static case 的第一颗骨牌。
- 当前恢复点：
  - 下一轮第一刀应只修 `NPC autonomous roam 的目标域 contract`
  - 不应顺手回吞 `facing owner`、`泛 spike 止血` 或统一大重构

## 2026-04-10 第一刀施工已落地
- 导航线程已按 `prompt_64` 的后续施工方向，实际落地了 `NPC autonomous roam 目标域 contract` 第一刀。
- 改动范围严格压在：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
- 已落的修法与第二轮窄验证结论完全对应：
  1. `错位 homeAnchor` 不再继续主导 roam center
  2. autonomous roam 不再接受大幅纠偏回来的 `nearest walkable`
- 当前 fresh 代码层证据：
  - `validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
    => `assessment=unity_validation_pending / owned_errors=0 / external_errors=0`
  - `git diff --check` 针对目标脚本通过
- 当前仍未补到 fresh Unity live runtime 证据；原因不是代码红，而是本机 MCP 基线未连接。
- 当前恢复点：
  - 如果继续，下一步优先做 fresh live 验证，先看 `Town 101/201` 与 `小动物围栏边` 两类对象是否不再抽歪目标

## 2026-04-10 NPC朝向反向快修（第二刀）
- 用户新主诉切到 `NPC 倒着走 + 摇头晃脑`，并明确要求先快速自查再修。
- 本轮先做了 Town live 快验（只读）：
  - 通过 `Tools/Sunset/NPC/Run Roam Spike Stopgap Probe` 在运行态采样 6 秒，fresh 报告：
    - `scene=Town`
    - `npcCount=25`
    - `roamNpcCount=7`
    - `avgFrameMs=0.825`
    - `maxFrameMs=11.316`
    - `maxGcAllocBytes=229982`
    - `maxBlockedNpcCount=0`
    - `topSkipReasons=AdvanceConfirmed`
  - 结论：这轮主症状不是“路径重建风暴”，而是“朝向链 owner 冲突/反向视觉”。
- 随后进入真实施工，只改：
  - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
- 这刀落了两条朝向保护：
  1. `SetFacingDirection` 在外部速度驱动期间，若强制朝向与当前移动方向相反，则自动钳回移动方向，避免“倒着走”。
  2. `ResolveFacingVelocity` 新增“指令速度与真实位移反向”判定：反向时优先采用真实位移朝向，避免左右来回翻脸。
- 代码层验证（fresh）：
  - `validate_script Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs --count 20 --output-limit 5`
    => `assessment=no_red / owned_errors=0 / external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs` 通过
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑（本轮未收口 sync）`
  - `Park-Slice=已跑`
  - 当前 `PARKED`

## 2026-04-10 第三刀：NPC完好系统收口
- 这轮不再泛讲“统一导航长期方案”，而是直接收 `NPC` 当前运行态最关键的两件事：
  1. 移动中的朝向 owner 收成单一真源
  2. 静态坏 case 更早熔断，避免 wall-thrash 和性能炸点
- 实际只改：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
  - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
- 没碰：
  - scene / prefab
  - 玩家链
  - director/crowd 脏文件
- 当前新的稳定判断：
  - `NPC` 不需要继续叠更多补救逻辑；关键是把“移动时谁说了算”和“静态死路何时放弃”收平
  - 这轮的朝向修法避免了 locomotion 期间被 misaligned `SetFacingDirection(...)` 扯脸
  - 这轮的 stop-loss 修法避免了静态坏 case 连续 rebuild / hard-push
- fresh 证据：
  - 合并 `validate_script` 两脚本 => `assessment=no_red`
  - Town 稳态探针：
    - `avgFrameMs=0.887`
    - `maxFrameMs=15.256`
    - `maxGcAllocBytes=37197`
    - `maxBlockedNpcCount=0`
    - `maxConsecutivePathBuildFailures=0`
- 当前恢复点：
  - 下一步如仍有视觉鬼畜，不再盲修路径，而是直接追 `SetFacingDirection(...)` 外部 owner
  - 下一步如仍有极少数卡墙，再继续压 `TryBeginMove / blockedAdvance` 的 bad-target 入口，而不是回退这轮性能止血

## 2026-04-10 鬼畜朝向诊断输出
- 这轮新增了 `NPC` 运行态鬼畜诊断，而不是继续凭截图猜：
  - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
- 关键能力：
  - 自动记录最后一次 `facing / externalVelocity / externalFacing` 的写入源
  - 当 `observedDir != appliedDir` 连续出现时，直接打 `[NPCFacingMismatch]`
- 这使得后续同类问题可以直接回答：
  - NPC 真正在往哪走
  - 屏幕上脸朝哪边
  - 最后是谁把脸写歪了
- Town 短窗口当前未复现该日志，说明：
  - 诊断已挂好
  - 但坏 case 更可能在 `Primary` 或更长的 free-roam 窗口里

## 2026-04-10 鬼畜朝向第四刀：burst 诊断 + 瞬时噪声止损
- 用户把坏相重新描述为：
  - `NPC` 持续往一个方向移动，但脸在左右来回翻
  - 这不是“路径回头”，而是“朝向判定被抖坏”
- 本轮继续只改：
  - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
- 关键收口：
  1. `[NPCFacingMismatch]` 不再只依赖连续多帧 mismatch，现已补成 burst 检测，能抓间歇性翻脸。
  2. `ResolveFacingVelocity(...)` 不再让一两帧短暂反向观测立即改脸；必须同一冲突方向持续超过确认窗口，observed movement 才能抢回朝向。
  3. `showDebugLog` 输出已补上 `ObservedDir / AppliedDir / ExternalFacing / 各写入源`，可以直接对照用户肉眼看到的鬼畜。
- fresh 证据：
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs --count 20 --output-limit 5`
    => `assessment=no_red`
  - fresh console => `error_count=0 / warning_count=0`
- 当前判断：
  - 这刀优先修的是我自己在朝向层放大的瞬时噪声，不是去改 NPC 漫游业务语义
  - 如果后续仍有翻脸，下一步就不是盲修，而是直接根据 `[NPCFacingMismatch]` / trace 的 owner 继续定责

## 2026-04-11 resident scripted control 冻结链修补与 Town 主线重锚
- 这轮用户先确认了 prompt_65 可以开工，但随后补充了更关键的现场事实：
  - `Town` 才是主现场
  - `001~003` 路径与朝向基本正常，但避障回退
  - 其它常驻 NPC 与小动物仍主要坏在 autonomous / free-roam 阶段
  - 剧情预设走位基本正常
- 所以这轮我没有把 `resident scripted control` 包装成“总根因”，而是分成两层报实：
  1. 这条链里确实存在一个真实 own bug，需要先补
  2. 但 Town 自漫游 `避障回退 + 朝向乱飘` 仍是下一条独立主线
- 本轮只在 own 文件里补了一个现实层漏洞：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
- exact 责任链已钉死：
  - [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs#L529) `AcquireResidentScriptedControl(...)`
  - [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs#L534) `_roamController.enabled = false`
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L543) `OnDisable()`
  - 旧逻辑这里会直接 `StopRoam()`
  - 而 `StopRoam()` 会清掉 `resumeRoamAfterResidentScriptedControl`
  - 导致 [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs#L583) `ReleaseResidentScriptedControl(..., _roamWasRoaming)` 发生后，也可能不再自动恢复 `StartRoam()`
- 本轮最小修补：
  - `OnDisable()` 现在在 `IsResidentScriptedControlActive` 时不再走 `StopRoam()`
  - 改为走 `HaltResidentScriptedMovement()`，以保留“release 后恢复 roam”的意图
- fresh 代码层验证：
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
    => `assessment=no_red / owned_errors=0 / external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 通过
  - fresh console：
    - `owned warnings/errors = 0`
    - 仅有 1 条 external warning：`MCP-FOR-UNITY [WebSocket] Unexpected receive error: WebSocket is not initialised`
- 当前阶段判断：
  - resident scripted control 冻结链里的一颗真 bug 已经补掉
  - 但它不是用户当前主痛点的全部答案
  - 下一条最值钱主线必须切到 `Town autonomous/free-roam` 的 `避障回退 + 朝向乱飘`
- 已为下一轮生成精确 prompt：
  - [2026-04-11_导航线程_Town自漫游避障回退与朝向乱飘收口prompt_66.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-11_导航线程_Town自漫游避障回退与朝向乱飘收口prompt_66.md)
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑（本轮未收 sync）`
  - `Park-Slice=已跑`
  - 当前 `PARKED`

## 2026-04-11 Town 自漫游第二刀：shared avoidance 现实速度语义修补
- 这轮继续不漂移，只收 `Town` 常驻 NPC 的两类坏相：
  - `001~003` 避障回退
  - 其它常驻 NPC 路径对但朝向乱飘
- 当前新的主判断：
  - 这两类坏相更像同根
  - 第一真问题仍在导航 own，而且落在 `NPCAutoRoamController -> NavigationAgentSnapshot` 这层“速度语义失真”
- 本轮只改：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
- exact 补口：
  1. [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L374)
     - `GetCurrentVelocity()` 现在优先取 `motionController.CurrentVelocity`
     - 再退到 `rb.linearVelocity`
     - 最后才退到 `motionController.ReportedVelocity`
  2. [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L403)
     - `IsNavigationSleeping()` 现在同时看现实速度，而不是只看 `state`
- 为什么这刀同时影响两类坏相：
  - `避障回退`：
    - shared avoidance 之前更容易拿到“命令意图速度”，拥挤 NPC 明明已经被接触/约束，snapshot 还在上报“我在正常稳走”
    - solver 因此更难把它们当成真正该绕开的对象
  - `朝向乱飘`：
    - 当交通判断错了，现实接触挤压就会变多
    - face 层被碰撞噪声放大的机会也会跟着变多
- fresh 代码层验证：
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
    => `assessment=no_red / owned_errors=0 / external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 通过
- 当前没站住的部分：
  - 这轮还没拿 fresh Town live 体验证据
  - 小动物是否完全同根，我当前只报“高度可疑同根”，不报死结论

## 2026-04-11 Town 常驻 NPC 只读分层：crowd owner 已成 exact blocker
- 这轮严格按 `prompt_66` 做只读复盘，没有继续修改导航代码。
- 新增最关键结论：
  1. `101~301` 这组常驻居民属于 `SpringDay1NpcCrowdManifest`
  2. `001~003` 不在这条 crowd owner 链里
  3. 所以两组坏相不能再继续混报成“全是导航 own 同一根因”
- exact 代码证据：
  - [SpringDay1NpcCrowdManifest.asset](D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset)
    - 只登记了 `101~301`
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L1730)
    - `ApplyResidentBaseline(...)` 在居民已交回 roam 后仍执行 `motionController.StopMotion()` + `SetFacingDirection(state.BaseFacing)`
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L1841)
    - `TickResidentReturnHome(...)` 也在持续写 `SetFacingDirection(step)`
- fresh live 证据：
  - `py -3 scripts/sunset_mcp.py status`
  - fresh console 直接命中：
    - `104 -> facingSource=SpringDay1NpcCrowdDirector.ApplyResidentBaseline`
    - `103/202 -> facingSource=SpringDay1NpcCrowdDirector.TickResidentReturnHome`
    - 说明 severe 摆头/倒走已经出现 exact external owner
- 对 `001~003` 的当前判断：
  - 它们能基本走稳，恰好反证没有被 crowd director 同步 stomp
  - 但“避障回退”仍像导航 own 残留问题，本轮没有 fresh Town 直证把这部分单独跑死
- 对小动物的当前判断：
  - fresh console 里鸡/牛的 mismatch 来源仍是 `NPCAutoRoamController.ApplyMoveVelocity`
  - 所以小动物更像 motion/facing 合成层的另一条子问题，不和 crowd owner 完全同根
- 额外验证报实：
  - 本轮尝试用 `NpcRoamSpikeStopgapProbe` 取 `Town` live
  - 但 Play 起场景被 guard 导到了 `Primary`
  - 所以这次 probe 只拿到了 `Primary` 的 2 个 NPC，不足以给 `Town` 常驻居民下 fresh 体验结论
- 当前阶段：
  - `Town` 常驻居民最严重的朝向乱飘，已能 exact 定责到 `SpringDay1NpcCrowdDirector`
  - `001~003` 的避障回退，以及小动物的 face lag，仍留在导航 own / motion own 待下一轮窄验

## 2026-04-11 导航线施工：Town resident crowd owner 正式接线到 resident scripted control
- 这轮从只读进入真实施工，目标不是回滚导航，而是保留当前玩家 traversal/性能版，只修 `Town` 常驻 resident 的 owner 冲突。
- 实际落地：
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
  - 新增 `ResidentScriptedControlOwnerKey`
  - 增加 `PrepareResidentRoamController / AcquireResidentDirectorControl / ReleaseResidentDirectorControl`
  - resident baseline / return-home / night-rest / reset / snapshot restore 全部改成显式 acquire/release
- 这轮没有继续动：
  - `NavGrid2D.cs`
  - `NavigationTraversalCore.cs`
  - `PlayerMovement.cs`
  - `TraversalBlockManager2D.cs`
  - `NavigationAvoidanceRules.cs`
- fresh 自检：
  - `validate_script CrowdDirector` => `owned_errors=0 / assessment=external_red`
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs` 通过
- fresh 外部 blocker：
  - `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
  - 当前外部测试编译红阻断 Play/live 闭环，不属于这轮 owner 接线 own red
- fresh Town 只读 probe：
  - runtime anchor readiness = `completed`
  - player-facing contract = `attention`，只剩入口群像第一屏视野提示，不是 resident owner blocker
- 当前收口判断：
  - `Town` 常驻 resident severe 朝向乱飘这条线，已从“只读定责”进入“代码已落地”
  - `001~003` 的避障回退与小动物 face lag 仍未在 live 上重新收口
  - 下一轮如果外部测试红清掉，最值钱动作就是直接跑 `Town` fresh live，看 crowd severe 是否消失、再决定是否还要补 NPC own 小口

## 2026-04-12 只读排查：Town / Day1 “NPC 不动 / 像冻结”第一真问题更像 Day1 caller 持续持有 resident scripted control
- 用户本轮主诉：
  - 继续追“到底出了什么问题”
  - 区分 `NPCFacingMismatch` 日志、剧情正常停位、以及真正的 resident freeze bug
  - 不直接改代码，只要先讲清 live 现实层到底是谁在控 NPC
- 这轮实际新钉死的事实：
  1. [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 自己并不缺 `release -> resume roam` 出口。
     - `AcquireResidentScriptedControl()` 会把 roam 置为 `Inactive` 并 freeze
     - `ReleaseResidentScriptedControl()` 在 `resumeRoamAfterResidentScriptedControl == true`、且已无 owner、且不在对白暂停态时，会直接 `StartRoam()`
     - `Update()` 里还有一层兜底：只要 `resumeRoamAfterResidentScriptedControl` 还挂着、owner 已清空，也会再触发一次 `StartRoam()`
  2. 所以“Town 这批人类居民站住不动”不优先像导航 own 忘了恢复，更像 caller 还在持续持有或反复重拿 `resident scripted control`。
  3. `NPCFacingMismatch` 仍只是 [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs) 的编辑器诊断 warning，不是“NPC 现实里彻底不走”的直接证据。
- 当前最像的真实 caller 分层：
  1. `001 / 002` 这组 story escort：
     - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 `ShouldDeferToStoryEscortDirector(...)` 会在 `EnterVillage / HealingAndHP / WorkbenchFlashback / FarmingTutorial / DinnerConflict / ReturnAndReminder` 这些阶段直接返回 `true`
     - 命中后 `ApplyResidentBaseline(...)` 会 `AcquireResidentDirectorControl(state, resumeRoamWhenReleased: false)`，也就是明确把 roam 交给剧情，不是让它自由漫游
  2. `101~301` 这组 Town 常驻居民：
     - 更像由 `SpringDay1NpcCrowdDirector.ApplyStagingCue(...)` + `SpringDay1DirectorStagingPlayback` 持有
     - `ApplyCue(...)` 若 `cue.suspendRoam == true`，会通过 `SpringDay1DirectorNpcTakeover.Acquire()` 拿 `resident scripted control`
     - 只有 `ClearCue()` / `Release()` 真的走到，才会释放控制，再由 `ApplyResidentBaseline(...)` 的 `ReleaseResidentDirectorControl(..., resumeResidentLogic: true)` / `TryBeginResidentReturnHome()` / `FinishResidentReturnHome()` 把居民放回 home 或 roam
- 对当前截图/文案的现实判断：
  - 用户截图里出现的提示文案能对上 [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 里 `StoryPhase.EnterVillage` 的分支：
    - `别掉队，村长和艾拉会停下来等你一起进村里。`
    - `先在村里这边站稳，等艾拉正式接手疗伤。`
  - 这说明当前更像还在 `EnterVillage` / `HouseArrival` 相关流程里，Town 居民整片停位首先应视为剧情 staging 现场，不该直接判成“导航死了”
- 这轮新的责任判断：
  - 如果用户现在看到的是 `Town / Day1 / EnterVillage` 那批人类居民整片站着：
    - 第一真责任点更像 `SpringDay1NpcCrowdDirector` / `SpringDay1DirectorStagingPlayback` / `SpringDay1Director` 这条 caller 链
    - 不是 `NPCAutoRoamController` own 文件缺了 `resume`
  - 真正需要继续追的，只剩一个窄问题：
    - 这批居民是“当前阶段本来就该站住”
    - 还是“cue / phase 明明该结束了，却没 release 或又被重抢”
- 当前阶段判断：
  - `显示层`：`NPCFacingMismatch` warning 的语义已经站住
  - `现实层`：`NPCAutoRoamController` 自己有 release/resume 出口也已站住
  - `未站住`：当前 live 那一屏 Town 居民到底是正常 staging，还是 stage 已过但 cue 没放；这一步如果继续查，第一刀应去 Day1 caller，不该回头泛修导航 own
- 本轮报实：
  - 只读分析，未改代码
  - 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`，因为没有进入真实施工
