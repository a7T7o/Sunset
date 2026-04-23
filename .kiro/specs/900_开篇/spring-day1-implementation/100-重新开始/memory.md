# 100-重新开始 - 开发记忆

## 模块概述
- 模块名称：100-重新开始
- 模块目标：作为 `Day1-V3` 的新接班分析工作区，先只读吃透旧 prompt、旧 memory、旧交接文档与当前代码现场，独立判断 `day1` 这条线的真实问题、误修成因与后续重构风险。

## 当前状态
- **完成度**：已完成首轮只读接班审计与问题归纳，未进入真实施工
- **最后更新**：2026-04-15
- **状态**：分析完成，等待用户决定是否进入下一轮只读冻结或真实施工

## 会话记录

### 会话 1 - 2026-04-15

**用户需求**:
> 你现在属于从0开始，那就必须彻底吃透先，因为我不打算让你直接重构，你先看完后告诉我你认为的问题有哪些，分析重构的风险以及分析为什么day1做了这么多轮都没有解决问题而是一直在产生问题，一直在不同的方式去犯错，我觉得这个是你需要先厘清的，他写了很多文档你都要分析清楚他到底是什么情况，我希望你不要被他带着跑但是也要有自己的思考。

**完成任务**:
1. 以只读模式完成 `skills-governor + sunset-workspace-router` 手工等价前置核查，明确本轮不进入真实施工，不跑 `Begin-Slice`。
2. 读取并交叉比对：
   - 用户转述给旧 `day1` 的核心 prompt 与旧回复
   - `004_runtime收口与导演尾账` 子工作区文档与 memory
   - `spring-day1 / spring-day1V2` 线程 memory
   - 当前 live 代码：`SpringDay1Director.cs`、`SpringDay1NpcCrowdDirector.cs`、`NPCAutoRoamController.cs`、`SpringDay1DirectorStagingTests.cs`
3. 独立确认当前真正的问题不止是“CrowdDirector 太重”，而是：
   - `CrowdDirector` 仍在继续持有剧情后 resident runtime owner
   - opening handoff 的文档语义、代码语义、测试语义三者仍未统一
   - `Director` 自身也仍深度参与运行态控制，`003` 特殊化尚未真正消失
4. 产出只读审计文档：
   - `2026-04-15_Day1-V3_接班诊断与重构风险审计.md`
5. 新建本工作区 memory，建立 `Day1-V3` 后续连续性入口。

**修改文件**:
- `D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/100-重新开始/2026-04-15_Day1-V3_接班诊断与重构风险审计.md` - [新增]：Day1-V3 首轮接班诊断、误修成因与重构风险审计
- `D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/100-重新开始/memory.md` - [新增]：当前工作区记忆

**解决方案**:
- 不直接延续旧线程“已经接近重构入口”的自我叙事，而是先把旧文档声明、当前代码真相、当前测试口径分别拆开核对。
- 把“为什么一直修出新问题”收束为 5 个独立原因：
  1. owner 没有单一真源
  2. 事件驱动与轮询驱动混在同一个控制器
  3. 文档、代码、测试保护三套不同真相
  4. `NPCAutoRoamController` 性能风险被坏输入持续放大
  5. 局部测试给了错误安全感

**遗留问题**:
- [ ] 还未建立“resident lifecycle 全入口冻结表”，后续若进入真实施工前必须先补。
- [ ] 还未单独梳理 `003` 特殊化残留入口清单。
- [ ] 还未整理“哪些 staging tests 是 stopgap 测试、哪些应在重构第一阶段重写/删除”的测试分层表。
- [ ] 本轮未跑 compile / live / profiler，只完成静态与历史交叉审计。

## 关键决策

| 决策 | 原因 | 日期 |
|------|------|------|
| 本轮保持只读，不进入真实施工 | 用户明确要求先从 0 吃透，不直接重构 | 2026-04-15 |
| 不直接沿用旧 `day1` 的交接结论，先核代码和测试 | 旧线程后期文档口径与代码/测试口径存在明显冲突 | 2026-04-15 |
| 先把问题归纳成“owner/真相不一致/测试错保”三层，再谈重构 | 当前最大风险不是不会修，而是继续在错边界上加补丁 | 2026-04-15 |

## 2026-04-15｜方向澄清补记：下一阶段不是“继续散修”，也不是“无边界一轮梭哈”
- 用户进一步追问的核心不是代码细节，而是：
  - 我到底是想继续一步一步修
  - 还是已经准备直接彻底重构
  - 冻结表在我这里到底是不是只是给用户看的展示品
- 当前明确结论：
  1. 冻结表对我不是展示品，而是施工图。
  2. 我不认同“现在直接一轮无边界全改到底”。
  3. 我也不认同回到旧线程那种“继续零散补丁修到自然变对”。
  4. 我真正倾向的是：
     - 先用一轮只读冻结把 owner / 入口 / 测试债一次钉死
     - 然后进入受控重构
     - 后续按 owner 主线连续收，不再散修
- 原因：
  1. 当前 `CrowdDirector` 的 owner、`003` 特殊化、night schedule、恢复链、stopgap tests 仍然缠在一起。
  2. 如果现在直接“大重构一轮梭哈”，风险不是我写不动，而是太容易把 `001/002 house lead`、`Primary` 稳定链和现有执行层一起误伤。
  3. 如果继续只按症状补丁，又会回到旧 `day1` 那种“每轮都像在纠偏，实际上一直在换坏相”。
- 当前最准确的方向定义：
  - `先冻结，再受控重构；不是继续散修，也不是现在直接无边界总重写。`

## 相关文件

### 核心文件（读取/审计）
| 文件 | 关系 |
|------|------|
| `D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs` | Day1 导演层当前运行态触碰口 |
| `D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs` | 当前最核心的 owner 冲突与 runtime 持有者 |
| `D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` | resident 执行层与性能放大器 |
| `D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs` | 当前测试口径与 stopgap 合同 |
| `D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/004_runtime收口与导演尾账/2026-04-15_spring-day1_Day1历史语义_系统边界_现状问题_重构交接总文档.md` | 旧线程最新总交接文档 |
| `D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/spring-day1/memory_0.md` | 旧 `day1` 线程记忆 |
| `D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/spring-day1V2/memory_0.md` | 更早期 Day1 V2 线程记忆 |

## 2026-04-18｜只读彻查补记：交互禁聊链与 roam 节奏真实来源已钉实
- 当前主线目标：
  - 不开 runtime 修复刀，先把“为什么 NPC 到现在还是不能正常聊天、为什么按 `E` 只冒一个字就消失、为什么 roam 节奏仍显得过密”用代码事实彻底钉死。
- 本轮子任务：
  1. 彻查 `NPCInformalChatInteractable -> PlayerNpcChatSessionService -> NPCAutoRoamController`
  2. 复核 `SpringDay1Director / SpringDay1NpcCrowdDirector` 对 formal / informal 组件的直接控制
  3. 复核人类 NPC 的 roam pause 真实来源与当前现值
- 已完成事项：
  1. 已确认一个 `NPC own` 全局硬 bug：
     - `NPCInformalChatInteractable.EnterConversationOccupation()` 会先 `AcquireStoryControl("npc-informal-chat")`
     - `PlayerNpcChatSessionService.Update()` 又把任何 `IsResidentScriptedControlActive` 当 takeover，下一帧就 `CancelConversationImmediate(...)`
     - 这正是“按 E 后只冒一个字就消失”的直接源码根因
  2. 已确认当前“不能正常聊天”至少有 4 层叠加：
     - `SpringDay1Director.ApplyStoryActorRuntimePolicy(...)`
     - `SpringDay1NpcCrowdDirector.SetResidentCueInteractionLock(...)`
     - `NPCInformalChatInteractable.IsResidentScriptedControlBlockingInformalInteraction()`
     - `PlayerNpcChatSessionService.Update()` 的 scripted-control 自杀判断
  3. 已确认 roam 节奏不靠神秘 hardcode，而是主吃 `NPCRoamProfile.asset`
     - crowd `101~203` 当前多为 `shortPause 0.5~3 / longPause 3~6`
     - `001` 为 `0.8~2.6 / 4~6.8`
     - `002` 为 `0.7~2.8 / 3.6~6.2`
     - 若要改成用户要求的 `2~5 / 5~8`，最有效落点应是现用 profile 资产，不是只改脚本默认值
  4. 已把上述结论同步回 `0417.md`：
     - `2. 当前代码真相`
     - `4. 全量问题清单`
     - `8. 迭代 tasks`
- 本轮没有做：
  1. 未改 runtime 代码
  2. 未跑 compile / tests / live
  3. 不宣称任何体验过线
- 当前恢复点：
  1. 下一刀若开修，优先级应是：
     - 先修 `NPC informal chat` 自杀链
     - 再清 Day1 own 的多层禁聊 / 关组件链
     - 然后再收 roam 停顿节奏
  2. 这轮已执行 `Begin-Slice` 用于文档/记忆落盘；真正停手前需补 `Park-Slice`

## 2026-04-18｜真实施工补记：NPC 自杀链、Day1 直接关组件链与 roam pause 已落代码
- 当前主线目标：
  - 按用户最新硬要求，把上轮只读钉死的 `NPC informal chat 自杀链 + Day1 直接禁聊链 + roam pause 过短` 真正落回代码，并尽量补到 targeted 验证。
- 本轮子任务：
  1. 修 `PlayerNpcChatSessionService` 对 `npc-informal-chat` 自己 owner 的误杀判断
  2. 修 `NPCInformalChatInteractable / NPCDialogueInteractable` 的 resident scripted-control gate
  3. 拆 `SpringDay1Director / SpringDay1NpcCrowdDirector` 直接关 `formal / informal` 组件的 runtime
  4. 把现用 human NPC profile 的 pause 改成 `short 2~5 / long 5~8`
  5. 把新增交互回归挂进现有 `Run NPC Formal Consumption Tests`
- 已完成事项：
  1. `PlayerNpcChatSessionService.Update()` 已改成：
     - 忽略 `npc-informal-chat` 自己的 resident scripted owner
     - 只在真正的外部 story/crowd owner 接管时才收掉闲聊
  2. `NPCInformalChatInteractable` 已补：
     - `InformalChatControlOwnerKey` 自识别
     - scripted-control gate 不再拦自己会话
  3. `NPCDialogueInteractable` 已补：
     - 与 informal 同步的 scripted-control gate
     - `FreeTime + FormalNavigation` 回家窗口允许正常 formal 交互
  4. `SpringDay1Director.ApplyStoryActorRuntimePolicy(...)`
     - 已不再直接 `enabled=false` 关 `formal / informal`
  5. `SpringDay1NpcCrowdDirector.SetResidentCueInteractionLock(...)`
     - 已不再把 `formal / informal` 组件当 cue 锁总闸
  6. 现用 human NPC profile 已回写：
     - `NPC_Default`
     - `001`
     - `002`
     - `003`
     - `101~203`
     统一为 `shortPause 2~5 / longPause 5~8`
  7. `SpringDay1TargetedEditModeTestMenu.cs` 已把这轮新增交互回归挂进 `Run NPC Formal Consumption Tests`
- 本轮验证结果：
  1. `manage_script validate`：
     - `PlayerNpcChatSessionService / NPCInformalChatInteractable / NPCDialogueInteractable / SpringDay1Director / SpringDay1NpcCrowdDirector / SpringDay1TargetedEditModeTestMenu`
     - `errors=0`
  2. `manage_script validate`：
     - `NPCInformalChatInterruptMatrixTests / NpcInteractionPriorityPolicyTests / SpringDay1DirectorStagingTests`
     - `errors=0`
  3. `Run NPC Formal Consumption Tests`：
     - `6/6 passed`
     - 已真实包含：
       - `FormalDialogueInteractable_ShouldFollowResidentScriptedControlContract`
       - `ConversationTakeoverProbe_ShouldIgnoreInformalChatSelfOwner`
       - `ConversationTakeoverProbe_ShouldTreatStoryOwnerAsRealTakeover`
  4. `Run Director Staging Tests`：
     - `30/10`
     - 本轮改到的相关 case 仍为 `Passed`
     - 剩余失败为旧 staging 债，不属于本轮新交互链
  5. fresh `errors`：
     - `0 error / 0 warning`
  6. `git diff --check` 覆盖本轮 own 文件通过
- 当前判断：
  1. 这轮已经不再是“交互根因只读钉死”，而是三条主链都已真实落代码。
  2. 当前最硬的新证据是：
     - `NPC Formal Consumption` 菜单 fresh `6/6 pass`
     - fresh console `0/0`
  3. 但这仍不是 packaged/live 已过线；`0417` 里仍有 `Director Staging` 历史债和 live/packaged 终验未闭环。
- 当前恢复点：
  1. 如果继续自动化，优先看：
     - 是否要继续清 `Director Staging` 旧 10 fail
     - 还是直接转 packaged/live / 用户终验
  2. 停手前需同步：
     - `0417.md`
     - 父层 memory
     - 线程 memory
     - `Park-Slice`

## 2026-04-17｜0417 主控文档已建立
- 用户最新要求：
  - 不先继续开新代码刀
  - 先做一份可持续维护的总文档 `0417.md`
  - 形式参考 `tasks.md`
  - 必须同时覆盖：
    1. 长语义梳理
    2. 当前代码真相
    3. 全量问题总表
    4. 分阶段执行计划
    5. 可持续维护的 tasks 区
- 本轮实际动作：
  1. 用 `skills-governor + sunset-workspace-router + preference-preflight-gate` 做前置核查。
  2. 执行 `Begin-Slice`：
     - `slice=0417文档建档与全量执行清单落盘`
  3. 读取：
     - 当前工作区 `memory`
     - 父工作区 `memory`
     - 线程 `memory`
     - `102` 阶段总表与导航/NPC contract
     - 用户指定参考 `D:\Unity\Unity_learning\Sunset\.kiro\specs\箱子系统\7_终极清算\tasks.md`
  4. 新增：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\100-重新开始\0417.md`
- `0417.md` 当前承担的职责：
  1. 把用户最新 Day1 语义写成唯一真值
  2. 把当前代码高置信真相与“用户已测但仍待我重核”的内容分开
  3. 把全量问题压成统一总表，并给出风险/安全性/推荐阶段
  4. 把后续施工写成 `P0~P6` 分阶段 tasks，作为下一轮持续维护入口
- 当前最重要的新冻结点：
  1. opening 后 ordinary resident 的正确语义已经改成：
     - 原地解散
     - 白天自由活动
     - 不再回 baseline/anchor/home
  2. dinner 当前采用最简硬语义：
     - 全员传起点
     - 走一次到终点
     - 最多 5 秒
     - 超时 snap
     - `18:00~19:30` 不再额外走位
  3. 夜间最终目标明确为：
     - `20:00 return-home`
     - `21:00 snap-to-anchor + hide`
     - 次日从 anchor 激活
- 本轮没有做的事：
  1. 未改运行时代码
  2. 未跑 compile / tests / live
  3. 不宣称任何体验过线
- 当前恢复点：
  1. 下轮若继续真实施工，直接以 `0417.md` 为唯一主控文档继续维护。
  2. 推荐顺序已写死为：
     - `P2-1` 疗伤距离小刀
     - `P1` opening/daytime
     - `P2` primary 与 `0.0.6`
     - `P3` dinner/late-day
     - `P4` 夜间统一合同
     - `P5` 抽象锚点与测试清扫
     - `P6` 全线终验

## 2026-04-17｜0417 二次冻结：任务区已重写为包级任务板
- 用户最新纠正：
  - 认为 `0417` 当前的任务区还不够细、不够全面，容易让我后续继续漂移。
  - 要求我重新客观梳理整个 Day1 的实际情况，再把任务区重写细。
- 本轮实际动作：
  1. 执行 `Begin-Slice`
     - `slice=0417任务区重整与Day1现状二次冻结`
  2. 重新对照关键代码与测试入口：
     - `SpringDay1Director.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - `SpringDay1DirectorStagingTests.cs`
     - `SpringDay1LateDayRuntimeTests.cs`
     - `SpringDay1NpcCrowdManifest.asset`
     - `SpringDay1DirectorStageBook.json`
     - `SpringDay1TownAnchorContract.json`
     - `Assets/Editor/Town/*`
     - `Assets/Editor/NPC/*`
  3. 重写 `0417.md` 的中后段，不再只保留粗粒度阶段标题。
- 本轮新增的关键冻结点：
  1. 新增 `2.2 当前已经结构性落地、但不能冒充过线的内容`
     - 明确：
       - NPC façade 已存在且 Day1 已部分接入
       - primary “不暂停时钟”的结构判断已写进代码和测试
       - resident return-home 主路已不是 transform 硬推
       - dinner 已存在 cue wait + actor prepare 链，但仍不是正确终局
  2. 新增 `2.3 当前最容易让我后续漂移的 5 个真源`
     - `runtime owner split`
     - `data asset 仍喂旧 beat/anchor`
     - `editor/bootstrap/validation 仍喂旧真相`
     - `tests 混合保护新旧两套真相`
     - `用户 live 与静态结构不等价`
  3. 全量问题表新增：
     - `I-14 旧 beat / semantic anchor 债务`
     - `I-15 editor/bootstrap/validation 合同仍旧`
  4. `8. 迭代 tasks` 已彻底从粗阶段标题改成 `Package A~G` 包级任务板：
     - 每个 package 都带：
       - 目的
       - 主要锚点
       - 前置
       - 具体 tasks
       - 退出条件
- 当前最重要的新判断：
  1. 之前 `0417` 容易漂，不是因为“任务数量少”，而是没有把：
     - runtime
     - 资产
     - editor
     - tests
     - 用户 live
     这五层错位一起压进任务区。
  2. 这轮重写后，后续每一刀都可以直接挂到真实代码入口和真实依赖层，不需要再靠聊天找方向。
- 本轮没做：
  1. 未改运行时代码
  2. 未跑 compile / tests / live
  3. 不宣称任何体验过线
- 当前恢复点：
  1. 下轮若继续真实施工，先从 `Package A` 和 `C-01` 开始，不再直接跳到 runtime patch。
  2. 当前推荐顺序已更新为：
     - `A`
     - `C-01`
     - `B`
     - `C`
     - `D`
     - `E`
     - `F`
     - `G`
  3. 本轮已执行 `Park-Slice`，当前状态=`PARKED`

## 2026-04-17｜Package A 已完成：六张冻结表已补齐
- 用户最新续工要求：
  - 不满足于“Package A 只有任务板”
  - 要我真正把 Package A 的六张冻结表写进 `0417.md`
