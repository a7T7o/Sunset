# 0.0.2清盘002 memory

## 2026-03-27｜002 非正式聊天闭环继续推进，当前拿到首轮 live 证据

- 当前主线目标：
  - 继续推进 `0.0.2清盘002`，优先把 `002 / 003` 的按 `E` 发起、玩家先说、NPC 延迟回复、可继续推进、可跑开中断的首个非正式聊天闭环做实。
- 本轮子任务：
  - 先排掉 Unity 当前 `Play + Pause` 假阻塞。
  - 在不停止 Unity 的前提下，为 `002 / 003` 补最小 Editor 验证入口，争取直接拿到闭环 live 证据。
- 本轮完成：
  - 已确认 Unity 现场曾卡在：
    - `is_playing = true`
    - `is_paused = true`
    - `is_changing = true`
  - 已先恢复时间流动，不停止 Unity。
  - 已新增并持续扩展：
    - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCInformalChatValidationMenu.cs`
  - 当前验证菜单已具备：
    - 直接触发 `002` 非正式聊天
    - 直接触发 `003` 非正式聊天
    - 将玩家移出当前闲聊范围，触发跑开中断
    - 触发前自动清掉导航 live validation 的 pending key 与 runner 干扰
    - 闭环 trace 的第一版自动追踪骨架
- 本轮关键 live 证据：
  - Console 已明确打印：
    - `[NPCValidation] 已触发 002 的非正式聊天，source=002, boundaryDistance=0.400`
  - Unity MCP 回读到：
    - `PlayerNpcChatSessionService.HasActiveConversation = true`
    - `PlayerThoughtBubblePresenter.IsVisible = true`
    - `PlayerThoughtBubblePresenter.CurrentBubbleText = "你好，我能在这儿和你说两句吗？"`
    - `002 -> NPCBubblePresenter.IsBubbleVisible = true`
    - `002 -> NPCBubblePresenter.CurrentBubbleText = "可以呀，这边正好不吵，你慢慢说。"`
  - 这说明当前至少已拿稳：
    - `002` 可进入 NPC 非正式聊天
    - 玩家先说
    - NPC 延迟回复
    - 双气泡都会出现
- 本轮额外判断：
  - `NPC_002_VillageDaughterDialogueContent.asset`
  - `NPC_003_ResearchDialogueContent.asset`
  当前内容资产都确认保有每档关系下至少 2 条 `exchanges`，所以“第二轮推进没观测稳”更像是 live 验证链仍不稳定，不是内容结构本身只剩 1 轮。
- 本轮验证与阻塞：
  - `validate_script` 已通过：
    - `Assets/Editor/NPCInformalChatValidationMenu.cs`
  - `git diff --check` 已通过：
    - `Assets/Editor/NPCInformalChatValidationMenu.cs`
  - 项目编译当前已被本轮 own 修改清红。
  - 但 live 闭环 trace 仍未拿稳：
    - 自动 trace 在当前现场会出现“首轮未在 4 秒内完成”的超时
    - 手动二次推进时，现场会混入 TMP importer / MCP component serializer 噪音
  - 当前更像是：
    - 会话首轮逻辑已成立
    - 第二轮与跑开中断的 live 取证仍需要更稳的验证窗口或更窄的调试点
- 本轮边界：
  - 没有碰：
    - `Assets/000_Scenes/Primary.unity`
    - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese*.asset`
- 当前恢复点：
  - 下一步优先继续压：
    - `002` 第二轮推进为什么在 live 里还没稳定取到
    - `Break Active Informal Chat` 是否已真正打到玩家跑路反应
  - 如果 `002` 仍不稳，再平移到 `003` 做同路径复核，判断是共性问题还是 `002` 单点问题。

## 2026-03-27｜闭环 trace 已升级到真实状态驱动，002 两轮证据已拿到，但 003 与中断 live 仍被外部验证抢跑

- 当前主线目标：
  - 继续把 `0.0.2清盘002` 压到“`002 / 003` 可按 `E` 发起、玩家先说、NPC 延迟回复、至少两轮、且有最小中断入口”的首个可玩闭环。
- 本轮子任务：
  - 不碰 `Primary.unity / GameInputManager.cs / NPCAutoRoamController.cs runtime`，只在脚本和 Editor 验证层把闭环 trace 做实，并把假阳性触发日志收掉。
