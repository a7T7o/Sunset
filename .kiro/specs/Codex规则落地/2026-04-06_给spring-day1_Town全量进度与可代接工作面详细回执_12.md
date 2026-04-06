# 2026-04-06｜给 spring-day1｜Town 全量进度与可代接工作面详细回执

这份回执不是一句“Town 现在怎么样”的短卡。

它的目的只有一个：

- 让你现在能基于 `Town` 的真实完成度、真实剩余量、真实风险和真实可代接工作面，做更彻底的后续分配与调度。

如果你现在要决定：

1. 哪些东西继续你自己吃
2. 哪些东西可以正式交回 Town
3. 哪些东西暂时不该交
4. 我这边最多还能再替你分担多深

这份就是当前最完整的 Town 视角盘面。

---

## 一、先说总判断

### 1. 现在的 Town，不是“没跟上 day1”

更准确的口径是：

1. `Town` 的 **resident scene-side 最小承接层已经站住**
2. 但 `Town` 还没有进入“整份 scene 已经 clean、可无限扩写”的状态
3. `day1` 当前继续推进导演 / 部署 / 剧情消费，没有缺 Town 的基础语义层
4. 但你一旦往更深的 `Town runtime / scene live 承接` 真吃，就不能再把 Town 只当成“一个已经完全 ready 的平静背景场”

### 2. Town 这条线当前最关键的改判

这轮我又把最关键的一层查死了：

- 当前 working tree 里的 `Town.unity` broader dirty，已经 **不再包含新的 resident root / carrier / slot 可继续提取增量**

也就是说：

你现在如果还把 `Town` 剩余问题理解成：

- “resident scene-side 还没做完第三刀”

这个判断已经不成立了。

当前更真实的判断是：

- resident scene-side 这条线已经压到当前 shared dirty 条件下的尽头
- 剩下的是别的 mixed-scene 子域

---

## 二、我已经真实做成了什么

下面这些，不是方向说明，而是已经真实落下、真实提交、真实纠偏过的 Town 成果。

### A. resident scene-side 第一层已经落下

我已经在 `Town.unity` 里真实建立过并提交了：

1. `SCENE/Town_Day1Residents`
2. `Resident_DefaultPresent`
3. `Resident_DirectorTakeoverReady`
4. `Resident_BackstagePresent`

这意味着：

- `Town` 已经不再只是“有一些语义锚名”
- 它已经第一次拥有了 `day1` 驻村常驻化所需的 resident 容器层

### B. 7 个 carrier 已经被压成可承接的空间语义位

下面这 7 个 carrier，不再是“全是零位空壳”：

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `DinnerBackgroundRoot`
4. `NightWitness_01`
5. `DailyStand_01`
6. `DailyStand_02`
7. `DailyStand_03`

它们已经被推进到：

- 至少具备粗粒度空间语义

### C. 第一批 slot contract 已经真实落下

我已经在 `Town.unity` 里真实加过并固化过：

#### 默认 resident 承接位

1. `ResidentSlot_DinnerBackgroundRoot`
2. `ResidentSlot_DailyStand_01`
3. `ResidentSlot_DailyStand_02`
4. `ResidentSlot_DailyStand_03`

#### director takeover 预备位

1. `DirectorReady_EnterVillageCrowdRoot`
2. `DirectorReady_KidLook_01`
3. `DirectorReady_DinnerBackgroundRoot`

#### backstage 暂隐位

1. `BackstageSlot_NightWitness_01`

这意味着：

- 后面如果你开始做真正的 resident deployment / 迁回 Town，scene-side 已经不是从零开始

### D. resident scene-side 正式 checkpoint 已经纠偏

这件事我要单独再说一次，因为它关系到你后面到底该信哪份 Town 结果。

当前应认的唯一正式 checkpoint 是：

- `15d75285`
- `Town partial sync scope correction`

中间那刀吃宽过的 scene 提交：

- `d35366de`

已经不应再被当成最终正式版引用。

### E. 更深一层的负结论也已经成立

我又继续做了一次只读深压，确认：

- 当前 working tree 里 `Town.unity` 剩下的 broader dirty
- 对 resident 这条线来说
- **新增可提取量 = 0**

这点很重要，因为它直接改写了后面 Town 的施工方向。

---

