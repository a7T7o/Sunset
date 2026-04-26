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

## 2026-04-13 只读监测：Town 里并不是只有 001~003 接上了避让，其他居民也挂着同一套 roam/avoidance 核
- 用户本轮要求：
  - 只读监测当前 `Town` 现场，直接看 MCP / console，回答“为什么只有 001~003 的避让明显，其他 NPC 像没接上一样”
- 这轮拿到的 live / 结构证据：
  1. 当前场景仍是 [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity)，console 当前没有新的导航/避让红错；最新只看到 [CloudShadowManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs#L670) 的普通 log。
  2. `Town` 当前共能读到 `25` 个挂了 `NPCAutoRoamController` 的对象，不是只有 `001~003`。
  3. 已抽查的 `001 / 002 / 003 / 101 / 103 / 201` 都同时挂着：
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
     - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
  4. `101 / 103 / 201` 的 `avoidanceRadius / avoidancePriority / sharedAvoidanceLookAhead / sharedAvoidanceRepathCooldown` 与 `001~003` 一致，所以当前证据不支持“后面那批 NPC 根本没接避让”。
  5. 真正的结构差异在 owner / 包装层：
     - `001~003` 位于 `NPCs/*`
     - `101~301` 这批 Town 常驻居民位于 `SCENE/Town_Day1Residents/Resident_DefaultPresent/*`
     - 并额外挂了 [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs#L495) 里的 `SpringDay1DirectorNpcTakeover`
     - 同时会经过 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L867) 的 `ConfigureBoundNpc(...)` 绑定链
  6. 运行态字段也对上了这层差异：
     - `101` 的 `NPCMotionController.DebugLastFacingWriteSource = SpringDay1NpcCrowdDirector.ConfigureBoundNpc`
     - `001` 没有这层 caller 痕迹
     - 说明 `101~301` 这批居民确实被 Day1 / resident crowd 体系碰过，不像 `001~003` 那么“纯 roam”
  7. 另一层实际差异是 roam profile：
     - `001~003` 的 `activityRadius` 约 `1.55 ~ 1.85`
     - `101 / 103 / 201` 当前都是 `3.0`
     - 所以后面那批更容易采到更远、更差的目标点，不等于它们没接上避让
- 当前最稳的结论：
  - 不是“只有 001~003 接上了避让，其他 NPC 没接”
  - 更像是：其他居民也在同一套自漫游/避让核上，但被 `Town resident + Day1 crowd/director` 这层 owner/绑定链包了一层，再叠加更大的 roam 半径，所以表现明显更差
  - 也就是说，第一真问题更像“同核不同外层”和“目标/状态层差异”，不是“脚本根本没挂到其他 NPC”
- 这轮结论强度边界：
  - 这轮能钉死“有没有接上”这个结构问题
  - 但不能钉死“当前 live 某一帧为什么没避开”，因为我收尾时 Unity 已回到 `Town` 的 Edit 模式，不再是新的跑动窗口；所以本轮不是新的 movement proof，只是结构 + 刚才抓到的运行态证据
- 本轮报实：
  - 只读监测，未改代码
  - 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`，因为没有进入真实施工

## 2026-04-13 fresh 归因补丁：静态障碍贴边/钻碰撞体与 Day1 resident 冻结已经可以分锅
- 用户最新主诉：
  - `Town` 里树、房屋、围栏这些静态障碍虽然 tag/collider 都配了，NPC / 小动物仍会把路线算进碰撞体边缘，卡在小区域还不够积极地重规划
  - 同时 day1 还同步了：`002` 后 `Town resident` 整片冻结已经被 fresh probe 钉到 `SpringDay1NpcCrowdDirector` 的 runtime registry 恢复漏口
- 本轮新增压实的结论：
  1. `Town resident` 整片冻结这件事，后续不要再先回导航泛排查。
     - day1 已明确把第一真锅钉到 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 `spawnStates/runtime registry` 恢复漏口
     - 这和“树/房屋静态障碍 clearance 算得太松”是另一类问题
  2. 用户给树/房屋挂 tag 不是当前第一真问题。
     - 我抽查的树/房屋子物体本身就挂了 `PolygonCollider2D`，tag 也在子链上
     - [NavGrid2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs#L2287) 的 `HasAnyTag(...)` 还会沿父链往上查
     - 所以不能再把当前坏相简单归因成“标签没挂到 collider”
  3. 当前静态导航真正的硬伤是：`occupiable / clearance` 判定半径明显小于 NPC/动物真实身体宽度。
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L195-L197) 当前默认：
       - `navigationFootProbeVerticalInset = 0.08`
       - `navigationFootProbeSideInset = 0.05`
       - `navigationFootProbeExtraRadius = 0.02`
     - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs#L520-L534) 的 `GetNavigationPointQueryRadius(...)` 会把静态 occupancy 查询半径压到大约 `0.07 ~ 0.10`
     - 但同一只 NPC 的动态避让半径在 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L211-L214) 里却是 `avoidanceRadius = 0.6`
     - 也就是说：当前路径/占位是在用“很窄的脚底探针”判定能不能过，不是在用“整个人/整只动物的真实 clearance”判定能不能过
     - 这正好对上用户截图里“绿色路线贴着树根/屋角/围栏边，甚至算进白色碰撞体边缘内部”的坏相
  4. `Town` 当前 scene config 还额外把导航推成了“显式阻挡源模式”。
     - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs#L465-L468) 只要显式数组非空，就视为已配置 traversal source
     - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs#L526-L532) 当前 `autoCollectSceneBlockingColliders = false`，而且会在显式源存在时直接短路
     - 所以这张图不是“全场景 blocker 自动吃进来”的鲁棒模式，而是“显式名单 + 少量关键词补充”的脆弱模式
     - 因此：tag 不是万能入口；真正生效的仍是“物理 collider 是否被 occupancy 查询命中”与“scene blocker source 是否纳入 NavGrid”
  5. “把玩家算法下放给 NPC”不能简单理解成另抄一套，因为玩家和 NPC 当前已经共用 [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs)。
     - 现在的问题不是“NPC 没吃到玩家 core”
     - 而是这套 shared core 自己对静态障碍的 clearance 半径定义偏小
  6. 低成本卡住重规划是可以做的，而且当前已有半套骨架，不需要从零发明。
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1637-L1707) 已有 `CheckAndHandleStuck(...)`
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L3651-L3670) 已有 `MoveCommandNoProgress`
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L3528-L3582) 已有 blocked advance -> rebuild / reroute / long-pause 链
     - 说明“卡住就低频重规划”本来就不是空白，只是当前对静态窄 clearance 场景不够积极，且太容易退成 stop/pause
- 当前总判断：
  - `Day1 resident 冻结` = `spring-day1` crowd registry 恢复漏口
  - `树/房屋/围栏等静态障碍贴边、钻边、算进碰撞体` = `shared traversal core 的 static clearance 半径太小 + Town 阻挡源模式较脆弱`
  - `小动物/NPC 在小区域互挤互卡后不够积极重规划` = `现有 stuck / blocked recovery 已有骨架，但对静态卡死场景恢复出口太保守`
- 本轮报实：
  - 只读分析，未改代码
  - 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`，因为没有进入真实施工

## 2026-04-13 施工落地：shared traversal static clearance 收紧 + NPC 低成本 blocked/stuck reroute 提升
- 用户这轮批准：
  - 直接开始修，不再停留在只读分析
  - 核心目标保持不变：`保有功能 + 保有性能`
- 本轮实际改动：
  1. 在 [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs) 给 `CanOccupyNavigationPoint(...)` 补了第二层 expanded static clearance 校验。
     - 以前：只用很小的脚底 query radius（对 NPC 约 `0.07~0.10`）判能不能站
     - 现在：在基础 bridge / 左右脚 probe 通过后，再额外用更接近真实身体宽度的 blended radius 做一次 `center + left/right` 静态 clearance 校验
     - 目的：不再让 NPC / 动物把路径和站位算进树根、屋角、围栏边的碰撞体边缘
     - 这层仍保留 bridge support 特判，不直接破坏桥/水 traversal contract
  2. 在 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 把 `blocked / stuck` 恢复顺序改成“先 fresh reroute，再 stopgap”。
     - `TryHandleBlockedAdvance(...)`：先尝试 `TryBeginMove(...)` 重新采样目的地，再决定是否停
     - `CheckAndHandleStuck(...)`：先尝试 fresh reroute，再走 `StuckCancelStopgap`
     - `TryHandlePassiveAvoidanceHardStop(...)`：同样把 fresh reroute 提前
  3. 同时把 autonomous roam 的 `static abort` 阈值放宽。
     - 新增 `AUTONOMOUS_STATIC_ABORT_MIN_FRAMES = 6`
     - 避免 NPC / 小动物在静态障碍附近刚卡两下就直接停住，不给 reroute 机会
- 为什么这刀符合“功能 + 性能”：
  1. 功能侧：
     - shared traversal core 不再只按“脚底针尖”判断可站立
     - 静态路径/占位会更尊重真实身体 clearance
  2. 性能侧：
     - 没有改成每帧全量重规划
     - 只是把已有的 `TryBeginMove / TryRebuildPath / blockedAdvance` 骨架调成更合理的先后顺序
     - reroute 仍挂在原有 cooldown / threshold 体系里，属于低成本恢复，不是暴力频繁重算
- 当前验证：
  - `validate_script`：
    - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs) `errors=0 warnings=0`
    - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) `errors=0`，仅剩一个既有级别的 GC warning（非这轮新红）
  - Unity refresh/compile：
    - `refresh_unity(scope=scripts, compile=request, wait_for_ready=true)` 已请求
    - fresh console 当前 `error=0 warning=0`
  - 这轮尚未做新的 live roam/play proof；属于 `代码层验证已过，体验层仍待 live 复测`
- thread-state 报实：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：待本轮 memory 结算后执行

## 2026-04-13 本轮收口状态：代码已落地并编译 clean，但 Ready-To-Sync 被 same-root 历史脏改阻断
- 本轮最终收口结果：
  1. 代码层：
     - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs) 已落 expanded static clearance
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 已落 blocked/stuck 先 reroute 再 stopgap
  2. 验证层：
     - `validate_script` 两个脚本均 `errors=0`
     - Unity fresh console `error=0 warning=0`
     - `git diff --check` 仅剩 CRLF/LF 提示，无新的格式红
  3. 治理层：
     - `Ready-To-Sync` 没通过
     - 第一 blocker 不是这两刀代码，而是 same-root 旧账：`Assets/YYY_Scripts/Controller/NPC/*` 与 `Assets/YYY_Scripts/Service/Navigation/*` 下还存在别的 remaining dirty/untracked
     - 因此这轮正确状态不是“可直接 sync”，而是“代码已落地、治理同步被同根旧账合法阻断”
