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

## 2026-03-31 Primary 迁移意图只读裁定补记：`Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 不是最终 canonical path

- 当前工作区主线不是继续做 UI，也不是继续跑 `preflight/sync`；这轮主线是按 `2026-03-30_典狱长_UI-V1_确认Primary迁移意图_01.md`，只回答 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 到底是不是当初想保留的最终 canonical path。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
- 本轮手工等价执行：
  - `sunset-startup-guard`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮最终裁定：`B｜迁移 sibling / 临时复制面`
- 直接证据：
  1. 当前线程自己在 `2026-03-29_UI-V1_全局警匪定责清扫第一轮回执_01.md` 明确写过：
     - `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 是否 scene owner：`否`
     - 当前定位：`UI evidence-only / mixed incident`
  2. `2026-03-30_UI-V1_StoryUI整根接盘开工回执_01.md` 与本工作区 memory 都只把它写成：
     - `按迁移 sibling 一起处理`
     - 不是“正式迁移成功后的 canonical path”
  3. `ProjectSettings/EditorBuildSettings.asset` 仍指向 `Assets/000_Scenes/Primary.unity`
  4. `Assets/Editor/NPCAutoRoamControllerEditor.cs` 仍硬编码旧路径
  5. `HEAD` 里旧路径与新路径 scene 的 `.meta` 当前同 GUID：`a84e2b409be801a498002965a6093c05`
- 当前关键判断：
  - 我这条 UI 线之所以在 2026-03-30 把它带进白名单，只能证明“为了整根 `Story/UI` 收口，我批准把它当 sibling 一起带走”，不能反推成“它本来就是最终 canonical `Primary`”。
  - 在 `Build Settings`、编辑器硬编码和旧路径 canonical 面都还没迁完之前，旧路径不能直接删掉提交。
- 当前恢复点：
  - 这轮语义裁定已经完成；
  - 若后续真要做这案子，应先由单独 single-writer 恢复旧 canonical path，再把新路径 scene 按 duplicate / sibling 另案处理；
  - UI-V1 / SpringUI 最多只应补“它当初为什么会出现在 UI 根旁边”的语义说明，不应直接接整张 scene 修复案。

## 2026-04-01：已把“Story 向 UI/UE 集成外包”收成可派工的 owner contract

- 当前子工作区主线不再是继续修 `Prompt / Workbench`，也不是继续讨论“我是不是全项目 UI 总包”；这轮唯一主线是按 `2026-04-01_UI线程_Story向UIUE集成owner边界收口任务书_01.md`，把后续到底该接什么、不该接什么、怎么派工，收成一份可执行 owner contract。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `preference-preflight-gate`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮是 docs-only，只读分析 + 文档落盘，不进入真实施工，因此：
  - 未跑 `Begin-Slice`
  - 无代码改动
  - 无 runtime / prefab 改动
- 本轮已完成：
  1. 新增主文档：`D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\2026-04-01_Story向UIUE集成owner边界与派工约定.md`
  2. 把 `Story / NPC / Day1` 玩家面链按代码职责拆成：
     - formal-face / overlay 壳体层
     - 玩家面提示表现层
     - 交互体 / 候选上报层
     - 会话 / 提示内容 / 玩家反馈调度层
     - 通用 UI 主系统
  3. 把后续默认岗位精确压成：
     - `Story / NPC / Day1 玩家面体验链的 UI/UE 集成 owner`
- 本轮钉实的关键判断：
  1. `SpringDay1ProximityInteractionService` 已经是当前 `Story/NPC/Day1` 链上的统一仲裁骨架；后续正确动作不是重做统一交互系统，而是在这条骨架上继续做 `contract 收口 / 玩家面 polish / 一致性修正`。
  2. 当前真正的提示主链应认定为：
     - `交互体 -> SpringDay1ProximityInteractionService -> SpringDay1WorldHintBubble + InteractionHintOverlay`
  3. `NpcWorldHintBubble` 当前应认定为：
     - `旧并行链 / carried legacy leaf`
     - 不是当前默认主链中枢
  4. 这条线默认该接的是：
     - `Story/UI` 玩家面表层与接回层
     - `SpringDay1ProximityInteractionService`
     - `CraftingStation / Bed / Day1Director` 里的玩家面体验切片
     - `NpcWorldHintBubble` 的旧链收口 contract
  5. 这条线默认不该吞的是：
     - `Assets/YYY_Scripts/UI/**` 通用 UI 主系统
     - `DialogueManager / SpringDay1Director` 的完整逻辑 owner
     - 全局输入 / `Primary` / scene hot-file
     - `NPC` 线程仍活跃时的 NPC 专项体验线
- 本轮证据层级已明确压回：
  - `代码结构 / 现有链路证据`
  - `owner / contract 推断`
  - 不把它包装成“玩家体验已过线”
- 本轮验证状态：
  - `静态推断成立`
  - 已补读 `global-preference-profile.md`，并通过 `preference-preflight-gate` helper 明确这轮只能站在 `structure / checkpoint`
- 当前恢复点：
  - `SpringUI` 子工作区现在已经具备一份可直接派工、可审回执的 owner contract；
  - 在用户过审这份 contract 前，不进入新的 UI 实现，不扩到 NPC 专项体验或全项目 UI 总包叙事；
  - 下一步如果继续，只应基于这份 contract 决定下一刀到底落在哪个模块簇。

## 2026-04-01：最近交互唯一提示 / 唯一 E 这一刀已中止并 PARKED，等待用户重裁 owner 边界

- 当前子工作区主线原本已切到：
  - `最近交互唯一提示 + 唯一 E 键仲裁 + 视觉归属一致`
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮真实发生过的施工与验证：
  1. 已跑 `Begin-Slice`，线程进入过 `ACTIVE`
  2. 已对主链仲裁切片留下代码现场，当前磁盘上仍可见的本轮相关差异为：
     - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
     - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
     - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
     - `Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`
  3. 已完成最小代码闸门：
     - `CodexCodeGuard` 对以上 5 个 C# 文件返回 `CanContinue = true`
     - 结构上已站住“主链仲裁 + NPC 闲聊向剧情让位 + 世界提示卡可读化”这一层
- 本轮新的硬裁定：
  1. 用户已明确要求立刻停手，不再继续这刀实现
  2. 当前这刀与 `spring-day1V2`、`NPC` 的 active scope 已确认撞车
  3. 因此这条切片不能继续推进、不能 `sync`，只能先退回 docs / contract / 审稿位
- 本轮已执行的现场动作：
  - 已跑 `Park-Slice`
  - 当前 `thread-state = PARKED`
  - blocker：
    - `与 NPC / spring-day1V2 active scope 撞车，等待用户重裁 owner 边界`
- 当前恢复点：
  - 在用户重新裁边界前，`SpringUI` 子工作区不继续进入“最近交互唯一提示 / 唯一 E 仲裁”实现；
  - 只保留 docs / contract / 审稿位；
  - 若后续要恢复实现，必须以用户新的 owner 裁定为前提重新起刀。

## 2026-04-01：身份 / 工作区 / owner 自审已完成，SpringUI 线不再继续把自己挂成 spring-day1V2 的影子

- 当前子工作区主线：
  - 不是继续实现，也不是继续碰“最近交互唯一提示 / 唯一 E”；
  - 只做身份 / 工作区 / exact-own 自审。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
- 本轮手工等价执行：
  - `sunset-startup-guard`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮是只读分析：
  - 未跑 `Begin-Slice`
  - 未改任何业务 / prefab / 实现文件
- 本轮重新钉死的结论：
  1. 当前最准确身份不是 `spring-day1V2` 代工位，而是：
     - `Story / NPC / Day1 玩家面体验链的 UI/UE 集成 owner`
  2. 这条线仍然是独立的 `SpringUI` 线，唯一工作区继续固定为：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI`
  3. 这轮之所以会“像 spring-day1V2 的影子”，根因不是工作区漂了，而是执行层一度误把：
     - `thread-state`
     - 当前线程记忆
     挂到了 `spring-day1V2` 这条旧线程上。
  4. 这 5 个文件里，按当前 `SpringUI` 身份继续认领的 exact file 只应保留：
     - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
  5. 其余 4 个都应释放为非当前 SpringUI 稳定 own：
     - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
     - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
     - `Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`
- file-level 自审理由：
  1. `NPCInformalChatInteractable.cs`
     - 属于 `NPC + 交互体` 实现面；
     - 这轮只是因为“唯一提示 / 唯一 E”切片误把交互体实现和 UI 集成绑在一起才短暂碰到；
     - 不应继续算 SpringUI 当前 exact own。
  2. `SpringDay1WorldHintBubble.cs`
     - 仍属于 `Story/UI` 玩家面提示表现层；
     - 与 `SpringUI` 作为玩家可见表层 owner 的身份一致；
     - 可以继续保留为 exact own。
  3. `SpringDay1DialogueProgressionTests.cs`
     - 是广义 Day1 进度 / 剧情 / 运行态验收测试；
     - 不是 SpringUI 专属测试面；
     - 当前应释放。
  4. `SpringDay1ProximityInteractionService.cs`
     - 虽然 owner contract 里曾把它列成“统一仲裁 contract 面”，
     - 但这次 active-scope 撞车已经证明：它不是 SpringUI 当前该单拿的 exact implementation file；
     - 当前应释放实现 own，只保留 contract 审稿权。
  5. `SpringDay1InteractionPromptRuntimeTests.cs`
     - 和 `SpringDay1ProximityInteractionService.cs` 强绑定；
     - 本质是“唯一提示 / 唯一 E”这刀的 slice 验证，不是稳定的 SpringUI 独立 own；
     - 当前应一并释放。
- 当前恢复点：
  - 在用户重新裁边界前，SpringUI 线继续只保留：
    - `SpringUI` 工作区
    - docs / contract / 审稿位
    - 以及 `SpringDay1WorldHintBubble` 这类真正玩家面表层文件的潜在 own 面
  - 不再继续把 `交互体 / 仲裁 service / Day1 广义测试` 自动归到自己名下。

## 2026-04-01：按独立 `UI / SpringUI` 主刀恢复一刀真实施工，已拿到玩家面近身提示链的 live 证据

- 当前子工作区主线：
  - 不再把本线混进 `spring-day1V2`；
  - 按 `2026-04-01_典狱长_UI_接管玩家面UIUE整合主刀_03.md`，只收 `Story / NPC / Day1` 玩家面体验链里“最近目标 / 唯一提示 / 唯一 E / 视觉归属一致 / NPC 剧情优先于闲聊”这一刀。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
  - `sunset-unity-validation-loop`
  - `sunset-ui-evidence-capture`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
  - `sunset-workspace-router`
- 本轮真实施工前已接入 `thread-state`：
  - 已沿用 `Begin-Slice`
  - 线程：`UI`
  - slice：`Story-NPC-Day1玩家面UIUE整合主刀`
- 本轮实际落在当前切片里的文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorldHintBubble.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\InteractionHintOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1BedInteractable.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\InteractionHintDisplaySettings.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1InteractionPromptRuntimeTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1DialogueProgressionTests.cs`
- 本轮站稳的实现结果：
  1. `SpringDay1ProximityInteractionService` 现在明确优先保留 `CanTriggerNow=true` 的候选，避免更近但还按不动的 teaser 抢走当前唯一 `E`。
  2. 世界提示与底部交互卡已经拆成两态：
     - `ready`：头顶提示保留按键与正式动作文案，底部 `InteractionHintOverlay` 显示正式卡片
     - `teaser`：头顶提示不再冒充可按 `E`，统一改成 `再靠近一些`，底部正式卡隐藏
  3. `SpringDay1WorldHintBubble` 已补玩家面所需的可读卡片化样式，并对外暴露：
     - `IsVisible`
     - `CurrentKeyLabel`
     - `CurrentCaptionText`
     - `CurrentDetailText`
     - `CurrentIsActionable`
  4. `InteractionHintOverlay` 已补正式背板、按键板、强调线与动态内容布局，不再是纯色测试条。
  5. `SpringDay1BedInteractable` 已最小接回 Day1 统一近身仲裁服务，并用 `ResolveRestInteractionDetail(...)` 做导演层提示文案兼容桥接，避免被底座方法签名漂移卡死。
- 本轮代码闸门：
  - `git diff --check` 对本轮 7 个 C# 文件通过
  - `CodexCodeGuard` 结果：
    - `OwnerThread = UI`
    - `ChangedCodeFiles = 7`
    - `AffectedAssemblies = Assembly-CSharp, Tests.Editor`
    - `ChecksRun = utf8-strict / git-diff-check / roslyn-assembly-compile`
    - `CanContinue = true`
- 本轮 live 证据：
  1. 已抓到新的 `accepted` 玩家视面证据：
     - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260401-173354-243_manual.png`
     - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260401-173354-243_manual.json`
  2. 这次图像里同时出现了：
     - 左侧 `PromptOverlay`
     - NPC 头顶 `SpringDay1WorldHintBubble`
     - 左下正式 `InteractionHintOverlay`
  3. 侧车 JSON 的 `runtimeValidationSnapshot` 已明确记录：
     - `WorldHint=001|E|交谈|按 E 开始对话|distance=0.00|priority=30|ready=True`
     - `PlayerFacing=0.0.2 首段推进链|focus=靠近 NPC 并按 E 开始首段对话。|progress=首段对话进行中|hint=001/E/交谈/按 E 开始对话/distance=0.00/priority=30/ready=True`
- 本轮未闭环项：
  1. `unityMCP run_tests` 的定向过滤这轮没有拿到可信通过结果：
     - 第一次像是过滤未命中新测试
     - 第二次被共享 Unity 现场的自动 Play 节奏拦住，报 `Cannot start a test run while the Editor is in or entering Play Mode`
  2. 因此这轮自动化验证当前只稳在：
     - `CodexCodeGuard 通过`
     - `live snapshot + 玩家视面抓图成立`
     - `Unity 定向 EditMode 测试仍待后续在干净窗口重跑`
- 当前阶段判断：
  - 这轮已经从“只有结构链”推进到“玩家面近身提示链已有 live 画面和运行态快照”的阶段；
  - 但还不能把它包装成“全部验证完毕”，因为 targeted tests 这轮被共享 Editor 节奏卡住。
- 当前恢复点：
  - 这轮不进入 `Ready-To-Sync`；
  - 下一步若继续，应优先在干净 Unity 测试窗口重跑 `SpringDay1InteractionPromptRuntimeTests` 与相关 `SpringDay1DialogueProgressionTests` 子集；
  - 如果用户先审当前效果，这轮已经具备一张可以直接讨论玩家面观感的真实证据图。

## 2026-04-01：主刀任务书收口补记，当前按 `PARKED` 等待用户基于玩家面证据继续裁定

- 当前子工作区主线：
  - 不是继续扩写 `Story / NPC / Day1` 玩家面近身提示链实现；
  - 而是按 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-01_典狱长_UI_接管玩家面UIUE整合主刀_03.md` 收口当前一刀，并用新的岗位口径正式对外回执。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮没有新增代码写入；主要完成的是：
  1. 重新补读：
     - `2026-04-01_Story向UIUE集成owner边界与派工约定.md`
     - 当前子工作区 `memory.md`
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\场景搭建（外包）\memory_0.md`
  2. 按最新主刀任务书重新确认：
     - 本线身份已经固定为 `Story / NPC / Day1 玩家面体验链的 UI/UE 集成主刀`
     - 不再把自己混成 `spring-day1V2` 的影子
  3. 因为这轮不继续扩写实现，也不进入 `sync`，已执行：
     - `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
     - 当前 `thread-state = PARKED`
- 当前线程状态与 blocker：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑；原因是这轮未准备收口到 `sync`
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
  - 第一真实 blocker：
    - `targeted-tests-blocked-by-shared-editor-playmode-window`
- 当前恢复点：
  - 这轮已经有足够让用户讨论玩家面结果的 `accepted` 图与 sidecar；
  - 若后续继续，最可信的下一步仍是先在干净 Unity 窗口补跑 targeted tests，而不是盲目扩刀。

## 2026-04-01：身份与 own 边界按新裁定回正，SpringUI 重新站到 `Story / NPC / Day1` 玩家面集成位

- 当前子工作区主线：
  - 不是继续只把自己理解成 `Spring` 小外包；
  - 而是按用户最新写死的口径，正式认领：
    - `Story / NPC / Day1 玩家面体验链的 UI/UE 集成 owner`
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮是只读自查，不进入新的真实施工：
  - 未新跑 `Begin-Slice`
  - 当前继续维持既有 `PARKED`
- 本轮按最新裁定重新钉死的 own 分层：
  1. `exact-own files`：
     - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
     - `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
     - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs` 的玩家面 contract / 焦点 / 唯一提示 / 唯一 `E` / 一致性切片
  2. `协作切片 files`：
     - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
     - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
     - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
     - `Assets/YYY_Scripts/Story/Interaction/SpringDay1BedInteractable.cs`
     - `Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`
     - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
  3. `明确释放 / 不再吞的 files`：
     - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
     - `Assets/000_Scenes/Primary.unity`
     - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
- 本轮关键判断：
  - 不能再犯“只剩 `SpringDay1WorldHintBubble.cs` 才算自己 own”的过窄错误；
  - 也不能把 NPC 底座、全局输入、`Primary` 和完整聊天体验整线吞成自己 own。
- 当前恢复点：
  - `SpringUI` 的岗位、工作区和 own 边界已经重新压实；
  - 下一轮若进入真实施工，应继续围绕“玩家真正看到的结果层整合”推进，而不是再漂回 `Spring-only` 或 `NPC 底座 owner` 叙事。

## 2026-04-02：左下角 `InteractionHintOverlay` 已接管“任务优先于闲聊”的文案仲裁

- 当前子工作区主线：
  - 只收 `InteractionHintOverlay` 的提示内容仲裁；
  - 当同一个对象当前更该走 `Spring` 正式任务语义时，左下角不再显示“闲聊”。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮真实施工前已跑：
  - `Begin-Slice`
  - slice：`左下角InteractionHintOverlay任务语义仲裁`
- 本轮实际修改：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1InteractionPromptRuntimeTests.cs`
- 本轮实现结果：
  1. `SpringDay1ProximityInteractionService` 现在会在“同一个锚点同时挂有 `NPCDialogueInteractable + NPCInformalChatInteractable`，且当前焦点文案仍是泛化的 `交谈 / 按 E 开始对话`”时，只改左下角 `InteractionHintOverlay` 的内容拷贝。
  2. 这时左下角会改成：
     - `caption = 进入任务`
     - `detail = 按 E 开始任务相关对话`
  3. 头顶 `SpringDay1WorldHintBubble` 不跟着被重写，仍保持当前世界提示链自己的文案来源。
  4. 工作台、床以及其他 `SpringDay1` 交互对象没有被卷进这刀。
- 本轮验证：
  - `git diff --check` 对本轮 2 个文件通过
  - `CodexCodeGuard` 对这 2 个 C# 文件通过：
    - `utf8-strict`
    - `git-diff-check`
    - `roslyn-assembly-compile`
  - 新增一个定向 runtime 测试，专门卡住：
    - 同 NPC 上正式任务语义压过闲聊时
    - 左下角改文案
    - 头顶 world hint 不被顺手改掉
  - 额外 Unity 定向测试尝试过一次，但插件会话在等待 `command_result` 时断开，没拿到可信 live pass
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑；本轮未进入 `sync`
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
  - blocker：
    - `unity-targeted-test-disconnected-while-awaiting-command_result`
- 当前恢复点：
  - 这刀代码与静态闸门已经站住；
  - 若后续继续，只需要在更稳定的 Unity 窗口里补一次 targeted test，不需要再扩大实现范围。

## 2026-04-02：玩家气泡正式面已回正为 NPC 同款，不再被浅色玩家预设反刷

- 当前子工作区主线：
  - 继续负责 `Story / NPC / Day1` 玩家真正看到的 UI/UE 结果层；
  - 这轮只收一个用户直接指出的 formal-face 倒退：玩家气泡样式明显跑偏，要求“和 NPC 一致就好”。
- 本轮子任务：
  1. 只修 `PlayerThoughtBubblePresenter` 的视觉预设与文本排版。
  2. 不回改 `NPCBubblePresenter`、`PlayerNpcChatSessionService`、NPC 会话状态机或别的玩家面系统。
  3. 给这条“玩家气泡 = NPC 正式面镜像”补一个最小 Editor 回归测试。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮 thread-state：
  - 沿用已存在的 `ACTIVE` slice：
    - `玩家气泡样式回正为NPC同款正式面`
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerThoughtBubblePresenterStyleTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerThoughtBubblePresenterStyleTests.cs.meta`
- 本轮实现结果：
  1. `PlayerThoughtBubblePresenter` 之前会在 `Awake / OnValidate` 里持续应用错误的浅色玩家专属预设，这是玩家气泡“看起来完全不对劲”的直接根因。
  2. 现在玩家气泡的边框、底色、阴影、文字色、描边色、最大宽度、最小宽度、单行字符数、描边宽度，已经全部对齐到 `NPCBubblePresenter` 的正式面口径。
  3. 只保留了玩家侧必要的镜像差异：
     - `bubbleLocalOffset.x` 为 NPC 的镜像值
     - `tailHorizontalBias` 为 NPC 的镜像值
  4. `FormatBubbleText(...)` 已改成和 NPC 一样的换行规则，避免玩家气泡内容密度继续和 NPC 正式面脱节。
  5. 新增 `PlayerThoughtBubblePresenterStyleTests.cs`：
     - 锁住“玩家预设应镜像 NPC 正式面”
     - 锁住“玩家文本换行应与 NPC 相同”
  6. 用户随后回报了测试编译错误；这轮已把测试改成反射取运行时类型，不再对 `PlayerThoughtBubblePresenter / NPCBubblePresenter` 做编译期强引用。
- 本轮验证：
  - `git diff --check`：通过
  - `CodexCodeGuard`：通过
    - `Assembly-CSharp`
    - `Tests.Editor`
    - `Diagnostics = []`
  - 当前验证分层：
    - `结构 / checkpoint`：已过
    - `targeted probe / 局部验证`：已过
    - `真实入口体验`：未做，这轮只站住静态与程序集级验证
- 当前边界与恢复点：
  - 这轮没有进入 Unity / MCP live 复测，因为用户此前已明确要求不要用 Unity 和 MCP。
  - 下一步如果继续，应优先让用户在真实画面里看两条链：
    - NPC 对话时的玩家气泡
    - 工具反馈复用的玩家气泡
  - 如果还要继续修，也应只在 `PlayerThoughtBubblePresenter` 内做局部数值微调，不要反向扩写 NPC 底座或提示系统。

## 2026-04-02：继续做玩家气泡回归加固，测试已覆盖到浮动、停留与收放参数

- 当前子工作区主线：
  - 不扩到别的 UI 面；
  - 继续只围着“玩家气泡正式面回正”做收口和回归加固。
- 本轮子任务：
  1. 重新登记一个极窄 slice：
     - `玩家气泡样式回正收口与回归加固`
  2. 不再改视觉实现，只增强测试覆盖，避免这条链以后又被悄悄改偏。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerThoughtBubblePresenterStyleTests.cs`
- 本轮实现结果：
  1. 没再碰 `PlayerThoughtBubblePresenter` 的业务逻辑。
  2. 把测试继续补强到以下细节参数也必须与 NPC 正式面一致：
     - `minBubbleHeight`
     - `bubbleGapAboveRenderer`
     - `visibleFloatAmplitude`
     - `visibleFloatFrequency`
     - `tailBobAmplitude`
     - `tailBobFrequency`
     - `showDuration`
     - `hideDuration`
     - `showScaleOvershoot`
  3. 这样玩家气泡现在不只是“颜色和宽度像 NPC”，连浮动、尾巴摆动和收放节奏这类次级表现参数也被一起钉住，不容易再出现“整体感觉又跑味了”。
- 本轮验证：
  - `git diff --check`：通过
  - `CodexCodeGuard`：通过
    - `Assembly-CSharp`
    - `Tests.Editor`
    - `Diagnostics = []`
- 当前恢复点：
  - 玩家气泡这条链目前已经从“修正实现”推进到“补齐回归护栏”；
  - 下一步如果继续，就不该再优先补测试，而应直接看用户在真实画面里的主观反馈。

## 2026-04-02：已修复玩家气泡测试里的 `Object` 二义性编译错误

- 当前子工作区主线：
  - 不扩实现，只修用户刚贴出的测试编译错误。
- 本轮子任务：
  1. 用户贴出：
     - `PlayerThoughtBubblePresenterStyleTests.cs(17,13)`
     - `PlayerThoughtBubblePresenterStyleTests.cs(22,13)`
     的 `CS0104`
  2. 只处理这一个测试装配问题。
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerThoughtBubblePresenterStyleTests.cs`
- 本轮实现结果：
  1. 根因是测试文件同时 `using System;` 与 `using UnityEngine;`，而 `DestroyImmediate(...)` 写成了裸 `Object`，触发了 `UnityEngine.Object` 与 `object` 的二义性。
  2. 已把这两处显式改为：
     - `UnityEngine.Object.DestroyImmediate(...)`
  3. 没有回改业务代码，也没有扩写测试结构。
- 本轮验证：
  - `git diff --check`：通过
  - `CodexCodeGuard`：通过
    - `Tests.Editor`
    - `Diagnostics = []`
- 当前恢复点：
  - 这条测试装配错误已经清掉；
  - 下一步应回到真实画面判断玩家气泡是否顺眼，而不是继续围着这条测试报错打转。

## 2026-04-02：玩家气泡这条线的未竟项已补齐，测试口径和边框真值都回正到 NPC 正式面

- 当前子工作区主线：
  - 继续收“玩家气泡要和 NPC 一致”这条未竟项；
  - 这轮不再处理测试装配，而是补齐之前真正没收完的语义和数值。
- 本轮子任务：
  1. 把 `PlayerThoughtBubblePresenterStyleTests.cs` 从“玩家气泡应与 NPC 保持明显区分”的错误方向，改回“玩家气泡应镜像 NPC 正式面”的正确方向。
  2. 把 `PlayerThoughtBubblePresenter` 里仍未对齐的边框真值与动态默认值补齐。
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerThoughtBubblePresenterStyleTests.cs`
- 本轮实现结果：
  1. 测试名和断言方向已回正：
     - 现在明确要求玩家气泡镜像 NPC 正式面
     - 不再写“应明显区分”的反向断言
  2. 测试覆盖重新加厚到：
     - 几何镜像
     - 颜色一致
     - 浮动 / 尾巴摆动 / 收放节奏一致
     - 换行规则一致
  3. `PlayerThoughtBubblePresenter` 当前已补齐到 NPC 当前正式面的边框真值：
     - `0.92, 0.79, 0.56, 1.0`
  4. 同轮把两处默认动态参数也补回正式预设：
     - `visibleFloatAmplitude = 0.004`
     - `tailBobAmplitude = 26`
- 本轮验证：
  - `git diff --check`：通过
  - `CodexCodeGuard`：通过
    - `Assembly-CSharp`
    - `Tests.Editor`
    - `Diagnostics = []`
- 当前恢复点：
  - 玩家气泡这条线现在不只是“能编过”，而是代码口径、测试口径和 NPC 正式面基线已经对齐；
  - 接下来若继续，优先应该看用户真实画面里的主观反馈，而不是再补新的静态护栏。

## 2026-04-02：玩家 / NPC 对话气泡的上下层、分隔与收尾闭环已继续收口

- 当前子工作区主线：
  - 继续收用户最新明确指出的玩家面问题：
    1. 谁在说话，谁的气泡必须在上面
    2. 玩家与 NPC 气泡必须真正分开，不能再糊成一团
    3. 玩家气泡要回到“玩家自语”的那一侧，不再被错误做成 NPC 同一张卡
    4. 这条对话气泡逻辑要有收尾闭环，不能只修一张图
- 本轮 thread-state：
  - `Begin-Slice`：沿用本轮已开启的活跃 slice
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerThoughtBubblePresenterStyleTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerNpcConversationBubbleLayoutTests.cs`
- 本轮实现结果：
  1. `PlayerNpcChatSessionService` 现在不再只是给双气泡一个固定偏移，而是显式记录当前会话焦点：
     - 玩家说话时，玩家气泡拿前景排序与更高抬升
     - NPC 说话时，NPC 气泡拿前景排序与更高抬升
     - 等待 / 自动续聊阶段继续沿用最近发言方，避免上下层来回抖
  2. 双气泡的分隔强度已明显加大：
     - 会话横向偏移基线从极小值拉回到真正能看出分开的量级
     - 还叠加了按双气泡宽高和近距离重叠压力计算的额外位移
     - 当前修法不是“内部 body 偏一点”，而是恢复成真正作用在会话气泡整体位置上的位移
  3. 两个 presenter 都补了会话排序加权：
     - `SetConversationSortBoost(...)`
     - `ClearConversationSortBoost()`
     - 因此“谁在说话谁在上面”现在不只靠位置猜，而是有显式排序保证
  4. 玩家气泡样式已从“几乎镜像 NPC”收回到玩家侧正式面：
     - 左偏移更明确
     - 尾巴方向与框体参数更像玩家自语
     - 填充色、描边和文字色重新拉回与 NPC 同风格但不再完全同坨
     - 同时保留与 NPC 正式面一致的字号和可读性节奏
  5. 收尾闭环也补了：
     - 对话结束
     - 立即取消
     - 跑开中断前清泡
     - 中断玩家句 / NPC 反应句 / 反应提示
     - 现在都会同步清掉 layout shift、sort boost 和 focus
- 本轮新增测试护栏：
  1. `PlayerThoughtBubblePresenterStyleTests.cs`
     - 继续锁住玩家气泡与 NPC 的正式面可读性基线
     - 但口径已回到“风格一致、身份分开”，不再误写成两边必须完全一样
  2. `PlayerNpcConversationBubbleLayoutTests.cs`
     - 新增锁定：
       - 玩家发言时玩家 bubble 的抬升和排序 boost 更高
       - NPC 发言时 NPC bubble 的抬升和排序 boost 更高
       - `ResetConversationBubbleVisualsImmediate()` 后 shift / sort boost / focus 会被清空
- 本轮验证：
  - `git diff --check`：通过
  - Unity batchmode 单测：未完成
    - 第一真实 blocker：项目当前已有用户打开的 Unity 实例，编辑器互斥，batchmode 直接拒绝打开同一项目
  - 临时 `csc` 试编：通过
    - 以 `NETStandard 2.1 + Unity Managed + Library/ScriptAssemblies` 为参考面
    - 覆盖了本轮改动的 3 个业务脚本和 2 个测试脚本
    - 另额外把 `NPCInformalChatInteractable.cs` 作为协作类型桥接源码纳入试编，但这轮没有改它
- 当前恢复点：
  - 这条线当前已经不只是“样式补一点”，而是把会话排布、排序优先级和收尾回收都补到了同一条逻辑链上；
  - 下一轮最值钱的不是继续扩抽象，而是拿用户真实画面确认：
    1. 现在玩家 / NPC 双气泡是否真的分开
    2. 谁在说话谁在上面是否成立
    3. 玩家气泡是否已经回到用户认可的“玩家自语”语义

## 2026-04-02：气泡线最终收尾停在“checkpoint 成立，但本轮未 sync”

- 当前主线目标：
  - 不再继续加实现，只把“玩家 / NPC 对话气泡层级、分隔与收尾闭环”这刀按最终收尾 prompt 干净结算。
- 本轮子任务：
  1. 核对 `UI.json` 的 `owned_paths / expected_sync_paths` 是否已经报真。
  2. 跑 `Ready-To-Sync`，判断这刀到底能不能按白名单安全收口。
  3. 若不能 `sync`，必须把 blocker 写死，并把 live 状态收成 `PARKED`。
- 本轮稳定结论：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\state\active-threads\UI.json` 当前已报真，own / expected sync 已包含本轮真实 10 条路径：
     - 3 个业务脚本
     - 4 个测试与 `.meta`
     - 2 个工作区 memory
     - 1 个线程 memory
  2. completion layer 这轮只能落到：
     - `结构 / checkpoint`：成立
     - `targeted probe / 局部验证`：成立或基本成立
     - `真实入口体验`：尚未验证
  3. `Ready-To-Sync` 已明确阻断，本轮不能硬 sync。
- 第一真实 blocker：
  - 当前白名单所属 own roots 里仍有未纳入本轮的 remaining dirty/untracked，同根残留至少包括：
    - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerToolFeedbackService.cs`
    - `Assets/YYY_Tests/Editor/OcclusionSystemTests.cs`
    - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
    - 以及 UI 根下未纳入本轮的额外文档 / 测试残留
  - 如果现在继续 `sync`，就会把别的旧尾账一并卷进来，这不符合“只收这刀”的边界。
- 本轮 thread-state：
  - `Begin-Slice`：已在本轮收尾 slice 前补登记并沿用
  - `Ready-To-Sync`：已跑，结果为 `BLOCKED`
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 这条气泡线作为 checkpoint 级完成物已经能诚实交接；
  - 下一次若要继续，不是再改体验，而是先处理 same-root remaining dirty 的归属 / 清理问题，之后才能重新判断是否允许 `sync`；
  - 真正的玩家视面终验仍待用户让出当前 Unity 实例后补做。

## 2026-04-03：接管 spring-day1 玩家面后，先把提示链坏点与 DayEnd 玩家面残留收成 targeted probe 级 checkpoint

- 当前主线目标：
  - 按 `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-03_UI线程_接管spring-day1全部玩家面问题并行prompt_01.md`
    接走 `spring-day1` 当前全部玩家面 `UI/UE` 残项，但不回漂到 `NPCBubblePresenter.cs`、`Primary.unity`、`GameInputManager.cs`。
- 本轮子任务：
  1. 重新审视 `Prompt / Hint / WorldHint / Workbench / DialogueUI` 当前代码现场，先分清“结构成立”和“玩家体验成立”。
  2. 先收最明确的坏点：`SpringDay1WorldHintBubble` 被改成空壳，teaser 态世界提示被错误隐藏。
  3. 额外处理一个真正玩家面尾巴：`DayEnd` 收束后低精力 warning 未清掉。
- 本轮实际落地：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
    - 恢复 teaser 态世界提示，不再因为 `CanTriggerNow == false` 就直接把头顶提示整块隐藏。
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorldHintBubble.cs`
    - 收回错误的“把卡片、按键板、标题、细节全部关掉”的写法。
    - 恢复正式卡面显示、可读尺寸、teaser/ready 两态布局和可用字体兜底。
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\InteractionHintOverlay.cs`
    - 补成和其他 Spring UI 一致的“字体必须可渲染”判定，避免资源能 load 但文字实际空白。
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\NpcWorldHintBubble.cs`
    - 同步补足字体可用性判定，防止玩家面提示进入“结构在、文本丢”的假过线。
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
    - 只切一个玩家面尾巴口：`HandleSleep()` 里在 `DayEnd` 收束时明确清掉低精力 warning，避免夜间结束后还残留红警示。
- 本轮关键判断：
  1. `SpringDay1WorldHintBubble` 当前不是“还可以调一调”的状态，而是功能上已经把正式提示面阉掉了；这必须先收。
  2. 当前更适合先收“玩家能直接看见的提示链断面”，而不是继续回到模板化或抽象层。
  3. `DayEnd` 的低精力 warning 残留虽然落在导演层方法里，但它本质是玩家面泄漏，所以允许用最小切口带走，不扩成导演层重做。
- 本轮验证：
  - Unity EditMode：
    - `SpringDay1InteractionPromptRuntimeTests`：`5/5 Passed`
    - `SpringDay1LateDayRuntimeTests`：`4/4 Passed`
  - Unity Console：
    - `error`：`0`
  - 真实入口体验：
    - 本轮未补 live GameView 证据
    - 原因不是“忘了做”，而是当前项目现场仍有更高风险 live 干扰：
      - `Primary.unity` 仍有用户独占锁
      - 当前编辑器主 scene 现场不是这条线应接管的安全入口
- completion layer：
  - `结构 / checkpoint`：成立
  - `targeted probe / 局部验证`：成立
  - `真实入口体验`：尚未验证，不能假装已过线
- 本轮 thread-state：
  - `Begin-Slice`：已在本轮开工前补登记并沿用
  - `Ready-To-Sync`：未跑
    - 原因：本轮没有准备做白名单 sync，也还没有把这条线所有 own-root 旧脏改重新梳干净
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 这轮已经把 spring-day1 玩家面里最明显的提示链坏点收回到可读正式面，并把一个晚间收束残留一起清掉；
  - 下一轮如果继续，应优先补“真实入口体验”层，而不是把当前 targeted probe 继续包装成全线过线。

## 2026-04-03：只读彻查 spring-day1 任务列表与 UI 提示缺字现状，明确区分“源头没给 / 显示链隐藏 / 文案过泛”

- 当前主线目标：
  - 用户要求我不要直接实现，而是先把 `spring-day1` 当前任务列表、左下角提示、头顶提示、工作台提示、睡觉提示的所有状态彻底查清，尤其要回答“到底漏了多少文字没填”和“UI 自己哪里还会吞字”。
- 本轮子任务：
  1. 下钻 `SpringDay1Director -> SpringDay1PromptOverlay` 的任务卡文案来源与显示链。
  2. 下钻 `NPCDialogueInteractable / NPCInformalChatInteractable / CraftingStationInteractable / SpringDay1BedInteractable -> SpringDay1ProximityInteractionService -> SpringDay1WorldHintBubble / InteractionHintOverlay` 的提示来源。
  3. 额外检查序列化默认值与 UI 文本组件设置，确认哪些是源头空、哪些是泛化、哪些是显示策略导致的潜在截断。
- 本轮稳定结论：
  1. `PromptOverlay` 现阶段的主要问题不是“字段空了”，而是“任务列表模型只建了当前这一条”。
     - `BuildPromptCardModel()` 的 `StageLabel / Subtitle / FocusText / FooterText` 在主要 `StoryPhase` 分支里基本都有值；
     - 真正的问题在 `BuildPromptItems()`：
       - `CrashAndMeet` 到 `DayEnd` 每个阶段都只返回 `1` 条任务；
       - `FarmingTutorial` 不是并列保留 `开垦 / 播种 / 浇水 / 木材 / 制作` 五条链，而是只显示当前目标那一条；
       - 因此如果按用户期望的“教学链完整挂在卡面上”理解，农田阶段同屏最多缺 `4` 条，不是 UI 吞掉，而是模型根本没建出来。
  2. `PromptOverlay` 自己还会主动隐藏空块，但当前 Day1 主链里真正被它隐藏的不是主文案，而是空 detail / 空 footer / 空 focus 的块。
     - `row.detail` 为空时该 detail 节点直接隐藏；
     - `FocusRibbon` 在 `FocusText` 为空时隐藏；
     - `FooterRoot` 在 `FooterText` 为空时隐藏；
     - 当前主流程 phase 分支里，真正长期为空的不是这些字段，而是 `manual prompt` 模式下的 `FooterText`。
  3. `PromptOverlay` 还存在一个结构性状态问题：`manualPromptText` 是粘性的。
     - `SpringDay1Director` 和 `WorkbenchOverlay` 会多次调用 `SpringDay1PromptOverlay.Instance.Show(...)`；
     - `BuildCurrentViewState()` 会用 `_manualPromptText` 覆盖 `model.FocusText`；
     - 这个手动提示不会自动按 phase 清空，只会被下一次 `Show` 覆盖或显式 `Hide()` 清掉；
     - 这意味着某些“桥接提醒 / 低精力提醒 / 工作台提醒”可能长期占据焦点条，造成“任务卡不是没字，而是焦点层被旧提醒顶掉”的感知。
  4. 交互提示源头真正“明确空着没填”的，目前最确定的是工作台普通态 detail。
     - `CraftingStationInteractable.ReportWorkbenchProximityInteraction()`：
       - tutorial 首次提示：`caption = 工作台`，`detail = 按 E 打开`
       - 普通工作台提示：`caption = bubbleCaption`，`detail = string.Empty`
     - 也就是说，工作台普通近身提示没有 detail 不是显示 bug，而是代码就是故意空着。
  5. 大量提示不是空，而是“过度泛化，没有 phase-sensitive 文案”。
     - `NPCDialogueInteractable`：caption 走 `bubbleCaption`，detail 固定 `按 E 开始对话`；
     - `NPCInformalChatInteractable`：
       - 未进入会话时：`闲聊 / 按 E 开口`
       - 进入会话后才会切成 `你在说话 / 对方在想 / 对方在回你 / 对话还在继续 / 聊到一半 / 聊完了` 等固定状态语；
     - `Primary` / `Assets/222_Prefabs/NPC/*.prefab` 里当前能看到的闲聊 NPC prefab（`101 / 102 / 103 / 104 / 201 / 202 / 203 / 301`）序列化默认值全部还是：
       - `interactionHint = 闲聊`
       - `bubbleCaption = 闲聊`
       - `bubbleDetail = 按 E 开口`
     - 这说明很多 NPC 玩家面提示并不是“没显示”，而是源头目前只有通用占位语义。
  6. `Bed` 提示是少数真正做了阶段化细分的提示链。
     - 只有 `FreeTime` 可交互；
     - caption / detail 会按 `NightWarning / AfterMidnight / FinalCall` 三档改写；
     - 这条链不是缺字重点，反而是当前少数语义比较完整的链。
  7. 左下角 `InteractionHintOverlay` 和头顶 `SpringDay1WorldHintBubble` 在显示能力上并不对称。
     - `InteractionHintOverlay`：
       - `captionText` 和 `detailText` 都是 `NoWrap + Ellipsis`
       - 也就是左下角 detail 只能单行省略，不具备完整显示长句的能力
       - 当前闲聊会话态 detail 像“会自动继续，按 E 只跳过这句动效”“对方会自动接话，按 E 跳过等待”这一类句子，结构上就有被截断风险
     - `SpringDay1WorldHintBubble`：
       - `caption` 是 `NoWrap + Ellipsis`
       - `detail` 是 `Normal + Overflow`
       - 所以头顶提示对长 detail 更宽容，但长 caption 仍可能省略
  8. `WorkbenchOverlay` 本身也还有明确的“文字显示策略问题”，不只是数据问题。
     - 左列 recipe 名称 `Name` 被强制 `NoWrap + Ellipsis`，长配方名天然会省略；
     - 右侧 `SelectedDescription` 在兼容布局里被 `Mathf.Min(..., 60f)` 硬上限截住，描述过长时不是自适应无限下推，而是会被高度上限卡住；
     - `SelectedMaterials` 已改成可换行 + 可滚动，不属于源头缺字，但仍受视口高度分配影响；
     - `ProgressLabel / StageHint` 目前是内容驱动测高，不是主要缺字点。
- 当前证据层级：
  - 只做到 `结构 / checkpoint` + `targeted probe / 局部验证`
  - 本轮没有进入 Unity live，也没有用截图冒充真实体验
- 当前恢复点：
  - 现在已经能把“缺字”分成三类：
    1. 源头真空：例如工作台普通态 detail
    2. 源头没建模：例如 `Prompt` 教学链只建当前 1 条任务
    3. 源头有值但 UI 有截断/隐藏风险：例如左下角 overlay detail 单行省略、工作台左列名称省略、描述高度硬上限
  - 如果下一轮继续，最值钱的不是盲修样式，而是先按这三类逐项收：先补任务模型，再收提示文案，再收显示链。

## 2026-04-03：按“缺字是主矛盾”重新落地第一轮玩家面修正，先把 runtime 壳体、NPC 头顶提示兜底与工作台边界判距拉回正确方向

- 当前主线目标：
  - 用户已把主矛盾重新说死为：
    1. 左侧任务列表和 Prompt 卡面出现“有壳没字”
    2. NPC 不应再有头顶交互提示，统一收回左下角
    3. 工作台交互距离和提示范围应按不规则可见边界来判
    4. 闲聊期间谁在说话，谁的气泡必须稳定在上层
- 本轮子任务：
  1. 先把 `SpringDay1PromptOverlay / SpringDay1WorkbenchCraftingOverlay` 的 runtime 创建链改成“只复用真正可用的 screen-overlay 运行壳”，不再被场景里旧的 world-space 壳体截胡。
  2. 修 `CraftingStationInteractable.GetClosestInteractionPoint()`，让 collider 包络线、sprite 可见边缘和 fallback 碰撞点一起参与“最近点”竞赛，而不是 collider 优先短路。
  3. 在 `SpringDay1ProximityInteractionService` 增加 NPC 头顶提示兜底禁令：即便上报链误传了 `showWorldIndicator=true`，NPC 也只能走左下角。
  4. 让左下角 `InteractionHintOverlay` 至少支持多行正文，不再默认单行省略吃字。
  5. 给 `PlayerThoughtBubblePresenter / PlayerNpcChatSessionService` 补“玩家当前发言时的前景排序 boost”，把“谁在说话谁在上面”从仅靠轻量 sort boost，补成显式前景焦点。
- 本轮实际改动：
  - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
    - `EnsureRuntime()` 现在先筛“可复用的 screen-overlay 实例”，否则清掉 stale static 单例后再实例化 prefab。
    - 对绑定成功的壳体统一执行 `ApplyRuntimeCanvasDefaults()`，强制回正成 `ScreenSpaceOverlay / worldCamera=null / 正确 sortingOrder / 全屏根锚点`。
  - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - 同样补了 stale static 清理、screen-overlay 复用筛选和运行态 Canvas 归一化，避免旧 world-space 工作台壳体继续截胡。
  - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
    - `GetClosestInteractionPoint()` 现在不再 collider 优先返回，而是比较：
      - collider 外轮廓点
      - sprite 可见边缘点
      - collider `ClosestPoint`
      最终取离玩家最近的那个。
  - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
    - 新增 NPC world-indicator 兜底裁决，NPC 交互候选统一不再上头顶。
  - `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - `captionText / detailText` 改为多行可扩展，不再默认 `NoWrap + Ellipsis`。
  - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
    - 新增玩家侧 `speakerForegroundSortBoost`，支持玩家说话时显式拿前景排序。
  - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - 玩家开始说话时显式拿前景焦点；NPC 开始说话时清掉玩家前景焦点；所有结束/中断/重置路径同步清理玩家前景焦点。
- 本轮新增测试护栏：
  - `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
    - 新增 `PromptOverlay / WorkbenchOverlay` runtime canvas 必须是 `ScreenSpaceOverlay` 的断言。
    - 新增 stale world-space static instance 必须被 runtime 真实例替换的断言。
    - 新增 PromptOverlay 前台必须实际渲染出非空任务标题的断言。
  - `Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`
    - 新增 NPC 候选即便误上报 world indicator，也必须只走左下角的断言。
    - 新增工作台最近点应优先命中最近可见边缘，而不是偏上 collider 包络的断言。
- 本轮验证：
  - `git diff --check -- <本轮 9 个改动文件>`：通过
  - Unity batch EditMode 指定测试：未能执行
    - 第一真实 blocker：项目当前已被另一实例打开，batchmode 被 `HandleProjectAlreadyOpenInAnotherInstance` 直接拦截，未生成 test result xml
  - 当前证据层：
    - `结构 / checkpoint`：成立
    - `targeted probe / 局部验证`：测试已补，但这轮没跑通 Unity batch
    - `真实入口体验`：仍未验证
- 本轮 thread-state：
  - 沿用既有 `UI` 活跃 slice 继续施工
  - `Ready-To-Sync`：未跑
    - 原因：这轮没有准备做白名单 sync，且仓库仍有大量非本刀 dirty
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 现在最该先让用户复看的是：
    1. Prompt 卡面是否终于回到“有字可读”
    2. NPC 头顶是否已经不再冒交互提示
    3. 工作台可交互范围是否明显变得更贴近可见边缘
    4. 闲聊时当前发言方是否稳定压到上层
  - 如果下一轮继续，我最确信的下一步是：
    - 补真实 GameView 证据
    - 再按玩家反馈继续收 Prompt 缺字和工作台矩阵剩余边界

## 2026-04-03：只读勘察工作台范围 / 上下翻转 / 气泡遮挡与说话者置顶链，确认这轮主矛盾更像“阈值与排序优先级错位”，不是“代码完全没做”

- 当前主线目标：
  - 用户要求我不要实现，只读勘察 `spring-day1` 当前 5 条关键玩家面链路，聚焦：
    1. 工作台交互范围 / 提示范围
    2. Workbench UI 上下翻转
    3. NPC / 玩家气泡遮挡
    4. 谁说话谁在上面
- 本轮子任务：
  1. 复核 `CraftingStationInteractable.cs` 的最近点、距离阈值、普通态提示文案和上下翻转判据。
  2. 复核 `SpringDay1WorkbenchCraftingOverlay.cs` 的上下翻转调用时机与悬浮进度显示条件。
  3. 复核 `NPCBubblePresenter.cs`、`PlayerThoughtBubblePresenter.cs`、`PlayerNpcChatSessionService.cs` 的渲染层级、说话者前景加权和清理链。
- 本轮稳定结论：
  1. 工作台距离问题不像“最近点算法没做”，而更像“最近点已做，但阈值太紧且被常量强制写死”：
     - `GetClosestInteractionPoint()` 已同时比较 collider 包络、visual edge 和 `ClosestPoint`
     - 但 `Day1WorkbenchInteractionDistance = 0.95f`、`Day1WorkbenchHintRevealDistance = 1.65f`
     - 且 `ApplyDay1WorkbenchTuningIfNeeded()` 会在运行时重写交互与提示距离
  2. 工作台普通态提示没 detail 不是 UI 吞字，而是源头直接传了空串。
  3. Workbench 上下翻转逻辑不是缺失：
     - `LateUpdate()` 可见态每帧都会重算
     - 真问题更像 `ShouldDisplayOverlayBelow()` 的判据过度依赖 `visual bounds center`
     - 只有 `0.04f` 极小死区才退回最近交互点，因此对不规则工作台容易翻错或翻得不稳
  4. 气泡遮挡的首嫌仍是两边都走 `WorldSpace + overrideSorting`，没有进入真正独立于场景精灵竞争的玩家面层。
  5. “谁说话谁在上面”链路并非完全没做，但当前优先级设计仍有两个风险：
     - conversation/speaker boost 只是加在 `targetRenderer.sortingOrder` 基础上的小增量，未必压得过稳定排序偏差
     - 玩家即时句 `ShowInterruptPlayerLine()` 只切了 focus 和 layout，没有显式补 `SetSpeakerForegroundFocus()`
  6. 悬浮小进度这条链本身已经符合“工作台大 UI 关闭时才显示”，不是这轮首嫌。
- 当前证据层：
  - `结构 / checkpoint`：成立
  - `targeted probe / 局部验证`：成立
  - `真实入口体验`：未验证
- 本轮验证：
  - 纯只读代码链排查
  - 未跑 `Begin-Slice`
    - 原因：本轮没有进入真实施工
- 当前恢复点：
  - 如果下一轮继续实现，最稳的顺序应是：
    1. 先收工作台阈值与普通态 detail
    2. 再收上下翻转判据稳定性
    3. 再决定气泡是继续堆排序，还是提升到更硬的前景层方案

## 2026-04-03：只读勘察 Prompt/任务列表“有壳没字”，当前最像运行时复用链过宽而不是导演层没给数据

- 当前子工作区主线：
  - 用户要求暂停实现，只读彻查 `spring-day1` 的 `Prompt / 任务列表` 为什么会出现“有壳没字”或部分任务文字不显示；重点限定在 `SpringDay1PromptOverlay.cs`、`SpringDay1Director.cs`、必要测试和少量相关 UI 工具代码。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
  - `sunset-workspace-router`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮只读检查结果：
  1. `SpringDay1Director.BuildPromptCardModel()` 和 `BuildFarmingTutorialPromptItems()` 本身能产出非空正式模型；尤其 `FarmingTutorial` 会明确构建 5 条非空任务，不像是“导演层压根没字”。
  2. 当前最可疑的第一根因是 `PromptOverlay` 的 runtime 复用判定过宽：
     - `CanReuseRuntimeInstance()` 只检查 `Canvas / CanvasGroup / TaskCardRoot`；
     - 但 `TryUseExistingRuntimeInstance() -> EnsureBuilt() -> TryBindRuntimeShell()` 后续实际还要求 `Page / TitleText / SubtitleText / FocusText / FooterText / TaskList` 全链完整；
     - 这意味着半残 screen-overlay 实例会先被接纳，再在同一 root 上 fallback `BuildUi()`，有机会留下“旧壳 + 新壳”或“壳体存在但文本链不完整”的现场。
  3. 第二根因是 prefab/scene 壳绑定规则过脆：
     - `BindExistingRows()` 只认 `TaskRow_` 前缀；
     - `BindRow()` 强依赖 `Image + BulletFill + Label + Detail` 全齐；
     - 一旦 live 实例不是正式 prefab 原样，任务行就可能不被真正接管。
  4. 第三根因是 prefab-first 仍可能被字体可用性闸门整块打回 `BuildUi()`：
     - `CanInstantiateRuntimePrefab()` 只要发现任意 `TMP` 节点字体“不好用”，就拒绝整个 prefab；
     - 测试目前证明的是“最后屏上至少有一个非空 Label”，还没证明运行时一定真的走了 formal-face prefab 主链。
  5. `manual prompt` 的真实影响更偏向“焦点条被旧提示顶住”，不是任务列表整块空白的第一根因；它应放在次级问题，而不是主矛盾。
- 本轮新增稳定判断：
  - 这次 `Prompt / 任务列表` 的“有壳没字”，最像：
    1. 正式任务模型是有的
    2. 但 runtime 可能先复用了不完整壳体
    3. 或 prefab 被字体闸门拒掉后回退到了代码壳
    4. 最终玩家看到的是“壳还在，文本链没真接上”
- 当前恢复点：
  - 下一轮如果进入实现，最值钱的顺序应是：
    1. 收紧 `CanReuseRuntimeInstance()`
    2. 拒绝半壳并在 fallback 前清理旧壳
    3. 放宽 `BindRow()/EnsureRows()` 的容错
    4. 最后再处理 `manual prompt` 覆盖范围

## 2026-04-03：重新进入真实施工，先收 Prompt/Workbench runtime 真源链，并压回本轮新增测试红错

- 当前主线目标：
  - 用户要求回到 `spring-day1` 玩家面主线，先确保红错消失，再继续收 `Prompt/任务列表缺字`、`工作台提示链` 与 `玩家/NPC 气泡层级`。
- 本轮子任务：
  1. 继续把 `SpringDay1PromptOverlay`、`SpringDay1WorkbenchCraftingOverlay` 从“错误旧壳 / 旧 runtime 复用”收回到 `prefab-first` 真源链。
  2. 先压回我本轮自己引入的测试编译红错，避免 shared root 留红。
- 本轮真实施工已完成：
  1. `SpringDay1PromptOverlay.cs`
     - 去掉了会捡回不兼容旧实例的回退路径；
     - `EnsureRuntime()` 现在会退休不兼容或重复实例，尽量只保留真正的 screen-overlay runtime 实例。
  2. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 同步收紧 runtime 复用与实例选择；
     - 避免错误 world-space/旧链路实例继续截胡 Workbench 正式面。
  3. `SpringDay1LateDayRuntimeTests.cs`
     - 去掉了测试文件对 `TMPro` 命名空间和 `TextMeshProUGUI` 强类型的直接依赖；
     - 改成 `Component + 反射取 text`，从源码层消除 `CS0246: TMPro` 这条红错根因。
  4. `SpringDay1InteractionPromptRuntimeTests.cs`
     - 给 `CraftingStationInteractable_PrefersNearestVisualEdgeOverFarColliderEnvelope()` 补了 `yield break;`，压回 `CS0161 not all code paths return a value`。
- 本轮关键判断：
  1. `Prompt/任务列表有壳没字` 的主嫌仍更像 runtime 真源链和旧壳复用，而不是导演层完全没给任务数据。
  2. 这轮先清测试红错是必须动作，因为用户当前明确要求“先确保红错消失”，而且这类 owned error 不能继续挂在现场。
  3. 当前还不能声称“体验过线”：
     - 这轮只站到源码链收紧和编译面自清红；
     - 还没有新的 Unity Console / GameView 证据。
- 本轮验证状态：
  - `静态推断成立`
  - `git diff --check`：通过
  - Unity 现场重新编译：尚未重新取证
- 当前恢复点：
  - 下一步继续玩家面主线时，优先顺序仍应是：
    1. Prompt / 任务列表缺字
    2. 工作台距离 / 提示范围 / 普通态 detail / 上下翻转
    3. NPC 头顶提示彻底左下角化
    4. 玩家 / NPC 气泡遮挡与“谁说话谁在上面”

## 2026-04-03：补收 Workbench 内容层排版与 Prompt row 链自愈，当前 owned 脚本已回到 0 error

- 当前子工作区主线：
  - 继续 `spring-day1` 玩家面 UI/UE 收口，但这轮只补最值钱的两处内容层问题：
    1. `Workbench` 左列与右侧详情的旧排版硬伤
    2. `PromptOverlay` “有壳但 row 链坏掉”时的误复用与空字自愈
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮手工等价执行：
  - `sunset-startup-guard`
  - `sunset-workspace-router`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮真实施工：
  1. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - 左列 `RecipeRow` 名称不再强制 `NoWrap + Ellipsis`，改为可换行，避免长配方名继续被错误省略号截掉。
     - 右侧 `SelectedName` 也改为可换行。
     - `RefreshCompatibilityLayout()` 不再把 `SelectedDescription` 用固定 `60f` 硬截断；现在会在不改壳体宽度和左右比例的前提下，按底部动作区剩余预算给描述腾高度，再把材料区、进度区和提示区整体向下推。
  2. `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
     - `CanBindPageRoot()` 现已从“只要有 `TaskList` 就算壳可复用”收紧为“至少存在一条可绑定的 `TaskRow_/BulletFill/Label/Detail` 链”。
     - `ApplyStateToPage()` 现在在写完前台页后会校验 `page.rows[i].label/detail` 是否真的匹配当前 `PromptCardViewState`；如果仍是空壳或错链，会当场重建 row 链并重刷，不再继续带着坏页显示。
- 本轮验证：
  - `validate_script`
    - `SpringDay1WorkbenchCraftingOverlay.cs`：`0 error / 1 warning`
    - `SpringDay1PromptOverlay.cs`：`0 error / 1 warning`
    - `CraftingStationInteractable.cs`：`0 error / 1 warning`
    - `SpringDay1Director.cs`：`0 error / 2 warning`
    - `SpringDay1InteractionPromptRuntimeTests.cs`：`0 error / 0 warning`
    - `SpringDay1LateDayRuntimeTests.cs`：`0 error / 0 warning`
  - `git diff --check` 对本轮 owned 代码未报阻断错误；仅有 Git 的 CRLF/LF 提示。
  - Unity Console 最新读取中仍有 error，但都落在 `PersistentManagers.cs / TreeController.cs` 的旧现场，不指向本轮 UI own 文件。
  - `run_tests` 两次都只返回 `total=0`，说明当前 MCP 这条测试调用没有真正命中目标用例，不能拿来当有效通过证据。
- 本轮判断：
  1. `Workbench` 这轮最值钱的修法不是再改壳，而是把“长名字不再错误省略、长描述不再被 60f 砍死”收回内容层。
  2. `Prompt` 这轮最值钱的修法不是再改导演层任务数据，而是把“半残壳误复用”卡死，并给前台 row 链补自愈。
  3. 当前只能站到：
     - `结构 / checkpoint`
     - `targeted probe / 局部验证`
     还不能写成 `真实入口体验已过线`。
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑，原因：这轮没有准备白名单 sync
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 下一轮如果继续真实施工，优先级仍应是：
    1. 真实入口下复核 `Prompt` 任务行是否已经彻底脱离“有壳没字”
    2. 继续收工作台剩余的 live 交互矩阵
    3. 再回到 NPC 提示与气泡层级的真实体验层验证

## 2026-04-04：继续补收 Workbench 静止锚定/翻面弹性/当前配方回显，并把 Prompt 前台 row 护栏压进测试

- 当前子工作区主线：
  - 继续按用户那 8 条收 `spring-day1` 玩家面，且这轮明确把“不爆红”放到第一优先级，只在 `Prompt / Workbench / 必要测试` 内继续。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮手工等价执行：
  - `sunset-startup-guard`
  - `sunset-workspace-router`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮真实施工：
  1. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - `Reposition()` 现已改成：
       - 常态直接贴锚点，不再持续 `Lerp` 造成“工作台和 UI 相对漂移”
       - 只有上下翻面时，才做竖向 `SmoothDamp` 弹性过渡
     - `RepositionFloatingProgress()` 现已改为直接贴锚点，不再平滑跟随，避免小悬浮框继续晃。
     - `Open()` 现在在已有制作队列时会优先选中当前正在制作的配方，重新打开工作台就能直接看到当前单件进度、剩余数量，并继续追加。
     - `CanReuseRuntimeInstance()` 再收紧一层：recipe 壳如果存在 `RecipeRow_` 但行项文本链不完整，就不再继续复用。
     - 悬浮小图标从 `24x24` 调到 `28x28`，仍保持 `45°`。
  2. `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
     - 保持上轮的 row 链自愈逻辑，并继续作为 `Prompt` 首要根因修正面。
  3. `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
     - `PromptOverlay_RuntimeCanvas_ShouldBeScreenOverlayAndRenderFilledTaskTexts` 不再扫全树任意 `Label`，而是直接断言前台 `_frontPage.rows[0]` 的 `label/detail` 非空。
     - 新增 `WorkbenchOverlay_ShouldReplaceIncompleteRecipeShellStaticInstance`，防止“左列 recipe 行文本链坏掉的 screen-overlay 壳”被继续复用。
- 本轮验证：
  - `validate_script`
    - `SpringDay1PromptOverlay.cs`：`0 error / 1 warning`
    - `SpringDay1WorkbenchCraftingOverlay.cs`：`0 error / 1 warning`
    - `CraftingStationInteractable.cs`：`0 error / 1 warning`
    - `SpringDay1Director.cs`：`0 error / 2 warning`
    - `SpringDay1InteractionPromptRuntimeTests.cs`：`0 error / 0 warning`
    - `SpringDay1LateDayRuntimeTests.cs`：`0 error / 0 warning`
  - `git diff --check` 对本轮 owned 文件未报阻断错误，仅剩 CRLF/LF 提示。
  - Unity Console 最新读取：`0 error`
  - `run_tests` 依然只返回 `total=0`，仍不能视作有效测试通过证据。
- 本轮判断：
  1. 这轮最核心的收口是把你第 7、8 条里最明显的偏差对齐了：
     - UI / 小悬浮常态不再漂
     - 上下翻面保留弹性，不再靠常态跟随假装“动画感”
  2. `Workbench` 左列空白这条风险现在不只在 `EnsureBuilt()` 兜底，连 runtime 可复用壳判定也一起收紧了。
  3. 当前仍然只能站到：
     - `结构 / checkpoint`
     - `targeted probe / 局部验证`
     还没拿到真实入口体验层证据。
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑，原因：这轮没有进入 sync 收口
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 下一轮如果继续，最值钱的顺序仍应是：
    1. 真实入口复核 `Prompt` 首屏任务栏与前台 row
    2. 继续收 `Workbench` 交互矩阵剩余边界
    3. 再回到 NPC 提示与气泡的真实体验层

## 2026-04-04：spring-day1 玩家面本轮最新用户需求原文整理与施工拆解

- 当前主线目标：
  - 继续完成 `spring-day1` 玩家面体验链里所有仍未落地的 `Prompt / Dialogue / Workbench / 左下角提示 / 悬浮框 / 制作交互闭环`，并且把“不要爆红”当成本轮最高硬约束。
- 用户重新钉死的原始 8 点需求：
  1. 游戏刚开始运行时，左边任务栏就应该有内容，不能空。
  2. 任务显示仍要维持“一次只显示一个当前主任务/当前小任务”，内容超出时要做纵向自适应或滚动，不能直接溢出。
  3. 工作台提示不能退化成只剩左边一个 `E`，中间提示内容必须完整且专业。
  4. 工作台左侧 recipe 栏不能再出现“有点击区但无文字”的老问题。
  5. 工作台制作数量从 `0` 开始，`1` 就意味着单件制作已经开始推进；大 UI 和悬浮框都要按“单件进度条 + 队列剩余数量”来表现，并支持继续追加制作。
  6. 如果背包里已经有 `3` 个及以上木头，木材任务应自动判完成，不能等玩家再去获取一次。
  7. 工作台一旦激活，就应始终允许制作；`E` 是工作台 UI 的 toggle；有制作内容时，大 UI 和悬浮框要按打开/关闭切换；大 UI 和小框都必须和工作台相对静止。
  8. 工作台 UI 的上下切换不是瞬间跳，而是保留弹性移动感。
- 用户在原 8 点之外，再次明确的已测问题：
  1. 任务列表仍有部分文字缺失。
  2. 村长对话框右下角“继续对话”的提示文字仍会空白。
  3. 工作台左侧栏目依旧可能空白但可点击。
  4. `先选择数量再开始制作` 这类文案不需要，进度状态本身就应承担提示职责。
  5. 进度与剩余数量需要常驻显示，而不是只在 hover 时才出现。
  6. `正在打造……当前单件……` 这类 verbose 文案要删，给材料区腾空间。
  7. 物品名称与简介要整体上移，但绝不能压到图标框以下并造成重叠。
- 用户追加的制作条语义：
  1. 制作条本身就是单件制作的增长条，背景条一直存在，进度随当前单件推进而增长。
  2. 正在制作时：
     - 若当前选择数量为 `0`，鼠标移入制作条/按钮区域表示“中断制作”。
     - 若当前选择数量大于 `0`，鼠标移入表示“追加制作”。
  3. 取消逻辑必须闭环：
     - 取消时材料要返还。
     - 若背包空间不够，溢出的返还材料要掉到背包外。
- 用户追加的悬浮框样式语义：
  1. 悬浮框里物品图片要尽量占满主区域。
  2. 下方保留带背景色的进度条，进度色不变。
  3. 进度条中间的文案统一改成：
     - 制作中且已有产出时：`进度  已完成数/总队列数`
     - 全部完成且未全部取走时：`制作完成 N个`
  4. 制作完成态的进度条颜色要切到偏黄色的完成样式。
  5. 如果未来存在多个不同物品并行制作，悬浮框期望是 `一行 3 个，最多 2 行`；相同物品追加时叠数量，不重复开新框。
- 用户本轮新增并钉死的工作台交互规范：
  1. 工作台头顶提示 UI 不再显示，只保留左下角提示卡。
  2. 工作台始终允许打开，无论制作中还是已经有完成产物都可以 `E` 打开大 UI。
  3. 但“拾取已完成产物”不走 `E` 键，不走世界提示，而是只允许点击工作台大 UI 内的制作条。
  4. 制作条现在需要承担 3 组状态：
     - `制作完成态`：
       - 常态就是黄色完成样式。
       - 点击制作条用于拾取全部已完成内容。
     - `制作中且已有可取产物`：
       - 常态显示进行中的单件进度条。
       - 鼠标移入制作条时，切到黄色拾取态，隐藏当前进度表现，提示“点击拿取已完成产物”。
       - 如果玩家把当前已完成产物拿完，但队列还在做后续内容，鼠标移出后恢复到进行中进度条。
     - `制作中但当前无可取产物`：
       - 鼠标移入制作条时，切到红色中断态，隐藏进行中进度表现。
       - 点击制作条执行中断。
       - 鼠标移出后，如果仍有制作进度，就恢复到进行中样式。
- 我对这些需求的当前拆解：
  1. `Prompt / Dialogue 缺字链`
     - 要解决的是：任务栏首屏空壳、前台 row 缺字、manual prompt 仍需保留一条可读任务行、continue 文案稳定中文可见。
  2. `Workbench 左列显示链`
     - 要解决的是：prefab/runtime 壳混用导致的 row 绑定失效、可点击但无字、以及坏壳误复用。
  3. `Workbench 正式面布局`
     - 要解决的是：名称/简介/材料/数量/制作条的空间重新分配，只允许纵向下推，不允许横向改壳。
  4. `Workbench 制作状态机`
     - 要解决的是：未制作、制作中、制作中可取产物、完成待领取、追加、中断、取消返还、背包溢出掉落。
  5. `Workbench 悬浮框`
     - 要解决的是：图标占比、进度条文字、完成态配色、制作中与可领取态切换，以及将来多卡并列的可扩展性。
  6. `左下角提示与世界提示分工`
     - 要解决的是：工作台只保留左下角提示；NPC 也不再回到头顶交互提示；视觉归属与实际触发对象一致。
- 当前阶段判断：
  - 这份拆解已经把用户原始 8 点、后续补丁、以及这次新增的制作条/拾取/悬浮框规则收成了本轮唯一施工基线。
  - 但它目前仍然只是 `结构 / 需求基线`，还不是实现完成，更不是体验过线。
- 当前恢复点：
  1. 先继续修 `Prompt / Dialogue` 的缺字链。
  2. 再继续修 `Workbench` 左列显示链与正式面布局。
  3. 最后进入制作条/拾取/取消/悬浮框这组状态机闭环。

## 2026-04-04：吸收剧情源协同边界后继续施工，并先结算一个可提交 checkpoint

- 当前主线目标：
  - 继续收 `spring-day1` 玩家面 `Prompt / Dialogue / Workbench / 左下角提示 / 悬浮框 / 制作交互闭环`，但新的协同边界已经明确：
    1. 不停工等剧情线程
    2. 不回吞 `SpringDay1Director.cs`
    3. 继续把 UI 做成能承受剧情源增长的稳结构
- 这轮新增稳定事实：
  1. `DialogueUI.cs`
     - `continue` 文案比较条件已改成 `System.StringComparison.OrdinalIgnoreCase`，从源码层切断了 `StringComparison` 红错依赖链；
     - 继续提示文本链仍按 runtime 自愈方向在收，不再只信 prefab 原值。
  2. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 左列 `RecipeRow` 现在开始主动修复文本链：
       - 绑定 row 时强制恢复 `Name / Summary` 可见态；
       - 刷新 row 时再次强制恢复；
       - “文本非空但组件失活/透明”的情况，已经纳入坏壳判定，不再只看字符串是否为空。
  3. `Prompt / Workbench / 左下角提示` 当前仍只站在 `结构 / targeted probe` 层；
     - 没有新的玩家可见 GameView 证据前，不得写成体验过线。
  4. 已吸收 `2026-04-04_UI线程_继续施工引导prompt_04.md` 与两份剧情源协同文档；
     - 后续继续 own：
       - 任务栏缺字
       - continue 空白
       - Workbench 左列空白
       - 正式面排版
       - 制作条状态机
       - 悬浮框
     - 后续不再扩写：
       - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
       - `Assets/111_Data/Story/Dialogue/*`
- 当前验证状态：
  1. `git diff --check` 在当前 UI own 代码范围内没有阻断错误，只有 CRLF/LF 提示。
  2. `unityMCP validate_script` 本机这轮不可用，原因是本地 `http://127.0.0.1:8888/mcp` 连接失败；
     - 所以这轮 checkpoint 的主证据仍是：
       - own 白名单 diff 核查
       - `git diff --check`
       - `Ready-To-Sync` 预检
- 当前阶段判断：
  - 这轮已经从“继续泛分析”切回“继续真实施工 + 先落一个可提交 checkpoint”。
  - 但仍然只是：
    - `结构成立，局部验证继续推进`
    - 不是 `真实入口体验已过线`
- 当前恢复点：
  1. 先完成当前 UI 白名单 checkpoint 提交。
  2. 提交后继续往下收：
     - `Prompt / Dialogue` 缺字
     - `Workbench` 左列与正式面
     - `制作条 / 悬浮框 / 拾取 / 取消` 状态机

- 2026-04-04 Prompt/Dialogue 字链定向修补：
  - 当前主线目标仍是 `spring-day1` 玩家面 UI/UE 收口；本轮子任务被明确收窄为：只在 `SpringDay1PromptOverlay.cs` 与 `DialogueUI.cs` 里继续修 `任务卡 / Prompt / continue` 的“有框无字或中文字体不对”。
  - 本轮实际完成：
    1. `SpringDay1PromptOverlay.cs`
       - 把 `subtitle` 正式纳入 readable recovery 判定，不再只盯 `title / focus / footer`；
       - `HasReadablePrimaryText` 改成同时校验：节点启用、alpha、Rect 尺寸、字体覆盖、当前文本与期望文案一致；
       - 新增 prompt 文案归一化比较与 `EnsurePromptTextContent`，坏旧壳上“有字但不是期望字”的情况会被判成不可继续沿用；
       - `PageMatchesState` 与 runtime page 绑定判定进一步收紧，减少旧 runtime 壳把错误文案/坏文本链继续带活的概率。
    2. `DialogueUI.cs`
       - `continue` 标签引用恢复链从“只找第一个子文本”改成“筛选可复用标签，否则自建标签”；
       - 新增 `continue` 标签 Rect 归正、父链激活/CanvasGroup alpha 恢复；
       - `continue` 文案自愈从“只修空白或英文”收紧到“非中文也回正为 `继续`”；
       - 空文本探针改成直接用 `继续`，避免空字符串时错误接受不支持中文的字体。
  - 代码层验证：
    - 已运行 `CodexCodeGuard`，对白名单中的 2 个 C# 文件完成 `utf8-strict + git diff --check + roslyn-assembly-compile`，本轮新增改动未引入代码层红错。
  - 当前仍未闭环：
    - Unity/MCP console 仍不可用，缺 fresh Unity 红错证据；
    - `Ready-To-Sync` 被 own root 内其他现存脏改阻断，阻断项不在这轮允许处理范围里，因此这轮只能写成“这两份脚本代码层 clean，线程级 sync 仍被同根残留挡住”。
  - 恢复点：
    - 后续如果继续推进 `Prompt / Dialogue`，优先看真实 GameView 或用户反馈确认这次“坏壳误复用 + 中文假可用字体”是否已明显收敛；没有玩家面证据前，仍只算 `targeted probe`。

- 2026-04-04 Opening 测试红错回查与 no-red 复核：
  - 当前主线目标不变，仍是 `spring-day1` 玩家面 UI/UE 收口；本轮子任务是用户新报的 `SpringDay1OpeningRuntimeBridgeTests.cs` 编译红错止血，确保测试层不再把 shared root 留红。
  - 本轮读取与判断结果：
    1. 当前磁盘版 [SpringDay1OpeningRuntimeBridgeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1OpeningRuntimeBridgeTests.cs) 已经是反射式写法，不再直接 `using Sunset.*`，也不再编译期硬引用 `SpringDay1Director / StoryManager / TimeManager / EnergySystem / HealthSystem`；
    2. 用户贴出的行号与当前文件内容已经对不上，说明那组 `Sunset / Type / SpringDay1Director` 报错不再对应当前磁盘版，很可能是旧编译快照或旧版本内容；
    3. `Tests.Editor.asmdef` 当前额外保留了 `Unity.TextMeshPro` 引用，用来兜住同批 Editor 测试里对 TMP 相关类型的编译需求。
  - 本轮验证证据：
    1. `CodexCodeGuard` 对以下 3 个测试文件的 `utf8-strict + git diff --check + roslyn-assembly-compile` 通过，`AffectedAssemblies = Tests.Editor`：
       - [SpringDay1OpeningRuntimeBridgeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1OpeningRuntimeBridgeTests.cs)
       - [SpringDay1OpeningDialogueAssetGraphTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1OpeningDialogueAssetGraphTests.cs)
       - [DialogueChineseFontRuntimeBootstrapTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/DialogueChineseFontRuntimeBootstrapTests.cs)
    2. `python scripts/sunset_mcp.py no-red Assets/YYY_Tests/Editor/SpringDay1OpeningRuntimeBridgeTests.cs --owner-thread UI --count 20`
       - 返回 `assessment=no_red`
       - `owned_errors=0`
       - Console 未归属到该文件的异常/警告仅剩外部项：空文件路径的 `NullReferenceException` 与 `There are no audio listeners in the scene`
    3. `git diff --check` 对上述 3 个测试文件与 [Tests.Editor.asmdef](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/Tests.Editor.asmdef) 通过。
  - 当前阶段判断：
    - 这轮只能得出“Opening 这组测试文件当前代码层已 clean，且目标文件已过一次 `no-red` 归属检查”；
    - 不能把它上升成“UI/UE 体验已过线”。
  - 当前恢复点：
    1. 主线恢复到 `Workbench 产物留台 / 拾取 / 取消 / 悬浮框进度` 的运行时闭环。
    2. 如果用户再次看到 Opening 测试同类旧错误，优先先看 fresh console 是否仍指向当前磁盘版，而不是直接把旧行号当成当前事实。

- 2026-04-04 Workbench 留台/领取/取消 第一轮闭环：
  - 当前主线目标仍是 `spring-day1` 玩家面 UI/UE 收口；这轮主刀重新回到 `Workbench`，把“制作后自动入包”的旧链正式掰回“产物留在工作台，玩家再领取”的语义。
  - 本轮实际代码改动集中在 3 个文件：
    - [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
    - [CraftingService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Crafting/CraftingService.cs)
    - [CraftingStationInteractable.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs)
  - 本轮已落成的行为变化：
    1. `WorkbenchOverlay` 内新增了工作台队列条目状态，开始区分：
       - 当前活跃制作
       - 已完成但未领取产物
       - 关闭 UI 后仍需保留的工作台会话状态
    2. `CraftRoutine()` 已改走 `TryCraft(recipe, false)` 分支，不再默认把产物直接塞进背包；
    3. 制作条点击语义已切换：
       - 有未领取产物时，点击优先领取；
       - 没有可领取产物但仍在制作时，点击执行取消；
       - 追加制作仍保留在原按钮/数量链路里；
    4. 取消制作时，如果当前单件材料已经被预扣，会尝试返还到背包；放不下的部分会掉到工作台附近世界里；
    5. `GetMaxCraftableCount()` 不再按背包容量截断可制作数量，改回以材料为主；
    6. 关闭工作台面板后，只要还有活跃制作或未领取产物，悬浮小框就能继续保留，而不会被 `CleanupTransientState(resetSession: true)` 一把清空；
    7. `CraftingStationInteractable` 的左下角提示文案已补出“普通打开 / 制作中 / 有可领取产物”分支，不再只会泛写“打开工作台”。
  - 当前显示层同步结果：
    1. 大面板制作条现在会按新语义切色：
       - 制作中默认绿条进度
       - hover 且可领取时转黄态
       - hover 且只能取消时转红态
    2. 关闭面板后的悬浮框也不再只认“活跃制作中”，对于“已完成但未领取”的状态会继续显示。
  - 当前验证状态：
    1. `CodexCodeGuard` 已对上述 3 个文件完成 `utf8-strict + git diff --check + roslyn-assembly-compile`，结果通过，`AffectedAssemblies = Assembly-CSharp`。
    2. `python scripts/sunset_mcp.py no-red ...` 对这 3 个文件返回：
       - `owned_errors = 0`
       - `external_errors = 0`
       - 但整体 assessment 仍是 `unity_validation_pending`，原因不是本轮代码红，而是当前 Unity MCP 状态持续卡在 `ready_for_tools = false / blocking_reasons = stale_status`。
  - 当前仍未闭环的点：
    1. 悬浮进度目前还是单卡模式，尚未扩成“多不同 recipe 并排 3x2 卡片”；
    2. `Prompt / 任务 / continue` 那条缺字链还缺一轮继续复核；
    3. 这轮能站住的是 `代码层 / targeted probe`，还不是玩家实机体验过线。
  - 当前恢复点：
    1. 下一步继续补 `Prompt / 任务 / continue` 缺字链；
    2. 如果继续回到 `Workbench`，优先收的是多浮窗卡片与更细的显示层 polish，而不是再回服务层重做大架构。

- 2026-04-04 Workbench 制作条真入口 + 缺字链后置字体兜底：
  - 当前主线目标不变，仍是 `spring-day1` 玩家面 UI/UE 收口；本轮子任务聚焦两个玩家最直接能看到的问题：
    1. `Workbench` 的“领取 / 取消”虽然已有状态，但点击入口还没真正迁到制作条；
    2. `Prompt / 任务栏 / Workbench 左列` 的“有组件没文字”仍像字体链后置不足导致的假活状态。
  - 本轮实际代码推进：
    1. [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
       - 给 `progressRoot` 正式绑定了 `progressButton` 与 hover relay；
       - 新增 `OnProgressBarClicked()`，把“有产物先领取、否则取消制作”的语义从 `craftButton` 收回到制作条本体；
       - `craftButton` 现在只保留“开始制作 / 追加制作”；
       - `UpdateQuantityUi / BuildStageHint / UpdateProgressLabel / BuildCraftButtonLabel` 已按新入口分流，领取/取消 hover 改看 `_progressBarHovered`，追加 hover 仍看 `_craftButtonHovered`；
       - 补了 `EnsureWorkbenchTextContent()`，在真正写入中文文案后再次校正字体、透明度和 mesh，重点兜底：
         - 左列 recipe 名称/摘要
         - 右侧名称/简介/材料
         - 进度条文案
         - 制作按钮文案
         - 提示文案
    2. [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
       - `EnsurePromptTextContent()` 改成实例级后置兜底；
       - 在标题、副标题、focus、footer、任务行 label/detail 真正赋值后，再按当前中文内容补一次字体可读性校正，避免“空字符串阶段没触发换字库，后面塞中文却仍挂旧字体”。
  - 关键判断：
    - 这轮更像是把“文本链只在空文本阶段自检”的漏洞补上；它不会自动等于体验过线，但它确实对准了“有壳无字 / 能点无字”的高概率根因。
  - 当前验证状态：
    1. `python scripts/sunset_mcp.py no-red --owner-thread UI ...`
       - 对 `SpringDay1WorkbenchCraftingOverlay.cs + SpringDay1PromptOverlay.cs + CraftingService.cs + CraftingStationInteractable.cs` 的 `CodexCodeGuard` 已通过；
       - `owned_errors = 0 / external_errors = 0`；
       - 当前仍未拿到 fresh Unity ready，只是因为 Editor 端继续卡 `stale_status`，不属于本轮 own red。
    2. 单独只扫 `PromptOverlay + WorkbenchOverlay` 时会出现“看不到 `TryCraft(recipe, false)` 重载”的代码闸门假红；
       - 把 [CraftingService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Crafting/CraftingService.cs) 一并纳入检查后恢复正常，说明这是当前 codeguard 视野切片问题，不是运行时代码真实回退。
  - 当前仍未闭环：
    1. 还没有拿到玩家面实机证据确认“任务栏开局不空 / continue 不空 / 左列 recipe 真出字”；
    2. Workbench 多悬浮框 3x2 排列、产物拾取后的完整视觉 polish 仍没收完；
    3. 这轮仍属于“代码层 + targeted no-red + 文本链修正”，不等于玩家体验终验。
  - 当前恢复点：
    1. 下一步优先要做的是继续对 `Prompt / continue / Workbench 左列` 做 live 取证或更深一层运行时排查；
    2. 如果这些文本链已恢复，再回到多悬浮框和剩余工作台 polish。

- 2026-04-05 凌晨补记：Workbench 左列 recipe 缺字链继续收窄到 runtime 坏壳复用门槛
  - 当前主线目标不变，仍是 `spring-day1` 玩家面缺字链；本轮子任务只守 [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs) 一处，不扩到 prefab、剧情源或别的 UI 文件。
  - 本轮新增稳定判断：
    1. 左列 `RecipeRow_` 的最可疑责任点已经进一步收窄到“旧 screen-overlay 壳仍可能把坏 row 当健康 row 复用”；
    2. 当前实现里 `HasReusableRecipeRowChain()` 只要命中一条可绑定 row 就允许整壳继续复用，`CanReuseRecipeText()` 也还不要求文本非空、alpha 可见、rect 有尺寸；
    3. 这正好会给“按钮还活着，但文字链已空/透明/尺寸坏掉”的 row 留下通过口子。
  - 本轮实际修改：
    1. `HasReusableRecipeRowChain()` 改为只检查当前 active 的 `RecipeRow_`，并要求这些 active row 全部通过文本链完整性检查后，整壳才允许继续复用；
    2. `CanReuseRecipeText()` 额外收紧为：
       - 文本必须启用且在 hierarchy 中可见
       - alpha 必须大于 0
       - rect 宽高必须大于 2
       - 文本内容不能是空白
       - 当前字体必须真的能渲染这段文本
    3. 因此左列如果再遇到“能点但没字”的旧 row，会直接失去复用资格，转而走重建/重刷链，而不是继续带着空壳过关。
  - 当前验证状态：
    1. `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs` 通过；仅有既存 `CRLF -> LF` 提示，不属于本轮代码错误；
    2. `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --skip-mcp` 本轮在本机超时，未拿到 fresh 程序集级结果；
    3. 因此这轮只能宣称：`最可能责任点已继续收窄并已补代码门槛，Unity/live 仍待验证`。
  - 当前恢复点：
    1. 下一步应优先拿一次 Workbench 左列 recipe 的 fresh live，确认坏壳不再截胡；
    2. 如果 fresh live 仍空，再继续往字体底座或数据注入链下钻，而不是先回漂 Workbench 全量 polish。

- 2026-04-05 凌晨补记：Town 中文 DialogueUI + Prompt/Workbench 缺字链继续施工
  - 当前主线目标没有换，仍是 `spring-day1 / Town` 玩家面缺字链；本轮子任务是把最影响可消费性的三处文字缺失继续往前推：
    1. `DialogueUI` 的中文正文与 continue 标签坏壳
    2. `PromptOverlay` 的任务栏 / 中间任务卡有壳无字
    3. `WorkbenchOverlay` 左列 recipe 能点没字
  - 本轮显式使用：
    - `skills-governor`
    - `preference-preflight-gate`
    - `sunset-no-red-handoff`
  - 本轮新增稳定判断：
    1. Town 中文链当前更像 `DialogueUI` 自己的 continue 标签绑定与字体兜底不稳，不是双 bootstrap 冲突；
    2. Prompt 缺字链当前最像 `runtime 坏壳/坏 row 误复用 + 写入后缺少字体/alpha/父链再校正`；
    3. Workbench 左列缺字链当前最像 `RecipeRow_` 旧壳复用门槛过宽，让空/透明/坏尺寸的 stale row 继续过关。
  - 本轮实际代码推进：
    1. [DialogueUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/DialogueUI.cs)
       - 在 `Awake / EnsureUsableRuntimeFonts / ResolveUsableDialogueFont` 前置调用 `DialogueChineseFontRuntimeBootstrap.EnsureRuntimeFontReady()`；
       - 收紧 continue 标签候选评分，优先选 enabled、全拉伸、含中文且字体真能渲染“继续”的节点；
       - 把 `任意键继续对话 / continue / jixu / 非中文占位` 统一收敛成稳定的 `继续`。
    2. [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
       - 新增更严格的 readable 判定，不再只看“有个节点”；
       - 对 title/subtitle/focus/footer 与 row label/detail 写入后再次补字体、alpha、父链可见性；
       - runtime 坏壳/坏 row 不再继续被当健康 shell 复用。
    3. [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
       - 收紧 `HasReusableRecipeRowChain()` 与 `CanReuseRecipeText()`：active row 的文本必须非空、可见、rect 有尺寸、字体可渲染，旧壳才允许复用；
       - 左列 row 与右侧关键文本写入后都会再走一次 `EnsureWorkbenchTextContent()`，补字体、alpha 与 mesh；
       - 因此“按钮还活着但 `Name/Summary` 已空/透明/坏尺寸”的 stale row 会被直接判坏并走重建/重刷链。
  - 当前验证状态：
    1. `git diff --check` 对上述 3 个目标脚本通过；只有 `CRLF -> LF` 提示，不属于 owned error；
    2. `python scripts/sunset_mcp.py validate_script ...` 对 `DialogueUI / PromptOverlay / WorkbenchOverlay` 本轮都在本机超时，未拿到 fresh 程序集级结果；
    3. 因此这轮只能站住：`代码层 / targeted probe 继续推进`，不能写成 `真实体验已过线`。
  - 当前恢复点：
    1. 下一步优先拿 fresh live 复核 4 个玩家面 case：
       - 开局左侧任务栏
       - 中间任务卡
       - 村长对话右下角继续
       - Workbench 左列 recipe
    2. 如果仍缺字，再继续往字体底座或数据注入链下钻；
    3. 不回漂到 NPC 气泡整线，也不把这轮包装成 UI 全面终验。

- 2026-04-05 只读核查补记：工作台左列空白与任务清单工作台阶段异常的代码性质判断
  - 本轮只读检查结论，不含实现：
    1. Workbench 左列空、右侧详情仍有内容，这不是单纯截图表象；从代码结构上确实存在“左右两侧不同步”的真实缺口。
    2. [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs) 里，右侧详情由 `GetSelectedRecipe() / RefreshSelection()` 直接驱动；左侧列表则走 `RefreshRows()` + row 绑定/可读性链。也就是说，只要 `_selectedIndex` 还有效，右侧可以正常显示，但左侧 row 文本链坏掉时仍会出现“右侧有详情、左侧空白”。
    3. 这个问题性质上属于 `WorkbenchOverlay` 局部但结构性的 UI 同步缺口，不是全项目所有 UI 的广泛字体问题。
    4. 任务清单这边也不是纯显示偶发；[SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 在工作台闪回阶段明确生成 2 条任务、农田教学阶段明确生成 5 条任务，但 [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs) 的 `BuildCurrentViewState()` 固定把 `PromptCardViewState.FromModel(..., maxVisibleItems: 1)` 写死了。
    5. 这意味着“进入工作台交互任务后任务清单状态不对”从代码上是一个真实的全局策略问题：不是这一个阶段数据没给，而是 `PromptOverlay` 整体就被硬截成只显示 1 条。
  - 当前判断：
    1. Workbench 左列问题 = 局部结构缺口；
    2. 任务清单阶段问题 = `PromptOverlay` 层面的全局策略问题；
    3. 两者都不是我现在看了截图才反推出来，而是代码里已经能直接读出的责任面。

- 2026-04-05 只读盘点补记：shared / UI `气泡 / 提示` 宿主现状
  - 本轮主线：
    - 只读盘点 shared/UI 范围内当前仍活着的 `气泡 / 提示 / tooltip` 视觉宿主与显式样式入口。
  - 当前稳定结论：
    1. [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs) 是 shared/UI 当前真正在线的交互提示卡宿主。
       - 没有显式 `preset / enum mode`
       - 但 `ApplyContentLayout()` 会根据 `hasKeyLabel / hasDetail` 切 `DetailCardWidth / CompactCardWidth`、accent 颜色与文案布局，所以代码上至少存在 `带键正式卡 / 无键紧凑卡` 两种可见布局态。
    2. [InteractionHintDisplaySettings.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintDisplaySettings.cs) 只负责可见性开关，不承载样式 preset。
    3. [ItemTooltip.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs) 是 shared/UI 另一条独立 tooltip 壳：
       - 没有显式 `preset / kind / mode`
       - 只有单一 tooltip 视觉壳，标题颜色、价格、品质图标和正文由 `TooltipVisualData` 驱动
       - [EnergyBarTooltipWatcher.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/EnergyBarTooltipWatcher.cs) 只是把精力条 hover 文案接到这条 shared tooltip 链。
    4. [DialogueUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/DialogueUI.cs) 属于 shared/UI 文本表现层，不是世界提示卡壳；但它确实有字体样式路由：
       - 代码硬编码 fallback key：`default / speaker_name / inner_monologue / garbled`
       - [DialogueFontLibrarySO.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Data/DialogueFontLibrarySO.cs) 允许继续扩展 key
       - 当前代码里实际出现的内容 key 至少有 `default / narration`
  - 当前验证状态：
    - `静态代码盘点成立`
    - 未改代码、未跑 live。
  - 当前恢复点：
    - 如果后续要统一 shared/UI 视觉语言，先决定三条链是否要共基线：
      1. `InteractionHintOverlay`
      2. `SpringDay1PromptOverlay`
      3. `ItemTooltip`

- 2026-04-05 只读复核补记：003->004 与 005 任务清单空窗的性质重新收窄
  - 本轮只读结论：
    1. `004 工作台闪回` 和 `005 教学阶段` 的任务数据本身并没有缺失；[SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 明确会生成：
       - `WorkbenchFlashback` = 2 条任务
       - `FarmingTutorial` = 5 条任务
    2. 因此这不是“任务系统没给内容”，也不是所有阶段都坏；用户实测里“做完后后面的会显示”与代码是一致的。
    3. 当前更像是两个同源但不同层级的问题叠在一起：
       - 正式剧情对话期间，任务清单本应退场，但 [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs) 里 `_suppressWhileDialogueActive` 从头到尾没有被置成 `true`，`OnDialogueStart/OnDialogueEnd` 都在写 `false`；
       - 同时 [DialogueUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/DialogueUI.cs) 的“隐藏其他 UI”还显式排除了 `SpringDay1PromptOverlay`，所以正式剧情对话时任务清单不会按预期先退掉。
    4. `003 -> 004` 和 `005` 切步骤时出现“只剩黑色透明条、文字短暂没了”，代码上更像是 `PromptOverlay` 在 phase / display signature 变化时会执行 `PlayPageFlip()`，壳体先翻页，文字链稍后补齐；如果这时新页文本链恢复不及时，就会出现短暂空窗。
  - 当前判断：
    1. 正式剧情时任务清单不隐藏 = 明确代码 bug；
    2. 阶段切换时短暂只剩底板 = 高概率同源的过渡显示 bug；
    3. 这两个都更像 `PromptOverlay <-> DialogueUI` 的协同问题，不是工作台任务数据源本身坏掉。
  - 用户新补充已记住但本轮不展开实现：
    1. 正式剧情对话前后应做 `0.5s 隐藏全部 UI -> 0.5s 显示对话框`；
    2. 玩家内心活动不要整块半透明背景，只收紧到可读样式；
    3. 剧情对话字体需要换成更清楚的像素字。

- 2026-04-05 继续只读治理补记：已审核 `NPC` 回执，并把 `UI/NPC` 玩家面边界重新收紧
  - 本轮主线：
    - 不继续实现；只做 `NPC` 回执审核、下一刀 prompt 分发、以及 `UI` 自身剩余任务清单重排。
  - 当前接受的 `NPC` 边界：
    1. `NPC own` 继续只守：
       - `NPCBubblePresenter.cs`
       - `PlayerNpcChatSessionService.cs`
       - `speaking-owner / pair / ambient bubble` 底座
    2. `UI` 继续只守 shared 玩家面壳与 formal-face：
       - `InteractionHintOverlay.cs`
       - `SpringDay1PromptOverlay.cs`
       - `SpringDay1WorkbenchCraftingOverlay.cs`
       - `DialogueUI.cs`
    3. 额外收紧一条：
       - shared 提示壳的 hide/show 接口仍归 `UI`
       - `NPC` 只给 NPC own 一侧的语义真值与 contract，不再泛吞提示壳控制面
  - 本轮已落盘产物：
    1. 新的 `NPC` prompt 已写入：
       - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-05_NPC线程_气泡层级遮挡与不透明基线第一刀prompt_02.md`
    2. 这刀只要求 `NPC` 收：
       - 谁在说话谁在上面
       - 气泡不被树木/场景压住
       - 背景完全不透明
  - 当前 `UI` 线最该继续的剩余项，已重新排序为：
    1. `PromptOverlay / DialogueUI` 协同链：
       - 正式剧情时任务清单正确退场
       - `003 -> 004 / 005` 阶段切换后任务文字稳定显示
    2. `PromptOverlay` 的任务显示策略：
       - 当前代码被硬截成只显示 1 条，需决定是修稳定性还是改回更完整阶段显示
    3. `WorkbenchOverlay` 左列 recipe 空白：
       - 仍是局部结构缺口，但优先级暂时低于前两项
  - 当前下一刀最深允许面：
    1. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
    2. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs`
    3. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
    4. 如确有必要，再补：
       - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
  - 当前下一刀明确不再碰：
    - `SpringDay1WorkbenchCraftingOverlay.cs`
    - `InteractionHintOverlay.cs`
    - `NPCBubblePresenter.cs`
    - `Primary.unity`
    - `GameInputManager.cs`
  - 当前判断：
    - 用户此刻的主矛盾已经收敛到 `PromptOverlay <-> DialogueUI` 的任务清单/正式剧情协同链；
    - 继续回去打 `Workbench` 全量 polish 或 `NPC` shared 壳，只会再次漂移。
  - thread-state：
    - 本轮未进入真实业务施工；
    - docs/prompt slice 已补 `Park-Slice`；
    - 当前状态继续视为 `PARKED`。

- 2026-04-05 玩家面大刀施工补记：`PromptOverlay / DialogueUI / Workbench / InteractionHint` 同轮推进
  - 当前主线：
    - 用户要求不要再停在小刀，直接把 `UI` 线当前能落的玩家面问题尽量做到底；本轮按 `PromptOverlay / DialogueUI / Workbench / InteractionHint / CraftingStation` 一起推进。
  - 本轮显式命中：
    - `skills-governor`
    - `preference-preflight-gate`
    - `sunset-no-red-handoff`
    - `user-readable-progress-report`
    - `delivery-self-review-gate`
  - 这轮实际施工结果：
    1. [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
       - 把 `OnDialogueStart` 的抑制逻辑改回真抑制，不再继续写成 `false`；
       - `OnDialogueEnd` 改成真正恢复 pending state，而不是空转；
       - `ShouldDelayPromptDisplay()` 收紧为“对话管理器真的处于 active 才持续压住”，这样能兼容旧的 synthetic runtime test，但正式剧情时仍会隐藏；
       - 任务页转场新增 `ShouldUsePageFlipTransition()`，默认走稳定即时刷新，不再让 phase 切换靠翻页把字翻没；
       - 任务条目构建链仍保持 `1` 条主任务显示，但内部改成稳定选择 primary item，避免在切 phase 时行序和 stale row 更容易漂。
    2. [DialogueUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/DialogueUI.cs)
       - 对话开始 / 结束时，统一把“其他 UI 淡出 / 淡回”最小时长收紧到 `0.5s`，正式剧情节奏更接近用户新要求；
       - `ShouldManageAsNonDialogueUi()` 不再把 `PromptOverlay` 排除在外，所以正式剧情时任务卡会跟着一起退场；
       - 内心独白不再把整块背景打到厚半透明，背景 alpha 改为更轻的 `0.12` 上限，优先保字。
    3. [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
       - 进度条默认文案从“准备好后即可开始打造”改回 `进度 0/0`；
       - `stageHintText` 的 hover 话痨提示大幅清空，默认只保留真正 blocker，给材料区和正式面腾空间；
       - 进度条 hover 语义改成：
         - 有可领产物时 = `领取产物`
         - 可中断时 = `中断制作`
         - 正常制作时 = `进度 ready/total`
       - 完成一个配方后不再顺手往 `PromptOverlay` 弹“已完成制作”那种干扰提示；
       - 左列 recipe 列表刷新后新增二次自检：如果 visible row 仍不可读，就强制 rebuild 一次，继续压低“左列可点但没字”的概率；
       - `HasReadyWorkbenchOutputs` 改为同时接受 `_craftQueueCompleted > 0` 的 active queue 真值，避免“已经有产出但语义还没切到可领取”。
    4. [CraftingStationInteractable.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs)
       - `BuildWorkbenchReadyDetail()` 全部重写成当前真实语义：
         - 打开工作台
         - 进度条领取产物
         - 查看单件进度 / 剩余数量 / 当前队列
         - 不再沿用“查看制作进度、可领取产物”那种泛旧话术
    5. [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
       - 工作台 fallback detail 改成 `打开工作台，查看配方、材料与制作进度。`，避免 detail 丢失时退回空白/测试态。
    6. [SpringDay1InteractionPromptRuntimeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs)
       - 补了工作台 fallback detail 的最小回归断言；
       - `BuildWorkbenchReadyDetail` 的断言改成新的真实 copy：`单件进度 + 领取产物`。
  - 当前没做成的部分：
    1. `Workbench` 多悬浮 `3x2` 阵列、不同配方并行悬浮、以及更大的队列系统，这轮还没继续深挖；
    2. `SpringDay1Director.cs / SpringDay1LateDayRuntimeTests.cs` 因为 `spring-day1` 当前 ACTIVE 占用，没有在这轮并发去改；
    3. `fresh compile / fresh live` 证据没补上，不是代码已完全验证过，而是工具链当前不给结果。
  - 本轮代码层自检：
    1. `git diff --check` 对本轮 touched 文件通过；只有 `CRLF -> LF` 提示，不是 owned error；
    2. `python scripts/sunset_mcp.py manage_script validate ...` 当前被 `127.0.0.1:8888` 拒连；
    3. `python scripts/sunset_mcp.py compile ... --skip-mcp` 以及 direct `CodexCodeGuard` explicit-path 调用，本轮都超时；
    4. 因此这轮只能诚实站住：
       - `代码层推进 + diff-check 通过`
       - `fresh compile / live 未闭环`
  - 当前恢复点：
    1. 如果下一轮继续真实施工，`UI` 线下一优先仍是：
       - `PromptOverlay / DialogueUI` 的正式剧情退场与任务字稳定链 fresh live
       - `Workbench` 左列 recipe fresh live
    2. 如果 fresh live 通过，再决定是否继续吞“多悬浮 / 多配方队列 / 更深 workbench 状态机”。
  - thread-state：
    - 本轮已跑 `Begin-Slice`
    - 收尾已跑 `Park-Slice`
    - 当前 live 状态：`PARKED`
    - 当前 blocker：
      1. `MCP manage_script validate 当前 127.0.0.1:8888 拒连，fresh native validate 不可用。`
      2. `sunset_mcp.py compile/validate_script 与 CodexCodeGuard explicit path 本轮均超时，fresh compile 证据未闭环。`

- 2026-04-05｜继续施工补刀：只收 `PromptOverlay` 缺字字体链 + `Workbench` 左列旧壳复用
  - 当前主线：
    - 在不回吞 `spring-day1` active 导演层的前提下，继续只收玩家面 `004/005` 缺字与 `Workbench` 左列“能点但没字”。
  - 本轮显式命中：
    - `preference-preflight-gate`
    - `sunset-no-red-handoff`
  - 本轮实际推进：
    1. [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
       - 把字体选择从“固定 probe 一次后长期复用”改成“按当前真实文本重选”；
       - `ApplyResolvedFontToShellTexts()`、`MeasureTextHeight()`、`EnsurePromptTextReady()`、`EnsurePromptTextContent()` 都改为按当前目标文本重算字体；
       - 目标是优先修 `003 -> 004 / 005` 边界出现的“底板还在、字没出来”。
    2. [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
       - 命中旧 prefab/manual recipe 行壳时，不再继续沿用半残左列；
       - `EnsureRecipeViewportCompatibility()` 与 `RefreshAll()` 现在都会优先把 manual `RecipeRow_*` 重建成代码生成行；
       - 左列文字字体链也改成按当前文本重选，避免老字体/老材质继续截断 `Name / Summary`。
  - 本轮静态判断：
    1. `PromptOverlay` 缺字主嫌仍是字体覆盖链，不像 `Director` 数据空；
    2. `Workbench` 左列空白主嫌仍是旧壳复用 + manual 行几何，不像 recipe 数据本身缺失；
    3. 当前只站住 `结构 / targeted probe`，不能写成体验已过线。
  - 本轮自检：
    1. `git diff --check -- SpringDay1PromptOverlay.cs SpringDay1WorkbenchCraftingOverlay.cs` 通过；仅有 `CRLF -> LF` warning；
    2. `sunset_mcp.py doctor` 已恢复到 listener/baseline `pass`；
    3. 但 `manage_script validate` 仍报 `No active Unity instance`；
    4. `sunset_mcp.py compile ... --skip-mcp` 即使只压单文件也继续超时；
    5. 因此本轮 no-red 口径只能写：
       - `代码层补刀已落`
       - `fresh compile / live 仍 blocked`
  - 当前恢复点：
    1. 下次若继续真实施工，先想办法拿到 active Unity instance 或别的 fresh compile 入口；
    2. 然后优先做 `004/005` 缺字 fresh live 与 `Workbench` 左列 recipe fresh live；
    3. 若 live 仍显示任务卡几何/挤压问题，再决定是否继续深挖 `PromptOverlay` 纵向自适应。
  - thread-state：
    - 本轮继续施工前已补 `Begin-Slice`
    - 停手已补 `Park-Slice`
    - 未跑 `Ready-To-Sync`
    - 原因：fresh compile / live 证据未闭环
    - 当前 live 状态：`PARKED`

- 2026-04-05｜中途看图后继续补刀：`PromptOverlay` 压缩过长空白并补 fresh validate
  - 当前主线：
    - 用户中途补了一张 Prompt 图，明确指出“内容没那么多，但背景被拉得很长”；本轮继续只收这个玩家面空白问题，不转题。
  - 本轮实际推进：
    1. [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
       - 继续补 legacy page 的纵向容纳逻辑；
       - `RefreshLegacyPageLayout()` 不再把 footer 死守在旧 baseline 上；
       - 现在只有当旧 baseline 只比当前内容低一点点时才保留，否则 footer 直接跟内容流走，避免“字不多却留下大块空白”。
    2. 前一刀补的 generated page 纵向容纳仍保留：
       - `RefreshPageContentLayout()` 会按内容测量 page 高度；
       - `RefreshCardShellHeight()` 会同步更新 card/page 高度，并通过 `SetHeightKeepingTop()` 让壳体主要向下长，不横向改壳体。
  - fresh 验证结果：
    1. `manage_script validate --name SpringDay1PromptOverlay`：`0 error / 1 warning`
    2. `manage_script validate --name SpringDay1WorkbenchCraftingOverlay`：`0 error / 1 warning`
    3. warning 都是同类非阻塞提示：`String concatenation in Update() can cause garbage collection issues`
    4. `sunset_mcp.py errors` 当前 fresh console：`0 error / 0 warning`
  - 当前判断：
    - 这轮终于拿到比前一轮更像 fresh 的 Unity 侧信号：至少当前 console 是干净的、own 脚本 validate 只有 warning；
    - 但还没有 GameView / 玩家面 fresh live，所以仍不能 claim 体验过线。
  - 当前恢复点：
    1. 如果继续，优先补：
       - Prompt 当前空白是否被压回正常高度
       - `004/005` 缺字是否在 live 里真消失
       - Workbench 左列是否已不再出现“能点但没字”
    2. 若 live 仍有异常，再决定是补 Prompt legacy/manual page 壳，还是补 Workbench 旧壳复用门槛。
  - thread-state：
    - 本轮已补 `Park-Slice`
    - 当前 live 状态：`PARKED`

- 2026-04-05｜继续真修：任务列表上飘 + 独白底板/字体 + Workbench 左列/浮动块回正
  - 当前主线：
    - 用户最新实测把主矛盾重新钉成 3 件事：
      1. `PromptOverlay` 任务列表卡片继续上飘，而且黑色半透明底板没动，说明漂的是卡片内层；
      2. `DialogueUI` 独白态仍被大块半透明背景压可读性，且字体不够清楚；
      3. `Workbench` 左列 recipe 仍会空白，同时还会出现主面板与浮动进度块像双开一样的越界感。
  - 本轮实际做成：
    1. [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
       - 继续沿用固定 baseline 的 page/card 高度链，不再让当前高度回写成下一轮几何基线；
       - legacy footer 的最小高度改为受控默认值，不再吃已经被拉坏的旧 rect 高度；
       - 目标是压掉“任务更新一次，卡片就继续往上顶、越顶越高”的累计漂移。
    2. [DialogueUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/DialogueUI.cs)
       - 独白态继续只改独白这一支：
         - 背景图直接清成 `alpha=0` 并禁用；
         - 独白字体切到 `DialogueChinese SoftPixel SDF`，不改其他普通对话字体链。
    3. [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
       - 不再每次 refresh 都强制把 manual recipe 壳整批销毁重建，先回到更接近旧稳定面的行绑定；
       - 面板打开时强制关掉 `floatingProgressRoot`；
       - 浮动块显示条件额外加了“主面板当前实际不可见”这一层，避免主面板和浮动块同时活着。
  - 当前判断：
    - 这轮重点不是“加更多 workbench 功能”，而是先把最近引入的坏几何和坏可视状态往稳定面收；
    - 代码层 now 已 no-red，但还没有 fresh live / GameView 证据，所以不能写成体验已过线。
  - 本轮自检：
    1. `manage_script validate --name SpringDay1PromptOverlay` = `0 error / 1 warning`
    2. `manage_script validate --name DialogueUI` = `0 error / 1 warning`
    3. `manage_script validate --name SpringDay1WorkbenchCraftingOverlay` = `0 error / 1 warning`
    4. `sunset_mcp.py errors --count 20 --output-limit 5` = `0 error / 0 warning`
    5. `git diff --check` 当前仅剩 `CRLF -> LF` warning，无 owned blocking error
  - 当前 blocker / 恢复点：
    1. 下一步必须拿 fresh live 复核：
       - 任务列表是否还会上飘
       - 独白是否还留大块半透明底板
       - Workbench 左列是否恢复文字
       - Workbench 是否还会和浮动块“双开”
    2. 当前 thread-state：
       - 沿用已开的施工 slice
       - 本轮未跑 `Ready-To-Sync`
       - 已补 `Park-Slice`
       - 当前 live 状态：`PARKED`

- 2026-04-05｜只读盘点：把当前未完成项重新压成用户可下令清单
  - 当前主线：
    - `UI` 线仍在收 `spring-day1` 玩家面结果层，当前最需要继续砍的不是扩题，而是把历史遗留重新收束成稳定剩余项。
  - 本轮性质：
    - `只读分析`
    - 未跑 `Begin-Slice`
    - 当前 live 状态保持 `PARKED`
  - 这轮重新确认的未完成主块：
    1. `Prompt / 任务列表`
       - 上飘累计问题基本已压住；
       - 但“字不多却被拉出很长背景”的自适应仍未收好；
       - `003 -> 004 -> 005` 过程中仍有“只剩黑色半透明底条、文字没出来”的阶段性坏态；
       - 正式剧情对话期间的 UI 淡出/淡入节奏还没按用户口径完全收顺。
    2. `DialogueUI`
       - 当前独白已经接了更清楚的像素字体；
       - 但独白背景仍走单独处理分支，还没完全回正到“背景和普通对话一致，只换更清楚字体”的最新口径；
       - 村长右下角继续提示与 Town 中文显示链也还没有 fresh live 过线证据。
    3. `Workbench`
       - 左列 recipe“能点但像空白”的问题仍未被 fresh live 证明真正消失；
       - 主面板仍有越界/排版溢出历史残项；
       - 主面板与 floating 的互斥护栏已补，但还没拿到用户 fresh retest 证明完全稳定。
    4. `Workbench 历史大需求`
       - 只保留左下角提示；
       - 单件进度条 / 完成拾取 / 取消 / 追加 / 悬浮块布局 / 常驻进度文案 / 弹性上下移动 / 相对静止 / 多悬浮 3x2 等一整组状态机与表现层需求，整体仍未闭环。
    5. `玩家面对话 / 气泡体验`
       - 谁说话谁在上、不要被场景遮挡、玩家气泡与 NPC 气泡边界与样式关系、背景不透明这些历史诉求，当前还没有被这一轮完整收掉。
  - 当前判断：
    - 这轮最核心的判断是：当前真正还没做完的重心仍然是 `Prompt / Dialogue / Workbench` 三块，其他历史需求也都存在，但暂时不该继续横向扩题。
    - 最薄弱点是：现在只有代码层 no-red 和局部静态判断，没有 fresh live，不能把任何一块包装成体验已过线。
  - 当前恢复点：
    1. 若下一轮继续真实施工，优先顺序应是：
       - 先收 `Prompt` 高度与 `004/005` 缺字坏态；
       - 再把 `DialogueUI` 独白背景彻底回正；
       - 再继续压 `Workbench` 左列与越界；
       - 最后才考虑 Workbench 大状态机历史尾账。

- 2026-04-05｜继续真修：Prompt 高度链 + Dialogue 独白背景回正 + Workbench 旧壳淘汰护栏
  - 当前主线：
    - `UI` 线继续只收 `spring-day1` 玩家面主矛盾，不回漂到 NPC 底座和全局系统。
  - 本轮真实施工：
    1. [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
       - 生成页不再直接吃 `LayoutUtility.GetPreferredHeight(page.contentRoot)`；
       - 改成按 `contentRoot` 下真实可见 section 逐段求和；
       - 同时把空的 `Subtitle / Focus / Footer / TaskList` 区块直接关掉，避免“没字也继续撑高度”。
    2. [DialogueUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/DialogueUI.cs)
       - 独白分支移除单独背景处理；
       - 保持背景跟普通对话一致，只保留更清楚的独白字体。
    3. [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
       - `RefreshAll()` 只要命中 manual recipe 壳就直接重建左列，不再继续沿用旧壳；
       - runtime 复用判定新增 `!UsesManualRecipeShell()`，旧 manual 壳不再能截胡；
       - legacy detail 手动布局新增一次按当前文本重排的压缩补口，优先防止右侧内容继续往下顶出边界。
    4. 测试补口：
       - [SpringDay1DialogueProgressionTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs)
         新增 Prompt 生成页高度策略护栏；
       - [SpringDay1LateDayRuntimeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs)
         新增 Workbench manual recipe 壳不应被 runtime 继续复用的回归测试。
  - 本轮验证：
    1. `mcp validate_script`
       - `SpringDay1PromptOverlay.cs` = `0 error / 1 warning`
       - `DialogueUI.cs` = `0 error / 1 warning`
       - `SpringDay1WorkbenchCraftingOverlay.cs` = `0 error / 1 warning`
       - 两个测试文件 = `0 error / 0 warning`
    2. `git diff --check`：
       - 无 owned blocking error，仅 `CRLF -> LF` warning
    3. targeted EditMode tests：
       - 新增 Prompt/Workbench 护栏测试已跑过
       - 相关 Prompt/Workbench runtime tests 已跑过
    4. fresh compile / console：
       - 清空 console 后重新请求 compile
       - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 10` = `errors=0 warnings=0`
  - 当前判断：
    - 这轮已经把“结构 / targeted probe”往前推了一大步，代码层 no-red 已站住；
    - 但还没有 fresh 玩家面 live / GameView 证据，所以仍不能写成体验已过线。
  - 当前恢复点：
    1. 若下一轮继续，优先直接看：
       - Prompt 过长空白和 `004/005` 黑底无字是否真消失；
       - Dialogue 独白背景是否已和普通对话一致；
       - Workbench 左列是否恢复，右侧是否仍越界。
    2. 再往后才继续吞 Workbench 大状态机历史尾账。
  - thread-state：
    - 本轮已跑 `Begin-Slice`
    - 未跑 `Ready-To-Sync`
    - 收尾已跑 `Park-Slice`
    - 当前 live 状态：`PARKED`

## 2026-04-05｜继续真修：对话世界硬锁 + 空格继续提示 + Prompt 深色壳体 + Workbench 旧 detail 壳淘汰
- 本轮主线：继续只收 `spring-day1` 玩家面三块主矛盾，不回吞剧情 owner 与 NPC 底座。
- 本轮实际落地：
  1. 正式剧情对话硬锁：
     - `GameInputManager` / `InventoryInteractionManager` / `NPCAutoRoamController` 已接正式剧情对话开始/结束事件；
     - 对话期间玩家世界输入、拖拽持物、NPC 漫游/ambient bubble 全部被压住；
     - 继续保留 `DialogueManager` 自身的时间暂停语义。
  2. `DialogueUI`：
     - 继续键从 `T` 改为 `空格`；
     - 继续按钮新增 `空格` 键帽与 hover/selected 高亮；
     - 独白背景保持与普通对话一致。
  3. `PromptOverlay`：
     - 任务卡外层深色壳体不再把隐藏背页旧高度算进去，继续压“字少但黑底超长”。
  4. `WorkbenchOverlay`：
     - runtime 复用判定新增 `legacy detail manual shell` 淘汰护栏；
     - 旧右侧手工壳不再允许继续复用，目标是压住左列空白与右侧越界沿用坏基线。
- 本轮验证：
  - direct MCP `validate_script` 覆盖 6 个业务脚本 + 2 个测试文件，全部 `0 error`；
  - fresh console 未见新的 owned error；
  - `git diff --check` 无 owned blocking error，仅 CRLF/LF warning；
  - targeted `run_tests` job 已发起且返回 succeeded，但计数口径不稳定，不作为完整测试过线证据。
- 当前判断：
  - 现在可以诚实 claim `代码层 no-red + targeted probe 站住`；
  - 仍不能 claim `玩家体验过线`，因为 Workbench 与 Prompt 还缺 fresh live。
- 下一步：
  1. 先 fresh live 复测三件事：对话期间世界交互硬锁、Prompt 黑底壳体回收、Workbench 左列/右侧是否仍旧壳复用；
  2. 若 live 已压住，再继续深砍 Workbench 大状态机尾账。

## 2026-04-05｜继续真修：回退 Dialogue 花哨提示，压 Prompt 最小高度，放过 prefab 壳
- 用户这轮明确纠偏：
  - 任务卡要回到“背景层包文字层”的简单逻辑；
  - Dialogue 继续提示只改文案，不要再造额外键帽；
  - Workbench 优先回正式 prefab 壳，不要让代码生成壳继续胡来。
- 本轮实际改动：
  1. `DialogueUI`：继续提示回到 `按空格继续`，撤掉额外键帽/hover relay。
  2. `PromptOverlay`：大幅下压最小高度与区块最小占位，继续压黑底超长。
  3. `WorkbenchOverlay`：修掉 prefab 壳被 `_rows.Count == 0` 误判为坏壳的结构问题，让 runtime 更倾向继续使用正式壳体。
- 本轮验证：
  - 3 个业务脚本 + 1 个测试脚本 direct MCP `validate_script` 全部 `0 error`；
  - `git diff --check` 无 owned blocking error，仅 CRLF/LF warning。
- 当前判断：
  - 这轮最大价值是“撤掉错误方向 + 抓到 Workbench 持续变丑的真实根因”；
  - 仍需 fresh live 才能判断体验是否真的被拉回正轨。

## 2026-04-05｜中途续工只读排查：左上任务清单 owner 继续缩圈，Workbench 收到具体问题段
- 当前主线：
  - 用户中途要求先报进度；这轮先不乱改，而是先把“到底修谁”继续查实，避免再修错壳。
- 本轮显式命中：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮只读排查结果：
  1. `SpringDay1StatusOverlay` 已明确排除，它是右上状态条，不是左上任务清单。
  2. 当前编辑态 `Primary` scene 里只有一个 `UI/SpringDay1PromptOverlay`，所以“静态重复实例打架”不是这轮首嫌。
  3. 当前 `UI` 根层真实子链已核对：
     - `UI/State`
     - `UI/ToolBar`
     - `UI/PackagePanel`
     - `UI/DebugUI`
     - `UI/DialogueCanvas`
     - `UI/SpringDay1PromptOverlay`
     - `UI/SpringDay1WorldHintBubble`
     - `UI/InteractionHintOverlay`
     说明左上任务清单仍大概率落在 `PromptOverlay / Dialogue 过场链`，而不是另一个独立 scene 面板。
  4. `Workbench` 当前继续锁到这几段真问题面：
     - `TryBindRuntimeShell() / HasStableWorkbenchShellBindings()`
     - `CanReuseRuntimeInstance() / HasReusableRecipeRowChain()`
     - `RefreshRows() / EnsureRecipeRowCompatibility()`
     - `EnsureMaterialsViewportCompatibility() / AdjustLegacyDetailLayoutToFitCurrentContent()`
- 本轮验证：
  - Unity MCP 只读读取 `Primary` scene hierarchy 与 `UI` 根对象链
  - shell 只读审查 `SpringDay1PromptOverlay.cs`、`SpringDay1WorkbenchCraftingOverlay.cs`、`DialogueUI.cs`
  - 本轮未改代码、未跑 fresh compile、未 claim 体验过线
- 当前判断：
  - 这轮最重要的价值是把一个真实误判排掉：不能再把左上任务清单随便挂到别的壳上修。
  - 但左上任务清单的最终运行态坏态链还没完全钉死，下一刀仍需继续 live/代码对齐排查。
- 当前恢复点：
  1. 下一轮若继续真修，优先先把左上任务清单的真实显示链继续钉死。
  2. 然后直接下刀 `Workbench` 上面四段核心方法，不再泛调别处。
  3. 本轮已跑 `Begin-Slice`，中途汇报后已 `Park-Slice`，当前状态：`PARKED`。

## 2026-04-05｜继续真施工：FooterText 回页内、NPC 世界提示尾巴改回三角、Workbench 停止误重排 prefab 壳
- 当前主线：
  - 用户最新点名 3 件事必须同轮落地：
    1. `SpringDay1PromptOverlay` 的 `FooterText` 不能再掉到 page 外；
    2. `NpcWorldHintBubble` 的气泡尾巴不能再是方块；
    3. `Workbench` 在用户实测前继续把 prefab 正式壳保住，不再被 fallback 兼容重排拉裂。
- 本轮显式命中：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮实际改动：
  1. `SpringDay1PromptOverlay.cs`
     - legacy page 高度改为“当前可见内容高度 + page 自己的壳体 inset”，不再只拿内容顶点；
     - 新增 `GetLegacyPageShellInset()` / `GetRectVerticalInsetWithinParent()`，专门把 `ContentRoot` 的上下边距算回正式 page 高度。
  2. `NpcWorldHintBubble.cs`
     - 箭头从“背景九宫格方块 + 45 度旋转”改回独立三角 sprite；
     - 箭头尺寸改为 `10x7`，并把下偏移从 `-8` 收回到 `-6`，避免恢复三角后继续和气泡底边错位。
  3. `SpringDay1WorkbenchCraftingOverlay.cs`
     - `RefreshCompatibilityLayout()` 现在遇到 `UsesPrefabDetailShell()` 直接停，不再把正式 prefab 详情壳当 legacy fallback 去重排；
     - `BindRecipeRow()` 绑定现成 row 时立刻补一次 `EnsureRecipeRowCompatibility()`；
     - `UsesPrefabRecipeShell()` / `UsesPrefabDetailShell()` 改成支持字段未预绑时的 fallback 查找，减少把正式壳误判成“不可信壳体”的概率。
  4. `SpringDay1DialogueProgressionTests.cs`
     - 补了 `PromptOverlay` page 壳体 inset 断言；
     - 补了 `NpcWorldHintBubble` 三角箭头断言；
     - 补了 `Workbench` prefab 详情壳不应再被 legacy 重排的护栏断言。
- 本轮验证：
  - `manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name NpcWorldHintBubble --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name SpringDay1WorkbenchCraftingOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
  - `validate_script` 直跑仍被本机 `dotnet 20s timeout` 卡住，因此这轮只能 claim 轻量 CLI clean，不能 claim Unity live 已过。
- 当前判断：
  - `FooterText` 出 page 与 NPC 尾巴方块，这两处根因已经被直接砍中；
  - `Workbench` 这轮最关键的是先阻断“正式 prefab 壳被 fallback 兼容链二次拉坏”，这样至少不再继续越修越散；
  - 但 `Workbench` 的真实体验是否完全回正，仍需用户 fresh live 再看。
- 当前恢复点：
  1. 若用户回测 `Workbench` 还有坏态，优先继续顺着 `TryBindRuntimeShell / RefreshRows / RefreshSelection` 这条正式壳链深挖；
  2. 当前这轮适合先让用户看 3 个点：
     - prompt page 里的 `FooterText`
     - NPC 世界提示的尾巴形状
     - workbench 是否还会把正式壳拉裂。

## 2026-04-05｜继续深修：任务卡 footer 与 Workbench 左列/标题的同源 top 对齐 bug 已直接切掉
- 当前主线：
  - 用户贴 fresh 截图后，当前要优先收的真问题已经进一步缩到两条：
    1. `PromptOverlay` 的 `FooterText` 仍被写到 page 外；
    2. `Workbench` 左列 recipe 仍空、右侧物品标题缺失。
- 本轮新增关键判断：
  1. `PromptOverlay` 和 `Workbench` 各自都有一个同源 bug：
     - `SetTopKeepingHorizontal()` 错把“横向拉伸”当成“纵向拉伸”处理；
     - 这会直接把 `FooterText`、recipe `Name/Summary` 这种节点写到容器外面。
  2. `Workbench` prefab 正式壳里的 `SelectedName / SelectedDescription` 自身也带坏几何：
     - 标题和简介的高度值本来就是坏的，所以即使绑定成功也可能看不到标题。
- 本轮新增实际改动：
  1. `SpringDay1PromptOverlay.cs`
     - `SetTopKeepingHorizontal()` 改为按 `anchorMin.y / anchorMax.y` 判断是否“纵向拉伸”；
     - 不再因为节点只是横向拉伸，就用错误的 `offsetMin/offsetMax` 写法把它甩出 page。
  2. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 同样修掉 `SetTopKeepingHorizontal()` 的纵向判定；
     - `EnsurePrefabDetailTextChain()` 现在会额外调用 `NormalizePrefabDetailShellGeometry()`；
     - 先把 prefab 正式壳里的 `SelectedName / SelectedDescription` 拉回可见高度与顶部区间，不再让标题直接消失。
  3. `SpringDay1DialogueProgressionTests.cs`
     - 新增护栏断言，要求这两处 top 对齐逻辑必须按 `anchorMin.y / anchorMax.y` 区分纵向拉伸；
     - 新增护栏断言，要求 Workbench 正式 prefab 壳继续保留 `NormalizePrefabDetailShellGeometry()`。
- 本轮验证：
  - `manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name SpringDay1WorkbenchCraftingOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
  - `git diff --check`：无新的文本层红，只剩 CRLF/LF warning
- 当前判断：
  - 这轮不再是“继续猜布局参数”，而是已经把真正的坐标写错点切掉了；
  - 如果用户这次 fresh 验后 `FooterText` 仍外飘或左列仍空，就该继续从更运行态的绑定链追，不再回到纯静态排版猜测。
- 当前恢复点：
  1. 先让用户 fresh 验：
     - task card 的 `FooterText`
     - workbench 左列 recipe
     - workbench 右侧顶部标题
  2. 若还坏，下一轮直接看运行态 scene/实例绑定，不再只靠源码静态推演。

## 2026-04-05｜补记：已收到新的“从古至今全量 backlog”续工 prompt

- 当前主线没有换：
  - 仍是 `spring-day1` 玩家面 `UI/UE` 收口。
- 本轮新增治理输入：
  - 已生成续工文件：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\2026-04-05_UI线程_day1玩家面从古至今全量清单与唯一主线续工prompt_06.md`
- 这份 prompt 新增钉死了 3 件事：
  1. `UI` own backlog 已按“从古至今”一次收清：
     - `任务栏 / Prompt / continue / 缺字链`
     - `Workbench 正式面`
     - `Workbench 交互矩阵`
     - `Workbench 悬浮小框`
  2. 明确不要再误漂到：
     - `Town.unity / Primary.unity / GameInputManager.cs`
     - `SpringDay1Director.cs / SpringDay1NpcCrowdDirector.cs`
     - `NPCBubblePresenter.cs / PlayerNpcChatSessionService.cs`
  3. 当前唯一主刀顺序被锁成：
     - 先收缺字链
     - 再收 Workbench 正式面与悬浮小框
     - 最后收 Workbench 交互矩阵
- 当前恢复点：
  - 后续如继续 `UI` 线，应以这份 prompt 作为最新统一入口，不再各自沿旧聊天摘任务。

## 2026-04-05：继续只收 task page / workbench 左列两条现有 UI 真问题，已把 legacy page 根节点和 recipe 左列硬自愈链补齐

- 当前主线：
  - 继续只做 `spring-day1` 玩家面现有 UI 问题收口，本轮先不扩到 `Workbench` 交互矩阵 / 多悬浮 / 气泡总整合。
- 本轮新增关键结论：
  1. `PromptOverlay` 里真正把 `FooterText` 拉出 page 的根因，不只是高度算错，而是 legacy prefab 没有 `ContentRoot` 时，`FooterRoot / SubtitleRoot / FocusRoot` 的回退逻辑会把整张 `page.root` 自己认成 section root。
  2. `PromptOverlay` 外层黑壳过高还有一层同源：legacy page 在 `contentRoot == page.root` 时还额外加了壳体 inset，导致 page 和黑壳都被平白拉长。
  3. `Workbench` 左列继续空白时，不能只靠“再试一次 prefab row”；需要在 prefab row 仍不可读时允许直接切到 runtime prefab-style row 的硬恢复。
  4. `Workbench` 右侧标题显示 `Axe_0 / Pickaxe_0` 的原因是玩家面名称仍优先用了内部 `recipeName`。
- 本轮实际改动：
  1. `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
     - 新增 `ResolveLegacySectionRoot()`，legacy `SubtitleText / FocusText / FooterText` 直接挂在 `page` 根上时，布局根节点改回文本本体，不再把 `page.root` 当成 footer 壳。
     - `GetLegacyPageShellInset(page)` 在 `contentRoot == page.root` 时改为 `0f`，让自适应只跟 page 里的真实内容走，不再额外空撑黑壳。
  2. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - `GetRecipeDisplayName()` 改为：若 `recipeName` 像内部 ID（如 `Axe_0`），玩家面优先显示 `item.itemName`。
     - 新增 `NeedsRecipeRowHardRecovery()` / `IsRectReasonablyInsideViewport()`。
     - `RefreshRows()` 发现左列仍是空壳或坏几何时，允许 `RebuildRecipeRowsFromScratch(forceRuntimePrefabStyle: true)`，不再继续复制坏 row。
  3. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 补护栏断言，要求 `PromptOverlay` 保留 `ResolveLegacySectionRoot()`。
     - 补护栏断言，要求 `Workbench` 保留 `NeedsRecipeRowHardRecovery()`、runtime prefab-style row 硬恢复，以及玩家面名称不再直接显示内部 ID。
- 本轮验证：
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1WorkbenchCraftingOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`：仅 CRLF/LF warning，无新的文本坏口
- 当前判断：
  - 这轮已经把 `FooterText/page.root` 的真根因切掉，也给 `Workbench` 左列补了“不再继续挂空壳”的硬恢复。
  - 但这仍是代码层与 targeted guard 层 clean，不等于用户 fresh live 已过线。
- 当前恢复点：
  1. 下一轮优先 fresh 验：
     - 任务卡 `FooterText` 是否已完全回到米色 page 内；
     - 黑壳是否不再空撑；
     - Workbench 左列 recipe 是否恢复显示；
     - Workbench 顶部标题是否不再显示 `Axe_0 / Pickaxe_0`。
  2. 若左列仍空，下一轮直接升到运行态实例绑定 / viewport 几何排查，不再回到静态猜布局。

## 2026-04-05：继续针对 fresh 截图深修，已把 legacy page 根从 stretch 壳回正，并把 workbench 左列恢复链改成强制重建正式 row

- 当前主线：
  - 仍只做两个现有 UI 问题：
    1. 任务卡 `page` 不许再无故拉长
    2. `Workbench` 左列不许再“可点击但空白”
- 本轮新增关键结论：
  1. `PromptOverlay` 这次 page 还被拉长，不是 footer 文本又掉出去了，而是 legacy prefab 的 `Page` 根本身仍然是纵向 stretch。
     - 之前 `ApplyPagePreferredHeight()` 给它写 `sizeDelta.y`，等于在“父壳高度”外又叠了一次页面高度。
  2. `Workbench` 左列这次仍空，是因为恢复链虽然触发了，但还在保留坏 row 当模板继续复制。
  3. `Workbench` 名称仍显示 `Hoe_0 / Axe_0` 的原因也继续确认了：
     - `item.itemName` 本身也是内部 ID，不能只在 `recipeName` 和 `itemName` 之间互切。
- 本轮实际改动：
  1. `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
     - `PrepareLegacyPage()` 里新增 `NormalizeLegacyPageRoot(page)`；
     - legacy prefab 复用时，`Page` 根先从 stretch 改回固定 page，避免再把父壳高度叠到 `page` 自身。
     - 同时在 `PrepareLegacyPage()` 里直接补 legacy `StageTag / Subtitle / Focus / Footer` section root 绑定，再重新记录 `defaultPosition / defaultPivot / defaultHeight`。
  2. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - `RefreshAll()` 和 `RefreshRows()` 的恢复链都改成：一旦进入恢复，直接 `RebuildRecipeRowsFromScratch(forceRuntimePrefabStyle: true)`，不再保留坏模板。
     - 新增 `BuildPlayerFacingInternalName()`；
     - 当 `recipeName` 和 `itemName` 都还是内部 ID 时，直接回退成真正玩家面名称：
       - `Axe` -> `斧头`
       - `Hoe` -> `锄头`
       - `Pickaxe` -> `镐子`
  3. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 补护栏断言，要求保留 `NormalizeLegacyPageRoot(page);`
     - 补护栏断言，要求左列恢复链强制走 `forceRuntimePrefabStyle: true`
     - 补护栏断言，要求保留 `BuildPlayerFacingInternalName`
- 本轮验证：
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1WorkbenchCraftingOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`：仅 CRLF/LF warning
- 当前判断：
  - 这轮 `PromptOverlay` 已经从“调 page 高度”继续推进到“改 page 根的几何真值”；
  - `Workbench` 左列也已经从“再尝试修 prefab row”推进到“坏就直接换正式 runtime row”。
- 当前恢复点：
  1. 下一轮 fresh 验优先看：
     - 任务卡是否还会整张 `page` 拉很长
     - `Workbench` 左列是否终于出现正式可见条目
     - `Workbench` 标题是否变成 `锄头 / 斧头 / 镐子`
  2. 若左列仍空，下一轮就不再继续猜，直接升运行态实例/遮罩/层级排查。

## 2026-04-06：继续真施工，已把任务卡基础页高、Workbench 左列强制恢复、正式对话字体统一推进到代码层 clean

- 当前工作区主线：
  - 继续只收 `spring-day1` 玩家面当前最直观的 3 条 UI 失败面：
    1. 任务卡不能再“缩成一团”或黑壳乱撑
    2. `Workbench` 左列不能再“可点击但空白”
    3. 正式对话主字体要统一回独白那套像素字体
- 本轮显式命中：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮实际推进：
  1. `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
     - legacy page 的最小高度不再塌回极小值，而是回到底层 `page.defaultHeight` 这条基础页高；
     - 外层黑壳高度改为跟 `ResolveLegacyShellMinimumHeight()` 走，不再只剩一条贴字小壳。
  2. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - `RefreshAll()` 改成先刷新、再执行 `ForceRuntimeRecipeRowsIfNeeded()`；
     - 左列一旦出现“文本虽有但被挤出 row 外 / row 仍不可读 / 继续沿用坏模板”这类坏态，就直接 `forceRuntimePrefabStyle: true` 重建；
     - 右侧详情列新增 `ApplyDetailColumnFontTuning()`，把标题、简介、材料、数量、进度、制作按钮文案整体小幅放大。
  3. `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
     - `ContinueButtonDisplayText` 收成纯文案 `摁空格键继续`，不再走额外键帽思路；
     - `ApplyFontPresetForNode()` 的正式对话默认字体改回 `innerMonologueFontKey`，统一到独白那套像素字；
     - 独白颜色逻辑维持当前白字，不再回到半透明背景链。
  4. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 补 string guard，锁住：
       - task page 基础高度
       - Workbench 左列强制 runtime row
       - 正式对话主字体统一回独白字体
       - `摁空格键继续` 纯文案
- 本轮验证：
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1WorkbenchCraftingOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name DialogueUI --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs --count 20 --output-limit 5`：`blocked`，原因是 `subprocess_timeout:dotnet:20s`
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 5`：`errors=0 warnings=0`
  - `git diff --check -- [touched files]`：只有 CRLF/LF warning，无新的文本坏口
- 当前判断：
  - 这轮已经把“继续猜左列为什么空”推进成“左列坏了就直接强制切可读 row”；
  - 也把正式对话字体统一推进到了代码层；
  - 但当前仍然只是 `代码层 clean + targeted guard`，还不能偷报 fresh live 已过线。
- 当前恢复点：
  1. 下一轮优先 fresh 验：
     - 任务卡 page 是否已经恢复“有基础长度但不再乱拉长”
     - `Workbench` 左列是否终于显示出正式可读内容
     - `Workbench` 右侧字体体感是否更接近正式面
     - 正式对话正文是否已经统一到独白像素字体
  2. 若左列仍空，下一轮直接升到运行态实例 / viewport / mask / sorting 读现场，不再回到纯静态修补。

## 2026-04-06：继续按 fresh 截图深修，已把任务卡底部关系改成底锚约束，并把 Workbench 左列升级成稳定生成式 row

- 当前工作区主线：
  - 继续只收你刚 fresh 钉死的两条真关系：
    1. `PromptOverlay` 的 `TaskList -> FocusRibbon/FooterText -> page 底边` 关系
    2. `Workbench` 左列显示链与右侧标题/简介重叠
- 本轮实际推进：
  1. `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
     - legacy page 底部区改成三条固定约束：
       - `LegacyTaskToBottomBandMinGap`
       - `LegacyFocusFooterGap`
       - `LegacyBottomPadding`
     - 现在不是再从顶部一路往下推到底，而是先算底部 `FocusRibbon + FooterText` 带，再反推 page 需要拉多长。
  2. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - `ForceRuntimeRecipeRowsIfNeeded()` 新增 `!HasGeneratedRecipeRowChain()` 触发条件，左列如果还不是稳定生成式 row，直接重建；
     - `CreatePrefabStyleRecipeRow()` 改成真正的生成式布局：`HorizontalLayoutGroup + VerticalLayoutGroup + TextColumn`；
     - `NormalizePrefabDetailShellGeometry()` 不再只改字号，右侧标题/简介连左右边距和顶部关系一起收正；
     - 新增 `SetTopStretchRect()`，避免标题字体变大后继续和简介/图标打架。
  3. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 补护栏，锁住上述底部关系和生成式 row 约束。
- 本轮验证：
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1WorkbenchCraftingOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
- 当前判断：
  - 这轮最核心的推进是：任务卡底部区终于不再靠“碰巧对齐”，而是改成明确的底边约束；`Workbench` 左列也不再靠旧手工 row 续命。
  - 但我仍然不能把它包装成 live 已过线，因为还缺你这边 fresh 画面回证。

## 2026-04-06：补一刀 Workbench 调试直跳开关，已可直接跳到 0.0.5 可开工作台态

- 当前工作区主线：
  - 用户临时插入一个支撑子任务：先做调试开关，不用走剧情，直接把 Day1 推到可开 `Workbench` 的 0.0.5。
- 本轮实际落地：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
     - 在 `Debug` 区新增 `debugSkipDirectToWorkbenchPhase05`
     - 新增 `TryApplyDebugWorkbenchSkip()`
     - 开关打开后，运行时会：
       - 直接切到 `StoryPhase.FarmingTutorial`
       - 视为 `005` 前置的开垦 / 播种 / 浇水 / 木材步骤已完成
       - 保留“回到工作台完成一次基础制作”这一步
       - 给出提示：`调试直跳已开启：直接进入 0.0.5，返回工作台完成一次基础制作。`
  2. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 补 string guard，锁住这个调试开关和运行时应用入口。
- 本轮验证：
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1Director --path Assets/YYY_Scripts/Story/Managers --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
- 当前恢复点：
  - 需要快速测 `Workbench` 时，直接在 `SpringDay1Director` 的 Inspector 勾上 `debugSkipDirectToWorkbenchPhase05` 再进 Play。

## 2026-04-06：继续深修 Workbench 左列坏态，并把 NPC/Dialogue 正式面补口压到代码层 clean

- 当前工作区主线：
  - 继续只收用户这轮仍然最不满意的几条玩家面问题：
    1. `Workbench` 左列 recipe 仍是空壳
    2. NPC 正式气泡 / 玩家正式气泡不能继续跑成方框
    3. `DialogueUI` 的 NPC 缺省头像与玩家字色要回到可消费基线
- 本轮实际落地：
  1. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - 先把 generated row 链正式承认为可复用目标态，不再被自身恢复逻辑反复判坏重建；
     - `RefreshSelection()` 在 prefab detail shell 下补 `EnsurePrefabDetailTextChain()` 和强制 layout rebuild；
     - 通过 fresh workbench capture 坐实“左列仍是空壳”后，继续把真根因切到 generated row 的 `HorizontalLayoutGroup`：
       - 创建时 `rowLayout.childControlWidth = true`
       - 创建时 `rowLayout.childControlHeight = true`
       - 复用已有 generated row 时同样强制把 `generatedLayout.childControlWidth / childControlHeight` 拉回真值
     - 这次修的是布局分配真因，不是再猜文本内容。
  2. `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
     - 在 `UpdateStyleVisuals()` 开头补 `EnsureBubbleShapeSprites()`；
     - 玩家正式气泡 runtime 每次刷新都会把 body / tail 重新绑回圆角气泡 + 倒三角尾巴。
  3. `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
     - 同步补 `EnsureBubbleShapeSprites()` / `ApplyBubbleImageShape(...)`；
     - NPC 正式气泡 runtime 不再允许继续回成方框和方尾巴。
  4. `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
     - 新增 NPC 头像 fallback：
       - 优先从 `Assets/Sprites/NPC/001.png` 对应 sprite 取图
       - 以第一行第二张的上半身为临时头像来源
     - 独白时玩家主文本颜色恢复 `_dialogueBaseColor`，不再被错误刷成纯白；
     - runtime destroy 时补清理 fallback 贴图 / sprite。
  5. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
     - 调试直跳开关继续补成 `EditorPrefs` 可控：
       - `DebugWorkbenchSkipEditorPrefKey`
       - `IsDebugWorkbenchSkipEnabled()`
     - 方便后续不走完整剧情直接进 `0.0.5` workbench 态。
  6. `Assets/Editor/Story/DialogueDebugMenu.cs`
     - 新增菜单：
       - `Sunset/Story/Debug/Toggle Skip To Workbench 0.0.5`
  7. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 新增护栏，锁住 workbench generated row 的 `childControlWidth / childControlHeight = true`；
     - 防止左列再次回到“背景还在、icon/文字尺寸被压没”的空壳状态。
- 本轮验证：
  - `manage_script validate` 为 `clean`：
    - `SpringDay1WorkbenchCraftingOverlay.cs`
    - `DialogueUI.cs`
    - `NPCBubblePresenter.cs`
    - `PlayerThoughtBubblePresenter.cs`
    - `SpringDay1Director.cs`
    - `DialogueDebugMenu.cs`
    - `SpringDay1DialogueProgressionTests.cs`
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 10` 当前回到 `errors=0 warnings=0`
  - 已拿到一张 fresh workbench 运行态证据：
    - `.codex/artifacts/ui-captures/spring-ui/pending/20260406-124142-995_workbench.png`
    - 这张图真实坐实了“左列还是空壳”
  - 继续修完布局根因后，尝试补第二张 fresh capture，但当前被外部 Play / 菜单干扰卡住，没拿到新的稳定复抓文件
- 当前判断：
  - 这轮最关键的推进不是又加了一层 fallback，而是把 `Workbench` 左列坏态切到了真正的布局分配根因；
  - 但当前证据层仍然只能诚实落在：
    - `结构 / checkpoint`
    - `targeted probe / 局部验证`
  - 还不能 claim `真实入口体验已过线`。
- 当前恢复点：
  1. 下轮若继续，应优先在稳定 Play 窗口里重抓一张 fresh workbench 画面，确认左列文字和 icon 已真正出现；
  2. 若 fresh live 仍空，再直接读运行态 row 几何 / mask / viewport，而不是回到纯源码猜排版。

## 2026-04-06：已为 day1 生成重型全量回执，便于后续继续按 UI owner 口径调度

- 当前工作区补充动作：
  - 按用户要求，已把这条 UI 线当前全部真实进度、已做成 / 未闭环 / 可继续深接边界，收成一份可直接给 day1 调度的重型回执：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_UI线程给day1全量回执_01.md`
- 回执重点：
  1. 明确 `任务卡` 已基本退出主战场
  2. 明确当前真主战场是 `Workbench`
  3. 明确 `Workbench` 左列真因已切到 generated row 布局分配层
  4. 明确当前仍不能把结果包装成体验过线
  5. 明确 day1 后续若要减负，应继续把 `spring-day1` 玩家面 UI/UE 压给 UI 线程，不再让 day1 自己吞 UI 壳体细节

## 2026-04-06：prompt_21 深度续工继续推进，Workbench 左列 runtime 恢复链与 one-shot 玩家面提示壳同步加厚

- 当前主线：
  - 继续只做 `Workbench / DialogueUI / formal-informal-resident 玩家面提示壳` 三块。
- 本轮新增做成：
  1. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 在 `TryBindRuntimeShell()` 里补了运行时强制恢复口：
       - 能读到正式 recipe
       - 左列还不是 generated row
       - 就直接 `RebuildRecipeRowsFromScratch(forceRuntimePrefabStyle: true)`
     - 这意味着旧 prefab 手工 row 不再只是“先复用再赌自愈”，而是 runtime 绑定时就主动切到稳定 generated row。
  2. `SpringDay1ProximityInteractionService.cs`
     - `ShouldReplaceCandidate()` 改成 `ForceFocus -> CanTriggerNow -> Priority -> Distance`
     - formal 不再因为更远一点被更近的 informal / resident 抢走
     - `ShouldUseTaskPriorityOverlayCopy()` 去掉 generic prompt 串依赖，只要 formal 仍是 `Available`，左下角就回正成 `进入任务 / 按 E 开始任务相关对话`
  3. `PlayerNpcNearbyFeedbackService.cs`
     - formal priority phase 会直接停止自动 nearby feedback
     - 且会立刻 `HideActiveNearbyBubble()`，避免 formal 接管瞬间残留环境气泡
  4. `NPCInformalChatInteractable.cs` + `PlayerNpcChatSessionService.cs`
     - 新增 `ShouldUseResidentPromptTone()`
     - formal 已让位后，玩家面 idle 提示不再一律是 `闲聊 / 按 E 开口`
     - 最小回落成 `日常交流 / 按 E 聊聊近况`
  5. `SpringDay1InteractionPromptRuntimeTests.cs` + `SpringDay1DialogueProgressionTests.cs`
     - formal vs informal runtime case 改成更硬版本：
       - informal 更近
       - formal 文案不是旧 generic 串
       - 仍应由左下角落成 `进入任务`
     - 同时补了字符串守门：
       - priority 必须先于 distance
       - formal copy 不再依赖 `LooksLikeGenericDialoguePrompt`
       - resident prompt tone 已暴露
       - formal phase nearby feedback 已纳入 suppress 规则
- 本轮验证：
  - `python scripts/sunset_mcp.py errors`：`errors=0 warnings=0`
  - `python scripts/sunset_mcp.py status`：
    - baseline `pass`
    - bridge `success`
    - `isCompiling=false`
  - touched files 的 `git diff --check`：
    - 无新的 owned 语法/空白错误
  - `validate_script --skip-mcp`：
    - 仍被 `subprocess_timeout:dotnet:60s` 卡住
- 当前判断：
  - 可以报实为：
    - `Workbench` 左列 runtime 恢复链更硬了
    - one-shot 玩家面提示壳更接近真实规则了
    - resident 最小回落语义已进玩家面
    - fresh console 当前 clean
  - 仍不能报：
    - `Workbench` 修后 fresh live 已过线
    - `DialogueUI` fresh live 已过线
- 当前恢复点：
  1. 下轮优先补 `Workbench` 修后 fresh live / capture 或第一真实 blocker
  2. 再补 `DialogueUI` 正式面 fresh live
  3. 已新增给 day1 的阶段回执：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_UI线程_给day1阶段回执_25.md`

## 2026-04-06：只读侦查审 DialogueUI / NpcWorldHintBubble / PlayerThoughtBubblePresenter，重新钉死 continue、字体、头像 fallback 与三角尾巴回弹点

- 当前主线：
  - 继续服务 `spring-day1` 玩家面 `UI/UE` 收口；本轮不是继续施工，而是按用户要求退到只读侦查，查清最近哪些代码最可能导致：
    1. `DialogueUI continue` 文案链回弹
    2. 正式对话或 NPC 相关气泡从倒三角回成方框
    3. 玩家独白 / 正式对话字体与头像 fallback 的潜在回弹点
- 本轮只读核对范围：
  - 直接目标：
    - `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
    - `Assets/YYY_Scripts/Story/UI/NpcWorldHintBubble.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
  - 直接关联：
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - `Assets/YYY_Scripts/Story/Data/DialogueFontLibrarySO.cs`
    - `Assets/YYY_Scripts/Story/Dialogue/DialogueChineseFontRuntimeBootstrap.cs`
    - `Assets/YYY_Scripts/Story/Data/DialogueNode.cs`
    - `Assets/YYY_Scripts/Story/Managers/DialogueManager.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
- 本轮新增稳定判断：
  1. `continue` 文案链当前首嫌明确落在 `DialogueUI.cs` 这一轮 dirty：
     - `ContinueButtonDisplayText` 被硬编码成 `摁空格键继续`
     - `NormalizeContinueButtonCopy()` 会把空串 / 非中文 / `continue` / `jixu` / 含 `按空格` 的旧文案，全部强改回这句
     - `GetFontProbeText()` 在目标文本为空时，也拿这句当字体覆盖探针
     - 这意味着 continue 文案、continue 字体探测和空文本阶段的 fallback 已被绑成同一个硬编码入口
  2. 这条 continue 链还有一个更隐蔽的回弹风险：
     - `DialogueChineseFontRuntimeBootstrap.WarmupSeedText` 当前并没有包含 `摁`
     - 但 `DialogueUI` 的 continue 探针却用到了 `摁空格键继续`
     - 如果目标字体对 `摁` 覆盖不稳，就会出现“明明是同一套中文字体，但 continue 这句单独掉字 / 误切 fallback / 字体突然变样”的灰区回弹
  3. 正式对话与独白字体当前被新的代码路径强耦合了：
     - `ApplyFontPresetForNode()` 对正式对话默认也走 `innerMonologueFontKey`
     - 但独白的最终展示又额外走 `ApplyInnerMonologuePresentation()` 里的硬编码 `SoftPixel`
     - 也就是说，正式对话和独白现在表面想统一，底层却同时走“fontLibrary key”与“硬编码 Resources.Load”两条路径；只要 `fontLibrary` 条目、资源名或回退顺序一处漂了，二者就可能再次分家
  4. 头像 fallback 当前也是明显的临时硬编码，不是稳定基线：
     - `ApplyPortrait()` 在缺头像时优先走 `GetOrCreateNpcFallbackPortrait()`
     - 这条 fallback 被写死到 `Assets/Sprites/NPC/001.png`
     - 还固定裁切 `001` 图集的一块上半身区域
     - 所以它不是“缺谁补谁”，而是“任何缺头像都先借 NPC001”；如果资源路径、sprite slicing 或角色基线一动，正式对话头像就会回成错人或灰色占位
  5. “方框尾巴”的第一嫌疑当前不在 `NpcWorldHintBubble.cs`：
     - 这个脚本现在仍在直接创建独立三角 `indicator` sprite，尾巴实现已经是三角链
     - 真正高风险的是 `NPCBubblePresenter.cs`：它会优先 `TryBindExistingBubbleUi()` 复用旧 `NPCBubbleCanvas`，但复用成功后 `UpdateStyleVisuals()` 只改颜色/字号，不像 `PlayerThoughtBubblePresenter` 那样每次都 `EnsureBubbleShapeSprites()` 把 body/tail sprite 重新绑回圆角+三角
     - 这意味着只要旧壳里曾残留方块 body / 方尾巴 sprite，NPC 正式气泡就有天然回弹口
  6. `PlayerThoughtBubblePresenter.cs` 这一轮反而是“相对安全”的：
     - 当前代码每次刷新都会 `EnsureBubbleShapeSprites()`
     - 玩家气泡这条线已具备运行时强制回绑圆角 body + 倒三角 tail 的补口
     - 所以如果用户现在看到“玩家气泡还是方块”，更像是 live 现场没走到这版代码，或上游会话编排/实例对象不是这条 presenter，而不是这份脚本自身缺重绑
- 本轮 owner / 边界改判：
  - UI 线程自己能修的首段：
    1. `DialogueUI.cs` 的 continue 文案/字体探测/头像 fallback
    2. `NpcWorldHintBubble.cs` 的 world hint 三角尾巴与投影链
    3. `PlayerThoughtBubblePresenter.cs` 的玩家气泡视觉壳，但只应保持在玩家气泡表现层，不外扩到会话状态机
  - UI 线程当前不该自己吞的：
    1. `NPCBubblePresenter.cs`
    2. `PlayerNpcChatSessionService.cs`
    3. 正式对话 / informal 会话 owner 的 runtime 编排
    4. scene / prefab / inspector 侧的 live 现场兜底
- 本轮验证层级：
  - 仅完成静态源码审查、当前 dirty diff 审查、最近提交归属核对与 workspace memory 对照
  - 没有改文件
  - 没有跑 fresh live
  - 结论层级只能落在 `结构 / checkpoint`
- 当前恢复点：
  1. 若下一刀要真修，UI 线程最该先切的是 `DialogueUI.cs` 的 continue 文案与字体单一事实源
  2. 若用户看到的仍是 NPC 正式气泡方框 / 方尾巴，优先把锅压回 `NPCBubblePresenter.cs` 的旧壳复用链，不要先去改 `NpcWorldHintBubble.cs`
  3. 若后续真要把头像 fallback 做成可交付状态，应改成 speaker-aware 或至少 neutral placeholder，而不是继续写死 `NPC001`
## 2026-04-07｜Workbench 交互闭环继续深收一刀

- 当前主线目标不变，仍是把 Day1 Workbench 从“能看”推进到“队列、领取、悬浮、多配方更接近闭环”，不回漂去改任务卡或 Town 其他 UI。
- 本轮子任务：
  1. 继续只守 `SpringDay1WorkbenchCraftingOverlay.cs / CraftingStationInteractable.cs / SpringDay1Director.cs`
  2. 把队列切换、悬浮卡聚合、按钮提示错态、E 提示一致性继续往玩家面语义收
- 本轮已完成：
  1. `SpringDay1WorkbenchCraftingOverlay.cs`
     - `CraftRoutine()` 改成走 `HasPendingCrafts()` + `FindNextPendingQueueEntry()`，当前 recipe 做完后会切到下一个待做 entry，不再靠内联扫描硬写
     - `BuildRowSummary()` 现在会直接反映 recipe 当前状态：`排队 n个 / 进度 x/y / 制作完成 n个`
     - `BuildCraftButtonLabel()` 去掉了非当前 recipe 在制作中的“查看队列 / 队列中”杂讯文案；当前只保留真正有动作语义的 `追加制作 / 加入队列 / 中断制作`
     - `UpdateQuantityUi()` 补了 hover 态守卫，避免“切到别的 recipe 时制作按钮 hover 误变红”
     - 悬浮卡链新增 `FloatingProgressDisplayState`，并通过 `BuildFloatingProgressStates()` 按 `resultItemId` 聚合；相同产物的多 recipe 现在会并到同一个悬浮卡里
     - 悬浮卡视觉基线进一步收紧：icon 区占比更大，底部进度条保留，卡片仍按 3x2 矩阵显示
  2. `CraftingStationInteractable.cs`
     - `GetInteractionHint()` 现在会跟 overlay 开关状态同步；工作台已开时统一回 `关闭工作台`
  3. `SpringDay1Director.cs`
     - `BuildWorkbenchCraftProgressText()` 改成 `工作台制作中 · 配方名 · 进度 ready/total · percent%`，不再用容易误解的“当前第几个”
- 本轮验证：
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --skip-mcp` => `owned_errors=0`
  - `validate_script Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs --skip-mcp` => `owned_errors=0`
  - `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs --skip-mcp` => `owned_errors=0`
  - 三次结果都仍是 `assessment=unity_validation_pending`，根因还是本机 `CodexCodeGuard timeout`，不是这轮 own compile red
  - `git diff --check` 对上述 3 个脚本均无新的文本坏口；只有 `WorkbenchOverlay.cs` 既存 `CRLF -> LF` warning
- 当前判断：
  1. 这轮已经不再只是“补语义占位”，而是把多 recipe 队列和多悬浮真正往可消费状态推进了一层
  2. 还没拿到 fresh live，所以现在只能 claim 到“代码层 own red 清、玩家面状态矩阵更完整”，还不能 claim 到“最终体验闭环已验完”
- 当前恢复点：
  1. 下一刀优先继续用 live 现场核对：悬浮卡聚合后的排版、ready/total 文案、进度条颜色与 pickup/cancel 的真实手感
  2. 如果用户再报 Workbench 交互不对，优先回看 `OnCraftButtonClicked / OnProgressBarClicked / UpdateFloatingProgressVisibility` 三条链，不要再回到左列材质旧问题
## 2026-04-07｜Workbench 进度条根因只读补记

- 用户 fresh 截图继续坐实：当前 Workbench 的主要坏点已从“左列内容缺失”切到“进度条状态机显示错误”。
- 这轮只读复核后，已确认 3 个高优先级根因：
  1. `SpringDay1WorkbenchCraftingOverlay.cs` 里主进度条与悬浮进度条的 fill 都用 `Image.Type.Filled`，但没有有效 sprite；`fillAmount` 不能稳定形成玩家可见的 0~100 单件动画。
  2. prefab `SpringDay1WorkbenchCraftingOverlay.prefab` 当前把 `progressLabelText` 绑定到 detail column 下的独立 `ProgressLabel`，并不在 `ProgressBackground` 内；代码 `TryBindRuntimeShell()` 也没有把它纠正回条内 overlay。
  3. `UpdateProgressLabel()` 虽然已经在代码里区分“制作中 / 领取 / 中断 / 排队”，但这些文字都被写到错误节点上，所以 hover 还像是对的，常驻条内文案却始终不成立。
- 当前判断：
  - 这不是再调 alpha 或层级就能过线的问题，而是 `fill 实现错误 + label 节点绑定错误` 两条主链同时坏了。
- 建议下刀顺序：
  1. 先把进度 fill 改成真正可裁切的实现
  2. 再给 `progressRoot` / 浮动卡条创建或重挂专属条内 label
  3. 最后再统一主条与悬浮条的状态机文案
## 2026-04-07｜Workbench 进度条闭环已开始实修

- 这轮已从只读定位进入真实施工，仍只改 `SpringDay1WorkbenchCraftingOverlay.cs`。
- 已落地的关键修复：
  1. 新增 runtime progress fill sprite，并通过 `EnsureProgressFillGraphic(...)` 统一补到主条、悬浮条和浮动卡，修正此前 `Image.Type.Filled` 无 sprite 导致的“fillAmount 不像真实进度动画”问题。
  2. 新增 `EnsureProgressLabelBinding()`，把 `progressLabelText` 强制绑回 `progressRoot` 内部条内节点；旧 prefab 里那个条外 `ProgressLabel` 若仍被序列化引用，会在运行时被隐藏。
  3. `TryBindRuntimeShell()`、`EnsurePrefabDetailTextChain()`、`UpdateProgressLabel()`、`UpdateFloatingProgressVisibility()` 都已接上这两条守卫，防止 runtime 复用旧壳继续回到错误节点。
- 当前验证：
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --skip-mcp` => `owned_errors=0`
  - `git diff --check` 仅既存 `CRLF -> LF` warning
- 当前判断：
  - 进度条的两条主根因已经开始被真实修复，但这轮还没拿到 fresh live 证据，因此还不能 claim 用户体验已完全闭环。
- 2026-04-08 只读补记：Workbench “中断制作导致全队列一起停/清” 的主根因已压实到 `SpringDay1WorkbenchCraftingOverlay.cs`
  - `CancelActiveCraftQueue()`（4270-4306）会直接停掉全局 `_craftRoutine` 并清空 active 状态；
  - 但它不会像 `CraftRoutine()`（3784-3882）那样在当前 entry 结束后调用 `FindNextPendingQueueEntry()`（4197-4208）续跑下一条 pending recipe；
  - 这意味着中断当前 active recipe 后，其他 recipe 的 pending entry 会一起失去驱动；
  - 若随后 overlay `Hide/OnDisable`，`CleanupTransientState()`（400-445）又会因 `HasWorkbenchFloatingState`（161-163）不把“仅 pending”算存活态而 `_queueEntries.Clear()`，于是用户会看到“整个队列都没了”。
  - 材料语义同时确认：
  - `CraftingService.TryCraft(recipe, false)`（406-475）是在每件开做前立刻扣单件材料；
  - `RefundReservedCraftMaterials()`（4329-4351）取消时只返还当前 reserve 的那一件；
  - `CancelQueuedRecipeEntry()`（4308-4327）不返材料，因为未开做的 queued 件本来就还没扣。

## 2026-04-08｜Workbench 全局态投射残留只读审查

- 当前主线目标：
  - 继续只收 `Workbench` 的 recipe 行 / 底部条 / 悬浮卡状态隔离闭环，不回漂其他 UI 面。
- 本轮子任务：
  - 只读审查 `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`，专门找“已经做过 entry-state 隔离后，仍可能把全局 active/queue 残影投到别的 recipe”的剩余点；本轮不改代码。
- 本轮已完成：
  1. 逐段复核了这些入口和 helper：
     - `Open()`
     - `UpdateQuantityUi()`
     - `UpdateProgressLabel()`
     - `BuildFloatingProgressStates()`
     - `BuildCraftButtonLabel()`
     - `GetMaterialPreviewQuantity()`
     - `PushDirectorCraftProgress()`
     - `GetRemainingCraftCount()`
     - `GetQueuedCraftCountAfterCurrent()`
     - `TryPickupSelectedOutputs()`
     - `CancelActiveCraftQueue()`
     - `CancelQueuedRecipeEntry()`
     - `CraftRoutine()`
     - `StopCraftRoutine()`
  2. 连同以下字段的全部使用点一起回看：
     - `_craftingRecipe`
     - `_activeQueueEntry`
     - `_craftProgress`
     - `_hasReservedActiveCraft`
     - `_craftQueueTotal / _craftQueueCompleted`
     - `_lastCompletedQueueTotal / _lastCompletedRecipeId`
  3. 当前高置信剩余串味点已收敛为 6 类：
     - `PushDirectorCraftProgress()` 仍把调用方 `recipe` 和全局 active 计数/进度混发；
     - `BuildFloatingProgressStates()` 仍按 `resultItemId` 聚合，跨 recipe 共用一张悬浮卡；
     - `Open()` 仍会被全局 active / ready 状态强行改写当前选中 recipe；
     - `UpdateQuantityUi() + BuildCraftButtonLabel()` 仍把 station-global busy 语义直接投到当前 footer；
     - `GetMaterialPreviewQuantity()` 仍通过 `GetQueuedCraftCountAfterCurrent() -> GetRemainingCraftCount()` 隐式读取全局 active queue 余量；
     - `_lastCompletedQueueTotal / _lastCompletedRecipeId` 仍是单槽完成残影，且 `CraftRoutine()` 收尾会把多 recipe `successCount` 压到最后一个 `recipeId` 上。
- 当前判断：
  - `row summary` 本体已经比上一刀干净得多；剩余真主嫌不在左列摘要，而在 `Open` 的焦点恢复、footer CTA、悬浮卡聚合，以及往 `SpringDay1Director` 推外部状态这几条仍保留“全局镜像”的链上。
- 当前最薄弱点：
  - `BuildCraftButtonLabel()` 这条是不是 bug，取决于产品定义。
  - 如果接受“工作台只要正在忙，任何 recipe 的动作区都进入 queue 语义”，那它是设计。
  - 如果要 footer 完全 recipe-local，它就是剩余串味点。
- 当前恢复点：
  - 如果用户下一轮要进实修，最小顺序应是：
    1. `PushDirectorCraftProgress()`
    2. `BuildFloatingProgressStates()`
    3. `Open() / GetMaterialPreviewQuantity()`
    4. `UpdateQuantityUi() / BuildCraftButtonLabel()`
  - `_lastCompleted...` 建议作为尾刀顺手清掉，不必抢在前 4 条之前。

## 2026-04-08｜接到新续工：Day1 打包字体链优先，启动卡顿不再误吞
- prompt 文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\2026-04-08_UI线程_day1最终闭环前打包字体与启动链prompt_07.md`
- 当前主线更新为：
  1. 优先收死打包后字体异常/缺字/显示不正常；
  2. 继续验证编辑器与打包版字体链一致性；
  3. 不再把 `PersistentPlayerSceneBridge.Start()` / `NavGrid2D.RebuildGrid()` 这类启动大卡顿主峰误判成 UI 主责。

## 2026-04-08｜Day1 打包字体链首刀已落地：不再在空 atlas 状态下提前判死动态中文字体

- 当前主线目标：
  - 只收 `Day1` 打包后中文字体缺字/显示异常，以及编辑器与打包版字体链不一致。
- 本轮完成：
  1. 改 `Assets/YYY_Scripts/Story/Dialogue/DialogueChineseFontRuntimeBootstrap.cs`：
     - `WarmAndValidate()` 改为统一走 `TryPrepareCharacters(...)`；
     - `TryPrepareCharacters(...)` 不再先用 `HasUsableAtlas()` 直接把字体判死；
     - 在 atlas 为空时先给动态字体一次预热/补字机会，再检查 atlas 与 `HasCharacters(...)`；
     - 新增 `TryAddCharactersSafe(...)`，把预热和实际补字统一到同一条安全入口。
  2. 补强 `Assets/YYY_Tests/Editor/DialogueChineseFontRuntimeBootstrapTests.cs`：
     - 用 `TMP_FontAsset.ClearFontAssetData(true)` 构造 build-like 的“动态 atlas 已清空”初始态；
     - 断言这时 `CanRenderText(...)` 仍能补回中文 probe text 并重新拿到可用 atlas。
  3. 只读核实 5 份中文字体资产：
     - `DialogueChinese V2 / Pixel / SoftPixel / SDF / BitmapSong`
     - 都仍是 `m_AtlasPopulationMode: 1`
     - 且都带 `m_ClearDynamicDataOnBuild: 1`
     - 说明这轮修的是 build 真差异点，不是盲修。
- 当前验证：
  - `validate_script` 针对：
    - `Assets/YYY_Scripts/Story/Dialogue/DialogueChineseFontRuntimeBootstrap.cs`
    - `Assets/YYY_Tests/Editor/DialogueChineseFontRuntimeBootstrapTests.cs`
    均为：
    - `owned_errors=0`
    - `external_errors=0`
    - `assessment=unity_validation_pending`
  - `git diff --check` 针对这两文件通过。
  - `sunset_mcp.py baseline` = `pass`
  - `sunset_mcp.py status` 显示当前 fresh console 仍有 1 条外部红：
    - `Assets/YYY_Tests/Editor/NpcResidentDirectorRuntimeContractTests.cs(104,20)` 缺 `NPCAutoRoamController`
- 当前判断：
  - 这轮已把“build 动态中文字体在 atlas 被清空后，还没来得及补字就先被判死”的核心逻辑口收住；
  - 已拿到代码层 + build-like 编辑器测试证据；
  - 但还没拿到 packaged build 真机/真包 fresh 终验，不能偷报“打包体验完全过线”。
- 当前恢复点：
  1. 若下轮继续 UI 主刀，优先补 packaged build / runtime 侧真实字体证据；
  2. 若受外部红阻挡，就把 `NpcResidentDirectorRuntimeContractTests.cs` 记为第一真实 blocker，不回漂 workbench 或泛 UI 修补。

## 2026-04-08｜补刀：箱子 E 键已收成 toggle 闭环，fresh console 回到 0 error

- 当前新增闭环点：
  1. `ChestController.OpenBoxUI()` 不再在“同一个箱子已经打开”时静默无响应；
  2. 现在再次命中同箱交互会直接 `Close()`，把 `OnInteract()` 真正收成开/关同源 toggle；
  3. `ReportProximityInteraction(...)` 现在允许“同箱已打开”这个唯一例外继续上报 proximity candidate；
  4. `SpringDay1ProximityInteractionService` 新增 `AllowWhilePageUiOpen` 例外语义，同箱页打开时不会再被全局 page-open 阻塞链直接清掉候选；
  5. 底部提示也改成了关闭语义：
     - `关闭箱子`
     - `按 E 关闭箱子`
- 当前验证：
  - `validate_script` 针对：
    - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
    - `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
    - `Assets/YYY_Tests/Editor/ChestProximityInteractionSourceTests.cs`
    仍是：
    - `owned_errors=0`
    - `external_errors=0`
    - `assessment=unity_validation_pending`
  - `sunset_mcp.py status` 当前 fresh console：
    - `error_count=0`
    - `warning_count=2`
- 当前判断：
  - 箱子这条线现在已经不是“能开不能关”的半截 E 键；
  - 它已经收成“同一真入口负责开/关，同箱打开时 proximity 也能继续保持 toggle 候选”的闭环。

## 2026-04-08｜补记：已生成给 farm 和 UI/day1 的阶段回执 prompt

- 新落地文件：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026-04-08_给farm_箱子E键toggle闭环与live终验回执prompt_03.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\2026-04-08_UI线程_day1打包字体链与箱子toggle阶段回执prompt_08.md`
- 当前用途：
  - 前者用于把 `farm` 后续工作压回“只做箱子 E 键 toggle 的 fresh runtime/live 终验”；
  - 后者用于把 `UI/day1` 当前阶段结论压成可直接转发的阶段回执，不再回漂到泛 UI 修补。

## 2026-04-09｜Package 地图页 / 关系页只读审查：当前主问题是“新壳叠旧壳 + 比例硬编码 + 外部 overlay 未统一退场”

- 当前主线目标：
  - 回到 UI 主线，先只读核实 Package 里地图页、NPC 关系页以及包裹面打开后的玩家面遮挡问题，不直接写代码。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `delivery-self-review-gate`
- `sunset-startup-guard`、`sunset-workspace-router` 当前会话未显式暴露；已按 Sunset / 全局 `AGENTS.md` 手工完成等价前置核查，并保持只读边界，因此本轮未跑 `Begin-Slice`。
- 本轮读取与核实：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Tabs\PackagePanelTabsUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Tabs\PackageMapOverviewPanel.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Tabs\PackageNpcRelationshipPanel.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Tabs\PackagePanelRuntimeUiKit.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1UiLayerUtility.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
- 本轮站稳的结论：
  1. `PackagePanelTabsUI.OpenPanel()` 只负责打开包裹页并显示 `Main/Top`，没有统一压掉外部 `Prompt / 任务卡` 这类 overlay；而 `SpringDay1PromptOverlay` 只是“下次刷新时检测 page-open 再延迟显示”，因此已经亮着的任务卡会留在背后，不会在包裹页打开瞬间被强制退场。
  2. `PackageMapOverviewPanel` 与 `PackageNpcRelationshipPanel` 都只会删除自己上次创建的 `RuntimeRootName`，没有任何“隐藏 legacy 子节点 / 清空旧 page 内容”的动作；所以当前非常像“新 runtime 壳加上去了，但原 page 里旧节点还活着”，这和玩家截图里“旧字/旧块透出来”的现象同源。
  3. 地图页的右栏比例现在是硬编码三块卡片高度：
     - `OverviewCard`
     - `PresenceCard`
     - `RouteCard`
     结果是信息量并不大，却占了很重的纵向空间，右栏空白感和占位感都很重。
  4. 地图主体的 `VillageZone / RidgeZone` 背景块透明底现在过大、过实，会把路线与点位的主语义压虚。
  5. 关系页右侧详情是 `VerticalLayoutGroup + 多块固定 preferred/min height` 的组合，`HeroCard / StageCard / NarrativeRow / FooterCard` 全都在抢有限高度；当前设计在不同分辨率和文本长度下很容易出现挤压、串层或“像测试板”的观感。
- 当前判断：
  - 这不是“某一个字距或锚点没调好”的级别，而是三个根因叠在一起：
    1. `Package` 打开时没有统一处理外部玩家面 overlay；
    2. `Map / Relationship` 的 runtime 页面没有把 legacy 内容退场；
    3. 页面内部大量靠固定高度撑结构，没围绕真实文本密度做收敛。
- 下一步推荐顺序：
  1. 先收 `Package` 打开时的 overlay 抑制闭环；
  2. 再收地图页 / 关系页的 legacy 内容退场；
  3. 然后才修地图右栏比例和关系页详情布局。
- 验证状态：
  - `静态推断成立`
  - `截图反馈 + 代码只读核实已对上`
  - `尚未进入 live 修复`
- 当前恢复点：
  - 如果下一轮继续真实施工，第一刀不要泛修别的 UI；
  - 只收 `Package` 这三件事：
    - 打开时外部 overlay 退场
    - 地图/关系页旧内容退场
    - 地图/关系页结构比例纠偏

## 2026-04-09｜任务清单 vs Toolbar 处理链只读审查

- 当前主线目标：
  - 用户怀疑 `SpringDay1PromptOverlay`（任务清单）整体处理链做错了，希望它和 `ToolBar` 一样按同级 persistent UI 处理；这轮只做只读审查，不改代码。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
- `sunset-startup-guard` 当前会话未显式暴露；已按 Sunset / 全局 `AGENTS.md` 手工完成等价前置核查。本轮只读，因此未跑 `Begin-Slice`。
- 本轮核实文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Tabs\PackagePanelTabsUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Toolbar\ToolbarUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PersistentPlayerSceneBridge.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
  - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Town.unity`
- 已坐实的判断：
  1. `ToolBar` 和 `SpringDay1PromptOverlay` 在 scene hierarchy 上确实都是 `UI` 根下的直接孩子，用户说“想让它们是同一级别关系”这一点从层级上本来就成立。
  2. 但实现链并不一样：`SpringDay1PromptOverlay` 自己带独立 `Canvas + CanvasGroup`，并在 `ApplyRuntimeCanvasDefaults()` 里强制 `ScreenSpaceOverlay + overrideSorting + sortingOrder=152`；它不是 `ToolBar` 那种跟着 persistent UI 大 Canvas 一起活的普通兄弟面板。
  3. `PackagePanelTabsUI` 打开背包时也会再给 `PackagePanel` 自己补一个独立 `Canvas`，强制 `sortingOrder=181`，然后额外调用 `promptOverlay.SetExternalVisibilityBlock(...)` 去压任务清单；说明当前任务清单显隐依赖“外部面板逐个通知它退场”，而不是像 `ToolBar` 一样走统一父层规则。
  4. `PersistentPlayerSceneBridge` 在 hierarchy 边界跟踪里把 `ToolBar` 和 `SpringDay1PromptOverlay` 都当作 `persistentUiRoot` 直接孩子看待，但这只是层级/边界层面的“同级”，不代表渲染和治理链也统一。
  5. 所以用户这次的怀疑是成立的：任务清单现在不是“和 toolbar 一样处理”，而是“看起来同级、实际上是独立 overlay canvas，再靠 suppress 补丁和其它 UI 协调”。
- 当前阶段判断：
  - 这轮只站住了“结构真相”，还没有进入改造方案落地；
  - 现阶段最稳的结论不是“它一定要物理并回 toolbar 那条 Canvas”，而是“当前治理链确实分裂，后续要么统一父层规则，要么把 overlay 退场逻辑集中化，不能再散在各 UI 里各自压它”。
- 验证状态：
  - `静态推断成立`
  - `代码只读核实已对上`
  - `尚未进入 live 修复`
- 当前恢复点：
  - 如果下一轮继续真实施工，应先针对 `PromptOverlay` 做“同级治理收口”设计，而不是继续叠新的 suppress 例外。

## 2026-04-09｜任务清单治理收口：改成更像 Toolbar 的父层口径

- 当前主线目标：
  - 把 `SpringDay1PromptOverlay` 收成更像 `ToolBar` 的同级治理口径，只改显示治理，不碰任务数据和剧情推进。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Tabs\PackagePanelTabsUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
- 本轮做成了什么：
  1. `PromptOverlay.ApplyRuntimeCanvasDefaults()` 现在优先对齐父级 `UI` 根 Canvas：
     - 有父级 Canvas 时，不再强制 `overrideSorting=true + sortingOrder=152`
     - 改为继承父级 `renderMode / worldCamera / planeDistance / sortingLayer / sortingOrder / pixelPerfect`
     - 只有找不到父级 Canvas 时，才回退到独立 overlay fallback
  2. `PromptOverlay.ShouldDelayPromptDisplay()` 不再把 `Package/Box` 打开本身当成“任务清单必须自己消失”的理由；包裹页不再靠这条自我 suppress 逻辑压任务清单。
  3. `PackagePanelTabsUI` 里原来那条散落的 `SyncExternalOverlaySuppression(true/false)` 调用已经删掉：
     - 打开背包
     - Box 模式打开背包壳
     - 关闭背包
     都不再手动通知 `PromptOverlay` 隐藏/恢复。
  4. 额外补了一个最小守门测试：
     - `PromptOverlay_UsesParentCanvasGovernance_WhenUiRootCanvasExists`
     - 用来锁住“挂在 UI 根 Canvas 下时，任务清单不应继续强制独立排序”。
- 当前判断：
  - 这刀已经把“任务清单不是和 toolbar 同口径”的根因往前推了一大步；
  - 现在它仍保留自己的 `Canvas`，但默认已回到“优先服从父级 UI 根 Canvas”的口径，Package 也不再继续散着 suppress 它。
- 验证状态：
  - `git diff --check`：
    - `SpringDay1PromptOverlay.cs` 通过，仅有既有 `CRLF/LF` warning
    - `PackagePanelTabsUI.cs` 通过
    - `SpringDay1LateDayRuntimeTests.cs` 通过
  - `sunset_mcp.py doctor`：`baseline pass`
  - `sunset_mcp.py compile`：被 `subprocess_timeout:dotnet:60s` 卡住，尚未拿到 fresh compile 结论
  - `manage_script validate`：当前 Unity listener 存在，但 `active Unity instance` 未就绪，不能给 fresh validation
- 当前恢复点：
  - 用户现在可以直接先测“对话/背包/包裹页下任务清单的关系是否回正”；
  - 如果还有 residual 问题，下一刀优先只收剩余的显示时机，不回漂到任务内容本身。

## 2026-04-09｜任务清单模态治理补口：背包/包裹页打开时按统一 utility 退场

- 当前主线目标：
  - 把“背包/包裹页打开时任务清单仍屹立不下”的 residual 收掉，继续只改显示治理，不碰任务数据和剧情推进。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1UiLayerUtility.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
- 本轮做成了什么：
  1. `SpringDay1UiLayerUtility` 新增 `ShouldHidePromptOverlayForParentModalUi()`，把“背包/包裹类页面是否应压任务清单”的判断集中回 utility。
  2. `PromptOverlay.ShouldDelayPromptDisplay()` 重新接回这条统一 utility 判断：
     - 包裹页 / 背包壳 / 箱子页打开时，任务清单会退场
     - 关闭后恢复
     - 不再依赖 `PackagePanel` 到处手动调用 suppress。
  3. 新增 runtime 测试 `PromptOverlay_HidesWhilePackagePanelIsOpen_AndRestoresAfterClose`，锁住“包裹页开 -> 隐藏；包裹页关 -> 恢复”这条体验闭环。
- 当前判断：
  - 这次补口后，结构上已经变成：
    - 父级 Canvas 口径统一
    - 模态隐藏逻辑集中在 utility
    - 不再是 `PackagePanel -> PromptOverlay` 的散链压制
  - 这仍然只影响显示时机，不应影响任务状态和剧情阶段推进。
- 验证状态：
  - `git diff --check`：
    - `SpringDay1UiLayerUtility.cs` 通过
    - `SpringDay1LateDayRuntimeTests.cs` 通过
    - `SpringDay1PromptOverlay.cs` 通过，仅有既有 `CRLF/LF` warning
  - fresh compile 仍未闭环：
    - `sunset_mcp.py compile` 上轮已被 `subprocess_timeout:dotnet:60s` 卡住
    - 本轮未拿到新的 compile 结果
- 当前恢复点：
  - 用户现在应优先重测：
    1. 正式对话时任务清单是否隐藏
    2. 打开背包/包裹页时任务清单是否隐藏
    3. 关闭后是否稳定恢复

## 2026-04-09｜Workbench 跨场景显示链止血 + HP/EP 分辨率自适应补口

- 当前主线目标：
  - 继续只收两个运行时 UI bug：
    1. `Workbench` 制作完成悬浮/产出提示跨场景穿到 `Home`
    2. 正式打包后 `HP/EP` 条在不同分辨率下跑出可视范围
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- thread-state：
  - 沿用已存在的 `ACTIVE slice = fix-workbench-floating-cross-scene-leak`
  - 收尾前已跑 `Park-Slice`
  - 当前状态：`PARKED`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\HealthSystem.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\EnergySystem.cs`
- 本轮做成了什么：
  1. `WorkbenchOverlay` 新增 scene-aware 显示守卫：
     - 记住当前工作台状态所属 scene
     - `sceneLoaded / activeSceneChanged` 时，如果玩家切到别的 scene，只断开 `_anchorTarget / _playerTransform / 浮窗显示`
     - 保留 `_queueEntries / readyCount / active craft` 这组工作台内部状态，不再把“跨场景隐藏”误做成“直接清空产物”
  2. `HasActiveCraftQueue / HasReadyWorkbenchOutputs / HasWorkbenchFloatingState` 现在只会在状态所属 scene 内对外暴露：
     - 别的 scene 不再看到旧工作台的悬浮/完成提示
     - 回到原 scene 时，原工作台队列仍可继续接回
  3. `HealthSystem / EnergySystem` 补了运行时布局矫正：
     - 首次绑定时记住场景原始 anchoredPosition
     - 每次分辨率变化或首次初始化时，都先恢复原始位置，再按 root canvas 可视范围做 clamp
     - 目标不是改条样式，而是保证打包后不同分辨率下两条都不会飞出屏幕
- 当前判断：
  - `Workbench` 这刀现在收成了“跨场景不再漏显示，但不丢原工作台状态”
  - `HP/EP` 这刀现在收成了“保留现有 scene 样式，只补运行时安全布局”
  - 这两刀都属于 `结构 / targeted probe` 已成立，`真实 build 体验` 还需要用户 live 终验
- 验证状态：
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 / external_errors=0`
  - `validate_script Assets/YYY_Scripts/Service/Player/HealthSystem.cs Assets/YYY_Scripts/Service/Player/EnergySystem.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 / external_errors=0`
  - `sunset_mcp.py errors --count 20 --output-limit 10`
    - `errors=0 warnings=0`
  - `git diff --check`
    - 这轮 touched 文件无新的阻断错误
    - `SpringDay1WorkbenchCraftingOverlay.cs` 仍有既有 `CRLF/LF` 提示，不是 red
- 当前恢复点：
  - 用户下一步应优先手测：
    1. Primary 做出工作台产物后切到 Home，确认悬浮/完成提示不会再跟过去
    2. 回到原工作台 scene，确认旧队列/已完成产物仍能继续接回
    3. 用不同分辨率启动 build，确认 `HP/EP` 条都仍在屏内

## 2026-04-09｜PromptOverlay 回到父级基础层，不再因背包打开而 fade

- 当前主线目标：
  - 用户明确驳回“开背包时任务清单靠透明度变化/隐藏解决”的做法，要求把 `PromptOverlay` 真正收成和 `toolbar / state` 同语义的基础层。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- thread-state：
  - 已跑 `Begin-Slice`
  - 收尾前已跑 `Park-Slice`
  - 当前状态：`PARKED`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
- 本轮做成了什么：
  1. `PromptOverlay.ApplyRuntimeCanvasDefaults()` 现在在有父级 `UI` Canvas 时：
     - 继续继承父级 renderMode / camera / pixelPerfect
     - 但不再 `overrideSorting = true`
     - 也不再把自己排序抬到父级之上
     - 直接回到父级基础层口径
  2. `ShouldDelayPromptDisplay()` 不再把 `Package/Box` 打开当成任务清单必须 fade/隐藏的理由。
  3. 运行时测试一起翻面：
     - 不再锁“Prompt 必须独立高层排序”
     - 改为锁“Prompt 属于父级基础层，背包属于更高模态层，但 Prompt 自己不 fade”
- 当前判断：
  - 这刀的本体不是“让任务清单消失”，而是“让它从错误的特殊 overlay 身份退回基础 UI 层”。
  - 现在背包打开时，它应像 `toolbar / state` 一样继续待在底层；是否被盖住，交给真正更高层的背包 UI 自己负责。
- 验证状态：
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
    - `assessment=no_red`
    - `owned_errors=0 / external_errors=0`
  - `sunset_mcp.py errors --count 20 --output-limit 10`
    - `errors=0 warnings=0`
  - `git diff --check`
    - 无新的阻断错误
    - `SpringDay1PromptOverlay.cs` 仍有既有 `CRLF/LF` 提示，不是 red
- 当前恢复点：
  - 用户现在应该直接看：
    1. 打开背包时任务清单是否不再自己变透明
    2. 任务清单是否像 `toolbar / state` 一样只是处在基础层
    3. 与背包重叠时是否由背包层自己盖住它，而不是任务清单自己退场

## 2026-04-09 20:31:51 +08:00｜PromptOverlay 父层修正补刀：从“挂错子 Canvas”收回基础层 Canvas

- 当前主线目标：
  - 用户继续追打 `PromptOverlay` 层级，明确要求它不是靠透明度变化解决，而是必须像 `toolbar / state` 一样天然待在更底层，由背包背景自己盖住。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- thread-state：
  - 已跑 `Begin-Slice`
  - 收尾前已跑 `Park-Slice`
  - 当前状态：`PARKED`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
- 本轮做成了什么：
  1. `PromptOverlay.EnsureBuilt()` 现在每次运行时刷新都会先：
     - 重新校正自己挂载到首选基础层父节点
     - 重新应用 runtime canvas 默认值
     - 不再因为旧绑定还活着就跳过父层纠偏
  2. `ResolveParent()` 不再盲目吃 `UI` 下第一个子 Canvas；
     - 现在会优先挑基础层 UI Canvas
     - 明确规避 `PackagePanel / Dialogue / Workbench / Prompt / Tooltip` 这类模态或错误目标 Canvas
  3. 新增回归测试：
     - 当 `UI` 根下同时存在基础 Canvas 和 `PackagePanel` 模态 Canvas 时
     - `PromptOverlay` 必须挂到基础 Canvas
     - 即使被误挂到 `PackagePanel`，下一次 `LateUpdate` 也会自动回正
- 当前判断：
  - 上一刀只把“排序口径”收正了一半；
  - 这次补到“父层入口 + 运行时回正”后，才更接近用户要的真正层级语义。
- 验证状态：
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
    - `assessment=no_red`
    - `owned_errors=0 / external_errors=0`
  - `git diff --check`
    - 无新的 blocking diff 错误
    - 仅有既有 `CRLF/LF` warning，不是 red
- 当前恢复点：
  - 用户现在应重点 live 复测：
    1. 打开背包时任务清单是否仍然在基础层，不再自己 fade
    2. 是否真的由背包背景自己盖住它
    3. 是否不再出现“像挂在错误独立 overlay 上”的感觉

## 2026-04-09 20:50:31 +08:00｜只读核实：工作台工具配方、材料 SO 与排序现状

- 当前主线目标：
  - 用户要求先不要动代码，先核实 `SO` 里现有工具 / 材料 / 工作台 recipe 的真值，再确认“工作台直接全部解锁工具配方”到底能闭环到哪一层。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-no-red-handoff` 的只读等价流程
- 本轮性质：
  - 只读分析，未进入真实施工，未跑 `Begin-Slice`
- 本轮核实结果：
  1. `SpringDay1WorkbenchCraftingOverlay` 当前确实只读：
     - `Resources/Story/SpringDay1Workbench`
     - 当前会把 `requiredStation == Workbench` 的 recipe 载入
  2. 当前资源目录里只有 5 个工作台 recipe：
     - `9100 Axe_0`
     - `9101 Hoe_0`
     - `9102 Pickaxe_0`
     - `9103 Sword_0`
     - `9104 Storage_1400`
  3. 当前 4 个工具 recipe 的现有消耗：
     - `Axe_0` = 木料 `3200 x3`
     - `Hoe_0` = 木料 `3200 x2`
     - `Pickaxe_0` = 木料 `3200 x3` + 石料 `3201 x2`
     - `Sword_0` = 木料 `3200 x4` + 石料 `3201 x2`
  4. 材料 SO 当前明确存在：
     - 木料 `3200`
     - 石料 `3201`
     - 黄铜矿 `3000` -> 黄铜锭 `3100`
     - 生铁矿 `3001` -> 生铁锭 `3101`
     - 黄金矿 `3002` -> 黄金锭 `3102`
  5. 工具 SO 当前明确存在：
     - 斧头 `0~5`
     - 镐子 `6~11`
     - 锄头 `12~17`
     - 剑 `200~205`
  6. 风险真值：
     - 工具 `materialTier` 里有 `4=钢质`，但当前材料 SO 没有钢矿 / 钢锭
     - 武器 `Sword_1~5` 的资源本体目前全部还是 `materialTier=0`，而且 `attackPower` 也没按档位拉开，只是耐久变化
  7. 排序现状：
     - 当前工作台 UI 不是纯 `recipeID` 排序
     - 现在是 `Axe -> Hoe -> Pickaxe -> 其他` 的自定义分组后，再按 `recipeID`
- 当前判断：
  - 真正稳定、语义闭环的数据面是：木 / 石 / 生铁 / 黄铜这四层工具最干净；
  - 钢层当前缺材料 SO；
  - 金层虽有矿和锭，但会被钢层缺口卡住；
  - 剑的高阶 SO 目前本体也没配好，不能直接当“完整高阶武器链”处理。
- 当前恢复点：
  - 如果用户下一步要我直接开做，优先把：
    1. 配方排序改成纯 `recipeID`
    2. 补 `Axe / Pickaxe / Hoe` 的扩展 recipe
    3. 再决定是否先修 `Sword_1~5` 的 SO 真值

## 2026-04-09 20:55:40 +08:00｜补记：洒水壶与箱子 SO 真值

- 只读补核结果：
  1. `WateringCan` 当前只有一个工具 SO：`itemID 18`，木质 0 档，没有更高阶水壶资源。
  2. 存储箱子 SO 当前有四个：
     - `1400 小木箱子`（12格）
     - `1401 大木箱子`（24格）
     - `1402 小铁箱子`（36格）
     - `1403 大铁箱子`（48格）
- 当前对用户的可执行口径：
  - 工作台可放入的清单应包含：`Axe/Pickaxe/Hoe` 的 `0/1/2/3/5` 档、`WateringCan`、`Sword_0`、以及四个箱子；
  - `4` 档钢系先跳过；`Sword_1~5` 仍先不放。

## 2026-04-09 22:16:00 +08:00｜工作台 recipe 扩展与本地队列提示条已落地

- 当前主线目标：
  - 继续只收 `Workbench`：补用户已拍板的工具/箱子 recipe，并在“超过 6 项继续排队”时只在工作台内给一个本地提示条。
- 本轮子任务：
  - 改 `SpringDay1WorkbenchCraftingOverlay.cs` 的 recipe 排序与 queue toast；
  - 新增 `Resources/Story/SpringDay1Workbench` 下缺失的 recipe 资产。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮是否进入真实施工：
  - 是；延续已开的 `ACTIVE` slice：`workbench-recipe-expansion-and-local-queue-toast-20260409`
- 本轮做成了什么：
  1. 工作台 recipe 排序已从自定义 `Axe -> Hoe -> Pickaxe -> 其他` 改成按 `resultItemID` 排，玩家面现在是纯物品 ID 顺序。
  2. 工作台内部新增了本地 queue toast：
     - 只挂在工作台面板内部
     - 渐入 `0.25s`
     - 停留 `0.5s`
     - 渐出 `0.25s`
     - 只在当前队列来源数 `> 6` 且继续点击追加制作时出现
  3. 运行时兼容链已补上：
     - 老 prefab 没有这个 toast 节点也会自动补出来
     - `Hide/Cleanup/Rebuild` 都会主动清 toast，不会残留
  4. 新增 recipe 资产共 `16` 个：
     - `Axe_1/2/3/5`
     - `Pickaxe_1/2/3/5`
     - `Hoe_1/2/3/5`
     - `WateringCan`
     - `Storage_1401/1402/1403`
  5. 因此当前工作台总 recipe 清单已扩到：
     - `Axe_0/1/2/3/5`
     - `Pickaxe_0/1/2/3/5`
     - `Hoe_0/1/2/3/5`
     - `WateringCan`
     - `Sword_0`
     - `Storage_1400/1401/1402/1403`
- 本轮刻意没做什么：
  - 没把钢档 `4` 强行补进来，因为当前没有钢材料 SO；
  - 没把 `Sword_1~5` 强行补进来，因为高阶剑 SO 真值还不干净；
  - 没把这轮包装成“体验最终过线”，当前只到结构/局部验证层。
- 验证结果：
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - `owned_errors=0`
    - `assessment=unity_validation_pending`
    - 阻塞点是 Unity 会话当时处于 `compiling/stale_status`，不是脚本 owned red
  - `errors --count 20 --output-limit 10`
    - `errors=0 warnings=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs Assets/Resources/Story/SpringDay1Workbench`
    - 仅有 `CRLF/LF` 提示，无新的 diff red
- 当前恢复点：
  - 下一刀如果继续，应直接做工作台 live 体验验收：
    1. 新 recipe 是否按预期显示
    2. 超过 6 项继续排队时 toast 是否只在工作台内出现
    3. 不同 recipe 混排时左列与悬浮卡是否仍稳定

## 2026-04-09 22:18:10 +08:00｜停车补记

- 当前 `thread-state`：
  - `PARKED`
- 当前 blocker：
  - `user-live-validation-pending`
- 说明：
  - 代码与资源这刀已停在可 live 验的恢复点，后续优先看工作台真实玩家面，不再回到只读猜测。

## 2026-04-09 22:35:55 +08:00｜只读复盘：Workbench 玩家面命名链为什么会坏

- 当前主线目标：
  - 只读核实用户截图里 `WateringCan / 3101_生铁锭 / 3102_黄金锭 / 开发口吻简介` 这些坏文案的真实根因，并给出应该怎么改的判断。
- 本轮子任务：
  - 只读检查 `Workbench` 的显示链、相关 item SO 真值、以及新补 recipe 资产里的文案。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
- 本轮是否进入真实施工：
  - 否，保持只读
- 只读确认到的真相：
  1. `Tool_18_WateringCan.asset` 的 `itemName` 本体就是 `WateringCan`，不是中文。
  2. `Material_3101_生铁锭.asset / Material_3102_黄金锭.asset` 的 `itemName` 本体分别就是 `3101_生铁锭 / 3102_黄金锭`，带内部 ID 前缀。
  3. `SpringDay1WorkbenchCraftingOverlay.BuildMaterialsText()` 现在直接拿 `ResolveItem(...).itemName` 上屏，没有做玩家面清洗，所以材料名会把内部前缀直接展示出来。
  4. `GetRecipeDisplayName()` 现在只对 `axe / pickaxe / hoe / storage / sword` 做了非常粗的内部名兜底，没覆盖 `wateringcan`，而且会把多档工具压成泛名 `斧头/镐子/锄头`，导致多档并排时辨识度也不够。
  5. 我这一轮新补的 recipe `description` 确实写成了开发者口吻，例如：
     - `把工作台里的基础耕种工具链补齐`
     - `把工作台里的金属箱子也补进来`
     - `作为工作台顶阶展示配方保留`
     这些都不应该直接给玩家看。
- 当前判断：
  - 这不是单个词没翻译，而是“把数据层内部字段直接拿来当玩家面文案”的结构性问题；
  - 正确修法不该继续抄 SO 原字段，而应该给工作台补一层玩家面显示名/简介映射；
  - 材料名至少要在显示层统一去掉 `^\d+_` 这种内部前缀。
- 当前恢复点：
  - 下一刀若开做，应先把 `Workbench` 文案链拆成：
    1. 玩家面名称
    2. 玩家面简介
    3. 材料显示名
    再替当前新 recipe 做一版真正可给玩家看的中文文案。

## 2026-04-09 22:59:33 +08:00｜真实施工：Workbench 玩家面文案层已接管内部字段

- 当前主线目标：
  - 继续只收 `Workbench` 玩家面文案，把名称、简介、材料名彻底从内部字段直出改成玩家面映射。
- 本轮子任务：
  - 只修改 `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮是否进入真实施工：
  - 是；新 slice：`workbench-player-facing-text-normalization-20260409`
- 本轮做成了什么：
  1. `GetRecipeDisplayName()` 已改成优先走工作台专用玩家面名称映射，不再直接信任 `recipeName/itemName` 原值。
  2. 新增了工作台玩家面简介映射，`selectedDescriptionText` 不再直接把新 recipe 的开发口吻 `description` 抛给玩家。
  3. `BuildMaterialsText()` 现在统一走材料显示名映射/清洗，不再把 `3101_生铁锭 / 3102_黄金锭` 这种内部前缀直接显示出来。
  4. 当前工作台里这些结果物已经有稳定中文玩家面名：
     - `木斧/石斧/铁斧/黄铜斧/金斧`
     - `木镐/石镐/铁镐/黄铜镐/金镐`
     - `木锄/石锄/铁锄/黄铜锄/金锄`
     - `洒水壶`
     - `木剑`
     - `小木箱子/大木箱子/小铁箱子/大铁箱子`
- 代码层验证：
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --count 20 --output-limit 10`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 / external_errors=0`
    - 卡点仍是 Unity `stale_status`，不是脚本红错
  - `errors --count 20 --output-limit 10`
    - `errors=0 warnings=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - 仅 `CRLF/LF` warning
- 当前恢复点：
  - 下一刀优先做 live 验：
    1. `WateringCan` 是否已经显示为 `洒水壶`
    2. 材料是否显示为 `生铁锭/黄金锭`
    3. 各档工具名是否已可区分

## 2026-04-09 23:00:30 +08:00｜停车补记

- 当前 `thread-state`：
  - `PARKED`
- 当前 blocker：
  - `user-live-validation-pending`

## 2026-04-09｜父层补记：Spring UI 性能问题根因已做只读分层

- 当前只读确认：
  1. `SpringDay1WorkbenchCraftingOverlay` 内部存在密集的 `ForceRebuildLayoutImmediate`、`ForceMeshUpdate`，且 `LateUpdate()` 常驻调用 `UpdateFloatingProgressVisibility()`；
  2. `SpringDay1PromptOverlay` 同样在 `LateUpdate()` 常驻建 state，并在多条链上做文本/布局立即重建；
  3. `CraftingStationInteractable.Update()` 每帧 `BuildProximityInteractionContext()` + `ReportCandidate()`，会放大提示/UI 事件刷新；
  4. 结合用户 Profiler 现场，当前最大嫌疑是 `EventSystem.Update + PlayerUpdateCanvases + UI 强制重建风暴`，不是单个 recipe 逻辑判断。
- 父层判断：
  - 正确第一刀应先打“无效刷新、重复上报、每帧强制重建”，而不是为了压峰值去砍功能、简化交互或偷弱化表现。
- 下一步建议：
  - 先做 quick-fix 止血层：dirty-flag、last-state 缓存、状态无变化不重建；
  - 再做全量刷新模型收口。

## 2026-04-10｜父层补记：刷新风暴止血第一刀已落地

- UI 子线程本轮已把第一刀落在 4 个文件：
  - `SpringDay1WorkbenchCraftingOverlay.cs`
  - `SpringDay1PromptOverlay.cs`
  - `CraftingStationInteractable.cs`
  - `SpringDay1ProximityInteractionService.cs`
- 已落地的止血点：
  1. Proximity 提示内容签名缓存，文案不变时不再每帧重复 `ShowPrompt()`；
  2. 工作台交互端缓存 player / overlay 引用，减少每帧查找；
  3. PromptOverlay 被 formal dialogue / modal 压住时，不再先白建 state；
  4. Prompt / Workbench 的文本网格刷新改成“内容变了才强刷”；
  5. Workbench 制作中逐帧刷新改成更轻的 runtime progress 更新，不再每帧整块数量区重跑；
  6. Workbench 悬浮卡状态改成 buffer / pool 复用，去掉逐帧 `new List + LINQ 排序结果` 分配。
- 当前验证状态：
  - 4 个目标脚本 `validate_script` 均为：
    - `assessment=unity_validation_pending`
    - `owned_errors=0 / external_errors=0`
  - `git diff --check`：
    - 仅 `CRLF/LF` warning
- 当前仍待补：
  - `errors` / fresh console 被本机 MCP baseline 阻塞，live 还未闭环。
