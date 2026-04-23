# 4.0.0 - 三场景持久化与门链收口

## 模块概述
- 本阶段只承接当前存档主线：
  - `Primary / Town / Home` 三场景 persistent baseline
  - Home seed 化后的门链与 scene-side 尾差
  - persistent UI 断链
  - runtime-first 资产定位
  - workbench / inventory 刷新合同
  - demo 范围内的存档 UI、设置页与相关体验收口

## 当前稳定结论
- 三场景持久化底座已经存在，不应再写成“核心功能没做”
- 当前剩余问题更偏：
  - live 门链
  - 体验尾差
  - 少数 runtime 契约补口
  - 存档 UI 与 demo 范围产品面收口

## 当前恢复点
- 后续只要是三场景 persistence、Home、save UI、workbench 刷新和门链问题，统一先归这里

## 2026-04-10 17:45 只读审核：Day1 剧情已消费态与读前档回退冲突

### 当前主线 / 子任务 / 恢复点
- 当前主线：存档系统收口，但这轮不是继续写代码，而是核清“读前档后剧情不回退”的责任边界。
- 本轮子任务：审核用户拟发给 `day1` 的话术是否准确，并判断这条 bug 到底是存档主责还是 `day1` 语义主责。
- 服务关系：这件事会直接影响“旧档是否还能回到对应剧情可触发态”，属于存档可用性边界，但未必是存档线程独立背锅。
- 恢复点：当前结论已经明确，可按“`day1` 先改语义，我这边再做边界复核”的顺序继续。

### 本轮结论
- `day1` 给出的核心判断是对的：
  - `StoryProgressPersistenceService` 读档时确实会执行 `ReplaceCompletedSequenceIds(...)`
  - 但 `SpringDay1Director.HasCompletedDialogueSequence()` 会先调用 `EnsurePhaseImpliedDialogueSequenceCompletion(...)`
  - 而这个方法会按 `StoryPhase` 直接 `EnsureCompletedSequenceId(...)` 回写到 `DialogueManager`
- 这意味着：
  - 旧档刚把 `completedDialogueSequenceIds` 恢复回来
  - `day1` 又按当前 `phase` 把“本该还没消费”的剧情重新补成 consumed
  - 所以会出现“任务进度回退了，但剧情不再触发，最终卡进度”的现象

### 额外边界判断
- 这锅不该先按“存档线程独立搞坏了”处理。
- 但我这边还确认到一个次风险：
  - `StoryProgressPersistenceService.ApplySpringDay1Progress(...)` 里，也有多处 `hasSequence || currentPhase > 某阶段` 的导演私有态补写
  - 这说明除了 `completedDialogueSequenceIds` 被 phase 反写之外，`day1` 还存在“导演内部布尔态跟着 phase 超前”的语义
- 所以当前更准确的口径是：
  - 主责在 `day1` 语义层
  - 存档线程需要做的是配合复核“读档后的真源是不是仍被 `day1` 二次覆盖”，而不是先自己盲修存档链

### 当前建议顺序
1. 先由 `day1` 主刀把“phase 推断已完成”改成只读推断，不再回写 `DialogueManager.completed`
2. 然后我这边再只读复核：
   - 读档恢复的 `completedDialogueSequenceIds`
   - `ApplySpringDay1Progress(...)` 派生的导演私有态
   是否还会把旧档剧情重新补成已消费
3. 只有在 `day1` 改完后仍存在“读档恢复态被覆盖”，才轮到存档线程补最小配合刀

### 本轮读取依据
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryProgressPersistenceService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\4.0.0_三场景持久化与门链收口\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\存档系统\memory_0.md`

### thread-state
- 本轮保持只读
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 原因：没有进入真实施工，只做责任审核与边界判断

## 2026-04-10 17:53 只读复查：Day1 剧情回退 bug 当前闭环程度

### 当前主线 / 子任务 / 恢复点
- 当前主线：存档系统收口。
- 本轮子任务：重新检查项目最新现场，确认“读前档后剧情不回退”现在到底是不是已经修住。
- 服务关系：这是对上轮责任判断的 fresh 复查，不是继续凭旧结论下判断。
- 恢复点：当前结论已更新为“主根因代码已改，但闭环还不算完整”。

### 复查后的现状
- 和上轮相比，`day1` 这边已经有真实修动：
  - `SpringDay1Director.HasCompletedDialogueSequence()` 现在不再调用 `EnsurePhaseImpliedDialogueSequenceCompletion(...)`
  - 当前实现已经改成直接 `return manager.HasCompletedSequence(sequenceId);`
- 这说明最核心的那条“phase 自动把旧剧情补成 consumed”的直接写回链，代码面已经被拿掉了。

### 仍未完全闭环的点
- `StoryProgressPersistenceService.ApplySpringDay1Progress(...)` 仍然存在多处：
  - `hasSequence || currentPhase > 某阶段`
  这种 phase 派生导演私有布尔态的写法
- 这不会再直接把 `DialogueManager.completedDialogueSequenceIds` 改脏，
  但仍可能让导演内部 `_xxxSequencePlayed / _workbenchOpened / _healingStarted` 一类状态比旧档更靠后。
- 当前测试也还没有把这条回归真正钉死：
  - `StoryProgressPersistenceServiceTests` 现在只验证“completed 列表能恢复、stale-sequence 会被替换掉”
  - 但没有专门验证“较早档读回后，不允许再被 phase 或导演私有态偷偷推进”

### 当前判断
- 现在不能再说“完全没修”，因为最直接的主根因代码已经被拿掉了。
- 但也不能说“已经彻底闭环”：
  - 主根因：代码面大概率已修
  - 剩余风险：导演私有态派生仍可能留边角覆盖
  - 测试覆盖：还没把这条具体回归用专门测试钉死

### 当前验证状态
- 代码观察成立：
  - `HasCompletedDialogueSequence()` 当前实现已变更
  - `EnsurePhaseImpliedDialogueSequenceCompletion()` 在当前文件里已不再被调用
- 脚本级验证：
  - `SpringDay1Director.cs`：`owned_errors=0`，但当前 Unity console 有外部 red，不是这条线新引入
  - `StoryProgressPersistenceService.cs`：`owned_errors=0`，同样被外部 red 污染
- 所以当前只能认：
  - 目标代码改动已落
  - 但“剧情回退 bug 已终验闭环”仍未成立

### 本轮读取依据
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryProgressPersistenceService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1DialogueProgressionTests.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\StoryProgressPersistenceServiceTests.cs`

### thread-state
- 本轮保持只读
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 原因：没有进入真实施工，只做 fresh 复查与闭环判断

## 2026-04-10 23:56 只读再确认：当前还不能判“完成到位”

### 当前主线 / 子任务 / 恢复点
- 当前主线：存档系统收口。
- 本轮子任务：按用户追问，再看一次这条 bug 现在是否已经真正“完成到位”。
- 服务关系：这是对“是否能验收通过”的最终口径确认，不新增代码改动。
- 恢复点：当前结论比 17:53 那轮更明确，仍应判为“未完全到位”。

### 再确认后的结论
- 现在仍然不能判“完成到位”。
- 原因不是主根因没改，而是：
  1. 主根因代码已经改了
  2. 但导演私有态派生风险还在
  3. 针对这条风险的专门回归测试还没有补上
  4. 当前相关文件工作树也还没完全收口

### 本轮 fresh 证据
- 代码面：
  - `SpringDay1Director.HasCompletedDialogueSequence()` 仍保持“直接读取 `DialogueManager.HasCompletedSequence(...)`”
  - 没有看到重新接回 `EnsurePhaseImpliedDialogueSequenceCompletion(...)`
- 仍存在的边角风险：
  - `StoryProgressPersistenceService.ApplySpringDay1Progress(...)` 里，仍有多处：
    - `hasSequence || currentPhase > StoryPhase.X`
  - 这说明导演内部 `_villageGateSequencePlayed / _houseArrivalSequencePlayed / _healingSequencePlayed / _workbenchOpened / _workbenchSequencePlayed` 等状态，仍可能因为 phase 比旧档更靠后而被派生推进
- 测试面：
  - `StoryProgressPersistenceServiceTests` 仍只覆盖：
    - completed 列表恢复
    - stale-sequence 被替换
  - 还没有专门覆盖：
    - “读更早档后，导演私有态也不能越过旧档语义”
- 工作树：
  - `SpringDay1Director.cs`、`SpringDay1DialogueProgressionTests.cs` 当前仍是修改中
  - `StoryProgressPersistenceService.cs`、`StoryProgressPersistenceServiceTests.cs` 当前仍是未纳入版本控制状态
- Unity console：
  - 当前 `errors` 是 3 条外部错误，来自 `DayNightOverlay.cs`
  - 不是这条剧情回退修复线的直接 owned red

### 当前判断
- 现在最准确的人话：
  - 这条 bug 已经不是“完全没修”
  - 也还不是“可以放心收口验收”
  - 当前状态是：
    - 主根因已修
    - 但闭环证据和回归护栏不足

### thread-state
- 本轮保持只读
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 原因：没有进入真实施工，只做“是否完成到位”的最终口径确认

## 2026-04-11 00:00 只读补正：late-day phase 偷推已补专项测试，但整条线仍未完全到位

### 当前主线 / 子任务 / 恢复点
- 当前主线：存档系统收口。
- 本轮子任务：补正刚才的结论，确认 `day1` 今晚新增那刀到底把这条 bug 收到了哪一层。
- 服务关系：这是对“完成到位”口径的更精确更新，不是推翻前面，而是把新落地的晚段修复补充进去。
- 恢复点：当前最准确口径更新为“late-day 已明显补强，但全链仍未完全到位”。

### 新增确认的事实
- `StoryProgressPersistenceService.ApplySpringDay1Progress(...)` 里，晚段 3 处 phase 偷推已经被收掉：
  - `_dinnerSequencePlayed = hasDinnerSequence`
  - `_returnSequencePlayed = hasReminderSequence`
  - `_freeTimeIntroCompleted / _freeTimeIntroQueued` 已改成只认 `safeSnapshot.freeTimeIntroCompleted || hasFreeTimeIntroSequence`
- `StoryProgressPersistenceServiceTests.cs` 里，已经新增专门回归测试：
  - `Load_DoesNotPromoteLateDayPrivateFlagsFromPhaseAlone()`
  - 它明确验证：从 `DayEnd` 脏现场读回 `FreeTime` 旧档后，
    - `dinner/reminder/free-time-opening` 不会继续残留为 completed
    - `_dinnerSequencePlayed / _returnSequencePlayed / _freeTimeIntroCompleted / _freeTimeIntroQueued` 会回到 `false`
- `validate_script Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
  - `owned_errors=0 / external_errors=0 / warnings=0 / native_validation=clean`
  - assessment 仍为 `unity_validation_pending`，原因只是 Unity `stale_status`

### 为什么仍然不能判“完全到位”
- 这条线现在已经不是“没专项测试”，而是：
  - `late-day` 的专项测试和对应修复都已经补上了
- 但整条链仍未完全到位，因为：
  - `ApplySpringDay1Progress(...)` 里早中段仍有 phase 派生：
    - `_villageGateSequencePlayed`
    - `_houseArrivalSequencePlayed`
    - `_healingSequencePlayed`
    - `_workbenchOpened`
    - `_workbenchSequencePlayed`
  - 目前还没有同等力度的“更早档读回后这些私有态不被 phase 偷推”的专项测试
- 所以现在最准确的阶段判断是：
  - 晚段这条偷推链已明显补强
  - 但整个“剧情回退不被任何导演私有态重新推进”还没全链验证完

### thread-state
- 本轮保持只读
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 原因：没有进入真实施工，只做最新落地面的补正确认

## 2026-04-11 00:11 只读再校正：这条 bug 已基本收住，但还不适合宣称“彻底签字完成”

### 当前主线 / 子任务 / 恢复点
- 当前主线：存档系统收口。
- 本轮子任务：按用户再次追问，重新检查“读前档后剧情不回退”现在是否已经完成到位，并纠正我上一轮可能说重了的风险判断。
- 服务关系：这轮不是继续施工，而是把“真实 bug 是否已收住”和“还差的到底是 bug 还是验收证据”分开。
- 恢复点：后续如果继续讨论这条线，应以本轮结论为准，不再把早中段 phase 派生一概当成同类未闭环 bug。

### 本轮新判断
- 我要主动修正上一轮口径：
  - 我之前把早中段 `_villageGateSequencePlayed / _houseArrivalSequencePlayed / _healingSequencePlayed / _workbenchOpened / _workbenchSequencePlayed` 的 `phase > X` 派生，直接按“同类残留风险”去说，判断偏重了。
- 重新核 phase 迁移链后，更准确的结论是：
  - 真正的主 bug 是“completed dialogue 会被 phase 自动补写回 consumed”
  - 这条主根因已经拆掉
  - 晚段真正存在的“phase 偷推私有态”也已经被修掉并补了专项测试
  - 早中段这些派生更多是在表达“当前 phase 已进入后续必经门，因此前序 formal 本来就应已完成”，不等于晚段那种会把旧档错误偷推进去的同类问题

### 支撑这个修正的 fresh 依据
- `SpringDay1Director.HasCompletedDialogueSequence()` 当前仍是直接读取 `DialogueManager.HasCompletedSequence(...)`，不再按 phase 回写 completed。
- `ShouldQueueDialogueSequence(...)` 现在统一以 `localPlayedFlag || IsDialogueSequenceConsumed(sequenceId)` 做 one-shot 守门。
- `TryRecoverConsumedSequenceProgression(...)` 也已经改成“只按已消费 completed 来恢复后续阶段”，而不是按 phase 倒写 consumed。
- 早中段 phase 迁移链本身是线性的：
  - `HealingAndHP` 由进村承接后进入
  - `WorkbenchFlashback` 在 `HealingSequenceId` 完成后进入
  - `FarmingTutorial` 在 `WorkbenchSequenceId` 完成后进入
- 所以像 `_healingSequencePlayed = hasHealingSequence || currentPhase > StoryPhase.HealingAndHP` 这类写法，在当前语义下更像“后续 phase 必经门后的恢复对齐”，而不是晚段那种被证实会误推进旧档的偷推链。

### 现在还不能直接叫“彻底签字完成”的原因
- 不是因为我又看到了新的现行逻辑 bug。
- 主要是还有两层没到最终签字态：
  1. 早中段虽然从源码语义看已经说得通，但还没有和晚段同等级的专门 load 回归测试。
  2. 相关文件工作树目前仍未收口到干净提交态，说明这条线还处在集成中，而不是已经稳定归档。

### 本轮 fresh 验证
- `git status --short` 相关文件仍为：
  - `M Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `M Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
  - `?? Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
  - `?? Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
- `python .\\scripts\\sunset_mcp.py errors --count 20 --output-limit 5`
  - 当前 `errors=0 warnings=0`
  - 但最新输出里有 1 条未计入 error_count 的 `Assert`，来自 `Assets/Editor/DayNightManagerEditor.cs:130`，不属于这条剧情回退修复线
- `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - assessment=`no_red`
  - owned/external error 均为 0
- `validate_script Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
  - assessment=`unity_validation_pending`
  - 但 owned/external error 均为 0，卡在 Unity `stale_status`
- `validate_script Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
  - assessment=`unity_validation_pending`
  - native validation clean，owned/external error 均为 0，同样卡在 Unity `stale_status`

### 当前最准确的人话结论
- 如果只问这条“读前档后任务回退，但剧情不再触发”的主 bug：
  - 现在已经基本修住了。
- 如果问“我能不能替这条线签字成 100% 无懈可击、彻底完成到位”：
  - 还不能这么说。
- 更准确的阶段判断是：
  - 功能主问题：基本闭环
  - 代码/Console：当前无 fresh 红
  - 版本/证据层：还差最后一小步收口

### thread-state
- 本轮保持只读
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 原因：没有进入真实施工，只做最新一轮责任校正与完成度判断

## 2026-04-11 02:42 只读总审：存档主链已可用，但还不能直接签成“完全没问题”

### 当前主线 / 子任务 / 恢复点
- 当前主线：存档系统收口，目标仍是做出一版可演示、可持续扩展、面向打包的 Sunset 存档 demo。
- 本轮子任务：只读总审“现在还有没有没闭环的内容、会不会有性能炸弹、是否已经优化到可以打包”。
- 服务关系：这是对当前存档线的阶段判定，不是继续施工。
- 恢复点：后续若继续推进，应从“去掉 packaged live 噪音 + 补 save 页护栏 + 做真实 build smoke”收最后一小圈。

### 本轮 fresh 依据
- `python .\scripts\sunset_mcp.py errors --count 20 --output-limit 20`
  - 当前 `errors=0 warnings=0`
- `python .\scripts\sunset_mcp.py manage_script validate --name PackageSaveSettingsPanel --path Assets/YYY_Scripts/UI/Save --level standard`
  - `status=clean errors=0 warnings=0`
- `python .\scripts\sunset_mcp.py manage_script validate --name SaveManager --path Assets/YYY_Scripts/Data/Core --level standard`
  - `status=warning errors=0 warnings=3`
  - warning 仍是 native 泛扫描类提示，不是 fresh compile red
- 重新核对了：
  - `SaveManager.cs`
  - `PersistentObjectRegistry.cs`
  - `PersistentPlayerSceneBridge.cs`
  - `PackageSaveSettingsPanel.cs`
  - `SaveActionToastOverlay.cs`
  - `SaveLoadDebugUI.cs`
  - `ChestController.cs`
  - `ChestControllerEditor.cs`
  - `ChestAuthoringSerializationTests.cs`
  - `PackagePanelLayoutGuardsTests.cs`
  - `StoryProgressPersistenceServiceTests.cs`
  - `WorkbenchInventoryRefreshContractTests.cs`
- `rg` 检查结果确认：
  - save 路径已统一走 `Application.persistentDataPath`
  - 运行时代码里的 `UnityEditor` 依赖都在 editor 条件编译或 editor 脚本里
  - 当前没有针对 `PackageSaveSettingsPanel` / save 页交互的专门自动化测试命中

### 当前最准确结论
- 存档系统现在已经不是“坏的不能用”。
- 主链功能当前可跑，项目 fresh 编译也已经干净。
- 但我还不能替它签成：
  - `完全没问题`
  - `已经无懈可击`
  - `已经做过最终打包级验证`

### 为什么不能直接签“完全没问题”
- 仍有工程尾账没彻底收干净：
  1. `SaveManager` 里 `FreshStartBaseline` 相关逻辑现在大多已退役，但仍留着半壳：
     - `ScheduleFreshStartBaselineCapture()`
     - `CaptureFreshStartBaselineRoutine()`
     - `ShouldSkipAutomaticFreshStartBaselineCapture()`
     - `TryRepairDefaultProgressFromBaseline()`
     - `TryPersistDefaultProgressOnQuit()`
  2. `PersistentObjectRegistry.RestoreAllFromSaveData(...)` 仍会在恢复时做一次全场景 `StoneDebris` 扫描。
  3. `PersistentPlayerSceneBridge` 在场景重绑路径上仍大量依赖 `FindObjectsByType / FindFirstComponentInScene`。
  4. save 页刷新普通槽时仍是逐槽读摘要，槽位多了会线性变慢。
- 上面这些还没到“功能 bug”级别，但已经足够让我拒绝说“完全没问题”。

### 性能判断
- 目前没看到 demo 规模下会立刻炸掉的“性能炸弹”。
- 但有几处明确的 load/save 峰值风险：
  1. `SaveManager.WriteSaveData(...)` 仍是主线程同步 `JsonUtility.ToJson + File.WriteAllText`
  2. `prettyPrintJson = true` 仍会放大 JSON 体积与写盘成本
  3. `PackageSaveSettingsPanel.RefreshView()/RebuildOrdinary()` 会对普通槽逐个读摘要
  4. `PersistentPlayerSceneBridge.RebindScene(...)` 的场景恢复路径有多次全场景查找
  5. `PersistentObjectRegistry.CleanupStoneDebris()` 是一次全场景扫描
- 当前判断：
  - `不是现在就会炸的性能炸弹`
  - `但也不是完全优化完毕`

### 打包判断
- 现在的打包方向基本正确：
  - 存档写入目录是 `Application.persistentDataPath`
  - 旧档迁移是兼容逻辑，不是继续向 `Assets/Save` 写运行时数据
  - `SaveLoadDebugUI` 已改成运行时自清理 legacy 调试面板；scene 里虽然还留着兼容组件名，但 packaged live 不再保留旧 F5/F9 调试 UI
  - 箱子作者预设内容仍随 prefab/scene 序列化进入 build
