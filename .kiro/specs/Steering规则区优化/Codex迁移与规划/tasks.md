# Codex迁移与规划 - 任务列表

**创建日期**: 2026-03-07
**最后更新**: 2026-03-10

---

## 一阶段：Codex 治理与工具落地

- [x] T1: 阅读 `CLAUDE.md` 与 `Claude迁移与规划` 核心文档
- [x] T2: 阅读 `.kiro/steering` 与 Hook 机制关键文档
- [x] T3: 提炼 Kiro / Claude / Codex 的能力边界差异
- [x] T4: 创建 `Codex迁移与规划` 子工作区基础四件套
- [x] T5: 形成《Codex工作流与技巧手册》初版
- [ ] T6: 用后续真实 Codex 会话验证手册是否足够稳定
- [ ] T7: 评估是否需要上提为项目级 Codex 指南
- [ ] T8: 若上提，设计最小化同步策略，避免与 Steering 双源漂移
- [x] T9: 输出 Unity MCP 候选对比文档
- [x] T10: 创建工作区路由 Skill
- [x] T11: 创建场景安全审视 Skill
- [x] T12: 创建锐评路由 Skill
- [x] T13: 为 History 交接目录建立总索引
- [x] T14: 创建 Unity 验证闭环 Skill
- [x] T15: 输出 Unity MCP 迁移试装方案

### 一阶段补充说明

- T1 ~ T5 已完成 Codex 在 Sunset 中的最小治理底盘。
- T9 ~ T15 已完成当前最需要的工具与技能落地。
- T6 ~ T8 仍保留为治理层长期验证任务，不在本轮 `.kiro/about` 文档工程里强行提前结案。

---

## 二阶段：Sunset 全面认知文档工程（2026-03-10 启动）

- [x] T16: 为 Sunset 全面分析建立可持续迭代的任务清单与执行节奏
- [x] T17: 在 `.kiro/about/` 初始化三份主文档骨架
- [ ] T18: 全量阅读现行真源文档并写入第一轮总览
- [ ] T19: 全量扫描代码与资源结构并建立系统索引
- [ ] T20: 对核心系统执行“文档-代码-memory”三向对照
- [ ] T21: 持续回写 `.kiro/about/` 三份主文档并标注可信度
- [ ] T22: 建立当前状态、漂移点与接手路径总结
- [ ] T23: 完成首轮全面分析交付并进行自校验
- [ ] T24: 在后续阅读过程中持续更新既有文档，不允许“写完即冻结”

### 二阶段进度看板（2026-03-10 当前轮次）

| 任务 | 状态 | 当前结论 |
|------|------|---------|
| T16 | ✅ 完成 | 已把“先列 tasks、持续对照、实时纠偏”的策略正式写入工作区任务清单。 |
| T17 | ✅ 完成 | `.kiro/about/` 三份主文档已建立：总览、系统全景、状态接手。 |
| T18 | 🔄 进行中 | 第二轮已补读 `存档系统`、`箱子系统`、`物品放置系统`、`制作台系统`、`UI系统` 的 live `memory.md`，并交叉读取 `History/总索引`、`Docx/分类/全局/*`、`Docx/Plan/000_设计规划完整合集.md`、`Docx/分类/交接文档/000_交接文档完整合集.md`。 |
| T19 | 🔄 进行中 | 已从“目录快照”推进到“场景/Prefab/SO 映射”：确认 `Primary.unity` 中 `InventorySystem/HotbarSelection/DialogueManagerRoot/PlacementManager/SaveManager/PersistentManagers` 的现场挂载，确认 `PackagePanel.prefab` 中 `InventoryPanelUI(upCount=36, downCount=6, database 已绑定)` 与 `BoxUIRoot`，并确认 `DialogueValidation.unity` 中 `DialogueCanvas` scale 为 `0,0,0`、`DialogueUI` 引用为空但场景对象名可被脚本自动补线。 |
| T20 | 🔄 进行中 | 已完成库存/UI、箱子、放置、存档、制作台、对话验证的第二轮链路级核对；并新增资产侧反证：`MasterItemDatabase.asset` 的 `allRecipes` 仍是 3 个空槽，当前未找到 `RecipeData` / `WorkstationData` 资产引用，也未找到 prefab 对 `CraftingService` 的直接挂载。 |
| T21 | 🔄 进行中 | `.kiro/about/` 已开始修正更细的旧判断，包括放置系统 `V3` 命名漂移、背包 `20+8` → `36+12` 漂移、制作台旧路径与配置状态不明等问题。 |
| T22 | 🔄 进行中 | 已把接手结论细化到系统级：存档主路径、箱子 UI 链、放置中断机制、Day1 当前“场景可见性与布局阻塞优先于脚本阻塞”的判断，以及制作台“代码已落地但资产/场景接入证据不足”的更收紧口径。 |
| T23 | ⏳ 待执行 | 等第一轮总览、系统索引和状态总结稳定后统一做自校验。 |
| T24 | 🔄 持续执行中 | 已开始采用“边读边修旧结论”的维护方式，后续继续保持。 |

