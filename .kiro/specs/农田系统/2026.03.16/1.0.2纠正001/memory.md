# 1.0.2纠正001 - 开发记忆

## 模块概述

本子工作区承接用户在 `1.0.0` / `1.0.1` 验收后追加的那一组深层交互纠偏，核心关注：

- 活跃手持槽位保护
- UI 打开时的冻结与恢复
- `WASD` / 右键导航 / 高频点击的统一中断语义
- 农田预览残留与执行预览回收
- 种植后幽灵透明作物的显示链防守

## 当前状态

- **完成度**: 85%
- **最后更新**: 2026-03-21
- **状态**: 代码已在 `main` working tree，待白名单收口与用户场景验收
- **当前焦点**: 把 `1.0.2` 正文与记忆补回 `main`，再执行本线 Git 收口

## 会话记录

### 2026-03-21：main-only 现场重接管并把 1.0.2 正文补回 main

**用户需求**:
> 当前已经切到 `main-only`，要求重新拾起此前所有关于农田交互升级的讨论与实现，把 `1.0.2` 的真实落地情况彻底对齐到 `main`，并开始守底线的 main-only 自治。

**当前主线目标**:
- 在 `D:\Unity\Unity_learning\Sunset @ main` 上收拢 `1.0.2纠正001` 的代码、文档与记忆，让用户后续直接在 `main` 做 Unity 场景验收。

**本轮子任务 / 阻塞**:
- 子任务是把当前 farm dirty 与历史 cleanroom 口径重新对齐，并把 `1.0.2` 正文和记忆正式补进 `main`。
- 共享阻断有两项，但都不属于 farm runtime 专属缺口：
  - `Assembly-CSharp-Editor` 仍卡在 `Assets/Editor/NPCPrefabGeneratorTool.cs -> NPCAutoRoamController`
  - MCP 当前返回 HTML 网关页，不能拿来做 Unity live 场景验收

**已完成事项**:
1. 重新核对 live 现场为 `D:\Unity\Unity_learning\Sunset @ main @ 8ac0fb5d0db0714f9879ed12885aefc056a03624`。
2. 重新核对本轮 farm dirty 代码范围，共 11 个脚本，均属于 `1.0.2` 的农田交互升级白名单。
3. 重新对照 `codex/farm-1.0.2-cleanroom001` 的 `requirements.md / analysis.md / design.md / tasks.md`，确认当前 `main` working tree 已承接大部分有效实现，且还有若干额外补位：
   - `CropController.cs`
   - `FarmToolPreview.cs`
   - `InventoryInteractionManager.cs`
   - `ToolbarSlotUI.cs`
4. 运行 runtime 独立编译，确认 `Assembly-CSharp.rsp = 0 error / 0 warning`。
5. 运行 Editor 独立编译，确认当前红编译来自共享 NPC Editor 文件，而不是 farm 代码。
6. 重新尝试 MCP 读场景 / Console，确认当前失败是网关 HTML，不是 Unity 项目报错。
7. 正式在 `main` 新建并补齐：
   - `requirements.md`
   - `analysis.md`
   - `design.md`
   - `tasks.md`
   - 本 `memory.md`

**关键决策**:
- 不再把 cleanroom 文档机械搬回 `main`，所有正文都改写为当前 `main` live 现场口径。
- 当前 `1.0.2` 的正确落点已经从“分支 continuation”切换为“`main` working tree + 白名单提交”。
- 当前不把共享 NPC Editor 红编译和 MCP 网关异常伪装成 farm 自身未闭环。

