# UI 系统 - 开发记忆（分卷）

> 本卷起始于 `memory_0.md` 之后。旧长卷已完整归档到 `memory_0.md`。

## 模块概述
- 本工作区负责 Sunset 通用 UI 系统主线，包括：
  - 面板切换与快捷键
  - 工具栏装备与使用
  - 物品图标自适应与旋转显示
  - Tab 页面管理
- 配方 / 制作台与技能等级等能力已拆到独立工作区，不再继续堆回本主卷。

## 当前状态
- **完成度**: 98%
- **最后更新**: 2026-03-16
- **状态**: 核心问题已修复，进入常态维护

## 分卷索引
- `memory_0.md`：2025-12-16 ~ 2026-01-07 的完整长卷，覆盖 UI 主线功能、面板切换、工具栏、图标旋转显示、Toggle 配置修复、拖拽 bug 教训与后续子工作区拆分。

## 承接摘要

### 最近归档长卷的稳定结论
- UI 主线的核心设计已经稳定为：
  - 统一入口
  - 快捷键与 Toggle 状态同步
  - 装备与使用分离
  - 图标 `45°` 旋转显示
- 已抽离出的独立模块包括：
  - `crafting-station-system`
  - `skill-level-system`
  - `1_背包V4飞升`
  - `2_背包交互逻辑优化`
- 已沉淀的关键教训：
  - 不得擅自修改用户现有 Toggle 配置
  - 修改任何场景 / UI 配置前必须先审视原配置与层级关系

### 当前恢复点
- 本工作区当前更适合作为 UI 主线总览与历史索引入口。
- 若后续重新进入，优先先判断是：
  - 修 UI 主线共性问题
  - 还是进入具体子工作区处理专项问题
- 若只是恢复上下文，直接查：
  - `memory_0.md`
  - 相关子工作区
  - `Docx/分类/界面UI/000_UI系统完整文档.md`

## 会话记录

### 会话 1 - 2026-03-16（主卷分卷治理）

**用户需求**:
> 继续做，不要停；把你已经识别出的超长 memory 继续治理掉，但不要碰当前活跃业务线程。

**完成任务**:
1. 发现 `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\memory.md` 已达到 `670` 行，且此前尚未分卷。
2. 将旧长卷完整归档为 `memory_0.md`。
3. 重建新的精简主卷，保留模块职责、稳定结论、分卷索引与恢复点。

