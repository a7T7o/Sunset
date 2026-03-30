# NpcAvoidsPlayer 自主闭环冲刺 - 详细汇报

## 本轮改了什么功能点

### 1. 补上 override 被清掉后的 recover 接力

- 文件：`D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
- 改动：
  - `TickMoving()` 里如果 `EvaluateWaypoint()` 返回 `ClearedOverrideWaypoint=true`，现在会先补一次 `TryReleaseSharedAvoidanceDetour(avoidancePosition, Time.time)`，再结束当前帧。
- 为什么要改：
  - `NavigationPathExecutor2D.EvaluateWaypoint()` 在接近 override waypoint 时会先把 override 清掉；
  - 但如果这时没有把 detour 的 `recover + ResetProgressState` 接上，NPC 侧会留下“override 已清、recover 未完成”的半状态。

### 2. 让 release helper 能消费一次“刚清掉但还没 recover”的 detour 上下文

- 文件：`D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
- 改动：
  - `TryReleaseSharedAvoidanceDetour(...)` 不再只接受 `hasDynamicDetour=true`；
  - 现在只要还存在“尚未 recover 的 detour 上下文”，就允许补做一次 recover。
- 为什么要改：
  - `ClearOverrideWaypoint()` 会把 `LastDetourOwnerId / LastDetourPoint` 留下来，并把 `LastDetourRecoverySucceeded` 置回 `false`；
  - 这本来就是为“清掉 override 后再补 recover”准备的状态，如果 helper 只认 `HasOverrideWaypoint`，这段状态就永远吃不到。

### 3. release / recover 成功时恢复当帧硬停返回

- 文件：`D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
- 改动：
  - 当 `detour.Cleared || detour.Recovered` 成功时，恢复 `StopMotion + rb.linearVelocity = Vector2.zero + return true`。
- 为什么要改：
  - 这条 slice 当前锁的是“release 后给 owner 留一个有效恢复窗口”；
  - recover 成功后当帧硬停，能避免继续沿着旧 direction / 旧速度把状态又带回混乱区。

## 这轮实际验证了什么

### 1. 恢复了非 MCP 的 fresh 触发路径

- 这轮没有再被 `Unity session not available` 卡死。
- 我直接枚举并命中了 Unity 原生菜单：
  - `工具 > Sunset > 导航 > Probe Setup > NPC Avoids Player`
  - 命令 ID：`40980`
- 触发后拿到：
  - `queued_action=SetupNpcAvoidsPlayer entering_play_mode`
  - `runtime_launch_request=SetupNpcAvoidsPlayer`
  - `scenario_start=NpcAvoidsPlayer`
  - `scenario_end=NpcAvoidsPlayer ...`
- 每次拿到 `scenario_end` 后都立刻补发停止播放，最终确认回到 Edit 现场。

### 2. 验证了补口已经真正编译进 Unity

- `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 通过。
- `Editor.log` 新增：
  - `34986:*** Tundra build success (3.61 seconds), 9 items updated, 862 evaluated`

### 3. 验证了 patch 后 fresh 结果

- baseline（补口前）：
  - `34741:[NavValidation] scenario_end=NpcAvoidsPlayer pass=False timeout=6.51 minClearance=0.826 npcReached=False`
- patch 后 fresh：
  - `35610:[NavValidation] scenario_end=NpcAvoidsPlayer pass=False timeout=6.51 minClearance=0.832 npcReached=False`

## 这轮失败解释是什么

### 1. 现在已经不是“没有 fresh”或“recover 分支完全没补上”

- 这轮 fresh 已经拿到，所以不是外部 blocker。
- 补口也已经编译生效，所以不是“代码没进 Unity”。

### 2. 但它也没有把 failure 翻过去

- patch 后结果仍然是：
  - `minClearance` 正常
  - `npcReached=False`
- 这说明当前主要 failure 不是“NPC 贴身撞穿玩家”；
- 更像是 NPC 仍然在 avoidance/detour 的循环态里打转，始终没有形成有效的 release 后续推进。

### 3. 因此第一责任点继续前移了

- 本轮之前怀疑的是：
  - `ClearedOverrideWaypoint` 之后 recover 没接上。
- 本轮 patch 后 fresh 仍同型失败，说明这已经不是最前面的责任点。
- 当前更小的责任点应继续锁在：
  - `TryHandleSharedAvoidance()` 内
  - `!avoidance.HasBlockingAgent -> TryReleaseSharedAvoidanceDetour(... rebuildPath:false)`
  - 也就是 release 窗口是否真正形成、还是始终被持续 blocking 吞掉。

## 用户现在可以怎么测

### 1. 复现方式

- 在 Unity 菜单执行：
  - `工具 > Sunset > 导航 > Probe Setup > NPC Avoids Player`

### 2. 预期现象

- 如果这条 slice 真闭环，NPC `001` 应该绕开静止玩家并继续推进到目标点；
- `Editor.log` 应出现：
  - `scenario_end=NpcAvoidsPlayer pass=True`
  - 或至少出现新的 failure 形态，而不是继续停在 `npcReached=False` 的同型失败。

### 3. 当前实际现象

- 目前仍是：
  - `scenario_end=NpcAvoidsPlayer pass=False ... npcReached=False`

## 下一步会继续打哪一刀

- 仍然只打同一条 slice。
- 下一刀只该继续确认：
  - `TryHandleSharedAvoidance()` 里的 release 窗口为什么没有稳定形成
  - 或 release 入口是否已被触达但马上又被重新判回 blocking
- 不会回漂到这些禁区：
  - `Primary.unity`
  - 字体
  - `NPCAutoRoamControllerEditor.cs`
  - `HomeAnchor`
  - solver 泛调
  - 大架构
  - broad cleanup
