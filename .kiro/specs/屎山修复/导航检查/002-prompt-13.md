# 002-prompt-13

这轮我先把你的最新回执定性清楚：

1. 我接受这轮。
2. 我接受的不是“又多迁了一点结构”，而是：
   - shared detour clear / recover 开始真正有节制
   - `PlayerAvoidsMovingNpc` 已从单场 `timeout fail` 拉成单场 `fresh live pass`
3. 但我现在只接受到“单场过线”，还不接受“导航整体已收口”。

原因很简单：

- 你这轮 fresh 验掉的只有：
  - `PlayerAvoidsMovingNpc = pass=True / minClearance=0.385 / playerReached=True / npcReached=True / timeout=3.13`
- 你还没有同轮 fresh 证明：
  - `NpcAvoidsPlayer`
  - `NpcNpcCrossing`
  没被这轮 detour clear / recover 节制改动打回归

所以你下一刀不准立刻漂去新责任簇。

---

## 一、当前已接受的基线

当前导航线基线继续固定为：

- `S0`：部分完成
- `S1 / S2 / S4`：部分完成
- `S3 / S5`：未完成
- `S6`：部分完成

当前已接受的真实 checkpoint 有四层语义：

1. `BuildPath / RebuildPath / ActualDestination / 路径后处理`
2. `stuck / repath / 恢复入口`
3. `detour create / clear / recover`
4. `detour clear / recover` 过密震荡已被压住，单场 `PlayerAvoidsMovingNpc` 已过线

所以这轮正确起点不是再讲这一刀为什么有效。

而是：

> 证明这刀不是“只救活一条单场”，  
> 而是至少在当前三条核心 probe 上没有回归。

---

## 二、这轮唯一主刀

### 这轮唯一主刀固定为：

> 把“单场过线”扩成“同轮三场 fresh 无回归”。

也就是在当前这版：

- clear hysteresis
- recovery cooldown
- owner release 条件

不大改方向的前提下，补齐同轮 fresh：

- `NpcAvoidsPlayer`
- `NpcNpcCrossing`

如这两条里出现回归，你允许只在当前 detour clear / recover 节制簇内做最小补口。

---

## 三、这轮允许做什么

### 允许：

1. 继续只围绕这轮新加的：
   - clear hysteresis
   - recovery cooldown
   - owner release 条件
   做最小补口
2. 跑同轮 fresh：
   - `NpcAvoidsPlayer`
   - `NpcNpcCrossing`
3. 如其中一条回归，允许同轮只修：
   - [NavigationPathExecutor2D.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs)
   - [PlayerAutoNavigator.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs)
   - [NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
   中与当前节制直接相关的最小条件
4. 若这轮确实动了节制条件，允许补跑：
   - `PlayerAvoidsMovingNpc`

---

## 四、这轮明确禁止

### 不允许：

1. 漂去：
   - `arrival / cancel / path-end`
   - `NavigationTrafficArbiter`
   - solver 参数泛调
2. 不允许把 unrelated test failure 混进主叙事
3. 不允许重新跑成长窗 full live
4. 不允许顺手开新的责任簇
5. 不允许拿外部 blocker 当停车位

---

## 五、这轮完成定义

只有满足下面任一条，这轮才算完成：

### 结局 A：三场同轮 fresh 全绿

你要明确给出：

- `PlayerAvoidsMovingNpc`
- `NpcAvoidsPlayer`
- `NpcNpcCrossing`

三条同轮 fresh 结果，并明确说明：

- 当前这版 detour clear / recover 节制没有带来回归

### 结局 B：补两条 fresh 后发现回归，但同轮已把回归重新压住

前提是你必须明确证明：

1. 回归确实由这轮新节制引起
2. 你没有漂去别的责任簇
3. 你只在当前 clear / recover 节制簇内做了最小补口
4. 最终三条同轮结果已重新回正，或至少失败形态已继续收窄到新的单一责任点

如果只是“`PlayerAvoidsMovingNpc` 过了，所以另外两条我没跑”，这轮不算完成。

---

## 六、live 纪律继续钉死

1. 只跑当前需要的最小场景
2. 一旦拿到 `scenario_end / all_completed` 或足够证据，立刻 `Stop`
3. 完成后必须确认回到 `Edit Mode`
4. 不允许再把日志刷成洪水

---

## 七、下一次回执固定格式

- 当前在改什么
- 这轮是否仍只锁 detour clear / recover 节制这一个责任簇
- `NpcAvoidsPlayer` 最新 fresh 结果
- `NpcNpcCrossing` 最新 fresh 结果
- 如本轮有重跑，`PlayerAvoidsMovingNpc` 最新 fresh 结果
- 三场是否同轮全绿；如果不是，哪一条回归了
- 如果出现回归，新的第一责任点是否仍然留在当前节制簇内
- changed_paths
- 新增或修改了哪些测试 / 静态验证 / 脚本验证
- live 是否在拿到证据后立刻 `Stop`
- 当前是否已退回 `Edit Mode`
- external_blocker_note
- blocker_or_checkpoint
- 一句话摘要
