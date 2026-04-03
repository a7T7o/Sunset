# Sunset 项目内容全清单 V1

> 文档目的：先把 `Sunset` 里“到底已经有哪些工作区、模块、设计块、规则块、线程块、文档块、运行时系统块、工具链块”完整列出来。  
> 这份清单不是最终策划案，也不是最终设计提炼稿，而是“全盘盘点地图”。  
> 当前重点：先解决“该去哪里找、已经有哪些、缺哪些入口”。

---

## 一、当前我已经确认的顶层版图

### 1. `.kiro/specs` 顶层工作区

- `000_Gemini`
- `000_代办`
- `000_代办清扫`
- `001_BeFore_26.1.21`
- `900_开篇`
- `999_全面重构_26.01.27`
- `999_全面重构_26.03.15`
- `Codex规则落地`
- `NPC`
- `SO设计系统与工具`
- `Steering规则区优化`
- `UI系统`
- `Z_光影系统`
- `存档系统`
- `共享根执行模型与吞吐重构`
- `技能等级系统`
- `农田系统`
- `屎山修复`
- `物品放置系统`
- `箱子系统`
- `云朵遮挡系统`
- `制作台系统`

### 2. `.codex/threads/Sunset` 当前线程

- `019d4d18-bb5d-7a71-b621-5d1e2319d778`
- `backup-script`
- `Codex规则落地`
- `NPC`
- `NPCV2`
- `Skills和MCP`
- `spring-day1`
- `spring-day1V2`
- `UI`
- `场景搭建（外包）`
- `导航检查`
- `导航检查V2`
- `农田交互修复V2`
- `农田交互修复V3`
- `树石修复`
- `项目文档总览`
- `遮挡检查`

### 3. `Docx` 顶层知识库

- `000_任务列表`
- `Memory`
- `Plan`
- `大总结`
- `分类`
- `全局场景层级`
- `设计`

### 4. `Assets` 里已经成体系的主要内容块

- 运行时代码：`Assets/YYY_Scripts`
- 编辑器工具：`Assets/Editor`
- 数据资产：`Assets/111_Data`
- 预制体资产：`Assets/222_Prefabs`
- 场景：`Assets/000_Scenes`
- 外部包与素材源：`Assets/ZZZ_999_Package`

---

## 二、`.kiro/specs` 工作区总览地图

## 1. `000_Gemini`

- 规模：`13` 个目录，`83` 份文档。
- 顶层块：
  - `000`
  - `1.0.0策划`
  - `2.0.0龙虾养殖`
- 里面已经存在的内容：
  - 一组早期规则文档、交接文档、审计文档。
  - 一整套剧本篇章集，按 `Deepseek / Gemini / 豆包` 多版本分卷保存。
  - `001_剧本篇章集`，说明这里沉淀过剧情文本、多模型写作对照与审核。
  - `002_事件编排重构`，说明这里不只是写剧情，还写过事件编排设计。
  - `Gemini三卷审视报告.md`、`审核001.md`、`回应001.md` 这类文件，说明这里还有评审层与纠偏层。
  - `2.0.0龙虾养殖` 里有 `龙虾操作手册.md`、`龙虾最小运行闭环.md`、`验收指南.md`，这是外部多智能体/工具流的另一条线。
- 你如果要找：
  - 早期剧情原文、分镜脚本、事件编排、Gemini/Deepseek/豆包多轮对照，这里是高概率入口。

## 2. `000_代办`

- 规模：`15` 个目录，`40` 份文档。
- 顶层块：
  - `codex`
  - `kiro`
- 里面已经存在的内容：
  - `codex/README.md`
  - 一批 `TD_XX_*.md`，本质上是总代办、阶段收尾、规则治理收口。
  - `kiro` 下又按旧工作区镜像分类：`UI系统`、`农田系统`、`箱子系统`、`物品放置系统`、`技能等级系统`、`SO设计系统与工具` 等。
- 这块的性质：
  - 它更像“总代办镜像”和“旧索引层”，不是最终业务设计正文，但能快速告诉你曾经哪些主线被明确排过优先级。

## 3. `000_代办清扫`

- 规模：`1` 个目录，`2` 份文档。
- 内容：
  - `系统审查报告.md`
  - `memory.md`
- 性质：
  - 更像一次对总代办体系本身的清扫与审查。

## 4. `001_BeFore_26.1.21`

- 规模：`40` 个目录，`107` 份文档。
- 这是一个非常重要的“早期系统设计母库”。
- 顶层子块：
  - `背包图标旋转`
  - `背包注入系统优化`
  - `编辑器工具`
  - `导航系统重构`
  - `动画系统`
  - `工具动画重构`
  - `砍树系统`
  - `砍树系统优化`
  - `矿石系统`
  - `全局调试系统`
  - `全局系统`
  - `摄像头死区同步`
  - `世界物品系统`
  - `树木系统`
  - `树木阴影控制`
  - `物品掉落拾取`
  - `物品显示尺寸统一`
  - `遮挡与导航`
- 已明确存在的设计/模块：
  - 导航与右键交互架构重构。
  - 放置系统导航修复。
  - 导航阶段路线图、技术文档、验收指南。
  - 工具动画重构与动画系统。
  - 树木系统、树木阴影控制、砍树系统、矿石系统。
  - 世界物品系统、掉落拾取、显示尺寸统一。
  - 背包系统、背包图标、注入系统。
  - 摄像头死区同步。
  - 遮挡与导航联动。
- 这块的价值：
  - 你现在很多系统的“原始设计语义”和“第一代结构思路”，大概率都在这里。

## 5. `900_开篇`

- 规模：`11` 个目录，`65` 份文档。
- 顶层子块：
  - `0.0阶段`
  - `spring-day1-implementation`
- `0.0阶段` 已存在：
  - `0.0.1剧情初稿`
  - `0.0.2初步落地`
  - `0.0.3V2`
  - 文档里明确有：
    - `春1日_坠落_格式A_时间轴版.md`
    - `春1日_坠落_格式B_分镜脚本版.md`
    - `春1日_坠落_融合版.md`
    - `初步规划文档.md`
    - `实现落地方案.md`
- `spring-day1-implementation` 已存在：
  - `000-深入交流`
  - `001-全面勘察`
  - `002-初步搭建`
  - `003-进一步搭建`
  - `requirements.md`
  - `OUT_design.md`
  - `OUT_tasks.md`
  - `scene-build_handoff.md`
  - 大量委托书、锐评、审视、UI 样式快照、验收与任务拆分文档。
- 这块说明：
  - 你的 Day1 不只是一个功能点，而是一整条“剧情初稿 -> 规划 -> 需求拆分 -> 初步搭建 -> 进一步搭建 -> UI 样式收口”的主线。

## 6. `999_全面重构_26.01.27`

