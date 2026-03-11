# Codex迁移与规划 - 任务列表

**创建日期**: 2026-03-07
**最后更新**: 2026-03-11

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
- [x] T18: 全量阅读现行真源文档并写入第一轮总览
- [x] T19: 全量扫描代码与资源结构并建立系统索引
- [x] T20: 对核心系统执行“文档-代码-memory”三向对照
- [x] T21: 持续回写 `.kiro/about/` 三份主文档并标注可信度
- [x] T22: 建立当前状态、漂移点与接手路径总结
- [x] T23: 完成首轮全面分析交付并进行自校验
- [ ] T24: 在后续阅读过程中持续更新既有文档，不允许“写完即冻结”

### 二阶段进度看板（2026-03-10 当前轮次）

| 任务 | 状态 | 当前结论 |
|------|------|---------|
| T16 | ✅ 完成 | 已把“先列 tasks、持续对照、实时纠偏”的策略正式写入工作区任务清单。 |
| T17 | ✅ 完成 | `.kiro/about/` 三份主文档已建立：总览、系统全景、状态接手。 |
| T18 | ✅ 完成 | 已完成现行真源文档、活跃工作区 memory、History 总索引与高价值 Docx 总纲的系统补读，并回写进 `00` 阅读地图。 |
| T19 | ✅ 完成 | 已完成从目录快照到场景/Prefab/SO 映射的首轮系统索引，关键证据已覆盖 `Primary.unity`、`DialogueValidation.unity`、`PackagePanel.prefab`、`PrefabDatabase.asset`、`MasterItemDatabase.asset`。 |
| T20 | ✅ 完成 | 已完成核心系统“文档-代码-memory”三向对照的首轮闭环，覆盖库存/UI、箱子、放置、存档、制作台、对话验证、导航/遮挡等主系统。 |
| T21 | ✅ 完成 | about 三文档已达到可接手版；后续持续修正纪律转入 T24 维护，不再作为本轮未完成项悬挂。 |
| T22 | ✅ 完成 | 已把当前状态、漂移点、接手路径细化到系统级，并形成可直接使用的接手指南与风险矩阵。 |
| T23 | ✅ 完成 | 已完成首轮全面分析交付并做过一致性审视；当前剩余不确定项都已显式写为“待验证”，没有隐藏成错误定论。 |
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

**状态**: ✅ 完成

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

**完成结论**:
- 当前已经形成稳定的“规则层 → 活跃工作区层 → 交接索引层 → 历史设计层”阅读路线。
- 后续若再补读，只属于 T24 的持续增强，不再影响“首轮总览已交付”的判断。

---

## T19: 全量扫描代码与资源结构并建立系统索引

**状态**: ✅ 完成

**目标**:
- 用真实仓库结构校准历史文档描述，避免继续把旧路径、旧目录映射当成当前现实。

**本轮已完成快照**:
- `Assets/YYY_Scripts/` 下有 172 个 `.cs` 文件
- `Assets/YYY_Tests/Editor/` 下有 8 个 EditMode 测试脚本
- `Assets/000_Scenes/` 下有 6 个场景（`Primary`、`DialogueValidation`、`Artist`、`Artist_Temp`、`SampleScene`、`矿洞口`）
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

**完成结论**:
- 当前已形成足以支撑接手的首轮系统索引；后续再补更多证据时，按 T24 持续回写即可。

---

## T20: 对核心系统执行“文档-代码-memory”三向对照

**状态**: ✅ 完成

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

**完成结论**:
- 当前 about 三文档已经能把主要系统的“代码入口 + 现场证据 + 漂移点 + 接手口径”串起来。
- 剩余不确定项已被显式写成未决问题，而不是继续混成含糊结论。

---

## T21: 持续回写 `.kiro/about/` 三份主文档并标注可信度

**状态**: ✅ 完成

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

**状态**: ✅ 完成

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

**完成结论**:
- 当前 `02_当前状态、差异与接手指南.md` 已能直接回答“项目现在在哪、最危险的误判是什么、我该怎么接手”。

---

## T23: 完成首轮全面分析交付并进行自校验

**状态**: ✅ 完成

**本轮自校验结果**:
- 三份主文档之间不能互相矛盾
- 关键结论必须能追溯到代码、规则文档或工作区记忆
- 不能把“历史设计”误写成“当前实现”
- 已修正的冲突包括：场景数量 `5 → 6`、`02` 中已过期的“后续仍要补齐”占位，以及多处“草稿式”成熟度表述

