# 1.0.3 基础 UI 与交互统一改进 - 工作区记忆

## 2026-03-22：建立 1.0.3 工作区，正式承接基础 UI 与交互统一改进

**用户目标**：
- 用户认可当前“先收尾、再开启下一阶段 UI / 交互统一”的方向，并建议在 `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16` 下正式新开一个 `1.0.3` 工作区，把这轮高质量分析完整落盘，后续以这里为准继续推进。

**当前主线目标**：
- 不再把新需求继续塞回 `1.0.2纠正001`。
- 在 `1.0.3` 下明确下一阶段“Tooltip / 实例态 / 掉落链 / 食物药水 / 术语统一”的真实边界、文件落点与任务顺序。

**本轮子任务 / 阻塞**：
- 子任务是文档建档与方向固化，不是继续写新功能。
- 当前阻塞不在业务理解，而在于必须把 live 现场事实写成可延续文档，避免后续 снова 回到口头共识。

**已完成事项**：
1. 复核当前 live 现场为 `D:\Unity\Unity_learning\Sunset @ main @ c6af26574234329e3525acbdfd5b645a3f5b278a`。
2. 复核背包 reject shake 卡顿的真实根因，并已在 `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs` 做最小修复：
   - `toggle.targetGraphic = null;`
   - `toggle.transition = Selectable.Transition.None;`
3. 复核 `ItemTooltip.cs` 当前只显示 `itemData.description`，没有消费 `GetTooltipText()`。
4. 复核 `ItemTooltip` 当前几乎没有真实交互调用入口，现状更接近“有脚本、无接线”。
5. 复核实例态数据链已经存在但 UI 拿不到：
   - `InventoryItem` 耐久
   - 种子袋保质期与实例状态
6. 复核 `ItemDropHelper.cs` 仍不支持实例态掉落。
7. 复核 `ItemUseConfirmDialog.cs` 仍停留在 `Debug.Log`，食物 / 药水没有真实改状态。
8. 新建 `requirements.md / analysis.md / design.md / tasks.md / memory.md`，把上述结论正式固化为 `1.0.3` 工作区正文。

**关键决策**：
- `1.0.3` 的新阶段定位成立，原因是当前问题已经从“农田交互正确性”切换到“基础 UI 与物品交互统一改进”。
- 当前不需要新建新的代码文件夹；后续若 Tooltip 文本拼装过重，最多新增一个 `ItemTooltipTextBuilder.cs` helper。
- 后续实现顺序固定为：
  1. Tooltip 入口
  2. Tooltip 静态出口统一
  3. Tooltip 实例态入口
  4. 实例态掉落链
  5. 食物 / 药水真实生效
  6. 术语统一