---

## T16: 为 Sunset 全面分析建立可持续迭代的任务清单与执行节奏

**状态**: ✅ 完成

**说明**:
- 已把“先立清单、持续对照、实时更新”的策略正式写入当前工作区。
- 之后所有关于 Sunset 全面理解的推进，都必须先回看本任务清单，再更新状态和文档。

---

## T17: 在 `.kiro/about/` 初始化三份主文档骨架

**状态**: ✅ 完成

**说明**:
- 已创建三份主文档：
  - `00_项目总览与阅读地图.md`
  - `01_系统架构与代码全景.md`
  - `02_当前状态、差异与接手指南.md`
- 三份文档都允许先写初稿、后续持续修订，不等待“全部读完”再统一落盘。

---

## T18: 全量阅读现行真源文档并写入第一轮总览

**状态**: 🔄 进行中

**目标**:
- 建立“真源在哪里、哪些只是历史快照、后来者先读什么”的稳定地图。

**本轮已完成**:
- `.kiro/steering/README.md`
- `.kiro/steering/archive/product.md`
- `.kiro/steering/archive/structure.md`
- `.kiro/steering/archive/progress.md`
- `.kiro/steering/archive/tech.md`
- `.kiro/specs/README.md`
- `.kiro/specs/农田系统/memory.md`
- `.kiro/specs/900_开篇/spring-day1-implementation/memory.md`
- `.kiro/specs/999_全面重构_26.01.27/memory.md`
- `.kiro/specs/存档系统/memory.md`
- `.kiro/specs/箱子系统/memory.md`
- `.kiro/specs/物品放置系统/memory.md`
- `.kiro/specs/制作台系统/memory.md`
- `.kiro/specs/UI系统/memory.md`（概览与关键阶段）
- `History/2026.03.07-Claude-Cli-历史会话交接/总索引.md`
- `Docx/分类/全局/000_项目全局文档.md`
- `Docx/分类/全局/全局功能需求总结文档.md`
- `Docx/Plan/000_设计规划完整合集.md`
- `Docx/分类/交接文档/000_交接文档完整合集.md`

**下一步**:
- 继续补读更细的子工作区 `memory.md`，但以“能改变总判断的证据”为主，不做机械平扫
- 把第二轮文档结论继续回写到 `.kiro/about/00_项目总览与阅读地图.md`

---

## T19: 全量扫描代码与资源结构并建立系统索引

**状态**: 🔄 进行中

**目标**:
- 用真实仓库结构校准历史文档描述，避免继续把旧路径、旧目录映射当成当前现实。

