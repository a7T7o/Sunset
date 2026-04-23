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

## 2026-04-15｜102-owner冻结与受控重构已进入 Package B 第一刀
- `spring-day1` 这条线当前新增一个活跃子阶段：
  - `102-owner冻结与受控重构`
- authoritative 结论：
  1. `Package B` 第一刀已经落代码，不再只停在冻结表或引导文本
  2. `SpringDay1NpcCrowdDirector` 的 opening/daytime release 已从 crowd-owned `return-home` 改成 shared baseline release
  3. `SpringDay1Director` 已删除 `003` opening 后专用 runtime shim，多点 handoff 调用已去掉
  4. Day1 侧对 `NPCAutoRoamController` 的低级写口已经迁到当前存在的 facade 面
- authoritative 验证：
  - direct MCP 定向 EditMode：6/6 passed
  - fresh console：`errors=0 warnings=0`
  - `git diff --check` 覆盖 touched files 通过
- authoritative 风险：
  - 这轮仍是“结构退权过线”，不是 live 体感最终过线
  - `night schedule / recover chain / NPC facade 本体 / 导航统一执行合同` 仍在后续包
- authoritative 细节入口：
  - [102-owner冻结与受控重构/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/memory.md)

## 2026-04-17｜late-day 聊天权限只读审计恢复点
- `004_runtime收口与导演尾账` 新增了一轮只读审计，专门回答：
  - 为什么 `19:30` 以后尤其 `20:00` 后会出现“只有 `002` 能聊天、其他 NPC 不能聊”。
- authoritative 结论新增 3 点：
  1. `001` 当前没有 player-initiated informal bundles，和 `002/003` 不是同一类问题。
  2. ordinary resident 与 `003` 在 `20:00+` 被 `return-home / scripted control / hide` 链直接收口，现码本来就会逐步不可聊。
  3. 最像的异常点是 synthetic `001/002/003` 没被纳入 `PersistentPlayerSceneBridge` 的 crowd-managed id 过滤，scene restore 后容易出现 owner/chat 恢复不对称。
- authoritative 细节入口：
  - [004_runtime收口与导演尾账/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/memory.md)
- 当前恢复点：
  - 如果后续要修这条线，第一刀优先收 synthetic ids 与 native snapshot 的 managed 集不一致；不要先放宽 `20:00` 回家时的互动 gate。
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

## 2026-04-13｜父层补记：Day1 当前 P0 阻塞已从“导航泛锅”收束到 crowd runtime registry 恢复漏口
- `spring-day1` 这轮围绕用户最新 P0 阻塞做了 fresh live + editor probe：`002` 后 Town resident 整片卡死/像冻结，当前已不再按“导航 own”处理。
- 当前 authoritative 结论更新：
  1. `Town / EnterVillage_HouseArrival` 的正常 fresh 现场里，`101~203` resident 可以是 `scriptedControlActive=false` 且 `isRoaming=true`，说明 resident 并非天然不会动；
  2. 但在一次 runtime 重建后，`SpringDay1NpcCrowdDirector` 会出现 `_spawnStates=0`、`spawned=0|missing=101~203`，而 Town scene 中 resident 实例仍然留在场；
  3. 这就是用户看到“scene 里 NPC 还在，但整片像死掉”的一条真根：crowd runtime registry 丢失后没有及时 rebind scene-bound residents。
- `spring-day1` 已在 `SpringDay1NpcCrowdDirector.Update()` 落了一刀最小恢复：Town 活场景下若发现 scene resident 仍在但 runtime registry 为空，就主动 `SyncCrowd()` 重建绑定。
- 当前状态：用户 live 口头反馈已看到“npc 可以走了”，所以线程本轮先停给用户继续黑盒终验；当前 `spring-day1 = PARKED`。

## 2026-04-13｜父层补记：已给导航落同步文件
- 新增：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-12_给导航_002后TownResident冻结根因与当前修复同步.md`
- 用途：
  - 把这次 `002` 后 `Town resident` 冻结的 fresh 定责同步给导航
  - 说明当前这条线已不优先按导航 core 处理

## 2026-04-13｜父层补记：已按用户要求生成“存档线程 / spring-day1 自身”双 prompt
- 新增：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给存档系统_Day1晚段恢复语义与职责分工prompt.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给spring-day1_晚段打包前彻底收尾prompt.md`
- 目的：
  - 先把 Day1 晚段恢复的职责分账说清
  - 避免 `spring-day1` 再越权去修通用 save/load
  - 同时也避免 save thread 反过来替 Day1 收 runtime staging / prompt 布局

## 2026-04-13｜父层补记：双 prompt 已升级为整条 Day1 v2
- 上一版只聚焦晚段 scope，已被用户明确否决。
- 当前应以新版 v2 为准：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给存档系统_整条Day1恢复contract与职责分工prompt_v2.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给spring-day1_整条Day1打包前总收尾prompt_v2.md`

## 2026-04-13｜父层补记：UI 语义 contract 与 spring-day1 v3 已生成
- 用户进一步裁定后，当前又新增：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给UI_整条Day1任务清单与Prompt语义contract.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给spring-day1_整条Day1打包前总收尾prompt_v3.md`
- 最新职责边界：
  - UI 全量接手 Day1 玩家可见 UI
  - `spring-day1` 不再自己改 UI 壳体，但必须把完整 UI 语义 contract 交全
  - 晚饭开场的 own runtime 兜底已被用户明确成“最多等待 5 秒，超时瞬移必要剧情 actor 后开戏”

## 2026-04-13｜父层补记：spring-day1 已落地晚饭 5 秒兜底与 001/002 必要强制就位
- `spring-day1` 这轮已按 v3 prompt 完成晚饭 P0 runtime 收尾：
  - `SpringDay1Director.BeginDinnerConflict()` 现在会对 `DinnerConflictTable` 最多等待 5 秒
  - 超时后不再无限卡住，而是继续开戏，并在开戏前把 `001/002` 强制拉回晚饭区域
  - 这条逻辑同时覆盖主动找村长触发与 `+` 快进后最终都会汇入的晚饭开场链
- 目标测试已过 4/4，fresh console `errors=0 warnings=0`
- 当前父层判断：
  - Day1 最阻塞打包的晚饭卡顿主根已从“会卡死”降成“最多等 5 秒，超时也能开戏”
  - 下一步最值钱的是用户 live 复测主动触发与 `+` 快进两条入口，而不是再泛修 UI 壳体

## 2026-04-13｜父层补记：晚段第二轮补口已压实 02:00 forced sleep、003 夜晚链与 19:30 resident 放行
- `spring-day1` 本轮继续围绕用户最新的晚段坏相收口，没有扩到 UI/save 越权面。
- 当前 authoritative 代码层补口：
  1. `SpringDay1Director.HandleHourChanged()` 在 Day1 `FreeTime && hour>=26` 时，如果 `TimeManager.Sleep()` 没即时把导演带回 `HandleSleep()`，会主动补一次，确保 Day1 立刻收进 `DayEnd`
  2. `SpringDay1Director.TryPlacePlayerNearCurrentSceneRestTarget()` 现在对带 `Rigidbody2D` 的玩家同时写 `body.position + transform.position`，Home 床边 fallback 不再只改刚体不改视觉落点
  3. `SpringDay1Director.ShouldKeepStoryActorNightRestControl()` 已把 `003` 纳入，夜晚导演控制不会再被下一帧 runtime policy 误释放
  4. `SpringDay1NpcCrowdDirector.ResolveResidentParent()` 新增 `19:30~20:00` free-time 释放口径：除真正的 `Priority witness actor` 外，其它 resident 回到 `Resident_DefaultPresent`，不再被扣在 `Resident_BackstagePresent / Resident_DirectorTakeoverReady`
- 当前 authoritative 验证：
  - `spring-day1-late-day-bridge-tests.json`：`13/13 passed`
  - `spring-day1-director-staging-tests.json`：`39/39 passed`
- 额外 authoritative 判断：
  - 我中途用于取证的 `Force Spring Day1 Dinner Validation Jump` live 样本并不稳定，不能拿来当产品最终体验结论
  - 但它反向暴露并帮助我钉死了一个真根：`19:30` resident baseline 仍在把普通居民扣进 backstage 语义，这条现在已由代码与测试一起收平

## 2026-04-14｜父层补记：Day1 当前三条关键 owner 已在只读层重新钉死
- `004_runtime收口与导演尾账` 本轮新增了一次只读彻查，不直接开修，目标是把用户最新追问的 3 条关键问题重新压成明确 owner：
  1. 开场剧情就位错误
  2. 剧情结束后 NPC 不按白天目标恢复、但 `20:00` 会回家
  3. `02:00` forced sleep 的 Day1 own 与共享层分工
- 当前 authoritative 结论：
  - 开场就位主锅在 `SpringDay1Director` 的 opening staging / hold / marker resolution contract，不在导航；
  - resident 白天释放与夜间回家主锅在 `SpringDay1NpcCrowdDirector` 的 state machine 分裂，不在导航；
  - `02:00` 的 Day1 第一夜剧情收束仍归 `SpringDay1Director`，但“所有天数统一超过 02:00 睡觉”最终不应长期留在 Day1 own。
- 当前恢复点：
  - 下一轮如果继续真实施工，应按 `opening -> resident -> 02:00 分层` 这个顺序回到 `004_runtime收口与导演尾账/memory.md` 的最新恢复点，不再把这三条问题混成 UI / save / navigation 泛锅。

## 2026-04-14｜父层补记：三线程 owner 边界 prompt 已生成
- 用户最新裁定不是立刻再开修，而是先把 `spring-day1 / 导航 / UI` 三条线程的边界与续工 prompt 正式写清。
- 本轮已新增三份正式 prompt：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-14_给spring-day1_Day1三线程owner边界重划与打包前唯一主刀prompt_v4.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-14_导航线程_Day1边界重划后非剧情态roam静态契约收口prompt_67.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.2_玩家面集成与性能收口\2026-04-14_UI线程_Day1最终UI语义与时间owner边界收口prompt_12.md`
- 当前父层 authoritative 边界：
  1. `spring-day1` 继续主刀 opening authored contract、resident 日夜状态机、Day1 第一夜 forced sleep
  2. `导航` 只主刀 Day1 已放人后的非剧情态 NPC/动物静态执行契约
  3. `UI` 只主刀 Day1 玩家可见 UI contract 与 `TimeManagerDebugger +/-`
- 本轮没有新增业务代码修改，属于文档/治理层收口；恢复点仍在 `004_runtime收口与导演尾账` 子层 memory 的最新条目。

## 2026-04-14｜父层补记：opening own 第一刀已回到真实代码
- 子层 `004_runtime收口与导演尾账` 本轮已从 prompt 收口重新进入真实施工，但 scope 仍严格限制在 opening own 第一刀。
- 当前 authoritative 结果：
  1. `EnterVillage_PostEntry` crowd cue 的 suppress 条件已改正，不再在对白前就被错误释放
  2. `SpringDay1Director` 现在会严格消费 scene authored 终点；authored root 已存在但点位缺失时，不再静默 fallback 开戏
  3. opening 相关 EditMode 回归已新增三条
- 当前 authoritative blocker：
  - Unity 当前外部 compile red 落在 `NPCAutoRoamController.cs`，不在 `spring-day1` 本轮 own scope
  - `Ready-To-Sync.ps1` 还被 stale `ready-to-sync.lock` 卡住，因此这刀目前停在 `PARKED`，不是 `READY`
- 父层恢复点：
  - 一旦外部 compile red 与 stale lock 清掉，优先回到子层 memory 里的 opening live 复验，而不是先扩到第二刀

## 2026-04-14｜父层补记：resident release 与 Day1 第一夜 forced sleep 第二刀已拿到 targeted 4/4
- 子层 `004_runtime收口与导演尾账` 继续在 `spring-day1 own` 内收了第二刀，但仍未越过 UI / 导航边界。
- 当前 authoritative 结果：
  1. resident scripted cue 结束后的 release 不再默认吃 `DailyStand_*` 聚堆点，而是优先回各自 `BasePosition`
  2. Day1 `26:00` forced sleep 现在和 generic 路径对齐成“非 `Home` 就先切 `Home` 再摆位”
  3. Day1 owner regression targeted menu 已新增，并拿到 `4/4 pass`
- 子层这轮还额外吸收了 UI 告知：
  - UI 线程继续 own 任务卡 / bridge prompt / PromptOverlay / `TimeManagerDebugger +/-`
  - `spring-day1` 只继续 own canonical state / staging / release / first-night state machine
- 父层当前判断：
  - `spring-day1` 这轮最关键的 owner contract 已经有定向 EditMode 证据，不再只是静态推断
  - 当前没能进 `READY` 不是因为这轮新刀没收平，而是 `spring-day1` 整条线历史 own roots 脏改量过大
- 父层恢复点：
  - 后续优先看用户 live 复测；若 live 仍有坏相，再回子层只收剩余 runtime contract，不扩到 UI / navigation

## 2026-04-14｜父层补记：spring-day1 当前自然停顿点已落本地 checkpoint
- `spring-day1` 子层本轮已把 own 第二刀收成最小本地回退点：
  - 提交 `442b9a40` `checkpoint: spring-day1 owner contract fixes`
- 父层当前 authoritative 状态：
  - `spring-day1` 已合法 `Park-Slice`
  - 当前 live 状态=`PARKED`
  - 当前 blocker 不再是 stale 锁，而是这条线程历史 own roots 残留脏改太多，导致 `Ready-To-Sync` 不能代表整线 clean
- 父层当前判断不变：
  - 已站住的是 `spring-day1 own canonical contract`
  - 仍待用户终验的是 opening 后 resident live 恢复、以及 Day1 `26:00` 真实切 `Home`
  - 后续不应再把这两条 live 结果回漂成 UI / 导航 / save 泛锅

## 2026-04-14｜父层补记：用户新一轮截图已把 Day1 / 导航边界重新压实
- 用户最新反馈不是单一坏点，而是把当前问题重新压成：
  1. `spring-day1 own`
     - dinner / reminder 正在错误复用 opening cue
     - release 后没有执行“回 anchor 再恢复 roam”
  2. `导航 own`
     - scripted move 期间静态 steering 被关闭，导致斜线路径偏头摇摆与卡顿
- 父层当前 authoritative 结论：
  - Day1 当前最优先不再是继续补测试，而是先把 own staging / release 语义改回用户原始需求：
    - opening / dinner 只有“起点 -> 终点 -> 最多 5 秒 -> 超时只 snap 必要 actor -> 对话前一刻就位”
    - 戏后不是原地开始 roam，而是先回 anchor/home 再恢复日常
  - scripted move 的静态避障与偏头摇摆，虽然坏相发生在 Day1 回家路上，但 exact root 已落到导航 contract，而不是 UI 或 save

## 2026-04-14｜父层补记：opening resident 抖动已缩成 Day1 own 的单点冲突
- 用户最新 live 报错把问题进一步缩小：
  - 不是所有 return-home 都坏，而是 `opening` 结束后 `101~203` 在“开始退场”与“被下一帧取消退场”之间打架。
- 子层这轮已落最小热修：
  - 在 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 中，只对“`ReleasedFromDirectorCue && NeedsResidentReset` 触发的白天退场”保留 return-home，不再被旧 clock gate 当场取消。
- 父层当前 authoritative 判断：
  - 这次 `101~203` 抖动不是导航 own，也不是 UI/save 越权，而是 `spring-day1` 自己在 `ApplyResidentBaseline()` 里写出了互相矛盾的 release contract。
  - 这轮修口仍严格在 `Town / Day1 own` 边界内，没有去碰 `Primary`、UI 壳体或导航 static clearance。
- 当前恢复点：
  1. 等用户 fresh live 复测 opening resident 是否脱离“走两步后抖死”
  2. 通过后再回到 dinner authored marker / 001002 就位 contract

## 2026-04-14｜父层补记：Town resident 的“出现时机”与“退场路径”已拆成两条不同 contract
- 用户最新复测把问题进一步压实：
  - `101~203` 现在不再是“不会回 anchor”，而是“在还没开放 Town 日常的阶段，被过早放活并留在 Town roam”
- 父层当前 authoritative 判断：
  1. `退场路径`
     - 这条是前一轮已经在收的 `cue release -> return-home -> 再决定是否 resume roam`
  2. `出现时机`
     - 这是新补清楚的一条单独 contract
     - 在 `HealingAndHP / Workbench / Tutorial` 等还没开放 Town 常驻居民的阶段，玩家即使手动回 Town，也不应看到 `101~203` 已经自己开始日常活动
- 子层这轮已按这个边界收口：
  - 默认不再把 `EnterVillage ~ ReturnAndReminder` 的 resident 当成“daytime baseline 就可见”
  - 只保留两类例外：
    - 当前 beat 明确要在场
    - 正在执行剧情散场退回 anchor/home
- 父层恢复点：
  1. 先让用户只复测 `Primary 对话后直接回 Town`
  2. 通过后再回到 dinner authored marker / 001002 站位 contract

## 2026-04-14｜父层补记：Day1 定点回退已复核到文件级，未发现越界回退
- 用户最新强制要求：
  - 必须确保“只回退我这条线自己的内容，不碰别人的内容”。
- 这轮父层收到的子层结论：
  1. 本次回退只点到了 5 个 Day1 文件：
     - `SpringDay1DirectorStaging.cs`
     - `SpringDay1Director.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - `SpringDay1DirectorStagingTests.cs`
     - `SpringDay1LateDayRuntimeTests.cs`
  2. 当前 `HEAD` 对比只剩 2 个文件还有本线 diff：
     - `SpringDay1DirectorStaging.cs`
     - `SpringDay1NpcCrowdDirector.cs`
  3. `SpringDay1Director.cs` 与两份测试文件已经完全回到仓库基线。
- 父层 authoritative 判断：
  - 现在可以站稳地说：这次回退没有把 `Day1` 之外的路径一起拖回去。
  - 还留在工作树里的本线差异，也已经缩成两处明确可解释的“兼容面 + 止血早退”，不是继续散开的乱修状态。
- 父层恢复点：
  1. 若继续施工，只能从这两个剩余文件继续追 `opening` crowd freeze
  2. 继续施工前，不再把“回退范围不清”当成额外不确定性

## 2026-04-14｜父层补记：当前坏相主锅更像 Day1 runtime owner 持有，不是导航代码被回退
- 用户转发导航线程最新结论：
  - 导航 own 的 `NPCAutoRoamController.cs` / `NPCMotionController.cs` 窄修仍在，没被 Day1 物理抹掉。
- 父层吸收后的 authoritative 判断：
  1. 当前 `101~203` 冻住，第一嫌疑仍在 Day1 runtime owner：
     - `ApplyStagingCue(...)` 只要还命中 cue，就不会进入 baseline release
     - `ApplyResidentBaseline(...)` 只有在 cue 不再命中后，才会真正 `ReleaseResidentDirectorControl(...)`
  2. `003` 的 Town 侧异常也仍更像 Day1 own 的 story/runtime 持有链，而不是导航 core 被回退
  3. 因此这条线现在必须继续按“Day1 runtime contract”修，不该再把“导航代码物理丢失”当主锅
- 对“回退前做过什么、现在回到了什么状态”的父层定性：
  - 回退前，我这条线确实做过几刀 runtime 语义热修：
    - resident release / return-home 语义
    - `HealingAndHP` 时机语义
  - 这些已经退回
  - 现在工作树只剩两处最小残留 diff：
    - `SpringDay1DirectorStaging.cs` 兼容面
    - `SpringDay1NpcCrowdDirector.cs` apply-cue 早退止血
- 父层恢复点：
  1. 后续继续施工时，只追 `EnterVillage crowd cue release` 和 `runtime owner release`
  2. 导航线只继续 own `free-roam static clearance / facing hysteresis`

## 2026-04-14｜父层补记：爆红已收口，但 checkpoint 仍卡在 same-root 依赖集合
- 父层阶段判断：
  - 这轮最新用户贴出的 `SpringDay1Director.cs` 爆红已经清掉，不再是 active blocker。
  - 当前真正的阻断已经重新收缩成“same-root own dependency set 没并入 checkpoint”，不是代码还红。
- 父层新结论：
  1. `TryPrepareTownVillageGateActors()` 的红面只是我这一刀里的局部变量初始化漏口，不是新的系统性崩盘。
  2. story actor 的 `night/home anchor` 现在已经改成 scene-authored 真值优先，目的是压住 dinner/夜间回家被 runtime anchor 污染的风险。
  3. `Ready-To-Sync` 当前只卡 7 个同根文件：
     - `DialogueManager.cs`
     - `SpringDay1NpcCrowdManifest.cs`
     - `StoryManager.cs`
     - `SpringDay1TownAnchorContract.cs(+.meta)`
     - `StoryProgressPersistenceService.cs(+.meta)`
