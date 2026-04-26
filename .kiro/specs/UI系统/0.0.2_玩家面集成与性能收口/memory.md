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

## 2026-04-12 UI 续记｜右侧提示卡排版重对齐 + 背包开关恢复任务清单压住
- 当前主线：
  - 用户最新 live 反馈聚焦两件事：
    1. 右侧 `ContextHintCard` 的字、键帽、页脚提示还不够正式，且退格没有可见按键提示
    2. 打开背包时，左侧任务清单没有再像 toolbar 那样被 page 正常压住
- 这轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 把右侧卡整体再放大一档：
      - `ContextCardWidth 220 -> 238`
      - `ContextCardMinHeight 154 -> 176`
      - `ContextCardRowHeight 17 -> 20`
      - `ContextCardRowGap 3 -> 4`
    - 行内文字重新按中线关系收齐：
      - 说明文案从 `Left` 改成 `MidlineLeft`
      - 键帽、正文、页脚都重新按中心线布局
    - 所有关键字重再抬一档：
      - 标题 / 正文 / 键帽 / 页脚全部增大
      - 键帽和底板同步轻微放大，但仍约束在卡内
    - 页脚从纯文字改成真正的按键提示：
      - 新增 `Backspace` 键帽
      - 页脚动作文字改成 `关闭提示`
  - [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs)
    - 新增最小恢复口 `RefreshPromptOverlayVisibilityBlock()`
    - 背包打开、关闭，以及箱子模式拉起 PackagePanel 时，都会直接把当前运行中的 `PromptOverlay` 走一次 `SetExternalVisibilityBlock(...)`
    - 这刀不去碰那份正在大改中的 [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs) 脏现场，只走现成接口做恢复
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `validate_script PackagePanelTabsUI.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs` => clean
  - 当前外部只剩 Unity 现场里的 warning / stale_status，不是本轮 own red
- 当前判断：
  - 这轮已经把用户直接指出的 3 个表层问题真正落成了代码：
    1. 键帽和正文中线不平
    2. 字太小
    3. 退格没有键帽提示
  - 同时把背包打开时任务清单“不再被压住”的回归口补回来了。
  - 但是否完全顺眼，仍需用户 fresh live 看一次真实画面。
- 当前恢复点：
  - 下一步优先等用户看：
    1. 右卡每行键帽和正文是否终于平了
    2. `Backspace` 页脚键帽是否清楚
    3. 打开背包或箱子时，任务清单是否恢复退场/被压住

## 2026-04-13 UI 续记｜右卡中心线再收口 + 退格改成图形键
- 当前主线：
  - 用户 fresh 截图继续指出右侧 `ContextHintCard` 还没过线：
    1. 行内键帽和说明文字仍然没真正平到同一中心线
    2. 页脚 `Backspace` 还是英文单词，不是退格图形键
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 行内 description 不再使用左右拉伸 offset 方式，而是改成与键帽同样的固定中心线布局：
      - `anchorMin/anchorMax = (0, 0.5)`
      - `pivot = (0, 0.5)`
      - `anchoredPosition = (keyColumnWidth + 10, -rowHeight * 0.5)`
    - 这样每一行的键帽中心和说明文字中心都落在同一条 Y 线上
    - 页脚键帽从英文文字键改成真正的图形键：
      - 新增 runtime `ContextFooterKeyIcon`
      - 新增 `GetOrCreateBackspaceIconSprite()` 生成退格图形 sprite
      - 原 `ContextFooterKeyText` 保留兼容，但运行时直接隐藏
      - 页脚现在显示为：退格图形键 + `关闭提示`
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`, `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
  - 当前 blocker 仍只是 Unity 现场的 `stale_status`，不是本轮 own red
- 当前判断：
  - 这轮已经把用户刚点名的两个问题都落实到代码：
    1. 行内中心线重新统一
    2. 页脚从英文单词键改成图形退格键
  - 但视觉是否终于顺眼，仍需用户 fresh live 终验
- 当前恢复点：
  - 下一步只看右侧卡：
    1. 每行键帽和说明文字是不是终于平了
    2. 页脚退格键图形是不是更像真正的退格键

## 2026-04-13 UI 续记｜退格键重画
- 当前主线：
  - 用户最新裁定很清楚：其他都过了，只剩右侧卡页脚的退格键“太丑”，要求重做。
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 把页脚退格键从上一轮那种不自然的块状箭头，重画成更接近真实退格语义的图形键：
      - 左侧箭头轮廓
      - 右侧主体
      - 中间删除 `X` 语义
    - 页脚键帽宽度从 `30 -> 34`
    - icon 可见尺寸从 `16x10 -> 18x12`
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`, `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
  - 当前 blocker 仍只是 Unity 现场 `stale_status`，不是本轮 own red
- 当前判断：
  - 这轮不是结构变更，而是纯视觉重画；代码层已经收住。
  - 是否顺眼，只差用户 fresh live 终验。

## 2026-04-13 UI 续记｜顶部改成单行结构并再缩一点
- 当前主线：
  - 用户最新截图指出两个微调点：
    1. 顶部 `玩法` 标签不好调整
    2. 希望整体再小一点，并且第一行像“放置模式提示”那样单行显示
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 整卡再缩一档：
      - `ContextCardWidth 228`
      - `ContextCardMinHeight 166`
      - `ContextCardRowGap 3`
    - 顶部结构改成单行：
      - `玩法` 标签与 `常用操作` 标题落到同一排
      - 标题不再单独占第二行
    - 这样顶部语义和放置状态提示更一致，也更容易继续人工微调
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`, `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
  - 当前 blocker 仍只是 Unity 现场 `stale_status`，不是本轮 own red
- 当前判断：
  - 这轮属于纯布局微调，代码层已收住
  - 还差用户 live 看顶部这一行是否终于顺手

## 2026-04-13 UI 续记｜顶部头部中心线与底部空腔再收口
- 当前主线：
  - 用户继续指出右侧卡“位置不对、细节一堆问题”，这轮继续只收几何关系，不扩功能。
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 整卡再收一档：
      - `ContextCardWidth 220`
      - `ContextCardMinHeight 156`
      - `ContextCardBottomInset 8`
    - 头部三件套重新压到同一条中心线上：
      - 左侧竖线
      - `玩法` 标签
      - `常用操作` 标题
    - 内容整体继续上提，底部 footer 区与尾部空腔同步压缩
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`, `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
  - 当前 blocker 仍只是 Unity 现场 `stale_status`，不是本轮 own red
- 当前判断：
  - 这轮是纯位置与比例重排，代码层已收住
  - 还差用户 fresh live 看“头部一行”和“整块留白”是否终于顺

