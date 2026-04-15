# 2.2.0 - 自漫游与避让收口

## 模块概述
- 承接 NPC 的行为层与移动层：
  - obstacle 自动收集
  - avoidance 响应恢复
  - `NPCAutoRoamController / NPCMotionController` 收薄
  - scene-side 漫游体验终验

## 当前稳定结论
- 当前坏点更偏 driver 过重与响应窗口失真，而不是 NPC 整体系统不存在
- 方向稳定、shared avoidance、detour、blocked 状态必须继续分清
- 后续最值钱的是最小 live 验，而不是盲改更多算法

## 当前恢复点
- 后续 roam / avoidance / motion 问题统一先归这里


## 2026-04-10｜导航/NPC 边界审稿：scene continuity 与 摇摆鬼畜必须拆开
- 用户目标：
  - 用户因 NPC 导航 / 朝向问题被连续多轮修复拖住，希望我直接判断：导航线程这轮说法到底对不对、这锅和 NPC 线有没有关系、以及现在最有建设性的下一步是什么。
- 本轮方式：
  - 只读审稿；未进真实施工，未跑 `Begin-Slice`。
- 本轮核对：
  - 线程 / 工作区记忆：
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\memory_0.md`
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md`
  - 代码：
    - `PersistentPlayerSceneBridge.cs`
    - `NpcResidentRuntimeContract.cs`
    - `SpringDay1NpcCrowdDirector.cs`
    - `NPCMotionController.cs`
    - `NPCAutoRoamController.cs`
    - `NPCInformalChatInteractable.cs`
- 当前稳定判断：
  1. `scene continuity` 问题：
     - 导航线程提出“切场前抓 resident/NPC 运行态，回场后恢复”的方向是对的；
     - 但更稳的落法不是平地再造一座独立桥，而是接到现有 `PersistentPlayerSceneBridge + resident snapshot` 链上。
  2. 用户当前最痛的 `free-roam 摇摆 / 摆头鬼畜`：
     - 这不是同一个问题，不能被 `scene continuity` 方案混掉；
     - 当前代码里仍有多个外部朝向写入源：
       - `SpringDay1NpcCrowdDirector.cs`
       - `NPCInformalChatInteractable.cs`
       - `NPCAutoRoamController.cs`
     - 所以“auto-roam 自身朝向修过一刀”不等于“全项目 NPC 朝向已经闭环”。
  3. 责任判断：
     - 这事和 `NPC` 线有关；
     - 但不是 `NPC` 单线程纯独占锅，当前已经带明显 shared / mixed 特征，至少涉及 `Day1 crowd/director` 和 `chat` 链。
- 当前最值钱的下一刀：
  - 不要继续让 `scene continuity` 冒充“摇摆鬼畜已修好”；
  - 如果继续真实施工，先做极窄调试面：
    - 在 `NPCMotionController.SetFacingDirection(...)` 上打调用源追踪；
    - 记录 `npcId/name + 实际移动方向 + 当前写入朝向 + caller/source + 当前状态(roam/director/chat)`；
    - 先把“谁在抢脸”钉死，再决定后续 contract 收口。

## 2026-04-10｜历史退化审计：现在不是“导航没了”，而是三层语义缠死了
- 用户目标：
  - 用户进一步追问：导航也在持续施工，这锅到底是不是导航的；为什么这么久没解决；以及“历史版本那种会走、会避让、只是有点卡”的状态还能不能恢复。
- 本轮方式：
  - 只读审计；未进真实施工，未跑 `Begin-Slice`。
- 本轮额外核对：
  - `git log` 关键提交：
    - `17708cba` `refactor: unify traversal core for player and npc`
    - `4c736a7a` `harden navigation runtime: throttle path rebuild and reduce blocker query overhead`
    - `592705f8` `npc-spin-hardstop-fix`
    - `aeaea9a0` `fix npc traversal bridge movement stall`
  - 当前 dirty diff：
    - `NPCAutoRoamController.cs`
    - `NPCMotionController.cs`
    - `NavGrid2D.cs`
    - `NavigationTraversalCore.cs`
    - `TraversalBlockManager2D.cs`
    - `PlayerAutoNavigator.cs`
