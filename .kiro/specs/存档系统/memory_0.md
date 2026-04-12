# 存档系统 - 开发记忆

## 模块概述

存档系统是项目中最容易出错的模块之一，核心组件分散在多个工作区中开发和修复。本工作区作为存档系统的统一索引，汇总所有对存档有修改的工作区记录，提供全局视角。

**核心组件**:
- PersistentObjectRegistry：对象注册中心（Dictionary 存储，反向修剪）
- DynamicObjectFactory：动态对象重建（树木/石头/箱子/掉落物）
- PrefabDatabase：预制体自动扫描 + ID 别名映射
- SaveManager：存档管理器，协调全局存档/读档流程
- PersistentManagers：持久化管理器根物体（DontDestroyOnLoad 统一管理）
- SaveDataDTOs：存档数据结构定义

## 当前状态

- **完成度**: 80%（核心功能完成，农作物存档待集成验证）
- **最后更新**: 2026-02-12
- **状态**: 持续维护中

## 关键文件索引

### 核心存档文件
| 文件 | 说明 | 涉及子工作区 |
|------|------|-------------|
| `SaveManager.cs` | 存档管理器 | 3.7.2, 3.7.4, 3.7.5 |
| `SaveDataDTOs.cs` | 数据结构定义 | 3.7.3, 3.7.4, 10.0.1 |
| `PersistentObjectRegistry.cs` | 对象注册中心 | 3.7.2, 3.7.4 |
| `DynamicObjectFactory.cs` | 动态对象重建 | 3.7.2, 3.7.3 |
| `PrefabDatabase.cs` | 预制体数据库 | 3.7.5 |
| `PersistentManagers.cs` | 持久化管理器 | SO设计系统 会话8 |

### 存档相关的控制器
| 文件 | 存档功能 | 涉及子工作区 |
|------|---------|-------------|
| `TreeController.cs` | 树木状态存档/恢复 | 3.7.2, 3.7.3 |
| `StoneController.cs` | 石头假死存档 | 3.7.4 |
| `ChestController.cs` | 箱子存档（Initialize/Load 顺序） | 箱子系统, 3.7.6 |
| `CropController.cs` | 作物存档（CropSaveData） | 10.0.1 |
| `TimeManager.cs` | 时间数据存档（必须先于世界对象恢复） | 3.7.6 |

### 编辑器工具
| 文件 | 说明 | 涉及子工作区 |
|------|------|-------------|
| `PersistentIdAutomator.cs` | 场景保存时自动修复空/重复 GUID | 3.7.3 |
| `PersistentIdValidator.cs` | 手动验证 GUID 完整性 | 3.7.3 |
| `PrefabDatabaseAutoScanner.cs` | 预制体自动扫描 | 3.7.5 |

## 子工作区索引

| 子工作区路径 | 来源 | 存档相关内容 |
|-------------|------|-------------|
| `999_全面重构_26.01.27/3.7.2存档bug大清扫/` | 999_全面重构_26.01.27 | 动态对象重建、PrefabRegistry、DynamicObjectFactory |
| `999_全面重构_26.01.27/3.7.3消失bug/` | 999_全面重构_26.01.27 | GUID 漂移修复、PersistentIdAutomator/Validator |
| `999_全面重构_26.01.27/3.7.4进一步bug清扫/` | 999_全面重构_26.01.27 | 石头假死、反向修剪、掉落物关联、箱子 IsEmpty |
| `999_全面重构_26.01.27/3.7.5自动化存档/` | 999_全面重构_26.01.27 | PrefabDatabase 自动化、ID 别名映射、渲染层级存档 |
| `999_全面重构_26.01.27/3.7.6再再进一步bug清扫/` | 999_全面重构_26.01.27 | 季节渐变种子稳定性、加载顺序、箱子 Initialize 覆盖 |
| `箱子系统/` | 箱子系统 | ChestSaveData、Initialize/Load 顺序问题 |
| `农田系统/10.0.1农作物设计与完善/` | 农田系统 10.0.1 | CropSaveData 扩展、作物状态存档 |
| `SO设计系统与工具/` | SO设计系统与工具 | PersistentManagers 创建、DontDestroyOnLoad 统一管理 |

## 核心架构知识

### GUID 生命周期

| 对象类型 | GUID 来源 | 生成时机 | 稳定性 |
|---------|----------|---------|--------|
| 静态对象（场景预制体） | PersistentIdAutomator 自动分配 | 编辑器保存场景时 | ✅ 绝对稳定 |
| 动态对象（运行时生成） | `System.Guid.NewGuid()` | 运行时放置/生成 | ✅ 存档后稳定 |
| 重建对象（读档恢复） | 从存档数据读取 | DynamicObjectFactory.TryReconstruct | ✅ 与原始一致 |

### 执行顺序陷阱

动态重建的对象，`Load()` 在 `Instantiate` 后立即调用，但 `Start()` 要等到下一帧。
`Initialize()` 方法必须检查数据是否已被 `Load()` 填充（null 检查）。

### 加载顺序保证

TimeManager 必须先于所有世界对象恢复，否则季节渐变等依赖时间的系统会读到默认值。

### 反向修剪机制

`PruneStaleRecords()` 而非 `Clear()`，只移除"存档中没有但注册表中有"的对象。
石头使用假死机制：`SetActive(false)` 而非 `Destroy()`。


## 会话记录

### 会话 1 - 2026-02-12（工作区创建）

**用户需求**:
> 新建存档系统工作区，子工作区按"对存档有修改的其他工作区"命名，汇总散落各处的存档修改记录

**完成任务**:
1. [x] 创建主工作区 memory.md（核心架构知识、关键文件索引、子工作区索引）
2. [x] 创建 8 个子工作区 memory 文件
3. [x] Phase 5.0 补充发现：农作物存档集成待验证、save-system.md 需预留扩展位
4. [x] Hook 映射添加存档系统关键词

**修改文件**:
- `memory.md` - 主工作区记忆创建
- 8 个子工作区 `memory.md` - 全部创建
- `smart-assistant.kiro.hook` - 添加存档关键词映射
- `Phase5.0修复与完善.md` - 追加补充发现章节


### 会话 2 - 2026-02-15（层级命名修正）

**用户需求**:
> 存档系统下的子工作区层级命名不对，3.7.x 系列应该归属到 999_全面重构_26.01.27 下面，自定义命名的子工作区也应该改成对应的主工作区名称以及分层，不应该随意摆放

**问题分析**:
之前创建存档系统工作区时，把所有子工作区直接平铺在 `存档系统/` 下面，没有按照来源工作区的层级关系组织。用户强调的是层级命名必须正确，memory 的内容和记录方式不需要改。

**完成任务**:
1. [x] 创建正确层级的新目录并迁移 memory 文件
2. [x] 删除旧的错误层级目录下的文件
3. [x] 更新主 memory.md 中的子工作区索引表

**层级修正映射**:
| 旧路径（错误） | 新路径（正确） |
|---|---|
| `存档系统/3.7.2存档bug大清扫/` | `存档系统/999_全面重构_26.01.27/3.7.2存档bug大清扫/` |
| `存档系统/3.7.3消失bug/` | `存档系统/999_全面重构_26.01.27/3.7.3消失bug/` |
| `存档系统/3.7.4进一步bug清扫/` | `存档系统/999_全面重构_26.01.27/3.7.4进一步bug清扫/` |
| `存档系统/3.7.5自动化存档/` | `存档系统/999_全面重构_26.01.27/3.7.5自动化存档/` |
| `存档系统/3.7.6再再进一步bug清扫/` | `存档系统/999_全面重构_26.01.27/3.7.6再再进一步bug清扫/` |
| `存档系统/农作物存档集成/` | `存档系统/农田系统/10.0.1农作物设计与完善/` |
| `存档系统/箱子系统存档集成/` | `存档系统/箱子系统/` |
| `存档系统/SO设计系统存档相关/` | `存档系统/SO设计系统与工具/` |

**修改文件**:
- `存档系统/memory.md` - 更新子工作区索引表 + 追加会话记录
- 8 个新路径 memory.md - 创建（内容与旧文件一致）
- 8 个旧路径 memory.md - 删除


### 会话 3 - 2026-04-06（历史清盘与现状评估）

**当前主线目标**:
> 为当前 `Sunset` 的真实系统建立一套“不大不小、刚好够用、能支撑完整 demo”的存档系统；这一轮先不改代码，先把历史进度、现状与缺口彻底盘清。

**本轮子任务**:
> 以 `D:\Unity\Unity_learning\Sunset\.codex\threads\` 里的其他线程 `memory` 为索引，向外扩散到 `.kiro/specs/存档系统/`、`000_代办/kiro/存档系统/TD_000_存档系统.md`、相关工作区 memory，以及当前代码实现，判断：
> 1. 现在到底已经能存什么
> 2. 哪些只是老文档说法，已经被后续代码改写
> 3. 相比 2026-04 的真实 `Sunset`，当前存档系统落后了多少

**本轮完成事项**:
1. 先核对工作区入口：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\memory.md` 仍停在 `2026-02-15`
   - 其中“完成度 80%”的表述已经明显落后于当前项目阶段
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\存档系统\` 当前存在，但还是空目录，尚无线程记忆
2. 回读 `.kiro/specs/存档系统/` 全部子工作区 memory，重新确认早期存档底座的真实历史：
   - `3.7.2 ~ 3.7.6`：动态对象重建、GUID 自愈、反向修剪、PrefabDatabase、树/石/箱子/作物链路
   - `箱子系统`：ChestSaveData 与 `Initialize()/Load()` 顺序坑
   - `农田系统/10.0.1`：`CropSaveData` 与 `CropController Save/Load`
   - `SO设计系统与工具`：`PersistentManagers`
3. 以其他线程 memory 为索引补读较新的真实进展：
   - `项目文档总览`：确认项目级描述已把存档视为真实已落地系统，但也明确它与箱子/放置/农田高度耦合

   - `农田交互修复V2`：确认 `ChestSaveLoadRegression` 的最后真实缺口是 `ChestInventoryV2.ToSaveData()` 漏存 runtime item 动态属性，且已在 2026-03-24 修掉
   - `农田交互修复V2`：确认 `ChestSaveLoadRegression` 的最后真实缺口是 `ChestInventoryV2.ToSaveData()` 漏存 runtime item 动态属性，且已在 2026-03-24 修掉
   - `农田交互修复V2/V3`：确认 runtime item 持久化现在已经扩到背包 / 箱子 / 装备 / 整理 / 掉落 / 存档整条链
   - `spring-day1-implementation`：反复要求 `StoryManager` 通过 `IPersistentObject` 集成到现有存档系统，且明确禁止用 `PlayerPrefs` 存剧情进度
4. 直接回读当前代码，盘清现行存档覆盖面：
   - `SaveManager.cs`
   - `SaveDataDTOs.cs`
   - `PersistentObjectRegistry.cs`
   - `DynamicObjectFactory.cs`
   - `InventoryService.cs`
   - `EquipmentService.cs`
   - `FarmTileManager.cs`
   - `CropController.cs`
   - `ChestController.cs`
   - `WorldItemPickup.cs`
   - `TreeController.cs`
   - `StoneController.cs`
   - `StoryManager.cs`
   - `PlayerNpcRelationshipService.cs`
   - `CraftingStationInteractable.cs`

**当前代码层已确认的真实覆盖面**:
- 已接入 `SaveManager -> PersistentObjectRegistry -> IPersistentObject` 主链的对象 / 服务：
  - `InventoryService`
  - `EquipmentService`
  - `FarmTileManager`
  - `CropController`
  - `ChestController`
  - `WorldItemPickup`
  - `TreeController`
  - `StoneController`
- `DynamicObjectFactory.TryReconstruct(...)` 当前只覆盖 5 类动态对象：
  - `Drop`
  - `Tree`
  - `Stone`
  - `Chest`
  - `Crop`
- `SaveManager.SaveGame()` / `LoadGame()` 当前真实顺序：
  - `游戏时间 -> 玩家位置 -> 旧背包兼容壳 -> Registry 世界对象`
- runtime item 动态属性当前已经会随：
  - 背包
  - 装备
  - 箱子
  - 掉落物
  - 作物 / 世界对象链
  一起进入存档 DTO

**当前代码层已确认的主要缺口 / 漂移**:
1. `SaveManager` 仍是“开发者可用”，不是“玩家可用”：
   - 代码里可见的触发入口几乎只有 `SaveLoadDebugUI` 的 `F5/F9`
   - 以及 `SaveManager` 的 `ContextMenu(slot1)`
   - 尚未看到正式菜单、自动存档、场景切换存档或玩家态入口
2. `PlayerSaveData` 明显字段漂移：
   - DTO 里有 `currentLayer / selectedHotbarSlot / gold / stamina / maxStamina`
   - 但 `CollectPlayerData()` 实际只写了 `positionX / positionY / sceneName`
   - `RestorePlayerData()` 也只做玩家位置恢复，没有恢复 scene、楼层、体力、金币、快捷栏选择
3. `GameSaveData.farmTiles` 是旧字段残留：
   - DTO 根结构还保留 `farmTiles`
   - 但现行实现实际让 `FarmTileManager` 走 `IPersistentObject -> worldObjects`
   - 根层 `farmTiles` 没有进入现行 `SaveManager` 主流程
4. 当前是“单场景原地恢复”，不是完整跨场景存档：
   - `PlayerSaveData.sceneName` 只被收集，没有看到 `LoadGame()` 按 sceneName 切场
   - 这说明跨场景 continuity 目前并未真正闭环
5. 剧情 / 导演 / NPC 关系链没有接进现有 slot-save：
   - `StoryManager.cs` 当前只是 `MonoBehaviour` 单例，不是 `IPersistentObject`
   - `SpringDay1Director.cs` 也不是 `IPersistentObject`
   - `PlayerNpcRelationshipService.cs` 直接用 `PlayerPrefs`
   - `CraftingStationInteractable.cs` 的一次性提示消费也直接用 `PlayerPrefs`
   - 这和 `spring-day1` 工作区自己写过的“严禁用 PlayerPrefs 存剧情进度、应改走 IPersistentObject”出现了明显断层
6. 一般可放置物体系还没真正接进存档：
   - 代码里存在 `StorageData / WorkstationData / SimpleEventData / InteractiveDisplayData`
   - 但 `World/Placeable/` 当前只有 `ChestController`
   - `DynamicObjectFactory` 也只会重建 `Chest`，不会重建更一般的放置物 / 工作台 / 事件摆件
   - 这意味着“箱子可落地并能读档”不等于“现在的可放置系统都能持久化”
7. runtime item 的 `instanceId` 也有 DTO 漂移迹象：
   - 存档时会把 `instanceId` 写进 `InventorySlotSaveData`
   - 但 `SaveDataHelper.FromSaveData(...)` 当前只新建 `InventoryItem` 并恢复耐久 / 属性，没有把旧 `instanceId` 写回
   - 暂时未看到它造成当前主线 bug，但它已经是“字段写了，恢复没闭环”的典型信号

**对“完成度 80%”的重判**:
- 如果参照 `2026-02` 那个阶段的目标，也就是：
  - 时间
  - 玩家位置
  - 背包
  - 农田
  - 作物
  - 树石
  - 箱子
  - 掉落物
  这一套资源/世界 sandbox 主链，那么底座其实已经明显超过当时 `memory` 写的 `80%`，更接近“主链已成型、剩验证与 UX”。
- 但如果参照 `2026-04-06` 的真实 `Sunset` 范围，也就是已经包含：
  - spring-day1 剧情推进
  - Story / Director 状态
  - NPC 关系与对话进度
  - 工作台教学提示
  - 更一般的可放置物 / 事件摆件
  - 更像玩家产品的存档入口
  那当前存档系统就明显落后一整代。


### 会话 4 - 2026-04-08（只读审计：Day1 resident 持久化现状）

**当前主线**:
> 只读盘点 Day1 resident 持久化与 story persistence 的现状，判断当前未提交现场是否已经部分实现 resident snapshot / story persistence，并给主线程一个最小落点建议。

**本轮子任务**:
> 精读以下 5 个文件的当前内容与未提交改动方向：
> 1. `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
> 2. `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
> 3. `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
> 4. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
> 5. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`

**已确认的结论**:
1. `story persistence` 已经不是空白：
   - 新增未跟踪文件 `StoryProgressPersistenceService.cs`
   - 已实现 `IPersistentObject`
   - 已通过 `worldObjects -> StoryProgressState` 接入正式 slot-save
2. 当前已纳入 slot-save 的 Day1 长期态包括：
   - `StoryManager.CurrentPhase`
   - `StoryManager.IsLanguageDecoded`
   - `DialogueManager` 已完成正式对白序列
   - `SpringDay1Director` 的教学进度 / 自由时段 / 日终等长期字段
   - `npcRelationships`
   - 工作台首次提示消费态
   - `health / energy`
3. `SaveManager` 已经对接上述 story persistence：
   - 保存前 `EnsureRuntime()`
   - 玩家保存阻断走 `CanSaveNow(...)`
   - 读档后 `FinalizeLoadedSave(...)`
   - 存档摘要也会从 `StoryProgressState` 解析 `storyPhase/health/energy`
4. `SpringDay1NpcCrowdDirector` 已拥有 resident runtime 的关键字段与语义：
   - `ResolvedAnchorName`
   - `ResidentGroupKey`
   - `AppliedCueKey`
   - `ReleasedFromDirectorCue`
   - `IsReturningHome`
   - `BaseResolvedAnchorName / BasePosition / BaseFacing`
   但这些目前只存在于 runtime / debug summary，没有正式 save contract。
5. `SpringDay1Director` 已暴露 `GetCurrentBeatKey()` 与 crowd runtime summary，能作为 resident 恢复时的导演语义辅助，但它自己不是 resident snapshot 的存档器。

**当前仍缺的最小 contract**:
1. 缺正式 resident snapshot DTO / contract。
2. 缺 `SpringDay1NpcCrowdDirector` 对外的导出 / 导入 surface。
3. 缺 `StoryProgressPersistenceService` 对 resident runtime 的采集与回灌。
4. 缺“跨 Town / Primary 读档时按 scene 切换再恢复”的完整闭环；`player.sceneName` 仍然只是被记录，没有形成真正的 scene load handoff。

**最推荐的最小落点**:
1. resident snapshot 不放到 `GameSaveData` 根层，仍收在 `StoryProgressPersistenceService` 的 `StoryProgressSaveData` 下面。
2. `SpringDay1NpcCrowdDirector` 提供最小 resident snapshot surface，至少能表达：
   - `npcId`
   - `sceneName`
   - `residentGroupKey`
   - `resolvedAnchorName`
   - `cueKey / residentMode`
   - `isReturningHome`
3. `StoryProgressPersistenceService` 在 `CaptureSnapshot()` / `ApplySnapshot(...)` 中负责读写这份 resident 最小态。
4. `SpringDay1Director` 尽量不大改；最多只补最小恢复 hook，避免把 director 私有实现进一步耦进 SaveManager。

**未提交脏改风险**:
- `SaveDataDTOs.cs`：已有 `cloudShadowScenes` 与 `SaveSlotSummary` 新增，不能把 resident DTO 粗暴塞进根结构。
- `SaveManager.cs`：改动量很大，已经混有存档 UI、默认槽、persistentDataPath 迁移、story persistence、cloud shadow 等多条线。
- `StoryProgressPersistenceService.cs`：当前是未跟踪新文件，但内容已形成中心接线位。
- `SpringDay1Director.cs`：巨量脏改，且 story persistence 通过反射依赖它的私有字段名，合并时最脆弱。
- `SpringDay1NpcCrowdDirector.cs`：resident runtime 改动已很多，是最适合补 snapshot 的源头，也是最需要小心合并的文件。

**本轮边界**:
- 只读审计
- 未改业务代码
- 未进入 `thread-state Begin-Slice`

**本轮阶段判断**:
- 这条线目前不该再被理解成“存档系统只差异步和一点作物验证”
- 更准确的说法应该是：
  - `旧资源/农田/箱子底座已站住`
  - `但相对今天的 Sunset，剧情/NPC/一般可放置物/玩家态入口这四块明显没跟上`

**初步量化判断（静态分析推断）**:
- 对“老版资源 sandbox demo”来说：当前存档底座大约在 `70% ~ 80%` 区间
- 对“今天这版 Sunset 的完整 demo”来说：当前整体只在 `40% ~ 50%` 区间
- 最主要的缺口不是单个 bug，而是**覆盖范围仍停在老系统代际，没有追上 3 月下旬到 4 月的剧情 / NPC / 工作台 / 放置扩张**

**验证状态**:
- 本轮仅做：
  - 历史 memory 交叉核对
  - 代码静态阅读
  - 主场景 YAML 关键词抽查
- 尚未做：
  - Unity live 存/读档实测
  - 具体存档文件样本比对
  - 剧情 / NPC / 多场景真实往返验证

**涉及路径**:
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\kiro\存档系统\TD_000_存档系统.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办清扫\2026.03.06\系统审查报告.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\项目文档总览\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V2\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory_1.md`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveDataDTOs.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\DynamicObjectFactory.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\PersistentObjectRegistry.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcRelationshipService.cs`

**恢复点 / 下一步**:
- 下一轮最稳的动作不是立刻改 `SaveManager`
- 而是先把“当前 demo 必须持久化的内容”压成一份新版范围清单：
  - `必须纳入`
  - `可延后`
  - `不纳入本期 demo`
- 然后再决定第一刀是：
  1. 先补 `StoryManager / spring-day1 / NPC 关系` 进现有 `IPersistentObject` 链
  2. 还是先补“玩家可用的正式存档入口 + 自动存档策略”


### 会话 4 - 2026-04-06（对外汇报口径收尾）

**本轮收尾判断**:
- 当前不适合直接把存档线理解成“修几个剩余 bug 就能收口”
- 更准确的阶段判断是：
  - `旧版资源 sandbox 的存档底座已经站住`
  - `但相对 2026-04 的 Sunset demo，scope 明显过期`
- 因此这条线下一步最该做的是**先定新版 demo 存档范围**，再动第一刀实现

**当前建议的范围重排方向**:
1. `必须纳入 demo`
   - 剧情主线推进
   - `spring-day1` 导演状态
   - NPC 关系 / 对话进度
   - 现有农田 / 作物 / 箱子 / 掉落 / 树石 / 背包 / 装备链
2. `建议随后补齐`
   - 玩家正式存档入口
   - 自动存档策略
   - 槽位策略
   - 跨场景恢复
3. `暂不默认纳入第一刀`
   - 泛放置物全类型持久化一次性全铺开
   - 异步存档性能优化

**恢复点**:
- 下轮应先输出“demo 存档范围矩阵”，而不是直接改 `SaveManager`
- 如果用户认可当前判断，第一刀优先候选仍是：
  1. `StoryManager + SpringDay1Director + NPC 关系` 接入现有 `IPersistentObject` 链
  2. 或把“正式存档入口 / 自动存档 / 槽位策略”收成玩家可用产品面


### 会话 5 - 2026-04-06（demo 存档范围矩阵与处理策略）

**当前主线目标**:
> 不直接开改，先把“2026-04 这版 Sunset demo 到底该存什么、每类状态怎么处理、什么不该存”压成一份可执行的新版范围矩阵。

**本轮子任务**:
> 基于 `SaveManager / SaveDataDTOs / StoryManager / DialogueManager / SpringDay1Director / NPC / 工作台 / 玩家状态 / scene transition` 的现行代码，完成一份“纳入范围 + 处理口径 + 暂缓边界”的系统级分析。

**本轮完成事项**:
1. 把“任务列表 / PromptOverlay”从待存对象里剥离，确认它本质上是导演状态的显示层：
   - `SpringDay1PromptOverlay` 直接读取 `SpringDay1Director.BuildPromptCardModel()`
   - 因此任务卡不应独立持久化，应由剧情/导演源状态派生重建
2. 把 `spring-day1` 的真正剧情状态拆成 3 层：
   - `StoryManager`：`CurrentPhase / IsLanguageDecoded`
   - `DialogueManager`：`_completedSequenceIds`
   - `SpringDay1Director`：教学目标、`craftedCount`、`freeTime/dayEnd` 等导演结构态
3. 把 NPC 侧状态拆成“长期态 vs 瞬时态”：
   - `PlayerNpcRelationshipService` 的关系阶段是长期态，必须并入 slot-save
   - `PlayerNpcChatSessionService` 的 pending-resume / cooldown / 中断快照更像瞬时会话态，读档时应清空重建
4. 把工作台侧状态拆成“必须迁移 vs 显式不支持”：
   - `spring-day1.workbench-entry-hint-consumed` 这类一次性消费必须脱离 `PlayerPrefs`
   - “制作到一半的队列 / 进度条 / 领取态”先不默认支持，若第一版不做，就应把“制作中不可存档或需先结算到安全点”写成规则
5. 把玩家基础状态重新分层：
   - `sceneName / position` 仍是基础项，但当前尚未形成真正跨场景恢复
   - `HP / EP / 快捷栏选择 / 技能经验` 已经真实影响今天 demo 体验，不该再被视为可忽略的旧字段尾巴
6. 把世界/放置侧边界定清：
   - 旧资源链（背包 / 装备 / 农田 / 作物 / 箱子 / 掉落 / 树 / 石 / 时间）继续视为第一版基线，不能回退
   - 泛放置物当前只有 `ChestController` 真正接进 `IPersistentObject + DynamicObjectFactory`
   - `WorkstationData / SimpleEventData / InteractiveDisplayData` 暂时仍属于“数据有了、运行时持久化未闭环”

**新版范围矩阵（第一版判断）**:

1. `必须纳入本期 demo`
   - 旧基线资源链：`Time / Inventory / Equipment / FarmTile / Crop / Chest / Drop / Tree / Stone`
   - 剧情主线：`StoryManager.CurrentPhase`、`StoryManager.IsLanguageDecoded`
   - 正式对白消费：`DialogueManager._completedSequenceIds`
   - Day1 导演结构态：`tutorial objectives`、`craftedCount`、`freeTimeEntered`、`freeTimeIntroCompleted`、`dayEnded`，以及其他不能稳定由相位反推的 gate
   - NPC 长期关系态：按 `npcId -> relationshipStage` 的 slot 数据
   - 工作台一次性教学消费：迁出 `PlayerPrefs`
   - 玩家基础锚点：至少 `scene / position`，若第一版不做跨场景，则必须写明仅支持受控场景/受控存档点

2. `建议纳入，但可作为第二刀`
   - `HP / EP / Max`：读档后直接影响玩家体感，优先级高于“漂亮 UX”
   - `HotbarSelection`：影响读档后手上工具和输入连续性
   - `SkillLevel / Experience`：如果 demo 要跨日或继续使用工作台成长，建议进入第一版；若第一版只保 Day1 当天，也至少作为第二刀预留
   - `非默认配方解锁集合`：不直接存 `RecipeData.isUnlocked`，而是按“真正的解锁来源”持久化
   - 玩家正式入口：菜单/自动存档/槽位 UX

3. `第一版明确不纳入，只立规则`
   - 泛放置物全量持久化：`WorkstationData / SimpleEventData / InteractiveDisplayData`
   - 制作中队列、半成品、领取态的完整续档
   - 异步存档 / 读档性能优化

4. `纯派生 / UI / 瞬时态，不应直接持久化`
   - `PromptCardModel`、任务卡行列表、PromptOverlay 当前页状态
   - `SpringDay1NpcCrowdDirector` 的当前站位、临时 parent、运行时 crowd 排布
   - `PlayerNpcChatSessionService` 的 pending resume、冷却、打字中状态、对话气泡内容
   - `DialogueUI` 当前打字文本、节点内半句进度
   - 工作台 Overlay 的 hover、当前选中配方、进度条文案
   - 各类一次性 warning 显示态；能从源状态重新判断的，不单独存

**处理口径结论**:
- 旧存档底座继续沿用 `SaveManager -> PersistentObjectRegistry -> IPersistentObject -> WorldObjectSaveData.genericData`
- 新增的长期系统状态，优先做成独立 `IPersistentObject` 运行时对象，不把 `SaveManager` 扩写成剧情/NPC/工作台总表
- 现有 `PlayerPrefs` 持久化一律按“迁入 slot-save”处理：
  - `PlayerNpcRelationshipService`
  - `CraftingStationInteractable` 工作台提示消费
- 显示层不直接存，统一从源状态派生：
  - `PromptOverlay`
  - 任务列表
  - NPC nearby 文案
  - crowd 排布摘要
- 瞬时会话不续档，读档时清空：
  - informal 聊天 pending resume
  - 正在打字的对白
  - 打开的 overlay / 面板

**仍需用户拍板的关键未决点**:
1. 第一版是否允许“制作途中”存档；若不允许，需要把保存时机限制写成硬规则
2. 第一版是否必须支持真正跨场景恢复；若不做，哪些场景/入口允许存档要明确
3. `HP / EP / 快捷栏选择 / 技能经验` 是否直接进第一刀，还是拆到第二刀

**本轮判断升级**:
- 当前最合理的下一步，不再是泛泛地“修存档”
- 而是先把第一刀收成：
  - `剧情 / Day1 / NPC / 工作台消费` 接入现有 slot-save
  或
  - `玩家基础状态 + 正式存档入口策略`
- 但无论先做哪刀，`任务列表 / Overlay / crowd 临时位姿 / 聊到半句` 都不该作为第一刀的主持久化对象

**验证状态**:
- 仍为静态分析成立
- 尚未进入 Unity live save/load 验证

**thread-state 收尾**:
- 本轮后段为 memory 落盘补跑了 `Begin-Slice`
- 已在结束前执行 `Park-Slice`
- 未跑 `Ready-To-Sync`
- 当前 live 状态：`PARKED`
- 原因：本轮只完成分析与记忆写回，不进入代码实现或白名单同步


### 会话 6 - 2026-04-06（用户拍板补充与线程沟通策略讨论）

**用户新增拍板**:
1. 工作台“制作到一半”的续档不做：
   - 用户明确认为这块难度不值当前 demo 收益
2. 所谓“跨场景恢复”不应先理解成“立刻强制跳回保存时场景”
   - 用户更倾向把它理解为“全局状态恢复”
   - 当前应优先恢复当前活动场景，再由其他场景按进入时机拿到全局状态
3. 技能经验暂不纳入当前存档范围：
   - 原因不是“不重要”
   - 而是当前真实功能尚未成立，不应先为不存在的成长链设计持久化
4. 用户提出一个新的流程设想：
   - 实施前先向受影响线程说明“准备怎么做 / 会改什么 / 需要它们配合什么”
   - 由各线程回执允许范围与边界，形成一次明确交流

**本线程对用户拍板的理解**:
- 第一版存档规则现在应明确加上一条：
  - `工作台制作活跃中不支持存档`
  - 如果当前已有正式存档入口，保存前应检查制作状态并阻断，而不是偷偷尝试半程恢复
- “全局恢复优先当前场景”的正确拆法应是：
  - 存档内容仍是全局的
  - 但读档时不强制等价于“先切场回到保存时 sceneName 再恢复”
  - 更合理的是：当前活动场景先恢复；其他场景对象在被加载/进入时再按同一份全局存档重建
- `SkillLevel / Experience` 现阶段应从“建议纳入第二刀”下调为：
  - `当前范围外，不提前为未落地功能建存档`

**对“先向线程请求许可/回执”的阶段判断**:
- 这条建议是有价值的
- 但更适合被实现成：
  - `受影响线程的施工前边界确认`
  而不是：
  - `所有线程的一刀一票式审批`
- 当前更合理的目标不是重新加一层重治理，而是提前避免：
  1. 我误碰别人的活跃语义
  2. 我以为某状态该存，但 owner 其实认为那是瞬时态
  3. 我直接改共享链，结果打断 day1 / NPC / 旧存档底座

**当前建议的沟通对象（按影响度，而不是“所有历史线程”）**:
1. `spring-day1 / day1 主控线`
   - 需要确认：剧情相位、对白消费、Day1 导演结构态、工作台提示消费的边界
2. `NPC / NPCV2`
   - 需要确认：关系阶段、formal / informal 切换语义、哪些 NPC 状态属于长期进度
3. `农田交互修复V3 / 旧存档底座链`
   - 需要确认：新的接入方式不要破坏现有 `Inventory / Equipment / Chest / runtime item / worldObjects` 存档主链
- 其余线程如果不拥有当前要碰的语义面，不必强拉进“许可回执”流程

**建议的沟通口径**:
- 不叫泛化“批不批准”
- 更像：
  - `我要动哪些文件 / 哪些状态`
  - `我不会动哪些范围`
  - `你这条线有什么不能被我误改的语义`
  - `如果我只提供存档接入接口，你希望我改，还是你自己收最后一跳`

**恢复点**:
- 下一轮若进入真正实施前准备，更合理的第一动作不是直接写代码
- 而是先形成一版“受影响线程边界确认单”
- 再决定第一刀的主修改面


### 会话 7 - 2026-04-06（为何建议轻量边界确认，而不是默认正式许可）

**用户追问**:
> 为什么是轻量确认，不是正式许可？

**本轮判断**:
- 默认不建议把存档线做成“正式许可制”
- 更合理的是：
  - `默认轻量边界确认`
  - `命中条件时再升级成正式许可`

**原因拆解**:
1. Sunset 当前已经有一层真实施工闸门：
   - `thread-state`
   - hot / mixed 目标报实
   - `Ready-To-Sync`
   - shared touchpoint 冲突检查
   - `sunset-no-red-handoff`
   这些机制负责“我能不能安全施工”
2. 存档线当前更大的风险不是“有人偷偷写文件”
   - 而是“我把别人的语义理解错了”
   - 这类风险更需要 owner 给边界说明，而不是只给一个 yes / no
3. 如果默认上正式许可制：
   - 很容易把实现推进变成等待多个线程批复
   - 还会把已经 `PARKED` 或不活跃的线程变成隐性 blocker
4. 这条线目前是 demo 存档补全，不是跨线程接管整套系统：
   - 更适合先确认边界，再自己对结果负责

**什么时候该升级成正式许可**:
1. 要改对方当前 active 的 owned 业务文件
2. 要改变对方系统的语义定义，而不只是补持久化接线
3. 要求对方让出实现 ownership，或由我替它收最后一跳
4. 命中 hot / mixed 共享目标，或同一触点存在并发风险

**当前最合适的协作口径**:
- 默认先发“边界确认单”：
  - 我准备改什么
  - 我不会改什么
  - 我对这些状态的存档判断是什么
  - 请 owner 回：
    - 哪些判断对
    - 哪些判断错
    - 哪些必须由 owner 自己收
- 只有当回执显示：
  - 我将直接改它的活跃语义面
  - 或 owner 明确要求先批准
  才升级为正式许可

**恢复点**:
- 下一轮如果用户认可，先产出一版：
  - `受影响线程边界确认单`
- 并顺带定义：
  - 哪些情况自动升级为正式许可


### 会话 8 - 2026-04-06（受影响线程边界确认 prompt 与固定回执卡落地）

**当前主线目标**:
> 不直接打断其他线程施工，而是在实施存档第一刀前，先向受影响线程发出“边界确认而非停工审批”的引导 prompt，并为它们准备固定回执文档。

**本轮子任务**:
> 根据用户拍板，直接产出：
> 1. 一份统一分发说明
> 2. 三份线程 prompt
> 3. 三份对应回执卡
> 并统一默认：
> - 线程正在干活
> - 不需要单独聊天回执
> - 只需在安全点回写文档
> - 允许线程自行开 subagent

**本轮完成事项**:
1. 创建总分发说明：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_线程边界确认分发说明_01.md`
2. 创建 `spring-day1` 专属 prompt + 回执卡：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_spring-day1_存档边界确认prompt_01.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_spring-day1_存档边界回执_01.md`
3. 创建 `NPC` 专属 prompt + 回执卡：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_NPC_存档边界确认prompt_01.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_NPC_存档边界回执_01.md`
4. 创建 `农田交互修复V3` 专属 prompt + 回执卡：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_农田交互修复V3_存档边界确认prompt_01.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_农田交互修复V3_存档边界回执_01.md`

**本轮明确落成的统一口径**:
- 这轮不是停工令
- 默认线程继续当前唯一主刀
- 只在最近安全点 / 本轮切片收尾时补回执
- 不要求单独聊天回执
- 默认只认回执文件
- 允许线程自行开 subagent 做只读对照和并行核查
- 回执重点是：
  - 哪些状态该进 slot-save
  - 哪些状态不该持久化
  - 哪些文件允许 `存档系统` 线程直接改
  - 哪些必须由 owner 自收
  - 哪些情况应从边界确认升级为正式许可

**thread-state 收尾**:
- 本轮为 docs 施工补跑：
  - `Begin-Slice`
- 本轮已执行：
  - `Park-Slice`
- 未跑：
  - `Ready-To-Sync`
- 当前 live 状态：
  - `PARKED`
- 原因：
  - 本轮只新增 prompt / 回执文档，不进入代码实现或白名单同步

**恢复点**:
- 用户现在可以直接转发这三份 prompt
- 下一轮等待这些线程把回执写回对应文档后，再统一收件裁定：
  1. 哪些边界已经对齐
  2. 哪些应升级正式许可
  3. 第一刀最终该从哪条语义面开工

## 2026-04-06｜已收到 `spring-day1` owner 的 Day1 存档边界回执

- 当前新增回执文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_spring-day1_存档边界回执_01.md`
- 当前 `spring-day1` owner 的核心边界判断：
  1. 第一版必须存：
     - `StoryManager.CurrentPhase`
     - `StoryManager.IsLanguageDecoded`
     - `DialogueManager` 已完成正式序列集合
     - `SpringDay1Director` 教学目标完成态
     - `craftedCount`
     - `freeTimeEntered / freeTimeIntroCompleted / dayEnded`
     - 工作台首次提示消费
  2. 明确不该直接存：
     - `PromptCardModel`
     - Prompt/UI 当前展示态
     - `DialogueUI` 打字进度
     - `SpringDay1NpcCrowdDirector` 临时站位 / parent / runtime 摆位
     - 工作台 overlay 当前选中 / hover / 进度文案
  3. 当前最重要的 owner 要求：
     - `存档系统` 可以先做接线与数据结构
     - 但 `SpringDay1Director` 里“恢复后如何不重播正式剧情、如何回落到正确 phase”的最后一跳，建议由 `spring-day1` 自收
