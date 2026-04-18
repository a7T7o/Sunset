# 102-owner冻结与受控重构 - 开发记忆

## 模块概述
- 模块名称：102-owner冻结与受控重构
- 模块目标：先把 `Day1` 当前所有仍 open 的问题、语义基线、owner 冻结表、跨线程分工和后续施工包写成整体文档，再进入下一轮真实退权。

## 当前状态
- **完成度**：文档层第一轮已完成
- **最后更新**：2026-04-15
- **状态**：已停车，等待用户确认下一轮是继续发导航/NPC 还是由本线程开始 Package B

## 会话记录

### 会话 1 - 2026-04-15

**用户需求**:
> 我希望是对于day1现存所有问题的总览以及整体施工方向，是一个整体的清单，不是这一刀的……做完这个后，我希望你还要给导航写一个语义同步contract……对于npc……你自行思考。

**完成任务**:
1. 在 `ACTIVE` slice=`102_owner-freeze-and-controlled-refactor` 下先完成只读审计补强：
   - resident lifecycle 全入口
   - `003` 特殊化残留
   - owner 级 stopgap tests
2. 新增 5 份工作文件：
   - [Day1 现存问题总览与整体施工总表](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_Day1现存问题总览与整体施工总表.md)
   - [施工方向与分阶段清单](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_施工方向与分阶段清单.md)
   - [owner 冻结总表](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_owner冻结总表.md)
   - [给导航的语义同步与执行边界 contract](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_给导航_语义同步与执行边界contract.md)
   - [给 NPC 的协作边界与 facade 落地 prompt](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_给NPC_协作边界与facade落地prompt.md)
3. 在合同文档之外，额外补了 3 份可直接转发的引导文本文件：
   - [给 Day1-V3 自己的整体推进分阶段续工引导 prompt](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_给Day1-V3_整体推进分阶段续工引导prompt_01.md)
   - [给导航的弱引导 prompt](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_给导航_Day1-V3语义同步与执行边界弱引导prompt_01.md)
   - [给 NPC 的弱引导 prompt](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_给NPC_Day1-V3协作边界与facade弱引导prompt_01.md)
4. 这轮把方向从“这一刀的 owner 冻结”提升成“Day1 全量 open 问题总览”：
   - 明确列出当前已核实 open 的问题
   - 区分“用户语义已定但我尚未重核”的部分
   - 明确“opening handoff 第一刀已落，但整题未解”
5. 这轮明确写死了三方最终边界：
   - `Day1` 负责剧情语义、staged contract、release intent
   - `NPC` 负责 resident state 与 facade
   - `导航` 负责所有真实 movement execution
6. 这轮给出主导判断：
   - `Day1-V3` 不继续吞 NPC facade
   - `NPC` 应主导高危 public API 收口
   - `导航` 应主导统一 movement execution contract
7. 补充只读外部材料：
   - `NPC` 活跃记忆与 2026-04-15 locomotion contract 草案
   - `导航` 活跃记忆与 prompt_68 后续边界
   - 两个 `gpt-5.4` 子线程只读审计回单
8. 收尾执行：
   - `Park-Slice`

**修改文件**:
- `.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_Day1现存问题总览与整体施工总表.md` - [新增]：Day1 全量 open 问题与整体施工方向
- `.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_施工方向与分阶段清单.md` - [新增]：阶段包与停步条件
- `.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_owner冻结总表.md` - [新增]：owner/003/tests 冻结表
- `.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_给导航_语义同步与执行边界contract.md` - [新增]：导航同步 contract
- `.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_给NPC_协作边界与facade落地prompt.md` - [新增]：NPC 协作 prompt
- `.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_给Day1-V3_整体推进分阶段续工引导prompt_01.md` - [新增]：给本线程自己的续工引导文本
- `.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_给导航_Day1-V3语义同步与执行边界弱引导prompt_01.md` - [新增]：给导航线程的弱引导文本
- `.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_给NPC_Day1-V3协作边界与facade弱引导prompt_01.md` - [新增]：给 NPC 线程的弱引导文本
- `.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/memory.md` - [新增]：本轮子工作区记忆

