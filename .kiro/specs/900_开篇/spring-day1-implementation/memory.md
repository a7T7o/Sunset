# Spring Day1 Implementation - 活跃入口记忆

> 2026-04-10 起，旧根母卷已归档到 [memory_3.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/memory_3.md)。本卷只保留阶段索引、当前主线和恢复点。

## 当前定位
- 本工作区负责 `spring-day1` 的长期实现分阶段索引。
- 不再让父工作区根卷继续吞对白验证、导演尾账、Town/Home 承接、玩家面协作和 runtime 黑盒终验的全过程。

## 当前状态
- **最后校正**：2026-04-10
- **状态**：阶段已重新分流
- **当前活跃阶段**：
  - `004_runtime收口与导演尾账`

## 阶段索引
- `000-深入交流`
- `001-全面勘察`
- `002-初步搭建`
- `003-进一步搭建`：已冻结，只保留历史阶段沉淀与提示
- `004_runtime收口与导演尾账`：当前唯一活跃阶段

## 当前稳定结论
- `day1` 当前已经不是“最小对话系统验证”阶段。
- 当前主风险集中在：
  - 导演尾账
  - runtime 黑盒终验
  - 与 `Town / Home / UI / NPC` 的 contract 吃回
  - 语义边界已经切给公共链的问题，如 placement

## 当前恢复点
- 后续 `day1` 新进展只写到：
  - [004_runtime收口与导演尾账/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/memory.md)
- 查 2026-03-24 到 2026-04-08 之间的大量协作 prompt、导演分场与旧阶段堆叠时，再回看：
  - [003-进一步搭建/memory_0.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/memory_0.md)

## 2026-04-10 最新收口提示
- `004_runtime收口与导演尾账` 已新增一轮 runtime/perf 记录：
  - `NPC 自漫游抽搐`
  - `Package 打开卡顿`
- 当前最新判断：
  - 这两条线都已吃进 `spring-day1` 自己的代码层第一刀
  - live 终验仍卡在 Unity MCP `instances=0 / no_unity_session`，不是代码层再度爆红

## 2026-04-10 补记
- `004_runtime收口与导演尾账` 当前又新增两条已落地恢复点：
  - `0.0.5` 农田教学里的 `PlacementMode` 提示语义已由 `day1` 自己收口
  - 给 UI 的唯一续工 prompt 已切到 `0.0.2_玩家面集成与性能收口`，不再回写冻结的 `0.0.1 SpringUI`

## 2026-04-10 夜间 resident 审计恢复点
- 本轮新增一份只读审计结论，已写入：
  - [004_runtime收口与导演尾账/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/memory.md)
- 当前 authoritative 判断：
  - “21:00 后 resident 一定回锚并站定、次日再恢复白天位置/漫游”的安全切入点不在 `NPCAutoRoamController` 单点补丁，而在：
    1. `SpringDay1Director.HandleHourChanged(...)` 负责时间触发和 snapshot 节点
    2. `SpringDay1NpcCrowdDirector.ApplyResidentBaseline(...)` 负责真正阻断 `StartRoam()` 回流
- 额外风险提醒：
  - `ApplyStoryActorMode(...)`
  - `PersistentPlayerSceneBridge` resident restore
  - `SnapResidentsToHomeAnchorsInternal()` 末尾重开 roam
  这三处都是夜间规则最容易被冲掉的地方。

## 2026-04-10 真实施工补记
- `004_runtime收口与导演尾账` 本轮已从只读转入真实施工，并新增一份 runtime 收口记录：
  - `Day1 时间守门`
  - `21:00 后 resident 夜间站定`
  - `存档回退 phase 防偷推`
- authoritative 细节入口：
  - [004_runtime收口与导演尾账/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/memory.md)
- 当前状态：
  - 代码层 `owned_errors=0`
  - Unity live 仍待下一轮黑盒复判
  - 还没开始吃 UI `34.md` 阶段回执

## 2026-04-11 fresh 复判补记
- `004_runtime收口与导演尾账` 已新增一轮 fresh targeted bridge 复判。
- authoritative 结论已改成：
  - 不能再说 `spring-day1` 当前“只差 UI 34 或只差黑盒没跑”；
  - opening / midday / late-day 三段桥接测试都已出现 fresh failure；
  - 因此 `day1` 当前仍是 `runtime 闭环未过线`，不是“你前面那一整轮都完成了”。
