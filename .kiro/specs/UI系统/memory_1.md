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

## 2026-04-05：只读审计 Inventory / Box 未提交 held owner 双轨残留

- 用户目标：
  - 只做只读分析，审计 `Assets/YYY_Scripts/UI/Inventory` 与 `Assets/YYY_Scripts/UI/Box` 当前未提交改动，回答：
    1. 箱子 / 背包 `held owner` 双轨残留在哪里
    2. 最小但有效的收口方案是什么
    3. 哪些文件必须一起改，哪些不该碰
- 本轮性质：
  - `只读分析`
  - 未跑 `Begin-Slice`
  - 不进入真实施工
- 本轮稳定结论：
  1. 当前 `held owner` 双轨的真正残留点，不在 tooltip / 视觉层，而在 `owner 判定 + 回源/丢弃分发层`：
     - 背包轨：`InventoryInteractionManager`
     - 箱子轨：`SlotDragContext + InventorySlotInteraction` 及 `BoxPanelUI` 的桥接分支
  2. 当前最小有效收口不该是重写全部拖拽链，而应只收“统一 owner 入口”：
     - 把 `manager.IsHolding` 与 `SlotDragContext.IsDragging` 的分支判断收成单一查询/分发层
     - 把箱子侧 `_chestHeldByShift/_chestHeldByCtrl/s_activeChestHeldOwner` 并入统一 held session 元数据
     - 保留现有写槽 / runtime item 保真算法，不动 tooltip / 选中态 / 外观层
  3. 当前若真要施工，必须一起看的代码核包括：
     - `Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs`
     - `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
     - `Assets/YYY_Scripts/UI/Inventory/SlotDragContext.cs`
     - `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`
     - 以及仅在入口签名变化时顺带的 `Assets/YYY_Scripts/UI/Inventory/InventoryPanelClickHandler.cs`
     - `Assets/YYY_Scripts/UI/Box/BoxPanelClickHandler.cs`
  4. 当前不该碰的文件：
     - `HeldItemDisplay.cs`
     - `ItemTooltip.cs`
     - `ItemTooltipTextBuilder.cs`
     - `InventorySlotUI.cs`
     - `InventoryPanelUI.cs`
     这些现在承担的是显示、tooltip 或选中态，不是 held owner authority。
- 验证结果：
  - 只读查看 `git diff`、`rg` 命中、相关 UI 工作区 memory 与 UI 线程 memory
  - 未改代码、未进 Unity、未跑测试
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

## 2026-04-03：SpringUI 子工作区开始正式接管 spring-day1 玩家面，先收掉提示链坏点与 DayEnd 玩家面泄漏

- 当前父工作区新增稳定事实：
  1. `spring-day1` 当前玩家面 `UI/UE` 残项已经从原实现线整体转交给 SpringUI 线处理，口径不再是“只帮忙补某个局部脚本”。
  2. 子工作区这轮先用 targeted probe 方式收了两个最明确的玩家面问题：
     - `SpringDay1WorldHintBubble` 被改成空壳，ready / teaser 都看不成正式提示卡
     - `DayEnd` 收束后低精力 warning 仍残留在玩家面
  3. 子工作区这轮没有回漂到 `NPCBubblePresenter.cs`、`Primary.unity`、`GameInputManager.cs`，只做了提示链和一个导演层最小玩家面切口。
- 父层验证结论：
  - Unity EditMode：
    - `SpringDay1InteractionPromptRuntimeTests`：通过
    - `SpringDay1LateDayRuntimeTests`：通过
  - Unity Console `error`：`0`
  - 当前仍只能落在：
    - `结构 / checkpoint`
    - `targeted probe / 局部验证`
  - `真实入口体验` 仍待补证据，不能误报完成
- 当前父层恢复点：
  - SpringUI 这条线已经不是只盯双气泡，而是开始真正收 `spring-day1` 当前玩家真正看到的结果层；
  - 下一轮更值钱的是补安全 live 入口下的玩家视角证据，或者继续按玩家面优先级收 Prompt / DialogueUI / Workbench 的真实体验残项。

## 2026-04-03：SpringUI 子工作区已完成 spring-day1 任务卡与提示缺字的只读诊断

- 当前父工作区新增稳定事实：
  1. `spring-day1` 当前“任务列表缺字”最核心的问题不在 `PromptOverlay` 壳体，而在 `SpringDay1Director.BuildPromptItems()` 的模型层：
     - 每个 `StoryPhase` 只返回 `1` 条任务；
     - 农田教学链没有把 `5` 条目标并列保留在卡面里。
  2. 当前“玩家面提示缺字”不能再混成一个问题，必须拆成三类：
     - `源头真空`：例如工作台普通态 detail 直接传空串
     - `源头过泛 / 未阶段化`：例如多数 NPC prefab 仍是 `闲聊 / 按 E 开口`
     - `显示链截断风险`：例如左下角 `InteractionHintOverlay` detail 单行省略、工作台左列 recipe 名称省略、描述高度被硬上限
  3. `PromptOverlay` 还存在一条新的结构性风险：
     - `manualPromptText` 会覆盖 `model.FocusText`
     - 且不会按 phase 自动清空
     - 因此“旧提醒长期顶着焦点条”的问题已经可以在代码层明确成立。
- 父层当前判断：
  - 这轮不是体验终验，而是一次足够具体的玩家面诊断；
  - 它已经能为后续施工提供明确顺序：
    1. 先补 `Prompt` 任务模型
    2. 再补 prompt/hint 文案源头
    3. 最后再修显示链截断
- 当前父层恢复点：
  - SpringUI 这条线现在对自己手里“到底是没填、没建模、还是被 UI 吞了”已经有了明确分类；
  - 下一轮如果继续，就不该再泛讲“提示还不稳”，而应直接按这三类收具体切片。

## 2026-04-03：SpringUI 子工作区已开始把“缺字是主矛盾”的修正真正落到 runtime 壳体与玩家面关键链路

- 当前父工作区新增稳定事实：
  1. 子工作区已经不再停留在只读诊断，而是按用户重新收窄后的主矛盾做了第一轮真实施工：
     - Prompt / Workbench runtime 壳体必须真正回到 `ScreenSpaceOverlay`
     - NPC 交互提示必须统一收回左下角
     - 工作台交互距离必须按最近可见边界判
     - 闲聊期间当前发言方必须稳定在上层
  2. 子工作区没有去碰 `Primary.unity`、`NPCBubblePresenter.cs`、`GameInputManager.cs` 这类用户明确不让漂过去的面，而是继续守“玩家面整合切片”边界。
  3. 子工作区这轮新增的测试护栏，已经开始直接覆盖：
     - stale world-space static overlay 被 runtime 真正替换
     - Prompt / Workbench runtime canvas 必须是 `ScreenSpaceOverlay`
     - NPC 候选不得再走头顶交互提示
     - 工作台最近交互点不能被偏上的 collider 包络线截胡
- 父层当前判断：
  - 这轮最有价值的不是“又改了几个 UI 细节”，而是把用户刚指出的 4 条主矛盾直接压回了代码结构层：
    1. runtime 真源
    2. 世界提示归属
    3. 交互几何
    4. 对话气泡前景焦点
  - 但它依然还不能报成体验过线，因为 Unity batch 被用户当前打开的编辑器互斥挡住了，真实 GameView 也还没补。
- 当前父层验证与 blocker：
  - 子工作区自检：
    - 本轮改动文件白名单 `git diff --check`：通过
  - Unity batch EditMode：
    - 未执行成功
    - 第一真实 blocker：项目当前已在另一个 Unity 实例中打开，batchmode 被 `HandleProjectAlreadyOpenInAnotherInstance` 直接阻断
  - 当前 completion layer 只能落到：
    - `结构 / checkpoint`
    - `targeted probe / 局部验证`
    - `真实入口体验`：仍待补
- 当前父层恢复点：
  - SpringUI 这条线下一步最该做的不是再扩模板化，而是拿真实玩家画面确认：
    1. Prompt 现在是否终于不再“有壳没字”
    2. NPC 头顶提示是否真的退场
    3. 工作台交互范围是否终于够大且贴边
    4. 谁在说话谁在上面是否稳定成立

## 2026-04-03：SpringUI 子工作区新增一轮只读链路勘察，重新把“做没做”和“做了但为什么体感还错”分开

- 当前父工作区新增稳定事实：
  1. 子工作区刚完成一轮只读勘察，结论是：
     - 工作台最近点算法并不是没做
     - 但阈值过紧、提示 detail 源头为空、上下翻转判据偏 `visual bounds center`，这三者一起更像当前真实主矛盾
  2. Workbench 悬浮小进度这条链已经守住“只在大 UI 关闭时显示”，不应再被误判成当前首嫌。
  3. 玩家 / NPC 气泡链不是完全没有“谁说话谁在上面”：
     - `PlayerNpcChatSessionService` 已有会话布局与 sort boost
     - `NPCBubblePresenter` / `PlayerThoughtBubblePresenter` 也各自有 foreground sort boost
     - 但它们现在仍建立在 `WorldSpace` 与 `targetRenderer.sortingOrder` 的稳定偏差之上，所以 live 里仍可能出现遮挡或说话者没压过对方
- 当前父层判断：
  - 这轮最重要的新增判断不是“又多看了几段代码”，而是把责任重新分账：
    1. 工作台范围问题：更像阈值和普通态提示源头
    2. Workbench 翻转问题：更像判据与稳定性
    3. 气泡问题：更像渲染承载层和排序优先级设计
  - 当前仍不能把这类只读判断写成体验过线，因为本轮没有 live 入口证据
- 当前父层恢复点：
  - 下一次若继续真实施工，SpringUI 这条线应优先做“最小修法”而不是再重抽象：
    1. 工作台距离 / 提示范围
    2. 普通态 detail
    3. 翻转判据稳定性
    4. 气泡前景层与说话者置顶优先级

## 2026-04-03：SpringUI 子工作区对 Prompt/任务列表缺字问题完成一轮只读定责，当前优先怀疑 runtime 复用链而非导演层数据为空

- 当前父工作区新增稳定事实：
  1. 子工作区已经只读核对 `SpringDay1PromptOverlay.cs`、`SpringDay1Director.cs` 与现有测试，确认 `Director` 在 `FarmingTutorial` 阶段会构建 5 条非空任务，数据面并不是首要嫌疑。
  2. 当前最可疑的真实根因被收窄为 `PromptOverlay` 的运行时显示链：
     - 半残 screen-overlay 实例被过宽的 `CanReuseRuntimeInstance()` 接纳；
     - `TryBindRuntimeShell()` 真正要求的文本/列表节点远比复用判定更严；
     - prefab 又可能被 `CanInstantiateRuntimePrefab()` 的字体可用性闸门整块打回 `BuildUi()`。
  3. 子工作区已明确把 `manual prompt` 重新降级为次级问题：
     - 它主要解释“焦点条被旧提醒顶住”，
     - 但不足以解释“整个任务列表有壳没字”。
- 父层当前判断：
  - 这轮最核心的收获不是“又查到一个 UI 症状”，而是把主矛盾重新压成：
    1. 数据是否存在
    2. runtime 是否真的接到了完整壳体
    3. prefab-first 是否在中途被回退链打断
  - 这使后续实现顺序可以更明确：先收复用/回退链，再收行项容错，最后才碰桥接提示覆盖。
- 当前父层恢复点：
  - 如果下一轮继续进实现，不该先泛调文案或布局，而应先把 Prompt runtime 真源和复用判定收紧。

## 2026-04-03：SpringUI 子工作区已从只读诊断切回真实施工，先做 runtime 真源收口和 owned 红错回收

- 当前父工作区新增稳定事实：
  1. 子工作区已经重新进入真实施工，但这轮仍是“先收硬阻塞”的阶段，不是体验终验阶段。
  2. 子工作区已先把 `PromptOverlay` 与 `WorkbenchOverlay` 的 runtime 真源链继续收紧：
     - 目标是减少旧 world-space/错误 runtime 实例继续截胡正式面的概率。
  3. 子工作区已先压回本轮自己引入的测试红错：
     - `SpringDay1LateDayRuntimeTests.cs` 不再直接依赖 `TMPro` 命名空间；
     - `SpringDay1InteractionPromptRuntimeTests.cs` 补回了协程返回路径。
- 当前父层判断：
  - 这轮最有价值的不是“又改了两份 UI 文件”，而是把主线重新拉回正确顺序：
    1. 先保证 shared root 不留我自己的红错
    2. 再继续收玩家真的卡住的 UI/UE 问题
  - 因此当前阶段应定义为：
    - `结构 / 编译面恢复继续推进中`
    - 不能写成 `玩家面体验已过线`
- 当前父层恢复点：
  - SpringUI 下一步仍应集中在玩家面四组主矛盾：
    1. Prompt / 任务列表缺字
    2. 工作台交互距离、提示范围、普通态 detail、上下翻转
    3. NPC 头顶交互提示统一左下角
    4. 玩家 / NPC 气泡遮挡与“谁说话谁在上面”

## 2026-04-03：SpringUI 子工作区继续补收内容层排版与 Prompt row 链，当前确认 owned 脚本未新增编译红错

- 当前父工作区新增稳定事实：
  1. 子工作区这轮继续只在 `PromptOverlay / WorkbenchOverlay` 收内容层，不扩到模板化、全局输入或 `NPC` 底座。
  2. `WorkbenchOverlay` 已把两个仍明显违背用户要求的硬伤收回内容层：
     - 左列长名称不再继续被错误省略号截断；
     - 右侧描述区不再被固定 `60f` 硬截断，而是按底部动作区剩余预算向下推。
  3. `PromptOverlay` 已把“半残壳仍被复用”这条链再收紧一刀：
     - 现在可复用页必须至少有可绑定的 `TaskRow_/Label/Detail`；
     - 写前台页后还会自检 row 文本是否真匹配当前 state，不匹配就重建 row 链再刷。
  4. 子工作区已重新核过本轮 own 脚本与相关测试文件，`validate_script` 全部 `0 error`；Unity Console 现存 error 仍是 `PersistentManagers / TreeController` 外部旧错，不是本轮 UI own 面。
  5. `run_tests` 目前依旧只返回 `total=0`，父层应把它视作“测试调用没真正命中”，而不是通过证据。
- 当前父层判断：
  - 这轮最重要的新增判断是：
    1. `Prompt` 主矛盾继续留在 runtime 壳复用与 row 链健康度
    2. `Workbench` 主矛盾继续留在内容层排版与 live 交互矩阵
  - 因此 SpringUI 当前阶段仍应写成：
    - `结构/局部验证继续推进`
    - 不能写成 `玩家体验已终验`
- 当前父层恢复点：
  - 如果下一轮继续，父层最该催的不是再开新抽象，而是：
    1. 真实入口复核 Prompt 是否脱离“有壳没字”
    2. 继续收工作台 live 手感
    3. 再补 NPC 提示/气泡的真实体验证据

## 2026-04-04：SpringUI 子工作区继续把 Workbench 的“常态静止 + 翻面弹性”收回正确语义，并把 Prompt/Workbench 护栏补进测试

- 当前父工作区新增稳定事实：
  1. 子工作区这轮继续只在 `Prompt / Workbench / 相关测试` 里推进，没有扩到模板化或全局输入。
  2. `WorkbenchOverlay` 已把用户第 7、8 条里最典型的偏差收正：
     - 常态定位不再持续 `Lerp`，避免 UI 和工作台相对漂
     - 只有上下翻面时做竖向弹性过渡
     - 悬浮小框也已改成直接贴锚点，不再平滑晃动
  3. `WorkbenchOverlay` 重新打开时现在会优先落到当前正在制作的配方，用户能直接看到当前单件进度并继续追加。
  4. `WorkbenchOverlay` 的 runtime 可复用壳判定已补强：
     - 左列如果存在 `RecipeRow_` 但行项文本链不完整，不再允许继续复用该 screen-overlay 壳。
  5. `PromptOverlay` 和 `WorkbenchOverlay` 的关键护栏已补进测试：
     - Prompt 前台 row 文本不再用“全树任意 Label 非空”这种宽判据
     - Workbench 左列坏壳误复用已有专门用例卡住
  6. 当前最新 Console 读取为 `0 error`，父层可确认这轮没有把 shared root 留成新的编译红面。
- 当前父层判断：
  - 这轮最值钱的新增判断是：`Workbench` 剩余问题越来越像 live 手感和边界矩阵，而不是基础结构仍然没接上。
  - 因此父层下一步应继续压：
    1. `Prompt` 首屏任务栏真实入口复核
    2. `Workbench` 剩余 live 手感
    3. `NPC` 提示/气泡体验证据
- 当前父层恢复点：
  - 下一轮如果继续，不该回去重抽象，而应继续在真实入口和 live 手感层做最小收口。

## 2026-04-04：SpringUI 子工作区已吸收剧情源协同边界，并准备先落一个 UI own checkpoint 再继续施工

- 当前父工作区新增稳定事实：
  1. 子工作区已经读取并吸收了：
     - `2026-04-04_UI线程_继续施工引导prompt_04.md`
     - `2026-04-04_UI线程_剧情源协同开发提醒_03.md`
     - `2026-04-04_UI线程_玩家面续工与剧情源协同prompt_03.md`
  2. 父层可确认新的 owner 边界已经再次收紧：
     - `UI / SpringUI` 继续 own 玩家真正看到与按到的结果层；
     - 不再向 `SpringDay1Director.cs` 和对白资产继续扩写。
  3. 子工作区本轮在继续施工前，又先收了一刀代码层硬阻塞：
     - `DialogueUI` 的 `StringComparison` 红错已经从源码层切断；
     - `WorkbenchOverlay` 左列 row 文本链开始做 runtime 可见态修复，而不是只看字符串是否被赋值。
  4. 子工作区当前正在先结算一个可提交 checkpoint；
     - 目的不是宣称体验过线；
     - 而是先把已经站住的 UI own 代码与记忆安全落盘，再继续剩余玩家面问题。
- 当前父层判断：
  - 这轮父层最核心的判断是：
    1. SpringUI 不该再围当前字串打一轮一次性补丁
    2. 也不该继续把剧情 owner 吞回来
    3. 正确顺序是：先提一个站得住的 UI own checkpoint，再继续收 Prompt/Workbench 剩余体验面
  - 因此当前父层阶段应继续写成：
    - `结构 / targeted probe 推进中`
    - 不能写成 `玩家面已全面终验`
- 当前父层恢复点：
  - 子工作区这次 checkpoint 提交后，父层下一步仍应催 3 件事：
    1. Prompt / Dialogue 缺字链
    2. Workbench 左列/正式面/状态机
    3. 悬浮框与拾取/取消闭环

- 2026-04-04 父层补记：
  - SpringUI 子工作区本轮又收了一刀 `Prompt / Dialogue` 字链问题，但范围被明确限制为：
    - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
    - `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
  - 当前新增稳定事实：
    1. Prompt 卡的 runtime readable 判定已开始覆盖“期望文案一致性”，不再只看组件是否有字；
    2. `continue` 标签的 runtime 恢复链已补到“引用选择 + Rect 归正 + 父链可见性 + 中文文案自愈”这一层；
    3. 这轮代码层通过了 `CodexCodeGuard` 的 Roslyn 程序集编译检查，但 Unity/MCP 侧仍无 fresh console 证据。
  - 当前父层判断不变：
    - 这是 `Prompt / Dialogue` 的 targeted probe 前进，不是整个 UI系统体验过线；
    - 线程级 `Ready-To-Sync` 仍被 UI own root 中其他历史脏改阻断，不能把这轮说成“UI 线程已整体 ready”。

