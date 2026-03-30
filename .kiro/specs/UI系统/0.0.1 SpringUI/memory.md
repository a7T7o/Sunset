# SpringUI 工作区记忆

## 2026-03-28 初始化补记：已完成 Day1 基线复刻与长期路线的只读总方案落盘

- 当前工作区主线：不是进入 `spring-day1` UI 实现，而是先在 `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI` 下收口一份高质量总方案，明确 Day1 当前 UI 事故的本体、正确顺序、分层答案和禁路线。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `delivery-self-review-gate`
- `sunset-startup-guard` 当前会话未显式暴露；已按 Sunset `AGENTS.md` 手工完成等价前置核查，并保持只读边界，不进入 Unity / MCP，不进入实现。
- 本轮读取依据：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\场景搭建（外包）\2026-03-28_给UI线程的重新对齐任务书.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\场景搭建（外包）\2026-03-28_spring-day1_UI线程审核报告.md`
  - 以及 Day1 已掌握的 prefab / runtime 代码 / 委托原文 / 交接证据
- 本轮站稳的结论：
  1. 这次 UI 问题本体不是“架构不够高级”，而是用户手调 prefab 这个唯一真视觉基线没有重新接回 runtime。
  2. 当前 runtime UI 不是直接实例化用户手调后的两个 prefab，而是仍在走 `EnsureRuntime() -> new GameObject -> AddComponent -> BuildUi()` 的代码生成路线，因此 formal-face 与 runtime face 双源并存。
  3. 当前不能先抽象；正确顺序必须是：
     - `先抄`
     - `再稳`
     - `再抽`
  4. 长期技术答案不是三选一，而是：
     - 视觉 prefab
     - 行为代码
     - 差异数据
  5. 用户那 6 条需求已经在总方案中完成分层和分阶段映射，后续实现不得再把 Phase 1 和 Phase 2 混写。
  6. 当前明确不能走的路线也已写死，包括：
     - 继续纯 `BuildUi()`
     - 把 prefab 继续当参考图
     - 现在直接跳 binder / provider / 模板化实现
     - 拿当前错版 runtime 直接推广到所有工作台
- 本轮产出：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\SpringUI-Day1基线复刻与长期技术路线总方案.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\memory.md`
- 验证状态：
  - `静态推断成立`
  - `已对齐指定任务书与审核报告`
  - `尚未进入实现`
- 当前恢复点：
  - `SpringUI` 工作区的路线基线已经建立；
  - 下一步等待用户/审核方审这份总方案；
  - 在过审前，不进入 Day1 UI 实现。

## 2026-03-28 Phase 1 第一刀补记：已把 Day1 两个 overlay 的 runtime 创建链切到 prefab-first

- 当前工作区主线：执行 `Phase 1 第一刀`，只把 `SpringDay1PromptOverlay` 与 `SpringDay1WorkbenchCraftingOverlay` 的 runtime 创建链从“代码现场长壳”改成“prefab-first 接回用户手调 prefab 作为视觉真源”。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `sunset-no-red-handoff`
- `sunset-startup-guard` 当前会话未显式暴露；已按 Sunset `AGENTS.md` 做手工等价前置核查，且全程未进入 Unity / MCP。
- 本轮实际修改：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1UiPrefabRegistry.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Resources\Story\SpringDay1UiPrefabRegistry.asset`
- 本轮明确没动：
  - `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\UI\Spring-day1\SpringDay1PromptOverlay.prefab`
  - `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\UI\Spring-day1\SpringDay1WorkbenchCraftingOverlay.prefab`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\CraftingStationInteractable.cs`
- 本轮站稳的实现结论：
  1. `PromptOverlay` 与 `WorkbenchOverlay` 的 `EnsureRuntime()` 现已先走：
     - 复用现有场景实例
     - 通过 prefab 实例化 runtime 壳
     - 只有前两步都失败时才 fallback 到旧 `BuildUi()`
  2. 两个 overlay 现在都会先从 `Resources/Story/SpringDay1UiPrefabRegistry.asset` 取到现有手调 prefab；编辑器下保留 `AssetDatabase.LoadAssetAtPath` 作为兜底，不再让 editor-only 入口冒充 runtime 主链。
  3. `SpringDay1UiPrefabRegistry` 只服务 Day1 这两份 prefab 的接回，不是 binder/provider/模板化通用层。
  4. `PromptOverlay` 的页面壳、文本节点、任务行模板已改成优先绑定 prefab 现有结构；`WorkbenchOverlay` 的面板壳、左右列、按钮和 recipe 行模板也改成优先绑定 prefab 现有结构。
  5. 这轮没有改任何 prefab 视觉参数；做的是“把 prefab 接回 runtime 的入口与绑定”，不是重新设计视觉。
- 本轮验证：
  - `git diff --check` 已通过（仅有 Git 的 CRLF/LF 提示，不是阻断错误）
  - `CodexCodeGuard` 对 3 个 C# 文件执行 `utf8-strict + git-diff-check + roslyn-assembly-compile`，结果 `CanContinue = true`
  - 验证状态：`静态编译闸门已过`
