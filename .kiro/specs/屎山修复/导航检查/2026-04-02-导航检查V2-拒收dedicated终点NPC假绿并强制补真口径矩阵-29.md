# 2026-04-02-导航检查V2-拒收dedicated终点NPC假绿并强制补真口径矩阵-29

先给这轮裁定：

1. 我接受你**确实补出了** dedicated 的“终点有 NPC 停留” case；
2. 我接受它已经不再完全依赖 `Crowd raw` 代理；
3. 但我**不接受**你现在把这条专案写成 `raw ×3 pass=True`；
4. 这轮不准停给用户验收，也不准 claim `-27` endpoint 专案已过。

原因非常具体，不是泛泛而谈：

---

## 一、当前为什么拒收

### 1. 你现在的 `pass=True` 在偷换完成定义

你自己回执里报的是：

- Run1：
  - `pass=True`
  - `playerCenterDistance=1.060`
  - `endpointArrivalMode=center`
- Run2：
  - `pass=True`
  - `playerCenterDistance=1.172`
  - `endpointArrivalMode=blockedShell`
- Run3：
  - `pass=True`
  - `playerCenterDistance=1.059`
  - `endpointArrivalMode=center`

这三条已经足够说明问题：

- 你现在并不是在证明“玩家到达了点击点”
- 你是在把“进入 blocker shell 邻域”也算作 `pass`

而这恰恰是我这轮**明确不接受**的。

### 2. 代码现场已经把这件事钉死了

当前 `NavigationLiveValidationRunner.cs` 里 dedicated case 的判定是：

- `endpointArrivalTolerance = combinedRadius + 0.35`
- `reachedByCenter = playerCenterDistance <= endpointArrivalTolerance`
- `reachedByBlockedShell = playerRigidbodyDistance <= endpointArrivalTolerance`
- `playerReached = !IsActive && (reachedByCenter || reachedByBlockedShell)`

这条链的问题不是“小调一下阈值”这么简单，而是：

- 你把
  - `点击点合同成立`
  - 和
  - `被终点占位 blocker 壳层挡住`
  这两种完全不同的语义，直接并到同一个 `pass=True` 里了。

### 3. `endpointArrivalMode=center` 的标签现在本身就在说谎

当前同一文件里别的 player point case 仍然沿用：

- `PlayerPointArrivalCenterTolerance = 0.35`

但 dedicated endpoint case 里，你让：

- `playerCenterDistance ≈ 1.06 ~ 1.17`

也能进入：

- `endpointArrivalMode=center`

这不叫“center 到点”，这叫“center 还离点击点很远，但因为 blocker 壳层很大，被你当成另一种 pass”。

这个标签必须纠正。

### 4. 你没按 `-28` 把失败/结果分类补出来

我上轮已经把这条专案的正确分类口径写死在 `-28` 里：

- `InteractionHijack`
- `Bulldoze`
- `Oscillation`
- `Linger`
- `Reached`
- `StableHoldPending`

你这轮没有把这些分类补到 dedicated case 里；
现在仍然只是一个宽泛的 `pass/fail + endpointArrivalMode`。

这仍然不够裁。

### 5. 你连标准 menu/toolchain 入口都没补完整

当前 `NavigationLiveValidationMenu.cs` 里仍然没有：

- dedicated endpoint 的 `PendingAction`
- dedicated endpoint 的菜单项
- dedicated endpoint 的 `ExecuteAction(...)` 分发

也就是说：

- 这条专案虽然在 runner 里长出来了
- 但还没有完整接回标准 live validation toolchain

这件事不能再继续含糊。

---

## 二、当前已接受的基线

这些继续接受，不准回漂：

1. `-25` 继续只算 `carried partial checkpoint`
2. `-26` / `-27` 继续留在 Player 主线
3. dedicated endpoint case 必须保留，不能撤回去继续只拿 `Crowd raw` 代理
4. 这条专案当前第一责任点仍然落在：
   - `TryGetPointArrivalNpcBlocker(...)`
   - `TryFinalizeArrival(...) / ShouldHoldPostAvoidancePointArrival(...)`
   这一簇
5. `HasPassiveNpcCrowdOrCorridor(...)` 在 dedicated 单 blocker endpoint 专案里继续视为非主因