- 当前 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：已跑，结果 `BLOCKED`
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`

## 2026-04-12 收口检查：导航检查线程当前 repo 内 own roots 已无可合法归仓差异
- 用户本轮要求：
  - 让我按历史 memory 和当前线程 memory，把“我自己能提交的内容”全部按规范检查并提交干净
- 本轮执行：
  1. 对 `导航检查` 线程自己的 repo 内 own roots 做白名单核对：
     - `.kiro/specs/屎山修复/导航检查/memory.md`
     - `.codex/threads/Sunset/导航检查/memory_0.md`
  2. 先跑 `Begin-Slice`，只登记这两个 own 路径
  3. 用 `git status --short -- <own roots>`、`git diff -- <own roots>`、`git log -- <own roots>` 复核
  4. 结论：当前这两个 own roots 在仓库内已经没有待提交差异，不能为了“看起来有动作”强行凑空提交
- 当前判断：
  - 我这条线程 repo 内“自己能合法提交”的内容当前是空集
  - 仓库里虽然还有大量 dirty/untracked，但都不属于这轮 `导航检查` 线程可安全吞并的 own 白名单
  - 其中 `Assets` 大面当前仍是 shared-root / 多线程混合现场，不能被我借“顺手提交”吞进去
- 补充发现：
  - 当前阻断 live 的第一外部红，不在导航 own，而在 [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
  - 所以即使我想继续跑 Town/NPC live，也会先被外部 compile red 挡回 EditMode
- thread-state 报实：
  - `Begin-Slice`：已跑
  - 因为 own roots 无差异，本轮没有进入 `Ready-To-Sync`
  - 已跑 `Park-Slice`
  - 当前状态按脚本返回值为 `PARKED`
## 2026-04-13 导航检查自查总账：当前不是“导航整套失效”，而是 3 类问题混在一起
- 用户本轮要求：
  - 不继续改代码，先把“现在导航还没做好的问题、用户真正要的导航形态、以及下一步解决清单”一次性讲清楚
- 这轮只读自查新增结论：
  1. 当前导航问题必须分成 3 类，不能再混成一个“NPC 导航全坏”：
     - `Day1/Town resident 冻结`
     - `静态障碍 clearance / blocker source / 坏目标采样`
     - `高密度动物/NPC 的 reactive avoidance 不够`
  2. `Town resident 冻结` 当前优先不归导航 core。
     - day1 同步文档已把它钉到 [2026-04-12_给导航_002后TownResident冻结根因与当前修复同步.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/2026-04-12_给导航_002后TownResident冻结根因与当前修复同步.md#L12) 所述的 `SpringDay1NpcCrowdDirector` runtime registry 恢复漏口
  3. 静态障碍问题也不能再归因成“用户 tag 没挂对”：
     - [NavGrid2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs#L2287) `HasAnyTag(...)` 会沿父链查
     - 但 [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs#L465-L532) 决定了 `Town` 当前是显式阻挡源模式，不是鲁棒全自动收集模式
  4. 当前静态路线贴边、算进树根屋角的第一真问题，仍然是 shared traversal core 对身体 clearance 估得太小：
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L196-L212) 的脚底 probe 很小
     - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs#L148-L200) 以前只按这类小半径做 occupancy 判定
     - 已落的 expanded static clearance 是对这条口子的第一刀
  5. 但这还没有把问题彻底关门：
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1567-L1587) 的 autonomous roam 仍是 `Random.insideUnitCircle * activityRadius`
     - 它只保证“找得到能站点”，还没有保证“目标仍在同一 enclosure / 同一局部可达域”
  6. 高密度 pen / 住民拥挤问题也还没真正解决：
     - [NavigationLocalAvoidanceSolver.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs#L396-L597) 仍是反应式 sidestep/slowdown，不是 reservation/crowd flow
     - 所以在小动物密集、小院子、围栏窄口这类 case 里，仍可能互挤、停滞、反复重试
  7. 路径执行层本身也有结构边界：
     - [NavigationPathExecutor2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs#L238-L380) 本质仍是中心点 A* + smoothing
     - 即使终点 occupancy 更严，中段路径也可能视觉上仍然贴边
- 这轮同时报实：
  - 用户之前给的临时截图路径已失效，本轮无法重新打开 temp 图片
  - 但基于现有代码证据、先前现场口头描述和 day1 fresh 同步，问题分层已经足够站住
- 当前恢复点：
  - 如果下一轮继续施工，最值钱顺序应是：
    1. `autonomous roam target sampling` 收紧到同一可达域 / enclosure
    2. `Town blocker source` 做一次 scene 审计，确认显式名单覆盖完整
    3. 只在这两层仍不够时，再补高密度 jam breaker

## 2026-04-13 真实施工：坏目标采样过滤 + Town blocker source 收口
- 用户这轮已明确放行真实施工，并允许使用 subagent。
- 本轮已进入真实施工，`Begin-Slice` 已跑：
  - slice = `navigation-enclosure-and-town-blocker-hardening`
  - own 路径包含：
    - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
    - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs)
    - [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity)
- 这轮实际落地：
  1. `NPCAutoRoamController`
     - 在 [TryBeginMove](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1552) 接入 built-path 过滤
     - 在 [TryResolveAutonomousRoamDestination](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L2283) 接入邻域 clearance 过滤
     - 新增：
       - [HasAutonomousRoamDestinationNeighborhoodClearance](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L2339)
       - [IsAutonomousRoamBuiltPathAcceptable](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L2378)
       - [ComputeCurrentPathTravelDistance](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L2417)
     - 目标：不再接受“贴墙/贴树根的 tight 点”和“虽然能建路但明显绕远、像跨 enclosure 的坏目标”
  2. `TraversalBlockManager2D`
     - [GetResolvedBlockingTilemaps](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs#L494) 与 [GetResolvedBlockingColliders](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs#L523) 改成“手工名单 + auto collect 可并存”
     - 修复原先“只要手工名单非空，auto collect 就完全失效”的短路
  3. `Town.unity`
     - 运行中把 [TraversalBlockManager2D](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity#L180421) 的 `autoCollectSceneBlockingColliders` 打开并已保存
- 当前验证：
  - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs) `validate_script => errors=0 warnings=0`
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) `validate_script => errors=0`，仅 1 条旧 GC warning
  - Unity fresh console：无新的导航红错
  - 短时 Play/Stop 已跑通并退回 EditMode；当前看到的 2 条 warning 为：
    - MCP WebSocket transport warning
    - `LiberationSans SDF (Runtime)` 静态字体 atlas warning
    - 都不是这轮导航新红
- 当前风险报实：
  - [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 当前 git diff 非常大，不只包含我这一轮 intended 的一个布尔开关
  - 我已确认 `autoCollectSceneBlockingColliders: 1` 在文件上生效
  - 但整张 `Town.unity` 仍是 mixed scene 现场，不能把整份 scene diff 都当成我这轮可安全同步的产物

## 2026-04-13 导航检查只读复盘：fresh profiler 已判定这轮止血没有命中主热点
- 用户最新要求：
  - 不再继续修
  - 先做只读自我审核，并用人话说明：性能为什么还炸、我修错了什么、用户真实需求是什么
- 这轮新增压实的判断：
  1. 当前主热点仍应优先归到 [NPCAutoRoamController.Update](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L467) 驱动的自漫游热链，而不是先甩给 `day1`、UI、镜头或泛场景。
  2. 我这轮确实把更多 correctness 过滤塞进了 hottest loop：
     - [TryBeginMove](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1552) 现在在 `pathSampleAttempts = 12` 的采样循环里调用 [TryResolveAutonomousRoamDestination](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L2283)
     - 其中 [HasAutonomousRoamDestinationNeighborhoodClearance](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L2339) 最多会额外打 `4` 次 `CanOccupyNavigationPoint(...)`
     - 建路成功后还会再走 [IsAutonomousRoamBuiltPathAcceptable](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L2378)
     - 这说明我加的不是冷路径补口，而是把更多 occupancy/path 校验直接叠进了热循环
  3. 我还把共享 occupancy 契约本身加重了：
     - [NavigationTraversalCore.CanOccupyNavigationPoint](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs#L148) 现在除基础 probe 外，还会继续走 [HasExpandedStaticClearance](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs#L520)
     - 所以变重的不只是 roam 选点，而是所有共用这条 occupancy contract 的查询
  4. 当前 live 文本源码里，导航主链仍是 non-alloc `Physics2D.OverlapCircle(..., cache)`：
     - [NavGrid2D.IsPointBlocked](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs#L811)
     - [NavGrid2D.IsWalkable](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs#L1690)
     - repo 内显式 `OverlapCircleAll` 目前只在 `ChestDropHandler`、`ChestController`、`PlayerAutoNavigator`
  5. 因此 `fresh profiler` 里的 `NPCAutoRoamController.Update -> OverlapCircleAll / GC.Alloc` 与当前 live 文本源码仍未完全钉死对应关系；当前最稳口径只能是：
     - 病灶仍在 NPC 自漫游热链
     - 但 `OverlapCircleAll` 这个具体标签，我还没有钉到当前 live 源码里的唯一调用点
     - 不能把“源码里没这个文本”偷换成“这条热点已经不存在”
  6. [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 当前仍是 mixed scene：
     - intended 改动只有 `TraversalBlockManager2D.autoCollectSceneBlockingColliders = 1`
     - 但 `git diff --numstat` 仍是 `1481 / 324`
     - 所以不能把整张 scene 当成我这一轮的纯净单点成果
- 当前统一结论：
  - 这轮失败不是“方向完全反了”，而是“在真正 hottest loop 还没拆掉前，我又往里面叠了更贵的过滤”
  - 当前已有 `dedupe / backoff / reuse` 不是不存在，而是没有挡住 `TryBeginMove` 里的单次坏 case 成本
  - 如果下一轮允许继续修，第一刀必须先拆热循环，不再往 `TryBeginMove` 叠更多 correctness
- 本轮报实：
  - 只读分析
  - temp 截图路径已失效，无法重新打开原图
  - 当前线程继续维持 `PARKED`

## 2026-04-13 导航检查真实修复：先拆 `TryBeginMove` 热循环，不再往 hottest loop 叠过滤
- 用户在只读审核后明确放行：`落地`
- 这轮真正执行的修复只有一刀：
  1. [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1552)
     - `TryBeginMove()` 从 `TryResolveAutonomousRoamDestination + 邻域 clearance + built-path acceptable` 退回到更便宜的 `TryResolveOccupiableDestination`
     - 新增 `GetAutonomousRoamSampleAttemptBudget()`：正常保持原采样预算；一旦进入 `blocked / stuck / path failure / pending progress` 坏 case，就把本轮采样预算收紧到 `4~6`
     - 目的：先把 hottest loop 的物理查询和路径重试量压下去，不再让坏 case 单次建路就打满整轮高成本过滤
  2. [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs#L148)
     - 去掉 `CanOccupyNavigationPoint()` 里额外追加的 `HasExpandedStaticClearance(...)`
     - 目的：撤掉我上一轮加在 shared occupancy 契约上的额外热查询，避免所有共用这条 contract 的 occupancy 判定一起变重
- 这轮验证：
  - `validate_script`：
    - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) `errors=0`，仅保留那条既有 GC warning
    - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs) `errors=0 warnings=0`
  - `git diff --check`：目标脚本通过
  - Unity fresh console：`0 error / 0 warning`
- 当前阶段判断：
  - 这刀的性质是“先把我自己叠进 hottest loop 的成本拿掉”
  - 属于 `结构/targeted probe 已落地`
  - 还不是 `真实体验已过`
- 下一步恢复点：
  - 如果用户允许下一轮 live 验，应该直接看：
    1. `NPCAutoRoamController.Update` 峰值是否明显回落
    2. NPC 自漫游是否恢复到“性能不炸，但朝向/基础移动仍正常”
    3. 再决定要不要补更便宜的坏目标过滤，而不是立刻把原来的重过滤塞回热循环

## 2026-04-13 live 调试复盘：`Building` 标签不是没处理，fresh profiler 的 101 热点也不是普通自漫游
- 用户这轮追加要求：
  - 先确保控制台调试输出足够关键
  - 再自己跑 live 测，最后只做分析汇报
- 这轮实际做了什么：
  1. 在 [NavGrid2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs) 新增 `BuildBlockingDebugSummary(...)`
     - 目的：当 NPC 顶住静态障碍时，能直接把 `current / next / destination` 三个点各自命中了哪些 collider、tag、layer、explicitObstacle/walkableOverride 原因打出来
  2. 在 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 新增 `LogBlockedMovementDiagnostics(...)`
     - 只在 `showDebugLog` 打开且发生 `ZeroStep / ConstrainedZeroAdvance / MoveCommandOscillation` 时输出
     - 输出包含：`blockedFrames`、`sampleBudget`、`current/waypoint/next/destination` 和 `NavGrid` 的阻挡摘要
  3. 先误入 `Primary` 做了一次 live：
     - 现场只绑定了 `2` 个 NPC
     - 很快确认这不是用户截图里的主要坏现场
  4. 随后回到 `Town` 正确现场，对 `101` 挂 `showDebugLog` 做 live 取证
- 这轮压实的新结论：
  1. `Building` 不是“完全没处理”：
     - [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity#L152395) / [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity#L179176) 的 `obstacleTags` 里明确含 `Building / Buildings`
     - [NavGrid2D.HasAnyTag](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs#L2287) 也会沿父链查 tag
     - `Town` live 启动日志里 [TraversalBlockManager2D](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs) 也明确报了 `阻挡碰撞体=107`
  2. 用户贴的 fresh profiler 里最热的 `101`，在 live 现场并不是“普通 autonomous roam”：
     - `101` 当前 runtime 状态是：
       - `IsResidentScriptedControlActive = true`
       - `IsResidentScriptedMoveActive = true`
       - `ResidentScriptedControlOwnerKey = spring-day1-director`
     - 也就是说，这个热点至少这一拍落在“导演接管下，仍走 NPCAutoRoamController 运动链”的 resident scripted move，不是纯自然漫游
  3. 这能解释为什么我前面只盯“autonomous roam bad sample”一直不够：
     - 用户图里最热实例 `101` 的 owner 分层本来就不是纯 roam
     - 所以性能锅不能再继续简单归成“只是自然漫游采样太重”
  4. 这轮 live 窗口里，没有成功复现 `101` 顶房 / 顶建筑的坏 case：
     - `101` 只走到了 `进入短停`
     - 没触发我新加的 `[NPCBlockedDebug]`
     - 因而这轮还不能用 live 证据宣称“building 已被正确避让”或“问题已定位到唯一 collider”
  5. 但是结论已经比之前更准确：
     - `Building tag` 这层不是主嫌疑
     - 当前最大新增证据是：`profiler 最热对象 101 = resident scripted control 现场`
- 这轮收口报实：
  - 运行已停止
  - 临时 scene 上的 `showDebugLog / logObstacleDetection` 已关回去
  - 线程已 `Park-Slice`

## 2026-04-13 只读深查补记：当前导航主问题已收敛成 `自动配置误配 + 自漫游 acceptance 退化 + resident 朝向 owner 冲突`
- 用户这轮 fresh 反馈：
  - `Farmland_Border` 被自动塞进障碍清单，但农田边界不该阻挡 traversal
  - `203 / 103 / 201 / 101` 等 NPC 仍会撞墙、贴柱、卡住
  - 小动物大面积停滞
  - console 里持续出现 `NPCFacingMismatch`
- 这轮只读核查后新增的 4 个稳定结论：
  1. **自动配置确实收错了农田边界**
     - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs#L23) 默认 `sceneBlockingIncludeKeywords` 仍含 `border`
     - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs#L648) `ShouldAutoCollectBlockingSource()` 走的是名字关键词自动收集
     - `Town` 当前序列化里 `autoCollectSceneBlockingTilemaps=1`、`autoCollectSceneBlockingColliders=1`，且 include keywords 也含 `border`
     - 所以 `Layer 1 - Farmland_Border` 会被 runtime 自动纳入 hard obstacle fallback；这不是用户手配错，是我这条自动收集规则过粗
  2. **静态障碍不是“没处理 Building 标签”，而是 NPC 的坏目标过滤被我退掉了**
     - `Town` 的 NavGrid `obstacleTags` 里明确含 `Building / Buildings / Tree / Rock`
     - 但 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1555) 当前 `TryBeginMove()` 已退回只用 `TryResolveOccupiableDestination(...)`
     - 文件里更稳的 `TryResolveAutonomousRoamDestination(...)` / `IsAutonomousRoamBuiltPathAcceptable(...)` 还在，但当前完全没接进热路径
     - 结果就是：性能峰值止住了，但“坏目标修正 / 邻域净空 / 路径质量筛选”也一起掉了，NPC 会挑到太贴边、太贴柱、太贴围栏的目标
  3. **现在的 occupancy 契约对 NPC 身体过窄，更像脚底探测，不足以保证整个人形安全通过**
     - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs#L518) 的 query radius 取的是 `foot probe inset + extra radius` 这组较小值
     - 这意味着动态约束主要在保“脚底中心 + 左右脚 probe”不踩进障碍，不等于保证整个人体盒体不贴进墙柱
     - 所以用户截图里会出现“路径线看着合法，但 sprite 身体已经贴进柱子/墙体”的坏 case
  4. **`201 / 103` 这类摆头并不是 sector 算法错，而是 day1 director 仍在抢朝向 owner**
     - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs#L181) 已经会尽量按实际移动向量纠偏
     - 但用户贴的 log 已直接显示：`externalVelocitySource=NPCAutoRoamController.ApplyMoveVelocity`，同时 `facingSource=SpringDay1NpcCrowdDirector.ResetStateToBasePose`
     - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L1408) 的 `ResetStateToBasePose()` 仍会 `StopMotion() + SetFacingDirection(BaseFacing)`
     - 因而对 `101/103/201/203` 这批 resident，当前问题不是单纯导航 core，而是“移动 owner”和“朝向 owner”同场打架
- 当前最准确的分层：
  - `001~003` 比较正常：说明核心运动链并非全盘坏死，但开放空间 case 不能证明系统已过线
  - `101/103/201/203`：同时命中 resident owner 冲突 + 贴边坏目标
  - `小动物`：主要暴露的是封闭区域 bad sample / 路径 acceptance 退化 / blocked 后只会停，不会稳定重新采样到好目标
- 当前恢复点：
  - 下一轮不能再继续泛修“Building 标签”
  - 最值钱修复顺序应是：
    1. 收自动配置误配，先把 `Farmland_Border` 从 hard obstacle 自动收集中排除
    2. 把 NPC 自漫游的 acceptance 层按低成本方式接回 `TryBeginMove()`
    3. 再把 resident 的 facing owner 收成“谁驱动移动，谁负责朝向”
    4. 最后才补玩家那种 blocked-navigation 式的末端阻挡缓冲

## 2026-04-13 真实施工补记：先收 `Farmland_Border` 自动误配这一刀
- 用户当轮追加纠偏：
  - 这轮不是全量只读；`Farmland_Border` 被自动纳入 obstacle 这一项必须先修掉，再一起汇报
- 本轮真实修改：
  1. [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs)
     - `sceneBlockingExcludeKeywords` 默认新增 `farmland`
     - `ShouldAutoCollectBlockingSource(...)` 新增 `IsAutoWalkableFarmlandSurfaceSource(...)` 早退
     - 新增 `NameLooksLikeWalkableFarmlandSurface(...)`
  2. 当前代码口径变成：
     - 自动收集仍会继续收 `wall / props / fence / rock / tree`
     - `Farmland_Water` 仍可因 `water` 关键字进入 soft-pass
     - 但 `Farmland_Border / Farmland_Center` 这类农田可走面不再被自动塞进 hard obstacle
- 这刀验证：
  - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs) `validate_script => 0 error / 0 warning`
  - Unity fresh console 当前读取到的仍是既有 external 问题：
    - 多条 `Missing Script`
    - TMP ellipsis 字体 warning
  - 当前没有证据显示这刀引入了新的编译红
- 当前状态：
  - `Farmland_Border 自动误收` 这一个问题已经落代码修掉
  - 但 `NPC 撞墙 / resident 摆头 / 小动物停滞` 仍未修
  - fresh console 还新增证明：朝向问题不只 resident owner 一层；`001`、小鸡、牛在 `facingSource=none` 下也会报 `NPCFacingMismatch`，说明 [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs#L583) 的 `GetSmoothedFacingDirection()` 保向/反向确认逻辑本身也会拖错朝向

## 2026-04-13 补刀收口：NPC 功能主链已重新回到“取点正确 + 朝向归 owner + fresh compile clean”
- 用户这轮正式放行，要求：
  - 功能到位优先
  - 但性能不能再回炸
- 本轮完成的 4 个实际动作：
  1. [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
     - 重新把 `TryResolveAutonomousRoamDestination(...)` 与 `IsAutonomousRoamBuiltPathAcceptable(...)` 接回 `TryBeginMove()`
     - 但只对普通自漫游开启，不影响 scripted/debug move 的轻量路径
  2. [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
     - 朝向求解现在优先跟真实位移/最近刚体速度
     - external facing 退到无真实位移时才兜底
  3. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - 新增 `ApplyFacingIfIdle(...)`
     - resident 还在 move 时，director 不再继续写 baseFacing 抢朝向 owner
  4. [SpringDay1LateDayRuntimeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs)
     - 顺手补平测试编译 blocker，只为恢复 fresh compile 验证
- 本轮 fresh 证据：
  - 上述 4 个文件 `validate_script` 都是 `errors=0`
  - `git diff --check` 通过
  - Unity refresh 后 `read_console(error|warning)` 返回 `0`
  - 当前打开的 `Primary` 短跑约 `6s`，没有再刷 `NPCFacingMismatch` 或导航红错
  - runtime 唯一看到的外部噪音是一条 TMP `LiberationSans SDF (Runtime)` warning
- 本轮最核心的判断：
  - 导航这条线当前已经不再是“功能只能靠砍性能换出来”，而是重新回到：
    - 自漫游坏点过滤回来了
    - resident 朝向 owner 分清了
    - compile/console fresh clean
  - 仍未闭环的只剩用户终验层：
    - `Town` 常驻居民与小动物的大场景体验
    - 这轮我没强行切场景，只在用户当前打开的现场安全口径下拿了 `Primary` live 证据

## 2026-04-13 导航检查 live 复测：先收 scripted-move 重建预算，再把 Town 卡顿证据重新分层
- 用户最新要求：
  - 不要只看静态代码
  - 继续往下做，并且必须自己跑 live / 全面测试
- 本轮实际动作：
  1. 继续使用 `skills-governor + sunset-workspace-router + sunset-unity-validation-loop + sunset-no-red-handoff`
  2. 读取 `editor/state`、`Town` 活动 scene、console、以及 `SpringDay1LatePhaseValidationMenu` / `NpcRoamSpikeStopgapProbeMenu` / `SpringDay1ActorRuntimeProbeMenu`
  3. 在 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 落一刀最小止血：
     - `DebugMoveTo()` 也接入 `TryAcquirePathBuildBudget()` 与 `RecordPathBuildOutcome(...)`
     - 新增 `SCRIPTED_MOVE_DECISION_REUSE_SECONDS = 0.08f`
     - scripted/debug move 稳定移动时允许短窗口跨帧复用已算好的 move decision，减少每个物理步都重跑整条重决策链
- 这轮先后拿到的 live 证据：
  1. **第一次 probe 不能再当干净性能基线**
     - 当时我在 PlayMode 里连续读取多个 GameObject `/components` 资源
     - 这触发了 MCP 自己的 `GameObjectSerializer` 撞 Animator/Playback 属性报错
     - 随后跑出的 `NpcRoamSpikeStopgapProbe` 显示：
       - `avgFrameMs = 95.33`
       - `maxFrameMs = 101.27`
       - `maxGcAllocBytes = 68433`
     - 现在确认：这次 `95ms` 读数混入了我自己的 MCP 深读干扰，不能再直接当“游戏本体现场就是 95ms”
  2. **打完补丁后，在不做组件深读的 clean run 里，Town 短跑已不复现卡爆**
     - fresh `Town` Play + `NpcRoamSpikeStopgapProbe`
     - 结果：
       - `npcCount = 25`
       - `roamNpcCount = 18`
       - `avgFrameMs = 0.90`
       - `maxFrameMs = 19.51`
       - `maxGcAllocBytes = 67014`
       - `maxConsecutivePathBuildFailures = 0`
       - `topSkipReasons = AdvanceConfirmed`
     - 当前 clean 现场里没有再出现 `[NPCFacingMismatch]` 风暴，也没有导航红错刷屏
  3. **当前仍不能把 Town 居民体验判成“已过线”**
     - fresh `Force Spring Day1 FreeTime Validation Jump` 后，`SpringDay1ActorRuntimeProbe` 反复显示：
       - `phase = FreeTime`
       - 但 `101~203` 仍是 `staging:*`
       - `roamControllerEnabled = false`
       - `scriptedControlOwnerKey = spring-day1-director`
     - 这说明：当前这条 validation menu 路径下，我还没把用户抱怨的“常驻居民真正自由动起来”的坏 case 干净复现出来
- 当前最稳的结论：
  1. 这轮我确实修掉了一处真实逻辑漏洞：
     - scripted/debug move 之前没走已有的 path-build budget/backoff，失败时会比 autonomous roam 更容易重建失控
     - 同时 `consecutivePathBuildFailures` 之前也会在 scripted success 后残留脏值
  2. 当前 `Town` clean live 下，**我已经拿不到“持续卡爆”证据**
     - 所以不能继续把“卡爆”当成已稳定复现的当前事实
  3. 但当前也**还没有**拿到“居民真实自由坏 case 已完全验过”的证据
     - 现在最多只能说：`结构/targeted probe 站住，真实体验仍待更贴近用户路径的 live 复现`
- 当前恢复点：
  - 下一轮如果继续，应优先做：
    1. 用更接近用户真实流程的入口把 `101~203` 放到真正的 resident free-roam / return-home 坏现场
    2. 只在那个现场再看是否还会撞墙、乱转、或重新出现性能峰值
    3. 不再用会触发 MCP serializer 噪音的 `/components` 深读去污染性能样本

## 2026-04-13 导航检查只读再收敛：Town NPC 当前不是“再加节流就好”，而是 `静态栅格真值 / 运行态占位真值 / resident owner` 三层没收平
- 用户这轮要求：
  - 不要再给泛方案
  - 直接回答：能不能只靠 `MCP/no-red` 收尾、到底是什么问题、我到底知不知道下一刀怎么改
- 这轮保持严格只读：
  - 复读并对齐了 `62/63/64/65` 这几份导航 prompt
  - 回看了 [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs)、[NavigationPathExecutor2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs)、[NavGrid2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs)、[NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs)、[NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)、[SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)、[SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs)
  - 复核了 [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 里 `TraversalBlockManager2D` 与 `NavGrid2D` 的当前序列化配置
- 这轮新压实的 4 个判断：
  1. **不能只靠 MCP/no-red 宣称“完全符合需求”**
     - `no-red` 只能证明编译/console 干净
     - 不能证明 NPC 在真实场景里不会撞墙、不会贴树、不会被 day1 owner 持续接管
     - 当前最直接反例就是 [spring-day1-actor-runtime-probe.json](D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/spring-day1-actor-runtime-probe.json)：结构上 clean，但 `101~203` 仍处于 `scriptedControlOwnerKey=spring-day1-director`
  2. **NPC 现在最核心的问题不是“没有节流”，而是 path 真值和 occupancy 真值分裂**
     - [NavigationPathExecutor2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs) 建路最终调用 [NavGrid2D.TryFindPath](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs)，走的是 baked `walkable[,]`
     - 但 [NavigationTraversalCore.CanOccupyNavigationPoint](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs) 每步移动约束走的是 runtime `NavGrid.IsWalkable(worldPos, queryRadius, ignoredCollider)`
     - 一旦静态房屋/树木/围栏 collider 没进 baked obstacle sources，路径会规划进去；运行时脚底 probe 又只会临门一脚停住，于是就变成顶墙、贴柱、原地磨
  3. **当前 Town 的 auto-collect 仍然过度依赖名字关键词，和用户“我配的标签/场景就是权威”这个要求不匹配**
     - [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 当前 `autoCollectSceneBlockingColliders=1`
     - 但 `sceneBlockingIncludeKeywords` 仍只有 `wall/props/fence/rock/tree/border`
     - 所以像 `House` / `Building` 这种静态结构，即使 tag 正确，也未必会进入 `ConfigureExplicitObstacleSources`
     - 这不是“Building 完全没处理”；更准确是：`Building` tag 已在 runtime blocker 判定层生效，但静态栅格层未必吃全
  4. **`101~203` 这批 Town resident 不是普通 roam，仍然有 day1 owner 层**
     - [SpringDay1NpcCrowdDirector.ApplyStagingCue](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 会持续把 cue 应到 resident
     - [SpringDay1DirectorStagingPlayback.ApplyCue](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs) 在 `suspendRoam=true` 时会重新 Acquire takeover
     - 所以这批 NPC 的最终体验不能被我假装成“只改导航 core 就能单线闭环”
- 当前我对下一刀的收敛结论：
  1. 先改 [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs)
     - 把静态阻挡采集从“名字关键词优先”改成“用户已配的 tag/layer/soft-pass/walkable override 为权威，再辅以名字关键词”
     - 目标是让房屋/建筑/树木正确进入 baked obstacle sources，同时保住 `farmland 可走 / water soft-pass`
  2. 再改 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
     - 自漫游继续保留现有 `budget/backoff/reuse`
     - 但只在 `TryBeginMove -> TryResolveAutonomousRoamDestination / IsAutonomousRoamBuiltPathAcceptable` 这条低频建路链上补“更宽一点的 body-clearance acceptance”
     - 不把重物理查询塞回每个 `FixedUpdate`
  3. [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs) 只做 helper 级补口
     - 不把 `CanOccupyNavigationPoint()` 重新改成热循环重查询
     - 只提供给上面的建路 acceptance 调用
  4. `101~203` 的 resident owner 仍需和 day1 分责
     - 这层不是我能靠 no-red 假装闭环的导航 core 问题
- 当前阶段判断：
  - `如何改` 已经压到足够具体，可以直接进下一刀真实施工
  - 但 `只靠 no-red/MCP 就宣称完全符合需求` 仍然不成立
  - 因为用户要的是真实运行体验，不是“编译通过”

## 2026-04-13 导航检查真实施工：先把静态阻挡真值和自漫游低频 body-clearance acceptance 接回去
- 用户本轮直接放行：`直接落地`
- 这轮真实改动只收 3 个文件：
  1. [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs)
     - `sceneBlockingIncludeKeywords` 默认补入 `building / house / structure`
     - `ShouldAutoCollectBlockingSource()` 不再只看名字关键词
     - 新增按 `NavGrid2D` 当前 obstacle tags / obstacle mask 的运行时语义补收静态 source
     - 仍保留：
       - `farmland center/border` 自动排除
       - `water` soft-pass
       - dynamic / trigger / 非 StaticObstacle navigation unit 排除
  2. [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs)
     - 新增 `CanOccupyNavigationPointWithClearanceMargin(...)`
     - 作用：只在低频建路 acceptance 时，用“扩一点的 traversal contract”判断身体净空
     - 没有改 `CanOccupyNavigationPoint()` 的 hot loop 语义
  3. [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
     - `TryResolveAutonomousRoamDestination(...)` 现在额外要求 destination body-clearance
     - `IsAutonomousRoamBuiltPathAcceptable(...)` 现在额外检查整条已建路径的低频 body-clearance
     - 采样点间距固定为 `0.32f`，只在 autonomous roam build 阶段触发，不进入每帧移动热循环
- 本轮代码层验证：
  - `validate_script Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs`
    - `assessment=external_red`
    - `owned_errors=0`
    - native `manage_script.validate=clean`
  - `validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - `assessment=external_red`
    - `owned_errors=0`
    - native `manage_script.validate=warning`
    - 仅保留既有 warning：`String concatenation in Update() can cause garbage collection issues`
  - `validate_script Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs`
    - `assessment=external_red`
    - `owned_errors=0`
    - native `manage_script.validate=clean`
  - `git diff --check`：这 3 个目标脚本通过