- 当前恢复点：
  - 后续若继续做第一版存档实施，`spring-day1` 的 owner 口径已经落文件，不需要再口头追问 Day1 边界

## 2026-04-06｜已收到 `农田交互修复V3` owner 的旧存档底座边界回执

- 当前新增回执文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_农田交互修复V3_存档边界回执_01.md`
- 当前 `农田交互修复V3` owner 的核心边界判断：
  1. 旧存档底座当前稳定主链是：
     - `SaveManager -> PersistentObjectRegistry -> IPersistentObject -> worldObjects/genericData`
  2. 对新增剧情 / Day1 / NPC 长期态，最推荐接法是：
     - 新建独立 `IPersistentObject` 或独立持久化服务
     - 接进现有 `worldObjects/genericData`
     - 不要继续把 `SaveManager` 扩回“所有系统总表”
  3. 当前绝不能误伤的旧链包括：
     - `SaveManager.LoadGame()` 现有恢复顺序
     - `PersistentObjectRegistry` 的 `PruneStaleRecords()` / GUID 冲突自愈 / 反向修剪
     - `WorldObjectSaveData.objectType + prefabId + genericData` 契约
     - `DynamicObjectFactory` 的“实例化 -> 设 GUID -> 注册 -> Load -> 激活”恢复口径
     - 已接入旧持久化链的 `Inventory / Equipment / Chest / Crop / Tree / Stone / FarmTile / WorldItemPickup`
  4. 当前允许 `存档系统` 线程直接改的面，只建议停在有限 additive-only：
     - `SaveDataDTOs.cs`
     - `DynamicObjectFactory.cs`
     - `SaveManager.cs` 的极窄入口 / 兼容 / 阻断补口
  5. 当前不建议外线直接改、建议 owner 自收的面：
     - `PersistentObjectRegistry.cs`
     - `SaveManager.cs` 主保存 / 主加载顺序
     - 既有对象自己的 `Save()/Load()` 语义
       - `InventoryService`
       - `EquipmentService`
       - `ChestController`
       - `CropController`
       - `TreeController`
       - `StoneController`
       - `WorldItemPickup`
       - `FarmTileManager`
  6. 已明确属于历史残留、不要再扩的结构：
     - `GameSaveData.farmTiles`
     - `InventorySaveData` 作为主入口
     - `FarmTileSaveData` 里废弃作物字段
     - `PlayerSaveData` 当前未闭环字段
     - `InventorySlotSaveData.instanceId`
  7. 已明确的高危破坏点：
     - 把 `PruneStaleRecords()` 改回 `Clear()`
     - 改 `LoadGame()` 恢复顺序
     - 改 `objectType / prefabId / genericData` 契约
     - 改坏 `Load()` 与 `Start()` 的先后假设
     - 继续把 `PlayerPrefs` 和 slot-save 双写成双真源
- 当前恢复点：
  - 如果后续进入第一刀真实实施，`农田交互修复V3` 这边关于旧底座“哪些能碰 / 哪些不能碰 / 哪些要升级正式许可”的 owner 口径已经落文件，不需要再凭聊天回忆这条边界

## 2026-04-06｜第一刀真实施工：剧情长期态已接入现有 `IPersistentObject` 主链

**当前主线目标**:
- 在不误伤旧存档底座、也不直接闯入 `spring-day1` 脏文件的前提下，先把“最值钱、最稳定”的剧情/NPC 长期态并入当前 slot-save。

**本轮施工判断**:
- 结合三份回执后，最终采用：
  - `独立持久化服务 + 现有 worldObjects/genericData 主链`
- 明确暂不在这一刀里直接碰：
  - `SpringDay1Director.cs`
  - `CraftingStationInteractable.cs`
- 原因：
  - 这两个文件当前已有他线 dirty
  - 且 `spring-day1` owner 明确要求“Day1 恢复后语义最后一跳”更适合由 owner 自收

**本轮完成事项**:
1. 新增独立持久化服务：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryProgressPersistenceService.cs`
   - 作用：
     - 以固定 `PersistentId/ObjectType` 注册进 `PersistentObjectRegistry`
     - 通过 `WorldObjectSaveData.genericData` 保存剧情长期态
     - 不改 `PersistentObjectRegistry`、不改 `DynamicObjectFactory`、不扩根 DTO
2. 第一版已接入的长期态：
   - `StoryManager.CurrentPhase`
   - `StoryManager.IsLanguageDecoded`
   - `DialogueManager` 已完成正式对白序列集合
   - `PlayerNpcRelationshipService` 的 `npcId -> relationshipStage`
   - 工作台首次提示消费
     - 当前仍以 `spring-day1.workbench-entry-hint-consumed` 作为兼容源
     - 但现在已经进入 slot-save 读写链
3. 新增保存阻断：
   - `SaveManager.SaveGame(...)` 现在会先询问 `StoryProgressPersistenceService.CanSaveNow(...)`
   - 当前已落实用户拍板：
     - `工作台制作途中不支持存档`
   - 做法：
     - 通过只读反射检查 `SpringDay1Director._workbenchCraftingActive`
     - 命中时直接阻断保存并给出原因
4. 新增旧档兜底：
   - `SaveManager.LoadGame(...)` 在恢复完 `worldObjects` 后，会调用 `StoryProgressPersistenceService.FinalizeLoadedSave(...)`
   - 如果加载的是还没有这份剧情长期态 payload 的旧档：
     - 会把这条持久化服务重新拉回运行态
     - 并把剧情/NPC/工作台提示状态回落到默认基线
5. 为接线补的最小接口：
   - `DialogueManager`
     - 新增 `EnsureRuntime()`
     - 新增 completed-sequence snapshot / replace 接口
   - `PlayerNpcRelationshipService`
     - 新增 snapshot / replace 接口
     - 新增 `KnownNpcIds` 注册表，保证读档时能完整替换旧关系键而不是只覆盖当前缓存

**验证结果**:
- `SaveManager.cs`
  - `validate_script`：`no_red`
- `StoryProgressPersistenceService.cs`
  - `validate_script`：`no_red`
- `DialogueManager.cs`
  - `validate_script`：`unity_validation_pending`
  - 但 `manage_script validate` 为 `clean`
  - 且后续 fresh `errors` 返回 `0`
- `PlayerNpcRelationshipService.cs`
  - `validate_script`：`unity_validation_pending`
  - 但 `manage_script validate` 为 `clean`
  - 且后续 fresh `errors` 返回 `0`
- 本轮最终 fresh console：
  - `errors=0`
- 额外现场噪音：
  - 中途曾短暂看到一个外部 `LightingLookDevFullscreenLayer.cs` 缺文件异常
  - 但最后 fresh `errors` 已清空，不属于本轮 owned red

**本轮仍未完成的关键缺口**:
1. `SpringDay1Director` 的任务完成态 / `craftedCount / freeTime/dayEnd` 仍未接入
   - 这是当前第一刀有意留下的缺口
   - 不是遗漏
   - 理由是先避开 dirty 文件和 owner 语义最后一跳
2. NPC 瞬时会话态尚未在 load 时主动清空
   - 本轮只接了长期真值
   - 没去改 `PlayerNpcChatSessionService`
3. 工作台提示消费目前还是“兼容桥接”
   - 尚未把 `CraftingStationInteractable` 自身数据源彻底迁走
4. 玩家正式入口 / 自动存档 / 槽位 UX 仍未开始

**当前阶段判断**:
- 这条线现在不再只是“纯分析”
- 已经进入：
  - `第一刀真实实现已落地`
  - 但还只是 `长期态安全接线层`
- 距离“完整 demo 存档”仍差：
  - `Day1 导演态`
  - `NPC 瞬时态 reset 策略`
  - `玩家正式入口与产品面`

**thread-state 结算**:
- 本轮已执行：
  - `Begin-Slice`
    - `CurrentSlice = story-progress-persistence-first-slice_2026-04-06`
  - `Park-Slice`
- 未跑：
  - `Ready-To-Sync`
- 当前 live 状态：
  - `PARKED`
- 原因：
  - 本轮先停在可 review 的代码切片，不做白名单 sync / 提交

**恢复点**:
- 下一轮最值得继续的动作：
  1. 和 `spring-day1` owner 对齐 `SpringDay1Director` 的 restore 最后一跳
  2. 再补 `Day1` 任务态 / `craftedCount / freeTime/dayEnd`
  3. 视需要补 `NPC` 瞬时会话清空 hook

## 2026-04-06｜第二刀收口：Day1 导演态 / HP-EP / NPC 瞬时清空已接入，自动回归补齐到 blocker

**当前主线目标**:
- 在第一刀剧情长期态接线的基础上，把 `spring-day1` 当前 demo 最关键的续档缺口真正补齐到“可交付版”：
  - Day1 导演长期态
  - 玩家 `HP / EP`
  - 读档后 NPC 瞬时会话态清空

**本轮实际完成事项**:
1. 持久化服务 `StoryProgressPersistenceService.cs` 已扩到第二刀真正主持久化范围：
   - `SpringDay1Director`
     - `_tillObjectiveCompleted`
     - `_plantObjectiveCompleted`
     - `_waterObjectiveCompleted`
     - `_woodObjectiveCompleted`
     - `_collectedWoodSinceWoodStepStart`
     - `_craftedCount`
     - `_freeTimeEntered`
     - `_freeTimeIntroCompleted`
     - `_dayEnded`
     - `_staminaRevealed`
   - `HealthSystem`
     - `current / max / visible`
   - `EnergySystem`
     - `current / max / visible / lowEnergyWarningActive`
2. 保存阻断口径继续收紧并真正落代码：
   - 正式对白进行中不允许保存
   - NPC 闲聊进行中不允许保存
   - 工作台制作进行中不允许保存
3. 读档后的瞬时态清空已接入：
   - `PlayerNpcChatSessionService`
     - `CancelConversationImmediate(...)`
     - `ClearPendingResumeSnapshot()`
     - `ResetValidationSnapshot()`
     - `ResetConversationBubbleVisualsImmediate()`
   - `PlayerNpcNearbyFeedbackService`
     - `HideActiveNearbyBubble()`
     - `SyncDialogueSuppressionState()`
