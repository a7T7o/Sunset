# Sunset：Codex 工作区 / AI 工作流 / MCP / skills / Unity 工具全盘点

## 1. 文档用途

本文用于集中整理 `Sunset` 项目里和以下主题直接相关的真实素材：

1. `Codex` 工作区、线程、治理结构
2. AI 协同开发的真实使用方式
3. `MCP / CLI / skills / steering / hooks / memory / thread-state` 这一整套治理链
4. 项目里已经存在并实际服务开发的 Unity `Editor` 工具体系

本文不是简历终稿，也不是作品集排版稿。

它的定位是：

1. 先把事实池完整盘清。
2. 先把硬证据、硬数量、硬文件名落下来。
3. 让后续任何简历位、作品集位、面试稿位都能直接按岗位筛选，不需要再重扫一次项目。

---

## 2. 一眼结论

先说最重要的判断。

如果只看 `Sunset` 当前和 AI、工具链、治理有关的部分，最值钱的并不是“会用 AI”，而是下面四件事已经同时成立：

1. 项目已经不是单线程聊天式协作，而是有工作区、线程、规则栈、memory 栈、skills 栈和回执裁定机制的多线程协同开发。
2. Unity 侧已经不是靠零散手工点 Inspector 在推进，而是形成了覆盖数据生产、世界资产、动画、场景治理、剧情验证、导航验证、持久化验证的 Editor 工具链。
3. `MCP` 在本项目里不是单独的炫技点，而是被收进了 `CLI first, direct MCP last-resort` 的真实验证工作流里。
4. 用户在这条链里承担的不是“下达一句 prompt”，而是拍板主线、拆线程、定规范、审回执、判继续或停发、做高频黑盒验收的主控角色。

换句话说，`Sunset` 当前最能拉开和普通学生差距的，不是“做了很多系统”，而是：

1. 你已经把策划、技术落地、AI 协作、工具链和验收流程压进了一个真实项目里。
2. 这些内容不是停留在空文档层，而是有线程、有 memory、有技能链、有脚本、有 Editor 菜单、有实际项目文件作证。

---

## 3. Unity Editor 工具体系总盘点

### 3.1 硬统计先给结论

基于当前项目本地扫描，和简历最相关的 Editor / 菜单体系事实如下：

1. `Assets/Editor` 下当前共有 `95` 个 `C#` 文件。
2. `Assets/YYY_Tests/Editor` 下当前共有 `38` 个 `C#` 文件。
3. 只看带 `MenuItem` 的命令入口，总入口数为 `238`。
4. 排除 `validate` 入口后，用户真正可点击的菜单命令数为 `169`。
5. 带菜单入口的生产级脚本数为 `67`。
6. 菜单根分布里，`Tools` 是最大根，`Sunset` 是第二大根，`FarmGame` 是历史遗留但仍存在的一小组根。
7. 当前这套工具体系已经不是“几个小脚本”，而是一整套策划侧生产、验证、导演、场景治理、运行态 probe 工具链。

这里最值得你后续在简历里提取的不是绝对数量本身，而是两层意思：

1. 你已经在用工具链支撑内容扩张，而不是靠手工硬堆。
2. 你不只是写数据工具，还在做剧情导演、NPC 验证、导航验证、场景 contract、持久化 probe 这种明显更接近工业化研发的东西。

### 3.2 `Assets/Editor` 目录物理结构

当前 `Assets/Editor` 的目录物理分布如下：

1. 根目录脚本 `62` 个
2. `Story` 子目录脚本 `15` 个
3. `NPC` 子目录脚本 `9` 个
4. `Home` 子目录脚本 `4` 个
5. `Town` 子目录脚本 `4` 个
6. `Diagnostics` 子目录脚本 `1` 个

这说明当前工具不是单点爆发，而是已经按业务域形成了比较清晰的分层：

1. 根目录更多是通用生产、资产、动画、场景维护、数据治理工具。
2. `Story / NPC / Town / Home` 明显已经进入了剧情导演、跨场景 contract、居民化、场景承接的专项阶段。

### 3.3 当前真正可拿出来讲的工具分组

#### A. 数据生产工具

这组工具解决的是“内容扩张后手填数据成本爆炸”的问题。

当前最核心的文件有：

1. `Tool_BatchItemSOGenerator.cs`
2. `Tool_BatchItemSOModifier.cs`
3. `Tool_BatchRecipeCreator.cs`
4. `DatabaseSyncHelper.cs`

这组工具能说明的真实能力点是：

1. 你已经在主动解决 `Item SO / Recipe SO` 的批量生产问题。
2. 你知道策划侧真正会卡死的不是设计脑暴，而是数据层批量维护成本。
3. 你已经把“字段变化后如何批量改、如何快速铺物品和配方”当成正式开发问题来处理。

#### B. 世界资产生产与接线工具

这组工具解决的是“资源如何快速变成玩法资产”的问题。

当前最核心的文件有：

1. `WorldPrefabGeneratorTool.cs`
2. `BatchCreatePrefabs.cs`
3. `BatchSpriteRendererSettings.cs`
4. `PrefabDatabaseAutoScanner.cs`
5. `PrefabDatabaseEditor.cs`
6. `TilemapToSprite.cs`