- 当前 external blocker 报实：
  - Unity 实例仍停在 `Primary.unity` 的 `playmode_transition / stale_status`
  - fresh console 里仍有既有外部红：
    - 多条 `The referenced script (Unknown) on this Behaviour is missing!`
    - 1 条 MCP websocket warning
  - 这些都不是这轮导航修改新引入
- 当前阶段判断：
  - 这轮已经把“路径规划进静态障碍”和“自漫游路径太贴边”这两层先接回来了
  - 但 `101~203` 的 resident owner 仍是 day1 caller 层，当前不能谎报“Town 全居民已完全恢复自然 roam”
- 下一次恢复点：
  1. 优先看 Town 里普通 roam NPC / 小动物是否不再贴房、贴树、贴围栏
  2. 如果 resident 101~203 还异常，优先沿 day1 owner 链继续分责，不再回泛导航

## 2026-04-14 导航检查 Town live 验收：当前没抓到性能/阻挡复现，但 day1 resident owner 漏口仍然实锤存在
- 用户本轮要求：直接做 `Town live 验收`，把还存在的问题测出来并汇报
- 本轮按受控 live 顺序执行：
  1. `Begin-Slice` -> `Town` 当前场景进入 Play
  2. 先跑一次原始入口短探针：
     - `Tools/Sunset/NPC/Run Roam Spike Stopgap Probe`
     - `Sunset/Story/Validation/Write Spring Day1 Actor Runtime Probe Artifact`
  3. 再做一次 `FreeTime` 跳转：
     - `Sunset/Story/Validation/Force Spring Day1 FreeTime Validation Jump`
     - 再写一次 actor runtime probe
     - 再跑一次 roam spike probe
  4. 证据拿到后立即 `STOP`，退回 EditMode
