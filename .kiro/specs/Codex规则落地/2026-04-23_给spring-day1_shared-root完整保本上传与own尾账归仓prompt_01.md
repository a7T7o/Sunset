请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root完整保本上传分发批次_01.md]

这轮不是继续做 `day1 runtime`，也不是继续补导演或 UI 体验。

你当前唯一主线固定为：
只做 `spring-day1` 当前本地 own 成果的 **完整保本上传**，把 clearly-own 的内容按最小批次安全归仓并推到 `origin`；不回退，不删改，不顺手吞 shared/mixed。

你必须先继承并且不要推翻的当前真状态：
1. 你当前 `thread-state = PARKED`
2. 当前工作树里，和你最直接相关的本地尾账主要在：
   - `.codex/threads/Sunset/spring-day1/memory_0.md`
   - `.codex/threads/Sunset/Day1-V3/memory_0.md`
   - `.kiro/specs/900_开篇/spring-day1-implementation/*`
   - `Assets/Editor/Story/*`
   - `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
   - `Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs`
   - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
   - `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
   - 若干新增的 `SpringDay1*Menu` / `SpringDay1*Probe` / `Sunset*CleanupMenu`
3. 当前树上还混着 `Story/UI`、`SaveManager` 相关测试和 shared 基础链；这些不默认算你 own。

这轮必须完成的事情，按顺序严格执行：

第一部分：先重新审你这轮真实 own 面
1. 只重看你自己的以下簇：
   - `spring-day1` 线程 memory
   - `900_开篇/spring-day1-implementation`
   - `Assets/Editor/Story/*`
   - `Assets/Resources/Story/SpringDay1/*`
   - `Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs`
   - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
   - `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
2. 把当前本地内容分成三类：
   - `A`： clearly own，应该这轮直接提交
   - `B`： shared/mixed，不该由你吞
   - `C`：像备份/探针/截图一类的保留项，需要你明确报实，不准静默删

第二部分：只收 A，不碰 B
1. 对 `A` 类内容，直接做最小白名单提交；如果 1 笔太大，允许拆成 `2~3` 笔，但都必须是“原样归仓”，不是顺手整理。
2. 这轮不允许继续改业务逻辑，不允许为了“看起来更干净”去重写、补口、重构、补测试。
3. 如果某个 clearly-own 文件当前就是你本地真实成果的一部分，即使还粗糙，也按“原样成果”看待，不要擅自重做。

第三部分：明确禁止吞的东西
1. 不要吞：
   - `Assets/YYY_Scripts/Story/UI/*`
   - `Assets/YYY_Scripts/UI/*`
   - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
   - `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
   - `Assets/YYY_Tests/Editor/SaveManager*`
   - `Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
   - `Assets/YYY_Tests/Editor/WorkbenchInventoryRefreshContractTests.cs`
   - `Assets/000_Scenes/*`
   - `ProjectSettings/*`
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
2. 如果你发现当前自己认为是 own 的文件其实已经撞进上面这些 shared/mixed 面：
   - 不准硬提
   - 直接在回执里报 exact file + why mixed

第四部分：完成定义
1. 本轮完成不等于“git status 全干净”。
2. 本轮完成只定义为：
   - 你的 clearly-own 本地成果已提交并 push 到 `origin`
   - 或者被你精确报成 shared/mixed blocker
3. 最终必须明确回答：
   - 本轮提交了哪些簇
   - 本轮提交 SHA
   - 是否已 push 到 `origin`
   - 当前 own 路径是否 clean
   - 如果不是 `yes`，还剩哪些 exact blocker files

第五部分：最终回执格式
1. 直接对用户回复：
   - `A1 保底六点卡`
   - `A2 用户补充层（可选）`
   - `B 技术审计层`
2. `B 技术审计层` 额外必须写：
   - `提交 SHA`
   - `push 状态`
   - `当前 own 路径是否 clean`
   - `remaining blocker files`

[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 旧线程不用废弃重开；但最晚必须在“下一次真实继续施工前”补这次 `Begin-Slice`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：
- 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 如果还没跑，原因是什么
- 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住

这不是建议，是当前 live 规则的一部分。除非这轮始终停留在只读分析，否则不要跳过。