- 本轮完成：
  - 已修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCInformalChatInteractable.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCInformalChatValidationMenu.cs`
  - `PlayerNpcChatSessionService` 现已补出最小调试快照：
    - 当前会话状态名
    - 已完成 exchange 数
    - 最近一轮玩家句 / NPC 句
    - 最近一次中断玩家退出句 / NPC 反应句
    - 最近一次会话结束原因
    - 验证专用的 `RequestAdvanceOrSkipActiveConversation()`
    - 验证专用的 `TryStartWalkAwayInterruptForValidation()`
  - `NPCInformalChatInteractable` 现已补出：
    - `TryHandleInteract(context)`，验证菜单可以拿到真实是否接住输入，不再把失败触发也记成“已触发”
  - `NPCInformalChatValidationMenu` 现已升级为：
    - 用会话真实状态和已完成 exchange 数驱动 trace，而不是盲按一次 `E`
    - 分开支持 `Closure` 与 `Interrupt` 两类 trace
    - 在 trace 前清导航 pending key / `NavigationLiveValidationRunner`
    - 在 trace 期间尝试清掉活跃正式对白，降低 spring-day1 live 验证抢跑带来的假失败
    - 触发失败时会明确打印 `dialogueActive / boundaryDistance / 未接住输入`，不再输出假阳性成功日志
- 本轮测试结果：
  - `validate_script` 已通过：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
    - `Assets/Editor/NPCInformalChatValidationMenu.cs`
  - `git diff --check` 已通过：
    - 上述 3 份脚本
  - 已拿到一组更强的 `002` 两轮 live 证据：
    - `[NPCValidation] 002 首轮完成 | 玩家="我一过来就觉得这边安静多了。" | NPC="你要是愿意，我以后都给你留这个位置。"`
    - `[NPCValidation] 002 已发送第二轮推进输入。`
    - `[NPCValidation] 002 第二轮完成 | 玩家="你一开口，我就不想那么快走了。" | NPC="你肯多待一会儿，我心里会亮堂很多。"`
  - 结合上一轮 MCP 回读，当前已可确认：
    - `002` 进入会话
    - 玩家先说
    - NPC 延迟回复
    - 至少 2 轮来回
- 当前测试受阻：
  - Unity Play 现场存在持续外部干扰，不是这条线 own 代码红：
    - `Sunset/Story/Debug/*`
    - `SpringDay1LiveValidation`
    - `[NavValidation] RunRealInputSingleNpcNear`
  - 这些外部 live 验证会在 Play 中自动拉起：
    - `001` 正式对白
    - `WorkbenchFlashback`
    - `ReturnAndReminder`
    - 导航实跑
  - 结果是：
    - `002` 的中断 trace 会被主剧情对白抢跑或在途中被别线自动动作插队
    - `003` 的 closure / interrupt trace 当前还没拿到干净窗口
  - 因此这轮只能 claim：
    - 代码实现和验证链已向前实质推进
    - `002` 两轮证据已拿到
    - `002` 中断与 `003` 整组 live 仍是 `测试受阻`，没有冒充成 done
- 当前边界：
  - 仍未碰：
    - `Assets/000_Scenes/Primary.unity`
    - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese*.asset`
- 当前恢复点：
  - 下一个安静的 Play 窗口里，优先重跑：
    - `Tools/Sunset/NPC/Validation/Trace 002 Informal Chat Interrupt`
    - `Tools/Sunset/NPC/Validation/Trace 003 Informal Chat Closure`
    - `Tools/Sunset/NPC/Validation/Trace 003 Informal Chat Interrupt`
  - 如果外部 spring-day1 / navigation live 继续抢跑，就先把它记为明确 live blocker，不再把 trace 超时误判成 NPC 闭环本身没实现。

## 2026-03-27｜剩余三条 live 证据已补齐，`0.0.2清盘002` 首个闭环切片可判 done

- 当前主线目标：
  - 继续只做 `002 / 003` 的 NPC 非正式聊天首个闭环，不切 `Primary.unity`，不碰 `GameInputManager.cs`，把此前仍待补的 3 条 live 证据拿齐。
- 本轮子任务：
  - 先复核当前 Play 窗口是否真的被外部导航 live 抢跑。
  - 再直接在真实脏窗口里重跑：
    - `002 interrupt`
    - `003 closure`
    - `003 interrupt`
- 本轮关键现场：
  - 重新进 Play 后，Console 第一时间仍出现：
    - `[NavValidation] runtime_launch_request=RunRealInputSingleNpcNear`
  - 这说明此前对 external live blocker 的判断是对的，不是误判。
- 本轮完成：
  - 已在导航 live 已经起跑的窗口里直接执行 `002 interrupt`。
  - trace 过程中已明确看到：
    - `[NavValidation] runner_disabled`
    - `[NavValidation] runner_destroyed`
  - 随后已拿到 `002` 中断硬证据：
    - `[NPCValidation] 002 首轮完成 | 玩家="我一看见你，就想先过来和你说两句。" | NPC="你这么说，我这边一下就暖起来了。"`
    - `[NPCValidation] 002 已发送第二轮推进输入。`
    - `[NPCValidation] 002 已在第二轮等待回复阶段直接触发跑开中断，state=WaitingNpcReply`
    - `[NPCValidation] 002 跑开中断完成 | playerExitSeen=True | npcReactionSeen=True | playerExit="我先走一步，等会儿一定回来找你。" | npcReaction="你又这样突然跑掉，我会想你的。" | endReason=WalkAwayInterrupt`
  - 已拿到 `003` 闭环两轮硬证据：
    - `[NPCValidation] 003 首轮完成 | 玩家="我可以在这儿和你说几句吗？" | NPC="可以，但你先别乱动，我想把这个观察收完整。"`
    - `[NPCValidation] 003 已发送第二轮推进输入。`
    - `[NPCValidation] 003 第二轮完成 | 玩家="你看得这么认真，我都不敢大声说话了。" | NPC="你要是不介意，我想把你刚才的反应也记进去。"`
  - 已拿到 `003` 中断硬证据：
    - `[NPCValidation] 003 首轮完成 | 玩家="我又绕回来了，你这边还有新发现吗？" | NPC="有，而且你回来得正好，我刚想对一下前后的差别。"`
    - `[NPCValidation] 003 已发送第二轮推进输入。`
    - `[NPCValidation] 003 已在第二轮等待回复阶段直接触发跑开中断，state=WaitingNpcReply`
    - `[NPCValidation] 003 跑开中断完成 | playerExitSeen=True | npcReactionSeen=True | playerExit="我先走一下，待会儿再回来让你继续看。" | npcReaction="好吧，我先把这一段停在这里。" | endReason=WalkAwayInterrupt`
- 本轮验证：
  - 已主动把 Unity 从 Play 退回 Edit Mode，没有把运行态现场留给别人。
  - `validate_script` 已通过：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
    - `Assets/Editor/NPCInformalChatValidationMenu.cs`
    - `Assets/Editor/Story/DialogueDebugMenu.cs`
    - `Assets/YYY_Scripts/Service/Navigation/Editor/NavigationLiveValidationMenu.cs`
  - 当前仅剩 2 条脚本级 warning，不是 blocking error：
    - `PlayerNpcChatSessionService.cs`
    - `NPCInformalChatInteractable.cs`
    - 内容都是 `String concatenation in Update() can cause garbage collection issues`
  - `git diff --check` 已通过本轮 own 代码与文档范围。
  - Console 当前没有新增 own error；Play 期间仍可见的 warning 只有：
    - `There are no audio listeners in the scene. Please ensure there is always one audio listener in the scene`
- 当前结论：
  - `0.0.2清盘002` 的首个 NPC 非正式聊天闭环切片已经具备完整 live 证据：
    - `002` 两轮
    - `002` 中断
    - `003` 两轮
    - `003` 中断
  - 这轮不再是“代码面已做、但验证受阻”，而是功能闭环已真实跑通。
  - `003 closure` trace 在第二轮完成后仍会附带一条 `boundaryDistance / endReason=Completed` 的收尾日志；当前不影响“两轮闭环已拿证”的判定，暂不在这刀里继续改验证菜单语义。
- 当前边界：
  - 仍未碰：
    - `Assets/000_Scenes/Primary.unity`
    - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese*.asset`
- 当前恢复点：
  - 当前业务面已可从 `0.0.2清盘002` 继续往下一刀推进。
  - 已尝试执行：
    - `sunset-git-safe-sync.ps1 -Action sync -Mode task -OwnerThread NPCV2`
  - 当前 own 路径仍未 clean，但 blocker 已被钉实，不再是“收口范围不明”：
    - own roots 共剩 `14` 项 same-root dirty / untracked
    - 主要卡在：
      - `Assets/YYY_Scripts/Service/Player/*`
      - `Assets/YYY_Scripts/Story/Interaction/*`
      - `.kiro/specs/NPC/2.0.0进一步落地/*`
    - 其中包含此前 NPCV2 更早留下的同根尾账，例如：
      - `EnergySystem.cs`
      - `HealthSystem.cs`
      - `PlayerAutoNavigator.cs`
      - `PlayerInteraction.cs`
      - `PlayerNpcNearbyFeedbackService.cs`
      - `PlayerNpcRelationshipService.cs`
      - `PlayerThoughtBubblePresenter.cs`
      - `CraftingStationInteractable.cs`
      - `NPCDialogueInteractable.cs`
  - 当前下一步已从“补 live 证据”切换为：
    - 先做 NPCV2 own dirty / untracked 的同根收口裁剪
    - 再执行白名单 sync

## 2026-03-27｜`0.0.2清盘002` 第四刀：按用户原始 prompt 重锚，自动续聊矩阵、双气泡分型与 `001` 提示壳首轮美化已落地

- 当前主线目标：
  - 不再把 `0.0.2` 只当成“`002 / 003` 能跑”，而是按用户最初总 prompt 一起收：
    - `002 / 003` 非正式聊天交互矩阵
    - 玩家 / NPC 双气泡样式区分与防重叠
    - `001` 正式对话悬浮提示 UI 首轮美化
- 本轮子任务：
  - 把 `E` 从“推进下一轮”里拿掉，只保留：
    - 发起闲聊
    - 跳过当前打字动效
    - 催 NPC 更快回复
  - 给双气泡补会话期错位
  - 重做 `NpcWorldHintBubble`
  - 把 `walkAwayReaction.reactionCue` 从普通对白气泡改成更像情绪 cue 的表现入口
- 本轮完成：
  - 已修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubblePresenter.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCInformalChatValidationMenu.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\NpcWorldHintBubble.cs`
  - `PlayerNpcChatSessionService` 现已改成：
    - 首轮后自动续聊，不再停在“等再按一次 E 才能进下一轮”
    - 新增 `AutoContinuing` 状态
    - 会话期间实时给玩家 / NPC 气泡施加相反方向的错位偏移
    - 跑开时的 `reactionCue` 改走 `ShowReactionCue()`，不再跟正常长句完全同壳
  - `PlayerThoughtBubblePresenter` 现已改成：
    - 更亮、更冷调的玩家气泡配色
    - 左偏尾尖与左对齐文本
    - 会话期可注入临时偏移
  - `NPCBubblePresenter` 现已改成：
    - 会话期可注入临时偏移
    - 支持 `ReactionCue` 的紧凑情绪 cue 模式
  - `NPCInformalChatValidationMenu` 现已改成：
    - 首轮完成后直接进入“自动续聊观察”
    - 不再按旧口径再发一次推进输入
  - `NpcWorldHintBubble` 现已完成首轮重做：
    - 深色卡片底
    - 暖色键帽
    - 更明显的尾尖 / 竖向强调线
    - `001 / 002 / 003` 共用这层 NPC 提示壳
- 本轮 live 证据：
  - `002 interrupt` 已再次拿到自动续聊链硬证据：
    - `[NPCValidation] 002 首轮完成 | 玩家="我一看见你，就想先过来和你说两句。" | NPC="你这么说，我这边一下就暖起来了。"`
    - `[NPCValidation] 002 已进入自动续聊观察。`
    - `[NPCValidation] 002 已在第二轮等待回复阶段直接触发跑开中断，state=WaitingNpcReply`
    - `[NPCValidation] 002 跑开中断完成 | playerExitSeen=True | npcReactionSeen=True | ... | endReason=WalkAwayInterrupt`
  - `003 interrupt` 已拿到同路径证据：
    - `[NPCValidation] 003 首轮完成 | 玩家="我又绕回来了，你这边还有新发现吗？" | NPC="有，而且你回来得正好，我刚想对一下前后的差别。"`
    - `[NPCValidation] 003 已进入自动续聊观察。`
    - `[NPCValidation] 003 已在第二轮等待回复阶段直接触发跑开中断，state=WaitingNpcReply`
    - `[NPCValidation] 003 跑开中断完成 | playerExitSeen=True | npcReactionSeen=True | ... | endReason=WalkAwayInterrupt`
  - 上述日志说明：
    - 第二轮现在是自动进入，不再依赖第二次 `E`
    - 跑开中断链未被这轮改坏
- 本轮验证：
  - `validate_script` 已通过：
    - `PlayerNpcChatSessionService.cs`
    - `PlayerThoughtBubblePresenter.cs`
    - `NPCBubblePresenter.cs`
    - `NPCInformalChatValidationMenu.cs`
    - `NpcWorldHintBubble.cs`
  - `git diff --check` 已通过本轮 own 代码范围
  - 当前仅有非阻断 warning：
    - `PlayerNpcChatSessionService.cs`
    - `PlayerThoughtBubblePresenter.cs`
    - `NPCBubblePresenter.cs`
    - `NpcWorldHintBubble.cs`
    - 内容均为：`String concatenation in Update() can cause garbage collection issues`
  - Unity 已退回 Edit Mode
- 当前还没放行：
  - 玩家 / NPC 气泡在真实镜头里的“是否还重叠得离谱、是否已经足够区分”
    - 目前只有代码与 live trace 证据，仍需要用户画面级复测
  - `001` 悬浮提示卡是否“真的变好看、达到审美要求”
    - 目前只有首轮重做，仍需要用户视觉验收
  - 玩家真实手按 `E` 的手感
    - 当前 trace 证明同一交互链能自动续聊，但用户尚未亲手复测，不算放行
  - 跑开情绪反应
    - 当前已升到“紧凑 cue + 反应句”首版，不等于完整表情包系统
- 当前边界：
  - 本轮没有改写：
    - `Assets/000_Scenes/Primary.unity`
    - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese*.asset`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
- 当前恢复点：
  - 逻辑层已经从“靠再次按 E 推进”切到“自动续聊”
  - 体验层当前进入用户画面级验收窗口：
    - `001` 提示卡外观
    - `002 / 003` 双气泡是否好看且不再重叠
  - Git 收口仍未做，因为 `NPCV2` same-root 旧尾账尚未清干净

## 2026-03-27｜用户要求把“玩家离开矩阵”整理成后续施工标准，完整交互矩阵文档已落地

- 当前主线目标：
  - 不再零碎解释“玩家离开”行为，而是把当前真实代码矩阵、已知 bug、理想矩阵与补洞方案一次性钉成文档标准。
- 本轮完成：
  - 已新增文档：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-03-27-NPC-非正式聊天完整交互矩阵与查漏补缺方案-01.md`
  - 文档内容明确覆盖：
    - 当前真实状态机
    - 当前真实“玩家离开”矩阵
    - 当前 `WalkAwayInterrupt` 与 `Cancelled` 的分界
    - 用户已实测 bug 的归位
    - 理想 `LeaveCause / LeavePhase / ExitOutcome` 正交矩阵
    - 具体落地方案与施工顺序
- 当前结论：
  - 当前 NPC 非正式聊天不是“完全没内容”，而是“离开矩阵不完备”
  - 后续正确方向已经从“边修边猜”切换成“按矩阵文档逐项查漏补缺”
- 当前恢复点：
  - 下一刀如果继续做实现，优先级应变成：
    - `PlayerTyping -> 跑开` 卡字 bug
    - `Cancelled` cause 拆分
    - leave grace window
    - `LeaveCause / LeavePhase` 显式建模

## 2026-03-28｜P0 第一刀已实装：跑开卡字、离开宽限、取消原因拆分与阶段快照都已落地

- 当前主线目标：
  - 继续把 `0.0.2清盘002` 从“有矩阵文档”推进到“矩阵 P0 真落进代码和 live 验证”。
- 本轮子任务：
  - 先补测此前真正没测到的项。
  - 再把 `PlayerTyping -> 跑开` 卡字、leave grace、取消 cause 拆分做进代码。
  - 最后用 Unity/MCP 短窗口拿最关键的 live 证据。
- 本轮完成：
  - 已修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCInformalChatValidationMenu.cs`
  - `PlayerNpcChatSessionService` 现已补上：
    - `sessionBreakGraceSeconds`
    - `LeavePhase` 显式快照
    - `SystemTakeover / TargetInvalid / PlayerUnavailable / ServiceDisabled` 结束原因
    - `PlayerTyping -> 跑开` 的原子接管
    - `AutoContinuing` 不再继续靠 `E` 催下一句
  - `NPCInformalChatValidationMenu` 现已补上：
    - `002 / 003 PlayerTyping Interrupt` 靶向 trace
    - `abortCause / leavePhase` 日志字段
    - `Completed` 与 `WalkAwayInterrupt` 的收尾日志分流
- 本轮 live 证据：
  - `002 PlayerTyping Interrupt`
    - 已拿到：
      - `endReason=WalkAwayInterrupt`
      - `abortCause=DistanceGraceExceeded`
      - `leavePhase=PlayerSpeaking`
  - `003 PlayerTyping Interrupt`
    - 已拿到：
      - `endReason=WalkAwayInterrupt`
      - `abortCause=DistanceGraceExceeded`
      - `leavePhase=PlayerSpeaking`
  - `003` 二轮等待中断回归
    - 已再次拿到：
      - `leavePhase=NpcThinking`
  - `002` 闭环回归
    - 已明确报为：
      - `闭环收尾完成`
      - `endReason=Completed`
- 本轮验证：
  - Unity 已重新编译并多次进出 Play，最终已退回 Edit Mode。
  - `validate_script`
    - `PlayerNpcChatSessionService.cs` 通过
    - `NPCInformalChatValidationMenu.cs` 通过
  - `git diff --check`
    - 上述 own 代码范围通过
  - 当前 own 脚本仅余 1 条非阻断 warning：
    - `PlayerNpcChatSessionService.cs`
    - `String concatenation in Update() can cause garbage collection issues`
  - Console 当前无新增 own error。
- 当前边界：
  - 本轮仍未碰：
    - `Primary.unity`
    - `GameInputManager.cs`
    - `NPCAutoRoamController.cs`
    - `DialogueChinese*`
- 当前恢复点：
  - `0.0.2` 的 P0 已从“计划”变成“已实装 + 已拿 live 证据”。
  - 下一刀若继续做内容，不该再回头修 P0，而应进入：
    - `LeaveCause` 真正数据驱动化
    - `SystemTakeover / TargetInvalid` 的专门 trace 与体验收束
  - 当前仍待人工感受验收的项：
    - `001` 提示卡审美
    - `002 / 003` 双气泡防重叠与风格
    - 玩家真实 `E` 体感

## 2026-03-28｜用户要求暂停测试，当前已整理出“无需测试即可继续推进”的剩余任务口径

- 当前主线目标：
  - 继续完成 `0.0.2清盘002`，但用户明确要求当前先不要继续测试。
- 本轮结论：
  - 现在无需测试也能继续做的内容，核心都落在 `P1/P2` 代码与收口层，不在体验终验层。
  - 其中最高优先级不再是修 P0，而是：
    - `LeaveCause` 数据驱动化
    - `SystemTakeover / TargetInvalid` 的体验化收束实现
    - 文案/表现层结构扩展
    - own 路径收口准备
- 当前无需测试即可继续推进的事项：
  - 给 `PlayerNpcChatSessionService` 补更完整的 `LeaveCause`
  - 给 `NPCDialogueContentProfile / NPCRoamProfile` 补 cause/phase 对应的数据结构
  - 给 `NPCInformalChatValidationMenu` 继续补 `SystemTakeover / TargetInvalid` trace 入口
  - 继续扩 `reactionCue / emotion indicator / continuity` 的代码骨架
  - 整理 NPCV2 own dirty / untracked 收口清单
- 当前仍必须留到测试后的事项：
  - `001` 提示卡审美
  - `002 / 003` 双气泡真实镜头重叠与观感
  - 玩家真实 `E` 体感

## 2026-03-28｜P1 底层续推：中断续接底座与 phase-aware 兜底矩阵已接入服务层

- 当前主线目标：
  - 继续只做 `0.0.2清盘002` 的非正式聊天底层加固，把不依赖用户画面/手感终验的部分继续做厚。
- 本轮子任务：
  - 给 `PlayerNpcChatSessionService` 补“中断后可续接”的运行时底座。
  - 给空资产场景补 `LeaveCause x LeavePhase` 的默认兜底反应，避免中断链静默断掉。
  - 用最小脚本闸门确认这轮 own 脚本不新增红错。
- 本轮完成：
  - 已修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NPCInformalChatInterruptMatrixTests.cs`
  - `PlayerNpcChatSessionService` 现已补上：
    - `resumeWindowSeconds`
    - 中断续接快照：
      - `PendingResumeNpcId`
      - `PendingResumeExchangeIndex`
      - `PendingResumeAbortCause`
      - `PendingResumeLeavePhase`
    - 可续接原因白名单：
      - `DistanceGraceExceeded`
      - `BlockingUi`
      - `DialogueTakeover`
    - 可续接阶段白名单：
      - `PlayerSpeaking`
      - `NpcThinking`
      - `NpcSpeaking`
      - `BetweenTurns`
    - 同一 NPC 在续接窗口内重新发起时，会优先从中断前的 bundle / exchange 继续，而不是强制重新从头开聊。
    - 如果换了 NPC、续接过期、或快照失效，则不会误续接。
    - 对空内容资产的 fallback 中断反应，现已按 phase 分流：
      - `DistanceGraceExceeded`
      - `BlockingUi`
      - `DialogueTakeover`
  - `NPCInformalChatInterruptMatrixTests` 现已新增覆盖：
    - 可续接中断会从 `completedExchangeCount` 对应轮次恢复
    - `CompletionHold` 不允许进入续接
    - 续接快照过期或换 NPC 时不会命中
    - `NpcSpeaking` 被打断时能拿到 phase-aware fallback reaction
- 本轮验证：
  - `validate_script`
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs` 通过
    - `Assets/YYY_Tests/Editor/NPCInformalChatInterruptMatrixTests.cs` 通过
  - `git diff --check`
    - 上述 2 个 own 文件通过
  - Console 当前可见红错不是本轮 own 引入，而是外部 blocker：
    - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
    - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
    - 均为：`SpringDay1WorldHintBubble.HideIfExists` 缺失
  - 当前 own warning 仍只有：
    - `PlayerNpcChatSessionService.cs`
    - `String concatenation in Update() can cause garbage collection issues`
- 当前边界：
  - 本轮未碰：
    - `Primary.unity`
    - `GameInputManager.cs`
    - `NPCAutoRoamController.cs`
    - `DialogueChinese*`
    - `spring-day1` 工作台 / 正式剧情业务链
- 当前恢复点：
  - NPC 非正式聊天底层已从“有中断”推进到“中断后可记住现场并允许续接”。
  - 下一刀若继续纯底层，优先级应是：
    - 给 `SystemTakeover / TargetInvalid` 补专门验证入口
    - 再决定是否把 continuity 从“恢复轮次”继续扩成“恢复语义”
  - 当前 own 路径仍未 clean：
    - 上述 2 个 C# 文件仍是 `??`
    - 不能 claim sync-ready

## 2026-03-28｜P1 继续推进：续聊补口已数据驱动化，验证入口同步补齐

- 当前主线目标：
  - 继续把 `0.0.2清盘002` 的 continuity 从“只会记住断点”推进到“可配置、可验证、可继续扩内容”的底层系统。
- 本轮子任务：
  - 给 resume intro 接上 `NPCDialogueContentProfile / NPCRoamProfile / NPCInformalChatInteractable` 的数据链。
  - 给验证菜单补 `BlockingUi / DialogueTakeover` 的“中断后续聊” trace 入口。
- 本轮完成：
  - 已修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\NPCDialogueContentProfile.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\NPCRoamProfile.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCInformalChatInteractable.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCInformalChatValidationMenu.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NPCInformalChatInterruptMatrixTests.cs`
  - `NPCDialogueContentProfile` 现已补上：
    - `InformalChatResumeIntro`
    - `InformalChatResumeRule`
    - `defaultResumeRules`
    - stage resume rules
    - `GetResumeIntro(...)`
  - `NPCRoamProfile / NPCInformalChatInteractable` 现已补出：
    - `GetResumeIntro(...)`
    - `ResolveResumeIntro(...)`
  - `PlayerNpcChatSessionService` 现已升级为：
    - 续聊补口先查配置化 resume rule
    - 配不到时再走 fallback resume intro
    - 同 NPC 回来时不再只是“直接跳到下一轮”，而是会先播 resume intro 再继续 exchange
    - 对 `DistanceGraceExceeded` 的首轮未完成场景，仍保留“不要硬塞续聊补口”的保护
  - `NPCInformalChatValidationMenu` 现已新增：
    - `Trace 002 BlockingUi Resume`
    - `Trace 003 BlockingUi Resume`
    - `Trace 002 DialogueTakeover Resume`
    - `Trace 003 DialogueTakeover Resume`
    - trace 会按：
      - 第一轮完成
      - 强制中断
      - 检查 pending resume snapshot
      - 重新触发同 NPC
      - 观察 resume intro
      - 继续看目标轮次完成
      的顺序推进
  - `NPCInformalChatInterruptMatrixTests` 现已补上：
    - fallback resume intro 规则
    - `GetResumeIntro(...)` exact rule 命中
    - `NPCRoamProfile` 对 resume intro 的暴露链
- 本轮验证：
  - `validate_script` 通过：
    - `NPCDialogueContentProfile.cs`
    - `NPCRoamProfile.cs`
    - `NPCInformalChatInteractable.cs`
    - `PlayerNpcChatSessionService.cs`
    - `NPCInformalChatValidationMenu.cs`
    - `NPCInformalChatInterruptMatrixTests.cs`
  - `git diff --check`
    - 上述 own 代码范围通过
  - 当前 own warning 仍仅有 2 条非阻断 warning：
    - `NPCInformalChatInteractable.cs`
    - `PlayerNpcChatSessionService.cs`
    - 内容均为：`String concatenation in Update() can cause garbage collection issues`
  - 当前 Console 外部 blocker 口径未变：
    - `NPCDialogueInteractable.cs`
    - `CraftingStationInteractable.cs`
    - `SpringDay1WorldHintBubble.HideIfExists` 缺失
- 当前恢复点：
  - 这条线仍未到“只能停下等测试”的阶段。
  - 纯底层还能继续做的下一块，优先是：
    - `TargetInvalid` 是否需要独立验证入口
    - continuity 是否要进一步拆成“resume intro / resume outcome / resume cooldown”
  - 当前 own 路径仍未 clean，不能 claim sync-ready

## 2026-03-28｜编译修正后继续推进：`TargetInvalid` trace 与续聊冷却已补上

- 当前主线目标：
  - 在修掉 `NPCInformalChatValidationMenu` 的签名编译错误后，继续把非正式聊天底层往“更稳、更少重复话术、更易验证”推进。
- 本轮子任务：
  - 修复 `StartValidationTrace(...)` 新签名导致的 `CS7036`。
  - 给 `TargetInvalid` 补独立 trace 和 fallback 收束。
  - 给续聊补口加 cooldown，避免同一 NPC 连续打断后重复播同样的补口句。
- 本轮完成：
  - 已修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCInformalChatValidationMenu.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NPCInformalChatInterruptMatrixTests.cs`
  - `NPCInformalChatValidationMenu`：
    - 已把 `StartValidationTrace(...)` 的 `interruptOnly` 改成默认参数，当前这轮 own 编译错已清掉。
    - 已新增：
      - `Trace 002 TargetInvalid Abort`
      - `Trace 003 TargetInvalid Abort`
    - 强制中断 trace 现已支持：
      - 第一轮完成
      - 强制 `TargetInvalid`
      - 等待收尾
      - 输出 `abortCause / endReason / leavePhase / playerExit / npcReaction`
  - `PlayerNpcChatSessionService`：
    - 已补 `TargetInvalid / PlayerUnavailable / ServiceDisabled` 的 fallback interrupt reaction
    - 已补 `resumeIntroCooldownSeconds`
    - 已补同 NPC 续聊补口冷却抑制，避免短时间内重复播同样的 resume intro
  - `NPCInformalChatInterruptMatrixTests`：
    - 已补 `TargetInvalid` fallback snapshot 测试
    - 已补 `ShouldSuppressResumeIntro(...)` 冷却测试
- 本轮验证：
  - `validate_script` 通过：
    - `NPCInformalChatValidationMenu.cs`
    - `PlayerNpcChatSessionService.cs`
    - `NPCInformalChatInterruptMatrixTests.cs`
  - `git diff --check`
    - 上述 own 文件通过
  - 编译刷新后，Console 已不再出现本轮那组 `NPCInformalChatValidationMenu.cs CS7036`
  - 当前 Console 仅见：
    - `MCP-FOR-UNITY [WebSocket] Unexpected receive error: WebSocket is not initialised`
    - 不属于本轮 NPC own 编译错误
  - 当前 own warning 仍只有：
    - `PlayerNpcChatSessionService.cs`
    - `String concatenation in Update() can cause garbage collection issues`
- 当前恢复点：
  - 这条线仍然还没到“只剩测试”的阶段。
  - 纯底层剩余更像是：
    - continuity 是否还要再拆 `resume outcome`
    - 是否需要把当前 warning 顺手降掉
  - 当前 own 路径仍未 clean，不能 claim sync-ready

## 2026-03-28｜P1 再压一层：续聊结果语义已落地，trace 现在能分清“为什么没补口”

- 当前主线目标：
  - 继续把非正式聊天 continuity 做成“可解释、可验证”的底层系统，不只是能续，还要知道这次续接结果到底是什么。
- 本轮子任务：
  - 给服务层补 `resumeOutcome` 结果语义。
  - 给验证菜单日志同步带上 `resumeOutcome`。
  - 把这层语义补进 Editor 测试。
- 本轮完成：
  - 已修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCInformalChatValidationMenu.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NPCInformalChatInterruptMatrixTests.cs`
  - `PlayerNpcChatSessionService` 现已新增：
    - `ConversationResumeOutcome`
      - `ResumedWithIntro`
      - `ResumedSilently`
      - `SuppressedByCooldown`
      - `Expired`
      - `DifferentNpc`
      - `InvalidSnapshot`
    - 调试快照：
      - `LastResumeOutcome`
      - `LastResumeOutcomeName`
      - `LastResumeNpcId`
    - 当 pending resume 命中、被冷却压掉、过期、或换 NPC 时，服务层现在会显式记录 outcome，而不是只留现象
  - `NPCInformalChatValidationMenu`：
    - 续聊 trace 的关键日志现在会带：
      - `resumeOutcome`
  - `NPCInformalChatInterruptMatrixTests`：
    - 已补：
      - `ResolveMatchedResumeOutcome`
      - `ResolveUnavailablePendingResumeOutcome`
      - `TryResolvePendingResumePlan` 的 different NPC outcome
- 本轮验证：
  - `validate_script` 通过：
    - `PlayerNpcChatSessionService.cs`
    - `NPCInformalChatValidationMenu.cs`
    - `NPCInformalChatInterruptMatrixTests.cs`
  - `git diff --check`
    - 上述 own 代码范围通过
  - 编译刷新后，这轮没有新增 own 编译错误
  - 当前 Console 新可见信号主要是外部现场问题，不是本轮 own red：
    - `The referenced script (Unknown) on this Behaviour is missing!`
    - `DialogueChinese SDF` 的 ellipsis 字符缺失
    - `OcclusionManager` 注册等待超时
- 当前恢复点：
  - 这条线仍未到“只能停下等测试”的阶段
  - 纯底层可继续项已从“大结构缺失”收缩到：
    - 是否要继续降 `String concatenation in Update()` warning
    - 是否再补更细的 resume / abort outcome 分流
  - 当前 own 路径仍未 clean，不能 claim sync-ready

## 2026-03-28｜live 重测结果：`002 BlockingUi Resume` 已拿证，`TargetInvalid` 受导航实跑抢占

- 当前主线目标：
  - 利用用户给出的安静窗口，复测这轮新加的 resume / abort 验证入口，而不是只停在脚本级通过。
- 本轮 live 结果：
  - 已成功重测：
    - `Tools/Sunset/NPC/Validation/Trace 002 BlockingUi Resume`
  - 已拿到的关键证据：
    - `002 首轮完成`
    - `002 已执行续聊前强制中断`
    - `abortCause=BlockingUi`
    - `leavePhase=BetweenTurns`
    - `002 已留下续接快照`
      - `pendingNpc="002"`
      - `pendingExchangeIndex=1`
      - `pendingAbortCause=BlockingUi`
      - `pendingLeavePhase=BetweenTurns`
    - `002 已进入续聊补口`
      - `resumePlayer="刚才被别的事打断了，我们接着说。"`
      - `resumeNpc="好，我们就从刚才那儿接着往下说。"`
      - `resumeOutcome=ResumedWithIntro`
    - `002 续聊完成`
      - 第二轮玩家/NPC 正常完成
      - `resumeOutcome=ResumedWithIntro`
- 本轮 live blocker：
  - `Tools/Sunset/NPC/Validation/Trace 002 TargetInvalid Abort`
    - 两次尝试都未进入 `[NPCValidation]` 主链
    - Console 只看到 `[NavValidation]` 自动实跑链路
    - 可判定为：
      - 当前 `TargetInvalid` live 仍受导航自动验证抢占
      - 不是这轮 NPC own 代码编译红
      - 也不是用户再次占用窗口
- 本轮现场管理：
  - 已在结束前把 Unity 退回 Edit Mode
  - 没有关闭 Unity
- 当前恢复点：
  - 当前可明确 claim：
    - `BlockingUi Resume` 已有 live 硬证据
  - 当前仍待 live：
    - `TargetInvalid Abort`
    - `DialogueTakeover Resume`
  - 当前这条线已经越来越接近“只剩 live/体验项”，但还没完全到那一步

## 2026-03-28｜补记断点：`002 DialogueTakeover Resume` 曾在首句 `PlayerTyping` 卡住，现场已退回 Edit Mode

- 当前主线目标：
  - 继续把 `0.0.2清盘002` 压到“底层只剩 live/体验项”，优先确认 `DialogueTakeover Resume` 是真实代码问题，还是当时 Play 窗口本身已不稳定。
- 本轮补记的未回写事实：
  - 在一次新的 Play 会话里，已执行：
    - `Tools/Sunset/NPC/Validation/Trace 002 DialogueTakeover Resume`
  - 当次 trace 没有正常走完首轮，而是超时停在：
    - `state=PlayerTyping`
    - `playerText="我"`
    - `npcText=""`
  - 这说明当次最可疑的点不是“续聊后半段没接上”，而是：
    - 首句玩家打字阶段就可能卡在首字。
  - 紧接着尝试再跑：
    - `Tools/Sunset/NPC/Validation/Trace 002 Informal Chat Closure`
    - 但当时验证菜单已经没拿到稳定 Play 现场，因此还不能下结论说“基线 closure 也坏了”。
- 本轮现场管理：
  - 随后已执行停止现场确认：
    - `Already stopped (not in play mode)`
  - Console 也已清理。
  - 因此当前恢复点应按：
    - Unity 已不在 Play Mode
    - 需要重新进入 fresh Play 窗口
    - 先观察是否有 `[NavValidation]` 抢跑
    - 再依次重跑 `002 closure -> 002 DialogueTakeover Resume`
    来理解，而不是继续沿用上一轮已抖动的现场。
- 当前恢复点：
  - `002 BlockingUi Resume` 仍然是已拿证状态。
  - `002 DialogueTakeover Resume` 当前应视为：
    - 有一条真实可疑现象
    - 但还需要 fresh Play 二次复核，才能决定是代码 bug 还是现场抖动。

## 2026-03-28｜fresh Play 复核补齐：`resume / target-invalid` live 组已全绿，验证菜单收尾日志也已修正

- 当前主线目标：
  - 继续把 `0.0.2清盘002` 压到“只剩用户画面/手感终验”，先把此前未稳的 live 入口和验证语义补齐。
- 本轮 live 结果：
  - 已再次跑通：
    - `Tools/Sunset/NPC/Validation/Trace 002 Informal Chat Closure`
    - `Tools/Sunset/NPC/Validation/Trace 002 DialogueTakeover Resume`
    - `Tools/Sunset/NPC/Validation/Trace 003 BlockingUi Resume`
    - `Tools/Sunset/NPC/Validation/Trace 003 DialogueTakeover Resume`
    - `Tools/Sunset/NPC/Validation/Trace 002 TargetInvalid Abort`
    - `Tools/Sunset/NPC/Validation/Trace 003 TargetInvalid Abort`
  - 已拿到的关键证据：
    - `002 closure`
      - `首轮完成`
      - `第二轮完成`
      - `闭环收尾完成`
      - `endReason=Completed`
    - `002 DialogueTakeover Resume`
      - `abortCause=DialogueTakeover`
      - `leavePhase=BetweenTurns`
      - `pending resume snapshot`
      - `resume intro`
      - `resumeOutcome=ResumedWithIntro`
      - `endReason=Completed`
    - `003 BlockingUi Resume`
      - `abortCause=BlockingUi`
      - `leavePhase=BetweenTurns`
      - `resumeOutcome=ResumedWithIntro`
    - `003 DialogueTakeover Resume`
      - `abortCause=DialogueTakeover`
      - `leavePhase=BetweenTurns`
      - `resumeOutcome=ResumedWithIntro`
    - `002 / 003 TargetInvalid Abort`
      - `abortCause=TargetInvalid`
      - `endReason=TargetInvalid`
      - `leavePhase=BetweenTurns`
      - 玩家退出句与 NPC 反应句都已看到
- 本轮额外判断：
  - 之前那次：
    - `state=PlayerTyping`
    - `playerText="我"`
    - `npcText=""`
    当前应判为：
    - 不稳定 Play 现场
    - 而不是 `DialogueTakeover Resume` 逻辑本身坏掉
  - 这轮还额外看到：
    - `[NavValidation] pending_action_suppressed_by_npc_validation`
    - `[NavValidation] dispatch_suppressed_by_npc_validation source=entered_play_mode`
    说明当前 NPC validation 会显式压住导航 pending action，不再像前一轮那样轻易被导航抢跑。
- 本轮代码补口：
  - 已修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCInformalChatValidationMenu.cs`
  - 已把续聊 trace 的成功日志延后到真正收尾落定后再打印：
    - 不再把 `endReason=None` 当成最终成功回执
    - 当前 `002 DialogueTakeover Resume` 已重新打到：
      - `endReason=Completed`
- 本轮验证：
  - `validate_script` 通过：
    - `Assets/Editor/NPCInformalChatValidationMenu.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Data/NPCDialogueContentProfile.cs`
    - `Assets/YYY_Scripts/Data/NPCRoamProfile.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
    - `Assets/YYY_Tests/Editor/NPCInformalChatInterruptMatrixTests.cs`
  - `git diff --check` 通过：
    - 上述 own 代码与本轮 memory/doc 范围
  - 当前 own warning 仍只有 2 条非阻断 warning：
    - `PlayerNpcChatSessionService.cs`
    - `NPCInformalChatInteractable.cs`
    - 内容均为：
      - `String concatenation in Update() can cause garbage collection issues`
  - Unity 已主动退回 Edit Mode，没有关闭 Unity。
- 当前恢复点：
  - `resume / abort` 这组底层 live 证据现在已从“部分拿证”推进到“整组补齐”。
  - 这条线若继续往下做，优先级应收缩到：
    - `LeaveCause x LeavePhase` 内容资产继续做厚
    - 视觉与手感终验前的局部精修
  - 当前真正仍待人工验收的项没有变化：
    - `001` 提示卡审美
    - `002 / 003` 双气泡真实镜头是否还重叠
    - 玩家真实按 `E` 的体感

## 2026-04-01｜0.0.2 第二轮体验补口：左下角统一提示壳 + 头顶小箭头 + 跑开中断止血

- 当前主线目标：
  - 把 NPC 非正式聊天从“底层状态机能跑”继续压到“交互壳和中断链至少不再明显违和”，优先修用户已明确指出的体验问题。
- 本轮完成：
  - 已继续真实改代码，不再停在文档层。
  - 统一提示壳进一步收口：
    - `SpringDay1ProximityInteractionService.cs`
      - 新增 `showWorldIndicator`
      - 允许“左下角提示还在，但头顶不再挂箭头”
    - `NPCInformalChatInteractable.cs`
      - 非正式聊天进行中改为：
        - `showWorldIndicator = false`
      - 结果是：
        - 头顶小箭头只表示“当前可发起交互”
        - 闲聊进行中不再和聊天气泡抢头顶位置
    - `SpringDay1WorldHintBubble.cs`
      - 头顶提示继续收窄成更小的倒三角
      - 跳动幅度进一步减小
      - 常态不再显示大 `E`
      - 常态不再显示明显 backplate，只保留更轻的头顶指示
    - `InteractionHintOverlay.cs` / `InteractionHintDisplaySettings.cs`
      - 左下角统一提示区与可隐藏设置接口继续保留
      - 当前口径仍是：
        - 提示显示开关关掉时
        - 头顶箭头与左下角提示会一起隐藏
  - 会话与中断链补口：
    - `PlayerNpcChatSessionService.cs`
      - `GetPromptDetail()` 已改成：
        - 默认说明“会自动续聊 / 按 E 只跳过动效或停顿”
      - `HandleInteract()` 已补：
        - `AutoContinuing`
        - `Interrupting`
        - `Completing`
        这些阶段现在也能被 `E` 正常跳过等待/收尾，而不再只会卡着等
      - 跑开中断链已改为：
        - 先强清旧会话气泡
        - 再直接显示玩家离场句 / NPC 反应
        - 不再复用那条容易鬼畜的旧打字机链
      - 正常收尾 `EndConversation()` 现改成：
        - 玩家 / NPC 气泡都走正常隐藏
        - 不再把玩家气泡一刀 `HideImmediate`
      - 强制取消 `CancelConversationImmediate()` 仍保留：
        - 立刻清场
        - 但现在会先清布局偏移再收
  - 气泡表现补口：
    - `PlayerThoughtBubblePresenter.cs`
      - `BeginTypedText()` 改为：
        - 打字机更新文本
        - 但不再每句重新做透明渐显
      - 新增公开 `HideBubble()`
        - 供正常会话结束时走平滑收起
    - `NPCBubblePresenter.cs`
      - `BeginTypedConversationText()` 同样改为：
        - 不再每句重新透明渐显
- 本轮验证：
  - `git diff --check` 通过：
    - 本轮 own 修改脚本范围无空白/补丁格式问题
  - Unity fresh compile 已拿到：
    - `Editor.log`
    - `ExitCode: 0`
    - `*** Tundra build success (4.34 seconds)`
    - 本轮修改已被 Editor 重新编进项目
  - 当前仍未拿到的证据：
    - MCP / `127.0.0.1:8888`
      - 仍不通
    - 因此本轮还没有补到新的运行态 live 复测
- 当前恢复点：
  - 代码侧当前最关键的变化是：
    1. 非正式聊天进行中不再把“交互箭头”和“聊天气泡”堆在 NPC 头顶
    2. 打字机开始时不再让气泡透明渐显
    3. 跑开中断不再复用容易鬼畜的 typed interrupt 链
  - 下一次继续时，第一优先级应直接做运行态重测：
    - `002 / 003`
    - 开聊
    - 自动续聊
    - 跑开中断
    - 头顶箭头 / 左下角提示同步

## 2026-04-01｜0.0.2 自测续记：四条 NPCValidation 全绿，提示壳改由会话服务强收口

- 当前主线目标：
  - 把 `002 / 003` 的非正式聊天闭环从“代码看起来对”推进到“Unity 运行态自测能连续过线”，同时把闲聊进行中的提示壳逻辑再收紧一层。
- 本轮完成：
  - 验证器补口：
    - `Assets/Editor/NPC/NPCInformalChatValidationMenu.cs`
    - 新增 `Application.runInBackground` 临时接管与恢复，避免失焦时 validation trace 卡在第一两个字。
  - 会话提示壳收口：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - 闲聊进行中改为由会话服务直接：
      - 压掉 `SpringDay1WorldHintBubble`
      - 刷新左下角 `InteractionHintOverlay`
    - 不再只赌 proximity service 下一帧自己把 UI 切对。
  - UI 只读暴露补口：
    - `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - 增加 `IsVisible / CurrentKeyLabel / CurrentCaptionText / CurrentDetailText`
    - 便于后续直接读运行态提示状态。
  - compile blocker 清红：
    - `Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`
    - 改成纯反射取 Sunset runtime 类型，不再直接编译依赖 NPC runtime 程序集与 `TMPro`
    - 解决了它把 Play Mode 直接挡死的问题。
- 本轮验证：
  - `validate_script` 通过：
    - `PlayerNpcChatSessionService.cs`
    - `NPCInformalChatValidationMenu.cs`
    - `InteractionHintOverlay.cs`
    - `SpringDay1InteractionPromptRuntimeTests.cs`
  - `git diff --check` 通过：
    - 只有 LF/CRLF warning，无新增 whitespace error
  - Unity 自测结果：
    - `002 closure`
      - `endReason=Completed`
    - `002 interrupt`
      - `endReason=WalkAwayInterrupt`
      - `playerExitSeen=True`
      - `npcReactionSeen=True`
    - `003 closure`
      - `endReason=Completed`
    - `003 interrupt`
      - `endReason=WalkAwayInterrupt`
      - `playerExitSeen=True`
      - `npcReactionSeen=True`
  - 当前仍未完全闭环的证据：
    - MCP 这轮对“闲聊进行中的中间帧 UI”取证不稳定，数次采样会落到已收尾帧
    - 因此“左下角提示中途具体显示哪一句文案”的最终观感，仍建议保留给人工终验。
- 当前恢复点：
  - 逻辑自测层现在已经不再卡在“第一两个字”或“跑开没有回收”
  - 下一次继续时，不应再回到补状态机；应直接做：
    - 人工看感受的提示 UI 验收
    - NPC 进行中提示文案是否完全符合最终体验预期

## 2026-04-01｜共享提示壳边界复核：当前 blocker 不再允许 NPC 把 Day1/UI 壳整包吞下

- 当前主线目标：
  - 重新判断 `0.0.2` 当前 blocker 里，哪些文件仍属于 NPC 非正式聊天 own，哪些已进入 shared 提示壳范围。
- 本轮只读结论：
  - `NPC` 仍应继续 own：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
  - 不应继续作为 `NPC` own 维持的 shared 壳：
    - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
    - `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - `Assets/YYY_Scripts/Story/UI/InteractionHintDisplaySettings.cs`
    - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
    - `Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`
  - 判断原因：
    - 当前统一近身仲裁、头顶世界提示、左下角提示卡、提示显隐设置都已经是 shared prompt shell；
    - 它们不仅服务 NPC 闲聊，还服务工作台、床、正式对话与 Day1 运行态摘要；
    - 因此 NPC 后续若继续施工，必须把 slice 收窄回四件套，不能再把“共享提示壳最终观感”伪装成 NPC 单线可独立完工项。
- 当前恢复点：
  - `0.0.2` 后续如果恢复真实施工，NPC 的最小下一刀应改成：
    - 只在四件套里继续压实自动续聊 / 跑开收束 / 双气泡占位
  - shared 提示壳部分维持待 shared owner 接盘，不再继续扩 claim

## 2026-04-01｜用户终验总包已落盘：0.0.2 这条线正式切到“等用户按矩阵回单”

- 当前主线目标：
  - 用户这轮不要我继续写新功能，而是要一份能直接拿去手测的总包：
    - 现在做到哪一步
    - 还剩什么要验
    - 详细情况矩阵
    - 详细测试矩阵
    - 可直接回填的回执单
- 本轮完成：
  - 已新建：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-01-NPC当前阶段用户验收总包-01.md`
  - 文档已明确拆开：
    - `NPC own` 当前真正需要用户终验的项
    - `shared/UI` 观察项
  - 文档已写清：
    - 当前 exact-own 边界
    - 当前阶段是“线程自测已过，等待人工体验终验”
    - `002 / 003` 闭环与跑开中断的当前实现状态
    - 当前不能装成过线的体验项
    - 用户建议验收顺序
    - 长回执单与短回单模板
- 本轮关键判断：
  - `0.0.2` 当前最准确的阶段判断不再是“继续补状态机”，而是：
    - 底层闭环已经够厚
    - 接下来最需要的是用户按体验矩阵回单
  - 同时必须继续守住边界：
    - 左下角统一提示壳
    - 头顶箭头最终观感
    - `001` 正式提示卡审美
    当前都只能作为 `shared/UI` 观察项，不再混成 `NPC own`
- 当前恢复点：
  - 这条线现在已经有一份正式的用户终验入口。
  - 下一步不是继续扩题，而是等待用户按验收包回单；收到回单后，再只针对回单中仍属于 `NPC own` 的失败项继续开刀。

## 2026-04-01｜验收总包落盘后已主动 Park：当前不继续扩做，等待用户回单

- 当前 live 状态：
  - 已执行：
    - `Park-Slice`
  - 当前线程状态为：
    - `PARKED`
- 当前 blocker：
  - `等待用户按NPC验收总包回单；当前保持NPC底座协作位，不恢复shared prompt shell主刀`
- 当前恢复点：
  - 这条线后续恢复施工的前提已经改成：
    - 先收到用户回单
    - 再只针对回单里仍属于 `NPC own` 的失败项重新 `Begin-Slice`

## 2026-04-02｜用户带图追加体验反馈：已拆成 UI prompt + NPC 自省清单

- 当前主线目标：
  - 用户这轮不是让我直接继续写实现，而是先根据最新截图和体验反馈：
    - 给 `UI` 一份可直接转发的 prompt
    - 再把 `NPC` 自己没做好的点和下一轮必修项讲清楚
- 本轮新增被钉死的用户反馈：
  - 当前 NPC 头顶提示仍会和闲聊气泡遮挡，问题严重
  - 玩家 / NPC 对话气泡目前避让过头，距离太远
  - 箭头必须跟着说话方头顶，不应跟着气泡 body 乱跑
  - 自动跳过时间要改成硬公式：
    - `1.0s + 字数 * 0.08s`
  - 当前 NPC 气泡样式漂了，要回到用户最初认可的那版
  - NPC 出生点 / 行动路线需要重排，不能继续扎堆
  - 如果 NPC 互撞，要补 NPC-NPC 吵架 / 拌嘴内容包
  - 左下角提示若当前是 `Spring` 任务语义，不能再写“闲聊”，这条外包给 `UI`
- 本轮完成：
  - 已新增 UI prompt：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-02-NPC给UI的左下角任务提示接管委托-01.md`
  - 已新增自省与下一轮清单：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-02-NPC本轮自省与下一轮施工清单-01.md`
- 当前关键判断：
  - 这轮最新反馈说明：
    - 我前面把一部分结构问题先做通了
    - 但视觉归属、节奏、样式基线和 scene 反扎堆都还没守住
  - 因此 `0.0.2` 下一轮真正的主刀，不应再回到“解释逻辑”，而应转成：
    - 气泡观感回调
    - 节奏公式化
    - scene 层反扎堆
- 当前恢复点：
  - 这轮交付的重点已经不是继续实现，而是把：
    - shared/UI contract
    - NPC own 失败点
    - 下一轮优先级
    一次性钉清

## 2026-04-02｜本轮文档交付后已重新 Park，等待用户裁定下一刀

- 当前 live 状态：
  - 已执行：
    - `Park-Slice`
  - 当前状态：
    - `PARKED`
- 当前 blocker：
  - `等待用户决定：先转发UI prompt，还是直接让我按自省清单恢复NPC own施工`
- 当前恢复点：
  - 这轮已经把：
    - UI prompt
    - NPC 自省
    - 新的体验硬要求
    全部落盘
  - 下一步不再由我自行外推，等用户裁定先走哪条

## 2026-04-02｜NPC 体验全盘补口已继续落地，但 live 自测被外部编译红阻断

- 当前主线目标：
  - 按用户最新体验反馈直接收这条 `0.0.2`：
    - 头顶只保留小箭头
    - 左下角提示与可交互态严格同拍
    - 漫游气泡不再和交互提示抢位
    - 玩家/NPC 对话气泡只做适度避让
    - 自动续聊停顿改成 `1.0s + 字数 * 0.08s`
    - 跑开中断不再鬼畜
    - `Primary` 里的 `001 / 002 / 003` 不再编辑态扎堆
- 本轮真实施工：
  - 沿用已存在的 `thread-state ACTIVE` 切片：
    - `NPC非正式聊天体验全盘修复`
  - 已修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubblePresenter.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_002_VillageDaughterDialogueContent.asset`
    - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_003_ResearchDialogueContent.asset`
    - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
- 本轮完成：
  - `SpringDay1ProximityInteractionService`
    - 头顶世界提示改成只在 `CanTriggerNow` 时出现，和左下角提示重新同拍。
    - 当前焦点 NPC 进入可交互态时，会强压它自己的 ambient bubble，避免箭头 / 闲聊气泡互相遮挡。
  - `NPCBubblePresenter`
    - 新增 `ShowConversationImmediate / ShowReactionCueImmediate`
    - 新增 `SetInteractionPromptSuppressed`
    - ambient bubble 在交互提示接管时不会继续抢头顶。
  - `PlayerThoughtBubblePresenter`
    - 暴露 `ShowImmediateText`
    - 跑开中断时玩家离场句会直接实心显示，不再重新走透明淡入。
  - `PlayerNpcChatSessionService`
    - 跑开超距时每帧都会先快进当前打字/等待，再进入中断链，降低“首字鬼畜”概率。
    - `sessionBreakGraceSeconds` 调回更稳的 `0.2s`。
    - 自动续聊停顿现在按 `1.0s + 字数 * 0.08s` 走；`autoContinueDelayMax` 已改成 `0`，不再把长句硬截到 `1.8s`。
    - 对话气泡横向和纵向避让进一步收窄，只保留适度错位。
    - 跑开中断里的玩家句 / NPC 反应 / cue 改成直接实心接管，不再重新淡入。
  - `NPC_002 / NPC_003` 对话内容包
    - 旧的 `……` reaction cue 已改成 ASCII 版，避免字体缺字 warning。
  - `Primary`
    - 复核后确认 `001 / 002 / 003` 当前编辑态站位已回到各自 `HomeAnchor`：
      - `001 = (-7.15, 6.55)`
      - `002 = (-9.35, 6.00)`
      - `003 = (-5.45, 4.45)`
    - 当前 scene 读回不再是 `002 / 003` 扎在同一片。
- 本轮验证：
  - `validate_script` 通过：
    - `NPCBubblePresenter.cs`
    - `PlayerThoughtBubblePresenter.cs`
    - `PlayerNpcChatSessionService.cs`
    - `SpringDay1ProximityInteractionService.cs`
  - `git diff --check` 通过：
    - 上述 own 代码 + `Primary.unity` + 本轮 NPC 数据资产
  - MCP 基线中途掉过一次：
    - `listener_missing`
    - 已通过 `Library/MCPForUnity/TerminalScripts/mcp-terminal.cmd` 拉回 `8888`
    - `check-unity-mcp-baseline.ps1` 已重新 `pass`
- 当前 blocker：
  - Unity 当前无法进入本轮 `Play` 自测，不是因为本轮 own 文件编译红，而是外部 compile blocker：
    - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
      - `TickStatusBarFade`
      - `ApplyStatusBarAlpha`
      缺失
    - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
      - `TickStatusBarFade`
      - `ApplyStatusBarAlpha`
      缺失
  - `Library/CodexEditorCommands/status.json` 一直报：
    - `isCompiling=true`
    - `lastCommand=waiting-editor-busy`
  - 因此这轮没法继续跑：
    - `002 / 003 closure`
    - `002 / 003 interrupt`
    - 头顶箭头 / 左下角提示 / 气泡距离的 fresh Play 复看
- 当前 live 状态：
  - 已执行：
    - `Park-Slice`
  - 当前状态：
    - `PARKED`
- 当前恢复点：
  - 等外部 compile blocker 清掉后，下一步不要再回到大改代码，直接做：
    - `002 closure`
    - `002 interrupt`
    - `003 closure`
    - `003 interrupt`
    - `001` 任务优先左下角提示复看
    - 头顶箭头与闲聊气泡是否已经不再重叠的最终画面验收

## 2026-04-02｜跑开中断链二次修正后已拿到 `closure / interrupt / player-typing interrupt` 全绿自验

- 当前主线目标：
  - 把用户反复点名的这组体验 blocker 真正压实：
    - 跑开后玩家气泡鬼畜
    - 玩家气泡卡首字/半透明残留
    - NPC 留在等待态
    - 正常对话在超距后还自己往下滚
- 本轮真实代码触点只认 3 个：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubblePresenter.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerThoughtBubblePresenterStyleTests.cs`
- 本轮完成：
  - `PlayerNpcChatSessionService.cs`
    - 把超距时的快进收窄成“只跳完当前正在打字的句子”，不再推进 `WaitingNpcReply / AutoContinuing`，避免玩家跑开后正常对话还继续自己往下走。
    - 把双气泡避让压回更轻的区间，避免 body 偏移过重。
  - `NPCBubblePresenter.cs`
    - 把 NPC 气泡边框色恢复回更早认可的暖金风格，不再沿错误 runtime 样式继续漂。
  - `PlayerThoughtBubblePresenterStyleTests.cs`
    - 改成反射版测试，避免 `Tests.Editor` 直接绑默认运行时程序集类型。
    - 顺手收掉 `Object` 二义性，保证这条护栏不再反咬编译面。
- 本轮真实自验：
  - 通过 `NPCInformalChatValidationMenu` + `CodexEditorCommandBridge` 拿到 6 条硬证据：
    - `002 closure -> endReason=Completed`
    - `003 closure -> endReason=Completed`
    - `002 interrupt -> endReason=WalkAwayInterrupt`
    - `003 interrupt -> endReason=WalkAwayInterrupt`
    - `002 player-typing interrupt -> leavePhase=PlayerSpeaking + WalkAwayInterrupt`
    - `003 player-typing interrupt -> leavePhase=PlayerSpeaking + WalkAwayInterrupt`
  - 期间有一次 Play 自动掉回 Edit，导致一轮 trace 出现 `menu-fail`；重新 `PLAY` 后补跑成功，不再把这类现场波动误判成逻辑失败。
- 当前判断：
  - `0.0.2` 这条线现在最准确的状态是：
    - 结构成立
    - targeted probe 已全绿
    - 真实观感仍待用户终验
  - 也就是说，这轮可以诚实 claim：
    - “跑开后继续自动滚对话”的主 bug 已经在 own service 层被压掉
  - 但还不能偷换成：
    - “NPC 非正式聊天体验已经全部过线”
- 当前边界：
  - 这轮没有继续重开：
    - `Primary.unity`
    - `GameInputManager.cs`
    - 导航 runtime
    - 字体资产
    - shared prompt shell / UI 玩家面整合
- 当前恢复点：
  - 代码与自测证据已经补齐。
  - 本轮收尾只差审计层落盘与重新 `Park-Slice`。
  - 如果后续继续恢复 `0.0.2`，优先看用户真实观感回单里仍属于 `NPC own` 的失败项，不再回到 shared/UI 壳主刀。

## 2026-04-02｜`0.0.2` 审计尾注：memory 已补齐并重新 `PARKED`

- 本轮收尾动作：
  - 已执行：
    - `Begin-Slice`（收尾落盘专用）
    - `Park-Slice`
  - 当前 `thread-state`：
    - `PARKED`
- 审计结果：
  - `skill-trigger-log.md` 已补记：
    - `STL-20260402-036`
  - `check-skill-trigger-log-health.ps1` 返回：
    - `Canonical-Duplicate-Groups = 1`
    - 现存 canonical 旧重号为：
      - `STL-20260402-029`
  - 这不是本轮新引入的重号，但意味着当前全局审计层不能 claim 完全 clean。
- 当前恢复点：
  - `0.0.2` 本线自己的代码自验、工作区记忆和线程记忆都已补齐。
  - 后续继续仍只看用户终验里属于 `NPC own` 的失败项。

## 2026-04-03｜春一日新 NPC 群像：`NPC-v` 本体层 preflight 已完成，8 份 content 空壳已被拉正

- 当前主线目标：
  - 不是继续补 `0.0.2` 旧闲聊体验，而是按新联合完工 prompt，先把“春一日新 NPC 群像”的 `NPC-v` 本体层收实：
    - 8 套 sprite / anim / prefab / roam / content / manifest 是否齐
    - prefab 本体是否真能聊、真能跑
    - 哪些问题仍属于 `NPC-v own`
- 本轮只认的 `NPC-v` own 范围：
  - `Assets/Sprites/NPC`
  - `Assets/100_Anim/NPC`
  - `Assets/111_Data/NPC/SpringDay1Crowd`
  - `Assets/222_Prefabs/NPC`
  - `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
- 本轮 preflight 结论：
  - 8 张 sprite：齐
  - 8 套动画目录：齐；每个目录当前都有 `1` 个 controller 和 `6` 个 clip
  - 8 个 prefab：齐
  - 8 份 roam profile：齐
  - `SpringDay1NpcCrowdManifest.asset`：齐；当前有 `8` 条 entry
  - 8 个 crowd prefab 本体层挂载也齐：
    - 全部存在 `NPCAutoRoamController`
    - 全部存在 `NPCInformalChatInteractable`
    - 全部存在 `roamProfile` 引用
    - 全部存在 `homeAnchor` 引用
- 本轮第一真实 blocker：
  - 8 份 `Assets/111_Data/NPC/SpringDay1Crowd/*DialogueContent.asset` 在开工前全是空壳：
    - `npcId` 为空
    - `defaultInformalConversationBundles` 为空
    - `playerNearbyLines` 为空
    - `defaultWalkAwayReaction.reactionCue` 为空
    - `defaultChatInitiatorLines / defaultChatResponderLines` 为空
  - 这意味着 prefab 虽然挂了 `NPCInformalChatInteractable`，但本体层“可聊”其实不成立。
- 本轮修复：
  - 已修：
    - `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
  - 根因不是 spec 没写，而是 bootstrap 原先用 `EditorJsonUtility.FromJsonOverwrite(...)` 没把 `NPCDialogueContentProfile` 的私有序列化字段真正写进去。
  - 现在改成：
    - 用实际 `NPCDialogueContentProfile` 嵌套类型逐层构造并回填私有字段
    - 然后通过已开着的 Unity 实例重跑：
      - `Tools/NPC/Spring Day1/Bootstrap New Cast`
  - 顺手把 `101` 的 walk-away cue 从 `……` 改成 ASCII `...`，避免重吃旧的字体告警坑。
- 本轮修后复核：
  - 8 份 `DialogueContent.asset` 现在都已具备：
    - `npcId`
    - `playerNearbyLines`
    - `defaultInformalConversationBundles`
    - `defaultWalkAwayReaction`
    - `defaultChatInitiatorLines`
    - `defaultChatResponderLines`
  - 当前可以诚实 claim：
    - 这 8 人在 `NPC-v` 本体层已经不再是“有 prefab 但 content 空壳”的假成立状态
- 当前仍未闭完的 `NPC-v` 项：
  - 8 份 crowd `DialogueContent.asset` 的 `pairDialogueSets` 仍然都是空数组
  - 也就是说：
    - 单人闲聊 / 玩家靠近 / walk-away / ambient 默认语义已成立
    - 但 pair-specific 语义还没真正 authored
- 当前恢复点：
  - `NPC-v` 本体层当前最硬的空壳问题已经补掉。
  - 下一步如果继续由我做，只应围绕：
    - pair-specific 内容是否要补
    - 8 人 runtime 下各自 informal chat / roam / facing 是否逐个 live 过
  - `StoryPhase` 消费、`Primary` phase 拉起与 Day1 时序判断，继续交给 integration 侧。

## 2026-04-03｜春一日新 NPC 群像：Day1 integration 侧已过 `Ready-To-Sync`

- 当前主线目标：
  - 按 `2026-04-03-本线程_春一日新NPC群像Day1整合并行任务单-03.md`，只做 `spring-day1` / `Day1 integration` 侧的 crowd 接入闭环：
    - 不重做 `NPC-v` 的 prefab / dialogue / roam 生产链
    - 不碰 `GameInputManager.cs`、UI 主线、字体主线或 `Primary.unity`
- 本轮只认的 own 范围：
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdManifest.cs`
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
- 本轮完成：
  - `SpringDay1NpcCrowdDirector.cs`
    - 新增 `CurrentRuntimeSummary`
    - `FindAnchor()` 改成优先解析稳定 home-anchor：
      - `001 -> 001_HomeAnchor`
      - `NPC001 -> 001_HomeAnchor`
      - `002/003` 同理
    - `PruneDestroyedStates()` 加强，避免 runtime 留半残 state
  - `SpringDay1Director.cs`
    - live snapshot 新增：
      - `Crowd`
      - `WorldHint`
      - `PlayerFacing`
    - 为了切断 slice 外硬编译耦合，把下面三处改成反射/兜底桥接：
      - `SpringDay1ProximityInteractionService.CurrentFocusSummary`
      - `SpringDay1WorldHintBubble` 的玩家面摘要字段
      - `SpringDay1BedInteractable.GetBoundaryDistance`
    - 同时补上 `StoryManager == null` 的空保护，避免 Day1 导演在未初始化窗口直接炸空引用
  - `SpringDay1Director.cs` 已补 `using System;`，消除了 `AppDomain / Type` 这组编译错误
  - `Ready-To-Sync` 过程中发现 `.kiro/state/ready-to-sync.lock` 是上次超时留下的 stale 锁：
    - 没有碰业务根
    - 只把 stale 锁改名让位后重跑
    - 当前 `Ready-To-Sync` 已通过
- 本轮稳定证据：
  - manifest 当前 8 人都能解析到稳定锚点，无 unresolved：
    - `101 -> 001_HomeAnchor`
    - `102 -> 003_HomeAnchor`
    - `103 -> 001_HomeAnchor`
    - `104 -> 001_HomeAnchor`
    - `201 -> 002_HomeAnchor`
    - `202 -> 002_HomeAnchor`
    - `203 -> 001_HomeAnchor`
    - `301 -> 003_HomeAnchor`
  - 已确认旧逻辑会按 `001/002/003` 这些 NPC 当下位置刷新 crowd，确实会把新群像刷偏；新逻辑改回 home-anchor 后，出生点差异是实质修复，不是空整理
  - 例子：
    - `102`
      - old：`003 -> (14.02, 2.68)`
      - new：`003_HomeAnchor -> (3.85, -1.08)`
    - `201`
      - old：`002 -> (10.39, 2.73)`
      - new：`002_HomeAnchor -> (0.77, 1.29)`
    - `301`
      - old：`003 -> (9.47, 0.73)`
      - new：`003_HomeAnchor -> (-0.70, -3.03)`
- 本轮验证：
  - `git diff --check` 已过
  - `Ready-To-Sync.ps1 -ThreadName spring-day1V2` 已过，返回：
    - `status = READY_TO_SYNC`
  - 当前 `thread-state`：
    - `Begin-Slice` 已执行
    - `Ready-To-Sync` 已执行并通过
    - 尚未 `Park-Slice`，因为正准备白名单 sync
- 当前恢复点：
  - 这条 Day1 integration slice 已不再卡编译闸门，也不再卡 thread-state preflight。
  - 下一步只剩：
    - 补完记忆/审计
    - 按白名单 sync
    - 然后把线程状态收回到非活跃态

## 2026-04-03｜NPC-v 本体层补强：验证菜单已扩到 sprite/anim/prefab/data/pair 全链，`002 StuckCancel` 更像 scene 旧锚点问题

- 当前主线目标：
  - 继续只做“春一日新 NPC 群像”的 `NPC-v` 本体层，不回漂到 Day1 integration、`Primary.unity` 大改或 shared UI。
- 本轮实际做成了什么：
  - 已继续修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPC\SpringDay1NpcCrowdValidationMenu.cs`
  - 当前验证菜单不再只查 `dialogue / roam / prefab / manifest` 是否存在，而是把以下护栏收进同一条 preflight：
    - `Assets/Sprites/NPC/{id}.png` 是否存在
    - `Assets/100_Anim/NPC/{id}/Controller/*` 是否存在且唯一
    - `Assets/100_Anim/NPC/{id}/Clips/*` 是否正好 6 条
    - prefab 是否挂了：
      - `SpriteRenderer`
      - `Animator`
      - `NPCAnimController`
      - `NPCMotionController`
      - `NPCAutoRoamController`
      - `NPCInformalChatInteractable`
      - `NPCBubblePresenter`
    - prefab 引用的 sprite / controller 是否和该 NPC 自己的产物一致
    - `pairDialogueSets` 不只非空，还会检查：
      - `partnerNpcId` 合法
      - 无重复 / 无自指
      - `initiatorLines / responderLines` 非空
      - 配对是否有反向 reciprocal link
  - 已通过当前 Unity 实例再次执行：
    - `Tools/NPC/Spring Day1/Validate New Cast`
  - 当前硬证据：
    - `Library/CodexEditorCommands/status.json`
      - `success = true`
      - `lastCommand = menu:Tools/NPC/Spring Day1/Validate New Cast`
    - `Editor.log`
      - `[SpringDay1NpcCrowdValidation] PASS | npcCount=8 | totalPairLinks=16 | roots=Assets/111_Data/NPC/SpringDay1Crowd / Assets/222_Prefabs/NPC / Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
- 本轮只读追加判断：
  - 用户刚贴的 `002 roam interrupted => StuckCancel` 没有被我偷接成当前 `NPC-v` own 施工。
  - 只读核对 `Assets/000_Scenes/Primary.unity` 后，旧 `001 / 002 / 003` 与各自 `HomeAnchor` 的 scene 现场为：
    - `001`
      - npc = `(-7.206, 5.094)`
      - anchor = `(-6.190, 6.290)`
      - delta = `(1.016, 1.196)`
    - `002`
      - npc = `(0.890, 7.594)`
      - anchor = `(-8.730, 6.150)`
      - delta = `(-9.620, -1.444)`
    - `003`
      - npc = `(3.816, 7.593)`
      - anchor = `(-6.350, 3.830)`
      - delta = `(-10.166, -3.763)`
  - 且 scene 里当前仍明确绑着：
    - `001 -> homeAnchor {fileID: 910010002}`
    - `002 -> homeAnchor {fileID: 920020002}`
    - `003 -> homeAnchor {fileID: 930030002}`
- 当前关键判断：
  - `002 StuckCancel` 现在更像 legacy scene 里“本体位置和 homeAnchor 分离”的旧账，不像我这轮 `NPC-v` 的 crowd pair/data 补口又把 roam 逻辑写坏了。
  - 而且这不只 `002` 单点成立：
    - `003` 同类漂移更严重
    - `001` 也有轻度偏离
- 当前恢复点：
  - `NPC-v` 这轮 own 层当前已更接近“产物链和 preflight 都成立”的状态。
  - 如果下一刀继续由我在 `NPC-v` 内施工，优先级应是：
    - 新 8 人 runtime 的 roam/chat/facing targeted probe
    - 而不是去吞旧 `001 / 002 / 003` 的 scene 位置善后
  - `002 / 003` 的 `HomeAnchor` 漂移应作为 `Primary / scene / Day1` 侧 blocker 单独交接，不再混进本轮 own 完成定义。

## 2026-04-03｜收盘补记：Day1 integration slice 已提交并回到 `PARKED`

- 本轮白名单 sync 已完成：
  - 提交：`03c0bf87`
  - 分支：`main`
  - push：已成功
- 当前 `thread-state`：
  - 已执行 `Park-Slice`
  - 当前状态：`PARKED`
- 当前恢复点：
  - 这条 slice 的代码、记忆和白名单提交都已闭环。
  - 后续如再被叫回，只需要从 `03c0bf87` 之后继续新的 Day1 integration 需求，不需要回头补本轮尾账。

## 2026-04-03｜新群像总线再厘清：旧验收文档退为背景，下一步收束成两段 runtime 闭环

- 当前主线目标：
  - 用户要求基于 4 月 1 日到 4 月 3 日的 5 份 NPC 文档和 `NPC-v` 最新回执，一次说清当前到底该继续什么，并交付两份下一轮可直接发的 prompt。
- 本轮子任务：
  - 只读复核：
    - `2026-04-01-NPC当前阶段用户验收总包-01.md`
    - `2026-04-02-NPC本轮自省与下一轮施工清单-01.md`
    - `2026-04-02-NPC给UI的左下角任务提示接管委托-01.md`
    - `2026-04-03-NPC-v_春一日新NPC群像联合完工续工prompt-02.md`
    - `2026-04-03-本线程_春一日新NPC群像Day1整合并行任务单-03.md`
  - 同时对照 `NPC-v` 最新回执、`NPC.json`、`spring-day1V2.json` 和两份新 prompt 草稿，不进入真实施工。
- 本轮稳定结论：
  - `2026-04-01` 的验收总包与 `2026-04-02` 的 UI/体验自省文档，描述的是旧 `002 / 003` 非正式聊天体验线；这些文档仍可当背景，但已经不是当前唯一主线。
  - 当前真正的主线已经收束为：
    - 春一日新 8 人群像要在 Day1 里真正活起来。
  - `NPC-v` 最新回执已经把当前阶段钉实为：
    - 工具级 preflight 已站住
    - `Validate New Cast = PASS`
    - `npcCount=8`
    - `totalPairLinks=16`
    - 但 runtime targeted probe 还没开始跑
    - 当前仍 `PARKED`
    - 当前 own dirty / untracked 仍未归仓
  - 我这边的 Day1 integration 则已经进主分支：
    - `03c0bf87`
    - `35144958`
    - 当前 `spring-day1V2` 也处于 `PARKED`
  - 因此现在不再差“结构设计”，而是只差两段运行态证据。
- 本轮新增产物：
  - 已确认并保留两份最新 prompt：
    - `2026-04-03-NPC-v_春一日新NPC群像运行态probe与本体归仓续工prompt-03.md`
    - `2026-04-03-本线程_春一日新NPC群像Day1剧情消费probe任务单-04.md`
- 当前恢复点：
  - 下一步只剩两段 runtime 闭环：
    1. `NPC-v`
       - 新 8 人 `instance / informal chat / pair dialogue / walk-away interrupt` targeted probe
       - 做完后把 own dirty / memory 一起归仓
    2. `spring-day1V2`
       - 用 Day1 live/snapshot 入口拿 `CrashAndMeet -> DayEnd` 的 crowd phase 消费矩阵
       - 必要时只在 own 范围补最小 probe 字段
  - 旧 `001 / 002 / 003` 的 `HomeAnchor` 漂移继续只算 legacy `Primary / scene / Day1` 背景风险，不回灌成这轮新 8 人 own 完成定义。

## 2026-04-03｜Day1 剧情消费 probe 首次续跑：own 最小 probe 已补，但 runtime 被 `NPC-v` compile red 挡住

- 当前主线目标：
  - 只做 `spring-day1` 这侧的 Day1 `phase/runtime consumption probe`，拿 `CrashAndMeet -> DayEnd` 的 crowd 消费矩阵，不回吞 `NPC-v` 本体层。
- 本轮真实施工：
  - 已执行：
    - `Begin-Slice`
  - 当前切片：
    - `春一日新NPC群像-Day1剧情消费probe`
  - 本轮只改：
    - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
- 本轮完成：
  - `SpringDay1NpcCrowdDirector.CurrentRuntimeSummary` 已补最小 probe 字段，不再只剩 `on/off`：
    - 新增 active 列表
    - 新增每个 runtime state 的 `anchorName`
    - 新增 `fallback=0/1`
    - 新增当前位置 `pos=x/y`
  - 这次补口仍严格留在 own 范围内，没有新建 editor 工具，也没有回写 scene / UI / 字体 / `GameInputManager.cs`
- 本轮第一真实 blocker：
  - Unity 当前无法进入这刀的 runtime probe，不是因为我这轮 own 代码红，而是 `NPC-v` 当前 own 文件：
    - `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
    仍有一组 `CS1061`
  - 具体表现是：
    - `NPCDialogueContentProfile.PairDialogueSet` 上访问了不存在的成员
    - Unity 一直停在 `isCompiling = true`
    - `CodexEditorCommandBridge` 也因此卡在：
      - `lastCommand = menu:Assets/Refresh`
  - 结果是：
    - `Play -> Bootstrap -> Snapshot / Step`
      这条现成 Day1 验收入口本轮还没能真正跑起来
- 当前恢复点：
  - 这轮已经把 own 侧“快照不够判 anchor/fallback”的缺口补掉。
  - 但 `CrashAndMeet -> DayEnd` 的实际运行态矩阵仍未拿到；下一步必须先让 Unity 退出这条 `NPC-v` compile red，再继续 Day1 probe。
  - 已执行：
    - `Park-Slice`
  - 当前状态：
    - `PARKED`

## 2026-04-03｜新 8 人 runtime targeted probe 已做实：pair 问题压成 `ambient bubble` 未点亮

- 当前主线目标：
  - 只做 `NPC-v` 本体层 runtime targeted probe，并把本轮 own dirty / memory 一起归仓。
- 本轮真实施工：
  - 已执行：
    - `Begin-Slice`
  - 本轮只改：
    - `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
  - Unity 内已真实完成：
    - `Tools/NPC/Spring Day1/Validate New Cast`
      - `PASS | npcCount=8 | totalPairLinks=16`
    - `Tools/NPC/Spring Day1/Run Runtime Targeted Probe`
      - 多轮复跑后结论一致
- 当前 runtime probe 稳定结果：
  - `8/8 instance 基础运行态`
    - PASS
  - `8/8 informal chat`
    - PASS
  - `2 组 pair dialogue`
    - FAIL
    - 失败组：
      - `101 <-> 103`
      - `201 <-> 202`
  - `2 个 walk-away interrupt`
    - PASS
- 当前 pair 失败已压窄到的可见小点：
  - `initiatorDecision` / `responderDecision` 都已成立：
    - `chatting with ...`
    - `joined chat with ...`
  - pair 台词数组在运行时解析正常：
    - `count = 2`
    - 首句可直接读到
  - 但同时：
    - `NPCBubblePresenter.visible = false`
    - `suppressed = false`
    - `channel = Ambient`
    - `conversationOwner = none`
  - 结论：
    - 不是 pair 数据缺失
    - 不是提示压制
    - 不是会话残留
    - 而是 `ambient pair decision -> bubble emission` 这段 own runtime 链没闭上
- 本轮现场 blocker / 噪声：
  - Unity 多次弹出：
    - `打开场景已在外部被修改。`
  - 这会直接卡死 `CodexEditorCommandBridge`。
  - 本轮处理口径：
    - 统一点 `忽略`
    - 不把外部 scene 变更吞进当前 probe 会话
  - 因此这个只算 shared live 环境噪声，不改写当前 pair 失败归因。
- 当前恢复点：
  - 下一刀如果继续 `NPC-v`，最该直接看：
    - `NPCAutoRoamController.TryStartAmbientChat`
    - `StartAmbientChatRoutine`
    - `PlayAmbientChatBubble`
    - `NPCBubblePresenter.ShowText`
  - 本轮在真正收口前，还需要：
    - 更新 memory
    - 跑 `Ready-To-Sync`
    - 判断 own 路径是否可真实归仓；如果不行，只报第一真实 blocker

## 2026-04-03｜本轮收口结论：probe 已完成，但 `Ready-To-Sync` 被 NPC 历史 own roots 挡住

- 本轮真实收口动作：
  - 已执行：
    - `Ready-To-Sync.ps1 -ThreadName NPC`
  - 返回：
    - `status = BLOCKED`
  - 随后已执行：
    - `Park-Slice.ps1 -ThreadName NPC`
  - 当前 live 状态：
    - `PARKED`
- 当前第一真实 blocker：
  - `NPC` own roots 仍有历史残包未纳入本轮
  - 已明确报实至少包括：
    - `M Assets/Editor/NPC/NPCInformalChatValidationMenu.cs`
    - `?? .kiro/specs/NPC/2.0.0进一步落地/0.0.2清盘002/2026-04-03-NPC-v_春一日新NPC群像运行态probe与本体归仓续工prompt-03.md`
    - `?? .kiro/specs/NPC/2.0.0进一步落地/0.0.2清盘002/2026-04-03-本线程_春一日新NPC群像Day1剧情消费probe任务单-04.md`
    - `?? Assets/Editor/NPC/CodexEditorCommandBridge.cs`
    - `?? Assets/Editor/NPC/CodexEditorCommandBridge.cs.meta`
- 本轮最终判断：
  - 运行态 probe 结论已经有效：
    - `8/8 instance`
    - `8/8 informal`
    - `0/2 pair`
    - `2/2 walk-away`
  - 但本轮 own 路径还不能 claim clean；
  - 不是因为 probe 没跑，而是因为 NPC 历史 own roots 尾账没收完。

## 2026-04-03｜收到 NPC 最新回执后的总线重判：新 8 人本体已大半站住，当前只剩 pair 与 Day1 消费未闭

- 当前主线目标：
  - 基于 `NPC-v` 最新 runtime targeted probe 回执，重新回答用户“这 8 个人现在到底是什么情况、能验到什么、还差什么”。
- 本轮稳定结论：
  - `NPC-v` 这边的新 8 人 runtime probe 已真实完成：
    - `8/8 instance` 通过
    - `8/8 informal` 通过
    - `2/2 walk-away interrupt` 通过
    - `2/2 pair dialogue` 未通过
  - pair 失败现在已经被压窄到 `NPC-v own`：
    - ambient pair 决策成立
    - pair 台词解析成立
    - 但 `NPCBubblePresenter` 没真正亮起来
  - 因此当前最新判断应更新为：
    - 这不再是 `spring-day1` / Day1 integration 当前主 blocker
    - Day1 integration 不需要为了 pair 失败回改 phase 消费
  - 同时 shared live 现场还有偶发：
    - “打开场景已在外部被修改”
    弹窗
    - 这被明确归类为 shared 噪声，不是当前 pair 根因
- 当前恢复点：
  - 新群像总线现在最准确的剩余项已经收束成两块：
    1. `NPC-v own`
       - ambient pair bubble emission
       - own roots 历史残包清扫 / 归仓
    2. `spring-day1V2`
       - `CrashAndMeet -> DayEnd` 的 Day1 crowd phase/runtime consumption 矩阵
  - 用户现在已经可以验“单体出现、单体闲聊、跑开收尾”这三块；
  - 但“成对群像真的说起来”和“Day1 各阶段真实消费矩阵”仍未闭环。

## 2026-04-03｜原剧本核实补记：当前 `101~301` 不能再直接 claim 为 Day1 已确认正式角色

- 当前主线目标：
  - 在继续推进“春一日新 NPC 群像”前，先核实原剧本里真正已设计的人物，并把 `NPC-v` 与 `spring-day1V2` 的下一轮 prompt 改回基于原案的切片。
- 本轮新查实的事实源：
  - 用户原始 Day1 剧情原文可直接追溯到：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\1.0.0策划\002_事件编排重构\Deepseek聊天记录001.md`
  - Day1 固化稿可追溯到：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\0.0阶段\0.0.1剧情初稿\春1日_坠落_融合版.md`
  - 长线核心 NPC 表可追溯到：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\1.0.0策划\001_剧本篇章集\000_Deepseek-2\Deepseek-2-P1.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\1.0.0策划\001_剧本篇章集\000_Deepseek-2\省流版VIP8.md`
- 已明确的原案角色链：
  - `马库斯`
  - `艾拉`
  - `卡尔 / 研究儿子`
  - `老杰克`
  - `老乔治 / 老铁匠`
  - `老汤姆`
  - `小米`
  - `围观村民 / 饭馆村民 / 小孩`
- 当前工程里已存在的老 Day1 角色底座：
  - `Assets/111_Data/NPC/NPC_001_VillageChief*`
  - `Assets/111_Data/NPC/NPC_002_VillageDaughter*`
  - `Assets/111_Data/NPC/NPC_003_Research*`
- 因此当前判断更新为：
  - `101~301` 这批新增 crowd 槽位只能先视为“当前实现现场”
  - 不能继续默认当成“用户原剧本已经确认过的 Day1 正式人物表”
  - 其中 `LedgerScribe / Seamstress / Florist / GraveWardenBone` 这类语义目前没有在原 Day1 剧本里找到直接来源
- 本轮新增产出：
  - `2026-04-03-NPC-v_春一日新NPC群像原剧本人设核实与NPC本体映射回正prompt-04.md`
  - `2026-04-03-本线程_春一日原剧本角色消费矩阵与群像整合回正任务单-05.md`
- 当前恢复点：
  - 下一轮 `NPC-v` 不再先修 pair / 扩对白，而是先做：
    - 原案角色 -> 当前 `101~301` 槽位映射
    - 能回正的最小命名 / 文案 / 角色摘要回正
  - 下一轮 `spring-day1V2` 不再先 claim 群像已扩完，而是先做：
    - 原 Day1 角色消费矩阵
    - 老 `NPC001/002/003` 与新 `101~301` 的承载分层判断

## 2026-04-03｜收口补记：原剧本核实双 prompt 已本地提交，远端推送被代理网络阻断

- 本轮白名单 sync 结果：
  - `Ready-To-Sync.ps1 -ThreadName spring-day1V2`
    已通过，返回：
    - `READY_TO_SYNC`
  - 随后白名单 `sync` 已在本地成功创建提交：
    - `e4ef0ad4`
    - `2026.04.03_spring-day1V2_03`
- 本轮最终 blocker：
  - 不是白名单范围错误
  - 不是文档自身冲突
  - 而是 `git push` 走到：
    - `127.0.0.1:7897`
    代理时连接失败
  - 因此当前状态应判为：
    - 本地提交已完成
    - 远端未同步
- 当前恢复点：
  - 当前仓库相对 upstream 为：
    - `ahead 1`
  - 线程已执行：
    - `Park-Slice.ps1 -ThreadName spring-day1V2`
  - 当前 live 状态：
    - `PARKED`

## 2026-04-03｜补记：新 8 人当前“看不见”的直接原因已钉实，probe 位置不等于正式 Day1 演出

- 当前主线目标：
  - 回答用户“这些新 NPC 到底在哪测、为什么在场景里没看到、下一轮我和 NPC-v 应该怎么分工”。
- 本轮新查实的运行态事实：
  - `NPC-v` 之前拿到的 `8/8 instance / 8/8 informal / 2/2 walk-away` 通过，主要来自：
    - `Tools/NPC/Spring Day1/Run Runtime Targeted Probe`
  - 这条 probe 不是把人正式摆进 Day1 村里给玩家消费；
  - 它会在 Play Mode 下临时创建：
    - `__SpringDay1NpcCrowdRuntimeProbeRoot`
  - 并把 `Probe_101 ~ Probe_301` 生在：
    - 约 `x=120 / y=120` 一带的隔离测试区
  - 所以用户在正常 `Primary` 体验里“没看到他们”，不是错觉。
- 同时查实的正式消费链：
  - 真正负责把 crowd 生进 `Primary` 的是：
    - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
  - 它只会在：
    - `Primary`
    - 且故事 phase 落在各自 `minPhase ~ maxPhase`
    时按 manifest 动态生成
  - 但这条正式消费链目前还没有拿到“已对齐原剧本”的整体验收结论。
- 当前总线判断更新为：
  - 新 8 人现在已经有：
    - prefab / data / manifest / targeted runtime probe
  - 但还没有完成：
    - 原剧本人设映射回正
    - Day1 phase 消费矩阵闭环
    - 玩家面正式演出验收
- 当前恢复点：
  - `NPC-v` 下一轮只做：
    - 原案角色 -> `101~301` 槽位映射
    - 如证据足够，最小命名 / 文案 / roleSummary 回正
  - `spring-day1V2` 下一轮只做：
    - `CrashAndMeet -> DayEnd` 的原 Day1 角色消费矩阵
    - 老 `NPC001/002/003` 与新 `101~301` 的承载分层
  - 本轮额外已执行：
    - `Park-Slice.ps1 -ThreadName spring-day1V2`
  - 原因：
    - 现场此前仍停在 `READY_TO_SYNC`，与当前实际“解释现状并停车待下一轮”不符，现已纠正回 `PARKED`

## 2026-04-03｜原 Day1 角色消费矩阵与群像承载分层判定：当前 101~301 仍只是 crowd 槽位，不等于原剧本已扩完

- 当前主线目标：
  - 按 `2026-04-03-本线程_春一日原剧本角色消费矩阵与群像整合回正任务单-05.md`
    先做原 Day1 角色消费矩阵，再判老 `NPC001/002/003` 与新 `101~301` 的承载分层。
- 本轮查实的原 Day1 角色消费矩阵：
  - `CrashAndMeet`
    - 原剧本必须角色：
      - `马库斯`
    - 原剧本群众层：
      - 无
  - `EnterVillage`
    - 原剧本必须角色：
      - `马库斯`
    - 原剧本群众层：
      - `围观村民 / 小孩`
  - `HealingAndHP`
    - 原剧本必须角色：
      - `马库斯`
      - `艾拉`
    - 原剧本群众层：
      - 无
  - `WorkbenchFlashback`
    - 原剧本必须角色：
      - `马库斯`
      - `艾拉`
    - 原剧本群众层：
      - 无
    - 备注：
      - `老铁匠 / 老乔治` 在这里是“村里只有他会做这些”的 lore，不是 Day1 当场必须出镜角色
  - `FarmingTutorial`
    - 原剧本必须角色：
      - `马库斯`
    - 原剧本群众层：
      - 可有背景村民，但不要求具名
  - `DinnerConflict`
    - 原剧本必须角色：
      - `马库斯`
      - `卡尔 / 研究儿子`
    - 原剧本群众层：
      - `饭馆村民`
  - `ReturnAndReminder`
    - 原剧本必须角色：
      - `马库斯`
    - 原剧本群众层：
      - 无
  - `FreeTime`
    - 原剧本必须角色：
      - 无硬性必须角色
    - 原剧本群众层：
      - 可接触 `老杰克 / 老乔治 / 老汤姆 / 小米 / 未具名村民`
  - `DayEnd`
    - 原剧本必须角色：
      - 无
    - 原剧本群众层：
      - 无
- 本轮查实的现有工程承载矩阵：
  - 老 Day1 主角色链：
    - `NPC001 / VillageChief`
      - 当前最稳的 `马库斯` 承载壳
      - `CrashAndMeet -> EnterVillage` 的正式入口仍直接依赖它
    - `NPC002 / VillageDaughter`
      - 当前最接近 `艾拉` 的承载壳
      - 但 `HealingAndHP` 当前主要由导演层直接播 `艾拉` 序列，还没有把 runtime 演出明确绑回 `NPC002`
    - `NPC003 / Research`
      - 当前最接近 `卡尔 / 研究儿子` 的语义壳
      - 但 Day1 主线里的 `卡尔` 目前主要只作为晚餐序列 speaker 出现，还不是稳定的 `NPC003` runtime 演出
  - 当前 crowd 新 8 人：
    - `101 莱札 / LedgerScribe`
      - 原案无直接来源
      - 当前被放到 `EnterVillage -> DayEnd`
      - 不应继续 claim 为正式角色
    - `102 炎栎 / Hunter`
      - 只与“大儿子在外打猎 / 猎户氛围”语义邻近
      - 原 Day1 没有稳定在场角色依据
    - `103 阿澈 / ErrandBoy`
      - 最接近 `小孩 / 跑腿 / 围观少年` 这一类群众语义
      - 可以保留为群众层，不应转正
    - `104 沈斧 / Carpenter`
      - 只能算村庄修缮氛围角色
      - 不能替代 `老铁匠 / 老乔治`
    - `201 白槿 / Seamstress`
      - 原案无直接来源
      - 被放到 `HealingAndHP -> DayEnd`，过早进入主链
    - `202 桃羽 / Florist`
      - 原案无直接来源
      - 最多保留为一般群众层
    - `203 麦禾 / CanteenKeeper`
      - 最接近 `饭馆村民 / 饭馆背景角色`
      - 可以保留为晚餐背景 crowd，但不是原案核心角色
    - `301 朽钟 / GraveWardenBone`
      - 原案无直接来源
      - 与当前 Day1 主线气质冲突最大
      - 不应继续 claim 为正式角色
  - 尚无稳定承载的原案角色：
    - `老杰克`
    - `老乔治 / 老铁匠`
    - `老汤姆`
    - `小米`
    - `卡尔` 的稳定 runtime 资产承载
- 当前判断：
  - `101~301` 这批内容仍然只能算：
    - 已有 prefab / data / manifest / probe 的 crowd 槽位
    - 不是“原 Day1 正式角色已经扩完”
  - 当前 manifest 的 phase 配置存在明显语义漂移风险：
    - `101 / 103` 提前从 `EnterVillage` 开始
    - `201` 提前从 `HealingAndHP` 开始
    - `104` 从 `WorkbenchFlashback` 开始
    - `301` 在 `ReturnAndReminder / FreeTime` 出现
    - 这些都比原案要求更早、更重
  - 但本轮不直接改 `SpringDay1NpcCrowdManifest.asset`
    - 第一真实 blocker 不是“改法不清楚”
    - 而是 `NPC-v` 还没完成“原案角色 -> 101~301 槽位”的 own 映射回正
    - `spring-day1` 这侧当前能先说死的是：
      - 谁是主角色链
      - 谁只是 crowd
      - 哪些原案角色还没被承载
- 当前恢复点：
  - `NPC-v`
    - 下一轮只做原案映射与最小 NPC 本体回正
  - `spring-day1V2`
    - 下一轮在 NPC-v 回正后，再决定是否需要最小收窄 manifest phase / 语义口径

## 2026-04-03｜补记：原剧本人设核实完成第一轮钉实，当前停在“可判偏差，不可合法一对一回正”

- 当前主线目标：
  - 按 `2026-04-03-NPC-v_春一日新NPC群像原剧本人设核实与NPC本体映射回正prompt-04.md`
    核实“原案角色 -> 当前 `101~301` 槽位”的真实关系，
    不再把当前新 8 人的现编名字、人设和对白当成既定真相。
- 本轮实际完成：
  - 已完整读取并对齐：
    - `Deepseek聊天记录001.md`
    - `春1日_坠落_融合版.md`
    - `初步规划文档.md`
    - `Deepseek-2-P1.md`
    - `省流版VIP8.md`
    - `SpringDay1NpcCrowdBootstrap.cs`
    - `SpringDay1NpcCrowdManifest.asset`
    - `NPC_001/002/003` 三份老 Day1 角色底座
  - 已查实原案真相源优先级：
    - 用户原始 Day1 剧情原文
    - `0.0.1剧情初稿`
    - 长线 NPC 角色表
    - 当前 `bootstrap / manifest`
  - 已查实原案稳定主角色链：
    - `马库斯`
    - `艾拉`
    - `卡尔 / 研究儿子`
  - 已查实原案稳定重要人物：
    - `老杰克`
    - `老乔治 / 老铁匠`
    - `老汤姆`
    - `小米`
    - `围观村民 / 饭馆村民 / 小孩`
  - 已查实当前工程承载分层：
    - `NPC_001_VillageChief*` = `马库斯` 主链壳
    - `NPC_002_VillageDaughter*` = `艾拉` 主链壳
    - `NPC_003_Research*` = `卡尔` 的最接近语义壳
    - 当前 `101~301` = 新增 crowd 槽位层，不是原案正式角色表
  - 已完成 `101~301` 第一轮归类判断：
    - `102 Hunter`
      - 只到“语义接近大儿子 / 猎户线索”，未到“直接可确认”
    - `103 ErrandBoy`
      - 只到“可降级为村中少年 / 目击型群众”
    - `203 CanteenKeeper`
      - 只到“语义接近饭馆背景群众”
    - `101 / 104 / 201 / 202 / 301`
      - 当前都没有原案直接来源，且 `301` 与 Day1 主线气质冲突最大
- 本轮没有做的事：
  - 没有改 `SpringDay1NpcCrowdBootstrap.cs`
  - 没有改 `SpringDay1NpcCrowdManifest.asset`
  - 没有改 `Assets/111_Data/NPC/SpringDay1Crowd/*`
  - 没有进入真实施工，也没有跑 `Begin-Slice`
- 第一真实 blocker：
  - 当前证据足够判定“哪些写偏了”，
    但不足以把 `101~301` 合法一对一改成另一套具名原案角色；
    若现在直接改名，会继续引入新的自创映射，而不是完成回正。
- 当前判断：
  - 这轮已经能说死：
    - 老 `001/002/003` 才是原 Day1 主角色底座
    - 当前 `101~301` 大多只是 crowd 槽位，不应继续 claim 为原案正式角色
  - 这轮还不能说死：
    - `101~301` 每个槽位应该精确回成哪一个原案具名角色
- 当前恢复点：
  - 如继续本线，下一刀只能二选一：
    - 补更高权威的 Day1 cast 证据后，再做最小 NPC-own 回正
    - 或显式授权把明显写偏槽位降级成“匿名 / 次级群众”口径再收口

## 2026-04-04｜补记：已向 `NPC-v` 重新下发“原剧本群像回正 + NPC 本体收口”续工 prompt

- 当前主线目标：
  - 不再让 `101~301` 的后补群像继续冒充原案正式角色；
  - 同时把 `NPC-v` own 的 bubble / pair / content / runtime 闭环重新压回它自己这条线。
- 本轮新增稳定结论：
  1. 线程边界重新钉死：
     - `UI` 只做玩家面 `UI/UE`
     - `spring-day1` 只做 opening / Day1 逻辑与当前真实 blocker
     - `NPC-v` 只做原剧本角色回正、NPC prefab/content/profile/roam、本体气泡与 runtime probe
  2. 已新建续工文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-04_NPC-v_春一日原剧本群像回正与NPC本体收口prompt_05.md`
  3. 这份 prompt 明确要求 `NPC-v` 只做两段：
     - 原剧本角色回正
     - NPC 本体运行与旧气泡 / pair bubble 收口
  4. 明确禁止 `NPC-v` 再碰：
     - `Primary.unity`
     - `GameInputManager.cs`
     - `SpringDay1Director.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - `SpringDay1NpcCrowdManifest.asset`
     - `PromptOverlay / Workbench / DialogueUI / opening tests / Town / 字体`
- 本轮动作：
  - 只写续工 prompt
  - 未改 NPC 资产本体
  - 未继续真实施工
- 当前恢复点：
  - 等 `NPC-v` 按 `prompt_05` 回执后，再把：
    - `NPC own`
    - `spring-day1 接盘`
    - `Town 冻结`
    三类尾项重新分账

## 2026-04-05｜补记：已按 Day1 owner 真值审回执，并给 `NPC-v` 下发 `prompt_06`

- 当前主线目标：
  - 不再让 `NPC-v` 继续含糊“formal / casual / ambient` 优先级；
  - 也不再让 `101~301` 的口径漂成“原案正式角色已经扩完”。
- 我方这轮对 `NPC-v` 回执的裁定：
  1. 接受它退出：
     - `SpringDay1Director.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - `Primary.unity`
     - `PromptOverlay`
     - opening / Day1 正式剧情控制
  2. 接受它继续守：
     - `NPCBubblePresenter.cs`
     - `PlayerNpcChatSessionService.cs`
     - NPC 非正式聊天 / pair / ambient bubble 底座
  3. 明确补真值：
     - Day1 统一优先级 = `formal > casual > ambient`
     - `CrashAndMeet ~ ReturnAndReminder` 这些正式阶段里，如果 formal 当前应接管，就不再允许共享提示显示“闲聊”
     - `NPC001/002/003` 继续视为原 Day1 主角色承载
     - `101~301` 在没有更高权威 exact mapping 前，统一按群众层/线索层/氛围层降级
  4. 明确指出：
     - `PlacementManager.cs(1694,23)` 外部编译红只能阻断它继续跑 fresh live probe
     - 不能阻断它先做静态 own 回正
- 本轮新增续工文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-05_NPC-v_Day1真值补线与NPC正式非正式优先级续工prompt_06.md`
- `prompt_06` 当前要求它只做：
  1. `101~301` 静态回正
  2. NPC own formal/casual/ambient 优先级与旧气泡 / pair bubble 收口
- 当前恢复点：
  - 等 `NPC-v` 按 `prompt_06` 回执后，再把：
    - 已静态回正的群众层
    - 仍待 live probe 的 bubble/runtime
    - 需要我方 Day1 再接的 formal 真值
    三块重新分账

## 2026-04-04｜补记：已对齐 `prompt_01 + prompt_05`，NPC 当前应拆成“玩家面分工”与“NPC本体收口”两段

- 当前主线目标：
  - 不直接开工，先把 `UI / spring-day1 / NPC` 三边最新口径统一成可执行判断：
    - 玩家面里哪些需求真归 `NPC`
    - `NPC` 自己现在第一刀到底该先做什么
- 本轮新增稳定结论：
  1. `prompt_01` 要求我先做：
     - `exact-own / 协作切片 / 明确不归我` 三类矩阵
     - 再从 `exact-own` 里只认第一刀
  2. `prompt_05` 要求我继续真实施工时，只收两段：
     - `101~301` 回到原剧本口径
     - `NPC own` 的旧气泡样式 / pair bubble / content / profile / prefab / runtime probe 收口
  3. 结合已抽查的真实产物，当前最合理的总判断是：
     - `玩家面 NPC 方向` 与 `NPC 本体收口` 不应混成一刀
     - 前者是 contract / 分工裁定，后者才是接下来真实施工的主刀
  4. 当前 `NPC` 在玩家面里的 `exact-own` 核心已收窄到：
     - speaking-owner 层级
     - NPC 气泡不被场景遮挡
     - 气泡背景不透明
     - 正式/非正式聊天闭环
     - 头顶提示退场后的 NPC 语义一致性
  5. 当前不应再吞：
     - `PromptOverlay`
     - `Workbench`
     - `Town DialogueUI`
     - `Primary.unity`
     - `GameInputManager.cs`
     - 全局字体底座
- 本轮没有做的事：
  - 没进入真实施工
  - 没跑 `Begin-Slice`
  - 没改任何代码或资产
- 当前恢复点：
  - 先按用户要求输出三段观后感 + 最终任务清单；
    待用户拍板后，再决定是继续停在只读裁定，还是正式开工第一刀

## 2026-04-05｜补记：NPC own 会话/气泡底座继续压实，当前因外部编译红转入 PARKED

- 当前主线目标：
  - 继续只做 `NPC own` 的非正式聊天会话/气泡底座修复；
  - 不改现有气泡主题样式，只记录当前样式盘点；
  - 不回吞 `Primary.unity`、`GameInputManager.cs`、`PromptOverlay / Workbench / Day1 UI shell`。
- 本轮真实施工：
  - 已继续维护并复核这 3 个 own 文件：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - `Assets/YYY_Tests/Editor/PlayerThoughtBubblePresenterStyleTests.cs`
  - 当前有效代码面结论：
    1. `PlayerNpcChatSessionService`
       - 会话气泡避让已收紧到“靠近头顶、谁在说话谁在上面”的较小偏移；
       - 自动停留时长锁定为 `1 + 0.08 * 字数`；
       - 距离打断链继续保留：先快进打字机，再进入离场收束。
    2. `NPCBubblePresenter`
       - 继续保持旧主题样式，不再把 `ReactionCue` 当成另一套视觉皮；
       - ambient 气泡现在会忽略“已经不可见的旧 conversation owner”，避免 pair / nearby bubble 被 stale owner 闷死。
    3. `PlayerThoughtBubblePresenterStyleTests`
       - 已锁定：
         - 玩家/NPC 气泡不透明；
         - 玩家/NPC 保持不同样式语义；
         - speaking-owner 排序带与布局偏移；
         - 旧 owner 释放。
- 本轮样式盘点结论：
  - 按“当前 live 主链真正在线的主样式”算：`4` 种
    - `NPCBubblePresenter`
    - `PlayerThoughtBubblePresenter`
    - `InteractionHintOverlay`
    - `SpringDay1PromptOverlay`
  - 按“工程里仍挂着的气泡/提示视觉壳”算：`7` 条
    - 上面 `4` 条
    - `SpringDay1WorldHintBubble.Interaction`
    - `SpringDay1WorldHintBubble.Tutorial`
    - `NpcWorldHintBubble`
  - 当前 `NPC own` 直接归口的是：
    - `NPCBubblePresenter`
    - 以及它的 NPC 侧驱动链
  - `PlayerThoughtBubblePresenter` 仍记为 player-side shared，不上收为 `NPC own`。
- 本轮验证：
  - `git diff --check`：
    - 对本轮 own 文件已通过。
  - `validate_script`：
    - `PlayerNpcChatSessionService.cs`：`0 error / 1 warning`
    - `NPCBubblePresenter.cs`：`0 error / 1 warning`
    - `PlayerThoughtBubblePresenter.cs`：`0 error / 1 warning`
    - `PlayerThoughtBubblePresenterStyleTests.cs`：`0 error`
    - `PlayerNpcConversationBubbleLayoutTests.cs`：`0 error`
    - `NPCInformalChatInterruptMatrixTests.cs`：`0 error`
  - Unity 现场 blocker：
    - `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs(1694,23)` 仍有外部编译错误：
      - `Operator '-' is ambiguous on operands of type 'Vector3' and 'Vector2'`
    - 因此这轮无法把 EditMode / runtime 自测继续推进到可信通过态。
- thread-state：
  - 已跑：
    - `Begin-Slice`
    - `Park-Slice`
  - 未跑：
    - `Ready-To-Sync`
  - 当前 live 状态：
    - `PARKED`
- 当前恢复点：
  - 如果后续继续本线，先等外部编译红清掉；
  - 然后优先做：
    1. fresh console / targeted tests 重跑；
    2. 针对 `NPC own` 会话打断与 pair bubble 再做一轮 live probe；
    3. 只把用户最终要验的部分交回用户，不扩去 shared UI 壳。

## 2026-04-05｜补记：已再次核实 NPC 线程遗留账，当前可稳定拆成“已完成 / 未完成 / 待外部解锁验证”

- 当前稳定判断：
  1. 已完成的，是代码底座与边界口径：
     - `speaking-owner` 排序底座
     - 气泡背景不透明
     - stale owner 释放护栏
     - 样式边界与计数口径
  2. 未完成的，是需要真实 runtime 继续闭环的体验项：
     - 跑开中断的最终体验
     - pair bubble 的 fresh live 证据
     - `101~301` 的 NPC 本体收口
  3. 当前最大不确定性不是需求，而是外部编译 blocker：
     - `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs(1694,23)`
- 当前恢复点：
  - 后续用户若再问“哪些已完成 / 未完成”，按上面三类口径回答，不再混写。

## 2026-04-05｜补记：NPC own 的 formal/casual/ambient 静态收口继续推进，当前先停在 PARKED

- 当前主线目标：
  - 继续只做 `NPC own` 的：
    - `formal > casual > ambient` 优先级底座
    - `101~301` crowd 真值锁定
    - `pair / ambient bubble` 的可见性护栏
  - 不碰：
    - `Primary.unity`
    - `GameInputManager.cs`
    - `SpringDay1Director.cs`
    - `SpringDay1NpcCrowdDirector.cs`
    - `PromptOverlay / Workbench / DialogueUI`
- 本轮真实施工：
  - 已对齐并读取：
    - `2026-04-05_NPC-v_Day1真值补线与NPC正式非正式优先级续工prompt_06.md`
    - `2026-04-05_NPC线程_气泡层级遮挡与不透明基线第一刀prompt_02.md`
    - `global-preference-profile.md`
  - 已继续沿用 slice：
    - `npc-own-priority-bubble-crowd-deep-slice-20260405`
  - 本轮实际改动：
    1. `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
       - `OnEnable()` 运行时主动 `EnsureRuntime()` 接回 `NpcAmbientBubblePriorityGuard`
       - `CanInteract()` / `TryHandleInteract()` / `ReportProximityInteraction()` 继续把 formal phase 下的 casual suppression 钉死
       - 所有早退分支统一清掉 `SpringDay1WorldHintBubble + NpcWorldHintBubble`
    2. `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
       - proximity 早退分支同步补 `NpcWorldHintBubble.HideIfExists(transform)`
    3. `Assets/YYY_Scripts/Story/Interaction/NpcAmbientBubblePriorityGuard.cs`
       - 将“正式阶段压掉当前可见 ambient bubble”的核心逻辑收成私有静态方法，便于复用和测试
    4. 新增测试：
       - `Assets/YYY_Tests/Editor/NpcAmbientBubblePriorityGuardTests.cs`
       - `Assets/YYY_Tests/Editor/NpcCrowdDialogueTruthTests.cs`
       - 继续保留 `Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs`
  - 本轮关键判断：
    - `NPCBubblePresenter.cs` 再次尝试 `apply_patch` 直接补 `CanShow()` 仍失败
    - 因此这轮改走：
      - 可写的 runtime guard
      - formal/crowd 真值测试
      来把同一件事从外围收住
  - `101~301` 当前静态 crowd 真值本轮已锁定为：
    - `101=LedgerScribe`
    - `102=Hunter`
    - `103=ErrandBoy`
    - `104=Carpenter`
    - `201=Seamstress`
    - `202=Florist`
    - `203=CanteenKeeper`
    - `301=GraveWardenBone`
    - pair matrix：
      - `101 -> 103/104/203`
      - `102 -> 301`
      - `103 -> 101/104/203`
      - `104 -> 101/103/203`
      - `201 -> 202`
      - `202 -> 201`
      - `203 -> 101/103/104`
      - `301 -> 102`
- 本轮验证：
  - `git diff --check`：
    - 对本轮 own 文件通过
  - `sunset_mcp.py recover-bridge`：
    - 已成功，baseline 恢复到 `pass`
  - 但 compile/validate/status 当前仍未形成可信闭环：
    - `validate_script` 对单文件持续超时
    - `compile` 持续超时
    - `status` 持续超时
    - `errors` 当前脚本自身报 `AttributeError: 'str' object has no attribute 'get'`
  - 因此这轮只能诚实判定为：
    - 代码层补强继续推进
    - Unity/CLI fresh red 证据未闭环
- thread-state：
  - 已跑：
    - `Begin-Slice`
    - `Park-Slice`
  - 未跑：
    - `Ready-To-Sync`
  - 当前 live 状态：
    - `PARKED`
- 当前恢复点：
  - 下一轮如继续本线，优先顺序是：
    1. 先重新拿 compile-first 结果，确认当前 own 文件没有新引红
    2. 再补 `formal suppression / pair bubble` 的 targeted probe
    3. 若 `NPCBubblePresenter.cs` 仍写不进去，就继续沿 guard/test 路线，不在热文件上空耗

## 2026-04-05｜补记：已清掉 NPC own 测试编译红，并把剩余 console 红分流回 foreign

- 当前主线目标不变：
  - 继续只守 `NPC own` 的 formal/casual/ambient 优先级、crowd 真值与 pair/ambient bubble 护栏。
- 本轮子任务：
  - 先处理用户指出的“你有大量报错”。
  - 仅清我这轮 own 的测试编译红，不回吞 `Primary.unity`、`GameInputManager.cs`、UI/Day1 热根。
- 本轮实际完成：
  1. 重写并收稳以下 3 份 Editor tests：
     - `Assets/YYY_Tests/Editor/NpcAmbientBubblePriorityGuardTests.cs`
     - `Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs`
     - `Assets/YYY_Tests/Editor/NpcCrowdDialogueTruthTests.cs`
  2. 处理方式统一改成：
     - 去掉对 `Sunset.Story` / `StoryPhase` / `StoryManager` / `NPCDialogueContentProfile` / `NPCInformalChatInteractable` / `InteractionContext` 的编译期强绑定
     - 改走反射与 `AssetDatabase.LoadAssetAtPath(path, type)` 的非泛型加载
  3. Unity fresh console 已确认：
     - 上述 NPC own 测试编译红已清零
- 本轮验证：
  - `sunset_mcp.py errors --count 30 --output-limit 30`
    - 一度返回 `errors=0 warnings=0`
    - 说明此前那组 `CS0246 / CS0103` 已不再存在
  - `sunset_mcp.py status`
    - 当前已不再报 NPC 测试编译红
    - 但 console 里还剩 foreign/runtime/importer 红：
      - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs:260`
      - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset`
  - `git diff --check`
    - 当前被 shared foreign 路径拦住
    - 不是本轮 NPC own 文件自身格式红
- 当前阶段判断：
  - NPC own 编译红：已清
  - 项目 console：仍非全绿，但剩余已不是这轮 NPC own 测试造成
- thread-state：
  - 已跑：
    - `Begin-Slice`
  - 未跑：
    - `Ready-To-Sync`
- 当前恢复点：
  - 如果下一轮继续 NPC 本线，直接从：
    1. fresh status / fresh console
    2. formal/casual/ambient targeted probe
    3. crowd truth / pair bubble 继续收口
    开始，不必再回头处理这批测试编译红。
## 2026-04-05｜补记：formal->casual 门禁已从“全局 phase 封杀”改回“same-object formal takeover”，002/003 首轮闭环与跑开中断 live 通过
- 当前主线目标：
  - 继续只做 NPC own 的非正式聊天会话底座、formal/casual 门禁与会话中断闭环；
  - 不回吞 Primary.unity、GameInputManager.cs、PromptOverlay / Workbench / UI shell。
- 本轮真实施工：
  1. 改动文件：
     - Assets/YYY_Scripts/Story/Interaction/NpcInteractionPriorityPolicy.cs
     - Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs
     - Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs
  2. 关键修正：
     - 保留 StoryPhase 级的 formal 高优先级判定；
     - 但 NPC casual 的真正 suppression 改成：
       - 只有“同一个 NPC 自己挂着的 formal dialogue 当前可接管”时，才压住 casual；
       - 不再因为 CrashAndMeet / EnterVillage ... 这类 formal phase，就把  02 / 003 这种非当前 formal 对象整段误杀。
  3. NPCInformalChatInteractable 同步补强：
     - context == null 时自动回退 BuildInteractionContext()；
     - proximity 早退统一补 NpcWorldHintBubble.HideIfExists(transform)；
     - OnEnable() 运行时继续确保 NpcAmbientBubblePriorityGuard 在场。
- 本轮验证：
  - 代码层：
    - py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20
      - fresh 返回 errors=0 warnings=0
    - py -3 scripts/sunset_mcp.py status
      - baseline=pass
      - console 仅剩 test-runner warning / TestResults 保存提示，不是 blocking error
    - git diff --check -- <3 touched files>
      - 通过
  - live targeted probe：
    1. ootstrap 后、尚未触发  01 正式剧情前：
       -  02 可直接起聊
       -  03 可直接起聊
    2.  02 闭环 trace：
       - 两轮自动续聊完成
       - endReason=Completed
    3.  02 PlayerTyping Interrupt：
       - playerExitSeen=True
       - 
pcReactionSeen=True
       - endReason=WalkAwayInterrupt
       - leavePhase=PlayerSpeaking
    4.  03 闭环 trace：
       - 两轮自动续聊完成
       - endReason=Completed
    5.  03 PlayerTyping Interrupt：
       - playerExitSeen=True
       - 
pcReactionSeen=True
       - endReason=WalkAwayInterrupt
       - leavePhase=PlayerSpeaking
- 当前阶段判断：
  - 这轮最关键的 NPC own blocker 已从“002/003 常被 formal phase 全局误杀”转成“门禁已回正，live 主链已能真实起聊/自动续聊/跑开中断”。
  - 当前仍未 claim 的不是底座本身，而是更外层 shared prompt / 玩家面 UI 壳体验。
- thread-state：
  - 已跑：
    - Begin-Slice
    - Park-Slice
  - 未跑：
    - Ready-To-Sync
  - 当前 live 状态：
    - PARKED
- 当前恢复点：
  - 如果下一轮继续 NPC 本线，优先从：
    1. 用户侧真实体验复核  02 / 003 的气泡距离、自动节奏和跑开观感；
    2. 只在 NPC own 范围内继续补 pair/ambient 与 speaking-owner 的体验细节；
    3. 不回吞 shared prompt shell / UI 壳。

## 2026-04-05｜补记：NPC 自然漫游撞墙静默卡死已收紧为真实零推进检测 + Moving 态异常显性中断

- 当前主线目标：
  - 修正式场景里 NPC 明显撞墙/贴墙零推进，但逻辑静默掉回短停的恢复链漏洞。
- 本轮只改：
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
- 本轮实际完成：
  1. 新增“上一帧已下发移动命令，但下一帧实体几乎没前进”的静态检测：
     - `TryHandlePendingMoveCommandNoProgress(...)`
     - `MarkMoveCommandIssued(...)`
  2. 把 blocked/stuck recover 成功时错误清零的问题收紧：
     - `TryBeginMove(...)` 与 `TryRebuildPath(...)` 新增 `preserveBlockedRecoveryState`
     - 让同一堵墙前的 blocked/terminal 计数不再被假恢复吞掉
  3. 把 `Moving` 态下的异常空路径 / 丢 waypoint 从静默短停改成显性 interruption：
     - `PathClearedWhileMoving`
     - `WaypointMissingWhileMoving`
  4. 已创建两次本地可回退 checkpoint：
     - `263f4ed0` `2026.04.05_导航检查V2_npc-wall-stall-recovery-tighten`
     - `bf386811` `2026.04.05_导航检查V2_npc-moving-path-loss-interrupt`
- fresh 验证：
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 通过
  - 最新 `Editor.log` 存在 fresh `*** Tundra build success (9.77 seconds), 9 items updated, 862 evaluated`
  - `Tools/Codex/NPC/Run Natural Roam Bridge Probe`：
    - `PASS natural-roam-bridge`
    - `npc=002`
    - `final=(-9.24, 1.71)`
    - `sawBridgeSupport=True`
    - 无新的 `roam interrupted` 样本
  - `Tools/Codex/NPC/Run Traversal Acceptance Probe`：
    - `PASS bridge+water+edge`
    - `bridge_probe_pass npc=002`
    - `edge_probe_pass npc=003`
    - Unity 已主动退回 `Edit Mode`
- 当前判断：
  - 之前那条“卡住但不报错”的静默恢复漏洞已经被真实收紧；
  - 现有 NPC traversal 探针 fresh 为绿；
  - 但“正式场景里所有 choke point 都已穷尽验证”仍不能过度宣称，本轮最强证据仍是已修复代码链 + 现有 probe fresh pass。
- 当前恢复点：
  - 如果后续用户再给出新的正式场景卡墙点，直接优先在 `NPCAutoRoamController.cs` 的 pending-move / blocked-recovery / moving-path-loss 同簇继续取证，不必回头重修桥/边缘 contract。

## 2026-04-05｜更正：上一条新增记录因 PowerShell 反引号转义出现脏字样，以本条为准

- 更正结论：
  1. 这轮真正修掉的是 formal 对 casual 的全局误杀。
  2. 当前正确口径是 same-object formal takeover only。
  3. 002 和 003 在 bootstrap idle 现场都能直接起聊。
  4. 002 和 003 的两轮 casual 闭环都已 live 通过。
  5. 002 和 003 的 PlayerTyping 跑开中断都已 live 通过，playerExitSeen=True，npcReactionSeen=True。
- 这条更正只修正文案，不改变上一条记录里已经写明的代码文件、验证结论和 thread-state。

## 2026-04-05｜补记：NPC own 继续深推到 crowd probe 收口，当前 fresh blocker 已缩窄到 pair bubble live 发声链

- 当前主线目标：
  - 继续只做 NPC own 的 crowd/runtime/probe 收口，不回吞 UI、Day1 导演、Primary、Town 场景本体。
- 本轮实际完成：
  1. 静态与编辑器层补口：
     - `Assets/YYY_Scripts/Story/Interaction/NpcInteractionPriorityPolicy.cs`
       - 新增 editor-only `SetEditorValidationPhaseOverride`，只给 validation/probe 用，不改导演线真实 phase。
     - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
       - 去掉“任何 phase 都让 formal candidate 永远压过 casual”的旧兜底，正式/非正式优先级现在只按 formal priority phase 生效。
     - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
       - 新增 `LastPresentedText`，保留最近一次真正塞进气泡的文本，便于后续 probe 与聊天链排查。
     - `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
       - pair probe 临时覆写到 `FreeTime`；
       - walk-away probe 会主动跳过中断收尾动效；
       - FAIL/Evidence 逐条拆日志；
       - pair probe 额外记 `seen text / last text / bubble state`。
  2. 测试与验证补口：
     - `NpcCrowdDialogueNormalizationTests`：
       - `manage_script validate` clean；
       - EditMode class `2/2` 通过。
     - `NpcInteractionPriorityPolicyTests`：
       - 修掉旧反射夹具误判（UnityEditor.InteractionContext / overload 模糊 / singleton 泄漏）；
       - fresh EditMode class `14/14` 通过。
     - `NpcBubblePresenterEditModeGuardTests`：
       - 新增“ShowText / ShowConversationImmediate 直接拉起气泡可见态”的覆盖；
       - 脚本级 validate clean。
- fresh runtime probe 结论：
  - `instance=8/8`
  - `informal=8/8`
  - `walkAway=2/2`
  - 当前唯一未过项：
    - `pair=0/2`
- 当前已确认的 pair live blocker：
  1. partner 选择与 pair 文案解析都已经命中：
     - `initiatorDecision=chatting with Probe_xxx`
     - `responderDecision=joined chat with Probe_xxx`
     - `initiatorLines / responderLines` 都能解析出正确 pair 台词。
  2. 但 fresh probe 里两边气泡都没有留下可见文本：
     - `initiatorText=""`
     - `responderText=""`
     - `seenInitiatorText=""`
     - `seenResponderText=""`
  3. 当前 blocker 更像：
     - `ambient pair` 的 live 发声链没有真正把文本送到可见气泡上；
     - 不再是 phase suppression，也不再是 formal/casual takeover。
- 当前外部现场噪音：
  - fresh console 反复混入外部 missing-script 报错；
  - 场景现场曾切到 `Primary.unity`；
  - 这会干扰继续拿“干净 live 结论”，所以本轮不再把 live 结果过度宣称为 done。
- 当前恢复点：
  1. 下一轮优先继续只打 `NPCBubblePresenter / ambient pair live 发声链`；
  2. 直接沿 `LastPresentedText + pair probe issue logs` 继续查；
  3. 不回吞 UI / Town / 字体 / Primary。

## 2026-04-05｜补记：pair bubble 已从 live blocker 推到 fresh pass，crowd 内部语义也已继续降回群众层

- 当前主线目标：
  - 继续只做 `NPC own` 的 `pair ambient / bubble / crowd truth` 收口；
  - 不回吞 `UI / SpringDay1Director.cs / SpringDay1NpcCrowdDirector.cs / Primary.unity / Town.unity / GameInputManager.cs`。
- 本轮真实施工：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
     - ambient 展示前若检测到“提示期 suppression 只是残留、当前焦点并不属于自己”，会自动释放 stale suppression；
     - 不再因为上一拍 proximity prompt 的残留状态，把 pair bubble 整段闷死。
  2. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - `PlayAmbientChatBubble(...)` 改成“挑一条有效台词后，允许做极短二次 show retry”；
     - 修掉 single-frame show race 直接把 pair/ambient bubble 吞掉的问题。
  3. `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
     - pair probe 开始前会主动清 `SetInteractionPromptSuppressed(false)`、`SetConversationChannelActive(false)`；
     - `IsExpectedPairLine(...)` 改为按去换行后的文本比较，避免把 formatter 插入的换行误判成 pair 没亮；
     - runtime probe 临时根节点去掉 `HideFlags.DontSave`，清掉 probe 离场后的 Unity `Assert` 噪音。
  4. `Assets/YYY_Tests/Editor/NpcBubblePresenterEditModeGuardTests.cs`
     - 新增 stale prompt suppression 回归测试。
  5. `Assets/111_Data/NPC/SpringDay1Crowd/*.asset`
     - 8 组 crowd 的 `m_Name` 内部命名收回到群众/线索/夜间见闻口径：
       - `101 -> ScribeCrowd`
       - `102 -> HunterClue`
       - `103 -> WitnessBoy`
       - `104 -> CarpenterCrowd`
       - `201 -> MendingCrowd`
       - `202 -> HerbCrowd`
       - `203 -> DinnerCrowd`
       - `301 -> NightWitness`
  6. `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
     - bootstrap 生成 token、`displayName`、`roleSummary` 全部回正到群众层/线索层/夜间见闻层；
     - 避免后续重跑 bootstrap 又把 crowd 重新写回“具名正式角色”口径。
  7. `Assets/YYY_Tests/Editor/NpcCrowdDialogueTruthTests.cs`
     - truth token 同步到新的群众口径。
  8. `Assets/YYY_Tests/Editor/NpcCrowdPrefabBindingTests.cs`
     - prefab 绑定测试同步到新的群众口径。
- 本轮 fresh 验证：
  - direct `validate_script` 通过：
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
    - `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
    - `Assets/YYY_Tests/Editor/NpcBubblePresenterEditModeGuardTests.cs`
    - `Assets/YYY_Tests/Editor/NpcCrowdDialogueTruthTests.cs`
    - `Assets/YYY_Tests/Editor/NpcCrowdPrefabBindingTests.cs`
  - `git diff --check` 对上述 own 范围通过。
  - fresh runtime targeted probe 已拿到：
    - `instance=8/8`
    - `informal=8/8`
    - `pair=2/2`
    - `walkAway=2/2`
  - fresh console / CLI：
    - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`
      回到 `errors=0 warnings=0`
    - direct console 只剩 `There are no audio listeners in the scene` warning；
      本轮自己的 `DontSave Assert` 已清掉。
- 当前判断：
  - `pair bubble` 这条主 blocker 已从 `0/2` 推到 `2/2`；
  - 当前 `NPC own` 这段不再卡在 ambient pair 发声链；
  - 同时 `101~301` 内部命名和 bootstrap 再生成口径也已经进一步降回群众层，不再继续往“正式具名角色”漂。
- thread-state：
  - 本轮沿用既有 slice：
    - `npc-own-deep-push-crowd-content-and-pair-ambient-closure-20260405`
  - 已跑：
    - `Park-Slice`
  - 未跑：
    - `Ready-To-Sync`
  - 当前 live 状态：
    - `PARKED`
  - 备注：
    - `Park-Slice` 返回 `PARKED`，`.kiro/state/active-threads/NPC.json` 也已写成 `PARKED`；
      但 `Show-Active-Ownership.ps1` 同轮仍短暂把 `NPC` 显示成 `ACTIVE`，像是状态展示层滞后，不按这条表象误报现场。
- 当前恢复点：
  1. 下轮若继续 NPC 本线，优先从：
     - 用户可感知的 `pair / informal / walk-away` 体验复核
     - 以及剩余更细的 crowd 内容语义微调
     继续；
  2. 不必再回头把 `pair=0/2` 当成当前 blocker。