**本轮已完成快照**:
- `Assets/YYY_Scripts/` 下有 172 个 `.cs` 文件
- `Assets/YYY_Tests/Editor/` 下有 8 个 EditMode 测试脚本
- `Assets/000_Scenes/` 下有 5 个场景
- `Assets/222_Prefabs/` 下有 343 个 Prefab
- `Assets/111_Data/Items/` 下有 71 个 Item/SO 资产
- `Assets/Scripts/Utils/` 下仍保留 1 个旧路径工具脚本
- `Primary.unity` 已确认挂有 `PlacementManager`、`SaveManager`、`PersistentManagers`
- `Primary.unity` 已确认存在 `InventorySystem`（挂 `InventoryService`，`inventorySize=36`）、`HotbarSelection`、`DialogueManagerRoot`
- `PersistentManagers` 在 `Primary.unity` 中已直接持有 `PrefabDatabase.asset`
- `Assets/111_Data/Database/PrefabDatabase.asset` 已确认扫描 `Tree/Rock/Box/WorldItems` 并维护旧 ID → 新 Prefab 名别名映射
- `Storage_1400_小木箱子.asset` 已确认容量 12、`boxUiPrefab` 指向箱子 UI 预制体
- `Assets/222_Prefabs/UI/Chest/Box/Box_36.prefab` 已确认存在 `Up/Down` 双区域与 `Sort/Trash` 按钮配置
- `Assets/222_Prefabs/UI/0_Main/PackagePanel.prefab` 已确认挂 `InventoryPanelUI`，其中 `upCount=36`、`downCount=6`、`database` 已绑定，并存在 `BoxUIRoot`
- `Assets/111_Data/UI/Fonts/Dialogue/` 已确认存在对话中文字体源文件目录
- `DialogueValidation.unity` 已确认 `DialogueCanvas` 的 `RectTransform.m_LocalScale = (0,0,0)`，`DialogueUI` 的 5 个关键序列化引用为空
- `DialogueValidation.unity` 中同时存在 `DialoguePanel`、`SpeakerNameText`、`DialogueText`、`ContinueButton`、`PortraitImage`、`Background`，且 `DialogueUI.cs` 有验证场景按对象名自动补线逻辑
- `DialogueManagerRoot` 在 `DialogueValidation.unity` 中同时挂有 `DialogueValidationBootstrap` 与 `DialogueManager`

**下一步**:
- 继续细化关键场景中的组件挂载与资产引用
- 继续建立 `Script -> Scene/Prefab/SO -> memory` 的映射，而不是只看目录数量

---

## T20: 对核心系统执行“文档-代码-memory”三向对照

**状态**: 🔄 进行中

**目标**:
- 对每个核心系统明确：哪些结论来自代码、哪些来自历史文档、哪些来自当前工作区记忆。

**本轮已完成入口核对**:
- 时间 / 季节 / 天气：
  - `TimeManager.cs`
  - `SeasonManager.cs`
  - `WeatherSystem.cs`
- 玩家 / 输入 / 动画：
  - `GameInputManager.cs`
  - `PlayerAutoNavigator.cs`
  - `NavGrid2D.cs`
- 农田 / 放置：
  - `FarmTileManager.cs`
  - `PlacementManager.cs`
  - `PlacementValidator.cs`
- 存档：
  - `SaveManager.cs`
  - `DynamicObjectFactory.cs`
  - `PrefabDatabase.cs`
  - `PrefabRegistry.cs`
- 对话：
  - `DialogueManager.cs`
  - `DialogueUI.cs`
  - `DialogueValidationBootstrap.cs`
- 库存 / UI：
  - `InventoryService.cs`
  - `InventoryPanelUI.cs`
  - `InventoryInteractionManager.cs`
  - `PackagePanelTabsUI.cs`
  - `InventoryBootstrap.cs`
- 箱子：
  - `ChestController.cs`
  - `ChestInventory.cs`
  - `ChestInventoryV2.cs`
  - `BoxPanelUI.cs`
- 制作台 / 配方：
  - `CraftingService.cs`
  - `CraftingPanel.cs`
  - `RecipeData.cs`
  - `WorkstationData.cs`

**本轮新增反证**:
- `Assets/111_Data/Database/MasterItemDatabase.asset` 当前 `allRecipes` 仍是 3 个 `{fileID: 0}` 空槽
- 当前未搜到任何 `.asset` 引用 `RecipeData.cs` 或 `WorkstationData.cs` 的脚本 GUID
- 当前未搜到 prefab 对 `CraftingService.cs` 的直接挂载引用

**下一步**:
- 继续补 `Primary.unity`、关键 Prefab 与数据库资产的现场证据
- 对每个系统补“运行链路 + 文档漂移 + 当前待验证点”