这组工具代表的不是单纯美术导入，而是：

1. Sprite 如何转成世界可落地资产。
2. `WorldPrefab` 如何快速生成并接到运行时链路。
3. Prefab 数据库如何减少手动维护漂移。
4. Tilemap 素材如何回流到可复用资源层。

如果后续写技术策划 / 工具链方向，这一组是很硬的材料。

#### C. 动画生产与批处理工具

这组工具解决的是“人物动作、工具动作、多方向动画配置过于重复”的问题。

当前最核心的文件有：

1. `001LayerAnimSetupTool.cs`
2. `LayerAnimSetupTool.cs`
3. `SliceAnimControllerTool.cs`
4. `ToolAnimationGeneratorTool.cs`
5. `ToolAnimationPipeline.cs`
6. `Tool_003_BatchAnimTransitions.cs`
7. `TriDirectionalAnimGenerator.cs`
8. `FarmAnimalPrefabBuilder.cs`

这组工具非常值钱，因为它们对应的是你简历里经常很难讲清的那部分真实工作：

1. 不是只会改一帧动画，而是把多方向动画、图层动画、工具动画接线做成了可复用流程。
2. 不是只会 Animator 面板点点点，而是在主动减少过渡配置和控制器创建的重复劳动。
3. 和你自己做的玩家 / 工具双层同步动画、手持动作链、素材导出处理，是能相互印证的。

#### D. 场景、层级、遮挡、持久化维护工具

这组工具解决的是“场景大了以后靠手工维护会越来越不稳”的问题。

当前最核心的文件有：

1. `Tool_001_BatchProject.cs`
2. `Tool_002_BatchHierarchy.cs`
3. `StaticObjectOrderAutoCalibrator.cs`
4. `BatchAddOcclusionComponents.cs`
5. `SpriteReorderTool.cs`
6. `TilemapSelectionToColliderWorkflow.cs`
7. `TilemapToColliderObjects.cs`
8. `PersistentIdAutomator.cs`
9. `PersistentIdValidator.cs`
10. `ScenePartialSyncTool.cs`
11. `ScenePartialSyncValidationMenu.cs`
12. `ScenePrimaryBackupScratchDryRunMenu.cs`
13. `CloudShadowManagerEditor.cs`
14. `OcclusionManagerEditor.cs`
15. `OcclusionTransparencyEditor.cs`
16. `PlayerAutoNavigatorEditor.cs`
17. `PrimaryNightLightAuthoringMenu.cs`
18. `TownFoundationBootstrapMenu.cs`
19. `HomeFoundationBootstrapMenu.cs`
20. `TownCameraRecoveryMenu.cs`

这一组说明你已经在碰下面这些非常实的研发问题：

1. 场景分层、排序、遮挡和导航是联动问题，不是美术问题。
2. 云影、遮挡透明、动态排序、静态对象 order、Collider 批处理都已经被当成正式问题处理。
3. 场景 contract、局部同步、持久化 ID 检查这些内容，已经明显不是“普通学生作业”的粒度了。

#### E. 树木、石头、农田等环境对象批处理工具

这组工具解决的是“环境对象阶段化、批量状态管理和配置同步”的问题。

当前最核心的文件有：

1. `Tool_004_BatchTreeState.cs`
2. `Tool_005_BatchStoneState.cs`
3. `TreeControllerEditor.cs`
4. `StoneControllerEditor.cs`
5. `TreeGUIDFixer.cs`

这组内容可以支撑你后续去讲：

1. 资源成长不是只写文档，而是实际走到了树木 / 石头阶段化控制。
2. 环境资源对象不是纯静态摆件，而是被放进了成长、掉落、采集、遮挡、存档和场景治理链里。

#### F. NPC、剧情、导演、居民化验证工具

这是当前项目里最有辨识度、也最适合后续面试展开的一组。

当前最核心的文件有：

1. `NPCPrefabGeneratorTool.cs`
2. `NPCSceneIntegrationTool.cs`
3. `NPCInformalChatValidationMenu.cs`
4. `NpcBubblePresenterGuardValidationMenu.cs`
5. `NpcCharacterRegistryHandPortraitAutoSync.cs`
6. `NpcResidentDirectorBridgeValidationMenu.cs`
7. `PlayerNpcRelationshipDebugMenu.cs`
8. `SpringDay1NpcCrowdBootstrap.cs`
9. `SpringDay1NpcCrowdValidationMenu.cs`
10. `CodexNpcTraversalAcceptanceProbeMenu.cs`
11. `CodexEditorCommandBridge.cs`
12. `DialogueDebugMenu.cs`
13. `DialogueChineseFontAssetCreator.cs`
14. `SpringDay1DirectorPrimaryLiveCaptureMenu.cs`
15. `SpringDay1DirectorPrimaryRehearsalBakeMenu.cs`
16. `SpringDay1DirectorStagingWindow.cs`
17. `SpringDay1DirectorTownContractMenu.cs`
18. `SpringDay1LiveSnapshotArtifactMenu.cs`
19. `SpringDay1MiddayOneShotPersistenceTestMenu.cs`
20. `SpringDay1NativeFreshRestartMenu.cs`
21. `SpringDay1TargetedEditModeTestMenu.cs`
22. `SpringDay1BedSceneBinder.cs`
23. `SpringDay1WorkbenchSceneBinder.cs`
24. `SpringUiEvidenceMenu.cs`
25. `SunsetPlayModeStartSceneGuard.cs`
26. `SunsetValidationSessionCleanupMenu.cs`
27. `TownNativeResidentMigrationMenu.cs`
28. `TownSceneEntryContractMenu.cs`
29. `TownScenePlayerFacingContractMenu.cs`
30. `TownSceneRuntimeAnchorReadinessMenu.cs`
31. `HomePrimaryDoorContractMenu.cs`
32. `HomeSceneRestContractMenu.cs`
33. `PersistentPlayerSceneRuntimeMenu.cs`