- 规模：`14` 个目录，`112` 份文档。
- 顶层子块：
  - `0_初步工作`
  - `1_入门工作`
  - `2_完善工作`
  - `3_微调`
  - `3_装备与工具`
  - `3.5_终极技术债务清算行动`
  - `3.6债务清算后的清扫`
  - `3.7.1玩家瞬移解决`
  - `3.7.2存档bug大清扫`
  - `3.7.3消失bug`
  - `3.7.4进一步bug清扫`
  - `3.7.5自动化存档`
  - `3.7.6再再进一步bug清扫`
- 这块说明：
  - 这里不是新设计主库，而是“重构与债务清算历史层”，但里面埋着很多你对装备、工具、存档、消失 bug、自动化存档的历史推进证据。

## 7. `999_全面重构_26.03.15`

- 规模：`5` 个目录，`12` 份文档。
- 顶层子块：
  - `存档检查`
  - `导航检查`
  - `遮挡检查`
- 性质：
  - 这是更靠近“专项检查 / 审核 / 重构审计”的层，不是纯策划正文，但它能告诉你哪些系统后来被拉出来做第二次专项核查。

## 8. `Codex规则落地`

- 规模：`32` 个目录，`201` 份文档。
- 这是当前项目治理、协作规范、并发模型、分发协议、线程制度的正式工作区。
- 顶层子块非常多，主要包括：
  - `01_现行入口与活文档重构`
  - `02_skills_AGENTS_与执行机制重构`
  - `03_线程解冻与WIP收口`
  - `04_冻结文档归档与版本快照`
  - `05_四件套与代办体系重构`
  - `06_memory分卷治理与索引收紧`
  - `07_历史编码巡检与治理边界收紧`
  - `08_第二批超长活跃memory主卷治理`
  - `09_强制skills闸门与执行规范重构`
  - `10_共享根仓库分支漂移与现场占用治理`
  - `11_main-branch-only回归与worktree退役收口`
  - `12_治理工作区归位与彻底清盘`
  - `20_shared-root现场回正与物理闸机落地`
  - `21_第一次唤醒复盘与shared-root分支租约闸门`
  - `22_恢复开发分发与回收`
  - `23_前序阶段补漏审计与并发交通调度重建`
  - `24_main-only极简并发开发与scene-build迁移`
  - `25_vibecoding场景规范与main收口`
- 这里面明确存在的内容：
  - 批次分发规范。
  - 典狱长模式。
  - 看守长模式。
  - thread-state 接线尾巴。
  - AGENTS / skills / memory / hooks / 共享根治理 / main-only 模式。
- 这块不是游戏设计正文，但它是你“AI 原生策划推进与协作规范能力”的核心证据库。

## 9. `NPC`

- 规模：`4` 个目录，`37` 份文档。
- 顶层子块：
  - `1.0.0初步规划`
  - `2.0.0进一步落地`
- 已明确存在的内容：
  - `NPC场景化与集成首版收口.md`
  - `NPC-导航接入契约与联调验收规范.md`
  - `NPC场景化真实落点与角色日常设计.md`
  - `NPC交互反应与关系成长设计.md`
  - `NPC系统实施主表.md`
  - `需求拆分.md`
  - `0.0.1全面清盘` 与 `0.0.2清盘002` 下还有：
    - 三名 NPC 场景点位方案卡
    - 双气泡视觉规范
    - 轻交互验收包
    - 关系成长首版验收包
    - 非正式聊天完整交互矩阵
    - 当前阶段用户验收总包
- 这块说明：
  - 你的 NPC 线不只是“有 NPC”，而是已经有场景点位、角色日常、关系成长、正式聊天/非正式聊天、双气泡视觉、导航接入规范这些内容。

## 10. `SO设计系统与工具`

- 规模：`7` 个目录，`24` 份文档。
- 顶层子块：
  - `0_字段优化重构`
  - `1_ID规范和管理器修复`
  - `2_批量生成物品工具优化`
  - `3_ID规范全面固定`
  - `4_WP生成`
  - `5_批量修改SO参数`
  - `old`
- 已明确存在的内容：
  - `ID分配规范.md`
  - 批量生成工具优化
  - ID 规范全面固定
  - WorldPrefab 生成
  - 批量修改 SO 参数
- 这块是你“物品 / 配方 / 资产批量生产链”的重要设计与工具中台。

## 11. `Steering规则区优化`

- 规模：`24` 个目录，`146` 份文档。
- 这是项目规则层、迁移层、Hook 层、当前运行基线层的另一条大母库。
- 顶层子块：
  - `2026.02.06规则更新`
  - `2026.02.11智能加载`
  - `2026.02.14_Hook机制问题与方案`
  - `2026.02.15_代办规则体系`
  - `2026.03.02_Cursor规则迁移`
  - `2026.03.18线程并发讨论`
  - `Claude迁移与规划`
  - `Codex迁移与规划`
  - `当前运行基线与开发规则`
  - `文档归档`
- 已明确存在的内容：
  - 智能加载与规则触发。
  - Hook 机制问题与方案。
  - Cursor/Claude/Codex 迁移。
  - 当前运行基线、活跃线程总表、文件认领与锁定表、当前规范快照、现行入口总索引。
  - 大量 Codex 恢复与迁移收口归档。
- 这块是你“为什么现在项目里会有这么多线程规范、智能 Hook、memory、入口索引”的根层。

## 12. `UI系统`

- 规模：`8` 个目录，`55` 份文档。
- 顶层子块：
  - `0.0.1 SpringUI`
  - `Before03.28`
- `0.0.1 SpringUI` 已明确存在：
  - `SpringUI-Day1基线复刻与长期技术路线总方案.md`
  - `ScreenSpaceOverlay取证工具说明.md`
  - `Story向UIUE集成owner边界与派工约定.md`
- `Before03.28` 已明确存在：
  - `0_背包交互系统升级`
  - `1_背包V4飞升`
  - `2_背包交互逻辑优化`
  - `3_选中状态交互优化`
  - `4_代码审核与优化`
  - 大量 `design / requirements / tasks / 输入场景矩阵 / 交互状态表 / 验收指南 / 场景检查报告`
- 这块说明：
  - 你的 UI 不只是“改样式”，而是有背包交互、选中态、输入矩阵、SpringUI 基线复刻、Story 向 UIUE 的集成治理。

## 13. `Z_光影系统`

- 规模：`4` 个目录，`13` 份文档。
- 顶层子块：
  - `0.0.1初出茅庐`
  - `0.0.2纠正`
- 已明确存在：
  - `design.md`
  - `requirements.md`
  - `配置指南.md`
  - 一组 `锐评001/002/003`
- 说明：
  - 光影系统不是空白，已经有第一轮设计、配置与纠正历史。

## 14. `存档系统`

- 规模：`10` 个目录，`9` 份文档。
- 顶层子块：
  - `999_全面重构_26.01.27`
  - `SO设计系统与工具`
  - `农田系统`
  - `箱子系统`
- 里面已经明确存在：
  - `3.7.2存档bug大清扫`
  - `3.7.3消失bug`
  - `3.7.4进一步bug清扫`
  - `3.7.5自动化存档`
  - `3.7.6再再进一步bug清扫`
  - 农作物、箱子和 SO 相关存档子线
