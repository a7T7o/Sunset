# 2026-04-06｜给 spring-day1｜Town 驻村常驻化 scene 第二刀槽位合同与第一批迁回优先级

这份回执建立在 [2026-04-06_给spring-day1_Town驻村常驻化scene第一刀已落地回执_08.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/2026-04-06_给spring-day1_Town驻村常驻化scene第一刀已落地回执_08.md) 之上。

新的真值不是“Town 只有 resident 根层了”，而是：

- `Town` 现在已经把 **第一批可迁回的空承接槽位** 也压出来了

---

## 一、这轮又压实了什么

除了上一轮已经落下的：

- `Town_Day1Residents`
- `Resident_DefaultPresent`
- `Resident_DirectorTakeoverReady`
- `Resident_BackstagePresent`

现在 `Town.unity` 里又真实新增了 8 个 scene-side 空槽位：

### 1. 默认 resident 直接承接位

挂在：

- `Town_Day1Residents/Resident_DefaultPresent`

当前已有：

1. `ResidentSlot_DinnerBackgroundRoot`
2. `ResidentSlot_DailyStand_01`
3. `ResidentSlot_DailyStand_02`
4. `ResidentSlot_DailyStand_03`

### 2. director takeover 预备位

挂在：

- `Town_Day1Residents/Resident_DirectorTakeoverReady`

当前已有：

1. `DirectorReady_EnterVillageCrowdRoot`
2. `DirectorReady_KidLook_01`
3. `DirectorReady_DinnerBackgroundRoot`

### 3. backstage 暂隐位

挂在：

- `Town_Day1Residents/Resident_BackstagePresent`

当前已有：

1. `BackstageSlot_NightWitness_01`

---

## 二、这些槽位现在的意义

这些不是 actor 实体。

也不是 runtime deployment 已经迁回。

它们现在的意义是：

1. 先把 `Town` 的 resident scene-side contract 从“root 级”推进到“slot 级”
2. 让你后面做 resident deployment 时，不需要再自己发明 Town 里的第一批空承接位
3. 让以后从 `Primary` 代理迁回 `Town` 时，scene-side 命名和层级不再重排一轮

---

## 三、第一批最适合迁回的 anchor

如果你后面准备把 resident deployment 真往 `Town` 迁，我现在的优先级判断是：

### 第一批最值得迁回

1. `DinnerBackgroundRoot`
2. `DailyStand_01`
3. `DailyStand_02`
4. `DailyStand_03`

原因：

1. 这四组最像真正的 resident 常驻生活位
2. 它们不依赖“玩家刚进村第一拍”的精确导演压迫
3. 迁回后最容易保持“本来就在村里”的语义

### 第二批可以继续迁回

1. `NightWitness_01`

但更准确地说，它不是“直接常驻 visible 位”，而是：

1. actor 先挂在 `Resident_BackstagePresent`
2. 需要时再被导演接到 `NightWitness_01`

### 当前仍不建议直接按 resident 常驻位硬吃

1. `EnterVillageCrowdRoot`
2. `KidLook_01`

原因：

1. 这两个更像 day1 开篇阶段的 director takeover 位
2. 它们适合接现有 resident actor
3. 但不适合被直接理解成“平时就站在那里不动的常驻 parking slot”

---

## 四、现在必须锁死不再漂的 scene-side 合同

下面这些名，我建议从现在起就不要再漂：

### resident 根层

1. `SCENE/Town_Day1Residents`
2. `Resident_DefaultPresent`
3. `Resident_DirectorTakeoverReady`
4. `Resident_BackstagePresent`

### director carrier 壳

1. `SCENE/Town_Day1Carriers`
2. `EnterVillageCrowdRoot`
3. `KidLook_01`
4. `DinnerBackgroundRoot`
5. `NightWitness_01`
6. `DailyStand_01`
7. `DailyStand_02`
8. `DailyStand_03`

### 第二刀新增 slot

1. `ResidentSlot_DinnerBackgroundRoot`
2. `ResidentSlot_DailyStand_01`
3. `ResidentSlot_DailyStand_02`
4. `ResidentSlot_DailyStand_03`
5. `DirectorReady_EnterVillageCrowdRoot`
6. `DirectorReady_KidLook_01`
7. `DirectorReady_DinnerBackgroundRoot`
8. `BackstageSlot_NightWitness_01`

---

## 五、你做 deployment 大 checkpoint 时，Town 现在已经能给你的东西

我现在给你的，不再只是“未来方向说明”，而是这三层现成承接面：

1. `root 层`
2. `group 层`
3. `slot 层`

换句话说：

- 等你这一轮 resident deployment / 导演数据大 checkpoint 做完，下一步如果要把第一批东西迁回 `Town`，scene-side 不需要从零开始搭。

---

## 六、我对你现在最短的建议

你现在可以继续狠狠干 resident deployment 与导演数据，因为：

- `Town` 这边已经给出了第一批可迁回的 slot contract

但如果你下一步真的准备迁回：

1. 先迁 `DinnerBackgroundRoot + DailyStand_01~03`
2. `NightWitness_01` 作为 backstage -> witness 的第二批
3. `EnterVillageCrowdRoot / KidLook_01` 继续保留为 director takeover 面，不要误降成普通常驻停车位

---

## 七、当前验证注意项

我这里还要补一条最新现场，避免你误读成“Town 这边做完第二刀后全局 console 仍是绝对干净”：

1. 当前 Unity console 里新出现了一条外部校验失败：
   - `SpringDay1NpcCrowdValidation`
   - 具体定性是：`EnterVillage_PostEntry` 的导演消费角色漂移，当前 `Trace` 里出现了 `301`
2. 这条失败来自：
   - `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
   - 它读的是 `manifest / 导演消费` 校验面，不直接读取 `Town.unity` 的 resident root / slot scene-side 结构
3. 所以当前更诚实的口径是：
   - `Town` 第二刀的 `scene-side root/group/slot` 产物仍成立
   - 但全局 fresh console 现在不应再被描述成 `0 error`
   - 这条红更像你当前 active resident deployment / 导演数据线上的外部漂移，不是我这边 `Town.unity` 自己新引入的 blocker
