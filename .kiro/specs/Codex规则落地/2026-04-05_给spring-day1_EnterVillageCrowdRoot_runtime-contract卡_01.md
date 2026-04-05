# 2026-04-05｜给 spring-day1｜EnterVillageCrowdRoot runtime contract 卡

这不是让你停手等我的命令单。

这是我这边把 `EnterVillageCrowdRoot` 再往下压一层之后，给你的一个更窄、更能直接消费的协作结论。

最短版先说死：

- `EnterVillageCrowdRoot` 现在已经不是“没有这个锚点”的问题；
- 它当前的问题是：`导演语义 + 数据 cue` 已经成立，但 `Town scene anchor -> runtime 真落位` 这条 contract 还没有接上。

所以你现在继续写导演层是安全的；

但如果你下一轮要开始真吃 runtime，请不要默认它已经是可直接消费的 live anchor。

---

## 一、我这边当前拿到的现场真值

## 1. `Town` 里确实已经有 `EnterVillageCrowdRoot`

当前 [Town.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 里已经有：

- `SCENE/Town_Day1Carriers/EnterVillageCrowdRoot`

同一组下面还有：

1. `KidLook_01`
2. `DinnerBackgroundRoot`
3. `NightWitness_01`
4. `DailyStand_01`
5. `DailyStand_02`
6. `DailyStand_03`

也就是说：

- 这批名字不是空想出来的
- 它们已经作为 scene 里的正式对象存在

## 2. 但它当前还是“空锚点”

我这边继续往下读到的结果是：

1. `Town_Day1Carriers` 自己只有 `Transform`
2. `EnterVillageCrowdRoot` 自己也只有 `Transform`
3. 当前没有子节点
4. 当前没有额外组件
5. 当前本地位置还是 `0,0,0`

同一批其他锚点也一样，当前基本都是：

- 只有名字
- 没有结构
- 没有组件
- 没有进一步的 runtime 信息

所以它目前更像：

- `scene 里的正式占位锚`

还不是：

- 已经摆好位置并能直接被 runtime 吃掉的 live anchor

---

## 二、`day1` 现在到底是怎么消费它的

## 1. 正文 / 导演层已经在正式消费它

你这边当前已经把它写进了：

1. [2026-04-05_spring-day1_导演分场与Town承接脚本_10.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/2026-04-05_spring-day1_导演分场与Town承接脚本_10.md)
2. [2026-04-05_spring-day1_NPC剧本走位与群像层矩阵_11.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/2026-04-05_spring-day1_NPC剧本走位与群像层矩阵_11.md)
3. [2026-04-05_spring-day1_阶段承载边界与后续runtime落位清单_12.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/2026-04-05_spring-day1_阶段承载边界与后续runtime落位清单_12.md)
4. [2026-04-05_spring-day1_导演线后续任务清单与轻量导演工具路线图_13.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/2026-04-05_spring-day1_导演线后续任务清单与轻量导演工具路线图_13.md)

所以在导演层，它已经是：

- 真锚点
- 真优先级第一位
- 真后续 runtime 候选

## 2. 数据层也已经在消费它

当前 [SpringDay1NpcCrowdManifest.asset](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset) 里：

1. NPC `101`
2. NPC `103`

都已经把 `EnterVillageCrowdRoot` 写进了 `semanticAnchorIds`。

而 [SpringDay1DirectorStageBook.json](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json) 里：

- `EnterVillage_PostEntry`

也已经把它写进 `actorCues.semanticAnchorId`。

所以现在成立的是：

- `导演语义 -> 数据 cue`

这条链已经有了。

## 3. 但 runtime 真落位还没有吃它

当前真正负责 runtime 群像落位的，还是：

- [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)

而它现在有两条更老的 contract：

### A. 它只在 `Primary` 场景工作

里面写死了：

- `private const string PrimarySceneName = "Primary";`

并且：

1. `Update()`
2. `HandleActiveSceneChanged()`
3. `GetOrCreateState()`

都只接受 `Primary`。

也就是说：

- 当前这套 crowd runtime 还没有切到 `Town`

### B. 它现在真正吃的不是 `semanticAnchorId`

当前 runtime 出生点解析走的是：

- `entry.anchorObjectName`

也就是 [SpringDay1NpcCrowdManifest.asset](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset) 里那些旧名字：

1. `001|NPC001`
2. `002|NPC002`
3. `003|NPC003`
4. 以及 `Bed / PlayerBed / HomeDoor / House 1_2` 这种旧 fallback 链

而不是：

- `EnterVillageCrowdRoot`

当前 `semanticAnchorId` 的作用更像：

- 让 `StageBook` 知道“这条 cue 是给谁的 / 属于哪类语义锚”

不是：

- 去 `Town` 场景里真正找哪个 `Transform` 来落位

---

## 三、为什么这件事现在会卡住

当前真正的卡点不是一个，而是 3 个连在一起：

## 1. `Town` 锚点现在还是空壳

`EnterVillageCrowdRoot` 当前只有空 `Transform`，没有进一步的 scene 落位信息。

## 2. runtime 还没切到 `Town`

`SpringDay1NpcCrowdDirector` 目前还是 `Primary-only`。

## 3. runtime 出生锚仍然沿用 `Primary` 老锚名

当前 `Manifest` 里的 `anchorObjectName` 还是：

1. `001|NPC001`
2. `002|NPC002`
3. `003|NPC003`

而我这边继续核对后已经确认：

- 这些名字在 [Primary.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Primary.unity) 里存在
- 但在 [Town.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 里并不存在同名运行锚

这意味着：

- 如果后面不先补 contract，就直接把这套 crowd runtime 硬切到 `Town`

最危险的结果不是“只是还没那么漂亮”，而是：

- 它会找不到旧锚
- 最后退到 `fallbackWorldPosition = 0,0`
- 然后把本该围观的群像直接丢到错误位置

---

## 四、我现在给你的更准确等级

我不再用之前那张更宽的 `L1~L5` 治理表来描述它。

对你当前最有用的，是这张更窄的 `runtime-readiness` 口径：

- `R0`：只有名字
- `R1`：scene 里已有锚点占位
- `R2`：导演语义 + 数据 cue 已成立，但 runtime 还没吃 scene anchor
- `R3`：runtime 已能解析 scene anchor 并安全落位
- `R4`：live 承接闭环已过

当前 `EnterVillageCrowdRoot` 我给它的判断是：

- `R2`

理由是：

1. `Town` scene 里已经有正式占位锚
2. 导演正文和 `StageBook` 都已经正式消费它
3. 但 runtime 仍然没有通过它来解 scene 实位

所以它现在不是：

- “还什么都没有”

也不是：

- “已经 live ready”

而是很明确地卡在：

- `语义和数据已经站住，但 runtime contract 还没切过来`

---

## 五、我这边对下一步的最小建议

我不建议你现在回头停写导演层。

你现在更应该把我这份卡理解成：

- 后面只要你开始要吃 runtime，就按这个 contract 接就行

### 当前最合理的最小顺序

#### 第一步

先承认：

- `EnterVillageCrowdRoot` 当前不是最终落位点
- 它是 `Town` 里的正式语义锚

#### 第二步

后面真的要接 runtime 时，先补清楚这条 contract：

1. `semanticAnchorId -> Town scene transform` 如何解析
2. 什么时候继续允许旧的 `anchorObjectName` 作为 fallback
3. `Primary-only` 的 crowd runtime 何时扩成 `Primary/Town`

#### 第三步

再进入 live 验收：

我建议第一组最小 live 验收只抓：

1. 一个围观型 NPC
2. 一个儿童观察型 NPC
3. `EnterVillage_PostEntry`

只要这组先过，后面：

1. `KidLook_01`
2. `DinnerBackgroundRoot`
3. `NightWitness_01`

都会更顺。

---

## 六、如果后面真的要改 scene / runtime，我这边当前的五段式分析

### 1. 原有配置

1. [Town.unity](/D:/Unity_learning/Sunset/Assets/000_Scenes/Town.unity) 里已经有 `SCENE/Town_Day1Carriers/EnterVillageCrowdRoot`
2. 但它当前只有一个空 `Transform`
3. [SpringDay1NpcCrowdManifest.asset](/D:/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset) 里的 runtime 出生锚仍然是 `001|NPC001` 这类 `Primary` 旧锚
4. [SpringDay1NpcCrowdDirector.cs](/D:/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 当前只在 `Primary` 场景工作

### 2. 问题原因

当前断点不在“有没有这个 anchor”，而在：

1. `semanticAnchorId` 只接到了导演数据层
2. runtime 解析仍没切到 `Town` scene anchor
3. 一旦硬切到 `Town`，旧 `anchorObjectName` 找不到时很可能直接回退到 `fallbackWorldPosition = 0,0`

### 3. 建议修改

如果未来你和我都同意进入真实修改，我建议顺序固定为：

1. 先明确 `semanticAnchorId -> Town scene transform` 的解析 contract
2. 再决定 `Primary-only` crowd runtime 是否扩成 `Primary/Town`
3. 再给 `EnterVillageCrowdRoot` 做真实场内落位，而不是先盲改 `DinnerBackgroundRoot / NightWitness_01`

### 4. 修改后效果

如果按这个顺序推进，`EnterVillage_PostEntry` 才会第一次真正具备：

1. 导演语义
2. 数据 cue
3. scene 实位
4. runtime 真落位

这四层合一的条件。

### 5. 对原有功能的影响

正确做法不会强拆你现在已经能跑的 `Primary` 旧链，而应该：

1. 保留旧 `anchorObjectName` 作为 fallback
2. 先让 `Town` 的 `EnterVillageCrowdRoot` 成为更高优先级的新 contract
3. 等第一组 live 承接过线后，再决定要不要继续外溢到其他锚点

---

## 七、如果你问我“Town 现在最该给我什么”

我的答案现在已经比上一轮更窄了：

不是：

- 再给你一份 `Town` 总边界说明

而是：

- 给你一份 `EnterVillageCrowdRoot runtime contract`

因为你现在最缺的，已经不是语义真值，而是：

- 当你开始真吃 runtime 时，到底哪一层已经成立，哪一层还没成立

---

## 八、给你的最短版

最短版只有一句：

- `EnterVillageCrowdRoot` 现在已经有 scene 占位、导演正文和数据 cue，但 crowd runtime 还没有真正按它在 Town 里落位；你继续写导演层没问题，但下一轮只要开始吃 runtime，就先按这张 contract 卡来接，不要默认它已经是 live anchor。`
