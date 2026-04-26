# 项目文档总览 - 线程活跃记忆

> 2026-04-10 起，旧线程母卷已归档到 [memory_1.md](D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/项目文档总览/memory_1.md)。本卷只保留当前线程角色和恢复点。

## 线程定位
- 线程名称：`项目文档总览`
- 线程作用：母卷协调、长期主文总索引、memory 治理收口入口

## 当前主线
- 当前最重要的事情不再是继续无限吸收材料，而是：
  - 维护母卷与长期主文的入口清晰
  - 继续执行 memory 治理
  - 把已吸收结论回写到长期主文
- 当前批次入口：
  - [05_第五批次_工程化与memory治理](D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/项目文档总览/05_第五批次_工程化与memory治理/README.md)

## 当前稳定结论
- `项目文档总览` 线程不再充当第二份超长母库
- 详细母材料、专项盘点、三维度母卷和 memory 治理过程都应回到工作区文档本体
- 2026-04-10 已完成第一轮核心病态 workspace / thread memory 真分卷，后续应转入抽检与维持，而不是再回到泛盘点
- `Codex规则落地` 已补齐索引层，说明这条治理样板现在不是“半截分卷”，而是可长期沿用的完整样板

## 当前恢复点
- 查旧完整盘点和历史长链时看 `memory_1.md`
- 继续总览治理时优先看：
  - [D:/Unity/Unity_learning/Sunset/.kiro/specs/项目文档总览/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/项目文档总览/memory.md)

## 2026-04-22｜数据生产、配置体系与内容接线链只读审计
- 当前主线：
  - 用户要求只读审计 `Sunset` 的数据生产、配置体系与内容接线链，重点回答 Item / Recipe / WorldPrefab / ScriptableObject / 配置资产 / 场景合同 / 内容接线链的真实结构，以及 Editor 工具、批量生成与校验机制如何支撑内容生产。
- 本轮完成：
  1. 已读并交叉核对：
     - `Docx\大总结\Sunset_持续策划案\06_工具链.md`
     - `.kiro\about\01_系统架构与代码全景.md`
     - `.kiro\specs\项目文档总览\2026-04-09_Codex工作区_AI工作流_MCP_skills_工具全盘点_01.md`
  2. 已核代码与资产：
     - `Assets\YYY_Scripts\Data\Items\ItemData.cs`
     - `Assets\YYY_Scripts\Data\Items\*.cs`
     - `Assets\YYY_Scripts\Data\Recipes\RecipeData.cs`
     - `Assets\YYY_Scripts\Data\Database\ItemDatabase.cs`
     - `Assets\YYY_Scripts\Data\Core\PrefabDatabase.cs`
     - `Assets\YYY_Scripts\Data\Core\PrefabRegistry.cs`
     - `Assets\Editor\WorldPrefabGeneratorTool.cs`
     - `Assets\Editor\DatabaseSyncHelper.cs`
     - `Assets\Editor\PrefabDatabaseAutoScanner.cs`
     - `Assets\Editor\PrefabDatabaseEditor.cs`
     - `Assets\Editor\Tool_BatchRecipeCreator.cs`
     - `Assets\Editor\Home\HomeSceneRestContractMenu.cs`
     - `Assets\Editor\Town\TownSceneEntryContractMenu.cs`
     - `Assets\Editor\Town\TownScenePlayerFacingContractMenu.cs`
     - `Assets\Editor\Story\SpringDay1TownAnchorContractMenu.cs`
     - `Assets\Editor\Story\SpringDay1DirectorTownContractMenu.cs`
     - `Assets\Editor\Story\SpringDay1DirectorPrimaryRehearsalBakeMenu.cs`
     - `Assets\Editor\Story\SpringDay1DirectorPrimaryLiveCaptureMenu.cs`
     - `Assets\Editor\Story\SpringDay1LiveSnapshotArtifactMenu.cs`
     - `Assets\Editor\Story\SpringDay1TargetedEditModeTestMenu.cs`
     - `Assets\Editor\Story\SpringUiEvidenceMenu.cs`
     - `Assets\111_Data\Database\MasterItemDatabase.asset`
     - `Assets\111_Data\Database\PrefabDatabase.asset`
  3. 已确认的关键事实：
     - Item 系统是 `ItemData` 基类 + 多个子类 SO 结构，ID / 分类 / 放置 / 装备 / 消耗 / 世界预制体等都在同一条数据模型里。
     - `ItemDatabase.asset` 当前有 71 个物品引用，`allRecipes` 只有 3 个空槽，说明配方结构存在，但实际配方资产尚未落满。
     - `Assets\111_Data\Recipes` 当前没有扫到真实 `RecipeData.asset`，这是一条重要的“链路能力 ≠ 资产已铺满”的边界。
     - `PrefabDatabase.asset` 当前有 81 个预制体条目，且已写入 `Storage_1400... -> Box_1/Box_2/Box_3/Box_4` 与 `Chest -> Box_1` 的别名兼容。
     - `PrefabRegistry.asset` 仍在，但代码层已经明确标为废弃兼容层。
     - `WorldPrefabGeneratorTool` 的真实作用是从 `ItemData.icon` 批量生成世界预制体并保存为 prefab，而不是一个通用世界系统本体。
     - 场景合同不是抽象口号，而是以 `Town/Home/SpringDay1` 的只读 probe、anchor contract、transition trigger 和导演 bake/live capture 菜单的形式存在。