- 2026-04-05 凌晨补记：SpringUI 子工作区把 Workbench 左列 recipe 的第一真实责任点继续压回 runtime 壳复用
  - 当前父层新增稳定事实：
    1. 子工作区这轮没有扩实现面，只在 [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs) 里继续做单文件最小补丁；
    2. 子工作区判断 Workbench 左列“能点但没字”的当前最像责任点，不在 `CraftingService` 或剧情源，而在 overlay 自己对 `RecipeRow_` 旧壳的复用门槛仍偏宽；
    3. 当前 active row 只要文本为空、透明或 rect 坏掉，就不再允许继续复用整个 screen-overlay 壳。
  - 当前父层验证状态：
    1. `git diff --check` 对目标文件通过；
    2. 目标脚本的 fresh `validate_script` 这轮被本机超时卡住，因此还不能把这刀包装成程序集级 fully closed；
    3. 当前只能站住：`代码责任点继续收窄，live 待验证`。
  - 当前父层恢复点：
    1. 下一步不是回去重抽象 UI 总架构，而是直接拿 Workbench 左列 recipe fresh live；
    2. 如果左列仍空，再继续把第一嫌疑切向字体底座或数据注入链。

- 2026-04-05 父层补记：SpringUI 子工作区把 Town/Day1 缺字链继续压回 3 个具体玩家面责任点
  - 子工作区本轮继续只在 `DialogueUI / PromptOverlay / WorkbenchOverlay` 三个 UI 脚本里推进，没有回吞 NPC 底座或剧情 owner。
  - 当前父层新增稳定事实：
    1. Town continue 缺字链的第一嫌疑落在 `DialogueUI` 自己的 continue 标签绑定与字体兜底；
    2. Prompt 缺字链的第一嫌疑落在 runtime 坏壳误复用与文本写入后未二次校正可读性；
    3. Workbench 左列 recipe 缺字链的第一嫌疑落在 `RecipeRow_` 旧壳复用门槛过宽。
  - 子工作区本轮代码层推进：
    1. `DialogueUI.cs` 前置 runtime 中文字体预热，并把 continue 标签文案统一收敛成 `继续`；
    2. `SpringDay1PromptOverlay.cs` 把 readable 判定与写入后二次可读性修复收紧到 title/subtitle/focus/footer 和任务行；
    3. `SpringDay1WorkbenchCraftingOverlay.cs` 把旧 row 复用门槛压回 `文本真可见且字体真能渲染`。
  - 当前父层验证状态：
    1. `git diff --check` 通过；
    2. fresh `validate_script` 继续被本机超时卡住，所以当前仍只能写成 `代码层推进，Unity/live 待验证`。
  - 当前父层恢复点：
    1. 下一步优先拿缺字链 4 case 的 fresh live；
    2. 若 fresh live 仍失败，再继续往字体底座或数据注入链下钻；
    3. 当前还不能把这轮包装成 SpringUI 体验过线。

