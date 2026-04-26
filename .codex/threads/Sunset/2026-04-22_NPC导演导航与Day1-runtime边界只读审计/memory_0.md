# 2026-04-22｜NPC、导演、导航与 Day1 runtime 边界只读审计

## 用户目标
- 在 `D:\Unity\Unity_learning\Sunset` 对 `NPC 常驻 / 导航 / 自漫游 / 关系对话 / 导演链 / Day1 主线` 的真实关系做只读审计。
- 输出不仅要有压缩判断，还要保留可反复迭代的技术提取底稿。
- 不改业务代码与工作区正文。

## 本轮实际完成
- 按 `skills-governor` 手工等价前置核查，确认本轮属于 `Sunset` 的只读分析，未进入真实施工，因此未跑 `Begin-Slice`。
- 按 `sunset-workspace-router` 命中并读取：
  - `.kiro/specs/900_开篇/spring-day1-implementation/memory.md`
  - `004_runtime收口与导演尾账` 总文档与 `memory.md`
  - `102-owner冻结与受控重构` 总表与 `memory.md`
  - `Docx/大总结/Sunset_持续策划案/04_剧情NPC.md`
  - `Docx/大总结/Sunset_持续策划案/08_进度总表.md`
- 交叉核对代码：
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NpcResidentRuntimeContract.cs`
  - `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs`
  - `Assets/YYY_Scripts/Service/Player/PlayerNpcRelationshipService.cs`
  - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
  - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
  - `Assets/YYY_Scripts/Story/Interaction/NpcInteractionPriorityPolicy.cs`
  - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
  - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
  - `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`

## 关键判断
- 当前真实结构已经不是“Day1 自己驱动所有 NPC 行为”，而是：
  - `SpringDay1Director` 管 phase / beat / dialogue / time guard / 特定 story actor
  - `SpringDay1NpcCrowdDirector` 仍然过深介入 resident 的 release 后生命周期、return-home、night-rest 与白天自治回放
  - `NPCAutoRoamController` 已出现 façade 化边界，提供 `AcquireStoryControl / ReleaseStoryControl / RequestStageTravel / RequestReturnHome / ResumeAutonomousRoam`
  - `NpcResidentRuntimeContract + PersistentPlayerSceneBridge` 负责 native resident 与 crowd-managed resident 的 snapshot/restore 分层
  - `NPCDialogueInteractable / NPCInformalChatInteractable / NpcInteractionPriorityPolicy` 负责 formal / informal 的交互优先级与关系成长入口
- 当前最大的 runtime owner/边界问题，不再是“NPC 缺功能”，而是：
  - `CrowdDirector` 仍持有剧情后 resident 的 runtime lifecycle
  - `20:00/21:00/26:00` 的 schedule 仍带有 Day1 私房 owner 痕迹
  - formal / informal 交互虽然已接 façade，但仍受 scripted control / formal navigation 相位强约束
  - scene continuity 通过 snapshot 已经成形，但 crowd-managed 和 native resident 仍是双轨

## 证据锚点
- `SpringDay1Director.CanConsumeStoryNpcInteraction / TryConsumeStoryNpcInteraction` 说明 Day1 主线对白入口会抢占 `001` 的交互。
- `SpringDay1Director.HandleHourChanged / HandleSleep` 说明 Day1 主线仍拥有时间裁定和 sleep 收束。
- `SpringDay1Director.ShouldReleaseEnterVillageCrowd / ShouldLatchEnterVillageCrowdRelease / ShouldHoldEnterVillageCrowdCue` 说明导演仍直接影响 crowd runtime release 节点。
- `SpringDay1NpcCrowdDirector.ApplyResidentBaseline / TickResidentReturnHome / FinishResidentReturnHome / SyncResidentNightRestSchedule` 说明 crowd 仍深持 resident release 后 runtime。
- `NPCAutoRoamController` 的 façade 表明 NPC own 边界正在成形，但 crowd/day1 仍大量消费其 runtime-only 结果。
- `NpcCharacterRegistry + PlayerNpcRelationshipService + NPCDialogueInteractable + NPCInformalChatInteractable` 说明关系成长与 formal/informal 对话已从“剧情本身”拆成独立数据/服务/交互链。

## 验证与限制
- 本轮只读，无代码修改、无测试执行、无 Unity live 操作。
- 结论主要基于：authoritative 文档 + 当前代码静态结构 + 既有 memory 中记录的 live 事实。
- 未重跑 fresh live，因此对部分“当前体感是否仍如此”保持“沿用已有文档证据，不冒充最新实测”。

## 主线与恢复点
- 当前主线目标：抽出 `NPC / 导演 / 导航 / Day1 runtime` 的真实边界和可写简历事实句。
- 本轮子任务：做只读审计与技术提取底稿。
- 服务对象：后续草稿本、简历事实句、边界治理与 owner 退权分析。
- 若后续继续：
  - 第一优先是把这轮 A-F 审计底稿转成长期草稿本。
  - 第二优先是按“owner 边界 / schedule owner / crowd-managed vs native resident 双轨”继续补证。