- 本轮 live 结果：
  1. **普通 Town 入口这 6 秒里，没有复现性能炸或 blocked storm**
     - [npc-roam-spike-stopgap-probe.json](D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/npc-roam-spike-stopgap-probe.json) 第一轮：
       - `npcCount=25`
       - `roamNpcCount=15`
       - `avgFrameMs=0.67`
       - `maxFrameMs=9.53`
       - `maxBlockedNpcCount=0`
       - `maxBlockedAdvanceFrames=0`
       - `maxConsecutivePathBuildFailures=0`
     - 说明当前至少在这条短窗口里，导航本体没有再打出之前那种 spike / blocked 风暴
  2. **FreeTime 跳转后的第二轮 6 秒，也没有复现 blocked storm**
     - 第二轮 probe：
       - `npcCount=25`
       - `roamNpcCount=17`
       - `avgFrameMs=0.62`
       - `maxFrameMs=12.32`
       - `maxBlockedNpcCount=0`
       - `maxBlockedAdvanceFrames=0`
       - `maxConsecutivePathBuildFailures=0`
     - 说明这轮我刚修的“静态阻挡真值 + 自漫游 acceptance”至少没有把性能打回去
  3. **真正仍然实锤存在的问题是 day1 resident owner 没放干净**
     - [spring-day1-actor-runtime-probe.json](D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/spring-day1-actor-runtime-probe.json) 当前 phase 已是：
       - `phase=FreeTime`
       - `beatKey=FreeTime_NightWitness`
     - 但 resident probe 里 `101~203` 仍然是：
       - `appliedBeatKey=EnterVillage_PostEntry`
       - `appliedCueKey=101~203`
       - `scriptedControlOwnerKey=spring-day1-director`
       - `roamControllerEnabled=false`
       - `roamState=Inactive`
     - 同时 `003` 也还没被放回自然 roam：
       - `scriptedControlOwnerKey=spring-day1-director`
       - `roamState=Inactive`
     - 而 `001/002` 已经恢复成：
       - `roamControllerEnabled=true`
       - `isRoaming=true`
       - `roamState=ShortPause`
     - 所以当前 Town live 最硬的剩余问题，不再是“导航 core 统一全坏”，而是：
       - `day1 owner release 不完整`
       - 至少 `003 + 101~203` 这批人还被老 cue/老 owner 挂住
  4. **本轮 live 收尾时 console 是干净的**
     - `sunset_mcp.py errors --count 40 --output-limit 15`
       - `errors=0 warnings=0`
     - 说明本轮 controlled live 没再刷新的导航红错
- 当前恢复点：
  1. 导航 core 当前在受控 Town live 里没有再出现 spike / blocked storm
  2. 当前最该继续追的是 `SpringDay1NpcCrowdDirector / SpringDay1DirectorStaging` 的 owner release 链
  3. 如果用户后面仍看到“贴房/贴树/贴围栏”，那更像是随机 roam 路径没在这 2 个 6 秒窗口里踩中，需要做更定向的现场复现，而不是现在就谎报“已完全没有坏相”

## 2026-04-14 导航检查只读深挖：当前病态导航至少已拆成 owner / roam / animal / static-steering 四层
- 用户本轮明确要求：
  - 只做更深入、更全面的只读彻查
  - 不允许跨 owner 直接修
  - 不只盯 `FreeTime roam`，还要覆盖更多非剧情态导航病态
- 本轮实际追加钉死的结论：
  1. `101~203 + 003` 在 `Town/FreeTime` 仍被 `spring-day1-director` 持有，不是导航 core 先坏
     - 证据：`Library/CodexEditorCommands/spring-day1-actor-runtime-probe.json`
     - 当前 `phase=FreeTime`，但 `appliedBeatKey` 仍停在 `EnterVillage_PostEntry`
     - `scriptedControlOwnerKey=spring-day1-director`
     - `roamControllerEnabled=false`
     - `roamState=Inactive`
  2. 普通非剧情态 NPC / 小动物 的坏相，核心不在“剧情移动链”，而在“自漫游随机目标 + NPC 运行时执行层”
     - `DriveResidentScriptedMoveTo / DebugMoveTo` 这类显式目标链会绕开随机 roam sampling
     - `TryBeginMove()` 的 autonomous roam 先随机采样，再做 acceptance
     - 所以“剧情里正常，自漫游犯病”符合当前代码结构
  3. NPC 和玩家虽然共享 `TraversalCore/NavGrid`，但静态世界处理并不等价
     - 玩家还有 caller 层的 `AdjustDirectionByColliders / TryMeasureBlockingClearance / SetBlockedNavigationInput`
     - NPC 在 `TickMoving()` 里主要只有：
       - `TryHandleSharedAvoidance()`（偏 agent-agent）
       - `ConstrainNextPositionToNavigationBounds()`（偏脚底约束）
       - `blocked/stuck` 止血
     - 所以 NPC 更容易出现“路径方向大体对，但身体贴房/贴树/顶墙”
  4. 共享 local avoidance 当前主要不是静态世界避让器
     - `NavigationLocalAvoidanceSolver` 只吃 `NavigationAgentSnapshot`
     - `NavigationAvoidanceRules.ShouldConsiderForLocalAvoidance()` 直接排除了 `StaticObstacle`
     - `INavigationUnit.ShouldAvoid(...)` 虽然存在，但当前 live solver 并不按它驱动
  5. `Town` 的 scene 序列化 traversal 配置落后于代码默认值
     - `TraversalBlockManager2D.cs` 默认 include 已含 `building/house/structure`
     - 但 `Town.unity` 当前序列化的 `sceneBlockingIncludeKeywords` 仍只有：
       - `wall / props / fence / rock / tree / border`
     - 当前 building 是否被补收，更依赖 `NavGrid obstacle tags/layers` 与祖先层级深度命中
  6. 农田当前代码口径仍是“中心/边框可走，水不可走”
     - `TraversalBlockManager2D.IsAutoWalkableFarmlandSurfaceSource()` 会跳过 `Farmland_*Center/Border`
     - `Farmland_Water` 不会被这个豁免放过
  7. 这轮没有抓到 fresh 性能 storm 复现
     - `npc-roam-spike-stopgap-probe.json` 当前短窗：
       - `avgFrameMs=0.6185`
       - `maxFrameMs=12.317`
       - `maxBlockedNpcCount=0`
       - `maxConsecutivePathBuildFailures=0`
     - 当前 freshest 证据更像“功能契约仍失真”，不是“此刻又稳定必炸”
- 当前分层后的问题地图：
  1. `day1 owner/release`：resident 释放漏口，归 `SpringDay1NpcCrowdDirector / SpringDay1Director / Staging`
  2. `普通自漫游 NPC`：目标 acceptance 与运行时静态执行契约不一致
  3. `小动物/围栏域`：沿用同一套 roam contract，但 closed domain 更容易踩到坏目标/坏停滞
  4. `玩家 vs NPC`：静态碰撞体处理能力不对等，玩家有 caller 级静态 steering，NPC 没有
- 本轮没有动代码、没有改场景、没有再开施工切片
- 当前线程状态保持：
  - `PARKED`
  - 不进入 `Begin-Slice`
- 下一轮如果继续，最值钱的不是“泛修导航”，而是：
  1. 先分 owner：day1 caller 链 vs 导航 core
  2. 再用 1 个普通 Town NPC + 1 个动物围栏 case 钉第一失真点
  3. 只有拿到确切失真点后，才决定修 scene truth、NPC static steering，还是 day1 owner release