## 2026-04-13 UI 续记｜退格符号标准化
- 当前主线：
  - 用户继续明确指出“这个回退键毫无意义”，所以这轮只收退格符号本体，不再泛调别处。
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 把页脚图标从块状怪形改成更标准的退格轮廓符号：
      - 左指向箭头
      - 右侧主体
      - 中间删除 `X`
    - 这是在独立 keycap 背景内绘制符号，不再把“键帽轮廓”和“退格符号”混成一个怪图
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=external_red`, `owned_errors=0`, `external_errors=4`
  - 这 4 条 `external_errors` 都是 Unity 现场现存的 `The referenced script (Unknown) on this Behaviour is missing!`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
- 当前判断：
  - 这轮 own 代码没红，外部红来自当前场景已有 missing script，不是本轮引入
  - 现在只差用户 fresh live 看这个退格符号是否终于像正常回退键

## 2026-04-13 UI 续记｜边界透明 / 箱子提示 / 关系页空框 / 工作台详情保留
- 当前主线：
  - 用户要求按最小、最安全、最可回退的方案，集中检查 4 个点：
    - 任务清单重新接回边界透明
    - 右侧提示卡在箱子页也要显示，且 footer 细节继续收
    - 关系页左上无头像时的大空框缩小
    - 工作台 A 配方完成时，不准把当前 B 配方的数量选择和详情上下文刷新掉
- 本轮完成：
  - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
    - `SpringDay1PromptOverlay` 重新接回 `Left | Top` 边界透明
    - HUD 边界透明最低值改成 `40%`
    - 渐隐曲线改成 `SmoothStep`，去掉原来的硬切到底
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 右侧提示卡最低透明度改成 `40%`，同样改成渐进曲线
    - 箱子打开时不再隐藏右侧提示，新增 `箱子` 组提示
    - footer 键帽跟随背包/箱子冷色语义，不再只有正文变蓝
    - footer 键帽与“关闭提示”改成同一水平中心线
    - 退格图标放大并重画成更明确的 backspace 轮廓
  - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
    - 详情页左上头像框从大块空框改成更小尺寸
    - 无头像时显示“暂无画像”，并把框体压缩到更紧的占位
  - [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
    - 新增“只在完成的正是当前选中配方时才清零数量”的保护
    - A 配方完成后，如果当前看的是 B，就不再重建 B 的详情列，不再把 B 的数量选择刷掉
- 验证：
  - `manage_script validate InteractionHintOverlay` => `errors=0`, `warnings=1`
  - `manage_script validate PackageNpcRelationshipPanel` => `errors=0`, `warnings=0`
  - `manage_script validate SpringDay1WorkbenchCraftingOverlay` => `errors=0`, `warnings=1`
  - `manage_script validate PersistentPlayerSceneBridge` => `errors=0`, `warnings=2`
  - `errors --count 20 --output-limit 5` => `errors=0`, `warnings=0`
  - `validate_script PersistentPlayerSceneBridge` => `assessment=unity_validation_pending`, `owned_errors=0`, 当前卡在 Unity `stale_status`
- 当前阶段：
  - 代码侧这 4 个点已经按最小刀口落完，下一步是用户集中 live 检查体验面
- 下一步只做什么：
  - 等用户集中验证这 4 个点，再决定是否继续只收局部细节

## 2026-04-13 UI 续记｜二次纠偏：箱子 block / 退格符号 / 关系页大方块
- 当前主线：
  - 用户 fresh live 后继续指出 3 个问题：
    - 箱子页打开时任务清单仍压在上面
    - 右卡 footer 图标像怪盒子，颜色也没统一
    - 关系页左侧的大阶段方块根本没被收掉
- 本轮完成：
  - [BoxPanelUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs)
    - 箱子 `Open/Close` 正式接入 `SpringDay1PromptOverlay.SetExternalVisibilityBlock(...)`
    - 关闭箱子时不再盲目清 block，而是和 `PackagePanelTabsUI.IsPanelOpen()` 一起算，避免箱子/背包互相打架
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 把 footer 图标从“框中框怪图”改成更小的纯退格箭头符号
    - 同时缩小 footer keycap 尺寸，避免再挤成一大块
  - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
    - 左侧 `今日名册` 头部从布局组改成固定锚点布局
    - 阶段芯片改成固定小尺寸，不再让它失控长成大方块
- 验证：
  - `manage_script validate BoxPanelUI` => `errors=0`, `warnings=0`
  - `manage_script validate InteractionHintOverlay` => `errors=0`, `warnings=1`
  - `manage_script validate PackageNpcRelationshipPanel` => `errors=0`, `warnings=0`
  - fresh `errors` => `errors=0`, `warnings=0`
- 当前阶段：
  - 这轮二次纠偏已经落完，等待用户看这 3 处最新 live 结果

## 2026-04-13 UI 续记｜三次纠偏：箱子/背包双链统一 block
- 当前主线：
  - 用户继续要求“进行”，所以这轮只补最后一个更稳的口，不再扩别的 UI：
    - 箱子页和背包页都要稳定压住任务清单，不能互相把 block 顶掉
- 本轮完成：
  - [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs)
    - `RefreshPromptOverlayVisibilityBlock()` 改成 `panelRoot.activeSelf || IsBoxUIOpen()`
  - [BoxPanelUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs)
    - `RefreshPromptOverlayVisibilityBlock()` 改成 `_isOpen || (packageTabs.IsPanelOpen() || packageTabs.IsBoxUIOpen())`
    - 这样箱子态和背包态会共用同一套 block 真值，不再互相清掉
- 验证：
  - `manage_script validate PackagePanelTabsUI` => `errors=0`, `warnings=0`
  - `manage_script validate BoxPanelUI` => `errors=0`, `warnings=0`
- 当前阶段：
  - 这轮 UI 代码已收住，等待用户 fresh live 检查箱子页/背包页任务清单遮挡是否 finally 稳定

## 2026-04-13 UI 续记｜Day1 PromptOverlay contract 收口第一刀
- 当前主线：
  - 用户已把 Day1 `任务清单 / Prompt / bridge prompt / 对话显隐 / modal 层级 / re-entry 玩家可见面` 全权交给 UI，本轮按最小安全方案先收真正的玩家面 blocker，不越权改 day1 runtime staging。
- 本轮完成：
  - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - 新增 `CurrentRuntimeInstanceOrNull / SetGlobalExternalVisibilityBlock`，让外部 modal block 统一命中真正可复用的 runtime 单例，不再靠 `FindFirstObjectByType` 碰运气
    - `EnsureAttachedToPreferredParent()` 现在会主动把任务清单插回 HUD 正确兄弟位，停在 `Package / Dialogue / Workbench / Settings` 这类 modal sibling 前面
    - 正式对话开始/结束改成直接吃 `DialogueStart/End` 事件真值，不再继续等待别的链“顺手帮我隐藏”，避免对话期残留
  - [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs)
    - prompt block 改成统一走 `SpringDay1PromptOverlay.SetGlobalExternalVisibilityBlock(...)`
  - [BoxPanelUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs)
    - 箱子链也统一走同一个 PromptOverlay block 入口，避免 stale duplicate/错误实例截胡
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - footer 退格键重画成正常 backspace 轮廓，底部键帽和文案比例一起收平
  - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
    - 无头像时左上占位进一步缩成更紧的小框，去掉大空腔感
  - [PackagePanelLayoutGuardsTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/PackagePanelLayoutGuardsTests.cs)
    - 新增护栏：`PromptOverlay` 必须回到 HUD lane 且停在 `PackagePanel` 前
    - 新增护栏：即使静态字段临时指到 stale duplicate，PackagePanel 仍要压住真正 runtime 单例
- 验证：
  - `manage_script validate SpringDay1PromptOverlay` => `errors=0`, `warnings=2`
  - `manage_script validate PackagePanelTabsUI` => `errors=0`, `warnings=0`
  - `manage_script validate BoxPanelUI` => `errors=0`, `warnings=0`
  - `manage_script validate InteractionHintOverlay` => `errors=0`, `warnings=1`
  - `manage_script validate PackageNpcRelationshipPanel` => `errors=0`, `warnings=0`
  - `manage_script validate PackagePanelLayoutGuardsTests` => `errors=0`, `warnings=0`
  - `errors --count 20 --output-limit 10` => `errors=0`, `warnings=0`
  - `PackagePanelLayoutGuardsTests` => `5/5 passed`
  - `SpringDay1LateDayRuntimeTests.PromptOverlay_ShouldHideDuringDialogueAndRecoverAfterwards` => `passed`
  - `SpringDay1LateDayRuntimeTests.PromptOverlay_UsesParentCanvasGovernance_WhenUiRootCanvasExists` => `passed`
  - `SpringDay1LateDayRuntimeTests.PromptOverlay_ShouldPreferBaseCanvasUnderUiRoot_InsteadOfModalPackageCanvas` => `passed`
- 当前阶段：
  - 结构 / targeted probe 已站住；用户 live 与 packaged 终验仍待继续
- 下一步只做什么：
  - 只等用户继续测 `任务清单在背包/箱子/对话里的玩家面表现` 与 `右卡 footer / 关系页占位`，再决定是否继续只收细节

## 2026-04-13 UI 续记｜PromptOverlay alpha owner 收口 + 关系页 chip 栏纠偏
- 当前主线：
  - 继续按 day1 contract 收 `任务清单 / PromptOverlay / bridge prompt / 对话显隐 / modal 层级`；用户本轮明确纠偏“卡尔的问题是列表右侧小筹码栏，不是头像壳”。
- 本轮完成：
  - `SpringDay1PromptOverlay.cs`
    - 新增 `_visibilityAlpha + _boundaryFocusAlpha` 合成口，开始把 PromptOverlay 收成单一 alpha owner
    - `LateUpdate` 的显隐判断改看 `_visibilityAlpha`，避免边界透明误导“已显示/未显示”判断
  - `PersistentPlayerSceneBridge.cs`
    - 命中 `SpringDay1PromptOverlay` 时，不再直写 `CanvasGroup.alpha`，改走 `SetBoundaryFocusAlpha(...)`
  - `DialogueUI.cs`
    - `ShouldManageAsNonDialogueUi(...)` 跳过 `SpringDay1PromptOverlay`，避免正式对话和 Prompt 自己的事件显隐双写 alpha
  - `PackageNpcRelationshipPanel.cs`
    - 列表右侧 chip 栏统一为固定栏宽、固定 chip 宽高、禁止换行、顶部对齐
    - `未露面` 这类长标签不再把卡尔这一列撑坏
  - `InteractionHintOverlay.cs`
    - footer backspace icon 又收一刀，改成更正常的 backspace 轮廓并略放大键帽
  - `PackagePanelLayoutGuardsTests.cs`
    - 新增两条护栏：`DialogueUI` 不准再把 PromptOverlay 当普通 sibling 淡出；boundary alpha 不准反向把隐藏中的 PromptOverlay 抬出来
- 验证：
  - `manage_script validate SpringDay1PromptOverlay` => `errors=0`, `warnings=2`
  - `manage_script validate PersistentPlayerSceneBridge` => `errors=0`, `warnings=2`
  - `manage_script validate DialogueUI` => `errors=0`, `warnings=1`
  - `manage_script validate PackageNpcRelationshipPanel` => `errors=0`, `warnings=0`
  - `manage_script validate InteractionHintOverlay` => `errors=0`, `warnings=1`
  - `manage_script validate PackagePanelLayoutGuardsTests` => `errors=0`, `warnings=0`
  - fresh `errors --count 20 --output-limit 10` => `errors=0`, `warnings=0`
- 当前恢复点：
  - 等用户继续 fresh live 看 `PromptOverlay` 在背包/边界/对话里的闪烁是否 finally 被打平
  - 同时看卡尔这一列 chip 栏是否 finally 统一

## 2026-04-13 UI 续记｜BridgePromptRoot 按手调真值回位
- 当前主线：
  - 用户把本轮切成窄刀，只收 `BridgePromptRoot` 的运行时布局，要求直接对齐他手动摆放出来的正式面。
- 本轮完成：
  - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - 新增 bridge prompt 专用常量：
      - `BridgePromptOffsetX = 13f`
      - `BridgePromptOffsetY = 368f`
      - `BridgePromptWidth = 328f`
      - `BridgePromptHeight = 34f`
    - `BuildBridgePromptShell()` 不再复用任务卡左边距，而是直接按 bridge prompt 自己的真值创建 `BridgePromptRoot`
    - 锚点 / pivot 继续保持左下锚与 `(0, 0.5)`，只收这条 root 的位置和尺寸
- 验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs --count 20 --output-limit 5`
    - `assessment=external_red`
    - `owned_errors=0`
    - 外部 blocker 是用户当前运行态现场里的 missing-script / stale_status，不是本轮引入
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level standard --output-limit 5`
    - `errors=0`
    - `warnings=2`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
    - 仅现有 CRLF/LF 提示，无新 diff 格式错误
- 当前恢复点：
  - 等用户直接看 `BridgePromptRoot` 是否已经回到他手调那套布局
  - 这轮仍只站住 `结构 / checkpoint`，未 claim live 体验终验

## 2026-04-13 UI 续记｜箱子提示卡配色统一 + 关闭提示键帽微调
- 当前主线：
  - 用户指出箱子场景下右侧提示卡“上半段和下半段不是一个颜色语义”，并补图要求把 footer 的关闭提示键帽再收精一点。
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 只读核实后确认：这次“箱子 UI 颜色怪”主要落在 `Chest` 场景的右侧提示卡，不是 `BoxPanelUI` 主脚本在动态上色
    - `ContextHintGroup.Chest` 现在改成完整统一的暖色 palette：顶部 accent/tag、行内 keycap、footer keycap、icon 和 footer 文案都走同一套暖色系，不再出现头脚像两张卡
    - footer 关闭提示键帽缩短到更紧的宽高，icon 放大一点并微微下压，减少“空心糖块”感
- 验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name InteractionHintOverlay --path Assets/YYY_Scripts/Story/UI --level standard --output-limit 5`
    - `errors=0`
    - `warnings=1`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs --count 20 --output-limit 5`
    - `assessment=external_red`
    - `owned_errors=0`
    - 外部 blocker 为 `ItemTooltip.cs:742` 的运行态 tooltip inactive coroutine，与本轮提示卡补丁无关
- 当前恢复点：
  - 等用户直接看箱子场景下的右侧提示卡是否终于统一、不再怪
  - 这轮仍只站住 `结构 / checkpoint`，未 claim live 体验终验

## 2026-04-13 UI 续记｜Tooltip 正式壳微调 + 跟鼠标越界收口
- 当前主线：
  - 用户接了 farm 的 tooltip 视觉委托后，又明确追加：只做微调，不改现有功能，只收字体、边框、正式感，以及 tooltip 跟鼠标时的越界和贴鼠标体验。
- 本轮完成：
  - [ItemTooltip.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs)
    - 只改 tooltip 自己，没有去碰 `ToolbarSlotUI` / `InventorySlotInteraction` 的 show/hide / hover 抑制逻辑
    - 壳体从更小更挤的测试态收成更正式的纸卡感：
      - 尺寸从 `184x74` 提到 `212x88`
      - 边框从 `3` 提到 `4`
      - 内容宽度、上下左右 padding、行间距、header padding 都一起放松
      - 标题 / 状态 / 描述 / 价格字号整体上调
      - 根框补了轻微 `Outline`，内板补了轻微 `Shadow`
    - 跟鼠标位置做了最小安全收口：
      - 只有当 `_movementRect` 真能容纳 tooltip 时，才继续在那个局部 bounds 内活动
      - 像 toolbar 这种窄条区域，不再被错误限制在太小的局部 bounds 里
      - 右侧 / 左侧 / 上下越界时都会优先翻边，再做带边距的最终 clamp
- 验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name ItemTooltip --path Assets/YYY_Scripts/UI/Inventory --level standard --output-limit 5`
    - `errors=0`
    - `warnings=1`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs --count 20 --output-limit 5`
    - `assessment=external_red`
    - `owned_errors=0`
    - 外部 blocker 为当前用户运行态现场里的 missing-script / stale_status，不是本轮 tooltip 改动引入
  - `git diff --check -- Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs`
    - clean
- 当前恢复点：
  - 等用户直接看 toolbar hover 和背包 hover 两处的 tooltip
  - 这轮仍只站住 `结构 / checkpoint`，未 claim live 体验终验

## 2026-04-13 UI 续记｜右侧提示卡黄系统一 + BACKSPACE 键帽 + 状态卡缩小 + 右上角调试提示恢复
- 当前主线：
  - 用户把本轮重新钉成两件事一起收：`右侧提示卡继续微调`，同时把 `右上角调试提示` 恢复出来，不能再被我关掉。
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 包裹页右侧提示卡不再走蓝色 accent，`Gameplay / Package / Chest` 三组统一回暖黄语义
    - footer 不再用退格图标，改成英文键帽 `BACKSPACE`
    - 放置模式状态卡整体收小：宽高、标题区、细节区、tag 和阴影都缩了一刀，并把默认状态文案缩短到单行优先
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - 右上角调试提示不再受 `ShouldHideTopRightHud()` 拦截
    - `EnsureAttached(...)` 默认 `showDebugInfoByDefault=true`
  - [PersistentManagers.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/PersistentManagers.cs)
    - 常驻管理器重新把 `TimeManagerDebugger` 以 `showDebugInfoByDefault: true` 接回 runtime
- 验证：
  - `manage_script validate InteractionHintOverlay` => `errors=0`, `warnings=1`
  - `manage_script validate TimeManagerDebugger` => `errors=0`, `warnings=1`
  - `manage_script validate PersistentManagers` => `errors=0`, `warnings=0`
  - `git diff --check -- Assets/YYY_Scripts/TimeManagerDebugger.cs Assets/YYY_Scripts/Service/PersistentManagers.cs Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - 只有既有 CRLF/LF 提示，无新 diff 格式错误
  - `validate_script` 当前没法拿到干净 compile 票：
    - `InteractionHintOverlay.cs` / `TimeManagerDebugger.cs` => `assessment=blocked`，`CodexCodeGuard returned no JSON`
    - `PersistentManagers.cs` => `assessment=external_red`，`owned_errors=0`
    - fresh console 仍是外部现场红：`DialogueUI.cs:1975 IndexOutOfRangeException` 和一组 missing script，不是这轮 touched 文件引入