这一组真正体现的不是“工具很多”，而是：

1. 你已经在用导演工具、场景 contract、resident 迁移、关系调试、正式 / 日常对话验证去推进剧情和 NPC。
2. `spring-day1` 不是停在一份剧情稿，而是已经有 staging、capture、rehearsal、targeted tests、scene binder、runtime contract 这一整套支撑。
3. 这类内容特别适合投“系统策划 + 技术策划 + AI 原生流程”交叉型岗位时讲，因为它有明显的工程感和项目主线感。

### 3.4 菜单体系里最重的命令簇

按当前菜单命令分布，最重的几组脚本如下：

1. `NPCInformalChatValidationMenu.cs`：`19` 个命令
2. `NavigationLiveValidationMenu.cs`：`17` 个命令
3. `SpringUiEvidenceMenu.cs`：`14` 个命令
4. `DialogueDebugMenu.cs`：`11` 个命令
5. `SpringDay1TargetedEditModeTestMenu.cs`：`11` 个命令
6. `DialogueChineseFontAssetCreator.cs`：`7` 个命令
7. `PlayerNpcRelationshipDebugMenu.cs`：`6` 个命令

这组统计的含义非常清楚：

1. 当前项目的工具重心已经明显落在 `NPC / 对话 / spring-day1 / 导航 / UI 证据抓取` 这些真正影响主线可玩的地方。
2. 你做的不是单纯“编辑器小工具合集”，而是围绕当前主线切片做高频验证与导演支撑。

### 3.5 `FarmGame` 与 `Sunset` 的关系

当前菜单根里还能看到一小组 `FarmGame/PrefabDatabase/*` 入口。

这说明：

1. 项目里保留了一部分更早阶段或更通用的 Prefab 数据库能力。
2. 当前重心已经明显转向 `Sunset` 自己的剧情、场景、导航、resident、导演链。
3. 后续如果写简历，建议把 `FarmGame` 相关能力吸收成“PrefabDatabase / 资产接线经验”，不要把它写成独立项目主轴。

### 3.6 这一大组工具，后续最能提炼成什么

如果只从“能写进简历”的角度看，Unity 工具体系最值得提炼的不是“我做了 169 个菜单命令”，而是下面几类表述：

1. 搭建策划侧数据生产工具链，覆盖 `Item SO / Recipe / WorldPrefab` 批量生成与修改。
2. 搭建资产接线与场景生产工具链，覆盖 `Tilemap -> Sprite -> Prefab -> 场景落地` 的批处理流程。
3. 搭建三方向动画与过渡批处理工具，减少人物 / 工具动画接线的重复成本。
4. 搭建剧情导演、NPC resident 化、导航验证、UI 证据抓取等专项验证菜单，支撑 `spring-day1` 的跨场景主线推进。
5. 形成围绕持久化 ID、场景 contract、遮挡透明、排序校准和云影治理的场景维护工具链。

---

## 4. `Codex` 工作区与线程结构总盘点

### 4.1 `.kiro/specs` 当前工作区数量

当前 `Sunset` 仓库内 `.kiro/specs` 共有 `23` 个一级工作区目录：

1. `000_Gemini`
2. `000_代办`
3. `000_代办清扫`
4. `001_BeFore_26.1.21`
5. `900_开篇`
6. `999_全面重构_26.01.27`
7. `999_全面重构_26.03.15`
8. `Codex规则落地`
9. `NPC`
10. `SO设计系统与工具`
11. `Steering规则区优化`
12. `UI系统`
13. `Z_光影系统`
14. `云朵遮挡系统`
15. `共享根执行模型与吞吐重构`
16. `农田系统`
17. `制作台系统`
18. `存档系统`
19. `屎山修复`
20. `技能等级系统`
21. `物品放置系统`
22. `箱子系统`
23. `项目文档总览`

这组结构直接说明两件事：

1. 你的项目不是“一个大文件夹 + 一堆零散文档”，而是已经工作区化了。
2. 这些工作区本身就是你 AI 原生策划工作流的重要产物，因为它们定义了问题域、边界、记忆入口和协作入口。

### 4.2 `.codex/threads/Sunset` 当前线程数量

当前 `.codex/threads/Sunset` 下共有 `24` 个线程目录：

