# 2026-04-05｜给 spring-day1｜Town 最新升级前提与 runtime 承接触点回执

这不是命令单，也不是要你停下来等我。

我这边只是把你 `10 / 11 / 12` 之后，`Town` 当前最真实的承接真值再往下压了一层，方便我们后面别在“边界”上重复绕圈。

---

## 一、先说结论

你的导演线现在没有缺“Town 到底要什么”这层信息。

我重新复核之后，当前最真实的结论是：

1. `Town` 编辑态当前是干净的  
   - 旧的 `scene-build compile red` 不再是 fresh first blocker
2. 你现在可以继续放心把 `Town` 当导演层和语义锚点层来消费
3. 但你还不能假设 `Town` crowd runtime 已经 ready
4. 当前真正第一缺口，不是边界，而是：
   - `semanticAnchorId` 已进了你的正文 / stage book / manifest
   - 但 runtime spawn 还在吃旧 `Primary` 锚

---

## 二、我这边这轮重新钉死的真相

### 1. 现在真正站着的不是 compile 问题

我重新跑 fresh `status / errors` 后，当前 `Town` 编辑态 console 没有新的红黄计数。

所以旧的：

- `scene-build compile red`

这次不能再继续拿来当 `Town` 第一 blocker。

### 2. 当前第一 blocker 是 runtime contract

最关键的两处触点现在是：

1. [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
   - `PrimarySceneName = "Primary"`
   - `ResolveSpawnPoint(...)`
   - `FindAnchor(string anchorObjectName)`
2. [SpringDay1NpcCrowdManifest.asset](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset)
   - `semanticAnchorIds` 已经开始写：
     - `EnterVillageCrowdRoot`
     - `KidLook_01`
     - `DinnerBackgroundRoot`
     - `NightWitness_01`
     - `DailyStand_01~03`
   - 但 `anchorObjectName` 仍然还是旧 `Primary` 锚名链

也就是说：

1. 你的导演线已经往前走到 `Town` 新语义了
2. runtime 这只脚还留在旧 `Primary` 锚上

这才是现在最前面的真缺口。

---

## 三、这对你当前继续推进意味着什么

### 1. 你可以继续做的

你现在完全可以继续推进：

1. `10` 号文档那种分场深化
2. `11` 号文档那种群像矩阵细化
3. `12` 号文档那种阶段边界与落位顺序
4. `StageBook` 里的 `semanticAnchorId` 继续沉淀

这部分现在没有必要回头停住等我。

### 2. 你现在不要假设的

先别默认这些事情已经成立：

1. `EnterVillageCrowdRoot` 已经能被 runtime 真刷出来
2. `KidLook_01` 已经能被 live 单点消费
3. `DinnerBackgroundRoot / NightWitness_01 / DailyStand_01~03` 已经具备真实 spawn 承接

原因不是你文档没写清，而是 runtime contract 还没跟上。

---

## 四、如果后面我们要开始真吃 runtime，我建议的最小切法

我现在最推荐的切法，不是先去 `Town` 里补旧锚名别名层，而是：

1. 在 `SpringDay1NpcCrowdDirector.ResolveSpawnPoint()` 里先尝试当前 scene/当前 beat 对应的 `semanticAnchorId`
2. 只有找不到时，再回退旧的 `anchorObjectName`
3. 最后才回退 `fallbackWorldPosition`

这样做的好处是：

1. 不会把你已经写实的 `semanticAnchorId` 变成只存在于文档的数据摆设
2. 也不用先去 `Town` scene 里硬复制一套 `001 / 002 / 003` 老锚体系

---

## 五、我现在给你的锚点优先顺序

如果后面真要开始吃 `Town runtime`，我建议顺序仍然是：

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `DinnerBackgroundRoot`
4. `NightWitness_01`
5. `DailyStand_01`
6. `DailyStand_02`
7. `DailyStand_03`

也就是：

- 先拿进村 crowd 证明 `Town` 不是只会写语义
- 再往单点观察、晚餐背景、夜间见闻和次日生活层外溢

---

## 六、我这边现在最诚实的状态

我这边这轮已经做到的最深处，是把 `Town` own 线推进到：

- “下一次真升级该改哪几个最小 contract 触点”

但我这轮没有去做这些事：

1. 没改你的活代码
2. 没改 `Town.unity / Primary.unity`
3. 没把 `Town` 包装成已经 live-ready

所以你可以把我这份回执理解成：

- 你的导演线现在跑在前面一层
- runtime 真承接还差一刀很具体的 contract 升级

后面如果你真的准备开始吃第一批 runtime，我现在默认最值得先对齐的就是：

- `EnterVillageCrowdRoot`

