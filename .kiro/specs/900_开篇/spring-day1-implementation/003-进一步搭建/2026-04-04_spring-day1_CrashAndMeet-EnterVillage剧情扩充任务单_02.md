# spring-day1｜CrashAndMeet / EnterVillage 剧情扩充任务单

## 1. 任务定位

从这一刀开始，我不再靠聊天记忆自由发挥。

后续真实施工只按这份任务单执行。

这份任务单是当前 `spring-day1` 的唯一主刀约束：

只做 `CrashAndMeet / EnterVillage` 的内部剧情扩充，把最初原案中的前半段补回当前逻辑链。

---

## 2. 本刀已接受基线

先继承下面这些已接受基线，不重打：

1. 当前 Day1 仍保留现有 9 个 `StoryPhase` 大骨架：
   - `CrashAndMeet`
   - `EnterVillage`
   - `HealingAndHP`
   - `WorkbenchFlashback`
   - `FarmingTutorial`
   - `DinnerConflict`
   - `ReturnAndReminder`
   - `FreeTime`
   - `DayEnd`

2. 当前最稳的正式剧情资产只有：
   - `SpringDay1_FirstDialogue.asset`
   - `SpringDay1_FirstDialogue_Followup.asset`

3. 当前正式原案角色承载，仍以：
   - `马库斯`
   - `艾拉`
   - `卡尔`
   为主，不把 `101~301` 当作正式真名角色。

4. 当前 UI 已独立并行，不由我主刀。

---

## 3. 本刀唯一主刀

只补这 5 件事：

1. 矿洞口醒来
2. 怪物逼近
3. 跟随村长撤离
4. 进村围观
5. 闲置小屋安置

翻成人话就是：

把当前被压扁成“首段对话 -> 后续说明 -> 直接疗伤”的前半段，扩回原案里的完整起势。

---

## 4. 本刀 exact-own 范围

本刀允许触碰的内容只限于：

1. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
2. `Assets/111_Data/Story/Dialogue/*`
3. 必要时新增的 Day1 正式对白资产
4. 必要时极小范围触碰正式对话入口脚本
   - 仅当为了让新增剧情资产合法接入
   - 且不改变 UI 显示层 owner

如非必要，本刀不扩到别的路径。

---

## 5. 本刀明确禁止

1. 禁止碰 UI 主刀文件：
   - `Assets/YYY_Scripts/Story/UI/*`
   - `Assets/222_Prefabs/UI/Spring-day1/*`
2. 禁止碰 `Primary.unity`
3. 禁止碰 `GameInputManager.cs`
4. 禁止提前改 `WorkbenchFlashback / DinnerConflict / ReturnAndReminder / FreeTime`
5. 禁止提前把 `Town` 正式内容写进 scene
6. 禁止把 `101~301` 当原案正式具名角色写进这刀主线
7. 禁止大改 `StoryPhase` 枚举

---

## 6. 本刀完成定义

本刀完成时，必须能明确站住下面 5 条：

1. `CrashAndMeet` 不再只是“和村长说两句”，而是至少包含：
   - 醒来
   - 语言错位
   - 危险感
   - 跟随撤离起点

2. `EnterVillage` 不再只是“后续说明”，而是至少包含：
   - 进村
   - 围观
   - 安置到闲置小屋
   - 交给艾拉的前置

3. 小屋仍然是：
   - 废弃闲置房
   - 不是大儿子的房子

4. 当前后续链不能被打断：
   - `HealingAndHP`
   - `WorkbenchFlashback`
   - `FarmingTutorial`
   - `DinnerConflict`
   - `ReturnAndReminder`
   - `FreeTime`
   - `DayEnd`
   仍要能被当前逻辑继续接住

5. 我改动后的剧情源，要继续能给 UI 提供稳定的任务/提示源头，而不是把显示层一起带崩

---

## 7. 本刀施工原则

### 7.1 优先补正式剧情资产

这一刀优先考虑把关键新增段写成正式对白资产，不优先继续把所有段都塞进 runtime `CreateSequence()`。

优先候选：

1. `SpringDay1_MineWake.asset`
2. `SpringDay1_MonsterCue.asset`
3. `SpringDay1_VillageGate.asset`
4. `SpringDay1_HouseArrival.asset`

### 7.2 如暂不新增资产，也必须保证结构可迁移

如果这刀先用临时过桥实现，也必须做到：

1. 段落职责清楚
2. 事件顺序清楚
3. 后续能平滑替换成正式资产

### 7.3 不向 UI 层借刀

这刀不许靠：

1. 修改 PromptOverlay
2. 修改 DialogueUI
3. 修改 WorkbenchOverlay
来“假装剧情补回了”

剧情补回必须发生在剧情源本身。

---

## 8. 对 UI 线程的协作合同

这刀会影响 UI 的，只有“剧情源文本与阶段语义”，不会影响 UI 文件本体。

因此我必须守下面 4 条：

1. 不直接改 UI 文件。
2. 如果开场任务源文案变化，要继续通过现有任务/提示源头输出，不自己绕开 UI。
3. 不删除现有 UI 仍在读取的核心源头接口，除非同刀完成替代。
4. 如果出现“UI 需要更多状态才能稳定显示”的情况，只新增最小剧情源合同，不顺手改显示层。

---

## 9. 推荐实现顺序

这一刀只按这个顺序做：

1. 查清 `FirstDialogue / Followup` 和 `CrashAndMeet / EnterVillage` 的现有接点
2. 先补 `矿洞口危险 -> 跟随撤离`
3. 再补 `进村围观 -> 闲置小屋安置`
4. 最后确认 `HealingAndHP` 还能无缝接上

只要这 4 步没闭环，就不往后漂。

---

## 10. 验证要求

本刀至少要交出：

1. 结构层证据
   - 新增/修改了哪些剧情资产或剧情步
2. 逻辑层证据
   - 当前前半段如何进入 `HealingAndHP`
3. 回归层证据
   - 后续 phase 没被打断

如果本轮没有跑 live，就明确写：

- `结构成立，live 待验证`

不要偷报成“已过线”。

---

## 11. 回执格式

先给用户可读层：

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

---

## 12. thread-state 接线要求

如果继续真实施工，先保持当前 `ACTIVE` slice。

第一次准备 sync 前，必须先跑：

- `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住、让位或不继续，改跑：

- `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补：

1. 是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
2. 如果没跑，原因是什么
3. 当前是 `ACTIVE / READY / PARKED / BLOCKED` 哪一个

---

## 13. 最后一句话

这刀不是“继续润色 Day1”，而是把春一日真正的开场半小时补回当前主线，而且只准切这一刀。
