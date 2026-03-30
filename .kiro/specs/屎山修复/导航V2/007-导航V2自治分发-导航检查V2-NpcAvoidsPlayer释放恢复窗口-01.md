# 007-导航V2自治分发-导航检查V2-NpcAvoidsPlayer释放恢复窗口-01

你现在继续作为 `导航检查V2` 实现线程施工。

这份委托来自当前 `导航V2 owner` 的自治分发，  
不是父线程替你转述，也不是新的大架构讨论入口。

## 当前已接受基线

1. 上位依据固定为：
   - `006-Sunset专业导航系统需求与架构设计.md`
   - `007-Sunset专业导航底座后续开发路线图.md`
   - `002-导航V2开发宪法与阶段推进纲领.md`
   - `004-导航V2接班准入与自治规约.md`
   - `005-导航V2偏差账本.md`
2. 当前第一责任点已经不再是：
   - `TickMoving()` 中 `ClearedOverrideWaypoint -> StopMotion -> linearVelocity = 0`
   这段 NPC 专有硬停副作用；
   这段只算前一轮已排掉的责任点。
3. 当前单一第一阻塞点继续压窄为：
   - `TryReleaseSharedAvoidanceDetour(... rebuildPath:false)`
   - `-> TryHandleSharedAvoidance() return true`
   - `-> TickMoving()` 当帧直接 `return`
   这条 NPC 侧 detour release / recover 链没有给 owner 留出稳定恢复窗口。
4. 当前冻结项不变：
   - `Primary.unity` mixed cleanup
   - `DialogueChinese*` 字体 cleanup
   - `TrafficArbiter / MotionCommand / DynamicNavigationAgent`
   - broad scene hygiene
   - 长窗 live / 多场同轮

## 这轮唯一主刀

只做：

> 继续锁 NPC 侧 detour release 后的恢复窗口，让 owner 在 release 之后至少稳定活过一个有效 execute window

更具体地说，这轮只允许你围绕下面这条链继续压窄并在必要时打一刀最小补口：

1. `TryReleaseSharedAvoidanceDetour(... rebuildPath:false)`
2. `TryHandleSharedAvoidance()`
3. `TickMoving()` 当帧 `return`

## 先完整读取

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\005-导航V2偏差账本.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2-NpcAvoidsPlayer第一责任点钉死与最小事务-09.md`
3. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\memory_0.md`
4. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
5. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
6. 如确属必要，只读对照：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationPathExecutor2D.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`

## 允许的 scope

这轮只允许你读取和必要时修改这些范围：

1. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\memory_0.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
3. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
4. 如确属必要，允许修改：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationPathExecutor2D.cs`
5. 如确属必要，只读：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NavigationAvoidanceRulesTests.cs`

## 明确禁止

1. 不准回漂 `NavigationLocalAvoidanceSolver` 权重。
2. 不准重开 `TrafficArbiter / MotionCommand / DynamicNavigationAgent / 全量 linearVelocity`。
3. 不准碰 `Primary.unity`、字体、`NPCAutoRoamControllerEditor.cs` 或 `NPCV2` 线。
4. 不准把这轮扩成 `NpcNpcCrossing`、三场同轮或 broad cleanup。
5. 不准把 `ClearedOverrideWaypoint` 已排掉的前一责任点重新包装成当前唯一主刀。
6. 不准先开长窗 live；最多只跑 1 条 `NpcAvoidsPlayer` fresh。

## 完成定义

只有满足下面任一结局，这轮才算有效完成：

### 结局 A：恢复窗口被钉死并完成最小事务

1. 你已明确说明 release 后恢复窗口当前究竟被哪一个分支吞掉；
2. 你只改了那一个最小 runtime 分支；
3. 你已最多跑 1 条 `NpcAvoidsPlayer` fresh；
4. 拿到 `scenario_end` 立刻 `Stop`，退回 `Edit Mode`；
5. 你已把本轮 own 路径收回到可说明状态。

### 结局 B：责任点继续压窄，但本轮仍不安全改

1. 你明确证明当前还不能安全下刀；
2. 但你把责任点继续从整条 release 链压到更小、更直接的单分支；
3. 你没有回漂别的议题；
4. 你没有新增无关 dirty。

## live 纪律

1. 只有在 release 后恢复窗口已经被钉死、且你真的做了最小补口后，才允许跑 1 条 fresh。
2. 如果这轮还没把恢复窗口钉死，就停在只读 / 静态证据层，不要硬跑。
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

这轮继续只打 NPC 侧 detour release 后的恢复窗口，不要回头再炒前一责任点，也不要漂回 solver、大架构、scene 或 cleanup。
