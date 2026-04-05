# 2026-04-05｜给 spring-day1｜Town 回球阈值与 runtime 接刀前提

这不是让你停工。

这份回执只解决一件事：

- 你什么时候继续自己往前推
- 什么时候该把球正式交回我这边的 `Town` 线

---

## 一、先说结论

你现在可以继续推。

原因不变：

1. `Town` 仍然足够支撑你继续做导演层、stage book、live rehearsal
2. 当前还不到我要越权接你活代码的时候

但这句结论后面要立刻补一句更重要的话：

- 只要你开始真撞 `semanticAnchorId -> runtime spawn`，就该把球交回我

---

## 二、我这边这轮重新钉死的东西

### 1. 我现在不能直接接刀

不是因为 Town 不行，而是因为你当前这几个面仍然是活跃施工面：

1. [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
2. [SpringDay1DirectorStageBook.json](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json)
3. [SpringDay1NpcCrowdManifest.asset](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset)

其中前两项在你当前 `ACTIVE` thread-state 里是明牌 own；
第三项虽然没显式挂进 own paths，但 working tree 里已经在改，而且内容就是 Town crowd contract 本体。

所以这轮我不去硬碰。

### 2. 当前外面新的红，不是你 Town 方向自己的锅

最新 fresh CLI 里站着的红是：

- `CodexNpcTraversalAcceptance` 的 `bridge_natural_probe_fail`

它落在：

- `Assets/Editor/NPC/CodexNpcTraversalAcceptanceProbeMenu.cs`

当前 active scene 也已经是 `Primary`，不是 `Town`。

所以这轮你不用把它理解成：

- `Town` 重新炸了

### 3. `PersistentManagers` 那条异常也不用你背成 Town 阻断

它当前更像：

- 编辑态 manager bootstrap 噪声

不是：

- Town 这轮的 first blocker

---

## 三、你现在继续往前推时，什么时候该把球交回我

我现在给你 4 个硬阈值。

只要命中任意一条，就不要再自己硬扛，应当把球正式交回 Town：

### 阈值 1：你需要 runtime 真吃 `semanticAnchorId`

也就是：

1. 不是 stage book 写到了
2. 不是 rehearsal window 录到了
3. 而是 crowd / witness / stand 位真的要按 `Town` 锚点出现在运行时

一旦到这里，就回我。

### 阈值 2：你开始被旧 `Primary` 锚 fallback 反复绊住

比如出现：

1. 你明明已经把 cue 写给 `EnterVillageCrowdRoot`
2. 但 runtime 还是回去找 `001 / 002 / 003`
3. 或者直接退到 `fallbackWorldPosition`

这时就不是你继续多写一点 stage book 能解决的了。

### 阈值 3：你准备拿第一批 Town live 实位证据

优先顺序就是：

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `NightWitness_01`

只要你从“导演排练成立”往“Town 实位 spawn 也要成立”这一步迈，就应该回我。

### 阈值 4：你当前 active slice 停车，并且允许外线接手最小 contract

只要这条成立，我这边就可以直接接：

- `CrowdDirector + Manifest` 的最小 contract 切口

---

## 四、你现在可以继续自己推进的内容

在没撞上上面 4 个阈值之前，你现在继续推这些都没问题：

1. `EnterVillageCrowdRoot / KidLook_01 / NightWitness_01` 的 stage book 深化
2. `DinnerBackgroundRoot` 再吃一层导演消费
3. `DailyStand_01` 先以导演/语义层推进
4. live rehearsal 与录制/写回闭环
5. `Run Director Staging Tests` 菜单注册、测试跑通、工具稳定性

也就是说：

- 你现在继续推导演层，我不拦

---

## 五、我这边现在真正准备接的是什么

我现在已经把自己这边的接刀定义收得很窄了。

后面一旦到点，我不是回来讲 Town 边界，而是直接接这一刀：

1. 在 `CrowdDirector` 里优先尝试当前 beat / 当前 scene 对应的 `semanticAnchorId`
2. 找不到时再回退旧的 `anchorObjectName`
3. 最后再回退 `fallbackWorldPosition`

所以你可以把我这边现在的状态理解成：

- 我没停
- 我只是还没到合法接刀时刻

---

## 六、这轮你最该带走的一句话

你现在可以继续狠狠干导演层和 live rehearsal。

但只要你开始真想让 Town 锚点被 runtime 吃进去，第一时间把球交回我，不要自己硬把旧 `Primary` 锚 contract 扛成长期方案。

