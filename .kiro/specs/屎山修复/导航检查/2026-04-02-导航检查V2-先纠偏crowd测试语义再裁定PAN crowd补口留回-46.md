# 2026-04-02-导航检查V2-先纠偏crowd测试语义再裁定PAN crowd补口留回-46

先给这轮裁定：

1. 我 **接受** 你已经把：
   - 假 `Tool_002` blocker
   - 假 `queued_action-only` blocker
   都 fresh 塌缩掉了。
2. 我也 **接受** 你已经把“当前最终代码矩阵”真正从零开始重跑完了，而且当前只剩旧 `Crowd raw ×3` 三连红。
3. 但我 **同样接受并采纳** 你这次自我纠偏里最关键的判断：
   - 你当前跑的这个旧 `Run Raw Real Input Crowd Validation`
   - 本质上是在测“点击点放在三只 NPC 堵墙后面，玩家能不能硬挤过去”
   - 这不是当前用户真正要的 crowd 主目标
4. 所以从现在开始，父线程不再允许你继续把这个旧 crowd case 当“玩家 crowd 主线”的完成定义。
5. 你下一轮的唯一主刀已经改判为：
   - **先把 crowd 测试语义纠偏**
   - **再在新语义下裁定你最近两刀 `PlayerAutoNavigator.cs` crowd 补口，到底该留还是该回**

---

## 一、当前已接受的基线

这些已经接受，不准回漂：

1. `-25` = `carried partial checkpoint`
2. `-29` = `carried partial checkpoint`
3. 当前最终代码矩阵里这些结果成立：
   - `EndpointNpcOccupied raw ×3`：绿
   - `Ground raw matrix ×1`：绿
   - `SingleNpcNear raw ×2`：绿
   - `MovingNpc raw ×1`：绿
   - `NpcAvoidsPlayer ×1 / NpcNpcCrossing ×1`：绿
4. 旧 `Crowd raw ×3` 三连红是事实
5. 但这个旧 crowd case **不再允许** 继续直接代表“玩家 crowd 主线是否过线”
6. 当前仍不能关闭的点：
   - `右键停位偏上 / 用户可视停位怪`

---

## 二、这轮唯一主刀

这轮不再先修 runtime。

唯一主刀改成：

- **crowd 验证语义纠偏**

更具体地说，你这轮必须先把 crowd 拆成下面两类，并给出各自的 case / outcome / pass 口径：

1. `可通过通道`
   - 玩家目标：正常绕/穿过近距 NPC 通道
   - 不允许长时间慢爬、反复倒转、鬼畜式来回切
2. `静止 NPC 堵墙`
   - 玩家目标：不再以“硬撞穿过去”为目标
   - 合法结果应是：
     - 停
     - 绕
     - 换路
     - 或明确失败语义
   - 不允许继续把“没撞穿墙”自动记成 fail

---

## 三、允许的 scope

这轮允许动：

1. `[NavigationLiveValidationRunner.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs)`
2. `[NavigationLiveValidationMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/Editor/NavigationLiveValidationMenu.cs)`
3. 如确实需要，为新语义补最小的结果结构 / outcome 枚举 / report 字段
4. 记忆与开发日志

这轮 **不默认允许** 动：

1. `[PlayerAutoNavigator.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs)`

只有在你先完成“新语义定义 + fresh 重跑”，并能明确证明“当前两刀 crowd 补口在新语义下是错的”时，才允许你在结尾给出：

- `建议保留`
- `建议回退`

但这轮默认 **先不实际回退，也不继续新增 PAN crowd 补口**。

---

## 四、明确禁止的漂移

这轮不准做这些事：

1. 不准继续把旧 `Crowd raw` 当主 acceptance case
2. 不准先改 `PlayerAutoNavigator.cs`，再倒回来补语义
3. 不准再回 blocker 解释层
4. 不准去碰 solver / NPC roam / scene / UI / chat
5. 不准把“新语义下的 blocked-wall case 不再要求撞穿”偷换成“什么都不做也算 pass”
6. 不准把：
   - `Ground raw matrix pass=True`
   - 偷换成
   - `右键停位偏上已关闭`

---

## 五、这轮 completion 定义

### A. 这轮可以被判成“crowd 测试语义已纠偏”，只有同时满足：

