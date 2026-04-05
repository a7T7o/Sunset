# 2026-04-05｜给典狱长｜Town 锚点升级条件复勘与最小 runtime-contract 触点

这轮不再回头重裁 `spring-day1 / Town` 的导演边界。

当前唯一主线是：

- 把 `Town` 继续推进到我在不擅自改生产 scene / 活代码前能做到的最深处
- 把“下一次真升级到底卡在哪个最小 contract”钉死

---

## A. 用户可读层

### 1. 当前主线

`Town` 现在已经不是“还能不能被导演层消费”的问题。

`spring-day1` 的 `10 / 11 / 12` 已经把：

1. 分场
2. 群像矩阵
3. phase 承载边界
4. runtime 落位顺序

都写到了可直接消费的程度。

所以这轮我继续往下压后，`Town` 当前最真实的任务已经变成：

1. 把各 anchor 的最新承接等级重新校正
2. 把过时 blocker 从新版真值里剔掉
3. 把“下一次真 runtime 承接要改哪几个触点”写死

### 2. 这轮新的关键变化

这轮最大的变化有 3 个：

1. 旧的 `scene-build compile red` 不再站着了  
   - fresh `status` 下，当前 `Town` 编辑态 console 是空的
2. `Town` 当前第一 blocker 不再是“外线还在爆红”  
   - 而是 `semanticAnchorId` 已进导演层，但 runtime 仍然吃老 `Primary` 锚
3. 这意味着 `Town` 现在最前面的 gap 已从“场景基础设施”升级成“运行时承接 contract”

### 3. 当前最真实的 first blocker

当前 first blocker 已重写为：

- `SpringDay1NpcCrowdDirector` 仍然把 crowd runtime 绑定在 `Primary + anchorObjectName` 这一套旧 contract 上

更直白地说：

1. `Town` 的 `EnterVillageCrowdRoot / KidLook_01 / DinnerBackgroundRoot / NightWitness_01 / DailyStand_01~03` 都已经有正式语义
2. `StageBook` 和 `Manifest.semanticAnchorIds` 也已经开始引用它们
3. 但 runtime 真落点仍然只认 `Manifest.anchorObjectName = 001|NPC001 / 002|NPC002 / 003|NPC003 ...`
4. 而这些旧锚在 `Primary` 有，在 `Town` 没有

所以当前真正第一刀，不是再补边界文档，而是补这层 contract。

### 4. Town 各 anchor 最新承接等级

这轮我把它们压成两条轴：

- `导演承接等级`
  - `D2` = 可写语义
  - `D3` = 可写分场 / 群像矩阵
  - `D4` = 可被正式正文直接消费
- `runtime 承接等级`
  - `R1` = 只有 scene 锚点
  - `R2` = 已进入数据 cue，但 runtime 未真正使用
  - `R3` = 已具最小 contract 候选资格
  - `R4` = 可正式 live 消费

| anchor | 导演承接等级 | runtime 承接等级 | 当前判断 | 下一步升级条件 |
| --- | --- | --- | --- | --- |
| `EnterVillageCrowdRoot` | `D4` | `R2` | 当前第一优先锚点；正文、矩阵、落位顺序都已写死 | 先补 `semanticAnchorId -> runtime spawn` contract |
| `KidLook_01` | `D4` | `R2` | 单点观察位语义很稳，但仍未进真实 runtime 查找链 | 跟随 `EnterVillageCrowdRoot` 同一 contract 升级 |
| `DinnerBackgroundRoot` | `D4` | `R2` | 晚餐背景层已可正式消费 | 先等 crowd contract 打通，再进多人背景 live |
| `NightWitness_01` | `D4` | `R2` | 夜间见闻层已可导演消费 | 先等夜间 runtime 入口和 crowd contract 对齐 |
| `DailyStand_01` | `D3` | `R2` | 次日生活位已具语义与数据 cue，但不是当前第一刀 | 前面 4 个锚点至少过 1 个 live 承接样本 |
| `DailyStand_02` | `D3` | `R2` | 同上 | 同上 |
| `DailyStand_03` | `D3` | `R2` | 同上 | 同上 |

### 5. 哪个 anchor 最先值得升级

仍然是：

- `EnterVillageCrowdRoot`

原因现在更明确了：

1. 它是 `Town` 从“导演可消费”迈向“runtime 可承接”的第一块试金石
2. 它能最早检验 `semanticAnchorId` 这条新语义链是否真的被 runtime 吃到
3. 一旦它过线，`KidLook_01` 基本是同 contract 跟进，`DinnerBackgroundRoot / NightWitness_01` 也会更容易判断

### 6. 当前最小 runtime-contract 触点

如果后续允许继续深推，当前最小且最值钱的 runtime-contract 触点不是 scene，而是下面两个代码点：