- 说明：
  - 你的存档并不是一个孤立系统，而是和农田、箱子、SO 资产、重构线缠在一起的。

## 15. `共享根执行模型与吞吐重构`

- 规模：`27` 个目录，`101` 份文档。
- 顶层子块：
  - `00_工作区导航`
  - `01_执行批次`
  - `02_专题分析`
- 性质：
  - 这是 shared root / 并发执行 / 多线程吞吐模型的治理工作区。
- 对项目内容盘点的价值：
  - 它不直接写游戏设计，但它决定了为什么你的项目后来会有大量线程分工、收口、接盘、最小事务窗口这些制度。

## 16. `技能等级系统`

- 规模：`1` 个目录，`4` 份文档。
- 当前可见内容偏少：
  - `memory.md`
  - `old/design.md`
  - `old/requirements.md`
  - `old/tasks.md`
- 说明：
  - 技能等级这条线存在，但目前在工作区层的展开度不高，更可能要去 `Assets/YYY_Scripts/Service/SkillLevelService.cs`、`SkillData.cs`、以及其他全局设计文档里找实际细节。

## 17. `农田系统`

- 规模：`47` 个目录，`406` 份文档。
- 这是整个项目里文档最重的一条业务工作区之一。
- 顶层可见块：
  - `2026.02.28`
  - `2026.03.01`
  - `2026.03.16`
  - `继承会话memory`
- 已明确存在的内容类型：
  - 农作物设计与完善。
  - 农 BUG 与 SO。
  - 收获 DropTable 与动画。
  - 智能交互 bug 修复。
  - 交互大清盘。
  - 静态再收口验收清单。
  - `最终交互矩阵.md`
- 说明：
  - 你要找作物、耕地、浇水、收获、农具、农田交互边界、交互矩阵，这条线是核心库。

## 18. `屎山修复`

- 规模：`4` 个目录，`98` 份文档。
- 顶层子块：
  - `导航V2`
  - `导航检查`
  - `树石修复`
  - `遮挡检查`
- 已明确存在的内容：
  - 大量导航 V2 宪法、执行日志、验收委托、责任点、验收尺。
  - 导航专业需求与架构设计。
  - 玩家/静态点/NPC 避让等专题。
  - 树石修复 memory。
  - 遮挡检查子线。
- 这块说明：
  - 如果你要找“导航、避让、静态点、终点 blocker、树石联动、遮挡检查”这类后来期硬收口内容，这里非常关键。

## 19. `物品放置系统`

- 规模：`11` 个目录，`50` 份文档。
- 顶层子块：
  - `0_V1版本实现`
  - `1_可放置物品SO设计`
  - `2_放置系统重构`
  - `3_放置系统V3重构`
  - `4_放置背包联动`
  - `4_放置系统Bug修复与规则完善`
  - `5_放置系统Bug修复与规则完善`
  - `6_预览与放置位置对齐修复`
  - `7_可放置物品开发规范`
  - `8_放置系统多问题修复`
  - `old`
- 已明确存在：
  - `放置系统完整规则汇总.md`
  - `可放置物品位置处理规则.md`
  - `问题分析报告.md`
  - `全面分析报告-V2.md`
  - `用户方案记录.md`
  - 多轮 `design / requirements / tasks / 验收指南`
- 说明：
  - 放置系统不仅有实现演进，还有规则汇总、开发规范、背包联动、预览对齐与 bug 修复历史。

## 20. `箱子系统`

- 规模：`14` 个目录，`61` 份文档。
- 顶层子块：
  - `0_箱子系统核心功能`
  - `1_箱子样式与交互完善`
  - `2_箱子与放置系统综合修复`
  - `3_箱子UI系统完善`
  - `4_箱子UI交互完善`
  - `5_箱子UI交互完善V2`
  - `6_锐评修复V3`
  - `7_终极清算`
  - `8_战后清扫`
  - `code-reaper-reviews`
  - `design`
  - `old`
  - `phases`
- 已明确存在：
  - `锁系统设计.md`
  - `drop-and-placement.md`
  - `interaction-flow.md`
  - `lock-and-ownership.md`
  - `storage-and-types.md`
  - `ui-and-panel-integration.md`
  - `交互场景矩阵.md`
- 说明：
  - 箱子系统已经拥有独立的锁、交互流、放置联动、存储类型、UI 接板设计。

## 21. `云朵遮挡系统`

- 规模：`1` 个目录，`4` 份文档。
- 当前可见内容偏旧：
  - `memory.md`
  - `old/design.md`
  - `old/requirements.md`
  - `old/tasks.md`
- 说明：
  - 云朵遮挡是存在的一条设计线，但它很可能后续被合并进 `遮挡检查`、`Docx/分类/遮挡与导航` 或运行时渲染脚本里。

## 22. `制作台系统`

- 规模：`1` 个目录，`4` 份文档。
- 当前可见内容：
  - `memory.md`
  - `old/design.md`
  - `old/requirements.md`
  - `old/tasks.md`
- 说明：
  - 制作台工作区本身展开度不高，但制作台真正的后续内容已经明显并入：
    - `900_开篇/spring-day1-implementation`
    - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
    - `Assets/YYY_Scripts/Service/Crafting`
    - `Assets/YYY_Scripts/UI/Crafting`

---

## 三、线程层内容清单

## 1. `Codex规则落地` 线程

- `memory_0.md` 到 `memory_6.md`
- 这是治理主线线程，记录了规范、skills、memory、main-only、共享根并发、典狱长/看守长等内容的推进历史。

## 2. `NPC` 线程

- 有 `memory_0.md`
- 有 `V2交接文档` 六件套：
  - 交接总纲
  - 线程身份与职责
  - 主线与支线迁移编年
  - 关键节点/分叉路/判废记录
  - 用户习惯/长期偏好/协作禁忌
  - 证据索引/必读顺序/接手建议
- 还有多轮回执与认定书。

## 3. `NPCV2` 线程

- 有总需求总表与时间线。
- 有多轮“全局警匪定责清扫”执行书和回执。
- 说明 NPC 线后期接盘与归仓过程很重。

## 4. `Skills和MCP` 线程

- `memory_0.md`
- 这条线程记录了 Skills、本地化 skill、Unity MCP、旧桥清退、新桥基线、MCP 迁移与验证闭环。

## 5. `spring-day1` 与 `spring-day1V2` 线程

- 存在多轮回执、执行书、接盘记录。
- 有自己的 `V2交接文档` 六件套。
- 说明 Day1 线后期不是一次性开发，而是多轮接盘、清扫、收口、分化为 UI / Story / Prompt / 字体等子问题。

## 6. `UI` 线程

- 当前可见 `memory_0.md`。
- UI 大量细节内容更多散落在 `场景搭建（外包）` 线程和 `UI系统` 工作区。

## 7. `场景搭建（外包）` 线程

- 保存了大量对 UI 线程的任务书、审核报告、进度快照、最终收尾 prompt。
- 说明你后来把某些 UI/Scene 收口明确分派出去过。

## 8. `导航检查` 与 `导航检查V2` 线程

