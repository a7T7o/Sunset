# 给 `spring-day1` 线程的存档边界确认 prompt

你这轮不用停下当前主刀，也不用切到“存档系统”做实现。

请先完整读取：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_spring-day1_存档边界回执_01.md`

## 你当前收到的请求

`存档系统` 线程准备为当前 `Sunset demo` 的第一版存档做实施前范围收敛。

这轮需要你做的不是施工，而是从 `spring-day1` owner 视角确认边界：
- 哪些 Day1 状态是长期态，应该进入 slot-save
- 哪些只是派生显示、运行时导演摆位或瞬时态，不该直接持久化
- 哪些文件 / 语义允许 `存档系统` 线程直接改
- 哪些最好由你自己收最后一跳

## 这轮统一前提

1. 默认你正在干活，不要求你暂停当前切片。
2. 请在你当前切片的最近安全点，或本轮切片收尾时，再补这份回执。
3. 不需要单独聊天回我；只回写回执文件。
4. 允许你开 subagent 做并行只读对照；最终仍只把结论写进回执文件。
5. 这轮不是让你立刻修存档，不要自行扩大成代码施工。

## 你需要重点确认的内容

请结合你对 `spring-day1` 当前真实语义的理解，重点回答这些问题：

1. 下列状态里，哪些必须进第一版存档：
   - `StoryManager.CurrentPhase`
   - `StoryManager.IsLanguageDecoded`
   - `DialogueManager` 已完成正式对白序列集合
   - `SpringDay1Director` 中的教学目标完成态
   - `SpringDay1Director` 中的 `craftedCount`
   - `freeTimeEntered / freeTimeIntroCompleted / dayEnded`
   - 工作台首次提示消费
2. 下列状态里，哪些明确不该直接存：
   - `PromptCardModel`
   - `SpringDay1PromptOverlay` 当前展示内容
   - `DialogueUI` 当前打字进度
   - `SpringDay1NpcCrowdDirector` 的当前站位 / parent / 临时摆位
   - 工作台 Overlay 当前选中配方 / hover / 进度文案
3. 下面这些文件里，你允许 `存档系统` 线程直接改哪些，哪些更希望由你自己收：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryManager.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\DialogueManager.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\CraftingStationInteractable.cs`
4. 如果 `存档系统` 线程只做接线与数据结构，而“最后一跳语义收口”应由你完成，请直接写清。
5. 哪些情况你认为必须从“边界确认”升级成“正式许可”。

## 明确禁止的漂移

- 不要把这轮扩成 Day1 功能修复
- 不要顺手改 UI / 对话 / 演出内容
- 不要把“结构上能存”误写成“体验上应该存”
- 不要额外在聊天里再写一份长回执

## 回执方式

只在这个文件里回写：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_spring-day1_存档边界回执_01.md`

如果你判断某块必须由 owner 自己收，请在回执里直接写明：
- `建议由本线程自收`
- 并给出一句理由
