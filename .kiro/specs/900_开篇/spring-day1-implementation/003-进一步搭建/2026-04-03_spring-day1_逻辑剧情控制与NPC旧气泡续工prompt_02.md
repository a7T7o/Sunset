# 2026-04-03 spring-day1｜逻辑剧情控制与 NPC 旧气泡续工 prompt

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_0.md`

这轮当前统一裁定已经改写为：

1. `UI / SpringUI` 线程正式接走 spring-day1 当前全部玩家面 `UI/UE` 问题。
2. spring-day1 从现在起，不再继续主刀这些 UI 问题。
3. spring-day1 这边只保留：
   - Day1 逻辑完善
   - 剧情 / 行为顺序 / 约束边界把控
   - `NPCBubblePresenter.cs` 里旧正式气泡这一个例外收尾

## 当前已接受基线

1. 正规对话可见性已经恢复到线程自测通过
2. 工作台制作中离开 + 离台浮层已经恢复到线程自测通过
3. 工作台 5 个配方已经统一到 5 秒
4. 这些 UI 结果从现在起都交给 `UI` 线程继续接走，不再由 spring-day1 往下做玩家面抛光

## 这轮唯一主刀

只做：

`非 UI 的逻辑 / 剧情控制收口 + NPC 旧正式气泡 live 终验`

翻成人话就是：

- 你负责把这条线剩下的逻辑、阶段控制、约束边界、剧情顺序继续守住
- 同时把 `NPC` 普通气泡和打断短气泡的旧正式样式做最后一次现场确认

## 这轮明确不做

1. 不再继续接玩家面 UI 问题
   - `DialogueUI.cs`
   - `SpringDay1PromptOverlay.cs`
   - `SpringDay1WorkbenchCraftingOverlay.cs`
   - `WorldHint / Hint / overlay / prefab-first` 这些后续玩家面收口
   - 全部交给 `UI` 线程
2. 不做 `town` 场景施工
3. 不做新 NPC 在当前 temp scene 的长期站位扩展
4. 对“前面的剧情和内容”先不继续扩
   - 因为这些 NPC 后续都会去 `town`
   - 当前 temp scene 不是它们的最终承载面
5. 不碰：
   - `Primary.unity`
   - `GameInputManager.cs`

## 这轮你的 exact-own 焦点

你这轮只围着下面这些问题继续：

1. `NPCBubblePresenter.cs`
   - 把普通 NPC 气泡和打断短气泡都补到 live/GameView 终验
   - 不再只停在“代码看起来回正了”
2. Day1 逻辑与剧情控制
   - 行为顺序
   - 阶段推进
   - 中断 / 恢复
   - 约束边界
   - 当前 temp scene 里还必须守住的剧情控制逻辑
3. 与上面强绑定的 debug / validation / runtime consumption / tests

## 关于剧情与内容的硬边界

这轮不要再继续往“把前面的剧情和内容全扩完”那个方向漂。

当前正确口径是：

1. `town` 还没做好
2. 这些 NPC 后续不会长期留在现在这个场景
3. 所以现在不继续做前置内容扩张
4. 你只做：
   - 现在这条线还必须正确运行的逻辑
   - 现在这条线的剧情顺序与约束控制
   - 后续能平稳迁到 `town` 的逻辑前提

## 这轮完成定义

你这轮做完时，必须至少回答清楚：

1. `NPC` 旧正式气泡有没有拿到 live 终验证据
2. 当前剩余逻辑问题里，哪些是现在就能修的
3. 哪些内容因为 `town` 未就位，应该明确继续冻结
4. 现在 temp scene 里 Day1 还要继续守住哪些阶段与行为顺序

不要把“town 以后会做”拿来替代当前逻辑判断。

## 这轮回执必须按这个格式

先给用户可读层：

1. 当前主线：
2. 这轮实际做成了什么：
3. 现在还没做成什么：
4. 当前阶段：
5. 下一步只做什么：
6. 需要我现在做什么（没有就写无）：

然后再补技术审计层：

- changed_paths：
- 验证状态：
- 是否触碰高危目标：
- blocker_or_checkpoint：
- 当前 own 路径是否 clean：

## 这轮 thread-state 接线要求

如果这轮会继续真实施工，先跑：

- `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 sync 前，必须先跑：

- `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住、让位或不准备收口，改跑：

- `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：

1. 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
2. 如果还没跑，原因是什么
3. 当前是 `ACTIVE / READY / PARKED / BLOCKED` 哪一个

## 最后一句话

这轮 spring-day1 不再继续当 UI 线程。

从现在起，你只做：

- 逻辑
- 剧情控制
- 约束边界
- `NPC` 旧正式气泡 live 终验
