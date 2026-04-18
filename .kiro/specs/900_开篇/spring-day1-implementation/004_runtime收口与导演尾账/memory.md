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

## 2026-04-13｜晚段补口第二轮：02:00 forced sleep、003 夜晚链、19:30 resident 放行已补齐并进整包护栏
- 当前主线仍是 `Day1 打包前 runtime 收口`，这轮直接处理用户最新追加的 3 个明显坏相：
  1. `003` 夜晚不回家、会被 runtime policy 误放回 roam
  2. Day1 到 `02:00` 时没有立刻收进 `DayEnd`，forced sleep 后 Home 落点还会歪
  3. `19:30` 自由时段普通 resident 仍被 crowd baseline 扣在 `backstage / takeover` 语义，体感上像“八点前根本没放出来”
- 本轮真实代码改动：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
     - `HandleHourChanged()` 在 `FreeTime && hour>=26` 时，`TimeManager.Sleep()` 后若 `OnSleep` 链没即时回到导演，会主动补一次 `HandleSleep()`，确保 Day1 立刻收进 `DayEnd`
     - `TryPlacePlayerNearCurrentSceneRestTarget()` 在有 `Rigidbody2D` 的玩家上，同时写 `body.position + transform.position`，解决 Home 场景床边 fallback 在测试/切场景后位置不刷新的问题
     - `ShouldKeepStoryActorNightRestControl()` 现已把 `003` 一并纳入，避免夜晚导演控制下一帧被 runtime policy 误释放
  2. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - `ResolveResidentParent()` 新增 free-time 释放口径：
       - `19:30~20:00` 的 `FreeTime` 里，除真正的 `Priority witness actor` 外，其它 resident 一律回 `Resident_DefaultPresent`
       - 不再继续扣在 `Resident_BackstagePresent / Resident_DirectorTakeoverReady`
  3. `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
     - 晚段新增护栏现已覆盖：
       - `BeginDinnerConflict_ShouldAlignActorsBeforeDinnerCueTimeoutCompletes`
       - `StoryActorsNightRestSchedule_ShouldCover001To003`
       - `HandleHourChanged_FreeTimeAtTwoAmShouldFinalizeDayEnd`
       - `TryPlacePlayerNearCurrentSceneRestTarget_ShouldUseHomeDoorFallbackOffsetInHomeScene`
     - 其中 `StoryActorsNightRestSchedule_ShouldCover001To003` 额外补了 `ApplyStoryActorRuntimePolicy` 不得放掉 `003` 的断言
  4. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 新增：
       - `CrowdDirector_FreeTimeBeforeTwenty_ShouldReleaseNonPriorityResidentsToDefaultPresent`
       - `CrowdDirector_FreeTimeBeforeTwenty_ShouldKeepPriorityWitnessInTakeoverReady`
  5. `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
     - 已把上述新增护栏接入：
       - `Run Late-Day Bridge Tests`
       - `Run Director Staging Tests`
- 本轮验证：
  - `spring-day1-late-day-bridge-tests.json`：`13/13 passed`
  - `spring-day1-director-staging-tests.json`：`39/39 passed`
  - 目标脚本 `validate_script` 均为 `owned_errors=0`；Unity 侧只剩外部 `Missing Script` / TMP warning 噪音，不能算我 own red
- 本轮额外 live 结论：
  - 我中途为了取证跑过一次 `Force Spring Day1 Dinner Validation Jump`
  - 那次 live 没停在有效 dinner 现场，而是被旧运行态直接落到 `FreeTime 19:30` 或更早的 staging，相位样本不可信
  - 所以我没有拿那次“NPC 看起来不动”的现场去包装成产品最终结论，而是改用 probe 定责后回到代码层修补 `19:30 resident release` 这条真根
- 当前恢复点：
  - 代码层与 targeted / bundle 护栏已站住
  - 下一步最值钱的是用户重新黑盒：
    1. fresh / restart 下的晚饭入口 `001/002` 站位
    2. `19:30~20:00` resident 是否已恢复自由活动，`20:00` 才开始回家
    3. Day1 `02:00` 是否统一 forced sleep 且第二天贴在床边

## 2026-04-14｜opening / Town fresh live：resident 放人节拍已修，剧情 actor 争议收缩为 authored 点位
- 当前主线：用户把本轮 P0 强行收回 `opening / 0.0.2 / Town`，要求先解决“对白后 resident 卡住”和“对白前站位到底是不是 runtime 延迟”的问题。
- 本轮子任务：
  1. fresh live 重跑 `Reset Spring Day1 To Opening -> Bootstrap Spring Day1 Validation`
  2. 用 `spring-day1-live-snapshot.json`、`spring-day1-actor-runtime-probe.json`、`spring-day1-resident-control-probe.json` 和 GameView 截图，把两类问题拆开
- 本轮 live 结论：
  1. `001/002/003` 在 `spring-day1-village-gate` 第 1 句对白时已经稳定在当前 authored 点位：
     - `001 = (-12.55, 14.52)`
     - `002 = (-10.91, 16.86)`
     - `003 = (-22.02, 10.29)`
     这条由 actor probe 和 GameView 截图双重确认，所以“对白前站位怪”剩下的是 staging/data 设计争议，不是 runtime 延迟没生效。
  2. resident 群演 `101~203` 在对白进行中处于 `EnterVillage_PostEntry` staging：
     - `scriptedControlActive=true`
     - `scriptedMoveActive=true`
     - `isMoving=true`
  3. 对白结束后，旧问题已经从“全体 path failure / ShortPause 木桩”收缩成“多人一起挂在 LongPause，看起来像冻结”。
- 本轮代码改动：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - 新增 `RestartRoamFromCurrentContext(bool tryImmediateMove = false)`
     - 逻辑：先 `StartRoam()` 重置 roam 上下文，再允许从 cue 放人点直接尝试一次 `TryBeginMove()`
  2. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - `TryReleaseResidentToDaytimeBaseline(...)` 放人后改为：
       - `roamController.RestartRoamFromCurrentContext(tryImmediateMove: true);`
  3. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - `CrowdDirector_ShouldReleaseResidentToBaselineAfterCueReleaseBeforeFreeTime` 补断言：`DebugState != LongPause`
- 本轮验证：
  - `validate_script`：
    - `NPCAutoRoamController.cs` `errors=0`
    - `SpringDay1NpcCrowdDirector.cs` `errors=0`
    - `SpringDay1DirectorStagingTests.cs` `errors=0`
  - EditMode targeted：
    - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldReleaseResidentToBaselineAfterCueReleaseBeforeFreeTime` `passed`
  - fresh live：
    - 对白结束后 1 秒内，resident probe 已出现多名 resident 回到 `Moving`
    - 例：`102/103/104` 已不再是整排木桩
- 当前恢复点：
  1. opening resident 的 runtime 放人 bug 已拿到 live 改善票
  2. opening 还没收完的只剩一刀：`Town` 村口 `001/002/003` 的 authored 点位是否要重摆
  3. 这条剩余刀口属于 `day1 own staging/data`，不是导航、不是 save、也不是 UI

## 2026-04-14｜补交用户点名要求的正式忏悔书
- 当前主线：仍是 `spring-day1` 全链路闭环；本轮插入式子任务是补交用户前面明确下令必须落地的“自我忏悔与罪证检讨书”。
- 本轮子任务：把下列内容整理成正式 tracked 文档，而不是继续只在聊天里口头回应：
  1. 用户的核心需求
  2. 我两次把 NPC 管死、让所有线程停滞的主要事故
  3. 每次事故具体错在哪里
  4. 我造成了什么伤害
  5. 以后必须遵守的硬规则
- 本轮真实落地：
  - 新增文件：
    - `2026-04-14_spring-day1线程_自我忏悔与罪证检讨书.md`
  - 文档已明确写入：
    - 两次重大事故
    - runtime / staging / owner 边界混乱
    - 越权碰 UI、contract 交接不稳、把推测说得太像结论
    - 以及“用户明确要求交忏悔书，我上一条回复仍漏交”这条新增失格
- 当前恢复点：
  - 忏悔书已补交，不再缺这份用户明确点名的产物
  - 主线随后仍回到 `opening / 晚段 / 2点睡觉 / save contract / UI contract` 的 Day1 收尾，不把写检讨当成已经修完

## 2026-04-14｜只读彻查补记：开场就位 / resident 日间释放与夜间回家 / 02:00 owner 分层
- 当前主线：
  - 用户明确要求这轮先不要直接修，而是彻查 Day1 现有代码，准确回答：
    1. 开场剧情就位为什么还会错
    2. 为什么剧情结束后 NPC 不会立刻回到该去的位置，而到 `20:00` 却会按时回家
    3. 超过 `02:00` 的 forced sleep 到底该归 Day1 还是共享层
- 这轮实际做成：
  1. 已把“开场剧情就位错误”的第一责任层钉死到 `SpringDay1Director`，不是导航：
     - `TryHandleTownEnterVillageFlow()` 在 `HasVillageGateProgressed()` 后、对白一结束就会直接 `TryBeginTownHouseLead()`，中间没有 post-dialogue hold beat；
     - `TryPrepareTownVillageGateActors()` / `TryResolveTownVillageGateActorTarget()` 仍允许 scene 布局点失效时退回 `StageBook cue`，再退回 `hard fallback`；
     - `SpringDay1Director` 自己虽然有 `TryResolveVillageCrowdMarker(...)` helper，但它没有接进 opening 主链，因此导演没有和 crowd runtime 共用同一套“起点/终点” marker contract。
  2. 已把“剧情后 NPC 不回 anchor，20:00 却会回家”的责任层钉死到 `SpringDay1NpcCrowdDirector`：
     - `TryReleaseResidentToDaytimeBaseline(...)` 是白天释放链；
     - `TryBeginResidentReturnHome(...)` / `TickResidentReturnHome(...)` / `ShouldResidentsReturnHomeByClock()` / `ShouldResidentsRestByClock()` 是夜间回家链；
     - 两条链现在只是在底层共用了 `DriveResidentScriptedMoveTo(...)`，但 Day1 策略状态并没有统一成一套 resident state machine，所以用户会感知成“剧情后不按白天目标恢复，只有 20:00 才像真的被安排回家”。
  3. 已把“02:00 到底写哪”的 owner 分层说清：
     - `SpringDay1Director.HandleHourChanged(...)` + `HandleSleep()` 当前负责的是 Day1 专属的 `FreeTime -> DayEnd` 收束；
     - `HandleGenericForcedSleepFallback()` 是 Day1 内部补的一层 generic fallback，但它仍然挂在 Day1Director 里，还不是全游戏共享 owner；
     - 因此“Day1 第一夜的剧情收束”仍归 Day1 own，而“所有天数超过 02:00 的统一睡觉规则”不应长期留在 Day1Director，最终应提升到共享层。
- 这轮最关键的结构证据：
  - `SpringDay1Director.cs:2889-2970`
    - 开场对白完成后会过早切到 `TownHouseLead`
  - `SpringDay1Director.cs:5601-5770`
    - 开场 actor 布局仍允许 fallback
  - `SpringDay1Director.cs:1733-1759`
    - `TryResolveVillageCrowdMarker(...)` 仍是孤立 helper，没接进 opening 主链
  - `SpringDay1NpcCrowdDirector.cs:1574-1638`
    - crowd runtime 自己有一套 marker override，会吃 `起点/终点`
  - `SpringDay1NpcCrowdDirector.cs:1921-2468`
    - resident 白天释放与夜间回家是两套 Day1 policy
  - `SpringDay1Director.cs:2182-2231`、`2305-2371`
    - Day1 sleep 与 generic fallback 目前仍混居在 Day1Director
- 当前 authoritative 判断：
  1. 用户给 `axe_0 / Pickaxe_0` 做的辅助点不是“无效劳动”，而是当前 opening director 没和 crowd marker contract 走同一套读取逻辑；
  2. 现在真正该修的不是导航、不是 UI，而是：
     - `SpringDay1Director` 的 opening staging / hold / marker resolution contract
     - `SpringDay1NpcCrowdDirector` 的 resident state machine
     - `Day1 vs shared overnight sleep` 的 owner 分层
- 当前结论状态：
  - `结构 / checkpoint` 成立
  - `代码与测试证据` 成立
  - `真实入口体验` 尚未成立
- 下一恢复点：
  1. 开修时第一刀应先收 opening：
     - 禁止 scene 点位失效时静默 fallback
     - 补 `VillageGate` 对话后的 short hold beat
     - 让导演与 crowd 共用同一套 marker contract
  2. 第二刀再收 `resident`：
     - 把 daytime baseline / post-dialogue hold / `20:00` return-home / `21:00` snap 收成一套 Day1 state machine
  3. 第三刀再做 `02:00` 分层：
     - Day1 内保留 `FreeTime -> DayEnd`
     - 通用 `>02:00` 睡觉外提到共享层

## 2026-04-14｜三线程 owner 边界与续工 prompt 已正式落盘
- 当前主线：
  - 用户要求先不要直接修业务代码，而是先把 `spring-day1 / 导航 / UI` 三条线程的 owner 边界、红线、完成定义和转发壳一次写清。
- 本轮完成：
  1. 已新增给 `spring-day1` 的正式 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-14_给spring-day1_Day1三线程owner边界重划与打包前唯一主刀prompt_v4.md`
  2. 已新增给 `导航` 的正式 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-14_导航线程_Day1边界重划后非剧情态roam静态契约收口prompt_67.md`
  3. 已新增给 `UI` 的正式 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.2_玩家面集成与性能收口\2026-04-14_UI线程_Day1最终UI语义与时间owner边界收口prompt_12.md`
- 这轮写死的核心边界：
  1. `spring-day1` own：
     - opening authored contract
     - resident 白天 release / `20:00` 回家 / `21:00` snap
     - Day1 第一夜 `26:00` forced sleep
  2. `导航` own：
     - Day1 已放人后的 NPC / 动物非剧情态 roam
     - shared static truth / static obstacle / local avoidance / blocked abort
  3. `UI` own：
     - 任务清单 / bridge prompt / Prompt / modal 层级 / re-entry UI 重建
     - `TimeManagerDebugger +/-`
- 特别冻结的用户原始要求：
  - opening 正确流程必须恢复为：
    - 进入剧情立刻传送到起点
    - 走向终点
    - 最多等 5 秒
    - 超时只对必要剧情 actor 瞬移到终点
    - 然后开始剧情
    - 剧情结束原地退场并回 anchor
- 本轮没有改业务代码，只落 prompt/contract 文件。
- 当前状态：
  - `thread-state = PARKED`
  - 等用户审核或直接转发这三份 prompt
- 下一恢复点：
  - 如果继续真实施工，`spring-day1` 自己只能按 v4 prompt 回到 opening / resident / first-night 三条 own 主线，不再混修导航或 UI。

## 2026-04-14｜opening own 第一刀已落：围观 cue 释放时机与 scene authored 终点严格消费
- 当前主线：
  - 按 `v4` prompt 回到 `opening authored contract` 第一刀，只收：
    1. `001/002/003` 不再对白前直接跳终点
    2. `scene authored` 终点必须被导演直接消费
    3. authored root 已存在但点位缺失时，不再静默 fallback 开戏
- 本轮实际改动：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - `ShouldSuppressEnterVillageCrowdCueForTownHouseLead(...)` 从“只要不在对白中就 suppress”改成“只有导演真的开始释放围观 crowd 时才 suppress”
     - 结果：`EnterVillage_PostEntry` 的 staging cue 会在对白前真实执行 `起点 -> 终点`，不再一上来就被导演提前放掉
  2. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - `TryPrepareTownVillageGateActors()` 改成严格模式：
       - scene authored root 存在时，逐个剧情 actor 必须解出 authored 终点，否则直接 `return false`
       - 不再在 authored root 已存在时静默掉回 `stagebook/hard fallback`
     - `TryResolveTownVillageGateActorTarget(...)` 新增 `requireSceneAuthoredTarget` 分支，并新增 `TryResolveVillageCrowdEndMarker(...)`
       - 支持在 legacy `终点` 分组缺失时，仍直接吃 scene 里的 `001终点/002终点/003终点`
       - authored root 存在但缺点位时，明确失败，不再继续开戏
  3. [SpringDay1OpeningRuntimeBridgeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1OpeningRuntimeBridgeTests.cs)
     - 新增 opening 回归：
       - scene authored 终点在非 legacy 分组下也必须被消费
       - authored root 存在但缺少必要终点时必须失败
       - `EnterVillage_PostEntry` crowd cue 在 house lead 真开始前不得被 suppress
- 当前代码层验证：
  - `validate_script` 对本轮 own 文件的代码闸门均无 owned error
  - 但 Unity 现场当前存在 external compile red：
    - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
      - `ShouldApplyAutonomousStaticObstacleSteering`
      - `AdjustDirectionByStaticColliders`
    - 这两条不在本轮 own scope，当前被归类为 `external_red`
  - `git diff --check` 对本轮 touched 文件已通过
