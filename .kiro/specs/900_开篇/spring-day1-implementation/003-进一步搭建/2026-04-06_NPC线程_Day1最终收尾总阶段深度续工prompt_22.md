请先完整读取并严格继承以下文件：

- [D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_NPC协作线最终收尾阶段清单_18.md]
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_Day1最终收尾阶段总控清单_16.md]

从这一条开始，不再停在“底座已经够了”的结论层。

这轮唯一总目标固定为：

- 把 `NPC` 线程当前能完成的 `Day1 resident / formal fallback / bridge 层` 和你自己这条线的剩余内容一起狠狠干到最深处，然后把真正可被 `day1` 直接吃的真值、内容和护栏交回来。

允许按需使用 `subagent`，但必须遵守：

1. 只在真能并行推进时才开
2. 优先用于：
   - 内容层补充
   - 独立 tests / probes / validation
   - 独立 helper/contract 补强
3. 关键判断和最终 NPC own 结果由你自己掌控
4. 用完就关

这轮不要只做 Day1 协作面，也要把你自己当前能清掉的 NPC own 遗留一起清掉。

---

## 第一部分：继续把 resident semantic matrix 压实成 Day1 可直接消费的真值层

这轮继续把这些 beat 真值守稳并尽量往下压：

1. `EnterVillage_PostEntry`
2. `DinnerConflict_Table`
3. `ReturnAndReminder_WalkBack`
4. `FreeTime_NightWitness`
5. `DayEnd_Settle`
6. `DailyStand_Preview`

本轮继续尽力做到：

1. `priority / support / trace / backstagePressure` 更稳
2. `semanticAnchorIds / presenceLevel / flags / note / growthIntent` 不漂
3. 如果 `day1` 吃这些数据时出现不够用的窄口，就主动把 manifest/helper 往可直接消费方向再补一层

---

## 第二部分：把 formal 一次性消耗后的真实内容承接继续做实

这轮不要只停在 `WillYieldToInformalResident()` 这个 contract 已经存在。

你还要继续尽力补：

1. formal 已消费后的 resident / informal 内容不发空
2. 不同 phase 下的补句不是一坨机械 fallback
3. 玩家再次聊天时，体感上真能区分：
   - 正式剧情已过去
   - 现在是居民日常 / 闲聊 / phase 后余波

如果这轮能继续补：

1. `post-phase 非正式补句池`
2. `resident 日常句池`
3. `pair / ambient / self-talk` 密度

那就不要留给下一轮。

---

## 第三部分：继续补 Day1 真要吃的 bridge / tests / validation

你这轮最值钱的继续推进，不是再写一份“当前边界说明”，而是继续补护栏。

尽量继续推进：

1. manifest -> stagebook bridge
2. resident consumption snapshot -> director consumption 护栏
3. formal consumed -> informal/resident yield 护栏
4. `DailyStand_02 / 03` 相关 bridge / probe
5. validation menu 对 resident consumption roster 的持续校验

如果这轮判断某个 bridge test 已开始真正贴 `day1` own，不能越权就别硬碰；
但要把你能继续做的更窄一层 tests / probes 尽量补完。

---

## 第四部分：往“常驻居民”而不是“阶段演员”继续压内容体感

用户偏好已经很明确：

这批人要越来越像 `驻村居民`，不是继续保守地当“到阶段才蹦一下”的 crowd。

所以这轮你可以继续把自己线上的遗留一起清掉：

1. 同一 NPC 在不同 phase 的常驻存在感
2. resident 日常语义分层
3. 玩家靠近时的轻响应
4. pair / ambient / interrupt / resume 的更真实承接
5. 任何目前仍让人觉得“只有 formal 一过去就空掉”的内容空洞

---

## 第五部分：这轮必须交回什么

你这轮最后交回给 `day1` 的，不是“NPC 底座大体够了”。

而必须尽量交回：

1. 更稳的 resident consumption 真值
2. 更实的 formal-consumed 后内容承接
3. 可直接用于回归的 bridge / validation / probe
4. 当前仍挡住 `day1` 最终整合的第一真实 NPC 侧 blocker

---

## 禁止事项

1. 不要回吞 `CrowdDirector` 主逻辑
2. 不要碰 `Town.unity`
3. 不要碰 `Primary.unity`
4. 不要碰 `GameInputManager.cs`
5. 不要回吞 `UI` 玩家面壳
6. 不要把 deployment / scene runtime 问题继续包装成“NPC 内容层没做完”
7. 不要爆 red；一旦 own red，立即止血

---

## 验证与收尾

这轮至少要补：

1. 代码层 no-red 自检
2. 可跑的 bridge tests / validation menu / probes
3. fresh console / targeted results 中当前能拿到的证据
4. memory / thread memory / skill-trigger-log 回写
5. 用完 subagent 就关
6. 如果达到可提交状态，提交你 own 的这一刀

---

## 最终汇报固定要求

最后给 `day1` 或用户时，必须先说人话，而且逐项显式写清：

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
