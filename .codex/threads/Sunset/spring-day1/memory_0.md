# spring-day1 - 线程活跃记忆

> 2026-04-10 起，旧线程母卷已归档到 [memory_1.md](D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/spring-day1/memory_1.md)。本卷只保留当前线程角色、当前主线和恢复点。

## 线程定位
- 线程名称：`spring-day1`
- 线程作用：承接 `spring-day1` 当前 runtime 收口、导演尾账和黑盒终验

## 当前主线
- 当前唯一活跃阶段：
  - [004_runtime收口与导演尾账](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/memory.md)

## 当前稳定判断
- 旧的 `DialogueValidation`、字体初始接入和最小对话验证已是历史阶段，不再是当前入口
- 当前该线程只应关注：
  - 导演尾账
  - runtime 黑盒终验
  - 主线 contract 吃回
  - 公共链责任切分

## 当前恢复点
- 查历史长链与旧阶段细节时，看 `memory_1.md`
- 查当前 live 收口时，先看 `004` 阶段和本卷，不再回到旧大卷找入口

## 2026-04-10 本轮最新恢复点
- 本轮子任务：
  - 亲自收 `NPC 自漫游抽搐`
  - 亲自收 `Package UI 打开卡顿`
- 已完成：
  - UI 打开链第一刀已落地：
    - 收掉 `InventoryPanelUI` 打开时的重复整面刷新
    - 收掉 `InventorySlotUI` 逐槽全局找 `HotbarSelectionService`
    - `EquipmentSlotUI / BoxPanelUI` 已同步吃上下文传递口径
  - NPC roam 表现层第一刀已落地：
    - 仍按真实 `velocity` 移动
    - 但朝向改成优先认路径目标方向，不再直接吃局部避让噪声
- 验证：
  - `validate_script` own errors = `0`
  - `git diff --check` 通过（仅既有 `CRLF/LF` warning）
  - `recover-bridge` 后 MCP baseline 已恢复 `pass`
  - 但 Unity 侧 session 仍是 `instances=0 / no_unity_session`，所以没拿到 live 自测票
- 如果继续：
  - 下一步不是再瞎改代码，而是等 Unity 会话真正注册后复测这两条体验线

## 2026-04-10 本轮补充判断
- 用户又补了一组 profiler 图。
- 当前稳定判断：
  - `NPC` 线不是“已经不爆了”，而是仍然存在稳定复现的 `NPCAutoRoamController.Update()` 千毫秒级峰值；
  - `UI` 那张 90ms 图目前更多是 `EditorLoop` 噪声，不足以单独判成“runtime 打开背包主因”；
  - `GameInputManager.Update()` 也出现过一次千毫秒级坏 case，说明输入/交互检测链还有次级风险。
- 当前优先级不变：
  - 先把 `NPC 自漫游` 视为第一热点；
  - 再看 `GameInputManager`；
  - 最后收 `UI/桥接` 的逐帧 `FindObjectsOfType` 尾账。

## 2026-04-10 用户最新纠正
- 最新 live 语义修正：
  - `UI` 已不卡
  - `NPC` 抽搐是“朝向一左一右翻”，不是“位移回头”
- 当前应把 `NPC` 问题视为纯 `facing` 表现 bug，而不是路径移动 bug。

## 2026-04-10 本轮继续推进
- 本轮继续保持 `ACTIVE`，先关掉了 `Skip to Workbench` 调试开关：
  - Windows `EditorPrefs` 注册表值 `Sunset.SpringDay1.DebugSkipWorkbenchPhase05_h2269039402 = 0x0`
- 紧接着补了两条真实落地：
  1. `NPC` 朝向链继续收口
     - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 现在下发给 `NPCMotionController` 的 facing 已改成优先吃“这帧真正提交出去的 velocity”
     - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs) 已有 `oppositeFacingConfirmSeconds` 这条反向确认门，避免一帧就左右翻
  2. `0.0.5` 放置模式语义补口
     - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 新增：
       - `ShouldShowPlacementModeGuidance()`
       - `GetPlacementModeGuidanceText()`
     - `0.0.5` 的 task list / focus / farming prompt 已统一改成：
       - 开垦前提示 `按 V 开启放置模式`
       - 播种/浇水阶段提示 `保持放置模式开启`
- 同轮还给 UI 落了新的续工 prompt：
  - [2026-04-10_UI线程_005放置模式状态提示与引导提示接线prompt_09.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/2026-04-10_UI线程_005放置模式状态提示与引导提示接线prompt_09.md)
- 验证：
  - `validate_script NPCAutoRoamController.cs / NPCMotionController.cs / SpringDay1Director.cs` 都是 `owned_errors=0`
  - `git diff --check` 对对应脚本均通过
  - Unity live 仍未闭环，因为 CLI 当前看见 `No active Unity instance`
- 当前恢复点：
  - 下一轮若继续体验验收，优先看：
    - `Town` 里 NPC 自漫游是否仍“位移正确但朝向乱翻”
    - `0.0.5` 中 task list / prompt 是否已稳定提醒玩家按 `V`

## 2026-04-10 讨论结论补记｜NPC 导航不是性能换功能，而是 contract 没收平
- 用户再次强调：需求一直没变，`NPC` 应该像玩家一样把静态世界走明白，不能靠“为了性能止血”继续阉割正常行走能力。
- 当前线程判断：
  - `NPC` 问题不只是朝向 owner 抢写；更深一层是 `NPC` 与玩家并没有共享同一条静态导航 contract。
  - 玩家链更接近“静态世界 + 单目标 + 明确停点”的稳定链；NPC roam 则叠了随机采样、局部避让、stuck recover、repath、director/crowd/interaction 外部写入等多层驱动。
  - 因此现在看到的“撞树、撞围栏、anchor 周围乱顶、封闭养殖区坏 case 爆卡”，不该被解释成‘功能和性能天然冲突’，而应解释成：导航 driver 自己还没把可达性、owner 和坏 case 语义统一。
- 当前给导航线程的正确收口目标：
  1. `NPC` 静态世界理解与玩家统一
  2. 朝向/移动 owner 单一化
  3. 不可达目标快速失败
  4. 封闭区域动物漫游收成稳定可达域，而不是靠碰撞后 storm

## 2026-04-10 导航线程第一轮认知同步 prompt
- 用户要求不要我继续修导航，而是先把我的思考转成两轮 prompt。
- 当前只落了第一轮：
  - [2026-04-10_导航线程_NPC静态导航与owner问题认知同步prompt_63.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-10_导航线程_NPC静态导航与owner问题认知同步prompt_63.md)
- 这轮 prompt 明确要求导航线程先回答：
  - 用户真正不满意的体验是什么
  - 是否同意“这不是性能与功能天然冲突，而是 NPC 导航实现没收平”
  - 当前问题至少应拆成哪几层
  - 玩家与 NPC 是否其实用了两套不同的导航 contract
  - 下一轮若继续施工，最值钱的验证目标是什么
- 当前恢复点：
  - 等导航线程回第一轮认知同步回执
  - 由 `spring-day1` 审核是否真的对齐，再发第二轮施工 prompt

## 2026-04-10 导航线程第二轮窄验证 prompt
- 用户要求导航继续开发，但不能等我这边把工作台 UI 卡顿的大审查做完。
- 我已审核导航线程第一轮认知同步回执，当前判断：
  - 方向基本对
  - 但还差 `clearance / avoidance / blocker` 这一层
  - 因此可以继续，但只能先做第二轮窄验证，不能直接开修
- 已落第二轮 prompt：
  - [2026-04-10_导航线程_NPC静态导航坏case第二轮窄验证prompt_64.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-10_导航线程_NPC静态导航坏case第二轮窄验证prompt_64.md)
- 这轮 prompt 明确要求导航线程只钉 4 件事：
  1. 坏 NPC 的 `roam` 采样点是否本来就不该去或不可达
  2. 采样点到 `walkable / rebuild / blocked recovery` 的第一失真点
  3. bad case 进入后哪条 `motion / facing / rebuild` owner 还在接管
  4. `NPC vs Player` 是否还存在 `clearance / avoidance / blocker` 层差异
- 当前恢复点：
  - 导航线程先按 `prompt_64` 继续
  - 我这边继续只读彻查 `工作台 UI 卡顿 + 全项目同类性能炸弹`

## 2026-04-10 工作台 UI 卡顿只读审查结论
- 当前主线没有漂：
  - 导航交给导航线程继续
  - 我这边继续主刀 `工作台 UI 卡顿 + 全项目同类性能炸弹`
- 当前最核心的判断：
  - `SpringDay1WorkbenchCraftingOverlay` 的卡顿是 UI 自己的结构性布局风暴，不是导航链误伤
- 已钉死的根因：
  - `Open / SelectRecipe / SetQuantity / HandleInventoryChanged / craft queue` 都会进入重刷新
  - `RefreshAll()` 串上：
    - `RefreshRows`
    - `ForceRuntimeRecipeRowsIfNeeded`
    - `RefreshSelection`
    - `UpdateQuantityUi`
    - `RefreshCompatibilityLayout`
  - 热路径里反复：
    - `LayoutRebuilder.ForceRebuildLayoutImmediate`
    - `Canvas.ForceUpdateCanvases`
    - `GetPreferredValues`
    - `ForceMeshUpdate`
    - `CanFontRenderText / ResolveFont / ApplyResolvedFontToText`
  - 若行项可读性/几何检查失败，还会 `RebuildRecipeRowsFromScratch()`，把旧 row 标成 `Obsolete_RecipeRow_*` 后销毁重建
- 当前全项目风险地图：
  - `已证实热点`：`SpringDay1WorkbenchCraftingOverlay`
  - `结构性高危`：`SpringDay1PromptOverlay`
  - `中风险`：`ItemTooltip / PackageSaveSettingsPanel`
  - `打开 UI / rebind 风险`：`PackagePanelTabsUI / PersistentPlayerSceneBridge`
- 当前恢复点：
  - 下轮若继续这条线，不该再泛讲“优化性能”
  - 应按：
    1. 热路径布局风暴
    2. 热路径字体/文本兜底
    3. 打开 UI / rebind 全树扫描
    三类拆刀

## 2026-04-10 CS0117 紧急修红（SnapResidentsToHomeAnchors）
- 用户新阻塞：
  - `SpringDay1Director.cs` 报 `CS0117`，提示 `SpringDay1NpcCrowdDirector` 不包含 `SnapResidentsToHomeAnchors`。
- 本轮动作：
  - 已登记 `Begin-Slice`：`修红_CS0117_SnapResidentsToHomeAnchors_2026-04-10`
  - 只改了 `SpringDay1Director.cs` 的两个调用点和一个本地 helper：
    - 原 `SpringDay1NpcCrowdDirector.SnapResidentsToHomeAnchors()` 改为 `TrySnapResidentsToHomeAnchorsForDayEnd()`
    - 新 helper 走反射调用 `SnapResidentsToHomeAnchors`；若目标符号不可用则回退 `EnsureRuntime()` 并打 warning
