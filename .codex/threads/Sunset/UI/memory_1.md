# UI 线程记忆

## 2026-04-01：按独立 `UI / SpringUI` 主刀恢复玩家面近身提示链施工，并拿到第一张 live 终面证据

- 当前主线目标：
  - 不再把本线程挂成 `spring-day1V2` 影子；
  - 按 `2026-04-01_典狱长_UI_接管玩家面UIUE整合主刀_03.md`，以 `Story / NPC / Day1 玩家面体验链的 UI/UE 集成主刀` 身份，把最近目标 / 唯一提示 / 唯一 `E` / 视觉归属一致这条玩家面链继续接顺。
- 本轮子任务：
  - 在不碰 `Primary.unity`、`GameInputManager.cs`、`NPCBubblePresenter.cs`、`PlayerNpcChatSessionService.cs`、`PlayerThoughtBubblePresenter.cs` 的前提下，只收：
    - `SpringDay1ProximityInteractionService`
    - `SpringDay1WorldHintBubble`
    - `InteractionHintOverlay`
    - 必要的 `SpringDay1BedInteractable`
    - 以及最小验证文件
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
- 本轮真实施工与结果：
  1. `SpringDay1ProximityInteractionService` 已明确优先保留 `CanTriggerNow=true` 的当前焦点，避免更近但 still teaser 的目标抢走真正可触发的 `E`。
  2. `SpringDay1WorldHintBubble` 已改成正式卡片化两态：
     - `ready` 态保留 `E + 标题 + 细节`
     - `teaser` 态去掉按键，细节改成 `再靠近一些`
  3. `InteractionHintOverlay` 已补正式底部交互卡，而不是只剩调试条。
  4. `SpringDay1BedInteractable` 已最小接回统一近身仲裁，并通过 `ResolveRestInteractionDetail(...)` 兼容导演层提示文案读取。
  5. 新增 `InteractionHintDisplaySettings.cs` 作为提示显隐开关承载。
- 本轮实际写到的文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorldHintBubble.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\InteractionHintOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1BedInteractable.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\InteractionHintDisplaySettings.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1InteractionPromptRuntimeTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1DialogueProgressionTests.cs`
- 本轮验证：
  - `git diff --check` 通过
  - `CodexCodeGuard` 对上述 7 个 C# 文件通过：
    - `utf8-strict`
    - `git-diff-check`
    - `roslyn-assembly-compile`
  - live 取证已成功：
    - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260401-173354-243_manual.png`
    - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted\20260401-173354-243_manual.json`
    - 侧车快照记录：
      - `WorldHint=001|E|交谈|按 E 开始对话|distance=0.00|priority=30|ready=True`
      - `PlayerFacing=0.0.2 首段推进链|focus=靠近 NPC 并按 E 开始首段对话。|progress=首段对话进行中|hint=001/E/交谈/按 E 开始对话/distance=0.00/priority=30/ready=True`
- 本轮未闭环项：
  1. `unityMCP run_tests` 的定向过滤没有拿到可信通过结果；
  2. 真实第一 blocker 不是代码编不过，而是共享 Unity 现场存在自动进入 / 退出 `PlayMode` 的验证节奏，导致 targeted tests 窗口被抢走。
- 本轮阶段判断：
  - 这轮已经从“结构链存在”推进到“玩家面 live 画面 + 运行态快照成立”；
  - 但还不能说“自动化验证也全部补齐”。
- 当前恢复点：
  - 本轮不做 `Ready-To-Sync`
  - 下一步若继续，应优先在干净 Unity 窗口里重跑：
    - `SpringDay1InteractionPromptRuntimeTests`
    - `SpringDay1DialogueProgressionTests` 的相关子集
  - 如果用户先审当前效果，当前最值钱的证据就是这张新的 `accepted` 玩家图。

## 2026-04-01：按主刀任务书完成只读收口，线程已从 `ACTIVE` 收回 `PARKED`

- 当前主线目标：
  - 不是继续新增玩家面实现；
  - 而是按 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-01_典狱长_UI_接管玩家面UIUE整合主刀_03.md` 的固定格式，把这轮已经完成的玩家面切片正确回执给用户。
- 本轮子任务：
  1. 补读主刀任务书要求的上下文：
     - `2026-04-01_Story向UIUE集成owner边界与派工约定.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\memory.md`
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\场景搭建（外包）\memory_0.md`
  2. 重新确认：
     - 我当前就是独立的 `UI / SpringUI` 玩家面主刀
     - 不是 `spring-day1V2` 的影子线
  3. 由于这轮不继续扩刀、不做 `sync`，已执行：
     - `Park-Slice`
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮新增落地：
  - 无新增业务代码 / prefab 写入
  - 线程状态已从 `ACTIVE` 改为 `PARKED`
- 当前 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑；因为这轮没有进入 `sync` 收口阶段
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
  - blocker：
    - `targeted-tests-blocked-by-shared-editor-playmode-window`
- 当前恢复点：
  - 用户现在已经可以直接基于现有 `accepted` 图和 sidecar 审玩家面结果；
  - 若后续继续，我最优先的下一步仍是先补干净窗口下的 targeted tests，而不是继续扩大实现范围。

## 2026-04-01：按用户新裁定完成身份 / 工作区 / exact-own 自查，UI 线程不再向两侧漂移

- 当前主线目标：
  - 先把本线程的身份、工作区、state 和 own 边界回正；
  - 暂不进入新的真实施工。
- 本轮子任务：
  1. 重新确认我当前身份不是：
     - `spring-day1V2` 影子
     - `Spring` 单点小外包
  2. 重新确认我当前唯一身份就是：
     - `Story / NPC / Day1 玩家面体验链的 UI/UE 集成 owner`
  3. 把本线程这轮相关文件分成：
     - `exact-own`
     - `协作切片`
     - `明确释放`
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮自查结论：
  1. `exact-own files`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorldHintBubble.cs`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\InteractionHintOverlay.cs`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs` 的玩家面整合切片
  2. `协作切片 files`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCDialogueInteractable.cs`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCInformalChatInteractable.cs`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\CraftingStationInteractable.cs`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1BedInteractable.cs`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1InteractionPromptRuntimeTests.cs`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1DialogueProgressionTests.cs`
  3. `明确释放 / 不再吞的 files`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubblePresenter.cs`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
     - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- 当前状态：
  - 线程仍为 `PARKED`
  - 第一 blocker 不变：
    - `targeted-tests-blocked-by-shared-editor-playmode-window`
- 当前恢复点：
  - 这轮身份和边界已经回正；
  - 下一轮若用户直接叫我继续真实施工，我应从 `UI / SpringUI` 身份出发进入 `Begin-Slice`，继续收玩家面链，而不是再把自己挂错到别的线程名下。

## 2026-04-02：左下角 `InteractionHintOverlay` 任务优先仲裁已落地，并按 checkpoint 停在 `PARKED`

- 当前主线目标：
  - 只做左下角 `InteractionHintOverlay` 的内容仲裁；
  - 当同一个对象当前更该走 `Spring` 正式任务语义时，左下角不再显示“闲聊”。
- 本轮子任务：
  1. 读取：
     - `D:/Unity/Unity_learning/Sunset/.kiro/specs/NPC/2.0.0进一步落地/0.0.2清盘002/2026-04-02-NPC给UI的左下角任务提示接管委托-01.md`
  2. 只改 shared/UI 里的仲裁层，不回改 NPC 会话状态机与头顶气泡整包。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
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
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1InteractionPromptRuntimeTests.cs`
- 本轮实现结果：
  1. 当同一个锚点同时具有 `NPCDialogueInteractable` 和 `NPCInformalChatInteractable`，而当前 ready 候选仍是泛化的 `交谈 / 按 E 开始对话` 时，左下角会被 shared/UI 覆写为：
     - `进入任务`
     - `按 E 开始任务相关对话`
  2. 这刀只改左下角 overlay 的内容仲裁，头顶 `SpringDay1WorldHintBubble` 保持原文案来源。
  3. 没有通过回改 NPC 会话逻辑去“假装解决 UI 仲裁”。
- 本轮验证：
  - `git diff --check`：通过
  - `CodexCodeGuard`：通过
  - Unity 定向 test：尝试过，但插件会话断开，未拿到可信 pass
- 当前状态：
  - `PARKED`
  - blocker：
    - `unity-targeted-test-disconnected-while-awaiting-command_result`
- 当前恢复点：
  - 代码与静态编译闸门已站住；
  - 后续若继续，只需补 targeted test，不需要扩大 scope。

## 2026-04-02：玩家气泡样式已回正为 NPC 正式面镜像，并修复了新测试的装配报错

- 当前主线目标：
  - 继续以 `UI / SpringUI` 身份处理玩家真正看到的结果层；
  - 这轮只收一个用户直接指出的新问题：玩家气泡样式完全不对，要求“和 NPC 一致就好”。
- 本轮子任务：
  1. 只检查并修改 `PlayerThoughtBubblePresenter`。
  2. 只新增最小 Editor 测试，不回改 NPC 底座和会话系统。
  3. 用户回报测试编译错误后，同轮继续把测试装配方式修正干净。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮 thread-state：
  - 延续既有 `ACTIVE` slice：
    - `玩家气泡样式回正为NPC同款正式面`
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerThoughtBubblePresenterStyleTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerThoughtBubblePresenterStyleTests.cs.meta`

## 2026-04-05：只读审计箱子 UI / Inventory 交互链，确认当前不一致根因是 Held 语义双轨而非单个堆叠判断

- 当前主线目标：
  - 用户要求只读审查 Sunset 当前 `UI/Inventory` 与箱子交互链，解释为什么箱子 UI 的 `shift/ctrl` 拿起、放回、同类堆叠、跨 Up/Down 落点语义和背包不一致；
  - 不改代码，只给结构结论与最小修复优先级。
- 本轮子任务：
  1. 手工等价执行 Sunset 启动闸门，只读核 `main / HEAD / dirty` 与相关工作区。
  2. 读取并比对：
     - `InventoryInteractionManager.cs`
     - `InventorySlotInteraction.cs`
     - `SlotDragContext.cs`
     - `InventorySlotUI.cs`
     - `BoxPanelUI.cs`
     - `ChestController.cs`
  3. 把“共享事实源 / 分叉点 / 最小修复建议”收成用户可直接判断的审计结论。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `delivery-self-review-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮关键判断：
  1. 箱子格数据真源已经是 `ChestController.RuntimeInventory -> ChestInventoryV2`；legacy `ChestInventory` 只保留兼容镜像。
  2. 真正共享的只有容器/堆叠层：
     - `IItemContainer`
     - `ItemStack.CanStackWith`
     - `container.GetMaxStack(itemId)`
  3. 真正不共享的是交互语义层：
     - 背包 `shift/ctrl` Held 由 `InventoryInteractionManager` 管
     - 箱子 `shift/ctrl` Held 由 `InventorySlotInteraction + SlotDragContext + _chestHeldByShift/_Ctrl` 另起一套
     - `Down -> Up` 走 `HandleManagerHeldToChest(...)`
     - `Up -> Down` 走 `HandleSlotDragContextDrop(...)`
  4. 因此当前不一致不是“能不能堆”的规则不同，而是同一面板里存在两套 Held owner、两套回源逻辑、两套跨区域落点函数。
- 本轮涉及路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryInteractionManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotInteraction.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\SlotDragContext.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Box\BoxPanelUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\World\Placeable\ChestController.cs`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\箱子系统\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\memory.md`
- 本轮验证：
  - `静态代码审计成立`
  - `未改业务代码`
  - `未跑测试 / 未进 Unity`
- 当前恢复点：
  - 若后续继续施工，最优先不是继续补箱子专用 if/else，而是先统一 `shift/ctrl` Held owner 和 Up/Down 落点入口；
  - 这轮是分析型收口，不进入真实施工，因此不跑 `Begin-Slice`，当前线程仍视为 `PARKED`。

## 2026-04-05：审计尾记

- 本轮审计层已补：
  - `C:\Users\aTo\.codex\memories\skill-trigger-log.md`
  - `STL-20260405-039`
- 当前最终停点：
  - 工作区记忆、线程记忆、skill 审计日志均已落盘；
  - 如用户继续，要直接从“统一 Held owner / 统一 Up-Down 落点入口”开始，不必再重复首轮盘点。
- 本轮实现结果：
  1. 根因已坐实：`PlayerThoughtBubblePresenter` 的 `ApplyPlayerBubbleStylePreset()` 会持续把玩家气泡刷回浅色玩家预设，所以它不是“现场偶发错位”，而是代码真源就是错的。
  2. 现在玩家气泡已回到和 NPC 正式面同语言：
     - 深色底
     - 金色边
     - 正式文字色与描边
     - 相同的宽度与换行基线
  3. 只保留玩家侧必要镜像差异：
     - `bubbleLocalOffset.x`
     - `tailHorizontalBias`
  4. `FormatBubbleText(...)` 已改成 NPC 同款规则，避免玩家气泡内容层继续和 NPC 不同频。
  5. 新增最小 Editor 测试，锁死：
     - 玩家预设应镜像 NPC 正式面
     - 玩家文本换行应与 NPC 一致
  6. 用户随后直接贴出 `Tests.Editor` 编译错误，这轮已继续修到位：
     - 测试不再强引用 runtime 类型
     - 改成反射 `ResolveTypeOrFail(...)`
- 本轮验证：
  - `git diff --check`：通过
  - `CodexCodeGuard`：通过
    - `Assembly-CSharp`
    - `Tests.Editor`
    - `Diagnostics = []`
  - 当前结论层级：
    - `结构 / checkpoint`：已过
    - `targeted probe / 局部验证`：已过
    - `真实入口体验`：未验证
- 当前恢复点：
  - 这轮已经把“玩家气泡被错误预设反刷”这条代码级问题清掉；
  - 如果用户下一轮继续给反馈，应优先看真实游戏里：
    - 玩家对话气泡
    - 工具反馈气泡
  - 若还要修，也继续只在 `PlayerThoughtBubblePresenter` 内做局部数值微调，不要借这刀扩到 NPC 底座或别的提示系统。

## 2026-04-02：继续把玩家气泡这刀加固到更厚的回归护栏，再次合法停回 `PARKED`

- 当前主线目标：
  - 延续上一轮玩家气泡正式面回正，不切去别的 UI；
  - 本轮只做“回归护栏更厚”，避免以后别人改浮动、节奏和停留时又把观感带歪。
- 本轮子任务：
  1. 从 `PARKED` 重新进入极窄 slice：
     - `玩家气泡样式回正收口与回归加固`
  2. 不再改 `PlayerThoughtBubblePresenter` 逻辑本体。
  3. 只增强 `PlayerThoughtBubblePresenterStyleTests.cs`。
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
  1. 继续把玩家气泡与 NPC 正式面的同构关系钉得更牢。
  2. 新增断言覆盖以下参数：
     - `minBubbleHeight`
     - `bubbleGapAboveRenderer`
     - `visibleFloatAmplitude`
     - `visibleFloatFrequency`
     - `tailBobAmplitude`
     - `tailBobFrequency`
     - `showDuration`
     - `hideDuration`
     - `showScaleOvershoot`
  3. 当前这条线已经从“修正错样式”推进到“把容易再次漂味的动态参数也纳入回归测试”。
- 本轮验证：
  - `git diff --check`：通过
  - `CodexCodeGuard`：通过
    - `Assembly-CSharp`
    - `Tests.Editor`
    - `Diagnostics = []`
- 当前恢复点：
  - 玩家气泡这刀当前更缺的不是继续补测试，而是用户在真实画面里看它是不是已经顺眼；
  - 如果下一轮继续，应优先围绕用户看到的实际气泡效果给反馈，而不是再加新抽象或扩写别的 UI 系统。

## 2026-04-02：用户现场贴出的 `Object` 二义性报错已当场修掉

- 当前主线目标：
  - 不扩题，直接修用户刚贴出来的 `CS0104`。
- 本轮子任务：
  1. 只看 `PlayerThoughtBubblePresenterStyleTests.cs`
  2. 只修：
     - `Object` 在 `System` 和 `UnityEngine` 之间的二义性
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerThoughtBubblePresenterStyleTests.cs`
- 本轮实现结果：
  1. 是我这边测试文件带出来的问题，不是用户现场额外弄坏了什么。
  2. 已把两处：
     - `Object.DestroyImmediate(...)`
     改成：
     - `UnityEngine.Object.DestroyImmediate(...)`
  3. 没再碰任何业务逻辑。
- 本轮验证：
  - `git diff --check`：通过
  - `CodexCodeGuard`：通过
    - `Tests.Editor`
    - `Diagnostics = []`
- 当前恢复点：
  - 这份测试文件的二义性编译错误已经清掉；
  - 后续应该回到玩家真实看到的气泡效果，而不是继续在这份测试上来回打补丁。

## 2026-04-02：继续补完玩家气泡未竟项，当前代码口径与测试口径都已回正

- 当前主线目标：
  - 不再只停在“测试能编过”；
  - 把玩家气泡这条线真正收成“和 NPC 一致”的状态。
- 本轮子任务：
  1. 纠正 `PlayerThoughtBubblePresenterStyleTests.cs` 当前仍然写反的测试语义。
  2. 补齐 `PlayerThoughtBubblePresenter` 里还没和 NPC 当前正式面对齐的边框与动态默认值。
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerThoughtBubblePresenterStyleTests.cs`
- 本轮实现结果：
  1. 测试已从错误的“玩家应和 NPC 保持明显区分”改回：
     - 玩家气泡应镜像 NPC 正式面
  2. 测试断言现在覆盖：
     - 偏移镜像
     - 颜色一致
     - 动态参数一致
     - 换行一致
  3. 玩家 presenter 当前边框真值也已补齐到 NPC 当前正式面：
     - `0.92 / 0.79 / 0.56 / 1`
  4. 两个动态默认值也补齐：
     - `visibleFloatAmplitude = 0.004`
     - `tailBobAmplitude = 26`
- 本轮验证：
  - `git diff --check`：通过
  - `CodexCodeGuard`：通过
    - `Assembly-CSharp`
    - `Tests.Editor`
    - `Diagnostics = []`
- 当前恢复点：
  - 玩家气泡这条线现在已经从“修一半、测一半”推进到“代码和测试方向都对了”；
  - 如果下一轮继续，最值钱的不是再补静态检查，而是看用户真实画面里是否已经觉得顺眼。

## 2026-04-02：继续把玩家 / NPC 对话气泡做成“谁说话谁在上面、两边分得开、收尾能闭环”的版本

- 当前主线目标：
  - 继续收用户对玩家 / NPC 双气泡的最新反馈，不再停留在“样式像不像”，而是直接修：
    1. 说话者必须在上层
    2. 玩家与 NPC 气泡必须明显分隔
    3. 玩家气泡要回到玩家自语语义
    4. 中断 / 结束后的逻辑必须闭环
- 本轮子任务：
  1. 收紧 `PlayerThoughtBubblePresenter` 与 `NPCBubblePresenter` 的会话位移和排序能力。
  2. 在 `PlayerNpcChatSessionService` 里引入发言焦点，让会话布局按当前发言方决定前景层级。
  3. 补一份新的 `Editor` 测试，把“谁说话谁在上面”和“重置后清干净”钉住。
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerThoughtBubblePresenterStyleTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerNpcConversationBubbleLayoutTests.cs`
- 本轮关键决策：
  1. 当前真正的大问题不是单个颜色值，而是“会话双气泡排布没有把当前发言者当成一等公民”。
  2. 因此这轮采用的主修法不是继续微调字号，而是：
     - 会话服务显式记录当前 bubble focus
     - presenter 显式支持 `sort boost`
     - 双气泡整体位移重新回到真正能看出分隔的量级
  3. 玩家气泡样式不再按“和 NPC 完全同一张卡”处理，而是保持同等级正式面质量，但回到玩家侧偏左、尾巴方向和填充色都更像玩家自语的口径。
- 本轮验证：
  - `git diff --check`：通过
  - Unity batchmode：未过入口
    - 第一真实 blocker：项目已被另一 Unity 实例占用，批处理互斥拒绝打开
  - 临时 `csc` 试编：通过
    - 参考面：`NETStandard 2.1 + Unity Managed + Library/ScriptAssemblies`
    - 覆盖：3 个业务脚本 + 2 个测试脚本
    - 另额外把 `NPCInformalChatInteractable.cs` 作为协作类型桥接源码纳入试编，但这轮没有改它
- 本轮 thread-state：
  - 进入本轮时：`ACTIVE`
  - 收尾时：已执行 `Park-Slice`
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 结构和静态逻辑现在已经推进到“说话者优先、分隔更强、收尾可清”的版本；
  - 下一轮若继续，应优先拿真实画面确认玩家主观感受，而不是再回去做无意义的静态补丁。

## 2026-04-02：气泡线最终收尾结论已锁死为“未 sync，但 blocker 明确”

- 当前主线目标：
  - 按 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\场景搭建（外包）\2026-04-02_UI最终收尾prompt_01.md` 把玩家 / NPC 气泡线作为 checkpoint 级完成物干净收口。
- 本轮子任务：
  1. 串行核对 `UI.json` 是否已按真实 own / expected sync 路径报真。
  2. 运行 `Ready-To-Sync`，判断这刀到底是能 sync 还是应直接报 blocker。
  3. 若不能 sync，则执行 `Park-Slice`，并把 completion layer 与恢复点写清楚。
- 本轮结论：
  1. `UI.json` 当前已报真，真实 own 集合为：
     - `PlayerNpcChatSessionService.cs`
     - `PlayerThoughtBubblePresenter.cs`
     - `NPCBubblePresenter.cs`
     - `PlayerThoughtBubblePresenterStyleTests.cs`
     - `PlayerThoughtBubblePresenterStyleTests.cs.meta`
     - `PlayerNpcConversationBubbleLayoutTests.cs`
     - `PlayerNpcConversationBubbleLayoutTests.cs.meta`
     - `.kiro/specs/UI系统/0.0.1 SpringUI/memory.md`
     - `.kiro/specs/UI系统/memory.md`
     - `.codex/threads/Sunset/UI/memory_0.md`
  2. 这刀的 completion layer 只能写成：
     - `结构 / checkpoint`：成立
     - `targeted probe / 局部验证`：成立或基本成立
     - `真实入口体验`：尚未验证
  3. `Ready-To-Sync` 已明确阻断，这轮不能硬 sync。