- 2026-04-05 父层补记：已接受 `NPC` 回执的大边界，并把 `UI` 下一刀重新收窄到任务清单 / 正式剧情协同链
  - 当前父层新增稳定事实：
    1. `NPC` 线程已明确释放 shared 玩家面壳：
       - `InteractionHintOverlay.cs`
       - `SpringDay1PromptOverlay.cs`
       - `SpringDay1WorkbenchCraftingOverlay.cs`
       - `DialogueUI.cs`
    2. `NPC` 继续只 own：
       - `NPCBubblePresenter.cs`
       - `PlayerNpcChatSessionService.cs`
       - `speaking-owner / pair / ambient bubble` 底座
    3. 父层额外收紧：
       - shared 提示壳的 hide/show 接口仍归 `UI`
       - `NPC` 只负责 NPC own 语义信号与 contract 真值
  - 当前父层已新增可转发产物：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-05_NPC线程_气泡层级遮挡与不透明基线第一刀prompt_02.md`
  - 父层对 `UI` 下一刀的新判断：
    1. 最该先打的是 `PromptOverlay <-> DialogueUI` 协同链；
    2. 目标不是泛收玩家面，而是先把：
       - 正式剧情时任务清单退场
       - `003 -> 004 / 005` 阶段任务文字稳定显示
       这两个主矛盾收住；
    3. `Workbench` 左列 recipe 仍是 `UI own` 局部缺口，但不再抢到这刀前面。
  - 当前父层允许子工作区下一刀最深只砍：
    - `SpringDay1PromptOverlay.cs`
    - `DialogueUI.cs`
    - `SpringDay1Director.cs`
    - 必要时补 `SpringDay1LateDayRuntimeTests.cs`
  - 当前父层明确禁止回漂：
    - `NPCBubblePresenter.cs`
    - `SpringDay1WorkbenchCraftingOverlay.cs`
    - `InteractionHintOverlay.cs`
    - `Primary.unity`
    - `GameInputManager.cs`
  - 当前父层阶段判断：
    - 这是 owner / scope / 下一刀的治理收口；
    - 不是新的体验过线 claim；
    - 子工作区当前继续 `PARKED`，等用户确认后再进下一刀真实施工。

- 2026-04-05 父层补记：SpringUI 子工作区已从只读判断转入一次真实大刀施工，并再次合法停车
  - 当前父层新增稳定事实：
    1. 子工作区本轮已真实施工，不再只是 scope 判断；
    2. 施工面收在：
       - `SpringDay1PromptOverlay.cs`
       - `DialogueUI.cs`
       - `SpringDay1WorkbenchCraftingOverlay.cs`
       - `CraftingStationInteractable.cs`
       - `InteractionHintOverlay.cs`
       - `SpringDay1InteractionPromptRuntimeTests.cs`
    3. 子工作区当前已完成的最重要行为改动：
       - 正式剧情时 `PromptOverlay` 不再继续裸露在玩家面前；
       - 任务卡 phase 切换默认不再走高风险翻页，而优先走稳定即时刷新；
       - `DialogueUI` 的“其他 UI 淡出 / 淡回”已收紧到 `0.5s` 节奏；
       - 内心独白背景明显减轻，不再盖字；
       - `Workbench` 默认进度文案、hover 语义、blocker 文案与完成后乱弹字已收一轮；
       - `Workbench` 左列 row refresh 后会再做一次 readable 自检，不通过就强制 rebuild；
       - `CraftingStation` 左下角 detail copy 已改到“打开工作台 / 进度条领取 / 单件进度 / 剩余数量”的语义。
  - 当前父层仍未过线的部分：
    1. fresh compile / fresh live 仍没拿到；
    2. `spring-day1` 当前 active 占着：
       - `SpringDay1Director.cs`
       - `SpringDay1LateDayRuntimeTests.cs`
       所以父层这轮没有继续往导演层和它 own 的 runtime tests 扩。
  - 当前父层判断：
    - 这轮已经从“只读治理”推进到“玩家面代码层真实前进”；
    - 但当前仍只能站在：
      - `结构 / checkpoint`
      - `targeted probe / 局部验证`
      还不能说成 `真实入口体验已过线`。
  - 当前父层 blocker：
    1. `MCP 8888 当前拒连`
    2. `sunset_mcp.py compile/validate_script 与 direct CodexCodeGuard 都超时`
  - 当前父层状态：
    - 子工作区已 `Park-Slice`
    - 当前 live 状态：`PARKED`

- 2026-04-05 只读审计补记：Tooltip / 工具状态条 / 时间调试链结构盘点
  - 用户目标：
    - 只读 `ItemTooltip / InventorySlotInteraction / InventorySlotUI / ToolbarSlotUI / ToolRuntimeUtility / PlayerThoughtBubblePresenter / TimeManagerDebugger / TimeManager / PersistentManagers`，核清当前已满足项、最可能仍不满足的逻辑点和最小修复优先级。
  - 已完成事项：
    1. 已按结构链拆开审计：
       - `Tooltip`：触发、延迟、跟随、隐藏、运行时兜底 UI
       - `工具状态条`：背包/快捷栏显示判定、耐久/水量比值来源、recent-use 触发
       - `时间调试链`：`TimeManagerDebugger` 自动挂载、键位、`SetTime/Sleep` 事件差异、`PersistentManagers` 接线
    2. 已明确这轮证据层级只到 `结构 / checkpoint`：
       - 未改代码
       - 未跑测试
       - 未进 Unity
       - 不把这轮审计包装成体验过线
  - 关键判断：
    1. 当前已站住的部分：
       - Tooltip 已有统一单例、鼠标跟随、交互抑制、出界隐藏和淡入淡出。
       - 工具状态条已统一走 `ToolRuntimeUtility.TryGetToolStatusRatio`，能区分耐久型与水量型，并支持最近使用后的短时显露。
       - `TimeManagerDebugger` 已被 `TimeManager` 与 `PersistentManagers` 双保险挂载，时钟与加减号整点微调链能直接驱动 `SetTime/Sleep`。
    2. 当前最可能仍不满足的逻辑点：
       - `InventorySlotInteraction.TryShowTooltip()` 没把 `sourceTransform` 传给 `ItemTooltip`，而 `ToolbarSlotUI` 传了；背包/箱子 Tooltip 缺少跟随边界与首选 Canvas 上下文，体验不一致。
       - `ItemTooltip` 运行时兜底 UI 会静默自建通用壳体，且未创建 `qualityIcon`；如果场景没接正式 prefab，会退回测试味 runtime face。
       - Tooltip 显示延迟被 `MinimumShowDelay = 1f` 强制兜底；即使序列化值更小，也不会快于 1 秒。
       - 工具状态条的 `recent-use` 只在 `TryConsumeHeldToolUseDetailed()` 成功提交后写时间戳；空水壶/耐久不足/精力不足这类失败尝试不会触发“最近使用”显露。
       - `ToolRuntimeUtility` 只把 `空水壶 / 工具损坏` 明确接到玩家反馈服务；`InsufficientDurability / InsufficientEnergy` 仍只有日志级失败口径。
       - `TimeManagerDebugger` 虽号称开发调试脚本，但 `TimeManager.Instance / Awake` 和 `PersistentManagers` 都会无条件 `EnsureAttached()`；默认还会强制 `enableDebugKeys = true`。
       - `TimeManagerDebugger.OnGUI()` 只要 `enableDebugKeys` 为真就始终画左上角快捷键说明；`showDebugInfo` 只管日志，不管 GUI 覆盖。
       - `TimeManager.SetTime()` 会补发小时/分钟事件，但不会在“日变化但未走 Sleep/AdvanceDay”时补发 `OnDayChanged`；调试器的季节跳转直接走 `SetTime()`，日更订阅者可能不同步。
    3. 证据边界：
       - 当前允许文件里看不到 `ItemTooltipTextBuilder` 与 `PlayerToolFeedbackService` 的实现，因此 Tooltip 正文内容和最终气泡反馈样式链只能做结构级保守判断，不能声称已核清最终玩家观感。
  - 涉及文件或路径：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts/UI/Inventory/ItemTooltip.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts/Data/Core/ToolRuntimeUtility.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts/TimeManagerDebugger.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts/Service/TimeManager.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts/Service/PersistentManagers.cs`
  - 验证结果：
    - `静态代码审计成立`
    - `体验未验证`
  - 遗留问题 / 下一步：
    1. 若进入最小修复，优先顺序应为：
       - P0：先把时间调试链改成真正的 dev-only / 可硬关闭，并补齐 `SetTime` 跳日的日变化通知或等价刷新。
       - P1：补齐背包 Tooltip 的 `sourceTransform` 上下文，并决定是否允许 runtime fallback 静默冒充正式 tooltip。
       - P2：让失败用具尝试也能显露状态条/反馈，至少覆盖 `空水 / 低耐久 / 精力不足`。
    2. 若继续深挖体验层，需要补：
       - 正式 Tooltip prefab / GameView 截图
       - 玩家侧实际失败反馈文案或视频
       - 时间调试在真实场景里的订阅者联动验证
  - 当前恢复点：
    - 这轮已把 Tooltip / 状态条 / 时间调试链的结构性风险点收口成可执行优先级；若用户继续推进，可直接转入最小修复，不必再重做首轮盘点。

