# 0.0.2初步落地 - 需求文档

## 1. 子工作区定位
- `0.0.2初步落地` 是 `0.0阶段` 的第一个执行子工作区。
- 它承接的上层总清单是：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\0.0阶段\0.0阶段主线任务清单.md`
- 它不负责完成整个春1日，而是只负责**第一个 checkpoint 的设计与落地任务清单**。

## 2. 本 checkpoint 的目标
- 为春1日建立“阶段推进主链”的最小实现方案。
- 让 Day1 从“可以播放单段对话”升级为“能够由主控脊柱驱动剧情前进”。

## 3. 本 checkpoint 必须覆盖的内容
- `StoryManager` 的存在性与职责范围
- `StoryPhase` 的最小阶段定义
- 对话完成后的统一事件
- `DialogueSequenceSO` 的完成后配置能力
- `NPCDialogueInteractable` 的首段 / 后续分流能力
- 临时状态 `languageDecoded` 向正式剧情推进链的归并方式

## 4. 本 checkpoint 不做的内容
- 不直接落地疗伤 / 血条表现
- 不直接落地工作台闪回演出
- 不直接落地耕种 / 砍树教学
- 不直接落地晚餐 / 睡觉结束
- 不返工已验收的 UI

## 5. 输入依据
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\requirements.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\OUT_design.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\OUT_tasks.md`
- 当前 live 代码：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\DialogueManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Data\DialogueSequenceSO.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCDialogueInteractable.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Events\StoryEvents.cs`

## 6. 子工作区验收标准
- 已形成首个 checkpoint 的清晰设计方案。
- 已形成可执行任务清单，明确到系统 / 数据 / 行为三层。
- 后续真实实现时，不需要再重新决定“第一个 checkpoint 到底先做什么”。
