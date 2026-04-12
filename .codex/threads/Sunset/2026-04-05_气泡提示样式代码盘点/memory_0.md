## 2026-04-05｜只读盘点：Sunset 当前 `气泡 / 提示` 样式种类与代码宿主

- 用户目标：
  - 在 `D:\Unity\Unity_learning\Sunset` 仅读代码，盘点当前“气泡/提示”相关的样式种类与代码宿主，不做任何修改；重点说明类/文件列表、是否有明确样式 `preset / visual kind / mode`、从代码角度到底算几种样式，以及哪些属于 `NPC own`、哪些属于 `shared/UI/day1`。
- 当前主线目标：
  - 给出一份以代码为准的静态盘点，而不是治理文档口径或运行时主观判断。
- 本轮子任务 / 阻塞：
  - 子任务是把真正负责“画样式”的宿主类，与只是“喂内容 / 切状态”的驱动类分开。
  - 本轮没有施工，也没有 Unity / 编译 blocker。
- 已完成事项：
  1. 只读盘点了核心宿主：
     - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
     - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
     - `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
     - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
     - `Assets/YYY_Scripts/Story/UI/NpcWorldHintBubble.cs`
     - `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
     - `Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs`
  2. 只读盘点了关键驱动 / 调用链：
     - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
     - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
     - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
     - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
     - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerToolFeedbackService.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerNpcNearbyFeedbackService.cs`
  3. 只读复核了辅助证据测试：
     - `Assets/YYY_Tests/Editor/PlayerThoughtBubblePresenterStyleTests.cs`
     - `Assets/YYY_Tests/Editor/PlayerNpcConversationBubbleLayoutTests.cs`
     - `Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`
- 关键决策：
  1. 主计数口径采用：
     - 只算“代码里能直接切出、且视觉参数确实变化”的 runtime 样式
     - 不把纯优先级 / 排序 / 焦点模式自动当成独立视觉样式
     - 不把单纯“有无 detail 导致尺寸变化”的适配态全部无限拆分
  2. 按这个口径：
     - `NPCBubblePresenter` = 1 套 NPC 气泡样式；`ReactionCue` 是显式 mode，但不是独立视觉壳
     - `PlayerThoughtBubblePresenter` = 1 套玩家思考气泡样式
     - `InteractionHintOverlay` = 1 套正式提示卡样式，附 `有键/无键` 两个布局态
     - `SpringDay1PromptOverlay` = 1 套任务卡样式，附 `未完成/已完成` row 颜色态
     - `SpringDay1WorldHintBubble` = 2 个显式 `HintVisualKind`，但当前 live 链未接通
     - `NpcWorldHintBubble` = 1 套旧 NPC hint 壳，但当前未接入显示链
     - `ItemTooltip` = 1 套 shared tooltip 壳，独立于主交互提示链
     - `DialogueUI` = 不是世界提示壳，但有字体表现 preset 路由；代码保底 key 为 `default / speaker_name / inner_monologue / garbled`，实际内容至少用了 `default / narration`
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\InteractionHintOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorldHintBubble.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\NpcWorldHintBubble.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltip.cs`
- 验证结果：
  - `静态代码盘点成立`
  - 未改代码、未跑测试、未进 Unity。
- 遗留问题 / 下一步：
  1. 如果以后继续做样式治理，第一优先不是改颜色，而是先做边界裁定：
     - `SpringDay1WorldHintBubble` 是恢复使用，还是认定为 dormant shell
     - `NpcWorldHintBubble` 是保留旧链，还是删除死代码
  2. 如果要做 shared/UI 统一主题，再判断 `InteractionHintOverlay / SpringDay1PromptOverlay / ItemTooltip` 是否共用一套视觉基线。
- 当前恢复点：
  - 本轮分析已完成；若用户继续追问，可直接基于这份代码盘点展开“删旧壳 / 接线 / 统一样式”下一步，而不需要重新搜一轮文件。

## 2026-04-05 Tooltip / 工具状态条 / 时间调试链只读审计

- 用户目标：
  - 只读审计 Sunset 当前 Tooltip / 工具状态条 / 时间调试链，根据历史需求核清现状和最可能仍不满足的点；限定代码读取范围为 9 个指定文件，不改代码。
- 当前主线目标：
  - 继续今日“气泡/提示样式代码盘点”主线，把 shared tooltip、工具状态条和时间调试链补成一份可直接决策的结构审计。
- 本轮子任务：
  - 分别核对 `ItemTooltip`、背包/快捷栏状态条、`TimeManagerDebugger -> TimeManager -> PersistentManagers` 接线，并说明哪些已满足、哪些最可能仍未满足。
