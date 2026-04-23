请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root第二波blocker分流批次_03.md]

不要再重跑你刚刚已经执行完的 `prompt_02`。

你当前唯一主线固定为：
承认 `prompt_02` 那个 tabs 小批已经撞死；这轮改成 `UI/Tabs` 根内整合小批的唯一上传尝试，把已确认属于同批核心件的 `PackagePanelTabsUI.cs` 正式纳入，不再把它当批外残留。

你必须先继承并且不要推翻的当前真状态：
1. 你上一刀 `prompt_02` 已经完成，不准重复再撞一次。
2. 当前 `thread-state = PARKED`
3. 你上一刀的 blocker 不是 `CodeGuard`，而是：
   - `Assets/YYY_Scripts/UI/Tabs` 根内仍有 `PackagePanelTabsUI.cs` 这个同根旧改文件在白名单之外
4. 当前代码事实已经被治理位补核：
   - [PackagePanelTabsUI.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs) 里直接 `EnsureOptionalPanelInstalled("PackageMapOverviewPanel")`
   - 也直接 `EnsureOptionalPanelInstalled("PackageNpcRelationshipPanel")`
   - 所以它不是外围噪音，而大概率就是这批 tabs/runtime-kit 的核心集成件

这轮唯一允许的切片固定为：
- [PackagePanelTabsUI.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs)
- [PackageMapOverviewPanel.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs)
- [PackageMapOverviewPanel.cs.meta](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs.meta)
- [PackageNpcRelationshipPanel.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
- [PackageNpcRelationshipPanel.cs.meta](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs.meta)
- [PackagePanelRuntimeUiKit.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs)
- [PackagePanelRuntimeUiKit.cs.meta](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs.meta)

这轮必须按顺序执行：
1. 先确认这 4 份代码主体现在能不能诚实视作同一历史整合批，而不是今天临时扩包。
2. 只对白名单这一组跑一次真实上传尝试。
3. 如果这组一进白名单后，`Assets/YYY_Scripts/UI/Tabs` 仍有新的 remaining dirty / mixed 没被覆盖：
   - 立刻停车
   - 只报新的 exact blocker
4. 不准因为这次扩包就顺手吞：
   - `Assets/YYY_Scripts/UI/Inventory/*`
   - `Assets/YYY_Scripts/UI/Toolbar/*`
   - `Assets/YYY_Scripts/UI/Box/*`
   - `Assets/YYY_Scripts/Story/UI/*`
5. 不准继续第二个切片。

这轮完成定义只有两种：
1. 这组 `UI/Tabs` 根内整合批真实提交并 push 到 `origin`
2. 或者你把这组新的 exact blocker 报死

最终回执必须额外明确：
1. 你是否正式承认 `PackagePanelTabsUI.cs` 属于这批核心件
2. 这组根内整合批有没有提交成功
3. 如果还失败，新的第一真实 blocker 是什么
4. 这轮没有越权扩到 `Inventory / Toolbar / Story/UI`

[thread-state 接线要求｜本轮强制]
如果你这轮会继续真实上传尝试，先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `sync` 前，必须先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住或收口，跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