- 父层恢复点：
  1. 下轮要先决定这 7 个文件里哪些属于本 slice 必须同提，哪些要明确留给 save/UI 或更大的 runtime checkpoint。
  2. 在这一步做完前，不要 claim “可以直接 commit/sync”；当前只到“代码无红、checkpoint 仍 blocked”。

## 2026-04-15｜父层补记：opening 后统一 release contract 第一刀已落，当前等待 fresh live
- 父层吸收的子层结论：
  1. `SpringDay1NpcCrowdDirector.cs` 已把 opening 后 resident 的 daytime release / return-home 从
     - `DebugMoveTo`
     - `StopRoam / RestartRoamFromCurrentContext`
     - `transform.position += step`
     这套组合拳里抽出来，改成只保留 `DriveResidentScriptedMoveTo(...) -> 到 anchor 后 release -> idle 时 StartRoam()`。
  2. `SpringDay1Director.cs` 已补 `ReleaseOpeningThirdResidentControlIfNeeded(...)`，把 `003` 在 `VillageGate -> house lead` 交棒边界从 `spring-day1-director` owner 里放回 resident contract。
  3. `SpringDay1DirectorStagingTests.cs` 已补 `003` release 守护，并把“手搓 step fallback”改成禁止项守护。
- 父层当前判断：
  - 这条子层现在已经不再是“继续泛修 runtime 语义”，而是确实在收用户批准的唯一窄刀口：`opening 后 003 + 101~203 的统一 release contract`。
  - 当前可以站住的只有“代码层 clean”，还不能说 live 已过线。
- 父层恢复点：
  1. 下一张 fresh live 票只看：
     - `101~203` 是否还会 opening 后两步抽搐
     - `003` 是否还罚站
     - 到 anchor 后是否恢复 roam
  2. 若不过线，仍只允许继续追这条 release contract；不准父层放行扩到 dinner / 夜间总合同 / 导航 core。
  3. 该子层本轮已 `Park-Slice`，当前 thread-state=`PARKED`，等待 fresh live 票再决定是否继续第二刀。

## 2026-04-15｜父层补记：用户已用“禁用 CrowdDirector 后 roam 恢复”把当前错位重新钉回 owner 架构
- 用户最新 live 反证非常直接：
  - 只要把 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 对应 runtime 关闭，Town resident 就恢复正常 roam；
  - 剩下只有“夜间不会自己回家”。
- 父层因此更新后的 authoritative 判断：
  1. 当前第一主锅不再是“导航没接住”；
  2. 而是 `spring-day1` own 的 `CrowdDirector` 继续越权代管 resident 的剧情后生命周期；
  3. 当前实现的真实形态已经可以定性为：
     - `Day1` 没有只发语义命令；
     - 而是把 `resident release / return-home / restart-roam / runtime owner` 一起吞进了自己。
- 子层这轮已经基于这条新证据落下一刀 WIP：
  - daytime 无显式 crowd beat 时，让 resident autonomy 接管，不再继续让 `SyncCrowd()` 抓着不放；
  - 同时把 `SyncCrowd()` 的重复 scratch 分配改成复用，直砍 profiler 里最显眼的 GC 面。
- 当前父层口径：
  - 这刀目前只站住“代码层 clean + 架构方向对”，还没有 fresh live 票；
  - 后续仍必须只围绕 `CrowdDirector` 这条 owner 越权链继续，不准再漂回 UI / 导航 core / Primary house-arrival。

## 2026-04-15｜父层补记：三线程新 prompt 与 spring-day1 自限红线文档已补齐
- 这轮不是继续实改代码，而是按用户最新要求，先把 `Day1 / 导航 / NPC` 三方的新边界和 `spring-day1` 自己的永久红线正式写成文档。
- 当前已新增 4 份正式文件：
  1. [2026-04-15_给spring-day1_Day1语义解耦与CrowdDirector退权唯一主刀prompt_v5.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/2026-04-15_给spring-day1_Day1语义解耦与CrowdDirector退权唯一主刀prompt_v5.md)
  2. [2026-04-15_spring-day1_红线与落地清单_剧情后resident退权版.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/2026-04-15_spring-day1_红线与落地清单_剧情后resident退权版.md)
  3. [2026-04-15_给导航_Day1解耦后return-home与free-roam执行合同唯一主刀prompt_68.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-15_给导航_Day1解耦后return-home与free-roam执行合同唯一主刀prompt_68.md)
  4. [2026-04-15_给NPC_Day1解耦后resident状态合同与外部调用面收口prompt_01.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/NPC/2026-04-15_给NPC_Day1解耦后resident状态合同与外部调用面收口prompt_01.md)
- 父层 authoritative 新结论：
  1. `spring-day1` 后续不再被允许把 `CrowdDirector` 扩成 resident 剧情后的第二导演。
  2. 下一刀若继续，只允许沿“剧情后 resident 退权”这一条前进，不准扩到 dinner / UI / 导航 core / `Primary`。
  3. 施工前必须先做冲突盘点，这条已被提升为文档硬约束，不再只是聊天提醒。
  4. 本轮文档工作已收口，`spring-day1` 当前 live 状态重新回到 `PARKED`。

## 2026-04-15｜父层补记：opening 后 resident 退权第一刀已继续压实，当前停在 fresh live 前
- 子层 `004_runtime收口与导演尾账` 本轮继续真实施工，但 scope 仍严格锁在：
  - `CrowdDirector` 从剧情后 resident 生命周期里退权
  - `003` 与 `101~203` 在 opening handoff 后进入同一 release contract
