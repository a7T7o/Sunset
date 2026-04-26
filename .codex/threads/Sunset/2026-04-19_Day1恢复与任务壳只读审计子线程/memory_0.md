# Day1恢复与任务壳只读审计子线程 - 线程记忆

## 2026-04-19｜只读审计：读档/重新开始/默认开局后 Day1 任务壳残留
- 用户目标：
  - 在 `D:\Unity\Unity_learning\Sunset` 禁止改代码，只做静态代码审查，回答 Day1 在读档/重新开始/默认开局后为什么还会残留左侧任务卡、提示壳、交互提示、暂停源。
- 当前主线目标：
  - 审计 `Day1 恢复与任务壳` 的恢复链与 UI 壳 owner，给出“已正式清理的瞬时壳 / 仍可能残留的壳 / 第二天挂 Day1 任务卡修口是否足够 / 最小安全补法”。
- 本轮子任务：
  - 只读核对：
    - `SaveManager.cs`
    - `SpringDay1Director.cs`
    - `StoryProgressPersistenceService.cs`
    - `SpringDay1PromptOverlay.cs`
    - `InteractionHintOverlay.cs`
    - `DialogueUI.cs`
- 已完成事项：
  1. 钉实 `SaveManager.ResetTransientRuntimeForRestore()` 已经显式清掉一批瞬时壳：
     - 活跃对白
     - package/box/workbench
     - inventory held/tooltips
     - prompt/world hint/interaction hint
     - thought/npc bubble
     - `Dialogue` / `SpringDay1Director` time pause source
  2. 钉实“残留”主因不是单一 hide 漏口，而是恢复后又被 owner 重建：
     - `StoryProgressPersistenceService.ApplySpringDay1Progress()` 尾部会重新 `RestoreLoadedDayEndTaskCardState()` 与 `SyncStoryTimePauseState()`
     - `SpringDay1PromptOverlay.LateUpdate()` 会持续按 `BuildPromptCardModel()` 重建正式任务卡
     - `InteractionHintOverlay.LateUpdate()` 会持续按 placement/context 规则重算提示卡
  3. 钉实默认开局/原生重开当前本来就被重建到 `Town + EnterVillage`：
     - `ApplyNativeFreshRuntimeDefaults()` -> `ResetToTownOpeningRuntimeState()`
     - 所以 Day1 正式 task card 再出现，按现码更像“合同仍生效”，不是纯粹脏 UI 没清
  4. 钉实“第二天还挂 Day1 任务卡”的现有修口不够：
     - `RestoreLoadedDayEndTaskCardState()` 只处理 `_dayEnded && CurrentPhase == DayEnd`
     - `TryResolvePlayerFacingPhase()` 仍允许 `_dayEnded && (StoryManager == null || CurrentPhase == None)` 回退成 `DayEnd`
     - 缺的是 authoritative 的 Day1 玩家面失效总闸
