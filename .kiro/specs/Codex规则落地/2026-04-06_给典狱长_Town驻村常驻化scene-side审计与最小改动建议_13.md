# 2026-04-06｜给典狱长｜Town 驻村常驻化 scene-side 审计与最小改动建议

这份不是让 `Town` 立刻去改 `spring-day1` active 代码。

这份只解决一件事：

- 在 `day1` 已正式改判为“驻村常驻化”之后，`Town` 这边当前最值钱、最安全、最窄的推进到底是什么

---

## A. 用户可读层

### 1. 当前主线

`Town` own 主线已经从“最小 runtime spawn contract”继续转向：

- **驻村常驻化的 scene-side 承接**

这轮我没有回去抢 `CrowdDirector`。

### 2. 这轮实际做成了什么

这轮我做成了 4 件事：

1. 确认 `Town.unity` 里现有的 `SCENE/Town_Day1Carriers` 只是导演 takeover 壳，不是 resident 常驻层
2. 确认 `Town` 当前根本还没有任何 `resident root`
3. 把 resident root / carrier / backstage 三层语义正式拆开
4. 给出一版以后从 `Primary` 代理迁回 `Town` 的最小 scene-side contract

### 3. 现在还没做成什么

这轮没有做成的，是：

- **没有直接修改 `Town.unity` 去新增 resident root 或摆 carrier 真实位置**

原因不是我没想清楚，而是：

1. `Town.unity` 当前本身就是 dirty 场景
2. 现有规则要求生产 scene 改动先审再改
3. 这轮能推进到最深的安全位置，是“把最小 scene-side 改动卡写死”，不是直接吞 scene 脏改

### 4. 当前阶段

当前阶段应理解成：

- `Town` 驻村常驻化的语义层和最小 scene-side contract 已经站住
- 但 scene 写入层还没正式开刀

### 5. 下一步只做什么

下一步只值得做两种：

1. 如果用户明确开 `Town.unity` 写窗，就按这份审计卡去做最小 scene-side 改动
2. 如果 `day1` 还继续做代理，就继续保持 docs-only 协作位，不抢它的代码面

### 6. 需要用户现在做什么

如果你要我继续最深推进，这里有两个明确选项：

1. 现在开放 `Town.unity` 的 scene-side 写窗，我按这份审计卡做最小 resident root 准备
2. 先不开 scene 写窗，我继续只守 docs-only，等 `day1` 真的回球再接

### 7. 这轮最核心的判断是什么

我这轮最核心的判断是：

- 当前第一 scene-side blocker 已经变了，不再是“runtime resolver 还没吃 semanticAnchorId”，而是“Town 还没有 resident 根层，carrier 也还是零位空壳”。

### 8. 为什么我认为这个判断成立

因为这次我拿到的是三类硬证据：

1. `Town.unity`
   - `SCENE/Town_Day1Carriers` 真实存在
   - 7 个 child 名都在
   - 但全部 `m_LocalPosition = {0,0,0}`
   - 当前没有任何 `Town_Day1Residents` 或等价 resident 根层
2. `Primary.unity`
   - `001 / 002 / 003` 老锚仍只存在于 `Primary`
3. `SpringDay1NpcCrowdManifest.asset`
   - `semanticAnchorIds / sceneDuties / growthIntent` 已经把“这批人是有驻村语义的”写出来了

### 9. 这轮最薄弱、最可能看错的点是什么

最可能看错的不是 carrier/resident 分层，而是：

- `Town.unity` 当前 dirty 里，是否已经混有别人准备做的 resident 相关现场

所以我这轮选择停在“精确改动卡”，不直接写 scene。

### 10. 自评

这轮我给自己 `8.5/10`。

好的地方是：

- 方向已经从“继续讲 runtime spawn”转成“驻村常驻化承接”
- 而且我把 scene-side 最窄 blocker 找到了

最不满意的地方是：

- 这轮还没真正进入 `Town.unity` 写入层

---

## B. scene-side 五段审计

### 1. 原有配置

当前 `Town` 这边的真实 scene-side 配置是：

1. `SCENE/Town_Day1Carriers` 已存在
2. 其下已有：
   - `EnterVillageCrowdRoot`
   - `KidLook_01`
   - `DinnerBackgroundRoot`
   - `NightWitness_01`
   - `DailyStand_01`
   - `DailyStand_02`
   - `DailyStand_03`
3. 这 7 个 child 当前全部是：
   - 激活的空 `Transform`
   - 本地位置全为 `(0,0,0)`
4. 当前 `Town` 里还没有独立的 resident 根层
5. `Primary` 里仍保留旧锚：
   - `001`
   - `002`
   - `003`

### 2. 问题原因

当前问题不是“锚点没名字”，而是 scene-side 层级错位：