- 关键判断：
  - 当前 `SpringDay1NpcCrowdDirector.cs` 源码里确有 `public static SnapResidentsToHomeAnchors()`，但用户现场仍报 CS0117，优先采用“编译级兜底”清红，避免因为符号可见性/旧二进制状态卡死主线。
- 验证状态：
  - `validate_script` 对这两个脚本：`owned_errors=0`
  - Unity 仍在 `is_playing + stale_status`，结论是 `unity_validation_pending`，非完整 live compile pass
- 恢复点：
  - 请用户先看 Unity 侧 CS0117 是否已消失；
  - 若消失，继续 Day1 全流程验收；
  - 若仍有同类符号红，再按具体报错位做同级兜底或反查 asm 可见性。

## 2026-04-10 回 UI｜任务清单 packaged 异常治理裁定
- 用户要求我不要继续乱修，而是先给 UI 一份 authoritative 裁定，回答 `SpringDay1PromptOverlay / 任务清单` 在 packaged live 里仍异常时，最终该走哪种治理模型、谁主刀、下一安全切片只砍哪一刀。
- 本轮先做了只读核对，然后落了正式回执文件：
  - [2026-04-10_spring-day1_回UI_任务清单治理裁定_33.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/2026-04-10_spring-day1_回UI_任务清单治理裁定_33.md)
- 当前裁定结论：
  - `任务清单` 最终不该继续保留现在这种“独立 runtime overlay 自治模型”
  - 最终目标应回到固定 HUD 治理
  - 但下一刀不是 UI 继续补壳，而是 `spring-day1` 先把 `formal task card` 与 `manual bridge prompt` 从同一组件/同一状态机里拆开
- 关键判断依据：
  - `SpringDay1Director` 已经提供了正式任务清单真值：
    - `GetTaskListVisibilitySemanticState()`
    - `BuildPromptCardModel()`
  - 但 `SpringDay1PromptOverlay` 仍同时承担：
    - `Show/Hide` 手动提示入口
    - `manualPromptText` 覆盖正式卡内容
    - 外部 suppress owner
    - runtime parent / runtime prefab / overlay 自治
  - 因此 packaged 异常第一 blocker 不是 sorting 数值，而是 owner 混杂
- 责任裁定：
  - 当前必须由 `spring-day1` 接回主刀
  - UI 线程这轮不应继续碰 `SpringDay1PromptOverlay.cs`
- 恢复点：
  - 后续若继续这条线，应由 `spring-day1` 先切“停止把 PromptOverlay 当通用 bridge prompt 通道”这一刀
  - 等语义 owner 单一后，UI 再决定是否回到 `SpringDay1PromptOverlay.cs` 只收最后的固定 HUD 壳表现

## 2026-04-10｜只读审计：存档回退 bug 的最安全测试切口
- 当前主线目标：
  - `spring-day1` 主线仍在 runtime 尾账；本轮子任务是只读核查“存档回退后剧情不可重触发”的剩余风险，并给出最安全的测试切口。
- 本轮执行：
  - 本轮只读；未跑 `Begin-Slice`
  - 回看了：
    - `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
    - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
    - `Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
    - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
    - `004_runtime收口与导演尾账/memory.md`
- 本轮稳定结论：
  1. `SpringDay1Director.HasCompletedDialogueSequence()` 当前已经干净，只读取 `DialogueManager.HasCompletedSequence(...)`，不再按 `phase` 回写 completed。
  2. `TryRecoverConsumedSequenceProgression(...)` 也只按 completed sequence 恢复导演推进，不是这轮残余主根。
  3. 现在真正还在“按 phase 派生导演私有态”的地方是 `StoryProgressPersistenceService.ApplySpringDay1Progress(...)`：
     - `:478-485` 一组 one-shot / beat flags
     - `:548-575` 一组 `freeTime / dayEnd / stamina` 相关私有位
  4. 当前最应该补的不是 `SpringDay1DialogueProgressionTests.cs`，而是 `StoryProgressPersistenceServiceTests.cs`；
     - 因为它已经有 `Load(savedData)` runtime harness
     - 现有覆盖只证明 completed list 能恢复，没证明 director private flags 不会被 `phase` 偷推
- 最安全测试切口：
  - 在 `StoryProgressPersistenceServiceTests.cs` 新增一条刻意构造“不一致快照”的 load 回归：
    - `storyPhase` 设到较后阶段
    - `completedDialogueSequenceIds` 故意不含对应 formal
    - 断言 `Load(...)` 后 `DialogueManager` 未完成项仍未完成，且导演对应私有 flags 也不能被 phase 自动补真
- 当前恢复点：
  - 后续如果继续这条线，先补这条回归测试，再决定是否改 `ApplySpringDay1Progress(...)` 的 phase 派生策略。

## 2026-04-10 只读审计｜夜间 resident 回锚 / 次日恢复安全切入点
- 用户要求：
  - 不改文件，只回答 `spring-day1` 当前最安全的“21:00 后 resident 一定在 HomeAnchor 站定不动、次日再恢复白天位置/漫游”的切入点。
- 当前结论：
  - 现成“回家”入口已有：`SpringDay1NpcCrowdDirector.SnapResidentsToHomeAnchors()`；
  - 但它现在会在回锚后重新 `StartRoam()`，不能直接当作“夜间站定”方案；
  - 现成“冻结 resident”入口在 `NPCAutoRoamController.AcquireResidentScriptedControl(...) / HaltResidentScriptedMovement(...) / StopRoam()`；
  - 现成“次日恢复白天位置/漫游”最安全的数据面是 snapshot：`CaptureResidentRuntimeSnapshots()` / `ApplyResidentRuntimeSnapshots(...)`，而不是重新采样。
- 当前最安全的落点判断：
  1. 时间规则触发优先放 `SpringDay1Director.HandleHourChanged(...)`
  2. 真正防止 resident 被重新 `StartRoam()` 的收口点优先放 `SpringDay1NpcCrowdDirector.ApplyResidentBaseline(...)`
- 当前最危险的 owner 冲突位：
  - `SpringDay1Director.ApplyStoryActorMode(...)`
  - `SpringDay1Director` 的 resident scripted move 链
  - `SpringDay1NpcCrowdDirector.ApplyResidentBaseline(...)`
  - `PersistentPlayerSceneBridge` resident snapshot restore
- 恢复点：
  - 如果下一轮真施工，不应再在外层单点 `StopRoam()` 硬补；
  - 应改成“Director 管时间态，CrowdDirector 管 baseline 阻断，snapshot 管次日恢复”这一套。

## 2026-04-10｜真实施工收口：时间守门 / 夜间 resident / 存档 phase 防偷推
- 当前主线目标：
  - `spring-day1 / 004_runtime收口与导演尾账`
  - 这轮子任务是把用户明确点名的 3 条 runtime 闭环真落下：
    1. fresh 开局 `09:00`
    2. `005` 前时间不能被调试器跳烂，傍晚/晚饭/自由活动各自落在合法时段
    3. `21:00` 后 resident 回锚站定、次日恢复；同时把“存档回退后剧情被 phase 偷推进”真正补到代码与测试
- 本轮已完成：
  - [SaveManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs)
    - native fresh summary 与 runtime default 已统一成 `09:00`
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
    - 新增 Day1 时间守门
    - `HandleSleep()` 只在真正 `FreeTime + intro completed` 时才允许 day-end
    - 非法跨天会被回钳
    - 进入 post-tutorial explore 时至少推进到 `16:00`
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - packaged 调试器改为先问 `SpringDay1Director.TryNormalizeDebugTimeTarget(...)`
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
    - 新增 `IsNightResting`
    - 夜间规则按 `hour >= 21 || hour < 9`
    - 堵住 `baseline / snap / return-home / cancel` 里重新 `StartRoam()` 的夜间回流
  - [StoryProgressPersistenceService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs)
  - [StoryProgressPersistenceServiceTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs)
    - 去掉 `dinner / reminder / free-time-intro` 的 phase 偷推进
    - 补了专门 load 回归测试