- 关键决策：
  1. 不建议继续先补 `PromptOverlay.Hide()` / `InteractionHintOverlay.HideAllImmediate()` 这类 view 层症状补丁。
  2. 最小安全补法应先收在 `SpringDay1Director`：
     - 新建统一总闸，例如 `ShouldExposeDay1PlayerFacingUi()`
     - 统一接进 `TryResolvePlayerFacingPhase()` / `BuildPromptCardModel()` / `GetTaskListVisibilitySemanticState()` / 必要时 `GetTaskListBridgePromptDisplayState()`
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryProgressPersistenceService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\InteractionHintOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory.md`
- 验证结果：
  - 纯静态推断成立；未改代码，未跑 tests / Unity / live。
- 遗留问题或下一步：
  1. 如果后续进入真实施工，先做 Day1 玩家面生命周期总闸，不要先改壳层。
  2. 如果只继续审计，可进一步补查 `第二天 / 次日早晨` 场景里 `StoryManager.CurrentPhase` 与 `_dayEnded` 的真实切换点，确认总闸条件应落在 `phase`、`day index`、还是两者并用。

## 2026-04-19｜只读静态审计：导演态读档 blocker 与默认存档/F9 是否误回 Day1
- 用户目标：
  - 在 `D:\Unity\Unity_learning\Sunset` 继续只读静态审计，不改代码；主线服务“打包前收住存档/读档/重开”，本轮只回答两条风险：
    1. 剧情导演态禁止读档是否还有漏口
    2. 默认存档 / `F9` 是否还有可能被错误拉回 Day1
- 当前主线目标：
  - 钉实现有码上的 `load blocker` 合同，以及把“真正仍有风险的窗口”和“其实已分开的语义”分开。
- 本轮子任务：
  - 只读核对：
    - `SaveManager.cs`
    - `StoryProgressPersistenceService.cs`
    - `SpringDay1Director.cs`
    - `DialogueUI.cs`
    - `PackageSaveSettingsPanel.cs`
    - `SpringDay1NativeFreshRestartMenu.cs`
- 已完成事项：
  1. 钉实 `SaveManager` 的普通读档、默认槽读档、`F9` 快读都会先走 `CanExecutePlayerLoadAction()`，再串到 `StoryProgressPersistenceService.CanLoadNow()`。
  2. 钉实 `StoryProgressPersistenceService.CanLoadNow()` 已覆盖三层拦截：
     - `DialogueManager.IsDialogueActive`
     - `PlayerNpcChatSessionService.HasActiveConversation`
     - `SpringDay1Director.TryGetStorySaveLoadBlockReason()`
  3. 钉实“默认存档/F9 误拉回 Day1”在当前主链上没有直接证据：
     - `QuickLoadDefaultSlot()` / `LoadGame()` 最终是真读槽位文件
     - 只有 `RestartToFreshGame()` 才会进入 `ApplyNativeFreshRuntimeDefaults()` -> `ResetToTownOpeningRuntimeState()`
  4. 钉实真正仍可能漏掉的导演态读档窗口：
     - `HandleSleep()` 会先把 `_dayEnded=true`、`StoryPhase=DayEnd`
     - 再做 blink / `Home` 切场 / `ArmForcedSleepRestPlacementRetries()`
     - 但 `TryGetStorySaveLoadBlockReason()` 对 `DayEnd` 没有专门 blocker，默认直接放行
  5. 额外确认一个相邻风险：
     - `PackageSaveSettingsPanel` 的“重新开始”与 editor 菜单 `Restart Spring Day1 Native Fresh` 都直接调 `RestartToFreshGame()`
     - 当前没有复用导演态 blocker
- 关键决策：
  1. 不建议误把 `F9` 当成“还会偷偷 native restart”的问题点；当前代码里两条语义已经分开。
  2. 最小安全修法应优先补 `DayEnd / forced-sleep` 瞬时 blocker，而不是重写默认槽语义。
  3. 若后续要把“剧情接管中禁止切回 Day1 原生开局”也收严，应让 restart 入口复用同一个权威 blocker，而不是 UI 层各写一份判断。
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryProgressPersistenceService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Save\PackageSaveSettingsPanel.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1NativeFreshRestartMenu.cs`
- 验证结果：
  - 纯静态推断成立；未改代码，未跑 tests / Unity / live。
- 遗留问题或下一步：
  1. 如果后续进入真实施工，先补 `TryGetStorySaveLoadBlockReason()` 的 `DayEnd / forced-sleep` 收束窗口 blocker。
  2. 然后再决定是否让 `RestartToFreshGame()` 也复用同一 blocker。

## 2026-04-19｜只读静态审计：正式存档 payload 漏口复核（背包/装备/箱子/掉落物/农地/workbench/off-scene）
- 用户目标：
  - 在 `D:\Unity\Unity_learning\Sunset` 继续保持只读，不改代码，主线服务“打包前收住存档/读档/重开”，专审正式存档 payload 是否还有明显漏口。
- 当前主线目标：
  - 给出三段式审计结论：
    - A. 已正式入盘的部分
    - B. 还没正式闭环的真实漏项
    - C. 最小最安全修法排序
- 本轮子任务：
  - 只读核对：
    - `SaveManager.cs`
    - `SaveDataDTOs.cs`
    - `PersistentPlayerSceneBridge.cs`
    - `EquipmentService.cs`
    - `ChestController.cs`
    - `WorldItemPickup.cs`
    - `FarmTileManager.cs`
    - `CropController.cs`
    - `CraftingService.cs`
    - `SpringDay1WorkbenchCraftingOverlay.cs`
- 已完成事项：
  1. 钉实 `GameSaveData` 当前正式字段已经包含：
     - `player`
     - `inventory`（legacy 壳）
     - `worldObjects`
     - `cloudShadowScenes`
     - `offSceneWorldSnapshots`
  2. 钉实玩家背包当前 live 正式入口不是根 `inventory`，而是 `worldObjects` 中的 `PlayerInventory`；玩家选中态则在 `PlayerSaveData` 中保存 `selectedHotbarSlot + selectedInventoryIndex`。
  3. 钉实装备栏当前已经通过 `EquipmentService` 进入正式 payload，且 `SaveManager.ValidateRequiredSavePayloads()` 会在写盘前硬检查其存在。
  4. 钉实箱子当前已正式保存并恢复：
     - contents
     - `isLocked`
     - `origin`
     - `ownership`
     - `hasBeenLocked`
     - `currentHealth`
  5. 钉实地面掉落物当前已正式保存并恢复 `DropDataDTO.runtimeItem`，不是旧结论里那种“只剩 itemId/quality/amount/sourceGuid”。
  6. 钉实农地 / 作物当前已正式进入：
     - 当前 scene 的 `worldObjects`
     - off-scene 的 `offSceneWorldSnapshots`
     且 `PersistentPlayerSceneBridge` 已把 `FarmTileManager / CropController / ChestController / WorldItemPickup` 纳入 off-scene snapshot capture。
  7. 钉实 workbench 当前没有正式持久化制作队列：
     - `CraftingService` 没有 `IPersistentObject`
     - `SpringDay1WorkbenchCraftingOverlay` 的 `_queueEntries / _activeQueueEntry / readyCount / currentUnitProgress` 全是 runtime
     - `StoryProgressPersistenceService.CanSaveNow()` 明确以 blocker 方式阻止“制作中 / ready output / floating queue state”时保存
  8. 钉实当前真正还存在的合同漏口不是上面这些系统都没入盘，而是：
     - workbench 队列态仍未正式入盘
     - `ValidateRequiredSavePayloads()` 的硬守门只覆盖 `StoryProgressState / PlayerInventory / EquipmentService`
