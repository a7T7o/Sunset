# 0.0.2 - 玩家面集成与性能收口

## 模块概述
- 本阶段只承接当前 UI 活线：
  - workbench / prompt / proximity
  - task-list / modal / formal dialogue 玩家面关系
  - packaged 字体链
  - ScreenSpaceOverlay 最终证据

## 当前状态
- **最后更新**：2026-04-10
- **状态**：新活跃阶段已建立

## 当前稳定结论
- 当前 UI 线不能再把“性能优化”写成牺牲功能与需求的借口
- 工作台、左下角 prompt、proximity 上报链的刷新风暴已经做过止血第一刀
- 后续更该补 live 体验证据，而不是继续泛重构 UI 壳

## 当前恢复点
- 后续 UI 新问题若属于玩家面集成、workbench、prompt、proximity、packaged/live 体验，一律先归这里

## 2026-04-10 只读结论补记｜任务清单治理模型与右上角调试字分辨率差异
- 用户目标：
  - 不改代码，先彻底厘清两件事：
    1. 为什么任务清单始终不像 `toolbar / state` 一样稳定
    2. 为什么右上角调试内容在 `16:10 / 16:9` 下体感大小不同
- 已完成事项：
  - 审读 `SpringDay1PromptOverlay.cs / SpringDay1StatusOverlay.cs / ToolbarUI.cs / PackagePanelTabsUI.cs / DialogueUI.cs / TimeManagerDebugger.cs`
  - 确认任务清单的根因不是“文字尺寸”本身，而是它和 `toolbar / state` 根本不是同一套治理模型
  - 确认右上角调试内容来自 `TimeManagerDebugger.OnGUI()` 的 IMGUI 直绘，不受 `CanvasScaler` 治理
- 关键判断：
  - `SpringDay1PromptOverlay` 现在是“独立 runtime overlay + 运行时找 parent canvas + 自己 fade/show/hide + director 手动 Show/Hide + 对话侧再接管一次”的混合体
  - `toolbar` 是普通稳定组件，`state` 是固定 `sortingOrder=145` 的简单 ScreenSpaceOverlay HUD；两者都没有 prompt 这一套动态重绑 parent 与多方显隐
  - `PackagePanelTabsUI` 没有正式接管 prompt；`SpringDay1PromptOverlay.SetExternalVisibilityBlock()` 只有定义没有调用点，所以背包/箱子打开时 prompt 不会像 toolbar 一样自然被 page 盖住
  - `DialogueUI` 之所以比背包场景更稳定，是因为它自己有一套 `FadeNonDialogueUi()` 的 sibling snapshot 治理，会在正式对话时统一淡出非对话 HUD
  - 右上角调试字用的是固定像素 `fontSize=14/22` 加 `Screen.width` 定位；不同宽高比下体感不一致是结构性结果，不是单纯数值没调准
- 对用户语义的回归理解：
  - 任务清单应该更像 `toolbar / state` 那样属于固定 HUD lane
  - 打开背包/箱子/workbench 这类 page UI 时，不应再靠 prompt 自己做透明度演出；正确语义是 page 在上层，prompt 正常待在下层
  - 只有 formal dialogue 这类高语义场景，才该统一淡出非对话 HUD
- 遗留问题 / 下一步：
  - 若下一轮进入真实施工，主刀方向应是把 `PromptOverlay` 收回固定 HUD 层级与统一父层治理，而不是继续调 alpha 或尺寸
  - 若要让右上角调试内容跨分辨率更一致，方向应是迁到 Canvas/CanvasScaler 语义，或至少按参考分辨率自己算缩放

## 2026-04-10｜005 放置模式状态提示续工入口
- `day1` 已补完上游真值，不该再让 UI 自己复制剧情判断：
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
  - 新增：
    - `ShouldShowPlacementModeGuidance()`
    - `GetPlacementModeGuidanceText()`
- 用户最新明确需求：
  - 放置模式必须有独立于交互提示的 `状态提示`
  - 显示位置在交互提示上方
  - 切换时 `0.25s` 渐入 / `2.0s` 常显 / `0.25s` 渐出
  - 进入 `005` 时要主动提醒玩家开启放置模式
  - 状态显示必须和真实 `GameInputManager.IsPlacementMode` 一致
- 给 UI 的最新施工 prompt 已落到：
  - [2026-04-10_UI线程_005放置模式状态提示与引导提示接线prompt_09.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/2026-04-10_UI线程_005放置模式状态提示与引导提示接线prompt_09.md)
- 当前恢复点：
  - UI 这一刀只收 `PlacementMode` 状态提示，不扩到 `toolbar/package/workbench` 壳体

## 2026-04-10 只读结论补记｜放置模式状态提示
- 用户新增目标：
  - 先只读核实“放置模式状态提示”现状是否已有支撑，并给出需求拆分与实现方案
- 已确认支持点：
  - `GameInputManager.IsPlacementMode` 是“玩家当前是否处于放置模式”的主语义真源，`V` 键切换、对话恢复、异常退出都在这里发生
  - `SpringDay1Director.ShouldShowPlacementModeGuidance()` 与 `GetPlacementModeGuidanceText()` 已经提供 0.0.5 农田教学阶段的提示语义与文案
  - `InteractionHintOverlay` 已有左下角交互提示壳、样式资源、底部锚点与持久化桥接