1. `Codex规则落地`
2. `NPC`
3. `NPCV2`
4. `Skills和MCP`
5. `spring-day1`
6. `spring-day1V2`
7. `UI`
8. `UnityMCP转CLI`
9. `云朵与光影`
10. `农田交互修复V2`
11. `农田交互修复V3`
12. `场景搭建（外包）`
13. `存档系统`
14. `导航检查`
15. `导航检查V2`
16. `树石修复`
17. `遮挡检查`
18. `项目文档总览`
19. `019d4d18-bb5d-7a71-b621-5d1e2319d778`
20. `019d70de-8018-74f3-aa6a-525d5ca5edcc`
21. `2026-04-05_气泡提示样式代码盘点`
22. `backup-script`
23. `build-cn-tmp-fontfix`
24. `scene-build-5.0.0-001`

如果后续你要给别人解释“我不是只开几个聊天窗口”，这一组就是很硬的证据。

因为从结构上已经能看出来：

1. 有治理线程。
2. 有业务线程。
3. 有验证 / 修复线程。
4. 有专项迁移线程。
5. 有临时探针或构建线程。

### 4.3 当前线程结构的真实意义

这套线程结构最值钱的不是数量，而是你已经形成了下面这种实际工作方式：

1. 先按问题域拆线程。
2. 再把信息通过工作区、memory、回执卡和总控线程进行转接。
3. 你自己负责做“典狱长 / 总控 / 验收主控”的那一层判断。
4. 不是所有线程一直往前做，而是有人该继续，有人该停给你验收，有人该停给你审核。

这比“会多开几个 AI”高级很多，因为它本质上已经在做协同开发组织。

---

## 5. `steering / hooks / specs / memory / thread-state` 治理栈盘点

### 5.1 `.kiro/steering` 当前覆盖面

当前 `.kiro/steering` 下已存在的规则主题包括：

1. `animation.md`
2. `chest-interaction.md`
3. `code-reaper-review.md`
4. `coding-standards.md`
5. `communication.md`
6. `debug-logging-standards.md`
7. `documentation.md`
8. `git-safety-baseline.md`
9. `items.md`
10. `layers.md`
11. `maintenance-guidelines.md`
12. `placeable-items.md`
13. `rules.md`
14. `save-system.md`
15. `scene-hierarchy-sync.md`
16. `scene-modification-rule.md`
17. `so-design.md`
18. `systems.md`
19. `trees.md`
20. `ui.md`
21. `workspace-memory.md`
22. 以及 `README.md` 与 `archive`

这说明你的项目治理不是一句“我们有规范”，而是已经按：

1. 动画
2. UI
3. 场景修改
4. SO 设计
5. 文档
6. 记忆
7. 代码与沟通

这些真实方向拆成了可读规则面。

### 5.2 `.kiro/hooks` 当前存在的 Hook

当前 `.kiro/hooks` 下已经存在的 Hook 文件有：

1. `git-preflight.kiro.hook`
2. `git-quick-commit.kiro.hook`
3. `memory-update-check.kiro.hook`
4. `smart-assistant.kiro.hook`

这一层虽然对外不好直接写，但它很能说明你做的不是“写几条建议”，而是：

1. 在尝试把规范变成会触发、会拦截、会提醒的机制。
2. 在主动减少 AI 或线程因为上下文漂移、忘记更新 memory、提交前不自检而出问题。

### 5.3 `Codex规则落地` 工作区的真实意义

当前 `Codex规则落地` 工作区不是普通笔记区，而是项目治理正文区。

从目录名本身就能看出它承担过的内容包括：

1. `skills / AGENTS / 执行机制重构`
2. `memory 分卷治理`
3. `强制 skills 闸门与执行规范重构`
4. `main-only 回归与 worktree 退役收口`
5. `并发交通调度重建`
6. `典狱长模式`
7. `看守长模式`
8. `thread-state 开工与收口规范`
9. `治理线程批次分发与回执规范`

这说明用户在项目里做的 AI 工作，不是“叫 AI 帮我写点代码”，而是已经显式在做：

1. 流程设计
2. 协作规范
3. 现场治理
4. 回执标准
5. 线程调度和停发表

### 5.4 memory 不是装饰，而是三层结构的一部分

从当前项目实际看，memory 至少分成下面几层：

1. 工作区 memory
2. 线程 memory
3. 项目文档总览母卷
4. 全局 skill 注册表
5. skill-trigger-log 审计层

这里最值得后续写进面试展开的不是“有 memory”，而是：

1. 你已经接受了 AI 线程彼此隔离这件事，所以主动用 memory 做承接。
2. 你不是只记最终结论，而是把当前主线、当前切片、恢复点、已完成内容和阻塞点写回文件。
3. 你已经把“父子 memory / 工作区 memory / 全局审计层”做成了不同职责，而不是一份混乱长日志。

### 5.5 `thread-state` 的真实含义

当前项目已经形成 `Begin-Slice -> Ready-To-Sync -> Park-Slice` 的线程状态治理链。

这件事在你的 AI 原生工作流里非常重要，因为它意味着：

1. 线程进入真实施工前要先登记自己这刀在做什么。
2. 准备 sync 前要先过闸，不允许半成品乱交。
3. 中途停车也要合法停车，不允许“人停了，线程状态还显示在施工”。

