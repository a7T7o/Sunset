# 2026-04-05｜给典狱长｜Town 各 anchor 可承接等级表 + Town 升级条件 + 剩余 blocker 推进图

这轮不再重裁 `spring-day1 / Town` 导演边界。

当前唯一主线固定为：

- 把 `Town` 继续治理到“不能再推进”为止
- 只回答 `Town` 还剩什么、谁该继续、谁该停、哪个 anchor 先升级、以及当前 first blocker 到底是谁

---

## A. 用户可读层

### 1. 当前主线

当前 `Town` 已不是“边界没说清”的问题。

`spring-day1` 已经把 `Town` 可消费面下沉成正式正文 `10 / 11 / 12`，所以治理位现在最值钱的工作，不再是重讲“哪些戏该在 Town”，而是：

1. 给 `Town` 各 anchor 定承接等级
2. 给 `Town` 定升级条件
3. 重审还活着的 blocker owner
4. 只把 prompt 发给真正还该继续的人

### 2. 这轮实际做成了什么

这轮把 `Town` 相关 active owner 重新审了一遍，且结论比 `03` 更新：

1. `Town` 相机线不再是当前 blocker  
   - `工具-V1` 已 `PARKED`
   - `Town` 相机跟随已按 `用户已测通过` 收口
2. `PlacementManager.cs` 也不再是当前 first compile blocker  
   - 当前 fresh `status` 里真正站着的编译红，已经换成：
   - `Assets/Editor/TilemapToColliderObjects.cs` 的 `CS0051`
   - 这条红来自 `scene-build-5.0.0-001` 当前 active 线
3. `Town` 现在真正还活着的主 blocker，被重写成两条半：
   - `scene-build-5.0.0-001` 的 editor compile red
   - `UI` 的 `DialogueUI / 字体链`
   - `导航检查V2` 的 Town 正式场景 NPC live 证据仍未闭环，但它不是当前 first blocker，而是第二梯队 runtime 条件
4. 已把 `Town` 各 anchor 收成一张升级表，明确谁最先值得升级

### 3. Town 全局还剩哪些未完成项

当前 `Town` 全局剩余未完成项只剩 5 类：

1. **全局编译层**  
   `TilemapToColliderObjects.cs` 当前 `CS0051` 还在 fresh console 里，先挡住了稳定开发入口。
2. **玩家可见层**  
   `DialogueUI / 中文字体链` 仍未形成 `Town` 侧 live 过线证据。
3. **正式场景 runtime 证据层**  
   `Town` 里 NPC 正式场景的 live capture 还没有在干净现场下闭环，`导航检查V2` 之前拿到的是“静态补口已做，但 live 被外部噪声挡住”。
4. **anchor 升级层**  
   现在 anchor 还主要停在“导演可消费 / 静态可承接”，还没进入“runtime 可正式交给别线消费”。
5. **治理收口层**  
   `Codex规则落地` own root 里仍有历史 dirty，治理位这轮可以合法停车，但还不适合把整个 own root 当 clean 现场宣称 `sync-ready`。

### 4. Town 各 anchor 可承接等级表

先固定等级定义：

- `L1`：只有语义名，还不能被别人直接消费
- `L2`：导演层可消费
- `L3`：锚点矩阵已成，静态承接可排
- `L4`：已具 runtime 候选资格，但仍被外线 blocker 卡住
- `L5`：runtime 可正式交给别线消费