## 2026-04-05｜只读审计：箱子 UI / Inventory 交互不一致的根因不在显示层，而在 Held 状态机双轨
- 当前主线：
  - 用户要求只读核当前 `UI/Inventory + Box` 交互链，解释为什么箱子 UI 里的 `shift/ctrl` 拿起、放回、同类堆叠、跨 Up/Down 落点语义和背包不一致。
- 本轮只读结论：
  1. 箱子 Up 区的数据真源是 `ChestController.RuntimeInventory`，也就是优先 `ChestInventoryV2`；`BoxPanelUI` 已经按这个源绑定，不再是 legacy `ChestInventory` 为真。
  2. 真正共享的只是容器/堆叠基础规则：
     - `IItemContainer`
     - `ItemStack.CanStackWith`
     - `GetMaxStack`
  3. 真正分叉的是交互语义：
     - 背包 `shift/ctrl` Held 由 `InventoryInteractionManager` 管
     - 箱子 `shift/ctrl` Held 由 `InventorySlotInteraction + SlotDragContext + _chestHeldByShift/_Ctrl` 另起一套
     - `Down -> Up` 和 `Up -> Down` 甚至不是同一个落点函数
  4. 因此当前 bug 不是“某个 if 写错”，而是同一面板里同时存在两套 Held owner 和两套回源/堆叠/交换语义。
- 本轮涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryInteractionManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotInteraction.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\SlotDragContext.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Box\BoxPanelUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\World\Placeable\ChestController.cs`
- 验证状态：
  - `静态审计成立`
  - `未改代码`
  - `未跑测试`
- 当前恢复点：
  - 若后续要修，优先收的不是样式或绑定，而是统一 `shift/ctrl` Held owner 与跨 Up/Down 的落点语义入口；
  - 在这之前继续补箱子专用桥接，只会继续放大差异。

## 2026-04-05｜UI 线程继续真修：`PromptOverlay` 缺字与 `Workbench` 左列旧壳复用
- 当前主线：
  - `Story / NPC / Day1` 玩家面链继续只收用户当前最痛的两个 UI 真问题：
    1. `004/005` 任务卡缺字只剩底板
    2. `Workbench` 左列 recipe 能点但没字
- 本轮实际做成了什么：
  1. `PromptOverlay` 字体选择链改成按当前真实文本重算，不再只靠固定 probe 选一次；
  2. `PromptOverlay` 的 shell 文本应用、可读性自愈和文本高度测量，都改成用当前文本来选可用字体；
  3. `Workbench` 左列命中旧 prefab/manual recipe 行壳时，优先强制重建生成行，不再继续复用半残旧 row；
  4. `Workbench` 左列文字链也改成按当前文本重选字体，避免旧材质/旧字体继续留下空白。
- 当前仍没做成什么：
  1. fresh compile 没闭环；
  2. Unity live / GameView 证据没补上；
  3. 因而这轮仍不能 claim 体验过线。
- 当前判断：
  - 这轮更像“把最可能的真实根因补上了”，不是“已经 fresh live 证明确实全好了”；
  - 后续第一优先不是再加功能，而是拿 fresh live 看这两刀是否真打中。
- 当前 blocker：
  1. `manage_script validate` 仍报 `No active Unity instance`
  2. `sunset_mcp.py compile ... --skip-mcp` 单文件仍超时
- 当前状态：
  - 已 `Park-Slice`
  - `PARKED`

## 2026-04-05｜UI 线程只读盘点后的剩余项重收束
- 当前主线：
  - `UI` 线程当前仍应只服务 `spring-day1` 玩家面结果层，不扩成全系统重做。
- 只读结论：
  1. 当前剩余第一梯队仍是 3 块：
     - `Prompt / 任务列表`：不再明显累计上飘，但高度自适应和 `004/005` 缺字坏态没彻底收干净；
     - `DialogueUI`：独白字体已接清楚像素字，但背景语义还没完全回正到“和普通对话一致”；
     - `Workbench`：左列空白与越界仍未拿到 fresh live 过线证据。
  2. 历史大需求没有消失，只是还没排到最前：
     - Workbench 完整交互/进度/悬浮块状态机；
     - 玩家面对话气泡层级、遮挡和边界一致性；
     - 正式剧情期间 UI hide/show 节奏与继续提示缺字链。
  3. 当前阶段仍是：
     - `结构 / targeted probe 有进展`
     - `真实体验未过线`
- 当前恢复点：
  - 下轮若继续，应先砍 `Prompt` 和 `DialogueUI` 的直接坏感，再回到 `Workbench`。

## 2026-04-05｜UI 真修 checkpoint：Prompt/Dialogue/Workbench 三簇同步推进
- 当前主线：
  - `spring-day1` 玩家面三簇继续往前推：`Prompt`、`DialogueUI`、`Workbench`。
- 这轮真实推进：
  1. `PromptOverlay` 生成页高度从“整块假 preferredHeight”改成“可见 section 逐段求和”，并把空 section 直接隐藏。
  2. `DialogueUI` 独白移除单独背景处理，回正到“背景同普通对话，只保留更清楚字体”。
  3. `Workbench` manual recipe 壳不再允许继续被 runtime 复用；旧壳命中后直接重建左列，并补了一层 legacy detail 压缩护栏。
  4. 新增 Prompt/Workbench 两层回归测试护栏。
- 本轮验证：
  - 3 个 UI 脚本 `validate_script` = `0 error / 1 warning`
  - 2 个测试文件 `validate_script` = `0 error / 0 warning`
  - fresh console = `errors=0 warnings=0`
  - targeted EditMode tests 已跑过
- 当前阶段：
  - `代码层 no-red + targeted probe 已成立`
  - `真实体验仍待 fresh live`
- 当前恢复点：
  - 下一轮优先拿 live 看 Prompt 空白/缺字、独白背景回正、Workbench 左列与越界。

## 2026-04-05｜继续真修：Prompt 过长空白收口到 legacy footer 基线
- 当前主线：
  - 用户中途补图后，当前 Prompt 主矛盾进一步收敛为：内容不多，但 legacy 壳把 footer 旧底线留得过深，导致整块背景被硬拉长。
- 本轮实际做成：
  1. `PromptOverlay` legacy footer 不再死守大段旧 baseline；
  2. 只在旧 baseline 与当前内容非常接近时才保留小间距，否则直接跟内容流走；
  3. `PromptOverlay` 与 `Workbench` 当前 own 脚本都拿到了 `manage_script validate = 0 error / 1 warning`；
  4. 当前 fresh console 读到 `0 error / 0 warning`。
- 当前判断：
  - 这轮已经不只是静态推断，而是拿到了比上一刀更像 fresh 的 Unity 侧 no-red 证据；
  - 但缺 GameView/live，所以仍不能宣称玩家面体验已经稳定。
- 当前恢复点：
  - 下轮如果继续，优先直接看 Prompt 高度是否已压回正常，不再留下大块空白。

## 2026-04-05｜继续真修：玩家面三处回归同步回正
- 当前主线：
  - `UI` 线继续只收玩家可见回归，不回漂到底座重构；这轮同步处理：
    1. `PromptOverlay` 任务列表上飘
    2. `DialogueUI` 独白底板与字体
    3. `Workbench` 左列空白与浮动块双开
- 本轮实际做成：
  1. `PromptOverlay` 的 legacy footer 高度不再吃已经被拉坏的旧 rect 高度，继续把几何基线压回可控默认；
  2. `DialogueUI` 独白态改为隐藏背景图并切到 `DialogueChinese SoftPixel SDF`，不改普通对话其他面；
  3. `Workbench` 停止每次 refresh 都强制重建 manual recipe 行，并且加了一层“主面板可见时绝不显示 floatingProgress”护栏。
- 当前验证：
  - 3 个 touched UI 脚本 `manage_script validate` 都是 `0 error / 1 warning`
  - fresh console = `0 error / 0 warning`
  - 当前仍缺 live / GameView 复核，所以只能 claim `代码层 no-red，体验待复核`
- 当前状态：
  - 已 `Park-Slice`
  - `PARKED`

## 2026-04-05｜Tooltip / 状态条 / 玩家气泡 5 文件未提交改动只读复审
- 当前主线：
  - 本轮不是继续真修，而是只读复审当前 5 个未提交脚本的实际推进度，回答“还缺什么、哪些只是表现层、哪些会碰逻辑、最该先落哪 3 点”。
- 已完成事项：
  1. 只读复审了：
     - `Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs`
     - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
     - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerToolFeedbackService.cs`
  2. 并额外只读核对了相关调用链，确认这轮未提交改动已经补上的项：
     - 槽位 Tooltip 现在已把 `sourceTransform` 传回 `ItemTooltip`，背包/箱子与快捷栏的首选 Canvas / 跟随边界不再继续分裂；
     - runtime fallback Tooltip 已补 `qualityIcon` 与优先字体，不再是上一轮那种明显缺件的兜底壳；
     - `PlayerThoughtBubblePresenter` 新增的 typed-text / 前景排序接口，已经被 `PlayerNpcChatSessionService` 实际接入。
  3. 当前仍明显没收掉的点：
     - Tooltip 仍被 `MinimumShowDelay = 1f` 硬钳住，hover 反馈偏慢；
     - 状态条虽然补了淡入淡出，但这轮没有把“失败使用也要亮条”接进去，仍更像 hover / 选中 / 成功使用后的表现优化；
     - 玩家工具反馈气泡在 NPC 会话中会直接静默丢弃，当前只有“避免抢话”这一侧，没有“延后补发/明确优先级裁定”这一侧；
     - 玩家气泡这轮虽然把 typed-text、排序和收尾 API 接上了，但仍缺 fresh GameView / 玩家面证据，不能把结构成立说成体验已过线。
