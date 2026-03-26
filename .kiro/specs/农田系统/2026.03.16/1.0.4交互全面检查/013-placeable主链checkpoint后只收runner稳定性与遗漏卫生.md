# 013-placeable 主链 checkpoint 后只收 runner 稳定性与遗漏卫生

这轮我先把你的上一轮回执做正式裁定：

1. 我接受你已经拿到了一个可用 checkpoint：
   - placeable 主链当前可以暂时认定为“至少不比 `124caccc` 更差”；
   - parent/container 不再默认落场景根目录；
   - 代表性箱子放置、preview 原格重刷、箱子存读链已经有 same-round live 证据。
2. 但我不接受你把这轮直接包装成“基本收完了”。
3. 因为当前还有两个硬尾巴没有收口：
   - 你这轮回执仍然漏报了 own untracked：
     - `Assets/YYY_Scripts/Farm/FarmRuntimeLiveValidationRunner.cs`
     - `Assets/YYY_Scripts/Farm/Editor/FarmRuntimeLiveValidationMenu.cs`
   - 你自己也承认当前唯一剩余点是：
     - `SaplingGhostOccupancy` 的 runner 取样偶发 timeout

所以这轮开始：

- 不再回头重打 placeable 主链大面；
- 只准把 `runner 稳定性 + 漏报 hygiene` 这两个尾巴收掉。

---

## 一、当前已接受的基线

当前农田线我只接受这些事实：

1. `124caccc` 仍是恢复锚点；
2. `012` 已把主链事故从：
   - 远停
   - 无法放置
   - 全场幽灵
   收缩到“主链已回正，剩 runner 稳定性尾巴”；
3. `SCENE/LAYER 1/Props/Farm` 这种非根目录 parent/container 口径，现在可以进入后续验收候选；
4. `GameInputManager.cs` 本轮可以暂时接受为：
   - 本线 own 的最小兼容补口
   - 不是借机把无关 runtime WIP 一起塞回来

但还不能宣称整条线 fully clean。

---

## 二、这轮唯一主刀

### 这轮唯一主刀固定为：

> 不再回头泛修 placeable 主链，  
> 只收 `SaplingGhostOccupancy` 的 runner 取样稳定性，  
> 外加把漏报的 `FarmRuntimeLiveValidationRunner / FarmRuntimeLiveValidationMenu` hygiene 处理干净。

更直白一点：

- 这轮不是再讲主链已经多好；
- 这轮不是再扩 `PlacementManager` 大逻辑；
- 这轮只回答：
  - 树苗场景为什么偶发 timeout
  - 它现在怎么被压掉
  - 那两份 untracked farm live 文件到底保留还是删除

---

## 三、这轮必须先做的 hygiene

你必须先处理这两份当前仍漏报的 own 文件：

1. `Assets/YYY_Scripts/Farm/FarmRuntimeLiveValidationRunner.cs`
2. `Assets/YYY_Scripts/Farm/Editor/FarmRuntimeLiveValidationMenu.cs`

你必须明确给出二选一结论：

### 结局 A：它们仍然是当前有效资产

那就：

1. 明确纳入 `changed_paths`；
2. 说明它们现在在整个农田 live 验证里的职责；
3. 不准再漏报。

### 结局 B：它们只是旧轮残留

那就：

1. 直接删除 / 回退；
2. 说明为什么当前 `012/013` 已不再需要它们；
3. 不准继续让它们 untracked 漂着。

---

## 四、这轮 live 范围必须缩窄

这轮 fresh live 只准围绕：

1. `SaplingGhostOccupancy`
   - 目标：压掉偶发 timeout，拿到稳定通过样本
2. 一个最小回归 sanity
   - 只能在你确实动了 placeable 主链逻辑时才补跑
   - 优先 `PreviewRefreshAfterPlacement` 或 `ChestReachEnvelope` 二选一

这轮不准再把整包 second-blade 四场都重刷一遍，除非你又改了主链。

---

## 五、这轮允许做什么

### 允许：

1. 只围绕这些文件做最小修正：
   - `Assets/YYY_Scripts/Service/Placement/PlacementSecondBladeLiveValidationRunner.cs`
   - `Assets/YYY_Scripts/Service/Placement/Editor/PlacementSecondBladeLiveValidationMenu.cs`
   - `Assets/YYY_Scripts/Farm/FarmRuntimeLiveValidationRunner.cs`
   - `Assets/YYY_Scripts/Farm/Editor/FarmRuntimeLiveValidationMenu.cs`
2. 如果树苗 timeout 证明根因不在 runner，而在一个非常窄的 placeable/farm 业务接口，也允许最小补口；
3. 保留已经成立的主链恢复结论，不准自己把它重新打回事故态。

---

## 六、这轮明确禁止

### 不允许：

1. 不准再回头泛修 `PlacementManager`；
2. 不准再把这轮重新包装成“整条 placeable 主链大修”；
3. 不准继续漏报 `FarmRuntimeLiveValidationRunner / Menu`；
4. 不准只交“timeout 偶发，业务主链没问题”这种半句结论，不给 fresh 证据；
5. 不准因为一个 runner 尾巴，再把整个 `012` 主链 checkpoint 重讲一遍。

---

## 七、这轮完成定义

只有下面这个结局才算完成：

### 结局：placeable 主链 checkpoint 保持成立，runner 稳定性与 hygiene 也已收口

你要明确给出：

1. `FarmRuntimeLiveValidationRunner / Menu` 最终是保留还是删除；
2. 当前 `changed_paths` 是否已经把 own 文件报全；
3. `SaplingGhostOccupancy` 这轮 fresh 是否稳定通过；
4. 如果补跑了 1 个最小回归 sanity，它是否继续通过；
5. 当前剩余点是否已经不再是业务主链，而只是可接受的验证工具尾差；
6. 当前是否已退回 `Edit Mode` 并可交用户验收主链。

---

## 八、下一次回执固定格式

- 当前在改什么
- 这轮是否仍只锁 runner 稳定性与遗漏 hygiene
- `FarmRuntimeLiveValidationRunner / Menu` 当前最终如何处置
- 当前真实 `changed_paths`
- `SaplingGhostOccupancy` fresh 结果
- 如果补跑了最小回归 sanity，它跑了哪一组、结果如何
- 当前是否还触碰 placeable 主链业务逻辑；如果触碰，为什么
- 当前是否已退回 `Edit Mode`
- blocker_or_checkpoint
- 一句话摘要