| anchor | 当前等级 | 当前判断 | 升级到下一层还缺什么 |
| --- | --- | --- | --- |
| `EnterVillageCrowdRoot` | `L4` | 当前最接近 runtime 化的第一锚点；导演分场、群像矩阵、落位优先序都已给到 | 先清 `compile red`，再让 `UI` 与 `导航检查V2` 在稳定 Town 现场拿到 live 证据 |
| `KidLook_01` | `L3` | 语义与群像职责已清，但仍更偏“静态可排” | 需要先跟着 `EnterVillageCrowdRoot` 过一轮 runtime 承接验证 |
| `DinnerBackgroundRoot` | `L3` | 晚餐背景层职责清楚，适合导演继续写，不适合现在假装 live ready | 需要玩家面字体链稳定、晚餐段 Town 背景层 live 可读 |
| `NightWitness_01` | `L3` | 夜间见闻层已可导演消费，但夜景 runtime 还没被 Town 正式验证 | 需要 Town 夜间段 live capture 和 NPC 正式场景证据 |
| `DailyStand_01` | `L2` | 已有正式语义，但更像“次日生活层待接” | 需要前面 crowd/night 两类 runtime 锚点先过线 |
| `DailyStand_02` | `L2` | 同上 | 同上 |
| `DailyStand_03` | `L2` | 同上 | 同上 |

### 5. 哪个 anchor 最先值得升级

当前最先值得升级的是：

- `EnterVillageCrowdRoot`

原因不是它“最显眼”，而是它同时满足 3 条：

1. 它已经是 `12` 号文档里写死的 runtime 落位优先顺序第 1 位
2. 它能最早检验 `Town` 是否真的从“导演可消费”推进到了“runtime 可承接”
3. 一旦它先过线，`KidLook_01`、`DinnerBackgroundRoot`、`NightWitness_01` 的升级判断都会更容易

### 6. 当前 first blocker 是什么

当前 first blocker 已不再是旧表里的 `PlacementManager.cs`，而是：

- `scene-build-5.0.0-001` 当前 shared-root 新增的 [TilemapToColliderObjects.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/TilemapToColliderObjects.cs) 编译红

定性：

- 这是 **全局 editor compile blocker**
- 它不只挡 `Town`，但它会先挡住 `Town` 后续任何想做稳定 live / validate 的动作
- 在它没清掉前，后面的 `UI live` 与 `导航 live` 证据都容易继续带噪

### 7. 哪个线程继续、哪个停、哪个无需继续发

#### `继续发 prompt`

1. `scene-build-5.0.0-001`
   - 原因：当前 first blocker 已转成它 own 的 editor compile red
2. `UI`
   - 原因：`DialogueUI / 字体链` 仍是 `Town` 当前唯一明确还活着的玩家面 blocker

#### `停给用户验收`

无。

原因：

- `Town` 还没到用户终验整条生活面的时候

#### `停给用户分析 / 审核`

无。

原因：

- 当前不缺边界裁定，缺的是把 blocker 真往前推

#### `无需继续发`

1. `019d4d18-bb5d-7a71-b621-5d1e2319d778`
   - `Town` 相机跟随已按 `用户已测通过` 收口
2. `导航检查`
   - 当前玩家 traversal 不再是 `Town` 的 first blocker
3. `农田交互修复V3`
   - 当前已无 `PlacementManager.cs` fresh compile red 证据支持它继续作为 `Town` 第一阻断 owner
4. `spring-day1`
   - 导演正文已足够，不该再拿它背 `Town` 当前推进停滞
5. `NPC`
   - 当前它应沿自己群像/内容线推进，但不是这轮 `Town blocker` 直接 owner

#### `继续 active 但这轮不追加 Town prompt`

1. `导航检查V2`
   - 它仍与 `Town` 的正式场景 NPC runtime 证据有关
   - 但当前不是 first blocker
   - 在 compile/UI 两层未先压干净前，再催它继续很容易重复卡在外部噪声里

### 8. 当前 Town 升级条件

`Town` 想从“导演层可消费”升级到“更接近 runtime 可承接”，当前至少要满足这 4 条：

1. **先 compile clean**  
   先清 `TilemapToColliderObjects.cs` 当前的 `CS0051`
2. **再玩家面可读**  
   `Town` 里的 `DialogueUI / 中文字体链` 至少要过一轮 fresh live
3. **再正式场景 runtime 证据**  
   `导航检查V2` 必须在较干净的 Town 现场抓到 NPC 正式场景的 live 真样本，而不是继续被外部噪声挡住