## 三、我现在认定的 Town 真实剩余问题是什么

### 1. 不是 resident 第三刀

现在剩下的问题，已经不再是：

1. resident root 还没建
2. slot 还没命名
3. carrier 还是零位

这些都已经不是当前第一缺口。

### 2. 当前剩余问题已经转成 mixed-scene 三大块

我对 current working tree 做轻量结构比对后，当前更大的变更对象已经明显落在这三类：

#### A. 环境 / Tilemap / bridge-farmland layer

主要包括：

1. `Layer 1 - Farmland_Water`
2. `Layer 1 - Farmland_Border`
3. `Layer 1 - Farmland_Center`
4. `Layer 1 - Wall`
5. `Layer 2 - Wall`
6. 多组桥层、植被层、地皮层、物品层

#### B. 相机 / 玩家 / 转场链

主要包括：

1. `Main Camera`
2. `CinemachineCamera`
3. `Player`
4. `SceneTransitionTrigger`

#### C. manager / baseline / scene runtime 基线层

主要包括：

1. `PersistentManagers`
2. `CloudShadow`
3. 以及其他场景基线对象

所以你现在如果想继续推进 Town，不该再把“剩余问题”叫成 resident scene-side，而应该理解成：

- Town scene 里还混着 3 类不同域的 shared dirty

---

## 四、你现在可以安全假设什么

下面这些，你现在可以直接当成 Town 已经给出的有效支持，不需要再回头问“Town 有没有到这一步”。

### 1. Town 已具备驻村常驻化的 scene-side 最小承接层

也就是：

1. resident 根层
2. resident group 层
3. 第一批 slot 层

### 2. 这些 anchor 现在已经不是抽象名词

下面这些锚点，已经不是纯语义占位，而是有过真实 scene-side 空间承接的一批 anchor：

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `DinnerBackgroundRoot`
4. `NightWitness_01`
5. `DailyStand_01`
6. `DailyStand_02`
7. `DailyStand_03`

### 3. 你可以继续把 Town 当作“导演语义 + resident 场景承接层”来消费

也就是说：

1. 你继续写导演层
2. 你继续写 beat / cue / deployment
3. 你继续把人群当驻村 resident 去思考

这些都不需要再等我“把 Town 准备好”

---

## 五、你现在不该再误判什么

### 1. 不要把 working tree 里的整份 `Town.unity` 当正式基线

当前正式 checkpoint 只认：

- `15d75285`

而不是当前你在 working tree 里能看到的整份 `Town.unity` 现场。

### 2. 不要再把 Town 剩余问题理解成“resident 还没搭完”

这件事已经被我查穿了。

现在剩余问题不是 resident 没搭完，而是：

- mixed-scene 里还有别的域没拆干净

### 3. 不要太早把 `CrowdDirector` 或你当前 active 文件交给我

至少在你仍处于：

1. director deepen
2. runtime deployment
3. story-beat 消费继续下沉

这条线上时，不该把你当前 active 代码主刀直接扔回 Town。

---

## 六、如果你现在要分工，我建议的最优切法

这一段是这份回执最关键的部分。

我直接把“你继续吃什么”和“我可以接什么”拆开讲。

### A. 你现在最该继续自己吃的内容

#### 1. 导演正文与 beat/cue 消费继续下沉

这部分仍然最适合你自己继续：

1. 正式剧情段继续推进
2. beat / cue 组织
3. director staging / deployment / runtime 消费逻辑
4. 叙事节奏和群像层的最终表达

原因很简单：

- 这部分现在还在你的 active 设计和验证链里
- 我去接，只会把主导权打散

#### 2. 当前 active 的 `spring-day1` 核心代码面

包括但不限于：

1. `CrowdDirector` 当前主逻辑
2. 你仍在深推的 deployment / director 代码
3. 你当前 own 的 StageBook / runtime 消费链

这几类现在都不该让我硬抢。

### B. 我现在可以最深替你分担的内容

这部分我分成“现在就能接”和“等你回球后能接得更深”两层。

#### 现在就能接的

##### 1. Town scene mixed-dirty 的新子域切刀

我现在可以立刻接的，不是 resident 第三刀，而是 Town 的新子域：

