请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-24_统一工具incident修复后最小复核分发批次_05.md]

这轮不要继续修 `Data/Core` 业务代码。

你当前唯一主线固定为：
在治理位已经修过 `CodexCodeGuard / git-safe-sync` 的前提下，只对白名单 `InventoryItem.cs + SaveDataDTOs.cs + SaveManager.cs` 做 `1` 次真实最小复核，确认这条线现在吐出的已经是“真实 blocker / 或真实可上传”，而不是旧的工具黑盒挂死。

你必须先继承并且不要推翻的当前真状态：
1. `Data/Core` 这 `3` 个文件仍视作同一历史小批。
2. 上一轮这条线停在工具 incident，不是停在业务边界不清。
3. 治理位当前已先修两处工具点：
   - [Program.cs](/D:/Unity/Unity_learning/Sunset/scripts/CodexCodeGuard/Program.cs)
   - [git-safe-sync.ps1](/D:/Unity/Unity_learning/Sunset/scripts/git-safe-sync.ps1)
4. 治理位本地代表性复核已经证明：
   - 这组白名单现在不会再卡成 `no JSON / hang`
   - 当前它会稳定返回真实代码闸门结果
   - 当前代表性结果是 [SaveManager.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs) 上 `4` 条 `CS1061`
5. 这并不等于你可以跳过真实线程复核；这轮仍要由你自己跑一次真实尝试，把线程自己的 blocker 收实。

这轮唯一允许的白名单固定为：
- [InventoryItem.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/InventoryItem.cs)
- [SaveDataDTOs.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs)
- [SaveManager.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs)

这轮明确不准做的事：
1. 不准顺手修 `SaveManager.cs` 里的真实编译错误。
2. 不准扩到：
   - [StoryProgressPersistenceService.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs)
   - 任意存档相关 tests
3. 不准因为这轮复核成功进入真实 blocker，就顺手开第二个切片。

这轮必须按顺序执行：
1. 先基于当前工具修复后的 `main`，对这 `3` 文件跑 `1` 次真实上传尝试。
2. 如果现在已经能通过 `Ready-To-Sync / preflight`：
   - 继续走到你这轮真实该到的位置
   - 如果最后能提交就提交；如果最后被新的 compile / root / sync blocker 挡住，就停在新的第一真实 blocker
3. 如果仍然被工具层挡住：
   - 只报新的、比上轮更精确的 exact blocker
4. 无论结果如何，这轮都只允许这 `1` 次，不准继续修代码。

这轮完成定义只有两种：
1. 这组现在成功进入真实上传流程，并给出真实业务结果
2. 或者你把这组在工具修复后的新的第一真实 blocker 报死

最终回执必须额外明确：
1. 这轮是否还会出现 `CodexCodeGuard / no JSON / hang`
2. 如果不再出现，新的第一真实 blocker 是什么
3. 这轮有没有顺手修业务代码：必须明确回答
4. 当前这组是“可以继续上传”，还是“已降级成真实业务 blocker”

[thread-state 接线要求｜本轮强制]
如果你这轮继续真实复核，先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `sync` 前，必须先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住或收口，跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
