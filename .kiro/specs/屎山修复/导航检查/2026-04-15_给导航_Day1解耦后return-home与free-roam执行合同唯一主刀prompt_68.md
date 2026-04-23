# 2026-04-15｜给导航｜Day1 解耦后 return-home 与 free-roam 执行合同唯一主刀 prompt - 68

## 当前唯一主刀

只收这一刀：

`当 Day1 已经 release 之后，NPC 如何真正像正常人一样回 anchor/home、恢复 free-roam、并在静态世界里稳定行走。`

你不是来替 Day1 擦屁股的，也不是来替 Day1 定义剧情 phase 的。

## 你必须接受的当前新边界

### Day1 own

1. 哪些 NPC 当前在剧情态。
2. 何时 acquire / release。
3. opening / dinner 的 staged contract：
   - 起点
   - 终点
   - 5 秒等待
   - 超时 snap
   - 对白期间冻结
4. `001/002` 的 opening 后 `house lead`。
5. `18:00 / 19:30 / 20:00 / 21:00 / 26:00` 的剧情接线语义。

### 导航 own

1. 一旦 NPC 已 release，如何从当前位置走到目标点。
2. 如何回 anchor/home。
3. 如何避静态障碍。
4. 如何局部避让。
5. 如何重规划。
6. 如何在回到 anchor 后稳定接回 free-roam。
7. 如何避免：
   - 高频小幅原地抽搐
   - 假 roam
   - 旧短停半态
   - 刚走完立刻又抽下一条病态路线

## 当前用户最新裁定

1. opening 和 dinner 里的 staged movement contract 由 Day1 决定；导航不要去重写其剧情语义。
2. opening / dinner 结束后，resident 回 anchor/home 必须使用正式导航回去。
3. 如果走不回去，是导航问题，不应继续由 Day1 手搓 fallback 位移。
4. `003` 在 opening 后不应再特殊化，应和 `101~203` 进入同一条 resident movement contract。
5. Healing / Workbench / Farming 期间，Town resident 仍应保持自由活动；不要再用“冻结 Town resident”当 workaround。

## 当前已经被 live 反证钉死的事实

1. 用户把 `SpringDay1NpcCrowdDirector` 关掉后，Town resident 会恢复正常 roam。
2. 剩下的问题主要是“剧情后不会自动回家”。
3. 这说明：
   - 当前第一主锅是 Day1 越权抓 resident
   - 不是“导航本身连 roam 都不会”
4. 但也同时说明：
   - 一旦 Day1 放手
   - 你这边必须把回 anchor/home 与 free-roam 执行合同真正收平

## 这轮你只聚焦 4 件事

1. 为什么 opening 结束后 `101~203` 还会高频小幅原地抽搐。
2. 为什么同一时刻 `003` 会罚站，而 `001/002` 正常。
3. `回 anchor/home` 这段如何走正式导航，而不是 debug / scripted / fallback 半混合合同。
4. `回到 anchor 后继续 roam` 这条如何稳定接回统一 free-roam contract。

## 你必须先承认的当前交叉污染点

1. Day1 之前确实越权深碰过：
   - `SetHomeAnchor`
   - `ApplyProfile`
   - `RefreshRoamCenterFromCurrentContext`
   - `StopRoam`
   - `RestartRoamFromCurrentContext`
   - `DebugMoveTo`
2. 这轮你不应该再反过来替 Day1 猜：
   - 谁该被接管
   - 什么时候该对白
   - 什么时候该放人
3. 你要做的是收“已 release 后的 movement quality”，不是再发明一套 Day1 语义。

## 绝对红线

1. 不替 Day1 定义剧情相位。
2. 不隐藏 Town resident 来掩盖问题。
3. 不接受“只要 20:00 会回家就算好了”这种口径。
4. 不继续容忍“return-home 用 debug/scripted move，free-roam 才用正式导航”的双轨病态。
5. 不把 `001/002` 的 story escort 稳定表现误判成全体 NPC 都已经正常。
6. 不把 `CrowdDirector` 越权问题重新偷换成“导航又要兜底一切”。

## 这轮必须给我的东西

### 1. owner matrix

必须明确列出：

1. `Day1 only`
2. `Navigation only`
3. `Shared contract`
4. `当前交叉污染点`
5. `禁止再碰点`

### 2. 唯一下一刀

你必须只选一刀，不准撒网。

格式固定为：

1. `最该收的唯一下一刀是什么`
2. `为什么它比别的刀更值钱`
3. `它是否会伤到 Day1 既有剧情语义`
4. `如果会，具体风险点在哪`

### 3. 证据口径

你必须明确：

1. 哪些判断是代码事实。
2. 哪些判断来自用户 live 反馈。
3. 哪些仍是推断，还没 fresh live。

## 固定回执格式

### A1 保底六点卡

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### A2 用户补充层

必须额外回答：

1. 你这轮证明了哪些坏相确实发生在“已 release 后”的移动执行层。
2. return-home 与 free-roam 当前到底是不是两套合同。
3. 如果是，第一断点 exact 落在哪。
4. 你这轮最核心的判断。
5. 你为什么认为这个判断成立。
6. 你最薄弱、最可能看错的点。
7. 自评。

### B 技术审计层

至少包含：

- 当前在改什么
- changed_paths
- exact caller chain
- exact state handoff chain
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