**当前判断**:
1. 当前 `Day1` 最真实的主问题仍是 owner 错位，不是导航参数或 roam 参数。
2. “只要 NPC 发生真实位移，原则上都应走导航执行合同” 这一条现在应该作为后续边界基线。
3. `NPC` 线程当前已经有足够成熟的 contract 草案，因此 facade 落地应由 `NPC` 主导，而不是 Day1 继续越界深碰。

**遗留问题**:
- [ ] 我还没有在这轮重核 `dinner / Healing / Workbench / 19:30` 的完整代码真相，只能把它们标成“语义已定、当前未重核”。
- [ ] 当前仍未进入 `Package B` 的真实代码退权。
- [ ] 还没把导航与 NPC 的这两份文档真正发出去并收到回执。

## 当前恢复点
1. 如果用户下一步要继续协调线程：
   - 先发给导航和 NPC 这两份新文档
   - 再决定是等回执，还是让 `Day1-V3` 先进入 `Package B`
2. 如果用户下一步要我继续施工：
   - 直接从 `Package B｜Day1 自己退权` 开始
   - 第一刀仍只收 `CrowdDirector` 对 resident 剧情后 lifecycle 的持续 owner

### 会话 2 - 2026-04-15

**用户需求**:
> 请先完整读取 102 的 Day1-V3 续工引导 prompt，按阶段顺序继续推进：先守住总表和边界，再进入 Package B 的真实退权。

**完成任务**:
1. 在 `ACTIVE` slice=`103_packageB_day1_self_deownership` 下进入真实施工，先收 `Package B`：
   - `SpringDay1NpcCrowdDirector` 不再在 opening handoff 后继续持有普通 resident 的 `return-home / resume-roam` 生命周期
   - `SpringDay1Director` 删除 `003` opening 后专用 runtime shim，多点 handoff 调用全部移除
2. `SpringDay1NpcCrowdDirector.cs` 真实改动：
   - `Update()` 提前在 `EnterVillage release latch + 无待放 resident` 时 stand-down，不再先跑 resident owner 热链
   - `ApplyResidentBaseline(...)` 改成：
     - opening/daytime release 后优先一次性 `baseline-release`
     - 只在 `20:00+` 的 clock schedule 窗口继续走 `return-home`
   - shared release path 现在会同时释放：
     - `SpringDay1NpcCrowdDirector`
     - `spring-day1-director`
     这两个 runtime owner，让 `003` 能并回普通 resident 合同
3. `SpringDay1Director.cs` 真实改动：
   - 删除 `ReleaseOpeningThirdResidentControlIfNeeded(...)`
   - 移除所有 `003` 专用 handoff call-site
   - `003` 不再纳入 story actor night-rest runtime 持有
   - Day1 侧 low-level NPC 写口迁移到当前 `NPCAutoRoamController` 已存在的 facade：
     - `AcquireStoryControl`
     - `ReleaseStoryControl`
     - `RequestStageTravel`
     - `RequestReturnToAnchor`
     - `HaltStoryControlMotion`
     - `BindResidentHomeAnchor`
4. `SpringDay1DirectorStagingTests.cs` 改写最贴刀口的测试真相：
   - opening resident 不再保护旧的 crowd-owned `return-home`
   - 新增 `003` 通过 shared resident contract 放手的断言
   - 保留并回归验证 `19:30` 与 `20:00` 的时序边界
5. 为了把编译链打通，额外收了 6 个 Editor 工具的机械 API 迁移：
   - `SetHomeAnchor -> BindResidentHomeAnchor`
   - `ApplyProfile -> SyncRuntimeProfileFromAsset`
   这些文件只做 compile unblock，不引入新语义
6. 已完成的验证：
   - `manage_script validate SpringDay1NpcCrowdDirector` => `errors=0 warnings=2`
   - `validate_script SpringDay1Director` => `owned_errors=0 external_errors=0`，但 CLI 因 Unity `stale_status` 落成 `unity_validation_pending`
   - `validate_script SpringDay1DirectorStagingTests` => `owned_errors=0 external_errors=0`，同样是 `unity_validation_pending`
   - direct MCP 清 console 后，fresh console => `errors=0 warnings=0`
   - 定向 EditMode 通过 6 条：
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldReleaseOpeningResidentToSharedBaselineAfterEnterVillageCrowdReleaseLatches`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldReleaseDirectorOwnedThirdResidentThroughSharedResidentContract`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldNotRetouchAlreadyAutonomousResidentsDuringDaytimeYield`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldNotCancelResidentReturnHomeDuringDaytimeYield`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldKeepResidentsRoamingBeforeTwentyDuringFreeTime`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldQueueReturnHomeAtTwentyDuringFreeTime`
   - touched files `git diff --check` 通过
