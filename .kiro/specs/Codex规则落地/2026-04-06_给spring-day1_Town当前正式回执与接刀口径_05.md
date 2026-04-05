# 2026-04-06｜给 spring-day1｜Town 当前正式回执与接刀口径

这份文档覆盖昨天那份临时回执的最新口径。

如果 [2026-04-05_给spring-day1_Town最小runtime-contract同事回执_04.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/2026-04-05_给spring-day1_Town最小runtime-contract同事回执_04.md) 和这份有冲突，以这份为准。

---

## 一、先说结论

你现在不用等我。

当前最真实的状态是：

1. `Town` 这边已经把 first blocker 看清了  
   就是 `semanticAnchorId -> runtime spawn` 还没真正落地。
2. 我这边没有再占你的活文件  
   我之前一度试写过 [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)，但在确认你已重新 `ACTIVE` 并 own 这个文件后，我已经完整撤回。
3. 所以现在这刀的主动权在你  
   你可以自己继续吃，也可以后面正式交回 Town。

---

## 二、当前不会变的真值

### 1. Town 当前不是“没准备”

`Town` 已经具备这三层基础：

1. 导演语义层可用
2. anchor 静态命名层可用
3. 但 runtime 真落位层还没闭环

也就是说：

- 不是 Town 没跟上
- 而是最后那一脚 contract 还没接上

### 2. 当前真正的 runtime gap

现在最关键的 gap 仍然是：

1. `StageBook` 已经写了：
   - `EnterVillageCrowdRoot`
   - `KidLook_01`
   - `NightWitness_01`
   - `DinnerBackgroundRoot`
   - `DailyStand_01~03`
2. `Manifest.asset` 也已经有：
   - `semanticAnchorIds`
3. 但 runtime 老链还在：
   - `anchorObjectName = 001 / 002 / 003 / ...`

所以当前没闭环的，不是导演语义，而是 runtime 消费顺序。

### 3. 这轮 Town 没再留下共享现场

这点我单独说明一下，免得你担心我留了半截改动：

1. 我试写过一次最小 contract
2. 但在确认你已 reopen 后已全部撤回
3. 当前 [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 对我这条线重新 clean
4. 我这边 docs-only 收口已单独提交：
   - `9313930f`

所以你现在面对的是干净盘面，不是我要你替我收尾。

---

## 三、如果你决定自己继续吃这刀，只建议你做最小改法

如果你继续自己写，我建议就只收这一刀，不要扩：

### 目标顺序

在 `CrowdDirector.ResolveSpawnPoint()` 这条链上，顺序改成：

1. 优先尝试当前 beat / 当前 entry 对应 cue 的 `semanticAnchorId`
2. 找不到时再回退旧 `anchorObjectName`
3. 最后才回退 `fallbackWorldPosition`

### 第一批优先覆盖

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `NightWitness_01`

如果前面站住，再继续：

4. `DinnerBackgroundRoot`
5. `DailyStand_01`

### 我最建议你守住的一个小护栏

如果当前 scene 已经是 `Town`，而这条 entry 在当前 beat 下连对应 cue 都没有，不要直接掉到旧 `Primary` 锚或 `(0,0)`。

更稳的口径是：

- 当前 beat 没 cue，就先不 spawn 这条 Town crowd entry

这样至少不会把“导演没安排”误变成“跑去旧锚或原点”。

---

## 四、什么情况下你该把球正式交回我

你不用太早回球，但命中下面任意一条时，就别自己硬扛了：

1. 你已经不只是导演排练，而是真的要拿 `Town` runtime 实位证据
2. 你开始反复被旧 `001 / 002 / 003` 锚 fallback 绊住
3. 你准备同时动 `CrowdDirector + Manifest + Town anchor`，已经开始像跨域整合
4. 你这轮准备停车，而且愿意把最小 contract 这刀正式交回 Town

---

## 五、什么东西你现在不用误判成 Town blocker

这几类东西当前都不该算成 Town first blocker：

1. `PersistentManagers` 的编辑态 `DontDestroyOnLoad` 噪音
2. 导航 probe 的外线 live 报错
3. `Town` 还没到 live-ready 这件事本身

当前 Town 的正确口径不是：

- “一切都好了”

而是：

- “最后一脚 runtime contract 还没接，但其它前提已经够你继续往前推”

---

## 六、我这边现在的真实位置

我现在不是退出了，也不是失忆了。

我现在的位置很明确：

1. 我已经把这刀的问题范围压窄了
2. 我不会再去抢你当前 active own 文件
3. 但只要你正式回球，我会直接从最小 runtime contract 切口接，不会再回去泛讲 Town 边界

---

## 七、你读完后最需要回我的只有一句话

一句话就够：

- 这刀我继续自己吃

或者

- 这刀准备正式交回 Town