---

## T24: 在后续阅读过程中持续更新既有文档，不允许“写完即冻结”

**状态**: 🔄 持续执行中

**说明**:
- 这是长期纪律，不是一次性任务。
- 后续每读到新脚本、新工作区、新历史文档，只要形成了新的稳定理解，就必须回头修正文档旧结论，不能把第一版当终稿。

---

## 三阶段：about 文档闭环增强（2026-03-10 当前轮次追加）

### 当前缺口诊断（基于最初需求回看）

- 缺口 1：`00_项目总览与阅读地图.md` 已有入口图，但还缺“角色化阅读路径、可信度矩阵、误读预警、只看三文档如何起步”的闭环级内容。
- 缺口 2：`01_系统架构与代码全景.md` 已有系统入口与部分现场证据，但还缺“系统覆盖矩阵、场景/Prefab/SO 映射表、验证入口、未决问题清单”。
- 缺口 3：`02_当前状态、差异与接手指南.md` 已有漂移点和接手顺序，但还缺“系统完成度总表、接手 SOP、风险优先级、闭环标准”。
- 缺口 4：三文档虽然已有核心结论，但还没完成一轮显式的“只看这三份文档是否足够独立接手”的自校验。

### 三阶段详细任务看板

| 任务 | 状态 | 目标 |
|------|------|------|
| T25 | ✅ 完成 | `00` 已补齐三文档使用方式、角色化阅读路径、可信度矩阵、误读预警，并顺手修正场景数量为 6。 |
| T26 | ✅ 完成 | `01` 已补齐系统覆盖矩阵、场景/Prefab/SO 映射速查表、测试与验证入口、未决问题清单。 |
| T27 | ✅ 完成 | `02` 已补齐系统完成度 / 可信度总表、接手 SOP、风险优先级矩阵、闭环标准。 |
| T28 | ✅ 完成 | 已完成 `00/01/02` 一致性审视，并统一修正“场景数量 5→6”、成熟度表述与 Day1 / 制作台 / 背包容量口径。 |
| T29 | ✅ 完成 | 已把本轮闭环增强结果同步回 `tasks.md`、子工作区 memory、父工作区 memory、线程记忆。 |

---

## T25: 为 `00_项目总览与阅读地图.md` 增补角色化阅读路径、可信度矩阵、误读预警与三文档使用方式

**状态**: ✅ 完成

**目标**:
- 让后来者不只知道“读什么”，还知道“按什么角色、什么深度、什么可信度去读”。

**本轮已完成**:
- 已补三文档总使用方式
- 已补角色化阅读路径（项目负责人 / 新开发者 / 系统修复型开发者 / 智能体）
- 已补文档可信度矩阵
- 已补常见误读预警

---

## T26: 为 `01_系统架构与代码全景.md` 增补系统覆盖矩阵、场景/Prefab/SO 映射、验证入口与未决问题清单

**状态**: ✅ 完成

**目标**:
- 把 `01` 从“系统说明”推进到“系统索引 + 证据索引 + 验证入口”的真正接手底图。

**本轮已完成**:
- 已补系统覆盖矩阵
- 已补场景 / Prefab / SO 现场映射速查表
- 已补测试与验证入口
- 已补当前未决问题清单

---

## T27: 为 `02_当前状态、差异与接手指南.md` 增补系统完成度表、接手 SOP、风险优先级与闭环标准

**状态**: ✅ 完成

**目标**:
- 让 `02` 能真正回答“项目做到哪了、我接手先做什么、做到什么算闭环”。

**本轮已完成**:
- 已补系统完成度 / 可信度表
- 已补接手 SOP
- 已补风险优先级矩阵
- 已补闭环验收标准

---

## T28: 对 `00/01/02` 三文档做一致性审视

**状态**: ✅ 完成

**目标**:
- 检查三文档是否已经足以独立支撑接手，是否还存在历史 / live / 推断混写。

**检查点**:
- 口径是否一致
- 结论是否可追溯
- 是否还保留“后续再补”但实际没动手的空洞段落