- 已完成事项：
  1. 已只读审计以下文件：
     - `Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs`
     - `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
     - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
     - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
     - `Assets/YYY_Scripts/Data/Core/ToolRuntimeUtility.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
     - `Assets/YYY_Scripts/TimeManagerDebugger.cs`
     - `Assets/YYY_Scripts/Service/TimeManager.cs`
     - `Assets/YYY_Scripts/Service/PersistentManagers.cs`
  2. 已确认当前成立的结构：
     - Tooltip 有统一单例、延迟显示、淡入淡出、鼠标跟随、拖拽/按键抑制和越界隐藏。
     - 背包/快捷栏状态条统一通过 `ToolRuntimeUtility.TryGetToolStatusRatio` 计算，支持耐久与水量两类工具。
     - `TimeManagerDebugger` 会被 `TimeManager` 和 `PersistentManagers` 自动挂载，支持天/季/倍速/暂停/整点微调与右上角时钟。
  3. 已收出最可能仍未满足的点：
     - 背包/箱子 Tooltip 未传 `sourceTransform`，与快捷栏 Tooltip 的边界/Canvas 上下文不一致。
     - Tooltip runtime fallback 会自建通用 UI，而且没有 `qualityIcon`，容易冒充正式 face。
     - Tooltip 延迟被最小 1 秒硬钳住。
     - 状态条 recent-use 只记录成功用具提交，失败尝试不会自动亮条。
     - `空水壶 / 工具损坏` 之外的失败原因仍偏日志口径。
     - 时间调试器默认无条件自动挂载且强制开 key；左上角帮助 GUI 也会跟着常驻。
     - `SetTime()` 不补 `OnDayChanged`，导致调试跳日/跳季可能漏掉日更订阅者。
- 关键决策：
  1. 这轮判断只落在 `结构 / checkpoint`，不宣称 UI/体验已过线。
  2. 优先级建议固定为：
     - P0：时间调试链 dev-only/可硬关闭 + `SetTime` 跳日事件补齐
     - P1：背包 Tooltip 上下文一致化 + fallback 是否允许继续冒充正式面
     - P2：失败用具尝试的状态条/反馈补齐
  3. `PlayerThoughtBubblePresenter` 在当前允许文件里只看得到通用 show/hide/换行壳；真正的工具失败文案链仍被 `PlayerToolFeedbackService` 隔开，不能假装已核清最终玩家气泡体验。
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
  - 仅完成静态代码审计。
  - 未改代码、未跑测试、未进 Unity。
- 遗留问题 / 下一步：
  1. 如果用户要求继续，下一步最稳的是先修 `TimeManagerDebugger/SetTime`，因为它属于高可见 debug 污染 + 订阅链一致性问题。
  2. 之后再修 Tooltip 上下文和 fallback，最后补失败用具反馈链。
- 当前恢复点：
  - 这轮盘点已完成；若继续，可直接进入“最小修复 P0”而不必重读 9 个文件。

## 2026-04-05｜5 文件未提交改动复审：Tooltip / 状态条 / 玩家气泡

- 用户目标：
  - 只读审计以下 5 个脚本的当前未提交改动，并简洁回答：
    1. Tooltip / 状态条 / 玩家气泡还缺什么
    2. 哪些是纯表现优化，哪些会碰逻辑
    3. 最值得先落的 3 个点
- 当前主线目标：
  - 继续今日“气泡 / 提示样式代码盘点”主线，把当前未提交 diff 收成一份可直接下判断的结构复审，而不是继续实现。
- 本轮子任务 / 阻塞：
  - 子任务是把上一轮旧盘点中已经被当前未提交改动补掉的点，与现在仍未收掉的点分开。
  - 本轮仍是纯只读，无 Unity / 编译 blocker。
- 已完成事项：
  1. 已只读复审：
     - `Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs`
     - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
     - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerToolFeedbackService.cs`
  2. 已额外只读核对相关调用链，确认当前未提交改动已经补上的项：
     - 槽位 Tooltip 的 `sourceTransform` 已接回 `ItemTooltip`
     - runtime fallback Tooltip 已补 `qualityIcon` 与优先字体
     - 玩家气泡 typed-text / 会话排序 API 已被 `PlayerNpcChatSessionService` 接入
  3. 已确认当前仍未收掉的 3 类缺口：
     - Tooltip 仍有 `1s` 硬延迟，非 slot 的 `ShowCustom` 调用侧上下文仍待统一
     - 状态条仍未把“失败使用”接成可见反馈，只是把现有显示条件做得更顺
     - 玩家工具反馈气泡在 NPC 会话中会直接被压掉，当前没有延后补发或更明确的优先级闭环
- 关键决策：
  1. 这轮只站在 `结构 / checkpoint`，不能 claim 真实体验已过线。
  2. `纯表现优化` 与 `会碰逻辑` 的边界：
     - 表现：Tooltip 壳体/字体/淡入淡出、状态条 alpha fade、玩家气泡样式参数
     - 逻辑：Tooltip 跟随上下文、选中态清理、typed-text/排序接口、工具损坏替换语气与聊天期抑制
  3. 当前最值得先落的 3 个点固定为：
     - Tooltip 出现时机与所有调用侧上下文统一
     - 失败用具尝试的状态条显露
     - 玩家气泡与 NPC 会话的优先级裁定
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts/UI/Inventory/ItemTooltip.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts/Service/Player/PlayerToolFeedbackService.cs`
- 验证结果：
  - `静态 diff + 调用链复审成立`
  - 未改业务代码、未跑测试、未进 Unity。
