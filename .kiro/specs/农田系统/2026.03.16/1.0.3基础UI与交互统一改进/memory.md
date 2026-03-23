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

## 2026-03-23：背包第一行与 Toolbar 热槽同步修复 + 食物药水生命恢复接线
- 已补完的代码闭环：
  - `InventorySlotUI.cs` 不再强行覆盖 prefab 上的 `Toggle` 过渡配置，恢复尊重原始 `Target / Selected / ToggleGroup` 关系。
  - `InventorySlotUI.cs` 已直接订阅 `HotbarSelectionService.OnSelectedChanged`，使背包第一行与 Toolbar 在同一热槽索引上实时同步，而不是等到第二次打开后才对齐。
  - `InventoryPanelUI.cs` 与 `BoxPanelUI.cs` 刷新背包区域时已同步调用 `RefreshSelection()`，确保第一次打开面板就能看到正确热槽。
  - `ToolbarSlotUI.cs` 已撤销脚本侧对 `Toggle.transition=None` 的强制覆盖，恢复 prefab 原配置口径。
  - `ItemUseConfirmDialog.cs` 已在现有 `EnergySystem` 基础上进一步接入 `HealthSystem`，使食物 / 药水的生命恢复与精力恢复都走真实状态系统，不再只是日志占位。
- 本轮根因修正：
  - 背包“全部红框常亮”以及“第一次打开背包第一行不跟 Toolbar 同步”的核心问题，不在业务逻辑，而在于此前脚本错误地覆盖了 prefab 原有 `Toggle` 配置关系，同时背包第一行又没有像 Toolbar 一样直接订阅热槽选择服务。
- 验证结果：
  - `Assembly-CSharp.rsp` Roslyn 运行时代码独立编译通过。
  - `git diff --check` 针对本轮相关脚本通过，仅剩 CRLF/LF 换行警告。
- 当前恢复点：
  - `1.0.3` 的食物/药水真实状态接线现在已从“只精力恢复”推进到“精力 + 生命恢复”；背包第一行与 Toolbar 的热槽同步也已修正，下一步继续回到剩余未闭环项（尤其是箱内实例态保真与用户现场验收）。

## 2026-03-23：恢复 prefab 原始 Toggle 配置口径，并补完背包第一行与 Toolbar 的实时同源同步
- 用户在现场指出一个关键事实：`InventorySlotUI` / `ToolbarSlotUI` 原本的 prefab 配置是 `Transition = Color Tint`、`Target Graphic = Target`、`Graphic = Selected`，而不是脚本硬改成 `transition=None / targetGraphic=null`。这说明此前为了修 shake 手感而在脚本中强制覆盖 Toggle 配置，本质上是偏离了项目既有配置口径。
- 本轮已执行的修正：
  - 撤销脚本侧对 `InventorySlotUI.cs`、`ToolbarSlotUI.cs` 的强制 Toggle 配置覆盖，重新尊重 prefab 中的 `Target / Selected / ToggleGroup` 关系。
  - `InventorySlotUI.cs` 现已直接订阅 `HotbarSelectionService.OnSelectedChanged`，并新增 `RefreshSelection()`，使背包第一行与 Toolbar 在同一热槽索引上实时同步，不再出现“Toolbar 切换后第一次打开背包未选中、第二次才正确”的延迟现象。
  - `InventoryPanelUI.cs` 与 `BoxPanelUI.cs` 在刷新背包区域时，现会同步调用 `InventorySlotUI.RefreshSelection()`，确保第一次打开面板就拿到正确热槽状态。
- 同轮还继续推进了未完成项：`ItemUseConfirmDialog.cs` 已识别到项目内现有 `HealthSystem.cs`，因此食物 / 药水效果已从“仅精力恢复”推进到“精力 + 生命恢复”都走真实状态系统。
- 当前运行时代码编译状态：`Assembly-CSharp.rsp` Roslyn 独立编译通过。
- 当前恢复点：
  - 背包第一行 / Toolbar 同步问题已按“恢复原配置 + 补实时同步”方向修正；
  - `1.0.3` 剩余未闭合的最大实质缺口已收敛为“箱子主 UI 仍主要走 `ChestInventory` 旧链，箱内实例态保真还未彻底统一到 V2”。
