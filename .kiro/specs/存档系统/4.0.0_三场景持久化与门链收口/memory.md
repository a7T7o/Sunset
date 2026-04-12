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
