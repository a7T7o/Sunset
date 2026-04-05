# 2026-04-05｜给典狱长｜Town 最小 contract 接管权裁定与外部噪声归类

这轮不再讨论 Town 重要不重要。

这轮只回答 3 件事：

1. `Town` 现在能不能直接接手最小 runtime contract 修改
2. 当前 fresh 噪声里哪些是 Town 自己的，哪些不是
3. 如果现在不能接手，Town 还能在不越权前提下推进到哪里

---

## A. 用户可读层

### 1. 当前主线

当前 `Town` own 主线已经很窄了：

- 不再回头做基础设施清扫
- 不再重讲边界
- 只判断能不能直接补 `semanticAnchorId -> runtime spawn` 这一刀

### 2. 这轮实际做成了什么

这轮把最关键的接管权问题钉死了：

1. **当前不能安全由 Town 线直接接手最小 contract 修改**
2. 当前 fresh console 里出现的新红，也**不是 Town own blocker**
3. 在不越权前提下，Town 还能继续推进的是：
   - 更精确地归类外部噪声
   - 明确 `day1` 在什么阈值把球交回给 Town
   - 把“什么时候必须由 Town 接手”写成硬条件

### 3. 为什么现在不能直接接手

原因很硬，不是感觉问题：

1. `spring-day1` 当前仍然是 `ACTIVE`
2. 它当前 active slice 是：
   - `live-rehearsal-and-deeper-town-staging-2026-04-05`
3. 它在线程状态里显式 own：
   - [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
   - [SpringDay1DirectorStageBook.json](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json)
4. 当前 working tree 里：
   - [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 仍在 dirty
   - [SpringDay1DirectorStageBook.json](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json) 是新增中的活文件
   - [SpringDay1NpcCrowdManifest.asset](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset) 也仍在 dirty

虽然 `Manifest.asset` 没写进 `spring-day1` 的 thread-state own paths，但它现在的脏改内容已经和 `Town` crowd contract 直接耦合：

- 新增 `sceneDuties`
- 新增 `semanticAnchorIds`
- 新增 `growthIntent`

所以现在如果 Town 线去改这 3 个触点，本质上就是直接撞进 `spring-day1` 当前 live 施工面。

### 4. 当前最真实的裁定

当前裁定只有一句：

- **Town 现在不能越权接这刀**

更准确地说：

- `Town` 已经走到“知道下一刀该改哪”
- 但还没走到“现在就可以自己改”

### 5. 当前 fresh 噪声该怎么归类

这轮 fresh 证据里有两类容易误判的东西：

#### `PersistentManagers` 那条异常

定性：

- **编辑态 manager bootstrap 噪声**

不是：

- `Town` scene wiring 自己炸了

原因：

1. [PersistentManagers.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/PersistentManagers.cs) 在 `InitializeIfNeeded()` 里无条件调用 `DontDestroyOnLoad(gameObject)`
2. 这条调用在 **编辑态 / 测试态 / 非 Play Mode** 触发时，就会报 Unity 的 editor-only 异常
3. 它说明的是：
   - 某条外部链在编辑态唤起了 manager bootstrap
4. 它不说明：
   - `Town` scene 本身已经重新带坏

因此它当前不该再被当成 `Town` first blocker。

#### 当前 fresh CLI 里的新红

当前 fresh `status/errors` 里站着的是：

- `[CodexNpcTraversalAcceptance] bridge_natural_probe_fail ...`

定性：

- **导航线程 live probe 外部噪声**

不是：

- `Town` contract blocker

原因：

1. 当前 active scene 已经被外线切到 `Primary`
2. 报错文件落在：
   - `Assets/Editor/NPC/CodexNpcTraversalAcceptanceProbeMenu.cs`
3. 它属于：
   - `导航检查 / 导航检查V2`
   这类外线 live 取证链

### 6. 在不能越权前提下，Town 还能推进到哪里

还能继续推进，但只能推进到这三层：

1. **接管阈值层**  
   明确什么时候必须把球交回 Town
2. **噪声归类层**  
   把外线红和 Town 自身 blocker 继续分开
3. **anchor 升级门槛层**  
   继续把 `EnterVillageCrowdRoot / KidLook_01 / NightWitness_01 / DinnerBackgroundRoot / DailyStand_01`
   的升级门槛写得更窄、更硬

### 7. 什么时候必须把球交回 Town

我现在给出 4 个硬触发条件：

1. `spring-day1` 需要让 runtime 真正按 `semanticAnchorId` 在 Town 里落点  
   - 这时球必须回 Town
2. `spring-day1` 当前 active slice 停车，且明确允许接手 `CrowdDirector + StageBook + Manifest`  
   - 这时 Town 才能合法接刀
3. live 排练已经站住，但首次真正需要 `EnterVillageCrowdRoot / KidLook_01 / NightWitness_01` 的 Town 实位 spawn  
   - 这时也必须回 Town
4. 当前 fallback 到旧 `Primary` 锚已经开始妨碍 `day1` 继续推进  
   - 这时不该再让 `day1` 自己硬扛

### 8. 哪个 anchor 最先值得回球

顺序不变：

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `NightWitness_01`
4. `DinnerBackgroundRoot`
5. `DailyStand_01`

但现在这张表的意义从“升级顺序”进一步收窄成了：

- **回球顺序**

也就是：

- `day1` 真开始撞 runtime 时，最先把哪一个交回 Town

### 9. 我这轮实际推进到了哪一层

这轮 Town own 线已经推进到：

- 把“能不能接刀”裁清了
- 把“哪些噪声不是我 own”裁清了
- 把“什么时候必须把球交回我”写成了硬条件

但这轮没有推进到：

- 真改 contract 代码
- 真接手 `CrowdDirector / StageBook / Manifest`

---

## B. 技术审计层

### 1. 核查依据

1. `Show-Active-Ownership.ps1`
2. [spring-day1.json](/D:/Unity/Unity_learning/Sunset/.kiro/state/active-threads/spring-day1.json)
3. [spring-day1/memory_0.md](/D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/spring-day1/memory_0.md)
4. `git status --short -- CrowdDirector / Manifest / StageBook`
5. `git diff -- Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
6. `py -3 scripts/sunset_mcp.py status`
7. `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 10`
8. [PersistentManagers.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/PersistentManagers.cs)

### 2. 关键证据

1. `spring-day1` 当前 `ACTIVE`
2. 其 own paths 明确包含：
   - `SpringDay1NpcCrowdDirector.cs`
   - `SpringDay1DirectorStageBook.json`
3. 当前 working tree：
   - `M  Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
   - `A  Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json`
   - ` M Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
4. `Manifest.asset` 当前 diff 已实际写入：
   - `sceneDuties`
   - `semanticAnchorIds`
   - `growthIntent`
5. fresh CLI：
   - 当前 active scene = `Primary`
   - 当前 red 来自 `CodexNpcTraversalAcceptanceProbeMenu.cs`
   - 不是 `Town` own 合约面
6. `PersistentManagers.cs:164`
   - `DontDestroyOnLoad(gameObject)` 在编辑态触发时会生成 editor-only 异常

### 3. 本轮最终裁定

1. **Town 当前不能安全接手最小 contract 修改**
2. **PersistentManagers 当前应归类为编辑态 manager bootstrap 噪声，不是 Town first blocker**
3. **Town 当前最深可推进面，是接管阈值、噪声归类和回球条件，而不是越权改代码**