- 已确认缺口：
  - 目前没有任何独立的“放置模式状态提示 UI”
  - `PlacementManager.OnPlacementModeChanged` 只覆盖可放置物/种子的预览态，不覆盖锄头/水壶这类也受 `IsPlacementMode` 约束的农具语义，不能拿来做总状态真源
  - `InteractionHintOverlay` 当前是单卡结构，`ShowPrompt/Hide` 直接控制整张卡，没有“上方状态卡 + 下方交互卡”的双卡堆叠能力
  - 当前也没有 0.25/2/0.25 的时序动画、0.0.5 首次提醒、或“切换时重置保持但不重复渐入”的控制器
- 当前判断：
  - 这件事代码上不是“完全没基础”，而是“语义源已有、展示控制器缺失”
  - 最稳妥的实现方向不是再造第三个独立 overlay，而是把 `InteractionHintOverlay` 升级成同一条底部 HUD lane 的双卡结构：交互卡在下，放置状态卡在上

## 2026-04-10 只读补记｜day1 prompt_09 对齐结论
- 已读：
  - [2026-04-10_UI线程_005放置模式状态提示与引导提示接线prompt_09.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/2026-04-10_UI线程_005放置模式状态提示与引导提示接线prompt_09.md)
- 新确认：
  - `day1` 已把上游真值归属说清楚，方向与 UI 线程刚刚的只读判断一致
  - 这轮唯一主刀应固定为 `005 放置模式状态提示`，不能再扩回 `toolbar/package/workbench/prompt` 泛修
  - UI 线程不应再自己猜 `005` 时机，而应直接消费：
    - `GameInputManager.IsPlacementMode`
    - `SpringDay1Director.ShouldShowPlacementModeGuidance()`
    - `SpringDay1Director.GetPlacementModeGuidanceText()`
- 当前判断：
  - 这份 prompt 不是新方向，而是把“上游真值归属”从隐含前提补成了正式施工边界
  - 因此后续施工清单应分成 4 层：状态真值、剧情提醒真值、底部双卡 HUD lane、时序/阻断闭环

## 2026-04-10 只读补记｜工作台 UI 卡顿与全项目同类性能炸弹
- 用户目标：
  - 先彻查 `SpringDay1WorkbenchCraftingOverlay` 的工作台卡顿，判断是不是 UI 自己有硬编码/性能炸弹
  - 再盘全项目同类问题，形成后续规约思路
- 当前证据层级：
  - `结构 / targeted probe` 成立
  - 当前没有 fresh 可读图片证据，不能把这轮包装成最终 runtime 终验
- 已确认的工作台根因：
  - 当前最像 `SpringDay1WorkbenchCraftingOverlay` 自己的布局风暴，而不是导航问题
  - 关键坏链是：
    1. `Open()`、`SelectRecipe()`、`SetQuantity()`、`HandleInventoryChanged()`、craft queue 各节点都会进入重刷新
    2. `RefreshAll()` 一次就串上：
       - `RefreshRows()`
       - `ForceRuntimeRecipeRowsIfNeeded()`
       - `RefreshSelection()`
       - `UpdateQuantityUi()`
       - `RefreshCompatibilityLayout()`
    3. `RefreshRows()` 内对每个 row 强制 `LayoutRebuilder.ForceRebuildLayoutImmediate(...)`
    4. 末尾再对 `recipeContentRect / recipeViewportRect` 强制 rebuild，并 `Canvas.ForceUpdateCanvases()`
    5. 若 `HasUnreadableVisibleRecipeRows()` / `NeedsRecipeRowHardRecovery()` 命中，会直接 `RebuildRecipeRowsFromScratch()`，把旧行命名成 `Obsolete_RecipeRow_*`、销毁重建
  - 这和用户 profiler 里出现的：
    - `LayoutGroup.OnChildRectTransformDimensionsChange`
    - `TMP Parse Text`
    - `TMP.SetArraySizes`
    - `GameObject.AddComponent`
    - `TMP_SubMeshUI.OnEnable`
    - `Obsolete_RecipeRow_*`
    是一致的
- 当前明确的次级放大器：
  - 热路径里反复做 `CanFontRenderText / ResolveFont / ApplyResolvedFontToText / ForceMeshUpdate`
  - 热路径里同时混用：
    - `VerticalLayoutGroup / HorizontalLayoutGroup`
    - `ContentSizeFitter`
    - `LayoutRebuilder.ForceRebuildLayoutImmediate`
    - `Canvas.ForceUpdateCanvases`
  - `HandleInventoryChanged()` 在面板可见时直接整面 `RefreshAll()`，说明库存变化不是局部刷新，而是整面重算
