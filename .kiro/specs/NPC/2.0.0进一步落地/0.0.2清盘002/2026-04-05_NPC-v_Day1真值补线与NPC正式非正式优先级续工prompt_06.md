# 2026-04-05｜NPC-v｜Day1 真值补线与 NPC 正式/非正式优先级续工 prompt

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-04_NPC-v_春一日原剧本群像回正与NPC本体收口prompt_05.md`

这轮不要再等 `spring-day1` 给你更长口头解释。

下面这些就是我以 `Day1 owner` 身份现在正式给你的真值和下一步分工。

## 0. 先裁定你这份回执

你的回执大方向是对的，我接受这 5 条：

1. 你不再接：
   - `SpringDay1Director.cs`
   - `SpringDay1NpcCrowdDirector.cs`
   - `Primary.unity`
   - `PromptOverlay`
   - opening / Day1 正式剧情控制
   - Day1 integration 总仲裁
2. 你继续守：
   - `NPCBubblePresenter.cs`
   - `PlayerNpcChatSessionService.cs`
   - NPC 非正式聊天底座
   - NPC 会话状态、speaking-owner、pair/ambient bubble 底座
3. `NPC001 / NPC002 / NPC003`
   - 继续按原 Day1 主角色承载看
4. `101~301`
   - 当前没有更高权威一对一 exact mapping 时，默认接受“证据不足的槽位降级为匿名 / 次级群众”
5. 你已站住的 speaking-owner / stale owner / 不透明背景 / 自动续聊底座
   - 我暂时按“底座已成立，但还待 live 终验”处理

但我这里补一个纠偏：

- `PlacementManager.cs(1694,23)` 这条外部编译红，只能阻断你继续做 fresh live runtime probe
- 不能阻断你这轮先把 `NPC own` 的静态真值、内容回正和优先级代码补完

也就是说：

- 你这轮不能因为那条外部红就整轮停成“什么都不做”
- 你应该先做不依赖 live compile 通过的 own 静态工作

## 1. 我现在给你的 Day1 真值

### 1.1 正式/非正式/环境气泡优先级

从现在起，这条线统一按这个优先级走：

1. `正式剧情 / 正式任务 / 正式会话`
2. `非正式聊天 / casual talk`
3. `ambient pair / 环境气泡`

翻成人话：

- 只要当前玩家处在 Day1 正式主链阶段，或者 formal interactable / formal dialogue 当前应当接管，
  就不能再让共享提示链对玩家显示“闲聊”。
- 非正式聊天只能在“当前没有 formal 接管需求”时浮上来。
- ambient/pair bubble 更不能压过 formal。

### 1.2 哪些阶段 formal 必须压住 casual

下面这些阶段，你默认按“formal 优先”处理：

1. `CrashAndMeet`
2. `EnterVillage`
3. `HealingAndHP`
4. `WorkbenchFlashback`
5. `FarmingTutorial`
6. `DinnerConflict`
7. `ReturnAndReminder`

在这些阶段里，如果玩家当前附近对象同时具备：

- formal 任务承载
- formal sequence 待触发
- formal sequence 正在播
- formal 任务未完成但对象仍是主链入口

那么：

- 共享提示不要再写“闲聊”
- NPC casual bubble 不要抢成当前主交互语义
- pair/ambient 只允许退到背景层，不许盖过主链

### 1.3 原剧本主角色真值

这 3 条你现在就按正式真值处理：

- `NPC001` = `马库斯 / 村长`
- `NPC002` = `艾拉 / 村长女儿`
- `NPC003` = `卡尔 / 研究儿子` 的当前最接近承载

你不要再让 `101~301` 去抢这三条的正式角色位。

### 1.4 `101~301` 当前允许的口径

在我没有给你更高权威 exact mapping 之前，你现在统一按下面这张表收：

- `101`
  - 不再 claim 正式具名原案角色
  - 降为：`匿名文书/抄录员群众`
- `102`
  - 不直接 claim 原案具名“大儿子”
  - 只保留：`猎户 / 外出打猎线索位 / 猎线索群众`
- `103`
  - 不 claim 正式具名角色
  - 降为：`目击型少年 / 跑腿少年群众`
- `104`
  - 不 claim 正式具名角色
  - 降为：`木匠 / 工匠群众`
- `201`
  - 不 claim 正式具名角色
  - 降为：`织补 / 照料型群众`
- `202`
  - 不 claim 正式具名角色
  - 降为：`花铺 / 安神草群众`
- `203`
  - 不 claim 正式具名核心角色
  - 降为：`饭馆大姐 / 饭馆背景群众`
- `301`
  - 当前不应继续当 Day1 正式角色
  - 如需保留，只能降为：`夜路/墓地传闻氛围位`
  - 不再当白天主链正式承载

一句话：

- `102 / 103 / 203` 只保留语义接近
- `101 / 104 / 201 / 202 / 301` 统一降回群众层或氛围层

## 2. 你这轮唯一主刀

你这轮只做两段，顺序不要乱。

### 第一段：NPC own 的静态回正

这段不等外部编译红解除，先做。

你只在自己的 own 路径里收这几件事：

1. 把 `101~301` 的 content / roleSummary / 对话口径 / 表现语义回正到上面的真值
2. 不再让它们的对话包、摘要、表现语义继续暗示“我就是原案正式主角”
3. 只保留群众层、线索层、氛围层该有的信息量
4. 如某个 prefab / profile / data 里还写着明显写偏的人设，就最小回正

这段你可以动：

- `Assets/111_Data/NPC`
- `Assets/222_Prefabs/NPC`
- `Assets/100_Anim/NPC`
- `Assets/Sprites/NPC`
- `Assets/Editor/NPC*`
- 以及你 own 的 NPC controller / service / interactable 链

但不要动：

- `SpringDay1Director.cs`
- `SpringDay1NpcCrowdDirector.cs`
- `SpringDay1NpcCrowdManifest.asset`
- `Primary.unity`

### 第二段：NPC own 的正式/非正式优先级和气泡收口

这段继续只在你 own 范围内做，不回吞我这边的 Day1 正式控制。

你要直接收这 4 件事：

1. formal > casual > ambient 的优先级在 NPC own 底座层落实
2. 当前 formal 应接管时：
   - 不再让共享提示链给玩家显示“闲聊”
   - 不再让 casual bubble 冒成当前主交互语义
3. `pair bubble` 真正亮起来
4. NPC 旧气泡样式继续贴住用户认可旧版，不准重新设计

如果你需要我给 formal 真值，你就直接用本文件第 1 节的真值，不要再卡住等回复。

## 3. 这轮你不准做的事

不要碰：

- `Primary.unity`
- `SpringDay1Director.cs`
- `SpringDay1NpcCrowdDirector.cs`
- `PromptOverlay`
- `Workbench`
- `DialogueUI`
- opening tests / opening bridge
- `GameInputManager.cs`
- Town 场景施工
- 字体 / TMP

也不要把外部编译红当成这轮完全不推进的借口。

## 4. 外部编译红怎么处理

`PlacementManager.cs(1694,23)` 那条：

- 不是你 own
- 不是你这轮可以接盘的目标
- 它只阻断你继续做 fresh runtime live probe

所以这轮正确顺序是：

1. 先完成第 2 节第一段和第二段里不依赖 live 通过的静态 own 改动
2. 再报实：
   - `static own 已完成`
   - `live probe 被外部编译红阻断`
3. 等 shared 红清掉后，再只补 runtime targeted probe

## 5. 这轮完成定义

你这轮完成，不是“我知道该怎么做了”，而是至少同时满足：

1. `101~301` 已回正到我给的真值口径
2. 不足以 claim 正式角色的槽位，已经真的降级为群众层 / 氛围层
3. formal > casual > ambient 的 NPC own 底座优先级已经补进去
4. 当前 formal 应接管时，不再冒“闲聊”语义
5. `pair bubble` 和旧气泡样式的剩余 own 修补已落到代码/资产层
6. 如果 live 还不能跑，你能明确报：
   - 静态已完成什么
   - live 被哪条 external red 挡住

## 6. 固定回执格式

先交用户可读层：

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
- 优先级落地矩阵：
  - formal
  - casual
  - ambient
- `101~301` 回正矩阵：
  - 哪些降级为群众层
  - 哪些只保留线索位
  - 哪些不再 claim 正式角色
- thread-state：
  - 是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
  - 如果没跑，原因是什么
  - 当前是 `ACTIVE / READY / PARKED / BLOCKED` 哪一个

## 7. 最后一句

这轮你不用再等我继续解释“Day1 真值是什么”。

我已经给了。

你现在只要按这个真值，把 `NPC own` 的：

- 角色口径
- 内容承载
- formal/casual/ambient 优先级
- pair / 旧气泡样式

真正收扎实。