## 2026-04-14｜补记：day1 线程已发来 owner 边界 prompt - 67
- 新增协作 prompt：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-14_导航线程_Day1边界重划后非剧情态roam静态契约收口prompt_67.md`
- 这份 prompt 冻结了三件事：
  1. opening marker / 对白前就位 / resident release / `20:00/21:00/26:00` 不是导航 own
  2. 导航只 own Day1 已放人后的 NPC / 动物非剧情态 roam 静态执行契约
  3. 如果 live 样本仍处在 Day1 scripted 持有期，正确动作是 exact blocker 报实，而不是越权代修
## 2026-04-14 11:34 prompt_67 续记
- 本轮严格按 [2026-04-14_导航线程_Day1边界重划后非剧情态roam静态契约收口prompt_67.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-14_导航线程_Day1边界重划后非剧情态roam静态契约收口prompt_67.md) 继续，只读核对：
  - valid roam 样本是谁
  - 当前静态真值缺口是否还混着 `day1 owner`
- 新稳定结论：
  1. `001/002/003` 才是这轮导航 own 的 valid roam 样本
  2. `101~203` 当前仍被 `spring-day1-director` 持有，不得回灌成导航 own 现场
  3. 动物 prefab 与普通 NPC 当前同根走 `NPCAutoRoamController + NPCMotionController`
  4. `Town.unity` 当前仍存在一个明确配置事实：
     - `walkableOverrideTilemaps / walkableOverrideColliders = []`
     - 所以 Town 的桥/水 walkable contract 不是显式挂好的状态
- 本轮没有继续改代码，已执行：
  - `Park-Slice`
  - 当前线程状态=`PARKED`
- 下一轮恢复点：
  - 继续只准沿 `prompt_67` 白名单收 `Day1 已放人后的 NPC/动物非剧情态 roam 静态执行契约`

## 2026-04-14 12:15 prompt_67 续记：静态避让/早退脱困刀已落，但 fresh 样本仍要区分“性能证据”和“valid roam 终证”
- 本轮已从只读进入真实施工，并且只动了：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
- 当前这刀做成的事：
  1. autonomous roam 的静态 clearance 半径与采样密度提高，避让更宽一点
  2. 纯静态 bad case 更早 retarget / abort，不再长时间原地磨
  3. 仍避开 resident scripted control，不把 Day1 own 的剧情移动链误伤成 roam stopgap
- fresh 验证结果：
  - `validate_script NPCAutoRoamController.cs` => `assessment=no_red`
  - fresh console => `0 error / 0 warning`
  - fresh Town `NpcRoamSpikeStopgapProbe`：
    - `avgFrameMs=0.6868`
    - `maxFrameMs=14.6529`
    - `maxGcAllocBytes=235072`
    - `maxBlockedNpcCount=0`
    - `maxConsecutivePathBuildFailures=0`
- 但 fresh actor probe 同时证明：
  - 当前这一次 live 入口处在 `EnterVillage_PostEntry`
  - 不是完整的 `Day1 已放人 resident roam` 窗口
  - 所以这轮只能说：
    - `性能没回炸`
    - `结构补口已落`
    - 不能说 `prompt_67 体验终证已过`
- 当前收口判断：
  - `Ready-To-Sync` 已跑，但被 own roots 里的历史残留 dirty/untracked 阻断
  - 已合法 `Park-Slice`
  - 当前状态=`PARKED`
- 下一轮恢复点：
  1. 回到真正 valid roam 时窗继续验 `001~003 + 动物`
  2. 如果还贴房/顶树/围栏边磨，继续优先补 `NPCAutoRoamController`
  3. 不准把这轮性能 probe 偷换成 Day1 resident roam 体验终证

## 2026-04-14 13:40 prompt_67 续记：fresh FreeTime live 已证明 `003` 可释放，但白天自由漫游仍存在“贴静态障碍选坏点”与“斜走摆头”
- 本轮先做了可回退存档：
  - [README.md](D:/Unity/Unity_learning/Sunset/.codex/backups/navigation-check/2026-04-14_12-43-48_prompt67_before_resident_roam_deadlock_fix/README.md)
- 当前 slice：
  - `prompt67-resident-roam-deadlock-fix-and-snapshot`
- 已落代码：
  1. [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
     - autonomous roam 目标 agent 去重
     - shared avoidance 动态互锁脱困出口
  2. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - `003` 在 opening 村口之后可释放回 roam
- fresh live 证据：
  - 强制推进到 `FreeTime`
  - `SpringDay1ActorRuntimeProbe` 显示：
    - `003` 已经不再是“始终不 roam”的死锁态
    - `101/103/201/202/203` 在有效样本里能进入 `isRoaming=true`
  - `NpcRoamSpikeStopgapProbe` 显示：
    - `avgFrameMs=12.95`
    - `maxFrameMs=62.88`
    - `maxBlockedNpcCount=1`
    - `maxBlockedAdvanceFrames=2`
- 但用户 live 追加反馈同时钉死了新的第一真问题：
  1. `101` 会贴房卡住
  2. `103` 会贴石头卡住
  3. 到 `20:00` 会被 Day1 夜间回家链收走，所以“最终回家成功”不能证明白天 free-roam 合同没问题
  4. 夜间/回家等斜向脚本移动仍会摆头
- 当前导航 own 判断更新为：
  1. `003 不 roam` 这条已不再是当前唯一主锅，至少 fresh live 能证明 release 可成立
  2. 白天 free-roam 期间真正仍未收平的是：
     - autonomous roam 选点 / 路径离静态障碍太近
     - 结果是 NPC 会贴房、贴石头、围着静态碰撞体磨步
  3. 这类坏相不会总是把 `blockedAdvanceFrames` 顶很高，更像是“短步被约束后进入 ShortPause，再继续采到坏点”
  4. 斜向摆头不是纯 Day1 问题，`NPCMotionController` 当前四向扇区在对角边界附近仍缺稳定滞回
- 与 Day1 的边界判断：
  1. Day1 对 `Dinner / Return` 错吃 `opening cue`、`release 后从 current context 开始 roam` 的分析大方向成立
  2. 但用户眼前 fresh 白天 free-roam 的 `101/103 贴静态障碍`，仍是导航 own，不能回甩给 Day1
  3. Day1 own 主要影响：
     - 晚饭 / 回家段 contract
     - scripted move 是否还在走错误 cue / 错误 release
  4. 导航 own 主要影响：
     - free-roam 静态 clearance 太窄
     - diagonal facing 在四向边界抖动
- 下一轮恢复点：
  1. 继续只收 `autonomous roam` 静态安全边与 bad-point 重采样
  2. 再单独收 `NPCMotionController` 的四向扇区滞回，解决斜走摆头
  3. 与 Day1 同步：scripted move 若继续长期使用，也必须吃到同一套静态 steering / facing contract

## 2026-04-14 14:48 prompt_67 续记：静态安全边与四向扇区滞回已落代码，fresh live 进一步确认 `101/103` 当前样本仍混有 crowd scripted move
- 本轮真实改动只动了：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
  - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
- 导航 own 已落地的两刀：
  1. `NPCAutoRoamController`
     - 提高 autonomous roam 的静态 body clearance 半径与路径采样密度
     - `RefreshHomePositionFromCurrentContext()` 不再直接把坏 anchor 点拿来当 roam center，而是优先在 anchor 周围解析一个“可站、离静态障碍更安全”的 roam center
  2. `NPCMotionController`
     - 新增四向扇区边界滞回 `facingBoundaryHysteresisDegrees`
     - 斜向速度贴近 45/135 度边界时，优先保持上一帧稳定朝向，减少左右/上下来回翻
- fresh 代码层验证：
  - `validate_script NPCAutoRoamController.cs` => `assessment=no_red`
  - `validate_script NPCMotionController.cs` => `owned_errors=0 external_errors=0`
    - 但连续两次都卡在 `unity_validation_pending(stale_status)`，不是脚本编译报错
  - `errors --count 20 --output-limit 5` => `0 error / 0 warning`
- fresh 短 live：
  - 路径：`Play -> Force FreeTime -> Actor Probe -> 6s Roam Probe -> Stop`
  - 结果：
    - `101/103/003` 均可见运动
    - 但 `101/103` 当前样本里仍是 `scriptedControlActive=true`
    - owner=`SpringDay1NpcCrowdDirector`
    - `003` 当前样本里也仍有 `spring-day1-director` scripted move
  - 因此这轮 live 只能说明：
    - 代码没炸回红面
    - 性能没回到 storm
    - 但还**不能**把这次样本当成“纯 free-roam fully released”终证
- 当前对 Day1 补充分析的吸收结论：
  1. Day1 对 `Dinner/Return` 错 alias 到 `opening`、`release 后 current-context roam` 的判断，大方向成立
  2. 但 `101/103` 贴静态障碍这条，在用户白天样本里依然是导航 own
  3. 当前应对 Day1 的同步口径是：
     - 它继续收 authored contract / release contract / scripted move owner 顺序
     - 我这边继续收 static clearance / diagonal facing / scripted move 共用 steering contract
- 当前线程状态：
  - `Park-Slice` 已跑
  - 状态=`PARKED`
- 下一轮恢复点：
  1. 用户 fresh 体感复测 `101/103` 是否还贴房贴石头、斜走是否还摆头
  2. 若 free-roam 坏相仍在，再继续加一层“局部静态脱困出口”
  3. 若 scripted move 仍是主样本，则和 Day1 对齐，把 scripted move 也接上同一套静态 steering contract

## 2026-04-14 14:55 只读核查补记：当前没有发现 Day1 直接回退导航线程刚落的两刀
- 本轮为只读核查，未进入施工，未跑 `Begin-Slice`
- 核查对象：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
  - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
- 结果：
  1. [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 仍保留：
     - `AUTONOMOUS_ROAM_SAFE_CENTER_*`
     - `TryResolveSafeRoamCenterNear(...)`
     - `TryBreakAutonomousSharedAvoidanceDeadlock(...)`
     - `AUTONOMOUS_ROAM_DESTINATION_AGENT_CLEARANCE_*`
  2. [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs) 仍保留：
     - `facingBoundaryHysteresisDegrees`
     - `ApplyFacingSectorHysteresis(...)`
  3. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 里的：
     - `ShouldUseTownThirdResidentStoryActorMode(...)`
     仍存在
- git 现场：
  - `git status` 里这轮导航 own 的脏改仍只在：
    - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
    - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
  - 说明我这两刀还在 working tree，没有被直接抹掉
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 当前是 clean，但相关 `003` release 代码仍在文件里，说明不是“内容被删了”，更像已经被吸收进当前 HEAD/检查点
- 结合 latest live 的判断更新：
  - 如果用户刚看到坏相，更像是 Day1 owner/runtime 合同又把 `101/103/003` 持有住了
  - 不是 Day1 把我刚落的 `static clearance + facing hysteresis` 代码直接回退掉

## 2026-04-14 17:30 prompt_67 续记：已证实 Town resident 在 `HealingAndHP` 阶段就开始 roam；并修掉了我 own 的“白天 resident 被错误锚到远处 home”问题
- 本轮重新进入施工 slice：
  - `prompt67-opening-vs-roam-proof-and-own-followup`
- 先做 live 取证，回答用户问题“是不是只有 opening 结束后才 roam”：
  - fresh `SpringDay1ActorRuntimeProbe`
  - fresh `SpringDay1LiveSnapshotArtifact`
- 结论：
  1. 在当前 live 样本里，Town resident 不是非要等 opening 全结束后才 roam
  2. 在 `HealingAndHP` 阶段：
     - `101~203` 已经 `isRoaming=true`
     - `scriptedControlActive=false`
     - 说明它们已经进入日常 roaming 合同
  3. `003` 在该样本里仍不是稳定 roam，这条不能被误判成“全部 NPC 都已正常 roam”
- 这轮抓到的一个我 own 的真问题：
  - 我前一刀加的 safe-center 逻辑写偏了
  - `RefreshHomePositionFromCurrentContext()` 会过度优先远处 `homeAnchor` 附近的安全点
  - 结果是白天 resident 明明应该围着当前 daytime/resident 位置 roam，却被错误地把 roam center 往夜间 home 附近拉
  - fresh live 直接表现为：
    - `101~203` 大面积 `ShortPause`
    - `consecutivePathBuildFailures=16`
- 本轮已真实修复：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
    - `RefreshHomePositionFromCurrentContext()` 现在先判断当前是否应继续使用当前位置作为白天 roam center
    - 只有在不该继续使用 current context 时，才优先围绕 `homeAnchor` 找安全 roam center
- fresh live 复测结果：
  - `101/102/103/203`
    - 已从 `ShortPause + pathBuildFailures=16` 恢复到 `Moving + pathBuildFailures=0`
  - `104/202`
    - 处于正常 `ShortPause / LongPause`
    - `pathBuildFailures=0`
  - `201`
    - 仍残留一个单点坏相：
      - `isRoaming=true`
      - `ShortPause`
      - `consecutivePathBuildFailures=16`
    - 说明“白天 resident 全体锚错中心”这条已经修掉，但还剩一个更窄的单点 case
- 代码层验证：
  - `validate_script NPCAutoRoamController.cs`
    - `owned_errors=0 / external_errors=0`
    - 但 CLI 仍卡在 `unity_validation_pending(stale_status)`
  - `validate_script NPCMotionController.cs`
    - `owned_errors=0 / external_errors=0`
    - 同样卡在 `unity_validation_pending(stale_status)`
  - `errors` => `0 error / 0 warning`
- 当前阶段判断：
  1. “是不是只有 opening 结束后才 roam”这个问题，已经有 fresh live 证据说明：不是
  2. 我这轮 own 修复已经明确消掉了一层大面积白天 resident stall
  3. 但还没到“导航 fully closed”：
     - 还残留 `201` 单点坏 case
     - `003` 在当前样本里也还不是稳定 roam
- 当前状态：
  - `Park-Slice` 已跑
  - thread-state=`PARKED`
- 下一轮恢复点：
  1. 优先继续钉 `201` 的单点坏 case
  2. 单独区分 `003` 是 Day1 own 还是导航 own
  3. 继续保留“白天 resident roam center 不要被夜间 home 拉走”这条修口
## 2026-04-14 18:13 prompt_67 续记：补上 non-story roam 的坏点逃逸和 stale failure 清零口
- 本轮主线目标：
  - 继续只收 `Day1 已放人后` 的 NPC / 动物非剧情态 roam 静态执行契约，不碰 Day1 authored contract
- 本轮子任务：
  - 只补 `NPCAutoRoamController` 的两处 own 缺口：
    1. `consecutivePathBuildFailures` 在真实成功推进后仍可能长期挂高
    2. 非剧情态 roam 在坏点 / 局部障碍附近恢复时，仍过度依赖旧 roam center 做随机采样
- 本轮实际改动：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
    - `TryBeginMove()` 现在会在 autonomous recovery 状态下：
      - 先基于当前位置解析一个更局部的 safe sampling center
      - 先尝试 8 个确定性逃逸方向
      - 再回到随机采样
      - 同时严格约束候选点仍留在原 roam 半径内
    - `NoteSuccessfulAdvance()` 现在会在确认真实推进后立刻清零：
      - `consecutivePathBuildFailures`
      - `nextPathBuildAllowedTime`
- 本轮判断：
  - 这刀不是“扩大 roam 半径”或“改静态真值”，而是收掉 own 的恢复链 bug：
    - 避免 `201` 这类“已经成功推进过，但 failure 计数仍卡 16” 的 stale 状态继续拖慢后续决策
    - 避免 NPC / 动物卡在局部坏点时还只围着旧中心盲抽
- 代码层验证：
  - `validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - `errors=0`
    - `warnings=1`
    - 无 owned compile error
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - 通过
- 外部 blocker：
  - 当前 Unity Console 仍有 `SpringDay1Director.cs` 外部 compile error：
    - `SyncPrimaryTownReturnGate` 缺失
  - 因此这轮无法继续做 fresh Play/live 复测，当前只能确认：
    - 我 own 脚本这刀没写红
    - live 验证被 Day1 外部红面阻断