**修改文件**:
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\memory.md`

**关键决策**:
- `UI系统` 根工作区当前不属于活跃业务线程 owner，适合作为 08 阶段最后一批安全候选之一。
- UI 根主卷以后只保留总览和分流功能，避免继续把所有子工作区历史叠回一个总卷。

**验证结果**:
- 旧长卷已完整保留为 `memory_0.md`。
- 新主卷已恢复为可快速接手的摘要入口。

**恢复点**:
- 当前 08 阶段在候选表上已经可以收口，本轮若停止，后续只需继续做常规长度巡检。

### 会话 2 - 2026-03-28（SpringUI 子工作区进入 Phase 1 第一刀）

**用户需求**:
> 在 `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI` 的总方案过审后，只允许开始 `Phase 1 第一刀`，把 Day1 两个 overlay 的 runtime 创建链切成 `prefab-first`，禁止顺手进入 Phase 2 / Phase 3。

**完成任务**:
1. 在子工作区 `0.0.1 SpringUI` 内执行了 `Phase 1 第一刀`，把 `SpringDay1PromptOverlay` 与 `SpringDay1WorkbenchCraftingOverlay` 的 runtime 创建链改成“现有实例 -> prefab 实例 -> 旧 BuildUi fallback”。
2. 新增仅服务 Day1 第一刀的 `SpringDay1UiPrefabRegistry` 与对应 `Resources` 资产，让两个现有手调 prefab 在真正 runtime 路径中可被读取，不再只靠 editor-only `AssetDatabase`。
3. 保持边界：本轮没有改 prefab 视觉参数，没有进入自适应、撕页、滚动链、状态机、离台小进度、固定锚定，也没有启动通用层抽象。
4. 完成静态验证：
   - `git diff --check` 通过
   - `CodexCodeGuard` 对本轮 3 个 C# 文件的 Roslyn 程序集级编译检查通过

**修改文件**:
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\memory.md`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1UiPrefabRegistry.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\Resources\Story\SpringDay1UiPrefabRegistry.asset`

**关键决策**:
- `SpringUI` 长期路线仍然不变：`视觉 prefab / 行为代码 / 差异数据`。
- 但在进入 Phase 2 之前，必须先让 Day1 prefab 真正回到 runtime 主链，所以允许新增一个只服务这两份 prefab 的最小 `Resources` 注册表。

**验证结果**:
- `静态推断成立`
- `Roslyn 程序集级编译检查已过`
- `尚未做 Unity 运行态人工验收`

**恢复点**:
- `SpringUI` 子工作区已经完成“先抄”的第一刀。
- 若后续继续，应进入 Phase 2 的体验增强，不再回到“prefab 只是参考图”的旧状态。

## 2026-03-28：SpringUI 新增 ScreenSpaceOverlay 最终观感取证能力

- 当前父工作区新增一套统一证据口径：`ScreenSpaceOverlay` / GameView UI 的最终验收，不再依赖 Main Camera 截图。
- 本轮新增能力：
  - runtime 最终合成屏抓图器
  - Unity 菜单 capture / promote / prune 入口
  - `.codex/artifacts/ui-captures/spring-ui` 目录约定
  - `scripts/SpringUiEvidence.ps1` 清理脚本
  - 全局 skill：`sunset-ui-evidence-capture`
- 当前对子工作区 `0.0.1 SpringUI` 的影响：
  - Prompt / Workbench 后续所有视觉终验，都可以直接引用 `.png + .json sidecar` 证据
  - UI 线程如果仍只给 Main Camera 图或只给代码链，应视为证据不足
- 当前恢复点：
  - SpringUI 的验收口径已经补齐；后续具体 UI 返工与终验继续在 `0.0.1 SpringUI` 子工作区推进

## 2026-03-28：SpringUI 子工作区收掉一刀“几何漂移纠偏”，已拿到 Prompt / Workbench live Rect 证据

- 当前父工作区关注点：不是继续泛做 UI 体验项，而是先确保 `0.0.1 SpringUI` 子工作区把 Day1 这套 `prefab-first` 几何真源重新收死。
- 子工作区本轮新增稳定结论：
  1. Prompt 侧的“壳体冻结、内容层纵向下推”口径已经继续成立，live `TaskCardRoot` 回到 prefab 真值：`(11.900024, -12.9672003) / (328, 229.9346008)`。
  2. Workbench 剩余的最后一处硬伤已查穿：`MaterialsViewport` 不是 prefab 现成节点，而是旧兼容层运行时新建时掉回了 `CreateRect()` 默认 `100x100`。
  3. 根因不是“自适应没做”，而是 `RefreshCompatibilityLayout()` 把 `QuantityTitle` 错认成 `DetailColumn` 直系子节点，导致本该命中的内容层纵向布局根本没有执行。
  4. 修正后 live 证据已显示：`MaterialsViewport.anchoredPosition = (17.6, -108.00001)`、`sizeDelta = (-27.6, 41.800003)`，已经不再是默认 `100x100`。
- 本轮父层新增证据资产：
  - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\pending\20260328-232925-346_manual.png`
  - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\pending\20260328-232925-346_manual.json`
- 当前父层恢复点：
  - SpringUI 这轮可交用户审的是“几何漂移纠偏证据”，不是“Phase 2 已可终验”
  - 若用户认可这组硬证据，后续才讨论是否继续下一刀；若用户不认可，也只能围绕这组 live 几何事实继续返修
  - Git 白名单 sync 已尝试，但仍被 `spring-day1` 自身 same-root historical dirty 阻断；当前状态应理解为“证据链闭环，仓库收口未闭环”

## 2026-03-29：SpringUI 当前续工已重新收口为“Workbench 整面 + Prompt 正式面”，最近交互提示系统后移

- 当前父工作区判断：用户刚补了最新 live 验收与交互需求后，`0.0.1 SpringUI` 子工作区不应继续沿用旧的宽口径 prompt，而要把当前唯一主刀重新压成两个可直接看图裁决的失败面。
- 本轮对子工作区的最新裁定：
  1. Workbench 当前不能只看“左列显不显示”，而应作为一整张面板收口：
     - 左列 recipe 列表真实可见
     - 右侧 `所需材料` 区域正式面收好，不再保留半成品感
  2. Prompt 当前仍需继续回正 formal-face，且继续守住“内容增多只向下推，不横向改壳体”的边界。
- 用户本轮新补、但暂不进入当前轮实现的需求已上升为 SpringUI 父层待办：
  1. NPC / 工作台 / 其他可交互物需要统一“最近目标唯一提示 + 唯一 `E` 交互”仲裁。
  2. 视觉归属与实际触发对象必须一致，不能出现多个提示同时争抢。
  3. NPC 的剧情交互优先级高于非正式气泡聊天。
  4. 近身提示不能只是 gizmos 下几乎不可见的小点；聊天气泡速度后续也需回调到正常可读节奏。
- 本轮父层新增治理产物：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\场景搭建（外包）\2026-03-28_UI线程Phase2-Workbench显示链与Prompt正式面收口任务书.md`
- 当前父层恢复点：
  - 现在应以这份新任务书作为 UI 线程唯一最新执行口径；
  - 等 UI 线程回执时，父层只审 `Workbench 整面是否过线` 与 `Prompt 是否回正`；
  - 最近交互提示系统在当前两面收口后，再单独切片推进。

