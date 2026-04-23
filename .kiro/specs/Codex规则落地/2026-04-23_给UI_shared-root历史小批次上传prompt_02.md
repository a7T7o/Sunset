请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root历史小批次上传分发批次_02.md]

这轮不要继续修 UI 体验，也不要回到第一波“非代码先交、剩下整包再说”的口径。

你当前唯一主线固定为：
只按过去实际开发历史，给 `UI` 再还原 `1` 个最小历史小批次上传；这轮只允许这一小批，撞 blocker 就停车，不换第二批。

你必须先继承并且不要推翻的当前真状态：
1. 你第一波已真实 push：
   - `ea6ac827` `2026.04.23_UI_01`
   - `edd3baea` `2026.04.23_UI_02`
   - `9050f74b` `2026.04.23_UI_03`
2. 你当前 `thread-state = PARKED`
3. 第一波真实剩余仍在：
   - `Assets/YYY_Scripts/Story/UI/*`
   - `Assets/YYY_Scripts/UI/*`
4. 用户最新改判不是“把所有 clearly-own UI 代码一起交”，而是“按过去每一小刀历史慢慢传”。

这轮唯一允许的小批次固定为：
只审并尝试上传下面这组新增 tab/runtime-kit 小批，不准扩到 `Inventory / Toolbar / Box / Story/UI` 其它链：

- `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs`
- `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs.meta`
- `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
- `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs.meta`
- `Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs`
- `Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs.meta`

这轮必须按顺序执行：
1. 先确认这 3 个新增 panel/runtime-kit 是否属于同一真实历史小批。
2. 只对白名单这一组跑真实上传尝试。
3. 如果 `CodexCodeGuard` / preflight 仍然在这组上 hang 或 no JSON，立刻停车。
4. 不准因为被卡就顺手吞：
   - `Assets/YYY_Scripts/UI/Inventory/*`
   - `Assets/YYY_Scripts/UI/Toolbar/*`
   - `Assets/YYY_Scripts/UI/Box/*`
   - `Assets/YYY_Scripts/Story/UI/*`
   - `PackagePanelTabsUI.cs`
5. 这轮不准继续第二个小批次。

完成定义只有两种：
1. 这组新 panel/runtime-kit 小批真实提交并 push 到 `origin`
2. 或者你把这一个小批次的 exact blocker 报死

最终回执必须额外明确：
1. 这组小批是不是独立历史批次
2. 这组小批有没有真实提交成功
3. 如果失败，第一真实 blocker 到底是 `CodeGuard` 还是 same-root mixed
4. 除这组之外，其它 UI 代码尾账先不动

[thread-state 接线要求｜本轮强制]
如果你这轮会继续真实上传尝试，先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `sync` 前，必须先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住或收口，跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
