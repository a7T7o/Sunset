# NPC - 线程活跃记忆

> 2026-04-10 起，旧线程母卷已归档到 [memory_1.md](D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/NPC/memory_1.md)。本卷只保留当前线程角色和恢复点。

## 线程定位
- 线程名称：`NPC`
- 线程作用：NPC 主线的分流执行入口

## 当前主线
- resident / 关系页 / 头像真源：
  - [2.1.0_resident与关系页收口](D:/Unity/Unity_learning/Sunset/.kiro/specs/NPC/2.1.0_resident与关系页收口/memory.md)
- 自漫游 / 避让 / motion：
  - [2.2.0_自漫游与避让收口](D:/Unity/Unity_learning/Sunset/.kiro/specs/NPC/2.2.0_自漫游与避让收口/memory.md)

## 当前恢复点
- 查旧全量过程、历史回执、共享根清扫材料时看 `memory_1.md`
- 查当前问题时先按内容层 / 行为层分流，不再回到单大卷

## 2026-04-10｜只读审稿补记：导航线程当前说法可采纳一半，但不能混题
- 当前主线目标：
  - 用户要求我直接判断 NPC 导航 / 朝向多轮返工里，到底哪些结论可信、哪些在混题，以及 NPC 线程现在最该提供什么建设性帮助。
- 本轮子任务：
  - 只读回看 `导航检查` 最新 memory 与相关代码；
  - 不进真实施工，不跑 `Begin-Slice`；
  - 重点审两件事：
    1. `scene continuity`
    2. `free-roam 摇摆/摆头鬼畜`
- 本轮稳定结论：
  1. `scene continuity`：
     - 导航线程方向大体正确；
     - 应接现有 `PersistentPlayerSceneBridge / NpcResidentRuntimeContract / SpringDay1NpcCrowdDirector resident snapshot`，不是另起一套完全平行大桥。
  2. `摇摆/摆头鬼畜`：
     - 不是同一个根因；
     - 当前仓库里仍有多处外部 `SetFacingDirection(...)` 写入：
       - `SpringDay1NpcCrowdDirector.cs`
       - `NPCInformalChatInteractable.cs`
       - `NPCAutoRoamController.cs`
     - 因此“导航线程修了 auto-roam 一刀”不等于全项目已经止住用户看到的摇摆。
  3. 责任边界：
     - 这件事和 NPC 线程有关；
     - 但当前不是 NPC 单独独占锅，已经是 NPC + Day1 crowd/director + chat 的 shared/mixed 面。
- 当前恢复点：
  - 如果用户让我继续真实施工，最值钱的第一刀不是立刻扩 `scene continuity bridge`；
  - 而是先在 `NPCMotionController.SetFacingDirection(...)` 做窄口调用源追踪，把“谁在抢脸”钉成 live 证据，再决定 contract 应该收在哪一层。

## 2026-04-10｜只读审稿补记：历史“会走会避让但卡”不能整包回滚
- 当前主线目标：
  - 用户要求我进一步回答：导航线程当前到底在修什么，为什么问题久修不住，以及“历史版本导航还能不能恢复”。
- 本轮子任务：
  - 只读看 `git log / git diff / 当前 dirty`；
  - 不进真实施工，不碰导航线程现场；
  - 重点审：
    1. 最近高风险提交窗口
    2. 当前导航线程真正改动域
    3. `NPC roam` 是否已被其它 owner 语义冻结
- 本轮稳定结论：
  1. 当前导航线程不只是改 `NavigationTraversalCore`，同时还在改：
     - `NPCAutoRoamController`
     - `NPCMotionController`
     - `NavGrid2D`
     - `TraversalBlockManager2D`
     - `PlayerAutoNavigator`
  2. 这说明当前不是单纯“底层导航核坏了”，而是导航 / NPC roam / 玩家入口一起在动。
  3. `NPCAutoRoamController` 当前已经带显式 `resident scripted control` 栈：
     - `AcquireResidentScriptedControl`
     - `ReleaseResidentScriptedControl`
     - `HaltResidentScriptedMovement`
     - `ShouldSuspendResidentRuntime`
     - 这层如果 release / resume 语义不稳，就会直接把 NPC 卡成“不卡但不走”。
  4. 历史版本不能直接整包回滚：
     - `17708cba` 把 player/NPC traversal 合并；
     - `4c736a7a` 又在上面叠了 runtime harden；
     - 当前 Day1 resident/director 还在消费新的控制语义；
     - 所以全回滚会把现在整个 shared contract 一起扯坏。
- 当前恢复点：
  - 如果继续分析，最值钱的是继续只读审 `resident scripted control` 的 owner/release 调用链；
  - 如果以后再允许我真实施工，我才会去做最窄证据钉死或最小恢复方案，不会越过导航线程去抢改。

## 2026-04-10｜补记：导航回执当前只站住“显示层增强”，没站住“现实层已解”
- 用户关键纠正：
  - `现实不是显示`