**本轮已修正**:
- 已把场景数量从旧结论 `5` 修正为实仓核对后的 `6`
- 已移除 `02` 中“后续仍要补齐”的过期占位，改为“本轮已补齐的闭环内容”
- 已统一 Day1 为“视觉 / 布局阻塞优先于纯脚本阻塞”的口径
- 已统一制作台为“代码骨架已在，但资产 / 场景接入证据不足”的口径
- 已统一背包 / Hotbar 为“36 格背包，第一行 12 格映射 Hotbar”的口径

---

## T29: 闭环增强后的全链路同步

**状态**: ✅ 完成

**目标**:
- 在本轮闭环增强完成后，及时回写 tasks 与多层 memory，确保后续线程能从当前最终状态继续，而不是从中途版本继续。

**本轮已完成**:
- 已收口本文件中 T18 ~ T23 的勾选状态，避免“正文已完成但清单仍未完成”的误导。
- 已收口 T28 / T29 的表格状态与正文状态，避免任务表自身再成为新的漂移源。
- 已按顺序同步：子工作区 `memory.md` → 父工作区 `memory.md` → 线程记忆 `memory_0.md`。

---

## 四阶段：工作区宪法与长期维护协议（2026-03-10 当前轮次追加）

### 当前新增目标

- 由于用户明确授权完全自治推进，并要求把其长期要求固化为工作区级“宪法”，当前需要在不改变三主文档方案的前提下，补一份专门约束本工作区执行方式的治理文档。
- 这份宪法不替代 `AGENTS.md`、`.kiro/steering/` 或 `.kiro/about/`；它的作用是把“这个工作区如何持续正确做事”固定下来，降低后续智能体或后续会话跑偏的概率。

### 四阶段详细任务看板

| 任务 | 状态 | 目标 |
|------|------|------|
| T30 | ✅ 完成 | `工作区宪法.md` 已创建，并固化用户要求、主交付形态、执行边界、迭代与同步纪律。 |
| T31 | ✅ 完成 | 宪法已接入 `00` 与 `02` 的维护入口，后来者可以从三主文档自然追到治理约束。 |
| T32 | ✅ 完成 | 长期维护触发条件、最小自检口径与恢复点已写入宪法，并补接到 `02` 的维护入口。 |
| T33 | ✅ 完成 | 已完成本轮宪法落盘后的子工作区 / 父工作区 / 线程记忆同步，并固定当前恢复点。 |

---

## T30: 创建工作区宪法文档

**状态**: ✅ 完成

**目标**:
- 把用户已经多轮明确的长期要求固化成当前工作区的唯一执行宪法，供后续 Codex / 其他智能体继续维护 `.kiro/about/` 文档工程时遵守。

**本轮已完成**:
- 已创建 `工作区宪法.md`
- 已把三主文档固定方案、语言要求、tasks 先行、证据优先、高风险审视边界、memory 同步顺序和闭环标准写成长期条款

---

## T31: 把宪法接入任务看板与文档维护入口

**状态**: ✅ 完成

**目标**:
- 确保后来者不会只看到三主文档，却漏掉维护这些文档时必须遵守的治理约束。

**本轮已完成**:
- 已在 `00_项目总览与阅读地图.md` 补“如果你是来继续维护这三份 about 文档”的入口
- 已在 `02_当前状态、差异与接手指南.md` 补维护前必读宪法的起手动作

---

## T32: 为 about 文档工程补长期维护触发条件与最小自检清单

**状态**: ✅ 完成

**目标**:
- 把“什么时候该继续更新 `.kiro/about/`、更新前后最少要检查什么”写成可复用协议，避免后续维护靠临场发挥。

**本轮已完成**:
- 已在宪法中写入 about 文档更新触发条件、最小自检清单和固定恢复点
- 已在 `02` 中补“什么情况下必须回写”的维护入口

---

## T33: 宪法落盘后的全链路同步

**状态**: ✅ 完成

**目标**:
- 在本轮宪法与维护协议落盘后，按顺序回写子工作区、父工作区和线程记忆，并把当前恢复点固定下来。

**本轮已完成**:
- 已完成 `tasks.md`、`工作区宪法.md`、`00`、`02` 的本轮收口。
- 已按顺序回写子工作区、父工作区和线程记忆。
- 已把当前恢复点固定为“后续新增 live 证据时，先更 tasks，再按宪法继续回写三主文档与 memory”。

---