这对外如果要讲，核心不是脚本名字，而是：

1. 你在主动解决多线程协作里最容易失控的“谁在改什么、改到哪、能不能收”问题。

---

## 6. skills 体系总盘点

### 6.1 当前本地 skills 总量

当前本机 `C:\\Users\\aTo\\.codex\\skills` 下共有 `29` 个 skill 目录。

其中和你这份项目最相关的可以分成三层：

1. 全局治理 skills
2. Sunset 项目 skills
3. 按需工具 skills

### 6.2 全局治理 skills

当前全局治理链共有 `7` 个核心 skill：

1. `skills-governor`
2. `skill-vetter`
3. `global-learnings`
4. `preference-preflight-gate`
5. `user-readable-progress-report`
6. `delivery-self-review-gate`
7. `acceptance-warden-mode`

这组 skill 说明你已经不满足于“让 AI 做事”，而是在主动治理：

1. 什么时候先做前置核查
2. 什么时候要加载偏好基线
3. 什么时候要先给用户能看懂的人话汇报
4. 什么时候交付前必须先自评
5. 什么时候应该给用户验收包而不是一句“你再测下”

### 6.3 Sunset 项目技能链

当前和 `Sunset` 直接相关的项目级 / 场景级 skills 共有 `18` 个：

1. `sunset-acceptance-handoff`
2. `sunset-doc-encoding-auditor`
3. `sunset-governance-dispatch-protocol`
4. `sunset-lock-steward`
5. `sunset-no-red-handoff`
6. `sunset-prompt-slice-guard`
7. `sunset-rapid-incident-triage`
8. `sunset-release-snapshot`
9. `sunset-review-router`
10. `sunset-scene-aseprite-batch`
11. `sunset-scene-audit`
12. `sunset-startup-guard`
13. `sunset-thread-wakeup-coordinator`
14. `sunset-ui-evidence-capture`
15. `sunset-unity-validation-loop`
16. `sunset-warden-mode`
17. `sunset-workspace-router`
18. `unity-mcp-skill`

这组 skill 的价值不在名字，而在于它们覆盖了几乎完整的项目协作链：

1. 开工前核查
2. 工作区路由
3. 热文件与锁治理
4. 提示词切片防漂移
5. Unity 验证
6. UI 证据抓取
7. release 快照
8. 验收交接
9. 典狱长裁定
10. 场景审计
11. 编码审计
12. 异常快速分诊

### 6.4 最值钱的不是 skill 数量，而是“本地化适配”

这里真正能体现你价值的，不是写一句“熟悉 skills”，而是：

1. 你没有停在假大空的通用 skill。
2. 你把 skills 做成了贴项目场景、本地文件结构、项目治理方式的本地化能力。
3. 你还给这些 skill 配了注册表和 trigger log，不是“装了就当用过”。

这一点很关键，因为它和普通“AI 使用者”的最大差异就在这里：

1. 你是在做 AI 协作能力的本地化治理。
2. 你不是把 AI 当一次性聊天对象，而是在做一套项目化方法。

---

## 7. `MCP -> CLI` 工作流总盘点

### 7.1 `Skills和MCP` 线程已经沉淀出的事实

从 `Skills和MCP` 线程记忆可确认的稳定事实包括：

1. `2026-03-07` 已落地首轮项目专用 skills：
   - `sunset-workspace-router`
   - `sunset-scene-audit`
   - `sunset-review-router`
2. `2026-03-10` 已补齐 `sunset-unity-validation-loop`
3. 做过 Unity MCP 候选对比、迁移试装、验证闭环设计
4. 做过旧桥口径和旧端口口径的清退治理
5. 已把当前 MCP live 基线统一到：
   - `unityMCP`
   - `8888`
   - `pidfile`
   - baseline 检查脚本

这说明你在 AI / MCP 这条线上做的，不只是“接上了一个工具”，而是：

1. 先比较候选
2. 再试装
3. 再统一基线
4. 再清旧口径
5. 再把它纳入正式规范

### 7.2 `UnityMCP转CLI` 线程已经沉淀出的事实

从 `UnityMCP转CLI` 线程记忆可确认的稳定事实包括：

1. 已落地 `scripts/sunset_mcp.py`
2. 已形成 `compile-first / no-red-first` 的 CLI 工作流
3. 已落地命令至少包括：
   - `baseline`
   - `status`
   - `doctor`
   - `errors`
   - `compile`
   - `no-red`
   - `recover-bridge`
   - `validate_script`
   - `manage_script` 的窄边界兼容
4. 当前正式判断是：
   - 这版 CLI 是“本地护栏壳 + MCP live 能力层”
   - 可以独立存在
   - 但不能完全脱离 MCP 完成高频 Unity live 红错验证

这一段材料后续非常适合写成“AI 原生开发 / 技术策划流程”方向的亮点，因为它体现了你做的是：

1. 验证链设计
2. 不是只靠 Unity Editor 手点
3. 不是只靠直接 MCP
4. 而是把高频爆红问题抽成了更轻量的 CLI 入口

### 7.3 当前正式口径：`CLI first, direct MCP last-resort`

