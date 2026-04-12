# 2026-04-08｜spring-day1｜Day1 最终闭环深砍自用 prompt 31

请先完整读取：

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-08_spring-day1_Day1最终闭环主控清单_30.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_原生resident迁移收尾主控prompt_26.md`
3. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_0.md`

从这一条开始，不回方案模式，不回只读分析，不回“先停安全点”模式。

你的身份固定为：

- `spring-day1 owner`
- `Day1 主控台`
- `导演线 owner`
- `Town -> Primary -> Town -> DayEnd` 最终整合位

当前唯一总目标固定为：

把 `Day1` 真正推进到“用户可以开始按完整剧情通路验收”的阶段。

你必须继承并且不要推翻的硬事实：

1. `Town` 现在已经站住：
   - entry contract
   - player-facing contract
   - `HomeAnchor` readiness
2. `one-shot formal` 跨场景掉回 bug 已经修住，而且已有 `DayEnd` live 证据
3. 用户已经明确裁定：
   - `Town` resident 必须是原生 resident
   - 位置由用户自己摆
   - 代码只能消费，不能偷改
4. 用户最新明确要的不是“导演工具再变强一点”，而是：
   - 剧情主链真的顺
   - 引路真的像引路
   - `001/002` 真的在对的地方、做对的事
   - 晚饭/回村/睡觉真的闭环
5. 用户刚补的 profiler 证据已经坐实：
   - 启动大卡顿主因之一是 `PersistentPlayerSceneBridge.Start()` / `RebindScene(...)`
   - 另一条大峰值是 `NavGrid2D.RebuildGrid()`
   - `UI/TMP` 不是这次最主要的启动卡顿责任面

允许按需使用 `subagent`，但只有真能并行推进时才开，而且默认模型用 `gpt-5.3-codex`；关键架构判断和主线整合由你自己掌控。

---

## 第一部分：Town 开场与进村围观，必须按用户现成 scene 点位收死

这部分完成定义不是“群众会动”，而是：

1. 开场就在 `Town`
2. 一进入剧情，群众就落在用户已摆好的起点区域
3. 群众走位吃用户已经调好的 scene 物体位置，不再从大老远 home 位跑来冒充起点
4. 进村围观的导演结果，和用户在层级里摆好的结果一致
5. `001` 村长带路，`002` 艾拉跟上

你要继续把下面这些收穿：

1. 剧情开始时 crowd actor 的起始落点逻辑
2. director/runtime 对“scene 点位 vs home anchor”的优先级
3. `001/002` 不被 crowd 或 roam 反向拉走
4. 走位完成前，不要抢开关键对白

---

## 第二部分：引路细节必须像真正的带路，不是 NPC 自己赶路

这条是 bug 线，不是优化线。

你必须继续守住并补到位：

1. 村长引路时，和玩家有真实最小距离控制
2. 当前口径按用户最新裁定：
   - 距离拉长到约 `5` 个单位
   - 提示词固定为：`小伙子，先跟我走`
3. 如果玩家掉队太远：
   - `001` 停
   - 不要抽搐回头
   - 不要反复 stop/start 抖动
4. 玩家跟上后继续走
5. 如果玩家已经更接近传送门，`001` 不要硬等到拖戏
6. `002` 艾拉也必须跟着村长一起走进 `Primary`

重点不是继续调导演工具，而是把 runtime 主链真的改对。

---

## 第三部分：Primary 主链必须从“乱跳”收成真正可玩的桥段

你必须把下面这些补成正式桥段，而不是半成品：

1. `001/002` 到了 `Primary` 后，在剧情控制期间：
   - 关闭漫游
   - 关闭自动打招呼
   - 关闭会和导演抢控制权的逻辑
2. 治疗段：
   - 改成玩家走到艾拉身边触发
   - 不再要求艾拉挤到玩家身上
   - 治疗前后保留必要停顿和对白过渡
3. 工作台段：
   - 村长先把玩家带到工作台附近
   - 到位后再开对白
   - 新增“箱子里有我给你准备的东西”这段桥接对白
   - 只有这段对白完成后，工作台交互才允许打开
4. 玩家完成工作台这段后：
   - `001/002` 在 `Primary` 等玩家，不继续乱逛
   - 真正准备回村时，再一起带回 `Town`

---

## 第四部分：回村、晚饭、自由活动、睡觉，必须形成完整闭环

你这轮不能只把白天跑通。

你必须继续把下面这段也收掉：

1. 回村 crowd 演出直接复用第一次进村那套语言
2. 晚饭 formal、回屋提醒 formal、自由活动开场 formal 继续守住 one-shot
3. 时间口径继续对齐用户要求：
   - 进村后是下午
   - 回村晚饭后是晚上
4. 自由活动之后，玩家必须能回房间和床交互
5. 床交互后必须能进入第二天/日终收尾语义

如果其中任何一段只剩结构没体验，你就继续收，不要提前说“主链差不多了”。

---

## 第五部分：原生 resident 和 one-shot 口径继续守到底

你不允许回退到旧路线。

必须继续守住：

1. 已消费正式剧情不能重播
2. 已完成任务不能再装成正式入口
3. `Town` resident 默认是 scene-owned native resident
4. 不再默认创建 `Town_Day1Residents` 这种运行时假常驻容器
5. live summary / runtime summary 要尽量看得出“当前消费的是哪个 scene resident”

---

## 第六部分：把 demo 启动卡顿也作为这轮 own 收口的一部分

你不能只管剧情链，不管 demo 一开场就卡死。

当前分工固定为：

1. `PersistentPlayerSceneBridge.Start()` 这条 scene-rebind / persistence 启动峰值，交给 `存档系统` 协作接
2. `NavGrid2D.RebuildGrid()` 这条整图重建峰值，你自己必须继续查和收
3. `NavGrid2DStressTest` 这类测试脚本如果仍混在 demo runtime 里，必须处理掉

目标不是做完整性能治理，而是把 demo 级别最肉眼可感的启动大卡顿收窄。

---

## 第七部分：本轮的外部协作只按这三个方向吃回

1. `存档系统`
   - 吃 resident 持久态第一版 contract + `PersistentPlayerSceneBridge` 启动卡顿责任面
2. `NPC`
   - 吃 scripted control / resident runtime snapshot contract
3. `UI`
   - 只吃打包字体链结果

除了这 3 条，不要再把主线甩回别人。

---

## 第八部分：只有两种情况允许停

### 情况 A：已经到用户可验的 baseline

用户至少可以开始验：

1. `Town` 开场
2. `001/002` 引路进 `Primary`
3. `Primary` 治疗 + 工作台桥接
4. 回村晚饭
5. 睡觉收尾
6. one-shot 不重播

### 情况 B：撞上非常具体的外部阻断

必须具体到：

1. 哪个文件/哪条链
2. 为什么你自己跨不过去
3. 需要谁回球
4. 回球后你下一刀落哪

不允许：

1. “我先停个安全点”
2. “这刀技术上成立了”
3. “后面等有空再补”

---

## 最终汇报必须显式写清

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么
7. 为什么现在能验 / 为什么还不能验

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
