# 2026-04-03｜NPC-v｜春一日新 NPC 群像 runtime probe 与本体归仓续工 prompt

这轮不要再把“结构已经齐了”继续往外扩成新的愿望清单。

用户当前真正要的是：

- 在已经实现的 `spring-day1` 主剧情基础上，把这批新增 NPC 真正扩进来；
- 也就是不再停在“资源、prefab、asset、prompt 都有了”，而要让人真的在运行态里成立。

你这轮不是主拿 `Day1 integration`。

你这轮只拿 `NPC-v` 本体层。

---

## 0. 当前已接受基线

下面这些已经成立，不要重做：

### 0.1 统一目标已经钉死

这条线最终要达成的是：

1. 8 个新 NPC 本体层成立
2. 8 个新 NPC 在 `spring-day1` 的正确阶段出现
3. 围绕“村长逃跑已发生”的群像对白真正可被运行时消费

### 0.2 你这边已经站住的结构基线

当前工程里已经真实存在：

- `Assets/Sprites/NPC/101~104, 201~203, 301`
- `Assets/100_Anim/NPC/101~301/*`
- `Assets/222_Prefabs/NPC/101.prefab ~ 301.prefab`
- `Assets/111_Data/NPC/SpringDay1Crowd/*.asset`

`SpringDay1NpcCrowdBootstrap.cs` 已经写入：

- 8 人身份
- 人设
- 与“村长逃跑”一致的对白方向
- phase 基线
- pairDialogueSets

### 0.3 你这边最新新增但尚未归仓的基线

你本轮最新回执已经确认：