- 当前稳定结论：
  1. 技术策划的价值主要体现在：把内容拆成可约束的 SO 字段、把 ID 和类别做成可批量维护的规范、把世界预制体和场景接线做成可扫描 / 可校验链路。
  2. 这条链已经能证明“生产效率”和“接线正确性”有正式工具支撑，但不能据此反推“所有内容都已完整落地”。
  3. 当前最稳的对外表述应是“有成熟的数据生产与接线工具链，且局部资产与场景合同已落地”，而不是“全量内容都已完成闭环”。
- 当前恢复点：
  - 后续如果继续这条线，优先补：`RecipeData` 真实资产分布、`WorkstationData.recipeListRef` 的实际接线、以及 `spring-day1` 场景合同工具和通用内容工具之间的边界。

## 2026-04-10｜第一批吸收结论已回写 01/04/07/08
- 当前主线：
  - 把第一批 memory 吸收结论正式回写进长期主文，而不是继续做 memory survey。
- 本轮完成：
  1. 更新：
     - `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\01_总览.md`
     - `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\04_剧情NPC.md`
     - `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\07_AI治理.md`
     - `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\08_进度总表.md`
  2. 把 `day1 / UI / NPC / 导航 / 存档 / Codex规则落地` 的最新阶段判断写回长期主文。
- 当前稳定结论：
  - 这轮之后，`项目文档总览` 手里已经有一套可直接沿用的主文母口径：
    - `day1` = 主线已成、导演尾差与 runtime 终验待补
    - `UI` = 玩家面集成与刷新止血已落地，live 终验待补
    - `NPC` = resident 合同已站住，剩最终并面验证
    - `导航` = 三层责任已压实，driver 尾差待收
    - `存档` = 三场景 persistent baseline 已落地，live 门链待补
    - `Codex规则落地` = 治理层，不代替业务主文
- 验证与状态：
  - `git diff --check` 已过
  - 当前 slice 已 `Park-Slice`
  - 当前 thread-state = `PARKED`
- 当前恢复点：
  - 若继续本线，下一轮优先补仍带旧阶段口径的长期主文卷，不再回到泛盘点。

## 2026-04-10｜简历素材母卷已建立
- 当前主线：
  - 将 `项目文档总览` 线程从“持续扩项目总览”切回用户真正需要的主线：为简历产出建立可直接使用的素材母卷。
