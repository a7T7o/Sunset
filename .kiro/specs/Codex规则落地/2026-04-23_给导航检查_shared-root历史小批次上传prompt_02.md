请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root历史小批次上传分发批次_02.md]

这轮不要继续修导航坏 case，也不要回到第一波“docs-only 上传完就先停”的口径。

你当前唯一主线固定为：
只按过去实际开发历史，给 `导航检查` 再还原 `1` 个最小历史小批次上传尝试；这轮只允许这一小批，撞 blocker 就停车，不换第二批。

你必须先继承并且不要推翻的当前真状态：
1. 你第一波已真实 push：
   - `91c99ec7` `docs: upload navigation own handoff artifacts`
   - `5195633a` `docs: record navigation upload checkpoint`
2. 你当前 `thread-state = PARKED`
3. 第一波 docs-only 上传已成立
4. 当前真实代码 blocker 仍在：
   - `Assets/Editor/NavigationStaticPointValidationMenu.cs`
   - `Assets/Editor/NavigationAvoidanceRulesValidationMenu.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavGrid2DStressTest.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs`
   - `Assets/YYY_Scripts/Service/Navigation/StairLayerTransitionZone2D.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs.meta`
   - `Assets/YYY_Scripts/Controller/NPC/NpcLocomotionSurfaceAttribute.cs`
   - `Assets/YYY_Scripts/Controller/NPC/NpcLocomotionSurfaceAttribute.cs.meta`
5. 你自己已经明确：这些代码一旦白名单过宽，就会把 own roots 扩成 `Assets/Editor`、`Assets/YYY_Scripts/Service/Navigation`、`Assets/YYY_Scripts/Controller/NPC` 并撞外线 mixed。

这轮唯一允许的小批次固定为：
只拿最小新增台阶/层切换小批做一次真实尝试，不准把旧的 NavGrid / Registry / NPC 属性一起带进来：

- `Assets/YYY_Scripts/Service/Navigation/StairLayerTransitionZone2D.cs`
- `Assets/YYY_Scripts/Service/Navigation/StairLayerTransitionZone2D.cs.meta`
- `Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs.meta`

这轮必须按顺序执行：
1. 先确认这组是不是一个真实历史小批，而不是你为上传临时拼的探测包。
2. 只对白名单这一组跑真实上传尝试。
3. 如果即便这组仍然导致 own roots 扩成大根并撞 mixed，立刻停车。
4. 不准因为被卡就顺手吞：
   - `NavGrid2D.cs`
   - `NavGrid2DStressTest.cs`
   - `NavigationAgentRegistry.cs`
   - `NavigationStaticPointValidationMenu.cs`
   - `NavigationAvoidanceRulesValidationMenu.cs`
   - `NpcLocomotionSurfaceAttribute.cs`
5. 这轮不准继续第二个小批次。

完成定义只有两种：
1. 这组台阶/层切换小批真实提交并 push 到 `origin`
2. 或者你把这一个小批次的 exact blocker 报死

最终回执必须额外明确：
1. 这组小批是不是独立历史批次
2. 第一真实 blocker 到底是 own-root 扩根、mixed，还是别的
3. 这轮没有去动第二组导航代码

[thread-state 接线要求｜本轮强制]
如果你这轮会继续真实上传尝试，先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `sync` 前，必须先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住或收口，跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
