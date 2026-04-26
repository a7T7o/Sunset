# 2026-04-18 world-state 跨场景连续性只读排查

## 当前主线

- 用户目标：只读排查 Sunset 项目里跨场景 world-state 连续性问题，不改代码。
- 本轮子任务：围绕 `PersistentPlayerSceneBridge / SaveManager / SaveDataDTOs` 与树、石、箱子、掉落物、农地、作物、门切场这几类对象，盘清“已有合同 / 缺口 / 最可能卡点 / 打包前最安全的最小修优先级”。
- 子任务服务于：在真正施工前，先把 world-state 是“会保存、会回放，还是会补跨天”这三层边界讲清楚，避免顺手误修。
- 恢复点：如果后续继续，优先从掉落物合同入手，再决定是否补树木 off-scene catch-up 语义。

## 本轮已完成

1. 补读并复用两条最近相关只读线：
   - `2026-04-17_背包toolbar箱子跨场景只读排查`
   - `2026-04-17_farm跨天与off-scene合同只读审计`
2. 静态核对以下文件：
   - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
   - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
   - `Assets/YYY_Scripts/Data/Core/PersistentObjectRegistry.cs`
   - `Assets/YYY_Scripts/Data/Core/DynamicObjectFactory.cs`
   - `Assets/YYY_Scripts/Controller/TreeController.cs`
   - `Assets/YYY_Scripts/Controller/StoneController.cs`
   - `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
   - `Assets/YYY_Scripts/World/WorldItemPickup.cs`
   - `Assets/YYY_Scripts/World/WorldItemPool.cs`
   - `Assets/YYY_Scripts/UI/Utility/ItemDropHelper.cs`
   - `Assets/YYY_Scripts/Farm/FarmTileManager.cs`
   - `Assets/YYY_Scripts/Farm/CropController.cs`
   - `Assets/YYY_Scripts/DoorTrigger.cs`
   - `Assets/YYY_Scripts/Service/Inventory/InventoryService.cs`
3. 明确当前主合同：
   - `SaveManager` 只把当前 active scene 的 `worldObjects` 直接交给 `PersistentObjectRegistry`
   - 已离场 scene 通过 `PersistentPlayerSceneBridge.offSceneWorldSnapshots` 入盘
   - 回场时由 bridge 做 snapshot restore / reconstruct / prune
4. 明确当前跨天消费面：
   - `TreeController.ApplyOffSceneElapsedDays(...)`
   - `CropController.ApplyOffSceneElapsedDays(...)`
   - `FarmTileManager.ApplyOffSceneElapsedDays(...)`
5. 钉死两个掉落物高风险断点：
   - `TreeController.SpawnDrops()/SpawnStumpDrops()` 没给掉落物写 `sourceNodeGuid`
   - `WorldItemPickup.Reset()` 没清 `sourceNodeGuid`，`ItemDropHelper` 复用对象池生成玩家丢弃物时也不会重置它

## 关键判断

1. bridge 级跨场景 world-state snapshot 已经存在，而且不是空壳：
   - `CaptureSceneWorldRuntimeState()`
   - `RestoreSceneWorldRuntimeState()`
   - `ExportOffSceneWorldSnapshotsForSaveInternal()`
   - `ImportOffSceneWorldSnapshotsFromSaveInternal()`
2. 农地 / 作物是当前合同最完整的一组：
   - `FarmTileManager` 管空地寿命与水分残留
   - `CropController` 管成长、缺水、成熟后过熟、季节切换
   - bridge 还专门用了 `FarmTileManager -> Crop` 的恢复优先级，以及 `FinalizeDeferredSceneWorldCatchUp(...)`，避免 manager 抢跑空地清理
3. 树木合同是“部分闭环”：
   - 有跨场景 snapshot
   - 有跨天 `ApplyOffSceneElapsedDays(...)`
   - 但 off-scene catch-up 只补成长，不补天气枯萎 / 冬季雪冻 / 冬季树苗销毁这类 runtime 事件
4. 石头 / 箱子主要是“跨场景存在性与内容连续”，不是“跨天语义对象”：
   - 石头保存阶段/矿种/血量
   - 箱子保存位置/旋转/锁状态/库存
   - 但两者都没有 elapsed-days 消费面
5. 掉落物是当前最像真 bug 的地方：
   - `DynamicObjectFactory` 对掉落物有“来源节点还活着就别重建”的保护
   - 这要求 `sourceNodeGuid` 生产与清理必须稳定
   - 现在树掉落不写、对象池又不清，保护条件本身就会失真

## 验证状态

- 已完成：静态代码审计。
- 未完成：Unity live 往返、跨天矩阵、打包实测。
- 当前结论：`静态推断成立`。

## 下一步

- 如果继续真实施工，最安全的最小顺序：
  1. 先修掉落物 `sourceNodeGuid` 合同
  2. 再补树木 off-scene catch-up 的冬季 / 天气语义
  3. 最后才考虑更广的 save/load 位置恢复或 UI/runtime 附属态

## 2026-04-18 追加｜0417 world-state / farm / Day1 尾项只读复核

### 当前主线

- 用户目标：继续服务 `0417.md` 主控板，只读审查 `world-state / farm / Day1` 里到底还差哪些真实代码缺口。
- 本轮子任务：重点复核 `PersistentPlayerSceneBridge / FarmTileManager / CropController / SaveManager / StoryProgressPersistenceService / SpringDay1Director` 与两份合同测试。
- 子任务服务于：把任务板上“仍未闭环”的旧描述清成真正的代码缺口清单，避免把 live 待测误判成代码没做。
- 恢复点：如果后续继续，优先补 Day1 source-contract 覆盖，再做 live 冒烟；不要先回头重修 farm 主业务。

### 本轮已完成

1. 对照 `0417.md`、`存档系统/memory.md` 与以下源码/测试做静态审查：
   - `PersistentPlayerSceneBridge.cs`
   - `FarmTileManager.cs`
   - `CropController.cs`
   - `SaveManager.cs`
   - `StoryProgressPersistenceService.cs`
   - `SpringDay1Director.cs`
   - `WorldStateContinuityContractTests.cs`
   - `SaveManagerDay1RestoreContractTests.cs`
2. 核实到：
   - `farm off-scene catch-up` 已在源码落地，不再只是待设计：
     - bridge 有 `capturedTotalDays`
     - restore 后先 `CropController.ApplyOffSceneElapsedDays(...)`
     - 再 `FarmTileManager.ApplyOffSceneElapsedDays(...)`
   - `Day1` 读档恢复的 stale UI / pause 清场链已在 `SaveManager.ResetTransientRuntimeForRestore(...)`
   - `SpringDay1Director` 的 `Town/Home` 切场在当前文件里已统一走 `LoadSceneThroughPersistentBridge(...)`
3. 缩小真正剩余缺口：
   - 目前更像“Day1 合同测试缺口”，不是主业务代码大面积缺失
   - `SaveManagerDay1RestoreContractTests.cs` 没把 `StoryProgressPersistenceService` 的 Day1 canonical restore 覆盖并进同一组 source-contract

### 关键判断

1. `world-state / farm` 里最容易误判成“还没做”的那条，代码层其实已经比 `0417` 尾部注记更靠前。
2. `Day1` 恢复链在本轮抽检文件里没有发现新的高置信断链；剩余更偏向 live / packaged 端到端验证不足。
3. 我这轮最薄弱点是：没有跑 Unity live，只能把结论停在静态代码层；因此“合同已接上”不等于“玩家路径已过线”。

### 验证状态

- 已完成：静态代码审查。
- 未完成：Unity live / packaged 终验。
- 当前口径：`静态推断成立，live / packaged 未终验`。

## 2026-04-18 追加｜“加载存档后作物没有保留”专项只读排查

### 当前主线
- 用户目标：只读查清 Sunset 里“加载存档后作物没有保留”为什么发生，并顺带核查农地 / 浇水 / 作物 / 工作台进度这些 world-state 恢复链当前代码到底有没有闭环。
- 本轮子任务：限定在 `SaveManager / PersistentPlayerSceneBridge / SaveDataDTOs / FarmTileManager / FarmTileData / CropController / StoryProgressPersistenceService / CraftingService` 及相关 save/load/restore/apply snapshot 入口，不改代码。
- 子任务服务于：把当前真正保存了什么、没保存什么，以及最小安全修法建议压成可直接决策的结论。
- 恢复点：如果后续转施工，优先补 legacy 农田存档向新 `Crop` world object 的迁移，不先大改 bridge 主链。

### 本轮已完成
1. 静态审查：
   - `SaveManager.cs`
   - `PersistentPlayerSceneBridge.cs`
   - `SaveDataDTOs.cs`
   - `PersistentObjectRegistry.cs`
   - `DynamicObjectFactory.cs`
   - `FarmTileManager.cs`
   - `FarmTileData.cs`
   - `CropController.cs`
   - `StoryProgressPersistenceService.cs`
   - `CraftingService.cs`
   - 以及 `SaveManagerDay1RestoreContractTests.cs / WorldStateContinuityContractTests.cs / StoryProgressPersistenceServiceTests.cs`
2. 钉死正式主链：
   - 当前 scene world-state 进 `saveData.worldObjects`
   - off-scene world-state 进 `saveData.offSceneWorldSnapshots`
   - `FarmTileManager` 自己保存农地/浇水/空置寿命
   - `CropController` 自己保存作物种子、生长、缺水、成熟后天数与格子坐标
3. 找到最强缺口：
   - `FarmTileSaveData` 仍保留旧 crop 字段做“兼容”
   - 但 `FarmTileManager.CreateTileFromSaveData()` 完全不再读这些字段
   - 也没有任何一步把旧字段迁移成新的 `Crop` world object

### 关键判断
1. 当前格式的新档在静态代码层基本闭环，不能直接说“农地/作物主链没做”。
2. 最像“读档后作物丢失”的真实代码缺口，是旧档兼容断层：
   - 旧档只会恢复耕地，不会恢复 crop 本体
   - 所以玩家看到的是“地还在，作物没了”
3. `GameSaveData.farmTiles` 根字段现在已经是死 schema，不是当前主链。
4. 工作台/任务进度需要分层：
   - Day1 教学 / 剧情 / `craftedCount` / 木头进度 / hint 已进 `StoryProgressPersistenceService`
   - 工作台活跃制作队列明确不支持中途存档，`CanSaveNow()` 会拦
   - `CraftingService` 自身没有持久化层，所以通用 crafting 的 station/runtime unlock 不能算已闭环

### 验证状态
- 已完成：静态代码审查。
- 未完成：Unity live / packaged 复现。
- 当前结论：`静态推断成立`。

### 下一步
- 若继续真实施工，第一刀先做 legacy crop 迁移与合同测试，不先扩大到 bridge 全链重写。

## 2026-04-18 追加｜加载存档/重开后农地作物与工作台 world-state 闭环只读分析

### 当前主线
- 用户目标：快速钉死“加载存档/重开后作物、农地、浇水、工作台进度、跨场景 world-state 保存恢复为什么没闭环”的最小根因，并给出最安全修法落点。
- 本轮子任务：只读并行审查 `SaveDataDTOs / SaveManager / PersistentPlayerSceneBridge / FarmTileManager / CropController / CropInstanceData`，以及实际相关的工作台脚本 `SpringDay1WorkbenchCraftingOverlay / StoryProgressPersistenceService / SpringDay1Director`；不改代码。
- 子任务服务于：把“当前哪些链已存在、哪些链缺口仍在、用户为什么还会看到作物没保留、工作台是否已进入正式保存”压成可直接决策的结论。
- 恢复点：若后续转施工，优先补最小读档归一化与工作台显式 save contract，不先大改 bridge 主链。

### 本轮已完成
1. 只读核查正式存档根结构：
   - `GameSaveData` 当前正式使用 `worldObjects + offSceneWorldSnapshots`，`farmTiles` 只剩 legacy 兼容壳。
2. 只读核查当前 scene 正式读档主链：
   - `SaveManager.CollectFullSaveData()` 只收当前活动 scene 的 `PersistentObjectRegistry` world objects；
   - 已离场 scene 通过 `PersistentPlayerSceneBridge.ExportOffSceneWorldSnapshotsForSave()` 落进 `offSceneWorldSnapshots`；
   - 读档时先 `RestoreAllFromSaveData(current scene worldObjects)`，再 `ImportOffSceneWorldSnapshotsFromSave(...)`。
3. 只读核查农地/作物链：
   - `FarmTileManager.Save/Load` 已正式保存农地、水分、空置寿命；
   - `CropController.Save/Load` 已正式保存作物 seed、生长、缺水、成熟后天数、格子坐标；
   - `PersistentPlayerSceneBridge` 已在 off-scene restore 中按 `FarmTileManager -> Crop` 顺序恢复，并在回场后补 `ApplyOffSceneElapsedDays(...)`。
4. 只读核查工作台链：
   - 用户指定的 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\World\Interactables\WorkbenchController.cs` 仓库内不存在；
   - 实际相关状态散在 `SpringDay1WorkbenchCraftingOverlay.cs`、`StoryProgressPersistenceService.cs`、`SpringDay1Director.cs`。

