# 2026-04-04 `NPC` 线程｜玩家面 NPC 方向分工与第一刀认领 prompt `01`

请先完整读取：

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\memory.md`
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory.md`
4. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\memory_0.md`

这轮不是让你回头吞 `Prompt / Workbench / 任务列表 / Town DialogueUI`。

这轮也不是让你重新泛谈 “NPC 全线以后怎么做”。

你这轮唯一主刀固定为：

- 把 **玩家面里真正属于 NPC 方向** 的剩余需求收成 `exact-own / 协作切片 / 明确不归你` 三类矩阵；
- 然后只认领 **第一刀最该由 NPC 落地** 的那一刀。

---

## 一、先承认的当前 owner 基线

下面这些是当前 accepted baseline，不要再抢：

1. `UI / SpringUI` 线程当前主刀：
   - `PromptOverlay`
   - `WorkbenchOverlay`
   - 任务列表 / formal-face
   - `Town` 内 `DialogueUI` 的中文基础显示链
2. `spring-day1` 当前主刀：
   - non-UI opening / Day1 逻辑顺序 / 剧情控制 / 约束边界
3. 你 `NPC` 方向现在更像：
   - `NPC 玩家可见聊天 / 气泡 / 提示体验 owner`
   - 但不是 UI 总包 owner
   - 也不是 `Town scene / Primary / 全局输入 / 全局 TMP 底座` owner

一句话说死：

- `UI` 管 `壳体与 formal-face`
- `spring-day1` 管 `剧情顺序与状态`
- `NPC` 管 `NPC 玩家可见聊天 / 气泡 / 提示行为链`

---

## 二、我方目前理解的“可分给 NPC 的剩余需求总账”

下面这些是用户已经反复明确、而且**可以分给 NPC 方向** 的剩余需求。

你必须先完整吸收，不准只挑一条自己喜欢的做。

### A. NPC 气泡显示层

1. 闲聊期间，**谁在说话谁的气泡就必须在上面**
   - 不只是玩家和 NPC
   - `NPC / NPC` 漫游相遇说话也要遵守
2. NPC / 玩家气泡不能再被树木或别的场景物体遮住
   - 用户已经明确把它定义成“这是 UI，不该被场景盖住”
3. 气泡背景不允许再带透明度
   - 统一改成不透明
4. 玩家气泡不能再和 NPC 气泡混成一套
   - 要保持和 `farm` 里玩家自言自语那套语义接近
   - 但这不等于你现在可以重做 `Town DialogueUI`

### B. NPC 提示 / 交互语义

1. NPC 头上的交互提示不该继续保留
   - 用户已经明确要尽量迁到左下角
2. 当同一个 NPC 当前更应该走正式任务 / 正式对话语义时
   - 左下角不要继续写成“闲聊”
   - NPC 方向至少要把自己的语义输出链说清楚
3. 焦点归属要一致
   - 别出现玩家看到的是一个目标
   - 但 NPC 自己输出的却是另一套提示/会话语义

### C. NPC 会话逻辑闭环

1. 正式对话与非正式闲聊的优先级必须稳定
2. 触发、打断、关闭、跑开、重新进入范围后的闭环要完善
3. 不要再留那种“二流子式半截状态”
4. 如果同屏有别的 NPC 在自己漫游或自己聊天
   - 你要把 speaking-owner、显示层级和打断边界想清楚

### D. NPC-NPC / 玩家-NPC 的玩家面一致性

1. `NPC / NPC` 世界内对话
   - speaking-owner 在上
   - 气泡不乱压
   - 不被场景挡
2. `玩家 / NPC` 对话
   - 玩家/NPC 一眼可区分
   - 但又不能各做各的、风格完全脱节

---

## 三、哪些现在明确不归你

你不要吞下面这些：

1. `SpringDay1PromptOverlay.cs`
2. `SpringDay1WorkbenchCraftingOverlay.cs`
3. 任务列表 / formal-face / prefab-first 壳体
4. `Town.unity`
5. `Primary.unity`
6. `GameInputManager.cs`
7. `DialogueUI.cs` 的 Town 中文基础显示链
8. 全局 TMP / 全局字体底座重构

如果你后面判断某条 NPC 需求真被这些东西阻断，
可以报 blocker，
但不能直接越权吞并。

---

## 四、这轮唯一主刀

你这轮只做两步，而且顺序不能乱：

### 第一步

把上面这些剩余需求，收成一份 `NPC 方向 own matrix`：

1. `exact-own`
2. `协作切片`
3. `明确不归我`

这一步不是形式活，你必须把文件 / 责任点 / 为什么归类都写清。

### 第二步

在 `exact-own` 里只认领 **第一刀最该由 NPC 开始做** 的那一刀。

我方建议你优先考虑的第一刀候选，按优先级排序是：

1. `说话者置顶 + 气泡不被场景遮挡 + 背景不透明`
2. `NPC 头顶提示退场 + 正式/闲聊语义一致`
3. `正式/非正式聊天闭环与 speaking-owner 一致性`

你可以不同意这个排序，
但你必须给出你的排序理由。

---

## 五、这轮明确不要做

1. 不要直接大铺实现
2. 不要顺手把 UI 的壳体问题一起修
3. 不要拿 “以后 Town 会统一” 当作当前不拆分的理由
4. 不要只回一份宽泛 owner 讨论
5. 不要把 `NPC` 自己的 exact-own 和 “需要 UI 配合” 混成一锅

---

## 六、完成定义

只有下面这些同时成立，这轮才算合格：

1. 你已把 NPC 方向可分配剩余需求完整吸收
2. 已明确分成：
   - `exact-own`
   - `协作切片`
   - `明确不归我`
3. 每一类都带：
   - 对应文件 / 系统
   - 原因
   - 现在该不该做
4. 已锁定第一刀
5. 已说明为什么这第一刀比其他候选更值钱

这轮**不是**要你直接把第一刀全部做完，
而是先把 NPC 方向的分工和开工顺序钉死，
让后续不会继续和 UI / spring-day1 混线。

---

## 七、回执格式

先给用户可读层六点卡：

1. 当前主线：
2. 这轮实际做成了什么：
3. 现在还没做成什么：
4. 当前阶段：
5. 下一步只做什么：
6. 需要我现在做什么（没有就写无）：

然后补一个 `NPC方向分工矩阵`：

1. exact-own：
2. 协作切片：
3. 明确不归我：

然后补一个 `第一刀排序`：

1. 第一刀：
2. 第二候选：
3. 第三候选：
4. 为什么这样排：

最后再给技术审计层：

- changed_paths：
- 是否真实施工：
- 这轮是否只做 docs / contract：
- 当前 live 状态：
- blocker_or_checkpoint：

---

## 八、thread-state

如果你这轮会继续真实施工，先跑：

- `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 sync 前，必须先跑：

- `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住、让位或只收 docs，不准备继续业务实现，改跑：

- `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补：

1. 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
2. 如果没跑，原因是什么
3. 当前是 `ACTIVE / READY / PARKED / BLOCKED` 哪一个

---

## 最后一句话

你这轮不是继续“做 NPC 很多东西”，
而是先把：

- 什么真归 `NPC`
- 什么只是协作
- 什么不归你
- 第一刀到底该从哪开

一次说死。  
后面再施工，才能不继续和 `UI / spring-day1` 互相踩面。