1. `Town 相机 / 转场 / 玩家位`
2. `Town 环境 / Tilemap / bridge-farmland`
3. `Town manager / baseline`

这三类里，只要你明确指一个，我就可以继续向下压。

##### 2. Town scene-side contract 的命名与承接纪律

如果你接下来还要继续扩 anchor、slot、resident layer，我可以继续替你做：

1. scene-side 命名收敛
2. root/group/slot 层级的持续治理
3. 防止 Town 场景再被混成一锅 shared dirty

##### 3. 后续“迁回 Town”前的 scene-side 接盘准备

当你还没正式交 runtime 代码之前，我可以提前替你做：

1. 迁回前 scene-side contract 的精修
2. 某一批 anchor 的最终承接位梳理
3. 某一类 resident 的 group / slot 组织方式

#### 你正式回球后，我能接得更深的

##### 1. resident actor scene-side 真承接

如果你后面决定：

- 某一批 resident 不再只靠 `Primary` 代理，而要真正往 `Town` 迁

那这件事我可以接。

但前提是：

1. 你要交回明确的 actor 范围
2. 你要交回那批 resident 的 anchor / slot 归属
3. 你要把 active 代码 ownership 明确切开

##### 2. runtime 迁回时的 Town side 承接

如果后面真的开始：

- 从 `Primary` 代理承接迁回 `Town`

那 Town side 的 scene 承接、结构承接、位置承接，我可以深接。

##### 3. Town runtime live 承接证据

如果后面你已经不想自己再盯 Town scene live 现象，我也可以接：

1. Town scene live 取证
2. Town runtime 现场归因
3. Town specific blocker 窄口修复

---

## 七、哪些东西现在不该交给我

为了避免你后面调度时又把球扔错，我把不建议现在交给我的东西也写死。

### 1. 不要把你当前 active 的 director 主刀交给我

包括：

1. 你当前仍在改的 deployment / staging / runtime deployment 主代码
2. 当前还在深化的 beat / cue 组织逻辑
3. 当前还在沉的剧情正文消费链

### 2. 不要把“是否驻村常驻化”这个叙事判断再交回我

这个方向已经定了。

我现在负责的是：

- 承接这个方向的 Town side 现实落地

不是回头帮你重判方向。

### 3. 不要把 `Town` 以外的活混算给我

比如：

1. UI
2. NPC 本体语义
3. Primary 总历史
4. 共享输入与全局控制链

这些都不该再混到 Town 线程名下。

---

## 八、如果你现在要做最彻底的后续调度，我建议你这样排

### 第一优先

你继续自己吃：

1. `day1` 导演线
2. runtime deployment
3. 正文和 beat/cue 深化

同时把 Town 只当已具备 resident scene-side 最小承接层的场。

### 第二优先

如果你希望我继续分担，就只从下面三类里交一类给我：

1. `Town 相机 / 转场`
2. `Town 环境 / Tilemap`
3. `Town manager / baseline`

这里我最推荐你优先交的是：

- `Town 相机 / 转场`

因为：

1. 它和 Town 实际体验最直接
2. 切片边界最清楚
3. 不会和你当前 director 主刀硬撞

### 第三优先

等你把 resident deployment 再推进一段，开始真的有一批东西准备迁回 `Town` 时，再把那一批明确回球给我。

那个时候我就不是只做 scene-side 文档，而是能做更深的真实承接。

---

## 九、我现在对自己能力边界的最诚实判断

### 我现在最深能帮你分担到哪里

我现在最深能帮你分担到：

1. `Town` 的 scene-side 与 mixed-scene 子域继续清刀
2. 未来某一批 resident actor 迁回 `Town` 时的 Town side 真承接
3. Town runtime / live 现象的窄口归因与修复

### 我现在还不应该越过去的地方

我现在不应该越过去的，是：

1. 你当前 active 的 day1 director 主逻辑
2. 你当前 still-moving 的 deployment / runtime 消费核心代码
3. 需要你自己主导的叙事表达与导演节奏决策

---

## 十、最后一句最短口径

你现在可以这样理解我这条线：

- `Town` resident scene-side 已经搭到位了，后面不该再把剩余问题误认成 resident 第三刀；如果你要我继续分担，我现在最适合接的是 Town 的 mixed-scene 子域，而不是抢你当前 active 的 day1 director 主刀。