这是当前整个 `Sunset` 项目在 AI 和验证层面最重要的一条口径之一。

它的真实含义不是一句术语，而是：

1. 高频的编译、红错、Console、no-red 判断，优先走轻量 CLI。
2. 只有 CLI 覆盖不到，或者任务明确进入 scene / play / inspector / runtime flow 时，才升级 direct MCP。
3. 这样做的目的是减少噪音、减少不必要的重型 live 依赖、缩短高频验证回路。

如果后续面试被问“你到底怎么用 AI 和 MCP”，这一条非常适合展开，因为它不是空泛讲工具，而是很明确的流程判断。

---

## 8. 你在 AI 原生策划推进上的真实工作

这一节是最重要的。

因为前面的工具、目录、threads、skills 都只是客观存在。
真正和你本人最相关的，是下面这些事情确实在项目里长期发生过。

### 8.1 你做的不是“提需求”，而是“主控需求定义”

从现有线程、memory 和文档看，你在项目里长期采用的真实工作方式是：

1. 用大白话长文本把需求一次性讲透。
2. 要求 AI 先复述理解，再进入方案与施工。
3. 不接受“差不多懂了就开始做”的状态。
4. 真正进入实现前，会持续做认知对齐和规则口径收口。

这意味着你的 AI 使用方式不是 prompt 工程炫技，而是：

1. 需求定义
2. 认知对齐
3. 规则收口
4. 实现验收

### 8.2 你在做工作区和线程分工

从 `23` 个工作区和 `24` 个线程的实际存在看，你长期做的不是“一个总线程什么都说”，而是：

1. 按问题域拆工作区
2. 按当前切片拆线程
3. 自己做线程之间的信息转接
4. 自己判断某线程是继续、停发、返工、还是停给自己验收

这里其实已经非常接近真实团队里的“策划主控 + 任务分发 + 验收裁定”。

### 8.3 你在做规范栈和治理栈

你在这几个月里实际推动并长期使用的治理要素包括：

1. `steering` 规则区
2. `specs` 工作区正文
3. `hooks`
4. `locks`
5. 分层 `memory`
6. `thread-state`
7. `skills` 本地化
8. `skill registry`
9. `skill-trigger-log`
10. `CLI first` 的验证口径

这组内容能说明的不是“你懂很多术语”，而是：

1. 你在持续解决 AI 协作里最难的几类问题：
   - 上下文漂移
   - 逻辑漂移
   - 线程失控
   - 回执失真
   - 验收口径不统一
   - 高风险线程继续乱做

### 8.4 你在做典狱长 / 看守长 / 验收主控

这是很容易被低估，但其实最能体现你主控权的部分。

从治理文件和工作方式上看，你长期做的事情包括：

1. 审线程回执，不接受空泛“做完了”。
2. 判断线程是否继续发 prompt。
3. 判断线程是否该停给你验收。
4. 判断线程是否该停给你分析 / 审核。
5. 要求线程交可验清单，而不是一句“你再测下”。

换句话说，你不是被 AI 带着走，而是在主动当 AI 团队的主控和验收负责人。

### 8.5 AI 已经真实改变了你的项目推进方式

这不是一句空话，而是已经体现在项目的真实推进里。

从近期文档与线程看，AI 协同已经参与并被你主控推进了下面这些关键方向：

1. `spring-day1 / Town / Primary / Home` 的跨场景主线切片推进
2. NPC 从一次性剧情演员到 resident 常驻居民结构的拍板和收口
3. 持久化玩家、跨场景唯一玩家、背包 / Hotbar / 工具态连续化
4. `Home` 镜头、黑底构图、场景承接口径
5. UI 结构收口、玩家面组织与最终证据抓取
6. `CLI first`、`no-red`、回执质量与继续 / 停发判断

这组内容非常重要，因为它说明 AI 在你项目里不是“辅助找 bug”，而是已经进入：

1. 主线推进
2. 设计裁定
3. 场景承接
4. 验收治理

---

## 9. 这些材料分别适合投什么岗位

### 9.1 系统策划岗位最该抽的材料

如果投系统策划 / 执行策划，最该抽的是：

1. `spring-day1` 切片拆解与跨场景推进
2. NPC resident 化和 formal consumed 后的日常回落
3. 背包 / 放置 / 农田 / 工作台 / 箱子 / 导航的交互边界收口
4. 四季五阶段、资源成长、工具门槛、工作台链
5. 你如何定义需求边界、验收节点和继续 / 停发判断

### 9.2 技术策划岗位最该抽的材料

如果投技术策划 / 工具链偏强的策划岗，最该抽的是：

1. Editor 工具链的五大分组
2. `SO / Recipe / WorldPrefab / PrefabDatabase / Tilemap -> Sprite` 的生产链
3. 三方向动画与过渡批处理
4. 场景治理、排序、遮挡透明、持久化 ID、Scene contract 工具
5. `spring-day1` 的导演工具、验证菜单、runtime probe

### 9.3 AI 原生开发 / AI 协作方向最该抽的材料

如果投 AI 工作流、AI 原生策划、技术型策划、研发效能相关方向，最该抽的是：

