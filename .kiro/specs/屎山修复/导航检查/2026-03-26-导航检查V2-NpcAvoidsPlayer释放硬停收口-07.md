# 2026-03-26-导航检查V2-NpcAvoidsPlayer释放硬停收口-07

你现在继续作为 `导航检查V2` 实现线程施工。

这轮不是回去继续讲 `导航V2` 锐评，也不是让你顺手处理 `Primary.unity / TMP 字体 / NPC scene` 底盘卫生。

## 当前已接受的基线

1. `导航V2` 的审核支线已经收口，不再继续占用实现入口。
2. detour owner 玩家侧最小保活窗口已成立：
   - `RealInputPlayerAvoidsMovingNpc` 已拿到有效执行窗口。
3. 你上一轮在 NPC 侧补了最小 release 处理后，拿到了一条有效失败样本：
   - `NpcAvoidsPlayer pass=False timeout=6.50 minClearance=-0.003 npcReached=False`
4. 你上一轮 own 路径状态是：
   - `no`
   这意味着这轮不能再漂成“继续改点东西再说”，而必须把失败 checkpoint 作为同一刀收口。

## 用户最新补充

1. 用户刚刚也在开 Unity。
2. `NPC` 线程也有过短暂使用。

这不自动等于“上一条 fresh 结果无效”，但它意味着：

- 你下一次 live 只允许在明确拿到短独占窗口时再跑；
- 如果拿不到，就停在代码 / 静态证据层回执，不要硬跑争议样本。

## 这轮唯一主刀

只做：

> 锁死 `NPCAutoRoamController.TickMoving()` 里 `ClearedOverrideWaypoint` 的 NPC 侧 release 硬停链

更具体地说，这轮只允许你判断并处理下面这一件事：

1. 你上一轮新增的：
   - `waypointState.ClearedOverrideWaypoint -> StopMotion() -> rb.linearVelocity = 0 -> return`
   是否就是当前 `NpcAvoidsPlayer npcReached=False` 的第一责任点。
2. 如果是：
   - 只做最小补口，让 NPC 在 detour clear 后恢复到正确的主路径推进语义，而不是被你自己钉在原地。
3. 如果不是：
   - 你必须给出比“NPC 侧 owner 还没过”更窄的单一第一责任点；
   - 但仍然只能留在同一 `NPCAutoRoamController` release / recover 链里，不准扩回 solver 或大架构。

## 允许的 scope

这轮只允许读取和必要时修改这些范围：

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2-NpcAvoidsPlayer释放硬停收口-07.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2复工准入后续工委托-06.md`
3. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\memory_0.md`
4. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
5. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
6. 如确属必要，只读对照：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationPathExecutor2D.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NavigationAvoidanceRulesTests.cs`

## 明确禁止

1. 不准回漂 `NavigationLocalAvoidanceSolver` 权重。
2. 不准重开 `TrafficArbiter / MotionCommand / DynamicNavigationAgent / 全量 linearVelocity`。
3. 不准顺手处理 `Primary.unity`、TMP 字体、`NPC/NPCV2` scene hygiene。
4. 不准把这轮扩成 `NpcNpcCrossing` 或三场同轮。
5. 不准把“用户刚刚也开着 Unity”直接偷换成“上一条失败样本作废”。
6. 不准再让 `NPCAutoRoamController.cs` 作为 own dirty 挂着不收口就继续报下一轮 feature 进展。

## 完成定义

只有满足下面任一结局，这轮才算有效完成：

### 结局 A：单场 fresh 过线

1. 你已最小修通 `ClearedOverrideWaypoint` release 硬停链；
2. 你已拿到 1 条 fresh `NpcAvoidsPlayer pass=True`；
3. 你已在拿到结果后立刻 `Stop` 并退回 `Edit Mode`；
4. 你已对白名单 own 路径完成自收口，不再留 `NPCAutoRoamController.cs` 漂在 working tree。

### 结局 B：仍未过线，但失败形态继续压缩

1. 你明确证明“`ClearedOverrideWaypoint` 硬停链”不是责任点，或已经被修掉；
2. 当前新的第一责任点比“NPC 侧 owner 还没进入有效窗口”更窄、更直接；
3. 你没有把刀漂回 solver / scene / font / NPC 别线；
4. 即使仍 fail，你也要把本轮 failed checkpoint 白名单收口，不准继续留 own dirty。

## live 纪律

1. 最多只跑 1 条 `NpcAvoidsPlayer`。
2. 开跑前先确认当前 Editor 有短独占窗口；如果没有，就不要硬跑。
3. 一旦拿到 `scenario_end`，立刻 `Stop`。
4. 完成后必须退回 `Edit Mode`。

## 固定回执格式

```text
已回写文件路径：
当前在改什么：
`NpcAvoidsPlayer` 最新 fresh 结果：
`ClearedOverrideWaypoint` 硬停链是否为第一责任点：`yes / no`
changed_paths：
当前 own 路径是否 clean：
blocker_or_checkpoint：
一句话摘要：
```

## 一句话总口径

这轮不要再横向发散，就锁 `NPCAutoRoamController` 自己那条 detour clear 后的硬停 early-return 链，把 NPC 侧 failure 继续压窄，并把 own dirty 一并收掉。
