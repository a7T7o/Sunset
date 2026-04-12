# 2026-04-06｜给 spring-day1｜Town 最终收尾阶段 scene-side 正式基线与下一安全切片回执

这份回执只做两件事：

1. 把 `Town` 当前对 `Day1` 真正已经可以直接吃的 `scene-side` 基线说死
2. 把 `Town` 这边为什么这轮**不继续盲补 resident slot**、而应把下一安全切片改判到别的 own 子域，说死

---

## 一、先说结论

### 1. 当前 `Town` resident scene-side 基线已经够 `day1` 继续吃

当前 `day1` 应认的 `Town` resident scene-side 正式基线，仍以：

- `15d75285`
- `Town partial sync scope correction`

为正式 checkpoint。

而当前 working tree 里那份更大的 `Town.unity` broader dirty，仍然**不能**被直接当成整份正式基线。

### 2. 这轮我没有继续在 `Town.unity` 上盲补更多 resident slot

原因不是我退回说明层。

而是我把当前 `Town_Day1Carriers / Town_Day1Residents` 这一层重新审了一遍后确认：

- 当前这批 `ResidentSlot_* / DirectorReady_* / BackstageSlot_*` 的**不对称分布本身就是语义设计的一部分**
- 不是“漏补几格空位”

所以这轮如果为了“看起来更满”继续往 `Town.unity` 补：

- `ResidentSlot_EnterVillageCrowdRoot`
- `ResidentSlot_KidLook_01`
- `DirectorReady_NightWitness_01`
- 或者把每个 anchor 都硬补成三层齐套

反而更容易把当前 `Day1` 的导演语义补歪。

### 3. `Town` 的下一安全切片，不再是 resident 第三刀

当前更诚实的改判是：

1. `resident root / group / slot` 最小 contract 已站住
2. 当前剩余 Town own 问题，不再是“resident scene-side 还没搭完”
3. `Town` 下一安全切片应转向：
   - `Town 相机 / 转场 / 玩家位`
   - 或 `Town 环境 / Tilemap`
   - 或 `Town baseline / manager`

而不是继续把整份 `Town` 脏改误叫成 `resident scene-side` 延长线

---

## 二、当前正式可吃的 Town resident scene-side 基线

下面这些对象层级，我已经重新按 scene YAML 核过，当前可继续视为 `day1` 可直接引用的 scene-side contract：

### 1. root / group 层

- `SCENE/Town_Day1Carriers`
- `SCENE/Town_Day1Residents`
- `Town_Day1Residents/Resident_DefaultPresent`
- `Town_Day1Residents/Resident_DirectorTakeoverReady`
- `Town_Day1Residents/Resident_BackstagePresent`

### 2. carrier 层

- `EnterVillageCrowdRoot`
- `KidLook_01`
- `DinnerBackgroundRoot`
- `NightWitness_01`
- `DailyStand_01`
- `DailyStand_02`
- `DailyStand_03`

### 3. slot 层

#### 默认 resident 承接位

- `ResidentSlot_DinnerBackgroundRoot`
- `ResidentSlot_DailyStand_01`
- `ResidentSlot_DailyStand_02`
- `ResidentSlot_DailyStand_03`

#### director takeover 预备位

- `DirectorReady_EnterVillageCrowdRoot`
- `DirectorReady_KidLook_01`
- `DirectorReady_DinnerBackgroundRoot`

#### backstage 暂隐位

- `BackstageSlot_NightWitness_01`

---

## 三、这套 contract 为什么现在是“不对称但正确”的

这一层这轮我专门按 `Town.unity` 当前 scene 结构和既有 Town 文档重新核过。

结论是：

### 1. `EnterVillageCrowdRoot / KidLook_01` 当前更像 director takeover 位

它们现在已经有：

- carrier
- `DirectorReady_*`

但没有默认 resident slot。

这不是漏补，更像当前语义本来就是：

- 先让 resident actor 以“可被导演接管”的方式存在
- 再在开篇时接入这两个 cue 位

如果现在把它们硬补成普通常驻停车位，会模糊它们“开篇导演位”的职责。

### 2. `NightWitness_01` 当前更像 backstage -> witness 位

它现在已经有：

- carrier
- `BackstageSlot_NightWitness_01`

这和之前 Town 第二刀合同里的解释一致：

- 先在 `Resident_BackstagePresent` 暂隐
- 需要时再被导演接到 `NightWitness_01`

所以它当前没有 `ResidentSlot_NightWitness_01`，也不应直接被误读成“少补了一格”。

### 3. `DinnerBackgroundRoot / DailyStand_01~03` 才是第一批真正像常驻生活位的 anchor

它们当前落在：

- `Resident_DefaultPresent`

这一层更合理，因为它们承担的是：

- 饭馆背景生活层
- 次日生活预示层

本来就比 `EnterVillage / KidLook / NightWitness` 更接近“平时就在村里”的 resident 语义。

---

## 四、这轮为什么不继续直接改 `Town.unity`

这轮我不是“不敢进 scene”。

