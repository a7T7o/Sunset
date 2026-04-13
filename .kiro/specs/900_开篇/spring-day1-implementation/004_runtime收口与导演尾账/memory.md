# 004 - runtime 收口与导演尾账

## 模块概述
- 本阶段只承接当前 `spring-day1` 真正还活着的部分：
  - 导演尾账收口
  - runtime 黑盒终验
  - `Town / Home / UI / NPC` contract 吃回
  - 当前 live 语义边界确认

## 当前状态
- **最后更新**：2026-04-10
- **状态**：新活跃阶段已建立

## 当前四条工作面
1. `005` 后收口对白 -> 傍晚自由活动 -> 晚饭切入链的最终黑盒终验
2. `0.0.5` 农田教学收口、formal/informal override、输入恢复等导演尾差复核
3. `Town / Primary / Home` 跨场景承接后的主线 contract 吃回
4. 已经明确切给公共链的问题继续维持边界，不再回拉进 `day1` 语义，例如 placement 公共链

## 当前稳定结论
- `day1` 主线代码链已经形成，不再是“主线没搭起来”
- 当前最值钱的动作不是再开新剧情，而是把 runtime 和导演尾差收平
- 任何新的 live 问题都应先判断：
  - 是导演尾差
  - 是跨场景承接
  - 还是已经不属于 `day1`，应切给公共链

## 当前恢复点
- 本阶段后续可直接承接当前 `spring-day1` 的 live 问题
- 旧阶段历史材料、分场 prompt 和协作文档不再往这里复制，只在需要时回查 `003/memory_0.md`

## 2026-04-10｜NPC 自漫游抽搐 + Package 打开卡顿第一刀
- 用户目标：
  - 不再分发给其他线程，由 `spring-day1` 自己彻查并落地两件事：
    - `NPC` 自漫游时摇头晃脑、像陀螺一样乱转
    - 打开 `Package UI` 时的明显卡顿
- 这轮实际做成：
  - `InventoryPanelUI` 收掉打开面板时的重复整面刷新：
    - `EnsureBuilt()` 改成 `首次/重绑只重绑，稳定态只刷新数据`
    - `OnEnable()` 不再在 `EnsureBuilt()` 后再走一轮整面 `RefreshAll()`
  - `InventorySlotUI` 不再让每个槽位在 `Awake/Bind` 里各自 `FindFirstObjectByType<HotbarSelectionService>()`
    - 改成由 `InventoryPanelUI / BoxPanelUI` 把 runtime context 直接传进来
  - `EquipmentSlotUI.Bind()` 收掉手工 `OnDisable()->OnEnable()` 触发的重复刷新，改成最小重绑 + 单次 `Refresh/RefreshSelection`
  - `BoxPanelUI` 同步改口，给 `InventorySlotUI` 传递现成的 `_hotbarSelection` / panel context
  - `NPCAutoRoamController` 收掉 roam 朝向直接跟随局部避让噪声的问题：
    - move 仍按真实 `velocity`
    - facing 改成优先吃“路径目标方向 `moveDirection`”，不是每帧都跟 `adjustedDirection` 抖动
  - `NPCMotionController` 的外部朝向最短保持时间从 `0.16s` 提到 `0.22s`，让四方向动画切面更稳
- 关键判断：
  - UI 卡顿的根因不是“开 UI 天生卡”，而是打开时存在“重复刷新 + 逐槽全局找服务”的放大链
  - NPC 抽搐的根因不是“必须牺牲导航质量”，而是 roam 表现层把局部避让噪声当成了真实朝向真源
- 涉及文件：
  - `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
  - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
  - `Assets/YYY_Scripts/UI/Inventory/EquipmentSlotUI.cs`
  - `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
- 验证结果：
  - `validate_script` 覆盖上述 UI/NPC 脚本：`owned_errors=0`，但 `codeguard` 在 `dotnet` 侧超时降级
  - `git diff --check` 对本轮 touched files 通过，仅剩既有 `CRLF/LF` warning
  - `recover-bridge` 后 `baseline=pass`
  - 但 MCP 当前 `instances=0 / no_unity_session`，所以这轮拿不到真正的 live PlayMode 证据
- 当前恢复点：
  - 代码层第一刀已落地，可继续等 Unity 实例真正注册后做黑盒复测
  - 若用户继续实机测，优先看：
    - `Town` 中 NPC 自漫游是否仍像陀螺一样乱转
    - 首次/再次打开 `Package` 是否仍出现明显尖峰

## 2026-04-10｜Profiler 只读复核结论补记
- 用户追加了一组 profiler 截图，要求先只读审核后再决定下一刀。
- 当前结论分三层：
  1. 多张主图重复命中 `NPCAutoRoamController.Update()`：
     - 单帧约 `1060ms ~ 1170ms`
     - 占比约 `95%`
     - 下挂 `Physics2D.OverlapCircleAll ≈ 27ms ~ 31ms`
     - 下挂 `GC.Alloc ≈ 11ms ~ 17ms`
     - 调用量约 `34 万` 级
     - 说明当前 demo 级灾难源仍然是 `NPC` 自漫游坏 case，不是偶发噪声
  2. “打开 UI”那张图当前选中的是 `EditorLoop ≈ 90.78ms`
     - 该帧不能直接当成 runtime UI 主因证据
     - 但图中确实暴露了 `PersistentPlayerSceneBridge / SpringDay1ProximityInteractionService / CameraDeadZoneSync` 的逐帧 `FindObjectsOfType` 尾账
  3. 另有一张图命中 `GameInputManager.Update() ≈ 1082ms`
     - 同样带着 `Physics2D.OverlapCircleAll` 和 `GC.Alloc`
     - 说明除 `NPC` 漫游外，输入/交互检测链也存在一条坏 case，但当前重复度和主占比仍以 `NPC` 为先
- 当前恢复点：
  - 若继续做性能，优先级应是：
    1. `NPCAutoRoamController`
    2. `GameInputManager` 交互检测坏 case
    3. UI 打开链里的逐帧 `FindObjectsOfType` 尾账

## 2026-04-10｜用户最新 live 纠正
- 用户最新实测回执：
  - `UI` 打开卡顿已明显缓解，当前不再是第一问题
  - `NPC` 抽搐仍在，但语义已进一步钉死：
    - 不是“往回走”
    - 不是“路径左右横跳”
    - 而是“位移方向持续正确，例如一直向左移动，但朝向表现一帧左、一帧右来回翻”
- 这条纠正意味着：
  - 当前 `NPC` 问题应收敛为 `facing / anim direction ownership` 竞争，不再优先怀疑路径本身
  - 下一刀应只盯：
    - `NPCMotionController` 朝向真源
    - `NPCAutoRoamController` 提交给 motion 的 facing
    - 动画层是否仍在吃 observed velocity / contact noise

## 2026-04-10｜放置模式语义补口与 UI 接线前置
- 用户新增明确裁定：
  - `0.0.5` 农田教学里，玩家必须被明确提醒“先按 V 开启放置模式”
  - 不能要求玩家靠试错去发现 `PlacementMode`
  - 这条提醒属于 `day1 -> UI` 的语义接线，不应由 UI 自己重写剧情判断