- Git 收口状态：
  - 已尝试 `sunset-git-safe-sync.ps1 -Action sync -Mode task -OwnerThread spring-day1`
  - 当前被 same-root remaining dirty 阻断，阻断根为：
    - `.kiro/specs/UI系统`
    - `Assets/Resources/Story`
    - `Assets/YYY_Scripts/Story/UI`
    - `.codex/threads/Sunset/spring-day1`
  - 阻断原因不是本轮 3 个脚本的代码闸门失败，而是 `spring-day1` 历史同根残留过多，脚本拒绝把这刀伪装成已 clean 收口
- 本轮明确还没做：
  - 自适应布局
  - 日历撕页
  - ScrollRect / Viewport / Mask 体验修复
  - 制作按钮 / 进度条状态机
  - 离台小进度
  - 固定锚定
  - binder / provider / 模板化
- 当前恢复点：
  - Phase 1 第一刀的目标已经完成到“runtime 先长对脸”这一层；
  - 若继续，只能进入 Phase 2 的体验增强项，不能再回头把 Phase 1 和模板化混写。

## 2026-03-28 Phase 2 第二步补记：Day1 UI 已补完 6 条体验问题的主实现，并拿到最小 Unity 运行证据

- 当前工作区主线：执行 `Phase 2` 第二步，只在 Day1 当前 prefab-first 主链上，把“看得顺眼、用起来不崩”的 6 条体验问题收口，不进入 binder/provider/模板化。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-no-red-handoff`
  - `sunset-unity-validation-loop`
- `sunset-startup-guard`、`sunset-workspace-router`、`user-readable-progress-report`、`delivery-self-review-gate` 当前会话未全部显式暴露；已按 Sunset `AGENTS.md` 与对应等价流程手工完成启动核查、工作区路由、用户向汇报组织和交付前自审。
- 本轮实际修改：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1UiLayerUtility.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
- 本轮站稳的实现结论：
  1. `PromptOverlay` 已在 prefab-first 主链上补齐 legacy 手摆结构的自适应布局、页面壳体补全、显示态缓存自愈、`DisplaySignature` / `ApplyPendingStateWithoutTransition` 原位刷新，以及连续对话结束事件的保护；不再要求 prefab 必须先长成代码壳那套结构才可用。
  2. `WorkbenchOverlay` 已在当前 prefab 壳上补齐 recipe viewport、materials viewport、floating progress 的兼容承载层；左侧 `Viewport/Content/ScrollRect/Mask`、右侧材料区滚动链和离台小进度不再依赖 prefab 必须提前有完整 runtime 节点。
  3. 右侧详情区现在会按名称、描述、材料清单、数量区、stage hint、progress label 的实际文本高度重新排版；文本增多时优先让位和滚动，而不是直接重叠。
  4. `PromptOverlay` 的翻页仍走 prefab-first 页面壳，但已经强化成“前页掀起、下页在下”的双页语义；同时对实时进度改成同页原位刷新，避免高频刷新把整页抖动成假翻页。
  5. `WorkbenchOverlay` 的按钮/进度条、离台小进度、固定锚定这三块没有再退回代码壳；当前通过兼容承载层 + `SnapToCanvasPixel` 保留 prefab 视觉基线，并把位置抖动压到像素对齐。
- 对 6 条体验项的当前判断：
  1. `自适应`：已落到 Prompt legacy 页和 Workbench 详情/材料区两侧的 runtime 布局逻辑中，主实现已补上。
  2. `撕页`：已保留双页结构并强化前页掀起/下页在下的动画语义，主实现已补上。
  3. `Recipe / Viewport / Scroll / Materials`：compatibility 层已补齐并通过 Workbench runtime 测试验证节点存在。
  4. `按钮 / 进度条状态机`：现有制作态状态机继续保留，Phase 2 没再退回死按钮；本轮主要是把 prefab 壳与兼容布局接稳。
  5. `离台小进度`：compatibility 层已确保 `FloatingProgressRoot/Icon/Fill/Label` 存在，并通过 runtime 测试回证。
  6. `固定锚定不漂`：主修正是把工作台大面板与离台小进度的位置统一改为 `SnapToCanvasPixel`，先压掉子像素漂移。
- 本轮验证：
  - `git diff --check` 通过（仅有 Git 的 CRLF/LF 提示，不是阻断错误）
  - `CodexCodeGuard` 对 `SpringDay1UiLayerUtility.cs`、`SpringDay1PromptOverlay.cs`、`SpringDay1WorkbenchCraftingOverlay.cs`、`SpringDay1UiPrefabRegistry.cs` 的 `utf8-strict + git-diff-check + roslyn-assembly-compile` 通过，`CanContinue = true`
  - `mcp validate_script`：
    - `SpringDay1PromptOverlay.cs`：`0 error / 1 warning`
    - `SpringDay1WorkbenchCraftingOverlay.cs`：`0 error / 1 warning`
    - `SpringDay1UiLayerUtility.cs`：`0 error / 0 warning`
  - 定向 EditMode 测试通过：
    - `SpringDay1LateDayRuntimeTests.PromptOverlay_RecoversWhenDisplayedStateCacheIsMissing`
    - `SpringDay1LateDayRuntimeTests.WorkbenchOverlay_RecoversCompatibilityNodesFromPrefabShell`
  - 额外文本级验收探针：
    - `SpringDay1DialogueProgressionTests.PromptOverlay_SuppressesItselfDuringDialogue` 现已不再卡在 Prompt 自身；剩余失败点明确落在 `SpringDay1WorldHintBubble.cs` 缺少 `ShouldIgnoreDialogueEndEvent()`，属于这轮主刀范围外的旧约束
  - 当前 Unity Console 仍不干净，但隔离后未出现直指 `PromptOverlay` / `WorkbenchOverlay` 的新堆栈；现场仍有 `AudioListener`、`NPCValidation` 与场景级 assertion 噪音，需要与本轮 UI 改动分开看
- 已知未闭环项：
  1. `SpringDay1LateDayRuntimeTests` 整类仍有一个与本轮 UI 改动无关的旧失败：`FreeTimeValidationStep_AdvancesFromFinalCallToDayEnd` 里 `IsLowEnergyWarningActive` 未在 DayEnd 后归零
  2. `SpringDay1WorldHintBubble.cs` 的连续对话 `DialogueEndEvent` 保护未在这轮 write set 内一起收掉，因此文本级验收探针仍报这一个范围外缺口
  3. 当前仓库 same-root dirty 很重，本轮依然不能直接 claim `spring-day1` 整根 sync-ready
- 当前阶段判断：
  - `Phase 2` 的 Day1 UI 主实现已经到“可交用户验体验”的阶段；
  - 但技术现场仍需要把“范围外旧噪音”和“这轮 UI 结果”分开理解，不能把整个 shared root 的脏现场误写成 Phase 2 自身失败。
- 当前恢复点：
  - 如果下一步由我继续，只应该进入用户终验后的精修或处理明确授权的范围外旧噪音；
  - 不应跳到 Step 3 模板化，也不应把 Day1 这轮又退回代码壳主链。

## 2026-03-28 现场复核补记：Phase 2 当前应收紧为“几何漂移纠偏”，不能按可终验放行

- 当前工作区主线：继续只做 Day1 UI 的审核与路线收口，不进入实现；本轮目标是把 UI 线程对 `Phase 2` 的“已可终验”口径，拿 live 现场重新复核一遍。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `unity-mcp-orchestrator`
- `sunset-startup-guard`、`user-readable-progress-report`、`delivery-self-review-gate` 当前会话未显式暴露；已按 Sunset AGENTS 做手工等价启动闸门、用户向汇报组织与停步自省。
- 本轮 live 取证：
  - shared root 当前仍为 `main + neutral`
  - Unity/MCP 单实例存在，但本轮只做最小读现场
  - 已进入一次最短 Play，执行 `Sunset/Story/Debug/Bootstrap Spring Day1 Validation`，随后读取 `SpringDay1LiveValidation` 快照与 runtime 组件值，并确认已回到 Edit Mode
- 本轮现场确认到的关键事实：
  1. runtime 下确实存在 `UI/SpringDay1PromptOverlay`
  2. 其 `Canvas.renderMode = 0`，即 `ScreenSpaceOverlay`
  3. 其 `CanvasGroup.alpha = 1.0`
  4. `SpringDay1LiveValidation` 日志明确给出：`Prompt=alpha=1.00|text=0.0.2 首段推进链`
  5. 因为它是 `ScreenSpaceOverlay`，MCP 从 `Main Camera` 抓到的 camera screenshot 不能再当成 Prompt 是否显示/是否长对的硬证据
- 本轮重新坐实的判断：
  - `Phase 2` 当前不能按“已可交用户终验”处理
  - 问题本体不是“没做自适应”，而是“自适应越权”
  - 当前该收的是：`外壳几何冻结 + 动态布局只允许纵向下推`
- 已再次钉实的越权点：
  - `SpringDay1PromptOverlay.cs:1279` 仍直接写 `cardRect.sizeDelta = new Vector2(pageWidth + 12f, pageHeight + 10f)`
  - `SpringDay1PromptOverlay.cs:1315` 仍直接写 `page.root.sizeDelta = new Vector2(width, height)`
  - `SpringDay1WorkbenchCraftingOverlay.cs:1192` 仍直接写 `panelRect.sizeDelta = new Vector2(panelWidth, panelHeight)`
  - `SpringDay1WorkbenchCraftingOverlay.cs:1206-1215` 仍通过一整串 `SetTopStretch / SetBottomStretch` 重新接管右栏几何
- 与 prefab 真值的治理口径：
  - Prompt `TaskCardRoot` 真值以 `SpringDay1PromptOverlay.prefab` 为准：`anchoredPosition = {11.900024,-12.9672}`、`sizeDelta = {328,229.9346}`
  - Workbench `PanelRoot` 真值以 `SpringDay1WorkbenchCraftingOverlay.prefab` 为准：`anchoredPosition = {87.79856,-126.66856}`、`sizeDelta = {428,257.1085}`
  - 后续纠偏必须围绕“冻结这些外壳几何”展开，不能再用兼容布局层重算整体壳体
- 本轮新增 / 更新产出：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\场景搭建（外包）\2026-03-28_UI线程Phase2几何漂移纠偏任务书.md` 已补入 live 取证口径与更硬的证据要求
- 当前恢复点：
  - 现在应该直接把“几何漂移纠偏任务书”发给 UI 线程
  - 下一轮只审它是否把外壳几何收死，不再接受“代码链存在 / 测试通过 / 仍待你看体验”式宽回执