## 2026-03-29：当前主线 / 支线 / 优先级总图已对齐

- 当前父工作区主线没有变化，仍然是：**先把 Day1 / SpringUI 当前最直观的两张失败面收掉，再往后推进**。
- 当前总图按优先级排序如下：
  1. `P0 主线`：等待 UI 线程按最新任务书回执，并审 `Workbench 整面过线 + Prompt 正式面回正`
  2. `P1 已完成支撑线`：`ScreenSpaceOverlay` / GameView 取证工具链已经落地，可直接服务后续所有 UI 终验，不再是 blocker
  3. `P1 下一主刀候选`：最近交互唯一提示 / 唯一 `E` 键仲裁 / NPC 剧情优先 / 气泡速度，这组需求已正式补记，但必须等当前 Workbench + Prompt 收口后单独开刀
  4. `P2 停放支线`：shared root 的 Git / hygiene / unrelated dirty 收口仍未完成，但它当前不决定玩家眼前 UI 体验，不应抢在主线前
  5. `P3 远期线`：SpringUI 模板化、provider / binder 抽象，仍然必须等 Day1 视觉与体验过线后再谈
- 当前父层判断：
  - 真正仍在推进中的“活任务”其实只有 **1 条硬主线**
  - 另外有 **1 条已完成支撑线**（取证工具）
  - **1 条已入账但后移的下一主刀**
  - 以及 **2 条明确停放的后续线**（仓库 hygiene、模板化）
- 当前父层恢复点：
  - 继续按这个优先级表推进，不再让支线抢主线
  - 等 UI 线程回执回来后，只做一件事：判它这一轮过不过线

## 2026-03-29：SpringUI 子工作区已拿到 Prompt / Workbench 两张 accepted 终面证据

- 当前父工作区新增稳定事实：
  1. `0.0.1 SpringUI` 子工作区本轮已经不再停留在“代码链解释”层，而是拿到了两张可直接给用户裁定的 `accepted` GameView 证据。
  2. Prompt formal-face 终面证据：
     - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260329-021448-153_manual.png`
     - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260329-021448-153_manual.json`
  3. Workbench 整面终面证据：
     - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260329-021738-840_manual.png`
     - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260329-021738-840_manual.json`
- 父层本轮重新坐实的关键判断：
  1. Workbench 左列问题确实是显示链，而不是数据链；最终侧车里 `recipeRowCount = 3`，并且右侧 `MaterialsViewport` / `SelectedMaterials` 也已恢复正式面可读性。
  2. Prompt 当前 formal-face 没有再被 runtime 横向改壳；accepted 侧车继续保持 `TaskCardRoot = (11.900024, -12.9672003) / (328, 229.9346008)`。
  3. 这轮主刀已经收完，下一步不该再自动扩到模板化或统一交互系统，而应先停给用户看图裁定。
- 父层新增运维纠正：
  - 之前父层记忆里对 `scripts/SpringUiEvidence.ps1` 的 PowerShell promote 可用性判断过于乐观；本轮现场再次证明，在当前 Windows PowerShell 5.1 上它仍会被 `System.IO.Path.GetRelativePath` 缺失拦住。
  - 当前真实可用的稳定链仍是：
    - Unity 菜单抓图
    - sidecar / latest 用 UTF-8 修正
    - 再引用 `accepted` 目录交付
