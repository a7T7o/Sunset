# Day1-V3 - 线程记忆

## 线程概述
- 线程名称：Day1-V3
- 线程分组：Sunset
- 线程定位：接替旧 `day1` 线做新的独立分析入口，先吃透历史 prompt、memory、交接文档与当前代码真相，不直接继承旧结论，不直接开重构。
- 当前核心目标：先把 `day1` 这条线真实的问题清单、误修成因、代码/测试/文档之间的冲突、以及后续重构风险讲清楚，再决定是否进入下一轮冻结表或真实施工。

## 当前状态
- **完成度**：已完成首轮接班只读审计
- **最后更新**：2026-04-15
- **状态**：只读分析完成，等待用户裁定下一步

## 会话记录

### 会话 1 - 2026-04-15

**用户需求**:
> 你先把这些内容彻底看完，然后再去彻查day1的memory，以及相关代码，你现在属于从0开始，那就必须彻底吃透先，因为我不打算让你直接重构，你先看完后告诉我你认为的问题有哪些，分析重构的风险以及分析为什么day1做了这么多轮都没有解决问题而是一直在产生问题，一直在不同的方式去犯错。

**完成任务**:
1. 以只读模式完成前置核查，明确本轮不进入真实施工，首条 commentary 已对用户报实“本轮只读，暂不跑 `Begin-Slice`”。
2. 深读旧 `day1` 的 prompt 叙事、交接总文档、`004_runtime` memory、`spring-day1` 线程 memory、`spring-day1V2` 线程 memory。
3. 对照当前 live 代码独立核验：
   - `SpringDay1NpcCrowdDirector.Update()` 在 `stand-down` 前仍持续跑 `SyncResidentNightRestSchedule()` 与 `TickResidentReturns()`
   - `ApplyResidentBaseline(...)` 仍保留 `opening release latch -> direct autonomy`
   - `SpringDay1DirectorStagingTests.cs` 里仍有多条测试在保护旧的 `CrowdDirector` owner 合同
   - `SpringDay1Director.cs` 仍直接管理 `003` 和 crowd runtime 协调
4. 形成独立判断：
   - 旧线程后期方向并非完全错误，但它没有真正把 owner 从代码和测试里拔干净
   - 这条线反复失败的根因，是“文档、代码、测试在保护三套不同真相”
5. 在新工作区 `100-重新开始` 落盘首轮只读审计文档与工作区 memory。

**修改文件**:
- `D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/100-重新开始/2026-04-15_Day1-V3_接班诊断与重构风险审计.md` - [新增]：首轮独立诊断
- `D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/100-重新开始/memory.md` - [新增]：工作区记忆
- `D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/Day1-V3/memory_0.md` - [新增]：线程记忆

**解决方案**:
- 先把旧线程“说了什么”和当前代码“实际是什么”分开。
- 再把问题拆成：
  1. owner 冲突
  2. direct autonomy / return-home 合同冲突
  3. 测试仍保护 stopgap
  4. `NPCAutoRoamController` 被坏输入放大
  5. 巨型文件高 churn 造成 review 不可控

**遗留问题**:
- [ ] 下一轮若继续只读，应先补“resident lifecycle 全入口冻结表”。
- [ ] 还需单独拉出 `003` 特殊化残留清单。
- [ ] 还需单独拉出“需废弃/需重写的 owner 级 staging tests”列表。
- [ ] 本轮没有跑 compile / live / profiler，不宣称体验或运行态已过线。

## 当前稳定判断
1. 当前主问题确实是 owner 写错，不是单个参数没调好。
2. 但旧线程没有真正把 owner 从代码和测试里拔干净，因此“分析逐步变对”和“现场继续出新坏相”会长期并存。
3. 当前最关键的硬冲突是：文档说 opening 后应先回 anchor/home，再 roam；代码和测试却仍保留 `release latch -> direct autonomy`。
4. 真正进入重构前，必须先做只读冻结，统一文档/代码/测试三套真相。