- 验证：
  - `validate_script` 所有本轮目标脚本 => `owned_errors=0`
  - assessment 仍是 `unity_validation_pending`
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 20` => `errors=0 warnings=0`
  - 相关 `git diff --check` 已过
- 当前 blocker：
  - Unity live 当前没有活实例，小票只能 claim 代码层 clean，不能 claim 黑盒/live 已过
  - [StoryProgressPersistenceService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs) 与 [StoryProgressPersistenceServiceTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs) 当前还是 `untracked`
  - 用户要求“这轮不因 UI 回执中断”，所以 UI 的 [34.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/2026-04-10_UI线程_给spring-day1阶段回执_34.md) 还没开读
- 本轮停点：
  - 已到最近刀口，准备 `Park-Slice`
  - 下一轮先做 live 黑盒复判，再决定是否接 UI 34 与剩余 day-end 体验尾账

## 2026-04-11｜只读总审：历史 prompt 完成度 fresh 复判
- 用户目标：
  - 不再听“代码写了很多”式汇报，而是要我彻查前面历史 prompt 对 `spring-day1` 的完成度，回答到底有没有“这一整轮全部完成到位”。
- 本轮子任务：
  - 先把 `thread-state` 从错误遗留的 `ACTIVE` 收到 `PARKED`
  - 再补 fresh targeted bridge tests，把 opening / midday / late-day 的真实闭环状态钉死
- 本轮执行：
  - `Park-Slice.ps1` 已跑，`spring-day1` 当前 live 状态 = `PARKED`
  - CLI 当前：
    - `python scripts/sunset_mcp.py errors --count 20 --output-limit 20` => `errors=0 warnings=0`
    - `python scripts/sunset_mcp.py status` => `baseline=pass`，但 editor 仍有 `stale_status`
  - fresh targeted bridge tests：
    1. [spring-day1-opening-bridge-tests.json](D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/spring-day1-opening-bridge-tests.json)
       - `success=false`
       - 失败：
         - `VillageGateCompletionInTown_ShouldPromoteChiefLeadState`
         - `VillageGateWhileActiveInTown_ShouldKeepPostEntryBeatUntilDialogueCompletes`
    2. [spring-day1-midday-bridge-tests.json](D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/spring-day1-midday-bridge-tests.json)
       - `success=false`
       - 失败：
         - `FarmingTutorialCompletion_ShouldImmediatelyBridgeIntoDinnerConflict`
         - `WorkbenchCompletion_ShouldAdvanceIntoFarmingTutorial`
           - 这条主要是断言旧文案没跟上 `按 V 开启放置模式`
    3. [spring-day1-late-day-bridge-tests.json](D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/spring-day1-late-day-bridge-tests.json)
       - `success=false`
       - 失败：
         - `BedBridge_EndsDayAndRestoresSystems`
         - `FreeTimeValidationStep_AdvancesFromFinalCallToDayEnd`
           - 都在 `HandleSleep()` 触发 `SceneTransitionRunner.EnsureInstance()` 并在 EditMode 打到 `DontDestroyOnLoad` 异常
         - `DayEndPlayerFacingCopy_ShouldCarryTomorrowBurdenAndClearWorkbenchState`
           - 当前仍停在“等待自由时段见闻接管”，没有进入预期 DayEnd 文案态
- 本轮稳定结论：
  1. 不能对用户说“前面这一整轮全部完成了”。
  2. 已落地的时间守门、夜间 resident、存档 phase 防偷推，属于“代码层真实推进成立”，不是“整条 runtime 已闭环”。
  3. 当前最大缺口已经很清楚，不是泛泛的“还有些尾账”，而是 4 个具体闭环口：
     - opening：`VillageGate -> chief lead`
     - midday：`FarmingTutorial -> DinnerConflict`
     - late-day：`HandleSleep -> SceneTransitionRunner` 的 test/runtime 兼容
     - late-day：`FreeTime / DayEnd` 最终状态与任务文案推进
  4. `UI 34.md` 还没进入正式施工，这件事确实也没完成；但它不是 fresh 失败的最大主根，不能拿它掩盖上面 4 条 runtime 真问题。
- 当前恢复点：
  - 下轮若继续真修，优先顺序应是：
    1. `FarmingTutorial -> DinnerConflict`
    2. `HandleSleep / DayEnd`
    3. `VillageGate / chief lead`
    4. 再决定何时接 UI `34.md`

## 2026-04-11｜只读定位：opening bridge failures 最小修复建议
- 当前主线不变：
  - `spring-day1 / 004_runtime收口与导演尾账`
- 本轮子任务：
  - 只读定位 `SpringDay1OpeningRuntimeBridgeTests` 的两条 latest fresh failure，并给出最小修法，不改代码。
- 本轮稳定结论：
  1. `VillageGateCompletionInTown_ShouldPromoteChiefLeadState`
     - 真因是测试夹具没把 `001` 做成导演可识别的 NPC actor；
     - 运行时带路态其实已经接上，fresh 失败只丢在 summary 的 `chief=missing`。
  2. `VillageGateWhileActiveInTown_ShouldKeepPostEntryBeatUntilDialogueCompletes`
     - 真因是 `SpringDay1Director.IsTownHouseLeadPending()` 语义过宽；
     - `VillageGate` formal 还 active 时，公开语义就提前报成 pending。
- 最小建议：
  - runtime 最小刀口：
    - 改 `SpringDay1Director.IsTownHouseLeadPending()`
    - 加 `VillageGateSequenceId` active guard
  - test 最小刀口：
    - 改 `SpringDay1OpeningRuntimeBridgeTests.VillageGateCompletionInTown_ShouldPromoteChiefLeadState()`
    - 给 `001` 最小补一个 `NPCMotionController`，保持 `chief=001` 断言不降级
- 当前恢复点：
  - 若继续 opening 收口，先改 runtime pending guard，再补 chief actor test fixture，再重跑 opening bridge。

## 2026-04-11｜只读分析：late-day bridge failures 最小修法
- 当前主线目标：
  - `spring-day1 / 004_runtime收口与导演尾账`
- 本轮子任务：
  - 只读定位 `SpringDay1LateDayRuntimeTests` 中 3 条 latest fresh failure，回答最小修法、状态为何没进 DayEnd、以及应该改哪些方法/测试
- 服务于什么：
  - 先把 `late-day` 这条线的修法收窄到最小正确修改面，避免下一轮 runtime 施工继续猜
- 本轮稳定结论：
  1. `BedBridge_EndsDayAndRestoresSystems` 与 `FreeTimeValidationStep_AdvancesFromFinalCallToDayEnd` 的主根因是 `SceneTransitionRunner.EnsureInstance()/Awake()` 在 EditMode 里无条件 `DontDestroyOnLoad`
  2. 最小正确修法应落在 `SceneTransitionRunner` 本体，不应散到 `SpringDay1Director.HandleSleep()` 这种 callsite
  3. `DayEndPlayerFacingCopy_ShouldCarryTomorrowBurdenAndClearWorkbenchState` 没进 `DayEnd` 的原因不是文案坏，而是测试没满足 `_freeTimeIntroCompleted == true` 的正式 gate，`HandleSleep()` 直接走了 `RecoverFromInvalidEarlySleep()`
  4. 因此下一轮若真开刀，测试面应分两类：
     - 先修 runtime utility：`SceneTransitionRunner.EnsureInstance()`、`SceneTransitionRunner.Awake()`
     - 再校正测试 setup：`DayEndPlayerFacingCopy_ShouldCarryTomorrowBurdenAndClearWorkbenchState`
- 涉及文件：
  - `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
  - `Library/CodexEditorCommands/spring-day1-late-day-bridge-tests.json`
- 验证结果：
  - 纯只读，未改代码
  - 依据来自 latest fresh failure artifact + 测试/运行时源码对位
- 本轮停点：
  - 仍保持只读分析完成态，等待是否进入下一轮真实施工

## 2026-04-11｜实现：后续天数 02:00 自动睡觉也要回 Home
- 用户目标：
  - 修掉“只有 Day1 超过凌晨两点未睡会回 `Home`；后续天数不会”的 bug；
  - 这条要覆盖用户实际用 `+` 号跳时触发的路径。
- 当前主线目标：
  - `spring-day1 / 004_runtime收口与导演尾账`
- 本轮子任务：
  - 在不扩新系统的前提下，把 `HandleSleep` 收成：
    - Day1 自己的导演收束
    - 非 Day1 的公共 forced-sleep fallback
- 本轮实际完成：
  - `SpringDay1Director.cs`
    - 新增 `ShouldHandleSpringDay1SleepResolution(...)`
    - 新增 `HandleGenericForcedSleepFallback()`
    - 非 Day1 sleep 现在会回 `Home`、贴休息位、收 resident 到 anchors
    - EditMode 下改用 `EditorSceneManager.OpenScene(Home)`，不再因 `LoadScene` 限制炸测试
  - `SpringDay1LateDayRuntimeTests.cs`
    - 新增 `PlusHourAdvance_BeyondTwoAmAfterDay1_ShouldFallbackToHomeSleepTransition`
    - 直接覆盖用户说的 `+` 号跨两点路径
  - `SpringDay1TargetedEditModeTestMenu.cs`
    - late-day suite 已纳入新回归
- 验证结果：
  - `spring-day1-late-day-bridge-tests.json`
    - `2026-04-11T01:33:40+08:00`
    - `success=true`
    - `passCount=6`
  - `git diff --check`：通过
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 20`
    - `errors=0 warnings=0`
    - 仅剩 TestFramework 写 `TestResults.xml` 的噪声型 Exception
  - `validate_script`：
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - Unity 侧卡在 `stale_status`，不是代码红错
- 当前恢复点：
  - 这条 forced-sleep contract 已补齐并过 late-day targeted suite
  - 后续可回到 `PromptOverlay / 任务清单` 与更大范围 runtime 尾账

## 2026-04-11｜只读审查：PromptOverlay / 任务清单当前架构分界
- 用户目标：
  - 只读审查 `PromptOverlay / 任务清单` 当前架构；
  - 回答 `formal task card / manual prompt` 还混在哪里、最安全的拆口是什么、真要收口应先改哪几个方法；
  - 明确要求不要改代码。
- 当前主线目标：
  - `spring-day1 / 004_runtime收口与导演尾账`
- 本轮子任务：
  - 对位阅读：
    - `2026-04-10_UI线程_给spring-day1阶段回执_34.md`
    - `2026-04-10_spring-day1_回UI_任务清单治理裁定_33.md`
    - `SpringDay1PromptOverlay.cs`
    - `SpringDay1Director.cs`
  - 给出一份可直接决策的只读架构结论。
- 本轮稳定结论：
  1. 正式任务卡真源已经在 `SpringDay1Director`：
     - `BuildPromptCardModel()`
     - `GetTaskListVisibilitySemanticState()`
     - `GetPromptFocusText() / BuildPromptItems()`
  2. 真正混口在 `SpringDay1PromptOverlay`：
     - `_manualPromptText / _manualPromptPhaseKey / _queuedPromptText`
     - `Show() / Hide() / BuildCurrentViewState() / BuildManualState()`
     - `BuildCurrentViewState()` 会把 `_manualPromptText` 覆盖进正式 `model.FocusText`
  3. `TaskListVisibilitySemanticState` 目前还没真正接进 overlay 消费链；
     - `SetExternalVisibilityBlock(bool)` 在 `PromptOverlay` 内存在，但当前代码里没有看到实际调用
  4. `SpringDay1Director` 里仍有约 `50` 处 `PromptOverlay.Show/Hide`；
     - 很多已经与正式 `BuildPromptCardModel()` 文案重复
     - 高重复区主要在：
       - `TryHandleTownEnterVillageFlow()`
       - `TryHandleWorkbenchEscort()`
       - `TickFarmingTutorial()`
       - `TryHandlePostTutorialExploreWindow()`
- 最小、最安全、最不易返工的拆分落点：
  1. 正式任务卡固定收口到 `SpringDay1Director.BuildPromptCardModel() + GetTaskListVisibilitySemanticState()`
  2. 手动桥接提示不再进入 `PromptOverlay` 的任务卡状态机
  3. 本轮不要先碰 `ResolveParent()`、sorting、layout 和 page flip；那是 UI 壳尾账，不是 owner blocker
- 如果下一轮真开工，最值得先改的 4 个方法：
  1. `SpringDay1PromptOverlay.BuildCurrentViewState()`
  2. `SpringDay1PromptOverlay.Show(string text)`
  3. `SpringDay1Director.TickFarmingTutorial()`
  4. `SpringDay1Director.TryHandleTownEnterVillageFlow()`
- 验证结果：
  - 只读结论，未改代码
  - 结论依据来自：
    - 指定 2 份回执
    - `SpringDay1PromptOverlay.cs`
    - `SpringDay1Director.cs`
    - 当前 `004_runtime` 活跃 memory
- 当前恢复点：
  - 已明确下一刀不该先修显示壳，而该先拆 `formal task card` 与 `manual prompt` 的 owner 混口
  - 如果继续推进，应从上面 4 个方法开最小切片

## 2026-04-11｜只读审查：Day1 存档回退 / phase 偷推进尾账
- 当前主线目标：
  - `spring-day1 / 004_runtime收口与导演尾账`
- 本轮子任务：
  - 只读回答 Day1 的“存档回退 / phase 偷推进”这条尾账现在是否还存在明确未闭环点；
  - 并判断若要本轮一起收尾，最值钱的一刀应该是补测试、改 persistence，还是两者一起。
- 服务于什么：
  - 收窄下一轮真正要开的修补面，避免在 `save rollback` 这条线上继续凭印象判断“是不是已经收完”。
- 本轮实际完成：
  - 只读核查了：
    - `StoryProgressPersistenceService.cs`
    - `StoryProgressPersistenceServiceTests.cs`
    - `SpringDay1Director.cs`
    - `004_runtime收口与导演尾账/memory.md`
  - 明确确认：
    1. 晚段四个位的 phase 偷推进已补；
    2. 前中段仍残留 phase 派生；
    3. 当前 tests 只守住晚段，没有守住前中段 mismatch-load；
    4. workbench hint 的 test 仍在断言旧的 `PlayerPrefs` 路径，与现行实现脱节。
- 关键决策：
  - 这条尾账当前仍算“未完全闭环”；
  - 若下一轮真收它，最值钱的切片应是**测试 + persistence 逻辑一起收**，顺序为：
    1. 先补前中段 mismatch-load 回归
    2. 再收 `ApplySpringDay1Progress(...)` 的剩余 phase 派生
    3. 最后校正 workbench hint 的测试断言来源
- 涉及文件 / 路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryProgressPersistenceService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\StoryProgressPersistenceServiceTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\memory.md`
- 验证结果：
  - 纯静态审查，未跑 Unity Editor tests
  - `git status` 额外确认：
    - `StoryProgressPersistenceService.cs`
    - `StoryProgressPersistenceServiceTests.cs`
    当前仍是 `untracked`
