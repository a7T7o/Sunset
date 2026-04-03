# 2026-04-02-父线程-UI-compile-gate急诊定责与owner失配说明-37

## 1. 事件摘要

当前阻断 `导航检查V2 / PlayerAutoNavigator` live 入口的 UI compile gate，不只是“有两份 UI 文件在报 `CS0103`”。

父线程继续只读排查后，已经把它收窄成一个 **技术 + owner 双重 incident**：

1. 技术层：
   - `Editor.log` 最新 forced recompile 仍报：
     - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
     - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
     - `CS0103: TickStatusBarFade / ApplyStatusBarAlpha`
2. 源码层：
   - 当前工作树里，这两个文件的方法本体都真实存在；
   - 仓库里没有第二份同名类；
   - 方法调用点 / 定义点标识符字节一致；
   - 文件括号结构正常；
   - 文件无 NUL 脏字节。
3. owner 层：
   - `rapid_incident_probe` 明确把最高嫌疑 owner 压到：
     - `农田交互修复V2` 这一条 inventory / toolbar 线
   - 当前活线程现场里，真正还在跑的是：
     - `农田交互修复V3`
   - 且它现在工作树上 **确实有**：
     - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
     - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
     - `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
     - `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
     - `Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs`
     - `Assets/YYY_Scripts/UI/Inventory/ItemTooltipTextBuilder.cs`
     - `Assets/YYY_Scripts/Data/Core/ToolRuntimeUtility.cs`
     这些相关 dirty；
   - 但 `农田交互修复V3` 当前 `thread-state` 的 own paths 却是：
     - `Assets/YYY_Scripts/Service/Placement`
     - `Assets/YYY_Scripts/Farm`
     - `Assets/YYY_Scripts/Service/Rendering`
     - `Assets/YYY_Scripts/Controller/Input`
   - **不包含** UI / Inventory / Toolbar。

因此这不是普通“外部 UI 红错”，而是：

- **疑似同一条农田交互线在 UI 子根留下了 active dirty，但 thread-state 没有把这部分路径合法纳入 own paths 的 owner 失配现场**

---

## 2. 当前最可信的责任判断

### 第一嫌疑线程

- `农田交互修复V2 / 农田交互修复V3` 这一条 inventory UI 继承线

更准确地说：

- 历史 owner 映射指向 `农田交互修复V2`
- 当前活现场 dirty 则落在 `农田交互修复V3`

所以父线程当前判断是：

- **责任家族高置信是 `农田交互修复V2 -> V3` 这条 inventory / toolbar 线**
- 当前真正该出来报实并接盘的人，更像是：
  - `农田交互修复V3`

### 不是当前第一嫌疑的线程

1. `导航检查V2`
   - 它当前 own 白名单只锁 `PlayerAutoNavigator.cs`
   - 没碰 UI/Inventory/Toolbar tracked 文件
2. `UI`
   - 当前 active slice 是“玩家与NPC对话气泡层级分隔与边界闭环修复”
   - 与 inventory / toolbar compile gate 的证据面不对齐

---

## 3. 当前最该采取的动作

### 对 `导航检查V2`

- 继续按 `-35` 先做 Unity 编译态 / 导入态自清恢复
- 但它不该接管 UI tracked 修复本身

### 对 `农田交互修复V3`

如果 `-35` 证明这条 gate **无法靠自清恢复消失**，那接下来最该接盘的不是 `导航检查V2`，而是：

- `农田交互修复V3`

并且它首先要做的不是“直接修”，而是先回答：

1. 这些 UI dirty 是否确属它这轮 own 工作的一部分
2. 如果是，为什么 `thread-state` 里没把 UI 根纳入 own paths
3. 如果不是，它为什么当前工作树仍带着这些 UI 脏改

---

## 4. 当前阶段判断

这份说明不是让父线程现在立刻抢修 UI。

它的作用是把后续分岔预先压清：

1. `V2` 先做 `-35` 的自清恢复
2. 如果自清成功：
   - 继续 PAN 大闭环
3. 如果自清失败：
   - 不再泛泛写“外部 UI gate”
   - 直接升级成：
     - `农田交互修复V3` owner/white-list/dirty 现场报实与接盘问题

---

## 5. 关键证据

### 当前 dirty

- `Assets/YYY_Scripts/Data/Core/ToolRuntimeUtility.cs`
- `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
- `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
- `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
- `Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs`
- `Assets/YYY_Scripts/UI/Inventory/ItemTooltipTextBuilder.cs`
- `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`

### 当前 owner 映射

- `rapid_incident_probe` top owner：
  - `农田交互修复V2`

### 当前 active 现场

- `农田交互修复V3`
  - `ACTIVE`
  - 但 own paths 不含 UI roots

### 当前 recent commits

- `5e3fe609 2026.03.29_农田交互修复V3_01`
- `e34aa655 2026.03.24_农田交互修复V2_03`
- `0e87c430 2026.03.23_农田交互修复V2_04`
- `c76d7471 2026.03.23_农田交互修复V2_03`

这些都直接触及当前 incident 路径。
