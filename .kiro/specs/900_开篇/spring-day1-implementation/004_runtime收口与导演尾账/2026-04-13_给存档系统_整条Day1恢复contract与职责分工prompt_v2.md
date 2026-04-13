## 当前已接受的基线

这次不是让你自己去推断 `Day1` 该怎么恢复。

当前正确分工是：

- `spring-day1` 负责定义整条 `Day1` 的剧情/时间/UI/NPC/runtime 恢复语义，以及在恢复后该由 day1 自己重建什么、强制清理什么、重新接管什么。
- `存档系统` 负责按这份 contract 做保存、读取、重新开始、恢复时的接线与回归测试。

所以你这轮不是“自己想一套 Day1 语义”，而是：

`接住 spring-day1 已定义的整条 Day1 恢复 contract，把 save/load/restart 做成不会破坏这条 contract 的恢复入口。`

## 当前唯一主刀

只收：

`整条 Day1（从开场 CrashAndMeet 到 DayEnd）在 save / load / restart / restore 后的恢复 contract 与恢复接线。`

不要扩到：

- PromptOverlay 视觉布局
- 001/002 的剧情站位逻辑
- NPC 导航/避障/runtime staging 自身
- Town / Primary / Home 的普通剧情导演逻辑

## 这轮为什么必须按“整条 Day1”来做

用户已经明确指出：

1. `重新开始` 已经会出问题
2. `读档` 只会更高风险
3. 问题不只是晚饭，而是 `Day1` 自己的语义没有完整传到 save/load
4. 这里面包含：
   - 剧情 phase
   - 对话消费状态
   - UI 状态
   - 时间与门禁
   - 各种恢复时要刷新的 runtime 状态

所以这轮禁止把 scope 再缩成“只修晚段”。

## 现在你要按这份 contract 工作

### A. 总原则

恢复目标不是“把所有运行时临时状态原样抄回去”，而是：

`恢复到该 phase 在玩家视角下应该看到的 canonical state。`

也就是说：

- 该清掉的临时 UI 要清掉
- 该恢复的剧情消费态要恢复
- 该恢复的时间/phase 要恢复
- 该交给 day1 runtime 重新接管的，要通过 day1 hook 重新接管

不要把下面这些临时态当成“必须原样持久化”的真值：

- 对话框当前开到第几帧的 UI 画面
- PromptOverlay 当前 alpha / 某条 bridge prompt 的瞬时文本
- story actor 的某一帧临时站位
- 某个 modal panel 正好打开的瞬时壳体状态

### B. 整条 Day1 的 canonical 恢复语义

#### 1. 开场到进村

- `CrashAndMeet`
- `EnterVillage`

恢复后必须满足：

- phase 正确
- completed sequence 正确
- 如果当前仍处于剧情推进链，就由 day1 runtime 继续接管
- 不允许因为 load/restart 留下 stale dialogue / stale prompt / stale modal

#### 2. 疗伤 / 工作台 / 农田教学

- `HealingAndHP`
- `WorkbenchFlashback`
- `FarmingTutorial`

恢复后必须满足：

- 教学阶段门禁正确
- 工作台/提示语义正确
- `+` 调时间不能直接把后续整天剧情一起跳完
- 在 `005` 前，Day1 的时间夹取/上限逻辑要保持成立

#### 3. 傍晚过渡

- day1 初始时间 = `09:00`
- 在用户完成进入晚饭前必要前置之前，时间逻辑不能因为 debug 跳时直接把整条后续剧情自动吃掉
- 在完成晚饭入口 unlock 前，相关 guardrail 仍需成立

#### 4. 晚饭冲突

- `DinnerConflict`
- 晚饭时间语义 = `18:00` 开始
- 无论是主动找村长触发，还是 `+` 快进触发，恢复后都必须能继续接管，不能卡在 `0.0.6`

注意：

- `001/002` 的剧情站位与强制就位是 `spring-day1` own，不是你这边自己修
- 但你必须保证：恢复到 `DinnerConflict` 时，不会因为 save/load 没有把正确语义交回 day1，而让晚饭入口永远不继续

#### 5. 归途提醒

- `ReturnAndReminder`
- 时间语义 = `19:00`
- 恢复后要能继续 reminder 对话/提示链