### 关键判断
1. 农地/作物新格式主链并不是“完全没做”，当前代码层已经有：
   - 当前 scene 正式 save/load；
   - off-scene runtime snapshot；
   - 回场跨天补票。
2. 用户仍会看到“作物没保留”的最小高置信根因，不是新主链完全缺失，而是兼容层没闭环：
   - `FarmTileSaveData` 仍保留旧 crop 字段；
   - `FarmTileManager.CreateTileFromSaveData()` 不再消费这些旧 crop 字段；
   - 正式读档只会先恢复耕地，旧档里的 crop 不会被迁移成新的 `Crop` world object，
   - 结果就是“地还在，作物没了”。
3. 工作台进度/状态目前没有进入正式保存：
   - `StoryProgressPersistenceService` 只保存 `craftedCount / workbenchHintConsumed / 剧情阶段` 这类长期剧情态；
   - 活跃制作、队列总数、完成数、当前配方名在 load 时被强制清零；
   - `SpringDay1WorkbenchCraftingOverlay` 只有运行时 `_queueEntries/_activeQueueEntry`，没有 `IPersistentObject` 或正式 DTO。
4. 当前工作台的真实口径是“明确不支持制作中或待领取状态存档”，不是“已经保存只是恢复坏了”。

### 验证状态
- 已完成：静态代码审查。
- 未完成：Unity live / packaged 复现。
- 当前口径：`静态推断成立，live 未终验`。