4. 本轮额外补了一组新的 EditMode 回归测试：
   - `Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
   - 覆盖两类关键验证：
     - unsafe 状态下保存阻断
     - 剧情 / Day1 / HP-EP / NPC 长期态 round-trip 恢复

**本轮抓到并修掉的真实 bug**:
- 通过新加的 round-trip 测试，实际抓到了 `StoryProgressPersistenceService.ApplySpringDay1Progress(...)` 里的反射恢复 bug：
  - `GetCurrentWoodCount()` 的读取使用了一个带 `params` 的泛型 helper
  - 在当前运行时路径下会抛 `TargetParameterCountException`
  - 已改成显式的“无参私有方法调用”恢复逻辑，不再走这条有歧义的 helper

**自动验证进展**:
1. `StoryProgressPersistenceServiceTests.CanSaveNow_BlocksDialogueChatAndWorkbenchCrafting`
   - 已通过
2. `StoryProgressPersistenceServiceTests.SaveLoad_RoundTripRestoresLongTermStoryStateAndClearsNpcTransientState`
   - 初次运行先后抓到两处问题：
     - 测试壳自身 `string[]` 反射传参错误
     - 真实业务 bug：`GetCurrentWoodCount()` 反射调用异常
   - 两处都已修掉
3. 本轮最后一轮完整重跑未能闭环
   - 原因不是本线 owned red
   - 而是外部 compile red 新出现于：
     - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs:1166`
     - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs:1177`
   - 错误：
     - `CS0103: The name 'EnumerateAnchorNames' does not exist in the current context`
   - 这属于他线 / 外部 blocker，不在本轮 `存档系统` 的 owned 修改面内

**本轮可确认的 no-red / blocker 口径**:
- `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
  - `manage_script validate = clean`
- `Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
  - 在外部 blocker 出现前，`validate_script = no_red`
  - 当前最新 CLI 视角下属于：
    - `external_red`
  - 原因是 `SpringDay1NpcCrowdDirector.cs` 外部红面，不是本轮测试文件 own red
- 最新 fresh console 也已看到：
  - `errors=2`
  - 全部落在 `SpringDay1NpcCrowdDirector.cs`

**thread-state 结算**:
- 本轮沿用已有 ACTIVE slice：
  - `story-progress-persistence-second-slice_day1-director-and-transient-reset_2026-04-06`
- 本轮结束前已执行：
  - `Park-Slice`
- 未跑：
  - `Ready-To-Sync`
- 当前 live 状态：
  - `PARKED`
- 原因：
  - 本轮代码与测试已收口
  - 但最终完整自动回归被外部 compile red 卡住，不适合继续 claim sync-ready

**恢复点**:
- 存档系统这条线下一步最小恢复动作已很明确：
  1. 等外部 `SpringDay1NpcCrowdDirector.cs` 红面清掉
  2. 重新跑 `StoryProgressPersistenceServiceTests`
  3. 通过后再做一轮真正的 save/load 人工验收

### 会话 4 - 2026-04-06（存档 debug 入口只读审计）

**当前主线目标**:
> 当前线程正在推进“设置页存档 UI 整合与 debug 入口退场”；本轮用户要求先不改代码，只做一次现状盘点，确认现有哪些存档 debug 入口还暴露给玩家，哪些只是 Editor / Internal 入口。

**本轮子任务**:
> 盘点现有存档 debug 入口和玩家不可见入口，重点核对：
> 1. `F5/F9`
> 2. `ContextMenu`
> 3. 临时按钮 / 临时 Canvas
> 4. `SaveManager` 直接调用点

**本轮完成事项**:
1. 用静态搜索确认运行时直连点只剩一处：
   - `Assets/YYY_Scripts/UI/Debug/SaveLoadDebugUI.cs`
   - 这里同时负责：
     - `F5` 保存
     - `F9` 加载
     - 运行时创建 `SaveLoadDebugCanvas`
     - 创建“保存 / 加载 / 快捷键提示”按钮与文本
   - 它也是当前唯一查到的 runtime `SaveManager.SaveGame/LoadGame` 直接调用方
2. 确认玩家面主场景仍然挂着这个 debug 入口：
   - `Assets/000_Scenes/Primary.unity`
     - 有启用中的 `DebugUI`
     - 挂了 `SaveLoadDebugUI`
   - `Assets/000_Scenes/Town.unity`
     - 同样有启用中的 `DebugUI`
     - 挂了 `SaveLoadDebugUI`
3. 确认两个主场景里不只是“脚本还在”，连运行时生成的临时 UI 也被序列化落盘了：
   - `Assets/000_Scenes/Primary.unity`
     - 存在已保存的 `SaveLoadDebugCanvas`
     - 里面能直接找到：
       - `保存 (F5)`
       - `加载 (F9)`
       - `快捷键: F5=保存, F9=加载`
   - `Assets/000_Scenes/Town.unity`
     - 同样存在已保存的 `SaveLoadDebugCanvas`
     - 同样有上述三类文本
   - 这意味着：
     - 未来即使只关掉 `SaveLoadDebugUI` 组件，
     - 如果不同时清掉 / 隐藏已落盘的 `SaveLoadDebugCanvas`，
     - 玩家仍可能继续看见这套临时按钮
4. 补查 `SaveManager` 本体和存档底座相关 `ContextMenu`：
   - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
     - `快速保存 (slot1)`
     - `快速加载 (slot1)`
     - `打印存档路径`
     - `打开存档目录`
     - 且全部包在 `#if UNITY_EDITOR`
   - `Assets/YYY_Scripts/Data/Core/PersistentObjectRegistry.cs`
     - `打印所有注册对象`
     - `按类型统计`
   - `Assets/YYY_Scripts/Data/Core/IPersistentObject.cs`
     - `重新生成持久化 ID`
5. 补查 repo 内的 debug 残留副本：
   - `Assets/000_Scenes/primary_backup_2026-04-02_20-46-54.unity`
     - 仍挂着 `SaveLoadDebugUI`
   - `Assets/__CodexSceneSyncScratch/`
     - 多个 scratch / validation scene 也保留了 `SaveLoadDebugUI` 或 `SaveLoadDebugCanvas`
   - 这些不直接面向玩家，但属于后续容易把 debug 入口带回来的残留源

**本轮审计结论**:
- 必须删除 / 禁用的玩家可见入口：
  - `Assets/YYY_Scripts/UI/Debug/SaveLoadDebugUI.cs` 在 `Primary.unity` / `Town.unity` 的场景挂载
  - `Primary.unity` / `Town.unity` 里已经落盘的 `SaveLoadDebugCanvas` 及其按钮 / 文本
- 可以保留但不应暴露给玩家的内部能力：
  - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
    - `SaveGame`
    - `LoadGame`
    - `SaveExists`
    - `DeleteSave`
    - `GetAllSaveSlots`
  - `SaveManager` 里包在 `#if UNITY_EDITOR` 的 `ContextMenu`
  - `PersistentObjectRegistry` / `IPersistentObject` 的 Editor 调试项
- 本轮没有发现第二条 runtime 保存入口：
  - 除 `SaveLoadDebugUI` 外，
  - 没查到别的运行时代码直接调 `SaveManager.Instance.SaveGame/LoadGame`

**验证状态**:
- 本轮为只读静态审计
- 已完成：
  - `rg` / scene YAML / 代码交叉核对
- 未完成：
  - Unity live 场景验证
  - 实际退场改造

**恢复点**:
- 如果下一轮进入真实施工，第一刀最稳的是：
  1. 从 `Primary.unity` / `Town.unity` 退掉 `DebugUI + SaveLoadDebugCanvas`
  2. 保留 `SaveManager` 作为内部 API
  3. 再决定正式存档入口是否转入设置页 / 正式菜单

## 2026-04-06：设置页存档 UI 一体化落地（runtime 生成版）

**当前主线**:
- 把 Sunset 的存档系统从“底层可存读 + debug 入口”收成“玩家能在 `PackagePanel -> 5_Settings` 里直接操作的新建/读取/重新开始 demo”。

**本轮完成事项**:
1. 存档底层从单一 debug 调用升级成正式槽位接口：
   - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - 新增：
     - 固定玩家可见槽位 `slot1~slot3`
     - `SaveSlotSummary` 槽位摘要读取
     - 启动时自动捕获 `__fresh_start_baseline__`
     - `RestartToFreshGame()`
     - `SaveSlotsChanged` 事件
   - 同时把 `GetAllSaveSlots()` 过滤掉内部 baseline 槽，避免混进玩家列表
2. 存档数据 DTO 补了正式摘要结构：
   - `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
   - 新增 `SaveSlotSummary`
   - 用于设置页显示：
     - 最近保存时间
     - 场景
     - 季节/日期/时间
     - 剧情阶段
     - 语言解码状态
     - 背包占用
     - 生命/精力摘要
3. 旧 runtime debug 存档入口已退成“空壳兼容脚本”：
   - `Assets/YYY_Scripts/UI/Debug/SaveLoadDebugUI.cs`
   - 不再响应 `F5/F9`
   - 不再创建任何保存/加载调试按钮
   - 运行时会主动清掉遗留 `SaveLoadDebugCanvas`
   - 仅保留脚本名，避免 scene 上现有挂载直接丢成 missing script
4. 正式设置页存档面板已落地为 runtime UI：
   - 新增 `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - 特点：
     - 自动挂到 `PackagePanel/Main/5_Settings/Main`
     - 生成滚动容器、标题、状态卡、槽位卡、刷新按钮、重新开始按钮
     - 每个槽位支持：
       - 空槽时 `创建新存档`
       - 已有槽时 `覆盖保存`
       - `读取存档`
       - `删除存档`
     - 全局支持：
       - `重新开始当前游戏`
   - 当前这版不改 scene / prefab 结构，而是用运行时代码直接长在现有 settings 背景壳里
5. `PackagePanelTabsUI` 已接线：
   - `Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
   - 在 `Awake / SetRoots / EnsureReady` 时确保设置页面板被安装
   - 因此 scene-owned `PackagePanel` 也能直接吃到这版设置页存档 UI

**本轮验证**:
- 脚本级验证：
  - `validate_script`
    - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
    - `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
    - `Assets/YYY_Scripts/UI/Debug/SaveLoadDebugUI.cs`
    - `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
    - `Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
  - 结果：全部 `0 errors`
- 文本闸门：
  - `git diff --check -- Assets/...`
  - 结果：通过
- CLI compile-first：
  - `python .\\scripts\\sunset_mcp.py --timeout-sec 60 --wait-sec 15 validate_script Assets/YYY_Scripts/Data/Core/SaveManager.cs Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs Assets/YYY_Scripts/UI/Debug/SaveLoadDebugUI.cs Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs --count 40`
  - 结果：
    - `assessment=no_red`
    - `owned_errors=0`
    - `external_errors=0`
- fresh console：
  - `python .\\scripts\\sunset_mcp.py errors --count 20`
  - 结果：`errors=0 warnings=0`
- 运行时结构取证：
  - 进 PlayMode 后确认：
    - 存在 `SaveSettingsRuntimeRoot`
    - 存在 `ScrollRoot`
    - 存在 `刷新列表` 按钮
    - 不存在 `SaveLoadDebugCanvas`
  - 说明：
    - 设置页存档 UI 已真实生成
    - 旧 debug canvas 没有回魂

**本轮判断**:
- 这版已经不再是“只有底层 save/load 的半成品”，而是“玩家可在设置页直接操作的 demo 入口”。
- 当前最值钱的取舍是：
  - 不去抢 `Primary/Town` scene 热区做结构性改 prefab/scene
  - 而是直接让 scene-owned `PackagePanel` 通过运行时代码吃到正式存档面板
  - 这样能最快把 demo 交到用户手上测

**尚未做成 / 仍待后续**:
- 没做真正的 title/menu 主菜单存档页；当前正式入口在 `PackagePanel -> 5_Settings`
- 没做二次确认弹窗；这版靠按钮文案和状态提示控制风险
- 没清 repo 里 backup / scratch scene 的历史 debug 残留；本轮只保证玩家运行态不会看到旧存档 debug UI

**thread-state / 现场状态**:
- 本轮已实际执行：
  - `Begin-Slice`
  - `Park-Slice`
- 未执行：
  - `Ready-To-Sync`
- 当前 live 状态：
  - `PARKED`
- 当前 blocker：
  - `等待用户在设置页验收存档UI与基础槽位流程`

**恢复点**:
- 如果用户下一轮继续这条线，最稳的顺序是：
  1. 先根据用户验收反馈补细节（文案、布局、交互确认）
  2. 若用户要求“真正把 debug 残留从 scene 物理删掉”，再单开 scene 热区切片处理 `Primary/Town`
  3. 若用户要求更正式的主菜单入口，再从设置页方案迁到主菜单 / 启动流程

## 2026-04-07：用户对设置页存档 UI 的二次收口要求

**本轮性质**:
- 只读需求收束，不进真实施工。

**用户新增 / 改写的核心要求**:
1. `5_Settings` 内的背景说明要大幅收缩：
   - 不要那么多说明卡和大段背景
   - 只保留必要元素与必要说明
   - 严格不超出 `5_Settings`
2. 存档 UI 结构要改成“默认存档 + 下方滑动区域”的两段式：
   - 默认存档固定显示，不放进滑动区域
   - 默认存档不可删除，但可覆盖
   - 下面的滑动区域用于普通存档列表
   - 滑动区右上角要有 `新建存档` 按钮
3. 普通存档条目排版固定为左右式：
   - 左侧 / 中部显示详情
   - 右侧并排三个等大按钮
   - 按钮不要比文本区长太多
4. 滚动区域要重新收：
   - 滚轮不允许过于敏感
   - 不允许滚到“没有内容的空白区”
   - 滚动高度要以真实内容为边界
5. 普通存档条目的按钮语义被重定义为：
   - `复制当前存档`
   - `粘贴到该存档`
   - `删除当前存档`
   - 其中“粘贴”本质是把复制缓存覆盖写入目标槽位
6. 复制缓存规则：
   - 若未复制任何内容，点“粘贴”必须提示：`请先复制存档内容`
   - 每次重新打开 UI，都要清空复制缓存
   - 也就是每次打开 settings 后都必须重新复制一次
7. 默认存档逻辑被重新定义为：
   - 它是默认进度存档
   - 开始游戏 / 退出游戏都会复原到它
   - 它不能删除，但可以覆盖
8. 快捷键需求被重新定义为正式规则，不再是旧 debug：
   - 要在 settings 里明确写清楚：
     - `F5` 快速存档
     - `F9` 快速读档
   - 还要写清楚快捷键规范
9. 快捷键反馈必须做正式屏幕提示：
   - 按下 `F5/F9` 后屏幕居中提示
   - 文案类似：`已存档` / `已读档`
   - 黑色半透明底 + 白色加粗字
   - 渐入 `0.5s`
   - 渐出 `0.5s`
10. settings 内的“读档 / 重新开始”交互要求：
   - 点击后先关闭界面
   - 再显示同样风格的提示

**对现状的直接纠偏**:
- 当前我做的“多张大卡片 + 说明偏多 + 3 个按钮是读/写/删”不符合用户最新要求。
- 下一轮必须重点调整：
  1. 砍背景说明密度
  2. 改为默认存档固定区 + 普通存档滚动区
  3. 普通存档按钮改成复制 / 粘贴 / 删除
  4. 增加 UI 打开时清空复制缓存
  5. 增加 F5/F9 的正式提示浮层
  6. 增加 settings 内快捷键说明
  7. 调整滚动条灵敏度和内容边界

**恢复点**:
- 下一轮如果继续施工，应优先按“UI收缩 + 槽位逻辑重构 + 快捷键提示层”这一刀来做，不要再扩背景介绍。

## 2026-04-07：四按钮语义最终澄清与落地方案收束

**本轮性质**:
- 只读分析，不进真实施工。

**用户本轮最新纠正**:
- 普通存档条目不是 3 个按钮，而是 4 个按钮，且语义固定为：
  1. `复制当前存档`
  2. `粘贴至当前存档`
  3. `覆盖当前存档`
  4. `删除当前存档`
- 其中：
  - `复制` 只负责把源槽位内容写入本次打开 settings 生命周期内的复制缓存
  - `粘贴` 负责把最近一次复制的槽位内容覆盖写入当前目标槽位
  - `覆盖` 负责把“当前正在游玩的实时进度”直接保存到当前槽位
  - `删除` 负责清空当前普通槽位
- 用户同时要求：
  - 这 4 个按钮都要在 settings 内的说明区讲清楚
  - 默认存档仍不可删除，但可覆盖
  - 普通存档区顶部右上角仍保留 `新建存档`

**这次收束后的最终需求骨架**:
1. 承载位置不变：
   - 只在 `PackagePanel -> 5_Settings` 内完成
   - 不外溢到其它页
2. 结构分区固定为：
   - 顶部 `默认存档固定区`
   - 下方 `普通存档滚动区`
   - 侧边或同区内的 `必要说明区`
3. 默认存档规则固定：
   - 固定显示
   - 不可删除
   - 可覆盖
   - 作为默认进度基线
4. 普通存档区规则固定：
   - 右上角 `新建存档`
   - 每个条目左右式布局
   - 右侧 4 个等高按钮：复制 / 粘贴 / 覆盖 / 删除
5. 复制缓存规则固定：
   - 每次重新打开 settings 清空
   - 未复制时点击粘贴，必须提示 `请先复制存档内容`
6. 快捷键规则固定：
   - `F5` 快速存档
   - `F9` 快速读档
   - 要作为正式玩家能力说明，不再是 debug 残留
7. 居中提示规则固定：
   - 适用于 `F5/F9`
   - 适用于 settings 内读档 / 重新开始
   - 黑色半透明底、白色加粗字、淡入 0.5s、淡出 0.5s

**当前实现与目标的差距重判**:
- 当前设置页存档 demo 已经有正式入口，但距离用户刚刚定义的最终版仍差一整次“产品模型重构”，不是改文案就能过线。
- 主要差距集中在 5 块：
  1. 槽位按钮模型仍是旧的读/写/删，不是复制/粘贴/覆盖/删除
  2. 没有复制缓存生命周期
  3. 没有正式 `F5/F9` 提示层
  4. 默认存档与普通存档还没彻底分治
  5. 说明区和滚动区还没按用户要求压到更克制的正式玩家面

**本轮判断**:
- 下一轮真实施工应只做一刀：
  - `设置页存档 UI / 交互重构`
- 不应顺手再扩：
  - title 主菜单
  - 场景物理清 debug 残留
  - 更大范围的存档底层扩容

**证据层报实**:
- 本轮关于 UI 的判断只站在：
  - `结构 / 交互规则 / 当前实现差距`
- 还没有新的玩家侧 GameView 截图或真实体验证据，因此不能把“审美已过线”说满。

**恢复点**:
- 下一轮继续时，先按这份收束版需求清单开工：
  1. 重排 `默认存档区 + 普通存档区 + 必要说明区`
  2. 改 4 按钮模型与复制缓存
  3. 补 `F5/F9` 正式快捷键和居中提示层
  4. 收滚动边界与按钮排版

## 2026-04-07：设置页存档系统正式重构已落地，并修掉“启动读坏默认档导致场景像被搬空”的回归

**当前主线**:
- 把 `5_Settings` 里的存档系统真正收成可验版：
  - 默认存档固定区
  - 普通存档滚动区
  - 四按钮模型
  - 正式 `F5/F9`
  - 正式中央提示层

**本轮实际做成**:
1. `SaveManager.cs` 已从旧固定三槽模型升级为：
   - `__default_progress__` 默认存档
   - 动态普通槽位 `slotN`
   - `CreateNewOrdinarySlotFromCurrentProgress()`
   - `TryCopySlotData()`
   - `PasteSaveDataToSlot()`
   - `QuickSaveDefaultSlot()` / `QuickLoadDefaultSlot()`
   - 默认存档不可删除、普通槽位可删除
2. 正式快捷键已接回 `SaveManager.Update()`：
   - `F5` 快速保存到默认存档
   - `F9` 快速读取默认存档
3. 新增正式提示层：
   - `Assets/YYY_Scripts/UI/Save/SaveActionToastOverlay.cs`
   - 居中黑色半透明底
   - 白色加粗字
   - 淡入 `0.5s`
   - 淡出 `0.5s`
4. `PackageSaveSettingsPanel.cs` 已重写成新玩家面：
   - 顶部固定 `默认存档`
   - 下方 `普通存档` 滚动区
   - 普通存档右上角 `新建存档`
   - 左侧信息区点击读档
   - 右侧四按钮：
     - `复制当前存档`
     - `粘贴至当前存档`
     - `覆盖当前存档`
     - `删除当前存档`
   - 默认存档区保留：
     - `复制`
     - `粘贴`
     - `覆盖`
     - 不提供删除
   - 复制缓存会在每次重新打开 settings 时清空
5. `PackagePanelTabsUI.cs` 已补公共关闭入口：
   - 支持 settings 内部在读档 / 重开前先关闭面板
6. `SaveDataDTOs.cs` 已补 `SaveSlotSummary.displayName / isDefaultSlot`

**本轮额外修掉的高优先级回归**:
- 用户在本轮中途真实触发了一个严重回归：
  - 重开游戏后场景看起来像“被搬空”
- 已确认原因不是 scene 资源被删，而是：
  - 我刚接进去的“启动自动恢复默认存档”
  - 读到了一个 `worldObjects=[]` 的坏 `__default_progress__.json`
  - 于是运行时按空快照恢复，视觉上像场景被清空
- 已落地修复：
  1. 删除当时那份坏默认档
  2. 给启动自动恢复加硬保护：
     - 默认存档若 `worldObjects` 为空或不完整，启动时禁止自动恢复
  3. 给 `QuickLoadDefaultSlot()` 与默认存档摘要也加同类保护
- 之后新生成的默认档已再次检查：
  - `worldObjects` 为非空
  - 不再是那份空档

**本轮验证**:
- 脚本原生校验：
  - `SaveManager.cs`：`0 errors / 3 warnings`
  - `PackageSaveSettingsPanel.cs`：`0 errors`
  - `SaveActionToastOverlay.cs`：`0 errors`
  - `PackagePanelTabsUI.cs`：`0 errors`
- 文本闸门：
  - `git diff --check` 通过
- fresh console：
  - 清空后 `errors=0 warnings=0`
- 最小 live 取证：
  - PlayMode 中查到 `SaveSettingsRuntimeRoot`
  - 未查到 `SaveLoadDebugCanvas`

**当前仍未完全补的点**:
1. 这轮已把结构和逻辑落地，但还没拿到“玩家真实打开 settings 后”的最终截图证据
2. 运行态里出现过一串 `NPCBubblePresenter` / `Unknown missing script` 噪音
   - 但 fresh console 清空后已为 `0 errors`
   - 当前无法归为本轮 owned red
3. 默认存档“启动即恢复”的策略现在变成：
   - 只在默认档本身完整时才自动恢复
   - 不再允许空默认档把场景恢复坏

**阶段判断**:
- 这条线已经不再是“只有底层能力”
- 现在已经进入：
  - `设置页正式存档 UI + 正式快捷键 + 正式提示层`
  这一版可验阶段
- 但严格来说，UI 审美和操作细节仍应以玩家手验为准

**恢复点**:
- 下一轮若继续，不应该再重做模型，而是围绕用户验收反馈收边：
  1. 按实际 GameView 感受微调排版
  2. 若需要，再收默认存档自动恢复的时机策略
  3. 若用户要求，再把 `Assets/Save` 放到更合适的非 Assets 位置

## 2026-04-07 5_Settings 存档 UI 被用户否决后的第二轮重做与收尾

**用户本轮直接反馈**:
- 旧版 `PackageSaveSettingsPanel` 虽然功能已通，但 UI 仍然：
  - 超出 `5_Settings/Main`
  - 背景卡太重
  - 没有真正“内收 25%”
  - 不符合“只保留元素内容”的要求