- 关键决策：
  1. 不再沿用旧结论把 `Chest`、`Drop.runtimeItem`、`off-scene snapshot` 列为“当前还没正式入盘”的主漏项。
  2. 当前最准确的第一漏项应改判为：
     - `workbench` 队列态是“显式不支持保存”，不是“已经正式持久化”
  3. 如果要打包前做最小最安全收口，优先应是：
     - 先保持/明确 `workbench` 的 blocker 语义
     - 再补更广覆盖面的 payload completeness guard
     - 而不是先去重做已经入盘的箱子/掉落物/off-scene DTO
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveDataDTOs.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PersistentPlayerSceneBridge.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Equipment\EquipmentService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\World\Placeable\ChestController.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\World\WorldItemPickup.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmTileManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\CropController.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Crafting\CraftingService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\StoryProgressPersistenceService.cs`
- 验证结果：
  - 纯静态推断成立；未改代码，未跑 tests / Unity / live。
- 遗留问题或下一步：
  1. 如果后续仍坚持“活跃 workbench 也要能存”，必须单开正式 DTO / restore 合同，不能误说成当前已支持。
  2. 如果打包前只求最小风险，则先补 payload completeness guard，让 `Chest / FarmTile / Crop / Drop / offSceneWorldSnapshots` 的缺失不再静默放行。

## 2026-04-19｜只读静态审计：20:00 后 resident 要玩家碰一下/挤一下才开始回家
- 用户目标：
  - 在 `D:\Unity\Unity_learning\Sunset` 保持只读，不改文件，只盯一个问题：为什么 20:00 后仍会出现 NPC 到点不自己走、要玩家碰一下/挤一下才开始回家。
- 当前主线目标：
  - 给出单一高概率根因链、对应方法/字段/分支，以及主线程最该先动的最小安全修口。
- 本轮子任务：
  1. 静态核对 `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
  2. 静态核对 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
  3. 必要补看 `SpringDay1ResidentControlProbeMenu.cs`、`SpringDay1LatePhaseValidationMenu.cs`、`NPCDialogueInteractable.cs`、`NPCInformalChatInteractable.cs`
- 已完成事项：
  1. 钉实 20:00 夜间调度并没有缺线：
     - `SyncResidentNightRestSchedule()` 在 `20 <= hour < 21` 时会走 `TryBeginResidentReturnHome()`
     - 再通过 `TryDriveResidentReturnHome()` 调 `NPCAutoRoamController.RequestReturnToAnchor()`
  2. 钉实“回家”用的是 `PointToPointTravelContract.FormalNavigation`，不是普通自由 roam。
  3. 钉实真正高概率失效点在 `NPCAutoRoamController` 的 blocked 恢复：
     - `TickMoving()`
     - `TryHandleBlockedAdvance()`
     - `TryHandleRecoverablePointToPointTravelBlockedAdvance()`
     - 当 `blockedAdvanceFrames >= FORMAL_NAVIGATION_BLOCKED_ABORT_MIN_FRAMES` 时会直接 `EndDebugMove(false)`
  4. 钉实这次 `EndDebugMove(false)` 会把 formal move 清掉，但 scripted control 仍可能保留，于是 controller 落入：
     - `IsResidentScriptedControlActive = true`
     - `IsResidentScriptedMoveActive = false`
     - `ShouldSuspendResidentRuntime() = true`
     表现上就是 NPC 冻住等下一次 crowd director 重发回家。
  5. 钉实 crowd director 的重发仍依赖当前位置重新建 formal path；如果 NPC 正被玩家/NPC 卡住，重发会持续失败。玩家“碰一下/挤一下”改变 clearance 之后，下一次请求才成功，所以现象看起来像“碰一下才启动”。
  6. 钉实现在已有现成 probe 可抓这个状态：
     - `SpringDay1ResidentControlProbeMenu`
     - 输出 `crowdSummary + ResidentControlProbeEntry`
     - 其中直接含 `scriptedControlActive / scriptedMoveActive / lastMoveSkipReason / blockedAdvanceFrames`