---

## T21: 持续回写 `.kiro/about/` 三份主文档并标注可信度

**状态**: 🔄 进行中

**目标**:
- 把 about 文档从“骨架”推进成真正可接手的总文档。

**本轮已执行的修正**:
- 把“仓库只有 `Assets/YYY_Scripts/`”修正为“`Assets/YYY_Scripts/` 为主、`Assets/Scripts/Utils/` 为旧残留并存”
- 把“`PrefabRegistry` 已彻底消失”修正为“主路径已转向 `PrefabDatabase`，但兼容回退和旧资产仍存在”
- 把“.kiro/specs/README.md 可以直接代表当前工作区”修正为“它只适合作为旧索引”
- 把“放置系统当前仍以 `PlacementManagerV3` 命名为主”修正为“live memory 保留 V3 术语，但当前代码文件名已回到 `PlacementManager/Validator/Preview`”
- 把“背包仍是旧文档里的 20 格主背包 + 8 格快捷栏”修正为“当前实现是 36 格背包，第一行 12 格与 Hotbar 映射”
- 把“制作台系统可直接按旧 Docx 路径理解”修正为“代码已迁入 `Assets/YYY_Scripts`，旧 `Assets/Scripts` 路径更多体现历史设计意图”
- 把“Day1 对话线当前主要是字体问题”修正为“更准确地说是 `DialogueValidation` 场景存在 UI 可见性与 Inspector 空引用问题，但空引用并非唯一阻塞，因为 `DialogueUI` 自带按名称自动补线”
- 把“制作台状态待确认”进一步收紧为“代码已落地，但当前资产与场景接入证据不足，不能写成完整闭环”

**下一步**:
- 每新读完一批文档或代码，就先修正文档旧判断，再追加新结论

---

## T22: 建立当前状态、漂移点与接手路径总结

**状态**: 🔄 进行中

**目标**:
- 让后来者只看 `.kiro/about/` 也能知道项目现在在哪、哪些文档还能信、下一步该从哪里接。

**本轮已明确的接手结论**:
- `农田系统` 当前最接近“代码已落地、待编辑器内手动回归”
- `spring-day1-implementation` 当前主阻塞在 `DialogueValidation` 场景验证与中文 TMP 引用验收
- `Codex迁移与规划` 当前新增主线是 `.kiro/about/` 三文档工程本身
- 如果接 `存档 / 箱子 / 放置`，应联读 `存档系统/memory.md`、`箱子系统/memory.md`、`物品放置系统/memory.md`，因为这三者已经形成实际耦合链
- 如果接 `UI / 背包`，应优先看 `UI系统/memory.md` 里的交互教训，再回到 `InventoryInteractionManager.cs` 与 `PackagePanelTabsUI.cs`
- 如果接 `制作台 / 配方`，当前更像“代码骨架已在，但资产与场景接入证据不足”的状态：`MasterItemDatabase.asset` 的 `allRecipes` 仍为空槽，也还没找到清晰的 `RecipeData` / `WorkstationData` 资产落点
- 如果接 `Day1 对话验证`，当前不能只盯字体：`DialogueCanvas` 目前 scale 为 `0,0,0`，`DialogueUI` 引用为空，但空引用并非唯一问题，因为场景对象名仍可被脚本自动补线，真正阻塞更偏向 UI 可见性 / 布局优先

**下一步**:
- 继续把“主线状态 + 漂移点 + 接手顺序”写得更具体

---

## T23: 完成首轮全面分析交付并进行自校验

**状态**: ⏳ 待执行

**自校验要求**:
- 三份主文档之间不能互相矛盾
- 关键结论必须能追溯到代码、规则文档或工作区记忆
- 不能把“历史设计”误写成“当前实现”

---

## T24: 在后续阅读过程中持续更新既有文档，不允许“写完即冻结”

**状态**: 🔄 持续执行中

**说明**:
- 这是长期纪律，不是一次性任务。
- 后续每读到新脚本、新工作区、新历史文档，只要形成了新的稳定理解，就必须回头修正文档旧结论，不能把第一版当终稿。
