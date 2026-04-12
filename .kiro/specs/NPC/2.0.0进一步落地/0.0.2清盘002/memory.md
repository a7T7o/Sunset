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

## 2026-04-05｜补记：formal 期 ambient 已从“整段全灭”收成“formal 当前接管时才压住”，并补齐 8 人 duty/phase 静态护栏

- 当前主线目标：
  - 继续只做 `NPC own` 的 formal/casual/ambient 优先级与 crowd 静态真值；
  - 不回吞 `Primary.unity / Town.unity / SpringDay1Director.cs / SpringDay1NpcCrowdDirector.cs / UI / GameInputManager.cs`。
- 本轮实际完成：
  1. `Assets/YYY_Scripts/Story/Interaction/NpcInteractionPriorityPolicy.cs`
     - ambient suppress 不再按 formal phase 全局闷死；
     - 现在只在两类情况压住 ambient：
       - formal dialogue 正在播放；
       - 当前 proximity focus 确实落在可接管的 formal NPC 上。
  2. `Assets/YYY_Tests/Editor/NpcAmbientBubblePriorityGuardTests.cs`
     - 补成三档回归：
       - formal 对话进行中应压掉 ambient；
       - formal phase 但没有当前接管时 ambient 仍可保留；
       - 当前 formal prompt 占焦点时应压掉 ambient。
  3. `Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs`
     - phase 基础矩阵回正为“formal phase 只直接决定 casual suppress”；
     - 新增“formal phase 无接管时 ambient 不应被全局闷掉”的断言。
  4. `Assets/YYY_Tests/Editor/NpcCrowdManifestSceneDutyTests.cs`
     - 从原来的局部抽查，补到 8 人完整 exact matrix；
     - 现在会同时校验：
       - `sceneDuties`
       - `semanticAnchorIds`
       - `growthIntent`
       - `minPhase / maxPhase`
- 本轮 fresh 验证：
  - `manage_script validate` clean：
    - `NpcInteractionPriorityPolicy.cs`
    - `NpcAmbientBubblePriorityGuardTests.cs`
    - `NpcInteractionPriorityPolicyTests.cs`
    - `NpcCrowdManifestSceneDutyTests.cs`
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`
    - fresh 回到 `errors=0 warnings=0`
  - `Tools/NPC/Spring Day1/Validate New Cast`
    - `PASS | npcCount=8 | totalPairLinks=16`
  - `Tools/NPC/Spring Day1/Run Runtime Targeted Probe`
    - `PASS | instance=8/8 | informal=8/8 | pair=2/2 | walkAway=2/2`
- 关键判断：
  - 这轮把剩余最大静态漏洞从“ambient 被 formal phase 一刀切”收回到了“只有当前 formal 真接管时才压”；
  - 同时把 crowd 8 人 duty/anchor/phase 的 NUnit 护栏补满，避免 101/102/104/202/203 再静悄悄漂掉。
- thread-state：
  - 已跑：
    - `Begin-Slice`
    - `Park-Slice`
  - 未跑：
    - `Ready-To-Sync`
  - 当前 slice：
    - `npc-own-priority-and-crowd-runtime-closure-20260405`
  - 当前 live 状态：
    - `PARKED`
- 当前恢复点：
  1. 若继续 NPC 本线，优先只剩：
     - crowd 内容语气强度细修；
     - 用户可感知的 pair / informal / walk-away 体验终验。
  2. 不必再回头把 ambient global suppress 当成未解决问题。

## 2026-04-05｜补记：NPC own 剩余测试契约与 EditMode 气泡底座已收平，当前已停在“只差体验终验 / 内容细修”

- 当前主线目标：
  - 继续只做 NPC own 的会话/气泡底座、crowd 内容与 targeted probe；
  - 不碰 `Primary.unity / Town.unity / SpringDay1Director.cs / SpringDay1NpcCrowdDirector.cs / UI / GameInputManager.cs`。
- 本轮子任务：
  - 核实 `Tests.Editor` 里疑似残留的 NPC own 失败是否还是真的；
  - 只收属于 NPC 的测试夹具与 EditMode 气泡底座契约。
- 本轮实际完成：
  1. `Assets/YYY_Tests/Editor/NpcAmbientBubblePriorityGuardTests.cs`
     - 把 `InteractionContext` 的反射解析从误指向 `UnityEditor.InteractionContext` 回正到 `Assembly-CSharp` 里的真实交互上下文；
     - 调整测试内类型解析顺序，避免同名类型再误判。
  2. `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
     - EditMode 下不再在 `Awake / OnValidate` 抢先创建 `NPCBubbleCanvas`；
     - 改成真正需要显示气泡时再懒创建，清掉 EditMode 测试里的 `SendMessage cannot be called during Awake/OnValidate` 噪音；
     - 不动现有气泡样式，只收初始化时机。
- 本轮验证：
  - `validate_script`：
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs` -> `0 error`
    - `Assets/YYY_Tests/Editor/NpcAmbientBubblePriorityGuardTests.cs` -> `0 error`
  - `git diff --check` 对本轮 touched files 通过。
  - targeted EditMode tests PASS：
    - `NpcAmbientBubblePriorityGuardTests.FormalPriorityPhase_ShouldHideVisibleAmbientBubble_WhenFormalPromptOwnsCurrentFocus`
    - `NpcBubblePresenterEditModeGuardTests.TemporaryEditObjectPresenter_ShowText_ShouldMakeBubbleVisible`
    - `PlayerThoughtBubblePresenterStyleTests.ConversationLayout_ShouldStayCloseToSpeakerHeads_WhileKeepingReadableSeparation`
    - `PlayerThoughtBubblePresenterStyleTests.ReadableHoldSeconds_ShouldFollowFixedPacingFormula`
    - `NPCInformalChatInterruptMatrixTests.ResumeIntroPlan_ShouldReturnContinuityLines_ForBlockingUiResume`
  - full `Tests.Editor` 后查 `C:\Users\aTo\AppData\LocalLow\DefaultCompany\Sunset\TestResults.xml`：
    - `NpcAmbientBubblePriorityGuardTests` = `4/4 Passed`
    - `NpcBubblePresenterEditModeGuardTests` = `5/5 Passed`
    - `NPCInformalChatInterruptMatrixTests` = `16/16 Passed`
    - `PlayerThoughtBubblePresenterStyleTests` = `7/7 Passed`
    - 当前剩余失败都落在 foreign：`Occlusion / ScenePartialSync / SpringDay1 Prompt/Director/Workbench`
  - Unity / MCP：
    - `Tools/NPC/Spring Day1/Run Runtime Targeted Probe` => `PASS | instance=8/8 | informal=8/8 | pair=2/2 | walkAway=2/2`
    - `Tools/NPC/Spring Day1/Validate New Cast` => `PASS | npcCount=8 | totalPairLinks=16`
    - 清 console 后 `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20` => `errors=0 warnings=0`
    - 当前已回到 Edit Mode。
- 当前判断：
  - 这一轮之后，NPC own 的“剩余测试契约没收平”已经不再是 blocker；
  - 代码层和 targeted probe 层都已再次站住；
  - 再往下继续写，就会开始碰用户体感终验或 UI / Day1 边界，不适合在本线程继续盲补。
- thread-state：
  - 本轮沿用 slice：`npc-own-remaining-test-contract-closure-20260405`
  - 已跑：
    - `Park-Slice`
  - 未跑：
    - `Ready-To-Sync`
  - 当前 live 状态：
    - `PARKED`
- 当前恢复点：
  1. 若继续 NPC 本线，优先只剩：
     - crowd 内容语气 / 强度细修；
     - 用户可感知的 `pair / informal / walk-away` 体验终验。
  2. 不需要再回头把 `Tests.Editor` 里的这组 NPC own fixture 当成当前 blocker。
  3. 若后续准备 sync，再先做 own 范围白名单收口与 `Ready-To-Sync`。

## 2026-04-05｜补记：只读复核再次确认“interrupt/style 残留失败”更像旧 runner/旧编译快照

- 当前补充结论：
  1. 对 `NPCInformalChatInterruptMatrixTests.ResumeIntroPlan_ShouldReturnContinuityLines_ForBlockingUiResume`、
     `PlayerThoughtBubblePresenterStyleTests` 这批旧失败的只读复核，与本轮实际验证结论一致：
     - 当前磁盘代码和当前已编 `Assembly-CSharp.dll` 都已经对上测试要求；
     - 旧失败列表更像先前半改状态或旧 runner/旧编译快照，不像当前树上仍未补口。
  2. `ResumeIntroPlan` 当前正确真值：
     - `CreateFallbackResumeIntroPlan()` 已返回测试要求的 continuity lines；
     - `ResumeIntroPlan` 也已经是属性形态，不是旧的 public field 版本。
  3. `PlayerThoughtBubblePresenter` 当前正确真值：
     - `ApplyPlayerBubbleStylePreset()` 已保持“与 NPC 明显不同但不透明”的 preset；
     - `FormatBubbleText()` 当前也已按 `preferredCharactersPerLine=10` 自动换行。
- 当前判断：
  - 这进一步加强了本轮总判断：
    - NPC own 当前不再卡在“interrupt/style 残留测试没收掉”；
    - 后续不应继续盲改 resume 文案或玩家气泡 preset，除非 fresh runner 再次给出新的真实失败。

## 2026-04-05｜补记：crowd 对话刷新链、人设化 interrupt/resume 与稳定旧 asset 路径已补齐，fresh live probe 复跑被外部串台噪音打断

- 当前主线目标：
  - 继续只做 NPC own 的 crowd 内容语气细修与 crowd dialogue 资产刷新链；
  - 不碰 `Primary.unity / Town.unity / SpringDay1Director.cs / SpringDay1NpcCrowdDirector.cs / UI / GameInputManager.cs`。
- 本轮实际完成：
  1. `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
     - 新增 `Tools/NPC/Spring Day1/Refresh Crowd Dialogue Assets` 菜单，只刷新 8 份 crowd dialogue asset；
     - `101~301` 的 crowd 文案继续往群众层/线索层压，补掉一批过重的“任务指令腔 / 主角腔 / 金句腔”；
     - 给 8 人都补上了 default `interrupt / resume` 人设化文案，不再全部落回通用 fallback；
     - 新增 `AssetStem` 稳定旧文件名映射，避免 slug 改名后再次生成第二套 dialogue / roam 文件。
  2. `Assets/YYY_Tests/Editor/NpcCrowdDialogueNormalizationTests.cs`
     - 扩充 stale crowd directive phrase 黑名单；
     - 新增“每份 crowd asset 不得再是空 `defaultInterruptRules / defaultResumeRules`”护栏；
     - 新增 bootstrap 源头必须保留 refresh 菜单、不得把 default rules 写空的护栏。
  3. `Assets/111_Data/NPC/SpringDay1Crowd/*.asset`
     - 已把 8 份 legacy crowd dialogue asset 真正刷新到最新 crowd 文案；
     - `defaultInterruptRules / defaultResumeRules` 已实写进资产；
     - 过程中误生成过一套新 slug 文件名的重复资产，已在 own root 内全部清掉，不留双份对话包尾账。