- 关键决策：
  1. `纯表现优化`：
     - Tooltip 壳体、字体、品质图标、淡入淡出、鼠标跟随与边界翻转；
     - 背包/快捷栏状态条的 alpha 渐隐渐显；
     - 玩家气泡的偏移、尾巴、颜色、换行和前景层数值。
  2. `会碰逻辑`：
     - Tooltip 的 `sourceTransform -> preferred canvas -> hover bounds` 链；
     - `InventorySlotUI.ClearSelectionState()` 这类选中态清理；
     - `PlayerThoughtBubblePresenter` 的 typed-text / sort boost / hide 语义；
     - `PlayerToolFeedbackService` 的 replacement tone、NPC 会话期间气泡抑制。
  3. 当前最值得先落的 3 点：
     - 先把 Tooltip 从“结构正确但 1 秒才出”收成真正可用，并补齐所有 `ShowCustom(...)` 调用侧的上下文；
     - 再把失败用具尝试也接进状态条可见反馈，不要只在成功提交后才亮；
     - 最后补一条玩家气泡优先级裁定：NPC 会话期间到底是绝对压掉工具反馈，还是改成延后补发。
- 验证结果：
  - 本轮只做到 `结构 / checkpoint` 级只读判断。
  - 未改代码、未跑测试、未进 Unity。
- 当前恢复点：
  - 若后续继续推进，不需要再重读这 5 个文件；直接从“Tooltip 时机 -> 失败状态条 -> 玩家气泡优先级裁定”这 3 点开刀即可。

## 2026-04-05｜气泡样式 / Resume 失败组只读复核：当前 DLL 已对齐大部分断言，失败报告更像旧结果

- 当前父工作区新增稳定事实：
  1. 这轮只读复核了：
     - `Assets/YYY_Tests/Editor/NPCInformalChatInterruptMatrixTests.cs`
     - `Assets/YYY_Tests/Editor/PlayerThoughtBubblePresenterStyleTests.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
     - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
  2. 这轮没有只停在源码比对，还额外对 `Library/ScriptAssemblies/Assembly-CSharp.dll` 做了离线反射抽样，已经直接确认：
     - `CreateFallbackResumeIntroPlan(BlockingUi, BetweenTurns, 1)` 返回的 `PlayerLine / NpcLine` 与 `NPCInformalChatInterruptMatrixTests.ResumeIntroPlan_ShouldReturnContinuityLines_ForBlockingUiResume` 断言完全一致；
     - `PlayerThoughtBubblePresenterStyleTests` 里可离线验证的断言当前都已对齐，包括：
       - 玩家 preset 偏移 / 透明度 / 颜色区分
       - 玩家与 NPC 的 10 字换行节奏
       - foreground sorting 常量
       - speaker focus boost
       - readable hold pacing 公式
  3. `unityMCP` 这轮定向 `run_tests(EditMode, NPCInformalChatInterruptMatrixTests, PlayerThoughtBubblePresenterStyleTests)` 仍只返回 `total=0`，因此不能把它当成可信失败名来源。
  4. 结合上面两点，用户手里的“`1 个 interrupt + 3 个 style fail`”口径，当前更像：
     - 旧 runner 结果
     - 旧编译快照
     - 或 `run_tests total=0` 这类不可信测试回执残留
     而不像当前磁盘代码和当前 `Assembly-CSharp.dll` 的真实契约缺口。
  5. 当前真正还没被我离线钉死的 style 点，只剩两条运行态依赖较重的测试：
     - `ConversationLayout_ShouldStayCloseToSpeakerHeads_WhileKeepingReadableSeparation`
     - `AmbientBubble_ShouldIgnoreHiddenStaleConversationOwner`
     如果后续还有可信失败，优先继续查：
     - `PlayerNpcChatSessionService.UpdateConversationBubbleLayout()`
     - `NPCBubblePresenter.CanShow(...)`
- 验证结果：
  - `源码 + 已编 DLL 离线反射` 已成立。
  - `unityMCP run_tests` 当前仍是 `total=0`，不可当通过/失败真值。
  - 本轮未改代码、未进 Play Mode。
- 当前恢复点：
  - 若后续继续查这组失败，不该先改文案或乱调样式；应先拿一份可信 fresh runner 结果，再看是否真的还剩 `ConversationLayout` / `AmbientBubble` 两条运行态断言。

## 2026-04-05：SpringUI 子工作区中途续工继续排错壳，当前明确排除了一个左上任务清单假 owner

- 当前父层新增稳定事实：
  1. 子工作区这轮中途没有继续盲改代码，而是先用 Unity scene 只读层级排查“左上任务清单到底是谁”。
  2. 当前已明确排除：
     - `SpringDay1StatusOverlay` 只是右上状态条，不是左上任务清单。
  3. 当前 `Primary` scene 的 `UI` 根层只有一个静态 `SpringDay1PromptOverlay`，编辑态没有第二个同名 scene 实例可直接背锅。
  4. 因而子工作区后续仍应继续把任务清单问题压在：
     - `PromptOverlay` 本体
     - `DialogueUI` 的过场 hide/show
     - 以及必要的 runtime/live 取证
     这条线上，而不是再发散到别的假壳。
- 当前父层恢复点：
  - 若继续催 SpringUI 子工作区推进，下一刀就该是：
    1. 继续钉死左上任务清单真实坏态链；
    2. 直接砍 `Workbench` 左列/prefab 壳/右侧 detail 的核心方法段；
    3. 不要再让它回到“修错对象”的循环里。

## 2026-04-05：SpringUI 子工作区继续真施工，先把 prompt footer / NPC 尾巴 / Workbench prefab 壳三处硬错收口

- 当前父层新增稳定事实：
  1. 子工作区这轮已经从只读回到真实施工，不再停在“继续排查 owner”。
  2. `PromptOverlay` 的 legacy page 高度已补回 page 壳体 inset，目标是把 `FooterText` 拉回米黄色 page 内，不再落到黑底外面。
  3. `NpcWorldHintBubble` 已把箭头从“背景九宫格方块”改回独立三角 sprite，并同步收了箭头尺寸与下偏移。
  4. `Workbench` 这轮最重要的结构纠偏是：正式 prefab `DetailColumn` 不再进入 legacy fallback 重排链，避免继续把正式壳越拉越散。
  5. `Workbench` 的 prefab/reuse 判断还补了字段未预绑时的 fallback 查找，减少把正式壳误判成不可信实例。
- 当前父层验证结论：
  - 子工作区已拿到 4 条轻量 CLI `manage_script validate ... --level basic` 的 `clean` 结果：
    - `SpringDay1PromptOverlay.cs`
    - `NpcWorldHintBubble.cs`
    - `SpringDay1WorkbenchCraftingOverlay.cs`
    - `SpringDay1DialogueProgressionTests.cs`
  - 但 `validate_script` 直跑仍被本机 `dotnet 20s timeout` 卡住，因此父层目前只能认“代码层轻量 clean”，不能代替 live 体验过线。
- 当前父层恢复点：
  - 下一轮如果还要继续催 UI 线程，优先顺序应改为：
    1. 先等用户 fresh live 看这 3 个刚修的真点；
    2. 如果 `Workbench` 仍有坏态，再继续顺着 `TryBindRuntimeShell / RefreshRows / RefreshSelection` 深挖，不要再回到泛收 UI 全链。

## 2026-04-05：SpringUI 子工作区继续深修，已定位并切掉 task footer / recipe text 的同源坐标错法

- 当前父层新增稳定事实：
  1. 子工作区根据 fresh 截图继续往下查后，已明确：`PromptOverlay` 和 `Workbench` 各自都存在同一个 `SetTopKeepingHorizontal()` 误判问题。
  2. 这条误判的本质是：
     - 把“横向拉伸、纵向不拉伸”的节点错当成“纵向拉伸”处理；
     - 结果直接把 `FooterText`、recipe `Name/Summary` 这类文本写到容器外面。
  3. 因此这轮不是继续调高度，而是直接改了这条共享坐标写法。
  4. `Workbench` 右侧标题缺失还额外有一层 prefab 自带坏几何，子工作区已补了最小 `NormalizePrefabDetailShellGeometry()` 来把标题/简介拉回可见区。
- 当前父层验证结论：
  - 子工作区再次拿到轻量 CLI `clean`：
    - `SpringDay1PromptOverlay.cs`
    - `SpringDay1WorkbenchCraftingOverlay.cs`
    - `SpringDay1DialogueProgressionTests.cs`
  - 这轮仍是代码层 clean，不等于 live 体验已过。
- 当前父层恢复点：
  - 下一轮若继续催 UI 线程，优先让它等用户 fresh 验：
    1. task footer 是否回 page 内；
    2. workbench 左列是否恢复；
    3. workbench 右侧标题是否恢复；
  - 若仍坏，再升级到运行态实例绑定排查。

## 2026-04-05：SpringUI 子工作区继续往下收，已把 legacy page 根节点误绑与 workbench 左列空壳硬恢复补齐

- 当前父层新增稳定事实：
  1. 子工作区已确认：`PromptOverlay` legacy prefab 没有 `ContentRoot` 时，`Subtitle / Focus / Footer` 的 section root 会误回退到整张 `page.root`，这是 `FooterText` 出 page 的更深层根因。
  2. 子工作区已把这条回退改成 `ResolveLegacySectionRoot()`，直接 child 文本会回到文本本体，不再把 page 自己当成 footer 容器。
  3. 子工作区还把 legacy page 在 `contentRoot == page.root` 时的额外 shell inset 清掉，目标是让 page 只按真实内容自适应，外层黑壳不再单独空撑。
  4. `Workbench` 左列这轮新增了更硬的自愈链：
     - 若 prefab row 仍不可见、仍不在 viewport 内、或只剩空壳，会直接切到 runtime prefab-style row 重建，而不是继续复制坏模板。
  5. `Workbench` 玩家面标题已新增内部 ID 识别，`Axe_0 / Pickaxe_0` 这类 `recipeName` 会优先让位给 `item.itemName`。
- 当前父层验证结论：
  - 子工作区再次拿到 3 条轻量 CLI `clean`：
    - `SpringDay1PromptOverlay.cs`
    - `SpringDay1WorkbenchCraftingOverlay.cs`
    - `SpringDay1DialogueProgressionTests.cs`
  - 文本层 `git diff --check` 无新的坏口，只有 CRLF/LF warning
  - 但父层仍只能认“代码层 clean + targeted guard 已补”，不能代替用户 fresh live
- 当前父层恢复点：
  - 下一轮如继续催 UI 线程，优先顺序应固定为：
    1. 先 fresh 验任务卡 `FooterText / 黑壳高度`
    2. 再 fresh 验 Workbench 左列与标题
    3. 若左列仍空，再直接升到运行态实例 / viewport 几何排查，不再回到泛调参数

## 2026-04-05：SpringUI 子工作区继续深修，已把 legacy page 根 stretch 真因切掉，并把 workbench 左列恢复改成强制正式 row

- 当前父层新增稳定事实：
  1. 子工作区继续确认：任务卡 page 还会过长的更深层根因，是 legacy prefab 的 `Page` 根本身仍是纵向 stretch；这会让 `sizeDelta.y` 在父壳高度之外再叠一层页面高度。
  2. 子工作区已经在 `PrepareLegacyPage()` 里加了 `NormalizeLegacyPageRoot(page)`，把 legacy `Page` 根回正成固定页面几何。
  3. `Workbench` 左列这次继续空白，不是恢复链没进，而是恢复链还在保留坏模板；子工作区已经把这条恢复统一改成 `forceRuntimePrefabStyle: true`。
  4. `Workbench` 玩家面名称也进一步收了真值：因为 `item.itemName` 同样是内部 ID，所以子工作区新增了内部名到玩家面的直接映射，不再只在两个内部 ID 之间互切。
- 当前父层验证结论：
  - 子工作区这轮再次拿到 3 条轻量 CLI `clean`
  - `git diff --check` 仍只有 CRLF/LF warning
  - 当前仍是代码层 clean，不代替 fresh live
- 当前父层恢复点：
  - 下一轮如果继续催 UI 线程，优先看：
    1. 任务卡 `page` 是否还无故拉长
    2. `Workbench` 左列是否终于可见
    3. `Workbench` 标题是否已经变成玩家面中文工具名

## 2026-04-06：SpringUI 子工作区继续只收玩家面 UI 三条真问题，当前已到代码层 clean、live 待 fresh 验

- 当前父层新增稳定事实：
  1. 子工作区这轮没有再泛收 `Workbench` 全链，而是继续只盯：
     - 任务卡基础页高
     - `Workbench` 左列可读性
     - 正式对话主字体统一
  2. `PromptOverlay` 已把 legacy page 的最小高度抬回 `page.defaultHeight` 这条基础长度，不再允许塌回极小壳。
  3. `Workbench` 左列恢复链已从“等坏 row 自己恢复”推进成“坏了就直接 `ForceRuntimeRecipeRowsIfNeeded()` 重建正式可读 row”。
  4. `Workbench` 右侧详情列已新增统一字体微抬链，目标是把正式面从“太小太虚”拉回可读状态。
  5. `DialogueUI` 的正式对话默认字体已统一切到独白像素字体，继续提示文案收成 `摁空格键继续`。
- 当前父层验证结论：
  - 子工作区轻量 CLI 结果为 `clean`：
    - `SpringDay1PromptOverlay.cs`
    - `SpringDay1WorkbenchCraftingOverlay.cs`
    - `DialogueUI.cs`
    - `SpringDay1DialogueProgressionTests.cs`
  - `validate_script` 直跑 `PromptOverlay` 仍被本机 `dotnet 20s timeout` 卡住，因此当前仍只能认“代码层 clean + console clean”，不能冒充 live 体验已过线。
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 5` 当前为 `errors=0 warnings=0`
- 当前父层恢复点：
  - 下一轮若继续催 UI 线程，优先顺序固定为：
    1. 先 fresh 验任务卡 page 长度
    2. 再 fresh 验 `Workbench` 左列与右侧正文字体
    3. 最后 fresh 验正式对话主字体
  - 若 `Workbench` 左列仍空，直接升到运行态实例 / viewport / mask 现场排查，不再回到纯源码猜测。

