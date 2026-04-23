请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root历史小批次上传分发批次_02.md]

这轮不要继续补存档逻辑，也不要回到第一波“docs 交完后再看整包代码”的口径。

你当前唯一主线固定为：
只按过去实际开发历史，给 `存档系统` 再还原 `1` 个最小历史小批次上传；这轮只允许这一小批，撞 blocker 就停车，不换第二批。

你必须先继承并且不要推翻的当前真状态：
1. 你第一波已真实 push：
   - `90e95fc7` `2026.04.23_存档系统_01`
   - `0d95e302` `2026.04.23_存档系统_02`
2. 你当前 `thread-state = PARKED`
3. 第一波 docs/prompt/memory 已 clean
4. 当前真实剩余 blocker 仍在：
   - `Assets/YYY_Scripts/Data/Core/InventoryItem.cs`
   - `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
   - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - `StoryProgressPersistenceService.cs` 与 5 个 tests 仍按 shared/mixed 保留
5. 用户最新改判是“按小历史批次慢慢传”，不是“Data/Core 一扩就顺手吞 mixed”。

这轮唯一允许的小批次固定为：
只处理 `InventoryItem.cs + SaveDataDTOs.cs + SaveManager.cs` 这组 `Data/Core` 小批；不要扩到 `StoryProgressPersistenceService` 或任何 editor tests。

这轮必须按顺序执行：
1. 先确认这 3 个文件是不是同一真实历史小批。
2. 只对白名单这一组跑真实上传尝试。
3. 如果 `CodexCodeGuard` / preflight 仍然挂死在这组上，立刻停车。
4. 不准因为被卡就顺手吞：
   - `StoryProgressPersistenceService.cs`
   - `SaveManagerDay1RestoreContractTests.cs`
   - `SaveManagerDefaultSlotContractTests.cs`
   - `StoryProgressPersistenceServiceTests.cs`
   - `WorkbenchInventoryRefreshContractTests.cs`
   - `SpringDay1DirectorStagingTests.cs`
5. 这轮不准继续第二个小批次。

完成定义只有两种：
1. 这组 `Data/Core` 小批真实提交并 push 到 `origin`
2. 或者你把这一个小批次的 exact blocker 报死

最终回执必须额外明确：
1. 这 3 个文件是不是独立历史批次
2. 这组小批有没有真实提交成功
3. 如果失败，第一真实 blocker 到底是不是 `CodeGuard`
4. 除这组之外，其它存档尾账先不动

[thread-state 接线要求｜本轮强制]
如果你这轮会继续真实上传尝试，先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `sync` 前，必须先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住或收口，跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
