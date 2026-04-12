请先完整读取：
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-13_给典狱长_shared-root剩余大头拆分与owner定责矩阵_01.md]

这轮不要再扩 Day1 正文、不要再开新剧情功能，也不要回 UI / NPC / Town 总历史。

你当前唯一主线固定为：
只收你在 shared-root 里剩余的 `Story / SpringDay1 / Editor-Story / SpringDay1 tests` own 尾账，直到能给出“我 own 的这批根现在是否 clean”的明确答案。

这轮至少要覆盖并盘清：
- `Assets/Editor/Story/*SpringDay1*`
- `Assets/YYY_Scripts/Story/**/*SpringDay1*`
- `Assets/YYY_Scripts/Story/Managers/DialogueManager.cs`
- `Assets/YYY_Scripts/Story/Managers/StoryManager.cs`
- `Assets/YYY_Tests/Editor/SpringDay1*`

完成定义：
1. 先重新核 owner 与 shared-root 现实，不要默认所有 `Story` 都归你。
2. 只收你 own 的 `Story / SpringDay1` 链，不吞 `NPC / UI / 树石`。
3. 给出你这轮实际白名单。
4. 能提就提，不能提就把 blocker 压成最小、报实 `当前 own 路径是否 clean`。
5. 严守 no-red 和 `thread-state`。

回卡最小格式：
- 已回写文件路径
- 是否完成本轮要求
- 当前 own 路径是否 clean
- 一句话摘要
