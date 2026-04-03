# 导航检查V2：只收最终验收交接，并与 Primary traversal 独立分账

## 一、先接受新的三方分工

从现在开始，导航线不是一条大锅，必须先接受下面 3 条事实：

1. **用户已认可当前 PAN / crowd 导航版本**
   - 当前玩家右键导航版本已经被用户明确认可
   - `PassableCorridor / StaticNpcWall` 的 red 继续保留，但现在只再算：
     - `targeted probe / 后续 polish 诊断项`
   - 不再作为当前版本“不可用”的主结论

2. **父线程 `导航检查` 不再继续主刀 crowd runtime**
   - 它这轮会把 `Primary traversal` 剩余 scene integration / live closure 从 PAN 主线里单独拆出来做
   - 也就是说：
     - `Primary traversal` 从现在起是父线程的独立切片
     - 不是你这条验收线程要继续混报的内容

3. **工具-V1 只保留 3 个脚本 contract**
   - `NavGrid2D.cs`
   - `PlayerMovement.cs`
   - `SceneTransitionTrigger2D.cs`
   - 它不再 own scene / binder / tool

所以你这轮新的唯一身份就是：

**只收 runner/menu 最终验收交接，并且把“当前玩家导航版本”和“Primary traversal 剩余闭环”明确拆开报实。**

---

## 二、这轮唯一主线

把你这条 runner/menu 验收线程，正式收成：

1. **当前玩家导航版本已被用户真实入口体验认可**
2. **但 targeted probe 仍保留两个诊断红面**
3. **`Primary traversal` 是父线程新的独立切片，不属于这条 handoff 的完成范围**

你这轮不是继续修：

- `PlayerAutoNavigator.cs`
- `PassableCorridor`
- `StaticNpcWall`
- `Primary traversal`
- `Primary.unity`
- scene / binder / hotfile

---

## 三、这轮明确禁止

1. 不准再碰：
   - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
   - 任何 PAN runtime
   - `Primary.unity`
   - `PrimaryTraversalSceneBinder.cs`
   - 任何 scene / prefab / hotfile

2. 不准再把这轮主目标写成：
   - 继续修 `PassableCorridor`
   - 继续修 `StaticNpcWall`
   - 继续追 final pack 全绿

3. 不准把 `Primary traversal` 混进你这轮“当前导航版本已认可”的验收结论里

4. 不准为了让结果更好看而：
   - 偷改 pack 阈值
   - 偷改 case 语义
   - 偷把 targeted probe 红面写成“已关闭”

5. 不准把：
   - `GroundRawMatrix`
   - 或 `center-only` 结构绿
   写成：
   - `右键停位偏上已关闭`

6. 这轮默认**不需要**再 fresh 重跑 `final acceptance pack`
   - 除非你发现上一轮 exact 结果写错
   - 否则只做只读复核 + handoff + sync 判定

---

## 四、你这轮必须先接受的新定性

从现在开始，这条线的完成定义要同时分成下面三层，不准再混成一个结论：

### 1. 真实入口体验层

- 当前玩家导航版本：**用户已认可 / 可接受**

### 2. targeted probe / 局部验证层

- `FinalPlayerNavigationAcceptancePack` 仍有红面：
  - `PassableCorridor ×3`
  - `StaticNpcWall ×3`
- 它们现在的正确身份是：
  - **后续 polish 诊断项**
  - **不是当前版本 release blocker**

### 3. 结构 / 验收工具层

- 你的：
  - `NavigationLiveValidationRunner.cs`
  - `NavigationLiveValidationMenu.cs`
  - `FinalPlayerNavigationAcceptancePack`
  已经完成

此外你必须额外再拆一层：

### 4. 分账层

- `Primary traversal` 剩余闭环：
  - 是父线程新的独立切片
  - 不属于你这轮 handoff 的“已完成范围”

---

## 五、这轮只允许做的动作

### 1. 先只读复核

只读回看以下事实是否一致：

- 你上一轮最新回执
- `FinalPlayerNavigationAcceptancePack` latest 红绿总图
- 当前用户最新认可裁定
- 父线程新的 `Primary traversal` 独立切片分工
- 当前你 own 的文件仍只限于：
  - `NavigationLiveValidationRunner.cs`
  - `NavigationLiveValidationMenu.cs`
  - 本轮 handoff / memory 文档

