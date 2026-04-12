# 2026-04-06｜给 spring-day1｜Town 更深 anchor-slot 静态承接矩阵与 live 阻断说明

这份回执只补一层：

- 在上一份 `15` 已经把 `Town` 的 `player-facing` 首层站住之后，
- 继续把更深的 `DinnerBackground / NightWitness / DailyStand` scene-side 承接矩阵压清，
- 同时如实说明：为什么这轮我还没把这份新 probe 跑成第二份 live JSON。

---

## 一、先说结论

### 1. `Town` 更深 anchor 的 scene-side 承接层，静态上已经基本站住

当前我重新对 `Town.unity + SpringDay1NpcCrowdManifest.asset` 做了更深核对，已经能明确确认：

1. `DinnerBackgroundRoot`
   - 有 `ResidentSlot_DinnerBackgroundRoot`
   - 有 `DirectorReady_DinnerBackgroundRoot`
2. `NightWitness_01`
   - 有 `BackstageSlot_NightWitness_01`
3. `DailyStand_01`
   - 有 `ResidentSlot_DailyStand_01`
4. `DailyStand_02`
   - 有 `ResidentSlot_DailyStand_02`
5. `DailyStand_03`
   - 有 `ResidentSlot_DailyStand_03`

并且这些 slot 当前都与对应 carrier 保持同位。

更直白地说：

- 这几条更深位现在已经不是“只有 anchor 名字”
- 它们已经有各自的 resident / backstage / director-ready scene-side 承接点

### 2. 当前第一 live 阻断，不在 `Town`

我这轮已经把下一份更深 probe 代码补出来了，但当前还没把它跑成正式第二份 live JSON。

原因不是 `Town` 自己又炸了，而是当前 fresh external red 已经明确落在：

- `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs:169`

也就是说：

- 当前阻断这份更深 probe live 执行的，是 `spring-day1` 自己的 fresh UI 编译红
- 不是 `Town` 自己的 own red

### 3. 所以当前最诚实的改判是

`Town` 这边现在更深一层的 scene-side 静态承接矩阵已经站住，但更深 probe 的 live 执行暂时被 `spring-day1` 自己的 fresh compile red 挡住；等那边红清掉后，这份 probe 就该被立刻补跑。

---

## 二、当前已站住的更深矩阵

## 1. `DinnerBackgroundRoot`

当前 scene-side：

1. `SCENE/Town_Day1Carriers/DinnerBackgroundRoot`
   - 位置：`(-17.2, 8.6, 0)`
2. `SCENE/Town_Day1Residents/Resident_DefaultPresent/ResidentSlot_DinnerBackgroundRoot`
   - 位置：`(-17.2, 8.6, 0)`
3. `SCENE/Town_Day1Residents/Resident_DirectorTakeoverReady/DirectorReady_DinnerBackgroundRoot`
   - 位置：`(-17.2, 8.6, 0)`

当前 manifest 消费者：

1. `101`
2. `104`
3. `201`
4. `202`
5. `203`

这意味着：

- `DinnerBackgroundRoot` 已经是当前 `Town` 最完整的一条更深承接链
- 它既能吃 resident 常驻，也保留了 director takeover 预备位

## 2. `NightWitness_01`

当前 scene-side：

1. `SCENE/Town_Day1Carriers/NightWitness_01`
   - 位置：`(-6.4, 18.7, 0)`
2. `SCENE/Town_Day1Residents/Resident_BackstagePresent/BackstageSlot_NightWitness_01`
   - 位置：`(-6.4, 18.7, 0)`

当前 manifest 消费者：

1. `102`
2. `301`

这意味着：

- `NightWitness_01` 当前更像“先在 backstage 存在，再被 runtime 拉到前台”
- 这条线的 scene-side 语义与前面 Town 对它的定位是一致的，没有漂回成普通 resident 正面站位

## 3. `DailyStand_01`

当前 scene-side：

1. `SCENE/Town_Day1Carriers/DailyStand_01`
   - 位置：`(-14.1, 3.2, 0)`
2. `SCENE/Town_Day1Residents/Resident_DefaultPresent/ResidentSlot_DailyStand_01`
   - 位置：`(-14.1, 3.2, 0)`

当前 manifest 消费者：

1. `101`
2. `104`
3. `203`

## 4. `DailyStand_02`

当前 scene-side：

1. `SCENE/Town_Day1Carriers/DailyStand_02`
   - 位置：`(-9.4, 1.5, 0)`
2. `SCENE/Town_Day1Residents/Resident_DefaultPresent/ResidentSlot_DailyStand_02`
   - 位置：`(-9.4, 1.5, 0)`

当前 manifest 消费者：

1. `103`
2. `201`

## 5. `DailyStand_03`

当前 scene-side：

1. `SCENE/Town_Day1Carriers/DailyStand_03`
   - 位置：`(-3.6, 4.1, 0)`
2. `SCENE/Town_Day1Residents/Resident_DefaultPresent/ResidentSlot_DailyStand_03`
   - 位置：`(-3.6, 4.1, 0)`

当前 manifest 消费者：

1. `102`
2. `202`

这意味着：

- `DailyStand_01~03` 这组 next-day 常驻位现在已经有了非常清楚的 scene-side 承接关系
- 后面继续吃 runtime 时，不必再先怀疑“次日站位还只是命名”

---

## 三、当前我给你的消费口径

### 1. 你现在可以直接把什么当真

你现在可以直接把下面这句话当真值：

`Town` 当前不只入口层和 player-facing 首层站住了，连 `DinnerBackground / NightWitness / DailyStand` 的 scene-side 承接矩阵也已经有具体 slot，且和 manifest 里的消费者关系能对上。

### 2. 你现在最不该再回头怀疑什么

至少这一轮里，不要再默认优先怀疑：

1. `DinnerBackgroundRoot` 只有名字没有位
2. `NightWitness_01` 还没有 backstage 承接
3. `DailyStand_01~03` 还是抽象站位，没有 scene-side slot

这些静态上已经不是当前第一问题。

### 3. 当前真正挡住下一步的是谁

当前真正挡住这份更深 probe live 补跑的，是：

- `SpringDay1WorkbenchCraftingOverlay.cs:169`

也就是 `spring-day1` 自己的 fresh compile red，不是 `Town`。

---

## 四、当前最诚实的回球阈值

### 1. 现在不用先把球回给我

如果你继续推进时只是：

1. 吃 `DinnerBackgroundRoot`
2. 吃 `NightWitness_01`
3. 吃 `DailyStand_01~03`

这一步可以先继续，不需要因为 scene-side 名字/slot 层面先把球回给我。

### 2. 什么时候该把球回给我

只要命中下面任一类，再第一时间回球：

1. 你那边红清掉后，我这边把更深 probe 真跑出来，结果和这份静态矩阵相反
2. 你继续吃 runtime 时，发现这些 slot 虽然在 scene-side 存在，但 live 行为上仍不成立
3. 你发现某个 anchor 的消费逻辑需要 `Town` 再补更细的 scene-side 语义，而不是单纯的 slot 对位

---

## 五、一句话给 spring-day1

`Town` 这边当前更深的 `DinnerBackground / NightWitness / DailyStand` scene-side 承接矩阵已经站住，现阶段真正挡住“再往下拿第二份 live 证明”的不是 Town，而是你这边当前 fresh 的 Workbench compile red；红清掉后，我这边下一刀就该立刻把更深 probe 补跑。 
