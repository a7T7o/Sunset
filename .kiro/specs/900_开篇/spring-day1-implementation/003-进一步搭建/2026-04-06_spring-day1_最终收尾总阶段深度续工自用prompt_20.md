请先完整读取并严格继承以下文件：

- [D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_Day1最终收尾阶段总控清单_16.md]
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_UI协作线最终收尾阶段清单_17.md]
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_NPC协作线最终收尾阶段清单_18.md]
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_Town协作线最终收尾阶段清单_19.md]

从这一条开始，不再回到方案、提醒、口头分工或轻量试探。

你现在不是普通施工线，你是 `Day1 owner / 导演 / 主控台 / 最终整合位`。

这轮唯一总目标固定为：

- 把 `Day1` 从“已有合同、已有导演工具、已有协作清单”推进到“主链 one-shot 规则开始真闭环，resident deployment / director consumption 真开始被吃回，后半段导演数据和正式剧情产物继续往最终 baseline 深压”。

允许按需使用 `subagent`，但必须遵守：

1. 只在真能并行推进时才开
2. 优先用于：
   - 只读梳理
   - 独立文件实现
   - tests / validation / verification
3. 关键架构、主线判断、最终集成必须你自己掌控
4. 用完确认无价值后立刻关闭

这轮不要只做 Day1 协作面，也要把你自己这条线当前能清掉的遗留一起清掉。

---

## 第一部分：先把 one-shot 不可重入规则真正钉进 Day1 主链

这是本轮第一优先级，不守住就是 bug。

本轮必须尽全力做到：

1. 已消费的正式剧情不能再次以正式剧情身份触发
2. 已完成任务不能再次以正式推进身份重播
3. 已消费 formal 的 NPC，再次对话只能回到：
   - `informal`
   - `resident 日常`
   - `phase 后非正式补句`
4. `director beat / prompt / phase / interaction hint / NPC formal entry` 的对外语义一致

如果你动到：

1. `SpringDay1Director.cs`
2. `SpringDay1NpcCrowdDirector.cs`
3. 剧情 phase / 任务推进链
4. formal 入口判断

那就必须顺手把这条 one-shot 规则一起落稳，不允许把它再留成“后面整合时再说”。

---

## 第二部分：把 resident deployment 和 director consumption 真正吃回主链

这轮要狠狠干，而不是只修一两个 cue。

优先顺序固定为：

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `NightWitness_01`
4. `DinnerBackgroundRoot`
5. `DailyStand_01`
6. 如果还能继续，再补 `DailyStand_02 / 03`

本轮至少尽力做到：

1. `BuildBeatConsumptionSnapshot()` 在 Day1 runtime 里开始被真正消费
2. `resident root / parent / semantic anchor / cue` 不再各走各的
3. `101~301` 的存在方式更像常驻 resident，而不是到点突然生成的临时演员
4. 如果当前 `Town` scene-side 已经够用，就吃到真实 Town 语义；如果还差某个窄口，就明确卡死到具体 anchor / live 现象，不准再泛化成“Town 整体还不行”

---

## 第三部分：继续把导演工具当生产工具，不是成果展板

这轮不要停在“工具已可用”。

你必须继续用它产出更多可消费的真实数据。

至少继续尽量推进：

1. `EnterVillage_PostEntry`
2. `DinnerConflict_Table`
3. `ReturnAndReminder_WalkBack`
4. `FreeTime_NightWitness`
5. `DayEnd_Settle`
6. `DailyStand_Preview`

当前最值钱的补刀点是：

1. `DinnerBackgroundRoot` 再往复杂多人背景层推进一刀
2. `DailyStand_02 / 03` 真的补进当前 runtime 可消费链
3. 继续把 stale 的绝对 path / proxy cue 往 `semantic anchor + offset` 迁

---

## 第四部分：继续把正式剧情产物往可资产化层压，不要又停在抽象说明

这轮允许走大一步，所以不要只写运行时。

要同时继续压这些段落：

1. `CrashAndMeet`
2. `EnterVillage`
3. `HealingAndHP`
4. `WorkbenchFlashback`
5. `FarmingTutorial`
6. `DinnerConflict`
7. `ReturnAndReminder`
8. `FreeTime`
9. `DayEnd`

能落对白稿就落对白稿，能落事件顺序就落事件顺序，能直接挂主链就直接挂，不要继续停在“后续可资产化”的抽象层。

---

## 第五部分：主动吃回三条协作线成果，不要被动等齐

这轮你要主动把：

1. `UI` 已做成的玩家面结果
2. `NPC` 已做成的 resident / formal contract
3. `Town` 已做成的 resident scene-side 承接

往 `Day1` 主链里真吃回。

如果三条线在本轮中途回了新的结果：

1. 能直接吃就直接吃
2. 能直接整合就直接整合
3. 不要把“等他们都完全做完再统一接”当成默认停车位

---

## 第六部分：把你自己的遗留也一起清

你这条线当前自己的遗留，不只包括业务逻辑，还包括这些：

1. 导演链验证没有 fresh 到最新菜单名单的问题
2. `DailyStand_02 / 03` 还没真正站住
3. `Town / UI / NPC -> Day1` 还没完成最终总整合
4. one-shot 规则还没在 Day1 主链上彻底钉死

本轮不要再把这些当成“下一轮正式整合前的准备项”。
能清的就清，能压的就压，命中真实 blocker 再停。

---

## 禁止事项

1. 不要回 UI 壳体细节
2. 不要碰 `Town.unity`
3. 不要碰 `Primary.unity`
4. 不要碰 `GameInputManager.cs`
5. 不要回吞 NPC own 会话 / 气泡底座
6. 不要把导演工具继续扩成豪华编辑器
7. 不要停在分析、提醒、计划或轻量试探
8. 不要爆 red；一旦 own red，立即止血

---

## 验证与收尾

这轮至少要补：

1. 最小 no-red 自检
2. fresh console / 菜单 / tests 中当前能跑的真实验证
3. worktree memory / thread memory / skill-trigger-log 回写
4. 用完 subagent 立刻关闭
5. 如果达到可提交状态，提交你 own 的这一刀

---

## 最终汇报固定要求

最后给用户时，必须先说人话，而且逐项显式写清：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么
7. 我这轮最核心的判断是什么
8. 我为什么认为这个判断成立
9. 我这轮最薄弱、最可能看错的点是什么
10. 我给自己这轮结果的自评

---

## thread-state

如果这轮从只读进入真实施工，或继续一个已开的施工切片，必须自己执行：

1. 开工前：`Begin-Slice`
2. 准备 sync 前：`Ready-To-Sync`
3. 中途停下或本轮结束：`Park-Slice`