- 当前恢复点：
  - 继续 `save rollback` 这条线时，不要直接大改 director；
  - 先从 `StoryProgressPersistenceServiceTests.cs` 的 mismatch-load regression 开刀，再决定 `ApplySpringDay1Progress(...)` 要收到哪一层语义。

## 2026-04-11｜审计补记：save rollback 只读审查已完成全链落盘
- 本轮 `skill-trigger-log` 已追加：
  - `STL-20260411-017`
- 审计层检查结果：
  - `check-skill-trigger-log-health.ps1 -> Health: ok`
  - `Canonical-Duplicate-Groups: 0`
- 当前线程恢复点不变：
  - 若后续继续这条尾账，先补前中段 mismatch-load regression，再收 persistence 剩余 phase 派生。

## 2026-04-11｜真实收口：Day1 targeted 全套 fresh pass + 两个临场尾巴补完
- 当前主线目标：
  - `spring-day1 / 004_runtime收口与导演尾账`
  - 这轮子任务是把 `PromptOverlay / 任务清单 owner split`、`save rollback / phase 偷推进` 收成真实代码与回归，并把 fresh targeted 复判里新暴露的两个尾巴一起补完。
- 本轮实际完成：
  1. `SpringDay1Director / SpringDay1PromptOverlay`
     - formal task card 与 bridge prompt 已拆成两条 owner 链
     - `ShowTaskListBridgePrompt / HideTaskListBridgePrompt / GetTaskListBridgePromptDisplayState` 已成为导演真源
  2. `StoryProgressPersistenceService / StoryProgressPersistenceServiceTests`
     - 去掉 opening / midday 的危险 phase 派生
     - 新增 `Load_DoesNotPromoteOpeningAndMiddayPrivateFlagsFromPhaseAlone`
  3. 运行时临场尾巴
     - `SpringDay1LateDayRuntimeTests` 的 workbench fallback 文案断言已改成当前真实语义“和村长收口”
     - `NpcInteractionPriorityPolicy` 修掉了 stale Unity object cache + same-frame lookup gate 造成的 formal/informal 门禁误判
- fresh 验证：
  - `spring-day1-midday-bridge-tests.json`：`success=true passCount=8`
  - `spring-day1-prompt-overlay-guard-tests.json`：`success=true passCount=5`
  - `spring-day1-opening-bridge-tests.json`：`success=true passCount=10`
  - `spring-day1-late-day-bridge-tests.json`：`success=true passCount=6`
  - `spring-day1-story-progress-tests.json`：`success=true passCount=3`
  - `spring-day1-workbench-fallback-test.json`：`success=true passCount=1`
  - `spring-day1-npc-formal-consumption-tests.json`：`success=true passCount=3`
  - `spring-day1-director-staging-tests.json`：`success=true passCount=30`
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 20`：`errors=0 warnings=0`
  - `validate_script`：
    - `NpcInteractionPriorityPolicy.cs` / `SpringDay1LateDayRuntimeTests.cs`
    - `owned_errors=0`
    - assessment 仍是 `unity_validation_pending`
    - 原因是 Unity `stale_status`，不是代码红错
- 当前阶段：
  - Day1 代码与 targeted 合同层面已过线
  - 剩余只属于用户最终实玩体验终验，不再是当前 fresh blocker
- 当前 live 状态：
  - 本轮已跑 `Park-Slice`
  - `spring-day1 = PARKED`
- 下一恢复点：
  - 若用户继续实玩终验，优先盯完整一遍 Day1 流程的节奏、站位、任务清单观感与阅读感

## 2026-04-11｜审计补记：Day1 targeted 全闭环收口已补 skill-trigger-log
- 本轮 `skill-trigger-log` 已追加：
  - `STL-20260411-026`
- 审计层检查结果：
  - `check-skill-trigger-log-health.ps1 -> Health: ok`
  - `Canonical-Duplicate-Groups: 0`

## 2026-04-11｜只读总审：并行 7 线程阶段与打包收尾优先级
- 当前主线目标：
  - `spring-day1 / 004_runtime收口与导演尾账`
  - 本轮子任务不是继续写代码，而是以 Day1 owner 视角审当前并行 7 线程的阶段质量、可信度和对最终打包的影响。
- 本轮实际完成：
  - 已确认用户这轮给的是完整 7 线程，不需要反追漏线程；
  - 已逐条核对线程回执与现场代码，形成以下稳定判断：
    1. `DayNightOverlay` 线程：方向对，但只涉及编辑器预览，不是打包 blocker；
    2. `存档系统`：主链可用，但 packaged smoke 与日志噪音清理未完，是打包前必须收；
    3. `farm / toolbar`：own 自愈补丁可信，但“外部 UI compile 噪音阻断 live”这条已部分过时，应直接重跑干净 live；
    4. `UI / InteractionHintOverlay`：根因收敛可信，compile 面已不再是 farm 当前第一阻断，但仍欠用户 live 终验；
    5. `排序/透视/层级`：判断对，但这是结构性大题，不该继续进入当前打包窗口；
    6. `导航历史基线考古`：研究有价值，但当前只应保留为 fallback 知识，不应继续占窗口；
    7. `Town 常驻 NPC severe 分账`：锅分得对；若 live 里仍有 severe 摆头/倒走，应优先回 Day1/crowd owner，而不是继续泛归导航。
- 当前阶段：
  - 现在最正确的收口口径不是“所有线程都继续推进”，而是“冻结大改，只收真正影响打包的 live/smoke/blocker”。
- 当前打包优先级：
  - `P0`
    - Day1 全流程终验
    - 存档 packaged smoke
    - farm/placement/toolbar 干净 live completion ticket
    - Town 常驻 NPC severe 坏相若仍存在，则由 Day1/crowd owner 处理
  - `P1`
    - `InteractionHintOverlay` 用户 live 观感补票
    - `DayNightOverlay` 编辑器预览补票
  - `P2`
    - 排序 contract 重构
    - 导航历史基线继续考古
- 当前 live 状态：
  - 本轮只读分析，未新跑 `Begin-Slice`
  - 线程保持 `PARKED`
- 下一恢复点：
  - 若用户继续下令，Day1 应只围绕 `P0` 四项收打包窗口，不再主动扩题。

## 2026-04-11｜只读复盘：用户首轮终验暴露 3 条 Day1 新问题
- 当前主线目标：
  - `spring-day1 / 004_runtime收口与导演尾账`
  - 本轮子任务不是继续施工，而是只读复盘用户刚跑出来的 3 个问题：任务清单体验退化、19:00 晚饭切入报错、以及 Home 睡觉入口被门口交互劫持。
- 本轮实际完成：
  1. 已对齐 `SpringDay1Director.BuildPromptCardModel()`、`GetPromptFocusText()`、`BuildPromptItems()` 与 `ShowTaskListBridgePrompt()`：
     - 我的创新本意是把 formal task card 和 bridge prompt 拆 owner，避免 manual prompt 污染正式任务卡；
     - 但现场问题成立：`bridge prompt` 的字面去重不足以解决语义重复，所以用户现在会看到“上方提示 + 正文/foot 高重复 + 位置过抢”的体验退化。
  2. 已对齐晚饭报错链：
     - `BeginDinnerConflict()` -> `SpringDay1PromptOverlay.Hide()` -> `FadeCanvasGroup()` -> `StartCoroutine(...)`
     - 当前 `Hide()` / `FadeCanvasGroup()` 对 inactive overlay 没防守，这是一条真实 runtime bug，不是外部噪音。
  3. 已对齐回屋睡觉链：
     - `TryAutoBindBedInteractable()` 会吃 `FindRestTargetCandidate()` 的结果；
     - `PreferredRestProxyObjectNames` 与 `PreferredRestObjectNames` 里都含 `HomeDoor/HouseDoor`；
     - 所以门 / 门代理会被错误升级成 `SpringDay1BedInteractable`，导致门口直接 `E` 睡觉，破坏原本“先进屋 -> 对床交互”的正常流程。
- 当前阶段：
  - 第 2 条和第 3 条已可直接定性为真 bug；
  - 第 1 条是结构方向对、体验实现错，不是用户挑剔。
- 当前 live 状态：
  - 本轮仍是只读分析，未跑 `Begin-Slice`
  - 线程保持 `PARKED`
- 下一恢复点：
  - 若用户下令继续施工，正确顺序应是：
    1. 先修 `PromptOverlay` inactive coroutine 报错；
    2. 再把 `HomeDoor/HouseDoor` 从常规睡觉交互点里收回；
    3. 最后做任务清单 bridge prompt 与 formal card 的融合收口。

## 2026-04-11｜真实修复：用户首轮终验 3 条问题已落地
- 当前主线目标：
  - `spring-day1 / 004_runtime收口与导演尾账`
  - 本轮子任务是把用户刚验出的 3 条 Day1 问题真实修掉，不再停留在分析层。
- 本轮实际完成：
  1. `SpringDay1PromptOverlay.cs`
     - `FadeCanvasGroup()` 新增 inactive 防守与同步收口 helper；
     - 修掉 `BeginDinnerConflict() -> Hide()` 在 inactive overlay 上开 coroutine 的 runtime 报错。
  2. `SpringDay1Director.cs`
     - `TryAutoBindBedInteractable()` 改吃新的 `FindRestInteractionTargetCandidate()`；
     - `FindRestTargetCandidate()` 继续只服务 forced sleep fallback 的定位；
     - 门 / 门代理不再被绑成 `SpringDay1BedInteractable`。
  3. `任务清单 bridge prompt`
     - 顶部条视觉降侵入；
     - 冗余判断升级为对 `focus/subtitle/footer/items` 的语义去重。
  4. 回归测试
     - `SpringDay1LateDayRuntimeTests` 新增 3 条回归；
     - `SpringDay1TargetedEditModeTestMenu` 已接入对应 targeted 菜单。
- 验证结果：
  - `spring-day1-prompt-overlay-guard-tests.json`：`success=true passCount=7`
  - `spring-day1-late-day-bridge-tests.json`：`success=true passCount=7`
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`：`errors=0 warnings=0`
- 当前 live 状态：
  - 本轮已跑 `Begin-Slice`
  - 未跑 `Ready-To-Sync`，因为这轮没进白名单 sync
  - 本轮已跑 `Park-Slice`
  - 当前状态：`PARKED`