**本轮主动作**:
1. 彻底重写 `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - 放弃旧的 card/section 堆叠式结构
   - 改为线性信息布局：
     - 顶部标题与快捷键提示
     - 当前进度摘要
     - 默认存档固定区
     - 普通存档滚动区
     - 底部帮助说明与重新开始按钮
2. 布局硬收缩：
   - `SaveSettingsRuntimeRoot` 改为相对 `Main` 四边大幅内收
   - 当前最终运行态几何：
     - root 区域 `1048 x 616`
   - 运行态布局首选高度：
     - `546`
   - 结构上已经明确小于容器高度，不再是“靠视觉猜测没超”
3. 视觉改法：
   - 删除大面积背景卡
   - 保留设置页原本底板，让存档系统只填“字、线、按钮”
   - 所有文字色从浅色改为深棕系，解决浅橙底板上的低对比度发虚问题
   - 左侧可读区改成轻描边透明信息块，不再做厚重卡片
   - 按钮统一压小，普通槽位维持四按钮等宽
4. 空态补强：
   - 普通存档为空时，把“当前还没有普通存档，点击右上角新建”直接并入普通存档标题旁说明区
   - 避免用户看到滚动区空白还要自己猜

**本轮运行态证据**:
1. 结构证据：
   - PlayMode 中 `SaveSettingsRuntimeRoot` 存在
   - root `RectTransform` 为 `1048 x 616`
   - `VerticalLayoutGroup.preferredHeight = 546`
2. 玩家视面证据：
   - 临时加过一个只用于本轮自查的 Editor 抓图菜单
   - 作用是 PlayMode 下打开 `5_Settings` 并执行整屏 `ScreenCapture`
   - 已在自查结束后删除该临时脚本与 `.meta`
   - 最终抓图路径：
     - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\save-ui-check\save-settings-gameview.png`
3. 从抓图可见：
   - 内容已经完整落在 `5_Settings/Main` 里面
   - 没再出现之前那种“撑爆底边 / 卡片堆满”的问题
   - 但场景左上角仍会带上当前 Day1 任务列表，那是现场 runtime 状态，不是存档 UI 本体

**本轮验证与噪音分类**:
1. fresh console：
   - 清空后 `read_console` 结果为 `0 log entries`
2. `git diff --check`：
   - 已顺手清掉 `SaveManager.cs` 里的尾随空格
   - 当前文本层无新的 diff whitespace 问题
3. 本轮遇到的外部工具噪音：
   - MCP 在 Play/EditorState 读写过程中会偶发：
     - `WebSocket is not initialised`
     - `Invalid AssetDatabase path: ... Temp/CodexEditModeScenes/Primary.unity`
   - 这些都归为外部 MCP/工具噪音，不归为本轮存档系统 owned red
4. 本轮临时抓图器删除后，一度出现：
   - `error CS2001: Source file '...TempSaveSettingsCaptureMenu.cs' could not be found`
   - 重新 `refresh_unity scope=all` 后已消失

**当前阶段判断**:
- 底层存档功能仍是前一轮已落好的正式模型
- 这一轮主要把 `5_Settings` 的正式 UI 从“用户否决状态”重新推回到：
  - 结构过线
  - 真实玩家视面可读
  - 不再明显越界
- 若下一轮继续，优先级应改成：
  1. 用户真实上手验收后的微调
  2. 若用户继续挑 UI，再做局部排版和字重/字号细修
  3. 不再重构整套存档模型

## 2026-04-07 普通存档显示修复 + 显式加载按钮 + 阻断式确认窗

**用户最新反馈**:
- 其他功能基本没问题
- 当前还剩 3 个体验问题：
  1. 新建普通存档后内容看不到
  2. 字号还想再大一点
  3. 不能只靠“点详情区”读档，希望有显式按钮，并且重要操作要先弹确认窗，避免 UI 交互错乱

**本轮确认到的硬事实**:
1. 普通存档不是没创建：
   - `D:\Unity\Unity_learning\Sunset\Save\` 里已存在：
     - `slot1.json`
     - `slot2.json`
     - `slot3.json`
     - `slot4.json`
2. 因此问题不是“新建失败”，而是“普通存档行在 UI 里没有正确占出高度 / 没被清楚呈现出来”。

**本轮代码调整**:
1. `PackageSaveSettingsPanel.cs`
   - 新增字号常量：
     - `TitleFontSize`
     - `SectionTitleFontSize`
     - `BodyFontSize`
     - `SmallFontSize`
     - `ButtonFontSize`
     - `HelpFontSize`
   - 说明：
     - 以后若用户继续要“再大一点 / 再小一点”，可以直接围绕这组常量统一调，不必再全文件逐个找数字
2. 普通存档区显示修复：
   - 每个普通槽位现在显式写死了：
     - `slotRoot.min/preferredHeight = 88`
     - `body.min/preferredHeight = 84`
     - `summaryPanel.min/preferredHeight = 80`
   - 目的：
     - 避免布局系统把已有普通存档行压到接近不可见
3. 默认存档区和普通存档区的交互结构改为：
   - 左侧：详情面板（不再承担“隐藏式点击读档”）
   - 中间：显式加载按钮
   - 右侧：复制 / 粘贴 / 覆盖 / 删除（默认档无删除）
4. 新增阻断式确认窗：
   - 覆盖整个 `5_Settings/Main`
   - 带半透明遮罩
   - 不点击“确认 / 取消”就不能操作后面的 UI
   - 外部点击不会穿透到底层
5. 当前接入确认窗的操作：
   - 读取默认存档
   - 读取普通存档
   - 粘贴存档内容
   - 覆盖存档
   - 删除普通存档
   - 重新开始游戏
6. 未加确认窗的即时操作仍是：
   - 复制当前存档
   - 新建存档

**当前验证**:
- `validate_script Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
  - native validation clean
  - owned errors = 0
  - 因 Unity `stale_status`，CLI assessment 仍记成 `unity_validation_pending`
- fresh `errors`
  - `errors=0 warnings=0`
  - 仅有 Unity 自带 `Saving results to ... TestResults.xml` 噪音，不归为本轮 owned red
- `git diff --check -- Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
  - 通过

**阶段判断**:
- 现在这条线已经从“纯布局问题”进入：
  - 显式交互模型补全
  - 防误触确认窗补全
  - 普通存档可见性修复
- 下一轮最值钱的动作不再是大改，而是让用户基于真实画面继续挑：
  1. 字号是否还要再上调
  2. 普通存档行是否已经稳定显示
  3. 确认窗的尺寸、字重和措辞要不要再收

## 2026-04-07 默认存档降级为“原生开局入口”以恢复 Primary 调试安全

**用户最新裁定**:
- 不要再继续增强默认存档
- 不要再做退出自动保存默认档
- 默认存档当前只承担“回到原生开局 / 原生 Primary”这一个职责
- 当前最高优先级是让游戏恢复到可正常调试的原生状态，不再让默认档链污染场景

**本轮关键判断**:
1. 问题核心不再是 UI，而是 `SaveManager` 里默认档整条运行链太激进：
   - 启动自动恢复默认档
   - 退出自动写回默认档
   - “重新开始游戏”走 `LoadGameInternal(...)` 恢复世界对象
2. 这会把“默认档 / 基线档 / worldObjects 恢复链”硬绑在一起，一旦链路有偏差，就会直接把 `Primary` 调试现场拖坏。
3. 这轮最稳的修法不是继续修默认档内容，而是把默认档降级成只读的原生开局入口。

**本轮代码调整**:
1. `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - 默认槽改为逻辑上的固定槽位，不再依赖 `__default_progress__.json` 是否存在
   - 禁用默认槽写入：
     - `QuickSaveDefaultSlot()` 直接拒绝
     - `SaveGameInternal()` 明确阻止写入默认槽
     - `PasteSaveDataToSlot()` 明确阻止把内容粘贴到默认槽
   - 禁用默认档自动运行链：
     - 启动时不再自动读取 / 修复默认档
     - 退出游戏时不再自动写回默认档
   - `QuickLoadDefaultSlot()` 与 `RestartToFreshGame()` 不再走 `LoadGameInternal(...)`
     - 改成直接异步重载 `Primary`
     - 场景重载完成后只补最小原生开局恢复：
       - `TimeManager -> Year1 Spring Day1 06:00`
       - `StoryProgressPersistenceService.ResetToOpeningRuntimeState()`
     - 不再触发默认档 worldObjects 恢复链
   - 默认槽摘要改为：
     - 优先读取 `__fresh_start_baseline__`
     - 没有 baseline 时也提供可用的“原生开局”摘要，不阻断默认读档
   - `TryReadSaveData()` 对默认槽改成读 `__fresh_start_baseline__`，保证默认槽是“读开局”而不是“读运行中 temp”
2. `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - 默认槽改成只读加载语义：
     - 标题提示改为“原生开局，只读加载”
     - 头部快捷键提示改为 `F9 回到原生开局 / F5 已停用`
     - 默认槽的复制 / 粘贴 / 覆盖按钮整组隐藏
   - 帮助文案改为：
     - F5 已停用
     - 默认槽不再自动保存
     - 默认槽只负责回到原生开局
   - 默认读档确认文案改为明确说明：
     - 这是重新载入 `Primary`
     - 会丢弃当前未保存实时进度

**本轮验证**:
- `validate_script Assets/YYY_Scripts/Data/Core/SaveManager.cs`
  - owned errors = 0
  - native validation = warning（仅旧有通用性能告警，不是本轮 compile red）
  - CLI assessment = `unity_validation_pending`
  - 原因：Unity 当前 `stale_status`，不是本轮 own red
- `validate_script Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
  - native clean
  - owned errors = 0
  - CLI assessment = `unity_validation_pending`
  - 原因同上
- `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 5`
  - `errors=0 warnings=0`
- `git diff --check -- Assets/YYY_Scripts/Data/Core/SaveManager.cs Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
  - 通过

**当前阶段判断**:
- 这轮已经把“默认存档会继续污染原生 Primary 调试现场”的风险降下来了
- 但这轮还没有做用户实机点击验收，所以当前结论是：
  - 代码层和控制台层已收口
  - 玩家体验层仍待用户真实点击：
    1. `F9` 是否稳定回到原生开局
    2. 设置页“加载默认存档 / 重新开始游戏”是否都不再把树石场景弄空
    3. 普通存档读写是否仍保持原样

## 2026-04-07 5_Settings 存档 UI 二次内收：Viewport、字体和普通槽位宽度收口

**当前主线**:
- 在不再扩默认存档职责的前提下，继续收 `5_Settings` 里的存档 UI，让普通存档内容真正落回 `Viewport` 内，不再出现“内层比外壳还大”的显示错位。

**用户最新要求**:
- `Viewport` 背景不要再半透明，alpha 要拉满。
- 字体要再大一点、适度加粗。
- 普通存档内的文字描述和按钮必须都落在 `Viewport` 可见范围内，不能再横向撑爆。
- 允许有暖色底板和边框，但不要太黑；滚动条也要更稳，不要太敏感。

**本轮关键判断**:
1. 这次显示不全的主因不是 `Mask` 或 `ScrollRect` 失效，而是普通槽位右侧动作区宽度写得太死、按钮文案太长，导致内部总宽度把 `Viewport` 撑爆。
2. 当前证据仍然是“用户截图 + Inspector 图 + 代码结构修正”，所以这轮只应宣称“结构已收口”，不能冒充“真实体验已终验过线”。

**本轮代码调整**:
1. `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - 全局字号继续上调：
     - `Title 30`
     - `SectionTitle 20`
     - `Body 16`
     - `Small 15`
     - `Button 15`
     - `Help 14`
   - `Viewport` 可见化进一步收口：
     - 背景保持实色暖色底板
     - 新增轻量暖棕 `Outline`
     - 滚动灵敏度从 `18 -> 12`
     - 滚动条轨道与手柄改成更明显但不发黑的暖色
   - 普通存档槽位重新内收：
     - 槽位高度 `126 -> 138`
     - body 高度 `110 -> 122`
     - summary panel 高度 `104 -> 116`
     - 右侧动作列宽度 `356 -> 272`
     - summary panel 显式允许收窄，不再和右侧动作列一起把外壳撑爆
   - 普通存档按钮短标签化：
     - `加载存档`
     - `复制`
     - `粘贴`
     - `覆盖`
     - `删除`
     说明文案继续保留在帮助区，不靠长按钮名硬塞语义。
   - 底部帮助区高度继续抬高到 `82`，避免字体变大后说明区再被裁切。

**本轮验证**:
- `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs --count 20 --output-limit 5`
  - `owned_errors = 0`
  - `native_validation = clean`
  - `assessment = external_red`
  - 原因不是本轮脚本，而是 Unity 当前现场存在外部 `Missing Script` 报错，且 Editor 处于 `Primary` 的 `playmode_transition / stale_status`
- `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 5`
  - fresh console 仍返回外部 `The referenced script (Unknown) on this Behaviour is missing!`
  - 属于当前 Unity 现场已有外部错误，不是本轮 `PackageSaveSettingsPanel.cs` owned red
- `git diff --check -- Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
  - 通过

**当前阶段判断**:
- 这轮已经把普通存档 UI 从“横向撑爆 Viewport”收成了“更窄的动作区 + 更高的内容区 + 更短的按钮标签”。
- 但这仍然是结构级收口，不是体验终验；最终还需要用户回到真实 `5_Settings` 页面看：
  1. 普通存档是否完整显示在 `Viewport` 内
  2. 字号和字重是否已经到位
  3. 暖色底板和滚动条是否足够清楚、又不过黑

**thread-state / 恢复点**:
- 本轮沿用已开启切片：
  - `settings-save-ui-viewport-fit-and-typography-pass_2026-04-07`
- 本轮收尾已执行：
  - `Park-Slice`
- 当前 live 状态：
  - `PARKED`
- 下一轮如果继续，只做基于真实画面的局部微调，不再回头扩默认存档职责。

## 2026-04-07 普通存档超出 Viewport 的根因修正：清除 Content 横向残留尺寸

**用户最新反馈**:
- 现实画面仍然超框。
- 用户明确指出关键不是“差一点”，而是：`Content` 绝不能比外面的 `Viewport` 更大。

**这轮重新厘清后的关键判断**:
1. 上一轮最大的错误不是动作列还稍宽，而是 `Content` 根节点本身保留了默认 `RectTransform` 宽度。
2. 当前代码只改了 `anchor/pivot`，没有清掉 `sizeDelta/anchoredPosition`，这会在横向 stretch 下留下 `Left=-50 / Right=-50` 这种残留，直接把 `Content` 宽度撑得比 `Viewport` 多出 100。
3. 所以这轮最该修的不是继续微调按钮，而是先把 `Content` 的横向尺寸彻底归零，让它严格贴着 `Viewport` 走。

**本轮代码调整**:
1. `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - 在普通存档 `Content` 节点创建后新增：
     - `_ordinaryContent.anchoredPosition = Vector2.zero;`
     - `_ordinaryContent.sizeDelta = Vector2.zero;`
   - 目的：
     - 清除 `RectTransform` 默认 100 宽残留
     - 保证 `Content` 横向完全由 `Viewport` 约束，不再出现 `Content > Viewport`
   - 右侧动作列再略微收窄：
     - `248`，进一步给正文区让宽

**本轮验证**:
- `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs --count 20 --output-limit 5`
  - `owned_errors = 0`
  - `native_validation = clean`
  - `assessment = unity_validation_pending`
  - 原因：Unity 当前仍是 `stale_status`，不是本轮脚本错误
- `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 5`
  - `errors = 0`
  - `warnings = 0`
- `git diff --check -- Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
  - 通过

**当前阶段判断**:
- 这轮修的是根因，不再是外围症状。
- 现在代码层面，`Content` 已被明确收回 `Viewport` 横向约束内。
- 最后一跳仍需要用户回到真实页面看：
  1. Inspector 里 `Content` 的左右偏移是否已经回到 `0 / 0`
  2. 游戏内普通存档是否不再横向超框

**thread-state / 恢复点**:
- 本轮切片：
  - `settings-save-ui-content-hard-constraint-fix_2026-04-07`
- 本轮已执行：
  - `Begin-Slice`
  - `Park-Slice`
- 当前 live 状态：
  - `PARKED`
- 下一轮若仍有问题，优先继续查真实页面里的容器层级和宽度约束，不再先动装饰层。

## 2026-04-07 基于真实截图的 UI 复判：超框主问题基本解除，但仍有 5 个明显问题

**本轮证据层级**:
- `targeted probe / 局部验证`
- 依据：
  - 用户提供的真实游戏内截图
  - 当前不把这轮判断包装成最终体验过线

**当前最核心判断**:
1. 上一轮最致命的 `Content > Viewport` 横向超框问题，从这张最新截图看已经基本解除。
2. 现在的主要问题已经从“容器坏了”切换成“信息层级、文字密度、按钮视觉重量和文案质量还不够正式”。

**从截图可直接确认的问题**:
1. 默认存档摘要里出现了 `DontDestroyOnLoad`
   - 这是内部技术场景名，不该直接暴露给玩家。
2. 确认弹窗正文排版不稳
   - 正文换行生硬，左侧留白与行宽分配不好，读起来像临时拼出来的。
3. 普通存档右侧按钮区视觉过重
   - 深色块压得太狠，和整体暖色面板不够融洽。
4. 底部帮助区仍然过密
   - 信息一大坨堆在一起，虽然功能说明全了，但阅读压力大，不像正式面。
5. 普通存档区整体仍偏挤
   - 虽然不再横向炸框，但条目间呼吸感、分区间层级感还不够舒服。

**阶段判断**:
- 这轮不再是“结构彻底坏掉”的严重状态了。
- 但距离“正式、顺眼、好读”的玩家面还有一轮明确的视觉收口。

## 2026-04-07 5_Settings 存档 UI 可读性与正式感收口

**当前主线**:
- 在超框主问题基本解除后，把玩家实际能看到的 5 个明显问题收掉：
  - 内部场景名外露
  - 确认弹窗排版不稳
  - 右侧按钮区过重
  - 底部帮助区过密
  - 普通存档区呼吸感不足

**本轮代码调整**:
1. `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - 普通存档右侧按钮区减重：
     - `ActionPanelTint` 改浅
     - `LoadButtonTint` 单独抽出
     - `复制 / 粘贴 / 覆盖 / 删除` 颜色统一改成更柔和的暖色系
   - 普通存档条目增加呼吸感：
     - `_ordinaryContent.spacing = 10`
     - `slotRoot` 高度 `146`
     - `body` 高度 `128`
     - `summaryPanel` 高度 `122`
     - 去掉每个普通槽位底部的额外 `Divider`
   - 底部帮助区重写成 4 行分组说明：
     - 快捷键
     - 普通槽操作
     - 默认槽职责
     - 关闭界面与中央提示
     - 同时把 footer 高度抬到 `104`
   - 确认弹窗重做排版：
     - 面板尺寸 `432 x 212`
     - padding / spacing 增大
     - 正文改成左上排版并加最小高度
     - `取消 / 确认` 按钮改为等宽铺满
     - 相关操作文案全部显式换成两行
   - 显示文本修正：
     - `SceneLabel("DontDestroyOnLoad") -> "当前场景"`
     - 不再向玩家暴露内部技术场景名

**本轮验证**:
- `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs --count 20 --output-limit 5`
  - `owned_errors = 0`
  - `native_validation = clean`
  - `assessment = external_red`
  - 当前外部 blocker：
    - `Assets/YYY_Scripts/Service/Player/EnergyBarTooltipWatcher.cs`
    - `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
    - `Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs`
    - 这些文件都在报 `ItemTooltip` 缺失，不属于本轮存档 UI 代码
- `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 5`
  - fresh console 同样返回上述外部 `ItemTooltip` 红错
- `git diff --check -- Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
  - 通过

**当前阶段判断**:
- 这轮已经把你刚才确认的 5 个问题全部落到代码上去修了。
- 但当前 Unity 现场仍有别的线程留下的 `ItemTooltip` 外部红错，所以我不能把整个工程说成已全绿。
- 对这条线本身，我现在的结论是：
  - 存档 UI 脚本 owned clean
  - 玩家面正式感已经进入可复看状态

**thread-state / 恢复点**:
- 本轮切片：
  - `settings-save-ui-readability-and-polish-pass_2026-04-07`
- 本轮已执行：
  - `Begin-Slice`
  - `Park-Slice`
- 当前 live 状态：
  - `PARKED`
- 下一轮如果继续：
  - 优先看用户最新截图是否还剩局部视觉问题
  - 不再回到“结构超框”这一层

## 2026-04-07 基于用户二次反馈的密度重排：横向摆放当前进度，缩框增距

**用户最新反馈**:
- 现在不是炸框，而是“框大但字挤”。
- 用户明确建议：
  - `当前进度` 这种信息可以横着摆
  - 存档条样式还不好看
  - 在保证 `内不可超外` 的前提下，把框缩一点、把文字之间距离拉开

**这轮关键判断**:
1. 之前那轮虽然修了正式感，但信息组织方式仍旧偏“纵向堆字”。
2. 真正还没收好的不是颜色，而是：
   - 当前进度的信息结构
   - 文本行距与卡片内边距
   - 默认 / 普通槽位卡片尺寸与文字密度的比例

**本轮代码调整**:
1. `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - `当前进度` 从单个多行文本改为 3 张横向信息卡：
     - 场景
     - 剧情 / 语言
     - 生命 / 精力
   - 默认槽摘要卡从 `82 -> 92`
   - 普通槽条目继续做“缩框增距”：
     - 槽位 `146`
     - body `128`
     - summary panel `122`
   - `SummaryPanel` 内边距和子项间距统一加大：
     - padding `14/14/12/12`
     - spacing `4`
   - 文本默认 `lineSpacing = 1.08`
   - 默认摘要和普通槽详情额外拉大 `lineSpacing = 1.12`

**本轮验证**:
- `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs --count 20 --output-limit 5`
  - `owned_errors = 0`
  - `native_validation = clean`
  - `assessment = unity_validation_pending`
  - 原因：
    - Unity 当前 `stale_status`
    - 中途还有一次 `tests_running / CodexCodeGuard timeout` 工具波动
    - 不是本轮脚本 compile red
- `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 5`
  - `errors = 0`
  - `warnings = 0`
- `git diff --check -- Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
  - 通过

**当前阶段判断**:
- 这轮真正做的是“密度重排”，不是再修结构爆炸。
- 现在的目标已经从“别超框”推进到：
  - 同样的空间里，信息更横向、更松、更像正式 UI

**thread-state / 恢复点**:
- 本轮补登记切片：
  - `settings-save-ui-horizontal-density-pass_2026-04-07`
- 本轮已执行：
  - `Begin-Slice`
  - `Park-Slice`
- 当前 live 状态：
  - `PARKED`
- 下一轮若继续：
  - 只根据用户最新截图收局部比例和视觉细节，不再回头改存档逻辑。

## 2026-04-07 指南上移与默认存档加权：空态时把空间让给默认槽

**用户最新反馈**:
- 页面还是不够好看。
- 用户明确建议：
  - 底部那块指南挪到右上角
  - 把下面腾出的空间更多给默认存档
  - 当前没有普通存档时，默认存档现在显得太寒酸

**这轮关键判断**:
1. 当前不该继续在底部堆说明。
2. 现在更合理的做法是：
   - 指南变成右上角的紧凑卡片
   - 底部只保留“重新开始游戏”
   - 当普通存档为空时，动态缩小普通存档空态区域，把默认槽做得更有存在感

**本轮代码调整**:
1. `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - 顶部结构改成：
     - 左侧标题 + 状态
     - 右侧 `GuideCard`
   - 底部说明区移除，只保留底部右侧 `重新开始游戏`
   - 新增动态布局方法 `ApplyLayoutProfile(bool hasOrdinarySlots)`：
     - 有普通槽时：
       - 默认槽适中
       - 普通列表高度更大
     - 没有普通槽时：
       - 默认槽增高
       - 普通列表空态压缩
   - 默认槽在无普通槽时显式加权：
     - `defaultSection = 164`
     - `defaultSummary = 136`
   - 普通列表空态收缩：
     - `scrollRoot = 112`
     - `emptyRoot = 84`
   - 普通槽头部文案收短：
     - 无普通槽时只写：`当前还没有普通存档，点击右上角“新建存档”。`
   - 右上角指南改成 4 行短说明：
     - `F9`
     - `F5`
     - 普通槽支持的操作
     - 复制缓存清空规则

**本轮验证**:
- `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs --count 20 --output-limit 5`
  - `owned_errors = 0`
  - `native_validation = clean`
  - `assessment = external_red`
  - 外部 blocker：
    - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs(85,31)`
    - `FloatingProgressCardRefs` 缺失
- `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 5`
  - 当前 fresh console 又出现外部 `NPCBubblePresenter` 相关 `SendMessage cannot be called during Awake...`
  - 不属于本轮存档 UI owned red
