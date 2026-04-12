请先完整读取：
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-13_给典狱长_shared-root剩余大头二级拆分与owner定责矩阵_02.md]

这轮不要继续做新的视觉 polish，也不要回 Day1 全剧情或 NPC 群像。

你当前唯一主线固定为：
只收你在 shared-root 里剩余的 UI own 尾账，重点已经不只是一张 `InteractionHintOverlay`，而是 `Story/UI + Package tabs + Bubble + 字体材质` 整组。

这轮至少要覆盖并盘清：
- `Assets/YYY_Scripts/Story/UI/*`
- `Assets/YYY_Scripts/UI/Tabs/*`
- `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
- `Assets/YYY_Tests/Editor/PackagePanel*`
- `Assets/YYY_Tests/Editor/PlayerNpcConversationBubbleLayoutTests.cs`
- `Assets/YYY_Tests/Editor/PlayerThoughtBubblePresenterStyleTests.cs`
- `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset`

完成定义：
1. 只收 UI own，不吞 `spring-day1 / NPC / 树石 / TownHome 基线`。
2. 先按 `02` 版矩阵重审 owner，而不是沿用只盯 `PackagePanelTabsUI` 的旧口径。
3. 给出这轮自己的白名单。
4. 能提就提；不能提就把 blocker 压成最小并报实 `当前 own 路径是否 clean`。
5. 严守 no-red。

[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回卡最小格式：
- 已回写文件路径
- 是否完成本轮要求
- 当前 own 路径是否 clean
- 一句话摘要