## 五阶段：Sunset Git 安全基线落地（2026-03-11 已闭环）

- [x] T34: 复核旧 Git 结论并把本轮治理任务写入执行看板
- [x] T35: 新增 Git 安全规范文档并接入 steering 路由
- [x] T36: 补 `.gitattributes` 统一 Unity 文本/二进制属性
- [x] T37: 更新 `.gitignore` 并处理 `.claude` 本地噪音跟踪策略
- [x] T38: 新增 Git preflight Hook 并重写 `git-quick-commit.kiro.hook`
- [x] T39: 更正 `10.2.2补丁002` 与相关记忆中的过期 Git 事实
- [x] T40: 完成本轮 Git 基线治理后的多层记忆同步

### 五阶段进度看板（2026-03-11 当前轮次）

| 任务 | 状态 | 当前结论 |
|------|------|---------|
| T34 | ✅ 完成 | 已确认当前 `main` 与 `origin/main` 同步，旧的 `ahead 4` 判断已过期，但 dirty 状态和 `.claude/worktrees` 污染仍然成立。 |
| T35 | ✅ 完成 | `.kiro/steering/git-safety-baseline.md` 已创建并接入 `README.md`、`AGENTS.md`、`smart-assistant.kiro.hook`。 |
| T36 | ✅ 完成 | `.gitattributes` 已落地，`.unity` / `.md` / `.kiro.hook` / `.cmd` 都已命中预期属性。 |
| T37 | ✅ 完成 | `.gitignore` 已更新，`.claude/worktrees/` 与 `.claude/settings.local.json` 已按本地噪音策略处理。 |
| T38 | ✅ 完成 | `git-preflight.kiro.hook` 已新增，`git-quick-commit.kiro.hook` 已改为安全版提交流程。 |
| T39 | ✅ 完成 | `10.2.2补丁002/memory.md` 与 `农田系统/memory.md` 已纠正“ahead 4”旧事实，改写为“dirty 状态与任务分支仍是阻塞”。 |
| T40 | ✅ 完成 | Git 基线结论已同步到子工作区、父工作区、业务工作区与相关线程记忆，并补出当前仓库脏改拆分清单。 |

## T34: 复核旧 Git 结论并把本轮治理任务写入执行看板

**状态**: ✅ 完成

**目标**:
- 确认另一条线程中的 Git 诊断哪些还有效、哪些已经过期，并把本轮仓库级基线任务正式纳入当前工作区的治理看板。

**本轮已完成**:
- 已确认 `git rev-list --left-right --count origin/main...main` 当前结果为 `0 0`
- 已确认旧结论中“有远端、Unity 可追踪基础正确、缺 `.gitattributes`、缺 Git 制度、`.claude/worktrees` 是噪音源”仍然有效
- 已确认旧结论中“`main ahead 4`”已过期，不能继续作为当前现场事实引用

## T35: 新增 Git 安全规范文档并接入 steering 路由

**状态**: ✅ 完成

**目标**:
- 形成一份仓库级、可复用、可被后续线程直接引用的 Git 安全规范，作为业务线程进入真实实现前的统一闸门。

**本轮已完成**:
- 已创建 `.kiro/steering/git-safety-baseline.md`
- 已把 Git 路由接入 `.kiro/steering/README.md`、项目 `AGENTS.md` 与 `.kiro/hooks/smart-assistant.kiro.hook`
- 已把“独立 `codex/` 分支、preflight、checkpoint、rollback、dirty 分类”固化成仓库级口径

## T36: 补 `.gitattributes` 统一 Unity 文本/二进制属性

**状态**: ✅ 完成

**目标**:
- 降低 Windows 环境下的 CRLF / LF 噪音，稳定 Unity 文本资源、脚本、规则文档与常见二进制文件的属性。

**本轮已完成**:
- 已创建仓库级 `.gitattributes`
- 已为 Unity 文本资源、规则文档、Hook、脚本与常见二进制资源建立稳定属性
- 已用 `git check-attr` 确认 `Primary.unity`、`git-safety-baseline.md`、`git-quick-commit.kiro.hook` 命中预期设置

## T37: 更新 `.gitignore` 并处理 `.claude` 本地噪音跟踪策略

**状态**: ✅ 完成

**目标**:
- 明确哪些 `.claude` 内容属于本地结构，后续不应再持续污染根仓库状态；同时避免误伤现有用户本地内容。