- 子层这轮新增的 authoritative 落点：
  1. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - 已补 `ReleaseOpeningThirdResidentControlIfNeeded(...)`
     - 并接到 `VillageGateSequenceId` 完成、`HasVillageGateProgressed()/HasHouseArrivalProgressed()` 早退分支、`TryBeginTownHouseLead()`
  2. [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - 已把旧“回 base pose / 手搓 step fallback”守护改成：
       - `yield to autonomy`
       - `禁止 manual step fallback`
       - `003 village gate handoff release`
  3. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - 本轮没有再扩题，只清了 return-home 残留的小脏口
- 当前父层判断：
  - 子层这轮没有重新扩到 dinner、夜间总合同、导航 core 或 UI 壳体，仍守在用户批准的唯一窄刀口里。
  - 当前更准确的状态是：
    - 代码层 contract 更接近“Day1 只放手，不继续代管 resident 下半生”
    - 但 live 体验票还没闭环
- 当前验证摘要：
  - `manage_script validate SpringDay1Director` => `errors=0 warnings=3`
  - `manage_script validate SpringDay1NpcCrowdDirector` => `errors=0 warnings=2`
  - `manage_script validate SpringDay1DirectorStagingTests` => `errors=0 warnings=0`
  - `errors --count 20 --output-limit 5` => `0 error / 0 warning`
  - `git diff --check` 覆盖本轮三文件通过
  - `validate_script` 两个主文件与测试文件都仍会落到 `unity_validation_pending`，原因是 Unity 现场 `stale_status`，不是 owned red
- 当前恢复点：
  1. 父层下一步不该放行第二刀；先等 fresh live 看 `003 / 101~203` 的实际 release 体验。
  2. 如果不过线，仍只允许继续追这条 opening 后 resident release contract。
  3. 子层本轮已重新 `Park-Slice`，当前状态=`PARKED`。

## 2026-04-15｜父层补记：fresh profiler 已把“当前像锁死”重新定性成 Day1 触发的 autonomous-roam 风暴，不是旧 scripted freeze 原样复发
- 用户最新 fresh 证据同时包含：
  1. live 体感：
     - resident 不再小幅抽搐，而是再次呈现“像直接不动”
  2. profiler：
     - 热点持续落在 `NPCAutoRoamController.Update()`
     - `SpringDay1NpcCrowdDirector.Update()` 占比很小
     - 热对象是 `101 / 102 / 103 / 104 / 202 / 203`
- 父层吸收后的 authoritative 判断：
  1. 当前已不能再把症状简单写成“CrowdDirector 还在 scripted hold”；
  2. 更准确的说法是：
     - `spring-day1` 这轮把 resident 从 opening cue 结束后直接 `ReleaseResidentToAutonomousRoam(...)`
     - 导致一批 crowd resident 在错误、拥挤的 cue 终点位直接启动 free-roam
     - 随后 `NPCAutoRoamController` 的 autonomous sampling / agent clearance / path build 进入性能风暴，玩家体感就像“全员又锁死”
  3. 所以这次“性能爆炸”不是单边锅：
     - **触发器在 Day1**：错误 release 语义
     - **爆炸面在 NPC/导航**：坏输入没有被 cheap fail，而是炸成 1s 级 `Update()`
- 父层恢复点：
  1. `spring-day1` 下一刀必须先把 opening 后 resident 改回“return-to-anchor/home 后再 roam”，不能继续 direct autonomous roam。
  2. 导航/NPC 也必须补一刀“坏 release 输入下的 fail-fast / 限流 / 不再全量 FindObjectsByType + 高成本采样风暴”。
  3. 当前这条判断站住的是 `fresh live + profiler + 代码对位`，还不是“修复已闭环”。

## 2026-04-15｜父层补记：opening direct-autonomy 短路已收掉，当前先停在 fresh live 口
- 子层 `004_runtime收口与导演尾账` 已沿着父层刚刚钉死的判断继续真实施工：
  - 不再讨论“是不是导航自己突然爆了”
  - 直接把 `Day1` 这条错误 release 触发器收回去
- 子层这轮新增的 authoritative 落点：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - `EnterVillage` 整段不再走 `YieldDaytimeResidentsToAutonomy`
     - `ReleasedFromDirectorCue && NeedsResidentReset` 改成：
       - 远离 `HomeAnchor` -> 先 `TryBeginResidentReturnHome(...)`
       - 不需要回家时才 autonomy
  2. [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - 新增 opening update-level guard，防止 `Update()` 再次短路 resident
     - 旧白天 release 守护已改成“先回家合同，不再立刻 autonomy”
- 当前父层判断：
  1. 这轮把 `spring-day1` 的第一主锅真正改到了代码上：
     - 不再由 `Day1` 把 opening crowd resident 从拥挤终点直接放飞到 autonomous roam
  2. 但 `NPC/导航` 侧的爆炸面还在：
     - 即使触发器被收掉，`NPCAutoRoamController` 的 fail-fast/缓存问题仍应继续交给对应线程
  3. 所以当前阶段是：
     - `spring-day1` 触发器已收一刀
     - 结构归因更完整了
     - live 体验还需要 fresh 票
- 当前验证摘要：
  - `validate_script` 两个本轮目标文件 `owned_errors=0`
  - 但都被 Unity 现场的 `8` 条外部 `Missing Script` 卡成 `external_red`
  - `manage_script validate SpringDay1NpcCrowdDirector` => `errors=0 warnings=2`
  - `manage_script validate SpringDay1DirectorStagingTests` => `errors=0 warnings=0`
  - `git diff --check` 覆盖本轮两文件通过
- 当前恢复点：
  1. 父层下一步仍不应该放行大扩题；先看 fresh opening live。
  2. 如果 live 仍不过线，子层继续只追 opening resident release，不准漂回 dinner / UI / Primary / 导航 core。

## 2026-04-15｜父层补记：用户 fresh 反证证明方向对了，但此前落地 claim 说早了
- 用户这次 fresh live 给出的两条证据，把父层判断重新钉得更硬：
  1. `CrowdDirector` 开着时，NPC 又回到了原地高频抖动
  2. 直接禁用 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 后：
     - 全部 NPC 恢复正常
     - 并且这次连回 anchor 都能自己做
- 父层因此必须明确更正：
  1. **方向判断是对的**
     - `CrowdDirector` 仍然越权，这条现在更稳
  2. **此前“release contract 已继续收平”的落地判断不成立**
     - 因为直到这轮之前，`CrowdDirector` 在 daytime 无显式 crowd beat 时，仍在 `Update()` 里持续碰 resident
     - 所以那时最多算“opening 触发器收了一刀”，不能算真正过线
- 子层这轮新增的 authoritative 修正：
  1. daytime autonomy 分支不再每轮反复触碰所有 resident
  2. 已经自治的 resident 和已经在回家的 resident，不再允许被 `CrowdDirector` 重写状态
  3. 新测试已经开始守这两条红线
- 当前父层判断：
  - 用户这次说“你现在根本就没修复”，从上一轮 claim 的角度看是成立的。
  - 更准确地说：
    - 不是完全没修
    - 而是之前修得不够，而且我把阶段判断说早了
  - 现在这轮才把第二个关键 runtime 干扰口真正收进代码。
- 当前恢复点：
  1. 父层下一步只看 fresh live，不再接受“只凭结构就说收平”。
  2. 如果这轮之后 still 只有“关掉 CrowdDirector 才正常”，子层下一刀必须继续把 `CrowdDirector` 削成 daytime 近乎 idle。

## 2026-04-15｜父层补记：当前 profiler 已把第一直接热点进一步压到 return-home 代驾循环
- 用户 fresh profiler 再次给出强证据：
  - `SpringDay1NpcCrowdDirector.Update()` self time 持续在 `75%+`
  - 同时伴随大量 `Physics2D.OverlapCircleAll` / `GC.Alloc`
- 子层这轮只读复核后的父层新判断：
  1. 当前最像直接热点的不是抽象的 “CrowdDirector 很重”，而是：
     `Update() -> TickResidentReturns() -> TickResidentReturnHome() -> TryDriveResidentReturnHome()`
  2. 也就是说，`CrowdDirector` 现在还在持续代驾 resident 回家，而不是只发一次“去 home/anchor”的语义。
  3. 这条链比 `daytime autonomy` 双遍历更贴近用户现在看到的：
     - 高频小幅抖动
     - 关掉 `CrowdDirector` 后 NPC 自己反而会回 anchor
- 父层当前恢复点：
  1. 下一刀如果继续，只应先削掉 `TickResidentReturnHome` 的重复驱动，不准先漂去别的议题。
  2. `HasPendingDaytimeAutonomyRelease` 双遍历仍算第二热点，但优先级下降到第二刀。

## 2026-04-15｜父层补记：opening handoff 退权与 return-home 节流这一刀已落
- 子层 `004_runtime收口与导演尾账` 本轮继续真实施工，只收 `CrowdDirector` 自己：
  - opening handoff 已 release latch 后，不再继续 `SyncCrowd()`
  - `TickResidentReturnHome()` 不再每帧重下发 `DriveResidentScriptedMoveTo(...)`
- 子层这轮新的 authoritative 落点：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - 新增 `ShouldStandDownAfterEnterVillageCrowdRelease(...)`
     - `ApplyResidentBaseline(...)` 在 opening handoff 已 latch 时，直接 `ReleaseResidentToAutonomousRoam(...)`，不再把 released resident 排进 crowd 自己的 `return-home owner`
     - `TickResidentReturnHome(...)` 只在 scripted move 丢失时才按 `0.35s` 重试，不再每帧重下发
  2. [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - opening release 测试已改成守“crowd 退权”
     - 新增 active scripted move 不重复下发的守护测试
- 父层当前判断：
  1. 这轮不再只是“分析正确”，而是把 `CrowdDirector` 的两个直接 runtime 写口一起收进了代码。
  2. 当前最准确的阶段口径是：
     - 代码层止血已落
     - 体验层仍待 fresh live
  3. 如果用户 fresh live 仍说“关掉 CrowdDirector 才正常”，下一刀仍继续只追 `CrowdDirector`，不扩 dinner / UI / Primary / 导航 core。
- 当前验证摘要：
  - `manage_script validate SpringDay1NpcCrowdDirector` => `errors=0 warnings=2`
  - `manage_script validate SpringDay1DirectorStagingTests` => `errors=0 warnings=0`
  - `validate_script SpringDay1NpcCrowdDirector.cs` => `external_red / owned_errors=0`
  - `validate_script SpringDay1DirectorStagingTests.cs` => 工具 blocker：`CodexCodeGuard returned no JSON`
  - `git diff --check` 覆盖这两文件通过
- 父层恢复点：
  1. fresh live 先只看：
     - `CrowdDirector` 开着时 resident 是否还高频抖动
     - `CrowdDirector` 开着时 resident 是否仍能自己回 anchor
     - `SpringDay1NpcCrowdDirector.Update()` 是否明显降温
  2. 如果 still 不过线，下一优先疑点保留：
     - `ShouldRecoverMissingTownResidents(...)`
     - `HasPendingDaytimeAutonomyRelease()` 双遍历

## 2026-04-15｜父层决策补记：下一阶段可切 dedicated refactor thread
- 用户已明确表达：不想再继续无限补丁，而是希望直接以完整清单交给新线程做正式重构。
- 父层当前判断：
  1. 这个方向现在已经成立。
  2. 之前没有直接切，是因为 direct refactor 的风险面还没被压窄；现在随着用户语义、红线、三方 prompt、以及 `CrowdDirector` exact 热链都已经清晰，重构前提基本够了。
  3. 下一阶段最合适的组织方式，不再是旧线程边补丁边猜，而是 dedicated refactor thread 单独 owning：
     - `SpringDay1Director` 的剧情语义收口
     - `SpringDay1NpcCrowdDirector` 的退权削薄
     - `NPC facade / Navigation contract` 的正式接线
- 父层建议：
  - 新线程不是“无边界全重写”，而是拿一份彻底修复清单，只做 `Day1 runtime decoupling` 这一条主线。

## 2026-04-15｜父层补记：正式总交接文档已建立，后续以文档而非长聊天交接
- 用户最新裁定：
  - 不要再继续写“唯一主刀 prompt”。
  - 要直接写一份正式总交接文档，把 `spring-day1` 这条线的历史语义、边界、现状问题、误修脉络、止血补丁定位、以及 dedicated refactor thread 的接班范围全部扯清。
- 子层 `004_runtime收口与导演尾账` 这轮已完成：
  1. 新增正式文档：
     - [2026-04-15_spring-day1_Day1历史语义_系统边界_现状问题_重构交接总文档.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/2026-04-15_spring-day1_Day1历史语义_系统边界_现状问题_重构交接总文档.md)
  2. 文档已经明确收进：
     - 用户最终语义基线
     - 正确职责切分
     - `SpringDay1Director` 与 `SpringDay1NpcCrowdDirector` 的越权点
     - fresh live / profiler 已钉死的事实
     - 历史误修时间线
     - 当前补丁为何只是止血
     - 接班线程该如何阅读和使用文档
     - 重构完成定义与必须遵守的红线
- 父层当前判断：
  1. 这份文件已经取代旧的 prompt 串联方式，成为后续接班重构的正式入口。
  2. 这轮站住的是“交接层和结构层已经彻底说清”，不是“runtime 体验已经闭环”。
  3. 后续如果继续推进，最正确的组织方式仍然是 dedicated refactor thread，但接班入口应是这份总交接文档，而不是继续翻长聊天。
- 当前恢复点：
  1. 父层后续如果再发续工，不应重复写同一份长正文，而应引用这份正式交接文档。
  2. 旧线程到这里可以正式停车，避免继续把“止血补丁现场”和“正式重构入口”混在一起。

## 2026-04-15｜父层补记：Day1-V3 已在 `100-重新开始` 建立独立接班审计入口
- 用户已明确改派新线程 `Day1-V3`，并明确要求：
  - 先从 `0` 开始吃透旧 prompt、旧 memory、旧交接文档与当前代码
  - 先分析“到底出了什么问题、为什么一直修出新问题、后续重构风险是什么”
  - 暂时不直接进入真实重构
- 本轮新入口与落点：
  1. 新子工作区：
     - `D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/100-重新开始/`
  2. 新线程记忆：
     - `D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/Day1-V3/memory_0.md`
  3. 首轮只读审计文档：
     - `D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/100-重新开始/2026-04-15_Day1-V3_接班诊断与重构风险审计.md`
- `Day1-V3` 这轮独立确认的新问题，不再直接沿用旧线程自述：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 `Update()` 在所谓 `stand-down` 前，仍无条件执行：
     - `SyncResidentNightRestSchedule()`
     - `TickResidentReturns()`
     说明 `CrowdDirector` 仍在继续持有剧情后 resident runtime owner。
  2. 同文件 `ApplyResidentBaseline(...)` 里仍保留：
     - `opening release latch -> direct autonomy`
     这与旧交接文档口头宣称的“先回 anchor/home 再 roam”并未真正统一。
  3. [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs) 仍有多条测试继续保护旧 owner 口径，例如：
     - `CrowdDirector_ShouldYieldOpeningResidentToAutonomyAfterEnterVillageCrowdReleaseLatches`
     - `CrowdDirector_ShouldResumeRoamAfterReturnHomeCompletes`
     - `CrowdDirector_ShouldNotReissueReturnHomeDriveWhileScriptedMoveIsAlreadyActive`
  4. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 也还没有退回纯语义层，仍继续深碰：
     - `003` 特殊释放
     - crowd cue settle / force sync
     - story actor runtime policy
- 父层当前新判断：
  1. 旧线程后期“方向越来越接近 owner 问题”这点成立。
  2. 但旧线程并没有真正把 owner 从代码和测试里拔干净，所以它反复出现的是：
     - 文档在纠偏
     - 代码在延续 stopgap
     - 测试在保护旧合同
  3. 因此这条线一直不是“完全没分析到”，而是“分析逐步变对，但始终没有统一单一真相”。
- 当前恢复点：
  1. 父层后续如果继续支持 `Day1-V3`，优先让它先做只读冻结表，而不是直接开重构。
  2. 真正施工前必须先统一：
     - opening handoff 唯一合同
     - `003` 是否继续特殊化
     - 哪些 staging tests 属于 stopgap、必须删或重写

## 2026-04-15｜父层补记：Day1-V3 已落 101 首刀，opening handoff 合同先统一
- `Day1-V3` 在完成 `100-重新开始` 的只读审计后，已新开子工作区：
  - `D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/101-opening-handoff统一合同/`
- 本轮真实施工只收一刀：
  - opening handoff 不再 `direct autonomy`
  - `003` 不再被导演从 opening 终点直接放回 roam
- 已落的代码口：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - `ApplyResidentBaseline(...)` 已去掉 opening release latch 后的 direct-autonomy 分支
     - 当前 opening handoff 先保持 `return-home` 合同；只有本来就在 home/anchor 附近时才落回 autonomy
  2. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - `ReleaseOpeningThirdResidentControlIfNeeded(...)` 已改成：
       - 放手 `003` 时不再直接恢复 roam
       - 立即 `ForceImmediateSync()`，交还给 crowd baseline 接手
  3. [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - opening handoff 测试已改成守“先 return-home”
     - `003` 测试已改成守“交回 crowd release contract”
- 当前验证摘要：
  - `manage_script validate SpringDay1NpcCrowdDirector` => `errors=0 warnings=2`
  - `manage_script validate SpringDay1Director` => `errors=0 warnings=3`
  - `manage_script validate SpringDay1DirectorStagingTests` => `errors=0 warnings=0`
  - fresh console => `errors=0 warnings=0`
  - `git diff --check` 覆盖 touched files 通过
  - 定向 EditMode 测试通过：
    - `CrowdDirector_ShouldQueueOpeningResidentReturnHomeAfterEnterVillageCrowdReleaseLatches`
    - `Director_ShouldYieldOpeningThirdResidentToCrowdReleaseContractWhenVillageGateHandsOff`
    - `CrowdDirector_ShouldKeepOpeningResidentsOnBaselineReleasePathDuringEnterVillage`
  - 整个 `SpringDay1DirectorStagingTests` 类仍有一批既有失败，但失败列表里不包含本轮这 3 条相关测试
  - `compile/no-red` 的 CLI 票仍被 `CodeGuard dotnet timeout` 卡住，因此当前不能宣称完整 Unity no-red 闭环
- 父层当前判断：
  1. 这轮已经把“文档说 opening 后先回 anchor/home，再 roam；代码却还 direct-autonomy”这条最硬冲突收平了。
  2. 但这轮不是完整退权：
     - `CrowdDirector` 仍在持有 `return-home` 这段 owner
     - `night schedule / 恢复链 / stopgap tests` 还没拆
  3. 当前最准确口径是：
     - opening handoff 合同第一刀已落
     - 结构继续向正确方向靠拢
     - 运行态整体闭环仍待后续验证
- 当前恢复点：
  1. 如果继续分析，下一轮优先补：
     - resident lifecycle 全入口冻结表
     - `003` 特殊化残留表
     - owner 级 stopgap tests 清单
  2. 如果继续施工，不要扩 dinner / 导航 core / Primary / UI，仍先沿 owner 清理线推进

## 2026-04-15｜父层补记：Day1-V3 已明确后续方向为“先冻结，再受控重构”
- 用户在看到 `101` 首刀后，进一步追问：
  - 我是不是其实已经有底气直接一轮彻底干到底
  - 冻结表是不是只是给用户看的
  - 我到底倾向“重构”还是“继续一步一步修”
- 父层当前明确口径：
  1. 冻结表不是展示层，它是后续真正开刀前的施工图。
  2. 当前不走“继续散修”。
  3. 当前也不走“无边界一轮梭哈式总重构”。
  4. 当前正确路线是：
     - `先冻结`
     - `再进入受控重构`
     - 后续按 owner 主线连续收
- 原因：
  1. `CrowdDirector` owner、`003` 特殊化、night schedule、恢复链、owner 级 stopgap tests 仍在同一团里。
  2. 现在直接一轮硬梭哈，风险不是做不动，而是容易误伤：
     - `001/002 house lead`
     - `Primary`
     - 当前还算稳定的执行层
  3. 但如果继续零散补丁，又会回到旧 `day1` 的路径依赖。
- 当前恢复点：
  1. 如果继续，只应先补冻结表，不再新增散修刀。
  2. 冻结表完成后，下一阶段应按受控重构继续，而不是重新回到“每轮补一处”。

## 2026-04-15｜父层补记：Day1-V3 已把 102 收成“全量总表 + 跨线程边界文件”，不再只是这一刀冻结
- 用户明确纠正：
  - 不要只写“这一刀的 owner 冻结”
  - 要写 `Day1` 现存所有 open 问题的总览、整体施工方向、以及给导航/NPC 的边界同步
- `Day1-V3` 这轮新的 authoritative 落点：
  1. 新增总表：
     - [2026-04-15_Day1-V3_Day1现存问题总览与整体施工总表.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_Day1现存问题总览与整体施工总表.md)
  2. 新增导航同步 contract：
     - [2026-04-15_Day1-V3_给导航_语义同步与执行边界contract.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_给导航_语义同步与执行边界contract.md)
  3. 新增 NPC 协作 prompt：
     - [2026-04-15_Day1-V3_给NPC_协作边界与facade落地prompt.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_给NPC_协作边界与facade落地prompt.md)
  4. 另外已补成 3 份可直接转发的引导文本：
     - [给 Day1-V3 自己的续工引导 prompt](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_给Day1-V3_整体推进分阶段续工引导prompt_01.md)
     - [给导航的弱引导 prompt](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_给导航_Day1-V3语义同步与执行边界弱引导prompt_01.md)
     - [给 NPC 的弱引导 prompt](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_给NPC_Day1-V3协作边界与facade弱引导prompt_01.md)
- 父层当前新判断：
  1. `Day1-V3` 当前不该继续自己吞 NPC facade；NPC 线程已经有足够成熟的 contract 草案，应由它主导 facade 落地。
  2. `导航` 线程需要同步到新的 Day1-V3 方向：只负责 movement execution，不再回吞 Day1 phase。
  3. 当前“只要 NPC 发生真实位移，原则上都应走统一导航执行合同”应成为后续三方协作的新硬基线。
- 当前恢复点：
  1. 如果父层下一步继续协调，优先先发导航/NPC 两份同步文档，而不是直接开第二刀大改。
  2. 如果不等外部回执，`Day1-V3` 自己的下一刀仍应是 `Package B｜Day1 自己退权`。

## 2026-04-15｜父层补记：NPC façade 第一刀已真实落地，Day1 后续可按新合同继续退权
- 当前父层新真值：
  - `NPC` 线程已经把 locomotion 对外合同面从草案推进成真实代码，不再只是边界讨论。
- 已落地的父层关键点：
  1. `NPCAutoRoamController` 已出现语义级 façade：
     - `AcquireStoryControl / ReleaseStoryControl`
     - `RequestStageTravel / RequestReturnToAnchor / RequestReturnHome`
     - `SnapToTarget`
     - `BeginAutonomousTravel / BeginReturnHome / ResumeAutonomousRoam / AbortAndReplan`
  2. `Day1` 自己最脏的 resident scripted 直调位已经被迁到 façade：
     - `SpringDay1Director.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - `SpringDay1DirectorStaging.cs`
  3. NPC own 的聊天冻结链也同步吃了 façade，因此这层不再是 `Day1` / `NPC` 两边各写一套的旧状态。
- 对父层排程的影响：
  1. `Day1-V3` 后续继续做 `Package B｜Day1 自己退权` 时，可以默认以这套 façade 为消费面，不需要再反复讨论“NPC 那边 façade 到底有没有做”。
  2. 真正还没闭完的，不是编译或 console，而是 Unity `test_names` 过滤层的 targeted test 发现问题；这应被视为验证缺口，不应被误判成 façade 没落地。

## 2026-04-15｜父层补记：Day1-V3 已把 night/recover 尾巴继续压到 facade 面
- `Day1-V3` 在 `104_packageB_night_recover_deownership` 下继续真实施工，并已 `Park-Slice`。
- 父层新进展：
  1. `SpringDay1NpcCrowdDirector` 的 `recover missing residents` 暗门已改成“只恢复 Day1 视图，不重启 NPC 身体”。
  2. `night-rest` 已不再由 `CrowdDirector` 深持 owner；进入夜间 resting 时改成释放 shared owner 后走 `SnapToTarget(...)`。
  3. `CrowdDirector` 内剩余的 restart 尾巴已继续收口到 NPC facade：
     - 不再直接使用 `StartRoam`
     - 不再保留 `ForceRestartResidentRoam`
     - release / baseline / cancel 后统一通过 `ResumeAutonomousRoam(...)`
  4. `return-home` 合同完成但不续 roam 时，现在也会明确放掉 shared owner，不再留下 crowd 残持有。
- 验证：
  1. `SpringDay1DirectorStagingTests` 新增 3 条贴刀口测试并通过：
     - `recover` 不再顺手 restart
     - `night-rest` 不再残留 crowd owner
     - `return-home complete without resume` 会释放 owner
  2. 与上轮 package-B 的 6 条既有定向测试一起，direct MCP EditMode 9/9 passed
  3. fresh console 最终=`errors=0 warnings=0`
- 父层当前判断：
  1. `Day1-V3` 现在已经不只是 opening handoff 退权，而是把 `recover/night-rest/restart-tail` 这一整圈最脏尾巴也压到了新 facade 面。
  2. 下一步真正的结构分水岭是：
     - `20:00 return-home` 这段 schedule owner 是否继续留在 Day1 私房 runtime。
     - 这已经不是简单尾巴清理，而是共享 schedule / navigation contract 的切层问题。

## 2026-04-15｜父层补记：Day1-V3 已把 clock return-home 的两条脏尾巴收掉
- `Day1-V3` 在 `105_packageB_clock_return_owner_cut` 下继续真实施工，并已 `Park-Slice`。
- 父层新进展：
  1. `20:00 return-home` 现在明确是“回家并停住”，不再在到家后自动恢复 roam。
  2. `return-home` snapshot 不再持久化，也不再跨场景恢复旧 owner。
  3. 因此 Day1 当前已经从：
     - opening/daytime release 的 owner
     - recover/night-rest/restart-tail
     - clock-return 的持久化尾巴
     这三层都继续后撤了一步。
- 验证：
  1. 新增 2 条 `clock-owned return-home` 测试通过：
     - 忽略旧 snapshot
     - 到家不续 roam
  2. snapshot 相关旧回归 3 条继续通过
  3. fresh console 最终=`errors=0 warnings=0`
- 父层当前判断：
  1. 下一步真正剩下的核心，不再是 snapshot 或到家后的尾巴，而是 `TryDriveResidentReturnHome / TickResidentReturnHome` 这条执行链本身是否还该由 Day1 拿着 owner。
  2. 这会开始逼近“shared schedule / navigation contract”那一层，风险比这轮更高，应继续按单刀推进。

## 2026-04-15｜父层只读结论：opening / dinner staged 坏相的第一真责任层在导演 reset/latch 耦合，不在 crowd resident cue owner
- 新增父层判断：
  1. opening 与 dinner 这次用户报的坏相不是两套无关 bug，而是同一种结构问题：
     - 对白前摆位已经完成
     - 但对白活跃期又把 `placed latch` 清零
     - 下一帧再按“第一次摆位”重跑 start -> end
  2. 对 `001~003 / 001/002` 而言，第一责任层在：
     - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - 不是 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 resident cue 应用层
  3. 这说明如果后续要起 dedicated refactor thread，`director staged contract` 仍然是必须单独收的一层，不能把所有坏相都粗暴并进 `CrowdDirector/Navigation`。
- 父层最小修法建议：
  1. `opening`：拆 `TownVillageGate` 的 wait reset 与 actor placed reset
  2. `dinner`：拆 `DinnerCue` 的 wait reset 与 story actor placed reset，并在 dinner dialogue 已 queue/active 后停止重复 staging
- 父层测试缺口：
  1. `opening` 缺“`_villageGateSequencePlayed=true` 且 dialogue active 时仍应保持 end pose”的回归
  2. `dinner` 缺“`_dinnerSequencePlayed=true` 或 dinner dialogue active 后再次 tick，不应重新回 start”的回归

## 2026-04-15｜父层只读结论：Primary 时间冻结与 0.0.6 回 Town 后 resident 冻结是两条 runtime contract 缺口
- 新增父层判断：
  1. `Primary` 时间一进入就停止流逝，第一责任层在 [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的时间暂停链：
     - `SyncStoryTimePauseState() -> ShouldPauseStoryTimeForCurrentPhase() -> ShouldKeepStoryTimeRunningForRuntimeBridge()`
     - 当前 working tree 下 `StoryPhase.EnterVillage` 仍以 `IsTownHouseLeadPending()` 作为唯一放行条件
     - 而 `IsTownHouseLeadPending()` 又强依赖 `IsTownSceneActive()`
     - 所以玩家一切到 `Primary`，即使仍处于 `EnterVillage` 承接窗口，条件也立刻失效，`PauseTime("SpringDay1Director")` 会重新压住时间
  2. `0.0.6` 回 `Town` 后 `003` 原地卡死，第一责任层是 `spring-day1-director` owner 在 scene continuity 里被原样带回，但导演后续没有对 `003` 对称释放：
     - `TryPrepareTownVillageGateActors() / TryDriveEscortActor() / ReframeStoryActor()` 会给 `003` 取得 `DirectorResidentControlOwnerKey`
     - `PersistentPlayerSceneBridge -> NpcResidentRuntimeContract -> NPCAutoRoamController.ApplyResidentRuntimeSnapshot()` 恢复 native resident snapshot 时，如果 `snapshot.scriptedControlActive=true` 会直接重新 `AcquireResidentScriptedControl(...)` 并提前返回
     - `UpdateSceneStoryNpcVisibility()` 只对 `001/002` 调 `ApplyStoryActorRuntimePolicy(...)`，没有对 `003` 做对应 release，因此 `003` 比 `001/002` 更容易带着旧 owner 回到 `Town`
  3. `101~203` dinner 前冻结的第一真责任层更像 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 crowd continuity / release contract 缺少明确“post-tutorial explore 已退权”语义：
     - `EnterPostTutorialExploreWindow()` 只把 `_postTutorialExploreWindowEntered=true`
     - 但 `GetCurrentBeatKey()` 仍把该窗口继续归到 `FarmingTutorial_Fieldwork`
     - crowd snapshot 恢复时只要 `underDirectorCue=true`，`ApplyResidentRuntimeSnapshotToState()` 就会重新 `AcquireResidentDirectorControl(...)`，并把 `ReleasedFromDirectorCue / NeedsResidentReset` 都压回 `false`
     - 之后 `SyncCrowd()` 只有在 `ApplyResidentBaseline()` 真正走到 release 分支时才会恢复 daytime baseline 或 autonomous roam；如果 beat/phase 语义仍像 tutorial fieldwork，就容易继续挂在旧 contract 上
- 父层最小修法建议：
  1. 时间冻结先只改 `EnterVillage` 的时间暂停判定，不要再用“切到 `Primary` 就天然不放行”的条件；至少要让 `TownHouseLead` 结束到下个正式 phase 之间不再被 `PauseTime` 卡死
  2. `003` 先补一个最小 release 点：
     - 要么在 opening handoff 结束 / `Town` re-entry 后显式释放 `003` 的 `spring-day1-director`
     - 要么在 native resident continuity 恢复时，对“已晚于 opening 语义”的 snapshot 不再原样恢复 scripted owner
  3. `101~203` 先补一个明确的 post-tutorial explore / `Town` re-entry 退权语义：
     - 最小可行是让 `GetCurrentBeatKey()` 在 `_postTutorialExploreWindowEntered && IsTownSceneActive()` 下不再继续返回 `FarmingTutorial_Fieldwork`
     - 或在 crowd snapshot 恢复后第一轮 `SyncCrowd()` 明确清掉 restored cue placeholder，再走 baseline release
- 父层测试缺口：
  1. `EnterVillage + Primary` 承接时，若 `IsTownHouseLeadPending()==false` 但剧情仍处 bridge window，断言 `TimeManager` 不被 `SpringDay1Director` 暂停
  2. `003` 带着 `spring-day1-director` owner 离开 `Town` 再回 `Town` 后，断言 owner 已释放或至少已恢复 roam，而不是继续 `IsResidentScriptedControlActive=true`
  3. crowd snapshot 带 `underDirectorCue=true` 且 `_postTutorialExploreWindowEntered=true` 的 `Town` re-entry 后，第一轮 `SyncCrowd()` 断言 `101~203` 不再残留 restored cue placeholder / director owner

## 2026-04-15｜父层补记：Day1-V3 已把 opening/dinner reset 回跳、Primary 时间冻结与白天 re-sync 坏相收成可验 checkpoint
- `Day1-V3` 在 `106_live-fix_opening-primary-dinner-regressions` 下继续真实施工，并已 `Park-Slice`。
- 父层新进展：
  1. `opening` 的 `001~003` 不再因为 `VillageGate` dialogue active 而被导演重置回起点。
  2. `dinner` 的 `001/002` 不再因为 `placed latch` 被清空而重复从 start 重走；晚饭 timeout 现在也和 opening 一样，settle 不完也不再无限等。
  3. `Primary` 里的 `Healing / Workbench / FarmingTutorial` 不再冻结 Day1 时钟；白天上限也从 `16:59` 收成 `16:00`。
  4. `_postTutorialExploreWindowEntered` 成立后，`GetCurrentBeatKey()` 不再继续把 `Town` resident 归到 `FarmingTutorial_Fieldwork`。
  5. `SpringDay1NpcCrowdDirector` 的白天 yield 早退口新增 re-sync 保护：
     - `_syncRequested=true`
     - 或 `_spawnStates` 有 stale/null shell
     就必须先 `SyncCrowd()`，不能直接因为“没有 pending release”而 return。
  6. `21:00+` 的 helper 语义已和 `20:00 return-home` 拆开；`rest` 不再被当成 `return-home contract` 的延长期。
- 父层验证：
  1. `SpringDay1Director / SpringDay1NpcCrowdDirector / SpringDay1DirectorStagingTests` 的脚本级 validate 均 `owned_errors=0`
  2. 定向 EditMode 15/15 passed：
     - staging 单测 11 条
     - opening/dinner runtime bridge 4 条
  3. fresh console 最终=`errors=0 warnings=0`
- 父层当前判断：
  1. 这轮已经把用户最新 live 指出的第一层 runtime regressions 拉回来了。
  2. 但 `20:00 -> 到家` 的 formal-navigation arrival owner 仍在 Day1/CrowdDirector 收尾链里，还不能包装成“最终完全退权”。
  3. 因此当前正确阶段是：
     - live 回归 checkpoint 已成
     - 下一轮才值得继续评估 `TickResidentReturnHome` 是否还能往共享 schedule / navigation 合同后撤。
## 2026-04-15｜父层补记：Day1-V3 已把 latest live follow-up 继续收成 opening/dinner/0.0.6 三点修正
- `Day1-V3` 在 `107_live-fix_opening-dinner-freeze-followups` 下继续真实施工，当前适合先停给用户 fresh live retest。
- 父层新增进展：
  1. `opening`
     - `MaintainTownVillageGateActorsWhileDialogueActive()` 改成对白开始后直接钉终点，不再允许 `001~003` 回起点重播。
  2. `dinner`
     - `TryResolveDinnerStoryActorRoute(...)` 先吃场景 `进村围观` 的 authored 起终点 markers，再退回 home-anchor fallback。
     - 这条直接对准 `001/002` 晚饭前走去错误位置的问题。
  3. `0.0.6 free window`
     - `ShouldYieldDaytimeResidentsToAutonomy()` 不再把 `FreeTime_NightWitness` 当成持续 hold 的例外。
     - 这条直接对准回 `Town` 后 resident 被 crowd runtime 卡成不动的问题。
- 父层验证：
  1. direct MCP `validate_script`
     - `SpringDay1Director.cs` => `errors=0`
     - `SpringDay1NpcCrowdDirector.cs` => `errors=0`
     - `SpringDay1DirectorStagingTests.cs` => `errors=0`
  2. direct MCP EditMode `7/7 passed`：
     - `Director_ShouldPreferVillageCrowdSceneMarkersForDinnerStoryRoute`
     - `CrowdDirector_ShouldYieldAutonomyDuringNightWitnessFreeWindow`
     - `Director_ShouldExposeFreeRoamBeatDuringPostTutorialExploreWindow`
     - `Director_ResetDinnerCueWaitState_ShouldPreservePlacedStoryActors`
     - `CrowdDirector_ShouldResyncRecoveredResidentsBeforeSkippingDaytimeYield`
     - `CrowdDirector_ShouldResolveDinnerCueFromDinnerBeatInsteadOfOpeningMirror`
     - `TownVillageGateDialogueActive_ShouldKeepStoryActorsAligned`
  3. fresh console 最终=`errors=0 warnings=0`
- 父层当前判断：
  1. 这轮是继续贴用户最新 live 真相补刀，不是继续扩 `20:00+`。
  2. 下一步最合理的是先等用户 fresh live retest：
     - `opening` 是否还会回起点
     - `0.0.6` 回 `Town` 后 resident 是否恢复正常
     - `dinner` 的 `001/002` 是否还会乱走或重走
## 2026-04-16｜父层补记：`按 E 一次就解放单个 NPC` 的只读根因判断
- `Day1-V3` 本轮按用户要求只做只读彻查，不改代码。
- 父层新增判断：
  1. 聊天系统不是根因本体，但它确实给单个 NPC 跑了一次局部 locomotion reset。
  2. `NPCDialogueInteractable / NPCInformalChatInteractable` 会对 `NPCAutoRoamController` 调：
     - `AcquireStoryControl`
     - `HaltStoryControlMotion`
     - `ReleaseStoryControl`
     - `ApplyIdleFacing`
     - 无 `ResumeAutonomousRoam`
  3. `PlayerNpcChatSessionService` 不直接碰上述 locomotion API；它只是开始/结束会话并间接触发 `ExitConversationOccupation()`。
  4. `ReleaseStoryControl(...)` 最终进入 `ReleaseResidentScriptedControl(...)`；当 owner 清空、允许 resume 且不在对白暂停态时，会直接 `StartRoam()`。
  5. 因此用户看到的“聊一下就恢复”，本质上是：
     - 这个 NPC 被单独做了一次 `HaltStoryControlMotion + ReleaseStoryControl(+ StartRoam)` 的 reset/restart
  6. 这条现象反而说明当前更像 `NPCAutoRoamController` 的 per-NPC runtime 坏态，而不太像“还被 Day1 owner 持续抓着不放”，因为 informal chat 本身就禁止对 `IsResidentScriptedControlActive == true` 的目标开启。
- 父层恢复点：
  1. 后续真施工时，优先继续追 `0.0.6` 回 Town 后 resident 是怎么被交回坏掉的 runtime 状态的。
  2. 重点继续看：
     - `SpringDay1NpcCrowdDirector.ApplyResidentBaseline(...)`
     - `SpringDay1NpcCrowdDirector.ReleaseResidentToAutonomousRoam(...)`
     - `ResumeResidentAutonomousRoam(...)`
     - `NPCAutoRoamController` 的 move/repath/recovery/detour 残留
## 2026-04-16｜父层补记：`0.0.6` Town 卡爆当前最像“crowd 批量放手 + roam 批量重启”风暴
- `Day1-V3` 本轮继续只读，不改代码。
- 父层新判断：
  1. `0.0.6` 回 Town 的 severe 卡爆，当前最像 herd-level 的 runtime 风暴，不像单个 lingering owner 就能解释完。
  2. `SpringDay1NpcCrowdDirector` 在白天 free window 里会成批走：
     - `YieldDaytimeResidentsToAutonomy()`
     - `ReleaseResidentToAutonomousRoam(...)`
     - `TryReleaseResidentToDaytimeBaseline(...)`
  3. 这两条 release path 最终都会把 resident 推向 `ResumeAutonomousRoam(..., tryImmediateMove: true)`。
  4. 但 `NPCAutoRoamController.ResumeAutonomousRoam(true)` 在 `!IsRoaming` 时只会 `StartRoam()`，而 `StartRoam()` 又固定先进 `EnterShortPause(false)`。
  5. 所以 `opening` 结束后“先卡一下再继续走”与 `0.0.6` Town 回来后“全体严重卡爆”，很可能是同一个 release/restart 设计在不同规模下的两种坏相：
     - 小规模 = 一拍停顿
     - 大规模 = 群体同时建路/共享避障/blocked recovery，直接把 `NPCAutoRoamController` 推进高成本循环
  6. 另一个必须并列看的状态机坏链是：
     - `IsResidentScriptedControlActive && !IsResidentScriptedMoveActive`
     - `NPCAutoRoamController.Update()/FixedUpdate()` 会在这条支路里每帧重复 `ApplyResidentRuntimeFreeze()`
     - 而 snapshot restore / formal navigation end / crowd release 都可能把 resident 留在这条坏态上
  7. `TryAcquirePathBuildBudget()` 目前只限制“单 NPC 每帧一次建路”，不限制“整群 NPC 同帧同时建路”，所以 crowd 一口气放手几十个 resident 时，保护是不够的。
- 父层当前最小修法方向：
  1. 先拆 `CrowdDirector` 的“批量放手 + 同帧立即重启 roam”。
  2. 再补 `NPCAutoRoamController.ResumeAutonomousRoam(true)` 的 immediate 语义，让 `!IsRoaming` 时不再必吃一拍 `ShortPause`。
  3. 并加一层护栏，避免 resident 长时间停在 `scripted active but no move contract`。
## 2026-04-16｜父层补记：`0.0.6` 回 Town 卡爆已落第一轮真实修复 checkpoint
- `Day1-V3` 已从只读进入真实施工，但当前仍只收 `0.0.6` 这条主痛点，不扩到别的体验问题。
- 父层新增真实改动：
  1. `SpringDay1NpcCrowdDirector.ApplyResidentRuntimeSnapshotToState(...)`
     - neutral snapshot restore 后不再只恢复位置/朝向；现在会清 shared runtime owner，再补一次 `ResumeAutonomousRoam(tryImmediateMove: false)`。
     - 白天 autonomy window 下，`snapshot.underDirectorCue` 现在会被视为 stale cue，不再恢复导演 owner。
  2. `SpringDay1NpcCrowdDirector.TryRecoverReleasedResidentState(...)`
     - recover released resident 后不再只补 tracked state；会补一次 `ResumeAutonomousRoam(tryImmediateMove: false)`，避免 NPC 保留坏掉的 roam runtime。
  3. crowd free-time release / baseline / cancel-return-home 的自治恢复口，统一改成 `tryImmediateMove: false`，降低 `0.0.6` 回 Town 时整群 resident 同帧建路风暴。
  4. `SpringDay1DirectorStagingTests.cs`
     - 改写并新增三条贴刀口测试，专门锁：
       - released resident recover 后必须真正重启自治 roam
       - neutral crowd snapshot restore 后必须真正重启自治 roam
       - stale director cue 在白天窗口不得回抓
- 父层验证：
  1. CLI `validate_script`
     - `SpringDay1NpcCrowdDirector.cs` => `assessment=no_red`
     - `SpringDay1DirectorStagingTests.cs` => `assessment=no_red`
  2. direct EditMode `8/8 passed`
     - `CrowdDirector_ShouldIgnoreStaleDirectorCueSnapshotDuringDaytimeAutonomyWindow`
     - `CrowdDirector_ShouldRecoverReleasedResidentsIntoTrackedStateAndRestartAutonomousRoam`
     - `CrowdDirector_ShouldRestartAutonomousRoamWhenApplyingNeutralResidentRuntimeSnapshot`
     - 以及 5 条原有 snapshot / free-window / yield 回归
  3. fresh console 最终 `0 error / 0 warning`
- 父层当前判断：
  1. 这轮已经把 `0.0.6` severe 卡爆里最关键的三条回流口都收了：
     - bad-state restore
     - stale cue 回抓
     - batch immediate path build 放大器
  2. 但这仍不是用户 live 终验；下一步最合理的是 fresh retest：
     - `0.0.6` 回 Town 后 `003~203` 是否仍 severe 卡爆
     - 单个 NPC 是否还需要靠聊天 reset 才恢复
     - opening 结束后是否仍有明显小卡顿
## 2026-04-16｜Day1-V3 追加：Town 自由时段卡顿这轮先收 NPC locomotion 性能卫生，不再误判成 CrowdDirector 独锅
- 子工作区 `100-重新开始` 已新增一条 `109_town_free_time_roam_overlapcircle_rootcause` 真实施工记录。
- 父层需要知道的最小事实：
  1. 用户 latest profiler 已把主热点转到 `NPCAutoRoamController.Update()`，`SpringDay1NpcCrowdDirector.Update()` 只剩轻量背景，不再是唯一根。
  2. 这轮没有继续改 Day1 phase/release 语义，而是先收了 NPC locomotion 自己的性能卫生：
     - `NPCAutoRoamController`：soft-pass 节流 + autonomous cross-frame reuse
     - `NavigationAgentRegistry`：per-frame snapshot cache
  3. targeted EditMode `7/7 passed`，fresh console 已清；但用户 live 体感仍待下一次复跑确认。
- 当前阶段判断：
  - 这轮是“性能根因转向后的第一轮真实收刀”，不是整条 Day1 语义线的终验。
  - 如果 live 仍卡，下一刀优先继续看 `AdjustDirectionByStaticColliders / ProbeStaticObstacleHits`，而不是回头再把锅一股脑甩给 CrowdDirector。
- 当前 thread-state：
  - `Day1-V3 = PARKED`
  - blocker=`fresh-live-retest-required-for-town-free-time-performance-and-day1-runtime-behavior`
## 2026-04-16｜Day1-V3 只读审计补记：导航V3反思/方案的独立复核结果
- 本轮没有改代码，只做跨线程审计。
- 结论收口：
  1. 导航对“Day1 主触发 + 导航主烧点”的责任划分，当前仍基本成立。
  2. 但当前 `0.0.6` / Town 返场问题，已经不能继续简化成旧版“`ResumeAutonomousRoam(true)` 群体立即建路风暴”：
     - Day1 free-time 主路径多数已改成 `tryImmediateMove:false`
     - 更准确的是 batch resume 后，NPC 在同一短窗内集中进入 `StartRoam -> ShortPause -> TryBeginMove`
  3. 转场黑屏偏长仍有 Day1/Crowd own 成本：
     - `RecoverReleasedTownResidentsWithoutSpawnStates` / `TryRecoverReleasedResidentState`
     - `FindSceneResidentHomeAnchor -> FindAnchor -> GameObject.Find`
  4. 导航给出的下一刀方向可以做，但必须收窄成“scene-entry herd-start 短窗治理”，不能再包装成把坏点/static/self-recovery 整个导航包重新做一遍。
- 对后续协作的约束：
  1. Day1 侧继续 own scene-entry 的批量补绑、批量放手、stale cue/runtime continuity。
  2. 导航侧只 own batch resume 之后的执行层降载与坏启动短路，不回吞剧情 phase。
## 2026-04-16｜Day1-V3 汇报补记：当前最该继续砍 Day1 own 的 Town 返场入口链
- 当前统一判断：
  1. 先不扩大到导航全包；Day1-V3 下一刀优先是 `scene-entry / Town 返场补绑 / 白天 batch release`。
  2. 语义仍未完全同步到代码，尤其是：
     - opening / dinner 统一的一次走位合同
     - Primary 时间正常流逝到 16:00 再锁
     - 白天 resident 不回 anchor、只在 20:00+ 进入回家/休息
     - `0.0.6` 回 Town 的黑屏与群体卡顿终验

## 2026-04-16｜Day1-V3 追加：晚饭 staged contract 已从 mixed dinner logic 改成单次起终点合同
- 父层需要知道的最小事实：
  1. 用户把晚饭语义重新钉死为 opening 同规格：
     - 全员先到起点
     - 再只走一次到终点
     - 最多 5 秒
     - 超时 snap
     - 对话开始后不再 roam / 不再二次走位
  2. 这轮已把晚饭里最脏的两类旧逻辑收掉：
     - `SpringDay1Director` 只给 `001/002` 特供的 dinner gather 链
     - `SpringDay1NpcCrowdDirector` 因共享 `DinnerBackgroundRoot` semantic anchor 导致的 cue 串号
  3. 当前具体结构变化：
     - `003` 已并入 dinner story-actor staged contract
     - 无显式 dinner cue 的 resident，会走 `DirectorReady_* -> ResidentSlot_*` 的 runtime dinner fallback cue，而不是退回共享锚点乱排
  4. targeted EditMode `4/4 passed`，fresh console 已清，`git diff --check` 已过。
- 当前阶段判断：
  - 这轮成立到“晚饭入口结构已重写并有回归护栏”，还不是用户 live 终验。
- 当前恢复点：
  - 下一步应优先让用户 fresh retest 晚饭进入瞬间的排队/二次走位问题；`20:00` 回家链仍未处理。
## 2026-04-16 只读彻查：晚饭入口新阻塞定位
- 用户最新 live：进入晚饭后，NPC 被传送到错误位置，剧情不开始，NPC 也不动。
- 结论：这是 Day1-V3 刚刚那轮晚饭半重写带出的新阻塞，不是用户语义变化，也不是单纯导航问题。
- 关键代码链：SpringDay1Director.ActivateDinnerGatheringOnTownScene() -> AlignTownDinnerGatheringActorsAndPlayer()（仅移动玩家） -> BeginDinnerConflict() -> PrepareDinnerStoryActorsForDialogue()（只处理 001/002/003） + ResolveDinnerCueSettledState()（等待 DinnerConflict_Table 的 crowd cue settled）。
- Crowd 侧新增的 dinner fallback cue 已会把没有显式 cue 的 resident 送去 DirectorReady_* / ResidentSlot_* / DinnerBackgroundRoot 相关锚点；导演层却没有同步改成“全员统一 staged 入口 + 全员统一 settle gate”，因此出现传错位但不开戏的半状态。
- 下一刀必须直接重写晚饭入口，不再保留 story actor 特化准备链作为开戏前置。
## 2026-04-16 晚饭入口统一 staged contract 第一刀完成
- 已完成：晚饭 marker 根优先级修正、DinnerConflict_Table 启用 scene marker override、晚饭入口强制同窗 crowd sync。
- 已更新测试：SpringDay1DirectorStagingTests、SpringDay1LateDayRuntimeTests 里的晚饭断言已经切到“起点 -> 终点”新语义。
- 验证结果：晚饭相关 targeted staging tests 5 条通过；late-day runtime tests 4 条通过；validate_script(no_red)；console 0 error / 0 warning。
- 下一刀仍是 20:00/21:00 resident owner 退权，不在本轮展开。
## 2026-04-16 只读排查：Day1 抽象中间层污染确认
- 用户裁定：`进村围观` 和 `001~203_HomeAnchor` 才是 Day1 原始真值；`Town_Day1Residents` / `Town_Day1Carriers` / `DirectorReady_*` / `ResidentSlot_*` / `DinnerBackgroundRoot` / `DailyStand_*` 应退场。
- 查明依赖范围：SpringDay1NpcCrowdDirector runtime、SpringDay1NpcCrowdManifest.asset、SpringDay1DirectorStageBook.json、SpringDay1TownAnchorContract.json、Town.unity、多条 tests 都还在消费这套抽象中间层。
- 结论：语义上可以删、也应该删；工程上必须先迁代码/数据/测试，再删 scene 节点。
## 2026-04-16｜父层补记：彻底删除抽象锚点体系前还要补切的容易漏项
- 用户要求把“彻底删除 `Town_Day1Residents / Town_Day1Carriers / DirectorReady_* / ResidentSlot_* / DinnerBackgroundRoot / DailyStand_* / NightWitness_01`”前的仓库炸点盘清。
- 父层需要知道的新增点：
  1. 除了 runtime 和 tests，`Assets/Editor/Town/*` 与 `Assets/Editor/NPC/*` 里还有一批菜单/探针/迁移脚本把这些抽象锚点当 readiness/migration/validation 真值；不先处理，删完 scene 节点后工具会持续报假红。
  2. `SceneTransitionTrigger2D.TownOpeningEntryAnchorName = "EnterVillageCrowdRoot"` 是 opening 入口常量级炸点；如果不先改，`Primary -> Town` 默认 entry 仍会落回旧抽象锚点。
  3. `SpringDay1Director` 里 `PreferredDinnerGatheringAnchorObjectNames = { "DirectorReady_DinnerBackgroundRoot", "DinnerBackgroundRoot" }` 仍是晚饭入口的残留硬依赖。
  4. `NpcCharacterRegistry.asset` 虽不直接存 anchor 名，但继续显式消费 `EnterVillage_PostEntry / DinnerConflict_Table / FreeTime_NightWitness / DailyStand_Preview` 这组旧 beat 语义；后续若把 late-day beats 一并收掉，需要同步评估它。
- 当前最小安全删除顺序更新为：
  1. 先切代码入口常量与 runtime fallback
  2. 再切 manifest / stagebook / town-anchor-contract（必要时补 character-registry）
  3. 再切 editor probe / migration / validation 工具与 tests
  4. 最后删 Town scene 抽象节点
## 2026-04-16｜父层补记：Town 场景误删的 101~203 已恢复，不回滚旧抽象根
- 子线在 `112_remove_day1_abstract_anchor_layers` 里修复了一次现场误删：
  - 前一刀删 `Town_Day1Residents / Town_Day1Carriers` 时把 `101~203` 一并删掉了。
- 本轮已恢复：
  1. `NPCs/101~203`
  2. `NPCs/101_HomeAnchor~203_HomeAnchor`
  3. 各自 `NPCAutoRoamController.homeAnchor` 重新绑定到对应 scene anchor
- 本轮明确保持不回流的旧结构：
  - `Town_Day1Residents`
  - `Town_Day1Carriers`
  - `EnterVillageCrowdRoot`
- 验证摘要：
  1. targeted EditMode `9/9 passed`
  2. `git diff --check -- Assets/000_Scenes/Town.unity` 通过
  3. fresh console=`0 error / 0 warning`
- 父层恢复点：
  - 现在可以继续按“只保留 `进村围观` 与 `001~203_HomeAnchor` 真值”的方向往下收，不需要再为 scene resident 缺席做额外兜底。
- 当前线程状态：
  - `Day1-V3 = PARKED`
  - blocker=`checkpoint-112-town-residents-restored-ready-for-next-runtime-cleanup`

## 2026-04-16｜父层补记：用户最新 5 问的代码真相已钉死
- 用户本轮不是要我继续开刀，而是要求把 5 个关键语义问题和现码真相彻底对清：
  1. `opening` 后 NPC 干什么
  2. 艾拉桥接距离多近
  3. `0.0.6` 回 `Town` 时 `001/002` 站哪
  4. `19:00` 提醒时段是否已经算剧情结束
  5. `001~003` 的 home anchor 与夜间回家链
- 父层需要知道的新增结论：
  1. 现码已经是“白天 resident 不回 anchor，20:00 后再回家”的大方向：
     - `SpringDay1NpcCrowdDirector.ShouldYieldDaytimeResidentsToAutonomy()` 明确在 `EnterVillage` 结束后、且不处于 `DinnerConflict_Table / ReturnAndReminder_WalkBack / DayEndSettle / DailyStandPreview` 时放普通 resident 回白天自治。
  2. `001/002` 仍是导演自己的 story actor：
     - 它们在 Town 的 story actor 模式只覆盖 `EnterVillage / DinnerConflict / ReturnAndReminder`
     - 在 Healing / Workbench / FarmingTutorial 主要活在 Primary 场景合同里
  3. `003` 已通过 synthetic resident entry 并回 resident runtime：
     - `currentPhase > EnterVillage && currentPhase <= DayEnd` 时，CrowdDirector 会额外补 `003` 进 resident runtime
     - 夜间也由 resident/crowd 合同接，不再归导演私房 story actor owner
  4. `19:00` 提醒段在现码里仍是正式剧情段：
     - `BeginReturnReminder()` 把相位切到 `ReturnAndReminder`
     - `EnterFreeTime()` 要到 `19:30` 才切 `FreeTime`
  5. `Town.unity` 当前 scene 真值里：
     - `001` 原生摆位 = `(-12.55, 14.52)`
     - `002` 原生摆位 = `(-10.91, 16.86)`
     - `003` 原生摆位 = `(-22.02, 10.29)`
     - `001_HomeAnchor = (-12.15, 13.12)`
     - `002_HomeAnchor = (-17.67, 13.94)`
     - `003_HomeAnchor = (43.70, -10.70)`
- 父层当前判断：
  1. 用户对白天语义的修正，现码已基本接上。
  2. 当前仍需特别小心的，是不要把 `19:00 reminder` 误说成“已经放完人”，因为代码里它还在剧情态。
  3. `0.0.6` 回 `Town` 的 `001/002` 当前没有额外导演待机点；它们吃的是 scene 原生站位。
- 本轮仍是只读调查，无代码改动、无 live 复验。

## 2026-04-17｜父层补记：打包版最新反馈对应的 Day1 真问题已收束
- 用户新增 packaged-build 反馈后，父层需要知道的高置信结论：
  1. `opening` 仍是双轨 owner：
     - crowd resident 由 `SpringDay1NpcCrowdDirector` 提前开跑
     - `001~003` 由 `SpringDay1Director` 单独驱动 story actor route
     - crowd director 还带 `[DefaultExecutionOrder(-300)]`
     - 这足以解释“101~203 先动，001~003 后动”
  2. `opening` 结束后 crowd resident 传回 baseline，不是用户错觉：
     - cue 释放后现码会走 `TryReleaseResidentToDaytimeBaseline(...)`
     - 这一步会直接把 NPC 传到 `BasePosition`
  3. `003` 现在不是“完全还在 special chain”：
     - 剧情外它已经 synthetic 并回 ordinary resident
     - 但 opening/dinner 仍吃导演 special route，所以“身份方向对了，入口 owner 还没完全统一”
  4. `19:00` 的普通 resident 异常不是 clock return-home：
     - crowd resident 的时钟回家固定是 `20:00~20:59`
     - `21:00+` 才 forced rest
     - 因此 `19:00` 看到的大量 resident 异常，更像 beat 切换后的 baseline teleport / visibility/runtime 偏差
  5. `001/002` 和 `003` 的交互分裂是当前合同结果：
     - `001/002` 在 `DinnerConflict / ReturnAndReminder` 仍由导演 story actor mode 禁聊天
     - `003` 没跟着进这条 story-actor policy
  6. 夜间实现仍分裂且不符合用户想要的“21:00 传到 anchor 并隐藏”：
     - `001/002` 走导演夜间链
     - `003~203` 走 crowd resident 夜间链
     - 当前只会回家/snap/rest，不会 hide，也没有次日统一 re-activate 合同
- 父层当前判断：
  1. 如果进入下一轮施工，最高价值不是再做零散参数修，而是：
     - opening 统一 owner
     - baseline teleport 改成原地解散
     - 夜间 20/21 统一到单一 resident contract
  2. 艾拉 `1.6` 是一条可独立插拔的低风险数值刀。
- 本轮仍是只读，不宣称体验已修完。

## 2026-04-17｜父层补记：箱子内容跨场景 / 跨天复活或回滚的只读根因排查
- 当前主线目标：
  - 不改代码，只读解释“箱子内容为什么会在切场或跨天后复活 / 回滚”。
- 本轮子任务：
  1. 串联 `ChestController`、`PersistentObjectRegistry`、`SaveManager`、`DynamicObjectFactory`、`PersistentPlayerSceneBridge`、`SceneTransitionTrigger2D`
  2. 重点区分“正式存档恢复链”和“切场 runtime continuity 链”各自有没有覆盖箱子
- 已完成事项：
  1. 已证实：`PersistentPlayerSceneBridge` 的 scene runtime continuity 根本没有覆盖 `ChestController`
     - `CaptureSceneWorldRuntimeState()` / `BuildSceneWorldRuntimeBindings()` 只收 `WorldItemPickup`、`TreeController`、`StoneController`
     - 回场恢复 `RestoreSceneWorldRuntimeState()` 也只会还原这三类
     - 这意味着箱子内容、空箱子状态、被挖掉后的缺失状态都不会被 bridge 带过场景切换
  2. 已证实：箱子本体回到场景后，会重新走 prefab 自身初始化与作者预设内容
     - `ChestController.Initialize()` 会在新建库存时执行 `ApplyAuthoringSlotsIfNeeded(...)`
     - 如果没有正式存档 `Load(...)` 或 bridge runtime snapshot 把运行时内容补回来，箱子会回到 prefab/作者预设状态
  3. 已证实：正式存档链和切场连续性链对箱子的覆盖范围不一致
     - `PersistentObjectRegistry.RestoreAllFromSaveData()` + `DynamicObjectFactory.TryReconstruct(data)` 明确支持 `Chest`
     - 但 `PersistentPlayerSceneBridge` 没有对应的 chest snapshot / restore / reconstruct
     - 结果就是“读档链认识箱子，切场链不认识箱子”
  4. 高概率：跨天回滚与“正式存档只收当前 scene worldObjects”直接相关
     - `SaveManager.CollectFullSaveData()` 注释和测试都明确：正式存档当前只收当前已加载 scene 的持久对象，off-scene world state 不并入 `worldObjects`
     - 因此如果跨天保存发生在不含该箱子的场景，别的场景箱子状态不会进档；之后再去那个场景，就更容易看到旧内容/旧存在状态
- 关键决策：
  1. 这轮只做静态链路定责，不提出修法。
  2. 当前最高置信问题不在 UI，也不在单一 `ChestController.Save/Load`，而在“两套持久化链路对箱子覆盖不一致”。
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\World\Placeable\ChestController.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\PersistentObjectRegistry.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\DynamicObjectFactory.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PersistentPlayerSceneBridge.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SceneTransitionTrigger2D.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SaveManagerDay1RestoreContractTests.cs`
- 验证结果：
  - 纯静态推断成立；未改代码，未跑 live，未宣称修复完成。
- 遗留问题或下一步：
  1. 如果后续还要继续钉“跨天”而不是“跨场景”，需要再顺着睡觉 / 新一天的实际保存入口追到具体调用点。
  2. 当前最可能看错的点不是 chest continuity 主链，而是“跨天究竟走哪条触发链”这件事。

## 2026-04-17｜父层补记：Day1-V3 已建立 0417 主控文档
- 当前 Day1 新入口已经从“长聊天 + 多份旧总表”切到：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\100-重新开始\0417.md`
- 这份文档现在承担 4 个角色：
  1. `用户语义真值`
  2. `当前代码真相`
  3. `全量问题总表`
  4. `P0~P6` 持续维护 tasks
- 父层当前最关键的新冻结点：
  1. opening 后 ordinary resident 的真语义已经正式写死为：
     - 原地解散
     - 白天自由活动
     - `20:00` 前不回 baseline/anchor/home
  2. dinner 的真语义已经正式写死为：
     - 全员传起点
     - 只走一次到终点
     - 最多 `5` 秒
     - 超时 snap
     - `18:00~19:30` 无额外走位
  3. 夜间最终合同已经正式写死为：
     - `20:00 return-home`
     - `21:00 snap-to-anchor + hide`
     - 次日从 anchor 激活
- 当前父层判断：
  1. Day1 这条线已经不再缺“语义定义”，现在缺的是按总表稳定施工。
  2. 后续若继续真实施工，应直接按 `0417.md` 的顺序推进，不再回到散修或从旧 prompt 拼上下文。
  3. 本轮是文档与冻结层收口，不是 runtime 修复轮，不宣称体验过线。

## 2026-04-17｜父层补记：0417 已从阶段清单升级为包级任务板
- 本轮不是新增代码修复，而是对 `0417.md` 做了第二次冻结和重构。
- 父层需要知道的新变化：
  1. `0417` 不再只是 `P0~P6` 的粗阶段标题。
  2. 现在已经升级为 `Package A~G` 的包级任务板，每个 package 明确写了：
     - 目的
     - 主要锚点
     - 前置
     - 具体 tasks
     - 退出条件
  3. 新增了两块非常关键的防漂移内容：
     - `当前已经结构性落地、但不能冒充过线的内容`
     - `当前最容易让我后续漂移的 5 个真源`
- 当前父层最重要的新冻结结论：
  1. Day1 当前真正的漂移源，不只是 runtime owner，还包括：
     - data asset
     - editor/bootstrap/validation
     - tests
     - 用户 live 与静态结构差
  2. 后续如果不把这几层同时放进任务区，就算 runtime 改对，仍会被旧资产和旧测试重新拉歪。
  3. 因此父层现在认可的执行顺序更新为：
     - `A`
     - `C-01`
     - `B`
     - `C`
     - `D`
     - `E`
     - `F`
     - `G`
- 本轮仍是文档冻结，不是代码施工，不宣称体验过线。

## 2026-04-17｜父层补记：Package A 六张冻结表已完成
- 父层当前新增的稳定事实：
  1. `0417.md` 里的 `Package A` 已从“待补”变成“已完成（冻结层）`。
  2. 已真实补齐的 6 张表包括：
     - 语义 -> `StoryPhase / beatKey / runtime entry / test` 映射表
     - owner matrix
     - resident lifecycle 全入口冻结表
     - 旧 beat / semantic anchor 依赖矩阵
     - editor/bootstrap/validation 残依赖矩阵
     - 测试分层表
  3. `A-01 ~ A-06` 现已全部标记完成。
- 父层当前判断：
  1. 后续 Day1 再漂移的风险已经显著下降，因为现在不再缺“冻结底板”。
  2. 下轮若继续真实施工，正确顺序已经进一步收窄为：
     - `C-01`
     - `B`
     - `C`
     - `D`
     - `E`
     - `F`
     - `G`
  3. 这轮仍然只是文档/冻结层完成，不是 runtime 修复轮，不宣称体验过线。

## 2026-04-17｜父层补记：opening 单独链与二次摆位风险的只读根因已钉死
- 本轮没有进入真实施工，只补了一轮 opening 专项静态定责。
- 父层需要知道的新增结论：
  1. `001~003` 之所以和 ordinary resident 分链，不是用户错觉，而是当前代码真合同：
     - `001/002` 被 `SpringDay1NpcCrowdDirector.ShouldDeferToStoryEscortDirector(...)` 长期留给导演 owner；
     - `003` 则因为 `ShouldIncludeThirdResidentInResidentRuntime(...)` 只在 `currentPhase > EnterVillage` 才 synthetic 并回 resident runtime，所以 opening 期间也仍在导演链。
  2. opening 当前不是一份 staged contract，而是至少四份信息源混用：
     - `TryResolveTownOpeningLayoutPoints()`
     - `TryResolveTownOpeningMarker()`
     - `TryResolveTownVillageGateCueTarget()`
     - `TryResolveTownVillageGateHardFallbackTarget()`
     只要这些来源不完全等价，就会继续制造 start/end 解释漂移。
  3. 最危险的二次摆位触发点不在导航，而在导演层 reset/latch：
     - `ResetTownVillageGateDialogueSettlementState()` 会同时清掉 cue wait 和 `_townVillageGateActorsPlaced`
     - `TryPrepareTownVillageGateActors()` 第一次又会把 `001~003` 重新放回 start
     - `MaintainTownVillageGateActorsWhileDialogueActive()` / `ForcePlaceTownVillageGateActorsAtTargets()` 则会继续往 end snap
     - 这正是“先到终点又回起点 / 或二次摆位”的最高置信根因
  4. current beat 也会在 `EnterVillage` 内部提早切档：
     - `ShouldUseEnterVillageHouseArrivalBeat()` 当前会被 `HasVillageGateCompleted()`、`_townHouseLeadStarted`、`_townHouseLeadTransitionQueued` 提前触发
     - 这会让 crowd release 与 director escort 在同一相位内提前分叉
- 当前最值钱的下一刀：
  1. 不是继续散修导航或参数
  2. 而是先把 opening 收成同一份 staged contract：
     - 统一 actor set
     - 统一 route source
     - 统一 release latch
     - 统一 beat switch
- 当前状态：
  - 纯静态推断成立
  - 未改代码、未跑 live，不宣称问题已修完

## 2026-04-17｜父层补记：`0.0.6` 回 Town 的 own 触发链已补到方法级
- 本轮不是继续施工，而是补一轮只读链路定责，专门对应用户点名的：
  - `SpringDay1Director / SpringDay1NpcCrowdDirector`
  - `_postTutorialExploreWindowEntered`
  - `ShouldAllowPrimaryReturnToTown`
  - `RecoverReleasedTownResidentsWithoutSpawnStates`
  - `ShouldYieldDaytimeResidentsToAutonomy`
  - `HandleSceneLoaded / HandleActiveSceneChanged`
  - `_syncRequested`
- 父层需要知道的新增结论：
  1. `0.0.6` 自由活动窗的 `Primary -> Town` 放行真源在 director：
     - `_postTutorialExploreWindowEntered=true` 后，`ShouldAllowPrimaryReturnToTown()` 直接放开 return gate
  2. 真正的 Town re-entry recover / resume 主体在 crowd：
     - scene change / scene load / phase change 都会把 `_syncRequested` 拉起
     - 之后 `RecoverReleasedTownResidentsWithoutSpawnStates()` 会分批补 `SpawnState`
     - 成功补绑的 resident 会进 `QueueResidentAutonomousRoamResume(...)`
  3. dinner 回 Town 还会多一层导演硬触发：
     - `HandleSceneLoaded()` 命中 `_pendingDinnerTownLoad + Town`
     - `ActivateDinnerGatheringOnTownScene()` 会 `SetPhase(DinnerConflict)`、`UpdateSceneStoryNpcVisibility()`、`ForceImmediateSync()`
  4. 当前最像“群体卡爆”的，不是 `20:00+` 的 return-home，而是：
     - `ApplyResidentRuntimeSnapshotToState()` 恢复旧 director cue
     - `_syncRequested` 多入口拉高
     - `RecoverReleasedTownResidentsWithoutSpawnStates()` 的分批 recover 波次
  5. 当前最像“聊天后单体解放”的，是 director 只对 `001/002` 做对称 release：
     - `UpdateSceneStoryNpcVisibility()` / `ApplyStoryActorRuntimePolicy()` 会在 Town 里释放并直接 resume `001/002`
     - `003~203` 仍在等 crowd 的 release 语义
- 当前父层判断：
  1. 下一轮若开刀，不该先碰 `return-home`，因为 dinner 前回 Town 主锅不在时钟夜间链。
  2. 最小顺序应收窄为：
     - `ApplyResidentRuntimeSnapshotToState()`
     - `HandleActiveSceneChanged() / HandleSceneLoaded() / RecoverReleasedTownResidentsWithoutSpawnStates()`
     - `UpdateSceneStoryNpcVisibility() / ApplyStoryActorRuntimePolicy()`
  3. 也就是说，先拆“旧 owner 被恢复 + 多轮 recover/sync”，再拆“director 只放活 001/002”。

## 2026-04-17｜父层补记：`Package E` 第一刀已落地，夜间 split owner 已收平一半
- 本轮性质：
  - 真实施工，主控板仍是 `100-重新开始/0417.md`。
- 父层需要知道的新增事实：
  1. `001/002` 已不再由 `SpringDay1Director.SyncStoryActorNightRestSchedule()` 持续夜管；
     - `Director` 现已退成 release；
     - `FreeTime / DayEnd` 的 `001/002/003` 会一起纳回 `SpringDay1NpcCrowdDirector` resident runtime。
  2. `21:00` 的统一 `SnapToAnchor + Hide` 已在 crowd 侧落成；
     - `ApplyResidentNightRestState(...)` 现在会先 snap，再 hide。
  3. `HandleSleep()` 已补 `SpringDay1NpcCrowdDirector.ClearResidentRuntimeSnapshots()`；
     - 这条是“不要把 Day1 夜间隐藏状态带进次日 snapshot”的结构保护。
  4. `0417.md` 已同步：
     - 新增 `C-15 / C-16 / C-17`
     - `Package E` 改成 `进行中`
     - `E-01 / E-02 / E-04 / E-05 / E-06` 已打勾
  5. `E-08` 已被钉成明确 blocker：
     - `NPCAutoRoamController` 还没有 `FormalNavigation completion` 对外信号
     - 在这个信号补齐前，不能安全删 `TickResidentReturnHome()` 的 arrival/finish 职责
- 已补验证：
  1. 脚本级 validate：
     - 4 个相关脚本 `errors=0`
  2. EditMode 定向 tests：
     - 夜间相关 `8` 条，`passed=8 failed=0`
  3. `git diff --check`：
     - own 文件通过
- 父层判断：
  1. `Package E` 现在已经从“未开始”推进到“进行中”。
  2. 这轮最重要的不是“夜间全收完”，而是先把 split owner 拉平，同时把不能硬砍的 `E-08` blocker 说实。
  3. 下一刀如果继续，不应再回掰 dinner，而应先补 `FormalNavigation completion` 信号，再回来收 `E-08`。

## 2026-04-17｜父层补记：晚饭入口第一拍、night home-anchor 真值、bridge native snapshot 回流已继续收口
- 父层新增事实：
  1. 晚饭入口现在又多了一条源码级保护：
     - `ActivateDinnerGatheringOnTownScene()` 第一拍就必须把 `001/002` 放到 authored 起点；
     - 对应测试 `ActivateDinnerGatheringOnTownScene_ShouldPlaceStoryActorsAtAuthoredStartsOnPhaseEntry` 已通过。
  2. `001/002/003` unified night runtime 的 `HomeAnchor` 真值已修：
     - 过去可能误绑到 NPC 本体；
     - 现在明确先解 `001_HomeAnchor / NPC001_HomeAnchor`，并禁止把 resident 本体当成 home anchor。
  3. `PersistentPlayerSceneBridge` 已新增 native resident snapshot 清理口：
     - Day1 `HandleSleep()` 现在除了清 crowd snapshots，还会清 `001/002/003` 的 bridge native snapshots；
     - 目的就是避免次日/跨场景把昨晚位置带回来。
- 已补验证：
  1. 关键窄回归 `7/7 passed`
  2. fresh console `0 error / 0 warning`
  3. 当前 own 文件 `git diff --check` 通过
- 父层当前判断：
  1. `20:00 不回家 / 21:00 原地隐藏 / 次日从消失点回来` 这条链上，已经继续实锤并修掉了真实代码根因。
  2. 当前仍未收完的重点不再是 anchor 真值，而是 `E-08` 的 tick/retry/finish 持续代管还没完全退干净。

## 2026-04-17 stale tests 只读恢复点
- `spring-day1 / 004_runtime收口与导演尾账` 新增一轮只读审计，目标只盯 `0417.md -> F-07`。
- authoritative 结论：
  1. 当前 stale tests 的最高风险面不在单个断言，而在 4 类旧真值：
     - `Town_Day1Residents / Town_Day1Carriers` 结构根
     - `EnterVillageCrowdRoot / DinnerBackgroundRoot / NightWitness_01 / DailyStand_*` 旧 semantic anchors
     - `DailyStand_Preview / FreeTime_NightWitness` 旧 beat 真值镜像
     - `return-home 完成后必须 resume roam / force restart roam`
  2. 最该先收的文件仍是：
     - `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 然后才是 `NpcCrowdManifestSceneDutyTests.cs`、`NpcCrowdResidentDirectorBridgeTests.cs`
  3. opening/dinner fallback 侧最可疑的旧要求在：
     - `SpringDay1OpeningRuntimeBridgeTests.cs` 里仍要求保留 `Town VillageGate actor fallback` 解析入口
- 当前恢复点：
  - 如果后续进入真实施工，先把测试断言从“旧中间层存在”改成“runtime 结果正确”，不要再为了过镜像测试把 Day1 runtime 拉回旧语义。

## 2026-04-17｜父层补记：`TownNativeResidentMigrationMenu.cs` 编译红已清
- 父层新增事实：
  1. 用户报告的 4 条 `CS0133` 来自 `TownNativeResidentMigrationMenu.cs` 中 `const string = string.Empty` 不是编译期常量。
  2. 当前已改为字面量 `""`，对应 4 个常量：
     - `TownResidentRootName`
     - `TownResidentDefaultRootName`
     - `TownResidentTakeoverRootName`
     - `TownResidentBackstageRootName`
  3. `0417.md` 已补记到 `Package F / F-05` 下：这条旧迁移菜单仍按 deprecated blocker 处理，不恢复旧 Town resident migration 链。
- 已补验证：
  1. `validate_script Assets/Editor/Town/TownNativeResidentMigrationMenu.cs` 的 native validate 为 clean。
  2. fresh console 已回到 `0 error / 0 warning`。
  3. `git diff --check -- Assets/Editor/Town/TownNativeResidentMigrationMenu.cs` 通过。
- 父层当前判断：
  1. 这是 editor/tooling 侧的编译 blocker 收口，不代表 Day1 runtime 体验已过线。
  2. 后续仍应按 `0417 / Package F` 继续清 stale editor/tests，而不是因为这条 compile 红已清就回到旧 runtime 语义。

## 2026-04-17｜父层补记：0417 进度盘点与“不可打包”判断
- 父层新增事实：
  1. 当前 `0417.md` 的任务量是：
     - `41 done / 27 todo`
     - `A=完成`
     - `B/C/D/E/F=进行中`
     - `G=未开始`
  2. 这轮额外收回了 5 个只读子智能体结论；没有新的 runtime 代码落地。
  3. 5 个子智能体一致指向：`B/D/F` 仍有实质未完，`E-08` 也还不是“彻底归零”。
- fresh 信号：
  1. `errors` 当前是 `0 error / 0 warning`
  2. 但 Unity `status` 显示当前在 `is_playing=true`、`playmode_transition/stale_status`
  3. 仓库仍是大面积 mixed dirty，且还有其他线程 `ACTIVE`
- 父层当前判断：
  1. 现在不能把 Day1 判成“可打包”。
  2. 原因不是这轮又引入了 compile 红，而是：
     - `0417` 还远未收尾
     - `Package G` 完全没开始
     - 缺 fresh live / packaged 证据
     - 当前 shared root 也不处于适合打包的稳定窗口

## 2026-04-17｜父层补记：打包前最小待完成范围已缩到夜间尾刀
- 父层新增事实：
  1. 用户最新 packaged 反馈表明：
     - opening / primary / dinner 当前不是最小打包 blocker
     - 真正剩下的是夜间尾刀
  2. 重新核代码后，夜间骨架并不缺：
     - `20:00` 回家入口已存在
     - `21:00 snap + hide` 已存在
     - 次日从 anchor 恢复已存在
  3. 当前最小待完成项已收窄为：
     - 单点修 `101` 的 `20:00` 不回家
     - 将 morning release 从 `9:00 -> 7:00`
     - 校准“到 anchor 附近即隐藏”
     - 保持 `21:00` 的强制 snap + hide
     - 保持次日从 anchor 激活，但改到 `7:00`
- 父层当前判断：
  1. 这条线下一轮不应再按“全 Day1 大收尾”开刀。
  2. 更准确的口径是：现在只剩 `Package E` 的打包前夜间 hotfix。

## 2026-04-17｜父层补记：夜间尾刀验证已补，`101` 先按场景 anchor 真值处理
- 父层新增事实：
  1. `Day1-V3` 本轮没有重开 opening / primary / dinner 主修，而是只继续收夜间打包尾单。
  2. `0417.md` 已新增 `C-23`，明确：
     - `HomeAnchor` 不是硬编码坐标；
     - `FindSceneResidentHomeAnchor(...)` 会按 `npcId / NPCxxx` 派生 `xxx_HomeAnchor`；
     - `101` 不允许写代码特判，优先调场景 `101_HomeAnchor`。
  3. `Town.unity` 已静态确认存在：
     - `001/002/003/101/102/103/104/201/202/203_HomeAnchor`
  4. 夜间尾刀当前已收成：
     - `20:00` 回家开始
     - 到 anchor 附近即隐藏
     - `21:00` 仍保留强制 `snap + hide`
     - 次日 `7:00` 从 anchor 激活
- 已补验证：
  1. 夜间尾单定向 EditMode tests：`8/8 passed`
  2. fresh console：清理旧噪音后 `0 error / 0 warning`
  3. `git diff --check` 覆盖本轮 Day1 own 文件通过
- 父层当前判断：
  1. 这轮 Night hotfix 的结构与 targeted 证据已经补齐。
  2. 当前还不能把这条线说成“packaged 已过线”，因为用户还没做新的 packaged/live 复验。
  3. `101` 现在最像 scene anchor / runtime 绑定核查问题，不该回退成 `101` 专用分支修法。
- 父层恢复点：
  1. 用户可以先在场景里调整 `101_HomeAnchor` 等对象位置；只要对象名不变，Day1 代码不用改坐标。
  2. 下一轮若用户给出新 packaged 反馈，应优先只复测：
     - `20:00`
     - `21:00`
     - 次日 `7:00`
     - `101`
  3. 若 `101` 仍异常，再继续抓 runtime probe 看真实绑定和导航启动，不扩回全 Day1 大修。

## 2026-04-17｜父层停车补记：Day1-V3 已合法 Park
- 当前 `thread-state`：
  - `Day1-V3 = PARKED`
  - blocker：`await-user-anchor-adjust-and-packaged-night-retest`
- 父层当前判断：
  1. 这条线此刻最正确的停点就是“等用户先调 anchor，再做 packaged/live 复验”。
  2. 现在不该再为了显得推进而乱开新代码刀。

## 2026-04-17｜父层补记：用户最新最小尾刀已继续收掉两条
- 父层新增事实：
  1. `Day1-V3` 这轮没有重开夜间/晚饭大修，而是只收用户刚点名的两条最小语义：
     - `TownHouseLead` 等待玩家时 `002` 继续补位；
     - 艾拉疗伤按圆范围触发并在触发后朝向玩家。
  2. 代码已落在：
     - `SpringDay1Director.TryBeginTownHouseLead()`
     - `SpringDay1Director.TryHandleHealingBridge()`
  3. `0417.md` 已同步更新：
     - `2. 当前代码真相`
     - `4. 全量问题清单`
     - `8. 迭代 tasks`
- 已补验证：
  1. 定向 EditMode tests `4/4 passed`
  2. fresh console `0 error / 0 warning`
  3. `git diff --check` 覆盖 own 文件通过
- 父层当前判断：
  1. 这两刀属于低风险、局部闭环、与用户最新语义完全对齐的必要尾刀。
  2. 当前仍不能把 Day1 整体写成“体验全过线”，因为这轮没有 fresh live / packaged 终验。

## 2026-04-17｜父层停车补记：Day1-V3 已就最新最小尾刀重新 Park
- 当前 `thread-state`：
  - `Day1-V3 = PARKED`
  - blocker = `await-live-retest-healing-circle-and-town-house-lead-companion-follow`
- 父层当前判断：
  1. 这轮最正确的停点就是等用户 live / packaged 复测疗伤圆范围与 `002` 等待补位。
  2. 不需要把这两条低风险尾刀再扩成新的 Day1 重构切片。

## 2026-04-17｜父层续记：`003` opening 导演私链已退场，opening 定向桥接回归已收平
- 父层新增事实：
  1. `Day1-V3` 已把 `003` 从 opening director 私链里退出：
     - `EnterVillage` 起就并入 resident runtime
     - director opening 不再 resolve / reframe / drive `003`
  2. opening active dialogue 窗口里，`001/002` 的 authored 终点对位又补了一层即时保护。
  3. opening 桥接测试里的旧反射 helper 也已修正，不再制造 `TargetParameterCountException` 假失败。
- 已补验证：
  1. opening 定向 EditMode tests `6/6 passed`
  2. direct `validate_script`：
     - `SpringDay1Director.cs` `errors=0`
     - `SpringDay1OpeningRuntimeBridgeTests.cs` `errors=0`
  3. fresh console 清理后 `0 error / 0 warning`
  4. `git diff --check` 覆盖 own 文件通过
- 父层当前判断：
  1. `003` 特殊化的 runtime 主问题已经继续缩小，现在主要剩 opening 单一 owner 未完成和 `Package F` 的旧真值残依赖。
  2. 这仍是结构 / targeted probe 成立，不等于 packaged/live 体验已过线。

## 2026-04-17｜父层续记：夜间“到家即隐藏”已从贴点判定收成到站半径
- 父层新增事实：
  1. `Day1-V3` 继续沿 `Package E` 收了一刀夜间热修，只修 `20:00 到家即隐藏` 的漏口。
  2. `SpringDay1NpcCrowdDirector` 现在把 night return 的 home arrival 收成统一 `0.35` 到站半径，不再靠过严的 `0.05` 贴点判定。
  3. `TryBeginResidentReturnHome(...)` 已改成“已在家附近 -> 直接隐藏”优先于 retry cooldown。
  4. `TickResidentReturnHome(...)` 在无 active FormalNavigation completion 的情况下，也会按同一到站半径直接收尾隐藏。
  5. `0417.md` 已同步新增 `C-26`，并更新 `G-09 / I-09 / I-10 / Package E`。
- 已补验证：
  1. direct `validate_script`：
     - `SpringDay1NpcCrowdDirector.cs`：`errors=0 warnings=2`
     - `SpringDay1DirectorStagingTests.cs`：`errors=0 warnings=0`
  2. fresh console：`0 error / 0 warning`
  3. `git diff --check` 覆盖本轮 own 文件通过
  4. CLI `validate_script` 给出 `unity_validation_pending owned_errors=0 external_errors=0`，block 在 Unity compile ready，不是 owned red
- 父层当前判断：
  1. 这刀已经把“到家即隐藏”更准确地接回导航到站语义。
  2. 现在仍不能 claim packaged/live 已过线，因为用户还没做新一轮夜间复测。
  3. 如果后续还有少数 resident 站在 anchor，优先看 runtime probe / home anchor 绑定，不回到旧中间层和代码特判。

## 2026-04-17｜父层停车补记：Day1-V3 夜间到站半径热修已 Park
- 当前 `thread-state`：
  - `Day1-V3 = PARKED`
  - reason = `night-return-arrival-radius-hotfix-waiting-packaged-retest`
- 父层恢复点：
  1. 优先等用户 packaged/live 复测 `20:00 到家即隐藏`。
  2. 若仍失败，下一刀不要重开全 Day1，只抓夜间 runtime probe：是否纳入合同、实际绑定哪个 `HomeAnchor`、是否进入 return-home/hide 分支。

## 2026-04-17｜父层续记：formal NPC 剧情外 resident informal 入口漏口已补
- 父层新增事实：
  1. `Day1-V3` 继续沿晚段 / 夜间自由窗口往下查，又定位到一条交互链 own 漏口：
     - `SpringDay1NpcCrowdDirector.ConfigureBoundNpc(...)`
     - 之前会因为 NPC 已经有 `formal` 组件而跳过自动补 `informal`
  2. 当前已改成：
     - 只要 NPC 缺 `NPCInformalChatInteractable`
     - 且 `RoamProfile` 有 resident informal content
     - 就允许自动补 informal 入口
  3. 新增定向测试：
     - `CrowdDirector_ConfigureBoundNpc_ShouldAddInformalChatWhenFormalDialogueExists`
  4. `0417.md` 已同步新增 `C-27`，并更新 `G-10 / I-08 / Package D`。
- 已补验证：
  1. `git diff --check` 覆盖 own 代码文件通过
  2. 当前 Unity 实例不在线；`doctor` 曾报 `listener_missing / pidfile_missing`，`recover-bridge` 后 CLI 仍拿到 `No active Unity instance`
  3. 所以当前只能诚实报 `结构已改 / Unity 验证待补`
- 父层当前判断：
  1. 这条 fix 很可能直接解释一类“剧情外 formal 已让位，但居民闲聊口仍缺失”的坏相。
  2. 但它现在还不能 claim live/pass，必须等 Unity 可用后补窄验证或等用户 packaged/live 继续测。

## 2026-04-17｜父层停车补记：formal->informal 入口热修已 Park
- 当前 `thread-state`：
  - `Day1-V3 = PARKED`
  - reason = `formal-to-informal-entry-hotfix-waiting-unity-validation-or-live-retest`
- 父层恢复点：
  1. Unity 可用后优先补这条交互链 hotfix 的窄验证。
  2. 若用户先做 packaged/live，重点只收：
     - `19:30` 后 ordinary resident 与 `001/002` 是否恢复可聊
     - `20:00` 回家途中是否仍可聊天

## 2026-04-17｜父层补记：Day1-V3 已清理 `SpringDay1DirectorStagingTests` 新增测试 compile red
- 父层新增事实：
  1. `Day1-V3` 新增的 `CrowdDirector_ConfigureBoundNpc_ShouldAddInformalChatWhenFormalDialogueExists` 测试，曾误用不存在的 `ResolveNestedTypeOrFail(...)` helper。
  2. 当前已改回现有反射 helper 口径：
     - `ResolveTypeOrFail("NPCDialogueContentProfile+InformalChatExchange")`
     - `ResolveTypeOrFail("NPCDialogueContentProfile+InformalConversationBundle")`
  3. 这条新增测试自己的 `CS0103` compile blocker 已清，不再卡住 `C-27`。
- 已补验证：
  1. `git diff --check` 覆盖该测试文件通过。
  2. fresh `status` 已出现 `Build completed with a result of 'Succeeded'`。
  3. fresh `errors` 当前只剩外部 `DialogueChinese V2 SDF.asset` importer inconsistent result。
  4. `validate_script` 对该测试文件给出 `owned_errors=0`。
- 父层当前判断：
  1. 这是一条支撑性清红，不是新的 runtime 进展。
  2. Day1 当前仍不能 claim 全线 no-red，因为项目现场还有外部 importer 红。

## 2026-04-17｜父层补记：疗伤桥已从“玩家中心 -> 艾拉脚点”改成“玩家中心 -> 艾拉真实中心”
- 父层新增事实：
  1. 用户 packaged 反馈指出：从 `002` 左侧接近时，疗伤桥仍要几乎碰到才触发。
  2. `Day1-V3` 继续沿 `0417` 对位后确认：之前只改了玩家采样点，疗伤圆心仍吃 `002.transform.position`。
  3. 当前已改成：
     - 玩家侧仍取本体中心
     - 艾拉侧改取 collider / presentation bounds.center
     - 验证贴近逻辑也切到同一 sample 点
  4. 新增 guard test：
     - `HealingBridge_ShouldUseSupportCenterAsProximityAnchor`
- 已补验证：
  1. `validate_script` 覆盖 `SpringDay1Director.cs` 与 `SpringDay1DirectorStagingTests.cs` 均为 `owned_errors=0`
  2. fresh `errors`：`0 error / 0 warning`
  3. `git diff --check` 覆盖 own 文件通过
- 父层当前判断：
  1. 这刀是对 packaged 真实体验反馈的根因修正，不是继续盲调参数。
  2. 仍需用户重新打包复测左侧接近入口，不能把结构修正说成体验已过线。

## 2026-04-17｜父层补记：夜间 `20:00/21:00` resident 合同已继续收硬
- 父层新增事实：
  1. `Day1-V3` 已把夜间 resident 合同再补两条硬规则：
     - `20:00` 回家途中丢了 active drive，不再一直晾到 `21:00`，会按短节流重试 return-home
     - forced day-end / `21:00` snap 链会明确写 `HideWhileNightResting=true`，不再只 snap 不 hide
  2. 同时补了一条无 `HomeAnchor` 的 `21:00` hide 兜底，避免因为 anchor 缺失而漏网。
  3. 新增 / 改写 3 条定向测试，钉死 night return retry、无 anchor 兜底 hide、以及 forced day-end snap 必须 hide。
- 已补验证：
  1. `validate_script` 覆盖 `SpringDay1NpcCrowdDirector.cs` 与 `SpringDay1DirectorStagingTests.cs` 均为 `owned_errors=0`
  2. fresh `errors`：`0 error / 0 warning`
  3. `git diff --check` 覆盖 own 文件通过
- 父层当前判断：
  1. 这刀继续贴近用户已经反复钉死的夜间真语义，不再让 `20:00` 的可见窗口被误实现成“失败就站到 21:00”。
  2. 仍需 packaged/live 继续验，不把结构硬化包装成体验过线。

## 2026-04-17｜父层补记：夜间 `20:00/21:00 resident return-home/hide` 只读真相已补到方法级
- 父层新增事实：
  1. `20:00` 当前不是“立刻 snap+hide”，而是 crowd 的可见 return-home 窗口：
     - `SpringDay1Director.HandleHourChanged(...)` 不直接调 crowd hide；
     - 真正执行链在 `SpringDay1NpcCrowdDirector.Update()` -> `SyncResidentNightRestSchedule()` -> `TryBeginResidentReturnHome(...)`。
  2. `21:00` 的 fallback 已明确是：
     - `IsNightResting=true`
     - `HideWhileNightResting=true`
     - `ApplyResidentNightRestState(...)` 里 `release shared owner -> SnapToTarget -> SetActive(false)`。
  3. 当前“明明看起来回家了却还站在场上”的高价值源码解释已收窄到：
     - `FormalNavigation arrival` 没到
     - 还没进 `0.35` 到站半径
     - `HomeAnchor` 绑定真值不对
     - scene/snapshot/recover 分支先把 resident 重新放回 active
  4. 一个容易和 `21:00 fallback` 混淆、但语义不同的入口已确认：
     - `HandleSleep()` -> `TrySnapResidentsToHomeAnchorsForDayEnd()` 这条 DayEnd snap 链不会先写死 `HideWhileNightResting=true`，所以它是“回 anchor”，不是“21:00 强制隐藏”。
- 父层当前判断：
  1. 这轮最值钱的不是再泛谈 `night schedule`，而是已经把真正要抓的点缩到 `arrival/completion / anchor truth / restore-recover 顺序`。
  2. 如果后续继续开刀，第一优先不该回掰 dinner/opening，而应先补夜间 runtime probe 或最小热修。

## 2026-04-17｜父层补记：`0.0.6 Town anchor-first` 已从静态方案进入真实 targeted 验证
- 父层新增事实：
  1. `Day1-V3` 已把 `0.0.6` 的 `001/002 anchor-first` 真正落回当前 Director 链：
     - `HandleSceneLoaded()` 在 `Town + FarmingTutorial + postTutorialExploreWindow` 时会提前触发一次 `UpdateSceneStoryNpcVisibility()`
     - `TryPrimeTownStoryActorsForExploreWindow(...)` 会把 `001/002` 对到 `001_HomeAnchor / 002_HomeAnchor`
     - `_postTutorialTownActorsAnchored` 保证这次 handoff 只做一拍
  2. 这条新 guard test：
     - `Director_ShouldSnapStoryActorsToHomeAnchorsDuringPostTutorialExploreWindowInTown`
     已被接进 `Run Director Staging Tests`
  3. 通过 `Library/CodexEditorCommands` 菜单桥二次重跑后，这条新 case 已真实进入结果包，且结果=`Passed`
  4. 当前 `Run Director Staging Tests` 整套仍是：
     - `28 pass / 10 fail`
     所以不能把“新 case 已通过”包装成“staging 整套已 clean”
- 已补验证：
  1. `validate_script Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
     - `assessment=no_red`
     - `owned_errors=0`
  2. `validate_script` 覆盖：
     - `SpringDay1Director.cs`
     - `SpringDay1DirectorStagingTests.cs`
     - `owned_errors=0`
     - assessment 仍受 `stale_status` 影响，未 claim Unity 全绿
  3. `git diff --check` 覆盖 own 文件通过
- 父层当前判断：
  1. 这条 `anchor-first` 已不再只是“我打算这么做”，而是已经有真实 runner 证明新 case 被纳入且通过。
  2. 但当前仍只站住 `结构 / targeted probe`；下一步应优先 packaged/live 再看 `001/002` 回 Town 首帧体感，不要误写成体验过线。

## 2026-04-17｜父层补记：Day1-V3 已把 `20:00` pending return-home 缺口收掉，并补完一轮主剧情 targeted 回归
- 父层新增事实：
  1. `Day1-V3` 已继续收 Day1 own 的夜间合同缺口：
     - `20:00` 首次 `return-home` drive 没挂起来时，不再掉回“没在回家里”的空态
     - 现在会先进入 `return-home-pending`，持有 owner、停住当前位置，再继续重试
  2. `Day1-V3` 同时把 `SpringDay1MiddayRuntimeBridgeTests` 里那条旧的“free time 刚进不能睡”断言改回用户最新语义：
     - `19:30` 一进入 free time，床立刻可用
  3. 这轮已新增一组新的 targeted 结果面：
     - `Opening Bridge`：`13/13 passed`
     - `Midday Bridge`：`8/8 passed`
     - `Dinner Contract`：`7/7 passed`
     - `Unified Night Contract`：`18/18 passed`
     - `Day1 Owner Regression`：`2/2 passed`
     - `Late-Day Bridge`：`11/13 passed`
- 已补验证：
  1. `validate_script`：
     - `SpringDay1NpcCrowdDirector.cs`：`assessment=no_red / owned_errors=0`
     - `SpringDay1MiddayRuntimeBridgeTests.cs`：`assessment=no_red / owned_errors=0`
  2. fresh `errors`：`0 error / 0 warning`
  3. `git diff --check` 覆盖当前 touched Day1 own 文件通过
- 父层当前判断：
  1. opening / midday / dinner / unified night / owner regression 这几组主剧情 targeted 面现在都已绿。
  2. 当前自动化残留只剩 late-day bridge 的 2 条：
     - `BedBridge_EndsDayAndRestoresSystems`
     - `DayEndPlayerFacingCopy_ShouldCarryTomorrowBurdenAndClearWorkbenchState`
  3. 所以现在最准确的口径是：
     - Day1 主剧情 targeted 质量明显上升
     - 但还不能 claim 全线自动化 clean，更不能偷渡成 packaged/live 已过线

## 2026-04-18｜父层补记：Day1-V3 已只读钉实“NPC 交互自杀链 + 多层禁聊链 + roam pause 真值”
- 父层新增事实：
  1. `Day1-V3` 已确认一个不是 Day1 独占、但当前直接炸在 Day1 路径上的 `NPC own` 硬 bug：
     - `NPCInformalChatInteractable.EnterConversationOccupation()` 先 `AcquireStoryControl("npc-informal-chat")`
     - `PlayerNpcChatSessionService.Update()` 又把任何 `IsResidentScriptedControlActive` 当 takeover，下一帧就 `CancelConversationImmediate(...)`
     - 这正好对上用户反馈的“按 E 后只冒一个字就消失”
  2. 已确认当前聊天异常不是单点，而是 4 层叠加：
     - `SpringDay1Director.ApplyStoryActorRuntimePolicy(...)`
     - `SpringDay1NpcCrowdDirector.SetResidentCueInteractionLock(...)`
     - `NPCInformalChatInteractable.IsResidentScriptedControlBlockingInformalInteraction()`
     - `PlayerNpcChatSessionService.Update()` scripted-control 自杀判断
  3. 已确认 NPC 漫游停顿节奏主吃 `NPCRoamProfile.asset`，不是单靠脚本默认值：
     - crowd `101~203` 现值大多 `shortPause 0.5~3 / longPause 3~6`
     - `001` / `002` 也各自仍偏短
     - 若要落用户要求的 `短停 2~5 / 长停 5~8`，最有效的真实落点是现用 profile 资产
- 父层当前判断：
  1. 这轮最值钱的新结论不是又抓到一个 Day1 小补丁，而是已经明确分开：
     - `Day1 own` 越权禁聊
     - `NPC own` 会话自杀
     - `roam` 节奏真源
  2. 下一刀如果继续施工，最合理顺序应改成：
     - 先修 `NPC informal chat` 自杀链
     - 再清 Day1 own 的关组件 / 禁聊泄漏
     - 最后再做 `NPCRoamProfile` 的节奏调慢

## 2026-04-18｜父层补记：Day1-V3 已把交互自杀链、Day1 禁聊链和 roam pause 真值真正落回代码
- 父层新增事实：
  1. `Day1-V3` 已完成三条真实施工：
     - `PlayerNpcChatSessionService` 不再误杀 `npc-informal-chat` 自己的会话
     - `NPCInformalChatInteractable / NPCDialogueInteractable` 现改成按 resident scripted-control contract 判定，而不是继续靠组件开关
     - `SpringDay1Director / SpringDay1NpcCrowdDirector` 已不再直接关 `formal / informal` 组件
  2. 现用 human NPC roam profile 也已回写到：
     - `shortPause 2~5`
     - `longPause 5~8`
     且已覆盖 `NPC_Default`、`001`、`002`、`003`、`101~203`
  3. `SpringDay1TargetedEditModeTestMenu.cs` 已把这轮新增交互回归挂进 `Run NPC Formal Consumption Tests`
- 已补验证：
  1. `manage_script validate` 覆盖本轮脚本与测试文件，`errors=0`
  2. `Run NPC Formal Consumption Tests`：`6/6 passed`
  3. `Run Director Staging Tests`：fresh=`30/10`
     - 本轮改到的相关 case 仍 `Passed`
     - 剩余 fail 属旧 staging 债
  4. fresh `errors`：`0 error / 0 warning`
  5. `git diff --check` 覆盖 own 文件通过
- 父层当前判断：
  1. 这轮已经把 2026-04-18 只读归因里点名的三条打包前主刀真正落地，而不是继续停在“下一刀建议”。
  2. 当前最能说明问题的不是口头判断，而是：
     - `NPC Formal Consumption 6/6 pass`
     - fresh console `0/0`
  3. 但父层仍不能把它包装成 Day1 全线已过线，因为：
     - `Director Staging` 仍有 `10` 条旧 fail
     - packaged/live 终验仍待用户路径

## 2026-04-18｜父层补记：Day1-V3 已把 `Director Staging` 剩余 5 条失败拆成 runtime 债与测试债
- 新增父层稳定事实：
  1. `SpringDay1DirectorStagingTests` 当天 fresh 结果 `30/10` 里，用户点名的 5 条并非同类问题。
  2. 当前最像真 runtime 债的是 3 条：
     - `StagingPlayback_ShouldDriveRoamControllerInsteadOfHardPushingTransformDuringCueMotion`
     - `NpcTakeover_ShouldDisableRoamAndInteractionsUntilRelease`
     - `RehearsalDriver_ShouldPauseExistingPlaybackUntilDisabled`
  3. 其中真正与 Day1 packaged runtime 主线直接相连、且应优先和打包前风险挂钩的是前 2 条：
     - `SpringDay1DirectorStagingPlayback.Update()` 仍在直接推 `transform`
     - `SpringDay1DirectorNpcTakeover.Acquire()/Release()` 没同步 `AcquireStoryControl/ReleaseStoryControl`
  4. 另外 2 条更像 stale tests：
     - `ResidentScriptedControl_DebugMoveShouldStillParticipateInSharedAvoidance`
     - `ResidentScriptedControl_PauseAndResumeShouldPreserveScriptedMove`
     现测试 setup 仍停留在旧的 `debugMoveActive + state + requestedDestination` 口径，没有吃当前 `activePointToPointTravelContract` 真合同。
- 父层判断更新：
  1. `Director Staging` 的剩余 fail 不能继续整体视作“都要打包前收”。
  2. 当前更准确的 pack 前优先级是：
     - 先收 `StagingPlayback + NpcTakeover` 的 runtime 合同
     - `RehearsalDriver` 视是否还要继续依赖导演排练工具决定是否同轮带上
     - 两条 scripted-control stale tests 不应反向阻断 Day1 packaged 主线
- 当前恢复点：
  1. 如果后续继续 Day1 自动化/代码刀，先从 `SpringDay1DirectorStaging.cs` 开，不要先回 `NPCAutoRoamController` 内核。
  2. 如果先给用户做打包前裁定，需要明确告诉用户：
     - 当前 5 条里真正像 pack-blocker 的只有 staging/takeover 这一簇
     - 其余至少有 2 条更像测试债

## 2026-04-18｜父层补记：4 条 staging semantic/stagebook fail 当前不属于 Day1 packaged 必收项
- 当前主线仍是 Day1 打包前裁定；本轮只读补拆 `SpringDay1DirectorStagingTests` 里 4 条 semantic/stagebook fail。
- 父层判断更新：
  1. `StageBook_ShouldPreferExactNpcCueBeforeSharedAnchorAndDutyFallback` 现在确实暴露了 `SpringDay1DirectorBeatEntry.TryResolveCue()` 的优先级缺陷，但仓内未找到 `ResolveSpawnPoint()` 调用点；当前实质影响面主要是 `SpringDay1DirectorStagingWindow` 预演与 dead helper，不是 packaged runtime 现役入口。
  2. `StagingPlayback_ShouldUseSemanticAnchorAsCueStartWhenConfigured`、`...OffsetStart`、`...RebaseLegacyAbsolutePath...` 三条共同指向 `SpringDay1DirectorStagingPlayback` 底层没接 semantic start / offset / legacy-rebase。
  3. 但当前 live crowd runtime 只在 `EnterVillage_PostEntry / DinnerConflict_Table` 走 `ApplyStagingCue() -> ResolveRuntimeCueOverride() -> playback.ApplyCue()`，且 override 已把 cue 落成 scene-marker 绝对坐标；所以这 3 条当前更像 editor preview / staging helper 债，而不是 Day1 packaged runtime 硬 blocker。
  4. `SpringDay1DirectorStageBook.json` 已真实落下 migrated semantic-start 数据，说明“测试想保护的方向”没错；错的是底层 playback/tooling 没跟上，且 runtime 暂时绕开了它。
- 最小安全修口方向：
  1. `TryResolveCue()` 改成 `npcId > semanticAnchorId > duty`。
  2. `SpringDay1DirectorStagingPlayback.ApplyCue()/ResolveTargetPosition()` 补 semantic-start、semantic-offset、legacy-path-rebase，只在 `cue.useSemanticAnchorAsStart` 分支生效。
- 当前恢复点：
  1. Day1 packaged 前仍优先盯 `staging/takeover` 那一簇真实 runtime 合同。
  2. 这 4 条更适合作为 `Package F / F-07` 的 editor-preview / staging-test cleanup 一刀。

## 2026-04-18｜父层补记：`Package F` 待清残依赖当前主要落在 editor/test/data，不在 scene exact 根名
- 当前主线仍是 Day1 `0417` 收尾；这轮只读把 `F-05~F-08` pending 尾项重新压成高置信代码事实。
- 父层判断更新：
  1. `Town.unity / Primary.unity` 已无 `Town_Day1Residents / Town_Day1Carriers / EnterVillageCrowdRoot` 等 exact scene 文本残留；`Town_Day1Carriers` 在代码/数据/scene 范围也无命中。
  2. 现在最实的 `Package F` 尾巴在：
     - editor/validation 仍吃旧 root/旧 beat；
     - tests 仍把旧 semantic-anchor / beat matrix 当真值；
     - data/runtime 里仍保存旧 anchor 名，且 `003` 仍有 synthetic runtime + director fallback 特判。
- 当前恢复点：
  1. 如果继续收 `Package F`，安全顺序应先 `F-05/F-06/F-07`，再 `F-08`。
  2. 不要先删 data/runtime 旧词，再让 validation/tests 反咬成假 blocker。

## 2026-04-18｜父层补记：Day1-V3 已把 `Package F` 主干收完，并把 `G` 推到自动化 clean + live partial
- 父层新增事实：
  1. `TownScenePlayerFacingContractMenu.cs` 已不再用旧 `EnterVillageCrowdRoot / KidLook_01` 当 blocker。
  2. `NpcCrowdManifestSceneDutyTests.cs` 已不再把旧 semantic anchor 名白名单写成 expected truth。
  3. fresh probe/contract 结果：
     - `Town Entry Contract Probe`=`completed`
     - `Town Player-Facing Contract Probe`=`completed`
     - `Day1 Staging Marker Probe`=`completed`
     - `Resident Director Bridge Tests`=`3/3 passed`
     - `Town Runtime Anchor Readiness Probe`=`deprecated-runtime-anchor-readiness-probe`（预期退役）
  4. fresh `errors` 仍为 `0 error / 0 warning`
  5. fresh PlayMode trace 已真实把 Day1 从 `CrashAndMeet` 推到 `EnterVillage`，且额外抓到 `FreeTime` live snapshot
- 父层判断：
  1. `Package F` 现在可以按“主干已收口”看待；剩下不再是 editor/test/scene 阻断，而是 live/package/profiler 终验。
  2. `Package G` 目前状态应更新为：
     - `G-01/G-02/G-03` 已完成
     - `G-04` 只有 partial live，不可冒充全路径
     - `G-05/G-06/G-07` 仍待后续
  3. `Force Spring Day1 Dinner Validation Jump` 当前并不能直接落到 dinner live 证据，这条要在后续 live 终验里继续单独核。
- 当前恢复点：
  1. 父层后续若再裁定 Day1，可直接以 `0417.md` 为准，不要再按旧 `Package F pending` 口径下判断。
  2. 接下来更像终验阶段，不是继续大面积 runtime 重构。

## 2026-04-18｜父层补记：`Package G` 当前还卡在 `Town` live/profiler 入口
- 父层新增事实：
  1. `Request Spring Day1 Town Lead Transition Artifact` 这次虽然返回 `transition-requested:PrimaryHomeDoor`，但 artifact 最终 `activeScene` 落在 `Assets/000_Scenes/Home.unity`，没有进入 `Town`。
  2. `NpcRoamSpikeStopgapProbe` 当前最新样本只在 `Primary` 跑出：
     - `npcCount=0`
     - `roamNpcCount=0`
     所以它不能当 `Town` 自由活动或 `20:00` profiler 证据。
- 父层判断：
  1. `Package F` 不需要回头重开。
  2. `Package G` 的真实 blocker 现在是：
     - 自动化 live 还没稳定带进 `Town`
     - profiler 探针还没打到 Town runtime
  3. 这两条只能继续查现有命令桥/菜单链，不能冒充体验已验。
- 当前恢复点：
  1. 父层后续如再评估 Day1，应明确写：
     - `G-01/G-02/G-03` 已完成
     - `G-04` 仍是 partial live
     - `G-06` 仍无 fresh Town profiler
  2. 不要再把当前状态叙述成“只差 packaged”。

## 2026-04-18｜父层补记：`Package G` 已把 `Town free-time / 20:00` 打通，当前 blocker 收窄到 dinner 与终验
- 父层新增事实：
  1. `SpringDay1LiveSnapshotArtifactMenu.cs` 已修口：
     - `Town Lead Transition Artifact` 不再误切到 `Home`
     - `Town -> Primary` / `Primary -> Town` 都已 fresh live 对上
  2. `SpringDay1LatePhaseValidationMenu.cs` 已新增 `Advance Spring Day1 Validation Clock +1h`
     - 可以把 `Town / FreeTime / 19:30` 受控推进到 `20:00`
  3. Day1-V3 已拿到新的 `Town` 现场证据：
     - `Town / FreeTime / 19:30`
     - `Town / FreeTime / 20:00`
     - `NpcRoamSpikeStopgapProbe`
       - `19:30`=`26/26`
       - `20:00`=`26/25`
  4. `Force Spring Day1 Dinner Validation Jump` 仍只到 `FarmingTutorial / 16:00`
     - 晚饭入口自动链仍未打通
- 父层判断：
  1. `G-04/G-06` 已明显前进，不再只是 `opening partial`。
  2. 现在最实的剩余 blocker 是：
     - `DinnerConflict`
     - `19:00/21:00/次日`
     - `G-05 packaged`
     - 手工 profiler / 验收
  3. 用户汇报时不能再写“Town 入口还没打通”，也不能写“Packaged 前只差 dinner 一点点”；要如实报“Town 现场已补齐一大段，但全线终验仍未完成”。

## 2026-04-18｜父层补记：`21:00` 对 Day1 tracked resident 已补 probe，次日当前只到 `Home/DayEnd`
- 父层新增事实：
  1. Day1-V3 已把稳定链继续推到：
     - `Town / FreeTime / 21:00`
  2. `NpcRoamSpikeStopgapProbe` 在 `21:00` 看到 `16 / 16`，
     但随后 `spring-day1-actor-runtime-probe.json` 与 `spring-day1-resident-control-probe.json` 对 Day1 tracked resident 已为空。
  3. 连续推进到次日后，当前 settled snapshot 落到：
     - `Home.unity / DayEnd / 07:00 AM`
     - 这还不是 `Town` 侧 morning release
- 父层判断：
  1. `21:00` 这条不能简单按 `roam probe=16` 判成“Day1 resident 仍没隐藏”。
  2. 当前更准确的剩余 blocker应写成：
     - `DinnerConflict`
     - `Town` 侧次日释放
     - packaged / profiler / 最终验收

## 2026-04-18｜父层补记：Day1 现有菜单链已能安全到 `Town free-time`，但还不是“一键 fresh->20:00/profiler”
- 父层新增事实：
  1. 只读复核已确认：当前仓内现成菜单已经足够把 PlayMode 稳定带回 `Town` 并直接跳到 `free-time`：
     - `Restart Spring Day1 Native Fresh`
     - `Force Spring Day1 FreeTime Validation Jump`
     - `Write Spring Day1 Live Snapshot Artifact`
  2. 现成代码还支持把 `FreeTime 19:30` 往 `20:00` 再推进一拍：
     - `TimeManagerDebugger` 在 PlayMode 自动挂载
     - `+` 键会把 `19:30` 规范化推进到 `20:00`
     - `SpringDay1NpcCrowdDirector.ShouldResidentsReturnHomeByClock()` 只看时钟，所以这一步能触发 resident `return-home`
  3. 但当前仍不能把它包装成“fresh->20:00/profiler 一键自动化”：
     - `TriggerRecommendedAction()` 会在 `FarmingTutorial` 收口前停住，不会自动补“和村长说一声”这一步
     - `Day1` 现有菜单里也没有专门的 `Town profiler` 自动入口
  4. `Town Lead Transition Artifact` 误落 `Home` 的根因已从代码层钉死：
     - artifact 菜单先问 director 是否存在待处理 escort；若没有，再退回“按当前 activeScene 猜目标场景，猜不出来就取第一个 trigger”的 fallback
     - 而 director 自己的 validation transition 走的是按目标场景过滤过的 cached trigger
- 父层判断：
  1. `Package G` 当前最准确的口径，不再是“Town 根本进不去”，而是：
     - `Town/free-time/snapshot` 现有链已成立
     - `fresh->late-phase` 的纯 stepper 仍有缺口
     - `profiler` 仍需人工窗口或后续专门入口
  2. 因此后续如果继续用现有入口拿 `Town` 晚段 live 证据，应优先改走 `Native Fresh + LatePhase Jump + Snapshot/Probe`，不要再围着 `Town Lead Transition Artifact` 打转。

## 2026-04-18｜父层补记：`DinnerConflict` 当前卡点属于 editor 验收入口竞态，不是 runtime 主链先天缺失
- 父层新增事实：
  1. `SpringDay1LatePhaseValidationMenu.ForceDinnerValidationJump()` 先同步执行 `PreparePostTutorialExploreWindow()`，紧接着又立刻打 `TryRequestDinnerGatheringStart(true)` 和 fallback `ActivateDinnerGatheringOnTownScene()`。
  2. 但 `SpringDay1Director.EnterPostTutorialExploreWindow()` 在 `Primary` 会优先走 `SceneTransitionRunner.TryBlink(...)`，真实把 `_postTutorialExploreWindowEntered` 设稳的动作可能在菜单返回后才发生。
  3. 因而当前晚饭强跳的根因是：
     - editor 菜单没有等 explore-window 真正落稳
     - 不是 `DinnerConflict` runtime 主链本身缺失
  4. 现有 `SpringDay1LiveValidationRunner.TriggerRecommendedAction()` 也仍停在 `FarmingTutorial` 收口口径，不会自己补“和村长收口 -> explore window -> dinner request”。
- 父层判断：
  1. 这条 blocker 最小修口位应放在 `Assets/Editor/Story/SpringDay1LatePhaseValidationMenu.cs`。
  2. 安全方案是补 `editor-only` 延迟 helper，等待 explore-window/blink/town-load 落稳后，再复用现有 director 私有方法推进到 `DinnerConflict`；当前没有证据支持先改 runtime 主链。

## 2026-04-18｜父层补记：Day1-V3 已把 `0417` 收成终验板，当前剩余只落人工 packaged / profiler
- 父层新增事实：
  1. Day1-V3 已回写 `0417.md`：
     - `Package G` 状态改为 `待人工终验`
     - `C-02 / C-08 / D-02 / D-03 / D-08b` 已按当前真实状态回勾
     - 新增 `8.1 当前封板判断`
  2. 已新增 `0418_打包终验包.md`，把：
     - build 路径
     - 最少必测包
     - profiler spot check
     - 失败像什么
     - 最终回执
     统一收口
  3. Day1-V3 这轮亲自试了 command bridge 串联 `PLAY/RESET/STOP`，结果确认：
     - 桥不是坏了
     - 但当前机器上 `.cmd` 消费延迟是分钟级
     - 不适合再被包装成“稳定自动化套跑”
  4. Unity 当前已回到 `EditMode`
- 父层判断：
  1. 当前这条线不要再按“还有一堆 runtime 未做完”来理解。
  2. 更准确的状态是：
     - runtime / targeted / fresh live helper 已封板
     - 只剩人工：
       - packaged build 用户路径
       - Unity Profiler
  3. `B-02/B-03/E-03/C-07` 仍可留作结构债或信心缺口，但当前不再阻塞打包前最小闭环。
- 当前恢复点：
  1. 父层后续若再裁定 Day1，应优先看：
     - `0417.md`
     - `0418_打包终验包.md`
  2. 不要再把 Day1 当前状态描述成“还卡在 live helper/菜单入口”。

## 2026-04-18｜父层补记：`Primary 001` 等待提示再次漂移，根因在 `WorkbenchEscort` 合同扩写
- 父层新增事实：
  1. Day1-V3 已只读核实：
     - `Primary/Town` 被导演 resolve 到的 `001` 都是同类 runtime actor
     - 不是 scene 对象天然不同
  2. 真正偏移点在 `SpringDay1Director.TryHandleWorkbenchEscort()`：
     - 没有只复用 `TownHouseLead` 的等待合同
     - 被扩成 `escort wait + workbench target + idle placement + briefing/ready gate`
  3. 这条 drift 已回写到 `0417.md`：
     - `C-24a`
     - `I-21`
     - `C-08a`
- 父层判断：
  1. 这是 Day1 own 的合同漂移，不是“Primary 的 001 和 Town 不是同一种对象”。
  2. 下一刀如果继续，应先收 `WorkbenchEscort` 等待提示合同，不应再从 scene identity 这条假方向绕。
- 验证结果：
  - 纯只读审计；未改 runtime、未进 Unity。

## 2026-04-18｜父层补记：夜间 return-home 到站判定已对齐 formal-navigation，roam 节奏已改成 `0.5~5 / 3~8`
- 父层新增事实：
  1. Day1-V3 这轮已进入真实施工，并把 `20:00 到家不隐藏` 的核心坏口从 crowd 侧收了一刀：
     - `SpringDay1NpcCrowdDirector` 现在不只看 raw `HomeAnchor`
     - 还会对齐 `NPCAutoRoamController.TryResolveFormalNavigationDestination(...)` 返回的实际到站代理点
     - arrival 半径同步收成 `0.64`
  2. 这刀明确不是给 `101` 写特判，而是把 `101` 和其他 resident 一起拉回同一套到家收尾合同。
  3. Day1-V3 同时把现用人类 NPC 的 roam profile 改成：
     - `shortPause 0.5~5`
     - `longPause 3~8`
  4. `0417.md` 已同步回写这轮结论。
- 父层验证口径：
  1. `git diff --check` 通过
  2. `validate_script` 覆盖关键脚本均为 `owned_errors=0`
  3. 但当前 CLI 仍被外部现场压成 `external_red`
     - `DialogueChinese V2 SDF.asset` importer inconsistent result
     - Unity `stale_status`
  4. `Ready-To-Sync` 没撞到代码 blocker，而是卡在：
     - `.kiro/state/ready-to-sync.lock`
- 父层判断：
  1. 当前这刀能诚实表述成：
     - `结构已收`
     - `代码层 own red 无`
     - `packaged/live 仍待复测`
  2. 不能表述成：
     - `101 已实机过线`
     - `全员夜间体验已完成`

## 2026-04-18｜父层补记：`101` 夜间单点异常先收 authored truth，不写 runtime 特判
- 父层新增事实：
  1. Day1-V3 这轮继续只盯 `101`，重新对位了：
     - `SpringDay1NpcCrowdManifest.asset`
     - `Town.unity`
     - `101/102/103 prefab`
  2. 高置信静态结论是：
     - `101_HomeAnchor` 引用正常
     - prefab 结构没有明显 101 专属 runtime 缺口
     - 最硬差异落在 `101 DayEnd_Settle` 仍被 author 成 `Pressure + AmbientPressure`
  3. 已把 `101 DayEnd_Settle` 收回普通 resident：
     - `presenceLevel=2`
     - `flags=ReturnHome + KeepRoutine`
  4. `NpcCrowdManifestSceneDutyTests.CrowdManifest_ShouldExposeResidentSemanticMatrix_ForVillageResidentLayer` 已 direct run `succeeded`
- 父层判断：
  1. 这刀是 data truth 修正，不是 `101` 专用代码补丁。
  2. 当前最准确口径是：
     - `101` 的 authored special-case 已收掉
     - packaged/live 体感仍待用户复测

## 2026-04-18｜父层补记：Day1-V3 已补收 3 条 reopen，当前回到“代码层闭环成立、体验待用户复测”
- 父层新增事实：
  1. Day1-V3 已把 `0.0.6` explore window 的交互语义重新钉回普通 NPC 规则：
     - `NpcInteractionPriorityPolicy`
     - `NPCDialogueInteractable`
     - `NPCInformalChatInteractable`
     - `SpringDay1NpcCrowdDirector.ShouldDeferToStoryEscortDirector(...)`
     现在都不再把这段窗口继续按 `FarmingTutorial formal-priority` 整体压闸
  2. `Primary 001` 等待提示已不再扩另一套私链：
     - `TryHandleWorkbenchEscort()` 已收回 `TownHouseLead` 基线
     - `002` 等待时继续补位，不再和 `001` 一起冻结
  3. `20:00` 回家半态又补了一刀：
     - `NPCAutoRoamController.DriveResidentScriptedMoveTo(...)` 现在不会再把别的 travel contract 误当成 `FormalNavigation` 已接上
     - 如果 formal-navigation 起手失败且 owner 不是原本顶层 owner，会回滚临时 scripted-control
     - `SpringDay1NpcCrowdDirector` 新增 `QueueResidentReturnHomeRetry(...)`，避免首次起手失败就冻结成 pending 假接管
  4. `0417.md` 已同步回写：
     - `C-31 / C-32 / C-33`
     - `I-22 / I-23 / I-24`
     - `8.2` 新增 tasks
- 验证口径：
  1. `validate_script` 覆盖相关脚本均为 `owned_errors=0`
  2. fresh `errors`=`0 error / 0 warning`
  3. 但 `validate_script` assessment 仍统一停在 `unity_validation_pending`
     - 原因是外部 `stale_status`
     - 这不是 own red
- 父层判断：
  1. Day1 当前状态不能再写成“只剩 packaged/profiler 全人工终验”。
  2. 更准确的口径是：
     - 用户 fresh 反馈 reopen 了 3 条体验问题
     - Day1-V3 已把这 3 条重新收成代码层闭环
     - 现在重新回到“等用户 fresh live / packaged 复测”这个阶段

## 2026-04-18｜父层补记：疗伤前 `Home` 门禁与 `Primary 001` 气泡误杀已补收
- 父层新增事实：
  1. `PrimaryHomeDoor` 已正式接回 `HealingAndHP` 门禁：
     - 疗伤未完成时门禁关闭
     - 疗伤完成后门禁恢复
  2. 若坏档已经落进 `Home`，现在会走强制退回 `PrimaryHomeEntryAnchor` 的恢复链。
  3. `ApplyStoryActorRuntimePolicy(...)` 已不再无条件 `HideBubble()`：
     - `conversation-priority` 气泡现在不会被 storyActorMode 自己闪掉
  4. `0417.md` 已同步写入：
     - `I-25 / I-26`
     - `C-34 / C-35`
     - `G-05c`
- 父层验证口径：
  1. `SpringDay1Director.cs` `validate_script => errors=0 / warnings=3`
  2. `SpringDay1DirectorStagingTests.cs` `validate_script => errors=0 / warnings=0`
  3. 新增窄测 `4/4 passed`
  4. fresh console 已清到 `0 error / 0 warning`
- 父层判断：
  1. 这刀是打包前的小闭环，不是 final packaged 终验。
  2. 现在能诚实说的是：
     - 代码层已闭环
     - 体验仍需用户 fresh live / packaged 复测

## 2026-04-18｜父层补记：`Tool_002_BatchHierarchy` 的自动恢复锁定选择已不再制造 cross-scene / DDOL 警告
- 父层新增事实：
  1. 用户贴出的 own warning 栈已对到：
     - `Tool_002_BatchHierarchy.OnEnable() -> LoadPersistedSelection()`
  2. 当前工作树里的 `Tool_002_BatchHierarchy.cs` 已经是 scene-path + sibling-path 版本，不是旧 `GlobalObjectId` 版本。
  3. 这轮实际收的不是“对象引用链本身”，而是工具打开时自动恢复旧锁定选择这条噪声触发口：
     - 已移除 `OnEnable()` 里的自动 `LoadPersistedSelection()`
- 父层验证口径：
  1. `validate_script Assets/Editor/Tool_002_BatchHierarchy.cs` => `errors=0 / warnings=2`
  2. clear console 后重新执行菜单：
     - `Tools/002批量 (Hierarchy窗口)`
  3. fresh console 未再出现用户贴出的 cross-scene / `DontDestroyOnLoad` warning
- 父层判断：
  1. 这条属于支撑子任务，已经从 own warning 角度收住。
  2. 如果后续还有同类警告，就应回到具体 scene/runtime 引用本身查，不再先算到 `Tool_002_BatchHierarchy` 头上。

## 2026-04-18｜父层补记：两份 `Town` 退役菜单当前不像活跃 own warning
- 父层新增事实：
  1. 进一步静态对位后确认：
     - `TownSceneRuntimeAnchorReadinessMenu.cs`
     - `TownNativeResidentMigrationMenu.cs`
     当前都已经是退役后直接返回 deprecated blocker/result 的版本。
  2. 这两份脚本当前只会 `Debug.Log(...)` 和写结果 json，没有主动 `Debug.LogWarning(...)`。
  3. `validate_script` 对这两份脚本都是 `errors=0 / warnings=0`。
- 父层判断：
  1. 从当前源码真相看，这两份文件不像活跃 own warning 根因。
  2. 更像旧 console 残留、一次性触发过的退役菜单日志，或截图正文被折叠后只剩文件路径。
  3. 当前不建议为了这张模糊图盲改这两份退役工具。

## 2026-04-19｜父层补记：`0.0.4` 提前开戏、`001` 日常聊天缺入口、疗伤半径过紧与 walk-away cue 露出已补一轮代码闭环
- 父层新增事实：
  1. `TryHandleWorkbenchEscort()` 不再只因玩家先贴近工作台就提前开戏：
     - 现在必须 `leader ready + playerNearWorkbench`
     - leader-ready 半径本地放宽到不小于 `0.75f`
  2. 疗伤桥接近判定现在有 runtime floor：
     - serialized 仍为 `1.6`
     - 运行时有效半径不再小于 `2.4`
  3. `NPCInformalChatInteractable.BootstrapRuntime()` 不再跳过“已有 formal 的 NPC”：
     - `001/002` 这类 formal NPC 现在也会自动补 resident informal 入口
  4. `PlayerNpcChatSessionService.RunWalkAwayInterrupt()` 不再把内部 `reactionCue` 直接显示到 NPC 气泡里
  5. `0417.md` 已同步写入：
     - `C-36 / C-37 / C-38`
     - `I-27 / I-28 / I-29`
     - `C-08a-4 / C-08b-1 / C-08b-2 / C-01d / G-05d`
- 父层验证口径：
  1. `validate_script`：
     - `SpringDay1Director.cs` => `errors=0 / warnings=3`
     - `SpringDay1DirectorStagingTests.cs` => `errors=0 / warnings=0`
     - `PlayerNpcChatSessionService.cs` => `errors=0 / warnings=1`
     - `NPCInformalChatInteractable.cs` => `errors=0 / warnings=1`
  2. 定向 EditMode tests `3/3 passed`
  3. assembly 级 EditMode discovery 正常，但整仓仍有外部旧红，不计入本轮 own blocker
- 父层判断：
  1. 这轮不是 packaged 终验，而是 Day1 交互 reopen 的又一轮最小闭环。
  2. 当前最值钱的下一步，不是再扩结构，而是等用户 fresh live / packaged 复测这 4 条体验：
     - `001` 日常聊天
     - `0.0.4` 开场时机
     - 疗伤靠近 `002`
     - walk-away cue 露出

## 2026-04-19｜父层补记：Day1 任务壳残留不是单一 hide 漏口，而是“恢复前清场 + 恢复后重建”双层混口
- 父层新增事实：
  1. `SaveManager.ResetTransientRuntimeForRestore()` 已显式清：
     - 对话
     - 包裹/箱子/工作台
     - prompt/world hint/interaction hint
     - bubble
     - `Dialogue` / `SpringDay1Director` pause source
  2. 但 `StoryProgressPersistenceService.ApplySpringDay1Progress()` 恢复尾部又会重新：
     - `HideTaskListBridgePrompt()`
     - `RestoreLoadedDayEndTaskCardState()`
     - `SyncStoryTimePauseState()`
  3. `SpringDay1PromptOverlay` 与 `InteractionHintOverlay` 都不是“一次 hide 就永久关闭”的纯瞬时壳：
     - 前者 `LateUpdate()` 会继续按 `BuildPromptCardModel()` 重建正式任务卡；
     - 后者 `LateUpdate()` 会继续按 placement/context 逻辑重算提示卡。
  4. 默认开局/原生重开的当前定义本来就不是“空白 HUD”：
     - `ApplyNativeFreshRuntimeDefaults()` -> `ResetToTownOpeningRuntimeState()`
     - 结果就是 `Town + EnterVillage`
     - 所以 Day1 左侧正式任务卡会按现合同再次出现。
- 父层判断：
  1. 当前“第二天还挂 Day1 任务卡”的修口还不够。
  2. 真缺的不是更多 `Hide()` 调用，而是一个 authoritative 的“Day1 玩家面 UI 何时彻底失效”总闸。
  3. 这条总闸应优先收在 `SpringDay1Director`，而不是继续散落在 `SaveManager` 或 `PromptOverlay` 壳层做症状补丁。

## 2026-04-19｜父层补记：`0417` 角色改成参考 / 对账板
- 父层新增事实：
  1. 用户已明确裁定：
     - 当前项目真实代码、资产、测试与现场事实才是底板
     - 历史迭代语义主导
     - `0417` 只做参考 / 对账
  2. 子工作区已同步把 `0417.md` 顶层口径改掉，不再自称唯一主控板。
  3. `Phase 0` 已收成“参考板建立”，`M-02 / M-03 / M-04` 已明确成持续维护规则。
  4. `B-02 / B-03 / C-07 / E-03` 这类开放项，当前已被重新裁定为打包前冻结的结构债，不再因为清单未勾而自动扩大施工面。
- 父层判断：
  1. `spring-day1` 当前主线已经不是“继续做 Day1 结构重构”。
  2. 当前正确方向是：
     - 以真实代码与 fresh 验证为底板
     - 继续做打包前必要收尾
     - 用 `0417` 做对账、防漂移和收口说明

## 2026-04-19｜父层补记：spring-day1 own 批次已提交并脱离 own dirty
- 父层新增事实：
  1. `spring-day1` 本轮已提交 2 个 checkpoint：
     - `f9496b01 spring-day1: land day1 v3 runtime closeout batch`
     - `0d59b8b3 spring-day1: finish persistent restore guard tail`
  2. 当前 `git status --short -- spring-day1 own roots` 已 clean。
  3. `Ready-To-Sync` 本轮没有形成成功票，原因是流程工具侧先后撞到 stale lock 与 `CodexCodeGuard` 非 JSON 输出。
- 父层判断：
  1. 这条线当前已经不是“own 改动还没提交”的状态。
  2. 后续该看的重点重新回到：
     - 用户 fresh live / packaged 体验
     - 以及与其他线程成果的后续整合

## 2026-04-19｜父层补记：Town 退役菜单 dead-code 清理已收，`61/62` 条错误不是新系统面
- 父层新增事实：
  1. 用户追加的两条 `CS0162` warning 来自：
     - `TownSceneRuntimeAnchorReadinessMenu.cs`
     - `TownNativeResidentMigrationMenu.cs`
  2. `spring-day1` 已把这两处 `return` 后死代码收掉。
  3. 中途出现的 `61/62` 条编译错，根因是 `TownNativeResidentMigrationMenu.cs` 在第一次 patch 后结构失衡，属于本轮单文件事故，不是新的跨系统红面。
  4. 最新 `errors` fresh 已回到 `0 error / 0 warning`。
  5. `validate_script` 后续读到的 `missing script` 属于外部红，不归这两份退役菜单 own。
- 父层判断：
  1. 这轮 Town 退役菜单尾巴已经收平。
  2. 后续如果用户继续追红面，不应再把这两份文件当主嫌。

## 2026-04-19｜父层补记：最新 console 只剩 Unity MCP 包缓存外红
- 父层新增事实：
  1. 最新 `sunset_mcp.py errors` 结果是 `4 errors / 0 warnings`。
  2. 4 条 error 全部落在：
     - `Library/PackageCache/com.coplaydev.unity-mcp@.../Editor/Helpers/GameObjectSerializer.cs`
     - `Library/PackageCache/com.coplaydev.unity-mcp@.../Editor/Resources/Scene/GameObjectResource.cs`
  3. 对 `TownSceneRuntimeAnchorReadinessMenu.cs` 与 `TownNativeResidentMigrationMenu.cs` 再次做 `validate_script`，结果都是：
     - `owned_errors=0`
     - `owned_warnings=0`
  4. 因此当前这张快照里，Town 两份退役菜单不是活跃红源。
- 父层判断：
  1. “62 个报错”在当前现场不成立为 Day1 own 红面。
  2. 当前更像 Unity MCP / editor stale 状态造成的外部红，后续如果继续追，要换到包缓存与 editor 状态侧查。

## 2026-04-19｜父层补记：`PrimaryHomeDoor` 现在要锁到 `0.0.5`
- 父层新增事实：
  1. 用户最新裁定已把 `Home` 入口真值改成：
     - 整个 `0.0.4 WorkbenchFlashback` 期间继续锁门
     - 只有正式进入 `0.0.5 FarmingTutorial` 后才放开
  2. `SpringDay1Director.ShouldAllowPrimaryHomeEntry()` 已跟进收成：
     - `CurrentPhase >= FarmingTutorial`
  3. `SpringDay1DirectorStagingTests` 相关门禁断言也已同步改成：
     - `WorkbenchFlashback` 继续关闭
     - `FarmingTutorial` 才开放
  4. 最新脚本级验证：
     - `SpringDay1Director.cs`=`owned_errors=0`
     - `SpringDay1DirectorStagingTests.cs`=`owned_errors=0`
     - fresh console=`0 error / 0 warning`
- 父层判断：
  1. 这条门禁语义现在不要再按“疗伤后即可进家”理解。
  2. 后续用户如果复测 `PrimaryHomeDoor`，通过标准应直接改成：
     - `0.0.4` 还不能进
     - `0.0.5` 才能进

## 2026-04-23｜父层补记：`spring-day1` 已切到 shared-root 保本上传准备态
- 父层新增事实：
  1. 用户已给出新的治理 prompt，要求 `spring-day1` 线程暂停继续开发，改为只做当前本地 own 成果的完整保本上传。
  2. 线程已只读读取：
     - `2026-04-23_给spring-day1_shared-root完整保本上传与own尾账归仓prompt_01.md`
     - `2026-04-23_shared-root完整保本上传分发批次_01.md`
     - 当前 live 规范快照
  3. 当前执行顺序已冻结为：
     - 先重看 prompt 点名 own 簇
     - 先做 `A/B/C` 分类
     - 明确不吞 `Story/UI`、`SaveManager`、scene、`ProjectSettings`、`GameInputManager`
     - 两个等待中的子智能体这轮默认不直接参与上传
  4. 线程本轮还未开始真实上传，因此 `Begin-Slice / Ready-To-Sync` 仍未跑；当前仍是只读准备态。
- 父层判断：
  1. 这轮完成定义不再是“继续把 Day1 功能做完”，而是“把 clearly-own 成果安全归仓或精确 blocker 化”。
  2. 后续若继续执行，应优先保证 own 分类与白名单边界正确，不能为了提速回流到 runtime 修补。
