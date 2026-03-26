# 011-从路径checkpoint转入124caccc定点恢复与placeable-live复验

这轮我先把上一轮回执的边界彻底钉死：

1. 我接受你上一轮只读回看得到的 3 个事实：
   - 最后一个优先认定的可工作基线是 `124caccc`
   - 当前最小恢复切口优先落在 `GameInputManager.cs / FarmToolPreview.cs`
   - 当前恢复路径优先判断为 `selective restore`
2. 我不接受你把这 3 个事实包装成“`010` 已完成”。
3. 治理审查已经正式裁定：
   - 你上一轮只能算 `readback / 恢复路径选定 checkpoint`
   - 不能算放置链已恢复

因为你自己已经承认：

- 还没执行业务代码 restore / rollback / forward fix
- 恢复向 live 是 `0` 组
- `远停 / 无法放置 / 全场幽灵` 3 个坏现象都还在
- 当前也还没恢复到“至少不比旧基线更差”

所以这轮不允许你再交一轮“我已经看明白了”的策略卡。

---

## 一、当前已接受的基线

当前农田线我只接受这些基线：

1. `010` 仍然是当前生效的治理主文档：
   - 目标不是 hover 单点
   - 而是整条 placeable / 放置交互事故处理
2. 你上一轮 checkpoint 已经提供了一个可执行的工作假设：
   - `124caccc` 是优先恢复锚点
   - `GameInputManager.cs / FarmToolPreview.cs` 是第一切口
3. 当前 placeable 主链关键文件里：
   - `PlacementManager.cs / PlacementNavigator.cs` 没有新的 direct dirty
   - 这意味着第一刀不该先泛化成“整个 placement 子系统都得推翻”
4. 当前必须保住而不能跟着误伤的链仍然是：
   - 真正有效的工具 runtime 改动
   - 真正有效的玩家反馈改动
   - 真正有效的箱子链修正
   - 已经证明有价值的 Tooltip / 背包基础链
5. 用户最新又补了一条 placeable 语义，而且这条不是“顺手美化”，而是本轮必须一起收的正确性要求：
   - 放置出来的物体不应该继续挂在场景根目录下面
   - 如果它属于当前层级 / 当前图层的对象，就应该挂到对应层级容器下面
   - 例如当前图层在 `SCENE/LAYER */Props` 下，就应落到对应层级对象下面，而不是一串 `(Clone)` 全刷在根节点

---

## 二、这轮唯一主刀

### 这轮唯一主刀固定为：

> 不再停在恢复路径选择，  
> 直接以 `124caccc` 为锚点，对 placeable / preview 主链做真实 `selective restore`，  
> 并在同一轮用代表性 live 证明当前状态至少不比旧基线更差。

更直白一点：

- 这轮不是 docs-only
- 这轮不是 checkpoint-only
- 这轮也不是再交一轮“问题定位更清楚了”

这轮你必须真正把恢复动作做下去。

---

## 三、这轮先做什么，后做什么

### 第一刀固定顺序