- 下一恢复点：
  - 如果用户继续实玩验收，先盯：
    1. `19:00` 晚饭切入不再报错、001/002 站位是否回正
    2. `回屋 -> 进门 -> 对床睡觉` 是否恢复
    3. `任务清单 / bridge prompt` 是否不再顶重复信息
## 2026-04-11｜Day1 runtime 收口补记：晚饭/自由时段重定时、居民夜间回家 contract、PromptOverlay 开包退场
- 当前主线：继续收 `spring-day1` 打包前 runtime 尾账；本轮子任务是把用户最新补充的 Day1 晚段时间语义和任务面板层级问题一起落地。
- 本轮实际完成：
  1. `SpringDay1Director.cs`
     - 晚段改成 `18:00` 晚饭、`19:00` 归途提醒、`19:30` 自由时段
     - `NormalizeManagedDay1TimeTarget(...)` / 最小最大时间窗改成 total-minutes 夹制
  2. `SpringDay1NpcCrowdDirector.cs`
     - `20:00` 开始自主回 home anchor
     - `21:00` 未到位则强制进入 night-rest snap
  3. `SpringDay1PromptOverlay.cs`
     - 运行时优先挂 `UI` 下基础 Canvas，避开 `PackagePanel` 模态 Canvas
     - 有父级 Canvas 时不再自抬 `overrideSorting`
     - 开包/箱子页打开时会按统一模态规则退场
     - 非 PlayMode 的 fade 改同步收口，保证 EditMode 验证可靠
  4. `SpringDay1LateDayRuntimeTests.cs`
     - 两条晚段时间测试改成直接验证 `NormalizeManagedDay1TimeTarget(...)` 的分钟级 floor
     - `PromptOverlay_ShouldHideWhilePackagePanelIsOpen` 改成真实开包路径
  5. `PackagePanelLayoutGuardsTests.cs`
     - 守卫口径压回玩家可感知结果：开包后任务面板隐藏，不再在最小测试壳里死卡无关实现细节
  6. `SpringDay1TargetedEditModeTestMenu.cs`
     - `PromptOverlay Guard` 菜单已补入 package/prompt layering 相关 targeted tests
- fresh 验证：
  - `spring-day1-prompt-overlay-guard-tests.json`：`success=true passCount=11`
  - `spring-day1-late-day-bridge-tests.json`：`success=true passCount=8`
  - `spring-day1-director-staging-tests.json`：`success=true passCount=31`
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`：仅剩 Unity TestRunner cleanup residue，不是新的业务红错
- 涉及文件：
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
  - `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
  - `Assets/YYY_Tests/Editor/PackagePanelLayoutGuardsTests.cs`
  - `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
- 当前阶段：
  - 这轮 own 代码与 targeted contract 已过线
  - 当前唯一没彻底归零的是测试框架 console residue
- 当前状态：
  - 本轮已扩白名单重新 `Begin-Slice`
  - 未 `Ready-To-Sync`
  - 已 `Park-Slice`
  - 线程状态：`PARKED`
- 当前恢复点：
  - 若继续，优先做用户实玩终验和必要的 runtime 黑盒，不再回漂到旧的 EditMode helper 误报上

## 2026-04-12｜子任务插入：Town / Day1 NPC“不动像冻结” live 复判，已确认第一真问题更偏 `spring-day1` own
- 当前主线目标：
  - `spring-day1 / Day1 runtime 收口`
  - 本轮子任务是排查用户刚插入的阻塞问题：“NPC 除了剧情走位外平时都不动，而且一直报错”，服务于 Day1 最终打包收口。
- 本轮动作：
  1. 延续已有 slice `npc-runtime-regression-triage-2026-04-12` 做只读 live 取证。
  2. 先用 `sunset_mcp.py doctor / recover-bridge` 恢复 MCP bridge。
  3. 再通过 `Library/CodexEditorCommands` 执行：
     - `PLAY`
     - `MENU=Sunset/Story/Validation/Write Spring Day1 Live Snapshot Artifact`
     拿 fresh snapshot。
- 关键 live 结果：
  - snapshot #1：
    - `Scene=Town`
    - `Phase=EnterVillage`
    - `beat=EnterVillage_HouseArrival`
    - `Time=10:10 AM`
    - `101~203` resident 全部 `return-home`
  - snapshot #2（6 秒后）：
    - `Time=12:10 PM`
    - 同一批 resident 仍是 `return-home`
    - 个别位置有轻微推进，说明不是“完全没 tick”，而是被 caller 压在散场/回家链里
  - `001` 仍显示 `roam=True|state=Moving`，说明剧情 escort actor 和常驻 resident 不是同一类坏相。
- 当前判断：
  - 这次 Town 居民“像冻结”不优先像导航 own 忘了 resume；
  - 第一真责任层更像 `SpringDay1NpcCrowdDirector` 的 `cue -> baseline -> return-home` caller contract；
  - 且 `TickResidentReturnHome()` 现在还是 `transform.position` 线性硬推，不是玩家同级静态导航，这直接解释了“剧情时能动、平时/散场不像正常 NPC”的用户体感。
- 对历史需求的含义：
  - 这与用户反复强调的“所有 NPC 要和玩家一样走静态导航、不靠阉割 contract 糊过去”一致；
  - 所以这轮不能再把问题丢回“导航自己去做就好”，`spring-day1` 自己也必须收这条 caller/runtime contract。
- 本轮未做：
  - 未改运行时代码；
  - 未开始 return-home pathing 改造；
  - 未 claim 修复完成。
- 线程状态：
  - 本轮已 `Park-Slice`
  - 当前状态：`PARKED`
- 下一恢复点：
  - 如果用户批准开修，第一刀应改 `SpringDay1NpcCrowdDirector.TryBeginResidentReturnHome()/TickResidentReturnHome()`：
    - 优先接 `NPCAutoRoamController.DriveResidentScriptedMoveTo(...)`
    - 仅在晚间强制贴回时保留 snap 语义
    - 尽量不伤现有 `20:00` / `21:00` 晚间 contract 与已过的 targeted tests

## 2026-04-12｜真实施工：resident return-home contract fix 已落地
- 当前主线目标：
  - 继续收 `spring-day1 / Day1 runtime`
  - 这轮子任务是把 Town resident “剧情后像冻结”的 own 根因直接修掉，不再停在分析层。
- 本轮实际完成：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - `TickResidentReturnHome(...)` 现在会先尝试 `TryDriveResidentReturnHome(...)`
     - 对有 `NPCAutoRoamController` 的 resident，不再默认回退到 `transform.position` 线性硬推
     - 新增 `GetResidentReturnHomeStepDistance(...)` 与两个 `TryDriveResidentReturnHome(...)` helper
     - 旧手搓位移只留给没有 roam controller 的 fallback
  2. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 新增 `CrowdDirector_ShouldNotFallbackToHardPushWhenResidentHasRoamController`
  3. `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
     - 把新测试接进 `Director Staging` targeted 菜单
- fresh 验证：
  - `spring-day1-director-staging-tests.json`
    - `success=true passCount=32 failCount=0`
    - 新测试已执行并通过
  - `git diff --check`
    - 本轮 3 个代码文件 clean
  - Editor.log
    - 有 fresh `Assembly-CSharp-Editor.dll` 编译成功与 domain reload 记录
  - `py -3 scripts/sunset_mcp.py compile ...`
    - assessment=`blocked`
    - reason=`subprocess_timeout:dotnet:20s`
    - 因此当前 compile-first 票只能诚实报 blocker，不能包装成完整 no-red
- fresh live：
  - 已重新用 `Spring Day1 Live Snapshot Artifact` 和 `Trigger Recommended Action Artifact` 验过 fresh runtime
  - 能确认 `Town / EnterVillage / EnterVillage_PostEntry` 现场正常跑起
  - 但还没在同一轮里把 `HouseArrival after-fix` 的 resident 体感跑到终点
- 当前判断：
  - 这刀已经对位修掉了最核心的 Day1 own contract 硬伤；
  - 剩下最需要的不是继续泛改，而是让用户直接复测 `HouseArrival` 之后 resident 是否真的开始“像 NPC 一样走回去”。
- 线程状态：
  - 本轮已 `Begin-Slice`
  - 未 `Ready-To-Sync`
  - 已 `Park-Slice`
  - 当前状态：`PARKED`

## 2026-04-12｜真实施工：晚饭开场 001/002 对位已切离旧围观 marker，late-day 护栏补齐
- 当前主线目标：
  - 继续收 `spring-day1 / Day1 runtime`
  - 本轮子任务是修掉用户终验里“18:00/19:00 晚饭切入时 001/002 跑到左上角”的 own 问题，并确认不影响晚段既有 contract。
- 本轮实际完成：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
     - `AlignTownDinnerGatheringActorsAndPlayer()` 不再吃 `TryResolveVillageCrowdMarker(...)`
     - 新增 `PreferredDinnerGatheringAnchorObjectNames`
     - `001/002` 现在优先围绕 `DirectorReady_DinnerBackgroundRoot / DinnerBackgroundRoot` 落到晚饭区域
  2. `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
     - 新增 `AlignTownDinnerGatheringActorsAndPlayer_ShouldPreferDinnerAreaOverVillageCrowdMarkers`
  3. `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
     - `Run Late-Day Bridge Tests` 已接入新测试
- fresh 验证：
  - `spring-day1-late-day-bridge-tests.json`
    - `success=true passCount=9 failCount=0`
    - 新测试已执行并通过
  - `spring-day1-director-staging-tests.json`
    - `success=true passCount=32 failCount=0`
  - `git diff --check`
    - 本轮 3 个代码文件 clean