- `导航检查` 有：
  - `1.0.0初步检查`
  - 导航代码索引
  - 导航全盘分析报告
  - 现状核实与差异分析
  - 审计阶段结案与移交建议
- `导航检查V2` 有：
  - 多轮清扫执行书
  - `PAN大闭环开发日志`
- 说明导航后来已经进入专项检查、专项修复、专项验收层。

## 9. `农田交互修复V2 / V3`

- 都有 `memory_0.md`
- V2 有 V2 交接文档六件套。
- V3 有多轮清扫与接盘回执。
- 说明农田线也经历了后期的专项修复与归仓。

## 10. `树石修复`

- 当前可见 `memory_0.md`
- 与 `屎山修复/树石修复`、`TreeController/StoneController` 形成对应。

## 11. `项目文档总览`

- 这条线程本身就是“简历 / README / 项目介绍 / 面试稿 / 现在这份全清单”的产出总线。
- 当前已存在：
  - 第一批次委托与分析
  - 第二批次成品初稿
  - 第三批次可落根收口
  - 第四批次验收总结与落根决策
  - 多轮共享根大扫除、回执、README、项目介绍文案

## 12. `遮挡检查`

- 有初步检查目录。
- 已明确存在：
  - `遮挡系统代码索引与调用链.md`
  - `遮挡-导航-命中一致性与性能风险报告.md`
  - `遮挡现状核实与差异分析（Codex视角）.md`

---

## 四、`Docx` 知识库内容清单

## 1. `Docx/Plan`

- `000_设计规划完整合集.md`
- `UI设计指导.md`
- `UI系统与总系统设计规划.md`
- `第二阶段初步规划.md`
- `砍树功能实现方案.md`
- `农田系统-超详细实施清单.md`
- `农田系统-快速开始.md`
- `世界物体设计.md`
- `种田系统实施指南.md`
- `种田系统完整设计方案.md`
- `总设计.md`

这部分说明：早期总体设计、种田、世界物体、UI 规划、第二阶段规划都在这里。

## 2. `Docx/分类`

### `背包交互_飞升`

- `000_背包交互系统完整文档.md`

### `交接文档`

- `000_交接文档完整合集.md`
- `Pivot对话交接.md`
- `Toolbar与工具联动交接文档.md`
- `农田设计交接文档.md`
- `树林遮挡核心系统优化交接文档.md`
- `物品设计交接稿.md`
- `遮挡透明 + 云朵阴影 交接文档.md`
- `遮挡透明 + 云朵阴影 交接文档2.0.md`

### `界面UI`

- `000_UI系统完整文档.md`
- `UI面板切换与快捷键系统优化方案.md`
- `UI物品图标自适应缩放方案.md`
- `工具栏工具装备与使用分离修复方案.md`

### `砍树`

- `砍树系统完整文档.md`

### `农田`

- `000_农田系统完整文档.md`
- `农田系统编译错误修复记录.md`

### `全局`

- `000_项目全局文档.md`
- `材料等级设计.md`
- `全局功能需求总结文档.md`
- `整体项目改进计划以及后续开发任务细则.md`

### `人物与工具动画`

- `000_动画系统完整文档.md`
- `动画同步离散帧边界问题解决方案.md`
- `疾跑状态管理系统.md`
- `人物与工具动画同步方案总结.md`
- `手持工具同步功能.md`

### `世界物品`

- `world prefab生成与功能设计.md`

### `树木`

- `000_树木系统完整文档.md`
- `树林隐蔽与导航完善总结.md`
- `树木成长动态障碍物同步方案.md`
- `树木成长状态与遮挡透明联动实现方案.md`

### `遮挡与导航`

- `000_遮挡与导航系统完整文档.md`
- `导航模块完整方案.md`
- `混合导航系统使用指南.md`
- `遮挡透明系统v2.0-Unity6完整配置流程.md`
- `遮挡透明系统v2.0-最终总结.md`
- `遮挡透明系统-开发进度报告.md`
- `遮挡透明系统-快速开始指南.md`
- `遮挡透明系统-问题解决记录.md`

## 3. `Docx/设计`

- `SO/1.07_SO设计概要.md`
- `SO/SO参数设计.md`

## 4. `Docx/Memory`

- `000_项目记忆点完整文档.md`
- `windsurf记忆点.md`
- `Windsurf记忆复制.md`

## 5. `Docx/000_任务列表`

- `1.04工具系统问题分析.md`
- `12.21任务清单.md`
- `12.28任务清单.md`
- `第二阶段正式冲刺.md`

## 6. `Docx/大总结`

- `第一阶段完结报告.md`
- `Sunset_腾讯面试级工业策划案_V1.md`
- 现在新增这份：`Sunset_项目内容全清单_V1.md`

---

## 五、运行时代码模块清单：`Assets/YYY_Scripts`

## 1. 动画层

- `Anim/Player`
  - `PlayerAnimController.cs`
  - `PlayerToolController.cs`
- `Anim/NPC`
  - `NPCAnimController.cs`
- `Anim/_...._`
  - `LayerAnimSync.cs`
  - `AnimatorExtensions.cs`
  - `AxeAnimController.cs`
  - 一整套动画同步说明、最终方案、修复总结、工具同步系统设置指南。

说明：这里明确对应“玩家动画、手持工具动画、三向动画、图层同步、工具参数设置、动画离散帧同步问题”。

## 2. 战斗/命中/资源节点交互层

- `Combat/PlayerToolHitEmitter.cs`
- `Combat/ResourceNodeRegistry.cs`

说明：对应工具命中、资源节点派发、工具伤害/精力/命中行为。

## 3. 输入与玩家控制层

- `Controller/Input/GameInputManager.cs`
- `Controller/Player/PlayerController.cs`
- `Service/Player/PlayerMovement.cs`
- `Service/Player/PlayerInteraction.cs`
- `Service/Player/ToolActionLockManager.cs`
- `Service/Player/SprintStateManager.cs`

说明：对应 Hotbar、工具态、放置态、玩家交互、疾跑、锁定态、输入边界。

## 4. 树木/石头/资源实体层

- `Controller/TreeController.cs`
- `Controller/StoneController.cs`
- `Controller/RockController.cs`
- `World/ResourceNode.cs`

说明：对应树木阶段、石头阶段、资源节点、成长、掉落、交互。

## 5. 数据层

### `Data/Items`

- `ItemData.cs`
- `ToolData.cs`
- `WeaponData.cs`
- `MaterialData.cs`
- `EquipmentData.cs`
- `SeedData.cs`
- `CropData.cs`
- `WitheredCropData.cs`
- `SaplingData.cs`
- `StorageData.cs`
- `PlaceableItemData.cs`
- `WorkstationData.cs`
- `InteractiveDisplayData.cs`
- `SimpleEventData.cs`
- `PotionData.cs`
- `FoodData.cs`
- `KeyLockData.cs`

### `Data/Recipes`

- `RecipeData.cs`

### `Data/Core`

