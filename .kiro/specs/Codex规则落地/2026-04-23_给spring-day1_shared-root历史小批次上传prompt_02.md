请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root历史小批次上传分发批次_02.md]

这轮不要继续做 `Day1 runtime`，也不要回到“把 clearly-own 全交了”的第一波口径。

你当前唯一主线固定为：
只按过去实际开发历史，给 `spring-day1` 再还原 `1` 个最小历史小批次上传；这轮只允许这一小批，撞 blocker 就停车，不换第二批。

你必须先继承并且不要推翻的当前真状态：
1. 你第一波已真实 push：
   - `8f1909da` `2026.04.23_spring-day1_01`
   - `950886bf` `2026.04.23_spring-day1_02`
2. 你当前 `thread-state = PARKED`
3. 第一波真实剩余 blocker 仍在：
   - `Assets/Editor/Story/*`
   - `Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs`
   - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
   - `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
   - 同根混着 `StoryProgressPersistenceService` 与多组 `SaveManager / Workbench / Chest` tests
4. 用户最新改判不是“继续把能交的都交”，而是“按过去每一小刀开发历史慢慢还原批次上传”。

这轮唯一允许的小批次固定为：
只审并尝试上传 `Assets/Editor/Story` 下那组“新增 menu / probe / snapshot / cleanup”历史小批，不准扩到 `Managers / Directing / YYY_Tests`。

优先只看这组文件：
- `Assets/Editor/Story/SpringDay1ActorRuntimeProbeMenu.cs`
- `Assets/Editor/Story/SpringDay1ActorRuntimeProbeMenu.cs.meta`
- `Assets/Editor/Story/SpringDay1LatePhaseValidationMenu.cs`
- `Assets/Editor/Story/SpringDay1LatePhaseValidationMenu.cs.meta`
- `Assets/Editor/Story/SpringDay1LiveSnapshotArtifactMenu.cs`
- `Assets/Editor/Story/SpringDay1LiveSnapshotArtifactMenu.cs.meta`
- `Assets/Editor/Story/SpringDay1MiddayOneShotPersistenceTestMenu.cs`
- `Assets/Editor/Story/SpringDay1MiddayOneShotPersistenceTestMenu.cs.meta`
- `Assets/Editor/Story/SpringDay1NativeFreshRestartMenu.cs`
- `Assets/Editor/Story/SpringDay1NativeFreshRestartMenu.cs.meta`
- `Assets/Editor/Story/SpringDay1ResidentControlProbeMenu.cs`
- `Assets/Editor/Story/SpringDay1ResidentControlProbeMenu.cs.meta`
- `Assets/Editor/Story/SunsetPlayModeStartSceneGuard.cs`
- `Assets/Editor/Story/SunsetPlayModeStartSceneGuard.cs.meta`
- `Assets/Editor/Story/SunsetValidationSessionCleanupMenu.cs`
- `Assets/Editor/Story/SunsetValidationSessionCleanupMenu.cs.meta`

这轮必须按顺序执行：
1. 先确认这组是不是一个真实历史小批，而不是你临时拼出来的大包。
2. 只对白名单这一组跑真实上传尝试。
3. 如果 `CodexCodeGuard` / preflight 仍然在这组上不返回稳定 JSON，立刻停车。
4. 不准因为被卡就改去吞：
   - `SpringDay1Director.cs`
   - `SpringDay1DirectorStaging.cs`
   - `SpringDay1DirectorStagingTests.cs`
   - `StoryProgressPersistenceService.cs`
   - 任意 `SaveManager*` / `Workbench*` / `Chest*` tests
5. 这轮不准继续做第二个小批次。

完成定义只有两种：
1. 这组 menu/probe 小批真实提交并 push 到 `origin`
2. 或者你把这一个小批次的 exact blocker 报死

最终回执必须额外明确：
1. 这组小批到底是不是独立历史批次
2. 这组小批有没有真实提交成功
3. 如果失败，第一真实 blocker 到底是 `CodeGuard` 还是 own-root / mixed
4. 除这组之外，剩余 `spring-day1` 尾账先不动

[thread-state 接线要求｜本轮强制]
如果你这轮会继续真实上传尝试，先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `sync` 前，必须先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住或收口，跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
