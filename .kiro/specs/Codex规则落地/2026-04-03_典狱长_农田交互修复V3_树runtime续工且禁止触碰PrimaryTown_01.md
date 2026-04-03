# 2026-04-03_典狱长_农田交互修复V3_树runtime续工且禁止触碰PrimaryTown_01

你这轮不要再申请 `Primary`，也不要再把 `Town`、场景基线、全局持久层和树 runtime 混成一锅。治理位已经把分工重新切开：`Primary` 的真正全局化/持久层抽离，改由 `Codex规则落地` 主线程接手；你这轮唯一主刀，只剩“不依赖 `Primary` 的树 runtime / 树苗放置卡顿链”。

## 当前已接受的基线

- 用户刚刚明确重裁了所有权：
  - `Primary` 不再由用户自己占着不用，但也**不是**这轮发给你
  - `Town` 继续由治理主线程单独接
- 当前治理位对你的裁定已经固定：
  - 你上一轮的只读判断方向基本成立
  - 但当前不放行你碰 `Primary`
- 当前 `Primary` 相关结论已经被治理位接管，不需要你重复证明：
  - 当前 `Primary.unity` 缺 `SeasonManager / TimeManager / TimeManagerDebugger`
  - 这条 manager/debugger 基线后续要有人修，但这轮不是你
- 当前你自己的真实现场不是 clean surgical writer；至少仍挂着这些 runtime dirty：
  - `Assets/YYY_Scripts/Controller/TreeController.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
  - `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
  - `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs`
  - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
- 因此这轮正确目标不是“继续要 `Primary` 写窗”，而是：
  - 把**不依赖 `Primary`** 的树 runtime / 放置卡顿链尽可能收到最窄
  - 并把剩余 blocker 收敛成“只剩 `Primary` manager/debugger 基线”

## 本轮唯一主刀

- 唯一主刀：
  - 树苗放置卡顿、树 runtime 生命周期、树 runtime 注册链里**不依赖 `Primary` 场景基线**的部分
- 本轮目标不是：
  - “把整套树问题全部宣告完成”
  - “顺手把 `Primary` 第 1 步做了”
  - “顺手接 Town”

## 允许的 scope

你这轮只允许围绕下面这条 runtime 线做事，优先先收已有 dirty，再决定是否补最小新文件：

- `Assets/YYY_Scripts/Controller/TreeController.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs`
- `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
- `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`

如果你在真正收口中发现**必须**碰到树 runtime 的注册链，可以额外只碰：

- `Assets/YYY_Scripts/.../ResourceNodeRegistry.cs`

但只有在你能明确说明“为什么这个改动属于树 runtime 注册总口，而不属于全局持久层/场景基线”时才允许。

## 绝对禁止

你这轮绝对禁止以下动作：

1. 触碰任何 scene 文件：
   - `Assets/000_Scenes/Primary.unity`
   - `Assets/000_Scenes/Town.unity`
   - 任何 backup / validation / scratch scene
2. 触碰真正全局化/持久层那条线：
   - `Assets/YYY_Scripts/Service/PersistentManagers.cs`
   - `Assets/YYY_Scripts/Data/Core/PersistentObjectRegistry.cs`
   - `Assets/YYY_Scripts/Service/TimeManager.cs`
   - `Assets/YYY_Scripts/Service/SeasonManager.cs`
   - `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
3. 再次申请 `Primary` 写窗，或把任何未过线问题口头归因为“不给 `Primary` 我没法做”
4. 顺手扩到：
   - `Town`
   - `Home`
   - 字体/TMP
   - `GameInputManager`
   - UI
   - NPC
5. 用“我已经知道第 1 步方向”来冒充“我可以继续写 `Primary`”

## 本轮真实完成定义

你这轮只有在下面条件成立时，才算真正完成：

1. 你把当前 runtime 线里**不依赖 `Primary`** 的部分尽可能收净
2. 你能明确把剩余未过线问题分成两类：
   - `本轮已在 runtime 线修掉的`
   - `唯一还剩且必须等 Primary manager/debugger 基线的`
3. 你不能把“需要 `Primary` 才能继续”的 blocker 扩成一坨宽泛大词；必须给 exact blocker
4. 当前 own 路径 clean，或者你明确报出为什么还不 clean

## 你必须先回答的两个问题

在任何新增施工前，你必须先给出这两个判断：

1. 你当前这批 dirty 中，哪些已经属于“第 2 步 生命周期/注册总口”的前置地基，哪些只是树苗卡顿线的局部减重
2. 如果这轮完全不碰 `Primary`，你还能把哪些 bug 真正收掉，哪些只能收敛成 blocker

不要再用“以后迟早要 `Primary`”来跳过这两个问题。

## 回执必须强制分两层

### A. 用户可读汇报层

必须按顺序逐项写：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### B. 技术审计层

然后再补：

1. 当前保留的 runtime scope
2. 当前禁止触碰的 `Primary/Town/持久层` 清单
3. 这轮新完成的 runtime 点
4. 这轮仍未完成但已被你收敛成 exact blocker 的点
5. `changed_paths`
6. 验证结果
7. 当前 own 路径是否 clean
8. blocker / checkpoint

## 一句话底线

你这轮的目标不是“拿回 `Primary`”，而是把树 runtime 线独立收窄到最小；凡是需要 `Primary` 的地方，只准收敛成 exact blocker，不准偷写 scene。

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