- 但我仍不能把它直接判成“最终可放心出包”，原因不是 compile red，而是：
  1. 本轮没有实际做一遍真实 build smoke
  2. save 页没有自己的自动化护栏
  3. packaged live 里仍有一批无条件日志噪音没有彻底清干净
- 所以当前最稳的人话口径应是：
  - `可以进入试打包/冒烟验证阶段`
  - `但还不该宣称已经完成最终出包级收口`

### 后续最值钱的收尾顺序
1. 清 packaged live 噪音：
   - 重点收 `PersistentObjectRegistry` 与 `ChestController` 的无条件日志
2. 收掉默认开局旧壳语义：
   - 把 `FreshStartBaseline` 半退役代码真正整理干净
3. 给 save 页补最小自动化护栏：
   - 至少覆盖默认槽、普通槽、新建/复制/粘贴/覆盖/删除、确认弹窗与摘要刷新
4. 做一次真实 build smoke：
   - 不是只看 Editor compile，而是真正验证 packaged live 的读写目录、默认开局、普通槽、箱子作者预设与读档提示

### thread-state
- 本轮保持只读
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 原因：本轮只做 fresh 审计与阶段判断

## 2026-04-11 02:50 讨论补口：把“先清日志和旧壳，再做 build smoke”翻译成人话

### 当前主线 / 子任务 / 恢复点
- 当前主线不变，仍是存档系统收口。
- 本轮子任务：向用户解释我上一轮建议的三步到底是什么意思，不进入真实施工。
- 恢复点：后续若用户批准继续施工，就按这次解释过的三步顺序执行。

### 人话解释结论
- `清 packaged live 无条件日志`
  - 意思是：把打包后玩家不需要看到、也不该一直刷的调试输出关掉或改成只在 editor/debug 时才打印。
  - 目的：避免打包版一读档/一开箱/一恢复场景就刷一堆开发日志，既吵又可能拖慢一点运行。
- `清 FreshStartBaseline 旧壳`
  - 意思是：把“默认存档以前想做成自动基线”的老方案残留代码彻底整理掉，只保留你现在真正要的语义：默认槽就是原生开局入口，不自动保存，不退出写回。
  - 目的：减少后面误触、误读和返工风险。
- `做真实 build smoke`
  - 意思是：不是停在 Editor 里说“应该没问题”，而是真打一个包，亲手验证存档目录、默认开局、普通槽、箱子作者预设、读档提示这些在打包后是否真的还成立。
  - 目的：拿到最终能不能签字的真实证据。

### thread-state
- 本轮只读讨论
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-17 16:05 续记｜最小打包刀已落地：删除态不复活 + 箱子底部对齐 + 读档/重开 UI 刷新

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是：把存档/world-state 收到可打包的最小稳定态。
- 本轮子任务严格只收 3 件事：
  1. `树 / 石头 / 箱子 / 掉落物 / 作物` 的跨场景删除态不复活
  2. 箱子开关 sprite 的底部对齐和碰撞同步
  3. 读档 / 重新开始后的背包、toolbar、选中态刷新闭环
- 当前恢复点已经更新为：
  - 结构合同已补
  - 下一刀优先进入最短 live 冒烟，而不是继续扩静态代码改动

### 本轮实际完成
1. `PersistentPlayerSceneBridge`
   - 回场恢复不再只清 `Drop`
   - 现在会把不在 off-scene 快照里的这些对象视为“权威删除态”并清掉：
     - `Drop`
     - `Tree`
     - `Stone`
     - `Chest`
     - `Crop`
2. off-scene 空快照现在也会保留并参与 save / load
   - 最后一棵树、最后一个掉落物被清掉后，不会再因为“快照空了”而丢失 tombstone
3. `ChestController`
   - 根节点 SpriteRenderer 现在会在运行时转成视觉子节点
   - 打开 / 关闭箱子按底边对齐
   - `PolygonCollider2D` 同步应用视觉偏移，避免打开后碰撞向下扩把玩家挤走
4. `SaveManager.RefreshAllUI()`
   - 现在会统一刷新所有：
     - `InventoryService`
     - `HotbarSelectionService`
     - `InventoryPanelUI`
     - `ToolbarUI`
   - 同时重发当前选中态，并强制 `Canvas` 刷新

### 验证结果
- `validate_script`
  - `PersistentPlayerSceneBridge.cs`：`0 error / 2 warning`
  - `ChestController.cs`：`0 error / 1 warning`
  - `SaveManager.cs`：`0 error / 3 warning`
  - `WorldStateContinuityContractTests.cs`：`0 error / 0 warning`
- EditMode 测试
  - `WorldStateContinuityContractTests`
  - `WorkbenchInventoryRefreshContractTests`
  - `11 / 11 passed`
- fresh console
  - `errors = 0`
  - `warnings = 0`
- scoped `git diff --check`
  - 通过（`SaveManager.cs` 只出现 CRLF 提示，不是内容级 diff-check 失败）

### 当前判断
1. 这轮已经不是“只分析”，而是真把最小打包清单里的核心合同补进代码了。
2. 但当前仍不能直接宣称“已经可外发打包”。
3. 这轮站住的是：
   - `结构 / 合同 / 编辑器级验证`
4. 当前还没补的是真实体验层：
   - 砍树后切场回场
   - 掉落物留地后切场回场
   - 读档 / 重新开始后的真实玩家路径

### 下一刀建议
1. 先跑最短 live 冒烟：
   - 砍树
   - 丢掉落物
   - 开箱 / 关箱
   - `Primary <-> Home`
2. 再跑：
   - `Save / Load`
   - `重新开始`
3. 只有上面过了，才进入 packaged smoke

### thread-state 报实
- 本轮已跑：
  - `Begin-Slice`
  - `Park-Slice`
- 本轮未跑：
  - `Ready-To-Sync`
- 当前 live 状态：
  - `PARKED`
- 停车原因：
  - 当前代码刀口已落完，接下来应先做 live 验证，不再继续叠新改动

## 2026-04-17 15:18 续记｜最小刀口停刀：Crop continuity 合同补刀已落地，当前主阻塞转到 duplicate runtime root

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是：按 `0417` 继续把存档/world-state/箱子-背包-工作台 continuity 收平，先收真正会影响打包与玩家体验的部分。
- 本轮子任务被用户压成一个最小刀口：先把“农地 / 浇水 / 作物离场回来会丢”的作物 continuity 再补一刀，然后停下来做一次面向用户的完整现状汇报。
- 当前恢复点已经更新为：
  1. `CropController` 的作物入盘/回盘合同这刀已写完。
  2. 下一刀不要再飘去 UI、美化或 F5/F9 文案。
  3. 真正该先查的是 `Primary <-> Home` 往返时为什么会出现重复 `EventSystem`，以及它和箱子/背包断联是不是同一个根因面。

### 本轮最小刀口实际落地
1. `Assets/YYY_Scripts/Farm/CropController.cs`
   - `Save()` 改为写入 `GetCellCenterPosition()`，不再把带视觉偏移的子节点位置写进 continuity。
   - `Load()` 现在会按 `cropSaveData.seedId` 恢复 `seedData`。
   - `Load()` 同步回填 `instanceData.seedDataID = cropSaveData.seedId;`，避免下一次切场时作物被判成不可保存对象。
2. `Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs`
   - 新增字符串合同断言，锁住上述 3 个作物 continuity 关键点，防止后续再被改回去。
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\0417.md`
   - 已回填 `9.2` 的代码层补口说明。
   - 已补入本轮停刀前的 runner 二次实跑判断，明确当前阻塞已不只是 crop 本身。

### 本轮 fresh 验证
- `mcp validate_script`
  - `Assets/YYY_Scripts/Farm/CropController.cs`：`0 error / 1 warning`
  - `Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs`：`0 error / 0 warning`
  - `Assets/YYY_Scripts/Service/Player/PersistentWorldStateLiveValidationRunner.cs`：`0 error / 0 warning`
- `git diff --check -- Assets/YYY_Scripts/Farm/CropController.cs Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs .kiro/specs/存档系统/0417.md`
  - 通过
- `python .\scripts\sunset_mcp.py errors --count 20 --output-limit 20`
  - 当前 fresh console 里读到 `2` 条 error，都是：
    - `There can be only one active Event System.`
    - `There are 2 event systems in the scene...`
- `python .\scripts\sunset_mcp.py validate_script ...`
  - 这次 CLI 侧没有拿到稳定 JSON，报的是 `CodexCodeGuard returned no JSON`
  - 这说明当前 CLI 验证面本轮不能当“更强绿证”，不能拿它反盖掉 Unity 现场错误

### 当前最准确判断
1. 这次最小刀口已经完成，但只完成到了“代码层 continuity 合同补齐”，不是“玩家体验已过线”。
2. 当前还不能说已经到可打包最小态。
3. 现在最大的真阻塞已经更具体了：
   - world-state 不是单纯还差 crop/farm
   - 更像是 `Primary <-> Home` 往返时，persistent UI / inventory / scene-rebind 现场出现重复实例
   - fresh console 已经稳定给出 `2 个 EventSystem`
4. 这条阻塞和下面这些玩家可见坏相高度相关：
   - `Home / Town` 箱子界面下半区背包像断联
   - world-state runner 跑到一半不再稳定出报告
   - 切场后 inventory / hotbar / workbench 的事实源可能重新分叉

### 下一刀建议
- 不要先回 `Town` blocker，也不要继续扩 `Save UI`
- 最应该先做的一刀是：
  1. 钉死谁在 `Primary <-> Home` 往返时又生成了第二个 `EventSystem`
  2. 顺手确认 `PlayerInventory` / persistent inventory root 是否也在重复生成或重复绑定
  3. 只有把这层 duplicate runtime root 收住，`9.2 / 9.3 / 13.4 / 14.x` 才有继续测的意义

### thread-state 报实
- 本轮已跑：
  - `Park-Slice`
- 本轮未跑：
  - `Ready-To-Sync`
- 当前 live 状态：
  - `PARKED`
- 停车原因：
  - 最小刀口已完成，当前需要先向用户报实，不继续扩下一刀

## 2026-04-17 续记｜只读复盘：Home 箱子上漂与箱子界面下半区背包失联已经有明确嫌疑链

### 用户目标
- 用户要求先暂停继续施工，改为彻底盘清当前这条存档/持久化线到底已经做了什么、还缺什么，以及最近冒出来的三个真实坏相：
  1. `Home` 里的箱子每次回场都会再往上漂一点
  2. `Home / Town` 打开箱子时，上半区箱子可操作，但下半区背包像断联
  3. 需要把“已经做完”和“还没闭环”分开说清楚，不能再拿文档黑话糊用户

### 本轮新钉住的结论
1. `Home` 箱子越进越高，当前最高置信根因已经不是泛猜：
   - `ChestController.AlignSpriteBottom()` 直接改的是箱子根节点 `transform.localPosition`
   - 对照 `TreeController.AlignSpriteBottom()`，树改的是 `spriteRenderer.transform.localPosition`
   - 这意味着箱子的“贴地修正”现在会直接污染物体真实位置，而不是只改视觉子节点
   - 这条链足以解释：
     - 回场后箱子越来越往上
     - 交互点越来越偏
     - continuity / save 记录的位置也跟着被污染
2. 箱子内容“回场吃回作者预设”目前仍然不能视为彻底收平：
   - `ChestController.Initialize()` 仍会在库存新建且为空时执行 `ApplyAuthoringSlotsIfNeeded()`
   - 如果某次离场 / 回场没有正确命中对应箱子的运行态恢复，箱子就会重新吃作者预设内容
   - 这和用户看到的“隔场回来箱子又满了”是同一类坏相
3. `Home / Town` 箱子界面的下半区背包失联，目前还没有钉死成单一函数，但已经能明确：
   - 不是“整个箱子 UI 都没开”
   - 上半区箱子主要走 `ChestInventoryV2 + BoxPanelUI` 直链
   - 下半区背包还要经过 `InventorySlotInteraction -> InventoryInteractionManager -> GameInputManager` 这条更长的输入/门控链
   - 所以“上半区能点、下半区像断联”是合理坏相，不是用户误判
4. 当前代码层自动化测试还不能证明这些问题已经没了：
   - 现有合同测试更像结构护栏
   - 它们没有覆盖用户现在真实报的 `Home / Town / packaged` 箱子交互坏相

### 当前最准确的人话判断
- 这条线目前不是“存档完全没做”，而是：
  - 正式存档骨架、切场 continuity 容器、背包选中语义第一轮修补、箱子/农地/作物入链这些基础设施已经搭了不少
  - 但真实玩家最痛的那几处体验问题还没有被 live 终验钉死
- 尤其现在最危险的不是一个小尾巴，而是两类根问题还没真正收平：
  1. 箱子根节点位置被错误改写
  2. 箱子面板下半区背包交互链在跨场景后仍不可信

### 恢复点
- 如果下一轮回到真实施工，优先顺序应改成：
  1. 先修 `ChestController` 的根节点对齐污染
  2. 再单独钉 `Home / Town` 箱子界面下半区背包失联
  3. 然后才继续做更大的 continuity / packaged 终验

### thread-state 报实
- 本轮只读分析
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-17 续记｜P0-B 第二簇真修补已继续落地

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是 `0417`。
- 本轮子任务之一：
  - 把背包真实选槽被 runtime reassert 偷偷抹掉、以及剧情开始只关箱子不关背包的坏相，继续从“问题树”推进到真实代码修补。
- 恢复点：
  - 当前 `P0-B` 已不是只剩静态审计。
  - 代码层又新增一簇真实修补，后续只剩 live / packaged 终验和更高层真选槽总合同。

### 本轮完成
1. `GameInputManager.ResetPlacementRuntimeState()` 不再把 `selectedInventoryIndex` 强压回 hotbar。
2. 面板关闭恢复、同 hotbar 重应用当前选择，都改成：
   - 保留背包偏好槽
3. `ToolbarSlotUI` 的 deferred / rejected 回退分支改成：
   - 不再 collapse 回 hotbar
4. `PlayerInteraction.ApplyCachedHotbarSwitch()` 的失败回退也改成：
   - 保留当前背包偏好槽
5. `GameInputManager.OnDialogueStart()` 不再只 `CloseBoxPanelIfOpen()`
6. 新增：
   - `CloseBlockingPanelsForDialogueLock()`
   统一关闭 `Package / Box`
7. `InventoryRuntimeSelectionContractTests` 新增护栏：
   - `RuntimeReassertions_ShouldPreserveInventorySelectionPreference`
   - `DialogueLock_ShouldClosePackageAndBoxPanelsTogether`

### 验证
- `InventoryRuntimeSelectionContractTests`
- `WorkbenchInventoryRefreshContractTests`
- `SaveManagerDay1RestoreContractTests`
- `WorldStateContinuityContractTests`
- 当前整体口径：
  - `14/14 passed`

### 当前判断
- 这轮不是“又多一个建议”，而是 `P0-B` 真正又向前收了一刀。
- 当前 `P0-B` 最准确状态：
  - 代码层第二簇修补已过
  - live / packaged 终验待收

## 2026-04-17 续记｜P0-C 第二刀已补作物 GUID 护栏，并开始 world-state live harness

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是 `0417` 的 `P0-C｜world-state 主链`。
- 本轮子任务：
  1. 把运行时新种作物可能没有 `PersistentId` 的漏口补掉
  2. 给 `任务 9` 增加可重复触发的 live harness
- 恢复点：
  - 当前 `P0-C` 不再只有 formal save / continuity 的代码合同
  - 已进入“代码合同 + live harness 并行推进”阶段

### 本轮完成
1. `CropController`
   - 现在会在 `OnEnable` 与 `PersistentId` 读取时自动补齐 GUID
   - 新种作物不再因为空 GUID 被 `PersistentPlayerSceneBridge` 的 continuity 捕获直接跳过
2. 新增 `WorldStateContinuityContractTests`
   - 护住：
     - 作物 GUID 不能为空着离场
     - `FarmTileManager / Crop / Chest` 必须持续在 scene snapshot 合同内
3. 新增 `PersistentWorldStateLiveValidationRunner`
   - 负责自动准备：
     - 箱子标记内容
     - 农地 / 浇水
     - 新种作物
   - 然后尝试跑：
     - `Primary -> Home -> Primary`
     - `Primary -> Town -> Primary`
4. 新增 `PersistentWorldStateLiveValidationMenu`
5. 新增 `Library/CodexEditorCommands/world-state-live-validation.command`
   - 用于不依赖点击 menu，直接在 Play 启动 live harness

### 验证
- `WorldStateContinuityContractTests`
- `InventoryRuntimeSelectionContractTests`
- `SaveManagerDay1RestoreContractTests`
- 当前整体合同测试口径：
  - `14/14 passed`

### live 取证现状
1. 当前已证实：
   - `Primary -> Home` 的 scene load 本体能真正进入 `Home`
   - `PersistentPlayerSceneBridge.QueueSceneEntry()` 已在离场前采到：
     - 箱子
     - 农地
     - 新种作物
2. 当前还没收平的是：
   - harness 自己在场景切换后的续跑稳定性
   - 还没有拿到 `Home -> Primary` 与 `Town -> Primary` 最终通过报告
3. 所以当前正确口径是：
   - `任务 9` 已经有真实抓手
   - 但 live 终验还没 claim 通过

### 当前判断
- 这轮最重要的代码修补，不是 UI，而是：
  - 运行时新种作物终于正式接进 continuity 真链
- 当前最真实的 blocker，不是 compile red，而是：
  - world-state harness 跨场景续跑还没完全收平

### thread-state
- 本轮最终已补：
  - `Park-Slice`
- 当前 live 状态：
  - `PARKED`
- 当前 blocker：
  1. `任务 9` 的 world-state live harness 还需收平跨场景续跑
  2. `9.1 ~ 9.3` 的 live / packaged 终验尚未完成

## 2026-04-17 续记｜P0-C 第一刀已把 off-scene formal contract 落成真实代码

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是 `0417` 下的 `P0-C｜world-state 主链`。
- 本轮子任务是把下面三件事从只读判断推进成真实施工：
  1. 冻结 `off-scene world-state` 正式入盘合同
  2. 先收箱子跨场景 / 读档与 continuity 打架
  3. 再收农地 / 浇水 / 作物跨场景 continuity
- 恢复点：后续继续这条线时，先回 `0417.md` 的 `P0-C` 区，再看本条记录补技术细节。

### 本轮实际完成
1. `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
   - `GameSaveData` 新增：
     - `offSceneWorldSnapshots`
   - 新增 DTO：
     - `SceneWorldSnapshotSaveData`
   - 语义固定为：
     - 当前 loaded scene 仍走 `worldObjects`
     - 已离场 scene 改走 `per-scene snapshot`
2. `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
   - 新增正式导出 / 导入链：
     - `ExportOffSceneWorldSnapshotsForSave()`
     - `ImportOffSceneWorldSnapshotsFromSave(...)`
   - 新增 direct load 防打架接线：
     - `SuppressSceneWorldRestoreForScene(...)`
     - `CancelSuppressedSceneWorldRestore(...)`
   - continuity 捕获 / 绑定范围已扩到：
     - `ChestController`
     - `FarmTileManager`
     - `CropController`
   - 恢复顺序显式排序为：
     - `FarmTileManager`
     - `Crop`
     - `Chest`
     - `Drop / Tree / Stone`
3. `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - 保存时开始写：
     - `PersistentPlayerSceneBridge.ExportOffSceneWorldSnapshotsForSave()`
   - 读档时开始导入：
     - `PersistentPlayerSceneBridge.ImportOffSceneWorldSnapshotsFromSave(...)`
   - 跨场景读档前会先抑制目标 scene 的 continuity restore，避免 target scene 先吃旧 runtime snapshot，再被正式存档覆盖