## 2026-03-28：ScreenSpaceOverlay / GameView 取证工具已在 shared root 落地并完成真闭环

- 当前工作区主线：不是继续改 Day1 UI 体验，而是补齐 `ScreenSpaceOverlay` 最终观感的取证能力缺口，让后续 Prompt / Workbench 验收不再误用 Main Camera 截图。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `unity-mcp-orchestrator`
  - `sunset-no-red-handoff`
  - `sunset-unity-validation-loop`
  - `skill-creator`
- `sunset-startup-guard` 当前会话未显式暴露；已按 Sunset AGENTS 手工完成 shared root / MCP / 工作区等价启动核查。
- 本轮实际落地：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringUiEvidenceCaptureRuntime.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringUiEvidenceMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\scripts\SpringUiEvidence.ps1`
  - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\README.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\2026-03-29_ScreenSpaceOverlay取证工具说明.md`
  - `C:\Users\aTo\.codex\skills\sunset-ui-evidence-capture\SKILL.md`
- 本轮同时处理的阻塞：
  - 已把 `Assets\YYY_Scripts\Story\UI\SpringDay1StatusOverlay.cs(.meta)` 按 `HEAD` 内容恢复，消除这轮工具验证前的外部编译缺口
  - 已清掉误落在 `Assets\Screenshots\` 下的临时截图，证据改统一落到 `.codex\artifacts\ui-captures\spring-ui`
- 本轮站稳的规则：
  1. Prompt / 其他 `ScreenSpaceOverlay` UI 的最终观感证据，必须来自最终合成屏抓图，而不是 Main Camera 截图
  2. 证据目录固定为：
     - `pending`
     - `accepted`
     - `latest.json`
     - `manifest.jsonl`
  3. `pending` 默认 `14` 天可清理，`accepted` 不自动删除
  4. 证据图不进 Git，只保留目录结构、说明和脚本
- 本轮验证：
  - `validate_script`：
    - `SpringUiEvidenceCaptureRuntime.cs` = `0 error / 0 warning`
    - `SpringUiEvidenceMenu.cs` = `0 error / 0 warning`
  - Unity live：
    - 已进入 PlayMode
    - 已执行 `Sunset/Story/Debug/Bootstrap + Capture Spring UI Evidence`
    - 成功生成：
      - `.codex/artifacts/ui-captures/spring-ui/pending/20260328-231142-283_bootstrap.png`
      - `.codex/artifacts/ui-captures/spring-ui/pending/20260328-231142-283_bootstrap.json`
    - 侧车 JSON 已确认：`prompt.renderMode = ScreenSpaceOverlay`、`prompt.canvasAlpha = 1.0`
    - Unity 菜单 `Promote Latest Spring UI Evidence` 已跑通，最新证据已进入 `accepted`
    - PowerShell 脚本 `latest / promote-latest / prune -DryRun` 已跑通
    - Unity 最后已退出 PlayMode，现场已回 EditMode
- 当前已知非本轮 blocker：
  - `git diff --check` 仍被仓库内他线已有的 TMP 字体资源 trailing whitespace 拦住，不是本轮新引入问题
  - shared root 仍有大量 unrelated dirty，因此本轮收口必须坚持白名单，不得假装整仓 clean
- 当前恢复点：
  - 以后凡是 Day1 / SpringUI 需要最终观感证据，都优先用这套工具
  - 下一步进入 Git 白名单收口；若继续被 unrelated dirty 或仓库级 `diff --check` 阻断，需要如实报实，不做越界清理

## 2026-03-28 Phase 2 几何漂移纠偏补记：Workbench 的 MaterialsViewport 已从默认新建块收回到内容层几何

- 当前工作区主线：严格执行 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\场景搭建（外包）\2026-03-28_UI线程Phase2几何漂移纠偏任务书.md`，不再沿用“Phase 2 已可终验”的旧口径，只做 `PromptOverlay / WorkbenchOverlay` 的几何越权回收。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-no-red-handoff`
  - `sunset-unity-validation-loop`
- 本轮手工等价执行：
  - `sunset-startup-guard`
  - `sunset-workspace-router`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮唯一代码主刀：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
- 本轮真正收掉的根因：
  1. `MaterialsViewport` 在 prefab 里并不存在现成节点，旧兼容层运行时临时创建时直接吃到了 `CreateRect()` 的默认 `100x100`。
  2. `RefreshCompatibilityLayout()` 旧判断把 `QuantityTitle` 错当成 `DetailColumn` 的直系子节点；但 prefab 真层级里 `QuantityTitle` 挂在 `QuantityControls` 下，所以这段纵向布局刷新根本没有命中。
- 本轮落实的几何纠偏：
  1. `EnsureMaterialsViewportCompatibility()` 现在在“从 `SelectedMaterials` 现地包一层 viewport”时，会直接复制 `SelectedMaterials` 的 prefab 几何与 sibling 顺序，再补 `Mask/ScrollRect/Content`，不再留下默认 `100x100` 的匿名新建块。
  2. `RefreshCompatibilityLayout()` 现在只改：
     - `SelectedName`
     - `SelectedDescription`
     - `MaterialsTitle`
     - `MaterialsViewport`
     也就是只在内容层做纵向下推；下边界改为以 `QuantityControls / ProgressBackground / CraftButton` 为 floor，不再错误依赖 `QuantityTitle` 的直系层级。
  3. `PanelRoot`、左右栏外壳、按钮区与进度区这轮都没有重新引入 `panelRect.sizeDelta = ...` / `SetBottomStretch(...)` 这类 runtime 几何重写；Workench 外壳宽高继续以 prefab-first 主链为真源。
- Prompt 当前状态：
  - 本轮没有再改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - 因为上一刀已经把 Prompt 的几何越权收窄到内容层：legacy 页面刷新只做 `SetTopKeepingHorizontal(...)` 的纵向下推，不再在刷新链里重写 `TaskCardRoot / Page / BackPage` 的宽高
  - 旧 `cardRect.sizeDelta` / `page.root.sizeDelta` 只剩 `BuildUi()` fallback 内的代码壳默认值，不是 prefab-first 主链 live 几何来源
- 本轮静态验证：
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs` 通过（仅有 Git 的 `CRLF/LF` 提示，不是阻断错误）
  - `validate_script(WorkbenchCraftingOverlay.cs)`：`0 error / 1 warning`
  - `CodexCodeGuard` 对以下 4 个文件的 UTF-8 / diff / Roslyn 程序集级编译检查通过，`CanContinue = true`
    - `SpringDay1PromptOverlay.cs`
    - `SpringDay1WorkbenchCraftingOverlay.cs`
    - `SpringDay1UiLayerUtility.cs`
    - `SpringDay1UiPrefabRegistry.cs`