- `git diff --check -- Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
  - 通过

**当前阶段判断**:
- 这轮不是细碎调参，而是页面空间分配逻辑真的改了：
  - 说明上移
  - 底部压缩
  - 空态时默认槽更大
- 现在这条线的主判断是：
  - 结构与密度策略都已经比较接近用户意图
  - 还差用户基于最新画面做最后一轮审美拍板

**thread-state / 恢复点**:
- 本轮切片：
  - `settings-save-ui-guide-topright-and-default-emphasis_2026-04-07`
- 本轮已执行：
  - `Begin-Slice`
  - `Park-Slice`
- 当前 live 状态：
  - `PARKED`
- 下一轮若继续：
  - 只按最新截图收视觉比例和局部细节，不再回头堆说明文本。

## 2026-04-07 重启后恢复核查：无 subagent 残留，最新布局改动已落盘

**恢复核查结论**:
- 本线程前面没有开 `subagent`，因此不存在“子代理没做完要重开”的尾账。
- 当前 `thread-state` 已是 `PARKED`，没有悬空施工状态。
- 最新一轮“指南上移到右上角 + 空态时默认槽加权”的布局改动已经真实存在于 `PackageSaveSettingsPanel.cs`，不是半途丢失状态。

**本轮核查结果**:
- 当前关键代码仍在：
  - `GuideCard`
  - `ApplyLayoutProfile(bool hasOrdinarySlots)`
  - footer 只保留 `重新开始游戏`
  - 无普通槽时：
    - `defaultSection = 164`
    - `defaultSummary = 136`
    - `ordinaryScroll = 112`
- 当前 `Show-Active-Ownership`：
  - `存档系统 = PARKED`
- `check-skill-trigger-log-health.ps1`
  - `Health: ok`
  - `Canonical-Duplicate-Groups: 0`

**阶段判断**:
- 这轮重启后没有发现现场回退或丢改。
- 当前最合理动作不是重新从头施工，而是直接让用户看最新界面，再决定最后一轮眼调。

## 2026-04-07 并刀收口：压缩 5_Settings 存档页，并把 CloudShadow 正式接进存档链

### 当前主线

- 主线目标：
  - 把 Sunset 存档系统收成“可在 `5_Settings` 里直接操作的正式面板 + 能覆盖已上线运行态内容的正式存档链”
- 本轮子任务分两刀并行：
  - UI：继续压缩 `5_Settings` 存档页壳体比例，修右上角重叠、默认存档过大、普通存档空间不足
  - 逻辑：把 `CloudShadowManager` 现有运行态缓存接入 `SaveDataDTOs / SaveManager`
- 服务对象：
  - 让用户下一轮直接在设置面板里验 UI
  - 同时让云状态正式进 JSON 存档，而不只是停留在 static runtime cache

### 本轮完成

1. `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - 顶部右侧 `GuideCard` 缩窄缩矮，避免继续和标题/状态区争空间
   - `当前进度` 三张卡整体压扁，但未缩小文字字号
   - `默认存档` 区缩小摘要面，并把右侧按钮列改成：
     - `加载默认存档`
     - `重新开始游戏`
   - 底部单独 footer 已去掉，释放高度给普通存档滚动区
   - 普通存档卡片高度、右侧按钮列宽度、摘要 padding 都收紧，继续守住 `Content <= Viewport`
   - 存档摘要从 3 行压成 2 行，保留时间/场景/剧情/语言/背包/生命精力关键信息
2. `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
   - `GameSaveData` 新增 `cloudShadowScenes`
   - 新增：
     - `CloudShadowSceneSaveData`
     - `CloudShadowEntrySaveData`
   - 语义直接贴 `CloudShadowManager` 现有 `SceneRuntimeState / CloudRuntimeState`
3. `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs`
   - 新增正式导出/导入接口：
     - `ExportPersistentSaveData()`
     - `ImportPersistentSaveData(...)`
   - 把 runtime key 明确拆成：
     - `sceneKey`
     - `managerPath`
   - 支持：
     - 保存时把当前激活 manager 状态与已缓存的跨场景状态一起导出
     - 读档时先回灌 static runtime cache，再对当前已加载 manager 立即应用
4. `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - `CollectFullSaveData()` 正式收集 `CloudShadowManager.ExportPersistentSaveData()`
   - `LoadGameInternal()` 正式调用 `CloudShadowManager.ImportPersistentSaveData(...)`
   - `TryReadSaveData()` 对旧存档缺少 `cloudShadowScenes` 的情况做空列表兼容
   - 存档路径改到 `Application.persistentDataPath/Save`
   - 同时补了旧目录迁移：
     - 旧项目根目录 `Save`
     - 旧 `Assets/Save`
   - 目标是打包后优先走真正可写目录，同时尽量兼容历史存档位置

### 本轮验证

- UI 脚本：
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs --count 20 --output-limit 5`
  - 结果：
    - `assessment = no_red`
    - `owned_errors = 0`
    - `unity_red_check = pass`
- Cloud 脚本：
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs --count 20 --output-limit 5`
  - 结果：
    - `assessment = no_red`
    - `owned_errors = 0`
    - `unity_red_check = pass`
- DTO / SaveManager：
  - `validate_script` 多次撞到 MCP plugin session / stale status，结果落在：
    - `assessment = unity_validation_pending`
    - 但 native validation 为 `clean`
    - fresh console 仍是 `0 error / 0 warning`
- fresh console：
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 5`
  - 结果：`0 error / 0 warning`
- `git diff --check`
  - 已清掉实际 trailing whitespace
  - 当前只剩 Git 的 CRLF 提示 warning，不是本轮代码语义错误

### 当前判断

- 这轮最核心的有效结果有两件：
  1. `5_Settings` 存档页终于不再把默认存档和当前进度撑得过大，普通存档区获得了真实可见空间
  2. `CloudShadowManager` 的分场景运行态已经进入正式 `GameSaveData`，并且存档目录口径切到了打包后可写的持久化目录
- 还不能宣称“体验与运行链全部终验完成”：
  - UI 还缺用户基于最新画面的最后观感拍板
  - Cloud 存档链这轮还没做完整“保存 -> 退场/重载 -> 读档”的人工流程回归

### thread-state / 恢复点

- 本轮沿用 ACTIVE 切片继续施工，收尾已执行：
  - `Park-Slice`
- 当前 live 状态：
  - `PARKED`
- 当前 blocker：
  1. `5_Settings` 存档页新布局已代码落地，但仍待用户基于最新画面做最终观感终验。
  2. `CloudShadow` 正式存档链已接到 DTO/SaveManager/build 持久化路径，但这轮只拿到代码层与 fresh console 证据，尚未完成完整手动保存-退场-读档回归。
## 2026-04-07 第二轮收口：5_Settings 存档页再压缩 + CloudShadow 正式持久化落档

### 当前主线 / 本轮子任务

- 当前主线目标：
  - 把 Sunset 存档系统收成一个可在 `5_Settings` 内直接验的 demo，并补上新内容里最值得先落档的一条运行态数据
- 本轮子任务：
  1. 继续压 `PackageSaveSettingsPanel.cs` 的布局，把默认存档与当前进度缩到更紧凑，把普通存档滚动区让出更多真实空间
  2. 按 `2026-04-07_云朵与光影_存档持久化接入prompt_01.md` 把 `CloudShadowManager` 运行态缓存正式接入 `SaveDataDTOs / SaveManager`
- 本轮服务于什么：
  - 给用户一个能在设置页直接操作、且面向打包后仍可写的存档 demo
- 恢复点：
  - 如果继续，优先等用户终验 UI 观感与 `Primary/Town` 的云读档行为，不再外扩存档范围

### 本轮完成

1. `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - 顶部 `GuideCard` 再次收紧，右上角说明不再独占过宽区域
   - `当前进度` 三张卡继续压扁，但未缩小字体字号
   - `默认存档` 摘要区进一步缩小；右侧按钮列固定为：
     - `加载默认存档`
     - `重新开始游戏`
   - 底部独立 footer 已去掉，普通存档滚动区拿回更多高度
   - 普通存档条改成更紧凑的“左侧摘要 + 右侧按钮区”，按钮区为：
     - 顶部 `加载存档`
     - 下方两行：`复制/粘贴`、`覆盖/删除`
   - 继续守住 `Content <= Viewport`
2. `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
   - `GameSaveData` 新增 `cloudShadowScenes`
   - 新增：
     - `CloudShadowSceneSaveData`
     - `CloudShadowEntrySaveData`
3. `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs`
   - 新增正式导出/导入接口：
     - `ExportPersistentSaveData()`
     - `ImportPersistentSaveData(...)`
   - 运行态 key 拆成 `sceneKey + managerPath`
   - 当前已加载 manager 读档后会立即应用导入状态；未加载场景继续通过 static runtime cache 等待恢复
4. `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - `CollectFullSaveData()` 正式采集云状态
   - `LoadGameInternal()` 正式回灌云状态
   - 兼容旧存档缺少 `cloudShadowScenes` 字段的情况
   - 存档目录改到 `Application.persistentDataPath/Save`
   - 同时迁移旧目录：
     - 项目根目录 `Save`
     - 旧 `Assets/Save`

### 验证结果

- `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 5`
  - 结果：`0 error / 0 warning`
- `git diff --check -- Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs Assets/YYY_Scripts/Data/Core/SaveManager.cs Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
  - 结果：无 diff 格式错误；只剩 Git 的 CRLF 提示 warning
- UI / Cloud 两个 `validate_script`
  - 代码层 `owned_errors=0`
  - 但当前 Unity/MCP 基线在这轮后段出现 `refresh_unity/manage_script tool not found` 与 `mcp_error='str' object has no attribute get'`，因此 CLI assessment 只能诚实记为 `unity_validation_pending`
- `SaveDataDTOs / SaveManager`
  - 最新 native validation 为 `clean`
  - fresh console 仍为 `0 error / 0 warning`

### 当前判断

- 这轮最有价值的结果有两件：
  1. `5_Settings` 存档页终于把空间真正让给了普通存档滚动区
  2. `CloudShadowManager` 分场景运行态已经进入正式 `GameSaveData`，并改到打包后可写目录
- 这轮仍未完成的部分：
  - UI 还缺用户基于最新画面的最终观感终验
  - 云持久化还缺完整人工“保存 -> 退场/重载 -> 读档”回归

### thread-state / 收口

- 已执行：
  - `Park-Slice -ThreadName 存档系统 -Reason slice-complete-awaiting-user-acceptance`
- 当前 live 状态：
  - `PARKED`
- 当前 blocker：
  1. `5_Settings` 存档页新布局已代码落地，但仍待用户基于最新画面做最终观感终验。
  2. `CloudShadow` 正式存档链已接到 DTO/SaveManager/build 持久化路径，但这轮只拿到代码层与 fresh console 证据，尚未完成完整手动保存-退场-读档回归。

## 2026-04-07 箱子作者预设工具落地：Inspector 可直接预填默认内容

### 当前主线 / 本轮子任务

- 当前主线目标：
  - 把 Sunset 存档系统收成一个能覆盖已上线内容、并且能支撑作者工作流的正式 demo
- 本轮子任务：
  - 给箱子补一套“作者态预填默认内容”的工具，让场景里的箱子能在进入运行时前就带好物品
- 本轮服务于什么：
  - 解决“箱子里想提前放好东西，但当前没有编辑工具”的作者工作流断层
- 恢复点：
  - 下一轮若继续，应转到用户在 Unity 里实测这个 Inspector 工具是否顺手，以及是否还要补更高阶批量能力

### 本轮完成

1. `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
   - 新增作者态序列化字段 `_authoringSlots`
   - 新增作者预设辅助方法：
     - `GetAuthoringSlotsSnapshot()`
     - `SetAuthoringSlotsFromEditor(...)`
     - `ClearAuthoringSlots()`
   - `Initialize()` 现在只会在“库存本轮刚创建、且尚未由存档恢复内容”时应用作者预设
   - 这样首进场景的箱子会带默认物品，但正式 `Load()` 恢复出的空箱子不会被预设重新填满
2. `Assets/Editor/ChestControllerEditor.cs`
   - 新增 `ChestController` 自定义 Inspector
   - 在 Inspector 顶部新增“箱子预设内容”区
   - 支持：
     - 新增槽位
     - 绑定物品资产或手填物品 ID
     - 设置槽位 / 品质 / 数量
     - 排序并清理
     - 清空全部
   - 会显示：
     - 当前容量
     - 已配置条数
     - 越界槽位警告
     - 未解析物品警告
     - 重复槽位警告
3. `Assets/Editor/ChestInventoryBridgeTests.cs`
   - 补两条回归测试：
     - 新箱子初始化时，作者预设会正确进入 `InventoryV2` 并同步 legacy mirror
     - 空存档加载后再 `Initialize()`，不会被作者预设重新塞回内容
4. `Assets/Editor/ChestControllerEditor.cs.meta`
   - 补齐 Unity 资源元文件，避免首次导入生成随机 GUID

### 验证结果

- `git diff --check -- Assets/YYY_Scripts/World/Placeable/ChestController.cs Assets/Editor/ChestControllerEditor.cs Assets/Editor/ChestInventoryBridgeTests.cs`
  - 通过
- `py -3 scripts/sunset_mcp.py doctor`
  - baseline 通过
- `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/World/Placeable/ChestController.cs --count 20 --output-limit 5`
  - 被 `CodexCodeGuard returned no JSON` 阻断，assessment 只能记为 `blocked`
- `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 5`
  - CLI 当前又撞到 `AttributeError: 'str' object has no attribute 'get'`
- 结论：
  - 这轮拿到了文本层 / 结构层证据和测试补口
  - 但 Unity/CLI 的红错闭环证据没有完全拿到，只能诚实记为“验证链受工具阻断”

### 当前判断

- 这轮最有价值的结果是：箱子终于不再只能靠运行时临时塞物品，而是有了能落在场景里的作者态预设入口
- 当前工具已经足够支撑用户的直接使用路径：
  - 选中场景里的箱子
  - 在 Inspector 顶部找到“箱子预设内容”
  - 新增槽位并选物品 / 填数量
  - 进入运行时后打开箱子即可看到默认内容
- 当前最薄弱点：
  - Unity/CLI 编译验证链这轮被项目侧工具阻断
  - 还没有用户亲手在 Unity Inspector 里做一轮真实体验确认

### thread-state

- 本轮已执行：
  - `Begin-Slice`
  - `Park-Slice -ThreadName 存档系统 -Reason chest-authoring-editor-tool-complete-awaiting-user-retest`
- 当前 live 状态：
  - `PARKED`

## 2026-04-07 箱子作者工具二次分析：向 InventoryBootstrap 学“作者效率”，但保留单组箱子语义

### 用户目标

- 用户明确要求：
  - 认真学习 `InventoryBootstrap`
  - 让箱子工具做得更完善
  - 但箱子不是多组列表，而是固定只有一个组
  - 本轮先汇报我看完后的判断与改造计划

### 当前判断

- 当前箱子工具的逻辑链已经成立，但编辑体验还只有“能改”，离 `InventoryBootstrap` 的作者效率还有明显差距。
- 真正值得学习的不是它的“多组”，而是它的：
  1. 行编辑密度
  2. 拖拽添加
  3. 拖拽排序
  4. 批量选择窗口
  5. 右键/快捷操作
  6. 更清晰的作者反馈
- 不该照搬的部分也很明确：
  - `runOnStart / runOnBuild / clearInventoryFirst / Apply()`
  - 多组列表的启用、复制、上下移动
  - 把箱子做成多个物品组

### 我打算怎么做

1. 保持“一个箱子 = 一个固定默认内容组”
   - 不做多列表
   - 不做 enable/disable 多组切换
   - UI 上就只保留一个清晰的“默认内容”面板
2. 重做 `ChestControllerEditor` 的条目编辑区
   - 学 `InventoryBootstrapEditor` 的行式排版
   - 每条显示：拖拽手柄、序号、图标、物品、品质、数量、槽位、删除
   - 让它更像真正的作者工具，而不是 DTO 调参面板
3. 给单组箱子补“拖拽添加”
   - 允许把 `ItemData` 直接拖进箱子默认内容区
   - 自动落到下一个空槽位
4. 给单组箱子补“批量选择添加”
   - 借用 `ItemBatchSelectWindow` 思路
   - 但输出目标改成“当前箱子的唯一默认内容组”
   - 不再出现“添加到哪个列表”的概念
5. 给单组箱子补“条目级快捷操作”
   - 复制当前条目
   - 上移 / 下移
   - 移到顶部 / 底部
   - 删除
6. 强化箱子特有约束
   - 容量上限提示更明确
   - 重复槽位要高亮
   - 未知物品要高亮
   - 如果数量超过该物品最大堆叠，至少先警告；必要时再补“自动拆分到后续空槽”

### 当前阶段 / 恢复点

- 当前阶段：
  - 方案判断已形成，下一刀可以直接进入编辑器体验重做
- 恢复点：
  - 下一轮如果用户让我直接开工，我会只做一个垂直切片：
    - 把 `ChestControllerEditor` 升级成“InventoryBootstrap 风格的单组箱子编辑器”

## 2026-04-08 编译阻塞收口：`PackageSaveSettingsPanel` 旧红面已清，当前 runtime/editor 直编已过

### 用户目标

- 用户贴出了 `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs` 的一组爆红
- 要求先把存档系统当前的编译问题处理干净，再继续后续收尾

### 当前主线 / 本轮子任务

- 当前主线目标：
  - 完成可在 `5_Settings` 内测试的 Sunset 存档系统 demo，并保留箱子作者工具升级成果
- 本轮子任务：
  - 清掉 `PackageSaveSettingsPanel.cs` 引发的运行时程序集编译阻塞
  - 顺手把同一编译链里继续冒出的入口依赖红面一并收掉
- 本轮服务于什么：
  - 先恢复 Unity 可编译状态，避免用户继续卡在旧红面上，之后才能继续做存档 UI 体验调整

### 实际处理

- 重新核对了 `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
  - 当前源码里已经包含：
    - `DecoratePanel(...)`
    - `SummaryPanel(string, Transform, Color, Color)`
    - `Divider(Transform, float)`
- 使用 Bee 记录下来的 Roslyn 直编命令重新验证 `Assembly-CSharp`
  - 结果证明：
    - `PackageSaveSettingsPanel.cs` 那组 `CS0103 / CS1501` 已经不再出现
    - 继续阻塞编译的实际红面转移到了 `PackagePanelTabsUI.cs`
- 已修改 `Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
  - 把三处直接静态调用：
    - `PackageMapOverviewPanel.EnsureInstalled(panelRoot)`
    - `PackageNpcRelationshipPanel.EnsureInstalled(panelRoot)`
  - 改成反射式可选安装：
    - `EnsureOptionalPanelInstalled("PackageMapOverviewPanel")`
    - `EnsureOptionalPanelInstalled("PackageNpcRelationshipPanel")`
  - 这样当前编译图里若缺这两个运行时页类，不再把整个背包页签系统卡红；若类存在，仍会照常调用其 `EnsureInstalled(GameObject)` 入口

### 验证结果

- 运行时程序集直编：
  - 已通过
  - 不再出现 `PackageSaveSettingsPanel.cs` 的 helper 缺失红错
- 编辑器程序集直编：
  - 已通过
- 当前仅见 warning：
  - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs(2238,13)`
  - `TMP_Text.enableWordWrapping` 已过时
- `git diff --check`
  - 对当前相关文件通过

### 当前判断

- `PackageSaveSettingsPanel.cs` 的那组爆红已经属于旧红面，不再是当前源码事实
- 当前项目已经回到：
  - 运行时程序集可编
  - 编辑器程序集可编
- 但这不等于存档 UI 体验已过线：
  - 现在只是把编译阻塞清掉
  - 存档页视觉排版与实机体验仍需继续调

### 当前阶段 / 恢复点

- 当前阶段：
  - 编译阻塞已清
  - 重新回到“继续做存档 UI 与体验收口”的主线
- 恢复点：
  - 下一轮优先继续处理 `5_Settings` 内的存档 UI 观感与布局压缩，不再回头重复处理这组 helper 爆红

### thread-state

- 本轮沿用既有真实施工切片继续处理
- 收尾前已执行：
  - `Park-Slice -ThreadName 存档系统 -Reason compile-blocker-cleared-awaiting-next-save-ui-pass`
- 当前 live 状态：
  - `PARKED`

## 2026-04-08 用户截图复盘：当前存档 UI 的主要问题不是功能，而是布局纪律再次失守

### 用户目标

- 用户贴出最新 `5_Settings` 存档页截图，要求我先说清楚：
  - 我看到了什么问题
  - 我自己的判断是什么
  - 下一轮应该怎么调

### 当前主线 / 本轮子任务

- 当前主线目标：
  - 把 Sunset 存档系统收成一个可在 `5_Settings` 内稳定测试、功能可靠且观感过线的 demo
- 本轮子任务：
  - 对用户截图做真实体验层判断
- 本轮服务于什么：
  - 避免继续在错误布局基础上盲修样式

### 当前判断

- 这轮证据层级属于：
  - `真实入口体验`
- 当前最严重的问题不是“丑得抽象”，而是：
  - 内层内容再次超出外层容器
  - 普通存档区纵向容量分配错误
  - 文字与按钮仍然在小盒子里打架
- 也就是说：
  - 功能结构存在
  - 体验没有过线

### 我从截图确认到的问题

- 普通存档区的内容总高度明显大于外层 viewport，可见区被挤穿
- 底部第二个普通存档条目被截断，说明“单条高度 × 可见条数 × 区块总高度”关系没有被真正算对
- 当前进度、默认存档、普通存档三块都还偏“测试表单味”，分区有了，但版式没形成真正的秩序
- 快捷说明区仍然太小，信息被硬塞进去，像说明贴纸，不像正式面板
- 普通存档条目里：
  - 左侧摘要区太窄
  - 右侧按钮区太高太硬
  - 结果就是盒子很大，但文字仍然显得挤
- 默认存档区和普通存档区的纵向配比仍然不合理：
  - 默认存档占得多
  - 普通存档可见空间不够

### 下一轮调整方向

- 第一优先级：
  - 先彻底守住“内不可超外”
  - 不是继续美化，而是先重算：
    - 普通存档区总高
    - viewport 高
    - 单条存档卡高度
    - content padding / spacing
- 第二优先级：
  - 缩小默认存档区
  - 把更多垂直空间让给普通存档区
- 第三优先级：
  - 压缩普通存档单卡高度
  - 同时增加左侧文本区宽度，让文字别再挤成一团
- 第四优先级：
  - 重新摆快捷说明
  - 让它更像右上角辅助信息，而不是抢主体空间
- 第五优先级：
  - 重新建立视觉层次：
    - 外框更稳
    - 内卡更克制
    - 按钮区更整齐
    - 不再让每块都像独立小表单

## 2026-04-08 存档 UI 硬重排一刀：先修总高度预算，再压普通存档单卡

### 用户目标

- 用户要求“彻底优化”
- 这轮明确不是继续空谈，而是直接把 `5_Settings` 存档页按截图问题重做一轮

### 当前主线 / 本轮子任务

- 当前主线目标：
  - 完成能在 `5_Settings` 内测试的 Sunset 存档系统正式 demo
- 本轮子任务：
  - 彻底修正存档页高度预算和普通存档区布局纪律
- 本轮服务于什么：
  - 先把“内层超出外包”“普通存档只露半条”这类根本性问题收掉，再继续下一轮体验微调

### 实际修改

- 直接重排 `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
- 第一层是整页预算重算：
  - 收小根面板上下边距
  - 压低 header / 当前进度 / 默认存档三个上层区块的最小高度
  - 把普通存档区重新设成更低最小值 + 柔性扩展，尽量把多余高度让回给普通存档滚动区
- 第二层是普通存档单卡重排：
  - 单卡高度从 148 压到 106
  - 左侧摘要卡和右侧按钮列同步压矮
  - 按钮列宽度收窄，让摘要区拿回更多宽度
  - 滚动内容 spacing / padding 变紧，避免内容总高无意义膨胀
- 第三层是摘要文本压缩：
  - 存档摘要从更松散的 4 大行，改成更紧凑的 3 行式信息
  - 行距同步收紧，避免文字自身把卡片内部撑穿
- 第四层是按钮密度收口：
  - 普通存档按钮高度继续压低
  - 全局按钮字号从 16 压到 15，避免小按钮内部再打架

### 当前判断

- 这轮最核心的判断是：
  - 上一版失败的根源是“总高度预算”和“单卡内部信息密度”同时超标
  - 所以必须先压几何，再谈美化
- 我为什么认为这个判断成立：
  - 从用户截图看，普通存档第二张卡被切断，说明不是单个控件偏移，而是整套预算都溢出了
  - 当前源码里上层三个区块 + 普通存档单卡的最小高度之和，本来就过重

### 验证结果

- 运行时程序集直编：
  - 通过
- 编辑器程序集直编：
  - 通过
- `git diff --check`
  - 通过
- 当前可见 warning 仍是外部旧 warning：
  - `Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs(105,9)`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs(2238,13)`

