请先完整读取并严格继承以下文件：

- [D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_Town协作线最终收尾阶段清单_19.md]
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_Day1最终收尾阶段总控清单_16.md]

从这一条开始，不再停在“Town 已经讲清楚了”的说明层。

这轮唯一总目标固定为：

- 把 `Town` 线程当前能完成的 `Day1 scene-side 承接 + Town 自家遗留` 一起狠狠干到最深处，然后把真正可被 `day1` 继续吃的 Town 基线和可代接工作面交回来。

允许按需使用 `subagent`，但必须遵守：

1. 只在真能并行推进时才开
2. 优先用于：
   - scene-side 只读审计
   - 窄口 live 取证
   - 独立 scene 子域整理
3. 关键判断、最终 Town own 结果由你自己掌控
4. 用完就关

这轮不要只做 Day1 协作面，也要把你自己当前能清掉的 Town own 遗留一起清掉。

---

## 第一部分：继续把 Town 对 Day1 的正式基线钉死成“可直接吃”

这轮你不要再只回“Town 可以承接导演层”。

而是要继续尽力把这些说死并推进到最深：

1. 当前 `day1` 到底应该信哪份 scene-side 基线
2. 哪些 `root / slot / anchor` 已是正式可吃的
3. 哪些仍只是 broader dirty 的一部分
4. 当前一旦继续吃 runtime，最先会撞到的具体触点是什么

如果这轮能进一步把 `day1` 可安全引用的 Town 基线缩得更硬，就不要留给下一轮。

---

## 第二部分：把 resident / anchor / slot 的 scene-side 承接继续往前推进

这轮继续尽力主攻这些点：

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `NightWitness_01`
4. `DinnerBackgroundRoot`
5. `DailyStand_01`
6. 如果还能继续，再补 `DailyStand_02 / 03`

这轮的目标不是泛泛复述，而是尽量推进到：

1. resident root / director-ready / backstage 三层更稳
2. 当前 `day1` 吃这些 anchor 时更少撞 scene-side 模糊区
3. 如果 scene-side 还差一刀，就把那一刀缩到非常具体

---

## 第三部分：Town 自家遗留别再停在 broader dirty 说明，要尽量往可施工子域压

你这轮除了 Day1 协作面，还要继续把 Town own 遗留能清就清：

1. 和 Day1 直接相关的 mixed-scene 子域进一步拆清
2. resident 相关子域与无关 dirty 的进一步剥离
3. 如果当前已具备合法 scene-side 写窗，就不要只写 docs，尽量把最值钱的 scene-side 准备继续往前落一刀
4. 如果当前确实还不宜写，就把“为什么不能写、下一刀具体该写什么”继续缩到最窄口

重点是：

不要再让 `Town` 停在“说明线程”。

---

## 第四部分：继续给 day1 一个真能代接的 Town 工作面

这轮你最终要交给 `day1` 的，不是大而散的 Town 状态说明。

而要尽量交回：

1. 当前 `Town` 真能代接的 Day1 scene-side 工作面
2. 当前不该代接的边界
3. 一旦 `day1` 回球，你接刀的第一刀该落在哪个 anchor / root / slot / 子域
4. 当前仍然挡住 `day1` 的第一真实 Town blocker

---

## 第五部分：Town 这轮也要尽量做自己的尾账，而不是只服务 day1

如果本轮在不越权前提下还能继续推进 Town own 内容，就一起做：

1. scene-side contract 的进一步收紧
2. broader dirty 的进一步归类
3. Town 对 future runtime 承接的更窄落位
4. 当前 checkpoint / baseline 的更稳固化

只要是你这轮合法能做完的，不要机械地全部留给“下一轮可能继续”。

---

## 禁止事项

1. 不要回吞 `day1` 的 active director / StageBook / deployment 主刀
2. 不要回吞 `NPC` 内容层
3. 不要回吞 `UI` 玩家面壳体
4. 不要碰 `GameInputManager.cs`
5. 不要把 `Town` 包装成“整份 scene 已 fully ready”
6. 不要因为怕越权就退回纯说明线程
7. 不要爆 red；一旦 own red，立即止血

---

## 验证与收尾

这轮至少要补：

1. 当前能做的 scene-side / docs / CLI 自检
2. 如涉及 live 现象，尽量拿窄口 runtime 证据
3. 当前正式 checkpoint 与 working tree broader dirty 的区分
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