## 当前恢复点
1. 如果用户继续要分析，不是再复述旧交接文档，而是先做一张“resident lifecycle 全入口冻结表”。
2. 如果用户要开始真施工，第一刀也不能直接重构执行层，而是先统一 opening handoff 的唯一合同，并清理与之冲突的测试。
3. 当前线程未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`，原因：本轮始终停留在只读分析。

## 2026-04-15｜审计补记：Day1-V3 首轮接班分析已补 `skill-trigger-log`
- 已追加：
  - `STL-20260415-028`
- 当前审计层结论：
  1. 这轮显式触发了：
     - `skills-governor`
     - `sunset-workspace-router`
     - `user-readable-progress-report`
     - `delivery-self-review-gate`
  2. `sunset-startup-guard` 仍为手工等价执行，因为当前会话未显式暴露。
  3. 本轮是只读接班审计，不涉及 `sunset-no-red-handoff` 的真实施工分支。

## 2026-04-15｜真实施工补记：101 首刀已把 opening handoff 合同收成唯一口径

**本轮主线**:
- 用户已批准直接落地；这轮从只读分析转入真实施工，但只收第一阶段最小真刀：
  - opening handoff 不再 `direct autonomy`
  - `003` 不再被导演从 opening 终点直接放回 roam

**本轮实际完成**:
1. 已执行 `Begin-Slice`
   - `thread = Day1-V3`
   - `slice = 101_opening-handoff-owner-unification`
2. 已修改：
   - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - `ApplyResidentBaseline(...)` 去掉 opening release latch 后直接 autonomy 的分支
   - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - `ReleaseOpeningThirdResidentControlIfNeeded(...)` 改成交回 crowd baseline，并在 handoff 后立即 `ForceImmediateSync()`
   - [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - opening handoff 与 `003` handoff 测试改成保护新合同
3. 已完成的最小验证：
   - `manage_script validate SpringDay1NpcCrowdDirector` => `errors=0 warnings=2`
   - `manage_script validate SpringDay1Director` => `errors=0 warnings=3`
   - `manage_script validate SpringDay1DirectorStagingTests` => `errors=0 warnings=0`
   - fresh console => `errors=0 warnings=0`
   - `git diff --check` 覆盖 touched files 通过
   - 定向 EditMode 测试通过：
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldQueueOpeningResidentReturnHomeAfterEnterVillageCrowdReleaseLatches`
     - `SpringDay1DirectorStagingTests.Director_ShouldYieldOpeningThirdResidentToCrowdReleaseContractWhenVillageGateHandsOff`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldKeepOpeningResidentsOnBaselineReleasePathDuringEnterVillage`
4. 额外验证发现：
   - 整个 `SpringDay1DirectorStagingTests` 类仍有既有失败，但失败列表不包含本轮触碰的 3 条测试
   - `compile/no-red` 的 CLI 票仍被 `CodeGuard dotnet timeout` 卡住，不能把这轮包装成完整 Unity no-red
5. 已执行：
   - `Park-Slice`
   - 当前状态 = `PARKED`

**当前判断**:
1. 这轮真正收平的，是“文档说 opening handoff 先回 anchor/home，再 roam；代码/测试却还 direct-autonomy”这条最硬冲突。
2. 这轮没有假装完成完整退权：
   - `CrowdDirector` 仍持有 `return-home` 这段 owner
   - `003` 的更深层特殊化残留还没完全清完
   - `night schedule / 恢复链 / stopgap tests` 仍待后续处理

**当前恢复点**:
1. 如果继续只读，下一轮优先补：
   - resident lifecycle 全入口冻结表
   - `003` 特殊化残留表
   - owner 级 stopgap tests 清单
2. 如果继续真实施工，仍不要扩 dinner / 导航 core / Primary / UI，先沿 owner 清理线推进。

## 2026-04-15｜方向澄清补记：当前路线不是“继续散修”，也不是“现在就一轮梭哈”
- 用户明确追问：
  - 我是不是已经倾向直接一轮彻底干到底
  - 冻结表是不是只是给用户看的
  - 我到底是想重构，还是想继续一步一步修
- 当前线程给出的稳定答案：
  1. 冻结表对我不是展示品，而是后续施工图。
  2. 我不想继续旧 `day1` 那种散修。
  3. 我也不想现在就无边界总重构。
  4. 我真正的方向是：
     - 先冻结
     - 再受控重构
     - 后续按 owner 主线连续往下收
- 原因：
  1. 当前 `CrowdDirector` owner、`003` 特殊化、night schedule、恢复链、owner 级 stopgap tests 还缠在一起。
  2. 现在直接一轮硬梭哈，最危险的是误伤稳定链；继续散修则会继续换坏相。
- 当前恢复点：
  1. 如果用户认可这一路线，下一轮就先补冻结表。
  2. 冻结表之后，才进入真正的受控重构阶段。

## 2026-04-15｜文档收口补记：102 已从“这一刀冻结”升级为“Day1 全量总表 + 导航/NPC 同步入口”
- 用户进一步纠正：
  - 不要只写这一刀的 owner 冻结
  - 要给出 `Day1` 现存所有 open 问题的总览、整体施工方向
  - 还要给导航一份边界同步 contract，并判断 NPC 是否需要单独 prompt
- 我这轮新的稳定落点：
1. 已新增总表：
   - [2026-04-15_Day1-V3_Day1现存问题总览与整体施工总表.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_Day1现存问题总览与整体施工总表.md)
2. 已新增导航同步文件：
   - [2026-04-15_Day1-V3_给导航_语义同步与执行边界contract.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_给导航_语义同步与执行边界contract.md)
3. 已新增 NPC 协作 prompt：
   - [2026-04-15_Day1-V3_给NPC_协作边界与facade落地prompt.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_给NPC_协作边界与facade落地prompt.md)
4. 已补成 3 份可直接转发的引导文本：
   - [给 Day1-V3 自己的续工引导 prompt](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_给Day1-V3_整体推进分阶段续工引导prompt_01.md)
   - [给导航的弱引导 prompt](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_给导航_Day1-V3语义同步与执行边界弱引导prompt_01.md)
   - [给 NPC 的弱引导 prompt](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_给NPC_Day1-V3协作边界与facade弱引导prompt_01.md)
  4. 我当前明确主导判断：
     - `Day1-V3` 继续主导 Day1 语义、owner 退权与集成收口
     - `NPC` 主导 facade 与高危 public API 收口
     - `导航` 主导统一 movement execution contract
  5. 我当前明确主张：
     - 只要 NPC 发生真实位移，原则上都应走统一导航执行合同
     - 只有 staged timeout / load repair / authored 失败兜底才允许显式 snap 例外
- 本轮方式：
  - 文档层施工
  - 未继续改运行时代码
  - 已执行 `Park-Slice`
  - 当前 thread-state=`PARKED`
- 当前恢复点：
  1. 如果用户下一步要继续协调线程，先转发导航/NPC 两份文件。
  2. 如果用户下一步要我继续施工，直接进入 `Package B｜Day1 自己退权`。

## 2026-04-15｜Package B 实装补记：Day1 自己退权第一刀已落

**本轮主线**:
- 用户明确要求按 `102` 自用 prompt 继续推进，不再写 prompt，直接进入 `Package B｜Day1 自己退权`

**本轮实际完成**:
1. 已补 `Begin-Slice` 后的真实施工，并在收尾执行 `Park-Slice`
   - slice=`103_packageB_day1_self_deownership`
   - 当前状态=`PARKED`
   - blocker=`checkpoint-package-b-day1-self-deownership-ready`
2. `SpringDay1NpcCrowdDirector.cs`
   - `Update()` 现在会在 `EnterVillage release latch + 无待放 resident` 时提前 stand-down，不再先 tick resident runtime
   - `ApplyResidentBaseline(...)` 改成：
     - opening/daytime release => `baseline-release`
     - `20:00+` schedule 才继续走 `return-home`
   - shared release path 会同时放掉：
     - `SpringDay1NpcCrowdDirector`
     - `spring-day1-director`
     两个 owner，`003` 因此不再需要专用导演 shim
3. `SpringDay1Director.cs`
   - 删除 `ReleaseOpeningThirdResidentControlIfNeeded(...)`
   - 移除其全部 call-site
   - `003` 不再挂在 story actor night-rest runtime 持有链上
   - Day1 对 `NPCAutoRoamController` 的旧低级调用改到 facade：
     - `AcquireStoryControl`
     - `ReleaseStoryControl`
     - `RequestStageTravel`
     - `RequestReturnToAnchor`
     - `HaltStoryControlMotion`
     - `BindResidentHomeAnchor`
4. `SpringDay1DirectorStagingTests.cs`
   - `opening resident` 测试改成保护 shared baseline release，不再保护旧 `CrowdDirector return-home owner`
   - 新增 `003` 通过 shared resident contract 放手的断言
   - 保留并重跑 `19:30 / 20:00 / daytime yield` 相关边界
5. 为清掉 compile blocker，还额外收了 6 个 Editor 工具的机械 API rename：
   - `SetHomeAnchor -> BindResidentHomeAnchor`
   - `ApplyProfile -> SyncRuntimeProfileFromAsset`
   其中 `Assets/Editor/Town/TownNativeResidentMigrationMenu.cs` 原本就是 pre-existing untracked 文件，这轮只做 compile unblock 改名，没有扩语义

**验证**:
1. `manage_script validate SpringDay1NpcCrowdDirector` => `errors=0 warnings=2`
2. `validate_script SpringDay1Director` / `SpringDay1DirectorStagingTests`
   - `owned_errors=0`
   - `external_errors=0`
   - 但 CLI 因 Unity `stale_status` 只给到 `unity_validation_pending`
3. direct MCP fallback：
   - 清 console 后 fresh console => `errors=0 warnings=0`
   - 定向 EditMode 6/6 passed：
     - `CrowdDirector_ShouldReleaseOpeningResidentToSharedBaselineAfterEnterVillageCrowdReleaseLatches`
     - `CrowdDirector_ShouldReleaseDirectorOwnedThirdResidentThroughSharedResidentContract`
     - `CrowdDirector_ShouldNotRetouchAlreadyAutonomousResidentsDuringDaytimeYield`
     - `CrowdDirector_ShouldNotCancelResidentReturnHomeDuringDaytimeYield`
     - `CrowdDirector_ShouldKeepResidentsRoamingBeforeTwentyDuringFreeTime`
     - `CrowdDirector_ShouldQueueReturnHomeAtTwentyDuringFreeTime`
4. touched files `git diff --check` 通过

**当前判断**:
1. 这轮已经把 `Package B` 第一刀收成真实结构成果，不再只是方向判断
2. 这轮还不能宣称 live 体感最终过线；现在成立的是：
   - owner 关系更干净了
   - tests 对准新结构了
   - compile/console 已无我 own 的红
3. 后续若继续，应优先接：
   - `night schedule / recover chain` 的剩余 owner
   - `NPC facade / 导航统一执行合同` 的后续接力

## 2026-04-15｜Package B 第二刀补记：recover/night-rest/restart-tail 退权
- 用户主线仍是：继续按 `102` 的 owner 清理线推进，不回到 prompt，不扩到无边界总重构。
- 本轮子任务：
  - `SpringDay1NpcCrowdDirector` 的 `night/recover` 退权第二刀。
  - 目标不是改完全部 schedule，而是把最脏的“剧情后 resident 下半生暗门”继续收掉。
- 本轮前置真值：
  1. 已先读 `skills-governor / sunset-workspace-router / sunset-no-red-handoff`，按 Sunset 规则做手工 startup 等价核查。
  2. 本轮先前已 `Begin-Slice`：
     - `ThreadName=Day1-V3`
     - `Slice=104_packageB_night_recover_deownership`
  3. 中途收到 NPC 线程回执，确认：
     - locomotion façade 第一刀已真实落地
     - Day1 后续只应消费 façade，不应再回写 resident scripted 内核
- 本轮实际改动：
  1. `SpringDay1NpcCrowdDirector.cs`
     - `RecoverReleasedTownResidentsWithoutSpawnStates(...)`
       - 从“state 丢了就强行 restart roam”改成“只恢复 `_spawnStates` 视图并补 anchor/profile”
     - `TryRecoverReleasedResidentState(...)`
       - 新增：把 scene resident 重新收回 Day1 视图，但不碰剧情后 locomotion 生命周期
     - `ApplyResidentNightRestState(...)`
       - 改成释放 shared owner 后走 `SnapToTarget(...)`
     - `SnapResidentsToHomeAnchorsInternal()`
       - 复用 `ApplyResidentNightRestState(...)`
     - `FinishResidentReturnHome(...)`
       - 完成但不续 roam 时也会释放 shared owner
     - `ResumeResidentAutonomousRoam(...)`
       - 新增 façade 级恢复 helper
     - 移除 `ForceRestartResidentRoam(...)`
     - 文件内不再直接使用 `StartRoam`
  2. `SpringDay1DirectorStagingTests.cs`
     - 新增 3 条测试：
       - `CrowdDirector_ShouldRecoverReleasedResidentsIntoTrackedStateWithoutRestartingRoam`
       - `CrowdDirector_ShouldReleaseSharedOwnerWhenReturnHomeCompletesWithoutRoamResume`
       - `CrowdDirector_ShouldNotKeepNightRestResidentsUnderCrowdScriptedOwner`
- 验证：
  1. `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - `owned_errors=0`
     - `unity_validation_pending`（原因仍是 Unity `stale_status` / CodeGuard timeout）
  2. `validate_script Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - `owned_errors=0`
     - 同样 `unity_validation_pending`
  3. direct MCP 定向 EditMode 9/9 passed：
     - 上轮 6 条 package-B 旧用例
     - 本轮 3 条新用例
  4. `git diff --check -- [own files]` 通过
  5. Unity console 曾出现 test runner 清理尾巴：
     - `Saving results to ... TestResults.xml`
     - `Files generated by test without cleanup`
     - 已删除 `C:/Users/aTo/AppData/LocalLow/DefaultCompany/Sunset/TestResults.xml`
     - 已 clear console
     - fresh console 最终=`errors=0 warnings=0`
- 本轮核心判断：
  1. 当前最大收益不是再碰 `clock-owned return-home` 的深语义，而是先把 `recover/night-rest/restart-tail` 收干净。
  2. 这轮做完后，`CrowdDirector` 至少已经不再：
     - `spawn-state` 掉空就顺手重启 resident roam
     - 在 `night-rest` 里继续深持身体 owner
     - release 后继续手搓 restart 低级口
- 本轮最薄弱点：
  1. `20:00 -> 到家` 的 `clock-owned return-home` 仍在 Day1 schedule 私房 runtime 内。
  2. `snapshot restore` 对 `return-home` 的恢复链也还没重审。
  3. 这轮是结构收口 + targeted tests，不是 live 体感终验。
- 当前恢复点：
  1. 如果继续下一刀，只看：
     - `TryBeginResidentReturnHome`
     - `TickResidentReturnHome`
     - `ApplyResidentRuntimeSnapshotToState`
     这三处是否仍在不该由 Day1 持有的 schedule owner 链上。
  2. 不要扩到 dinner / UI / Primary / navigation core。
- 收尾：
  1. 已执行 `Park-Slice`
  2. 当前状态=`PARKED`
  3. 当前 blocker=`checkpoint-104-night-recover-deownership-ready`

## 2026-04-15｜Package B 第三刀补记：clock return-home snapshot 与到家语义收口
- 用户主线未变：继续按 `102` 的 owner 清理线推进，不回到 prompt，不扩到无边界总重构。
- 本轮子任务：
  - 只收 `20:00 clock-owned return-home` 的两条脏尾巴：
    1. 到家后又恢复 roam
    2. 跨场景恢复旧 `return-home` owner
- 本轮前置与约束：
  1. 已 `Begin-Slice`：`105_packageB_clock_return_owner_cut`
  2. 继续沿 `skills-governor / sunset-workspace-router / sunset-no-red-handoff` 的手工等价流程执行
  3. 不直接碰导航 core，也不在这轮强拆 `TryDriveResidentReturnHome` 的最终 owner
- 本轮实际改动：
  1. `SpringDay1NpcCrowdDirector.cs`
     - `TryBeginResidentReturnHome(...)`
       - 改成 `ResumeRoamAfterReturn=false`
     - `ShouldPersistResidentReturnHomeSnapshot(...)`
       - 改成始终不再持久化 `return-home` snapshot
     - `ShouldRestoreResidentReturnHomeSnapshot(...)`
       - 改成始终不再恢复 `return-home` snapshot
  2. `SpringDay1DirectorStagingTests.cs`
     - 新增：
       - `CrowdDirector_ShouldIgnoreClockOwnedReturnHomeSnapshotAtTwenty`
       - `CrowdDirector_ShouldQueueClockReturnHomeWithoutRoamResumeAtTwenty`
- 本轮验证：
  1. `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - `owned_errors=0`
     - `unity_validation_pending`
  2. `validate_script Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - `owned_errors=0`
     - `unity_validation_pending`
  3. direct MCP EditMode：
     - 第一轮 11/11 passed
     - 第二轮 snapshot 回归 4/4 passed
  4. Unity test runner 仍会生成 `TestResults.xml` 尾巴；本轮已两次手动删除并 clear console
  5. fresh console 最终=`errors=0 warnings=0`
  6. `git diff --check -- [own files]` 通过
- 本轮核心判断：
  1. 这轮最值钱的，不是再多砍一层，而是把 `clock-owned return-home` 先从混合脏合同收成清晰语义：
     - 回家
     - 不续 roam
     - 不靠旧 snapshot 续命
  2. 这样下一刀才值得专门审 `TryDriveResidentReturnHome / TickResidentReturnHome` 这条执行 owner。
- 本轮最薄弱点：
  1. `20:00 -> 到家` 的执行链本身仍在 `CrowdDirector` 内。
  2. 还没 fresh live 证明体感终局。
- 当前恢复点：
  1. 下一刀只看：
     - `TryDriveResidentReturnHome`
     - `HasActiveResidentReturnHomeDrive`
     - `TickResidentReturnHome`
  2. 不要扩到 dinner / UI / Primary / navigation core。
- 收尾：
  1. 已执行 `Park-Slice`
  2. 当前状态=`PARKED`
  3. 当前 blocker=`checkpoint-105-clock-return-owner-cut-ready`

## 2026-04-15｜Package B 第四刀补记：106 live 回归修正 checkpoint 已落，等待用户 fresh retest
- 用户最新 live 真相：
  1. `opening` 的 `001~003` 在对白开始后回起点
  2. opening 后白天 resident 不再要求回 anchor，`20:00` 前应自由漫游
  3. `Primary` 一进入就冻结时间
  4. `0.0.6` 回 `Town` 时 `003~203` 罚站
  5. `dinner` 的 `001/002` 会重复从 start 重走
- 本轮 slice：
  - `106_live-fix_opening-primary-dinner-regressions`
  - 已 `Park-Slice`
  - 当前 blocker=`checkpoint-106-live-regressions-fixed-awaiting-user-retest`
- 本轮实际改动：
  1. `SpringDay1Director.cs`
     - `TryHandleTownEnterVillageFlow()` 在 `VillageGate` dialogue active 时不再清 `_townVillageGateActorsPlaced`
     - `BeginDinnerConflict()` 拆开 dinner 的 `wait reset / placed reset`
     - `EvaluateDinnerCueStartPermission()` 改成与 opening 一致：timeout 后 settle 不完也不再无限等待
     - `ShouldPauseStoryTimeForCurrentPhase()` 改成 `Healing / Workbench / FarmingTutorial` 不再暂停
     - 白天最大时间从 `16:59` 收成 `16:00`
     - `_postTutorialExploreWindowEntered` 时 `GetCurrentBeatKey()` 改成 `FreeTime_NightWitness`
  2. `SpringDay1NpcCrowdDirector.cs`
     - 白天 autonomy-yield 早退口增加 `ShouldResyncBeforeSkippingDaytimeAutonomyYield()`
     - `_syncRequested=true` 或 `_spawnStates` 残留 stale/null shell 时，必须先 `SyncCrowd()`
     - `ShouldAllowResidentReturnHome()` 改成只认 `20:00 <= hour < 21:00`
  3. `SpringDay1DirectorStagingTests.cs`
     - 新增 / 改写 6 条贴刀口回归：
       - `Director_ShouldKeepVillageGateActorsPlacedWhileVillageGateDialogueActive`
       - `Director_ResetDinnerCueWaitState_ShouldPreservePlacedStoryActors`
       - `Director_ShouldKeepStoryTimeRunningDuringPrimaryPhases`
       - `Director_ShouldExposeFreeRoamBeatDuringPostTutorialExploreWindow`
       - `CrowdDirector_ShouldResyncRecoveredResidentsBeforeSkippingDaytimeYield`
       - `CrowdDirector_ShouldNotTreatTwentyOneAsReturnHomeContractWindow`
- 本轮验证：
  1. `validate_script SpringDay1Director.cs` => `owned_errors=0`
  2. `validate_script SpringDay1NpcCrowdDirector.cs` => `owned_errors=0`
  3. `validate_script SpringDay1DirectorStagingTests.cs` => `owned_errors=0`
  4. 统一仍是 `unity_validation_pending`（`stale_status / CodeGuard timeout`），但 direct MCP 定向 EditMode 15/15 passed：
     - staging 单测 11 条
     - opening/dinner runtime bridge 4 条
  5. fresh console 最终=`errors=0 warnings=0`
- 当前判断：
  1. 这轮已经把用户最新 live 直接指出的第一层 runtime regressions 收成可验 checkpoint。
  2. `20:00 -> 到家` 的 formal-navigation arrival owner 仍在 `TickResidentReturnHome()` 的 Day1/CrowdDirector 收尾链里，当前不安全继续硬拔。
  3. 当前正确口径只能是：
     - 结构 + targeted runtime tests 成立
     - 真实体验仍待用户 fresh live retest
- 当前恢复点：
  1. retest 最该先看：
     - `opening` 是否还会回起点
     - `Primary` 时间是否正常流逝并只在 `16:00` 锁住
     - `0.0.6` 回 `Town` 后 `003~203` 是否恢复自由漫游
     - `dinner` 是否还会重复从 start 走
  2. 如果这些过了，下一刀才继续看：
     - `TryBeginResidentReturnHome`
     - `TickResidentReturnHome`
     - `FinishResidentReturnHome`
     能否继续往共享 schedule / navigation contract 后撤。

## 2026-04-15｜107 live follow-up：opening/dinner 单次 staged contract 与 0.0.6 free window 补刀
- 用户这轮最新主线重新钉死为：
  1. `opening / dinner` 都只允许“一次 start -> 一次 end -> 开戏后不再动”的 staged contract。
  2. `0.0.6` 回 `Town` 时，除 `001/002` 外的 resident 不能继续被 crowd runtime 卡死。
- 本轮 slice：
  - `107_live-fix_opening-dinner-freeze-followups`
- 本轮实际改动：
  1. `SpringDay1Director.cs`
     - `MaintainTownVillageGateActorsWhileDialogueActive()` 改成对白开始后直接 `ForcePlaceTownVillageGateActorsAtTargets()`，不再重播 start。
     - `TryResolveDinnerStoryActorRoute(...)` 增加 `TryResolveDinnerStoryActorRouteFromSceneMarkers(...)` 优先级，晚饭 story actor 先吃场景 authored 起终点。
  2. `SpringDay1NpcCrowdDirector.cs`
     - `ShouldYieldDaytimeResidentsToAutonomy()` 不再把 `FreeTime_NightWitness` 当成持续 hold 的例外。
  3. `SpringDay1DirectorStagingTests.cs`
     - 新增：
       - `Director_ShouldPreferVillageCrowdSceneMarkersForDinnerStoryRoute`
       - `CrowdDirector_ShouldYieldAutonomyDuringNightWitnessFreeWindow`
- 本轮验证：
  1. direct MCP `validate_script`
     - `SpringDay1Director.cs` => `errors=0 warnings=3`
     - `SpringDay1NpcCrowdDirector.cs` => `errors=0 warnings=2`
     - `SpringDay1DirectorStagingTests.cs` => `errors=0 warnings=0`
  2. direct MCP EditMode 贴刀口回归 `7/7 passed`
  3. 测试尾巴 `TestResults.xml` 已删除并 clear console
  4. fresh console 最终=`errors=0 warnings=0`
  5. `git diff --check -- [own files]` 通过
- 本轮核心判断：
  1. opening 回起点、dinner 错位乱走、以及 `0.0.6` free window 继续卡住，这三条现在都已经有对应代码修正与 targeted tests。
  2. 这仍然不是 live 终验；当前只成立到“结构 + targeted runtime tests + fresh console clean”。
- 当前恢复点：
  1. 先等用户 fresh live retest：
     - `opening` 是否还会回起点
     - `0.0.6` 回 `Town` 后 resident 是否恢复正常
     - `dinner` 的 `001/002` 是否还会走错位或重走
  2. 如果这些过了，再继续看 `ReturnAndReminder / 20:00+`，不要提前扩题。

## 2026-04-16｜只读事故定责补记：`0.0.6` 回 Town 严重卡爆，当前最像 crowd 批量放手与 roam 批量重启叠出的性能/状态机坏链
- 用户最新要求：
  - 只读彻查，不改代码
  - 重点回答：
    1. `Update / SyncCrowd / TickResidentReturns / ShouldYieldDaytimeResidentsToAutonomy / ApplyResidentBaseline`
    2. 哪些条件会让 NPC 持续处于 `scripted control active but no stable move contract`
    3. 为什么 `opening` 结束后也会先卡一下再继续走
- 本轮前置：
  1. 已按 `skills-governor + sunset-workspace-router` 做前置核查
  2. 本轮只读，不跑 `Begin-Slice`
  3. 但现场 thread-state 发现自己仍挂着：
     - `ACTIVE`
     - `slice = 108_town_free_time_roam_spike_rootcause`
     收尾前应补 `Park-Slice`
- 本轮最核心判断：
  1. 现在最像的主根是：
     - `SpringDay1NpcCrowdDirector` 在白天 free window 成批 release resident
     - `NPCAutoRoamController` 对每个 NPC 同时重启 roam/pathing
     - 但它没有 herd-level 全局节流，只有 per-controller 的 path budget
     - 结果就是群体性的 repath / shared-avoidance / blocked-recovery 风暴
  2. 这条链直接落在：
     - `SpringDay1NpcCrowdDirector.Update()`
     - `ShouldYieldDaytimeResidentsToAutonomy()`
     - `YieldDaytimeResidentsToAutonomy()`
     - `ReleaseResidentToAutonomousRoam()`
     - `TryReleaseResidentToDaytimeBaseline()`
     - `NPCAutoRoamController.ResumeAutonomousRoam()`
     - `NPCAutoRoamController.StartRoam()`
     - `NPCAutoRoamController.TryBeginMove()/TickMoving()/TryRebuildPath()`
  3. `opening` 结束后“先卡一下再继续走”是同一根的轻量版：
     - `ResumeAutonomousRoam(true)` 在 `!IsRoaming` 时只会 `StartRoam()`
     - `StartRoam()` 结尾固定 `EnterShortPause(false)`
     - 所以 release 后天然会有一拍停顿
  4. 第二条并列危险链是：
     - `IsResidentScriptedControlActive && !IsResidentScriptedMoveActive`
     - `NPCAutoRoamController.Update()/FixedUpdate()` 会在这条支路里每帧 `ApplyResidentRuntimeFreeze()`
     - snapshot restore / formal-navigation end / crowd release 都可能把 NPC 留在这条坏态上
- 关键方法级事实：
  1. `NeedsDaytimeAutonomyRelease(...)`
     - 只要 `ReleasedFromDirectorCue / NeedsResidentReset / AppliedCueKey / AppliedBeatKey / IsResidentScriptedControlActive` 任一残留，就会继续进入白天放手队列
  2. `ApplyResidentRuntimeSnapshotToState(...)`
     - snapshot 如果带 `underDirectorCue`，会重新 `AcquireResidentDirectorControl(...)`
     - 但不保证当场就补 fresh move contract
  3. `EndDebugMove(...)`
     - recoverable point-to-point 结束时，会 `HaltResidentScriptedMovement()`
     - move contract 被停，但 owner 仍要等 crowd 后续手动放
  4. `TryAcquirePathBuildBudget()`
     - 只防“单个 NPC 同帧重复建路”
     - 不防“一群 NPC 同帧一起建路”
- 为什么“按 E 聊一次会解放一个 NPC”：
  1. `NPCDialogueInteractable / NPCInformalChatInteractable` 会给单个 NPC 跑一遍：
     - `AcquireStoryControl`
     - `HaltStoryControlMotion`
     - `ReleaseStoryControl`
  2. 这相当于一次单 NPC 的 locomotion hard reset + restart。
  3. 所以聊天更像“局部复位针”，不是根因本体。
- 当前最小修复方向（只记录，不动代码）：
  1. Day1/Crowd 先拆掉“批量放手 + 同帧立即重启 roam”。
  2. NPC/roam 再补 `ResumeAutonomousRoam(true)` 的 immediate 语义。
  3. 另外必须阻断 resident 长时间停在 `scripted active but no move contract`。
## 2026-04-16｜只读分析：`0.0.6` 回 Town 后为什么单独按 E 聊一次会把某个 NPC 解放
- 本轮性质：
  - 只读分析，不改代码，不跑 `Begin-Slice`
- 用户最新问题：
  1. `0.0.6` 回 `Town` 后，`003~203` 严重卡顿/卡爆
  2. 单独对某个 NPC 按 `E` 聊一次，这个 NPC 就会恢复正常漫游
  3. 要求彻查聊天链到底对 `NPCAutoRoamController` 做了什么，以及为什么这会把单个 NPC 解放
- 本轮结论：
  1. `PlayerNpcChatSessionService` 不直接调 `ResumeAutonomousRoam / HaltStoryControlMotion / StopMotion`；它只通过 `EnterConversationOccupation / ExitConversationOccupation` 间接驱动 interactable。
  2. 真正会改 locomotion 的是：
     - `NPCDialogueInteractable.EnterDialogueOccupation()`：
       `AcquireStoryControl -> HaltStoryControlMotion -> ApplyIdleFacing`
     - `NPCDialogueInteractable.HandleDialogueEnded()`：
       `ReleaseStoryControl`
     - `NPCInformalChatInteractable.EnterConversationOccupation()`：
       `AcquireStoryControl -> HaltStoryControlMotion`
     - `NPCInformalChatInteractable.ExitConversationOccupation()`：
       `ReleaseStoryControl`
     - fallback 无 roamController 时会直接 `StopMotion`
  3. `ReleaseStoryControl(...)` 进入 `NPCAutoRoamController.ReleaseResidentScriptedControl(...)` 后，只要：
     - owner 清空
     - `resumeResidentLogic == true`
     - `resumeRoamAfterResidentScriptedControl == true`
     - 当前不在 formal dialogue pause
     就会直接 `StartRoam()`
  4. `HaltStoryControlMotion()` 会做一次完整的局部 hard reset：
     - 清 move decision cache
     - 清 debug/direct-travel contract
     - 清 requested destination / interruption / soft-pass / shared-avoidance debug state
     - `state = Inactive`
     - `path.Clear()`
     - `StopMotion()`
  5. 所以“按 E 就恢复”的真实含义不是对话系统修好了它，而是：
     - 聊天链替这个 NPC 跑了一次 `hard reset + release + StartRoam`
     - 它像一次局部重启
- 最强判断：
  1. 这条现象更支持“每个 NPC 自己的 `NPCAutoRoamController` runtime 坏了”，而不是“还被 Day1 owner 持续抓着”。
  2. 强证据是 `NPCInformalChatInteractable.CanInteractWithResolvedSession(...)` 在 `IsResidentScriptedControlActive == true` 时直接返回 false；用户既然能对这些 NPC 开启闲聊，说明它们在聊天开始前大概率并没有 active scripted owner。
  3. 因此当前最可能根因是：
     - `0.0.6` 回 Town 时，Day1/CrowdDirector 把 resident 交回了一种“表面自治、内部坏态”的 roam runtime
     - 聊天链刚好把这个坏态清空并重新起漫游
  4. 这也解释了为什么随着“逐个聊天解放 NPC”，整体性能会逐步回升：更像每个 NPC 都各自卡在高成本 runtime 循环，而不是单个全局 owner 在统一锁死所有人
- 下一步最该继续看的代码：
  - `SpringDay1NpcCrowdDirector.ApplyResidentBaseline(...)`
  - `SpringDay1NpcCrowdDirector.ReleaseResidentToAutonomousRoam(...)`
  - `ResumeResidentAutonomousRoam(...)`
  - `NPCAutoRoamController` 的 move/repath/recovery/detour 残留
## 2026-04-16｜108 真实施工 checkpoint：`0.0.6` 回 Town 卡爆第一轮已收 bad-state restore / stale cue / batch immediate build
- 本轮性质：
  - 真实施工，沿用 `108_town_free_time_roam_spike_rootcause`
- 用户目标：
  1. 先把 `0.0.6` 回 `Town` 后 `003~203` severe 卡爆这条主痛点查清并处理。
  2. 语义不允许漂移；用户确认历史文档和 live 反馈没有偏。
- 本轮关键判断：
  1. `0.0.6` severe 卡爆不是单点条件，而是三条链叠在一起：
     - neutral snapshot / released-state recover 没有真正重启自治 roam
     - stale `underDirectorCue` 可能在白天 autonomy window 回抓导演 owner
     - crowd 在 free-time release 里批量 `ResumeAutonomousRoam(..., tryImmediateMove: true)`，把整群 resident 同帧推向建路/避障/blocked recovery
  2. 聊天链仍然只是“单 NPC 复位针”，不是根因 owner 本体。
- 本轮代码改动：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - `TryRecoverReleasedResidentState(...)`
       - recover released resident 后补 `ResumeAutonomousRoam(tryImmediateMove: false)`，不再只补 tracked state。
     - `ApplyResidentRuntimeSnapshotToState(...)`
       - neutral snapshot restore 后清 shared runtime owner，并补 `ResumeAutonomousRoam(tryImmediateMove: false)`。
       - 白天 autonomy window 下，`snapshot.underDirectorCue` 视作 stale cue，不再恢复导演 owner。
     - free-time release / baseline / cancel-return-home 的自治恢复口统一改成 `tryImmediateMove: false`。
  2. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 改写：
       - `CrowdDirector_ShouldRecoverReleasedResidentsIntoTrackedStateAndRestartAutonomousRoam`
     - 新增：
       - `CrowdDirector_ShouldRestartAutonomousRoamWhenApplyingNeutralResidentRuntimeSnapshot`
       - `CrowdDirector_ShouldIgnoreStaleDirectorCueSnapshotDuringDaytimeAutonomyWindow`
- 本轮验证：
  1. CLI `validate_script`
     - `SpringDay1NpcCrowdDirector.cs` => 最终 `assessment=no_red`
     - `SpringDay1DirectorStagingTests.cs` => `assessment=no_red`
  2. direct validate
     - `SpringDay1NpcCrowdDirector.cs` => `errors=0 warnings=2`
  3. direct EditMode `8/8 passed`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldIgnoreStaleDirectorCueSnapshotDuringDaytimeAutonomyWindow`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldRecoverReleasedResidentsIntoTrackedStateAndRestartAutonomousRoam`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldRestartAutonomousRoamWhenApplyingNeutralResidentRuntimeSnapshot`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldCaptureAndReapplyResidentRuntimeSnapshot`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldNotPersistStoryDrivenReturnHomeSnapshotBeforeFreeTime`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldIgnoreClockOwnedReturnHomeSnapshotAtTwenty`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldResyncRecoveredResidentsBeforeSkippingDaytimeYield`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldYieldAutonomyDuringNightWitnessFreeWindow`
  4. fresh console：
     - 测试尾巴 / 外部 warning 已清
     - 最终 `0 error / 0 warning`
- 当前恢复点：
  1. 下一次 fresh live 最该先验：
     - `0.0.6` 回 Town 后 `003~203` 是否还 severe 卡爆
     - 是否还需要靠聊天 reset 才能逐个恢复
     - opening 结束后是否仍残留明显小卡顿
  2. 如果还有残余，下一刀继续看：
     - 是否还有未覆盖的 `ResumeAutonomousRoam(true)` 群体触发口
     - `NPCAutoRoamController` 是否仍有 lingering blocked-recovery / soft-pass / repath 坏态需要 NPC 侧补护栏
## 2026-04-16｜slice 109：Town 自由时段卡顿从 Day1 owner 问题转向 NPC locomotion 重负载
- 本轮主线：
  - 用户最新要求是继续彻查“晚饭前 Town 自由时段仍严重卡顿”，不要被旧叙事带着跑。
  - 前一阶段 Day1/Crowd 的 stale cue / released-state / immediate restart 已收过一轮；现在 profiler 主热点明确转到 `NPCAutoRoamController.Update()`。
- 这轮实际动作：
  1. 继续保持 `Begin-Slice` 后的 `ACTIVE` 状态，沿用 `109_town_free_time_roam_overlapcircle_rootcause`。
  2. 只读审计确认：
     - 主峰值链更像 `TryHandleSharedAvoidance + NavigationAgentRegistry.GetNearbySnapshots + NavigationLocalAvoidanceSolver.Solve`
     - 次峰值链更像 `AdjustDirectionByStaticColliders + ProbeStaticObstacleHits`
     - `UpdateTraversalSoftPassStateOncePerFrame` 是次级常驻成本，不是唯一主锅
  3. 真实改动：
     - `NPCAutoRoamController.cs`
       - traversal soft-pass refresh throttle
       - autonomous move decision cross-frame reuse
     - `NavigationAgentRegistry.cs`
       - active per-frame snapshot cache
     - `NavigationAvoidanceRulesTests.cs`
       - 4 条新/补回归
  4. 尝试过 `NPCBubblePresenter` hidden 早退，但由于当前 bubble guard 测试没给出足够干净的归因票，已主动撤回，不把它混进交付。
- 验证：
  1. script validate：
     - `NPCAutoRoamController.cs` / `NavigationAgentRegistry.cs` / `NavigationAvoidanceRulesTests.cs` 均 `errors=0`
  2. EditMode targeted `7/7 passed`
  3. fresh console 最终已 clear
  4. 期间有一次外部 blocker：
     - test run 因 Unity 进入 Play Mode 被拒；已 `stop` 后重跑成功
- 当前最核心判断：
  - 这轮成立的是“Town 自由时段卡顿的首批 NPC/runtime 性能治理”，不是 Day1 体验已经过线。
- 最薄弱点：
  - 没有 fresh live；还不能 claim 用户体感已经恢复。
  - 第二嫌疑 `AdjustDirectionByStaticColliders` 还没动，如果 live 仍卡，这里是最可能的下一刀。
- 当前恢复点：
  - 若下一轮继续：
    1. 先看用户 fresh live
    2. 若仍卡，优先查 static steering / static obstacle probe
  - 若本轮先停：
    - 已执行 `Park-Slice`
    - 当前 `thread-state = PARKED`
    - blocker=`fresh-live-retest-required-for-town-free-time-performance-and-day1-runtime-behavior`
## 2026-04-16 09:47:20｜只读审计：导航V3“切 Town 启动风暴”方案是否可行

### 用户目标
- 用户要求我独立审核导航V3的反思与修复方案，判断它是否真的可行，不能被对方文案带着跑。

### 当前主线
- Day1-V3 当前仍在“厘清 Day1 own 与导航 own 的真实边界”，并判断下一刀该不该交给导航、交给什么形状的导航刀。

### 本轮实际做成
1. 复读了：
   - `100-重新开始 memory`
   - `spring-day1-implementation 父层 memory`
   - `Day1-V3 thread memory`
   - `导航V3 memory`
2. 直接核了现码：
   - `SpringDay1NpcCrowdDirector.Update / ShouldRecoverMissingTownResidents / RecoverReleasedTownResidentsWithoutSpawnStates / TryRecoverReleasedResidentState / ShouldYieldDaytimeResidentsToAutonomy / ReleaseResidentToAutonomousRoam`
   - `NPCAutoRoamController.StartRoam / ResumeAutonomousRoam / TryBeginMove / AdjustDirectionByStaticColliders / ProbeStaticObstacleHits / TryAcquirePathBuildBudget`
   - `NavigationAgentRegistry.GetNearbySnapshots / active cache / snapshot cache`
3. 得到的独立判断：
   - 导航“Day1 主触发 + 导航主烧点”的大方向仍成立。
   - 但它继续把问题讲成旧版 `tryImmediateMove:true` 风暴已经不够准；Day1 白天 free-time 主释放口现在大多已改成 `false`。
   - 真实残留更像：
     - Day1 在 Town 返场/白天释放时成批把 resident 送回 roam runtime
     - NPC/导航在同一短窗内集中进入 `StartRoam -> ShortPause -> TryBeginMove`
   - 转场黑屏偏长还有明确 Day1 own 搜索/补绑成本，不能只归导航。
4. 对方案本身的裁定：
   - “可行，但不够完整”
   - “可做，但必须收窄”
   - “不能按原文继续泛修”

### 现在还没做成什么
- 没有改代码。
- 没有 fresh live / compile / profiler 复跑。
- 这轮只成立到“代码对位后的高置信审计结论”。

### 当前阶段
- 处于“路线审计完成、可判下一刀形状”的阶段，不是施工阶段。

### 下一步只做什么
- 如果用户让我继续开刀，Day1-V3 这边应继续盯：
  1. Town 返场补绑/找 anchor 的一次性入口成本
  2. 白天 batch release 的语义边界
- 对导航的协作口径则应收窄成：
  - scene-entry herd-start 短窗治理
  - bad-start cheap-fail
  - 不回吞 Day1 phase

### 需要用户现在做什么
- 无。

### 关键判断
- 导航的方案不是错方向，而是“方向对、包太大、并且有一部分其实已经做过了”。
- 如果继续按那份原文全包推进，最容易重复犯的错，就是把已经部分落地的护栏再重写一轮，最后继续制造“看起来一直在修、其实切口不够硬”的返工。

### 最薄弱点
- 这轮没有 fresh runtime 证据，所以我不能把它说成 live 铁证，只能说“现码与既有 profiler 记忆能对上”。

### 自评
- `8.9/10`
- 因为这轮已经把“导航方案能做什么、不能替代什么、哪些地方已经过时”分清了；没到满分，是因为我这轮刻意没进 live 复核。
## 2026-04-16 09:57:29｜用户向汇报判断：下一刀、未完成项、语义同步程度

### 用户问题
- 用户问：
  1. 我认为的下一刀方向是什么
  2. 当前还有什么没做好
  3. 语义是否已经同步完全

### 当前答案
1. 下一刀方向：
   - 先砍 Day1 own 的 `scene-entry / Town 返场补绑 / 白天 batch release` 入口链
   - 不先去做导航全包重构
   - 不先去泛修剧情表现
2. 还没做好的内容：
   - opening / dinner 统一的一次走位合同还不能 claim 完全过线
   - Primary 时间流逝语义还不能 claim 完全过线
   - `0.0.6` 回 Town 的黑屏长与群体卡顿还没有体验终验
3. 语义同步程度：
   - 边界和方向已经基本同步
   - 完整 runtime 落地仍未完成
   - 所以答案是：`没有完全同步`

### 为什么这样判断
- 因为当前代码里仍能看到：
  - Day1 在 Town 返场时的 resident 搜索/补绑/找 anchor 成本
  - Day1 在白天窗口的 batch release 触发
  - 而用户最新强语义里的 several hard requirements 还没有资格 claim 全部已过

### 当前恢复点
- 如果下一轮继续施工，先看：
  - `RecoverReleasedTownResidentsWithoutSpawnStates`
  - `TryRecoverReleasedResidentState`
  - `FindSceneResidentHomeAnchor`
  - `FindAnchor`
  - `ShouldYieldDaytimeResidentsToAutonomy`
  - `ReleaseResidentToAutonomousRoam`

## 2026-04-16｜停在最近刀口：只做进度收口
- 用户当前要求：
  - 停在最近刀口
  - 言简意赅汇报所有进度情况
- 当前停点：
  - 这轮不再继续开新的修复刀
  - 只把进度收口到 `Day1 own scene-entry / Town 返场补绑 / 白天批量放手` 这条入口链
- 当前已确认的进度状态：
  1. `Day1 / 导航 / NPC` 的边界已经基本同步，但完整 runtime 仍未终验。
  2. `0.0.6` 回 Town 的 severe 卡爆，已经从单纯 Day1 owner 问题收窄为 `Day1 返场入口成本 + NPC locomotion 重负载`。
  3. `opening / dinner` 的一次走位合同、Primary 时间流逝、以及 20:00 以后的夜间链仍未完全过线。
- 当前恢复点：
  - 下轮若继续，仍优先看 Town 返场补绑、白天批量放手和转场入口成本，不先扩成全包重构。

## 2026-04-16｜110 真实施工 checkpoint：晚饭 full staged rewrite 第一刀已落
- 本轮 slice：
  - `110_dinner_full_staged_rewrite`
- 用户最新强语义：
  1. 晚饭 `18:00 -> 19:30` 期间，`001~203` 都按 opening 同规格 staged contract 处理。
  2. 所有 NPC 只允许：
     - 先到起点
     - 再走一次到终点
     - 最多 5 秒
     - 超时 snap 到终点
     - 对话开始后不再 roam / 不再二次走位
  3. 本轮先不碰 `20:00` 回 anchor/home。
- 本轮关键判断：
  1. 晚饭“一排站队”的根因不是单点坐标错，而是两条旧链叠在一起：
     - `SpringDay1Director` 仍有 `001/002` 专用 dinner gather 逻辑，没把晚饭当成 opening 同规格 staged contract。
     - `SpringDay1NpcCrowdDirector` 对 `DinnerConflict_Table` 的 cue 解析会让没有显式 dinner cue 的 resident 因共享 `DinnerBackgroundRoot` semantic anchor 误吃别人的 cue。
  2. 第二条是这次最脏的 owner/数据口：它直接解释了为什么晚饭开始时多名 resident 会被塞去同一条错路线。
- 本轮真实改动：
  1. `SpringDay1Director.cs`
     - `PrepareDinnerStoryActorsForDialogue()` 把 `003` 也并入晚饭 staged contract。
     - `TryResolveDinnerStoryActorRoute(...)` 不再退回 home-anchor / `start=end` 假路线；现在只吃：
       - scene authored dinner markers
       - dinner beat 自身 cue
  2. `SpringDay1NpcCrowdDirector.cs`
     - `TryResolveStagingCueForCurrentScene(...)`
       - `DinnerConflict_Table` 下如果 stage book 返回的 cue 不是当前 npcId 的显式 cue，就丢弃，不再允许共享 semantic anchor 串号。
       - 新增 `TryBuildDinnerFallbackCue(...)`，为没有显式 dinner cue 的 resident 生成：
         - `DirectorReady_* -> ResidentSlot_*`
         的单次 runtime dinner fallback route。
  3. `SpringDay1DirectorStagingTests.cs`
     - 新增 `003` dinner route 测试
     - 新增 dinner fallback cue 测试
     - 保留 dinner beat 不得串 cue 的回归
- 验证：
  1. `validate_script`
     - `SpringDay1Director.cs` => `errors=0 warnings=3`
     - `SpringDay1NpcCrowdDirector.cs` => `errors=0 warnings=2`
     - `SpringDay1DirectorStagingTests.cs` => `errors=0 warnings=0`
     - CLI assessment 被外部 `Town.unity` PlayMode/stale_status + scene 外部 errors 卡成 `external_red`，不是我 own 红
  2. direct EditMode targeted `4/4 passed`
  3. `git diff --check` 通过
  4. test tail `TestResults.xml` 已删除
  5. fresh console 最终 `0 log entries`
- 当前状态：
  1. 已执行 `Park-Slice`
  2. 当前 thread-state=`PARKED`
  3. checkpoint=`110_dinner_full_staged_rewrite_ready_for_live_retest`
- 当前恢复点：
  - 下一步优先让用户 fresh retest 晚饭进入瞬间：
    1. 是否还会“一排站队”
    2. 是否还会二次走位
    3. 对话开始后是否彻底不再 roam
  - `20:00` 回家链仍未处理，不把这轮包装成整条晚饭+夜间全过。
## 2026-04-16 只读彻查：晚饭入口新阻塞定位
- 当前主线：修正晚饭 staged contract，恢复用户要求的“18:00~19:30 全员先传起点、只走一次到终点、对白开始后不再动”。
- 本轮子任务：只读钉死“为什么现在传错位、不开戏、NPC 不动”。
- 关键结论：这是我上一刀晚饭半重写造成的新阻塞。导演层仍在用 001/002/003 特化开戏链，crowd 层已经新增 dinner fallback cue；两边合同不一致，导致 NPC 被送去 DinnerBackgroundRoot 相关锚点后，DinnerConflict 仍等不到统一 settled，剧情不开始。
- 涉及文件：SpringDay1Director.cs、SpringDay1NpcCrowdDirector.cs、SpringDay1DirectorStageBook.json、Town.unity。
- 本轮未改代码、未跑测试；thread-state 保持 PARKED，下一轮需要先 Begin-Slice 再进入真实施工。
## 2026-04-16 切片 111_dinner_unified_entry_rewrite 完成并停在可复测基线
- 本轮真实施工：
  - 修正 SpringDay1Director 的晚饭 marker 根优先级，优先 `进村围观` 分组。
  - 晚饭入口在切 Phase 后立即 ForceImmediateSync crowd。
  - CrowdDirector 的 dinner runtime cue 开始使用 scene marker override。
  - 更新 SpringDay1DirectorStagingTests / SpringDay1LateDayRuntimeTests 到新的“起点->终点”语义。
- 自验：
  - targeted staging tests 5/5 pass
  - targeted late-day runtime tests 4/4 pass
  - validate_script(4 files) => no_red
  - console 0 error / 0 warning
- 当前阶段：晚饭入口 bug 修复切片已完成；尚未继续 20:00/21:00 后续退权。
- 恢复点：下次从 resident return-home / rest owner 退权继续。
## 2026-04-16 只读排查：opening/dinner 垃圾中间层依赖
- 当前主线：按用户原始语义清理 Day1，opening/dinner 只吃 `进村围观/起点/终点`，回家只吃 `001~203_HomeAnchor`。
- 本轮子任务：只读查明 `Town_Day1Residents` / `Town_Day1Carriers` 及相关抽象锚点是否可删、现在依赖在哪里。
- 结论：它们没有不可替代语义，但依赖已经散到 runtime、manifest、stagebook、contract、tests 和 scene；必须先断依赖再删节点。
- 重点污染：SpringDay1NpcCrowdDirector 的 reparent 分组、semantic alias、dinner fallback cue、DailyStand/NightWitness baseline、StageBook/Manifest 的 abstract semantic anchors、SceneTransitionTrigger2D 的 EnterVillageCrowdRoot 入口锚。
- 下一恢复点：开施工刀时先移除/禁用抽象 anchor fallback 与 root reparent 依赖，再清数据和测试，最后删 Town scene 节点。
## 2026-04-16 只读盘点：抽象锚点体系彻底删除时的必改清单
- 当前主线：给用户一份“如果彻底删除抽象锚点体系，哪些文件会直接炸”的最小安全清单，不做代码改动。
- 本轮子任务：补查容易漏掉的 editor 工具、入口常量和半外围数据资产。
- 新增关键发现：
  1. `Assets/Editor/Town/TownSceneRuntimeAnchorReadinessMenu.cs`、`TownScenePlayerFacingContractMenu.cs`、`TownNativeResidentMigrationMenu.cs` 直接把 `EnterVillageCrowdRoot / DinnerBackgroundRoot / NightWitness_01 / DailyStand_* / DirectorReady_* / ResidentSlot_* / Town_Day1Residents` 当场景 contract 真值。
  2. `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`、`SpringDay1NpcCrowdBootstrap.cs` 直接把旧 semanticAnchorIds 和 duty 分布写死；删锚点后即使 runtime 已迁，validation/bootstrap 也会继续把旧体系当正确答案。
  3. `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs` 的 `TownOpeningEntryAnchorName = "EnterVillageCrowdRoot"` 是 opening 默认入口常量级炸点。
  4. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs` 仍保留 `PreferredDinnerGatheringAnchorObjectNames = { "DirectorReady_DinnerBackgroundRoot", "DinnerBackgroundRoot" }`，是晚饭入口的残留硬依赖。
  5. `Assets/Resources/Story/NpcCharacterRegistry.asset` 继续显式消费 `EnterVillage_PostEntry / DinnerConflict_Table / FreeTime_NightWitness / DailyStand_Preview` 这组旧 beat 语义；若后续把 late-day 旧 beat 一并去语义化，需要同步收口。
