# 2026-04-03｜工具-V1 三脚本归仓与 `Ready-To-Sync` 收口 prompt

你这轮不是继续开新功能，也不是继续回 scene / binder / 工具线。

当前已接受的基线先写死：

1. 你的边界重置已经基本成立：
   - 当前 live `thread-state` 已经收窄到 3 个脚本：
     - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
     - `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
   - 当前状态也已经是 `PARKED`
2. 你上一轮回执里关于“只保留脚本契约、不再碰 `Primary/Town` 场景实写、不再碰 binder / 通用工具”的判断，我现在接受。
3. 但这还不是“这条线可以彻底散场”，因为你这 3 个脚本当前还没完成：
   - `Ready-To-Sync`
   - 最终 script-only 归仓
   - own 路径 clean 报实

## 本轮唯一主刀

只做这 3 个脚本的最终收口：

- `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
- `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
- `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`

说人话就是：

- 不再继续改功能方向
- 不再继续补 scene wiring
- 不再继续回 `Primary / Town`
- 只把这 3 个脚本作为一个最小白名单切片，做成：
  - 能过你自己的 pre-sync 闸门
  - 能诚实说明当前是否可直接归仓
  - 如果不能归仓，blocker 也要写死

## 允许的 scope

你这轮只允许：

1. 继续微调这 3 个脚本内部，前提是它直接服务于：
   - compile / pre-sync / contract consistency
2. 跑这 3 个脚本相关的最小自检
3. 跑 `Ready-To-Sync`
4. 如果通过，就只归这 3 个脚本
5. 如果没通过，就停在 blocker，不扩题

## 明确禁止的漂移

这轮明确禁止：

1. 再碰 `Assets/000_Scenes/Primary.unity`
2. 再碰 `Assets/000_Scenes/Town.unity`
3. 再碰 `Assets/Editor/Story/PrimaryTraversalSceneBinder.cs`
4. 再碰 `Assets/Editor/ScenePartialSyncTool.cs`
5. 再碰 `Assets/YYY_Tests/Editor/ScenePartialSyncToolTests.cs`
6. 再碰 `Assets/Editor/Tool_002_BatchHierarchy.cs`
7. 再碰 `Assets/Editor/StaticObjectOrderAutoCalibrator.cs`
8. 再顺手扩成：
   - scene live-apply
   - Town 转化
   - 通用工具
   - 镜头 / UI / 其它运行层尾项

## 完成定义

你这轮只允许落到二选一：

### A｜script-only 已可归仓

必须同时满足：

1. 只动了这 3 个脚本
2. `Ready-To-Sync` 已跑
3. 这 3 个脚本作为 own 路径已 clean
4. 已完成最小白名单归仓
5. 能明确说：
   - 本轮提交 SHA 是什么
   - 之后主线程可以直接拿这 3 个脚本去接 `Primary / Town` 场景

### B｜script-only 仍不能归仓，但 blocker 已写死

必须同时满足：

1. 你没有偷开新功能
2. 你没有继续碰 scene / binder / 工具
3. 你能准确说明：
   - 为什么还不能 `Ready-To-Sync` 或不能归仓
   - blocker 是你 own 路径内部问题，还是 shared-root / compile / 外部 dirty 问题
   - 下一步最小恢复动作是什么

## 这轮不允许说成“已完成”的东西

以下内容这轮都不允许 claim：

- `Primary` 已完成
- `Town` 已完成
- 场景已接回
- binder 已完成
- 工具线已完成
- 用户入口体验已完成

你最多只能 claim：

- 这 3 个脚本的 contract / compile / pre-sync / script-only checkpoint 已完成

## 固定回执格式

必须按下面顺序回复，不要改顺序，不要省字段：

### A1 保底六点卡

- 当前主线：
- 这轮实际做成了什么：
- 现在还没做成什么：
- 当前阶段：
- 下一步只做什么：
- 需要用户现在做什么：

### A2 用户补充层（可选）

- 只补用户做决策真正需要知道的内容
- 不要把技术审计内容提前塞到这里

### B 技术审计层

必须显式回答：

- changed_paths：
- 本轮是否只触碰 3 个脚本：
- `git diff --check` 是否通过：
- `Ready-To-Sync` 是否已跑：
- 如果没跑，原因是什么：
- 当前是否已 sync：
- 如果已 sync，提交 SHA：
- 当前 own 路径是否 clean：
- 当前 live 状态是 `ACTIVE / READY / PARKED` 还是 blocker：
- blocker_or_checkpoint：

## 这轮我的裁定

你现在不是该继续拿新活，而是该把这 3 个脚本真正收成一个可交给主线程接场景的最小 checkpoint。

如果你已经能归仓，就直接归。
如果你还不能归仓，就把 blocker 写死后继续 `PARKED`。

不要再多做半刀。

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