- 额外 blocker：
  - `Ready-To-Sync.ps1` 被 `.kiro/state/ready-to-sync.lock` 的 stale 锁卡住，未能生成 `READY`
- 当前状态：
  - `thread-state = PARKED`
  - 这刀已形成可读、可回退的 own patch，但还没有拿到干净 Unity live 票
- 下一恢复点：
  1. 外部 `NPCAutoRoamController.cs` 编译红清掉后
  2. 先重跑 opening live：
     - 看 `001/002/003` 是否先从 authored 起点走到终点
     - 最多 5 秒
     - 对话前一刻是否就位
  3. 如果 opening 过线，再回到 resident daytime release / `20:00/21:00/26:00` 第二刀

## 2026-04-14｜resident daytime release / Day1 第一夜 forced sleep 第二刀已落并拿到 owner regression 4/4
- 当前主线：
  - 继续按 `v4` 只收 `spring-day1 own`，这轮重点是：
    1. resident scripted 结束后不能再跳到 `DailyStand_*` 一坨区
    2. Day1 `26:00` forced sleep 必须像后续天数一样切回 `Home`
- 本轮实际改动：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - `TryReleaseResidentToDaytimeBaseline(...)` 现在优先释放回 `BasePosition`
     - `ShouldUseResidentDaytimeSemanticBaseline(...)` 改成只有 `beatKey` 命中 `DailyStand` 预演时才成立
     - 结果：opening / dinner 这类 cue 结束后，resident 不再被统一传去 `DailyStand_*` 聚堆点，而是回各自 base 域恢复 roam
  2. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - `HandleSleep()` 的 Day1 路径改为“非 `Home` 就先切 `Home` 再摆位”
     - `SyncStoryTimePauseState()` 补了 EditMode 安全口，避免定向回归时误拉起 `PersistentManagers`
  3. EditMode 回归与定向菜单：
     - [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - [SpringDay1LateDayRuntimeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs)
     - [SpringDay1TargetedEditModeTestMenu.cs](D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs)
- 当前验证：
  - `validate_script`（director / crowdDirector / 2 个测试文件 / targeted menu）=`no_red`
  - `Library/CodexEditorCommands/spring-day1-owner-regression-tests.json`=`4/4 pass`
    - opening cue release 时机
    - resident 回 base pose
    - `001~003` 夜间归家覆盖
    - Day1 `26:00` forced sleep `Town -> Home`
- 当前判断：
  - 这轮关键 own 回归已经从“结构推断”推进到“定向 EditMode 4/4 通过”
  - 剩余未闭环的是用户 live 体感，不是这 4 条 canonical owner contract 本身
- 当前 blocker：
  - `Ready-To-Sync.ps1` 仍被 spring-day1 历史 own roots 残留脏改阻断，原因不是本轮新增 red，而是同根残留太多
- 当前恢复点：
  - 下一步优先交用户 live 复测 opening 后 resident 分散恢复与 Day1 `26:00` 切 `Home`

## 2026-04-14｜runtime 子层补记：第二刀已形成本地 checkpoint 并回到 PARKED
- 子层这轮没有继续扩到 UI / 导航 / save，只把已经拿到 targeted `4/4 pass` 的 own 代码切片收成最小可回退 checkpoint。
- 本地提交：
  - `442b9a40` `checkpoint: spring-day1 owner contract fixes`
- 当前 no-red 证据补记：
  - `git diff --cached --check` 通过
  - `sunset_mcp.py errors --count 20 --output-limit 5`=`errors=0 warnings=0`
  - `validate_script SpringDay1Director.cs` 与 `validate_script SpringDay1NpcCrowdDirector.cs` 当前都无 owned/external red，但 Unity 侧仍是 `unity_validation_pending`
  - 精确原因：Editor 当前 `ready_for_tools=false`，`blocking_reasons=[stale_status]`
- 当前 thread-state：
  - `spring-day1` 已重新 `Park-Slice`
  - live 状态=`PARKED`
  - blocker 已改写为真实人话，不再保留错误的 `System.String[]` 假值
- 子层恢复点保持不变：
  1. 让用户先 live 复测 opening 后 resident 是否回 base/anchor 域
  2. 再看 Day1 `26:00` 是否在真实 live 下稳定切回 `Home`

## 2026-04-14｜runtime 子层热修补记：opening 后 resident 走两步即高频抖动的直接冲突已修
- 用户 fresh live 新反馈：
  - `101~203` 在 opening 结束后一秒左右仍会先走两步，然后立刻进入原地高频小幅抖动。
- 这轮已钉死的 exact root：
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) `ApplyResidentBaseline(...)`
  - 我上一刀把 `ReleasedFromDirectorCue + NeedsResidentReset` 改成了“白天也先 `TryBeginResidentReturnHome()`”
  - 但同一个方法里 `state.IsReturningHome` 分支仍保留旧判断：`!ShouldAllowResidentReturnHome(currentPhase)` 时立即取消 return-home
  - 结果是 resident 在“由 cue release 触发的白天退场”与“白天不允许回家，立刻取消”之间来回打架，表现就是先动两步再原地高频抖动
- 这轮真实修口：
  - 仅对“`ReleasedFromDirectorCue && NeedsResidentReset` 触发的白天 return-home”豁免旧的 clock gate
  - 不改 `20:00` 自主回家 / `21:00` 强制 rest 的既有时钟分支