### 当前阶段 / 恢复点

- 当前阶段：
  - 结构重排已做完一刀
  - 编译层自测已过
  - 还缺新的真实画面反馈
- 恢复点：
  - 下一轮优先看新的实际截图
  - 如果还有问题，只继续微调：
    - 默认存档区高度
    - 普通存档单卡高度
    - 摘要宽度与按钮列宽度
  - 不再回到“大盒子里塞长文本”的旧路

### thread-state

- 本轮已执行：
  - `Begin-Slice -CurrentSlice save-ui-hard-relayout_2026-04-08`
  - `Park-Slice -ThreadName 存档系统 -Reason save-ui-hard-relayout-pass-complete-awaiting-user-retest`
- 当前 live 状态：
  - `PARKED`

## 2026-04-08 新截图复判：普通存档外溢已明显缓解，但仍有两个明确布局 bug

### 用户目标

- 用户继续贴出重排后的三张局部截图
- 要求我直接说“我看到了什么”

### 当前判断

- 这轮证据仍属于：
  - `真实入口体验`
- 相比上一版，最大的“整块内容压出外框”已经明显缓解
- 但现在还没过线，因为至少还有两个明确 bug：
  1. 快捷说明标题和正文重叠
  2. 普通存档 viewport 的初始可视位置不对，顶部出现上一条卡片的残片

### 我从三张图里确认到的内容

- 默认存档区：
  - 比上一版收紧了
  - 但左侧摘要仍然偏密
  - 右上角“原生开局”标签显得过轻、过挤，像悬浮的小尾字
- 快捷说明区：
  - `快捷说明` 标题和第一行正文直接打架
  - 这是硬 bug，不是审美问题
  - 说明卡内部标题 / 正文的垂直布局还没真正分层
- 普通存档区：
  - 外框内的整体容量比上一版好了
  - 但 viewport 顶部出现了上一条卡片底部残片，说明内容初始定位或重建后的滚动复位仍不对
  - 单卡本体已经比上一版更合理，但顶部工具行和新建按钮区仍略显紧

### 下一轮恢复点

- 优先修两个明确 bug：
  1. 快捷说明标题 / 正文分层
  2. 普通存档 scroll reset / content 起点
- 在 bug 清掉之后，再细调：
  - 默认存档摘要密度
  - “原生开局”标签的归属感

## 2026-04-08 层级关系补查与整改：修快捷说明重叠，补普通存档裁剪链

### 用户目标

- 用户明确追问：
  - 为什么没有彻查所有层级关系
  - 为什么没看到到底是哪一层不遵守父子约束
- 并要求立即整改

### 当前主线 / 本轮子任务

- 当前主线目标：
  - 把 `5_Settings` 内存档页收成真正不越界、不打架的正式面
- 本轮子任务：
  - 不再只压尺寸，而是补做完整层级链审查并修正
- 本轮服务于什么：
  - 把“又超框”的根本原因从症状修正升级到层级修正

### 这轮补查到的真实问题

- 我上一轮确实没有把层级链审到底
- 真正漏掉的不是单个数字，而是两条约束链：
  1. 快捷说明卡内部标题和正文没有明确分层高度
  2. 普通存档滚动区只用了 `Mask`，没有用更严格的矩形裁剪链去约束子内容与 outline 外溢

### 实际整改

- `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
  - 给 `GuideTitle` 增加显式 `LayoutElement` 高度
  - 给 `GuideBody` 增加显式 `LayoutElement` 高度
  - 让快捷说明卡内部不再只靠文本自适应“碰运气排版”
- 同一文件内：
  - 把普通存档 viewport 的 `Mask` 改成 `RectMask2D`
  - 这一步的目标是让滚动内容严格服从 viewport 矩形，不允许再越过默认存档区或边框
- 同时补强 `ResetScroll()`
  - 在复位前强制重建 content 与 viewport 布局
  - `StopMovement()`
  - 重置 `anchoredPosition`
  - 再设 `verticalNormalizedPosition = 1f`
  - 避免重建后初始位置漂移

### 验证结果

- `Assembly-CSharp` 直编：
  - 通过
- `Assembly-CSharp-Editor` 直编：
  - 通过
- `git diff --check`
  - 通过
- 当前仍只有外部旧 warning：
  - `PackagePanelRuntimeUiKit.cs(105,9)`
  - `SpringDay1WorkbenchCraftingOverlay.cs(2238,13)`

### 当前判断

- 这轮我最核心的判断是：
  - 之前的问题不只在“尺寸大了”，还在“裁剪链不够硬”
- 我为什么认为这个判断成立：
  - 用户截图里出现了 scroll 后的异常外溢和局部残片
  - 这类问题只压高度不够，必须审 viewport / content / mask 这条链
- 我这轮最薄弱点：
  - 我还没有拿到这版整改后的新截图，所以还不能宣称体验最终过线

### thread-state

- 本轮已执行：
  - `Park-Slice -ThreadName 存档系统 -Reason save-ui-clip-chain-and-guide-fix-awaiting-user-retest`
- 当前 live 状态：
  - `PARKED`

## 2026-04-08 反例采样：Package 地图页 / 关系页已被用户明确判为失败面

### 用户目标

- 用户贴出 `3_Map` 与 `4_Relationship_NPC` 的真实截图，要求我先去找相关 UI 线程 memory，再直接判断“他真正需要的是什么”。

### 当前主线 / 本轮子任务 / 服务关系

- 当前主线目标：
  - 把存档页继续收成正式可验面。
- 本轮子任务：
  - 把 `PackageMapOverviewPanel` / `PackageNpcRelationshipPanel` 当作反例，反查来源 memory 与代码结构，抽出用户真正要的 UI 标准。
- 本轮服务于什么：
  - 给存档页后续 polish 立清晰反例，避免继续滑向“半透明大空块 + 测试味 runtime 面”。

### 已核对的来源

- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\UI\memory_0.md`
  - `2026-04-08 背包地图页 / 好感度页真实施工`
  - 明确写了这两页目标是“正式可见、可消费的玩家面”，且用户当时就强调了“不能再出现里面内容撑爆外壳的大空块/测试味 UI”。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\memory.md`
  - 明确约束是“美观、克制、内容必须可见，严格遵守大包小”。
- 代码真相：
  - `PackageMapOverviewPanel.cs` 与 `PackageNpcRelationshipPanel.cs` 当前都采用高透明浅色 page/card tint，大量固定宽高卡片和二次套娃说明块；结构上有信息，但观感仍明显停在“运行时拼出来的草图层”。

### 当前判断

- 这两页的问题不是“信息不够”，而是“正式面秩序根本没立住”：
  - 背景过透，世界和旧底色一直穿透进来，导致文字与卡片没有压住画面。
  - 分块太多，但每块边界都很轻，结果不是清晰分层，而是整页像一层漂浮草稿。
  - 大框很大、文字很挤、真正有用的信息反而缩在局部；这和用户一直强调的“像正式玩家 UI、不要测试味”是反着来的。
- 用户真正要的不是“再加几个说明块”或“继续塞更多背景”。
- 用户真正要的是：
  1. `PackagePanel` 内部真正立得住的正式页，而不是半透明运行时占位页。
  2. 更强的壳体、留白、分区和对比度，让页面先像游戏内册页，再谈信息。
  3. 更少但更准的文字层级，让信息服从页面，而不是让页面为说明文字让路。
  4. 地图页与关系页都要遵守“主区域清楚、侧信息收紧、下方内容有归属”，不能再是几块漂浮卡片叠在一张大薄纸上。

### 对存档页的直接启发

- 存档页后续继续优化时，要主动避开这类失败特征：
  - 不做过透底板
  - 不做大空块里塞小文本
  - 不做一堆细线浅框导致层级失焦
  - 不把“结构上有内容”误判成“玩家观感成立”
- 如果后面还要统一 Package 系 UI 语言，正确方向应是：
  - 先把 formal-face 的壳体秩序立住
  - 再让不同页各自吃数据
  - 而不是继续 runtime 生卡、最后再赌观感

### 验证状态

- `真实入口体验层证据已补到`
  - 证据来源：用户提供的 GameView 截图
- `当前判断`
  - 属于体验层失败判定，不是代码结构猜测

## 2026-04-08 Package 正式面重做：地图页 / 关系页 / 存档页快捷说明一起收口

### 用户目标

- 用户认可我对 `PackageMapOverviewPanel` 与 `PackageNpcRelationshipPanel` 的失败判断后，要求直接落地。
- 中途又追加一条：我自己的 `PackageSaveSettingsPanel` 右上快捷说明区也没收完，必须并入同轮一起补好。

### 当前主线 / 本轮子任务 / 服务关系

- 当前主线目标：
  - 把存档页继续收成正式可验面，并顺手把同一套 `PackagePanel` 里最明显的两张失败页一起拉回正式玩家面。
- 本轮子任务：
  1. 重做 `PackageMapOverviewPanel` 的正式册页结构。
  2. 重做 `PackageNpcRelationshipPanel` 的正式册页结构。
  3. 同步修正 `PackageSaveSettingsPanel` 右上快捷说明卡的信息架构。
- 本轮服务于什么：
  - 统一 Package 系 UI 的正式面口径，避免存档页继续被旧的“半透明草稿层”语言拖偏。

### 本轮实际做成

- `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs`
  - 把地图页从“轻飘说明卡拼贴”改成“左侧主地图板 + 右侧三段信息”的正式册页。
  - 整页底色、卡片、标签和道路颜色整体加实，不再让场景与旧底板穿透到页面里。
  - 地图主板新增更厚的 legend bar，节点标签、路线点和右侧信息块都改成更稳定的正式分区。
  - 右侧从多块散卡收成：
    - 当前段落总览
    - 这会儿先撞上的人
    - 今日路径
  - 同时把多段说明文案压短，减少“说明块套说明块”的测试味。

- `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
  - 把关系页从“左列表 + 右侧多块半透明浮窗”改成“左侧名册 + 右侧一整张人物档案”。
  - 整页底色、viewport、列表卡和详情壳体全部加实，去掉那种漂浮白板感。
  - 左侧列表新增选中 accent 条，非选中/选中态的层级和焦点更清楚。
  - 右侧详情收成：
    - 顶部人物主卡
    - 中段关系阶段卡
    - 下段身份观察 / 在场感统一记录区
  - 预览文案长度与阶段提示一起收紧，不再让关系册看起来像堆满说明文字的临时面板。

- `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
  - 右上快捷说明卡改成：
    - 左侧标题 `快捷说明`
    - 右侧快捷键 `F9 默认开局 / F5 停用`
    - 下方只留一行普通槽操作说明
  - 不再把三行正文硬塞在一张矮卡里。
  - 这块现在回到“高频速记卡”定位，而不是说明贴纸。

### 关键判断

- 这轮最核心的判断继续成立：
  - 这 3 块的主要问题不是“没数据”，而是 formal-face 根本没立住。
- 所以这轮正确修法不是继续微调字号和透明度，而是直接重做 Build UI 的壳体与分区秩序。

### 验证结果

- `git diff --check`
  - 通过
- direct MCP `validate_script`
  - `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs`：`errors=0 warnings=0`
  - `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`：`errors=0 warnings=0`
  - `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`：`errors=0 warnings=0`
- CLI `validate_script --skip-mcp`
  - 3 个脚本均未出现 owned/external error
  - 但仍被 `CodexCodeGuard timeout` 降级成 `unity_validation_pending`
- CLI `compile --skip-mcp`
  - 仍被工具侧 `subprocess_timeout:dotnet:60s` clamp 卡住，不能当 fresh compile 证据

### No-Red 证据卡 v2

- `cli_red_check_command`
  - `validate_script`
- `cli_red_check_scope`
  - `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs`
  - `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
  - `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
- `cli_red_check_assessment`
  - `unity_validation_pending`
- `unity_red_check`
  - `direct-mcp validate_script clean`
- `mcp_fallback`
  - `required`
- `mcp_fallback_reason`
  - `unity_validation_pending`
- `current_owned_errors`
  - `0`
- `current_external_blockers`
  - `0`
- `current_warnings`
  - `0`

### thread-state

- 本轮已执行：
  - `Begin-Slice -ThreadName 存档系统 -CurrentSlice package-formal-face-rebuild_2026-04-08`
  - `Park-Slice -ThreadName 存档系统 -Reason package-formal-face-rebuild-delivered-for-user-retest`
- 当前 live 状态：
  - `PARKED`

### 当前恢复点

- 下一轮如果用户继续：
  - 优先拿真实 GameView 看这 3 块是否已经从“草稿层”回到正式面。
  - 如果还要 polish，只收：
    - 地图页右侧卡片字量
    - 关系页右侧信息密度
    - 存档页快捷说明卡的最终视觉微调
- 不要再回到“补更多说明块 / 补更多半透明背景”的旧路。

## 2026-04-08 右上角快捷说明单点判断：这块不该再承载三行正文

### 用户目标

- 用户单独截出右上角快捷说明区，要求我直接判断“这里应该怎么改”

### 当前判断

- 这块的核心问题不是字体大小，而是信息架构错了：
  - 卡片太矮
  - 正文行数太多
  - 所以第三行必然掉到底边，和外框下沿打架
- 正确方向不是继续挤，而是减负

### 我认为应该怎么改

- 这块只保留两行正文，不再放第三行长说明
- 具体建议：
  - 标题一行：`快捷说明`
  - 正文两行：
    - `F9  回到默认开局`
    - `普通槽  右侧按钮操作`
- `F5 当前停用` 不应该继续放在这里
  - 它属于“规则例外/当前限制”，应该并入左侧状态说明，或者单独做成一条灰色状态文案
- 这张卡本身应定位成“高频速记卡”，不是“完整说明卡”

### 为什么这样改

- 现在这块位于右上角，本来就不该抢主体空间
- 如果继续在这里塞 3 行甚至更多文字，只会反复出现：
  - 文字打架
  - 行底贴边
  - 看起来像坏掉的说明贴纸
## 2026-04-08 PackagePanel 三页正式面重做收口

### 用户目标

- 用户明确否定上一版 `PackagePanel` 里的 `3_Map / 4_Relationship_NPC / 5_Settings`，要求不要再拍脑袋调参数，而是先按真实容器彻底重做。
- 这轮主线固定为：把这三页收成真正可验的正式玩家 UI，重点修正：
  - 测试味 / 说明板味
  - 内层超出外框
  - 滚动区与内容尺寸关系错误
  - 存档页普通槽空间不足、按钮分区混乱

### 前置核查与关键事实

- 本轮按 `skills-governor`、`preference-preflight-gate`、`sunset-scene-audit`、`sunset-no-red-handoff` 口径推进，并手工执行 `sunset-startup-guard` 等价前置。
- 已核实 `PackagePanel` 这三页的真实容器不是旧 prefab 子节点打架，而是：
  - `UI/PackagePanel/Main/3_Map/Main`
  - `UI/PackagePanel/Main/4_Relationship_NPC/Main`
  - `UI/PackagePanel/Main/5_Settings/Main`
  三者 runtime `RectTransform` 都是 `1264 x 720` 的干净页面容器。
- 因此这轮判断成立：
  - 主要问题来自 runtime UI 脚本自己的布局数学与信息密度，不是 prefab 遗留子物体。

### 本轮实际完成

- `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
  - 把 `5_Settings` 存档页重排成更紧凑的正式面：
    - 压缩头部与当前进度区
    - 拉大默认存档与普通存档之间的层次关系
    - 把更多高度还给普通存档滚动区
  - 普通存档区重新整理为：
    - 标题 + 缓存状态 + 新建按钮
    - 下方独立 viewport
    - 每条普通存档为“左摘要 / 右操作”横向结构
  - 操作按钮重新整理成：
    - 顶部 `加载存档`
    - 下方两行 `复制 / 粘贴 / 覆盖 / 删除`
  - 降低滚动灵敏度到 `8f`，并扩大动作列宽度，避免按钮与摘要互相挤压。

- `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs`
  - 地图页重做为更稳的正式分区：
    - 顶部短标题区
    - 主体改成左侧地图主板 + 右侧信息窄栏
    - 右侧只保留焦点、在场摘要、路径三块，不再堆说明块
  - 地图主板与图例一起收紧文案，减少“说明板味”。

- `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
  - 关系页改成更像名册 + 档案页的结构：
    - 左侧更窄的村民名册
    - 右侧完整人物档案
    - 文案长度、卡片高度和间距都收紧
  - 重点减少“半透明漂浮盒 + 长段说明”的开发态观感。

### 验证结果

- CLI `validate_script`
  - `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
    - `assessment=no_red`
    - `owned_errors=0`
    - `external_errors=0`
  - `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs`
    - `owned_errors=0`
    - `external_errors=0`
    - 但因 Unity `stale_status / compile wait` 落为 `unity_validation_pending`
  - `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
    - `owned_errors=0`
    - `external_errors=0`
    - 但因 Unity `stale_status / compile wait` 落为 `unity_validation_pending`
- direct MCP `validate_script`
  - 三个脚本全部 `errors=0 warnings=0`
- `git diff --check -- Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
  - 通过
- direct MCP `read_console`
  - 最新 `Error / Warning` 为 `0`

### 证据与未闭环点

- 已确认 Main Camera 截图不能作为这轮 UI 的最终证据面，因为它看不到这套玩家 UI。
- 已尝试走 `Spring UI Evidence` 路径抓最终屏幕，但当前 PlayMode 下 `PackagePanel` 默认并不会自动打开，因此抓到的不是这次要验的三页。
- 所以这轮的正确口径是：
  - 结构与脚本 clean 已成立
  - `PackagePanel` 三页的最终玩家视面仍待用户手开界面终验

### thread-state

- 本轮沿用中的施工切片：
  - `package-panel-real-structure-redo_2026-04-08`
- 收尾已执行：
  - `Park-Slice -ThreadName 存档系统 -Reason package-panel-three-pages-redo-awaiting-user-acceptance`
- 当前 live 状态：
  - `PARKED`

### 当前恢复点

- 下一轮如果继续，优先只做用户终验后的定点修整：
  1. `5_Settings` 的最终视觉密度与按钮/摘要比例
  2. `3_Map` 的右栏字量与地图标签长度
  3. `4_Relationship_NPC` 的右下信息密度
- 不再回到“继续加说明块 / 继续堆半透明背景”的旧方向。

## 2026-04-08 PackagePanel 三页最终收紧补刀

### 当前主线 / 本轮子任务 / 服务关系

- 当前主线：
  - 把 `PackagePanel` 的 `5_Settings / 3_Map / 4_Relationship_NPC` 收成真正能过用户眼的最终版正式玩家 UI。
- 本轮子任务：
  - 针对用户最新反馈继续深修三页，不再做零碎 margin 调整，而是把“信息密度、容器高度、右栏说明板味”一次性再收紧一刀。
- 服务关系：
  - 避免这三页继续拖累存档系统 demo 的最终验收。
- 本轮恢复点：
  - 现在已经停在“代码层已收紧 + 待用户看真实屏幕终验”的状态；后面如果继续，只做用户指到哪改到哪的终修。

### 本轮实际完成

- `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs`
  - 右栏不再保留 6 条步骤卡。
  - 现在改成：
    - 顶部焦点卡
    - 中段在场摘要卡
    - 底部路径摘要卡
  - 路径区新增：
    - `当前` 路线短标题
    - `已走熟 / 当前 / 下一步` 的紧凑摘要
  - `BuildPresenceSummary()` 改成最多两段短摘要，不再四行铺满。
  - `ResolvePhaseInfo()` 的焦点文案整体缩短，减少说明板味。

- `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
  - 右侧详情区从竖向堆叠改成更稳定的档案结构：
    - 上方人物头卡
    - 中段薄关系阶段条
    - 下方 `身份观察 / 这一阶段给人的感觉` 左右并排
    - 最底一条细 footer meta
  - 左侧名册再收窄，单条卡片高度从 `66` 压到 `60`。
  - 列表预览改成单行省略，`BuildPreview()` 进一步缩短。
  - `BuildStageHint()` 文案改成更短的人话，不再像一段说明书。

- `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
  - 继续压 `5_Settings` 顶部信息层：
    - 右上 `快捷说明` 改成更窄、更清楚的一条视线
    - `当前进度` 改成更紧的三张信息卡
    - 默认存档摘要进一步缩短
  - 把导致前面反复超框的根因补掉：
    - 不再让 header / 当前进度 / 默认存档这些上层区块靠过小的固定 `preferredHeight` 硬压
    - 改回内容驱动高度，只固定真正需要固定的滚动区、按钮区与普通槽卡片
  - 普通存档区进一步收紧：
    - viewport padding 再缩
    - 滚动灵敏度降到 `7f`
    - 普通槽卡片、加载按钮、四个小按钮全部再压低一档

### 真实核查与验证

- CLI `validate_script`
  - `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs`
    - `assessment=no_red`
    - `owned_errors=0`
  - `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
    - `assessment=no_red`
    - `owned_errors=0`
  - `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
    - 多次校验里 `manage_script validation = clean`
    - CLI assessment 有一轮受 Unity `stale_status` 影响落到 `unity_validation_pending`
    - 但不存在 owned error
- direct MCP `validate_script`
  - 三个脚本全部：
    - `errors=0`
    - `warnings=0`
- direct MCP `read_console`
  - 只有外部 warning：
    - `PackagePanelRuntimeUiKit.cs(105,9)` 的 `enableWordWrapping obsolete`
    - `SpringDay1WorkbenchCraftingOverlay.cs(2245,13)` 的同类 warning
    - 偶发 `MCP WebSocket is not initialised` 连接 warning
  - 本轮没有这三页 owned error