1. 先以 `124caccc` 为锚点，针对：
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
   - `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
   里 placeable / preview 直接相关的改动做真实 `selective restore`
2. 先不要一上来扩到整条 farm 大面
3. restore 完第一刀后，立刻跑恢复向 live

### 只有在第一刀不足以把系统拉回“不比旧基线更差”时

你才允许在同一轮里继续扩到直接绑定链：

- `Assets/YYY_Scripts/Service/Player/PlayerInteraction.cs`
- `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
- `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
- `Assets/YYY_Scripts/Data/Core/InventoryItem.cs`
- `Assets/YYY_Scripts/Data/Core/ToolRuntimeUtility.cs`
- `Assets/YYY_Scripts/Data/Items/ToolData.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementNavigator.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`

但前提必须是：

- 你已经先做完第一刀 restore
- 并且第一刀 live 证明“只动 `GameInputManager / FarmToolPreview` 还不够”

---

## 四、这轮必须回答的新问题

你这轮不再回答“恢复策略是什么”，而必须回答这些实施问题：

1. 你实际 restore 了哪些行为，而不是只说 restore 了哪些文件
2. `124caccc` 相比当前事故态，具体哪几条 placeable 行为被你拿回来了
3. 第一刀 restore 后：
   - `远停`
   - `无法放置`
   - `全场幽灵`
   这 3 条里哪些已经消失，哪些只改善，哪些还在
4. 如果第一刀不够，你为什么扩刀，以及扩刀后又拿回了什么
5. 哪些之前已证明有效的链被你保住了，没有被 restore 一起带坏
6. 当前 placeable 实例的 parent 语义为什么会掉到根目录；正确的目标 parent 规则是什么

---

## 五、这轮允许你怎么做

### 允许：

1. 直接对 `GameInputManager.cs / FarmToolPreview.cs` 做 `selective restore`
2. 必要时先从 `124caccc` 对照当前 working tree 精确摘回 placeable / preview 相关逻辑
3. 在同一轮里保留当前确实好的 runtime / feedback / chest 改动
4. 如果第一刀 restore 不够，允许继续做小范围 forward fix
5. 跑 focused live，但每组都必须服务于“placeable 主链恢复”
6. 如果这轮需要改现有场景 / parent 归属规则，你必须先按场景规则做最小审视：
   - 原有层级 / parent 配置是什么
   - 现在为什么会掉到根目录
   - 你建议改成什么 parent 解析规则
   - 改完后会落到哪个层级容器
   - 对原有功能影响是什么

---

## 六、这轮明确禁止

### 不允许：

1. 不准再交纯分析 / 纯策略 / 纯 checkpoint 回执
2. 不准再把这轮缩成 hover-only
3. 不准用 `0` 组 live 交账
4. 不准在还比旧基线更差时声称“主线已回正”
5. 不准一上来做没有依据的全量大回退
6. 不准把这轮漂到无关子系统
7. 不准只修“能不能放下”，却继续放任 placeable 实例刷在错误的 Scene 根层级

---

## 七、这轮完成定义

只有满足下面任一结局，这轮才算完成。

### 结局 A：placeable 主链已恢复到可工作基线

你要明确给出：

1. 实际采用的恢复路径：
   - `selective restore`
   - 或 restore 后追加的局部 `forward fix`
2. 第一刀实际 restore 了哪些行为
3. 当前至少这几类现场已经回正：
   - 玩家不会在很远处就停
   - 代表性 placeable 可以真正放置成功
   - 不再全场到处都是幽灵预览
   - 成功 / 失败边界至少重新变得可理解
4. 哪些好的链被你保住了，没有因为 restore 一起打坏
5. 代表性放置物的层级归属已经回正：
   - 不再默认落在场景根目录
   - 会落到当前层级对应的正确 parent / container 下

### 结局 B：整链已恢复到“不比旧基线更差”，并收敛成单一剩余点

如果你判断这轮来不及把所有放置体验完全做漂亮，也可以接受。

但前提是你必须先做到：

1. 当前状态已经至少不比 `124caccc` 更差
2. 用户最痛的 3 个事故态已经不再同时成立：
   - 远停
   - 根本放不下
   - 全场幽灵
3. 剩余问题已经被压成单一剩余点
4. 你必须明确说明：
   - 第一刀 restore 了什么
   - 后续补刀了什么
   - 接下来唯一剩余点是什么
5. placeable 的 parent 归属至少已经回到“用户可理解、和当前层级一致”的状态

如果当前仍然整体更差，这轮不算完成。

---

## 八、这轮 live 至少覆盖这些代表场景

1. 一个代表性 placeable：
   - 走到边上即可放下
   - 不会远停
2. 一个箱子类 placeable：
   - 不会 retry 三次后原地停
   - 不会明明到边上却失败
3. 一个树苗 / 树相关场景：
   - 连点 / 移动中点击不再轻易出幽灵
4. hover sanity：
   - 不能出现“为了救 placeable，hover 又把整链拖坏”
5. hierarchy sanity：
   - 代表性 placeable 放下后，不再挂在场景根目录
   - 会进入当前层级对应的正确 parent / container

每组 live 都必须：

- 先写清只验证什么
- 最多跑几次
- 拿到什么信号立刻 `Stop`
- 结束后退回 `Edit Mode`

---

## 九、下一次回执固定格式

- 当前在改什么
- 这轮是否仍把它当成“整条放置链回归事故处理”，而不是 hover 单点
- 当前认定的最后可工作基线是谁
- 这轮最终采用的是 `selective rollback / selective restore / forward fix` 哪一条
- 第一刀是否已实际执行 `GameInputManager / FarmToolPreview` 的定点 restore
- 如果第一刀后又扩刀，扩到了哪些直接绑定链，为什么
- 你实际 restore / 恢复了哪些关键行为
- changed_paths
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