**涉及文件或路径**:
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\CropController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmToolPreview.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Inventory\HotbarSelectionService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementNavigator.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementPreview.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryInteractionManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotInteraction.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Toolbar\ToolbarSlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\requirements.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\analysis.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\design.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\memory.md`

**验证结果**:
- Runtime 编译：通过，`0 error / 0 warning`
- Editor 编译：失败，但失败点为共享 NPC Editor 缺口
- MCP：失败，原因为网关 HTML 返回，不可作为 Unity live 验收依据

**恢复点 / 下一步**:
- 当前已经回到主线的“做本线白名单 Git 收口，然后交给用户在 `main` 做真实场景验收”这一步。

## 2026-03-22：1.0.2 收尾抖动修复 + 下一阶段 UI / 交互盘点

**用户目标**:
- 先修复背包槽位拒绝抖动“发黏、卡顿、不如 Toolbar 灵动”的收尾问题。
- 然后纯只读盘点三类历史内容：种子保质期 UI、耐久度显示、精力/体力扣减与 UI 代办，为下一阶段“全面交互改进”做准备。

**当前主线目标**:
- 继续服务农田交互主线的收尾与下一阶段衔接，不新建工作区；先把现存交互细节修顺，再把后续 UI / 交互改造入口说清楚。

**本轮子任务 / 阻塞**:
- 子任务 1：定位背包 reject shake 与 Toolbar 观感不一致的真实原因并做最小修复。
- 子任务 2：只读核对保质期 / 耐久度 / 精力的代码现状与历史代办，区分“已经落地”“历史口径已过时”“下一阶段真实缺口”。
- 阻塞不在 farm 代码本身，而在于当前项目存在大量 unrelated dirty，所以这轮 Git 收尾必须严格白名单。

**已完成事项**:
1. 回读 `InventorySlotUI.cs`、`ToolbarSlotUI.cs`、`InventorySlotInteraction.cs`、`InventoryInteractionManager.cs` 与背包槽位 prefab，确认 reject shake 协程本身并无差异。
2. 锁定根因：背包槽位 `Toggle` 仍保留默认视觉过渡，而 Toolbar 在 `Awake()` 中已关闭 `targetGraphic/transition`；两者叠加后，背包 shake 会被 Toggle 过渡拖慢。
3. 在 `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs` 的 `Awake()` 中补齐：
   - `toggle.targetGraphic = null;`
   - `toggle.transition = Selectable.Transition.None;`
4. 同文件顺手把一条过期注释改成写实口径，明确这里只关闭 Toggle 自带视觉过渡，避免后续误读。
5. 使用项目现成 `Library/Bee/artifacts/1900b0aE.dag/Assembly-CSharp.rsp`，配合 `C:\Program Files\dotnet\sdk\9.0.301\Roslyn\bincore\csc.dll` 做两次最小编译验证；修改前后结果均为 `0 error / 0 warning`。
6. 完成第二段只读核查，确认：
   - 种子袋保质期运行时逻辑已存在：`SeedData` + `SeedBagHelper` + `InventoryService.OnDayChanged` + `PlacementManager` 的种植消耗链路。
   - 当前缺的不是保质期底层逻辑，而是 UI 出口。
   - `ItemTooltip` 目前只显示 `itemData.description`，根本没有消费 `ItemData.GetTooltipText()`，导致很多已经写好的保质期 / 耐久度 / 精力文案实际上没有统一展示入口。
   - 耐久度条本身已在 `InventorySlotUI` / `ToolbarSlotUI` 落地，但掉落链 `ItemDropHelper` 仍有明确 TODO：不支持 `InventoryItem` 实例数据，掉落后不保留耐久度。
   - 精力系统当前已真实存在于 runtime：`EnergySystem`、`PlayerInteraction`、`TreeController`、`StoneController`、`ToolData.energyCost`、`FoodData.energyRestore`、`PotionData.energyRestore` 都在工作；旧文档里“精力系统不存在/待确认实现”的口径已经过时。

**关键决策**:
- 这轮不新建新工作区，仍按现有农田系统主线推进；当前只把背包 shake 作为 `1.0.2` 收尾修复处理。
- 下一阶段“全面交互改进”的重点不该再写成“从零做保质期 / 精力 / 耐久”，而应该写成“把已有底层能力统一接到可见 UI 与真实交互出口上”。
- `GetTooltipText()` 现成能力没有被 `ItemTooltip` 使用，是后续 UI 整合的关键断点；保质期、工具精力/耐久、食物精力恢复说明都应围绕这个出口整口径。

**涉及文件 / 路径**:
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Toolbar\ToolbarSlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltip.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Items\SeedData.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\SeedBagHelper.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Inventory\InventoryService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\InventoryItem.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Utility\ItemDropHelper.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\EnergySystem.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerInteraction.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\SprintStateManager.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.02.28\10.0.1农作物设计与完善\requirements.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.02.28\10.0.1农作物设计与完善\design.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.02.28\10.0.1农作物设计与完善\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\kiro\精力系统\TD_000_精力系统.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.01.27\2_完善工作\memory.md`