## 2026-04-06：SpringUI 子工作区继续深修，任务卡底部关系与 Workbench 左列显示链都已推进到更硬约束

- 当前父层新增稳定事实：
  1. 子工作区这轮已经把 task card 底部关系从“顶推式布局”推进成“底部带约束”：
     - `TaskList` 与 `Focus/Footer` 保留最小间距
     - `FocusRibbon` 与 `FooterText` 保持固定相对距离
     - `FooterText` 与 page 底边保持固定底距
  2. `Workbench` 左列现在不再依赖旧手工 row；子工作区已把 runtime row 升成 `HorizontalLayoutGroup + VerticalLayoutGroup + TextColumn` 的稳定生成式链。
  3. `Workbench` 右侧标题/简介这轮不再只改字号，而是连左右边距和顶部位置一起重排。
- 当前父层验证结论：
  - 子工作区本轮新增 touched 文件的轻量 CLI 仍为 `clean`
  - 当前仍停在代码层 clean，不替代 fresh live

## 2026-04-06：SpringUI 子工作区新增一个只服务调试提速的 director 开关

- 当前父层新增稳定事实：
  1. 子工作区为了加速 `Workbench` 复测，临时补了一刀支撑调试开关，不是换主线。
  2. 开关落在 `SpringDay1Director` 的 `Debug` 区，打开后会直接把 Day1 推到 `FarmingTutorial`，也就是可开 `Workbench` 的 0.0.5 态。
  3. 这刀的目标只是缩短复测路径，不改正式玩家路径完成定义。

## 2026-04-06：SpringUI 子工作区继续把 Workbench 左列坏态往真根因压，并顺手把 NPC/Dialogue 正式面补口收成 compile clean

- 当前父层新增稳定事实：
  1. 子工作区已通过一张 fresh workbench capture 坐实“左列仍然只剩背景壳，icon/文字没真正出来”，不是用户错觉。
  2. 在这张 live 证据基础上，子工作区继续把 Workbench 左列真根因切到 generated row 的 `HorizontalLayoutGroup`：
     - 不只是 row 本体生成了
     - 还必须真实给子项分配宽度和高度
     - 否则 icon / text column 会继续被压成 0 尺寸空壳
  3. 因此子工作区这轮又补了一层更硬的真约束：
     - `rowLayout.childControlWidth = true`
     - `rowLayout.childControlHeight = true`
     - 复用已有 generated row 时也强制把 `generatedLayout.childControlWidth / childControlHeight` 拉回真值
  4. 同轮还把这组玩家面正式显示补口重新压到 compile clean：
     - `DialogueUI` 的 NPC fallback 头像链
     - 玩家独白字色回正
     - NPC / 玩家正式气泡的圆角 body + 倒三角 tail runtime 强制回绑
- 当前父层验证结论：
  - 子工作区本轮涉及的工作台 / Dialogue / NPC / 玩家 / director / test 文件，轻量 CLI 全部 `clean`
  - `errors --count 20 --output-limit 10` 当前为 `errors=0 warnings=0`
  - 但新一轮 fresh workbench 复抓仍被外部 Play / 菜单干扰卡住，所以父层当前只能认：
    - 代码层和 targeted probe 继续前进
    - 真实体验证据仍待补
- 当前父层恢复点：
  1. 下一轮优先补稳定 live 复抓，确认 `Workbench` 左列 icon / 文字是否真的出来；
  2. 若仍失败，直接升级成运行态 row 几何 / mask / viewport 读现场，不再回到源码层猜测。

## 2026-04-06：SpringUI 子工作区继续把 prompt_21 主战场往前压，day1 可直接消费的玩家面 contract 又增加一层

- 当前父层新增稳定事实：
  1. 子工作区这轮继续只砍 `Workbench / DialogueUI / formal-informal-resident 提示壳`，没有漂回任务卡主结构。
  2. `Workbench` 左列恢复链又加硬了一层：
     - runtime 绑定时如果左列还不是 generated row
     - 且正式 recipe 资源可读
     - 就直接 `RebuildRecipeRowsFromScratch(forceRuntimePrefabStyle: true)`
     - 也就是旧 prefab 手工 row 不再继续被 runtime 当成可长期复用真链。
  3. 玩家面提示壳 contract 继续收紧：
     - `SpringDay1ProximityInteractionService.ShouldReplaceCandidate()` 改成先比 `Priority` 再比 `Distance`
     - formal 不再被更近的 informal / resident 抢走
     - formal 左下角任务 copy 不再依赖旧 generic prompt 串
  4. 子工作区还顺手把 formal consumed 后的最小 resident 回落语义补进玩家面：
     - `日常交流`
     - `按 E 聊聊近况`
  5. automatic nearby feedback 现在在 formal priority phase 会直接停掉，并立刻回收已有环境气泡，不再让 formal 接管瞬间残留旧反馈。
- 当前父层验证结论：
  - `python scripts/sunset_mcp.py errors`：`errors=0 warnings=0`
  - `python scripts/sunset_mcp.py status`：baseline `pass`、bridge `success`、`isCompiling=false`
  - touched files 的 `git diff --check`：未见新的 owned 语法/空白问题
  - 但 `validate_script` 仍被 `subprocess_timeout:dotnet:60s` 卡住
- 当前父层阶段判断：
  - 子工作区这轮可被父层吃回的是：
    - `Workbench` 左列 runtime 恢复链更接近真实可用
    - one-shot 玩家面提示壳 contract 更贴近 formal / informal / resident 真规则
    - fresh console 当前 clean
  - 但仍不能被父层包装成：
    - `Workbench live 已过线`
    - `DialogueUI live 已过线`
