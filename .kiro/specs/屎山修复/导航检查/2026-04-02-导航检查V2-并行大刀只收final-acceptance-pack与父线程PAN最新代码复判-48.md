# 2026-04-02-导航检查V2-并行大刀只收final-acceptance-pack与父线程PAN最新代码复判-48

先给这轮裁定：

1. 这轮已经不是“你继续主刀 PAN runtime”，而是**父线程与 `导航检查V2` 的并行大刀**。
2. 父线程这边已经明确接管：
   - [PlayerAutoNavigator.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs)
   - 也就是说，**PAN crowd / wall runtime 这一刀现在由父线程自己砍**。
3. 你这轮的唯一职责改判为：
   - **只收 runner/menu + final acceptance pack + live 复判**
   - 在父线程当前最新 `PAN` 工作树上，把最终导航验收矩阵真正跑起来并报实
4. 所以从现在开始，你**不准再碰 `PlayerAutoNavigator.cs`**；你只负责把“正确题面 + 正确入口 + 正确 fresh 结果”落稳。

---

## 一、当前已接受的基线

这些已经接受，不准回漂：

1. 旧 `Crowd raw` 现在只算：
   - `legacy blocked-wall stress`
2. 新 crowd 主语义已经定死为：
   - `PassableCorridor`
   - `StaticNpcWall`
3. 当前仍不能写成已关闭的点：
   - `右键停位偏上 / 玩家可视停位偏上`
4. 这轮并行分工已经定死：
   - 父线程主刀：`PlayerAutoNavigator.cs`
   - 你主刀：`NavigationLiveValidationRunner.cs` + `NavigationLiveValidationMenu.cs` + fresh live 复判

---

## 二、你这轮唯一主刀

你这轮只做 2 件事，而且两件都只围绕 runner/menu 与 live 复判：

1. 把最终导航验收入口做成固定 `final acceptance pack`
2. 用**父线程当前最新 PAN 代码** fresh 重跑这套 pack，并给出当前最终导航到底还剩几条红

更直白地说：

- 父线程现在负责“把导航修好”
- 你负责“把最终验收矩阵跑真、跑满、跑清楚”

---

## 三、允许的 scope

这轮允许动：

1. [NavigationLiveValidationRunner.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs)
2. [NavigationLiveValidationMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/Editor/NavigationLiveValidationMenu.cs)
3. 记忆 / 开发日志

这轮禁止动：

1. [PlayerAutoNavigator.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs)
2. NPC roam
3. solver
4. scene / prefab
5. 旧 `legacy crowd` 主 acceptance 口径

如果你发现当前工作树里的 `PlayerAutoNavigator.cs` 已经和你上轮记忆不同，不准回退，也不准自己重写；直接把它当父线程最新工作树基线来复判。

---

## 四、这轮必须补出来的 final acceptance pack

你这轮必须在 runner/menu 里补一个清楚可见的最终导航验收入口。

名字可不同，但语义必须是：

- `Final Player Navigation Acceptance Pack`

它的固定顺序必须是：

1. `PassableCorridor ×3`
2. `StaticNpcWall ×3`
3. `EndpointNpcOccupied ×1`
4. `Ground raw matrix ×1`
5. `SingleNpcNear ×1`
6. `MovingNpc ×1`
7. `NpcAvoidsPlayer ×1`
8. `NpcNpcCrossing ×1`

要求：

1. 旧 `legacy crowd` 不准混进 pack
2. 日志里必须清楚区分：
   - case 名
   - 次数
   - 每条结果
   - 最终总结果
3. 如果现有 runner 已经能顺序跑其中一部分，也要补一层更清楚的 final pack 名义入口，不要让最终验收还靠人手一条条点

---

## 五、这轮 live 执行顺序

顺序固定为：

1. 先确认当前 Unity / compile gate 是 clean，可合法进 Play
2. 如果 final pack 菜单还没补，就先补
3. fresh compile
4. fresh 跑 final acceptance pack
5. 若 pack 里只剩 `PassableCorridor / StaticNpcWall` 红，继续把其他绿面报实，不要再被“好像很多都没做完”带偏

---

## 六、这轮 completion 定义

### A. 这轮可以被判成“并行验收侧完成”，只有同时满足：

1. 你没有碰 `PlayerAutoNavigator.cs`
2. final acceptance pack 已落在 runner/menu
3. current workspace 上 fresh compile clean
4. 你已经用父线程当前最新 PAN 代码跑完：
   - `PassableCorridor ×3`
   - `StaticNpcWall ×3`
   - `EndpointNpcOccupied ×1`
   - `Ground raw matrix ×1`
   - `SingleNpcNear ×1`
   - `MovingNpc ×1`
   - `NpcAvoidsPlayer ×1`
   - `NpcNpcCrossing ×1`
5. 你把 pack 的总结果压成用户能直接看的剩余图：
   - 当前还剩哪些红
   - 哪些已经绿
   - 现在能不能把“导航基本可用”说出口

### B. 这轮明确不允许宣称的东西

1. 不准写：
   - `我顺手又补了 PAN runtime`
2. 不准写：
   - `legacy crowd 也一起算进主 acceptance`
3. 不准写：
   - `Ground raw matrix pass=True，所以右键停位偏上已关闭`

---

## 七、固定回执格式

### A1. 用户可读汇报层

固定 6 项，顺序不得改：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

并且这轮必须额外显式回答 6 句：

1. 父线程这轮主刀的是哪个文件，你这轮主刀的是哪两个文件
2. 你这轮有没有碰 `PlayerAutoNavigator.cs`
3. final acceptance pack 是否已经补出来，入口叫什么
4. final pack 跑完后，当前红面还剩哪几条
5. final pack 跑完后，当前绿面已经有哪些
6. 你现在还能不能把“右键停位偏上”写成已关闭

### B. 技术审计层

至少逐项回答：

1. 当前在改什么
2. 这轮是否新增 final acceptance pack；名称是什么
3. 这轮是否碰了 `PlayerAutoNavigator.cs`；直接写 `是/否`
4. `PassableCorridor ×3` 结果
5. `StaticNpcWall ×3` 结果
6. `EndpointNpcOccupied ×1` 结果
7. `Ground raw matrix ×1` 结果
8. `SingleNpcNear ×1` 结果
9. `MovingNpc ×1` 结果
10. `NpcAvoidsPlayer ×1` 结果
11. `NpcNpcCrossing ×1` 结果
12. 当前是否还能把“右键停位偏上”写成已关闭；如果不能，直接写 `不能`
13. changed_paths
14. 当前 own 路径是否 clean
15. blocker_or_checkpoint
16. 一句话摘要
17. 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
18. 如果没跑，原因是什么
19. 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住

---

## 八、这轮停发边界

这轮只有三种合法收口：

### 1. final acceptance pack 已补出，并且当前最新 PAN 工作树的 fresh pack 结果已完整报实

或

### 2. final pack 已补出，但 fresh live 被一个**晚于当前所有证据**的 external compile/live gate 截停，并给出 exact lines

或

### 3. 你证实 final pack 不需要新增代码，只是在当前 runner/menu 下已经能合法完成同顺序全矩阵，并且结果已报实

除此之外，不准再给“我先分析一下父线程这刀大概会不会好”的空回执。

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