## 2026-03-23：1.0.3 第二轮收口，补齐箱子实例态与农田预览遮挡
- 用户在本轮明确追加了一个来自“遮挡检查”线程的交接任务：农田专属预览遮挡联动应由 farm 线程接手，不改单独的遮挡核心协议，而是在 `FarmToolPreview.cs` 内复用 `OcclusionManager.SetPreviewBounds(Bounds?)`。同时，箱子链不再只是“文档里未完成”，而是需要真实补完 `ChestInventoryV2` 运行时保真。
- 本轮已完成的代码收口：
  - `ChestInventory.cs` 与 `ChestInventoryV2.cs` 的 `Set/Clear/SwapOrMerge/Remove` 路径统一补发 `OnInventoryChanged`，让箱内实例态修改可以稳定触发 UI 刷新。
  - `ChestController.cs` 新增 `RuntimeInventory`，并把 `_inventoryV2.OnInventoryChanged` 接到 `SyncV2ToInventory()`；`BoxPanelUI.cs` 改为优先订阅、刷新、排序这个运行时容器，不再只绑定旧 `_inventory`。
  - `InventorySlotInteraction.cs`、`InventoryInteractionManager.cs`、`SlotDragContext.cs` 进一步补齐 runtime item 保真：同容器交换、箱子/背包互拖、装备回滚、Manager held 放入箱子时，都不再把实例态错误写回成静态 `ItemStack`。
  - `FarmToolPreview.cs` 新增 hover 预览遮挡通知，只把当前 `ghostTilemap + cursorRenderer` 的 Bounds 推给遮挡系统；`Hide()`、`ClearGhostTilemap()` 与 `OnDestroy()` 都会清理 preview bounds，不把 queue/executing 预览并入遮挡。
- 本轮验证结果：
  - `git diff --check` 针对农田白名单通过，仅有 CRLF/LF 提示，无新的空白格式错误。
  - 基于 `D:\1_BBB_Platform\Unity\6000.0.62f1\Editor\Data\NetCoreRuntime\dotnet.exe + D:\1_BBB_Platform\Unity\6000.0.62f1\Editor\Data\DotNetSdkRoslyn\csc.dll` 的 `Assembly-CSharp.rsp` 独立编译再次通过。
- 尚未做 Unity live 场景验收，因此“箱内实例态最终手感”和“锄头/水壶 hover 遮挡视觉”仍待用户在场景里确认。
- 当前恢复点：
  - `1.0.3` 的剩余缺口已经从“箱子主 UI 仍走旧链”推进到“代码链已补齐，等待用户在 Unity 现场验收箱内实例态保真与农田预览遮挡表现”。

## 2026-03-23：旧截图进度已被 main 超前覆盖，当前唯一剩余项是 live 验收入口
- 用户本轮给出的是更早一版截图，其中“下一刀该做箱子实例态”已经不再是 live 事实。回读 `main` 上的 farm 提交链后已确认：`0e87c430` 已把背包第一行 / Toolbar 同步与食物药水真实状态接线并入，`2218b47d` 已把箱子实例态保真与农田 hover 预览遮挡并入。
- 当前 live Git 现场为 `D:\Unity\Unity_learning\Sunset @ main @ 4f76b1b87efb455dc0cc370988ca8b69afc601a3`。针对 `1.0.3` 白名单路径执行 `git status --short --branch -- <paths>` 结果为空，说明当前 farm 这批路径没有未提交草稿，截图中的“未完成”并不是现在的仓库状态。
- 本轮再次使用 Unity 6000.0.62f1 自带 `dotnet.exe + csc.dll` 独立编译 `Library/Bee/artifacts/1900b0aE.dag/Assembly-CSharp.rsp`，结果通过，进一步确认当前 `main` 上 `1.0.3` 代码不是停在半编译状态。
- 当前真正没有闭上的只剩 Unity live 验收入口：本会话 `list_mcp_resources` / `list_mcp_resource_templates` 为空，而 `C:\Users\aTo\.codex\config.toml` 当时仍落在旧端口口径（已失效）；与此同时，`127.0.0.1:8888` 监听正常且 `Library\MCPForUnity\RunState\mcp_http_8888.pid` 存在。这说明 live 验收阻塞更像 MCP 会话 / 配置未回正，而不是 `1.0.3` 代码还没做完。
- 当前恢复点：
  - `1.0.3` 的代码闭环已经覆盖旧截图中的剩余项；
  - 后续真正要继续的不是再补箱子或再补遮挡，而是先让 Unity live 验收入口回正，然后在场景里回归箱内实例态与农田 hover 遮挡表现。

## 2026-03-23：shell 版 unityMCP live 验收已恢复，当前只剩手动交互闭环
- 用户明确告知“mcp 可以了，就是 8888”，并要求我先完成验收，再继续未完成内容。本轮改按 shell 版 `unityMCP@8888` 直接握手，而不再依赖当前会话里失败的 RMCP 封装。已确认：
  - `mcpforunity://instances` 返回唯一实例 `Sunset@21935cd3ad733705`
  - `mcpforunity://editor/state` 返回活动场景 `Assets/000_Scenes/Primary.unity`
  - `mcpforunity://project/info` 与 `mcpforunity://custom-tools` 均可正常读取