- 本轮实际动作：
  1. 再次 `Begin-Slice`
     - `slice=PackageA_冻结补表与防漂移底板`
  2. 围绕以下入口补齐冻结表：
     - `SpringDay1Director.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - `SpringDay1DirectorStagingTests.cs`
     - `SpringDay1LateDayRuntimeTests.cs`
     - `SpringDay1NpcCrowdManifest.asset`
     - `SpringDay1DirectorStageBook.json`
     - `SpringDay1TownAnchorContract.json`
     - `Assets/Editor/Town/*`
     - `Assets/Editor/NPC/*`
  3. 已把 `0417.md` 新增为真实冻结层内容，而不只是任务标题：
     - `5.4.1 语义 -> StoryPhase / beatKey / runtime entry / test 映射表`
     - `5.4.2 当前 owner matrix`
     - `5.4.3 resident lifecycle 全入口冻结表`
     - `5.4.4 旧 beat / semantic anchor 依赖矩阵`
     - `5.4.5 editor / bootstrap / validation 残依赖矩阵`
     - `5.4.6 测试分层表`
     - `5.4.7 Package A 当前完成度`
  4. 同步把 `Package A` 下的：
     - `A-01 ~ A-06`
     全部标记为已完成
- 本轮最重要的新冻结结论：
  1. Package A 现在已经不是“以后要补”，而是“已补齐”。
  2. 这轮后，Day1 后续施工不再缺：
     - 语义到 phase/beat/runtime/test 的映射
     - owner matrix
     - resident lifecycle 全入口冻结
     - 旧 beat/anchor 依赖图
     - editor/bootstrap/validation 残依赖图
     - 测试分层表
  3. 这意味着下一轮可以合法按 `C-01 -> B -> C -> D -> E -> F -> G` 继续，而不需要再回头补“防漂移底板”。
- 本轮没做：
  1. 未改运行时代码
  2. 未跑 compile / tests / live
  3. 不宣称任何体验过线
- 当前恢复点：
  1. 下轮若继续，优先收：
     - `C-01` 艾拉回血距离 `0.9 -> 1.6`
     - 然后进入 `Package B` opening/daytime 主链
  2. 本轮已执行 `Park-Slice`
     - 当前状态=`PARKED`

## 2026-04-15｜107 live follow-up：opening/dinner 单次 staged contract 与 0.0.6 自由放手补刀
- 用户最新 live 真相重新收敛为两条：
  1. `opening / dinner` 都必须是最简单的单次 staged contract：
     - 先传送到起点
     - 再朝终点走
     - 最多 5 秒
     - 超时 snap 到终点
     - 对话一开始后不允许再动
  2. `0.0.6` 回 `Town` 时，除 `001/002` 外的 resident 不能继续被 crowd runtime 卡成原地罚站。
- 本轮真实施工 slice：
  - `107_live-fix_opening-dinner-freeze-followups`
- 本轮实际改动：
  1. `SpringDay1Director.cs`
     - `MaintainTownVillageGateActorsWhileDialogueActive()` 改成对白开始后直接钉终点，不再重跑 start。
     - `TryResolveDinnerStoryActorRoute(...)` 现在先吃场景 `进村围观` 起终点 markers，再退回 home-anchor fallback；用于修正 `001/002` 晚饭前乱走到错误位置的问题。
  2. `SpringDay1NpcCrowdDirector.cs`
     - `ShouldYieldDaytimeResidentsToAutonomy()` 不再把 `FreeTime_NightWitness` 当成持续 hold 的例外；`0.0.6` 的 free window 会允许 resident 恢复自治。
  3. `SpringDay1DirectorStagingTests.cs`
     - 新增：
       - `Director_ShouldPreferVillageCrowdSceneMarkersForDinnerStoryRoute`
       - `CrowdDirector_ShouldYieldAutonomyDuringNightWitnessFreeWindow`
- 本轮验证：
  1. direct MCP `validate_script`
     - `SpringDay1Director.cs` => `errors=0`
     - `SpringDay1NpcCrowdDirector.cs` => `errors=0`
     - `SpringDay1DirectorStagingTests.cs` => `errors=0`
  2. direct MCP EditMode 贴刀口回归 `7/7 passed`：
     - `Director_ShouldPreferVillageCrowdSceneMarkersForDinnerStoryRoute`
     - `CrowdDirector_ShouldYieldAutonomyDuringNightWitnessFreeWindow`
     - `Director_ShouldExposeFreeRoamBeatDuringPostTutorialExploreWindow`
     - `Director_ResetDinnerCueWaitState_ShouldPreservePlacedStoryActors`
     - `CrowdDirector_ShouldResyncRecoveredResidentsBeforeSkippingDaytimeYield`
     - `CrowdDirector_ShouldResolveDinnerCueFromDinnerBeatInsteadOfOpeningMirror`
     - `TownVillageGateDialogueActive_ShouldKeepStoryActorsAligned`
  3. fresh console 最终=`errors=0 warnings=0`
- 当前判断：
  1. 这轮成立的是“直接对应用户最新 live 报错的 runtime fix + targeted tests”，不是体验终验。
  2. 还没继续扩到 `20:00+`，也还没把 `ReturnAndReminder / DayEnd` 全部重新裁一遍。
3. 下一步应先等用户 fresh live retest：
   - `opening` 是否还会回起点
   - `0.0.6` 回 `Town` 后 resident 是否恢复正常
   - `dinner` 的 `001/002` 是否还会走到错误位置或重走

## 2026-04-16｜只读事故定责：`0.0.6` 回 Town 卡爆的最可能坏链已收窄到 crowd 批量放手 + roam 批量重启
- 本轮性质：
  - 只读分析，不改代码，不跑 `Begin-Slice`
- 用户本轮要求：
  - 只查 `0.0.6` 回 Town 后全体 NPC 严重卡爆时，`SpringDay1NpcCrowdDirector + NPCAutoRoamController` 的高频循环里最可能的性能/状态机坏链
  - 重点看：
    - `Update / SyncCrowd / TickResidentReturns / ShouldYieldDaytimeResidentsToAutonomy / ApplyResidentBaseline`
    - 哪些条件会让 NPC 长时间停在 `scripted control active but no move contract`
    - 为什么 `opening` 结束后也会先卡一下再继续走
- 本轮稳定判断：
  1. 最可能的主根不是单一 if，而是两层链叠在一起：
     - `SpringDay1NpcCrowdDirector` 在白天放手窗口会成批调用 `ReleaseResidentToAutonomousRoam(...) / TryReleaseResidentToDaytimeBaseline(...)`
     - `NPCAutoRoamController` 在同一窗口里对每个 NPC 各自重启 roam / 建路 / 共享避障 / blocked recovery，但它只有“每个 NPC 每帧最多一次建路”，没有“整群 NPC 的全局节流”
  2. 因此 `0.0.6` 回 Town 时最像是：
     - crowd 在同一时间窗把很多 resident 一起交回 roam
     - roam controller 群体同时进入 `StartRoam / TryBeginMove / TickMoving / TryRebuildPath / shared avoidance`
     - 形成 herd-level 的 repath / deadlock / blocked-recovery 风暴
  3. 这条判断比“还被 Day1 持续 scripted owner 抓着”更强，因为：
     - 用户能对这些 NPC 开闲聊
     - `NPCInformalChatInteractable` 本身就禁止对 `IsResidentScriptedControlActive == true` 的目标开聊
     - 更像是“已经回到自治表面，但自治 runtime 留在坏态”
  4. `opening` 结束后的“先卡一下再继续走”更像同一根的弱版：
     - `ResumeAutonomousRoam(true)` 在 `!IsRoaming` 时直接 `StartRoam()` 然后返回
     - `StartRoam()` 会进入 `EnterShortPause(false)`
     - 所以刚释放时不会立刻迈步，而会先出现一拍停顿
- 关键方法级锚点：
  1. `SpringDay1NpcCrowdDirector.Update()`
     - 先跑 `TickResidentReturns()`
     - 再看 `ShouldYieldDaytimeResidentsToAutonomy()`
     - 命中时会批量 `YieldDaytimeResidentsToAutonomy()`
  2. `NeedsDaytimeAutonomyRelease(...)`
     - 只要 `ReleasedFromDirectorCue / NeedsResidentReset / AppliedCueKey / AppliedBeatKey / IsResidentScriptedControlActive` 任一残留，就会继续被判成待放手对象
  3. `ReleaseResidentToAutonomousRoam(...)`
     - 直接 `CancelResidentReturnHome(...)`
     - 再 `ResumeResidentAutonomousRoam(..., tryImmediateMove: true)`
  4. `TryReleaseResidentToDaytimeBaseline(...)`
     - 会为单个 resident 做 `TryResolveOccupiablePosition(...)`
     - 再 `ResumeResidentAutonomousRoam(..., tryImmediateMove: true)`
  5. `NPCAutoRoamController.ResumeAutonomousRoam(...)`
     - 当 `!IsRoaming` 时只会 `StartRoam()`，不会真正使用 `tryImmediateMove`
  6. `NPCAutoRoamController.StartRoam()`
     - 结尾直接 `EnterShortPause(false)`，所以 release 后天然会先停一拍
  7. `NPCAutoRoamController.Update()/FixedUpdate()`
     - 只要 `IsResidentScriptedControlActive && !IsResidentScriptedMoveActive`，就会每帧/每个 FixedUpdate 调 `ApplyResidentRuntimeFreeze()`
     - 这是“scripted owner 还活着但 move contract 已经没了”时最贵的 freeze 支路
  8. `NPCAutoRoamController.EndDebugMove(...)`
     - 对 recoverable travel（含 `FormalNavigation`）结束时，会 `HaltResidentScriptedMovement()`
     - 这会把 move 停掉，但不会在这里直接释放 scripted owner；后续还要等 crowd 自己收尾
  9. `NPCAutoRoamController.ApplyResidentRuntimeSnapshot(...)` / `SpringDay1NpcCrowdDirector.ApplyResidentRuntimeSnapshotToState(...)`
     - 如果 snapshot 带 `underDirectorCue`，会直接重新 `AcquireResidentDirectorControl(...)`
     - 这是“scripted active but no fresh move contract”最危险的恢复入口之一
  10. `NPCAutoRoamController.TryAcquirePathBuildBudget()`
      - 只限制“单 NPC 每帧一次建路”，不限制“整群 NPC 同帧集体建路”
- 对“聊一次就解放”的解释：
  1. `NPCDialogueInteractable / NPCInformalChatInteractable` 会对单个 NPC 跑：
     - `AcquireStoryControl`
     - `HaltStoryControlMotion`
     - `ReleaseStoryControl`
  2. 这等于给这个 NPC 做了一次局部 locomotion reset/restart。
  3. 如果它当前是“自治坏态 / blocked recovery 坏态 / stale path 坏态”，这次局部重启就会把它从坏循环里拉出来。
  4. 这更像“聊天是复位针”，不是“聊天链是根因”。
- 当前最小修复方向（只记录，不动代码）：
  1. Day1/Crowd 侧：
     - 不要再在同一窗口里把大批 resident 同帧 `ResumeAutonomousRoam(true)` 扔回去
     - 白天 release 应退成“一次性标记放手”，不要继续按批量 restart owner 的方式驱动
  2. NPC/roam 侧：
     - `ResumeAutonomousRoam(true)` 需要真正尊重 immediate 语义，至少不能在 `!IsRoaming` 时永远先吃一拍 `ShortPause`
  3. 状态机护栏：
     - 必须清掉 `scripted control active but no move contract` 的 lingering 状态
     - 尤其是 snapshot restore / formal navigation end / crowd release 这三条入口
- 当前恢复点：
  1. 如果下一轮继续只读，先针对：
     - `ApplyResidentBaseline(...)`
     - `TryReleaseResidentToDaytimeBaseline(...)`
     - `ResumeAutonomousRoam(...)`
     - `EndDebugMove(...)`
     - `ApplyResidentRuntimeSnapshotToState(...)`
     做一次“谁会把 resident 留在坏态”冻结表。
  2. 如果下一轮进入真实施工，第一刀不该先改聊天链，而应先切：
     - crowd 批量放手
     - roam 批量立即重启
     这两者的耦合点。
## 2026-04-16｜只读分析：为什么 `0.0.6` 回 Town 后对单个 NPC 按 E 一次会把它“解放”
- 用户最新只读问题：
  1. 不改代码，只彻查为什么 `0.0.6` 回 `Town` 后，单独对某个 NPC 按 `E` 聊天一次，这个 NPC 就会从卡死/卡顿状态恢复正常漫游。
  2. 重点要求核对：
     - `NPCDialogueInteractable / NPCInformalChatInteractable / PlayerNpcChatSessionService` 到底对 `NPCAutoRoamController` 做了什么
     - 是否会调用 `ResumeAutonomousRoam / ReleaseStoryControl / HaltStoryControlMotion / ApplyIdleFacing / StopMotion / 其他清状态入口`
     - 为什么这些调用会把单个 NPC 从问题状态里“解放”
- 只读结论：
  1. `PlayerNpcChatSessionService` 自己不直接碰 `ResumeAutonomousRoam / HaltStoryControlMotion / StopMotion`，它只是把开始/结束会话路由到 interactable。
  2. 真正会动 locomotion 的是两个 interactable：
     - `NPCDialogueInteractable.EnterDialogueOccupation()`：
       `AcquireStoryControl -> HaltStoryControlMotion -> ApplyIdleFacing`
     - `NPCDialogueInteractable.HandleDialogueEnded()`：
       `ReleaseStoryControl`
     - `NPCInformalChatInteractable.EnterConversationOccupation()`：
       `AcquireStoryControl -> HaltStoryControlMotion`
     - `NPCInformalChatInteractable.ExitConversationOccupation()`：
       `ReleaseStoryControl`
     - informal / formal 都可能补 `ApplyIdleFacing`；没有直接调 `ResumeAutonomousRoam`
  3. `ReleaseStoryControl(...)` 最终落到 `NPCAutoRoamController.ReleaseResidentScriptedControl(...)`；如果 owner 清空、允许 resume、且不在正式对白暂停态，就直接 `StartRoam()`。
  4. `HaltStoryControlMotion()` 不是轻量停步，而是一次局部 locomotion hard reset：
     - 清 `debugMoveActive`
     - 清 `activePointToPointTravelContract`
     - 清 `residentScriptedMovePaused`
     - 清 requested destination / traversal soft-pass / interruption / shared-avoidance debug state
     - `state = Inactive`
     - `path.Clear()`
     - `StopMotion()`
  5. 因此“按 E 后恢复”的真正意义是：
     - 对这个 NPC 单独跑了一次 `HaltStoryControlMotion + ReleaseStoryControl(+ StartRoam)` 的 reset/restart 闭环
     - 它像一次局部重启，而不是对白系统本身修好了问题
- 当前最强判断：
  1. 这条现象更像 `NPCAutoRoamController` 的 per-NPC runtime 状态被 Day1 在 `0.0.6` 回 Town 后留在了坏态。
  2. 它不像“NPC 还被 Day1 owner 抓着不放”，因为：
     - `NPCInformalChatInteractable.CanInteractWithResolvedSession(...)` 明确禁止对 `IsResidentScriptedControlActive == true` 的 NPC 开启闲聊
     - 用户既然能对 `003~203` 逐个按 `E`，这些 NPC 至少在聊天开始前并不处于 active scripted owner 状态
  3. 更符合事实的解释是：
     - 这些 NPC 表面上已经回到自治态，但内部仍有坏掉的 roam/path/recovery/detour/runtime 缓存
     - 聊天链恰好把这些内部状态清空并重新 `StartRoam()`，所以单个 NPC 会被“解放”
  4. 这也解释了为什么性能会随着“逐个聊天释放”而逐步恢复：
     - 如果根因是全局 crowd owner 每帧硬抓，单独聊一个 NPC 不该明显降低整体卡顿
     - 现在是每解放一个 NPC，整体负载就下降一些，更像每个 NPC 都各自卡在 `NPCAutoRoamController` 的坏 runtime 里
- 本轮建议恢复点：
  1. 下一刀如果真修，不该先去改聊天链；聊天链现在更像定位针。
  2. 最该继续追的是 `0.0.6` 回 Town 时，Day1/CrowdDirector 到底把 resident 交回了什么 runtime 状态，尤其是：
     - `ApplyResidentBaseline(...)`
     - `ReleaseResidentToAutonomousRoam(...)`
     - `ResumeResidentAutonomousRoam(...)`
     - `NPCAutoRoamController` 的 move/repath/recovery/detour 坏态是否没有被正确重置
## 2026-04-16｜108 真实施工：收 `0.0.6` 回 Town 的 bad-state restore / stale cue 回抓 / 群体立即建路放大器
- 本轮 slice：
  - `108_town_free_time_roam_spike_rootcause`
- 用户最新强语义：
  1. `0.0.6` 回 `Town` 后的 severe 卡爆必须优先处理，其他问题先让路。
  2. 语义不允许漂移；历史文档与用户 live 反馈都算真证据，但最终仍以代码和实际 runtime 为准。
- 这轮新增代码判断：
  1. `NPCInformalChatInteractable` 明确禁止对 `IsResidentScriptedControlActive == true` 的 NPC 开聊；因此“按 E 后单个 NPC 被解放”更像自治坏态被 reset，而不是仍被导演 owner 抓着。
  2. `SpringDay1NpcCrowdDirector.ApplyResidentRuntimeSnapshotToState(...)` 之前会把 neutral snapshot 只恢复位置/朝向，不真正重启 roam runtime；而聊天链恰好会对单个 NPC 做 `Acquire -> Halt -> Release -> StartRoam`。
  3. `RecoverReleasedTownResidentsWithoutSpawnStates(...)` 之前只把 resident 重新纳回 tracked state，不会补真正的 autonomous roam restart。
  4. 白天 free window 下如果 snapshot 仍带 `underDirectorCue`，那在语义上已经是 stale cue；不能再把 resident 在 `0.0.6` 回 Town 时重新抓回导演控制。
  5. 群体卡爆的放大器还包括：crowd 在 free-time 放手窗口会批量 `ResumeAutonomousRoam(..., tryImmediateMove: true)`，这会把整群 NPC 同帧推向建路/避障/blocked recovery。
- 这轮真实改动：
  1. `SpringDay1NpcCrowdDirector.cs`
     - `TryRecoverReleasedResidentState(...)`
       - recover released resident 后不再只补 tracked state；会补一次 `ResumeAutonomousRoam(tryImmediateMove: false)`，清掉表面自治但内部坏掉的 roam runtime。
     - `ApplyResidentRuntimeSnapshotToState(...)`
       - neutral snapshot restore 后改为：
         - 先清 shared runtime owner
         - 再补一次 `ResumeAutonomousRoam(tryImmediateMove: false)`
       - 白天 autonomy window 下，`snapshot.underDirectorCue` 视为 stale，不再恢复导演 owner。
     - 白天 release / baseline / cancel-return-home 等 free-time 自治恢复口，统一改成 `tryImmediateMove: false`，避免 `0.0.6` 回 Town 时整群 resident 同帧立即建路。
  2. `SpringDay1DirectorStagingTests.cs`
     - 改写：
       - `CrowdDirector_ShouldRecoverReleasedResidentsIntoTrackedStateAndRestartAutonomousRoam`
     - 新增：
       - `CrowdDirector_ShouldRestartAutonomousRoamWhenApplyingNeutralResidentRuntimeSnapshot`
       - `CrowdDirector_ShouldIgnoreStaleDirectorCueSnapshotDuringDaytimeAutonomyWindow`
- 本轮验证：
  1. CLI `validate_script`
     - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
       - 最终 `assessment=no_red`
       - owned errors `0`
       - direct validate `errors=0 warnings=2`
     - `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
       - `assessment=no_red`
       - owned errors `0`
  2. direct EditMode 回归 `8/8 passed`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldIgnoreStaleDirectorCueSnapshotDuringDaytimeAutonomyWindow`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldRecoverReleasedResidentsIntoTrackedStateAndRestartAutonomousRoam`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldRestartAutonomousRoamWhenApplyingNeutralResidentRuntimeSnapshot`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldCaptureAndReapplyResidentRuntimeSnapshot`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldNotPersistStoryDrivenReturnHomeSnapshotBeforeFreeTime`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldIgnoreClockOwnedReturnHomeSnapshotAtTwenty`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldResyncRecoveredResidentsBeforeSkippingDaytimeYield`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldYieldAutonomyDuringNightWitnessFreeWindow`
  3. console：
     - 测试尾巴与外部 warning 已清理
     - fresh console 最终 `0 error / 0 warning`
- 当前最核心判断：
  1. `0.0.6` 的 severe 卡爆不是单点 if 错，而是：
     - neutral snapshot / released-state recover 没有真正重启自治 runtime
     - stale director cue 在白天窗口可能回抓
     - crowd 又会在 free-time 放手时批量 immediate 建路
  2. 这轮已经把这三条都各收了一刀，但仍然缺真正的 fresh live retest。
- 当前恢复点：
  1. 最值得下一次 live 先验：
     - `0.0.6` 回 Town 后 `003~203` 是否仍会 severe 卡爆
     - 单个 NPC 是否还需要靠聊天 reset 才恢复
     - opening 结束后是否仍有明显“先卡一下”坏相
  2. 如果还有残余问题，下一刀只该继续查：
     - 是否还存在未覆盖的 `ResumeAutonomousRoam(true)` 群体触发口
     - `NPCAutoRoamController` 是否仍有 lingering blocked-recovery / soft-pass / repath 坏态需要在 NPC 侧补护栏
## 2026-04-16｜109 真实施工：Town 自由时段卡顿根因转向 NPC locomotion 重负载，先收 shared-avoidance / soft-pass 两刀
- 当前主线：
  - 用户 latest live/profiler 已把 `0.0.6` 回 Town 与晚饭前 Town 自由时段的严重卡顿，主热点钉到 `NPCAutoRoamController.Update()`，不再能继续把 `SpringDay1NpcCrowdDirector.Update()` 当唯一主锅。
  - 这轮主目标改成：先按代码和 profiler 事实收 `NPC locomotion` 自己的高频重负载，再回头看 Day1 语义尾账。
- 本轮实际完成：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - 给 `UpdateTraversalSoftPassStateOncePerFrame(...)` 增加位置/状态/时间三重节流：
       - `TRAVERSAL_SOFT_PASS_REFRESH_MIN_INTERVAL_SECONDS`
       - `TRAVERSAL_SOFT_PASS_REFRESH_MIN_MOVE_DISTANCE`
       - `ShouldRefreshTraversalSoftPassState(...)`
     - 给自由漫游稳定移动补了跨帧重决策复用窗口：
       - `AUTONOMOUS_MOVE_DECISION_REUSE_SECONDS`
       - `CanReuseCrossFrameMoveDecisionWindow()`
       - `CanReuseStableMoveDecisionAcrossFrames(...)` 现在允许 autonomous roam 在稳定前进时复用上次重决策，而不是每帧都重跑 shared avoidance + static steering。
  2. `Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs`
     - 新增每帧 `ActiveSnapshotsCache`，把 `GetNearbySnapshots(...)` 从“每个 caller 都重新 `NavigationAgentSnapshot.FromUnit(...)` 一遍”改成“同一帧只建一轮 snapshot，所有 NPC 共用”。
     - 这刀直接对应 profiler 上 shared avoidance 的 `O(N^2)` 放大器。
  3. `Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs`
     - 新增：
       - `NPCAutoRoamController_ShouldReuseCrossFrameDecisionWindow_ForAutonomousRoam`
       - `NPCAutoRoamController_ShouldThrottleTraversalSoftPassRefresh_WhenStateAndPositionAreStable`
       - `NPCAutoRoamController_ShouldRefreshTraversalSoftPass_WhenRoamStateChanges`
       - `NavigationAgentRegistry_ShouldReturnNearbySnapshots_WithoutIncludingSelf`
     - 并复跑 existing registry / static steering 回归。
- 关键判断：
  1. 当前 Town 自由时段卡顿的第一主嫌疑是：
     - `TickMoving -> TryHandleSharedAvoidance -> NavigationAgentRegistry.GetNearbySnapshots -> NavigationLocalAvoidanceSolver.Solve`
  2. 第二主嫌疑是：
     - `TickMoving -> AdjustDirectionByStaticColliders -> ProbeStaticObstacleHits`
  3. `UpdateTraversalSoftPassStateOncePerFrame` 是常驻成本，但比前两条更像放大器而非唯一根；因此这轮先收“节流 + 复用 + snapshot cache”，不直接动 static steering 行为语义。
- 验证：
  1. `validate_script`
     - `NPCAutoRoamController.cs` => `errors=0`（native warning 仍是既有的 “String concatenation in Update() can cause garbage collection issues”）
     - `NavigationAgentRegistry.cs` => `errors=0`
     - `NavigationAvoidanceRulesTests.cs` => `errors=0`
  2. targeted EditMode 回归 `7/7 passed`
     - 3 条新性能护栏
     - 3 条 registry 回归
     - 1 条既有 static steering 回归
  3. fresh console：
     - 一度只剩 test runner 尾巴；已 clear
  4. 过程中出现一次外部 blocker：
     - Unity 当时处于/进入 Play Mode，导致 test run 不能启动；已先 `stop` 回 Edit Mode 后重跑通过。
- 没做/仍待验证：
  1. 还没有 fresh live 复跑 `0.0.6` 回 Town / 晚饭前 Town 的真实体感，所以这轮仍然不是体验终验。
  2. `AdjustDirectionByStaticColliders` 仍是第二嫌疑，若 live 仍卡，下一刀该优先看它的渐进 probe 或其他低风险降载。
  3. `NPCBubblePresenter` hidden-early-return 想法这轮试过但未纳入交付；为了不混入未验收益，已撤回，不把它算进完成。
- 当前恢复点：
  - 用户下一次 fresh live 最该先验：
    1. `0.0.6` 回 Town 后整体帧率是否明显回升
    2. 晚饭前 Town 自由时段是否还会“所有 NPC 都会动，但整体依旧卡爆”
    3. 逐个聊天是否仍会像之前那样对性能有明显“解放”效应
  - 如果仍卡：
    - 下一刀先查 `AdjustDirectionByStaticColliders / ProbeStaticObstacleHits`
    - 再决定是否需要补 `NPCBubblePresenter` 背景耗时治理
- 收尾状态：
  - 本轮已执行 `Park-Slice`
  - `thread-state = PARKED`
  - blocker=`fresh-live-retest-required-for-town-free-time-performance-and-day1-runtime-behavior`
## 2026-04-16｜只读审计：导航V3对“Town 切场 herd-start 风暴”反思与修复方案的独立复核
- 本轮性质：
  - 只读审计，不改代码，不跑 `Begin-Slice`
- 用户最新问题：
  1. 要我独立审核导航V3的反思与修复方案是否可行，不能只复述对方文案。
  2. 重点要分清：哪些判断已经被现码支持，哪些已经过时，哪些即便正确也不足以单独解决用户当前痛点。
- 代码对位后的结论：
  1. 导航对“主烧点在 `NPCAutoRoamController.Update()`、Day1 是主触发不是 steady-state 主锅”的判断，和现码仍然一致：
     - Day1/Crowd 触发口仍在 `Update -> ShouldRecoverMissingTownResidents / RecoverReleasedTownResidentsWithoutSpawnStates / ShouldYieldDaytimeResidentsToAutonomy / YieldDaytimeResidentsToAutonomy`
     - 导航重链仍在 `ResumeAutonomousRoam / StartRoam -> TryBeginMove`，以及 moving 期的 `AdjustDirectionByStaticColliders -> ProbeStaticObstacleHits`
  2. 但导航文案里把当前问题继续描述成“同帧 immediate restart 风暴”已经不完全准确：
     - Day1 现在白天 free-time 主路径大多已改成 `ResumeAutonomousRoam(tryImmediateMove:false)`
     - 当前更准确的是：Day1 仍会在 Town 返场/白天释放窗口里成批把 resident 交回 roam runtime；即使不是立即建路，也会在同一短窗内把一批 controller 送回 `StartRoam -> ShortPause -> TryBeginMove` 链
     - 唯一还明显保留 `tryImmediateMove:true` 的 Day1 路径是 `FinishResidentReturnHome(...)` 的归家结束续跑，不是用户这次骂的 `0.0.6` 白天返 Town 主入口
  3. “黑屏偏长”不能只按导航 herd-start 解释：
     - Day1/Crowd 在 Town 返场补绑时，仍会对 manifest resident 批量做 `FindSceneResident / FindSceneResidentHomeAnchor`
     - `FindSceneResidentHomeAnchor` 内部还会落到 `GameObject.Find`
     - 因此转场刚入 Town 的一次性黑屏/长帧，仍有明确 Day1 own 的 scene-entry 搜索/补绑成本，不能被导航方案一笔带过
  4. 导航方案的方向总体可行，但它把“坏点收紧 / static probe 降载 / blocked 自救更早更便宜”都当成下一刀统一大包，不够收口：
     - 这些点里有不少已经部分落地：
       - traversal soft-pass 刷新节流
       - cross-frame heavy move decision reuse
       - registry active/snapshot cache
       - autonomous bad-point acceptance 收紧
       - static blocked destination 记忆
       - near-target soft arrival
     - 所以它如果继续按“大一统导航重做包”推进，容易重复改、重复调、再次把“已收一半的局部护栏”重写成新一轮泛修
  5. 当前最合理的导航下一刀，不是再泛讲“三根都收”，而是缩成：
     - 专门面向 `primary -> Town` / scene-entry / batch release 的 herd-start 短窗治理
     - 明确目标是“不要让一批 NPC 在同一小窗同时进入完整重链”
     - 而不是再把 Day1 相位、return-home 语义或全局剧情时序回吞到导航里
- 审计判断：
  1. 导航方案“方向上可行”，但“作为单独总解法不够”，因为它没有覆盖 Day1 返场补绑/找 anchor 的一次性入口成本。
  2. 导航方案“需要收窄”，因为其中一半内容已被当前代码部分实现；再按原文全包推进，重复劳动风险高。
  3. 如果要和 Day1-V3 接轨，正确边界应是：
     - Day1 继续收 scene-entry 批量补绑/放手语义
     - 导航只收 batch resume 后的执行层短窗降载与坏启动 cheap-fail
- 最值得继续盯的现码证据：
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
    - `Update()` / `ShouldRecoverMissingTownResidents()` / `RecoverReleasedTownResidentsWithoutSpawnStates()` / `TryRecoverReleasedResidentState()` / `ShouldYieldDaytimeResidentsToAutonomy()` / `ReleaseResidentToAutonomousRoam()`
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - `StartRoam()` / `ResumeAutonomousRoam()` / `TryBeginMove()` / `AdjustDirectionByStaticColliders()` / `ProbeStaticObstacleHits()` / `TryAcquirePathBuildBudget()`
  - `Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs`
    - `GetNearbySnapshots()` / active cache / snapshot cache
## 2026-04-16｜只读汇报判断：Day1-V3 下一刀方向、未完成项、语义同步程度
- 用户最新问题：
  1. 我的下一刀方向是什么。
  2. 当前还有哪些内容没做好。
  3. 语义是否已经完全同步。
- 当前判断：
  1. 下一刀不该先去大包重构导航，也不该先泛修剧情表现；最该先收的是 Day1 own 的 `scene-entry / Town 返场补绑与批量放手` 这条入口链。
  2. 具体要盯：
     - `RecoverReleasedTownResidentsWithoutSpawnStates(...)`
     - `TryRecoverReleasedResidentState(...)`
     - `FindSceneResidentHomeAnchor(...)`
     - `FindAnchor(...)`
     - `ShouldYieldDaytimeResidentsToAutonomy(...)`
     - `ReleaseResidentToAutonomousRoam(...)`
  3. 目的不是再让 CrowdDirector 继续“照顾 NPC”，而是把：
     - Town 返场时的一次性搜索/补绑成本
     - 白天 batch release 的触发密度
     继续往更薄、更一次性的边界收。
  4. 语义还没有“完全同步”：
     - 用户最新明确语义里，至少还有这些没有资格 claim 完全落地：
       - opening / dinner：所有 NPC 先传起点，再只走一次到终点，最多 5 秒，超时 snap 到终点，剧情开始后不再动
       - 白天 free-time：opening 后不再要求 resident 回 anchor；除 20:00 起的夜间流程外，白天就是全图自由漫游
       - Primary 时间：不是一进入 Primary 就停表；应正常流逝到最晚 16:00 才锁住
       - `0.0.6` 回 Town：仍然要确保不会再出现群体卡顿/黑屏长/批量坏态
     - 也就是说，边界判断和部分 runtime 护栏已经对齐，但“你的完整 Day1 语义”还没有资格说完全同步
- 对用户的最短口径：
  - 下一刀：先砍 Day1 自己在 Town 返场/白天释放窗口里的入口成本与批量触发。
  - 还没做好：opening/dinner 一次走位合同、Primary 时间流逝、`0.0.6` 返 Town 体验终验。
  - 语义同步程度：方向已对齐，完整落地仍未完成。

## 2026-04-16｜停在最近刀口：仅做进度收口，不再继续开新刀
- 用户当前要求：
  - 停在最近刀口
  - 言简意赅汇报所有进度
- 当前停点：
  - 这轮不再继续往下砍新的语义/代码刀
  - 只收口到 Day1 own 的 `scene-entry / Town 返场补绑 / 白天批量放手` 入口链
- 当前已确认的进度状态：
  1. Day1 与导航的边界已基本同步到位，但完整 runtime 仍未终验。
  2. `0.0.6` 回 Town 的 severe 卡爆，已经从“只怪 Day1”收窄到“Day1 返场入口成本 + NPC locomotion 重负载”两层。
  3. `opening / dinner` 的一次走位合同、Primary 时间流逝语义、以及 20:00 以后的夜间链仍属于未完全终验项。
- 当前恢复点：
  - 下轮若继续，优先仍是 Day1 own 的返场补绑、白天批量放手和转场入口成本，不先扩成全包重构。

## 2026-04-16｜110 真实施工：晚饭 staged contract 统一重写，先收“一排站队”坏相
- 用户最新强语义：
  1. 晚饭 `18:00 -> 19:30` 期间，`001~203` 都按 opening 同规格 staged contract 处理。
  2. 所有 NPC 只允许：
     - 先到各自起点
     - 再走一次到各自终点
     - 最多等 5 秒
     - 超时 snap 到终点
     - 对话开始后不再 roam / 不再二次走位
  3. 本轮先不碰 `20:00` 回 anchor/home。
- 本轮核心问题定位：
  1. `SpringDay1Director` 晚饭入口仍保留 `001/002` 专用 dinner gathering 逻辑，只对 player 做了入口对齐，和“全员同合同”不一致。
  2. `SpringDay1NpcCrowdDirector` 在 `DinnerConflict_Table` 下会让没有显式 dinner cue 的 resident 因共享 `DinnerBackgroundRoot` semantic anchor 误吃别人的 cue，形成晚饭排成一列的串号坏相。
- 本轮真实改动：
  1. `SpringDay1Director.cs`
     - `PrepareDinnerStoryActorsForDialogue()` 现在把 `003` 也并入晚饭 staged contract。
     - `TryResolveDinnerStoryActorRoute(...)` 改成只接受：
       - scene authored dinner markers
       - dinner beat 自身的 cue
       不再退回 home-anchor / `start=end` 的假路线。
  2. `SpringDay1NpcCrowdDirector.cs`
     - `TryResolveStagingCueForCurrentScene(...)`
       - `DinnerConflict_Table` 下如果 stage book 返回的 cue 不是当前 npcId 的显式 cue，就直接丢弃，不再允许共享 semantic anchor 串号。
       - 对没有显式 dinner cue 的 resident，新加 `TryBuildDinnerFallbackCue(...)`：
         - start = `DirectorReady_*`
         - end = `ResidentSlot_*`
         - 统一生成一次性 dinner fallback route
     - 这样晚饭 crowd resident 不再静默退回共享 DinnerBackgroundRoot / home-anchor fallback。
  3. `SpringDay1DirectorStagingTests.cs`
     - 新增 `003` 的 dinner authored route 测试
     - 新增 dinner fallback cue 生成测试
     - 保留并重跑 dinner cue 不得串 beat / 串 cue 的回归
- 验证：
  1. `SpringDay1Director.cs` / `SpringDay1NpcCrowdDirector.cs` / `SpringDay1DirectorStagingTests.cs` 脚本级 validate 均 `errors=0`
  2. EditMode targeted `4/4 passed`：
     - `Director_ShouldPreferVillageCrowdSceneMarkersForDinnerStoryRoute`
     - `Director_ShouldResolveDinnerRouteForThirdResidentFromSceneMarkers`
     - `CrowdDirector_ShouldBuildDinnerFallbackCueFromReadyAndResidentSlotsWhenStageBookMissingCue`
     - `CrowdDirector_ShouldResolveDinnerCueFromDinnerBeatInsteadOfOpeningMirror`
  3. test runner 尾巴 `TestResults.xml` 已删
  4. fresh console 最终 `0 log entries`
  5. `git diff --check` 已过
- 当前判断：
  1. 这轮已经真实收掉“晚饭进入后 resident 因 dinner cue 串号 / fallback 乱吃而排成一列”这条结构问题。
  2. 这轮还没有资格 claim 晚饭 live 终验完成；现在成立的是：
     - 晚饭入口链已改成同 opening 的 staged contract 方向
     - dinner cue 不再串给错误 NPC
     - `003` 不再游离在晚饭 staged contract 外
- 当前恢复点：
  - 下一轮若继续，先等用户 fresh live retest 晚饭进入瞬间是否还会“一排站队”、是否还会二次走位；`20:00` 回家链仍留待后续处理。
## 2026-04-16 只读彻查：晚饭入口新阻塞定位
- 用户最新 live：进入晚饭后，NPC 被传送到错误位置，剧情不开始，NPC 也不动。
- 结论：这是 Day1-V3 刚刚那轮晚饭半重写带出的新阻塞，不是用户语义变化，也不是单纯导航问题。
- 已钉死的代码因果：
  - SpringDay1Director.ActivateDinnerGatheringOnTownScene() 仍只调用 AlignTownDinnerGatheringActorsAndPlayer()，而该方法只移动玩家，没有把晚饭全员统一送到各自起点。
  - BeginDinnerConflict() 开戏前仍依赖 PrepareDinnerStoryActorsForDialogue() + ResolveDinnerCueSettledState()；前者只处理 001/002/003，后者却在等 DinnerConflict_Table 的 crowd cue settled。
  - SpringDay1NpcCrowdDirector.TryResolveStagingCueForCurrentScene() 已接入 dinner fallback cue；当 stage book 缺显式 cue 时，会把 resident 送去 DirectorReady_* / ResidentSlot_* / DinnerBackgroundRoot 相关锚点。
  - 结果就是：一部分 NPC 被 fallback cue 挪走，但导演层没有一个“全员原子开场 + 全员统一 settled gate”，于是晚饭卡在“人被搬了但对白启动条件没满足”的半状态。
- 当前最像的直接错源：TryBuildDinnerFallbackCue() 使用 DinnerBackgroundRoot 语义锚点作为兜底；Town 场景里确实存在 DirectorReady_DinnerBackgroundRoot / ResidentSlot_DinnerBackgroundRoot，因此会把 NPC 送到用户看到的“另一个地方”。
- 下一刀方向已经明确：晚饭要改成 opening 同款的全员统一 staged contract；不能再保留“001/002/003 特化准备 + crowd 另走一套 cue settled”的双轨入口。
- 本轮未改代码；thread-state 维持 PARKED。
## 2026-04-16 晚饭入口统一 staged contract 第一刀完成
- 当前主线：先把晚饭的“传错位 / 不开戏 / NPC 不动”修干净，再继续 20:00/21:00 的时序退权。
- 本轮完成：
  - SpringDay1Director.cs
    - PreferredVillageCrowdMarkerRootObjectNames 改为优先吃 `进村围观` 分组，不再先被 bare `EnterVillageCrowdRoot` 误导。
    - ActivateDinnerGatheringOnTownScene() 在切到 DinnerConflict 后立即 `SpringDay1NpcCrowdDirector.ForceImmediateSync()`，晚饭 crowd cue 不再等下一拍才上场。
    - BeginDinnerConflict() 在准备 story actor 前先强制一次 crowd sync，保证晚饭入口是同一时间窗的统一 staged 入口。
  - SpringDay1NpcCrowdDirector.cs
    - `ShouldUseVillageCrowdMarkerCueOverride()` 扩到 `DinnerConflict_Table`，晚饭 resident cue 在有场景 marker 时优先使用 `进村围观/起点/终点`，不再回落到 `DirectorReady_DinnerBackgroundRoot` / `ResidentSlot_*` 那条脏兜底。
  - SpringDay1DirectorStagingTests.cs
    - 新增 `Director_ShouldPreferGroupedVillageCrowdMarkersOverBareAnchorRoot`
    - 新增 `CrowdDirector_ShouldPreferSceneMarkersForDinnerFallbackCueWhenMarkersExist`
  - SpringDay1LateDayRuntimeTests.cs
    - 把旧的晚饭 runtime 测试从 `DinnerBackgroundRoot` 语义改成新的“起点 -> 终点”语义。
    - 新语义验证包括：player align 只移动玩家、BeginDinnerConflict 未超时前从起点起步、超时后 snap 到终点。
- 验证：
  - targeted EditMode 5 条 staging 测试：passed
  - targeted EditMode 4 条 late-day runtime 测试：passed
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs` => assessment=`no_red`
  - console: 0 error / 0 warning
- 本轮核心判断：晚饭新坏相的根因就是“导演层拿错 marker 根 + dinner crowd 不吃 scene marker override + 晚饭入口没有同窗 sync”。这一刀已经直接对着这三点收口。
- 当前未继续：20:00 return-home / 21:00 rest / 后续 free-time owner 退权，这些留到下一刀。
## 2026-04-16 只读排查：opening/dinner 垃圾中间层依赖
- 用户裁定：原始语义只有 `进村围观/起点/终点` 作为 opening+dinner 剧情走位真值，`001~203_HomeAnchor` 作为 NPC 回家真值；`Town_Day1Residents` / `Town_Day1Carriers` 及其子节点是污染源，原则上要删。
- 本轮只读查明：这些垃圾层不是纯场景装饰，已经被多层代码/资产/测试消费：
  - 场景：Town.unity 存在 `Town_Day1Carriers`、`Town_Day1Residents`、`Resident_DefaultPresent`、`Resident_DirectorTakeoverReady`、`Resident_BackstagePresent`、`ResidentSlot_*`、`DirectorReady_*`、`DailyStand_*`、`DinnerBackgroundRoot`、`NightWitness_01`。
  - runtime：SpringDay1NpcCrowdDirector.cs 常量绑定 `ResidentRootName` / `CarrierRootName`；ApplyStagingCue 会把 NPC reparent 到 `_carrierRoot`；ApplyResidentBaseline/ResolveResidentParent 会把 resident reparent 到 `_residentDefaultRoot/_residentTakeoverReadyRoot/_residentBackstageRoot`；FindAnchor/EnumerateSemanticAnchorAliasNames 会自动尝试 `DirectorReady_*` / `ResidentSlot_*` / `BackstageSlot_*`；TryBuildDinnerFallbackCue 会吃 `DirectorReady_DinnerBackgroundRoot` 和 `ResidentSlot_DinnerBackgroundRoot`。
  - 数据：SpringDay1NpcCrowdManifest.asset 的 semanticAnchorIds 仍大量包含 `EnterVillageCrowdRoot`、`DinnerBackgroundRoot`、`DailyStand_*`、`NightWitness_01`。
  - 数据：SpringDay1DirectorStageBook.json 仍大量含 `EnterVillageCrowdRoot` / `DinnerBackgroundRoot` / `NightWitness_01` / `DailyStand_*`。
  - 数据：SpringDay1TownAnchorContract.json 仍是这些抽象锚点的 fallback 数据源。
  - 测试：多条测试仍把这些抽象锚点当应保契约，尤其 NpcCrowdManifestSceneDutyTests、SpringDay1DialogueProgressionTests、SpringDay1DirectorStagingTests。
- opening 里也有垃圾：
  - `SceneTransitionTrigger2D.TownOpeningEntryAnchorName = "EnterVillageCrowdRoot"` 仍把 Town 入口锚定到抽象 root，而不是玩家/剧情真实点位。
  - `SpringDay1NpcCrowdDirector.TryResolveStagingCueForCurrentScene` 仍会通过 StageBook 解析 `EnterVillage_PostEntry` 的 `EnterVillageCrowdRoot` cue，只是 runtime marker override 会在有 `进村围观` marker 时覆盖；这属于“脏 fallback 还活着”。
- 当前判断：这些层没有不可替代语义，但删除前要先断代码/数据/测试依赖；否则会爆编译或把 runtime fallback 打空。
- 下一步建议：先改代码禁用/移除这些抽象 alias 和 parent 分组依赖；再改 manifest/stagebook/contract/tests；最后删 Town 场景节点。
## 2026-04-16 只读盘点：彻底删除抽象锚点体系时容易漏炸的文件
- 用户要求：只做只读分析，盘点如果彻底删除 `Town_Day1Residents` / `Town_Day1Carriers` / `DirectorReady_*` / `ResidentSlot_*` / `DinnerBackgroundRoot` / `DailyStand_*` / `NightWitness_01`，当前仓库里哪些数据资产、测试、场景入口常量会直接炸，并给出最小安全删除顺序。
- 这轮新增钉死的“容易漏项”：
  1. Editor/Scene 工具链不只是文档噪音，而是会继续把抽象锚点当成 readiness/contract/migration 真值：
     - `Assets/Editor/Town/TownSceneRuntimeAnchorReadinessMenu.cs`
     - `Assets/Editor/Town/TownScenePlayerFacingContractMenu.cs`
     - `Assets/Editor/Town/TownNativeResidentMigrationMenu.cs`
     - `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
     - `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
  2. opening 入口常量除了 `SpringDay1Director` 自己的 root fallback 外，还有：
     - `SceneTransitionTrigger2D.TownOpeningEntryAnchorName = "EnterVillageCrowdRoot"`
     这会让 `Primary -> Town` 的默认 entry 继续吃旧抽象锚点。
  3. 数据资产里除了 manifest / stagebook / town-anchor-contract，还有 `NpcCharacterRegistry.asset` 的 `relationshipBeatSemantics` 继续显式引用 `EnterVillage_PostEntry / DinnerConflict_Table / FreeTime_NightWitness / DailyStand_Preview`；它不一定因删 anchor 直接编译炸，但会继续把旧 beat 体系当正式语义消费。
  4. `SpringDay1Director` 里仍有晚饭 fallback 常量：
     - `PreferredDinnerGatheringAnchorObjectNames = { "DirectorReady_DinnerBackgroundRoot", "DinnerBackgroundRoot" }`
     删除抽象锚点体系前若不先切，会直接让晚饭入口找空。
  5. `SpringDay1NpcCrowdDirector` 的 alias / fallback / parent 依赖仍是抽象锚点总入口：
     - `EnumerateSemanticAnchorAliasNames(...)`
     - `TryResolveDinnerFallbackStartPosition(...)`
     - `TryResolveDinnerFallbackEndPosition(...)`
     - `ResolveDinnerFallbackSemanticAnchor(...)`
     - `ReparentState(...)`
- 最小安全删除顺序补充：
  1. 先切 opening/dinner/late-day 的入口常量和 runtime fallback，让导演与 crowd 不再查 `EnterVillageCrowdRoot` / `DirectorReady_*` / `ResidentSlot_*` / `DinnerBackgroundRoot` / `DailyStand_*` / `NightWitness_01`
  2. 再切数据资产：`SpringDay1NpcCrowdManifest.asset`、`SpringDay1DirectorStageBook.json`、`SpringDay1TownAnchorContract.json`，必要时同步 `NpcCharacterRegistry.asset`
  3. 再切 Editor 工具与测试旧断言，避免删除后工具菜单持续报假 blocker
  4. 最后才删 `Town.unity` 里的 `Town_Day1Residents` / `Town_Day1Carriers` 及整套抽象节点
## 2026-04-16｜切片 112：误删的 101~203 与独立 HomeAnchor 已恢复到 NPCs
- 用户最新阻塞：
  - 我在删 `Town_Day1Residents / Town_Day1Carriers` 时把 `101~203` 一并删掉了；用户要求不要恢复旧脏根，只把这些常驻村民和各自 `*_HomeAnchor` 干净补回 `NPCs`。
- 本轮真实施工：
  1. 沿用 `112_remove_day1_abstract_anchor_layers`，不回滚旧抽象根。
  2. 从旧 `Town.unity` 倒出 `101_HomeAnchor~203_HomeAnchor` 的历史位置，确认正确恢复目标是：
     - `NPCs/101~203`
     - `NPCs/101_HomeAnchor~203_HomeAnchor`
  3. 通过 Unity 现场把 `101/102/103/104/201/202/203` prefab 重新实例化到 `NPCs`。
  4. 同时创建并恢复：
     - `101_HomeAnchor`
     - `102_HomeAnchor`
     - `103_HomeAnchor`
     - `104_HomeAnchor`
     - `201_HomeAnchor`
     - `202_HomeAnchor`
     - `203_HomeAnchor`
  5. 逐个把新增 resident 的 `NPCAutoRoamController.homeAnchor` 重新绑定到对应 `*_HomeAnchor`；`navGrid` 仍保持 `NavigationRoot`。
  6. 保持删除结果不回流：
     - `Town_Day1Residents` 不恢复
     - `Town_Day1Carriers` 不恢复
     - `EnterVillageCrowdRoot` 不恢复
- 场景结果：
  - `Town.unity` 里现在保留 `TownPlayerEntryAnchor`，且重新出现 `101_HomeAnchor~203_HomeAnchor`。
  - `101~203` 已重新回到 `NPCs` 链下，scene resident 结构恢复成“独立 NPC + 独立 HomeAnchor”。
- 自验：
  1. targeted EditMode `9/9 passed`：
     - `SpringDay1DirectorStagingTests.Director_ShouldPreferVillageCrowdSceneMarkersForDinnerStoryRoute`
     - `SpringDay1DirectorStagingTests.Director_ShouldResolveDinnerRouteForThirdResidentFromSceneMarkers`
     - `SpringDay1DirectorStagingTests.Director_ShouldPreferGroupedVillageCrowdMarkersOverBareAnchorRoot`
     - `SpringDay1DirectorStagingTests.Director_ShouldIgnoreBareEnterVillageCrowdRootWhenDinnerMarkersAreMissing`
     - `SpringDay1DirectorStagingTests.Director_ShouldRequireTownOpeningStartAndEndMarkersInGroupedRoot`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldRequireSceneMarkersForDinnerCue`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldBuildDinnerCueFromSceneMarkers`
     - `SpringDay1OpeningRuntimeBridgeTests.GenericPrimaryTransitionTrigger_ShouldFallbackToTownOpeningEntryAnchor`
     - `SpringDay1OpeningRuntimeBridgeTests.PersistentPlayerSceneBridge_ShouldTreatPipeSeparatedEntryNamesAsAliases`
  2. `git diff --check -- Assets/000_Scenes/Town.unity` 通过；Town 场景新增的行尾空白已清掉。
  3. fresh console 最终=`errors=0 warnings=0`。
  4. `Assets/__CodexEditModeScenes` 与 `TestResults.xml` 这类测试副产物已清理。
- 当前恢复点：
  - 场景误删已修复；下一轮可回到 Day1 抽象锚点退场后的剩余 runtime/owner 清扫，不需要再为 `101~203` 缺席补锅。
- 收尾：
  - 已执行 `Park-Slice`
  - 当前状态=`PARKED`
  - 当前 blocker=`checkpoint-112-town-residents-restored-ready-for-next-runtime-cleanup`

## 2026-04-16｜只读调查：用户提出的 5 个语义问题已对到代码真相
- 本轮性质：
  - 只读调查，不进入真实施工；thread-state 继续维持 `PARKED`。
- 用户这轮要求我彻查 5 件事：
  1. `opening` 后所有 NPC 到底该干什么
  2. “走到艾拉附近”现在判多近
  3. 做完任务回 `Town` 时 `001/002` 应该站哪
  4. `19:00` 的提醒剧情到底怎么处理
  5. `001~003` 的家在哪里、晚上会不会回家
- 已钉死的代码结论：
  1. `opening` 后不是“所有 NPC 一起立刻自由活动”：
     - 普通 resident（`101~203` + synthetic `003`）在 `currentPhase > EnterVillage` 且不处于 `DinnerConflict_Table / ReturnAndReminder_WalkBack / DayEndSettle / DailyStandPreview` 时，才会被 `SpringDay1NpcCrowdDirector.ShouldYieldDaytimeResidentsToAutonomy()` 放回白天自治。
     - `001/002` 仍由 `SpringDay1Director` 继续当 story actor 持有，直到离开 Healing / Workbench / FarmingTutorial 的前段合同。
  2. “走到艾拉附近”的主阈值现在确实是 `healingSupportApproachDistance = 0.9f`，但它比较的是玩家 `interaction sample point` 与艾拉位置，不是单纯 player transform。
  3. `0.0.6` 回 `Town` 时，导演层并没有额外定义 `001/002` 的白天待机点：
     - `AlignTownDinnerGatheringActorsAndPlayer()` 只会移动玩家。
     - 晚饭开始前，`001/002` 会停在 `Town.unity` 自身的 scene 原生摆位；当前 scene 真值是：
       - `001 = (-12.55, 14.52)`
       - `002 = (-10.91, 16.86)`
  4. `19:00` 不是“NPC 剧情已经过完”的时刻：
     - `BeginReturnReminder()` 会把相位切到 `ReturnAndReminder`，并把受管控时间夹到 `19:00`
     - `EnterFreeTime()` 才在 `19:30` 把相位切到 `FreeTime`
     - 因此 `19:00~19:29` 仍是提醒剧情段，不是完全自由段
  5. `001~003` 都有 scene home anchor：
     - `001_HomeAnchor = (-12.15, 13.12)`
     - `002_HomeAnchor = (-17.67, 13.94)`
     - `003_HomeAnchor = (43.70, -10.70)`
     - `001/002` 的夜间回家由 `SpringDay1Director.SyncStoryActorNightRestSchedule()` 控
     - `003` 不再归导演夜间 owner，而是作为 synthetic resident 走 `SpringDay1NpcCrowdDirector.SyncResidentNightRestSchedule()`
- 当前最重要的偏差判断：
  1. 用户白天语义里“不要求 resident 回 anchor，只要 20:00 后再回家”这条，现码已经基本对齐。
  2. `19:00` 这段在现码里仍然是正式剧情段，不是已经放人后的纯自由活动。
  3. `0.0.6` 回 `Town` 时 `001/002` 没有单独“导演指定站位”；它们现在吃的是 scene 原生位置，这点必须和用户期待分开看。
- 本轮未改代码、未跑 compile/live；只完成了代码/scene 交叉调查。

## 2026-04-17｜只读彻查：打包版最新反馈已收成问题总表
- 本轮性质：
  - 只读彻查，不进入真实施工；thread-state 继续维持 `PARKED`。
- 用户新增打包版反馈：
  1. `opening` 里 `101~203` 先走位，`001~003` 后走位；结束后 `101~203` 还会直接传回 baseline/anchor，而不是原地解散。
  2. 疗伤靠近艾拉触发回血要改成 `1.6`，其余不动。
  3. `0.0.6` 回 `Town` 时只要求确认 `001/002` 白天站位不乱改。
  4. `19:00` 附近除 `001~003` 外其他 resident 体感上像“全没了/回家了”；`003` 可聊天，`001/002` 不能。
  5. 用户希望夜间统一成：
     - `20:00` 开始回 home anchor
     - `21:00` 传到 anchor 后隐藏
     - 次日从 anchor 激活
- 本轮新钉死的代码问题：
  1. `opening` 仍是双轨 owner：
     - `101~203` 由 `SpringDay1NpcCrowdDirector` 的 staged cue 先跑
     - `001~003` 由 `SpringDay1Director` 的 village-gate story actor 路线单独跑
     - `SpringDay1NpcCrowdDirector` 还有 `[DefaultExecutionOrder(-300)]`，天然先于导演层执行
  2. `opening` 结束后 crowd resident 不是“原地解散”：
     - cue 释放后会走 `TryReleaseResidentToDaytimeBaseline(...)`
     - 该方法会直接把 resident 传到 `BasePosition`
     - 所以用户看到的“直接传回 baseline/anchor”是当前代码真相
  3. `003` 的身份目前是“剧情内仍吃导演 opening/dinner 路线、剧情外 synthetic 并回普通 resident”
     - 这说明它对白天/夜间 ordinary resident 身份基本对上了
     - 但 opening 入口仍和 `001/002` 一起吃导演链，没有彻底并成“全员同合同”
  4. `19:00` 时普通 resident 的异常不是时钟回家合同：
     - crowd 的 clock return-home 明确是 `20:00~20:59`
     - `21:00+` 才 forced rest
     - 因此 `19:00` 的异常更像 beat 切换后的 baseline teleport / runtime 可见性偏差，而不是夜间时钟语义本身
  5. `19:00` 时 `001/002` 不可聊天、`003` 可聊天，是当前代码合同结果：
     - `001/002` 在 `DinnerConflict / ReturnAndReminder` 仍属 story actor mode，会被导演层禁 formal/informal chat
     - `003` 在 Town 里没有跟着 `001/002` 进这条 story-actor runtime policy
  6. 夜间合同仍分裂：
     - `001/002` 走 `SpringDay1Director.SyncStoryActorNightRestSchedule()`
     - `003~203` 走 `SpringDay1NpcCrowdDirector.SyncResidentNightRestSchedule()`
     - 当前实现是 `20:00` 请求回家、`21:00` snap/rest，但不会隐藏
     - 也没有“次日统一从 anchor 激活”的 clean contract
- 当前判断：
  1. 这轮最新反馈里最硬的 Day1 runtime 问题不是单一参数，而是：
     - opening 双轨 owner
     - cue release 后 baseline teleport
     - 夜间合同分裂且不隐藏
  2. 艾拉回血距离则是明显的单参数刀，独立且低风险。
- 本轮未改代码、未跑 compile/live；只完成问题收束与风险评估前置调查。

## 2026-04-17｜只读彻查：opening 入口为何分裂、最小统一点在哪、哪里最容易二次摆位
- 本轮性质：
  - 只读彻查，不进入真实施工；thread-state 继续维持 `PARKED`。
- 用户这轮要求我只回答 opening 入口 contract：
  1. 为什么 `001~003` 会走单独链
  2. 哪些最小改动能把 opening 统一到一份 staged contract
  3. 哪些 call-site / 状态位最容易造成“先到终点又回起点 / 或二次摆位”
- 已钉死的代码结论：
  1. `001/002` 之所以天然走单独链，不是偶发，而是 `SpringDay1NpcCrowdDirector.ShouldDeferToStoryEscortDirector(...)` 明确把它们在 `EnterVillage / HealingAndHP / WorkbenchFlashback / FarmingTutorial / DinnerConflict / ReturnAndReminder` 全部留给导演 owner。
  2. `003` 在 opening 也没有和 ordinary resident 合流：
     - `SpringDay1NpcCrowdDirector.ShouldIncludeThirdResidentInResidentRuntime(...)` 只有 `currentPhase > EnterVillage` 才把 `003` synthetic 并回 resident runtime；
     - 因此 opening 时 `003` 也只能跟 `SpringDay1Director.TryHandleTownEnterVillageFlow()`、`TryPrepareTownVillageGateActors()`、`ForcePlaceTownVillageGateActorsAtTargets()` 走导演链。
  3. ordinary resident 的放手链和 `001~003` 的 story actor 链当前是两份合同：
     - 导演层用 `TryResolveTownOpeningLayoutPoints()` / `TryResolveTownOpeningMarker()` / `TryResolveTownVillageGateActorRoute()` 给 `001~003` 做 start->end；
     - crowd 层则根据 `ShouldReleaseEnterVillageCrowd()`、`ShouldLatchEnterVillageCrowdRelease()`、`ShouldSuppressEnterVillageCrowdCueForTownHouseLead(...)` 提前抑制 `EnterVillagePostEntry` cue，并在 `ApplyResidentBaseline(...) -> TryReleaseResidentToDaytimeBaseline(...)` 里把 ordinary resident release 到 baseline/autonomy。
  4. opening beat 在 `EnterVillage` 相位内就会中途切档：
     - `SpringDay1Director.GetCurrentBeatKey()` 只要 `ShouldUseEnterVillageHouseArrivalBeat()` 成立，就会从 `EnterVillagePostEntry` 切到 `EnterVillageHouseArrival`；
     - 而 `ShouldUseEnterVillageHouseArrivalBeat()` 现在不只看 `HouseArrivalSequenceId`，还看 `HasVillageGateCompleted()`、`_townHouseLeadStarted`、`_townHouseLeadTransitionQueued`；
     - 这意味着 crowd release 和 director escort 会在同一相位内提早分叉。
- 当前最小统一改动点：
  1. opening actor route 只保留一份来源：
     - 优先统一到 `SpringDay1DirectorBeatKeys.EnterVillagePostEntry` 对应的 `SpringDay1DirectorStageBook` cue；
     - 也就是让 `TryPrepareTownVillageGateActors()` / `ForcePlaceTownVillageGateActorsAtTargets()` 不再混用 `TryResolveTownOpeningLayoutPoints()`、`TryResolveTownOpeningMarker()`、`TryResolveTownVillageGateCueTarget()`、`TryResolveTownVillageGateHardFallbackTarget()` 四套来源。
  2. `ShouldReleaseEnterVillageCrowd()` 与 `ShouldLatchEnterVillageCrowdRelease()` 不应再被 `_townHouseLeadStarted / _townHouseLeadTransitionQueued / _townHouseLeadWaitingForPlayer` 提前触发；
     - 最小收法是把 release 条件收窄到真正完成 opening staged contract 的单一 latch，而不是“village gate 刚过 / house lead 刚起步”就放人。
  3. `ShouldUseEnterVillageHouseArrivalBeat()` 也应同步收窄：
     - 不要再让 `_townHouseLeadStarted / _townHouseLeadTransitionQueued` 直接把 beat 从 `EnterVillagePostEntry` 切到 `EnterVillageHouseArrival`；
     - 最小正确口径是只在 `HouseArrivalSequenceId` active / consumed，或显式 opening-complete latch 成立后再切。
  4. `001~003` 的 opening actor 集合应只在一处定义：
     - 现在 `001/002` 由 `ShouldDeferToStoryEscortDirector(...)` 控，`003` 由 `ShouldIncludeThirdResidentInResidentRuntime(...)` 反向排除；
     - 最小收法是抽成同一份 opening story actor 集合，让 crowd/runtime 与 director 都读同一个入口真值。
- 当前最容易导致“先到终点又回起点 / 或二次摆位”的 call-site / 状态位：
  1. `TickTownSceneFlow()` 每轮固定先后调用：
     - `TryAdoptTownOpeningState()`
     - `TryHandleTownEnterVillageFlow()`
     - `MaintainTownVillageGateActorsWhileDialogueActive()`
     这意味着同一轮里既可能先做 prepare，又可能立刻做 force-place hold。
  2. `TryPrepareTownVillageGateActors()` 里的 `_townVillageGateActorsPlaced` / `_townVillageGateActorsWaitStartedAt`
     - 第一次会先把 actor `Reframe` 到 start；
     - 后续再 drive/snap 到 end；
     - 只要 `_townVillageGateActorsPlaced` 被清零，就有再次回 start 的风险。
  3. `ResetTownVillageGateDialogueSettlementState()`
     - 它会同时清 `_townVillageGateCueWaitStartedAt`、`_townVillageGateActorsWaitStartedAt`、`_townVillageGateActorsPlaced`；
     - 这类“cue wait reset + actor placed reset”绑在一起，是最危险的二次摆位源。
  4. `ForcePlaceTownVillageGateActorsAtTargets()`
     - 它会无条件按 end route snap `001~003`；
     - 一旦前一轮 prepare 仍在使用另一套 start/end 解析源，就会出现先被一套 route 放到终点、下一轮又被另一套 route 重新解释的坏相。
  5. `HandleDialogueSequenceCompleted(VillageGateSequenceId)` -> `TryBeginTownHouseLead()` / `TryQueuePrimaryHouseArrival()`
     - village gate 一收束就立刻进入下一段 escort/transition；
     - 如果 crowd release 与 beat 切档已经提前发生，就会在“opening 刚结束”和“house lead 刚开始”之间叠出第二段摆位。
  6. `TryQueuePrimaryHouseArrival()` 的多处调用：
     - `HandleDialogueSequenceCompleted(...)`
     - `TickPrimarySceneFlow()`
     - `TryRecoverConsumedSequenceProgression(...)`
     它本身有 guard，但和前面的 beat 切档 / release latch 叠在一起时，仍是最容易制造中途重排的入口。
- 当前判断：
  1. 这轮 highest-confidence root cause 不是导航执行层，而是 opening 在导演层与 crowd 层同时维护：
     - actor set
     - beat 切档
     - release latch
     - route source
     这四件事。
  2. 如果后续进入真实施工，最值钱的一刀不是大改导航，而是先把 `TryPrepareTownVillageGateActors()`、`ForcePlaceTownVillageGateActorsAtTargets()`、`ShouldReleaseEnterVillageCrowd()`、`ShouldUseEnterVillageHouseArrivalBeat()` 收成同一份 staged contract。
- 本轮未改代码、未跑 compile/live；只完成 opening contract 静态定责与最小改动点收束。

## 2026-04-17｜只读窄审：20:00 后 resident return-home 能否去掉 `TickResidentReturnHome()` 持续代管
- 本轮性质：
  - 只读分析，不进入真实施工；thread-state 未登记，继续维持 `PARKED`。
- 当前主线目标：
  - 只回答一个窄问题：在当前 `SpringDay1NpcCrowdDirector -> NPCAutoRoamController` 合同下，`20:00` 后 resident return-home 能不能从“每帧 drive/retry/finish”收成“只在时序边缘下发一次或少量指令，然后不再持续代管 arrival/finish”。
- 已钉死的代码链路：
  1. `CrowdDirector.Update()` 每帧固定先跑 `SyncResidentNightRestSchedule()` 再跑 `TickResidentReturns()`。
  2. `20:00~20:59` 的夜间回家入口有两处：
     - `SyncResidentNightRestSchedule()` 命中 `ShouldResidentsReturnHomeByClock()` 后直接 `TryBeginResidentReturnHome(state)`；
     - `ApplyResidentBaseline()` 在 cue release 后若仍命中时钟窗口，也会 `TryBeginResidentReturnHome(state)`。
  3. `TryBeginResidentReturnHome(state)` 只负责：
     - 把 `state.IsReturningHome=true`
     - 尝试一次 `TryDriveResidentReturnHome(...)`
     - 若失败则写 `NextReturnHomeDriveRetryAt`
  4. 真正的持续代管在 `TickResidentReturnHome(state, deltaTime)`：
     - 到家阈值判断
     - `HasActiveResidentReturnHomeDrive(...)` 轮询
     - `NextReturnHomeDriveRetryAt` 到点后反复补发
     - 最终调用 `FinishResidentReturnHome(state)`
  5. `TryDriveResidentReturnHome(...)` 当前已经不再直接推 transform，而是走
     - `NPCAutoRoamController.RequestReturnToAnchor(...)`
     - `RequestReturnHome(...)`
     - `DriveResidentScriptedMoveTo(..., PointToPointTravelContract.FormalNavigation, ...)`
  6. 但 `NPCAutoRoamController` 在 `FormalNavigation` 到达后，当前只会在 `EndDebugMove(reachedDestination:true)` 内部记录：
     - `lastFormalNavigationArrivalTime`
     - `lastFormalNavigationArrivalPosition`
     - 并 `HaltResidentScriptedMovement()`
     - 不会主动 release `resident scripted control`，也不会向 `CrowdDirector` 发 arrival/finish 信号。
- 当前判断：
  1. 以现合同看，**不能**直接把 `TickResidentReturnHome()` 整体删掉。
  2. 可以收成“边缘下发 + 不再每帧补 drive”的前提，是先把“arrival/finish 从 crowd 外层代管，迁进导航合同可观测信号”补出来；否则 `CrowdDirector` 仍是唯一知道什么时候该 `FinishResidentReturnHome()` 的地方。
- 最小可行切口：
  1. 保留 `20:00` 的时序入口仍在 `CrowdDirector`。
  2. 让 `CrowdDirector` 只做一次 `RequestReturnToAnchor(...)` 下发，不再每帧 `retry/finish`。
  3. 新增一个非常窄的导航完成信号，让 `CrowdDirector` 只在“边缘/事件”上收尾：
     - 例如按 `ownerKey + FormalNavigation` 暴露一次性 completion/arrival consume 口
     - 或显式 `ReturnHomeCompleted` 回调 / 轮询信号
  4. `FinishResidentReturnHome()` 的 owner release、`IsReturningHome=false`、后续 `resume roam / night-rest` 仍保留在 `CrowdDirector`，但触发时机改成“收到导航完成信号”，而不是继续自己靠 transform 距离和 active-drive 轮询。
- 如果现在直接删 `TickResidentReturnHome()`，最可能立刻坏的点：
  1. `TryBeginResidentReturnHome()` 首次下发失败后，不会再有 `0.35s` retry；被临时阻挡或首帧不可达的 resident 会直接卡在 `IsReturningHome=true`，直到 `21:00` forced rest 才被 snap。
  2. 即使 `NPCAutoRoamController` 已经走到 anchor，`CrowdDirector` 也不会再调用 `FinishResidentReturnHome()`：
     - `state.IsReturningHome` 不会清
     - scripted owner 不会 release
     - `NeedsResidentReset / ReleasedFromDirectorCue` 相关语义不会回到正常 baseline/autonomy 终态
  3. `ApplyResidentBaseline()` 会因为 `state.IsReturningHome` 仍为真，持续走 `:return-home` 分支提早返回，白天/剧情释放分支拿不到清洁终态。
  4. `20:00~20:59` 这一个小时里，resident 更容易表现成“到家后僵住但 owner 还没交回”，而不是“回家完成并正式收口”。
- 当前最值得先补的单一接口 / 信号：
  1. 优先级最高的不是再补一个新的 `Request...` 入口，而是补一个**FormalNavigation 完成可观测信号**。
  2. 最小正确形态应至少能让外层区分：
     - 这次 `FormalNavigation` 是谁发的
     - 是否已到达
     - 这次完成信号是否已经被消费
  3. 只要这一个信号成立，`TickResidentReturnHome()` 就有机会收成“时序边缘下发 + completion 边缘收尾”；在这之前，删 tick 只会把 arrival/finish 责任留空。
- 验证状态：
  - 纯静态审计，未改代码，未跑 compile/live。
- 当前恢复点：
  1. 如果后续进入真实施工，第一刀应先补 `NPCAutoRoamController` 的 `FormalNavigation completion` 对外信号，再考虑收 `TickResidentReturnHome()`。
  2. 在那个信号出现前，不要把“已经改成 `RequestReturnToAnchor(...)`”误判成“arrival/finish 已经归导航自己接管”。

## 2026-04-17｜Package E 第一刀：夜间 split contract 已收平到 crowd，`E-08` blocker 已钉死
- 本轮性质：
  - 真实施工；沿 `0417` 的 `Package E` 继续推进。
- 当前主线目标：
  - 先把 `001/002` 从导演夜间私链里退出来，收齐全 NPC 统一的 `20:00/21:00` 夜间合同第一刀，但不把结构成立偷换成 live 过线。
- 本轮实际落地：
  1. `SpringDay1NpcCrowdDirector.GetRuntimeEntries(...)`
     - 在 `FreeTime / DayEnd` 把 `001/002/003` 一起纳回 resident runtime；
     - `001/002` 不再夜间 split 成 director 私链。
  2. `SpringDay1Director.SyncStoryActorNightRestSchedule()`
     - 退成只做 release，不再持续管理 `001/002` 的夜间 return-home / rest。
  3. `SpringDay1NpcCrowdDirector.SyncResidentNightRestSchedule()` + `ApplyResidentNightRestState(...)`
     - 已补统一 `21:00 snap + hide`；
     - `HandleSleep()` 现在会清 `SpringDay1NpcCrowdDirector` runtime snapshots，避免把 Day1 夜间隐藏状态带进次日。
  4. `0417.md`
     - 已新增 `C-15 / C-16 / C-17`；
     - `Package E` 状态改成 `进行中`；
     - `E-01 / E-02 / E-04 / E-05 / E-06` 已打勾；
     - `E-08` 已明确写成 blocker：缺 `FormalNavigation completion` 对外信号。
  5. 测试层已同步：
     - `SpringDay1DirectorStagingTests.CrowdDirector_RuntimeEntries_ShouldIncludeStoryActorsDuringFreeTimeNightContract`
     - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldNotKeepNightRestResidentsUnderCrowdScriptedOwner`
     - `SpringDay1LateDayRuntimeTests.StoryActorsNightRestSchedule_ShouldStandDownForUnifiedResidentNightContract`
- 本轮额外判断：
  1. 我曾试探过把 `TickResidentReturnHome()` 往“只发起不 finish”再退一刀；
  2. 但在只读 explorer 审计后，确认这条现在还不能安全删：
     - `NPCAutoRoamController` 还没有把 `FormalNavigation` 到达/完成回执给 `CrowdDirector`
     - 直接删 tick 会丢 arrival/finish 收口
  3. 这段试探性改动已在本轮内撤回，没有把现场留在半退权状态。
- 验证：
  1. `validate_script`：
     - `SpringDay1NpcCrowdDirector.cs` `errors=0`
     - `SpringDay1Director.cs` `errors=0`
     - `SpringDay1DirectorStagingTests.cs` `errors=0`
     - `SpringDay1LateDayRuntimeTests.cs` `errors=0`
  2. EditMode 定向 tests：
     - 共 `8` 条，`passed=8 failed=0`
     - 覆盖 `runtime entries / 20:00 / 21:00 / director 夜链退场 / 02:00 forced sleep`
  3. `git diff --check`：
     - 当前 own 文件通过。
  4. fresh console：
     - 无我这轮新引入红错；
     - 仅见外部 `Screen position out of view frustum` 旧噪声。
- 当前未完成：
  1. `E-03`：crowd 自己的 night runtime 仍是私房模型。
  2. `E-07`：次日激活只有“clear snapshot”结构保护，live 仍待证。
  3. `E-08`：还缺 `FormalNavigation completion` 信号，不能安全去掉 `TickResidentReturnHome()` 的 arrival/finish 职责。
  4. `E-09`：`T-09/T-10/T-11/T-12` 的整包还没补齐到 full live。
- 当前恢复点：
  1. 下一刀不要再碰 dinner；
  2. 正确入口是先补 `NPCAutoRoamController` 面向 `ownerKey + FormalNavigation` 的 completion/arrival 可观测信号；
  3. 信号成立后，再回来收 `E-08` 的 tick/retry/finish 退权。

## 2026-04-17｜Package F 补记：`TownNativeResidentMigrationMenu.cs` 的 `CS0133` 已清
- 本轮性质：
  - 真实施工收口；只收用户刚报出的 editor compile 红，不扩 runtime。
- 当前主线目标：
  - 沿 `0417.md / Package F` 把 editor/tooling 侧旧链路继续降级，同时确保 shared root 不留我 own 的编译红。
- 本轮实际动作：
  1. `Assets/Editor/Town/TownNativeResidentMigrationMenu.cs`
     - 把 4 个 `const string = string.Empty` 改成编译期常量字面量 `""`；
     - 对应报错行是 `TownResidentRootName / TownResidentDefaultRootName / TownResidentTakeoverRootName / TownResidentBackstageRootName`。
  2. fresh 验证：
     - `validate_script Assets/Editor/Town/TownNativeResidentMigrationMenu.cs`：
       - `manage_script validate = clean`
       - compile-first assessment = `unity_validation_pending`
       - 原因是 Unity `stale_status`，不是脚本自身仍红。
     - `status`：bridge/baseline 正常、fresh console 为空。
     - `errors`：已回到 `0 error / 0 warning`。
     - `git diff --check -- Assets/Editor/Town/TownNativeResidentMigrationMenu.cs`：通过。
- 当前判断：
  1. 用户刚报的 4 条 `CS0133` 已经清掉。
  2. 这轮没有新增 runtime 结论；只是把 editor/tooling 侧的 compile blocker 收干净并记回主控板。
- 当前恢复点：
  1. 后续继续 `0417 / F-05 ~ F-07` 时，不要再把这 4 条旧编译红当现场真相。
  2. 下一刀仍应回到 stale editor/tests 清理，而不是回 runtime 散修。

## 2026-04-17｜进度盘点：0417 尚未完成，5 个子智能体均为只读审计结论
- 本轮性质：
  - 只读进度盘点；未新增 runtime 施工。
- 当前主线目标：
  - 给用户一个可下决策的真实状态：`0417` 还剩多少、5 个子智能体停在什么结论、现在是否可以打包。
- 当前总进度（按 `0417.md`）：
  - `TOTAL = 41 done / 27 todo`
  - `Package A = 6/6`
  - `Package B = 4 done / 5 todo`
  - `Package C = 4 done / 4 todo`
  - `Package D = 8 done / 2 todo`
  - `Package E = 8 done / 1 todo`
  - `Package F = 4 done / 5 todo`
  - `Package G = 0 done / 7 todo`
- 5 个子智能体已全部收回并关闭；它们都没有新增代码施工，只带回只读结论：
  1. `Averroes`
     - 高置信指出晚饭入口 `001/002` 仍是延迟起步、夜间 `001/002/003` 仍有显隐双 owner 风险。
  2. `Halley`
     - 钉住 `E-08` 仍有一层 owner 读取残留；可继续砍，但不该误判成已经完全归零。
  3. `Russell`
     - 明确 dinner 仍存在 `001/002` 导演私链与 ordinary resident crowd 链分叉，`D-02/D-03` 尚未真正完成。
  4. `Newton`
     - 明确 `F-05/F-06` 的 editor/bootstrap/validation 里仍有旧 anchor/old beat/old root 残依赖。
  5. `Meitner`
     - 明确 `F-07` 仍有 4 簇 stale tests 要删/重写，尤其旧中间层真值、旧 return-home->resume roam 合同、旧 fallback 测试。
- fresh 现场信号：
  1. `sunset_mcp.py errors`：`0 error / 0 warning`
  2. `sunset_mcp.py status`：bridge/baseline 正常，但 Unity 当前处于 `is_playing=true`、`playmode_transition/stale_status`，不适合作为打包验收结论
  3. 当前仓库仍是大面积 mixed dirty，且还有别的线程 `ACTIVE`
- 当前判断：
  1. 现在不能说 `0417` 已完成。
  2. 现在也不能说“可以打包”。
  3. 原因不是又红了，而是：
     - `B/C/F/G` 仍明显未完
     - 5 个子智能体带回的是“还有哪些没清”，不是“已经清完”
     - 当前 live/editor 也不在一个可作为打包终验的稳定窗口
- 当前恢复点：
  1. 继续施工时，仍按 `0417` 从未完成包往下砍，优先 `B / C / F / G`。
  2. 如果要进入“可打包”判断，至少要先把 `G-01 ~ G-05` 跑起来，再配 fresh live / packaged 证据。

## 2026-04-17｜打包前最小待完成清单重算：范围已收窄到夜间尾刀 + 7:00 放人
- 本轮性质：
  - 只读分析；根据用户最新 packaged 反馈，重算“最小待完成打包清单”。
- 用户最新裁定：
  1. opening / primary / dinner 大体已稳定，不再是当前最小打包 blocker。
  2. 当前最需要注意的只剩：
     - `101` 在 `20:00` 卡在原地，不往家里走
     - 次日放人时间从 `9:00` 改到 `7:00`
     - 其他夜间语义保持：
       - `20:00` 开始走导航回 `HomeAnchor`
       - 到 anchor 附近就可直接隐藏，视作进屋
       - `21:00` 仍未到家的，强制 snap 到 anchor 再隐藏
       - 次日从 anchor 激活
- 重新核代码后的关键事实：
  1. 夜间骨架已经存在，不需要从零重做：
     - `20:00` 入口：`SpringDay1NpcCrowdDirector.SyncResidentNightRestSchedule()`
     - 回家导航：`TryDriveResidentReturnHome() -> RequestReturnToAnchor(...)`
     - `21:00 snap + hide`：`ApplyResidentNightRestState(...)`
     - 次日从 anchor 恢复：`RestoreResidentFromNightRestState(...)`
  2. 当前真实还没收死的，是 live 收口，不是结构缺失。
  3. 早上放人时间当前确实硬编码为 `9`：
     - `SpringDay1NpcCrowdDirector.ResidentMorningReleaseHour = 9`
     - `SpringDay1Director.StoryActorMorningReleaseHour = 9`
- 当前最小待完成打包清单：
  1. 单点修 `101` 的 `20:00` 回家异常，不扩成全 NPC 夜间重构。
  2. 把 `ResidentMorningReleaseHour / StoryActorMorningReleaseHour` 从 `9 -> 7`。
  3. 校准 `20:00` 的到家即隐藏语义，让“到 anchor 附近就消失”真正成立。
  4. 保持 `21:00` 的强制 snap 到 anchor 再隐藏，不回滚现有骨架。
  5. 保持次日从 anchor 激活，但把触发时刻统一到 `7:00`。
  6. 跑一轮最小打包回归：
     - `20:00`
     - `21:00`
     - 次日 `7:00`
     - packaged 路径复验
- 当前判断：
  1. 这条线已经不该再背“全 Day1 最终大清扫”的包袱。
  2. 真正的最小打包尾单已经收窄成夜间 E 包尾刀。
  3. 下一轮如果继续真实施工，只该从这 6 条里做最小闭环，不再回头重翻 opening/primary/dinner。

## 2026-04-17｜Package D/E 继续：晚饭 phase-entry 起点、night home-anchor 真值、bridge native snapshot 清理已补
- 本轮性质：
  - 真实施工；继续按 `0417.md` 收 `dinner -> night -> next-day` 这一条链。
- 本轮新增落地：
  1. `SpringDay1Director.ActivateDinnerGatheringOnTownScene()`
     - 新增保护测试，要求 `001/002` 在 `18:00` 晚饭入口第一拍就被拉到 authored 起点。
  2. `SpringDay1NpcCrowdDirector.FindSceneResidentHomeAnchor(...)`
     - 修掉 `001/002/003` 在 unified night runtime 下把 NPC 本体误绑成 `HomeAnchor` 的真 bug。
  3. `PersistentPlayerSceneBridge.ClearNativeResidentRuntimeSnapshots(...)`
     - 新增原生 resident snapshot 清理口；
     - `SpringDay1Director.HandleSleep()` 现在会清掉 `001/002/003` 的 bridge native snapshots，避免次日/跨场景把昨晚位置带回来。
- 新增/补强测试：
  - `ActivateDinnerGatheringOnTownScene_ShouldPlaceStoryActorsAtAuthoredStartsOnPhaseEntry`
  - `CrowdDirector_ShouldBindUnifiedNightRuntimeStoryActorsToOwnHomeAnchors`
  - `PersistentPlayerSceneBridge_ClearNativeResidentRuntimeSnapshots_ShouldDropDay1SyntheticActors`
- 当前回归结果：
  - 关键窄回归 `7/7 passed`
  - fresh console `0 error / 0 warning`
  - `git diff --check` 当前 own 文件 clean
- 当前判断：
  1. 用户最新关心的三条里，至少又实锤并修掉了两个高价值根因：
     - `20:00/21:00` 的 home anchor 真值错误
     - 次日/跨场景恢复链里的 native snapshot 回流
  2. `E-08` 仍未完成；现在剩的是 `TickResidentReturnHome / retry / finish` 的持续代管还没彻底削薄。
- 当前恢复点：
1. 如果继续真实施工，优先评估是否还能安全再收 `E-08` 一刀；
2. 如果先等用户 fresh live，则重点复测 `18:00 / 20:00 / 21:00 / 次日`。

## 2026-04-17｜夜间打包尾刀收口：20:00 到家即隐藏、7:00 放人、101 不特判
- 本轮主线：
  - 继续收用户最新打包反馈里的夜间尾单，不再重开 opening / primary / dinner 主修。
- 用户最新补充：
  1. `101` 只是本轮 packaged 测到的个例，不允许我只查或特判 `101`。
  2. 用户如果调整场景里的 `HomeAnchor`，代码应直接吃场景对象位置，不应硬编码坐标。
  3. 其他主流程目前问题不大，但仍要求我全面检查自己 own 的夜间链。
- 本轮已核实 / 已完成：
  1. `Town.unity` 中存在 `101_HomeAnchor`，也存在 `001/002/003/102/103/104/201/202/203_HomeAnchor`。
  2. `SpringDay1NpcCrowdDirector.FindSceneResidentHomeAnchor(...)` 会按 `npcId` / `NPCxxx` 名字派生并解析 `xxx_HomeAnchor`。
  3. 夜间 `TryBeginResidentReturnHome(...)`、`FinishResidentReturnHome(...)` 使用 `state.HomeAnchor.transform.position`，所以移动场景里的 anchor 对象不需要改代码坐标。
  4. `ResidentMorningReleaseHour` 与 `StoryActorMorningReleaseHour` 当前均为 `7`。
  5. `20:00` 已补“到 `HomeAnchor` 附近即隐藏”，`20:00` 已在家附近的 resident 也会直接隐藏。
  6. `21:00` 的强制 `snap + hide` 保留，次日从 `HomeAnchor` 激活保留。
  7. `0417.md` 已新增 `C-23` 并更新 `G-09 / I-09 / I-10 / Package E`，明确 `101` 不写特判，后续只查 runtime 绑定与导航启动。
- 验证结果：
  1. 夜间尾单定向 EditMode tests：`8/8 passed`。
  2. fresh console：清理旧噪音后 `0 error / 0 warning`。
  3. `git diff --check` 覆盖本轮 own 文件通过。
  4. `validate_script` 曾因 Unity Play/transition 状态出现 `blocked / external_red`，清理 console 并停止 Play 后 fresh console clean；当前不把这类旧噪声算作 own red。
- 遗留问题 / 下一步：
  1. 用户可先调整 `101_HomeAnchor` 等场景对象位置；对象名保持不变即可。
  2. 下一轮 packaged / live 只需重点复测：
     - `20:00` 全员是否开始回家或到家即隐藏；
     - `101` 调整 anchor 后是否正常；
     - `21:00` 未到家者是否 snap 到 anchor 后隐藏；
     - 次日 `7:00` 是否从 anchor 激活。
  3. 若 `101` 调整 anchor 后仍异常，下一刀查 runtime probe：实际绑定的 `HomeAnchor`、与 anchor 距离、`RequestReturnToAnchor` 返回、导航是否被 active state / scene entry 打断。
- thread-state：
  - 本轮重新 `Begin-Slice`：`0417-packaging-final-night-anchor-and-regression-close`。
  - 收尾后执行 `Park-Slice`。

## 2026-04-17｜停车补记：本轮已合法 Park
- `Park-Slice`：
  - 已执行
  - 状态：`PARKED`
  - blocker：`await-user-anchor-adjust-and-packaged-night-retest`
- 当前恢复点：
  1. 先等用户调整 `101_HomeAnchor` 等场景 anchor。
  2. 然后做 packaged/live 复测：
     - `20:00`
     - `21:00`
     - 次日 `7:00`
     - `101`
  3. 如果 `101` 仍异常，再继续抓 runtime probe，不回退成 `101` 特判。

## 2026-04-17｜Package B/C 最小补刀：`002` 等待补位与艾拉圆形回血已落地
- 当前主线目标：
  - 继续把 `0417.md` 作为唯一主控板，只收用户最新点名的最小尾刀，不扩回全 Day1 大修。
- 用户本轮新增语义：
  1. `TownHouseLead` 中 `001` 停下等玩家时，`002` 不应一起停死，应继续和 `001` 保持编队距离并补位。
  2. 艾拉附近回血应按艾拉周围圆范围触发，不要求玩家面向艾拉。
  3. 触发治疗后，允许艾拉最小朝向玩家。
- 本轮实际落地：
  1. `SpringDay1Director.TryBeginTownHouseLead()` 等待分支改为：
     - `001` 停等玩家；
     - `002` 继续通过 `TryDriveEscortCompanionTowardLeaderFormation(...)` 追动态编队位。
  2. `SpringDay1Director.TryHandleHealingBridge()` 改为：
     - 使用玩家本体位置参与圆形距离判断；
     - 半径仍为 `1.6`；
     - 触发后调用 `FaceHealingSupportTowardPlayer(...)`。
  3. `0417.md` 已同步新增 `C-24 / C-25`，并更新 `G-11/G-12`、`I-11/I-12`、`T-04/T-13` 与 `Package B/C` tasks。
- 验证结果：
  1. 定向 EditMode tests：`4/4 passed`
     - `TownHouseLeadWait_ShouldKeepCompanionClosingFormationAroundPausedChief`
     - `HealingBridge_ShouldUsePlayerCenterAsProximityPoint`
     - `HealingBridge_ShouldTurnCompanionToFacePlayerOnceHealingTriggers`
     - `EscortTransition_ShouldRequireChiefAndCompanionReady`
  2. fresh console：`0 error / 0 warning`
  3. `git diff --check` 覆盖本轮 own 文件通过
- 当前恢复点：
  1. 下一轮 live / packaged 优先验 `T-04` 与 `T-13`。
  2. 这两刀不需要继续扩改 NPC `E` 交互、workbench escort、night contract 或其他剧情段。

## 2026-04-17｜停车补记：最小尾刀已合法 Park
- `thread-state`：
  - `Day1-V3 = PARKED`
  - blocker = `await-live-retest-healing-circle-and-town-house-lead-companion-follow`
- 恢复点：
  1. live / packaged 先验：
     - `T-04`
     - `T-13`
  2. 只要这两条通过，这轮不再继续扩刀。

## 2026-04-17｜只读总览补记：0417 当前阶段与剩余内容已重新归纳
- 本轮性质：
  - 只读分析汇总；未进入新施工，未跑 `Begin-Slice`。
- 当前总判断：
  1. `0417.md` 已经把 Day1 从“散修”收成 Package A~G 的主控板。
  2. 打包前最小尾刀层面，当前真正等待的是 live / packaged 复测，而不是继续盲目扩修。
  3. 完美架构层面仍有债务，主要是 opening/dinner movement owner 未完全单一、`003` 残留特殊入口、Package F 的 data/editor/tests/scene 清扫未完成、Package G 终验未开始。
- 当前最小恢复点：
  1. 优先验 `T-04` 艾拉圆范围回血与朝向。
  2. 优先验 `T-13` `001` 等玩家时 `002` 继续补位。
  3. 再按用户时间验夜间 `20:00 / 21:00 / 次日 7:00 / 101`。
  4. 若这些过，当前可进入打包验收包；若失败，只回对应局部，不再回散修。

## 2026-04-17｜Package B 补刀：`003` opening 导演私链已退出
- 本轮性质：
  - 真实施工；沿 `0417.md` 继续收 opening / `003` 身份残留，不重开全 Day1 大修。
- 本轮目标：
  1. 把 `003` 从 `SpringDay1Director` 的 opening story actor 私链里退出。
  2. 让 `003` 在 `EnterVillage` 阶段就并入 `SpringDay1NpcCrowdDirector` 的 ordinary resident runtime。
  3. 修掉 opening 桥接测试里仍按旧反射签名和旧 `003` 私链理解产生的假失败。
- 已完成事项：
  1. `SpringDay1NpcCrowdDirector.ShouldIncludeThirdResidentInResidentRuntime(...)` 已改为 `currentPhase >= EnterVillage`。
  2. `SpringDay1Director` opening 入口不再 resolve / reframe / drive `003`。
  3. opening 对话 active 时改为通过 `TryPrepareTownVillageGateActors(forceImmediate: true)` 直接钉 `001/002` 终点，避免 active dialogue 窗口仍留在旧位置。
  4. `SpringDay1OpeningRuntimeBridgeTests.InvokeInstance(...)` 已改成按参数与 optional 参数匹配目标方法，避免同名/optional 签名导致 `TargetParameterCountException`。
  5. `0417.md` 已同步更新 `C-01 / C-03 / I-03 / Package B tasks / owner matrix / lifecycle table`。
- 验证结果：
  1. direct `validate_script`：
     - `SpringDay1Director.cs`：`errors=0`
     - `SpringDay1OpeningRuntimeBridgeTests.cs`：`errors=0`
  2. opening 定向 EditMode tests：`6/6 passed`
     - `CrowdDirector_ShouldNotDeferThirdResidentToStoryEscortDirectorDuringEnterVillage`
     - `CrowdDirector_RuntimeEntries_ShouldIncludeThirdResidentAfterOpening`
     - `CrowdDirector_ShouldBindThirdResidentIntoResidentRuntimeDuringOpening`
     - `TownVillageGatePreparation_ShouldAlignStoryActorsBeforeDialogueStarts`
     - `TownVillageGatePreparation_ShouldIgnoreMissingThirdResidentMarkerBecause003RunsThroughCrowdRuntime`
     - `TownVillageGateDialogueActive_ShouldKeepStoryActorsAligned`
  3. fresh console：清理测试残留后 `0 error / 0 warning`
  4. `git diff --check` 覆盖 own 文件通过
  5. CLI `validate_script` 对本轮两文件返回 `assessment=unity_validation_pending owned_errors=0 external_errors=0`，原因是 `stale_status / CodeGuard timeout-downgraded`，不是 owned red。
- 当前判断：
  1. `003` 的 runtime 主修已落地；它现在结构上已经是普通 resident，不再走 opening/dinner 导演私链。
  2. opening 仍未完全成为单一 movement owner，因为 `001/002` 仍在 director 链，ordinary resident 仍在 crowd 链。
  3. packaged/live 仍待用户复测，不能把 targeted tests 写成体验过线。
- 当前恢复点：
  1. 如果用户继续要求打包验收，优先按 `0417 / Package G` 做最小验收包。
  2. 如果用户继续清架构债，下一刀仍应围绕 opening 单一 owner 或 Package F 旧真值清扫，不回到 `003` 专用 runtime。

## 2026-04-17｜Package E 补刀：`20:00 到家即隐藏` 的到站半径已收平
- 本轮性质：
  - 真实施工；只收夜间合同里“到家就进屋”的漏口，不扩回其它 Day1 包。
- 本轮目标：
  1. 把 `20:00` 到家即隐藏从过严的贴点判定改成更符合导航合同的到站半径。
  2. 收掉两种仍会让 resident 站在 anchor 门口的坏态：
     - `retry cooldown` 期间其实已经到家
     - 没有 active `FormalNavigation` 但其实已经贴近 anchor
- 已完成事项：
  1. `SpringDay1NpcCrowdDirector` 新增统一的 home arrival 半径 `0.35`。
  2. `TryBeginResidentReturnHome(...)` 里“已经在家附近 -> 直接隐藏”现在优先于 retry cooldown。
  3. `TickResidentReturnHome(...)` 在无 active navigation completion 时，也会按 arrival 半径直接 `FinishResidentReturnHome(...)`。
  4. `ShouldQueueResidentReturnHomeAfterCueRelease(...)` 与 `TryDriveResidentReturnHome(...)` 也已切到同一 arrival 半径口径。
  5. `0417.md` 已同步新增 `C-26`，并更新 `G-09`、`I-09`、`I-10` 与 `Package E` tasks。
- 验证结果：
  1. direct `validate_script`：
     - `SpringDay1NpcCrowdDirector.cs`：`errors=0 warnings=2`
     - `SpringDay1DirectorStagingTests.cs`：`errors=0 warnings=0`
  2. fresh console：`0 error / 0 warning`
  3. `git diff --check` 覆盖本轮 own 文件通过
  4. CLI `validate_script` 仍是 `unity_validation_pending owned_errors=0 external_errors=0`，原因是 Unity 编译状态未 ready，不是 owned red
- 当前判断：
  1. 这刀已经把“到家即隐藏”从过严点判定收成导航到站半径。
  2. 当前仍只站住 `结构 / targeted probe`，packaged/live 还需要用户继续复测。
  3. 如果后续仍看到有人站在 anchor，下一步优先抓 runtime probe 看是否压根没进入夜间合同或没拿到 `HomeAnchor`，不回到旧中间层真值。
- 当前恢复点：
  1. 优先复测 `20:00`：
     - 到家即消失
     - 不再站在 anchor 门口
  2. 再看 `21:00` 强制 snap+hide 与次日 `7:00` 恢复。

## 2026-04-17｜停车补记：`20:00 到家即隐藏` 热修已 Park
- `thread-state`：
  - `Day1-V3 = PARKED`
  - reason = `night-return-arrival-radius-hotfix-waiting-packaged-retest`
- 当前恢复点：
  1. 优先 packaged/live 复测 `20:00`：到达 `HomeAnchor` 附近应立即隐藏，不再站在门口。
  2. 若仍有 NPC 站在 anchor，下一刀只抓 runtime probe / home anchor 绑定 / 是否进入夜间合同，不扩回旧中间层。

## 2026-04-17｜Package D/E 补刀：formal NPC 剧情外也会自动拿到 resident informal 入口
- 本轮性质：
  - 真实施工；只收 `19:30 / 20:00` 剧情外“应该恢复普通 NPC 聊天入口”的 own 漏口。
- 本轮目标：
  1. 对位“只有少数 NPC 能聊”的代码根因。
  2. 修掉 formal NPC 在剧情外理论上应让位给居民闲聊、但实际上没有 informal 入口组件的坏态。
- 已完成事项：
  1. 继续核 `SpringDay1Director` 后确认：
     - `FreeTime` 已经会重新启用 `001/002` 的 formal / informal 组件；
     - 问题更像“入口不存在”，不是单纯 `enabled=false`。
  2. `SpringDay1NpcCrowdDirector.ConfigureBoundNpc(...)` 已改成：
     - 只要 NPC 还没有 `NPCInformalChatInteractable`
     - 且 `RoamProfile` 确实有 resident informal content
     - 就允许自动补 informal 入口
     - 不再因为已经有 `NPCDialogueInteractable` 就跳过
  3. 新增定向测试：
     - `CrowdDirector_ConfigureBoundNpc_ShouldAddInformalChatWhenFormalDialogueExists`
  4. `0417.md` 已同步新增 `C-27`，并更新 `G-10`、`I-08` 与 `Package D` task。
- 验证结果：
  1. `git diff --check` 覆盖本轮 own 代码文件通过
  2. 当前 Unity MCP 基线掉线：
     - `doctor` 先报 `listener_missing / pidfile_missing`
     - 已执行 `recover-bridge`
     - 但随后 `validate_script` 仍拿到 `unity_validation_pending / baseline_fail / No active Unity instance`
  3. 因此这刀当前只能报：
     - `结构已改`
     - `静态判断成立`
     - `Unity 验证待补`
- 当前判断：
  1. 这刀很值钱，因为它直接补的是 formal NPC 在剧情外缺居民闲聊入口的 own 漏口。
  2. 但当前不能把它包装成“live 已证”，因为 Unity 实例当下不在线。
- 当前恢复点：
  1. 下一轮若 Unity 可用，优先补 `NPC`/`late-day` 窄验证。
  2. packaged/live 重点复测：
     - `19:30` 后 ordinary resident 与 `001/002` 是否都恢复可聊
     - `20:00` 回家途中是否仍可聊天

## 2026-04-17｜停车补记：formal->informal 入口热修已 Park
- `thread-state`：
  - `Day1-V3 = PARKED`
  - reason = `formal-to-informal-entry-hotfix-waiting-unity-validation-or-live-retest`
- 当前恢复点：
  1. Unity 一恢复，先补这条 hotfix 的窄验证。
  2. 如果用户先做 packaged/live，优先看：
     - `19:30` 后 ordinary resident 与 `001/002` 是否恢复可聊
     - `20:00` 回家途中是否仍可聊天

## 2026-04-17｜补记：`SpringDay1DirectorStagingTests` 新增测试 compile blocker 已清
- 本轮性质：
  - 支撑子任务；不扩 Day1 runtime，只清 `SpringDay1DirectorStagingTests.cs` 自己制造的编译红。
- 本轮目标：
  1. 修掉 `ResolveNestedTypeOrFail(...)` 不存在导致的两处 `CS0103`。
  2. 把 `C-27` 那条新增 guard test 拉回 compile-clean。
- 已完成事项：
  1. `CrowdDirector_ConfigureBoundNpc_ShouldAddInformalChatWhenFormalDialogueExists` 里两处 nested type 解析已改回：
     - `ResolveTypeOrFail("NPCDialogueContentProfile+InformalChatExchange")`
     - `ResolveTypeOrFail("NPCDialogueContentProfile+InformalConversationBundle")`
  2. 磁盘内容已对位，不再引用不存在的 helper。
  3. `0417.md` 已同步追加 compile blocker 清理事实。
- 验证结果：
  1. `git diff --check -- Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs` 通过。
  2. fresh `status` 已出现：`Build completed with a result of 'Succeeded'`。
  3. fresh `errors` 当前只剩外部 blocker：
     - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset` importer inconsistent result
  4. `validate_script` 对该测试文件给出：`owned_errors=0`，不再是这条测试自己的 compile red。
- 当前判断：
  1. 这条 compile blocker 已解决。
  2. 当前不能 claim 全项目 no-red，因为 Unity console 仍有外部 `TextMesh Pro` importer 错。
- 当前恢复点：
  1. Day1 主线可继续按 `0417` 往下做，不必再被这条测试 helper 红阻塞。
  2. 后续若要 claim no-red，需要先处理或绕开外部 importer blocker。

## 2026-04-17｜Package E 补刀：夜间 `20:00/21:00` 合同继续硬化
- 本轮性质：
  - 真实施工；只收夜间 resident `return-home / hide` own 漏口，不扩到 `0.0.6`、dinner 或 opening。
- 本轮目标：
  1. 修掉 `20:00` 回家途中一旦丢了 active drive 就站到 `21:00` 的坏态。
  2. 修掉 forced day-end / `21:00` snap 链只 snap 不 hide 的漏口。
  3. 补 `21:00` 对无 `HomeAnchor` 的兜底隐藏，避免漏网。
- 已完成事项：
  1. `TickResidentReturnHome(...)` 现在在无 active `FormalNavigation` drive 且未到 anchor 时，不再只是空等：
     - 会按短节流窗口继续 retry `TryDriveResidentReturnHome(...)`
     - 失败后释放 shared owner，并留下 `return-home-pending` 重试窗
  2. `SyncResidentNightRestSchedule()` 现在在 `21:00` rest 窗口不再因为 `HomeAnchor == null` 直接跳过；无 anchor 的 resident 也会被标记进 `night-rest-hidden` 并隐藏。
  3. `SnapResidentsToHomeAnchorsInternal()` 现在会先写 `HideWhileNightResting = true`，forced day-end snap 不再只回家不隐身。
  4. 新增 / 改写定向测试：
     - `CrowdDirector_ShouldScheduleReturnHomeRetryWhenNoDriveIsActive`
     - `CrowdDirector_ShouldHideResidentsAtTwentyOneEvenWhenHomeAnchorIsMissing`
     - `CrowdDirector_SnapResidentsToHomeAnchors_ShouldHideResidentsForNightRest`
- 验证结果：
  1. `validate_script`：
     - `SpringDay1NpcCrowdDirector.cs`：`owned_errors=0`
     - `SpringDay1DirectorStagingTests.cs`：`owned_errors=0`
     - 当前 assessment 仍为 `unity_validation_pending`，原因是 Unity `stale_status`，不是 owned red
  2. fresh `errors`：`0 error / 0 warning`
  3. fresh `status`：baseline pass，console clean
  4. `git diff --check` 覆盖 own 文件通过
- 当前判断：
  1. 这刀已经把夜间合同进一步压向你的真实语义：`20:00` 继续送回家，`21:00` 只做最后硬兜底。
  2. 当前仍只站住 `结构 / targeted probe`；是否彻底消灭 packaged 里的 20:10 留场个例，还要你继续打包复测。
- 当前恢复点：
  1. 优先 packaged 复测：
     - `20:00` 开始回家
     - 到 anchor 即消失
     - `21:00` 只剩极少数兜底 snap+hide
  2. 若仍有个例留场，下一刀优先抓：
     - `HomeAnchor` 真值
     - `FormalNavigation arrival`
     - scene/snapshot/recover 先恢复 active 再补夜间合同的顺序

## 2026-04-17｜Package C 补记：疗伤桥 packaged 左侧几乎贴脸才触发，已收成“玩家中心 -> 艾拉真实中心”
- 本轮性质：
  - 真实施工；只修用户 packaged 反馈里的疗伤桥触发圆心偏移，不扩其他 Day1 语义。
- 本轮目标：
  1. 对位“从 002 左侧接近时，几乎碰到才会触发回血”的根因。
  2. 保持半径 `1.6` 不变，只把疗伤判定圆心从 `002.transform.position` 改成艾拉真实碰撞/展示中心。
- 已完成事项：
  1. `TryHandleHealingBridge()` 现在用 `GetHealingSupportSamplePoint(...)` 取艾拉 sample，不再直接拿 `supportNpc.position`。
  2. `TrySnapValidationPlayerNearHealingSupport(...)` 已切到同一 sample 点口径，避免验证逻辑和 runtime 口径分叉。
  3. `SpringDay1Director` 新增 `TryGetCenterSamplePoint(...)`，优先取 collider bounds.center，其次 presentation bounds.center，最后才回退到 transform。
  4. `0417.md` 已同步回写：
     - `C-25`
     - `I-11`
     - `2026-04-17 Package B/C 补刀` 段
- 验证结果：
  1. `validate_script`：
     - `SpringDay1Director.cs`：`owned_errors=0`
     - `SpringDay1DirectorStagingTests.cs`：`owned_errors=0`
     - 两者当前 assessment 均为 `unity_validation_pending`，block 在 `stale_status`，不是 owned red
  2. fresh `errors`：`0 error / 0 warning`
  3. fresh `status`：console 为空，baseline pass
  4. `git diff --check` 覆盖本轮 own 文件通过
  5. 新增定向测试：
     - `HealingBridge_ShouldUseSupportCenterAsProximityAnchor`
- 当前判断：
  1. 这刀修的是 packaged 反馈对应的真实根因，不是继续盲调半径数字。
  2. 当前只站住 `结构 / targeted probe`；是否彻底解决左侧接近坏相，仍需要你重新打包复测。
- 当前恢复点：
  1. 重点只复测疗伤桥：
     - 玩家从 002 左侧接近
     - 不需要几乎贴脸
     - 进入 `1.6` 圆范围就能触发
  2. 这条若通过，再继续看其他 Day1 尾项。

## 2026-04-17｜只读彻查：Day1 `0.0.6` 在 `DinnerConflict / 回 Town` 前后对 `001/002` 的当前处理
- 本轮性质：
  - 只读分析；不改代码，只回答 `0.0.6` 开始时 `001/002` 现在到底由谁驱动，以及如果要改成“先传到各自 Town anchor，再在 Town 侧进入下一段移动”，现有代码最先撞上的入口在哪里。
- 本轮目标：
  1. 钉实现在是否仍在 `Primary` 里 escort 玩家回 `Town`。
  2. 钉实 `Town` 侧 `001/002` 当前是吃哪条链：scene transition、director 手动摆位、还是 crowd/runtime baseline。
  3. 给出最冲突的文件 / 方法 / 行附近锚点。
- 已完成事项：
  1. 已钉实 `0.0.6` 现在仍保留 `Primary -> Town` escort：
     - `SpringDay1Director.IsReturnToTownEscortPending()` 要求 `CurrentPhase == DinnerConflict && IsPrimarySceneActive() && !HasCompletedDialogueSequence(DinnerSequenceId)`。
     - `SpringDay1Director.BeginDinnerConflict()` 在 `Primary` 内不会直接进晚饭对白，而是先 `TryHandleReturnToTownEscort()`。
     - `TryHandleReturnToTownEscort()` 会持续驱动 `001/002` 朝 `Primary -> Town` 的 `SceneTransitionTrigger2D` 前进，并在玩家 / 双 NPC 都到位后触发切场。
  2. 已钉实现有 scene transition 只给玩家排 `Town` 入场 anchor，不会顺手给 `001/002` 做各自 Town anchor handoff：
     - `SceneTransitionTrigger2D.TryStartTransition()` 只调用 `PersistentPlayerSceneBridge.QueueSceneEntry(...)`。
     - `ResolveTargetEntryAnchorName()` 在 `Primary -> Town` 时只回 `TownPlayerEntryAnchor`。
  3. 已钉实切回 `Town` 后，导演入口只对 player 做对位，`001/002` 的 Town 侧不是“先到各自 anchor 再走一段”：
     - `ActivateDinnerGatheringOnTownScene()` -> `AlignTownDinnerGatheringActorsAndPlayer()` 只移动玩家。
     - 随后 `BeginDinnerConflict()` -> `PrepareDinnerStoryActorsForDialogue()` 才直接把 `001/002` reframe 到晚饭 story markers。
     - `TryResolveDinnerStoryActorRouteFromSceneMarkers()` 只吃 `Town.unity` 里的 `001起点/终点`、`002起点/终点`。
  4. 已钉实 `Town` 侧当前没有为 `001/002` 准备“先传 Town anchor 再走下一段”的统一数据合同：
     - `SpringDay1TownAnchorContract.json` 只有 `EnterVillageCrowdRoot / NightWitness_01 / DinnerBackgroundRoot / DailyStand_*`，没有 `001/002` 专属 Town anchor。
     - `SpringDay1DirectorStageBook.json` 的 `DinnerConflict_Table` 没有 `001/002` actor cue；`001/002` 晚饭站位是导演代码 special-case，不是 stage book 驱动。
  5. 已钉实 crowd/runtime 在 `DinnerConflict` 起就把 `001/002` 纳回 unified night resident runtime，但 baseline defer 规则又和 director special-case 形成交叉：
     - `SpringDay1NpcCrowdDirector.ShouldIncludeStoryEscortInUnifiedNightRuntime()` 从 `DinnerConflict` 起 synthetic 加入 `001/002`。
     - `ApplyResidentBaseline()` 在 `DinnerConflict` 不再走 `ShouldDeferToStoryEscortDirector(...)` 的旧白天 defer。
- 关键判断：
  1. 现在的 `0.0.6` 不是“到点切 Town 后 `001/002` 已经各自在 Town 锚点上，再开始下一段移动”，而是：
     - `Primary` 里先 escort 玩家回村；
     - 切场时只保证 player 落到 `TownPlayerEntryAnchor`；
     - `Town` 里再由导演把 `001/002` 直接摆到晚饭 story markers。
  2. 如果要改成“`0.0.6` 一开始先把 `001/002` 传到各自 Town anchor，再在 Town 侧进入下一段移动”，最先会撞上的不是单点 if，而是 4 层合同一起冲突：
     - `Primary` 返场 escort 仍存在；
     - scene transition 只运 player；
     - `Town` 晚饭 special-case 直接重摆 `001/002`；
     - crowd/runtime 从 `DinnerConflict` 起已把 `001/002` 当 resident runtime 处理。
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1NpcCrowdDirector.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SceneTransitionTrigger2D.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Directing\SpringDay1DirectorStaging.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Resources\Story\SpringDay1\Directing\SpringDay1DirectorStageBook.json`
  - `D:\Unity\Unity_learning\Sunset\Assets\Resources\Story\SpringDay1\Directing\SpringDay1TownAnchorContract.json`
  - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Town.unity`
  - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
- 验证结果：
  - 纯静态推断成立；未改业务代码，未跑 live。
- 当前恢复点：
  1. 用户若继续追“Town anchor first”，下一刀应先决定：
     - 这是要替换掉 `Primary` return escort；
     - 还是保留 `Primary` escort，但把 Town load 后的 `001/002` handoff 改成 anchor-first。
  2. 在没统一这层 owner 之前，不宜只补 marker 或只补 anchor 数据，否则很容易被现有 `BeginDinnerConflict()` / crowd baseline 重新覆盖。

## 2026-04-17｜Package C 补记：`0.0.6 Town anchor-first` 已落代码、已接进菜单、new case 已过
- 本轮性质：
  - 真实施工；只收 `0.0.6` 回 Town 时 `001/002` 的 one-shot anchor-first handoff 与验证接线，不扩到 scene transition 协议。
- 本轮目标：
  1. 让 `001/002` 在 `FarmingTutorial + postTutorialExploreWindow` 的 Town load 首帧先落到各自 `HomeAnchor`。
  2. 把这条新 guard test 真正接进现有 `Run Director Staging Tests` 套件，避免继续掉出验证面。
- 已完成事项：
  1. `SpringDay1Director` 已有的 `_postTutorialTownActorsAnchored / HandleSceneLoaded / TryPrimeTownStoryActorsForExploreWindow` anchor-first 链确认保留为当前真值。
  2. `SpringDay1TargetedEditModeTestMenu.cs` 已把：
     - `SpringDay1DirectorStagingTests.Director_ShouldSnapStoryActorsToHomeAnchorsDuringPostTutorialExploreWindowInTown`
     接进 `DirectorStagingTargetTestNames`。
  3. 已通过 `Library/CodexEditorCommands/requests/*.cmd` 真实触发 `Run Director Staging Tests`。
  4. 第二次重跑后，这条新 case 已真实出现在结果包里，结果=`Passed`。
- 验证结果：
  1. `Run Director Staging Tests`：
     - 新 case 已纳入
     - 新 case=`Passed`
     - 整套当前=`28 pass / 10 fail`
  2. `validate_script Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`：
     - `assessment=no_red`
     - `owned_errors=0`
  3. `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`：
     - `owned_errors=0`
     - `assessment=unity_validation_pending`
     - blocker=`stale_status`
  4. `validate_script Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`：
     - `owned_errors=0`
     - `assessment=unity_validation_pending`
     - blocker=`stale_status`
  5. `git diff --check` 覆盖：
     - `SpringDay1Director.cs`
     - `SpringDay1DirectorStagingTests.cs`
     - `SpringDay1TargetedEditModeTestMenu.cs`
     通过
- 当前判断：
  1. 这刀现在至少不再停留在“静态推断”：新 case 已被菜单 runner 真跑到。
  2. 但这仍然只是 `结构 / targeted probe` 成立，因为整套 `Director Staging` 仍有 `10` 条旧失败，packaged/live 也还没复测。
- 当前恢复点：
  1. 下一步优先做 packaged/live，只看：
     - `0.0.6` 回 Town 后 `001/002` 是否先出现在各自 `HomeAnchor`
     - 是否还会被同帧旧链 reframe 走
  2. 若仍失败，下一刀只抓：
     - Town load 同帧回流
     - `HandleSceneLoaded -> UpdateSceneStoryNpcVisibility`
     - 是否有别的链在这一拍又重摆 `001/002`

## 2026-04-17｜Package E / G 补记：`20:00` pending return-home 热修已落，主剧情 targeted tests 新一轮已收口
- 本轮主线：
  - 继续按 `0417` 收 Day1 打包前最值钱的夜间合同缺口，并补一轮覆盖 opening / midday / dinner / unified night / owner regression / late-day 的 targeted 验证。
- 本轮真实施工：
  1. `SpringDay1NpcCrowdDirector.ApplyResidentBaseline(...) / TryBeginResidentReturnHome(...)` 已补“`20:00` 首次回家 drive 失败后，先进入 `return-home-pending` 持有态，不再掉回空态”。
  2. 这刀会：
     - 先拿住 resident owner
     - 停住当前位置
     - 保留 `IsReturningHome`
     - 后续按短 retry 窗继续重试
     - 若其实已经到家并被 hide，也不会再回流白天 release 分支
  3. `SpringDay1MiddayRuntimeBridgeTests.DinnerAndReminderCompletion_ShouldBridgeIntoFreeTime` 已回正到用户最新语义：
     - `19:30` 一进入 free time，床就应立刻可用
- 本轮验证结果：
  1. `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - `assessment=no_red`
     - `owned_errors=0`
  2. `validate_script Assets/YYY_Tests/Editor/SpringDay1MiddayRuntimeBridgeTests.cs`
     - `assessment=no_red`
     - `owned_errors=0`
  3. `Run Opening Bridge Tests`：`13/13 passed`
  4. `Run Midday Bridge Tests`：`8/8 passed`
  5. `Run Dinner Contract Tests`：`7/7 passed`
  6. `Run Unified Night Contract Tests`：`18/18 passed`
  7. `Run Day1 Owner Regression Tests`：`2/2 passed`
  8. `Run Late-Day Bridge Tests`：`11/13 passed`
     - remaining:
       - `BedBridge_EndsDayAndRestoresSystems`
       - `DayEndPlayerFacingCopy_ShouldCarryTomorrowBurdenAndClearWorkbenchState`
  9. fresh `errors`：`0 error / 0 warning`
  10. `git diff --check` 覆盖当前 touched Day1 own 文件通过
- 当前判断：
  1. opening / midday / dinner / unified night / owner regression 这 5 组 targeted 面当前都已绿。
  2. 当前自动化残留只剩 late-day bridge 的 2 条老尾项，所以不能把这轮包装成“Day1 全线已绿”。
  3. 就打包前风险判断来说，当前最值钱的新收口是：
     - `20:00` 首次起手失败不再掉回空态
     - `Midday -> FreeTime` 测试口径已和用户最新语义重新对齐
- 当前恢复点：
  1. 若继续自动化补刀，下一步只该对位 late-day 剩余 2 fail 的真因，不回头扩 opening/dinner/night。
  2. 若优先交用户判断，必须明确：
     - 主剧情 targeted 已大面积绿
     - late-day 自动化还剩 `2` 条未闭环

## 2026-04-18｜只读彻查：`SpringDay1DirectorStagingTests` 剩余 5 条失败的真根归类
- 当前主线目标：
  - 不改代码，只读钉死 `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs` 里 5 条剩余失败到底哪些是真 runtime 债、哪些更像测试债，并判断是否属于 Day1 打包前必须收的范围。
- 本轮子任务：
  1. 直接回读 `SpringDay1DirectorStagingTests.cs` 这 5 条用例源码。
  2. 回读对应 runtime 入口：
     - `SpringDay1DirectorStaging.cs`
     - `NPCAutoRoamController.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - 必要时补 `SpringDay1Director.cs`、`PlayerNpcChatSessionService.cs`、`NPCInformalChatInteractable.cs`、`NPCDialogueInteractable.cs`
  3. 对照当天最新 `Library/CodexEditorCommands/spring-day1-director-staging-tests.json` 的真实失败输出。
- 已完成事项：
  1. 已确认最新真实失败输出就是这 5 条：
     - `NpcTakeover_ShouldDisableRoamAndInteractionsUntilRelease`
     - `RehearsalDriver_ShouldPauseExistingPlaybackUntilDisabled`
     - `ResidentScriptedControl_DebugMoveShouldStillParticipateInSharedAvoidance`
     - `ResidentScriptedControl_PauseAndResumeShouldPreserveScriptedMove`
     - `StagingPlayback_ShouldDriveRoamControllerInsteadOfHardPushingTransformDuringCueMotion`
  2. 已钉实它们不是同一种债：
     - `StagingPlayback...` = 真 runtime 债，当前 playback 仍在 `Update()` 里直推 `transform`，没走 `NPCAutoRoamController` facade。
     - `NpcTakeover...` = 真 runtime 债，`SpringDay1DirectorNpcTakeover.Acquire()/Release()` 只关组件，不再同步 `AcquireStoryControl/ReleaseStoryControl`，导致 owner 对外不可见。
     - `RehearsalDriver...` = 真 editor-runtime/tooling 债，`OnEnable()/OnDisable()` 代码里已没有“暂停已有 playback / 结束后恢复”的逻辑，和 2026-04-07 的目标文档口径发生漂移。
     - `ResidentScriptedControl_DebugMove...` = 更像测试债；当前失败点落在“纯 debug move 仍应 bypass shared avoidance”这半句，但测试只把 `debugMoveActive=true`，没有把 `activePointToPointTravelContract` 设成 `PlainDebug`，不符合现码真实入口 `DebugMoveTo(...)`。
     - `ResidentScriptedControl_PauseAndResume...` = 更像测试债；测试同样没建立 `ResidentScripted / FormalNavigation` travel contract，只手搓 `debugMoveActive + Moving + requestedDestination`，因此 `IsResidentScriptedMoveActive` 不会成立。
  3. 已补一层跨文件一致性核对：
     - `PlayerNpcChatSessionService.Update()`、`NPCInformalChatInteractable`、`NPCDialogueInteractable` 都以 `IsResidentScriptedControlActive / ownerKey / IsFormalNavigationDriveActive()` 作为真实 runtime 合同输入；
     - 所以 `NpcTakeover...` 的失败不是“测试想太多”，而是 staging/takeover 链确实少了一层关键 owner 同步。
- 当前稳定判断：
  1. 这 5 条里，真正该先收的是同一簇：
     - `StagingPlayback_ShouldDriveRoamControllerInsteadOfHardPushingTransformDuringCueMotion`
     - `NpcTakeover_ShouldDisableRoamAndInteractionsUntilRelease`
     两条其实都打在 `SpringDay1DirectorStaging.cs` 的 staging/takeover 合同上。
  2. `RehearsalDriver_ShouldPauseExistingPlaybackUntilDisabled` 也是 runtime/tooling 真债，但更偏编辑器导演工具，不是当前 Day1 packaged 主线的第一优先。
  3. 两条 `ResidentScriptedControl_*` 当前更像 stale tests，没有足够证据说明现 runtime 合同本身坏了。
- 验证状态：
  - `静态推断成立 + 当天现成 targeted 结果已对上`
  - 未改任何业务代码，未新跑 Unity live
- 当前恢复点：
  1. 如果继续真实施工，最小安全顺序应是：
     - 先收 `SpringDay1DirectorStagingPlayback + SpringDay1DirectorNpcTakeover` 这组 runtime 合同
     - 再决定是否顺手把 `RehearsalDriver` 补回“暂停/恢复 playback”
     - 最后才回头改两条 scripted-control stale tests
  2. 不要把这 5 条当成一个“大 staging 全面重构”；当前最小真刀仍应只切 staging/takeover 合同。

## 2026-04-17 遮挡与 Primary 气泡抑制只读核查
- 当前主线仍是 Day1 打包前收尾；本轮是插入式只读核查，不开施工刀。
- 用户新增反馈收窄为两点：
  1. Primary / 工作台引导里，001 的提示气泡疑似被抑制，怀疑 prompt suppression 白名单没做好。
  2. 遮挡已不再表现为“全 Town 失效”，更像树林透明扩散参数偏大。
- 已核实：
  - 图里这句“跟上，到工作台我再慢慢教你。”来自 `SpringDay1Director.TryShowEscortWaitBubble()`，走的是 `NPCBubblePresenter.ShowConversationText()` 对话优先级通道，不是普通 ambient nearby bubble。
  - `NPCBubblePresenter.SetInteractionPromptSuppressed()` 只会压 `Ambient` 通道，不会压 `Conversation` 通道；所以这张图对应的导演气泡理论上属于已白名单。
  - 真正仍未白名单的是 `PlayerNpcNearbyFeedbackService` 这条普通 nearby ambient 气泡链：它会在 `NpcInteractionPriorityPolicy.ShouldSuppressAmbientBubbleForCurrentStory()` 为真时全局压掉，而 `FarmingTutorial` 仍被归为 formal-priority phase，没有给 001 的 Primary/教学引导开例外。
  - 遮挡当前参数实值：`Town.unity` 与 `Primary.unity` 都是 `sameSortingLayerOnly=1`、`useOcclusionRatioFilter=1`、`minOcclusionRatio=0.4`、`enableForestTransparency=1`、`rootConnectionDistance=1.5`、`maxForestSearchDepth=50`、`maxForestSearchRadius=15`。
  - `OcclusionManager.AreTreesConnected()` 除了 `rootConnectionDistance` 外，还存在树冠重叠率 `>= 0.15` 的硬编码连通条件；因此“树林扩散偏大”不只受一个参数控制。
- 结论：
  - 001 气泡问题要分两条看：导演 escort wait 气泡本身是 conversation 白名单；普通 nearby ambient 引导仍被 formal-phase 总抑制吃掉，老问题还在。
  - 遮挡当前不像“整条链坏了”，更像森林连通/扩散口径偏宽；首要关注 `rootConnectionDistance` 与 `maxForestSearchRadius`，其次才是硬编码的树冠重叠阈值。
- 本轮未改代码、未跑 Begin-Slice，Day1-V3 继续保持 `PARKED`。

## 2026-04-18｜只读审计：`SpringDay1DirectorStagingTests` 4 条 semantic/stagebook fail 的真实归因与 pack 前裁定
- 当前主线仍是 Day1 打包前收尾；本轮是插入式只读分析，不开施工刀。
- 用户要求：
  - 彻查 `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs` 里 4 条失败用例：
    - `StageBook_ShouldPreferExactNpcCueBeforeSharedAnchorAndDutyFallback`
    - `StagingPlayback_ShouldUseSemanticAnchorAsCueStartWhenConfigured`
    - `StagingPlayback_ShouldSupportSemanticAnchorOffsetStart`
    - `StagingPlayback_ShouldRebaseLegacyAbsolutePathAroundSemanticAnchorStart`
  - 必须直接对源码、`SpringDay1DirectorStageBook.json` 和最新 test artifact 给出真实根因、入口链、最小安全修法，以及它们是否属于 Day1 当前 pack 前必须收。
- authoritative 结论：
  1. `StageBook_ShouldPreferExactNpcCueBeforeSharedAnchorAndDutyFallback` 的真根因在 `SpringDay1DirectorBeatEntry.TryResolveCue()`：
     - 现在按 `actorCues` 顺序，用 `SpringDay1DirectorActorCue.Matches()` 首个命中即返回；
     - `Matches()` 把 `duty` 也算真命中，因此 `201` entry 会先吃到前面的 `101` cue，而不是后面的 exact `npcId=201` cue。
  2. 这条 exact-cue 失败当前**没有 live packaged runtime 现役调用点**：
     - 仓内静态搜索未发现 `ResolveSpawnPoint()` 的实际调用；
     - 当前实际消费者是 `SpringDay1DirectorStagingWindow.TryResolveDraftCueForPreview()` 的 editor 预演链，以及 `SpringDay1NpcCrowdDirector.ResolvePreferredSemanticAnchor()/TryResolveTownContractAnchor()` 这组暂时未被外层走到的 helper。
  3. 三条 `StagingPlayback_*Semantic*` fail 的根因一致：
     - `SpringDay1DirectorStagingPlayback.ApplyCue()` 目前完全忽略 `useSemanticAnchorAsStart` 与 `startPositionIsSemanticAnchorOffset`；
     - `ResolveTargetPosition()` 只在 `pathPointsAreOffsets=true` 时做偏移，不会为 legacy absolute path 做围绕 semantic start 的重基；
     - 最新 `Library/CodexEditorCommands/spring-day1-director-staging-tests.json`（`2026-04-18T01:47:38.6103382+08:00`）已直接打出：
       - semantic start 仍落到 `99,99`
       - semantic offset 仍落到 `0.5,-0.25`
       - legacy path rebase 仍落到旧 `5,4`
  4. 但这 3 条当前也**不是 Day1 packaged runtime 硬 blocker**：
     - live crowd runtime 只在 `EnterVillage_PostEntry / DinnerConflict_Table` 走 `ApplyStagingCue() -> ResolveRuntimeCueOverride() -> playback.ApplyCue()`；
     - `ResolveRuntimeCueOverride()` 已先把 scene-marker cue 改写成绝对 `startPosition/path`，并显式清掉 `useSemanticAnchorAsStart/startPositionIsSemanticAnchorOffset/pathPointsAreOffsets`；
     - `FreeTime_NightWitness / ReturnAndReminder_WalkBack / DayEnd_Settle / DailyStand_Preview` 当前不走 live staging playback 这条链。
  5. 这些测试也不是空想：
     - `SpringDay1DirectorStageBook.json` 现在确实已落了 migrated semantic-start 数据，例如 `reminder-bg-203`、`reminder-bg-201`、`dayend-watch-301`、`daily-101/103/102/104/203/201`；
     - 现状是“数据已迁，playback/editor 底座没跟上，runtime 又暂时绕开这套底座”。
- 最小安全修法：
  1. 在 `SpringDay1DirectorBeatEntry.TryResolveCue()` 增加三段优先级：
     - 先 exact `npcId`
     - 再 `semanticAnchorId`
     - 最后才 `duty`
     不再用“首个 `Matches()` 命中”。
  2. 在 `SpringDay1DirectorStagingPlayback.ApplyCue()` 增加 semantic start 解析：
     - `cue.useSemanticAnchorAsStart=true` 时，先用 `SpringDay1DirectorSemanticAnchorResolver.TryResolveWorldPosition(...)` 取锚点；
     - `cue.startPositionIsSemanticAnchorOffset=true` 时，起点为 `anchor + cue.startPosition`；
     - 否则起点直接取 anchor。
  3. 在 `ResolveTargetPosition()` 增加 legacy rebase 分支：
     - `pathPointsAreOffsets=true` 时继续按 resolved start 做 offset；
     - `useSemanticAnchorAsStart=true && !startPositionIsSemanticAnchorOffset && !pathPointsAreOffsets` 时，按 `resolvedStart - cue.startPosition` 去重基旧 absolute path。
  4. 所有分支都只在 `cue.useSemanticAnchorAsStart` 为真时生效，避免污染现有绝对坐标 cue。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1DirectorStagingTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Directing\SpringDay1DirectorStaging.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1NpcCrowdDirector.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1DirectorStagingWindow.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Resources\Story\SpringDay1\Directing\SpringDay1DirectorStageBook.json`
  - `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\spring-day1-director-staging-tests.json`
- 验证结果：
  - 纯静态审计 + 最新 staging artifact 回读成立；
  - 未改代码，未跑 `Begin-Slice`，未做 live/packaged 复测。
- pack 前裁定：
  1. 这 4 条都**不建议**作为 Day1 当前 packaged runtime 的“必须先收”。
  2. 若本轮目标是恢复 `SpringDay1DirectorStagingWindow` 预演可信度，或清 `Director Staging` 套件噪音，可把 3 条 playback bug 当一组工具债一起收。
  3. exact-cue 这条更像 editor/helper 正确性债，应并入 `Package F / F-07`，不该反向阻断打包主线。
- 当前恢复点：
  1. Day1 当前更像 pack-blocker 的仍是 `staging/takeover` runtime 合同那一簇，不是这 4 条 semantic/stagebook fail。
  2. 如果后续继续施工，这 4 条更适合作为 `Package F / editor-preview cleanup` 单独一刀。

## 2026-04-18｜只读审计：0417 `Package F` 仍 pending 的 `F-05/F-06/F-07/F-08` 残依赖复核
- 当前主线目标：
  - 继续服务 `0417.md` 的 Day1 收尾，但这轮不改代码，只把 `Package F` 还没清掉的 editor / validation / tests / runtime-data 残依赖按 `F-05~F-08` 钉成高置信事实。
- 本轮已确认：
  1. `F-05｜editor 菜单旧常量`
     - `Assets/Editor/Story/SpringDay1ActorRuntimeProbeMenu.cs` 仍把 `Town_Day1Residents` 放在 `KnownNpcRootNames`，而 `ResolveTrackedNpcId()` 仍会把“处于这些 root 下的对象”当成 tracked NPC 真值输入。
  2. `F-06｜bootstrap/validation 旧 beat/anchor`
     - 本轮没扫到 bootstrap 文件里直接写死旧 anchor/beat；
     - 但 `TownScenePlayerFacingContractMenu.cs` 仍把 `EnterVillageCrowdRoot / DinnerBackgroundRoot / NightWitness_01 / DailyStand_01` 当 probe runtime anchors；
     - `SpringDay1DirectorPrimaryLiveCaptureMenu.cs` 仍把 `FreeTime_NightWitness / DailyStand_Preview` 与 `night-witness-102 / daily-*` 当固定 capture target；
     - `SpringDay1DirectorTownContractMenu.cs` 仍把 `003` 放进 `TargetNpcIds`，并要求 `003起点/003终点` staged markers 一起齐全。
  3. `F-07｜tests 旧真值`
     - `NpcCrowdManifestSceneDutyTests.cs` 仍把旧 semantic anchor / beat matrix 当 expected truth，连 `DinnerBackgroundRoot` 都被要求必须出现在 snapshot priority anchors。
     - `NpcCrowdResidentDirectorBridgeTests.cs` 仍要求 `EnterVillage_PostEntry / DinnerConflict_Table` 的 manifest-backed resident cue 桥接持续成立，并用 `AssertCueContract()` 把 cue 的 `semanticAnchorId/duty` 继续绑在 manifest 白名单上。
     - `SpringDay1DirectorStagingTests.cs` 里仍有 editor-preview/stagebook 真值测试继续保护旧 semantic-anchor/exact-cue 矩阵；其中还保留 `003` dinner route 必须从 scene markers 解析的断言。
  4. `F-08｜runtime/data 残留`
     - `SpringDay1NpcCrowdManifest.asset`、`SpringDay1TownAnchorContract.json`、`SpringDay1DirectorStageBook.json` 仍直接保存 `EnterVillageCrowdRoot / DinnerBackgroundRoot / NightWitness_01 / DailyStand_*`。
     - `SpringDay1DirectorStageBook.json` 里仍有 `cueId/npcId/semanticAnchorId = 003` 的 opening cue。
     - `SpringDay1NpcCrowdDirector.cs` 仍会在 manifest 不含 `003` 时合成 `BuildSyntheticThirdResidentResidentEntry()`。
     - `SpringDay1Director.cs` 仍保留 `ThirdResidentNpcId`、`TownOpeningThirdResidentPointName = "003终点"`，并在 `TryResolveTownVillageGateHardFallbackTarget()` 里给 `003` 硬编码 fallback 坐标。
- 本轮额外确认的“无命中”：
  1. `Assets/000_Scenes/Town.unity` 与 `Assets/000_Scenes/Primary.unity` 中，exact search 未再命中 `Town_Day1Residents / Town_Day1Carriers / EnterVillageCrowdRoot / DinnerBackgroundRoot / NightWitness_01 / DailyStand_01~03`。
  2. `Town_Day1Carriers` 在 `Assets/Editor + Assets/YYY_Scripts + Assets/YYY_Tests + Assets/Resources + Assets/000_Scenes` 下无命中。
  3. `Town_Day1Residents` 在 runtime/data/scene 范围无命中；当前 exact code hit 只剩 editor probe 菜单与旧迁移禁用提示。
- 当前判断：
  1. `Package F` 现在更像“editor/test/data truth cleanup”，不是 scene 文本残根没删。
  2. 真正有 runtime-risk 的尾巴主要是：
     - data 资产仍保存旧 anchor/beat；
     - `003` 仍有 synthetic runtime + director hard fallback 两层特殊入口。
- 当前恢复点：
  1. 如果继续真实施工，最小安全顺序应是：
     - 先清 `F-05/F-06/F-07` 的 editor/test truth；
     - 再收 `F-08` 的 data/runtime；
     - 不要反过来先删 data，再让 validation/tests 全部报假 blocker。
  2. 本轮始终只读，未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`。

## 2026-04-18｜Package F 主干已收口，G 自动化 fresh 证据补到 scene/probe/live-partial
- 当前主线目标：
  - 继续按 `0417.md` 收 Day1 打包前必要项；这轮真实施工只收 `F-06/F-07` 的 editor/test 残依赖，并把 `F-09/G-01~G-04` 的 fresh 自动化证据补齐。
- 本轮子任务：
  1. 清 `TownScenePlayerFacingContractMenu.cs` 的旧 anchor blocker。
  2. 清 `NpcCrowdManifestSceneDutyTests.cs` 里仍把旧 semantic anchor 名当 expected truth 的断言。
  3. 重跑 scene/editor/data probe，并尝试补一段真实 PlayMode trace。
- 已完成事项：
  1. `TownScenePlayerFacingContractMenu.cs`
     - `CriticalEntryAnchors/RuntimeAnchors` 已退成空集合，不再拿 `EnterVillageCrowdRoot / KidLook_01` 当 player-facing blocker。
  2. `NpcCrowdManifestSceneDutyTests.cs`
     - `ExpectedEntry` 已不再携带旧 semantic anchor 白名单，测试只保留 duty/phase/growthIntent 真合同。
  3. fresh `validate_script`
     - `TownScenePlayerFacingContractMenu.cs`=`no_red`
     - `NpcCrowdManifestSceneDutyTests.cs`=`no_red`
  4. fresh scene/editor/data probe：
     - `Run Town Entry Contract Probe`=`completed`
     - `Run Town Player-Facing Contract Probe`=`completed`
     - `Run Day1 Staging Marker Probe`=`completed`
     - `Run Resident Director Bridge Tests`=`3/3 passed`
     - `Run Town Runtime Anchor Readiness Probe`=`deprecated-runtime-anchor-readiness-probe`
       这是预期退役口径，不再当 blocker
  5. fresh console：
     - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 5` => `0 error / 0 warning`
  6. fresh live partial：
     - `Reset Spring Day1 To Opening + Live Snapshot Artifact` 的真实 PlayMode trace 已从 `CrashAndMeet` 推到 `EnterVillage`
     - `FreeTime` live snapshot 已拿到
     - `Force Spring Day1 Dinner Validation Jump` 当前 snapshot 仍落在 `FarmingTutorial / 16:00 / FreeTime_NightWitness`，不能包装成 dinner live 已验
  7. `0417.md` 已回写：
     - `L-08 / L-09`
     - `I-14 / I-16`
     - `Package F`=`已完成`
     - `Package G`=`进行中`
     - `G-01/G-02/G-03`=`已完成`
- 当前判断：
  1. `Package F` 当前可以按“主干已收口”处理；scene/editor/test 不再被旧中间层继续反咬。
  2. `G` 现在只站住：
     - 自动化 compile/test/console clean
     - live partial trace 已有
  3. 仍不能写成：
     - packaged 已过
     - profiler 已过
     - 用户体验已过线
- 当前恢复点：
  1. 如果继续 Day1 这条线，下一步只剩：
     - `G-04` 更完整的 live 路径
     - `G-05` packaged 用户路径
     - `G-06` profiler
     - `G-07` 最终验收包
  2. 如果先停，这轮必须同步父层 memory、线程 memory、skill-trigger-log，并按现场决定是否 `Park-Slice`。

## 2026-04-18｜补记：`G-04/G-06` 最新自动化事实仍未打到 `Town`
- 当前主线目标：
  - 继续按 `0417.md` 推 `Package G`，先补漏记事实，再继续查 live/profiler 能不能安全进 `Town`。
- 本轮新增事实：
  1. `Request Spring Day1 Town Lead Transition Artifact`
     - 最新 artifact：`actionResult = transition-requested:PrimaryHomeDoor`
     - 但 `activeScene = Assets/000_Scenes/Home.unity`
     - 没有进入 `Town`
  2. `NpcRoamSpikeStopgapProbe`
     - 最新报告：`scene = Primary`
     - `npcCount = 0`
     - `roamNpcCount = 0`
     - 因此不能当 `Town/free-roam/20:00` profiler 证据
- 当前判断：
  1. `G-04` 现在仍只有 `opening -> EnterVillage` 与 `FreeTime` 的 partial live。
  2. `G-06` 现在仍没有 fresh Town profiler spot check。
  3. 这两条必须继续查现成命令桥/菜单链，不能包装成“终验已过”。
- 当前恢复点：
  1. 下一步优先查现有 `CodexEditorCommandBridge + live menu` 是否能安全把自动链真正带进 `Town`。
  2. 如果仍打不到，就把它们明确留在 `G-04/G-06` blocker，而不是回头重开 `Package F`。

## 2026-04-18｜继续施工：`G-04/G-06` 已补到 `Town free-time / 20:00`，当前剩余 blocker 收窄到 dinner
- 当前主线目标：
  - 继续按 `0417.md` 推 `Package G`，在不碰 runtime 主链的前提下，把现成 live 菜单链尽量推到更完整。
- 本轮代码改动：
  1. `Assets/Editor/Story/SpringDay1LiveSnapshotArtifactMenu.cs`
     - `Request Spring Day1 Town Lead Transition Artifact` 现在先走 `director.TryRequestValidationEscortTransition()`；
     - 若没有 pending escort，再按当前场景/`TargetSceneName` 解析正确 trigger；
     - 不再退回“抓到第一个 trigger 就切”的脏 fallback。
  2. `Assets/Editor/Story/SpringDay1LatePhaseValidationMenu.cs`
     - 新增 `Advance Spring Day1 Validation Clock +1h`；
     - 用 `director.TryNormalizeDebugTimeTarget()` + `TimeManager.SetTime()` 做受控时间推进。
- fresh 验证：
  1. `validate_script`
     - `SpringDay1LiveSnapshotArtifactMenu.cs`=`no_red`
     - `SpringDay1LatePhaseValidationMenu.cs`=`no_red`
  2. fresh live：
     - `Town -> Primary` settled snapshot 真实落到 `Primary.unity`
     - `Primary -> Town` settled snapshot 真实落到 `Town.unity`
     - `Reset -> step` 已推进到 `FarmingTutorial`
     - `Force FreeTime + Request Transition` 已真实拿到 `Town / FreeTime / 19:30`
     - `Advance +1h` 已真实拿到 `Town / FreeTime / 20:00`
  3. fresh runtime spot probe：
     - `NpcRoamSpikeStopgapProbe`
       - `Town 19:30`=`npcCount 26 / roamNpcCount 26`
       - `Town 20:00`=`npcCount 26 / roamNpcCount 25`
  4. fresh blocker 复钉：
     - `Force Spring Day1 Dinner Validation Jump` 当前仍只到 `FarmingTutorial / 16:00`
     - 即使请求回 `Town`，也仍未进入 `DinnerConflict`
- 当前判断：
  1. `G-04` 已不再只是 opening partial；现在至少已经覆盖到：
     - `opening`
     - `primary`
     - `healing`
     - `workbench`
     - `farming`
     - `Town free-time 19:30`
     - `Town 20:00`
  2. `G-06` 的 `Town` scene-entry/free-roam/20:00` runtime spot probe` 已有。
  3. 当前最实的剩余 blocker 已收窄成：
     - `DinnerConflict` 入口自动链
     - `19:00/21:00/次日`
     - `G-05` packaged
     - 真正人工 profiler/最终验收
- 当前恢复点：
  1. 后续若继续收 `Package G`，优先盯晚饭入口和夜间后半段，不要回头重做 `Town` free-time/20:00`.
  2. 用户汇报时必须明确区分：
     - `Town spot probe 已有`
     - `dinner/live full-path` 仍未完成。

## 2026-04-18｜继续施工补记：`21:00` 已补 probe，次日当前只到 `Home/DayEnd`
- 当前主线目标：
  - 在 `Town free-time / 20:00` 已打通的基础上，继续把晚段后半截的 live/probe 往后推。
- 本轮新增事实：
  1. 同一条稳定链已经继续补到：
     - `Town / FreeTime / 21:00`
  2. `NpcRoamSpikeStopgapProbe`
     - `21:00`=`npcCount 16 / roamNpcCount 16`
  3. 但紧接着补跑：
     - `spring-day1-actor-runtime-probe.json`
     - `spring-day1-resident-control-probe.json`
     结果里 Day1 tracked resident 已都不在 active / visible / scripted-control 列表
  4. 连续再推进到次日后：
     - 当前 settled snapshot 落到 `Home.unity / DayEnd / 07:00 AM`
     - `NpcRoamSpikeStopgapProbe`=`0 / 0`
     - 这还不是 `Town` 侧 morning release
- 当前判断：
  1. `21:00` 的 `16/16` 不能直接解读成“Day1 resident 还没隐藏”。
  2. 当前更准确的口径是：
     - Day1 tracked resident 在 `21:00` probe 侧已收掉
     - `Town` 侧次日释放仍待专门 live
  3. `Package G` 当前最实的剩余 blocker 进一步收窄到：
     - `DinnerConflict`
     - `Town` 侧次日 `07:00` release
     - packaged / 手工 profiler / 最终验收

## 2026-04-18｜只读审计补记：`DinnerConflict` 入口仍卡在 `FarmingTutorial / 16:00` 的静态根因
- 当前主线目标：
  - 只读钉死为什么现有 live validation 还进不了 `DinnerConflict`，并给出最小、最安全的 `editor-only helper` 插入位。
- 本轮新增事实：
  1. `Force Spring Day1 Dinner Validation Jump` 的同步调用顺序当前存在竞态：
     - `SpringDay1LatePhaseValidationMenu.ForceDinnerValidationJump()`
     - `PreparePostTutorialExploreWindow()`
     - `TryRequestDinnerGatheringStart(true)`
     - 失败后直接 `ActivateDinnerGatheringOnTownScene()`
  2. 真正的问题不在 runtime 晚饭主链，而在菜单没有等待 `EnterPostTutorialExploreWindow()` 的异步 blink/切场完成：
     - `EnterPostTutorialExploreWindow()` 在 `Primary` 优先走 `SceneTransitionRunner.TryBlink(...)`，可能先 `return`
     - 这时 `_postTutorialExploreWindowEntered` 还没真正落稳
     - 紧接着调用的 `TryRequestDinnerGatheringStart()` 依赖 `IsPostTutorialExploreWindowActive()`，于是直接返回 `false`
     - fallback `ActivateDinnerGatheringOnTownScene()` 如果当前还不在 `Town`，也会立刻 early-return
     - 结果现场只留下 `FarmingTutorial / 16:00`
  3. 现有通用 stepper 也不会自己补这段桥：
     - `SpringDay1LiveValidationRunner.TriggerRecommendedAction()` 在 `FarmingTutorial` 只委托 `SpringDay1Director.TryAdvanceFarmingTutorialValidationStep()`
     - 该方法在 5 个教学目标完成后只返回“接下来应和村长收口”，没有继续触发 `postTutorial wrap / explore window / dinner request`
- 最小安全修口建议：
  1. 不改 runtime 主链。
  2. 只在 `Assets/Editor/Story/SpringDay1LatePhaseValidationMenu.cs` 增加一个延迟/轮询型 `editor-only helper`：
     - 先做 `PreparePostTutorialExploreWindow()`
     - 等 `_postTutorialExploreWindowEntered=true` 且 `SceneTransitionRunner.IsBusy=false`
     - 再调用现有 runtime 方法 `TryRequestDinnerGatheringStart(true)`
     - 如已切到 `Town` 再补 `ActivateDinnerGatheringOnTownScene()`
     - 直到 `StoryManager.CurrentPhase == StoryPhase.DinnerConflict` 或超时退出
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1LatePhaseValidationMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1LiveSnapshotArtifactMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\DialogueDebugMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
- 验证结果：
  - 纯静态审计，未改代码，未进 Unity，未跑 `Begin-Slice`
- 当前恢复点：
  1. 下一刀若要修，只需要补 `editor-only` 等待 helper，不要先改 `SpringDay1Director` 晚饭主链。
  2. 在 helper 落地前，`TriggerRecommendedAction/Step` 仍只能稳定到 `FarmingTutorial` 收口口径，不能自动跨进 `DinnerConflict`。

## 2026-04-18｜只读工具链入口审计：当前最短 packaged build 与 profiler spot check 路径
- 当前主线目标：
  - 只读查清当前最短、最安全的 `packaged build` 与 `profiler spot check` 入口，不改代码、不补新脚本。
- 本轮新增事实：
  1. 仓内未找到现成 `BuildPipeline.BuildPlayer` / `BuildPlayerOptions` / 自定义 `-executeMethod` 打包入口。
     - `scripts/` 现有 `sunset_mcp.py`、`sunset-mcp.ps1` 等只覆盖 compile / console / validation，不提供 player build。
     - 当前 `Assets/Editor/` 也没有 project-local 一键 build 菜单。
  2. 因此当前最短最安全的 packaged build 入口仍是 Unity 自带 `Build Profiles / Build Settings`。
     - 这轮只读核到磁盘字面的 `ProjectSettings/EditorBuildSettings.asset` 已包含：
       - `Assets/000_Scenes/Town.unity`
       - `Assets/000_Scenes/Primary.unity`
       - `Assets/000_Scenes/Home.unity`
     - 对 Day1 / Town / Home 这条 packaged 路径，当前至少不再卡在“场景没进 Build Profiles”这一层。
  3. 本机当前可用 Unity 安装路径已能从 `Editor.log` 直接读到：
     - `D:\1_BBB_Platform\Unity\6000.0.62f1\Editor\Unity.exe`
     - 但仓内没有与之配套的 `build` execute-method，因此不能把“命令行打开 Unity”误写成“命令行一键打包已具备”。
  4. profiler 侧当前已有一批 editor menu + artifact helper，但它们都仍是 targeted/live helper，不是 Unity Profiler 本体：
     - `Sunset/Story/Validation/Write Spring Day1 Live Snapshot Artifact`
     - `Sunset/Story/Validation/Force Spring Day1 FreeTime Validation Jump`
     - `Sunset/Story/Validation/Force Spring Day1 Dinner Validation Jump`
     - `Sunset/Story/Validation/Advance Spring Day1 Validation Clock +1h`
     - `Sunset/Story/Validation/Force Spring Day1 Morning Town Validation Jump`
     - `Sunset/Story/Validation/Write Spring Day1 Resident Control Probe Artifact`
     - `Sunset/Story/Validation/Write Spring Day1 Actor Runtime Probe Artifact`
     - `Tools/Sunset/NPC/Run Roam Spike Stopgap Probe`
  5. `0417.md` 当前口径已明确把二者分开：
     - 上述 helper 已足以把 `Town free-time / dinner / 20:00 / 21:00 / 次晨 Town` 的 targeted live path 与 JSON artifact 补齐；
     - 但 `G-06` 仍保守写成“当前真正剩下的是人工 Unity Profiler 手工 spot check”。
  6. 现成结果与命令桥落点已在：
     - `Library/CodexEditorCommands/status.json`
     - `Library/CodexEditorCommands/spring-day1-live-snapshot.json`
     - `Library/CodexEditorCommands/spring-day1-resident-control-probe.json`
     - `Library/CodexEditorCommands/spring-day1-actor-runtime-probe.json`
     - `Library/CodexEditorCommands/npc-roam-spike-stopgap-probe.json`
     - 以及 `Library/CodexEditorCommands/requests/*.cmd` + `CodexEditorCommandBridge`
- 最短执行建议：
  1. `packaged build`
     - 当前最短安全路不是 repo 脚本，而是直接进 Unity Editor 的 `Build Profiles / Build Settings` 做 Windows player build。
  2. `profiler spot check`
     - 先用现有菜单链把现场稳定推进到目标时段，并回读 `Library/CodexEditorCommands/*.json` 确认场景 / phase / beat / actor/runtime 状态；
     - 真正的性能 spot check 仍必须人工打开 Unity Profiler 采样，不能拿 stopgap probe / snapshot artifact 顶替。
- 失败信号 / 日志位置：
  1. build 失败先看：
     - Unity Console
     - `%LOCALAPPDATA%\\Unity\\Editor\\Editor.log`
     - 其中 `Build completed with a result of 'Succeeded' / 'Failed'` 是当前已有历史口径。
  2. packaged 运行后看：
     - `C:\\Users\\aTo\\AppData\\LocalLow\\DefaultCompany\\Sunset\\Player.log`
  3. menu / probe 是否真正执行到位：
     - `Library/CodexEditorCommands/status.json`
     - 对应 `spring-day1-*.json` / `npc-roam-spike-stopgap-probe.json`
- 验证结果：
  - 纯只读审计；未改文件、未进 Unity、未跑 `Begin-Slice`。
- 当前恢复点：
 1. 若下一轮只想拿最短 packaged smoke，直接走 Unity 自带 build。
 2. 若下一轮想先缩小 profiler 风险面，先用现有 validation 菜单把现场带到 `free-time / dinner / 20:00`，再人工开 Unity Profiler。

## 2026-04-18｜文档收口：`0417` 已回写成可下决策的终验板，`0418_打包终验包.md` 已落地
- 当前主线目标：
  - 把 Day1-V3 这条线从“结构/targeted/live helper 已补齐但板子还散着”收成真正可交接的终验状态，不再让 `0417` 停留在旧未勾项和旧 blocker 上。
- 本轮实际做成：
  1. 已更新 `0417.md`：
     - 新增 `C-25`，把 packaged build / Profiler 的工具链真相钉死：
       - 仓内没有一键打包脚本
       - 当前 build 入口只能走 Unity 自带 `Build Profiles / Build Settings`
       - stopgap probe / live artifact 不能冒充人工 Profiler
     - 把这些旧未勾项回写为当前真实状态：
       - `C-02`
       - `C-08`
       - `D-02`
       - `D-03`
       - `D-08b`
     - `Package G` 状态改成：
       - `待人工终验`
     - `G-07` 已标记完成，并挂接新验收包文件
     - 新增 `8.1 当前封板判断`：
       - runtime / targeted / fresh live helper 已封板
       - 当前真正剩下的是 `G-05 packaged` 与 `G-06` 人工 Profiler
  2. 已新建：
     - `D:\\Unity\\Unity_learning\\Sunset\\.kiro\\specs\\900_开篇\\spring-day1-implementation\\100-重新开始\\0418_打包终验包.md`
     - 用来统一承接：
       - build 路径
       - 最少必测包
       - Profiler spot check
       - 失败判读
       - 最终回执
  3. 已只读核实并吸收两个关键判断：
     - `B-02/B-03/E-03/C-07` 仍属于结构债或归因信心缺口
     - 但当前不再和“打包前最小闭环是否还能继续推进”混成一件事
- 这轮额外探测到的真实 blocker：
  1. 我尝试用 `CodexEditorCommandBridge` 自动串 `PLAY -> RESET -> jump/probe -> STOP` 补 `G-06`，但桥层在当前机器上处理 `.cmd` 的延迟是“分钟级”而不是“秒级”：
     - `PLAY` 实际成功进了 PlayMode，但我原脚本 60 秒超时会误判失败
     - `RESET` 与 `STOP` 后续也都进了 `archive`，只是速度过慢
  2. 因此这轮不能把“桥层能慢慢吃掉命令”包装成“G-06 自动套跑已稳定”
  3. Unity 现已确认退回 `EditMode`
- 涉及文件：
  - `D:\\Unity\\Unity_learning\\Sunset\\.kiro\\specs\\900_开篇\\spring-day1-implementation\\100-重新开始\\0417.md`
  - `D:\\Unity\\Unity_learning\\Sunset\\.kiro\\specs\\900_开篇\\spring-day1-implementation\\100-重新开始\\0418_打包终验包.md`
  - `D:\\Unity\\Unity_learning\\Sunset\\Library\\CodexEditorCommands\\status.json`
  - `D:\\Unity\\Unity_learning\\Sunset\\Library\\CodexEditorCommands\\archive`
- 验证结果：
  1. `git diff --check` 覆盖 `0417.md / 0418_打包终验包.md` 通过
  2. Unity 当前状态：
     - `isPlaying=false`
     - `isCompiling=false`
     - `lastCommand=playmode:EnteredEditMode`
- 当前恢复点：
  1. 如果下一轮继续，不再优先折腾桥层批处理自动化。
  2. 直接按 `0418_打包终验包.md` 进入：
     - packaged build 用户路径复验
     - 人工 Unity Profiler spot check

## 2026-04-18｜只读复核：`Primary 001` 的等待提示问题不是对象差异，而是我把 `WorkbenchEscort` 做成了新合同
- 当前主线目标：
  - 只读查清“为什么 `Primary` 的 `001` 等待提示又坏了，为什么本该只换文案却被我做成了不同逻辑”，并把这次偏移写回主控板。
- 本轮实际做成：
  1. 已核实 `Primary` 与 `Town` 里真正被导演 resolve 到的 `001` 都有：
     - `NPCAutoRoamController`
     - `NPCBubblePresenter`
  2. 所以这次问题不是“Primary 的 001 和 Town 不是同一种对象”。
  3. 已钉死真正偏移点在：
     - `SpringDay1Director.TryHandleWorkbenchEscort()`
  4. 已确认这条链不是“只复用 `TownHouseLead` 的等待合同然后换一句文案”，而是被我扩成了：
     - `escort wait`
     - `workbench target`
     - `idle placement`
     - `briefing / ready gate`
     的复合 runtime。
  5. 代码级最硬的 drift：
     - `TownHouseLead` 等待时只暂停 `chief`
     - `WorkbenchEscort` 等待时我把 `chief/companion` 都一起暂停
     - `TownHouseLead` 还会继续驱动 `002` 向编队位补位
     - `WorkbenchEscort` 等待窗口直接 `return`
  6. 已同步回写：
     - `0417.md`
       - `C-24a`
       - `I-21`
       - `C-08a`
- 当前判断：
  1. 用户这次说得对，这不是单纯换文案，而是我第二次把已知正确的提示合同重做成了另一套逻辑。
  2. 这轮仍然只是：
     - `结构 / checkpoint` 的只读归因
     - 不是 runtime 已修
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\CraftingStationInteractable.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\100-重新开始\0417.md`
- 验证结果：
  - 纯只读审计；未改 runtime，未进 Unity，未跑 `Begin-Slice`。
- 当前恢复点：
 1. 下一刀如果开修，不应先继续猜 scene 差异。
 2. 应直接把 `WorkbenchEscort` 等待提示合同收回到 `TownHouseLead` 的已知正确基线，再单独处理 workbench briefing / ready / E 交互。

## 2026-04-18｜真实施工：夜间 return-home 到站/隐藏判定已对齐 formal-navigation，roam profile 已改成 `0.5~5 / 3~8`
- 当前主线目标：
  - 收掉用户这轮最新三条硬问题：
    1. `20:00` 到家后为什么还不隐藏
    2. `101` 为什么特别容易卡在回家链上
    3. roam 停顿节奏改成现有字段下的 `shortPause 0.5~5 / longPause 3~8`
- 本轮实际做成：
  1. 已跑 `Begin-Slice`
     - `ThreadName=Day1-V3`
     - `CurrentSlice=night-return-arrival-closeout-and-roam-pause-tuning`
  2. 在 [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 把夜间 return-home 的 crowd 判定从“只看 raw home anchor 的近点”收成：
     - `arrival radius = 0.64`
     - 同时对齐 `NPCAutoRoamController.TryResolveFormalNavigationDestination(...)` 返回的 formal-navigation 实际到站代理点
  3. 在 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 新增 runtime-only 语义口：
     - `TryResolveFormalNavigationDestination(...)`
     - 只给 runtime owner 对齐“authored anchor”和“formal-navigation 实际到站点”使用，不是新移动入口
  4. 在 [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs) 新增回归：
     - `CrowdDirector_TickResidentReturnHome_ShouldHideWhenResidentIsWithinFormalArrivalTolerance`
  5. 已把现用人类 NPC profile 改成：
     - `shortPause 0.5~5`
     - `longPause 3~8`
     - 覆盖：
       - `NPC_Default`
       - `001`
       - `002`
       - `003`
       - `101~203`
       - `301`
- 这轮关键判断：
  1. `101` 不是缺 `HomeAnchor`，也不该写特判。
  2. 更像是它的 `HomeAnchor` 更容易落在“formal-navigation 会取 nearby proxy 到站、但 crowd 之前仍拿 raw anchor 判 finish”的坏窗口里。
  3. 这轮修的是合同对齐，不是给 `101` 打补丁。
- 验证结果：
  1. `git diff --check` 覆盖本轮 own 路径通过
  2. `validate_script` 覆盖：
     - `NPCAutoRoamController.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - `SpringDay1DirectorStagingTests.cs`
     都是 `owned_errors=0`
  3. 但 CLI assessment 当前被外部现场压成 `external_red`
     - blocker 是 `DialogueChinese V2 SDF.asset` importer inconsistent result
     - 另有 Unity `stale_status`
  4. 我尝试跑 `Ready-To-Sync`，没撞到代码 blocker，而是卡在：
     - `.kiro/state/ready-to-sync.lock`
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1NpcCrowdDirector.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1DirectorStagingTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_DefaultRoamProfile.asset`
  - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_001_VillageChiefRoamProfile.asset`
  - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_002_VillageDaughterRoamProfile.asset`
  - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_003_ResearchReviewProfile.asset`
  - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\SpringDay1Crowd\NPC_101_LedgerScribeRoamProfile.asset`
  - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\SpringDay1Crowd\NPC_102_HunterRoamProfile.asset`
  - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\SpringDay1Crowd\NPC_103_ErrandBoyRoamProfile.asset`
  - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\SpringDay1Crowd\NPC_104_CarpenterRoamProfile.asset`
  - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\SpringDay1Crowd\NPC_201_SeamstressRoamProfile.asset`
  - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\SpringDay1Crowd\NPC_202_FloristRoamProfile.asset`
  - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\SpringDay1Crowd\NPC_203_CanteenKeeperRoamProfile.asset`
  - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\SpringDay1Crowd\NPC_301_GraveWardenBoneRoamProfile.asset`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\100-重新开始\0417.md`
- 当前恢复点：
  1. 如果下一轮继续夜间线，先补 packaged/live 复测，不要先回去给 `101` 写特判。
  2. 如果下一轮要真收口，先查清 `ready-to-sync.lock` 的状态层 blocker，再决定是否进 sync。

## 2026-04-18｜真实施工：`101 DayEnd_Settle` authored 真值收回普通 resident，不写 runtime 特判
- 当前主线目标：
  - 只收用户最新单点：`101` 夜间仍异常，但不能写 `101` 专用代码分支。
- 本轮子任务：
  - 对位 `101` 的 manifest / scene / prefab 差异，优先修 data truth，而不是继续扩 runtime 链。
- 已完成事项：
  1. 重新核对 `SpringDay1NpcCrowdManifest.asset / Town.unity / 101~103 prefab` 后，确认：
     - `101_HomeAnchor` 引用存在
     - prefab 结构无明显 101 特异点
     - 当前最硬的静态差异是 `101 DayEnd_Settle` 仍被写成 `Pressure + AmbientPressure`
  2. 已把 `101 DayEnd_Settle` 改成：
     - `presenceLevel: 4 -> 2`
     - `flags: 224 -> 192`
  3. 已同步更新 `NpcCrowdManifestSceneDutyTests.cs` 的 resident semantic matrix 预期。
  4. 已回写 `0417.md`：
     - `C-30`
     - `G-09`
     - `I-09 / I-10`
     - `E-11`
- 验证结果：
  1. `validate_script Assets/YYY_Tests/Editor/NpcCrowdManifestSceneDutyTests.cs` => `assessment=no_red / owned_errors=0 / external_errors=0`
  2. direct MCP `run_tests(EditMode)`：
     - `NpcCrowdManifestSceneDutyTests.CrowdManifest_ShouldExposeResidentSemanticMatrix_ForVillageResidentLayer`
     - `completed=1 / total=1 / status=succeeded`
  3. `git diff --check` 覆盖：
     - `SpringDay1NpcCrowdManifest.asset`
     - `NpcCrowdManifestSceneDutyTests.cs`
     - `0417.md`
     通过
- 关键决策：
  1. 这刀属于 authored truth 修正，不是 `101` runtime 特判。
  2. 现在能诚实说的是：
     - `结构 / targeted test` 已补
     - `101` packaged/live 是否彻底过线，仍待用户复测
- 当前恢复点：
1. 如果继续夜间线，下一步先看用户 fresh packaged/live 对 `101` 的结果。
2. 不要回头把这件事改写成 scene 坐标硬编码或 `npcId==101` 分支。

## 2026-04-18｜只读结论：`重新开始` 后 Town world reset 不完整，最高概率是 restart 路径先吃了旧 Town snapshot
- 当前主线目标：
  - 只读查清“重新开始游戏后，Town 里的树/石头等 world object 没像 Primary 一样完整刷新”的真实代码链，并给出最小安全修法。
- 本轮子任务：
  - 精读 `SaveManager.cs`、`PersistentPlayerSceneBridge.cs`、`SaveDataDTOs.cs`、`TreeController.cs`、`StoneController.cs`、`WorldStateContinuityContractTests.cs`，只追 `restart / scene world restore / off-scene snapshot` 相关方法。
- 已完成事项：
  1. 已确认普通跨场景链走 `PersistentPlayerSceneBridge.QueueSceneEntry()`，离场前会 `CaptureSceneRuntimeState()`，其中 `CaptureSceneWorldRuntimeState()` 会把 `Tree/Stone/Chest/Crop/Drop/FarmTileManager` 抓进 `sceneWorldSnapshotsByScene`。
  2. 已确认正式读档切场链在 `SaveManager.BeginSceneSwitchLoad()` 里会先 `PersistentPlayerSceneBridge.SuppressSceneWorldRestoreForScene(targetSceneName)`，避免目标场景刚加载时先吃旧 bridge snapshot，再由 `ApplyLoadedSaveData()` 走 authoritative restore。
  3. 已确认 `重新开始` 链不同：
     - `SaveManager.NativeFreshRestartRoutine()` 直接 `SceneManager.LoadSceneAsync("Town")`
     - 没有先 `QueueSceneEntry()`
     - 也没有先 `SuppressSceneWorldRestoreForScene("Town")`
     - 所以 `PersistentPlayerSceneBridge.OnSceneLoaded() -> RebindScene() -> ScheduleSceneWorldRestore()` 仍会尝试把旧 `Town` snapshot 回放到刚加载的 fresh Town 上。
  4. 已确认 `SaveManager.ApplyNativeFreshRuntimeDefaults()` 里虽然会调用 `PersistentPlayerSceneBridge.ResetPersistentRuntimeForFreshStart()`，而后者会清空：
     - `sceneWorldSnapshotsByScene`
     - `sceneWorldSnapshotCapturedTotalDaysByScene`
     - `nativeResidentSnapshotsByScene`
     - `crowdResidentSnapshots`
     但这一清空发生在 `Town` 已加载并可能已被旧 snapshot 回放之后；它不会把当前场景已被回放的树/石头再恢复回 authored baseline。
  5. 已确认 `Primary` 之所以看起来“恢复正常”，是因为 restart 后缓存已被清空；之后再进 `Primary` 时，`ScheduleSceneWorldRestore()` 对它拿不到 snapshot，就只剩 authored scene 默认状态。
- 关键判断：
  1. 当前最可能根因不是 `TreeController/StoneController` 自己不会存，而是 `重新开始` 路径漏掉了“抑制目标场景旧 snapshot 回放”这一步。
  2. 用户会感知成“Primary 恢复了、Town 没恢复”，本质上是：
     - `Town` 走了“当前场景刚加载即回放旧 snapshot”的路径
     - `Primary` 走了“后续离场场景无 snapshot，可直接落回 authored baseline”的路径
  3. 当前最小安全修法优先落在 `SaveManager.NativeFreshRestartRoutine()`：
     - 在 `LoadSceneAsync("Town")` 之前先 `PersistentPlayerSceneBridge.SuppressSceneWorldRestoreForScene(NativeFreshSceneName)`
     - 并在 load fail/null 分支补 `CancelSuppressedSceneWorldRestore(NativeFreshSceneName)`
     - 不先改 bridge 主逻辑，不改 `Tree/Stone` 控制器。
- 验证状态：
  - 已完成：静态代码审查。
  - 未完成：Unity live / packaged 复现。
  - 当前口径：`静态推断成立，live 未终验`。

## 2026-04-18｜真实施工：补收 `0.0.6` 交互白名单、`Primary 001` 提示同源、以及 `20:00` 起手失败半态
- 当前主线目标：
  - 继续按 `0417` 收用户最新 3 条 reopen：
    1. `16:00` 后 `002/003` 应能像普通 NPC 一样聊天
    2. `Primary 001` 等待提示要和 `TownHouseLead` 同源，`002` 要继续贴着 `001` 跟随
    3. `20:00` 回家起手失败后不该卡在“推一下才开始走”的半态
- 本轮实际做成：
  1. `SpringDay1Director.IsPostTutorialExploreWindowActive()` 改为可供交互层读取的真相口。
  2. `NpcInteractionPriorityPolicy` 新增 explore-window 级别的 free-interaction 判定；`NPCDialogueInteractable / NPCInformalChatInteractable` 的 scripted-control 放行，不再只认 `FreeTime`。
  3. `SpringDay1NpcCrowdDirector.ShouldDeferToStoryEscortDirector(...)` 在 explore window 打开后停止对 `001/002` 的 crowd defer。
  4. `TryHandleWorkbenchEscort()` 的等待合同已拉回 `TownHouseLead` 基线：
     - 只暂停 `001`
     - `002` 等待时继续补位
     - 非等待态也改回围绕 `001` 的显式编队跟随
  5. `NPCAutoRoamController.DriveResidentScriptedMoveTo(...)` 现在只在 contract 相同的情况下复用旧 move；若 formal-navigation 起手失败且 owner 不是原本顶层 owner，会回滚这次临时 scripted-control。
  6. `SpringDay1NpcCrowdDirector` 新增 `QueueResidentReturnHomeRetry(...)`：
     - `20:00` 首次起手失败时不再直接冻结成 pending 假接管
     - 已经正式进入 return-home contract 后的丢 drive 才保留 `IsReturningHome=true` 的 pending 重试
  7. `NpcInteractionPriorityPolicyTests.cs` 新增：
     - `FarmingTutorialExploreWindow_ShouldStopSuppressingResidentInformalInteraction`
     - `InformalChatInteractable_ShouldRemainAvailableDuringPostTutorialExploreWindowReturnNavigation`
- 验证结果：
  1. `validate_script` 覆盖：
     - `SpringDay1Director.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - `NPCAutoRoamController.cs`
     - `NpcInteractionPriorityPolicy.cs`
     - `NPCDialogueInteractable.cs`
     - `NPCInformalChatInteractable.cs`
     - `NpcInteractionPriorityPolicyTests.cs`
     均为 `owned_errors=0`
  2. `errors` fresh=`0 error / 0 warning`
  3. `validate_script` assessment 仍是 `unity_validation_pending`，原因是外部 `stale_status`；这轮不能把代码层成立包装成体验已过线
- 当前恢复点：
  1. 下一步优先等用户 fresh 复测这 3 条 reopen。
  2. 如果还有 reopen，再沿 `0417 -> G-05b` 继续，不回到散修。

## 2026-04-18｜真实施工：补收疗伤前 `Home` 门禁与 `Primary 001` 气泡误杀
- 当前主线目标：
  - 继续按 `0417` 收当前打包前 reopen，先把两条最危险的 runtime 口补死：
    1. 疗伤前误进 `Home` 会把 Day1 卡死
    2. `Primary 001` 的 conversation 提示被导演层自己闪掉
- 本轮实际做成：
  1. `SpringDay1Director.SyncPrimaryHomeEntryGate()` 已把 `PrimaryHomeDoor` 正式接回 `HealingAndHP` 门禁：
     - 疗伤未完成时，同时关闭 `SceneTransitionTrigger2D.enabled` 和门体 `Collider2D.enabled`
     - 疗伤完成后才重新放开
  2. `SpringDay1Director` 已补“坏档已落 Home”的恢复链：
     - 若当前在 `Home`
     - 且疗伤未完成
     - 会判定为 `pre-healing home intrusion`
     - 并退回 `PrimaryHomeEntryAnchor`
  3. `ApplyStoryActorRuntimePolicy(...)` 已不再无条件 `HideBubble()`：
     - `conversation-priority` 气泡现在会保留
     - 导演层只继续压掉非 conversation-priority 的泡
  4. `0417.md` 已同步回写：
     - `C-34 / C-35`
     - `I-25 / I-26`
     - `C-01c / C-08a-3 / G-05c`
- 验证结果：
  1. `validate_script`：
     - `SpringDay1Director.cs` => `errors=0 / warnings=3`
     - `SpringDay1DirectorStagingTests.cs` => `errors=0 / warnings=0`
  2. 新增窄测 `4/4 passed`：
     - `SpringDay1DirectorStagingTests.Director_ShouldKeepPrimaryHomeEntryGateClosedBeforeHealingCompletes`
     - `SpringDay1DirectorStagingTests.Director_ShouldAllowPrimaryHomeEntryGateAfterHealingCompletes`
     - `SpringDay1DirectorStagingTests.Director_ShouldRecoverPreHealingHomeIntrusionWhenHomeSceneIsActive`
     - `SpringDay1DirectorStagingTests.Director_ShouldKeepConversationBubbleVisibleDuringStoryActorMode`
  3. 测试过程产生了 Unity Test Framework `TestResults.xml` 清理噪声和一条 `CloudShadowManager` teardown 异常；
     - 当前已删掉 `TestResults.xml`
     - 已 clear console
     - fresh console=`0 error / 0 warning`
- 当前恢复点：
  1. 下一步如果继续，只看用户 fresh live / packaged 对：
     - 疗伤前 `Home` 门禁
     - 疗伤后 `Home` 放开
     - `Primary 001` 气泡不再闪掉
     的真实反馈。
  2. 不回头再造第三套提示链，也不把这条门禁扩成新的场景切换系统。

## 2026-04-18｜支撑子任务：`Tool_002_BatchHierarchy` 自动恢复锁定选择会打出 cross-scene / DDOL 警告
- 当前主线目标：
  - 继续 Day1 打包前收尾；本轮子任务只是修掉用户点名的 own warning，不换主线。
- 本轮实际做成：
  1. 对位用户贴的 warning 栈后，确认触发口就在：
     - `Tool_002_BatchHierarchy.OnEnable() -> LoadPersistedSelection()`
  2. 当前工作树里的 `Tool_002_BatchHierarchy.cs` 已不是旧 `GlobalObjectId` 版本，而是 scene-path + sibling-path 版本；真正会反复触发 warning 的，是窗口打开时自动恢复上次锁定选择这件事本身。
  3. 已去掉 `OnEnable()` 里的自动 `LoadPersistedSelection()`：
     - 工具仍可正常打开
     - 不再在窗口开启时主动重放旧锁定列表
     - 从而不再把 `Town` 里那些带 `DontDestroyOnLoad` 关联的对象顺手拉出来触发 Unity 噪声
- 验证结果：
  1. `validate_script Assets/Editor/Tool_002_BatchHierarchy.cs` => `errors=0 / warnings=2`
  2. clear console 后执行菜单：
     - `Tools/002批量 (Hierarchy窗口)`
  3. fresh console 未再出现用户贴出的那串 cross-scene / `DontDestroyOnLoad` warning
  4. `git diff --check -- Assets/Editor/Tool_002_BatchHierarchy.cs` 通过
- 当前恢复点：
  1. 这条 own warning 已收。
  2. 如果后续用户还看到同类 warning，就要按对象链回到具体 scene/runtime 引用本身查，不再先怀疑 `Tool_002_BatchHierarchy`。

## 2026-04-18｜只读自查：截图里的 `TownSceneRuntimeAnchorReadinessMenu / TownNativeResidentMigrationMenu` 当前不像活跃 own warning
- 当前主线目标：
  - 继续 Day1 打包前收尾；本轮子任务只是用户补了一张模糊截图，要我自查这两个 `Town` editor 工具是不是我这条线当前还留着的 own warning。
- 本轮实际做成：
  1. 静态对位了：
     - `Assets/Editor/Town/TownSceneRuntimeAnchorReadinessMenu.cs`
     - `Assets/Editor/Town/TownNativeResidentMigrationMenu.cs`
  2. 两个脚本当前都已经是“退役后直接返回 deprecated blocker/result”的版本。
  3. 当前源码里这两份文件都只会：
     - `Debug.Log(...)`
     - 写 result json
     不会主动 `Debug.LogWarning(...)`。
  4. 进一步搜了全仓，当前也没有别的脚本在主动调用这两个菜单路径或 blocker code。
- 验证结果：
  1. `validate_script Assets/Editor/Town/TownSceneRuntimeAnchorReadinessMenu.cs` => `errors=0 / warnings=0`
  2. `validate_script Assets/Editor/Town/TownNativeResidentMigrationMenu.cs` => `errors=0 / warnings=0`
  3. 当前 console 也没有这两条 fresh warning 证据。
- 当前判断：
  1. 从现在的代码真相看，这两条不像“仍然存在的活跃 own warning”。
  2. 更像：
     - 旧 console 残留
     - 一次性触发过的退役菜单日志/编译瞬态
     - 或截图时正文被折叠后只剩文件路径
  3. 这轮不建议为了这张模糊截图去盲改这两个退役工具。
- 当前恢复点：
  1. 如果它们再次出现，最值钱的不是猜，而是抓完整 warning 正文。
  2. 在当前证据下，Day1 这条线不把它们算成待修 blocker。

## 2026-04-19｜真实施工：收 `0.0.4` 提前开戏、`001` 日常聊天缺入口、疗伤半径过紧与 walk-away cue 露出
- 当前主线目标：
  - 继续 Day1 打包前收尾；本轮子任务是把用户 fresh reopen 的 4 条体验坏相直接收成最小安全代码闭环：
    1. `0.0.4` 不能在 `001` 还没到位时提前开戏
    2. `001` 日常聊天不能再像“没做内容”
    3. 疗伤靠近 `002` 不能继续近乎贴脸才触发
    4. NPC walk-away 不能再把内部 `reactionCue` 漏给玩家
- 本轮实际做成：
  1. `SpringDay1Director.TryHandleWorkbenchEscort()` 已收回错误 force-ready：
     - 现在必须 `leader ready + playerNearWorkbench`
     - 不再只要玩家先贴近工作台就提前开戏
     - 本地 leader-ready 半径额外放宽到不小于 `0.75f`
  2. 疗伤桥接近判定已补 runtime floor：
     - serialized 仍是 `1.6`
     - 运行时 `GetEffectiveHealingSupportApproachDistance()` 不再小于 `2.4`
     - 避免旧 Inspector 小值把体验重新盖回贴脸触发
  3. `NPCInformalChatInteractable.BootstrapRuntime()` 不再跳过“已有 formal 的 NPC”：
     - `001/002` 这类 formal NPC 只要 roamProfile 有居民闲聊内容，现在也会自动补 informal 入口
  4. `PlayerNpcChatSessionService.RunWalkAwayInterrupt()` 不再把内部 `reactionCue` 直接显示到玩家可见气泡里
  5. `0417.md` 已同步回写：
     - `C-05` 更新为运行时有效疗伤半径
     - 新增 `C-36 / C-37 / C-38`
     - `4.2` 新增 `I-27 / I-28 / I-29`
     - `8.2` 新增 `C-08a-4 / C-08b-1 / C-08b-2 / C-01d / G-05d`
- 验证结果：
  1. `validate_script`：
     - `SpringDay1Director.cs` => `errors=0 / warnings=3`
     - `SpringDay1DirectorStagingTests.cs` => `errors=0 / warnings=0`
     - `PlayerNpcChatSessionService.cs` => `errors=0 / warnings=1`
     - `NPCInformalChatInteractable.cs` => `errors=0 / warnings=1`
  2. `git diff --check` 覆盖 4 个 own 脚本通过
  3. 新增定向测试 `3/3 passed`：
     - `SpringDay1DirectorStagingTests.Director_ShouldNotForceWorkbenchBriefingFromPlayerProximityBeforeLeaderArrives`
     - `SpringDay1DirectorStagingTests.Director_ShouldEnforceHealingSupportApproachRuntimeMinimum`
     - `SpringDay1DirectorStagingTests.InformalChatBootstrap_ShouldAddInformalComponentEvenWhenFormalDialogueExists`
  4. 额外 assembly 级 EditMode 跑通了 discovery，但仓里仍有大量外部旧红；本轮不拿整仓失败冒充 own 回归失败
- 当前恢复点：
  1. 下一步若继续，优先看用户 fresh live / packaged 对：
     - `001` 日常聊天
     - `0.0.4` 入口时机
     - 疗伤靠近 `002`
     - walk-away cue 是否彻底不再露出
  2. 当前 slice 仍是 `fix_primary_home_gate_opens_before_0_0_4`，但实际已经扩展收掉同一条 primary/day1 交互闭环里的 4 个 reopen 点。

## 2026-04-19｜0417 口径校准：代码真相是底板，`0417` 只做参考 / 对账
- 当前主线目标：
  - 把 Day1 打包前收尾口径彻底校准，避免后续再把 `0417` 当成唯一主控板去反向绑代码。
- 本轮实际做成：
  1. 已把 `0417.md` 顶层职责改成：
     - 不是唯一主控板
     - 是参考 / 对账文档
  2. 已明确真值优先级：
     - 当前项目真实代码与资产现场
     - 用户最新裁定
     - 历史迭代语义
     - `0417`
  3. 已把 `Phase 0` 改成“参考板建立”。
  4. 已把 `M-02 / M-03 / M-04` 改成持续维护规则。
  5. 已把 `B-02 / B-03 / C-07 / E-03` 明确压成当前打包前冻结的结构债，不再为了勾表主动扩大施工。
- 当前恢复点：
  1. 后续若继续真实收尾，先看真实代码和 fresh 验证，再用 `0417` 对账。
  2. 不再允许“因为 `0417` 还留空项，就去顺手扩大 Day1 结构重构”。

## 2026-04-19｜收口补记：spring-day1 own 批次已提交
- 当前主线目标：
  - 把这条线已经落地的 runtime / test / 文档 / memory 成果真正收成 git checkpoint，而不是继续挂在 own dirty 里。
- 本轮实际做成：
  1. 已把 spring-day1 own roots 的 staged 内容提交为：
     - `f9496b01 spring-day1: land day1 v3 runtime closeout batch`
  2. 已把收尾遗漏的 `PersistentPlayerSceneBridge.cs` 与 `SaveManagerDay1RestoreContractTests.cs` 再补成第二个小提交：
     - `0d59b8b3 spring-day1: finish persistent restore guard tail`
  3. `git status --short -- spring-day1 own roots` 当前已 clean。
- 额外报实：
  1. 本轮曾尝试走 `Ready-To-Sync`。
  2. 前两次先后撞到：
     - stale `ready-to-sync.lock`
     - `CodexCodeGuard` 未返回 JSON
  3. 这两条都属于流程工具侧噪音，不是这批内容本身的 blocker。
- 当前恢复点：
  1. 这条线现在已从“own dirty 待收口”进入“已提交 checkpoint，等待后续真实体验复测”。
  2. 若下轮继续，应从 `0d59b8b3` 往后看，不再回到这轮之前的脏现场。

## 2026-04-19｜阻塞补记：Town 退役菜单 warning 清理时一度炸出 61/62 条编译错，现已收平
- 当前主线目标：
  - 用户追加报告 `TownSceneRuntimeAnchorReadinessMenu.cs` / `TownNativeResidentMigrationMenu.cs` 的 `CS0162` warning，要直接修掉。
- 本轮实际做成：
  1. 已删除 `TownSceneRuntimeAnchorReadinessMenu.BuildProbeResult()` 中 `return result;` 后的死代码。
  2. 已把 `TownNativeResidentMigrationMenu.Migrate()` 重写回最小退役版本，彻底删掉 `return report;` 后残留的旧迁移函数体。
  3. 过程中一度因为我第一次删坏了 `TownNativeResidentMigrationMenu.cs` 的结构，fresh console 炸出 `61/62` 条语法错；根因已确认是我这次 patch 造成的单文件结构失衡，不是新的系统性爆炸。
  4. 现已重新收平该文件结构。
- 验证结果：
  1. `python scripts/sunset_mcp.py errors --count 40 --output-limit 40` 最新结果：`errors=0 warnings=0`
  2. 两个文件的 `manage_script validate` 当前都返回 `clean`
  3. `validate_script` 对 `TownSceneRuntimeAnchorReadinessMenu.cs` 仍会读到外部 `The referenced script (Unknown) on this Behaviour is missing!`，判定为 `external_red`，不是这两份脚本 own 错
- 当前恢复点：
  1. 这轮 `CS0162` 与中途炸出的 `61/62` 条编译错都已对位并收平。
  2. 如果后续还要继续查红面，下一步该看的是那批外部 `missing script`，不是继续盯这两个 Town 退役菜单。

## 2026-04-19｜Primary Home 门禁语义改口：锁到 `0.0.5` 才放开
- 当前主线目标：
  - 用户最新裁定把 `Home` 入口从“`0.0.3` 结束后可进”进一步改成“整个 `0.0.4` 都锁住，只有进入 `0.0.5` 才能进家”。
- 本轮实际做成：
  1. `SpringDay1Director.ShouldAllowPrimaryHomeEntry()` 已从：
     - `CurrentPhase >= WorkbenchFlashback`
     改成：
     - `CurrentPhase >= FarmingTutorial`
  2. 这意味着：
     - `HealingAndHP`
     - `WorkbenchFlashback`
     两段都继续锁住 `PrimaryHomeDoor`
     - 只有正式进到 `0.0.5`
     才重新放行。
  3. `0417.md` 已同步改口，不再保留“疗伤后就开门”的旧真值。
- 验证结果：
  1. `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
     - `owned_errors=0`
     - native validation=`warning(3)`，仍是既有性能类提示
  2. `validate_script Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - `owned_errors=0`
     - native validation=`clean`
  3. fresh console：
     - `errors=0 warnings=0`
  4. `git diff --check` 覆盖这两份文件通过
- 当前恢复点：
  1. 这刀已经把门禁合同改成用户最新语义。
  2. 如果下一轮要继续，不该再讨论“疗伤后能不能进家”，而是直接按“`0.0.5` 才开”继续验 live / packaged。

## 2026-04-23｜shared-root 保本上传补记：docs/memory/manifest 已归仓，代码批次停在工具与禁吞 blocker
- 当前主线目标：
  - 不继续开发 Day1，只把 `spring-day1` 当前 clearly-own 成果按 shared-root 白名单安全归仓并 push。
- 本轮实际完成：
  1. 已执行 `Begin-Slice -> Ready-To-Sync -> sync` 的安全批次：
     - `.codex/threads/Sunset/spring-day1/memory_0.md`
     - `.codex/threads/Sunset/Day1-V3/memory_0.md`
     - `.kiro/specs/900_开篇/spring-day1-implementation/*` 本轮 touched memory / `0417.md`
     - `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
  2. 已生成并 push：
     - commit=`2026.04.23_spring-day1_01`
     - sha=`8f1909da`
  3. 已继续只读核对第二批代码根：
     - `Assets/Editor/Story`
     - `Assets/YYY_Scripts/Story/Directing`
     - `Assets/YYY_Scripts/Story/Managers`
     - `Assets/YYY_Tests/Editor`
- 当前 blocker：
  1. `Assets/Editor/Story + Assets/YYY_Scripts/Story/Directing`
     - `git-safe-sync preflight` 被 `CodexCodeGuard 未返回 JSON` 卡住，当前无法合法进入 sync。
  2. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
     - 同根还混着 prompt 明确禁止吞的 `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
  3. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 同根还混着 prompt 明确禁止吞的：
       - `Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
       - `Assets/YYY_Tests/Editor/SaveManagerDefaultSlotContractTests.cs`
       - `Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
       - `Assets/YYY_Tests/Editor/WorkbenchInventoryRefreshContractTests.cs`
     - 以及额外 unrelated：
       - `Assets/YYY_Tests/Editor/ChestPlacementGridTests.cs`
- 当前恢复点：
  1. docs/memory/manifest 这一批已经安全归仓，不需要返工。
  2. 后续若要继续收第二批，必须先解决 `CodexCodeGuard` 工具 blocker，或由治理位裁定如何处理同根禁吞文件。