4. `Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
   - 新增 4 条 source-contract 护栏，覆盖：
     - off-scene snapshot DTO 已存在
     - SaveManager 已接导出 / 导入链
     - 跨场景读档会抑制当前 load 目标 scene 的 continuity restore
     - bridge 已覆盖 `Chest / FarmTile / Crop`

### 当前最核心判断
- 这轮真正收住的不是一个表层 bug，而是“正式存档 vs 切场 continuity”的最小分工合同：
  - 当前 scene = `worldObjects`
  - off-scene = `offSceneWorldSnapshots`
  - direct load 当前目标 scene 不再允许先吃 continuity，再吃正式存档
- 这正是前面“箱子回场复活 / 农地离场回档 / save 与 continuity 打架”的主根因。

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

### 这轮没有伪装完成的部分
1. `7.3` 以及 `9.1 ~ 9.3`
   - 仍是 `live / packaged` 待测，不是“我已经替玩家体验签字”
2. 当前还没有 claim：
   - packaged live 最终稳了
   - `Primary / Home / Town` 三场景往返人工体验已经终验

### 下一步
- 优先回 `0417.md`：
  - `任务 9｜world-state 检查点`
- 也就是先补：
  - `Primary 箱子离场回来不复活`
  - `农地 / 浇水 / 作物离场回来不丢`
  - `Town / Home / Primary` 往返稳定

## 2026-04-17 只读补充｜跨场景箱子/农地/背包整链坏相已收敛成“覆盖范围不一致”

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是存档系统在打包前的最后收口。
- 本轮子任务不是施工，而是彻底查清用户反馈的“跨场景后箱子、农地、背包、toolbar 像整套一起瘫痪”的真实问题总图。
- 恢复点：后续若进入真实施工，应优先回到 `跨场景 world-state 与持久 UI rebind 合同统一`，而不是继续追单个表层症状。

### 本轮只读已证实的问题
1. 正式普通存档当前仍以“当前已加载场景”为主收 `worldObjects`：
   - Town 存档样本 `slot1.json / slot6.json` 里看得到 `FarmTileManager`，但看不到别的场景箱子或作物对象。
   - Home 存档样本 `slot3.json` 里能看到 Home 箱子对象。
   - 这说明现在不是“全世界统一入档”，而是“当前现场进得多，离场场景进得少，而且覆盖不一致”。
2. `PersistentPlayerSceneBridge` 的离场 continuity 当前只覆盖：
   - 掉落物
   - 树
   - 石头
   不覆盖：
   - 箱子
   - 农地
   - 作物
3. 箱子内容复活不是用户错觉：
   - 箱子运行态没有被切场 continuity 带走
   - 重新进场后又会重新吃作者预设内容
   - 所以会出现“拿空了又长回来”“过了一天又满了”的体感。
4. 农地 / 浇水 / 作物离场后消失，也不是纯 UI 假象：
   - 正式 Save/Load 链认识它们
   - 但切场 continuity 链不认识它们
   - 所以当前现场改了，离场再回来就像没发生。
5. 背包 / 箱子 / toolbar 为什么会让用户感觉“整套都坏了”：
   - 持久 UI 壳体会跨场景保留
   - 但每次切场都要重新找背包、装备、热栏选择、箱子运行时引用
   - 一旦 world-state 本体没有统一连续，UI 这层就会表现成显示、选中、手持、交互彼此不同步。
6. 新补钉实的“操作像卡死”链：
   - 切场只记 `hotbar` 高亮，不记真正的“背包选槽来源”
   - 场景切换 / 重开时，输入层又会把真实背包选槽压回 `hotbar` 同号格
   - 剧情开始时只强关箱子，不强关背包，但工具使用 / 移动 / 切栏又统一受“有无面板打开”门控影响
   - 所以玩家会真实感受到：高亮还在、东西还在，但工具突然不能用、种子像不认、toolbar 像锁死

### 本轮高概率问题
1. `ToolbarUI` 失活再激活后，可能没有重新订阅库存和选中事件：
   - 玩家体感会像“条还在，但不再跟刷新、亮块不跟、数量不动”
2. 背包面板和箱子面板每次重新打开都会把视觉选中重置到跟随 hotbar：
   - 玩家体感会像“我刚才明明点的是下排/另一格，重新开又跳回第一排对应格”
3. UI 重绑大量依赖运行时兜底查找：
   - 一旦重绑没完全接好，不一定会直接红错，而是可能静默绑到当前场上的第一个对象
   - 所以最容易出现“界面还活着，但绑的不是现在该绑的那个东西”

### 当前最准确结论
- 现在最大的根因不是“又有一个小 bug 没修”，而是：
  - 正式存档
  - 切场 continuity
  - 默认重开 / fresh start
  这三套语义覆盖范围不一致。
- 所以用户会同时看到：
  - 有的内容场景内像能保存
  - 一换场景就回原样
  - 再打开面板又像另一套状态
  - 导致整套系统体感像“根本收不住”。

### 这轮没做什么
- 没改代码
- 没做 live 复现
- 没 claim 修好
- 当前只把问题总图和证据链钉实，供下一刀施工使用

### thread-state
- 本轮只读分析
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-17 00:30 续收口｜背包/toolbar rebind 第二簇验证完成 + Primary 农地离场丢状态只读归因

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是存档系统收口。
- 本轮先完成上一刀的第二簇验证与边界协调，再按用户新补反馈只读彻查：
  - `重新开始 -> 再走剧情 -> 在 Primary 种地浇水 -> 离开 -> 回到 Primary` 后农地状态消失。
- 恢复点：
  1. 背包/toolbar/runtime rebind 这刀当前已到“代码层证据成立，待 live 终验”
  2. Primary 农地问题已钉到 off-scene world-state 未入 continuity 合同，不是本轮显示链小 bug

### 本轮完成
1. 补完第二簇脚本与邻接 contract test 验证：
   - `PersistentPlayerSceneBridge.cs`：`errors=0 warnings=2`
   - `InventoryRuntimeSelectionContractTests.cs`：`errors=0 warnings=0`
   - `SaveManagerDay1RestoreContractTests.cs`：`errors=0 warnings=0`
   - `WorkbenchInventoryRefreshContractTests.cs`：`errors=0 warnings=0`
2. 跑了本轮 own 文件的 `git diff --check`：
   - 未发现 fresh 空白/格式 red
   - 只有 Git 的 CRLF/LF 提示，不构成当前 compile blocker
3. 发现 `toolbar 固定槽位丢图标` 与我这条线真实撞面：
   - `农田交互修复V3` 当前 active slice 把 `ToolbarUI.cs / ToolbarSlotUI.cs / PersistentPlayerSceneBridge.cs` 都挂进了 owned paths
   - 我已新建边界收口 prompt，要求它继续主刀 `ToolbarUI / ToolbarSlotUI`，并退出 `PersistentPlayerSceneBridge.cs`
   - prompt 文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-17_给农田交互修复V3_toolbar图标与scene-rebind边界收口prompt_01.md`
4. 只读彻查 Primary 农地离场丢状态：
   - `SaveManager.CollectFullSaveData()` 仍明确只收“当前已加载 scene 内”的 `worldObjects`
   - `PersistentPlayerSceneBridge.QueueSceneEntry()` 离场前会抓 runtime state，但它的 `CaptureSceneWorldRuntimeState()` 当前只覆盖：
     - `WorldItemPickup`
     - `TreeController`
     - `StoneController`
   - `FarmTileManager` 和 `CropController` 虽然都实现了 `IPersistentObject`，但它们只进入“正式存档/读档链”，没有进入“离场 scene continuity”捕获名单

### 当前最准确结论
- 这轮前半段四项修补，当前最诚实口径是：
  - 代码层修补已落地
  - 脚本级验证与 contract test 验证已过
  - 还没做 live 终验，所以不能报成“玩家侧 100% 已验完”
- 用户新补的农地问题，当前主根因已经很明确：
  - 不是单纯显示没刷
  - 是 `Primary` 离场时，农地/作物 runtime state 根本没有进入 bridge continuity snapshot
  - 所以回到 `Primary` 时会按原生场景重新起来，刚刚耕地/浇水的运行态自然消失
- 更进一步的风险也已明确：
  - 现在不仅“离场再回来”会丢
  - 如果玩家离开 `Primary` 后在别的场景存档，当前正式存档也拿不到 off-scene 的农地状态

### 证据锚点
- `SaveManager.cs`
  - `CollectFullSaveData()` 与注释：`694-709`
  - `ApplyNativeFreshRuntimeDefaults()`：`956-958`
- `PersistentPlayerSceneBridge.cs`
  - `QueueSceneEntry()`：`165-168`
  - `CaptureSceneRuntimeState()` 调 `CaptureSceneWorldRuntimeState()`：`566-599`
  - `CaptureSceneWorldRuntimeState()` 只抓 `WorldItemPickup / TreeController / StoneController`：`722-740`
  - `RestoreSceneWorldRuntimeState()` 绑定列表同样只含上面三类：`906-913`
- `FarmTileManager.cs`
  - `IPersistentObject` 定义与注册：`17`, `127`
  - `Save()`：`1009`
  - `Load()`：`1057`
- `CropController.cs`
  - `IPersistentObject` 定义与注册：`33`, `188`
  - `Save()`：`898`
  - `Load()`：`932`

### blocker / 未完成
- `read_console` 本轮没拿到 fresh 证据：
  - Unity session 当前返回 `not ready for read_console`
- Primary 农地问题按用户要求本轮只做了彻查，没有进入真实修复
- 背包/toolbar rebind 这刀还没做 live/manual 终验

### thread-state
- 本轮处于真实施工后的收口阶段
- 已跑：
  - `Begin-Slice`
  - `Park-Slice`
- 尚未跑：
  - `Ready-To-Sync`
- 当前 live 状态：`PARKED`

## 2026-04-14｜build fail 只读归因：主阻塞不是 SaveManager，而是 DayNightManager 的 editor-only 调用泄漏到 player 编译面

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是存档系统打包前收尾与可打包性。
- 本轮子任务：根据用户贴出的 build 日志，定位“和存档线程有关、或最容易被误归因到存档线程”的真实阻塞点，并给出最小修法。
- 恢复点：如果下一轮进入真实施工，优先只收 `Assets/YYY_Scripts/Service/Rendering/DayNightManager.cs`，必要时再单独收 `CloudShadowManager` 的 editor 预览噪音，不扩到 `SaveManager` 其他逻辑。

### 只读结论
- 这次打包失败里，当前能明确钉死的真正编译红错不是 `SaveManager`。
- 真正阻塞点是：
  - `Assets/YYY_Scripts/Service/Rendering/DayNightManager.cs:208`
  - `Start()` 里直接调用了 `EditorRefreshNow();`
- 但 `EditorRefreshNow()` 方法本体在同文件的 `#if UNITY_EDITOR` 区块内。
- 结果是：
  - Editor 下能编
  - Player build 时方法被裁掉
  - 调用还在
  - 于是触发 `CS0103`

### 补充判断
- 用户日志里那批 `CS0414` warning 当前都不是 build blocker。
- `CloudShadowManager` 末尾的自动检测日志与 `DontSaveInEditor` assertion，更像 `[ExecuteAlways] + EditorUpdate + hideFlags` 的编辑器态副作用，不是这次 C# 编译失败的第一主因。
- 所以后续修复优先级应是：
  1. 先修 `DayNightManager` 的 editor-only 调用泄漏
  2. 再看要不要顺手治理 `CloudShadowManager` 的编辑器断言噪音
  3. 警告类字段可后置统一清理

### 建议最小修法
- 在 `DayNightManager.Start()` 里把 `if (!Application.isPlaying) { EditorRefreshNow(); return; }` 改成 editor 条件编译保护。
- 最小安全口径是：
  - `UNITY_EDITOR` 下：保留 `EditorRefreshNow(); return;`
  - 非编辑器 build 下：只 `return;`
- 不建议这一刀顺手大改整条昼夜预览体系。

### thread-state
- 本轮仍是只读分析
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-16｜背包/toolbar/重开/Home 只读事故排查：已经能明确拆成“两组问题叠加”

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是存档系统打包前与运行时收口。
- 本轮子任务：按用户描述彻查“重新开始游戏、进入剧情、进出 Home 后，背包显示/toolbar 选中与使用/箱子 sort/背包 sort 全部变怪”的相关链路，判断正确逻辑与当前真实坏点。
- 恢复点：如果下一轮进入真实施工，优先不要泛修整条存档线，而是拆成：
  1. `背包真选中 / 假选中 / sort 语义统一`
  2. `restart/Home/剧情后的 runtime rebind contract`

### 当前最重要的新判断
- 这次不是一个单点 bug。
- 当前更像是两组问题叠加：
  1. **明确存在的本地逻辑裂缝**
     - hotbar 真选中
     - InventoryPanel 本地选中
     - BoxPanel 本地选中
     - 三者不是同一个真源
     - sort 也存在两套相互冲突的语义
  2. **高风险 runtime rebind 面**
     - `restart / 进剧情 / Home 往返` 时，`PersistentPlayerSceneBridge` 必须重绑 player / inventory / hotbar / input / UI
     - 这条链当前高度依赖 runtime patch
     - 一旦某次重绑打偏，就会把“显示错 / 手持错 / 交互错”一起放大

### 已经明确钉死的显性问题
1. `HotbarSelectionService.SelectInventoryIndex()` 对 `>=12` 的背包槽只改 `selectedInventoryIndex`
   - 不触发 `OnSelectedChanged`
   - 不调用 `EquipCurrentTool()`
   - 结果：背包里看起来选中了，但真实运行时仍按 `selectedIndex` 那个 hotbar 槽工作
2. `InventorySlotUI.ResolveSelectionState()` 在箱子打开时，对玩家背包槽采用：
   - `BoxPanelUI.IsInventorySlotSelected(index) || InventoryPanelUI.IsInventorySlotSelected(index)`
   - 结果：隐藏背包页的旧选中态、箱子 Down 区选中态、真实 hotbar 选中态会混成一层假高亮
3. sort 语义已经分裂：
   - `InventorySortService.SortInventory()` 排 `0..35`
   - `PlayerInventoryData.Sort()` / `InventoryService.Sort()` 只排 `12..35`
   - 结果：同样叫“整理背包”，不同入口会不会动 hotbar 根本不是一回事
4. `GameInputManager.ResetPlacementRuntimeState()` 与若干恢复口会调用：
   - `hotbarSelection?.ReassertCurrentSelection(collapseInventorySelectionToHotbar: true, invokeEvent: true);`
   - 结果：剧情/场景切换后真实运行态会强制折回 hotbar 选中，但 UI 本地选中态不一定同步清掉

### 已确认的高风险恢复面
- `Town.unity` 中 `GameInputManager` 的核心序列化引用本来就是空：
  - `playerToolController / inventory / hotbarSelection / packageTabs = {fileID: 0}`
- 这意味着：
  - restart 后能不能正常玩
  - 不是靠场景预绑
  - 而是高度依赖 `PersistentPlayerSceneBridge.RebindScene -> RebindSceneInput -> RebindPersistentCoreUi`
- 目前静态看，这条重绑链本身不是“明显写错了一行”的程度，但它是整条异常里最该做 live contract 验证的责任面

### 当前对症状的解释
- “背包显示和真实可用槽位对不上”
  - 最像 `选中态分叉 + sort 语义分裂`
- “toolbar 内容无法选中和使用”
  - 最像 `真选中还停在 hotbar.selectedIndex，但 UI 已经把别的槽位画成选中`
- “打开箱子后只有箱子里的 sort 能工作 / 看起来更正常”
  - 最像箱子页自己有额外刷新桥接，但它又把本地选中态再叠了一层，所以表现成“局部能动、整体仍怪”
- “重开 / 进剧情 / Home 往返后一起变怪”
  - 最像 `runtime rebind + reset` 在放大上面那套本地裂缝，而不是单独新增了另一套背包系统

### 测试覆盖现状
- 现有 `SaveManagerDay1RestoreContractTests` 只盯：
  - 恢复前清旧 UI / 清 pause / 清提示壳
- 现有 `WorkbenchInventoryRefreshContractTests` 只盯：
  - inventory changed contract
- 当前没有覆盖的是：
  - `restart -> 剧情 -> Home 往返 -> toolbar / inventory / input / sort 仍一致`

### 推荐下一刀切片
1. 先统一“真选中语义”
   - 明确到底谁是唯一真源：`HotbarSelectionService`
   - 禁止 `InventoryPanelUI / BoxPanelUI` 各自长期保存独立真选中
2. 再统一 sort 语义
   - 必须拍板“整理背包是否允许动 hotbar”
   - 然后只保留一套实现
3. 最后补一条整链 contract
   - `restart -> story -> home round-trip -> inventory/toolbar/input consistent`

### thread-state
- 本轮只读分析
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-13｜Day1 恢复入口卫生链落地 + 离场 scene world-state 审计停表

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是存档系统收口。
- 本轮子任务固定为两条：
  1. 按 `spring-day1` 已给出的整条 `Day1 restore contract`，把 `save/load/restart` 入口的 stale UI / stale modal / stale pause / stale input 清理链接住。
  2. 重新审计“离场 scene 的 runtime world state 要不要进正式存档”，并给出这轮是否落代码的最小裁定。
- 恢复点：
  - `Day1` 这轮已经先把 `SaveManager` 恢复入口卫生链补上；
  - `off-scene world-state` 仍停在合同层，不直接改 `PersistentPlayerSceneBridge`、不直接改正式 DTO 消费链。

### 本轮实际落地
1. `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - `ApplyLoadedSaveData(...)` 和 `ApplyNativeFreshRuntimeDefaults()` 现在都会先走统一的 `ResetTransientRuntimeForRestore(...)`。
   - 这条统一清理链现在会做：
     - `DialogueManager.StopDialogue()`
     - 关闭 `Package / Box`
     - 关闭 `SpringDay1WorkbenchCraftingOverlay`
     - 清掉背包拖拽 / chest held / tooltip / use-confirm
     - 清掉 `SpringDay1PromptOverlay` 的外部阻断与 bridge prompt 残留
     - 隐藏 `InteractionHintOverlay / NpcWorldHintBubble / SpringDay1WorldHintBubble`
     - 隐藏玩家 thought bubble 与 NPC bubble
     - 清掉已知 pause source：
       - `Dialogue`
       - `SpringDay1Director`
       - `__manual__`
     - 最后再统一 `ForceResetPlacementRuntime(...)`
   - 同轮把旧的 `CheckPositionNextFrame(...)` 和相关“内鬼脚本”爆红诊断壳从 `SaveManager` 里删掉，避免 packaged live 再带着这类死日志尸体。
2. `Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
   - 新增 3 条 source-contract 护栏：
     - 读档 / 原生重开都会走同一条恢复卫生链
     - 已知的 Day1 transient UI / modal / pause source 不能悄悄脱钩
     - `worldObjects` 当前仍只代表“当前 loaded scene 内”的正式持久对象，不能被误说成已经覆盖 off-scene world-state
3. `off-scene world-state` 裁定
   - `SaveManager.CollectFullSaveData()` 旁已经加了明确注释：
     - 正式存档当前只收当前 loaded scene 的 `PersistentObjectRegistry`
     - 已离场 scene 的 runtime continuity 仍由 `PersistentPlayerSceneBridge.sceneWorldSnapshotsByScene` 维护
     - 在不改 bridge 消费合同前，不能把 off-scene state 粗暴并进 `worldObjects`

### 验证
- `validate_script Assets/YYY_Scripts/Data/Core/SaveManager.cs`
  - `owned_errors=0`
  - `external_errors=0`
  - `assessment=unity_validation_pending`
  - 卡点仍是 `stale_status / codeguard timeout-downgraded`，不是 own red