- 具体 authoritative 入口：
  - [004_runtime收口与导演尾账/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/memory.md)
- 当前最新恢复点：
  - 先修 runtime 桥接与睡觉/DayEnd 真闭环
  - UI `34.md` 仍是后续调度项，不是当前 fresh 失败的主根

## 2026-04-11 late-day 只读定位补记
- `004_runtime收口与导演尾账` 又补了一轮只读定位，目标只盯 `late-day bridge failures`，没有进入真实施工。
- authoritative 结论新增两点：
  1. `BedBridge_EndsDayAndRestoresSystems` / `FreeTimeValidationStep_AdvancesFromFinalCallToDayEnd` 的最小正确修法在 `SceneTransitionRunner` 本体：
     - `EnsureInstance()` / `Awake()` 的 `DontDestroyOnLoad(...)` 需要按 `Application.isPlaying` 条件化
     - 不能只在 `HandleSleep()` callsite 打补丁
  2. `DayEndPlayerFacingCopy_ShouldCarryTomorrowBurdenAndClearWorkbenchState` 当前失败主因不是 DayEnd 文案坏了，而是测试没有满足 `_freeTimeIntroCompleted == true` 的正式前置条件，所以根本没进入 `DayEnd`
- authoritative 细节入口：
  - [004_runtime收口与导演尾账/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/memory.md)

## 2026-04-11 opening 两条 fresh failure 只读结论补记
- `004_runtime收口与导演尾账` 已新增一条 opening 窄审计。
- authoritative 结论：
  1. `VillageGateCompletionInTown_ShouldPromoteChiefLeadState`
     - 主因是测试夹具漂移，不是运行时没进 chief lead；
     - 需要补 chief actor 测试夹具，而不是先放松导演的 actor 解析 contract。
  2. `VillageGateWhileActiveInTown_ShouldKeepPostEntryBeatUntilDialogueCompletes`
     - 主因是 `SpringDay1Director.IsTownHouseLeadPending()` 语义过宽；
     - `VillageGate` formal 仍 active 时，就被过早报成 pending。
- 具体修法与方法级结论已写入：
  - [004_runtime收口与导演尾账/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/memory.md)

## 2026-04-11 后续天数 02:00 自动睡觉 contract 已补口
- `004_runtime收口与导演尾账` 已完成一刀真实施工，专门收“Day1 之后超过凌晨两点未睡也要回 Home 床边并走睡觉收束”。
- authoritative 结论：
  - `SpringDay1Director.HandleSleep()` 不再只会处理 Day1 自己的 `DayEnd`
  - 非 Day1 sleep 现在也会走公共 fallback：
    - 强制回 `Home`
    - 贴到当前休息位
    - 同时收束 resident 到 home anchors
  - 用户明确点名的 `+` 号跳时路径，已经有 targeted EditMode 回归覆盖
- authoritative 细节入口：
  - [004_runtime收口与导演尾账/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/memory.md)
- 当前状态：
  - late-day bridge suite fresh 通过
  - `PromptOverlay / 任务清单` 仍是后续尾账，不在这刀范围内

## 2026-04-11 Day1 存档回退 / phase 偷推进尾账补记
- `004_runtime收口与导演尾账` 已新增一轮只读审查。
- authoritative 结论：
  - 这条线**还没完全闭环**；
  - 晚段 `dinner / reminder / free-time` 的 phase 偷推进已补；
  - 但 `StoryProgressPersistenceService.ApplySpringDay1Progress(...)` 仍会按 `currentPhase` 回填前中段私有位，当前 tests 也还没用 mismatch-load 快照把这组剩余风险钉死。
- 当前最值钱的下一刀：
  - 不是“只补测试”或“只改 persistence”二选一；
  - 而是先补前中段 mismatch-load 回归，再把 persistence 里剩余 phase 派生收平。
- 额外现场提醒：
  - `StoryProgressPersistenceService.cs` 与 `StoryProgressPersistenceServiceTests.cs` 当前仍是 `untracked`，所以仓库落地层同样还不能算闭环。
- authoritative 细节入口：
  - `004_runtime收口与导演尾账/memory.md`

## 2026-04-11 targeted fresh 复判已过线
- `004_runtime收口与导演尾账` 这轮已经从“只读判断还有尾账”推进到“核心 targeted contract fresh 复判通过”。
- authoritative 结论更新为：
  - `PromptOverlay / 任务清单` owner split 已落地
  - `save rollback / phase 偷推进` 的前中后段 targeted 回归已落地
  - `opening / midday / late-day / prompt / story-progress / workbench fallback / NPC formal gate / director staging`
    全部已拿到 fresh pass artifact
  - 当前剩下的不是新的代码 blocker，而是用户最终实玩体验层终验