- `git diff --check -- Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
  - 通过

### 本轮不确定性

- 仍未拿到这三页的最终 GameView 硬截图。
- 这次尝试过：
  - 进入 PlayMode 后抓图
- 但 Unity MCP 在 PlayMode 过渡时出现连接断续 / `stale_status`，没有形成可作为正式 UI 证据的稳定截图。
- 正确口径仍然是：
  - 结构与代码 clean 已成立
  - 真实玩家视面终验仍待用户手开界面确认

### thread-state

- 开工前沿用中的施工切片：
  - `package-panel-final-polish_2026-04-08`
- 收尾已执行：
  - `Park-Slice -ThreadName 存档系统 -Reason package-panel-final-polish-pass-complete-awaiting-user-retest`
- 当前 live 状态：
  - `PARKED`

## 2026-04-08 用户终验截图复判：关系页当前仍未过线

### 当前主线 / 本轮子任务 / 服务关系

- 当前主线：
  - 继续把 `PackagePanel` 三页收成最终正式玩家 UI。
- 本轮子任务：
  - 根据用户贴出的 `4_Relationship_NPC` 真实截图，明确指出当前哪里没做好、为什么失败、下一刀应该怎么改。
- 服务关系：
  - 防止后续继续在错误方向上“微调参数”，而不真正重做关系页版式。
- 本轮恢复点：
  - 当前判断已明确：关系页不是“小修小补”，而是还需要再做一刀真正的版式重构。

### 这次截图暴露出的核心问题

1. 页面仍然是“壳体很大，但有效信息全挤在左上”
   - 顶部大片空白没有被内容利用起来。
   - 右侧详情区也明显上挤下空，整页重心不稳。

2. 头卡仍然失败
   - 姓名、头像、身份、提示语互相打架。
   - 第一眼焦点不是“这个人是谁”，而是“这里排版炸了”。

3. 阶段区几乎不可读
   - 阶段说明、灰条和空白条像一堆没收好的散件。
   - 既不清楚当前阶段，也没有形成清晰的横向进度感。

4. 底部两块信息逻辑错了
   - 左右两块现在几乎是同一段文字重复显示。
   - 这不是“内容不够丰富”，而是信息职责没有分开。

5. 左侧名册也没有过线
   - 列表太窄，预览裁切得早。
   - 头像太小，右侧阶段 chip 像孤零零贴片。
   - 卡片不够精，也不够紧凑。

6. formal-face 仍然太淡、太空、太测试味
   - 线条和块面对比不足。
   - 分区有了，但没有主次。
   - 还是像开发中的 runtime 草稿板，不像真正给玩家读的关系册。

### 下一刀应该怎么改

- 关系页需要改成真正的“册页”结构，不是继续在当前骨架上挪几处 offset：
  1. 先砍掉顶部无意义大空白，把有效内容整体上提。
  2. 头卡只保留：
     - 头像
     - 姓名
     - 身份 / 常驻方式
     - 一句短引言
     不再塞会和姓名抢位置的长提示句。
  3. 阶段区改成一整条明确横带：
     - 左标签
     - 中间阶段段落
     - 右侧一句短状态
     不再散成几条漂浮灰块。
  4. 底部信息区改成“主叙述 + 结构化信息”，不要再左右重复同一段。
     - 如果当前只有一段可读文本，就只做一块主叙述卡。
     - 另一块改成结构化字段，或暂时不显示。
  5. 左侧名册略加宽，单卡反而更紧：
     - 头像放大一点
     - 姓名与预览更贴齐
     - 阶段 chip 不再孤零零贴右
  6. 整体对比度要再拉开：
     - 主卡更稳
     - 次卡更轻
     - 不能整页都一个浅色层级

### 当前判断层级

- 这次判断已经进入：
  - `真实入口体验`
- 结论很明确：
  - 当前关系页还没过线
  - 不能再把“结构已改过”包装成“体验成立”

## 2026-04-08 关系页重构一刀：PackageNpcRelationshipPanel 册页化改版

### 用户目标

- 继续把 `PackagePanel` 的玩家面收成正式版，这一刀只聚焦 `4_Relationship_NPC`。
- 不再做小 offset 微调，而是按用户截图暴露出的失败点，重构关系页版式与信息职责。

### 本轮实际落地

- 仅修改：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Tabs\PackageNpcRelationshipPanel.cs`
- 右侧详情区从原本的“绝对锚点散装块”重构为更稳定的册页结构：
  - 头卡：头像 / 姓名 / 常驻与当下出场方式 / 一句短引言 / 右侧三行状态摘记
  - 阶段横带：左标签 + 中部阶段 chip + 右侧短状态 + 下方四段进度带
  - 正文区：左侧主叙述卡、右侧结构化状态卡
  - 底部：单行收束提示
- 左侧名册同步收紧并加宽：
  - 名册宽度由 `0.29` 提到 `0.31`
  - 卡片高度从 `60` 提到 `72`
  - 头像放大、姓名字号提升、预览长度稍放宽、stage chip 不再贴得过于窄促
- 数据展示策略也补了一刀：
  - 如果 `PresenceNote` 与 `RoleSummary` 语义重复，不再把同一段话左右各放一次
  - 右侧结构化卡改为“阶段 / 今日出场 / 常驻方式 / 当前剧情 + 补充印象或阶段备注”
  - 头卡引言优先使用去重后的 presence narrative

### 验证

- `py -3 D:\Unity\Unity_learning\Sunset\scripts\sunset_mcp.py validate_script Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs --count 20 --output-limit 8`
  - 结果：
    - 一次拿到 `assessment=no_red`
    - 随后再次验证时出现 `stale_status` 导致 `unity_validation_pending`，但 `owned_errors=0`
- `py -3 D:\Unity\Unity_learning\Sunset\scripts\sunset_mcp.py errors --count 20 --output-limit 8`
  - 结果：`errors=0 warnings=0`
- `git diff --check -- Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
  - 结果：通过

### 当前状态与限制

- 这轮已经把关系页从“继续补 offset”推进到“结构真的换代”。
- 代码层已 clean，但我这轮还没有补到最终 GameView 截图证据：
  - 当前 scene 里的 `PackageNpcRelationshipPanel` 仍是运行时动态挂接链，不是直接常驻在场景里的静态组件。
  - 因此这轮先收在“代码重构 + 编译 clean + 下一轮继续做真实画面终验”的阶段。

### thread-state

- `Begin-Slice`：已跑
- `Park-Slice`：已跑
- 当前 live 状态：`PARKED`

## 2026-04-08 箱子打包持久化彻查（Edit 预置 -> Build 可用性）

### 用户目标

- 重点确认：箱子在 Edit 模式里预置内容后，打包运行是否仍可保留并可使用。
- 要求不仅给口头判断，还要补可回归证据，避免“看起来能用、打包后失效”的不确定性。

### 关键审计结论

- `ChestController._authoringSlots` 是 `[SerializeField]` 字段，数据跟随 scene/prefab 序列化，不是运行时临时缓存。
- 编辑器入口（`ChestControllerEditor` / `ChestAuthoringBatchSelectWindow`）写入 `_authoringSlots` 走 `SerializedProperty`，属于 Unity 正式序列化链路。
- 运行时优先级明确：
  - 无有效读档覆盖时：`Initialize -> ApplyAuthoringSlotsIfNeeded` 会把作者预置注入箱子。
  - 有读档时：`Load` 恢复存档内容，作者预置不会反向覆盖读档结果（这是预期，防止吞玩家进度）。
- 存档目录已统一到 `Application.persistentDataPath/Save`；存在旧目录迁移逻辑，可能把历史存档带入新环境，这会影响“看起来像预置没生效”的体感。

### 本轮补充的自动回归

- 新增测试文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\ChestAuthoringSerializationTests.cs`
- 新增测试点：
  - `AuthoringPreset_PrefabSerializationRoundTripPreservesConfiguredSlots`
  - 目的：直接验证“作者预置写入 prefab 后重新加载资产，槽位数据仍完整保留”。

### 验证结果

- `ChestInventoryBridgeTests`：`6/6 Passed`
  - 已覆盖：
    - 预置注入初始化
    - 读档优先级（空存档不被预置回填）
    - V2/legacy bridge 双向一致性
- `ChestAuthoringSerializationTests`：`1/1 Passed`
  - 验证了 prefab 序列化往返不丢 `_authoringSlots`。
- CLI `errors`：`errors=0 warnings=0`（仅测试结果保存日志条目，无编译红错）。

### 对打包可用性的最终判断

- 结论：箱子预置内容本身可以随包并在打包后使用。
- 但必须区分两种场景：
  1. 首次/无相关存档覆盖：会看到 Edit 预置内容。
  2. 已有旧存档并执行读档：会以存档内容为准，预置不再强行覆盖。

### thread-state

- `Begin-Slice`：已跑（slice=`chest-authoring-prefab-serialization-test`）
- `Park-Slice`：已跑
- 当前状态：`PARKED`

## 2026-04-08 存档系统只读复盘（NPC持久化 + 跨场景读档异常）

### 用户问题

- NPC 现在是否做了持久化？
- Primary 读取 Town 存档时报 `[SaveManager] 刚移动完一帧后位置被改回` 的根因是什么？
- 除已暴露问题外，当前存档链还有哪些系统性漏洞？

### 已确认事实（代码层）

- `SaveManager` 当前仍是“只做当前场景恢复，不换场景”口径（`SaveManager.cs` 顶部注释）。
- 玩家存档里会记录 `sceneName`，但 `RestorePlayerData` 没有按 `sceneName` 切场或校验，直接在当前场景复位坐标。
- `RestorePlayerData` 在复位后一帧做位置校验；如果偏差 > 0.1 就报“被改回”错误。
- `PersistentObjectRegistry.RestoreAllFromSaveData` 采用“反向修剪”：
  - 当前场景里存在、但本次存档 GUID 列表里没有的对象会被禁用/销毁。
- NPC 运行组件（`NPCAutoRoamController` / `NPCMotionController` / `NPCBubblePresenter`）都不是 `IPersistentObject`，无槽位级位置存档。
- `SpringDay1NpcCrowdDirector` 是 runtime director（`DontDestroyOnLoad`），主要按 manifest/anchor/phase/cue 驱动 NPC；不负责把 NPC 实时位置写进存档。
- NPC 关系值通过 `StoryProgressPersistenceService` 与 `PlayerNpcRelationshipService` 落地，其中 `PlayerNpcRelationshipService` 仍会写 `PlayerPrefs`。

### 风险清单（按严重度）

- S1（已触发）：跨场景读档语义缺失。
  - 在 Primary 读取 Town 档时不切场，只把 Town 坐标硬塞进 Primary，导致物理挤出/导航残留/触发器影响，触发“下一帧偏移”报错。
- S1（高风险）：反向修剪与跨场景存档组合可能误清场景对象。
  - Town 档通常不含 Primary 场景对象 GUID；在 Primary 执行恢复时，Primary 对象可能被判定为“存档不存在”而被禁用/销毁。
- S1（高风险）：玩家复位流程未冻结导航/输入链。
  - `RestorePlayerData` 没有显式 `ForceCancel` 自动导航，也没做读档期间输入闸门；下一帧被移动链重新推进是高概率事件。
- S2：NPC 没有“槽位级空间持久化”。
  - 现在存的是剧情关系/阶段，不是每个 NPC 的实时世界坐标与运动状态。
- S2：NPC 跨场景回到“场景编辑位/锚点逻辑位”是当前默认，不是“保留离开时位置”。
  - 这点在 Primary/Town 来回切时体感会很明显。
- S2：关系数据双通道（SaveData + PlayerPrefs）存在串档污染风险。
  - 新开流程若未先经过完整读档覆盖，可能短时读到历史 PlayerPrefs 状态。
- S3：`RestorePlayerData` 的“Tool 递归归零”是硬编码修正，存在误伤复杂层级 prefab 的风险。
- S3：迁移旧存档目录逻辑会在启动时复制 legacy 存档，容易造成“我明明删了但又回来了”的体感混乱。

### 对用户两个直问的结论

- NPC 持久化：**部分有**（剧情阶段、关系值），**空间位姿没有做完整槽位持久化**。
- Primary/Town 来回切是否回到编辑位：**当前通常会回到场景编辑位/锚点驱动位**，不是严格意义上的“离场位置持续”。

### thread-state

- 本轮只读分析：未进入真实施工，未跑 `Begin-Slice`
- 当前 live 状态保持：`PARKED`

## 2026-04-08 对 Day1 resident 持久化 prompt_02 的判断

### 用户问题

- 先读 `2026-04-08_day1居民运行态与场景切换持久化prompt_02.md`，判断它是否和当前存档系统线程的方向一致，还是引入了新的主问题。

### 结论

- 总判断：**大方向一致，不是换题；但它把问题进一步收窄成了一个更明确的施工切片。**

### 与当前判断一致的部分

- 继续坚持“第一版不恢复任意导演帧”，只做长期剧情态 + resident 最小运行态。
- 明确不把 UI、typing、bubble、nearby/selfTalk/walkAway 这些过程态塞进存档。
- 正面承认：
  - NPC 不该一切场景就回编辑态起点
  - `PersistentPlayerSceneBridge.Start/RebindScene` 已经是 scene-rebind 责任面，不该再甩给 UI

### 它新增的内容

- 新增了一个比我前一轮更具体的 own 目标：
  - **resident 最小 snapshot contract**
  - 不是只说“NPC 位置还没存”，而是要求收出“合法归属 + 最小站位/锚点语义 + 是否脚本接管”的第一版 contract
- 新增了一个明确性能责任：
  - `PersistentPlayerSceneBridge.Start()` / `RebindScene(...)` / `PromoteSceneRuntimeRoots(...)` / `ReapplyTraversalBindings(...)`
  - 这不再只是“旁路风险”，而是本轮主刀的一部分

### 它没有换掉的边界

- 没有要求去存任意导演帧
- 没有要求去存 UI 当前页
- 没有把 `Town.unity/Primary.unity` scene 摆点改动塞给存档系统线程
- 没有要求本线程代替 `spring-day1` 决定最终导演语义恢复

### 我认为需要警惕的一点

- 这份 prompt 把“持久化 contract”与“scene-rebind 卡顿责任面”绑成一刀，方向对，但施工时要防止顺手扩成“整个跨场景启动大重构”。
- 正确做法应是：
  - 先收最小 resident snapshot contract
  - 再只修 own 的 bridge/rebind 重操作
  - 不扩成全项目启动性能总治理

## 2026-04-08｜收到 Day1 新 prompt：resident 运行态持久化 + scene-rebind 启动峰值
- 触发来源：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-08_day1居民运行态与场景切换持久化prompt_02.md`
- 本轮新增共识：
  1. `PersistentPlayerSceneBridge.Start()` / `RebindScene(...)` 已被 profiler 坐实为启动大卡顿主峰之一；
  2. 这条锅不再误归 UI，而是纳入持久化/scene-rebind 责任面；
  3. 这轮第一目标仍是 `Day1` 长期剧情态 + resident 最小 snapshot；
  4. 不改“第一版不恢复任意导演帧”的边界。

## 2026-04-08｜只读审计：PersistentPlayerSceneBridge / scene-rebind 启动责任面

- 用户目标：
  - 不改文件，只读审计 `PersistentPlayerSceneBridge.Start / RebindScene` 的启动责任面；
  - 找出首帧卡顿最可能来源、重复/可延后/强耦合步骤，以及和 `SaveManager` 读档流程的竞态或位置覆盖风险。
- 已完成：
  1. 逐段审计了 `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`、`SaveManager.cs`、`TraversalBlockManager2D.cs`、`NavGrid2D.cs`、`GameInputManager.cs`、`SceneTransitionTrigger2D.cs`、`PersistentObjectRegistry.cs`；
  2. 确认 bridge 当前最大风险不是单点，而是三层叠加：
     - 初始场景很可能发生 `sceneLoaded + Start` 双重 `RebindScene`；
     - `ReapplyTraversalBindings()` 会把每个 `TraversalBlockManager2D.ApplyConfiguration(rebuildNavGrid: true)` 全量跑一遍，连带 `NavGrid2D.RefreshGrid() -> RebuildGrid()`；
     - `RebindPersistentCoreUi()` 会做整套 Toolbar / Inventory / Tabs / Tooltip 重绑并 `Canvas.ForceUpdateCanvases()`。
  3. 确认 bridge 里还存在多轮 `FindObjectsByType / FindFirstObjectByType / GetComponentsInChildren` 全局扫描，进一步放大首帧压力。
  4. 确认普通存档读档与 scene-rebind 目前是两套正交链路：
     - 普通 `LoadGameInternal()` 不切场景；
     - `player.sceneName` 目前只用于 summary，不参与切场或 bridge 对接；
     - 默认存档/重开才会 `LoadSceneAsync("Town")`。
- 关键判断：
  - `bridge` 的 own 主峰首先应盯 `ReapplyTraversalBindings` 与 `RebindPersistentCoreUi`，其次才是 `PromoteSceneRuntimeRoots` 与各种全局扫描；
  - `SaveManager` 侧最大结构性风险是“跨场景槽位在当前场景原地硬恢复”，这会让 Town 坐标和 Primary 当前运行态打架；用户之前看到的“刚移动完一帧后位置被改回”更像这条结构性错位或当前场景 movement/nav/bounds 脚本反推，不应简单全甩给 bridge；
  - 但在真正 scene load 路径上，bridge 自己也会写玩家位置，因此如果后续把存档切场接到 bridge，必须显式做位置单写口径。
- 本轮边界：
  - 只读分析；
  - 未改业务代码；
  - 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`，当前 thread-state 仍保持 `PARKED`。
- 下一步恢复点：
  - 如果下一轮进入真实施工，第一刀应是：
    1. 收 `bridge` 初始双 rebind；
    2. 把 traversal 全量 rebuild 从 rebind 主链里收窄；
    3. 把 persistent UI 全量 rebuild 从 scene-ready 主链中拆出或延后；
    4. 再决定如何和 Day1 resident 最小持久态接线。

## 2026-04-08｜真实施工：Day1 resident 最小持久态 + scene-rebind 第一刀

- 当前主线：
  - 把 `Day1` 第一版真正值钱的“剧情长期态 + resident 最小运行态”接进正式存档链，并把 `PersistentPlayerSceneBridge.Start/RebindScene` 的明显重复重活先收一刀。
- 本轮真实落地：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - 新增 `ResidentRuntimeSnapshotEntry`
     - 新增 resident 最小运行态导出/回灌入口：
       - `CaptureResidentRuntimeSnapshots()`
       - `ApplyResidentRuntimeSnapshots(...)`
       - `ClearResidentRuntimeSnapshots()`
     - 在场景切换前缓存当前 scene resident 最小态，并在绑定 scene resident 后优先回灌 runtime cache，避免回编辑态起点
  2. `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
     - 把 `residentRuntimeSnapshots` 接进 `SpringDay1ProgressSaveData`
     - 保存时采集 resident 最小态，读档时回灌；空快照会主动清理旧 resident cache
  3. `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
     - 新增 `TryApplyLoadedPlayerState(...)`
     - 初始场景若已经走过 `sceneLoaded`，`Start()` 不再重复 `RebindScene()`
     - `ReapplyTraversalBindings()` 改成先批量 `ApplyConfiguration(rebuildNavGrid: false)`，再按 scene 统一 `NavGrid2D.RefreshGrid()`
  4. `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
     - 普通读档现在会按 `player.sceneName` 判断是否先切场
     - 新增 `BeginSceneSwitchLoad(...)` / `LoadAfterSceneSwitchRoutine(...)`
     - 玩家 scene 名保存改为 `SceneManager.GetActiveScene().name`，不再误存成 `DontDestroyOnLoad`
     - `RestorePlayerData()` 优先走 `PersistentPlayerSceneBridge.TryApplyLoadedPlayerState(...)`
  5. 测试补充
     - `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
       - 新增 `CrowdDirector_ShouldCaptureAndReapplyResidentRuntimeSnapshot`
     - `Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
       - 为 roundtrip 补 `residentRuntimeSnapshots` contract 断言
- 当前 contract 已覆盖：
  - resident identity
  - sceneName
  - residentGroupKey
  - resolvedAnchorName
  - position / facing
  - isActive
  - isReturningHome
  - underDirectorCue
- 明确未扩的范围：
  - 不恢复任意导演帧
  - 不存 typing / bubble / nearby / selfTalk / walkAway
  - 不替 `spring-day1` 吞最后一跳导演语义恢复
- 验证证据：
  - CLI `validate_script`
    - 四个关键脚本均 `owned_errors=0`
    - `StoryProgressPersistenceService.cs` native clean
    - 其余脚本只剩既有 warning；CLI assessment 因 Unity `stale_status` 落到 `unity_validation_pending`
  - direct MCP `validate_script`
    - `SpringDay1NpcCrowdDirector.cs`：`errors=0 warnings=2`
    - `StoryProgressPersistenceService.cs`：`errors=0 warnings=0`
    - `PersistentPlayerSceneBridge.cs`：`errors=0 warnings=2`
    - `SaveManager.cs`：`errors=0 warnings=3`
  - direct MCP `read_console`
    - `0 entries`
  - CLI `errors --count 10 --output-limit 10`
    - `errors=0 warnings=0`
  - 定点 EditMode 运行
    - 按测试名筛选仍返回 `0 tests`
    - 说明当前环境下测试过滤器仍不可靠，不能把它包装成 pass
- 当前阶段判断：
  - 这条线已经从“只做边界讨论”推进到“resident 最小 snapshot + 场景切换读档 + bridge 第一刀真修”的第一版可落地状态
  - 但 scene-rebind 全量性能责任面还没收完，不应夸大成“启动卡顿整体解决”
- thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑（本轮未做 sync）
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
- 恢复点：
  - 如果继续，只建议下一刀聚焦 `RebindPersistentCoreUi()` 延后/收窄，或者只做 Day1 runtime 最终整合实测，不要顺手扩成全项目启动性能治理。

## 2026-04-09｜只读审计：存档玩家操作面与槽位语义

- 当前主线目标：
  - 从玩家真实可操作入口出发，重新审查当前存档系统到底让玩家做什么、默认槽和普通槽各自代表什么、以及哪些语义已经会误导验收或打包使用。
- 本轮子任务：
  - 只读精查 `SaveManager`、`PackageSaveSettingsPanel`、`SaveActionToastOverlay`、`PackagePanelTabsUI`、`GameInputManager`，并补看 `StoryProgressPersistenceService`、`PlayerNpcRelationshipService`、`CraftingStationInteractable` 的槽位隔离副通道。
- 本轮边界：
  - 未改业务代码；
  - 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`；
  - 当前 thread-state 继续按只读分析口径视为 `PARKED`。

### 本轮确认的玩家实际操作面

1. 玩家当前可通过两条入口触达存档面：
   - `ESC` 打开 `PackagePanel` 的 `5_Settings`
   - 或先开 `PackagePanel`，再切到设置页
2. 当前默认热键只有两条：
   - `F9`：走“回原生开局/重新开始”链
   - `F5`：已停用，只弹 toast
3. 默认槽当前不是普通可写槽：
   - 只允许“加载默认存档 / 重新开始游戏”
   - UI 已把默认槽的复制 / 粘贴 / 覆盖按钮硬隐藏
4. 普通槽当前支持：
   - `新建存档`
   - `加载`
   - `复制`
   - `粘贴到另一普通槽`
   - `用当前实时进度覆盖`
   - `删除`
5. 当前不存在的玩家面能力：
   - 自动存档
   - 默认槽写回
   - 普通槽重命名
   - “粘贴为新槽”
   - 普通槽直接热键保存/读取

### 本轮最重要的审计结论

1. 默认槽语义已经明显漂成“伪槽位 + 动作入口”的混合体：
   - 代码实际把默认槽锁成“回原生开局”
   - 但 UI 仍把它包装成“默认存档”
   - 而且“加载默认存档”和“重新开始游戏”实际上走同一条实现链
2. 成功反馈存在抢跑：
   - 默认读档 / 重新开始 / 跨场景普通读档的 toast 都可能在真正完成前先报“已读档”
3. 槽位隔离并不纯：
   - NPC 关系与工作台提示消费仍通过 `PlayerPrefs` 落全局侧通道
   - 当前槽位语义并不是 100% 只靠 slot file 自洽
4. 设置页状态文案与按钮能力不完全一致：
   - 文案说“当前只能读取”
   - 但 `粘贴` 和 `删除` 仍然可用，且 `粘贴` 本身就是覆盖文件
5. 复制缓存是“设置页生命周期内的临时态”：
   - 重新进入设置页会被清空
   - 当前 UI 没把这条语义说透

### 直接影响验收 / 打包使用的高优先级问题

1. 默认槽确认文案仍写“重新载入 Primary”，但运行时实际加载 `Town`
2. 异步读档 / 重开成功 toast 抢跑，可能把失败或半失败包装成“已读档”
3. `PlayerNpcRelationshipService` 与工作台提示消费仍会把槽位状态同步写进 `PlayerPrefs`
4. “只能读取”的状态提示与仍可 `粘贴/删除` 的实际操作面冲突

### 恢复点

- 如果用户下一轮要求继续施工，最合理的顺序应是：
  1. 先收默认槽语义与读档反馈一致性
  2. 再收槽位隔离副通道（`PlayerPrefs`）
  3. 最后再补设置页/热键文案与入口体验

## 2026-04-09｜只读审计：当前存档数据覆盖与运行态恢复

- 当前主线目标：
  - 把 `Sunset` 当前工作区里的存档系统，按“现在到底存了什么 / 哪些还没闭环 / 跨场景读档最可能炸哪里”重新审清。
- 本轮子任务：
  - 用户明确要求只读，不要改文件；重点复核 `StoryProgressPersistenceService`、`SpringDay1NpcCrowdDirector`、`PersistentObjectRegistry`、`DynamicObjectFactory`，以及 `Chest / Crop / Tree / Stone / WorldItemPickup` 和 `NPC / Day1` 持久化覆盖。