- `validate_script Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
  - `owned_errors=0`
  - `external_errors=0`
  - `assessment=unity_validation_pending`
- `EditMode` 测试：
  - `SaveManagerDay1RestoreContractTests`
  - `passed=3 failed=0`
- `git diff --check -- SaveManager.cs SaveManagerDay1RestoreContractTests.cs`
  - 通过
- `python scripts/sunset_mcp.py errors --count 20 --output-limit 5`
  - 仍有 `12` 条 external error
  - 当前展示的是一批 `The referenced script (Unknown) on this Behaviour is missing!`
  - 这不是本轮 `SaveManager` 自己引入的新红面

### 当前判断
- 这轮真正完成的是：
  - `Day1 restore hygiene` 已经从“只有 story snapshot，缺入口清理”推进成“load/restart 前先统一清场，再让 day1 runtime 自己按 phase 重新接管”
- 这轮没有伪装完成的是：
  - `off-scene world-state` 还没有正式进入存档文件
  - 现在最小正确合同仍然是：
    - 单独的 per-scene snapshot 容器
    - scene 真加载后再消费
    - owner 仍在 `PersistentPlayerSceneBridge` 这一侧
  - 所以这轮不建议在 `runtime bridge` 正式合同没对齐前，先去改 `SaveDataDTOs` / 旧档迁移把它硬塞进正式存档

### thread-state
- 本轮已跑：
  - `Begin-Slice`
  - `Park-Slice`
- 未跑：
  - `Ready-To-Sync`
- 当前 live 状态：`PARKED`
- 停车原因：
  - `SaveManager` 入口卫生链和轻量回归护栏已落地
  - `off-scene world-state` 这轮明确停在审计结论，不继续抢 bridge

## 2026-04-13｜审计收尾：两笔存档提交已落在 main，当前只再收 memory 与停车

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是存档系统收口。
- 本轮子任务：把“当前这条线真能提交的内容”按规范彻底报实，不再误把 shared-root 上其他线程的大量脏改算成存档线程自己的尾账。
- 恢复点：`Data/Core` code checkpoint 与对应 memory checkpoint 都已进入 `main`；当前线程后续若继续，只能再开新的窄切片，不应把这两笔已成立提交重新混回“大清盘”口径。

### 本轮重新钉死的事实
1. 两笔已成立提交：
   - `0fa99813` `2026.04.13_存档系统_01`
   - `7fd6a606` `2026.04.13_存档系统_02`
2. 当前 `main` / `origin/main` 共同停在：
   - `8a3ad181` `2026.04.13_Codex规则落地_10`
3. 这说明：
   - 上面两笔存档提交都已经在 `main` 历史里
   - 当前并不存在“我这条线还有一笔没推上去的 save 提交”这种尾账

### 当前最准确口径
- 我这轮能合法提交的 save own 内容，已经先后收成了两笔：
  - 一笔代码 checkpoint
  - 一笔 memory checkpoint
- 现在 shared root 上剩下的大量 dirty，绝大多数都不是存档线程这轮应该继续盲吞的 own 范围。
- 所以这轮最后要做的，不是再硬凑第三笔业务提交，而是：
  1. 把这个判断回写 memory / 审计
  2. 合法 `Park-Slice`

### 风险与边界
- 当前没有宣称“整个存档系统已经无懈可击”。
- 当前也没有宣称“shared root 已 clean”。
- 更准确的人话：
  - 存档线程这轮自己能证明、能归仓的部分已经归仓
  - 剩下的是其他线程的活跃施工面，或需要以后另开窄切片再收的内容

### thread-state
- 本轮已重新 `Begin-Slice`：
  - `收口 Data/Core 已提交事实并完成最终审计停车`
- 当前 live 状态：`ACTIVE`
- 下一步只剩：
  1. 补 `skill-trigger-log`
  2. 重新 `Ready-To-Sync`
  3. 提交本轮 memory
  4. `Park-Slice`

## 2026-04-13｜已提交：Data/Core 三文件 code checkpoint

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是存档系统收口。
- 本轮子任务：按用户要求，把当前这条线在 `Assets/YYY_Scripts/Data/Core` 下能够合法单独归仓的内容先提交干净，不扩到更大的 `Story / Rendering / Player / UI` 脏根。
- 恢复点：这笔 code checkpoint 已落库；后续如果继续这条线，应只补这轮的 memory/审计结算，或再开新的窄切片，不要把 `Data/Core` 三文件和其他大根重新混回同一刀。

### 本轮实际完成
1. 重新按 `thread-state` 缩窄白名单，只保留：
   - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
   - `Assets/YYY_Scripts/Data/Core/ToolRuntimeUtility.cs`
2. 为了让这三个文件能独立通过 codeguard，而不把 `Story / Rendering / Player / Save` 整片脏根一并吞进来，补了两层向后兼容壳：
   - `SaveManager.cs`
     - 对 `StoryProgressPersistenceService` 改成反射可选调用
     - 对 `CloudShadowManager.Export/ImportPersistentSaveData()` 改成反射可选调用
     - 对 `SaveActionToastOverlay` 改成反射可选调用
     - 同时保留本轮真正要收的默认开局语义清理
   - `ToolRuntimeUtility.cs`
     - 去掉对 `PlayerToolFeedbackService.ToolReplacementTone` 新 enum 的硬耦合
     - 保留武器耐久 / 自动替换主链
     - 工具损坏反馈退回到当前 HEAD 已存在的两参接口
3. 通过 `sunset-git-safe-sync.ps1 -Action sync` 完成白名单归仓。

### 提交结果
- 提交 SHA：`0fa99813`
- 提交说明：`2026.04.13_存档系统_01`
- 影响文件：
  - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
  - `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
  - `Assets/YYY_Scripts/Data/Core/ToolRuntimeUtility.cs`

### 过线证据
- `sunset-git-safe-sync.ps1 -Action sync -OwnerThread 存档系统 -IncludePaths <Data/Core 三文件>`
  - `own roots remaining dirty 数量 = 0`
  - `代码闸门适用 = True`
  - `代码闸门通过 = True`
  - `代码闸门原因 = 已对 3 个 C# 文件完成 UTF-8、diff 和程序集级编译检查`
- `validate_script SaveManager.cs / SaveDataDTOs.cs / ToolRuntimeUtility.cs`
  - `owned_errors = 0`
  - 当前仍统一停在 `unity_validation_pending / stale_status`
  - 说明这轮没有 fresh owned red，但 Unity 侧仍不是 fresh-ready 基线

### 当前判断
- 这笔提交成立的是：`Data/Core` 三文件的代码 checkpoint 已经合法落库。
- 这笔提交没有宣称的是：整个存档系统已经全部收完。
- 更准确的人话：
  - 我先把最容易继续腐烂的同根 code tail 收进去了
  - 但 save 线程在 shared root 上仍有大量其他历史脏根，不能因为这一笔 code checkpoint 成立，就误判为“存档线整体 clean”

### 额外报实
- `sync` 成功后，shared root 上又出现了他线新提交，当前 `HEAD` 已继续前移。
- 但 `0fa99813` 已经进入 `main` 历史，这不影响本笔 checkpoint 成立。

## 2026-04-11 13:47 施工收口：默认开局旧壳已清，典狱长 prompt 已生成

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是存档系统收口。
- 本轮子任务有且只有两件事：
  1. 真的把 `FreshStartBaseline / 默认开局旧壳` 从 save 主链里清掉
  2. 给典狱长生成一份只收 `portfolio-review` 外发版体系的硬切 prompt
- 恢复点：当前 save 线程 own 已完成，后续如果继续回到这条线，应转向 packaged live 噪音/护栏/build smoke；外发版体系改由典狱长位接手。

### 本轮实际落地
1. `SaveManager.cs` 已清掉默认开局旧壳：
   - `BootstrapRuntime()` 不再调 baseline 捕获，只保留 `Instance` 自举
   - 删除了旧 baseline 的自动捕获/修复/退出写回空壳：
     - `ScheduleFreshStartBaselineCapture()`
     - `CaptureFreshStartBaselineRoutine()`
     - `EnsureDefaultProgressPreparedForRuntime()`
     - `IsValidRuntimeDefaultSave()`
     - `TryRepairDefaultProgressFromBaseline()`
     - `TryPersistDefaultProgressOnQuit()`
   - 默认开局不再映射到任何落盘 baseline 文件
   - 旧文件名 `__fresh_start_baseline__` 仅作为 legacy reserved name 保留，用于隐藏历史残留文件，避免它重新混成普通存档
   - `SaveGameInternal()` 现在会直接拒绝：
     - 默认槽
     - 旧 legacy baseline 槽
