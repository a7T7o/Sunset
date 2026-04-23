请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root完整保本上传分发批次_01.md]

这轮不是继续修导航坏 case，也不是继续追 NPC/Day1 contract。

你当前唯一主线固定为：
只做 `导航检查` 当前本地 own 成果的 **完整保本上传**，把 clearly-own 的内容按最小批次安全归仓并推到 `origin`；不回退，不删改，不扩功能，不吞 shared/mixed。

你必须先继承并且不要推翻的当前真状态：
1. 你当前 `thread-state = ACTIVE`
   - `town-live-nav-own-roam-quality-audit`
2. 当前工作树里，和你最直接相关的本地尾账主要在：
   - `.codex/threads/Sunset/导航检查/*`
   - `.codex/threads/Sunset/导航V3/*`
   - `.kiro/specs/999_全面重构_26.03.15/导航检查/memory.md`
   - `.kiro/specs/屎山修复/导航V3/memory.md`
   - `.kiro/specs/屎山修复/导航检查/*`
   - `Assets/YYY_Scripts/Service/Navigation/*`
   - `Assets/Editor/Navigation*`
   - `Assets/YYY_Scripts/Controller/NPC/NpcLocomotionSurfaceAttribute.cs`
3. 当前树上还混着 `NPCMotionController / NPCAutoRoamController / SpringDay1 crowd / scenes / ProjectSettings` 的 shared/mixed 外线；这些不默认算你可吞面。

这轮必须完成的事情，按顺序严格执行：

第一部分：先处理你当前 ACTIVE 现场
1. 先判断当前 `ACTIVE` slice 是否直接服务于“本轮 own 上传”。
2. 如果不是：
   - 先合法 `Park-Slice` 旧 slice
   - 再为本轮上传动作新开上传 slice
3. 不允许一边继续功能施工，一边顺手做上传。

第二部分：只重审你自己的 own 簇
1. 只重看：
   - 导航 memory / specs / prompt
   - `Assets/YYY_Scripts/Service/Navigation/*`
   - `Assets/Editor/Navigation*`
   - `Assets/YYY_Scripts/Controller/NPC/NpcLocomotionSurfaceAttribute.cs`
2. 分成三类：
   - `A`： clearly own，直接提交
   - `B`： shared/mixed，不能吞
   - `C`：保留型 docs/prompt/证据，必须报实，不准静默删

第三部分：只收 A
1. 对 `A` 类内容做最小白名单提交；必要时可拆 `2~3` 笔，但都必须是“原样归仓”。
2. 不准借这轮继续改导航逻辑、继续补 traversal、继续收 NPC facing/roam。
3. 当前 clearly-own 的导航 docs/prompt 也属于本地成果的一部分，不能漏提。

第四部分：明确禁止吞的面
1. 不要吞：
   - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
   - `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
   - `Assets/YYY_Scripts/Story/*`
   - `Assets/000_Scenes/*`
   - `ProjectSettings/*`
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
2. 如果你发现当前 `NavigationAgentRegistry.cs / NavGrid2D.cs / StairLayerTransitionZone2D.cs` 已经和 scene-side 或 NPC own contract 混到不能独立提交：
   - 不准硬吞
   - 直接报 exact blocker

第五部分：完成定义
1. 这轮完成只定义为：
   - clearly-own 的导航线本地成果已提交并 push 到 `origin`
   - 或者被你精确报成 shared/mixed blocker
2. 必须明确回答：
   - 提交了哪些簇
   - 提交 SHA
   - 是否已 push 到 `origin`
   - 当前 own 路径是否 clean
   - 如果不是 `yes`，还剩哪些 exact blocker files

第六部分：最终回执格式
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