- 本轮 live 取证：
  - 已进入 Play，执行 `Sunset/Story/Debug/Bootstrap Spring Day1 Validation`
  - 通过 `Step Spring Day1 Validation` 把现场推进到工作台链出现过的阶段后抓证据
  - 已执行 `Sunset/Story/Debug/Capture Spring UI Evidence`
  - 已生成：
    - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\pending\20260328-232925-346_manual.png`
    - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\pending\20260328-232925-346_manual.json`
  - 已确认退出 PlayMode，现场回到 EditMode
- 本轮 live 证据摘录：
  - Prompt：
    - `Canvas.renderMode = ScreenSpaceOverlay`
    - `CanvasGroup.alpha = 1.0`
    - `TaskCardRoot.anchoredPosition = (11.900024, -12.9672003)`
    - `TaskCardRoot.sizeDelta = (328, 229.9346008)`
    - `Page.anchoredPosition = (0, 0)` / `Page.sizeDelta = (0, 0)` / `Page.rectSize = (328, 229.9346008)`
    - `BackPage.anchoredPosition = (0, 0)` / `BackPage.sizeDelta = (0, 0)` / `BackPage.rectSize = (328, 229.9346008)`
  - Workbench：
    - `PanelRoot.anchoredPosition = (349.5, 264.75)`
    - `PanelRoot.sizeDelta = (428, 257.10849)`
    - `Recipe Viewport.anchoredPosition = (0, -14)` / `sizeDelta = (-16, -48)`
    - `MaterialsViewport.anchoredPosition = (17.6, -108.00001)` / `sizeDelta = (-27.6, 41.800003)`，已不再是默认 `100x100`
    - `ProgressBackground.anchoredPosition = (-0.0000153, 14.999001)` / `sizeDelta = (-10.258, 20.798)`
    - `CraftButton.anchoredPosition = (0, 14.999496)` / `sizeDelta = (-10.2577, 20.7989)`
