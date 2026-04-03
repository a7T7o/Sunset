# 2026-04-02-农田交互修复V3-若V2自清失败则立即报实并接盘Inventory-UI-compile-gate-38

这不是立即生效的施工指令。

这是父线程给 `农田交互修复V3` 预备的 **升级接盘 prompt**。

只有当 `导航检查V2 -35` 做完整套 Unity 自清恢复后，仍然证明：

- `InventorySlotUI.cs / ToolbarSlotUI.cs` compile gate 继续稳定真红
- 且无法靠自清恢复消失

才应该发这份。

---

## 一、当前为什么会轮到你

父线程已把这条 incident 压到下面这组事实：

1. 当前 compile gate 真红路径在：
   - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
   - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
2. 当前工作树里，这些 UI 文件以及相关 inventory UI 链有成组 dirty：
   - `Assets/YYY_Scripts/Data/Core/ToolRuntimeUtility.cs`
   - `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
   - `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
   - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
   - `Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs`
   - `Assets/YYY_Scripts/UI/Inventory/ItemTooltipTextBuilder.cs`
   - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
3. `rapid_incident_probe` 的 top owner 家族指向：
   - `农田交互修复V2`
4. 当前还在跑的活线程则是：
   - `农田交互修复V3`
5. 但你当前 `thread-state` own paths **没有**把这些 UI roots 纳进去。

因此如果 `V2 -35` 已证明这条 gate 不是可自清恢复的问题，下一步最该出来报实和接盘的人就是你。

---

## 二、这轮唯一主刀

只做一件事：

- **把当前 inventory / toolbar UI compile gate 的 owner / white-list / dirty 现场彻底报实，并在确认接盘后把它修到 compile clean**

不准顺手回 placement 主线，不准扩到别的体验项。

---

## 三、这轮必须先回答的事

在改代码前，先回答：

1. 当前这些 UI dirty 是否确属你这轮 own 工作的一部分
2. 如果是，为什么 `thread-state` 里没有把：
   - `Assets/YYY_Scripts/UI/Inventory`
   - `Assets/YYY_Scripts/UI/Toolbar`
   - `Assets/YYY_Scripts/Data/Core/ToolRuntimeUtility.cs`
   纳入 own paths / touchpoints
3. 如果不是，为什么当前工作树还带着这些相关 dirty

这三问答不清，不准直接埋头修。

---

## 四、这轮允许的作用域

### 允许

1. `Assets/YYY_Scripts/UI/Inventory`
2. `Assets/YYY_Scripts/UI/Toolbar`
3. `Assets/YYY_Scripts/Data/Core/ToolRuntimeUtility.cs`
4. 你自己的 memory / thread-state / 审计尾账

### 禁止

1. 不准顺手回 `GameInputManager.cs` 主链
2. 不准扩到 placeable / farm / rendering 其它问题
3. 不准把 `导航检查V2` 的 PAN runtime 线一起吞并

---

## 五、这轮完成定义

最小完成：

1. owner / white-list / dirty 现场已报实
2. 当前 compile gate 已消失
3. Unity compile clean
4. 不再阻断 `导航检查V2` 进入 Play / live

---

## 六、固定回执格式

### A1. 用户可读汇报层

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### B. 技术审计层

1. 当前在改什么
2. 当前这些 UI dirty 是否属于你
3. 如果属于你，为什么之前 own paths 没报这部分
4. 当前 compile gate 根因是什么
5. 你具体改了哪些文件 / 哪几个分支
6. 最新 compile / console 结果
7. changed_paths
8. 当前 own 路径是否 clean
9. blocker_or_checkpoint
10. 一句话摘要
11. 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
12. 如果没跑，原因是什么
13. 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住
