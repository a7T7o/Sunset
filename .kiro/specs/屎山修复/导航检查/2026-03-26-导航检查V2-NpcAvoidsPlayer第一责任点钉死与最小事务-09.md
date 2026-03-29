# 2026-03-26-导航检查V2-NpcAvoidsPlayer第一责任点钉死与最小事务-09

你现在继续作为 `导航检查V2` 实现线程施工。

这轮请以你自己最新回执里的真实判断为准：  
当前不是再泛泛说“NPC 侧 owner 还没过”，也不是我替你预设某一小段代码一定就是唯一凶手。  
你已经把问题收缩到 `NPCAutoRoamController.cs` 同一条 detour `release / recover` 执行链里，这轮就沿这条线继续，不要漂走。

## 当前已接受的基线

1. 总主线没有变：
   - 真实右键导航里的 runtime 过线；
   - 当前你这条子线的职责，是继续把 `NpcAvoidsPlayer` 的 NPC 侧不过线问题打穿。
2. `导航V2` 审核支线已停：
   - Gemini 可吸收的是问题意识，不是新的施工上位法。
3. 玩家侧 accepted checkpoint 保留：
   - `RealInputPlayerAvoidsMovingNpc` 已拿到有效执行窗口。
4. NPC 侧上一条 fresh 仍是：
   - `NpcAvoidsPlayer pass=False timeout=6.50 minClearance=-0.003 npcReached=False`
5. 你自己最新判断已经把责任簇压到这三段相邻逻辑：
   - [TickMoving()](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L558)
   - [TryHandleSharedAvoidance()](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L873)
   - [TryReleaseSharedAvoidanceDetour()](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1019)

## 这轮唯一主刀

只做：

> 先只读钉死 `NpcAvoidsPlayer` 当前第一责任点；如果责任点在本轮被钉死，再只改那一个最小 runtime 分支，并最多跑 1 条 fresh

更具体地说，这轮只允许你在下面两类嫌疑里做裁决：

1. [TickMoving()](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L580) 里这段：
   - `ClearedOverrideWaypoint -> StopMotion -> linearVelocity = 0 -> return`
2. 或者更完整的一整段 release 链：
   - [TryReleaseSharedAvoidanceDetour()](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1026)
   - -> [TryHandleSharedAvoidance()](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L898)
   - -> [TickMoving()](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L621)

## 先完整读取

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2-NpcAvoidsPlayer执行链硬停与握手续工-08.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2-NpcAvoidsPlayer释放硬停收口-07.md`
3. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\memory_0.md`
4. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
5. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
6. 如确属必要，只读对照：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationPathExecutor2D.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NavigationAvoidanceRulesTests.cs`

## 允许的 scope

这轮只允许你读取和必要时修改这些范围：

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2-NpcAvoidsPlayer第一责任点钉死与最小事务-09.md`
2. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\memory_0.md`
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
4. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
5. 如确属必要，只读：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationPathExecutor2D.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NavigationAvoidanceRulesTests.cs`

## 明确禁止

1. 不准回漂 `NavigationLocalAvoidanceSolver` 权重。
2. 不准重开 `TrafficArbiter / MotionCommand / DynamicNavigationAgent / 全量 linearVelocity`。
3. 不准碰 `Primary.unity`、字体、`NPCAutoRoamControllerEditor.cs` 或 `NPCV2` 线。
4. 不准把这轮扩成 `NpcNpcCrossing`、三场同轮或 broad cleanup。
5. 不准先拍脑袋改代码；本轮第一步必须是把第一责任点钉死。
6. 不准为了“多看一点”继续长窗 live；最多只跑 1 条 `NpcAvoidsPlayer`。

## 完成定义

只有满足下面任一结局，这轮才算有效完成：

### 结局 A：责任点钉死并完成最小事务

1. 你已明确判定当前第一责任点是上面两类嫌疑中的哪一类；
2. 你只改了那一个最小 runtime 分支；
3. 你已最多跑 1 条 `NpcAvoidsPlayer` fresh；
4. 拿到 `scenario_end` 立刻 `Stop`，退回 `Edit Mode`；
5. 你已把本轮 own 路径收回到可说明状态。

### 结局 B：责任点被继续压窄，但本轮仍不安全改

1. 你明确证明当前还不能安全下刀；
2. 但你把第一责任点从“整条 release 链都可疑”压到一个更小、更直接的分支；
3. 你没有回漂别的议题；
4. 你没有新增无关 dirty。

## live 纪律

1. 只有在第一责任点已经被钉死、且你真的做了最小补口后，才允许跑 1 条 fresh。
2. 如果第一责任点这轮还没钉死，就停在只读 / 静态证据层，不要硬跑。
3. 一旦拿到 `scenario_end`，立刻 `Stop`。
4. 完成后必须退回 `Edit Mode`。

## 固定回执格式

```text
已回写文件路径：
当前在改什么：
当前第一责任点：
这轮是否已进入最小 runtime 补口：`yes / no`
`NpcAvoidsPlayer` 最新 fresh 结果：
changed_paths：
当前 own 路径是否 clean：
blocker_or_checkpoint：
一句话摘要：
```

## 一句话总口径

这轮不要再一边诊断一边乱补；先把 `NPCAutoRoamController` 这条 detour `release / recover` 执行链上的第一责任点钉死，钉死后才允许做一个最小 runtime 事务。
