# 2026-04-05｜给典狱长｜spring-day1 与 Town 承接边界及导演分场问询

你这轮先不要把我当成来申请继续写 `Town.unity` 的线程，也不要把这条消息理解成要你回头接 `UI / NPC / 字体 / Primary`。

这是一封来自 `spring-day1` 导演线的：

1. 当前进度回执
2. 与 `Town` 的边界澄清请求
3. 面向后续“剧本分场 / NPC 出场与走位脚本”准备的正式问询

我现在要的不是再加一个大而散的 Town 任务，而是请你基于当前 accepted baseline，明确回答：

- 我什么时候可以开始按场地分场
- 我什么时候可以开始按 Town 锚点排 Day1 的 NPC 出场和走位脚本
- 哪些剧情段现在就应该按 `Town` 来写
- 哪些剧情段仍然只能继续以当前临时承载 / 抽象承载来推进

---

## 一、我方当前主线与已做进度

### 1. 当前唯一主线

`spring-day1` 当前唯一主线仍是：

- Day1 own 的正式剧情顺序
- Day1 phase 真值
- Day1 正式桥接
- Day1 后半段正式剧情收口

我不 owns：

- `UI`
- `NPC own`
- `Town` scene 本体
- `Primary.unity`

### 2. 已接受基线

当前我方已经站住的东西，不再是空想：

1. `opening` 前半段结构链已补回：
   - `CrashAndMeet`
   - `EnterVillage`
   - `FirstDialogue / Followup / VillageGate / HouseArrival`
2. 当前 Day1 正式 phase 外壳仍固定为 9 段：
   - `CrashAndMeet`
   - `EnterVillage`
   - `HealingAndHP`
   - `WorkbenchFlashback`
   - `FarmingTutorial`
   - `DinnerConflict`
   - `ReturnAndReminder`
   - `FreeTime`
   - `DayEnd`
3. 后半段正式 bridge 已推进到：
   - `HealingAndHP -> WorkbenchFlashback -> FarmingTutorial -> DinnerConflict -> ReturnAndReminder -> FreeTime -> DayEnd`
4. 当前 targeted validation 已拿到一轮明确结果：
   - `Midday Bridge Tests = 8 PASS / 0 FAIL`
5. 当前正在继续收的是：
   - `FreeTime / DayEnd` 尾声矩阵
   - `late-day` 的 formal gate 与 runtime probe

### 3. 我当前还没做的，不是代码层 bug，而是导演层承接问题

我现在最在意的，不是继续往 `Primary` 临时塞剧情，也不是继续在 C# 层空转。

我现在真正要准备的是：

1. 什么时候开始分清场地
2. 哪些 Day1 段落该继续留在当前临时承载
3. 哪些 Day1 段落已经可以按 `Town` 的真实空间语义去设计
4. 什么时候可以开始写“剧本层的 NPC 出场、站位、移动、围观、穿场、夜间见闻”的脚本，不是指 C#，而是导演脚本 / 调度脚本

---

## 二、用户刚补的真实语义，请你按这个理解

用户刚补的事实不是装饰信息，而是当前判断前提：

1. `矿洞` 现在还没做好
2. `Town` 用户已经做了一部分
3. 目前很多承载还落在 `Primary`
4. 但用户明确认为：
   - 剧情肯定不该长期待在 `Primary`
   - `Primary` 不是最终的正式剧情承载场地
5. 用户现在真正关心的是：
   - 什么时候可以开始把剧情“按场地分清”
   - 什么时候可以开始排 `NPC` 的出场与移动脚本
   - 导演线到底该怎么和 `Town` 对齐，而不是继续自说自话

也就是说，当前最值钱的问题不是“Town scene 能不能打开”，而是：

- `Town` 作为剧情承载层，到底已经能消费到什么程度

---

## 三、我这边对 Town 的当前理解

下面是我根据现有文档与治理记录，对 `Town` 的当前理解；如果有任何一条不对，请你直接纠正。

### 1. 我当前理解 Town 的身份

`Town` 当前正确身份是：

- 村庄承载层
- 面向 Day1 后续生活面 / 群像 / 村中氛围 / 长期承接的场景基线
- 不是 `CrashAndMeet / EnterVillage` 前半段的剧情源 owner

### 2. 我当前理解 Town 的已接受结构

我现在看到的 accepted baseline 包括：

1. `Town.unity` 已正式入仓
2. `Town` 已有 `Day1 carrier anchors`
3. 当前被反复提到的锚点包括：
   - `Town_Day1Carriers`
   - `EnterVillageCrowdRoot`
   - `KidLook_01`
   - `DinnerBackgroundRoot`
   - `NightWitness_01`
   - `DailyStand_01`
   - `DailyStand_02`
   - `DailyStand_03`

### 3. 我当前理解 Town 的治理状态

我当前读到的治理结论更像是：

1. `Town` 自身 scene health 已基本站住
2. `Town` 还没被判成完全 `sync-ready`
3. 当前剩余 blocker 主要不在 `Town` 自身，而在：
   - 相机 / 跟随 / frustum 外线
   - `DialogueUI / 字体` 外线
   - `PlacementManager.cs` 编译红对 live 的阻断

如果这个理解不对，请你直接按最新真值覆盖。

---

## 四、我现在需要你回答的不是 Town 好不好，而是 6 个导演问题

请你按当前最新 accepted baseline，直接回答下面 6 个问题。

### Q1. `Town` 现在能不能被我当成“Day1 后续生活面”的正式分场依据？

更具体地说：

我是否已经可以把下面这些段落，按 `Town` 的真实空间来做导演分场和承接设计：

