# 2026-04-15｜给 spring-day1｜Day1 语义解耦与 CrowdDirector 退权唯一主刀 prompt v5

## 当前唯一主刀

只收这一刀：

`把 Day1 自己的剧情语义、staged contract、release contract、以及“何时开始/结束 resident 自由态”的 owner 边界重新钉死；同时把 SpringDay1Director 和 SpringDay1NpcCrowdDirector 的职责彻底切开，不再让 Day1 越权代管剧情后的 resident 下半生。`

## 你必须接受并以此为唯一新基线

### 1. opening 的 staged contract

1. `001/002` 在 opening 里仍是必要剧情 actor。
2. 但 opening 的 staged movement contract 不是只给 `001/002`，而是给所有参与 opening 的 Town NPC，包括 `003`、`101~203`。
3. 正确顺序固定为：
   - 进入 `Town`
   - 所有参与 opening 的 NPC 先传送到 scene authored `起点`
   - 再从 `起点` 走到 scene authored `终点`
   - 最多等待 `5` 秒
   - `5` 秒后未到位者直接传送到 `终点`
   - 只有全部归位后，才允许剧情对话框弹出
   - 任意对白进行中不再发生任何走位
   - 对话结束后才允许放人
4. 绝对不允许再出现：
   - `对白开始时人没站对，等对白结束后反而才跑到位`

### 2. opening 结束后的语义

1. `001/002` 继续吃“带玩家进 Primary / house lead”这条导演语义，这条链不要碰。
2. `003` 不再特殊化。opening 结束后，`003` 应和 `101~203` 进入同一条 normal resident contract。
3. 所有已 release 的 Town resident，正确语义是：
   - 先判断自己是否远离 anchor
   - 若远离，则使用导航回 anchor
   - 回到 anchor 后，再进入自由 roam
4. Day1 只允许定义：
   - 需要回哪个 anchor
   - 何时 release
   - 何时 snap
5. Day1 不允许继续手搓：
   - return-home 的 transform fallback
   - `StopRoam / RestartRoam / DebugMoveTo` 组合拳

### 3. Healing / Workbench / Farming 阶段

1. `Primary` 已稳定，不允许改。
2. `001/002` 在该阶段只能出现在 `Primary`。
3. 玩家在此阶段返回 `Town` 时，Town resident 不应因为 Day1 为规避 bug 而被冻结或隐藏。
4. 新基线固定为：
   - 玩家离开 `Town` 时 resident 在哪
   - 玩家回到 `Town` 时 resident 就应仍在哪并保持自由活动
5. 不准再用：
   - `suppress`
   - `hidden`
   - 不让玩家看到 Town resident
   去掩盖 contract 缺陷。

### 4. dinner 的 staged contract

1. dinner 不再把 `001/002` 视为特殊 staged actor。
2. dinner 的 staged movement contract 与 opening 同步：
   - 所有参与 dinner 的 NPC 先到起点
   - 再走终点
   - 最多等 `5` 秒
   - 超时全部 snap 到终点
   - 朝向正确后才开对白
   - 对白进行中不再走位
   - 对话结束后统一 release
3. 不允许 dinner 再 mirror / fallback 到 opening 的 crowd cue 语义。

### 5. 时间语义

1. `18:00 = dinner 开始`
2. `19:30 = dinner 相关剧情收完，玩家恢复自由活动`
3. `20:00 = resident 自主回家`
4. `21:00 = 未到位者才 snap`
5. `26:00 = forced sleep`
6. 但 `20:00~26:00` 不应是 Day1 私房逻辑。Day1 只负责把 Day1 的剧情接到这套“全日通用夜间合同”上，不要再写成 Day1 专属特例。

## 你现在必须重新承认的职责切分

### SpringDay1Director 应该只 own

1. opening / dinner 的剧情语义与 staged contract
2. `001/002` 的 opening 后 `house lead`
3. scripted 对话开始前的就位判定、5 秒等待、超时 snap、对白期间冻结
4. 什么时候 acquire story control
5. 什么时候 release 到 resident contract
6. Day1 首夜如何接入共享夜间合同

