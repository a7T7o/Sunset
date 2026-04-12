# 2026-04-08｜给 day1｜NPC 原生 resident 接管与持久态协作回执 18

## 1. 当前主线

这轮我只做 `native resident` 的 runtime contract：

1. 让 scene-owned resident 被导演接管时，不再乱跑、乱打招呼、乱回旧逻辑。
2. 给 `存档系统` 暴露一份最小但真能用的 resident snapshot surface。

我没有回去补：

1. `runtime spawn / deployment`
2. `Town.unity / Primary.unity`
3. `SpringDay1NpcCrowdDirector` 主消费逻辑

---

## 2. 这轮实际做成了什么

### 2.1 resident 接管 contract 已经落到 `NPCAutoRoamController`

文件：

1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`

新增公开状态面：

1. `IsResidentScriptedControlActive`
2. `ResidentScriptedControlOwnerKey`
3. `ResidentStableKey`
4. `ResumeRoamWhenResidentControlReleases`
5. `IsNativeResidentRuntimeCandidate`

新增公开 contract：

1. `AcquireResidentScriptedControl(string ownerKey, bool resumeRoamWhenReleased = true)`
2. `ReleaseResidentScriptedControl(string ownerKey, bool resumeResidentLogic = true)`
3. `ClearResidentScriptedControl(bool resumeResidentLogic = false)`
4. `CaptureResidentRuntimeSnapshot()`
5. `ApplyResidentRuntimeSnapshot(NpcResidentRuntimeSnapshot snapshot, bool resumeResidentLogic = false)`

实际效果：

1. 只要 resident 进入 scripted control，`Update / FixedUpdate` 会统一冻结 resident runtime。
2. 自动漫游、ambient/selfTalk、旧的移动恢复链不会继续偷偷抢控制权。
3. 释放时会按 `resumeRoamWhenReleased` 决定是否回到合法 resident roam。

### 2.2 NPC own 的 nearby / informal / active session 已经一起让位

文件：

1. `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
2. `Assets/YYY_Scripts/Service/Player/PlayerNpcNearbyFeedbackService.cs`
3. `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`

已经落下去的 gating：

1. resident 被 scripted control 接管时，`NPCInformalChatInteractable` 不再继续开放闲聊入口，也不在收尾时偷着把 roam 恢复回去。
2. `PlayerNpcNearbyFeedbackService` 会跳过 scripted control 中的 resident；如果当前 nearby 气泡已经挂着，也会及时收掉。
3. `PlayerNpcChatSessionService` 如果发现当前正在聊的 NPC 半路被系统接管，会直接按 `SystemTakeover / DialogueTakeover` 收束掉当前闲聊，不让旧会话继续拖住 resident。

### 2.3 day1 现有导演接管入口已经吃通这条 contract

文件：

1. `Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs`

当前 `SpringDay1DirectorNpcTakeover` 已经不是只靠“禁组件”硬压：

1. takeover 时会同步调用 `AcquireResidentScriptedControl("spring-day1-director", ...)`
2. release 时会同步调用 `ReleaseResidentScriptedControl("spring-day1-director", ...)`

我保留了原来的 disable 保险带，没有擅自重写 day1 的主消费逻辑，所以这轮是：

1. `NPC` 提供 contract
2. `day1` 继续沿着自己已有入口消费 contract

### 2.4 resident 最小 snapshot surface 已正式成型

文件：

1. `Assets/YYY_Scripts/Controller/NPC/NpcResidentRuntimeSnapshot.cs`
2. `Assets/YYY_Scripts/Controller/NPC/NpcResidentRuntimeContract.cs`

当前 snapshot 正式字段：

1. `stableKey`
2. `sceneName`
3. `residentGroupName`
4. `residentGroupHierarchyPath`
5. `homeAnchorName`
6. `homeAnchorHierarchyPath`
7. `hasHomeAnchor`
8. `homeAnchorSceneOwned`
9. `residentPosition`
10. `homeAnchorPosition`
11. `scriptedControlActive`
12. `scriptedControlOwnerKey`
13. `resumeRoamWhenReleased`

当前 helper contract：

1. `CaptureSceneSnapshots(Scene scene, bool includeInactive = true)`
2. `TryApplySnapshot(Scene scene, NpcResidentRuntimeSnapshot snapshot, bool resumeResidentLogic = false)`
3. `TryFindResident(Scene scene, string stableKey, out NPCAutoRoamController controller)`
4. `ResolveSceneTransform(...)`
5. `BuildHierarchyPath(Transform target)`

这层现在已经够 `day1 / 存档系统` 直接拿去做：

1. 场景切走前抓 resident 最小 runtime 状态
2. 场景回来后按 stable key 回灌到原生 resident
3. 恢复 HomeAnchor / resident 位置 / scripted control 状态

### 2.5 对应 tests 也补上了

文件：