2. 已新增给典狱长的 prompt 文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-11_给典狱长_portfolio-review外发版体系最小闭环统筹_01.md`
   - 这份 prompt 已明确：
     - 存档线程 own 只收默认开局旧壳
     - 典狱长只收 `portfolio-review` 外发版体系
     - 不允许再把 save / UI / Day1 / Town 业务线一起打包成泛治理大作文

### 本轮验证
- `python .\scripts\sunset_mcp.py validate_script Assets/YYY_Scripts/Data/Core/SaveManager.cs --count 20 --output-limit 8`
  - assessment=`unity_validation_pending`
  - owned/external errors = `0/0`
  - native validation = warning 3（旧的泛扫描类 warning，不是 fresh compile red）
  - 卡点仍是 Unity `stale_status`
- `python .\scripts\sunset_mcp.py errors --count 20 --output-limit 8`
  - `errors=0 warnings=0`
- `git diff --check`
  - shared root 里仍有大量他线历史 trailing whitespace 噪音，不能作为我 own 失败判断
  - 本轮未新增 fresh owned compile red

### 当前最准确结论
- 存档线程这轮 own 已完成：
  - 默认开局旧壳已真正收掉
  - 典狱长 prompt 已生成
- 当前这条线不再需要继续包办外发版体系
- 下一轮若继续：
  - save 线程只该回到 packaged live 噪音/护栏/build smoke
  - `portfolio-review` 体系改由典狱长位统筹

### thread-state
- 本轮已跑：
  - `Begin-Slice`
  - `Park-Slice`
- 未跑：
  - `Ready-To-Sync`
- 当前 live 状态：`PARKED`
- 停车原因：本轮 own 已完成，等待用户转发典狱长 prompt 或下一轮裁定

## 2026-04-11 13:12 讨论裁定：先清默认开局旧壳，再把外发版体系交典狱长统筹

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是存档系统收口。
- 本轮子任务：回应用户提出的新拆法，判断是否应该把“默认开局旧壳清扫”和“portfolio-review 外发版体系”拆成两条责任线。
- 恢复点：后续若进入真实施工，存档线程先做 `FreshStartBaseline` 旧壳清扫；外发版体系改由典狱长统筹后续 prompt 与落地。

### 裁定结果
- 同意这个拆法，而且比我上一轮“我自己先清日志再 build smoke”的顺序更合理。
- 当前正确拆分应是：
  1. 存档线程先做 `FreshStartBaseline` / 默认开局旧壳清扫
  2. 典狱长再接“portfolio-review 外发版体系”这条更大的外发链
  3. 最终由外发链把打包前清理、构建配置、候选包与打包后复检整合起来

### 为什么这样拆更对
- `FreshStartBaseline` 旧壳属于存档语义本体：
  - 默认槽到底是什么
  - 会不会自动写回
  - F9 / 默认开局到底恢复什么
  这些都应该先由存档线程收干净
- “外发版/portfolio-review”已经不只是存档问题，而是更大的发布体系问题：
  - `Mono -> IL2CPP`
  - `MCPForUnity.Runtime.dll`
  - `DoNotShip`
  - 开发入口隔离
  - 发布命名/README/构建语义
  这更适合交给典狱长统筹，而不是继续压在存档线程上

### 我对后续边界的建议
- 存档线程负责：
  - 默认开局语义彻底清一遍
  - 确保默认槽不再带历史自动基线残留
- 典狱长后续可统筹：
  - packaged live 日志噪音清扫
  - `portfolio-review` 构建体系
  - 候选外发包的打包后复检
- 但最后的 packaged save smoke 仍应带上存档验收项，不应完全脱离存档线程定义

### thread-state
- 本轮只读讨论
- 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-17 只读复盘｜`Primary -> Home -> Primary` 物品回弹并不是单纯 world-state 没存好

### 用户目标
- 用户要求这轮暂停继续补 case，先重新做一次更底层的大调查：
  1. 为什么 `Primary` 丢掉物品，进 `Home` 看起来正常，回 `Primary` 又回到背包里
  2. 为什么地上那份还在，捡起后会临时双份，再切场又掉回去
  3. 为什么这条线一直修不透

### 本轮只读结论
1. 三个主场景都各自内置整套 scene-local 根：
   - `Systems`
   - `InventorySystem`
   - `HotbarSelection`
   - `EquipmentSystem`
   - `UI`
   - `EventSystem`
   - `PersistentManagers`
2. `PersistentPlayerSceneBridge` 是 runtime bootstrap，不在场景里；所以切场时一定会同时遇到：
   - 旧场景留下来的 persistent 根
   - 新场景刚加载出来的 scene-local 根
3. 当前最高置信根因位于 `PersistentPlayerSceneBridge.RebindScene()`：
   - 先 `PromoteSceneRuntimeRoots(scene)`，把 duplicate roots 标记为 `Destroy`
   - 但同一帧后面又立刻：
     - `ResolveRuntimeInventoryService(scene)`
     - `ResolveRuntimeHotbarSelectionService(scene)`
     - `ResolveRuntimeInputManager(scene)`
     - `RebindPersistentCoreUi(...)`
     - `RebindSceneInput(...)`
   - `Destroy()` 是帧末才真正销毁，所以这里拿到的仍然可能是“已经判死但还没消失”的 duplicate
4. 这不是一处小漏，而是整条库存/UI链都在放大它：
   - `ToolbarUI`
   - `InventoryPanelUI`
   - `InventoryInteractionManager`
   - `BoxPanelUI`
   - `InventorySlotInteraction`
   - `GameInputManager`
   - `PlacementManager`
   - `PlayerInteraction`
   - `CraftingService`
   - 上述链路里存在大量 `FindFirstObjectByType<InventoryService / HotbarSelectionService / PackagePanelTabsUI / EquipmentService>` 回退
5. 用户最新复现因此可以统一解释为：
   - 掉落物是一套事实
   - persistent 背包可能是一套事实
   - package / box / inventory UI 又可能临时绑到另一套 scene-local duplicate
   - 所以才会出现：
     - 在 `Home` 看起来对
     - 回 `Primary` 又回弹
     - 地上那份还在
     - 捡起后临时双份
     - 再切场又掉回去
6. 额外高风险点：
   - 三个场景里的 `InventoryService` 都带同一个 `_persistentId`
   - `EquipmentService` 代码仍是固定单例 ID
   - duplicate 活着的窗口里，注册中心 / 恢复层也存在撞 ID 风险
7. `InventoryBootstrap` 已排除为这轮 build/live 主因：
   - `Primary` 有它，但当前序列化是 `runOnBuild = 0`
   - `Home / Town` 没有它

### 当前判断
- 这轮最关键的新判断是：
  - 之前一直补 `Tree / Drop / Chest / FarmTile / Crop` 的方向并不是白做
  - 但它已经压不住更底层的“切场后到底谁才是那份真的背包 / 热栏 / 输入 / Package UI”问题
- 所以下一刀若进入真实施工，第一优先级不该再是单个 world-state case，而应先收：
  1. persistent 根已存在时，桥接层不能再把 scene-local `InventorySystem / HotbarSelection / EquipmentSystem / Systems / EventSystem / UI` 当 runtime 真源
  2. bridge / box / inventory / package 链都要收成同一套 runtime context
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

## 2026-04-17 追加｜门链与真源继续收口：背包/热栏快照改抓 authoritative runtime，直载入口统一回 bridge

### 本轮完成
1. `PersistentPlayerSceneBridge.CaptureSceneRuntimeState()` 改成只抓 resolved runtime：
   - `ResolveRuntimeInventoryService(scene)`
   - `ResolveRuntimeHotbarSelectionService(scene)`
   不再用场景副本做离场快照来源。
2. `SpringDay1Director` 内 3 处 `SceneManager.LoadScene(..., LoadSceneMode.Single)` 已统一通过 `LoadSceneThroughPersistentBridge()` 转接，先 `QueueSceneEntry()` 再切场。
3. `DoorTrigger` 也补了 `QueueSceneEntry()`，避免普通门链仍有绕过 bridge 的旧口。
4. `CraftingService / HotbarSelectionService / InventorySortService / AutoPickupService / PlacementManager / PlayerInteraction / SaveManager` 已补 bridge 优先真源，降低 scene-local duplicate 重新被绑定的概率。
5. `WorldStateContinuityContractTests` 已同步新增相关合同断言。

### 验证
1. `manage_script validate`
   - 所有本轮改动脚本：`0 error`
   - 仅保留既有 warning
2. EditMode：
   - `WorldStateContinuityContractTests`
   - `WorkbenchInventoryRefreshContractTests`
   - `17/17 passed`
3. fresh console：
   - `errors=0 warnings=0`

### 结论
1. 这轮已经把“离场快照抓错场景副本 + 直切场绕过 bridge”两条主根因同时收进去。
2. 当前这条子线距离“可直接打包的最小态”只差 live / packaged 冒烟，不再差新的大块代码语义。

## 2026-04-17 追加｜箱子运行时摆位纠偏：以场景 authored closed pose 为真

### 用户目标
- 用户当前最新反馈不是“箱子还在继续越长越高”，而是更精确的一层：
  - 场景里摆好的箱子位置，与编辑器 Play / 打包运行看到的位置不一致。
  - 用户明确裁定：`closed` 状态在场景里怎么摆，运行时就必须怎么显示；open / close 只能相对这个真值变化，不能整体重摆一遍。

### 本轮代码修正
1. `ChestController.Awake()` 在迁移 SpriteRenderer 之前，先执行 `CaptureAuthoredVisualBaseline(_spriteRenderer)`。
2. 新增 authored 视觉基线字段：
   - `_authoredVisualLocalPosition`
   - `_authoredBottomLocalY`
   - `_authoredVisualBaselineCaptured`
3. 新增 `ApplyAuthoredVisualPoseToRenderer()`：
   - root SpriteRenderer 迁成 `__ChestSpriteVisual` 后，立即把 child 复位到 authored pose，而不是从 `localPosition=0` 再重贴底。
4. `AlignSpriteBottom()` 改为：
   - 不再执行 `localPos.y = -spriteBottomOffset`
   - 改为 `localPos.y = _authoredBottomLocalY - spriteBottomOffset`
   - 也就是只让当前 sprite 的底边对齐到“作者摆好的底边基线”
5. root transform 仍保持不动；Collider 偏移链继续跟随 visual child。

### 验证
1. `validate_script Assets/YYY_Scripts/World/Placeable/ChestController.cs`
   - owned error = `0`
   - native validation = `warning`
   - warning 内容为既有 `Update()` 字符串拼接 GC 提示
   - CLI assessment = `unity_validation_pending`（原因是 Unity 编译轮询超时，不是 owned compile fail）
2. `validate_script Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs`
   - owned error = `0`
   - native validation = `clean`
   - CLI assessment = `unity_validation_pending`（同上）
3. fresh console：
   - `errors=0 warnings=0`
4. EditMode：
   - `WorldStateContinuityContractTests.ChestController_ShouldNotMoveRootTransformWhenAligningSpriteBottom`
   - `1/1 passed`

### 当前判断
1. 箱子这条视觉错位的根因已经从“数学贴底”修成“authoring pose 为真”。
2. 这轮能明确 claim 的层级是：
   - `结构 / checkpoint` 成立
   - `targeted probe / 合同测试` 成立
3. 还不能 claim：
   - `真实入口体验已过`
   - 因为还缺 Home 箱子的 live / packaged 目测复核。

## 2026-04-17 追加｜玩家背包：sort / 垃圾桶 / runtime rebind 收口

### 用户目标
- 用户明确把当前问题重新收窄为“玩家背包”，不是箱子：
  1. 背包 `sort` 语义不对
  2. `垃圾桶` 要检查
  3. 跨场景后背包问题更严重

### 代码修正
1. [InventorySortService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Inventory/InventorySortService.cs)
   - `SortStartIndex = 0`
   - 整包 `0..Capacity-1` 统一整理
   - 每次优先吃 bridge 当前 runtime inventory / database
2. [InventoryService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Inventory/InventoryService.cs)
   - `Sort()` fallback 不再走旧的 `_inventoryData.Sort()` 语义
   - 已改成与 `InventorySortService` 同一条整包整理合同
3. [InventoryInteractionManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs)
   - 新增 `ResolveRuntimeContext()`
   - 背包交互、归位、sort 前都会先重绑当前 runtime inventory / equipment / sortService
   - 垃圾桶改走 `DiscardHeldItem()`，不再掉到玩家脚下
4. [InventorySlotInteraction.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs)
   - `SlotDragContext` 拖到垃圾桶时改成真删除
   - 只有拖到面板外才继续走掉地逻辑
5. [InventoryPanelUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs)
   - 面板重开时不再只在字段为 null 时才补 runtime context
   - 当前会优先覆盖成 bridge 当前真源

### 验证
1. Unity `validate_script`
   - `InventorySortService / InventoryService / InventoryPanelUI / InventoryRuntimeSelectionContractTests` = `0 error / 0 warning`
   - `InventoryInteractionManager / InventorySlotInteraction` = `0 error / 1 warning`（既有 GC warning）
2. fresh console：
   - `errors=0 warnings=0`
3. EditMode：
   - `InventoryRuntimeSelectionContractTests.InventorySort_ShouldUseWholeBackpackAndRuntimeAuthoritativeContext`
   - `InventoryRuntimeSelectionContractTests.InventoryTrash_ShouldDiscardHeldItemInsteadOfDroppingAtPlayerFeet`
   - `InventoryRuntimeSelectionContractTests.SceneRebind_ShouldRefreshInputAndSortRuntimeContext`
   - `3/3 passed`

### 当前判断
1. “箱子 sort 正常、背包 sort 一套旧语义”的裂缝已经补上。
2. “垃圾桶是假删除、其实把东西丢地上”的伪语义也已经补正。
3. 当前代码层剩余风险已经从“逻辑未落地”降成“还缺真实三场景 live / packaged 复测”。

## 2026-04-17 21:05 箱子视觉/碰撞与 Home 强刷链诊断补记

### 用户当前主线
- 用户把本轮重新收窄为两个只读问题：
  1. 为什么箱子在编辑器里摆好一个样，运行后视觉和碰撞都乱
  2. 为什么进入 `Home` 后背包某些坏相会好，但普通 `Town <-> Primary` 切场没学到同样刷新

### 静态结论
- 箱子这边现在的真问题不是“Unity 自己乱飞”，而是 [ChestController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/World/Placeable/ChestController.cs) 仍在运行时重建一套自己的视觉参考系：
  - `Awake()` 里先拿当前 `_spriteRenderer` 抓 authored baseline
  - 随后 `EnsureSpriteRendererUsesVisualChild()` 强行创建或接管 `__ChestSpriteVisual`
  - 后续 `UpdateSpriteForState() / AlignSpriteBottom() / UpdateColliderShape()` 都围绕这个运行时 child 继续算
- 这会直接违背用户要的语义：
  - `编辑器里怎么摆，运行就该怎么看`
  - 当前代码不是忠实吃场景现状，而是“先抓一版、再换一套 runtime 承载结构”
- 碰撞体“像跑飞了”不是第二个独立 bug，而是同一条链的连锁：
  - `PolygonCollider2D` 仍挂在 root
  - `UpdateColliderShape()` 会把 physics shape 叠加 `GetColliderVisualLocalOffset()`
  - 一旦 visual child 的 local pose 不是编辑器正式面，碰撞也会一起偏

### 额外证据
- `Home.unity` 当前没有现成的 `__ChestSpriteVisual` 序列化对象；说明这层 child 是运行时新造的，不是场景作者原本摆好的那层。
- `Home` 里的箱子 prefab instance 只有正常的位置 override 和 authoringSlots override，没有“作者本来就多摆了一层 runtime child”的证据。

### 背包 4/8 与切场刷新结论
- 用户说“进 Home 后 4/8 会恢复”这个观察是有价值的，方向是对的。
- 目前最像的真解释不是 `Home` 本身有魔法，而是 `BoxPanelUI` 的刷新比普通切场更重：
  - `RefreshRuntimeContextFromScene()`
  - `RefreshInventorySlots()`
  - 对每个槽位重新 `Bind(...) + Refresh() + RefreshSelection()`
- 普通 `PersistentPlayerSceneBridge.RebindScene()` 虽然也会做 runtime rebind，但现在更偏“服务级重绑”：
  - 改 inventory / hotbar / input / persistent UI 引用
  - 不等于所有背包显示链都执行了 `BoxPanelUI` 这种逐格强刷
- 所以用户说的“把进 Home 时那套默认切场刷新，学到 Town 和 Primary 往返上”是正确方向。

### 恢复点
- 下一刀真正该修的，不是继续猜偏移数值，而是：
  1. 箱子 closed 态必须以场景当前正式视觉为唯一真值
  2. 碰撞必须跟同一真值走，不能自己再算另一套偏移
  3. `Home` 里能把 4/8 拉回来的那条强刷链，要推广成普通切场后的统一刷新合同

## 2026-04-17 23:48 箱子 authored 真值修正 + 普通切场统一强刷新落地

### 本轮完成
1. [ChestController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/World/Placeable/ChestController.cs)
   - root `SpriteRenderer` 迁到 `__ChestSpriteVisual` 时，authoring 基线改为 root-local 真值
   - 这次不再把场景摆位、旋转、缩放再套第二遍
   - collider 也改成逐点走 `TransformSpritePhysicsPointToChestLocal(...)`
2. [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
   - `InventoryPanelUI`、`InventoryInteractionManager` 都改成显式 `ConfigureRuntimeContext(...)`
   - 再叠加这轮前面已落的 `sceneInventory.RefreshAll()` / `ReassertCurrentSelection(...)` / `activeBoxPanel.RefreshUI()`
   - 普通切场后的 UI 强刷合同已经比之前更接近 `Home/BoxPanelUI` 那条重绑路径
3. 合同测试同步补强：
   - [WorldStateContinuityContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs)
   - [InventoryRuntimeSelectionContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/InventoryRuntimeSelectionContractTests.cs)

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

### 补记结论
1. 箱子“编辑器里一个样、运行后乱掉”的最高置信根因，已经从代码层被改成 authored 真值优先。
2. 普通切场这边也不再只靠 `Home` 或打开箱子 UI 才顺手恢复；统一强刷合同已经继续往 bridge 主链上收。

## 2026-04-18 00:12 Home 后背包 4/8 异常补刀

### 新坏相
- 用户最新 live 反馈：进入 `Home` 后，背包 `4/8` 槽仍异常。

### 最小修复
1. [InventoryPanelUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs)
   - `ConfigureRuntimeContext(...)` 现在会主动 `InvalidateSlotBindings()`
   - 让 runtime context 更新后必走一次 `BuildUpSlots() / BuildDownSlots()`
2. `RefreshAll()` 现在也会先 `EnsureBuilt()`
3. [InventoryRuntimeSelectionContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/InventoryRuntimeSelectionContractTests.cs)
   - 已补对应合同断言

### 判断
1. 这说明普通切场还不只是 bridge 服务层要重绑，背包页自己的槽位缓存也必须一起失效。
2. 当前这刀是最小、安全、贴着用户坏相的修正，不是继续绕回更大的 world-state 漫游。

## 2026-04-18 只读补记｜toolbar 固定槽位 4/8 切场异常更像 ToolbarUI 自身重绑脆弱

### 用户新增目标
1. 用户要求只读排查：
   - 为什么 toolbar 固定槽位 `4/8`（第 `5/9` 格）切场后图标/显示异常
   - 为什么进出 `Home` 又会恢复
   - 打包前最安全的最小修复应该怎么收
2. 本轮明确不要改代码，只收根因判断。

### 本轮补读
1. UI / bridge / selection 主链：
   - [ToolbarUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs)
   - [ToolbarSlotUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs)
   - [InventoryPanelUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs)
   - [BoxPanelUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs)
   - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
   - [HotbarSelectionService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs)
2. authored 结构核对：
   - [ToolBar.prefab](D:/Unity/Unity_learning/Sunset/Assets/222_Prefabs/UI/0_Main/ToolBar.prefab)
   - `Home.unity / Primary.unity / Town.unity` 内 `ToolBar/Grid` 子节点顺序

### 新判断
1. 当前最高概率根因不在 `InventoryPanelUI` 或 `BoxPanelUI`：
   - 这两条链都是按循环索引直接 `Bind(..., i, ...)`
   - runtime context 更新后也都有显式的 `ConfigureRuntimeContext(...) / EnsureBuilt() / Refresh...()`
2. 当前最脆弱的点集中在 [ToolbarUI.Build()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs)：
   - toolbar 是这条链里唯一还在依赖“子物体名字解析索引”的地方
   - `ResolveToolbarSlotIndex()` 只认 `Bar_00_TG` 名字后缀
   - `Build()` 还会先排序、再按 `boundIndices` 去重
   - 场景切换时 [PersistentPlayerSceneBridge.RebindPersistentCoreUi()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs) 每次都会重新 `toolbarUi.Build(); toolbarUi.ForceRefresh();`
3. 这意味着：
   - 只要 toolbar 运行时子节点命名 / 绑定时序 / 去重条件有一点漂移
   - toolbar 就会比 inventory panel / box panel 更容易留下“个别格子显示错，但服务真源其实已经重绑”的坏相

### 为什么现在不把 4/8 认成 scene authored 顺序问题
1. 本轮静态核对结果：
   - `ToolBar/Grid` 在 `Home / Primary / Town` 里的 12 个子节点顺序一致
   - authored 名字也都是从 `Bar_00_TG` 到 `Bar_00_TG (11)` 的标准序列
2. 所以 `4/8` 不是代码里被硬编码成特殊位，也不像 prefab / scene 写错顺序。
3. 更像是：
   - toolbar 重绑脆弱是“通病”
   - `4/8` 只是当前玩家最稳定暴露问题的槽位
   - 很可能因为这两个位上常驻的是更容易看出显示异常的工具/运行态道具

### 为什么进出 Home 会恢复
1. `PersistentPlayerSceneBridge` 的 persistent UI 根通常就是从 `Home` 这套 authored UI 提升出来的。
2. 回到 `Home` 时：
   - toolbar hierarchy 和 persistent UI baseline 最一致
   - bridge 会再次执行 `Build + ForceRefresh + sceneInventory.RefreshAll() + sceneHotbarSelection.ReassertCurrentSelection(...)`
3. 所以用户体感上的“进出 Home 能恢复”，更像是：
   - 回到了 persistent UI 的原始 authored 基线
   - 把普通切场里留下的某些 toolbar 局部脏态重新刷正了

### 打包前最安全的最小修复建议
1. 最小刀口应优先只收 [ToolbarUI.Build()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs)，不要再扩到 inventory / box / save 主链：
   - 不再靠 `ResolveToolbarSlotIndex()` 用名字推断槽位
   - 直接按 `gridParent` 的 sibling 顺序 `0..11` 绑定
   - 最多保留一个校验日志，发现 childCount 不对再报错
2. 这是当前最安全的原因：
   - prefab 与三大主场景的 `Grid` 顺序已经静态证实一致
   - inventory panel / box panel 也早就在用“按顺序直接绑”的策略
   - 改 toolbar 去和它们统一，风险最小、解释力最强
3. 如果这一刀后坏相仍在，再继续看：
   - `ToolbarUI` 订阅恢复
   - `ToolbarSlotUI` runtime context 刷新时序
   - `HotbarSelectionService` 快照是否还需要带 `selectedInventoryIndex`

## 2026-04-17 22:53 补记｜off-scene 树成长补票 + Primary 遮挡运行时重绑

### 用户新反馈
1. 用户继续在存档主线下追加两条真实坏相：
   - 离场场景的树跨天后不成长
   - `Primary` 里玩家被树/石/房屋挡住时不透明，但 `Town` 正常
2. 用户同时报实：
   - `farm` 这一轮体感上已经恢复正常
   - 这轮不要漂回 UI 漫修，而是继续收打包前最小必修链

### 本轮已落地
1. [SaveDataDTOs.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs)
   - `SceneWorldSnapshotSaveData` 新增：
     - `hasCapturedTotalDays`
     - `capturedTotalDays`
2. [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
   - `CaptureSceneWorldRuntimeState(scene)` 现在会一起记录 `totalDays`
   - `ExportOffSceneWorldSnapshotsForSaveInternal(...) / ImportOffSceneWorldSnapshotsFromSaveInternal(...)`
     已把离场天数正式写进 / 读回 off-scene 存档链
   - `RestoreSceneWorldRuntimeState(scene)` 恢复时会先算 `elapsedDays`
   - 对树对象新增 `ApplySceneWorldCatchUp(...)`，让离场场景回场时补掉经过天数
   - 切场重绑新增 `RebindSceneOcclusionManagers(scene)`，统一把场景里的 `OcclusionManager` 指向 persistent player
3. [TreeController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/TreeController.cs)
   - 新增 `ApplyOffSceneElapsedDays(int elapsedDays, int targetTotalDays)`
   - 当前最小合同只给树补离场期间经过的天数，不在这刀里扩成所有 world-state 的统一跨天模拟
4. [OcclusionManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs)
   - `Start()` 统一走 `RefreshPlayerBindings(forceSearch: true)`
   - `Update()` 前新增 `EnsurePlayerBindings()`
   - 新增 `RefreshRuntimePlayerBinding(Transform runtimePlayer)` 供 bridge 切场时显式重绑
   - `playerLayer` 不再只在 `Start()` 缓存一次，运行中会跟当前真实玩家 sprite 一起刷新
5. 合同测试补强：
   - [WorldStateContinuityContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs)
   - 已补：
     - off-scene snapshot 必须记录离场天数
     - 树回场必须做 catch-up
     - OcclusionManager 必须在切场后重绑到 persistent player

### 对只读子线程结论的处理
1. `Fermat` 只读回执指出：
   - `Primary` / `Town` 的 manager 参数差异不大
   - `playerLayer` 的一次性缓存确实是代码面嫌疑
   - `Primary.unity` 后续仍值得继续看实例覆写
2. 本轮没有直接去改 `Primary.unity`
   - 因为 scene authoring 改动属于高风险面，且当前最小安全修法先能落在脚本侧
   - 如果用户回测后 `Primary` 仍异常，下一刀才值得按 scene-audit 口径继续核具体实例覆写

### 验证
1. `git diff --check`
   - 当前 owned 文件通过
2. `py -3 scripts/sunset_mcp.py validate_script`
   - `SaveDataDTOs.cs + TreeController.cs + OcclusionManager.cs + PersistentPlayerSceneBridge.cs`
   - `owned_errors=0`
   - assessment=`unity_validation_pending`
   - blocker 仍是 Unity `stale_status`
3. `py -3 scripts/sunset_mcp.py manage_script validate`
   - `TreeController`：`errors=0 warnings=1`
   - `OcclusionManager`：`errors=0 warnings=2`
   - `PersistentPlayerSceneBridge`：`errors=0 warnings=2`
4. `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`
   - `errors=0 warnings=0`

### 当前判断
1. 树的跨场景跨天冻结，这轮已经从“只读结论”推进到正式存档链代码层补口。
2. `Primary` 遮挡这轮先收的是更安全的运行时重绑与玩家层自愈，不是直接去改 production scene。
3. `farm` 这轮没有被我继续改；用户体感恢复更像是之前 bridge/world-state 收口后的连锁缓解，而不是这刀直接修出来的。

## 2026-04-18 只读补记｜world-state live runner 在 bootstrap passed 后静默卡住的静态结论

### 本轮目标
1. 不改代码，只解释 [PersistentWorldStateLiveValidationRunner.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentWorldStateLiveValidationRunner.cs) 为什么停在：
   - `[WorldStateLive] awake`
   - `[WorldStateLive] run_started`
   - `[WorldStateLive] scenario=bootstrap passed=True`
2. 重点区分：
   - runner 自身 orchestration / lifecycle 漏洞
   - 业务链同步调用点是否可能成为触发源

### 本轮读取
1. [PersistentWorldStateLiveValidationRunner.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentWorldStateLiveValidationRunner.cs)
2. [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
3. [FarmTileManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Farm/FarmTileManager.cs)
4. [CropController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Farm/CropController.cs)
5. [ChestController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/World/Placeable/ChestController.cs)
6. 当前 `Editor.log` 中最新 `WorldStateLive` 片段

### 静态结论
1. 现象最像卡在 runner 自己的 `post-bootstrap -> first scenario` 交接窗口，而不是已经进入 `QueueSceneEntry / LoadScene` 后的场景切换链。
2. 代码原因有两层：
   - `StartRun()` 先 `StartCoroutine(RunValidation())`，再 `StartCoroutine(WatchdogRun())`
   - 如果 `RunValidation()` 在第一次把控制权还给 `StartRun()` 前，就同步卡进 `RunPrimaryHomePrimaryScenario()` 的前置准备区，watchdog 根本还没启动
3. 这解释了为什么当前会出现：
   - 没有 `scenario_start`
   - 没有 watchdog 失败条目
   - 没有 report 落盘
4. 如果一定要在 runner 调到的业务链里挑最像的同步触发源，优先级是：
   - `TryPreparePrimaryState()` 内的 `FarmTileManager.CreateTile / SetWatered`
   - 然后才是 `CropController.Initialize`
   - `ChestController.SetSlot`
   - `PersistentPlayerSceneBridge.QueueSceneEntry / LoadScene`
5. 但这四条里，真正把“卡住”放大成“完全静默且无 report”的主因，仍然是 runner 自己：
   - watchdog 启动时机太晚
   - `OnDisable / OnDestroy` 只记日志、不兜底 `FinalizeRun()`

### 当前最小最安全修法建议
1. 第一刀先修 runner，不先动业务链：
   - 先启动 watchdog，再启动 `RunValidation()`
   - `bootstrap passed` 后显式补一个过渡 heartbeat / log，必要时 `yield return null`
   - `OnDisable / OnDestroy` 若本轮尚未 `finalized`，补一个兜底失败 report
2. 第二刀才是定位业务同步触发源：
   - 在 `TryPreparePrimaryState()` 前后按 `chest / create_tile / set_watered / crop_initialize` 打最小进度点
3. 当前不建议先改 `PersistentPlayerSceneBridge` 场景切换链：
   - 因为从现象上看，这次还没稳定走到 `transition_started`

### 验证状态
1. 本轮是纯静态代码 + 当前 `Editor.log` 复核。
2. 结论类型：
   - `runner 自身问题为主`
   - `业务链同步触发源为次要候选`
   - `尚未 live 复证`

## 2026-04-18 补记｜默认存档语义已从聊天与追加记录回写到 0417 前板

### 当前主线 / 子任务 / 恢复点
- 当前主线没变，仍是存档系统的打包前最小收尾。
- 本轮子任务不是继续改 runtime，而是把已经落地的 `默认存档 / F5 / F9 / 重新开始` 施工结果，正式同步回 `0417` 的前板、任务板和测试板，避免主控板前半段继续挂旧语义。
- 恢复点：这条线现在应统一按“默认存档是真实受保护槽”继续，不再把 `默认开局 / F5 停用` 当成当前产品口径。

### 本轮完成
1. `0417.md` 已同步修正：
   - 当前统一裁定
   - `A3` 正确语义
   - `E2-F`
   - `T8 / T9`
   - 任务 `12.1 / 12.3 / 12.4 / 12.5`
   - `R8-A ~ R8-F`
2. 当前固定口径已经回到：
   - 默认槽 = 真实受保护 `默认存档`
   - `F5` = 快速保存默认存档
   - `F9` = 快速读取默认存档
   - `重新开始` = 独立危险操作，不自动改写默认存档
3. 这轮没有继续改 `SaveManager / PackageSaveSettingsPanel` 业务代码：
   - 代码改动本身来自上一刀
   - 这轮做的是主控板与记忆层收口

### fresh 验证
1. `validate_script`
   - `SaveManager.cs`：`assessment=no_red`
   - `PackageSaveSettingsPanel.cs`：`assessment=no_red`
   - `SaveManagerDefaultSlotContractTests.cs`：`assessment=no_red`
2. fresh console：
   - `errors=0 warnings=0`
3. `git diff --check`
   - 当前 docs / memory 自身无格式红错

### 当前判断
1. `默认存档` 这条线现在已经不是“待设计”，而是“代码层已过、live / packaged 待验”。
2. 后续如果继续收 `R8`，正确动作应是：
   - 先做 `F5 / F9 / 默认槽复制 / 重新开始` 的 live / packaged 真实回归
   - 不再重开一轮“默认槽到底叫什么 / F5 到底是不是停用”的语义争论

## 2026-04-18 连续施工｜P0-B / P0-D / P0-C 补口联动

### 本轮目标
1. 把 `P0-B` 剩下最硬的一条输入门控旧壳代码层收住。
2. 把 `P0-D` 的 canonical restore 合同测试补到 `StoryProgressPersistenceService -> SpringDay1Director`。
3. 把 `P0-C` 的 world-state runner 从“只能手动停才有 report”推进到“自己会落失败票”。

### 已落地代码
1. `GameInputManager.ResetPlacementRuntimeState(...)`
   - 现已硬复位 `_inputEnabled`
   - 取消自动导航与旧农具运行态
   - 清掉 `InventoryInteractionManager` / `InventorySlotInteraction` held 壳
   - 强解 `ToolActionLockManager`
2. `InventoryPanelUI / BoxPanelUI`
   - `ConfigureRuntimeContext(...)` 现已按 `selectedInventoryIndex` 重同步选槽与 follow 状态
3. `SaveManagerDay1RestoreContractTests.cs`
   - 新增对
     - `ApplySnapshot`
     - `ApplySpringDay1Progress`
     - `HasCompletedDialogueSequence`
     - `TryRecoverConsumedSequenceProgression`
     - `LoadSceneThroughPersistentBridge`
     的字符串合同断言
4. `PersistentWorldStateLiveValidationRunner.cs`
   - 拆出 `RunScenarioSequence()`
   - 新增递归 `RunNestedEnumerator(...)`
   - 新增 `Update` watchdog
   - 新增后台 `Timer` watchdog

### fresh 验证
1. `validate_script`
   - `GameInputManager.cs`：`assessment=no_red`
   - `PersistentWorldStateLiveValidationRunner.cs`：`assessment=no_red`
   - `InventoryRuntimeSelectionContractTests.cs`：`assessment=no_red`
   - `SaveManagerDay1RestoreContractTests.cs`：`assessment=no_red`
   - `WorldStateContinuityContractTests.cs`：`assessment=no_red`
2. fresh console：
   - `errors=0 warnings=0`

### fresh live runner 结果
1. 当前 `world-state-live-validation.json` 已能在不手动 `STOP` 的情况下自动生成。
2. 最新失败点：
   - `bootstrap = passed`
   - `primary-home-primary = failed`
   - 失败详情：`watchdog_timeout phase=scene_loaded|scene=Home mode=Single active=Home lastSceneLoaded=Home lastActiveScene=Home`

### 当前判断
1. runner 自己“不写报告”的旧阻塞已基本收掉。
2. 当前更像真实业务 / runtime re-entry 卡在 `Home` 段，而不是 runner 继续静默。
3. `P0-B` / `P0-D` 当前新增内容属于“代码层已补，live 待验”；不应再写成“还没施工”。

## 2026-04-18 只读续记｜重新开始后 toolbar 固定槽位 4/8 异常更像 restart 顺序问题叠加 toolbar 自身刷新脆弱

### 用户目标
1. 只读解释一个限定问题：
   - 为什么“重新开始游戏”（包括存档页重开与直接重开）后，toolbar 固定槽位 `4/8` 会显示异常；
   - 为什么和 NPC 对话后，或进出 `Home` 又会恢复。
2. 范围只限：
   - `SaveManager`
   - `PersistentPlayerSceneBridge`
   - `HotbarSelectionService`
   - `ToolbarUI / ToolbarSlotUI`
   - `InventoryPanelUI`
   - `InventoryInteractionManager`
   - `BoxPanelUI`
   - `PackagePanelTabsUI`
   - 以及 restart / load / rebind / refresh 触点。
3. 不改代码，只输出最可能根因链、恢复原因和最小安全修法。

### 本轮已核读
1. `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
2. `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
3. `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
4. `Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs`
5. `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
6. `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
7. `Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs`
8. `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`
9. `Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`