- 当前恢复点：
  - `Phase 2 几何漂移纠偏` 这刀已经拿到可直接回用户的硬证据
  - 下一步不是继续扩体验项，而是等待用户审这轮“外壳冻结 / 内容层纵向下推 / live Rect 实值”是否过线
- Git 收口状态：
  - 已尝试使用稳定 launcher 执行 `sunset-git-safe-sync.ps1 -Action sync -Mode task -OwnerThread spring-day1`
  - 当前被 same-root hygiene 阻断，阻断根为：
    - `.kiro/specs/UI系统`
    - `Assets/YYY_Scripts/Story/UI`
    - `.codex/threads/Sunset/spring-day1`
  - 阻断原因不是本轮几何纠偏脚本编译失败，而是 `spring-day1` 在这些 own roots 下仍有 `109` 条未收同根 dirty / untracked，脚本拒绝把这刀伪装成已 clean 收口

## 2026-03-29 需求补记：当前优先修 Workbench 整面与 Prompt 正式面，最近交互提示系统单列后续切片

- 当前工作区主线：继续服务 Day1 / SpringUI 的 UI 收口，但用户刚追加了最新验收与交互需求，因此本轮需要把“立刻修的”和“先记住、后续再开刀的”明确分层。
- 本轮新增治理结论：
  1. 当前正在推进的 UI 修复切片，应收窄为：
     - `Workbench` 整面过线：左列 recipe 真实可见 + 右侧 `所需材料` 正式面收好
     - `Prompt` 任务栏 formal-face 回正
  2. 用户已明确确认：Workbench 左列“空但可点可切换”，因此问题本体不是没数据，而是显示链 / 配置链；后续排查应优先围绕 `RecipeColumn / Viewport / Content / Mask / ScrollRect / item 可见性`。
  3. 用户已明确确认：Workbench 右侧 `所需材料` 区也仍是半成品样式，不能把“左边显示了”当成 Workbench 这一轮过线。