- live 验收先按最小范围做项目级核查：
  - `read_console` 先读到的 `OcclusionTransparency` 注册失败与 `Unknown script missing` 是旧 Console 条目；
  - 清空 Console 后，再执行 `Play -> 等待 -> read_console -> Stop`，结果当前窗口内 `0 error / 0 warning`；
  - 同时确认场景里存在激活的 `Primary/1_Managers/OcclusionManager`，所以农田 hover 遮挡当前不再被“场景里没有 Manager”这一假前提阻断。
- 测试层补充：
  - 尝试通过 `run_tests(EditMode, test_names=[OcclusionSystemTests])` 做遮挡核心回归，但 Unity 插件会话在等待 `command_result` 时断开；这被记录为 MCP/plugin 级不稳定，不写成项目失败。
  - `refresh_unity` 随后成功恢复连接，并再次确认 Editor 已回到 Edit Mode、Console 为空。
- 在 live 验收恢复后，本轮继续补完 `1.0.3` 剩余的代码项“术语与文案统一”：
  - `EquipmentData.cs` 将 `Vitality` 的用户显示口径从“体力”统一为“精力”
  - `FoodData.cs` / `PotionData.cs` 将 Tooltip 中的 `HP` 统一为“生命”
  - `ItemUseConfirmDialog.cs` 的结果日志也同步改为“生命 / 精力”口径
- 验证结果：
  - `Assembly-CSharp.rsp` 再次独立编译通过
  - `git diff --check` 针对本轮脚本通过，仅剩 CRLF/LF 提示
  - `refresh_unity` 后 `read_console` 当前为 `0 error / 0 warning`
- 当前恢复点：
  - `1.0.3` 现在已经没有继续扩写的新代码缺口；
  - 当前唯一剩余项只剩“无法由现有 MCP 工具完全替代的手动交互验收”，也就是箱内实例态拖拽手感与农田 hover 遮挡表现的真实操作确认。

## 2026-03-23：`1.0.3` 已按 `全局交互V3` 语义完成收尾并正式转交 `1.0.4`
- 本轮按 `001最后通牒.md` 的要求，没有继续写业务代码，而是先对 `1.0.3` 做了只读收尾判断。回读范围覆盖：`1.0.2纠正001` 四件套、`1.0.3/tasks.md`、`最终交互矩阵.md`、线程记忆、父/根工作区记忆，以及当前 live 代码链 `PlacementManager.cs / PlacementNavigator.cs / PlacementPreview.cs / FarmToolPreview.cs / GameInputManager.cs / ChestInventory.cs / ChestInventoryV2.cs / ChestController.cs / BoxPanelUI.cs / InventorySlotUI.cs / ToolbarSlotUI.cs / InventoryInteractionManager.cs / InventorySlotInteraction.cs / SlotDragContext.cs / ItemTooltip.cs / ItemTooltipTextBuilder.cs / InventoryItem.cs / ToolData.cs / EnergySystem.cs / HealthSystem.cs`。
- 当前 live 现场确认：`D:\Unity\Unity_learning\Sunset @ main @ 2a304c6f80199f0e34c65ac9ce71a8dd61015bcb`。本轮认领的结论是，`1.0.3` 已经完成了基础 Tooltip 入口、实例态跟随、掉落链、食物药水真实状态接线、背包第一行与 Toolbar 同源同步、箱子运行时容器接线、农田 hover 遮挡和 Toolbar 输入边界收口；剩余问题已不再是“再补几个尾项”，而是必须升级为 `1.0.4` 的全局交互治理主题。
- 因此，`1.0.3` 的最终边界已明确写死为：它负责基础 UI 与交互统一改进的第一轮代码闭环，不再继续承接放置成功事务、导航与 reach envelope 统一、受保护槽位最终语义、工具运行时参数链统一、箱子单真源与桥接策略这类系统性边界问题。
- 本轮没有新增 `1.0.3` 业务实现；只完成了收尾判断并把“未完成但必须升级转入 `1.0.4`”的五大主题正式移交：
  1. 放置系统与“幽灵占位”
  2. 箱子放置时导航 retry 3 次停下
  3. 背包关闭后 Toolbar 双选中，以及不该触发的抖动锁定
  4. 工具耐久 / 精力 / Tooltip / SO 参数链
  5. 箱子 `StackOverflowException` 与双库存同步递归
- 当前恢复点：
  - `1.0.3` 收尾已完成；
  - 后续交互主线从这一刻起正式由 `1.0.4交互全面检查` 接管。
