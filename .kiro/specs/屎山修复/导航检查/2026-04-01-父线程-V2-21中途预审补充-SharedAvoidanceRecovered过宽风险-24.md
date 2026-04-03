# 2026-04-01-父线程-V2-21中途预审补充-SharedAvoidanceRecovered过宽风险-24

## 一、这份补充解决什么问题

这不是新的 runtime 施工单。

这份文档只解决一个父线程中途新增的验收风险：

- `导航检查V2 -21` 当前方向大体是对的；
- 但从它正在施工的 [NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 现场来看，`SharedAvoidanceRecovered` 这条 interruption reason 目前疑似过宽；
- 如果不提前钉住，后面很容易出现一种“无限鬼畜确实少了，但 NPC 只要正常绕开一次也会被判成异常中断”的假成功。

父线程这份补充的作用，就是把这个风险先显式写死，避免后续收件时只看见“有 interruption 了”，却漏掉“中断条件是不是过宽”。

---

## 二、当前中途现场的新增事实

### 1. `导航检查V2` 当前 thread-state 是合规的

父线程当前只读看到：

- [导航检查V2.json](/D:/Unity/Unity_learning/Sunset/.kiro/state/active-threads/导航检查V2.json)
  - `status=ACTIVE`
  - `current_slice=冻结19并转NPC漫游异常中断-21`
  - `owned_paths=[Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs]`

这说明它这轮至少没有明显偏航回：

- `PlayerAutoNavigator.cs`
- solver
- static runner

### 2. 它当前 dirty 的方向也基本对

父线程当前只读 `git diff -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`，看到的正向信号包括：

1. 确实在 `NPCAutoRoamController.cs` 上补：
   - `RoamMoveInterruptionReason`
   - `RoamMoveInterruptionBlockerKind`
   - `RoamMoveInterruptionSnapshot`
   - `RoamMoveInterrupted`
2. `TryInterruptRoamMove(...)` 仍保留：
   - `if (debugMoveActive || state != RoamState.Moving) return false;`
   - 说明它还在守“只打 roam，不误伤 DebugMoveTo”这条边界。
3. 触发点也确实接在 prompt 指向的热区：
   - `CheckAndHandleStuck(...)`
   - `TryHandleSharedAvoidance(...)`
   - `TryReleaseSharedAvoidanceDetour(...)`

所以父线程当前不否认一件事：

- `V2 -21` 至少在文件选择和热区选择上，是朝对的方向在走。

---

## 三、当前新增的最关键风险：`SharedAvoidanceRecovered` 疑似过宽

### 1. 风险发生在哪

当前 dirty 里最值得警惕的一段是：

- [NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
  - `TryReleaseSharedAvoidanceDetour(...)`

父线程当前只读到的语义是：

1. 先判断：
   - `!avoidance.HasBlockingAgent`
2. 再进入：
   - `TryReleaseSharedAvoidanceDetour(...)`
3. 而在 `TryReleaseSharedAvoidanceDetour(...)` 里，只要：
   - `detour.Cleared || detour.Recovered`
4. 就立刻触发：
   - `TryInterruptRoamMove(RoamMoveInterruptionReason.SharedAvoidanceRecovered, ...)`

### 2. 为什么这看起来不像“明确异常”

从名字和当前上下文看：

- `SharedAvoidanceRepathFailed`
  - 很像明确异常
- `StuckCancel / StuckRecoveryFailed`
  - 也很像明确异常
- 但 `SharedAvoidanceRecovered`
  - 更像“临时 detour 已经清掉 / 已经恢复主路径”
  - 这本身未必是坏事

也就是说，当前这条链存在一种真实风险：

- NPC 只是正常绕开了阻挡者；
- 前方窗口打开后，detour 被正常 clear / recover；
- 结果代码也把它当成 interruption；
- 当前 roam 段被直接切断，NPC 进入 `EnterShortPause(false)`；
- 从玩家眼里看，就会像：
  - NPC 明明绕开成功了，却忽然自己停住；
  - 或者每次稍微避让一次，就重新 pause / 重新想；
  - 不再是“无限鬼畜”，但会变成“异常胆小 / 异常犹豫 / 走走停停”。

### 3. 当前最该警惕的不是 compile，而是“止住大灾难，却引入新僵硬”

这类风险最容易骗过一次粗糙验收：

- 用户原先最痛的是无限互顶、无限改道、无限鬼畜；
- 现在如果这些现象明显减少，看起来像成功；
- 但如果代价是：
  - 正常恢复主路也会被当成异常中断
  - roam move 被过早切短
  - NPC 常态行为变得碎、怂、停顿多

那这仍然不能算真正过线。

---

## 四、这会如何改变父线程下一次收件的审法

`-22` 和 `-23` 里已经有：

- 是否真的冻结 `-19`
- 是否真的切到 `NPCAutoRoamController`
- 是否真的命中真实 roam 互卡现场
- 是否误伤 `DebugMoveTo(...)`

现在父线程要额外新增第 5 个重点追问：

### 新增追问：`SharedAvoidanceRecovered` 到底是“异常恢复失败链”还是“正常恢复主路链”？

`导航检查V2` 下次回执时，必须额外交代清楚：

1. 为什么 `SharedAvoidanceRecovered` 在它这轮语义里算“应中断”的异常，而不是“正常绕开后恢复”；
2. 它有没有额外门槛，把这条 reason 收窄到真正异常现场；
3. 如果没有额外收窄，它凭什么证明普通临时避让不会也被切成 interruption。

如果这些问题答不清，那就算：

- roam 互卡现场被切断了

也仍然只能算：

- “止住一种灾难，但高风险引入另一种常态僵硬”

不能直接放行。

---

## 五、父线程新增要求的最小证据

如果 `V2 -21` 后续想真正站稳，不再只需要“1 条 roam 互卡被打断”的证据。

父线程新增最小要求是：

### 证据 A：真实异常互卡被打断

这条仍然要有：

- 真实 roam 互卡 / 互顶 / detour-recover 循环
- 当前 move 被 interruption
- reason / blocker / requested / current 可读

### 证据 B：正常短暂避让后恢复，不会被同样一刀切断

这条现在也应该补到回执解释里，哪怕是：

- 一条 dedicated live
- 或一条极明确的日志 / 条件解释

至少要能说明：

- 不是所有 `detour clear / recover` 都会触发异常中断；
- 否则父线程会默认把这条风险视为未收口。

注意：

父线程不是要求它这轮把 NPC 漫游“做丝滑”；
父线程只是要求：

- 不准把“正常恢复主路”也误伤成异常 interruption。

---

## 六、另一条只读事实：event hook 仍只是 hook，不是下游完成

当前父线程 `rg` 到的事实仍然是：

- `RoamMoveInterrupted` 目前只在 [NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 内定义和触发
- 暂未看到明确订阅者

所以后续回执如果写成：

- “玩家打断降好感 / NPC 互吵等后续逻辑已具备”

父线程仍然要拒绝这种表述。

这轮最多只能 claim：

- 预留了中断原因口
- 后续 NPC 系统可以接

不能 claim：

- 下游消费逻辑已完成

---

## 七、父线程当前更新后的正式口径

### 当前接受

1. `V2 -21` 当前文件方向是对的；
2. 它确实在 `NPCAutoRoamController.cs` 做 roam interruption；
3. 它也确实还在守 `DebugMoveTo(...)` 边界。

### 当前不接受提前放行

在没有额外解释或证据前，父线程不接受直接把下面这件事放行：

- `SharedAvoidanceRecovered` 直接作为 interruption reason 被广义接入

因为这条链当前高概率还没证明：

- 它只打异常，不打正常恢复。

---

## 八、一句话结论

父线程当前新增的最重要中途判断是：

- `V2 -21` 现在不是明显偏航，而是**已经打到对的文件上了**；
- 但它当前最危险的新副作用风险，也已经很清楚：
  - **`SharedAvoidanceRecovered` 可能过宽，导致正常绕开后的恢复主路也被切成异常中断。**

所以下一次收件时，父线程不会只看“有没有 interruption”，
还会额外看：

- **这个 interruption 到底有没有只打异常，不误伤正常恢复。**