- 本轮验证：
  - `manage_script validate` clean：
    - `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
    - `Assets/YYY_Tests/Editor/NpcCrowdDialogueNormalizationTests.cs`
  - `git diff --check` 对：
    - `Assets/111_Data/NPC/SpringDay1Crowd`
    - `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
    - `Assets/YYY_Tests/Editor/NpcCrowdDialogueNormalizationTests.cs`
    通过。
  - `Editor.log` 已拿到：
    - `Tools/NPC/Spring Day1/Validate New Cast` => `PASS | npcCount=8 | totalPairLinks=16`
  - fresh console：
    - 仅跑 `Validate New Cast` 后 => `errors=0 warnings=0`
  - fresh runtime probe 复跑：
    - 已真实进入 Play，再自动退回 Edit；
    - 但这次 `Editor.log` 只拿到 `START`，没拿到新的 `PASS / FAIL` 终行；
    - 同时 fresh console 混入了 foreign 噪音：
      - `ExecuteMenuItem failed because there is no menu named 'Sunset/Story/Validation/Run Director Staging Tests'`
      - `PersistentManagers DontDestroyOnLoad` editor-side exception
    - 因此这轮只能报实为：
      - `fresh runtime probe attempted`
      - `但结果被外部串台/外部 editor exception 污染，不能把这次复跑继续 claim 成 clean pass`
- thread-state：
  - 本轮沿用 slice：
    - `npc-own-crowd-content-deep-polish-20260405`
  - 已跑：
    - `Begin-Slice`
    - `Park-Slice`
  - 未跑：
    - `Ready-To-Sync`
  - 当前 live 状态：
    - `PARKED`
- 当前判断：
  - crowd 这条线现在不再缺“资产刷新链 / resume/interrupt 底座 / 文件路径稳定性”这类代码层补口；
  - 剩余最值钱的 own 工作已经缩窄成两类：
    1. 等 foreign 噪音停下后，再补一次 clean runtime targeted probe
    2. 用户真实体验维度的 crowd 体感终验与极少量语气微调
- 当前恢复点：
  - 若后续继续 NPC 本线，先别再改 shared/UI/Day1；
  - 先在干净现场补一次 fresh runtime targeted probe；
  - 若 probe clean，再进入用户体验验收或 sync 前白名单收口。

## 2026-04-05｜补记：用户现场放开 Unity 后，quick test 已真实拿到 clean runtime pass

- 当前主线目标：
  - 继续只做 NPC own 的 crowd 内容与 runtime probe 收口；
  - 不碰 `Primary / Town / Day1 Director / UI / GameInputManager`。
- 本轮快速测试结果：
  1. `Tools/NPC/Spring Day1/Validate New Cast`
     - `PASS | npcCount=8 | totalPairLinks=16`
  2. `Tools/NPC/Spring Day1/Run Runtime Targeted Probe`
     - `PASS | instance=8/8 | informal=8/8 | pair=2/2 | walkAway=2/2`
  3. fresh console：
     - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`
     - `errors=0 warnings=0`
  4. Unity 状态：
     - 已确认回到 Edit Mode
- 关键说明：
  - 这次不是旧日志复用；
  - 本轮重新进了 Play，再自动退回 Edit；
  - `Editor.log` 已拿到这次新的 runtime probe `PASS` 终行与完整 evidence。
- 当前判断：
  - crowd 这条 own 线现在已经不再卡在 runtime targeted probe；
  - 代码层 + targeted runtime 层都再次 clean 站住；
  - 剩余主要只剩：
    1. 用户真实体验终验
    2. 如有必要的极少量 crowd 语气微调
- thread-state：
  - 本轮 slice：`npc-quick-clean-runtime-retest-20260405`
  - 已跑：`Begin-Slice`、`Park-Slice`
  - 未跑：`Ready-To-Sync`
  - 当前状态：`PARKED`

## 2026-04-05｜补记：基于 day1 自续工 prompt 的多角度只读判断已稳定

- 用户本轮要求：
  - 阅读 `spring-day1` 给自己的续工 prompt；
  - 不写代码，只从多个角度判断：
    1. prompt 本身方向是否对；
    2. `NPC own` 当前完成度是否已经担得起 Day1 群像承接；
    3. `Town` 现在到底跟不跟得上 `day1`。
- 当前稳定结论：
  1. day1 prompt 本身是对的：
     - 没漂回方案模式；
     - 真正把火力放在“导演工具 live 接入 + 后半段导演消费下沉”；
     - 对 Town 也保持了窄口径，不是假装全闭环。
  2. 从 `NPC own` 当前完成度看，NPC 现在已经能担起 Day1 后半段群像的底座责任：
     - 8 人 cast / crowd asset / runtime targeted probe / informal / pair / walk-away 已有真实通过证据；
     - 因此当前主 blocker 不再是 NPC 底座。
  3. `Town` 现在跟得上，但只是“轻量、anchor 优先、一个个 cue 打穿”这个级别的跟得上：
     - 可优先承接：
       - `EnterVillageCrowdRoot`
       - `KidLook_01`
       - `NightWitness_01`
       - `DinnerBackgroundRoot`
       - `DailyStand_01`（前面稳住后）
     - 不可误判为：
       - 后半段整张 Town 已 fully ready
       - 可以直接跳过具体 live 排练/写回/保存验证
  4. 当前更值得警惕的真实风险，在 day1 自己的导演链而不是 NPC：
     - `Run Director Staging Tests` 菜单桥问题
     - 排练/录制/写回链是否长期稳定
     - live 现场噪音不要污染导演验证
- 当前恢复点：
  - 若后续继续 NPC 本线，不应盲写新底座；
  - 应围绕 day1 已明确分给 NPC 的群像承接位继续补本体内容与语义细化；
  - 但不把 `NPC ready` 偷换成 `day1 已完工`。

## 2026-04-05｜补记：Day1 后半段群像内容并行续工已落一轮实改，且 clean runtime 仍站住

- 本轮主线：
  - 不再补 NPC 底座；
  - 只收 `Day1` 后半段会被导演线消费的群像内容层；
  - 重点覆盖：
    - `EnterVillage_PostEntry`
    - `DinnerConflict_Table`
    - `ReturnAndReminder_WalkBack`
    - `FreeTime_NightWitness`
    - `DayEnd_Settle`
    - `DailyStand_Preview`
- 本轮真实施工内容：
  1. `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
     - 重写 `101/102/103/104/201/202/203/301` 的 crowd 内容源；
     - 明确把这些 NPC 的 `selfTalk / playerNearby / defaultChatInitiator / defaultChatResponder / 部分 pair dialogue` 调到后半段群像口径：
       - `101 / 103`：进村围观、停手、偷看、压低嗓子、次日照常站位
       - `104 / 201 / 202 / 203`：晚餐冲突背景压力、散场回屋、低声议论、仍要照旧开门/开摊/开火
       - `102 / 301`：夜见闻、后坡、收夜规矩、夜路脚步与回声
  2. `Assets/YYY_Tests/Editor/NpcCrowdDialogueNormalizationTests.cs`
     - 新增 `CrowdDialogueAssets_ShouldCarryLateSceneSemanticCoverage`；
     - 用资产级 token 断言把这轮新增的后半段群像语义钉住，避免回退成“结构在、内容空”。
  3. `Assets/111_Data/NPC/SpringDay1Crowd/*.asset`
     - 通过 `Tools/NPC/Spring Day1/Refresh Crowd Dialogue Assets` 真正刷新 8 份 crowd dialogue asset；
     - 现在 source-of-truth 与资产序列化内容一致。
- 本轮验证：
  - `manage_script validate --name SpringDay1NpcCrowdBootstrap --path Assets/Editor/NPC --level standard` => clean
  - `manage_script validate --name NpcCrowdDialogueNormalizationTests --path Assets/YYY_Tests/Editor --level standard` => clean
  - `git diff --check -- Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs Assets/YYY_Tests/Editor/NpcCrowdDialogueNormalizationTests.cs Assets/111_Data/NPC/SpringDay1Crowd` => 通过
  - `Tools/NPC/Spring Day1/Validate New Cast` => `PASS | npcCount=8 | totalPairLinks=16`
  - `Tools/NPC/Spring Day1/Run Runtime Targeted Probe` => `PASS | instance=8/8 | informal=8/8 | pair=2/2 | walkAway=2/2`
  - fresh console：
    - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`
    - `summary = errors=0 warnings=0`
    - 但返回体里仍带 1 条 `PersistentManager` assert 型 entry，当前未被 CLI 归类为 error/warning，先报实为“无红计数，但有 editor-side assert 噪音条目”。
- 当前判断：
  - 这轮后半段群像内容已经不是空壳；
  - day1 后面只要继续消费这些 crowd NPC，至少能吃到更明确的：
    - 围观/偷看/停手感
    - 晚餐与散场压力
    - 夜见闻与回屋规矩
    - 次日“村子照常转”的冷处理感
- 当前恢复点：
  - 若下一轮继续 NPC 本线，优先不再补底座；
  - 应转向：
    1. 用户真实体验终验
    2. 若终验指出某个 scene-duty 的 crowd 语气还不够，再做一小轮定点微调。

## 2026-04-06｜补记：已确认“NPC 突然生成”属于 runtime crowd director 契约问题，不是内容层问题

- 用户新观察：
  - 新 NPC 看起来不像本来就在村里；
  - 而是到对话/阶段时才突然蹦出来；
  - 用户质疑它们本来就该常驻在村子里 / `Town` 里。
- 本轮只读核查结论：
  1. 真正负责运行时生成的核心文件是：
     - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
  2. 关键事实：
     - 它把 crowd runtime 限定在 `Primary`：
       - `PrimarySceneName = "Primary"`
       - 非 `Primary` 时会 `TeardownAll()`
     - 它在同步 crowd 时，会对每个 entry 调 `GetOrCreateState(...)`
     - 若 state 不存在，就 `Instantiate(entry.prefab, spawnPosition, Quaternion.identity)`
  3. 因此当前体验上的“突然生成”，不是错觉，也不是我这轮刚补的群像文本导致的；
     - 而是当前 crowd runtime contract 本来就是“按 phase/beat 在 `Primary` 中临时生成与显隐”，不是“Town 里本来就摆着一批常驻 NPC”。
- 边界判断：
  - 这件事当前不应由我这条 NPC 内容线默认吞掉；
  - 因为它已经越过“群像内容层”，进入：
    - `SpringDay1NpcCrowdDirector.cs`
    - `Town runtime contract`
    - 可能还会碰 scene 级常驻布置/常驻 root
  - 而这些在最新 day1 -> NPC prompt 里都明确不是我当前 own。
- 当前恢复点：
  - 如果后续要修“突然生成 / 常驻在村里”这件事，默认应由 day1 / Town runtime owner 接盘；
  - NPC 这边可以配合提供：
    - manifest / semantic duty / 文本内容 / crowd mapping
  - 但不应在未重新授权前，自己把部署契约一并吞掉。

## 2026-04-06｜补记：已给 day1 写出正式回执信与引导 prompt

- 本轮用户目标：
  - 不是立即施工；
  - 而是把我这边近几轮完成的 NPC 相关内容、已吸收的 day1 导演相关真值，以及最新“NPC 突然生成 / 驻村部署”判断，正式写成一份给 day1 的回执信；
  - 再补一份可直接转发给 day1 的引导 prompt。
- 本轮产出文件：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_后半段群像回执与驻村部署问题汇总_09.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_读取回执并判断crowd驻村部署prompt_10.md`
- 回执信里已明确写清：
  1. 我这边已完成的 crowd 底座与后半段群像内容层
  2. 我对 day1 当前导演线进度的吸收结果
  3. “突然生成”问题的代码定位：
     - `SpringDay1NpcCrowdDirector.cs`
  4. 为什么这属于 deployment/runtime contract，而不是内容层小问题
  5. 我当前对 owner 边界与下一步协作方式的判断