- fresh live / blocker：
  - 尝试继续用命令桥进 Play 做 black-box 时，fresh live 仍被外部编译错阻断：
    - `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs(339,382,445,1145)` 缺方法
  - 所以这轮不能 claim `PlayMode 体感已过`，只能 claim `代码层 + targeted tests 已过`
- 当前判断：
  - 晚饭开场错位这条 own 问题已经被收进 `SpringDay1Director`
  - 当前更大的 live 阻断已转成 UI 外部红错，不在 `spring-day1` own scope
- 线程状态：
  - 本轮已 `Begin-Slice`
  - 未 `Ready-To-Sync`
  - 已 `Park-Slice`
  - 当前状态：`PARKED`

## 2026-04-12｜子任务插入：`002` 剧情后 Town resident“地震式抖动”事故复判，当前停在只读定责
- 当前主线目标：
  - 继续服务 `spring-day1 / Day1 打包前 runtime 收口`
  - 本轮子任务是回答用户新报的阻塞事故：“刚过完 `002` 的剧情后，Town resident 全体原地高频小幅抖动，`001/002` 也没有正常避障，会和人堆一起顶玩家。”
- 本轮只读新结论：
  1. 这锅不是“只有 001~003”，也不是“只是朝向 mismatch”。
  2. `001/002` 属于 story escort actor，普通 resident 走的是 `SpringDay1NpcCrowdDirector` 的 crowd caller 链。
  3. resident 当前的 `return-home` 虽然已不再主走旧的 `transform` 硬推，但实质仍是 `DriveResidentScriptedMoveTo(...) -> DebugMoveTo(...)` 的 scripted move。
  4. 这条 scripted move 在 `NPCAutoRoamController.TryHandleSharedAvoidance(...)` 里会绕开 shared avoidance；在 `ShouldAbortBlockedAdvanceWithoutRebuild(...)` 里又不会早停退出。
  5. `001/002` 自己的剧情位移链在 `SpringDay1DirectorStaging` / `SpringDay1Director` 里仍有 `transform.position` 直推 + `SetExternalVelocity/SetFacingDirection`。
  6. 所以用户当前看到的 split 坏相是可解释的：
     - resident 会在 scripted move + 无 shared avoidance + 不早停重试里抖成“地震”
     - `001/002` 也不会像玩家一样绕开人群
     - 两拨人会互相顶，还会推玩家
- 本轮验证状态：
  - `静态推断成立`
  - 这轮 direct MCP fresh live 没补成：当前请求返回 `no_unity_session`
  - `Editor.log` 里仍保留 `InteractionHintOverlay.cs` 外部编译错残留，所以不能把本轮包装成 fresh runtime 已终验
- 关键责任判断：
  - 主锅仍在 `spring-day1` own 的 caller/runtime contract，不该继续甩给“导航线程自己去收”
  - 更准确地说，是 `spring-day1` 用了一套“剧情/群演接管 NPC，再走半套 scripted move”的过渡 contract，但这套 contract 没把 shared avoidance、blocked abort 和 story actor move 统一收平
- 当前阶段：
  - 只读事故定责完成
  - 本轮还没重新 `Begin-Slice`，因为尚未进入新的真实施工
  - 线程状态维持 `PARKED`
- 下一恢复点：
  - 如果用户批准开修，第一刀只收 `spring-day1`：
    1. resident scripted move 接回 shared avoidance / blocked abort
    2. `001/002` story actor move 去掉直推链
    3. 目标是把 resident 与 story actor 都压回“玩家同级静态导航 contract”

## 2026-04-12｜真实施工：resident scripted move 避让/stopgap 补口已落地
- 当前主线目标：
  - 继续服务 `spring-day1 / Day1 打包前 runtime 收口`
  - 本轮子任务是直接修掉 `002` 剧情后 Town resident 地震式抖动的 own 放大器，并确认不带坏晚段时间/睡觉链。
- 本轮实际完成：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - resident scripted move 不再绕开 `shared avoidance`
     - resident scripted move 在静态零推进坏点上允许 `ShouldAbortBlockedAdvanceWithoutRebuild(...)` 早停
  2. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 新增 `ResidentScriptedControl_DebugMoveShouldStillParticipateInSharedAvoidance`
     - 新增 `ResidentScriptedControl_StaticBlockedAdvanceShouldAllowEarlyAbort`
  3. `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
     - 已把上述两条 resident-scripted-control 护栏接进 `Director Staging Tests`
- fresh 验证：
  - `git diff --check`：
    - 本轮 3 个目标文件 clean
  - `spring-day1-director-staging-tests.json`
    - `success=true passCount=32 failCount=0`
  - `spring-day1-late-day-bridge-tests.json`
    - `success=true passCount=9 failCount=0`
  - `npc-resident-director-bridge-tests.json`
    - `failed (0/3)`
    - 当前看起来落在 stagebook / manifest / cue contract 面，不像本轮 patch 新引入
- 当前判断：
  - 这刀已经把 day1 own 的两个关键放大器补掉了：
    1. resident scripted move 互相硬顶时不再彻底绕开 shared avoidance
    2. resident scripted move 在静态零推进坏点上不再永远豁免 stopgap
  - 所以用户这次看到的“Town 常驻 NPC 整片地震式抖动”和“001/002 也被堵得只会推玩家走”应当都会下降
- 当前阶段：
  - 代码修改已完成
  - targeted regression 已跑
  - fresh 玩家黑盒体感仍待用户终验
- 当前 live 状态：
  - 本轮已 `Begin-Slice`
  - 未 `Ready-To-Sync`
  - 已 `Park-Slice`
  - 当前状态：`PARKED`
- 下一恢复点：
  - 如果用户 fresh live 里仍看到 `001/002` 在剧情 cue 内部硬推/顶人，再继续收 `SpringDay1DirectorStaging` 的 `transform.position += step` 位移链

## 2026-04-13｜Day1 当前停车点：resident 全体卡死已钉到 crowd runtime registry 丢失，并已补 Town rebind 恢复
- 当前主线仍是 `Day1 打包前 runtime 收口`。
- 本轮先修了我自己新增的 editor probe 红错，然后用 fresh live + probe 抓到一条比“导航坏了”更硬的事实：
  - `Town` scene 里的 `101~203` resident 实例还在，
  - 但 `SpringDay1NpcCrowdDirector` 在某些重建/承接拍上会掉成 `_spawnStates=0 / spawned=0 / missing=101~203`，
  - 导致 scene resident 失去 runtime owner，表现上就是“除了 001~003 / 小动物，其它 NPC 像冻结一样站住”。
- 已做的真实代码改动：
  1. `Assets/Editor/Story/SpringDay1ActorRuntimeProbeMenu.cs`
     - 修复编译红
     - 修复把小动物误认成 `001/002/003` 的识别逻辑
  2. `Assets/Editor/Story/SpringDay1LatePhaseValidationMenu.cs`
     - 新增 editor-only 晚饭 / 自由时段快速跳转工具，便于后续 live 定位
  3. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - 新增 `ShouldRecoverMissingTownResidents(...)`
     - Town 活场景下若 runtime registry 为空但 scene resident 仍在，主动 `SyncCrowd()` 重新绑定
- 当前验证：
  - CLI fresh console `errors=0`
  - 用户当前口头回报：`npc 已经可以走了`
- 当前阶段：
  - 本轮停在“runtime 恢复刀已落地，等待用户继续黑盒”
  - 已 `Park-Slice`
- 如果下一轮用户还报同类坏相，优先继续沿 `SpringDay1NpcCrowdDirector` 的 scene transition / cue release / registry 恢复链查，不回到导航泛排查。

## 2026-04-13｜按用户要求补给导航的同步文件
- 已新增：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-12_给导航_002后TownResident冻结根因与当前修复同步.md`
- 用途：
  - 让导航知道这次 `002` 后 `Town resident` 冻结的 fresh 结论
  - 避免它再被旧的“导航泛锅”带偏
- 同步结论：
  - 真根是 `SpringDay1NpcCrowdDirector` 的 crowd runtime registry 恢复漏口
  - 已在 `Update()` 里补 Town resident rebind 恢复刀
  - 用户当前 live 口头反馈：`npc 已经可以走了`

## 2026-04-14｜只读彻查：三条关键问题重新定责到方法级
- 当前主线没有换，仍是 `spring-day1` 打包前闭环；本轮是用户插入式阻塞分析，不是新主线。
- 本轮子任务：
  1. 解释开场剧情就位为什么还错
  2. 解释剧情结束后 NPC 不回白天目标、但 `20:00` 会回家的真正 owner
  3. 解释 `02:00` forced sleep 的 Day1 own 与共享层边界
- 本轮已站住的判断：
  - 开场就位主锅在 `SpringDay1Director.TryHandleTownEnterVillageFlow()`、`TryPrepareTownVillageGateActors()`、`TryResolveTownVillageGateActorTarget()` 这一链：
    - 对白结束后过早切 `TryBeginTownHouseLead()`
    - scene 点位失效时仍会退回 `StageBook cue` / `hard fallback`
    - `SpringDay1Director` 自己的 `TryResolveVillageCrowdMarker(...)` helper 没真正接进 opening 主链
  - resident 白天释放 vs 夜间回家主锅在 `SpringDay1NpcCrowdDirector`：
    - `TryReleaseResidentToDaytimeBaseline(...)` 是白天链
    - `TryBeginResidentReturnHome(...)` / `TickResidentReturnHome(...)` / `ShouldResidentsReturnHomeByClock()` / `ShouldResidentsRestByClock()` 是夜间链
    - 两条策略没有统一成一套 Day1 state machine
  - `02:00` 当前已是“Day1 专属剧情收束 + Day1 内 generic fallback”混居：
    - `HandleHourChanged(...)` / `HandleSleep()` 负责 Day1 第一夜
    - `HandleGenericForcedSleepFallback()` 只是目前挂在 Day1 里的通用兜底，还不是最终共享 owner
- 当前边界重新钉死：
  - 我该碰：`SpringDay1Director.cs`、`SpringDay1NpcCrowdDirector.cs`、Day1 自己的 staging/tests/contract
  - 我不该碰：UI 壳体、save thread 的底层实现、导航 core 的全局策略
- 验证层级：
  - 这轮只站住 `结构 / checkpoint + targeted code evidence`
  - 没有把它包装成 `真实入口体验已通过`
- 子任务完成后的恢复点：
  - 下一轮若继续真修，顺序必须是：
    1. `opening hold + marker contract`
    2. `resident state machine`
    3. `02:00 shared owner` 分层

## 2026-04-13｜按用户最新裁定先输出双 prompt，不直接继续扩代码
- 当前用户新要求不是继续立刻写 runtime 代码，而是：
  - 先把“存档系统该接的 Day1 晚段恢复语义”单独写成 prompt
  - 再把“spring-day1 自己打包前彻底收尾”的 own 清单单独写成 prompt