- 当前稳定判断：
  1. 导航线程现在不是只在修“纯底层导航核”，它同时在动：
     - 共享 traversal / occupancy 查询
     - NPC roam stop-loss / hard-stop / path-build backoff
     - 玩家 scene 绑定口
  2. 最近最危险的不是单一算法回退，而是 `NPCAutoRoamController` 自己被塞进了 `resident scripted control` 语义：
     - 有 `AcquireResidentScriptedControl / ReleaseResidentScriptedControl / HaltResidentScriptedMovement / ShouldSuspendResidentRuntime`
     - 这会直接把 NPC 从 roam 逻辑里冻结出去；
     - 如果 owner release / resume 没收平，就会表现成“不卡，但不走 / 傻站 / 只剩乱摆”。
  3. 所以当前退化不是一个单点 bug，而是三层缠死：
     - `shared traversal core`
     - `NPC auto-roam 自己的 stop-loss / recovery`
     - `Day1 resident/director scripted control`
- 关于“历史版本恢复”的判断：
  - 不能简单整包回滚；
  - 因为 `17708cba -> 4c736a7a` 之间已经把 player/NPC shared traversal 和当前 Day1 resident 依赖绑到一起；
  - 真要恢复，正确做法应是：
    1. 先锁定“历史体感较好”的 checkpoint 窗口；
    2. 再只抽取 `NPC roam` 相关行为语义；
    3. 不能把整个当前导航/Day1/shared contract 一起拉回去。
- 当前恢复点：
  - 如果后续继续只读深挖，最值钱的不是再猜 solver；
  - 而是做一次 `NPCAutoRoamController resident scripted control` 调用链审计，确认谁拿了控制权、谁没释放、谁把 roam 挂成了冻结态。

## 2026-04-10｜补记：用户明确纠正“现实不是显示”
- 用户进一步纠正：
  - 日志/显示不是现实，不接受把“诊断输出补上了”包装成问题已经被回答。
- 当前更明确的判断：
  1. 导航线程这次补的 `NPCMotionController` 日志和 burst mismatch 过滤有价值，但它只属于“观测层 + 症状层缓和”。
  2. 它没有直接回答当前最现实的问题：
     - 为什么 NPC 会进入“不卡但不走 / 傻站 / 乱摆”
  3. 结合现有代码，现实层更像：
     - `NPCAutoRoamController` 的 `resident scripted control` 冻结链
     - `StartRoam / StopRoam / SetFacingDirection` 多 owner 并存
     - `shared traversal + stop-loss` 把坏状态更早压成 freeze/abort
  4. 所以现在不能把“显示更清楚了”误当成“现实已经修好”。
- 当前恢复点：
  - 后续对用户解释时，必须明确区分：
    - `观测层已增强`
    - `现实层根因仍在 owner/freeze/resume 语义`

## 2026-04-11｜历史可用窗口考古：4/3 认可的是 handoff truth，不是当天新增单一导航 sync
- 用户目标：
  - 用户要求我彻底找出：之前让导航“彻底提交那个可用版本”到底落在哪些文档、哪些 memory、哪些真实提交；
  - 并进一步判断：之前为什么可以、现在为什么不可以，方便后续更快回退或做最小恢复。
- 本轮方式：
  - 只读考古；未进真实施工，未跑 `Begin-Slice`。
- 本轮核对：
  - 文档 / memory：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-03-导航检查V2-用户已认可当前导航版本仅收最终验收交接与可归仓判定-52.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-03-导航检查V2-保持停车并冻结最终验收交接只等专用cleanup再唤醒-58.md`
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\memory_1.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory_0.md`
  - git：
    - `acfc7f27`
    - `6a6db43e`
    - `cc0bc7f8`
    - `343c3910`
    - `7e06c2e6`
    - `7cd57279`
    - `263f4ed0`
    - `bf386811`
    - `c29a80a2`
    - `592705f8`
    - `767d389b`
    - `aeaea9a0`
    - `17708cba`
    - `4c736a7a`
