请先完整读取：
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-13_给典狱长_shared-root剩余大头二级拆分与owner定责矩阵_02.md]

这轮不要继续扩 Day1 正文，不要回 NPC/UI 总历史，也不要把“Story 目录里剩下的都算我的”。

你当前唯一主线固定为：
只收你在 shared-root 里剩余的 `spring-day1 / Story-Editor / Story runtime bridge / SpringDay1 tests` own 尾账，直到你能明确回答“我 own 的这批根现在是否 clean”。

这轮至少要覆盖并盘清：
- `Assets/Editor/Story/*SpringDay1*`
- `Assets/Editor/Story/DialogueDebugMenu.cs`
- `Assets/Editor/Story/SpringUiEvidenceMenu.cs`
- `Assets/YYY_Scripts/Story/Directing/SpringDay1*`
- `Assets/YYY_Scripts/Story/Managers/SpringDay1*`
- `Assets/YYY_Scripts/Story/Managers/DialogueManager.cs`
- `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
- `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
- `Assets/YYY_Tests/Editor/SpringDay1*`

完成定义：
1. 先按 `02` 版矩阵重审 owner，不要沿用旧的粗分组。
2. 只收 `spring-day1` own，不吞 `NPC / UI / 树石 / TownHome 基线`。
3. 给出这轮真实白名单。
4. 能提就提；不能提就把 blocker 压成最小，并报实 `当前 own 路径是否 clean`。
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