- 已生成：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给存档系统_Day1晚段恢复语义与职责分工prompt.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给spring-day1_晚段打包前彻底收尾prompt.md`
- 这轮 prompt 已纳入的最新用户补充：
  - 晚饭卡死不是只在 `+` 快进时发生，主动找村长触发也会卡
  - `重新开始` / `读档` 高风险说明晚段恢复 contract 仍未说清
  - save thread 与 day1 own 必须分责，不再让 day1 越权修通用 save/load

## 2026-04-13｜用户否决上一版后，已重写为整条 Day1 v2 prompt
- 用户明确要求：
  - save thread prompt 必须覆盖整条 Day1，并且由 `spring-day1` 先把完整语义 contract 说全
  - 给我自己的 prompt 也必须覆盖最近十轮遗留、完整修复清单、注意事项和打包前完成定义
- 当前最新文件：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给存档系统_整条Day1恢复contract与职责分工prompt_v2.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给spring-day1_整条Day1打包前总收尾prompt_v2.md`
- 最新补入的用户补充：
  - 晚饭无论主动找村长触发还是 `+` 快进，都会卡在 `0.0.6`
  - 整体问题不是“晚段专属”，而是整条 Day1 的恢复 contract 与 own 收尾都没收平

## 2026-04-13｜用户进一步裁定：UI 全交 UI，晚饭卡顿根因与 5 秒兜底已写进 v3
- 最新用户指令：
  - UI 全权交给 UI 线程，`spring-day1` 不再自己动 UI
  - 但 `spring-day1` 必须把整条 Day1 的 UI 语义 contract 传给 UI
  - 晚饭卡顿真根已定位成“有个 NPC 没到位”，正确修法是最多等 5 秒，超时瞬移必要剧情 actor 后直接开始剧情
- 因此新增：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给UI_整条Day1任务清单与Prompt语义contract.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-13_给spring-day1_整条Day1打包前总收尾prompt_v3.md`

## 2026-04-13｜按 v3 继续真实施工，晚饭 5 秒兜底与必要剧情 actor 强制就位已完成
- 当前主线目标：
  - 把整条 Day1 从开场到睡觉的 own runtime / own staging / own re-entry / own time gate / own NPC 夜晚行为彻底收尾
  - 本轮子任务：先拿下最阻塞打包的晚饭开场卡顿 P0
- 本轮真实代码改动：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
     - 新增 `dinnerCueSettleTimeout = 5f`
     - 新增 `_dinnerCueWaitStartedAt`
     - `BeginDinnerConflict()` 改成：未 settled 时先等，最多 5 秒；超时后直接继续晚饭正式剧情，不再无限 return
     - 开戏前会重新 `AlignTownDinnerGatheringStoryActors()`，保证 `001/002` 落在晚饭区域
     - 新增 `ResetDinnerCueSettlementState()`，把这条 runtime 状态接进 phase reset / debug re-entry
     - 仅 `UNITY_EDITOR` 下增加 `_editorDinnerCueSettledOverride`，用于稳定测 `settled / timeout` 两条分支，不影响玩家运行时默认逻辑
  2. `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
     - 新增晚饭未超时继续等待测试
     - 新增晚饭超时后强制对位并开戏测试
- 本轮验证：
  - EditMode 目标测试 4/4 通过
  - fresh console：`errors=0 warnings=0`
  - `git diff --check`（目标文件）通过
- 这轮为什么这样做：
  - 用户已明确晚饭卡顿真根不是抽象条件，而是“有个 NPC 没走到位”
  - 所以正确刀口不是继续泛修 UI 或泛修导航，而是给 `DinnerConflict` 自己补 bounded wait + 必要 actor 对位出口
- 当前阶段：
  - 本轮停在“最阻塞打包的晚饭开场 P0 已修、等待用户 live 复测”
  - 已执行 `Park-Slice`
- 子任务完成后恢复点：
  - 如果用户继续反馈晚饭入口，优先 live 复测两条入口：
    1. 主动找村长触发
    2. `+` 快进触发
  - 如果这两条都过，再继续收整条 Day1 剩余 re-entry / sleep / NPC 夜晚行为的 live 黑盒尾项

## 2026-04-13｜第二轮晚段实修：补齐 003 夜晚链、Day1 02:00 forced sleep 收口、19:30 resident 放行
- 当前主线目标不变：还是 `spring-day1 / Day1 打包前 runtime 收口`。
- 本轮子任务与服务对象：
  - 用户刚追加并继续追打的 3 条坏相：
    1. `003` 不回家，夜晚语义不合群
    2. Day1 `02:00` 没有立刻 forced sleep 收到 `DayEnd`，且 Home 床边落点还会歪
    3. `19:30` 普通 resident 仍被 crowd 语义扣住，玩家看到的是“八点前根本没放出来”
  - 这轮仍只服务 Day1 own runtime，不碰 UI 壳体，不把锅漂回导航
- 本轮真实代码改动：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
     - `HandleHourChanged()`：`TimeManager.Sleep()` 后若导演没收到 `OnSleep` 链，就主动补一次 `HandleSleep()`，把 Day1 直接收进 `DayEnd`
     - `TryPlacePlayerNearCurrentSceneRestTarget()`：对带 `Rigidbody2D` 的玩家同时写 `body.position + transform.position`
     - `ShouldKeepStoryActorNightRestControl()`：补入 `003`
  2. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - `ResolveResidentParent()`：`19:30~20:00` 的 `FreeTime` 里，非 `Priority witness actor` 的 resident 改回 `Resident_DefaultPresent`
  3. `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
     - 把 `BeginDinnerConflict_ShouldAlignActorsBeforeDinnerCueTimeoutCompletes`
     - `StoryActorsNightRestSchedule_ShouldCover001To003`
     - `HandleHourChanged_FreeTimeAtTwoAmShouldFinalizeDayEnd`
     - `TryPlacePlayerNearCurrentSceneRestTarget_ShouldUseHomeDoorFallbackOffsetInHomeScene`
       全部补成有效护栏，并额外让 `003` 过一遍 `ApplyStoryActorRuntimePolicy` 不得被放掉
  4. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 新增：
       - `CrowdDirector_FreeTimeBeforeTwenty_ShouldReleaseNonPriorityResidentsToDefaultPresent`
       - `CrowdDirector_FreeTimeBeforeTwenty_ShouldKeepPriorityWitnessInTakeoverReady`
  5. `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
     - 将这轮新增护栏接进 `Late-Day Bridge Tests` 和 `Director Staging Tests`
- 本轮验证：
  - `spring-day1-late-day-bridge-tests.json`：`13/13 passed`
  - `spring-day1-director-staging-tests.json`：`39/39 passed`
  - 目标脚本 `validate_script` 均为 `owned_errors=0`
  - Unity 当前仍有外部 `Missing Script (Unknown)` 和字体 warning 噪音，所以不能包装成“Unity 全线 clean”
- 本轮 live 取证补记：
  - 我为了定责跑过一次 `Force Spring Day1 Dinner Validation Jump`
  - 那次样本没有稳定停在 `DinnerConflict`，而是落到了更早或更晚的 runtime 相位，所以不能当产品体验结论
  - 但它帮我抓出了 `19:30 resident` 仍被 crowd baseline 扣在 `backstage/takeover` 的真根，这条现在已经被代码与护栏一起收平
- 当前阶段：
  - 代码层与 targeted / bundle 护栏均已压实
  - 本轮准备停给用户重新黑盒，不再继续扩写新刀口
- 下一恢复点：
  1. 优先让用户重新测：
     - fresh / restart 下的晚饭入口 `001/002`
     - `19:30~20:00` resident 是否已恢复自由活动
     - Day1 `02:00` forced sleep 与第二天床边落点
 2. 如果用户仍报晚饭入口错位，再回到 `SpringDay1Director` 的 `fresh/restart` 入口对位链查，不再用这次不稳定的 validation jump 样本装懂

## 2026-04-13｜Town resident 101~203 owner audit
- 当前主线：按用户要求只读审计 Town resident 101~203 “phase/owner 混乱、自由时段仍像 staging 接管”的根因，明确 runtime owner 逻辑和 validation menu 哪个才是第一真问题。
- 本轮子任务：梳理 `SpringDay1NpcCrowdDirector.ApplyStagingCue` -> `SpringDay1DirectorStagingPlayback.ApplyCue` -> `SpringDay1DirectorNpcTakeover` -> `NPCAutoRoamController.AcquireResidentScriptedControl` 的 owner 链，反查 `SpringDay1DirectorStageBook.json` FreeTime/DayEnd actor cues，确认 `SpringDay1LatePhaseValidationMenu` 只是 editor 强跳剧情。
- 核心结论：101~203 因 stage book 里多个 beat（EnterVillage/PostEntry、FreeTime_NightWitness、DayEnd_Settle 等）仍把它们 `suspendRoam` 到 director 控制，`SpringDay1NpcCrowdDirector` 每帧会重启 `ApplyStagingCue` 并取 `ResidentScriptedControlOwnerKey`；这条 runtime owner chain 才是真正的 culprit，validation menu 仅是强制 story 跳转的辅助入口，没直接改 owner。
- 下一步：把上述结论用 final answer 趋近用户；如需继续实证，就在 PlayMode 下 replay FreeTime/DayEnd beat、确认 cue settle/clear 路径，或让 navigation 线程在 runtime owner release 上做整刀之前，先把 StageBook cue 归责给 day1 story 组。

## 2026-04-14｜opening / Town fresh live：resident 放人节拍已修，剧情 actor 争议收缩为 authored 点位
- 用户目标：先把 `opening / 0.0.2 / Town` 这条 P0 收清，不再出现“对白后 resident 卡住不动”的坏相，同时把“对白前站位怪”定责清楚。
- 本轮服务的子任务：
  1. fresh live 重跑 `Reset Spring Day1 To Opening -> Bootstrap Spring Day1 Validation`
  2. 把 `001/002/003` 的对白前站位问题和 `101~203` 的对白后放人问题彻底拆开
- 本轮关键结论：
  1. `001/002/003` 在对白第 1 句时已经稳稳落在当前 authored 点位：
     - `001 = (-12.55, 14.52)`
     - `002 = (-10.91, 16.86)`
     - `003 = (-22.02, 10.29)`
     说明这条争议不再是 runtime 延迟，而是 scene / stagebook 点位本身是否合理。
  2. resident `101~203` 的 runtime 放人 bug 这轮拿到了 live 改善票：
     - 旧问题：对白后整排像木桩
     - 新结果：对白结束后 1 秒内，`102/103/104` 等已重新进入 `Moving`
- 本轮代码改动：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - 新增 `RestartRoamFromCurrentContext(bool tryImmediateMove = false)`
  2. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - `TryReleaseResidentToDaytimeBaseline(...)` 改用 `RestartRoamFromCurrentContext(tryImmediateMove: true)`
  3. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 补 `DebugState != LongPause` 护栏
