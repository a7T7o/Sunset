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

## 2026-04-17 00:30 真实施工续记：第二簇验证收完，Primary 农地离场丢状态完成只读归因
- 当前主线没变，仍是存档系统收口；本轮继续服务“重开/剧情/Home 后背包、toolbar、箱子、sort、input 混合坏相”这条主线。
- 本轮子任务分两段：
  1. 把上一刀的第二簇验证补完
  2. 按用户新补反馈，只读彻查 `Primary` 农地/浇水状态离场后消失

### 本轮已完成
1. 脚本/测试验证：
   - `PersistentPlayerSceneBridge.cs`：`validate_script => errors=0 warnings=2`
   - `InventoryRuntimeSelectionContractTests.cs`：`errors=0 warnings=0`
   - `SaveManagerDay1RestoreContractTests.cs`：`errors=0 warnings=0`
   - `WorkbenchInventoryRefreshContractTests.cs`：`errors=0 warnings=0`
2. `git diff --check` 针对本轮 own 文件 clean：
   - 没有 fresh 空白/格式 red
   - 只有 CRLF/LF 提示
3. 发现 `toolbar 固定槽位 4/8 丢图标` 与我当前 slice 撞到 `PersistentPlayerSceneBridge.cs`
   - `农田交互修复V3` 的 active-thread state 当前把：
     - `ToolbarUI.cs`
     - `ToolbarSlotUI.cs`
     - `PersistentPlayerSceneBridge.cs`
     都挂成 owned
   - 我判断不该硬打断它整条线，但必须收边界
   - 已生成 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-17_给农田交互修复V3_toolbar图标与scene-rebind边界收口prompt_01.md`
   - 口径：
     - `ToolbarUI / ToolbarSlotUI` 继续归它
     - `PersistentPlayerSceneBridge / scene-rebind / save continuity` 归我
4. `Primary` 农地问题只读归因已经钉实：
   - `SaveManager.CollectFullSaveData()` 明确只收当前 loaded scene 的 `worldObjects`
   - `PersistentPlayerSceneBridge.QueueSceneEntry()` 会在离场前抓 runtime continuity
   - 但 `CaptureSceneWorldRuntimeState()` 当前只抓：
     - `WorldItemPickup`
     - `TreeController`
     - `StoneController`
   - `FarmTileManager / CropController` 虽然是 `IPersistentObject`，却没有进入离场 continuity snapshot
   - 所以用户在 `Primary` 新做的耕地/浇水/作物运行态，离场后没有桥接缓存，回场景时就按原生空场重新起来

### 当前判断
- 我这边前一刀的四项修补，现在可以说：
  - 代码层修补已落地
  - 静态/脚本级验证成立
  - 还没有做 live/manual 终验，所以不能写成“玩家侧已彻底验完”
- 用户新补的 `Primary` 农地问题，不是显示链小毛病：
  - 是 off-scene world-state 合同缺口
  - 甚至会外溢到“离开 Primary 后在别的场景存档也拿不到农地状态”

### 证据锚点
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
  - `694-709`, `956-958`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PersistentPlayerSceneBridge.cs`
  - `165-168`, `566-599`, `722-740`, `906-913`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmTileManager.cs`
  - `17`, `127`, `1009`, `1057`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\CropController.cs`
  - `33`, `188`, `898`, `932`

### 未完成 / blocker
- `read_console` 本轮未取到 fresh 结果：Unity session 返回 `not ready for read_console`
- `Primary` 农地问题按用户要求本轮没有修，只做了独立彻查
- 当前这轮准备停在汇报，不做 sync

### thread-state
- 本轮已跑：
  - `Begin-Slice`
  - `Park-Slice`
- 尚未跑：
  - `Ready-To-Sync`
- 当前 live 状态：`PARKED`

## 2026-04-17 02:03 只读复审与prompt路由更正：0417 已整改完成，外发改用纯文本
- 用户贴回了 `0417.md` 维护者的整改结果，并要求我重新按“已完成态”复审，不再继续沿用上一轮“补充整改 prompt”的判断。
- 本轮只读复审后的结论：
  1. `0417.md` 已经把默认档语义纠正到 `Town 原生入口 / 进村开局`
  2. 已把 `F5 / F9` 纠正为：
     - `F5 已停用`
     - `F9 默认档读取`
  3. 已补进 `thread-state / 线程记忆 / skill-trigger-log`
  4. 已吃进 `PersistentPlayerSceneBridge.cs` 与 `农田交互修复V3` 的禁并写边界
  5. `P0-B` 已改成“第一轮代码修补 + 第二簇验证已过，live / packaged 待测”
  6. `当前建议的下一刀` 已切到：
     - `P0-C｜world-state 主链`
- 因此，上一步我临时创建的 F5/F9 语义补充 prompt 文件已经不再需要：
  - 已删除 `2026-04-17_给0417维护者_F5F9与原生开局语义补充prompt_01.md`
- 用户最新裁定还要求：
  - 后续如果要给对方续工，不再写 tracked prompt 文档
  - 直接给纯文本 prompt

### 当前判断
- 现在已经不需要再给对方发“请整改 0417”的 prompt。
- 更合适的下一条外发内容，应该是：
  - 直接让对方按 `0417.md` 现状态进入 `P0-C｜world-state 主链`

### thread-state
- 本轮只读复审与清理 prompt 文件
- 已跑：
  - `Begin-Slice`
  - `Park-Slice`
- 未跑：
  - `Ready-To-Sync`
- 当前 live 状态：`PARKED`

## 2026-04-16 23:16 只读总排查：重开/剧情/Home 后背包与 toolbar 混合坏相不是单点 bug

### 用户目标
- 用户要求我彻查“重新开始游戏、进入剧情、进出 Home 后，背包显示错、toolbar 选不中/不能用、工具物品都用不了、背包 sort 与显示对不上、箱子 sort 行为怪”的整条链，并说明正确逻辑与当前问题。

### 本轮完成
- 使用 `skills-governor` 做前置核查，并手工执行了 `sunset-startup-guard`、`sunset-workspace-router`、`user-readable-progress-report`、`delivery-self-review-gate` 的等价流程。
- 跑了 `sunset-rapid-incident-triage` 的快速探针。
- 并行做了两条只读探路：
  1. 背包 / toolbar / 箱子 / sort / UI 绑定链
  2. restart / 剧情 / Home / scene rebind / input 恢复链
- 核对了关键文件：
  - `SaveManager.cs`
  - `PersistentPlayerSceneBridge.cs`
  - `GameInputManager.cs`
  - `HotbarSelectionService.cs`
  - `InventoryPanelUI.cs`
  - `InventorySlotUI.cs`
  - `InventoryInteractionManager.cs`
  - `BoxPanelUI.cs`
  - `InventorySortService.cs`
  - `PlayerInventoryData.cs`
  - `SceneTransitionTrigger2D.cs`
  - `Town.unity`

### 关键结论
- 这次不是一个单点 bug，而是两组问题叠加：
  1. **显性逻辑裂缝**
     - `HotbarSelectionService.SelectInventoryIndex()` 对 `>=12` 背包槽只改 `selectedInventoryIndex`，不发 `OnSelectedChanged`，也不重装备
     - `InventorySlotUI.ResolveSelectionState()` 在箱子打开时，把 `BoxPanelUI` 与 `InventoryPanelUI` 两套本地选中态做了 `OR`
     - `InventorySortService.SortInventory()` 会排 `0..35`，而 `PlayerInventoryData.Sort()` / `InventoryService.Sort()` 只排 `12..35`
  2. **高风险 runtime rebind 面**
     - `Town.unity` 里 `GameInputManager` 的核心序列化引用本来就是空
     - 整条输入 / 背包 / UI / toolbar 能否正常工作，高度依赖 `PersistentPlayerSceneBridge.RebindScene`

### 我现在最认可的解释
- “显示不对 + 不能用 + sort 看起来对不上”
  - 主要不是单纯存档没读出来
  - 更像 `真实运行态` 与 `UI 本地状态` 分叉了
- “重开 / 进剧情 / Home 往返后一起变怪”
  - 更像 `scene rebind / input reset` 把这套分叉放大

### 当前最像的主问题分组
1. **真选中语义不唯一**
   - 真运行态依赖 `HotbarSelectionService.selectedIndex`
   - 但背包页与箱子页又各自保存本地 `_selectedInventoryIndex`
2. **sort 规则不唯一**
   - 有的路径动 hotbar
   - 有的路径不动 hotbar
3. **恢复 contract 不完整**
   - 现有测试只盯“恢复前清壳”
   - 没盯“restart -> story -> Home -> toolbar/inventory/input 仍一致”

### 额外确认
- `Town.unity` 中 `GameInputManager` 的 `playerToolController / inventory / hotbarSelection / packageTabs` 都是空引用，说明第二条判断不是猜测。
- `SaveManagerDay1RestoreContractTests` 目前只覆盖恢复前清旧态，不覆盖整条背包/toolbar/input 一致性。

### 下一刀建议
- 如果下一轮进入真实施工，不要泛修整条存档线。
- 应拆成两个硬切片：
  1. `HotbarSelection + InventoryPanel + BoxPanel + InventorySlotUI + Sort` 统一真语义
  2. `restart/Home/剧情` 的 scene rebind / input / toolbar / inventory integration contract

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

## 2026-04-17 只读续记｜跨场景箱子/农地/背包整链坏相总排查

### 用户目标
- 用户明确要求停止零散补 bug，改为彻底查清：
  - 为什么跨场景后箱子、农地、背包、toolbar 会出现“大面积瘫痪感”
  - 为什么换场景、重开、过天之后会出现箱子复活、农地回档、显示和交互一起变怪
- 用户要求这轮只做人话汇报，不先讲代码修法。

### 本轮完成
1. 继续保持只读分析，没有进入真实施工。
2. 收到箱子链只读子智能体结论：
   - 切场 continuity 不覆盖箱子
   - 箱子重新进场会回到作者预设内容
3. 亲自补核了真实磁盘存档：
   - `slot3.json`（Home 档）可见 Home 箱子对象
   - `slot1.json / slot6.json`（Town 档）看得到 `FarmTileManager`，但看不到 off-scene 的 Chest / Crop
   - 说明现在的正式存档覆盖范围并不统一
4. 继续补核 UI / rebind 链：
   - `PersistentPlayerSceneBridge` 会在切场后重绑背包、装备、热栏选择、PackagePanel、InventoryInteractionManager、BoxPanel
   - 持久 UI 壳体跨场景活着，但 world-state 本体并没有统一连续

### 当前最核心判断
- 现在最大的问题不是“Chest Save/Load 坏了一个点”，而是三套语义没有统一：
  1. 正式普通存档
  2. 切场 continuity
  3. 默认重开 / fresh start
- 这三套覆盖范围不一致，才会让玩家体感变成：
  - 当前场景里像是对的
  - 一换场景就像回档
  - 再打开 UI 又像另一套状态

### 已证实的问题
1. 切场 continuity 只覆盖掉落物 / 树 / 石头，不覆盖箱子 / 农地 / 作物。
2. 箱子内容会在重新进场后重新吃作者预设，表现成“箱子复活”。
3. 农地 / 浇水 / 作物的正式 Save/Load 虽存在，但切场 continuity 不接它们，所以离场回来会像没发生。
4. 正式普通存档当前仍主要保存当前已加载场景的世界对象；off-scene 内容不是稳定统一入盘。
5. 持久 UI 壳体跨场景保留，但每次切场又要重新找 runtime 引用；当 world-state 本体不连续时，显示、选中、手持、交互就会一起乱。
6. 新补钉实：
   - 切场只保存 `hotbar` 高亮，不保存真正的“背包选槽来源”
   - 重开 / 切场时又会把真实背包选槽压回 `hotbar`
   - 剧情开始时只关箱子，不关背包，但输入门控会把“任何面板开着”都视为禁止移动 / 切栏 / 用工具
   - 所以会出现“看起来还选着，但工具不能用、种子不认、toolbar 像锁死”的坏相

### 高概率问题
1. `ToolbarUI` 失活再激活后，可能没有重新订阅库存 / 选中广播，导致看起来不再跟刷新。
2. 背包面板与箱子面板每次重新打开，都会把视觉选中重置成跟随 hotbar，而不是恢复上次真实选槽。
3. UI 重绑大量依赖 `FindFirstObjectByType` 静默兜底，所以一旦链路没完全接好，最容易出现“界面还活着，但绑错对象”的状态。

### 这轮没做什么
- 没改业务代码
- 没做 Unity live 复现
- 没 claim 修好
- 当前阶段是“问题总图已钉死，等待下一刀真实修复切片”

### 恢复点
- 后续若进入真实施工，优先方向不该再是追单个表层现象，而应回到：
  - world-state 跨场景 continuity 合同统一
  - 正式存档对离场场景的覆盖策略统一
  - 持久 UI rebind 与运行态事实源统一

## 2026-04-17 续记｜0417 总控活文档已落地

### 用户目标
- 用户要求不要先继续零散开刀，而是先做一份新的长语义 + 长任务文档，命名简化为 `0417.md`。
- 这份文档要能承担下一轮主控职责：让我后续可以围绕它持续领取任务、持续维护、持续验证，而不是重新散落到聊天与 memory 里。

### 本轮完成
1. 新建：
   - [0417.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/0417.md)
2. 已把以下内容收进文档：
   - 文档定位
   - 维护规则
   - 完成定义
   - 用户裁定冻结
   - 存档语义梳理
   - 已证实 / 高概率问题树
   - 需求拆分
   - 边界与不越权清单
   - 执行顺序
   - 测试矩阵
   - 滚动任务区
   - 迭代记录
3. 已明确下一轮默认起手顺序：
   - `P0-B｜玩家可测试性恢复`
   - `P0-C｜world-state 主链`
   - `P0-D｜Day1 恢复 contract`

### 当前最核心判断
- `0417.md` 不是摘要，而是这条线的新“总控活文档”。
- 后续如果继续这条线，默认应该先读 `0417.md`，再领取切片施工，而不是重新从聊天里拼语义。

### 当前恢复点
- 当前首要恢复点已更新为：
  - [0417.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/0417.md)
- 需要看技术细证据时，再回：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-17 续记｜0417 一致性整改

### 用户目标
- 用户贴出对 `0417.md` 的静态一致性审视，要求我客观判断：哪些批评成立，成立就直接整改，不要停在讨论层。

### 本轮完成
1. 确认这轮审视是有效纠偏，不是吹毛求疵。
2. 已直接整改 `0417.md` 的 6 类问题：
   - 默认档语义改为 `Town 原生入口 / 进村开局`
   - `F5` / `F9` 口径改为：
     - `F5 已停用`
     - `F9 默认档读取`
   - 维护规则补入 `thread-state`、线程记忆、skill 审计
   - 跨线程边界补入：
     - `PersistentPlayerSceneBridge.cs` 不再与 `农田交互修复V3` 并写
   - `P0-B` 改成“第一轮代码修补已落地 + 第二簇验证已完成 + live/package 待测”
   - 测试矩阵改成 `代码层 / live / packaged` 三层状态板
3. 顺手修复了任务编号重复，避免后续继续领取任务时混乱。

### 当前最核心判断
- 这轮批评大体成立。
- `0417.md` 原先最大的缺点不是方向错，而是它还停留在“初始化版主控稿”。
- 经过这轮整改后，它已经可以作为后续继续领活的控制板使用。

### 当前恢复点
- 下一轮如果继续真实施工，仍然先看：
  - [0417.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/0417.md)
- 但要按整改后的最新口径理解：
  1. 默认档 = `Town 原生入口 / 进村开局`
  2. `F5 已停用`
  3. `F9` = 默认档读取
4. 下一刀默认起手 = `P0-C｜world-state 主链`

## 2026-04-17 续记｜已生成“以 0417 开工”的自用引导 prompt

### 用户目标
- 用户本轮不要求我继续施工，只要求我基于当前最新现场，写一份可以直接发给“下一轮的我”的引导 prompt。

### 本轮完成
1. 已把 prompt 口径固定为：
   - 先读 `0417.md`
   - 当前唯一主刀 = `P0-C｜world-state 主链`
   - 只收：
     - off-scene world-state 正式合同
     - 箱子跨场景 / 跨天闭环
     - 农地 / 浇水 / 作物跨场景闭环
   - 不漂回：
     - `P0-B`
     - `F5/F9`
     - Save UI
     - Day1 own staging
     - packaged smoke
2. 这轮没有继续代码施工，也没有改业务语义，只产出可直接复制的下一轮开工话术。

### thread-state
- 本轮已跑：
  - `Begin-Slice`
- 预计收尾：
  - `Park-Slice`
- 当前 live 状态：准备从 `ACTIVE` 回到 `PARKED`

### thread-state
- 本轮已跑：
  - `Begin-Slice`
- 预计收尾：
  - `Park-Slice`
- 当前 live 状态：准备从 `ACTIVE` 回到 `PARKED`

### thread-state
- 本轮只读分析
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-17 续记｜0417-P0-C world-state 主链第一刀已施工

### 用户目标
- 用户明确要求不要再按“一轮一讨论”的方式推进，而是把 `0417` 当唯一主控板，从当前最高优先项开始连续施工。
- 当前唯一主刀固定为：
  - `P0-C｜world-state 主链`
- 这轮只收：
  1. `off-scene world-state` 正式合同
  2. 箱子跨场景 / 跨天闭环的第一刀
  3. 农地 / 浇水 / 作物跨场景闭环的第一刀

### 本轮完成
1. 继续沿用已登记的 slice：
   - `0417-P0-C world-state主链连续施工`
2. 在 `SaveDataDTOs.cs` 里新增：
   - `SceneWorldSnapshotSaveData`
   - `GameSaveData.offSceneWorldSnapshots`
3. 在 `PersistentPlayerSceneBridge.cs` 里新增 / 落实：
   - `ExportOffSceneWorldSnapshotsForSave()`
   - `ImportOffSceneWorldSnapshotsFromSave(...)`
   - `SuppressSceneWorldRestoreForScene(...)`
   - `CancelSuppressedSceneWorldRestore(...)`
   - continuity 捕获 / 恢复纳入：
     - `ChestController`
     - `FarmTileManager`
     - `CropController`
   - world-state 恢复排序：
     - `FarmTileManager -> Crop -> Chest -> Drop/Tree/Stone`