- `ToolRuntimeUtility.cs`
- `PersistentObjectRegistry.cs`
- `SaveManager.cs`
- `SaveDataDTOs.cs`
- `PrefabDatabase.cs`
- `PrefabRegistry.cs`
- `InventoryItem.cs`
- `DynamicObjectFactory.cs`

### 其他关键数据

- `SkillData.cs`
- `StageConfig.cs`
- `StoneStageConfig.cs`
- `StoneDropConfig.cs`
- `ToolResourceConfig.cs`
- `TreeSpriteData.cs`
- `NPCDialogueContentProfile.cs`
- `NPCRoamProfile.cs`
- `NPCRelationshipStage.cs`
- `NPCInformalChatExitModel.cs`
- `DropTable.cs`
- `ItemDatabase.cs`

说明：这里已经明确承载了工具、武器、作物、树苗、箱子、工作台、材料、锁钥匙、技能、矿物/石头阶段、树木表现、NPC 资料等设计数据结构。

## 6. 农田层

- `Farm/CropController.cs`
- `Farm/CropInstance.cs`
- `Farm/CropManager.cs`
- `Farm/FarmTileManager.cs`
- `Farm/FarmTileData.cs`
- `Farm/FarmVisualManager.cs`
- `Farm/FarmToolPreview.cs`
- `Farm/FarmlandBorderManager.cs`
- `Farm/SeedBagHelper.cs`
- `Farm/SoilMoistureState.cs`
- `Farm/Data/CropStageConfig.cs`
- `Farm/Data/LayerTilemaps.cs`
- `Farm/Data/CropInstanceData.cs`
- `Farm/FarmRuntimeLiveValidationRunner.cs`

说明：这里明确对应“耕地、播种、作物实例、成长阶段、湿润状态、边框、层级 Tilemap、农具预览、农田视觉、农田 live 验证”。

## 7. 背包/快捷栏/装备/箱子层

- `Service/Inventory/InventoryService.cs`
- `Service/Inventory/HotbarSelectionService.cs`
- `Service/Inventory/InventorySortService.cs`
- `Service/Inventory/InventoryBootstrap.cs`
- `Service/Inventory/ChestInventory.cs`
- `Service/Inventory/ChestInventoryV2.cs`
- `Service/Equipment/EquipmentService.cs`
- `UI/Inventory/*`
- `UI/Toolbar/*`
- `UI/Box/BoxPanelUI.cs`
- `World/Placeable/ChestController.cs`
- `World/Placeable/ChestDropHandler.cs`

说明：这里明确承载背包、快捷栏、装备、箱子、拖拽、Tooltip、丢弃、容器桥接等系统。

## 8. 放置系统层

- `Service/Placement/PlacementManager.cs`
- `Service/Placement/PlacementValidator.cs`
- `Service/Placement/PlacementPreview.cs`
- `Service/Placement/PlacementNavigator.cs`
- `Service/Placement/PlacementGridCalculator.cs`
- `Service/Placement/PlacementGridCell.cs`
- `Service/Placement/PlacementLayerDetector.cs`
- `Service/Placement/PlacementExecutionTransaction.cs`
- `Service/Placement/PlacementSecondBladeLiveValidationRunner.cs`

说明：这里明确承载放置预览、验证、网格计算、导航接近、执行事务、第二刀 live 验证。

## 9. 导航系统层

- `Service/Navigation/NavGrid2D.cs`
- `Service/Navigation/NavigationPathExecutor2D.cs`
- `Service/Navigation/NavigationLocalAvoidanceSolver.cs`
- `Service/Navigation/NavigationAvoidanceRules.cs`
- `Service/Navigation/NavigationAgentRegistry.cs`
- `Service/Navigation/NavigationLiveValidationRunner.cs`
- `Service/Navigation/NavigationStaticPointValidationRunner.cs`
- `Service/Player/PlayerAutoNavigator.cs`
- `Controller/NPC/NPCAutoRoamController.cs`

说明：这里明确对应玩家导航、NPC 漫游、避让规则、路径执行、静态点验证、live 验证。

## 10. 时间/季节/天气/技能/昼夜层

- `Service/TimeManager.cs`
- `Service/SeasonManager.cs`
- `Service/WeatherSystem.cs`
- `Service/SkillLevelService.cs`
- `Service/Rendering/DayNightManager.cs`
- `Service/Rendering/DayNightConfig.cs`
- `Service/Rendering/DayNightOverlay.cs`
- `Service/Rendering/GlobalLightController.cs`
- `Service/Rendering/PointLightManager.cs`
- `Service/Rendering/NightLightMarker.cs`
- `TimeDisplayUI.cs`
- `TimeManagerDebugger.cs`

说明：这里明确承载时间、季节、天气、昼夜表现、技能等级与时间调试。

## 11. 遮挡/渲染/云影层

- `Service/Rendering/OcclusionManager.cs`
- `Service/Rendering/OcclusionTransparency.cs`
- `Service/Rendering/CloudShadowManager.cs`
- `Service/DynamicSortingOrder.cs`
- `Controller/NPC/NPCBubblePresenter.cs`
- `Story/UI/NpcWorldHintBubble.cs`
- `Story/UI/SpringDay1WorldHintBubble.cs`

说明：这里明确对应遮挡透明、云影、动态排序、世界气泡显示层。

## 12. 剧情 / Story 层

### 数据

- `Story/Data/DialogueNode.cs`
- `Story/Data/DialogueSequenceSO.cs`
- `Story/Data/StoryPhase.cs`
- `Story/Data/DialogueFontLibrarySO.cs`

### 管理

- `Story/Managers/StoryManager.cs`
- `Story/Managers/DialogueManager.cs`
- `Story/Managers/SpringDay1Director.cs`
- `Story/Managers/DialogueValidationBootstrap.cs`

### 交互

- `Story/Interaction/CraftingStationInteractable.cs`
- `Story/Interaction/NPCDialogueInteractable.cs`
- `Story/Interaction/NPCInformalChatInteractable.cs`
- `Story/Interaction/SpringDay1BedInteractable.cs`
- `Story/Interaction/SpringDay1ProximityInteractionService.cs`

### UI