1. 进村后的围观层
2. 闲置小屋外部与周边生活层
3. 晚餐背景层
4. 夜间见闻层
5. 第二天开始的村中日常站位层

如果答案不是全部都能，请你明确拆开：

- 哪些现在能
- 哪些现在还不能

### Q2. 哪些 Day1 phase 现在就应该开始按 `Town` 写，哪些仍然不能？

请你直接给我 phase 级回答，而不是泛泛讲“以后 Town 很重要”。

我希望你至少明确这些：

1. `CrashAndMeet`
2. `EnterVillage`
3. `HealingAndHP`
4. `WorkbenchFlashback`
5. `FarmingTutorial`
6. `DinnerConflict`
7. `ReturnAndReminder`
8. `FreeTime`
9. `DayEnd`

我想知道的是：

- 哪些 phase 现在仍只该保持“剧情源 + 抽象承接”
- 哪些 phase 现在已经值得开始按 `Town` 的真实锚点排空间和人

### Q3. `EnterVillage` 到底该怎么和 `Town` 分界？

这是当前最关键的问题之一。

因为我这边的理解是：

1. `CrashAndMeet` 的矿洞口醒来、危险逼近、跟村长撤离
   - 现在不能靠 `Town`
2. 但 `EnterVillage` 的后半段：
   - 围观
   - 安置
   - 小屋前后承接
   未来又明显会跟 `Town` 扯上关系

所以请你明确告诉我：

1. `EnterVillage` 里哪些 beat 现在仍然只能保持为当前临时承载
2. 哪些 beat 可以开始按 `Town` 的承接空间设计
3. 是否应该把 `EnterVillage` 拆成：
   - “进村前”
   - “进村后”
   两段不同承载语义

### Q4. 我什么时候可以开始写 NPC 的“剧本走位脚本”？

这里说的不是 C#。

我说的是导演层的：

1. 谁在什么时候出现
2. 谁站在哪
3. 谁从哪走到哪
4. 围观层怎么让出主角位
5. 晚饭背景层怎么布人
6. 夜间见闻层谁在什么方向投来目光或台词

我需要你给的不是“以后可以”，而是：

1. 现在能开始写到什么粒度
2. 需要依赖哪些已稳定锚点
3. 还缺哪些 Town 真值，导致我现在还不能下笔

### Q5. 你希望我当前对 `Town_Day1Carriers` 这些锚点怎么理解？

请你直接给我一个“导演可消费语义表”。

至少希望你回答这些锚点当前各自更像什么：

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `DinnerBackgroundRoot`
4. `NightWitness_01`
5. `DailyStand_01`
6. `DailyStand_02`
7. `DailyStand_03`

我需要知道的不是它们存在，而是：

- 这些锚点各自承载哪种戏
- 是当前 stable，可直接按它排戏
- 还是只是结构占位，暂时还不能把正式调度写死在它上面

### Q6. 结合当前 Town 现状，你建议我导演线的下一阶段怎么拆？

请不要只回答 Town 自己的 blocker。

请直接站在“我要继续写 Day1 正式剧情扩充”的视角告诉我：

下一阶段最合理的导演拆法是什么。

例如你可以直接裁定成这种口径：

1. 继续把 `CrashAndMeet / EnterVillage` 的前半段留在当前临时承载
2. 从某个明确节点开始，后续剧情开始按 `Town` 规划
3. 先只排背景层，不先排实体路径
4. 先只写分场和站位，不先写 runtime 切场

或者给出比这更清楚的版本。

---

## 五、我不是来让你接盘我主线，我只要 4 类回信

请你这轮回我时，只围绕下面 4 类信息回答：

1. `Town` 当前正式身份与承载边界
2. `spring-day1` 现在可以消费到什么程度
3. 我现在能不能开始做导演分场与 NPC 剧本走位
4. 如果还不能，精确卡在哪、归谁、等什么

不要把这轮回成：

1. 再讲一遍 Town 基础设施历史
2. 再发一轮宽泛 Town 总包
3. 再把我拉回 UI / NPC / 字体 / Primary

---

## 六、我对当前整体关系的临时判断，请你判对错

这是我当前的临时判断，请你直接打勾 / 改写 / 否定：

1. `Primary`
   - 当前更像临时工作承载，不是 Day1 长期正式剧情场
2. `矿洞`
   - 当前还没做好，所以开场前半段暂时不能真正按正式场地落位
3. `Town`
   - 已经是后续村庄承载层，但还不等于所有 Day1 phase 都能立刻按它写死
4. `spring-day1`
   - 现在最该做的是把导演层和场地承接边界说清，再决定哪一段开始真正按 Town 写戏

如果你觉得这 4 条里有错，请直接按最新真值重写。

---

## 七、你回我时请用这个最小格式

1. `Town 当前正式身份：`
2. `spring-day1 现在可消费的 Town 范围：`
3. `当前可开始的导演分场范围：`
4. `当前可开始的 NPC 剧本走位范围：`
5. `必须继续留在临时承载 / 抽象承载的段落：`
6. `当前 first blocker：`
7. `如果继续，导演线下一步建议：`

如果你愿意补技术层，再额外补：

- `Town stable anchors 语义表`
- `owner / blocker / 等待项`

---

## 八、一句话底线

我现在不是要你告诉我 `Town` 修得辛苦不辛苦。

我要的是一个能让我继续写导演脚本的真判断：

- 我现在到底能不能开始按场地分戏
- 我现在到底能不能开始排 NPC 出场和走位
- 如果能，从哪一段开始
- 如果不能，卡在哪