- 当前恢复点：
  - 等用户 live 看 4 件事：
    1. 右上角调试提示是否已恢复常驻
    2. 背包右侧提示卡是否已从蓝色统一到黄系
    3. footer 的 `BACKSPACE` 键帽是否终于顺眼
    4. 放置模式状态卡是否已更接近交互提示卡体量

## 2026-04-14 UI 只读续记｜tooltip / 右卡 / 右上角时间 UI 三线拆分
- 当前主线：
  - 用户要求这轮不要再直接改，而是先把 6 张图和 farm 补充 contract 一起吃透，明确后续施工边界。
- 本轮拆分结果：
  - `tooltip 线`
    - 命名不成熟：直接显示 `Axe_0 / Hoe_0 / WateringCan`
    - 正文可读性仍偏弱：字号、对比度、两行截断一起叠加
    - 正确修法：复用工作台成熟命名映射 + 再收字体/对比度/行数预算
  - `右侧提示卡线`
    - 视觉问题：footer 键帽和说明语义顺序不对、太挤
    - 语义问题：还没完全对齐 farm 新 contract 的 world/package/chest 差异
    - 正确修法：交换 footer 文案与键帽位置，并补齐 package/chest 的差异提示
  - `右上角时间/调试线`
    - 视觉问题：目前只是裸文本，没有正式 HUD 壳
    - 核心事实：分钟精度受 `TimeManager` 真源限制，想改成 1 分钟粒度必须单列 runtime 改造，不是 UI 小修