- 本轮补记到后续切片、暂不进入当前轮实现的要求：
  1. NPC 靠近提示不能只是 gizmos 下几乎看不见的小点，后续必须做成真实可感知、专业、小巧的交互提示。
  2. 同一时刻只能由最近的一个可交互目标触发提示与 `E` 交互：
     - NPC
     - 工作台
     - 其他可交互物
     必须统一仲裁，不能多目标同时提示。
  3. 视觉归属与实际触发对象必须一致：看到谁头上的提示，就只能由谁触发。
  4. NPC 的 `E` 交互中，剧情对话优先于非正式气泡聊天。
  5. 聊天气泡后续需要把速度放慢到正常可读节奏。
- 对这批后续需求的当前裁定：
  - `尚未进入实现`
  - `必须保留`
  - `应在 Workbench / Prompt 当前收口完成后，单独切一轮“最近交互提示统一仲裁 + NPC 提示体验”再做`
- 当前恢复点：
  - UI 线程当前仍应先完成 Workbench 与 Prompt 这一轮硬切片；
  - 最近交互提示 / NPC 气泡速度 这组需求已经正式补记，后续不可遗漏。

## 2026-03-29 Phase2 收口补记：Workbench 整面与 Prompt formal-face 已拿到 accepted GameView 证据

- 当前工作区主线：严格执行 `2026-03-28_UI线程Phase2-Workbench显示链与Prompt正式面收口任务书.md`，只收两张面：
  - `Workbench` 整面过线
  - `Prompt` 回到 formal-face
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `sunset-ui-evidence-capture`
  - `sunset-no-red-handoff`
  - `sunset-unity-validation-loop`
- 本轮手工等价执行：
  - `sunset-startup-guard`
  - `preference-preflight-gate`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮实际结论：
  1. `Workbench` 左列之前不是没数据，而是显示链问题；承接上一刀对 `SpringDay1WorkbenchCraftingOverlay.cs` 的修正后，live 证据已经确认：
     - `recipeRowCount = 3`
     - 左列图标与文字真实可见
     - 右侧 `MaterialsViewport` / `SelectedMaterials` / `StageHint` 已回到正式面可读状态
  2. `Prompt` 本轮没有继续改实现代码，主要是重新抓干净证据；live 证据确认：
     - `canvasAlpha = 1.00`
     - `TaskCardRoot.anchoredPosition = (11.900024, -12.9672003)`
     - `TaskCardRoot.sizeDelta = (328, 229.9346008)`
     - formal-face 仍守住“只允许内容向下推，不横向改壳体”的边界
  3. `Workbench` 的 clean 图额外确认：
     - `PanelRoot.sizeDelta = (428, 257.10849)`
     - `MaterialsViewport.sizeDelta = (-27.6, 41.800003)`
     - `Prompt.canvasAlpha = 0.00`
     - 说明这张最终图里看到的是纯 `Workbench` 终面，而不是 Prompt 叠层残影
- 本轮新增 accepted 证据：
  - Prompt formal-face：
    - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260329-021448-153_manual.png`
    - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260329-021448-153_manual.json`
  - Workbench 整面：
    - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260329-021738-840_manual.png`
    - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260329-021738-840_manual.json`
- 本轮验证：
  - `list_mcp_resources(server=unityMCP)` 已确认当前会话资源暴露正确
  - `scripts/check-unity-mcp-baseline.ps1` = `baseline_status: pass`
  - `manage_scene(get_active)` = `Assets/222_Prefabs/UI/Spring-day1/Primary.unity`
  - `validate_script(SpringDay1WorkbenchCraftingOverlay.cs)` = `0 error / 1 warning`
  - `validate_script(SpringDay1PromptOverlay.cs)` = `0 error / 1 warning`
  - `git diff --check` 仅剩 `CRLF/LF` 提示，无阻断错误
  - Unity 已在抓证后退回 `EditMode`
- 本轮新增运维事实：
  - `scripts/SpringUiEvidence.ps1 -Action promote-latest` 在当前 Windows PowerShell 5.1 现场仍会因 `System.IO.Path.GetRelativePath` 缺失而失败，不能再把它当成这台机器上的稳定 promote 入口
  - 本轮 accepted 侧车与 `latest.json` 已用 UTF-8 安全方式修正回正；后续若继续依赖 PowerShell promote，需要单独修脚本兼容性
- 本轮明确未做：
  - `binder / provider / 模板化`
  - `最近交互唯一提示 / 唯一 E 键仲裁 / NPC 剧情优先 / 气泡速度`
- 当前恢复点：
  - 这轮已经具备可直接回用户的最终屏证据
  - 下一步不是继续扩 UI 系统，而是等待用户按 accepted 图裁定这两张面是否过线
  - 若用户继续加刀，优先从已记入 memory 的“最近交互提示统一仲裁”单独切下一轮

## 2026-03-29 用户终验驳回补记：当前主刀改为“排版自适应与信息层级纠偏”

- 当前工作区主线没有切走，仍然只围绕 `Prompt + Workbench` 两张面继续返修；但用户已经明确驳回上一轮 accepted 图，因此当前不能再把这轮说成“已可切下一刀”。
- 用户这次驳回的关键点：
  1. `Prompt` 仍然很差劲，用户已用图明确标出：
     - 主任务区内部存在夸张空白
     - 底部提示条与主卡片关系错误
     - 右下小菱形 / 装饰不对
  2. `Workbench` 左列的省略号不是用户设计，说明左侧 recipe 行信息排版仍未收正。
  3. `Workbench` 右侧名称与描述没有贴近，信息层级不对。
  4. `Workbench` 多材料时，材料项与 `预计耗时` 挤在一起，说明纵向自适应仍未真正落地。