- 第一真实 blocker：
  - 当前 own roots 仍有未纳入本轮的 remaining dirty/untracked，会把别的旧尾账卷进这刀；
  - 直接点名的同根残留包括：
    - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerToolFeedbackService.cs`
    - `Assets/YYY_Tests/Editor/OcclusionSystemTests.cs`
    - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
    - 以及 UI 根下未纳入本轮的额外文档 / 测试残留
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：已跑，结果 `BLOCKED`
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 这条气泡线现在的正确口径是“checkpoint 已站住，但本轮未 sync，blocker 已明确”；
  - 下次恢复时先处理 same-root remaining dirty 的归属 / 清理，再决定是否重新进入收口；
  - 玩家真实视面终验仍待用户让出当前 Unity 实例后补做。

## 2026-04-03：UI 线程正式接走 spring-day1 玩家面后，本轮先收提示链坏点与夜间收束残留

- 当前主线目标：
  - 从 `spring-day1` 手里接走当前全部玩家面 `UI/UE` 问题与体验收口，但不回到 `NPCBubblePresenter.cs`、`Primary.unity`、`GameInputManager.cs`。
- 本轮子任务：
  1. 先审玩家真正看到的 `Prompt / Hint / WorldHint / Workbench / DialogueUI` 代码现场。
  2. 先修最明确的头顶提示坏点，再看是否有同类玩家面泄漏能用最小切口带走。
- 本轮实际做成：
  - `SpringDay1ProximityInteractionService` 恢复 teaser 态世界提示，不再把“再靠近一些”的头顶提示错误隐藏。
  - `SpringDay1WorldHintBubble` 恢复正式卡片显示、teaser/ready 两态布局、可读尺寸与字体可用性。
  - `InteractionHintOverlay` / `NpcWorldHintBubble` 补成“字体必须可渲染”的一致口径，避免玩家面出现空字壳。
  - 额外在 `SpringDay1Director.HandleSleep()` 切了一个最小玩家面口，确保 `DayEnd` 后低精力 warning 不残留。
- 本轮验证：
  - `SpringDay1InteractionPromptRuntimeTests`：`5/5 Passed`
  - `SpringDay1LateDayRuntimeTests`：`4/4 Passed`
  - Unity Console `error`：`0`
  - live 真实入口体验：未补
    - 原因：当前不是安全 live scene 窗口，`Primary.unity` 仍有用户独占锁，这轮不应为了取图去碰主现场
- 本轮关键判断：
  1. 当前最该先收的不是模板化，而是“玩家一眼就能看见不对”的正式提示面。
  2. 这轮已经形成可信 checkpoint，但还不能说真实体验过线。
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
    - 原因：本轮未准备 sync，且 own-root 旧脏改还没重新梳成可提交白名单
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 下一轮若继续，应优先补真实入口体验层证据，或继续从玩家面优先级去收 Prompt / DialogueUI / Workbench 的剩余正式面问题；
  - 当前不要把 targeted probe checkpoint 误写成“玩家体验已经全面过线”。

## 2026-04-03：用户要求彻查任务列表与 UI 提示缺字，这轮已查清代码层真相但未进入实现

- 当前主线目标：
  - 用户要我彻底查清 `spring-day1` 当前任务列表和所有玩家面提示到底漏了多少字、漏在源头还是漏在显示链，并先做反思汇报，不准直接实现。
- 本轮子任务：
  1. 把 `PromptOverlay` 的任务卡状态链从 `Director` 到 UI 壳体完整串起来。
  2. 把 `NPCDialogue / NPCInformalChat / Workbench / Bed -> ProximityService -> WorldHint / InteractionHintOverlay` 的提示链完整串起来。
  3. 补查 prefab / scene 默认值和文本组件策略，区分“没填”“过泛”“被省略”。
- 本轮已完成：
  - 查清 `Prompt` 任务卡当前不是“字段空”，而是 `BuildPromptItems()` 所有主阶段都只建 `1` 条任务，农田教学链最多同时缺 `4` 条应保留目标；
  - 查清 `NPC` 闲聊 prefab 当前默认就是 `闲聊 / 按 E 开口`，不是 UI 把更具体文案吞掉；
  - 查清工作台普通态 detail 源头就是空串；
  - 查清左下角 `InteractionHintOverlay` 的 caption/detail 都是单行省略，长句 detail 有结构性截断风险；
  - 查清 `PromptOverlay` 的 `_manualPromptText` 会粘住并覆盖焦点条，存在旧提醒长期顶住 focus 的状态风险；
  - 查清 `WorkbenchOverlay` 左列名称强制省略、右侧描述高度被 `60f` 上限卡住。
- 当前关键判断：
  1. 这轮最重要的不是“再调一调 UI”，而是先承认我之前把三类问题混在一起说了。
  2. 现在必须把问题拆成：
     - 源头真空
     - 模型没建
     - UI 截断/隐藏
     否则后续施工会继续一刀里同时误修三件不同的事。
  3. 当前证据只站到代码与序列化层，不能把它说成玩家体验终验。
- 本轮验证：
  - 纯只读代码 / prefab / scene YAML 排查
  - 未跑 `Begin-Slice`
    - 原因：本轮没有进入真实施工
- 当前恢复点：
  - 我现在已经知道自己之前“到底没看清什么”了：不是单纯漏了几个字，而是对模型层和显示层没有分账；
  - 下一轮若进入实现，应按“先任务模型、再文案源头、再显示链”的顺序做，不能再反过来。

## 2026-04-03：用户把“缺字才是主矛盾”重新说死后，UI 线程已先落第一轮 runtime 真源/提示归属/交互几何修正

- 当前主线目标：
  - 直接收用户刚重新明确的玩家面主矛盾：
    1. Prompt / 任务列表不能再有壳没字
    2. NPC 交互提示不要再上头顶
    3. 工作台交互范围与提示范围要按不规则可见边界
    4. 闲聊里谁说话谁在上面
- 本轮子任务：
  1. 把 `SpringDay1PromptOverlay / SpringDay1WorkbenchCraftingOverlay` 的 runtime 创建链从“旧现场壳体也能抢到单例”改成“优先复用真正可用的 screen-overlay 运行壳，旧 world-space stale static 先失效再说”。
  2. 把 `CraftingStationInteractable.GetClosestInteractionPoint()` 改成真正的最近点比较，而不是 collider 包络优先短路。
  3. 给 `SpringDay1ProximityInteractionService` 加 NPC 头顶提示兜底禁令。
  4. 给 `InteractionHintOverlay` 补多行正文能力。
  5. 给玩家气泡补前景排序 boost，并把它接进 `PlayerNpcChatSessionService` 的说话流与清理流。
- 本轮实际改动：
  - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
  - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
  - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
  - `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
  - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
  - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
  - `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
  - `Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`
- 本轮验证：
  - 本刀白名单 `git diff --check`：通过
  - Unity batch EditMode：未执行成功
    - 第一真实 blocker：项目当前已被另一 Unity 实例打开，batchmode 直接被 `HandleProjectAlreadyOpenInAnotherInstance` 拦截
  - 真实入口体验：未补
- 本轮 thread-state：
  - 本轮没有新开 slice，沿用已有 `UI` active slice 继续施工
  - `Ready-To-Sync`：未跑
    - 原因：本轮未准备 sync，且 shared root 仍有大量他线 / 旧线 dirty
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 这轮已经把最容易“明明写对了，但一运行又被旧壳体截胡”的坑先收掉了；
  - 下一轮如果继续，最值钱的是拿真实 GameView 看：
    1. Prompt 是否终于回正成可读正式面
    2. NPC 头顶交互提示是否真的退场
    3. 工作台提示范围是否不再偏上偏小
    4. 谁在说话谁在上面是否终于稳定成立

## 2026-04-03：用户要求只读勘察 5 个链路文件，这轮已把“真实问题点 / 代码位置 / 最小修法方向”重新压实

- 当前主线目标：
  - 用户要求不要改代码，只读排查 `spring-day1` 工作台交互范围 / 提示范围、UI 上下翻转、NPC / 玩家气泡遮挡与说话者置顶链路，并明确指出最可能的真实问题点、代码位置和最小修法。
- 本轮子任务：
  1. 复核 `CraftingStationInteractable.cs`。
  2. 复核 `SpringDay1WorkbenchCraftingOverlay.cs`。
  3. 复核 `NPCBubblePresenter.cs`、`PlayerThoughtBubblePresenter.cs`、`PlayerNpcChatSessionService.cs`。
- 本轮已完成：
  - 确认工作台最近点算法存在，但距离阈值偏小且被 `ApplyDay1WorkbenchTuningIfNeeded()` 强制写死；
  - 确认工作台普通态 detail 源头为空串，不是 UI 吞字；
  - 确认 Workbench 上下翻转每帧都在重算，问题更像判据偏 `visual bounds center`，不是链路没接；
  - 确认玩家 / NPC 气泡都还是 `WorldSpace + overrideSorting`，所以遮挡主嫌更像承载层方案，不像单纯样式问题；
  - 确认“谁说话谁在上面”链路存在，但 sort boost 只是叠加在稳定排序偏差上，且玩家即时句路径少了一次显式 `SetSpeakerForegroundFocus()`。
- 当前关键判断：
  1. 我这轮最核心的判断是：这 5 个文件里的主矛盾大多不是“完全没做”，而是“做了，但阈值 / 判据 / 排序优先级不够硬”。
  2. 我最不放心的点是 NPC 头顶交互提示：在这 5 个文件主链里没看到把它重新打开的源头，所以如果 live 里仍出现，主嫌很可能在别处或旧 runtime 现场，而不是这 5 个文件当前版本本身。
  3. 这轮只站到代码结构层，不能把它包装成真实体验已验证。
- 本轮验证：
  - 纯只读勘察
  - 未跑 `Begin-Slice`
    - 原因：本轮没有进入真实施工
- 当前恢复点：
  - 现在可以直接按“工作台阈值 / 普通态 detail / 翻转判据 / 气泡承载层与说话者优先级”这四刀去决定下一步，而不用再把“没做”和“做错”混讲。

## 2026-04-03：只读追查 spring-day1 Prompt/任务列表缺字，当前最像 runtime 复用/回退链问题

- 当前主线目标：
  - 用户要求不要改代码，只读查清 `spring-day1` 的 `Prompt / 任务列表` 为什么会出现“有壳没字”或部分任务文字不显示。
- 本轮子任务：
  1. 核对 `SpringDay1Director.BuildPromptCardModel()` 是否真的给了字。
  2. 核对 `SpringDay1PromptOverlay` 的 runtime 复用、prefab 实例化、壳绑定和行项绑定链。
  3. 核对现有测试到底覆盖了什么、没覆盖什么。
- 本轮实际读取：
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1UiPrefabRegistry.cs`
  - `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
  - `Assets/222_Prefabs/UI/Spring-day1/SpringDay1PromptOverlay.prefab`
- 本轮稳定结论：
  1. `Director` 侧不是第一嫌疑：
     - `BuildPromptCardModel()` 在主 phase 分支里会给 `StageLabel / Subtitle / FocusText / FooterText`
     - `BuildFarmingTutorialPromptItems()` 会明确构建 5 条非空任务
  2. 当前第一嫌疑是 `PromptOverlay` 的 runtime 复用闸门过宽：
     - `CanReuseRuntimeInstance()` 只检查到 `TaskCardRoot`
     - 但真正绑定时还要求 `Page / TitleText / SubtitleText / FocusText / FooterText / TaskList`
     - 半残实例可能先被复用，再在同一 root 上 fallback `BuildUi()`
  3. 第二嫌疑是行项绑定容错过弱：
     - `BindExistingRows()` 只认 `TaskRow_`
     - `BindRow()` 强依赖 `BulletFill / Label / Detail`
     - 这更像“live 壳体稍有偏差就不接管”，不是 prefab 正式面本身没字
  4. 第三嫌疑是 prefab-first 会被字体闸门整块打回：
     - `CanInstantiateRuntimePrefab()` 只要发现任意一个 `TMP` 节点字体不可用，就拒绝整个 prefab
     - 现有测试还没证明运行时一定真实走到了 formal-face prefab 主链
  5. `manual prompt` 会顶掉 `FocusText`，但它更像次级显示问题，不足以解释任务列表整块空白。
- 本轮验证状态：
  - `静态推断成立`
  - `未进入实现`
  - `未跑 Begin-Slice`，原因：本轮始终只读
- 当前恢复点：
  - 如果下一轮进实现，我最确信的顺序是：
    1. 先收 `CanReuseRuntimeInstance()` 和 stale 壳清理
    2. 再收 `BindRow()/EnsureRows()` 容错
    3. 再处理 prefab 字体闸门
    4. 最后再管 `manual prompt` 覆盖范围

## 2026-04-03：继续真实施工，先压回测试红错并把 Prompt/Workbench runtime 真源链往 prefab-first 收紧

- 当前主线目标：
  - 回到 `spring-day1` 玩家面主线，优先解决：
    1. `Prompt / 任务列表缺字`
    2. 工作台提示链与交互范围
    3. NPC 头顶提示左下角化
    4. 玩家 / NPC 气泡遮挡与说话者层级
- 本轮子任务：
  1. 按 `sunset-no-red-handoff` 先压回我本轮引入的测试编译红错。
  2. 继续把 `SpringDay1PromptOverlay`、`SpringDay1WorkbenchCraftingOverlay` 的 runtime 选壳逻辑收回到 `prefab-first` 真源。
- 本轮实际做成：
  1. `SpringDay1PromptOverlay.cs`
     - 去掉会复用不兼容旧实例的回退链；
     - `EnsureRuntime()` 现在会退休不兼容或重复实例，尽量避免旧错壳复活。
  2. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 同步收紧 runtime 复用和实例选择；
     - 避免错误 world-space runtime 继续截胡。
  3. `SpringDay1LateDayRuntimeTests.cs`
     - 去掉 `TMPro` 直接依赖；
     - 文本断言改成 `Component + 反射取 text`，从源码层压回 `CS0246: TMPro`。
  4. `SpringDay1InteractionPromptRuntimeTests.cs`
     - 补上 `yield break;`，压回 `CS0161`。
- 当前判断：
  1. 这轮最核心的判断是：我已经先把自己新增的编译红错从源码层清掉了，现在主线可以继续回到真实玩家面问题。
  2. 我最不满意的点是：这轮还没有新的 Unity Console / GameView 证据，所以只能说“源码层已去根因”，不能说“玩家面已经过线”。
  3. 如果我判断错，最可能错在 Unity 现场还残着旧编译缓存或旧 runtime 实例，而不是当前这两份测试源码还在引用 `TMPro`。
- 本轮验证：
  - `git diff --check`：通过
  - `SpringDay1LateDayRuntimeTests.cs` 源码第 6 行已不再是 `using TMPro;`
  - `Assets/YYY_Tests/Editor` 范围内未再扫到该文件对 `TMPro` 的直接命名空间引用
- 本轮 thread-state：
  - `Begin-Slice`：未重跑
    - 原因：`UI.json` 当前已是 `ACTIVE`，沿用现有 slice 继续施工
  - `Ready-To-Sync`：未跑
    - 原因：本轮还没到 sync / 收口阶段
  - `Park-Slice`：未跑
    - 原因：本轮仍在继续
  - 当前 live 状态：`ACTIVE`
- 当前恢复点：
  - 下一步仍按这个顺序继续最稳：
    1. Prompt / 任务列表缺字
    2. 工作台提示链与交互范围
    3. NPC 头顶提示退场
    4. 气泡遮挡与“谁说话谁在上面”

## 2026-04-03：补收 Workbench 内容层排版和 Prompt row 链自愈后停车，当前线程状态已回到 PARKED

- 当前主线目标：
  - 继续 `spring-day1` 玩家面 UI/UE 收口，但这一轮只做两处能直接改善用户当前主矛盾、且不横向改壳体的内容层修正：
    1. `Workbench` 长文本排版
    2. `Prompt` 空壳 row 链自愈
- 本轮子任务：
  1. 把 `SpringDay1WorkbenchCraftingOverlay` 左列配方名的省略号与右侧描述 `60f` 硬截断收回去。
  2. 把 `SpringDay1PromptOverlay` 的壳复用判定再收紧，并在前台 row 写入后补一次自检/重建。
- 本轮实际做成：
  1. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - 左列 `RecipeRow` 名称改成可换行，不再强制 `NoWrap + Ellipsis`。
     - `SelectedName` 改成可换行。
     - `SelectedDescription` 不再被固定 `60f` 截断；现在会按底部动作区可用高度计算上限，把材料区、进度区和提示区一起向下推。
  2. `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
     - `CanBindPageRoot()` 现在要求至少有一条可绑定 `TaskRow_/BulletFill/Label/Detail` 链，半残页不再算“可复用”。
     - `ApplyStateToPage()` 现在会在写完前台页后校验 `page.rows` 的 `label/detail` 是否真的匹配当前 state；如果还是空壳，就重建 row 链并重刷。
- 本轮验证：
  - `validate_script`
    - `SpringDay1WorkbenchCraftingOverlay.cs`：`0 error / 1 warning`
    - `SpringDay1PromptOverlay.cs`：`0 error / 1 warning`
    - `CraftingStationInteractable.cs`：`0 error / 1 warning`
    - `SpringDay1Director.cs`：`0 error / 2 warning`
    - `SpringDay1InteractionPromptRuntimeTests.cs`：`0 error / 0 warning`
    - `SpringDay1LateDayRuntimeTests.cs`：`0 error / 0 warning`
  - `git diff --check`：未出现 owned 阻断错误，仅有 CRLF/LF 提示。
  - Unity Console 最新 error 仍落在 `PersistentManagers.cs / TreeController.cs` 的外部旧现场，不是我这轮 UI own 文件。
  - `run_tests` 目前仍只返回 `total=0`，所以我没有把它当成通过证据。
- 本轮关键判断：
  1. 我这轮最核心的判断是：`Workbench` 现在该先收内容层排版，`Prompt` 现在该先收 row 链健康度，而不是再去重抽象或乱动导演层数据。
  2. 我这轮最薄弱的地方是：还没有新的 live/GameView 证据，所以不能说真实玩家体验已经过线。
  3. 如果我现在最可能看错，错点会在“运行时现场还存在我没在代码层复现到的旧 UI 壳”，而不是当前这两份脚本还有直接编译红错。
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑，原因：这轮没有进入 sync 收口
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 下一轮若继续，我最确信的顺序仍是：
    1. 真实入口复核 Prompt 前台 row 是否彻底脱离“有壳没字”
    2. 继续收工作台 live 交互矩阵
    3. 再回到 NPC 提示与气泡层级的真实体验层

## 2026-04-04：继续落地 8 点未完项，重点收了 Workbench 静止锚定/翻面弹性/当前配方回显，并确认当前 Console 为 0 error

- 当前主线目标：
  - 用户明确要求继续把那 8 条剩余需求往下落，而且再一次强调“不要爆红”；所以这轮继续只在 `Prompt / Workbench / 必要测试` 里推进。
- 本轮子任务：
  1. 把 `Workbench` 常态定位从持续平滑跟随改回“相对静止”。
  2. 只把上下翻面保留为弹性过渡。
  3. 让已有制作队列时，重新打开大 UI 能直接回到当前配方。
  4. 把 `Prompt / Workbench` 两条关键护栏补进测试。
- 本轮实际做成：
  1. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - 常态定位不再 `Lerp` 漂移；
     - 只有翻面时才 `SmoothDamp` 做竖向弹性；
     - 悬浮小框改成直接贴锚点；
     - 重新打开工作台时会优先选中当前正在制作的配方；
     - 左列坏壳误复用判定再收紧一层；
     - 悬浮图标放大到 `28x28` 且保持 `45°`。
  2. `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
     - Prompt 测试改成直接验前台 row 文本；
     - 新增 Workbench 左列坏壳误复用测试。
- 本轮验证：
  - `validate_script`
    - `SpringDay1PromptOverlay.cs`：`0 error`
    - `SpringDay1WorkbenchCraftingOverlay.cs`：`0 error`
    - `CraftingStationInteractable.cs`：`0 error`
    - `SpringDay1Director.cs`：`0 error`
    - `SpringDay1InteractionPromptRuntimeTests.cs`：`0 error`
    - `SpringDay1LateDayRuntimeTests.cs`：`0 error`
  - `git diff --check`：无阻断错误，仅 CRLF/LF 提示
  - Unity Console：最新读取 `0 error`
  - `run_tests`：仍只返回 `total=0`，我没有把它当成有效通过证据
- 本轮关键判断：
  1. 这轮最核心的判断是：你第 7、8 条里的大偏差已经从“结构没对齐”收到了“live 手感还需要真实入口再看”。
  2. 我这轮最不放心的地方仍然是没有新的真实 GameView 证据，所以我不会把它写成体验已过线。
  3. 如果我这轮最可能看错，错点会在 live 入口下还有个别手感边界，而不是当前源码还在爆红。
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑，原因：这轮没有进入 sync
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 下一轮若继续，我最确信的顺序仍是：
    1. 真实入口复核 Prompt 首屏任务栏与前台 row
    2. 继续收 Workbench 交互矩阵和 live 手感
    3. 再回到 NPC 提示与气泡层级

## 2026-04-04：用户要求先提交当前可提交内容，本线程先吸收新协同边界，再结算 UI own checkpoint

- 当前主线目标：
  - 主线没有变，仍是 `spring-day1` 玩家面 UI/UE 收口；
  - 本轮子任务先切到：
    1. 吸收 `UI线程_继续施工引导prompt_04`
    2. 按新边界确认不再回吞 `Director`
    3. 先把当前能负责提交的 UI own 内容提交掉
    4. 提交后继续往下做 `Prompt / Dialogue / Workbench`
- 这轮新增关键事实：
  1. 我已经补读并吸收：
     - `2026-04-04_UI线程_继续施工引导prompt_04.md`
     - `2026-04-04_UI线程_剧情源协同开发提醒_03.md`
     - `2026-04-04_UI线程_玩家面续工与剧情源协同prompt_03.md`
  2. 新边界已经明确：
     - 继续 own 玩家面显示结果；
     - 不因为剧情源将扩写就停工；
     - 也不再继续扩写 `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`。
  3. 本轮在继续收 UI 前，又先压了一刀当前编译硬阻塞：
     - `DialogueUI.cs` 的 `StringComparison` 比较条件已改成 `System.StringComparison.OrdinalIgnoreCase`；
     - 这条红错从源码层已经被切断。
  4. `SpringDay1WorkbenchCraftingOverlay.cs` 又推进了一刀左列显示链：
     - 绑定 row 时就恢复 `Name / Summary` 文本可见态；
     - 刷新 row 时再次恢复；
     - “文本非空但组件失活/透明”的坏壳，开始会被判成不可继续复用。
  5. 当前 checkpoint 仍只站在：
     - `结构 / targeted probe`
     - 还没有新的玩家可见终验证据。
- 当前验证状态：
  1. 当前可用验证：
     - UI own 白名单 `git diff` 核查
     - `git diff --check`
     - 接下来准备跑 `Ready-To-Sync`
  2. 当前不可用验证：
     - `unityMCP validate_script`
     - 原因：本机 `http://127.0.0.1:8888/mcp` 连接失败
- 当前恢复点：
  1. 先完成当前 UI own checkpoint 提交。
  2. 提交后立刻继续：
     - `Prompt / Dialogue` 缺字链
     - `Workbench` 左列/正式面/状态机
     - 悬浮框与拾取/取消闭环

- 2026-04-04 16:50 Prompt/Dialogue 定向施工：
  - 当前主线目标没有变化，仍是 `Story / NPC / Day1` 玩家面体验链里的 UI/UE 收口；本轮子任务是用户新收口后的窄切片：只修 `Prompt / Dialogue` 字链，且只能动 `SpringDay1PromptOverlay.cs` 与 `DialogueUI.cs`。
  - 本轮实际做成：
    1. `SpringDay1PromptOverlay.cs`
       - readable recovery 现在会检查 `subtitle`；
       - 坏壳判定新增“当前显示文字是否等于期望文字”的校验；
       - page 匹配与复用条件收紧到文本链、字体链、alpha、Rect 尺寸这一层。
    2. `DialogueUI.cs`
       - `continue` 标签引用恢复从“拿到任意子文本就算”升级成“选择可复用标签，否则新建标签”；
       - 补了 `continue` 标签 Rect 归正、父链可见性恢复；
       - 非中文/空白/英文 continue 文案统一自愈为 `继续`，并把空文本探针直接切到中文。
  - 本轮验证：
    - `CodexCodeGuard` 已对这 2 个脚本完成 `utf8-strict + git diff --check + roslyn-assembly-compile`，结果通过；
    - `Ready-To-Sync` 未通过，但 blocker 不是这两份脚本，而是 `Assets/YYY_Scripts/Story/UI` own root 里还挂着其他历史 dirty：`InteractionHintOverlay.cs`、`NpcWorldHintBubble.cs`、`SpringDay1WorldHintBubble.cs` 等。
  - 当前阶段判断：
    - 这轮只能算 `Prompt / Dialogue` 的 targeted probe 前进；
    - 不能宣称玩家侧已过线，也不能宣称 UI 线程已整体可 sync。
  - 下一步恢复点：
    - 如果继续沿当前切片推进，优先拿玩家面证据验证 `任务卡 / continue` 仍是否存在“有框无字”；
    - 如果准备线程级 sync，必须先处理当前 own root 内剩余 dirty 的归属与收口。

- 2026-04-04 17:29 OpeningRuntimeBridgeTests 红错回查：
  - 当前主线目标仍是 `spring-day1` 玩家面 UI/UE 收口；本轮子任务是用户新报的 `SpringDay1OpeningRuntimeBridgeTests.cs` 测试红错排查，它只是主线上的编译阻塞处理，不是换线。
  - 本轮实际做成：
    1. 重新核对了 [SpringDay1OpeningRuntimeBridgeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1OpeningRuntimeBridgeTests.cs) 当前磁盘版，确认它已是反射式实现，不再直接依赖 `Sunset.*` runtime 类型；
    2. 重新核对了同批 opening 测试与 [Tests.Editor.asmdef](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/Tests.Editor.asmdef)；
    3. 用 `CodexCodeGuard` 对 `SpringDay1OpeningRuntimeBridgeTests.cs`、`SpringDay1OpeningDialogueAssetGraphTests.cs`、`DialogueChineseFontRuntimeBootstrapTests.cs` 做了 `Tests.Editor` 程序集级编译检查，结果通过；
    4. 用 `python scripts/sunset_mcp.py no-red Assets/YYY_Tests/Editor/SpringDay1OpeningRuntimeBridgeTests.cs --owner-thread UI --count 20` 做了 fresh no-red 归属检查，结果 `assessment=no_red / owned_errors=0 / external_errors=0`。
  - 关键判断：
    - 用户贴出的 `Sunset / Type / SpringDay1Director` 这组报错行号，已经和当前磁盘版文件对不上，因此那组错误不再对应当前文件事实，更像旧编译快照或旧版本内容；
    - 当前这组 opening 测试在代码层已经 clean，但这不等于主线 UI/UE 体验已经过线。
  - 当前验证状态：
    - `git diff --check`：通过
    - `CodexCodeGuard`：通过
    - `sunset_mcp.py no-red`：目标文件 owned red = 0
    - Unity Console 仍有外部无归属异常：空路径 `NullReferenceException` 与无音频监听警告，不属于这份测试文件
  - 当前恢复点：
    1. 编译阻塞已从这份 opening 测试文件上解除；
    2. 主线恢复到 `Workbench 产物留台 / 拾取 / 取消 / 悬浮框进度` 的运行时闭环。

- 2026-04-04 19:08 Workbench 留台/领取/取消 第一轮闭环：
  - 当前主线目标没有变化，仍是 `spring-day1` 玩家面 UI/UE 收口；本轮子任务继续服务主线，专门把 `Workbench` 从“自动入包旧语义”推进到“留台待领 + 取消返还 + 悬浮延续”的第一轮闭环。
  - 本轮实际做成：
    1. 在 [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs) 里接入工作台队列条目状态；
    2. `CraftRoutine()` 改走 [CraftingService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Crafting/CraftingService.cs) 已有的 `TryCraft(recipe, false)`，不再默认自动入包；
    3. 制作条点击现在开始分出“领取优先 / 否则取消”的语义，追加制作继续保留给数量与按钮链路；
    4. 取消制作时如果当前单件已预扣材料，会尝试返还到背包，塞不下就掉到世界里；
    5. 关闭工作台面板后，只要还有活跃制作或未领取产物，悬浮状态就不会再被会话清理直接抹掉；
    6. [CraftingStationInteractable.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs) 的左下角 ready 提示文案补出了“普通打开 / 制作中 / 有可领取产物”分支。
  - 当前验证：
    - `CodexCodeGuard` 对 `SpringDay1WorkbenchCraftingOverlay.cs + CraftingService.cs + CraftingStationInteractable.cs` 通过；
    - `sunset_mcp.py no-red` 返回 `owned_errors=0 / external_errors=0`；
    - 但 Unity fresh ready 仍没闭环，因为当前 MCP 状态卡在 `stale_status`，所以不能把它包装成“Unity 终验完成”。
  - 当前恢复点：
    1. `Workbench` 这刀已经从“服务层根语义错误”推进到了“代码层闭环成立”；
    2. 下一步先转去继续压 `Prompt / 任务 / continue` 缺字链，再视情况回到工作台做多浮窗与进一步 polish。