- 当前父层恢复点：
  1. 下轮优先让子工作区补 `Workbench` 修后 fresh live
  2. 再补 `DialogueUI` 正式面 fresh live
  3. 本轮已给 day1 新增阶段回执：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_UI线程_给day1阶段回执_25.md`

## 2026-04-06：父层补记，UI 线程只读侦查已把 continue / 字体 / 头像 fallback / 三角尾巴的最新首嫌重新收窄

- 当前父层新增稳定事实：
  1. 子工作区本轮没有继续施工，而是按用户要求退到只读审查，重新核了：
     - `DialogueUI.cs`
     - `NpcWorldHintBubble.cs`
     - `PlayerThoughtBubblePresenter.cs`
     - 以及直接关联的 `NPCBubblePresenter.cs / DialogueFontLibrarySO.cs / DialogueChineseFontRuntimeBootstrap.cs / DialogueNode.cs / DialogueManager.cs / SpringDay1UiLayerUtility.cs / PlayerNpcChatSessionService.cs`
  2. `DialogueUI continue` 回弹当前首嫌已被重新压实到 `DialogueUI.cs` 这轮 dirty：
     - `ContinueButtonDisplayText = 摁空格键继续`
     - `NormalizeContinueButtonCopy()` 会把多种旧文案强改回这句
     - 空文本阶段的字体探针也直接使用这句
     - 因此 continue 文案与 continue 字体 fallback 已被绑成同一条硬编码链
  3. 这条 continue 链还带了一个新的字体隐患：
     - `DialogueChineseFontRuntimeBootstrap` 的 warmup 文本当前并不包含 `摁`
     - 但 continue 探针包含 `摁`
     - 所以当前存在“正式对话正文能显示，但 continue 这句单独误切 fallback / 掉字”的结构风险
  4. 正式对话与独白字体当前也被新代码强耦合：
     - `DialogueUI.ApplyFontPresetForNode()` 默认把正式对话也送到 `innerMonologueFontKey`
     - 但独白展示又额外走 `ApplyInnerMonologuePresentation()` 里的硬编码 `SoftPixel`
     - 父层当前因此认定：正式对话 / 独白字体表面想统一，底层却仍有“两条事实源”，后续极易再回弹
  5. “NPC / 正式气泡方框化”的首嫌当前不在 `NpcWorldHintBubble.cs`：
     - world hint 当前仍是独立三角 indicator
     - 真正高风险点在 `NPCBubblePresenter.cs`：旧 `NPCBubbleCanvas` 可被复用，但复用后不会像 `PlayerThoughtBubblePresenter` 那样强制把 body / tail sprite 重新绑回圆角+三角
     - 所以旧壳残留就是父层当前最优先怀疑的回弹口
  6. 玩家气泡这轮的代码面反而比 NPC 气泡更安全：
     - `PlayerThoughtBubblePresenter` 当前每次刷新都会 `EnsureBubbleShapeSprites()`
     - 它不像 `NPCBubblePresenter` 那样缺少“复用后重绑 shape”护栏
- 当前父层 owner 判断：
  - 仍属于 UI 线程自己能收的：
    1. `DialogueUI.cs` 的 continue / 字体 / 头像 fallback
    2. `NpcWorldHintBubble.cs` 的 world hint 三角尾巴链
    3. `PlayerThoughtBubblePresenter.cs` 的玩家气泡表现层
  - 父层不建议 UI 线程自己吞的：
    1. `NPCBubblePresenter.cs`
    2. `PlayerNpcChatSessionService.cs`
    3. 正式对话 / informal 会话 runtime 编排 owner
- 当前父层验证结论：
  - 本轮只站住静态源码审查与 recent diff / recent commit 归属核对
  - 没有新增实现
  - 没有 fresh live
  - 所以父层这轮只能认“回弹点重新缩圈了”，不能认“体验已过线”
- 当前父层恢复点：
  1. 若 UI 线程下一刀继续施工，优先先切 `DialogueUI.cs` 的 continue 文案与字体单一事实源
  2. 若用户继续报 NPC 正式气泡方框化，优先把问题压回 `NPCBubblePresenter.cs` 的旧壳复用链，不再先去动 `NpcWorldHintBubble.cs`

## 2026-04-08｜Workbench 取消链只读补记

- 用户点名要求只读排查 `SpringDay1WorkbenchCraftingOverlay.cs` 的 Workbench 取消/中断逻辑。
- 当前最稳结论：
  - “中断当前 recipe 后别的 recipe 也一起被清掉”的一级直接根因，是 Workbench 整体只靠一个 `_craftRoutine` 驱动，而 `CancelActiveCraftQueue()` 会停掉这个全局协程，却不接续 `FindNextPendingQueueEntry()`。
  - “之后看起来整个队列都没了”的二级放大器，是 `CleanupTransientState()` 仍会在 pending-only 状态下清空 `_queueEntries`。
- 后续若进入实修，最小安全切口优先级：
  1. 先让 `CancelActiveCraftQueue()` 在取消当前 active entry 后切到下一条 pending entry，避免全局 dispatcher 被一起杀死。
  2. 再决定是否把 pending-only 队列也纳入 `HasWorkbenchFloatingState` 或避免 `Hide/OnDisable` 直接 `_queueEntries.Clear()`。

## 2026-04-08｜收到 Day1 新 prompt：UI 本轮只收打包字体链
- 新 prompt：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\2026-04-08_UI线程_day1最终闭环前打包字体与启动链prompt_07.md`
- 新共识：
  1. 用户 profiler 已坐实启动大卡顿主峰不在 UI；
  2. UI 这轮不再误吞启动卡顿主责；
  3. 这轮唯一主刀收回到：
     - 打包字体异常
     - 编辑器/打包版字体链一致性

## 2026-04-08｜父层补记：UI 子线程已开始收 Day1 打包字体链，Farm 箱子 E 键链只补守门不重写主链

- 子线程这轮真正推进的不是 `Workbench`，而是两条更窄的新切片：
  1. `Day1` 打包字体链
  2. `Farm` 箱子 `E` 键近身交互守门
- 当前对子线程最稳的新判断：
  1. 打包字体链的真根因已压到 `DialogueChineseFontRuntimeBootstrap`：
     - 动态中文字体在 build 后 atlas 会先被清空；
     - 旧逻辑会在 `TryAddCharacters(...)` 之前先用 `HasUsableAtlas()` 把字体直接判死；
     - 这会造成“编辑器正常，打包后缺字/回退异常”。
  2. 子线程已把这条提前判死逻辑改掉，并补了一个 build-like 测试：
     - `TMP_FontAsset.ClearFontAssetData(true)` 后仍能通过 `CanRenderText(...)` 补回中文字形和 atlas。
  3. `Farm` 这边没有再重写箱子运行时主链，而是确认：
     - 当前 shared root 里 `ChestController -> SpringDay1ProximityInteractionService -> OnInteract()` 已接上；
     - `GameInputManager` 的右键自动走近旧链仍在；
     - 新增的是 `ChestProximityInteractionSourceTests.cs` 这类最小守门测试。
- 当前父层验证状态：
  - 子线程本轮 own scope 的 `validate_script` 都是 `owned_errors=0 / external_errors=0 / unity_validation_pending`；
  - `sunset_mcp.py status` 读到 1 条外部红：
    - `Assets/YYY_Tests/Editor/NpcResidentDirectorRuntimeContractTests.cs(104,20)` 缺 `NPCAutoRoamController`
- 当前父层阶段判断：
  - `UI` 子线程这轮已经从“只读判断打包字体像有问题”推进到“真修了 build 差异点”；
  - 但 packaged build / live 字体终验仍待补；
  - 箱子 `E` 键链当前可被父层吃回成“主链已在，守门已补，fresh live 终验仍待补”。

## 2026-04-08｜父层补记：箱子 E 键已从“能开”推进到“开关同源 toggle 闭环”

- 当前对子线程新增可消费结果：
  1. `ChestController.OnInteract()` 现在经由 `OpenBoxUI()` 收成真正 toggle：
     - 同箱已开时，再次交互会关闭，而不是无响应。
  2. `SpringDay1ProximityInteractionService` 也补了 page-open 例外：
     - 只有“同一个箱子自己已打开”时，才允许 proximity 候选在箱子页打开期间继续存活。
  3. `ChestProximityInteractionSourceTests.cs` 已把这两个新事实锁进源码守门。
- 当前 fresh 状态：
  - `sunset_mcp.py status` 已回到 `error_count=0`；
  - 剩余仅 2 条外部 warning，不是 blocking red。

## 2026-04-09｜父层补记：任务清单与 Toolbar 是“层级同级、治理分裂”

- UI 子线程本轮新增只读审查结论：
  1. `SpringDay1PromptOverlay` 与 `ToolBar` 在 `UI` 根下确实是同级孩子；
  2. 但 `PromptOverlay` 不是 toolbar 那种普通 persistent UI，而是独立 `Canvas + CanvasGroup + overrideSorting` 的 overlay 实现；
  3. `PackagePanelTabsUI` 打开时也用独立 `Canvas(sortingOrder=181)`，再额外手动调用 `SetExternalVisibilityBlock()` 去压 `PromptOverlay`；
  4. 所以当前“任务清单像 toolbar 一样处理”并不成立，用户怀疑“整体处理方法就错了”是有代码依据的。
- 当前父层判断：
  - 后续如果继续修任务清单，不该再往更多 UI 里散落 suppress 逻辑；
  - 应优先统一 `PromptOverlay` 的父层治理口径，至少把“何时退场、何时恢复”的责任集中起来。

## 2026-04-09｜父层补记：任务清单已开始回收到父级 Canvas 治理

- UI 子线程这轮已实际落地：
  1. `PromptOverlay` 现在优先继承父级 `UI` 根 Canvas 的治理口径，不再默认强制独立 `overrideSorting=152`。
  2. `PackagePanelTabsUI` 打开/关闭时，已删掉对 `PromptOverlay.SetExternalVisibilityBlock()` 的 scattered suppress 调用。
  3. `PromptOverlay.ShouldDelayPromptDisplay()` 也已去掉“Package/Box 打开就必须自我消失”的硬编码判断。
- 当前父层判断：
  - 这说明任务清单已经从“独立 overlay + 各面板到处压它”开始往“跟父级 UI 根一起治理”回收；
  - 这轮仍未拿到 fresh compile/live 证据，但结构方向已经从根上改对。

## 2026-04-09｜父层补记：背包/包裹页下任务清单隐藏已重新收回统一 utility

- UI 子线程这轮继续补口：
  1. 把“包裹页 / 背包壳打开时任务清单应隐藏”的判断重新集中回 `SpringDay1UiLayerUtility`
  2. `PromptOverlay` 自己通过 utility 决定是否退场
  3. 不再恢复到 `PackagePanel` 手动压 `PromptOverlay` 的旧散链