- 本轮完成：
  1. 在外部目录建立：
     - `D:\QQ\Project\综合实训三NEW\刀\简历素材母卷`
  2. 已落下 `00 ~ 09` 共 10 份材料，其中最关键的是：
     - `01_项目事实基线.md`
     - `07_量化事实、慎写边界与简历句库.md`
     - `09_简历积木母本_直取版.md`
  3. `09` 已按 5 大类直取组织：
     - 自我评价
     - 项目经历
     - 技能掌握
     - 游戏经历
     - 游戏拆解
- 当前稳定结论：
  1. 这轮之后，这个线程不需要再把“项目理解”继续无限扩成另一份总览，而是已经拥有可直接服务简历的外部母库。
  2. 若用户下一步要一页策划实习简历，最稳入口就是：
     - `09_简历积木母本_直取版.md`
     - `07_量化事实、慎写边界与简历句库.md`
     - `01_项目事实基线.md`
- 验证与状态：
  - 本轮为 docs-only / external-docs-only
  - 未改代码、未改 Scene、未改 Prefab
- 当前恢复点：
  - 若继续本线，直接从外部母卷压简历，不再回到泛项目总览扩写。

## 2026-04-10｜简历初稿两版已落地
- 当前主线：
  - 从外部母卷继续压到可直接投递的简历正文。
- 本轮完成：
  1. 新增：
     - `D:\QQ\Project\综合实训三NEW\刀\简历素材母卷\10_策划实习简历初稿_通投版.md`
     - `D:\QQ\Project\综合实训三NEW\刀\简历素材母卷\11_策划实习简历初稿_技术型策划版.md`
  2. 两版都已基于 `01 + 07 + 09` 这组稳写母料压出完整结构，用户可直接删改。
- 当前稳定结论：
  1. 这条线现在已经从“素材母卷”推进到“正文初稿”层。
  2. 下一步不再需要继续扩分析，只需要按岗位继续收字数、换重心。
- 验证与状态：
  - 本轮为 docs-only / external-docs-only
  - 当前 slice 已 `Park-Slice`
  - 当前 thread-state = `PARKED`
- 当前恢复点：
  - 若继续本线，优先从 `10 / 11` 继续压最终投递版。

## 2026-04-10｜简历优化方向进一步校正
- 当前主线：
  - 沿用户现有简历框架深改，而不是继续自造一版更通用的结构。
- 本轮稳定结论：
  1. 用户更认可“现有框架 + 深度精修”的方案。
  2. 项目经历仍要以主控权、主线收口、NPC resident 化、连续世界体验、Unity 工具链、AI 治理为主轴。
  3. 游戏经历与拆解必须继续从“时长堆叠”往“品类认知 + 设计反哺”转。
- 当前恢复点：
  - 若继续本线，直接在用户当前这份分类框架上打最终投递稿。

## 2026-04-16｜《Sunset》项目简历表达重审：回到当前现场，不再沿用旧黑话
- 当前主线：
  - 用户明确否决旧版《Sunset》项目文案，要求回到当前项目现场、线程、母卷与关键代码，重新判断“项目简介 + 核心成果条目”该怎么写。
