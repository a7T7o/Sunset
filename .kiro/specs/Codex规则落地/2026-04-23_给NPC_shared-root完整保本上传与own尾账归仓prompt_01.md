请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root完整保本上传分发批次_01.md]

这轮不是继续修 resident、roam、摆头或导演边界。

你当前唯一主线固定为：
只做 `NPC` 当前本地 own 成果的 **完整保本上传**，把 clearly-own 的内容按最小批次安全归仓并推到 `origin`；不回退，不删改，不扩功能，不吞 shared/mixed。

你必须先继承并且不要推翻的当前真状态：
1. 你当前 `thread-state = PARKED`
2. 当前工作树里，和你最直接相关的本地尾账主要在：
   - `.codex/threads/Sunset/NPC/memory_0.md`
   - `.kiro/specs/NPC/*`
   - `Assets/111_Data/NPC/*`
   - `Assets/Editor/NPC/*`
   - `Assets/Resources/Story/NpcCharacterRegistry.asset`
   - `Assets/Sprites/NPC_Hand/*`
   - `Assets/YYY_Scripts/Controller/NPC/*`
3. 当前树上同时还混着 `SpringDay1NpcCrowdManifest`、导航、导演和 shared scene 合同；这些不默认算你这轮可吞面。

这轮必须完成的事情，按顺序严格执行：

第一部分：只重审你自己的 own 簇
1. 只重看：
   - NPC memory / specs
   - `Assets/111_Data/NPC/*`
   - `Assets/Editor/NPC/*`
   - `Assets/Resources/Story/NpcCharacterRegistry.asset`
   - `Assets/Sprites/NPC_Hand/*`
   - `Assets/YYY_Scripts/Controller/NPC/*`
2. 重新分成三类：
   - `A`： clearly own，直接提交
   - `B`： shared/mixed，不能吞
   - `C`：保留型证据或临时产物，必须报实，不准静默删

第二部分：只收 A
1. 对 `A` 类内容做最小白名单提交；必要时可拆 `2~3` 笔，但都只能是“当前成果原样归仓”。
2. 不准借这轮顺手继续修 `NPCAutoRoamController`、`NPCMotionController`、`NpcResidentRuntimeContract` 的行为逻辑。
3. 不准为了“提得更漂亮”而重做头像、profile、runtime contract 或 editor tool。

第三部分：明确禁止吞的面
1. 不要吞：
   - `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
   - `Assets/YYY_Scripts/Story/*`
   - `Assets/YYY_Scripts/Service/Navigation/*`
   - `Assets/000_Scenes/*`
   - `ProjectSettings/*`
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
2. 如果你发现 `NpcResidentRuntimeContract.cs / NpcResidentRuntimeSnapshot.cs` 已经明显撞到 `Day1` 或 `导航` 的 shared contract：
   - 不准一把吞
   - 直接报 exact blocker

第四部分：完成定义
1. 这轮完成只定义为：
   - 你的 clearly-own NPC 本地成果已提交并 push 到 `origin`
   - 或者被你精确报成 shared/mixed blocker
2. 必须明确回答：
   - 提交了哪些簇
   - 提交 SHA
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
