# 2026-04-06｜spring-day1｜NPC 协作线最终收尾阶段清单

这份不是 `NPC` 线程的全部任务单。

这份只列 `Day1 最终收尾阶段` 里，`NPC` 继续帮 `day1` 扛的那部分内容。

---

## 开始前统一口径

### 允许按需使用 subagent

允许按需使用 `subagent`，但优先用于：

1. 内容梳理
2. 独立 tests / probes / validation
3. 独立 contract 补强

有问题先回 `spring-day1` 主控台，没有问题就继续下一部分。

### 这份清单的边界

`NPC` 这轮不该回吞：

1. `CrowdDirector` 主刀
2. `Town` scene runtime 承接
3. `day1` 的剧情 owner 判断
4. `UI` 玩家面壳体

---

## 当前已核实的真实起点

已核实真实存在：

1. `SpringDay1CrowdDirectorConsumptionRole`
2. `BuildBeatConsumptionSnapshot()`
3. `NPCFormalDialogueState`
4. `HasConsumedFormalDialogueForCurrentStory()`
5. `WillYieldToInformalResident()`

当前判断已经明确：

`NPC` 不再是“底座不够”的阶段，而是“交出来的 contract 要被 Day1 真吃回去”的阶段。

---

## 第一部分：继续把 resident semantic matrix 稳成 Day1 可持续消费的真值层

### 当前必须继续守住的 beat

1. `EnterVillage_PostEntry`
2. `DinnerConflict_Table`
3. `ReturnAndReminder_WalkBack`
4. `FreeTime_NightWitness`
5. `DayEnd_Settle`
6. `DailyStand_Preview`

### 必须继续稳住的字段

1. `semanticAnchorIds`
2. `presenceLevel`
3. `flags`
4. `note`
5. `sceneDuties`
6. `growthIntent`

### 这部分对 Day1 的意义

`day1` 后续要做的是 resident deployment / director consumption / 最终整合。

所以 `NPC` 必须继续保证：

1. 同一 beat 该谁是 `priority`
2. 该谁是 `support`
3. 该谁只是 `trace`
4. 该谁是 `backstagePressure`

这些真值不要漂。

---

## 第二部分：把 formal 一次性消耗后的内容承接彻底做实

这是 `Day1` 当前硬要求，不能只停在 contract 名字存在。

### 必须继续守住

1. formal 未消费前
   - formal 优先
2. formal 已消费后
   - 不再重播 formal
   - 不再伪装成还能再次正式推进
   - 必须让位给：
     - `informal`
     - `resident 日常`
     - `phase 后非正式补句`

### NPC 这侧继续要补的不是规则本身，而是内容密度

要尽量避免出现：

1. formal 消费后虽然逻辑切走了，但内容层发空
2. 玩家再次对话时只有机械 fallback
3. 不同 phase 下 resident 的体感没有变化

---

## 第三部分：继续补 Day1 真要吃的 bridge / probe / validation

`NPC` 现在最值钱的继续推进，不是再发大方向说明，而是继续补能卡住回归的护栏。

### 优先继续守的桥接点

1. manifest -> director stagebook
2. resident roster -> beat consumption snapshot
3. formal consumed -> informal/resident yield
4. 关键 beat 的 `semanticAnchorId / duty / role` 一致性

### 如果还能继续，优先补这些

1. `DailyStand_02 / 03` 相关 bridge 护栏
2. 更窄的 runtime probe
3. validation menu 对 resident consumption roster 的持续校验

---

## 第四部分：继续把 resident 内容层往“常驻驻村”体感压，而不是“阶段出现一次就没了”

用户已经明确偏好：

这批人要做成 `常驻居民`，不是保守地继续留在“阶段到了才出现的 crowd”。

### 这意味着 NPC 侧继续要补的方向

1. 常驻居民的存在感分层
2. 同一 NPC 在不同 phase 的日常语义变化
3. 与 formal 主戏错开后的 resident 补句池
4. 不同 beat 的 pair / ambient / 自言自语密度

### 这部分不要误解成 scene deployment

`NPC` 补的是：

1. 内容语义
2. contract
3. probe / tests

不是自己回去写 `Town runtime` 或 `CrowdDirector` 主刀。

---

## 第五部分：最终交回 day1 时只交什么

`NPC` 最终交回 `day1` 的，应该是：

1. 可直接吃的 resident consumption 真值
2. formal-consumed 后的可直接吃 contract
3. resident / informal / post-phase 内容承接
4. 对应的 tests / validation / probe 证据
5. 当前仍挡 Day1 最终整合的第一真实 blocker

---

## 当前不该回吞的内容

1. 不要回吞 `CrowdDirector` 主逻辑
2. 不要回吞 `Town` 常驻落位
3. 不要回吞 `UI` 提示壳与正式玩家面
4. 不要把 deployment/runtime scene 问题继续包装成“NPC 内容层还没做完”
