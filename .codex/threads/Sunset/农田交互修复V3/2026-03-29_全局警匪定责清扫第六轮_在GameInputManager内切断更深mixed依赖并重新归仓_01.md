# 2026-03-29｜农田交互修复V3｜全局警匪定责清扫第六轮｜在 GameInputManager 内切断更深 mixed 依赖并重新归仓｜01

你第五轮最有价值的结果，不是“又没上 git”，而是把 blocker 再往里钉了一层：

- same-root blocker 仍然没有回来
- 当前挡住你的，已经不是“缺 `GameInputManager.cs / ToolRuntimeUtility.cs`”
- 而是 `GameInputManager.cs` 自己还在 compile-time 咬更深 mixed-root

所以第六轮不要再继续扩包。

## 本轮唯一主刀

**只在 `GameInputManager.cs` 内，把当前对 `PlayerInteraction.cs` 和 `TreeController.cs` 的 compile-time 依赖切成本地 compat / fallback；然后保持第五轮同一组白名单，重新真实跑 `preflight -> sync`。**

## 当前 blocker 已钉死

第五轮的 first blocker 现在已经很具体：

1. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs:2666`
   - 当前直接读取：
     - `playerInteraction.LastActionFailureReason`
2. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs:3249`
   - 当前直接调用：
     - `targetTree.ShouldBlockAxeActionBeforeAnimation(attacker, tool)`

同时 current working tree 还显示：

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerInteraction.cs` = dirty
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\TreeController.cs` = dirty

这说明：

- 如果你继续把这两个文件带回白名单
- 农田线就会重新掉回 mixed-root 扩包

第六轮明确不允许这么走。

## 允许 scope

### 当前 clean subroots 代码
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementPreview.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmToolPreview.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryInteractionManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryPanelUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotInteraction.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltip.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltipTextBuilder.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\SlotDragContext.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Toolbar\ToolbarSlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\World\Placeable\ChestController.cs`

### 本轮允许继续带回的共享依赖
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\ToolRuntimeUtility.cs`

### own docs / memory
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-03-29_全局警匪定责清扫第一轮认定书_01.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-03-29_全局警匪定责清扫第一轮回执_01.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-03-29_全局警匪定责清扫第二轮执行书_01.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-03-29_全局警匪定责清扫第二轮回执_01.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-03-29_全局警匪定责清扫第三轮_认领归仓与git上传_01.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-03-29_全局警匪定责清扫第三轮回执_01.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-03-29_全局警匪定责清扫第四轮_可自归仓子根收口_01.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-03-29_全局警匪定责清扫第四轮回执_01.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-03-29_全局警匪定责清扫第五轮_最小共享依赖扩包归仓_01.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-03-29_全局警匪定责清扫第五轮回执_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\0.0.1交互大清盘\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\0.0.1交互大清盘\2026-03-27-交互大清盘_最终验收手册.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\0.0.1交互大清盘\2026-03-27-交互大清盘_纯代码最终验收报告.md`

## 这轮明确禁止

1. 不准把 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerInteraction.cs` 带回白名单
2. 不准把 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\TreeController.cs` 带回白名单
3. 不准 broad 带回 `Assets/YYY_Scripts\Service\Player\*`
4. 不准 broad 带回 `Assets/YYY_Scripts\Controller\*`
5. 不准再扩 `Assets/YYY_Scripts\Data\*`，只允许 `ToolRuntimeUtility.cs`
6. 不准顺手继续补业务交互或别的农田功能

## 本轮必须做成的事

### 1. 只在 `GameInputManager.cs` 内做 compat 切口

你要把下面两类 compile-time 依赖切掉：

- `playerInteraction.LastActionFailureReason`
- `targetTree.ShouldBlockAxeActionBeforeAnimation(...)`

允许做法：

- 本地 helper
- reflection / `GetType().GetProperty(...)`
- reflection / `GetType().GetMethod(...)`
- 缺失成员时 fallback 到：
  - `ToolUseFailureReason.None`
  - `false`

这轮目标不是保留全部 rich 行为，而是让“第五轮同一包”在不带 `PlayerInteraction.cs / TreeController.cs` 的情况下也能编译并尝试归仓。

### 2. `GameInputManager.cs` 必须继续显式报触点

回执里的 `touched_touchpoints` 这轮至少要覆盖：

- `ShouldPreservePlacementModeForCurrentRightClick`
- `HandleRightClickAutoNav`
- `TryRejectActiveFarmToolSwitch`
- `AbortFarmToolOperationImmediately`
- `TryBlockAxeActionAgainstHighTierTree`
- `ShouldBlockToolActionBeforeAnimation`

如果你实际还碰了别的农田相关触点，也必须老实补全。

### 3. 仍然用第五轮同一组白名单重跑

这轮不是新扩包。

你必须保持：

- 第五轮同一组 clean subroots
- `GameInputManager.cs`
- `ToolRuntimeUtility.cs`
- own docs / memory

重新真实运行：

1. `preflight`
2. 若放行，再 `sync`
3. 若仍失败，只报第一真实 blocker

## 完成定义

### A｜已把 compat 切口后的归仓组上 git

必须同时满足：

1. 已真实运行 `preflight`
2. 已真实运行 `sync`
3. 已给出提交 SHA

### B｜第一真实阻断已再次钉死

必须同时满足：

1. 已真实运行 `preflight`
2. 若未继续 `sync`，必须给 exact blocker path / reason
3. 必须明确说明：这次 blocker 是否已经不再是 `PlayerInteraction / TreeController`

## 固定回写路径

- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-03-29_全局警匪定责清扫第六轮回执_01.md`

## 第六轮回执必须显式回答

### A1 保底六点卡

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### A2 用户补充层

- 这轮在 `GameInputManager.cs` 内实际切掉了哪两个更深 mixed 依赖
- 这轮的 compat / fallback 是怎么收口的
- 这轮新的 first blocker 是否已经不再是 `PlayerInteraction / TreeController`
- 这轮实际纳入白名单准备归仓的文件有哪些

### 停步自省

- 自评
- 最薄弱点
- 最可能看错的地方
- 为什么下一步只该这样走

### B 技术审计层

1. 是否已真实运行 `preflight`
2. 是否已真实运行 `sync`
3. 如果已 sync，提交 SHA 是什么
4. 如果未 sync，第一真实 blocker 是什么
5. 当前 own 路径是否 clean
6. `touched_touchpoints`

## 聊天最小回卡

聊天里只回：

- 已回写文件路径
- 是否完成本轮要求
- 当前 own 路径是否 clean
- `已上 git / 第一真实阻断已钉死`
