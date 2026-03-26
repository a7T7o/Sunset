# 012-外部 blocker 解除后立即完成 placeable live 与卫生清扫

这轮我先把你上一轮回执和当前 shared root 现场之间的冲突钉死：

1. 我接受你上一轮只成立了一个 checkpoint：
   - 你已经把 placeable 的 parent/container 代码口径往 `SCENE/LAYER */Props` 收回了；
   - 也补了 second-blade runner / menu 的 live 入口准备。
2. 我不接受你继续把这件事包装成“只能等外部 blocker 清掉后再说”。
3. 因为治理线程已经主动做了 shared root refresh / compile 与 Console 复核，当前现场事实是：
   - 你上一轮拿来停车的 `_panelVelocity` 外部 blocker 已经不再成立；
   - 当前 Console 已经清到 `0` 条 `error/warning`。
4. 所以从现在开始：
   - 你不能再交“代码口径已改，但 live 还没启动”的回执；
   - 你也不能继续拿 blocker 当默认停车位；
   - 这轮必须把“卫生清扫 + same-round placeable live”一起收掉。

---

## 一、当前已接受的基线

当前农田线我只接受这些基线：

1. `124caccc` 仍是优先恢复锚点；
2. `011` 只成立到“真实 restore 已开始，但 live 证据仍未拿到”；
3. 当前仍不能 claim：
   - `远停` 已压掉
   - `无法放置` 已压掉
   - `全场幽灵` 已压掉
4. 用户新增的 parent 语义要求是硬要求，不是顺手美化：
   - 放置物不应继续默认落在场景根目录；
   - 应落到当前层级对应的正确 parent / container 下。

---

## 二、这轮唯一主刀

### 这轮唯一主刀固定为：

> 先把农田自家当前脏范围报实并清卫生，  
> 再在同一轮 fresh second-blade live 里，真正证明 placeable 主链已经恢复到“至少不比 `124caccc` 更差”，并且 parent/container 不再落根目录。

更直白一点：

- 这轮不是再讲 restore 策略；
- 这轮不是再交“runner 已准备好”；
- 这轮必须真正起跑 live。

---

## 三、这轮先做什么，后做什么

### 第一步：先做 own hygiene

你必须先把当前 own dirty 报实并清楚归类：

当前 shared root 至少还挂着这些农田相关 dirty：

- `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementSecondBladeLiveValidationRunner.cs`
- `Assets/YYY_Scripts/Service/Placement/Editor/PlacementSecondBladeLiveValidationMenu.cs`

这轮你必须先回答：

1. `GameInputManager.cs` 当前这份 dirty 到底是不是你这条线 own 的；
2. 如果不是本轮必要内容，就先回退；
3. 如果是本轮必要内容，就明确纳入 `changed_paths`；
4. 不准再出现“实际 dirty 还在，但回执里不认领”的情况。

### 第二步：再做 same-round placeable live

在 hygiene 收完后，同一轮 fresh 至少重跑：

1. 一个代表性普通 placeable
   - 目标回答：还会不会远停 / 放不下
2. 一个箱子类 placeable
   - 目标回答：放置导航和判定是否一致
3. 一个树苗 / 树相关场景
   - 目标回答：还会不会全场幽灵
4. hierarchy sanity
   - 目标回答：放下后 parent 是否进入正确 container，而不是场景根目录
5. hover sanity
   - 目标回答：placeable 恢复有没有把 hover 链重新拖坏

这轮不接受 `0` 组 live。

---

## 四、这轮允许做什么

### 允许：

1. 继续以 `124caccc` 为锚点做 `selective restore + 局部 forward fix`；
2. 只围绕这些热区继续最小修正：
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
   - `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
   - `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
   - `Assets/YYY_Scripts/Service/Placement/PlacementSecondBladeLiveValidationRunner.cs`
   - `Assets/YYY_Scripts/Service/Placement/Editor/PlacementSecondBladeLiveValidationMenu.cs`
3. 如果 current parent/container 规则还不够，允许继续最小补口；
4. 允许你在 live 中直接验证 `SCENE/LAYER */Props` 与当前 farm layer `propsContainer` 的落点解析；
5. 允许你保留此前已经证明有效的：
   - 工具 runtime / 玩家反馈链
   - 箱子链
   - Tooltip / 背包基础链

---

## 五、这轮明确禁止

### 不允许：

1. 不准继续拿外部 blocker 停车；
2. 不准继续交 `0` 组 live；
3. 不准再把这轮缩成 hover-only；
4. 不准只说“代码口径不再默认落根目录”，却不给 actual hierarchy 证据；
5. 不准一上来泛修整个 farm 大面；
6. 不准回避 `GameInputManager.cs` 当前 dirty 的归属问题。

---

## 六、这轮完成定义

只有满足下面任一结局，这轮才算完成。

### 结局 A：placeable 主链已恢复到可工作基线

你要明确给出：

1. hygiene 已完成，own dirty 已报实；
2. same-round fresh live 已真正跑起来；
3. 当前至少这些现场已经回正：
   - 玩家不会在很远处就停
   - 代表性 placeable 可以真正放置成功
   - 不再全场到处都是幽灵预览
   - 代表性放置物不再落在场景根目录
   - 会进入当前层级对应的正确 parent / container
4. hover 链没有被一起打坏；
5. 当前状态已至少不比 `124caccc` 更差。

### 结局 B：先恢复到“不比 `124caccc` 更差”，并压成单一剩余点

如果这轮仍来不及把整个 placeable 体验完全做漂亮，也可以接受。

但前提是你必须先做到：

1. hygiene 已完成；
2. 代表性 live 已真正跑过；
3. `远停 / 无法放置 / 全场幽灵` 不再三项同时成立；
4. parent/container 归属已经回到用户可理解状态；
5. 剩余问题已经被压成单一剩余点。

如果这轮仍然没有 fresh live，这轮不算完成。

---

## 七、live 纪律继续钉死

1. 每组 live 前先写清：
   - 这组只验证什么
   - 最多跑几次
   - 看见什么现象立刻 `Stop`
2. 拿到足够证据就立刻 `Stop`
3. 结束后必须退回 `Edit Mode`
4. 如果你自己肉眼仍然能看到：
   - 很远就停
   - 明明到边上还放不下
   - 到处是幽灵
   - 放下后还挂在场景根目录
   就不准因为某个 validator 字段好看而 claim 通过

---

## 八、下一次回执固定格式

- 当前在改什么
- 这轮是否仍把它当成“整条放置链回归事故处理”，而不是 hover 单点
- 当前认定的最后可工作基线是谁
- 这轮最终采用的是 `selective rollback / selective restore / forward fix` 哪一条
- 当前真实 `changed_paths`
- 这轮先清掉了哪些 own hygiene；`GameInputManager.cs` 当前归属是什么
- 第一刀是否已实际执行 `GameInputManager / FarmToolPreview` 的定点 restore
- 如果第一刀后又扩刀，扩到了哪些直接绑定链，为什么
- 你实际 restore / 恢复了哪些关键行为
- 当前保住了哪些之前已证明有效的链，没有被回退一起打坏
- 实际跑了哪几组代表性 live
- `远停 / 无法放置 / 全场幽灵` 这 3 个坏现象里，哪些已经被压掉，哪些还在
- 代表性放置物当前会挂到哪个 parent / container；是否还会落在场景根目录
- 当前是否已经恢复到“至少不比 `124caccc` 更差”
- 如果还没完全收口，新的单一剩余点是什么
- live 是否都在拿到证据后立刻 `Stop`
- 当前是否已退回 `Edit Mode`
- blocker_or_checkpoint
- 一句话摘要
