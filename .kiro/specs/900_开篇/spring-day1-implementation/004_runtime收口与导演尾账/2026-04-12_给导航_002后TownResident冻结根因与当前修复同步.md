## 当前同步目的

这不是让你接手修 `spring-day1`。

这份同步只为了把这次 `002` 后 `Town resident` 整片冻结/不动的 fresh 结论告诉你，避免后续再把它误判回“导航 core 泛坏”。

## 这次 fresh 现场我实际钉到什么

这次最关键的 live 证据不是“NPC 根本不会走”，而是：

- `Town` scene 里的 resident 实例其实还在
- 但 [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 runtime registry 会在某些重建/承接拍上掉成 `spawned=0 / _spawnStates=0`
- scene 里的 `101~203` 因为没有重新 bind 回 crowd runtime，最后停在 `Inactive / not roaming`

也就是说，这轮用户看到的“除了 `001~003` / 小动物，其他 NPC 都像冻结一样站住”，第一真锅不在导航 core 本体，而在 `spring-day1` own 的 crowd runtime 恢复漏口。

## 我这轮做了什么

### 1. 先把取证链修到可用

- 新增/修好 [SpringDay1ActorRuntimeProbeMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1ActorRuntimeProbeMenu.cs)
- 修掉了 probe 的编译红
- 修掉了 probe 误把 `Baby Chicken Yellow (1)` 之类动物认成 `001`
- 现在只会在严格 `001/NPC001/NPC_001` 或明确 Day1 resident root 下认定为目标 NPC

### 2. 新增晚段快速跳转工具

- 新增 [SpringDay1LatePhaseValidationMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1LatePhaseValidationMenu.cs)
- 提供：
  - `Force Spring Day1 Dinner Validation Jump`
  - `Force Spring Day1 FreeTime Validation Jump`

这两个都是 editor-only，纯为 live 复盘提速，不改 runtime 业务语义。

### 3. 在 Day1 runtime 本体上补恢复刀

我在 [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 加了：

- `ShouldRecoverMissingTownResidents(...)`

并在 `Update()` 前段加了一层 Town rebind 恢复：

- 当前 active scene 是 `Town`
- `_spawnStates.Count == 0`
- `StoryManager / SpringDay1Director / manifest` 都已就位
- 但 Town scene 里仍然能找到 resident root 或 manifest resident 实例

满足这些条件时，就主动 `SyncCrowd()`，把 scene resident 重新 bind 回 crowd runtime。

## 现在已经站住的判断

1. `002` 后 Town resident 整片卡死，这次不是优先像导航 own
2. 更准确地说，是 `SpringDay1NpcCrowdDirector` 的 runtime registry 恢复漏口
3. 现场证据链是：
   - scene resident 还在
   - crowd summary 掉成 `spawned=0|missing=101~203`
   - resident probe 显示它们变成 `isRoaming=false / isMoving=false / roamState=Inactive`

## 当前结果

- CLI fresh console：`errors=0`
- 用户当前 live 口头反馈：`npc 已经可以走了`

所以我这轮先停在这里，不继续扩第二刀。

## 如果后面还复发，最该往哪查

如果用户后面再报同类“Town resident 整片又冻结”，下一步最值钱的方向不是回导航泛排查，而是继续顺着下面这条链查：

- [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
- scene transition 后 runtime registry 为什么会掉
- cue release / resident re-entry / crowd rebuild 时机有没有再次漏掉 rebind

## 一句话结论

这次 `002` 后 Town resident 冻结，我已经用 fresh probe 钉到是 `spring-day1` own 的 crowd runtime registry 恢复漏口，并已经在 `SpringDay1NpcCrowdDirector.Update()` 里补了 Town resident rebind 恢复刀；当前用户 live 口头反馈是 `npc 已经可以走了`。