- 本轮已完成：
  - 在 [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 新增：
    - `ShouldShowPlacementModeGuidance()`
    - `GetPlacementModeGuidanceText()`
  - `0.0.5` 的任务清单、focus text、进入 farming tutorial 时的 prompt overlay 文案，已统一改口为：
    - 开垦前提示 `按 V 开启放置模式`
    - 播种/浇水阶段提示 `保持放置模式开启`
  - 已把给 UI 的续工 prompt 落到：
    - [2026-04-10_UI线程_005放置模式状态提示与引导提示接线prompt_09.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/2026-04-10_UI线程_005放置模式状态提示与引导提示接线prompt_09.md)
- 同轮补记：
  - `DebugSkipWorkbenchPhase05` 的 EditorPrefs 已强制关成 `0x0`，全流程测试不会再默认跳过到工作台
- 验证：
  - `validate_script SpringDay1Director.cs`：`assessment=unity_validation_pending`，`owned_errors=0`
  - `git diff --check -- SpringDay1Director.cs`：通过
  - Unity live 仍待用户 / 活实例终验，因为当前 CLI 看见 `No active Unity instance`
- 当前恢复点：
  - `day1` 这边已把“005 时该不该提醒玩家开 PlacementMode”收成稳定真值
  - UI 下一刀只该消费这套真值并做玩家面表现，不该再复制一套剧情判断

## 2026-04-10｜用户对导航主诉的统一判断补记
- 用户再次明确主诉没有变：
  - `NPC` 必须像玩家一样把静态世界走明白
  - 不应在 anchor 周围乱撞树、乱撞围栏
  - 不应为了止血卡顿而牺牲正常导航功能
- 当前综合判断：
  1. 这不是“性能与正确性天然冲突”，而是 `NPC` 导航 contract 本身没收平；
  2. `player` 与 `NPC` 当前并不是同一条静态导航 driver：玩家更接近“单目标 + 静态世界 + 明确 stop 距离”的链，而 `NPC` roam 叠了随机采样、局部避让、卡住恢复、重建路径、外部脚本抢朝向/抢停走等多层行为；
  3. 当前坏体验至少分两层：
     - `表现层`：多 owner 抢写朝向，导致“位移对，但脸左右翻”
     - `行走层`：roam 目的地采样 / 可达性判断 / 近距阻挡处理没有像玩家那样稳定收敛，导致在静态障碍边也会乱撞、反复重试；
  4. 封闭养殖区不是无解场景，真正的问题是：`NPC` 在封闭区域里没有被约束到一个稳定、可达的 roam domain，坏 case 下会持续重采样和碰撞查询，最终把 `NPCAutoRoamController.Update` 打爆。
- 当前对导航线程的正确要求不应只是“继续削尖峰”，而应是：
  - 统一 `NPC` 与玩家的静态世界理解
  - 统一朝向/移动 owner
  - 不可达目标尽早失败，不再 storm
  - 封闭区域/养殖区应有稳定的局部 roam 约束，而不是靠碰撞后补救

## 2026-04-10｜只读审计：Day1 存档回退 bug 的最安全测试切口
- 用户目标：
  - 只读核查 `StoryProgressPersistenceService`、相关 tests、以及 `SpringDay1Director` 里 completed/dialogue/private flags 的恢复关系；
  - 回答哪里仍在按 `phase` 派生导演私有态、最该补到哪个测试文件、以及最小但有价值的回归测试应断言什么。
- 本轮执行：
  - 未进入真实施工；未跑 `Begin-Slice`
  - 只读核对了：
    - `StoryProgressPersistenceService.cs`
    - `SpringDay1Director.cs`
    - `StoryProgressPersistenceServiceTests.cs`
    - `SpringDay1DialogueProgressionTests.cs`
    - 当前 `004_runtime` 工作区 memory
- 本轮稳定结论：
  1. `completedDialogueSequenceIds` 会先恢复进 `DialogueManager`，但随后 `ApplySpringDay1Progress(...)` 仍会按 `currentPhase` 回填导演私有态，所以 load 链上仍存在“phase 派生 director flags”的单独风险面。
  2. 当前最直接的 phase 派生点在：
     - `StoryProgressPersistenceService.cs:236-248`：先 `ReplaceCompletedSequenceIds(...)`，后 `ApplySpringDay1Progress(...)`
     - `StoryProgressPersistenceService.cs:478-485`：`_villageGateSequencePlayed / _houseArrivalSequencePlayed / _healingStarted / _healingSequencePlayed / _workbenchOpened / _workbenchSequencePlayed / _dinnerSequencePlayed / _returnSequencePlayed`
     - `StoryProgressPersistenceService.cs:548-575`：`_staminaRevealed / _freeTimeEntered / _freeTimeIntroCompleted / _freeTimeIntroQueued / _dayEnded`
  3. `SpringDay1Director` 当前 `HasCompletedDialogueSequence(...)` 已只读 `DialogueManager.HasCompletedSequence(...)`，不再按 phase 回写 completed；`TryRecoverConsumedSequenceProgression(...)` 也是按 completed sequence 走恢复，不是按 phase 直接补 consumed。
  4. 现有最适合补这条回归的文件仍是 `StoryProgressPersistenceServiceTests.cs`，因为它已经有完整 `Load(savedData)` runtime harness；`SpringDay1DialogueProgressionTests.cs` 目前只是源码合同级 guard，不是 save/load 行为回归位。
- 最安全测试切口：
  - 直接在 `StoryProgressPersistenceServiceTests.cs` 新增一个“刻意制造 phase 与 completed 不一致”的 load 回归；
  - 不要依赖 `Save()` 先产出一致快照，否则测不到 `phase 偷推 director private flags` 这条风险。
- 最小但有价值的断言：
  - `Load(...)` 后，`DialogueManager` 里未完成的 sequence 仍未完成；
  - 且对应导演 one-shot 私有位也不能因为 `phase` 被偷偷补成 `true`；
  - 至少应覆盖一组中后段位，例如：
    - `storyPhase = FreeTime`，`completedDialogueSequenceIds` 不含 `spring-day1-dinner / spring-day1-reminder / spring-day1-free-time-intro`
    - 断言 `_dinnerSequencePlayed == false`
    - 断言 `_returnSequencePlayed == false`
    - 断言 `_freeTimeIntroQueued == false`
    - 断言 `_freeTimeIntroCompleted == false`
- 当前恢复点：
  - 若后续真开刀，优先补这条 `StoryProgressPersistenceServiceTests` 行为回归；
  - 补完后再决定 `ApplySpringDay1Progress(...)` 是否需要把 one-shot director flags 从“phase 派生”改成“只认 completed/snapshot 真值”。

## 2026-04-10｜夜间 resident 回锚 / 次日恢复只读审计
- 用户目标：
  - 不改文件，只回答 `spring-day1` 当前最安全的“21:00 后 resident 回 HomeAnchor 站定不动、次日再恢复白天位置/漫游”的切入点。
- 当前结论：
  1. 现成“回 HomeAnchor”的入口已经有，但现成“回去后保持不动”的入口还没有一键版：
     - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 `SnapResidentsToHomeAnchors()` / `SnapResidentsToHomeAnchorsInternal()` 会把 resident 拉回 `HomeAnchor`、`StopRoam()`、`StopMotion()`、重设 `BaseFacing`；
     - 但它在末尾又会 `StartRoam()`，所以它现在是“瞬移回家后继续跑”，不是“回家站定”；
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 的 `AcquireResidentScriptedControl(...)`、`HaltResidentScriptedMovement(...)`、`StopRoam()` 才是现成的“冻结 resident 逻辑”入口。
  2. 现成“次日恢复白天位置/漫游”的最安全基础设施不是重新采样，而是 snapshot：
     - `SpringDay1NpcCrowdDirector.CaptureResidentRuntimeSnapshots()` / `ApplyResidentRuntimeSnapshots(...)`
     - `NPCAutoRoamController.CaptureResidentRuntimeSnapshot()` / `ApplyResidentRuntimeSnapshot(...)`
     - `PersistentPlayerSceneBridge` 已经在场景切换时消费这套 contract，说明“存白天位置 -> 夜里停锚 -> 次日还原”这条数据面是现成的。
  3. 当前会重新 `StartRoam()` 的关键位置有：
     - `SpringDay1NpcCrowdDirector.ApplyResidentBaseline(...)`
     - `SpringDay1NpcCrowdDirector.SnapResidentsToHomeAnchorsInternal()`
     - `SpringDay1NpcCrowdDirector.FinishResidentReturnHome(...)`
     - `SpringDay1NpcCrowdDirector.CancelResidentReturnHome(..., resumeRoam: true)`
     - `SpringDay1Director` 里 story actor 从导演控制释放时的 `ReleaseResidentScriptedControl(...) + StartRoam()`
     - `NPCAutoRoamController` 自身的 `Start()`、`Update()` 自动恢复、`ReleaseResidentScriptedControl(...)`、`ClearResidentScriptedControl(...)`、`ApplyResidentRuntimeSnapshot(..., resumeResidentLogic: true)`
- 当前最安全的切入点：
  1. **时间触发层**：优先放在 [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的 `HandleHourChanged(int hour)`。
     - 原因：当前 `day1` 里只有这层已经稳定订阅 `TimeManager.OnHourChanged`，并且已经在做晚饭 / 自由时段 / 睡觉压力的时间裁定；
     - 这里最适合只做“21:00 进入夜间 resident freeze / 次日早上解除 freeze”的状态翻转与 snapshot capture/apply 触发。
  2. **真正的收口层**：优先放在 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 `ApplyResidentBaseline(...)`，或者一个只由它调用的新 helper。
     - 原因：这里是 crowd resident 每轮 sync 后最终决定 `SetHomeAnchor / ApplyProfile / StartRoam / BaseFacing` 的统一 choke point；
     - 不先在这里加夜间 rule，任何“先 snap 再 stop”的外部调用，都会被后续 baseline sync 或 scene restore 重新开 roam 冲掉。
- 当前最容易和剧情导演 owner 冲突的地方：
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的 `ApplyStoryActorMode(...)`：它会 `AcquireResidentScriptedControl(...)`，释放后又可能立刻 `StartRoam()`；
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的 `DriveResidentScriptedMoveTo(...)` / `HaltStoryActorNavigation(...)`：剧情移动期间会抢 resident 的 movement owner；
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 `ApplyResidentBaseline(...)`：它本身就会在非剧情 hold 状态下重开 roam；
  - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs) 的 resident snapshot restore：跨场景恢复时会重放 resident 运行态，如果夜间冻结态没有并入 snapshot 语义，scene load 后也会被冲掉。
- 当前恢复点：
  - 如果下一轮真开刀，最稳的做法不是“再找一个地方强行 StopRoam”，而是：
    1. `SpringDay1Director.HandleHourChanged` 负责切夜间状态 + 记录白天 snapshot；
    2. `SpringDay1NpcCrowdDirector.ApplyResidentBaseline` 负责夜间一律回锚并禁止重开 roam；
    3. 次日早上优先走现成 snapshot restore 恢复白天位置和 roam 状态。

## 2026-04-10｜真实施工：Day1 时间守门 + 夜间 resident 站定 + 存档回退 phase 防偷推
- 当前主线：
  - `spring-day1 / 004_runtime收口与导演尾账`
  - 用户要求把 `9:00 开局`、`005 前时间上限`、`21:00 后 NPC 回锚站定且次日恢复`、以及“存档回退后不再被 phase 偷推进”的 runtime 尾账一起落下。
- 本轮真实施工：
  1. **Day1 时间守门已落到正式代码链**
     - [SaveManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs)
     - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
     - 现状：
       - fresh/native 开局默认时间改成 `09:00`
       - `TownOpeningHour` 已统一到 `9`
       - `SpringDay1Director` 新增 Day1 时间守门，运行态会把 `Year 1 / Spring / Day 1` 以及当前 phase 对应时段钳回合法范围
       - `EnterPostTutorialExploreWindow()` 进入傍晚自由活动时会把时间至少推进到 `16:00`
       - packaged 调试器现在会先问 `SpringDay1Director.TryNormalizeDebugTimeTarget(...)`，不再在 Day1 早段直接 `Sleep()/SetTime()` 把剧情跳烂
       - 非法提前跨天会被 `HandleSleep()` 拦下，并回钳到当前 phase 允许时段
  2. **夜间 resident 站定链已收进 CrowdDirector**
     - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - 现状：
       - `SpawnState` 新增 `IsNightResting`
       - `Update()` 新增 `SyncResidentNightRestSchedule()`
       - 夜间规则当前按 `hour >= 21 || hour < 9`
       - 夜间态会把 resident 拉回 `HomeAnchor`、停掉 roam/motion、保留基础朝向，并阻断：
         - `ApplyResidentBaseline(...)` 里的 `StartRoam()`
         - `SnapResidentsToHomeAnchorsInternal()` 末尾重开 roam
         - `FinishResidentReturnHome(...)` 的自动恢复 roam
         - `CancelResidentReturnHome(..., resumeRoam: true)` 的夜间回流
       - 额外补了一层 parent 保险：夜间 resident 默认回到 `Resident_DefaultPresent`，避免停在 backstage/隐藏 parent 里
  3. **存档回退 phase 偷推进已去掉最危险派生**
     - [StoryProgressPersistenceService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs)
     - [StoryProgressPersistenceServiceTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs)
     - 现状：
       - `ApplySpringDay1Progress(...)` 不再按 `currentPhase` 自动把：
         - `_dinnerSequencePlayed`
         - `_returnSequencePlayed`
         - `_freeTimeIntroCompleted`
         - `_freeTimeIntroQueued`
         补成已消费
       - 新增专门回归测试：
         - `Load_DoesNotPromoteLateDayPrivateFlagsFromPhaseAlone`
         - 覆盖 `storyPhase = FreeTime` 但 `completedDialogueSequenceIds` 不含 `dinner / reminder / free-time-opening` 时，load 后这些 completed 与导演私有位仍必须保持 `false`
- 本轮验证：
  - `validate_script`：
    - `SpringDay1Director.cs + TimeManagerDebugger.cs + SaveManager.cs` => `assessment=unity_validation_pending`，`owned_errors=0`
    - `SpringDay1NpcCrowdDirector.cs` => `assessment=unity_validation_pending`，`owned_errors=0`
    - `StoryProgressPersistenceService.cs + StoryProgressPersistenceServiceTests.cs` => `assessment=unity_validation_pending`，`owned_errors=0`
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 20` => `errors=0 warnings=0`
  - 相关 `git diff --check`：通过
- 当前 blocker / 未收口：
  - Unity live 仍是 `instances=0 / unity_validation_pending`，所以这轮只能 claim `代码层无 owned red + console 0`，不能 claim “live 全链路已过”
  - [StoryProgressPersistenceService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs) 与 [StoryProgressPersistenceServiceTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs) 当前在工作树里是 `untracked`，本轮还没进入 sync/收口
  - 用户要求“本轮先完成原主线，不因 UI 回执中断”；因此 [2026-04-10_UI线程_给spring-day1阶段回执_34.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/2026-04-10_UI线程_给spring-day1阶段回执_34.md) 尚未正式纳入本轮施工
- 当前恢复点：
  - 下一轮优先做 live 黑盒复判：
    1. fresh/native 开局是否真从 `09:00` 进入
    2. `005` 前调试跳时是否被稳稳钳在 `<=16:00`
    3. 傍晚自由活动是否能从 `16:00` 自然走到 `19:00` 或与村长对话直开晚饭
    4. `21:00` 后 resident 是否都在各自 anchor 站定、睡醒后能否从 anchor 再恢复白天态
    5. 旧档 load 回退时，`dinner / reminder / free-time-opening` 是否不再因为 phase 被偷判已消费

## 2026-04-11｜fresh 桥接复判：当前不能宣称“全部完成”
- 本轮先把 `spring-day1` 的 thread-state 从 `ACTIVE` 合法收到了 `PARKED`，因为当前工作已经转为只读总审，不再继续保持施工态。
- 随后补跑了 3 组 fresh targeted bridge tests，结果明确说明：`day1` 当前还没有达到“你前面历史 prompt 全部完成到位”的标准。
- fresh 结果：
  1. [spring-day1-opening-bridge-tests.json](D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/spring-day1-opening-bridge-tests.json)
     - `success=false`
     - 关键失败：
       - `VillageGateCompletionInTown_ShouldPromoteChiefLeadState`
       - `VillageGateWhileActiveInTown_ShouldKeepPostEntryBeatUntilDialogueCompletes`
     - 说明 opening 段当前仍存在 `村长带路 / gate 收束` 的 runtime contract 问题，不是全绿。
  2. [spring-day1-midday-bridge-tests.json](D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/spring-day1-midday-bridge-tests.json)
     - `success=false`
     - 关键失败：
       - `FarmingTutorialCompletion_ShouldImmediatelyBridgeIntoDinnerConflict`
         - 农田教学结束后仍停在 `FarmingTutorial`，没有立刻切进 `DinnerConflict`
       - `WorkbenchCompletion_ShouldAdvanceIntoFarmingTutorial`
         - 这是文案断言过期；当前预期旧文案仍写“先用锄头开垦一格土地”，而现在 runtime 已改成“按 V 开启放置模式，再用锄头开垦一格土地”
     - 说明中段不是只差验收，而是至少还存在 1 条真实未闭环推进链。
  3. [spring-day1-late-day-bridge-tests.json](D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/spring-day1-late-day-bridge-tests.json)
     - `success=false`
     - 关键失败：
       - `BedBridge_EndsDayAndRestoresSystems`
       - `FreeTimeValidationStep_AdvancesFromFinalCallToDayEnd`
         - 两者都在 `HandleSleep()` 里打到 `SceneTransitionRunner.EnsureInstance()`，并在 EditMode 触发 `DontDestroyOnLoad` 异常
       - `DayEndPlayerFacingCopy_ShouldCarryTomorrowBurdenAndClearWorkbenchState`
         - 当前仍拿到“等待自由时段见闻接管”，没有进入预期的 DayEnd 文案态
     - 说明晚段/睡觉收束这条链也还没有闭环。
- 本轮新结论：
  - 代码层主刀不是白做：
    - fresh/native `09:00` 快照仍成立
    - `005` 前时间守门、`21:00` 夜间 resident、存档 `phase` 防偷推这些代码都在现场
  - 但按照用户真正的完成定义，现在只能说：
    - `结构推进成立`
    - `桥接与晚段 runtime 仍有真实未闭环项`
    - `绝不能对用户说“前面那一大轮全部都完成了”`
- 当前最值钱的恢复点已经收缩成 4 条：
  1. opening：`VillageGate -> chief lead` 收束 contract
  2. midday：`FarmingTutorial -> DinnerConflict` 立即桥接
  3. late-day：`HandleSleep -> SceneTransitionRunner` 的 test/runtime 双场景兼容
  4. late-day：`FreeTime / DayEnd` 的最终任务文案与状态推进

## 2026-04-11｜只读定位：late-day bridge failures 最小修法与修改面
- 用户目标：
  - 只读回答 `SpringDay1LateDayRuntimeTests` 里这 3 条 latest fresh failure 的真正原因、最小正确修法、以及应该改哪些方法/测试：
    - `BedBridge_EndsDayAndRestoresSystems`
    - `FreeTimeValidationStep_AdvancesFromFinalCallToDayEnd`
    - `DayEndPlayerFacingCopy_ShouldCarryTomorrowBurdenAndClearWorkbenchState`
- 本轮执行：
  - 未进入真实施工；未跑 `Begin-Slice`
  - 只读核对了：
    - `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
    - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
    - `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
    - `Library/CodexEditorCommands/spring-day1-late-day-bridge-tests.json`
- 本轮稳定结论：
  1. 前两条失败的最小正确修法应落在 `SceneTransitionRunner` 本体，不应散落到 `HandleSleep()` 之类的 callsite。
     - 当前异常根因很直接：
       - `SceneTransitionRunner.TryBlink()` -> `EnsureInstance()` -> `DontDestroyOnLoad(runnerObject)`
       - EditMode test 环境下这里会抛 `DontDestroyOnLoad can only be used in play mode`
     - 最小正确修法：
       - 在 `SceneTransitionRunner.EnsureInstance()` 与 `SceneTransitionRunner.Awake()` 里，把 `DontDestroyOnLoad(...)` 包到 `Application.isPlaying` 条件内；
       - 这样 PlayMode/runtime 仍保留持久 runner，EditMode tests 则退回“普通 scene 内临时 runner”。
     - 为什么不建议只在 `SpringDay1Director.HandleSleep()` 加 `if (!Application.isPlaying)`：
       - 因为当前 `SceneTransitionRunner` 还被 `Primary` 承接、`postTutorialExploreWindow` 等其他 blink 链复用；
       - 只修 `HandleSleep()` 会留下同类 EditMode 入口继续爆。
  2. `DayEnd` 文案/状态仍停在“等待自由时段见闻接管”，不是 `DayEnd` 文案写错，而是测试根本没进入 `DayEnd`。
     - 直接链路：
       - `GetCurrentProgressLabel()` 在 `phase == FreeTime && !_freeTimeIntroCompleted` 时固定返回 `FreeTimeIntroPendingProgressText`
       - `HandleSleep()` 进入 `DayEnd` 前，先走 `CanFinalizeDayEndFromCurrentState(...)`
       - 该 gate 要求：`CurrentPhase == FreeTime && _freeTimeEntered && _freeTimeIntroCompleted && !_dayEnded`
     - 这条失败测试当前只设置了：
       - `_freeTimeEntered = true`
       - `_staminaRevealed = true`
       - workbench crafting state
       - `StoryManager.ResetState(FreeTime, false)`
     - 但它没有设置 `_freeTimeIntroCompleted = true`，也没有通过 `FreeTimeIntroSequenceId` 收束 intro；
       - 因此 `HandleSleep()` 会走 `RecoverFromInvalidEarlySleep(...)` 直接返回；
       - phase 仍停在 `FreeTime`
       - `GetCurrentProgressLabel()` 继续返回“等待自由时段见闻接管”
     - 这说明第 3 条 failure 目前更像“测试前提不满足当前正式语义”，不是 `HandleSleep()` 把 `DayEnd` 文案写坏了。
  3. 当前最该改的测试面很明确：
     - `FreeTimeValidationStep_AdvancesFromFinalCallToDayEnd`
     - `BedBridge_EndsDayAndRestoresSystems`
       - 语义本身没错，主问题是被 `SceneTransitionRunner` 的 EditMode `DontDestroyOnLoad` 异常打断；
       - 这两条测试在修完 `SceneTransitionRunner` 之后应先原样复跑。
     - `DayEndPlayerFacingCopy_ShouldCarryTomorrowBurdenAndClearWorkbenchState`
       - 需要把 setup 改成合法的 `sleep-ready FreeTime`：
         - 至少补 `_freeTimeIntroCompleted = true`
         - 更稳的是同时对齐到 “自由时段 intro 已完成” 的正式入口语义，再触发 `HandleSleep()` / 睡觉桥接
       - 当前不应靠放宽 `HandleSleep()` gate 去迁就这条测试，因为同文件其他 late-day 测试已经明确把“intro pending 时不能立刻睡”当成正式语义的一部分。
- 当前恢复点：
  - 若下一轮真开刀，先修：
    1. `SceneTransitionRunner.EnsureInstance()`
    2. `SceneTransitionRunner.Awake()`
  - 然后复核是否需要额外在 `SpringDay1LateDayRuntimeTests.TearDown()` 补 `SceneTransitionRunner` 清理，避免 EditMode runner 跨测试残留
  - 再单独校正 `DayEndPlayerFacingCopy_ShouldCarryTomorrowBurdenAndClearWorkbenchState` 的 setup，不要反向放宽 `day1` 正式语义

## 2026-04-11｜只读定位：opening bridge 两条 fresh failure 的最小修复建议
- 用户目标：
  - 只读审查 `SpringDay1OpeningRuntimeBridgeTests` 里 opening 两条 fresh failure：
    - `VillageGateCompletionInTown_ShouldPromoteChiefLeadState`
    - `VillageGateWhileActiveInTown_ShouldKeepPostEntryBeatUntilDialogueCompletes`
  - 不改代码，只回答真因、最小应改点、测试是否该改预期。
- 本轮执行：
  - 未进入真实施工；未跑 `Begin-Slice`
  - 对位核查了：
    - `SpringDay1OpeningRuntimeBridgeTests.cs`
    - `SpringDay1Director.cs`
    - fresh 结果文件 `spring-day1-opening-bridge-tests.json`
- 本轮稳定结论：
  1. `VillageGateCompletionInTown_ShouldPromoteChiefLeadState` 当前失败的真因不是“Town 围观收束后没有进入带路态”，而是测试夹具没有把 `001` 做成可被导演识别的 NPC actor。
     - fresh 现场实际值是：
       - `started=True`
       - `queued=False`
       - `waiting=False`
       - `chief=missing`
       - `target=SceneTransitionTrigger`
     - 说明 `HandleDialogueSequenceCompleted(...)` 已经把带路态和目标 trigger 接起来了；
     - 真正丢的是 `GetTownHouseLeadSummary()` 里对 chief actor 的解析。
     - 当前 `ResolveStoryChiefTransform() -> FindPreferredStoryNpcTransform(...) -> IsLikelyNpcActorTransform(...)` 只认：
       - `NPCAutoRoamController`
       - `NPCMotionController`
       - `NPCDialogueInteractable`
       - `NPCInformalChatInteractable`
     - 但测试只创建了一个裸 `GameObject(\"001\")`，所以 summary 里会回 `chief=missing`。
  2. `VillageGateWhileActiveInTown_ShouldKeepPostEntryBeatUntilDialogueCompletes` 是真实 runtime 语义 bug，不是测试漂移。
     - 失败根因在 `SpringDay1Director.IsTownHouseLeadPending()`：
       - 它当前只看 `EnterVillage + Town + HasVillageGateProgressed() + !HasHouseArrivalProgressed()`
       - 而 `HasVillageGateProgressed()` 又把 `_villageGateSequencePlayed` 当成 true，即使 `VillageGate` formal 还在播
     - 所以当前代码会在 `VillageGate` 对话仍 active 时，过早把公开语义报成“已进入村长带路态”。
     - `GetCurrentBeatKey()` 这条链本身是对的，因为 `ShouldUseEnterVillageHouseArrivalBeat()` 已先挡住了 active `VillageGate`。
- 最小修复建议：
  1. 运行时代码最小刀口优先放：
     - `SpringDay1Director.IsTownHouseLeadPending()`
     - 追加“`VillageGateSequenceId` 仍 active 时返回 false”的 guard
     - 不建议先改 `HasVillageGateProgressed()`，因为它调用面更广，blast radius 更大。
  2. 测试侧最小刀口优先放：
     - `SpringDay1OpeningRuntimeBridgeTests.VillageGateCompletionInTown_ShouldPromoteChiefLeadState()`
     - 把测试里创建的 `001` 补成“可识别 NPC actor”，例如最小加一个 `NPCMotionController`
     - 不建议为了过测试去放宽 `SpringDay1Director.FindPreferredStoryNpcTransform(...) / IsLikelyNpcActorTransform(...)`，那会把运行时 actor 解析 contract 变松。
- 测试预期是否要改：
  - 第二条 `VillageGateWhileActive...`：**不改预期**
    - 这条预期符合当前 desired contract：围观 formal 没播完时，不该公开报成 chief lead pending。
  - 第一条 `VillageGateCompletion...`：**不改语义预期，只改测试夹具**
    - 当前 `chief=001` 这条断言仍有价值，它在验证带路 leader 的 actor 解析真接上；
    - 如果把它降成只看 `target=SceneTransitionTrigger`，测试价值会明显下降。
- 当前恢复点：
  - 若后续开刀，opening 最小顺序应是：
    1. 改 `IsTownHouseLeadPending()`
    2. 补 `VillageGateCompletion...` 的 chief actor 测试夹具
    3. 重跑 opening bridge tests

## 2026-04-11｜真实施工：后续天数 02:00 自动睡觉 fallback 补口
- 用户目标：
  - 修掉“只有 Day1 超过凌晨两点未睡会回 `Home` 床边；后续天数不会”的 runtime 漏口；
  - 用户明确说明这条是通过 `+` 号跳时复现，所以要把 `TimeManagerDebugger` 的 `+` 路径一起纳入回归。
- 本轮实际做成：
  1. `SpringDay1Director.HandleSleep()` 现在正式拆成两条语义：
     - `Spring Day1` 真收束：继续走导演自己的 `FreeTime -> DayEnd` 桥接
     - 非 `Spring Day1`：直接走公共强制睡觉 fallback，回 `Home` 再贴到休息位
  2. 新增 `ShouldHandleSpringDay1SleepResolution(...)`
     - 把“当前这次 sleep 是否仍属于 Day1 收束”钉成单独判断
     - 既兼容 runtime 的 `OnSleep` 事件时点，也兼容 EditMode 里直接调用 `HandleSleep()` 的晚段测试夹具
  3. 新增 `HandleGenericForcedSleepFallback()`
     - 非 Day1 sleep 时会：
       - 收掉已有 `PromptOverlay`
       - 释放 story time pause
       - 把场景切回 `Home`
       - 再调用现成 `TryPlacePlayerNearCurrentSceneRestTarget()` 贴到床边/休息位
       - 并补一次 `SnapResidentsToHomeAnchors()`，保证夜里人和玩家一起收束
  4. EditMode 兼容一起补了：
     - 非 PlayMode 下改用 `EditorSceneManager.OpenScene("Assets/000_Scenes/Home.unity", OpenSceneMode.Single)`
     - 不再因为 `LoadScene` 的 play-only 限制把 targeted tests 卡死
  5. `SpringDay1LateDayRuntimeTests` 新增：
     - `PlusHourAdvance_BeyondTwoAmAfterDay1_ShouldFallbackToHomeSleepTransition`
     - 这条直接走 `TimeManagerDebugger.AdvanceOneHour()`，覆盖用户现场说的 `+` 号路径
  6. `SpringDay1TargetedEditModeTestMenu` 已把新回归并入 late-day bridge 套件
- 涉及文件：
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
  - `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
- 本轮验证：
  - `spring-day1-late-day-bridge-tests.json`
    - `timestamp=2026-04-11T01:33:40`
    - `success=true`
    - `passCount=6`
    - 新增通过项：
      - `PlusHourAdvance_BeyondTwoAmAfterDay1_ShouldFallbackToHomeSleepTransition`
  - `git diff --check --` 本轮 3 个目标文件：通过
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 20`
    - `errors=0 warnings=0`
    - 仅剩一条 Unity TestFramework 写 `TestResults.xml` 的噪声型 `Exception`，不计项目红错
  - `validate_script` 覆盖本轮 3 文件：
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - 当前卡在 Unity `stale_status`，不是代码 owned red
- 当前恢复点：
  - 这条“后续天数 + 号跨两点也要回 Home 睡觉”已落成正式 contract
  - late-day targeted suite 当前全绿，可继续回到更大的 `runtime / PromptOverlay / 任务清单` 尾账

## 2026-04-11｜只读审查：PromptOverlay / 任务清单架构分界与最小拆口
- 用户目标：
  - 不改代码，只读审查：
    - `2026-04-10_UI线程_给spring-day1阶段回执_34.md`
    - `2026-04-10_spring-day1_回UI_任务清单治理裁定_33.md`
    - `SpringDay1PromptOverlay.cs`
    - `SpringDay1Director.cs`
  - 回答：
    - `formal task card / manual prompt` 还混在哪些字段与方法里
    - 最小、最安全、最不易返工的拆分落点是什么
    - 如果这一轮真收口，最该先改哪几个方法
- 本轮执行：
  - 只读分析；未进入真实施工；未跑 `Begin-Slice`
  - 额外核对了当前活跃 `004_runtime` memory，确认 2026-04-10 之后没有比这组回执更新的 `PromptOverlay` 架构裁定
- 本轮稳定结论：
  1. `formal task card` 的语义 owner 已经在 `SpringDay1Director`，而且主要 contract 已成形：
     - `GetTaskListVisibilitySemanticState()`
     - `BuildPromptCardModel()`
     - `GetPromptFocusText() / BuildPromptItems()`
  2. 真正的混合点仍在 `SpringDay1PromptOverlay` 内部状态机：
     - 字段：
       - `_manualPromptText`
       - `_manualPromptPhaseKey`
       - `_queuedPromptText`
     - 方法：
       - `Show(string text)`
       - `Hide()`
       - `BuildCurrentViewState()`
       - `BuildManualState(string text)`
       - `LateUpdate()`
     - 其中最硬的混点是：
       - `BuildCurrentViewState()` 先取 `BuildPromptCardModel()`，再用 `_manualPromptText` 覆盖 `model.FocusText`
       - `PromptCardViewState.FromModel(..., focusText, ...)` 允许 overlay 继续把手动文案灌回正式任务卡
  3. `formal visibility` 也还没有真正落到消费链：
     - `SpringDay1Director.GetTaskListVisibilitySemanticState()` 已能产出 `forceHidden / allowRestore / semanticKey`
     - 但 `SpringDay1PromptOverlay` 当前并没有直接消费这套正式可见性语义
     - 反而保留了无人调用的 `SetExternalVisibilityBlock(bool blocked)`，说明“正式隐藏语义”和“overlay 自己抑制自己”仍是两套链
  4. `manual prompt` 仍然散落在 `SpringDay1Director` 的大量流程方法里：
     - 当前仍有约 `50` 处 `SpringDay1PromptOverlay.Instance.Show/Hide`
     - 其中相当多与 `GetPromptFocusText()` / `BuildPromptItems()` 产出的正式文案重复或近似重复
     - 高重复区主要在：
       - `TryHandleTownEnterVillageFlow()`
       - `TryHandleWorkbenchEscort()`
       - `TickFarmingTutorial()`
       - `TryHandlePostTutorialExploreWindow()`
       - `CompleteFreeTimeIntro()`
       - `ShowFreeTimePressurePromptForHour()`
- 最小拆分落点：
  1. **正式任务卡**：
     - 固定收口到 `SpringDay1Director.BuildPromptCardModel() + GetTaskListVisibilitySemanticState()`
     - 让 `PromptOverlay` 只消费这套真值，不再自己决定“正式页该显示什么”
  2. **手动桥接提示**：
     - 不再进入 `SpringDay1PromptOverlay` 的任务卡状态机
     - 这一层应另走独立 bridge/hint 通道；至少也要先和 `PromptCardViewState` 彻底断开
  3. **这轮先不要碰的面**：
     - `ResolveParent()`
     - runtime parent / sorting / page flip / 布局细节
     - 这些都属于显示壳尾账，不是当前 owner 混乱的第一 blocker
- 如果下一轮真收口，最该先改的 4 个方法：
  1. `SpringDay1PromptOverlay.BuildCurrentViewState()`
     - 先把“formal model + manual override”的合流口拆开
  2. `SpringDay1PromptOverlay.Show(string text)`
     - 先切断手动 prompt 直接写入任务卡状态的入口
  3. `SpringDay1Director.TickFarmingTutorial()`
     - 这里有一组最典型、且与正式模型重复度最高的 `Show(...)`
  4. `SpringDay1Director.TryHandleTownEnterVillageFlow()`
     - 这里集中了 `bridge / wait / transition` 三类 manual prompt，是另一块高价值去混口
- 当前恢复点：
  - 这轮已经能稳定回答：真正第一刀该拆的是“状态 owner”，不是“视觉参数”
  - 如果后续进入真实施工，建议按上面 4 个方法开最小切片，不要先从 `ResolveParent / sorting / alpha` 这类 UI 壳层入手

## 2026-04-11｜只读审查：Day1 存档回退 / phase 偷推进尾账现状
- 用户目标：
  - 只读审查 `StoryProgressPersistenceService`、对应 editor tests、`SpringDay1Director` 与当前 `004_runtime` 记忆；
  - 回答这条线现在是否还存在明确未闭环点，以及若要这轮一起收尾，最值钱的一刀该是补测试、改 persistence，还是两者一起。
- 本轮执行：
  - 未进入真实施工；未跑 `Begin-Slice`
  - 只读核查了：
    - `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
    - `Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
    - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
    - 当前 `004_runtime收口与导演尾账/memory.md`
- 本轮稳定结论：
  1. 这条尾账**还没有完全闭环**：
     - 晚段四个位（`_dinnerSequencePlayed / _returnSequencePlayed / _freeTimeIntroCompleted / _freeTimeIntroQueued`）的 phase 偷推进已专门补过；
     - 但 `StoryProgressPersistenceService.ApplySpringDay1Progress(...)` 仍会按 `currentPhase` 回填前中段私有位：
       - `_villageGateSequencePlayed`
       - `_houseArrivalSequencePlayed`
       - `_healingStarted`
       - `_healingSequencePlayed`
       - `_workbenchOpened`
       - `_workbenchSequencePlayed`
       - `_staminaRevealed`
  2. 当前 tests 还没有把这条剩余风险钉死：
     - `Load_DoesNotPromoteLateDayPrivateFlagsFromPhaseAlone` 只守住晚段；
     - `SaveLoad_RoundTripRestoresLongTermStoryStateAndClearsNpcTransientState` 主要还是 round-trip 与 farming/health/energy 回填，没有专门制造“phase 晚于 completed”的前中段错配快照。
  3. 额外发现一个 test drift：
     - 当前 workbench hint 实现已经只走 `StoryProgressPersistenceService` 的 runtime 静态态；
     - 但 `StoryProgressPersistenceServiceTests.cs` 仍在用 `PlayerPrefs` 种值与断言，说明这部分断言已经不能准确代表现行实现。
  4. 如果这轮要一起收尾，最值钱的一刀不是二选一，而是**测试 + persistence 逻辑一起收**：
     - 只补测试：能暴露问题，但不能真正收口；
     - 只改 persistence：没有回归护栏，后面仍可能把 phase 派生重新带回来。
- 建议改点：
  1. 先在 `StoryProgressPersistenceServiceTests.cs` 新增 1~2 条“手工构造 phase/completed 不一致”的 load 回归，优先盯：
     - `VillageGate / HouseArrival`
     - `Healing`
     - `Workbench`
  2. 再把 `ApplySpringDay1Progress(...)` 收成“phase 只决定当前阶段，consumed/played/private one-shot 只认 completed ids 或显式 snapshot 真值”；
  3. 若这轮必须压缩范围，先收：
     - `_villageGateSequencePlayed`
     - `_houseArrivalSequencePlayed`
     - `_healingSequencePlayed`
     - `_workbenchSequencePlayed`
     - `_workbenchOpened`
     再决定 `_healingStarted / _staminaRevealed` 是否第二刀做语义归位。
- 当前现场补记：
  - `git status` 显示：
    - `StoryProgressPersistenceService.cs`
    - `StoryProgressPersistenceServiceTests.cs`
    仍是 `untracked`
  - 所以即使按“代码方向已接近”来算，这条尾账在仓库落地层也还不能算真正闭环。
- 验证状态：
  - 纯静态审查，未跑 Unity Editor tests
- 当前恢复点：
  - 若下一轮真开刀，最稳顺序应是：
    1. 先补前中段 mismatch-load 回归
    2. 再收 `ApplySpringDay1Progress(...)` 的剩余 phase 派生
    3. 顺手校正 workbench hint 的测试断言来源

## 2026-04-11｜真实收口：PromptOverlay owner split + phase 防偷推 + Day1 targeted fresh 复判
- 当前主线：
  - `spring-day1 / 004_runtime收口与导演尾账`
  - 本轮目标是把当前还挂在 `PromptOverlay / 任务清单`、`save rollback / phase 偷推进`、以及新鲜跑出来的 `workbench fallback / NPC formal gate` 尾账一起收口，并补成可直接给用户验的 fresh targeted 证据。
- 本轮实际做成：
  1. **PromptOverlay / 任务清单 owner 已拆成 formal card + bridge prompt 两条链**
     - `SpringDay1Director` 新增：
       - `TaskListBridgePromptDisplayState`
       - `ShowTaskListBridgePrompt(...)`
       - `HideTaskListBridgePrompt()`
       - `GetTaskListBridgePromptDisplayState()`
     - `SpringDay1PromptOverlay` 不再把 `_manualPromptText` 灌回 formal `FocusText`
     - bridge prompt 现在走独立 `BridgePromptRoot / bridgePromptText / bridgePromptCanvasGroup`
     - load 存档时会额外 `HideTaskListBridgePrompt()`，避免旧档把临时提示壳残留带回来
  2. **Day1 save rollback / phase 偷推进尾账已从前中后段一起补到代码和回归**
     - `StoryProgressPersistenceService.ApplySpringDay1Progress(...)` 已去掉 opening / midday 的危险 one-shot phase 派生：
       - `_villageGateSequencePlayed`
       - `_houseArrivalSequencePlayed`
       - `_healingSequencePlayed`
       - `_workbenchSequencePlayed`
     - `StoryProgressPersistenceServiceTests` 新增：
       - `Load_DoesNotPromoteOpeningAndMiddayPrivateFlagsFromPhaseAlone`
     - 同时把 workbench hint 断言切回当前 runtime static 真源，不再误吃旧 `PlayerPrefs`
  3. **fresh targeted 补跑时新抓到的两个尾巴也一起收了**
     - `SpringDay1LateDayRuntimeTests.Director_WorkbenchFallback_ShouldNotMarkCraftObjectiveComplete`
       - 断言已改成当前真实语义“推进到和村长收口”，不再吃旧的“推进到晚餐”口径
     - `NpcInteractionPriorityPolicy`
       - 修掉了 `StoryManager / PackagePanelTabsUI` 在“同帧销毁后重建”场景下的 stale cache 漏口
       - 现在当缓存对象已失效时，会先尝试即时重取，再决定是否走本帧空结果短窗
       - 这刀直接把 `NPC formal / informal` 门禁 targeted suite 拉回全绿
- 涉及文件：
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
  - `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
  - `Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
  - `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
  - `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
  - `Assets/YYY_Scripts/Story/Interaction/NpcInteractionPriorityPolicy.cs`
- fresh 验证结果：
  - `spring-day1-midday-bridge-tests.json`
    - `2026-04-11T03:13:29+08:00`
    - `success=true`
    - `passCount=8`
  - `spring-day1-prompt-overlay-guard-tests.json`
    - `2026-04-11T03:14:35+08:00`
    - `success=true`
    - `passCount=5`
  - `spring-day1-opening-bridge-tests.json`
    - `2026-04-11T03:14:39+08:00`
    - `success=true`
    - `passCount=10`
  - `spring-day1-late-day-bridge-tests.json`
    - `2026-04-11T03:14:43+08:00`
    - `success=true`
    - `passCount=6`
  - `spring-day1-story-progress-tests.json`
    - `2026-04-11T03:14:45+08:00`
    - `success=true`
    - `passCount=3`
  - `spring-day1-workbench-fallback-test.json`
    - `2026-04-11T03:25:05+08:00`
    - `success=true`
    - `passCount=1`
  - `spring-day1-npc-formal-consumption-tests.json`
    - `2026-04-11T03:25:11+08:00`
    - `success=true`
    - `passCount=3`
  - `spring-day1-director-staging-tests.json`
    - `2026-04-11T03:16:35+08:00`
    - `success=true`
    - `passCount=30`
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 20`
    - `errors=0 warnings=0`
    - 仅剩 Unity TestFramework 写 `TestResults.xml` 的噪声型 `Exception`
  - `git diff --check --` 本轮 own 目标文件：
    - 无 blocking diff error
    - 仅 `SpringDay1PromptOverlay.cs` 有既有 `CRLF/LF` warning
- 当前阶段：
  - `day1` 当前主链 targeted contract 已全部 fresh 复判通过
  - 这轮能 claim 的是：`结构 + targeted runtime guard + save rollback + prompt/task-list owner split` 已收口
  - 还不能替用户 claim 的只剩真正的人眼体验终验，例如完整一遍实玩节奏、站位观感、UI 阅读感
- 当前恢复点：
  - 代码与 targeted 票据层面已经到可交用户终验的状态
  - 本轮已跑 `Park-Slice`，`spring-day1` 当前 live 状态 = `PARKED`

## 2026-04-11｜并行 7 线程审计与打包收尾判断
- 当前主线：
  - `spring-day1` 作为 Day1 owner，用户要求暂停继续施工，先只读审核当前并行 7 线程的阶段产出，判断哪些已可接受、哪些仍会影响打包，以及 Day1 自己下一步该如何面向最终打包收口。
- 本轮实际完成：
  1. 已逐条核对 7 份线程回执与现场代码，确认这轮没有漏线程：
     - `DayNightOverlay / 云朵与光影`
     - `存档系统`
     - `farm / toolbar`
     - `UI / InteractionHintOverlay`
     - `排序/透视/层级审计`
     - `导航历史基线考古`
     - `Town 常驻 NPC severe 分账`
  2. 当前审计结论已压实：
     - `DayNightOverlay` 的 editor `GlobalScene` 预览改法方向正确，且只影响编辑器预览，不是打包 blocker；
     - `存档系统` 主链可用，但 packaged smoke、save 页护栏和 packaged 噪音仍未真正验完，是打包前必须收的一条；
     - `farm / toolbar` 的引用/订阅自愈补丁可信，但“被 UI 外部红和 TestRunner 噪音打断”这条 blocker 已部分过时，因为当前 `InteractionHintOverlay` 侧 compile clean；这条线现在更该直接重跑干净 live，而不是继续等待；
     - `UI / InteractionHintOverlay` 的根因分析可信，且现在已不像先前那样会反复给 farm 造成明显 compile 干扰，但仍欠用户 live 终验；
     - `排序/透视/层级` 的系统性审计结论可信，但这是结构性重构题，不是当前打包窗口应开启的大刀；
     - `导航历史基线考古` 结论可信，但现在只适合作为 fallback 基线知识，不该继续占当前打包窗口；
     - `Town 常驻 NPC severe 分账` 结论可信，`SpringDay1NpcCrowdDirector` 对 resident 的 `StopMotion()/SetFacingDirection()` stomp 是第一真风险；如果 live 里还看到 severe 摆头/倒走，这条锅应先回 Day1/crowd owner，而不是再泛甩导航。
  3. 当前打包导向下的优先级已经明确：
     - **P0 必做**
       - Day1 全流程用户终验
       - 存档 packaged smoke
       - farm/placement/toolbar 干净 live completion ticket
       - 若 Town 常驻 NPC 仍有 severe 坏相，则直接按 crowd owner 收口
     - **P1 可延后**
       - `InteractionHintOverlay` 纯表现层小瑕疵二次修口
       - `DayNightOverlay` 编辑器预览完善
     - **P2 暂缓**
       - 排序 contract 重构
       - 导航历史基线/最小回退方案继续考古
- 涉及现场依据：
  - `DayNightOverlay.cs` 当前确实在 editor `GlobalScene` 预览走 `FindObjectsByType<Renderer>` 聚合 scene bounds
  - `SaveManager.cs` 仍是主线程同步 `File.WriteAllText(...)`
  - `PackageSaveSettingsPanel.cs` 仍逐槽 `TryGetSlotSummary(...)`
  - `ToolbarUI.cs / ToolbarSlotUI.cs` 已有运行时重新订阅 / 重绑逻辑
  - `InteractionHintOverlay.cs` 已改成优先认领已有实例 + 退休重复 overlay + 避免整棵重建
  - `Tool_002_BatchHierarchy.cs / StaticObjectOrderAutoCalibrator.cs / DynamicSortingOrder.cs / PlacementManager.cs / PlacementLayerDetector.cs` 确实并存多套排序 contract
  - `SpringDay1NpcCrowdDirector.cs` 与 `SpringDay1NpcCrowdManifest.asset` 支撑了 crowd owner stomp 与 `101~301` 分账结论
- 当前阶段：
  - 不是“项目可以直接放心打最终包”的阶段；
  - 是“核心 blocker 已缩到极少数真正影响打包的 live 验证项，接下来应冻结大改、只收 smoke / live / owner 真问题”的阶段。
- 当前恢复点：
  - `spring-day1` 后续不应再主动开排序重构或导航考古；
  - 应只盯 `P0` 四项，把项目收成可 smoke / 可终验的冻结窗口。

## 2026-04-11｜用户终验首轮问题复盘：任务清单重复、晚饭切入报错、门口睡觉回归
- 当前主线：
  - 用户已经按我上一轮要求跑了一遍 Day1 首轮终验；这轮不是继续改代码，而是先只读复盘 3 个新问题，判断哪些是真 bug、哪些是体验层实现过头，以及每条的原需求与正确修法。
- 本轮实际完成：
  1. `任务清单 / PromptOverlay`
     - 已对齐 `SpringDay1Director.BuildPromptCardModel()`、`GetPromptFocusText()`、`BuildPromptItems()` 与 `ShowTaskListBridgePrompt()` 这两条链；
     - 结论是：我当时的结构本意是对的，想把“正式任务卡”和“桥接提示”拆 owner，避免 manual prompt 污染 formal card；
     - 但当前现场问题也成立：`bridge prompt` 虽然做了 `IsBridgePromptRedundant(...)` 保护，可它只挡字面重复，挡不住语义高度相似；因此用户现在会看到“上方提示 + 卡片正文/foot 信息重复率过高、位置也更抢戏”的坏体验。
  2. `19:00 -> Town 晚饭切入报错`
     - 已对齐用户报错栈与代码：
       - `SpringDay1Director.BeginDinnerConflict()` -> `SpringDay1PromptOverlay.Instance.Hide()`
       - `SpringDay1PromptOverlay.Hide()` -> `FadeCanvasGroup(0f, false)`
       - `FadeCanvasGroup()` 直接 `StartCoroutine(...)`
     - 结论是：这是一条真实 runtime bug，不是噪音；当前 `Hide()` / `FadeCanvasGroup()` 对 inactive overlay 没防守，`gameObject` inactive 时依旧会试图开 coroutine，所以才会炸 `Coroutine couldn't be started because the game object 'SpringDay1PromptOverlay' is inactive!`
  3. `回屋睡觉交互回归`
     - 已对齐 `TryAutoBindBedInteractable()`、`FindRestTargetCandidate()`、`IsRestProxyCandidate()`；
     - 当前 `PreferredRestProxyObjectNames = { "House 1_2", "HomeDoor", "HouseDoor", "Door" }`，`PreferredRestObjectNames` 里也含 `HomeDoor/HouseDoor`；
     - 这意味着找不到真实床时，会把门 / 门代理直接升级成 `SpringDay1BedInteractable`；
     - 所以现在门口就能 `E` 睡觉，也因此破坏了原本“先进屋 -> 对床交互 -> 睡觉”的正常 Home 流程。
- 当前阶段：
  - 这 3 条里，`晚饭切入报错` 和 `门口睡觉回归` 已经可以直接定性为真 bug；
  - `任务清单` 则不是用户挑剔，而是我这边实现策略过头：结构拆 owner 是对的，但信息层级和位置取舍没做好。
- 正确修法方向：
  - `任务清单`
    - 不是简单删掉新版文案，而是做融合：
      - 保留新版更准确的语义；
      - 降低 `bridge prompt` 的侵入性与信息量；
      - 和 `focus / footer / items` 做语义去重，不再只靠字面去重。
  - `晚饭切入报错`
    - `Hide()` 路径不能再对 inactive overlay 开 coroutine；
    - 最稳的是：inactive 时直接同步收口 alpha / blocksRaycasts，不走 fade coroutine；或让 `FadeCanvasGroup()` 自己先判 `isActiveAndEnabled && gameObject.activeInHierarchy`。
  - `门口睡觉回归`
    - 必须把“回屋路径代理”和“正式睡觉交互对象”重新拆开；
    - `SpringDay1BedInteractable` 只绑定真实床；
    - `HomeDoor/HouseDoor` 最多保留为 forced sleep fallback 的内部定位代理，不得再作为常规 `E` 睡觉交互点。
- 当前恢复点：
  - 下一刀如果继续施工，优先顺序应是：
    1. 修 `PromptOverlay` inactive coroutine 报错；
    2. 收回 `HomeDoor/HouseDoor` 的床交互绑定；
    3. 再做任务清单桥接提示与正式卡片的融合收口。

## 2026-04-11｜真实修复：PromptOverlay 报错、门口睡觉回归、任务清单 bridge prompt 过重
- 当前主线：
  - `spring-day1 / 004_runtime收口与导演尾账`
  - 这轮按用户刚跑出来的 3 条问题做最小真实施工，不扩到别的 Day1 逻辑。
- 本轮实际完成：
  1. `SpringDay1PromptOverlay`
     - `FadeCanvasGroup()` 已补 inactive 防守：
       - 对 `gameObject / overlayCanvas` 不在 active hierarchy 的场景，不再盲开 coroutine；
       - 改为直接同步收口 `CanvasGroup`；
       - 对位修掉了 `BeginDinnerConflict() -> Hide()` 时的 `Coroutine couldn't be started because the game object 'SpringDay1PromptOverlay' is inactive!`
  2. `SpringDay1Director`
     - `TryAutoBindBedInteractable()` 不再复用 `FindRestTargetCandidate()`；
       - 新增只找真实床位的 `FindRestInteractionTargetCandidate()` / `IsBedCandidate()`；
       - `FindRestTargetCandidate()` 仍保留给 forced sleep fallback 的定位语义；
       - 现在不会再把 `HomeDoor/HouseDoor` 之类的 rest proxy 误绑成 `SpringDay1BedInteractable`
  3. `任务清单 / bridge prompt`
     - `bridge prompt` 的视觉壳体已降侵入：
       - 更贴近卡片、更矮、更轻
     - 冗余判断已从“只挡字面 contains”升级为：
       - `focus / subtitle / footer / items` 全部参与比对
       - 加入二元组 overlap 语义相似度
       - 语义上已经被 formal task card 表达的 bridge prompt，会自动隐藏，不再在顶部重复顶一条
  4. 回归护栏
     - `SpringDay1LateDayRuntimeTests` 新增：
       - `PromptOverlay_Hide_ShouldNotStartCoroutineWhenRuntimeInstanceIsInactive`
       - `PromptOverlay_DirectorBridgePrompt_ShouldHideWhenSemanticallyRedundantWithFormalCard`
       - `Director_TryAutoBindBedInteractable_ShouldIgnoreDoorOnlyRestProxy`
     - `SpringDay1TargetedEditModeTestMenu` 已把新增测试接进 prompt / late-day 两条 targeted 菜单
- fresh 验证：
  - `manage_script validate`
    - `SpringDay1PromptOverlay.cs`：`errors=0`（仅旧 warning）
    - `SpringDay1Director.cs`：`errors=0`（仅旧 warning）
    - `SpringDay1LateDayRuntimeTests.cs`：`clean`
    - `SpringDay1TargetedEditModeTestMenu.cs`：`errors=0`（仅旧 warning）
  - `spring-day1-prompt-overlay-guard-tests.json`
    - `success=true passCount=7`
    - 包含新增：
      - `PromptOverlay_DirectorBridgePrompt_ShouldHideWhenSemanticallyRedundantWithFormalCard`
      - `PromptOverlay_Hide_ShouldNotStartCoroutineWhenRuntimeInstanceIsInactive`
  - `spring-day1-late-day-bridge-tests.json`
    - `success=true passCount=7`
    - 包含新增：
      - `Director_TryAutoBindBedInteractable_ShouldIgnoreDoorOnlyRestProxy`
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 20`
    - `errors=0 warnings=0`
    - 仅剩 `Saving results to ... TestResults.xml` 这类测试框架信息，不是红错
- 当前阶段：
  - 用户刚报出的这 3 条里：
    - `PromptOverlay` inactive 报错：已收
    - 门口直接睡觉回归：已收
    - 任务清单 bridge prompt 过重 / 重复：已做第一轮融合收口
  - 剩下仍需用户再验的，只是实际体验是否已经回到你要的手感与阅读感
- 当前恢复点：
  - 这轮已跑 `Park-Slice`
  - `spring-day1 = PARKED`
  - 如果用户继续验收，优先复测：
    1. `19:00 Primary -> Town` 晚饭切入
    2. `回屋 -> 进门 -> 对床睡觉`
    3. `任务清单 / bridge prompt` 观感与重复率
## 2026-04-11｜真实施工：晚饭/自由时段重定时 + 居民 20:00 回家/21:00 强制 resting + PromptOverlay 归基础 HUD 并在开包时退场
- 当前主线目标：
  - `spring-day1 / 004_runtime收口与导演尾账`
  - 本轮子任务是把用户最新补充的 Day1 语义同步一次性落地：
    1. 晚饭从 `19:00` 前移到 `18:00`
    2. 归途提醒锁到 `19:00`
    3. 自由时段从 `19:30` 开始
    4. Town 居民 `20:00` 自主回 anchor，`21:00` 未到位则强制贴回 anchor
    5. 任务清单 `PromptOverlay` 要回到和 toolbar 同级的基础 HUD 语义，并在背包/包裹打开时按统一模态规则退场
- 本轮实际完成：
  1. `SpringDay1Director.cs`
     - `DinnerReturnHour/Minute = 18:00`
     - `ReminderStartHour/Minute = 19:00`
     - `FreeTimeStartHour/Minute = 19:30`
     - `EnsureStoryTimeAtLeast(...)` 调用点已同步到 `BeginDinnerConflict()` / `BeginReturnReminder()` / `EnterFreeTime()`
     - Day1 managed time guardrail 已改成 total-minutes 夹制；`DinnerConflict` / `ReturnAndReminder` / `FreeTime` 的最小/最大时间窗已按分钟级重写
  2. `SpringDay1NpcCrowdDirector.cs`
     - 新增 `ResidentReturnHomeHour = 20`
     - 新增 `ResidentForcedRestHour = 21`
     - `ShouldResidentsReturnHomeByClock()`：`20:00~20:59` 返回 true
     - `ShouldResidentsRestByClock()`：`>=21:00` 或 `<09:00` 返回 true
     - `SyncResidentNightRestSchedule()` 已变成两段式：先自主回家，再强制 night-rest snap
  3. `SpringDay1PromptOverlay.cs`
     - runtime parent 解析不再直接挂到 `UI` 根，而是优先认 `UI` 下的基础 Canvas，避开 `PackagePanel` 模态 Canvas
     - 有父级 Canvas 时不再自抬 `overrideSorting`，改回父级 HUD 治理
     - `LateUpdate()` 现在会直接吃 `SpringDay1UiLayerUtility.ShouldHidePromptOverlayForParentModalUi()`，背包/箱子页打开时统一退场
     - `FadeCanvasGroup()` 在非 PlayMode 下改为同步收口，避免 EditMode 验证里 coroutine 不落地
  4. `SpringDay1LateDayRuntimeTests.cs`
     - `BeginDinnerConflict_ShouldNormalizeClockToSixPm` 改为直接验证 `NormalizeManagedDay1TimeTarget(...)` 的分钟级 floor，避免 EditMode 里误依赖 `Application.isPlaying`
     - `ReminderCompletion_ShouldEnterFreeTimeWithIntroPendingAndYieldWorkbenchToFormalNightIntro` 同步改为验证 `19:30` floor
     - `PromptOverlay_ShouldHideWhilePackagePanelIsOpen` 现在按真实 `PackagePanelTabsUI.SetRoots + ShowPanel(true)` 路径建壳，不再用 ghost-open 假场景
  5. `PackagePanelLayoutGuardsTests.cs`
     - 守卫口径收窄到玩家可感知结果：`PackagePanel` 打开后 `PromptOverlay` 退场；不再在最小测试壳里死盯与用户体验无关的 `overrideSorting/pixelPerfect` 细枝末节
  6. `SpringDay1TargetedEditModeTestMenu.cs`
     - `PromptOverlay Guard` targeted 菜单已纳入：
       - `PackagePanelLayoutGuardsTests.PackagePanelTabsUI_ShowPanel_ShouldRaiseCanvasAndHidePromptOverlayThroughUnifiedModalRule`
       - `PromptOverlay_UsesParentCanvasGovernance_WhenUiRootCanvasExists`
       - `PromptOverlay_ShouldPreferBaseCanvasUnderUiRoot_InsteadOfModalPackageCanvas`
       - `PromptOverlay_ShouldHideWhilePackagePanelIsOpen`
- fresh 验证：
  - `spring-day1-prompt-overlay-guard-tests.json`
    - `success=true passCount=11 failCount=0`
  - `spring-day1-late-day-bridge-tests.json`
    - `success=true passCount=8 failCount=0`
  - `spring-day1-director-staging-tests.json`
    - `success=true passCount=31 failCount=0`
  - `validate_script`
    - `SpringDay1PromptOverlay.cs`：`owned_errors=0`（assessment `unity_validation_pending`，原因仍是 Unity `stale_status`）
    - `SpringDay1LateDayRuntimeTests.cs`：`owned_errors=0`
    - `SpringDay1TargetedEditModeTestMenu.cs`：`owned_errors=0`
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`
    - fresh console 只剩 `Unity TestRunner` 的 cleanup residue：
      - `Saving results to ... TestResults.xml`
      - `Files generated by test without cleanup.`
    - 当前没有新的 Day1 runtime / compile owned red
- 当前阶段：
  - 这轮新增的时间语义、Town 居民夜间回家 contract、以及任务面板与背包的层级/退场关系，代码与 targeted 合同层都已过线
  - 当前唯一没收成“零 console 噪音”的，只剩测试框架 residue，不是业务逻辑 blocker
- 当前 live 状态：
  - 本轮已跑 `Begin-Slice`
  - 未跑 `Ready-To-Sync`（本轮未进白名单 sync）
  - 本轮已跑 `Park-Slice`
  - 当前线程状态：`PARKED`
- 下一恢复点：
  - 用户继续实跑时，优先验：
    1. `18:00` 晚饭切入、`19:00` 归途提醒、`19:30` 自由活动的体感是否正确
    2. `20:00` Town 居民是否开始主动回 anchor
    3. `21:00` 未到位居民是否被强制贴回 anchor
    4. 开包时左侧任务面板是否不再和背包并排露出

## 2026-04-12｜Day1 live 事故复判：Town 居民“像冻结”主嫌疑已落到 crowd caller，且 return-home 仍走硬推链
- 当前主线：
  - `spring-day1 / 004_runtime收口与导演尾账`
  - 本轮子任务是只读复判“Town / Day1 里 NPC 不动、像冻结，到底是导航 own 坏了，还是 Day1 caller 还在控人”，暂不改代码。
- 本轮实际完成：
  1. 重新拉起 MCP bridge，并通过 `CodexEditorCommandBridge + SpringDay1LiveSnapshotArtifactMenu` 拿到 fresh live snapshot。
  2. live snapshot 明确显示：
     - `Scene=Town`
     - `Phase=EnterVillage`
     - `beat=EnterVillage_HouseArrival`
     - `Time=10:10 AM` 与 6 秒后 `12:10 PM`
     - `101~203` 这批 Town resident 全都处于 `return-home`
     - 不是 `NPCAutoRoamController` 自己彻底没恢复，而是 `SpringDay1NpcCrowdDirector` caller 层仍在把居民压在散场/回家语义里。
  3. 代码链复核补站住了一个更硬的事实：
     - `NPCAutoRoamController.ReleaseResidentScriptedControl(...)` 自己有 `StartRoam()` 恢复出口；
     - 但 `SpringDay1NpcCrowdDirector.TickResidentReturnHome(...)` 当前不是走玩家同级静态导航，而是直接按 `transform.position` / `SetFacingDirection(...)` 做线性硬推。
- authoritative 结论：
  - 这次“Town 居民像冻结”的第一嫌疑不再是导航 own 忘了 resume；
  - 更像是 `Day1` 的 `cue -> baseline -> return-home` caller contract 还在持有，且持有后的回家链本身没有接回统一导航 contract。
  - 换句话说：这更像 `spring-day1` own 问题，不是导航线程单独该背的锅。
- 当前风险：
  - 如果继续沿用现在这条 `TickResidentReturnHome()` 硬推链，就会持续违背用户“NPC 应和玩家一样走静态导航”的要求；
  - 就算不彻底冻结，也会出现“剧情时能动、平时/散场像傻站或慢磨”的体感。
- 当前状态：
  - 本轮已 `Begin-Slice`
  - 未 `Ready-To-Sync`（未改代码）
  - 已 `Park-Slice`
  - 当前线程状态：`PARKED`
- 下一恢复点：
  - 若继续施工，第一刀应优先把 `SpringDay1NpcCrowdDirector` 的 resident return-home 从线性硬推改成 `NPCAutoRoamController.DriveResidentScriptedMoveTo(...)` 同级 contract；
  - 同时保留现有 `20:00` 自主回家、`21:00` 强制贴回 anchor 的晚间语义，不去伤已经完成的时间闭环。

## 2026-04-12｜真实修复：resident return-home 已接回 scripted move/pathing，不再对有 roam controller 的居民硬推 transform
- 当前主线：
  - `spring-day1 / 004_runtime收口与导演尾账`
  - 本轮子任务是把上一个只读结论落成真实修复，优先解决 Town resident “剧情后像冻结/傻站”的 Day1 own contract 问题。
- 本轮实际完成：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - `TickResidentReturnHome(...)` 不再默认对有 `NPCAutoRoamController` 的 resident 做 `transform.position` 线性硬推；
     - 新增 `TryDriveResidentReturnHome(...)` 和 `GetResidentReturnHomeStepDistance(...)`；
     - resident 有 roam controller 时，return-home 现在优先走 `DriveResidentScriptedMoveTo(...)` 的 scripted move/pathing；
     - 只有缺失 roam controller 的极窄 fallback 才继续保留旧的手搓位移。
  2. [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - 新增 `CrowdDirector_ShouldNotFallbackToHardPushWhenResidentHasRoamController`
     - 把“有 roam controller 的 resident 不允许再偷偷回退成硬推位移”钉成回归护栏。
  3. [SpringDay1TargetedEditModeTestMenu.cs](D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs)
     - 已把新回归测试接进 `Run Director Staging Tests` 的 targeted 菜单。
- fresh 验证：
  - `spring-day1-director-staging-tests.json`
    - `success=true passCount=32 failCount=0`
    - 新护栏 `CrowdDirector_ShouldNotFallbackToHardPushWhenResidentHasRoamController` 已实际执行并通过
  - `git diff --check`
    - 对本轮 3 个代码文件是 clean
  - Unity 编译面：
    - Editor.log 有 fresh `Assembly-CSharp-Editor.dll` 编译成功与 domain reload 记录
  - CLI compile-first：
    - `py -3 scripts/sunset_mcp.py compile ...`
    - assessment=`blocked`
    - 原因=`subprocess_timeout:dotnet:20s`
    - 这表示 CLI 侧的 CodeGuard 这次没在 20 秒内给完票，不等于 Unity 编译失败
- fresh live：
  - 重新跑过 `Spring Day1 Live Snapshot Artifact` 与 `Trigger Spring Day1 Recommended Action Artifact`
  - 已确认当前 fresh runtime 能稳定进 `Town / EnterVillage / EnterVillage_PostEntry`
  - 这轮没再把 same-window 现场硬推进到 `HouseArrival` 完整散场收口，所以“HouseArrival after-fix 体感”仍待用户终验
- authoritative 结论：
  - 这刀已经把 Day1 自己最明显的 contract 硬伤收掉了：resident return-home 不再优先走手搓 transform；
  - 它现在至少会优先复用 NPC 自己的 scripted move/pathing 链，方向上与“像玩家一样走静态导航”一致。
- 当前阶段：
  - `代码层 + targeted 合同层已过线`
  - `HouseArrival after-fix 玩家体感` 仍待终验
- 当前 live 状态：
  - 本轮已 `Begin-Slice`
  - 未 `Ready-To-Sync`（未进白名单 sync）
  - 已 `Park-Slice`
  - 当前线程状态：`PARKED`
- 下一恢复点：
  - 用户下一轮最值得先验：
    1. `Town / EnterVillage_HouseArrival` 之后，常驻 resident 是否开始正常走回 daily/home，而不是像站桩
    2. 晚上 `20:00` 回家、`21:00` 强制贴回 anchor 的旧 contract 是否仍正常
    3. 001/002 的剧情走位是否未被这刀影响

## 2026-04-12｜Day1 晚饭开场收口：001/002 不再回落到围观左上角旧 marker
- 当前主线目标：
  - 继续收 `spring-day1 / 004_runtime收口与导演尾账`
  - 本轮子任务是把用户终验里晚饭开场的 `001/002` 站位异常收掉，并确认这刀不反伤晚段睡觉/夜压链。
- 本轮实际完成：
  1. `SpringDay1Director.cs`
     - `AlignTownDinnerGatheringActorsAndPlayer()` 不再复用 `EnterVillageCrowdRoot` 的 `001起点/002起点`
     - 新增 `PreferredDinnerGatheringAnchorObjectNames`
     - 晚饭开场时，`001/002` 现在优先围绕 `DirectorReady_DinnerBackgroundRoot / DinnerBackgroundRoot` 落到晚饭区域，而不是继续被扔回进村围观左上角
  2. `SpringDay1LateDayRuntimeTests.cs`
     - 新增 `AlignTownDinnerGatheringActorsAndPlayer_ShouldPreferDinnerAreaOverVillageCrowdMarkers`
     - 把“001/002 不得再吃旧 crowd marker”钉成回归护栏
  3. `SpringDay1TargetedEditModeTestMenu.cs`
     - `Run Late-Day Bridge Tests` 已接入新测试
- fresh 验证：
  - `spring-day1-late-day-bridge-tests.json`
    - `success=true passCount=9 failCount=0`
    - 新护栏已实际执行并通过
  - `spring-day1-director-staging-tests.json`
    - `success=true passCount=32 failCount=0`
    - 说明这刀没有把前面刚修好的 resident return-home/staging contract 带坏
  - `git diff --check`
    - 本轮 3 个代码文件 clean
- fresh live / blocker：
  - 这轮尝试继续做 fresh live 黑盒时，编辑器侧仍被外部文件 `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` 的编译错误阻断：
    - `SyncContextHintCard`
    - `SetContextCardAlpha`
    - `ApplyContextHintCardLayout`
  - 因此当前不能把 `PlayMode live 体验` 包装成已终验；这次能诚实 claim 的仍是 `代码层 + targeted runtime tests`
- 当前阶段：
  - `晚饭开场错位` 这条 own 修复已落地
  - `fresh live 终验` 仍被 UI 外部红错卡住
- 当前 live 状态：
  - 本轮已 `Begin-Slice`
  - 未 `Ready-To-Sync`
  - 已 `Park-Slice`
  - 当前线程状态：`PARKED`

## 2026-04-12｜Day1 事故复判补记：`002` 剧情后 Town resident“地震式抖动”主根已收束到 spring-day1 caller contract
- 当前主线：
  - 继续服务 `spring-day1 / Day1 打包前 runtime 收口`
  - 本轮子任务是只读回答用户新报的阻塞事故：`002` 剧情刚过后，除 `001~003` 外的 Town resident 全体原地高频小幅抖动，`001/002` 也缺少正常避障并会推着玩家走。
- 本轮新站住的代码事实：
  1. `001/002` 与普通 resident 不是同一 contract。
     - `SpringDay1NpcCrowdDirector.ShouldDeferToStoryEscortDirector(...)` 只对白名单 `001/002` 生效；
     - 其它常驻 resident 仍走 `SyncCrowd() -> ApplyResidentBaseline() -> TryBeginResidentReturnHome() / TickResidentReturnHome()` 这条 crowd caller 链。
  2. resident 当前的 `return-home` 虽然不再走旧的 `transform` 硬推主路径，但仍不是“玩家同级静态导航 contract”。
     - `TickResidentReturnHome()` 会把 resident 送进 `NPCAutoRoamController.DriveResidentScriptedMoveTo(...)`；
     - `DriveResidentScriptedMoveTo(...)` 进一步落到 `DebugMoveTo(...)`，本质仍是 `spring-day1` caller 在接管 NPC 的 scripted move。
  3. 这条 scripted move 在当前实现里会主动绕开 shared avoidance。
     - `NPCAutoRoamController.TryHandleSharedAvoidance(...)` 里，`IsResidentScriptedControlActive && debugMoveActive` 会直接 `return false`，即 resident scripted move 不吃共享避让。
  4. 同时，这条 scripted move 又不会像 autonomous roam 那样在卡住时早停放弃。
     - `NPCAutoRoamController.ShouldAbortBlockedAdvanceWithoutRebuild(...)` 对 `debugMoveActive || IsResidentScriptedControlActive` 直接禁用早停；
     - 结果就是卡住后会继续 rebuild / 重试，而不是尽快退出坏 case。
  5. `001/002` 的剧情位移链本身也还不是统一导航 / 避障 contract。
     - `SpringDay1DirectorStaging` 与 `SpringDay1Director` 里仍存在 `transform.position += step / actor.position = nextPosition + SetExternalVelocity + SetFacingDirection` 的直推链；
     - 所以 `001/002` 被常驻 resident 挤住、推着玩家走，并不是用户误判，而是剧情 actor 自己也没走玩家同级避障。
  6. “抖动但朝向不怎么变”也有代码解释。
     - `NPCMotionController.SetFacingDirection(...)` 在 locomotion active 且朝向写入与运动向量不一致时会拒绝 stray facing 写入；
     - 所以当 scripted move 在近距离碰撞/重试里持续喂小幅速度时，会出现“身体微抖，但脸没明显跟着变”的体感。
- 当前综合判断：
  - 这次用户骂的锅，主锅已经不能再往“导航线程自己去收”上甩。
  - 更准确的说法是：`spring-day1` 现在用了一套“剧情/群演 caller 接管 NPC，再走半套 scripted move”的 contract；
  - 这套 contract 同时缺：
    1. resident scripted move 的 shared avoidance
    2. 卡住后的硬退出语义
    3. `001/002` story actor 与普通 resident 的统一避障口径
  - 所以玩家看到的结果才会是：
    - resident 整片像地震一样抖
    - `001/002` 也不会像玩家一样绕人
    - 人堆之间互相顶、还能把玩家推出去
- 当前验证状态：
  - `静态推断成立`
  - 这轮 direct MCP fresh live 还没补上，因为当前 Unity session 未重新挂回 MCP，请求只返回 `no_unity_session`；
  - `Editor.log` 里同时仍保留外部 `InteractionHintOverlay.cs` 编译错残留，所以不能把这轮包装成 fresh runtime 已终验。
- 下一恢复点：
  - 若继续施工，第一刀应只改 `spring-day1` own：
    1. resident scripted move 必须重新接回 shared avoidance / blocked abort 语义
    2. `001/002` story actor move 不再继续走 `transform` 直推链
    3. 目标是把 `story actor` 与 `resident return-home` 都压回“玩家同级静态导航 contract”，而不是继续补局部朝向或局部 stopgap

## 2026-04-12｜真实修复：resident scripted move 不再绕开 shared avoidance，并为 scripted move 恢复静态坏点早停
- 当前主线：
  - 继续服务 `spring-day1 / Day1 打包前 runtime 收口`
  - 本轮真实施工只收两件事：`resident scripted move` 的避让缺口，以及它在静态零推进坏点上的 stopgap 语义。
- 本轮实际改动：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - 新增 `ShouldBypassSharedAvoidanceForCurrentMove()`
     - `TryHandleSharedAvoidance(...)` 不再对 `resident scripted control + debugMoveActive` 直接跳过 shared avoidance
     - `ShouldAbortBlockedAdvanceWithoutRebuild(...)` 从“所有 debug/scripted move 一律不早停”改成“只保留纯 debug move 旧口径；resident scripted move 在静态零推进坏点上允许早停”
  2. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 新增 `ResidentScriptedControl_DebugMoveShouldStillParticipateInSharedAvoidance`
     - 新增 `ResidentScriptedControl_StaticBlockedAdvanceShouldAllowEarlyAbort`
  3. `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
     - 已把上述两条 resident-scripted-control 护栏接入 `Director Staging Tests` 目标清单
- 这刀解决的人话结果：
  - `spring-day1` 自己再把 resident / escort actor 送进 scripted move 时，不再把 NPC 当成“可以互相硬顶的脚本块”
  - 人堆和窄口会重新吃 shared avoidance
  - 遇到树边 / 静态角落那种零推进坏点时，scripted move 也不再无限把 stopgap 语义豁免掉
- 本轮验证：
  - `git diff --check` 对本轮 3 个目标文件 clean
  - `spring-day1-director-staging-tests.json`：`success=true passCount=32 failCount=0`
    - 说明这刀没有把既有 `crowd/staging/return-home` 护栏带坏
  - `spring-day1-late-day-bridge-tests.json`：`success=true passCount=9 failCount=0`
    - 说明晚饭时间、睡觉 fallback、自由时段桥接没有被这刀带坏
  - `npc-resident-director-bridge-tests.json`：`failed(0/3)`
    - 这组失败落在 stagebook / manifest / cue contract 面，当前不在本轮改动路径内；本轮不把它包装成这刀新引入的问题
- 当前状态：
  - 本轮已 `Begin-Slice`
  - 未 `Ready-To-Sync`
  - 已 `Park-Slice`
  - 当前线程状态：`PARKED`
- 下一恢复点：
  - 优先等用户做 fresh live 黑盒，看 `002` 剧情后 Town resident 是否不再整片“地震式抖动”
  - 如果还有残留，再继续收 `SpringDay1DirectorStaging` 的手搓 `transform.position += step` cue 位移链

## 2026-04-13｜Day1 阻塞复判与当前停车点：已抓到 resident 全体卡死的 lifecycle 恢复漏口，并补上 Town runtime rebind
- 当前主线：
  - `spring-day1 / Day1 打包前 runtime 收口`
  - 这轮唯一主刀是用户新报的 P0 阻塞：`002` 后 Town 里除 `001~003/小动物` 外的 resident 看起来整片卡死，导致无法继续打包验收。
- 这轮真实施工与取证：
  1. 新增 editor-only 探针 `[Assets/Editor/Story/SpringDay1ActorRuntimeProbeMenu.cs]`，用于输出：
     - 当前 loaded scenes
     - `001/002/003/101~203` 是否真实存在
     - 所在 scene / hierarchy
     - roam/state/scripted-control/shared-avoidance 运行态
  2. 修正该探针的两轮误判：
     - 第一轮修掉编译红（`ToArray()` / 移除不存在的 `CurrentFacingDirection`）
     - 第二轮把 NPC 识别规则从“任意名字抽数字”收紧为“只认真 NPC objectName，或明确位于 `NPCs / Town_Day1Residents / Resident_*` 根下的对象”，避免把 `Baby Chicken Yellow (1)` 之类小动物误记成 `001`
  3. 新增 editor-only 快速跳转菜单 `[Assets/Editor/Story/SpringDay1LatePhaseValidationMenu.cs]`
     - 只服务这轮 live 取证，支持把当前 Play 现场快速推进到 `Dinner` / `FreeTime` 验收入口
     - 不改 runtime 业务逻辑
  4. fresh live 关键证据：
     - 在 `Town / EnterVillage_HouseArrival / 09:10` 的正常现场里，resident probe 明确显示 `101~203` 都是 `scriptedControlActive=false` 且大多 `isRoaming=true|isMoving=true`，说明“所有 resident 天生就不动”并不成立
     - 但在一次 refresh / domain reload 后，fresh snapshot 与 probe 抓到：
       - `Crowd=scene=Town|phase=EnterVillage_HouseArrival|spawned=0|missing=101,102,103,104,201,202,203|active=none`
       - 同时 `Town` scene 里的 `101~203` 实例仍然存在，但 `roamState=Inactive / isRoaming=false / isMoving=false`
     - 这说明真问题不是单纯导航，而是：`SpringDay1NpcCrowdDirector` 的 runtime registry 在某些重建/承接拍上会丢失，scene resident 还在，但 crowd runtime 没把它们重新 bind 回 `_spawnStates`，于是整批 NPC 被留在失活态
  5. 已落地 runtime 修口：
     - `[Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs]`
     - 在 `Update()` 前段新增 `ShouldRecoverMissingTownResidents(...)`
     - 当满足：
       - 当前 active scene = `Town`
       - `_spawnStates.Count == 0`
       - `StoryManager / SpringDay1Director / manifest` 都已就位
       - 且 Town scene 里确实还存在 `Town_Day1Residents / Resident_*` 根或 manifest resident 实例
       就会主动执行一次 `SyncCrowd()` 把 scene resident 重新 bind 回 runtime registry
     - 这刀只补生命周期恢复，不改剧情 phase 判定、不改既有 resident 语义
- 当前验证：
  - `validate_script` 对 `[Assets/Editor/Story/SpringDay1ActorRuntimeProbeMenu.cs]` 与 `[Assets/Editor/Story/SpringDay1LatePhaseValidationMenu.cs]` 代码层通过
  - `python scripts/sunset_mcp.py errors --count 20 --include-warnings --output-limit 20`：`errors=0`
  - 当前 console 仍有一批 `[NPCFacingMismatch]` warning，它们是 warning，不是 blocking error
- 当前阶段：
  - 这轮已经从“怀疑导航/怀疑 owner”推进到“Day1 crowd runtime lifecycle 恢复漏口已钉实并已补 runtime 恢复刀”
  - 用户当前 live 口头回报是“npc 已经可以走了”，所以本轮按 `等待用户黑盒继续终验` 合法停车，不继续扩第二刀
- 当前状态：
  - 已 `Park-Slice`
  - `spring-day1 = PARKED`
- 后续恢复点：
  1. 如果用户 live 继续通过，这轮主修口可视为成立，下一步转用户终验反馈
  2. 如果 live 仍有“002 后 Town resident 整片站死/抖动”，下一刀优先继续查 `SpringDay1NpcCrowdDirector` 在 scene transition / cue release 后的 registry 再建与 release 时机，不再回导航线程泛查

## 2026-04-13｜已按用户要求补给导航的同步文件
- 已新增同步文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-12_给导航_002后TownResident冻结根因与当前修复同步.md`
- 目的：
  - 让导航明确知道这次 `002` 后 `Town resident` 整片冻结的 fresh 结论
  - 避免后续再把这条问题漂回“导航 core 泛锅”
- 同步核心：
  - 真根是 `SpringDay1NpcCrowdDirector` 的 crowd runtime registry 恢复漏口
  - 已在 `Update()` 里补 Town resident rebind 恢复刀
  - 用户当前 live 口头反馈：`npc 已经可以走了`

## 2026-04-13｜按用户最新裁定改为先产出双 prompt 分账
- 用户最新明确要求：
  - 先不要继续让我直接闷头改
  - 要先把“该由存档线程接的 Day1 晚段恢复语义”与“我自己这边的打包前彻底收尾清单”各自收成 prompt
  - 且 prompt 必须能直接转发，不再让用户自己从长聊天里拼
- 已新增 2 份 prompt 文件：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给存档系统_Day1晚段恢复语义与职责分工prompt.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给spring-day1_晚段打包前彻底收尾prompt.md`
- 这两份 prompt 已吸收的最新用户补充：
  - 晚饭不只是 `+` 快进会卡，主动找村长触发也会卡
  - `重新开始` / `读档` 高风险说明当前不只是 runtime 自身问题，还包括恢复语义没交全
  - 存档线程负责恢复 contract，`spring-day1` 自己负责晚段 runtime re-entry 与 own 自愈，不互相越权

## 2026-04-13｜上一版双 prompt 已被用户否决，已重写为“整条 Day1”v2
- 用户明确指出：
  - 问题不是“晚段”独有，而是整条 `Day1`
  - 存档 prompt 不该让 save thread 自己去猜语义，而应由 `spring-day1` 先把完整恢复 contract 说清，再分工出去
  - 给 `spring-day1` 自己的 prompt 也不能再分成窄刀口，而要覆盖最近十轮遗留、完整修复清单、注意事项和打包前完成定义
- 已新增新版 prompt：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给存档系统_整条Day1恢复contract与职责分工prompt_v2.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给spring-day1_整条Day1打包前总收尾prompt_v2.md`
- 这两份 v2 已纳入的核心口径：
  - save thread 接的是“按 spring-day1 已定义 contract 做 save/load/restart/re-entry 接线”，不是自己发明 Day1 语义
  - `spring-day1` 自己接的是“整条 Day1 打包前 own runtime / own UI / own staging / own re-entry 总收尾”，不是继续只盯晚段
  - 用户最新补充“晚饭无论主动触发还是 + 快进都会卡在 0.0.6”也已写入新版 prompt

## 2026-04-13｜用户再裁定后，已把 UI 完全分出去并生成 v3
- 用户最新明确裁定：
  - 晚饭卡顿的真根是“有个 NPC 没有走到位”
  - 正确 runtime 处理应为：最多等待 `5` 秒，超时则只对必要剧情 actor 瞬移到位，然后立刻开始剧情
  - `UI` 全权交给 `UI` 线程，`spring-day1` 不要再自己动 UI
  - 但 `spring-day1` 必须把整条 Day1 的 UI 语义 contract 完整传给 UI
- 已新增：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给UI_整条Day1任务清单与Prompt语义contract.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给spring-day1_整条Day1打包前总收尾prompt_v3.md`
- 当前最新口径：
  - UI 视觉/布局/层级/最终玩家可见面由 UI own
  - `spring-day1` own prompt 已改成只收 runtime/staging/re-entry/time gate/NPC 晚间行为/对外 contract，不再自己碰 UI 壳体

## 2026-04-13｜v3 晚饭 5 秒兜底 runtime 已落地并完成目标测试
- 本轮按 `2026-04-13_给spring-day1_整条Day1打包前总收尾prompt_v3.md` 继续真实施工；只动 `spring-day1` own runtime，不碰 UI 壳体。
- 已改：
  1. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
     - 新增 `dinnerCueSettleTimeout = 5f`
     - 新增 `_dinnerCueWaitStartedAt`
     - 将晚饭 actor 对位拆成 `AlignTownDinnerGatheringStoryActors()`
     - `BeginDinnerConflict()` 不再因 `DinnerConflictTable` 未 settled 无限 `return`
     - 现逻辑为：先等 cue settled；若未 settled，则最多等 5 秒；超时后仍直接继续，并在开戏前强制把 `001/002` 对到晚饭区域
     - 新增 `ResetDinnerCueSettlementState()`，把这条计时状态接进 fresh / phase reset / debug re-entry
     - 仅编辑器下新增 `_editorDinnerCueSettledOverride`，只服务 EditMode 测试，不进玩家运行包默认语义
  2. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
     - 新增 `BeginDinnerConflict_ShouldKeepWaitingBeforeDinnerCueTimeout`
     - 新增 `BeginDinnerConflict_ShouldForceAlignStoryActorsAfterDinnerCueTimeout`
- 已验证：
  - 目标 EditMode 测试 4/4 通过：
    - `SpringDay1LateDayRuntimeTests.BeginDinnerConflict_ShouldKeepWaitingBeforeDinnerCueTimeout`
    - `SpringDay1LateDayRuntimeTests.BeginDinnerConflict_ShouldForceAlignStoryActorsAfterDinnerCueTimeout`
    - `SpringDay1LateDayRuntimeTests.BeginDinnerConflict_ShouldNormalizeClockToSixPm`
    - `SpringDay1LateDayRuntimeTests.AlignTownDinnerGatheringActorsAndPlayer_ShouldPreferDinnerAreaOverVillageCrowdMarkers`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py errors --count 20 --output-limit 10` => `errors=0 warnings=0`
  - 两个目标脚本 `validate_script` 均为 `owned_errors=0`
- 当前用户下轮 live 预期：
  - 主动找村长触发晚饭，不会再因单个 NPC 没走到位而无限卡住
  - `+` 快进触发晚饭，也会走同一套 5 秒兜底
  - 最坏情况就是等满 5 秒后，`001/002` 被拉到晚饭区然后直接开戏
- 本轮已执行 `Park-Slice`，当前线程状态：`PARKED`
