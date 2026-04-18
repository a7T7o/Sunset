# 2026-04-15｜给 NPC｜Day1-V3 协作边界与 facade 弱引导 prompt

请先完整读取下面 2 份文件：

1. [Day1 现存问题总览与整体施工总表](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_Day1现存问题总览与整体施工总表.md)
2. [给 NPC 的协作边界与 facade 落地 prompt](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/102-owner冻结与受控重构/2026-04-15_Day1-V3_给NPC_协作边界与facade落地prompt.md)

这不是让你替 Day1 收剧情，也不是让你替导航决定 pathing。

这是一次弱引导同步，你要先自己判断，再回答你 own 的那部分。

## 你现在需要同步的核心语义

1. `Day1` 负责剧情语义，不该继续深碰 NPC 身体
2. `NPC` 负责 resident state 与 locomotion facade
3. `导航` 负责 movement execution
4. `003` opening 之后最终应并回普通 resident release contract
5. 剧情结束后，Day1 只该发 release intent，不该继续代管 resident 下半生

## 你现在不要做什么

1. 不要回吞 Day1 phase / beat / cue
2. 不要回吞导航策略
3. 不要因为这次同步就机械照抄我给的 facade 名字
4. 不要只把 facade 做成“换个名字的低级 public 原语”

## 你要自己回答的问题

请你自己判断，并回我下面这些：

1. 你现在是否仍然认为 facade 应该由 NPC 线程主导落地
2. 以你现有代码现实，高危 public 写口里哪些必须优先 internal-only
3. Day1 最先应该停止碰哪些低级 API
4. 导航未来还能合法直接碰哪些口，哪些不该碰
5. 你认为最小可落地的 facade 面应该长什么样
6. 你的唯一下一刀应该是什么

## 你的回执重点

只需要回答：

1. 你 own 什么
2. 你不 own 什么
3. 你建议先落哪一层 facade
4. 你觉得 Day1 当前最危险的越权写口是什么

不要把回复写成：

1. Day1 应该怎么推进剧情
2. 导航该怎么做 avoidance

这两块都不是你 own。