### 新关键判断
1. 当前代码里已经没有 `4/8` 特判，也没有旧的 `ToolbarUI.ResolveToolbarSlotIndex()` 名字推断链；因此 `4/8` 不是逻辑特殊槽位，而是 restart 后最容易暴露脏显示的受害位。
2. 这次最像的主因不是存档内容真坏，而是 `RestartToFreshGame()` 的时序先让 bridge 把“旧运行态”重新绑回 toolbar，再在同一轮 coroutine 里把 persistent inventory / hotbar 清到 fresh state：
   - `SaveManager.NativeFreshRestartRoutine()` 先 `LoadSceneAsync(Town)`
   - `PersistentPlayerSceneBridge.OnSceneLoaded()` 立刻 `RebindScene()`
   - 里面会 `RestoreSceneInventoryState()`、`RebindHotbarSelection()`、`RebindPersistentCoreUi()`
   - 也就是 toolbar 会先吃到“重开前那一套 runtime snapshot”
   - 随后 `SaveManager.ApplyNativeFreshRuntimeDefaults()` 才调用 `PersistentPlayerSceneBridge.ResetPersistentRuntimeForFreshStart()`
   - 最后才补 `RefreshAllUI()`
3. 所以 restart 路径天然是：
   - `旧态先重绑`
   - `新态后清空`
   只要 toolbar 侧还有任何“一次性 Build/ForceRefresh 没完全洗掉旧显示”的脆弱点，就会被这条顺序放大。
4. 当前 toolbar 侧仍存在一个明确脆弱点：
   - `ToolbarUI.OnDisable()` 只解除订阅，但没有把 `subscribedInventory/subscribedSelection` 清空
   - 之后如果对象走过 disable/enable，而 runtime service 引用还是同一个对象，`SyncInventorySubscription()/SyncSelectionSubscription()` 会因为“引用没变”直接 early-return，不会重新挂回事件
   - 这会让 toolbar 更依赖那次 `Build()/ForceRefresh()` 的一次性重绘，而不是依赖后续 inventory/selection 事件自己纠正
5. 因此当前最高概率链应写成：
   - `restart 的旧态先重绑 -> fresh reset 后只有一次 best-effort UI refresh -> toolbar 自身订阅/刷新合同不够硬 -> 少数槽位留下旧显示`

### 为什么对话或进出 Home 能恢复
1. `Home` 往返恢复最容易解释：
   - 它会再次走 `PersistentPlayerSceneBridge.RebindScene()`
   - 然后再走 `RebindPersistentCoreUi()`
   - 里面会重新：
     - `toolbarUi.Build()`
     - `toolbarUi.ForceRefresh()`
     - `packagePanel.ConfigureRuntimeContext(...)`
     - `packagePanel.EnsureReady()`
     - `packagePanel.ClosePanelForExternalAction()`
     - `sceneInventory.RefreshAll()`
     - `sceneHotbarSelection.ReassertCurrentSelection(...)`
   - 这是一条比 restart 末尾 `RefreshAllUI()` 更“重”的强制收口链。
2. NPC 对话能恢复，说明坏的更像 toolbar 的运行态/选中态/订阅后的展示残留，而不像 save data 本身损坏：
   - `GameInputManager.OnDialogueStart()` 会强制 `CloseBlockingPanelsForDialogueLock()`
   - 面板关闭后，`GameInputManager.Update()` 的 UI 恢复分支会跑 `ReassertCurrentSelectionPreservingInventoryPreference()`
   - 对话本身还会把输入、放置模式、held 壳统一收尾
   - 也就是说，对话路径会再打一轮“把当前 hotbar 真状态重新压回 UI”的整理动作。

### 最小安全修法
1. 第一刀不要去碰 `SaveData` 结构，也不要回头重改 `InventoryPanelUI / BoxPanelUI`。
2. 最小且最安全的收法应优先二选一，顺序建议如下：
   - 第一优先：把 `SaveManager.NativeFreshRestartRoutine()` 的 fresh reset 与 UI 重绑顺序改成“先清 fresh runtime，再做一次 bridge 级 UI rebind”，不要再保留“旧态先绑、fresh 后清”的窗口。
   - 第二优先：补硬 `ToolbarUI` 的订阅恢复合同，在 `OnDisable()` 清空 `subscribedInventory/subscribedSelection`，或在 `OnEnable()` 强制重挂，不允许同引用 early-return 吃掉重订阅。
3. 如果只能做一刀，我更建议先收 restart 顺序：
   - 因为这条能同时解释“存档页重开”和“直接重开”都中招；
   - 而且它正好是对话/Home 不会走到的独有路径。

### 验证状态
1. 本轮仅完成静态代码审查。
2. 结论口径：
   - `restart 先旧态 rebind、后 fresh reset`：静态证实
   - `4/8 没有逻辑特判`：静态证实
   - `ToolbarUI` 订阅恢复合同存在脆弱点`：静态证实
   - `dialogue/Home 是更强的状态重整入口，所以会把异常洗掉`：高概率成立

## 2026-04-18 只读补记｜“加载存档后作物没有保留”与 world-state 恢复链静态结论

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是存档系统的 world-state 收口与打包前风险收缩。
- 本轮子任务：只读排查为什么“加载存档后作物没有保留”，并顺带核清农地 / 浇水 / 作物 / 工作台进度这些恢复链当前代码到底保存了什么、没保存什么。
- 恢复点：如果后续允许真实施工，最小安全入口不是泛修 bridge，而是先补旧农田存档到新 Crop world-object 语义的兼容迁移。

### 本轮已完成
1. 只读审查以下核心脚本与合同测试：
   - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
   - `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
   - `Assets/YYY_Scripts/Data/Core/PersistentObjectRegistry.cs`
   - `Assets/YYY_Scripts/Data/Core/DynamicObjectFactory.cs`
   - `Assets/YYY_Scripts/Farm/FarmTileManager.cs`
   - `Assets/YYY_Scripts/Farm/FarmTileData.cs`
   - `Assets/YYY_Scripts/Farm/CropController.cs`
   - `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
   - `Assets/YYY_Scripts/Service/Crafting/CraftingService.cs`
   - `Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
   - `Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs`
   - `Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
2. 钉死当前正式保存口径：
   - `SaveManager.CollectFullSaveData()` 只把当前 active scene 的 persistent objects 写进 `worldObjects`
   - 已离场 scene 的 world-state 通过 `PersistentPlayerSceneBridge.ExportOffSceneWorldSnapshotsForSave()` 入盘
   - `FarmTileManager` 自己作为 `worldObjects` 里的 `FarmTileManager` 条目保存耕地/浇水/空置寿命
   - `CropController` 自己作为 `worldObjects` 或 off-scene snapshot 里的 `Crop` 条目保存种子、生长、缺水、成熟后天数与格子坐标
3. 确认当前最像“读档后作物消失”的真实代码缺口不在现格式主链，而在 legacy 兼容层：
   - `FarmTileSaveData` 仍保留旧的 `cropId / cropGrowthStage / cropQuality / daysGrown / daysWithoutWater`
   - 但 `FarmTileManager.CreateTileFromSaveData()` 已完全不消费这些旧字段
   - 当前也没有任何归一化步骤把旧农田里的 crop 字段迁移成新的 `Crop` world object

### 关键判断
1. 当前格式的新存档链，农地 / 浇水 / 作物在静态代码层基本已经闭环：
   - 当前 scene：`SaveManager -> PersistentObjectRegistry.RestoreAllFromSaveData()`
   - off-scene：`PersistentPlayerSceneBridge` snapshot capture / import / restore
   - 离场补票：`FinalizeDeferredSceneWorldCatchUp()` 先 `CropController.ApplyOffSceneElapsedDays(...)`，再 `FarmTileManager.ApplyOffSceneElapsedDays(...)`
2. “加载存档后作物没有保留”最强代码解释是旧档兼容断层：
   - 旧档若只在 `FarmTileSaveData` 里存作物，现码会恢复出耕地，但不会重建 crop
   - 结果就是土地还在、浇水状态可能还在、作物本体丢失
3. `GameSaveData.farmTiles` 根字段现在是死 schema：
   - DTO 里还在
   - 但当前保存与读取主链都不再写/读它
   - 真正生效的是 `worldObjects` 内的 `FarmTileManager` JSON 与 `Crop` world object
4. 工作台 / 任务进度需要分两层看：
   - `StoryProgressPersistenceService` 已保存 Day1 教学进度、`craftedCount`、木头进度、完成对白、关系、体力/精力、workbench hint
   - 但“正在制作中的工作台队列”不是漏存，而是显式禁止存档：`CanSaveNow()` 会直接拦截
5. `CraftingService` 本身没有持久化接口：
   - `currentStation` 是 runtime-only
   - `recipe.isUnlocked` 是运行时/资产对象状态，没有 per-save 持久化层
   - 所以通用 crafting 的非 Day1 队列/解锁进度，目前不能算完整 world-state 持久化链

### 验证状态
- 已完成：静态代码审查 + 合同测试文本抽查。
- 未完成：Unity live / packaged 实测。
- 当前口径：`静态推断成立；legacy 农田存档兼容仍有高置信缺口；当前格式 live 待终验`。

### 后续最小安全修法建议
1. 不要回退到旧 `GameSaveData.farmTiles` 根字段继续扩写。
2. 最小安全修法应落在“读档归一化”而不是“运行时多处猜测补洞”：
   - 读取旧档时，如果 `FarmTileSaveData.cropId >= 0` 且当前缺少对应 `Crop` world object，就把旧字段一次性迁移成新的 `CropSaveData / WorldObjectSaveData`
   - 然后仍走现有 `CropController.Load()` 与 bridge/registry 主链
3. 补两类测试：
   - 旧农田存档字段迁移测试：旧 `FarmTileSaveData` 能恢复出 crop
   - 当前格式 round-trip 测试：当前 scene + off-scene crop/farm 连续读档不丢

## 2026-04-18 12:52 只读审查｜restart/load 后 toolbar 固定槽位 4/8 异常链路复核

### 当前主线目标
- 仍服务 `0417` 打包前最小安全闭环；本轮子任务是只读确认 `restart/load -> toolbar 固定槽位 4/8 异常` 在当前最新代码下最可能的根因链、最小安全修法和高风险顺序点。

### 本轮读取范围
1. `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
2. `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
3. `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
4. `Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs`
5. `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
6. `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
7. `Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs`
8. `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`
9. `Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
10. `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`

### 当前最新代码下重新钉实的事实
1. `4/8` 不是当前代码里的特殊槽位：
   - `ToolbarUI.Build()` 已按 `gridParent` sibling 顺序直接 `0..11` 绑定
   - `ToolbarSlotUI` / `HotbarSelectionService` / `GameInputManager` 当前都没有对 `4/8` 做特判
2. `load/restart` 目前都是两拍：
   - 第一拍：`SceneManager.sceneLoaded -> PersistentPlayerSceneBridge.RebindScene()`，先按离场 snapshot 恢复 `inventory/hotbar/ui/input`
   - 第二拍：`SaveManager.ApplyLoadedSaveData(...)` 或 `ApplyNativeFreshRuntimeDefaults()` 再把“真正的读档数据 / fresh 数据”压进去，最后跑 `RefreshAllUI()`
3. `RebindScene()` 里的第一拍不是轻量操作：
   - 会 `RestoreSceneInventoryState()`
   - 会 `RebindHotbarSelection()` 并 `RestoreSelectionState(...)`
   - 会 `RebindPersistentCoreUi()`
   - 会 `RebindSceneInput()`，而 `GameInputManager.ResetPlacementRuntimeState()` 末尾又会 `ReassertCurrentSelectionPreservingInventoryPreference()`
4. 第二拍收尾的 `SaveManager.RefreshAllUI()` 虽然会重刷 UI，但它不是同等级的 bridge/input 重绑：
   - 它会 `ReassertCurrentSelection(...)`
   - 但这一步发生在 toolbars `Build()/ForceRefresh()` 之前
   - 也不会再补一次 `GameInputManager.ResetPlacementRuntimeState()` 那种“输入壳 + 选中态 + 面板壳”统一收尾
5. 当前 `SaveManager` 也没有把 hotbar 选中态当成 save payload 的正式恢复字段：
   - `ApplyLoadedSaveData()` 只恢复时间、玩家、背包、world objects、off-scene snapshots
   - hotbar 选中态实际上来自 `PersistentPlayerSceneBridge` 在切场瞬间抓到的 snapshot

### 当前最强判断
1. 当前最像的主因是顺序层，不是 save data 真坏：
   - `sceneLoaded` 先把“离场前旧 runtime snapshot”重绑回 toolbar / input / selection
   - 随后 `load/restart` 第二拍才写入真正目标数据
   - 但第二拍结尾没有再补一遍 bridge/input 级别的 authoritative rebind
   - 因此 toolbar 更容易留下少数槽位的脏显示或脏选中残留
2. `4/8` 更像当前最稳定暴露坏相的受害位，不像逻辑特判位。
3. `InventoryPanelUI / BoxPanelUI` 虽然各自维护本地 `selectedInventoryIndex`，但它们当前都有显式 `ConfigureRuntimeContext(...) / EnsureBuilt() / Refresh...()`；相比之下，本轮最该先收的仍是 load/restart 顺序层，而不是先泛修面板。

### 为什么 Home / NPC 会恢复
1. `Home` 往返会重新走完整 `PersistentPlayerSceneBridge.RebindScene()`：
   - 再次跑 `RebindPersistentCoreUi()`
   - 再次跑 `RebindSceneInput()`
   - 而 `ResetPlacementRuntimeState()` 会在 UI 已经挂回后再次 `ReassertCurrentSelection...`
   - 所以这条链比 `SaveManager.RefreshAllUI()` 更像一次“强制再对齐”
2. `NPC` 对话恢复也更像 UI/input 再对齐，而不是存档内容突然修好：
   - `GameInputManager.OnDialogueStart()` 会 `CloseBlockingPanelsForDialogueLock()`
   - 面板从开到关后，`Update()` 里的 `_wasUIOpen -> false` 分支会再次 `ReassertCurrentSelectionPreservingInventoryPreference()`
   - 对话链同时会清理 held / placement / queue / lock 等输入运行态

### 最小安全修法建议
1. 第一刀应落在 `SaveManager` 的顺序层，不要先改 DTO，也不要先泛修全部 panel：
   - 把 `load/restart` 的最终 authoritative 数据落稳之后，再补一次 bridge/input 级别的 post-restore rebind
2. 如果只能选一个落点，我当前更建议收在 `SaveManager` 的公共收尾层：
   - 让 `ApplyLoadedSaveData(...)` 与 `NativeFreshRestartRoutine()` 共用同一个“最终态落稳后的 rebind/refresh” helper
   - 目标是消灭“旧态先绑、真态后写，但最后没再强收一次”的窗口
3. 当前不建议第一刀去改：
   - `SaveDataDTOs`
   - `ToolbarSlotUI` 单槽逻辑
   - `InventoryPanelUI / BoxPanelUI` 的本地选槽策略
   因为这些都更像次级风险，不是这轮最小安全闭环的首刀

### 仍待 live 验证
1. 需要确认最小 post-restore rebind 一刀，是否同时收掉：
   - 默认槽读档
   - 普通槽读档
   - 重新开始
   三条入口下的 `4/8` 异常
2. 需要确认 `4/8` 当前到底是：
   - 图标显示残留
   - 选中态残留
   - 还是输入壳/placement 壳残留
   这决定第二刀要不要再补局部 UI contract
3. 需要继续盯的高风险点：
   - `LoadAfterSceneSwitchRoutine()` / `NativeFreshRestartRoutine()` 都会让 `sceneLoaded` 第一拍先发生
   - hotbar 选中态当前不是 save payload 的正式恢复字段
   - `InventoryPanelUI / BoxPanelUI` 都有自己的本地选槽镜像，若后续仍复现，优先看“第二拍后是否还需要统一 reassert”

### thread-state
- 本轮性质：只读分析
- 未跑：`Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`

## 2026-04-18 补记｜Town 只剩 House 4_0 遮挡异常，根因收敛到 OcclusionTransparency 渲染缓存自愈

### 当前主线 / 子任务 / 恢复点
- 主线仍是 `0417` 打包前最小安全闭环；这轮子任务是把用户现场剩下的 `Town / House 4_0` 单点遮挡异常收成最小安全修口，不再继续扩成整套遮挡重构。
- 恢复点更新：
  1. `Town.unity` 里确实存在历史 scene-level 额外添加的 `OcclusionTransparency` 残留，但这轮没有动 production scene。
  2. 真正主修落在 `OcclusionTransparency.cs` 的渲染缓存自愈，不是继续改 `OcclusionManager` 算法，也不是直接清 `Town`。

### 本轮已完成
1. 先把 `Town / House 4_0` 的现场证据和 prefab/scene YAML 对上：
   - `House 4.prefab` 根节点本身已自带 1 个 `OcclusionTransparency`
   - `Town.unity` 的 `House 4_0` 场景实例又额外叠了 1 个 scene-level `OcclusionTransparency`
   - 同类历史残留不只 `House 4_0`，`House 1/2/3` 也有；所以“为什么很多物体都是两个这个组件”这个反馈是事实成立
2. 同时补跑唯一剩余的建筑遮挡票：
   - `OcclusionSystemTests.PlayerBehindBuilding_WithPixelSamplingHole_StillOccludesByBoundsFallback`
   - 初始失败不是因为 bounds fallback 公式错，而是 `occluderLayer` 读成空字符串，前置 `sameSortingLayerOnly` 过滤直接把建筑跳过
3. 根因收敛后，正式修口只做了一刀：
   - `OcclusionTransparency` 新增 `EnsureRendererCache(...)`
   - 把 `Awake` 时一次性缓存，改成 `Awake + OnEnable + Update + GetSortingLayerName/GetSortingOrder/GetBounds/ContainsPointPrecise/CalculateOcclusionRatioPrecise` 都会按需自愈
   - 这样即便组件是在测试构造链、重绑链、或尚未完整跑过初始化的时机被访问，也不会再因为 `mainRenderer == null` 把 layer 读成空

### fresh 验证
1. `EditMode`
   - `OcclusionSystemTests.PlayerBehindBuilding_WithPixelSamplingHole_StillOccludesByBoundsFallback`：`passed`
   - 遮挡最小回归 5 张票：`5/5 passed`
     - `PlayerInFrontOfTree_EvenIfSpriteOverlap_DoesNotTriggerOcclusion`
     - `PlayerBehindTree_WithSameSpriteOverlap_TriggersOcclusion`
     - `PlayerBehindTree_WhenRootPivotIsAboveFoot_StillTriggersOcclusion`
     - `PlayerBehindTree_WhenSortingOrderAlreadySaysBehind_DoesNotFallBackToBadFootBounds`
     - `PlayerBehindBuilding_WithPixelSamplingHole_StillOccludesByBoundsFallback`
2. 代码层：
   - 当前本刀 owned 范围没有新的 owned compile error
3. 当前 external blocker：
   - `Assets/YYY_Tests/Editor/PackagePanelLayoutGuardsTests.cs(381,52)` 仍有外部 `IEqualityComparer<>` 编译红
   - 这不是本刀引入，也不是遮挡线文件

### 关键判断
1. `House 4_0` 当前最像的单点坏相，不是“它的 prefab 配置和别人不一样”，而是它把现有遮挡链里“组件被访问时渲染缓存尚未补齐”的老缝暴露到了最后。
2. `Town` 里很多对象出现双 `OcclusionTransparency`，更像历史 batch 加组件残留；这轮没有证据证明“只清这些场景残留”就能单独修掉 `House 4_0`。
3. 当前最小安全路径已经收窄为：
   - 先补组件自愈缓存，保住 runtime/测试链一致
   - 不在这刀里顺手改 `Town.unity`

### 当前恢复点
1. 如果下一轮继续遮挡线，优先做用户 live 复测：
   - `Town / House 4_0` 是否恢复
2. 如果用户后续仍要求清“双组件”历史残留，建议单开 scene cleanup 刀：
   - 只删 `Town.unity` 中 scene-level 重复 `OcclusionTransparency`
   - 保留 prefab 自带那一份
   - 但这件事不应和当前打包前主闭环混成同一刀

## 2026-04-18 12:57 只读审查｜作物/农地/工作台保存恢复链静态结论

### 当前主线目标
- 仍是打包前最小安全闭环；这轮子任务只做只读代码审查，确认“加载存档后作物没有保留 / 农地与作物恢复是否闭环 / 工作台进度到底保存到哪”。

### 本轮子任务
- 只读检查：
  - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
  - `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
  - `Assets/YYY_Scripts/Data/Core/PersistentObjectRegistry.cs`
  - `Assets/YYY_Scripts/Data/Core/DynamicObjectFactory.cs`
  - `Assets/YYY_Scripts/Farm/FarmTileManager.cs`
  - `Assets/YYY_Scripts/Farm/FarmTileData.cs`
  - `Assets/YYY_Scripts/Farm/CropController.cs`
  - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
  - `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
  - `Assets/YYY_Scripts/Service/Crafting/CraftingService.cs`
  - 合同测试：`SaveManagerDay1RestoreContractTests.cs`、`WorldStateContinuityContractTests.cs`、`StoryProgressPersistenceServiceTests.cs`、`WorkbenchInventoryRefreshContractTests.cs`、`FarmTileSaveRoundTripTests.cs`、`CropSystemTests.cs`

### 已完成事项
1. 已确认当前格式主链不是“农地存了但作物完全没链路”：
   - `SaveManager.CollectFullSaveData()` 把当前 scene 的持久对象写进 `worldObjects`
   - `PersistentObjectRegistry.CollectAllSaveData()` / `RestoreAllFromSaveData()` 负责保存与恢复
   - `FarmTileManager` 作为 `FarmTileManager` world object 单独保存耕地/浇水/空置寿命
   - `CropController` 作为 `Crop` world object 单独保存 seedId / 生长 / 缺水 / 成熟后天数 / layer / cell
   - `DynamicObjectFactory` 支持 `Crop` 动态重建
2. 已确认跨场景 continuity 也不是空白：
   - `PersistentPlayerSceneBridge` 已把 `FarmTileManager` 与 `CropController` 纳入 snapshot capture / binding
   - scene 回来后会先对 `CropController` 做 `ApplyOffSceneElapsedDays(...)`，再对 `FarmTileManager` 做同名 catch-up
3. 已确认真正的静态断层在 legacy 兼容：
   - `GameSaveData.version` 仍固定为 `1`
   - `GameSaveData.farmTiles` 仍保留在 DTO
   - `FarmTileSaveData` 仍保留旧 crop 字段，但当前主链没有 migration 把这些旧字段转换成新的 `Crop` world object
   - `FarmTileManager.CreateTileFromSaveData()` 当前只恢复耕地自身字段，不消费旧 crop 字段
4. 已确认工作台“长期态”和“runtime 队列态”是分开的：
   - `StoryProgressPersistenceService` 只保存 Day1 长期剧情与进度，如 `completedDialogueSequenceIds`、`woodCollectedProgress`、`craftedCount`、`workbenchHintConsumed`
   - 同时会在 `ApplySpringDay1Progress()` 中把 `_workbenchCraftingActive / _workbenchCraftProgress / _workbenchCraftQueueTotal / _workbenchCraftQueueCompleted / _workbenchCraftRecipeName` 全部重置
   - `SpringDay1WorkbenchCraftingOverlay` 自己维护 `_queueEntries`、`_activeQueueEntry`、`readyCount`
   - Overlay 不是 `IPersistentObject`，也没有正式 `Save/Load`

### 关键判断
1. 如果问题表述是“当前新档读档后作物一定丢”，这条静态判断不成立；当前新格式主链已经有农地 + 作物分离保存恢复链。
2. 如果问题表述是“旧档里作物为什么会丢”，这条高概率成立；旧 `FarmTileSaveData.cropId/daysGrown/daysWithoutWater` 到新 `Crop` world object 的迁移缺失。
3. 工作台当前不是“什么都不存”：
   - 长期剧情推进相关进度有存
   - 制作中进度是有意不存，并通过 `CanSaveNow()` 阻止保存
   - 已完成未领取的产物 / queue runtime state 目前没有正式落盘路径，且静态上看还没被 blocker 全覆盖

### 验证结果
- 本轮性质：只读审查
- 结论等级：静态代码链 + 合同测试交叉成立
- 未做：Unity live save/load、PlayMode、真实旧档样本迁移验证

### 修复后恢复点
- 如果下一轮允许施工，最小安全入口优先级建议为：
  1. 先补 legacy 读档归一化：把旧 `FarmTileSaveData` 的 crop 信息迁成新 `Crop` world object
  2. 同时补一条 legacy 农地旧档迁移合同测试
  3. 工作台先扩大 save blocker，阻止 ready outputs / floating queue state 静默丢失
  4. 暂不优先做完整 overlay 队列持久化

### thread-state
- 本轮性质：只读分析
- 未跑：`Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：保持 `PARKED`
## 2026-04-18 真实施工｜Town 首屏遮挡补刷 + House 4_0 场景重复组件止血 + restart Town suppress + 0.0.4 workbench 兜底