4. 在 `SaveManager.cs` 里接上：
   - 保存导出 off-scene snapshots
   - 读档导入 off-scene snapshots
   - 跨场景读档前先抑制目标 scene continuity restore
5. 扩充 `SaveManagerDay1RestoreContractTests.cs`，新增 world-state formal contract 护栏
6. 回写：
   - `.kiro/specs/存档系统/0417.md`
   - `.kiro/specs/存档系统/memory.md`
   - `.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md`

### 验证
- `SaveManagerDay1RestoreContractTests`
  - `passed=6 failed=0`
- `validate_script Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
  - `owned_errors=0`
  - `external_errors=0`
  - `assessment=unity_validation_pending`
- `validate_script Assets/YYY_Scripts/Data/Core/SaveManager.cs`
  - `owned_errors=0`
  - `external_errors=0`
  - `assessment=unity_validation_pending`
- `manage_script validate Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
  - `clean`
- `manage_script validate Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
  - `clean`
- `python scripts/sunset_mcp.py errors --count 20 --output-limit 10`
  - `errors=0 warnings=0`

### 当前判断
- 这轮代码层已经把 `P0-C` 的“formal save vs scene continuity”最小合同立住了。
- 但这不等于整条 `0417` 已完成，也不等于 live / packaged 玩家体验已经终验。
- 当前还剩的真实尾项主要是：
  - `7.3` 箱子跨天 live 判断
  - `9.1 ~ 9.3` world-state live / packaged 检查点

### 恢复点
- 下次继续先看：
  - `.kiro/specs/存档系统/0417.md`
- 当前默认下一刀：
  - `任务 9｜world-state 检查点`
  - 如果 live 结果稳，再看是否转回 `P0-B` 或继续推 `P0-D`

### thread-state 收尾
- 本轮已补：
  - `Park-Slice`
- 当前 live 状态：
  - `PARKED`
- 当前 blocker：
  - 无

## 2026-04-17 续记｜0417 连续施工继续推进到 P0-B / P0-C 新切片

### 用户目标
- 用户明确要求：
  - 继续按 `0417.md` 作为唯一主控板往下做
  - 不要再“一轮一讨论”
  - 当前要从最高优先项继续沉下去做，不是停在解释层

### 本轮完成
1. 先补齐了上一刀还没回写的索引层恢复点，并继续沿当前 `ACTIVE` slice 施工。
2. `P0-B` 继续收了一簇真实代码修补：
   - `GameInputManager / ToolbarSlotUI / PlayerInteraction`
     - runtime reassert 统一改为保留背包偏好槽
   - `GameInputManager.OnDialogueStart()`
     - 改为统一关闭 `Package / Box`
3. `InventoryRuntimeSelectionContractTests` 新增两条护栏：
   - `RuntimeReassertions_ShouldPreserveInventorySelectionPreference`
   - `DialogueLock_ShouldClosePackageAndBoxPanelsTogether`
4. `P0-C` 继续补到底层漏口：
   - `CropController` 现在会在启用和 `PersistentId` 读取时补齐 GUID
   - 运行时新种作物不再因为空 GUID 被 continuity 捕获跳过
5. 新增 world-state live harness：
   - `PersistentWorldStateLiveValidationRunner`
   - `PersistentWorldStateLiveValidationMenu`
   - `WorldStateContinuityContractTests`
6. 当前整体合同测试口径已重新跑通：
   - `InventoryRuntimeSelectionContractTests`
   - `WorkbenchInventoryRefreshContractTests`
   - `SaveManagerDay1RestoreContractTests`
   - `WorldStateContinuityContractTests`
   - `14/14 passed`

### 当前判断
- 这轮最重要的新事实有两条：
  1. `P0-B` 不只是“待 live 验”，而是代码层又真收掉了一刀
  2. `P0-C` 里运行时新种作物空 GUID 这个真漏口已经补掉
- 当前 world-state 的 live harness 已经拿到第一段信号：
  - `Primary -> Home` 的切场本体能进入 `Home`
  - 离场前能看到 `QueueSceneEntry` 确实采到了箱子 / 农地 / 新种作物
- 但 harness 自己跨场景续跑还没完全收平：
  - 还没拿到 `Home -> Primary` / `Town -> Primary` 的最终 pass 报告

### 当前恢复点
- 继续这条线时，优先顺序固定为：
  1. 先回 [0417.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/0417.md)
  2. 再看 [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)
  3. 当前下一刀仍然先收：
     - `任务 9｜world-state live harness / live 终验`
  4. 当前不应漂去：
     - Save UI
     - `F5 / F9`
     - packaged smoke
     - Day1 own staging

### thread-state
- 本轮最终已补：
  - `Park-Slice`
- 当前 live 状态：
  - `PARKED`
- 当前 blocker：
  1. `任务 9` 的 world-state live harness 仍需收平跨场景续跑
  2. `9.1 ~ 9.3` 的 live / packaged 终验尚未完成

## 2026-04-17 续记｜0417-P0-C world-state 主链只读探针：runtime/live runner 与 editor menu 基建盘点

### 用户目标
- 用户要求只读查清 Sunset 现有的 runtime/live validation runner 与 editor menu 基建，判断最适合复用哪一套来做：
  - `Primary / Home / Town world-state continuity` 验证
- 明确禁止：
  - 改代码
  - 回写项目文档

### 本轮完成
1. 完整读取并对照了当前最相关的 runtime / menu / contract / bridge 链：
   - `PersistentPlayerSceneRuntimeMenu`
   - `FarmRuntimeLiveValidationRunner` + menu
   - `PlacementSecondBladeLiveValidationRunner` + menu
   - `NavigationLiveValidationMenu`
   - `SceneTransitionTrigger2D`
   - `PersistentPlayerSceneBridge`
   - `Home / Town` 现有 contract probe
   - `CodexEditorCommandBridge`
2. 明确判断：
   - 最适合复用的不是 `Farm` / `Placement` 那套“重交互 live runner 本体”
   - 而是以 `PersistentPlayerSceneRuntimeMenu` 为核心入口，外加 `CodexEditorCommandBridge` 的 `Library/CodexEditorCommands` 读回通道
   - 如果后续需要“从 Edit Mode 一键自动进 Play 再跑”，再借 `PlacementSecondBladeLiveValidationMenu` / `NavigationLiveValidationMenu` 的 pending-run 壳
3. 明确了门链最稳查找口径：
   - 先按当前 active scene 限域
   - 再按 `targetSceneName / targetScenePath` 过滤 `SceneTransitionTrigger2D`
   - 最后只把对象名当 preferred hint，不把裸名字当唯一真相
4. 明确了最小 live 自动验证 API 面：
   - `SceneTransitionTrigger2D.TryStartTransition()`
   - `PersistentPlayerSceneBridge.QueueSceneEntry(...)` 所在真实门链
   - `PersistentPlayerSceneBridge.ExportOffSceneWorldSnapshotsForSave()`
   - `PersistentPlayerSceneBridge.ImportOffSceneWorldSnapshotsFromSave(...)`
   - `PersistentPlayerSceneBridge.SuppressSceneWorldRestoreForScene(...)`
   - `PersistentPlayerSceneBridge.CancelSuppressedSceneWorldRestore(...)`
   - 如需校验读档后玩家落位，再补 `PersistentPlayerSceneBridge.TryApplyLoadedPlayerState(...)`
   - 走正式存读档闭环时，对接 `SaveManager.SaveGame(...) / LoadGame(...)`

### 当前判断
- 这轮最关键的结论是：
  - `world-state continuity` 的真实业务锚点在 `PersistentPlayerSceneBridge + SceneTransitionTrigger2D + SaveManager`
  - 现有 `Farm` / `Placement` runner 虽然提供了 PlayMode 协程 runner 模式，但它们的验证对象是输入/放置/反馈链，复用成本和误绑成本都高
- 更稳的下一刀不是新造一个大而全的通用 runner，而是：
  - 先围绕 `PersistentPlayerSceneRuntimeMenu` 扩成 world-state 专用 probe / step runner
  - 再按 `Placement/Navigation` 菜单的 pending-run 模式给它补自动进 Play 的薄壳

### 恢复点
- 下次如果进入真实施工，优先落点应在：
  - runtime runner：`Assets/YYY_Scripts/Service/Player/`
  - editor menu 薄壳：`Assets/Editor/Home/` 或同域 `SceneSync`
- 当前默认下一刀建议：
  - `任务 9｜world-state 检查点` 先做最小 world-state continuity runner / menu
  - 不先去改 `Farm` / `Placement` 现有 runner

### thread-state 报实
- 本轮性质：
  - 只读分析
- 未跑：
  - `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：
  - 保持 `PARKED`

## 2026-04-17 22:53 切片补记：树离场跨天补票 + Primary 遮挡运行时重绑

### 当前主线
- 仍是存档系统打包前最小、安全、可演示收尾，不是换线。

### 本轮子任务
1. 把“离场场景的树不会跟着跨天成长”收进正式存档/scene continuity 链。
2. 把 `Primary` 的玩家遮挡失效先用最小安全的 runtime 重绑收住。

### 已完成
1. `Begin-Slice` 已补登记，切片名：
   - `跨场景树石跨天推进+Primary遮挡链排查与最小修复`
2. 代码落地：
   - [SaveDataDTOs.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs)
   - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
   - [TreeController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/TreeController.cs)
   - [OcclusionManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs)
   - [WorldStateContinuityContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs)
3. 这轮对树的合同是：
   - off-scene snapshot 记录离场 `totalDays`
   - 回场恢复时计算 `elapsedDays`
   - 只给 `TreeController` 做最小 catch-up
4. 这轮对遮挡的合同是：
   - bridge 切场时统一重绑场景 `OcclusionManager`
   - `OcclusionManager` 自己也会在运行中自愈失效玩家引用与旧 `playerLayer` 缓存
5. `Fermat` 子线程只读回执已收并吸收：
   - 它认为 scene authoring 也值得继续核
   - 但这轮没有直接去改 `Primary.unity`

### 验证
1. `git diff --check`：通过
2. `validate_script`：
   - touched scripts `owned_errors=0`
   - `assessment=unity_validation_pending`
   - 原因：Unity `stale_status`
3. `manage_script validate`：
   - `TreeController`：`0 error / 1 warning`
   - `OcclusionManager`：`0 error / 2 warning`
   - `PersistentPlayerSceneBridge`：`0 error / 2 warning`
4. `errors --count 20`：
   - `0 error / 0 warning`

### 关键判断
1. 树冻结这条目前已经正式进了 save/load continuity 主链，不再只是只读结论。
2. `Primary` 遮挡这条，这轮先收脚本层最小修口，比直接改 production scene 更安全。
3. 如果用户 live 回测后 `Primary` 仍不对，下一恢复点是：
   - 只读继续核 `Primary.unity` 的实例覆写 / `canBeOccluded` / prefab instance 差异
   - 再决定是否进 scene 改动

### thread-state 报实
1. 本轮已跑：
   - `Begin-Slice`
   - `Park-Slice`
2. 本轮未跑：
   - `Ready-To-Sync`
3. 当前 live 状态：
   - `PARKED`

## 2026-04-17 23:02 状态总盘点补记

### 用户目标
- 用户要求我不要再只报某一刀，而是用人话重新说明：
  - 历史还有什么没完成
  - `0417` 和之前提过的主线内容现在完成到了哪

### 结论
1. 这条线当前不是“全完”，而是：
   - 代码层大头已收
   - live 回归未跑透
   - packaged 验证未收口
2. `0417` 当前可以视为：
   - 主控板成立
   - 关键补口已推进很多
   - 但最终完成定义还没全达成
3. 如果现在要排优先级：
   - 先验 live / packaged
   - 不要先继续美化 Save UI 或回头抠文案

## 2026-04-17 23:48 本轮恢复点更新：箱子 authored 真值修正 + 普通切场统一强刷新已落地

### 当前主线
- 仍是存档系统打包前最小、安全、可演示的收尾。
- 本轮用户明确要求两件事同时完成：
  1. 直接修我自己已经承认的箱子 authored 真值 / 碰撞错链
  2. 直接把 `Home` 那套更重的背包/toolbar 强刷新推广到普通切场
- 同时要求我复核 `farm` 只读结论，但不要越权顺手改农田业务。

### 本轮实际完成
1. [ChestController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/World/Placeable/ChestController.cs)
   - `CaptureAuthoredVisualBaseline()` 现在先识别 root-renderer authored 场景
   - root `SpriteRenderer` 迁到运行时 visual child 时改用 root-local 真值：
     - `Vector3.zero`
     - `Quaternion.identity`
     - `Vector3.one`
   - `UpdateColliderShape()` 不再只加 `colliderOffset`
   - 已改成 `TransformSpritePhysicsPointToChestLocal(...)`，让碰撞跟完整视觉局部变换走