**本轮已完成**:
- 已更新 `.gitignore`
- 已把 `.claude/worktrees/agent-a2df3da0` 与 `.claude/settings.local.json` 从根仓库跟踪中移除
- 已保留本地文件与工作树目录本身，不做破坏性删除

## T38: 新增 Git preflight Hook 并重写 `git-quick-commit.kiro.hook`

**状态**: ✅ 完成

**目标**:
- 用“先预检、再提交”的方式替代“无差别全量 add + 直推 main”。

**本轮已完成**:
- 已新增 `.kiro/hooks/git-preflight.kiro.hook`
- 已把 `.kiro/hooks/git-quick-commit.kiro.hook` 改写为“先预检、显式筛文件、按当前分支推送”的安全版
- 已用 `ConvertFrom-Json` 校验两个 Hook 的 JSON 正确性

## T39: 更正 `10.2.2补丁002` 与相关记忆中的过期 Git 事实

**状态**: ✅ 完成

**目标**:
- 避免业务线程继续沿用已经失效的现场状态，把真正仍未解决的 Git 阻塞重新表述准确。

**本轮已完成**:
- 已在 `10.2.2补丁002/memory.md` 回写“`main ahead 4` 已过期，当前阻塞改为 dirty 状态拆分 + 任务分支建立”
- 已在 `农田系统/memory.md` 同步业务侧恢复点
- 已确保“Git 基线已补齐，但业务实现仍暂停”这一口径不再漂移

## T40: 完成本轮 Git 基线治理后的多层记忆同步

**状态**: ✅ 完成

**目标**:
- 按项目记忆纪律把本轮治理结论同步到子工作区、父工作区、业务工作区和线程记忆，确保后续线程从最新事实接手。

**本轮已完成**:
- 已完成 `Codex迁移与规划` 子工作区 `memory.md` 更新
- 已完成父工作区 `Steering规则区优化/memory.md` 更新
- 已完成 `10.2.2补丁002/memory.md` 与 `农田系统/memory.md` 的受影响业务记忆更新
- 已完成相关线程记忆同步
- 已补 `当前仓库 Git 脏改拆分清单_2026-03-11.md`，明确当前治理 checkpoint 边界与进入实现前阻塞

---

## 六阶段：深水区证据补强与巡检清单（2026-03-11 当前轮次）

### 当前判断

- 当前三主文档与宪法、tasks、memory 体系已经闭环，可直接接手。
- 本轮继续补的不是新的主报告，而是最容易影响接手质量的三块深水区证据，以及一份可持续复用的巡检清单。

### 六阶段详细任务看板

| 任务 | 状态 | 目标 |
|------|------|------|
| T41 | ✅ 完成 | 已为 `01` 补导航 / 遮挡 / 树木的代码、现场、测试三层证据，并抬升其可接手度。 |
| T42 | ✅ 完成 | 已补 `Primary` / `DialogueValidation` / `Artist` / `Artist_Temp` / `SampleScene` / `矿洞口` 的场景责任矩阵。 |
| T43 | ✅ 完成 | 已把制作台 / 配方从“泛泛证据不足”推进为“目录、资产、Prefab、场景挂载、UI 视觉残留”多维搜证。 |
| T44 | ✅ 完成 | 已创建 `about一致性巡检清单.md` 并接入三主文档维护入口，降低后续巡检成本。 |

---

## T41: 补导航 / 遮挡 / 树木系统证据章节

**状态**: ✅ 完成

**目标**:
- 把导航 / 遮挡 / 树木从“已识别但证据仍薄”的状态，推进到和存档 / 箱子 / 背包同等级的接手章节。

**本轮已完成**:
- 已确认 `Primary.unity` 同时挂有 `NavGrid2D`、`OcclusionManager`、`ResourceNodeRegistry`
- 已核对 `NavGrid2D.cs` 的自动世界边界检测、`Physics2D.SyncTransforms()` 刷新与障碍物采样逻辑
- 已核对 `OcclusionManager.cs` 与 `OcclusionTransparency.cs` 的“玩家 Collider 中心 + SortingLayer + 树林整体透明 + 放置预览遮挡”链路
- 已核对 `TreeController.cs` 的 6 阶段成长、资源节点、遮挡、持久化耦合
- 已把 `OcclusionSystemTests.cs` 与 `CloudShadowSystemTests.cs` 纳入系统证据