- 当前 authoritative 入口仍是：
  - [004_runtime收口与导演尾账/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/memory.md)
- 当前状态：
  - `spring-day1` 本轮已合法 `Park-Slice`
  - 线程 live 状态 = `PARKED`

## 2026-04-11 并行线程审计补记
- `004_runtime收口与导演尾账` 又新增了一轮只读总审，但这次不是只看 Day1 自己，而是从 Day1 视角审当前并行 7 线程的可交付度与打包影响。
- authoritative 结论：
  - 当前不该再扩大施工面；
  - `P0` 应冻结到少数真正影响打包的 live/smoke 项：
    1. Day1 全流程终验
    2. 存档 packaged smoke
    3. farm/placement/toolbar completion ticket
    4. 若 Town 常驻 NPC 仍 severe，则回 Day1/crowd owner 收口
  - 排序重构、导航考古、编辑器预览完善都不应继续抢当前打包窗口。
- authoritative 细节入口：
  - [004_runtime收口与导演尾账/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/memory.md)

## 2026-04-11 用户终验首轮问题补记
- `004_runtime收口与导演尾账` 新增一轮只读复盘，专门对应用户首轮手跑暴露的 3 条问题：
  1. `任务清单 / PromptOverlay` 的 bridge prompt 与正式任务卡语义重复、位置过抢；
  2. `19:00 -> Town` 晚饭切入时报 `SpringDay1PromptOverlay` inactive coroutine 错；
  3. Home 流程被门口 `E` 睡觉劫持，无法按原需求“先进屋，再对床睡觉”。
- authoritative 结论：
  - 第 2 条和第 3 条已可直接定性为真 bug；
  - 第 1 条不是用户主观挑剔，而是 `owner split` 后体验层取舍没做好：结构方向对，桥接提示策略过头。
- authoritative 修法方向：
  - `PromptOverlay.Hide()/FadeCanvasGroup()` 必须对 inactive 对象做同步收口，不得再盲开 coroutine；
  - `HomeDoor/HouseDoor` 只能做 forced sleep fallback 的定位代理，不能再被自动绑成 `SpringDay1BedInteractable`；
  - 任务清单后续要做“保留新版准确文案 + 降低 bridge prompt 侵入 + 对 formal card 做语义去重”的融合收口。
- authoritative 细节入口：
  - [004_runtime收口与导演尾账/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/memory.md)

## 2026-04-11 用户首轮终验三条问题已做真实修复
- `004_runtime收口与导演尾账` 已完成一刀真实施工，专门收：
  1. `SpringDay1PromptOverlay` inactive fade coroutine 报错
  2. 门口被误绑成睡觉交互点
  3. 任务清单 bridge prompt 语义重复与侵入性过重
- authoritative 结论：
  - `PromptOverlay.Hide/FadeCanvasGroup` 现在已对 inactive runtime instance 做同步收口，不再对 inactive 对象开 coroutine；
  - `TryAutoBindBedInteractable()` 不再把 `HomeDoor/HouseDoor` 这类 rest proxy 误当正式睡觉交互点；
  - `bridge prompt` 现在会对 `focus/subtitle/footer/items` 做更强的语义去重，并降视觉侵入。