- 当前状态：
  - 本轮只做文档与交接材料；
  - 未进入新的代码施工；
  - thread-state 已 `Park-Slice`，当前状态 `PARKED`。

## 2026-04-06｜补记：驻村常驻居民语义矩阵已真正落盘到 manifest，可直接给 day1 导演线吃

- 这轮主线：
  - 读取 `prompt_11 + prompt_12` 后，继续只做 `NPC own` 的 resident 语义层；
  - 不吞 `CrowdDirector`、不碰 `Town runtime contract`、不改 deployment；
  - 目标是把 `101~301` 做成能被 day1 直接读取的常驻居民语义矩阵。
- 本轮真实落地：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdManifest.cs`
     - 新增并站住：
       - `SpringDay1CrowdResidentBaseline`
       - `SpringDay1CrowdResidentPresenceLevel`
       - `residentBeatSemantics`
       - `GetResidentPresenceLevel / GetResidentBeatFlags / HasResidentBeatFlags`
       - `IsDirectorPriorityBeat / IsDirectorSupportBeat / IsDirectorTraceBeat`
  2. `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
     - 为 8 个 NPC 全量补齐 6 段 beat 的 resident matrix：
       - `EnterVillage_PostEntry`
       - `DinnerConflict_Table`
       - `ReturnAndReminder_WalkBack`
       - `FreeTime_NightWitness`
       - `DayEnd_Settle`
       - `DailyStand_Preview`
     - 每段都落了：
       - `presenceLevel`
       - `flags`
       - `note`
     - 同时把 manifest 里的 `displayName / roleSummary` 刷成更像常驻村民的口径。
  3. `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
     - `Validate New Cast` 现在也会盯 resident baseline / beat semantics / presenceLevel，不再只看旧 duty。
  4. `Assets/YYY_Tests/Editor/NpcCrowdManifestSceneDutyTests.cs`
     - 新增 resident matrix 真值护栏；
     - 直接卡：
       - 旧具名角色名不得回流
       - baseline 不得漂
       - 6 段 beat flags 不得漂
       - helper contract 不得漂
  5. `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
     - 已执行 `Refresh Crowd Resident Manifest`；
     - 资源里已真实写入 resident 字段，不是只停在脚本定义。
- 当前 resident baseline 稳定结论：
  - 白天默认常在：`101 / 103 / 201 / 203`
  - 白天背景常在：`104 / 202`
  - 白天低可见、夜里更强：`102 / 301`
- 当前最适合给 day1 先吃回的 beat：
  1. `EnterVillage_PostEntry`：
     - `101 / 103`
  2. `DinnerConflict_Table`：
     - `101 / 104 / 201 / 202 / 203`
  3. `FreeTime_NightWitness`：
     - `102 / 301`
  4. 次级继续吃：
     - `ReturnAndReminder_WalkBack`
  5. support layer：
     - `DayEnd_Settle`
     - `DailyStand_Preview`
- 本轮验证：
  - `manage_script validate`
    - `SpringDay1NpcCrowdManifest.cs` clean
    - `SpringDay1NpcCrowdBootstrap.cs` clean
    - `SpringDay1NpcCrowdValidationMenu.cs` clean
    - `NpcCrowdManifestSceneDutyTests.cs` clean
  - `Refresh Crowd Resident Manifest`：成功
  - `Validate New Cast`：
    - `PASS | npcCount=8 | totalPairLinks=16`
  - `git diff --check`：通过
  - fresh console：
    - `errors=0 warnings=0`
  - Unity：
    - 已退回 `Edit Mode`
- 当前恢复点：
  - `NPC own` 这边可继续压的 resident 语义层已经基本压到头；
  - 下一步真正限制 day1 的，不再是我的语义矩阵，而是它自己的 deployment / director consumption 落地。

## 2026-04-06｜补记：formal 剧情聊天已改成一次性消耗，消费后只回落到 informal / resident

- 用户新增硬约束：
  - 正式剧情聊天不能重复触发同一段；
  - 消费后只能回落到：
    1. informal 闲聊
    2. resident 日常句池
    3. phase 后非正式补句
- 本轮真实改动：
  1. `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
     - `ResolveDialogueSequence()` 不再在 formal 消费后回放 `followup`；
     - 新增 `HasConsumedFormalDialogue()`；
     - 现在只要 initial formal 已被完成 / 推相位 / 解码，正式交互就直接让出，不再重播。
  2. `Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs`
     - 新增 same NPC formal 已消费后，informal 应重新接手的回归测试。
  3. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 新增文本级护栏，防止有人把 `NPCDialogueInteractable` 改回“formal 完了还能点出 followup 重播”。
- 本轮验证：
  - `manage_script validate`
    - `NPCDialogueInteractable.cs` clean
    - `NpcInteractionPriorityPolicyTests.cs` clean
    - `SpringDay1DialogueProgressionTests.cs` clean
  - `git diff --check`：通过
  - fresh console：
    - `errors=0 warnings=0`
- 当前恢复点：
  - 现在 `NPC own` 这边已同时站住：
    1. resident 矩阵
    2. formal 一次性消费
    3. post-consume 回落 informal/resident 的优先级契约

## 2026-04-06｜只读补记：resident manifest / formal-once-only 现场下仍可补的 day1 直接消费层

- 当前主线目标：
  - 不碰 `SpringDay1NpcCrowdDirector.cs`、`Town/Primary scene`、`UI`、`GameInputManager`；
  - 只读判断 NPC 线程还能补哪一层 helper / contract / test，最方便 `spring-day1` 直接消费。
- 本轮子任务：
  - 读取 resident manifest、formal once-only、day1 现有消费点与测试空白；
  - 收敛 3 个以内不越界建议。
- 本轮只读结论：
  1. 最值钱的 helper 不是再补 runtime 逻辑，而是把 manifest 现有逐条 API 收成“按 beat 可直接取用的消费快照”；
     当前 `Entry` 已有 `GetResidentPresenceLevel / GetResidentBeatFlags / GetDirectorConsumptionRole`，但顶层仍缺一次拿全 beat roster 的 helper。
  2. formal once-only 现在行为已成立，但 `NPCDialogueInteractable` 还没有公开的只读消费状态口径；
     day1 若想知道某 NPC 当前还是 formal one-shot，还是已回落 informal / resident，只能间接走 `CanInteract()` 或读私有逻辑。
  3. 当前最缺的测试不是再测 manifest 或 stage book 各自是否存在，而是二者之间的桥接契约；
     resident matrix 与 `StageBook` / cue 的断层还没有独立护栏。
- 建议的下一层：
  1. `helper`
     - 在 `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdManifest.cs`
       增加 beat 级只读快照 / roster helper，让 day1 一次拿到：
       - priority / support / trace / backstage-pressure
       - 对应 `npcId`
       - `semanticAnchorIds`
       - `note`
  2. `contract`
     - 在 `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
       增加 formal 消费状态的公开只读口径；
       让 day1 / validation 不必再靠私有方法推断“formal 是否已消费、是否已回落 informal/resident”。
  3. `test`
     - 新增或扩充一组 bridge tests，交叉验证：
       - `SpringDay1NpcCrowdManifest`
       - `SpringDay1DirectorStageBook`
       - `NPCDialogueInteractable`
       三者在核心 beat 上的消费边界与 cue 对齐关系。
- 本轮明确不建议：
  - 不建议再补 resident root / parent / active 判定 helper；
  - 这已经贴近 `CrowdDirector` 的 deployment / runtime 归类，边界太近。
- 涉及只读文件：
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdManifest.cs`
  - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
  - `Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs`
  - `Assets/YYY_Tests/Editor/NpcCrowdManifestSceneDutyTests.cs`
  - `Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs`
  - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
  - `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
- 验证状态：
  - 纯静态代码 / 文档取证；
  - 未改业务代码；
  - 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
    - 原因：本轮始终停留在只读分析。
- 当前恢复点：
  - 如果后续真要继续 NPC 自己的一小刀，最优先顺序应是：
    1. manifest beat-consumption helper
    2. formal 状态公开 contract
    3. manifest <-> stagebook bridge tests

## 2026-04-06｜续工补记：day1 可直接消费的 beat roster 与 formal state contract 已补齐

- 当前主线目标：
  - 继续只做 NPC own；
  - 不碰 `CrowdDirector / Town runtime / scene / UI / GameInputManager`；
  - 把 resident semantic contract 压到 day1 不用再手搓外部筛选。
