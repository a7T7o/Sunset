请先完整读取：
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-13_给典狱长_shared-root剩余大头拆分与owner定责矩阵_01.md]

这轮不要继续做新的视觉 polish，也不要回 Day1 全剧情或 NPC 群像。

你当前唯一主线固定为：
只收你在 shared-root 里剩余的 UI own 尾账，重点是 `InteractionHintOverlay / PackagePanelTabsUI / UI tests / UI docs` 这条链。

这轮至少要覆盖并盘清：
- `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
- `Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
- `Assets/YYY_Tests/Editor/PackagePanelLayoutGuardsTests.cs`
- `Assets/YYY_Tests/Editor/PlayerThoughtBubblePresenterStyleTests.cs`
- `Assets/YYY_Tests/Editor/PlayerNpcConversationBubbleLayoutTests.cs`

完成定义：
1. 只收 UI own，不吞 `spring-day1 / NPC / 树石 / 存档系统`。
2. 先重新核当前 parked 后现场，而不是默认旧 slice 仍然准确。
3. 给出这轮自己的白名单。
4. 能提就提；不能提就报实 blocker 和 `当前 own 路径是否 clean`。
5. 严守 no-red 和 `thread-state`。

回卡最小格式：
- 已回写文件路径
- 是否完成本轮要求
- 当前 own 路径是否 clean
- 一句话摘要
