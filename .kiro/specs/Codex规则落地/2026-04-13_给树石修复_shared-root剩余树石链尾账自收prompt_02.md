请先完整读取：
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-13_给典狱长_shared-root剩余大头二级拆分与owner定责矩阵_02.md]

这轮不要再只试 stone-only 四文件，也不要再停在“same-root blocked”的解释层。

你当前唯一主线固定为：
把 `树 / 石 / batch tool / 对应 editor` 这组 own 尾账成组收口，直到你能明确回答“我 own 的这批根现在是否 clean”。

这轮至少要覆盖并盘清：
- `Assets/Editor/StoneControllerEditor.cs`
- `Assets/Editor/TreeControllerEditor.cs`
- `Assets/Editor/Tool_004_BatchTreeState.cs`
- `Assets/Editor/Tool_005_BatchStoneState.cs`
- `Assets/YYY_Scripts/Controller/StoneController.cs`
- `Assets/YYY_Scripts/Controller/TreeController.cs`

完成定义：
1. 不再假装只提 stone 4 文件就能过线；要正面处理 same-root remaining dirty。
2. 只收树石 own，不吞 `NPC / spring-day1 / UI / TownHome 基线`。
3. 给出这轮真实白名单。
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
