# 2026-04-06｜给 spring-day1｜Town 原生 resident 场景迁移第一刀与 HomeAnchor 配置位回执

这份回执只汇报一件事：

- `Town` 这边已经不再停在“resident slot 有了”的说明层，
- 而是把原生 resident 的 scene-side 第一刀，真正往“用户可手摆 HomeAnchor、代码后续只消费现成位置”的方向推进了一步。

---

## 一、先说结论

### 1. 这轮已经把 `Town` 的 resident scene-side 从“只有槽位”推进到“有可配置位”

当前 `Town.unity` 不再只有：

1. `ResidentSlot_*`
2. `DirectorReady_*`
3. `BackstageSlot_*`

这些语义壳。

这轮我已经在现有 8 个承接位下面，补进了对应的 `HomeAnchor` 子节点。

### 2. 这轮没有越权去碰 `day1 / NPC / UI` 主逻辑

我这轮只做了两类东西：

1. `Town.unity`
   - 补 scene-side 的 `HomeAnchor` 配置位
2. `Assets/Editor/Town/TownSceneRuntimeAnchorReadinessMenu.cs`
   - 补对 `HomeAnchor` 存在性的检查

我没有去碰：

1. `SpringDay1NpcCrowdDirector.cs`
2. `StageBook / Manifest`
3. `NPC` 内容矩阵
4. `UI` 玩家面

### 3. 这轮之后，`Town` 当前最值钱的 scene-side 新事实是

`Town` 现在已经具备：

1. resident 根层
2. slot / director-ready / backstage 承接位
3. 与这些承接位一一对应的 `HomeAnchor` 用户配置位

也就是说：

- 这条线现在不再只是“可讨论的语义结构”，
- 而是已经进入“用户真能进 scene 手摆位置，后续逻辑真能朝只消费现成位置去收”的阶段。

---

## 二、这轮真实做成了什么

## 1. 在 `Town.unity` 里新增了 8 个 `HomeAnchor` 配置位

新增如下：

### resident 默认在场层

1. `SCENE/Town_Day1Residents/Resident_DefaultPresent/ResidentSlot_DinnerBackgroundRoot/Resident_DinnerBackgroundRoot_HomeAnchor`
2. `SCENE/Town_Day1Residents/Resident_DefaultPresent/ResidentSlot_DailyStand_01/Resident_DailyStand_01_HomeAnchor`
3. `SCENE/Town_Day1Residents/Resident_DefaultPresent/ResidentSlot_DailyStand_02/Resident_DailyStand_02_HomeAnchor`
4. `SCENE/Town_Day1Residents/Resident_DefaultPresent/ResidentSlot_DailyStand_03/Resident_DailyStand_03_HomeAnchor`

### director takeover 预备层

1. `SCENE/Town_Day1Residents/Resident_DirectorTakeoverReady/DirectorReady_EnterVillageCrowdRoot/DirectorReady_EnterVillageCrowdRoot_HomeAnchor`
2. `SCENE/Town_Day1Residents/Resident_DirectorTakeoverReady/DirectorReady_KidLook_01/DirectorReady_KidLook_01_HomeAnchor`
3. `SCENE/Town_Day1Residents/Resident_DirectorTakeoverReady/DirectorReady_DinnerBackgroundRoot/DirectorReady_DinnerBackgroundRoot_HomeAnchor`

### backstage 暂隐层

1. `SCENE/Town_Day1Residents/Resident_BackstagePresent/BackstageSlot_NightWitness_01/Backstage_NightWitness_01_HomeAnchor`

这些节点当前全部都是：

1. 局部坐标 `(0,0,0)`
2. 直接挂在现有 slot/ready/backstage 位下面
3. 不改原 slot 名，不改原 slot 位置

## 2. `TownSceneRuntimeAnchorReadinessMenu.cs` 已补 `HomeAnchor` 检查

当前这份 probe 不再只检查：

1. anchor 是否存在
2. slot 是否存在
3. slot 与 anchor 是否对位

它现在还会继续检查：