- 本轮真实完成：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdManifest.cs`
     - 新增 `SpringDay1CrowdDirectorConsumptionRole`
     - 新增 `GetDirectorConsumptionRole()`
     - 新增 `IsDirectorBackstagePressureBeat()`
     - 新增 `TryGetEntry()`
     - 新增 `GetEntriesForDirectorConsumptionRole()`
     - 新增 `BuildBeatConsumptionSnapshot()`
  2. `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
     - `Validate New Cast` 现在会检查：
       - `EnterVillage_PostEntry` 的 `priority / trace`
       - `DinnerConflict_Table` 的 `priority / backstagePressure`
       - `FreeTime_NightWitness` 的 `priority`
  3. `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
     - 新增 `NPCFormalDialogueState`
     - 新增 `HasFormalDialogueConfigured`
     - 新增 `GetFormalDialogueStateForCurrentStory()`
     - 新增 `HasConsumedFormalDialogueForCurrentStory()`
     - 新增 `WillYieldToInformalResident()`
  4. `Assets/YYY_Tests/Editor/NpcCrowdManifestSceneDutyTests.cs`
     - 新增 direct-consumption role 与 snapshot helper 护栏
  5. `Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs`
     - formal consumed 后公开状态 contract 的回归测试已补
  6. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - public formal-state contract 文本护栏已补
- 本轮中途真实问题：
  - `Validate New Cast` 初次重跑报了 48 条 `presenceLevel = None`
  - 根因不是逻辑回退，而是 `SpringDay1NpcCrowdManifest.asset` 里的旧脏序列化值
  - 已通过 `Tools/NPC/Spring Day1/Refresh Crowd Resident Manifest` 刷回
  - 后续又修掉：
    - 离屏 `KeepRoutine` 被误判成前台 support 的角色层判断
    - 需要显式走一次 `Assets/Refresh`，避免 Unity 继续拿旧编译快照跑菜单
- 本轮验证：
  - `manage_script validate` clean：
    - `SpringDay1NpcCrowdManifest.cs`
    - `SpringDay1NpcCrowdValidationMenu.cs`
    - `NPCDialogueInteractable.cs`
    - `NpcCrowdManifestSceneDutyTests.cs`
    - `NpcInteractionPriorityPolicyTests.cs`
    - `SpringDay1DialogueProgressionTests.cs`
  - `Assets/Refresh` 后最新：
    - `Tools/NPC/Spring Day1/Validate New Cast`
    - `PASS | npcCount=8 | totalPairLinks=16`
  - `git diff --check` 通过
  - fresh console：
    - `errors=0 warnings=0`
  - Unity 保持 `Edit Mode`
- 当前阶段：
  - NPC own 的 resident semantic contract + direct-consumption helper + formal state contract 已压到头
- 当前恢复点：
  - 继续往下不该再默认扩到 stagebook/runtime/deployment
  - 若以后真要再补，只剩“是否授权补 bridge tests”这一类跨边界协作项

## 2026-04-06｜续工补记：bridge tests 已真实落地并跑过

- 当前主线目标：
  - 在不碰 `CrowdDirector / Town runtime / scene / UI` 的前提下，
    继续把 NPC own 能补的最后一层“day1 直接消费护栏”压完。
- 本轮真实完成：
  1. `Assets/YYY_Tests/Editor/NpcCrowdResidentDirectorBridgeTests.cs`
     - 补了 3 个桥接测试：
       - priority resident 在 `EnterVillage_PostEntry / FreeTime_NightWitness` 必须能 resolve 到 cue
       - `DinnerConflict_Table` 的当前 cue roster 不得越出 resident priority 白名单
       - 关键 beat 的 cue `semanticAnchorId / duty` 必须仍在 manifest 合同内
  2. `Assets/Editor/NPC/NpcResidentDirectorBridgeValidationMenu.cs`
     - 补了菜单：
       - `Tools/NPC/Spring Day1/Run Resident Director Bridge Tests`
- 本轮排障：
  - 菜单第一次没被 Unity 注册
  - 查实不是菜单路径问题，而是菜单脚本 own 编译错：
    - `TestStatus` 被误写成可空链
  - 已修正并重新 `Assets/Refresh`
- 本轮验证：
  - `manage_script validate` clean：
    - `NpcCrowdResidentDirectorBridgeTests.cs`
    - `NpcResidentDirectorBridgeValidationMenu.cs`
  - 真实菜单执行结果：
    - `Library/CodexEditorCommands/npc-resident-director-bridge-tests.json`
    - `status=passed`
    - `total=3`
    - `passed=3`
    - `failed=0`
  - `git diff --check` 通过
  - fresh console：
    - `errors=0 warnings=0`
    - 仅剩 test runner 写结果 XML 的信息项
- 当前阶段：
  - NPC own 的 helper / contract / bridge-test 三层都已压完
- 当前恢复点：
  - 再继续往下就不是 NPC 自己还能高质量单线推进的内容了
  - 现在更该等 day1 结合这套 contract 去接 deployment / director consumption / Town 落位

## 2026-04-06｜补记：已为 day1 落一份全量承接回执，并准备先停车等导演线调度

- 当前主线目标：
  - 不再继续扩 `NPC` 新代码；
  - 先把 `NPC` 这几轮已经做成的 resident / formal / bridge 层完整交给 `day1`；
  - 让 `day1` 能基于真实边界继续做 deployment / director / Town 承接。
- 本轮实际完成：
  1. 新增回执文档：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_全量进度与承接边界回执_14.md`
  2. 文档里已系统写清：
     - 我已做成的 resident semantic matrix / beat consumption helper / formal once-only contract / bridge tests
     - 当前真正 blocker 已经转到 `day1` 的 resident deployment / director consumption / Town 常驻落位
     - 我还能继续分担的最深层次
     - 我明确不该继续吞的边界
     - 给 `day1` 的建议调度顺序与优先吃回段
- 当前关键判断：
  - `NPC` own 现在已经不是“还缺概念层”，而是“底座已交，deployment/runtime 才是主 blocker”。
  - 如果后续还要继续协作，我最适合继续守的是：
    - `manifest / content profile / formal fallback contract / tests / probe`
  - 不适合默认回吞的是：
    - `CrowdDirector / Town runtime / scene 落位 / UI`
- 验证状态：
  - 本轮只写回执文档，没有新增业务代码；
  - 之前已站住的验证结果仍然是：
    - `Validate New Cast` = `PASS | npcCount=8 | totalPairLinks=16`
    - `Run Resident Director Bridge Tests` = `3/3 PASS`
    - fresh console = `errors=0 warnings=0`
    - Unity 保持 `Edit Mode`
- 当前恢复点：
  - 先把当前 slice 停到 `PARKED`
  - 等 `day1` 审完回执，再决定是否重新授权 `NPC` 往更深一层协作

## 2026-04-06｜续记：resident fallback 已继续压到 phase-aware nearby，并已双回执收口

- 当前主线目标：
  - 在不越界去吞 `director / Town / UI` 的前提下，把 `formal consumed -> resident fallback` 再往下压一层；
  - 做到不只闲聊 bundle 按 `StoryPhase` 回 resident，连玩家靠近 NPC 的 nearby 轻反馈也按 `StoryPhase` 走；
  - 然后把这轮阶段安全点同时回执给 `day1` 和 `存档系统`。
- 本轮实际完成：
  1. 代码与资产：
     - `NPCDialogueContentProfile.cs` 新增 `PhaseNearbySet / phaseNearbyLines / GetPlayerNearbyLines(relationshipStage, storyPhase) / TryGetPhaseNearbySet`
     - `NPCRoamProfile.cs` 新增 phase-aware nearby 透传
     - `PlayerNpcNearbyFeedbackService.cs` 改为按当前 `StoryPhase` 解析 nearby resident lines
     - `SpringDay1NpcCrowdBootstrap.cs` 新增 `BuildPhaseNearbyPayloads / BuildPhaseNearbySets / PhaseNearbyPayload`
     - 8 份 `Assets/111_Data/NPC/SpringDay1Crowd/*DialogueContent.asset` 已刷新进 `phaseNearbyLines`
     - `SpringDay1NpcCrowdValidationMenu.cs` 新增 `ValidatePhaseNearbyCoverage`
     - tests 已补到：
       - `NpcInteractionPriorityPolicyTests.cs`
       - `NpcCrowdDialogueNormalizationTests.cs`
       - `SpringDay1DialogueProgressionTests.cs`
  2. 回执：
     - 新增给 `day1` 的阶段安全点回执：
       - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_阶段安全点回执_15.md`
     - 已按固定文件回写 `存档系统` 边界回执：
       - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_NPC_存档边界回执_01.md`
- 本轮验证结果：
  - `manage_script validate` clean：
    - `NPCDialogueContentProfile.cs`
    - `NPCRoamProfile.cs`
    - `PlayerNpcNearbyFeedbackService.cs`
    - `SpringDay1NpcCrowdBootstrap.cs`
    - `SpringDay1NpcCrowdValidationMenu.cs`
    - `NpcInteractionPriorityPolicyTests.cs`
    - `NpcCrowdDialogueNormalizationTests.cs`
  - `SpringDay1DialogueProgressionTests.cs`
    - `native_validation=clean`
    - `owned_errors=0`
    - 但 `validate_script` 被现场已有 external console 噪音打成 `external_red`
  - 菜单真值：
    - `Tools/NPC/Spring Day1/Refresh Crowd Dialogue Assets` 已执行
    - `Tools/NPC/Spring Day1/Validate New Cast` 已执行
    - `Editor.log` 最新仍是：
      - `[SpringDay1NpcCrowdValidation] PASS | npcCount=8 | totalPairLinks=16`
  - `git diff --check` 对本轮 own 路径通过
  - `Run Resident Director Bridge Tests`
    - 之前最后一份稳定结果仍是 `passed`
    - 本轮 rerun 结果文件停在 `running/started`，未 claim 新通过
- 当前关键判断：
  - `NPC` own 现在又多交出了一层真正可消费的 resident contract：`phaseNearbyLines`
  - 后续最该由 `day1` 去吃回的，仍然是 `resident deployment / director consumption / Town 常驻落位`
  - `存档系统` 第一版只该接 `relationshipStage + formal consumed / completed sequence` 这种长期态，不该碰聊天过程态
- 当前恢复点：
  - 这轮已到安全点，不再继续往下压新功能；
  - 接下来只需 `Park-Slice`、补 thread memory 和审计日志即可离场。

## 2026-04-06｜续记：phase selfTalk + phase walkAway 已落地，day1 新安全点回执 16 已写

- 当前主线目标：
  - 按 `day1` 的 27 号补充口径，继续只做 `NPC` own 的 resident 常驻语义、formal consumed 后的 resident/informal 回落、以及可直接给 `day1` 吃的 validation/contract；
  - 不再围绕 `runtime spawn / deployment / Town resident 主逻辑` 扩写。