- 本轮新的治理判断：
  - 上一轮 accepted 图只能证明“有最终图”，不能证明“正式面和排版过线”。
  - 当前唯一主刀必须进一步收窄成：
    - `Prompt` 排版层 / 信息层级 / 底部提示区关系纠偏
    - `Workbench` 左列 recipe 行排版 + 右侧多材料纵向自适应纠偏
  - 用户已经明确要求：**这轮就把自适应真正加进去，而且仍只允许纵向下推，不允许横向改壳体。**
- 本轮新增治理产物：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\场景搭建（外包）\2026-03-29_UI线程Phase2-排版自适应与信息层级纠偏任务书.md`
- 当前恢复点：
  - 子工作区当前不能切去“最近交互提示统一仲裁”
  - 必须先按这份新任务书把 `Prompt + Workbench` 的排版、自适应、信息层级收正，再交回用户

## 2026-03-29 Phase2 排版自适应与信息层级纠偏补记：Prompt / Workbench 已补齐正式 GameView 证据，当前停给用户终验

- 当前工作区主线：继续只做 `2026-03-29_UI线程Phase2-排版自适应与信息层级纠偏任务书.md` 这一刀，不切去最近交互提示、唯一 `E` 仲裁、NPC 优先级或模板化。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-ui-evidence-capture`
  - `sunset-no-red-handoff`
- 本轮手工等价执行：
  - `sunset-startup-guard`
  - `sunset-workspace-router`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮承接上刀已有代码修正，没有再扩写新的布局逻辑；真正新增的收口动作是：
  1. 重新导入 `Assets/Editor/Story/SpringUiEvidenceMenu.cs`，让新加的 `Open Pickaxe Workbench + Capture Spring UI Evidence` 菜单真正注册进 Unity。
  2. 用新的 `Pickaxe` 证据入口抓到多材料 Workbench 正式图，并确认这次卡点已经从“布局没收好”变成“Editor 证据入口没刷新”。
  3. 重新抓并提升 3 张最终 `accepted` 图：
     - Prompt：`D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260329-115249-920_bootstrap.png`
     - Workbench 单材料：`D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260329-115405-915_workbench.png`
     - Workbench 多材料（Pickaxe）：`D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260329-114946-365_workbench-pickaxe.png`
- 本轮对用户点名问题的当前判断：
  1. `Prompt` 主任务区空白：已明显收紧；accepted 图里标题、主任务行、底部说明区重新贴近同一张卡的纵向层级。
  2. `Prompt` 底部提示条：已回正；不再像额外拼接的横条，而是回到卡片底部的一体关系。
  3. `Prompt` 右下装饰：accepted Prompt 图中不再出现用户否决的右下小菱形 / 小装饰问题。
  4. `Workbench` 左列省略号：当前最终图里 recipe 行已不再出现错误 `...` 截断。
  5. `Workbench` 名称与描述层级：右侧描述已重新贴到名称下方，不再隔出一大段空区。
  6. `Workbench` 多材料与预计耗时：`Pickaxe` accepted 图里两行材料和 `预计耗时 1.4 秒` 已彻底分层；材料列表向下撑开后，下方控件继续被自然下推。
- 这轮真正落稳的纵向自适应链：
  1. 左列：`RefreshRows() -> EnsureRecipeRowCompatibility() -> RefreshRecipeContentGeometry()`，让 row 高度按内容量增长，并继续由 `Viewport/Content` 向下堆叠。
  2. 右列：`RefreshSelection() -> RefreshMaterialsContentGeometry() -> RefreshCompatibilityLayout()`，让 `名称 -> 描述 -> 材料标题 -> 材料区 -> 预计耗时 -> 数量区 -> StageHint` 这一整串只按纵向下推，不横向改壳体。
  3. Prompt：继续守 legacy 页面内部 `SetTopKeepingHorizontal(...)` 的内容层重排，不重写卡片外壳宽高；accepted 侧车仍确认 `TaskCardRoot.sizeDelta = (328, 229.9346008)`。
- 本轮关键技术答案：
  1. 左列省略号之前的根因，不是数据缺失，而是 recipe 行兼容层没有把 prefab 手工排版转成稳定的内容高度；现在通过 `EnsureRecipeRowCompatibility()` + `RefreshRecipeContentGeometry()` 收回。
  2. 右侧名称与描述之前分太开，是因为 legacy detail shell 里多个节点还保留 prefab 的旧锚点关系，没有被统一收进 top-flow 链；现在 `RefreshCompatibilityLayout()` 先归一到 top-flow，再按文本高度逐段往下推。
  3. 材料列表之前没有把下面内容推开，是因为 `SelectedMaterials` 文本高度变化没有真实回写到 `MaterialsViewport/Content`，导致下游控件仍沿旧位置排；现在 `RefreshMaterialsContentGeometry()` 会先把文本和 content 高度同步，再由 detail 布局链统一计算下游位置。
