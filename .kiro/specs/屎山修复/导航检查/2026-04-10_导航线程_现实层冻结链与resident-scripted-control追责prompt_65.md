# 2026-04-10｜导航线程｜现实层冻结链与 resident scripted control 追责 prompt - 65

这轮不要继续把 `显示层 / 日志层 / mismatch 观测层` 当成主刀。

用户最新裁定已经很明确：

1. 现实不是显示。
2. `NPCMotionController` 现在更会打印日志，这件事可以认。
3. 但用户真正不接受的现实是：
   - 历史版本里，NPC 至少是“会走、会避让、只是有点卡”。
   - 现在的坏相是“不卡了，但傻了 / 不会走 / 乱摆 / 像被冻结”。
4. 所以这轮不准再把“诊断输出更全了”包装成“已经接近修好”。

---

## 当前已接受的基线

这些可以视为已站住，不用再重复 claim：

1. `NPCMotionController.cs` 已补：
   - `showDebugLog` 下更完整的方向 / 来源输出
   - `[NPCFacingMismatch]` 的 burst 抓取
   - 对瞬时观测噪声更不敏感
2. 这层最多只算：
   - 观测层增强
   - 症状层缓和
3. 这层不等于：
   - 现实层根因已解
   - NPC 已恢复到历史“会走会避让”的状态

---

## 本轮唯一主刀

只钉一件事：

### `NPC` 现实层“不走 / 傻站 / 像冻结”是否来自 `resident scripted control` 的 acquire / halt / release / resume 链失真

你这轮必须回答的不是：

- `observedDir` 和 `appliedDir` 现在打得多不多
- `NPCFacingMismatch` 能不能抓到
- free-roam 朝向是否比上一刀少抖一点

你这轮必须回答的是：

1. 哪个 owner 在拿 `resident scripted control`
2. 拿了之后 NPC 为什么被挂进 freeze / inactive
3. release 是否真的发生
4. release 之后为什么没有恢复 `StartRoam()`
5. 如果不是 release 漏了，那是谁又重新抢回控制权

---

## 允许的 scope

### 允许只读核对

- `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
- `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
- `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
- `Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs`
- `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`

### 允许真实落代码

只允许在下面 1 个 own 文件内做最小补口：

- `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`

### 如果第一真问题落在下面这些 caller

- `SpringDay1Director.cs`
- `SpringDay1DirectorStaging.cs`
- `SpringDay1NpcCrowdDirector.cs`

这轮不要越权代修。

你要做的是：

- 报 exact caller
- 报 exact method
- 报 exact 条件
- 报为什么它会把 NPC 留在冻结态或重新抢走 roam

---

## 明确禁止的漂移

1. 不要再把 `NPCMotionController` 的显示层增强继续当主刀。
2. 不要回去泛修 `NavigationTraversalCore / NavGrid2D / PlayerAutoNavigator`。
3. 不要顺手扩成“历史版本整包回滚方案”。
4. 不要碰 `Primary.unity / Town.unity / UI / 字体 / 气泡样式 / PromptOverlay`。
5. 不要再用“剧情链正常、所以大方向是对的”来回避 free-roam 现实冻结。
6. 不要把“日志更全了”写成“现实更接近修好”。

---

## 完成定义

这轮完成，必须至少满足下面二选一之一：

### A. 第一真问题在 `NPCAutoRoamController.cs`

你要：

1. 查实 freeze / resume 的现实链；
2. 只在 `NPCAutoRoamController.cs` 做最小修复；
3. 跑最小代码层验证；
4. 明确回答为什么这刀是在修现实层，而不是继续修显示层。

### B. 第一真问题不在你 own 文件，而在 Day1 caller

你要：

1. 给出 exact caller chain：
   - 谁 `AcquireResidentScriptedControl`
   - 谁 `HaltResidentScriptedMovement`
   - 谁应该 `ReleaseResidentScriptedControl`
   - 哪一步没有发生，或哪一步被重复抢回
2. 明确回答：
   - 这不是“还没定位”
   - 而是“第一真责任点已经越过导航 own，落到 Day1 caller”
3. 这轮停在 exact blocker，不要跨线代修。

---

## 强制回答格式

你这轮回执不要再泛说“朝向更稳了”。

固定按下面格式回：

### A1 保底六点卡
1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### A2 用户补充层
必须额外明确分开写：

1. `显示层已站住什么`
2. `现实层已站住什么`
3. `现实层还没站住什么`
4. 你这轮为什么判断“问题久修不住”
5. 你这轮最薄弱、最可能看错的点
6. 你的自评

### B 技术审计层
至少包含：

- 当前在改什么
- changed_paths
- 当前复现链 / failing NPC / failing scene
- exact acquire / halt / release / resume 链
- 如果命中 Day1 caller：
  - exact file
  - exact method
  - exact condition
- code_self_check
- pre_sync_validation
- 当前是否可以直接提交到 `main`
- blocker_or_checkpoint
- 当前 `own path` 是否 clean
- 一句话摘要

---

## thread-state

如果你这轮继续真实施工：

1. 先跑 `Begin-Slice`
2. 第一次准备 sync 前跑 `Ready-To-Sync`
3. 如果这轮停下或卡住，跑 `Park-Slice`

回执里补：

- `Begin-Slice / Ready-To-Sync / Park-Slice` 是否已跑
- 如果没跑，原因是什么
- 当前 live 状态是 `ACTIVE / READY / PARKED` 还是被 blocker 卡住

---

## 一句话钉死

你这轮不是来证明“我现在更会打印 NPC 鬼畜日志了”。

你这轮是来钉死：

**为什么 NPC 现实里会进入“不走 / 傻站 / 像冻结”的状态，以及第一真责任点到底还在不在导航 own。**
