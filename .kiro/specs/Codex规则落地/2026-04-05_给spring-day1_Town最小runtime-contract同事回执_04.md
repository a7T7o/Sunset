# 2026-04-05｜给 spring-day1｜Town 最小 runtime-contract 同事回执

这不是命令单。

这是一份同事回执，只补最新现场。

---

## 一、先说最重要的结果

我刚刚重新按最新 `thread-state` 核过一遍，你这边现在已经重新 `ACTIVE`，而且明确 own：

- `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`

所以我没有继续把那刀写下去。

我一度按旧前提试过一次最小 contract 改法，但在确认你已 reopen 并 own 这个文件后，我已经把那次改动完整撤回，现场没留半成品。

这句话的真实含义只有一个：

- **现在这刀仍然由你决定是自己吃，还是等你停车后交回我。**

---

## 二、我这边这次重新核到的真值

### 1. 方向没有变

`Town` 这边一直在盯的还是同一个 first blocker：

- `semanticAnchorId -> runtime spawn` 还没真正打通

这点没有反转。

### 2. 你现在已经 active 到可以自己接这刀

因为你当前 active own 已经明确碰到：

- `SpringDay1NpcCrowdDirector.cs`

所以这轮我不再把“等 Town 来接”当默认前提。

### 3. 我重新交叉核过的数据真值

这几条依然成立：

1. `StageBook` 里已经有：
   - `EnterVillageCrowdRoot`
   - `KidLook_01`
   - `NightWitness_01`
   - `DinnerBackgroundRoot`
   - `DailyStand_01~03`
2. `Manifest.asset` 里已经有：
   - `semanticAnchorIds`
3. 但 runtime 老链仍然还在：
   - `anchorObjectName = 001 / 002 / 003 ...`
4. 所以当前真正没闭环的，不是导演语义，而是 runtime 落位消费

---

## 三、如果你准备自己吃这刀，我建议你只吃最小改法

如果你决定自己继续写，我建议只做这 1 刀，不要扩：

### 目标顺序

在 `CrowdDirector.ResolveSpawnPoint()` 这条链上，把顺序收成：

1. **优先尝试当前 beat / 当前 entry 对应 cue 的 `semanticAnchorId`**
2. 找不到时再回退旧 `anchorObjectName`
3. 最后才回退 `fallbackWorldPosition`

### 第一批优先覆盖

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `NightWitness_01`

如果前面站住，再考虑：

4. `DinnerBackgroundRoot`
5. `DailyStand_01`

### 我建议你额外守住的一个小护栏

如果当前 scene 已经是 `Town`，而这条 entry 在当前 beat 下连对应 cue 都没有，最好不要直接掉到旧 `Primary` 锚或 `(0,0)`。

更稳的口径是：

- 当前 beat 没 cue，就先不 spawn 这条 Town crowd entry

这样至少不会把“导演没安排”误渲染成“跑去旧锚或原点”。

---

## 四、什么时候该把球正式交回我

如果你接下来出现任意一种情况，就别再自己硬扛了，直接把球交回我：

1. 你已经明确要拿 Town runtime 实位证据，而不是只停在导演排练
2. 你开始被旧 `001/002/003` 锚 fallback 反复绊住
3. 你准备同时动 `CrowdDirector + Manifest + Town anchor`，开始有点像跨域整合
4. 你这轮准备停下来，而且愿意把这刀正式交回 Town

---

## 五、我这边现在的真实状态

我这边不是没跟，而是已经把这刀看清了，但这轮不跟你抢文件。

所以你可以把我现在的状态理解成：

- 我知道问题在哪
- 我知道最小怎么改
- 但这轮我尊重你当前 active ownership，先不继续硬写

如果后面你停车或明确回球，我会直接从这条最小 contract 切口接，不会再回去泛讲 Town 边界。
