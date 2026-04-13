## 当前已接受的基线

下面这些已经不是当前第一主刀：

1. resident scripted move 已重新接回 shared avoidance / blocked abort
2. Town resident runtime registry 丢失时，`SpringDay1NpcCrowdDirector` 已补 `Town rebind`
3. editor-only probe / late-phase validation jump 已经存在

这些都不等于 Day1 可以打包。

## 当前唯一主刀

`把整条 Day1 从开场到睡觉的 own runtime、own UI、own staging、own re-entry 闭环一次性尽量收平，直到用户可以继续往打包验证走。`

不要再缩成某一个小刀口，也不要只盯“晚段”。

## 这轮必须吃下的完整 own 范围

你自己的职责是：

- 整条 Day1 的剧情运行语义
- Day1 的时间与门禁
- Day1 own 的 task/prompt/dialogue 可见状态
- Day1 own 的剧情 actor staging / 强制就位 / 自愈重入
- Day1 own 的 NPC 晚间行为与 day-end 过渡
- 给 save thread 的完整恢复 contract

你不要再把“恢复 contract 没说全”留给存档线程自己猜。

## 这轮必须覆盖的用户真实需求

### 1. 整天时间与节奏语义

- Day1 开始时间应为 `09:00`
- 在完成相应前置前，不能因为 `+` 调时间把整条后续剧情自动跑完
- 在 `005` 前，时间上限 / guardrail 必须生效，至少不能让流程被直接跳坏
- 晚饭应在 `18:00`
- 归途提醒应在 `19:00`
- 自由时段应在 `19:30`

### 2. 晚饭开场当前 P0 blocker

这是当前最急、最硬的打包阻塞：

- 晚饭无论是玩家主动找村长触发
- 还是 `+` 快进触发

现在都会卡在 `0.0.6 晚餐冲突`，剧情不继续。

你必须自己查清并修掉：

- 不是只写分析
- 不是只写可能原因
- 而是把入口接管真正打通

### 3. 001 / 002 的剧情就位与朝向

用户已明确裁定：

- 剧情一旦开始
- `001/002` 就必须在必要入口被强制拉到正确位置和朝向
- 这只能发生在“该做和必要做的地方”
- 绝对不允许回到全局过渡管理乱抢控制的坏模式

所以你必须做成：

- fresh 进入晚饭时，必要时强制就位
- 重新开始恢复到晚饭时，必要时强制就位
- 读档恢复到晚饭时，必要时强制就位

### 4. Prompt / 任务 UI own 语义

这轮只收你 own 的部分：

- `PromptOverlay` 里你新增的 bridge prompt 要和任务卡同根，并放在任务卡下面
- 不要再飘在任务卡上方抢画面
- 不要改坏已经过线的任务卡主体
- 任务清单 / prompt / dialogue 在不同 phase、对话、modal、重入时的显隐语义要收平

### 5. 夜晚 NPC 与睡觉语义

- `20:00` NPC 自发往 anchor 回
- `21:00` 没回到 anchor 的强制 snap 到 anchor，并停住
- 第二天开始时从 anchor 出发，再恢复漫游
- Day1 仅在超过 `2:00` 还没睡时，强制把玩家送到 `Town` 床边，并走一次睡觉转场
- 睡觉任务列表要先显示完成语义，再在进入第二天后正确收起

### 6. 重新开始 / 恢复重入 / 存档协作

你必须先把自己的语义说全，而不是等 save thread 自己猜。

你要明确输出：

- 每个 Day1 phase 在恢复后，day1 runtime 要重新做什么
- 哪些 UI / prompt / dialogue / time gate 需要 day1 自己强制归一
- 哪些 actor staging 必须由 day1 自己在 re-entry 时强制修正

这份 contract 是你 own 的交付物之一。

## 这轮建议执行顺序

### P0 先收打包阻塞

1. 晚饭入口卡死
2. `001/002` 晚饭/重开/恢复时的就位与朝向
3. 给 save thread 的整条 Day1 恢复 contract

### P1 再收整天节奏与夜晚闭环

4. 时间 guardrail
5. free-time / reminder / sleep 任务清单与 prompt 语义
6. `20:00 -> 21:00 -> 次日` 的 NPC anchor 逻辑

### P2 最后收视觉位置

7. bridge prompt 下移到任务卡下面

## 明确禁止的漂移

- 不要去修通用 save/load 系统
- 不要去修 package panel / UI 主系统壳体
- 不要再把锅甩给导航
- 不要只修一半就停在“等用户再试”
- 不要把结构 checkpoint 说成整条 Day1 已打包可用

## 你这轮必须产出的东西

1. 代码修复
2. 回归测试
3. 一份给 save thread 的 Day1 全量恢复 contract
4. 一份清楚的人话汇报，让用户知道：
   - 这轮修了什么
   - 还剩什么
   - 离“可以打包”还有多远

## 最低测试矩阵

至少要自测/补测试覆盖：

1. fresh 正常跑到晚饭
2. 主动找村长触发晚饭
3. `+` 快进触发晚饭
4. 重新开始后进入晚饭
5. `DinnerConflict -> ReturnAndReminder -> FreeTime`
6. free-time 睡觉
7. 超过 `2:00` forced sleep
8. NPC `20:00` 回家 / `21:00` snap / 次日从 anchor 起步
9. Prompt/task 在对话、modal、恢复重入时的显隐

## 完成定义

只有下面这些都站住，才算你这轮真的往“可打包”推进：

1. 晚饭入口不再卡死
2. `001/002` 在必要入口会强制就位和朝向正确
3. `+` 调时间不再把整条 Day1 跳坏
4. bridge prompt 已落到任务卡下面
5. 晚间 NPC / sleep / free-time 链闭环
6. 给 save thread 的整条 Day1 恢复 contract 已明确交付

## 固定回执格式

先给 `A1 保底六点卡`

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

再给 `A2 用户补充层`

- 这轮最核心的判断
- 为什么这样判断
- 这轮最薄弱点
- 离可打包还有多远
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
