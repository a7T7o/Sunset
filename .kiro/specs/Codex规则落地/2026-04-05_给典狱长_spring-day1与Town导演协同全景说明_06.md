# 2026-04-05｜给典狱长｜spring-day1 与 Town 导演协同全景说明

这不是一张“下一刀命令单”，也不是让你马上替我接盘 `spring-day1`。

这是一份给典狱长的：

1. 当前全景情况说明
2. 线程间已分出的工作面说明
3. 你这边仍值得继续推进的治理工作面说明

目的只有一个：

- 让你知道 `spring-day1` 这条导演线现在在做什么、已经做到哪
- 让你知道哪些内容已经分给 `NPC / UI / Town 外线`
- 让你判断自己接下来还要继续盯哪些治理位与 Town 剩余 blocker

---

## 一、当前全景总状态

### 1. `spring-day1` 当前真实位置

`spring-day1` 现在已经不在“只会跑最小骨架”的状态了。

当前已经站住的，是两层：

1. Day1 own 的正式 phase 外壳已经稳定
2. 导演线开始进入“按场地分戏、按群像分层、按 Town 承接后续生活面”的阶段

### 2. 当前已稳定的 Day1 phase 外壳

目前仍固定为 9 段：

1. `CrashAndMeet`
2. `EnterVillage`
3. `HealingAndHP`
4. `WorkbenchFlashback`
5. `FarmingTutorial`
6. `DinnerConflict`
7. `ReturnAndReminder`
8. `FreeTime`
9. `DayEnd`

### 3. 当前已做成的阶段性成果

#### 3.1 opening 前半段

已经不是口头说“以后要补”，而是结构上已经补回：

1. `CrashAndMeet`
2. `EnterVillage`
3. `FirstDialogue / Followup / VillageGate / HouseArrival`

也就是说：

- 醒来
- 语言错位
- 危险逼近
- 进村
- 围观
- 小屋安置

这条前半段至少在剧情源和阶段划分上已经被正式拉回当前主线。

#### 3.2 后半段正式 bridge

当前后半段已经推进到：

- `HealingAndHP -> WorkbenchFlashback -> FarmingTutorial -> DinnerConflict -> ReturnAndReminder -> FreeTime -> DayEnd`

并且不是只靠口头 claim，而是有 targeted validation 站住：

- `Midday Bridge Tests = 8 PASS / 0 FAIL`

#### 3.3 尾声矩阵

`ReturnAndReminder / FreeTime / DayEnd` 现在已经不再是空骨架。

目前导演线上已经补出：

1. `FreeTime intro -> intro complete -> night pressure -> DayEnd` 的 formal gate
2. `PromptOverlay inactive` 那条用户 live 崩点已收口
3. 后半段 targeted validation 已通过一轮 fresh

所以当前导演线已经从“能不能跑”推进到“怎么继续做正式分场和群像调度”。

---

## 二、用户这轮真正把问题改写成了什么

用户最新关注点已经明显改变。

现在用户不再只盯：

- 某个 UI bug
- 某个工作台表现
- 某个单点红错

用户现在明确在问：

1. 什么时候可以开始把剧情按场地分清
2. 什么时候可以开始排 NPC 的剧本层移动、站位、围观、夜间见闻
3. `Primary` 现在只是临时承载，不应再被当成正式长期剧情场地
4. `Town` 既然已经做了一部分，那么导演线什么时候可以真正开始消费它

所以现在的主问题不是“Town 能不能打开”，而是：

- `Town` 作为村庄承载层，已经可以被导演线消费到什么程度

---

## 三、当前最新真值：导演线已经拿到什么，没拿到什么

你刚刚已经给了一轮关键真值，我这里把它收成全景说明，方便你后续继续治理。

### 1. 已经拿到的真值

#### 1.1 `Town` 的正式身份

当前 `Town` 应被视为：

- 村庄承载层
- 后续生活面、背景层、群像层、夜间见闻层、日常站位层的场地基线

它不是：

- `CrashAndMeet / EnterVillage` 前半段的剧情源 owner

#### 1.2 `spring-day1` 现在可消费的 Town 范围

当前导演线已经可以消费的是：

- 导演层
- 分场层
- 背景调度层

当前已经可以按 `Town` 去写的，是：

1. `EnterVillage` 进村后的围观层
2. 小屋外部周边生活感
3. `DinnerConflict` 的晚餐背景层
4. `FreeTime / DayEnd` 的夜间见闻层
5. 第二天开始的村中日常站位层

#### 1.3 现在能开始做什么

现在就可以开始，而且建议立刻开始的是：

- 导演分场
- 群像站位
- 视线关系
- 围观层
- 背景层密度

但这仍然只是：

- 导演层可开始

不是：

- runtime 全闭环

### 2. 还没拿到的真值

当前还不能假设 `Town` 已经能稳定承接的是：

1. 完整切场
2. 精确路径
3. 相机联动
4. 最终 runtime 触发
5. tile 级 nav route
6. 秒级移动脚本

也就是说，导演线现在可以消费 `Town` 的锚点语义，但不能消费“Town live 已彻底闭环”这个前提。

---

## 四、现在已经明确分出去的内容

当前不是所有内容都还挤在 `spring-day1` 身上。

### 1. 分给 `UI` 的

`UI` 当前继续 owns：

1. 玩家面 `UI / UE`
2. DialogueUI / continue / 任务栏 / Prompt 的玩家可见表现
3. 工作台正式面、悬浮面、任务文字链、字体显示链
4. 其他直接面向玩家的结果层

导演线当前不再 owns 这些。

### 2. 分给 `NPC` 的

