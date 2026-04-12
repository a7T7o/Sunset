## 先读这份，再继续施工

这不是让你回去重补 NPC 底座，也不是让你回吞 day1 导演工具。  
这是基于当前最新现场，给你重新钉死：你现在在 `Day1` 里最值钱、最该并行、最不会撞线的一刀到底是什么。

---

## 当前主线固定为

继续只做 `Day1` 后半段会被导演线消费的 `NPC 群像内容层`。

说白话：
- 不是再修“NPC 能不能用”
- 也不是去碰 `SpringDay1Director.cs / SpringDay1NpcCrowdDirector.cs / Town / UI`
- 而是把你 own 的那批 `围观 / 背景 / 夜见闻 / 次日站位` 内容，补到不空、不硬、不抢正式主戏

---

## 你必须继承的当前真状态，不要推翻

1. 你当前 own 的底座已经不是主 blocker：
- `Validate New Cast` 已 pass
- `Run Runtime Targeted Probe` 已 pass
- informal / pair / walk-away / crowd interrupt-resume 底座已站住

2. 你当前真正能担的，不是 Day1 总导演，而是这些：
- `NPCBubblePresenter.cs`
- `PlayerNpcChatSessionService.cs`
- NPC 非正式聊天底座
- 群像内容层
- 被 day1 消费时的语气、强度、闲聊口径、背景承接

3. 你不要回吞这些：
- `SpringDay1Director.cs`
- `SpringDay1NpcCrowdDirector.cs`
- `SpringDay1DirectorStageBook.json`
- `Primary.unity`
- `Town.unity`
- `PromptOverlay / Workbench / DialogueUI`
- `GameInputManager.cs`
- Town runtime contract

4. 当前 day1 导演线的最新真状态：
- 导演工具已经 fresh 到：
  - `Primary live capture` 已打通
  - `Director Staging Tests` 已 `7/7 PASS`
- 所以后面会越来越直接地消费你这边的群像内容，不再只是看你底座通不通

5. 当前 Town 的真状态：
- `Town` 语义承接层够用了
- 但 runtime contract 还没完全接上
- 这意味着你现在最该做的是把“内容层”准备好，而不是去扛运行时锚点

---

## 这轮唯一主刀

只做 `Day1 后半段群像内容并行续工`。

你这轮的目标不是再解释“我这边已经够用了”，而是把后面会被导演线吃进去的那些群像内容，真实往前补一轮。

---

## 这轮要补的内容范围

### A. EnterVillage_PostEntry

围绕这些承载：
- `enter-crowd-101`
- `enter-kid-103`

你要补的是：
- 进村后围观层的口气
- 小孩/少年视角的好奇、试探、偷看
- 让位感、停手感、目光压到主角身上的群像氛围

硬要求：
- 不要把 crowd 写成正式 named 主角
- 不要变成任务提示器
- 不要直接教玩家去哪

### B. DinnerConflict_Table / ReturnAndReminder_WalkBack

围绕这些承载：
- `dinner-bg-203`
- `dinner-bg-104`
- `dinner-bg-201`
- `dinner-bg-202`
- `reminder-bg-203`
- `reminder-bg-201`

你要补的是：
- 晚餐冲突时的背景压力
- 谁在看、谁不说话、谁低声议论、谁让位
- 晚饭后回屋提醒时的散场氛围

硬要求：
- 这些人只能是背景层、群像层、侧压力层
- 不要抢主戏，不要冒充正式推进者

### C. FreeTime_NightWitness / DayEnd_Settle / DailyStand_Preview

围绕这些承载：
- `night-witness-102`
- `night-witness-301`
- `dayend-watch-301`
- `daily-101`
- `daily-103`
- `daily-102`
- `daily-104`
- `daily-203`
- `daily-201`

你要补的是：
- 夜间见闻的“看见你但不完全接纳”的感觉
- 第一夜收束时的外部压力和规矩感
- 次日站位预示的“村子照常转，你只是暂时被塞进来”的感觉

硬要求：
- 不要写得像系统旁白
- 不要写成主题句
- 不要写成给玩家的剧情总结

---

## 这轮完成定义

这轮不接受“我再分析一下群像方向”。  
你至少要尽量做到：

1. 真实落到你 own 的群像内容资产 / 文本 / 默认规则链  
2. 不破坏 `Validate New Cast` / `Runtime Targeted Probe` 既有 clean 结果  
3. 明确告诉我：
- 哪些后半段群像内容你这轮已经补到可消费
- 哪些还没补
- 还有哪些地方因为角色真值不足，只能继续按匿名/次级群众处理

---

## 角色真值口径继续锁死

1. `NPC001 / 002 / 003` 仍按原 Day1 主角色承载看  
2. `101~301` 这批当前大多继续按 crowd 槽位 / 次级群众层处理  
3. 证据不足的槽位，不要强行 claim 成原案正式角色  
4. 如果只能做匿名 / 次级群众，也要老老实实按匿名 / 次级群众写，不要偷偷文学化升级

---

## 禁止事项

1. 不要碰 `SpringDay1Director.cs`
2. 不要碰 `SpringDay1NpcCrowdDirector.cs`
3. 不要碰 `SpringDay1DirectorStageBook.json`
4. 不要碰 `Primary.unity`
5. 不要碰 `Town.unity`
6. 不要碰 `PromptOverlay / DialogueUI / Workbench UI`
7. 不要擅改 NPC 气泡样式
8. 不要回到“再补底座”
9. 不要爆红

---

## 回执格式

回我时必须先给用户可读汇报层，顺序固定：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

然后再给技术审计层，至少补：
- changed_paths
- 这轮具体补了哪几个 cue/群像层
- `Validate New Cast / Runtime Targeted Probe` 是否仍 clean
- 当前 own 路径是否 clean
- 当前 thread-state
- blocker_or_checkpoint

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