- 2026-04-04 20:xx 制作条入口回正 + 文本链后置字体修复：
  - 当前主线目标仍是 `spring-day1` 玩家面 UI/UE 收口；本轮继续真实施工，不是换线。
  - 本轮子任务有两个：
    1. 把 `Workbench` 里“领取 / 取消”的点击入口从残留的 `craftButton` 彻底收回到制作条；
    2. 补 `Prompt / Workbench` 文本在“真正写入中文以后”再次做字体/alpha/mesh 校正的后置兜底，针对用户反复报的“有框无字 / 能点无字”。
  - 本轮实际做成：
    1. [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
       - `progressButton` 正式接回绑定链；
       - 新增 `OnProgressBarClicked()`；
       - `craftButton` 只保留“开始 / 追加”，不再承担“领取 / 取消”；
       - 领取/取消 hover 语义改看 `_progressBarHovered`；
       - 新增 `EnsureWorkbenchTextContent()`，把左列 recipe、右侧详情、进度条、按钮、提示条在中文真正落进去后再补一次可读性校正。
    2. [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
       - `EnsurePromptTextContent()` 改为实例级；
       - 标题、副标题、focus、footer、row label/detail 在赋值后再按当前文本二次校正字体可读性。
  - 本轮关键判断：
    - 任务栏空白、Prompt 有壳无字、Workbench 左列能点无字，很像是“空字符串阶段没触发换中文字体，后面写入中文时又没再补一次字体校正”；这轮是按这个根因去补的。
  - 本轮验证：
    1. `python scripts/sunset_mcp.py no-red --owner-thread UI` 对：
       - [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
       - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
       - [CraftingService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Crafting/CraftingService.cs)
       - [CraftingStationInteractable.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs)
       返回 `owned_errors = 0 / external_errors = 0`；
    2. `CodexCodeGuard` 通过；
    3. Unity fresh ready 仍被 `stale_status` 卡住，所以本轮验证层级仍是“代码层 clean + targeted no-red”，不是 live 体验终验。
  - 阻塞 / 恢复点：
    1. 当前没有 own red；
    2. 下一步恢复到：继续追 `Prompt / continue / Workbench 左列` 的 live 证据，确认这轮后置字体兜底是否真的把缺字链压住。

- 2026-04-04 中场协同判断：给 `day1` 一份“opening 验证闭环 / 第一真实 blocker”窄切 prompt
  - 当前主线目标没有换题，仍是 `spring-day1` 玩家面 UI/UE 收口；本轮子任务是用户中场休息时让我看 `day1` 最新回卡，并判断要不要给它新的 prompt。
  - 本轮判断结果：
    1. 需要给；
    2. 但不该再让 `day1` 继续补 opening 结构；
    3. 当前最合理的切刀必须压回：
       - `opening 验证闭环`
       - 或 `第一真实 blocker 报实`
  - 本轮新增 prompt 文件：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_spring-day1_opening验证闭环与第一真实blocker续工prompt_06.md`
  - 这份 prompt 的作用：
    1. 冻结当前已接受基线：non-UI opening 归 `spring-day1`，UI 不回吞；
    2. 强制 `day1` 只交 fresh opening 验证证据，或者第一真实 blocker；
    3. 明确禁止它继续扩 opening 文案、扩结构、或回漂到 UI。
  - 当前恢复点：
    - 如果用户要现在转发给 `day1`，优先转这份 `prompt_06`；
    - UI 线程自己随后仍回到玩家面缺字链与 Workbench 剩余收口，不接 `day1` 的 non-UI opening owner。

- 2026-04-04 中场协同判断：给 `NPC` 一份“玩家面 NPC 方向分工与第一刀认领” prompt
  - 当前主线目标被用户临时切到“先派工，不继续自己实现”；本轮子任务是根据当前剩余需求，给 `NPC` 线程写一份可直接转发的分工 prompt。
  - 本轮判断结果：
    1. 需要给 `NPC` 一份新 prompt；
    2. 但不该再让 `NPC` 被泛泛要求“去做 NPC 那边的所有 UI”；
    3. 当前最合理的切刀是先让 `NPC` 自己把：
       - `exact-own`
       - `协作切片`
       - `明确不归我`
       三类矩阵说死，再认领第一刀。
  - 本轮新增 prompt 文件：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_NPC线程_玩家面NPC方向分工与第一刀认领prompt_01.md`
  - 这份 prompt 的作用：
    1. 冻结 `UI / spring-day1 / NPC` 三方基线；
    2. 把可分给 `NPC` 的总账集中到：
       - speaking-owner 层级
       - 气泡不被场景遮挡
       - 气泡背景不透明
       - 头顶提示退场与语义一致
       - 正式/非正式聊天闭环
    3. 禁止 `NPC` 继续吞 `Prompt / Workbench / Town DialogueUI / Primary / 全局字体底座`。
  - 当前恢复点：
    - 如果用户要现在转发给 `NPC`，优先转这份 `prompt_01`；
    - 我方线程这轮已 `PARKED`，暂停当前实现，等待派工后再决定下一刀。

## 2026-04-05 00:06｜Workbench 左列 recipe 缺字链单文件补丁

- 当前主线目标没有换：
  - 仍是 `spring-day1` 玩家面 `缺字链`；
  - 本轮子任务被用户再次收窄为：只排查并最小修补 [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs) 里 Workbench 左列 recipe “能点击但没文字”的最可能责任点。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `sunset-no-red-handoff`
- 本轮开工前现场：
  - `UI` 在线程状态层里已存在 `ACTIVE / PromptOverlay 缺字链任务列表稳定化`；
  - 我尝试补跑 `Begin-Slice` 时被脚本拦下，提示同一线程已 `ACTIVE`；
  - 因此本轮沿用现有 `UI` active slice 继续最小施工，没有强行覆盖别的 live 状态。
- 本轮新增稳定判断：
  1. 当前最可能责任点进一步收窄到 overlay 自己的 runtime 壳复用门槛，而不是配方数据源；
  2. 旧实现里 `HasReusableRecipeRowChain()` 只要命中一条可绑定 row 就允许整壳继续复用，`CanReuseRecipeText()` 也还不要求文本非空、alpha 可见、rect 有尺寸；
  3. 这会给“按钮还活着，但 `Name/Summary` 已空、透明或尺寸坏掉”的 stale row 留下复用口子。
- 本轮实际改动：
  1. `HasReusableRecipeRowChain()` 只再看当前 active 的 `RecipeRow_`；
  2. active row 里只要有一条 `Name/Summary` 文本链不完整，整个旧壳就直接判坏，不再继续复用；
  3. `CanReuseRecipeText()` 现在额外要求：
     - `activeInHierarchy`
     - `alpha > 0`
     - `rect.width/height > 2`
     - `text` 非空白
     - 当前字体确实能渲染这段文本
- 本轮验证：
  1. `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs` 通过；只有 `CRLF -> LF` 提示，不是本轮 owned error；
  2. `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --skip-mcp` 在本机超时，未拿到 fresh 程序集级结果；
  3. 因此当前只能诚实写：`最可能责任点已修，代码层文本链门槛已收紧，Unity/live 待验证`。
- 当前恢复点：
  1. 下一步优先看 Workbench 左列 recipe fresh live 是否恢复；
  2. 如果仍空，再继续下钻字体底座或数据注入链；
  3. 不回漂 Workbench 全量 polish。

## 2026-04-05 00:17｜Town 中文 DialogueUI + Prompt/Workbench 缺字链继续施工

- 当前主线目标：继续推进 `spring-day1 / Town` 玩家面缺字链，不把代码层 clean 冒充成体验过线。
- 本轮子任务：
  1. 修 `DialogueUI` 的中文 continue 标签与字体兜底；
  2. 修 `PromptOverlay` 的任务栏 / 任务卡缺字链；
  3. 修 `WorkbenchOverlay` 左列 recipe 空壳复用门槛。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- thread-state 现场：
  - 试图补跑 `Begin-Slice` 时被拦下，因为 `UI` 已存在 `ACTIVE / Town DialogueUI + Prompt/Workbench 缺字链继续收口`；
  - 本轮沿用现有 active slice 继续最小施工；
  - 收尾前会补 `Park-Slice`。
- 本轮实际完成：
  1. `DialogueUI.cs`：continue 标签候选筛选更稳，中文字体预热更早，`任意键继续对话` 之类旧壳文案会收敛成 `继续`；
  2. `SpringDay1PromptOverlay.cs`：坏壳/坏 row 的 readable 判定更严，文本写入后会再次补字体、alpha 和父链可见性；
  3. `SpringDay1WorkbenchCraftingOverlay.cs`：`RecipeRow_` 旧壳复用门槛收紧，能点没字的 stale row 不再继续过关。
- 本轮验证：
  1. `git diff --check` 对 3 个目标脚本通过；
  2. `validate_script` 对 3 个脚本本轮都超时，未拿到 fresh 程序集级结果；
  3. 因此当前只能写 `代码层 / targeted probe 推进`。
- 本轮最薄弱点：
  - 没拿到 fresh Unity/live 证据，不能确认玩家入口 4 个 case 已全部恢复。
- 下一步恢复点：
  1. fresh live 复核：开局左侧任务栏 / 中间任务卡 / 村长 continue / Workbench 左列 recipe；
  2. 若仍缺字，再继续下钻字体底座或数据注入链；
  3. 不回漂到 NPC 整线或 Workbench 全量 polish。

## 2026-04-05｜只读核查：Workbench 左列空白 vs 任务清单工作台阶段异常
- 当前主线未变，仍是 `UI` 线程玩家面缺字链；本轮子任务是用户要求我不要修，只读判断“我是否在代码里看到了这两个 bug”。
- 只读结论：
  1. 我之前没有把这两个问题准确抬到前台，这是我的漏判；
  2. 从代码本身看，确实都存在真实问题，不需要靠截图才能成立。
- 代码层判断：
  1. `WorkbenchOverlay` 左列空白但右侧详情仍正常：
     - 右侧由 `_selectedIndex -> GetSelectedRecipe() -> RefreshSelection()` 驱动；
     - 左侧由 `RefreshRows()` 和 row 文本链单独驱动；
     - 因而 `_selectedIndex` 仍有效时，右侧可以有内容，左侧 row 仍可能空白。
     - 这是局部但结构性的同步缺口。
  2. 任务清单在工作台阶段显示不对：
     - `SpringDay1Director.BuildPromptItems()` 在 `WorkbenchFlashback` 会生成 2 条，在 `FarmingTutorial` 会生成 5 条；
     - 但 `SpringDay1PromptOverlay.BuildCurrentViewState()` 固定传 `maxVisibleItems: 1`；
     - 所以这是 `PromptOverlay` 层面的全局截断策略问题，不是工作台阶段数据没给。
- 当前恢复点：
  - 如果下一步继续做，这两个点要分开处理：
    1. Workbench 左右同步缺口
    2. PromptOverlay 的全局单条截断策略

## 2026-04-05｜只读复核：003->004 与 005 任务清单空窗不是“数据没给”，而是退场逻辑缺失 + 过渡空窗
- 当前主线未变，仍是 `UI` 线程玩家面问题核查；本轮子任务是用户要求我重新只读审查任务清单，不要把问题说得过头。
- 修正后的结论：
  1. `WorkbenchFlashback` 和 `FarmingTutorial` 的任务数据在 `SpringDay1Director` 里都是明确生成的，不是任务源没给；
  2. `PromptOverlay` 只是被写成“只显示当前那一条”，所以“做完后后面的会显示”与代码一致；
  3. 正式剧情对话时任务清单本应隐藏，但 `_suppressWhileDialogueActive` 没有真正接上，`OnDialogueStart/OnDialogueEnd` 都写成了 `false`；
  4. 同时 `DialogueUI` 在隐藏其他 UI 时显式排除了 `SpringDay1PromptOverlay`，所以正式剧情时任务清单不会正确退场；
  5. `003 -> 004` 与 `005` 出现短暂“只剩黑色透明底板”的现象，更像 `PromptOverlay` phase 切换时 `PlayPageFlip()` 带来的过渡空窗，新页壳先出来，文字链稍后补上。
- 当前性质判断：
  - 不是任务数据广泛缺失；
  - 是 `PromptOverlay <-> DialogueUI` 协同层的问题；
  - 其中“正式剧情不隐藏”是明确代码 bug，“阶段切换短暂空窗”是高概率同源 bug。
- 用户新要求已记住：
  - 正式剧情前 `0.5s` 隐藏全部 UI，之后 `0.5s` 渐显对话框；
  - 内心活动背景不要整块半透明；
  - 剧情字体换成更清楚的像素字。

## 2026-04-05｜只读治理收口：审核 NPC 回执、下发 NPC 下一刀、重排 UI 自身剩余任务
- 当前主线目标：
  - 不继续实现；先把 `NPC` 和 `UI` 的 shared 玩家面边界、下一刀分工、以及我自己的剩余任务清单收清。
- 本轮子任务：
  1. 审 `NPC` 回执是否可接受；
  2. 给 `NPC` 一份不漂移的下一刀 prompt；
  3. 把 `UI` 自身下一刀真正收窄。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-prompt-slice-guard`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮实际完成：
  1. 接受 `NPC` 的大边界：
     - `NPCBubblePresenter.cs`
     - `PlayerNpcChatSessionService.cs`
     - `speaking-owner / pair / ambient bubble` 底座
  2. 明确 `UI` 继续 own shared 玩家面壳：
     - `InteractionHintOverlay.cs`
     - `SpringDay1PromptOverlay.cs`
     - `SpringDay1WorkbenchCraftingOverlay.cs`
     - `DialogueUI.cs`
  3. 额外收紧一条：
     - shared 提示壳 hide/show 接口仍归 `UI`
     - `NPC` 只负责 NPC own 语义真值与 contract
  4. 已落新 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-05_NPC线程_气泡层级遮挡与不透明基线第一刀prompt_02.md`
     - 只要求 `NPC` 做：
       - 说话者在上
       - 气泡不被场景压
       - 背景不透明
- 当前我对自己线的最新任务清单：
  1. `PromptOverlay / DialogueUI` 协同链：
     - 正式剧情时任务清单正确退场
     - `003 -> 004 / 005` 阶段切换后任务文字稳定显示
  2. `PromptOverlay` 任务显示策略：
     - 当前被硬截成只显示 1 条，需决定是只修稳定性，还是改回阶段内更完整显示
  3. `WorkbenchOverlay` 左列 recipe 空白：
     - 确认仍是 `UI own` 局部缺口，但优先级暂时排后
- 当前我下一刀最深允许面：
  1. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  2. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs`
  3. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  4. 必要时：
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
- 当前我明确不再吞：
  - `NPCBubblePresenter.cs`
  - `SpringDay1WorkbenchCraftingOverlay.cs` 的全量 polish
  - `InteractionHintOverlay.cs`
  - `Primary.unity`
  - `GameInputManager.cs`
- 当前判断：
  - 用户最新主矛盾已经不是继续泛收 UI 全链，而是 `PromptOverlay <-> DialogueUI` 的任务清单 / 正式剧情协同 bug；
  - 我下一刀如果再回去打 `Workbench` 全量体验或 `NPC` shared 壳，会再次偏题。
- thread-state：
  - 本轮仍是 docs/prompt 治理，不进真实业务施工；
  - 已补 `Park-Slice`；
  - 当前 live 状态：`PARKED`。

## 2026-04-05｜真实大刀施工：玩家面任务卡 / 对话 / 工作台 / 左下角提示同轮推进
- 当前主线目标：
  - 不再停在小刀分析；按用户要求把我 own 的玩家面链条尽可能一路往深处落，直到再继续写代码只会撞到外部 blocker 或他线 active scope。
- 本轮子任务：
  1. 修 `PromptOverlay / DialogueUI` 协同；
  2. 修 `WorkbenchOverlay / CraftingStation` 的玩家面提示与进度语义；
  3. 给左下角 `InteractionHintOverlay` 补 formal fallback。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- thread-state：
  - 本轮进入真实施工前已跑 `Begin-Slice`
  - 收尾已跑 `Park-Slice`
  - 当前状态：`PARKED`
- 本轮实际做成了什么：
  1. [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
     - `OnDialogueStart` 不再是假抑制；
     - `OnDialogueEnd` 会恢复 pending state；
     - 对话真实 active 时才继续压住任务卡；
     - phase 切换默认不再优先走翻页，而改成稳定即时刷新，降低“只剩底板没字”。
  2. [DialogueUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/DialogueUI.cs)
     - 正式剧情开始 / 结束时，其他 UI 和对话框淡入淡出最小时长都收到 `0.5s`；
     - `PromptOverlay` 不再被排除在非对话 UI fade 之外；
     - 内心独白背景 alpha 明显收轻，优先保文字可读。
  3. [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
     - 默认进度文案改回 `进度 0/0`；
     - stage hint 的大部分 hover 废话清空；
     - 进度条 hover 改成 `领取产物 / 中断制作`；
     - 完成制作后不再往 `PromptOverlay` 乱弹“已完成制作”；
     - 左列 recipe 刷新后如果 row 还是不可读，会强制 rebuild 一次；
     - active queue 的 `_craftQueueCompleted` 也会参与“有可领产物”判断。
  4. [CraftingStationInteractable.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs)
     - `BuildWorkbenchReadyDetail()` 改成当前真实玩家语义：
       - 打开工作台
       - 进度条领取
       - 单件进度
       - 剩余数量 / 当前队列
  5. [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
     - 工作台 detail 缺失时的 fallback 文案不再空白，改成正式工作台语义。
  6. [SpringDay1InteractionPromptRuntimeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs)
     - 补了 `InteractionHintOverlay` 的工作台 fallback detail 回归断言；
     - `BuildWorkbenchReadyDetail` 的断言已对齐新 copy。
- 现在还没做成什么：
  1. `Workbench` 多悬浮 `3x2` 阵列与多配方深队列，这轮没有继续硬吞；
  2. `SpringDay1Director.cs / SpringDay1LateDayRuntimeTests.cs` 因 `spring-day1` 当前 ACTIVE 占用，没有并发去改；
  3. fresh compile / fresh live 证据未闭环。
- 当前阶段：
  - 代码层和玩家面语义层已经往前推了一大截；
  - 但当前仍是 `结构 / targeted probe`，不是体验终验通过。
- 当前最薄弱点：
  - 不是我不知道还该改什么；
  - 而是 `MCP 8888 拒连 + sunset_mcp.py / CodexCodeGuard 超时`，导致我这轮拿不到 fresh compile / live。
- 当前外部 blocker：
  1. `MCP manage_script validate 当前 127.0.0.1:8888 拒连，fresh native validate 不可用。`
  2. `sunset_mcp.py compile/validate_script 与 CodexCodeGuard explicit path 本轮均超时，fresh compile 证据未闭环。`
- 当前恢复点：
  1. 如果继续，就先补 fresh compile / live；
  2. 然后优先抓：
     - `PromptOverlay / DialogueUI` 的正式剧情退场与任务字稳定
     - `Workbench` 左列 recipe 的 fresh live
  3. 如果这些都稳定，再决定是否继续下钻 `Workbench` 多悬浮与多配方队列。

## 2026-04-05｜继续补刀：`PromptOverlay` 缺字与 `Workbench` 左列空白
- 当前主线：
  - 用户继续要求我把还能做的 UI 玩家面问题尽量往深处收；这轮继续只打 `PromptOverlay` 缺字和 `Workbench` 左列空白，不回漂去吞 `spring-day1` 导演层。
- 本轮子任务：
  1. 重新 `Begin-Slice` 后补 fresh compile / no-red 证据；
  2. 并行静态排查 `004/005` 任务卡只剩底板与 `Workbench` 左列“能点但没字”；
  3. 在 own 文件里落最小修复，再合法 `Park-Slice`。
- 本轮显式使用：
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮实际做成：
  1. `PromptOverlay`：
     - 通过代码审查确认主嫌更像字体覆盖链，不像 `Director` 空模型；
     - 已把字体解析从固定 `FontCoverageProbeText` 改成按当前真实文本重算；
     - `ApplyResolvedFontToShellTexts` / `MeasureTextHeight` / `EnsurePromptTextReady` / `EnsurePromptTextContent` 都已收成按当前文本选字。
  2. `Workbench`：
     - 通过代码审查确认主嫌更像旧 prefab/manual recipe 行壳复用，不像 recipe 数据本身空；
     - 已把命中 manual `RecipeRow_*` 时的左列刷新改成优先 `RebuildRecipeRowsFromScratch()`；
     - 左列 `Name / Summary` 的字体链也改成按当前文本重选。
  3. 子智能体辅助只读结论已吸收，且已全部关闭，没有留下悬挂 agent。
- 本轮没做成：
  1. `manage_script validate` 仍拿不到 fresh 结果，因为当前 `No active Unity instance`；
  2. `sunset_mcp.py compile ... --skip-mcp` 即使只压单文件也继续超时；
  3. 因而这轮仍没有 fresh compile / live 证据。
- 当前判断：
  - 这轮代码确实继续往前推了，但当前只能 claim“最可能根因已补刀”，不能 claim“体验已过线”；
  - 最大不确定性不是我还没想到哪里要改，而是工具链不给 fresh compile / live。
- 当前恢复点：
  1. 如果继续真实施工，先解 active Unity instance / compile timeout；
  2. 再拿 `004/005` 缺字与 `Workbench` 左列 recipe 的 fresh live；
  3. 若 live 仍显示任务卡被挤坏，再考虑继续补 `PromptOverlay` 纵向自适应。
- thread-state：
  - 已跑 `Begin-Slice`
  - 未跑 `Ready-To-Sync`
  - 已跑 `Park-Slice`
  - 当前状态：`PARKED`
  - 当前 blocker：
    1. `manage_script validate -> No active Unity instance`
    2. `compile --skip-mcp` 单文件仍超时

## 2026-04-05｜中途看图后继续补刀：Prompt 背景过长空白
- 当前主线：
  - 用户中途贴图说明：Prompt 现在不是“字不够”，而是“内容不多但背景被拉得过长”；我继续只收这条 UI 空白问题。
- 本轮子任务：
  1. 利用用户图片反推 Prompt legacy page 纵向布局问题；
  2. 在 own 文件里继续补 legacy footer 基线；
  3. 再补一轮 Unity 侧 validate / console 证据。
- 本轮实际做成：
  1. `SpringDay1PromptOverlay.cs`
     - `RefreshLegacyPageLayout()` 现在不会再无条件吃 `footerBaselineTop`；
     - 只有当旧 baseline 只比当前内容低很少时才保留，否则 footer 直接跟内容压缩；
     - 这刀就是专门针对“背景很长但内容很少”的空白问题。
  2. fresh Unity 侧信号：
     - `manage_script validate PromptOverlay = 0 error / 1 warning`
     - `manage_script validate WorkbenchOverlay = 0 error / 1 warning`
     - `errors = 0`
  3. 当前 warning 仍是非阻塞同类提示：
     - `String concatenation in Update() can cause garbage collection issues`
- 当前判断：
  - 这轮比上一刀更接近可验状态，因为已经拿到 own validate warning-only + fresh console clean；
  - 但缺 live/GameView，所以我仍只认“代码层 + targeted probe 更扎实了”，不认“体验已过线”。
- 当前恢复点：
  - 下轮如继续，第一优先直接看用户刚指出的这张图对应问题是否已消失；
  - 如果 Prompt 空白已回正，再回头看 `004/005` 缺字与 Workbench 左列 live。
- thread-state：
  - 已补 `Park-Slice`
  - 当前状态：`PARKED`

## 2026-04-05｜只读子代理补记：spring-day1 `004/005` 任务卡缺字根因已收敛到 PromptOverlay 字体覆盖链
- 当前线程主线没有换：
  - 仍是 UI 线程自己的 `PromptOverlay / DialogueUI / Workbench` 玩家面主矛盾；
  - 本轮只是按用户要求做一刀只读代码探索，不进入真实施工。
- 本轮性质：
  - `只读分析`
  - 未跑 `Begin-Slice`
  - 继续保持让 ACTIVE 施工 slice 留给当前 UI 正在跑的主刀。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `delivery-self-review-gate`
  - `sunset-startup-guard` 手工等价
- 本轮实际完成：
  1. 只读复核 `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`、`Assets/YYY_Scripts/Story/UI/DialogueUI.cs`、`Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`。
  2. 交叉回看 `SpringDay1LateDayRuntimeTests.cs`、`SpringDay1MiddayRuntimeBridgeTests.cs`、`SpringDay1DialogueProgressionTests.cs`，确认当前测试有：
     - 任务卡结构/row 自愈
     - 对话 suppress
     - `004 -> 005` phase bridge
     但没有一条专门卡住“`DialogueUI` hide/show 之后，`004/005` 当前 row 文本仍能被当前字体实际吃下”的 fresh readability probe。
  3. 当前最像主根因的不是 `Director` 没给字，而是 `PromptOverlay` 的固定 probe 字体链：
     - `_fontAsset` 只按 `FontCoverageProbeText = "当前任务工作台继续制作"` 选一次；
     - `004/005` 文案出现了 probe 外的大量字符，如 `靠近 / 看完 / 回忆 / 开垦 / 花椰菜 / 浇水壶 / 收集 / 木材 / 真正 / 基础`；
     - `EnsurePromptTextReady()` / `EnsurePromptTextContent()` 又不会按新文本重选字体，所以会留下“底板活着、字没出来”的假活态。
  4. `DialogueUI` 更像放大器：
     - `FadeNonDialogueUi()` / `ApplyNonDialogueUiSnapshot()` 会在对白时把 `PromptOverlay` sibling UI 整体 hide / restore；
     - 因而 `003 -> 004`、`004 -> 005` 这种对白收束边界正好最容易把这个字体缺口打出来。
  5. 为什么玩家看到的是“一条黑色透明底条”而不是整卡消失：
     - `PromptOverlay` 当前只显示 1 条 primary row；
     - `ApplyRow()` 会继续把 `row.root / row.group / row.plate / row.bulletFill` 保持在可见态；
     - 真正丢的是 TMP 文本可读性，不是 row 容器本身。
- 当前自评：
  - 这轮判断我给 `8/10`；
  - 最有把握的是“不是 Director 空模型”这件事已经坐实；
  - 最薄弱点是还没有 fresh live 去直接目击当前字体资产在 `004/005` 边界的实际 render 行为。
- 当前恢复点：
  1. 若当前 UI 主刀继续真修，第一刀应优先只收在 `SpringDay1PromptOverlay.cs`：
     - 让字体选择按当前目标文本重算，而不是固定 probe；
     - 并在 `DialogueUI` hide/show 后对 `004/005` row label/detail 加 fresh-readability runtime test。
  2. 暂不建议先回改 `SpringDay1Director` 文案或扩大到 `DialogueUI` 整体 fade 策略。

## 2026-04-05｜只读分析：Workbench 左列 recipe“能点但像空白”根因收束
- 用户目标：
  - 作为 `Sunset UI` 线程的代码探索子代理，只做只读分析，不改业务文件；
  - 检查 Workbench 左列 recipe 栏“可以点击但文字经常不显示/像空白”的最可能根因，并给出最小修复面。
- 当前主线 / 子任务 / 恢复点：
  - 当前 UI 线程主线仍是 `玩家真正看到的正式面结果`；
  - 本轮子任务收窄为 `SpringDay1WorkbenchCraftingOverlay` 左列 recipe 空白归因；
  - 本轮结论已回收到“优先收紧壳复用 + 补左列 fresh-readability test”，后续若真修应从这一步继续。
- 本轮只读检查：
  - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1UiPrefabRegistry.cs`
  - `Assets/222_Prefabs/UI/Spring-day1/SpringDay1WorkbenchCraftingOverlay.prefab`
  - `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
  - `Assets/Resources/Story/SpringDay1Workbench/Recipe_9100_Axe_0.asset`
  - `Assets/Resources/Story/SpringDay1Workbench/Recipe_9101_Hoe_0.asset`
- 当前稳定结论：
  1. 数据源不是主嫌：
     - `EnsureRecipesLoaded()` 能从 `Resources/Story/SpringDay1Workbench` 稳定拿到 recipe；
     - `RefreshRows()` 每轮也会明确写 `Name / Summary`。
  2. 最可能根因 1：
     - `EnsureRuntime() -> CanReuseRuntimeInstance() -> HasReusableRecipeRowChain()` 的复用门槛仍偏宽；
     - 旧壳只要 `Button/Image/Text` 当下看起来过关，就可能被继续复用。
  3. 最可能根因 2：
     - prefab/manual `RecipeRow_*` 不走代码新建的 `HorizontalLayoutGroup + TextColumn`，而是 `Name / Summary` 直挂 row；
     - `UsesManualRecipeShell()` 命中后改走 `EnsureManualRecipeRowGeometry()`；
     - 现有 readable 判据抓不到“文本有值但被父级几何/裁切吃掉”的情况。
  4. 次根因：
     - 更像 `刷新时序 + manual 几何`，不像字体缺 glyph；
     - 因为脚本已经有 `ResolveFont / ApplyResolvedFontToShellTexts / EnsureWorkbenchTextContent` 多层字体兜底。
  5. 为什么会出现“左列空白但仍可点”：
     - row 的 `Button / Image` 链还在；
     - 真正丢的是 `Name / Summary` 的可读性，不是整行交互链。
- 建议最小修复面：
  1. 先只动 `SpringDay1WorkbenchCraftingOverlay.cs`：
     - 收紧 `CanReuseRuntimeInstance()` / `HasReusableRecipeRowChain()`；
     - 或在命中 manual/prefab row 壳时，首次 `RefreshAll()` 前直接 `RebuildRecipeRowsFromScratch()`。
  2. 再补 `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`：
     - 新增“refresh 后左列首行 `Name / Summary` 仍可读”的 probe。
- 验证结果：
  - `静态推断成立`
  - 未改代码、未进 Unity、未跑测试。

## 2026-04-05｜继续真实施工：三处玩家面回归同步回正后停车
- 用户目标：
  - 优先修当前最烦人的 3 个玩家面问题，不要再漂：
    1. 任务列表卡片继续上飘，且黑色半透明底板位置没变；
    2. 独白态大块半透明底板影响可读性，字体也要换成更清楚的像素字；
    3. Workbench 左列 recipe 仍空白，且主面板和浮动进度块像双开一样乱飞。
- 当前主线 / 子任务 / 恢复点：
  - 当前主线仍是 `Story / NPC / Day1 玩家面体验链的 UI/UE 收口`；
  - 本轮子任务收紧为：
    - `PromptOverlay` 几何漂移
    - `DialogueUI` 独白 face
    - `Workbench` 左列与 floating 状态冲突
  - 本轮停手后的恢复点：
    - 先拿 fresh live 看这三处是否真回正，再决定是否继续深挖。
- 本轮实际修改：
  - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - legacy footer 高度不再吃坏掉的旧 rect 高度，继续压住卡片累计拉长/上飘。
  - [DialogueUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/DialogueUI.cs)
    - 独白背景图改为彻底隐藏；
    - 独白字体改走 `DialogueChinese SoftPixel SDF`。
  - [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
    - 停止每次 refresh 都强制重建 manual recipe 行；
    - 面板打开时强制关掉 `floatingProgressRoot`；
    - floating 显示条件改成“主面板当前必须真的不可见”。
- 验证结果：
  - `manage_script validate --name SpringDay1PromptOverlay`：`0 error / 1 warning`
  - `manage_script validate --name DialogueUI`：`0 error / 1 warning`
  - `manage_script validate --name SpringDay1WorkbenchCraftingOverlay`：`0 error / 1 warning`
  - `sunset_mcp.py errors --count 20 --output-limit 5`：`0 error / 0 warning`
  - `git diff --check`：仅 `CRLF -> LF` warning，无 owned blocking error
- 当前判断：
  - 这轮代码层 no-red 已站住；
  - 但 UI/体验层仍缺 fresh live，所以不能把这轮包装成“已经体验过线”。
- thread-state：
  - 沿用已开的 `Begin-Slice`
  - 未跑 `Ready-To-Sync`
  - 已补 `Park-Slice`
  - 当前 live 状态：`PARKED`
  - 当前 blocker：
    - `fresh live verification pending for Prompt tasklist drift, Dialogue inner-monologue face, and Workbench left-column/layout`

## 2026-04-05｜只读汇总：当前 UI 线程剩余未完成项
- 用户目标：
  - 先不要继续闷头实现，而是把“历史上还没做完的内容”重新收成一份言简意赅、能直接下令的清单。
- 本轮性质：
  - `只读分析`
  - 未跑 `Begin-Slice`
  - 继续保持 `PARKED`
- 本轮稳定结论：
  1. 当前第一优先剩余项仍是三块：
     - `Prompt / 任务列表`：自适应仍错，`003/004/005` 阶段仍有黑底无字坏态；
     - `DialogueUI`：独白背景语义仍未完全回正，只能保留更清楚字体；
     - `Workbench`：左列空白、整体越界、主面板与 floating 互斥还缺 fresh live 证明。
  2. 当前第二优先剩余项：
     - Workbench 历史整套状态机与表现层需求仍未闭环；
     - 村长 continue / Town 中文显示链仍未 fresh live 过线；
     - 玩家/ NPC 气泡层级、遮挡与样式边界仍是历史尾账。
  3. 当前最重要的判断：
     - 现在不能把这条线说成“只差一点点 polish”；它还处在“主矛盾已经收窄，但几个直接影响玩家感知的坏态还没清完”的阶段。
  4. 下一轮若继续真实施工，推荐顺序：
     - `Prompt` 高度与缺字
     - `DialogueUI` 独白背景回正
     - `Workbench` 左列与越界
     - 然后再看要不要继续吞 Workbench 大状态机历史尾账

## 2026-04-05｜继续真实施工 checkpoint：把三大块都往前推到代码层 no-red
- 用户目标：
  - 不要再小修，要把当前主矛盾狠狠干一刀，并且保证 no-red。
- 本轮实际做成：
  1. `PromptOverlay`
     - 生成页高度改成按真实可见区块求和；
     - 空 `Subtitle / Focus / Footer / TaskList` 会直接隐藏，不再继续把卡片拉长。
  2. `DialogueUI`
     - 独白分支移除单独背景处理；
     - 当前逻辑已经回到“背景跟普通对话一致，只保留更清楚字体”这一侧。
  3. `Workbench`
     - manual recipe 旧壳被正式判定为不可继续复用；
     - 命中旧壳时会直接重建左列；
     - legacy detail 右侧内容增加了一层按当前文字压缩重排的护栏。
  4. 测试
     - 新增 Prompt 高度策略测试；
     - 新增 Workbench manual 壳不应复用的 runtime test。
- 本轮验证：
  - 3 个 UI 脚本 `mcp validate_script` = `0 error / 1 warning`
  - 2 个测试文件 `mcp validate_script` = `0 error / 0 warning`
  - targeted EditMode tests 已跑过
  - clear console + refresh compile 后，fresh `errors` = `0 error / 0 warning`
  - `git diff --check` 无 owned blocking error，仅 CRLF/LF warning
- 当前判断：
  - 这轮已经把当前最烦人的 3 个主矛盾都真正往前推进了；
  - 但因为还缺 fresh 玩家面 live，所以只能 claim `代码层 no-red + targeted probe 站住`，不能 claim `体验过线`。
- 当前剩余：
  1. 还没拿到 Prompt / Dialogue / Workbench 的 fresh 玩家面证据；
  2. Workbench 大状态机历史诉求仍大段未收；
  3. 气泡层级/遮挡/完整玩家面对话体验还没继续深砍。
- thread-state：
  - 本轮已跑 `Begin-Slice`
  - 未跑 `Ready-To-Sync`
  - 收尾已跑 `Park-Slice`
  - 当前状态：`PARKED`

## 2026-04-05｜只读审计：Inventory / Box held owner 双轨残留与最小收口边界
- 用户目标：
  - 只做只读分析，不改代码；审计 `Assets/YYY_Scripts/UI/Inventory` 与 `Assets/YYY_Scripts/UI/Box` 当前未提交改动，明确 held owner 双轨残留、最小收口方案、以及必须一起改 / 不该碰的文件。
- 当前主线 / 子任务 / 恢复点：
  - 当前主线仍是 `UI 交互链与玩家面收口`；
  - 本轮子任务是把 `箱子 / 背包 held owner` 的未提交状态查清，避免下一刀继续在错误边界上扩写；
  - 本轮结束后的恢复点：若继续施工，先只收 owner 判定层，不扩散到 tooltip / 视觉层。
- 本轮稳定结论：
  1. 双轨真正残留在 `InventoryInteractionManager` 与 `SlotDragContext + InventorySlotInteraction + BoxPanelUI` 的并行 owner 分发，不在 `HeldItemDisplay / ItemTooltip / InventorySlotUI`。
  2. 最小有效收口方案是：
     - 统一 `active held owner` 查询与外部分发入口；
     - 把箱子侧 `_chestHeldByShift/_chestHeldByCtrl/s_activeChestHeldOwner` 并入统一 held session 元数据；
     - 继续复用现有 `SetContainerSlotPreservingRuntime / ReturnHeldToSourceContainer / ReturnChestItemToSource` 这类写回算法，不重写物品写槽逻辑。
  3. 若要真实动刀，必须一起看的文件：
     - `Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs`
     - `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
     - `Assets/YYY_Scripts/UI/Inventory/SlotDragContext.cs`
     - `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`
     - 以及入口桥接 `InventoryPanelClickHandler.cs` / `BoxPanelClickHandler.cs`
  4. 当前不该碰的文件：
     - `HeldItemDisplay.cs`
     - `ItemTooltip.cs`
     - `ItemTooltipTextBuilder.cs`
     - `InventorySlotUI.cs`
     - `InventoryPanelUI.cs`
- 验证结果：
  - `git diff -- Assets/YYY_Scripts/UI/Inventory Assets/YYY_Scripts/UI/Box`
  - `rg` 追踪 `held / owner / SlotDragContext / IsHolding / ReplaceHeldItem / HandleHeldClickOutside`
  - 只读查看 `InventoryInteractionManager.cs`、`InventorySlotInteraction.cs`、`SlotDragContext.cs`、`BoxPanelUI.cs`、`InventoryPanelClickHandler.cs`、`BoxPanelClickHandler.cs`
  - 未改代码、未进 Unity、未跑测试

## 2026-04-05｜继续真修：对话世界硬锁 + 空格继续提示 + Prompt 深色壳体收口 + Workbench 旧 detail 壳淘汰
- 当前主线：
  - `UI` 线继续收 `spring-day1` 玩家面三块主矛盾：正式剧情对话硬锁、任务列表/Prompt 壳体、Workbench 玩家面壳体稳定性。
- 本轮真实施工：
  1. `Dialogue / 世界输入硬锁`
     - `GameInputManager.cs`：正式剧情对话开始时会关闭世界输入、退出放置、立刻中断农具/种植链、暂停当前导航、关闭箱子面板；对话期间 `Update/面板热键/工具使用/右键自动导航` 全部早退；结束后恢复输入、恢复预览与暂停导航缓存。
     - `InventoryInteractionManager.cs`：对话期间手持物会退回来源并重置，点击/拖拽/丢弃/外部点击全部直接封锁。
     - `NPCAutoRoamController.cs`：对话期间 NPC 漫游/物理更新早退，主动停运动、断 ambient chat、隐藏气泡。
  2. `DialogueUI.cs`
     - 推进键从 `T` 改成 `Space`；
     - 继续按钮补了 `空格` 键帽；
     - 继续按钮新增 hover/selected 高亮态；
     - 独白背景继续保持和普通对话一致，不再单独透明化。
  3. `SpringDay1PromptOverlay.cs`
     - 外层深色任务卡高度不再吃隐藏背页的旧高度；
     - 只按当前真正可见 page 参与 shell 高度计算，针对“字不多但黑底被拉很长”补口。
  4. `SpringDay1WorkbenchCraftingOverlay.cs`
     - runtime 复用判定新增 `legacy detail manual shell` 淘汰护栏；
     - 旧右侧手工 detail 壳不再允许继续复用，命中时会走重建，优先压住“左列空白/右侧越界沿用旧坏壳”的历史问题。
  5. 测试补口：
     - `SpringDay1DialogueProgressionTests.cs`：更新 DialogueUI 空格继续断言，并补 Prompt 壳体不该继续吃隐藏背页高度的护栏。
     - `SpringDay1LateDayRuntimeTests.cs`：新增 Workbench 旧 detail 手工壳不应继续被 runtime 复用的 runtime 测试。
- 本轮验证：
  - direct MCP `validate_script`：
    - `DialogueUI.cs` = `0 error / 1 warning`
    - `SpringDay1PromptOverlay.cs` = `0 error / 1 warning`
    - `GameInputManager.cs` = `0 error / 2 warnings`
    - `InventoryInteractionManager.cs` = `0 error / 1 warning`
    - `NPCAutoRoamController.cs` = `0 error / 1 warning`
    - `SpringDay1WorkbenchCraftingOverlay.cs` = `0 error / 1 warning`
    - `SpringDay1DialogueProgressionTests.cs` = `0 error / 0 warning`
    - `SpringDay1LateDayRuntimeTests.cs` = `0 error / 0 warning`
  - fresh console：仅见场景现场 warning / log，未见新的 owned error。
  - `git diff --check`：无 owned blocking error，仅 `CRLF/LF` warning。
  - `run_tests` 发起过 targeted EditMode job，但返回计数口径不稳定；本轮不把它当成体验或完整测试过线证据。
- 当前判断：
  - 这轮已经把“正式剧情期间还能种树苗/拖拽/继续漫游”“任务卡黑底壳体过长”“Workbench 旧坏壳继续复用”三类主矛盾继续往前推；
  - 但目前仍然只是 `代码层 no-red + targeted probe`，不是玩家体验已过线。
- 当前剩余：
  1. 还没拿到 fresh live/GameView 证据，不能 claim 体验过线；
  2. Workbench 大状态机历史需求（拾取/取消/追加/多悬浮/完整布局）仍未整体闭环；
  3. 用户刚才盯的 Workbench 左列与右侧越界，需要 fresh live 再判这次“淘汰旧壳”是否已经真正压住。
- 恢复点：
  1. 下一轮若继续，先做 fresh live 复测：
     - 正式剧情对话期间是否彻底封锁世界交互；
     - Prompt 黑底壳体是否回收；
     - Workbench 是否还在复用旧 detail 壳造成左列空白/右侧越界。
  2. 若上述 live 已压住，再继续深砍 Workbench 大状态机尾账。
- thread-state：
  - 本轮已沿用在跑 slice 的 `Begin-Slice`
  - 未跑 `Ready-To-Sync`
  - 本轮结束已跑 `Park-Slice`
  - 当前状态：`PARKED`

## 2026-04-05｜继续真修：把方案收回简单口径，压任务卡最小高度，修 Workbench prefab 壳误判
- 当前主线：
  - 继续只收 `spring-day1` 玩家面三块：任务列表壳体、Workbench 正式壳体、Dialogue 继续提示。
- 用户新增明确裁定：
  1. 任务清单就该是“全局背景层包文字层”，不要再做复杂壳体逻辑；
  2. Dialogue 继续提示不要再加单独空格按钮/键帽，直接改成一句文案；
  3. Workbench 如果做不好就优先回正式 prefab / 稳定壳体，不要再让代码生成壳继续乱跑。
- 本轮追加施工：
  1. `DialogueUI.cs`
     - 撤掉上一轮额外加的 `空格` 键帽和 hover relay；
     - 继续提示回到单句文案 `按空格继续`；
     - 保留 `Space` 作为推进键，但不再额外造按钮样式。
  2. `SpringDay1PromptOverlay.cs`
     - 把任务卡最小高度继续下压：`MinPageHeight` 从 `178` 降到 `112`；
     - card 初始高度从 `188` 降到 `124`；
     - 空 subtitle/task/focus/footer 区块的最小高度改为 `0`；
     - 壳体高度改成按当前 page 高度 + 小边距，不再硬保留旧大底板。
  3. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 抓到根因之一：prefab 壳在 `TryBindRuntimeShell()` 里会因为“当前还没有预生成 recipe rows”被误判为无效，随后直接回退到那套很丑的代码生成壳；
     - 现在去掉了这条误判：`_rows.Count == 0` 不再让 prefab 壳绑定失败；
     - `HasStableWorkbenchShellBindings()` 不再要求 `_rows.Count > 0`；
     - runtime 复用链 `HasReusableRecipeRowChain()` 也允许“还没生成 recipe row 的稳定 prefab 壳”继续复用；
     - 目标是让 Workbench 优先回到 prefab 正式壳体，而不是一直掉进代码生成壳。
  4. 测试补口：
     - `SpringDay1DialogueProgressionTests.cs` 改成断言 `按空格继续`；
     - 并明确断言不应再出现 `ContinueKeyChipDisplayText` / `ContinueHoverRelay` 这类额外样式扩写。
- 本轮验证：
  - `DialogueUI.cs` / `SpringDay1PromptOverlay.cs` / `SpringDay1WorkbenchCraftingOverlay.cs` direct MCP `validate_script` = `0 error`；
  - `SpringDay1DialogueProgressionTests.cs` direct MCP `validate_script` = `0 error`；
  - `git diff --check` 无 owned blocking error，仅 `CRLF/LF` warning。
- 当前判断：
  - 这轮最关键的修正不是又加了一层 UI，而是把我前面多做的东西删掉，并且抓到了 Workbench 丑壳持续出现的一个真实结构根因；
  - 但仍然没有 fresh live / GameView 证据，所以不能 claim 体验已过线。
- 当前剩余：
  1. 任务卡是否真的不再出现“框对、里面内容高度错、黑底超长”，还需 live 复测；
  2. Workbench 是否已真正回到 prefab 正式壳体，还需 live 复测；
  3. Workbench 大状态机历史诉求仍未整体闭环。
- 恢复点：
  1. 下轮优先 live 看：
     - Prompt 黑底是否真正收回；
     - Workbench 是否不再掉进那套生成壳；
     - Dialogue 继续提示是否已回到纯文案。
- thread-state：
  - 本轮继续施工前已跑 `Begin-Slice`
  - 未跑 `Ready-To-Sync`
  - 本轮结束已跑 `Park-Slice`
  - 当前状态：`PARKED`

## 2026-04-05｜中途续工只读排查：先把任务清单假 owner 排掉，再继续真修
- 当前主线目标：
  - 仍然是你骂得最重的 3 块：
    - 左上任务清单
    - Workbench 左列/整体壳
    - Dialogue 继续提示纯文案
- 本轮子任务：
  1. 先补前置核查和偏好闸门；
  2. 重新 `Begin-Slice` 接回 UI 线程现场；
  3. 不先乱改代码，先把左上任务清单真实 owner 与 Workbench 当前最该砍的方法段查清。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮关键结论：
  1. `Begin-Slice` 一开始被旧 `ACTIVE` 状态拦住，随后已用 `-ForceReplace` 正常接回当前 UI slice。
  2. `SpringDay1StatusOverlay` 已明确排除，它是右上状态条，不是左上任务清单。
  3. 当前编辑态 `Primary` scene 的 `UI` 根层只看到一个 `UI/SpringDay1PromptOverlay`，说明“静态重复实例打架”不是这轮首嫌。
  4. 当前 `UI` 根层真实子链已核对为：
     - `State`
     - `ToolBar`
     - `PackagePanel`
     - `DebugUI`
     - `DialogueCanvas`
     - `SpringDay1PromptOverlay`
     - `SpringDay1WorldHintBubble`
     - `InteractionHintOverlay`
     因此左上任务清单仍大概率在 `PromptOverlay / Dialogue 过场链` 里，不是别的独立 scene 壳。
  5. `Workbench` 已重新缩到具体问题段：
     - `TryBindRuntimeShell() / HasStableWorkbenchShellBindings()`
     - `CanReuseRuntimeInstance() / HasReusableRecipeRowChain()`
     - `RefreshRows() / EnsureRecipeRowCompatibility()`
     - `EnsureMaterialsViewportCompatibility() / AdjustLegacyDetailLayoutToFitCurrentContent()`
- 本轮验证：
  - Unity MCP 只读读取 `Primary` scene 与 `UI` 根层对象
  - shell 只读审查：
    - `SpringDay1PromptOverlay.cs`
    - `SpringDay1WorkbenchCraftingOverlay.cs`
    - `DialogueUI.cs`
  - 本轮未改代码、未跑 fresh compile
- 当前恢复点：
  - 下一轮若继续真修，不再从“猜左上是谁”开始；
  - 直接从：
    1. 左上任务清单真实坏态链继续钉死
    2. `Workbench` 上述四段核心方法直接下刀
    3. `DialogueUI` 继续提示保持纯文案
    往下推进。
- thread-state：
  - 本轮已跑 `Begin-Slice`
  - 中途进度汇报前已跑 `Park-Slice`
  - 当前状态：`PARKED`

## 2026-04-05：继续真施工，先收 prompt footer / NPC 尾巴 / Workbench prefab 壳

- 当前主线：
  - 用户直接点名必须同轮完成：
    1. `FooterText` 不能再掉到 page 外；
    2. NPC 世界提示的气泡尾巴不能再变方块；
    3. `Workbench` 在用户测前尽量把正式 prefab 壳稳住，不再继续越修越散。
- 本轮显式命中：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮做成了什么：
  1. `SpringDay1PromptOverlay.cs`
     - legacy page 高度现在会把 `ContentRoot` 的上下壳体 inset 算回去；
     - 新增 `GetLegacyPageShellInset()` / `GetRectVerticalInsetWithinParent()`；
     - 目标是直接修掉 `FooterText` 掉出 page 的问题。
  2. `NpcWorldHintBubble.cs`
     - 箭头改回独立三角 sprite；
     - 不再使用背景气泡 sprite 冒充箭头；
     - 箭头偏移同步按新三角尺寸收正。
  3. `SpringDay1WorkbenchCraftingOverlay.cs`
     - `RefreshCompatibilityLayout()` 遇到正式 prefab 详情壳时直接停，不再按 legacy fallback 去重排；
     - `BindRecipeRow()` 绑定现成 row 时立即补几何修正；
     - prefab shell 识别增加字段未预绑时的 fallback 查找，降低误判。
  4. `SpringDay1DialogueProgressionTests.cs`
     - 增加了上面 3 个点的文本护栏断言。
- 本轮验证：
  - `manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name NpcWorldHintBubble --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name SpringDay1WorkbenchCraftingOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
  - `git diff --check`：只有 CRLF/LF warning，无新的文本坏口
  - `validate_script` 仍被本机 `dotnet 20s timeout` 卡住，所以当前不能 claim Unity live 已验清。
- 当前判断：
  - 这轮我最有把握的是前两刀：`FooterText` page 壳体高度、NPC 箭头方块问题，都是直接命中根因；
  - `Workbench` 这轮更像是把“正式壳还在被兼容链反复拉坏”这条同源先切断；
  - `Workbench` 是否已经完全回正，仍要等用户 fresh live。
- 当前恢复点：
  1. 如果用户回测 `Workbench` 还坏，下一轮继续顺着：
     - `TryBindRuntimeShell()`
     - `RefreshRows()`
     - `RefreshSelection()`
     这条正式壳链继续深挖；
  2. 这轮先适合让用户验：
     - prompt page 的 `FooterText`
     - NPC 世界提示的尾巴形状
     - `Workbench` 是否还会把正式壳拉裂。
- thread-state：
  - 本轮沿用已存在的 `ACTIVE` UI slice 继续施工
  - 本轮未跑 `Ready-To-Sync`
  - 准备停给用户 fresh 验，因此收尾前补 `Park-Slice`

## 2026-04-05：根据 fresh 截图继续深修，已切掉任务卡 footer 与 workbench 左列文本的同源 top 对齐 bug

- 当前主线：
  - 用户 fresh 截图确认后，当前最需要立刻收口的是：
    1. task card 的 `FooterText` 仍在 page 外；
    2. workbench 左列仍空；
    3. workbench 右侧顶部标题仍不见。
- 本轮新增关键结论：
  1. `SpringDay1PromptOverlay.cs` 和 `SpringDay1WorkbenchCraftingOverlay.cs` 的 `SetTopKeepingHorizontal()` 都有同一个误判：
     - 以前只看 `anchorMin.x / anchorMax.x`；
     - 结果把“横向拉伸、纵向固定”的文本错走了 `offsetMin/offsetMax`；
     - 直接导致 `FooterText`、recipe `Name/Summary` 被写到容器外。
  2. `Workbench` 右侧标题缺失还不只是绑定问题，prefab 的 `SelectedName / SelectedDescription` 几何本身就坏，所以额外补了最小几何回正。
- 本轮新增实际改动：
  1. `SpringDay1PromptOverlay.cs`
     - `SetTopKeepingHorizontal()` 改为按 `anchorMin.y / anchorMax.y` 判断纵向是否拉伸；
     - 不再把 `FooterText` 这类底锚点文本写出 page 外。
  2. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 同样修掉 `SetTopKeepingHorizontal()` 的纵向判定；
     - `EnsurePrefabDetailTextChain()` 增加 `NormalizePrefabDetailShellGeometry()`；
     - 直接把 `SelectedName / SelectedDescription` 拉回可见高度。
  3. `SpringDay1DialogueProgressionTests.cs`
     - 新增对这条同源坐标 bug 的护栏断言；
     - 新增对 `NormalizePrefabDetailShellGeometry()` 的护栏断言。
- 本轮验证：
  - `manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name SpringDay1WorkbenchCraftingOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
  - `git diff --check`：无新的文本层红，只有 CRLF/LF warning
- 当前判断：
  - 这轮已经从“继续猜参数”推进到“真正把坐标写错点切掉”；
  - 如果 fresh 验后仍坏，下一轮应该直接升到运行态实例排查，而不是继续在静态排版里打转。
- 当前恢复点：
  1. 先等用户 fresh 验：
     - task footer
     - workbench 左列
     - workbench 标题
  2. 若仍坏，直接查运行态实例绑定链。

## 2026-04-05：继续只收 task page / workbench 左列这两条现有 UI 问题，已补 legacy page section-root 回正与 recipe 左列硬自愈

- 当前主线目标：
  - 只收用户刚钉死的两个现有 UI 问题：
    1. `任务卡`：所有内容必须在 `page` 内，黑壳只跟着 `page`
    2. `Workbench`：右侧先不大改，只把左列正式 UI 和玩家面名称收顺
- 本轮子任务：
  - 修 `SpringDay1PromptOverlay.cs` 的 legacy page section-root 误绑
  - 修 `SpringDay1WorkbenchCraftingOverlay.cs` 的左列空壳恢复和内部 ID 显示
- 本轮实际完成：
  1. `PromptOverlay`
     - 新增 `ResolveLegacySectionRoot()`，让 legacy `SubtitleText / FocusText / FooterText` 直接 child 回到文本本体，而不是再错误回退到 `page.root`
     - `GetLegacyPageShellInset(page)` 在 `contentRoot == page.root` 时改为 `0f`，让 `page` 只按真实内容自适应
  2. `Workbench`
     - `GetRecipeDisplayName()` 改成内部 ID 检测后优先 `item.itemName`
     - 新增 `NeedsRecipeRowHardRecovery()` / `IsRectReasonablyInsideViewport()`
     - `RefreshRows()` 如果左列仍空壳，会 `RebuildRecipeRowsFromScratch(forceRuntimePrefabStyle: true)`
  3. 测试护栏
     - `SpringDay1DialogueProgressionTests.cs` 追加对上述两条真约束的 string guard
- 本轮验证：
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1WorkbenchCraftingOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`：仅 CRLF/LF warning
- 当前判断：
  - 这轮最核心的判断是：`PromptOverlay` 之前不是单纯高度参数没调对，而是 legacy footer 根节点直接绑错了；`Workbench` 左列也不能再靠“希望 prefab row 自己恢复”，必须要有硬恢复兜底
  - 这轮我最有把握的是 `PromptOverlay` 的 root 修正；我最不放心的是 `Workbench` 左列是否还藏着运行态实例链问题，所以不能偷报体验已过线
- 当前恢复点：
  1. 下一步先让用户 fresh 验：
     - task `FooterText`
     - task 黑壳高度
     - workbench 左列 recipe
     - workbench 标题玩家面名称
  2. 如果 `Workbench` 左列仍空，下一轮直接查运行态实例绑定/viewport，不再继续静态猜排版
- thread-state：
  - 本轮已重新 `Begin-Slice`
  - 本轮未跑 `Ready-To-Sync`
  - 本轮准备停给用户验，因此收尾补 `Park-Slice`

## 2026-04-05：继续针对 fresh 截图深修，已切掉 legacy page 根 stretch 叠高与左列坏模板续命

- 当前主线目标：
  - 继续只收：
    1. 任务卡 `page` 过长
    2. `Workbench` 左列空白与内部 ID 名称
- 本轮子任务：
  - 修 `SpringDay1PromptOverlay.cs` 的 legacy page 根 stretch 叠高
  - 修 `SpringDay1WorkbenchCraftingOverlay.cs` 的左列恢复策略与玩家面工具名
- 本轮实际完成：
  1. `PromptOverlay`
     - `PrepareLegacyPage()` 新增 `NormalizeLegacyPageRoot(page)`；
     - legacy prefab 复用时，`Page` 根先从 stretch 改回固定 page
     - 并在 `PrepareLegacyPage()` 内直接补 legacy section roots，再重记 `defaultPosition / defaultPivot / defaultHeight`
  2. `Workbench`
     - `RefreshAll()` / `RefreshRows()` 的恢复链统一改成 `RebuildRecipeRowsFromScratch(forceRuntimePrefabStyle: true)`
     - 新增 `BuildPlayerFacingInternalName()`，内部 ID 直接映射成 `斧头 / 锄头 / 镐子`
  3. 测试护栏
     - `SpringDay1DialogueProgressionTests.cs` 追加 `NormalizeLegacyPageRoot(page);`
     - 追加 `forceRuntimePrefabStyle: true`
     - 追加 `BuildPlayerFacingInternalName`
- 本轮验证：
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1WorkbenchCraftingOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`：只有 CRLF/LF warning
- 当前判断：
  - 这轮最核心的判断是：任务卡 page 过长的真因在 `Page` 根 stretch；`Workbench` 左列空白的真因在坏模板被反复续命
  - 这轮我最有把握的是 `PromptOverlay`，`Workbench` 左列还得继续等 fresh live
- 当前恢复点：
  1. 下一轮先 fresh 验：
     - 任务卡 page 是否还拉长
     - 左列是否出现内容
     - 名称是否变成玩家面中文
  2. 若左列仍空，下一轮直接查运行态实例、mask、sorting、viewport，不再继续静态修补
- thread-state：
  - 本轮已重新 `Begin-Slice`
  - 本轮未跑 `Ready-To-Sync`
  - 准备停给用户 fresh 验，收尾补 `Park-Slice`

## 2026-04-06：继续真实施工，已把任务卡基础长度、Workbench 左列强制恢复、正式对话字体统一推进到代码层 clean

- 当前主线：
  - 继续收 `spring-day1` 玩家面 UI 三条真问题：
    1. 任务卡要有基础长度，但不能再无故拉长
    2. `Workbench` 左列不能再空白
    3. 正式对话主字体统一到独白像素字体
- 本轮子任务：
  1. 先补偏好闸门与 no-red 口径
  2. 修 `PromptOverlay` 的 legacy page 基础页高
  3. 修 `Workbench` 左列恢复链与右侧字体
  4. 修 `DialogueUI` 的正式对话字体与继续文案
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮实际完成：
  1. `SpringDay1PromptOverlay.cs`
     - legacy page 的最小高度回到底层 `page.defaultHeight`
     - 外层黑壳跟 `ResolveLegacyShellMinimumHeight()` 走，不再缩成一团
  2. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 新增 `ForceRuntimeRecipeRowsIfNeeded()`
     - 左列文本如果仍被挤出 row 外，也会直接判坏并强制 runtime row 重建
     - 右侧详情列新增 `ApplyDetailColumnFontTuning()`，整体字体微抬
  3. `DialogueUI.cs`
     - `ContinueButtonDisplayText` 改为 `摁空格键继续`
     - 正式对话默认字体改到 `innerMonologueFontKey`
  4. `SpringDay1DialogueProgressionTests.cs`
     - 补齐上述 3 条链的 string guard
- 本轮验证：
  - `manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name SpringDay1WorkbenchCraftingOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name DialogueUI --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs --count 20 --output-limit 5`：`blocked`，原因是 `subprocess_timeout:dotnet:20s`
  - `errors --count 20 --output-limit 5`：`errors=0 warnings=0`
  - `git diff --check -- [touched files]`：仅 CRLF/LF warning
- 当前判断：
  - 这轮最核心的推进不是“再猜哪里不对”，而是已经把 `Workbench` 左列恢复从被动修补改成主动硬恢复；
  - 当前最薄弱点仍是缺 fresh live，尤其是左列是否真的显出来、任务卡 page 体感是否已经对。
- 当前恢复点：
  1. 下一轮先让用户 fresh 验：
     - task page 长度
     - `Workbench` 左列
     - `Workbench` 右侧字体
     - 正式对话主字体
  2. 若 `Workbench` 左列仍空，直接升运行态实例 / viewport / mask 读现场。
- thread-state：
  - 本轮沿用已有 `ACTIVE` slice 继续施工
  - 收尾前已补 `Park-Slice`
  - 当前状态：`PARKED`

## 2026-04-06：继续按 fresh 截图深修，已把任务卡底部关系改成底锚约束，并把 Workbench 左列升级成稳定生成式 row

- 当前主线：
  - 继续只修：
    1. task `FooterText/FocusRibbon/page 底边` 关系
    2. `Workbench` 左列和右侧标题/简介坏态
- 本轮实际完成：
  1. `SpringDay1PromptOverlay.cs`
     - 新增 `LegacyTaskToBottomBandMinGap`
     - 新增 `LegacyFocusFooterGap`
     - 新增 `LegacyBottomPadding`
     - legacy page 现在先算底部带，再反推需要的 page 高度
  2. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 左列如果还不是生成式 row，会直接强制重建
     - `CreatePrefabStyleRecipeRow()` 升成 `HorizontalLayoutGroup + VerticalLayoutGroup + TextColumn`
     - 右侧标题/简介改为连位置一起收正
  3. `SpringDay1DialogueProgressionTests.cs`
     - 补 string guard 锁住底部关系和生成式 row
- 本轮验证：
  - `manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name SpringDay1WorkbenchCraftingOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
- 当前判断：
  - 这轮最有价值的推进是两条关系都从“经验修补”改成了“硬约束”
  - 当前最不放心的仍是 fresh live 下左列到底是否已经真出来

## 2026-04-06：插入式支撑子任务，已补一个直跳 0.0.5 的 Workbench 调试开关

- 当前主线没有换：
  - 仍然是 `Workbench` 和玩家面 UI 收口；
  - 这轮只是为了让后续复测不用再走整段剧情。
- 本轮实际完成：
  1. `SpringDay1Director.cs`
     - 新增 `debugSkipDirectToWorkbenchPhase05`
     - 新增 `TryApplyDebugWorkbenchSkip()`
     - 勾开后直接进入 `FarmingTutorial`，并把前置教学目标视为已完成，只保留“回工作台做一次基础制作”。
  2. `SpringDay1DialogueProgressionTests.cs`
     - 补了对应 string guard。
- 本轮验证：
  - `manage_script validate --name SpringDay1Director --path Assets/YYY_Scripts/Story/Managers --level basic`：`clean`
  - `manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
- 当前恢复点：
  - 后续要快速测 `Workbench`，直接开这个开关，不用再把前面剧情手跑一遍。

## 2026-04-06：继续真实施工，已把 Workbench 左列空壳切到布局真因，并补齐 NPC/Dialogue 正式面这一刀的代码层 clean

- 当前主线：
  - 继续只收用户当前还没放过的 3 条：
    1. `Workbench` 左列 recipe 还是空壳
    2. NPC / 玩家正式气泡不能继续变方框
    3. `DialogueUI` 的 NPC 缺省头像和玩家字色要回正
- 本轮显式使用：
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
  - `sunset-unity-validation-loop`
- 本轮手工等价执行：
  - `sunset-startup-guard`
  - `skills-governor`
  - `sunset-workspace-router`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮 thread-state：
  - 续接已开的 `ACTIVE` slice：
    - `Restore NPC bubble last good face + fix workbench full display + bind NPC portrait fallback + fresh live`
  - 这轮结束前会补 `Park-Slice`
- 本轮实际完成：
  1. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 先把 generated row 链正式承认为可复用目标态，不再被自身恢复逻辑反复打坏；
     - fresh workbench capture 已真实坐实“左列仍空壳”；
     - 在此基础上继续把问题切到布局真因：
       - 创建 runtime row 时，`rowLayout.childControlWidth = true`
       - 创建 runtime row 时，`rowLayout.childControlHeight = true`
       - 复用已有 generated row 时，同样强制把 `generatedLayout.childControlWidth / childControlHeight` 拉回真值
     - 这次修的是“子项根本没拿到尺寸”，不是再猜文本内容。
  2. `PlayerThoughtBubblePresenter.cs`
     - `UpdateStyleVisuals()` 开头补 `EnsureBubbleShapeSprites()`；
     - 玩家正式气泡 runtime 每次刷新都会把圆角气泡 body / 倒三角 tail 重新绑回正确 sprite。
  3. `NPCBubblePresenter.cs`
     - 同步补 `EnsureBubbleShapeSprites()` / `ApplyBubbleImageShape(...)`；
     - NPC 正式气泡 runtime 不再允许继续回成方框和方尾巴。
  4. `DialogueUI.cs`
     - 新增 NPC fallback 头像链：
       - 取 `Assets/Sprites/NPC/001.png`
       - 优先用第一行第二张的上半身
     - 玩家独白主文字颜色恢复 `_dialogueBaseColor`
     - destroy 时补释放 fallback 贴图 / sprite。
  5. `SpringDay1Director.cs` + `DialogueDebugMenu.cs`
     - 工作台直跳调试开关继续补成 `EditorPrefs` 可控；
     - 菜单里可直接 `Toggle Skip To Workbench 0.0.5`。
  6. `SpringDay1DialogueProgressionTests.cs`
     - 补 string guard，把 workbench generated row 的 `childControlWidth / childControlHeight = true` 直接锁死。
- 本轮验证：
  - `manage_script validate` 为 `clean`：
    - `SpringDay1WorkbenchCraftingOverlay`
    - `DialogueUI`
    - `NPCBubblePresenter`
    - `PlayerThoughtBubblePresenter`
    - `SpringDay1Director`
    - `DialogueDebugMenu`
    - `SpringDay1DialogueProgressionTests`
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 10`：`errors=0 warnings=0`
  - fresh live 证据：
    - 已拿到 `.codex/artifacts/ui-captures/spring-ui/pending/20260406-124142-995_workbench.png`
    - 这张图直接坐实了“Workbench 左列仍空壳”
  - 修完布局真因后，继续尝试 fresh recapture，但当前被外部 Play / 菜单干扰卡住，没有拿到新的稳定 capture 文件
- 当前判断：
  - 这轮最核心的推进是：终于把 `Workbench` 左列问题从“现象修修补补”推进到“布局系统根因”；
  - 当前最薄弱点仍是缺一张新的稳定复抓图，所以这轮不能把结果包装成“真实体验已过线”。
- 当前恢复点：
  1. 下一轮先补稳定 workbench fresh live，确认左列 icon / 文字是否已真正出现；
  2. 若仍空，再直接读运行态 row 几何 / viewport / mask；
  3. 当前这条线不再回到 task card 微调，主火力继续留在 `Workbench` 和正式对话面。

## 2026-04-06：按用户要求停在这个刀口，已给 day1 输出一份可直接调度的重型回执

- 当前主线没有换：
  - 仍然是 `spring-day1` 玩家面 UI/UE 收口；
  - 这轮只是按用户要求，停在“已经形成可判断刀口”的位置，把现状收成 day1 可直接拿去分配和调度的回执。
- 本轮新增交接物：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_UI线程给day1全量回执_01.md`
- 这份回执里已明确：
  1. 当前 UI 已做成什么
  2. `Workbench` 左列的最新真根因判断
  3. 哪些仍是 `targeted probe / 局部验证`
  4. 哪些还不能 claim 体验过线
  5. day1 后续若要减负，哪些玩家面 UI/UE 可以继续彻底压给我
- 当前停点：
  - 准备收回 `thread-state` 到 `PARKED`
  - blocker 继续报实为：
    - `fresh-workbench-live-recapture-still-pending`
    - `external-play-menu-interference-on-ui-capture-window`

## 2026-04-06：继续按 prompt_21 深砍，当前这一刀站在“可给 day1 回执”的刀口停下

- 用户目标：
  - 继续把 `Day1 玩家面结果 + UI 自家遗留` 一起往最深处推进；
  - 当前主战场固定为：
    1. `Workbench`
    2. `DialogueUI / 正式对话面`
    3. formal / informal / resident 玩家面提示壳 one-shot 对齐
  - 用户中途插入的阻塞子任务：
    - 修我自己引入的 `StringComparison` 编译红
  - 阻塞修完后已回到原主线。
- 本轮实际做成：
  1. `SpringDay1WorkbenchCraftingOverlay.cs`
     - `TryBindRuntimeShell()` 新增左列 runtime 强制恢复：
       - 能读到正式 recipe
       - 但左列还不是 generated row
       - 就直接 `RebuildRecipeRowsFromScratch(forceRuntimePrefabStyle: true)`
  2. `SpringDay1ProximityInteractionService.cs`
     - 候选仲裁顺序改成 `ForceFocus -> CanTriggerNow -> Priority -> Distance`
     - formal 左下角 copy 不再依赖旧 generic prompt 串
  3. `PlayerNpcNearbyFeedbackService.cs`
     - formal priority phase 会停止 automatic nearby feedback
     - 且会主动回收已经显示的环境气泡
  4. `NPCInformalChatInteractable.cs` + `PlayerNpcChatSessionService.cs`
     - formal consumed 后，玩家面 idle 提示最小回落到：
       - `日常交流`
       - `按 E 聊聊近况`
  5. `SpringDay1InteractionPromptRuntimeTests.cs`
     - formal vs informal runtime case 变成更硬版本：
       - informal 更近
       - formal 文案不是旧 generic 串
       - 左下角仍必须落成 `进入任务`
  6. `SpringDay1DialogueProgressionTests.cs`
     - 补字符串守门：
       - priority 先于 distance
       - formal copy 不再依赖 `LooksLikeGenericDialoguePrompt`
       - resident prompt tone 已暴露
       - formal phase nearby feedback 已纳入 suppress 规则
  7. 已给 day1 写出一份新的阶段回执：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_UI线程_给day1阶段回执_25.md`
- 本轮验证：
  - `python scripts/sunset_mcp.py errors`：`errors=0 warnings=0`
  - `python scripts/sunset_mcp.py status`：
    - baseline `pass`
    - bridge `success`
    - `isCompiling=false`
  - touched files 的 `git diff --check`：
    - 无新的 owned 语法/空白问题
  - `validate_script --skip-mcp`：
    - 仍被 `subprocess_timeout:dotnet:60s` 卡住
- 当前关键判断：
  - 这轮最值钱的推进不是又修一个壳，而是：
    1. 把 `Workbench` 左列 runtime 恢复链从“继续赌旧 prefab row 自愈”推进到“绑定时主动强切 generated row”
    2. 把玩家面 one-shot 提示壳从“距离优先 + generic copy 命中”推进到更接近真实 formal contract
  - 但当前最薄弱点仍是：
    - 缺修后 fresh live 图
    - `validate_script` 被外部 codeguard timeout 卡住
- 当前阶段：
  - 可诚实 claim：
    - `结构 / targeted probe 更厚`
    - `fresh console clean`
    - `day1 可消费的玩家面 contract 又增加一层`
  - 不可 claim：
    - `Workbench live 已过线`
    - `DialogueUI live 已过线`
- 当前恢复点：
  1. 下轮优先补 `Workbench` 修后 fresh live / capture 或第一真实 blocker
  2. 再补 `DialogueUI` 正式面 fresh live
  3. 当前这一刀收口前需要：
     - 回写 skill-trigger-log
     - 跑 log health 检查
     - `Park-Slice`

## 2026-04-06：只读侦查，重新审 `DialogueUI / NpcWorldHintBubble / PlayerThoughtBubblePresenter` 的回弹首嫌

- 用户目标：
  - 不改文件，只做代码审查；
  - 查清最近哪些代码最可能导致：
    1. `DialogueUI continue` 文案链回弹
    2. 正式对话或 NPC 相关气泡从倒三角回成方框
    3. 玩家独白 / 正式对话字体与头像 fallback 的潜在回弹点
- 当前主线与子任务关系：
  - 主线仍是 `spring-day1` 玩家面 `UI/UE` 收口；
  - 本轮子任务是只读侦查，为后续真修先缩圈首嫌；
  - 子任务完成后，主线恢复点被重新压到：
    1. `DialogueUI.cs` 的 continue / 字体 / 头像 fallback
    2. 必要时把 NPC 正式气泡问题退回 `NPCBubblePresenter.cs` owner
- 本轮实际完成：
  1. 核对当前 dirty 与最近提交归属：
     - `DialogueUI.cs`
     - `NpcWorldHintBubble.cs`
     - `PlayerThoughtBubblePresenter.cs`
     - `NPCBubblePresenter.cs`
  2. 复核了直接关联：
     - `DialogueFontLibrarySO.cs`
     - `DialogueChineseFontRuntimeBootstrap.cs`
     - `DialogueNode.cs`
     - `DialogueManager.cs`
     - `SpringDay1UiLayerUtility.cs`
     - `PlayerNpcChatSessionService.cs`
  3. 收紧出的最新判断：
     - `DialogueUI continue` 首嫌是 `ContinueButtonDisplayText = 摁空格键继续` + `NormalizeContinueButtonCopy()` + 空文本字体探针三者绑死
     - `DialogueChineseFontRuntimeBootstrap` 的 warmup 文本没覆盖 `摁`，continue 这句存在单独掉字 / 误切 fallback 风险
     - 正式对话与独白当前表面想统一字体，底层却同时走 `innerMonologueFontKey` 与硬编码 `SoftPixel` 两条路径
     - 头像 fallback 现在是写死 `Assets/Sprites/NPC/001.png` 的临时裁切，不是稳定 per-speaker 基线
     - `NpcWorldHintBubble.cs` 目前仍是独立三角 indicator，不是“方框尾巴”的第一嫌疑
     - 真正高风险的是 `NPCBubblePresenter.cs`：旧 `NPCBubbleCanvas` 可复用，但复用后不会像 `PlayerThoughtBubblePresenter` 一样强制重绑圆角 body / 倒三角 tail
- 本轮 owner 边界结论：
  - UI 线程自己能修：
    1. `DialogueUI.cs`
    2. `NpcWorldHintBubble.cs`
    3. `PlayerThoughtBubblePresenter.cs` 的玩家气泡表现层
  - UI 线程当前不该自己吞：
    1. `NPCBubblePresenter.cs`
    2. `PlayerNpcChatSessionService.cs`
    3. 正式对话 / informal 会话 runtime 编排 owner
- 验证结果：
  - 仅静态源码审查 / recent diff / recent commit 归属核对
  - 未改代码
  - 未跑 fresh live
  - 结论层级：`结构 / checkpoint`
- 下一步恢复点：
  1. 若继续真修，先切 `DialogueUI.cs` 的 continue 文案与字体单一事实源
  2. 若用户继续看到 NPC 正式气泡方框化，优先把锅压回 `NPCBubblePresenter.cs` 的旧壳复用链

## 2026-04-06：用户要求 UI 线程转为只读侦查 explorer，已完成 Workbench runtime 空左列与右列重叠的代码审查

- 当前主线目标：
  - 不继续真实施工；
  - 只读查清 `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs` 为什么 runtime `Workbench` 仍可能出现：
    1. 左列 recipe 可点击但无内容
    2. 右侧标题 / 描述 / 材料区重叠或超边界
- 本轮子任务：
  1. 手工等价执行 Sunset 启动闸门，只读确认这是 `spring-day1 / UI` 直接问题；
  2. 只读 `SpringDay1WorkbenchCraftingOverlay.cs` 的 runtime shell / row recovery / detail compatibility 链；
  3. 补看 `SpringDay1WorkbenchCraftingOverlay.prefab` 与 `CraftingStationInteractable.cs` 的直接入口，不扩到别的系统。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `delivery-self-review-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮 thread-state：
  - 这轮始终是只读分析；
  - 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
  - 当前 live 状态继续视为 `PARKED`
- 本轮最关键的判断：
  1. 左列问题的真根因不是单个文本赋值漏掉，而是 `RecipeRow` 恢复逻辑同时存在“blind spot + false positive”：
     - orphan row 不进 `_rows`
     - recovery 只扫 `_rows`
     - generated row 又被 `NeedsRecipeRowHardRecovery()` 的父子关系判断误杀
  2. 右列问题的真根因不是单个 offset 偏了，而是 detail shell 仍有两套旧合同：
     - prefab 壳：固定绝对布局 + Overflow 文本 + 材料区退化成文本本体
     - legacy 手动壳：空间不够时仍强保最小描述高和最小材料区高
- 本轮涉及路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\UI\Spring-day1\SpringDay1WorkbenchCraftingOverlay.prefab`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\CraftingStationInteractable.cs`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory.md`
- 本轮验证：
  - `静态代码审查成立`
  - `未改业务代码`
  - `未跑 Unity / Play / MCP live`
- 当前恢复点：
  - 若用户下一轮授权继续修，最小正确入口应先统一：
    1. `RecipeRow` 唯一生成式结构与 hard recovery 判定
    2. detail 唯一 viewport/layout 结构
  - 不应继续在当前 prefab 绝对坐标上叠局部补丁。

## 2026-04-06：继续真实施工，已把 Workbench 结构合同继续收正，并补 DialogueUI 的 continue/字库回弹护栏

- 当前主线目标：
  - `day1` 暂停后，继续只收我 own 的 UI/UE 遗留；
  - 本轮真实施工仍只盯：
    1. `SpringDay1WorkbenchCraftingOverlay.cs`
    2. `DialogueUI.cs`
    3. `DialogueChineseFontRuntimeBootstrap.cs`
    4. 对应 runtime / bootstrap 测试
- 本轮实际做成：
  1. `SpringDay1WorkbenchCraftingOverlay.cs`
     - `NeedsRecipeRowHardRecovery()` 不再把 generated `TextColumn` row 误判成坏壳；
     - `RefreshCompatibilityLayout()` 新增 `RefreshPrefabDetailShellGeometry()`，让 prefab detail shell 也按内容重新排；
     - `EnsureMaterialsViewportCompatibility()` 在 prefab 壳下补回真实 `MaterialsViewport -> Content -> SelectedMaterials`，不再让材料区直接退化成文本本体；
     - `GetActionAreaFloorTop()` 也把 `QuantityTitle` 纳入 floor 计算，减少材料区继续压到下半区。
  2. `SpringDay1LateDayRuntimeTests.cs`
     - workbench 旧进度测试已回正为当前真实文案：`进度  0/1`。
  3. `DialogueUI.cs`
     - continue 显示文案与字体探针拆成独立常量；
     - `ApplyInnerMonologuePresentation()` 先走 `innerMonologueFontKey`，只在必要时才退回 `SoftPixel` 资源 fallback。
  4. `DialogueChineseFontRuntimeBootstrap.cs`
     - warmup 串新增 `摁空格键继续`。
  5. `DialogueChineseFontRuntimeBootstrapTests.cs`
     - bootstrap 探针已同步覆盖 `摁空格键继续`。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮并行只读侦查：
  - 开了 2 个 `gpt-5.4 explorer`
  - 分别审 Workbench 左列/右列真根因 与 DialogueUI/NPC 气泡回弹归属
  - 侦查结论已吸收后关闭，未留活 agent
- 本轮验证：
  - `manage_script validate`
    - `SpringDay1WorkbenchCraftingOverlay`：`clean`
    - `SpringDay1LateDayRuntimeTests`：`clean`
    - `DialogueUI`：`clean`
    - `DialogueChineseFontRuntimeBootstrap`：`clean`
    - `DialogueChineseFontRuntimeBootstrapTests`：`clean`
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 10`：`errors=0 warnings=0`
  - `git diff --check -- [touched files]`：无新的 owned 语法/空白问题，仅 `CRLF/LF` 提示
- 当前最核心判断：
  1. `Workbench` 这轮继续从“调布局表象”推进到了“收唯一容器链和唯一 row 合同”；
  2. `DialogueUI` 这轮最值钱的不是换文案，而是把 continue 字体掉字的自打架链切开。
- 当前最薄弱点：
  - 仍未拿到 fresh live / GameView 证据；
  - 因此不能把这轮包装成玩家面体验已过线。
- 当前恢复点：
  1. 下轮优先补 `Workbench` fresh live，确认左列与右列在玩家视面都已真正恢复；
  2. 若用户继续看到 NPC 正式气泡方框化，优先把锅压回 `NPCBubblePresenter.cs` owner，不由 UI 线程回吞。

## 2026-04-06｜继续真实施工：按 prompt 28 守 resident 终态，并把 UI live 证据链补厚

- 当前主线目标：
  - 继续只做 `Day1 玩家面结果 + UI 自家遗留`；
  - 新口径固定为：
    1. formal one-shot 玩家面
    2. formal consumed 后 resident / informal fallback
    3. `Workbench / DialogueUI` fresh live 证据
    4. 不回漂到泛 UI 修补
- 本轮子任务：
  - 把 UI evidence 从“只会抓 Prompt/Workbench”推进到“能直接证明 DialogueUI、左下角提示壳和 Workbench 左列真值”。
- 本轮做成：
  1. `Assets/YYY_Scripts/Story/UI/SpringUiEvidenceCaptureRuntime.cs`
     - 新增 `dialogue` 侧车
     - 新增 `interactionHint` 侧车
     - `workbench` 新增 `runtimeShellSummary`
  2. `Assets/Editor/Story/SpringUiEvidenceMenu.cs`
     - 新增 `Play Dialogue + Capture Spring UI Evidence`
  3. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 新增静态护栏，防止上述证据链被删回去
- 本轮验证：
  - `manage_script validate`
    - `SpringUiEvidenceCaptureRuntime`：一度 `clean`
    - `SpringUiEvidenceMenu`：`clean`
  - `git diff --check`
    - 3 个 touched 文件通过
- 本轮 live 尝试：
  1. 通过 `CodexEditorCommandBridge` 发 `PLAY`
  2. 现场卡到：
     - 外部 compile red：`SpringDay1NpcCrowdDirector.cs` 缺 `EnumerateAnchorNames`
     - MCP transport 坏态：`status/errors => 'str' object has no attribute 'get'`
  3. 已发 `STOP`，现场退回 `EditMode`
- 当前最核心判断：
  - 这轮最值钱的不是再改 Prompt/Dialogue 壳，而是把“下一次 live 一旦恢复就能直接证明 formal/resident/workbench 是否正确”的证据链搭完整。
- 当前最薄弱点：
  - 没拿到 fresh `DialogueUI / Workbench` GameView；
  - 原因不是 UI 入口缺失，而是外部 compile + MCP transport 双 blocker。
- 当前恢复点：
  1. 清 `SpringDay1NpcCrowdDirector.cs` 外部编译红
  2. 恢复 `sunset_mcp.py status/errors/manage_script`
  3. 直接跑：
     - `Play Dialogue + Capture Spring UI Evidence`
     - `Open Workbench + Capture Spring UI Evidence`
     取回 fresh 图

## 2026-04-06 PackagePanel / 5_Settings 只读审计
- 用户目标：
  - 在 Sunset 仓库里只读盘点 `PackagePanel / 5_Settings` 当前 UI 结构，重点看 `Assets/222_Prefabs/UI/0_Main/PackagePanel.prefab` 与页签收集脚本，回答：
    1. `5_Settings` 适合挂什么新子层级
    2. 需要场景改动还是改 prefab 即可
    3. 现有滚动/布局组件能否复用
- 当前主线目标：
  - 先把 `PackagePanel` 页签收集规则和 `5_Settings` 实际层级看清，再决定后续若开工应改哪一层。
- 本轮子任务：
  - 只读审计 `PackagePanel.prefab`、`PackagePanelTabsUI.cs`、`GameInputManager.cs`、`Primary.unity`、`Town.unity`。
- 本轮做成：
  1. 确认页签收集逻辑：
     - `PackagePanelTabsUI.Collect()` 只收两类直系子物体：
       - `topParent` 下名字以 `Top_` 开头的 Toggle
       - `pagesParent` 下名字满足 `数字 + '_'` 的页面根
     - `OpenSettings()` 直接走索引 `5`。
  2. 确认 prefab 结构：
     - `PackagePanel` 根显式绑了 `panelRoot / topParent / pagesParent / boxUIRoot`。
     - `pagesParent` 直系页面是：`0_Props / 1_Recipes / 2_Ex / 3_Map / 4_Relationship_NPC / 5_Settings`。
     - `5_Settings` 当前只有一个直系子节点 `Main`，没有更深内容层。
  3. 确认 scene 关系：
     - `Primary.unity` 里的 `PackagePanel` 和 `Town.unity` 里的 `PackagePanel` 都是 scene-owned 对象，`m_PrefabInstance` 为 `0`，不是 `PackagePanel.prefab` 的 live prefab instance。
     - `Primary` 的 `GameInputManager.packageTabs` 直接序列化指向 scene 里的 `PackagePanelTabsUI`；`Town` 当前该引用还是 `0`。
  4. 确认滚动/布局现状：
     - 当前 prefab 里存在多处 `GridLayoutGroup`（`Top`、`0_Props/Up`、`0_Props/Down`、`1_Recipes/Left`、`Require_List`）。
     - 唯一现成 `ScrollRect` 在 `1_Recipes` 根，但 `m_Content` 与 `m_Viewport` 都是 `0`，不是一个可直接复用的已接好滚动容器。
     - `5_Settings` 自己没有现成 `ScrollRect / Mask / ContentSizeFitter / VerticalLayoutGroup`。
- 关键判断：
  1. 若是在现有 `5_Settings` 内加内容，新层级最适合挂在 `5_Settings/Main` 下面，再往下拆 `ContentRoot`，需要滚动时继续加 `Viewport -> Content`。
  2. 不适合把新内容直接挂成 `pagesParent` 下新的 `5_*` 直系 sibling；那会被 collector 当成新的 page index `5`，有覆盖/误收风险。
  3. 若只是想更新当前游戏里正在用的设置页，不能只改 `PackagePanel.prefab`；至少要改 `Primary.unity` 里的 scene copy。若还要让模板和旧场景一致，再额外同步 prefab，必要时连 `Town.unity` 一起补。
  4. 现有布局可以复用“模式”，不能直接复用“现成设置页容器”：
     - 可借鉴 `GridLayoutGroup` 的排布方式
     - 但当前没有一套已接好的 settings scroll 容器可直接拿来用
- 验证状态：
  - 静态推断成立；本轮未进 Unity live、未改文件、未跑 `Begin-Slice`。
- 涉及文件：
  - `Assets/222_Prefabs/UI/0_Main/PackagePanel.prefab`
  - `Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
  - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
  - `Assets/000_Scenes/Primary.unity`
  - `Assets/000_Scenes/Town.unity`
- 当前阶段：
  - 只读审计完成，可直接进入实施决策。
- 恢复点 / 下一步：
  - 如果用户要我继续做，先确认目标是：
    1. 只改 `Primary` 当前运行页
    2. `Primary + PackagePanel.prefab` 双同步
    3. 还要不要补 `Town`
  - 实施时优先从 `5_Settings/Main` 往下长，不碰 `pagesParent` 的页签根命名规则。

## 2026-04-06 DialogueUI｜剧情推进 submit 改为空格专用
- 当前主线目标：
  - 守 `DialogueUI / Workbench` 玩家面遗留与 fresh live 证据；用户临时插入一个明确阻塞：剧情推进按键必须改成空格
- 本轮子任务：
  - 只改 `DialogueUI` 的 submit 兼容链与对应静态测试，不扩到 Prompt / Workbench / NPC owner
- 本轮做成：
  1. `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
     - `WasSubmitAdvancePressedThisFrame()` 现在只认 `Space`
     - 移除了 `Return / KeypadEnter` 的 continue submit 入口
  2. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 补了“只认空格、不认回车”的静态护栏
- 验证结果：
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/DialogueUI.cs Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
    - 通过；仅提示 `DialogueUI.cs` 既有 CRLF/LF 归一化警告
  - CLI：
    - `validate_script DialogueUI.cs` => `assessment=unity_validation_pending`，owned/external error = 0，native validate = `warning`（1 条既有 `String concatenation in Update()`）
    - `validate_script SpringDay1DialogueProgressionTests.cs` => `assessment=unity_validation_pending`，owned/external error = 0，native validate = `clean`
    - `compile two files` => `assessment=blocked`，`subprocess_timeout:dotnet:20s`
  - direct MCP fallback：
    - `validate_script DialogueUI.cs` => `errors=0 warnings=1`
    - `validate_script SpringDay1DialogueProgressionTests.cs` => `errors=0 warnings=0`
    - `read_console count=30` => `0 log entries`
- 当前判断：
  - 这轮已经把“空格提示对了，但回车还偷偷能推进”的残口收掉
  - 这只是 `结构 / targeted probe` 过线，不是 live 体验终验
- 当前恢复点：
  1. 若继续主线，回到 `DialogueUI / Workbench` fresh live capture
  2. 若用户先验这个小点，重点复测“剧情 continue 只认空格，不再认回车”

## 2026-04-07 Workbench 左列只读核实｜不是“缺 Font Material 就对了”
- 用户目标：
  - 核实工作台左列内容一直不显示，是否真的是 `Image.material` 缺失导致；用户给了 Inspector 截图，要求快查根因
- 本轮只读检查：
  1. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - 左列行数据绑定里，代码只会给：
       - `row.icon.sprite`
       - `TMP` 文字的 `font / fontSharedMaterial`
     - 没有任何一段代码会把 `Font Material` 塞给 `RecipeRow/Accent/IconCard/TextColumn` 这些 `Image.material`
  2. `Assets/222_Prefabs/UI/Spring-day1/SpringDay1WorkbenchCraftingOverlay.prefab`
     - `RecipeRow_0/1/2` 及其 `Accent/IconCard/Icon/Name/Summary` 的序列化 `m_Material` 全是 `{fileID: 0}`
     - 说明 prefab 基线并不是“缺一个专门材质引用”
  3. `Assets/000_Scenes/Primary.unity`
     - 现有序列化里也没有把工作台左列 row 的 `Image.material` 预设成什么专用材质的证据
  4. 用户截图证据：
     - 把 `Font Material` 塞到 `Image.material` 后，Unity 直接报：
       - `Material Font Material doesn't have _Stencil property`
     - 这和代码里的 `recipeViewportRect + Mask` 正好对上，说明 `Font Material` 并不适合作为被 mask 的 UI Image 材质
     - 你第三张图里整行被 `Axe_0` 平铺，也进一步说明这是“错误材质/错误 source image 造成的偶然可见”，不是正式修法
- 当前判断：
  - 左列“不显示”的主因，不是“代码漏给了某个专用材质”
  - 更像是：
    1. row 的 `Image/source sprite` 被错误写入或被手动改坏
    2. prefab row / runtime generated row 的层级与可见性恢复链没有真正落到玩家面正确状态
    3. 手动把 `Font Material` 塞进 `Image` 只会制造 stencil 警告，不是正确解
- 后续正确排查方向：
  1. 查 runtime row 绑定后的 `Icon/Name/Summary/Accent` 当前 source image、rect、active、mask 关系
  2. 查为什么玩家面实际看到的是“左列空壳”，而 scene 里对象层级又还在
  3. 不再走“给 row Image 塞 Font Material”这条路

## 2026-04-07 DialogueUI + Workbench｜空格推进最终补口 + 左列 row 清污恢复
- 当前主线目标：
  - 用户要求这轮把两个玩家面真问题一起收完：
    1. 剧情推进/跳过按键彻底改成空格
    2. Workbench 左列空壳彻底修复
- 本轮子任务：
  - 只写：
    - `DialogueUI.cs`
    - `SpringDay1WorkbenchCraftingOverlay.cs`
    - `SpringDay1DialogueProgressionTests.cs`
    - 以及最小场景热修 `Primary.unity` 的 DialogueUI 旧序列化值
- 本轮做成：
  1. `DialogueUI.cs`
     - 新增 `NormalizeAdvanceInputSettings()`
     - `Awake()` / `OnValidate()` 中强制：
       - `advanceKey = KeyCode.Space`
       - `enablePointerClickAdvance = false`
  2. `Primary.unity`
     - 把旧场景残值从：
       - `advanceKey: 116`
       - `enablePointerClickAdvance: 1`
       改成：
       - `advanceKey: 32`
       - `enablePointerClickAdvance: 0`
  3. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 在 `EnsureRecipeRowCompatibility()` 里加入 row 图形链清污：
       - 背景 / accent / iconCard 清 `material + sprite`
       - icon 清 `material`，保留真正 item sprite
     - 新增 `NormalizeGeneratedRecipeRowGeometry(...)`
       - 强制把 `Accent / IconCard / TextColumn / Name / Summary` 拉回稳定 generated row 几何
     - `NeedsRecipeRowHardRecovery()` 新增：
       - `row.background.material != null`
       - `row.background.sprite != null`
       - `row.accent.material != null`
       - `row.accent.sprite != null`
       - `row.icon.material != null`
       作为坏壳判定
  4. `SpringDay1DialogueProgressionTests.cs`
     - 新增静态护栏覆盖上述输入链与 row 恢复链
- 验证结果：
  - CLI `validate_script`
    - `DialogueUI.cs` => no owned error, warning=1
    - `SpringDay1WorkbenchCraftingOverlay.cs` => no owned error, warning=1
    - `SpringDay1DialogueProgressionTests.cs` => clean
  - direct MCP
    - `DialogueUI.cs` => `errors=0 warnings=1`
    - `SpringDay1WorkbenchCraftingOverlay.cs` => `errors=0 warnings=1`
    - `SpringDay1DialogueProgressionTests.cs` => `errors=0 warnings=0`
    - console 仅 MCP 外部 warning：`[WebSocket] Unexpected receive error: WebSocket is not initialised`
  - `git diff --check`
    - `.cs` 文件通过
    - `Primary.unity` 暴露大量既有 trailing whitespace 旧账，不是这轮新引入的 code red
- 当前判断：
  - “还是 T 键”不是你错觉，真实根因就是 `Primary.unity` 还留着旧序列化值；这轮已经清掉
  - “左列材质缺失”也不是真根因，正确修法是：清掉错误材质/错误 sprite 污染，再把 generated row 几何拉回稳定壳
- 当前恢复点：
  1. 从 `Primary` 直接复测剧情推进，预期只认空格
  2. 打开 Workbench 复测左列，预期 row 文本和外壳恢复，不再继续空壳
## 2026-04-07｜Workbench 左列恢复链本轮补记

- 本轮子任务：修 `SpringDay1WorkbenchCraftingOverlay` 左列 recipe 行彻底空白，并同步止血你刚报的 `SpringDay1LateDayRuntimeTests.cs` 红。
- 已完成：
  1. `EnsureRecipeRowCompatibility()` 不再把 row 背景/强调条/iconCard 的 sprite 清空；改为只清错误材质。
  2. 新增 `HasInvalidRecipeRowGraphicMaterial()`，把 row 恢复条件从“无脑重建”收窄成“只在真污染时触发”。
  3. `ResolveItem()` 补 `AssetLocator.LoadItemDatabase()` 兜底，避免材料文案和右侧图标因服务数据库未就位而退化。
  4. `SpringDay1LateDayRuntimeTests.cs` 新增护栏测试，并把测试改成纯反射/资产读取，消掉了用户报的 `FarmGame` 红。
- 验证：
  - `validate_script Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs --skip-mcp` => owned red 0
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --skip-mcp` => owned red 0
  - `no-red` 总检查仍被 CodexCodeGuard timeout 卡住，属于工具 blocker，不是当前 owned compile red
- 关键判断：
  - 这轮不是继续瞎调布局，而是已经改到真正根因：误清图像壳 + 缺数据库兜底。
- 恢复点：继续围绕 Workbench live 结果收口；如果左列仍异常，优先看 runtime row 绑定/场景实例，而不是再回去泛改任务卡或对话框。
## 2026-04-07｜Workbench 左列材质链三次确认

- 最新刀口已从“Image/row 壳体”进一步收束到“TMP 文本 shared material 误覆盖”。
- 这轮在 `SpringDay1WorkbenchCraftingOverlay.cs` 里彻底去掉了 `fontSharedMaterial` 强写；当前 Workbench 文本链只改 font，不再手动覆盖 TMP 遮罩材质。
- 代码层 fresh validate：owned red 0；live 仍待用户实际打开 Workbench 再看左列文字是否回来了。
## 2026-04-07｜继续深收 Workbench 交互闭环

- 当前主线目标：
  - 继续只收 `Workbench` 玩家面交互闭环，不回漂去改任务清单、Town 其他 UI 或 NPC owner 范围。
- 本轮子任务：
  - 在 `SpringDay1WorkbenchCraftingOverlay.cs / CraftingStationInteractable.cs / SpringDay1Director.cs` 上继续推进多 recipe 队列、悬浮卡聚合和 E 提示一致性。
- 本轮已完成：
  1. `SpringDay1WorkbenchCraftingOverlay.cs`
     - `CraftRoutine()` 改成 `HasPendingCrafts() + FindNextPendingQueueEntry()` 切下一个待做项
     - `BuildRowSummary()` 改成直接显示 `排队 / 进度 / 制作完成`
     - `BuildCraftButtonLabel()` 去掉非当前 recipe 的“查看队列 / 队列中”杂讯
     - `UpdateQuantityUi()` 修掉“切到别的 recipe 时 hover 背景误变红”
     - 悬浮卡新增 `FloatingProgressDisplayState`，按 `resultItemId` 聚合；相同产物追加会并到同一卡
     - 悬浮卡视觉基线继续向“图标占大头 + 底部进度条 + 3x2 排列”靠拢
  2. `CraftingStationInteractable.cs`
     - `GetInteractionHint()` 在 overlay 打开时统一回 `关闭工作台`
  3. `SpringDay1Director.cs`
     - `BuildWorkbenchCraftProgressText()` 改成 `进度 ready/total · percent%`
- 验证：
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --skip-mcp` => `owned_errors=0`
  - `validate_script Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs --skip-mcp` => `owned_errors=0`
  - `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs --skip-mcp` => `owned_errors=0`
  - `git diff --check --` 上述脚本通过；overlay 仍有既存 `CRLF -> LF` warning
  - `CodexCodeGuard` 仍 timeout，所以整轮只能报到 `unity_validation_pending`，不是 own red
- 关键判断：
  1. 这轮不是新造一层假语义，而是把“多 recipe 排队 + 同物品悬浮聚合 + 交互提示一致性”真正写进运行时代码
  2. 下一步不该再回去泛改左列材质链，应该优先用 live 核悬浮卡和进度条矩阵
- 当前恢复点：
  - 若继续施工，优先看 `OnCraftButtonClicked / OnProgressBarClicked / UpdateFloatingProgressVisibility`
  - 若需要对外汇报，可报“代码层 own red 清，Workbench 队列/悬浮闭环又推进了一层，但 fresh live 仍待验证”
## 2026-04-07｜Workbench 进度条缺字/无动画只读复核

- 这轮只做代码与 prefab 只读审查，不动实现；目标是查清“为什么制作中/中断/完成都没条内文字和动画，只有追加正常”。
- 当前证据层级：
  - 用户 fresh 截图 = `真实入口体验` 证据
  - 源码 + prefab 只读审查 = `结构 / targeted probe` 证据
  - 结论可以落到“根因已定位”，但还不能 claim “体验已修好”。
- 已确认的真实根因有 3 个：
  1. `SpringDay1WorkbenchCraftingOverlay.cs` 里主进度条和悬浮进度条的 fill 都是 `Image.Type.Filled`，但创建时没有给 sprite：
     - 主条：`BuildUi()` 里 `progressFillImage = progressFill.gameObject.AddComponent<Image>()` 后直接设 `type = Filled`
     - 悬浮条：`BuildUi()` / `CreateFloatingProgressCard()` 里 `floatingProgressFillImage` / `fillImage` 同样没有 sprite
     - 这会导致 `fillAmount` 不能稳定表现为真正的 0~100 裁切，玩家面就会像一整块实心色条，而不是单件进度动画。
  2. 当前 `progressLabelText` 绑定错层：
     - prefab `SpringDay1WorkbenchCraftingOverlay.prefab` 里 `progressLabelText` 绑定到一个独立的 `ProgressLabel` 节点，它的父节点是 detail column，不是 `ProgressBackground`
     - 代码 `TryBindRuntimeShell()` 也只是全局找 `ProgressLabel`
     - `UpdateProgressLabel()` 更新到的其实是“条外那行文案”，不是进度条内部 overlay 文字
     - 所以会出现你截图里的：上方出现 `中断制作 / 进度 1/2` 一类字，但条内部没有字。
  3. 当前状态机虽然在 `UpdateProgressLabel()` 里区分了“制作中 / 可领取 / 中断 / 排队”，但都落在错误的 label 节点上；因此 hover 看起来部分正确，常驻态却不成立。
- 额外确认：
  - `追加制作` 正常，是因为它走的是 `craftButtonLabel` 这条独立链，不依赖进度条内文字节点。
  - 左侧滚动条此前会跳顶，真根因是 `RefreshRecipeContentGeometry()` 里每次刷新都写死 `verticalNormalizedPosition = 1f`；这条已经定位，不属于本轮进度条主问题。
- 当前恢复点：
  1. 真正该修的不是再调颜色，而是先把“条内文字节点”接回 `progressRoot / floating card progress background`，不要继续复用 prefab 里那个独立 `ProgressLabel`。
  2. fill 动画需要改成“有 sprite 的 Filled Image”或“宽度/遮罩驱动”，否则 `_craftProgress` 再怎么变也不会像真正单件进度动画。
  3. 修复顺序应是：
     - 先修 fill 实现
     - 再修 label 绑定/重挂
     - 再把 `UpdateProgressLabel()` 和悬浮条统一到同一套显式状态机
## 2026-04-07｜Workbench 进度条闭环开始真实修复

- 当前主线目标不变：继续只收 Workbench 进度条闭环，不回漂去改别的 UI。
- 本轮已落的实现：
  1. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 新增 `_runtimeProgressFillSprite`，并通过 `EnsureProgressFillGraphic(...)` 给主进度条、悬浮进度条、动态浮动卡进度条统一补上真正可裁切的 sprite + Filled Horizontal 配置
     - 新增 `EnsureProgressLabelBinding()`，强制把 `progressLabelText` 重新绑定到 `progressRoot` 内部的条内文字节点；如果 prefab 里旧的条外 `ProgressLabel` 仍存在，就在运行时隐藏它
     - `TryBindRuntimeShell()` 现在会在拿到 `progressRoot/progressFillImage` 后立刻跑 `EnsureProgressFillGraphic()` 和 `EnsureProgressLabelBinding()`，避免复用旧 prefab 壳时继续绑错节点
     - `EnsurePrefabDetailTextChain()` 也会先纠正 `progressLabelText` 的条内绑定，再做字体/可读性补口
     - `EnsureFloatingProgressCompatibility()` 和 `CreateFloatingProgressCard()` 也统一改成走 `EnsureProgressFillGraphic()`，避免悬浮条继续是“有 fillAmount 但没裁切”的假进度
     - `UpdateProgressLabel()` 与 `UpdateFloatingProgressVisibility()` 每次刷新前都再次补一次 fill/label 守卫，防止旧壳回弹
- 本轮验证：
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --skip-mcp` => `owned_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs` 无新的文本错误；仅既存 `CRLF -> LF` warning
  - `CodexCodeGuard` 仍 timeout，整体 assessment 仍是 `unity_validation_pending`
- 当前判断：
  1. 这轮已经把“为什么制作中没动画、条内没字”的两条主根因真正改进了代码里，不再停留在只读分析
  2. 下一步要看的不再是“有没有条内节点”，而是 live 下 `UpdateProgressLabel()` 的状态文案和颜色切换是否完全贴合用户语义
- 当前恢复点：
  - 若继续下一刀，应优先 live 核：制作中绿条动画、100% 后黄条常驻、hover 中断/领取切换、主条与悬浮条是否一致

## 2026-04-07｜只读核查：打包后中文/TMP 异常的当前最可能根因已收敛

- 当前主线目标：
  - 继续服务 `UI / Day1` 玩家面缺字链；本轮子任务是只读查清“为什么 editor 偶尔像没事、但打包后中文/TMP 还是异常”。
- 本轮边界：
  - 不改文件
  - 不进 Unity live 写
  - 不跑 `Begin-Slice`
- 本轮已完成：
  1. 只读核查了 `DialogueChinese*` 五份字体资产：
     - 全部是 `AtlasPopulationMode.Dynamic`
     - 磁盘上 `m_GlyphTable / m_CharacterTable` 为空
     - `m_ClearDynamicDataOnBuild = 1`
     - `m_FallbackFontAssetTable = []`
  2. 只读核查了 `TMP Settings.asset`：
     - 默认字体仍是 `LiberationSans SDF`
     - 全局 fallback 为空
  3. 只读核查了 `DialogueFontLibrary_Default.asset`：
     - 所有 key 都指向 `DialogueChinese Pixel SDF`
  4. 只读核查了 `DialogueChineseFontRuntimeBootstrap.cs`、`DialogueUI.cs`、`SpringDay1PromptOverlay.cs`、`SpringDay1WorkbenchCraftingOverlay.cs`：
     - 当前 runtime bootstrap 只预热固定 seed
     - UI resolver 主要靠 `HasCharacters(...)` 判定，不会先按真实 `actualText` 生字
     - 一旦判 unusable，仍可能掉回 `TMP_Settings.defaultFontAsset`
  5. 只读核查了 `SpringDay1PromptOverlay.prefab`、`SpringDay1UiPrefabRegistry.asset`、`Town/Primary` 场景：
     - prefab/registry/source font GUID 当前都没看到 Addressables 断链
     - 但 `Town/Primary` 里仍残留一批 `m_fontAsset: 0` 或 `LiberationSans` 的旧壳文本节点
- 当前最核心判断：
  - 这轮我认为最可能的主根因不是“资源没打进去”，而是：
    1. build 前把动态中文字体清成空壳
    2. runtime 只预热了固定 seed
    3. 真正文案超出 seed 后，字体 resolver 又过早回退到没有中文 fallback 的 `LiberationSans`
- 为什么我认为这个判断成立：
  1. 资产面已经直接坐实“动态 + build 清空 + 无 fallback”
  2. 代码面已经直接坐实“多数 resolver 只判 `HasCharacters`，不先对真实文本 `TryAddCharacters`”
  3. 抽样 `SpringDay1` 对话文案后，当前 seed 之外的缺口字符仍然很多，不是 1-2 个边角字
- 当前最薄弱点：
  - 我这轮没有跑真实 player build，也没有 live 截屏复核 build 包；所以这是高可信静态根因结论，不是“我已在打包产物里亲眼验过”的终验结论。
- 自评：
  - 这轮我给自己 `8/10`。
  - 站稳的部分是资产配置、代码路径和 build 风险链已经闭环。
  - 不够满分的地方是还缺一次真正的 build/release 侧实机验证。
- 如果下一步继续，我最确信的顺序：
  1. 先统一 `DialogueUI / PromptOverlay / Workbench / Hint` 的字体 resolver，让它们只返回 runtime-safe 中文字体，并在实际文本写入时 `TryAddCharacters(actualText)`
  2. 再决定是否把 build-critical 中文字体改成静态/半静态预烘焙资产
  3. 最后才做 fresh build 验证
- 当前恢复点：
  - 若用户下一刀要我真实修，就直接从“runtime clone + actualText 生字 + 禁止早退到 LiberationSans”开始，不要先回去泛查 Addressables 或 prefab 绑定。

## 2026-04-07 Workbench 主条互斥 + 完成悬浮保留
- 当前主线：继续只收 `Workbench` 主进度条/领取/悬浮卡闭环，不回漂其他 UI。
- 本轮真实施工前置：
  - 读了 `preference-preflight-gate`、`sunset-no-red-handoff`、`global-preference-profile.md`
  - 证据层是用户最新提供的真实玩家面截图，属于“真实体验层”
  - `Begin-Slice` 没重开成功，原因是 `UI` 线程本来就处于 `ACTIVE`；本轮直接沿现有切片 `workbench-progress-closure` 继续
- 本轮实际改动：
  1. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - 新增主条状态色常量，统一绿色制作态 / 深黄色完成态 / 红色取消态。
     - 新增 `EntryHasReadyOutputs / GetPendingCraftCount / ShouldShowCraftButton / ApplyProgressLabelState`，把主条 label 和按钮 label 职责拆开。
     - `UpdateQuantityUi()` 改成：
       - `CraftButton` 只在数量 `> 0` 且没有 ready 成品时显示
       - `craftButtonLabel` 与 `progressLabel` 不再同层叠字
       - 完成态背景只在真正完成时常驻，不再在“还有 pending”时误亮黄色
     - `UpdateProgressLabel()` 改成真正互斥状态：
       - 制作中 = 绿色进度
       - 已有 ready 且 hover = 黄色领取
       - hover 取消/中断 = 红色提示
       - 按钮态 = 主条文字留空
       - 空闲态 = 单独显示 `进度  0/0`
     - `CanCraftSelected()` / `GetSelectableQuantityCap()` 新增 ready 产物阻断，未领取前不能继续该 recipe。
     - `CraftRoutine()` 收尾改成：隐藏状态下只要还有 floating state，就不再 reset session；这样全部完成后悬浮卡会继续保留，等待玩家重新打开工作台领取。
     - `EnsureProgressLabelForeground()` 给主条与悬浮卡 label 补深色 shadow，提高白字可读性。
- 本轮验证：
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --owner-thread UI --output-limit 12 --skip-mcp`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 5`
    - 工具自身 `AttributeError: 'str' object has no attribute 'get'`
    - 这轮不把它当成当前脚本 own red
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - 通过
    - 仅有 `CRLF -> LF` warning
- 当前最核心判断：
  - 这轮最关键的两个真根因已经被落地处理：
    1. 底部主条是“双文本链抢同一区域”导致重叠
    2. 完成后成品消失是“CraftRoutine 收尾误清 session”导致队列被抹掉
- 当前仍待 live 复核：
  1. 黄色完成态的肉眼可读性是否已经过线
  2. 全部完成后悬浮卡是否稳定保留到手动领取为止
  3. 同 recipe ready 未领取时是否已完全阻断继续制作
- 当前恢复点：
  - 如果继续这条线，直接从 `UpdateProgressLabel / UpdateQuantityUi / CraftRoutine / UpdateFloatingProgressVisibility` 四处继续，不要再回去泛查左列可见性或任务卡。

## 2026-04-07 Workbench 底部交互矩阵补刀
- 本轮继续只收 Workbench 底部条交互矩阵。
- 核心补口：不再只靠“某个 label 置空”，而是把底部条硬切成 `动作层` 与 `进度层` 两个互斥层；动作层出现时，进度层整个 `progressRoot` 都会被隐藏。
- 目的：彻底防止 `加入队列 / 追加 / 开始制作` 和 `进度 / 制作完成 / 领取 / 取消` 同时占同一条底栏。
- 代码层验证：`validate_script --skip-mcp` 仍是 `owned_errors=0`，Unity live 仍待用户复测。

## 2026-04-08 Workbench 追加优先级覆盖领取态
- 这轮继续只收 Workbench 底部状态机。
- 最新语义已按用户 live 反馈调整：`selectedQuantity > 0` 时动作层绝对优先，不再允许 `readyCount` 抢回黄色领取态。
- 已删掉 `先领取当前已经完成的成品` 这条 blocker；领取现在只允许作为当前选中 recipe 进度栏状态出现。
- 代码层验证：`validate_script --skip-mcp` 仍是 `owned_errors=0`。

## 2026-04-08 背包地图页 / 好感度页真实施工
- 当前主线目标：
  - 已从 Workbench 线切到新 UI 任务：把背包里的 `3_Map` 与 `4_Relationship_NPC` 两个空页做成正式可见、可消费的玩家面。
- 本轮子任务：
  - 直接根据项目真实数据源重建这两页，不回到 Workbench，不做临时占位图。
- 为什么这轮这么做：
  - 用户明确要求“自己参考项目实际情况与 NPC 情况去创作”，并强调不能再出现里面内容撑爆外壳的大空块/测试味 UI。
- 已完成：
  1. 新增 `PackagePanelRuntimeUiKit.cs`
     - 抽出运行时背包页 UI 创建工具，统一用 TMP 中文字体链、Image/Rect/Button 构建和 children 清理。
  2. 新增 `PackageMapOverviewPanel.cs`
     - 在 `3_Map/Main` 上运行时生成地图页。
     - 读取 `StoryManager.CurrentPhase` 决定当前高亮地标和右侧阶段说明。
     - 读取 `SpringDay1NpcCrowdManifest` 的 beat 语义，给地图页右侧补“当前会撞上的人群压强摘要”，并且绕开旧 helper 的 beat 回退问题。
  3. 新增 `PackageNpcRelationshipPanel.cs`
     - 在 `4_Relationship_NPC/Main` 上运行时生成关系册。
     - 数据源来自 `SpringDay1NpcCrowdManifest.asset` + `PlayerNpcRelationshipService.GetSnapshot()`。
     - 左侧列表 + 右侧详情结构已落地，详情里有阶段条、身份观察、在场感、常驻方式。
     - 头像从 NPC prefab 的 `SpriteRenderer` 提取，不再做空头像盒子。
  4. 修改 `PackagePanelTabsUI.cs`
     - 在 `Awake / SetRoots / EnsureReady` 自动安装上面两页。
  5. 补齐三个新脚本 `.meta`。
- 当前验证：
  - `git diff --check` 已过。
  - `validate_script --skip-mcp` 对新脚本与 `PackagePanelTabsUI.cs` 没吐出 owned/external error。
  - 真正的 fresh compile receipt 仍被 `CodexCodeGuard` 超时卡住，状态只能诚实写成 `unity_validation_pending / blocked-by-timeout`。
- 当前阻塞：
  - `CLI CodexCodeGuard timeout prevents fresh compile receipt`
- 当前 live 状态：
  - 已执行 `Park-Slice`
  - `UI` 线程当前状态为 `PARKED`
- 当前恢复点：
  - 下一轮如果继续，直接从“真实打开 PackagePanel 看地图页 / 关系页观感与滚动”继续，不要重新回到入口勘察。

## 2026-04-08 插入式 blocker：skip 到工作台后的失活 NPC 气泡协程报错
- 当前主线目标：
  - 主线仍是背包 `地图 / 好感度` 页收口；这轮是插入式阻塞处理，不是换线。
- 本轮子任务：
  - 止血用户贴出的 `Toggle Skip To Workbench` 后 `Coroutine couldn't be started because the game object '001' is inactive!`。
- 已完成：
  1. 只读定位确认：
     - 根因不在 Workbench 制作逻辑本体；
     - 是 `SpringDay1Director.TryShowEscortWaitBubble(...)` 仍在给失活的 `001` 发等待气泡；
     - `NPCBubblePresenter.ShowTextInternal(...)` 对 inactive host 还会继续 `StartCoroutine(...)`。
  2. 代码修复：
     - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
       - 在 `UpdateTypedConversationText(...)`、`ShowTextInternal(...)`、`HideBubble()`、`StartVisibilityAnimation(...)` 增加 `CanRunBubbleRuntime()` 防护；
       - inactive host 下直接不再播气泡/不再起协程。
     - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
       - `TryShowEscortWaitBubble(...)` 增加 presenter 失活早退。
  3. 最小验证：
     - 两个脚本 `validate_script --skip-mcp` 都是 `owned_errors=0 / external_errors=0`；
     - `git diff --check` 通过；
     - Unity Edit Mode fresh compile 后控制台只剩两个旧 `TMP_Text.enableWordWrapping` warning，没有新的 error。
- 当前最核心判断：
  - 这条报错的最稳修法确实应该落在 presenter 自保；导演层只做额外减压护栏。
- 当前最薄弱点：
  - live 复跑没有补齐，因为刚刚为验证而停 PlayMode 会打断用户现场；这轮先停在“代码层止血 + Edit Mode 编译干净”的安全点。
- 当前恢复点：
  - 修完这个 blocker 后，主线恢复到背包页 live 观感验证；
  - 如果用户要继续追这条 skip 坏链，再从不打断现场的最小 live 复现往下走。

## 2026-04-08 Workbench 中断只取消当前 recipe，并续跑其他队列
- 当前主线目标：
  - 原主线仍是 UI/Workbench 收口；这轮子任务是修“点击某个 recipe 的中断后，把其他排队制作也一起停掉甚至清空”的逻辑 bug。
- 本轮已完成：
  1. 确认根因：
     - Workbench 多 recipe 队列当前只有一条全局 `_craftRoutine`；
     - `CancelActiveCraftQueue()` 以前会直接 `StopCoroutine(_craftRoutine)`，但不会切到 `FindNextPendingQueueEntry()` 续跑；
     - 同时 `HasWorkbenchFloatingState` 只认 active/ready，不认 pending-only，导致隐藏或关闭后 pending entry 还有被 `CleanupTransientState()` 清空的风险。
  2. 已修改 `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`：
     - `HasWorkbenchFloatingState` 现在把 pending-only 队列也视为有效工作台状态。
     - 新增 `HasTrackedQueueEntries`，防止 pending queue 被误判成“什么都没有”。
     - 新增 `ResumeNextPendingCraftQueueIfAny()`，当前 recipe 被中断后会自动切到下一条 pending queue 继续制作。
     - `CancelActiveCraftQueue()` 现在只取消当前 active recipe：
       - 只返还当前正在制作这一件已扣掉的材料；
       - 当前 recipe 的剩余 pending 会被截断到 `readyCount`；
       - 其他 recipe 队列不会被一起停掉；
       - 返还后会立刻刷新背包/UI，并在有下一条 pending 时自动续跑。
  3. 代码层验证：
     - `validate_script --skip-mcp Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
       - `owned_errors=0`
       - `assessment=unity_validation_pending`（仍是 `CodexCodeGuard` timeout 降级）
     - `git diff --check` 通过（仅 CRLF/LF 提示）
     - Unity Edit Mode fresh compile 后控制台无新的 compile error；仍有外部既有日志：
       - `The referenced script (Unknown) on this Behaviour is missing!`
- 当前关键判断：
  - 这条 bug 的主根因已经实修，不再只是分析：
    - 中断当前 recipe 不会再把其他 recipe 的执行器一起掐掉。
- 当前恢复点：
  - 如果下一轮继续，优先让用户实测：
    1. 多 recipe 排队时中断当前项，其他项是否继续制作；
    2. 当前单件中断后材料是否立即返还、材料区和背包是否立刻刷新；
    3. 关闭面板后 pending/完成悬浮是否还在。

## 2026-04-08 Workbench 正确交互矩阵重建（先审口径，不落代码）
- 用户最新裁定：
  - 当前逻辑依旧错乱，根因不是某一个按钮没接上，而是“每个制作物品没有真正做成独立数据和独立交互”，导致斧头在制作时，箱子那一行也能继承领取/中断语义。
- 当前只读结论：
  1. 你真正要的不是“工作台一个全局按钮状态”，而是：
     - `每个 recipe 一条独立状态机`
     - `工作台只有一个全局调度器`
  2. 全局调度器只负责：
     - 当前哪一条在做
     - 当前单件进度是多少
     - 下一条 pending 是谁
  3. 每个 recipe 自己必须独立持有：
     - `totalCount`
     - `readyCount`
     - `isActive`
     - `currentUnitProgress`
     - `reservedCurrentUnitMaterials`
     - `selectedQuantity`
     - `hoverIntent`
  4. 任何一行的底部条、领取、中断、取消排队，都只能读自己这条 entry 的状态，不能读“工作台全局正在做某件物品”后再投射到当前选中行。
- 正确矩阵（当前准备按此重构）：
  1. `空闲行`
     - 自己没有 total、没有 ready、不是 active
     - 只允许 `开始制作 xN`
     - 不允许显示 `领取产物 / 中断制作 / 取消排队`
  2. `排队未开始行`
     - 自己 `total > 0 && ready == 0 && !active`
     - 默认只显示自己的排队信息
     - hover/click 只能取消自己这一行的排队
     - 绝不能继承 active 行的中断语义
  3. `当前制作行（无产出）`
     - 自己 `active && ready == 0`
     - 默认显示绿色单件进度动画
     - hover 才切红色 `中断当前`
     - click 只中断自己这一行
  4. `当前制作行（已有产出但仍在继续）`
     - 自己 `active && ready > 0 && total > ready`
     - 默认仍显示绿色单件进度动画
     - hover 切黄色 `领取当前已完成产物`
     - click 只领取自己这一行已经完成的部分
     - 领取完后如果还在继续做，移出鼠标恢复绿色进度
  5. `已完成未领取行`
     - 自己 `ready > 0 && total == ready && !active`
     - 默认黄色 `制作完成 n个`
     - click 只领取自己这一行
  6. `选数量中的动作优先`
     - 只要当前这一行 `selectedQuantity > 0`
     - 这一行必须强制进入动作层
     - active 行显示 `追加制作 xN`
     - 非 active 行显示 `加入队列 xN` 或 `开始制作 xN`
     - 此时不能再被领取/中断状态抢走
- 悬浮框真值：
  - 每个产物/recipe 的悬浮卡也必须是独立态，不是一个全局共享动作条。
  - 当前 active 的卡才允许显示绿色单件进度动画。
  - 其他卡只能显示自己的 `进度 ready/total` 或 `制作完成 n个`，不能继承别的卡的领取/中断语义。
- 当前最核心判断：
  - 这已经不是“小修按钮文案”问题，而是 `row-state / footer-state / floating-state / scheduler-state` 四层没有彻底隔离。
- 当前恢复点：
  - 如果继续施工，下一刀应直接按这份矩阵重做：
    1. 行状态源
    2. 底部条状态机
    3. 悬浮卡状态机
    4. 调度器与 entry 的边界

## 2026-04-08｜Workbench 取消/中断逻辑只读根因分析
- 当前主线目标：
  - 本轮按用户要求插入只读分析 `SpringDay1WorkbenchCraftingOverlay.cs` 的 Workbench 取消/中断链；不进入真实施工，不改业务代码。
- 本轮子任务：
  - 解释为什么点击某个配方的“中断制作”后，其他排队中的制作也会一起停掉/后续被清空。
- 已完成：
  1. 静态复核 `OnCraftButtonClicked / OnProgressBarClicked / CraftRoutine / CancelActiveCraftQueue / CancelQueuedRecipeEntry / StopCraftRoutine / RemoveQueueEntryIfEmpty / TryPickupSelectedOutputs / FindNextPendingQueueEntry`。
  2. 交叉确认 `CraftingService.TryCraft(recipe, false)` 的真实扣料时机：
     - 每开做一件就立刻扣一件材料；
     - 完工后只是把 `readyCount +1`，不会再次扣料；
     - 取消时只有当前已 reserve 的单件会经 `RefundReservedCraftMaterials(...)` 返还。
  3. 根因结论已压实：
     - Workbench 全队列实际由单个 `_craftRoutine + _activeQueueEntry` 驱动；
     - `CancelActiveCraftQueue()` 会直接 `StopCoroutine(_craftRoutine)` 并清空全局 active 状态，但不会像 `CraftRoutine()` 那样再走 `FindNextPendingQueueEntry()` 接续下一条待制作 entry；
     - 因此一旦中断当前 active recipe，其他 recipe 的 pending entry 虽未当场逐个删除，也会一起失去驱动；
     - 若随后 overlay `Hide/OnDisable`，`CleanupTransientState(resetSession: true)` 还会因为 `HasWorkbenchFloatingState` 不把“仅 pending、无 active/ready”算存活态，而把 `_queueEntries.Clear()`，形成用户感知里的“全队列被清空”。
- 关键方法与定位：
  - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - `OnCraftButtonClicked()`：3692-3750
    - `OnProgressBarClicked()`：3752-3782
    - `CraftRoutine()`：3784-3882
    - `StopCraftRoutine()`：3884-3918
    - `RemoveQueueEntryIfEmpty()`：4155-4167
    - `TryPickupSelectedOutputs()`：4233-4268
    - `CancelActiveCraftQueue()`：4270-4306
    - `CancelQueuedRecipeEntry()`：4308-4327
    - `RefundReservedCraftMaterials()`：4329-4351
    - `FindNextPendingQueueEntry()`：4197-4208
    - `CleanupTransientState()`：400-445
    - `HasWorkbenchFloatingState`：161-163
  - `Assets/YYY_Scripts/Service/Crafting/CraftingService.cs`
    - `TryCraft(recipe, bool deliverToInventory)`：406-475
- 当前最核心判断：
  - 真正直接把“当前 recipe 的中断”放大成“整个队列一起停”的，不是 refund 本身，而是 `CancelActiveCraftQueue()` 把单全局协程直接停掉，却没有切到 `FindNextPendingQueueEntry()` 续跑。
- 当前最薄弱点：
  - 仅凭静态代码能 100% 解释“全队列一起停”和“关闭后被清空”两层现象；如果用户描述的是“面板不关闭时其他 entry 也立刻从列表里消失”，那还需要 live 复现确认是否另有 UI 层显示问题。
- 当前恢复点：
  - 如果用户下一步要修，最小安全方向是：`CancelActiveCraftQueue()` 中断当前 active entry 后，立即选取 `FindNextPendingQueueEntry()` 并重启/续接 `_craftRoutine`；同时审一遍 `Hide/CleanupTransientState`，避免 pending-only 队列被当成可丢弃瞬态。

## 2026-04-08 Workbench entry-state 隔离重构第一刀
- 当前主线目标：
  - 继续收 Workbench 交互，不再做点状补丁；这轮要把“全局 active 状态串到当前选中行”的问题切开。
- 本轮已完成：
  1. 在 `SpringDay1WorkbenchCraftingOverlay.cs` 增加 entry 维度 helper：
     - `IsActiveEntry(...)`
     - `CanPickupEntryOutputs(...)`
     - `CanInterruptActiveEntry(...)`
     - `CanCancelQueuedEntry(...)`
     - `GetEntryUnitProgress(...)`
     - `ResetEntryRuntimeState(...)`
     - `SetActiveEntryProgress(...)`
     - `SetActiveEntryReserved(...)`
  2. `WorkbenchQueueEntry` 现在开始持有运行态字段：
     - `currentUnitProgress`
     - `hasReservedCurrentCraft`
     也就是“当前单件进度”和“当前单件是否已扣料”开始回到 entry 自身。
  3. `CraftRoutine / StopCraftRoutine / CancelActiveCraftQueue / ResumeNextPendingCraftQueueIfAny` 已同步改成驱动 entry 运行态：
     - 当前单件进度不再只挂在全局 `_craftProgress`
     - 当前单件已扣材料状态不再只挂在全局 `_hasReservedActiveCraft`
  4. 底部条与点击矩阵已按 entry 隔离：
     - `OnProgressBarClicked()` 只按当前选中 entry 自己的 `pickup / interrupt / cancel-queue` 状态执行
     - `UpdateProgressLabel()` 不再把 active 行的领取/中断语义投到别的行
     - `UpdateQuantityUi()` 的 hover / 背景色判断也改成 entry 维度
     - `BuildCraftButtonLabel()` 与 `GetMaterialPreviewQuantity()` 也改成优先看 selected entry 是否 active
  5. 选中 recipe 时会清空 hover 残留：
     - `SelectRecipe()` 现在会把 `_craftButtonHovered / _progressBarHovered` 清零，避免前一行的 hover 状态残留到当前行。
  6. 悬浮卡状态也开始走 entry 进度：
     - `FloatingProgressDisplayState` 新增 `ActiveUnitProgress`
     - active 卡显示不再直接读全局 `_craftProgress`
  7. 完成态缓存降级：
     - `ShouldShowCompletedProgress(...)` 不再用 `_lastCompletedRecipeId/_lastCompletedQueueTotal` 给无 entry 的行刷完成态，避免旧完成缓存串味。
- 代码层验证：
  - `validate_script --skip-mcp Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - `owned_errors=0`
    - `assessment=unity_validation_pending`（工具仍是 `dotnet:20s` timeout 降级）
  - Unity Edit Mode fresh compile 后控制台 `0 error`
  - 当前 only warnings：
    - `PackagePanelRuntimeUiKit.cs(105,9)` obsolete warning
    - `SpringDay1WorkbenchCraftingOverlay.cs(2238,13)` obsolete warning
- 当前最核心判断：
  - 这轮已经不是“多 recipe 中断不连坐”那么窄了，而是把 footer / 行摘要 / 悬浮卡 三层的状态源开始切回 recipe entry 自身。
- 当前最薄弱点：
  - 还没做 live 实机复现，所以这轮能确认“代码矩阵已按 entry 隔离重写一刀”，但玩家体感是否完全过线还得看你下一轮真实操作。
- 当前恢复点：
  - 下一轮优先看 3 个玩家面 case：
    1. 斧头 active 时，箱子/短剑是否还会出现不属于自己的领取/中断
    2. 当前 active 行 hover/click 是否只影响自己
    3. 悬浮卡是否只让 active 卡跑绿色单件进度，其他卡只保留自己的进度/完成态

## 2026-04-08｜Workbench 剩余全局态投射点只读盘点
- 当前主线目标：
  - 继续只收 `Workbench` recipe 行 / 底部条 / 悬浮卡的状态隔离闭环。
- 本轮子任务：
  - 用户要求只读审查 `SpringDay1WorkbenchCraftingOverlay.cs`，列出这份脚本里仍可能让不同 recipe 之间状态串味的剩余“全局态投射”点，并给出最小修法建议；本轮不改代码。
- 已完成事项：
  1. 逐段回看 `Open / UpdateQuantityUi / UpdateProgressLabel / BuildFloatingProgressStates / BuildCraftButtonLabel / GetMaterialPreviewQuantity / PushDirectorCraftProgress / GetRemainingCraftCount / GetQueuedCraftCountAfterCurrent / TryPickupSelectedOutputs / CancelActiveCraftQueue / CancelQueuedRecipeEntry / CraftRoutine / StopCraftRoutine`。
  2. 把剩余高置信点压成 6 类：
     - `PushDirectorCraftProgress()`：仍把“当前选中 recipe”与“全局 active counts/progress”混发；
     - `BuildFloatingProgressStates()`：仍按 `resultItemId` 聚合，多个 recipe 可能共用一张卡；
     - `Open()`：仍会被全局 active / first-ready 逻辑强制改写当前选中项；
     - `UpdateQuantityUi() + BuildCraftButtonLabel()`：仍把 station-global busy 语义直接投到当前 footer；
     - `GetMaterialPreviewQuantity()`：仍通过全局 active queue 余量链决定材料预览含义；
     - `_lastCompletedQueueTotal / _lastCompletedRecipeId`：仍保留单槽完成残影，且 `CraftRoutine()` 收尾会把多 recipe 成功数压到最后一个 recipe 上。
  3. 同时确认：
     - `row summary` 自身现在主要走 `entry`，不是本轮首要主嫌；
     - 最大剩余风险面已经收缩到 `director push / floating card / open focus / footer CTA / material preview` 这 5 条外放链。
- 当前判断：
  - 这轮最值得先修的不是再去碰行摘要，而是先砍掉“把全局 active 状态继续往外投”的几条残留出口。
- 当前最薄弱点：
  - `BuildCraftButtonLabel()` 这条到底算 bug 还是设计，仍取决于你们是否接受“工作台忙 = 所有 recipe 的动作按钮都进入 queue 语义”。
- 当前恢复点：
  - 真要继续下刀，建议顺序：`PushDirectorCraftProgress -> BuildFloatingProgressStates -> Open / MaterialPreview -> footer CTA`；`_lastCompleted...` 放尾刀。

## 2026-04-08｜Workbench 右侧进度串味已切掉并拿到 live 证据
- 当前主线目标：
  - 继续收 UI 线程 own 的 `Workbench` 状态隔离，不再让 active recipe 的进度污染当前选中的其他 recipe。
- 本轮做成：
  1. 根因修复：
     - `CraftRoutine()` 的逐帧刷新改成 `UpdateProgressLabel(GetSelectedRecipe())`，不再用 active recipe 强刷右侧详情。
     - `PushDirectorCraftProgress()` 只从真实 active entry 取进度，切别的 recipe 行时不会再把导演层工作台进度串坏。
  2. 连带修补：
     - `Open()` 保留当前有效选中项，避免每次打开都被全局 active/ready 状态强跳。
     - 悬浮卡改成按 recipe/entry 自己构建，不再按 `resultItemId` 聚合。
     - 同配方完成品未领取时，先禁止继续给该 recipe 排新单。
     - `RefreshSelection()` 会同步刷新左列行摘要，避免悬浮卡/左列/右侧三处数字打架。
  3. 自建 live probe：
     - 扩了 `SpringUiEvidenceMenu` 的 workbench probe，在开始制作后自动切到另一条 recipe，并记录当前 `selected/progress`。
- 验证：
  - CLI `validate_script --skip-mcp`：
    - `SpringDay1WorkbenchCraftingOverlay.cs` owned_errors=0
    - `SpringUiEvidenceMenu.cs` owned_errors=0
    - `SpringDay1DialogueProgressionTests.cs` owned_errors=0
  - Unity fresh compile：清 Console 后 `0 error`
  - live log：`WorkbenchSelectionIsolation => switched=True, selected='锄头', progress='选择配方后即可开始制作'`
- 当前判断：
  - 这轮最关键的“右侧进度跟错 recipe”已经有 live 证据说明被切掉了。
- 当前薄弱点：
  - `SpringDay1DialogueProgressionTests.WorkbenchInteraction_ContainsRuntimeBindingBridge` 里还有老口径源码断言会失败，需要后续专项回正；它不是 compile red，但会污染 targeted source test。
- 当前恢复点：
  - 下一轮如果继续 workbench，优先盯底部 CTA/材料预览是否还残留 station-global queue 语义，其次再清 `_lastCompleted...` 单槽残影。

## 2026-04-08：只读体验复盘，当前 Workbench 主逻辑近线，但默认制作条与悬浮卡表达仍不够最终版

- 当前主线目标：
  - 继续服务 `spring-day1` 的 Workbench 玩家面收口；本轮子任务是只读判断：
    1. 默认 idle 制作条显示 `进度 0/0` 是否合理；
    2. 悬浮制作卡当前还差哪些体验层收口；
    3. 是否还有值得后续统一的轻逻辑尾账。
- 本轮完成：
  1. 复读 `SpringDay1WorkbenchCraftingOverlay.cs` 的：
     - `UpdateProgressLabel()`
     - `ShouldShowCraftButton()/ShouldShowProgressFooter()`
     - `UpdateFloatingProgressVisibility()`
     - `BuildFloatingProgressStates()`
     - `RepositionFloatingProgressCards()`
  2. 结合用户最新 live 反馈，把当前问题重新压回“状态表达层”，而不是再误判成 recipe 串味大 bug。
- 关键结论：
  1. 默认 idle 态 `进度 0/0` 不自然。
     - 当前代码明确把“无制作任务”写成了一个假进度标签。
  2. 底部区现在仍是两套控件互斥切换，不是严格的一根多状态制作条。
     - 只要 `_selectedQuantity > 0`，就隐藏 `progressFooter`，切到 `craftButton`。
  3. 悬浮卡当前最大问题已不是逻辑，而是视觉秩序：
     - `4/5` 张卡时第二排不会按本排卡数居中；
     - 卡位会随 `active/ready/recipeId` 动态排序而跳位；
     - 当前 `72x78` 小卡比例可用，但仍偏测试态。
  4. “排队未开始”当前仍有被误写成“进度 0/总数”的语义毛边。
  5. “未领取完成品阻断”当前只在 `ready && no pending` 时完全成立；`ready + pending` 还没被彻底统一。
- 验证：
  - 本轮只做源码阅读与用户最新实测反馈映射。
  - 未跑 fresh live capture。
  - 因此这轮判断只站住 `targeted probe / partial validation`，不是 `real entry experience` 终验。
- 当前薄弱点：
  - 这轮没有自己补 fresh live，所以对“最新两张图的体感”只能基于用户描述 + 当前代码，不冒充终验结论。
- 当前恢复点：
  - 下一轮如果继续 Workbench，最值钱的一刀顺序应是：
    1. 先去掉 idle `0/0`
    2. 再把底部区统一成一根多状态条
    3. 最后精修悬浮卡的居中、稳定排序和比例

## 2026-04-08：工作台距离外异常提示卡已收一刀，范围外 teaser 不再点亮左下角

- 当前主线目标：
  - 继续服务 `spring-day1` 的 Workbench 玩家面收口；本轮子任务是修掉“没进交互距离却还冒左下角工作台卡”的异常。
- 本轮完成：
  1. 查实根因不在 `CraftingStationInteractable` 本体距离门，而在更上层的 `SpringDay1ProximityInteractionService`：
     - `ShouldKeepOverlayVisibleForTeaser()` 仍允许 teaser 候选点亮 `InteractionHintOverlay`。
  2. 已将 `ShouldKeepOverlayVisibleForTeaser()` 收成直接返回 `false`，让 teaser 候选不再点亮左下角卡。
  3. 同轮把 `SpringDay1InteractionPromptRuntimeTests` 的旧断言回正为：
     - “Workbench teaser 在进入交互距离前应保持隐藏”。
- 验证：
  - `git diff --check` 通过
  - `validate_script --skip-mcp`：
    - `SpringDay1ProximityInteractionService.cs` owned_errors=0
    - `SpringDay1InteractionPromptRuntimeTests.cs` owned_errors=0
  - Unity/MCP fresh console 当前未拿到：
    - `sunset_mcp status / errors` 被外部 `listener_missing / WinError 10061` 卡住
- 当前判断：
  - 这刀最核心的玩家面异常已经在代码层被切到了。
  - 但当前没有 fresh live 证据，所以只 claim“结构成立 + 代码层 clean”，不 claim 终验过线。
- 当前薄弱点：
  - 当前没拿到 fresh GameView/live console；这刀仍待用户或后续 live 复测补最后一张证据。
- 当前恢复点：
  - 下一轮若继续，先让用户 fresh 验图二那张卡是否消失；
  - 若通过，再回到底部制作条和悬浮卡最终版收口。

## 2026-04-08：工作台距离外左下角 teaser 卡修复到可汇报刀口

- 当前主线目标：
  - 继续收 `Workbench` 玩家面异常；本轮子任务是修掉“未进入交互距离时，左下角仍冒 `靠近工作台 / 再靠近一些`”这张异常卡。
- 本轮完成：
  1. 跑了 `Begin-Slice`，owned 路径锁到：
     - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
     - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
     - `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
  2. 查实：
     - `CraftingStationInteractable` 自己已经在 closed/out-of-range 时挡了一层；
     - 真正更硬的开口在 `SpringDay1ProximityInteractionService.ShouldKeepOverlayVisibleForTeaser()`，它仍允许 teaser 点亮底部卡。
  3. 已把这条开口切掉：
     - `ShouldKeepOverlayVisibleForTeaser()` 现直接返回 `false`。
  4. 旧 runtime 测试口径同步回正：
     - `ProximityService_WorkbenchTeaser_ShouldUseBottomCardInsteadOfHeadHint`
     - 已改成
     - `ProximityService_WorkbenchTeaser_ShouldStayHiddenUntilInRange`
     - 期望也改成 overlay canvas `inactive`。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1InteractionPromptRuntimeTests.cs`
- 验证：
  - `git diff --check -- Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`：通过
  - `validate_script --skip-mcp` 两个 touched 文件：`owned_errors=0`
  - `sunset_mcp status / errors`：被外部 `listener_missing / WinError 10061` 卡住，未拿到 fresh live
- 当前判断：
  - 代码层已切掉图二这类异常卡的服务层开口。
  - 但因为 Unity MCP listener 当前不通，我还没拿到 fresh live 证据，不冒充终验。
- 当前薄弱点：
  - 这轮不能证明“玩家现场一定 100% 过线”，只能证明“代码层开口已经被切掉，值得你 fresh 复测”。
- 当前恢复点：
  - 下一轮优先看用户 fresh 验图二是否消失；
  - 若过，再继续回到底部多状态制作条和悬浮卡的最终版收口。

## 2026-04-08：只读核实，箱子 `E` 键近身交互不该由 UI 线程主刀

- 当前主线目标：
  - 回答“箱子 `E` 键近身交互该谁主刀、要不要互发 prompt”。
- 本轮完成：
  1. 只读核了：
     - `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
     - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
  2. 坐实：
     - `ChestController` 已有真开箱入口 `OnInteract()/OpenBoxUI()`；
     - `GameInputManager` 已有“点击箱子 -> 自动走近 -> 到点交互”链；
     - 真正缺的是“箱子接入近身 `E` 键 candidate / proximity / 提示抑制”。
- 当前判断：
  - 这条线是 `runtime / input / interaction` owner，不是 UI owner。
  - 因而应由 Farm/交互线程主刀；
  - UI 线程只适合后续配合提示文案或提示壳表现。
  - 最省成本的协作方式不是双向互发 prompt，而是直接让 Farm 开做；若必须走话术，我给他 prompt 比他给我 prompt 更合理。
- 当前恢复点：
  - 若后续继续推进箱子 `E` 键交互，默认把主刀给 Farm/交互线程，不把 runtime 主链切给 UI。

## 2026-04-08：day1 新 prompt 已确认是新的 UI 主刀切片，且已为 farm 落地箱子 E 键主刀 prompt

- 当前主线目标：
  - 回答 day1 新 prompt 是否只是旧方向延续，还是新的问题；
  - 并先把 farm 的箱子 `E` 键近身交互主刀 prompt 落出来。
- 本轮完成：
  1. 读取：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\2026-04-08_UI线程_day1最终闭环前打包字体与启动链prompt_07.md`
  2. 核实判断：
     - 它和我当前正在收的 `Workbench / 提示 / 近身交互表现` 不冲突；
     - 但确实是新的主刀切片；
     - 新主刀已切到 `Day1 打包字体异常 / 缺字 / 编辑器与打包版字体链一致性`。
  3. 已按用户要求先给 farm 落 prompt 文件：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\UI\2026-04-08_给farm_箱子E键近身交互主刀prompt_01.md`
- 当前判断：
  - 下一轮应分两条线：
    - Farm：箱子 `E` 键近身交互主刀
    - UI：day1 打包字体链主刀
  - 当前最省成本的协作方式不是双向互发 prompt，而是 farm 直接按现成 prompt 开做；我这边本轮先不生成新的 day1 prompt。
- 当前恢复点：
  - 若下一轮继续我自己的主线，直接切到 day1 新给的打包字体链；
  - 若要推进箱子 `E` 键，则直接把现成 prompt 发给 farm。

## 2026-04-08：箱子 `E` 键近身交互 worker 收口到最小可交接刀口

- 当前主线目标：
  - 作为并行 worker 收“箱子接入近身 `E` 键交互主链”，同时不破坏右键点击箱子后自动走近并开箱的旧链。
- 本轮完成：
  1. 核实当前现场：
     - `ChestController` 已经在当前工作区里接入 `SpringDay1ProximityInteractionService.ReportCandidate(...)`；
     - 触发动作已复用 `() => OnInteract(context)`；
     - `GameInputManager` 的右键自动走近链仍走 `HandleInteractable -> BeginPendingAutoInteraction -> TryInteract`；
     - `SpringDay1UiLayerUtility.IsBlockingPageUiOpen()` 已把 `BoxPanelUI.ActiveInstance.IsOpen` 作为阻塞页判定的一部分。
  2. 没有去重写运行时主链，避免踩到当前 shared root 里别的脏改；改为补最小守门测试，防止这条链后续被静默打回。
  3. 新增：
     - `Assets/YYY_Tests/Editor/ChestProximityInteractionSourceTests.cs`
     - `Assets/YYY_Tests/Editor/ChestProximityInteractionSourceTests.cs.meta`
  4. 新测试收住了 4 个关键事实：
     - 箱子近身提示继续走 `SpringDay1ProximityInteractionService`
     - 近身触发继续复用 `ChestController.OnInteract()`
     - 箱子 UI 打开仍会被阻塞页 / 同箱抑制链拦住
     - 右键点击箱子的自动走近交互链仍留在 `GameInputManager` 旧路径
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\ChestProximityInteractionSourceTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\ChestProximityInteractionSourceTests.cs.meta`
- 验证：
  - `validate_script Assets/YYY_Tests/Editor/ChestProximityInteractionSourceTests.cs --skip-mcp`：
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `validate_script` 针对：
    - `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
    - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
    - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
    结果均为：
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `git diff --check -- Assets/YYY_Tests/Editor/ChestProximityInteractionSourceTests.cs Assets/YYY_Tests/Editor/ChestProximityInteractionSourceTests.cs.meta`：通过
- 当前判断：
  - 这条箱子 `E` 键链在当前 shared root 里已经接上；
  - 本轮最值钱的补口不是再重写运行时，而是把“走 proximity 主链 / 复用 OnInteract / 不破坏右键旧链”的真值锁成最小测试。
- 当前薄弱点：
  - 这轮拿到的是代码层 guard，不是 fresh Unity/live 终验；
  - `validate_script` 仍因本机 `CodexCodeGuard timeout` 停在 `unity_validation_pending`，不能冒充 runtime 已终验。
- 当前恢复点：
  - 若后续继续这条线，下一步应由主刀线程补一次真实 runtime 验证：
    - 近身是否出现统一 `E` 提示并可直接开箱
    - 右键自动走近开箱是否仍正常
    - 箱子 UI 已开时是否不再重复冒提示或重复触发

## 2026-04-08：并行推进，Day1 打包字体链已实修一刀，Farm 箱子 E 键链已改为“吸收守门 + 关闭子智能体”

- 当前主线目标：
  - 并行推进两件事：
    1. `UI/day1` 打包字体异常、缺字、编辑器/打包版不一致；
    2. `Farm` 箱子近身 `E` 键交互。
- 本轮子任务：
  1. 主刀改 `DialogueChineseFontRuntimeBootstrap`，修动态中文字体在空 atlas 状态下被提前判死的问题；
  2. 审核并吸收箱子 `E` 键链结果，确认运行时主链已经接上，只补守门测试，不重写旧链。
- 已完成事项：
  1. `Assets/YYY_Scripts/Story/Dialogue/DialogueChineseFontRuntimeBootstrap.cs`
     - `WarmAndValidate()` 改为统一走 `TryPrepareCharacters(...)`；
     - `TryPrepareCharacters(...)` 去掉“先看 atlas、为空就直接 return false”的旧逻辑；
     - 空 atlas 时先预热/补字，再看 atlas 与 `HasCharacters(...)`；
     - 新增 `TryAddCharactersSafe(...)` 统一预热与补字调用。
  2. `Assets/YYY_Tests/Editor/DialogueChineseFontRuntimeBootstrapTests.cs`
     - 新增 build-like 编辑器测试：
       - `TMP_FontAsset.ClearFontAssetData(true)` 后仍应能通过 bootstrap 补回中文字形与 atlas。
  3. `Assets/YYY_Tests/Editor/ChestProximityInteractionSourceTests.cs`
     - 吸收并保留箱子 `E` 键守门测试；
     - 运行时代码主链当前不再额外重写。
  4. 并行子智能体 `019d6d85-da0b-7f21-b5e1-a5408483551a` 已关闭，避免继续占用。
- 关键决策：
  1. `day1` 新 prompt 是新的 UI 主刀切片，不再回漂 `Workbench`；
  2. `Farm` 箱子 `E` 键当前不是“还没接”，而是“主链已接，只差 live 终验”；
  3. 不吞外部红，当前第一条真实 blocker 是：
     - `Assets/YYY_Tests/Editor/NpcResidentDirectorRuntimeContractTests.cs(104,20)` 缺 `NPCAutoRoamController`
- 验证结果：
  - `validate_script` 针对：
    - `Assets/YYY_Scripts/Story/Dialogue/DialogueChineseFontRuntimeBootstrap.cs`
    - `Assets/YYY_Tests/Editor/DialogueChineseFontRuntimeBootstrapTests.cs`
    - `Assets/YYY_Tests/Editor/ChestProximityInteractionSourceTests.cs`
    - `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
    均为：
    - `owned_errors=0`
    - `external_errors=0`
    - `assessment=unity_validation_pending`
  - `sunset_mcp.py baseline` = `pass`
  - `sunset_mcp.py status`：
    - MCP listener 正常
    - Editor 非编译态
    - fresh console 有 1 条外部 error、2 条外部 warning
- 当前阶段：
  - `字体链`：代码层已收住 build 差异点，并有 build-like 测试；packaged build/live 终验仍待补。
  - `箱子 E 键`：运行时主链已成立，守门已补；fresh live 终验仍待补。
- 恢复点：
  1. 下轮若继续 UI 主线，优先补 packaged build / live 字体证据；
  2. 若继续 Farm 协作，只补箱子 `E` 键 fresh runtime 验证，不再回去重写主链。

## 2026-04-08：补刀，箱子 E 键已收成 toggle 闭环，并且 fresh console 回到 0 error

- 当前新增子任务：
  - 用户明确追加：`E` 打开箱子要做成 toggle，而且要和原有交互闭环。
- 本轮新增落地：
  1. `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
     - 同箱已开时，再次 `OnInteract()` 会经 `OpenBoxUI()` 直接关闭当前箱子页；
     - proximity 提示不再在“同箱已开”时整个抑制掉；
     - 提示文案切到 `关闭箱子 / 按 E 关闭箱子`。
  2. `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
     - 候选数据新增 `AllowWhilePageUiOpen`；
     - 当 page UI 已开但当前 pending 候选显式允许时，不再被统一阻塞链直接清掉。
  3. `Assets/YYY_Tests/Editor/ChestProximityInteractionSourceTests.cs`
     - 新增 toggle 守门：
       - 同箱已开时必须命中同一个真入口关闭；
       - proximity 服务必须允许这类 page-open 例外继续存活。
- 当前验证：
  - `validate_script` 针对上述 3 文件均为：
    - `owned_errors=0`
    - `external_errors=0`
    - `assessment=unity_validation_pending`
  - `git diff --check` 通过
  - `sunset_mcp.py status`：
    - `error_count=0`
    - `warning_count=2`
- 当前判断：
  - 箱子 `E` 键现在不是半截开箱，而是“同一真入口负责开/关，同箱已开时 proximity 也能继续存活”的闭环 toggle。
- 当前恢复点：
  1. 还缺的只剩 live 终验；
  2. 以及按用户要求，收尾时要额外产出给 `farm` 与 `UI/day1` 的回执 prompt 文件。

## 2026-04-08：已按用户要求补出 farm 与 UI/day1 两份回执 prompt

- 新文件：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026-04-08_给farm_箱子E键toggle闭环与live终验回执prompt_03.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\2026-04-08_UI线程_day1打包字体链与箱子toggle阶段回执prompt_08.md`
- 当前恢复点：
  - 如果这轮要给别人继续转发，不需要再从长聊天里拼装口径；
  - 直接读这两个文件即可。

## 2026-04-09：回到 UI 主线，只读审查 Package 地图页 / 关系页与包裹打开时的玩家面遮挡问题

- 当前主线目标：
  - 用户要求我先自己看清 UI 还存在哪些问题、这些问题本质是什么、应该怎么修，然后再决定是否进入真实施工。
- 本轮子任务：
  - 只读核实 `Package` 地图页、NPC 关系页，以及包裹页打开时任务卡/Prompt 是否会留在背后。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `delivery-self-review-gate`
- `sunset-startup-guard`、`sunset-workspace-router` 当前会话未显式暴露；已按 Sunset / 全局 `AGENTS.md` 做手工等价前置核查。因为本轮没有开始真实施工，所以未跑 `Begin-Slice`。
- 本轮实际读取：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Tabs\PackagePanelTabsUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Tabs\PackageMapOverviewPanel.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Tabs\PackageNpcRelationshipPanel.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Tabs\PackagePanelRuntimeUiKit.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1UiLayerUtility.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
- 已坐实的判断：
  1. `PackagePanelTabsUI.OpenPanel()` 没有统一处理外部玩家面 overlay；`PromptOverlay` 只是在自己下一次刷新时才看 `IsBlockingPageUiOpen()`，因此已经亮着的任务卡不会在包裹页打开瞬间被强制收掉。
  2. `PackageMapOverviewPanel` 和 `PackageNpcRelationshipPanel` 都只删自己创建的 `RuntimeRootName`，不处理页面里原有 legacy 节点；玩家看到的“旧文字/旧块透出来”很可能就是新旧两套内容叠在一起。
  3. 地图页右栏 `Overview / Presence / Route` 三块比例靠固定高度撑，信息量不大却占了太多高度，导致空白重、占位感强。
  4. 地图主体的 `VillageZone / RidgeZone` 底色块过大过实，会把路线、节点和标签压虚。
  5. 关系页右侧详情区是 `VerticalLayoutGroup + 多块固定高度` 组合，`HeroCard / StageCard / NarrativeRow / FooterCard` 在分辨率或文本长度变化时容易挤压、串层、产生测试板观感。
- 当前阶段判断：
  - 这轮不是“已经修好”，而是把 `Package` 这块 UI 的根因重新看清了；
  - 现在可以明确按三刀收：
    1. 包裹打开时外部 overlay 统一退场
    2. 地图/关系页旧内容退场
    3. 地图/关系页比例与留白纠偏
- 当前恢复点：
  - 如果用户让我继续落地，下一轮直接进入真实施工；
  - 第一刀只做上面三件事，不再泛修别的 UI。

## 2026-04-09：Package NPC 关系页收口为稳定父子布局

- 当前主线目标：
  - 只改 `PackageNpcRelationshipPanel.cs`，把 `4_Relationship_NPC/Main` 宿主上的关系页收成更稳的父子布局，不再继续探索别的 Package/UI 问题。
- 本轮子任务：
  - 先压掉 `Main` 宿主下非 runtime root 的 legacy 子节点，再把左侧名册与右侧详情改成 layout-group 驱动的稳定结构。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- `sunset-startup-guard`、`user-readable-progress-report`、`delivery-self-review-gate` 当前会话未显式暴露；已按 Sunset / 全局 `AGENTS.md` 做手工等价前置核查、用户向汇报和停步自省。
- 本轮 thread-state：
  - 已跑 `Begin-Slice -ForceReplace`，slice=`Package NPC关系页父子布局稳态重构`
  - 已跑 `Park-Slice`
  - 当前 `thread-state=PARKED`
- 本轮只改：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Tabs\PackageNpcRelationshipPanel.cs`
- 本轮实际做成：
  1. `EnsureBuilt()` 现在会先执行 `PreparePageHost()`，把 `Main` 宿主下除 `NpcRelationshipRuntimeRoot` 外的 legacy 子节点统一 `SetActive(false)`，避免旧内容和 runtime root 叠层。
  2. 左侧列表改成 `ListPanel -> Label -> Summary -> ScrollRoot -> Viewport -> Content` 的父子结构，不再靠标题/滚动区绝对锚点硬撑。
  3. 右侧详情改成 layout-group 驱动：
     - `HeroCard` = `HeroTopRow(身份列 + 状态卡) + QuotePlate`
     - `StageCard` = `StageHeader(标签 + 阶段 chip + hint) + StageTrack`
     - `NarrativeRow` = 两张纵向卡片，内部都是 `Label + Body`
     - `FooterCard` = 简单纵向收束
  4. 左侧名册卡片改成 `Accent + Portrait + TextColumn + StageChip` 横向父子布局，去掉名字/摘要/stage chip 的绝对锚点拼装。
  5. `CreateMetaRow()` 改成纵向 `Label + Value` 行语义，状态卡内部不再靠上下 offset 顶位置。
- 验证结果：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs --count 20 --output-limit 5`
    - 最终结果：`assessment=unity_validation_pending`
    - `owned_errors=0 / external_errors=0`
    - 当前只剩 1 条外部 warning：`LiberationSans SDF (Runtime)` 静态字库无法增补字符
    - `manage_script.validate` 仍为 `clean`
  - `git diff --check -- Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
    - 通过
- 当前没做的事：
  - 没继续做地图页、prompt、package 其它 tabs。
  - 没补 scene/live 最终玩家面截图；这轮只收结构稳定性，不宣称体验终验已过。
- 当前恢复点：
  - 如果下一轮继续，不要再回到“继续猜 offset”；
  - 直接从“真实打开 `4_Relationship_NPC/Main` 看 legacy 是否已被压住、右栏是否仍有文本溢出”继续。

## 2026-04-09｜只读审查：任务清单是否应像 Toolbar 一样处理

- 当前主线目标：
  - 用户要求先审查 `SpringDay1PromptOverlay`（任务清单）现在到底怎么走，并判断它是不是应该和 `ToolBar` 一样按同级 persistent UI 处理。
- 本轮子任务：
  - 不改代码，只核实 hierarchy、Canvas/sorting、Package 打开时的 suppress 链，以及 `PersistentPlayerSceneBridge` 对两者的处理差异。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
- `sunset-startup-guard` 当前会话未显式暴露；已手工完成等价前置核查。本轮只读，未跑 `Begin-Slice`。
- 已完成事项：
  1. 核实 `Primary.unity / Town.unity`：`ToolBar` 与 `SpringDay1PromptOverlay` 在 hierarchy 上都挂在 `UI` 根下。
  2. 核实 `SpringDay1PromptOverlay.ApplyRuntimeCanvasDefaults()`：它自带独立 `Canvas`，强制 `ScreenSpaceOverlay + overrideSorting + sortingOrder=152`。
  3. 核实 `PackagePanelTabsUI`：打开背包时会给 `PackagePanel` 自己补独立 `Canvas(sortingOrder=181)`，并主动调用 `promptOverlay.SetExternalVisibilityBlock(...)` 压任务清单。
  4. 核实 `PersistentPlayerSceneBridge`：它在边界跟踪里把 `ToolBar` 和 `SpringDay1PromptOverlay` 都当作 `persistentUiRoot` 直接孩子看待，但这只是 hierarchy/边界层面的同级，不是统一治理链。
- 关键决策/判断：
  - 用户这次判断是对的：任务清单现在“看起来和 toolbar 同级”，但“实现上不是 toolbar 那一套”。
  - 当前真实问题不是单纯层级错，而是渲染/显隐治理链分裂：
    - `ToolBar` 更像 persistent UI 大布上的普通兄弟节点；
    - `PromptOverlay` 是独立 overlay canvas，再靠其它 UI 各自通知它退场。
- 验证结果：
  - `静态推断成立`
  - `代码/scene 只读核实已对上`
  - `尚未进入 live 修复`
- 遗留问题 / 下一步：
  - 如果用户让我继续施工，下一步不该继续叠 suppress 例外，而是先把 `PromptOverlay` 的“同级治理口径”重新收一遍。

## 2026-04-09｜分工 prompt 定稿：我主刀共享 UI 治理，day1 只补剧情语义边界

- 当前主线目标：
  - 用户确认要把“任务清单处理链改成更像 toolbar 的同级治理口径”收成真正可转发的分工 prompt。
- 本轮子任务：
  - 不进代码修改，只把协同边界收成两条可直接转发的 prompt：
    1. 我主刀共享 UI 治理
    2. `day1` 只补正式剧情/任务阶段的显隐语义
- 本轮显式使用：
  - `skills-governor`
  - `sunset-prompt-slice-guard`
- 已完成事项：
  1. 确认 ownership：这刀根因在 `SpringDay1PromptOverlay / PackagePanelTabsUI / PersistentPlayerSceneBridge` 的共享 UI 治理，不在 `day1` 的任务推进真源。
  2. 确认分工：我主刀改显示治理；`day1` 只提供“哪些剧情阶段必须隐藏/恢复任务清单”的语义边界，不主刀 UI 壳。
  3. 生成两条可直接转发的 prompt 文本，后续用户可分别发给我和 `day1`。
- 验证结果：
  - `结构分工已定`
  - `尚未进入代码落地`

## 2026-04-09｜真实施工：任务清单开始回收到父级 UI 根治理

- 当前主线目标：
  - 直接落地“任务清单改成更像 toolbar 的同级治理口径”，优先解决背包/包裹页/正式对话下的显示关系，不再停留在审查。
- 本轮子任务：
  - 只改 `PromptOverlay` 的 Canvas/显隐治理 + `PackagePanelTabsUI` 的散落 suppress 链；不碰任务数据与剧情推进。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- thread-state：
  - 本轮开工前发现 `UI` 线程已处于 `ACTIVE`
  - 收尾前已补 `Park-Slice`
- 本轮实际改动：
  1. `SpringDay1PromptOverlay.cs`
     - `ApplyRuntimeCanvasDefaults()` 现在优先继承父级 `UI` 根 Canvas，不再默认强制独立 `overrideSorting=152`
     - `ShouldDelayPromptDisplay()` 不再把 `Package/Box` 打开本身当成必须 suppress 的理由
  2. `PackagePanelTabsUI.cs`
     - 删除 `OpenPanel / EnsurePanelOpenForBox / ClosePanel` 里的 `SyncExternalOverlaySuppression(...)`
     - 删除 `SyncExternalOverlaySuppression` 私有方法
  3. `SpringDay1LateDayRuntimeTests.cs`
     - 新增 `PromptOverlay_UsesParentCanvasGovernance_WhenUiRootCanvasExists`
- 关键判断：
  - 这轮不是“把 PromptOverlay 完全变没 Canvas”，而是先把它从“默认独立排序 overlay”拉回“优先服从父级 UI 根 Canvas”的口径；
  - 这样改动范围小，风险主要集中在显示关系，不会碰任务状态与剧情阶段。
- 验证结果：
  - `git diff --check`：
    - `PackagePanelTabsUI.cs` 通过
    - `SpringDay1LateDayRuntimeTests.cs` 通过
    - `SpringDay1PromptOverlay.cs` 通过，仅有既有 `CRLF/LF` warning
  - `sunset_mcp.py doctor`：`baseline pass`
  - `sunset_mcp.py compile`：`blocked`，原因是 `subprocess_timeout:dotnet:60s`
  - `manage_script validate`：当前 listener 在，但 `active Unity instance` 未就绪，没拿到 fresh validate
- 当前恢复点：
  - 用户现在可优先测：
    1. 正式对话时任务清单是否退场
    2. 打开背包/包裹页时任务清单是否不再被那套独立 overlay 链错误处理
  - 如果继续下一刀，先盯 residual 显示时机，再决定是否需要更深层去掉 `PromptOverlay` 自有 Canvas。

## 2026-04-09｜补刀：背包/包裹页打开时任务清单重新接回统一模态治理

- 当前主线目标：
  - 修掉“任务清单在背包/包裹页打开时仍然屹立不下”的 residual。
- 本轮子任务：
  - 只补一条集中化模态隐藏链：
    - utility 判定
    - PromptOverlay 消费
    - runtime test 守门
- thread-state：
  - 本轮已再次 `Park-Slice`
  - 当前状态：`PARKED`
- 本轮实际改动：
  1. `SpringDay1UiLayerUtility.cs`
     - 新增 `ShouldHidePromptOverlayForParentModalUi()`
  2. `SpringDay1PromptOverlay.cs`
     - `ShouldDelayPromptDisplay()` 重新接回 utility 模态判断
  3. `SpringDay1LateDayRuntimeTests.cs`
     - 新增 `PromptOverlay_HidesWhilePackagePanelIsOpen_AndRestoresAfterClose`
- 关键判断：
  - 这次补口不是回退到“PackagePanel 到处手动压 PromptOverlay”；
  - 而是把“包裹类模态页打开时任务清单该退场”集中回统一 utility，再由 `PromptOverlay` 自己消费。
- 验证结果：
  - `git diff --check` 通过（`SpringDay1PromptOverlay.cs` 只有既有 CRLF/LF warning）
  - fresh compile / validate 仍未闭环，原因还是这台机器的 CLI / Unity active-instance 取证链不稳定
- 当前恢复点：
  - 用户现在应该重测“背包/包裹页打开时任务清单是否终于下去”；
  - 如果还不过线，下一刀就不是再猜层级，而是继续查 `PackagePanel` 现场到底还有哪类 modal 壳没有被 utility 覆盖。

## 2026-04-09｜运行时补刀：Workbench 跨场景悬浮不再漏到 Home，HP/EP 条补了分辨率保底

- 当前主线目标：
  - 在 UI 主线里先收两个最新运行时 blocker：
    1. 工作台完成悬浮 / 产出提示跨场景穿到 `Home`
    2. 打包后 `HP/EP` 条不适配不同分辨率
- 本轮子任务：
  - 只改 `WorkbenchOverlay` 的 scene-aware 显示治理，以及 `HealthSystem / EnergySystem` 的 runtime 布局保底；
  - 不碰剧情推进、不碰任务判定、不扩到别的 UI 面。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- thread-state：
  - 本轮沿用已有 `ACTIVE slice = fix-workbench-floating-cross-scene-leak`
  - 收尾前已跑 `Park-Slice`
  - 当前状态：`PARKED`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\HealthSystem.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\EnergySystem.cs`
- 本轮做成了什么：
  1. `WorkbenchOverlay`：
     - 绑定状态所属 scene
     - scene 切换时只断开旧锚点与显示，不清空 `_queueEntries / readyCount`
     - `HasActiveCraftQueue / HasReadyWorkbenchOutputs / HasWorkbenchFloatingState` 改成只在所属 scene 内可见，避免别的 scene 误读旧工作台状态
  2. `HealthSystem / EnergySystem`：
     - 首次绑定时记住状态条原始 anchoredPosition
     - 每次分辨率变化时先恢复原位，再按 root canvas 可视范围做 clamp
     - 目标是保留现有视觉，不再飞出屏幕
- 关键判断：
  - `Workbench` 这刀最大的安全点是：我没有把跨场景 bug 粗暴修成“切场景就清空工作台队列”，而是只切显示链，保留原工作台真实状态；
  - `HP/EP` 这刀最大的不确定性是：当前拿到的是代码层和 console 层 clean，还没拿到 packaged build 实机图。
- 验证结果：
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 / external_errors=0`
  - `validate_script Assets/YYY_Scripts/Service/Player/HealthSystem.cs Assets/YYY_Scripts/Service/Player/EnergySystem.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 / external_errors=0`
  - `sunset_mcp.py errors --count 20 --output-limit 10`
    - `errors=0 warnings=0`
  - `git diff --check`
    - 无新的 blocking diff 错误
    - `SpringDay1WorkbenchCraftingOverlay.cs` 只有既有 `CRLF/LF` warning
- 当前恢复点：
  - 现在最值得用户直接测的就是：
    1. workbench 产物切场景不再漏显示
    2. 回到原 scene 工作台状态还能继续接回
    3. 不同分辨率下 `HP/EP` 条都还在屏内

## 2026-04-09｜任务清单层级纠偏：不再自抬排序，不再因背包打开而 fade

- 当前主线目标：
  - 按用户最新语义，把 `PromptOverlay` 收成真正和 `toolbar / state` 同级的基础层 UI。
- 本轮子任务：
  - 只改任务清单的层级/显示治理；
  - 不碰任务内容，不碰剧情推进，不再用透明度变化假装解决。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- thread-state：
  - 已跑 `Begin-Slice`
  - 已跑 `Park-Slice`
  - 当前状态：`PARKED`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
- 本轮做成了什么：
  1. `PromptOverlay` 在父级 `UI` Canvas 下不再独立抬排序，已经回到父级基础层治理。
  2. `ShouldDelayPromptDisplay()` 不再因为 `Package/Box` 打开就把任务清单 fade 掉。
  3. 对应测试也一起翻面：
     - 不再守“Prompt 必须排在父级 UI 之上”
     - 改守“Prompt 是基础层，Package 是更高模态层，但 Prompt 不应自己 fade”
- 当前判断：
  - 这刀真正修的是“身份错误”：
    - 之前 `PromptOverlay` 像一个特殊 overlay
    - 现在它才更像真正的基础层任务清单
  - 这更符合用户说的“它只需要老老实实在下面”。
- 验证结果：
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
    - `assessment=no_red`
    - `owned_errors=0 / external_errors=0`
  - `sunset_mcp.py errors --count 20 --output-limit 10`
    - `errors=0 warnings=0`
  - `git diff --check`
    - 无新的 blocking diff 错误
- 当前恢复点：
  - 现在该让用户直接看 live 结果：
    1. 打开背包时任务清单是否不再自己变透明
    2. 是否真的回到基础层语义
    3. 是否由更高层背包 UI 自己负责覆盖它

## 2026-04-09 20:31:51 +08:00｜任务清单父层补刀：修正错误子 Canvas 归属

- 当前主线目标：
  - 把 `PromptOverlay` 彻底收成和 `toolbar / state` 同语义的基础层，不再靠透明度变化掩盖层级问题。
- 本轮子任务：
  - 修正 `PromptOverlay` 运行时父层归属；
  - 防止它挂到 `PackagePanel` 这类模态子 Canvas；
  - 补一条自动回正测试。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- thread-state：
  - 已跑 `Begin-Slice`
  - 已跑 `Park-Slice`
  - 当前状态：`PARKED`
- 本轮实际改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
- 本轮做成了什么：
  1. `EnsureBuilt()` 不再因为已有绑定就跳过层级修正；现在会先回正父层，再继续绑定判断。
  2. `ResolveParent()` 改成优先选基础层 UI Canvas，主动避开 `PackagePanel / Dialogue / Workbench / Prompt / Tooltip` 一类错误子 Canvas。
  3. 新增测试覆盖“UI 根下同时存在基础 Canvas 和模态 Canvas”以及“误挂后自动回正”两种情况。
- 当前判断：
  - 这次终于碰到根因，不再只是边缘 suppress；
  - 但是否彻底符合用户体感，仍需 live 复测。
- 验证结果：
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
    - `assessment=no_red`
    - `owned_errors=0 / external_errors=0`
  - `git diff --check`
    - 仅 `CRLF/LF` warning，无新的 red
- 当前恢复点：
  - 等用户继续看背包打开时的真实层级效果；如果还有问题，下一刀应继续查实际 scene 里的基础层 Canvas 真值，而不是再回透明度补丁。

## 2026-04-09 20:50:31 +08:00｜只读核实：工作台配方真值与可扩展范围

- 当前主线目标：
  - 按用户要求，先核实 SO 真值，再回答工作台能直接补哪些工具 recipe、材料消耗怎么定。
- 本轮子任务：
  - 只读检查 `RecipeData / SpringDay1Workbench 资源目录 / 材料 SO / 工具 SO / 排序逻辑`。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-no-red-handoff` 的只读等价流程
- 本轮是否进入真实施工：
  - 否，仅只读
- 本轮做成了什么：
  1. 确认当前工作台只加载 `Resources/Story/SpringDay1Workbench`。
  2. 确认当前已有 recipe 只有 `9100~9104` 五个，其中工具只有 `Axe_0 / Hoe_0 / Pickaxe_0 / Sword_0`。
  3. 确认材料 SO 有木、石、黄铜矿/锭、生铁矿/锭、黄金矿/锭。
  4. 确认工具 SO 有全套 `Axe / Pickaxe / Hoe / Sword` 的 `0~5` 档。
  5. 确认排序逻辑目前不是纯 ID，而是自定义分组排序。
  6. 确认两条重要脏点：
     - 没有钢材料 SO
     - `Sword_1~5` 资源本体没配出真正高阶差异
- 当前判断：
  - 如果下一步真要落地，最稳的是先补 `Axe / Pickaxe / Hoe`，并把排序改成纯 `recipeID`；
  - `Sword_1~5` 是否一起上，要先看用户是否接受“先修 sword SO 再上 recipe”。
- 当前恢复点：
  - 这轮可以直接对用户给结论，不需要继续探查更多路径。

## 2026-04-09 20:55:40 +08:00｜补记：洒水壶和箱子可纳入工作台清单

- 只读确认：
  - `WateringCan` = `itemID 18`，当前只有 0 档；
  - `Storage` = `1400/1401/1402/1403` 四个箱子 SO；
- 用户向结论应更新为：
  - 可放入清单包含 `Axe/Pickaxe/Hoe` 的 `0/1/2/3/5` 档、`WateringCan`、`Sword_0`、所有箱子；
  - 继续排除钢档和 `Sword_1~5`。

## 2026-04-09 22:16:00 +08:00｜真实施工：工作台 recipe 扩展 + 本地 queue toast

- 当前主线目标：
  - 继续只收 `Workbench`：把用户已拍板的工具/箱子 recipe 真落到 `Resources/Story/SpringDay1Workbench`，并补“超过 6 项继续追加制作”时只在工作台内出现的提示条。
- 本轮子任务：
  - 修改 `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
  - 新增 `Assets/Resources/Story/SpringDay1Workbench/Recipe_9105~9120*.asset`
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮 thread-state：
  - 延续已开切片 `workbench-recipe-expansion-and-local-queue-toast-20260409`
  - 当前状态先前为 `ACTIVE`
- 本轮做成了什么：
  1. 工作台 recipe 排序已改为按 `resultItemID` 排，不再走旧的 `Axe -> Hoe -> Pickaxe -> 其他` 分组。
  2. 新增工作台本地 queue toast：
     - 只在工作台面板内部
     - 渐入 `0.25s`
     - 停留 `0.5s`
     - 渐出 `0.25s`
     - 只有队列来源数 `> 6` 且继续点击追加制作时才触发
  3. 旧 prefab 兼容已补：
     - 如果 prefab 没这个 toast 节点，runtime 会自动创建
     - `HideImmediate / CleanupTransientState / ClearRuntimeShellForRebuild` 都会清它
  4. 新增 recipe 资产 `16` 个，工作台总清单现在为：
     - `Axe_0/1/2/3/5`
     - `Pickaxe_0/1/2/3/5`
     - `Hoe_0/1/2/3/5`
     - `WateringCan`
     - `Sword_0`
     - `Storage_1400/1401/1402/1403`
- 本轮刻意保留的边界：
  - 钢档 `4` 继续不做，因为没有钢材料 SO；
  - `Sword_1~5` 继续不做，因为高阶剑 SO 真值不干净；
  - 当前只能 claim 到“结构/局部验证成立”，不能 claim 到 live 体验完全过线。
- 代码层验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --count 20 --output-limit 10`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - Unity 当时卡在 `compiling/stale_status`，不是 owned red
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py errors --count 20 --output-limit 10`
    - `errors=0 warnings=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs Assets/Resources/Story/SpringDay1Workbench`
    - 仅 `CRLF/LF` 提示，无 diff red
- 关键决策：
  - toast 只做工作台内局部提示，不去碰全局 UI；
  - recipe 只补当前数据真值干净的部分，不把钢档和高阶剑硬塞进来；
  - 先守住 no-red，再把资源补齐，不混成一坨施工。
- 当前恢复点：
  - 下一步若继续这条线，直接转 live 验工作台：
    1. 左列完整显示
    2. 新 recipe 排序是否符合用户认知
    3. 超过 6 项继续追加时 toast 是否只在工作台内提示

## 2026-04-09 22:18:10 +08:00｜Park-Slice 已执行

- 当前 `thread-state`：
  - `PARKED`
- 当前 blocker：
  - `user-live-validation-pending`
- 停车原因：
  - 这刀的代码与资源都已落地，剩余价值最高的是用户直接看工作台 live 体验，而不是继续盲改。

## 2026-04-09 22:35:55 +08:00｜只读恢复：Workbench 命名链问题复盘

- 当前主线目标：
  - 只读回答用户“为什么命名会这么烂、应该怎么调整”，不做代码改动。
- 本轮子任务：
  - 检查 `GetRecipeDisplayName / BuildMaterialsText` 和相关 item/recipe 资产文案真值。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
- 本轮是否进入真实施工：
  - 否
- 结论：
  1. `WateringCan` 是 `Tool_18_WateringCan.asset` 的原始 `itemName`。
  2. `3101_生铁锭 / 3102_黄金锭` 是材料 SO 的原始 `itemName`。
  3. 材料显示链没做清洗，直接把内部名搬到了玩家面。
  4. 工具显示链只做了粗暴泛化，没做真正的分档中文命名。
  5. 我新补 recipe 的简介里混入了明显开发口吻，这是我这轮自己的问题。
- 当前恢复点：
  - 下一刀若开做，先做工作台专用的玩家面文案层，不再直接信任 `recipeName / itemName / description` 原字段。

## 2026-04-09 22:59:33 +08:00｜真实施工：Workbench 玩家面名称/简介/材料名统一回正

- 当前主线目标：
  - 把工作台玩家面最显眼的内部名、开发口吻简介、材料前缀问题一次性收掉。
- 本轮子任务：
  - 只改 `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
- 本轮是否进入真实施工：
  - 是；已跑 `Begin-Slice`
- 本轮做成了什么：
  1. 新增工作台专用的玩家面名称映射；
  2. 新增工作台专用的玩家面简介映射；
  3. 材料显示统一走 `GetItemDisplayName()`，内部前缀不再直接上屏；
  4. `RefreshSelection()` 不再直接读 recipe/item 原 description。
- 本轮验证：
  - `validate_script`：`owned_errors=0`
  - `errors`：`0/0`
  - `git diff --check`：只有 `CRLF/LF` warning
- 当前恢复点：
  - 下一步看 live：确认 `WateringCan -> 洒水壶`、`3101_生铁锭 -> 生铁锭`、高阶工具名是否已可区分。

## 2026-04-09 23:00:30 +08:00｜Park-Slice 已执行

- 当前 `thread-state`：
  - `PARKED`
- 当前 blocker：
  - `user-live-validation-pending`

## 2026-04-09 23:17:45 +08:00｜用户新增硬约束：性能优化不得牺牲功能与需求

- 当前主线目标：
  - 用户提醒我不要像导航线程那样，为了优化性能把功能和需求抛之脑后，并要求我铭记。
- 本轮子任务：
  - 不做代码；把这条约束回写到线程与全局记忆层。
- 本轮显式使用：
  - `global-learnings`
- 关键结论：
  - 后续做性能、峰值、GC、卡顿或 UI 刷新优化时，必须先守住功能语义、交互闭环和玩家体验，不能把“弱化功能”包装成优化完成。
- 当前恢复点：
  - 后续如果接性能问题，这条约束优先级高于“先把指标压下来”的冲动。

## 2026-04-09 23:57:04 +08:00｜只读分析：UI 性能/交互异常根因与修法分层

- 当前主线目标：
  - 用户要求我不要再泛讲“怎么修”，而是把我自己对问题根因、修复顺序、快修/全修取舍想清楚并说清楚。
- 本轮子任务：
  - 只读审查 `SpringDay1WorkbenchCraftingOverlay.cs`、`SpringDay1PromptOverlay.cs`、`CraftingStationInteractable.cs` 的高频刷新点，并结合用户给的 Profiler 现象做根因分层。
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮是否进入真实施工：
  - 否；保持 `PARKED`
- 当前核心判断：
  1. 最大头不是单个工作台逻辑判断，而是 UI 强制重建风暴：`ForceRebuildLayoutImmediate`、`ForceMeshUpdate`、`LateUpdate` 常驻刷新、提示链反复上报，一起把 `EventSystem.Update / PlayerUpdateCanvases` 顶高。
  2. `SpringDay1WorkbenchCraftingOverlay` 是重灾区：`LateUpdate()` 每帧都跑 `UpdateFloatingProgressVisibility()`；行刷新、选中刷新、材料区几何刷新里分布大量立即重建。
  3. `SpringDay1PromptOverlay` 不是无辜旁观者：它在 `LateUpdate()` 每帧生成 state、判断 signature、并在多条链上 `ForceMeshUpdate / ForceRebuildLayoutImmediate`。
  4. `CraftingStationInteractable.Update()` 每帧 `BuildProximityInteractionContext()` + `ReportCandidate()`，更像“事件风暴放大器”；它未必是最大耗时，但会持续刺激提示/UI/事件系统。
  5. 这类问题本质不是“算法不会写”，而是当前 Spring UI 太依赖“状态一变就立刻强刷 Canvas/文本/布局”的做法。
- 我的修法分层：
  - 快修止血：
    1. 给 workbench 浮层、进度卡、材料区、选中详情区加脏标记和上一帧快照；
    2. 状态没变时，不再重复 `ForceRebuildLayoutImmediate / ForceMeshUpdate / ReportCandidate`；
    3. `LateUpdate` 只负责位置跟随，不再顺手做整套内容刷新；
    4. 提示链只在 caption/detail/可交互状态变化时重新上报；
    5. 先保体验和功能完全不变，把无效刷新拿掉。
  - 全修收口：
    1. 统一 Spring UI 的刷新模型，拆开“位置跟随”“内容变化”“显隐变化”“过渡动画”；
    2. workbench、prompt overlay、hint overlay、proximity service 走统一 dirty-driven 刷新；
    3. 再处理 `SpringDay1Director.Update()` 的 GC 热点，避免剧情链继续制造分配峰值。
- 当前恢复点：
  - 如果下一轮进入真实施工，第一刀不该乱改视觉，也不该砍功能；应先只收“高频强制重建风暴 + proximity 重复上报”。

## 2026-04-10 00:36:34 +08:00｜真实施工：Spring UI 刷新风暴止血第一刀已落地并停车

- 当前主线目标：
  - 把 `Workbench / PromptOverlay / ProximityService` 里的无效重建风暴和重复上报链打掉，不牺牲功能语义、交互闭环和玩家体验。
- 本轮子任务：
  - 修改：
    - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
    - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
    - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮是否进入真实施工：
  - 是；已跑 `Begin-Slice`
- 本轮做成了什么：
  1. `CraftingStationInteractable` 不再每帧重新找 player / overlay，改成缓存；工作台提示链统一走 `ResolveWorkbenchOverlay()`。
  2. `SpringDay1ProximityInteractionService` 增加左下角提示签名缓存；提示文案没变时，不再每帧重复 `ShowPrompt()`。
  3. `SpringDay1PromptOverlay` 先判是否应被 formal dialogue / modal 压住，再决定是否构建 prompt state；文本没变时不再强制 `ForceMeshUpdate()`。
  4. `SpringDay1WorkbenchCraftingOverlay` 去掉了 `RefreshSelection()` 里对整列 recipe row 的无效重刷。
  5. 工作台制作中的逐帧刷新，已从“整块 `UpdateQuantityUi()` 每帧重跑”改成更轻的 `RefreshRuntimeProgressVisuals()`。
  6. 工作台文本辅助链改成“内容真的变了才 `ForceMeshUpdate()`”。
  7. 工作台悬浮进度卡改成复用 buffer / pool，不再每帧 `new List + OrderBy + ToList` 分配排序结果。
- 当前验证：
  - `validate_script Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs --count 20 --output-limit 5`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 / external_errors=0`
  - `validate_script Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs --count 20 --output-limit 5`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 / external_errors=0`
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs --count 20 --output-limit 5`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 / external_errors=0`
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --count 20 --output-limit 5`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 / external_errors=0`
  - `git diff --check -- <4 files>`
    - 仅 `CRLF/LF` warning
  - `errors --count 20 --output-limit 5`
    - 被本机 MCP baseline 阻塞：`WinError 10061`
- 当前阶段判断：
  - 这刀已经把“代码层止血”落下去了，但 `Unity/MCP fresh live console` 还没拿到，所以不能 claim 体验或 live 已过。
- 当前 `thread-state`：
  - 已跑 `Park-Slice`
  - 当前状态：`PARKED`
- 当前恢复点：
  - 下一轮优先做 live 验收：
    1. 工作台制作中是否明显更稳，不再每帧拖慢整块底部 UI；
    2. 左下角交互提示在工作台附近是否不再无意义闪刷；
    3. formal dialogue / modal 打开时，任务卡是否不再白白构建状态。