- 本轮实际完成：
  1. `phase-aware selfTalk` 已全链路落地：
     - `NPCDialogueContentProfile.cs` 新增 `PhaseSelfTalkSet / phaseSelfTalkLines / GetSelfTalkLines(StoryPhase) / TryGetPhaseSelfTalkSet`
     - `NPCRoamProfile.cs` 新增 `GetSelfTalkLines(StoryPhase)`
     - `NPCAutoRoamController.cs` 新增 `TryShowResidentSelfTalk(...)`，长停自语优先吃当前 `StoryPhase`
     - `SpringDay1NpcCrowdBootstrap.cs` 新增 `BuildPhaseSelfTalkSets / BuildPhaseSelfTalkPayloads`
     - 8 份 `Assets/111_Data/NPC/SpringDay1Crowd/*DialogueContent.asset` 已刷新出 `phaseSelfTalkLines`
  2. `phase-aware walkAwayReaction` 已继续补深：
     - `SpringDay1NpcCrowdBootstrap.cs` 新增 `BuildPhaseWalkAwayReactionPayload(...)`
     - `phaseInformalChatSets` 现在优先写入 phase-specific `walkAwayReaction`
     - 8 份 crowd dialogue assets 已刷新出各自 phase walkAway cue
  3. validation / tests 继续加厚：
     - `SpringDay1NpcCrowdValidationMenu.cs` 新增 `ValidatePhaseSelfTalkCoverage(...)`
     - runtime targeted probe 已补 selfTalk 入口代码，但本轮未 live claim
     - `NpcCrowdDialogueNormalizationTests.cs` 新增：
       - `CrowdDialogueAssets_ShouldContainPhaseAwareResidentSelfTalkLines`
       - `CrowdDialogueAssets_ShouldContainPhaseAwareWalkAwayReactions`
     - `SpringDay1DialogueProgressionTests.cs` 新增：
       - `NpcAutoRoamSelfTalk_UsesPhaseAwareResidentLines`
       - `CrowdBootstrap_UsesPhaseAwareWalkAwayReactionsAfterFormalConsumed`
  4. 用户临时报的编译红已止血：
     - 以 `carried foreign blocker fix` 方式最小补了
       `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - fresh `errors` 已回到 `errors=0 warnings=0`
  5. 回执文件：
     - 新增给 `day1` 的安全点回执：
       `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_阶段安全点回执_16.md`
     - 新增给 `存档系统` 的边界回执：
       `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_NPC_存档边界回执_02.md`
- 本轮验证结果：
  - `validate_script` clean：
    - `NPCDialogueContentProfile.cs`
    - `NPCRoamProfile.cs`
    - `SpringDay1NpcCrowdBootstrap.cs`
    - `SpringDay1NpcCrowdValidationMenu.cs`
    - `NpcCrowdDialogueNormalizationTests.cs`
    - `SpringDay1DialogueProgressionTests.cs`
  - `NPCAutoRoamController.cs`：`errors=0 warnings=1`
  - `SpringDay1NpcCrowdDirector.cs`：`errors=0 warnings=2`
  - `Tools/NPC/Spring Day1/Refresh Crowd Dialogue Assets` 已执行
  - `Tools/NPC/Spring Day1/Validate New Cast`：
    - `PASS | npcCount=8 | totalPairLinks=16`
  - targeted EditMode 小集 job `succeeded`
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 10`
    - `errors=0 warnings=0`
  - own + carried 路径 `git diff --check` 通过
  - Unity 当前在 `Edit Mode`
- 当前关键判断：
  - `NPC` own 这轮又多交出了两层真正可消费的 resident contract：
    - `phaseSelfTalkLines`
    - phase-specific `walkAwayReaction`
  - 继续往下最该由 `day1 / Town` 吃回的，仍然是：
    - 原生 resident deployment
    - director consumption
    - scene / anchor 落位
  - 存档边界不需要因这轮新增内容扩写：
    - `phaseSelfTalk / walkAwayReaction` 都是内容资产，不是新的长期态
- 当前恢复点：
  - 代码已到安全点，准备离场；
  - 下一步只做：
    1. 追加父级 / 线程 memory
    2. 追加 `skill-trigger-log`
    3. `Park-Slice`

## 2026-04-06｜收口：thread-state 已正式 PARKED

- `Park-Slice` 已执行：
  - `status = PARKED`
  - `blockers = 无`
- 当前离场状态：
  - Unity 在 `Edit Mode`
  - fresh `errors = 0`
  - 这轮安全点已正式结算完毕

## 2026-04-07｜只读分析：Town 正式对话后 NPC prompt 链还能安全继续砍什么

- 当前主线：
  - 用户要求只读评估 `Town` 里正式对话结束后，大量 NPC 同时恢复 `Update` 提示链时，这 3 个脚本内部还剩哪些安全性能刀口；
  - 明确限制：只看
    - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NpcInteractionPriorityPolicy.cs`
  - 不扩到更多运行时代码文件。
- 本轮子任务：
  - 只读复盘第一刀之后的剩余热点；
  - 收敛“还值得继续砍的点 / 最安全高收益的点 / 一动就越界的点”。
- 本轮确认：
  1. 剩余最大热点仍在 `NPCInformalChatInteractable.ReportProximityInteraction(...)`
     - 当前顺序仍是先 `ResolveSessionService + IsConversationActiveWith + CanInteractWithResolvedSession`
     - 再算 `boundaryDistance`
     - 这意味着远处 NPC 也会先支付一整套 session / suppression / formal takeover 判定。
  2. `NpcInteractionPriorityPolicy.ShouldSuppressInformalInteractionForCurrentStory(...)`
     - 仍通过 `NPCDialogueInteractable.CanInteract(context)` 走整条 formal 可交互链；
     - 其中 UI / Dialogue / formal consumed / story phase 判定，和 informal 链当前已有的全局 gate 有明显重复。
  3. `NPCDialogueInteractable.Update()`
     - 在 formal 已 consumed 的 NPC 上，仍会继续做 `TryBuildInteractionContext -> GetBoundaryDistance -> CanInteract`
     - 如果 Town 里很多 NPC 仍挂 formal 组件，这条链还有剩余成本。
  4. 两个 `ReportCandidate(..., () => OnInteract(context), ...)`
     - 仍然是 per-frame capture closure；
     - `InteractionContext` 也仍是 per-NPC per-frame 构建对象。
- 当前最安全且收益最大的下一刀：
  1. 先在 `NPCInformalChatInteractable.ReportProximityInteraction(...)` 里做 coarse distance 早退
     - 先用 `Mathf.Max(bubbleRevealDistance, SessionBreakDistance)` 做最宽半径筛掉远处 NPC；
     - 只有进到可见半径后，再去做 session / suppression / caption-detail 链。
  2. 给 formal takeover 拆一个“轻量 availability 判定”
     - 不再让 `NpcInteractionPriorityPolicy` 为了 suppression 每次都调用 `NPCDialogueInteractable.CanInteract(context)`；
     - 应改成只回答“当前剧情下 formal 是否仍可接管”的轻量路径，避免重复走 UI/dialogue/story gate。
  3. 去掉 per-frame capture delegate
     - 把两处 `() => OnInteract(context)` 改成 cached action + 最近一次采样数据；
     - 先砍每帧闭包分配，再决定要不要继续处理 `InteractionContext` 对象本身。
- 本轮不建议做：
  - 玩家侧集中收集/统一仲裁附近候选
    - 这已经越过当前 3 文件，进入 shared interaction service 范围。
  - 改 `SpringDay1ProximityInteractionService`、bubble/UI 显示链、Story/session manager 的共享 contract
    - 会碰 shared/UI/day1。
  - 把 formal consumed 后的 prompt/update 生命周期彻底改成跨系统事件驱动
    - 需要外部 story/dialogue/save 失效时机共同配合，本轮边界太大。
- 验证状态：
  - 本轮纯静态只读分析；
  - 未改任何运行时代码；
  - 未跑 `Begin-Slice`。
- 当前恢复点：
  - 如果继续真实施工，下一刀仍可只碰这 3 个脚本；
  - 优先顺序应是：
    1. informal 远距早退
    2. formal takeover 轻量化
    3. capture-free callback

## 2026-04-07｜真实续工：Town prompt 链第二刀已落到 NPC own，warning cleanup 也已顺手止血

- 当前主线：
  - 继续只做 `NPC own` 的 Town prompt 链性能压缩；
  - 不扩到 shared prompt shell / UI / day1 runtime。
- 本轮新增完成：
  1. `NPCInformalChatInteractable.cs`
     - distance 早退
     - bounds / context / callback 缓存化
     - 重复 `using System` warning 已清
  2. `NPCDialogueInteractable.cs`
     - distance 早退
     - bounds / context / callback 缓存化
     - 新增轻量 formal availability 判定
  3. `NpcInteractionPriorityPolicy.cs`
     - blocking page UI 按帧缓存
     - suppression 改走轻量 formal takeover
  4. `NPCBubblePresenter.cs`
     - play smoke 暴露的 `Awake` 创建 UI residue 已最小补口到 `Start()`
- 本轮验证：
  - `git diff --check` own 文件通过
  - fresh `errors` 曾回到 `errors=0 warnings=0`
  - `manage_script validate`
    - `NpcInteractionPriorityPolicy.cs` clean
    - `NPCDialogueInteractable.cs` / `NPCInformalChatInteractable.cs` 只剩 generic GC warning
- 当前没闭掉的点：
  - `NPCBubblePresenter` 的最终 play smoke 还没重跑完
  - 后续被外部 console/compile 噪音打断：
    - `SpringDay1WorkbenchCraftingOverlay.cs` external red
    - repeated `The referenced script (Unknown) on this Behaviour is missing!`
- 当前恢复点：
  - 代码层这刀已成安全点；
  - 只要外部现场回净，就直接回 `Town` 做最终 live 复测。

## 2026-04-07｜NPC 气泡形状补口：旧 scene 空壳重绑后会强制刷回真正的气泡 sprite

- 当前主线：
  - 用户直接追 `NPC` 气泡为什么还是“方泡/方框尾巴”；
  - 这轮只收 `NPCBubblePresenter` 的旧壳补口，不扩到 UI/day1/shared shell。
- 本轮查实：
  1. `Town.unity / Primary.unity` 里本来就落着旧的 `NPCBubbleCanvas` 壳；
  2. 这些旧壳上的 body/tail `Image` 有节点，但 `m_Sprite` 是空；
  3. 之前 `TryBindExistingBubbleUi()` 只绑引用，不回刷 sprite/type，所以旧空壳会直接接管现场视觉。
- 本轮改动：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
     - `EnsureBubbleUi()` 在绑定旧壳成功后，不再直接返回；
     - 先执行 `RefreshBoundBubbleUiAssets()`，把 body/tail 六张图重新赋成 runtime rounded-rect / tail sprite；
     - 同步重设 `Image.Type`、字体、layout、sorting；
     - 运行态补口继续保留在 `Start()`，避免 `Awake()` 期残留问题。
  2. `Assets/YYY_Tests/Editor/NpcBubblePresenterEditModeGuardTests.cs`
     - 新增 `TemporaryEditObjectPresenter_ShowText_ShouldRefreshLegacyBubbleSprites`；
     - 直接造一套和 scene 一样的旧空壳，验证 `ShowText()` 后 body/tail sprite 会被真正刷回。
- 本轮验证：
  - 新增目标测试：`Passed (1/1)`
  - 整份 `NpcBubblePresenterEditModeGuardTests`：`Passed (6/6)`
  - `git diff --check` 对本轮 2 个 own 文件通过
  - `validate_script` 对本轮 own 文件为 `owned_errors=0`，但 Unity 侧 assessment 仍会被 `stale_status` 挂成 `unity_validation_pending`
- 当前未闭环：
  - 真实 `Town` 里的最终 GameView 观感还需要玩家肉眼终验；
  - 这轮不 claim “体验已完全过线”，只 claim “根因已查清、代码补口已落地、旧壳回刷测试已过”。
- 外部 blocker：
  - fresh console 里仍有非本线 external red：
    - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs` 缺少 `EnsureProgressFillGraphic / EnsureProgressLabelBinding`