### 当前主线 / 子任务 / 恢复点
- 主线仍是 `0417` 打包前最小安全闭环。
- 本轮子任务是把用户最新 4 个坏相收成一刀：
  1. 第一次启动直进 `Town` 不遮挡
  2. `House 4_0` 闪一下后失效
  3. `重新开始` 后 `Town` 世界状态不 fresh
  4. `0.0.4` 工作台阶段卡住
- 恢复点更新：
  - 当前这 4 条线都已经有真实代码补口和合同测试，不再停在只读判断。

### 本轮已完成
1. [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
   - `StartupFallbackRebindRoutine()` 末尾新增一轮 `WaitForEndOfFrame -> RefreshSceneRuntimeBindingsInternal(activeScene)`
   - `RefreshSceneRuntimeBindingsInternal(...)` 现会补重绑当前场景的：
     - inventory / hotbar
     - AutoPickup
     - core UI
     - scene input
     - placement
     - occlusion managers
   - 目的：解决首屏 `Town` 里 runtime player / scene occluder / UI 壳晚一拍就位，导致“第一次不遮挡，切一次场才好”的坏相
2. [OcclusionManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs)
   - 新增 `RefreshRuntimePlayerBinding(...)`
   - 新增 `RefreshRegisteredOccludersFromScene()`
   - 场景重绑后主动重新抓 runtime player 与当前 scene 的 `OcclusionTransparency`
3. [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity)
   - 只删 `House 4_0` scene-level 额外 `OcclusionTransparency`
   - 保留 prefab authored 那一份，不做整场景双组件大清扫
4. [SaveManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs)
   - `NativeFreshRestartRoutine()` 在载入 `Town` 前补：
     - `PersistentPlayerSceneBridge.SuppressSceneWorldRestoreForScene(NativeFreshSceneName)`
   - 失败分支补：
     - `CancelSuppressedSceneWorldRestore(NativeFreshSceneName)`
   - 目的：防止 `重新开始` 时 `Town` 先吃旧 continuity snapshot，导致世界状态看起来没 reset 干净
5. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
   - `UpdateWorkbenchEscortSnapshot()` 现取玩家到 `chief / workbench` 的最小距离
   - `TryHandleWorkbenchEscort()` 新增 `playerNearWorkbench && !HasWorkbenchBriefingCompleted()` 的 briefing 兜底
   - 明确没有放开 `001` 直聊；仍保持“工作台 briefing 才是 0.0.4 正式入口”

### 验证结果
1. `validate_script`
   - `PersistentPlayerSceneBridge.cs`：`assessment=unity_validation_pending`
   - `OcclusionManager.cs`：`assessment=unity_validation_pending`
   - `SaveManager.cs`：`assessment=unity_validation_pending`
   - `SpringDay1Director.cs`：`assessment=unity_validation_pending`
   - 四者共同状态：
     - `owned_errors=0`
     - `external_errors=0`
   - 卡点是 Unity `stale_status`，不是这轮新红
2. fresh console
   - `errors=0 warnings=0`
3. `EditMode` 目标 9 票
   - `9/9 passed`
   - 其中关键新增票：
     - `WorldStateContinuityContractTests.StartupFallback_ShouldRunOneMoreRuntimeRefresh_ForFirstTownBootstrap`
     - `WorldStateContinuityContractTests.NativeFreshRestart_ShouldSuppressTargetSceneWorldRestore_BeforeLoadingTown`
     - `SaveManagerDay1RestoreContractTests.NativeFreshRestart_ShouldAlsoSuppressTownRestore_BeforeRestartLoad`
     - `SaveManagerDay1RestoreContractTests.SpringDay1WorkbenchEscort_ShouldFallbackToWorkbenchArea_WhenChiefDistanceLags`

### 当前判断
1. 这轮不该再说“问题只是猜测”；四个坏相都已经有代码层止血与合同护栏。
2. 当前最大未闭环项只剩 live：
   - 首次启动直进 `Town`
   - `Town / House 4_0`
   - `重新开始 -> Town`
   - `重新开始 -> 推到 0.0.4 -> 走到工作台`
3. 这轮刻意没有继续顺手做：
   - Town 全量 scene cleanup
   - Save UI / 默认槽
   - Day1 staging
   - packaged smoke

## 2026-04-20 只读梳理｜Day1 剧情进行中判定节点与正式保存清单

### 用户目标
- 不改 `Day1` 代码，只把“当前剧情是否正在进行、哪些节点会拦存/读档、正式存档到底该保存什么”查成一份可以直接照着读参数判断的清单。

### 当前权威判断链
1. `StoryProgressPersistenceService.CanSaveNow()` / `CanLoadNow()` 是顶层入口。
2. 先判：
   - `DialogueManager.IsDialogueActive`
   - `PlayerNpcChatSessionService.HasActiveConversation`
3. 再判：
   - `SpringDay1Director.TryGetStorySaveLoadBlockReason(...)`
4. 只有 `save` 额外再判工作台临时态：
   - `_workbenchCraftingActive`
   - `SpringDay1WorkbenchCraftingOverlay.HasReadyWorkbenchOutputs`
   - `SpringDay1WorkbenchCraftingOverlay.HasWorkbenchFloatingState`

### 当前 Day1 分阶段 blocker 结论
1. `CrashAndMeet`
   - 当前相位内直接拦，不给空窗。
2. `EnterVillage`
   - 只有当下面三件事都不成立时才放行：
     - `IsTownHouseLeadPending()`
     - `IsFirstFollowupPending()`
     - `!HasCompletedDialogueSequence(HouseArrivalSequenceId)`
3. `HealingAndHP`
   - 只有当下面两件事都不成立时才放行：
     - `IsHealingBridgePendingForValidation()`
     - `!HasCompletedDialogueSequence(HealingSequenceId)`
4. `WorkbenchFlashback`
   - 只有当下面三件事都不成立时才放行：
     - `!HasWorkbenchBriefingCompleted()`
     - `!_workbenchOpened`
     - `!HasCompletedDialogueSequence(WorkbenchSequenceId)`
5. `FarmingTutorial`
   - 白天教学过程本身不是剧情 blocker。
   - 真正拦的是“去和村长收口”这一拍：
     - `IsAwaitingPostTutorialChiefWrap()`
     - `_postTutorialWrapSequenceQueued`
     - `IsDialogueSequenceCurrentlyActive(PostTutorialWrapSequenceId)`
6. `DinnerConflict`
   - 只有当下面三件事都不成立时才放行：
     - `IsReturnToTownEscortPending()`
     - `IsDinnerDialoguePendingStart()`
     - `!HasCompletedDialogueSequence(DinnerSequenceId)`
7. `ReturnAndReminder`
   - 只有当下面两件事都不成立时才放行：
     - `IsReminderDialoguePendingStart()`
     - `!HasCompletedDialogueSequence(ReminderSequenceId)`
8. `FreeTime`
   - 只在 `IsFreeTimeIntroPending()` 为真时拦。
   - 夜间引导过后，这一相位本身允许存/读档。
9. `DayEnd`
   - 当前 `TryGetStorySaveLoadBlockReason()` 没有 `DayEnd` blocker 分支。
   - 也就是说，现代码里 `DayEnd` 不是这条剧情 blocker 的正式拦截点。

### 直接可读的判断依据
1. `StoryManager.CurrentPhase`
2. `StoryManager.IsLanguageDecoded`
3. `DialogueManager.IsDialogueActive`
4. `DialogueManager.HasCompletedSequence(sequenceId)` / 已完成对白 ID 集
5. `PlayerNpcChatSessionService.HasActiveConversation`
6. `SpringDay1Director` 公开可读节点：
   - `IsFirstFollowupPending()`
   - `IsTownHouseLeadPending()`
   - `IsHealingBridgePendingForValidation()`
   - `IsDinnerDialoguePendingStart()`
   - `IsReminderDialoguePendingStart()`
7. 需要从现成私有字段读取的节点：
   - `_workbenchOpened`
   - `_workbenchCraftingActive`
   - `_postTutorialWrapSequenceQueued`
   - `_freeTimeEntered`
   - `_freeTimeIntroCompleted`
   - `_dayEnded`
8. 需要从对白完成集读取的关键序列：
   - `spring-day1-first`
   - `spring-day1-first-followup`
   - `spring-day1-village-gate`
   - `spring-day1-house-arrival`
   - `spring-day1-healing`
   - `spring-day1-workbench-briefing`
   - `spring-day1-workbench`
   - `spring-day1-post-tutorial-wrap`
   - `spring-day1-dinner`
   - `spring-day1-reminder`
   - `spring-day1-free-time-opening`

### 正式存档应保存的内容
1. 时间：
   - `day / season / year / hour / minute`
2. 玩家：
   - 场景
   - 坐标
   - 当前层
   - `selectedHotbarSlot`
   - `selectedInventoryIndex`
3. 剧情长期态：
   - `storyPhase`
   - `isLanguageDecoded`
   - `completedDialogueSequenceIds`
   - `npcRelationships`
   - `workbenchHintConsumed`
4. Day1 运行进度：
   - 农田教学 5 项完成态
   - `woodCollectedProgress`
   - `craftedCount`
   - `freeTimeEntered`
   - `freeTimeIntroCompleted`
   - `dayEnded`
   - `staminaRevealed`
   - `residentRuntimeSnapshots`
5. 角色状态：
   - `health`
   - `energy`
6. 世界状态：
   - `worldObjects`
   - `offSceneWorldSnapshots`
   - 箱子内容/锁态/归属/血量
   - 掉落物完整运行时物品
   - 农地浇水态
   - 作物生长/缺水/枯萎态
   - 树/石头等可采集物
7. 其他当前已正式入档运行态：
   - `cloudShadowScenes`

### 不该直接保存的内容
1. 正播到一半的对白壳
2. NPC 闲聊播到一半的临时会话
3. 弹窗/面板当前开合壳
4. 纯 UI 高亮
5. 工作台中途制作队列与待领取结果
   - 当前不是入盘，而是 `CanSaveNow()` 直接阻止在这类状态下保存

## 2026-04-20 追加迭代｜Day1 最小存读档时间窗 + 新建普通槽 blocker 提示

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是存档系统收口。
- 本轮子任务被用户明确收窄为两个最小点：
  1. Day1 不再“一路被挡到第二天”，按最小时间窗放开存/读档。
  2. 新建普通存档在当前不能保存时，必须给明确提示，不能像按钮失灵。
- 恢复点：如果后续继续围绕 Day1 存读档做收尾，应以本轮这套“时间窗只负责放行，阶段文案仍由原 blocker 负责”作为基线。

### 本轮实际落地
1. `SpringDay1Director.TryGetStorySaveLoadBlockReason(...)` 入口前新增最小放行窗：
   - 只在 `Year1 / Spring / Day1` 生效
   - `0.0.6` 已正式打开时，`16:01 ~ 17:59` 放行
   - `19:31` 及以后统一放行 Day1 阶段 blocker
2. 这刀没有继续重写各阶段提示文案：
   - 时间窗未命中时，仍返回原本按阶段说人话的 blocker reason
3. `PackageSaveSettingsPanel` 当前采用的“新建存档”行为已对齐本轮目标，不再表现成按钮失灵。
4. 点击“新建存档”时，当前代码会先显式走：
   - `CanExecutePlayerSaveAction(out blockerReason)`
   - 当前不能存时直接 toast 原因
   - 真正创建失败时 toast `新建存档失败，请稍后重试。`
5. 文本护栏已补：
   - `SaveManagerDay1RestoreContractTests.cs`
   - `SaveManagerDefaultSlotContractTests.cs`

### 验证
1. `validate_script`
   - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
   - `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - `Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
   - `Assets/YYY_Tests/Editor/SaveManagerDefaultSlotContractTests.cs`
   结果均为：
   - `owned_errors=0`
   - `external_errors=0`
   - `assessment=unity_validation_pending`
2. 当前未拿到 compile-first 绿票的原因不是这刀 own red，而是 Unity 持续返回 `stale_status`。
3. `git diff --check --` 针对本轮 4 个目标文件：通过。

### 风险与边界
1. `SpringDay1Director.cs` 当前文件里仍有他线既存脏改，我这轮没有触碰、也没有回退。
2. 这轮只收了：
   - Day1 最小放行窗
   - 新建普通槽 blocker 提示
3. 这轮没有继续处理：
   - 其他普通槽“覆盖”按钮的 blocker 透传
   - 更大范围的 Day1 导演语义重构

### thread-state
- `Begin-Slice`：已执行
- `Park-Slice`：已执行
- 当前 live 状态：`PARKED`

## 2026-04-20 只读审查｜工作台跨场景 continuity 与正式存档合同缺口

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是 `存档系统` 收口。
- 本轮子任务被用户明确收窄为：只读彻查“工作台有内容时切场回来会丢、存档加载也会丢”到底为什么，并给出最安全的彻底修法。
- 本轮不改代码、不跑 `Begin-Slice`；目标是把根因层级和正确合同先钉死，避免再拿 UI patch 假装 persistence 已闭环。

### 本轮新钉实的事实
1. 工作台队列真源不在 `CraftingService`，而在 `SpringDay1WorkbenchCraftingOverlay` 自己的 runtime 内存：
   - `_queueEntries`
   - `_activeQueueEntry`
   - `readyCount`
   - `currentUnitProgress`
2. `CraftingService` 只负责材料预扣、完成通知和 runtime 背包上下文，不负责持久化，也没有 `IPersistentObject` 合同。
3. `StoryProgressPersistenceService` 当前只保存 Day1 长期态、体力/精力、关系、`workbenchHintConsumed` 等；没有保存工作台队列 / 待领取产物 / 当前进度。
4. `ApplySpringDay1Progress()` 在读档恢复时还会主动把导演里的工作台 runtime 镜像字段清零：
   - `_workbenchCraftingActive`
   - `_workbenchCraftProgress`
   - `_workbenchCraftQueueTotal`
   - `_workbenchCraftQueueCompleted`
   - `_workbenchCraftRecipeName`
5. `SaveDataDTOs` 当前没有任何正式 `WorkbenchSaveData / CraftQueueEntrySaveData`。
6. `PersistentPlayerSceneBridge` 切场只会给 `CraftingService` 重新绑背包事实源，不会 capture / restore 工作台队列。
7. `PersistentPlayerSceneBridge` 的 off-scene scene snapshot 目前只抓：
   - `FarmTileManager`
   - `CropController`
   - `ChestController`
   - `WorldItemPickup`
   - `TreeController`
   - `StoneController`
   不含工作台状态。

### 这轮最关键的新发现
1. 当前 save blocker 对工作台不是全局安全的，而是有“离场盲区”：
   - `StoryProgressPersistenceService.CanSaveNow()` 只看
     - `overlay.HasReadyWorkbenchOutputs`
     - `overlay.HasWorkbenchFloatingState`
   - 但这两个属性都要求“工作台状态绑定在当前 active scene”
   - 也就是说：你在 `Primary` 留下一批工作台队列或待领取产物，切到 `Home/Town` 后，这两个 blocker 会失效
   - 此时是可以存档的，但存档里并没有这批工作台状态，结果就是静默丢失
2. `SpringDay1WorkbenchCraftingOverlay.OnDisable()` 是真实清空点：
   - 会执行 `StopCraftRoutine()`
   - 然后 `CleanupTransientState(resetSession: true)`
   - 这会把 `_queueEntries`、`_activeQueueEntry`、ready outputs 一起清掉
3. Overlay 的 runtime 父节点解析也不稳：
   - `SpringDay1UiLayerUtility.ResolveUiParent()` 现在直接 `GameObject.Find("UI")`
   - 但项目里其实已经有 `PersistentPlayerSceneBridge.GetPreferredRuntimeUiRoot()`
   - 这代表工作台 overlay 有机会挂到“当前 scene 的临时 UI”而不是 persistent UI root
   - 一旦那个临时 UI 在切场或 duplicate root 清理时被销毁/失活，就会触发上面的 `OnDisable -> 清空队列`

### 当前判断
1. 这不是“少存了一个字段”。
2. 当前工作台问题至少有 4 层：
   - 真值宿主放错：队列真源放在 overlay UI 内存里
   - 切场 continuity 缺合同：bridge 只重绑材料事实源，不维护工作台状态
   - 正式存档缺合同：save DTO 没有工作台 runtime 队列
   - blocker 还有 off-scene 盲区：离开 scene 后能存，但存进去会丢
3. 所以它现在不是“偶发坏”，而是“当前设计本来就没闭环”。

### 最安全的彻底修法
1. 不再把工作台队列真源放在 `SpringDay1WorkbenchCraftingOverlay`。
2. 新建一个正式 runtime 宿主，例如：
   - `WorkbenchRuntimeStateService`
   - 或同级职责的持久化宿主
3. 这个宿主至少要持有：
   - 稳定 `stationKey`
   - `sceneKey`
   - 每个队列项的 `recipeId / resultItemId / resultAmountPerCraft / totalCount / readyCount / currentUnitProgress`
   - 当前 active entry 标识
4. `SpringDay1WorkbenchCraftingOverlay` 退回成“展示层 + 交互层”：
   - 负责显示
   - 负责收输入
   - 不再自己拥有工作台真值
5. 切场 continuity 要正式接到这个宿主上：
   - 离场不清空
   - 回场只重绑 station / inventory / anchor
   - 不再靠 overlay 自己“活着就算 continuity”
6. 正式存档也要写/读这个宿主：
   - 最稳是给它正式 DTO 或 `IPersistentObject` 合同
   - 让 save/load 真正能恢复 queue / ready outputs / partial progress
7. `CanSaveNow()` 要跟着改口径：
   - 如果正式入盘了，就不该再拦“ready outputs / floating queue state”
   - 如果这一轮只做到一半，还没正式入盘，那至少要把 off-scene raw state 也纳入 blocker，不能再有离场盲区
8. `ResolveUiParent()` 要改吃 `PersistentPlayerSceneBridge.GetPreferredRuntimeUiRoot()`，不能继续拿 `GameObject.Find("UI")` 碰运气。

### 明确不该走的假修法
1. 只给 `StoryProgressPersistenceService` 再补几个私有字段读写，但不下沉真值宿主。
   - 这样 `OnDisable` 清空和 scene UI 父节点问题还会继续炸
2. 只继续扩大 blocker。
   - 这能止血一部分存档丢失，但修不好跨场景 continuity
3. 继续把状态放在 overlay 里，再到处补“切场前抄一下 / 回场后塞回去”。
   - 这类 patch 未来极容易再次被 UI 生命周期击穿

### 下一步恢复点
- 如果下一轮进入真实施工，正确顺序应是：
  1. 先把工作台真值下沉到正式 runtime 宿主
  2. 再接 save/load DTO
  3. 再把 blocker 从“止血”切到“正式支持”
  4. 最后补 3 类验证：
     - 跨场景 continuity
     - 存档/读档恢复
     - overlay/runtime UI 父节点稳定性

## 2026-04-20 18:01 真实施工｜工作台 runtime state 正式入盘 + 切场 continuity + save/load 接入

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是 `0417` 下的“打包前最小安全闭环”。
- 本轮子任务是用户批准后的真实施工：把工作台 queue / ready outputs / partial progress 从“只会 blocker 的临时态”推进到“正式存档可保存、切场不丢”的第一刀。
- 这轮做完后，工作台线的恢复点已经从“继续研究根因”切到“剩 live 回归与 packaged 终验”。

### 本轮实际落地
1. `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
   - 新增正式 DTO：
     - `WorkbenchRuntimeSaveData`
     - `WorkbenchQueueEntrySaveData`
2. `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
   - 新增静态 runtime 真值仓：
     - `WorkbenchRuntimeStateByStation`
   - 新增 API：
     - `StoreWorkbenchRuntimeState`
     - `RemoveWorkbenchRuntimeState`
     - `ClearWorkbenchRuntimeStates`
     - `TryGetWorkbenchRuntimeState`
     - `GetWorkbenchRuntimeStatesSnapshot`
   - `CaptureSnapshot()` 现在会先 `SpringDay1WorkbenchCraftingOverlay.FlushRuntimeStateToPersistence()`，再把 `workbenchStates` 写入正式 snapshot
   - `ApplySnapshot()` 现在会 `ReplaceWorkbenchRuntimeStates(...)`，再通知 overlay `NotifyPersistentStateReplaced()`
   - `CanSaveNow()` 已移除旧的工作台 blocker 扩口，不再因为 ready/floating state 一刀切拒绝保存
3. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
   - 新增稳定 station binding：
     - `_boundWorkbenchStationKey`
     - `BuildWorkbenchStationKey(...)`
   - `OnDisable()` 不再走 `StopCraftRoutine() + CleanupTransientState(resetSession: true)`；改成：
     - `SuspendCraftRoutineForPersistence()`
     - `FlushCurrentRuntimeStateToPersistence()`
     - `DetachScenePresentation()`
     - `CleanupTransientState(resetSession: false)`
   - `Open(...)` 现在会先 flush 旧站点，再按 `stationKey` 从正式持久层恢复 runtime state
   - 新增/补齐：
     - `FlushRuntimeStateToPersistence()`
     - `NotifyPersistentStateReplaced()`
     - `ResolveRecipeById(...)`
     - `LoadRuntimeStateFromPersistence(...)`
     - `ApplyPersistedWorkbenchRuntimeState(...)`
     - `ResumeCraftRoutineIfNeeded()`
   - queue mutation 点已补回写：
     - `CraftRoutine(...)`
     - `ResumeNextPendingCraftQueueIfAny()`
     - `TryPickupSelectedOutputs()`
     - `CancelActiveCraftQueue()`
     - `CancelQueuedRecipeEntry()`
4. `Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs`
   - `ResolveUiParent()` 已优先吃 `PersistentPlayerSceneBridge.GetPreferredRuntimeUiRoot()`，不再先挂 scene-local `UI`
5. 合同测试已同步到新语义：
   - `Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
     - 工作台制作中不再默认 blocker
     - 新增 `SaveLoad_RoundTripRestoresWorkbenchRuntimeStates()`
   - `Assets/YYY_Tests/Editor/WorkbenchInventoryRefreshContractTests.cs`
     - 新增“persist runtime state + persistent UI root”文本护栏

### 这轮的关键决策
1. 没有把工作台状态塞进 `PersistentPlayerSceneBridge` 的 off-scene snapshot。
   - 这轮选择的正式宿主是 `StoryProgressPersistenceService`
   - 原因是它本身就是 DDOL 的长期持久宿主，刀口更小、更安全
2. `SpringDay1WorkbenchCraftingOverlay` 这轮仍保留展示/交互职责，但不再独占真值。
3. 这轮优先收“能保存、能恢复、切场不清空”，没有顺手扩成通用 crafting persistence 系统。

### 当前验证
1. `validate_script`
   - `SpringDay1WorkbenchCraftingOverlay.cs`：无 own red，CLI 结果 `unity_validation_pending`
   - `SpringDay1UiLayerUtility.cs`：无 own red，CLI 结果 `unity_validation_pending`
   - `StoryProgressPersistenceServiceTests.cs`：无 own red，CLI 结果 `unity_validation_pending`
   - `WorkbenchInventoryRefreshContractTests.cs`：无 own red，CLI 结果 `unity_validation_pending`
2. `git diff --check --`（本轮 4 个目标文件）：通过
3. `errors --count 20 --output-limit 10`
   - `error_count = 0`
   - `warning_count = 0`
   - 控制台仍带 1 条非 blocking `Assert` 噪音，但不算 compile red
4. `compile`（显式 4 路径）：
   - 被 `CodexCodeGuard timeout: dotnet 20s` 卡成 `blocked`
   - 当前不能把它包装成“fresh compile 已闭环”

### 还没做完的部分
1. 还没做 live 回归：
   - `Primary` 工作台排队 -> 切到 `Home/Town` -> 回来队列/ready outputs/progress 是否保持
   - 工作台有内容时正式保存 -> 读档恢复是否正确
2. 还没做 packaged smoke。
3. 还没补 `Ready-To-Sync`；这轮只做到本地施工 + 代码层验证，还没有进入 sync 收口。

### 下一步恢复点
- 下一轮如果继续这条线，只做 3 件事：
  1. live 跑工作台切场 continuity
  2. live 跑 save/load round-trip
  3. packaged 再做最短 smoke

## 2026-04-20 18:20 真实施工｜读取存档改回“非剧情即可读”，不再共用保存时间窗

### 当前主线 / 子任务 / 恢复点
- 当前主线仍是打包前最小安全闭环。
- 本轮子任务是用户对存档语义的即时纠偏：`load` 不能继续吃 `save` 的 Day1 时间窗；应改为“只要不在剧情接管中就能读取，并正确刷新全局状态”。

### 本轮实际落地
1. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
   - 拆出：
     - `TryGetStorySaveBlockReason(...)`
     - `TryGetStoryLoadBlockReason(...)`
     - `TryGetStorySaveLoadBlockReasonInternal(bool allowDay1SaveWindowBypass, ...)`
   - `save` 继续允许 Day1 最小时间窗放行
   - `load` 不再提前走 `IsDay1SaveLoadWindowOpen(...)`
2. `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
   - `CanSaveNow()` 改走 `director.TryGetStorySaveBlockReason(...)`
   - `CanLoadNow()` 改走 `director.TryGetStoryLoadBlockReason(...)`
3. `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - `RefreshView()` 现显式区分 `canSave` 与 `canLoad`
   - 默认槽 / 普通槽的读取动作都会先 `CanExecutePlayerLoadAction(...)`
   - 命中剧情 blocker 时直接 toast 真实原因，而不是继续统一报“读取失败”
4. 合同测试
   - `Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
     - 钉死 `load` 不再吃 Day1 保存时间窗
     - 钉死读档后必须刷新全局 runtime/UI 链
   - `Assets/YYY_Tests/Editor/SaveManagerDefaultSlotContractTests.cs`
     - 钉死设置页必须区分 `canSave / canLoad`
     - 钉死加载动作要先检查 load blocker

### 当前验证
1. `validate_script`
   - `SpringDay1Director.cs`：无 own red
   - `StoryProgressPersistenceService.cs`：无 own red
   - `PackageSaveSettingsPanel.cs`：无 own red
   - 两份合同测试：无 own red
2. `errors --count 20 --output-limit 10`
   - `0 errors / 0 warnings`
3. `git diff --check --`
   - 通过
4. `compile-first`
   - 仍受 `CodexCodeGuard timeout + stale_status` 干扰，没拿到 fresh compile 闭环

### thread-state
- 本轮已执行 `Park-Slice`
- 当前 live 状态：`PARKED`
- 当前 blocker：
  1. `待用户复测：非剧情状态下默认存档/普通存档均可读取`
  2. `待用户复测：剧情接管中读取仍会给真实 blocker 提示`
  3. `CLI compile-first 仍受 stale_status 影响，未拿到 fresh compile 闭环`

## 2026-04-20 追加索引｜树苗放置/预览偏移只读补查
- 用户反馈已经钉死：`树苗放置和预览位置不对了`，但 `种子` 和 `箱子` 仍然正确。
- 本轮只读归因已经收敛到一条最小链：
  1. 前面修箱子时，`PlacementGridCalculator` 被整体切到“真实 Collider center / authored visual baseline”合同
  2. 这条新合同对箱子成立，因为 `ChestController` 已经改成“作者摆好的 closed 视觉面是真值，运行时视觉 child 再相对它切换”
  3. 树苗仍走 `TreeController` 的运行时 `AlignSpriteBottom()`，而且 `Tree` 的 SpriteRenderer 和 Collider 还在同一个 GameObject 上，所以它吃的是旧的“底部对齐会带着 collider 一起动”的语义
  4. 种子没有吃同一条预制体放置合同，而是走自己的 crop preview 分支，因此没有被这轮全局锚点改动误伤
- 最小安全修法判断：
  - 不回退箱子修复
  - 不碰种子链
  - 只给 `SaplingData / TreeController` 单独补回底部对齐合同，让树苗预览和落地继续按 runtime bottom-align 语义走
- 这条结论目前仍是静态审查成立，尚未改代码，也未做 live 复测。

## 2026-04-22 只读审计｜存档、跨场景承接与持久化链现状复盘
- 当前主线没换，仍属于 `4.0.0_三场景持久化与门链收口`；本轮子任务是按用户指定文件做只读审计，回答“正式存档到底做到哪层、`SaveManager/PersistentPlayerSceneBridge/场景切换/玩家状态/背包工具/任务进度/部分世界状态` 的真实承接关系，以及哪些说法适合写进技术策划简历、哪些不能夸大”。

### 本轮实际核对
1. 文档与 memory：
   - `.kiro/specs/存档系统/memory.md`
   - `.kiro/specs/900_开篇/spring-day1-implementation/memory.md`
   - `Docx/大总结/Sunset_持续策划案/03_交互系统.md`
   - `Docx/大总结/Sunset_持续策划案/08_进度总表.md`
   - `.codex/threads/Sunset/存档系统/memory_0.md`
2. 关键代码：
   - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
   - `Assets/YYY_Scripts/Data/Core/DynamicObjectFactory.cs`
   - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
   - `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
   - `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
   - `Assets/YYY_Scripts/Service/Inventory/InventoryService.cs`
   - `Assets/YYY_Scripts/Service/Equipment/EquipmentService.cs`
   - `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
   - `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
   - `Assets/YYY_Scripts/World/WorldItemPickup.cs`
   - `Assets/YYY_Scripts/Farm/FarmTileManager.cs`
   - `Assets/YYY_Scripts/Farm/CropController.cs`
   - `Assets/YYY_Scripts/Service/Crafting/CraftingService.cs`
   - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
3. 合同测试 / 护栏：
   - `Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
   - `Assets/YYY_Tests/Editor/WorkbenchInventoryRefreshContractTests.cs`

### 当前稳定结论
1. 正式落盘链现在已经不是“只有当前 scene 的 `worldObjects`”这一层：
   - `SaveManager` 仍以当前 active scene 的 `worldObjects` 作为主 payload
   - 但 `GameSaveData` 已正式带上 `offSceneWorldSnapshots`
   - `PersistentPlayerSceneBridge` 会把非当前 scene 的 `FarmTileManager / Crop / Chest / Drop / Tree / Stone` 快照导出到该字段，并在读档时导回 bridge 内存
2. 玩家承接已经拆成两层：
   - 正式存档：`PlayerSaveData` 只负责位置、sceneName、`selectedHotbarSlot / selectedInventoryIndex`
   - 真正背包与装备：分别由 `InventoryService` 和 `EquipmentService` 作为 `IPersistentObject` 进入 `worldObjects`
3. 工具态不是独立 DTO：
   - 没有“当前工具对象”单独存盘
   - 真实恢复方式是：先恢复背包与 hotbar 选中，再由 `HotbarSelectionService.RestoreSelectionState()` 触发 `EquipCurrentTool()`
4. 剧情 / 任务长期态已经正式入盘：
   - `StoryProgressPersistenceService` 通过 `StoryProgressState` 写入 `StoryPhase`
   - `completedDialogueSequenceIds`
   - `springDay1` 关键导演长期态
   - `health / energy`
   - `npcRelationships`
   - `workbenchHintConsumed`
   - 以及现在已经正式入盘的 `workbenchStates`
5. 工作台不再只是 blocker 口径：
   - `WorkbenchRuntimeSaveData / WorkbenchQueueEntrySaveData` 已经是正式 DTO
   - `StoryProgressPersistenceService` 负责持久化它
   - overlay 的 `OnDisable()` 也已改成先 suspend + flush，再 detach，不再直接清 session
6. 当前最重要的真实边界不是“都持久化了”：
   - off-scene NPC resident snapshot 仍只在 bridge 运行时缓存，不在正式 save 文件里
   - `SaveManager.ValidateRequiredSavePayloads()` 的强校验只硬检查 `StoryProgressState / PlayerInventory / EquipmentService`
   - `BuildSlotSummary()` 仍从根 `saveData.inventory.slots` 统计背包占用，这条更像旧格式兼容摘要，不是现行主 payload 真相

### 本轮对旧结论的校正
1. `spring-day1` memory 里关于“bridge 只覆盖 `WorldItemPickup / Tree / Stone`、不覆盖箱子/农地/作物”的旧结论已经过时；当前源码已覆盖 `FarmTileManager / Crop / Chest / Drop / Tree / Stone`。
2. `workbench` “只能 blocker、没有正式 DTO”的旧结论也已经过时；当前代码已把工作台队列与进度并入 `StoryProgressState.workbenchStates`。
3. 但“resident 位置已经正式入档”这类话仍然不能成立；这块依旧只是 bridge runtime continuity，不是正式 payload。

### 当前恢复点
- 如果后续继续沿这条线做技术策划草稿本或下一轮只读/施工：
  1. 先沿本轮结论区分“正式落盘链”和“bridge runtime continuity 链”
  2. 优先记住 `SaveManager + StoryProgressPersistenceService + PersistentPlayerSceneBridge + SceneTransitionTrigger2D` 是四个核心锚点
  3. 不要再沿用“箱子/农地/作物没进 off-scene snapshot”与“workbench 仍只靠 blocker”的过时口径
