请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root第二波blocker分流批次_03.md]

不要再重跑你刚刚已经执行完的 `prompt_02`。

你当前唯一主线固定为：
承认 `Editor/Story` 那组 validation/probe 新文件小批已经撞死；这轮改成 `Assets/Editor/Story` 根内整合批的唯一上传尝试，把上一刀已经证实挡路的 7 个旧改文件正式纳入，但仍然绝不扩到 `Managers / Directing / Tests`。

你必须先继承并且不要推翻的当前真状态：
1. 你上一刀 `prompt_02` 已经完成，不准重复再撞一次。
2. 当前 `thread-state = PARKED`
3. 上一刀第一真实 blocker 已钉死为：
   - `Assets/Editor/Story` 同根 still dirty
4. 当前 exact blocker files 已明确是：
   - [DialogueDebugMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/DialogueDebugMenu.cs)
   - [SpringDay1DirectorPrimaryLiveCaptureMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1DirectorPrimaryLiveCaptureMenu.cs)
   - [SpringDay1DirectorPrimaryRehearsalBakeMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1DirectorPrimaryRehearsalBakeMenu.cs)
   - [SpringDay1DirectorStagingWindow.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1DirectorStagingWindow.cs)
   - [SpringDay1DirectorTownContractMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1DirectorTownContractMenu.cs)
   - [SpringDay1TargetedEditModeTestMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs)
   - [SpringUiEvidenceMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringUiEvidenceMenu.cs)

这轮唯一允许的切片固定为：
1. 上一刀已经试过的 8 组新文件：
   - `SpringDay1ActorRuntimeProbeMenu`
   - `SpringDay1LatePhaseValidationMenu`
   - `SpringDay1LiveSnapshotArtifactMenu`
   - `SpringDay1MiddayOneShotPersistenceTestMenu`
   - `SpringDay1NativeFreshRestartMenu`
   - `SpringDay1ResidentControlProbeMenu`
   - `SunsetPlayModeStartSceneGuard`
   - `SunsetValidationSessionCleanupMenu`
   - 以及各自 `.meta`
2. 再加上上面那 7 个已修改旧文件

这轮必须按顺序执行：
1. 先确认这批现在能不能诚实视作 `Editor/Story` 根内整合批，而不是今天为了过根硬凑的包。
2. 只对白名单这一组跑一次真实上传尝试。
3. 如果这组一进白名单后，`Assets/Editor/Story` 根里还有新的 remaining dirty / mixed 没被覆盖：
   - 立刻停车
   - 只报新的 exact blocker
4. 不准因此顺手吞：
   - [SpringDay1Director.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
   - [SpringDay1DirectorStaging.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs)
   - [SpringDay1DirectorStagingTests.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
   - 任意 `SaveManager* / StoryProgressPersistenceService* / Workbench* / Chest*` tests
5. 不准继续第二个切片。

这轮完成定义只有两种：
1. 这组 `Editor/Story` 根内整合批真实提交并 push 到 `origin`
2. 或者你把新的 exact blocker 报死

最终回执必须额外明确：
1. 这批现在是否正式升级为 `Editor/Story` 根内整合批
2. 这组有没有提交成功
3. 如果还失败，新的第一真实 blocker 是什么
4. 这轮没有越权扩到 `Managers / Directing / Tests`

[thread-state 接线要求｜本轮强制]
如果你这轮会继续真实上传尝试，先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `sync` 前，必须先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住或收口，跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