- 当前恢复点：
  - `NPC` 线程已 `Park-Slice`
  - 下一步如果继续，只需要回 `Town` 做一次人工终验：
    1. 看 NPC 气泡是否从方框回到圆角气泡
    2. 看尾巴是否恢复成三角尾巴
    3. 若仍不对，再继续沿旧壳 Rect/Image 参数链往下查

## 2026-04-07｜Primary/001 方泡续查：缓存残留也会在下一次展示前被强制回刷

- 用户继续反馈：`Primary/001` 真入口里还是会看到方泡，说明“初次绑定旧空壳就回刷”这一层还不够。
- 本轮进一步查实：
  1. `NPCBubblePresenter` 当前 runtime 生成的 body/tail 贴图本体仍是圆角 body + 三角 tail，不是生成函数自己画成了方块；
  2. 真正残余漏点在 `EnsureBubbleUi()`：
     - 以前只要 `_canvas/_bubbleText` 缓存还在就会直接早退；
     - 一旦运行中某次 residue 又把 body/tail `Image.sprite` 打回空，下一次 `ShowText()` 就不会再补口；
     - 旧 scene 空壳这时会直接显示 Unity 默认矩形色块，也就是用户看到的“方泡 / 方尾巴”。
- 本轮落地：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
     - 新增 `HasResolvedBubbleUi()`；
     - `EnsureBubbleUi()` 改成：
       - 已解析 UI 时，每次展示前也会重新 `RefreshBoundBubbleUiAssets()`；
       - 缓存半残时先 `ResetBubbleUiCache()` 再重新绑定/创建。
  2. `Assets/YYY_Tests/Editor/NpcBubblePresenterEditModeGuardTests.cs`
     - 新增 `TemporaryEditObjectPresenter_ShowText_ShouldRecoverCachedBubbleSpritesAfterTheyGoNull`；
     - 先正常显示一次，再把六张 body/tail image 的 `sprite` 清空、`type` 改坏；
     - 第二次 `ShowText()` 后断言六张图都会被重新刷回正确 sprite/type。
  3. `Assets/Editor/NPC/NpcBubblePresenterGuardValidationMenu.cs`
     - 新增真正挂在 Editor 装配里的命令桥菜单：
       `Tools/Sunset/NPC/Validation/Run Bubble Presenter Guard Tests`
- 本轮验证：
  - `Editor.log` fresh compile：`Tundra build success`
  - 无本轮 own error
  - external 仍只见 `SpringDay1WorkbenchCraftingOverlay.cs(2232)` 的过时 API warning
  - 命令桥测试结果：
    - `Library/CodexEditorCommands/npc-bubble-presenter-guard-tests.json`
    - `passed / success=true / total=7 / passed=7 / failed=0`
  - `git diff --check` 对本轮 3 个文件通过
- 当前诚实结论：
  - 结构层和 targeted probe 层已经站住：
    - 旧空壳首次回刷
    - 缓存残留后二次回刷
    都有代码和测试证据；
  - 但 `Primary/001` 的真实入口体验是否最终过线，仍需用户回现场再看一眼；
  - 如果用户复测后仍然看到方泡，下一步就不再优先怀疑 `NPCBubblePresenter` 自己，而要继续追别的头顶壳/提示链是不是在出图。

## 2026-04-08｜Town/002 只读排查：自动冒泡不是正常 NPC 逻辑，而是场景实例被挂进测试模式

- 用户反馈：
  - `Town` 里的 `002` 一直自己冒气泡，看起来像“被当成测试 NPC 了”，要求只读检查，不改现场。
- 本轮只读查实：
  1. `Town.unity` 里 `002` 的场景实例额外挂了 `NPCBubbleStressTalker`：
     - `Assets/000_Scenes/Town.unity:164616`
     - `m_Script guid = 2a83cf9837b058742b73c9ff3ad3796a`
  2. 这个 guid 对应：
     - `Assets/YYY_Scripts/Controller/NPC/NPCBubbleStressTalker.cs.meta`
  3. 同一场景实例上当前序列化值明确是测试态：
     - `Assets/000_Scenes/Town.unity:164621` -> `startOnEnable: 1`
     - `Assets/000_Scenes/Town.unity:164622` -> `disableRoamWhileTesting: 1`
     - `Assets/000_Scenes/Town.unity:164628` 起是一整段 `testLines`
  4. 脚本源码注释已经直接写明：
     - `Assets/YYY_Scripts/Controller/NPC/NPCBubbleStressTalker.cs:4`
     - `仅用于压测 NPC 气泡布局。`
- 额外判断：
  - 这不是 `002.prefab` 整体坏了，而是 `Town` 里的 `002` 场景实例被额外切进了测试模式；
  - 对照发现 `003.prefab` 也挂着同脚本，但 `startOnEnable: 0`，说明测试脚本本身不是问题，问题是 `Town/002` 被开成了自动测试。
- 最小恢复建议：
  1. 首选：从 `Town` 场景里的 `002` 实例移除 `NPCBubbleStressTalker`
  2. 次选：保留组件，但把 `startOnEnable` 改回 `0`
  3. 由于 `disableRoamWhileTesting: 1` 也开着，只要测试态持续，`002` 的正常漫游也会被压住
- 本轮边界：
  - 只读分析
  - 未改代码、未改场景、未改 Unity 运行状态
  - `thread-state` 维持 `PARKED`

## 2026-04-08｜NPC 气泡字体只读诊断：当前显示发糙不是单点，而是字体选择、文本样式和材质同步三层叠加

- 用户贴图反馈：
  - `Town/Primary` 里的 NPC 气泡字体现在发糙、发挤、显示观感明显不对。
- 本轮只读查实：
  1. `NPCBubblePresenter` 与 `PlayerThoughtBubblePresenter` 这两条气泡链当前都在硬用像素字体优先序：
     - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs:34-38`
     - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs:24-29`
  2. 所有当前 NPC prefab 也都直接序列化绑定了：
     - `fontAsset = DialogueChinese Pixel SDF`
     - 例如：
       - `Assets/222_Prefabs/NPC/001.prefab:304`
       - `Assets/222_Prefabs/NPC/002.prefab:276`
       - `Assets/222_Prefabs/NPC/003.prefab:277`
  3. 这套字体本体其实是：
     - `Fusion Pixel 10px Mono zh_hans`
     - 见 `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset:265`
     - 它是单宽像素中文字体，不适合继续叠当前这组重样式。
  4. 气泡文本样式参数现在过重：
     - NPC：
       - `characterSpacing = 1.25`
       - `lineSpacing = -5`
       - `outlineWidth = 0.18`
       - 见 `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs:912-915`
     - 玩家：
       - `characterSpacing = 1.25`
       - `lineSpacing = -2.5`
       - `outlineWidth = 0.18`
       - 见 `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs:569-572`
  5. `DialogueChineseFontRuntimeBootstrap` 运行时虽然更偏向先试 `DialogueChinese V2 SDF`
     - 见 `Assets/YYY_Scripts/Story/Dialogue/DialogueChineseFontRuntimeBootstrap.cs:10-17`
     - 但气泡链如果 prefab 已经绑了 `Pixel SDF`，`ResolveBestFontForText` 会优先继续吃这个 preferred font，不会自动切到 `V2`
  6. 另外一层代码缺口是：
     - `NPCBubblePresenter` / `PlayerThoughtBubblePresenter` 在换 `font` 时，没有像 `DialogueUI` / `NpcWorldHintBubble` 那样同步 `fontSharedMaterial`
     - 这会让运行时换字体后的材质状态不够稳
     - 对照：
       - 正确做法：`Assets/YYY_Scripts/Story/UI/NpcWorldHintBubble.cs:435-438`
       - 当前缺口：`Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs:905`、`1013-1016`
  7. 现场还有一个佐证：
     - `Town` / `Primary` 已出现多份 `DialogueChinese Pixel SDF Material + LiberationSans SDF Atlas X (Instance)` 运行态实例
     - 说明这套字体 atlas 在现场已经被动态扩写得比较重，不是干净的单 atlas 静态面
- 当前判断：
  - 玩家现在看到的“字体问题很大”，不是幻觉；
  - 真因不是“只有某个字没字形”，而是：
    1. 气泡链在吃不合适的像素单宽字体
    2. 同时套了偏重的字距 / 行距 / 描边参数
    3. 运行时字体切换又没把 shared material 同步完整
- 本轮边界：
  - 只读诊断
  - 未修改任何字体资产、气泡脚本或 prefab

## 2026-04-08｜NPC 气泡字体修复已落地：回正到非 Pixel 主链，并补齐材质与轻字体参数

- 当前主线：
  - 把用户贴图里暴露出来的 `NPC/玩家` 气泡字体显示问题，直接修到可打包状态；
  - 不碰 scene，不改 shared UI 壳，只改 `NPC own` 的两条气泡链和对应 editor tests。
- 本轮实际落地：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
     - `CurrentStyleVersion` 升到 `14`，让旧 prefab/runtime 自动吃到新样式；
     - 气泡字体优先序改成：
       - `DialogueChinese V2 SDF`
       - `DialogueChinese SDF`
       - `DialogueChinese SoftPixel SDF`
       - `DialogueChinese Pixel SDF`
     - `ResolveFontAsset()` 改成先找资源级 preferred runtime font，不再被 prefab 里旧 `Pixel SDF` 绑死；
     - 创建/回刷字体时补齐 `fontSharedMaterial = resolvedFont.material`；
     - 文本参数回正：
       - `characterSpacing = 0`
       - `lineSpacing = -0.8`
       - `textOutlineWidth = 0.08`
  2. `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
     - 同步改成非 `Pixel` 主链优先；
     - 同步补 `fontSharedMaterial`；
     - 玩家气泡字体参数回正：
       - `characterSpacing = 0`
       - `lineSpacing = -0.4`
       - `textOutlineWidth = 0.08`
  3. `Assets/YYY_Tests/Editor/NpcBubblePresenterEditModeGuardTests.cs`
     - 新增 guard：
       - `TemporaryEditObjectPresenter_ShowText_ShouldUseStableFontMaterialAndLighterTypography`
       - 明确卡：
         - 字体不再回到 `Pixel`
         - `fontSharedMaterial` 必须与 `font.material` 对齐
         - 新字距 / 行距 / 描边值必须成立
  4. `Assets/YYY_Tests/Editor/PlayerThoughtBubblePresenterStyleTests.cs`
     - 新增 `PlayerBubble_ShouldUseStableFontMaterialAndLighterTypography`
     - 同步守住玩家气泡的字体链和轻文本参数
- 本轮验证：
  - `git diff --check` 对本轮 4 文件通过
  - `Editor.log` 最新 compile 证据：
    - `*** Tundra build success (4.30 seconds), 7 items updated, 862 evaluated`
  - 最新 compile 未见本轮 own `error CS`
  - 当前日志里仅见 external warnings：
    - `PackagePanelRuntimeUiKit.cs(105)` 过时 API warning
    - `SpringDay1WorkbenchCraftingOverlay.cs(2245)` 过时 API warning
- 当前边界：
  - 代码层与编译层已站住
  - direct MCP 当前监听掉线，没拿到最新 live 视觉复测
  - 因此这轮结论是：
    - `结构/编译已过`
    - `玩家最终观感仍待现场终验`

## 2026-04-08｜接到新续工：native resident 接管与持久态协作
- prompt 文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-08_给NPC_day1原生resident接管与持久态协作prompt_17.md`
- 新一轮唯一主刀被钉死为：
  1. 给 `Day1` 提供原生 resident 被导演完全接管时不乱跑/乱打招呼/乱回旧逻辑的 contract；
  2. 给 `存档系统` 暴露 resident 最小 runtime snapshot surface；
  3. 明确不回吞 `runtime spawn / Town scene writer / CrowdDirector 主消费逻辑`。

