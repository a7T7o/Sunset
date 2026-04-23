请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root第二波blocker分流批次_03.md]

不要再重跑你刚刚已经执行完的 `prompt_02`。

你当前唯一主线固定为：
承认“楼梯层级切换三件套”那一刀已经撞死；这轮改成 `Assets/YYY_Scripts/Service/Navigation` 根内整合批的唯一上传尝试，把上一刀已证实挡路的 `NavGrid2D / NavGrid2DStressTest / NavigationAgentRegistry` 正式纳入，但仍然不扩到 `Editor` 菜单或 `NPC` 属性文件。

你必须先继承并且不要推翻的当前真状态：
1. 上一刀 `prompt_02` 已经完成，不准重复再撞一次。
2. 当前 `thread-state = PARKED`
3. 上一刀第一真实 blocker 已钉死为：
   - `Assets/YYY_Scripts/Service/Navigation` 父根扩根
4. 当前 exact blocker files 已明确是：
   - [NavGrid2D.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs)
   - [NavGrid2DStressTest.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2DStressTest.cs)
   - [NavigationAgentRegistry.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs)

这轮唯一允许的切片固定为：
- [StairLayerTransitionZone2D.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/StairLayerTransitionZone2D.cs)
- [StairLayerTransitionZone2D.cs.meta](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/StairLayerTransitionZone2D.cs.meta)
- [NavigationTraversalCore.cs.meta](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs.meta)
- [NavGrid2D.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs)
- [NavGrid2DStressTest.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2DStressTest.cs)
- [NavigationAgentRegistry.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs)

这轮必须按顺序执行：
1. 先确认这批现在能不能诚实视作 `Service/Navigation` 根内整合批，而不是今天为了过根临时拼的包。
2. 只对白名单这一组跑一次真实上传尝试。
3. 如果这组一进白名单后，`Assets/YYY_Scripts/Service/Navigation` 根内还有新的 remaining dirty / mixed 没被覆盖：
   - 立刻停车
   - 只报新的 exact blocker
4. 不准因此顺手吞：
   - [NavigationStaticPointValidationMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/NavigationStaticPointValidationMenu.cs)
   - [NavigationAvoidanceRulesValidationMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/NavigationAvoidanceRulesValidationMenu.cs)
   - [NpcLocomotionSurfaceAttribute.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NpcLocomotionSurfaceAttribute.cs)
5. 不准继续第二个切片。

这轮完成定义只有两种：
1. 这组 `Service/Navigation` 根内整合批真实提交并 push 到 `origin`
2. 或者你把新的 exact blocker 报死

最终回执必须额外明确：
1. 这批现在是否正式升级为 `Service/Navigation` 根内整合批
2. 这组有没有提交成功
3. 如果还失败，新的第一真实 blocker 是什么
4. 这轮没有越权扩到 `Editor` 菜单和 `NPC` 属性文件

[thread-state 接线要求｜本轮强制]
如果你这轮会继续真实上传尝试，先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `sync` 前，必须先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住或收口，跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