1. 当前 primary slot 下是否已有 `HomeAnchor`
2. secondary slot 下是否已有 `HomeAnchor`
3. 这些 `HomeAnchor` 是否仍在 `_CameraBounds` 内

这意味着：

- 后面再跑这份 probe 时，
- 它不只会回答“slot 有没有”，
- 还会直接回答“用户位置配置位有没有”。

---

## 三、这轮为什么这样做

### 1. 原有配置已经不够支撑“用户手摆位置”

原有 `Town` scene-side 配置虽然已经有：

1. `ResidentSlot_*`
2. `DirectorReady_*`
3. `BackstageSlot_*`

但它还缺一层关键东西：

- 用户真能拿来摆位置的 `HomeAnchor`

如果没有这层，方向上虽然说“代码不要偷改位置”，但 scene 里其实还没有把位置权落到一个清楚的节点上。

### 2. 这轮不适合直接塞原生 NPC 实体

我没有在这轮直接往 `Town` 里塞叫 `101~301` 的原生 NPC 对象。

原因是：

1. 当前 `day1` 代码还在 active 迁移期
2. 现在直接塞同名 resident，容易让逻辑提前绑定到半成品对象
3. 这会越过 `Town` own 的“scene-side 承接”边界，闯进 `day1/NPC` 主逻辑

所以这轮最值钱、最安全的一刀，就是先把：

- `slot -> HomeAnchor`

这层 scene-side 配置位补出来。

---

## 四、给 spring-day1 的直接消费口径

### 1. 你现在可以直接信什么

你现在可以把下面这句话当真值：

`Town` 当前已经不仅有 resident 根层和 slot 语义层，还已经把 8 个优先承接位的 `HomeAnchor` 配置位补到了 scene 里。

### 2. 你现在不要再误判什么

至少这轮之后，不要再把 `Town` 当前的问题理解成：

1. “只有 slot，没有用户位置节点”
2. “原生 resident scene-side 还完全没落到可施工层”

因为这两条已经不是当前事实。

### 3. 你下一步如果继续接这条线，第一安全接刀点是什么

当前最安全的接刀点是：

1. 继续把 `day1` 逻辑往“只消费 scene-side 现成位置”迁
2. 但先不要假设 `Town` 已经有最终版 native resident actor
3. 先把这批 `HomeAnchor` 当成原生 resident 的位置配置位和后续迁移触点

---

## 五、现在还没做成什么

这轮还没做成的有 3 件事：

1. 还没有把真实 native resident actor 本体放进 `Town`
2. 还没有把 `day1` 逻辑正式改成消费这批新的 `HomeAnchor`
3. 还没有把 native resident actor 的原生存在性推进到 scene 本体层

---

## 六、fresh live 证据已经补齐

桥恢复后，这轮新增的 `HomeAnchor` 承接层已经拿到 fresh live 证据：

1. 菜单已真实执行：
   - `Tools/Sunset/Scene/Run Town Runtime Anchor Readiness Probe`
2. 输出文件：
   - `Library/CodexEditorCommands/town-runtime-anchor-readiness-probe.json`
3. 当前结果：
   - `status = completed`
   - `success = true`
   - `blockingFindings = 0`
   - `attentionFindings = 0`
4. 这份 probe 已明确看到：
   - 8 个 anchor 全部存在
   - 8 个 slot / ready / backstage 位全部存在
   - 8 个新补的 `HomeAnchor` 全部存在
   - 它们都仍在 `_CameraBounds` 内
   - `anchor -> slot` 距离全部为 `0`
   - `slot -> HomeAnchor` 也处于同位
5. 同轮代码/控制台证据：
   - `validate_script Assets/Editor/Town/TownSceneRuntimeAnchorReadinessMenu.cs` = `assessment=no_red`
   - `errors --count 10 --output-limit 5` = `errors=0 warnings=0`

---

## 七、一句话给 spring-day1

`Town` 这边已经把原生 resident scene-side 第一刀推进成“8 个优先承接位都有独立 HomeAnchor 配置点”的状态；你现在可以继续把逻辑往“消费现成位置”收，但还不要把 `Town` 误当成 native resident actor 已全部入场的最终态。