- 当前稳定判断：
  1. 用户口中的“4/3 可用版本”在 memory 里确实存在，但它被记录成：
     - `用户已认可当前导航版本`
     - `导航检查V2 settled 的是 handoff，不是 sync`
     - `v2-handoff-complete-but-sync-still-blocked`
     也就是说：它首先是一个**被用户认可的体验窗口 truth**，不是“4/3 当天又新做了一次单独 runtime sync 提交”。
  2. 如果只问“那版真实代码现在到底落在哪”：
     - 玩家导航 accepted 核心更接近：
       - `acfc7f27`：`NavigationLiveValidationRunner.cs` + `NavigationLocalAvoidanceSolver.cs`
       - `6a6db43e`：`PlayerAutoNavigator.cs`
     - 同期 NPC 侧只看到较早且较轻的：
       - `cc0bc7f8`：早期 `NPCAutoRoamController.cs` 小改
  3. 如果只想找一个**整仓可回看的单一 SHA**，最实用的候选不是 4/3 文案本身，而是：
     - `343c3910`
     - 原因：它是 `7e06c2e6` 这次“大规模 NPC/static-audit 扩张”前，导航线程自己最后一个可见 checkpoint；
     - 且从 accepted 文档窗口到 `343c3910`，运行时代码没有再发生新的导航/NPC runtime 扩张。
  4. 4/4 之后真正把事情带进现在这团 mixed/shared 乱局的起点，是：
     - `7e06c2e6`
     - 它一次性重改：
       - `NPCAutoRoamController.cs`
       - `NavGrid2D.cs`
       - `TraversalBlockManager2D.cs`
       - `PlayerAutoNavigator.cs`
       - `PlayerMovement.cs`
     - 之后又接连叠了：
       - `7cd57279 / 263f4ed0 / bf386811 / c29a80a2 / 592705f8 / 767d389b / aeaea9a0`
       - 再到 `17708cba` 把 player/NPC traversal core 合并
       - 再到 `4c736a7a` 做 runtime harden
  5. 所以“之前为什么可以、现在为什么不可以”的最稳解释是：
     - 之前被认可的主要是**玩家导航入口体验**；
     - 当时 targeted probe 仍有红面，但已被降级成 polish 诊断项；
     - 而 NPC free-roam / resident scripted control / shared traversal core 这三层，在 4/4 之后才开始被深度缠到一起。
  6. 现在不好，不是因为“4/3 认可版其实不存在”，而是因为：
     - 后续新增了大量 `NPCAutoRoamController / NavGrid2D / PlayerMovement / shared traversal core` 改动；
     - NPC 线当前坏相更像：
       - player/NPC 共用 traversal
       - NPC 自己的 stuck/hard-stop/recovery
       - resident scripted control / Day1 caller
       三层互相叠死。
- 当前恢复点：
  - 如果后续要做最小恢复或对照，不该先整包乱回滚；
  - 最值钱的基线是：
    1. 用 `343c3910` 当 accepted-window 的单一整仓快照；
    2. 用 `7e06c2e6` 当“开始大规模把 NPC 和 shared traversal 拧在一起”的分水岭；
    3. 然后只对 `NPCAutoRoamController / NavGrid2D / PlayerMovement / NPCMotionController / NavigationTraversalCore / TraversalBlockManager2D` 做窄 diff 和最小恢复。

## 2026-04-14｜打包报错责任拆解：NPC 线当前不是 build blocker，本线只命中 player-warning
- 用户目标：
  - 用户贴了 Player build 日志，要求我只拆跟 NPC 线有关的问题，明确“我的问题是什么”，并给出彻底的解决方案。
- 本轮方式：
  - 只读排查 `DayNightManager.cs`、`NPCMotionController.cs` 和同批 warning 文件；
  - 不进真实施工，不跑 `Begin-Slice`。