## 2026-04-14 UI 续记｜正式玩家面三刀并收
- 当前主线：
  - 继续 `0.0.2` 的玩家面集成与性能收口，不漂回 toolbar / prompt / workbench 泛修补，只收 tooltip、右侧提示卡、右上角时间 HUD。
- 本轮完成：
  - tooltip：
    - [ItemTooltipTextBuilder.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/ItemTooltipTextBuilder.cs) 增加玩家向命名 helper，消灭 `Axe_0 / Hoe_0 / WateringCan` 这类内部名直出
    - [ItemTooltip.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs) 标题接 helper，纸卡感、字号、留白继续收正式
  - 右侧提示卡：
    - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs) 将 `Gameplay / Package / Chest` 文案按 farm contract 对齐
    - footer 收成 `关闭提示 + BACKSPACE`
    - 边界透明改为渐进，下限 40%
  - 右上角时间 HUD：
    - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs) 把右上角裸字改成正式 HUD 卡片
    - 快捷键 keycap 自适应，`+/-` 变成保留分钟的小时微调
  - 分钟粒度：
    - [TimeManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/TimeManager.cs) 真源改成逐分钟流逝
- 验证：
  - `validate_script` 覆盖 touched 文件时 `owned_errors=0`
  - 当前 Unity assessment 仍为 `unity_validation_pending`
  - fresh console 里仍有 external missing-script，不属于这轮 own UI 修改
- 当前阶段：
  - 结构已落；体验待用户 live 终验

## 2026-04-14 UI 只读续记｜`+/-` 快进语义回归分析
- 当前主线：
  - 用户要求先彻查 `+` 快进时间的分钟语义，不动代码。
- 结论：
  - 当前 `+/-` 被我改成了“保留分钟”的真实逻辑，不是单纯显示问题
  - 这会把调试整点跳转语义带偏，并且在 `26` 点边界后通过 `Sleep()+补分钟` 形成第二天 `06:xx`
  - `Day1 guardrail` 也会继承这个 minute，不会帮忙归零
  - 用户提出的 `:59 -> 自然 +1 分钟` 抓住了边界问题，但若直接用现有 `SetTime(:59)` 会多发一次 `:59` 事件；若要走这条路，必须用不广播中间态的内部 helper

## 2026-04-14 UI 只读续记｜超过 2 点睡觉逻辑 owner 归位
- 当前主线：
  - 进一步确认 Day1 第三点的 owner，不再让 UI 线程越权。
- 结论：
  - 全局 `> 02:00 -> Sleep()` contract 属于 `TimeManager`
  - Day1 的“稳定回家睡觉 / 收束到 DayEnd / 回住处 / snap actors home anchors / 非法早睡拦截 / 时间锁”属于 `SpringDay1Director`
  - 因此第三点整体应由 `spring-day1` 主刀；UI 只保留 `TimeManagerDebugger` 的调试整点跳转语义修复

## 2026-04-14 UI 续记｜状态说明文件已交给转发层
- 当前主线：
  - 将上面的 owner 判断收成一份可直接转发给 `spring-day1` 的状态说明，不再只是聊天口头结论。
- 本轮完成：
  - 新增：
    - [2026-04-14_UI线程_给spring-day1_时间owner边界与自收内容告知_11.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/2026-04-14_UI线程_给spring-day1_时间owner边界与自收内容告知_11.md)
  - 这份文件明确冻结了三点：
    1. 第三点整体由 `spring-day1` 主刀
    2. UI 不再继续碰 `SpringDay1Director` 的睡觉/DayEnd 逻辑
    3. UI 只自收 `TimeManagerDebugger +/-`