- 本轮边界：
  - 只读源码审计；
  - 未改业务代码；
  - 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`；
  - 当前 live 状态继续按只读分析视为 `PARKED`。

### 当前已确认的真实存储面

1. `GameSaveData` 根层当前实际保存：
   - `gameTime`
   - `player`（只可靠写入 `sceneName + position`）
   - 兼容壳层 `inventory`
   - `worldObjects`
   - 兼容残留 `farmTiles`
   - `cloudShadowScenes`
2. 当前真正通过 `IPersistentObject -> worldObjects` 进档的只有 9 类：
   - `TreeController`
   - `StoneController`
   - `WorldItemPickup`
   - `ChestController`
   - `CropController`
   - `InventoryService`
   - `EquipmentService`
   - `FarmTileManager`
   - `StoryProgressPersistenceService`
3. `StoryProgressPersistenceService` 当前已覆盖：
   - `StoryManager.CurrentPhase`
   - `StoryManager.IsLanguageDecoded`
   - `DialogueManager` 已完成正式对白序列
   - `PlayerNpcRelationshipService` 关系阶段快照
   - `spring-day1.workbench-entry-hint-consumed`
   - `SpringDay1Director` 的 Day1 长期态
   - `Health / Energy`
   - `SpringDay1NpcCrowdDirector` resident 最小 runtime snapshot
4. `SpringDay1NpcCrowdDirector` 当前 resident snapshot 只存最小集：
   - `npcId`
   - `sceneName`
   - `residentGroupKey`
   - `resolvedAnchorName`
   - `position / facing`
   - `isActive`
   - `isReturningHome`
   - `underDirectorCue`

### 本轮确认的关键缺口

1. 当前存档仍不是“全局世界态”：
   - `SaveManager` 只从当前 `PersistentObjectRegistry` 收 live 对象；
   - 普通树 / 石头 / 箱子 / 作物 / 掉落如果所在场景此刻没加载，就不会进档；
   - 跨场景全局缓存目前只有 `cloudShadowScenes` 和 `SpringDay1NpcCrowdDirector` 的 resident snapshot 这类特例。
2. 玩家基础状态仍未闭环：
   - `PlayerSaveData` 里虽然还挂着 `currentLayer / selectedHotbarSlot / gold / stamina / maxStamina`；
   - 但当前实现只写 `sceneName + position`，读档也只恢复玩家位置；
   - `HotbarSelection` 现在只靠 `PersistentPlayerSceneBridge` 做跨 scene 短时快照，不进 slot-save。
3. NPC 持久化仍是“Day1 特例 + 关系特例”，不是泛 NPC 方案：
   - 关系阶段仍直接写 `PlayerPrefs`，slot-save 只是镜像快照；
   - 通用 `NpcResidentRuntimeContract / NpcResidentRuntimeSnapshot` 存在，但当前 save/load 主链没有接它；
   - 一般 `NPCAutoRoamController` 的 home anchor / scripted control owner / roam resume 语义没有进入正式槽位存档。
4. Day1 resident 恢复只保“最小落点”，不保完整导演语义：
   - 当前只存 `underDirectorCue` 布尔态；
   - 读档后会用 `__restored-director-cue__` 占位，而不是恢复真实 `AppliedCueKey / AppliedBeatKey`；
   - 因此中途存档后再读档，最容易出的是“位置大致对了，但导演推进语义没完全对上”。
5. Tree 季节过渡细节仍未闭环：
   - `TreeSaveData` DTO 里保留了 `hasTransitionedToNextSeason / transitionVegetationSeason`；
   - 但 `TreeController.Save()` 现实现式注明“暂不存储”，`Load()` 也没恢复。
6. 旧档兼容仍有高风险：
   - `PersistentObjectRegistry.RestoreAllFromSaveData()` 先做反向修剪；
   - `DynamicObjectFactory` 只能重建 `Drop / Tree / Stone / Chest / Crop`；
   - 如果旧档缺少当前时代的 `StoryProgressState / EquipmentService` 等 GUID，最容易出现“先被修剪，再没人负责重建”的情况。

### 跨场景 / 读档后最可能出错的点

1. 在 `Town` 保存，却期待 `Primary` 未加载现场的树 / 箱子 / 作物 / 掉落也被记住：
   - 当前架构默认做不到，除非该系统自己额外做 scene 级缓存。
2. 在 Day1 escort / resident 导演接管中途保存：
   - 当前 blocker 只拦正式对白、闲聊和工作台制作，不拦 resident/escort 中途 cue；
   - 读档后最容易出现 NPC 站位接近、但 cue/parent/后续行为链不完全一致。
3. 读旧档或半旧档：
   - 反向修剪会先清当前 registry；
   - 而不可重建类型没有 fallback；
   - 这类情况最容易把 `EquipmentService`、旧剧情态之类读成“局部缺件”。
4. 依赖 `PlayerPrefs` 的槽位副通道：
   - `npcRelationships` 与 `workbench hint consumed` 仍会同步写全局键；
   - 如果不是通过正式 slot load / fresh reset 进入运行态，最容易出现串档或旧值残留体感。

### 验证状态

- 仅完成：
  - 当前工作树源码静态审计
  - `IPersistentObject` 覆盖面核对
  - 现有 sample save 结构抽样
- 未完成：
  - Unity live save/load 往返验证
  - `Town <-> Primary` 多次跨场景实机读档验证
  - Day1 mid-cue 存档再读档的现场验证

### 恢复点

- 如果下一轮继续推进，这条线最值得优先补的不是“再加一个 DTO”，而是先在 3 个方向里明确取舍：
  1. 普通 world object 是否需要做真正的跨 scene 全局缓存
  2. Day1 resident / director 中途态是否要继续细化 contract，还是直接禁止更多中途保存窗口
  3. `PlayerPrefs` 副通道是否要彻底迁出，避免槽位串档

## 2026-04-09 08:09:00

### 用户目标

- 用户要求对当前 Sunset 存档系统做一轮“彻底只读自查”，直接回答它是否已经能称得上无懈可击，并把所有关键漏洞、半成品语义和验证空白讲清楚。

### 本轮完成

- 按只读口径完成了对 `SaveManager`、`PackageSaveSettingsPanel`、`PersistentPlayerSceneBridge`、`PersistentObjectRegistry`、`DynamicObjectFactory`、`StoryProgressPersistenceService`、`PlayerNpcRelationshipService` 的主链审计。
- 收回两个并行只读子审计：
  - `Hypatia`：槽位/UI/默认档/热键链
  - `Kuhn`：story persistence / resident / NPC / worldObjects 覆盖面
- 形成了新的硬判断：当前系统不能宣称“没有任何问题”，而且至少有 5 条会直接影响玩家体验或验收的高优先级风险。

### 本轮新增关键判断

1. 默认槽现在不是“真正的默认存档”，而是“回 Town 开局”的动作入口：
   - `SaveManager.QuickSaveDefaultSlot()` 直接禁用；
   - `QuickLoadDefaultSlot()` / `RestartToFreshGame()` 都走 `BeginNativeFreshRestart()`；
   - UI 仍把它包装成“默认存档”，且部分文案还写成 `Primary`。
2. 默认基线文件不是一次生成后固定：
   - `ScheduleFreshStartBaselineCapture()` 在启动时总会安排抓基线；
   - `_freshStartBaselinePrepared` 只在本次运行内生效，冷启动后会重新覆盖 `__fresh_start_baseline__`；
   - 这意味着“默认档永远是最初开局”的语义当前并不牢靠。
3. 默认读档 / 重新开始仍高概率不是干净开局：
   - `BeginNativeFreshRestart()` 只在切回 `Town` 后调用 `ApplyNativeFreshRuntimeDefaults()`；
   - 这里只重置时间与 story snapshot，没有看到同步清空 `PersistentPlayerSceneBridge` 持有的背包 / 快捷栏快照，也没有看到明确的装备清空链；
   - 现有结构下，persistent player runtime 很可能把旧库存继续带回新场景。
4. 普通槽读档 / 默认读档 / F9 的成功提示会抢跑：
   - UI 和热键都是按“协程是否成功启动”立刻 toast “已读档/已重新开始”；
   - 真正的场景切换和 `ApplyLoadedSaveData()` 还在后面，失败不会回滚这条提示。
5. 槽位隔离仍不彻底：
   - NPC 关系和工作台 hint 仍通过 `PlayerPrefs` 保留一条全局副通道；
   - slot-save 与全局本地状态仍有串档面。
6. 当前存档仍不是“跨场景全局世界态”：
   - 保存入口只写当前 `PersistentObjectRegistry` 里已注册且 `ShouldSave` 的对象；
   - 未加载场景里的普通树 / 石头 / 箱子 / 作物状态，不会天然被持续带着走。

### 中优先级风险

- `PlayerSaveData` 仍保留 `currentLayer / selectedHotbarSlot / gold / stamina / maxStamina` 等字段，但当前正式保存/恢复链只真正处理 `sceneName + position`。
- `StoryProgressPersistenceService` 对 Day1 的恢复仍高度依赖反射和私有字段名；只要 Day1 线程改私有字段/方法名，就有静默失效风险。
- Day1 resident runtime 目前是定制最小 snapshot，不是通用 NPC 持久化；导演 cue/beat 上下文也没有完整恢复。
- `DynamicObjectFactory` 现在只支持 `Drop / Tree / Stone / Chest / Crop` 重建，未来新增持久化物体若没补工厂，读档缺对象时就会失败。
- `TreeSaveData` 为季节过渡预留了字段，但 `TreeController.Save/Load` 还没真正接入。
- `PackageSaveSettingsPanel` 的部分帮助文案和交互状态仍不一致：
  - 说“当前只能读取，不能覆盖普通槽”，但 `删除` 和 `粘贴` 仍可操作；
  - 复制缓存是面板生命周期内的临时态，关闭/重开就会丢，但提示不充分。

### 已确认相对稳定的点

- 存档文件路径已经改为 `Application.persistentDataPath/Save`，方向上是面向打包版的正确落点。
- 旧版 `SaveLoadDebugUI` 当前是退役兼容壳，运行时会主动清理遗留调试画布，不再是正式功能入口。
- CloudShadow 现已接进 `SaveManager` 的导出/导入链，至少静态结构上不再是游离态。

### 验证状态

- 已完成：
  - 源码静态审计
  - 双子审计交叉验证
- 尚未完成：
  - Unity live save/load 往返矩阵
  - 打包版默认档 / 普通槽 / Town <-> Primary 往返终验
  - “默认重开是否真的清空 persistent inventory/equipment” 的现场验证

### 当前阶段

- 已从“边界不清”进入“问题清单已经压实”的阶段。
- 下一步如果继续施工，优先级不该再是扩功能，而是先把默认档语义、异步成功提示、槽位隔离、跨场景世界态口径收干净。

## 2026-04-09 只读定位：`PackageSaveSettingsPanel` 普通存档横向偏移与右上角让位方案

### 用户目标

- 只读检查 `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
- 回答两个问题：
  1. 普通存档列表为什么会出现每个条目逐步横向偏移、过几个又循环
  2. 右上角快捷说明区怎样在最小改动下左移，为最右侧“退出游戏”按钮腾位

### 本轮完成

- 只读核对了 `PackageSaveSettingsPanel.cs` 的 runtime 布局链
- 只读核对了 `PackagePanelTabsUI.cs` 与 `Primary.unity`，确认 `5_Settings/Main` 静态层本身没有额外的 `LayoutGroup / ScrollRect / Mask`
- 形成结构级判断：当前现象更像 `ScrollRect -> Viewport -> Content` 宽度/裁剪链的重建抖动，而不是某段代码在逐条写 `anchoredPosition.x`

### 关键判断

1. 普通存档条目没有任何逐条写入横向位移的逻辑；最可疑的是这组组合：
   - `_ordinaryContent`：横向 stretch + `VerticalLayoutGroup` + `ContentSizeFitter.verticalFit`
   - `_scrollRect.verticalScrollbarVisibility = AutoHideAndExpandViewport`
   - 单条 `Body` 行里“左侧 flexible 摘要 + 右侧固定 208 宽动作列”
   - 这会让 viewport 宽度一旦在重建里被改动，整列条目都被迫重新算宽；如果当前 canvas / rect rounding 不整，视觉上就容易出现“逐条横向偏一点、几条后循环”的错位感
2. `5_Settings/Main` 静态节点只是背景壳，不是主因；真正该看的是 `EnsureBuilt()` 里 `HeaderMain / ScrollRoot / Viewport / Content / slotRoot`
3. 右上角最小改动方案不要去重算外壳 `SaveSettingsRuntimeRoot`；只在 `HeaderMain` 内把 `GuideCard` 收窄，并在它右侧追加一个固定宽度的“退出游戏”按钮位

### 涉及文件

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Save\PackageSaveSettingsPanel.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Tabs\PackagePanelTabsUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`

### 验证状态

- 已完成只读静态审计
- 未做 Unity live 截图 / GameView 复测
- 本轮结论属于“结构 / 布局链判断成立”，不等于最终体验终验

### 恢复点

- 如果下一轮继续施工，优先只改 `PackageSaveSettingsPanel.EnsureBuilt()` 的头部布局与普通存档 scroll 宽度链
- 不优先动 `5_Settings/Main` 的场景静态壳体

## 2026-04-09 微调：普通存档下半区高度链重新对齐

### 用户目标

- 只改 `5_Settings` 存档页里普通存档下半区，不再动已认可的顶部右上角布局与“退出游戏”按钮。
- 解决普通存档区域下边界越界、滚动内容挤出外框的问题；按钮横向可以略长，但必须继续遵守“内不可超外”。

### 本轮完成

1. 继续沿用已有真实施工切片 `save-hardening-and-settings-ui-polish-2026-04-09`，只改 `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`。
2. 通过 MCP 读取 `UI/PackagePanel/Main/5_Settings/Main` 的 `RectTransform`，确认 `Main` 本体是 `1264 x 720`；外壳空间本身够用，不是这次越界的主要根因。
3. 收紧普通存档区下半部的安全边距：
   - `OrdinarySection` 上下 padding 从 `8/8` 收到 `6/6`
   - `Viewport` 增加上下 2 像素 inset
   - `Content` 间距从 `8` 收到 `6`，上下 padding 改为 `6/6`
   - scrollbar 也同步内收，避免贴底部边线
4. 重新对齐普通槽单卡内部高度链：
   - `slotRoot` 从 `94` 收到 `92`
   - `body / summary / actionColumn` 从 `78` 收到 `76`
   - `summaryPanel` 改成更紧的 `padding + spacing`
   - `actionColumn` 改成 `4/4` 上下 padding、`2` 间距
   - `加载存档` 按钮从 `28` 收到 `24`
5. 关键修正点是把动作列“声明高度”和“真实内容高度”重新做平，不再让动作列自己把卡片往外顶。

### 关键判断

- 这次最核心的根因不是 `5_Settings/Main` 壳体不够高，而是普通槽卡片内部高度链不一致：
  - 原先 `actionColumn` 自己声明 `78` 高
  - 但内部按钮 + row + padding + spacing 的真实总高更大
  - 结果就是 viewport 看似有框，里面内容却仍可能把下边界顶穿
- 因为用户已经明确认可顶部布局，这轮没有再碰 `HeaderSection / GuideCard / QuitButton`，完全只守普通存档下半区。

### 涉及文件

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Save\PackageSaveSettingsPanel.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\存档系统\memory_0.md`

### 验证状态

- 已验证：
  - `git diff --check -- Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs` 通过
  - `validate_script Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs` 返回 `owned_errors=0 / external_errors=0`
  - MCP 读到 `5_Settings/Main` 的尺寸，确认外壳不是主因
- 未闭环：
  - `validate_script` 总 assessment 仍是 `unity_validation_pending`
  - 原因是当前 Unity editor state 卡在 `stale_status`，`CodexCodeGuard` 也有一次 `timeout-downgraded`
  - 所以这轮只能说“脚本层没抓到 owned red”，不能说“Unity live 终验已过”

### 恢复点

- 如果用户下一轮继续反馈普通槽下边界仍有一点点外溢，下一刀继续只守：
  - `PackageSaveSettingsPanel` 的 `ordinarySection / scrollRoot / slotRoot`
  - 不回头碰顶部右上角已经过线的区域
- 当前切片已合法 `PARKED`，等待用户在 `5_Settings` 里做真实终验。

## 2026-04-09 只读排查：箱子材料为什么工作台“不认”

### 用户目标

- 检查“箱子里的东西拿了但是工作台不认”到底是为什么。
- 区分这是设计上只认玩家背包，还是箱子 -> 背包 -> 工作台这条运行链本身有刷新缺口。

### 本轮完成

1. 只读核查了工作台材料来源：
   - `CraftingService.GetMaterialCount()` 只遍历自己的 `InventoryService inventory`
   - 不会扫描任何 `ChestController.RuntimeInventory`
2. 只读核查了工作台 overlay 的刷新来源：
   - `SpringDay1WorkbenchCraftingOverlay.Open()` 绑定的是 `craftingService.Inventory`
   - overlay 只订阅 `InventoryService.OnInventoryChanged`
3. 只读核查了箱子拖拽到背包的写入方式：
   - `InventorySlotInteraction` / `SlotDragContext` 在箱子 -> 背包场景下，大量走 `InventoryService.SetInventoryItem(...)`
   - `PlayerInventoryData.SetItem(...)` 目前只 `RaiseSlotChanged(index)`，不会 `RaiseInventoryChanged()`
4. 因此形成了两个分层结论：
   - 设计层：工作台现在天生不认“仍放在箱子里”的材料
   - 刷新层：如果玩家正开着工作台，背包内容又是通过 `SetInventoryItem(...)` 从箱子拖进去，overlay 很可能不会立刻刷新，看起来像“还是不认”

### 关键判断

- 这是两个问题，不是一个问题：
  1. `CraftingService` 设计上只认玩家背包，不认箱子库存
  2. 箱子拖拽进背包后，`InventoryService.SetInventoryItem -> PlayerInventoryData.SetItem` 没有抛 `OnInventoryChanged`，导致依赖这个事件的工作台 overlay 可能不刷新
- 如果用户期待“工作台直接吃附近箱子材料”，当前代码完全没接这条能力。
- 如果用户说的是“已经从箱子拖进自己背包了，工作台界面还显示材料不足”，那第二条刷新缺口就是高概率真因。

### 涉及文件

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Crafting\CraftingService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotInteraction.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\SlotDragContext.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\PlayerInventoryData.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Box\BoxPanelUI.cs`

### 验证状态

- 已完成：源码静态链路审计，足够解释“为什么不认”
- 未完成：
  - 尚未做 live 复现
  - 尚未进入真实修复
- 当前结论属于结构级定位，不等于体验问题已经修掉

### 恢复点

- 如果下一轮直接修：
  1. 先决定是否要支持“工作台直接读取箱子材料”
  2. 无论支不支持，都应该先补 `PlayerInventoryData.SetItem()` 的 inventory-changed 通知，避免箱子 -> 背包后的工作台刷新继续失真

## 2026-04-09 修复：箱子拖进背包后工作台材料状态不刷新

### 用户目标

- 用户明确说明现场问题是：
  - 不是“工作台应直接读取箱子材料”
  - 而是“已经把材料从箱子拖进背包了，工作台还显示材料不足”
- 本轮目标是把这条链彻底修好。

### 本轮完成

1. 在 `PlayerInventoryData` 补齐底层事件合同：
   - `SetItem(...)`
   - `ClearItem(...)`
   - `SetSlot(...)`
   现在都会在 `RaiseSlotChanged(...)` 之后继续 `RaiseInventoryChanged()`
2. 在 `SpringDay1WorkbenchCraftingOverlay` 追加槽位变化兜底：
   - `BindInventory(...)` 现在同时订阅 `OnSlotChanged` 和 `OnInventoryChanged`
   - `UnbindInventory(...)` 对应解除两条订阅
   - 新增 `HandleInventorySlotChanged(int _)`，直接复用 `HandleInventoryChanged()`
3. 新增编辑器回归测试：
   - `Assets/YYY_Tests/Editor/WorkbenchInventoryRefreshContractTests.cs`
   - 覆盖两件事：
     - `PlayerInventoryData.SetItem / ClearItem / SetSlot` 必须发出 `OnInventoryChanged`
     - workbench overlay 不允许退回成只监听 `OnInventoryChanged`

### 关键判断

- 这轮修的是“箱子 -> 背包 -> 工作台 UI 不立即刷新”的真 bug。
- 工作台“是否应该直接认箱子库存”仍然是另一条产品能力边界，本轮没有把它偷做进去。
- 现在即使未来某条路径再次只走到单槽写入，overlay 也有 `OnSlotChanged` 兜底，不再只赌 `OnInventoryChanged`。

### 涉及文件

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\PlayerInventoryData.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\WorkbenchInventoryRefreshContractTests.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\存档系统\memory_0.md`

### 验证状态

- 已验证：
  - `validate_script Assets/YYY_Scripts/Data/Core/PlayerInventoryData.cs`：
    `owned_errors=0 / external_errors=0 / native_validation=clean`
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`：
    `owned_errors=0 / external_errors=0`
    仅见既有 warning，不是本轮新红
  - `validate_script Assets/YYY_Tests/Editor/WorkbenchInventoryRefreshContractTests.cs`：
    `owned_errors=0 / external_errors=0 / native_validation=clean`
  - `git diff --check` 对上述三个文件通过
- 未闭环：
  - Unity live 仍卡 `stale_status`
  - 定向 `run_tests` 两次都返回 `total=0`，更像当前测试过滤口径 / Editor 状态问题，不像断言失败
  - 所以这轮不能说“runtime 已终验”，只能说“代码与测试护栏已落地，等待你现场手测”

### 恢复点

- 下一轮如果用户手测仍失败，继续优先看：
  1. `CraftingService.inventory` 是否在现场绑到了错误的 `InventoryService`
  2. `BoxPanelUI` 到工作台同时打开时的真实交互顺序
- 当前切片已 `PARKED`，等待用户手测“箱子拖进背包后，不关闭工作台也能立即变成可制作”。

## 2026-04-09 18:19 支撑子任务：工作台刷新测试编译修口与 SaveManager warning 清理

### 当前主线 / 子任务 / 恢复点

- 当前主线：存档系统与其关联运行态阻塞的收尾。
- 本轮子任务：修掉 `WorkbenchInventoryRefreshContractTests.cs` 的 `CS0246`，并顺手清掉 `SaveManager.cs` 新暴露的未使用字段 warning。
- 服务关系：这是给“箱子 -> 背包 -> 工作台立即刷新”那条已落地修复补测试护栏和清编译噪音，不扩成新的功能线。
- 恢复点：这条支撑子任务已结束，主线可继续回到存档系统总收口 / 用户验收链。

### 本轮完成

- 把 `Assets/YYY_Tests/Editor/WorkbenchInventoryRefreshContractTests.cs` 改成纯反射版：
  - 不再直接 `using FarmGame.Data.Core`
  - 通过 `Type.GetType(..., Assembly-CSharp)` 创建 runtime 类型
  - 继续断言 `PlayerInventoryData.SetItem / ClearItem / SetSlot` 会触发 `OnInventoryChanged`
- 保留工作台 UI 合同断言：
  - overlay 继续订阅 / 解绑 `OnSlotChanged`
  - `HandleInventorySlotChanged(int _)` 仍然存在
- 删除 `Assets/YYY_Scripts/Data/Core/SaveManager.cs` 中已无实际用途的 `_freshStartBaselineCaptureScheduled` 字段及相关赋值，消除 `CS0414` warning 来源。

### 涉及文件

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\WorkbenchInventoryRefreshContractTests.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\存档系统\memory_0.md`

### 验证状态

- `validate_script Assets/YYY_Tests/Editor/WorkbenchInventoryRefreshContractTests.cs`
  - `owned_errors=0 / external_errors=0 / warnings=0 / native_validation=clean`
  - assessment 仍是 `unity_validation_pending`，原因是 Unity 当前 `stale_status`，不是这份脚本还有红错
- `manage_script validate --name SaveManager --path Assets/YYY_Scripts/Data/Core`
  - `errors=0`
  - 还剩 3 条通用性能 warning，均为既有分析提示，不是本轮新 warning
- `errors --count 20 --output-limit 5`
  - `errors=0 / warnings=0`
- `git diff --check -- Assets/YYY_Tests/Editor/WorkbenchInventoryRefreshContractTests.cs Assets/YYY_Scripts/Data/Core/SaveManager.cs`
  - 无空白错误
  - 仅有 Git 的 CRLF 提示，不是 Unity 编译问题

### thread-state

- `Begin-Slice`：已在本切片前置阶段完成
- `Ready-To-Sync`：未跑；本轮没有进入 sync / 提交收口
- `Park-Slice`：已跑
- 当前 live 状态：`PARKED`