7. 收尾：
   - `Park-Slice`
   - 当前状态=`PARKED`
   - 当前 blocker=`checkpoint-package-b-day1-self-deownership-ready`

**修改文件**:
- `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs` - [修改]：opening/daytime release 从 crowd-owned return-home 改成 shared baseline release，补 shared runtime owner release
- `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs` - [修改]：删 `003` opening 后专用 shim，迁移 Day1 调用面到 NPC facade
- `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs` - [修改]：重写 opening/003/package-B 直接相关测试
- `Assets/Editor/NPCAutoRoamControllerEditor.cs` - [修改]：Editor 工具 API rename，compile unblock
- `Assets/Editor/NPCPrefabGeneratorTool.cs` - [修改]：Editor 工具 API rename，compile unblock
- `Assets/Editor/NPCSceneIntegrationTool.cs` - [修改]：Editor 工具 API rename，compile unblock
- `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs` - [修改]：Editor 工具 API rename，compile unblock
- `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs` - [修改]：Editor 工具 API rename，compile unblock
- `Assets/Editor/Town/TownNativeResidentMigrationMenu.cs` - [修改]：预存未跟踪文件的 compile unblock 改名

**当前判断**:
1. `Package B` 这一刀已经把 Day1 的两个直接病灶都收到了结构层：
   - `CrowdDirector` 不再继续深持 opening/daytime released residents 的下半生
   - `003` 不再依赖导演专用 shim 才能 handoff
2. 这一刀是结构退权，不是体验终验：
   - 现在能证明的是 owner 关系和回归测试
   - 还不能宣称 live 体感已经完全等同于最终语义
3. `Day1` 调用面已经开始对齐 NPC facade，这为下一步 `NPC / 导航` 线程继续接手执行合同创造了干净入口

**遗留问题**:
- [ ] 还没做 fresh live / profiler 终验，因此不能把结构进展说成体感过线
- [ ] `CrowdDirector` 的 night schedule / recover missing residents 更深层 owner 仍未全部迁出
- [ ] `dinner / navigation execution contract / NPC facade 本体` 仍在后续包

## 2026-04-15｜协作回流：NPC 线程已把 locomotion façade 与低级 public 写口收口到代码
- 当前协作真值：
  - `NPC` 线程已经从“只读合同草案”进入真实施工，并落完第一刀 façade。