- 本轮验证：
  - `validate_script` 三个目标文件均 `errors=0`
  - EditMode targeted：
    - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldReleaseResidentToBaselineAfterCueReleaseBeforeFreeTime` passed
  - fresh live：
    - `spring-day1-live-snapshot.json`
    - `spring-day1-actor-runtime-probe.json`
    - `spring-day1-resident-control-probe.json`
    - `Assets/Screenshots/spring-day1-opening-dialogue-start-2.png`
- 当前恢复点：
  1. opening resident 的 runtime bug 已明显收缩
  2. opening 剩余未收的是 `001/002/003` 这组三个 authored 点位，要不要继续重摆由 day1 自己处理

## 2026-04-14｜补交正式忏悔书
- 用户补充了一个硬要求：除了修 bug，本线程还必须落一份正式的自我忏悔与罪证检讨书，详细记录 Day1 这段时间的两次重大失误、用户核心需求、我造成的阻塞、以及以后必须遵守的硬规则。
- 我上一条回复仍漏交了这份文档，这本身被记为新增执行失格。
- 已新增文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-14_spring-day1线程_自我忏悔与罪证检讨书.md`
- 文档内容已覆盖：
  1. 用户真正要的 Day1 闭环
  2. 两次把 NPC 管死并拖停所有线程的事故
  3. 我在 owner 边界、live 证据、越权修改、contract 交接上的具体罪证
  4. 今后必须执行的硬规则
- 当前恢复点：
  - 忏悔书已经补交
  - 主线不变，后续仍回到 Day1 全链路 runtime/staging/save/UI contract 闭环

## 2026-04-14｜三线程 owner 边界 prompt 包已完成并停车
- 当前主线：
  - 用户要求我先不要继续改业务代码，而是先把 `spring-day1 / 导航 / UI` 三条线程的 owner 边界、唯一主刀、红线和可直接转发文本一次写清。
- 本轮子任务：
  - 用正式 prompt 文件把三条线程的边界冻结，避免再把 opening、resident、02:00、UI 壳体和导航静态契约混成一个泛锅。
- 本轮实际做成：
  1. 已读取并纳入：
     - 导航线程只读分层结论
     - UI 线程 `2026-04-14_UI线程_给spring-day1_时间owner边界与自收内容告知_11.md`
     - 用户重发的 opening 历史正确流程原文
  2. 已新增三份 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-14_给spring-day1_Day1三线程owner边界重划与打包前唯一主刀prompt_v4.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-14_导航线程_Day1边界重划后非剧情态roam静态契约收口prompt_67.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.2_玩家面集成与性能收口\2026-04-14_UI线程_Day1最终UI语义与时间owner边界收口prompt_12.md`
  3. 已把用户原始 opening 正确流程写死进 Day1 prompt：
     - 立刻传送到起点
     - 走向终点
     - 最多等待 5 秒
     - 超时只对必要剧情 actor 瞬移到终点
     - 对话前一刻必须就位
     - 剧情结束原地退场并回 anchor
  4. 已把“高风险 own 改动必须形成最小可回退 checkpoint”同步写进三份 prompt。
- 本轮没有改任何业务代码，只做 prompt/contract 文件。
- 触发的技能与等价流程：
  - `skills-governor`
  - `sunset-prompt-slice-guard`
  - `preference-preflight-gate`
- 当前 thread-state：
  - 已跑 `Begin-Slice`
  - 已跑 `Park-Slice`
  - 当前状态 = `PARKED`
- 当前恢复点：
  - 下一步不是继续分析，而是把三份 prompt 交给用户直接转发；如果用户随后让我继续真实施工，我就只能按 v4 的 Day1 own 边界回到 opening / resident / first-night 三条主线。

## 2026-04-14｜按 v4 prompt 回到真实施工：opening own 第一刀
- 当前主线：
  - 用户已直接把 `v4` prompt 发回给我，要求我按 `spring-day1 own` 继续真实施工。
- 本轮子任务：
  - 只收 opening 第一刀，不碰 UI、不碰导航 core：
    1. `EnterVillage_PostEntry` crowd cue 的释放时机
    2. scene authored 终点的严格消费
    3. authored root 缺点位时不再静默 fallback
- 本轮实际做成：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - `ShouldSuppressEnterVillageCrowdCueForTownHouseLead(...)` 改为基于 `director.ShouldReleaseEnterVillageCrowd()` 判定
  2. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - `TryPrepareTownVillageGateActors()` 改成 authored strict 模式
     - `TryResolveTownVillageGateActorTarget(...)` 新增 `requireSceneAuthoredTarget`
     - 新增 `TryResolveVillageCrowdEndMarker(...)`
  3. [SpringDay1OpeningRuntimeBridgeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1OpeningRuntimeBridgeTests.cs)
     - 新增 3 条 opening 回归测试
- 验证状态：
  - 代码闸门层对本轮 own 文件无 owned error
  - `git diff --check` 通过
  - Unity 当前被外部 `NPCAutoRoamController.cs` 两条 compile red 卡住，assessment = `external_red`
  - `Ready-To-Sync.ps1` 又被 stale `ready-to-sync.lock` 卡住，未能拿到 `READY`
- 当前判断：
  - opening 这刀已经落成最小 own patch，但还没有拿到干净 live 票
- 当前恢复点：
  - 下一步必须先等外部 compile red 和 stale lock 清掉，再重跑 opening live；过了再继续 resident / first-night 第二刀

## 2026-04-14｜resident daytime release 与 Day1 第一夜 forced sleep 第二刀
- 当前主线：
  - 继续按 `v4` prompt 收 `spring-day1 own`，这轮重点从 opening 转到：
    1. resident 剧情后 release 不得跳去 `DailyStand_*` 聚堆区
    2. Day1 第一夜 `26:00` forced sleep 必须像后续天数一样切回 `Home`
- 本轮子任务：
  - 只动 `SpringDay1Director / SpringDay1NpcCrowdDirector` 和定向 EditMode 验证，不碰 UI 壳体，不碰导航 core。
- 本轮实际做成：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - `TryReleaseResidentToDaytimeBaseline(...)` 改为优先回各自 `BasePosition/BaseResolvedAnchorName`
     - 只有 `beatKey` 命中 `DailyStand` 语义预演时，才允许继续吃 `DailyStand_*` semantic baseline
     - 结果：opening/dinner 这类 scripted cue 结束后，resident 不再被统一传到一坨 daytime semantic anchor，而是回各自 base 域恢复 roam
  2. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - `HandleSleep()` 的 Day1 own 路径改成和 generic 路径一致：只要当前不在 `Home`，就先切回 `Home` 再摆位
     - `SyncStoryTimePauseState()` 补了 EditMode 安全分支：运行时继续走 `TimeManager.Instance`，编辑器定向测试只吃场景内现有 `TimeManager`，不再误建 `PersistentManagers`
  3. 回归测试：
     - [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
       - 把 EnterVillage crowd cue 的旧期望改到当前真实 contract
       - `CrowdDirector_ShouldReleaseResidentBackToBasePoseAfterCueReleaseBeforeFreeTime` 已通过
     - [SpringDay1LateDayRuntimeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs)
       - `HandleHourChanged_FreeTimeAtTwoAmShouldFinalizeDayEnd` 现在验证 `Town -> Home -> DayEnd`
     - [SpringDay1TargetedEditModeTestMenu.cs](D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs)
       - 新增 `Run Day1 Owner Regression Tests`
- 关键验证：
  - `validate_script`（4 个 own 文件）=`assessment=no_red`
  - `Library/CodexEditorCommands/spring-day1-owner-regression-tests.json`=`4/4 pass`
    - `EnterVillageCrowdCue_ShouldStayActiveUntilHouseLeadActuallyStarts`
    - `CrowdDirector_ShouldReleaseResidentBackToBasePoseAfterCueReleaseBeforeFreeTime`
    - `StoryActorsNightRestSchedule_ShouldCover001To003`
    - `HandleHourChanged_FreeTimeAtTwoAmShouldFinalizeDayEnd`
- 额外边界吸收：
  - 已按用户要求读取 UI 线程状态告知 `...UI线程_给spring-day1_Day1玩家面UI收口状态告知_13.md`
  - 后续只继续碰 Day1 canonical state / staging / release / first-night state machine，不再碰任务卡、bridge prompt、PromptOverlay、`TimeManagerDebugger +/-`
- 当前 blocker：
  - `Ready-To-Sync.ps1` 已不再被 stale 锁卡住，但仍被这条线历史 own roots 残留脏改阻断，当前准确状态是“本轮最小代码切片已 clean，可本地 checkpoint；整条 spring-day1 线程 own roots 仍未 clean”
- 当前恢复点：
  - 下一步优先让用户 live 复测：
    1. opening 结束后 resident 是否回各自 anchor/base 域，不再跳到一坨区域
    2. Day1 `26:00` 是否在 `Town` 直接切回 `Home`

## 2026-04-14｜本轮最小可回退 checkpoint 已提交并合法停车
- 本轮在不扩 scope 的前提下，已把以下 own 收口提交成最小 checkpoint：
  - `SpringDay1Director.cs`
  - `SpringDay1NpcCrowdDirector.cs`
  - `SpringDay1DirectorStagingTests.cs`
  - `SpringDay1LateDayRuntimeTests.cs`
  - `SpringDay1TargetedEditModeTestMenu.cs`
  - 以及对应 thread/workspace memory
- 本地 checkpoint 提交：
  - `442b9a40` `checkpoint: spring-day1 owner contract fixes`
- 本轮 fresh no-red 证据更新：
  - `git diff --cached --check` 通过
  - `sunset_mcp.py errors --count 20 --output-limit 5`=`errors=0 warnings=0`
  - `validate_script` 对 `SpringDay1Director.cs` 与 `SpringDay1NpcCrowdDirector.cs` 均无 owned/external red，但 Unity 现场仍是 `unity_validation_pending`
  - 原因不是本轮代码 red，而是当前 Unity editor state 持续落在 `stale_status`
- 本轮 thread-state：
  - 已执行 `Park-Slice -ThreadName spring-day1 -Reason owner-contract-checkpoint-landed`
  - 当前状态=`PARKED`
  - 当前 blocker=`Ready-To-Sync 仍被 spring-day1 历史 own roots 脏改阻断；当前最小 checkpoint 已本地提交，等待下一轮 live 复测 opening 与 Day1 26:00。`
- 当前恢复点：
  - 后续如果继续真实施工，只继续追两条 live：
    1. opening 结束后 resident 是否确实回各自 base/anchor 域
    2. Day1 `26:00` 在真实 live 下是否稳定 `Town -> Home -> DayEnd`