1. `23` 个工作区、`24` 个线程的项目结构
2. `skills / steering / hooks / memory / thread-state / locks` 的治理链
3. `skills` 本地化适配与注册表 / trigger log
4. `MCP` 基线治理
5. `UnityMCP -> CLI` 的 `compile-first / no-red-first` 流程
6. 典狱长 / 看守长 / 回执裁定 / 停发表的真实工作方式

---

## 10. 简历里慎写、面试里再展开的点

这一节不是否定这些亮点，而是提醒后续压缩时怎么更稳。

### 10.1 简历里可以写，但不要直接硬堆黑话的

1. `threads / specs / memory / hooks / steering / MCP`
2. `thread-state`
3. `skill-trigger-log`
4. `典狱长 / 看守长`

这些内容在母卷里必须保留，因为它们是你的真实工作。

但压成简历时，更稳的写法通常应该是：

1. 先写它解决了什么问题
2. 再写它形成了什么流程
3. 最后再在面试里解释内部术语

### 10.2 简历里最好不要直接写绝对数字的

1. 不建议把 `169`、`238`、`67` 这类数字硬写进简历正文
2. 这组数字更适合做母材料、作品集、面试展开证据
3. 简历里更稳的是写：
   - “搭建策划专属生产工具链”
   - “搭建剧情 / 导航 / UI 证据验证菜单”
   - “形成多线程 AI 协同与验证治理流程”

### 10.3 面试里最能打人的其实是“你为什么这么做”

后续最值得展开的不是工具名单，而是：

1. 为什么要把 `MCP` 收进 `CLI first`
2. 为什么需要 `skills` 本地化，而不是用假大空 skill
3. 为什么线程需要 `Begin-Slice / Ready-To-Sync / Park-Slice`
4. 为什么 NPC 要做 resident 化，而不是剧情结束就消失
5. 为什么你要亲自做继续 / 停发 / 验收裁定

---

## 附录 A：`Assets/Editor` 现存脚本清单

### A1. 根目录脚本 `62`

1. `001LayerAnimSetupTool.cs`
2. `BatchAddOcclusionComponents.cs`
3. `BatchCreatePrefabs.cs`
4. `BatchSpriteRendererSettings.cs`
5. `ChestAuthoringBatchSelectWindow.cs`
6. `ChestAuthoringSerializationTests.cs`
7. `ChestControllerEditor.cs`
8. `ChestInventoryBridgeTests.cs`
9. `CloudShadowManagerEditor.cs`
10. `CodexMcpHttpAutostart.cs`
11. `DatabaseSyncHelper.cs`
12. `DayNightConfigCreator.cs`
13. `FarmAnimalPrefabBuilder.cs`
14. `InventoryBootstrapEditor.cs`
15. `ItemBatchSelectWindow.cs`
16. `ItemDataEditor.cs`
17. `LayerAnimSetupTool.cs`
18. `NPCAutoRoamControllerEditor.cs`
19. `NPCBubbleStressTalkerEditor.cs`
20. `NPCPrefabGeneratorTool.cs`
21. `NPCSceneIntegrationTool.cs`
22. `NavigationStaticPointValidationMenu.cs`
23. `OcclusionManagerEditor.cs`
24. `OcclusionTransparencyEditor.cs`
25. `PersistentIdAutomator.cs`
26. `PersistentIdValidator.cs`
27. `PlaceableItemDataEditor.cs`
28. `PlacementExecutionTransactionTests.cs`
29. `PlacementManagerAdjacentIntentTests.cs`
30. `PlacementReachEnvelopeTests.cs`
31. `PlayerAutoNavigatorEditor.cs`
32. `PrefabDatabaseAutoScanner.cs`
33. `PrefabDatabaseEditor.cs`
34. `PrimaryNightLightAuthoringMenu.cs`
35. `SaplingDataEditor.cs`
36. `ScenePartialSyncTool.cs`
37. `ScenePartialSyncValidationMenu.cs`
38. `ScenePrimaryBackupScratchDryRunMenu.cs`
39. `SliceAnimControllerTool.cs`
40. `SpriteBatchSelectWindow.cs`
41. `SpriteReorderTool.cs`
42. `StaticObjectOrderAutoCalibrator.cs`
43. `StoneControllerEditor.cs`
44. `TilemapSelectionToColliderWorkflow.cs`
45. `TilemapToColliderObjects.cs`
46. `TilemapToSprite.cs`
47. `ToolAnimationGeneratorTool.cs`
48. `ToolAnimationPipeline.cs`
49. `Tool_001_BatchProject.cs`
50. `Tool_002_BatchHierarchy.cs`
51. `Tool_003_BatchAnimTransitions.cs`
52. `Tool_004_BatchTreeState.cs`
53. `Tool_005_BatchStoneState.cs`
54. `Tool_BatchItemSOGenerator.cs`
55. `Tool_BatchItemSOModifier.cs`
56. `Tool_BatchRecipeCreator.cs`
57. `TownCameraRecoveryMenu.cs`
58. `TownFoundationBootstrapMenu.cs`
59. `TreeControllerEditor.cs`
60. `TreeGUIDFixer.cs`
61. `TriDirectionalAnimGenerator.cs`
62. `WorldPrefabGeneratorTool.cs`

