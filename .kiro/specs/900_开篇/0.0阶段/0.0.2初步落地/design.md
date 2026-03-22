# 0.0.2初步落地 - 设计文档

## 1. Checkpoint 名称
- Checkpoint 1：Day1 阶段推进主链

## 2. 上层承接关系
- 本子工作区只负责根层总清单中的第一个 checkpoint。
- 根层全局执行文件是：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\0.0阶段\0.0阶段主线任务清单.md`
- 本文不重复承担根层全局路线图职责，只展开当前 checkpoint 的详细设计。

## 3. 当前 live 缺口

### 3.1 已有内容
- 已有基础 `DialogueManager`
- 已有 `DialogueSequenceSO`
- 已有 `NPCDialogueInteractable`
- 已有 `IsLanguageDecoded` 临时状态
- 已有基础 UI 与对话播放能力

### 3.2 缺失内容
- 没有 `StoryManager`
- 没有 `StoryPhase`
- 没有“对话完成 -> 剧情推进”的统一事件
- 没有把 Day1 的临时状态组织成一条正式主链
- 没有把首段与后续对话的分流固化为正式机制

## 4. 设计目标
- 用最小新增骨架建立 Day1 主控脊柱。
- 让后续疗伤 / 闪回 / 教学等段落都能挂到同一条推进链上。
- 避免把阶段推进逻辑继续散落在单个 NPC 或单个对话资产上。

## 5. 设计方案

### 5.1 新增 `StoryPhase`
- 最小先覆盖以下阶段：
  - `CrashAndMeet`
  - `EnterVillage`
  - `HealingAndHP`
  - `WorkbenchFlashback`
  - `FarmingTutorial`
  - `DinnerConflict`
  - `ReturnAndReminder`
  - `FreeTime`
  - `DayEnd`

### 5.2 新增 `StoryManager`
- 职责：
  - 保存当前 `StoryPhase`
  - 保存 `languageDecoded`
  - 提供推进到下一阶段的统一入口
  - 对外发出阶段变化事件
- 边界：
  - 这一轮不把所有剧情段都实现进去
  - 先只建立主控骨架与最小状态迁移

### 5.3 扩展 `StoryEvents`
- 新增：
  - `DialogueSequenceCompletedEvent`
  - `StoryPhaseChangedEvent`
- 目的：
  - 让对话、主控、后续系统通过事件串起来，而不是硬耦合

### 5.4 扩展 `DialogueSequenceSO`
- 增加最小完成后配置：
  - 完成后是否解码语言
  - 完成后是否推进到指定阶段
  - 完成后是否切换到 follow-up 对话

### 5.5 扩展 `DialogueManager`
- 在序列自然完成时发出 `DialogueSequenceCompletedEvent`
- 保留当前对话播放能力，不重写已有打字机/UI链

### 5.6 扩展 `NPCDialogueInteractable`
- 从“单一序列”升级为“首段 / 后续分流”
- 分流依据优先读 `StoryManager` 状态，其次兼容本地序列完成状态

## 6. 为什么先做这个 checkpoint
- 后续每个段落都需要依附到某个“当前剧情阶段”上。
- 如果不先补主控脊柱，后面疗伤、闪回、教学都会继续各写各的，收不成完整 Day1。

## 7. 本 checkpoint 的用户可感知效果
- 同一个 NPC 的首段对话播完后，不会一直重复首段。
- `languageDecoded` 不再只是局部临时变量，而是进入 Day1 正式推进链。
- 项目开始具备“这一段完成后进入下一段”的主线能力。
