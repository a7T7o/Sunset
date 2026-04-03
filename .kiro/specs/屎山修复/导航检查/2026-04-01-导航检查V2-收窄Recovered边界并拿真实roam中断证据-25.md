# 2026-04-01-导航检查V2-收窄Recovered边界并拿真实roam中断证据-25

先给这轮裁定：

1. 我接受你把 `-19` 冻结成 carried partial checkpoint；
2. 我接受你已经把 roam interruption contract 真正接进了 `NPCAutoRoamController.cs`；
3. 我接受你这轮没有把 `DebugMoveTo(...)` 两条 NPC guardrail 带坏；
4. 但我**不接受**你把 `-21` 现在这个状态叫 done。

原因只有两个，而且都已经很具体：

1. 你还没有拿到 **1 组真实 roam 互卡 / 异常中断 fresh**；
2. 你这轮没有正面回答父线程 `-24` 的新增风险：
   - 当前 `SharedAvoidanceRecovered` 被广义接成 interruption reason，
   - 它很可能把“正常绕开后恢复主路”也误判成异常中断。

所以这轮不要再做“大面积继续试”。

下一刀只做一件事：

- **把“真正异常 interruption”与“正常 detour clear / recover 后回主路”彻底切开，并拿到对应 fresh 证据。**

---

## 一、当前已接受的基线

这些继续接受，不要回头打散：

1. `-19` 当前只作为 carried partial checkpoint 存在  
   - single 静止 NPC 推挤改善事实保留  
   - 双 NPC / corridor 仍未闭环  
   - static / 点击点偏上这轮仍是 `无`
2. `NPCAutoRoamController.cs` 当前已存在：
   - `RoamMoveInterruptionReason`
   - `RoamMoveInterruptionBlockerKind`
   - `RoamMoveInterruptionSnapshot`
   - `RoamMoveInterrupted`
   - `DebugLastRoamInterruption*`
3. 当前已接上的 fail-fast 入口包括：
   - `StuckCancel`
   - `StuckRecoveryFailed`
   - `SharedAvoidanceRepathFailed`
4. `NpcAvoidsPlayer ×1 / NpcNpcCrossing ×1` 这轮 fresh 仍是绿的

这些都不要回退。

---

## 二、这轮唯一主刀

只收这一件事：

- **`SharedAvoidanceRecovered / SharedAvoidanceClear` 当前到底是不是“异常中断”，还是只是“正常恢复主路”**

父线程当前默认怀疑是后者。

换句话说：

- 不是所有 `detour.Cleared || detour.Recovered` 都应该切成 interruption；
- 如果只是正常让开、窗口打开、回到主路径，那就不该被判成异常 fail-fast；
- 只有当它仍然代表“这次 roam move 已经进入不该继续的坏循环”时，才配叫 interruption。

这轮你要把这条边界收窄清楚。

---

## 三、当前唯一允许的实现方向

### 方向 A：优先方向

把 `TryReleaseSharedAvoidanceDetour(...)` 里对：

- `detour.Cleared`
- `detour.Recovered`

的处理收窄成：

- **默认先视为正常恢复主路，不直接 interruption**

也就是：

1. detour 清掉 / recover 成功后  
2. 先走回主路 / rebuild / 继续当前 roam move  
3. 不要立刻 `TryInterruptRoamMove(SharedAvoidanceRecovered, ...)`

### 方向 B：只有在你拿到硬证据时才允许

如果你能用 fresh 证据证明：

- 某种 `Recovered/Cleared` 在当前 roam 语义里其实稳定等价于“异常恢复链复活前的最后一个节点”

那你可以保留某个 interruption reason。

但这时你必须同时做到：

1. 不再广义接整段 `detour.Cleared || detour.Recovered`；
2. 只对你能证明的那一种异常恢复情形触发；
3. 给出非常具体的 gate 条件和 live 证据。

如果做不到这 3 条，就默认按方向 A 走，不准强撑。

---

## 四、这轮不准再做的事

1. 不准继续拿“自然 roam 短窗再等一会儿”当主策略  
   - `25s + 60s` 这类低信号等窗已经不够了
2. 不准继续把 `SharedAvoidanceRecovered` 广义挂着，同时又说“以后再证明”
3. 不准回头再补 `PlayerAutoNavigator.cs`
4. 不准回头再补 solver
5. 不准再把 static / 点击点偏上带进来
6. 不准只交两条 guardrail 绿，就继续 claim `-21` 快完成