- 已确认落地：
  1. `NPCAutoRoamController` 新增 façade：
     - `AcquireStoryControl`
     - `ReleaseStoryControl`
     - `RequestStageTravel`
     - `RequestReturnToAnchor`
     - `RequestReturnHome`
     - `SnapToTarget`
     - `BeginAutonomousTravel`
     - `BeginReturnHome`
     - `ResumeAutonomousRoam`
     - `AbortAndReplan`
  2. resident scripted 低级口已收回控制器内部：
     - `AcquireResidentScriptedControl`
     - `ReleaseResidentScriptedControl`
     - `DriveResidentScriptedMoveTo`
     - `Pause/Resume/HaltResidentScriptedMovement`
  3. `Day1` 侧已迁移到 façade 消费：
     - `SpringDay1Director.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - `SpringDay1DirectorStaging.cs`
  4. `NPC own` 的 formal / informal 聊天冻结链也已切到 façade：
     - `NPCDialogueInteractable.cs`
     - `NPCInformalChatInteractable.cs`
- 当前对 `Day1-V3` 的意义：
  1. 这说明 `Package B｜Day1 自己退权` 现在已经不是“还要先等 NPC 把 façade 做出来”，而是 `NPC` 侧第一刀已到位；
  2. `Day1` 后续应该继续只消费这些 façade，不要再回写 resident scripted 内核；
  3. `NPCMotionController` 的低级 facing / velocity 写口也已补出更高层包装，`Day1` 不应再直接回退去碰旧口。
- 当前缺口：
  - `NPC` 线程这轮拿到了编译 clean + console clean；
  - 但 Unity 的 targeted `run_tests(test_names=...)` 过滤两次都返回 `total=0`，所以还缺一张真正“Unity 发现并跑到了 façade surface test”的证据票。

## 2026-04-15｜Package B 第二刀补记：night/recover 退权已收成稳定 checkpoint
- 本轮 slice：
  - `104_packageB_night_recover_deownership`
  - 已 `Park-Slice`
  - 当前 blocker=`checkpoint-104-night-recover-deownership-ready`
- 本轮先吃进 NPC 线程新回执后的稳定约束：
  1. `NPC locomotion facade` 已真实落地，`Day1` 后续继续退权时只允许消费 facade。
  2. 因此这轮不再给 `CrowdDirector` 新造低级 restart/owner 补丁，而是继续收 Day1 自己剩余的越权尾巴。
- 本轮真实代码收口：
  1. `RecoverReleasedTownResidentsWithoutSpawnStates(...)`
     - 不再在 `_spawnStates == 0` 时顺手重启 resident roam。
     - 改成：只把场上已存在的 resident 重新纳回 `_spawnStates` 视图，并同步 anchor/profile 基线，不碰剧情后 locomotion 生命周期。
  2. `ApplyResidentNightRestState(...)`
     - 不再 `AcquireResidentDirectorControl(...)` 持有夜间 resident。
     - 改成：释放 shared owner 后，通过 `SnapToTarget(...)` 进入 `night-rest` 定位。
  3. `SnapResidentsToHomeAnchorsInternal()`
     - 改为复用 `ApplyResidentNightRestState(...)`，不再保留另一套 night-rest owner 写法。
  4. `FinishResidentReturnHome(...)`
     - 现在即使 `return-home` 合同结束后不续 roam，也会明确释放 shared owner。
  5. `SpringDay1NpcCrowdDirector.cs` 内剩余的 roam restart 尾巴已收口到 facade：
     - `StartRoam / ForceRestartResidentRoam` 不再被 `CrowdDirector` 直接使用。
     - release / baseline / cancel 相关恢复统一走 `ResumeAutonomousRoam(...)`。
- 新增并通过的贴刀口测试：
  1. `CrowdDirector_ShouldRecoverReleasedResidentsIntoTrackedStateWithoutRestartingRoam`
  2. `CrowdDirector_ShouldReleaseSharedOwnerWhenReturnHomeCompletesWithoutRoamResume`
  3. `CrowdDirector_ShouldNotKeepNightRestResidentsUnderCrowdScriptedOwner`
- 本轮验证：
  1. `validate_script SpringDay1NpcCrowdDirector.cs`
     - `owned_errors=0`
     - 仍因 Unity `stale_status` 落成 `unity_validation_pending`
  2. `validate_script SpringDay1DirectorStagingTests.cs`
     - `owned_errors=0`
     - 同样是 `unity_validation_pending`
  3. direct MCP 定向 EditMode 9/9 通过：
     - 上轮 6 条 package-B 既有用例继续通过
     - 新增 3 条 `recover/night-rest/owner-release` 用例通过
  4. 中途出现的 console 错误是 Unity test runner 遗留 `TestResults.xml` 清理尾巴，不是代码回归：
     - 已删除 `C:/Users/aTo/AppData/LocalLow/DefaultCompany/Sunset/TestResults.xml`
     - 已 clear console
     - fresh console=`errors=0 warnings=0`
  5. touched files `git diff --check` 通过
- 当前判断：
  1. `CrowdDirector` 在“剧情后 resident 下半生”这条线上的最脏暗门，已经从：
     - `spawn-state` 掉空后顺手重启身体
     - `night-rest` 时继续持 owner
     - `release` 后继续手搓 restart
     收成了 facade 级语义调用。
  2. 当前还没动的是更深一层的 `clock-owned return-home`：它在 `20:00 -> 到家` 这段仍属于 Day1 schedule 私房 runtime，需要下一刀单独判断是否继续留在 Day1，还是迁到共享 schedule / navigation 合同。
- 当前恢复点：
  1. 如果继续施工，下一刀只看：
     - `TryBeginResidentReturnHome / TickResidentReturnHome / snapshot-restore` 是否还在不该由 Day1 持有的 schedule owner 链上
  2. 不要回退去补 UI / dinner / Primary / navigation core。

## 2026-04-15｜Package B 第三刀补记：clock-owned return-home 的 snapshot 与到家语义已收口
- 本轮 slice：
  - `105_packageB_clock_return_owner_cut`
  - 已 `Park-Slice`
  - 当前 blocker=`checkpoint-105-clock-return-owner-cut-ready`
- 本轮切口：
  1. 不直接去碰导航 core，也不在这轮强拆 `20:00 return-home` 的最终 owner。
  2. 先把这段合同里最容易制造“Day1 继续代管 resident 下半生”的两处尾巴收干净：
     - 到家后又恢复 roam
     - 跨场景还恢复旧 `return-home` owner
- 本轮真实代码改动：
  1. `TryBeginResidentReturnHome(...)`
     - `20:00` 的 `clock-owned return-home` 现在明确设置 `ResumeRoamAfterReturn=false`
     - 含义改成：回家并停住，而不是到家后又恢复 free-roam
  2. `ShouldPersistResidentReturnHomeSnapshot(...)`
     - 改成不再持久化 `return-home` snapshot
  3. `ShouldRestoreResidentReturnHomeSnapshot(...)`
     - 改成不再恢复 `return-home` snapshot
     - 场景切换后的 `20:00` 回家语义应由时序重新下发，不再靠 Day1 持旧 owner 快照续命
- 新增并通过的贴刀口测试：
  1. `CrowdDirector_ShouldIgnoreClockOwnedReturnHomeSnapshotAtTwenty`
  2. `CrowdDirector_ShouldQueueClockReturnHomeWithoutRoamResumeAtTwenty`
- 本轮验证：
  1. `validate_script SpringDay1NpcCrowdDirector.cs`
     - `owned_errors=0`
     - `unity_validation_pending`（仍受 Unity `stale_status` / CodeGuard timeout 影响）
  2. `validate_script SpringDay1DirectorStagingTests.cs`
     - `owned_errors=0`
     - 同样 `unity_validation_pending`
  3. direct MCP EditMode 两轮定向通过：
     - 第一轮 11/11 passed：
       - `Clock return-home` 新用例 2 条
       - `recover / night-rest / owner-release` 既有新用例
       - `Package B` opening/daytime 回归用例
     - 第二轮 4/4 passed：
       - `snapshot` 相关旧回归 3 条
       - `ClockOwned return-home snapshot` 新用例 1 条
  4. 每次 run_tests 后 Unity test runner 都会再留 `TestResults.xml` 清理尾巴；本轮已两次清掉并 clear console
  5. fresh console 最终=`errors=0 warnings=0`
  6. touched files `git diff --check` 通过
- 当前判断：
  1. 现在 `20:00 return-home` 已经不再是“到家又 roam + 跨场景恢复旧 owner”的混合脏合同。
  2. 这让下一刀可以更专注判断：
     - `TryDriveResidentReturnHome / TickResidentReturnHome` 这条执行 owner，到底还该不该留在 `Day1`。
- 当前恢复点：
  1. 如果继续下一刀，只看：
     - `TryDriveResidentReturnHome`
     - `TickResidentReturnHome`
     - `HasActiveResidentReturnHomeDrive`
     这三处是否还能进一步从 `CrowdDirector` 退权。
  2. 不要扩到 dinner / UI / Primary / navigation core。

## 2026-04-15｜Package B 第四刀补记：106 live 回归修正与语义改口 checkpoint
- 本轮 slice：
  - `106_live-fix_opening-primary-dinner-regressions`
  - 已 `Park-Slice`
  - 当前 blocker=`checkpoint-106-live-regressions-fixed-awaiting-user-retest`
- 本轮先吃进用户最新 live 真相：
  1. `opening` 期间 `001~003` 走到终点后又被打回起点。
  2. opening 后白天 resident 不再要求回 anchor；`20:00` 前都应自由漫游。
  3. `Primary` 不该一进场就冻结时间；`0.0.5` 完成前只允许卡到 `16:00`。
  4. `0.0.6` 回 `Town` 前晚饭窗口里，`003~203` 会罚站。
  5. `dinner` 必须和 opening 同一 staged contract：进对白前只走一次，不得再 reset 回 start。
- 本轮真实代码改动：
  1. `SpringDay1Director.cs`
     - `TryHandleTownEnterVillageFlow()` 在 `VillageGate` 对白 active 时不再清空 `_townVillageGateActorsPlaced`，避免 `001/002/003` 被重新当成“首次摆位”打回起点。
     - `BeginDinnerConflict()` 拆开 dinner 的 `wait reset` 与 `placed reset`：
       - dinner dialogue active 时不再重复 `PrepareDinnerStoryActorsForDialogue()`
       - queue 正式对白前只清等待计时，不再清 `_dinnerStoryActorsPlaced`
       - `EvaluateDinnerCueStartPermission()` 改成和 opening 一致：超时后即使 `ForceSettleBeatCue()` 失败，也不再无限等待。
     - `ShouldPauseStoryTimeForCurrentPhase()` 改成：
       - `HealingAndHP / WorkbenchFlashback / FarmingTutorial` 不再暂停时间
     - `GetManagedMaximumTotalMinutesForPhase()` 改成白天主线在 `16:00` 即锁住，不再放到 `16:59`
     - `GetCurrentBeatKey()` 在 `_postTutorialExploreWindowEntered` 时不再继续返回 `FarmingTutorial_Fieldwork`，而是改成 `FreeTime_NightWitness`
  2. `SpringDay1NpcCrowdDirector.cs`
     - `ShouldYieldDaytimeResidentsToAutonomy()` 那条白天早退分支新增 `ShouldResyncBeforeSkippingDaytimeAutonomyYield()`：
       - 只要 `_syncRequested=true`
       - 或 `_spawnStates` 里还残留空实例 / 旧 continuity 壳
       - 就不允许因为“没有 pending daytime release”而直接 `return`
     - `ShouldAllowResidentReturnHome()` 改成只认 `20:00 <= hour < 21:00`
       - `21:00+` 的 `rest` 不再被 helper 误当成 `return-home` 延长期
  3. `SpringDay1DirectorStagingTests.cs`
     - 新增 / 改写 5 条贴刀口回归：
       - `Director_ShouldKeepVillageGateActorsPlacedWhileVillageGateDialogueActive`
       - `Director_ResetDinnerCueWaitState_ShouldPreservePlacedStoryActors`
       - `Director_ShouldKeepStoryTimeRunningDuringPrimaryPhases`
       - `Director_ShouldExposeFreeRoamBeatDuringPostTutorialExploreWindow`
       - `CrowdDirector_ShouldResyncRecoveredResidentsBeforeSkippingDaytimeYield`
       - 并补 `CrowdDirector_ShouldNotTreatTwentyOneAsReturnHomeContractWindow`
- 本轮验证：
  1. `validate_script SpringDay1Director.cs`
     - `owned_errors=0`
     - `unity_validation_pending`（原因仍是 `stale_status / CodeGuard timeout`）
  2. `validate_script SpringDay1NpcCrowdDirector.cs`
     - `owned_errors=0`
     - `unity_validation_pending`
  3. `validate_script SpringDay1DirectorStagingTests.cs`
     - `owned_errors=0`
     - `unity_validation_pending`
  4. 定向 EditMode 15/15 passed：
     - staging 单测 11 条
     - opening/dinner runtime bridge 4 条
  5. fresh console 最终=`errors=0 warnings=0`
  6. `git diff --check -- [own files]` 通过
- 当前判断：
  1. 这轮收掉的是用户 live 直接打脸的第一层 runtime regressions，不是继续包装结构成果。
  2. `opening / dinner / Primary 时间 / Town re-sync` 这四块现在已经重新对齐用户语义。
  3. `20:00 -> 到家` 的 formal-navigation arrival owner 仍在 `CrowdDirector.TickResidentReturnHome()` 里，当前不安全继续硬拔；因为 `NPCAutoRoamController` 到家后还不会自己 release formal-navigation owner。
- 当前恢复点：
  1. 下一轮若继续 owner 退权，只该看：
     - `TryBeginResidentReturnHome`
     - `TickResidentReturnHome`
     - `FinishResidentReturnHome`
     三处如何迁成更像“边缘发指令 + 到点收尾”的合同
  2. 本轮之后最该做的是用户 fresh live retest，不要把这 15 条定向测试冒充成体验终验。