## 2026-04-14 UI 补记｜day1 线程已回发最终 UI owner prompt - 12
- 新增协作 prompt：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.2_玩家面集成与性能收口\2026-04-14_UI线程_Day1最终UI语义与时间owner边界收口prompt_12.md`
- 这份 prompt 进一步冻结：
  1. UI 全量 own Day1 玩家可见任务清单 / bridge prompt / Prompt / modal 层级 / re-entry UI 重建
  2. 新增提示与任务清单同根、放在任务清单下方、避免和 footer / 主任务文本重复
  3. `TimeManagerDebugger +/-` 由 UI 自收回整点跳转语义
  4. UI 不再越权碰 Day1 `HandleSleep / DayEnd / anchor / resident release`

## 2026-04-14 UI 续记｜prompt_12 第一刀已落地
- 当前主线：
  - 按 `prompt_12` 只收 Day1 玩家可见 UI contract 与 `TimeManagerDebugger +/-`，不越权碰 Day1 真逻辑。
- 本轮完成：
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - `AdvanceOneHour()` / `RewindOneHour()` 已恢复整点跳转
    - 普通小时跳转落 `xx:00`
    - 跨 `26` 直接走 `Sleep()` 真链，不再补分钟
  - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - `BridgePromptRoot` 从 overlay 顶层漂浮壳改为挂在 `TaskCardRoot` 下方
    - 复用绑定时会校验 parent 是否正确，不再接受旧的漂浮壳
    - bridge prompt 现在随任务卡壳一起布局、一起重建，并补进 live binding 校验
  - [SpringDay1UiLayerUtility.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs)
    - prompt modal block 新增 workbench 退让判定，收平 package / box / workbench 语义
  - 新增状态说明：
    - [2026-04-14_UI线程_给spring-day1_Day1玩家面UI收口状态告知_13.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/2026-04-14_UI线程_给spring-day1_Day1玩家面UI收口状态告知_13.md)
- 验证：
  - `manage_script validate`：
    - `SpringDay1UiLayerUtility` = clean
    - `SpringDay1PromptOverlay` = warning only（既有 `GameObject.Find in Update` / 字符串拼接提示）
    - `TimeManagerDebugger` = warning only（既有字符串拼接提示）
  - `validate_script` 覆盖 3 个 touched 文件时 `owned_errors=0 / external_errors=0`
  - 当前 assessment 仍为 `unity_validation_pending`
  - 原因是 Unity 现场卡在 `playmode_transition / stale_status`
- 当前阶段：
  - 结构与代码层已收住；玩家可见 live 体验仍待用户手测
- 明确未碰：
  - `SpringDay1Director.HandleSleep()`
  - `RecoverFromInvalidEarlySleep()`
  - `CanFinalizeDayEndFromCurrentState()`
  - Day1 `20:00 / 21:00 / 26:00` 夜间状态机
  - resident / actor anchor / staging / release

## 2026-04-14 UI 补记｜workbench 打开即关闭回归已定位并回退
- 用户阻塞反馈：
  - 工作台打开后 UI 闪一下立刻关闭。
- 根因：
  - 我刚把 workbench 塞进了 [SpringDay1UiLayerUtility.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs) 的 `IsBlockingPageUiOpen()`
  - 但 [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs) 自己的 `LateUpdate()` 也会读这条判定
  - 结果变成“工作台一打开就把自己当成外部 modal，然后立即 Hide()”
- 修复：
  - `IsBlockingPageUiOpen()` 回退为只认 `package / box`
  - `ShouldHidePromptOverlayForParentModalUi()` 单独补认 workbench
  - 这样 PromptOverlay 仍会对 workbench 退让，但 workbench 不会自杀式关闭
- 验证：
  - `manage_script validate SpringDay1UiLayerUtility` = clean

## 2026-04-14 UI 续记｜右上角调试卡 / 右侧提示卡 / bridge prompt 对齐微调
- 当前主线：
  - 用户要求这一轮只收 5 个可见面细节，并补一段可直接转发给 `day1` 的引导壳。
- 本轮完成：
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - 右上角调试卡加宽、加高
    - 时间与日期状态改为居中，收掉右侧空洞
    - 快捷键卡行高和文字容器加大，解决“下一天 / 切换季节 / 倍速 / 暂停 / 调整时间”底部被切
  - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - `BridgePromptOffsetX` 改为 `0`
    - 下方提示条左边缘重新对齐任务卡左边缘，不再往外冒
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 右侧常用操作卡 footer 改成“低强调关闭区”：`BACKSPACE` 改深底浅字、补 outline，与操作键帽拉开语义
    - footer 间距与尺寸重新分配，不再像普通操作行
    - 左下交互卡整体轻缩：`DetailCard / CompactCard` 尺寸下调，箱子提示会更紧凑
- 验证：
  - `manage_script validate TimeManagerDebugger` = warning only
  - `manage_script validate SpringDay1PromptOverlay` = warning only
  - `manage_script validate InteractionHintOverlay` = warning only
  - `git diff --check` 覆盖 3 个 touched 文件 = clean（仅 CRLF/LF 提示）
- 当前阶段：
  - 这轮是纯可见面微调，代码层已落；等用户 live 看图再决定是否还要第二轮精修

## 2026-04-14 UI 补记｜右侧常用操作卡 footer 与 tag pill 二次微调
- 用户反馈：
  - `关闭提示` 与 `BACKSPACE` 还不够拉开
  - 整体间距还可以再大一丢丢
  - `背包 / 玩法` 的 tag pill 需要更窄一点、字更大一点
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `ContextCardRowGap` 从 `3` 调到 `4`
    - footer 顶部间距与 `footerGap` 继续拉开
    - tag pill 从 `40` 缩到 `36` 宽
    - tag 文字从 `9` 提到 `9.75`
    - 标题起点略向左收，整体更紧致
- 验证：
  - `manage_script validate InteractionHintOverlay` = `errors=0 warnings=1`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` = clean

## 2026-04-14 UI 只读续记｜任务清单完成态与下一任务切换错位
- 当前主线：
  - 用户要求只读查清“任务完成后，应该先完整显示完成态，再切到下一条未完成任务；完成框和任务内容必须属于同一个状态”。
- 本轮核实结论：
  - 主因在 [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs) 的 UI 状态机时序，不是先去改 `spring-day1` 真逻辑。
  - `LateUpdate()` 每帧先用 `BuildCurrentViewState()` 直接生成最新 `_pendingState`，任务一完成，目标态就立刻变成“下一条未完成任务”。
  - `TransitionToPendingState()` 虽然会播 `AnimateRowCompletion(...)`，但这只作用在旧 row 局部；随后整张卡仍会按新的 `targetState` 重写标题、focus、footer 和 rows。
  - `PromptCardViewState.FromModel(...)` 当前又固定优先取“第一个未完成项”做 primary，所以正文天然会跳到下一条。
  - `SpringDay1Director.BuildPromptCardModel()` 现在没有提供“刚完成这一帧先保留完成态整卡快照”的过渡层。
- 我现在的判断：
  - 用户体感是对的：当前实现会出现“完成框还在表现旧任务，但任务正文已经换成下一条”的错位。
  - 这条问题应先由 UI own 收在 `PromptOverlay`，而不是直接改 `Director` 真逻辑。
- 最稳下一刀：
  - 只改 `SpringDay1PromptOverlay.cs`
  - 在 `TransitionToPendingState()` 引入旧任务的 completed snapshot
  - 先完整显示旧任务完成态整卡，再切到新的未完成任务态
  - 不再只播单行勾选动画
- 当前阶段：
  - 只读根因已定位；尚未进入代码施工

## 2026-04-14 UI 续记｜任务清单完成态快照修复已落地
- 当前主线：
  - 用户批准开修，并要求先有可回退 checkpoint，再安全修复任务清单“完成态先显示，再切下一条”的问题。