`NPC` 当前继续 owns：

1. NPC own 气泡
2. casual / ambient / pair bubble 底座
3. NPC 会话状态、speaking-owner、formal/casual 让位的 NPC 本体层
4. 之后按导演脚本去承接群像层站位与出场逻辑

导演线不再 owns：

- NPC own runtime bubble
- 会话状态机
- pair bubble 本体

### 3. 仍留在 `spring-day1` 的

我当前仍 owns：

1. Day1 phase 真值
2. 正式剧情顺序
3. 哪一段该留在临时承载
4. 哪一段开始可按 `Town` 分场
5. NPC 在剧本层什么时候出现、承担哪种戏

### 4. 仍在 `Town` / 外线治理面里的

当前 `Town` 还没有 `sync-ready`。

它现在剩下的，不是导演脚本层问题，而是治理层 / 外线问题：

1. 相机 / frustum
2. `DialogueUI / 字体链`
3. `PlacementManager.cs` 编译红

这几件事不该再被误算成 `spring-day1` own。

---

## 五、当前导演线已经可以开始做的，不需要再等你

这部分我单独说清，是为了让你知道：

- 你接下来继续治理 Town，并不是我继续开写的前置条件

当前导演线已经可以开始做的是：

### 1. `EnterVillage` 拆分

现在应正式拆成：

1. 进村前
2. 进村后

其中：

- 进村前继续临时承载
- 进村后开始按 `Town` 分场

### 2. 后续可按 `Town` 分场的段落

当前已经可以开始按 `Town` 写的是：

1. `EnterVillage` 的 post-entry crowd
2. `DinnerConflict` 的背景承接
3. `ReturnAndReminder` 的回村氛围承接
4. `FreeTime` 的夜间村庄层
5. `DayEnd` 的夜间收束与次日站位预示

### 3. 现在可以开始写的 NPC 剧本层内容

现在可以开始写的，不是精确 runtime 逻辑，而是：

1. 谁出现
2. 占哪个锚点
3. 朝向谁
4. 谁围观
5. 谁让位
6. 谁构成晚餐背景层
7. 谁承担夜间见闻位

---

## 六、你现在仍然值得继续推进的，不是导演线，而是 Town 总治理

这部分是我这份说明最想让你继续盯住的。

### 1. 你现在不需要替我决定下一句台词写什么

导演线现在已经可以自己推进：

- 分场
- 群像层
- 剧本走位

### 2. 你现在最值钱的继续推进点，是 Town 总治理

你现在继续往下做，最该盯的是：

1. `Town` 什么时候从“导演层可消费”推进到“更接近 sync-ready”
2. 哪些外线 blocker 该继续往下压
3. 哪些线程继续该施工
4. 哪些线程其实该停

### 3. 你现在继续推进最有价值的面

#### 3.1 `Town` 剩余 blocker 治理

当前仍值得你继续总闸盯住的，是：

1. 相机 / frustum 外线
2. `DialogueUI / 字体链`
3. `PlacementManager.cs` 编译红

#### 3.2 Town 进入下一阶段的标准

你这边后续最值得继续给出的，不再是“Town 很重要”，而是：

1. 什么时候它仍只是导演消费层
2. 什么时候它开始接近真正可落 runtime 承接
3. 什么时候它可以被判成更高等级的可交付状态

#### 3.3 线程协同总图

你现在已经最适合继续掌握的，是这个三分结构：

1. `spring-day1`
   - 剧情顺序 / 分场 / 导演调度
2. `NPC`
   - 群像层 / 会话层 / NPC own 承接
3. `Town / 外线`
   - 场景健康 / runtime blocker / 进入可交付状态

---

## 七、当前 `Town stable anchors` 的导演语义，已足够进入协同层

这部分我也收在这里，方便你后续分发时引用。

- `EnterVillageCrowdRoot`
  - 进村后围观层、让位层、初见集体视线
- `KidLook_01`
  - 单点儿童观察位、好奇目光位
- `DinnerBackgroundRoot`
  - 晚餐背景层、远景生活层、非主戏人群层
- `NightWitness_01`
  - 夜间见闻位、夜里有人看见主角或投来一句话的位置
- `DailyStand_01`
  - 次日村中日常站位 1
- `DailyStand_02`
  - 次日村中日常站位 2
- `DailyStand_03`
  - 次日村中日常站位 3

---

## 八、你现在最应该知道的结论

我把这份说明压成一句话就是：

`spring-day1` 现在已经不该继续停在“先别分场、先别写群像”的状态了。  
导演线现在可以开始按 `Town` 的锚点语义分戏、写站位、写围观层和夜间见闻层。  
但 `Town` 还没到 runtime 全闭环，所以你这边仍值得继续推进治理总闸，而不是停手。

---

## 九、如果你后续继续往下做，我最希望你继续盯的 5 件事

1. `Town` 的剩余 blocker 何时真正清到可升级
2. `UI / DialogueUI / 字体链` 何时不再阻断 `Town` 的更高等级验收
3. `PlacementManager.cs` 何时不再继续卡住 Town live
4. Town 与导演线的协同状态是否需要再生成新的治理说明
5. 当 `Town` 状态变化时，是否要及时回灌给 `spring-day1 / NPC`

---

## 十、这轮你不用回我“下一刀做什么”，你只要知道这张全景图

这份说明不是催你立刻接某一刀。

它只是让你知道：

1. 我已经开始进入导演分场阶段
2. 哪些内容已经分给别人
3. 哪些问题仍然在你的治理视野里
4. 你后续如果继续推进 Town 总治理，应该盯哪些面，而不是重新把导演线和 Town scene 本体搅回一锅
