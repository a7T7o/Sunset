请先完整读取并严格继承以下文件：

- [D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_NPC协作线最终收尾阶段清单_18.md]
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_全量进度与承接边界回执_14.md]
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_阶段安全点回执_15.md]

再额外继承一条新的硬改判：

1. `day1` 这边已经正式不再接受“runtime resident 继续扩写”这条路线
2. 最终目标改成：
   - `Town` 原生 resident
   - 用户手摆 home anchor
   - 代码只消费不改位置
3. 因此你这条 NPC 线后面不该再围绕：
   - runtime spawn 预热
   - 阶段演员式突然出现
   继续扩写

从这一条开始，你继续做 NPC own，但只按“原生 resident 常驻语义 + formal 回落 + director 可消费 contract”这条主线狠狠干。

允许按需使用 `subagent`，但必须遵守：

1. 真能并行推进时才开
2. 只用于：
   - 内容层补充
   - tests / validation / probe
   - 独立 helper
3. 最终 NPC own 判断和提交由你自己掌控
4. 用完就关

---

## 第一部分：继续把 resident 常驻语义做实，不要再往 runtime spawn 漂

这轮继续把 `101~301` 往真正的“驻村原生居民”语义推进，而不是阶段演员语义。

优先继续压：

1. `residentBaseline`
2. `residentBeatSemantics`
3. `sceneDuties`
4. `semanticAnchorIds`
5. `growthIntent`
6. `presenceLevel / flags / note`

重点不是再证明“这批人可以被导演消费”，而是让他们更像：

1. 本来就住在村里
2. formal 过去后还能继续活着
3. 不会只在剧情点到时才突然有存在感

---

## 第二部分：formal 一次性消费后的内容回落继续补深

你已经有 contract 了，但这轮继续尽量补到更实：

1. formal consumed 后，玩家再次聊天不空
2. resident / informal / phase 后补句的差异更明显
3. `日常交流 / 聊聊近况` 这类回落不只是壳，而是真有内容层支撑
4. pair / ambient / self-talk / nearby 继续补常驻密度

如果这轮能继续补：

1. phase-specific resident lines
2. post-phase 非正式补句
3. 同一 NPC 在不同 beat 的常驻层次

那就不要留到下一轮。

---

## 第三部分：继续补 day1 真要吃的 contract，但停止围绕 runtime spawn 扩写

你后面还能最值钱地继续帮 day1 的，是这些：

1. `BuildBeatConsumptionSnapshot()` 相关 helper / tests / validation
2. director consumption role 护栏
3. formal consumed -> resident fallback 护栏
4. manifest <-> stagebook 的窄 bridge tests

但不要再主动往这些方向扩：

1. resident runtime spawn 预热
2. 临时生成再迁回
3. “先用 runtime 假身位顶着”的额外适配

---

## 第四部分：你这轮最终交回给 day1 的东西

1. 更实的 resident 常驻内容层
2. 更稳的 formal consumed -> resident/informal 回落 contract
3. 更厚的 bridge / validation / tests
4. 仍挡住 day1 的第一真实 NPC blocker

请记住：

你这轮不是去替 day1 / Town 做 scene 迁移。

---

## 禁止事项

1. 不要碰 `Town.unity`
2. 不要碰 `Primary.unity`
3. 不要碰 `GameInputManager.cs`
4. 不要回吞 `CrowdDirector`
5. 不要把 runtime spawn 问题继续包装成你要补的主线
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