2. [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
   - `InventoryPanelUI` 改成显式 `ConfigureRuntimeContext(...)`
   - `InventoryInteractionManager` 改成显式 `ConfigureRuntimeContext(...)`
   - 继续保留本轮前面已落的：
     - `sceneInventory.RefreshAll()`
     - `sceneHotbarSelection.ReassertCurrentSelection(...)`
     - `activeBoxPanel.ConfigureRuntimeContext(...)`
     - `activeBoxPanel.RefreshUI()`
3. 合同测试同步补强：
   - [WorldStateContinuityContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs)
   - [InventoryRuntimeSelectionContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/InventoryRuntimeSelectionContractTests.cs)
4. `farm` 复核结论已确认：
   - `Primary` 离场后农田/作物没补跨天结算，这个主根因成立
   - 正确合同应拆成：
     - bridge / save contract 提供 `elapsed days`
     - farm 消费这个事实做 catch-up
   - 本轮没动 `FarmTileManager / CropController`，因为 DTO / off-scene snapshot 还没有权威天数锚点，且 `FarmTileManager -> Crop` 恢复顺序有误删风险

### 验证
1. `validate_script`
   - `ChestController.cs`：`owned_errors=0`
   - `PersistentPlayerSceneBridge.cs`：`owned_errors=0`
   - `WorldStateContinuityContractTests.cs + InventoryRuntimeSelectionContractTests.cs`：`owned_errors=0`
   - 统一 assessment=`unity_validation_pending`，原因是 Unity `stale_status`
2. fresh console：
   - `errors=0 warnings=0`
3. scoped `git diff --check`
   - 本轮 4 个 owned 代码/测试文件通过

### 关键决策
1. 箱子这条不再继续猜偏移数值；authoring 正式面就是唯一真值。
2. 普通切场这条不再只做“服务级换引用”；要显式进 UI/交互管理器的 runtime context 入口。
3. `farm` 这条本轮只停在安全合同裁定，不越权乱修。

### 下一步恢复点
1. 如果继续施工，最合理的下一刀是 `farm off-scene catch-up contract`。
2. 这刀要做的是：
   - 给 off-scene snapshot / save contract 补离场到回场的天数事实
   - 再定义 farm 消费这段天数的安全时机
3. 不要再回头做：
   - 继续猜箱子视觉偏移
   - 继续补 `Home` 特例刷新

### thread-state 报实
1. 本轮已跑：
   - `Begin-Slice`
   - `Park-Slice`
2. 本轮未跑：
   - `Ready-To-Sync`
3. 当前 live 状态：
   - `PARKED`

## 2026-04-18 00:12 本轮追加恢复点：Home 背包 4/8 槽位异常

### 用户新反馈
- 用户在上一刀后立即补充：
  - 箱子基本好了
  - 但进入 `Home` 后，背包 `4/8` 槽仍然出问题

### 本轮实际修正
1. [InventoryPanelUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs)
   - `ConfigureRuntimeContext(...)` 新增 `InvalidateSlotBindings()`
   - `RefreshAll()` 新增先 `EnsureBuilt()`
2. [InventoryRuntimeSelectionContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/InventoryRuntimeSelectionContractTests.cs)
   - 已补对应合同断言

### 为什么这样修
1. 这次最像的根因不是 bridge 又没重绑，而是：
   - bridge 已经把 runtime inventory / selection 交过去了
   - 但 `InventoryPanelUI` 自己因为看到还是同一个 persistent 对象引用，就把旧槽位绑定缓存继续沿用了
2. 这会制造“服务真源对了，但个别格子还是旧绑定状态”的坏相，和用户现在说的 `4/8` 固定位置异常高度吻合。

### 验证
1. `manage_script validate InventoryPanelUI`
   - clean
2. `validate_script InventoryRuntimeSelectionContractTests.cs`
   - `owned_errors=0`
   - `unity_validation_pending`（Unity `stale_status`）
3. fresh console
   - `errors=0 warnings=0`
4. scoped `git diff --check`
   - 通过

### 当前恢复点
1. 现在优先等用户重新测：
   - `Primary -> Home`
   - 打开背包
   - 看 `4/8`
2. 如果这次还坏，就要继续查：
   - `InventorySlotUI` 自己的 per-slot 显示状态
   - 或 `PackagePanelTabsUI` 打开时序

## 2026-04-17 21:05 箱子视觉/碰撞与切场强刷补充诊断

- 用户把焦点重新拉回两个具体现象：
  1. 箱子在编辑器里怎么摆，运行就应该怎么看；现在连碰撞体都像跑飞
  2. 进入 `Home` 后背包 `4/8` 等坏相会缓解，说明普通切场缺的不是“更多业务逻辑”，而是“同等级别的默认刷新”

- 只读确认结果：
  1. [ChestController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/World/Placeable/ChestController.cs) 仍在 `Awake()` 里先抓当前 renderer 的 authored baseline，再 `EnsureSpriteRendererUsesVisualChild()` 强行创建/接管 `__ChestSpriteVisual`
  2. 后续 `UpdateSpriteForState() / AlignSpriteBottom() / UpdateColliderShape()` 都围绕这个运行时 child 继续算，导致“场景正式摆放结果”没有被当成唯一真值
  3. `PolygonCollider2D` 的 physics shape 还会叠加 visual child 的 local offset，所以视觉抓错，碰撞一起错
  4. `Home.unity` 里没有现成的 `__ChestSpriteVisual` 序列化对象，说明这层 child 确实是运行时新造，不是作者原本摆好的场景层
  5. `BoxPanelUI` 的 `RefreshRuntimeContextFromScene() + RefreshInventorySlots()` 会对背包槽位逐格重新 `Bind + Refresh + RefreshSelection`
  6. 这条强刷链比普通 `PersistentPlayerSceneBridge.RebindScene()` 只做服务级重绑更重，因此能解释用户看到的“进 Home 会恢复、普通 Town/Primary 往返没学到”

- 新恢复点：
  1. 真修箱子时，不能再调偏移数字，要把 authored closed pose 重新钉成唯一视觉真值
  2. 真修 `4/8` 时，不能只补局部图标或单个 UI，要把 `Home` 那套逐格强刷推广成普通切场后的统一 refresh contract

- 本轮性质：
  - 只读分析
- `thread-state`：
  - 保持 `PARKED`

## 2026-04-17 18:56 +08:00｜箱子摆位与 authored pose 基线修正

### 用户目标
1. 用户最新反馈从“箱子继续长高”收窄成更明确的问题：
   - 场景里摆好的箱子位置，和编辑器 Play / 打包运行时看到的位置不一致。
2. 用户明确纠正：
   - 不要再把“Sprite 底边对齐”理解成“运行时自动把箱子贴到 root 原点”。
   - 真正要守的是：场景 authored 的 `closed` 视觉位置。

### 本轮完成
1. 已继续真实施工，沿用 ACTIVE slice：
   - `箱子摆位与sprite底边对齐修正_以场景摆位为真`
2. 已修改：
   - `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
   - `Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs`
3. `ChestController` 新语义：
   - `Awake()` 先捕获 authored visual baseline
   - 若 root SpriteRenderer 迁到 `__ChestSpriteVisual`，child 会立即回放 authored pose
   - `AlignSpriteBottom()` 不再用 `localPos.y = -spriteBottomOffset`
   - 改为只让当前 sprite 的底边对齐 authored 底边基线
4. 这样 `closed` 状态会保持场景里摆出来的正式面，`open` 状态只相对它变化，不再整体漂移。

### 验证
1. `validate_script Assets/YYY_Scripts/World/Placeable/ChestController.cs`
   - owned error=`0`
   - native validation=`warning`
   - CLI assessment=`unity_validation_pending`
   - 原因：Unity 编译轮询超时，不是 owned compile fail
2. `validate_script Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs`
   - owned error=`0`
   - native validation=`clean`
   - CLI assessment=`unity_validation_pending`
3. fresh console：
   - `errors=0 warnings=0`
4. EditMode：
   - `WorldStateContinuityContractTests.ChestController_ShouldNotMoveRootTransformWhenAligningSpriteBottom`
   - `1/1 passed`
5. scoped `git diff --check`：
   - 仅检查本轮 owned 两文件，已通过

### 当前判断
1. 箱子位置错位的最高置信根因已经改到代码层。
2. 这轮站住的是：
   - 结构层
   - 合同测试层
3. 还没站住的是：
   - Home 箱子的 live / packaged 最终视觉体验

### 恢复点
1. 下一步若继续这条线，优先做最小 live 复核：
   - `closed` 是否保持场景摆位
   - `open` 是否只改箱盖/高度，不再整体挪位
2. 如果 live 仍不对，再回看：
   - authored baseline 是否在某些 prefab/动态生成箱子上被更晚的 sprite 覆盖
   - 是否还有别的运行时代码在 `Awake/Initialize/Load` 后再次改写 child localPosition

### thread-state
1. 本轮已跑：
   - `Begin-Slice`
   - `Park-Slice`
2. 本轮未跑：
   - `Ready-To-Sync`
3. 当前 live 状态：
   - `PARKED`
4. 停车原因：
   - 代码层修正与合同验证已完成，下一步需要 Home 箱子 live / packaged 视觉复核。

## 2026-04-17 19:24 +08:00｜玩家背包 sort / 垃圾桶 / runtime context 收口

### 用户目标
1. 用户明确说箱子先不要动，箱子的交互和 sort 目前基本没问题。
2. 当前主问题是玩家背包：
   - 背包 sort 不对
   - 跨场景后更不对
   - 垃圾桶也要检查
3. 用户指出的关键语义：
   - 背包 sort 不应该像只整理部分/一行一行，而应该是整包统一整理。

### 本轮完成
1. 已进入真实施工：
   - `Begin-Slice`
   - slice：`玩家背包sort与垃圾桶跨场景一致性修复`
2. 已修改：
   - `Assets/YYY_Scripts/Service/Inventory/InventorySortService.cs`
   - `Assets/YYY_Scripts/Service/Inventory/InventoryService.cs`
   - `Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs`
   - `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
   - `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
   - `Assets/YYY_Tests/Editor/InventoryRuntimeSelectionContractTests.cs`
3. 背包 sort 新语义：
   - `InventorySortService` 从 `12..35` 改成 `0..Capacity-1`
   - `InventoryService.Sort()` fallback 同步改成整包语义
   - 普通物品会先合并，再排序，再逐槽回写 runtime item
4. 跨场景重绑：
   - `InventorySortService` 每次 sort 前优先吃 bridge 当前 runtime inventory
   - `InventoryInteractionManager` 的底层 get/set/clear/return/sort 前也会先重绑当前 runtime inventory / equipment / sortService
   - `InventoryPanelUI` 重新打开时也会优先覆盖到当前 runtime context，不再只看字段是否为 null
5. 垃圾桶语义：
   - 背包垃圾桶从“掉到玩家脚下”改成真删除
   - 拖到面板外仍然是掉地，和垃圾桶删除分开

### 验证
1. Unity `validate_script`：
   - `InventorySortService.cs`：`0 error / 0 warning`
   - `InventoryService.cs`：`0 error / 0 warning`
   - `InventoryPanelUI.cs`：`0 error / 0 warning`
   - `InventoryRuntimeSelectionContractTests.cs`：`0 error / 0 warning`
   - `InventoryInteractionManager.cs`：`0 error / 1 warning`，warning 为既有字符串拼接 GC 提示
   - `InventorySlotInteraction.cs`：`0 error / 1 warning`，warning 同类
2. fresh console：
   - `errors=0 warnings=0`
3. EditMode：
   - `InventorySort_ShouldUseWholeBackpackAndRuntimeAuthoritativeContext`
   - `InventoryTrash_ShouldDiscardHeldItemInsteadOfDroppingAtPlayerFeet`
   - `SceneRebind_ShouldRefreshInputAndSortRuntimeContext`
   - `3/3 passed`

### 当前判断
1. 代码层已经修掉最像主因的三件事：
   - 背包 sort 不是整包
   - 背包 sort / InventoryService fallback 语义不统一
   - 垃圾桶其实是掉地，不是真删除
2. 跨场景问题也通过 runtime context 重绑加固了一层。
3. 还不能宣称玩家体验终验过，因为仍缺 live / packaged 路径复测：
   - `Primary` 背包 sort
   - `Home` 背包 sort
   - `Town` 背包 sort
   - 垃圾桶真删除
   - sort 后 toolbar / 当前选中 / 使用链是否仍正常

### 恢复点
1. 下一步只做 live / packaged 复测，不再先动箱子。
2. 如果用户复测仍发现背包回弹或 sort 后显示与使用不一致，下一刀优先查：
   - 是否还有 PackagePanel / InventorySlotUI 持有旧 scene-local `InventoryService`
   - 是否还有非 manager 路径直接改旧背包

### thread-state
1. 本轮已跑：
   - `Begin-Slice`
   - `Park-Slice`
2. 本轮未跑：
   - `Ready-To-Sync`
3. 当前 live 状态：
   - `PARKED`
4. 停车原因：
   - 玩家背包 `sort / 垃圾桶 / runtime context` 的代码层与合同验证已完成，下一步需要真实三场景 live / packaged 复测。

## 2026-04-17 真实施工补记｜最小打包收尾刀第二簇：切场唯一真源 + 直载入口收口

### 用户目标
- 用户最终把目标收成：不是再做过程刀口，而是这一轮直接落“最小、最安全、一轮内砍到可直接打包”的完整收尾刀。
- 用户最在意的实际坏相：
  1. `Primary -> Home -> Primary` 物品回弹、临时双份
  2. 箱子上半区/下半区、Sort、toolbar、工具使用断联
  3. 树/掉落物/农地/作物切场后复活或丢失

### 本轮新增施工
1. `PersistentPlayerSceneBridge.CaptureSceneRuntimeState()` 改为通过 `ResolveRuntimeInventoryService(scene)` / `ResolveRuntimeHotbarSelectionService(scene)` 抓离场快照，避免把 scene-local duplicate 当快照真源。
2. `SaveManager.RestoreInventoryData()` 的旧存档兼容回写改为优先 `PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()`。
3. 以下运行时消费者已改为 bridge 优先真源：
   - `CraftingService`
   - `HotbarSelectionService`
   - `InventorySortService`
   - `AutoPickupService`
   - `PlacementManager`
   - `PlayerInteraction`
4. `SpringDay1Director` 内 3 处直接 `SceneManager.LoadScene(...)` 已统一改成 `LoadSceneThroughPersistentBridge()`，先 `QueueSceneEntry()` 再切场。
5. 旧 `DoorTrigger` 也补了 `QueueSceneEntry()`。
6. `WorldStateContinuityContractTests` 新增：
   - 离场快照必须抓 resolved runtime inventory / hotbar
   - 直切场入口必须先走 bridge
   - 运行时库存消费者必须优先 bridge 真源

### 验证
1. `manage_script validate`：
   - 本轮改动脚本均 `0 error`，仅保留既有 warning。
2. EditMode：
   - `WorldStateContinuityContractTests`
   - `WorkbenchInventoryRefreshContractTests`
   - `17/17 passed`
3. fresh console：
   - `errors=0 warnings=0`

### 当前判断
1. 这轮已经把最伤人的两个主根因同时收了：
   - 切场后抓错背包/热栏真源
   - 一部分剧情/旧门链切场绕过 bridge
2. 当前是否已经完全到“可直接打包最小态”，还差最后一层 live / packaged 冒烟，不再差新的大块代码语义。
3. 下一轮若继续，最有价值的只剩：
   - `Primary 丢物 -> Home -> Primary`
   - `Home 箱子上下半区 + Sort`
   - `Primary 树/掉落物/农地 continuity`
   的真实玩家路径验证。

## 2026-04-17 只读追加｜Primary world-state 复活/丢失根因复核
- 用户把问题收束成单题：只读解释为什么 `Primary` 的树、耕地、浇水、作物在切到 `Home/Town` 再回 `Primary` 时仍可能复活或丢失，不做实现。
- 结论优先级：
  1. 普通门链 `SceneTransitionTrigger2D.TryStartTransition()` 本身会先 `QueueSceneEntry()` 再异步切场，看起来不是第一主嫌。
  2. 更危险的是项目里仍存在绕过 bridge 的直载入口，已静态钉到 [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 多处 `SceneManager.LoadScene(..., LoadSceneMode.Single)`。
  3. 树的删除态目前仍偏脆：`TreeController` 被彻底 `Destroy` 后，bridge 主要靠“快照中没有这个 GUID”来推断已删；这要求离场捕获、回场恢复、场景原生 GUID 三件事同时成立。
  4. `FarmTileManager.Save/Load` 与 `CropController.Save/Load` 本身结构是能对上的；如果耕地/浇水/作物一起丢，更像整次 scene continuity 快照没吃到，而不是单字段漏存。
- 本轮推荐的施工起点已经固定：
  1. `SpringDay1Director` 统一切到 bridge 合同
  2. `PersistentPlayerSceneBridge.CaptureSceneWorldRuntimeState / RestoreSceneWorldRuntimeState / RemoveUnexpectedSceneWorldBindings`
  3. `TreeController` 的删除态表达
- 验证状态：
  - 纯静态代码审计
  - 未做 live / packaged 复测

## 2026-04-17 续记｜用户要求暂停续工，先把箱子/背包/持久化当前真实问题彻底讲清楚

### 用户目标
- 用户明确要求这轮不要继续往 `0417` 施工面硬推，先做只读总复盘：
  1. 用人话汇总这条线已经做了什么、还没做什么
  2. 认真排查 `Home` 箱子越来越高、`Home / Town` 箱子界面下半区背包断联、箱子内容回场复活这些问题
  3. 不要再拿阶段名、任务编号或文档黑话和用户说

### 本轮只读结论
1. `Home` 箱子上漂已经有高置信代码根因：
   - `ChestController.AlignSpriteBottom()` 改的是箱子根节点 `transform.localPosition`
   - 对照 `TreeController`，正确模式应是只改 `spriteRenderer.transform.localPosition`
   - 当前写法会把视觉对齐混进真实位置，所以箱子会越回场越偏、交互点也会跟着漂
2. 箱子内容“回来又吃预设”也有明确风险来源：
   - `ChestController.Initialize()` 里 `ApplyAuthoringSlotsIfNeeded()` 还会在新建空库存时重新灌作者预设
   - 如果某次 continuity / restore 没命中，这个逻辑就会让箱子看起来“复活”
3. 箱子界面下半区背包断联，目前还没锁到唯一函数，但边界已经收窄：
   - 上半区箱子主要靠 `ChestInventoryV2 + BoxPanelUI`
   - 下半区背包要再经过 `InventorySlotInteraction -> InventoryInteractionManager -> GameInputManager`
   - 所以“箱子那边能动、背包这边像死掉”不是用户错觉，而是两条链稳定度不同
4. 当前这条线已经做出来的，不是空白：
   - 正式存档 UI / 默认档入口壳体
   - Day1 读档/重开入口卫生链
   - 背包真选槽第一轮修补
   - 箱子 / 农地 / 作物离场快照正式容器
   - 对应的合同测试与 live harness 雏形
5. 但当前绝不能宣称“已经全好”：
   - `Home / Town / packaged` 的真实箱子和背包交互仍未被 live 终验证明
   - 现有 `14/14` 测试只能证明结构护栏，不等于玩家体验已过线

### 对下一轮的实际含义
- 如果下一轮转回真实施工，最该先收的已经不是泛存档讨论，而是：
  1. 先修箱子根节点位置污染
  2. 再钉箱子界面下半区背包失联
  3. 然后再回头做 continuity / packaged 终验

### thread-state 报实
- 本轮只读分析
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-17 15:18 最小刀口停刀汇总：Crop continuity 已补，当前用户向汇报前先合法停车

### 用户目标
- 用户要求把 `0417` 当唯一主控板持续做，但这轮先不要再漫游，先做完一个最小刀口，然后详细汇报：
  1. 已做完什么
  2. 还没做完什么
  3. 最大头还剩什么
  4. 哪些内容可以先不纳入打包

### 本轮子任务
- 不继续扩修 Save UI / F5F9 / Day1 staging。
- 只补一个最小刀口：
  - 把 `农地 / 浇水 / 作物离场回来会丢` 里的作物 continuity 合同再补完整。
- 然后把当前现场重新核一遍，尤其是 world-state runner 与 console 的真实阻塞。

### 本轮实际完成
1. `Assets/YYY_Scripts/Farm/CropController.cs`
   - `Save()` 改为保存 `GetCellCenterPosition()`
   - `Load()` 改为从 `seedId` 恢复 `seedData`
   - `Load()` 同步回填 `instanceData.seedDataID`
2. `Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs`
   - 新增字符串合同断言，锁住上述作物 continuity 关键点
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\0417.md`
   - 已回填 `9.2` 与 runner 二次实跑证据

### fresh 验证与当前判断
- `mcp validate_script`
  - `CropController.cs`：`0 error / 1 warning`
  - `WorldStateContinuityContractTests.cs`：`0 error / 0 warning`
  - `PersistentWorldStateLiveValidationRunner.cs`：`0 error / 0 warning`
- scoped `git diff --check`：通过
- fresh CLI console：
  - 读到 `2` 条 `EventSystem` 错误
- fresh CLI `validate_script`：
  - 本轮未拿到稳定 JSON，报 `CodexCodeGuard returned no JSON`
- 当前最关键判断：
  1. 这次最小刀口已经完成
  2. 但当前还不能说“可打包”
  3. 真正最大的阻塞已经收窄成：
     - `Primary <-> Home` 往返时的 duplicate runtime root / duplicate EventSystem / inventory rebind 冲突
  4. 这层问题比 crop 单点更大，也更像 `Home/Town` 箱子背包断联、runner 中断、workbench 识别异常的共同根因面

### 还没做完的最大头
1. `Primary / Home / Town` 往返的 world-state 真实闭环
2. `Home / Town` 箱子界面下半区背包不断联
3. 箱子拖到背包、排序、切场后工作台识别链
4. Save / Load / Restart / 切场 smoke
5. packaged live smoke

### 当前可先不纳入打包阻塞的内容
1. Save UI 进一步美化
2. F5/F9 与默认档文案细修
3. Day1 staging own 逻辑
4. 泛外发版体系化整理

### 恢复点
- 下一轮如果继续真实施工，不要先回 UI 或 Town blocker。
- 先查：
  1. 第二个 `EventSystem` 是谁在 `Primary <-> Home` 往返时生成的
  2. `PlayerInventory` 或 persistent inventory root 是否也在重复生成/重绑
  3. 收住这层 duplicate runtime root 之后，再回 `13.4 / 14.x`

### thread-state
- 本轮已跑：
  - `Park-Slice`
- 本轮未跑：
  - `Ready-To-Sync`
- 当前 live 状态：
  - `PARKED`
- 停车原因：
  - 用户这轮要完整现状汇报，不继续往下一刀扩写

## 2026-04-17 16:05 最小打包刀施工完成：删除态不复活、箱子底边对齐、读档/重开刷新闭环

### 用户目标
- 用户明确要求：不要再追边角，要把打包前最必要的 world-state / 箱子 / 掉落物 / 读档 / 重新开始这条线真正收紧，并确保 toolbar、背包、交互刷新不能再出错。

### 本轮施工范围
1. `PersistentPlayerSceneBridge.cs`
2. `ChestController.cs`
3. `SaveManager.cs`
4. `WorldStateContinuityContractTests.cs`
5. `0417.md`

### 本轮实际完成
1. 补齐 off-scene 删除态合同
   - 不再只清理 `Drop`
   - 现在 `Tree / Stone / Chest / Crop / Drop` 都能按“快照里不存在 = 已被玩家移除”处理
   - 空快照也会保留并导出，解决“最后一棵树/最后一个掉落物”因快照为空而复活的问题
2. 修箱子开关的视觉与碰撞
   - 根节点 SpriteRenderer 运行时转视觉子节点
   - 打开/关闭按底边对齐
   - PolygonCollider 跟随视觉偏移，不再向下扩挤玩家
3. 修读档 / 重新开始后的 UI 刷新
   - 统一刷新所有 InventoryService / HotbarSelectionService / InventoryPanelUI / ToolbarUI
   - 重发当前选中态
   - 强制 Canvas 刷新

### 验证结果
- `validate_script`
  - `PersistentPlayerSceneBridge.cs`：`0 error / 2 warning`
  - `ChestController.cs`：`0 error / 1 warning`
  - `SaveManager.cs`：`0 error / 3 warning`
  - `WorldStateContinuityContractTests.cs`：`0 error / 0 warning`
- EditMode 测试
  - `WorldStateContinuityContractTests`
  - `WorkbenchInventoryRefreshContractTests`
  - `11/11 passed`
- fresh console
  - `errors=0 warnings=0`

### 当前判断
1. 这轮最核心的语义洞已经补上了：系统现在开始真正记住“这个东西已经被移除了，不该回来”。
2. 这能直接覆盖：
   - 树复活
   - 石头复活
   - 箱子/掉落物/作物的同类问题
3. 但这轮还没有做 live / packaged 的最终玩家路径验证，所以不能直接把“结构已补”说成“体验已过线”。

### 下一步恢复点
1. 最短 live 冒烟：
   - 砍树
   - 丢掉落物
   - 开箱/关箱
   - `Primary <-> Home`
2. 再测：
   - `Save / Load`
   - `重新开始`
3. 如果 live 过，再进 packaged smoke

### thread-state
- 本轮已跑：
  - `Begin-Slice`
  - `Park-Slice`
- 本轮未跑：
  - `Ready-To-Sync`
- 当前 live 状态：
  - `PARKED`
- 停车原因：
  - 当前应先做 live 冒烟，不再继续叠静态代码改动

## 2026-04-17 只读大调查补记：当前更底层的问题是切场真源分叉，不只是 world-state case 没补全

### 用户目标
- 用户这轮明确要求：
  1. 不继续盲修单点 bug
  2. 先重新看场景实际配置与运行链
  3. 用一个更底层的解释把 `Primary -> Home -> Primary` 的物品回弹、临时双份、箱子下半区背包断联讲清楚

### 本轮只读新结论
1. `Primary / Home / Town` 三个主场景都各自内置整套 scene-local 根：
   - `Systems`
   - `InventorySystem`
   - `HotbarSelection`
   - `EquipmentSystem`
   - `UI`
   - `EventSystem`
   - `PersistentManagers`
2. `PersistentPlayerSceneBridge` 本身不是场景对象，而是 runtime bootstrap；所以切场时天然会出现“persistent 旧根 + 新场景副本”并存窗口。
3. 当前最高置信根因在 `PersistentPlayerSceneBridge.RebindScene()`：
   - `PromoteSceneRuntimeRoots(scene)` 先把 duplicate roots 标记为 `Destroy`
   - 但同一帧后面又立刻去解析并绑定：
     - `InventoryService`
     - `HotbarSelectionService`
     - `GameInputManager`
     - `PackagePanelTabsUI / InventoryPanelUI / ToolbarUI`
   - 因为 `Destroy()` 是帧末才真正销毁，所以 bridge 很可能是在用“已判死的 scene-local duplicate”做 runtime 真源重绑
4. 这条根因会被大量全局搜索进一步放大：
   - `ToolbarUI / InventoryPanelUI / InventoryInteractionManager / BoxPanelUI / InventorySlotInteraction / GameInputManager / PlacementManager / PlayerInteraction / CraftingService`
   - 这些链路里到处都有 `FindFirstObjectByType<InventoryService / HotbarSelectionService / PackagePanelTabsUI / EquipmentService>` 回退
5. 用户新复现因此更像：
   - 掉落物事实是一套
   - persistent 背包可能是一套
   - package / box / inventory UI 又可能临时绑到 scene-local duplicate
   - 所以才会出现“某场景里看起来对，回另一个场景又回弹，地上那份还在”
6. 额外高风险点也已确认：
   - 三个场景里的 `InventoryService` 都带同一个 `_persistentId`
   - `EquipmentService` 代码仍是固定单例 ID
   - duplicate 活着的窗口里，注册中心 / 恢复层也存在撞 ID 风险
7. `InventoryBootstrap` 已排除为当前 build/live 主因：
   - `Primary` 场景虽然有它，但当前序列化是 `runOnBuild = 0`
   - `Home / Town` 没有它

### 当前判断
- 这轮最重要的方向校准是：
  - 下一刀不能继续只补 `Tree / Drop / Chest / FarmTile / Crop`
  - 必须先收“切场后唯一库存 / 热栏 / 输入 / Package UI 真源合同”
- 更准确的人话：
  - 之前的 world-state 修补不是白做
  - 但它已经压不住更底层的 scene rebind 真源分叉

### 恢复点
- 下一轮若进入真实施工，第一优先级应是：
  1. persistent 根已存在时，不再把 scene-local `InventorySystem / HotbarSelection / EquipmentSystem / Systems / EventSystem / UI` 当 runtime 绑定源
  2. bridge / box / inventory / package 链统一只吃同一套 runtime context
  3. 然后再重测：
     - `Primary 丢物 -> Home -> Primary`
     - `Home 箱子上下半区`
     - `Primary 砍树 / 掉落物 -> 切场回场`

### thread-state 报实
- 本轮性质：
  - 只读分析
- 未跑：
  - `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：
  - 保持 `PARKED`

## 2026-04-18 00:26 只读总审：打包前最小收尾清单口径

### 当前主线目标
- 用代码重审的方式，重新给用户一份“到底完成了多少、还差多少、打包前最小要补哪几刀”的人话总表。

### 本轮子任务
- 不改代码，只重新核关键实现和合同测试，避免再靠旧文档或旧记忆直接下结论。

### 重新核过后的稳定结论
1. 默认档当前真语义已经固定为：
   - `Town` 原生开局
   - `F5` 停用
   - `F9` 只负责读默认开局
2. world-state 正式链路已经真的覆盖到：
   - `FarmTileManager / Crop / Chest / Drop / Tree / Stone`
3. 删除态权威恢复已经进 bridge：
   - 快照里没有的 `Tree / Stone / Chest / Drop / Crop` 会被关掉，不该再原地复活
4. 树的离场跨天成长已经真的补上：
   - `SceneWorldSnapshotSaveData` 记录离场天数
   - bridge 回场时会算经过了几天
   - `TreeController.ApplyOffSceneElapsedDays(...)` 会执行补票
5. 背包 / toolbar / sort / 垃圾桶这几条代码主链已经收过：
   - 切场重绑会重配 runtime inventory、selection、panel、toolbar、box panel
   - sort 已是整包语义
   - 垃圾桶已是真删除，不再伪装成掉地
6. 箱子 authored 显示基线、视觉子节点与 collider 同步逻辑已经在代码里。
7. Day1 那个最致命的“按 phase 自动补写 completed dialogue”问题，静态代码上已不再存在于 `HasCompletedDialogueSequence()`。

### 仍未闭环的点
1. 农田 / 作物的离场跨天补票还没有像树那样真正接进 bridge catch-up。
2. Day1 整条 restart / load / restore 的实机闭环还没有重新跑透。
3. Save UI 按钮功能在，但体验层不能报最终验收。
4. packaged smoke 还没有最后过线。

### 这轮对用户该怎么说
1. 不能说“全部完成，可以放心打包”。
2. 更准确的说法应是：
   - 代码层大头已经收进去很多
   - 真正还挡打包的是 live / packaged 的最后几条硬回归
   - 不挡打包的主要是 UI 美化和文案尾账

### 恢复点
- 下一轮若继续，应优先收：
  1. 三场景 live 真回归
  2. 重开 / 读档 / Day1 恢复真回归
  3. packaged smoke

## 2026-04-18 默认槽/F5/F9/重新开始语义只读彻查

### 当前主线目标
- 围绕 `存档系统` 继续把最终产品语义钉实，避免默认槽、快速键和 UI 按钮继续沿用临时止血版的“默认开局入口”旧壳。

### 本轮子任务
- 用户要求我只读彻查相关代码，直接回答：
  - 现在的 `默认开局 / 停用 / 加载默认开局 / 重新开始` 在代码里到底是什么意思
  - 为什么它和用户要的“默认存档 + F5 快速保存 + F9 快速读取”冲突
  - 默认槽 UI 哪些地方只是文案错，哪些地方是底层语义也错
  - 下一轮最小但完整的修法应该怎么拆

### 本轮读取与结论
1. 重新核对：
   - [SaveManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs)
   - [PackageSaveSettingsPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs)
   - [SaveDataDTOs.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs)
   - 历史来源 [memory_0.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/memory_0.md) `2026-04-07 默认存档降级为“原生开局入口”`
2. 钉实当前代码语义：
   - 默认槽目前不是正式默认存档，而是 `Town` 原生开局入口
   - `F5` 当前只弹“已停用”
   - `F9` 当前直接走原生重开链
   - `QuickLoadDefaultSlot()` 与 `RestartToFreshGame()` 当前本质同函数
   - 默认槽不读真实文件、不写真实文件
3. 钉实 UI 层偏差：
   - 默认区标题/说明/快捷说明/按钮文字都建立在这套错误语义上
   - 默认槽左侧信息之所以和普通槽不一样，是因为代码故意分了 `DefaultSummary()` 与 `CompactSummary()`
   - 默认描述贴得太上，是默认摘要卡的 padding + title + `UpperLeft` 对齐共同造成的

### 关键判断
1. 这不是“只改字”能修好的问题。
2. 必须一起改 5 个面：
   - 默认槽能力矩阵
   - 默认槽是否真实落盘
   - `F5/F9`
   - 默认区摘要模板
   - `重新开始` 的独立语义与位置
3. 用户最新正式语义我已按下面理解固定：
   - 默认槽名字就是“默认存档”
   - `F5 = 快速保存到默认存档`
   - `F9 = 快速读取默认存档`
   - 默认槽允许加载、复制、快速保存、快速读取
   - 默认槽禁止粘贴、手动覆盖、删除
   - `重新开始` 是独立危险操作，不等于默认槽读档

### 下一轮最稳修法
1. 先改 `SaveManager`：
   - 让默认槽重新变成真实文件槽
   - 把 quick-save / quick-load 与 restart 彻底拆义
   - 把 `legacy baseline reserved` 和 `default slot protected` 分成两类规则
2. 再改 `PackageSaveSettingsPanel`：
   - 默认区改名“默认存档”
   - 删除“只读入口”
   - 默认摘要改用普通槽同一模板
   - 默认区只留加载/复制
   - `重新开始游戏` 挪到顶部独立区域
3. 最后补合同验证：
   - 默认槽 quick-save / quick-load
   - 默认槽禁粘贴删除
   - restart 不再等于 quick-load

### 验证状态
- 本轮是只读代码审计，没有进真实施工。
- 当前结论属于：`静态代码 + 历史裁定来源` 已成立，尚未 live 验证。

### thread-state
- 本轮性质：只读分析
- 未跑：`Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-18 切片补记｜Town House 4_0 遮挡单点收尾改成组件缓存自愈

### 当前主线 / 子任务 / 恢复点
- 主线仍是 `0417` 打包前最小安全闭环。
- 本轮子任务：把用户新收窄出来的 `Town / House 4_0` 单点遮挡异常收掉，同时解释“为什么很多物体都有两个 OcclusionTransparency”。
- 修复后恢复点：
  1. 遮挡线这轮不要去动 `Town.unity`
  2. 真正主修已经落在 `OcclusionTransparency.cs`
  3. `Town` scene-level 重复组件残留仍可另开 cleanup 刀，但不再和当前打包前闭环混做一刀

### 本轮已完成
1. 先做只读归因：
   - `House 4.prefab` 根节点原本就有 1 个 `OcclusionTransparency`
   - `Town.unity` 中 `House 4_0` 又额外加了 1 个 scene-level `OcclusionTransparency`
   - 同类残留在 `House 1/2/3` 也存在，所以“很多物体都是两个组件”这个用户观察成立
2. 补跑剩余唯一建筑遮挡票：
   - `OcclusionSystemTests.PlayerBehindBuilding_WithPixelSamplingHole_StillOccludesByBoundsFallback`
   - 定位到失败真因不是 bounds fallback 算法本身，而是 `GetSortingLayerName()` 读到空字符串，建筑在 `sameSortingLayerOnly` 前置过滤就被跳过
3. 正式修口：
   - `OcclusionTransparency` 新增 `EnsureRendererCache(...)`
   - `Awake / OnEnable / Update / ApplyAlpha / GetSortingOrder / GetSortingLayerName / GetBounds / GetColliderBounds / GetPreviewOcclusionBounds / ContainsPointPrecise / CalculateOcclusionRatioPrecise` 全部改为按需自愈 renderer cache
   - 这样即便组件在初始化链之外先被访问，也不会因为 `mainRenderer == null` 直接丢 layer、丢 bounds、丢精确采样链
4. 测试已过：
   - `PlayerBehindBuilding_WithPixelSamplingHole_StillOccludesByBoundsFallback`：`passed`
   - 遮挡最小 5 票回归：`5/5 passed`

### 关键判断
1. `House 4_0` 的剩余坏相，不是“它的 authored 配置和别的房子不同”，而是它把旧的组件缓存缝暴露成了最后一个 live 症状。
2. `Town` 里的双组件更像历史场景残留，不是本轮唯一主根因。
3. 当前最小安全打包态，不应为了这个单点再去广泛清 `Town` scene YAML；先保 runtime 链一致更安全。

### 验证 / blocker
- EditMode fresh：
  - `OcclusionSystemTests` 5 张关键票全部通过
- 当前 external blocker：
  - `Assets/YYY_Tests/Editor/PackagePanelLayoutGuardsTests.cs(381,52)` 仍有外部 `IEqualityComparer<>` 编译红
  - 这不是本轮 owned 文件

### thread-state
- 本轮性质：真实施工
- 已跑：`Begin-Slice`（`0417_遮挡收尾_House4建筑孔洞兜底修复`）
- 尚未跑：`Ready-To-Sync / Park-Slice`

## 2026-04-18 施工恢复点｜0417 背包旧快照回弹主修已落，world-state/workbench 边界已补清

### 当前主线目标
- 仍是 `0417` 的打包前最小安全闭环。
- 本轮先收：
  1. `Load / Restart` 后背包与 `4/8` 的旧快照回弹
  2. `crop / farm / workbench` 的最小保存边界
  3. 遮挡线只保留最小必要续修，不再无限漫游

### 本轮子任务
- 正式施工：
  - `SaveManager.cs`
  - `PersistentPlayerSceneBridge.cs`
  - `WorldStateContinuityContractTests.cs`
  - `InventoryRuntimeSelectionContractTests.cs`
- 并行只读：
  - `crop / farm / workbench` 保存恢复链复核
- 顺手延续：
  - `OcclusionManager.cs`
  - `OcclusionSystemTests.cs`

### 已完成事项
1. 已补 `PersistentPlayerSceneBridge.SyncActiveSceneInventorySnapshot()`。
2. 已在 `ApplyLoadedSaveData()` 里加入：
   - rebind 前同步一次 snapshot
   - `RefreshAllUI()` 后再同步一次 snapshot
3. 已在 `NativeFreshRestartRoutine()` 里补同一套 restart 后 snapshot 同步。
4. 已把 `RefreshSceneRuntimeBindingsInternal()` 改成先：
   - `RestoreSceneInventoryState(sceneInventory)`
   - `RebindHotbarSelection(sceneHotbarSelection, sceneInventory)`
   再继续 UI / 输入重绑。
5. 已补合同测试守住这条修法：
   - `WorldStateContinuityContractTests.SaveManager_LoadAndRestart_ShouldRefreshInventoryToolbarAndSelection`
   - `InventoryRuntimeSelectionContractTests.SceneRebind_ShouldRefreshInputAndSortRuntimeContext`
6. 已复核 `crop / farm / workbench` 当前边界：
   - `legacy crop` 提升链仍在
   - `workbench` 活跃制作队列仍未正式入盘
   - 但 `CanSaveNow()` 已明确阻止“制作中 / 待领取 / floating state”保存
7. 已跑 fresh 验证：
   - `WorldStateContinuityContractTests.SaveManager_LoadAndRestart_ShouldRefreshInventoryToolbarAndSelection` = `passed`
   - `InventoryRuntimeSelectionContractTests.SceneRebind_ShouldRefreshInputAndSortRuntimeContext` = `passed`
   - `WorldStateContinuityContractTests.SaveManager_ShouldPromoteLegacyFarmTilesIntoCurrentCropWorldObjects` = `passed`
   - `WorkbenchInventoryRefreshContractTests.WorkbenchOpen_ShouldRebindCraftingServiceBeforeCountingMaterials` = `passed`
   - `StoryProgressPersistenceServiceTests.CanSaveNow_BlocksDialogueChatAndWorkbenchCrafting` = `passed`
   - `StoryProgressPersistenceServiceTests.CanSaveNow_ShouldAlsoBlockReadyWorkbenchOutputsAndFloatingQueueState` = `passed`
   - `errors --count 30` = `errors=0 warnings=0`

### 关键判断
1. `读档 / 重开后背包像旧状态重新盖回来` 的最高置信根因，已经收敛成：
   - bridge 内部旧 `inventorySnapshot / hotbarSelectionSnapshot / selectedInventoryIndex snapshot`
   - 没有在 load / restart 后被真值覆盖
   - 下一次场景重绑又把旧状态打回 runtime
2. 这轮修法是最小而且正确的：
   - 先同步 bridge snapshot
   - 再 authoritative rebind
   - 最后再刷新 UI 壳
3. `crop / farm` 现在更像要继续盯 live 兼容面，而不是重写主链。
4. `workbench` 当前不要误判成“已正式持久化”；最安全口径仍是 blocker。
5. 遮挡线主修已进代码，但仍留 1 张像素孔洞票待继续收；当前不是 build 红。

### 验证结果
- 本轮性质：真实施工
- 已跑：
  - `Begin-Slice`（`0417_打包前最小闭环_遮挡+背包回弹+存档恢复`）
  - 收尾前已跑 `Park-Slice`
- 未跑：
  - `Ready-To-Sync`
- 当前 live 状态：
  - `PARKED`

### 修复后恢复点
1. 下轮若继续，优先做：
   - `load / restart -> Primary/Home/Town 往返` 的 fresh live / packaged 回归
   - 直接看旧快照回弹是否真的消失
2. world-state 继续时，优先 fresh 核“作物没保留”到底是不是 legacy 档口。
3. 不要下一轮一上来就去扩工作台正式 DTO。
4. 遮挡线若继续，只收：
   - `PlayerBehindBuilding_WithPixelSamplingHole_StillOccludesByBoundsFallback`
   - 不再重开更大范围的遮挡重构。

## 2026-04-18 12:57 只读审查｜加载存档后作物保留、农地/作物闭环、工作台进度落盘范围

### 用户目标
- 用户要求我作为 `Sunset` 存档系统的只读代码审查子线程，围绕打包前最小安全闭环，静态检查三件事：
  1. 加载存档后作物为什么会不会保留
  2. 农地与作物恢复当前是否闭环
  3. 工作台进度当前到底保存到哪
- 明确要求：只读、不改代码、不回退别人改动，最后按“当前主链事实 / 最可能根因 / 最小安全修法 / 仍待 live 验证点”输出中文结论。

### 当前主线目标
- 仍是打包前最小安全闭环。

### 本轮子任务 / 阻塞
- 子任务是把“当前格式主链”“legacy 兼容断层”“工作台 runtime 队列态缺口”三者拆开，避免后续修法误落在错误主链。

### 已完成事项
1. 已确认当前新格式主链里，`SaveManager -> PersistentObjectRegistry -> FarmTileManager/CropController -> DynamicObjectFactory` 的耕地/作物保存恢复链是存在的。
2. 已确认 `PersistentPlayerSceneBridge` 已把 `FarmTileManager` 与 `CropController` 纳入 off-scene snapshot / binding / catch-up。
3. 已确认真正高概率断层在 legacy：
   - `GameSaveData.version` 固定为 `1`
   - DTO 仍保留 `farmTiles`
   - `FarmTileSaveData` 仍保留旧 crop 字段
   - 当前读档主链没有把旧 crop 字段迁成新 `Crop` world object
4. 已确认工作台长期进度只保存 Day1 长期剧情相关状态：
   - `completedDialogueSequenceIds`
   - `woodCollectedProgress`
   - `craftedCount`
   - `workbenchHintConsumed`
   - 以及相关剧情 / 关系 / 体力生命长期态
5. 已确认工作台 runtime 队列态未正式持久化：
   - overlay 自己维护 `_queueEntries`、`_activeQueueEntry`、`readyCount`
   - overlay 不是 `IPersistentObject`
   - `ApplySpringDay1Progress()` 会把工作台制作中 runtime 字段全部清零
   - `CanSaveNow()` 只明确阻止“制作中”，未从静态代码上证明 ready outputs / floating state 已被完全阻止保存

### 关键决策
1. 后续如果要修，首刀不该重写当前 world-state 主链。
2. 最小最安全修法应优先放在 loader-side legacy normalize / migrate。
3. 工作台在打包前最小闭环上，更适合先扩大 save blocker，而不是立即做完整 queue 持久化。

### 涉及文件 / 路径
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveDataDTOs.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\PersistentObjectRegistry.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\DynamicObjectFactory.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmTileManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmTileData.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\CropController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PersistentPlayerSceneBridge.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryProgressPersistenceService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Crafting\CraftingService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SaveManagerDay1RestoreContractTests.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\WorldStateContinuityContractTests.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\StoryProgressPersistenceServiceTests.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\WorkbenchInventoryRefreshContractTests.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\FarmTileSaveRoundTripTests.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\CropSystemTests.cs`

### 验证结果
- 本轮仅做静态代码审查与合同测试阅读。
- 未改任何业务文件。
- 未跑 Unity live 验证、未做 PlayMode、未使用 MCP live 写。

### 修复后恢复点
- 如果下一轮继续主线，恢复点是：
  1. 先补 legacy 农地旧档到新 `Crop` world object 的读档迁移
  2. 再补对应合同测试
  3. 然后决定工作台是扩大 blocker 还是进入完整队列持久化

### thread-state
- 本轮性质：只读分析
- 未跑：`Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-18 12:52 只读代码审查｜restart/load 后 toolbar 固定槽位 4/8 异常

### 当前主线目标
- 仍是打包前最小安全闭环；本轮子任务是只读确认 `restart/load` 后 toolbar 固定槽位 `4/8` 异常在当前最新代码下最可能的根因链、最小安全修法和高风险顺序点。

### 本轮完成
1. 重新只读核对了限定 10 文件：
   - `SaveManager`
   - `PersistentPlayerSceneBridge`
   - `HotbarSelectionService`
   - `ToolbarUI / ToolbarSlotUI`
   - `InventoryPanelUI`
   - `InventoryInteractionManager`
   - `BoxPanelUI`
   - `PackagePanelTabsUI`
   - `GameInputManager`
2. 钉实旧记忆里的两个旧嫌疑在当前代码下已不成立：
   - `ToolbarUI` 不再靠名字推断索引，`Build()` 已按 sibling 顺序直绑 `0..11`
   - `ToolbarUI.OnDisable()` 当前已经清空 `subscribedInventory/subscribedSelection`
3. 重新确认当前最强根因不是“4/8 特判”，而是顺序层：
   - `sceneLoaded` 第一拍先按 bridge runtime snapshot 重绑 `inventory/hotbar/ui/input`
   - `SaveManager` 第二拍才落真正的 load/fresh 数据
   - 第二拍结尾只有 `RefreshAllUI()`，没有再补一轮同等级的 bridge/input authoritative rebind
4. 重新确认 `Home/NPC` 会恢复的代码解释：
   - `Home`：再次走 `RebindScene() -> RebindPersistentCoreUi() -> RebindSceneInput() -> ResetPlacementRuntimeState()`
   - `NPC`：对话会关闭阻塞 panel，并在 UI 收口后再次 `ReassertCurrentSelectionPreservingInventoryPreference()`

### 关键判断
1. 当前最新代码下，最强根因应写成：
   - `旧 runtime snapshot 先绑 -> 真正 load/fresh 数据后写 -> 收尾只做 generic refresh，没有再做 authoritative rebind`
2. `4/8` 只是最稳定暴露坏相的位置，不是当前代码里的特殊位。
3. 第一刀最安全入口仍应落在 `SaveManager` 的顺序层，不要先去改 DTO、slot widget 或整条 panel 逻辑。

### 恢复点
1. 如果下一轮允许施工，优先让 `ApplyLoadedSaveData(...)` 与 `NativeFreshRestartRoutine()` 共用一个“最终态落稳后再做一次 authoritative rebind”的收尾 helper。
2. 若这刀后仍复现，再继续查：
   - `InventoryPanelUI / BoxPanelUI` 的本地选槽镜像
   - hotbar 选中态当前不入 save payload 带来的漂移

### thread-state
- 本轮性质：只读分析
- 未跑：`Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-18 12:46 只读审查｜`scene_loaded(Home)` 后 runner 不续跑的最可能根因

### 当前主线目标
- 仍服务 `存档系统 / P0-C｜world-state 主链`；本轮子任务是只读解释：为什么 `PersistentWorldStateLiveValidationRunner` 在 `scene_loaded(Home)` 与 bridge `rebind_end scene=Home` 之后，自己没有继续打出 `scene_reached / wait_scene_poll / 后续场景步骤`，最后只剩 `watchdog_timeout phase=scene_loaded|scene=Home ...`。

### 本轮子任务
- 只读联查：
  - `Assets/YYY_Scripts/Service/Player/PersistentWorldStateLiveValidationRunner.cs`
  - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
  - `Assets/Editor/Home/PersistentWorldStateLiveValidationMenu.cs`
  - `Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs`
- 结合 live 事实：
  - runner 只打到 `scene_loaded(Home)`
  - bridge 已完整打到 `rebind_end scene=Home`
  - Play 中 3 秒后 active scene 仍是 `Home`
  - fresh console 无 error / warning
  - `WaitForScene` 轮询从 `WaitForSecondsRealtime` 改成 `yield return null` 也无变化

### 已完成事项
1. 已确认 `WaitForScene()` 的命中条件本身非常宽：
   - 只要 `active scene == Home`
   - 或 `lastSceneLoadedName == Home`
   - 或 `lastActiveSceneName == Home`
   - 任一成立，就必须先打 `scene_reached`
2. 已确认这三个信号里至少两个在 live 已成立：
   - `HandleActiveSceneChanged()` 已打 `active_scene_changed(Home)`
   - `HandleSceneLoaded()` 已打 `scene_loaded(Home)`
   - 3 秒后 active scene 仍是 `Home`
3. 已确认 bridge 的 `OnSceneLoaded -> RebindScene(scene)` 整条 Home 重绑确实完整跑完，并没有中途抛 fresh console 红黄。
4. 因此当前最强结论不是“Home 没到”或“PollInterval 太慢”，而是：
   - `WaitForScene()` 所在协程链在 `scene_loaded(Home)` 之后没有再回到下一次循环
   - 否则它最迟下一帧就该打印 `scene_reached`，最迟 1 秒内也该打印一次 `wait_scene_poll`

### 关键判断
1. 最高概率根因是“runner 的场景等待协程在 `LoadSceneMode.Single` 边界被中断 / 丢调度”，不是 `Home` 场景没切到。
2. 次高概率根因是“同一 runner 没有 singleton / run-stamp 防重入，存在隐藏二次 `StartRun()` 或并存实例把正在等场景的协程句柄停掉”，只是目前 live 事实没有第一条那么直接。
3. 低概率才是字段复用或场景事件时序本身，因为：
   - 事件字段已经被正确写成 `Home`
   - 活跃场景也已稳定是 `Home`
   - 轮询等待从 `WaitForSecondsRealtime` 换成 `yield return null` 仍无改善

### 修复后恢复点
- 如果下一轮进入真实施工，最小安全入口优先是：
  1. 把 `WaitForScene` 从“靠下一帧重新回圈发现 Home”改成“显式事件闩锁 / generation token / `yield return pendingSceneLoadOperation` 后立即判定”
  2. 再补 runner singleton / run-stamp，防止隐藏重启把活协程停掉
  3. 然后重跑 `Primary -> Home` 单段 live 票，而不是一口气扩大到全部 scenario

### thread-state
- 本轮性质：只读分析
- 未跑：`Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-18 继续施工：0417 与默认存档语义同步回写
- 当前主线没换，仍是 `存档系统 / 0417` 打包前最小收尾。
- 用户最新补充要求我把另一个子线程对 `默认存档 / F5 / F9 / 重新开始` 的语义审计结果正式吃回 `0417`，并继续沿主线推进，不能只停在聊天。

### 本轮完成
1. 已重新把 thread-state 切到：
   - `0417与默认存档语义同步回写`
2. 已修正 `0417.md` 前板与任务板的旧口径：
   - 当前统一裁定
   - `A3`
   - `E2-F`
   - `T8 / T9`
   - 任务 `12.1 / 12.3 / 12.4 / 12.5`
   - `R8-A ~ R8-F`
3. 当前这条线的固定口径已经正式写回主控板：
   - 默认槽 = 真实受保护 `默认存档`
   - `F5` = 快速保存默认存档
   - `F9` = 快速读取默认存档
   - `重新开始` = 独立危险操作，不自动改写默认存档
4. fresh CLI 验证已补：
   - `SaveManager.cs`：`assessment=no_red`
   - `PackageSaveSettingsPanel.cs`：`assessment=no_red`
   - `SaveManagerDefaultSlotContractTests.cs`：`assessment=no_red`
   - `errors --count 20`：`errors=0 warnings=0`

### 当前判断
1. 默认存档这条线现在已经不是“待设计”或“只读方案”，而是：
   - 代码层已过
   - live / packaged 待验
2. 这轮没有继续扩改 runtime 业务代码；主要是把 `0417` 与记忆层收口到能继续接力的状态。

### 下一步恢复点
1. 如果下一轮继续沿 `R8` 收尾，优先做：
   - `F5` 首次写默认槽
   - `F9` 真实读默认槽
   - 默认槽复制
   - 重新开始按钮新位置与体验
2. 不要再回到旧语义：
   - `默认开局`
   - `F5 已停用`
   - `QuickLoadDefaultSlot == RestartToFreshGame`

## 2026-04-18 00:52 只读审查｜world-state live runner bootstrap 后静默卡住

### 当前主线目标
- 继续服务 `存档系统 / P0-C｜world-state 主链`，这轮子任务是只读解释 `PersistentWorldStateLiveValidationRunner` 为什么在 `bootstrap passed` 之后静默停住、没有继续 scenario，也没有写 report。

### 本轮子任务
- 不改代码，只审：
  - [PersistentWorldStateLiveValidationRunner.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentWorldStateLiveValidationRunner.cs)
  - [FarmTileManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Farm/FarmTileManager.cs)
  - [CropController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Farm/CropController.cs)
  - [ChestController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/World/Placeable/ChestController.cs)
  - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
  - 当前 `Editor.log` 的 `WorldStateLive` 片段

### 已完成事项
1. 已核对最新 `Editor.log`：
   - 当前确实只看到 `awake / run_started / bootstrap passed`
   - 没有 `scenario_start`
   - 没有 `disabled / destroyed`
   - 没有 watchdog / finalizing / report
2. 已把控制流收窄到：
   - `RunValidation()` 从 `WaitForBootstrapTargets()` 成功返回之后
   - 到第一个 scenario 真正开始之间
3. 已核对 runner 调到的四条同步链：
   - `ChestController.SetSlot()`
   - `FarmTileManager.CreateTile() / SetWatered()`
   - `CropController.Initialize()`
   - `PersistentPlayerSceneBridge.QueueSceneEntry()`

### 关键决策 / 判断
1. 当前最高概率不是“场景切换链又坏了”，而是 runner 自己的 orchestration 漏洞：
   - `StartRun()` 先启动 `RunValidation()`，后启动 `WatchdogRun()`
   - 如果前者在第一次让出控制权前就同步卡住，后者根本来不及启动
2. 这条判断比“直接怀疑业务链”更强，因为：
   - 现象里连 `scenario_start` 都没有
   - 所以更像卡在 `post-bootstrap -> first scenario` 的前置同步窗口
3. 若要在业务链里给次级嫌疑排序：
   - 第一嫌疑：`TryPreparePrimaryState()` 内的 `CreateTile / SetWatered`
   - 第二嫌疑：`CropController.Initialize()`
   - 第三嫌疑：`ChestController.SetSlot()`
   - 当前不像已经走到 `QueueSceneEntry / LoadScene`

### 为什么会“没有错误、没有 watchdog 报告”
1. runner 没有外层 `try/catch + finalize` 兜底。
2. `WatchdogRun()` 不是先起的，而是 `RunValidation()` 之后才起。
3. `OnDisable / OnDestroy` 当前只记日志和退订事件，不会补失败 report。
4. 所以只要发生：
   - 主线程同步卡死
   - coroutine 在第一段同步区失去控制权
   - 或 runner 还没起 watchdog 就被现场打断
   就会变成“完全静默、不报错、不写 report”。

### 最小最安全修法建议
1. 第一刀只修 runner：
   - watchdog 先于 `RunValidation()` 启动
   - `bootstrap passed` 后补显式 heartbeat / log，并让出至少一帧
   - `OnDisable / OnDestroy` 对未完成 run 补兜底 report
2. 第二刀才加最小定位日志：
   - `TryPreparePrimaryState()` 内按 `chest / create_tile / set_watered / crop_initialize` 拆点
3. 当前不建议第一刀就改 bridge 或场景业务链。

### 验证结果
- 本轮性质：只读分析
- 验证层级：`静态代码 + 当前日志`
- 当前结论：`runner 自身问题为主，业务链可能只是触发源`

### 修复后恢复点
- 如果下一轮进入真实施工，恢复顺序固定为：
  1. 先修 runner 的 watchdog / finalize 兜底
  2. 再复打 world-state live runner
  3. 再根据第一条新增进度点决定是否继续查 `CreateTile / SetWatered / Crop.Initialize`

### thread-state
- 本轮性质：只读分析
- 未跑：`Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-18 连续施工｜0417打包前最小闭环总收尾继续推进

### 当前主线 / 子任务 / 恢复点
- 主线不变：继续按 `0417` 收打包前最小安全闭环。
- 本轮子任务：
  1. 收 `P0-B` 输入硬复位
  2. 补 `P0-D` Day1 canonical restore 合同测试
  3. 修 `P0-C` world-state runner 自动落失败报告
- 恢复点更新：
  - runner 现在已经会自己写失败 report
  - 下一刀若继续，不再回头修“runner 会不会写报告”，直接盯 `Home` 段 continuity / re-entry

### 本轮已完成
1. `GameInputManager.ResetPlacementRuntimeState(...)`
   - 新增 `_inputEnabled` 硬复位
   - 清旧 held / chest-held
   - 强解 `ToolActionLockManager`
   - 取消自动导航与旧农具运行态
2. `InventoryPanelUI / BoxPanelUI`
   - runtime context 重绑时会按 `selectedInventoryIndex` 重新同步选槽/follow
3. `SaveManagerDay1RestoreContractTests.cs`
   - 新增 `StoryProgressPersistenceService` / `SpringDay1Director` 的 canonical restore 合同断言
4. `PersistentWorldStateLiveValidationRunner.cs`
   - 拆出 `RunScenarioSequence()`
   - 递归推进嵌套 IEnumerator
   - 增加 `Update + Timer` 双 watchdog

### 验证结果
1. `validate_script`
   - `GameInputManager.cs`：`assessment=no_red`
   - `PersistentWorldStateLiveValidationRunner.cs`：`assessment=no_red`
   - `InventoryRuntimeSelectionContractTests.cs`：`assessment=no_red`
   - `SaveManagerDay1RestoreContractTests.cs`：`assessment=no_red`
   - `WorldStateContinuityContractTests.cs`：`assessment=no_red`
2. fresh console：
   - `errors=0 warnings=0`
3. fresh live runner：
   - 已自动写出 `Library/CodexEditorCommands/world-state-live-validation.json`
   - 最新结果：
     - `bootstrap = passed`
     - `primary-home-primary = failed`
     - `details = watchdog_timeout phase=scene_loaded|scene=Home mode=Single active=Home lastSceneLoaded=Home lastActiveScene=Home`

### 关键判断
1. `P0-B` / `P0-D` 这轮不再是纯分析，是真实代码已落地。
2. 当前 runner 旧问题已从“静默无 report”推进到“能稳定落失败票”。
3. 当前剩余更像真实 `Home` 段 re-entry / continuity 阻塞，而不是验证器本身没闭环。

### thread-state
- 本轮性质：真实施工
- 已跑：沿用当前 `存档系统` active slice（`0417打包前最小闭环总收尾`）
- 尚未跑：`Ready-To-Sync / Park-Slice`
- 补记：2026-04-18 本轮收尾前已执行 `Park-Slice`，当前 live 状态 = `PARKED`

## 2026-04-18 10:39 只读恢复点｜Day1 当前是现场占用，不是主文件硬冲突

### 当前主线目标
- 仍是 `0417` 打包前最小闭环；这轮子任务只是判断 `Day1` 现在是否真的在占用 Unity 现场，避免误把自己的 live 停滞归咎到错地方。

### 本轮子任务
- 只读核：
  - `thread-state`
  - `Day1-V3` 最新 memory
  - `mcp-single-instance-occupancy.md`
  - `mcp-live-baseline.md`
  - `Library/CodexEditorCommands/status.json`
  - `Library/CodexEditorCommands/archive/*.cmd`
  - `spring-day1-live-snapshot.json`
  - `spring-day1-actor-runtime-probe.json`

### 已完成事项
1. 已确认 `Day1-V3` 线程状态为 `ACTIVE`。
2. 已确认 Unity 当前确实在 `PlayMode`：
   - `status.json` = `isPlaying=true`
   - `lastCommand=playmode:EnteredPlayMode`
3. 已确认最近被桥消费的命令就是 Day1 验证链：
   - `Bootstrap Spring Day1 Validation`
   - `Reset Spring Day1 To Opening`
   - `Force Spring Day1 Dinner Validation Jump`
   - `Write Spring Day1 Live Snapshot Artifact`
   - `Write Spring Day1 Actor Runtime Probe Artifact`
4. 已确认 Day1 结果文件在同一时间刚被刷新：
   - `spring-day1-live-snapshot.json` 10:38:33
   - `spring-day1-actor-runtime-probe.json` 10:38:34

### 关键判断
1. 现在要严谨地区分两件事：
   - `文件语义冲突`：暂时没看到我和 Day1 在我当前主链核心文件上的直接 shared touchpoint 冲突。
   - `Unity 现场冲突`：成立，而且是当前实时成立。
2. 所以我不是“完全不能继续”，而是：
   - 只读分析可以继续
   - 但任何新的 live runner / PlayMode / MCP live 验证，这一刻都不该接着跑
3. 对我下一步最关键的影响是：
   - `world-state-live-validation.json` 的下一刀本来就需要重新进 Unity 取 `Home` 段真票
   - 这一步必须等 Day1 退出当前运行态后再做，否则现场会互相污染

### 验证结果
- 本轮性质：只读分析
- 结论等级：`现场命令归档 + 结果文件时间戳 + status.json` 交叉成立

### 修复后恢复点
- 如果下一轮继续我的主线，先重新确认：
  1. `status.json.isPlaying=false`
  2. `Day1` 不再持续消费新的 validation `.cmd`
  3. 再回到 `world-state` 的 `Home` 段 re-entry 真票

### thread-state
- 本轮性质：只读分析
- 未跑：`Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`
## 2026-04-18 真实施工｜0417 四问题收口：Town 首次遮挡 / House 4_0 / restart Town / 0.0.4 workbench
- 用户目标：
  1. 首次启动直进 `Town` 时，树和房屋必须正常遮挡玩家
  2. `House 4_0` 不能再闪一下就失效
  3. `重新开始` 后 `Town` 世界状态要像 `Primary` 一样回到 fresh authored baseline
  4. `0.0.4` 到工作台附近必须能正常接起 briefing，不再靠挤 `001`
- 本轮真实改动：
  - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
  - [OcclusionManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs)
  - [SaveManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs)
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
  - [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity)
  - [WorldStateContinuityContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs)
  - [SaveManagerDay1RestoreContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs)
- 关键修口：
  1. `PersistentPlayerSceneBridge` 首屏 fallback 后补一轮 `RefreshSceneRuntimeBindingsInternal(activeScene)`，并在这条链里补 `RebindSceneOcclusionManagers`
  2. `OcclusionManager` 新增 runtime player / current scene occluders 主动补绑
  3. `Town.unity` 只删 `House 4_0` scene-level 额外 `OcclusionTransparency`
  4. `SaveManager.NativeFreshRestartRoutine()` 在载入 `Town` 前补 `SuppressSceneWorldRestoreForScene`，失败分支补 `CancelSuppressedSceneWorldRestore`
  5. `SpringDay1Director` 把 workbench escort 的玩家距离改成 `chief/workbench` 取小值，并在玩家已经到工作台区域时强制接 briefing 兜底
- fresh 验证：
  - `validate_script`
    - `PersistentPlayerSceneBridge.cs`：`owned_errors=0 external_errors=0`
    - `OcclusionManager.cs`：`owned_errors=0 external_errors=0`
    - `SaveManager.cs`：`owned_errors=0 external_errors=0`
    - `SpringDay1Director.cs`：`owned_errors=0 external_errors=0`
    - assessment 统一为 `unity_validation_pending`，原因是 Unity `stale_status`
  - `errors --count 20`：`errors=0 warnings=0`
  - `EditMode` 目标 9 票：`9/9 passed`
- 当前判断：
  1. 这四条线现在都是代码层已落地，不再是只读猜测
  2. 当前剩余只该看 live 终验，不该再在这轮继续扩代码
- 下一步恢复点：
  1. 首次启动直进 `Town`，看遮挡是否恢复
  2. 单独复测 `Town / House 4_0`
  3. `重新开始 -> Town`，看树石和世界状态是否 truly fresh
  4. `重新开始 -> 推到 0.0.4 -> 靠近工作台`，看是否自动接起 briefing
## 2026-04-18 遮挡冷启动主问题收口｜Tool 子物体抢 Player 标签
- 用户目标：先修“全场启动不遮挡”的主问题，不再先纠缠 `House 4_0`。
- 本轮主因：确认 `Town` / `Primary` 里都存在 `Tool` 子物体也被打成 `Player` 标签，`OcclusionManager` 冷启动用 `FindGameObjectWithTag("Player")` 很容易先抓到这个假目标。
- 已做修口：
  - `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`
    - 改成扫描所有 `Player` 候选，再按 `PlayerAnimController / PlayerMovement / PlayerController / usable sprite / sorting` 选真正玩家本体
    - `ResolvePlayerSprite()` 优先吃 `PlayerAnimController.BodySpriteRenderer`
    - `HasUsablePlayerBindings()` 不再把不可用的 `Tool` renderer 当成已绑定
  - `Assets/YYY_Scripts/Anim/Player/PlayerAnimController.cs`
    - 补了 `BodySpriteRenderer` 公开 getter
  - `Assets/YYY_Tests/Editor/OcclusionSystemTests.cs`
    - 增了 `RefreshPlayerBindings_PrefersRealPlayerRootOverTaggedToolChild` 护栏
- 自检结果：
  - 三个改动脚本 `validate_script` 全部 `errors=0`
  - `compile` 被 `dotnet` 60s timeout 卡住，属于工具超时，不是脚本编译红
  - 上轮被打断测试留下的 `Assets/__CodexEditModeScenes` 临时残留已手动清掉
- 恢复点：
  - 下一步只该继续补最小 live / 单测验证，优先确认冷启动首屏遮挡是否恢复
  - 当前已新增最小遮挡护栏票 `OcclusionSystemTests.RefreshPlayerBindings_PrefersRealPlayerRootOverTaggedToolChild = passed`
  - 清理 `Assets/__CodexEditModeScenes` 后，测试控制台不再出现 `Files generated by test without cleanup`
## 2026-04-18 真实施工｜House 4 单点遮挡再收口
- 当前主线目标：继续把打包前遮挡尾账收平，但这轮只收 `Town / House 4_0` 单点异常，不再回头改全场或扩算法。
- 本轮子任务：核实 `House 4` 到底是 `Town` scene override 问题，还是 prefab 内部结构冲突；在最小范围内直接修掉。
- 关键纠偏：
  1. 重新审计后确认：`Town.unity` 里的 `House 4` 没有额外多挂遮挡脚本。
  2. 真问题在 `Assets/222_Prefabs/House/House 4.prefab`：嵌套子物体 `House 4 柱子_0` 被 prefab 实例层额外挂了一份 `OcclusionTransparency`。
  3. 这轮已删掉这 1 份 added `OcclusionTransparency`，保留柱子的 sprite / collider，不碰 `Town.unity`、不碰 `House 4 柱子_0.prefab` 源资源。
- 涉及路径：
  - `Assets/222_Prefabs/House/House 4.prefab`
- 验证结果：
  - `manage_prefabs get_hierarchy` 已确认 `House 4 柱子_0` 不再带 `OcclusionTransparency`
  - `refresh_unity(scope=assets)` 成功
  - `read_console(error+warning)=0`
  - `git diff --check` 对目标 prefab 通过
- 当前恢复点：
  1. 主线已回到“等用户 live 终验 House 4 单点”
  2. 如果用户仍反馈 `House 4` 异常，再继续看“建筑多分片是否需要共享父根 occlusion root”；这一刀先不扩大
## 2026-04-18 真实施工｜House 4 纹理 warning + BatchHierarchy own warning 收口
- 当前主线目标：继续把打包前遮挡与 own warning 尾账收平；这轮在 `House 4` 单点修完后，补收用户刚贴出的两类警告。
- 本轮子任务：
  1. 让 `House 4_0` 的贴图不再因 `isReadable=0` 刷 warning
  2. 让 `Tool_002_BatchHierarchy` 不再在打开窗口时自己拉出 `DontDestroyOnLoad / 交叉场景引用` warning
- 关键改动：
  1. `Assets/Sprites/Generated/House 4.png.meta`
     - `TextureImporter.isReadable` 改为 `1`
  2. `Assets/Editor/Tool_002_BatchHierarchy.cs`
     - 用 `scene.path + sibling-index hierarchy path` 取代 `GlobalObjectId` 持久化/恢复
     - `LoadPersistedSelection()` 不再调用 `GlobalObjectIdentifierToObjectSlow`
- 验证结果：
  - `validate_script`：`Tool_002_BatchHierarchy.cs` `errors=0`
  - 清空 Console 后重开 `Tools/002批量 (Hierarchy窗口)`，warning 未复现
  - `House 4.png.meta` 已确认 `isReadable: 1`
  - 最终 `read_console(error+warning)=0`
  - `git diff --check` 通过
- 当前恢复点：
  1. 这轮 own warning 已收口，可回到用户 live 验
  2. 如果后续再见到新的同类 warning，优先先判“是贴图 importer 还是 Editor 工具恢复路径”，不再把两类问题混着查
## 2026-04-19 真实施工｜楼梯层级切换脚本最小落地
- 当前主线目标：
  - 按用户刚追加的新需求，先落一个完全独立、可直接挂在楼梯区域上的最小脚本，不继续扩成泛导航系统。
- 本轮子任务：
  - 新增楼梯层级切换脚本，并把最关键的误判边界一起收住。
- 实际完成：
  - 新增 [StairLayerTransitionZone2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/StairLayerTransitionZone2D.cs)
  - 新增 [StairLayerTransitionZone2DTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/StairLayerTransitionZone2DTests.cs)
  - 语义固定为：
    1. 用一个 trigger 覆盖整块楼梯平面
    2. 进入不切层
    3. 仅从上/下边界离开时切 `Unity Layer + Sorting Layer`
    4. 左右离开忽略，靠空气墙约束
  - 关键补强：
    1. 脚本会自动确保 collider 是 trigger
    2. 切层会同步到玩家根、所有子物体、`SpriteRenderer`、`SortingGroup`
    3. 边界判定优先吃玩家根本体 collider，避免工具或其他 child collider 把出口方向带偏
    4. 保留多碰撞器 overlap 计数，只在最后一次真正离开时切层
    5. `OnDisable()` 会清 overlap 状态
- 验证：
  - `mcp validate_script` 两文件均 `0 errors / 0 warnings`
  - `python scripts/sunset_mcp.py validate_script` 两文件均 `owned_errors=0 external_errors=0`
  - `git diff --check` 通过
  - 当前没拿到完整 Unity ready 绿票，原因是现场 `stale_status`，不是这刀留红
- 阻塞 / 恢复点：
  - `Ready-To-Sync` 被 `D:\Unity\Unity_learning\Sunset\.kiro\state\ready-to-sync.lock` 卡住
  - 已执行 `Park-Slice`
  - 下次恢复时只做两件事：
    1. 在目标楼梯对象上挂脚本并填上下出口层
    2. 做最小实机三票：上离开升层、下离开降层、左右离开不切层
## 2026-04-19 只读审计：正式存档/读档/重新开始 覆盖范围与高风险漏项
- 当前主线目标没变，仍是存档系统收口；本轮子任务是只读审计“正式存档 / 读档 / 重新开始”到底覆盖了哪些世界状态与玩家状态，以及最该先补哪几条真缺口。

### 本轮完成
1. 静态核对了：
   - `SaveManager.cs`
   - `SaveDataDTOs.cs`
   - `PersistentPlayerSceneBridge.cs`
   - `StoryProgressPersistenceService.cs`
   - `FarmTileManager.cs`
   - `CropController.cs`
   - `ChestController.cs`
   - `WorldItemPickup.cs`
   - `CraftingService.cs`
   - `SpringDay1WorkbenchCraftingOverlay.cs`
   - `InventoryService.cs`
   - `HotbarSelectionService.cs`
   - `EquipmentService.cs`
2. 已确认正式覆盖成立的域：
   - 时间
   - 玩家位置 / scene
   - 当前 loaded scene 的 registry world objects
   - off-scene 的 `FarmTile / Crop / Chest / Drop / Tree / Stone`
   - 剧情长期态（phase / dialogue completion / Day1 关键导演进度 / health / energy / NPC relationship）
   - 云影持久态
3. 已确认高风险漏项 / 半覆盖：
   - `EquipmentService` 有 `Save/Load` 但没进正式 registry 主链
   - `PlayerSaveData` 里 `selectedHotbarSlot / gold / stamina / maxStamina / currentLayer` 只是 DTO 字段，主链没真正写回
   - `HotbarSelectionService` 只在 bridge session snapshot 里暂存，不入正式 save 文件
   - `ChestController` 只存内容和 `isLocked`，没存 `origin / ownership / hasBeenLocked / currentHealth`
   - `WorldItemPickup` 没存 `runtimeItem` 的动态属性 / 耐久 / 实例态
   - workbench 队列与待领取产物不是正式持久化，而是靠 save blocker 禁止中途存档
   - off-scene resident runtime snapshot 只在 bridge 内存里，不在正式 save 文件里

### 最关键判断
- 当前最像“用户一读档就觉得坏了”的不是农田主链，而是三条数据合同缺口：
  1. 装备栏不回档
  2. 世界箱子规则态不回档
  3. 掉落物 runtime item 被压扁成普通堆叠物
- `toolbar / hotbar` 选中丢失也很容易被用户感知，但它更像第四优先级：
  - 直接错手感
  - 但破坏性弱于上面三条

### 下一步恢复点
1. 如果下一轮进入真实施工，最小优先级应是：
   - `EquipmentService` 正式入盘
   - `ChestController` 补规则态字段
   - `WorldItemPickup` 补 runtime item DTO
2. 这轮只是静态审计，不进入代码修改，不跑 `Begin-Slice / Ready-To-Sync / Park-Slice`。
3. 当前 live 状态：保持 `PARKED`
## 2026-04-19 续工补记｜剧情读档/重开 blocker 再收口
- 当前主线没有变化，仍是打包前的存档闭环。
- 本轮真实施工：
  1. `SaveManager` 新增 `CanExecutePlayerRestartAction()`，`RestartToFreshGame()` 现在先过剧情/场景恢复 blocker。
  2. `SpringDay1Director.TryGetStorySaveLoadBlockReason()` 新增 `StoryPhase.DayEnd`，第一夜睡眠收束与回家安置未落稳前不放行读档/重开。
  3. `SaveManagerDay1RestoreContractTests` / `SaveManagerDefaultSlotContractTests` 已补对应合同护栏。
- 本轮现实样本核对：
  1. 直接扫了 `C:\Users\aTo\AppData\LocalLow\DefaultCompany\Sunset\Save`。
  2. 当前现存 `__default_progress__.json` 与普通槽位都属于旧格式：
     - 有 `StoryProgressState`
     - 有 `PlayerInventory`
     - 无 `EquipmentService`
  3. 结论：旧档缺装备栏合同是历史现实，不能把这类旧档粗暴判成不可读；正确口径是“新写盘拒绝半截档，旧读档保持兼容”。
- 本轮验证：
  1. `manage_script validate`
     - `SaveManager.cs`：`errors=0`，仅旧 warning 3 条
     - `SpringDay1Director.cs`：`errors=0`，仅旧 warning 3 条
     - 两个合同测试文件：`clean`
  2. `errors --count 20`：`0 errors / 0 warnings`
  3. `git diff --check`：通过
- 当前未闭环：
  1. 还没拿到 live 终验，不能假装 DayEnd/导演态拦截已经玩家实测通过。
 2. Unity compile-first 仍被 `stale_status / codeguard timeout` 干扰，这轮只能诚实报“代码层 clean，live 待验”。

## 2026-04-20｜只读排查：Day2 后仍无法新建/读取存档
- 当前主线目标：继续把存档系统在打包前收成真实可用的 save/load/restart 语义。
- 本轮子任务：不改代码，只读核对 `SaveManager`、`StoryProgressPersistenceService`、`SpringDay1Director`、`PackageSaveSettingsPanel` 的 `can save/load/new slot blocker` 链，回答“Day14 已进 Day2 仍不能新建/读档”的最可能主根因与最小安全修法。
- 子任务服务于什么：给下一刀真正修 blocker 时一个足够窄的入口，避免误把锅甩给 UI 或 `SaveManager` 顶层。
- 修复后恢复点：如果下一轮进入真实施工，先回到 `SpringDay1Director.IsDayEndSceneSettlePending()` / `TryGetStorySaveLoadBlockReason()` 这组 DayEnd 退出逻辑，不要先散到 `SaveManager` 或存档 DTO。

### 只读结论
1. 主根因更像“剧情状态未正确退出”：
   - `HandleSleep()` 会把 `_dayEnded = true` 且 `StoryManager.CurrentPhase = StoryPhase.DayEnd`。
   - 但当前正常运行链里没有看到 Day2 常态入口去清掉这组 Day1 日终态。
2. 真正把 Day2 后 save/load 继续挡住的条件点在 `SpringDay1Director.IsDayEndSceneSettlePending()`：
   - 只要 `_dayEnded` 为真且当前不在 `Home`，它就直接返回 `true`。
   - 所以第二天一旦离开 `Home`，`TryGetStorySaveLoadBlockReason()` 仍把状态解释成“第一夜收束未完成”。
3. `新建普通槽位` 和 `读取存档` 被挡是同一主因的两个入口：
   - `CreateNewOrdinarySlotFromCurrentProgress()` -> `SaveGame()` -> `CanSaveNow()`
   - `LoadGame()/QuickLoadDefaultSlot()` -> `CanLoadNow()`
   - 两边都会走到 `director.TryGetStorySaveLoadBlockReason(...)`
4. UI 层确实有误导，但不是主根因：
   - `PackageSaveSettingsPanel.RefreshView()` 只看 `CanExecutePlayerSaveAction()`，顶部会写成“可读但不可写”，实际 `load` 可能同样被拦。
   - `HandleDefaultLoad()/HandleLoad()` 失败 toast 只有“默认存档读取失败 / 读档失败”，没有把真实 blocker reason 透出。

### 最小安全修法建议
1. 第一刀优先只改 `SpringDay1Director`：
   - 把 `IsDayEndSceneSettlePending()` 收窄成“只在真正日终收束窗口还没结束时返回 true”，例如仅覆盖强制摆位/切场收尾尚未完成的帧。
   - 不要继续用 `!IsHomeSceneActive()` 作为永久 blocker。
2. 第二刀再补 UI 反馈：
   - 顶部状态文案分开显示 `save` / `load`。
   - 读档失败 toast 透传真实 blocker reason。
3. 更长期但更正宗的方向：
   - 给 DayEnd 增加一条 Day2 常态退出口，避免 `StoryManager` / `SpringDay1Director` 长期停留在 Day1 `DayEnd` 语义。

### 证据文件
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryProgressPersistenceService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Save\PackageSaveSettingsPanel.cs`

### 验证状态
- 本轮是只读分析，未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`。
- 当前判断状态：`静态推断成立`
- 当前 live 状态：`PARKED`

## 2026-04-20 只读排查：默认存档读取失败链
- 当前主线目标：继续收口存档系统，优先把玩家会感知成“默认存档读失败”的链路误导钉死。
- 本轮子任务：只读检查 `SaveManager`、`PackageSaveSettingsPanel`、默认槽 UI 入口、`F9`、默认存档文件路径/摘要、读档 blocker 与错误提示链，不改代码。
- 本轮服务于什么：为下一刀最小修法缩小到“提示链/入口链”而不是误回大改 save DTO 或读档主流程。
- 本轮后恢复点：如果后续进入真实施工，第一刀只改 `PackageSaveSettingsPanel` 的 load blocker 前置提示与状态文案，不先动 `SaveManager` 的正式读档实现。

### 本轮完成
1. 核对了 `SaveManager` 的默认槽主链：
   - `Update()` 中 `F9 -> ExecuteQuickLoadHotkey()`
   - `LoadGame()` / `QuickLoadDefaultSlot()`
   - `TryGetDefaultSlotSummary()` / `TryReadSaveData()` / `GetSaveFilePath()`
2. 核对了设置页默认槽入口：
   - `PackagePanelTabsUI.OpenSettings()` / `PackageSaveSettingsPanel.EnsureInstalled()`
   - `PackageSaveSettingsPanel.RefreshView()` / `HandleDefaultLoad()` / `HandleLoad()`
3. 回看了读档 blocker 权威来源：
   - `StoryProgressPersistenceService.CanLoadNow()`
   - `SpringDay1Director.TryGetStorySaveLoadBlockReason()`
   - `SaveManagerDefaultSlotContractTests`
   - `SaveManagerDay1RestoreContractTests`

### 最关键判断
1. “默认存档读取失败”当前最可能的单一主根因，不是默认存档文件路径，也不是 `SaveManager` 还在把默认槽偷转成重新开始。
2. 真正最像的根因是设置页默认槽 UI 把“当前不可读的 blocker”误包装成了统一失败：
   - `RefreshView()` 用 `CanExecutePlayerSaveAction()` 写头部状态，不代表默认槽现在可读。
   - `_defaultLoadButton` 只按摘要是否可读启用，不看 `CanExecutePlayerLoadAction()`。
   - `HandleDefaultLoad()` 不在确认前前置报 blocker，确认后直接把任何 `false` 压成“默认存档读取失败”。
3. `F9` 自己不是这条主根因：
   - `ExecuteQuickLoadHotkey()` 已先报“空默认档”或真实 `blockerReason`
   - 只有真正通过前置检查后再读文件失败时，`F9` 才会落到通用失败 toast。

### 涉及文件 / 方法
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
  - `LoadGame()`
  - `CanExecutePlayerLoadAction()`
  - `QuickLoadDefaultSlot()`
  - `TryGetDefaultSlotSummary()`
  - `LoadGameInternal()`
  - `TryReadSaveData()`
  - `ExecuteQuickLoadHotkey()`
  - `GetSaveFilePath()`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Save\PackageSaveSettingsPanel.cs`
  - `RefreshView()`
  - `HandleDefaultLoad()`
  - `HandleLoad()`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Tabs\PackagePanelTabsUI.cs`
  - `Awake()`
  - `OpenSettings()`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryProgressPersistenceService.cs`
  - `CanLoadNow()`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `TryGetStorySaveLoadBlockReason()`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SaveManagerDefaultSlotContractTests.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SaveManagerDay1RestoreContractTests.cs`

### 验证与结论
- 默认档路径：
  - 仍是 `Application.persistentDataPath/Save/__default_progress__.json`
  - 旧 `项目根/Save` 与 `Assets/Save` 只是迁移兼容
- 摘要链：
  - 与正式读档共用 `GetSaveFilePath()` / `TryReadSaveData()`
  - 若摘要已正常显示，路径错误不是这次最像的首嫌
- 静态结论：
  - `SaveManager` 读默认档主链成立
  - 最像的误导点在 `PackageSaveSettingsPanel` 的 load blocker 前置缺失与错误提示压平
- 本轮未跑测试、未改代码；验证状态：`静态推断成立`

### 最小安全修法建议
1. 只改 `PackageSaveSettingsPanel`，不改 `SaveManager` 的文件读写/scene switch 主链。
2. 默认槽和普通槽在点击前统一先判 `manager.CanExecutePlayerLoadAction(out blockerReason)`：
   - 有 blocker：直接 toast 真实原因
   - 无 blocker：再继续确认弹窗与现有读档调用
3. `RefreshView()` 把默认槽状态文案改成“摘要状态 + load blocker”双态，避免继续写出“可读但不可写”这种会误导用户的话。
4. 补一条面板层合同测试，钉死“命中 load blocker 时必须报 blocker，不得统一显示读取失败”。

### thread-state
- 本轮始终只读：
  - `Begin-Slice`：未跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：未跑
- 当前 live 状态：保持 `PARKED`

## 2026-04-20 真实施工：Day2 存档 blocker 退场 + 设置页真实 load blocker 透传
- 当前主线目标：继续按 `0417` 收口存档系统，把“默认存档无法读取 + Day2 后仍不能新建/读档”直接修到代码层闭环。
- 本轮子任务：沿既有 `ACTIVE` slice `0420_save-default-slot-and-day2-gate-fix` 直接真实施工：
  1. 修 `SpringDay1Director` 的 `DayEnd` 退出口
  2. 修 `PackageSaveSettingsPanel` 的 `save/load` 状态与 blocker 提示链
  3. 补合同护栏和最小闸门
- 子任务服务于什么：把玩家当前最痛、最阻塞打包体验的存档坏相先收掉，不再继续停在只读分析。
- 本轮后恢复点：下一轮只需要优先拿 live 票验证：
  1. `Day2 / D14` 是否已能 `新建普通存档 / 读取普通存档 / 读取默认存档`
  2. 左侧 Day1 任务栏是否已退出

### 本轮实际完成
1. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
   - 新增 `_dayEndSceneSettlePending`
   - `HandleSleep()` 进入 `DayEnd` 时显式置位
   - `IsDayEndSceneSettlePending()` 改成只看真正的瞬时收束态，不再把“离开 Home”本身当永久 blocker
   - 新增 `IsManagedDay1RuntimeCurrentlyActive()` 与 `ShouldPersistCurrentRuntimeState()`
   - `IsStoryRuntimeSceneActive()` 现在会在 Day2 常态退场
   - `TryResolvePlayerFacingPhase()` 跟着 runtime 窗口退场，避免第二天继续挂 Day1 任务/提示
2. `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
   - `CaptureSpringDay1Progress()` 只在 Day1 runtime 仍有效时才保存 Day1 临时快照
3. `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - `RefreshView()` 现在分开读 `canSave / canLoad / canRestart`
   - 顶部状态会直接报“当前可保存但不可读取 / 当前可读取但不可保存 / 当前都不可用”
   - 默认槽摘要在“文件存在但当前被 blocker 拦住”时会显示真实 blocker
   - 默认槽/普通槽加载按钮同时服从 `canLoad`
   - `HandleDefaultLoad()` / `HandleLoad()` 在确认前先报真实 blocker，不再统一 toast “读取失败”
4. 合同护栏
   - `Assets/YYY_Tests/Editor/SaveManagerDefaultSlotContractTests.cs`
   - `Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`

### 本轮验证
1. `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs Assets/YYY_Tests/Editor/SaveManagerDefaultSlotContractTests.cs Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
   - 通过
2. `manage_script validate`
   - `SpringDay1Director`：`warning(3)`，无 error
   - `StoryProgressPersistenceService`：`clean`
   - `PackageSaveSettingsPanel`：`clean`
   - 两份测试：`clean`
3. `errors --count 20 --output-limit 10`
   - `0 errors / 0 warnings`
4. `compile` 与部分 `validate_script`
   - 仍被 `CodexCodeGuard timeout / stale_status` 卡住
   - 当前可诚实宣称：
     - `代码层 clean`
     - `fresh console clean`
     - `Unity compile-first 绿票待补`

### 当前判断
1. 这轮不是“又补一条只读结论”，而是最小安全修法已经落地。
2. 主根因是 `DayEnd` blocker 没退场，设置页只是第二层误导；两条都已经收。
3. 当前最大剩余是 live 终验，不是方向不清。

### thread-state
- 本轮沿既有 slice 继续真实施工：
  - `Begin-Slice`：沿用已存在 `ACTIVE` slice
  - `Ready-To-Sync`：未跑（本轮未进入 sync）
  - `Park-Slice`：已跑
- 当前 live 状态：`PARKED`
- 当前 blocker：
  - `Day2/默认存档 live 终验待补`
  - `Unity compile-first 仍受 CodexCodeGuard timeout / stale_status 阻塞`

## 2026-04-20 回滚记录：撤回我刚才那刀 Day2 blocker 修法
- 用户直接反馈我刚才那刀已经把现场带坏：
  1. 玩家从屋子里出来的位置不对
  2. 允许存档的位置/时机不对
  3. 剧情提示不对
  4. 整体状态错乱
- 我复盘后确认，这次犯错不在于“只改了设置页文案”，而在于我把存档修法越线带进了 `SpringDay1Director` 的剧情导演层：
  - 改了 `DayEnd` 收束判断
  - 改了 Day1 何时仍算 runtime active
  - 改了任务卡/提示层何时继续接管
  - 改了 Day1 临时快照何时继续写进正式存档
- 这是错误的刀口选择，因为它直接碰了“什么时候还算 Day1 / 玩家什么时候算已经从第一夜收出来”的剧情合同。

### 我这次具体做错了什么
1. 在 `SpringDay1Director.cs` 里新增了：
   - `_dayEndSceneSettlePending`
   - `ShouldPersistCurrentRuntimeState()`
   - `IsManagedDay1RuntimeCurrentlyActive()`
2. 并用这些新判断改了：
   - `TryResolvePlayerFacingPhase()`
   - `HandleSleep()`
   - `IsDayEndSceneSettlePending()`
   - `TryFinalizePendingForcedSleepRestPlacement()`
   - `IsStoryRuntimeSceneActive()`
3. 在 `StoryProgressPersistenceService.cs` 里给 `CaptureSpringDay1Progress()` 加了新的 Day1 runtime 门槛。
4. 在 `PackageSaveSettingsPanel.cs` 里改了：
   - `save/load` 双态 blocker 文案
   - 默认槽/普通槽加载按钮条件
   - `HandleDefaultLoad()` / `HandleLoad()` 前置 blocker 透传
5. 又补了两份合同测试去护这些改动。

### 为什么这些改动会把现场带坏
1. 我本来想收的是“Day2 后 save/load blocker 没退干净”。
2. 但我实际碰到的是“Day1 导演层什么时候还在接管”的底层判定。
3. 这会连带改掉：
   - 出屋/回屋一类依赖导演收束的行为
   - 左侧任务卡和剧情提示何时显示/隐藏
   - Day1 临时快照何时写入正式存档
4. 所以它不只是修了存档口，而是改了剧情语义本身，这就是这轮失手的核心。

### 我已经怎么处理
1. 已重新 `Begin-Slice`
2. 已把我这轮新增的上述改动全部撤回：
   - `SpringDay1Director.cs`
   - `StoryProgressPersistenceService.cs`
   - `PackageSaveSettingsPanel.cs`
   - 两份对应合同测试
3. 回退后最小自检结果：
   - `git diff --check`：通过
   - `SpringDay1Director`：仅旧 warning 3 条，无新 error
   - `StoryProgressPersistenceService`：clean
   - `PackageSaveSettingsPanel`：clean
4. 当前我已 `Park-Slice`

### 回滚后的当前状态
1. 我这轮引入的 story/save 错误改动已经撤掉。
2. 当前 relevant 文件里仍有别的 dirty，但不是我这轮新增：
   - `SpringDay1Director.cs` 还存在他线已有的改动
   - 我已明确避开未动
3. 当前 blocker：
   - `需用户复测：出屋位置 / 可存档时机 / 剧情提示是否恢复`
   - `SpringDay1Director 仍有他线既存脏改，已避开未动`

## 2026-04-20｜只读梳理 Day1 剧情进行中判断节点

### 当前主线目标
- 继续服务 `存档系统` 主线，但本轮子任务已经从“修 Day2 blocker”切回成纯只读：
  - 不改 `Day1`
  - 只把“剧情是否正在进行、哪些节点会拦存/读档、正式存档该保存什么”查成清单

### 本轮完成
1. 重新核对了 `StoryPhase`、`StoryManager`、`StoryProgressPersistenceService`、`SpringDay1Director`、`SpringDay1WorkbenchCraftingOverlay`、`SaveDataDTOs`。
2. 钉实了顶层判断链：
   - `CanSaveNow()/CanLoadNow()`
   - 正式对白
   - NPC 闲聊
   - `TryGetStorySaveLoadBlockReason(...)`
   - `save` 专属工作台 blocker
3. 把 `Day1` 各阶段 blocker 逐条钉实：
   - `CrashAndMeet` 整段拦
   - `EnterVillage` 看 `TownHouseLead / FirstFollowup / HouseArrival 完成态`
   - `HealingAndHP` 看 `HealingBridge / HealingSequence 完成态`
   - `WorkbenchFlashback` 看 `WorkbenchBriefing / _workbenchOpened / WorkbenchSequence 完成态`
   - `FarmingTutorial` 只拦“和村长收口”这拍
   - `DinnerConflict` 看 `ReturnEscort / Dinner pending / Dinner 完成态`
   - `ReturnAndReminder` 看 `Reminder pending / Reminder 完成态`
   - `FreeTime` 只拦 `FreeTimeIntroPending`
   - `DayEnd` 当前不在正式剧情 blocker 分支
4. 把正式存档应保存的合同重新收成 7 类：
   - 时间
   - 玩家位置与选槽来源
   - 剧情长期态
   - Day1 运行进度
   - 血量/精力
   - scene worldObjects
   - off-scene world snapshots

### 关键决定
1. 后续如果只是判断“现在能不能存/读”，不要再自己发明一套剧情判定。
2. 最稳口径是：
   - 先直接吃 `StoryProgressPersistenceService.CanSaveNow()/CanLoadNow()`
   - 真要拆原因，再展开到各 phase 节点
3. `DayEnd` 当前没有进 `TryGetStorySaveLoadBlockReason()` 的正式 blocker 分支，这条必须如实记住，不能再按旧猜测乱补。

### 涉及文件
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Data\StoryPhase.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryProgressPersistenceService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveDataDTOs.cs`

### 验证状态
- `静态代码核对成立`
- `本轮未改业务代码`
- `本轮未跑 Begin-Slice`

### 下一步恢复点
- 如果用户下一轮要继续做“剧情时禁止存/读”的修法，必须先沿这份只读清单走，不再直接碰 `Day1` 导演层。

## 2026-04-20｜Day1 最小时间窗与新建普通槽提示已施工

### 当前主线目标
- 继续服务 `存档系统` 主线，但本轮真实施工只收用户新裁定的最小刀：
  1. Day1 存/读档时间窗
  2. 新建普通槽失败提示

### 本轮完成
1. 已跑 `Begin-Slice`，切片名：
   - `Day1存档时间窗最小修法`
2. `SpringDay1Director.cs`
   - 在 `TryGetStorySaveLoadBlockReason(...)` 前新增 `IsDay1SaveLoadWindowOpen(currentPhase)`
   - 仅在 `Year1 / Spring / Day1` 生效
   - `0.0.6` 打开后 `16:01 ~ 17:59` 放行
   - `19:31+` 放行
3. `PackageSaveSettingsPanel.cs`
   - 当前“新建存档”行为已对齐目标语义
   - `_newSlotButton` 在 `SaveManager` 存在时保持可点
   - `HandleNewSlot()` 先显式检查 `CanExecutePlayerSaveAction(out blockerReason)`
   - 不能存时直接 toast 原因
4. 两份合同测试已补文本护栏：
   - `SaveManagerDay1RestoreContractTests.cs`
   - `SaveManagerDefaultSlotContractTests.cs`

### 关键决定
1. 这轮没有再重做 Day1 全阶段语义，只让最小放行窗优先于阶段 blocker。
2. 时间窗未命中时，仍沿用原 blocker 的阶段提示，不把用户提示改成纯时间文案。
3. 新建存档只补“点了有反馈”，没有顺手扩到“覆盖”按钮，保持最小刀口。

### 验证状态
1. `validate_script` 4 个目标文件：
   - `owned_errors=0`
   - `external_errors=0`
2. `git diff --check --` 本轮 4 个目标文件：通过
3. 当前 compile-first 仍是：
   - `unity_validation_pending`
   - 原因：Unity `stale_status`

### 涉及文件
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Save\PackageSaveSettingsPanel.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SaveManagerDay1RestoreContractTests.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SaveManagerDefaultSlotContractTests.cs`

### 本轮收尾状态
- `Park-Slice`：已执行
- 当前 live 状态：`PARKED`
- 当前最直接待用户复测点：
  1. Day1 `0.0.6` 的 `16:01 ~ 17:59`
  2. Day1 `19:31+`
  3. 当前不能存时点击“新建存档”是否会立刻弹出 blocker 提示

## 2026-04-20｜只读彻查 工作台跨场景丢失 / 存档加载丢失

### 当前主线目标
- 继续服务 `存档系统` 主线，但本轮子任务被用户收窄为纯分析：
  - 工作台有内容时切场回来为什么会丢
  - 存档加载为什么也会丢
  - 应该怎样最安全、最彻底地闭环

### 本轮完成
1. 已按 `skills-governor` + `sunset-workspace-router` 做手工等价前置核查：
   - 本轮只读
   - 不跑 `Begin-Slice`
   - 工作区仍归 `存档系统`
2. 重新核对了：
   - `CraftingService.cs`
   - `SpringDay1WorkbenchCraftingOverlay.cs`
   - `CraftingStationInteractable.cs`
   - `StoryProgressPersistenceService.cs`
   - `PersistentPlayerSceneBridge.cs`
   - `SaveDataDTOs.cs`
   - `SaveManager.cs`
   - `SpringDay1UiLayerUtility.cs`
3. 钉实了工作台真值当前不在 `CraftingService`，而在 overlay 自己的 runtime 内存：
   - `_queueEntries`
   - `_activeQueueEntry`
   - `readyCount`
   - `currentUnitProgress`
4. 钉实了正式存档当前没有工作台 queue DTO：
   - `StoryProgressPersistenceService` 只收长期剧情态、关系、血量/精力、`workbenchHintConsumed`
   - `SaveDataDTOs` 没有 `WorkbenchSaveData`
   - `ApplySpringDay1Progress()` 读档时还会把工作台 runtime 镜像字段全部清零
5. 钉实了 bridge 没有工作台切场合同：
   - `RebindPersistentCoreUi()` 只给 `CraftingService` 重绑 runtime 背包上下文
   - scene snapshot 只抓 `FarmTileManager / CropController / ChestController / WorldItemPickup / TreeController / StoneController`
6. 钉实了 save blocker 不是全局安全，而是有 off-scene 盲区：
   - `CanSaveNow()` 看的 `overlay.HasReadyWorkbenchOutputs / HasWorkbenchFloatingState`
   - 这两个属性都要求工作台状态绑定当前 active scene
   - 所以你离开工作台所在 scene 后，虽然 runtime 队列还可能存在，但 blocker 已经不拦
   - 与此同时，这批状态也根本不会写进存档
7. 钉实了 overlay 自己存在生命周期清空点：
   - `OnDisable()` 会 `StopCraftRoutine()`
   - 然后 `CleanupTransientState(resetSession: true)`
   - 这会把 `_queueEntries`、`_activeQueueEntry`、ready outputs 一起清掉
8. 又补钉了一条更深的风险：
   - `SpringDay1UiLayerUtility.ResolveUiParent()` 现在直接 `GameObject.Find("UI")`
   - 但项目里其实已经有 `PersistentPlayerSceneBridge.GetPreferredRuntimeUiRoot()`
   - 这意味着 overlay 有机会挂到 scene-local 临时 UI，而不是 persistent UI root
   - 一旦切场时临时 UI 被销毁/退役，就会触发上面的 `OnDisable -> 清空`

### 关键决定
1. 当前问题不能再按“补一个字段”来理解。
2. 最安全的彻底修法必须同时收 4 层：
   - 真值宿主下沉
   - 切场 continuity
   - 正式 save/load DTO
   - overlay 生命周期安全
3. 不能继续把工作台真值放在 overlay，再靠 scene return / Hide / OnDisable 打补丁。
4. 如果下一轮要正式开修，最稳顺序是：
   1. 新建正式 runtime 宿主（如 `WorkbenchRuntimeStateService`）
   2. 给每个 workbench 一个稳定 `stationKey`
   3. 让 overlay 只做展示，不再拥有真值
   4. 把 queue / ready outputs / partial progress 正式写入存档
   5. 再把 save blocker 从“止血”改成“正式支持”
5. 如果本轮只做短期止血，不做完整 persistence，至少也要补两条：
   - off-scene workbench raw state 也要拦存档
   - overlay runtime parent 改吃 persistent UI root
   但这仍不等于彻底修好

### 验证状态
- `静态代码审查成立`
- `本轮未改业务代码`
- `本轮未跑 Begin-Slice`

### 下一步恢复点
- 如果后续用户要求直接开修工作台 persistence，不要再从 `CraftingService` 或 `PackagePanel` 面上兜圈子，应直接进入：
  - `SpringDay1WorkbenchCraftingOverlay` 真值下沉
  - `StoryProgressPersistenceService / SaveDataDTOs / SaveManager` 正式保存恢复
  - `PersistentPlayerSceneBridge` continuity 合同补齐

## 2026-04-20 18:01 真实施工｜工作台正式持久化第一刀已落

### 用户目标
- 用户已批准按我前面给出的顺序直接开修：
  1. 工作台真值下沉到正式持久层
  2. 切场不再丢
  3. save/load 正式恢复
  4. 去掉旧的工作台 save blocker
  5. 补最小安全测试与自检

### 本轮完成
1. `SaveDataDTOs.cs`
   - 新增 `WorkbenchRuntimeSaveData / WorkbenchQueueEntrySaveData`
2. `StoryProgressPersistenceService.cs`
   - 新增 `WorkbenchRuntimeStateByStation`
   - 新增 `Store/Remove/Clear/TryGet/GetSnapshot` 工作台 runtime API
   - `CaptureSnapshot()` 会先 flush overlay，再把 `workbenchStates` 写入正式 snapshot
   - `ApplySnapshot()` 会替换 `workbenchStates` 并通知 overlay 重载
   - `CanSaveNow()` 已移除旧工作台 blocker，改成正式入盘
3. `SpringDay1WorkbenchCraftingOverlay.cs`
   - `OnDisable()` 改为 suspend + flush + detach，不再直接清空 session
   - `Open()` 按 `stationKey` 从正式持久层恢复
   - 补齐 `ResolveRecipeById(...)`
   - queue mutation 点统一补 `FlushCurrentRuntimeStateToPersistence()`
4. `SpringDay1UiLayerUtility.cs`
   - `ResolveUiParent()` 改为优先 persistent UI root
5. 测试
   - `StoryProgressPersistenceServiceTests.cs` 改到新语义，并补 round-trip
   - `WorkbenchInventoryRefreshContractTests.cs` 补 persist/UI-root 文本护栏

### 本轮关键决策
1. 没有把工作台状态塞进 bridge 的 off-scene snapshot。
   - 正式宿主选择 `StoryProgressPersistenceService`
   - 理由：更小刀口、更安全，且本身就是 DDOL 长期态宿主
2. overlay 继续保留展示/交互职责，但不再独占真值。

### 验证
1. `validate_script`
   - 4 个目标文件均 `owned_errors=0`
   - 但 Unity CLI 仍受 `stale_status` 干扰，assessment 多数落成 `unity_validation_pending`
2. `errors`
   - `0 errors / 0 warnings`
3. `compile`
   - 显式 4 路径时被 `CodexCodeGuard timeout(donet 20s)` 卡成 `blocked`
4. `git diff --check --`
   - 通过

### 遗留 / 下一步
1. 还差 live 回归：
   - 工作台排队 -> 切场 -> 回场
   - 工作台有内容 -> save -> load
2. 还差 packaged smoke。
3. 如果下一轮继续，就直接做上述 2 类 live，不要再回到“工作台是不是继续 blocker”那套旧语义。

### thread-state 收尾
- 本轮已执行 `Park-Slice`
- 当前 live 状态：`PARKED`
- 当前 blocker：
  1. `待做 live 回归：工作台排队切场/回场 continuity`
  2. `待做 live 回归：工作台 save/load round-trip`
  3. `待做 packaged smoke：工作台持久化最短闭环`

## 2026-04-20 18:20 追加施工｜读取存档与保存时机正式拆开

### 用户纠偏
- 用户明确裁定：`读取存档` 应该随时可读，但前提是“不在剧情中”。
- 也就是说：
  - 不能继续让 `load` 共用 `save` 的 Day1 时间窗
  - 但剧情接管、正式对白、NPC 闲聊、场景恢复中这些 blocker 仍然成立

### 本轮完成
1. `SpringDay1Director.cs`
   - 拆出 `TryGetStorySaveBlockReason(...) / TryGetStoryLoadBlockReason(...)`
   - `load` 现在不再吃 `IsDay1SaveLoadWindowOpen(...)`
2. `StoryProgressPersistenceService.cs`
   - `CanSaveNow()` 与 `CanLoadNow()` 已分别走 save/load 专属 director blocker
3. `PackageSaveSettingsPanel.cs`
   - 状态文案改成 `canSave + canLoad` 双态
   - 默认槽 / 普通槽读取动作都先检查 `CanExecutePlayerLoadAction(...)`
   - 命中 blocker 时直接提示真实原因
4. 合同测试
   - 钉死 `load` 不再吃 Day1 保存时间窗
   - 钉死读档后必须刷新全局 runtime/UI 链

### 当前验证
1. `validate_script`
   - 相关脚本 / 测试均无 own red
2. `errors`
   - `0 errors / 0 warnings`
3. `git diff --check --`
   - 通过
4. compile-first
   - 仍被 `stale_status` 卡住，未拿到 fresh compile 闭环

### 当前停车状态
- 已执行 `Park-Slice`
- 当前 blocker：
  1. `待用户复测：非剧情状态下默认存档/普通存档均可读取`
  2. `待用户复测：剧情接管中读取仍会给真实 blocker 提示`
  3. `CLI compile-first 仍受 stale_status 影响，未拿到 fresh compile 闭环`

## 2026-04-20 只读补查｜树苗放置/预览回归
- 用户新报：`树苗放置和预览位置不对了`，但 `种子` 和 `箱子` 仍然正常。
- 静态归因已经收敛：
  - 前面修箱子时，`PlacementGridCalculator` 的全局放置锚点被改成“真实 Collider center / authored visual baseline”
  - 箱子已经改成 authored visual baseline，所以这套新合同对它成立
  - 树苗仍保留 `TreeController.AlignSpriteBottom()` 的 runtime 底部对齐，而且 `Tree` 的 SpriteRenderer 和 Collider 还在同一个 GameObject 上，所以它吃的是旧的“底部对齐会带着 collider 一起动”的语义
  - 种子走的是自己的 crop preview 分支，不在同一条预制体放置合同里
- 最小安全修法：
  - 只给 `SaplingData / TreeController` 单独补回底部对齐合同
  - 不回退箱子
  - 不碰种子
- 当前状态：静态推断成立，尚未改代码，尚未 live 复测。

## 2026-04-22｜只读审计：存档、跨场景承接与持久化链草稿本级底稿

### 当前主线目标
- 继续服务 `存档系统` 主线；本轮子任务是按用户明确指定的文档与代码入口，做一轮“存档、跨场景承接与持久化链”的只读审计，并且不是只给压缩结论，而是要留下后续可反复迭代的技术提取底稿。

### 本轮完成
1. 核对了工作区 / 线程 / 策划文档：
   - `.kiro/specs/存档系统/memory.md`
   - `.codex/threads/Sunset/存档系统/memory_0.md`
   - `.kiro/specs/900_开篇/spring-day1-implementation/memory.md`
   - `Docx/大总结/Sunset_持续策划案/03_交互系统.md`
   - `Docx/大总结/Sunset_持续策划案/08_进度总表.md`
2. 核对了当前真实代码主链：
   - `SaveManager / SaveDataDTOs / DynamicObjectFactory`
   - `PersistentPlayerSceneBridge / SceneTransitionTrigger2D`
   - `StoryProgressPersistenceService / SpringDay1Director`
   - `InventoryService / EquipmentService / HotbarSelectionService`
   - `ChestController / WorldItemPickup / FarmTileManager / CropController`
   - `CraftingService / SpringDay1WorkbenchCraftingOverlay`
3. 额外核了合同测试：
   - `SaveManagerDay1RestoreContractTests`
   - `WorkbenchInventoryRefreshContractTests`
4. 还顺带钉实了两个仓库事实：
   - 当前不存在 `Assets/YYY_Scripts/Service/Save/*`
   - 当前也不存在 `Assets/YYY_Scripts/Controller/Scene/*`
   - 用户指的保存/切场主链实际已迁到 `Data/Core`、`Story/Managers`、`Story/Interaction` 和 `Service/Player`

### 关键判断
1. 现行链路已经不是“SaveManager 单核存档”：
   - `SaveManager` 负责根 DTO、写盘/读盘、slot、默认档、restart、UI 刷新
   - `StoryProgressPersistenceService` 负责剧情长期态、Day1 导演态、血量/精力、NPC 关系、workbench 状态
   - `PersistentPlayerSceneBridge` 负责 DDOL 玩家壳、跨场景服务重绑、off-scene world snapshot、scene restore 抑制和 runtime continuity
   - `SceneTransitionTrigger2D` 是正常门链入口，会先 `QueueSceneEntry()` 再开转场
2. 当前正式 payload 已经达到“当前 scene 正式落盘 + 已离场 scene 快照入档”这一层：
   - 当前 scene：`worldObjects`
   - 已离场 scene：`offSceneWorldSnapshots`
3. 玩家状态是“轻 DTO + 重 worldObjects + 运行时重装备”：
   - `PlayerSaveData` 只真写位置 / scene / hotbar selection source
   - 真背包由 `PlayerInventory`
   - 真装备由 `EquipmentService`
   - 当前工具态靠 `HotbarSelectionService.RestoreSelectionState()` 驱动 `EquipCurrentTool()` 恢复
4. 工作台现在不能再按旧口径写成“只有 blocker”：
   - 当前已存在 `WorkbenchRuntimeSaveData / WorkbenchQueueEntrySaveData`
   - 状态已经进 `StoryProgressState.workbenchStates`
   - overlay `OnDisable()` 也改成 flush + detach，不再直接清空 session
5. 当前最值得保留的负边界有三条：
   - resident/NPC off-scene 站位仍不是正式 save payload
   - payload completeness guard 仍偏窄
   - 部分摘要/UI 仍混用旧根 DTO 视角，不能直接拿来代表真实存档覆盖面

### 文档与源码的差异校正
1. `spring-day1` memory 中“bridge 不覆盖 Chest/Farm/Crop”的旧结论已经过时；当前源码已覆盖 `FarmTileManager / Crop / Chest / Drop / Tree / Stone`。
2. `存档系统` memory 中“workbench 仍只有 blocker、不入正式 DTO”的旧阶段结论也已过时；当前源码已经正式落到 `workbenchStates`。
3. 但“resident/NPC 跨场景位置已经正式存档”这类推断仍不能成立；这块仍只是 bridge runtime cache。

### 恢复点
- 如果下一轮继续沿这条线做草稿本、简历证据提取或继续只读/施工：
  1. 先按“正式存档链 / runtime continuity 链 / UI 刷新链 / door transition 链”四层拆
  2. 不要再沿用 04-17 之前那些已被后续代码推进推翻的旧结论
  3. 第一优先恢复点就是本轮这份审计底稿

### thread-state
- 本轮只读分析
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`