- 本轮完成：
  - 先对 UI 4 个脚本做本地 checkpoint 提交：
    - `33985c17` `checkpoint: save UI prompt baseline before completion snapshot fix`
  - 再只改 [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - 给 `PromptRowState / PromptCardViewState` 补了 clone 能力
    - 在 `TransitionToPendingState()` 里新增 completed hold state
    - 当旧主任务完成且下一条即将接管时，UI 会先构造“旧卡完成态整卡快照”
    - 先完整显示旧任务的完成态，再切到新的未完成任务态
  - 修复提交已单独落本地 commit：
    - `f172ec62` `fix: keep completed prompt card state before advancing`
- 验证：
  - `validate_script` 覆盖：
    - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
    - `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs`
    - `Assets/YYY_Scripts/TimeManagerDebugger.cs`
    - 结果：`assessment=no_red owned_errors=0 external_errors=0`
  - `manage_script validate SpringDay1PromptOverlay`：
    - `errors=0 warnings=2`
    - 警告仅为既有 `Update()` 性能提示
  - `errors --count 20`：
    - `0 error / 0 warning`
- 当前判断：
  - 这次修的是 UI own 展示时序，不碰 `SpringDay1Director` 真值
  - 现在任务卡的完成框和正文会先统一在同一个 completed snapshot 上，再交给下一条任务
- 当前阶段：
  - 代码层已落，等待用户 live 验体感

## 2026-04-14 UI 续记｜右上角调试卡微缩 + 右侧 footer 间距回调 + 任务清单二次复盘
- 当前主线：
  - 用户要求这一轮直接落两处排版微调，并重新说明“为什么任务清单还是绿一下就换字、动画和内容不同步”。
- 本轮完成：
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - 右上角调试卡总宽度从 `272` 收到 `248`
    - 只缩窄面板宽度，不缩文字和键帽
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `关闭提示` 向 `BACKSPACE` 靠近
    - 整个 footer 行相对上方操作项再下移一点，拉开上下关系
- 二次复盘结论：
  - 任务清单现在的问题已经不只是“completed hold 太短”。
  - 第一层问题：我上次修的 `completed snapshot` 只收了主任务卡本体，没有冻结 bridge/manual prompt 这条副链。
    - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs) 在 `LateUpdate()` 里会先 `ApplyBridgePromptState(...)`
    - `SpringDay1Director` 在 0.0.5 农田教学推进时，又会立刻 `SpringDay1PromptOverlay.Instance.Show(...)` 下一步文案
    - 结果就是：左边 row 的完成框可能还在播旧任务 completed，但下方/旁边的提示文字已经被下一步 prompt 抢先刷新
  - 第二层问题：当任务清单处于被对话或 modal 压住的阶段时，当前实现不会积累“中间完成态队列”。
    - 它恢复显示时只会拿最新状态，不知道中间错过了哪些 completed 过渡
  - 第三层问题：现有完成态停留时间太短，视觉上只像“绿一下”。
- 当前判断：
  - 你这次的体感是对的：现在确实还是“动画一条线、正文另一条线、bridge prompt 又是第三条线”，所以看起来像各走各的。
  - 下一刀真正要修的不是继续硬拉单行动画，而是把 `PromptCard + BridgePrompt` 的过渡统一到同一个 completion transaction 里。
- 验证：
  - `validate_script InteractionHintOverlay.cs + TimeManagerDebugger.cs`：`owned_errors=0 / external_errors=0 / assessment=unity_validation_pending`
  - fresh console 当前有 2 条外部 `EventSystem` 红，不归这轮 own
  - `manage_script validate InteractionHintOverlay`：`errors=0 warnings=1`
  - `manage_script validate TimeManagerDebugger`：`errors=0 warnings=1`

## 2026-04-14 UI 续记｜completion transaction 二次收口 + sibling 抢位止血
- 当前主线：
  - 用户认可二次分析，要求彻底解决任务清单“完成框、正文、bridge prompt 各走各的”，并补查 `PackagePanel` 与 `SpringDay1PromptOverlay` 层级互跳。
- 本轮完成：
  - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - `_pendingState` 与 `_pendingBridgePromptState` 现在会一起刷新，不再把 bridge prompt 放在主卡之前单独推进
    - 新增 `_suspendBridgePromptSync`，当主任务卡发生显示签名切换时，会先冻结 bridge prompt，等主卡 transition 结束后再统一释放
    - `TransitionToPendingState()` 现在把 `PromptCard + BridgePrompt` 放进同一个 completion transaction
    - `SetExternalVisibilityBlock(false)` 从“解除遮挡后直接 Apply 最新态”改成：如果显示签名变了，就走正式 transition，不再跳过完成过程
    - `OnDialogueEnd()` 也会在需要时启动同一套 transition，而不是只把面板直接淡回来
    - `EnsureHudSiblingOrder()` 在 `Package/Box/Dialogue/Workbench` 这类 modal 打开时直接停手，不再继续抢 sibling，避免 `PackagePanel` 和 `PromptOverlay` 在 hierarchy 里来回互换位置
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - 右上角调试卡宽度继续收窄到更紧凑的 `248`
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `关闭提示` 更贴近 `BACKSPACE`
    - footer 整体与上方操作项再拉开一点
- 当前判断：
  - 这次已经不只是“拉长 completed hold”，而是把主卡、bridge prompt、hidden/re-entry 三条过渡链统一进同一事务
  - `PackagePanel` / `PromptOverlay` 的 hierarchy 互跳问题，代码根因确认来自 `PromptOverlay` 每帧 sibling 纠正；这轮已在 `PromptOverlay` 内部安全止血，不需要去动 `PackagePanel` 逻辑
- 验证：
  - `manage_script validate SpringDay1PromptOverlay`：`errors=0 warnings=2`
  - `errors --count 20 --output-limit 10`：`0 error / 0 warning`
  - `validate_script PromptOverlay + InteractionHintOverlay + TimeManagerDebugger`：
    - `owned_errors=0 / external_errors=0`
    - `assessment=unity_validation_pending`
    - 当前 blocker 是 Unity 现场 `playmode_transition`，不是 owned red
    - fresh console 剩下的是外部字体省略号 warning，不归这轮 own

## 2026-04-17 UI 补记｜木稿工作台配方和木斧对齐
- 用户要求：
  - 木稿子配方改成和木斧一致，只吃木头。
- 已落地：
  - [Recipe_9102_Pickaxe_0.asset](D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1Workbench/Recipe_9102_Pickaxe_0.asset)
    - 从 `3200 x3 + 3201 x2` 改成仅 `3200 x3`
    - 描述同步去掉“木石拼出”的旧语义
- 验证：
  - `git diff --check` 通过
  - `sunset_mcp.py errors` fresh console 为 `0 error / 0 warning`
- 风险控制：
  - 只改资产文件，避免卷入当前工作台 UI / crafting 代码脏现场。

## 2026-04-17 UI 补记｜右上角调试卡显示优化
- 用户要求：
  - 根据截图继续优化右上角时间/调试卡片，但只能改显示，不能动别的内容。
- 已落地：
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - 总宽度收窄到 `224`
    - 时钟卡、快捷键卡高度同步收短
    - tag pill 与标题左收
    - key pill 缩窄，说明文字更贴近内容区
- 验证：
  - `git diff --check` 通过
  - `manage_script validate TimeManagerDebugger` = `errors=0 warnings=1`
  - `sunset_mcp.py errors` fresh console 为 `0 error / 0 warning`
- 风险控制：
  - 仅改 `OnGUI()` 绘制参数和局部布局，不碰功能逻辑。

## 2026-04-17 UI 只读补记｜工作台预扣材料方案
- 结论：
  - 当前工作台不是“点击即扣料”，而是首件在 CraftRoutine() 开始时才扣，追加/排队不扣。
- 风险：
  - 直接复用 TryCraft(recipe, false) 在点击时预扣，会提前触发 OnCraftSuccess，误伤 Day1 的 crafted 计数。
- 推荐窄补丁：
  - CraftingService 增加独立预扣入口
  - Overlay 点击时先预扣再入队
  - 完成一件时再单独发 success
  - 中断/取消统一按未完成件数退款
  - 选择上限去掉 queuedAlready 二次扣减