4. **最后才谈 anchor 升级**  
   先让 `EnterVillageCrowdRoot` 迈到 `L5` 候选，再依次外溢到 `KidLook_01 / DinnerBackgroundRoot / NightWitness_01`

### 9. 下一轮最值得继续施工的是谁

如果只选一个，当前最值得继续施工的是：

- `scene-build-5.0.0-001`

因为它手里握着当前最前面的 compile blocker。

如果允许两条并行，则顺序是：

1. `scene-build-5.0.0-001`
2. `UI`

`导航检查V2` 当前不该再被催成第一刀。

### 10. 我自己这轮实际推进到了哪一层

这轮治理位已经推进到：

- **不是只读复述边界**
- 而是把 `Town` 当前剩余 blocker 的 owner 图、anchor 承接等级表、升级条件和下一轮发单顺序，全部重写成了新版真值

但这轮还没有推进到：

- 直接替外线线程修代码
- 或替 `Town` 自己再开新 own slice

治理位当前最正确的动作是：

1. 只发最小必要 prompt
2. 更新总闸真值
3. 自己合法停车

---

## B. 技术审计层

### 1. 本轮重审依据

1. [2026-04-05_给典狱长_spring-day1与Town导演协同全景说明_06.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/2026-04-05_给典狱长_spring-day1与Town导演协同全景说明_06.md)
2. [2026-04-05_给典狱长_导演正文同步与Town后续承接提示_07.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/2026-04-05_给典狱长_导演正文同步与Town后续承接提示_07.md)
3. [2026-04-05_Town场景健康live复核与blocker重裁定_03.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/2026-04-05_Town场景健康live复核与blocker重裁定_03.md)
4. `Show-Active-Ownership.ps1` 最新输出
5. `UI / 019... / 导航检查 / 导航检查V2 / scene-build-5.0.0-001 / 农田交互修复V3` 最新 memory 尾部
6. `py -3 scripts/sunset_mcp.py status`

### 2. 本轮最关键的新证据

1. 当前 active owner 里：
   - `scene-build-5.0.0-001` 是 `ACTIVE`
   - `019...` 是 `PARKED`
   - `农田交互修复V3` 是 `PARKED`
2. 当前 fresh `status` console 错误只有 1 条：
   - `Assets\\Editor\\TilemapToColliderObjects.cs(177,24): error CS0051`
3. [TilemapToColliderObjects.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/TilemapToColliderObjects.cs) 当前是 untracked 新文件，且来源与 `scene-build-5.0.0-001` 最新 memory 一致
4. `PlacementManager.cs` 当前不在 working tree dirty 面里，且这轮没有 fresh console 证据支持它仍是 Town 当前 compile blocker
5. `工具-V1` memory 已明示：
   - `Town` 相机跟随按 `用户已测通过` 收口
6. `导航检查V2` 最新 memory 已明示：
   - 静态补口已做
   - 但 `Town` live 当时被 `Unknown script / Occlusion` 外部噪声挡住

### 3. 当前治理位 own 收口边界

1. 这轮没有 reopen `Town.unity`
2. 这轮没有去改 `UI / scene-build / 导航` 外线代码
3. 这轮新增产物只应留在：
   - `Codex规则落地`
   - `Codex规则落地` 线程记忆
4. 当前 same-root 仍有历史 dirty，因此治理位这轮应以：
   - `更新真值 + 最小分发 + 合法停车`
   为收口，而不是冒充已经 `sync-ready`

### 4. 本轮最终裁定

- `Town` 全局未完成项：已重写为 `compile red + UI/font + runtime live evidence + anchor upgrade`
- `Town` 当前第一升级锚点：`EnterVillageCrowdRoot`
- `Town` 当前 first blocker：`scene-build-5.0.0-001 / TilemapToColliderObjects.cs`
- 下一轮最值得继续施工：`scene-build-5.0.0-001`，其次 `UI`