---

## 五、作用域硬限制

### 允许修改

只允许：

1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`

如果你**绝对必须**加一个支撑入口来拿证据，最多再允许：

2. `Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs`

这个支撑入口只允许做一件事：

- 增加 **真实 roam 语义** 的最小证据入口

不准把它做成：

- 新验证框架
- 新大矩阵
- 继续走 `StopRoam + DebugMoveTo(...)` 的伪 roam

### 不允许修改

1. `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
2. `Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs`
3. `Assets/YYY_Scripts/Service/Navigation/Editor/NavigationLiveValidationMenu.cs`
4. `Assets/YYY_Scripts/Service/Navigation/NavigationStaticPointValidationRunner.cs`
5. `Assets/Editor/NavigationStaticPointValidationMenu.cs`
6. `Assets/000_Scenes/Primary.unity`

---

## 六、这轮验证要求

这轮不是只要一条“异常中断 fresh”。

必须最小成对证明两件事：

### 证据 1：真实异常会中断

至少 1 条 fresh，必须满足：

1. 是 **roam 语义**
2. 不是 `DebugMoveTo(...)`
3. 能读到：
   - `roam interrupted =>`
   - `Reason`
   - `Trigger`
   - `BlockerKind`
   - `BlockerId`
   - `Requested / Active / Current`

### 证据 2：正常恢复不会被误伤

至少 1 条 fresh，必须满足：

1. 也是 **roam 语义**
2. NPC 出现过短暂 shared avoidance / detour
3. 后续能恢复主路继续 move
4. **没有**被同样切成 interruption

如果你拿不到第 2 条，那就说明 `-24` 风险还没收干净，不能 claim 这轮完成。

### 护栏

继续补：

1. `NpcAvoidsPlayer ×1`
2. `NpcNpcCrossing ×1`

---

## 七、这轮完成定义

### 结局 A：可接受

必须同时满足：

1. `-19` 继续保持冻结，不再扩
2. `SharedAvoidanceRecovered / Clear` 的边界已经收窄清楚
3. 真实异常 roam 会 interruption
4. 正常短暂避让恢复主路不会被误伤成 interruption
5. `NpcAvoidsPlayer ×1 / NpcNpcCrossing ×1` 不坏
6. static / 点击点偏上这轮继续明确写 `无`

### 结局 B：仍 fail

你必须明确回答：

1. 当前为什么还拿不到真实 roam 互卡 fresh
2. 当前卡在“异常不触发”，还是“正常恢复也会误伤”
3. `SharedAvoidanceRecovered` 这条链最后是保留了、收窄了，还是撤掉了
4. 下一步最窄第一责任点还剩哪里

---

## 八、固定回执格式

### A1. 用户可读汇报层

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### B. 技术审计层

1. 当前在改什么
2. `-19` 当前 carried partial checkpoint 是否有变化；如果没有，直接写没有
3. `SharedAvoidanceRecovered / Clear` 这轮最终是：
   - 保留广义 interruption
   - 收窄后保留
   - 还是已撤回为正常恢复链
4. 你具体改了 `NPCAutoRoamController.cs` 的哪几个分支
5. 这轮有没有新增一条真实 roam 语义的证据入口；如果有，改了哪里
6. 真实异常会中断的 fresh 结果
7. 正常恢复不会误伤的 fresh 结果
8. `NpcAvoidsPlayer ×1 / NpcNpcCrossing ×1` 结果
9. 当前 latest interruption snapshot 的 `Reason / Trigger / BlockerKind / BlockerId / Requested / Active / Current / Blocker`
10. `DebugMoveTo(...)` 是否仍保持旧语义；如果保持，为什么
11. static / 点击点偏上这轮是否处理；如果没有，直接写 `无`
12. changed_paths
13. 当前 own 路径是否 clean
14. blocker_or_checkpoint
15. 一句话摘要
16. 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
17. 如果没跑，原因是什么
18. 当前是 `ACTIVE / READY / PARKED` 还是被 blocker 卡住

---

## 九、一句话提醒

你这轮不是去“再多抓一会儿 roam”。

你这轮真正要收的是：

- **异常 interruption 的边界**
- **正常恢复主路的边界**
- **并用成对 fresh 证据把这两件事分开**

如果这条边界没切开，就算你偶然抓到一条 interruption，也还不能放行。

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
