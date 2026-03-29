# 2026-03-26-导航检查V2-NpcAvoidsPlayer执行链硬停与握手续工-08

你现在继续作为 `导航检查V2` 实现线程施工。

这轮不是回去继续讲 `导航V2` 锐评，也不是把 `NPCV2` 刚修掉的 `NPCAutoRoamControllerEditor.cs @ 24886aad` 误写成 runtime 导航已经被修好。

## 当前已接受的基线

1. `导航V2` 审核支线已经收口：
   - `000-gemini锐评-1.0.md` 的 `Path B` 边界已冻结；
   - `000-gemini锐评-1.1.md` 继续维持 `Path C`；
   - Gemini 可吸收的是问题意识，不是新的上位施工蓝图。
2. 当前实现主线仍然只认执行层握手：
   - `ShouldRepath` / detour owner 是否真的稳定接管 `PlayerAutoNavigator / NPCAutoRoamController / NavigationPathExecutor2D`。
3. 玩家侧 accepted checkpoint 保留：
   - `RealInputPlayerAvoidsMovingNpc` 已拿到首个有效执行窗口。
4. 你上一轮在 NPC 侧补的最小口之后，拿到的是有效失败样本：
   - `NpcAvoidsPlayer pass=False timeout=6.50 minClearance=-0.003 npcReached=False`
5. 当前 runtime 热点仍在你自己这条线：
   - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 还在 working tree 里 dirty；
   - `NPCAutoRoamControllerEditor.cs @ 24886aad` 只修了 Inspector / Play Mode 的 `MarkSceneDirty` 报错，不属于这轮 runtime 导航过线证据。

## 这轮唯一主刀

只做：

> 锁死 `NPCAutoRoamController.TickMoving()` 里 `ClearedOverrideWaypoint` 的硬停 early-return，继续验证 detour owner / release 握手是否因此在 NPC 侧断掉

更具体地说：

1. 你上一轮新增的：
   - `waypointState.ClearedOverrideWaypoint -> StopMotion() -> rb.linearVelocity = 0 -> return`
   当前是第一嫌疑点。
2. 这轮只允许围绕它和紧邻的共享 release / recover 链判断：
   - 它是不是把 NPC 在 detour clear 后直接钉停，导致既没恢复主路径推进，也没重新进入正确的 owner 接管窗口。
3. 如果是：
   - 只做最小修通，不准顺手扩成别的导航议题。
4. 如果不是：
   - 你必须把第一责任点继续压窄到同一条 `NPCAutoRoamController / NavigationPathExecutor2D` 执行链里的更小分支；
   - 仍然不准回漂 solver、大架构、`TrafficArbiter / MotionCommand`。

## 先完整读取

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2复工准入后续工委托-06.md`
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

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2-NpcAvoidsPlayer执行链硬停与握手续工-08.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2-NpcAvoidsPlayer释放硬停收口-07.md`
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2复工准入后续工委托-06.md`
4. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\memory_0.md`
5. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
6. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
7. 如确属必要，只读对照：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationPathExecutor2D.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NavigationAvoidanceRulesTests.cs`

## 明确禁止

1. 不准把 `NPCAutoRoamControllerEditor.cs @ 24886aad` 当成本轮 runtime 修复。
2. 不准回漂 `NavigationLocalAvoidanceSolver` 权重。
3. 不准重开 `TrafficArbiter / MotionCommand / DynamicNavigationAgent / 全量 linearVelocity`。
4. 不准顺手处理 `Primary.unity`、TMP 字体或 `NPCV2` 的底盘 hygiene。
5. 不准把这轮扩成 `NpcNpcCrossing` 或三场同轮。
6. 不准为了取证继续长窗 live；最多只跑 1 条 `NpcAvoidsPlayer`。
7. 不准继续留 `NPCAutoRoamController.cs` own dirty 却把这轮包装成已闭环。

## 完成定义

只有满足下面任一结局，这轮才算有效完成：

### 结局 A：单场 fresh 过线

1. 你已最小改掉或下线 `ClearedOverrideWaypoint` 的硬停 early-return；
2. 你已拿到 1 条 fresh `NpcAvoidsPlayer pass=True`；
3. 你已在拿到 `scenario_end` 后立刻 `Stop` 并退回 `Edit Mode`；
4. 你已对白名单 own 路径完成自收口，不再留 `NPCAutoRoamController.cs` 漂在 working tree。

### 结局 B：仍未过线，但失败形态继续压缩

1. 你明确证明当前这段硬停链已经不是第一责任点，或已被修掉；
2. 新的第一责任点比“NPC 侧 owner 还没过”更窄，而且仍在同一条 `NPCAutoRoamController / NavigationPathExecutor2D` 执行链里；
3. 你没有回漂 solver / scene / font / 审核支线；
4. 即使仍 fail，你也把本轮 failed checkpoint 白名单收口，不准继续留 own dirty。

## live 纪律

1. 最多只跑 1 条 `NpcAvoidsPlayer`。
2. 开跑前先确认当前 Editor 有短独占窗口；如果没有，就停在代码 / 静态证据层回执。
3. 一旦拿到 `scenario_end`，立刻 `Stop`。
4. 完成后必须退回 `Edit Mode`。

## 固定回执格式

```text
已回写文件路径：
当前在改什么：
`NpcAvoidsPlayer` 最新 fresh 结果：
`TickMoving` 里的 `ClearedOverrideWaypoint -> StopMotion -> linearVelocity=0 -> return` 是否已被下线或改写：`yes / no`
当前新的第一责任点：
changed_paths：
当前 own 路径是否 clean：
blocker_or_checkpoint：
一句话摘要：
```

## 一句话总口径

这轮继续只打执行层握手，不要再横向发散；把 `NPCAutoRoamController` 自己那条 detour clear 后的硬停 early-return 链打穿，并把 own dirty 一并收掉。