### A2. `Story` 目录脚本 `15`

1. `DialogueChineseFontAssetCreator.cs`
2. `DialogueDebugMenu.cs`
3. `SpringDay1BedSceneBinder.cs`
4. `SpringDay1DirectorPrimaryLiveCaptureMenu.cs`
5. `SpringDay1DirectorPrimaryRehearsalBakeMenu.cs`
6. `SpringDay1DirectorStagingWindow.cs`
7. `SpringDay1DirectorTownContractMenu.cs`
8. `SpringDay1LiveSnapshotArtifactMenu.cs`
9. `SpringDay1MiddayOneShotPersistenceTestMenu.cs`
10. `SpringDay1NativeFreshRestartMenu.cs`
11. `SpringDay1TargetedEditModeTestMenu.cs`
12. `SpringDay1WorkbenchSceneBinder.cs`
13. `SpringUiEvidenceMenu.cs`
14. `SunsetPlayModeStartSceneGuard.cs`
15. `SunsetValidationSessionCleanupMenu.cs`

### A3. `NPC` 目录脚本 `9`

1. `CodexEditorCommandBridge.cs`
2. `CodexNpcTraversalAcceptanceProbeMenu.cs`
3. `NPCInformalChatValidationMenu.cs`
4. `NpcBubblePresenterGuardValidationMenu.cs`
5. `NpcCharacterRegistryHandPortraitAutoSync.cs`
6. `NpcResidentDirectorBridgeValidationMenu.cs`
7. `PlayerNpcRelationshipDebugMenu.cs`
8. `SpringDay1NpcCrowdBootstrap.cs`
9. `SpringDay1NpcCrowdValidationMenu.cs`

### A4. `Home` 目录脚本 `4`

1. `HomeFoundationBootstrapMenu.cs`
2. `HomePrimaryDoorContractMenu.cs`
3. `HomeSceneRestContractMenu.cs`
4. `PersistentPlayerSceneRuntimeMenu.cs`

### A5. `Town` 目录脚本 `4`

1. `TownNativeResidentMigrationMenu.cs`
2. `TownSceneEntryContractMenu.cs`
3. `TownScenePlayerFacingContractMenu.cs`
4. `TownSceneRuntimeAnchorReadinessMenu.cs`

### A6. `Diagnostics` 目录脚本 `1`

1. `LoadedSceneMissingScriptProbeMenu.cs`

---

## 附录 B：当前可直接点名的 skills 清单

### B1. 全局治理 `7`

1. `skills-governor`
2. `skill-vetter`
3. `global-learnings`
4. `preference-preflight-gate`
5. `user-readable-progress-report`
6. `delivery-self-review-gate`
7. `acceptance-warden-mode`

### B2. Sunset 项目链 `18`

1. `sunset-acceptance-handoff`
2. `sunset-doc-encoding-auditor`
3. `sunset-governance-dispatch-protocol`
4. `sunset-lock-steward`
5. `sunset-no-red-handoff`
6. `sunset-prompt-slice-guard`
7. `sunset-rapid-incident-triage`
8. `sunset-release-snapshot`
9. `sunset-review-router`
10. `sunset-scene-aseprite-batch`
11. `sunset-scene-audit`
12. `sunset-startup-guard`
13. `sunset-thread-wakeup-coordinator`
14. `sunset-ui-evidence-capture`
15. `sunset-unity-validation-loop`
16. `sunset-warden-mode`
17. `sunset-workspace-router`
18. `unity-mcp-skill`

---

## 附录 C：当前 `Sunset` 线程与工作区清单

### C1. 工作区 `23`

1. `000_Gemini`
2. `000_代办`
3. `000_代办清扫`
4. `001_BeFore_26.1.21`
5. `900_开篇`
6. `999_全面重构_26.01.27`
7. `999_全面重构_26.03.15`
8. `Codex规则落地`
9. `NPC`
10. `SO设计系统与工具`
11. `Steering规则区优化`
12. `UI系统`
13. `Z_光影系统`
14. `云朵遮挡系统`
15. `共享根执行模型与吞吐重构`
16. `农田系统`
17. `制作台系统`
18. `存档系统`
19. `屎山修复`
20. `技能等级系统`
21. `物品放置系统`
22. `箱子系统`
23. `项目文档总览`

### C2. 线程 `24`

1. `Codex规则落地`
2. `NPC`
3. `NPCV2`
4. `Skills和MCP`
5. `spring-day1`
6. `spring-day1V2`
7. `UI`
8. `UnityMCP转CLI`
9. `云朵与光影`
10. `农田交互修复V2`
11. `农田交互修复V3`
12. `场景搭建（外包）`
13. `存档系统`
14. `导航检查`
15. `导航检查V2`
16. `树石修复`
17. `遮挡检查`
18. `项目文档总览`
19. `019d4d18-bb5d-7a71-b621-5d1e2319d778`
20. `019d70de-8018-74f3-aa6a-525d5ca5edcc`
21. `2026-04-05_气泡提示样式代码盘点`
22. `backup-script`
23. `build-cn-tmp-fontfix`
24. `scene-build-5.0.0-001`