- 当前父层恢复点：
  - 继续以 accepted 证据为准回用户
  - 如果用户认这两张图过线，下一刀才轮到“最近交互唯一提示 / 唯一 E 键仲裁 / NPC 优先级 / 气泡速度”

## 2026-03-29：用户已明确驳回 accepted 图，SpringUI 当前继续只修 Prompt / Workbench 的排版与自适应

- 当前父工作区裁定：这轮不是“停给用户验收后顺利过线”，而是**用户已明确驳回上一轮 accepted 图**。
- 驳回原因已经被用户点得非常具体：
  1. `Prompt` 当前正式面仍差，尤其是：
     - 主任务区留白失衡
     - 底部提示条脱节
     - 右下装饰元素不对
  2. `Workbench` 当前不是只剩一个小瑕疵，而是：
     - 左列 recipe 行错误省略
     - 名称与描述层级不对
     - 多材料时材料项与 `预计耗时` 冲突
     - 说明真正的纵向自适应仍未落稳
- 父层新的唯一执行口径：
  - 继续发 prompt
  - 但只允许继续修：
    - `Prompt` 排版 / 信息层级 / 底部提示条
    - `Workbench` 左列行排版 / 右侧多材料纵向自适应
  - 仍然不准切去：
    - 最近交互唯一提示
    - 唯一 `E` 键仲裁
    - NPC 优先级
    - 模板化
- 本轮父层新增治理产物：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\场景搭建（外包）\2026-03-29_UI线程Phase2-排版自适应与信息层级纠偏任务书.md`
- 当前父层恢复点：
  - 继续以这份新任务书作为 UI 线程唯一最新口径
  - 等它回执时，不再问“过/不过”大概感受，而是逐项核：
    - Prompt 空白 / 底部条 / 装饰
    - Workbench 省略号 / 名称描述贴合 / 多材料与耗时分离

## 2026-04-01：`0.0.1 SpringUI` 子工作区已把“Story 向 UI/UE 集成外包”收成 owner contract

- 当前父工作区新增稳定事实：
  1. `0.0.1 SpringUI` 子工作区这轮没有继续做 `Prompt / Workbench` 实现，也没有扩成全项目 UI 总包讨论；
  2. 子工作区已按任务书交付主文档：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\2026-04-01_Story向UIUE集成owner边界与派工约定.md`
  3. 这份文档已经把后续默认岗位压成：
     - `Story / NPC / Day1 玩家面体验链的 UI/UE 集成 owner`
- 父层本轮重新坐实的关键判断：
  1. `SpringDay1ProximityInteractionService` 已经是当前 `Story/NPC/Day1` 玩家面链上的统一仲裁骨架，后续应继续在其上做收口，而不是另起一套“统一交互系统”。
  2. 当前默认提示主链应认定为：
     - `交互体 -> SpringDay1ProximityInteractionService -> SpringDay1WorldHintBubble + InteractionHintOverlay`
  3. `NpcWorldHintBubble` 当前更适合被认定为：
     - `旧并行链 / carried legacy leaf`
     - 不是默认主链中枢
  4. 子工作区的默认 own 边界，已经从“泛 UI”收窄成：
     - `Story/UI` 玩家面表层与接回层
     - Day1 交互统一仲裁 contract
     - `CraftingStation / Bed / Day1Director` 的玩家面体验切片
  5. 默认禁止边界也已收清：
     - 通用 UI 主系统
     - 剧情主状态机完整 owner
     - 全局输入 / `Primary` / scene hot-file
     - `NPC active owner` 仍在线时的 NPC 专项体验线
- 父层本轮补强的证据纪律：
  - 这轮判断已经明确压在 `代码结构 / owner contract` 层；
  - 没有把 owner 边界文档偷换成体验过线证明；
  - 后续如果真要说“玩家体验成立”，仍要补 live / capture / 用户终验。
- 当前父层恢复点：
  - UI 系统父层现在已经有一份可直接派工、可审回执的子工作区 owner contract 基线；
  - 在用户基于这份 contract 重新划刀前，不应再把 `SpringUI` 默认外推出“全项目 UI/UE 总包”。

## 2026-04-01：`SpringUI` 子工作区的“最近交互唯一提示 / 唯一 E”实现刀已按用户要求 PARKED

- 当前父工作区新增稳定事实：
  1. `0.0.1 SpringUI` 子工作区这轮没有继续扩大实现范围；
  2. 用户已明确叫停这刀，并指出它与 `spring-day1V2`、`NPC` active scope 撞车；
  3. 子工作区已按要求把本轮切片从 `ACTIVE` 收到 `PARKED`。
