请先完整读取并严格继承以下文件：

- [D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-06_给spring-day1_Town全量进度与可代接工作面详细回执_12.md]
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-06_给spring-day1_Town相机转场玩家位_contract-probe与回球阈值_14.md]
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-06_给spring-day1_Town更深player-facing-contract与下一撞点改判_15.md]
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_Town协作线最终收尾阶段清单_19.md]

再额外继承一条新的总改判，而且这一条会直接改你下一轮主线：

1. `day1` 不再接受“runtime resident 先顶着，后面再迁回”作为继续扩写方向
2. 最终正确形态改成：
   - `Town.unity` 里原生存在 resident
   - 用户自己在 scene 里调整 `001_HomeAnchor / 002_HomeAnchor / 003_HomeAnchor` 这类位置
   - 代码只消费现成位置，不再偷偷改位置
3. 因此你这条 Town 线下一轮最值钱的，不再是继续解释 contract，而是把 `Town` 真往“原生 resident 场景承接”推进

从这一条开始，你继续做 Town own，但只按“Town 原生 resident scene-side 承接”这条主线狠狠干。

允许按需使用 `subagent`，但必须遵守：

1. 真能并行推进时才开
2. 只用于：
   - scene-side 只读审计
   - 窄口 live/probe
   - 不与主刀撞写面的独立子域
3. 最终 Town own 判断和提交由你自己掌控
4. 用完就关

---

## 第一部分：Town 下一轮主刀正式改成“原生 resident scene-side”

你后面优先盯的，不再是“resident contract 说明够不够”，而是：

1. `Town` 里哪些 resident 需要原生存在
2. `Town_Day1Residents` 这层是否已经从“runtime 容器概念”真正落成“scene 原生居民承接层”
3. `Resident_DefaultPresent / DirectorReady / Backstage` 三层怎样服务原生 resident，而不是服务 runtime 生人
4. `Primary` 里的旧 resident 后面应怎样剥离回 Town

---

## 第二部分：你这轮最该继续接的具体工作面

优先顺序固定：

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `NightWitness_01`
4. `DinnerBackgroundRoot`
5. `DailyStand_01`
6. 如果还能继续，再补 `DailyStand_02 / 03`

但请注意：

这轮你的重点不是去替用户决定最终 anchor 坐标。

新的硬边界是：

1. 用户自己调 `001_HomeAnchor` 这类位置
2. 你负责 scene-side 原生存在性、根层、分组层、承接层、合理接刀点
3. 不要再让代码或 probe 结果把“位置配置权”拿走

---

## 第三部分：你这轮必须明确钉死的边界

1. 哪些 resident / slot / root 已经是 Town 原生可承接层
2. 哪些仍只是历史 runtime 过渡痕迹
3. 哪些是用户要手摆的位置
4. 哪些是你 Town own 还能继续合法推进的 scene-side 部分
5. 哪些应该留给 day1 去吃逻辑，不该由你回吞

你不要再回“Town 已经说明清楚了”，而要直接把可施工面压到更窄、更硬。

---

## 第四部分：Town 这轮最终交回给 day1 的东西

1. 当前 `Town` 真正已经原生具备的 resident 承接层
2. 当前还缺的 scene-side 最窄口
3. day1 一旦继续集成时，第一安全接刀点是什么
4. 如果需要用户自己进 scene 手摆，明确告诉 day1 哪些是用户配置位，代码不能碰

---

## 禁止事项

1. 不要碰 `GameInputManager.cs`
2. 不要回吞 `day1` active 逻辑
3. 不要回吞 `NPC` 内容层
4. 不要回吞 `UI` 玩家面
5. 不要再把 runtime resident 当成终态继续包装
6. 不要偷偷用代码或 scene 自动化替用户改 `HomeAnchor` 位置
7. 不要爆 red

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