- 当前判断：
  - 这轮已能给出删除顺序：先 runtime/入口常量，再数据资产，再 editor/tests，最后场景节点。
  - thread-state 仍为 `PARKED`，因为本轮始终只读。
## 2026-04-16｜切片 112：Town 场景误删恢复已完成
- 用户最新纠偏：
  - 我删掉抽象根时把 `101~203` 也删没了；用户要求不要把旧脏根加回来，只把人和各自 `HomeAnchor` 放回 `NPCs`。
- 本轮真实施工：
  1. 沿用 `112_remove_day1_abstract_anchor_layers` 继续做 scene 修复。
  2. 从旧 `Town.unity` 反查 `101_HomeAnchor~203_HomeAnchor` 历史位置。
  3. 通过 Unity 现场把 `101/102/103/104/201/202/203` prefab 重新实例化到 `NPCs`。
  4. 新建并恢复 `101_HomeAnchor~203_HomeAnchor` 到 `NPCs`。
  5. 给 7 个新增 resident 的 `NPCAutoRoamController` 重新绑定 `homeAnchor`；`navGrid` 保持 `NavigationRoot`。
  6. 保持旧抽象根不回流：
     - `Town_Day1Residents` 仍不存在
     - `Town_Day1Carriers` 仍不存在
     - `EnterVillageCrowdRoot` 仍不存在
- 自验：
  1. targeted EditMode `9/9 passed`
     - staging marker / dinner cue 7 条
     - opening runtime bridge 2 条
  2. `git diff --check -- Assets/000_Scenes/Town.unity` 通过
  3. fresh console 最终=`errors=0 warnings=0`
  4. 测试副产物 `Assets/__CodexEditModeScenes` 与 `C:/Users/aTo/AppData/LocalLow/DefaultCompany/Sunset/TestResults.xml` 已清理
- 当前判断：
  1. 这轮修掉的是我自己制造的 scene 现场事故，不是新语义变化。
  2. 现在 `Town` 已回到“scene resident 齐全，但旧抽象根继续退场”的正确方向。
- 当前恢复点：
  - 下一轮继续回到 Day1 抽象中间层退场后的 runtime/owner 清扫，不需要再为 `101~203` 缺席分神。
- 收尾：
  - 已执行 `Park-Slice`
  - 当前状态=`PARKED`
  - 当前 blocker=`checkpoint-112-town-residents-restored-ready-for-next-runtime-cleanup`

## 2026-04-16｜只读调查：用户 5 个语义问题的代码真相
- 用户最新明确要我停下施工，先把这 5 个问题彻底查清：
  1. `opening` 后所有 NPC 应该干什么；用户语义是什么；现码是什么
  2. “走到艾拉附近”当前到底判多近，改起来是不是只改数字
  3. 做完任务回 `Town` 时 `001/002` 应该站哪里
  4. `19:00` 提醒两点前睡下时，代码做了什么；是否意味着 NPC 已经过完剧情
  5. `001~003` 的家在哪里；晚上会不会回家
- 本轮只读调查结论：
  1. `opening` 后：
     - 普通 resident（`101~203` + synthetic `003`）不是在 `EnterVillage` 里立刻放飞，而是在离开 `EnterVillage` 后，且不处于 `DinnerConflict_Table / ReturnAndReminder_WalkBack / DayEndSettle / DailyStandPreview` 时，才由 `SpringDay1NpcCrowdDirector.ShouldYieldDaytimeResidentsToAutonomy()` 放回白天自治。
     - `001/002` 不是普通 resident；它们仍由 `SpringDay1Director` 作为 story actor 持有，直到各自剧情桥接收完。
  2. 艾拉桥接距离：
     - 当前主阈值是 `SpringDay1Director.healingSupportApproachDistance = 0.9f`
     - 但比较点是 `interaction sample point` 而不是 player transform；体感还受 `healingSupportPauseDuration = 0.18f` 影响
  3. `0.0.6` 回 `Town` 时 `001/002` 的位置真相：
     - 代码里没有“回 Town 后再把 001/002 摆去某个单独待机点”的逻辑
     - `AlignTownDinnerGatheringActorsAndPlayer()` 只移动玩家
     - 晚饭开始前，`001/002` 吃的是 `Town.unity` 的 scene 原生摆位：
       - `001 = (-12.55, 14.52)`
       - `002 = (-10.91, 16.86)`
  4. `19:00` 提醒段：
     - `BeginReturnReminder()` 会切到 `ReturnAndReminder`
     - `EnterFreeTime()` 要到 `19:30` 才切 `FreeTime`
     - 所以 `19:00~19:29` 仍然是剧情段，不是“NPC 已经过完剧情”
  5. `001~003` 夜间回家链：
     - `Town.unity` 里当前 home anchor 真值：
       - `001_HomeAnchor = (-12.15, 13.12)`
       - `002_HomeAnchor = (-17.67, 13.94)`
       - `003_HomeAnchor = (43.70, -10.70)`
     - `001/002` 夜里由 `SpringDay1Director.SyncStoryActorNightRestSchedule()` 在 `20:00` 请求回家、`21:00` 强制 rest
     - `003` 不再归导演夜间 owner，而是通过 `SpringDay1NpcCrowdDirector.BuildSyntheticThirdResidentResidentEntry()` 并回 resident runtime，夜里走 crowd 的 `SyncResidentNightRestSchedule()`
- 当前最关键判断：
  1. 用户最新“白天不回 anchor，只要 20:00 后再回家”这条，现码已经大体对齐。
  2. 但 `19:00 reminder` 这段绝不能包装成“剧情已放手”，因为代码里它仍是正式剧情态。
  3. `0.0.6` 回 `Town` 的 `001/002` 不是被导演摆在某个额外白天点位，而是 scene 原生摆位；这点如果用户期待别的站位，后面必须明确再改。
- 本轮没有改代码、没有跑 `Begin-Slice`，thread-state 继续保持 `PARKED`。

## 2026-04-17｜只读彻查：打包版最新 5 点反馈的根因与修法评估
- 用户新增 packaged-build 实测后，要求我：
  1. 结合他的深度测试和语义，把 Day1 当前所有实际问题列清。
  2. 每个问题给出解决办法、风险评级、能不能在打包前安全修。
- 本轮新收敛的代码事实：
  1. `opening` 顺序问题的直接代码根是：
     - crowd resident 由 `SpringDay1NpcCrowdDirector` staged cue 先跑
     - `001~003` 由 `SpringDay1Director` village-gate route 单独跑
     - crowd director 还有 `[DefaultExecutionOrder(-300)]`
  2. `opening` 结束后 crowd resident 传回 baseline 的根是：
     - cue 释放后 `ApplyResidentBaseline(...)` 进入 `TryReleaseResidentToDaytimeBaseline(...)`
     - 方法内部直接把 resident 传到 `BasePosition`
  3. `19:00` 时普通 resident 的异常不是时钟回家：
     - crowd resident return-home 明确是 `20:00~20:59`
     - `21:00+` forced rest
     - 因此 `19:00` 的异常更像 beat 切换后的 baseline teleport / visibility 偏差
  4. `001/002` 与 `003` 的交互分裂是代码现状：
     - `001/002` 在 `DinnerConflict / ReturnAndReminder` 仍被 story actor mode 禁 formal/informal chat
     - `003` 没跟进这条 policy
  5. 夜间合同仍 split：
     - `001/002` = director night schedule
     - `003~203` = crowd resident night schedule
     - 当前只有回家/snap/rest，没有 21:00 hide 与次日统一激活
  6. 疗伤靠近艾拉触发回血仍是单参数：
     - `healingSupportApproachDistance = 0.9f`
- 当前我的主判断：
  1. 这轮最值得优先收的不是再争论截图，而是 3 组硬问题：
     - opening owner split
     - daytime baseline teleport
     - night schedule split
  2. 艾拉 `1.6` 可以单独收成一个极低风险小刀。
  3. `001/002` 在 `FreeTime` 后若仍无法聊天，单看 Day1 代码并没有继续硬锁到 `FreeTime`，这条后续若还存在，可能已跨到 interaction/dialogue runtime，不应只怪 Day1 一边。
- 本轮仍是只读；未跑 `Begin-Slice`，当前 `thread-state = PARKED`。

## 2026-04-17｜0417 主控文档已落盘并完成停车
- 用户最新裁定：
  - 不先继续开新代码刀
  - 先把 Day1 现阶段全部语义、代码真相、问题总表、施工顺序和持续维护 tasks 收成一份新的 `0417.md`
  - 这份文件以后要成为整轮 Day1 修复的唯一主控文档
- 本轮已做：
  1. 显式使用：
     - `skills-governor`
     - `sunset-workspace-router`
     - `preference-preflight-gate`
  2. 手工等价执行：
     - `sunset-startup-guard`
     - `user-readable-progress-report`
     - `delivery-self-review-gate`
  3. 执行 `Begin-Slice`
     - `slice=0417文档建档与全量执行清单落盘`
  4. 读取：
     - `100-重新开始/memory.md`
     - 父层 `spring-day1-implementation/memory.md`
     - 当前线程 `memory_0.md`
     - `102-owner冻结与受控重构` 里的总表与导航/NPC contract
     - 用户指定参考 `箱子系统/7_终极清算/tasks.md`
  5. 新增：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\100-重新开始\0417.md`
  6. 执行 `Park-Slice`
     - 当前 `thread-state = PARKED`
- `0417.md` 当前已写死的关键内容：
  1. `用户语义真值`
     - opening：`001~203` 全员传起点、走一次到终点、最多 5 秒、超时 snap、对白后不再动
     - opening 后：ordinary resident 原地解散，白天自由活动，`20:00` 前不回 baseline/anchor/home
     - primary：时间正常流逝，到 `16:00` 才锁；艾拉回血距离应改到 `1.6`
     - dinner：`18:00~19:30` 全员只走一次位，不再额外走位
     - 夜间：`20:00 return-home`、`21:00 snap-to-anchor + hide`、次日从 anchor 激活
  2. `当前代码真相`
     - opening 双 owner
     - opening 后 baseline teleport
     - `003` 仍有 story-actor 残留入口
     - 夜间合同仍 split
     - 疗伤距离当前还是 `0.9`
     - 抽象锚点残依赖仍在
  3. `全量问题总表`
     - `I-01 ~ I-13`
     - 每条都有：现象、根因摘要、证据等级、风险、打包前安全性、推荐阶段
  4. `分阶段施工与 tasks`
     - `P0 ~ P6`
     - 含验证点 `T-01 ~ T-12`
- 本轮没做：
  1. 未改运行时代码
  2. 未跑 compile / tests / live
  3. 不宣称任何体验过线
- 当前最核心判断：
  1. Day1 现在不再缺“用户语义”，也不再缺“问题总图”。
  2. 下一轮如果继续，必须直接以 `0417.md` 为唯一主控板，不再从长聊天或旧 prompt 拼上下文。
  3. 当前推荐执行顺序已经冻住：
     - `P2-1` 疗伤距离小刀
     - `P1` opening/daytime 主链
     - `P2` primary 与 `0.0.6`
     - `P3` dinner/late-day
     - `P4` 夜间统一合同
     - `P5` 抽象锚点与测试清扫
     - `P6` 全线终验
- 审计层：
  1. 已追加 `STL-20260417-012`
  2. `check-skill-trigger-log-health.ps1` 结果：
     - `Canonical-Duplicate-Groups = 0`

## 2026-04-17｜0417 任务区二次冻结：已改成包级任务板
- 用户最新纠正：
  - 认为 `0417` 当前的任务部分不够细、不够全面，后续会导致我继续漂移。
  - 要求我重新客观梳理整个 Day1 的实际情况，然后完善文档。
- 本轮真实动作：
  1. 执行 `Begin-Slice`
     - `slice=0417任务区重整与Day1现状二次冻结`
  2. 重新核对：
     - `SpringDay1Director.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - `SpringDay1DirectorStagingTests.cs`
     - `SpringDay1LateDayRuntimeTests.cs`
     - `SpringDay1NpcCrowdManifest.asset`
     - `SpringDay1DirectorStageBook.json`
     - `SpringDay1TownAnchorContract.json`
     - `Assets/Editor/Town/*`
     - `Assets/Editor/NPC/*`
  3. 已执行 `Park-Slice`
     - 当前 `thread-state = PARKED`
- `0417.md` 这轮新增的关键内容：
  1. 新增 `2.2 当前已经结构性落地、但不能冒充过线的内容`
     - NPC façade 已存在且 Day1 已部分接入
     - primary “不暂停时钟”的结构判断已写进代码与测试
     - resident return-home 主路已不是 transform 硬推
     - dinner 已存在 cue wait + actor prepare 链，但仍不是正确终局
  2. 新增 `2.3 当前最容易让我后续漂移的 5 个真源`
     - runtime owner split
     - data asset 仍在喂旧 beat / anchor
     - editor/bootstrap/validation 仍在喂旧真相
     - tests 混合保护新旧两套真相
     - 用户 live 与静态结构不等价
  3. 全量问题新增：
     - `I-14 旧 beat / semantic anchor 债务`
     - `I-15 editor/bootstrap/validation 合同仍旧`
  4. `8. 迭代 tasks` 已重写为 `Package A~G`
     - 每个 package 都包含：
       - 目的
       - 主要锚点
       - 前置
       - 具体 tasks
       - 退出条件
- 当前最核心的新判断：
  1. 之前 `0417` 容易漂，不是因为任务条目太少，而是没有把：
     - runtime
     - 资产
     - editor
     - tests
     - 用户 live
     这五层错位一起压进任务区。
  2. 现在这版 `0417` 已经足够作为后续连续施工的唯一主控板。
  3. 下轮如果继续真实施工，不应再从旧 prompt 和长聊天取上下文，而应直接按 `Package A~G` 维护。
- 当前恢复点：
  1. 推荐顺序已更新为：
     - `A`
     - `C-01`
     - `B`
     - `C`
     - `D`
     - `E`
     - `F`
     - `G`
  2. 本轮仍未改运行时代码，未跑 compile / tests / live，不宣称体验过线。

## 2026-04-17｜Package A 已真实完成：六张冻结表落盘
- 用户继续要求：
  - 不要只停在“任务板结构变细”
  - 要真正把 `Package A` 的六张冻结表补出来