**验证结果**:
- 代码级根因验证：通过。
- 最小编译验证：通过，`Assembly-CSharp.rsp` 两次编译均为 `0 error / 0 warning`。
- Unity live 手感验收：尚未做，待用户在 `main` 现场确认背包 shake 已与 Toolbar 观感对齐。

**恢复点 / 下一步**:
- 当前已经回到主线的“让用户先验收这次背包 shake 收尾修复；若通过，再进入下一阶段 UI / 交互统一改造设计与实现”这一步。

## 2026-03-22：1.0.2 收尾 Git 预检与 main-only 白名单收口口径确认

**用户目标**:
- 在不新建工作区的前提下，先把这轮背包 reject shake 收尾修复真正做到可收口，再把下一阶段 UI / 交互升级的入口、文件范围和真实缺口彻底讲清楚。

**当前主线目标**:
- 继续服务农田交互主线的收尾与下一阶段衔接；当前阶段仍是 `1.0.2` 收尾修复 + 只读盘点，不切换到新工作区。

**本轮子任务 / 阻塞**:
- 子任务 1：确认这轮 main-only 现场能否用显式白名单安全收口，不把 NPC / spring-day1 / 治理线 dirty 混进来。
- 子任务 2：把“已修完的背包 shake”和“下一阶段 UI / 交互升级真实入口”沉淀为可继续执行的稳定结论。
- 共享阻塞不在 farm 代码本身，而在当前仓库存在大量 unrelated dirty，所以本轮 Git 只能走严格白名单。

**已完成事项**:
1. 回读 `scripts/git-safe-sync.ps1`，确认当前 shared root 的旧 task 分支闸门仍存在，但 `-Mode governance + -IncludePaths` 可以在 `main` 上做显式白名单 preflight / sync。
2. 运行只读 preflight，确认当前这轮允许纳入同步的 farm 文件只有两个：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotUI.cs`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\memory.md`
3. 复核第二阶段分析涉及的真实断点，补强结论：
   - `ItemTooltip` 仍只显示 `itemData.description`，没有消费 `ItemData.GetTooltipText()`。
   - 种子袋保质期、耐久度、精力恢复与工具精力消耗都已有底层实现，不是从零开发。
   - 下一阶段真正缺的是统一 UI 出口、实例数据链路和术语口径，而不是底层 runtime 能力。

**关键决策**:
- 本轮 Git 收尾按 `main-only + governance 白名单 sync` 处理，不再回退到旧的 `codex/*` 分支口径。
- `ItemTooltip` 不走 `GetTooltipText()` 被正式确认为下一阶段 UI 升级的第一关键断点。
- `ItemDropHelper` 的实例数据 TODO 被正式确认为耐久度链路的关键技术债，后续不能再只看 UI 条有没有显示。

**涉及文件 / 路径**:
- `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltip.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Items\ItemData.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Items\SeedData.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\SeedBagHelper.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Inventory\InventoryService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\InventoryItem.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Utility\ItemDropHelper.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\EnergySystem.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerInteraction.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\TreeController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\StoneController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\SprintStateManager.cs`

**验证结果**:
- `git-safe-sync.ps1 -Action preflight -Mode governance`：通过，且未误吃 unrelated dirty。
- 本轮未新增 Unity / MCP live 写入。
- 代码侧最小编译闭环仍以 `Assembly-CSharp.rsp = 0 error / 0 warning` 为准。

