请先完整读取：
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-13_给典狱长_shared-root剩余大头拆分与owner定责矩阵_01.md]

这轮不要继续扩 NPC 新行为，也不要回 spring-day1 大剧情或 UI 外观。

你当前唯一主线固定为：
只收你在 shared-root 里剩余的 `NPC editor / NPC runtime / Npc tests` own 尾账，直到你能明确回答“我 own 的这批根现在是否 clean”。

这轮至少要覆盖并盘清：
- `Assets/Editor/NPC/*`
- `Assets/YYY_Scripts/Controller/NPC/*`
- `Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs`
- `Assets/YYY_Scripts/Data/NPCDialogueContentProfile.cs`
- `Assets/YYY_Scripts/Data/NPCRoamProfile.cs`
- `Assets/YYY_Tests/Editor/Npc*`

完成定义：
1. 只收 NPC own，不吞 `spring-day1 / UI / 树石 / Codex工具`。
2. 重新核当前 parked 现场，不要假设之前的 owner 判断永远正确。
3. 给出最小白名单。
4. 能提就提；不能提就把 blocker 压成最小并报实 `当前 own 路径是否 clean`。
5. 严守 no-red 和 `thread-state`。

回卡最小格式：
- 已回写文件路径
- 是否完成本轮要求
- 当前 own 路径是否 clean
- 一句话摘要
