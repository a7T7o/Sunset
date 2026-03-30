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