- 本轮验证：
  - `git diff --check` 对 `SpringDay1PromptOverlay.cs` / `SpringDay1WorkbenchCraftingOverlay.cs` / `SpringUiEvidenceMenu.cs` 仅剩 Git 的 `CRLF/LF` 提示，无阻断错误
  - `validate_script`：
    - `SpringDay1PromptOverlay.cs`：`0 error / 1 warning`
    - `SpringDay1WorkbenchCraftingOverlay.cs`：`0 error / 1 warning`
    - `SpringUiEvidenceMenu.cs`：`0 error / 0 warning`
  - Unity 现场：
    - 当前已回到 `EditMode`
    - Console 当前仅见 1 条范围外旧警告：`There are no audio listeners in the scene`
- 当前阶段判断：
  - 这轮已经从“accepted 图被驳回”推进到“新的正式面证据已补齐、6 个问题都能逐项回答”的阶段；
  - 验证状态是：`线程自测 + GameView 最终证据已到位，用户尚未终验`
- 当前恢复点：
  - 下一步不再继续加刀；
  - 应直接按这轮 3 张 accepted 图和逐项对照结论，停给用户终验；
  - 只有用户继续指出具体观感问题时，才允许再回到 `Prompt + Workbench` 这一刀。

## 2026-03-29 UI-V1 全局警匪定责清扫补记：第二轮回执已补，第三轮真实 preflight 被 same-root hygiene 阻断

- 当前工作区主线没有切回实现，也没有继续扩 `Prompt / Workbench` 体验；这轮主线是按 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\2026-03-29_UI-V1_补第二轮回执并进入第三轮认领归仓与git上传_01.md` 完成 cleanup 收口。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `sunset-no-red-handoff`
- 本轮手工等价执行：
  - `sunset-startup-guard`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮实际完成：
  1. 确认并补落盘第二轮回执：`D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\2026-03-29_UI-V1_全局警匪定责清扫第二轮回执_01.md`
  2. 只基于已接受的 still-own 白名单，真实运行 stable launcher `preflight`
  3. 把第三轮结果回写到：`D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\2026-03-29_UI-V1_全局警匪定责清扫第三轮回执_01.md`
- 本轮真实 `preflight` 结论：
  - `是否允许按当前模式继续: False`
  - 第一真实 blocker 不是 git 脚本没跑，也不是代码闸门，而是 same-root hygiene：
    - `Assets/Editor/Story/DialogueDebugMenu.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
    - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
    - `.codex/threads/Sunset/spring-day1/2026-03-29_UI-V1_全局警匪定责清扫第一轮回执_01.md`
    - `.codex/threads/Sunset/spring-day1/2026-03-29_UI-V1_全局警匪定责清扫第一轮认定书_01.md`
- 本轮关键决策：
  - 不扩大 scope 去吞 `SpringDay1WorldHintBubble.cs`、手调 prefab、`Primary.unity`、`NpcWorldHintBubble.cs` 或父线程治理面；
  - 不越过 `preflight` 强跑 `sync`；
  - 当前只把结论定格为：`已补二轮回执，且第一真实阻断已钉死`
- 当前验证状态：
  - `preflight` 已真实运行
  - `sync` 未运行，因为 `preflight` 已阻断
  - 当前 own 路径：`no`
- 当前恢复点：
  - 这轮已经不是 UI 实现问题，而是 cleanup same-root hygiene 问题；
  - 下一步只有两种合法方向：
    1. 另起 cleanup 刀，专收 same-root remaining
    2. 治理位明确扩大白名单 / 改裁定
  - 在新的 cleanup 指令前，不进入新的 UI 实现或 live 验证。

## 2026-03-30 Story/UI 整根接盘补记：白名单已改成整根接盘，第一次真实 preflight 已通过

- 当前工作区主线已从“第二轮回执补落盘 + 第三轮 blocker 报实”改成新的治理裁定：按 `2026-03-30_典狱长_UI-V1_StoryUI整根接盘开工_01.md`，把 `Assets/YYY_Scripts/Story/UI` 整根连同 `Assets/222_Prefabs/UI/Spring-day1` 与 `SpringDay1UiPrefabRegistry.asset(.meta)` 一起推进到真实 `preflight -> sync`。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `sunset-no-red-handoff`
- 本轮手工等价执行：
  - `sunset-startup-guard`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮新的边界判断：
  1. `SpringDay1WorldHintBubble.cs` 不再停成 mixed 噪音，而是正式并入 `Story/UI` 接盘面。
  2. `NpcWorldHintBubble.cs(.meta)` 仍不是我的语义 own，但本轮被批准按 `carried foreign leaf` 随 `Story/UI` 整根带走。
  3. `Assets/222_Prefabs/UI/Spring-day1/Primary.unity(.meta)` 这轮已按迁移 sibling 一起处理，不再退回事故口径。
  4. `Assets/Editor/Story`、`Assets/YYY_Tests/Editor`、`Assets/YYY_Scripts/Story/Interaction`、`Assets/YYY_Scripts/Story/Managers` 和字体底座继续留在范围外。
- 本轮第一次真实 `preflight`：
  - `是否允许按当前模式继续: True`
  - `own roots remaining dirty 数量: 0`
  - `代码闸门通过: True`
- 当前恢复点：
  - `Story/UI` 整根白名单已经成型，而且第一次真实 `preflight` 已经过线；
  - 下一步只剩带上本轮回执与 memory，同白名单再做收口前确认并执行 `sync`；
  - 在 `sync` 结果出来前，不扩到 `Editor/Story`、`Tests/Editor`、`Interaction`、`Managers` 或字体底座。