**涉及文件 / 路径**：
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltip.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotInteraction.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Toolbar\ToolbarSlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Utility\ItemDropHelper.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemUseConfirmDialog.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.3基础UI与交互统一改进\requirements.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.3基础UI与交互统一改进\analysis.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.3基础UI与交互统一改进\design.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.3基础UI与交互统一改进\tasks.md`

**验证结果**：
- 已验证：背包 shake 与 Toolbar 手感差异的真实根因，且最小修复已落地。
- 已验证：`Assembly-CSharp.rsp` Roslyn 最小编译通过（`0 error / 0 warning`）。
- 已验证：种子袋保质期、耐久、精力 runtime 链存在。
- 已验证：Tooltip 当前未接线、未消费 `GetTooltipText()`、无实例态入口。
- 未验证：Unity live 手感与后续 1.0.3 功能体验，仍待后续实现和用户验收。

**恢复点 / 下一步**：
- 当前已经回到主线的“1.0.3 文档正式建档完成，等待后续按该工作区进入真实实现”这一步。

## 2026-03-23：1.0.3 第一轮实现已落地，当前停在共享编译阻断前
- 已完成代码落地：
  - `ItemTooltip.cs` 不再直接显示 `itemData.description`，开始统一走 `GetTooltipText()` 的静态出口，并叠加实例态信息。
  - 新增 `ItemTooltipTextBuilder.cs`，集中拼装静态文本、工具当前耐久、种子袋开袋状态、剩余种子数与剩余保质期。
  - `InventorySlotInteraction.cs` 与 `ToolbarSlotUI.cs` 已接入 Tooltip 悬浮显示/隐藏入口；装备槽通过 `InventorySlotInteraction` 复用该入口。
  - `EquipmentSlotUI.cs` 暴露当前装备槽的 `ItemStack / InventoryItem / Database` 读取口径，供 Tooltip 与后续实例态链使用。
  - `InventoryItem.cs` 新增动态属性可见性口径，`PlayerInventoryData.cs` 与 `ChestInventoryV2.cs` 改为把所有实例态属性都视为不可堆叠数据，避免种子袋/耐久物品被错判成普通静态物品。
  - `InventoryInteractionManager.cs`、`InventorySlotInteraction.cs`、`SlotDragContext.cs` 已补齐主要拖拽/归位/丢弃链上的实例态随行，避免工具耐久、种子袋状态在换格或丢到地上时直接丢失。
  - `ItemDropHelper.cs` 与 `WorldItemPickup.cs` 已支持 `InventoryItem` 实例态掉落与拾取回背包。
  - `BoxPanelUI.cs` 的 SlotDragContext 丢弃入口也已跟上实例态掉落分流。
  - `ItemUseConfirmDialog.cs` 已把食物/药水的精力恢复接到 `EnergySystem`；生命恢复因当前项目缺少明确的玩家生命系统，仍写实保留 warning，不伪称已完成。
- 已完成验证：
  - 中途 Roslyn 运行时代码独立编译曾通过。
  - 当前再次跑全量 `Assembly-CSharp.rsp` 时，新的阻断来自共享现场：`Assets/YYY_Scripts/Story/Managers/StoryManager.cs(127,13): SpringDay1Director` 未定义，不属于农田这批脚本。
  - `git diff --check` 针对本轮农田白名单脚本已无空白格式错误，只剩 CRLF/LF 提示。
- 当前未完成 / 残留：
  - `1.0.3` 的任务文档 checkbox 还未回填到实现后状态。
  - 父工作区、根工作区、线程记忆与 skill 审计还未同步这轮真实实现结果。
  - 生命恢复仍未真正落地，因为项目内暂未发现可接的玩家生命系统。
  - 当前箱子主 UI 仍走 `ChestInventory` 旧链，因此“箱子内实例态完整保真”不应冒充已彻底解决。
- 当前恢复点：
  - 农田 `1.0.3` 已从“纯设计”推进到“Tooltip + 实例态 + 掉落 + 食物药水精力恢复”的第一轮实现，下一步先同步文档/记忆，再按白名单提交 checkpoint，然后交给用户按清单验收。

## 2026-03-23：修复背包“全部选中”回归
- 根因：`InventorySlotUI.cs` 在此前为了修复 reject shake 手感，把 `Toggle` 的默认视觉过渡关闭后，背包槽位自己的 `selectedOverlay` 仍未像 `ToolbarSlotUI.cs` 那样被显式同步管理；结果原本隐性的选中覆盖层状态被直接暴露出来，表现为“背包内容全部选中”。
- 修复：
  - 在 `InventorySlotUI.cs` 中补齐与 `ToolbarSlotUI.cs` 同口径的选中视觉管理。
  - `Awake()` 中显式将 `Toggle` 初始化为未选中。
  - 监听 `toggle.onValueChanged`，统一驱动 `selectedOverlay.enabled`。
  - `Bind()` / `BindContainer()` 以及 `Select()` / `Deselect()` 都改为同步更新选中覆盖层。
- 验证：`Assembly-CSharp.rsp` Roslyn 运行时代码独立编译通过，`git diff --check` 针对本次最小修复通过。
- 当前恢复点：已把背包“全选”回归单独收敛成一个最小修复点，下一步可直接由用户在背包界面手动确认选中视觉是否恢复正常。