1. `Assets/YYY_Tests/Editor/NpcResidentDirectorRuntimeContractTests.cs`
2. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`

当前护栏覆盖：

1. `CaptureResidentRuntimeSnapshot_ShouldExposeStableKeyAnchorAndControlState`
2. `ApplySnapshot_ShouldRestoreResidentPositionAnchorAndScriptedControl`
3. `CaptureSceneSnapshots_ShouldOnlyReturnResidentsFromRequestedScene`
4. `NpcTakeover_ShouldDisableRoamAndInteractionsUntilRelease`

---

## 3. 现在还没做成什么

这轮我明确没 claim done 的内容：

1. `Town` resident 的 scene 落位 / deployment
2. `SpringDay1NpcCrowdDirector` 主 runtime 消费链改造
3. formal 是否已消费、relationshipStage 这类长期剧情态的持久化实现
4. typing、气泡文本、nearby 最近触发痕迹、formal 播到半句这类过程态恢复
5. “任意导演中间帧”恢复

另外这轮验证上还有一个诚实限制：

1. `sunset_mcp.py compile` 和 `validate_script` 的 dotnet/codeguard 这轮都卡过超时
2. 但 fresh console 当前是 `errors=0 warnings=0`
3. `validate_script` 返回的是 `assessment=unity_validation_pending`，不是 `own_red`

所以我这轮对外口径是：

1. 代码层没有拿到 own red
2. console 是干净的
3. 但统一编译桥这次没有给我一张漂亮的 compile-pass 小票

---

## 4. 当前阶段

我判断现在已经不是“NPC 还没给 day1 契约面”，而是：

`NPC` 这边的 resident 接管 contract 和最小 snapshot surface 都已经能用了，下一步最值钱的是 `day1` 去消费它，而不是我继续回吞导演主链。

---

## 5. 下一步只做什么

如果下一轮还让我继续压，这边最合理只做两类：

1. 只在 `NPC` own 范围里补 contract 细节或补新的 guard/test
2. 如果 `day1` 在真实消费时撞到 contract 缺口，我再补最小 owner 收口

我不会主动回去吞：

1. resident deployment
2. crowd director 主消费
3. Town scene 写入

---

## 6. 需要主控台现在做什么

`day1` 现在可以直接按下面这条消费：

1. 当导演要完全接管某个原生 resident 时，拿到它的 `NPCAutoRoamController`，调用 `AcquireResidentScriptedControl("day1-owner-key", true/false)`。
2. 当导演释放它时，调用 `ReleaseResidentScriptedControl("day1-owner-key", resumeResidentLogic)`。
3. 如果要跨 scene 或跨阶段保 resident 现场，就抓 `CaptureResidentRuntimeSnapshot()`，或者整场景用 `NpcResidentRuntimeContract.CaptureSceneSnapshots(scene)`。
4. 回灌时按 stable key 用 `TryApplySnapshot(...)`，不要自己再造一套“根据名字硬找 + 直接瞬移 + 手动停 roam”的平行链。

现在最不该做的是：

1. 自己再复制一套“停 roam / 停 nearby / 停 informal”的临时逻辑
2. 把 typing、气泡文本、nearby 痕迹当成 resident 持久态去存

---

## 7. 新增 contract 名称 / 用法 / 限制

### 7.1 给 day1 的接管 contract

名称：

1. `AcquireResidentScriptedControl`
2. `ReleaseResidentScriptedControl`
3. `ClearResidentScriptedControl`

推荐用法：

1. `day1` takeover 开始时调 `AcquireResidentScriptedControl("spring-day1-director", resumeRoamWhenReleased)`
2. `day1` takeover 结束时调 `ReleaseResidentScriptedControl("spring-day1-director", resumeResidentLogic)`
3. 只有在确实要强制清空 owner 栈时才调 `ClearResidentScriptedControl`

限制：

1. 这不是 deployment 系统
2. 这不是 director path 中间帧恢复系统
3. 这也不负责 formal consumed 长期剧情态

### 7.2 给存档系统的 snapshot contract

名称：

1. `NpcResidentRuntimeSnapshot`
2. `NpcResidentRuntimeContract.CaptureSceneSnapshots`
3. `NpcResidentRuntimeContract.TryApplySnapshot`

推荐用法：

1. 存档系统只把这份 snapshot 当“原生 resident 最小运行态”
2. 存长期剧情态时，formal consumed / relationshipStage 仍走它们自己的长期真值来源
3. 读档后只把 resident 拉回合法 scene/group/anchor/runtime 状态，不承诺恢复聊天过程

明确不包含：

1. 当前气泡文本
2. 正在打字到第几个字
3. nearby 最近一次触发
4. informal 冷却 / pending resume
5. formal 对话播到哪一句

---

## 8. 验证

1. `py -3 scripts/sunset_mcp.py errors --count 30 --output-limit 20`
   - `errors=0 warnings=0`
2. `py -3 scripts/sunset_mcp.py validate_script ...`
   - `assessment=unity_validation_pending`
   - `owned_errors=0`
   - `external_errors=0`
   - `codeguard=timeout-downgraded`
3. `git diff --check`
   - 对这轮 own/carried 文件通过

---

## 9. thread-state

1. `Begin-Slice`
   - 已跑
2. `Ready-To-Sync`
   - 未跑
   - 原因：这轮先收在安全点，没有准备立刻 sync
3. `Park-Slice`
   - 已跑
4. 当前 live 状态
   - `PARKED`
5. 当前 first blocker
   - 无