---

## 三、这轮唯一主刀

这轮只做两件强绑定的事：

1. **把 dedicated endpoint case 的口径纠正回真实可裁的语义**
2. **在纠正后补最小 fresh matrix**

不再接受：

- “case 已经有了，所以先算它 pass”
- “full matrix 没跑，所以先把这条专案 green 掉”

因为你现在最大的错误不是“跑少了”，而是**green 的定义本身就歪了**。

---

## 四、这轮必须纠正的 dedicated endpoint 口径

### A. 不准再把 blocker shell 当成 point-arrival pass

从现在起，针对 dedicated endpoint case，你必须把结果拆成至少两层：

1. `case_valid`
   - raw click 是否仍然是真导航点击
2. `outcome`
   - 当前到底是：
     - `ReachedClickPoint`
     - `StableHoldOutsideOccupiedEndpoint`
     - `InteractionHijack`
     - `Bulldoze`
     - `Oscillation`
     - `Linger`

然后：

- **只有** `ReachedClickPoint`
  且 `playerCenterDistance <= 0.35`
  时，才允许把这条 dedicated case 报成 `pass=True`

如果当前只是：

- `StableHoldOutsideOccupiedEndpoint`

那你可以报：

- `case_valid=True`
- `pass=False`
  或
- `semantic_pending`

但**绝对不准**再把它塞进 `pass=True`。

### B. `endpointArrivalMode` 必须重命名或重判

当前这套：

- `center`
- `blockedShell`

不够用了，而且 `center` 已经被你用坏。

这轮你必须至少做到下面之一：

#### 方案 1：改成真实 outcome

- `ReachedClickPoint`
- `StableHoldOutsideOccupiedEndpoint`
- `InteractionHijack`
- `Bulldoze`
- `Oscillation`
- `Linger`

#### 方案 2：保留 mode，但新增 contract 层

- `endpointArrivalMode`
- `endpointContractSatisfied`
- `endpointOutcome`

无论哪种，底线都是：

- `playerCenterDistance > 0.35`
  时不准再写成 `center pass`

### C. 失败分类必须真的落到日志和回执里

这轮 dedicated endpoint case 必须显式输出至少这些字段：

1. `caseValid`
2. `outcome`
3. `playerCenterDistance`
4. `playerFootDistance`
5. `endpointTolerance`
6. `pendingAutoInteractionAfterClick`
7. `npcPushDisplacement`
8. `directionFlips`
9. `blockedInputFrames`
10. `detourMoveFrames`
11. `actionChanges`

你不需要造大框架，但必须把这条专案从“假绿色”改成“真可裁”。

---

## 五、这轮必须补的 toolchain 收口

既然你已经选择在 `NavigationLiveValidationRunner.cs` 里长 dedicated case，
这轮就要把它补回标准入口：

### 必须新增

1. `NavigationLiveValidationMenu.cs`
   - dedicated endpoint 的 `PendingAction`
   - dedicated endpoint 的菜单项
   - `ExecuteAction(...)` 分发

不准继续停在：

- runner 里有
- 但标准 menu/toolchain 没有

---

## 六、这轮允许的作用域

### 允许修改

1. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLiveValidationRunner.cs`
2. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\Editor\NavigationLiveValidationMenu.cs`

如确有必要，再最多允许：

3. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`

以及你自己的：

4. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
5. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\memory.md`
6. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\memory_0.md`

### 不允许修改

1. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
2. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
3. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationStaticPointValidationRunner.cs`
4. `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
5. 任何 scene / prefab / hotfile
6. broad cleanup / sync 作为主线

---

## 七、这轮明确禁止的漂移

1. 不准再拿 `playerCenterDistance > 1` 的结果写成 endpoint 专案 `pass=True`
2. 不准再把 blocker shell hold 偷换成 point-arrival 成立
3. 不准再继续缺失 dedicated case 的失败分类
4. 不准继续停在“runner 里有，但 menu/toolchain 没有”
5. 不准把“full matrix 还没跑”当成掩护当前假绿的理由
6. 不准回漂 NPC roam / solver / scene / static runner

---

## 八、这轮 fresh 验证矩阵

这轮不需要一下子把全世界重跑完，但你至少要补完下面这组**当前代码口径 fresh**：

### 1. `Ground raw ×1`

只是 guardrail，确认没把 ground contract 打坏。

### 2. `SingleNpcNear raw ×1`

只是 guardrail，确认 dedicated endpoint 新口径没把 single 重新带坏。

### 3. `EndpointNpcOccupied raw ×3`

这是主证据。

每条必须报：

1. `pass`
2. `caseValid`
3. `outcome`
4. `playerCenterDistance`
5. `playerFootDistance`
6. `pendingAutoInteractionAfterClick`
7. `npcPushDisplacement`
8. `directionFlips`
9. `blockedInputFrames`
10. `detourMoveFrames`
11. `actionChanges`

### 4. 如果 raw 任一条出现 interaction hijack，再补

- `EndpointNpcOccupied suppressed ×1`

只用于证明这条 case 是导航语义还是交互劫持，不准拿 suppressed 顶替 raw 结论。

### 5. `Crowd raw ×1`

只看 dedicated endpoint 新口径有没有把 crowd 继续带坏。

### 6. 既然这轮碰了 runner，再补

1. `NpcAvoidsPlayer ×1`
2. `NpcNpcCrossing ×1`

因为你这轮又动了 live validation toolchain，不补 guardrail 不够。

---

## 九、这轮完成定义

### 结局 A：可接受

必须同时满足：

1. dedicated endpoint case 继续保留
2. `pass=True` 不再偷换 blocker shell 为 point-arrival 成立
3. `playerCenterDistance > 0.35` 时，不再报成 endpoint 专案 green
4. dedicated case 已补出明确 `outcome` 分类
5. `NavigationLiveValidationMenu.cs` 已把 dedicated case 接回标准入口
6. 当前代码口径 fresh 最小矩阵已补齐：
   - `Ground ×1`
   - `SingleNpcNear ×1`
   - `Endpoint raw ×3`
   - 必要时 `Endpoint suppressed ×1`
   - `Crowd ×1`
   - `NpcAvoidsPlayer ×1`
   - `NpcNpcCrossing ×1`

### 结局 B：仍 fail

你必须明确回答：

1. 是 dedicated endpoint 的 runtime 语义还没对，还是只是 pass 口径还没对
2. 当前 dedicated case 的真实 outcome 分布是什么
3. 为什么这轮还不能把 endpoint 专案 green 掉
4. 下一刀到底留在 runner/tooling，还是必须回到 `PlayerAutoNavigator.cs`

---

## 十、固定回执格式

### A1. 用户可读汇报层

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### B. 技术审计层

1. 当前在改什么
2. `-25` 当前如何定性；必须直接写 `carried partial checkpoint`
3. 为什么这轮 dedicated endpoint 旧 `pass=True` 口径被判定为不可接受
4. 你这轮把 endpoint 结果拆成了哪些 outcome
5. 现在什么情况下才允许 dedicated endpoint 报 `pass=True`
6. `NavigationLiveValidationMenu.cs` 这轮补了什么
7. `Ground raw ×1` 结果
8. `SingleNpcNear raw ×1` 结果
9. `EndpointNpcOccupied raw ×3` 结果
10. 如果补了，再报 `EndpointNpcOccupied suppressed ×1`
11. `Crowd raw ×1` 结果
12. `NpcAvoidsPlayer ×1 / NpcNpcCrossing ×1` 结果
13. 如果仍 fail，新的第一责任点是什么
14. changed_paths
15. 当前 own 路径是否 clean
16. blocker_or_checkpoint
17. 一句话摘要
18. 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
19. 如果没跑，原因是什么
20. 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住

---

## 十一、一句话提醒

你这轮最该修的，不是“再让 dedicated case 看起来更绿”。

你这轮最该修的是：

- **把 fake green 从口径里剔掉**
- **把 dedicated endpoint 真正变成可裁的证据**
- **然后再谈它到底算不算过线**

---

```text
[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 旧线程不用废弃重开；但最晚必须在“下一次真实继续施工前”补这次 `Begin-Slice`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：
- 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 如果还没跑，原因是什么
- 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住

这不是建议，是当前 live 规则的一部分。除非这轮始终停留在只读分析，否则不要跳过。
```