## 2026-04-08｜resident 接管 contract 与最小 snapshot surface 已落地，线程停在安全回执点

- 本轮主线：
  - 继续执行 `prompt 17`，只做 `native resident` runtime contract：
    1. 给 `day1` 一套 scene-owned resident 被导演接管时不乱跑/不乱打招呼/不乱回旧逻辑的 owner contract；
    2. 给 `存档系统` 暴露最小可序列化的 resident runtime snapshot surface；
    3. 不回吞 `runtime spawn / deployment / Town/Primary scene writer / CrowdDirector` 主消费逻辑。
- 本轮代码落地：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - 新增 resident scripted control owner 栈与公开状态面：
       - `IsResidentScriptedControlActive`
       - `ResidentScriptedControlOwnerKey`
       - `ResidentStableKey`
       - `ResumeRoamWhenResidentControlReleases`
       - `IsNativeResidentRuntimeCandidate`
     - 新增公开 contract：
       - `AcquireResidentScriptedControl(...)`
       - `ReleaseResidentScriptedControl(...)`
       - `ClearResidentScriptedControl(...)`
       - `CaptureResidentRuntimeSnapshot()`
       - `ApplyResidentRuntimeSnapshot(...)`
     - `Update / FixedUpdate` 改成 resident scripted control active 时统一冻结 resident runtime。
  2. `Assets/YYY_Scripts/Controller/NPC/NpcResidentRuntimeSnapshot.cs`
     - 新增最小 DTO，当前字段只表达 stable key、scene/group/anchor、位置与 scripted control 状态。
  3. `Assets/YYY_Scripts/Controller/NPC/NpcResidentRuntimeContract.cs`
     - 新增 scene 级 helper：
       - `CaptureSceneSnapshots`
       - `TryApplySnapshot`
       - `TryFindResident`
       - `ResolveSceneTransform`
       - `BuildHierarchyPath`
  4. `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
     - resident 被 scripted control 接管时，不再开放 informal prompt / interact，也不在收尾时误恢复 roam。
  5. `Assets/YYY_Scripts/Service/Player/PlayerNpcNearbyFeedbackService.cs`
     - resident 被 scripted control 接管时，不再发 nearby bubble，已有 nearby 也会收掉。
  6. `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
     - active NPC 若在闲聊中途进入 scripted control，会按 `SystemTakeover / DialogueTakeover` 收束掉当前闲聊。
  7. `Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs`
     - 现有 `SpringDay1DirectorNpcTakeover` 已接入：
       - `AcquireResidentScriptedControl("spring-day1-director", ...)`
       - `ReleaseResidentScriptedControl("spring-day1-director", ...)`
     - 保留旧的 disable 保险带，没有去重写 day1 主消费逻辑。
  8. tests
     - `Assets/YYY_Tests/Editor/NpcResidentDirectorRuntimeContractTests.cs`
       - 新增 snapshot capture/apply/scene helper 护栏
     - `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
       - 新增 `NpcTakeover` acquire/release resident control 断言
- 本轮验证：
  1. `py -3 scripts/sunset_mcp.py errors --count 30 --output-limit 20`
     - `errors=0 warnings=0`
  2. `py -3 scripts/sunset_mcp.py compile ...`
     - 两次都被 `dotnet/codeguard timeout` 卡成 `assessment=blocked`
     - 没拿到一张漂亮的 compile-pass 小票
  3. `py -3 scripts/sunset_mcp.py validate_script ...`
     - `assessment=unity_validation_pending`
     - `owned_errors=0`
     - `external_errors=0`
     - `codeguard=timeout-downgraded`
     - MCP 侧最终卡在 `stale_status`
  4. `git diff --check`
     - 对本轮 own/carried 文件通过
- 本轮关键判断：
  - 现在 `NPC` 这边最值钱的事情已经不是继续回吞导演主线，而是把 resident 接管 contract 和最小 snapshot surface 正式交给 `day1 / 存档系统` 消费；
  - formal 是否已消费、relationship、剧情 one-shot 这些长期态仍不在本轮 snapshot 里，不能把 process-state 混成 resident runtime state。
- 本轮回执产物：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-08_NPC给day1_原生resident接管与持久态协作回执_18.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_NPC_存档边界回执_01.md`
     - 已追加 2026-04-08 resident snapshot 边界补充
- 当前阶段：
  - contract-ready，等待 `day1 / 存档系统` 按公开 contract 消费；
  - 线程已主动停在安全点，不继续自转去吞 runtime spawn / scene writer。
- thread-state：
  1. `Begin-Slice` 已跑
  2. 中途做过一次 `Begin-Slice -ForceReplace`，把 `NpcResidentRuntimeSnapshot.cs` 补进 owned paths
  3. `Ready-To-Sync` 未跑，因为这轮先停安全点不准备立刻 sync
  4. `Park-Slice` 已跑
  5. 当前 live 状态：`PARKED`
  6. 当前 blocker：无

## 2026-04-09｜统一人物主表第一刀已落地，正式对白/关系页/NPC内容资产开始回到同一条真值链

- 当前主线目标：
  - 不再停留在“剧情 NPC、关系页 NPC、场景 NPC 是三套东西”的只读结论层，而是先把统一人物主表真正落下来。
- 本轮子任务：
  1. 新建一份可运行的统一人物主表资产
  2. 把正式对白头像 fallback 接回主表
  3. 把关系页的人物身份真值从 crowd manifest 剥离出来，改由主表提供
  4. 用 editor 测试把 authored dialogue / NPC content asset / crowd manifest 三边捆成一条护栏
- 本轮完成：
  1. 新增 `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs`
     - 主表字段目前统一收：
       - `npcId`
       - `canonicalName`
       - `relationshipDisplayName`
       - `roleSummary`
       - `speakerAliases`
       - `prefab`
       - `handPortrait`
       - `showInRelationshipPanel`
       - `relationshipBaseline`
       - `relationshipBeatSemantics`
     - 新增主表 API：
       - `TryResolveNpcId(...)`
       - `GetRelationshipPanelEntries()`
  2. 新增 `Assets/Resources/Story/NpcCharacterRegistry.asset`
     - 当前已覆盖：
       - `001/002/003`
       - `101/102/103/104/201/202/203/301`
     - 当前已桥接的 formal / crowd alias：
       - `村长 / 马库斯 -> 001`
       - `艾拉 -> 002`
       - `卡尔 -> 003`
       - `村民 / 围观村民 -> 101`
       - `小孩 / 小米 -> 103`
       - `饭馆村民 -> 203`
       - `老杰克 -> 102`
       - `老乔治 -> 104`
       - `老汤姆 -> 301`
  3. `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
     - 正式对白头像不再永远回落到默认 `001`
     - 改成：
       - 先吃 `DialogueNode.speakerPortrait`
       - 再按 `speakerName -> NpcCharacterRegistry`
       - 再回退到旧默认图
  4. `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
     - 关系页现在由主表提供人物身份、显示名和角色简介
     - crowd manifest 继续只提供 Day1 在场语义，不再冒充人物总表
  5. 新增 `Assets/YYY_Tests/Editor/NpcCharacterRegistryTests.cs`
     - 当前护栏已覆盖：
       - 主表覆盖全 roster
       - core alias -> stable npcId
       - formal / crowd fallback speaker 有 portrait
       - `DialogueUI` / 关系页消费主表
       - 所有 authored dialogue `speakerName` 都能回到主表
       - 所有 `NPCDialogueContentProfile` 资产与 `SpringDay1NpcCrowdManifest` 条目都能回到主表
- 本轮验证：
  1. `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs --count 20 --output-limit 5`
     - `owned_errors=0`
     - `assessment=unity_validation_pending`
     - `manage_script validate = clean`
  2. `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Tests/Editor/NpcCharacterRegistryTests.cs --count 20 --output-limit 5`
     - `owned_errors=0`
     - `assessment=unity_validation_pending`
     - `manage_script validate = clean`
  3. 当前没拿到漂亮的 Unity ready 小票，不是 own 红，而是：
     - `codeguard timeout-downgraded`
     - `wait_ready / stale_status`
     - 以及中途现场被外部 PlayMode 状态抢占
- 当前关键判断：
  - 这轮已经把“统一表”从口头方案推进成了真实资产 + 真实消费口 + 真实护栏；
  - 但还不能谎报成“所有 NPC 消费口都完全同源了”，因为：
    1. live 终验还没补
    2. `PackageNpcRelationshipPanel.cs` 当前与 `UI` 线程同路径重叠，后续不应继续单方面叠改
- 当前恢复点：
  - 下一步如果继续，优先顺序应是：
    1. 在 Unity 里确认 `NpcCharacterRegistry.asset` 引用没丢
    2. 确认关系页实际能看到 `001/002/003/301`
    3. 确认 formal 对白里 `村长/艾拉/卡尔/村民/小孩/饭馆村民` 不再统一回默认 `001` 图
    4. 如果还要继续扩，同步更多消费口到主表，但先避开和 `UI` 当前重叠的文件
- thread-state：
  1. `Begin-Slice` 已跑：
     - `npc-unified-character-registry`
  2. `Ready-To-Sync` 未跑：
     - 这轮停在 safe stop，不准备直接 sync
  3. `Park-Slice` 已跑
  4. 当前状态：
     - `PARKED`
  5. 当前 blocker：
     - `live终验未补：主表资产/正式对白头像/关系页实际观感仍待Unity现场确认`
     - `共享消费口重叠：PackageNpcRelationshipPanel.cs 当前与UI线程同路径重叠，后续不再继续叠改`

## 2026-04-09 11:15 NPC_Hand 全局头像统一第一刀
- 用户目标：
  - 让正式对白与背包关系页优先吃 `Assets/Sprites/NPC_Hand` 里的手绘头像，并保持头像不越出父级框架。