- `Story/UI/DialogueUI.cs`
- `Story/UI/SpringDay1PromptOverlay.cs`
- `Story/UI/SpringDay1StatusOverlay.cs`
- `Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
- `Story/UI/InteractionHintOverlay.cs`
- `Story/UI/InteractionHintDisplaySettings.cs`
- `Story/UI/SpringUiEvidenceCaptureRuntime.cs`

说明：这里明确承载剧情节点、对话序列、阶段、主线管理、Day1 导演、NPC 交互、工作台提示、PromptOverlay、StatusOverlay。

---

## 六、编辑器工具链清单：`Assets/Editor`

## 1. 数据批量生产类

- `Tool_BatchItemSOGenerator.cs`
- `Tool_BatchItemSOModifier.cs`
- `Tool_BatchRecipeCreator.cs`
- `DatabaseSyncHelper.cs`
- `ItemBatchSelectWindow.cs`
- `SpriteBatchSelectWindow.cs`

## 2. 世界物件 / Prefab / 资产生成类

- `WorldPrefabGeneratorTool.cs`
- `BatchCreatePrefabs.cs`
- `BatchSpriteRendererSettings.cs`
- `TilemapToSprite.cs`
- `PrefabDatabaseAutoScanner.cs`
- `PrefabDatabaseEditor.cs`

## 3. 动画 / 三向 / 工具动作工具类

- `LayerAnimSetupTool.cs`
- `001LayerAnimSetupTool.cs`
- `SliceAnimControllerTool.cs`
- `ToolAnimationGeneratorTool.cs`
- `ToolAnimationPipeline.cs`
- `TriDirectionalAnimGenerator.cs`
- `SpriteReorderTool.cs`
- `Tool_003_BatchAnimTransitions.cs`

## 4. 树木 / 遮挡 / 场景排序类

- `StaticObjectOrderAutoCalibrator.cs`
- `TreeControllerEditor.cs`
- `StoneControllerEditor.cs`
- `BatchAddOcclusionComponents.cs`
- `CloudShadowManagerEditor.cs`
- `OcclusionManagerEditor.cs`
- `OcclusionTransparencyEditor.cs`

## 5. 放置 / 箱子 / 导航验证类

- `PlacementExecutionTransactionTests.cs`
- `PlacementManagerAdjacentIntentTests.cs`
- `PlacementReachEnvelopeTests.cs`
- `ChestInventoryBridgeTests.cs`
- `NavigationStaticPointValidationMenu.cs`
- `PlayerAutoNavigatorEditor.cs`

## 6. NPC / Story / UI 支撑类

- `NPCPrefabGeneratorTool.cs`
- `NPCSceneIntegrationTool.cs`
- `NPCInformalChatValidationMenu.cs`
- `PlayerNpcRelationshipDebugMenu.cs`
- `DialogueChineseFontAssetCreator.cs`
- `DialogueDebugMenu.cs`
- `SpringDay1BedSceneBinder.cs`
- `SpringDay1WorkbenchSceneBinder.cs`
- `SpringUiEvidenceMenu.cs`
- `CodexEditorCommandBridge.cs`

## 7. 存档 / GUID / 持久化类

- `PersistentIdAutomator.cs`
- `PersistentIdValidator.cs`
- `TreeGUIDFixer.cs`

## 8. 其他辅助类

- `CodexMcpHttpAutostart.cs`
- `InventoryBootstrapEditor.cs`
- `ItemDataEditor.cs`
- `PlaceableItemDataEditor.cs`
- `SaplingDataEditor.cs`
- `Tool_001_BatchProject.cs`
- `Tool_002_BatchHierarchy.cs`
- `Tool_004_BatchTreeState.cs`

---

## 七、数据资产、预制体、场景与素材源

## 1. `Assets/111_Data`

- `Database`
- `Drop Table`
- `Items`
  - `Crops`
  - `Crops_Withered`
  - `Equipment`
  - `Foods`
  - `Keys`
  - `Locks`
  - `Materials`
  - `Placeable`
  - `Placeable/Storage`
  - `Potions`
  - `Saplings`
  - `Seeds`
  - `Tools`
  - `Weapons`
- `NPC`
- `Recipes`
- `Story/Dialogue`
- `UI/Fonts/Dialogue/Pixel`
- `UI/Fonts/Dialogue/PixelAlt`

这说明你的数据资产层已经按“作物 / 枯萎作物 / 种子 / 工具 / 武器 / 材料 / 树苗 / 箱子 / 锁钥匙 / 配方 / NPC / Story / 字体”做了分类。

## 2. `Assets/222_Prefabs`

- `Box`
- `Crops`
- `Dungeon props`
- `Farm`
- `House`
- `NPC`
- `Pan`
- `Rock`
- `Tree`
- `UI`
  - `0_Main`
  - `Botton`
  - `Chest`
  - `Grid`
  - `Spring-day1`
- `WorldItems`
  - `Crops`
  - `Crops_Withered`
  - `Equipment`
  - `Keys`
  - `Locks`
  - `Materials`
  - `Placeable`
  - `Placeable/Storage`
  - `Saplings`
  - `Seeds`
  - `Tools`
  - `Weapons`

说明：预制体层已经明确拆成世界物件、树、石头、房屋、农田、NPC、UI、Spring-day1、各种 WorldItems。

## 3. `Assets/000_Scenes`

- `Artist.unity`
- `Artist_Temp.unity`
- `DialogueValidation.unity`
- `Home.unity`
- `Primary.unity`
- `Town.unity`
- `矿洞口.unity`

说明：你当前场景层已经明确包含主场景、Home、Town、矿洞口、对话验证场景、Artist/Artist_Temp 场景。

## 4. `Assets/ZZZ_999_Package`

- 包含：
  - `000_Tile/Farmland`
  - `Pixel Crawler`
  - `Tiny Wonder Farm Free`
  - `Stardew Valley UI`
  - `RPG Arsenal`
  - `Enemy`
  - `UI`
  - `Weapon`
  - `NPC`
  - `Props`
- 从目录可直接看出的素材来源方向：
  - 农田素材
  - 角色与三向动作素材
  - 怪物素材
  - UI 素材
  - 武器图标素材
  - 建筑 / 工作台 / 环境 Tile 素材

---

## 八、规则层 / Hook / 锁 / 脚本层

## 1. `.kiro/steering`

- `README.md`
- `rules.md`
- `workspace-memory.md`
- `coding-standards.md`
- `documentation.md`
- `git-safety-baseline.md`
- `scene-modification-rule.md`
- `save-system.md`
- `ui.md`
- `animation.md`
- `trees.md`
- `systems.md`
- `items.md`
- `so-design.md`
- `placeable-items.md`
- `chest-interaction.md`
- `layers.md`
- `scene-hierarchy-sync.md`
- `debug-logging-standards.md`
- `communication.md`
- `maintenance-guidelines.md`
- `code-reaper-review.md`
- `000-context-recovery.md`
- `archive/farming.md`

说明：这些 steering 文件名本身就已经把项目规则主题钉死了，包括：

- UI 规则
- 动画规则
- 树木规则
- 系统规则
- 物品规则
- SO 设计规则
- 放置物规则
- 箱子交互规则
- 图层规则
- 场景层级同步
- 存档规则
- 调试日志规范

## 2. `.kiro/hooks`

- `smart-assistant.kiro.hook`
- `memory-update-check.kiro.hook`
- `git-quick-commit.kiro.hook`
- `git-preflight.kiro.hook`

说明：项目已经明确有智能助手、memory 更新检查、git 快提交、git preflight 的 hook 层。

## 3. `.kiro/locks`

- `shared-root-queue.md`
- `shared-root-branch-occupancy.md`
- `mcp-single-instance-occupancy.md`
- `mcp-single-instance-log.md`
- `mcp-live-baseline.md`
- `mcp-hot-zones.md`

说明：这部分明确承载共享根、MCP 占用、MCP 基线、热区等 live 协作规则。

## 4. `.kiro/scripts`

### thread-state

- `Begin-Slice.ps1`
- `Ready-To-Sync.ps1`
- `Park-Slice.ps1`
- `Show-Active-Ownership.ps1`
- `StateCommon.ps1`

### locks

- `Acquire-Lock.ps1`
- `Release-Lock.ps1`
- `Check-Lock.ps1`
- `LockCommon.ps1`

说明：项目已经把线程施工状态和锁管理做成了脚本化基础设施。

---

## 九、按“你真正关心的设计主题”重组后的查找索引

下面这部分不是按文件夹，而是按“你说的那些实际内容”重组。

## 1. 如果你要找“剧情 / 主线 / Day1 / 聚落复兴入口”

优先去：

- `.kiro/specs/900_开篇/0.0阶段/0.0.1剧情初稿`
- `.kiro/specs/900_开篇/0.0阶段/0.0.2初步落地`
- `.kiro/specs/900_开篇/0.0阶段/0.0.3V2`
- `.kiro/specs/900_开篇/spring-day1-implementation`
- `.kiro/specs/NPC`
- `Assets/YYY_Scripts/Story/*`
- `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
- `Assets/YYY_Scripts/Story/Data/*`
- `Docx/Plan/总设计.md`

已明确存在的关键词证据：

- `春1日_坠落`
- `剧情初稿`
- `需求拆分报告`
- `Day1`
- `PromptOverlay`
- `StatusOverlay`
- `NPC交互反应与关系成长设计`

## 2. 如果你要找“NPC 设定 / 日常 / 关系成长 / 双气泡 / 非正式聊天”

优先去：

- `.kiro/specs/NPC/2.0.0进一步落地/NPC场景化真实落点与角色日常设计.md`
- `.kiro/specs/NPC/2.0.0进一步落地/NPC交互反应与关系成长设计.md`
- `.kiro/specs/NPC/2.0.0进一步落地/0.0.1全面清盘`
- `.kiro/specs/NPC/2.0.0进一步落地/0.0.2清盘002`
- `.codex/threads/Sunset/NPC`
- `.codex/threads/Sunset/NPCV2`
- `Assets/YYY_Scripts/Controller/NPC/*`
- `Assets/YYY_Scripts/Service/Player/PlayerNpcRelationshipService.cs`
- `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
- `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`

## 3. 如果你要找“四季五阶段 / 早春 / 晚春早夏 / 晚夏早秋 / 晚秋 / 冬 / 天气 / 昼夜”

优先去：

- `Assets/YYY_Scripts/Service/TimeManager.cs`
- `Assets/YYY_Scripts/Service/SeasonManager.cs`
- `Assets/YYY_Scripts/Service/WeatherSystem.cs`
- `Assets/YYY_Scripts/Service/Rendering/DayNightManager.cs`
- `Assets/YYY_Scripts/_...._/季节渐变系统_最终架构.md`
- `Assets/YYY_Scripts/_...._/TimeManager_使用说明.md`
- `Docx/分类/树木/*`
- `Docx/分类/全局/*`
- `Docx/Plan/总设计.md`

## 4. 如果你要找“树木阶段 / 树苗 / 树木成长 / 树林遮挡 / 林的定义 / 树木与季节联动”

优先去：

- `.kiro/specs/001_BeFore_26.1.21/树木系统`
- `.kiro/specs/001_BeFore_26.1.21/砍树系统`
- `.kiro/specs/001_BeFore_26.1.21/砍树系统优化`
- `.kiro/specs/屎山修复/树石修复`
- `Assets/YYY_Scripts/Controller/TreeController.cs`
- `Assets/YYY_Scripts/Data/TreeSpriteData.cs`
- `Assets/YYY_Scripts/Data/ToolResourceConfig.cs`
- `Docx/分类/树木/000_树木系统完整文档.md`
- `Docx/分类/树木/树木成长动态障碍物同步方案.md`
- `Docx/分类/树木/树木成长状态与遮挡透明联动实现方案.md`
- `Docx/分类/树木/树林隐蔽与导航完善总结.md`

## 5. 如果你要找“石头阶段 / 矿石阶段 / 矿物等级 / 资源阶段 / 血量 / 掉落”

优先去：

- `.kiro/specs/001_BeFore_26.1.21/矿石系统`
- `Assets/YYY_Scripts/Controller/StoneController.cs`
- `Assets/YYY_Scripts/Controller/RockController.cs`
- `Assets/YYY_Scripts/Data/StoneStageConfig.cs`
- `Assets/YYY_Scripts/Data/StoneDropConfig.cs`
- `Assets/YYY_Scripts/Combat/PlayerToolHitEmitter.cs`
- `Assets/YYY_Scripts/World/ResourceNode.cs`
- `Assets/YYY_Scripts/Data/DropTable.cs`
- `Docx/分类/全局/材料等级设计.md`

## 6. 如果你要找“农田 / 作物 / 浇水 / 成长阶段 / 收获 / 湿润 / 农具”

优先去：

- `.kiro/specs/农田系统`
- `.codex/threads/Sunset/农田交互修复V2`
- `.codex/threads/Sunset/农田交互修复V3`
- `Assets/YYY_Scripts/Farm/*`
- `Assets/YYY_Scripts/Farm/Data/*`
- `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
- `Assets/YYY_Scripts/Data/Items/SeedData.cs`
- `Assets/YYY_Scripts/Data/Items/CropData.cs`
- `Assets/YYY_Scripts/Data/Items/WitheredCropData.cs`
- `Docx/分类/农田/000_农田系统完整文档.md`
- `Docx/Plan/种田系统完整设计方案.md`
- `Docx/Plan/农田系统-超详细实施清单.md`

## 7. 如果你要找“精力 / 血量 / 疾跑 / 玩家状态”

优先去：

- `Assets/YYY_Scripts/Service/Player/EnergySystem.cs`
- `Assets/YYY_Scripts/Service/Player/HealthSystem.cs`
- `Assets/YYY_Scripts/Service/Player/SprintStateManager.cs`
- `Assets/YYY_Scripts/Service/Player/PlayerToolFeedbackService.cs`
- `Assets/YYY_Scripts/UI/Debug/DurabilityTestUI.cs`
- `Docx/分类/人物与工具动画/疾跑状态管理系统.md`

## 8. 如果你要找“工具设计 / 武器设计 / 工具等级 / 武器等级 / 材料等级 / 阶段限制”

优先去：

- `.kiro/specs/SO设计系统与工具`
- `.kiro/specs/999_全面重构_26.01.27/3_装备与工具`
- `Assets/YYY_Scripts/Data/Items/ToolData.cs`
- `Assets/YYY_Scripts/Data/Items/WeaponData.cs`
- `Assets/YYY_Scripts/Data/Items/EquipmentData.cs`
- `Assets/YYY_Scripts/Data/Items/MaterialData.cs`
- `Assets/YYY_Scripts/Data/SkillData.cs`
- `Assets/YYY_Scripts/Utils/MaterialTierHelper.cs`
- `Assets/111_Data/Items/Tools`
- `Assets/111_Data/Items/Weapons`
- `Docx/分类/全局/材料等级设计.md`
- `Docx/000_任务列表/1.04工具系统问题分析.md`

## 9. 如果你要找“制作 / 工作台 / 配方 / 锁箱钥匙 / 经营生产”

优先去：

- `.kiro/specs/制作台系统`
- `.kiro/specs/箱子系统`
- `.kiro/specs/900_开篇/spring-day1-implementation`
- `Assets/YYY_Scripts/Service/Crafting/*`
- `Assets/YYY_Scripts/Data/Recipes/RecipeData.cs`
- `Assets/YYY_Scripts/Data/Items/WorkstationData.cs`
- `Assets/YYY_Scripts/Data/Items/StorageData.cs`
- `Assets/YYY_Scripts/Data/Items/KeyLockData.cs`
- `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
- `Assets/YYY_Scripts/UI/Crafting/*`
- `Docx/分类/世界物品/world prefab生成与功能设计.md`

## 10. 如果你要找“放置系统 / 预览 / 对齐 / 放置-背包联动 / 幽灵占位”

优先去：

- `.kiro/specs/物品放置系统`
- `.kiro/specs/箱子系统/2_箱子与放置系统综合修复`
- `Assets/YYY_Scripts/Service/Placement/*`
- `Assets/YYY_Scripts/Data/Items/PlaceableItemData.cs`
- `Assets/YYY_Scripts/Data/Items/SaplingData.cs`
- `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`

## 11. 如果你要找“背包 / Hotbar / 选中态 / 交互矩阵 / Toolbar 工具联动”

优先去：

- `.kiro/specs/UI系统/Before03.28`
- `.kiro/specs/001_BeFore_26.1.21/背包图标旋转`
- `.kiro/specs/001_BeFore_26.1.21/背包注入系统优化`
- `Docx/分类/背包交互_飞升/000_背包交互系统完整文档.md`
- `Docx/分类/界面UI/工具栏工具装备与使用分离修复方案.md`
- `Assets/YYY_Scripts/Service/Inventory/*`
- `Assets/YYY_Scripts/UI/Inventory/*`
- `Assets/YYY_Scripts/UI/Toolbar/*`

## 12. 如果你要找“动画多层级设计 / 手持工具同步 / 三向动画 / Pivot / Slice / Crush / Pierce / Watering / Fish”

优先去：

- `.kiro/specs/001_BeFore_26.1.21/工具动画重构`
- `.kiro/specs/001_BeFore_26.1.21/动画系统`
- `Docx/分类/人物与工具动画/*`
- `Assets/YYY_Scripts/Anim/*`
- `Assets/Editor/LayerAnimSetupTool.cs`
- `Assets/Editor/SliceAnimControllerTool.cs`
- `Assets/Editor/ToolAnimationPipeline.cs`
- `Assets/Editor/ToolAnimationGeneratorTool.cs`
- `Assets/ZZZ_999_Package/Pixel Crawler/Entities/Characters/Body_A/Animations/*`

## 13. 如果你要找“遮挡 / 云朵 / 植被遮挡 / 导航联动 / 林的遮挡”

优先去：

- `.kiro/specs/云朵遮挡系统`
- `.kiro/specs/屎山修复/遮挡检查`
- `.kiro/specs/001_BeFore_26.1.21/遮挡与导航`
- `.codex/threads/Sunset/遮挡检查`
- `Docx/分类/遮挡与导航/*`
- `Docx/分类/树木/*`
- `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`
- `Assets/YYY_Scripts/Service/Rendering/OcclusionTransparency.cs`
- `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs`

## 14. 如果你要找“导航 / 玩家自动导航 / NPC 避让 / 静态点 / detour / blocker”

优先去：

- `.kiro/specs/001_BeFore_26.1.21/导航系统重构`
- `.kiro/specs/屎山修复/导航检查`
- `.kiro/specs/屎山修复/导航V2`
- `.codex/threads/Sunset/导航检查`
- `.codex/threads/Sunset/导航检查V2`
- `Assets/YYY_Scripts/Service/Navigation/*`
- `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
- `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`

## 15. 如果你要找“存档 / 持久化 / GUID / 消失 bug / 自动化存档”

优先去：

- `.kiro/specs/存档系统`
- `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
- `Assets/YYY_Scripts/Data/Core/PersistentObjectRegistry.cs`
- `Assets/YYY_Scripts/Data/Core/IPersistentObject.cs`
- `Assets/Editor/PersistentIdValidator.cs`
- `Assets/Editor/PersistentIdAutomator.cs`
- `Assets/Editor/TreeGUIDFixer.cs`

## 16. 如果你要找“光影 / 昼夜 / 天气 / 夜晚灯光”

优先去：

- `.kiro/specs/Z_光影系统`
- `Assets/YYY_Scripts/Service/Rendering/DayNightManager.cs`
- `Assets/YYY_Scripts/Service/Rendering/GlobalLightController.cs`
- `Assets/YYY_Scripts/Service/Rendering/PointLightManager.cs`
- `Assets/YYY_Scripts/Service/WeatherSystem.cs`

## 17. 如果你要找“AI协同 / skills / hooks / steering / MCP / 线程制度”

优先去：

- `.kiro/specs/Codex规则落地`
- `.kiro/specs/Steering规则区优化`
- `.codex/threads/Sunset/Skills和MCP`
- `.codex/threads/Sunset/Codex规则落地`
- `.kiro/steering/*`
- `.kiro/hooks/*`
- `.kiro/locks/*`
- `.kiro/scripts/thread-state/*`

---

## 十、现在可以明确下的判断

1. 你的项目绝对不是“只有一个 Sunset 项目简介 + 几个代码文件”。
2. 你现在至少同时有五层内容库：
   - 业务系统设计工作区
   - 专项修复与专项审计工作区
   - 线程级交接与接盘历史
   - Docx 总结与分类知识库
   - 运行时系统 / 数据资产 / Editor 工具链 / 场景与 Prefab 层
3. 你真正应该做的下一步，不是凭空重写一份通用策划案，而是：
   - 从这份总清单里先勾出你要我重点提炼的设计主题；
   - 然后我再对这些主题逐块做“原始设计提取版”。
4. 目前最肥的内容库有四条：
   - `农田系统`
   - `物品放置系统`
   - `箱子系统`
   - `900_开篇 + spring-day1`
5. 当前最容易被忽略、但对你“原创设计”很关键的内容库有五条：
   - `NPC`
   - `SO设计系统与工具`
   - `UI系统`
   - `树木/矿石/全局设计` 相关早期工作区
   - `Docx/分类/全局、树木、遮挡与导航、人物与工具动画`

---

## 十一、我建议你下一轮怎么审我

你可以直接按这四种方式审：

1. 只审“我漏了哪些工作区 / 线程 / 大块内容”。
2. 只审“哪些主题我归类错了”。
3. 只审“接下来先提炼哪 5 个主题”。
4. 只审“哪些地方只是名字像，但其实不是你要的真实设计库”。

这份文档的作用，就是先把“地图”画对。