- 本轮真实动作：
  1. 执行 `Begin-Slice`
     - `slice=PackageA_冻结补表与防漂移底板`
  2. 以以下文件为锚点完成冻结补表：
     - `SpringDay1Director.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - `SpringDay1DirectorStagingTests.cs`
     - `SpringDay1LateDayRuntimeTests.cs`
     - `SpringDay1NpcCrowdManifest.asset`
     - `SpringDay1DirectorStageBook.json`
     - `SpringDay1TownAnchorContract.json`
     - `Assets/Editor/Town/*`
     - `Assets/Editor/NPC/*`
  3. 在 `0417.md` 中新增：
     - `5.4.1 语义 -> StoryPhase / beatKey / runtime entry / test 映射表`
     - `5.4.2 当前 owner matrix`
     - `5.4.3 resident lifecycle 全入口冻结表`
     - `5.4.4 旧 beat / semantic anchor 依赖矩阵`
     - `5.4.5 editor / bootstrap / validation 残依赖矩阵`
     - `5.4.6 测试分层表`
     - `5.4.7 Package A 当前完成度`
  4. 已把 `Package A` 下的：
     - `A-01 ~ A-06`
     全部标记完成
  5. 已执行 `Park-Slice`
     - 当前 `thread-state = PARKED`
- 当前最核心判断：
  1. Day1 这条线现在已经不缺“防漂移底板”。
  2. 下一轮不需要再回头补冻结表，可以直接按顺序进入：
     - `C-01`
     - `B`
     - `C`
     - `D`
     - `E`
     - `F`
     - `G`
  3. 这轮依然只是文档/冻结层完成，不是 runtime 修复，不宣称体验过线。
- 审计层：
  1. 需追加本轮新的 `skill-trigger-log`
  2. 本轮未改运行时代码，未跑 compile / tests / live

## 2026-04-17｜只读专项：opening staged contract 分裂与二次摆位风险补记
- 当前主线目标：
  - 不改代码，只读回答 opening 入口为什么仍分裂，以及最小统一点在哪。
- 本轮子任务：
  1. 精查 `SpringDay1Director.cs` 的：
     - `TryHandleTownEnterVillageFlow()`
     - `TryPrepareTownVillageGateActors()`
     - `ForcePlaceTownVillageGateActorsAtTargets()`
     - `ShouldReleaseEnterVillageCrowd()`
     - `ShouldLatchEnterVillageCrowdRelease()`
     - `ShouldUseEnterVillageHouseArrivalBeat()`
     - `ResetTownVillageGateDialogueSettlementState()`
  2. 精查 `SpringDay1NpcCrowdDirector.cs` 的：
     - `ShouldDeferToStoryEscortDirector(...)`
     - `ShouldIncludeThirdResidentInResidentRuntime(...)`
     - `ShouldSuppressEnterVillageCrowdCueForTownHouseLead(...)`
     - `ApplyResidentBaseline(...)`
     - `TryReleaseResidentToDaytimeBaseline(...)`
- 只读结论：
  1. `001/002` 的单独链是代码硬约束：
     - crowd director 在 `ShouldDeferToStoryEscortDirector(...)` 里明确把它们留给导演 owner。
  2. `003` 在 opening 也没真正并回 ordinary resident：
     - 它只在 `currentPhase > EnterVillage` 时才通过 `BuildSyntheticThirdResidentResidentEntry()` 合流 resident runtime；
     - opening 时仍跟 `001/002` 一样靠导演层 `TryPrepareTownVillageGateActors()` / `ForcePlaceTownVillageGateActorsAtTargets()` 摆位。
  3. ordinary resident 的 release 现在和 `001~003` 的 staged move 不是同一份合同：
     - crowd 侧用 `ShouldReleaseEnterVillageCrowd()` / `ShouldLatchEnterVillageCrowdRelease()` 提前放手并压掉 `EnterVillagePostEntry` cue；
     - 然后 `ApplyResidentBaseline(...) -> TryReleaseResidentToDaytimeBaseline(...)` 把 ordinary resident 送回 baseline/autonomy。
  4. current beat 会在 `EnterVillage` 相位内提早从 `EnterVillagePostEntry` 切到 `EnterVillageHouseArrival`：
     - 触发条件不仅有 `HouseArrivalSequenceId`
     - 还包括 `HasVillageGateCompleted()`、`_townHouseLeadStarted`、`_townHouseLeadTransitionQueued`
     - 这会把 crowd release 和 director escort 在同一相位中提前拆开。
  5. 最容易制造“先到终点又回起点 / 或二次摆位”的点，是导演层 reset 与多源 route 并存：
     - `ResetTownVillageGateDialogueSettlementState()` 同时清 cue wait 与 `_townVillageGateActorsPlaced`
     - `TryPrepareTownVillageGateActors()` 首次会把 actor 重新放 start
     - `MaintainTownVillageGateActorsWhileDialogueActive()` / `ForcePlaceTownVillageGateActorsAtTargets()` 又会反复往 end snap
     - route source 还混用了 layout points / opening markers / stagebook cue / hard fallback 四套来源
- 最小修法建议：
  1. opening route source 统一到一份 staged contract，优先让 `EnterVillagePostEntry` 的 `StageBook` cue 成为唯一真值。
  2. `ShouldReleaseEnterVillageCrowd()` / `ShouldLatchEnterVillageCrowdRelease()` 不再被 `_townHouseLeadStarted / _townHouseLeadTransitionQueued / _townHouseLeadWaitingForPlayer` 提前触发。
  3. `ShouldUseEnterVillageHouseArrivalBeat()` 不再在 house-lead 刚起步时提早切 beat，只在真正进入 `HouseArrival` 承接时切。
  4. `001~003` 的 opening actor 集合只在一处定义，不再由 `ShouldDeferToStoryEscortDirector(...)` 与 `ShouldIncludeThirdResidentInResidentRuntime(...)` 分散表达。
- 验证状态：
  - 纯静态审计，未改代码，未跑 compile/live。
- 当前恢复点：
  1. 如果继续真实施工，最值钱的第一刀应落在 `Package B`：
     - 先统一 opening staged contract
     - 再删 ordinary resident baseline release 主路径
  2. 先不要把这轮结论误扩成“导航层就是第一责任人”。

## 2026-04-17｜只读专项：Primary 进场后时间像停了的 Day1 own 触发链审计
- 当前主线目标：
  - 不改代码，只读回答 `Primary` 进场后为什么会体感“时间像停了”，并把 `Day1 own` 与外部可能拆开。
- 本轮子任务：
  1. 核对 `SpringDay1Director.ShouldPauseStoryTimeForCurrentPhase()`、`EnforceDay1TimeGuardrails()`、`GetManagedMaximumTotalMinutesForPhase()` 是否足以单独造成停表体感。
  2. 只查 `SpringDay1Director / StoryManager / DialogueManager / TimeManager` 内的暂停链、场景切换链、validation/debug 影响点。
- 只读结论：
  1. `ShouldPauseStoryTimeForCurrentPhase()` 足以单独让 `TimeManager` 真正停走：
     - `SyncStoryTimePauseState()` 会直接对 `TimeManager.PauseTime("SpringDay1Director")` / `ResumeTime(...)`。
     - `TimeManager.Update()` 在 `isPaused` 时直接 `return`。
  2. `EnforceDay1TimeGuardrails()` + `GetManagedMaximumTotalMinutesForPhase()` 本身不会调用 `PauseTime`，但会在导演 `Update()` 和 `sceneLoaded/hourChanged` 链里持续 `SetTime(...)` 回上限：
     - 它们足以单独制造“时钟卡在某个时刻不再前进”的体感
     - 但这是“被夹住”，不是 pause source 栈意义上的真暂停。
  3. `DialogueManager` 是另一条独立暂停源：
     - `PlayDialogue()` -> `PauseTime("Dialogue")`
     - `StopDialogueInternal()` -> `ResumeTime("Dialogue")`
     - 所以对白结束后若时间仍停，更像 `SpringDay1Director` 还在栈里，而不是 `Dialogue` 漏恢复。
  4. 进入 `Primary` 后最可疑的 `Day1 own` 主链是：
     - `StoryManager.CurrentPhase == EnterVillage`
     - `TickPrimarySceneFlow()` 每轮调用 `SyncStoryTimePauseState()`
     - `ShouldPauseStoryTimeForCurrentPhase(EnterVillage)` 在 `Primary` 下通常返回 `true`
     - 因为 `ShouldKeepStoryTimeRunningForRuntimeBridge(EnterVillage)` 只看 `IsTownHouseLeadPending()`，而它要求当前场景必须是 `Town`
     - 结果是：只要 `EnterVillage` 还没被 `HouseArrivalSequenceId` 收束并推进到 `HealingAndHP`，导演会持续把 `SpringDay1Director` 这条 pause source 留在 `TimeManager`。
  5. 这条链的释放条件也很明确：
     - `TryQueuePrimaryHouseArrival()` 成功接起 `HouseArrivalSequenceId`
     - `HandleDialogueSequenceCompleted(HouseArrivalSequenceId)` -> `BeginHealingAndHp()`
     - `BeginHealingAndHp()` -> `SetPhase(HealingAndHP)` -> `SyncStoryTimePauseState()`
     - `HealingAndHP` 在 `ShouldPauseStoryTimeForCurrentPhase()` 里明确返回 `false`
  6. 因此真正高风险点不在 `DialoguePauseSource`，而在“`Primary` 承接对白有没有顺利排上并完成”：
     - `TryQueuePrimaryHouseArrival()` 受 `HasVillageGateProgressed()`、`TryPreparePrimaryArrivalActors()`、`_primaryArrivalDialogueReadyAt`、`DialogueManager.IsDialogueActive` 共同约束
     - 任何一个条件长期不满足，`EnterVillage` 就会一直留住导演暂停源。
  7. 第二个明确的 `Day1 own` 停时链是 `WorkbenchFlashback`：
     - `ShouldPauseStoryTimeForCurrentPhase()` 对 `WorkbenchFlashback` 只在 `_workbenchOpened` 前保持暂停
     - 如果进了 `Primary` 但工作台未真正打开，时间会继续被导演按住。
  8. `debugSkipDirectToWorkbenchPhase05` 不是更可疑的停表源：
     - 它直接把状态推进到 `FarmingTutorial`，反而倾向于绕开前面的暂停桥。
  9. validation 相关的 `_validationBorrowedRunInBackground` 只借用 `runInBackground`，没有看到它给 `TimeManager` 加 pause source。
- Day1 own 概率排序：
  1. `EnterVillage -> Primary` 承接对白未接起/未完成，导致 `SpringDay1Director` 长期保留暂停源。
  2. 已进入 `WorkbenchFlashback`，但 `_workbenchOpened` 一直没成立，导演继续暂停时间等待工作台交互。
  3. 更晚阶段的 `DinnerConflict` 回 Town escort 在 `Primary` 中未收束，导演继续暂停时间等待返村承接。
- 外部可能但不算 Day1 own：
  1. `DialoguePauseSource` 本身只在对白活跃期间成立，不应被当成“Primary 后长期停表”的首要锅。
  2. `EnforceDay1TimeGuardrails()` 的时钟上限夹持会制造“表不再动”的观感，但它不是 pause source 栈本身。
  3. validation `runInBackground`、debug skip 都未看到能单独长期 pause `TimeManager` 的链。
- 最小安全切口建议（不写代码）：
  1. 先对 `EnterVillage -> HouseArrival -> HealingAndHP` 做一刀最小诊断，把“为什么 `Primary` 里没有从 `EnterVillage` 及时推进出去”查清，而不是先改 `TimeManager`。
  2. 如果要收最小行为切口，优先让 `ShouldPauseStoryTimeForCurrentPhase(EnterVillage)` 与 `Primary` 承接状态解耦，避免“对白没接起就整段真停时”。
  3. `WorkbenchFlashback` 保留导演控时可以，但要把“未打开工作台”和“对白/摆位未就绪”拆开报因，避免所有 Primary 停时都被看成同一种故障。
- 验证状态：
  - 纯静态审计，未改代码，未跑 compile/live。
- 当前恢复点：
  1. 如果继续真实施工，最值钱的第一刀应先落 `EnterVillage -> HouseArrival` 接续链。
  2. 不要先把锅甩给 `DialogueManager` 或全局 `TimeManager`；当前更像 Day1 导演相位桥接没有及时放时。

## 2026-04-17｜只读窄审：20:00 后 return-home 去 Tick 化可行性
- 当前主线目标：
  - 不改代码，只读判断 `20:00` 后 resident return-home 能否从 `TickResidentReturnHome()` 的持续 drive/retry/finish，收成“边缘下发一次或少量指令，然后不再持续代管 arrival/finish”。
- 本轮子任务：
  1. 只盘 `SpringDay1NpcCrowdDirector.cs` 与 `NPCAutoRoamController.cs`。
  2. 只回答依赖链、最小切口、直接删 tick 的炸点、以及最值得先补的一个接口/信号。
- 只读结论：
  1. 现状已经不是 transform 硬推主路径：
     - `CrowdDirector.TryDriveResidentReturnHome(...)` 现在走 `NPCAutoRoamController.RequestReturnToAnchor(...) -> RequestReturnHome(...) -> DriveResidentScriptedMoveTo(..., FormalNavigation, ...)`。
  2. 但 arrival/finish 责任仍然在 `CrowdDirector` 外层：
     - `TryBeginResidentReturnHome()` 只负责置 `IsReturningHome` 和首次尝试；
     - `TickResidentReturnHome()` 继续承担到家阈值判断、active-drive 轮询、retry 节奏和最终 `FinishResidentReturnHome()`。
  3. `NPCAutoRoamController` 在 `FormalNavigation` 到达后，目前只在内部 `EndDebugMove(reachedDestination:true)` 记录
     - `lastFormalNavigationArrivalTime`
     - `lastFormalNavigationArrivalPosition`
     - 然后 `HaltResidentScriptedMovement()`
     - 不会主动 release scripted owner，也没有对外 completion/arrival 信号。
  4. 因此以当前合同看，不能直接删 `TickResidentReturnHome()`：
     - 首次请求失败后会丢 retry；
     - 到家后不会再 `FinishResidentReturnHome()`；
     - `state.IsReturningHome` 和 owner release 会卡住；
     - `ApplyResidentBaseline()` 会继续因 `IsReturningHome` 早退，白天/剧情 release 终态不干净。
- 最小安全切口：
  1. 时序边缘仍由 `CrowdDirector` 在 `20:00` 统一发起。
  2. 先补一个 `FormalNavigation` 完成可观测信号，再把 finish 从 tick 迁成边缘收尾。
  3. 也就是说，真正该先补的不是新的 `Request...` 口，而是“导航已经完成这次 return-home，外层现在可以 release owner”的单一 completion 信号。
- 当前最值钱的接口/信号建议：
  1. 最值得先补的是按 `ownerKey + FormalNavigation` 暴露的一次性 completion/arrival consume 信号。
  2. 只要这个信号成立，`CrowdDirector` 才有机会从“每帧代管 finish”收成“边缘收尾”。
- 验证状态：
  - 纯静态审计，未改代码，未跑 compile/live。
  - 本轮只读，未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`。
- 当前恢复点：
  1. 如果继续真实施工，先补导航 completion signal，再考虑砍 `TickResidentReturnHome()`。
  2. 在 completion signal 出现前，不要把“已经改成 `RequestReturnToAnchor(...)`”误判成“arrival/finish 已经是导航自管”。

## 2026-04-17｜真实施工 checkpoint：`Package E` 第一刀完成，`E-08` blocker 已写死
- 当前主线目标：
  - 继续按 `0417.md` 推进 Day1，先收全 NPC 夜间统一合同第一刀，再把不能继续硬砍的 blocker 说清。
- 本轮实际做成了什么：
  1. `SpringDay1NpcCrowdDirector`
     - `FreeTime / DayEnd` 会把 `001/002/003` 一起纳回 resident runtime；
     - `21:00` 已补统一 `snap + hide`；
     - `20:00/21:00` 的夜间合同不再 split 成 `001/002` director 私链 + `003~203` crowd 私链。
  2. `SpringDay1Director`
     - `SyncStoryActorNightRestSchedule()` 已退成 release，不再持续夜管 `001/002`。
  3. `HandleSleep()`
     - 已补 `SpringDay1NpcCrowdDirector.ClearResidentRuntimeSnapshots()`，防止 Day1 夜间隐藏状态带进次日 snapshot。
  4. 测试
     - 新增 / 改写夜间统一合同相关定向 tests；
     - 夜间相关 `8` 条 EditMode tests 现已 `passed=8 failed=0`。
  5. `0417.md`
     - 已新增 `C-15 / C-16 / C-17`
     - `Package E` 改成 `进行中`
     - `E-01 / E-02 / E-04 / E-05 / E-06` 已打勾
- 本轮关键判断：
  1. `E-08` 不能继续硬砍。
  2. 真实 blocker 已钉死：
     - `NPCAutoRoamController` 还没有把 `FormalNavigation` completion/arrival 对外暴露给 `CrowdDirector`
     - 所以现在不能安全删 `TickResidentReturnHome()` 的 arrival/finish 职责
  3. 我中途试探过一小刀去 tick 化，但在 explorer 只读结论回来后已经全部撤回，没有把 shared root 留在半退权状态。
- 验证：
  1. `validate_script`：
     - `SpringDay1NpcCrowdDirector.cs` `errors=0`
     - `SpringDay1Director.cs` `errors=0`
     - `SpringDay1DirectorStagingTests.cs` `errors=0`
     - `SpringDay1LateDayRuntimeTests.cs` `errors=0`
  2. EditMode tests：
     - `CrowdDirector_RuntimeEntries_ShouldIncludeThirdResidentAfterOpening`
     - `CrowdDirector_RuntimeEntries_ShouldIncludeStoryActorsDuringFreeTimeNightContract`
     - `CrowdDirector_ShouldNotKeepNightRestResidentsUnderCrowdScriptedOwner`
     - `CrowdDirector_ShouldQueueReturnHomeAtTwentyDuringFreeTime`
     - `CrowdDirector_ShouldNotTreatTwentyOneAsReturnHomeContractWindow`
     - `CrowdDirector_ClockSchedule_ShouldStartReturnAtTwentyAndRestAtTwentyOne`
     - `StoryActorsNightRestSchedule_ShouldStandDownForUnifiedResidentNightContract`
     - `HandleHourChanged_FreeTimeAtTwoAmShouldFinalizeDayEnd`
     - 结果：`8/8 passed`
  3. `git diff --check`：
     - 当前 own 文件通过
  4. fresh console：
     - 无本轮新红；
     - 仅有外部 `Screen position out of view frustum` 旧噪声。
- 当前未完成：
  1. `E-03` crowd 自己的 night runtime 仍未彻底去私房化。
  2. `E-07` 次日激活目前只有 `clear snapshot` 结构保护，live 仍待证。
  3. `E-08` 仍 blocked 于缺 `FormalNavigation completion signal`。
- 当前恢复点：
  1. 下次继续不要回掰 dinner；
  2. 直接从 `FormalNavigation completion signal` 设计/落地开始；
  3. 信号补完后，再回来收 `TickResidentReturnHome()` 的边缘收尾化。

## 2026-04-17｜继续施工：晚饭 phase-entry 起步、night home-anchor 真值、bridge native snapshot 清理已补
- 本轮主线：
  - 沿 `0417.md` 继续收 `18:00 / 20:00 / 21:00 / 次日` 这一整条链，不把结构成立冒充成体验过线。
- 本轮新增落地：
  1. `ActivateDinnerGatheringOnTownScene()` 已补保护测试，要求 `001/002` 在 `18:00` 入口第一拍就到 authored 起点。
  2. `FindSceneResidentHomeAnchor(...)` 已修掉 `001/002/003` 把 NPC 本体误绑为 `HomeAnchor` 的问题。
  3. `PersistentPlayerSceneBridge` 已新增 `ClearNativeResidentRuntimeSnapshots(...)`；
     - `HandleSleep()` 现在会清 `001/002/003` 的 bridge native snapshots，避免次日/跨场景把昨晚位置带回来。
- 本轮新增测试：
  - `ActivateDinnerGatheringOnTownScene_ShouldPlaceStoryActorsAtAuthoredStartsOnPhaseEntry`
  - `CrowdDirector_ShouldBindUnifiedNightRuntimeStoryActorsToOwnHomeAnchors`
  - `PersistentPlayerSceneBridge_ClearNativeResidentRuntimeSnapshots_ShouldDropDay1SyntheticActors`
- 当前回归结果：
  - 关键窄回归 `7/7 passed`
  - fresh console `0 error / 0 warning`
  - 当前 own 文件 `git diff --check` 通过
- 当前判断更新：
  1. `E-08` 不再是“缺 completion signal”；signal 已有，剩的是 `TickResidentReturnHome / retry / finish` 还没完全退薄。
  2. `E-07` 也不再只有 crowd snapshot 清理；现在已经连 bridge native snapshot 回流也一起堵上了，但 live 仍待证。
- 当前恢复点：
  1. 如果继续真实施工，优先评估 `E-08` 是否还能安全再去私房化一刀；
  2. 如果先等用户 fresh live，重点看 `18:00 / 20:00 / 21:00 / 次日` 四个节点。

## 2026-04-17｜只读审计：Package F-05/F-06 editor/bootstrap/validation 旧真值残依赖
- 本轮主线：
  - 按 `0417.md` 只读审 `Package F-05/F-06`，找出 editor/bootstrap/validation 里仍把旧 semantic anchor / 旧 beat / 旧中间层当 runtime truth 的位置，并给出最小安全修改方向。
- 本轮子任务：
  - 只读核查以下 6 个文件：
    - `Assets/Editor/Story/SpringDay1DirectorTownContractMenu.cs`
    - `Assets/Editor/Town/TownSceneRuntimeAnchorReadinessMenu.cs`
    - `Assets/Editor/Town/TownNativeResidentMigrationMenu.cs`
    - `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
    - `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
    - `Assets/Editor/NPC/NpcResidentDirectorBridgeValidationMenu.cs`
- 关键审计结论：
  1. `SpringDay1DirectorTownContractMenu` 仍把 `EnterVillageCrowdRoot / NightWitness_01 / DinnerBackgroundRoot / DailyStand_01` 这组旧 anchor 当“导演关键锚点”与 pass/block 判据；其中 `NightWitness_01` 已被 `0417.md` 明确列为 F 包应退场对象，不应继续充当 director runtime truth。
  2. `TownSceneRuntimeAnchorReadinessMenu` 仍把旧 anchor 名、`DirectorReady_* / ResidentSlot_* / BackstageSlot_*` 命名体系以及 manifest `SupportsSemanticAnchor(...)` 命中，当成“更深 runtime readiness”真值；这里的 `consumerNpcIds == 0` 目前也更适合记成 attention/warning，不应再暗示缺消费者就等于 runtime 未就绪。
  3. `TownNativeResidentMigrationMenu` 仍深绑旧中间层：
     - root：`Town_Day1Residents / Resident_DefaultPresent / Resident_DirectorTakeoverReady / Resident_BackstagePresent`
     - anchor 映射：`SemanticHomeAnchorNames`
     - 分流依据：`entry.residentBaseline`
     - fallback：semantic anchor 不存在时直接回退到旧 semantic anchor Transform
     这会把旧中间层重新写回 scene 结构与初始落位。
  4. `SpringDay1NpcCrowdBootstrap` 仍在生成 manifest 时写死旧 semantic anchors（`EnterVillageCrowdRoot / NightWitness_01 / DinnerBackgroundRoot / DailyStand_*`）和旧 resident beats（`FreeTime_NightWitness / DailyStand_Preview`）；这不是“兼容旧数据”，而是在继续供血旧 data truth。
  5. `SpringDay1NpcCrowdValidationMenu` 仍把上述旧 anchor / 旧 beat / 旧 scene duty / 旧 phase fallback 直接写成硬断言；其中整簇 `FreeTime_NightWitness` / `DailyStand_Preview` 相关校验，会在 F 包改语义后反向阻止新合同落地。
  6. `NpcResidentDirectorBridgeValidationMenu` 运行的测试名仍直接绑 `EnterVillageAndNightWitness`、`ManifestSemanticAnchorsAndDuties_ForResidentBridgeBeats`；菜单本身就是旧桥接合同的执行入口。
- 最小安全修改方向（本轮未改代码）：
  1. 删除：
     - 把 `NightWitness_01`、`FreeTime_NightWitness`、`DailyStand_Preview` 继续作为 runtime/validation 真值的菜单常量、bootstrap 生成项与 validation 硬断言。
     - `TownNativeResidentMigrationMenu` 中按 `residentBaseline -> old root` 的 root 分流和“找不到 HomeAnchor 就回旧 semantic anchor Transform”的 fallback。
  2. 改名/降级：
     - `DirectorReady_* / ResidentSlot_* / BackstageSlot_*` 如仍保留，只能改成 scene authoring / staging probe 名称，不能再叫 runtime readiness / home truth。
     - `SpringDay1DirectorTownContractMenu` 的 `TargetAnchors` 若暂不删，应改成“authoring probes / deprecated anchor probes”口径，而不是导演 contract pass 条件。
  3. 改成忽略/告警而非 runtime truth：
     - `consumerNpcIds == 0`
     - 旧 slot 距离/旧 secondary slot 缺失
     - 旧语义 anchor 仍存在但无人消费
     这些最多报 warning，用来清残留，不应用来判 runtime blocked。
- 验证/文件变更：
  - 本轮纯只读，无代码改动、无 `Begin-Slice`、无 Unity live 写。
- 当前恢复点：
  1. 若下一轮进入真实施工，优先先收 `F-05` 菜单/validation 常量降级，再收 `F-06` bootstrap/migration 生成链，避免旧数据继续回灌。
  2. 不要再建议恢复任何旧 `NightWitness / DailyStand / semantic-anchor-root` 行为，只允许删、降级或改成 warning。

## 2026-04-17｜收口补记：`TownNativeResidentMigrationMenu.cs` 的 4 条 `CS0133` 已清
- 用户目标：
  - 先确认 `Assets/Editor/Town/TownNativeResidentMigrationMenu.cs(20~23)` 的 `CS0133` 是否仍在，并把这组 compile 红收干净。
- 已完成事项：
  1. 确认磁盘文件中 4 个常量当前均为编译期字面量：
     - `TownResidentRootName = ""`
     - `TownResidentDefaultRootName = ""`
     - `TownResidentTakeoverRootName = ""`
     - `TownResidentBackstageRootName = ""`
  2. `validate_script Assets/Editor/Town/TownNativeResidentMigrationMenu.cs`
     - native validate = `clean`
     - compile-first assessment = `unity_validation_pending`
     - 原因是 Unity `stale_status`，不是脚本自身仍有 owned red。
  3. `status` 与 `errors` fresh 复读后：
     - bridge/baseline 正常
     - fresh console = `0 error / 0 warning`
  4. `git diff --check -- Assets/Editor/Town/TownNativeResidentMigrationMenu.cs` 通过。
- 关键决策：
  1. 这轮只收 compile blocker，不扩 runtime。
  2. `TownNativeResidentMigrationMenu` 继续按 deprecated blocker 看待，不恢复旧 Town resident migration 语义。
- 影响文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Town\TownNativeResidentMigrationMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\100-重新开始\0417.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\100-重新开始\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory.md`
- 验证结果：
  - 用户刚报的 4 条 `CS0133` 已消失；
  - 当前没有我 own 的 fresh console 红。
- 遗留问题 / 下一步：
  1. 回到 `0417 / Package F-05 ~ F-07`，继续清 editor/tests 里把旧中间层当真值的残依赖。
  2. 这条 compile 红已清，不代表 Day1 runtime 体验已过线。

## 2026-04-17｜进度盘点：0417 还没做完，5 个子智能体均已收回
- 用户目标：
  - 让我给出 `0417` 的剩余量、5 个子智能体是否做完、以及当前是否可以打包。
- 已完成事项：
  1. 读取 `0417.md` 并统计 package 级进度：
     - `TOTAL 41 done / 27 todo`
     - `A = 6/6`
     - `B = 4 done / 5 todo`
     - `C = 4 done / 4 todo`
     - `D = 8 done / 2 todo`
     - `E = 8 done / 1 todo`
     - `F = 4 done / 5 todo`
     - `G = 0 done / 7 todo`
  2. 收回并关闭 5 个子智能体：
     - `Averroes`
     - `Halley`
     - `Russell`
     - `Newton`
     - `Meitner`
  3. 归并后的可用结论：
     - `B` 仍有 opening / dinner owner split
     - `D` 晚饭统一合同未真正收平
     - `E-08` 仍有一层 owner 读取残留
     - `F-05/F-06/F-07` 仍有 editor/bootstrap/tests 旧真值残依赖
  4. fresh 现场核查：
     - `sunset_mcp.py errors` = `0 error / 0 warning`
     - `sunset_mcp.py status` 显示 Unity 当前 `is_playing=true`、`playmode_transition/stale_status`
     - `git status --short` 显示 shared root 仍是大面积 mixed dirty，且还有别的线程 `ACTIVE`
- 关键决策：
  1. 5 个子智能体都“做完”了它们各自的只读审计任务，但没有哪一个完成了新的 runtime 落地。
  2. 现在不能判定 Day1 可以打包。
- 验证结果：
  - 当前没有 fresh console 红错；
  - 但这不等于 packaged-ready。
- 当前判断：
  1. `0417` 还明显没完。
  2. 真正压后的剩余主量在：
     - `Package B`
     - `Package C`
     - `Package F`
     - `Package G`
  3. `Package G` 完全没开始，所以“可打包”这件事当前没有证据支撑。
- 下一步：
  1. 若继续施工，仍按 `0417` 先收未完 package；
  2. 若要进入打包判断，必须先补 `G-01 ~ G-05` 的 fresh 证据链。

## 2026-04-17｜最小打包清单重算：当前只剩夜间尾刀
- 用户目标：
  - 用户纠正“前面很多内容其实已经差不多做好”，要求重新检查现状并重做最小待完成打包前清单。
- 本轮实际核查：
  1. `SpringDay1NpcCrowdDirector` 当前已经有：
     - `20:00` return-home schedule
     - `RequestReturnToAnchor(...)` 导航回 anchor
     - `21:00 SnapToTarget + SetActive(false)`
     - 次日 `RestoreResidentFromNightRestState(...)` 从 `HomeAnchor` 激活
  2. `SpringDay1Director / SpringDay1NpcCrowdDirector` 里 morning release 当前都是 `9`。
- 关键结论：
  1. 夜间不是从零缺系统，而是已有骨架需要打包前热修。
  2. 最小清单不再包含 opening / primary / dinner 主修，只做必要回归。
- 当前最小待完成清单：
  1. 单点修 `101` 在 `20:00` 不走回 anchor 的问题。
  2. 把 `ResidentMorningReleaseHour` 从 `9` 改到 `7`。
  3. 把 `StoryActorMorningReleaseHour` 从 `9` 改到 `7`。
  4. 校准“20:00 后到 anchor 附近即隐藏，视为进屋睡觉”。
  5. 保持 `21:00` 未到家强制 snap 到 anchor 后隐藏。
  6. 次日从 anchor 激活，触发时间按 `7:00`。
  7. 最小回归只验 `20:00 / 21:00 / 次日7:00 / packaged path`，opening/primary/dinner 只做防回归抽检。
- 当前恢复点：
  - 下一轮真实施工只按这张夜间尾单开刀，不要重新扩大到 `0417` 全量清扫。

## 2026-04-17｜夜间尾刀续收：anchor 吃场景真值，`101` 不写特判
- 当前主线目标：
  - 继续按夜间打包尾单收口，而不是重开 `0417` 全量大修。
- 用户这轮新增语义：
  1. `101` 只是当前 packaged 里测到的个例，不允许我只围着 `101` 打补丁。
  2. 如果用户重新调整场景里的 `HomeAnchor`，代码应直接吃场景物体位置，不应硬编码坐标。
  3. 虽然用户觉得其他问题不大，我仍要全面检查自己 own 的夜间链。
- 本轮实际完成：
  1. 重新 `Begin-Slice`：
     - `0417-packaging-final-night-anchor-and-regression-close`
  2. 静态核实 `Town.unity`：
     - 已确认存在 `101_HomeAnchor`
     - 也存在 `001/002/003/102/103/104/201/202/203_HomeAnchor`
  3. 静态核实 `SpringDay1NpcCrowdDirector.FindSceneResidentHomeAnchor(...)`：
     - 会按 `npcId` / `NPCxxx` 解析 `xxx_HomeAnchor`
     - `TryBeginResidentReturnHome(...)` / `FinishResidentReturnHome(...)` 使用的是 `state.HomeAnchor.transform.position`
     - 因此用户移动场景 anchor 位置时，不需要改代码坐标
  4. 更新 `0417.md`：
     - 新增 `C-23`
     - 更新 `G-09 / I-09 / I-10 / Package E`
     - 明确 `101` 不写特判，只按场景 anchor 真值和 runtime probe 继续查
  5. Unity 验证：
     - 先 `manage_editor stop` 退回 Edit Mode
     - 夜间尾单定向 EditMode tests：`8/8 passed`
     - 清 console 后 fresh `errors`：`0 error / 0 warning`
     - `git diff --check` 覆盖本轮 own 文件通过
- 当前判断：
  1. 夜间尾刀这轮没有新的 own 红。
  2. `HomeAnchor` 这条线当前站住的是“场景对象真值”，不是硬编码。
  3. `101` 如果后续仍异常，最合理的下一刀是 runtime probe，看它实际绑定了谁、有没有成功发起 return-home，而不是写 `101` 分支。
  4. 这轮仍不能 claim “packaged 已过线”，因为用户还没给新的 packaged/live 复验。
- 恢复点：
  1. 用户可以先调场景里的 `101_HomeAnchor` 位置，名字保持不变即可。
  2. 下一轮优先只复测：
     - `20:00`
     - `21:00`
     - 次日 `7:00`
     - `101`
  3. 若 `101` 仍异常，再抓 runtime probe：
     - `HomeAnchor`
     - 距 anchor 判定
     - `RequestReturnToAnchor` 返回
     - 导航/active state 是否中断
  4. 本轮收尾需执行 `Park-Slice`。

## 2026-04-17｜停车补记：本轮已 Park-Slice
- `thread-state`：
  - `Begin-Slice`：已跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
  - blocker：`await-user-anchor-adjust-and-packaged-night-retest`
- 当前恢复点：
  1. 等用户先调场景里的 `101_HomeAnchor`。
  2. 然后只做 packaged/live 复测：
     - `20:00`
     - `21:00`
     - 次日 `7:00`
     - `101`
  3. 若仍异常，下一刀只抓 runtime probe，不扩回全 Day1 大修。

## 2026-04-17｜当前线程续记：`002` 等待补位与艾拉圆形回血最小尾刀
- 当前主线目标：
  - Day1-V3 继续按 `0417.md` 收打包前必要尾刀，保持最小、安全、不扩题。
- 本轮子任务：
  1. 修 `TownHouseLead` 中 `001` 停等玩家时 `002` 也停死的问题。
  2. 修艾拉疗伤不是圆范围触发、像是需要面向艾拉的问题。
  3. 若安全，在疗伤触发后让艾拉朝向玩家。
- 已完成事项：
  1. `SpringDay1Director.TryBeginTownHouseLead()` 已改成 `001` 停等玩家、`002` 继续追基于 `001` 当前位置的动态编队位。
  2. 新增 `TryDriveEscortCompanionTowardLeaderFormation(...)`。
  3. `SpringDay1Director.TryHandleHealingBridge()` 已改为玩家本体位置圆距离判断，半径保持 `1.6`。
  4. 新增 `TryGetPlayerHealingProximityPoint(...)`、`GetHealingProximityPoint(...)`、`FaceHealingSupportTowardPlayer(...)`。
  5. `0417.md` 已补 `C-24/C-25` 与相关问题、测试、tasks 条目。
- 验证结果：
  1. 定向 EditMode tests `4/4 passed`。
  2. fresh console `0 error / 0 warning`。
  3. `git diff --check` 覆盖本轮 own 文件通过。
- 遗留问题 / 下一步：
  1. 需要用户 live / packaged 复测 `T-04`：艾拉周围圆范围内可触发回血，且艾拉朝向玩家。
  2. 需要用户 live / packaged 复测 `T-13`：`001` 停等玩家时 `002` 继续补位。
  3. 不要把这两刀扩成 NPC `E` 交互、workbench escort、night contract 或 opening owner 大修。

## 2026-04-17｜线程状态补记：本轮最小尾刀已重新 Park
- `thread-state`：
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
  - blocker：`await-live-retest-healing-circle-and-town-house-lead-companion-follow`
- 当前恢复点：
  1. 用户优先验 `T-04` 与 `T-13`。
  2. 若失败，只回头抓这两条语义，不扩回其它 Day1 包。

## 2026-04-17｜当前线程只读总结：0417 的真实完成度与剩余面
- 本轮性质：
  - 只读回看 `0417.md`、子工作区 memory 与线程 memory，重新整理“当前到底做完了什么、还剩什么”。
- 结论：
  1. `Day1-V3` 当前不是“还在靠猜”，而是已经有明确 Package A~G 主控板。
  2. 当前打包视角的最小 blocker 已缩到用户 live / packaged 复测，不再是继续大面积代码扩写。
  3. 架构长期债仍在：
     - opening 双 owner
     - `003` 残留 special handoff
     - dinner 全员单一 owner 未完全落平
     - Package F 清扫未完
     - Package G 终验未开始
  4. 当前最现实的顺序是先拿 live 票，再决定是进入打包验收还是继续清架构债。

## 2026-04-17｜当前线程续记：`003` opening 私链退权与 opening 桥接回归
- 当前主线目标：
  - 继续以 `0417.md` 为唯一主控板，按代码真相清掉 Day1 里仍明显不符合用户语义的残留 owner。
- 本轮子任务：
  1. 把 `003` 从 opening `SpringDay1Director` story actor 私链里退出来。
  2. 确认 `003` 在 `EnterVillage` 阶段就走 ordinary resident runtime。
  3. 补齐 opening bridge 定向测试，让这一刀不是只靠静态判断。
- 已完成事项：
  1. `SpringDay1NpcCrowdDirector.ShouldIncludeThirdResidentInResidentRuntime(...)` 已改为 `currentPhase >= EnterVillage`。
  2. `SpringDay1Director` 不再在 opening 的 visibility/control、prepare、force-place 链里处理 `003`。
  3. `MaintainTownVillageGateActorsWhileDialogueActive()` 现在 active dialogue 时会先 `TryPrepareTownVillageGateActors(forceImmediate: true)`，失败再走 force-place 兜底。
  4. `SpringDay1OpeningRuntimeBridgeTests.InvokeInstance(...)` 已改为参数匹配型反射 helper，修掉 optional 参数导致的 `TargetParameterCountException`。
  5. `0417.md`、子工作区 memory、父工作区 memory 已回写。
- 验证结果：
  1. opening 定向 EditMode tests `6/6 passed`。
  2. direct `validate_script`：本轮 touched 脚本 / 测试均 `errors=0`。
  3. fresh console 清理后 `0 error / 0 warning`。
  4. `git diff --check` 覆盖 own 文件通过。
  5. CLI `validate_script` 给出 `unity_validation_pending owned_errors=0 external_errors=0`，阻塞来自 `stale_status / CodeGuard timeout-downgraded`，不按体验过线处理。
- 遗留问题 / 下一步：
  1. opening 仍是 `001/002` director 链 + ordinary resident crowd 链的双 owner，不是最终“全员同一 movement owner”。
  2. `003` runtime 主修已完成；后续不要再给它写专用 runtime。
  3. 若继续施工，优先选择 opening 单一 owner 或 Package F 旧真值清扫；若进入验收，必须走 packaged/live，不把 targeted tests 冒充体验结论。

## 2026-04-17｜当前线程续记：夜间“到家即隐藏”到站半径热修
- 当前主线目标：
  - 继续按 `0417.md` 收打包前必要尾刀，当前只锁 `20:00 到家即隐藏` 这条夜间语义。
- 本轮子任务：
  1. 修掉 resident 明明已经到 `HomeAnchor` 附近，却还站在门口不隐藏的漏口。
  2. 保证这条语义优先级高于 retry cooldown 和“没有 active FormalNavigation completion”的坏态。
- 已完成事项：
  1. `SpringDay1NpcCrowdDirector` 新增统一的 home arrival 半径 `0.35`。
  2. `TryBeginResidentReturnHome(...)` 现在先判“是否已在家附近”，再看 retry cooldown。
  3. `TickResidentReturnHome(...)` 现在在无 active FormalNavigation completion 时，也会按同一 arrival 半径直接 `FinishResidentReturnHome(...)`。
  4. `ShouldQueueResidentReturnHomeAfterCueRelease(...)` 与 `TryDriveResidentReturnHome(...)` 已改用同一到站半径口径。
  5. `SpringDay1DirectorStagingTests.cs` 新增两条保护测试：
     - `CrowdDirector_TryBeginResidentReturnHome_ShouldHideImmediatelyWhenWithinArrivalRadiusDuringRetryCooldown`
     - `CrowdDirector_TickResidentReturnHome_ShouldHideWhenResidentIsAtAnchorWithoutActiveNavigation`
  6. `0417.md`、子工作区 memory、父工作区 memory 已同步回写。
- 验证结果：
  1. direct `validate_script`：
     - `SpringDay1NpcCrowdDirector.cs`：`errors=0 warnings=2`
     - `SpringDay1DirectorStagingTests.cs`：`errors=0 warnings=0`
  2. fresh console：`0 error / 0 warning`
  3. `git diff --check` 覆盖本轮 own 文件通过
  4. CLI `validate_script` 仍是 `unity_validation_pending`，原因是 Unity compile ready 未闭环，不是 owned red
- 遗留问题 / 下一步：
  1. 这刀当前只站住 `结构 / targeted probe`，还需要用户 packaged/live 继续看 `20:00` 是否仍有人站在 anchor。
  2. 若后续仍异常，下一刀优先抓：
     - resident 是否纳入夜间合同
     - `HomeAnchor` 是否成功绑定
     - 是否真正进入 return-home / hide 链
  3. 不回流 `Town_Day1Residents / Town_Day1Carriers / EnterVillageCrowdRoot` 这类旧中间层。

## 2026-04-17｜线程状态补记：夜间到站半径热修已 Park
- `thread-state`：
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
  - reason：`night-return-arrival-radius-hotfix-waiting-packaged-retest`
- 当前恢复点：
  1. 用户优先验 `20:00` 到家即隐藏。
  2. 若仍有 NPC 到 anchor 后站着不消失，下一刀只查 runtime probe 与 home anchor 绑定，不扩回旧中间层。

## 2026-04-17｜当前线程续记：formal NPC 剧情外 resident informal 入口热修
- 当前主线目标：
  - 继续按 `0417.md` 收打包前必要尾刀，这轮只锁 `19:30 / 20:00` 剧情外聊天入口。
- 本轮子任务：
  1. 对位“为什么剧情外只剩少数 NPC 能聊”的 own 链。
  2. 修掉 formal NPC 在剧情外应让位给居民闲聊、却没有 informal 入口组件的漏口。
- 已完成事项：
  1. 继续核 `SpringDay1Director` 后确认：`FreeTime` 已经会重新启用 `001/002` 的 formal / informal 组件；问题不是简单的 `enabled=false` 没放开。
  2. `SpringDay1NpcCrowdDirector.ConfigureBoundNpc(...)` 已改成：
     - 不再因为 NPC 已经有 `NPCDialogueInteractable` 就跳过
     - 只要缺 `NPCInformalChatInteractable` 且 `RoamProfile` 有 resident informal content，就自动补 informal 入口
  3. `SpringDay1DirectorStagingTests.cs` 新增：
     - `CrowdDirector_ConfigureBoundNpc_ShouldAddInformalChatWhenFormalDialogueExists`
  4. `0417.md`、子工作区 memory、父工作区 memory 已同步回写。
- 验证结果：
  1. `git diff --check` 覆盖 own 代码文件通过
  2. 当前 Unity MCP 基线不稳：
     - `doctor` 先报 `listener_missing / pidfile_missing`
     - `recover-bridge` 后仍是 `No active Unity instance`
     - 所以 `validate_script` 只能报 `unity_validation_pending / baseline_fail`
  3. 当前这刀只能诚实落成：
     - `结构已改`
     - `静态判断成立`
     - `Unity 验证待补`
- 遗留问题 / 下一步：
  1. 下一轮 Unity 可用后，优先补这条 fix 的窄验证。
  2. 用户 packaged/live 重点复测：
     - `19:30` 后 ordinary resident 与 `001/002` 是否都恢复可聊
     - `20:00` 回家途中是否仍可聊天

## 2026-04-17｜线程状态补记：formal->informal 入口热修已 Park
- `thread-state`：
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
  - reason：`formal-to-informal-entry-hotfix-waiting-unity-validation-or-live-retest`
- 当前恢复点：
  1. Unity 一恢复，先补这条 hotfix 的窄验证。
  2. 若用户先做 packaged/live，重点只收 `19:30/20:00` 剧情外聊天入口是否正常。

## 2026-04-17｜当前线程续记：`SpringDay1DirectorStagingTests` compile blocker 清理
- 当前主线目标：
  - 继续按 `0417.md` 收打包前必要尾刀；本轮只是先把新增测试制造的 compile red 清掉，避免主线带红。
- 本轮子任务：
  1. 修掉 `SpringDay1DirectorStagingTests.cs` 里两处 `ResolveNestedTypeOrFail(...)` 不存在导致的 `CS0103`。
  2. 不扩 Day1 runtime，不改用户场景语义。
- 已完成事项：
  1. `CrowdDirector_ConfigureBoundNpc_ShouldAddInformalChatWhenFormalDialogueExists` 里的 nested type 解析已改为：
     - `ResolveTypeOrFail("NPCDialogueContentProfile+InformalChatExchange")`
     - `ResolveTypeOrFail("NPCDialogueContentProfile+InformalConversationBundle")`
  2. 测试文件磁盘内容已确认，不再引用不存在的 helper。
  3. `0417.md`、子工作区 memory、父工作区 memory 已同步回写。
- 验证结果：
  1. `git diff --check -- Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs` 通过。
  2. `validate_script` 对该文件给出 `owned_errors=0`。
  3. fresh `status` 已出现 `Build completed with a result of 'Succeeded'`。
  4. fresh `errors` 只剩外部 importer blocker：
     - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset`
- 遗留问题 / 下一步：
  1. 这条测试 helper 编译红已经不再阻塞 Day1 主线。
  2. 若下一轮要 claim no-red，必须先处理或隔离外部 importer 红。

## 2026-04-17｜当前线程续记：疗伤桥左侧 packaged 触发过窄已修为“艾拉真实中心”
- 当前主线目标：
  - 继续按 `0417.md` 收打包前必要尾刀；这轮只锁用户最新 packaged 反馈的疗伤桥左侧接近坏相。
- 本轮子任务：
  1. 查清为什么从 `002` 左侧接近时，必须几乎碰到才会触发回血。
  2. 保持半径 `1.6` 不变，只改疗伤判定圆心。
- 已完成事项：
  1. 对位代码后确认：之前玩家侧已改成 `GetHealingProximityPoint(playerTransform)`，但艾拉侧仍直接吃 `supportNpc.position`。
  2. `SpringDay1Director` 新增：
     - `GetHealingSupportSamplePoint(...)`
     - `TryGetCenterSamplePoint(...)`
  3. `TryHandleHealingBridge()` 与 `TrySnapValidationPlayerNearHealingSupport(...)` 已统一改成：
     - 玩家本体中心 -> 艾拉真实碰撞/展示中心
  4. `SpringDay1DirectorStagingTests.cs` 新增：
     - `HealingBridge_ShouldUseSupportCenterAsProximityAnchor`
  5. `0417.md`、子工作区 memory、父工作区 memory 已同步回写。
- 验证结果：
  1. `validate_script`：
     - `SpringDay1Director.cs`：`owned_errors=0`
     - `SpringDay1DirectorStagingTests.cs`：`owned_errors=0`
     - 当前 assessment 为 `unity_validation_pending`，原因是 Unity `stale_status`，不是 owned red
  2. fresh `errors`：`0 error / 0 warning`
  3. fresh `status`：baseline pass，console clean
  4. `git diff --check` 覆盖本轮 own 文件通过
- 遗留问题 / 下一步：
  1. 这刀当前只站住 `结构 / targeted probe`。
  2. 下一步最值钱的是用户重新打包复测 `002` 左侧接近疗伤桥；如果仍异常，再抓 scene 里 `002` 的实际 collider / renderer 布局，不回头继续盲调半径。

## 2026-04-17｜当前线程续记：夜间 `20:00/21:00` return-home/hide 合同硬化
- 当前主线目标：
  - 继续按 `0417.md` 收打包前必要尾刀；这轮只锁夜间 resident `return-home / hide` 的 own 漏口。
- 本轮子任务：
  1. 修掉 `20:00` 回家途中一旦丢了 active drive 就站到 `21:00` 的坏态。
  2. 修掉 forced day-end / `21:00` snap 链没把 hide 立起来的漏口。
  3. 给无 `HomeAnchor` 的个例补 `21:00` hide 兜底。
- 已完成事项：
  1. `TickResidentReturnHome(...)` 现在在无 active drive 且未到 anchor 时，会按短节流重试 `TryDriveResidentReturnHome(...)`；失败则释放 shared owner 并写 `return-home-pending`。
  2. `SyncResidentNightRestSchedule()` 在 `21:00` rest 窗口不再因 `HomeAnchor == null` 直接跳过；无 anchor 个例也会被隐藏。
  3. `SnapResidentsToHomeAnchorsInternal()` 现在先写 `HideWhileNightResting = true`，forced day-end snap 不再只回家不隐身。
  4. `SpringDay1DirectorStagingTests.cs` 已新增 / 改写：
     - `CrowdDirector_ShouldScheduleReturnHomeRetryWhenNoDriveIsActive`
     - `CrowdDirector_ShouldHideResidentsAtTwentyOneEvenWhenHomeAnchorIsMissing`
     - `CrowdDirector_SnapResidentsToHomeAnchors_ShouldHideResidentsForNightRest`
- 验证结果：
  1. `validate_script`：
     - `SpringDay1NpcCrowdDirector.cs`：`owned_errors=0`
     - `SpringDay1DirectorStagingTests.cs`：`owned_errors=0`
     - 当前 assessment 为 `unity_validation_pending`，原因仍是 Unity `stale_status`，不是 owned red
  2. fresh `errors`：`0 error / 0 warning`
  3. fresh `status`：baseline pass，console clean
  4. `git diff --check` 覆盖 own 文件通过
- 遗留问题 / 下一步：
  1. 这刀当前只站住 `结构 / targeted probe`，还没拿到新的 packaged 证据。
  2. 下一刀准备切到 `0.0.6` 旧 escort 入口，继续把 `Town anchor first` 语义对回代码真相。

## 2026-04-17｜当前线程只读续记：Day1 夜间 `20:00/21:00 resident return-home/hide` 语义彻查
- 当前主线目标：
  - 不改代码，只读回答 Day1 夜间 resident 当前到底怎么从 `20:00 return-home` 走到 `21:00 forced rest/hide`，并给出“为什么到 anchor 后还可能不立刻隐藏”“21:00 具体怎么兜底”“还有哪些入口/owner 仍会让 NPC 到家后留在场上”的精确方法链。
- 本轮子任务：
  1. 拉通 `SpringDay1Director -> SpringDay1NpcCrowdDirector -> NPCAutoRoamController -> PersistentPlayerSceneBridge`
  2. 重点核对 `HandleHourChanged / Update / SyncResidentNightRestSchedule / TryBeginResidentReturnHome / TickResidentReturnHome / ApplyResidentNightRestState / ApplyResidentRuntimeSnapshotToState / RecoverReleasedTownResidentsWithoutSpawnStates`
  3. 不进入真实施工，保持只读
- 已完成事项：
  1. 确认 `20:00` 本身只是“开始回家”的可见合同，不是直接 hide：
     - `HandleHourChanged(...)` 不调 crowd hide；
     - crowd 真正的夜间逻辑在 `Update()` 每帧轮询。
  2. 确认当前“到家却没立刻消失”只剩 4 类源码解释：
     - 没拿到 `FormalNavigation arrival`
     - 没进 `0.35` 到站半径
     - `TryDriveResidentReturnHome(...)` 失败后进入 retry/pending
     - 绑定到的 `HomeAnchor` 真值不是玩家以为的那个锚点
  3. 确认 `21:00` 的 fallback 当前是明确的：
     - 释放 shared owner
     - `SnapToTarget(...)`
     - `SetActive(false)`
     - 不是继续 retry return-home
  4. 确认还会把 NPC 留在场上的高价值入口有 3 组：
     - `PersistentPlayerSceneBridge.RestoreCrowdResidentRuntimeState()` -> `ApplyResidentRuntimeSnapshotToState()`
     - `HandleActiveSceneChanged()` / `HandleSceneLoaded()` -> `_syncRequested` -> `RecoverReleasedTownResidentsWithoutSpawnStates()`
     - `HandleSleep()` -> `TrySnapResidentsToHomeAnchorsForDayEnd()` 这条 DayEnd snap 链本身不等于 `21:00 hide`
- 关键判断：
  1. 这轮高置信成立的核心判断是：当前最大不确定性已经不在“有没有夜间系统”，而在“arrival/completion / anchor truth / restore-recover 顺序”。
  2. 这轮最薄弱点是：还没有 fresh runtime probe 去区分某个 packaged 个例到底是 anchor 绑错，还是 navigation completion 没到。
  3. 自评：这轮静态结论可信度高，尤其是 crowd/night/restore 调用顺序；但对单个场景个例，仍只能给到方法级怀疑链，不能冒充 live 证据。
- 当前恢复点：
  1. 若用户让我继续，不该重开全 Day1，只抓夜间 probe：
     - `HomeAnchor`
     - `RequestReturnToAnchor(...)`
     - `TryConsumeFormalNavigationArrival(...)`
     - `ApplyResidentRuntimeSnapshotToState(...)`
  2. 本轮始终只读，未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`。

## 2026-04-17｜只读静态分析：Day1 `0.0.6` / `DinnerConflict` 对 `001/002` 的当前回 Town 入口
- 当前主线目标：
  - 不改代码，只读钉死 Day1 `0.0.6` / `DinnerConflict` 在 `Primary -> Town` 前后对 `001/002` 的真实 owner / escort / reframe 链，并回答如果要改成“先传到各自 Town anchor，再开始 Town 侧下一段移动”，最小最安全的插点和测试补口各是什么。
- 本轮子任务：
  1. 复核 `SpringDay1Director.cs` 里 `TryHandleReturnToTownEscort()`、`BeginDinnerConflict()`、`PrepareDinnerStoryActorsForDialogue()`、`TryResolveDinnerStoryActorRouteFromSceneMarkers()`、`FindPreferredStoryNpcTransform()`
  2. 复核 `SpringDay1NpcCrowdDirector.cs` 里 `ApplyStagingCue()`、`TryResolveStagingCueForCurrentScene()`、`ResolveRuntimeCueOverride()`、`FindSceneResidentHomeAnchor()`
  3. 复核 `SceneTransitionTrigger2D.cs`、`Town.unity`、`Primary.unity`、`SpringDay1TownAnchorContract.json`、`SpringDay1DirectorStageBook.json`、`SpringDay1NpcCrowdManifest.asset` 与现有编辑器测试
- 已完成事项：
  1. 已钉实 `Primary -> Town` 仍然先走 escort，而不是切场即 handoff：
     - `SpringDay1Director.BeginDinnerConflict()` 在 `Primary` 内先 `TryHandleReturnToTownEscort()`，不直接开 Town 晚饭对白。
     - `TryHandleReturnToTownEscort()` 持续用 `TryDriveEscortActor()` 把 `001/002` 朝 `Primary -> Town` 的 `SceneTransitionTrigger2D` 推进，直到 chief / companion / player 都 ready 才触发切场。
  2. 已钉实 scene transition 只给 player 排 entry anchor，不会顺手给 `001/002` 做各自 Town anchor handoff：
     - `SceneTransitionTrigger2D.ResolveTargetEntryAnchorName()` 在 `Primary -> Town` 时只回 `TownPlayerEntryAnchor`。
     - `SceneTransitionTrigger2D.TryStartTransition()` 只 `QueueSceneEntry(resolvedSceneName, resolvedScenePath, ResolveTargetEntryAnchorName())`。
  3. 已钉实回到 `Town` 后，`001/002` 当前仍会再被导演层直接 reframe 到晚饭 markers：
     - `ActivateDinnerGatheringOnTownScene()` -> `AlignTownDinnerGatheringActorsAndPlayer()` 只移动 player。
     - `BeginDinnerConflict()` -> `PrepareDinnerStoryActorsForDialogue()` 若找到 `进村围观/起点/终点`，会对 `001/002` 直接 `ReframeStoryActor(...chiefStart/companionStart...)`。
     - 若 cue settle 超时，`ForceSettleDinnerStoryActors()` 再把 `001/002` 直接 `ReframeStoryActor(...chiefEnd/companionEnd...)`。
  4. 已钉实 `Town` 当前可直接复用的原生锚点只有：
     - `TownPlayerEntryAnchor = (-34.5, 15.8)`。
     - `001_HomeAnchor = (-12.48, 11.85)`。
     - `002_HomeAnchor = (-3.72, 17.36)`。
     - YAML 静态扫描未在当前 `Town.unity` 命中 `进村围观 / 001起点 / 001终点 / 002起点 / 002终点 / DinnerBackgroundRoot / DirectorReady_* / ResidentSlot_* / BackstageSlot_*`，说明这些晚饭/旧 runtime marker 目前不在正式 Town scene 里作为现成对象存在。
  5. 已钉实数据合同层现在也没有 `001/002` 专属 Town anchor：
     - `SpringDay1TownAnchorContract.json` 只有 `EnterVillageCrowdRoot / KidLook_01 / NightWitness_01 / DinnerBackgroundRoot / DailyStand_*`。
     - `SpringDay1DirectorStageBook.json` 的 `DinnerConflict_Table` 只有 `203/104/201/202` 这些 background cue，没有 `001/002` actor cue。
     - `SpringDay1NpcCrowdManifest.asset` 对 `001/002` 仍主要挂 `EnterVillageCrowdRoot / DinnerBackgroundRoot / DailyStand_* / Crowd_201_01` 这类旧 semantic anchors，不是 `001_HomeAnchor / 002_HomeAnchor`。
  6. 已钉实现有测试最接近这次改动面的入口有三组：
     - `SpringDay1OpeningRuntimeBridgeTests` 已覆盖 `Primary -> Town` 只落 `TownPlayerEntryAnchor`。
     - `SpringDay1DirectorStagingTests` 已覆盖 dinner route 必须优先吃 `进村围观/起点/终点`，并忽略 bare `EnterVillageCrowdRoot`。
     - `SpringDay1LateDayRuntimeTests` 已覆盖 `BeginDinnerConflict()` 在 Town 入口只动 player、未超时前先把 `001/002` 拉到各自 dinner start、超时后 snap 到 dinner end。
- 关键决策：
  1. 当前 `0.0.6` 的真实结构是：
     - `Primary` 先 escort 到切场 trigger；
     - 切场只保证 player 落 `TownPlayerEntryAnchor`；
     - 回 Town 后导演层再 special-case 直摆 `001/002` 到 dinner markers。
  2. 如果后续要做“Town anchor first”，最小最安全插点应优先放在 `SpringDay1Director.BeginDinnerConflict()` 的 Town 分支、且位于 `AlignTownDinnerGatheringActorsAndPlayer()` 之后、`PrepareDinnerStoryActorsForDialogue()` 之前：
     - 因为这时已经完成 `Primary -> Town` escort，但还没开始对 `001/002` 做 dinner marker reframe；
     - 只要新增一个 one-shot 的 `TrySnapOrReframeDinnerStoryActorsToTownAnchors()`，就能把 anchor-first handoff 压成导演层的局部补口，而不用先改 scene transition player contract。
  3. 如果继续往更底层做“切场即 handoff”，第二候选插点才是 `SceneTransitionTrigger2D.ResolveTargetEntryAnchorName()/TryStartTransition()` 配套扩协议；但这会把 player-only contract 扩成多 actor contract，风险明显更高。
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1NpcCrowdDirector.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SceneTransitionTrigger2D.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1DirectorStagingTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1OpeningRuntimeBridgeTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Resources\Story\SpringDay1\Directing\SpringDay1TownAnchorContract.json`
  - `D:\Unity\Unity_learning\Sunset\Assets\Resources\Story\SpringDay1\Directing\SpringDay1DirectorStageBook.json`
  - `D:\Unity\Unity_learning\Sunset\Assets\Resources\Story\SpringDay1\SpringDay1NpcCrowdManifest.asset`
  - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Town.unity`
  - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
- 验证结果：
  - 纯静态推断成立；未改任何业务代码 / scene / data 文件，未跑 Unity live。
- 当前主线目标 / 本轮子任务 / 恢复点：
  - 当前主线目标：厘清 Day1 晚段 `001/002` 的 return-to-town owner 与 anchor 合同。
  - 本轮子任务：只读回答 `0.0.6` / `DinnerConflict` 对 `001/002` 的当前回 Town 入口和最小安全改法。
  - 恢复点：如果继续真实施工，先决定 `Town anchor` 是否直接复用 `001_HomeAnchor / 002_HomeAnchor`；若决定复用，先在 `BeginDinnerConflict()` 前半段加 one-shot handoff，并补两组测试：
    - `Primary -> Town` 仍只给 player 排 `TownPlayerEntryAnchor`
    - `DinnerConflict` Town 入口先吃 `001/002` anchor，再进后续 Town 侧 movement / dinner marker

## 2026-04-17｜当前线程续记：`0.0.6 Town anchor-first` 已接进菜单 runner，new case 已过
- 当前主线目标：
  - 继续按 `0417.md` 收 `Package C`；这轮只把 `0.0.6` 的 `001/002 anchor-first` 从“代码已落”推进到“菜单 runner 真跑到”。
- 本轮子任务：
  1. 把 `Director_ShouldSnapStoryActorsToHomeAnchorsDuringPostTutorialExploreWindowInTown` 接进现有 `Run Director Staging Tests`。
  2. 拿到这条新 case 的真实结果，不再停留在静态推断。
- 已完成事项：
  1. `SpringDay1TargetedEditModeTestMenu.cs` 已把：
     - `SpringDay1DirectorStagingTests.Director_ShouldSnapStoryActorsToHomeAnchorsDuringPostTutorialExploreWindowInTown`
     接进 `DirectorStagingTargetTestNames`
  2. 通过 `Library/CodexEditorCommands/requests/*.cmd` 真实触发了 `MENU=Sunset/Story/Validation/Run Director Staging Tests`
  3. 第一次运行未包含新 case；对位后确认是“菜单运行先于 compile 完成”，不是新 case 本身失效
  4. 第二次重跑后：
     - 新 case 已真实出现在结果包
     - 新 case=`Passed`
     - 整套当前=`28 pass / 10 fail`
  5. `0417.md`、子工作区 memory、父工作区 memory 已同步回写
- 验证结果：
  1. `validate_script Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
     - `assessment=no_red`
     - `owned_errors=0`
  2. `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
     - `owned_errors=0`
     - `assessment=unity_validation_pending`
     - blocker=`stale_status`
  3. `validate_script Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - `owned_errors=0`
     - `assessment=unity_validation_pending`
     - blocker=`stale_status`
  4. `git diff --check` 覆盖：
     - `SpringDay1Director.cs`
     - `SpringDay1DirectorStagingTests.cs`
     - `SpringDay1TargetedEditModeTestMenu.cs`
     通过
- 当前判断：
  1. 这刀现在至少已经站住 `结构 / targeted probe`，不再只是“新 test 写进源码了”。
  2. 但这仍不是 packaged/live 过线，因为整套 `Director Staging` 还有旧失败，用户也还没 fresh 验 `0.0.6` 回 Town 首帧体感。
- 当前恢复点：
  1. 下一步优先 packaged/live 复测：
     - `0.0.6` 回 Town 后 `001/002` 是否先出现在各自 `HomeAnchor`
     - 是否还会被同帧旧链 reframe 走
  2. 若失败，下一刀只抓：
     - `HandleSceneLoaded -> UpdateSceneStoryNpcVisibility`
     - `_postTutorialTownActorsAnchored`
     - 是否有别的链在 Town load 同帧又重摆 `001/002`

## 2026-04-17｜线程状态补记：`0.0.6 Town anchor-first` 文档同步与 targeted 验证已 Park
- `thread-state`：
  - `Begin-Slice`：已跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
  - reason=`checkpoint-anchor-first-docsync-and-targeted-validation-recorded`

## 2026-04-17｜当前线程续记：`20:00 pending return-home` 已热修，主剧情 targeted 回归新一轮结果已记录
- 当前主线目标：
  - 继续按 `0417` 收打包前最关键的 Day1 own 风险，不扩散到无关系统。
- 本轮子任务：
  1. 收掉 `20:00` 首次回家 drive 失败后 resident 掉回空态的夜间合同缺口。
  2. 把 `Midday -> FreeTime` 的旧睡觉门槛测试回正到用户最新语义。
  3. 跑一轮 opening / midday / dinner / unified night / owner regression / late-day 的 targeted tests。
- 已完成事项：
  1. `SpringDay1NpcCrowdDirector` 已改成：
     - `20:00` 首次 `TryBeginResidentReturnHome(...)` 失败后
     - 仍保留 `IsReturningHome`
     - 进入 `return-home-pending`
     - 持有 owner、停住当前位置，并继续短 retry
  2. `SpringDay1MiddayRuntimeBridgeTests` 已把
     - `DinnerAndReminderCompletion_ShouldBridgeIntoFreeTime`
     的床交互断言改回“free time 一进入就允许睡”
  3. 已跑 targeted suites：
     - `Opening Bridge`=`13/13 passed`
     - `Midday Bridge`=`8/8 passed`
     - `Dinner Contract`=`7/7 passed`
     - `Unified Night Contract`=`18/18 passed`
     - `Day1 Owner Regression`=`2/2 passed`
     - `Late-Day Bridge`=`11/13 passed`
- 验证结果：
  1. `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - `assessment=no_red`
     - `owned_errors=0`
  2. `validate_script Assets/YYY_Tests/Editor/SpringDay1MiddayRuntimeBridgeTests.cs`
     - `assessment=no_red`
     - `owned_errors=0`
  3. fresh `errors`：`0 error / 0 warning`
  4. `git diff --check` 覆盖当前 touched Day1 own 文件通过
- 当前判断：
  1. 这轮已经把 Day1 主剧情 targeted 面大面积收绿。
  2. 当前自动化剩余明确压缩到 late-day bridge `2` 条，不再是全线散红。
  3. 这仍然不是 packaged/live 已过线；结构 / targeted probe 明显增强，但体验仍待用户路径终验。
- 当前恢复点：
  1. 如果继续自动化补刀，只抓：
     - `BedBridge_EndsDayAndRestoresSystems`
     - `DayEndPlayerFacingCopy_ShouldCarryTomorrowBurdenAndClearWorkbenchState`
  2. 如果先向用户汇报，应明确：
     - 哪些 targeted suites 已全绿
     - 哪两条 late-day 仍未闭环

## 2026-04-17 遮挡与 Primary 气泡抑制只读核查
- 主线仍是 Day1 打包前收尾；本轮是插入式阻塞核查，不进入真实施工，未跑 Begin-Slice。
- 用户新增两条高价值反馈：
  1. Primary / 工作台引导时，001 的提示气泡怀疑被 suppression 吃掉。
  2. 遮挡重新出现后，树林透明扩散感觉偏大，希望核实参数。
- 已确认：
  - 截图里的文案“跟上，到工作台我再慢慢教你。”不是普通 ambient nearby bubble，而是 `SpringDay1Director.TryShowEscortWaitBubble()` 直接调用 `NPCBubblePresenter.ShowConversationText()` 打出的导演气泡。
  - `NPCBubblePresenter.SetInteractionPromptSuppressed()` 只阻断 `Ambient`；`Conversation` 通道是白名单，所以这张图对应的导演气泡不属于被 prompt suppression 直接吃掉的那条链。
  - 仍然存在的老毛病在 `PlayerNpcNearbyFeedbackService.ShouldSuppressNearbyFeedbackForCurrentStory()`：它把 `NpcInteractionPriorityPolicy.ShouldSuppressAmbientBubbleForCurrentStory()` 直接套在 all-formal phases 上，而 `FarmingTutorial` 仍在 formal-priority 集合里，没有给 001/教学引导做例外。
  - 遮挡现值：Town / Primary 两个场景都序列化为 `rootConnectionDistance=1.5`、`maxForestSearchRadius=15`、`maxForestSearchDepth=50`、`minOcclusionRatio=0.4`。
  - `OcclusionManager.AreTreesConnected()` 还叠了树冠重叠率 `>= 0.15` 的硬编码条件，所以树林扩散不是单参数问题。
- 判断：
  - 001 气泡要区分“导演 conversation 气泡”和“ordinary nearby ambient 气泡”；前者已白名单，后者仍被总抑制吃掉。
  - 遮挡问题当前更像参数口径偏宽，不像整条 occlusion runtime 坏死。
- 后续最小安全刀：
  1. 如果要修 001 提示气泡，应在 nearby ambient 抑制链上补 Day1/001/教学阶段白名单，而不是去动 conversation 通道。
  2. 如果要收树林扩散，优先从场景序列化参数 `rootConnectionDistance / maxForestSearchRadius` 下手，再决定要不要动硬编码 overlap 阈值。

## 2026-04-18｜只读彻查：NPC 交互自杀链、多层禁聊链与 roam 节奏真值
- 当前主线目标：
  - 继续服务 Day1 打包前收尾，但这轮不先开修，而是把“为什么 NPC 到现在还是不能正常聊天、为什么按 `E` 只冒一个字就消失、为什么 roam 节奏显得过密”用代码彻底钉死。
- 本轮子任务：
  1. 彻查 `NPCInformalChatInteractable -> PlayerNpcChatSessionService -> NPCAutoRoamController`
  2. 复核 `SpringDay1Director / SpringDay1NpcCrowdDirector` 的 formal / informal 组件关口
  3. 复核人类 NPC roam pause 的真实来源与现值
- 已完成事项：
  1. 已确认一个 `NPC own` 全局硬 bug：
     - `NPCInformalChatInteractable.EnterConversationOccupation()` 会 `AcquireStoryControl("npc-informal-chat")`
     - `PlayerNpcChatSessionService.Update()` 又把任何 `IsResidentScriptedControlActive` 当 takeover，下一帧直接 `CancelConversationImmediate(...)`
     - 这就是“按 E 后只冒一个字就消失”的直接源码根因
  2. 已确认当前聊天异常至少有 4 层叠加：
     - `SpringDay1Director.ApplyStoryActorRuntimePolicy(...)`
     - `SpringDay1NpcCrowdDirector.SetResidentCueInteractionLock(...)`
     - `NPCInformalChatInteractable.IsResidentScriptedControlBlockingInformalInteraction()`
     - `PlayerNpcChatSessionService.Update()` scripted-control 自杀判断
  3. 已确认 roam 节奏当前主吃 `NPCRoamProfile.asset`：
     - crowd `101~203` 多为 `shortPause 0.5~3 / longPause 3~6`
     - `001` 为 `0.8~2.6 / 4~6.8`
     - `002` 为 `0.7~2.8 / 3.6~6.2`
     - 要落用户要求的 `短停 2~5 / 长停 5~8`，应优先改现用 profile 资产，不是只改脚本默认值
  4. 已把上述结论同步进 `0417.md` 的：
     - `2. 当前代码真相`
     - `4. 全量问题清单`
     - `8. 迭代 tasks`
- 本轮没有做：
  1. 未改 runtime 代码
  2. 未跑 compile / tests / live
  3. 不宣称任何体验过线
- 当前恢复点：
  1. 下一刀若继续真实施工，优先级应是：
     - 先修 `NPC informal chat` 自杀链
     - 再清 Day1 own 的多层禁聊 / 关组件链
     - 然后再收 `NPCRoamProfile` 的停顿节奏
  2. 这轮为落 `0417` 与 memory 已补跑 `Begin-Slice`；停手前需补 `Park-Slice`

## 2026-04-18｜真实施工续记：`C-06a~e` 已落代码并拿到 targeted 结果
- 当前主线目标：
  - 把上轮只读钉死的 `NPC informal chat 自杀链 / Day1 直接禁聊链 / roam pause 过短` 真正收成代码闭环，并补最小 targeted 验证。
- 本轮子任务：
  1. 修 `PlayerNpcChatSessionService` 的 self-owner 误杀
  2. 修 `NPCInformalChatInteractable / NPCDialogueInteractable` 的 scripted-control gate
  3. 清 `SpringDay1Director / SpringDay1NpcCrowdDirector` 的直接关组件链
  4. 改现用 human NPC roam profile pause
  5. 把新增回归挂进 `Run NPC Formal Consumption Tests`
- 已完成事项：
  1. `PlayerNpcChatSessionService.Update()` 已不再把 `npc-informal-chat` 自己 owner 当 takeover
  2. `NPCInformalChatInteractable` 已增加 self-owner 放行
  3. `NPCDialogueInteractable` 已增加 formal 对应的 scripted-control gate
  4. `SpringDay1Director.ApplyStoryActorRuntimePolicy(...)` 已不再直接关 `formal / informal`
  5. `SpringDay1NpcCrowdDirector.SetResidentCueInteractionLock(...)` 已不再直接关 `formal / informal`
  6. `NPC_Default / 001 / 002 / 003 / 101~203` 的 pause 已统一到 `2~5 / 5~8`
  7. `SpringDay1TargetedEditModeTestMenu.cs` 已把这轮新增 3 条交互回归挂进 `Run NPC Formal Consumption Tests`
- 验证结果：
  1. `manage_script validate`：
     - 本轮脚本与测试文件 `errors=0`
  2. `Run NPC Formal Consumption Tests`：
     - `6/6 passed`
  3. `Run Director Staging Tests`：
     - `30/10`
     - 本轮相关 case 过
     - 剩余 fail 属旧 staging 债
  4. fresh `errors`：
     - `0 error / 0 warning`
  5. `git diff --check`：
     - 覆盖本轮 own 文件通过
- 当前判断：
  1. `C-06a~e` 现在已经从 0417 的“待做”进入“代码已落 + targeted 已拿到”。
  2. 这轮最关键的新结构不是再加一条 phase 例外，而是彻底改成：
     - 组件常驻
     - interaction contract 按 owner/phase 判定
     - `20:00` 回家窗口也可正常交互
  3. 仍未闭环的不是这轮新增交互链，而是：
     - `Director Staging` 旧 10 fail
     - packaged/live 最终用户路径
- 当前恢复点：
  1. 若继续自动化，优先决定：
     - 是继续清 staging 老 fail
     - 还是停给用户做 packaged/live 终验
  2. 停手前必须补：
     - `0417` 回写
     - 父层/线程 memory
     - `skill-trigger-log`
     - `Park-Slice`

## 2026-04-18｜只读审计：4 条 `SpringDay1DirectorStagingTests` semantic/stagebook fail 不属于当前 packaged 必收
- 当前主线目标：
  - 继续服务 `0417` 的 Day1 packaged 前裁定，本轮只读回答 4 条 staging semantic/stagebook fail 的真实归因、入口链和 pack 前优先级。
- 本轮实际做成：
  1. 直接回读了 `SpringDay1DirectorStagingTests.cs`、`SpringDay1DirectorStaging.cs`、`SpringDay1NpcCrowdDirector.cs`、`SpringDay1DirectorStagingWindow.cs`、`SpringDay1DirectorStageBook.json`。
  2. 回读了最新 `Library/CodexEditorCommands/spring-day1-director-staging-tests.json`，确认这 4 条在 `2026-04-18 01:47:38 +08:00` 仍真实失败。
  3. 把 4 条分成两类：
     - `StageBook exact-cue priority`：`TryResolveCue()` 先吃 `duty`，导致 `201` 先命中 `101`
     - `semantic start/offset/rebase`：`SpringDay1DirectorStagingPlayback` 底层完全没接这三种语义
  4. 同时钉死：它们当前都不是 Day1 packaged runtime 的现役硬 blocker。
- 关键判断：
  1. exact-cue 这条主要影响 `SpringDay1DirectorStagingWindow.TryResolveDraftCueForPreview()` 与 dead helper；当前仓内未找到 `ResolveSpawnPoint()` 调用点。
  2. semantic start 三条虽然暴露底层 playback 真缺口，但 live crowd runtime 目前只在 `EnterVillage_PostEntry / DinnerConflict_Table` 走 scene-marker absolute override；`FreeTime/ReturnReminder/DayEnd/DailyStand` 当前不走 live staging playback。
  3. `SpringDay1DirectorStageBook.json` 里 migrated semantic-start 数据是真存在的，所以这不是“假测试”；只是 data 已迁、底层 editor/playback 没跟上、runtime 又暂时绕开。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1DirectorStagingTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Directing\SpringDay1DirectorStaging.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1NpcCrowdDirector.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1DirectorStagingWindow.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Resources\Story\SpringDay1\Directing\SpringDay1DirectorStageBook.json`
  - `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\spring-day1-director-staging-tests.json`
- 验证结果：
  - 纯静态推断 + 最新 artifact 复核成立；
  - 未改代码，未跑 live，未进入 `Begin-Slice`。
- 当前主线 / 子任务 / 恢复点：
  - 主线仍是 Day1 packaged 前裁定。
  - 本轮子任务是把这 4 条 fail 从“疑似 runtime blocker”收窄成“editor-preview / staging-helper 债”。
  - 如果继续施工：
    1. packaged 主线仍优先 `staging/takeover` runtime 合同；
    2. 这 4 条改归 `Package F / F-07`，只在要恢复导演预演可信度时再成组处理。

## 2026-04-18｜只读彻查：`SpringDay1DirectorStagingTests` 5 条剩余失败的真实根因
- 当前主线目标：
  - 继续服务 Day1 打包前裁定，但这轮不写代码，只把 `SpringDay1DirectorStagingTests.cs` 里用户点名的 5 条剩余失败拆成“真 runtime 债 / editor-runtime 债 / stale tests”。
- 本轮子任务：
  1. 直接回读 5 条失败用例源码。
  2. 回读命中的真实入口：
     - `SpringDay1DirectorStagingPlayback`
     - `SpringDay1DirectorNpcTakeover`
     - `SpringDay1DirectorStagingRehearsalDriver`
     - `NPCAutoRoamController` 的 `RequestStageTravel / RequestReturnHome / DebugMoveTo / PauseStoryControlMotion / ResumeStoryControlMotion`
  3. 对照当天 `Library/CodexEditorCommands/spring-day1-director-staging-tests.json` 的 fresh 失败输出。
- 已完成事项：
  1. 已确认 5 条失败里最值钱的 pack 前簇是：
     - `StagingPlayback_ShouldDriveRoamControllerInsteadOfHardPushingTransformDuringCueMotion`
     - `NpcTakeover_ShouldDisableRoamAndInteractionsUntilRelease`
     两条都打在 `SpringDay1DirectorStaging.cs` 当前 staging/takeover 合同。
  2. 已钉实：
     - `SpringDay1DirectorStagingPlayback.Update()` 仍直接 `transform.position += step`，没有走 `NPCAutoRoamController` facade。
     - `SpringDay1DirectorNpcTakeover.Acquire()/Release()` 当前只关 `roam / dialogue / informal / bubble` 等组件，没有同步 `AcquireStoryControl/ReleaseStoryControl`，因此 `IsResidentScriptedControlActive` 为假。
  3. `RehearsalDriver_ShouldPauseExistingPlaybackUntilDisabled` 更像 editor-runtime/tooling 真债：
     - 2026-04-07 的旧文档明确说过“开始排练先暂停 playback，结束后恢复”
     - 但现码 `OnEnable()/OnDisable()` 已不再处理 playback enabled 状态
  4. 两条 `ResidentScriptedControl_*` 更像 stale tests，不像 runtime 坏：
     - `ResidentScriptedControl_DebugMoveShouldStillParticipateInSharedAvoidance`
     - `ResidentScriptedControl_PauseAndResumeShouldPreserveScriptedMove`
     现测试仍手搓旧字段，没有建立 `activePointToPointTravelContract`
     - 第一条的 plain-debug 分支没有吃 `DebugMoveTo(...)`
     - 第二条没有把 move contract 设成 `ResidentScripted/FormalNavigation`
- 当前稳定判断：
  1. 这轮最核心判断是：剩余 5 fail 不能整体当成“Day1 runtime 还没修好”，真正 pack-blocker 更像 `staging/takeover` 这 2 条。
  2. 我为什么这样判断：
     - 现码与当天 fresh json 对得上
     - `PlayerNpcChatSessionService / NPCInformalChatInteractable / NPCDialogueInteractable` 都已经把 `IsResidentScriptedControlActive / ownerKey / IsFormalNavigationDriveActive()` 当真合同输入
     - 所以 `NpcTakeover` 少 owner 同步不是表面字段问题，而是 runtime 对外语义真的断了
  3. 最薄弱点：
     - 我这轮没有新跑 Unity live，只是吃了当天已有 fresh suite 结果和源码
     - 对 `RehearsalDriver` 是否算当前打包前必须收，仍带一层“是否还依赖导演排练工具”的产品判断
  4. 自评：
     - 这轮结论可信度高，尤其是 `staging/takeover` 与两条 stale tests 的拆分；
     - 但对 `RehearsalDriver` 的打包优先级，我给自己的是“中高置信，不冒充绝对值”。
  5. 当前恢复点：
     - 如果继续真实施工，第一刀先收 `SpringDay1DirectorStagingPlayback + SpringDay1DirectorNpcTakeover`
     - 不要先回 `NPCAutoRoamController` 内核，也不要把两条 stale tests 当成主刀方向
- 验证状态：
  - `静态推断成立 + 当天 fresh director-staging result 已对上`
  - 本轮始终只读，未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`

## 2026-04-18｜只读审计：Day1 `0417 / Package F` 仍 pending 的 `F-05~F-08` 残依赖复核
- 用户目标：
  - 只读代码审计，不改业务文件；帮 Day1-V3 收 `0417` 里仍 pending 的 `Package F` 残依赖，只回答高置信代码事实。
- 本轮实际确认：
  1. `F-05`：
     - `SpringDay1ActorRuntimeProbeMenu.cs` 仍把 `Town_Day1Residents` 保留在 `KnownNpcRootNames`，并在 tracked-NPC 识别里继续消费这套旧 root。
  2. `F-06`：
     - bootstrap 文件未扫到 direct 旧 beat/anchor 命中；
     - validation/probe 菜单仍有三簇旧真值：
       - `TownScenePlayerFacingContractMenu.cs` 仍把 `EnterVillageCrowdRoot / DinnerBackgroundRoot / NightWitness_01 / DailyStand_01` 当 runtime anchors；
       - `SpringDay1DirectorPrimaryLiveCaptureMenu.cs` 仍把 `FreeTime_NightWitness / DailyStand_Preview` 与 `night-witness-102 / daily-*` 当固定 capture target；
       - `SpringDay1DirectorTownContractMenu.cs` 仍把 `003` 纳入 authored marker probe，并要求 `003起点/003终点`。
  3. `F-07`：
     - `NpcCrowdManifestSceneDutyTests.cs`、`NpcCrowdResidentDirectorBridgeTests.cs`、`SpringDay1DirectorStagingTests.cs` 里仍有把旧 semantic anchor / old beat / 003 scene-marker 特判当真值的断言。
  4. `F-08`：
     - `SpringDay1NpcCrowdManifest.asset`、`SpringDay1TownAnchorContract.json`、`SpringDay1DirectorStageBook.json` 仍直接保存旧 anchor 名；
     - `SpringDay1DirectorStageBook.json` 仍保留 `003` 专用 cue；
     - `SpringDay1NpcCrowdDirector.cs` 仍有 `BuildSyntheticThirdResidentResidentEntry()`；
     - `SpringDay1Director.cs` 仍有 `003` 专用 object/marker/fallback 链。
  5. “无命中”也已补钉：
     - `Town.unity / Primary.unity` 无 exact old-root 文本残留；
     - `Town_Day1Carriers` 在代码/数据/scene 无命中；
     - `Town_Day1Residents` 在 runtime/data/scene 无命中，当前只剩 editor probe/禁用提示。
- 当前判断：
  1. `Package F` 现在主要是 editor/test/data truth cleanup，不是 scene 文本没删干净。
  2. 最该警惕的 runtime-risk 不是旧根名还在 scene，而是：
     - data 仍显式保存旧 anchor/beat；
     - `003` 仍有 synthetic runtime + director fallback 两层特殊入口。
- 当前恢复点：
  1. 如果后续进入真实施工，先清 editor/test 的旧真值，再收 data/runtime。
  2. 本轮始终只读，未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`。

## 2026-04-18｜继续施工：Package F 主干收口 + G 自动化 fresh 补证
- 当前主线目标：
  - 继续把 `0417.md` 当唯一主控板，优先完成 Day1 打包前还能安全收完的 editor/test/scene/automation 事项，不冒充 packaged/live 已过线。
- 本轮子任务：
  1. 清 `TownScenePlayerFacingContractMenu.cs` 的旧 anchor blocker。
  2. 清 `NpcCrowdManifestSceneDutyTests.cs` 的旧 semantic-anchor expected truth。
  3. 补 `F-09` fresh probe，外加最小 live runner 取证。
- 已完成事项：
  1. `TownScenePlayerFacingContractMenu.cs`
     - 已把旧 runtime anchor blocker 退场；fresh `Town Player-Facing Contract Probe` 现在 `completed`。
  2. `NpcCrowdManifestSceneDutyTests.cs`
     - `ExpectedEntry` 不再携带旧 semantic anchor 白名单；只保留 duty/phase/growthIntent 主合同。
  3. fresh `validate_script`
     - `Assets/Editor/Town/TownScenePlayerFacingContractMenu.cs`=`no_red`
     - `Assets/YYY_Tests/Editor/NpcCrowdManifestSceneDutyTests.cs`=`no_red`
  4. fresh probe/test：
     - `Town Entry Contract Probe`=`completed`
     - `Town Player-Facing Contract Probe`=`completed`
     - `Day1 Staging Marker Probe`=`completed`
     - `Resident Director Bridge Tests`=`3/3 passed`
     - `Town Runtime Anchor Readiness Probe`=`deprecated-runtime-anchor-readiness-probe`
  5. fresh console：
     - `errors --count 20 --output-limit 5` => `0 error / 0 warning`
  6. fresh live partial：
     - `Reset Spring Day1 To Opening` 后，`Live Snapshot Artifact` 的 step trace 已真实从 `CrashAndMeet` 推进到 `EnterVillage`
     - 另抓到 `FreeTime` live snapshot
     - `Force Spring Day1 Dinner Validation Jump` 当前仍落到 `FarmingTutorial / 16:00 / FreeTime_NightWitness`，不能当 dinner live 证据
  7. `0417.md` 已回写：
     - `L-08 / L-09`
     - `I-14 / I-16`
     - `Package F`=`已完成`
     - `Package G`=`进行中`
     - `G-01/G-02/G-03`=`已完成`
- 关键判断：
  1. `Package F` 这轮可以收口，不需要再为了 editor/test/scene 旧真值继续大刀。
  2. 现在真正剩下的是终验，不是结构主修：
     - `G-04` 更完整 live
     - `G-05` packaged
     - `G-06` profiler
     - `G-07` 验收包
  3. `ForceDinnerValidationJump` 暂时不能用来声称晚饭 live 已过，这条必须如实保留。
- 验证结果：
  1. `validate_script` 两个新增 touched files 均 `assessment=no_red`
  2. fresh probes/test 如上
  3. fresh console `0/0`
  4. live 仅到 partial trace，不是全路径终验
- 当前恢复点：
  1. 如果继续这条线程，优先继续想办法把 `G-04` live 拉到更完整，而不是回头再清 `F`。
  2. 如果准备停下，先补 `skill-trigger-log`，再按现场决定是否 `Park-Slice`。

## 2026-04-18｜继续施工补记：`G-04/G-06` 最新 artifact 仍未覆盖 `Town`
- 当前主线目标：
  - 继续按 `0417` 直接收 `Package G`，优先查 live/profiler 自动链是否能安全打到 `Town`。
- 本轮新增事实：
  1. `spring-day1-live-snapshot.json`
     - 这次 `Request Spring Day1 Town Lead Transition Artifact` 后
     - `actionResult = transition-requested:PrimaryHomeDoor`
     - `activeScene = Assets/000_Scenes/Home.unity`
     - 说明当前 Town lead 自动链还没把场景带进 `Town`
  2. `npc-roam-spike-stopgap-probe.json`
     - 当前最新只在 `Primary` 采到 `npcCount=0 / roamNpcCount=0`
     - 不能当 `Town/free-roam/20:00` profiler 证据
- 当前判断：
  1. `Package F` 现在已经不是主刀方向。
  2. `Package G` 目前最实的 blocker 是 Town live/profiler 自动入口不足。
  3. 这两条必须如实留在 `G-04/G-06`，不能包装成“只差 packaged”。
- 当前恢复点：
  1. 下一步优先彻查 `CodexEditorCommandBridge`、现有 live menu、late-phase jump、scene transition 链，找是否存在无需新增代码的 Town 入口。
  2. 若找不到，就明确把它们收成当前终验 blocker。

## 2026-04-18｜继续施工：`Package G` 已推进到 `Town free-time / 20:00`，dinner 仍是 blocker
- 当前主线目标：
  - 继续按 `0417` 收 `Package G`，尽量把 live/runtime probe 往后推，但不把结构成立写成体验已过线。
- 本轮代码改动：
  1. `Assets/Editor/Story/SpringDay1LiveSnapshotArtifactMenu.cs`
     - 修正 `Request Spring Day1 Town Lead Transition Artifact` 的 fallback trigger 选择。
  2. `Assets/Editor/Story/SpringDay1LatePhaseValidationMenu.cs`
     - 新增 `Advance Spring Day1 Validation Clock +1h`。
- fresh 验证：
  1. `validate_script`
     - `SpringDay1LiveSnapshotArtifactMenu.cs`=`no_red`
     - `SpringDay1LatePhaseValidationMenu.cs`=`no_red`
  2. fresh live：
     - `Town -> Primary` settled snapshot = `Primary.unity`
     - `Primary -> Town` settled snapshot = `Town.unity`
     - `Reset -> step` 已真实推进到 `FarmingTutorial`
     - `Force FreeTime + Request Transition` 已真实拿到 `Town / FreeTime / 19:30`
     - `Advance +1h` 已真实拿到 `Town / FreeTime / 20:00`
  3. fresh runtime spot probe：
     - `NpcRoamSpikeStopgapProbe`
       - `Town 19:30`=`npcCount 26 / roamNpcCount 26`
       - `Town 20:00`=`npcCount 26 / roamNpcCount 25`
  4. dinner blocker：
     - `Force Spring Day1 Dinner Validation Jump` 当前仍只落到 `FarmingTutorial / 16:00`
     - 即使请求回 `Town`，也仍是 `FarmingTutorial`
- 当前判断：
  1. `G-04` 现在至少已覆盖：
     - `opening`
     - `primary`
     - `healing`
     - `workbench`
     - `farming`
     - `Town free-time 19:30`
     - `Town 20:00`
  2. `G-06` 的 `Town` scene-entry/free-roam/20:00` runtime probe` 已有。
  3. 当前真正剩余 blocker 已收窄到：
     - `DinnerConflict`
     - `19:00/21:00/次日`
     - packaged
     - 人工 profiler / 最终验收
- 当前恢复点：
  1. 如果继续收 `Package G`，下一步优先盯晚饭入口和夜间后半段。
  2. 如果先向用户汇报，必须报实：
     - `Town` 现场验证已明显前进
     - `DinnerConflict` 和终验还没完。

## 2026-04-18｜继续施工补记：`21:00` 对 Day1 tracked resident 已补 probe，次日当前只到 `Home/DayEnd`
- 当前主线目标：
  - 在已拿到的 `Town 19:30 / 20:00` 基础上，继续补晚段后半截的 live/probe。
- 本轮新增事实：
  1. 同一条稳定链已真实拿到：
     - `Town / FreeTime / 21:00`
  2. `NpcRoamSpikeStopgapProbe`
     - `21:00`=`npcCount 16 / roamNpcCount 16`
  3. 但 `spring-day1-actor-runtime-probe.json` 与 `spring-day1-resident-control-probe.json` 在 `21:00` 对 Day1 tracked resident 已为空：
     - activeActors=`[]`
     - residentOwned=`[]`
  4. 连续推进到次日后：
     - 当前 settled snapshot 落到 `Home.unity / DayEnd / 07:00 AM`
     - `NpcRoamSpikeStopgapProbe`=`0 / 0`
     - 这不是 `Town` 侧早晨释放
- 当前判断：
  1. `21:00` 的 `16/16` 主要不是 Day1 tracked resident。
  2. 当前最实的剩余 blocker 已进一步收窄到：
     - `DinnerConflict`
     - `Town` 侧次日 `07:00` release
     - packaged / 手工 profiler / 最终验收

## 2026-04-18｜只读工具链探索：CodexEditorCommandBridge / requests / status / archive 用法彻查
- 当前主线目标：
  - 继续服务 `Day1-V3 / Package G`，只读确认现有 Editor 命令桥是否足够稳定支撑 `PLAY -> MENU -> 等结果 -> STOP` 这类自动链，不改代码。
- 本轮只读核实：
  1. `Assets/Editor/NPC/CodexEditorCommandBridge.cs`
     - bridge 只消费 `Library/CodexEditorCommands/requests/*.cmd`
     - 只认三类命令：`PLAY`、`STOP`、`MENU=...`
     - 每次只取按文件名排序后的第一条请求
     - 消费后会把原请求 `File.Move` 到 `archive/*.cmd`
     - `status.json` 只提供桥层状态：`timestamp / isPlaying / isCompiling / lastCommand / success`
  2. 现场目录：
     - `requests` 里当前残留了 3 个 `.txt` 请求：`PLAY/PLAY/STOP`
     - 因为扩展名不是 `.cmd`，桥不会执行它们
     - `archive` 里存在大量真实样本，命令正文可直接看到：
       - `PLAY`
       - `MENU=Sunset/Story/Validation/Write Spring Day1 Live Snapshot Artifact`
       - `STOP`
  3. 现有稳定结果模式分两层：
     - bridge 层完成 = 请求文件从 `requests` 消失并进入 `archive`，同时 `status.json` 更新
     - 业务层完成 = 对应菜单自己写出的结果文件进入终态
       - 例如 `spring-day1-live-snapshot.json.status=completed`
       - 例如 `spring-day1-director-staging-tests.json.status=completed`
  4. 已有稳定样例：
     - `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs` 会先把结果文件写成 `running`，测试结束再写成 `completed/failed`
     - `Assets/Editor/Story/SpringDay1LiveSnapshotArtifactMenu.cs` 在 PlayMode 内写 `spring-day1-live-snapshot.json`
     - `Assets/Editor/Town/TownScenePlayerFacingContractMenu.cs` 在 EditMode 写 `town-player-facing-contract-probe.json`
  5. memory 里已有明确口径：
     - 先 `MENU=Assets/Refresh` 让新菜单编译/注册
     - 再发目标 `MENU=...`
     - Play 链收尾要看 `status.json => isPlaying=false, isCompiling=false, lastCommand=playmode:EnteredEditMode`
- 关键判断：
  1. 当前最稳的调用方式不是直接盯 `status.json.success=true`，而是：
     - 写唯一一条 `requests/*.cmd`
     - 等它被搬进 `archive`
     - 再看目标结果文件是否进入 `completed/attention/blocked/failed` 等终态
  2. `status.json` 可以判断桥是否空闲、是否仍在 Play/Compile、最近一条桥命令是什么；但它不能单独证明菜单业务已经真正跑完。
  3. 现桥可以安全串联短链 `PLAY -> MENU -> 等业务结果 -> STOP`，前提是：
     - 一次只排 1 条请求
     - 每一步都等待前一步从 `requests` 清空
     - 目标菜单本身有可靠结果文件
     - PlayMode 中途没有 compile/domain reload/旧会话卡死
  4. 它不够安全支撑“多条命令无等待连发”的乐观批处理：
     - bridge 没有 requestId
     - `status.json.lastCommand` 会被 `playmode:*`、`compilation-finished` 等异步状态覆盖
     - 无法只靠 `lastCommand` 把某次具体 `MENU=...` 与结果一一绑定
- 已知坑：
  1. `.txt` 请求不会被吃，只会残留并误导排查。
  2. `MENU=...` 成功只代表 `ExecuteMenuItem()` 找到菜单并调用，不代表业务结果已完成。
  3. 菜单未注册时，bridge 会消费请求，但 `status.json.lastCommand` 只会留下 `menu:...` + `success=false`；memory 里已有 `ExecuteMenuItem failed because there is no menu named ...` 实例。
  4. `status.json` 会被 `playmode:EnteredPlayMode/EnteredEditMode`、`compilation-finished` 覆盖，不能拿它当长期结果仓库。
  5. 若 Editor 正在 `isCompiling` 或 `isUpdating`，bridge 只会反复写 `waiting-editor-busy`，不会消费队列。
  6. `requests` 里若堆多条命令，只按文件名字典序串行消费；若命名不稳定，执行顺序就不稳。
  7. 某些 live 菜单本身要求 `Application.isPlaying=true`，若没先 `PLAY`，结果文件会写成 `blocked/not-in-playmode`。
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPC\CodexEditorCommandBridge.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1LiveSnapshotArtifactMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Town\TownScenePlayerFacingContractMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1TargetedEditModeTestMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\status.json`
  - `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\requests`
  - `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\archive`
  - `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\spring-day1-live-snapshot.json`
  - `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\town-player-facing-contract-probe.json`
  - `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\spring-day1-director-staging-tests.json`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_1.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\memory_0.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\100-重新开始\memory.md`
- 验证结果：
  - 纯只读推断 + 现场目录/结果文件/历史 memory 交叉成立
  - 未改代码，未跑 `Begin-Slice`
- 当前恢复点：
  1. 如果下一轮要真用这套桥打 `Town` 自动链，优先采用“单命令单等待单结果文件”的保守序列。
  2. 如果仍打不到 `Town`，应把 blocker 诚实记为“现有菜单入口不足”，而不是继续怀疑 bridge 本身是否完全失效。

## 2026-04-18｜只读代码探索：Spring Day1 现有菜单链能推进到哪一步
- 用户目标：
  - 以 `Day1-V3` 只读探索分身身份，彻查 `Spring Day1` 现有 live/validation 菜单、runner、debug menu、native fresh restart、late-phase jump、scene transition，回答是否已有一条不用新增代码的安全自动链，能把 PlayMode 从 fresh/reset 推到 `Town free-time`、`20:00 return-home`，或至少稳定进入 `Town` 并拿到 live snapshot。
- 本轮实际做成：
  1. 已确认一条高置信现成链：
     - 编辑态 `Restart Spring Day1 Native Fresh` 对齐 `Town`
     - PlayMode 内同菜单走 `SaveManager.RestartToFreshGame()`，重开到 `Town@09:00`
     - `Force Spring Day1 FreeTime Validation Jump` 可直接把 runtime 拉到 `Town free-time`
     - `Write Spring Day1 Live Snapshot Artifact` / `Write Spring Day1 Actor Runtime Probe Artifact` / `Write Spring Day1 Resident Control Probe Artifact` 可直接在该现场写证据
  2. 已确认一条更贴近 `20:00 return-home` 的现成链：
     - `Force Spring Day1 Dinner Validation Jump`
     - 再用 `Trigger/Step Spring Day1 Validation` 或 `Force Complete Or Advance Dialogue` 把 dinner/reminder 推到 `FreeTime`
     - `TimeManagerDebugger` 在 PlayMode 自动挂载；按一次 `+` 可把 `19:30` 推到 `20:00`
     - `SpringDay1NpcCrowdDirector.ShouldResidentsReturnHomeByClock()` 只看时钟，因此会触发 resident return-home
  3. 已钉死两个现成边界：
     - `TriggerRecommendedAction()` 并不会自动触发 `postTutorial chief wrap`，所以不存在“fresh opening 开始只靠同一个 Step 菜单一路跑到晚段”的单链
     - 当前没有 `Day1/Town` 专用 profiler 自动菜单；snapshot/probe 有，profiler 仍要人工开窗口
  4. 已解释旧坏相：
     - `Request Spring Day1 Town Lead Transition Artifact` 会误落 `Home`，是因为它在 director 没有待处理 escort 时，会退回“按当前 activeScene 猜目标场景；猜不出来就取第一个 trigger”的 fallback
     - 那次返回 `transition-requested:PrimaryHomeDoor`，最终落 `Home.unity`，与代码完全对上
- 关键判断：
  1. 现有入口已经足够拿 `Town free-time/live snapshot`，所以 `Package G` 现在不是“完全没有入口”。
  2. 但若把目标提高到“fresh->20:00/profiler 一键自动化”，最小 blocker 仍在：
     - 缺 post-tutorial chief-wrap 的自动 step
     - 缺 Town profiler 专用入口
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1NativeFreshRestartMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1LatePhaseValidationMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1LiveSnapshotArtifactMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\DialogueDebugMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SceneTransitionTrigger2D.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PersistentPlayerSceneBridge.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\TimeManagerDebugger.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Town.unity`
  - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
  - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Home.unity`
- 验证结果：
  - 纯静态审计，未改代码，未进 Unity，未跑 `Begin-Slice`
- 恢复点：
  1. 如果下一轮继续只用现有入口拿 Town 晚段 live，优先走 `Native Fresh + FreeTime/Dinner Jump + Snapshot/Probe`。
  2. 不要再把 `Town Lead Transition Artifact` 当可靠主入口；它当前应视为“未过滤 trigger 的不安全桥”。

## 2026-04-18｜只读代码审计：为什么现有 live validation 还进不了 `DinnerConflict`
- 用户目标：
  - 作为 `Day1-V3` 并行只读代码审计子线程，只看 `SpringDay1LatePhaseValidationMenu.cs`、`SpringDay1LiveSnapshotArtifactMenu.cs`、`DialogueDebugMenu.cs`、`SpringDay1Director.cs` 及直接调用 helper，查清：
    1. 为什么 `Force Dinner Validation Jump` 还只会落到 `FarmingTutorial / 16:00`
    2. 最小、最安全的 `editor-only helper` 应该插在哪里、做什么
    3. 不建议改 runtime 主链，除非 editor helper 做不到
- 已完成事项：
  1. 已钉死 `ForceDinnerValidationJump()` 的同步竞态：
     - 菜单先 `PreparePostTutorialExploreWindow()`
     - 随后立即 `TryRequestDinnerGatheringStart(true)`
     - 失败再 `ActivateDinnerGatheringOnTownScene()`
  2. 已钉死真正断点在 editor 入口没有等待 `EnterPostTutorialExploreWindow()` 的异步 blink 落稳：
     - `EnterPostTutorialExploreWindow()` 在 `Primary` 优先 `SceneTransitionRunner.TryBlink(...)`
     - 回调未落地前，`_postTutorialExploreWindowEntered` 仍未成立
     - 所以 `TryRequestDinnerGatheringStart()` 因 `!IsPostTutorialExploreWindowActive()` 返回 `false`
     - fallback `ActivateDinnerGatheringOnTownScene()` 也会因当前尚未处于 `Town` 直接 early-return
     - 最终现场就是 `FarmingTutorial / 16:00`
  3. 已确认通用 live stepper 也不会补这段桥：
     - `SpringDay1LiveValidationRunner.TriggerRecommendedAction()` 在 `FarmingTutorial` 只会调 `SpringDay1Director.TryAdvanceFarmingTutorialValidationStep()`
     - 该方法在教学目标完成后仅返回“去和村长收口”，并不会触发 `postTutorial wrap / explore window / dinner request`
- 关键决策：
  1. 最小修口位应放在 `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1LatePhaseValidationMenu.cs`
  2. 方案应是补一个 `editor-only` 延迟/轮询 helper：
     - 等 `_postTutorialExploreWindowEntered=true`
     - 等 `SceneTransitionRunner.IsBusy=false`
     - 再复用现有 director 私有方法 `TryRequestDinnerGatheringStart(true)` / `ActivateDinnerGatheringOnTownScene()`
     - 直到 `StoryManager.CurrentPhase == StoryPhase.DinnerConflict` 或超时
  3. 当前没有证据支持先改 `SpringDay1Director` runtime 晚饭主链
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1LatePhaseValidationMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1LiveSnapshotArtifactMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\DialogueDebugMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\100-重新开始\memory.md`
- 验证结果：
  - 纯静态审计成立
  - 未改代码
  - 未进 Unity
  - 未跑 `Begin-Slice`
- 遗留问题或下一步：
  1. 如果下一轮要真修，只修 editor 菜单 helper，不先碰 runtime 主链。
  2. 在 helper 落地前，现有 `TriggerRecommendedAction/Step` 仍只能稳定到 `FarmingTutorial` 收口，不会自动跨进 `DinnerConflict`。

## 2026-04-18｜并行只读代码审计：为什么次日只落 Home/DayEnd/07:00，还没有 Town 侧 07:00 resident release 证据
- 用户目标：
  - 作为 `Day1-V3` 的并行只读代码审计子线程，只看 `SpringDay1LatePhaseValidationMenu.cs`、`SpringDay1ActorRuntimeProbeMenu.cs`、`SpringDay1ResidentControlProbeMenu.cs`、`SpringDay1Director.cs`、`SpringDay1NpcCrowdDirector.cs` 及紧邻 helper，查清当前 live 链为什么次日只能停在 `Home/DayEnd/07:00`，以及若想拿到 `Town` 侧次日 `07:00` resident release 证据，最小 editor-only helper/validation 路径是什么、哪些 probe 会误判。
- 本轮实际做成：
  1. 已钉死“为何落 Home 而非 Town”：
     - `SpringDay1LatePhaseValidationMenu.AdvanceOneHourValidationClock()` 在目标小时 `> 26` 时直接调用 `TimeManager.Sleep()`，并不会切 `Town`。
     - `SpringDay1Director.HandleSleep()` 在 `FreeTime -> DayEnd` 收束时，`FinalizeSleepSceneState()` 会强制 `LoadSceneThroughPersistentBridge(HomeSceneName)`，随后执行 `TrySnapResidentsToHomeAnchorsForDayEnd()`、`TrySnapStoryActorsToHomeAnchorsForDayEnd()`，并清掉 `SpringDay1NpcCrowdDirector.ClearResidentRuntimeSnapshots()` 与 `PersistentPlayerSceneBridge.ClearNativeResidentRuntimeSnapshots("001","002","003")`。
     - `SpringDay1LiveValidationRunner.GetRecommendedNextAction()` / `TriggerRecommendedAction()` 对 `StoryPhase.DayEnd` 直接判定为“下一轮再测明天”，不会再给 Town 次日入口。
  2. 已钉死“为什么拿不到 Town 侧次日 07:00 resident release 证据”：
     - `SpringDay1NpcCrowdDirector` 的 runtime 只支持 `Primary/Town`；`Home` 不是 supported runtime scene，切到 `Home` 后 `Update()` 和 `HandleActiveSceneChanged()` 都会走 `TeardownAll()`。
     - 因为导演在 `HandleSleep()` 里又主动清空 resident runtime snapshots，所以晚段 `return-home/night-rest` 的 crowd 连续态不会带进次日 `Home@07:00`。
     - 即便 director 的当前 beat 在 `_dayEnded=true` 时会映射成 `DailyStandPreview`，`SpringDay1NpcCrowdDirector.ShouldYieldDaytimeResidentsToAutonomy()` 仍在 `currentPhase >= StoryPhase.DayEnd` 时直接返回 `false`；所以“看到 `DailyStandPreview`”不等于“已经跑过 Town 白天 release”。
  3. 已给出最小 helper 方向：
     - 如果只要“Town 侧 07:00 endpoint 证据”，最小 editor-only helper 应该从 `Home/DayEnd/07:00` 现场出发，调用 director 的 `LoadSceneThroughPersistentBridge("Town")`，等 active scene 真正变成 `Town` 后执行 `SpringDay1NpcCrowdDirector.ForceImmediateSync()`，再写 `ResidentControlProbe` 或 `ActorRuntimeProbe`。
     - 这样拿到的是“Town 侧 07:00 已在 scene 内、resident 当前未被 scripted control 持有”的 endpoint 证据；不是“整夜合同连续保留到次日 release”的 continuity 证据，因为 `HandleSleep()` 已经清空过 snapshots。
  4. 已标出会误判的 probe：
     - `LatePhaseValidationMenu.AdvanceOneHourValidationClock()` 的“已跨天推进到 07:00”只代表睡觉收束成功，不代表 Town 侧 resident release 已验证。
     - `SpringDay1ResidentControlProbeMenu` / `SpringDay1ActorRuntimeProbeMenu` 只写“当前 active scene”快照；如果在 `Home/DayEnd/07:00` 时运行，写出来的不是 Town 证据。
     - `SpringDay1NpcCrowdDirector.CurrentRuntimeSummary` 在 `Home` 或 teardown 后只会反映“当前没有 Town/Primary runtime state”，不能把 `not-created` / 空 states 误读成“release 成功”。
     - `DailyStandPreview` beat 本身也会误导：它来自 `SpringDay1Director.GetCurrentBeatKey()` 的 DayEnd 分支，不等于 crowd 已走完次日 Town release。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1LatePhaseValidationMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1ActorRuntimeProbeMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1ResidentControlProbeMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1NpcCrowdDirector.cs`
- 验证结果：
  - 纯静态代码审计，未改业务代码，未进 Unity，未跑 `Begin-Slice`。
- 恢复点：
  1. 如果后续只想补“Town 侧 07:00 endpoint 证据”，应新增一个最小 editor-only helper：`DayEnd/Home@07:00 -> Load Town -> ForceImmediateSync -> 写 ResidentControlProbe/ActorRuntimeProbe`。
  2. 如果后续想补“整夜连续合同成立”的更强证据，则当前 `HandleSleep()` 的清 snapshot 设计本身就是 blocker，单靠现有 probe 不能证明。

## 2026-04-18｜收口补记：0417 已转成终验主控板，当前我的真实停点只剩人工 packaged / profiler
- 当前主线目标：
  - 把 `Day1-V3` 从“fresh live helper 已补齐但板子与记忆还没最终收口”推进到真正可交接的终验状态。
- 本轮实际做成：
  1. 更新主控板：
     - `D:\\Unity\\Unity_learning\\Sunset\\.kiro\\specs\\900_开篇\\spring-day1-implementation\\100-重新开始\\0417.md`
     - 新增 `C-25` 记录工具链真相
     - 回勾 `C-02 / C-08 / D-02 / D-03 / D-08b`
     - `Package G` 改成 `待人工终验`
     - 新增 `8.1 当前封板判断`
  2. 新建最终验收包：
     - `D:\\Unity\\Unity_learning\\Sunset\\.kiro\\specs\\900_开篇\\spring-day1-implementation\\100-重新开始\\0418_打包终验包.md`
  3. 吸收只读审计结论：
     - `B-02/B-03/E-03/C-07` 仍是结构债或信心缺口
     - 但当前不再阻塞打包前最小闭环
  4. 亲自试了 command bridge 自动串联 `PLAY/RESET/STOP` 想补 `G-06`：
     - `PLAY`、`RESET`、`STOP` 最终都会进 `archive`
     - 但当前机器上桥的消费延迟是分钟级
     - 这会把脚本误导成 timeout，因此不能把桥层批处理包装成“稳定自动化 perf suite”
  5. 已确认 Unity 当前回到 `EditMode`：
     - `status.json => isPlaying=false / isCompiling=false / lastCommand=playmode:EnteredEditMode`
- 当前判断：
  1. 这条线现在不该再被表述成“还卡在 Day1 runtime 主修”。
  2. 更诚实的状态是：
     - `结构 / targeted / fresh live helper` 已封板
     - `packaged / Profiler / 用户体感` 仍待人工终验
  3. 当前我自己无法再假装完成的，只剩：
     - `G-05` packaged build 用户路径复验
     - `G-06` 人工 Unity Profiler spot check
  4. `G-07` 已通过 `0418_打包终验包.md` 落地。
- 涉及文件：
  - `D:\\Unity\\Unity_learning\\Sunset\\.kiro\\specs\\900_开篇\\spring-day1-implementation\\100-重新开始\\0417.md`
  - `D:\\Unity\\Unity_learning\\Sunset\\.kiro\\specs\\900_开篇\\spring-day1-implementation\\100-重新开始\\0418_打包终验包.md`
  - `D:\\Unity\\Unity_learning\\Sunset\\Library\\CodexEditorCommands\\status.json`
  - `D:\\Unity\\Unity_learning\\Sunset\\Library\\CodexEditorCommands\\archive`
- 验证结果：
  1. `git diff --check` 覆盖 `0417.md / 0418_打包终验包.md` 通过
  2. 这轮没有新增 runtime 代码
  3. 这轮也没有 fresh packaged 或人工 Profiler 结果
- 当前恢复点：
1. 下一步如果继续，直接按 `0418_打包终验包.md` 做人工终验。
2. 不要再优先折腾 command bridge 的多步自动链。

## 2026-04-18｜真实施工：补收 `16:00` 交互、`Primary 001` 提示同源、`20:00` 回家半态
- 用户目标：
  1. `16:00` 后自由活动时，`002/003` 必须能像普通 NPC 一样聊天
  2. `Primary` 的 `001` 等待提示必须回到和 `TownHouseLead` 同源，只保留文案差异，`002` 要继续贴着 `001` 跟随
  3. `20:00` 回家不能再出现“先卡住，推一下才开始走”的半态
- 已完成事项：
  1. 前置核查已补：
     - `skills-governor`
     - `sunset-workspace-router`
     - `preference-preflight-gate`
     - `sunset-no-red-handoff`
     - 已读 `global-preference-profile.md`、`0417.md`、线程 memory
  2. `SpringDay1Director`：
     - `IsPostTutorialExploreWindowActive()` 改成可供交互层读取
     - `TryHandleWorkbenchEscort()` 的等待合同收回 `TownHouseLead` 基线
       - 只暂停 `001`
       - `002` 等待时继续补位
       - 非等待态也改回围绕 `001` 的显式编队跟随
  3. `NpcInteractionPriorityPolicy`：
     - 新增 `AllowsResidentFreeInteractionPhase(...)`
     - `FarmingTutorial + explore window` 不再被当成 formal-priority 总闸
  4. `NPCDialogueInteractable / NPCInformalChatInteractable`：
     - scripted-control 放行不再只认 `FreeTime`
     - 现在认 `FreeTime` 或 `postTutorialExploreWindow`
  5. `SpringDay1NpcCrowdDirector`：
     - explore window 打开后停止对 `001/002` 的 story-escort defer
     - 新增 `QueueResidentReturnHomeRetry(...)`
     - `20:00` 首次起手失败不再立刻冻结成 pending 假接管
  6. `NPCAutoRoamController`：
     - `DriveResidentScriptedMoveTo(...)` 只在 contract 相同的情况下复用旧 move
     - formal-navigation 起手失败且 owner 不是原本顶层 owner 时，会回滚临时 scripted-control
  7. 新增测试：
     - `FarmingTutorialExploreWindow_ShouldStopSuppressingResidentInformalInteraction`
     - `InformalChatInteractable_ShouldRemainAvailableDuringPostTutorialExploreWindowReturnNavigation`
  8. 已同步回写：
     - `0417.md`
     - 子工作区 memory
     - 父工作区 memory
- 验证结果：
  1. `validate_script` 覆盖：
     - `SpringDay1Director.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - `NPCAutoRoamController.cs`
     - `NpcInteractionPriorityPolicy.cs`
     - `NPCDialogueInteractable.cs`
     - `NPCInformalChatInteractable.cs`
     - `NpcInteractionPriorityPolicyTests.cs`
     均为 `owned_errors=0`
  2. fresh `errors`=`0 error / 0 warning`
  3. `validate_script` 的统一 assessment 仍是 `unity_validation_pending`
     - 原因：外部 `stale_status`
     - 当前没有 own red 证据
  4. `git diff --check` 全仓失败是外部脏场景/资源尾账，不是我这轮 own 文件问题；本轮未清 unrelated dirty
- 关键决策：
  1. `16:00` 的根因不是 `001` 把所有 NPC 都吃掉，而是 explore window 仍挂在 `FarmingTutorial` 相位上。
  2. `Primary 001` 提示的根因不是对象差异，而是我把 `WorkbenchEscort` 扩成了另一套等待合同。
  3. `20:00` 的根因不是“多广播几次”，而是 scripted-control / return-home 起手失败后的半态策略脏了。
- 当前恢复点：
  1. 下一步优先等用户 fresh live / packaged 复测这 3 条 reopen。
  2. 如果仍有 reopen，继续按 `0417 -> G-05b` 处理，不回到散修。

## 2026-04-18｜只读复核：`Primary 001` 提示再次漂移的根不是对象差异，而是我把 `WorkbenchEscort` 扩成了新合同
- 用户最新要求：
  - 不先修，先只读彻查并反思：为什么 `Primary` 的 `001` 提示又和 `Town` 不一样，为什么本该只换文案却被我做成了不同逻辑。
- 本轮只读结论：
  1. `Primary` 与 `Town` 里真正被导演 resolve 到的 `001`，都具备 `NPCAutoRoamController + NPCBubblePresenter`，不是“Primary 的 001 天生不同”。
  2. 真正的 drift 在 `SpringDay1Director.TryHandleWorkbenchEscort()`：
     - 没有只复用 `TownHouseLead` 的等待合同
     - 被我扩成 `escort wait + workbench target + idle placement + briefing/ready gate`
  3. 代码级最明显的偏移：
     - `TownHouseLead` 等待时只暂停 `chief`
     - `WorkbenchEscort` 等待时我把 `chief/companion` 都一起暂停
     - `TownHouseLead` 还会继续驱动 `002` 向编队位补位
     - `WorkbenchEscort` 等待窗口直接 `return`
  4. 用户这次的判断成立：
     - 这不是单纯换文案
     - 而是我第二次把一条已知正确的提示合同重新做成了另一套逻辑
  5. 这轮已把问题回写到：
     - `0417.md` 的 `C-24a / I-21 / C-08a`
- 这轮没做什么：
  - 未改 runtime
  - 未跑 Unity live
  - 未跑 `Begin-Slice`
- 下一步恢复点：
  1. 如果用户允许开修，先只收 `WorkbenchEscort` 的等待提示合同，不扩别的线。
  2. 修法方向应是回到 `TownHouseLead` 已知正确基线，只保留文案差异，把 workbench briefing/ready/E 交互与“等玩家跟上”分层。

## 2026-04-18｜真实施工切片：night-return-arrival-closeout-and-roam-pause-tuning
- 当前主线目标：
  - 继续收 `Day1` 当前打包前硬点：
    1. `20:00` 到家后不隐藏
    2. `101` 夜间回家异常但不写特判
    3. roam 停顿节奏按现有字段改成 `shortPause 0.5~5 / longPause 3~8`
- 本轮子任务：
  - 把 crowd 的“到家即隐藏”判定和 formal-navigation 实际到站合同对齐，并统一回写现用 roam profile。
- 已完成事项：
  1. 已跑 `Begin-Slice`
     - slice=`night-return-arrival-closeout-and-roam-pause-tuning`
  2. 在 `NPCAutoRoamController` 新增 runtime-only：
     - `TryResolveFormalNavigationDestination(...)`
  3. 在 `SpringDay1NpcCrowdDirector`：
     - `ResidentReturnHomeArrivalRadius` 从 `0.35 -> 0.64`
     - `TryBeginResidentReturnHome(...)` / `TickResidentReturnHome(...)` / arrival consumption 现在都可对齐 formal-navigation arrival proxy
  4. 新增 targeted test：
     - `CrowdDirector_TickResidentReturnHome_ShouldHideWhenResidentIsWithinFormalArrivalTolerance`
  5. 已把现用人类 NPC profile 收成：
     - `shortPause 0.5~5`
     - `longPause 3~8`
     - 覆盖 `001/002/003/default/101~203/301`
  6. 已回写：
     - `0417.md`
     - 子/父工作区 memory
- 验证结果：
  1. `git diff --check` 通过
  2. `validate_script` 覆盖 `NPCAutoRoamController.cs / SpringDay1NpcCrowdDirector.cs / SpringDay1DirectorStagingTests.cs` 均为 `owned_errors=0`
  3. 当前 external blocker：
     - `DialogueChinese V2 SDF.asset` importer inconsistent result
     - Unity `stale_status`
  4. `Ready-To-Sync` 尝试失败不是代码 blocker，而是状态层锁：
     - `.kiro/state/ready-to-sync.lock`
- 关键决策：
  1. `101` 不写专用分支，继续按普通 resident 合同修。
  2. 这轮根因判断改成：
     - `FormalNavigation` 到站尺度
     - crowd 到家隐藏尺度
     之前不一致。
- 当前恢复点：
  1. 若继续这条线，先做 packaged/live 复测，不要回头写 `101` 特判。
  2. 若要收口 sync，先处理 `ready-to-sync.lock` 的状态层 blocker。

## 2026-04-18｜真实施工：`101 DayEnd_Settle` 从夜间压场 authoring 收回普通 resident
- 用户目标：
  - 继续只盯 `101` 夜间单点异常，但不允许我写 `101` 特判。
- 本轮子任务：
  - 查清 `101` 的最硬静态差异，并优先修 authored truth。
- 已完成事项：
  1. 用代码/scene/prefab 三面对位后确认：
     - `101_HomeAnchor` 正常存在且有引用
     - `101/102/103` prefab 上没有明显只会导致 `101` 夜间坏掉的结构差异
     - 最硬差异是 `101 DayEnd_Settle` 还在 manifest 里被写成 `Pressure + AmbientPressure`
  2. 已修改：
     - `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
       - `101 DayEnd_Settle presenceLevel: 4 -> 2`
       - `flags: 224 -> 192`
     - `Assets/YYY_Tests/Editor/NpcCrowdManifestSceneDutyTests.cs`
  3. 已同步更新：
     - `0417.md`
     - 子/父工作区 memory
- 验证结果：
  1. `validate_script Assets/YYY_Tests/Editor/NpcCrowdManifestSceneDutyTests.cs` => `assessment=no_red`
  2. direct MCP `run_tests(EditMode)`：
     - `NpcCrowdManifestSceneDutyTests.CrowdManifest_ShouldExposeResidentSemanticMatrix_ForVillageResidentLayer`
     - `status=succeeded`
  3. `git diff --check` 覆盖本轮 touched files 通过
- 关键决策：
  1. 这刀继续守住“不写 `101` 专用 runtime 分支”。
  2. 现在只能宣称：
     - `101` 的 authored special-case 已收
     - packaged/live 还需要用户 fresh 复测
- 当前恢复点：
  1. 下一轮如果用户继续报 `101` 还异常，再查 runtime bind / nav startup，不回滚到 data truth 争论。
  2. 收尾前记得补 `Park-Slice`、skill trigger log、并关闭子线程。

## 2026-04-18｜只读根因排查：Day1 `16:00` 后 `002/003` 仍不能正常交互
- 当前主线目标：
  - 只读钉死 `Day1` 在 `16:00` 后 post-tutorial explore window 里，为什么 `002/003` 仍不像普通 NPC 一样能聊，并区分哪些限制属于 Day1 own、哪些属于 NPC own。
- 本轮子任务：
  1. 只审用户点名的 4 个脚本：
     - `NPCInformalChatInteractable.cs`
     - `NPCDialogueInteractable.cs`
     - `SpringDay1Director.cs`
     - `SpringDay1NpcCrowdDirector.cs`
  2. 必要时只补看邻近 helper：
     - `NpcInteractionPriorityPolicy.cs`
- 已完成事项：
  1. 确认 `SpringDay1Director.CanConsumeStoryNpcInteraction(...)` 是严格的 `001` 专用分支：
     - 先 `ReferenceEquals(actor, ResolveStoryChiefTransform())`
     - 非 chief 直接 `false`
     - 所以 Day1 直接剧情吃交互的 own 目标只有 `001`
  2. 确认 `EnterPostTutorialExploreWindow()` 虽然把时间钉到 `16:00` 并打开 explore window，但没有把 `StoryManager.CurrentPhase` 切出 `FarmingTutorial`。
  3. 确认 NPC 交互层仍主要按 `StoryPhase` 判断，而不是按“post-tutorial explore window 已经语义上自由活动”判断：
     - `NPCDialogueInteractable.IsResidentScriptedControlBlockingFormalInteraction()`
     - `NPCInformalChatInteractable.IsResidentScriptedControlBlockingInformalInteraction()`
     - 两者都只在 `ResolveCurrentStoryPhase() == StoryPhase.FreeTime` 且是 return-home formal navigation drive 时才放行 resident scripted control
     - `NpcInteractionPriorityPolicy.IsFormalPriorityPhase(...)` 仍把 `FarmingTutorial` 视为 formal-priority phase
  4. 确认 crowd 层对 `002/003` 的归因不同：
     - `002` 如果走 resident runtime，会在 `ShouldDeferToStoryEscortDirector(...)` 中整段 `FarmingTutorial` 继续 defer 给导演私链
     - `003` 不在这个 defer 条件里；按代码应回到普通 resident baseline，不属于 chief-only director consume
- 关键判断：
  1. 这轮最硬公共根因不是“Day1 把所有 NPC 都吃掉了”，而是：
     - post-tutorial explore window 语义已经是自由活动
     - 但技术相位仍是 `FarmingTutorial`
     - NPC 交互 gating 继续按 tutorial 相位执行
  2. `001` 的限制是 Day1 own 且符合语义。
  3. `002` 的死锁是 mixed：
     - Day1/crowd 继续持有 scripted control
     - NPC 交互代码又只在 `FreeTime` 才给 scripted-control 例外
  4. `003` 不属于 chief-only Day1 consume；
     - 它如果仍不能聊，代码落点只能回到 NPC own 的 phase-gated suppression / scripted-control 条件，而不是 `SpringDay1Director.CanConsumeStoryNpcInteraction(...)`
- 验证结果：
  1. 本轮只读分析，未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
  2. 未改业务代码、未做 Unity live 写
  3. 结论来源是指定脚本的静态条件链交叉对位
- 涉及文件：
  - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
  - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
  - `Assets/YYY_Scripts/Story/Interaction/NpcInteractionPriorityPolicy.cs`
- 当前恢复点：
  1. 如果下一轮允许开修，最小最安全方向不要改 whole phase/timeline，也不要把 `16:00` 直接切成真正 `FreeTime`。
  2. 更安全的是补一个“post-tutorial explore window 对非 `001` 视为 free-chat”的窄 helper，然后只在：
     - resident scripted-control 放行条件
     - informal suppression 条件
     两处放开 `002/003`，继续保留 `001` 的 chief-only consume。

## 2026-04-18｜真实施工：疗伤前 `Home` 门禁与 `Primary 001` 气泡误杀已补收
- 当前主线目标：
  - 继续按 `0417` 收 Day1 打包前 reopen，优先堵住两条会直接把流程打死或让用户误判的 runtime 口：
    1. 疗伤前仍可误进 `Home`
    2. `Primary 001` 的 conversation 提示被导演层自己闪掉
- 本轮实际做成：
  1. `SpringDay1Director.SyncPrimaryHomeEntryGate()` 现在会同时控制：
     - `PrimaryHomeDoor` 的 `SceneTransitionTrigger2D.enabled`
     - 门体 `Collider2D.enabled`
     疗伤未完成前统一关闭，疗伤完成后才放开。
  2. `SpringDay1Director` 已补 `pre-healing home intrusion` 恢复链：
     - 当前在 `Home`
     - 且疗伤未完成
     - 会被拉回 `PrimaryHomeEntryAnchor`
  3. `ApplyStoryActorRuntimePolicy(...)` 不再无条件 `HideBubble()`：
     - `conversation-priority` 气泡现在不会被 storyActorMode 下一拍误杀
     - `Primary 001` 的等待提示回到正常保持显示
  4. `0417.md` 已同步：
     - `C-34 / C-35`
     - `I-25 / I-26`
     - `8.2` 的 `C-01c / C-08a-3 / G-05c`
- 验证结果：
  1. `SpringDay1Director.cs` `validate_script => errors=0 / warnings=3`
  2. `SpringDay1DirectorStagingTests.cs` `validate_script => errors=0 / warnings=0`
  3. 新增窄测 `4/4 passed`：
     - `SpringDay1DirectorStagingTests.Director_ShouldKeepPrimaryHomeEntryGateClosedBeforeHealingCompletes`
     - `SpringDay1DirectorStagingTests.Director_ShouldAllowPrimaryHomeEntryGateAfterHealingCompletes`
     - `SpringDay1DirectorStagingTests.Director_ShouldRecoverPreHealingHomeIntrusionWhenHomeSceneIsActive`
     - `SpringDay1DirectorStagingTests.Director_ShouldKeepConversationBubbleVisibleDuringStoryActorMode`
  4. 测试过程里的 `TestResults.xml` 清理噪声已清掉，fresh console=`0 error / 0 warning`
- 关键决策：
  1. 这刀修的是门禁合同和导演层气泡策略，不是再造新的场景跳转系统。
  2. 这刀能诚实表述成：
     - `结构 / targeted / fresh console` 已闭环
     - `packaged / 用户体感` 仍待复测
- 当前恢复点：
  1. 下一步如果继续，只看用户 fresh live / packaged 对：
     - 疗伤前 `PrimaryHomeDoor` 不可进入
     - 疗伤后自动恢复
     - `Primary 001` 气泡不再闪掉
  2. 这轮收尾前还需要：
     - `Park-Slice`
     - 补 `skill-trigger-log`

## 2026-04-18｜支撑子任务：`Tool_002_BatchHierarchy` own warning 已收
- 当前主线目标：
  - 继续 Day1 打包前收尾；这轮只是用户插入的阻塞修复，不是换主线。
- 本轮实际做成：
  1. 用户贴的 warning 栈已对到：
     - `Tool_002_BatchHierarchy.OnEnable() -> LoadPersistedSelection()`
  2. 当前文件已不是旧 `GlobalObjectId` 版本；真正制造噪声的是窗口开启时自动恢复旧锁定选择。
  3. 已移除 `OnEnable()` 里的自动 `LoadPersistedSelection()`，不再在窗口打开瞬间重放旧锁定列表。
- 验证结果：
  1. `validate_script Assets/Editor/Tool_002_BatchHierarchy.cs` => `errors=0 / warnings=2`
  2. clear console 后执行：
     - `Tools/002批量 (Hierarchy窗口)`
  3. fresh console 未再出现用户贴出的 cross-scene / `DontDestroyOnLoad` warning
  4. `git diff --check -- Assets/Editor/Tool_002_BatchHierarchy.cs` 通过
- 关键决策：
  1. 这刀只收 own warning 触发口，不顺手扩成场景引用大清扫。
  2. 如果后续还有同类 warning，就要回到具体 scene/runtime 引用本身查，不再先怪 `Tool_002_BatchHierarchy`。
- 当前恢复点：
  1. 这条支撑子任务已闭环。
  2. 收尾还需：
     - `Park-Slice`
     - `skill-trigger-log`

## 2026-04-18｜只读自查：截图里的两份 `Town` 退役菜单当前不像活跃 own warning
- 当前主线目标：
  - 继续 Day1 打包前收尾；这轮只是用户补了一张模糊截图，让我自查 `TownSceneRuntimeAnchorReadinessMenu / TownNativeResidentMigrationMenu` 是否还是 own warning 源。
- 本轮实际做成：
  1. 静态对位了两份脚本的当前版本。
  2. 当前确认：
     - 两者都已是“退役后直接返回 deprecated blocker/result”的版本
     - 当前只会 `Debug.Log(...)`
     - 不会主动 `Debug.LogWarning(...)`
  3. 仓内也没有别的脚本在主动调用这两个菜单路径或 blocker code。
- 验证结果：
  1. `validate_script Assets/Editor/Town/TownSceneRuntimeAnchorReadinessMenu.cs` => `errors=0 / warnings=0`
  2. `validate_script Assets/Editor/Town/TownNativeResidentMigrationMenu.cs` => `errors=0 / warnings=0`
  3. 当前 console 没有这两条 fresh warning 证据
- 关键决策：
  1. 在当前证据下，不把这两份文件算成活跃 blocker。
  2. 不为了模糊截图盲改这两个退役工具。
- 当前恢复点：
  1. 如果后续它们再次出现，最值钱的是抓完整 warning 正文。
  2. 当前这轮按“只读排除项”结算，不开 runtime / editor 改动刀。

## 2026-04-19｜Town 退役菜单与 Unity MCP 当前 console 快照
- 当前主线目标：
  - 继续 Day1 打包前收尾；本轮只做只读核查并向用户汇报最新红面。
- 本轮实际做成：
  1. 重新抓了最新 `sunset_mcp.py errors`。
  2. 当前快照是 `4 errors / 0 warnings`，且 4 条都来自 `Library/PackageCache/com.coplaydev.unity-mcp/...`，不是 Day1 业务脚本 own 红。
  3. 对 `Assets/Editor/Town/TownSceneRuntimeAnchorReadinessMenu.cs` 与 `Assets/Editor/Town/TownNativeResidentMigrationMenu.cs` 又做了一轮 `validate_script`：
     - 两者 own_errors 都是 `0`
     - own_warnings 都是 `0`
  4. 这说明用户口中的“62 个报错”在当前现场没有复现成同一组本地业务红，而更像 Unity MCP / stale 状态侧的外部信号。
- 关键决策：
  1. 这两个 Town 退役菜单当前不应再被当成主嫌。
  2. 如果后续继续追红，优先看 Unity MCP 包缓存与 editor stale 状态，不要回头重打这两份退役工具。
- 当前恢复点：
  1. 等用户下一轮决定是继续追外部包红，还是切回 Day1 业务 own 红。
  2. 本轮已完成只读核查，后续若要施工再另起刀。

## 2026-04-19｜Primary Home 门禁改到 `0.0.5`
- 当前主线目标：
  - 用户把 `Home` 门禁进一步收紧：不是 `0.0.3` 结束后开，也不是 `0.0.4` 一进场就开，而是整个 `0.0.4` 都锁着，进到 `0.0.5` 才能进。
- 本轮实际做成：
  1. `SpringDay1Director.ShouldAllowPrimaryHomeEntry()` 已从 `WorkbenchFlashback` 门槛改成 `FarmingTutorial` 门槛。
  2. `SpringDay1DirectorStagingTests` 已同步改口：
     - `WorkbenchFlashback` 继续关闭
     - `FarmingTutorial` 才开放
  3. `0417.md` 已同步更新为这套新真值。
- 验证结果：
  1. `validate_script SpringDay1Director.cs` => `owned_errors=0`
  2. `validate_script SpringDay1DirectorStagingTests.cs` => `owned_errors=0`
  3. fresh console => `0 error / 0 warning`
  4. `git diff --check` 覆盖本轮两份文件通过
- 关键决策：
  1. 这刀只改门禁相位，不顺手扩别的 Primary/Home 逻辑。
  2. 后续复测标准也要一起换口径：`0.0.4` 不能进，`0.0.5` 才能进。
- 当前恢复点：
  1. 这刀代码层已闭环。
  2. 若下轮继续，优先等用户 fresh live / packaged 复测，不回头再争旧语义。

## 2026-04-19｜Day1 返家卡顿与 001 闲聊缺口的最小修口
- 当前主线目标：
  - 继续 Day1 打包前收尾，盯住两根硬问题：
    1. `20:00` 后 NPC 到点不自己回家，要玩家“挤一下”才开始走。
    2. `001`（村长）剧情外仍然不能正常聊天。
- 本轮实际做成：
  1. 在 `NPCAutoRoamController` 里把 `FormalNavigation` 的 blocked/stuck 早停改成“保留合同继续跑”，不再在恢复失败时直接 `EndDebugMove(false)`。
  2. 在 `SpringDay1Director` 里给 story actor 增加普通闲聊面兜底：只要 `RoamProfile` 本来就有 informal content，退回 ordinary NPC 规则时就自动补 `NPCInformalChatInteractable`。
  3. 给 `NPC_001_VillageChiefDialogueContent.asset` 补了最小可玩的 `defaultInformalConversationBundles`，让 `HasInformalConversationContent` 真值先成立。
  4. 新增了两条编辑器测试：
     - `NpcAutoRoamController_FormalNavigationBlockedAdvanceShouldKeepContractAlive`
     - `Director_ShouldRestoreMissingInformalSurfaceWhenStoryActorsReturnToOrdinaryNpcRules`
- 验证结果：
  1. `git diff --check` 覆盖本轮改动通过。
  2. `validate_script` 对三份脚本的 codeguard 都是 `error=0 / warning=0`。
  3. 但 Unity 侧仍然没有 active instance，`refresh_unity` / `wait_ready` 报 `No active Unity instance`，所以最终验证仍停在 `unity_validation_pending`。
- 关键判断：
  1. `001` 这次的首因更像是数据资产缺 bundle，不是 priority policy 首因。
  2. `20:00` 回家卡住则更像是 FormalNavigation 恢复失败后被过早掐断合同，而不是导演根本没发回家命令。
  3. 这两刀都属于打包前可做的最小安全修法。
- 当前恢复点：
  1. 代码层已落刀。
  2. 还差 Unity active instance / live 侧验证。
  3. 如果下一轮继续，先看 `001` 闲聊是否恢复，再看夜间回家是否还要玩家推一下。
