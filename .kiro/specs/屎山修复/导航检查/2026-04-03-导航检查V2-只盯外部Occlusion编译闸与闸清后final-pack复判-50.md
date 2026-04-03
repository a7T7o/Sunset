# 2026-04-03-导航检查V2-只盯外部Occlusion编译闸与闸清后final-pack复判-50

先给这轮裁定：

1. 父线程还在主刀 [PlayerAutoNavigator.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs)，而且刚刚又补了两刀：
   - `静止 NPC 墙阵识别` 不再跟着玩家当前绕行方向摆头，而是按 NPC 集群本身判定
   - `PassableCorridor` 的 detour keep 口径改成更宽 lateral，不再一上绕就误判“已脱离 crowd”
2. 但这两刀**还没 fresh live 复判**，不是因为父线程停了，而是因为当前 shared root 又冒出一个新的 external compile gate：
   - [OcclusionManager.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs)
   - [OcclusionSystemTests.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/OcclusionSystemTests.cs)
3. 所以你这轮的唯一职责收窄为：
   - **只塌缩这条 external gate 到底是不是当前活 blocker**
   - **如果 gate 清了，就立刻在父线程当前最新 PAN 工作树上重跑 final acceptance pack**
4. 你这轮**不准碰 `PlayerAutoNavigator.cs`**
5. 你这轮**不准碰 `OcclusionManager.cs / OcclusionSystemTests.cs`**
   - 这条 gate 目前先按 external blocker 看待
   - 你只负责 fresh 判定它是 `active / stale / cleared`

---

## 一、当前父线程最新事实

这些已接受，不准回漂：

1. 旧 `Crowd raw` 现在仍只算：
   - `legacy blocked-wall stress`
2. crowd 主 acceptance 仍只认：
   - `PassableCorridor`
   - `StaticNpcWall`
3. 仍然**不能**写成已关闭：
   - `右键停位偏上`
4. 父线程当前最新 `PAN` 现实：
   - `PassableCorridor` latest fresh 仍红：`outcome=Oscillation`
   - `StaticNpcWall` latest fresh 仍红：`outcome=ReachedBlockedTarget`
   - 之后父线程又补了 wall 识别与 detour keep 两刀，但**尚未拿到 fresh live**
5. 父线程脚本级代码闸门已过：
   - `CodexCodeGuard(PlayerAutoNavigator.cs) = CanContinue=true`
   - 所以当前不是父线程把 `PAN` 自己写红

---

## 二、你这轮唯一主刀

你这轮只做这 2 件事：

1. fresh 塌缩 `Occlusion` 这条 external compile gate：
   - 它现在到底是 `active / stale / cleared`
2. 只有在它被 fresh 证实为 `cleared` 之后，才继续：
   - 跑 `Final Player Navigation Acceptance Pack`

如果 gate 仍 `active`，这轮就**停在 blocker receipt**，不准假装跑了 final pack。

---

## 三、允许与禁止 scope

### 允许

1. [NavigationLiveValidationRunner.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs)
2. [NavigationLiveValidationMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/Editor/NavigationLiveValidationMenu.cs)
3. `Editor.log`
4. `CodexEditorCommands status/archive`
5. 记忆 / 开发日志

### 禁止

1. [PlayerAutoNavigator.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs)
2. [OcclusionManager.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs)
3. [OcclusionSystemTests.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/OcclusionSystemTests.cs)
4. NPC roam
5. solver
6. scene / prefab

---

## 四、这轮执行顺序

顺序固定：

1. fresh 读当前磁盘：
   - `OcclusionManager.cs`
   - `OcclusionSystemTests.cs`
2. fresh 执行：
   - `Assets/Refresh`
3. 读最新：
   - `Editor.log`
   - `Library/CodexEditorCommands/status.json`
   - `archive`
4. 只判断 1 件事：
   - `Occlusion` 这条 gate 现在到底是不是当前活 blocker
5. 如果它已 `cleared`：
   - 立刻按你现有 final acceptance pack 入口重跑：
     - `PassableCorridor ×3`
     - `StaticNpcWall ×3`
     - `EndpointNpcOccupied ×1`
     - `Ground raw matrix ×1`
     - `SingleNpcNear ×1`
     - `MovingNpc ×1`
     - `NpcAvoidsPlayer ×1`
     - `NpcNpcCrossing ×1`
6. 如果它仍 `active`：
   - 本轮直接收 blocker
   - 不准把 pack 写成“已开始但没结果”

---

## 五、completion 定义

### A. 这轮可以判成完成，只有两种合法形态

#### 形态 1：blocker receipt

同时满足：

1. 你没有碰 `PlayerAutoNavigator.cs`
2. 你没有碰 `OcclusionManager.cs / OcclusionSystemTests.cs`
3. 你已 fresh 取证 `Occlusion` gate 仍是 `active`
4. 你报出 latest exact compile lines
5. 你明确写：
   - `final pack 本轮未起跑`
   - 原因就是这条 gate

#### 形态 2：final pack rerun

同时满足：

1. 你没有碰 `PlayerAutoNavigator.cs`
2. 你已 fresh 证实 `Occlusion` gate `cleared`
3. 你已在父线程当前最新 PAN 工作树上跑完 final pack
4. 你把红绿面按 case 报实
5. 你仍然没有把：
   - `Ground raw matrix pass=True`
   偷换成：
   - `右键停位偏上已关闭`

### B. 这轮明确不允许宣称

1. 不准写：
   - `我顺手修了 Occlusion gate`
2. 不准写：
   - `父线程 PAN 这两刀我帮它接着补了`
3. 不准写：
   - `右键停位偏上已关闭`

---

## 六、固定回执格式

### A1. 用户可读汇报层

固定 6 项：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

并额外显式回答：

1. 这轮你有没有碰 `PlayerAutoNavigator.cs`
2. 这轮你有没有碰 `OcclusionManager.cs / OcclusionSystemTests.cs`
3. `Occlusion` gate 现在到底是 `active / stale / cleared`
4. 如果 gate 仍 active，latest exact compile lines 是什么
5. 如果 gate 已 cleared，final pack 这轮有没有完整重跑
6. 你现在还能不能把“右键停位偏上”写成已关闭

### B. 技术审计层

至少逐项回答：

1. 当前在改什么
2. 这轮是否碰了 `PlayerAutoNavigator.cs`；直接写 `是/否`
3. 这轮是否碰了 `OcclusionManager.cs / OcclusionSystemTests.cs`；直接写 `是/否`
4. `Occlusion` gate 当前到底是 `active / stale / cleared`
5. 如果 `active`，latest exact compile lines
6. 如果 `cleared`，`Final Player Navigation Acceptance Pack` 结果
7. `PassableCorridor ×3` 结果
8. `StaticNpcWall ×3` 结果
9. `EndpointNpcOccupied ×1` 结果
10. `Ground raw matrix ×1` 结果
11. `SingleNpcNear ×1` 结果
12. `MovingNpc ×1` 结果
13. `NpcAvoidsPlayer ×1` 结果
14. `NpcNpcCrossing ×1` 结果
15. 当前是否还能把“右键停位偏上”写成已关闭；直接写 `不能/能`
16. changed_paths
17. 当前 own 路径是否 clean
18. blocker_or_checkpoint
19. 一句话摘要
20. 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
21. 如果没跑，原因是什么
22. 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住

---

## 七、thread-state

- 如果你这轮继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执额外补：

1. 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
2. 如果没跑，原因是什么
3. 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住