- 本轮更明确的线程判断：
  1. 导航这次对 `NPCMotionController` 的处理，最多只说明：
     - 日志更细
     - burst 式翻脸更容易抓
     - 朝向对瞬时噪声不再那么敏感
  2. 这不等于它已经回答：
     - 谁让 NPC 进入“不走/冻结”
     - 谁在拿住 roam 控制权不放
     - 为什么历史“会走但卡”退化成现在“不卡但傻”
  3. 当前我对“现实层”的最稳判断仍是：
     - `resident scripted control` 冻结/释放语义
     - 多 owner `StartRoam / StopRoam / SetFacingDirection`
     - stop-loss / harden 提前把坏态压成 freeze

## 2026-04-10｜已给导航线程起单刀 prompt：只追现实层冻结链
- 当前主线目标：
  - 用户认可应给导航线程发新 prompt，但要求这次不要再围绕显示层兜圈，而是直接把现实层问题钉死。
- 本轮子任务：
  - 进入真实施工前已跑 `Begin-Slice`；
  - 只新建一份 prompt 文件，不做代码修改，不做 sync；
  - 起刀后已跑 `Park-Slice` 收回现场。
- 本轮完成：
  - 已新建 prompt：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-10_导航线程_现实层冻结链与resident-scripted-control追责prompt_65.md`
  - prompt 核心裁定：
    1. 不再把 `NPCMotionController` 显示层增强当主刀；
    2. 只追 `NPCAutoRoamController` 的 `resident scripted control` acquire / halt / release / resume 现实链；
    3. 如果第一真问题落在 Day1 caller，就报 exact caller chain，不越权代修。
- thread-state：
  - `Begin-Slice=已跑`
  - `Ready-To-Sync=未跑（本轮未进入 sync）`
  - `Park-Slice=已跑`
  - 当前 `PARKED`
- 当前恢复点：
  - 用户现在可以直接转发这份 prompt 给导航线程；
  - 我这轮不再继续扩写，等待导航线程按新刀口回复。

## 2026-04-11｜只读考古补记：4/3 可用版是“用户认可窗口”，真实代码基线应拆成 handoff truth / 实际 SHA / 后续退化 三层
- 当前主线目标：
  - 用户追问：之前让导航“彻底提交”的那个可用版本到底在哪里、有没有 memory 和真实提交记录，以及之前和现在到底差在哪，方便后续更快回退和修复。
- 本轮子任务：
  - 只读回看导航线程 handoff 文档、memory 和 git 历史；
  - 不进真实施工，不跑 `Begin-Slice`。
- 本轮稳定结论：
  1. 4/3 的“可用版本”是存在的，但在导航线程里首先被记录成：
     - `用户已认可当前导航版本`
     - `settled 的是 handoff，不是 sync`
     - `v2-handoff-complete-but-sync-still-blocked`
     所以它不是“4/3 当天新增了一个独立 runtime sync SHA”，而是“用户真实体验认可窗口 + 之前已有提交组成的代码树”。
  2. 真正构成那版代码底子的关键提交是：
     - `acfc7f27`：`NavigationLiveValidationRunner.cs` + `NavigationLocalAvoidanceSolver.cs`
     - `6a6db43e`：`PlayerAutoNavigator.cs`
     - `cc0bc7f8`：早期 `NPCAutoRoamController.cs` 小改
  3. 如果要找一个最方便直接 `checkout/diff` 的整仓基线，当前最实用的是：
     - `343c3910`
     - 它仍处在 `7e06c2e6` 之前，属于 accepted-window 后、NPC/static-audit 大扩张前的快照。
  4. 当前坏相真正的分水岭是：
     - `7e06c2e6`
     - 然后继续叠加：
       - `7cd57279`
       - `263f4ed0`
       - `bf386811`
       - `c29a80a2`
       - `592705f8`
       - `767d389b`
       - `aeaea9a0`
       - `17708cba`
       - `4c736a7a`
  5. “之前为什么可以”：
     - 被用户认可的核心是玩家导航入口体验；
     - 那时 NPC free-roam/shared traversal/resident scripted control 还没被后面这些大改深度缠到一起。
  6. “现在为什么不可以”：
     - 4/4 之后大量共享底层和 NPC roam 改动叠加；
     - 当前 NPC 问题不是单一导航核坏了，而是：
       - shared traversal core
       - NPC 自己的 stuck/recovery/hard-stop
       - resident scripted control / Day1 caller
       三层互相影响。
- 当前恢复点：
  - 如果后续继续只读支持用户调度，我现在最该拿来做回退/修复对照的双基线是：
    - `343c3910`：accepted-window 单一整仓快照
    - `7e06c2e6`：大规模共享/NPC 扩张起点
  - 如果后续允许我继续分析，我下一步会优先只做：
    - `343c3910..HEAD` 在 `NPCAutoRoamController / NavGrid2D / PlayerMovement / NPCMotionController / NavigationTraversalCore / TraversalBlockManager2D` 的精确责任差异表。