- 当前全项目同类高风险地图：
  - `已证实热点`
    - [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
  - `结构性高危，已做过止血但仍应警惕`
    - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
      - 同样存在 runtime 造壳、任务行项重建、`ForceMeshUpdate`、`ForceRebuildLayoutImmediate`
  - `中风险：打开/悬停时可能有突刺，但频率低于前两者`
    - [ItemTooltip.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs)
    - [PackageSaveSettingsPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs)
  - `结构性高危但暂未命中 live 热点`
    - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
    - [PackageMapOverviewPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs)
      - 共同特征是 runtime `AddComponent + LayoutGroup` 很重，打开时可能贵，但当前没有 workbench 那样的热路径证据
  - `非布局风暴型，但属于打开 UI 时的全树扫描风险`
    - [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs)
      - `EnsureReady()` 时会多次 `GetComponentsInChildren<Transform>(true)` 全树扫
    - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
      - scene rebind 时会成批 `GetComponentsInChildren<>`、`Build/EnsureBuilt/EnsureReady`，最后再 `Canvas.ForceUpdateCanvases()`
- 当前规范判断：
  - 当前 UI 线反复出性能事故，不是单个类“写差了”，而是项目里存在同一种高危模式：
    - runtime code-built UI
    - `LayoutGroup + ContentSizeFitter`
    - 热路径 `ForceRebuildLayoutImmediate / Canvas.ForceUpdateCanvases`
    - 文本/字体兜底放在热路径
    - 自愈式兼容/恢复逻辑直接混进正常刷新
  - 后续优化不该先砍功能，而应先拆：
    1. 打开时的一次性 build/bind
    2. 局部内容刷新
    3. 只在异常时才允许进入 hard recovery

## 2026-04-10 实装收口｜005 放置模式状态提示 + Prompt HUD 治理 + 调试字缩放
- 当前主线：
  - 本轮真正落地 3 件事：
    1. `005` 放置模式状态提示
    2. 任务清单收回固定 HUD 层治理
    3. 右上角调试字接入参考分辨率缩放
- 本轮实际做成了什么：
  - `InteractionHintOverlay.cs`
    - 从单卡交互提示升级为同一条底部 HUD lane 的双卡结构
    - 下卡仍是交互提示
    - 上卡新增 `PlacementStatusCard`
    - 读取 `GameInputManager.IsPlacementMode` 做开/关状态提示
    - 读取 `SpringDay1Director.ShouldShowPlacementModeGuidance()` / `GetPlacementModeGuidanceText()` 做 `005` 自动提醒
    - 动画按 `0.25s 渐入 + 2.0s 常显 + 0.25s 渐出` 跑，且卡片已显示时重触发不会重复叠渐入
    - page UI / formal dialogue 阻断时会收起状态卡，避免假状态残留
  - `SpringDay1PromptOverlay.cs`
    - 任务清单 overlay 改成固定 `ScreenSpaceOverlay + overrideSorting + sortingOrder=142`
    - 不再跟随运行时父 Canvas 的 renderMode / sortingOrder 漂移
    - 目的就是把它收回稳定 HUD 语义，避免像 package/workbench/dialogue 的父 Canvas 一变就一起漂
  - `TimeManagerDebugger.cs`
    - 右上角调试信息改为按参考分辨率计算 GUI scale
    - 新增 `guiReferenceResolution / guiMatchWidthOrHeight / guiScaleClamp`
    - 所有字体、边距、时钟框、快捷键提示框都随统一缩放因子走
- 当前还没做成什么：
  - 没有做 live 场景终验
  - 没有把任务清单彻底迁成 Toolbar 那种单一 Canvas 组件，只先把最核心的“父 Canvas 漂移”切掉
- 当前阶段：
  - 结构和代码层闭环已成，等待用户 live 验收这三条体验线
- touched files：
  - `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
  - `Assets/YYY_Scripts/TimeManagerDebugger.cs`
- No-Red 证据卡 v2：
  - `cli_red_check_command`: `validate_script`
  - `cli_red_check_scope`:
    - `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
    - `Assets/YYY_Scripts/TimeManagerDebugger.cs`
  - `cli_red_check_assessment`: `unity_validation_pending`
  - `unity_red_check`: `blocked`
  - `mcp_fallback`: `required`
  - `mcp_fallback_reason`: `baseline_fail`
  - `current_owned_errors`: `0`
  - `current_external_blockers`: `0`
  - `current_warnings`: `0`
  - 额外说明：
    - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Scripts/TimeManagerDebugger.cs`
    - 结果仅有 `CRLF/LF` warning，无文本层错误

## 2026-04-10 补口｜InteractionHintOverlay 编译阻塞修复
- 用户新反馈：
  - `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs(125,24): error CS1061`
  - 根因是我把实现改名成 `HideAllImmediate()` 后，`EnsureRuntime()` 里还残留旧调用 `HideImmediate()`
- 已处理：
  - 将 `EnsureRuntime()` 中的旧调用改为 `HideAllImmediate()`
- 验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs --count 20 --output-limit 5`
  - 结果：
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - Unity 侧仍未闭环，原因仍是当前无 active Unity instance

## 2026-04-10 收口｜状态提示卡排版补口 + toolbar 病因定位
- 当前主线：
  - 只收 `状态 UI` 的重叠问题，并只读定位 `toolbar 点击失效 / 非放置模式工具不能用`
- 这轮实际做成了什么：
  - `InteractionHintOverlay.cs`
    - 把状态卡尺寸从偏紧的小框放宽为：
      - `StatusCardWidth = 272`
      - `StatusCardCompactHeight = 60`
      - `StatusCardDetailHeight = 82`
    - 把标题区和正文区重新拉开，避免两段文字互相压住
    - 保持状态卡仍在交互提示卡上方同 lane，不回漂到别的 UI
  - 对 `toolbar` 病因已做只读定位：
    - `滚轮` 仍走 `GameInputManager -> HotbarSelectionService.SelectNext/Prev -> EquipCurrentTool`
    - `点击` 走 `ToolbarSlotUI.OnPointerClick()`，但这条链前面叠了多层拦截与 restore
    - 目前更像是“Toggle/选中样式变了，但真正 selection/equip 没稳稳走完”
- 现在还没做成什么：
  - 没有 live 验状态卡最终观感
  - 没有修 `toolbar/工具使用` 逻辑；这轮只报实病因，不越界施工
- 当前阶段：
  - `状态 UI` 已完成代码补口并过最小自检
  - `toolbar` 问题已完成根因定位，但还没进入修复
- 关键证据：
  - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
    - `OnPointerClick()` 内有 package/box/farm-tool-lock 等多层前置拦截
  - `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
    - 真正换手持在 `SelectIndex()` -> `RestoreSelection()` -> `EquipCurrentTool()`
  - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
    - 滚轮与数字键仍直接走 hotbar selection 主链
- 验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs --count 20 --output-limit 5`
  - 结果：
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
    - `current_warnings=1`
  - warning 为外部字体警告：
    - `DialogueChinese V2 SDF (Runtime)` 缺省略号字符 fallback

## 2026-04-10 补口｜状态标签与标题彻底错位
- 用户新反馈：
  - 状态卡虽然恢复显示，但左上角 `状态` 标签仍被标题压住
  - 从截图看不是透明度问题，而是标签和标题占了同一块区域
- 已处理：
  - `InteractionHintOverlay.cs`
    - 把 `StatusTag` 收到更靠左上的小角标区
    - 把 `StatusTitleText` 整体右移并下移，和标签彻底分区
    - 同步缩小标签字和标签底板，避免再和标题抢空间
- 结果预期：
  - 左上只保留一个小标签
  - 标题单独落在右侧一行，不再盖住标签
- 验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs --count 20 --output-limit 5`
  - 结果：
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - 当前阻断：
    - Unity 编辑器状态为 `stale_status`

## 2026-04-10 只读核实｜day1 当前完成情况与 UI 边界
- 本轮性质：
  - 只读核实 `spring-day1` 当前真实进度
  - 未进入 UI 真实施工，未跑 `Begin-Slice`
- 已核实到的代码事实：
  - `SaveManager.cs`
    - fresh start 默认时间已改为 `09:00`
  - `TimeManagerDebugger.cs`
    - 已接入 `SpringDay1Director.TryNormalizeDebugTimeTarget(...)`
  - `SpringDay1NpcCrowdDirector.cs`
    - 已有 `IsNightResting` 状态与夜间阻断 roam 的实际代码
  - `StoryProgressPersistenceService.cs`
    - `dinner / reminder / free-time-intro` 私有位当前按 completed sequence 写，不再由 phase 单独偷推
  - `StoryProgressPersistenceServiceTests.cs`
    - 已有 `Load_DoesNotPromoteLateDayPrivateFlagsFromPhaseAlone`
- git 现场核实：
  - `Modified`
    - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
    - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
    - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
    - `Assets/YYY_Scripts/TimeManagerDebugger.cs`
  - `Untracked`
    - `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
    - `Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
- 当前判断：
  - `day1` 不是只写了文档，关键 runtime 代码确实已经在现场
  - 但这条线仍只能 claim `代码层推进成立`，不能 claim `live / packaged 已闭环`
  - `PromptOverlay` 当前继续按 day1 裁定冻结，UI 不再对 `SpringDay1PromptOverlay.cs` 追加治理补丁
- 对 UI 的恢复意义：
  - UI 线程接下来继续只守 own：
    - `hotbar / placement / save-load`
    - `status HUD / hint HUD`
  - `task list` owner 拆分继续等 day1 主刀完成

## 2026-04-11 只读盘点｜UI 当前剩余未闭环清单
- 本轮性质：
  - 只读盘点
  - 未进入真实施工，未跑 `Begin-Slice`
- 当前按 owner 划分后的剩余项：
  1. UI own：`hotbar / placement / save-load`
     - 代码层已推进统一入口与 reset
     - 但 live 闭环仍未拿到，尤其是：
       - 点击切槽与滚轮切槽的真实装备/使用结果是否完全一致
       - 读档 / fresh start / 切场后是否始终默认 placement-off
       - 非放置模式下是否还会残留 preview / 工具误触发
  2. UI own：`status HUD / hint HUD`
     - `CanvasScaler` 已补
     - 但还没拿到 `16:10 / packaged` 最终 live 票：
       - 血条/精力条是否仍溢出
       - 状态卡与提示卡是否仍重叠或错位
  3. UI own 但未闭环：`packaged 字体 / 缺字 / 显示异常`
     - 当前没有新的闭环证据
     - 不能 claim 已过
  4. 非 UI own、已 authoritative 转交 `day1`：`PromptOverlay / task list`
     - 这条线当前第一 blocker 是 `formal task card` 与 `manual prompt` 仍在同组件/同状态机混跑
     - UI 这边不应继续往 `SpringDay1PromptOverlay.cs` 上打新的治理补丁
- 当前 git 现场提醒：
  - UI 自己仍有这些文件在脏现场：
    - `GameInputManager.cs`
    - `ToolbarSlotUI.cs`
    - `SaveManager.cs`
    - `SpringDay1StatusOverlay.cs`
    - `InteractionHintOverlay.cs`
    - `SpringDay1PromptOverlay.cs`
  - 其中 `SpringDay1PromptOverlay.cs` 虽仍在现场 dirty，但 owner 裁定已切回 `day1`
- 当前恢复判断：
  - 我自己真正还没闭环的是：
    - 输入/放置/切槽 live 闭环
    - 状态 HUD / hint HUD 的 packaged 分辨率与排版闭环
    - packaged 字体链
  - `task list` 不再算我继续主刀的收口项

## 2026-04-11 实装补口｜16:10 状态条真实边界 + 状态提示卡排版
- 本轮性质：
  - 已进入真实施工
  - `Begin-Slice` 已跑
- 这轮实际落地：
  1. `HealthSystem.cs / EnergySystem.cs`
     - 修正了 `16:10` 溢出判断对象：
       - 之前只按 `Slider` 自己的小父框算边界
       - 现在改为按整条血条/精力条的实际可见子层内容算边界
     - 同时在重新显示 HUD 时强制刷新一次 responsive layout，避免“隐藏时没算，显示时继续沿旧位置”
  2. `InteractionHintOverlay.cs`
     - 状态卡内部重新分层：
       - `状态` 标签缩小
       - 标题移到标签下方独立一行
       - 正文和标题重新拉开
       - 左侧竖线下移并缩短，避免刺进标题区域
- 当前判断：
  - 这轮已经把两个问题从“判断错对象/布局逻辑错位”收回到正确方向
  - 但仍需用户 live 终验：
    - `16:10` 下 HP / EP 是否完全不再出界
    - 状态卡是否终于不再出现标签/标题打架
- 代码层验证：
  - `HealthSystem.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
  - `EnergySystem.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
  - `InteractionHintOverlay.cs`
    - `assessment=external_red`
    - `owned_errors=0`
    - 外部阻断为 Unity editor `stale_status / GridEditorUtility` 噪音，不是本轮 own red
- 当前 blocker：
  - Unity 当前 `ready_for_tools=false / stale_status`
  - 因此只能 claim 代码层 self-check 过，不能 claim live 已过

## 2026-04-11 实装补口｜InteractionHintOverlay 去重与老场景升级补件
- 本轮性质：
  - 延续当前 `ACTIVE` slice 继续真实施工
- 这轮确认的真实根因：
  1. `InteractionHintOverlay` 在 `Primary / Home / Town` 场景里本来就有序列化实例，但默认 inactive
  2. 旧版 `EnsureRuntime()` 只会无脑新建 runtime overlay，不会先认领已有实例
  3. 旧版 `EnsureBuilt()` 一旦发现缺字段，就会整棵 `BuildUi()` 重建；而场景里的旧 overlay 正好缺 `overlayScaler / interactionCardCanvasGroup / status*` 这批新字段，结果会在同一个实例里再长一套子树
  4. 放置模式切换与教学 guidance 会在同一帧连续打两次状态卡，造成“双状态叠着出”的体感
- 这轮实际落地：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `EnsureRuntime()` 改为优先认领现有实例，并退休重复 overlay
    - `BuildUi()` 改为“复用已有子树 + 只补缺件”，不再对老场景对象整棵重建
    - 对旧场景实例补 `CanvasScaler / interaction card canvas group / status card` 缺口
    - `状态` 标签改为独立角标，标题右移，避免继续和标签互相遮挡
    - 状态切换发生时，如果同帧已经有 guidance 文案，就直接消费当前 guidance，不再连发第二张状态卡
    - `EnsureBuilt()` 恢复快路径，避免在 `LateUpdate()` 每帧递归扫子节点制造性能炸弹
- 新增协作物：
  - [2026-04-11_UI线程_给farm_toolbar空选中与手持链闭环协作prompt_10.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/2026-04-11_UI线程_给farm_toolbar空选中与手持链闭环协作prompt_10.md)
- 代码层验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs --count 20 --output-limit 5`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
    - Unity 侧仍是 `stale_status` 外部阻断
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - 通过
- 当前恢复点：
  - 这条 own 刀口现在停在“代码层结构已收平，等待用户 live 看双状态 / 重叠 / 精致度”
  - 如果用户 live 仍能复现，再继续只收 `InteractionHintOverlay`
  - `Park-Slice` 已跑，当前 UI 线程状态为 `PARKED`

## 2026-04-11 UI 精修｜状态提示卡字号、间距与单行文案
- 本轮性质：
  - 用户按最新截图反馈要求，只做 `InteractionHintOverlay` 的纯 UI 精修
- 本轮实际调整：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 状态卡宽度从 `272` 提到 `312`
    - `状态` 标签字号、标签壳尺寸、标题字号、说明字号整体放大
    - 顶部标签与标题整体再下移一档
    - 标题行与说明行的垂直间距收紧
    - 开关态说明文案改成更短版本：
      - `农田/播种/浇水输入已开启，按 V 关闭。`
      - `农田/播种/浇水输入已关闭，按 V 开启。`
- 代码层验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs --count 20 --output-limit 5`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
    - Unity 侧仍是 `stale_status / 编译后 editor 状态未 fresh` 外部阻断
- 当前恢复点：
  - 这一刀现在只差用户看 runtime 观感

## 2026-04-11 UI 微调｜再缩行距、缩短竖条、拉开标签与标题
- 本轮性质：
  - 延续用户最新截图反馈，继续只做状态提示卡纯视觉微调
- 本轮实际调整：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 左侧竖条高度从 `30` 缩到 `24`
    - 标题起始 X 从 `78` 推到 `88`，拉开 `状态` 标签与标题的水平间距
    - 说明行整体上提，进一步缩小上下两行的垂直距离
- 代码层验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs --count 20 --output-limit 5`
    - `assessment=external_red`
    - `owned_errors=0`
    - 外部为运行中 Unity 现场里的 `Missing Script (Unknown)` 与 TMP 溢出 warning，不是本轮 own red
- 当前恢复点：
  - 继续等用户看 runtime 观感

## 2026-04-11 UI 微调｜状态底板缩短，左竖条回拉
- 用户纠正点：
  - 上一轮缩错对象了
  - 该缩的是 `状态` 底板，不是左竖条
- 本轮调整：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 左竖条高度回拉到 `28`
    - `状态` 底板宽度收窄到 `44`
    - `状态` 文本区域同步收窄
- 验证：
  - `validate_script` = `assessment=external_red`
  - `owned_errors=0`
  - 外部 blocker 为 `SpringDay1LateDayRuntimeTests.cs` 里的 `SpringDay1BedInteractable` 缺符号

## 2026-04-11 UI 微调｜按检视器截图直接抄值
- 用户这轮不给我再猜，直接给了检视器截图作为真值
- 本轮按图抄入：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `StatusAccentLine.height = 47.08`
    - `StatusTag.position = (29, -16)`
    - `StatusTag.size = (44, 18)`
    - `StatusTitleText = left 88 / right 18 / posY -12 / height 24`
    - `StatusDetailText = left 29.8 / right 10.2 / posY 12 / height 22`
- 验证：
  - `validate_script` = `assessment=external_red`
  - `owned_errors=0`
  - 当前 external 为 Unity 运行现场 `Missing Script (Unknown)` 噪音

## 2026-04-11｜farm 协作补口：toolbar 空选中死态继续追到 PackagePanel ghost state

- 触发来源：
  - UI 线程给出的 `toolbar 空选中与手持链闭环` 协作 prompt 后续继续落地。
- 本轮新确认：
  - 之前 selection 真值链那刀方向是对的，但仍剩一条 UI/runtime 交界尾差：
  - `BoxPanelUI.Close()` 可直接把箱子子物体关掉，却不一定同步关闭 `PackagePanelTabsUI.panelRoot`；
  - 于是会留下 `panelRoot.activeSelf=true`、但 `Main/Top/BoxUI` 都不可见的 ghost panel，进一步把 `GameInputManager.IsAnyPanelOpen()` 卡成真。
- 本轮已落地：
  - `PackagePanelTabsUI.IsPanelOpen()` 增加 ghost state 识别与自愈关闭；
  - `PackagePanelTabsUI.CloseBoxUI()` 增加 `_activeBoxUI == null` 的缺失态恢复；
  - `GameInputManager.CloseBoxPanelIfOpen()` 改为优先走 `tabs.CloseBoxUI(false)` 的完整关闭链。
- 当前判断：
  - 这次“toolbar 看着没选中、点击没反应”的主根，已经从“纯 selection 恢复语义”进一步压实到“面板残留硬闸门”。
  - UI 线程后续如果继续精修，只需要围绕视觉和交互表现，不需要再主刀这条 runtime 根因。

## 2026-04-11｜V0.5 checkpoint 已落地，包裹页关系/地图进入正式玩家面重构
- authoritative owner 结论：
  - `PromptOverlay / 任务清单` 已按 `spring-day1_回UI_任务清单治理裁定_33` 回到 `day1` 主刀
  - UI 本轮不再碰 [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
- 本地 checkpoint：
  - commit: `6ff90a48`
  - message: `checkpoint(ui): V0.5 HUD and overlay fixes`
  - 只包含 HUD / overlay own 收口，不吞 `toolbar/farm` 和 `PromptOverlay` 尾账
- 新 slice：
  - `Package关系页与地图页V0.5体验重构_2026-04-11`
- 当前施工范围：
  - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
  - [PackageMapOverviewPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs)
  - [PackagePanelRuntimeUiKit.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs)
  - [PackagePanelLayoutGuardsTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/PackagePanelLayoutGuardsTests.cs)
- 这轮已做：
  - `关系页`
    - 新增当前阶段 chip
    - 按 `presence/stage/phase relevance` 排序名册
    - 默认选中改为“当前阶段最相关的人”，不再把 `卡尔` 这种唯一有进度的人硬推成页面默认主角
    - 名册卡新增 `出场方式 + 关系阶段` 双 chip，预览改吃当前 beat 的真实印象
  - `地图页`
    - 主图区改成 zone blocks + route nodes + active halo
    - 去掉底部 legend 栏，降低“开发示意板”味道
    - 强化当前重点点位的可读关系
- 代码层验证：
  - `validate_script PackageNpcRelationshipPanel.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `validate_script PackageMapOverviewPanel.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `validate_script PackagePanelRuntimeUiKit.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `validate_script PackagePanelLayoutGuardsTests.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `errors --count 20` => `errors=0 warnings=0`
- blocker 报实：
  - 仍是 Unity `stale_status`，不是这轮 own compile red

## 2026-04-12 UI 新刀｜玩法态 / 背包态右侧操作提示面板
- 用户新增目标：
  - 在屏幕右侧靠边中部补一张简约的常驻操作提示卡。
  - 只分两组：
    - `玩法态`
    - `背包态`
  - `Backspace` 只关闭当前组提示，不做全局 toggle。
  - 左下角交互提示壳保持独立，不改。
- 本轮实现：
  - 继续沿用 [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs) 做承载，不去碰 [GameInputManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs) 共享输入热点。
  - 新增右侧 `ContextHintCard`：
    - 运行时自建壳体
    - 顶部 tag / 标题 / 一行上下文说明
    - 下面是键位 + 说明的轻量行列表
    - 底部固定一条 `退格关闭这组提示`
  - 当前上下文切换逻辑：
    - `PackagePanelTabsUI.IsPanelOpen()` => `背包态`
    - `Dialogue / Box / Workbench` => 直接隐藏右侧提示卡
    - 其他正常游玩态 => `玩法态`
  - 当前提示内容：
    - `玩法态`
      - `右键 点击地面导航`
      - `左键 使用当前手持` / 放置模式时变成 `放置 / 播种 / 浇水`
      - `E 交互`
      - `Tab 打开背包`
      - `V 开启/关闭放置模式`
      - `1-5 切换手持`
    - `背包态`
      - `左键 选中 / 拖拽物品`
      - `Shift+左键 二分拿取`
      - `Ctrl+左键 单个拿取`
      - `B/M/L/O 快速切页`
      - `Tab 回到物品页 / 收起`
      - `Esc 切到设置页`
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`
  - `owned_errors=0`
  - 当前 blocker 不是脚本 compile red，而是本机没有 active Unity instance，fresh live 证据还没补到
- 当前恢复点：
  - 下一步优先等玩家侧看右侧卡的真实大小、相对位置、遮挡和文案体感

## 2026-04-12 UI 窄刀｜toolbar 第五格错位根因与玩法态补 `Shift` 提示
- 当前主线：
  - 继续只收 UI own 的两个窄点：
    1. `toolbar 第五格内容不显示 / 只放 5 个物体时错位`
    2. 右侧 `ContextHintCard` 漏掉 `Shift 加速`
- 根因压实：
  - [ToolbarUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs) 的 `Build()` 之前是直接按 `gridParent` 子物体 hierarchy 顺序 `index++` 绑定 hotbar 索引。
  - 但 [ToolBar.prefab](D:/Unity/Unity_learning/Sunset/Assets/222_Prefabs/UI/0_Main/ToolBar.prefab) 以及 `Home / Primary / Town` 里的 `Bar_00_TG` 子物体顺序都是乱的，不是视觉上的 `0,1,2,3,4...`。
  - 所以这次更像 `toolbar 视觉槽位绑定错位`，不是“第五个物体 sprite 坏了”。
- 本轮改动：
  - [ToolbarUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs)
    - `Build()` 先收集 direct children，再按 `Bar_00_TG / Bar_00_TG (n)` 解析出的真实槽号排序后绑定。
    - 非 `Bar_00_TG` 子物体不会再抢占 `0` 号槽位，避免以后 prefab 多挂装饰节点又把索引带歪。
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `MaxContextRowCount` 提到 `7`
    - `玩法态` 文案补上：
      - `Shift 加速移动`
      - `1-5 / 滚轮 切换手持`
    - 顶部说明同步改成包含“加速”语义
- 代码层验证：
  - `validate_script ToolbarUI.cs` => `assessment=unity_validation_pending`, `owned_errors=0`, `external_errors=0`
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`, `external_errors=0`
  - 两次 `unity_validation_pending` 的 blocker 都是：当前没有 active Unity instance，不是本轮 own red
  - `git diff --check -- Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
- 当前判断：
  - 这轮已把 `toolbar 第五格` 的结构根因修到代码层，并把玩法态提示缺项补齐。
  - 还没有玩家侧 live 画面，因此不能 claim 体验已过线。
- 恢复点：
  - 优先等用户实测：
    1. 只放 5 个物体时，第 5 格是否恢复显示
    2. 放 6 个以上物体时，后续格位是否仍然错位
    3. 右侧玩法态提示里是否已经出现 `Shift 加速`

## 2026-04-12 UI 精修｜右侧提示卡去测试味
- 当前主线：
  - 用户已给出 live 画面，明确指出右侧 `ContextHintCard` 很丑；这轮只收这张卡的布局和气质，不扩回别的 UI。
- 真实问题归因：
  - 不是单纯“颜色不对”，而是结构关系错了：
    1. 壳体过大过重
    2. 键帽冲出面板左边
    3. 左侧长竖线把整块做成了“流程图/说明板”气质
    4. 键帽列和说明列没有稳定对齐
    5. 说明文案太像开发提示，不像正式玩家面
- 本轮改动：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 缩小 `ContextHintCard` 壳体宽度与最小高度
    - 缩短左侧 accent，改成只服务表头，不再整卡贯穿
    - 所有键帽改为完整收回卡内，固定成稳定左列
    - 行布局改成卡内两列：左边键帽列，右边说明列
    - 缩小外阴影和描边，让卡从“大黑板”回到“轻量提示”
    - 标题改成 `操作提示`
    - 顶部说明文案改成更像正式玩家面：
      - `导航、交互、加速与手持操作。`
      - `切页、拿取与拖拽操作。`
    - 页脚缩成 `退格关闭提示`
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`, `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
  - 当前 blocker 仍是没有 active Unity instance，不是本轮 own red
- 当前判断：
  - 这轮已经把“出框、流程图感、体量过大、测试味太重”从结构上改掉。
  - 但是否真正顺眼，还必须等用户看新的 live 画面；当前仍只站住 `targeted probe`。

## 2026-04-12 UI 精修｜ContextHintCard 第二刀减法
- 当前主线：
  - 用户继续要求“只修这张提示卡”，这轮继续只收 `ContextHintCard` 的精致度，不碰其他 UI。
- 用户最新 live 反馈映射出的真实问题：
  - 现在不是布局炸了，而是：
    1. 表头关系还不够精致
    2. 卡片还是略宽
    3. 长键位还是偏挤
    4. 页脚太弱
    5. 整体仍有一点“现代帮助卡”味道
- 本轮改动：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 再次缩小卡宽、最小高度、行高与行间距
    - 再缩短 accent，继续减弱“说明板”味道
    - 标签、标题、副标题、页脚字号统一再收一档
    - 键帽宽高继续缩小，色彩饱和度降低，减轻“按钮味”
    - 表头文案减法：
      - `操作提示 -> 常用操作`
      - `导航、交互、加速与手持操作。 -> 导航、交互、加速、手持。`
      - `切页、拿取与拖拽操作。 -> 切页、拿取、拖拽。`
    - 列表文案减法：
      - `导航`
      - `使用`
      - `加速`
      - `背包`
      - `切页`
      - `设置页`
      - `二分 / 单取`
    - 页脚改成更短的 `退格关闭`，并右对齐，让收尾更像设计过的卡片
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`, `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
  - 当前 blocker 仍是没有 active Unity instance，不是本轮 own red
- 当前判断：
  - 这轮把问题从“结构不对”继续压到“信息减法和精致度补口”。
  - 还差玩家侧 final live 判断，当前仍只站住 `targeted probe`。

## 2026-04-12 UI 续记｜右侧提示卡边界透明安全口径确认 + 正式剧情右上角隐藏
- 当前主线：
  - 继续只收 UI own 的 `ContextHintCard / 右上角时间与调试提示`，不扩回任务清单、工作台或其他 HUD 泛修。
- 用户最新要求：
  - 右侧提示卡也要像 `toolbar / 任务清单` 一样，在玩家靠近场景对应边界时自适应透明。
  - 但做法必须安全，不能误伤左下角交互提示、状态提示或别的 overlay。
- 这轮压实的关键结论：
  - 最安全的做法不是把整根 [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs) 挂进共享的 boundary focus 根层一起淡。
  - 因为 `InteractionHintOverlay` 根下面还同时带着左下角交互提示和放置模式状态提示；如果整根 fade，会把不该淡的内容一起淡掉。
  - 当前仓库里的正确实现已经改成：只给右侧 `contextCard` 自己做 alpha 合成，公式是 `显示请求 alpha × 边界 alpha`。
  - 边界阈值直接复用 [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs) 同语义常量：
    - `0.25 / 0.18 / 0.02 / 18f / 0.58`
  - `Package` 页打开时保持完全可见，避免 page UI 里还跟着淡；`Dialogue / Box / Workbench` 打开时则直接隐藏右侧卡。
- 这轮同步压实的另一条 UI own 结论：
  - 右上角 `时间 / 调试帮助` 的真源仍是 [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs) 的 `OnGUI()`。
  - 当前仓库里已经新增 `ShouldHideTopRightHud()`：
    - `DialogueManager.Instance.IsDialogueActive`
    - `SpringDay1Director.Instance.ShouldForceHideTaskListForCurrentStory()`
  - 命中任一时，右上角时钟和调试帮助都不再绘制。
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`, `external_errors=0`
  - `validate_script TimeManagerDebugger.cs` => `assessment=unity_validation_pending`, `owned_errors=0`, `external_errors=0`
  - 两次 `unity_validation_pending` 的 blocker 都是：当前没有 active Unity instance，不是本轮 own red
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs Assets/YYY_Scripts/TimeManagerDebugger.cs` => clean
- 当前判断：
  - 这轮已经把“怎么安全做右卡边界透明”这件事压成可复用结论，并确认代码链已经走在“只作用右卡本身”的正确方向上。
  - 但这轮没有 fresh live 截图，因此只能 claim `结构 / targeted probe`，不能 claim 体验已过线。
- 当前恢复点：
  - 下次如果用户继续验这块，优先只看 3 个点：
    1. 玩家贴右边界时，右卡是否平滑减淡
    2. 左下角交互提示 / 放置状态提示是否完全不受影响
    3. 正式剧情 / 正式对话开始时，右上角时间与调试帮助是否彻底退场