- 遗留问题 / 下一步：
  1. 如果以后继续施工，先收 Tooltip 时机，再收失败状态条，再做玩家气泡优先级裁定。
  2. 如果以后要判断“是否真的顺眼”，还需要 fresh GameView / 玩家面证据，当前代码审计不够。
- 当前恢复点：
  - 这轮复审已完成；若继续追问，可直接从这份 3 点优先级往下拆，不必重新扫 diff。

## 2026-04-05｜失败组只读复核：`BlockingUi resume` 与 `PlayerThoughtBubblePresenterStyleTests`

- 用户目标：
  - 只读分析一组测试失败，不改文件；聚焦：
    - `NPCInformalChatInterruptMatrixTests.ResumeIntroPlan_ShouldReturnContinuityLines_ForBlockingUiResume`
    - `PlayerThoughtBubblePresenterStyleTests` 的 3 个失败
  - 输出每个失败的最可能根因、最小修法建议、exact files/methods，以及是测试夹具坏了还是代码契约缺口。
- 当前主线目标：
  - 继续今日“气泡 / 提示 / 玩家气泡链”主线，把这组失败判断成“当前代码真缺口”还是“旧 runner / 旧快照残留”。
- 本轮子任务 / 阻塞：
  - 子任务是把源码、当前已编 `Assembly-CSharp.dll` 和测试文件本身三者对齐。
  - 阻塞是 `unityMCP run_tests` 这轮依旧只返回 `total=0`，不能拿它当可信失败名来源。
- 已完成事项：
  1. 只读复核了：
     - `Assets/YYY_Tests/Editor/NPCInformalChatInterruptMatrixTests.cs`
     - `Assets/YYY_Tests/Editor/PlayerThoughtBubblePresenterStyleTests.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
     - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
  2. 额外只读核了当前 `unityMCP` 基线、实例与 `Library/ScriptAssemblies/*.dll`。
  3. 通过离线反射探针已经直接确认当前已编 `Assembly-CSharp.dll` 中：
     - `CreateFallbackResumeIntroPlan(BlockingUi, BetweenTurns, 1)` 的 `PlayerLine / NpcLine` 与测试断言完全一致
     - 玩家 preset、10 字换行、foreground sorting 常量、speaker boost、readable hold 公式都与 style test 断言一致
  4. 当前没法被我离线钉死的只剩两条运行态依赖更重的 style 点：
     - `ConversationLayout_ShouldStayCloseToSpeakerHeads_WhileKeepingReadableSeparation`
     - `AmbientBubble_ShouldIgnoreHiddenStaleConversationOwner`
- 关键决策：
  1. 这组失败报告当前更像 `旧 runner / 旧快照 / 不可信 test receipt`，不像“当前磁盘代码仍缺那几个字符串或常量”。
  2. `BlockingUi resume` 这一条在当前已编 DLL 里已经对齐；如果外部仍报失败，最该怀疑的是旧结果残留，而不是继续改文案。
  3. 如果后续要继续追这组失败，第一优先不是改 `preset` 或 resume 文案，而是先拿一份可信 fresh runner 结果；拿不到，就只剩运行态两点值得继续深查：`UpdateConversationBubbleLayout()` 与 `NPCBubblePresenter.CanShow(...)`。
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NPCInformalChatInterruptMatrixTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerThoughtBubblePresenterStyleTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Library\ScriptAssemblies\Assembly-CSharp.dll`
  - `D:\Unity\Unity_learning\Sunset\Library\ScriptAssemblies\Tests.Editor.dll`
- 验证结果：
  - `源码 + 已编 DLL 离线反射` 成立。
  - `unityMCP run_tests(EditMode, NPCInformalChatInterruptMatrixTests, PlayerThoughtBubblePresenterStyleTests)` 仍返回 `total=0`，不可当有效失败/通过证据。
  - 本轮未改业务代码、未进 Play Mode。
- 遗留问题 / 下一步：
  1. 如果用户后续继续追这组失败，先拿可信 fresh runner 结果。
  2. 若 fresh runner 仍红，再优先深查：
     - `PlayerNpcChatSessionService.UpdateConversationBubbleLayout()`
     - `NPCBubblePresenter.CanShow(...)`
  3. 若拿不到可信 runner，则不应继续把旧失败截图当当前代码真相。
- 当前恢复点：
  - 这轮分析已把“当前代码真缺口”与“旧失败回执”初步分开；若继续，可直接从 `ConversationLayout / AmbientBubble / runner 可信度` 三条往下追，不必重扫整套气泡文件。