---

## T42: 建立场景责任矩阵

**状态**: ✅ 完成

**目标**:
- 明确 `Primary` 之外其他场景的定位，尤其是 `Artist.unity`、`Artist_Temp.unity`、`矿洞口.unity` 当前到底是验证场景、资源场景还是潜在业务入口。

**本轮已完成**:
- 已确认 `ProjectSettings/EditorBuildSettings.asset` 当前只启用 `Primary.unity`
- 已确认 `DialogueValidation.unity` 挂有 `DialogueUI`、`DialogueManager`、`DialogueValidationBootstrap`
- 已确认 `Artist.unity`、`Artist_Temp.unity`、`矿洞口.unity` 当前未直接挂自定义 MonoBehaviour，且对象名分别更像 UI 草图、Tilemap 试验场与大型地图片段
- 已把以上判断回写到 `00`、`01`、`02`

---

## T43: 制作台 / 配方专项搜证

**状态**: ✅ 完成

**目标**:
- 继续核对 `RecipeData`、`WorkstationData`、`CraftingService` 的资产、Prefab、场景挂载与数据库引用，把当前“证据不足”推进为更强的正证或反证。

**本轮已完成**:
- 已确认 `Assets/111_Data/Recipes/` 文件夹存在但当前为空
- 已确认 `Assets/111_Data/` 下未找到引用 `RecipeData.cs` / `WorkstationData.cs` GUID 的 `.asset` / `.prefab` / `.unity`
- 已确认全仓 `Prefab / Scene` 范围内未找到 `CraftingService.cs` 或 `CraftingPanel.cs` 的 live 挂载引用
- 已确认 `PackagePanel.prefab` 内存在 `1_Recipes` 区域和多份 `Recipe.prefab` 视觉实例，说明制作台 UI 视觉残片存在，但运行时主挂载链仍未建立
- 已把制作台口径收紧为“代码骨架与局部 UI 资产存在，但 live 数据与场景接入仍明显不足”

---

## T44: 建立 about 一致性巡检清单

**状态**: ✅ 完成

**目标**:
- 为后续迭代建立固定的巡检动作，减少每次都靠人工重新发明自校验路径的成本。

**本轮已完成**:
- 已创建 `.kiro/specs/Steering规则区优化/Codex迁移与规划/about一致性巡检清单.md`
- 已把“起手前检查、核心事实快照、三文档逐份检查、跨文档一致性检查、tasks / memory 同步检查”固化为清单
- 已把巡检清单接入 `00` 与 `02` 的维护入口，供后来者直接照表执行

---

## 九阶段：Git 自动同步现状说明与跨工作区通报（2026-03-11 启动）

### 九阶段详细任务看板

| 任务 | 状态 | 目标 |
|------|------|------|
| T53 | ✅ 完成 | 已产出一份面向其他工作区的当前仓库 Git/分支/自动同步现状说明文档，写清已完成事项、真实现场与未完成项。 |
| T54 | ✅ 完成 | 已将现状说明文档、补漏治理文档与多层记忆纳入白名单同步口径，并通过干净临时 worktree 推送到远端可查询基线。 |

## T53: 产出跨工作区现状说明文档

**状态**: ✅ 完成

**目标**:
- 把当前仓库真实状态、已经完成的 Git 治理工作、自动同步脚本落地结果、分支现状和未完成事项，整理成一份其他工作区也能直接查阅的说明文档。

**本轮已完成**:
- 已新建 `当前仓库Git自动同步与治理现状说明_2026-03-11.md`。
- 已在文档内明确区分“远端最新治理基线”和“当前本地 main 的旧状态”，避免后来者误把 `3b45da72` 当成最新现场。
- 已在文档中补充“本轮新增的跨工作区记录动作”“哪些文件已经在远端基线里、但本地旧 main 还看不到”“更新后的未完成事项清单”。

## T54: 将说明文档、补漏治理文档与多层记忆安全同步到远端

**状态**: ✅ 完成

**目标**:
- 不在当前 dirty 的本地 `main` 上硬提交流水，而是用白名单方式把本轮治理文档和记忆安全同步到远端公开基线。