1. [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
   - `PrimarySceneName = "Primary"`
   - `ResolveSpawnPoint(...)`
   - `FindAnchor(string anchorObjectName)`
2. [SpringDay1NpcCrowdManifest.asset](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset)
   - `anchorObjectName` 仍是旧 `Primary` 锚
   - `semanticAnchorIds` 已经开始写 `Town` 锚

当前最小升级思路应是：

1. runtime 先优先尝试当前 beat / 当前 scene 对应的 `semanticAnchorId`
2. 找不到时再回退到旧的 `anchorObjectName`
3. 最后才回退 `fallbackWorldPosition`

而不是：

1. 继续只用 `anchorObjectName`
2. 或者先去 `Town` 里硬做一套旧锚名别名层

### 7. 现在还剩哪些次级 blocker

在 first blocker 从 compile red 改写后，当前次级 blocker 变成这 3 类：

1. `Town` live 噪声仍未完全消失  
   - `导航检查V2` 最新 memory 里仍有 `Unknown script / OcclusionManager` 外部噪声记录
2. `DialogueUI / 中文字体链` 仍未被我 own 线重新 fresh 复核成 `Town` 可忽略项
3. `PersistentManagers` 在编辑态仍出现过 `DontDestroyOnLoad` 非 play 调用异常  
   - 当前它不是 `Town` first blocker
   - 但仍是需要后续继续定性的编辑态噪声

### 8. 哪些线程现在还值得继续

按我这轮复勘后的口径：

#### `继续 but 不是我来替他写`

1. `spring-day1`
   - 仍可继续把导演层与 stage book 往前推
   - 但别假设 `Town` crowd runtime 已 ready
2. `导航检查V2`
   - 后面仍值得回到 `Town` 抓正式场景 live
   - 但应排在 runtime contract 之后

#### `暂时不用因 Town 再催`

1. `scene-build-5.0.0-001`
   - 之前的 compile red 当前已不再是 fresh blocker
2. `UI`
   - 当前不是第一刀
   - 除非后续 live 再次证明它在 `Town` 里直接挡路
3. `019d4d18-bb5d-7a71-b621-5d1e2319d778`
   - `Town` 相机链已经按用户实测收口

### 9. 我这轮实际推进到了哪一层

这轮已经把 `Town` own 治理从：

- “还能不能消费 Town”

推进到了：

- “下一次真 runtime 升级最小该改哪 2 个 contract 触点”

但这轮还没有做的，我也必须报实：

1. 没有去改 `Town.unity / Primary.unity`
2. 没有去改 `spring-day1` 活代码
3. 没有去伪装成 `Town` 已经 runtime-ready

---

## B. 技术审计层

### 1. 本轮读取依据

1. [2026-04-05_给典狱长_spring-day1与Town导演协同全景说明_06.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/2026-04-05_给典狱长_spring-day1与Town导演协同全景说明_06.md)
2. [2026-04-05_给典狱长_导演正文同步与Town后续承接提示_07.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/2026-04-05_给典狱长_导演正文同步与Town后续承接提示_07.md)
3. [2026-04-05_给典狱长_Town各anchor可承接等级表_升级条件与剩余blocker推进图_08.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/2026-04-05_给典狱长_Town各anchor可承接等级表_升级条件与剩余blocker推进图_08.md)
4. [2026-04-05_spring-day1_导演分场与Town承接脚本_10.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/2026-04-05_spring-day1_导演分场与Town承接脚本_10.md)
5. [2026-04-05_spring-day1_NPC剧本走位与群像层矩阵_11.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/2026-04-05_spring-day1_NPC剧本走位与群像层矩阵_11.md)
6. [2026-04-05_spring-day1_阶段承载边界与后续runtime落位清单_12.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/2026-04-05_spring-day1_阶段承载边界与后续runtime落位清单_12.md)
7. [SpringDay1DirectorStageBook.json](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json)
8. [SpringDay1NpcCrowdManifest.asset](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset)
9. [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
10. `py -3 scripts/sunset_mcp.py status`
11. `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 10`
12. `Show-Active-Ownership.ps1`
13. [导航检查V2/memory_0.md](/D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/导航检查V2/memory_0.md)

### 2. 新证据摘要

1. fresh `status`：
   - `active_scene = Assets/000_Scenes/Town.unity`
   - `error_count = 0`
   - `warning_count = 0`
2. `errors`：
   - `errors=0 warnings=0`
   - 但返回了两条 `Exception` 记录
   - 其中更值得盯的是：
     - `[PersistentManagers] DontDestroyOnLoad can only be used in play mode`
3. `StageBook` 已大量写入：
   - `EnterVillageCrowdRoot`
   - `KidLook_01`
   - `DinnerBackgroundRoot`
   - `NightWitness_01`
   - `DailyStand_01~03`
4. `Manifest` 现状是：
   - `semanticAnchorIds` 已写 `Town` 锚
   - 但 `anchorObjectName` 仍是旧 `Primary` 锚名链
5. `CrowdDirector` 现状是：
   - `PrimarySceneName = "Primary"`
   - `ResolveSpawnPoint()` 只吃 `entry.anchorObjectName`
   - `FindAnchor()` 只按老锚名去找 GameObject
6. [Town.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 当前确实存在：
   - `Town_Day1Carriers`
   - `EnterVillageCrowdRoot`
   - `KidLook_01`
   - `DinnerBackgroundRoot`
   - `NightWitness_01`
   - `DailyStand_01~03`
7. [Primary.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Primary.unity) 当前仍存在：
   - `001`
   - `002`
   - `003`

### 3. 本轮最终裁定

1. `Town` 当前 first blocker：
   - `semanticAnchorId -> runtime spawn` contract 未打通
2. `Town` 当前第一升级锚点：
   - `EnterVillageCrowdRoot`
3. 当前最值得继续的下一层：
   - 不是再写边界文档
   - 而是在拿到授权后补 `SpringDay1NpcCrowdDirector + Manifest` 的最小 contract 升级