**恢复点 / 下一步**:
- 当前已经回到主线的“执行白名单 sync 收掉本轮背包 shake 修复，并把下一阶段 UI / 交互改造计划交给用户审阅”这一步。
## 2026-03-22：背包 reject shake 手感收尾完成，实例态 Tooltip 缺口被正式坐实
**用户目标**：
- 先修复背包槽位 reject shake 相比 Toolbar 更“发黏、卡顿、不灵动”的收尾问题。
- 再把种子保质期 UI、工具耐久、精力/体力显示的真实现状和下一阶段改造入口彻底讲清楚，但这一段只做只读分析。

**当前主线目标**：
- 继续服务农田交互主线的收尾与下一阶段衔接；本轮不是新开子工作区，而是在现有 `1.0.2纠正001` 语义下完成一个小修复，并把下一阶段 UI/交互统一改造的真实断点讲透。

**本轮子任务 / 阻塞**：
- 子任务 1：确认背包 shake 不如 Toolbar 的根因并做最小代码修复。
- 子任务 2：确认“保质期 / 耐久 / 精力”到底缺的是底层逻辑、静态 tooltip 出口，还是运行时实例数据出口。
- 当前阻塞不在 farm 业务逻辑，而在于项目里存在大量 unrelated dirty，因此 Git 收尾必须坚持显式白名单。

**已完成事项**：
1. 回读 `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs` 与 `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`，确认两边 `RejectShakeCoroutine()` 基本一致，差异不在 shake 算法本身。
2. 在 `InventorySlotUI.Awake()` 中补齐：
   - `toggle.targetGraphic = null;`
   - `toggle.transition = Selectable.Transition.None;`
   让背包槽位和 Toolbar 一样关闭 Toggle 自带视觉过渡，避免它与自定义 reject shake 叠加打架。
3. 用 `Library/Bee/artifacts/1900b0aE.dag/Assembly-CSharp.rsp` 配合 Roslyn `csc.dll` 做最小编译验证，结果为 `0 error / 0 warning`。
4. 只读核对 `ItemTooltip.cs`、`ItemData.cs`、`SeedData.cs`、`SeedBagHelper.cs`、`InventoryService.cs`、`InventoryItem.cs`、`ItemDropHelper.cs`、`EnergySystem.cs`、`PlayerInteraction.cs`、`SprintStateManager.cs`，确认下一阶段缺口分成两层：
   - 静态说明出口缺口：`ItemTooltip` 现在只写 `itemData.description`，没有消费 `ItemData.GetTooltipText()`。
   - 实例态数据出口缺口：即便改成 `GetTooltipText()`，`ItemTooltip.Show()` 目前仍只拿到 `ItemStack` 和 `ItemData`，吃不到 `InventoryItem` 的动态属性，因此种子袋开袋状态、剩余种子、保质期剩余天数这类运行时信息仍然无处展示。
5. 只读确认旧口径里“精力系统不存在/待实现”的说法已经过时；当前 runtime 里真实存在 `EnergySystem`，工具消耗与食物/药水恢复也都已经接上。

**关键决策**：
- 这轮背包 shake 不重写协程，只做 Toggle 过渡对齐，因为真正的问题是 UI 组件默认视觉反馈干扰了自定义 shake。
- 下一阶段不能再把“保质期 UI”理解成简单补几行文案；它至少是“统一 Tooltip 静态出口”与“给 Tooltip 打开实例态入口”两步。
- 工具耐久条已经在背包/Toolbar UI 中存在，但掉落链 `ItemDropHelper` 仍会丢 `InventoryItem` 实例数据，这一条必须算进下一阶段的真实改造范围。