- 边界：
  - 不碰 Director、InventoryService、PersistentBridge、SaveManager。

## 2026-04-17 UI 补记｜工作台点击即预扣材料落地
- 已完成：
  - 工作台点击开始 / 追加 / 加入队列时，材料立即从背包扣除。
  - 制作完成时才触发 success 通知，避免 Day1 crafted 计数提前。
  - 中断当前制作、取消排队、硬停协程都会退回未完成件数的材料。
  - 回到绑定工作台场景时补运行时状态刷新与背包重绑。
- touched：
  - CraftingService.cs
  - SpringDay1WorkbenchCraftingOverlay.cs
  - WorkbenchInventoryRefreshContractTests.cs
- 验证：
  - WorkbenchInventoryRefreshContractTests：7/7 passed
  - fresh console：0 error / 0 warning
- 剩余：
  - 需要用户 live 验：点击后背包材料是否立即减少；中断/取消是否立即返还；跨场景回来后完成/领取是否正常。

## 2026-04-17 UI 补记｜工作台合同尾账收平
- 用户裁定：
  - “点击制作后材料不足显示红字”这个玩家语义保持不动，不改材料红字逻辑；只修我自查里发现的安全尾账。
- 本轮落地：
  - [SpringDay1DialogueProgressionTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs)
    - 把 `Pickaxe_0` 的旧石头断言改成当前真实配方：`3200 x3`
    - 增补 `DoesNotContain("itemID: 3201")`，防止旧石料需求回流
    - 把工作台首次提示记忆的旧 `PlayerPrefs.SetInt` 断言改成当前真实实现：`StoryProgressPersistenceService.IsWorkbenchHintConsumed()/MarkWorkbenchHintConsumed()`
- 验证：
  - `validate_script`：
    - `SpringDay1DialogueProgressionTests.cs` = `0 error / 0 warning`
    - `WorkbenchInventoryRefreshContractTests.cs` = `0 error / 0 warning`
  - `WorkbenchInventoryRefreshContractTests` EditMode = `7 passed / 0 failed`
  - fresh console（清空后）只剩 `TestJobManager failed to initialize` warning；没有新的 own error
- 未闭环但已报实：
  - 单条 `SpringDay1DialogueProgressionTests.WorkbenchInteraction_ContainsRuntimeBindingBridge` 通过 Unity MCP 跑测试时连续两次卡在 `failed to initialize within timeout`，不是断言失败，暂未拿到该条 live 票。

## 2026-04-18 UI 只读补记｜关系页右侧标签“看起来被区别对待”的真因
- 用户反馈：
  - 关系页里从 `卡尔` 往下的右侧标签块，看起来比上面的 `压场 / 正面 / 路过` 更突出，怀疑被区别对待。
- 只读结论：
  - 我上一轮把主因说成“选中态”是误判；这次复核后确认，真正的问题不是 `卡尔` 单人特殊，而是 `未露面 + 陌生` 这组状态的视觉权重明显更重。
  - 代码里标签尺寸仍是统一常量：
    - `ListChipColumnWidth = 78`
    - `ListChipWidth = 72`
    - `ListChipHeight = 24`
  - 但 `PresenceLevel.None` 返回文案 `未露面`，`Stage.Stranger` 返回文案 `陌生`，并且二者颜色几乎完全相同：
    - `GetPresenceChipColor(None)` = `new Color(0.72f, 0.63f, 0.55f, 0.92f)`
    - `GetStageChipColor(Stranger)` = `new Color(0.72f, 0.63f, 0.55f, 0.92f)`
  - 所以玩家面上会出现“两块同色、同分量、且上面还是三字文案”的重块效果；看起来像下半区 chip 被做得更宽、更重，但不是 `npcId == 003` 这种 if-else 区别对待。
- 当前判断：
  - 这是“状态组合导致的视觉误判被放大”，不是“卡尔被单独特殊处理”。

## 2026-04-18 UI 补记｜关系页 chip 收平 + 时间调试快捷键列对齐
- 用户要求：
  - 不再停留在只读判断，直接把关系页右侧标签“看起来更长更重”的问题修掉，同时优化右上角 `TimeManagerDebugger` 调试快捷键卡片的按钮与文字对齐。
- 已落地：
  - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
    - 右侧 chip 列从“统一等宽重块”改成更轻的两级宽度：
      - presence chip `68`
      - stage chip `64`
    - `chipColumnLayout.childForceExpandWidth` 改为 `false`
    - `chipColumnLayout.childAlignment` 改为 `UpperRight`
    - `未露面` 与 `陌生` 的底色拆开，不再继续用同一棕灰色值
    - chip 内边距和字号轻微收紧，降低“下半区更长更重”的视觉错觉
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - 快捷键标题与说明列对齐到同一纵向网格
    - 所有 key pill 改成固定列宽，不再按文案长度左窄右宽
    - 行距、标题基线和说明列起点重新收平
- 验证：
  - `manage_script validate PackageNpcRelationshipPanel`：`clean`
  - `validate_script TimeManagerDebugger`：`assessment=no_red`
  - `sunset_mcp.py errors`：`0 error / 0 warning`
- 当前阶段：
  - 结构和代码层已过，等待用户 live 看观感是否真的过线。

## 2026-04-18 UI 补记｜设置页右侧重复 F5/F9 介绍移除
- 用户最新裁定：
  - 设置/存档页右侧那块 `F5/F9` 说明与左侧标题下的副说明重复，而且当前还发生了重叠；不要再挪位置，直接删掉右侧这块多余介绍。
- 本轮实际落地：
  - [PackageSaveSettingsPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs)
    - 删除 `headerRight` 中运行时生成的 `GuideCard`
    - 一并移除 `_guideShortcutText / _helpText` 字段与两处刷新赋值
    - 右侧现在只保留真正需要的操作按钮：`重新开始`、`退出游戏`
- 用户可感知结果：
  - 左侧继续承担 `F5 / F9` 与默认槽说明
  - 右侧不再重复再讲一遍，也不会再因为那张卡片过窄而叠字
- 验证：
  - `validate_script PackageSaveSettingsPanel`：`0 error / 0 warning`
  - `find_in_file HelpFontSize|GuideTint|_guideShortcutText|_helpText`：`0 match`
  - fresh console：无新的 own error / warning
- 边界：
  - 这轮只删冗余介绍卡，不改保存逻辑、不改按钮行为、不改整页布局体系。

## 2026-04-23 UI 上传补记｜shared-root 保本上传收口结果
- 当前主线：
  - 不再继续补玩家面体验，只把 UI 当前本地 own 成果按 shared-root 白名单规则安全归仓并 push 到 `origin`。
- 本轮已完成：
  - 文档/记忆批：
    - `.codex/threads/Sunset/UI/memory_0.md`
    - `.kiro/specs/UI系统/memory.md`
    - `.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/memory.md`
    - 3 份 `2026-04-14_UI线程_*` prompt
    - 已提交并 push：`ea6ac827338bb1a382e5ef1b41ac49cc3b392353`
  - 字体材质批：
    - `DialogueChinese Pixel SDF.asset`
    - `DialogueChinese V2 SDF.asset`
    - 已提交并 push：`edd3baea26acf78fd4327a16cfe0e81656e83cc3`
- 当前 blocker：
  - 剩余 `Assets/YYY_Scripts/UI/*` 与 `Assets/YYY_Scripts/Story/UI/*` 代码批次未继续归仓。
  - 原因不是白名单越界，而是 `CodexCodeGuard` 在这批 `.cs` 上进入 `pre-sync` 时无法稳定返回 JSON：
    - `Ready-To-Sync(UI runtime batch)` 报：`FATAL: CodexCodeGuard 未返回 JSON 结果`
    - 直跑 `CodexCodeGuard.dll` 到单文件也会挂住或 exit 1，无可用 JSON