而是按当前场景修改规则，把“原有配置 / 问题原因 / 建议修改 / 修改后效果 / 对原有功能影响”重新过了一遍，结论是：

### 1. 原有配置

当前 `Town.unity` 在 `resident scene-side` 这一块，已经有：

1. `Town_Day1Carriers`
2. `Town_Day1Residents`
3. 三个 resident group
4. 八个第一批 slot contract

而且这些 slot 的分配方式不是平均铺开的。

### 2. 问题原因

当前真正的问题不是“scene 里少几个对象名”。

当前真正的问题是：

1. 这套不对称 contract 很容易被误读成“没补完”
2. working tree 里还有更大的 broader dirty，会继续污染 `day1` 对 Town 基线的判断

### 3. 建议修改

这轮不建议直接继续补更多 resident slot。

建议是：

1. 先把当前不对称 contract 正式定性为“有语义的正式基线”
2. 把 `Town` 下一安全切片从 resident 第三刀改判出去

### 4. 修改后效果

这样做的效果不是“场景里对象变更多”。

而是：

1. `day1` 后续不会把 `EnterVillage / KidLook / NightWitness` 误当成普通 resident parking slot
2. `Town` own 施工方向不再漂回“再多补一点 slot”
3. `Town` 可以更诚实地把下一刀让给更像 own backlog 的 mixed-scene 子域

### 5. 对原有功能的影响

不继续盲补 slot 的影响是：

1. 不会破坏当前已有 anchor 语义
2. 不会冒着 broader dirty 现场继续扩写 `Town.unity`
3. 保住当前 `day1` 正在吃的 resident scene-side contract 不被我补歪

---

## 五、给 day1 的正式消费口径

### 1. 当前你可以继续吃什么

你现在可以继续把 `Town` 当成：

- 已有 resident scene-side 最小 contract
- 已有第一批 carrier / slot / backstage / director-ready 命名
- 已能支撑 `DinnerBackgroundRoot / DailyStand_01~03`
- 已能支撑 `EnterVillage / KidLook / NightWitness` 的 cue 级场景承接

### 2. 当前你不要误读什么

不要把下面两件事混成一件：

1. `Town` 已经有 resident scene-side contract
2. `Town.unity` 整份 working tree 都已经 fully ready

前者成立，后者仍不成立。

### 3. 当前第一真实 Town blocker 是什么

对 `day1` 来说，当前第一真实 Town blocker 已经不再是：

- resident root / slot 不存在

而是：

- 当前 broader dirty 仍和 resident contract 共处同一份 `Town.unity`
- `day1` 需要一个更明确的“信哪份 checkpoint、别把哪部分误算进正式基线”的口径

### 4. `DailyStand_02 / 03` 当前最先会撞到的不是 scene-side 空位

这点我也顺手重新钉死：

- `DailyStand_02`
- `DailyStand_03`

当前在 `Town.unity` 的 scene-side 承接位已经真实存在：

- `DailyStand_02`
- `DailyStand_03`
- `ResidentSlot_DailyStand_02`
- `ResidentSlot_DailyStand_03`

所以如果你下一步继续把 `DailyStand_02 / 03` 往 runtime 可消费链推进，最先撞到的更可能是：

1. `TownAnchorContract` / semantic contract 代码侧
2. director / runtime deployment 消费侧
3. 你当前主链如何把这两个 anchor 真正吃回

而不是 `Town` scene-side 还没有给出站位名或承接位。

---

## 六、Town own 下一安全切片改判

### 1. 不再建议开 resident 第三刀

这条线这轮我正式改判为：

- 在当前 shared dirty 条件下，`resident scene-side` 已经压到本轮能诚实继续引用的尽头

### 2. 如果继续 Town own，下一刀更安全的顺序

当前更推荐的顺序是：

1. `Town 相机 / 转场 / 玩家位`
2. `Town 环境 / Tilemap / bridge-farmland`
3. `Town baseline / manager`

### 3. 当前我最推荐的下一刀

如果用户或治理位要继续给我开 `Town own`，我当前最推荐：

- `Town 相机 / 转场 / 玩家位` 单独切成一刀

原因是：

1. 它和 `Day1` 后续在 `Town` 里的真实玩家体验关系最直接
2. 它不会继续污染当前 resident contract
3. 切片边界比环境层和 manager 层更窄

---

## 七、这轮之后 Town 应该怎么被理解

这轮之后，`Town` 这条线更准确的理解应是：

1. `Day1` 的 resident scene-side 最小承接层：已成立
2. 当前 first blocker：不是 slot 不够，而是 broader dirty 与正式基线仍共处一份 scene
3. 当前 next safe slice：不是 resident 第三刀，而是切去 `Town` own mixed-scene 子域

---

## 八、给 spring-day1 的一句话

你现在可以继续把当前 `Town` 当成已经具备第一批 resident scene-side contract 的场来吃；但不要再把 `Town` 剩余问题误判成“slot 还没补完”，因为这轮我已经把它改判成了一个**有意不对称的正式基线**，下一刀该切去 `Town own` 的别的子域了。