### SpringDay1Director 不该再 own

1. resident 在剧情结束后的持续 runtime 持有
2. resident return-home 的执行细节
3. resident 回 anchor 后的 restart-roam 细节
4. 通过 `ShouldReleaseEnterVillageCrowd / ShouldLatchEnterVillageCrowdRelease / ShouldHoldEnterVillageCrowdCue` 这类条件继续偷偷抓 resident

### SpringDay1NpcCrowdDirector 应该被削薄成

1. crowd roster / manifest / binding
2. 只在显式 crowd beat 存在时，为 staged crowd 提供对位协助
3. debug summary / runtime 可视化辅助

### SpringDay1NpcCrowdDirector 不该再继续 own

1. 剧情后的 resident lifecycle
2. resident return-to-baseline / return-home tick
3. `FinishResidentReturnHome`
4. `ForceRestartResidentRoam`
5. Day1 私房的 resident 夜间 schedule owner

## 这轮施工前的硬规矩

在你做任何真实改动前，必须先做一轮只读冲突盘点，至少回答：

1. 当前代码里哪些地方在重复实现同一段 resident release 语义。
2. 哪些地方在实现相反语义：
   - 一边 release
   - 一边下一帧又 reacquire
3. 哪些地方还残留：
   - `DebugMoveTo`
   - `StopRoam`
   - `RestartRoamFromCurrentContext`
   - transform 手搓 fallback
4. 哪些地方在继续把 `003` 差异化。
5. 哪些地方还把 `20:00~26:00` 写成 Day1 私房逻辑。

如果这 5 件事没盘清，不允许直接开改。

## 这轮只允许落下去的唯一窄刀口

只允许先收：

`SpringDay1NpcCrowdDirector 从“剧情后 resident 生命周期 owner”退权，改成 Day1 只在显式 crowd beat 中持有；一旦对白结束并 release，resident 的后续状态切换交还 NPC + 导航合同。`

更具体地说：

1. opening 结束后，`003` 与 `101~203` 必须吃同一条 release contract。
2. daytime 无显式 crowd beat 时，`CrowdDirector` 不得继续 `SyncCrowd()` 抓 resident。
3. `SpringDay1Director` 只负责发 release intent，不得继续深碰 resident runtime。
4. 这轮不要扩到 dinner。
5. 这轮不要扩到 `20:00~26:00` 的全日共享夜间合同实现。
6. 这轮不要扩到导航 core。

## 绝对红线

1. 不准再手搓 return-home 的 transform fallback。
2. 不准再用 `StopRoam / RestartRoam / DebugMoveTo` 组合拳去代替导航。
3. 不准再碰 UI 壳体。
4. 不准再碰 `Primary` 稳定链。
5. 不准再把 `003` 差异化成半剧情半 resident。
6. 不准再用隐藏 Town resident 来规避 bug。
7. 不准再把结构 checkpoint 说成体验已过线。
8. 不准再把 `SpringDay1NpcCrowdDirector` 做成 resident 剧情后的第二个导演。

## 这轮回执必须给我

### A1 保底六点卡

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### A2 用户补充层

必须额外回答：

1. `SpringDay1Director` 和 `SpringDay1NpcCrowdDirector` 现在各自还剩哪些越界点。
2. 你这轮清掉了哪些重复实现 / 冲突实现。
3. `003` 是否已经彻底并回 `101~203` 合同；如果还没有，卡在哪。
4. 你这轮最核心的判断。
5. 你为什么认为这个判断成立。
6. 你这轮最薄弱、最可能看错的点。
7. 自评。

### B 技术审计层

至少包含：

- 当前在改什么
- changed_paths
- exact duplicate/conflict inventory
- `SpringDay1Director` 触碰点
- `SpringDay1NpcCrowdDirector` 触碰点
- code_self_check
- pre_sync_validation
- 当前是否可以直接提交到 `main`
- blocker_or_checkpoint
- 当前 own path 是否 clean
- 一句话摘要

[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：

- 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 如果还没跑，原因是什么
- 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住
