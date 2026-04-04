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