- 父层本轮应记录的真实现场：
  1. 子工作区本轮确实已经留下代码现场，当前与这刀直接相关的差异文件是：
     - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
     - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
     - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
     - `Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`
  2. 但这轮已经被用户中途叫停，因此：
     - 不继续扩写
     - 不 `sync`
     - 不再把它描述成“正在推进中的实现 owner”
  3. 当前唯一合法口径是：
     - `SpringUI` 退回 docs / contract / 审稿位
     - 等用户重裁 owner 边界后再决定是否重新开实现刀
- 父层恢复点：
  - 后续如果继续审 `SpringUI`，先看用户是否重新明确：
    - 这刀到底归 `spring-day1V2`
    - 还是归 `NPC`
    - 还是重新切回 `SpringUI`
  - 在新的裁定下来前，父层不应再把“最近交互唯一提示 / 唯一 E”继续发给 `SpringUI` 当默认实现刀。

## 2026-04-01：SpringUI 身份 / 工作区 / exact-own 自审补记

- 当前父工作区新增稳定事实：
  1. `SpringUI` 子工作区已经完成一轮身份 / 工作区 / file-level own 自审；
  2. 本轮结论不是恢复实现，而是把它从 `spring-day1V2` 影子关系里脱出来。
- 父层本轮重新钉死的判断：
  1. `SpringUI` 仍是独立的 UI 线；
  2. 其唯一工作区继续固定为：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI`
  3. 当前最准确岗位口径仍应是：
     - `Story / NPC / Day1 玩家面体验链的 UI/UE 集成 owner`
     - 而不是 `spring-day1V2` 的代工实现位
  4. 但这次 5 文件自审后，`SpringUI` 的 current exact own 需要比旧 contract 更窄：
     - 继续保留：`Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
     - 释放：`NPCInformalChatInteractable.cs`、`SpringDay1DialogueProgressionTests.cs`、`SpringDay1ProximityInteractionService.cs`、`SpringDay1InteractionPromptRuntimeTests.cs`
- 父层需要继续记住的现实问题：
  - 这轮“像 spring-day1V2 的影子”不是因为工作区错了，而是因为执行层一度把：
    - `thread-state`
    - 当前线程记忆
    误挂到 `spring-day1V2` 旧线程名下。
- 父层恢复点：
  - 后续若继续派 `SpringUI`，应优先落在真正的玩家面表层 / formal-face-backed 表现层；
  - 不再把 `交互体实现 / 仲裁 service / 广义 Day1 测试` 默认塞回 `SpringUI` 名下。

## 2026-04-01：SpringUI 子工作区已拿到玩家面近身提示链的第一张 live 终面证据

- 当前父工作区新增稳定事实：
  1. `0.0.1 SpringUI` 子工作区这轮不再停在 docs / contract，而是按独立 `UI` 线程身份恢复了一刀真实施工。
  2. 当前玩家面提示主链的实际运行结果，已经拿到一张新的 `accepted` GameView 证据：
     - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260401-173354-243_manual.png`
     - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260401-173354-243_manual.json`
  3. 这张图不是 Prompt / Workbench 旧证据复用，而是新的玩家面近身交互证据：同屏同时出现 `PromptOverlay`、NPC 头顶世界提示和左下正式交互卡。
- 父层本轮重新坐实的判断：
  1. `SpringDay1ProximityInteractionService` 这条统一仲裁骨架已经开始真正投射到玩家面：
     - 当前快照里 `WorldHint=001|E|交谈|按 E 开始对话|distance=0.00|priority=30|ready=True`
     - 说明唯一世界提示的归属、按键和文案已经对齐到 NPC `001`
  2. 子工作区本轮没有把交互链重新做成“只有小点 / 只有调试字”的测试味状态；
     - 新图里 `SpringDay1WorldHintBubble` 与 `InteractionHintOverlay` 已经是正式卡片化表现
  3. 但父层当前仍不能把这条线判成“完全过线”，因为：
     - `unityMCP` 的定向 EditMode 测试这轮没有拿到可信跑通结果
     - 原因不是代码闸门失败，而是共享 Unity 现场的自动 Play / stop 节奏在抢窗口
- 当前对子工作区的最新验证分层：
  - `结构 / checkpoint`：已过
  - `targeted probe / 局部验证`：`CodexCodeGuard` 已过，live 快照成立
  - `真实入口体验`：已补到玩家可见 capture，但自动化 targeted tests 仍待在干净窗口补跑