- 本轮实际落地：
  1. `DialogueUI.cs`
     - 正式对白头像改成优先按 `speakerName -> NpcCharacterRegistry -> handPortrait` 解析；
     - 只有主表没解到时，才回退到 `DialogueNode.speakerPortrait`。
  2. `NpcCharacterRegistry.cs`
     - `ResolveRelationshipPortrait()` 改成与对白一致，优先 `handPortrait`，再回退 prefab 默认帧。
  3. `PackageNpcRelationshipPanel.cs`
     - 详情头像框与列表头像框都加了 `RectMask2D`；
     - 内边距改成更贴边但不出框：详情 `8px`，列表 `4px`。
  4. `NpcCharacterRegistry.asset`
     - 已补挂 `003`、`103` 的 `NPC_Hand` 头像；
     - 当前手绘已接上的 roster：`001/002/003/103`。
  5. `NpcCharacterRegistryTests.cs`
     - 新增护栏：`001/002/003/103` 只要有 `handPortrait`，对白与关系页都必须优先吃它。
- 新判断：
  - 用户刚提出的方向是对的，下一刀不该继续手工逐个挂图，而应改成“真源路径查询 + 一次性字典缓存”：
    1. 先按 NPC ID 去 `NPC_Hand` 找；
    2. 找不到再回退 prefab 默认帧；
    3. 不能做每次现查，必须做一次性索引/字典。
  - 但 Unity 运行时不能直接用 `AssetDatabase` 扫 `Assets/Sprites/NPC_Hand`，最佳实现应是：
    - editor 侧自动扫 folder 生成索引/主表映射；
    - runtime 侧只吃预构建好的字典或资源索引，不做全场搜索。
- 本轮验证：
  - `validate_script` 针对：
    - `NpcCharacterRegistry.cs`
    - `DialogueUI.cs`
    - `PackageNpcRelationshipPanel.cs`
    - `NpcCharacterRegistryTests.cs`
  - 结果统一为：
    - `owned_errors=0`
    - `assessment=unity_validation_pending`
  - `git diff --check`：
    - 没有空白错误；
    - 仅有 `DialogueUI.cs` 的 `CRLF -> LF` 警告。
- 本轮未完成：
  - 还没把 `NPC_Hand` 自动建字典这条更优架构真正落地；
  - 还没做 live 终验，因为当前 CLI/MCP 没拿到活动 Unity 实例。
- 恢复点：
  - 下一刀直接收“`NPC_Hand` 真源字典 + fallback contract”，把手工挂图降成兼容层而不是主路径。

## 2026-04-09 12:53 NPC_Hand 真源字典与打包态 fallback 落地
- 用户目标：
  - 不再继续手工逐个挂 `handPortrait`，而是把 `Assets/Sprites/NPC_Hand` 做成真正的头像真源；
  - 运行时要面向打包态，不能靠 runtime 扫 `Assets` 或低性能全局搜索。
- 本轮实际落地：
  1. `NpcCharacterRegistry.cs`
     - 新增 runtime 字典缓存：`npcId -> Entry`、`speaker/alias -> Entry`、`npcId -> handPortrait`
     - `TryGetEntryByNpcId / TryResolveSpeaker / GetRelationshipPanelEntries` 改成吃缓存
     - 新增 `TryResolveHandPortrait(rawValue, out Sprite)`，先归一到 `npcId`，再走 hand 头像字典
     - fallback contract 固定为：`handPortrait -> prefab 默认帧`
  2. 新增 `Assets/Editor/NPC/NpcCharacterRegistryHandPortraitAutoSync.cs`
     - `NPC_Hand` 文件夹现在成为 editor 真源
     - 目录里新增/删除/移动头像时，会自动把同名 `npcId` 写回 `NpcCharacterRegistry.asset`
     - 同时提供手动菜单：`Tools/Sunset/NPC/Sync Hand Portrait Registry`
  3. `NpcCharacterRegistryTests.cs`
     - 不再写死头像 roster
     - 改成直接读取 `Assets/Sprites/NPC_Hand` 当前文件名来验
     - 先通过反射触发自动同步器，再验证主表与对白/关系页 canonical 同源
  4. `NpcCharacterRegistry.asset`
     - 自动同步已真实生效
     - 当前已同步的 hand 头像 roster：`001 / 002 / 003 / 101 / 103 / 104`
- 当前验证：
  - `validate_script` 针对：
    - `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs`
    - `Assets/Editor/NPC/NpcCharacterRegistryHandPortraitAutoSync.cs`
    - `Assets/YYY_Tests/Editor/NpcCharacterRegistryTests.cs`
  - 结果统一为：
    - `owned_errors=0`
    - `manage_script validate = clean`
    - `assessment=unity_validation_pending`
  - Unity console 当前 own red = 0
  - 当前唯一看到的 error 是外部编辑器噪音：
    - `GridEditorUtility.cs: Screen position out of view frustum`
  - `run_tests(NpcCharacterRegistryTests)` 没拿到真实执行结果：
    - `Test job failed to initialize (tests did not start within timeout)`
- 当前恢复点：
  - 这套“真源目录 -> 主表同步 -> 运行时字典 -> prefab fallback”已经站住；
  - 下一刀如果继续，优先补 live 终验：
    1. formal 对白实际是否换成 `NPC_Hand`
    2. 背包关系页实际是否与 formal 同图
    3. 后续新增头像是否继续自动写回主表

## 2026-04-09 16:58 NPC 人物简介排版与内容梳理
- 用户目标：
  - 这轮不落代码，先把 NPC 简介本身的内容规划、排版分区和信息层级梳理清楚。
- 本轮实际完成：
  - 新增文档：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-09-NPC人物简介排版分区与内容梳理.md`
  - 文档内容覆盖：
    1. 关系页 / 人物册当前到底在记录什么
    2. 推荐固定分区：
       - 顶部识别区
       - 身份与位置区
       - 今日你会看到的样子
       - 你为什么会记住他
       - 后续预留区
    3. 推荐内容层级：
       - 一句身份锚点
       - 两句人物简介
       - 当前剧情感知句
    4. 当前 11 个 NPC 的分组与逐个梳理：
       - 核心正面承接组 `001/002/003`
       - 白天常驻村民组 `101/102/103/104`
       - 生活照应组 `201/202/203`
       - 边缘目击与夜间组 `301`
    5. 当前不该写什么、为什么不能写成百科
- 本轮依据：
  - 直接回看了：
    - `NpcCharacterRegistry.asset` 当前 `roleSummary / relationshipBeatSemantics`
    - `PackageNpcRelationshipPanel.cs` 当前承载字段与分区
    - Day1 正式对白里的已有角色真值
- 当前结论：
  - 现在 NPC 简介不缺“有没有句子”，真正缺的是统一的信息架构、统一的分区逻辑和统一的文案口径。
  - 这份文档已经把“后续该怎么写简介”压成可直接执行的标准。
- 当前恢复点：
  - 如果下一刀继续，不该先乱补新文案，而应该按这份梳理先收关系页 / 人物册的内容结构，再逐人精修文案。

## 2026-04-09 17:41 NPC 简介分工 prompt 已分发落盘
- 用户目标：
  - 不是继续分析，而是把刚完成的 NPC 简介结构梳理，拆成可直接发给 `UI / Day1 / NPC` 的 3 份 prompt。
- 本轮实际完成：
  - 新增 3 份 prompt 文件：
    1. `2026-04-09-NPC给UI_关系页人物简介排版收口prompt-01.md`
    2. `2026-04-09-NPC给Day1_人物简介剧情真值与曝光边界核定prompt-01.md`
    3. `2026-04-09-NPC自刀_按简介结构稿回填文案与关系册内容prompt-01.md`
- 当前分工压法：
  - `UI`
    - 只收关系页玩家面里的排版、分区、阅读顺序与层级
    - 不负责编人物内容
  - `Day1`
    - 只核 11 个 NPC 简介的剧情真值与曝光边界
    - 不接 UI、不接主表实现
  - `NPC`
    - 只回填 NPC own 的简介文案与关系册内容层同源
    - 不回吞 UI 壳
- 当前结论：
  - 简介这条线现在已经不是“缺方向”，而是进入可分工执行状态。
  - 这 3 份 prompt 是围绕同一份结构稿拆出来的，不再各自自由发挥。

## 2026-04-09 18:15 剧情里玩家与旁白头像切到 000
- 用户目标：
  - 把 `D:\Unity\Unity_learning\Sunset\Assets\Sprites\NPC_Hand\000.png` 接进正式剧情；
  - 所有玩家台词与旁白/内心独白头像统一换成这张。
- 本轮实际完成：
  1. `DialogueUI.cs`
     - 新增 `PlayerPortraitNpcId = "000"`
     - 新增玩家/旁白特殊头像解析：
       - `旅人`
       - `陌生旅人`
       - `玩家`
       - `主角`
       - `旁白`
       - `内心旁白`
       - 以及所有 `isInnerMonologue = true` 的节点
     - `ApplyPortrait()` 现在会先走 `ResolveSpecialDialoguePortrait(node)`，也就是优先回到 `000` 号条目
     - inner monologue 分支不再直接把头像关掉，而是同样调用 `ApplyPortrait(eventData.Node)`
  2. `NpcCharacterRegistry.asset`
     - 新增 `000` 条目：
       - `npcId = 000`
       - `canonicalName = 旅人`
       - `showInRelationshipPanel = 0`
       - `handPortrait = NPC_Hand/000.png`
     - 当前它只作为剧情玩家/旁白头像源，不进入关系页
  3. `NpcCharacterRegistryTests.cs`
     - 不再忽略 `旅人 / 陌生旅人`
     - 新增断言：
       - 这两个名字必须解析到 `000`
       - 对白头像必须能拿到 `000`
     - 新增源码护栏：
       - `DialogueUI` 必须包含玩家/旁白特殊头像解析入口
- 本轮验证：
  1. `validate_script Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
     - 首轮命中过 own red：
       - `StringComparison` 漏命名空间
     - 已修回
     - 复跑结果：
       - `owned_errors=0`
       - `external_errors=0`
       - `assessment=unity_validation_pending`
       - `manage_script validate = warning only`
  2. `validate_script Assets/YYY_Tests/Editor/NpcCharacterRegistryTests.cs`
     - `owned_errors=0`
     - `external_errors=0`
     - `assessment=unity_validation_pending`
     - `manage_script validate = clean`
  3. `errors`
     - 当前 console：`errors=0 warnings=0`
  4. `git diff --check`
     - 通过
     - 仅 `DialogueUI.cs` 存在 `CRLF -> LF` 提示
- 当前恢复点：
  - 这刀已经把正式剧情里的玩家台词和旁白头像统一到 `000.png`
  - 下一步如果继续，最值钱的是 live 看一眼：
    1. `旅人` 台词是否显示 `000`
    2. 内心独白是否也显示 `000`
    3. 不会误回到默认 NPC 图