1. `Town_Day1Carriers` 当前只够表达“导演接管位”
2. 它表达不了“这些人本来就在村里”
3. 没有 resident 根层时，所有常驻语义都会被误压回：
   - runtime 临时生成
   - 或旧 `Primary` 代理锚
4. 7 个 carrier 仍全在 `(0,0,0)`，说明它们还只是语义壳，不是可迁回的真实 scene 位

### 3. 建议修改

当用户明确开放 `Town.unity` 写窗后，我建议只做最小 scene-side 改动，不做大改：

#### 建议 1：新增 resident sibling root

在 `SCENE` 下新增：

- `Town_Day1Residents`

并固定 3 个子组：

1. `Resident_DefaultPresent`
2. `Resident_DirectorTakeoverReady`
3. `Resident_BackstagePresent`

#### 建议 2：保留 `Town_Day1Carriers` 原名与 7 个 child 原名

不要重命名，也不要把它改造成 resident 根。

#### 建议 3：只给 7 个 carrier 补“粗粒度空间位”

这轮不求精修，不求 final polish，只求先脱离 `(0,0,0)` 空壳：

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `DinnerBackgroundRoot`
4. `NightWitness_01`
5. `DailyStand_01`
6. `DailyStand_02`
7. `DailyStand_03`

至少要进入“有真实空间语义”的阶段。

#### 建议 4：暂不在这轮把 resident 实体直接塞进 scene

原因是：

1. `NPC` 当前也在 active
2. 这轮更值钱的是先把 scene-side 容器层稳定下来
3. 真 resident actor 实体是否现在就进 scene，最好等下一轮 ownership 更清楚后再做

### 4. 修改后效果

如果按上面这版最小改动去做，效果会是：

1. `Town` 第一次拥有“本来就在村里的人”这一层 scene-side 容器
2. `Town_Day1Carriers` 不再被误当 resident 常驻根
3. 后续从 `Primary` 代理迁回 `Town` 时，不需要重新设计整套根层语义
4. `day1` 可以继续做代理，而 Town 这边已经把真实村庄承接层准备好

### 5. 对原有功能的影响

这套最小改动对现有功能的影响是可控的：

1. 不需要立刻改 `spring-day1` 当前 active 核心代码文件
2. 不会推翻现有 `semanticAnchorId` 体系
3. 不会推翻 `Town_Day1Carriers` 这一层已存在的导演语义
4. 主要新增的是：
   - resident 根层
   - scene-side 容器层
   - carrier 脱零位

---

## C. 驻村常驻化 scene-side 语义表

| 层 | 建议根名 | 含义 | 典型承接 |
| --- | --- | --- | --- |
| 常驻在场层 | `Town_Day1Residents/Resident_DefaultPresent` | 玩家未必看见，但人本来就在村里 | `101/103/104/201/202/203` |
| 导演接管预备层 | `Town_Day1Residents/Resident_DirectorTakeoverReady` | 已存在，但准备被导演拉到 carrier | `101/103`、部分饭馆背景人 |
| 语义存在但暂隐层 | `Town_Day1Residents/Resident_BackstagePresent` | 屋内 / 村边 / 夜路 / 远处 | `102/301`、未出镜背景人 |
| 导演锚点壳 | `Town_Day1Carriers/*` | 被导演接管后的具体锚点 | `EnterVillageCrowdRoot` 等 7 个锚 |

---

## D. 各 anchor 的常驻化承接等级

| Anchor | 当前定位 | 是否适合直接承接 resident actor |
| --- | --- | --- |
| `EnterVillageCrowdRoot` | 进村围观 / 让位 takeover 位 | 否，更适合作为 director carrier |
| `KidLook_01` | 单点好奇视线位 | 否，更适合作为 director carrier |
| `DinnerBackgroundRoot` | 饭馆生活背景位 | 是，适合后续直接承接 resident actor |
| `NightWitness_01` | 夜间被看见位 | 否，应先从 backstage 存在，再被接管 |
| `DailyStand_01` | 次日农事常驻位 | 是 |
| `DailyStand_02` | 次日观察常驻位 | 是 |
| `DailyStand_03` | 次日边缘职业常驻位 | 是 |

---

## E. 当前最窄 blocker 与 reopen 条件

### 当前最窄 blocker

不是泛泛的 `Town 还没 ready`。

而是：

- `Town` 还没有 resident 根层，carrier 仍是零位空壳

### 什么时候 reopen 成 scene 写入刀

命中下面任一条，就值得从 docs-only 切到 scene-side 真实施工：

1. 用户明确开放 `Town.unity` 写窗
2. `day1` 明确要把 `Primary` 代理承接迁回 `Town`
3. 当前 ownership 已允许我去写 resident 根层和 carrier 真实空间位
