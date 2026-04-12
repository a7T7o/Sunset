请先完整读取并严格继承以下文件：

- [D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_UI线程给day1全量回执_01.md]
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_UI线程_给day1阶段回执_25.md]
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_UI协作线最终收尾阶段清单_17.md]

再额外继承一条新的 day1 总改判：

1. day1 后面不再继续接受 `runtime resident` 这条终态路线
2. 最终目标改成：
   - `Town` 原生 resident
   - formal one-shot
   - formal consumed 后玩家面回落到 resident / informal / 非正式补句
3. 因此你这条 UI 线后面不需要再为“runtime 假居民”额外做玩家面适配

从这一条开始，你继续只做 `Day1 玩家面结果 + UI 自家遗留`，但按新的 resident 终态口径来收。

允许按需使用 `subagent`，但必须遵守：

1. 真能并行推进时才开
2. 只用于：
   - capture / probe / live 证据
   - 独立 UI 壳体实现
   - tests / 验证
3. 最终 UI 结果判断由你自己掌控
4. 用完就关

---

## 第一部分：继续把 one-shot 玩家面彻底守住

这轮继续优先守：

1. formal 未消费前，提示壳必须像正式入口
2. formal 已消费后，不允许玩家面还装成正式推进
3. formal consumed 后必须明确回落到：
   - resident 日常
   - informal 闲聊
   - phase 后非正式补句
4. 已完成任务不能继续假装未完成

这部分和“原生 resident 终态”是同一件事，不是小修。

---

## 第二部分：UI 只围绕“现成 resident 被消费”去收，不围绕 runtime 假居民补壳

后面你继续收 `DialogueUI / Prompt / Workbench / 互动提示` 时，统一按这个口径：

1. 玩家看到的是现成 resident 的状态变化
2. 不是 runtime 新生成一个人然后 UI 再去跟着补说法
3. 所以提示文案、idle caption、formal/informal/resident 区分，都要更像常驻居民语义

如果你这轮还能继续补：

1. resident fallback 的更细文案
2. formal consumed 后的更自然提示壳
3. DialogueUI live 终验
4. Workbench live 终验

就不要留到下一轮。

---

## 第三部分：这轮 UI 真正该继续主刀的仍然是

1. `Workbench` 修后 fresh live 图或第一真实 blocker
2. `DialogueUI` 正式面 fresh live 图
3. one-shot 玩家面提示壳
4. resident fallback 玩家面语义

不要回去重做已经过线的任务卡主结构，也不要漂回泛 UI 修补。

---

## 禁止事项

1. 不要碰 `Town.unity`
2. 不要碰 `Primary.unity`
3. 不要碰 `GameInputManager.cs`
4. 不要回吞 NPC 会话 / 气泡底座 owner
5. 不要为了兼容 runtime 假居民再加一层新壳
6. 不要爆 red

---

## 最终汇报固定要求

最后给 `day1` 或用户时，必须先说人话，而且逐项显式写清：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么
7. 你这轮最核心的判断是什么
8. 你为什么认为这个判断成立
9. 你这轮最薄弱、最可能看错的点是什么
10. 你给自己这轮结果的自评

---

## thread-state

如果这轮从只读进入真实施工，或继续一个已开的施工切片，必须自己执行：

1. 开工前：`Begin-Slice`
2. 准备 sync 前：`Ready-To-Sync`
3. 中途停下或本轮结束：`Park-Slice`
