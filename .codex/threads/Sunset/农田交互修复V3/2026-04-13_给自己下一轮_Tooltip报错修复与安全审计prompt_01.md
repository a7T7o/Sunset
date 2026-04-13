# 2026-04-13｜给自己下一轮｜Tooltip 报错修复与安全审计 prompt 01

请先完整读取：

1. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltip.cs`
2. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltipTextBuilder.cs`
3. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Toolbar\ToolbarSlotUI.cs`
4. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotInteraction.cs`
5. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryInteractionManager.cs`
6. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Box\BoxPanelUI.cs`
7. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\memory_0.md`
8. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\UI\2026-04-13_给UI_Tooltip视觉contract与边界说明_01.md`

当前已接受的基线：

1. tooltip 视觉正式面，后续主要交给 UI 线程处理。
2. farm 线程继续自己负责 tooltip runtime 逻辑、报错、边界条件和安全性。
3. 这轮不要再泛修 toolbar / inventory / box 交互体验，不要扩回掉落物、树、placement、day1。

---

## 本轮唯一主刀

### 先修 tooltip runtime 报错，再做一轮 tooltip 逻辑安全审计

你这轮只做两件事：

1. 修掉这条真实运行时报错：
   - `Coroutine couldn't be started because the game object 'RuntimeItemTooltip' is inactive!`
2. 在修完后，只做一轮 `tooltip` 相关的安全/逻辑/边界审计，把问题和后续修法列清楚

---

## 第一部分：必须真实修掉的报错

当前已钉死的真因是：

1. `ToolbarSlotUI.OnPointerExit()` 会调 `ItemTooltip.Instance?.Hide()`
2. `ItemTooltip.Hide()` 目前只挡了 `activeSelf`
3. `ItemTooltip.StartFade()` 直接 `StartCoroutine(...)`
4. 当 tooltip 自己 `activeSelf=true` 但层级上 `activeInHierarchy=false` 时，就会报错

这轮必须把这条报错真正收口。

### 完成定义

1. `RuntimeItemTooltip inactive` 这条协程报错不再出现
2. 修法必须是安全守卫，不得引入新的 show/hide 乱序
3. 不允许为了消报错把 tooltip 直接废掉

---

## 第二部分：修完后只做审计，不继续大改

修完上面的报错后，这轮不要继续大规模改 tooltip 逻辑。

你只允许：

1. 彻查 tooltip 当前可能存在的逻辑漏洞
2. 找出安全问题
3. 找出边界情况
4. 列成清单
5. 再列清下一步修复方案任务清单

### 审计重点

至少检查这些面：

1. show / hide 的 owner 是否单一
2. `ToolbarSlotUI` 与 `InventorySlotInteraction` 是否存在双写 tooltip 状态
3. tooltip 在背包关闭、箱子关闭、页面切换、对象 inactive 时是否还有残留引用
4. fade / hover / followMouse 是否存在 race condition
5. tooltip 在拖拽、长按、ctrl/shift 快捷拿取时是否可能出现不一致
6. tooltip 与状态条、耐久条、选中态是否存在相互抢写
7. 是否还有类似 “activeSelf 可以但 activeInHierarchy 不行” 的同类隐患

---

## 严格禁止

这轮禁止：

1. 大改 tooltip 视觉
2. 重构整套 inventory 交互
3. 顺手改背包/箱子拖拽语义
4. 扩到 `Primary / Town / Placement / Tree / Drop / day1`
5. 把审计中发现的问题顺手全修完

除第一条真实报错外，其他内容这轮只收成：

- 问题清单
- 风险判断
- 修复方案任务清单

---

## 回执必须这样报

先用人话写：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

然后再写技术审计层：

### A. 报错修复

- 改了哪些文件
- 这条报错的真因
- 修法是什么
- no-red 证据

### B. 审计结果

- 发现了哪些潜在安全问题
- 发现了哪些逻辑漏洞
- 发现了哪些边界情况
- 哪些是高优先级
- 哪些可以后移

### C. 下一步任务清单

按优先级列：

1. 下一刀修什么
2. 为什么先修它
3. 哪些先不动

---

## thread-state 接线要求

如果这轮会继续真实施工，先跑：

`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `sync` 前，必须先跑：

`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住、让位或不准备收口，改跑：

`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