- 当前阶段：
  - 可安全提交的文档与字体资产已全部上到 `origin`；剩余 UI 代码面停在 exact blocker，未硬吞。

## 2026-04-23 UI 上传续记｜历史小批次 Tabs 新 panel/runtime-kit 试传被同根残留阻断
- 当前主线：
  - 按第二波 shared-root 治理改判，不再整包上传剩余 UI 代码；只还原 `1` 个历史小批次上传，并且撞 blocker 就停车。
- 本轮唯一尝试白名单：
  - `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs`
  - `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs.meta`
  - `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
  - `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs.meta`
  - `Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs`
  - `Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs.meta`
- 历史批次判断：
  - 这 3 个 `.cs` 都是 `Assets/YYY_Scripts/UI/Tabs/` 下同一簇新增 runtime panel/helper 文件，创建时间都落在 `2026-04-08`，可视为同一历史创建批次。
  - 但它们不是“可独立 sync 的纯净批次”，因为现存脏改 [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs) 已经接入并引用了这批新增 panel。
- 本轮真实结果：
  - 已按白名单单独跑 `Begin-Slice`
  - 已对白名单这 6 个文件单独跑 `Ready-To-Sync`
  - 未提交、未 push
- 第一真实 blocker：
  - 不是 `CodexCodeGuard`
  - 是 same-root mixed / own-root remaining dirty
  - `Ready-To-Sync` 明确报：
    - 当前白名单所属 `own root = Assets/YYY_Scripts/UI/Tabs`
    - 同根仍有未纳入本轮的剩余脏改：`M Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
  - 因此这轮在 preflight 第一层就被阻断，根本还没走到 codegate 挂 JSON 的那一层
- 当前阶段：
  - 这一个历史小批次已被精确报死
  - 除这组之外，其它 `Inventory / Toolbar / Box / Story/UI` 尾账本轮未动
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：已跑，结果为 blocker
  - `Park-Slice`：已跑，当前重新回到 `PARKED`

## 2026-04-23 UI 复核补记｜重复收到同一历史小批次 prompt，不重跑第二次
- 用户再次下发同一份 `历史小批次上传 prompt_02`。
- 我已重新读取 prompt、核对 [UI.json](D:/Unity/Unity_learning/Sunset/.kiro/state/active-threads/UI.json) 和当前 Tabs 白名单现场。
- 结论：
  - 这不是一个新的第二批，也不是新 blocker。
  - 当前仍然是同一个已执行过的小批次，状态仍为：
    - `PARKED`
    - blocker = `Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs` 同根残留
- 因此本轮不再重复跑第二次 `Begin-Slice / Ready-To-Sync`，以免把“撞 blocker 就停车”变成无效重撞。

## 2026-04-24 UI 上传续记｜Tabs 根内整合批纳入 PackagePanelTabsUI 后，阻断升级为 CodeGuard
- 当前主线：
  - 按 `prompt_03` 不再重跑 `prompt_02`，而是把 [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs) 正式纳入同批核心件，对 `UI/Tabs` 根内整合批做一次新的真实上传尝试。
- 核心批次判断：
  - 现已正式承认 `PackagePanelTabsUI.cs` 属于这批核心件。
  - 依据不是今天临时扩包，而是代码事实：
    - `PackagePanelTabsUI.cs:54`
    - `PackagePanelTabsUI.cs:55`
    - `PackagePanelTabsUI.cs:68`
    - `PackagePanelTabsUI.cs:69`
    - `PackagePanelTabsUI.cs:216`
    - `PackagePanelTabsUI.cs:217`
    - 都直接 `EnsureOptionalPanelInstalled("PackageMapOverviewPanel") / EnsureOptionalPanelInstalled("PackageNpcRelationshipPanel")`
- 本轮唯一白名单：
  - `PackagePanelTabsUI.cs`
  - `PackageMapOverviewPanel.cs`
  - `PackageMapOverviewPanel.cs.meta`
  - `PackageNpcRelationshipPanel.cs`
  - `PackageNpcRelationshipPanel.cs.meta`
  - `PackagePanelRuntimeUiKit.cs`
  - `PackagePanelRuntimeUiKit.cs.meta`
- 根内现场：
  - 当前 `Assets/YYY_Scripts/UI/Tabs` 根下相关脏改正好就是这 7 个文件，没有新的根内 remaining dirty/mixed。
- 本轮真实结果：
  - 已跑 `Begin-Slice`
  - 没有继续到 `Ready-To-Sync / sync`
  - 未提交、未 push
- 新的第一真实 blocker：
  - 不再是 same-root mixed
  - 已升级为 `CodexCodeGuard` / CLI codegate 阻断
  - 具体表现：
    - `PackageMapOverviewPanel.cs`：`CodexCodeGuard returned no JSON`
    - `PackageNpcRelationshipPanel.cs`：`CodexCodeGuard returned no JSON`
    - `PackagePanelRuntimeUiKit.cs`：`CodexCodeGuard returned no JSON`
    - `PackagePanelTabsUI.cs`：`validate_script = unity_validation_pending`，并带 `baseline_fail / mcp connect refused`
- 当前阶段：
  - `UI/Tabs` 根内整合批已完成一次真实上传尝试
  - 本轮 exact blocker 已从“同根残留”推进到“代码闸门失稳”
  - 除此之外，本轮没有越权扩到 `Inventory / Toolbar / Box / Story/UI`
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Park-Slice`：已跑
  - 当前重新回到 `PARKED`

## 2026-04-26 UI 复核续记｜工具修复后 UI/Tabs 七文件最小复核
- 当前主线：
  - 按 `prompt_05`，在治理位已修 `CodexCodeGuard / git-safe-sync` 后，只对白名单 `UI/Tabs` 七文件做 `1` 次真实最小复核；不修业务代码，不换第二批。
- 本轮真实动作：
  - 已跑新的 `Begin-Slice`
  - 已顺序跑 `Ready-To-Sync`
  - 未修任何 `UI/Tabs` 业务代码
  - 未进入第二个切片
- 这轮最关键的结论：
  - 这批现在**不再**出现：
    - `CodexCodeGuard returned no JSON`
    - `baseline_fail 黑盒`
  - `Ready-To-Sync` 已能稳定返回真实代码闸门结果
- 新的第一真实 blocker：
  - 已从工具 incident 降级成真实业务 blocker
  - `Ready-To-Sync` 明确报：
    - `3` 条错误
    - `1` 条警告
  - 全部都落在 [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs)
  - 具体为：
    - `CS0103` `PackagePanelTabsUI.cs:53:9` 当前上下文中不存在名称 `PackageSaveSettingsPanel`
    - `CS0103` `PackagePanelTabsUI.cs:67:9` 当前上下文中不存在名称 `PackageSaveSettingsPanel`
    - `CS0103` `PackagePanelTabsUI.cs:215:9` 当前上下文中不存在名称 `PackageSaveSettingsPanel`
    - `CS0649` `PackagePanelTabsUI.cs:20:40` 字段 `boxUIRoot` 从未赋值
- 根内状态：
  - `own roots = Assets/YYY_Scripts/UI/Tabs`
  - `own roots remaining dirty 数量 = 0`
  - 说明这轮确实已经不再是同根漏项问题
- 当前阶段：
  - 这组现在不属于“工具黑盒未解”
  - 也还不是“可以继续上传”
  - 它已经明确降级成 `真实业务 blocker`
- 边界：
  - 本轮没有越权扩到 `Inventory / Toolbar / Box / Story/UI`
  - 本轮没有顺手修任何业务代码
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：已跑，得到真实 blocker
  - `Park-Slice`：已跑，当前 `PARKED`