- 当前线程状态：
  - 已执行 `Park-Slice`
  - `thread-state=PARKED`
- 下一轮恢复点：
  1. 等外部 compile blocker 清掉后，直接回 Town fresh live 复测：
     - `201` 是否还会挂 `pathBuildFailures=16`
     - 动物互锁是否减轻
     - `101/103/203` 是否仍会贴房/顶树
  2. 若仍复现，再继续收 `静态 clearance / agent clearance` 的更窄缺口，不回漂 Day1
## 2026-04-14 23:42 只读补记：打包失败后控制台的 `NavigationRoot` 跨场景引用报错，不是 DayNight 本体写坏 Town
- 用户主诉：
  - 打包失败后控制台出现：
    - `不支持交叉场景引用：场景 002 'Primary' 无效引用了场景 NavigationRoot 中的 'Town'`
    - 调用链落在 [EnsureDayNightSceneControllers.cs](D:/Unity/Unity_learning/Sunset/Assets/Editor/EnsureDayNightSceneControllers.cs)
- 本轮子任务：
  - 只读解释这条报错到底是什么，不改代码
- 已核实事实：
  1. [EnsureDayNightSceneControllers.cs](D:/Unity/Unity_learning/Sunset/Assets/Editor/EnsureDayNightSceneControllers.cs) 会在编辑态 additive 打开 `Primary.unity` 和 `Town.unity`，然后调用 `EditorSceneManager.SaveScene(scene)`
  2. 它自己显式补线的是 `DayNightManager/Overlay/GlobalLight/PointLightManager`，没有直接写 `Town.NavigationRoot`
  3. [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 里的 `NavigationRoot/NavGrid2D` 对象 fileID 是 `1107722571 / 1107722573`
  4. [Primary.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Primary.unity) 当前落盘文本里没有这些 fileID；本地序列化仍是自己的 `NavigationRoot/NavGrid2D`（如 `navGrid: {fileID: 163170581}`）
- 当前最核心判断：
  - 这更像“编辑器当前内存态出现了临时跨场景引用”，不是 `Primary.unity` 已经稳定落盘成坏文件
  - `EnsureDayNightSceneControllers` 更像“保存时把问题暴露出来”，不一定是第一责任人
- 当前最高嫌疑链：
  1. [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 的 `OnValidate()` 在编辑态会进 `CacheComponents()`
  2. `CacheComponents()` 里如果 `navGrid == null`，会直接 `FindFirstObjectByType<NavGrid2D>()`
  3. 当 `Primary` 和 `Town` 被同时打开时，这种全局查找可能把 `Primary` 里的某个 NPC 组件临时指到 `Town` 的 `NavGrid2D / NavigationRoot`
  4. 一旦随后保存 `Primary`，Unity 就会报“跨场景引用不支持”并拒绝保存
- 补充判断：
  - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs) 当前 auto-collect 已有 `component.gameObject.scene == gameObject.scene` 过滤，因此不像这一条报错的第一嫌疑
- 验证状态：
  - `静态推断成立`
  - 本轮未改代码、未 live 写
- 下一轮恢复点：
  1. 如果要继续查 exact 责任点，优先核所有会在编辑态跑的 `FindFirstObjectByType<NavGrid2D>()` 链
  2. 重点是把“全局找 NavGrid”改成“只在自己 scene 内找 NavGrid”
## 2026-04-15 00:10 只读大调查：当前 NPC 病态不是单点 bug，而是 `Day1 / crowd / roam / motion` 多 owner 混写
- 当前主线目标：
  - 用户要求我做一轮全盘只读清查，说明现在 NPC 的卡墙、不避让、opening 后回 anchor 病态、到点又继续 roam、以及我与 Day1 边界失控到底在哪里
- 本轮子任务：
  - 只读压清 `导航 own / Day1 own / 交叉污染点 / 体验层失真点`
- 当前最核心判断：
  - 现在的主锅不是“导航一个参数不对”，而是同一个 NPC 至少有 4 条链都在改移动真值：
    1. [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs) 的 staging playback 直接改 `transform`，并写 `SetExternalVelocity / SetFacingDirection`
    2. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 resident crowd/release/return-home 链会 `Acquire/ReleaseResidentScriptedControl`、`PrepareResidentRoamController()`、`ForceRestartResidentRoam()`、`DebugMoveTo()`，甚至在失败时手搓位移
    3. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的 story actor policy 也会 `Acquire/ReleaseResidentScriptedControl`，并在 release 后直接 `StartRoam()`
    4. [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 自己的 autonomous roam state machine 会在到点后 `EnterShortPause -> TryBeginMove` 持续循环采样
- 关键代码事实：
  1. opening 后 resident 回 anchor / home 的链，当前并没有和玩家走同一套静态避障合同：
     - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) `TryBeginResidentReturnToBaseline()` / `TryDriveResidentReturnHomeAutonomously()` 会走 `roamController.DebugMoveTo(homeAnchor)`
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) `ShouldApplyAutonomousStaticObstacleSteering()` 只在 `!debugMoveActive && !IsResidentScriptedControlActive` 时为真
     - 所以一旦走 scripted/debug return-home，这层 autonomous static steering 就被关掉
  2. 如果 autonomous return-home 失败，crowd 还会直接退回手搓位移：
     - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) `TickResidentReturnHome()` 在 `TryDriveResidentReturnHomeAutonomously()` 失败后会 `roamController.StopRoam()`，然后直接 `state.Instance.transform.position += step`
     - 这条 fallback 完全不走静态避障
  3. “刚走完又继续 roam”不是测试残留，而是当前运行时合同：
     - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) `FinishResidentReturnHome()` 明确在 `ResumeRoamAfterReturn` 时调用 `ForceRestartResidentRoam(..., tryImmediateMove: true)`
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 自己本来就是连续 roam：`FinishMoveCycle()` -> `EnterShortPause()` -> `TryBeginMove()`
  4. 玩家为什么看起来比 NPC 稳：
     - 玩家当前是更单一的 `PlayerAutoNavigator -> PlayerMovement` 执行链
     - blocked input / nav bounds / close-range constraint 都在同一条链里收口
     - NPC 只有 autonomous roam 时才开一部分 static steering，story/crowd scripted move 并没有共享这条合同
  5. 小动物和普通 NPC 同根：
     - [TraversalBlockManager2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs) `ApplyNpcTraversalBindings()` 会对场景里所有 `NPCAutoRoamController` 统一绑同一套 `navGrid / soft pass / bounds`
     - 所以只要 `NPCAutoRoamController` 这层合同有缺口，动物也会一起吃到
  6. 001/002/003 与 101~203 不是同一 owner 面：
     - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) `001/002/003` 属 story actor/third resident own
     - `101~203` 更多走 crowd resident 链
     - 这也是为什么用户体感会觉得它们像两种 NPC 系统
- 对用户主诉的直接归因：
  1. `opening 后回 anchor 路上卡墙/不避让`：成立，第一真问题是 `return-home scripted/debug contract` 没和 autonomous/player 静态避障对齐
  2. `到 anchor 后又继续 roam`：成立，但这不是测试在偷偷跑，而是当前 Day1+roam 合同就是这么写的
  3. `剧情里走位顺，free-roam/居民态病`：成立，因为 staging playback 直接改 transform，不是同一套 roam/path/avoidance owner
  4. `我和 Day1 互相干涉`：成立，而且现在是实打实的代码事实，不是用户感觉错了
- 当前最该承认的边界结论：
  - Day1 不只是“决定什么时候放人”，它现在还在改 navigation runtime 配置和生命周期：
    - `SetHomeAnchor`
    - `ApplyProfile`
    - `RefreshRoamCenterFromCurrentContext`
    - `StopRoam`
    - `RestartRoamFromCurrentContext`
  - 这已经越过了“剧情 owner 只管 acquire/release/目的地”的干净边界
- 体验层判断：
  - 当前最多只站住 `结构 / targeted probe`
  - 绝不能 claim `真实入口体验已过线`
- 下一轮恢复点：
  1. 如果继续只读对齐，应给 Day1 一份 exact owner matrix
  2. 如果进入修复，第一正刀不是再调避障参数，而是先收单一 locomotion owner contract
## 2026-04-15 00:16 补记：Day1 中间产物与现码对齐后，确认 `木桩修口` 与 `回家路上不避障` 仍是两层问题
- 用户补充了 Day1 当前中间产物，要求我纳入并继续自己的大调查
- 已核实：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 当前确实已有 `ForceRestartResidentRoam(...)`
  2. [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs) 里确实已有：
     - `CrowdDirector_ShouldForceRestartRoamAfterReturnHomeCompletesEvenWhenControllerAlreadyLooksRoaming()`
     - 同时也保留了 `CrowdDirector_ShouldFallbackToStepReturnHomeWhenRoamControllerCannotStartPath()`
- 这说明两件事：
  1. Day1 现在主攻的是“回到 anchor 后 controller 卡在假 roam / 旧短停半态”的 handoff 问题
  2. 但它并没有解决、而且代码上仍明确保留“return-home 路径起不来时用手搓位移继续退场”的 fallback
- 因此当前更准确的总判断：
  - Day1 的这轮补口能覆盖 `回到 anchor 后像木桩`
  - 但不能覆盖 `回 anchor 路上卡墙/不避障`
  - 后者仍然是 `return-home scripted/debug/mannual fallback` 与 `autonomous static steering` 没收成同一合同

## 2026-04-15 02:34 真实施工续记：导航 own 已把 `release 后 home return` 从 plain debug 收成 formal contract
- 本轮主线目标：
  - 不碰 `spring-day1` 当前 ACTIVE 文件
  - 只在导航 own 内，把 `release 后 DebugMoveTo(homeAnchor)` 收成正式导航执行合同
- 本轮实际落地：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
  - [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs)
- 当前稳定结论：
  1. `homeAnchor/homePosition` 附近的 debug move 现在会被识别成 `formal navigation contract`
  2. 这类 move 不再沿用 plain debug 的 shared-avoidance bypass
  3. 这类 move 现在也会吃静态 steering，并在 stuck/blocked 时优先对同一目标 rebuild，而不是随机抽新 roam 点
  4. formal home return 完成后，`RestartRoamFromCurrentContext(true)` 现在会给一个很短的 settle 窗口，避免“刚回到家就立刻抽下一条 roam”
- 当前 fresh 验证：
  - `validate_script NPCAutoRoamController.cs + NavigationAvoidanceRulesTests.cs`
    - `owned_errors=0`
    - `external_errors=0`
    - 但仍卡 `unity_validation_pending(stale_status)`
  - `errors`
    - `0 error / 0 warning`
  - `git diff --check`
    - 通过
- 当前恢复点：
  1. 下轮先等 Unity `stale_status` 稳下来，再去 Town fresh 看真实体感
  2. 若 live 里仍看到 return-home 最终退回手搓 fallback，那是 Day1 仍在兜底，不是导航 own 还没把 formal contract 接进来

## 2026-04-15 02:48 只读事故定责：`101~203` 再次锁死的最高嫌疑已明确回到 `spring-day1`，而且本质仍是同一个 owner/lifecycle 问题
- 用户主诉：
  - `day1` 又把 `101~203` 锁死，用户认为这已经是第 5 次同类问题，表面上像是不同 bug，实质上像同一根
- 本轮只读核查，未进入施工：
  - 已显式使用：
    - `skills-governor`
    - `sunset-rapid-incident-triage`
    - `sunset-workspace-router`
- 现场事实：
  1. `Show-Active-Ownership` 显示：
     - `spring-day1` 当前又回到 `ACTIVE`
     - 当前 slice 名就叫 `revert-latest-opening-release-regression`
  2. `rapid_incident_probe` 直接把最高嫌疑线程排成：
     - `spring-day1`
     - 分数明显高于 `NPC`
  3. 当前不是“某个旧 commit 已经稳定坏掉”，而是 `spring-day1` 自己的 active dirty working tree 正在改：
     - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
- 当前最关键的新判断：
  - 这不是第 5 个独立根因，而是同一个根问题在反复换壳：
    - `Day1` 还在把 `EnterVillage resident release / return-home / restart-roam` 当成自己要直接手搓的 runtime lifecycle
    - 所以每次只要它再改一次“何时放人 / 放人后怎么 restart / 什么时候 suppress cue”，`101~203` 就会再次被 hold 住