- `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
  已经补成全链 preflight
- 当前 Unity 菜单验证再次 `PASS`
  - `npcCount=8`
  - `totalPairLinks=16`
- 当前线程仍是：
  - `PARKED`
- 当前 own 路径仍不 clean：
  - `SpringDay1NpcCrowdValidationMenu.cs` 还没归仓
  - memory 也已更新但未 sync

### 0.4 我的区已经不是 blocker

`spring-day1` / `Day1 integration` 这边已经完成并进入主分支：

- 提交基线：
  - `03c0bf87`
  - `35144958`
- 当前 `spring-day1V2` 已：
  - `PARKED`

所以你这轮不要再把“等 integration 先做”当成默认停车位。

### 0.5 旧 `001 / 002 / 003` scene 漂移不是你这轮主刀

你只读查到的：

- `001 delta = (1.016, 1.196)`
- `002 delta = (-9.620, -1.444)`
- `003 delta = (-10.166, -3.763)`

这个读数很重要，但它当前应判为：

- legacy `Primary / scene / Day1` 侧背景风险

不是你这轮 `NPC-v` own 的唯一主刀。

---

## 1. 你这轮唯一主刀

只做一刀：

- 继承当前已做好的新群像本体层结构和 preflight 基线，
- 然后把“新 8 人 runtime targeted probe”真正拿到手，
- 并把你这轮 own dirty / untracked 一起归仓。

一句话：

- 这轮不是继续“做资源”。
- 这轮是把“新 8 人在运行态里到底站不站得住”说死。

---

## 2. 这轮允许 scope

你这轮只允许在 `NPC-v` 本体层活动：

- `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
- `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
- `Assets/111_Data/NPC/SpringDay1Crowd/*`
- `Assets/222_Prefabs/NPC/*`
- `Assets/100_Anim/NPC/*`
- `Assets/Sprites/NPC/*`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md`

只读参考允许：

- `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
- `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
- `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\state\active-threads\spring-day1V2.json`

---

## 3. 明确禁止漂移

这轮禁止：

- 不要写 `Primary.unity`
- 不要去修旧 `001 / 002 / 003` scene 位置
- 不要改 `SpringDay1Director.cs`
- 不要改 `SpringDay1NpcCrowdDirector.cs`
- 不要重配 manifest 的 phase / anchor / spawnOffset
- 不要碰 UI / 字体 / `GameInputManager.cs`
- 不要把 legacy `002 StuckCancel` 说成这轮新 8 人 own 已修
- 不要回到“继续补一堆结构工具”而不拿 runtime 证据

---

## 4. 你这轮具体要做什么

### 4.1 先继承当前 preflight 基线

开工后先确认并继承：

1. `SpringDay1NpcCrowdValidationMenu.cs` 当前已落的全链检查
2. `Tools/NPC/Spring Day1/Validate New Cast` 当前 `PASS`
3. `pairDialogueSets` 当前总链路数是 `16`

不要重写这套结构；除非 runtime probe 直接证明它漏了关键检查。

### 4.2 再做新 8 人 runtime targeted probe

你这轮真正要补的是运行态，而不是静态护栏。

至少要把下面 4 组 probe 做实：

#### R1. 8/8 instance 基础运行态

逐个确认：

- prefab 真能实例化
- `SpriteRenderer / Animator / NPCAnimController / NPCMotionController / NPCAutoRoamController / NPCInformalChatInteractable / NPCBubblePresenter` 在运行态没有断
- `homeAnchor / roamProfile / facing` 运行态不空、不炸

#### R2. 8/8 单体 informal chat 可进入

逐个确认：

- 新 8 人各自至少能进入自己的默认 informal chat
- 不是只有 asset 有数据，运行态却开不起来

#### R3. 至少 2 组 pair dialogue 真实跑通

至少拿到 2 组代表性 pair 的真实运行态证据，建议优先：

- `101 <-> 103`
- `201 <-> 202`

如果你认为有更能代表风险的组，也可以换，但要说明理由。

#### R4. 至少 2 个 walk-away interrupt 真实跑通

至少拿到 2 个新 NPC 的：

- 发起对话
- 中途跑开
- 玩家离场句 / NPC 反应句 / 收尾链

真实运行态证据。

### 4.3 做完后把这轮 own dirty 一起归仓

如果这轮 probe 没被外部 blocker 打断：

- 你要把当前 `SpringDay1NpcCrowdValidationMenu.cs`
- 以及本轮 memory

一起收进你这轮 own 白名单，不要再把“工具已经写好但还没 sync”继续拖一轮。

---

## 5. 完成定义

这轮完成，不是指“又多了一份自信”。

而是至少要同时满足：

1. 你能明确说出：
   - 8/8 新 NPC 在运行态里哪些已经真正站住
   - 哪些还没站住
2. 你能把问题分清：
   - `NPC-v own`
   - `Day1 integration`
   - `legacy scene / Primary`
3. 你能把当前 `SpringDay1NpcCrowdValidationMenu.cs` 和本轮 memory 一并归仓；
   不再让它们继续悬空
4. 如果这轮还是没法归仓，你必须报第一真实 blocker，
   不能只回“还要继续看”

---

## 6. 如果被 blocker 卡住，怎么报

如果 runtime targeted probe 做不下去，你只能把 blocker 归到下面三类之一：

1. `NPC-v own`
   - 比如 prefab / content / roam / pair / interrupt 自己就坏了
2. `Day1 integration`
   - 比如 phase 消费、spawn 时机、manifest 消费出了问题
3. `legacy scene / Primary`
   - 比如旧 `001 / 002 / 003` scene 现场直接挡住 probe

不要把这三类混成一句“运行态有点问题”。

---

## 7. 固定回执格式

回复时先按这个顺序：

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
- 运行态 probe 结果：
  - `8/8 instance 基础运行态`
  - `8/8 informal chat`
  - `2 组 pair dialogue`
  - `2 个 walk-away interrupt`
- 问题归类：
  - `NPC-v own`
  - `Day1 integration`
  - `legacy scene / Primary`
- thread-state：
  - 是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
  - 如果没跑，原因是什么
  - 当前是 `ACTIVE / READY / PARKED / BLOCKED` 哪一个

---

## 8. 你开工前先读

- `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
- `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
- `Assets/111_Data/NPC/SpringDay1Crowd/*`
- `Assets/222_Prefabs/NPC/101.prefab ~ 301.prefab`
- `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md`
- 配对文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-03-本线程_春一日新NPC群像Day1剧情消费probe任务单-04.md`

---

## 9. 最后一条

这轮别再把“新 8 人已经有 prefab 和 asset”误判成“春一日已经扩完了”。

用户现在真正要看的，是：

- 这些人是不是在运行态里活了
- 会不会说对话
- 跑开时会不会收
- 这些问题到底还是不是你 own

---

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

这不是建议，是当前 live 规则的一部分。除非这轮始终停留在只读分析，否则不要跳过。