- 父层恢复点：
  - 现在如果用户要先审玩家面效果，已经有一张新的真实视面证据可看；
  - 如果后续还要继续把这条线收成“可 sync 的一刀”，父层应先要求补干净窗口下的 targeted tests，而不是只拿 live 图说满。

## 2026-04-01：SpringUI 玩家面主刀这一刀已按 `PARKED` 收口，父层等待用户基于当前证据继续裁定

- 当前父工作区新增稳定事实：
  1. `0.0.1 SpringUI` 子工作区这轮没有继续扩写实现，而是按最新主刀任务书完成回执收口。
  2. 子工作区已重新确认自己的唯一身份是：
     - `Story / NPC / Day1 玩家面体验链的 UI/UE 集成主刀`
  3. 子工作区当前已执行 `Park-Slice`，live 状态从 `ACTIVE` 收回到 `PARKED`。
- 父层本轮需要记住的真实边界：
  1. 当前最有价值的证据仍然是：
     - `accepted` 玩家面图
     - sidecar 中的 `WorldHint / PlayerFacing` 快照
     - `CodexCodeGuard` 已过
  2. 当前还不能说“整刀验证完毕”，因为 targeted tests 仍被共享 Unity 现场自动 Play / stop 节奏卡住。
  3. 因此子工作区现在最准确的状态不是 `READY`，而是：
     - `PARKED`
     - 等用户基于现有玩家面证据继续裁定，或等干净窗口补完 targeted tests。
- 当前父层恢复点：
  - 后续若继续审 `SpringUI` 这一刀，先看用户是要：
    - 基于当前画面继续指出体验问题
    - 还是先要求补 targeted tests
  - 在新的裁定下来前，父层不应把这刀错误地包装成“已经 sync-ready”。

## 2026-04-01：SpringUI 子工作区的身份与 own 边界已按用户新裁定回正