### 建议的最小最安全修法
1. 农地/作物先补读档归一化，不先动 bridge 主链：
   - 落点优先在 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs` 的 `NormalizeLoadedSaveData()` / `PromoteLegacyFarmStateForLoad()` 一带，
   - 让 legacy `FarmTileSaveData.crop*` 在加载时统一提升成标准 `Crop` world object，而不是指望 `FarmTileManager.Load()` 重新理解旧字段。
2. `FarmTileManager` 继续只管土壤，不要把新版 crop 持久化逻辑重新塞回 tile：
   - 保持 `FarmTileManager.Load()` 只恢复农地和浇水；
   - 让 `CropController.Load()` 继续负责把 `tile.cropData` 和 `tile.cropController` 绑定回去。
3. 工作台若要闭环，最安全做法是新增显式 DTO/服务，不要偷塞进剧情快照：
   - 新增 `WorkbenchQueueSaveData` 一类正式结构；
   - 保存/恢复入口优先放在 `SpringDay1WorkbenchCraftingOverlay` 或单独 `WorkbenchPersistenceService`；
   - 然后由 `SaveManager.CollectFullSaveData()` / `ApplyLoadedSaveData()` 显式接入。
4. 如果短期不准备做工作台正式持久化，至少保留并强化当前 blocker 口径：
   - 继续在 `StoryProgressPersistenceService.CanSaveNow()` 拦住 active queue / ready outputs / floating state，
   - 避免用户误以为“可以正式保存工作台中途状态”。

## 2026-04-18 追加｜`重新开始` 后 Town world reset 不完整的只读归因

### 当前主线
- 用户目标：只读查清“重新开始游戏后，Town 的树/石头等 world object 没像 Primary 一样完整刷新”的最高概率根因，并压成方法级最小修法。
- 本轮子任务：限定在 `SaveManager / PersistentPlayerSceneBridge / SaveDataDTOs / TreeController / StoneController / WorldStateContinuityContractTests` 的 restart/world reset 直接链路，不改代码。
- 子任务服务于：解释为什么玩家会看到 `Primary 恢复、Town 不恢复` 的不对称现象，并给出 1 刀最小安全修法。
- 恢复点：若后续转施工，先只补 `SaveManager.NativeFreshRestartRoutine()` 的 restart 抑制逻辑，不先动 bridge 主干或资源控制器。

### 本轮已完成
1. 已确认普通跨场景离场链：
   - `PersistentPlayerSceneBridge.QueueSceneEntry()`
   - `CaptureSceneRuntimeState()`
   - `CaptureSceneWorldRuntimeState()`
   会把当前场景 `Tree/Stone/Chest/Crop/Drop/FarmTileManager` 抓进 `sceneWorldSnapshotsByScene`。
2. 已确认正式读档切场链有保护：
   - `SaveManager.BeginSceneSwitchLoad()` 会先 `SuppressSceneWorldRestoreForScene(targetSceneName)`
   - 因此目标场景刚加载时不会先吃 bridge 旧 snapshot。
3. 已确认 `重新开始` 链没有这层保护：
   - `SaveManager.NativeFreshRestartRoutine()` 直接 `LoadSceneAsync("Town")`
   - 没有先 `QueueSceneEntry()`
   - 也没有先 `SuppressSceneWorldRestoreForScene("Town")`
   - 所以 `PersistentPlayerSceneBridge.OnSceneLoaded() -> RebindScene() -> ScheduleSceneWorldRestore()` 仍可能把旧 `Town` snapshot 回放到 fresh Town 上。
4. 已确认 `ApplyNativeFreshRuntimeDefaults() -> ResetPersistentRuntimeForFreshStart()` 虽然会清空：
   - `sceneWorldSnapshotsByScene`
   - `sceneWorldSnapshotCapturedTotalDaysByScene`
   - `nativeResidentSnapshotsByScene`
   - `crowdResidentSnapshots`
   但这发生在 `Town` 载入并可能已被旧 snapshot 回放之后，不会把当前场景里已经被回放的树/石头再重建回 authored baseline。
5. 已确认 `Primary` 后续看起来恢复正常，是因为 restart 之后这些 off-scene snapshot 已被清空；再进 `Primary` 时拿不到 snapshot，最终只剩 authored 默认状态。

### 关键判断
1. 最高概率根因：
   - `SaveManager.NativeFreshRestartRoutine()` 缺少对 `Town` 的 `SuppressSceneWorldRestoreForScene()`，导致 restart 当前场景先吃旧 Town snapshot。
2. 促成用户体感不对称的直接原因：
   - `Town` 是“当前场景，先被旧 snapshot 回放，再清缓存”
   - `Primary` 是“之后进入的离场场景，缓存已空，直接回 authored baseline”
3. 最小安全修法：
   - 只在 `NativeFreshRestartRoutine()` 的 `LoadSceneAsync(NativeFreshSceneName)` 前补 `SuppressSceneWorldRestoreForScene(NativeFreshSceneName)`，
   - 并在异常/null 分支补 `CancelSuppressedSceneWorldRestore(NativeFreshSceneName)`。
4. 最值得补的测试票：
   - 给 `WorldStateContinuityContractTests` 补一张 restart contract，要求 `RestartToFreshGame / NativeFreshRestartRoutine` 对 `NativeFreshSceneName` 明确抑制 scene world restore，而不是只覆盖普通读档切场。

### 验证状态
- 已完成：静态代码审查。
- 未完成：Unity live / packaged 复现。
- 当前口径：`静态推断成立`。
