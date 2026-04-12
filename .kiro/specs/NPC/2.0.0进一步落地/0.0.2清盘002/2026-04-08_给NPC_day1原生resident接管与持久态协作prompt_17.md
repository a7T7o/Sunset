# 2026-04-08｜给 NPC｜Day1 原生 resident 接管与持久态协作 prompt 17

请先完整读取：

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_阶段安全点回执_16.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_NPC_存档边界回执_02.md`
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-08_spring-day1_Day1最终闭环主控清单_30.md`

从这一条开始，不要回去补 `runtime spawn`，也不要漂去 `Town/Primary scene writer`。

当前唯一主刀固定为：

给 `Day1` 和 `存档系统` 提供一份真正能用的 `native resident` runtime contract，让 scene-owned resident：

1. 被导演接管时不会乱跑、乱打招呼、乱回旧逻辑
2. 在需要持久化时有最小可序列化的 resident snapshot surface

---

## 你必须继承的真状态

1. 你已经把 resident 内容层和 formal fallback 继续补厚了
2. 你已经明确不该再回吞：
   - `runtime resident deployment`
   - `Town` scene 落位
   - `CrowdDirector` 主消费逻辑
3. 用户当前新的真实抱怨是：
   - NPC 会被别的引线拉走
   - 导演接管不彻底
   - 场景切换后 resident 容易回到最原始状态

也就是说，现在最值钱的不是继续加内容，而是让 `Day1` 真能稳稳消费原生 resident。

---

## 这轮唯一主刀

### 把 `native resident scripted control contract` 收出来

完成定义至少包括：

1. 当 `Day1` 或导演工具接管某个 scene resident 时：
   - 自动漫游停掉
   - 自动打招呼停掉
   - nearby / selfTalk / walkAway 不再抢控制权
2. 接管释放后，resident 能按合法方式回到常驻逻辑
3. 这套 contract 不依赖 runtime 假 resident

---

## 强绑定支撑目标

### 给存档系统暴露 resident 最小 snapshot surface

你不需要自己实现完整 save/load，但要把这层面暴露清楚。

至少要让外部可读/可写：

1. resident stable key / identity
2. 当前是否被脚本接管
3. 当前合法的 scene/group/anchor 语义
4. 恢复后足以避免掉回编辑态初始逻辑的最小 resident state

不要把 typing、气泡句子、当前 nearby 文案这类过程态塞进去。

---

## 明确禁止事项

1. 不要继续补 `runtime spawn`
2. 不要改 `Town.unity / Primary.unity`
3. 不要主刀 `SpringDay1NpcCrowdDirector`
4. 不要回吞 `spring-day1` 导演主线
5. 不要把“只要关一个 roam 开关”冒充成完整接管 contract

---

## 这轮最该落的文件层

优先围绕你 own 的 resident/runtime 面：

1. `NPCAutoRoamController`
2. resident 相关 interaction / nearby / bubble 入口的 scripted control gating
3. 能承载 resident runtime snapshot 的最小数据面
4. 对应 tests / validation / probe

如果需要和 `spring-day1` 或 `存档系统` 对一个接口，请明确接口名和责任边界，不要只写概念说明。

---

## 完成定义

这轮不是“我觉得 day1 应该更好接了”，而是要让主控台一眼看到：

1. `Day1` 怎样一键/明确地接管原生 resident
2. 为什么 resident 不会再被别的逻辑拉走
3. `存档系统` 能拿哪份 snapshot 面去持久化
4. 哪些 resident 状态仍然不能也不该持久化

---

## 回执时必须写清

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要主控台现在做什么
7. 这轮新增的 resident contract 名称 / 用法 / 限制

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