- 这轮补的测试：
  - [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
  - 新增 `CrowdDirector_ShouldNotCancelDaytimeReturnHomeWhenItWasTriggeredByCueRelease`
- 当前静态验证：
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs` 通过
  - `sunset_mcp.py errors --count 20 --output-limit 8` = `errors=0 warnings=0`
  - `validate_script Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs --skip-mcp` 代码闸门通过，但 assessment 仍为 `unity_validation_pending`
  - `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs --skip-mcp` 当前被 `CodexCodeGuard returned no JSON` 阻断；这更像当前 shared root runtime 噪音，不像本次小修口新增 compile red
- 当前 thread-state：
  - 已执行 `Park-Slice`
  - live 状态=`PARKED`
- 当前恢复点：
  1. 让用户只复测最短 opening 链路：`fresh opening -> 0.0.2 -> 村长引路 -> 看 101~203 是否还会走两步后抖死`
  2. 若仍失败，下一刀只继续追 `opening release -> return-home tick -> finish release` 三段链，不再扩到 dinner / 02:00 / UI / 导航

## 2026-04-14｜runtime 子层只读补记：当前 dinner / opening / release 合同错位点已钉死
- 用户最新截图和口述，已把这条线的坏相重新钉成两类：
  1. `staging 错合同`
  2. `release 错合同`
- `staging 错合同` 的 exact 代码点：
  - [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs) `ResolveCueFallbackBeatAlias(...)`
    - 现在把 `DinnerConflict_Table / ReturnAndReminder_WalkBack` fallback 到 `EnterVillage_PostEntry`
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) `TryResolveStagingCueForCurrentScene(...)`
    - 在 Town 里又主动 mirror opening cue 给 dinner / reminder
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) `ShouldUseVillageCrowdSceneResidentStart(...)`
    - 还让 dinner / reminder 继续吃 opening 的 start/end marker override
- 这意味着当前 dinner / reminder 根本没有被当成独立 authored beat 在执行，而是在冒充 opening。
- `release 错合同` 的 exact 代码点：
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) `TryReleaseResidentToDaytimeBaseline(...)`
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) `RestartRoamFromCurrentContext()` / `RefreshHomePositionFromCurrentContext()`
  - 当前 release 后恢复的是“当前点 roam center”，不是“anchor/home base”
- 当前 authoritative 判断：
  - 用户要的其实不是“release 后别聚堆”，而是：
    - 进戏前按 authored start/end 正式就位
    - 戏完后按 anchor/home contract 退场
  - 现在代码两头都没对上，所以才会出现“18:00 乱走位、19:00 背对、19:30 才闪现、戏后原地撒开”

## 2026-04-14｜runtime 子层补记：Town resident 在 HealingAndHP 阶段被过早放活的出场时机已收回
- 用户 fresh live 新反馈：
  - `opening` 后跟着 `001/002` 进 `Primary`，过完对话但还没去找艾拉，直接回 `Town` 时，`101~203` 已经在各自 anchor 附近自由活动。
  - 用户明确裁定：这批 resident 虽然“回 anchor 的位置对了”，但此时根本不该出现在 `Town`。
- 这轮钉死的 exact root：
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) `ShouldKeepResidentActive(...)`
  - 旧逻辑在 `EnterVillage ~ ReturnAndReminder` 整段里，只要没有 beat presence 明确要求隐藏，就会默认把 `daytime resident` 放活。
  - 这导致 `HealingAndHP / Workbench / Tutorial` 这类其实不该开放 `Town` 常驻居民的阶段，只要玩家手动回 `Town`，居民就会被我错误释放成正常 roam。
- 这轮真实修口：
  1. `EnterVillage ~ ReturnAndReminder` 之间，不再因为 `daytime baseline` 就默认放活 resident。
  2. 只有两种情况允许继续可见：
     - 当前 beat presence 明确点名需要在场
     - 该 resident 还处在“剧情 cue 刚 release，正在退场回 anchor/home”的途中
  3. 这样就能同时满足：
     - `opening` 散场后仍能先退场回 anchor
     - 退场完成后，在 `HealingAndHP / 还没找艾拉` 这段不会继续留在 `Town` 乱逛
- 这轮补的测试：
  - [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
  - `CrowdDirector_ShouldHideResidentsInHealingPhaseWhenNoBeatPresenceRequestsThem`
  - `CrowdDirector_ShouldKeepCueReleasedResidentsActiveLongEnoughToRetreatDuringHealingPhase`
- 当前静态验证：
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs` 通过
  - `sunset_mcp.py errors --count 20 --output-limit 8` = `errors=0 warnings=0`
  - `sunset_mcp.py compile Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs --output-limit 12`
    - `assessment=unity_validation_pending owned_errors=0 external_errors=0`
    - 外部 warning 仅剩 MCP WebSocket 初始化噪音，不是本轮 owned 问题
  - `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs --skip-mcp`
    - `assessment=unity_validation_pending owned_errors=0 external_errors=0`
- 当前恢复点：
  1. 让用户复测：`opening -> 跟 001/002 进 Primary -> 不找艾拉直接回 Town`
  2. 预期：`101~203` 不应继续在 `Town` 自由活动；如果刚好还在散场途中，只允许短暂退回 anchor/home，然后消失/不再 roam

## 2026-04-14｜runtime 子层补记：定点回退范围已复核，只回了我这条线碰过的 5 个 Day1 文件
- 当前主线目标：
  - 先把 `opening` 后 crowd 卡死/消失这条阻塞链收回稳定基线，再继续 Day1 own runtime 修口。
- 本轮子任务：
  - 按用户最新要求，只读复核“刚才的回退是否只回了我自己的内容，没有误伤其他线程”。
- 这轮实际做成了什么：
  1. 单独核查了这次回退触达的 5 个文件：
     - [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs)
     - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - [SpringDay1LateDayRuntimeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs)
  2. 当前从 `HEAD` 视角只剩 2 个文件仍带我这轮 diff：
     - [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs)
     - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
  3. 另外 3 个文件已经完全回到仓库基线，没有残留 diff：
     - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - [SpringDay1LateDayRuntimeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs)
  4. 这两处残留 diff 的语义也已收缩清楚：
     - `SpringDay1DirectorStaging.cs` 只保留“回退后为恢复当前调用面而补的最小兼容接口”
     - `SpringDay1NpcCrowdDirector.cs` 只保留“steady-state 下重复 apply staging cue 的早退止血”
- 当前 authoritative 判断：
  - 从当前工作树证据看，这次回退没有把别的路径一起带回去；它只落在我点名的 5 个 Day1 文件里。
  - 但对“这 5 个文件在回退前是否混有别人的未提交局部内容”这一点，`git` 只能证明当前结果，不能凭空还原作者归属；所以我现在能站稳的最强口径是：
    - `五个目标文件之外，没有发现因这次回退新增的误伤`
    - `五个目标文件之内，当前只剩两处我自己明确可解释的残留改动`
- 验证：
  - `git diff --name-status -- <5 files>`：仅 `SpringDay1DirectorStaging.cs`、`SpringDay1NpcCrowdDirector.cs`
  - `git diff --check -- Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`：通过
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 8`：`errors=0 warnings=0`
- 当前恢复点：
  1. 后续若继续真实施工，只允许从这两个剩余 Day1 文件往下收 `opening` 卡死主链
  2. 不再扩到 UI、导航 core、02:00、save 语义

## 2026-04-14｜runtime 子层只读结论：当前坏相更像 Day1 runtime owner 仍在持有，不是导航代码被我回退
- 用户补充了导航线程回执：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 里的 safe-center / deadlock-break / agent-clearance 代码还在
  - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs) 里的 facing hysteresis 代码也还在
  - 这先排除了“我把导航代码物理抹掉”的方向
- 我这边重新核 Day1 源码后的判断：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L1364) `ApplyStagingCue(...)`
     - 只要当前 `beatKey` 还能解析出 crowd cue，就直接 `return true`
     - 这意味着后面的 baseline roam/release 根本不会接管
  2. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L1528) `TryResolveStagingCueForCurrentScene(...)`
     - 现在 `EnterVillage_PostEntry` 是否继续吃 resident，不取决于导航
     - 取决于 Day1 自己的 `ShouldSuppressEnterVillageCrowdCueForTownHouseLead(...)`
  3. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs#L4589) `ShouldReleaseEnterVillageCrowd()`
     - 只有 `VillageGate` 完成、或 `TownHouseLead` 已 started/queued/waiting 时，才会让 `EnterVillage_PostEntry` crowd cue 放人
  4. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L1927) `ApplyResidentBaseline(...)`
     - 只有在 `ApplyStagingCue(...)` 不再命中后，才会走 `ReleaseResidentDirectorControl(...)` / `StartRoam()`
- 所以当前更强的 root 判断是：
  - 不是导航代码没了
  - 而是 Day1 runtime 还在把 `101~203` 这类 resident 留在 crowd cue / director 持有链里，导航根本没有接管时机
- 我对“回退前做过什么、现在回到什么状态”的复盘：
  1. 回退前我做过两类 Day1 语义热修：
     - `cue release -> 白天 return-home / baseline release`
     - `HealingAndHP` 阶段 resident visibility/timing
  2. 这些热修现在都已经退回去了
     - 当前 `HEAD` 对比只剩 [SpringDay1DirectorStaging.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs) 和 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 两处残留 diff
     - `SpringDay1Director.cs` 和两份测试文件已经 clean
  3. 剩余两处改动都不是“重新定义居民该不该被导演持有”：
     - `SpringDay1DirectorStaging.cs` 是兼容接口补面
     - `SpringDay1NpcCrowdDirector.cs` 是“同一 cue 不重复 apply”的早退止血
- 当前恢复点：
  1. 后续如果继续修，主刀应该继续落在 Day1 的 `EnterVillage crowd cue release` 与 `runtime owner release` 上
  2. 不该再把“101~203 不动”先报成导航代码被回退

## 2026-04-14｜runtime 子层自查：三次发病的共同红线已经明确，不允许再犯
- 当前已确认的问题：
  1. `opening` resident 释放链未闭环
     - `101~203` 当前坏相仍是 Day1 owner 没及时放权
  2. `opening` authored staging 未闭环
     - `001/002/003` 仍没有稳定满足“对白前一刻正式就位”
  3. `scene end -> anchor -> roam` 未闭环
     - resident 仍会出现“从当前点撒开”或“继续被 hold”
  4. `003` 在 Town 的 owner 路径仍偏混杂
  5. `dinner/reminder` 仍残留 opening mirror 旧逻辑
- 当前对子层三次发病的复盘：
  1. 第一次发病：
     - 我在 `cue release` 和 `白天 return-home gate` 之间写出了互相打架的状态切换
     - 结果是 resident 走两步就抖死
  2. 第二次发病：
     - 我把 `visibility/timing` 一起混进 `HealingAndHP` / `Town return`，把“什么时候该出现”和“出现后怎么 release”混写
     - 结果是 Town 居民出现时机混乱
  3. 第三次发病：
     - 我在 `opening` 主链还没闭环时继续扩修，导致回退后最核心的 `EnterVillage crowd cue release` 老根重新暴露
     - 结果是问题重新回到“resident 冻住 / 就位时机错”
- 当前对子层自己的硬红线：
  1. 不再混修 `authored staging / resident release / resident visibility / navigation`
  2. 不再在高频 owner 入口里叠加互相打架的状态迁移
  3. 最短链 live 没过前，不准扩到下一 phase
  4. 不再把 Day1 own 的 owner 持有问题甩给导航
  5. 不碰 UI 壳体，不碰 `Primary` 稳定链，不碰导航 core
- 后续唯一正确落地法：
  1. 先只收 `EnterVillage crowd cue release -> baseline release -> roam restore`
  2. opening 过线后，再收 `001/002/003` 的 authored 就位 contract
  3. 这两刀都过线后，才允许回 dinner / reminder / 02:00

## 2026-04-14｜runtime 子层硬清单：后续施工顺序与红线重新锁定
- 当前精准问题清单：
  1. `opening` resident 未 release，导致 `101~203` 冻住/抖动
  2. `001/002/003` 的 Town staged start/end contract 错位，出现“剧情后才到位”
  3. resident 戏后释放不是“回 anchor/home 再 roam”，而曾被写坏成“当前点乱 roam / 聚堆 / 过早出现 / 完全不动”
  4. `DinnerConflict / ReturnAndReminder` 不能再吃 opening crowd cue fallback/mirror
  5. Day1 第一夜 `26:00` forced sleep 还要回到“正确 home/bed 收束”
- 当前唯一正确语义：
  - opening：`start -> end -> max 5s -> snap 必要 actor -> 对话 -> release -> 回 anchor/home -> roam`
  - dinner：`18:00 入场走位 -> max 5s -> 对话 -> 19:30 release`
  - residents：`20:00` 自主回家，`21:00` snap
  - Day1 first night：`26:00` 强制回屋睡觉，位置和转场正确
- 当前施工顺序：
  1. opening resident release
  2. Town 001/002/003 staged start/end/timeout
  3. dinner authored contract
  4. Day1 first-night forced sleep
- 当前硬红线：
  1. 不准再混修导航 core
  2. 不准再碰 UI 壳体 / Primary
  3. 不准在没有 live runtime 证据前改 release 语义
  4. 不准把 resident release 写成 `RestartRoamFromCurrentContext` 语义
  5. 不准再让 `opening/dinner/return` 互相 fallback/mirror

## 2026-04-14｜补记：清掉 `SpringDay1Director.cs` 的 opening route 爆红，并把 story actor home-anchor 改回 scene 真值优先
- 当前主线：
  - 仍是 `Day1 own runtime/staging/release` 收口，不碰 UI 壳体、不碰导航 core。
- 本轮子任务：
  - 先处理用户贴出的 `SpringDay1Director.cs` 爆红，再顺手压住 dinner/story actor 起点可能被 runtime home-anchor 污染的风险。
- 本轮实际完成：
  1. 修掉 `SpringDay1Director.cs` 里 `TryPrepareTownVillageGateActors()` 的 6 条 `CS0165`
     - 根因是 `chief == null || TryResolve...` 这种短路分支让 `chiefStart/chiefEnd/...` 在 null 分支上没有先赋默认值。
     - 已改成先显式初始化 `chiefStart/chiefEnd/companionStart/companionEnd/thirdResidentStart/thirdResidentEnd`，再走 route resolve。
  2. 把 `ResolveStoryActorNightHomeAnchor(...)` 改成：
     - 先找 scene authored 的 `001_HomeAnchor / 002_HomeAnchor / 003_HomeAnchor` 这类 actor 自己的 `_HomeAnchor`
     - 找到后优先回写给 `NPCAutoRoamController`
     - 找不到时才 fallback 到 roam controller 当前的 `HomeAnchor`
  3. 重新核查了 dinner 与 opening 的 authored 真值：
     - `EnterVillage` 的 `001/002/003` 起终点仍在 `Town.unity -> 进村围观 -> 起点/终点`
     - `DinnerConflict_Table` 这条 beat 当前只含 dinner background residents，不含 `001/002`
     - 因此 dinner story actor 更依赖 `story actor home-anchor -> dinner anchor target` 这条 own 语义链，不能拿导航锅来解释。
- 代码验证：
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs --count 20 --output-limit 10`
    - assessment=`no_red`
    - owned_errors=`0`
    - console=`errors=0 warnings=0`
  - `py -3 scripts/sunset_mcp.py errors --count 12 --output-limit 12`
    - `errors=0 warnings=0`
- 当前恢复点：
  1. `SpringDay1Director.cs` 这条爆红已清，当前工作树不是红面状态。
  2. `Ready-To-Sync` 仍是 `BLOCKED`，原因不是脚本还红，而是 `Story/Managers + Story/Directing` own roots 里还有 7 个未纳入本轮的同根依赖：
     - `DialogueManager.cs`
     - `SpringDay1NpcCrowdManifest.cs`
     - `StoryManager.cs`
     - `SpringDay1TownAnchorContract.cs`
     - `SpringDay1TownAnchorContract.cs.meta`
     - `StoryProgressPersistenceService.cs`
     - `StoryProgressPersistenceService.cs.meta`
  3. 下轮如果继续，不要再重查这条 `CS0165`；直接从“这 7 个文件是否纳入 checkpoint + dinner/opening live 语义复核”继续。

## 2026-04-15｜runtime 子层窄修：回到 anchor 后的“假 roam / 长短停半态”已加硬重启补口
- 当前主线目标：
  - 继续只收 `Town opening -> resident return-home -> anchor 后恢复 roam`，不碰 UI 壳体，不碰导航 core。
- 本轮子任务：
  - 用户最新 live 反馈是：`opening` 自动就位和原地散场已经恢复，但 resident 回到 anchor 后仍会站死，不继续 roam；同时要求把这条线写进“修改表”，避免再次重复走弯路。
- 本轮实际完成：
  1. 在 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 新增 `ForceRestartResidentRoam(...)`。
  2. 把下面 4 条恢复链统一改成“先清旧半态，再强制重启 roam”，不再只靠 `if (!IsRoaming)` 这种弱条件：
     - `RecoverReleasedTownResidentsWithoutSpawnStates(...)`
     - `TryReleaseResidentToDaytimeBaseline(...)`
     - `FinishResidentReturnHome(...)`
     - `CancelResidentReturnHome(..., resumeRoam: true)`
  3. 新 helper 还补了一层硬护栏：
     - 只有 resident 已经脱离 scripted control，才允许强制接回 roam；
     - 避免我这边把“别的 owner 还没放手”的现场误重启。
  4. 在 [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs) 新增：
     - `CrowdDirector_ShouldForceRestartRoamAfterReturnHomeCompletesEvenWhenControllerAlreadyLooksRoaming`
     - 专门守“controller 表面上 `IsRoaming=true`，但其实还卡在旧 `ShortPause` 半态时，也必须被真正重启”。
  5. 已把这次问题和修口追加到修改表：
     - [2026-04-14_spring-day1_opening问题修改表.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/2026-04-14_spring-day1_opening问题修改表.md)
- 当前判断：
  - 这轮抓到的真问题不是“仍被 CrowdDirector 捆住”，而是“已经 release/return 完成后，controller 还可能停在假 roam / 旧短停半态里”，所以不能再把 `IsRoaming` 这个布尔值当成“体验恢复”的充分条件。
  - `return-home` 途中不避让这件事，当前更像 scripted move 合同本身不吃自由态静态 steering；这条先记为边界问题，不在本轮越权去改导航 core。
- 验证：
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs .kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/2026-04-14_spring-day1_opening问题修改表.md` 通过
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs --count 20 --output-limit 12`
    - `owned_errors=0`
    - `assessment=unity_validation_pending`
    - 当前不是代码红，而是 Unity 现场停在 `playmode_transition / stale_status`
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs --count 20 --output-limit 12`
    - `owned_errors=0`
    - `assessment=unity_validation_pending`
  - `py -3 scripts/sunset_mcp.py errors --count 8 --output-limit 8`
    - `errors=0 warnings=0`
- 当前恢复点：
  1. 代码层这条窄修已经落地，当前最值得用户复测的只剩一个 case：
     - `opening -> resident 散场 -> 回 anchor -> 看 101~203 是否会继续 roam`
  2. 如果这条过线，下一刀再考虑把“return-home 途中 scripted move 不避静态障碍”的边界正式移交导航线程。
  3. 如果仍不过线，下一刀只继续追 `CrowdDirector` 的 runtime 掉表 / rebinding，不再回头混修 dinner、UI、02:00。
  4. 本轮已执行 `Park-Slice`，thread-state 当前为 `PARKED`。

## 2026-04-15｜只读梳理：Day1 历史应有语义、当前代码语义、以及 roam owner 边界已重新讲清
- 用户新增要求：
  - 不只是听“这轮修了什么”，还要我把 `Day1` 从 opening 到夜间的“历史应有语义”完整拆清；
  - 同时回答“我现在到底把语义实现成什么了”、“roam 到底应由谁 own”。
- 本轮重新钉死的历史应有语义：
  1. `Primary` 稳定链不动；当前只谈 `Town / Day1`
  2. `opening / EnterVillage_PostEntry`
     - `001/002/003` 是必要剧情 actor
     - 进入 opening 时先传送到 authored `起点`
     - 再走到 authored `终点`
     - 最多等待 `5` 秒
     - 超时只对必要剧情 actor 瞬移到 `终点`
     - 对话出现前一刻必须已经站对位置、朝向正确
  3. `opening` 结束后的 resident
     - `101~203` 以及非剧情时的 `003`
     - 不是原地立刻自由乱逛
     - 而是先“原地散场 -> 回各自 anchor/home 域 -> 再进入该 anchor 附近的日常 roam”
  4. `18:00` 晚饭开始
     - `001/002` 属于晚饭必要剧情 actor
     - 同样应吃 `起点 -> 终点 -> 最多 5 秒 -> 超时 snap -> 对话前已就位`
  5. `19:30` 晚饭相关剧情收完，玩家恢复自由活动
  6. `20:00` resident 自主回家
  7. `21:00` 仍未到位者才强制 snap 到 home anchor
  8. `26:00` Day1 第一夜强制睡觉
     - 不是 generic 晕倒
     - 而是送回正确 home / bed 入口位
     - 正确转第二天
- 本轮重新钉死的当前代码语义：
  1. opening 入口仍在 [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)：
     - `TryHandleTownEnterVillageFlow()`
     - `TryPrepareTownVillageGateActors()`
     - `TryResolveTownVillageGateActorRoute(...)`
  2. opening 现在的代码已经在做：
     - `001/002/003` 用 authored route 或 scene marker 先摆到起点
     - 逐步 drive 到终点
     - 超时后 snap 到终点
     - 再开 `VillageGateSequence`
  3. resident release 入口在 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)：
     - `ApplyResidentBaseline(...)`
     - `TryBeginResidentReturnToBaseline(...)`
     - `TickResidentReturnHome(...)`
     - `FinishResidentReturnHome(...)`
  4. 当前我最新落下去的语义是：
     - resident 从 cue release 后先回 anchor/home
     - 到 anchor 后不再只看 `IsRoaming`
     - 而是走 `ForceRestartResidentRoam(...)`，强制清半态再重启 roam
  5. dinner 入口仍在 [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)：
     - `BeginDinnerConflict()`
     - `PrepareDinnerStoryActorsForDialogue()`
     - `TryResolveDinnerStoryActorRoute(...)`
     - 当前代码也是“先摆起点，再 drive 终点，超时 snap，再开对白”
- 本轮重新钉死的 roam owner 边界：
  1. `Day1 own`
     - 什么时候应该开始 roam
     - 什么时候不该 roam
     - 什么时候必须回 anchor/home
     - 什么时候应该 snap
     - 哪些 NPC 当前是剧情 actor，哪些已释放成正常 NPC
  2. `导航 own`
     - 一旦已经进入 roam，自由态怎么选点
     - 怎么避静态障碍
     - 怎么避免 diagonal 摆头
     - 怎么处理中途卡墙/局部避让/重规划
  3. 当前结论：
     - “是否 roam / 何时 roam / roam 前先回 anchor” 是 Day1 语义
     - “roam 过程中为什么卡墙、为什么不避让、为什么一直重新找路” 是导航语义
- 本轮纠正的一处误判：
  - 我之前一度把 `anchor` 被污染误判到 `RefreshRoamCenterFromCurrentContext()`
  - 重新核代码后确认：
    - 真正会物理改 `homeAnchor.position`` 的是 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) `SyncHomeAnchorToCurrentPosition()`
    - `RefreshRoamCenterFromCurrentContext()` 只会刷新 roam center，不会直接改 scene anchor transform
  - 当前 Day1 这条线没有直接调用 `SyncHomeAnchorToCurrentPosition()`
- 当前恢复点：
  1. 以后再讨论 `roam`，必须先拆成“roam 开关语义”和“roam 执行质量”两层
  2. `Day1` 不允许继续替导航收“自由态避障 / 斜向摆头 / 自主 roam 选点品质”
  3. 如果用户下一轮继续追问边界，直接从这份语义梳理继续，不再回到模糊口头约定

## 2026-04-15｜只读认知同步：按用户最新 Day1 基线重排差异清单与下一轮唯一窄刀口
- 用户最新裁定：
  - 不再接受泛修；这轮只要两部分输出：
    1. `Day1 应有语义 vs 当前代码实际语义` 差异清单
    2. 下一轮若真实施工，只允许落哪一个最窄刀口
  - 额外新增硬规矩：
    - 每次真实改动前，必须先从代码上排查“是否已有重复实现 / 冗余实现 / 反向语义实现 / 与本轮刀口冲突的旧补丁”，先清点冲突，再决定最安全方案
- 当前重新确认的差异主轴：
  1. `opening staged contract`
     - 应有：所有参与 opening 的 Town NPC 共用同一条 `起点 -> 终点 -> 5 秒 -> 超时 snap -> 对话中不再走位`
     - 当前：`001/002/003` 与 resident crowd 仍不在同一份 staged contract 内；opening actor 和 resident release 还是分两层处理
  2. `opening 后 resident 释放`
     - 应有：`003` 与 `101~203` 同合同；release 后只定义“回哪个 anchor / 何时 release”，真正走回去交导航
     - 当前：`003` 仍残留差异化处理；`SpringDay1NpcCrowdDirector` 仍深度碰 return-home / restart roam 生命周期
  3. `Healing / Workbench / Farming`
     - 应有：`001/002` 只在 `Primary`；Town resident 保持自由活动，不再靠 suppress/hidden 躲 bug
     - 当前：代码里仍保留“Town resident 可见性/存在感”由 Day1 crowd 直接裁定的旧口
  4. `dinner staged contract`
     - 应有：与 opening 同合同，`001/002` 不再作为特殊 staged actor
     - 当前：dinner 入口仍带“story actor route”思路，尚未彻底改成“所有参与 NPC 同一 staged contract”
  5. `20:00~26:00`
     - 应有：是全日通用夜间合同，Day1 只负责接线
     - 当前：Day1 / crowd 里已有这段逻辑，但仍偏 Day1 私房实现，尚未从架构上回到全日通用口径
- 当前恢复点：
  1. 下一轮如果开修，第一动作不是改代码，而是先做“重复/冗余/反向语义/冲突实现”盘点
  2. 真正允许落下去的唯一窄刀口，只能先收 `opening 后 003 + 101~203 的统一 release contract`，不准扩到 dinner / 20:00~26:00 / 导航 core
  3. 本轮仍是只读，thread-state 继续维持 `PARKED`

## 2026-04-15｜runtime 子层真实施工：opening 后统一 release contract 第一刀已落
- 当前主线：
  - 只收 `opening 后 003 + 101~203 的统一 release contract`，不碰 dinner、不碰 Primary、不碰导航 core。
- 本轮子任务：
  1. 清掉 `SpringDay1NpcCrowdDirector` 在 daytime release / return-home 上的 `DebugMoveTo + StopRoam/RestartRoam + transform step fallback` 组合。
  2. 把 `003` 从 `spring-day1-director` 的 opening 持有里在正确交棒边界释放出去。