- 本轮完成：
  1. 重新核对了当前 live 事实入口：
     - `spring-day1 / NPC / 存档系统` 活跃 memory
     - 项目总览与系统代码全景
     - `spring-day1 / NPC / 存档系统 / 项目文档总览` 线程记忆
     - 简历母卷 `01 / 05 / 06 / 07 / 09`
  2. 额外核了关键代码触点：
     - `SpringDay1Director.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - `SaveManager.cs`
     - `NPCAutoRoamController.cs`
  3. 回收了 3 条 `gpt-5.4` 子代理结论，分别覆盖：
     - `spring-day1 + NPC` 现状与慎写口径
     - `存档 / 持久化 / UI / 剧情绑定`
     - `AI 并行开发治理与可写边界`
- 当前稳定结论：
  1. 项目条目不能再重复“核心能力”里已经写过的工具链和 AI 流程清单；项目部分必须改成“你把这些能力拿去做成了什么”。
  2. 《Sunset》当前最值钱的项目亮点，不是“做了很多系统”，而是：
     - 你把导演 / 剧情、放置 / 耕地、背包 / 箱子 / UI、时间 / 季节、存档 / 恢复、玩家跨场景承接、NPC resident / 漫游 / 关系、树石 / 掉落 / 作物等系统，真正接成了 `Town -> Primary -> Home` 的可运行 Day1 主线切片。
  3. “连续世界与聚落秩序”这条不能再写成：
     - `resident 化`
     - `formal 一次性消费`
     - `runtime 语义`
     这类项目黑话；
     正确方向应翻译成招聘方能看懂的结果，例如：
     - 跨场景状态继承
     - 存档 / 读档 / 场景切换不再像“重开”
     - NPC 从一次性剧情触发器，进入可持续生活、可持续反馈的居民结构
  4. AI 相关也不能再写成 hooks / steering / memory / thread-state 等内部机制名；真正可写的是：
     - 以策划主控身份组织多 Agent 并行开发
     - 持续做需求定义、范围裁定、语义拍板、回执审核、黑盒回归和终验回拉
     - 把项目推进保持在可控边界内
  5. 存档 / 持久化的正确写法也已校准：
     - 可以写“注册式持久化架构、scene-aware save/load、Day1 长期态正式入槽位、跨场景玩家与 resident 承接”
     - 不能写“完整通用全世界态持久化”或“所有状态都已完全入存档”
- 当前恢复点：
  - 下一轮如果继续本线，不该再从旧版 `26.03.23` 那批简历成稿直接微调；
  - 应直接基于这轮重审结果重写《Sunset》的：
    1. 项目简介
    2. 核心成果标题
    3. 每条 bullet 的“问题 -> 动作 -> 结果”表达

## 2026-04-16｜经营成长 + 交互系统 + 运行时承接：只读系统全景审计
- 当前主线：
  - 用户要求只读分析，不写简历；专门审计 `Sunset` 当前“经营成长 + 交互系统 + 运行时承接”的系统全景，并按真正运行时链路重组结论。
- 本轮完成：
  1. 已读主文入口：
     - `02_经营成长.md`
     - `03_交互系统.md`
     - `08_进度总表.md`
     - `.kiro/about/01_系统架构与代码全景.md`
  2. 已补读相关工作区记忆：
     - `项目文档总览`
     - `制作台系统`
     - `物品放置系统`
     - `存档系统`
     - `农田系统`
  3. 已核关键代码 / 资产：
     - `GameInputManager.cs`
     - `InventoryService.cs`
     - `PlacementManager.cs`
     - `SaveManager.cs`
     - `PersistentPlayerSceneBridge.cs`
     - `ChestController.cs`
     - `CraftingService.cs`
     - `CraftingStationInteractable.cs`
     - `SpringDay1WorkbenchCraftingOverlay.cs`
     - `FarmTileManager.cs`
     - `CropController.cs`
     - `CropManager.cs`
     - `BoxPanelUI.cs`
     - `InventoryInteractionManager.cs`
     - `MasterItemDatabase.asset`
  4. 已稳定形成的重分类主轴：
     - 时间 / 季节驱动链
     - 库存 / Hotbar / 手持真值链
     - 世界目标选择与交互优先级链
     - 放置 / 农田执行链
     - 资源转化与工作台 / 箱子链
     - 正式存档与跨场景 runtime 承接链
- 稳定判断：
  1. `Sunset` 当前真正的复杂度来源不是“模块数量”，而是共享状态语义和跨系统承接。
  2. `GameInputManager` 实际是交互总调度器：对话接管时会统一关输入、停农田链、退放置、停导航、关箱子，并在结束后恢复部分状态。
  3. `InventoryService` 已经被并入时间链与持久化链，而不是纯背包容器。
  4. `PlacementManager` 已是完整状态机，且“面板打开 = 暂停并隐藏预览，不等于中断链路”这层语义已经站住。
  5. `SaveManager + PersistentPlayerSceneBridge` 已形成双持久化层：前者做正式 save/load，后者做跨场景 runtime continuity 与 scene world restore。
  6. 制作链存在明确双轨：
     - 通用 `CraftingService` 依赖 `ItemDatabase.allRecipes`
     - `spring-day1` 工作台专用 overlay 直接从 `Resources/Story/SpringDay1Workbench` 读配方
  7. `RecipeData` 资产已存在但 `MasterItemDatabase.asset` 的 `allRecipes` 仍为空槽，说明“局部闭环已通”与“系统总数据库已闭环”必须区分。
  8. `ChestController` 的复杂度不在“能开箱子 UI”，而在它同时是 `IResourceNode + IInteractable + IPersistentObject`，并桥接 legacy `ChestInventory` 与 authoritative `ChestInventoryV2`。
- 当前恢复点：
  - 若继续本线，应把这次运行时重分类择要回灌到主文与后续对外表达母本；
  - 不再按旧目录平铺系统，而要优先按运行时主链解释项目复杂度。

## 2026-04-23｜README 与项目叙事历史演变只读盘点

- 用户目标：
  - 在不改文件的前提下，只读梳理 `Sunset` 当前 README 的主要问题、README 可直接复用的高价值表达、首页与下沉文档的分层方式，以及一版真正可投递 / 可开源展示的 README 章节骨架。
- 本轮完成：
  1. 已确认根 `README.md` 仍是“`README 草案 v2（可落根版）`”，而且正文里自己也把“如果后续落到仓库根 README.md”写成未来式，说明当前并非最终对外页。
  2. 已抓出当前最强表达源：
     - `2026-04-16_Sunset项目表达骨架版_01.md`
     - `2026-04-08_给外部简历智能体_Sunset项目素材总包_01.md`
     - `2026-04-08_给外部简历智能体_Sunset三维度母卷_01.md`
     - `Docx/大总结/Sunset_持续策划案/01_总览.md`
  3. 已补看三份 `.kiro/about/*` 与 `History` 总索引，确认仓库里已经存在成熟的阅读地图、系统架构、接手指南和历史 handoff 入口，README 不需要再自己承载这些长内容。
  4. 已补看更早期总文：
     - `Docx/分类/全局/000_项目全局文档.md`
     - `Docx/大总结/第一阶段完结报告.md`
     用来确认项目叙事从“完成度 + 系统清单”一路演变到“跨场景主线、连续世界、居民化、项目化推进”的轨迹。
- 关键决策：
  1. README 首页今后应优先讲：
     - 项目是什么
     - 为什么不是普通 Demo
     - 目前有哪些可感知亮点
     - 现在处在哪个阶段
     - 去哪里继续读
  2. `.kiro/about/00~02` 应继续承担：
     - 阅读地图
     - 架构全景
     - 接手指南 / 漂移点
     而不是把这些接手向细节塞进首页首屏。
  3. “我的作用 / AI 协作治理”应保留，但必须晚于项目本体，并控制在差异化补充位，不能抢占首页 opening。
  4. 公开展示版 README 还必须补当前草案没有真正补齐的外部读者要素：
     - GIF / 截图 / Demo
     - 最小运行入口
     - 依赖与打开前提
     - License / 公开策略
- 验证状态：
  - 本轮是只读分析，未进入真实施工，因此未跑 `Begin-Slice`。
  - 未改任何 tracked 文件。
- 下一步恢复点：
  - 若用户下一轮要真正开工，直接按“首页内容 / 下沉内容 / 章节骨架”三层方案施工即可。