1. 你明确拆出了两个 dedicated case：
   - `PassableCorridor`（名字可不同，但语义必须一致）
   - `StaticNpcWall`（名字可不同，但语义必须一致）
2. 你明确给出了每类 case 的：
   - 目标语义
   - outcome 分类
   - pass / fail 判定
3. 旧 `Crowd raw` 被显式降级：
   - 要么改名为 `legacy / blocked-wall stress`
   - 要么明确标记“不再作为玩家 crowd 主 acceptance`
4. 你在 **当前最新代码** 上 fresh 重跑：
   - `PassableCorridor ×3`
   - `StaticNpcWall ×3`
5. 你补最小护栏确认 runner/menu 语义纠偏没有带坏：
   - `EndpointNpcOccupied raw ×1`
   - `Ground raw matrix ×1`
   - `SingleNpcNear raw ×1`
   - `MovingNpc raw ×1`
6. 最后你必须显式裁定：
   - 当前两刀 `PlayerAutoNavigator.cs` crowd 补口，在新语义下是
     - `建议保留`
     - 还是 `建议下轮回退`

### B. 这轮明确不允许宣称的东西

1. 不准写：
   - `crowd 已修好`
   如果你只是把测试语义换了，但还没 fresh 重跑新 case
2. 不准写：
   - `blocked-wall case 现在 pass`
   但 pass 其实只是“没有真正定义失败语义”
3. 不准写：
   - `右键停位偏上已关闭`

---

## 六、建议打法

顺序固定为：

1. 先冻结当前两刀 PAN crowd 补口，不再新增 runtime 改动
2. 在 runner/menu 里补两类新 case
3. 先把旧 `Crowd raw` 降级成 legacy 或 blocked-wall stress
4. 在当前代码上跑：
   - `PassableCorridor ×3`
   - `StaticNpcWall ×3`
   - 护栏最小回归
5. 最后再判断：
   - 当前两刀 crowd 补口该留还是该回

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

并且这轮必须额外显式回答 5 句：

1. 旧 `Crowd raw` 现在被你改判成什么语义类别
2. 新 `PassableCorridor ×3` 结果是什么
3. 新 `StaticNpcWall ×3` 结果是什么
4. 你现在对最近两刀 `PlayerAutoNavigator.cs` crowd 补口的裁定是 `建议保留` 还是 `建议下轮回退`
5. 你现在还能不能把“右键停位偏上”写成已关闭

### B. 技术审计层

至少逐项回答：

1. 当前在改什么
2. `-25` 当前如何定性；直接写 `carried partial checkpoint`
3. `-29` 当前如何定性；直接写 `carried partial checkpoint`
4. 这轮是否新增 runner/menu case；新增了哪些
5. 旧 `Crowd raw` 现在被你如何降级 / 改名 / 标记
6. `PassableCorridor ×3` 结果
7. `StaticNpcWall ×3` 结果
8. `EndpointNpcOccupied raw ×1` 结果
9. `Ground raw matrix ×1` 结果
10. `SingleNpcNear raw ×1` 结果
11. `MovingNpc raw ×1` 结果
12. 这轮是否实际改了 `PlayerAutoNavigator.cs`；直接写 `是/否`
13. 如果没有，当前两刀 crowd 补口你建议 `保留 / 下轮回退`，为什么
14. 如果仍 fail，新的第一责任点是什么
15. 当前是否还能把“static / 右键停位偏上”写成已关闭；如果不能，直接写 `不能`
16. changed_paths
17. 当前 own 路径是否 clean
18. blocker_or_checkpoint
19. 一句话摘要
20. 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
21. 如果没跑，原因是什么
22. 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住

---

## 八、这轮停发边界

这轮只有三种合法收口：

### 1. crowd 语义已纠偏，新 case 已跑出 fresh 结果，并明确裁定最近两刀 PAN crowd 补口该留还是该回

或

### 2. 新 case 已建好，但其中一类 fresh 仍红，且第一责任点已经被明确压回某个更具体的 PAN crowd 分支

或

### 3. 你在纠偏 runner/menu 时撞到了一个新的、晚于当前所有证据的 fresh blocker，并给出了 exact lines

除此之外，不准再给第四种“明知旧 crowd 语义错了，还继续拿它测 runtime”的 partial checkpoint。

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