- authoritative 验证：
  - `spring-day1-prompt-overlay-guard-tests.json`：`passCount=7`
  - `spring-day1-late-day-bridge-tests.json`：`passCount=7`
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 20`：`errors=0 warnings=0`
- authoritative 细节入口：
  - [004_runtime收口与导演尾账/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/memory.md)
## 2026-04-11｜子工作区 004 新收口已过线：Day1 时间语义重定时 + 夜间回家 contract + PromptOverlay HUD/modal 收平
- `spring-day1 / 004_runtime收口与导演尾账` 这轮已新增一刀真实施工，并且 fresh targeted 已通过。
- authoritative 结论：
  1. Day1 晚段时间语义已改成：
     - `18:00` 晚饭
     - `19:00` 归途提醒
     - `19:30` 自由时段
  2. Town 居民夜间 contract 已改成：
     - `20:00` 自主回 anchor
     - `21:00` 未到位则强制贴回 anchor / night-rest
  3. `PromptOverlay` 现在会优先挂到 `UI` 下的基础 Canvas，不再误挂 `PackagePanel` 模态层；开包时也会按统一模态规则退场，用户截图里那种“左侧任务面板还露着”的坏相已对位修正
  4. 晚段分钟级 guardrail 的测试已改成验证 `NormalizeManagedDay1TimeTarget(...)`，不再被 EditMode 下 `Application.isPlaying == false` 的 helper 前提卡死
- authoritative 验证：
  - `spring-day1-prompt-overlay-guard-tests.json`：`11/11 passed`
  - `spring-day1-late-day-bridge-tests.json`：`8/8 passed`
  - `spring-day1-director-staging-tests.json`：`31/31 passed`
- 当前 authoritative 风险：
  - fresh console 仍残留 Unity TestRunner cleanup residue（`TestResults.xml` 提示），它是验证工具噪音，不是新的 Day1 runtime blocker
- 当前状态：
  - 本轮已 `Park-Slice`
  - `spring-day1 = PARKED`

## 2026-04-12｜Day1 live 复判新增锚点：Town 居民“像冻结”不是导航 resume 缺口，而是 crowd caller + return-home contract 问题
- `spring-day1` 本轮新增了一次 fresh live 复判，不再只靠静态源码猜。
- authoritative live 事实：
  - 通过 `Spring Day1 Live Snapshot Artifact` 抓到 `Town / EnterVillage / EnterVillage_HouseArrival` 现场；
  - `101~203` 这批 resident 在 `10:10 AM` 和 `12:10 PM` 两张快照里都仍处于 `return-home`；
  - `001` 自己仍显示 `roam=True|state=Moving`，说明“剧情 actor 在动”和“常驻居民像冻结”不是同一根。
- authoritative 判断：
  - 这类坏相第一责任层已落到 `SpringDay1NpcCrowdDirector` 的 caller/runtime contract，而不是 `NPCAutoRoamController.ReleaseResidentScriptedControl()` 自己失效；
  - 更关键的是，resident 的 `return-home` 当前仍在 `TickResidentReturnHome()` 里做直线硬推，没有接回玩家同级静态导航 contract，这一点与用户长期需求直接冲突。
- 当前结论状态：
  - `静态推断 + fresh live snapshot` 已成立；
  - 尚未进入真实修复，因此不能 claim“已解决”。
- 下一步唯一正确方向：
  - 继续施工时，应优先把 `SpringDay1NpcCrowdDirector` 的 resident return-home 接回 `NPCAutoRoamController.DriveResidentScriptedMoveTo(...)`；
  - 这是 Day1 own 的 runtime 收口，不应再把锅漂回“导航线程自己去修”。

## 2026-04-12｜Day1 runtime 新收口：resident return-home contract 已从硬推 transform 改回 scripted move/pathing
- `spring-day1 / 004_runtime收口与导演尾账` 本轮已把上一个 live 复判结论落成真实修复。
- authoritative 结论：
  1. `SpringDay1NpcCrowdDirector` 的 resident return-home 不再对有 `NPCAutoRoamController` 的 NPC 做手搓位移；
  2. 现在优先走 `DriveResidentScriptedMoveTo(...)`，与 NPC 自己的 motion/facing/pathing 链对齐；
  3. 旧的线性硬推只保留给极窄的“没有 roam controller” fallback，不再是主路径。
- authoritative 验证：
  - `spring-day1-director-staging-tests.json`：`32/32 passed`
  - 新护栏 `CrowdDirector_ShouldNotFallbackToHardPushWhenResidentHasRoamController` 已执行通过
  - Editor.log 有 fresh `Assembly-CSharp-Editor.dll` 编译成功
- 当前 authoritative 风险：
  - CLI compile-first 这次被 `dotnet 20s timeout` 卡成 `blocked`，所以不能把 CLI 票包装成完全 no-red；
  - `HouseArrival` after-fix 的真实玩家体感还没在同一轮 live 里完整跑到终点，仍需用户终验。
- 当前状态：
  - 本轮已 `Park-Slice`
  - `spring-day1 = PARKED`

## 2026-04-12｜Day1 继续收口：晚饭开场的 001/002 站位已从旧围观 marker 切回晚饭区域
- `spring-day1` 本轮继续围绕打包前晚段体验做 own 收口，没有扩到别的系统。
- authoritative 代码改动：
  - `SpringDay1Director.AlignTownDinnerGatheringActorsAndPlayer()` 现在不再把 `001/002` 对位到 `EnterVillageCrowdRoot` 的旧 `起点` marker；
  - 改成优先围绕 `DirectorReady_DinnerBackgroundRoot / DinnerBackgroundRoot` 做晚饭开场对位，避免再次出现“001/002 跑到左上角围观位”的坏相。
- authoritative 护栏：
  - 新增 `SpringDay1LateDayRuntimeTests.AlignTownDinnerGatheringActorsAndPlayer_ShouldPreferDinnerAreaOverVillageCrowdMarkers`
  - `Run Late-Day Bridge Tests` 已接入该护栏
- authoritative 验证：
  - `spring-day1-late-day-bridge-tests.json`：`9/9 passed`
  - `spring-day1-director-staging-tests.json`：`32/32 passed`
- 当前 authoritative blocker：
  - fresh PlayMode live 仍被外部 UI 文件 `InteractionHintOverlay.cs` 的编译错误阻断，所以这轮还不能 claim“玩家体感终验完成”
- 当前状态：
  - 本轮已 `Park-Slice`
  - `spring-day1 = PARKED`

## 2026-04-12｜Day1 新事故复判：`002` 剧情后 Town resident 的“地震式抖动”主锅仍在 day1 caller/runtime contract
- 这轮没有继续改代码，严格停在只读事故定责。
- 新增 authoritative 判断：
  - `001/002` 和普通 resident 不是同一条控制链；
  - 普通常驻 resident 仍由 `SpringDay1NpcCrowdDirector` 通过 `ApplyResidentBaseline() -> TryBeginResidentReturnHome() / TickResidentReturnHome()` 接管；
  - 这条 return-home 现在虽然已经不再主走 `transform` 硬推，但仍是 `DriveResidentScriptedMoveTo(...) -> DebugMoveTo(...)` 的 scripted move，不是玩家同级静态导航 contract。
- 新增 authoritative 根因锚点：
  1. resident scripted move 当前会主动绕开 shared avoidance；
  2. resident scripted move 在 blocked advance 下又不会像普通 roam 那样早停退出；
  3. `001/002` 的剧情链自己也还在走 `transform.position / SetExternalVelocity / SetFacingDirection` 的直推 move，而不是统一避障 contract。
- 对用户坏相的解释：
  - 这三层叠起来，就会出现：
    - resident 整片原地高频小抖
    - 朝向不明显变化
    - `001/002` 被人堆顶住、还能把玩家推着走
- 当前验证状态：
  - `静态推断成立`
  - direct MCP fresh live 这轮未补成，因为 Unity session 当前没重新挂回 MCP；同时 `Editor.log` 仍带 UI 外部红错残留，不能包装成 fresh live 已过。
- 下一恢复点：
  - 若继续施工，第一刀必须继续收 `spring-day1` own：
    - resident scripted move 接回 shared avoidance / blocked abort；
    - `001/002` story actor move 去掉直推链，统一回到玩家同级静态导航 contract。

## 2026-04-12｜Day1 继续收口：resident scripted move 已重新接回避让语义，晚段桥接未被带坏
- `spring-day1` 本轮重新进入真实施工，目标就是收掉“`002` 剧情后 Town resident 整片地震式抖动”和“`001/002` 缺少正常避障”这一组 day1 own runtime 坏相。
- authoritative 代码改动：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
    - resident scripted move 不再绕开 shared avoidance
    - resident scripted move 在静态零推进坏点上重新允许 stopgap 早停
  - [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
    - 新增两条 resident-scripted-control 护栏测试
  - [SpringDay1TargetedEditModeTestMenu.cs](D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs)
    - 已把这两条护栏接进 `Director Staging Tests`
- authoritative 验证：
  - `git diff --check`：本轮 3 个目标文件 clean
  - `spring-day1-director-staging-tests.json`：`32/32 passed`
  - `spring-day1-late-day-bridge-tests.json`：`9/9 passed`
- authoritative 残余：
  - `npc-resident-director-bridge-tests.json` 当前 `0/3`，但它落在 stagebook / manifest / cue contract 面，不是本轮改动路径
  - 所以这轮能诚实 claim 的是：`当前修复已落地、既有晚段桥接没被带坏；fresh 玩家体感仍待黑盒终验`
- 当前状态：
  - 本轮已 `Park-Slice`
  - `spring-day1 = PARKED`