### 2. 再收“最终验收交接稿”

你这轮必须把结果收成一份明确面向用户/治理的最终验收交接稿，至少要写清：

1. 当前玩家导航版本为什么可以被视为：
   - **用户真实体验已接受**
2. 为什么同时仍保留：
   - `PassableCorridor / StaticNpcWall` targeted probe 红面
3. 这些红面现在的正确身份是什么：
   - **后续 polish 诊断项**
4. 为什么 `Primary traversal` 不在这轮 handoff 已完成范围里：
   - 因为它已经被父线程拆成新的独立切片
5. 哪些事仍然不能 claim：
   - `右键停位偏上已关闭`
   - `final acceptance pack 全绿`
   - `整个导航系统全线完成`

### 3. 最后只做一次最小 sync 判定

只允许做一次最小 `Ready-To-Sync` 判定：

- 如果你 own 路径已经合法可 sync，就同步你 own 的 runner/menu + 文档
- 如果 `Assets/YYY_Scripts/Service/Navigation` 同根 blocker 仍在，而且是父线程或别的线程 remaining dirty：
  - 不要清 foreign dirty
  - 不要扩大 own 范围
  - 直接把状态报成：
    - `用户验收事实已完成`
    - `归仓被 Navigation 同根 blocker 合法阻断`

---

## 六、这轮完成标准

你这轮只有满足下面全部条件，才算完成：

1. 你已经明确写入：
   - 当前玩家导航版本已被用户认可

2. 你已经明确区分四层：
   - 真实入口体验：已认可
   - targeted probe：仍有两个诊断红面
   - 结构工具层：pack 已完成
   - 分账层：`Primary traversal` 已移交父线程独立处理

3. 你没有再碰：
   - `PlayerAutoNavigator.cs`
   - runtime
   - scene / prefab / hotfile

4. 你已经重新给出：
   - 当前 own 路径是否 clean
   - 当前能否 sync
   - 如果不能，第一真实 blocker 是谁

5. 你已经把 Unity 留在：
   - `Edit Mode`

---

## 七、固定回执格式

### A1. 用户可读汇报层

至少写清：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

并且必须直接报实：

- 当前玩家导航版本是否已被用户认可：`是/否`
- targeted probe 红面现在是否仍保留：`是/否`
- 这些红面当前身份是不是 release blocker：`是/否`
- `Primary traversal` 是否属于你这轮 handoff 已完成范围：`是/否`

### A2. 用户补充层

必须额外显式回答：

1. 你这轮有没有重跑 live；如果没有，为什么合法不需要
2. `FinalPlayerNavigationAcceptancePack` 当前 latest 红绿总图
3. 为什么它不能代表“整个导航系统全线完成”
4. 为什么 `Primary traversal` 必须独立报，不准并入当前 handoff

### B. 技术审计层

至少补：

- 当前在改什么
- 这轮是否重跑 live；如果没有，为什么合法不需要
- `final acceptance pack` 当前 latest 红绿总图
- 当前是否还能把“右键停位偏上”写成已关闭
- changed_paths
- 当前 own 路径是否 clean
- blocker_or_checkpoint
- `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态

---

## 八、thread-state

如果你这轮从只读进入真实施工或要更新 tracked 文档：

1. 先补或重开 `Begin-Slice`
2. 若尝试 sync，必须跑 `Ready-To-Sync`
3. 若这轮停在 blocker 或停车态，必须跑 `Park-Slice`

这轮结束前必须报实：

- `Begin-Slice=是否已跑`
- `Ready-To-Sync=是否已跑`
- `Park-Slice=是否已跑`
- 当前是 `ACTIVE / READY / PARKED / BLOCKED`

---

## 九、一句话总结

**这轮 V2 不再继续修导航，而是把“用户已认可当前玩家导航版本”正式收成最终验收交接，并明确与父线程新拆出的 `Primary traversal` 切片分账；targeted probe 红面可保留为后续 polish 诊断项，但不准再把它们写成当前版本不可用。**