**本轮已完成**:
- 已采用干净临时 worktree `C:\Users\aTo\AppData\Local\Temp\sunset-status-sync-d380b3e638d04631bf802aabe7814ae1`，承接本轮同步动作。
- 已把 `当前仓库Git自动同步与治理现状说明_2026-03-11.md`、`about一致性巡检清单.md`、九阶段看板更新与相关多层记忆纳入白名单同步范围。
- 已将“当前本地 `main` 仍落后远端、仓库依旧 dirty、农田 `10.2.2` 仍不得直接进入实现”这一口径同步为可查阅的远端基线。

---

## 十阶段：Sunset 线程 - 分支 - Worktree 固化治理（2026-03-11 启动）

### 十阶段详细任务看板

| 任务 | 状态 | 目标 |
|------|------|------|
| T55 | ✅ 完成 | 已盘点当前 `Sunset` 长期线程、真实分支、现有 worktree 与 Codex 状态库中的 `cwd/git_branch` 现状。 |
| T56 | ✅ 完成 | 已固化 A 类治理线程留在根目录 `main`、B 类功能线程进入独立 worktree 的映射原则。 |
| T57 | ✅ 完成 | 已创建项目内唯一有效的 `线程分支对照表.md`。 |
| T58 | ✅ 完成 | 已创建 `D:\Unity\Unity_learning\Sunset_worktrees` 作为长期 worktree 根目录。 |
| T59 | ✅ 完成 | 已为 `codex/npc-generator-pipeline` 创建独立 `NPC` worktree。 |
| T60 | ✅ 完成 | 已为 `codex/farm-10.2.2-patch002` 创建独立农田 worktree。 |
| T61 | ✅ 完成 | 已复核并保护 `D:\Unity\Unity_learning\Sunset` 根目录继续绑定稳定 `main`。 |
| T62 | ✅ 完成 | 已备份并对齐 Codex 状态层中相关线程的默认 `cwd/git_branch/git_sha`。 |
| T63 | ✅ 完成 | 已把“进入线程先核验当前目录与真实分支”的纪律写入治理规则。 |
| T64 | ✅ 完成 | 已把“哪些线程允许留在 `main`、哪些绝不允许在 `main` 做实现”写入规则。 |
| T65 | ✅ 完成 | 已输出最终 `Codex` 用户使用说明、实施记录与验收口径。 |

## T55: 盘点当前 `Sunset` 长期线程、真实分支、worktree 与状态库现场

**状态**: ✅ 完成

**目标**:
- 在动手创建 worktree 之前，先把“现在到底有哪些线程、哪些分支、哪些已有 worktree、哪些线程的 `cwd` 还停在根目录”盘清楚，避免二次误配。

**本轮已完成**:
- 已确认根目录真实分支为 `main`，且当前根工作树干净。
- 已确认本地存在并可用的关键分支：`main`、`codex/npc-generator-pipeline`、`codex/farm-10.2.2-patch002`、`codex/restored-mixed-snapshot-20260311`。
- 已确认既有特殊 worktree 仅剩 `.claude/worktrees/agent-a2df3da0`。
- 已核对 `C:\Users\aTo\.codex\state_5.sqlite` 的 `threads` 表，确认 `cwd`、`git_branch`、`git_sha` 可作为本轮线程默认目录对齐入口。

## T56: 固化治理线程 / 功能线程映射原则

**状态**: ✅ 完成

**目标**:
- 正式把 `Sunset` 的长期线程划分为两类：
  - A 类：治理 / 总览 / 只读 / 审计线程，固定根目录 `main`
  - B 类：独立功能实现线程，固定各自 `codex/*` 分支与独立 worktree

**本轮已完成**:
- 已正式采纳 A / B 两类线程划分。
- 已明确：`Codex规则落地`、`spring-day1`、`项目文档总览`、`导航检查`、`遮挡检查` 留在根目录 `main`。
- 已明确：`NPC` 与 `农田交互修复V2` 进入各自独立 worktree。

## T57: 创建唯一有效的 `线程分支对照表.md`

**状态**: ✅ 完成

**目标**:
- 在项目内落一份长期可查的对照表，明确线程名、默认工作目录、默认分支、适用边界与例外说明。

**本轮已完成**:
- 已创建 `D:\Unity\Unity_learning\Sunset\.codex\threads\线程分支对照表.md`
- 已把线程类型、默认目录、默认分支、特殊说明与红线写清。