- 当前父层判断：
  - 这条 residual 现在已经从“结构回正但缺模态隐藏”推进到“父层口径 + 模态口径都开始统一”；
  - 余下主要看用户 live 体感是否彻底过线。

## 2026-04-09｜父层补记：Workbench 已切断跨场景显示链，HP/EP 已补运行时分辨率保底

- UI 子线程本轮新增稳定结论：
  1. `Workbench` 当前不再把“切场景隐藏”做成“清空工作台状态”：
     - scene 切换时只断开旧 `_anchorTarget` 和悬浮显示
     - `_queueEntries / readyCount / active craft` 仍保留在原 scene 语义下
     - 对外暴露的 `HasActiveCraftQueue / HasReadyWorkbenchOutputs` 改成 scene-aware，避免 `Home` 继续看到旧工作台完成态
  2. `HP/EP` 当前不是改剧情数值，而是补 runtime layout 保底：
     - 记录场景原始 anchoredPosition
     - 分辨率变化时恢复原位后按 root canvas 可视范围 clamp
     - 目标是“不同 build 分辨率下仍留在屏内”
- 当前父层验证状态：
  - `WorkbenchCraftingOverlay.cs`：
    - `validate_script = unity_validation_pending`
    - `owned_errors=0 / external_errors=0`
  - `HealthSystem.cs + EnergySystem.cs`：
    - `validate_script = unity_validation_pending`
    - `owned_errors=0 / external_errors=0`
  - fresh console：
    - `sunset_mcp.py errors` = `errors=0 warnings=0`
- 当前父层判断：
  - 这轮可以交用户做的不是“泛 UI 再看看”，而是两条非常具体的 live retest：
    1. workbench 产出不再穿场景
    2. build 分辨率变化下 `HP/EP` 不再飞出屏幕

## 2026-04-09｜父层补记：任务清单已从“自抬排序 overlay”收回父级基础层

- UI 子线程这轮新增稳定结论：
  1. `PromptOverlay` 在有父级 `UI` Canvas 时，不再 `overrideSorting=true` 把自己抬成特殊高层；
  2. `Package/Box` 打开也不再触发 `PromptOverlay` 自己 fade/隐藏；
  3. 当前真实语义已经改成：
     - `PromptOverlay = 基础层`
     - `PackagePanel = 更高模态层`
     - 谁该盖住谁，由层级决定，不再由 Prompt 自己退场假装解决
- 当前父层验证状态：
  - `PromptOverlay.cs + SpringDay1LateDayRuntimeTests.cs`
    - `validate_script = no_red`
    - `owned_errors=0 / external_errors=0`
  - fresh console：
    - `errors=0 warnings=0`
- 当前父层判断：
  - 这刀比“再补 suppress/fade”更接近用户真实语义；
  - 余下就该交用户看实际屏幕层级效果，而不是继续在代码里叠透明度补丁。

## 2026-04-09｜父层补记：PromptOverlay 已补上基础层父 Canvas 选择与运行时回正

- UI 子线程这轮继续把任务清单从“视觉上像独立 overlay”往真正基础层推进：
  1. 不是再补 fade，而是补 `ResolveParent` 的基础层 Canvas 选择；
  2. 运行时每次 `EnsureBuilt` 都会先校正挂载父层并重套 canvas 默认值；
  3. 即使实例一度挂错到 `PackagePanel` 模态 Canvas，下次刷新也会自动回正。
- 当前父层验证状态：
  - `PromptOverlay.cs + SpringDay1LateDayRuntimeTests.cs`
    - `validate_script = no_red`
    - `owned_errors=0 / external_errors=0`
- 当前父层判断：
  - 这刀比“继续叠 suppress/透明度”更接近用户真实语义；
  - 当前真正待用户看的，是 live 层级关系是否终于像 `toolbar / state`，而不是代码里又多了一个隐藏开关。

## 2026-04-09｜父层补记：工作台工具 recipe 与材料链真值已核实

- UI 线程只读确认：
  1. `SpringDay1WorkbenchCraftingOverlay` 当前只从 `Resources/Story/SpringDay1Workbench` 读 recipe；
  2. 当前目录里只有 `Axe_0 / Hoe_0 / Pickaxe_0 / Sword_0 / Storage_1400` 五个 recipe；
  3. 工作台 UI 当前排序不是纯 ID，而是 `Axe -> Hoe -> Pickaxe -> 其他` 后再按 `recipeID`。
- 数据真值：
  - 材料 SO 有 `3200 木料 / 3201 石料 / 3000 黄铜矿 / 3001 生铁矿 / 3002 黄金矿 / 3100 黄铜锭 / 3101 生铁锭 / 3102 黄金锭`
  - 工具 SO 有 `Axe 0~5 / Pickaxe 0~5 / Hoe 0~5 / Sword 0~5`
- 关键风险：
  - `MaterialTier` 有 `Steel(4)`，但当前没有钢材料 SO；
  - `Sword_1~5` 武器 SO 本体现在都还是 `materialTier=0`，不适合直接当真高阶武器链。

## 2026-04-09｜补记：WateringCan 与 Storage SO 已核实

- `WateringCan` 只有 `itemID 18` 这一档；
- `Storage` 现有 `1400/1401/1402/1403` 四个箱子 SO，容量分别为 `12/24/36/48`；
- 因此下一步若开做，工作台清单可以安全扩到：工具 `0/1/2/3/5` 档 + 洒水壶 + 四个箱子，但仍应跳过钢档和高阶剑链。

## 2026-04-09｜父层补记：Workbench recipe 扩展与工作台内 queue toast 已落地

- UI 子线程本轮继续只收工作台，不回漂别的 UI：
  1. `SpringDay1WorkbenchCraftingOverlay` 的 recipe 排序已改成按 `resultItemID`；
  2. 工作台内已新增本地 queue toast，只在队列来源数超过 `6` 且继续追加制作时显示；
  3. 兼容旧 prefab：即使 prefab 还没手存这个节点，runtime 也会自动补出 toast；
  4. `Resources/Story/SpringDay1Workbench` 已补齐缺失 recipe，当前总清单已扩到：
     - `Axe_0/1/2/3/5`
     - `Pickaxe_0/1/2/3/5`
     - `Hoe_0/1/2/3/5`
     - `WateringCan`
     - `Sword_0`
     - `Storage_1400/1401/1402/1403`
- 父层验证状态：
  - `SpringDay1WorkbenchCraftingOverlay.cs`
    - `validate_script = unity_validation_pending`
    - `owned_errors=0`
    - 当前 `errors` fresh console = `0/0`
- 父层判断：
  - 这刀已经把“可放进去的 recipe 面”和“超过 6 项时的玩家提示面”补到了当前代码层可继续验的状态；
  - 仍未把钢档和高阶剑链冒充成已完成内容。

## 2026-04-09｜父层停车补记：Workbench 这刀已停到 live 待验

- 当前状态：
  - `PARKED`
- 当前 blocker：
  - `user-live-validation-pending`
- 备注：
  - 本轮不再继续扩写别的 UI，下一步应直接看工作台 live 结果。

## 2026-04-09｜父层补记：Workbench 命名问题根因已只读确认

- 当前只读确认：
  1. `WateringCan`、`3101_生铁锭`、`3102_黄金锭` 不是显示时偶发串值，而是底层 `itemName` 本体就这么写；
  2. `BuildMaterialsText()` 直接把原始 `itemName` 上屏；
  3. `GetRecipeDisplayName()` 的内部名兜底过粗，只覆盖少数英文词，而且会把多档工具压成泛名；
  4. 新增 recipe 的 `description` 里还混入了明显开发口吻。
- 父层判断：
  - 这不是单个 recipe 文案坏，而是“数据层内部名直接冒充玩家面文案”的结构性问题；
  - 下一刀正确方向应是补工作台专用的玩家面文案层，而不是继续抄 SO 原字段。

## 2026-04-09｜父层补记：Workbench 玩家面文案层已接管

- UI 子线程本轮继续只收工作台文案链：
  1. 名称：已不再直接信任 `recipeName/itemName` 原值；
  2. 简介：已改成工作台玩家面简介映射；
  3. 材料名：已做显示层清洗，去掉内部 `3101_` 这类前缀。
- 父层验证状态：
  - `SpringDay1WorkbenchCraftingOverlay.cs`
    - `validate_script = unity_validation_pending`
    - `owned_errors=0 / external_errors=0`
    - `errors = 0 / warnings = 0`
- 父层判断：
  - 这刀已经把最刺眼的“内部命名直接冒出来”问题从结构层收住；
  - 还差用户 live 看一眼真实玩家面是否已经顺眼。

## 2026-04-09｜父层停车补记：Workbench 文案修正已停到 live 待验

- 当前状态：
  - `PARKED`
- 当前 blocker：
  - `user-live-validation-pending`

## 2026-04-09｜父层补记：UI 性能/峰值问题当前根因判断

- 当前根因判断：
  1. 主因是 Spring UI 刷新模型过重，尤其是 Workbench 与 PromptOverlay 的立即布局重建、文本网格重建和每帧状态驱动；
  2. 次因是工作台 proximity/hint 链每帧重复上报，持续刺激 EventSystem 与 overlay 刷新；
  3. 附因是导演链自身仍有 GC 热点，但暂不是这条 UI 峰值问题的第一刀主目标。
- 当前口径：
  - 性能优化不能以牺牲功能、需求语义、交互闭环、玩家体验为代价；
  - 下一刀应先做不改体验的止血型刷新治理。

## 2026-04-10｜父层补记：UI 刷新治理已从分析进入代码落地

- 当前已落实：
  1. `Workbench`：制作中逐帧刷新已从全量底部 UI 重刷，收缩到更轻的 progress visual 更新；
  2. `Workbench`：悬浮卡状态改成复用 buffer / pool，不再每帧重新分配排序结果；
  3. `PromptOverlay`：被 formal dialogue / modal 压住时不再白建 state；
  4. `PromptOverlay / Workbench`：文本没变时不再强制 `ForceMeshUpdate()`；
  5. `Proximity`：左下角提示文案签名缓存已落地，内容不变时不再重复推 overlay。
- 当前验证：
  - 代码层 `validate_script` 对 4 个脚本均无 owned red；
  - fresh console 仍被本机 MCP baseline 卡住，live 待补。