- 这次最可疑的 exact 改动点：
  1. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs#L4627)
     - `ShouldReleaseEnterVillageCrowd()` 现在把放人条件收紧成：
       - `VillageGate` 不能还 active
       - 且必须 `HasVillageGateCompleted()` 或 `house lead` 已 started/queued/waiting
  2. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs#L4641)
     - 新增 `ShouldLatchEnterVillageCrowdRelease()`
  3. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L396)
     - `RefreshEnterVillageCrowdReleaseLatch()` 每帧刷新 `_enterVillageCrowdReleaseLatched`
  4. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L1654)
     - `ShouldSuppressEnterVillageCrowdCueForTownHouseLead()` 现在又吃 `_enterVillageCrowdReleaseLatched` 和 `ShouldReleaseEnterVillageCrowd()`
  5. [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs#L871)
     - 测试名都已经变成“只有 TownHouseLead runtime 真开始才 suppress/release”
- 为什么我认为这就是这次反复锁死的同根：
  1. Day1 这次不是去改导航参数，而是再次改了 opening resident 的“何时继续持有 / 何时才允许 release”合同
  2. 它新增的是更严格的 `VillageGate completed / house lead 真启动` 判定，而不是更早更稳地释放
  3. 这和用户看到的坏相完全一致：
     - `101~203` 不是走歪，而是根本像没被放出来
- 当前不该误判的点：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 这轮虽然也 dirty，但那是我刚落的 `formal return-home contract`，不解释“opening resident 直接被锁死”
- 当前恢复点：
  1. 如果继续，只该让 `spring-day1` 回看这 5 处 release/latch/suppress 代码，不要再泛讲“又是另一个新问题”
  2. 正确口径应压成一句：
     - `Day1 还在直接 owns resident release lifecycle，所以每次改 opening release 条件都会反复把 101~203 锁回去`

## 2026-04-15 03:00 用户现场补证：禁用 `SpringDay1NpcCrowdDirector` 后 resident 不再锁死，但也不会在剧情后自己回家
- 用户现场 fresh 观察：
  1. 直接禁用 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 后，`101~203` 不再出现那种“被锁死/被重新抓住”的 opening 坏相
  2. 但同时也失去了“剧情后自己回 anchor/home”的行为
- 这条观察的意义非常大：
  - 它直接证明当前主锅不是“导航自己把人卡死”
  - 而是 `Day1 crowd director` 既在做剧情 owner，又在做剧情后的 resident runtime 管理
- 当前实现的真实分层：
  1. `Day1` 现在实际上 own 了：
     - resident acquire/release
     - release 后是否回家
     - 如何发起回家
     - 回家后何时 restart roam
     - 甚至失败时手搓 fallback 位移
  2. `NPCAutoRoamController` 现在 own 了：
     - autonomous roam state machine
     - shared avoidance / static steering / replan
     - 我这边刚补的 formal home-return execution contract
  3. 但“剧情结束，谁通知 NPC 该回家”这一步，目前仍然主要绑在 `SpringDay1NpcCrowdDirector`
- 所以用户这句“剧情结束后，Day1 只该放手；NPC 自己回家；导航只负责带路”是对的
- 只是当前现码还没有完全实现成这套边界：
  - 现在更像是：
    - `Day1` 不只放手，还继续亲自管理 resident 的 post-story lifecycle
  - 这就是为什么：
    1. 它开着时，容易重新锁住 resident
    2. 它关掉时，冲突没了，但“回家触发器”也一起没了
- 当前恢复点：
  1. 后续不能再把“关掉 crowd director 后问题消失”误解成“那就把 director 永久关掉”
  2. 正确方向是把 `post-story return-home trigger` 从 Day1 的深度持有里拆轻：
     - Day1 只发 release + target/home 语义
     - NPC 接过“已演完剧情”的状态
     - 导航执行正式回家路径与避障

## 2026-04-15 11:03 工作区补记：用户已把主锅重新钉回导航 own，这轮只收 pure autonomous roam 的互锁与静态障碍执行质量
- 当前新的主线锚点：
  - 即使禁用 `SpringDay1NpcCrowdDirector`，NPC 仍会互相卡死、撞墙、卡墙，所以这轮不再继续围 Day1 打转，而是直接修导航 own。
- 本轮真实施工已落：
  - [NavigationLocalAvoidanceSolver.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs)
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
  - [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs)
- 当前最核心的新判断：
  1. `NPC-NPC` 互锁并不是“完全没有 contract”，而是 deadlock break 太迟、yielding peer 太容易冻住、hold-course peer 又不够稳
  2. `静态撞墙/卡墙` 也不是完全没做，而是 static steering 之前更像止血，不够像真正的侧绕执行合同
- 本轮修法：
  1. `shared avoidance` 提前脱困：
     - yielding NPC 更积极侧移
     - hold-course NPC 更少一起慢停
     - deadlock 判定从 `18f/6 sightings` 提前到 `10f/3 sightings`
     - 死锁时优先建 detour，而不是傻等或直接随机抽新点
  2. `static obstacle steering` 更早、更宽、更稳定：
     - probe 半径和前瞻距离增大
     - 当前方 repulse 接近纯后退时，追加稳定侧绕偏置
     - 被动 NPC blocker reroute 更早触发
- 当前验证状态：
  - `validate_script` 对 3 文件结果：
    - `owned_errors=0`
    - `external_errors=0`
    - 但仍 `unity_validation_pending(stale_status)`
  - `manage_script validate`
    - solver=`clean`
    - NPCAutoRoamController=`warning(1 条既有字符串拼接 GC 提示)`
    - tests=`clean`
  - `git diff --check` 通过
- 当前恢复点：
  1. 下一轮先回 Town 做 pure autonomous roam 的 fresh live，不再混进 Day1 持有样本
  2. 如果互锁/贴墙仍在，再继续只收导航 own 的 `destination agent clearance / detour distance`

## 2026-04-15｜补记：已收到新版导航 prompt，Day1 后 release / return-home / free-roam 的边界已按用户最新裁定重写
- 这轮新增 prompt：
  - [2026-04-15_给导航_Day1解耦后return-home与free-roam执行合同唯一主刀prompt_68.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-15_给导航_Day1解耦后return-home与free-roam执行合同唯一主刀prompt_68.md)
- 新 prompt 固定了 3 个关键口径：
  1. opening / dinner 的 staged contract 仍归 Day1 own；
  2. 导航只收“已 release 之后”的 movement quality；
  3. 不再接受 `return-home` 继续走 debug / scripted / fallback 半混合合同。
- 当前恢复点：
  1. 后续如果导航继续开刀，应优先按 `prompt_68` 的 owner matrix 与唯一下一刀回执；
  2. 不再回到“导航顺便替 Day1 收 phase 语义”的旧口径。

## 2026-04-15 11:44 只读补记：用户牛圈截图已把当前导航主锅进一步压向“逐步执行 clearance 仍偏脚底化”
- 本轮严格只读，未进真实施工。
- 新补入的高价值证据：
  1. 用户最新牛圈截图说明，局部抽搐不只发生在房屋/树根贴边，也会发生在动物近接触区。
  2. 这与当前代码事实能对上：
     - `NavigationTraversalCore.CanOccupyNavigationPoint(...)` 仍以脚底三点 probe 为主
     - `NPCAutoRoamController.TickMoving()` 的逐步推进仍要回落到这套占位判定
     - `NPC` 的 `GetAvoidanceRadius()` 还被 cap 在 `colliderRadius + 0.06f`
     - `PlayerAutoNavigator` 的静态前瞻与 repulse 则更强
- 当前父线程新判断：
  - `复杂 collider 形状 / 位置不一` 会放大 `NPC` 坏相，但更本质的问题不是场景错，而是 `NPC` 逐步执行 clearance 合同比玩家窄。
  - 牛圈 case 不像纯静态 truth 漏收，更像：
    - `per-step occupancy 偏窄`
    - 叠加 `local avoidance / blocked recovery`
    - 最后形成原地小幅抽搐。
- 当前恢复点：
  1. 后续导航 own 如果继续，只该优先收“已 release 后 movement execution 的 body-clearance-aware contract”
  2. 不要再把这层问题简化成“只调参数”或“只怪场景 collider”

## 2026-04-15 12:17 施工补记：`released-movement-body-clearance-contract` 第一刀已落
- 本轮真实施工只收导航 own：
  - 把 `released 后 return-home / free-roam` 的 per-step occupancy，从脚底 probe 主导，收成 body-clearance-aware 的下一步位置约束
- 实际修改：
  - [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs)
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
  - [NavigationAvoidanceRulesTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs)
- 当前稳定结论：
  1. 这刀没有去改 `Day1 phase` 或 staged movement
  2. 只让：
     - autonomous free-roam
     - formal home return
     这两类 `released 后` movement，改走 clearance-aware 的 next-position constraint
  3. plain debug / resident scripted move 不吃这刀，边界仍守住
- fresh 代码层验证：
  - `validate_script <3 files>`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
    - Unity 仍被 `stale_status` 阻断
  - `manage_script validate`
    - `NavigationTraversalCore=clean`
    - `NavigationAvoidanceRulesTests=clean`
    - `NPCAutoRoamController=warning(既有 string concat GC 提示)`
  - `git diff --check`
    - 通过
- 当前恢复点：
  1. 下轮优先做 `Town` fresh live，只验：
     - released 后贴边磨
     - 牛圈/动物局部抽搐
  2. 若仍复现，继续只收导航 own 的：
     - detour distance
     - destination agent clearance

## 2026-04-15 15:08 补记：当前 `NavigationAvoidanceRulesTests.cs` 的 3 个 compile 红已清，compile-first 剩余阻断改判为外部噪音
- 用户新贴的 3 个错误：
  - `CS1061: Component.enabled`
  - `CS0246: NavigationAgentRegistry`
  - `CS1503: Component -> string`
- 当前结论：
  1. 这 3 个错误对应的是上一版测试写法，当前现码已不再保留那套写法。
  2. fresh `manage_script validate` 结果：
     - `NavigationAvoidanceRulesTests=clean`
     - `NavigationAgentRegistry=clean`
     - `NPCAutoRoamController=warning only`
  3. compile-first 还拿不到完全干净票，但当前阻断已经不是这 3 个 owned compile red：
     - `validate_script` 会吃到外部 `DialogueChinese V2 SDF.asset` import inconsistent
     - 或外部 `The referenced script (Unknown) on this Behaviour is missing!`
     - `compile` 会被 `CodexCodeGuard/dotnet timeout` 阻断
- 额外现场处理：
  - 已发 `STOP` 命令把 Unity 退回 `EditMode`
  - 当前 `thread-state=Parked`
- 当前恢复点：
  1. 若后续继续追 `no-red`，先清外部 console 噪音，再重跑 compile-first
  2. 导航 own 本身此刻不再卡在这 3 个编译错误上

## 2026-04-15 16:24 补记：Town 自漫游 live probe 已完成，两轮结果都说明“未过线”
- 当前主线：
  - 只测导航 own 的 `Town` 自漫游质量，不混 `Day1`。
- 本轮实际做成了什么：
  - 在 Unity `Play Mode` 的 `Town` 场景里，连续跑了两轮 `Tools/Sunset/NPC/Run Roam Spike Stopgap Probe`
  - 每轮 6 秒，直接采当前 roam NPC 的 live 运行态
  - 同时补读 fresh console
- 结果：
  1. 两轮都是 `npcCount=25 / roamNpcCount=25`
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
  4. fresh `errors` 为 `0`
- 当前判断：
  - 这说明现在不是“导航完全没工作”
  - 但也绝对还不能叫“导航正确了”
  - 因为 live 样本里仍持续有：
    - `Stuck`
    - `SharedAvoidance`
    - `MoveCommandNoProgress`
    - `ReusedFrameMoveDecision / ReusedFrameStopDecision`
  - 玩家体感上对应的就是：
    - 局部互卡
    - 原地小幅抽搐
    - 刚想走又停、反复修同一小段
- 当前恢复点：
  1. 下一刀继续只收导航 own：
     - `destination agent clearance`
     - `local avoidance oscillation`
     - `detour candidate distance`
  2. 当前验证层只站住 `targeted probe + live`
  3. 不能包装成“体验过线”

## 2026-04-15 16:42 补记：用户把导航问题压成“坏点 / 静态导航垃圾 / 不会自救”三根，这个归纳比我前几轮拆法更对
- 当前新增判断：
  1. 这三条不是抱怨，而是正确的主根拆解。
  2. 我前几轮真实落地的是：
     - 坏点 first-fit 止血
     - 一部分 body-clearance 止血
     - 一部分 blocked/oscillation 止血
     - 外加全场扫描降噪
  3. 但这些仍主要是止血和减爆炸，不是一次性把三根都切死。
- 当前最准确口径：
  - `坏点`：还没收成统一硬合同
  - `静态导航`：还停在“路径后处理更保守”，没完全推进到路径成立层
  - `自救`：还停在“更快放弃坏状态”，没做到稳定脱困闭环
- 当前恢复点：
  1. 后续如果继续施工，必须按这三个根重新排唯一下一刀，而不是再按症状散修
  2. 用户现在要的是“彻底讲清楚我前几轮到底改了什么、为什么还不够、下一步要怎么真落地”

## 2026-04-15 17:02 正式总交接文档已落
- 当前主线：
  - 用户要求导航线程不要再只给局部回卡，而是写一份正式总交接文档，把历史语义、边界、代码地图、历史修改、用户裁定、Day1/NPC 接缝和后续接手方式一次性讲清。
- 本轮实际做成了什么：
  1. 已新增正式文档：
     - [2026-04-15_导航线程总交接文档_own问题_边界语义_历史修改_后续接手.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-15_导航线程总交接文档_own问题_边界语义_历史修改_后续接手.md)
  2. 文档已明确收了：
     - 用户对 `opening / dinner / 001/002 / 003 / Town resident / 夜间` 的语义裁定
     - `prompt_68` 的 `Day1 / Navigation` 边界
     - `NPC` 线程关于低级 public 写口和 facade 的判断
     - 导航 own 代码总地图
     - 历史修改时间线
     - 当前已证实 / 未证实事实
     - `坏点 / 静态导航 / 不会自救` 三根主问题
     - 后续不是“唯一主刀”的 work package 建议
     - 所有历史材料入口路径
- 当前阶段：
  - 导航线程已完成“正式交接整理”阶段
  - 不是继续施工阶段
- 当前恢复点：
  1. 后续如果要继续导航 own 施工，应先读这份总交接，再决定是否按 `坏点 -> 静态路径成立层 -> 完整脱困闭环` 三包继续
  2. 当前线程已合法 `PARKED`

## 2026-04-19 只读补记：Town 空气墙更像 scene 真实 Tilemap/CompositeCollider，而不是 TraversalBlockManager2D 凭空造墙
- 当前主线：
  - 用户要求只读排查 `Town.unity` 里的“看起来空白的位置撞墙”，明确区分它更像真实 scene collider / tilemap collider，还是 `TraversalBlockManager2D` 自动收集/导航阻挡配置导致。
- 本轮子任务：
  - 只读核对：
    - `Assets/000_Scenes/Town.unity`
    - `Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs`
    - `Town` 里与阻挡/Collider/Tilemap 相关的场景对象
  - 不改代码、不改 scene 资产、不进真实施工。
- 本轮新站稳的事实：
  1. `Town.unity` 里这份 `TraversalBlockManager2D` 不是空配置：
     - 它已显式落盘 `blockingTilemaps` 与 `blockingColliders`
     - 同时还开着 `autoCollectSceneBlockingTilemaps=1`、`autoCollectSceneBlockingColliders=1`
     - 证据在 [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity#L180899) 一带。
  2. 但它自己不会生成新的 `Collider2D`：
     - 脚本只会把现有 `Tilemap/Collider2D` 收集后喂给 `NavGrid2D.ConfigureExplicitObstacleSources(...)`
     - 再把 `bindPlayerMovement / enforcePlayerNavGridBounds` 这类约束绑给玩家/NPC。
  3. `Town` 里已经真实存在多组非 trigger 的 scene 阻挡体，而且有几组体量很大：
     - `SCENE/LAYER 1/Tilemap/"基础设施"/-9970`
       - `Tilemap size = 52x20`
       - 挂了 `TilemapCollider2D + CompositeCollider2D + Rigidbody2D`
       - GO 还被打了 `Building` tag
       - 证据在 [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity#L69491)、[Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity#L70387)、[Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity#L70632)。
     - `SCENE/LAYER 1/Tilemap/"基础设施"/-9960`
       - `Tilemap size = 10x12`
       - 同样是 `TilemapCollider2D + CompositeCollider2D`
       - 证据在 [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity#L166292)。
     - `SCENE/LAYER 1/Tilemap/"基础设施"/-9975`
       - `Tilemap size = 6x12`
       - 同样是 `TilemapCollider2D + CompositeCollider2D`
       - 证据在 [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity#L180182)。
  4. 另外 `轨道` 组还有 5 张窄条 `Props` tilemap 直接挂着 `TilemapCollider2D`，都是真碰撞，不是导航影子：
     - `Layer 1 - Props / (1) / (2) / (3) / (4)`
     - tile 尺寸分别约 `1x4 / 1x2 / 1x2 / 1x3 / 1x1`
     - 如果玩家是在轨道/边条附近“撞到一条看不明显的线”，这组也很可疑。
  5. `Layer 1 - Farmland_Water` 虽然也被塞进了 `blockingColliders`，但当前 `Tilemap size = 0x0`：
     - 这说明它作为“当前空气墙主嫌”的证据反而偏弱，更像历史残留配置或空 source。
- 当前最准确判断：
  1. `Town` 的“空气墙”更像 scene 里已经存在的真实 `TilemapCollider2D / CompositeCollider2D` 在拦人；
  2. `TraversalBlockManager2D` 的作用更像“把这些现成阻挡再次同步进 NavGrid 和玩家/NPC 约束”，会强化“这里不能走”的体验；
  3. 但如果用户说的是“人物像撞到实体墙一样被顶住”，第一嫌疑仍应先看 scene 真实 collider，而不是先怪 `TraversalBlockManager2D`。
- 不该误判的点：
  1. 不要把“manager 开了 auto collect”误判成“manager 自己凭空造出了碰撞体”。
  2. 不要把 `Layer 1 - Farmland_Water` 这种当前 `size=0x0` 的空 tilemap，当成这轮空气墙铁证。
  3. 不要忽略 `基础设施` 下面那几组大尺寸 `CompositeCollider2D`；它们比脚本关键词本身更像“看起来空、其实有碰撞轮廓”的直接来源。
- 当前恢复点：
  1. 如果后续继续排这类空气墙，优先先在 `Town.unity` 里对位 `基础设施/-9970/-9960/-9975` 和 `轨道/Props_*` 的可见图层与 collider 轮廓，不要先改 `TraversalBlockManager2D.cs`
  2. 当前这轮结论已够支持“先按 scene collider 真值排查”，不需要把第一刀误发给导航代码 owner

## 2026-04-23 16:30｜shared-root 保本上传先缩到 docs-only，代码面只留 blocker
- 当前主线：
  - 按 `2026-04-23_给导航检查_shared-root完整保本上传与own尾账归仓prompt_01.md` 只做导航 own 成果保本上传，不继续修导航坏 case。
- 本轮实际动作：
  1. 先把过宽上传 slice `nav-check-own-upload-2026-04-23` 合法 `Park-Slice`。
  2. 已确认旧 slice 不适合继续的原因：
     - 它会把 `Assets/Editor`、`Assets/YYY_Scripts/Service/Navigation`、`Assets/YYY_Scripts/Controller/NPC` 一起带成 parent roots。
     - 在 shared-root `task` 模式下会直接撞 `own roots remaining dirty`，不符合这轮只收 clearly-own 的完成定义。
  3. 随后新开 docs-only slice：
     - `nav-check-own-docs-upload-2026-04-23`
  4. docs-only `Ready-To-Sync` 已通过。
  5. 中途 `ready-to-sync.lock` 只是一段外部共享锁占用，不是导航 own 范围问题；等锁释放后已正常通过。
- 当前已明确可安全归仓的导航 own：
  - `.codex/threads/Sunset/导航检查/memory_0.md`
  - `.codex/threads/Sunset/导航检查/memory_1.md`
  - `.codex/threads/Sunset/导航V3/memory_0.md`
  - `.kiro/specs/999_全面重构_26.03.15/导航检查/memory.md`
  - `.kiro/specs/屎山修复/导航V3/memory.md`
  - `.kiro/specs/屎山修复/导航检查/memory.md`
  - `.kiro/specs/屎山修复/导航检查/03_NPC自漫游与峰值止血/memory.md`
  - `.kiro/specs/屎山修复/导航检查/2026-04-14_导航线程_Day1边界重划后非剧情态roam静态契约收口prompt_67.md`
  - `.kiro/specs/屎山修复/导航检查/2026-04-15_导航线程总交接文档_own问题_边界语义_历史修改_后续接手.md`
  - `.kiro/specs/屎山修复/导航检查/2026-04-15_给导航_Day1解耦后return-home与free-roam执行合同唯一主刀prompt_68.md`
- 当前不能吞并、必须原样保留的 exact blocker files：
  - `Assets/Editor/NavigationStaticPointValidationMenu.cs`
  - `Assets/Editor/NavigationAvoidanceRulesValidationMenu.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2DStressTest.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs`
  - `Assets/YYY_Scripts/Service/Navigation/StairLayerTransitionZone2D.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs.meta`
  - `Assets/YYY_Scripts/Controller/NPC/NpcLocomotionSurfaceAttribute.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NpcLocomotionSurfaceAttribute.cs.meta`
- blocker 原因：
  - 这些文件一旦进入本轮白名单，`git-safe-sync` 会把 own roots 扩成：
    - `Assets/Editor`
    - `Assets/YYY_Scripts/Service/Navigation`
    - `Assets/YYY_Scripts/Controller/NPC`
  - 而这些同根当前仍混着外线 dirty / mixed，不符合这轮“只收 clearly-own，不吞 shared/mixed”的上传边界。
- 当前恢复点：
  1. 本轮先完成 docs-only 导航 own 归仓。
  2. 代码面后续要么等治理位拆 root，要么等更明确的 owner 分流后再开独立上传切片。

## 2026-04-23 16:36｜docs-only own upload 已落 commit 并 push
- 本轮提交：
  - `91c99ec7` `docs: upload navigation own handoff artifacts`
- push 状态：
  - 已 push 到 `origin/main`
- 当前 thread-state：
  - `导航检查 = PARKED`
- 当前仍留本地 blocker：
  - `Assets/Editor/NavigationStaticPointValidationMenu.cs`
  - `Assets/Editor/NavigationAvoidanceRulesValidationMenu.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2DStressTest.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs`
  - `Assets/YYY_Scripts/Service/Navigation/StairLayerTransitionZone2D.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs.meta`
  - `Assets/YYY_Scripts/Controller/NPC/NpcLocomotionSurfaceAttribute.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NpcLocomotionSurfaceAttribute.cs.meta`

## 2026-04-23 16:52｜shared-root 第二波只试 1 个历史小批次，首撞 own-root 扩根后已停车
- 当前主线：
  - 按 `2026-04-23_给导航检查_shared-root历史小批次上传prompt_02.md` 只还原 1 个历史小批次上传尝试；撞 blocker 就停车，不换第二批。
- 本轮唯一尝试批次：
  - `Assets/YYY_Scripts/Service/Navigation/StairLayerTransitionZone2D.cs`
  - `Assets/YYY_Scripts/Service/Navigation/StairLayerTransitionZone2D.cs.meta`
  - `Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs.meta`
- 这组是不是独立历史批次：
  - `是，但不是今天临时拼的探测包。`
  - 证据是 `2026-04-19` 历史记忆里已经存在“楼梯层级切换最小脚本”这一刀；当前这轮按治理位要求，只取其中最小 script/meta 子簇做一次真实上传尝试。
- 真实上传尝试结果：
  - `Ready-To-Sync` 失败。
- 第一真实 blocker：
  - 不是代码闸门，也不是 shared-root 大厅本身。
  - 第一阻断点就是：这组白名单虽然只有 3 个文件，但 own root 仍被归到 `Assets/YYY_Scripts/Service/Navigation`，随后撞到同根 remaining dirty。
- exact blocker files：
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2DStressTest.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs`
- 本轮明确没做的事：
  - 没有去动第二组导航代码。
  - 没有顺手吞：
    - `NavigationStaticPointValidationMenu.cs`
    - `NavigationAvoidanceRulesValidationMenu.cs`
    - `NpcLocomotionSurfaceAttribute.cs`
- thread-state：
  - 已 `Begin-Slice`
  - 已真实跑 `Ready-To-Sync`
  - 已按 blocker `Park-Slice`
  - 当前状态：`PARKED`
- 当前恢复点：
  - 这轮第二波单批次尝试已经结束；后续若继续，必须由治理位或下一轮 prompt 明确给出新的唯一小批，不能在这轮里自己换批次。

## 2026-04-24 01:59｜shared-root 第三波改走 Service/Navigation 根内整合批，首 blocker 升级为 CodexCodeGuard 工具 incident
- 当前主线：
  - 按 `2026-04-23_给导航检查_ServiceNavigation同根整合上传prompt_03.md`，承认 `prompt_02` 已完成，不再重复撞楼梯三件套；本轮只做 `Service/Navigation` 根内整合批 1 次真实上传尝试。
- 本轮唯一切片：
  - `Assets/YYY_Scripts/Service/Navigation/StairLayerTransitionZone2D.cs`
  - `Assets/YYY_Scripts/Service/Navigation/StairLayerTransitionZone2D.cs.meta`
  - `Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs.meta`
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2DStressTest.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs`
- 这批现在是否能诚实视作 `Service/Navigation` 根内整合批：
  - `是。`
  - 原因：第二波已经把 `Service/Navigation` 父根扩根钉成首 blocker，而当前根内脏改正好就是这 6 个文件；把前一刀挡路的 `NavGrid2D / NavGrid2DStressTest / NavigationAgentRegistry` 正式并入后，这组才首次覆盖到当前整个根内导航残留面。
- 真实上传尝试：
  1. 已按规则执行：
     - `Begin-Slice`
     - `Ready-To-Sync`
     - `Park-Slice`
  2. 中途出现过一次外层命令 timeout，随后只做了同一切片的 stale lock 清理与继续，不是换第二刀。
  3. 最终拿到的第一真实 blocker 已变化：
     - 不再是 `same-root remaining dirty`
     - 而是 `CodexCodeGuard` 在 `Ready-To-Sync` 阶段未返回 JSON
- 新的第一真实 blocker：
  - `CodexCodeGuard incident during Ready-To-Sync (no JSON result)`
- 当前能站稳的负面结论：
  - 这轮没有提交成功
  - 没有新 commit
  - 没有新 push
  - 也没有在当前返回里出现新的 `Service/Navigation` 同根 remaining dirty 清单；工具 incident 在那之前先把流程打断了
- 本轮明确没越权扩到：
  - `Assets/Editor/NavigationStaticPointValidationMenu.cs`
  - `Assets/Editor/NavigationAvoidanceRulesValidationMenu.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NpcLocomotionSurfaceAttribute.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NpcLocomotionSurfaceAttribute.cs.meta`
- 当前恢复点：
  - 如果继续，这条线下一刀不该再重跑同一整合批，而应先把 `CodexCodeGuard` 工具 incident 单独查穿，再决定是否重试当前根内整合批。