## T58: 创建长期 worktree 根目录

**状态**: ✅ 完成

**目标**:
- 在仓库外侧创建 `D:\Unity\Unity_learning\Sunset_worktrees`，作为 `Sunset` 长期功能线程的统一物理目录根。

**本轮已完成**:
- 已创建 `D:\Unity\Unity_learning\Sunset_worktrees`

## T59: 为 `codex/npc-generator-pipeline` 创建独立 `NPC` worktree

**状态**: ✅ 完成

**目标**:
- 让 NPC 线程拥有独立文件视图，不再与根目录 `main` 共享同一份工作树。

**本轮已完成**:
- 已创建 `D:\Unity\Unity_learning\Sunset_worktrees\NPC`
- 已确认该目录真实分支为 `codex/npc-generator-pipeline`
- 已确认该目录 HEAD 为 `40493346`

## T60: 为 `codex/farm-10.2.2-patch002` 创建独立农田 worktree

**状态**: ✅ 完成

**目标**:
- 让农田补丁线程拥有独立文件视图，不再通过根目录反复切分支。

**本轮已完成**:
- 已创建 `D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002`
- 已确认该目录真实分支为 `codex/farm-10.2.2-patch002`
- 已确认该目录 HEAD 为 `a47da9e1`

## T61: 复核并保护根目录只绑定稳定 `main`

**状态**: ✅ 完成

**目标**:
- 确保 `D:\Unity\Unity_learning\Sunset` 继续作为治理与联调主路，只承载稳定 `main`，不再承载长期功能分支实现。

**本轮已完成**:
- 已确认根目录 `D:\Unity\Unity_learning\Sunset` 当前真实分支仍为 `main`
- 已确认根目录当前未被切到 NPC 或农田功能分支
- 已把“根目录只承载稳定 `main`”回写到规则与对照表

## T62: 对齐 Codex 状态层中的默认 `cwd/git_branch`

**状态**: ✅ 完成

**目标**:
- 先备份 `C:\Users\aTo\.codex\state_5.sqlite`，再把相关 `Sunset` 活跃线程的默认目录对齐到正确 worktree，减少线程重新打开时继续抢占根目录的概率。

**本轮已完成**:
- 已备份 `C:\Users\aTo\.codex\state_5.sqlite.bak-20260311-183352-sunset-worktree-routing`
- 已将相关活跃线程的默认 `cwd` / `git_branch` / `git_sha` 对齐：
  - 治理类线程 -> `D:\Unity\Unity_learning\Sunset` / `main`
  - NPC 线程 -> `D:\Unity\Unity_learning\Sunset_worktrees\NPC` / `codex/npc-generator-pipeline`
  - 农田线程 -> `D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002` / `codex/farm-10.2.2-patch002`

## T63: 写入“进入线程先核验目录 / 分支”纪律

**状态**: ✅ 完成

**目标**:
- 把“UI 分支提示不等于真实 Git，进入线程先核验 `cwd` 与 `git branch --show-current`”写入治理规则，作为固定闸门。

**本轮已完成**:
- 已把该纪律写入 `.kiro/steering/git-safety-baseline.md`
- 已把该纪律接入项目 `AGENTS.md`

## T64: 写入 `main` 与功能线程边界规则

**状态**: ✅ 完成

**目标**:
- 把“哪些线程允许留在 `main`、哪些线程绝不允许在 `main` 做实现”正式写成规则，避免后续再次回到多线程共用根目录开发。

**本轮已完成**:
- 已把 A 类 / B 类线程边界写入 `.kiro/steering/git-safety-baseline.md`
- 已在 `AGENTS.md` 中补充“长期功能线程默认进入独立 worktree”的路由口径

## T65: 输出 `Codex` 用户使用说明与验收口径

**状态**: ✅ 完成

**目标**:
- 产出一份用户可直接照着做的说明，写清楚：
  - 每个线程应该进哪个目录
  - 进入后先看什么
  - 如果目录或分支不对该怎么处理
  - 当前不允许提前做的动作（尤其是 NPC 合并到 `main`）

**本轮已完成**:
- 已创建 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\Codex迁移与规划\Codex线程Worktree使用说明.md`
- 已创建 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\Codex迁移与规划\Sunset线程Worktree治理实施记录_2026-03-11.md`