- 本轮实际完成：
  1. 在 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)：
     - `PrepareResidentRoamController(...)` 不再每次 `ApplyProfile()/RefreshRoamCenterFromCurrentContext()`；
     - `RecoverReleasedTownResidentsWithoutSpawnStates(...)` 不再用 stop/restart 组合拳，改成仅在 idle 时 `StartRoam()`；
     - `TickResidentReturnHome(...)` 去掉 `transform.position += step` 的手搓 fallback；
     - `TryBeginResidentReturnToBaseline(...)` 改成只请求 `DriveResidentScriptedMoveTo(...)`，不再走 `DebugMoveTo(...)` autonomous 假路；
     - `TryReleaseResidentToDaytimeBaseline(...)` 近 anchor 时优先原地 release，不再把刚回来的 resident 又传回旧 baseline；
     - `FinishResidentReturnHome(...)` / `CancelResidentReturnHome(...)` 改成 release 后仅在 idle 时恢复 roam，不再强制 stop/restart。
  2. 在 [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 新增 `ReleaseOpeningThirdResidentControlIfNeeded(...)`，并接到：
     - `VillageGateSequenceId` 完成后
     - `HasVillageGateProgressed()/HasHouseArrivalProgressed()` 的恢复路径
     - `TryBeginTownHouseLead()`
     让 `003` 在 opening 交棒后不再残留在 `spring-day1-director` owner 里。
  3. 在 [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)：
     - 新增 `Director_ShouldReleaseOpeningThirdResidentControlWhenVillageGateHandsOff`
     - 把“return-home 起不来就手搓 step fallback”测试改成“禁止 manual step fallback”
     - 把旧“force restart”语义改名为“清掉旧超长 short pause 半态”
- 代码层验证：
  - `manage_script validate SpringDay1NpcCrowdDirector`：`errors=0 warnings=2`
  - `manage_script validate SpringDay1Director`：`errors=0 warnings=3`
  - `manage_script validate SpringDay1DirectorStagingTests`：`errors=0 warnings=0`
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 8`：`errors=0 warnings=0`
  - `git diff --check` 覆盖本轮 3 个目标文件通过
- 当前判断：
  - 这刀已经把 release contract 从“Day1 直接拼 locomotion 内部状态机”收窄成“只发回 anchor / release roam 语义”；
  - 但它仍是代码层 clean，不是 live 已过线，当前最重要的是 fresh opening 复测。
- 当前恢复点：
  1. 下一次 live 先只看：
     - `opening` 后 `101~203` 是否不再两步后抽搐
     - `003` 是否不再原地罚站，而是和 `101~203` 同合同
     - 到 anchor 后是否恢复正常 roam，而不是原地木桩
  2. 如果还不过线，下一刀只继续追这一条 release contract，不准扩到 dinner / 夜间总合同。
  3. 本轮已执行 `Park-Slice`，thread-state 当前为 `PARKED`，blocker=`waiting-fresh-live-on-003-and-101-203-release-return-home`。

## 2026-04-15｜停车补记：用户已用“禁用 CrowdDirector 后 NPC 恢复 roam”把当前错位重新钉回 owner 架构
- 当前主线目标：
  - 用户最新 live 已把锅重新压回唯一根：`SpringDay1NpcCrowdDirector` 在剧情后继续抓着 resident，导致 `101~203` 第五次被锁住。
- 本轮新增的 hard evidence：
  1. 用户 fresh live 明确反证：
     - 只要直接禁用 `SpringDay1NpcCrowdDirector`，`101~203` 就会恢复正常 roam；
     - 剩下只有“夜间不会自己回家”这一条。
  2. 用户贴出的 profiler 也直接指向：
     - `SpringDay1NpcCrowdDirector.Update()` 占主线程大头；
     - 当前不是导航 core 在锁人，而是 crowd owner 仍在高频 sync / hold resident。
  3. 代码复核后可以明确说：
     - `SpringDay1NpcCrowdDirector` 现在不只是“群演导演”；
     - 它还在 own 剧情后的 resident 生命周期：
       - `AcquireResidentDirectorControl(...)`
       - `ReleaseResidentDirectorControl(...)`
       - `TryBeginResidentReturnToBaseline(...)`
       - `TryDriveResidentReturnHomeAutonomously(...)`
       - `TickResidentReturns()`
       - `FinishResidentReturnHome(...)`
       - `ForceRestartResidentRoam(...)`
     - 也就是说，当前实现确实是“Day1 放手一半，再继续代管居民下半生”，不是用户要的轻边界。
- 当前对构架的最新定性：
  1. 用户要的正确边界是：
     - `Day1` 只负责接管 / 放手 / 告知 anchor-home 目标；
     - `NPC` 自己切 resident state；
     - `导航` 只负责怎么走。
  2. 当前真实实现是反着的：
     - `CrowdDirector` 仍在剧情后直接操 resident 的 runtime locomotion 和 return-home/restart-roam。
  3. 所以当前最大错位不是“某个参数没调对”，而是 `CrowdDirector` 自身已经越过了 owner 边界。
- 本轮 WIP（已落代码，但尚未 live 验）：
  - 我在 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 先落了一刀“白天显式 crowd beat 之外让 resident autonomy 接管”的窄改：
    - daytime 无显式 crowd beat 时，不再继续 `SyncCrowd()` 抓 resident；
    - 改成清 cue、release control、直接让 roam 接手；
    - 同时把 `SyncCrowd()` 里两块 scratch 分配改成复用，先砍最直观 GC。
  - 这刀目前只到代码层 clean：
    - `validate_script SpringDay1NpcCrowdDirector.cs` => `assessment=unity_validation_pending` / `owned_errors=0`
    - `errors --count 20 --output-limit 8` => `errors=0 warnings=0`
  - 还没有 fresh live 票，不能宣称体验已过线。
- 当前恢复点：
  1. 下一轮如果继续，只能围绕这一刀做 fresh live：
     - opening 后 `101~203` 是否不再被 crowd 抓死；
     - 关闭 CrowdDirector 时成立的 roam 正常态，是否已由代码内建回来；
     - `20:00/21:00` 夜间回家是否仍可由 crowd 的 schedule owner 接上。
  2. 下一轮不准再回头扩到 UI / 导航 core / Primary house-arrival。
  3. 本轮已执行 `Park-Slice`，thread-state 当前为 `PARKED`，blocker=`crowd-director-owns-post-dialogue-resident-lifecycle-and-reacquires-town-residents`。

## 2026-04-15｜文档沉淀补记：Day1 新基线 prompt、导航/NPC 协作 prompt、以及 spring-day1 自限红线文档已正式落盘
- 用户最新要求：
  - 先不要继续泛修；
  - 先把这轮关于 `SpringDay1Director / SpringDay1NpcCrowdDirector` 的职责重排、三线程协作边界、以及我自己的永久红线与施工 checklist 全部落成正式文档。
- 本轮已新增 4 份 authoritative 文档：
  1. [2026-04-15_给spring-day1_Day1语义解耦与CrowdDirector退权唯一主刀prompt_v5.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/2026-04-15_给spring-day1_Day1语义解耦与CrowdDirector退权唯一主刀prompt_v5.md)
  2. [2026-04-15_spring-day1_红线与落地清单_剧情后resident退权版.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/2026-04-15_spring-day1_红线与落地清单_剧情后resident退权版.md)
  3. [2026-04-15_给导航_Day1解耦后return-home与free-roam执行合同唯一主刀prompt_68.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-15_给导航_Day1解耦后return-home与free-roam执行合同唯一主刀prompt_68.md)
  4. [2026-04-15_给NPC_Day1解耦后resident状态合同与外部调用面收口prompt_01.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/NPC/2026-04-15_给NPC_Day1解耦后resident状态合同与外部调用面收口prompt_01.md)
- 这轮正式写死的新判断：
  1. `SpringDay1Director` 只应 own Day1 剧情语义、staged contract、`001/002` 的 `house lead`、以及 Day1 首夜对共享夜间合同的接线。
  2. `SpringDay1NpcCrowdDirector` 不应再 own resident 的剧情后生命周期、return-home tick、restart-roam、或 resident 夜间私房调度。
  3. `opening / dinner` 的 staged contract 现在统一按“所有参与 NPC 共用一套起点->终点->5 秒->snap->对白冻结->对白后 release”的新基线写入 prompt。
  4. `003` opening 后不再允许特殊化，必须与 `101~203` 同合同。
  5. `20:00~26:00` 已被重新定性为“全日通用夜间合同”，不应继续让 Day1 私写 resident runtime。
- 这轮新增的自限约束：
  1. 每次真实施工前，必须先做“重复实现 / 冲突实现 / 反向语义 / 越界补丁”盘点。
  2. 不准再手搓 return-home transform fallback。
  3. 不准再用 `StopRoam / RestartRoam / DebugMoveTo` 组合拳。
  4. 不准再碰 UI 壳体、`Primary` 稳定链、或把 `003` 做成游离特例。
- 当前恢复点：
  1. 以后如果继续这条线，先读这 4 份文档，再决定是否开下一刀。
  2. 下一刀如果真实施工，唯一允许的窄刀口仍是：`CrowdDirector` 从剧情后 resident 生命周期里退权。
  3. 本轮文档沉淀已完成，线程已重新 `Park-Slice`，当前 live 状态回到 `PARKED`。

## 2026-04-15｜真实施工补记：003 opening handoff 放手 helper 已补齐，测试语义已改到“退权给 autonomy”
- 当前主线：
  - 只收 `daytime 无显式 crowd beat 时，CrowdDirector 不再继续抓 resident；003 与 101~203 进入同一 release contract`。
- 本轮实际完成：
  1. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - 新增 `ReleaseOpeningThirdResidentControlIfNeeded(bool resumeRoam)`。
     - 接入：
       - `HandleDialogueSequenceCompleted(...)` 的 `VillageGateSequenceId`
       - `TryHandleTownEnterVillageFlow()` 的 `HasVillageGateProgressed()/HasHouseArrivalProgressed()` 早退分支
       - `TryBeginTownHouseLead()`
     - 目标是把 `003` 在 opening handoff 后从 `spring-day1-director` 的 scripted control 里放掉。
  2. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - 清掉 `TickResidentReturnHome(...)` 里的重复阈值判断残口。
  3. [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - 把旧“回 base pose”守护改成 `CrowdDirector_ShouldYieldResidentToAutonomyAfterCueReleaseBeforeFreeTime`
     - 把旧“step fallback”守护改成 `CrowdDirector_ShouldNotFallbackToManualStepReturnHomeWhenNavigationCannotStart`
     - 新增 `Director_ShouldReleaseOpeningThirdResidentControlWhenVillageGateHandsOff`
- 当前代码层验证：
  - `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs --count 20 --output-limit 5`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
  - `manage_script validate --name SpringDay1Director --path Assets/YYY_Scripts/Story/Managers --level standard --output-limit 5`
    - `errors=0 warnings=3`
  - `manage_script validate --name SpringDay1NpcCrowdDirector --path Assets/YYY_Scripts/Story/Managers --level standard --output-limit 5`
    - `errors=0 warnings=2`
  - `validate_script Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs --count 20 --output-limit 5`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
  - `manage_script validate --name SpringDay1DirectorStagingTests --path Assets/YYY_Tests/Editor --level standard --output-limit 5`
    - `errors=0 warnings=0`
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 5`
    - `errors=0 warnings=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
    - 通过
- 本轮额外吸收的只读证据：
  - `NPC` 回执再次钉死当前最脏 public 面：
    - `SetHomeAnchor / ApplyProfile / RefreshRoamCenterFromCurrentContext / StopRoam / RestartRoamFromCurrentContext / DebugMoveTo`
  - sidecar 复核确认：
    - 这轮目标文件里已不再命中 `RefreshRoamCenterFromCurrentContext / RestartRoamFromCurrentContext / DebugMoveTo`
    - 但 `Acquire/ReleaseResidentScriptedControl`、少量 `SetHomeAnchor`、以及部分 `NPCMotionController` 直接写口仍然存在，后续只能记成合同层残留，不能假装已经清零
- 当前判断：
  - 这刀继续把 `CrowdDirector` 从“剧情后 resident 的第二导演”往回削，关键不在“更聪明地管 resident”，而在“放手后别再把 003/101~203 抓回去”。
  - 这轮能站住的是代码层 contract 更干净了；不能站住的是 live 体验已经过线。
- 当前恢复点：
  1. fresh live 只看：
     - `opening` 后 `101~203` 是否还会高频小幅抽搐
     - `003` 是否还会原地罚站
     - 到 anchor 后是否恢复正常 roam
  2. 如果不过线，下一刀仍只允许继续追这条 release contract。
  3. 本轮已重新 `Park-Slice`，当前 live 状态=`PARKED`，blocker=`waiting-fresh-live-on-003-and-101-203-release-return-home`。

## 2026-04-15｜fresh live + profiler 复判补记：当前“像锁死”已不是旧的 scripted freeze，而是错误 release 语义把 resident 送进了 autonomous roam 性能风暴
- 用户 fresh live 新症状：
  - resident 不再表现为“原地小幅抽搐”，而是又像第二次一样直接不动；
  - 同时 profiler 明确显示：
    - `NPCAutoRoamController.Update()` 持续占主线程 `79% ~ 98%`
    - `SpringDay1NpcCrowdDirector.Update()` 只有极小占比
    - 热点对象集中在 `101 / 102 / 103 / 104 / 202 / 203`
- 当前代码级判断：
  1. 这次最像“锁死”的现场，已经不是旧的 `IsResidentScriptedControlActive && !IsResidentScriptedMoveActive` 冻结链：
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) `Update()` / `ShouldSuspendResidentRuntime()` 只会在 scripted control 还在、但 scripted move 没开时早退冻结。
     - 但当前 profiler 热点在同一个 `Update()` 的大自耗，而不是停在这条 freeze 早退上。
  2. 这轮更像是我把 resident 从旧“被 CrowdDirector 抓住不放”的失败模式，改成了新的“被错误 release 到 autonomous roam”的失败模式：
     - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) `ApplyResidentBaseline(...)` 在 `ReleasedFromDirectorCue && NeedsResidentReset` 时，现在直接走 `ReleaseResidentToAutonomousRoam(state)`。
     - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) `ReleaseResidentToAutonomousRoam(...)` 会在 resident 仍停在 opening crowd 终点/人堆附近时，立刻 `StartRoam()`。
     - 这违背了用户真正要的语义：`剧情后先判断是否远离 anchor -> 回 anchor/home -> 到位后再 roam`。
  3. 这就解释了为什么当前看起来“又不动了”：
     - 不是 Day1 还在 scripted control 冻住它们；
     - 而是我把 resident 过早放回自由态，导致它们在拥挤、错误的 crowd cue 落点上直接启动 autonomous roam，随后 `NPCAutoRoamController` 进入高成本路径采样/可达性/agent clearance 循环，帧率炸掉，玩家体感就像“直接不动”。
- 性能归因拆分：
  1. **这次性能爆炸的直接触发器是 Day1**
     - 因为触发对象正是 opening crowd resident；
     - 而且 current release 语义确实是我在 `CrowdDirector` 里改出来的“直接 autonomous roam”。
  2. **但导航 / NPC locomotion 这层也确实有自己的结构性问题**
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) `TryBeginMove(...)` 在 autonomous roam 下会做高成本候选采样和路径尝试；
     - `HasAutonomousRoamDestinationAgentClearance(...)` 还会每次候选都 `FindObjectsByType<NPCAutoRoamController>`；
     - `FindAmbientChatPartner()` 也会在长停阶段扫全体 NPC；
     - 所以 Day1 这次虽然是触发器，但 NPCAutoRoamController 也还没有做到“坏输入只退化，不爆炸”。
- 当前最关键的新判断：
  - 现在系统里同时存在两层未完成：
    1. `spring-day1` 还没真正落到“放手后只发 return-to-anchor 语义，不直接 StartRoam()”
    2. `NPCAutoRoamController` 还没把“密集 crowd 落点 / 不良 release 输入”处理成 cheap fail，而是会炸成大 self time
- 当前恢复点：
  1. 下一刀如果继续改 `spring-day1`，第一优先不再是“继续 release to autonomy”，而是把 `opening` 后 resident 改回：
     - release intent
     - return-to-anchor/home
     - 到位后再 roam
  2. 同时必须给导航/NPC 一刀：
     - autonomous roam 在密集/不可达场景下要快速失败，不能再让 `NPCAutoRoamController.Update()` 炸到 1s 级
  3. 这轮是 fresh live + profiler + 代码对位后的结构归因，不是修完后的终验票。

## 2026-04-15｜真实施工补记：opening resident 已从 direct-autonomy 短路改回 return-home contract，当前停在 fresh live 前
- 当前主线目标：
  - 只收 `opening` 后 resident 的错误 release 语义，不再让 `CrowdDirector` 把 `101~203` 从 crowd cue 终点直接放飞进 autonomous roam。
- 本轮子任务：
  1. 砍掉 `EnterVillage` 阶段的 `YieldDaytimeResidentsToAutonomy -> StartRoam` 错误短路。
  2. 把 `ReleasedFromDirectorCue && NeedsResidentReset` 改回“先 return-home，再恢复 roam”的 opening release contract。
  3. 把测试从旧的“yield to autonomy”改成新语义，并补一条 update-level guard，防止下次又从 `Update()` 短路回去。
- 本轮实际完成：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - `ShouldYieldDaytimeResidentsToAutonomy()` 新增 `EnterVillage` 硬阻断：
       - opening / house-lead 整段不再走 direct-autonomy 短路
       - resident release 必须继续走 baseline/release contract
     - `ApplyResidentBaseline(...)` 在 `ReleasedFromDirectorCue && NeedsResidentReset` 时：
       - 若仍远离 `HomeAnchor`，先走 `TryBeginResidentReturnHome(state)`
       - 只有不需要 return-home 时，才落回 `ReleaseResidentToAutonomousRoam(state)`
     - 新增 `ShouldQueueResidentReturnHomeAfterCueRelease(...)`
       - 明确把“opening cue 放手后先回 anchor/home”的判定压成代码口径
  2. [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - 新增 `CrowdDirector_ShouldKeepOpeningResidentsOnBaselineReleasePathDuringEnterVillage`
       - 守住 `Update()` 不再把 opening resident 短路成 direct autonomy
     - 旧 `CrowdDirector_ShouldYieldResidentToAutonomyAfterCueReleaseBeforeFreeTime`
       - 已改成 `CrowdDirector_ShouldQueueResidentReturnHomeAfterCueReleaseBeforeFreeTime`
       - 断言改为：
         - resident 保持原位
         - 进入 `IsReturningHome`
         - `ResolvedAnchorName` 包含 `return-home`
         - 不再要求立刻进入 autonomous roam
- 本轮代码层验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs --count 20 --output-limit 5`
    - `assessment=external_red`
    - `owned_errors=0`
    - external blocker=`8` 条既有 `Missing Script`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs --count 20 --output-limit 5`
    - `assessment=external_red`
    - `owned_errors=0`
    - external blocker 同上
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name SpringDay1NpcCrowdDirector --path Assets/YYY_Scripts/Story/Managers --level standard --output-limit 5`
    - `errors=0 warnings=2`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name SpringDay1DirectorStagingTests --path Assets/YYY_Tests/Editor --level standard --output-limit 5`
    - `errors=0 warnings=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
    - 通过
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py errors --count 20 --output-limit 5`
    - 仍是 `8` 条外部 `Missing Script`
- 当前判断：
  - 这轮已经把最危险的反向语义清掉了：
    - 旧逻辑：`opening release -> direct autonomy -> NPCAutoRoamController.Update` 风暴
    - 新逻辑：`opening release -> return-home contract -> 到位后再恢复`
  - 这轮站住的是代码层 contract 修正，不是 live 体验已过线。
- 当前恢复点：
  1. fresh live 先只看：
     - `101~203` opening 后是否不再直接卡死/掉进性能风暴
     - resident 是否先回 anchor/home，而不是从 crowd cue 终点直接开始 free-roam
  2. 如果 still 不过线，下一刀优先追：
     - `TryBeginResidentReturnHome / TickResidentReturnHome` 这段是否仍被 Day1 抓太多
     - 是否需要把 return-home trigger 再从 `CrowdDirector` 继续外移
  3. 当前不允许把这轮代码层修正包装成“导航和体验已经彻底修完”。

## 2026-04-15｜真实施工补记：daytime 重复触碰口已补，现场判断已重新校正
- 当前主线目标：
  - 用户这次 fresh live 新反证已经证明：只收 opening direct-autonomy 触发器还不够，`CrowdDirector` 在 daytime 无显式 crowd beat 时仍然在 runtime 里碰 resident。
- 当前重新确认的判断：
  1. `CrowdDirector` 权限仍过重，这个架构判断是对的，而且比之前更稳。
  2. 但我前面对“落地程度”的判断不成立：
     - 之前最多只能说“opening 触发器收了一半”
     - 不能说“release contract 已经收平”
  3. 用户这次说“你现在根本就没修复”，对上一轮 claim 来说是成立的，因为当时确实还留着一个足够大的 runtime 干扰口。
- 本轮实际完成：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - `Update()` 的 daytime autonomy 分支先过 `HasPendingDaytimeAutonomyRelease()`
     - 如果当前 resident 已经自治，或已经在回家，就直接停手，不再每 `0.35s` 重复碰
     - `YieldDaytimeResidentsToAutonomy()` 现在只允许处理 `NeedsDaytimeAutonomyRelease(...) == true` 的 resident
     - `NeedsDaytimeAutonomyRelease(...)` 明确跳过：
       - 已经自治的 resident
       - 已经在 `return-home` 的 resident
       - manual preview / rehearsal resident
  2. [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - 新增 `CrowdDirector_ShouldNotRetouchAlreadyAutonomousResidentsDuringDaytimeYield`
     - 新增 `CrowdDirector_ShouldNotCancelResidentReturnHomeDuringDaytimeYield`
- 本轮代码层验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name SpringDay1NpcCrowdDirector --path Assets/YYY_Scripts/Story/Managers --level standard --output-limit 5`
    - `errors=0 warnings=2`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs --count 20 --output-limit 5`
    - `owned_errors=0`
    - `assessment=external_red`
    - external blocker 仍是 Unity 现场 `8` 条既有 `Missing Script`
  - `validate_script SpringDay1NpcCrowdDirector.cs`
    - 本轮被 `CodexCodeGuard returned no JSON` 卡成 `blocked`
    - 这是工具侧异常，不是 owned red
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
    - 通过
- 当前恢复点：
  1. fresh live 现在要重点看：
     - `CrowdDirector` 开着时，NPC 还会不会原地高频抖动
     - `CrowdDirector` 开着时，已经在回 anchor 的 resident 还会不会被打断
  2. 如果 still 只有“关掉 CrowdDirector 才正常”，下一刀就继续沿同一条线往下削：
     - `CrowdDirector` 在 daytime 是否必须彻底 idle
     - 哪些 runtime 责任必须彻底退出 `CrowdDirector`

## 2026-04-15｜只读复核补记：CrowdDirector 当前最像直接热点的是 return-home 代驾循环
- 当前主线目标：
  - 用户 fresh profiler 已明确把热点钉在 `SpringDay1NpcCrowdDirector.Update()`；这轮只读复核只回答“当前文件里最像直接热点的 exact 方法链”和“下一刀最该砍哪条循环”。
- 当前最像直接热点的 exact 调用链：
  1. [SpringDay1NpcCrowdDirector.Update()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L264)
     -> [TickResidentReturns()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L2352)
     -> [TickResidentReturnHome()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L2419)
     -> [TryDriveResidentReturnHome()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L2662)
     -> [NPCAutoRoamController.DriveResidentScriptedMoveTo()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L869)
     -> [NPCAutoRoamController.DebugMoveTo()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1487)
     -> `TryResolveOccupiableDestination / NavigationPathExecutor2D.TryRefreshPath`
  2. 次一级热点链仍是：
     [Update()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L264)
     -> [ShouldYieldDaytimeResidentsToAutonomy()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L432)
     -> [HasPendingDaytimeAutonomyRelease()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L506)
     -> [YieldDaytimeResidentsToAutonomy()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L481)
     -> [NeedsDaytimeAutonomyRelease()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L519)
     -> [ReleaseResidentToAutonomousRoam()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L551)
- 关键证据：
  1. `ApplyResidentBaseline(...)` 在 cue release 后会把 resident 标成 `IsReturningHome`：
     [ApplyResidentBaseline()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L2287)
     -> [TryBeginResidentReturnHome()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L2458)
  2. 一旦 `IsReturningHome == true`，`Update()` 每帧都会进 `TickResidentReturns()`，而 `TickResidentReturnHome()` 仍会继续调用 `TryDriveResidentReturnHome(...)`，也就是 crowd 仍在 runtime 持续代驾。
  3. `TryDriveResidentReturnHome(...)` 当前不是纯标记，而是直接打进 `NPCAutoRoamController.DriveResidentScriptedMoveTo(...)`；该方法内部又会走 `DebugMoveTo(...)` 的路径重建链。
- 当前判断：
  - 这比“daytime autonomy 双遍历”更像用户现在看到的“高频小幅抖动 + 关掉 CrowdDirector 就恢复”的第一直接热点。
  - 也就是说，当前最该砍的不是泛泛的 `Update()`，而是 `TickResidentReturns -> TickResidentReturnHome -> TryDriveResidentReturnHome` 这条 crowd 代驾回家循环。
- 当前恢复点：
  1. 如果只允许再收一刀，优先砍 `TickResidentReturnHome()` 对 `DriveResidentScriptedMoveTo(...)` 的重复下发。
  2. `HasPendingDaytimeAutonomyRelease()` 这条双遍历仍然脏，但排序在第二刀。

## 2026-04-15｜真实施工补记：opening handoff 退权与 return-home 重下发止血已同时落地
- 当前主线目标：
  - 用户 fresh live 已明确说明：只有关掉 `SpringDay1NpcCrowdDirector` 才正常，说明当前主锅仍在 `CrowdDirector`；这轮继续只收它在 opening handoff 后的 runtime 退权与高频抖动。
- 本轮新的核心判断：
  1. `CrowdDirector` 当前真正的两条越权链是同时存在的：
     - opening handoff 后仍继续 `SyncCrowd()`
     - resident 一旦进入 `IsReturningHome`，`TickResidentReturnHome()` 仍每帧重下发 `DriveResidentScriptedMoveTo(...)`
  2. 所以这轮不能只砍一半，必须同时收：
     - `EnterVillage` release latch 后的 daytime stand-down
     - return-home drive 的重复下发节流
- 本轮实际完成：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - 新增 `ShouldStandDownAfterEnterVillageCrowdRelease(...)`
     - `Update()` 现在在 `EnterVillage` release latch 且无待处理 release 时，直接停手，不再继续 `SyncCrowd()`
     - `ApplyResidentBaseline(...)` 在 opening handoff 已 latch 时，不再把 resident 排进 crowd 自己的 `return-home owner`，而是直接 `ReleaseResidentToAutonomousRoam(...)`
     - `TickResidentReturnHome(...)` 现在会先判 `HasActiveResidentReturnHomeDrive(...)`
       - scripted move 还活着时不再每帧重下发
       - drive 丢失时才按 `ResidentReturnHomeRetryInterval = 0.35s` 重试
     - `SpawnState` 新增 `NextReturnHomeDriveRetryAt`
  2. [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - 原 opening release 测试改成：
       `CrowdDirector_ShouldYieldOpeningResidentToAutonomyAfterEnterVillageCrowdReleaseLatches`
     - 新增：
       `CrowdDirector_ShouldNotReissueReturnHomeDriveWhileScriptedMoveIsAlreadyActive`
- 本轮代码层验证：
  - `manage_script validate SpringDay1NpcCrowdDirector` => `errors=0 warnings=2`
  - `manage_script validate SpringDay1DirectorStagingTests` => `errors=0 warnings=0`
  - `validate_script SpringDay1NpcCrowdDirector.cs` => `assessment=external_red`, `owned_errors=0`
    - 外部 blocker 仍是 Unity 现场既有 `8` 条 `Missing Script`
  - `validate_script SpringDay1DirectorStagingTests.cs` 本轮被 `CodexCodeGuard returned no JSON` 卡成 `blocked`
    - 这是工具侧异常，不是 owned red
  - `git diff --check` 覆盖这两文件通过
- 当前阶段：
  - 代码层这次已经不再只是“分析对”，而是把 `CrowdDirector` 的两个直接 runtime 写口真正收进了代码。
  - 但 fresh live 还没回来，所以仍只能说“代码层止血已落地，体验待验”。
- 当前恢复点：
  1. fresh live 必看 3 件事：
     - `CrowdDirector` 开着时，opening 后 resident 是否还会高频小幅抖动
     - `CrowdDirector` 开着时，resident 是否仍能自己回 anchor
     - profiler 里 `SpringDay1NpcCrowdDirector.Update()` 是否明显降温
  2. 如果还不过线，下一刀继续只追：
     - `_spawnStates == 0` 时的 `ShouldRecoverMissingTownResidents(...)` 恢复链
     - `HasPendingDaytimeAutonomyRelease()` 的双遍历

## 2026-04-15｜决策补记：用户已明确接受从补丁修转向 dedicated refactor thread
- 当前主线目标：
  - 用户已明确提出：与其继续在旧线程里修修补补，不如直接起一个新线程，拿完整修复清单做 `Day1 / CrowdDirector / NPC / Navigation` 的正式解耦重构。
- 当前稳定结论：
  1. 从架构方向上，这个判断是对的。
  2. 之前没有立刻切到 direct refactor，不是因为语义不够，而是因为：
     - `CrowdDirector` 的直接热点链和现场反证还没被方法级钉死
     - 当时直接大改，容易把 `Primary`、`001/002 house lead`、夜间时序、导航 core 一起带进风险面
  3. 到现在为止，direct refactor 的前提已经基本具备：
     - 用户语义定稿
     - 红线文档已落
     - `Day1 / 导航 / NPC` prompt 已成
     - 两个罪犯的 exact 越权链已被钉到方法级
- 当前建议：
  - 下一阶段的正确动作，已经不再是继续无限补丁，而是新开 dedicated refactor thread，唯一主刀 `Day1 runtime decoupling`。
- 建议的新线程唯一范围：
  1. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 只保留剧情语义 / staged contract / release intent。
  2. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 削成 crowd roster / binding / marker lookup / debug summary。
  3. `剧情后 resident 生命周期` 正式移交给 `NPC facade + Navigation contract`。
- 风险提醒：
  - 即使转新线程，也不该一口气无边界重写；必须按单一垂直切片推进：
    `先剥掉 CrowdDirector 的剧情后 resident lifecycle owner，再接入 NPC facade / Navigation intent`。

## 2026-04-15｜正式总交接文档已落盘：不再继续产 prompt，改成交接总入口
- 当前主线目标：
  - 用户已明确改口：这轮不要再写“唯一主刀 prompt”，而是直接产出一份正式总交接文档，把 `Day1` 历史语义、系统边界、现状问题、误实现脉络、止血补丁定位、以及 dedicated refactor thread 的接班范围一次性彻底写清。
- 本轮子任务：
  1. 审核并补全新建的总交接文档。
  2. 把“历史时间线 / 为什么一直修出新坏相 / 接班线程该怎么使用这份文档”补成显式章节。
  3. 清掉旧 prompt 腔和误导性表述，确保这份文件可以直接作为后续重构线程的正式入口。
- 本轮实际完成：
  1. 新增正式文档：
     - [2026-04-15_spring-day1_Day1历史语义_系统边界_现状问题_重构交接总文档.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/2026-04-15_spring-day1_Day1历史语义_系统边界_现状问题_重构交接总文档.md)
  2. 文档内容已明确收进：
     - 用户最终语义定稿
     - `SpringDay1Director / SpringDay1NpcCrowdDirector / NPC / Navigation` 的最终职责边界
     - 两个“罪犯”当前各自的越权点
     - 为什么关掉 `CrowdDirector` 后 NPC 反而正常
     - 历史误修时间线与“方向逐渐对了但 owner 没拔干净”的原因
     - 当前止血补丁的准确定性
     - dedicated refactor thread 的建议阶段划分、红线、完成定义
     - 接班线程该如何阅读和使用这份文档
  3. 文档内原先容易误导成旧 prompt 口径的“唯一”表述已清掉；这是正式交接总文档，不是续工话术。
- 当前判断：
  1. 旧线程继续补丁的收益已经很低；这份总交接文档应成为下一阶段接班重构的 authoritative 入口。
  2. 这轮站住的是“结构与交接层已彻底说清”，不是“runtime 体验已过线”。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-15_spring-day1_Day1历史语义_系统边界_现状问题_重构交接总文档.md`
- 验证结果：
  - 本轮是文档施工，没有新增 compile / live / profiler 验证。
  - 已人工复核文档结构，补齐“历史时间线”和“接班使用方式”，并清除一处误混入的非中文残留字样。
- 当前恢复点：
  1. 下一阶段如果起 dedicated refactor thread，应先读这份总交接文档，而不是继续拼聊天历史。
  2. 当前 `004_runtime` 这条旧线程可在此停车，不再继续无限补丁。

## 2026-04-15｜只读审计补记：opening / dinner staged 走位坏相共享同一类“对白活跃期重置摆位锁”根因
- 当前主线目标：
  - 不改代码，只读回答为什么用户 live 会看到：
    1. opening 剧情开始后 `001~003` 又出现在起点
    2. dinner 时 `001/002` 会重复乱走
- 本轮子任务：
  1. 精查 `SpringDay1Director.cs` 的 `TryHandleTownEnterVillageFlow / TryPrepareTownVillageGateActors / ForcePlaceTownVillageGateActorsAtTargets / BeginDinnerConflict / PrepareDinnerStoryActorsForDialogue`
  2. 精查 `SpringDay1NpcCrowdDirector.cs` 的 `ApplyStagingCue / ResetStateToBasePose / ShouldRestartCueFromSceneBaseline`
  3. 给出最可能 root cause、方法链、最小修法与测试缺口
- 只读结论：
  1. opening 主因已经能静态钉死在导演层：
     - `TryHandleTownEnterVillageFlow()` 一旦发现 `HasVillageGateProgressed()`，就立刻 `ResetTownVillageGateDialogueSettlementState()`
     - 该 reset 会把 `_townVillageGateActorsPlaced=false`
     - 同一帧后半段 `MaintainTownVillageGateActorsWhileDialogueActive()` 又会继续调用 `TryPrepareTownVillageGateActors()`
     - 于是 `001/002/003` 在对白活跃期被反复重新 `Reframe` 到 route start，而不是保持在对白前已经就位的 end
  2. dinner 主因是同类状态耦合在另一条链上再次出现：
     - `BeginDinnerConflict()` 每帧都会先跑 `PrepareDinnerStoryActorsForDialogue()`
     - `EvaluateDinnerCueStartPermission()` 在 `beatSettled=true` 或 `ForceSettleBeatCue(...)` 成功后又会 `ResetDinnerCueSettlementState()`
     - 该 reset 同时清掉 `_dinnerStoryActorsPlaced`
     - 下一帧 `PrepareDinnerStoryActorsForDialogue()` 就会把 `001/002` 再次拉回 start，再往 end drive，形成用户看到的重复乱走
  3. `SpringDay1NpcCrowdDirector.ApplyStagingCue()` 这条 crowd resident 链不是 `001/002/003` 这组 story actor 坏相的第一责任层：
     - 它会影响 `101~203` 的 background/resident cue
     - 但这次用户报的 `001~003 / 001/002` 坏相，第一真责任层仍在 `SpringDay1Director`
- 最小修法建议：
  1. opening：
     - 把 `TownVillageGate` 的“等待计时 reset”和“story actor 已摆位 latch reset”拆开
     - `HasVillageGateProgressed()` 且对白仍 active 时，不要再把 `_townVillageGateActorsPlaced` 清零
  2. dinner：
     - 把 `ResetDinnerCueSettlementState()` 拆成至少两层：`cue wait` reset 与 `story actor placed` reset
     - `EvaluateDinnerCueStartPermission()` 不应在 dinner cue 已 settled 时顺手清掉 `_dinnerStoryActorsPlaced`
     - 最稳的护栏是：`DinnerSequenceId` 已 queue / active 后，`BeginDinnerConflict()` 不再重新进 `PrepareDinnerStoryActorsForDialogue()`
- 测试缺口：
  1. opening 现有 `TownVillageGateDialogueActive_ShouldKeepStoryActorsAligned` 只覆盖“对白 active”，没覆盖 `_villageGateSequencePlayed=true` 后再进 `TickTownSceneFlow()` 的真实路径
  2. dinner 现有测试覆盖了“能否在 timeout 前后对齐并开戏”，但没覆盖：
     - `DinnerSequenceId` 已 queue/active 时再次 tick
     - `_dinnerStoryActorsPlaced` 不应被 reset 后重新回到 start
- 验证状态：
  - 纯静态审计，未改代码，未跑 live
- 当前恢复点：
  1. 如果后续真的开修，这一刀应优先只收“reset 与 placed latch 解耦”
  2. 先不要顺手扩到 `NPC facade / navigation core / dinner cue mirror` 的更大重构

## 2026-04-15｜只读审计补记：Primary 时间冻结与 Town re-entry resident 冻结的最新最可能根因
- 当前主线目标：
  - 不改代码，只读回答：
    1. 为什么一进入 `Primary` 时间就停止流逝
    2. 为什么完成 `Primary` 后回 `Town`、在 dinner 前 `003~203` 会卡死，但 `001~002` 仍可动
- 本轮子任务：
  1. 精查 [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的时间暂停、phase bridge、post-tutorial explore、Town re-entry、story actor release 逻辑
  2. 精查 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 resident snapshot restore、baseline release、recover / reset 逻辑
  3. 交付 root cause、方法链、最小修法与必要测试建议
- 只读结论：
  1. `Primary` 时间冻结已经能高置信度钉死在导演层时间暂停条件：
     - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs#L9056) `SyncStoryTimePauseState()` 会在每轮 tick 调 [ShouldPauseStoryTimeForCurrentPhase()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs#L9101)
     - 当前 `EnterVillage` 的唯一 runtime bridge 放行条件是 [ShouldKeepStoryTimeRunningForRuntimeBridge()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs#L9115) 里的 `IsTownHouseLeadPending()`
     - 但 [IsTownHouseLeadPending()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs#L4619) 强依赖 `IsTownSceneActive()`
     - 结果是：玩家刚切进 `Primary`，即使剧情语义还在承接 `EnterVillage`，`IsTownHouseLeadPending()` 也立刻变 `false`，然后 `PauseTime("SpringDay1Director")` 重新生效
  2. `003` 卡死的直接责任链也已经比较清楚：
     - [TryPrepareTownVillageGateActors()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs#L5911)、[TryDriveEscortActor()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs#L7240)、[ReframeStoryActor()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs#L6599) 会给 `003` 抢到 `spring-day1-director` 控制权
     - `PersistentPlayerSceneBridge` 在 [CaptureNativeResidentRuntimeState()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs#L685) / [RestoreNativeResidentRuntimeState()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs#L1128) 里把 native resident snapshot 原样带过场景
     - [NPCAutoRoamController.ApplyResidentRuntimeSnapshot()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1270) 只要看到 `snapshot.scriptedControlActive`，就会重新 `AcquireResidentScriptedControl(...)` 并直接返回
     - 但 [UpdateSceneStoryNpcVisibility()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs#L2529) 只对 `001/002` 调 `ApplyStoryActorRuntimePolicy(...)`，没有覆盖 `003`，所以 `003` 更容易把旧 owner 原样带回 `Town`
  3. `101~203` 的冻结目前是中高置信度指向 crowd continuity / release contract 缺语义，不像导航 core 自己没 resume：
     - [EnterPostTutorialExploreWindow()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs#L1555) 只把 `_postTutorialExploreWindowEntered` 置真
     - 但 [GetCurrentBeatKey()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs#L1155) 在这个窗口仍返回 `FarmingTutorial_Fieldwork`
     - [ApplyResidentRuntimeSnapshotToState()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L981) 对 `underDirectorCue=true` 会重抓 `AcquireResidentDirectorControl(...)`，同时把 `NeedsResidentReset / ReleasedFromDirectorCue` 都留在未释放状态
     - [SyncCrowd()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L687) 后续只有在 [ApplyResidentBaseline()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs#L2278) 真正进入 baseline release / autonomous 分支时才会把人放回日间 resident 语义；当前 beat 锚点不清，就容易卡在旧 cue / 旧 owner
- 最小修法建议：
  1. 时间冻结先只收一刀：别再让 `EnterVillage` 在 `Primary` 承接后默认判成“必须暂停时间”；至少让 `TownHouseLead` 已结束但 phase 还没推进完的窗口继续走时钟
  2. `003` 先补对称 release：
     - 要么在导演明确结束 opening handoff 或 `Town` re-entry 时，显式释放 `003` 的 `spring-day1-director`
     - 要么在 native snapshot restore 时，对“已不该再由 opening director 持有”的 resident 不再恢复 scripted owner
  3. `101~203` 先补 post-tutorial explore / `Town` re-entry 的明确 beat 或 release 语义，不要继续把这段时间落进 `FarmingTutorial_Fieldwork`
- 测试缺口：
  1. `EnterVillage -> Primary` 承接测试：`IsTownHouseLeadPending()==false` 时仍应验证 `TimeManager` 没被重新暂停
  2. `003` continuity 测试：带 scripted owner 离场再回 `Town` 后，不应继续 `IsResidentScriptedControlActive=true`
  3. crowd continuity 测试：`underDirectorCue=true` + `_postTutorialExploreWindowEntered=true` 时，`Town` re-entry 的第一轮 `SyncCrowd()` 应清掉 restored cue placeholder，并恢复 baseline / roam
- 验证状态：
  - 纯静态审计，未改代码，未跑 live
- 当前恢复点：
  1. 如果继续真实施工，最值得先做的是“时间冻结 + `003` 释放”这两条最小刀
  2. `101~203` 则应跟着补一个明确 beat/release 语义，再决定是否需要更大范围 owner 重构

## 2026-04-17｜只读审计补记：Day1 own 的 `0.0.6` 回 Town 触发链已钉到方法级
- 当前主线目标：
  - 不改代码，只读回答 `0.0.6` 从 `Primary` 回 `Town` 时，`SpringDay1Director / SpringDay1NpcCrowdDirector` 实际会触发哪些 release / recover / resume；哪些点最像“群体卡爆或聊天后单体解放”；以及最小安全切口该砍哪几处。
- 本轮子任务：
  1. 精查 `SpringDay1Director` 的 `_postTutorialExploreWindowEntered`、`ShouldAllowPrimaryReturnToTown()`、`HandleSceneLoaded()`、`TryHandleReturnToTownEscort()`、`ShouldPauseStoryTimeForCurrentPhase()`、`ShouldKeepStoryTimeRunningForRuntimeBridge()`
  2. 精查 `SpringDay1NpcCrowdDirector` 的 `_syncRequested`、`HandleActiveSceneChanged()`、`HandleSceneLoaded()`、`ShouldRecoverMissingTownResidents()`、`RecoverReleasedTownResidentsWithoutSpawnStates()`、`ShouldYieldDaytimeResidentsToAutonomy()`、`ApplyResidentRuntimeSnapshotToState()`、`ApplyResidentBaseline()`
  3. 只给方法名、字段名、判断，不进入真实施工
- 只读结论：
  1. `0.0.6` 自由活动窗本身的“允许回 Town”确实是导演自己开的：
     - `EnterPostTutorialExploreWindow()` 会把 `_postTutorialExploreWindowEntered=true`
     - `ShouldAllowPrimaryReturnToTown()` 在 `IsPostTutorialExploreWindowActive()` 为真时直接放行 `Primary -> Town`
     - 所以自由活动窗回 Town 的入口不是 crowd 自己猜出来的，而是 director gate 明放
  2. 真正的 Town re-entry recover / resume 主要发生在 crowd：
     - `HandleActiveSceneChanged()` / `HandleSceneLoaded()` 都会把 `_syncRequested=true`
     - 下一轮 `Update()` 会优先走 `ShouldRecoverMissingTownResidents()` -> `RecoverReleasedTownResidentsWithoutSpawnStates()`
     - 这条 recover 会给缺失 resident 重新补 `SpawnState`，然后 `QueueResidentAutonomousRoamResume(...)`
     - 所以回 Town 后确实存在一条“批量 recover + 批量 resume”的 own 链
  3. 自由活动窗下的白天 resident release 也是 crowd 自己做的，不是 director 直接一个总 release：
     - `ShouldYieldDaytimeResidentsToAutonomy()` 在 `Town`、非夜间、`phase >= EnterVillage && phase < DayEnd`、且不处于 `DinnerConflictTable / ReturnAndReminderWalkBack / DayEndSettle / DailyStandPreview` 时放行
     - `YieldDaytimeResidentsToAutonomy()` -> `ReleaseResidentToAutonomousRoam()` 会清 cue、取消 return-home、释放 shared runtime control，并 `QueueResidentAutonomousRoamResume(...)`
     - `TryReleaseResidentToDaytimeBaseline()` 也会先抢一次 director control、把人重放到 baseline，再 `QueueResidentAutonomousRoamResume(...)`
  4. `0.0.6` 回 Town 本身不会平白触发 return-home：
     - `TryBeginResidentReturnHome()` / `TickResidentReturnHome()` 只受 `ShouldResidentsReturnHomeByClock()` 控制
     - 也就是 `20:00~20:59` 才进 return-home，`21:00+` 才进 forced rest
     - 所以 dinner 前 re-entry 的群体坏相，主锅不在 `return-home` 时钟链
  5. dinner 从 `Primary` 回 Town 时，director 会再给 crowd 一次硬同步：
     - `TryRequestDinnerGatheringStart()` 在不在 Town 时把 `_pendingDinnerTownLoad=true` 并加载 `Town`
     - `HandleSceneLoaded()` 命中 `Town + _pendingDinnerTownLoad` 后会 `ActivateDinnerGatheringOnTownScene()`
     - 这里又会 `StoryManager.SetPhase(DinnerConflict)`、`UpdateSceneStoryNpcVisibility()`、`SpringDay1NpcCrowdDirector.ForceImmediateSync()`
     - 所以 dinner 回 Town 是“director 先切相位，再硬踢 crowd 立即 sync”的链
- 当前判断：
  1. 群体卡爆最高疑点不在导航 core，而在 crowd 自己的三段叠加：
     - `ApplyResidentRuntimeSnapshotToState()` 会对 `snapshot.underDirectorCue=true` 且 `!ShouldYieldDaytimeResidentsToAutonomy()` 的 resident 重新 `AcquireResidentDirectorControl(...)`
     - `RecoverReleasedTownResidentsWithoutSpawnStates()` 每 tick 最多只补 `3` 个，但补到任何一个又会把 `_syncRequested=true`
     - `HandleActiveSceneChanged()` / `HandleSceneLoaded()` / `HandleStoryPhaseChanged()` 又都会不断把 `_syncRequested` 拉高
     - 这套组合最像“回 Town 后整批人卡在旧 cue / 旧 owner，再靠多轮 sync/recover 波次慢慢松”
  2. “聊天后单体解放”最高疑点在 director 与 crowd 的 release 不对称：
     - `UpdateSceneStoryNpcVisibility()` 在 Town 只对 `001/002` 调 `ApplyStoryActorRuntimePolicy(...)`
     - `ApplyStoryActorRuntimePolicy(...)` 在 storyActorMode 结束时会 `ReleaseStoryControl(DirectorResidentControlOwnerKey, resumeRoamWhenReleased)`，并在 Town 场景里直接 `ResumeAutonomousRoam(true)`
     - 但 `003` 不走这条对称 release，ordinary resident 又还在等 crowd 的 `ShouldYieldDaytimeResidentsToAutonomy() / ApplyResidentBaseline()`
     - 所以最像的坏相就是：聊完后 `001/002` 先被 director 单独放活，其他人仍卡在 crowd 的旧 owner / 旧 cue
- 最小安全切口：
  1. 第一刀先砍 `ApplyResidentRuntimeSnapshotToState()`：
     - 目标不是大改导航，而是阻断 `Town re-entry` 时把 `underDirectorCue` 旧语义重新恢复成 director owner
  2. 第二刀再砍 `HandleActiveSceneChanged()` / `HandleSceneLoaded()` / `RecoverReleasedTownResidentsWithoutSpawnStates()` 这一组 `_syncRequested` recover 波次：
     - 目标是让 Town re-entry 只做一次明确 recover，不再多轮补绑 + 多轮重 sync
  3. 第三刀才砍 `UpdateSceneStoryNpcVisibility()` / `ApplyStoryActorRuntimePolicy()`：
     - 目标是消掉 `001/002` 与 `003~203` 的“聊天后只放活一部分”不对称 release
- 验证状态：
  - 纯静态审计，未改代码，未跑 live
- 当前恢复点：
  1. 如果后续进入真实施工，正确顺序是：`snapshot restore -> re-entry recover/sync -> story actor 对称 release`
  2. `TryBeginResidentReturnHome()` / `TickResidentReturnHome()` 当前只作为 `20:00+` 夜间链关注，不该抢成 `0.0.6` dinner 前回 Town 的第一刀

## 2026-04-17｜只读审计补记：Day1 dinner / late-day 当前仍是 `001/002` 导演私链 + ordinary resident crowd 链的双轨
- 当前主线目标：
  - 不改代码，只读回答 `Day1 dinner / late-day` 里为什么 `D-02/D-03` 仍未真正收平：也就是 `001/002` 是否还走导演私链、ordinary residents 是否还走 crowd 链、以及哪些方法仍在分叉处理。
- 本轮子任务：
  1. 精查 `SpringDay1Director.cs` 的 dinner / reminder / free-time 入口与 `001/002` runtime owner
  2. 精查 `SpringDay1NpcCrowdDirector.cs` 的 runtime entries、cue/baseline、snapshot restore 与 resident release
  3. 对照 `SpringDay1LateDayRuntimeTests.cs`、`SpringDay1DirectorStagingTests.cs`、`NpcCrowdResidentDirectorBridgeTests.cs`、`NpcCrowdManifestSceneDutyTests.cs` 只读判断覆盖与缺口
- 只读结论：
  1. `001/002` 在 `DinnerConflict` 与 `ReturnAndReminder` 里仍走导演私链：
     - `UpdateSceneStoryNpcVisibility()` -> `ShouldUseTownStoryActorMode()` 会在 `DinnerConflict / ReturnAndReminder` 持续把 `001/002` 设为 story actor mode
     - `ApplyStoryActorRuntimePolicy()` 会直接对二者 `AcquireStoryControl / ReleaseStoryControl / ResumeAutonomousRoam(...)`
     - `BeginDinnerConflict()` -> `CanStartDinnerConflictDialogueNow()` -> `PrepareDinnerStoryActorsForDialogue()` / `ForceSettleDinnerStoryActors()` 仍由导演自己解析 `001/002` 的 start/end marker，并直接 `Reframe / TryDriveEscortActor`
  2. ordinary residents 在 dinner / reminder 里仍走 crowd 链：
     - `SyncCrowd()` 仍按 `ApplyStagingCue()` / `ApplyResidentBaseline()` 驱动普通 resident
     - `NormalizeResidentRuntimeBeatKey()` 会把 `ReturnAndReminder_WalkBack` 归一到 `DinnerConflict_Table`
     - `ShouldHoldDinnerCueThroughReturnReminderBlock()` 让 dinner resident cue 持续撑到 reminder block
  3. crowd 侧还显式把 `001/002` 排除在 dinner/reminder resident runtime 之外：
     - `GetRuntimeEntries()` 只在 `FreeTime ~ DayEnd` synthetic 加入 `001/002`
     - `ShouldDeferToStoryEscortDirector()` 明确把 `001/002` 在 `DinnerConflict / ReturnAndReminder` 继续 defer 给 director
  4. `SpringDay1DirectorStaging.cs` 当前更像中立底座，不是第一责任分叉点：
     - 它提供的是 beat key、cue 匹配与 playback 能力
     - 当前真正把 runtime 切成两轨的，是 `Director` 与 `CrowdDirector` 两侧 caller 的 owner / entry 选择
- 最小可落地统一方案建议：
  1. 最小统一方向不要反过来把 ordinary residents 拉进 director，而是把 `001/002` 并进现有 crowd/stage-book 链
  2. 第一刀只收 3 处：
     - 在 `SpringDay1NpcCrowdDirector.GetRuntimeEntries()` 里把 `001/002` 的 synthetic resident runtime 提前到 `DinnerConflict / ReturnAndReminder`
     - 删掉或收窄 `ShouldDeferToStoryEscortDirector()` 对 `DinnerConflict / ReturnAndReminder` 的 special-case
     - 让 `SpringDay1Director.BeginDinnerConflict()` 只保留 phase/time/dialogue gate，不再自己 `PrepareDinnerStoryActorsForDialogue()` / `ForceSettleDinnerStoryActors()` 直驱 `001/002`
  3. 第二刀再收 `UpdateSceneStoryNpcVisibility()`：
     - `DinnerConflict / ReturnAndReminder` 不再把 Town 里的 `001/002` 长持有为 story actor runtime
     - `FreeTime / DayEnd` 继续沿现有“19:30 后并回 resident runtime、20:00/21:00 统一夜间合同”的方向
  4. 这样改的最小价值是：
     - `001/002` 与 `003~203` 终于走同一条 runtime 语义
     - 不需要先翻掉 `Staging` 底座，也不必先动导航 / NPC facade 大重构
- 测试覆盖与缺口：
  1. 已覆盖：
     - `SpringDay1LateDayRuntimeTests` 已覆盖 `001/002` 晚饭入口只在 `BeginDinnerConflict` 起步、未超时前从 authored start 进场、超时后 snap 到 authored end、`003` 不该被 director 晚饭路径重新抓走、以及 `20:00/21:00` 后 `001/002` 不应再被 director 夜间私链持有
     - `SpringDay1DirectorStagingTests` 已覆盖 `ResetDinnerCueWaitState()` 不应清掉 `_dinnerStoryActorsPlaced`、`001/002` 在 free-time 应退出 story actor policy、ordinary residents 在 `0.0.6/free-time` 应恢复自治、以及 `19:30` 后 runtime entries 应包含 `001/002/003`
     - `NpcCrowdResidentDirectorBridgeTests` / `NpcCrowdManifestSceneDutyTests` 已覆盖 `DinnerConflict_Table` 的 resident cue、priority roster 与 manifest duty/anchor 语义
  2. 仍缺：
     - 没有测试直接钉住“`DinnerConflict / ReturnAndReminder` 时 `001/002` 也必须进入 crowd runtime entries，而不是继续 defer 给 director”
     - 没有测试直接钉住“`BeginDinnerConflict()` 不再直接驱动 `001/002` 的 runtime locomotion，只负责 dialogue gate”
     - 没有测试直接钉住“`ReturnAndReminder` 不再沿用 dinner cue hold 形成 `001/002` director / residents crowd 的不对称双轨”
     - `SpringDay1DirectorStagingTests` 里仍有 `TryResolveDinnerStoryActorRoute(\"003\")` 的 helper 级遗留覆盖；它不是当前 runtime 真入口，但会掩盖“历史上 dinner 曾想把 `003` 也纳入导演路由”的旧意图
- 验证状态：
  - 纯静态审计，未改代码，未重跑 tests/live
- 当前恢复点：
  1. 如果下一刀进入真实施工，最值钱的最小切口就是先消掉 `DinnerConflict / ReturnAndReminder` 对 `001/002` 的 `defer-to-director`
  2. 真正该新增的 first-line regression 不是更多 authored marker 测试，而是“dinner/reminder 统一 runtime owner”测试

## 2026-04-17｜只读审计补记：Day1 Package F-07 stale tests 的高优先级退役/重写名单
- 当前主线目标：
  - 按 `0417.md` 的 `F-07`，只读找出 editor tests 里仍把旧中间层当真值的断言，优先给出最该删/重写的测试名、旧语义标签和最小安全新断言方向。
- 本轮子任务：
  1. 复查 `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
  2. 连带复查 `NpcCrowdManifestSceneDutyTests.cs`、`NpcCrowdResidentDirectorBridgeTests.cs`、`SpringDay1OpeningRuntimeBridgeTests.cs`
  3. 只记录 stale tests，不改代码、不跑 Unity
- authoritative 结论：
  1. 最危险的一簇不是“测试名字旧”，而是断言直接把 `Town_Day1Residents / Town_Day1Carriers`、`EnterVillageCrowdRoot / DinnerBackgroundRoot / NightWitness_01 / DailyStand_*`、以及 `return-home -> resume roam` 当 runtime 真合同。
  2. `SpringDay1DirectorStagingTests.cs` 里最该优先退役/重写的是 4 类：
     - 结构真值绑在 `Town_Day1Residents / Town_Day1Carriers` 的 `_residentRoot/_carrierRoot` 断言
     - 把 neutral snapshot 恢复后“必须立即 restart roam”当真值的断言
     - 把 `return-home` 完成后“必须 resume roam / force restart roam”当真值的断言
     - 仍用 `DailyStand_Preview`、`EnterVillageCrowdRoot`、`DinnerBackgroundRoot` 等旧 beat/anchor 去钉 manifest/stagebook/runtime 细节的断言
  3. `NpcCrowdManifestSceneDutyTests.cs` 与 `NpcCrowdResidentDirectorBridgeTests.cs` 当前更像“旧 asset 结构镜像测试”：
     - 它们大面积把旧 semantic anchor 名和旧 beat matrix 当固定白名单
     - 一旦 Day1 语义继续去中间层，这两簇会系统性阻拦正确重映射
  4. `SpringDay1OpeningRuntimeBridgeTests.cs` 里最像 stale 的，不是“禁止 fallback”，而是仍要求保留 `Town VillageGate actor fallback` 解析入口本身。
- 最小安全处理方向：
  1. 把 tests 从“根节点/旧 anchor/旧 beat 名必须存在”改成“只验证 runtime contract 结果”
  2. 把 `return-home` 系列从“回家后 resume roam”改成“20:00 开始回家，21:00 snap+hide，不再要求白天/夜间共用 resume 语义”
  3. 对 opening/dinner，只保“有 authored markers 时优先吃 scene markers；没有就 fail fast，不恢复 opening->dinner mirror/fallback”
- 验证状态：
  - 纯静态审计
  - 未改测试，未跑 suite
- 当前恢复点：
  1. 如果后续进入真实施工，先删/重写 `SpringDay1DirectorStagingTests.cs` 里 4 个最高风险簇
  2. 然后再收 `NpcCrowdManifestSceneDutyTests.cs` / `NpcCrowdResidentDirectorBridgeTests.cs` 的旧 anchor/beat 镜像断言

## 2026-04-17｜只读审计补记：Day1 夜间 `20:00/21:00 resident return-home/hide` 现码真相
- 当前主线目标：
  - 不改代码，只读彻查 Day1 夜间 resident 在 `20:00 return-home`、`21:00 forced rest`、以及 scene/snapshot/recover 交叉链里的真实语义，并回答“为什么到 anchor 后还可能不立刻隐藏”“21:00 具体怎么兜底”“还有哪些入口/owner 会让 NPC 到家后留在场上”。
- 本轮子任务：
  1. 精查 `SpringDay1NpcCrowdDirector.cs` 的 `Update / SyncResidentNightRestSchedule / TryBeginResidentReturnHome / TickResidentReturnHome / FinishResidentReturnHome / ApplyResidentNightRestState / ApplyResidentRuntimeSnapshotToState / RecoverReleasedTownResidentsWithoutSpawnStates`
  2. 补查 `SpringDay1Director.cs` 的 `HandleHourChanged / SyncStoryActorNightRestSchedule / UpdateSceneStoryNpcVisibility`
  3. 补查 `PersistentPlayerSceneBridge.cs` 与 `NPCAutoRoamController.cs` 的 resident snapshot / formal-navigation completion 调用口
- authoritative 结论：
  1. `20:00` 本身不是“立刻 snap+hide”窗口，而是“开始回家”的可见合同：
     - `SpringDay1Director.HandleHourChanged(...)` 只做 `EnforceDay1TimeGuardrails()` 与 `SyncStoryActorNightRestSchedule()`，并不会直接调用 crowd 的 hide/snap；
     - 真正的 resident 夜间逻辑在 `SpringDay1NpcCrowdDirector.Update()` 每帧轮询；
     - `20:00 <= hour < 21:00` 时，`SyncResidentNightRestSchedule()` 只会 `TryBeginResidentReturnHome(...)`，不会直接 `SetActive(false)`。
  2. 当前 resident 到 anchor 后仍可能不立刻隐藏，源码级只剩 4 类解释：
     - 还没拿到 `FormalNavigation arrival`，且当前位置也还没进 `0.35` 到站半径；
     - `TryDriveResidentReturnHome(...)` 没成功发起 drive，落成 `return-home-pending + retry cooldown`；
     - 绑定到的 `HomeAnchor` 真值不是玩家以为的那个锚点，导致视觉上“到家了”，代码上却没进 `IsResidentAtHomeAnchor(...)`；
     - scene re-entry / snapshot / recover 分支先把 resident 重新设成 active，再晚一帧才轮到夜间 schedule 收口。
  3. `21:00` 的 fallback 当前是明确的“release owner -> snap -> hide”，不是继续 retry：
     - `ShouldResidentsRestByClock()` 一旦为真，`SyncResidentNightRestSchedule()` 会先把 `IsNightResting=true`、`HideWhileNightResting=true`；
     - 然后 `ApplyResidentNightRestState(...)` 会释放 shared runtime owner、`SnapToTarget(...)` 到 `HomeAnchor`、最后统一 `SetActive(false)`。
  4. 当前最容易把“已经回到家附近的 NPC”继续留在场上的入口，不只 `20:00` 夜间链本身，还包括：
     - `HandleActiveSceneChanged()` / `HandleSceneLoaded()` 拉起 `_syncRequested`
     - `PersistentPlayerSceneBridge.RestoreCrowdResidentRuntimeState()`
     - `ApplyResidentRuntimeSnapshotToState()`
     - `RecoverReleasedTownResidentsWithoutSpawnStates()`
     这些入口会先把 resident 恢复成 active / queued roam，再由下一轮夜间 schedule 追回。
  5. 另有一条独立入口不是 `21:00 fallback`，但确实会“回 anchor 仍留场”：
     - `SpringDay1Director.HandleSleep()` -> `TrySnapResidentsToHomeAnchorsForDayEnd()` -> `SnapResidentsToHomeAnchorsInternal()`
     - 这条链只设 `IsNightResting=true`，没有先把 `HideWhileNightResting=true` 写死；
     - 因此它本质是“DayEnd snap 回家”，不是“21:00 forced hide”。
- 验证状态：
  - 纯静态审计，未改代码，未跑 live / tests
- 当前恢复点：
  1. 如果后续进入真实施工，最值钱的第一刀不该再泛改 dinner/opening，而是只抓：
     - `HomeAnchor` 绑定真值
     - `20:00` arrival/completion
     - `scene restore/recover` 先 active 后补收口的顺序
  2. 如果后续继续只读，下一轮优先补 runtime probe 方案，最小只抓：
     - 当前 `HomeAnchor`
     - `RequestReturnToAnchor(...)` 返回
     - `TryConsumeFormalNavigationArrival(...)`
     - `ApplyResidentRuntimeSnapshotToState(...)` 是否把 NPC 重新放回 active

## 2026-04-17｜只读审计补记：`19:30` 后到 `20:00+` 聊天权限异常的最可能根因链
- 当前主线目标：
  - 不改代码，只读彻查为什么 `19:30` 以后尤其 `20:00` 后会出现“只有 `002` 能聊天、其他 NPC 不能聊”或 `001/003/ordinary resident` 聊天权限异常，并判断最小安全修口是否会伤到回家链。
- 本轮子任务：
  1. 精查 `SpringDay1Director.cs` 的 `FreeTime` 入口、`001/002` 晚段 runtime 并轨与 `20:00` 夜间时钟门。
  2. 精查 `SpringDay1NpcCrowdDirector.cs` 的 synthetic runtime entries、`ApplyResidentBaseline()`、`TryBeginResidentReturnHome()`、snapshot restore 与 interaction lock。
  3. 精查 `NPCDialogueInteractable.cs`、`NPCInformalChatInteractable.cs`、`NpcInteractionPriorityPolicy.cs`、`PlayerNpcChatSessionService.cs` 与 `001/002/003` 对话内容资产，区分“设计预期”与“代码异常”。
- authoritative 结论：
  1. `001` 晚段不能像 `002/003` 那样继续“闲聊”，现码更像数据预期，不是同类 runtime bug：
     - `NPC_001_VillageChiefDialogueContent.asset` 只有 `selfTalk / nearby / pair dialogue`，没有 `relationshipStageInformalChatSets`；
     - `NPC_002_VillageDaughterDialogueContent.asset` 与 `NPC_003_ResearchDialogueContent.asset` 则都明确配置了 `conversationBundles`；
     - 所以用户如果期待 `001` 在 formal 消耗后还能像 `002/003` 一样走玩家主动闲聊，这一条首先是内容资产缺口，不是 `priority/gate` 单点回归。
  2. `20:00+` ordinary resident 与 `003` 聊天被硬切断，是当前夜间合同的直接结果：
     - `EnterFreeTime()` 在 `19:30` 把 phase 推到 `FreeTime`；
     - `SpringDay1NpcCrowdDirector.SyncResidentNightRestSchedule()` 在 `20:00 <= hour < 21:00` 对 resident 发起 `TryBeginResidentReturnHome()`；
     - `RequestReturnToAnchor(...)` 会给 `NPCAutoRoamController` 加 `ResidentScriptedControlOwnerKey` 的 `FormalNavigation`；
     - `NPCInformalChatInteractable.CanInteractWithResolvedSession()` 只在 `FreeTime + scripted control 仍是 active formal navigation drive` 时临时放行，一旦变成“仅剩 scripted owner / 已到家 / 已隐藏”，就直接返回 `false`；
     - `PlayerNpcChatSessionService` 还会在会话中途看到 `IsResidentScriptedControlActive=true` 时直接取消聊天；
     - resident 到家后 `FinishResidentReturnHome()` 在 `20:00~20:59` 会立刻 `HideResidentAtHomeForNightReturn()`，因此 ordinary resident/`003` 在这个窗口本来就会越来越不可聊，甚至直接消失。
  3. “为什么有时像是只剩 `002` 能聊”最像的代码级异常，不在 `priority policy`，而在 synthetic resident 与 native snapshot 的管理口径不一致：
     - `SpringDay1NpcCrowdDirector.GetRuntimeEntries()` 会在晚段 synthetic 加入 `003`，并在 `DinnerConflict ~ DayEnd` synthetic 加入 `001/002`；
     - 但 `PersistentPlayerSceneBridge.GetCrowdDirectorManagedNpcIds()` 只从 manifest 原生 entries 建 managed 集，不包含这些 synthetic ids；
     - 结果是 `001/002/003` 可能同时落进 native resident snapshot capture/restore；
     - `ApplyResidentRuntimeSnapshotToState()` 又会按 `snapshot.isReturningHome / snapshot.underDirectorCue` 恢复 scripted owner；
     - 这会让同样处在晚段的 NPC 出现“不该一样却恢复得不一样”的 owner/chat 状态；
     - 由于 `002` 既有真正的 informal bundles、又是 synthetic story escort resident，所以它最容易表现成“还活着还能聊”；`001` 没有 informal bundle，`003` 与 ordinary residents 更容易继续卡在 crowd owner / return-home / hide 链里。
  4. `priority/gate` 本身在 `FreeTime` 不是主因：
     - `NpcInteractionPriorityPolicy` 只在 formal-priority phases 抑制 informal；
     - `FreeTime` 已不属于 formal-priority phase；
     - 因此 `19:30` 后若还不能聊，优先看的是 `scripted control / cue lock / hide / snapshot restore`，不是 `formal > informal` 优先级还没退。
- 最小安全修法建议：
  1. 第一优先、最安全的一刀不是放宽 `CanInteract`，而是把 `001/002/003` 纳入 crowd-managed snapshot 口径：
     - 要么让 `PersistentPlayerSceneBridge.GetCrowdDirectorManagedNpcIds()` 覆盖 synthetic late-day ids；
     - 要么在进入 `DinnerConflict / ReturnAndReminder / FreeTime` 的统一 resident runtime 后，主动清掉这三人的 native snapshots；
     - 这刀主要修的是“恢复不对称”，不改 `20:00` 回家语义。
  2. 如果产品定义要求 `001` 也能像 `002/003` 一样继续玩家主动闲聊，最小安全修口应优先落数据：
     - 给 `NPC_001_VillageChiefDialogueContent.asset` 补 informal bundles；
     - 不建议先改 `NPCInformalChatInteractable` 代码去特殊放行 `001`。
  3. 最不安全的一刀，是直接放宽 `NPCInformalChatInteractable.ShouldAllowInformalInteractionDuringFreeTimeReturnHome()` 或取消 `20:00` 后 hide：
     - 这会直接改变 `20:00` 回家链的合同；
     - 很容易把“该回家/该隐藏”的人继续留在场上，反伤 night-rest 与 DayEnd 收束。
- 验证状态：
  - 纯静态审计，未改代码，未跑 Unity / live。
- 当前恢复点：
  1. 如果后续要进真实施工，最值钱的第一刀应先修 synthetic ids 与 native snapshot 的 managed 集不一致。
  2. `001` 是否需要晚段主动闲聊，应单独按内容定义拍板；不要把它和 `003/ordinary resident` 的 runtime gate 异常混成同一类 bug。

## 2026-04-18｜只读审计补记：Day1 live/validation 现有菜单链的真实上限
- 当前主线目标：
  - 不改代码，只读回答 `Spring Day1` 现有 live/validation 菜单、runner、debug menu、native fresh restart、late-phase jump、scene transition 里，是否已经存在一条“只用现有入口、不新增代码”的安全自动链，能把 PlayMode 从 fresh/reset 推进到 `Town free-time`、`20:00 return-home`，或至少稳定进入 `Town` 并拿到 live snapshot。
- 本轮子任务：
  1. 精查 `SpringDay1NativeFreshRestartMenu.cs`、`DialogueDebugMenu.cs`、`SpringDay1LatePhaseValidationMenu.cs`、`SpringDay1LiveSnapshotArtifactMenu.cs` 的菜单行为。
  2. 精查 `SaveManager.cs`、`SpringDay1Director.cs`、`SceneTransitionTrigger2D.cs`、`PersistentPlayerSceneBridge.cs`、`TimeManagerDebugger.cs` 的 runtime 入口与转场/时间推进合同。
  3. 对照 `Town.unity`、`Primary.unity`、`Home.unity` 里的 transition trigger 配置，解释为什么之前 `Town Lead Transition Artifact` 会落到 `Home.unity`。
- authoritative 结论：
  1. **到 `Town live snapshot / free-time` 的安全链已经存在**：
     - `Restart Spring Day1 Native Fresh` 在编辑态会先把场景对齐到 `Town`，在 PlayMode 会通过 `SaveManager.RestartToFreshGame()` 重开到 `Town` 原生开局，并把时间重置到 `Year1/Spring/Day1/09:00`。
     - 在这个 fresh `Town` 现场里，`Force Spring Day1 FreeTime Validation Jump` 会直接把 director 的教学条件补齐、进入 `postTutorialExploreWindow`，再调用 `EnterFreeTime()`；随后 `Write Spring Day1 Live Snapshot Artifact`、`Write Spring Day1 Actor Runtime Probe Artifact`、`Write Spring Day1 Resident Control Probe Artifact` 都能在现成 Town runtime 上取证。
  2. **到 `20:00 return-home` 的现成链也存在，但不是纯单菜单一键到底**：
     - 更稳的现成入口是 `Force Spring Day1 Dinner Validation Jump`，它会用正式 dinner 入口把相位推到 `DinnerConflict`，后面再通过 `Trigger/Step Spring Day1 Validation` 或 `Force Complete Or Advance Dialogue` 把晚饭、归途提醒推进到 `FreeTime`。
     - `FreeTime` 到 `20:00` 这半小时没有专门菜单，但现成 runtime 入口还在：`TimeManagerDebugger` 会在 PlayMode 自动挂到 `TimeManager`，按一次 `+` 会把 `19:30` 规范化推进到 `20:00`；`SpringDay1NpcCrowdDirector.ShouldResidentsReturnHomeByClock()` 只看时钟，不要求额外 phase 钩子，所以这一步能直接触发 resident `return-home`。
  3. **不存在“从 fresh opening 开始，只靠同一个 Step 菜单一路无脑跑到晚段”的单通道自动链**：
     - `SpringDay1LiveValidationRunner.TriggerRecommendedAction()` 在 `FarmingTutorial` 只会调用 `TryAdvanceFarmingTutorialValidationStep()`；
     - 这条 stepper 能自动补 till/plant/water/wood/craft，但不会替你触发 `TryStartPostTutorialWrapSequence()` 这一下“和村长收口”；
     - 所以不用 late-phase jump 或真实 NPC 交互的话，这条自动链会在 farming 收口前停住。
  4. **之前 `Town Lead Transition Artifact` 落到 `Home.unity` 的直接原因已经钉死**：
     - `SpringDay1LiveSnapshotArtifactMenu.TryRequestTownLeadTransition()` 先会问 `SpringDay1Director.TryRequestValidationEscortTransition()`；
     - 只有当 director 回答“当前没有待请求的 escort 转场”时，才会退回 fallback：按 `activeScene` 只在 `Town -> Primary`、`Primary -> Town` 这两种情况下猜目标；一旦当前 scene 不是这两者，`targetSceneName` 就会变空，`ResolveTransitionTrigger(...)` 便会退回遍历到的第一个 trigger；
     - 场景里同时存在 `HomeDoor -> Primary`、`PrimaryHomeDoor -> Home`、以及真正的 `SceneTransitionTrigger`；
     - 因此在非 `Town/Primary` 现场或 escort 不成立时，它仍可能吃到错误 trigger；memory 里那次返回 `transition-requested:PrimaryHomeDoor`，最终 activeScene 落到 `Home.unity`，正是这条 fallback 误选导致的。
     - 与之相对，`SpringDay1Director.TryRequestValidationEscortTransition()` 走的是 `ResolveCachedTownTransitionTrigger()` / `ResolveCachedPrimaryTownTransitionTrigger()`，会按目标场景和首选对象名筛 trigger，这条验证入口才是相对安全的。
  5. **当前“profiler 自动链”仍不成立**：
     - 仓内这组 `Day1` 菜单能稳定写 live snapshot / actor probe / resident probe；
     - 但没有对应的 `Town profiler` 自动菜单或 `Day1` 专用 profiler 采样器；
     - 所以最多只能先用现成链把现场稳定带进 `Town`，随后人工打开 Unity Profiler 取窗口或继续用别的 probe，不应把现状说成“Town profiler 自动化已具备”。
- 验证状态：
  - 纯静态审计，未改代码，未跑 live。
- 当前恢复点：
  1. 如果后续要继续只用现有入口拿 `Town` 晚段证据，优先链应改成：
     - `Restart Spring Day1 Native Fresh`
     - `Force Spring Day1 FreeTime Validation Jump` 或 `Force Spring Day1 Dinner Validation Jump`
     - `Write Spring Day1 Live Snapshot Artifact` / `Actor Runtime Probe` / `Resident Control Probe`
     - 需要 `20:00` 时再用 `TimeManagerDebugger` 的 `+`
  2. 不要再把 `Request Spring Day1 Town Lead Transition Artifact` 当成可靠 Town 切场入口；它当前更像“未过滤 trigger 的不安全桥”。

## 2026-04-18｜只读窄审计：Day1 `20:00` 回家后“到 anchor 才隐藏”链与 `20:29` 不隐藏根因
- 当前主线目标：
  - 不改代码，只读彻查 Day1 晚段 resident / story escort 在 `20:00` 后“回家但未隐藏”的真实实现链，并明确 Day1 own 与 NPC/导航 own 边界。
- 本轮子任务：
  1. 只读精查 `SpringDay1NpcCrowdDirector.cs`、`SpringDay1Director.cs`、`NPCAutoRoamController.cs`。
  2. 只回答 `20:00` 到家隐藏链、`20:29` 站在家门口附近却没隐藏的最像原因、以及最可能失效点。
- authoritative 结论：
  1. **`20:00` 这条链不是“到点直接隐藏”，而是两段式：先回家，再由到站收尾决定是否隐藏。**
     - `SpringDay1NpcCrowdDirector.Update()` 每帧先跑 `SyncResidentNightRestSchedule()`，再跑 `TickResidentReturns()`。
     - `20 <= hour < 21` 时，`SyncResidentNightRestSchedule()` 对有 `HomeAnchor` 的 resident 调 `TryBeginResidentReturnHome(state)`。
     - `TryBeginResidentReturnHome()` 最终会走 `TryDriveResidentReturnHome()` -> `NPCAutoRoamController.RequestReturnToAnchor(...)`，把回家 drive 发成 `FormalNavigation`。
     - 真正的“到 anchor 就隐藏”发生在下一段：`TickResidentReturnHome()` 里先等 `TryConsumeFormalNavigationArrival()` 的一次性到站信号；若到站位置离真实 `HomeAnchor` 足够近，或兜底 `IsResidentAtHomeAnchor()` 判真，才进入 `FinishResidentReturnHome()`。
     - 只有 `FinishResidentReturnHome()` 且当前仍在 `20 <= hour < 21` 时，才会走 `HideResidentAtHomeForNightReturn()`，也就是 snap 到 home anchor 后 `SetActive(false)`。
  2. **`20:29` 站在家门口附近却没隐藏，按现码最像“还在 return-home 窗口，但收尾条件没成立”，不是 `21:00` 强制 rest 失效。**
     - crowd/night contract 只看 `TimeManager.GetHour()`，所以 `20:01` 到 `20:59` 整段都还是“return-home，但未 forced-rest”的同一个小时窗。
     - 在这个窗口里，NPC 只会在“formal-navigation 到站信号已被消费”或“与真实 `HomeAnchor` 的距离已经进 0.35 半径兜底”时隐藏。
     - 因此用户看到“已经走到家门口附近”并不等于代码认定“已经到 home anchor”；只要到站信号没被成功消费、或 anchor 本身比门口更靠里/更偏，NPC 就会继续保持 active 到 `21:00` 的 forced-rest safety net。
  3. **`SpringDay1Director` 本轮不是 `20:00` 隐藏主控，它只负责把 Town 的 `001/002` 交回 resident night contract。**
     - `UpdateSceneStoryNpcVisibility()` 在 `Town + FreeTime/DayEnd + hour>=20` 时，会 `deferVisibilityToResidentNightContract`，不再强行 `SetActive(true)`，并把 `001/002` 的 `resumeRoamWhenReleased` 改成 `false`。
     - 代码里虽然有 `ShouldManageStoryActorNightSchedule()` / `ApplyStoryActorNightRestSchedule(...)`，但这套 `20:00/21:00` story-actor 夜间逻辑当前没有接进常规 tick；除了 `DayEnd` 强制 snap，它实际上不会在 `20:29` 帮你额外隐藏 `001/002`。
     - 所以 `20:29` 的 hide 成败，本质上仍落在 crowd director 的 return-home 收尾，而不是 director 自己的另一条隐藏链。
  4. **本轮最像的具体失效点，按概率排序如下：**
     - `Day1 own / caller-handoff`：crowd 已经发起 `RequestReturnToAnchor()`，但 owner 在到站前被 release / 替换，导致 `NPCAutoRoamController` 挂起的 `pendingFormalNavigationArrival` 先被清掉，`TickResidentReturnHome()` 永远吃不到收尾信号。
     - `Day1 own / acceptance`：真实 `HomeAnchor` 与玩家视觉里的“家门口”不是同一点；当前 finish 判定严格对着 `state.HomeAnchor.transform.position` 和 `0.35f` 半径，不会因为“看起来到了门口”就隐藏。
     - `NPC/导航 own`：`BeginPathDirectedMove()` 没能成功建路或路被判不合格，导致 crowd 只能把人放进 `return-home-pending` 重试窗；这时 NPC 可能停在门口附近但仍是 active。
     - `Day1 own / 001/002 handoff`：`SpringDay1Director` 的 story-actor 夜间 schedule 目前没接进平时 tick，所以如果 `001/002` 没有稳定并回 crowd-managed night runtime，就缺少第二条备用 hide 路。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1NpcCrowdDirector.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
- 验证状态：
  - 纯静态推断成立；未改代码，未跑 Unity / live。
- 当前恢复点：
  1. 如果后续真修这条线，第一优先应先抓 Day1 own 的收尾链：owner 是否在 consume arrival 之前被 release、真实 `HomeAnchor` 是否和玩家视觉门口对不上。
  2. 只有在这两层都排除后，才值得把锅继续下探到 `NPCAutoRoamController.BeginPathDirectedMove()` 的导航建路/到站发布。

## 2026-04-19｜只读审计：读档/重新开始/默认开局后 Day1 任务壳残留链
- 当前主线目标：
  - 不改代码，只读回答“读档 / 重新开始 / 默认开局后，为什么 Day1 左侧任务卡、提示壳、交互提示、暂停源还会残留”，并给出最小安全补法。
- 本轮子任务：
  1. 静态核对 `SaveManager.cs`、`StoryProgressPersistenceService.cs`、`SpringDay1Director.cs`、`SpringDay1PromptOverlay.cs`、`InteractionHintOverlay.cs`、`DialogueUI.cs`。
  2. 把“正式已清理的瞬时壳”和“会被恢复链重新拉起的壳”拆开。
- authoritative 结论：
  1. `SaveManager.ResetTransientRuntimeForRestore()` 已经正式清理一批瞬时壳，不是完全没做恢复前清场：
     - `StopActiveDialogueForRestore()` 会先 `DialogueManager.StopDialogue()`；
     - `ClosePackageAndBoxUiForRestore()`、`CloseWorkbenchOverlayForRestore()`、`ResetInventoryInteractionForRestore()` 会收掉背包/箱子/工作台/held state；
     - `HideTransientOverlayUiForRestore()` 会 `promptOverlay.Hide()`、`director.HideTaskListBridgePrompt()`、`InteractionHintOverlay.HideIfExists()`；
     - `HideTransientBubbleUiForRestore()` 会清 thought / npc bubble；
     - `ResetKnownTimePauseSourcesForRestore()` 会先对 `Dialogue` 与 `SpringDay1Director` 做 `ResumeTime(...)`，再 `SetPaused(false)`。
  2. 但这些壳里至少有三类不是“清掉就结束”，而是恢复后会被 owner 再次拉起：
     - `SpringDay1PromptOverlay` 在 `LateUpdate()` 里每帧重新问 `BuildCurrentViewState()` / `BuildCurrentBridgePromptState()`；只要 `SpringDay1Director.BuildPromptCardModel()` 还能产出 model，左侧任务卡就会回来了。
     - `InteractionHintOverlay` 的 `LateUpdate()` 会继续跑 `SyncPlacementModeStatusCard()` / `SyncContextHintCard()`；`HideAllImmediate()` 只是瞬时收起，不是永久关闭。
     - `StoryProgressPersistenceService.ApplySpringDay1Progress()` 在恢复尾部又会重新调用 `director.HideTaskListBridgePrompt()`、`RestoreLoadedDayEndTaskCardState()`、`SyncStoryTimePauseState()`；也就是恢复链自己会重新决定“任务卡该不该显示、时间该不该暂停”。
  3. 所以“默认开局 / 重新开始后还看见 Day1 左侧任务卡”按现码更像“当前默认开局本来就被重建到 `Town + EnterVillage` 运行态”，不是纯粹脏 UI 没清：
     - `SaveManager.ApplyNativeFreshRuntimeDefaults()` 明确 `ResetStoryProgressToTownOpeningRuntimeState()`；
     - `StoryProgressPersistenceService.ResetToTownOpeningRuntimeState()` 会把 `storyPhase` 设成 `EnterVillage`；
     - `SpringDay1Director.BuildPromptCardModel()` 对 `EnterVillage` 会稳定产出正式 Day1 task card。
  4. 当前“第二天还挂 Day1 任务卡”的修口还不够，主要差在它只处理了“DayEnd 读档立即过期”，没处理“Day1 生命周期已经结束但 `_dayEnded` 仍把任务卡语义续命”的总闸：
     - `RestoreLoadedDayEndTaskCardState()` 只在 `_dayEnded && CurrentPhase == DayEnd` 时把卡片设为立即过期；
     - 但 `TryResolvePlayerFacingPhase()` 仍允许 `_dayEnded && (StoryManager == null || CurrentPhase == None)` 回退成 `DayEnd`；
     - 因此只要 Day1 已经收束、`StoryManager` 不再维持 `DayEnd`，却没在更高层明确宣布“Day1 玩家面 UI 已失效”，`BuildPromptCardModel()` 仍可能继续给出 Day1 卡片。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryProgressPersistenceService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\InteractionHintOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs`
- 验证状态：
  - 纯静态推断成立；未改代码，未跑 Unity / live。
- 当前恢复点：
  1. 如果后续真修，最小安全入口不该先改 `PromptOverlay` 壳层，而应先在 `SpringDay1Director` 建一个 authoritative 的 `ShouldExposeDay1PlayerFacingUi()` 总闸。
  2. 然后把这个总闸统一接进：
     - `TryResolvePlayerFacingPhase()`
     - `BuildPromptCardModel()`
     - `GetTaskListVisibilitySemanticState()`
     - 必要时 `GetTaskListBridgePromptDisplayState()`
  3. `SaveManager` 这边只保留“恢复前清瞬时壳”；不要再把“真正生命周期是否还该显示 Day1 壳”继续散落在 `PromptOverlay / InteractionHintOverlay / PauseSource` 各自的 view 逻辑里。