- 关键决策：
  1. 这轮不把锅主判给 20:00 常量或 Day1 时钟，而主判给 `FormalNavigation` 在近身阻挡下过早 abort。
  2. 最小安全修口优先级放在 `NPCAutoRoamController.TryHandleRecoverablePointToPointTravelBlockedAdvance()`，而不是先改 `SpringDay1NpcCrowdDirector` 的 schedule。
  3. 如果要先低风险证伪/证实，先抓 20:00 后的 resident probe，看 `lastMoveSkipReason` 是否稳定落到 `FormalNavigationBlockedAbort` 一带。
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1NpcCrowdDirector.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1ResidentControlProbeMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1LatePhaseValidationMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCDialogueInteractable.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCInformalChatInteractable.cs`
- 验证结果：
  - 纯静态推断成立；未改代码，未跑 tests / Unity / live。
- 遗留问题或下一步：
  1. 若后续进入真实施工，第一刀只收窄 `FormalNavigation` 的 blocked abort，不先扩到 autonomous roam 全链。
  2. 若第一刀后仍复现，再补 crowd director 对“夜间 pending return-home 失败类型”的分流与更快重发。

## 2026-04-19｜只读静态审计：001（村长）剧情外仍不能正常聊天
- 用户目标：
  - 在 `D:\Unity\Unity_learning\Sunset` 保持只读，不改文件，只盯一个问题：为什么 001（村长）在剧情外仍然不能正常聊天。
- 当前主线目标：
  - 给出单一高概率根因链、对应方法/字段/分支，以及主线程最该先动的最小安全修口。
- 本轮子任务：
  1. 静态核对 `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  2. 静态核对 `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
  3. 静态核对 `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
  4. 静态核对 `Assets/YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
  5. 必要补看 `NpcInteractionPriorityPolicy.cs` 与 001 的 roam / dialogue content 资产
- 已完成事项：
  1. 钉实 `NPCInformalChatInteractable.CanInteractWithResolvedSession()` 在 `RoamProfile == null || !RoamProfile.HasInformalConversationContent` 时会直接 return false。
  2. 钉实 `PlayerNpcChatSessionService.StartConversation()` 也要求能 resolve 出 playable bundle；拿不到 bundle 时会直接 return false。
  3. 钉实 `NPC_001_VillageChiefRoamProfile.asset` 的 `dialogueContentProfile` 指向 `NPC_001_VillageChiefDialogueContent.asset`。
  4. 钉实 001 的 dialogue content 里只有：
     - `playerNearbyLines`
     - `defaultChatInitiatorLines`
     - `defaultChatResponderLines`
     - `pairDialogueSets`
     但没有可供 `HasInformalConversationContent` 成立的 `defaultInformalConversationBundles / relationshipStageInformalChatSets / phaseInformalChatSets`。
  5. 钉实因此 001 的“剧情外聊天失败”会在闲聊 session 启动前就被挡住，不是先走到 `priority policy` 才失败。
  6. 额外确认：
     - `NpcInteractionPriorityPolicy` 在 `FreeTime` 默认不再把 informal interaction 视为 formal priority phase
     - 所以这轮首因不主判给 formal priority 抢占
- 关键决策：
  1. 这轮主判给 001 数据层缺少 informal conversation bundles，而不是 `SpringDay1Director` 或 `NpcInteractionPriorityPolicy` 的主逻辑。
  2. `resident scripted control` 在 `FreeTime` 里可能仍会形成第二层阻挡，但它不是这轮最先该动的口子。
  3. 最小安全修口优先级放在：
     - 先给 `NPC_001_VillageChiefDialogueContent.asset` 补最小一组 playable informal conversation bundle
     - 再复测是否还会被 resident control / return-home 逻辑二次拦截
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCDialogueInteractable.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCInformalChatInteractable.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NpcInteractionPriorityPolicy.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\NPCDialogueContentProfile.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\NPCRoamProfile.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_001_VillageChiefRoamProfile.asset`
  - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_001_VillageChiefDialogueContent.asset`
- 验证结果：
  - 纯静态推断成立；未改代码，未跑 tests / Unity / live。
- 遗留问题或下一步：
  1. 若后续进入真实施工，第一刀只补 001 的 informal bundles，不先碰 policy。
  2. 若补 bundle 后仍聊不起来，再专项复核 `IsResidentScriptedControlBlockingInformalInteraction()` 与 `IsFormalNavigationDriveActive()` 这条 FreeTime 放行链。
