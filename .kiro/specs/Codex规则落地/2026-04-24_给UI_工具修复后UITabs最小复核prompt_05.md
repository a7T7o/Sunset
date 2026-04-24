请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-24_统一工具incident修复后最小复核分发批次_05.md]

这轮不要继续修 `UI/Tabs` 业务代码。

你当前唯一主线固定为：
在治理位已经修过 `CodexCodeGuard / git-safe-sync` 的前提下，只对白名单 `UI/Tabs` 七文件做 `1` 次真实最小复核，确认这条线现在吐出的已经是“真实 blocker / 或真实可上传”，而不是旧的 `CodexCodeGuard returned no JSON`。

你必须先继承并且不要推翻的当前真状态：
1. `PackagePanelTabsUI.cs` 已正式承认为这批核心件。
2. `UI/Tabs` 这 `7` 个文件当前已经覆盖这批的同根脏改，不再是“批外漏项”问题。
3. 上一轮这条线停在工具 incident，不是 same-root 边界没收清。
4. 治理位当前已先修两处工具点：
   - [Program.cs](/D:/Unity/Unity_learning/Sunset/scripts/CodexCodeGuard/Program.cs)
   - [git-safe-sync.ps1](/D:/Unity/Unity_learning/Sunset/scripts/git-safe-sync.ps1)
5. 治理位本地代表性复核已经证明：
   - 这组白名单现在不会再卡成 `CodexCodeGuard returned no JSON`
   - 当前它会稳定返回真实代码闸门结果
   - 代表性结果是 `3` 条错误、`1` 条警告
6. 这并不等于你可以跳过真实线程复核；这轮仍要由你自己跑一次真实尝试。

这轮唯一允许的白名单固定为：
- [PackagePanelTabsUI.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs)
- [PackageMapOverviewPanel.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs)
- [PackageMapOverviewPanel.cs.meta](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs.meta)
- [PackageNpcRelationshipPanel.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
- [PackageNpcRelationshipPanel.cs.meta](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs.meta)
- [PackagePanelRuntimeUiKit.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs)
- [PackagePanelRuntimeUiKit.cs.meta](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs.meta)

这轮明确不准做的事：
1. 不准顺手修 `UI/Tabs` 里的真实代码错误或 warning。
2. 不准扩到：
   - `Assets/YYY_Scripts/UI/Inventory/*`
   - `Assets/YYY_Scripts/UI/Toolbar/*`
   - `Assets/YYY_Scripts/UI/Box/*`
   - `Assets/YYY_Scripts/Story/UI/*`
3. 不准因为这轮复核成功进入真实 blocker，就顺手开第二个切片。

这轮必须按顺序执行：
1. 先基于当前工具修复后的 `main`，对这 `7` 文件跑 `1` 次真实上传尝试。
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
1. 这轮是否还会出现 `CodexCodeGuard returned no JSON / baseline_fail 黑盒`
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