- 本轮稳定结论：
  1. 当前真正拦打包的 fatal blocker 不是 `NPC` 导航/漫游线，而是：
     - `Assets/YYY_Scripts/Service/Rendering/DayNightManager.cs:208`
     - `Start()` 里直接调用了 `EditorRefreshNow()`；
     - 但 `EditorRefreshNow()` 定义在 `#if UNITY_EDITOR` 内；
     - 所以 Editor 里不一定炸，Player build 会直接 `CS0103`。
  2. 跟 `NPC` 线直接相关的是：
     - `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
     - `_facingMismatchFrameCount`
     - `_facingMismatchBurstCount`
     - `_lastFacingMismatchLogTime`
     - `_lastFacingMismatchSeenTime`
     这 4 个字段的 `CS0414` warning。
  3. 这 4 个 warning 不是 build blocker；
     - 根因是这些字段只服务 editor diagnostic；
     - 在 Player build 里只剩“赋值”，没有真正读取，所以编译器报“assigned but never used”。
  4. 其它 `drawGizmos / showGizmos / previewInEditor / _preGenerateId` warning 也是同一类：
     - editor-only gizmo / preview / OnValidate 字段；
     - 不是当前 packaging fail 的首要根因。
- 当前恢复点：
  - 如果后续允许我真实施工，NPC 线该收的是 `NPCMotionController` 这批 editor-only diagnostic 字段；
  - 但真正要先清掉的打包 blocker，是 `DayNightManager` 的 editor-only 调用泄漏。

## 2026-04-14｜最小补丁已下：只收 `NPCMotionController` 的 4 条 player-warning
- 用户目标：
  - 用户要求我先修掉属于我这条 `NPC` 线的 warning，而且必须是最安全、最小、不影响原逻辑的补丁。
- 本轮方式：
  - 进入真实施工前已跑 `Begin-Slice`；
  - 只改 `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`；
  - 不扩到 `NPCAutoRoamController`、`DayNightManager` 或别的 shared 根。
- 本轮完成：
  1. 已把这 4 个仅供 editor 诊断使用的字段收进 `#if UNITY_EDITOR`：
     - `_facingMismatchFrameCount`
     - `_facingMismatchBurstCount`
     - `_lastFacingMismatchLogTime`
     - `_lastFacingMismatchSeenTime`
  2. 已把 `ResetFacingMismatchDiagnostics()` 改成：
     - editor 下按原逻辑重置
     - player 下空操作
- 当前稳定判断：
  - 这是一刀纯编译面清理；
  - 没改移动、朝向、动画判定、roam、外部 facing owner 链；
  - 语义上比 `#pragma warning disable` 更稳，因为直接把 editor-only 状态从 player 编译面拿掉了。
- 验证：
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`：通过
  - `validate_script NPCMotionController.cs`：
    - `owned_errors=0`
    - 当前只剩 external red，来源是 `DayNightOverlay.cs`
    - `manage_script validate` 只剩 2 条旧式泛 warning，不再是这 4 条 `CS0414`
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑（本轮未准备 sync）`
  - `Park-Slice=已跑`
  - 当前 `PARKED`
- 当前恢复点：
  - 这 4 条属于 `NPCMotionController` 的 player-warning 现在已经按最小方式收掉；
  - 后续如果要继续清包，先处理外部 `DayNightOverlay / DayNightManager` 红面，再决定是否把其它 editor-only warning 一并治理。

## 2026-04-14｜最小 warning 补丁尾验完成
- 已补跑审计：
  - `check-skill-trigger-log-health.ps1` => `Health: ok`
  - `validate_script NPCMotionController.cs`
- 最新判断：
  - `owned_errors=0`
  - `current_warnings=0`
  - assessment=`unity_validation_pending`
  - 原因是 Unity `stale_status`，不是 `NPCMotionController` 新红
- 当前恢复点：
  - 这刀的目标已经完成：`NPC` 自己的 4 条 `CS0414` warning 已被安全移出 player 编译面；
  - 继续往下收时，应转向外部 render/day-night 问题，而不是继续回头返工这把刀。