- 当前父工作区新增稳定事实：
  1. `0.0.1 SpringUI` 子工作区不再把自己挂成 `spring-day1V2` 影子，也不再把自己缩成只服务 `Spring` 的小外包。
  2. 当前唯一正确岗位已经被用户重新写死为：
     - `Story / NPC / Day1 玩家面体验链的 UI/UE 集成 owner`
  3. 当前唯一工作区继续固定为：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI`
- 父层本轮重新坐实的 own 分层：
  1. `exact-own` 站在玩家面结果层：
     - `SpringDay1WorldHintBubble.cs`
     - `InteractionHintOverlay.cs`
     - `SpringDay1ProximityInteractionService.cs` 的玩家面整合切片
  2. `协作切片` 站在交互体与验证接驳层：
     - `NPCDialogueInteractable.cs`
     - `NPCInformalChatInteractable.cs`
     - `CraftingStationInteractable.cs`
     - `SpringDay1BedInteractable.cs`
     - 两个相关 Editor tests
  3. `明确不吞` 继续守住：
     - `NPCBubblePresenter.cs`
     - `PlayerNpcChatSessionService.cs`
     - `PlayerThoughtBubblePresenter.cs`
     - `Primary.unity`
     - `GameInputManager.cs`
- 当前父层恢复点：
  - 后续只要继续派 `SpringUI`，就应沿这套“三层 own 分法”派工；
  - 不再走“过窄只管一个 UI 文件”或“过宽吞整个 NPC / 全局底座”这两个极端。

## 2026-04-02：SpringUI 子工作区已接下“左下角任务提示优先于闲聊”这一刀

- 当前父工作区新增稳定事实：
  1. `0.0.1 SpringUI` 子工作区这轮没有扩成整个提示系统重做；
  2. 只在 shared/UI 仲裁层收了：
     - 当同一个对象更该走 `Spring` 正式任务语义时，左下角 `InteractionHintOverlay` 不再说“闲聊”。
- 父层本轮站稳的边界：
  1. 代码主改只落在：
     - `SpringDay1ProximityInteractionService.cs`
     - `SpringDay1InteractionPromptRuntimeTests.cs`
  2. 头顶 `SpringDay1WorldHintBubble` 没被一起吞进来重写；
  3. `NPCBubblePresenter / PlayerNpcChatSessionService / PlayerThoughtBubblePresenter / Primary / GameInputManager` 继续保持未碰。
- 当前验证状态：
  - `git diff --check` 已过
  - `CodexCodeGuard` 已过
  - Unity 定向 test 本轮尝试时遇到插件会话断开，因此当前仍停在 `PARKED checkpoint`，不是 `READY`
- 当前父层恢复点：
  - 这刀后续若继续，只需补 targeted test；
  - 不应再把它扩成“整个 NPC / Story 提示系统统一重做”。

## 2026-04-02：SpringUI 子工作区已把玩家气泡正式面从错误浅色版拉回 NPC 同语言

- 当前父工作区新增稳定事实：
  1. `0.0.1 SpringUI` 子工作区这轮没有扩回 `Prompt / Workbench / 最近交互唯一提示`，而是只收了一个新的玩家面回归：玩家气泡视觉明显跑偏。
  2. 真正的根因不是偶发显示错，而是 `PlayerThoughtBubblePresenter` 自己在 `Awake / OnValidate` 中持续把样式刷回浅色玩家预设。
  3. 子工作区已经把这条链改回：
     - 玩家气泡整体视觉参数与 `NPCBubblePresenter` 正式面一致
     - 只保留左右镜像差异
     - 文本换行规则也与 NPC 对齐
  4. 同轮新增了最小 Editor 回归测试，并在用户回报测试编译错误后，已把测试装配方式改成反射取类型，避免 `Tests.Editor` 对 runtime 类型做强引用。
- 父层本轮验证结论：
  - `git diff --check` 已过
  - `CodexCodeGuard` 已过
  - 当前站住的是：
    - `结构 / checkpoint`
    - `targeted probe / 局部验证`
  - 还没站住的是：
    - `真实入口体验`
- 当前父层恢复点：
  - 这轮子工作区已经把“玩家气泡被错误预设反刷”的代码级问题收掉；
  - 若后续继续，应先看用户在真实画面里对玩家气泡是否认可，而不是把这刀再漂成别的 UI 大修。

## 2026-04-02：SpringUI 子工作区继续把玩家气泡这刀补到了“回归护栏更厚”的状态

- 当前父工作区新增稳定事实：
  1. 子工作区在完成玩家气泡正式面回正后，又沿同一条主线继续做了一轮极窄的回归加固。
  2. 这轮没有再动 `PlayerThoughtBubblePresenter` 逻辑本体，而是只扩充 `PlayerThoughtBubblePresenterStyleTests.cs` 的断言范围。
  3. 当前新增被钉住的参数包括：
     - 浮动幅度与频率
     - 尾巴摆动幅度与频率
     - 最低高度与 renderer 上方间距
     - show / hide 时长与 overshoot
  4. 这意味着玩家气泡这条线现在已经不是“只修到看起来差不多”，而是把更容易再次漂味的次级表现参数也纳入了回归护栏。
- 父层验证结论：
  - `git diff --check` 已过
  - `CodexCodeGuard` 已过
  - 依然只站住：
    - `结构 / checkpoint`
    - `targeted probe / 局部验证`
  - 仍未新增：
    - `真实入口体验`
- 当前父层恢复点：
  - 子工作区在玩家气泡这刀上，当前更缺的已经不是代码级护栏，而是用户对真实画面的主观确认。

## 2026-04-02：SpringUI 子工作区已清掉玩家气泡测试里的 `Object` 二义性

- 当前父工作区新增稳定事实：
  1. 子工作区上一轮继续加固测试后，用户立即贴出了新的 `Tests.Editor` 编译错误。
  2. 这次不是业务逻辑问题，而是测试文件里 `Object.DestroyImmediate(...)` 的命名二义性。
  3. 子工作区已把两处调用改成 `UnityEngine.Object.DestroyImmediate(...)`，没有顺手扩别的实现。
- 父层验证结论：
  - `git diff --check` 已过
  - `CodexCodeGuard` 对 `Tests.Editor` 已过
- 当前父层恢复点：
  - 玩家气泡这条线当前又回到了“代码与测试装配都站住”的状态；
  - 后续优先级应回到用户真实观感，而不是再围着这份测试文件做无意义扩写。

## 2026-04-02：SpringUI 子工作区已把玩家气泡这条线真正收成“与 NPC 正式面同口径”

- 当前父工作区新增稳定事实：
  1. 子工作区又继续补了一轮真正未竟项，不是只修编译报错。
  2. 当前新增收口有两类：
     - 测试口径从“玩家气泡应与 NPC 区分”回正为“玩家气泡应镜像 NPC 正式面”
     - 玩家 presenter 的边框真值和动态默认值补齐到 NPC 当前正式面口径
  3. 因此这条线现在已经同时站住：
     - 实现代码方向
     - 回归测试方向
     - 与 NPC 当前正式面的参数一致性
- 父层验证结论：
  - `git diff --check` 已过
  - `CodexCodeGuard` 已过
  - 依旧属于：
    - `结构 / checkpoint`
    - `targeted probe / 局部验证`
  - 仍待用户给的部分：
    - `真实入口体验`
- 当前父层恢复点：
  - 这条玩家气泡线现在更适合进入“看真实画面是否顺眼”的阶段；
  - 不应再漂回“只是测试能编过就算结束”的口径。

## 2026-04-02：SpringUI 子工作区已把玩家 / NPC 对话气泡这刀推进到“会话层级、分隔和收尾闭环都已站住”

- 当前父工作区新增稳定事实：
  1. 子工作区这轮不再只是修玩家气泡静态样式，而是继续把“玩家 / NPC 双气泡对话时谁在上面、能不能分开、结束会不会留脏状态”这一整条玩家面链收了下去。
  2. 当前新增收口包括三层：
     - `PlayerNpcChatSessionService` 增加当前会话焦点，按发言方决定前景排序与抬升
     - 玩家 / NPC 两侧 presenter 都补了会话排序 boost 与真正的整体位移能力
     - 中断 / 结束 / 立即取消时，layout shift / sort boost / focus 会一起清掉
  3. 玩家气泡本身也从“几乎镜像 NPC”收回到更像玩家自语的一侧，但仍保持和 NPC 同等级的正式面质量与可读性。
  4. 子工作区还新增了专门的 `PlayerNpcConversationBubbleLayoutTests.cs`，把“谁说话谁在上面”和“重置后清干净”这两个回归点钉住。
- 父层验证结论：
  - `git diff --check` 已过
  - 临时 `csc` 试编已过
    - 覆盖了本轮 3 个业务脚本 + 2 个测试脚本
    - 另额外把 `NPCInformalChatInteractable.cs` 作为协作类型桥接源码纳入试编，但这轮没有改它
  - Unity batchmode 真实单测这轮未能执行
    - 第一真实 blocker：项目当前已有用户打开的 Unity 实例，编辑器互斥
- 当前父层恢复点：
  - 这条线当前更缺的已经不是再补抽象或再补测试，而是用户真实画面判断：
    - 双气泡是否终于分开
    - 当前发言方是否稳定压在上层
    - 玩家气泡是否已经回到用户认可的玩家侧语义

## 2026-04-02：SpringUI 子工作区已把玩家 / NPC 气泡线收成“checkpoint 成立，但 sync 被 own-root 残留阻断”

- 当前父工作区新增稳定事实：
  1. 子工作区没有再开新实现，而是按最终收尾 prompt 只做了 `UI.json` 真值核对、`Ready-To-Sync` 判定与 live 状态结算。
  2. `UI.json` 当前已经按真实 own / expected sync 路径报真，不再存在这轮 own 集合漏掉测试 `.meta` 的旧问题。
  3. 这刀的 completion layer 已被收窄为：
     - `结构 / checkpoint`
     - `targeted probe / 局部验证`
     - 不再误报成 `真实入口体验` 已过线
  4. 子工作区最终没有硬 sync，而是把线程合法收成 `PARKED`。
- 父层 blocker 结论：
  - `Ready-To-Sync` 的第一真实 blocker 不是这刀自己的白名单内容，而是 same-root remaining dirty：
    - `NPCAutoRoamController.cs`
    - `PlayerAutoNavigator.cs`
    - `PlayerToolFeedbackService.cs`
    - `OcclusionSystemTests.cs`
    - `SpringDay1DialogueProgressionTests.cs`
    - 以及 UI 根下未纳入本轮的额外文档 / 测试残留
  - 因此当前正确口径是“checkpoint 已成立，但本轮未 sync，blocker 已明确”，而不是继续把旧尾账一并吞进这刀。
- 当前父层恢复点：
  - SpringUI 这条线现在已经有了一个诚实、可交接的停手点；
  - 之后若继续，应先处理 own-root 同根残留的归属 / 清理，再重新判断 sync；
  - 玩家真实视面终验仍待用户让出当前 Unity 实例后补做。