**涉及文件 / 路径**：
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Toolbar\ToolbarSlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltip.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Items\ItemData.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Items\SeedData.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\SeedBagHelper.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Inventory\InventoryService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\InventoryItem.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Utility\ItemDropHelper.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\EnergySystem.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerInteraction.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\SprintStateManager.cs`

**验证结果**：
- 已验证：背包 shake 与 Toolbar 的真实差异点、最小代码修复、Roslyn 编译通过。
- 已验证：种子保质期逻辑、工具耐久逻辑、精力消耗/恢复逻辑在 runtime 侧都已存在。
- 已验证：`ItemTooltip` 当前没有消费 `GetTooltipText()`，也没有实例态入口。
- 未验证：Unity live 手感是否已与 Toolbar 完全一致，仍待用户在 `main` 场景内实手验收。

**恢复点 / 下一步**：
- 当前已经回到主线的“用白名单提交收掉这次 shake 收尾修复，然后把下一阶段 UI/交互统一改造方案交给用户确认”这一步。

## 2026-03-23：Toolbar 输入边界口径纠偏并固化为 live 规则
**用户目标**：
- 用户明确纠正 Toolbar 规则：数字键只允许 `1~5` 直选前五格，但滚轮在 `1~12` 间循环本身是正确设计，不应再被误判成违规。
- 要求我不要继续沿着误解推进，而是直接按这个 live 口径落地。

**当前主线目标**：
- 主线仍是农田交互边界收口；本轮子任务是把 Toolbar/背包热槽输入边界重新钉死，避免后续实现和文档继续被旧口径带偏。

**本轮子任务 / 阻塞**：
- 子任务 1：复核 live 代码里当前真正生效的 Toolbar 切换入口。
- 子任务 2：把“数字键 1~5 直选、滚轮 1~12 循环”的边界同时固化到代码与规则文档。
- 当前没有新的业务阻塞，主要风险是旧文档残留继续误导后续开发。

**已完成事项**：
1. 回读 `GameInputManager.cs`、`HotbarSelectionService.cs`、`ToolbarSlotUI.cs`、`InventoryPanelUI.cs` 与 `PlayerInteraction.cs`，确认当前 live 切换入口只有：
   - `GameInputManager.HandleHotbarSelection()` 中的数字键 `1~5`
   - 同一方法中的滚轮循环
   - `ToolbarSlotUI.OnPointerClick(...)` 的 UI 点击
2. 在 `InventoryService.cs` 中新增 `HotbarDirectSelectCount = 5`，把“数字键直选前五格”固化成显式常量。
3. 在 `GameInputManager.cs` 中把数字键切换逻辑改为明确依赖 `HotbarDirectSelectCount`，并补注释区分：
   - 数字键只负责前五格直选
   - 滚轮继续在 12 格内循环
4. 修正当前 live 文档口径：
   - `1.0.2纠正001/requirements.md`
   - `1.0.2纠正001/tasks.md`
   - `最终交互矩阵.md`
   - `.kiro/steering/ui.md`
   - `.kiro/steering/items.md`
   - `.kiro/steering/maintenance-guidelines.md`
   统一去掉“快捷键泛化”“1-8 / 1-9 数字键”之类旧表述。

**关键决策**：
- `HotbarWidth = 12` 本轮不再被视为错误，因为它服务的是滚轮循环范围，不是数字键直选范围。
- Toolbar 点击继续被视为 UI 交互入口，不把它重新描述成“键盘快捷键集合”的一部分。
- `PlayerInteraction.enableLegacyInput` 仍只是潜在调试残留，不将其误写成当前 live 工具切换入口。

**涉及文件 / 路径**：
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Inventory\InventoryService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\requirements.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\最终交互矩阵.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\steering\ui.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\steering\items.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\steering\maintenance-guidelines.md`

**验证结果**：
- `Assembly-CSharp.rsp` 独立编译通过。
- `git diff --check` 针对白名单路径通过，仅剩 CRLF/LF 换行提示。
- 当前 live 代码未发现新的额外 Toolbar 快捷切换入口。

**恢复点 / 下一步**：
- 当前已经回到主线的“继续做用户真实场景验收与剩余交互收尾，而不是再纠缠 Toolbar 输入边界到底是不是 12 格”这一步。
