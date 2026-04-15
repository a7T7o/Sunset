# 存档系统 - 线程活跃记忆

> 2026-04-10 起，旧线程母卷已归档到 [memory_1.md](D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/存档系统/memory_1.md)。本卷只保留当前线程角色和恢复点。

## 线程定位
- 线程名称：`存档系统`
- 线程作用：三场景持久化与门链收口入口

## 当前主线
- 当前唯一活跃阶段：
  - [4.0.0_三场景持久化与门链收口](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 当前恢复点
- 查旧根基底、3.7.x 旧索引、早期汇总时看 `memory_1.md`
- 当前 live 问题统一回到 `4.0.0`

## 2026-04-10 17:45 只读审核：读前档后剧情不回退的责任判断
- 当前主线目标没有变，仍是存档系统收口；本轮只是核清一条高风险边界：
  - “读前档后任务回退了，但剧情不再触发，最终卡进度”
- 用户这轮不是让我立刻改代码，而是要我判断：
  - 这锅是不是该先由存档线程背
  - `day1` 给出的责任判断和处理方向准不准确

### 本轮完成
1. 只读核对了：
   - `StoryProgressPersistenceService.ApplySnapshot()`
   - `SpringDay1Director.HasCompletedDialogueSequence()`
   - `SpringDay1Director.EnsurePhaseImpliedDialogueSequenceCompletion()`
   - `StoryProgressPersistenceService.ApplySpringDay1Progress()`
2. 已确认：
   - `StoryProgressPersistenceService` 读档时会老实恢复 `completedDialogueSequenceIds`
   - 但 `SpringDay1Director` 会按当前 `StoryPhase` 主动把若干剧情 `EnsureCompletedSequenceId(...)` 回写进 `DialogueManager`
3. 已额外确认一个次风险：
   - `ApplySpringDay1Progress()` 本身也会按 `currentPhase > 某阶段` 派生导演内部 `_xxxSequencePlayed` 一类布尔态

### 关键判断
- `day1` 的主判断基本正确：
  - 当前主责更像在 `day1` 语义层，不在存档线程本体
- 更准确的人话是：
  - 旧档把剧情完成列表读回来了
  - 但 `day1` 又按当前 `phase` 把它补成“已经演过”
  - 所以玩家看到的是“任务回退了，剧情却不让重播”
- 我这边不该先按“存档线程独立背锅”去大改
- 但后续也不能完全离场，因为 `day1` 改完后，还要由我复核“读档恢复态是否仍被导演私有态二次覆盖”

### 下一步建议
1. 先让 `day1` 主刀把“phase 推断已完成”改成只读判断，不再写回 `DialogueManager.completed`
2. 然后我这边再做一次窄复核：
   - `completedDialogueSequenceIds`
   - `ApplySpringDay1Progress()` 派生的导演私有态
   是否还会把旧档剧情态顶掉
3. 只有在这一步之后仍有残留覆盖，我才接最小存档配合刀

### 本轮读取依据
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryProgressPersistenceService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\4.0.0_三场景持久化与门链收口\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\存档系统\memory_0.md`

### thread-state
- 本轮只读分析
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-14 23:00 打包报错只读归因：主阻塞在 DayNightManager，不在 SaveManager

### 用户目标
- 用户贴出一组 build 日志，要求我只读找出“和我这条线有关的问题”、拆清责任面，并给出可执行解决方案。

### 已完成
- 已按 `skills-governor` 做前置核查，并手工补了 `sunset-startup-guard` 等价流程。
- 已核对：
  - `Assets/YYY_Scripts/Service/Rendering/DayNightManager.cs`
  - `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs`
  - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
- 已确认：
  - 这次打包失败的直接代码红错不是 `SaveManager`
  - 真正阻塞点是 `DayNightManager.Start()` 在 player 编译面调用 `EditorRefreshNow()`
  - `EditorRefreshNow()` 本体位于 `#if UNITY_EDITOR` 区块内，所以 Player build 触发 `CS0103`

### 关键判断
- 当前可明确落锅的 build blocker 是：
  - `Assets/YYY_Scripts/Service/Rendering/DayNightManager.cs:208`
- `CloudShadowManager` 的自动检测日志与 `DontSaveInEditor` assertion 更像编辑器预览链副作用，属于次级治理项，不是这次第一主因。
- 用户贴出的 `CS0414` warnings 当前都不是 build 阻塞项。

### 建议方案
- P0：
  - 先把 `DayNightManager.Start()` 内的 `EditorRefreshNow()` 调用包进 `#if UNITY_EDITOR`
  - 非编辑器 build 下只 `return`
- P1：
  - 后续再治理 `CloudShadowManager` 的 `[ExecuteAlways] + EditorUpdate + hideFlags` 断言噪音
- P2：
  - 未使用字段 warning 统一后置清理

### 恢复点
- 如果下一轮进入真实施工，先跑 `Begin-Slice`
- 最小切片只收：
  - `Assets/YYY_Scripts/Service/Rendering/DayNightManager.cs`
  - 如确有必要，再加 `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs`
- 不要把这轮 build fail 误扩成存档链全面返工

### thread-state
- 本轮只读分析
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-13 02:02 审计收尾：这条线该提交的已经提交，当前只收 memory 与停车
- 当前主线仍是存档系统收口；本轮子任务不是继续扩功能，而是把“我自己能合法提交的内容到底还有没有尾账”这件事彻底钉死。
- 我重新核了：
  - `git log --oneline --decorate -n 12`
  - `git branch -vv`
  - 当前 `thread-state`
  - 工作区 / 线程 memory 里前一轮留下的恢复点

### 本轮确认下来的事实
1. 两笔存档提交都已成立并在 `main` 历史里：
   - `0fa99813` `2026.04.13_存档系统_01`
   - `7fd6a606` `2026.04.13_存档系统_02`
2. 当前 `main` 和 `origin/main` 共同指向：
   - `8a3ad181` `2026.04.13_Codex规则落地_10`
3. 所以现在不存在“我这条线还有一笔没 push 的 save 提交”。
4. 当前真正还没收的是规范尾巴：
   - `skill-trigger-log`
   - docs slice 的正式 `Park-Slice`

### 当前判断
- 用户这轮要的“你能提交的都提交干净”，对存档线程来说已经基本完成：
  - 代码能提的先提成了 `0fa99813`
  - memory 能提的随后提成了 `7fd6a606`
- 现在再盲提第三笔业务提交，反而更容易越过 own 边界去吞 shared-root 里别人的 active 脏改。
- 所以这轮正确动作是：
  1. 只补 memory / skill 审计
  2. 只提这轮 memory checkpoint
  3. 立刻 `Park-Slice`

### thread-state
- 发现旧状态还停在 `READY_TO_SYNC`，不适合直接继续写 memory。
- 已用原来的 docs 白名单重新 `Begin-Slice -ForceReplace`，把当前切片拉回：
  - `收口 Data/Core 已提交事实并完成最终审计停车`
- 本轮下一步：
  - 补 `skill-trigger-log`
  - `Ready-To-Sync`
  - 提交 docs checkpoint
  - `Park-Slice`

## 2026-04-13 01:02 真实收口：Data/Core 三文件已按规范提交
- 当前主线仍是存档系统收口；本轮子任务是响应用户“把你自己能提交的都提交干净”，先把当前线程在 `Assets/YYY_Scripts/Data/Core` 同根下能合法独立归仓的 code tail 真正提交。
- 这轮没有再扩新功能，而是围绕 `SaveManager.cs / SaveDataDTOs.cs / ToolRuntimeUtility.cs` 做一次完整的 same-root hygiene + codeguard + sync 收口。

### 本轮完成
1. 先重跑 `Begin-Slice`，把旧的单文件白名单改成真实三文件白名单：
   - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
   - `Assets/YYY_Scripts/Data/Core/ToolRuntimeUtility.cs`
2. 处理通过 codeguard 所需的最小兼容补口：
   - `SaveManager.cs`
     - 把 `StoryProgressPersistenceService`、`CloudShadowManager.Export/ImportPersistentSaveData()`、`SaveActionToastOverlay` 改成反射可选调用
     - 目的不是撤销 save 语义，而是不把 `Story / Rendering / Save` 其他脏根强行并入这次提交
   - `ToolRuntimeUtility.cs`
     - 去掉对 `PlayerToolFeedbackService.ToolReplacementTone` 的硬依赖
     - 保留武器耐久/自动替换链
     - 工具损坏反馈退回当前 HEAD 已存在的旧接口
3. 重新过线：
   - `Ready-To-Sync` 通过
   - `sunset-git-safe-sync.ps1 -Action sync` 通过
4. 生成提交：
   - `0fa99813`
   - `2026.04.13_存档系统_01`

### 关键证据
- `sync` 时：
  - `own roots = Assets/YYY_Scripts/Data/Core`
  - `own roots remaining dirty 数量 = 0`
  - `代码闸门适用 = True`
  - `代码闸门通过 = True`
  - `代码闸门原因 = 已对 3 个 C# 文件完成 UTF-8、diff 和程序集级编译检查`
- `validate_script`：
  - `SaveManager.cs`：owned/external errors = `0/0`，停在 `unity_validation_pending`
  - `SaveDataDTOs.cs`：owned/external errors = `0/0`，停在 `unity_validation_pending`
  - `ToolRuntimeUtility.cs`：owned/external errors = `0/0`，停在 `unity_validation_pending`

### 当前判断
- 这轮我已经把当前最该先烂掉的 `Data/Core` 同根 code tail 真正收成了可回退 checkpoint。
- 但我没有把这件事包装成“存档线程整体 clean”：
  - shared root 上仍有大量其他存档相关历史脏根
  - 这轮只是把当前能独立合法提交的一坨先收住

### 恢复点
- 代码 checkpoint 已落库：`0fa99813`
- 当前紧接着要做的只有两件事：
  1. 把这条事实回写到工作区 memory / 线程 memory / skill 审计
  2. 完成本轮 `Park-Slice`

## 2026-04-11 02:50 讨论补口：后续三步的人话定义
- 当前主线仍是存档系统收口；本轮子任务是把“清 packaged live 日志、清 FreshStartBaseline 旧壳、做真实 build smoke”翻译给用户听，不进入真实施工。
- 解释口径固定为：
  1. 清日志 = 去掉打包后玩家不该看到的开发输出
  2. 清旧壳 = 把默认槽历史残留语义真正收成“原生开局入口”
  3. build smoke = 真打包、真验证，不再只靠 Editor 推断
- 后续若进入施工，就按这三步顺序执行。

### thread-state
- 本轮只读讨论
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-13 14:34 真实施工：SaveManager 已补 Day1 restore hygiene，off-scene world-state 维持审计结论
- 当前主线没有变，仍是存档系统收口；本轮子任务是把 `spring-day1` 已交出的整条 `Day1 restore contract` 真接到 `save/load/restart` 入口上，同时把“离场 scene world-state 是否入正式存档”做一次不返工的安全裁定。

### 本轮完成
1. `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - `ApplyLoadedSaveData(...)` / `ApplyNativeFreshRuntimeDefaults()` 现在统一先走 `ResetTransientRuntimeForRestore(...)`
   - 这条恢复卫生链会：
     - stop active dialogue
     - 关闭 `Package / Box / Workbench overlay`
     - 清掉背包 held / tooltip / confirm dialog
     - 清掉 `SpringDay1PromptOverlay` 外部 block 与 bridge prompt 残留
     - 隐藏 `InteractionHintOverlay / NpcWorldHintBubble / SpringDay1WorldHintBubble`
     - 隐藏玩家 / NPC bubble
     - 清掉 `Dialogue / SpringDay1Director / __manual__` pause source
     - 最后再统一 `ForceResetPlacementRuntime(...)`
   - 同轮删除了 `CheckPositionNextFrame(...)` 那类旧的 packaged 爆红诊断壳。
2. `Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
   - 新增 3 条 source-contract 护栏，钉住：
     - load / native fresh restart 共享同一条 restore hygiene
     - Day1 transient UI / modal / pause source 不得被删脱
     - `worldObjects` 当前仍只代表 loaded scene，不能冒充 off-scene formal save
3. `off-scene world-state`
   - 这轮没有落 DTO / bridge 代码
   - 维持的正式判断是：
     - 当前正式存档不能覆盖离场 scene 的 runtime world state
     - 最小正确合同仍是 `per-scene snapshot + scene 真加载后消费`
     - owner 仍在 `PersistentPlayerSceneBridge`，现在不建议先改正式存档代码

### 验证
- `validate_script Assets/YYY_Scripts/Data/Core/SaveManager.cs`
  - `owned_errors=0`
  - `external_errors=0`
  - `assessment=unity_validation_pending`
  - 卡在 `stale_status / codeguard timeout-downgraded`
- `validate_script Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
  - `owned_errors=0`
  - `external_errors=0`
  - `assessment=unity_validation_pending`
- `EditMode`：
  - `SaveManagerDay1RestoreContractTests`
  - `3/3 passed`
- `git diff --check -- SaveManager.cs SaveManagerDay1RestoreContractTests.cs`
  - clean
- `python scripts/sunset_mcp.py errors --count 20 --output-limit 5`
  - 当前仍有 `12` 条 external `missing script` error
  - 不是本轮 save own 新引入

### 当前判断
- 这轮已经把 `Day1 restore contract` 里最危险、最容易在 packaged live 里表现成“假冻结/假卡死”的那层入口残留清掉了。
- 但这不等于“整条 save 线已经无懈可击”：
  - `StoryProgressPersistenceService` 这条长期态链当前仍未被我这轮正式接盘入 same-root 提交
  - `off-scene world-state` 也还在合同层
- 所以下一恢复点如果继续，优先级应是：
  1. 结合 `day1` 最终 live 反馈，再决定是否要正式接盘 `StoryProgressPersistenceService` 这条链
  2. `off-scene world-state` 只有在 bridge owner 明确消费合同后再动

### thread-state
- 本轮已跑：
  - `Begin-Slice`
  - `Park-Slice`
- 未跑：
  - `Ready-To-Sync`
- 当前 live 状态：`PARKED`

## 2026-04-11 13:47 施工收口：本轮 own 已完成
- 当前主线仍是存档系统收口；本轮实际做的是两件事：
  1. 清 `FreshStartBaseline / 默认开局旧壳`
  2. 生成给典狱长的 `portfolio-review` 外发版体系 prompt

### 已完成
- `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
  - 删除默认开局旧 baseline 自动捕获/修复/退出写回壳
  - 默认开局不再映射任何落盘 baseline 文件
  - `__fresh_start_baseline__` 改成 legacy reserved name，只用于隐藏历史残留，不再参与现行默认槽语义
- 新增 prompt：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-11_给典狱长_portfolio-review外发版体系最小闭环统筹_01.md`

### 验证
- `validate_script SaveManager.cs`
  - `owned_errors=0`
  - `external_errors=0`
  - assessment=`unity_validation_pending`
  - 原因：Unity `stale_status`
- `errors`
  - `errors=0 warnings=0`

### 阶段判断
- 这轮 save own 已收完
- 外发版体系不再由 save 线程继续包办
- 后续如继续 save 线，只该回 packaged live 噪音/护栏/build smoke

### thread-state
- 已跑 `Begin-Slice`
- 已跑 `Park-Slice`
- 未跑 `Ready-To-Sync`
- 当前 live 状态：`PARKED`

## 2026-04-11 13:12 讨论裁定：当前最合理的责任拆分
- 当前主线不变，仍是存档系统收口；本轮子任务是判断用户提出的拆法是否合理。
- 裁定：合理，而且优于我上一轮自己全包的顺序。

### 责任拆分
1. 存档线程先做：
   - `FreshStartBaseline` / 默认开局旧壳清扫
2. 典狱长后续统筹：
   - `portfolio-review` 外发版体系
   - 打包前清理与候选包流程
3. 最终打包 smoke：
   - 仍需带存档验收项，但可以并入外发线的最终复检

### 原因
- 默认开局旧壳是 save 语义 owner 面
- 外发版体系已超出 save 范围，属于更大的发布治理问题

### thread-state
- 本轮只读讨论
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-11 02:42 只读总审：当前存档线的真实阶段判断
- 当前主线仍是存档系统收口；本轮子任务是只读确认“还有没有没闭环、会不会有性能炸点、是否已经优化到可以打包”。
- 本轮没有进入真实施工，继续保持 `PARKED`。

### fresh 检查
1. `python .\scripts\sunset_mcp.py errors --count 20 --output-limit 20`
   - 当前 `errors=0 warnings=0`
2. `python .\scripts\sunset_mcp.py manage_script validate --name PackageSaveSettingsPanel --path Assets/YYY_Scripts/UI/Save --level standard`
   - clean
3. `python .\scripts\sunset_mcp.py manage_script validate --name SaveManager --path Assets/YYY_Scripts/Data/Core --level standard`
   - warning 3，但无 fresh compile red
4. 重新核对了 save 主链文件、Chest authoring、debug 退役壳、layout guards 与 story/workbench 相关测试。

### 当前结论
- 存档主链现在可用，项目当前 fresh 编译也已经 clean。
- 但还不能签成“完全没问题”。
- 更准确的人话：
  - 主功能可跑
  - 打包方向正确
  - 还留着一层工程尾账与 packaged live 噪音

### 关键风险点
- 没看到 demo 规模下会立刻爆炸的性能炸弹。
- 但有明确的 load/save 峰值风险：
  - `SaveManager` 主线程同步 JSON 序列化与写盘
  - `prettyPrintJson=true`
  - save 页刷新时逐槽读摘要
  - `PersistentPlayerSceneBridge` 重绑场景时多次全场景查找
  - `PersistentObjectRegistry` 读档时清理 `StoneDebris` 的全场景扫描
- 另外 `FreshStartBaseline` 相关旧壳仍未真正整理掉。

### 打包口径
- 现在可以进入“试打包 / build smoke”阶段。
- 但还不该宣称“已经完成最终出包级收口”，因为：
  1. 本轮没有真实 build smoke 证据
  2. save 页无专门自动化护栏
  3. packaged live 还有无条件日志噪音未清

### 下一步若继续施工
1. 清 packaged live 无条件日志
2. 清默认开局/FreshStartBaseline 旧壳
3. 给 save 页补最小测试护栏
4. 做一次真实 build smoke 再决定是否签字

### thread-state
- 本轮只读分析
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-11 00:00 只读补正：这条线已经比刚才更靠前，但还没全链到位
- 当前主线不变，仍是存档系统收口；这轮子任务是补正我上一条结论里“没有专项测试”的部分。
- 这轮依旧没有进真实施工，只对最新落地的 day1 修复做补核。

### 本轮完成
1. 核对了 `StoryProgressPersistenceService.ApplySpringDay1Progress()` 当前晚段赋值逻辑
2. 核对了 `StoryProgressPersistenceServiceTests.cs` 新增测试
3. 跑了 `StoryProgressPersistenceServiceTests.cs` 的最新 `validate_script`

### 补正后的结论
- 刚才那句“还没有专门回归测试”现在要改成更准确的版本：
  - `late-day` 已经有专门回归测试了
  - 对应代码也已经把 `dinner/reminder/free-time-intro` 这条 phase 偷推链收掉了
- 具体表现：
  - `_dinnerSequencePlayed` 现在只认 `hasDinnerSequence`
  - `_returnSequencePlayed` 现在只认 `hasReminderSequence`
  - `_freeTimeIntroCompleted / _freeTimeIntroQueued` 也已不再靠 `currentPhase >= DayEnd` 偷推
  - `Load_DoesNotPromoteLateDayPrivateFlagsFromPhaseAlone()` 会验证这 4 个晚段私有态不会因为 phase 偏后而残留推进

### 为什么我仍然不判“完成到位”
- 因为这次补强主要覆盖的是晚段：
  - dinner
  - reminder
  - free-time-intro
- 但更早一些的导演私有态仍然还在 phase 派生：
  - `_villageGateSequencePlayed`
  - `_houseArrivalSequencePlayed`
  - `_healingSequencePlayed`
  - `_workbenchOpened`
  - `_workbenchSequencePlayed`
- 目前还没有看到针对这些更早阶段的同类专项回归测试
- 所以最准确的人话是：
  - 这条 bug 现在已经不是“只修了一半”
  - 晚段部分已经被明确补强
  - 但整条线还没到“全段位都能放心验收”的程度

### 验证结果
- `validate_script Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
  - `owned_errors=0 / external_errors=0 / warnings=0 / native_validation=clean`
  - assessment=`unity_validation_pending`，仅因 Unity `stale_status`

### 当前建议
- 如果要把这条线真正收成“完成到位”，下一步最该补的是：
  - 早中段私有态的专项回归测试
  - 也就是验证更早档读回后：
    - `village-gate / house-arrival / healing / workbench` 不会因 phase 偷推进

### thread-state
- 本轮只读分析
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-10 23:56 只读再确认：这条 bug 还不能算“完成到位”
- 当前主线仍是存档系统收口；这轮子任务是用户继续追问“你现在再看看，是否完成到位”。
- 这轮依旧没有进真实施工，只做最后一轮只读确认。

### 本轮完成
1. 再次核对：
   - 相关代码现状
   - 相关测试覆盖
   - 当前工作树状态
   - 当前 Unity console 外部红面
2. 输出更明确的验收口径：
   - 不是没修
   - 但也还没到“彻底完成到位”

### 最终判断
- 现在仍然不能判“完成到位”
- 理由分 4 层：
  1. `HasCompletedDialogueSequence()` 的主根因修复已存在
  2. `ApplySpringDay1Progress()` 仍会按 phase 派生导演私有布尔态，留有边角风险
  3. 没有针对“读更早档后导演私有态不被偷推”的专门回归测试
  4. 相关文件工作树还没收口到干净状态

### fresh 证据
- 代码：
  - `SpringDay1Director.HasCompletedDialogueSequence()` 仍直接读取 completed 集合
  - `StoryProgressPersistenceService.ApplySpringDay1Progress()` 仍有多处 `hasSequence || currentPhase > StoryPhase.X`
- 测试：
  - `StoryProgressPersistenceServiceTests` 只覆盖 completed 列表恢复和 stale 替换
  - 还没覆盖“早档读回后导演私有态不能继续超前”
- 工作树：
  - `SpringDay1Director.cs`、`SpringDay1DialogueProgressionTests.cs` = 已修改
  - `StoryProgressPersistenceService.cs`、`StoryProgressPersistenceServiceTests.cs` = 未纳入版本控制状态
- Unity console：
  - 当前 3 条错误来自 `Assets/YYY_Scripts/Service/Rendering/DayNightOverlay.cs`
  - 不是这条修复线自己的 owned red

### 当前建议
- 如果要把这条问题真正收成“完成到位”，下一步最该做的不是我现在接存档大改
- 而是补一条专门回归测试：
  - 读一个更早的档后
  - `completedDialogueSequenceIds` 和导演私有 `_xxxSequencePlayed` 都不能被 `phase` 偷推

### thread-state
- 本轮只读分析
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-10 17:53 只读复查：剧情回退 bug 现在到底修到哪了
- 当前主线仍是存档系统收口；本轮子任务是按用户要求重新检查最新项目现场，不沿用上一轮旧判断。
- 这轮没有进真实施工，只做 fresh 代码与测试复查。

### 本轮完成
1. 重新核对了最新版本的：
   - `SpringDay1Director.HasCompletedDialogueSequence()`
   - `StoryProgressPersistenceService.ApplySnapshot()`
   - `StoryProgressPersistenceService.ApplySpringDay1Progress()`
   - `SpringDay1DialogueProgressionTests.cs`
   - `StoryProgressPersistenceServiceTests.cs`
2. 跑了两份目标脚本的最新 `validate_script`
3. 检查了相关工作树状态

### 新结论
- 和上一轮不同，现在主根因代码确实已经动过：
  - `HasCompletedDialogueSequence()` 不再调用 `EnsurePhaseImpliedDialogueSequenceCompletion()`
  - 现在只直接读取 `DialogueManager.HasCompletedSequence(...)`
- 所以那条“phase 自动回写 completed 列表”的最直接 bug 链，代码面上已经被拿掉

### 还没完全闭环的原因
- `StoryProgressPersistenceService.ApplySpringDay1Progress()` 仍会按 `currentPhase > 某阶段` 派生导演私有 `_xxxSequencePlayed` 状态
- 这不会直接重写 completed 列表，但仍可能让导演内部推进态比旧档偏后
- 当前测试没有专门验证：
  - “读一个更早的档后，不允许再被 phase 或导演私有态偷偷推进”
- 所以目前更准确的结论是：
  - 主根因代码大概率已修
  - 但完整闭环和回归护栏还不够

### 验证结果
- `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `owned_errors=0`
  - assessment=`external_red`
  - 外部 console 有 4 条缺脚本错误和若干 warning，不是这条修复线单独新引入
- `validate_script Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
  - `owned_errors=0`
  - assessment=`external_red`
  - 同样被当前 Unity 外部 red 污染
- 当前工作树里相关文件仍未收口到干净状态：
  - `SpringDay1Director.cs`、`SpringDay1DialogueProgressionTests.cs` 为已修改
  - `StoryProgressPersistenceService.cs`、`StoryProgressPersistenceServiceTests.cs` 当前显示为未纳入版本控制现场

### 当前判断
- 不能再说“完全没修”
- 也不能说“已经彻底修完并覆盖完毕”
- 当前最诚实的口径：
  - `day1` 已经落下主根因代码修复
  - 但还差一个针对“早档读回不被重新推进”的专门回归测试
  - 还差一次针对导演私有态派生风险的再核

### thread-state
- 本轮只读分析
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-11 00:11 只读再校正：这条 bug 现在更接近“基本修住”，但还不是我愿意直接签字的最终态
- 当前主线没有变，仍是存档系统收口；本轮子任务是按用户追问重新核“现在到底算不算完成到位”，并纠正我上一轮可能说重了的判断。
- 本轮仍然没有进入真实施工，只做 fresh 源码、测试、git 现场和 Unity CLI 验证。

### 本轮完成
1. 重新核对了：
   - `SpringDay1Director.HasCompletedDialogueSequence()`
   - `ShouldQueueDialogueSequence(...)`
   - `TryRecoverConsumedSequenceProgression(...)`
   - `StoryProgressPersistenceService.ApplySpringDay1Progress(...)`
   - `StoryProgressPersistenceServiceTests.cs`
2. 重新检查了：
   - 相关 phase 迁移链
   - 当前 `git status`
   - 当前 `errors`
   - `SpringDay1Director.cs / StoryProgressPersistenceService.cs / StoryProgressPersistenceServiceTests.cs` 的最新 `validate_script`

### 纠偏后的判断
- 我要主动修正上一轮一个偏重口径：
  - 早中段 `_villageGateSequencePlayed / _houseArrivalSequencePlayed / _healingSequencePlayed / _workbenchOpened / _workbenchSequencePlayed` 的 `phase > X` 派生，不应直接按“和晚段同类的未闭环 bug 风险”去说。
- 更准确的人话是：
  - 主根因 `phase => 自动把 completed dialogue 补成 consumed` 已经拆掉
  - 晚段 `dinner/reminder/free-time-intro` 的真实偷推链也已经被修掉，并有专项 load 回归测试
  - 早中段这些 phase 派生，目前从源码语义看更像“后续必经门的恢复对齐”，不是已经再次看到的同类现行 bug

### 为什么我仍不直接说“彻底完成到位”
- 不是因为又发现了新的明确逻辑 bug
- 而是还差最后两层签字条件：
  1. 早中段没有和晚段同等级的专门 load 回归测试
  2. 相关文件工作树仍未收口到干净提交态

### 最新验证
- `python .\\scripts\\sunset_mcp.py errors --count 20 --output-limit 5`
  - `errors=0 warnings=0`
  - 但最新输出里有 1 条不计入 error_count 的 `Assert`，来源是 `Assets/Editor/DayNightManagerEditor.cs:130`，不是这条存档/day1 问题自己的责任面
- `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - assessment=`no_red`
  - owned/external error 均为 0
- `validate_script Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
  - assessment=`unity_validation_pending`
  - native validation clean，owned/external error 均为 0，卡在 `stale_status`
- `validate_script Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
  - assessment=`unity_validation_pending`
  - native validation clean，owned/external error 均为 0，卡在 `stale_status`
- 当前工作树仍是：
  - `M SpringDay1Director.cs`
  - `M SpringDay1DialogueProgressionTests.cs`
  - `?? StoryProgressPersistenceService.cs`
  - `?? StoryProgressPersistenceServiceTests.cs`

### 当前最准确口径
- 如果只问“主 bug 还在不在”：
  - 现在看，主 bug 已基本修住。
- 如果问“我能不能现在就替这条线签成 100% 无懈可击”：
  - 还不能。
- 当前阶段应描述为：
  - 功能面基本闭环
  - 版本与证据层还差最后一小步

### thread-state
- 本轮只读分析
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`