#### 6. 自由时段

- `FreeTime`
- 时间语义 = `19:30`
- 恢复后要能区分：
  - free-time intro 尚未完成
  - free-time intro 已完成
  - 夜间 warning / 午夜 warning / final call 的压力态

恢复后任务清单 / prompt / 睡觉可交互状态要重新正确建立。

#### 7. 夜间 NPC / 睡觉 / DayEnd

- `20:00` NPC 开始往 anchor 回
- `21:00` 若仍未到位，则强制 snap 到 anchor 并停住
- 第二天开始时，NPC 应从 anchor 状态出发再恢复漫游
- `Day1` 仅在超过凌晨 `2:00` 还未睡时，强制把玩家送到 `Town` 床边并走一次睡觉转场
- 睡觉后任务列表应先展示完成语义，再进入第二天后收起

这些语义不一定都要由你存具体瞬时值，但恢复后不能把它们破坏掉。

### C. 你必须恢复/接线的最小数据与 hook 语义

#### 1. 必须准确恢复的 canonical 数据

至少核清并处理：

- 当前 scene / scene context
- `StoryPhase`
- `completedDialogueSequenceIds`
- Day1 director 的关键私有推进态
- 当前受管控时钟
- day1 是否已经进入 free-time / day-end

#### 2. 恢复时必须清理或归一化的 transient 状态

至少包括：

- Dialogue active UI
- PromptOverlay manual/bridge 残留
- Package / Box / Workbench / Prompt 等临时 modal 残留
- time pause / input gate 残留
- 任何会导致“看起来像卡死，其实是 stale UI/blocked state”的旧壳状态

#### 3. 恢复后必须交回给 day1 runtime 的 re-entry

这是重点。

你不能只把 phase 写回去就结束。

必须明确在 load/restart 后：

- 哪些 phase 需要通知 day1 自己重新建 prompt/task/runtime gate
- 哪些 phase 需要通知 day1 重新接剧情入口
- 哪些 phase 需要通知 day1 重新建立 sleep/night-pressure 状态

如果 day1 需要补新的公开 re-entry hook，你应和 day1 配合接这条 hook，而不是自己猜一套 runtime 流程。

## 你这轮必须产出的东西

1. 一份清楚的人话结论：
   - 哪些是存档系统 own
   - 哪些是 spring-day1 own
   - 你这轮具体接了哪部分
2. 代码层恢复接线
3. 回归测试
4. 明确哪些问题仍需 spring-day1 继续修

## 你这轮绝对不要做的事

1. 不要自己去修 `001/002` 的剧情站位
2. 不要自己去改 PromptOverlay 的视觉布局
3. 不要自己去改 Town 晚饭开场 staging
4. 不要把 day1 runtime own 问题包装成“存档已闭环”

## 建议测试矩阵

至少覆盖这些存档/恢复场景：

1. 开场后、进村前存档再读档
2. `Primary` 承接段存档再读档
3. 农田教学中存档再读档
4. 傍晚 unlock 前后用 `+` 调时，再存档/读档
5. `DinnerConflict` 入口存档/读档
6. `ReturnAndReminder` 存档/读档
7. `FreeTime` intro 前 / intro 后存档/读档
8. 超过 `2:00` 的 forced sleep 前后恢复
9. `重新开始` 到 Day1 晚段的恢复初始化

## 完成定义

你只有在下面都成立时，才能 claim 这轮过线：

1. 不是只修晚段，而是整条 Day1 恢复 contract 已收清
2. save/load/restart 不再把 stale UI / stale dialogue / stale pause 栈带回来
3. 读档不会把 `Day1` 卡在一个 phase 名字对了、但 runtime 不继续的假恢复态
4. 你清楚标出仍需 `spring-day1` own 继续收的内容

## 固定回执格式

先给 `A1 保底六点卡`：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

再给 `A2 用户补充层`：

- 你接住了哪些 Day1 恢复语义
- 你没有越权接哪些 runtime 问题
- 这轮最核心的判断
- 最薄弱点
- 自评

最后才给 `B 技术审计层`

[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 旧线程不用废弃重开；但最晚必须在“下一次真实继续施工前”补这次 `Begin-Slice`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：
- 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 如果还没跑，原因是什么
- 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住
