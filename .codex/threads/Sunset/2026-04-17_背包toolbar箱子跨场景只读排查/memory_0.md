# 2026-04-17 背包 / toolbar / 箱子跨场景只读排查

- 用户目标：只读排查 `InventoryService`、`HotbarSelectionService`、`ToolbarUI/ToolbarSlotUI`、`InventoryPanelUI`、`BoxPanelUI`、`GameInputManager`、`PersistentPlayerSceneBridge`、`AutoPickup/Equipment/Placement` 重绑链，解释为什么背包 / toolbar / 箱子界面在重开、剧情推进、Home/Primary/Town 切换后会出现显示错位、选中态混乱、工具不可用、跨场景后像瘫痪。
- 当前主线：输出中文问题清单，按 `已证实 / 高概率` 区分，并给出一句人话解释与关键证据方法；不提供修复方案，不改代码。
- 本轮子任务：静态阅读跨场景重绑、输入门控、选中态同步、面板开关与放置/工具保护链。
- 服务于什么：为后续真正修复前先明确“到底是状态丢失、UI 订阅丢失，还是输入门控卡住”。
- 修复后恢复点：如果后续要施工，应优先回到 `HotbarSelectionService <-> PersistentPlayerSceneBridge <-> GameInputManager` 的跨场景状态一致性，再看 `ToolbarUI` 的订阅恢复和面板视觉选中链。

## 本轮已完成

- 读完并交叉比对：
  - `Assets/YYY_Scripts/Service/Inventory/InventoryService.cs`
  - `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
  - `Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs`
  - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
  - `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
  - `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`
  - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
  - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
  - `Assets/YYY_Scripts/Service/Player/AutoPickupService.cs`
  - `Assets/YYY_Scripts/Service/Equipment/EquipmentService.cs`
  - `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
  - `Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
  - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
- 已证实的主因：
  - `PersistentPlayerSceneBridge` 只快照并恢复 `selectedIndex`，没有保留 `HotbarSelectionService.selectedInventoryIndex`，而 `RebindHotbarSelection` 与 `GameInputManager.ResetPlacementRuntimeState` 又会把 `inventoryIndex` 强行压回 hotbar 索引。
  - `GameInputManager` 的剧情开始只关闭箱子，不关闭背包主面板；但输入门控把“任何面板打开”都视为禁止移动 / 切栏 / 用工具。
- 高概率问题：
  - `ToolbarUI` 在 `OnDisable` 只解绑，不清空 `subscribedInventory/subscribedSelection`，下次同引用重新激活时会因为 `Sync*Subscription` 早退而不重新订阅。
  - `InventoryPanelUI` 与 `BoxPanelUI` 在打开 / 激活时都会把选中态重置为跟随 hotbar，而不是恢复之前的面板内选槽。

## 关键判断

- 最核心判断：跨场景后的“瘫痪感”不是单点故障，而是三层叠加：
  - 真实背包选槽状态丢失
  - 面板打开态继续挡输入
  - 部分 UI 存在重激活后不再跟随服务事件的风险
- 我为什么这样判断：这三条都能在代码里直接形成闭环，并且分别对应了用户描述的三类症状：
  - “选中态混乱”
  - “工具不可用”
  - “看起来像没反应 / 瘫痪”

## 验证状态

- 验证方式：静态代码证据链
- 结论状态：
  - `selectedInventoryIndex` 跨场景丢失：已证实
  - 剧情后背包仍开导致输入持续被挡：已证实
  - `ToolbarUI` 重激活后丢事件订阅：高概率
  - 背包 / 箱子面板每次打开都把视觉选中重置到 hotbar：高概率

## 遗留与下一步

- 当前还没做：没有跑 Unity live 复现，也没有给修复方案。
- 如果后续继续：先验证用户现场到底更接近哪一条主因，再决定是修状态快照、输入门控，还是 UI 订阅恢复。

## 2026-04-18 只读续记｜toolbar 固定槽位 4/8 异常更像 ToolbarUI 名字绑定脆弱

- 用户目标：只读排查 Sunset 项目里 toolbar 固定槽位 `4/8`（第 `5/9` 格）在切场后图标/显示异常、但进出 `Home` 又会恢复的问题；不要改代码；要输出最可能根因、为什么偏偏 4/8、为什么 Home 会恢复、以及打包前最安全的最小修复方案。
- 当前主线：把“跨场景 inventory / toolbar / box / bridge 真源问题”进一步收窄到 toolbar 这一条局部坏相，形成可直接交给后续施工的根因判断。
- 本轮子任务：静态补读 `ToolbarUI / ToolbarSlotUI / InventoryPanelUI / BoxPanelUI / PersistentPlayerSceneBridge / HotbarSelectionService`，并核对 `ToolBar.prefab + Home/Primary/Town` 的 authored 子节点顺序。
- 服务于什么：避免后续继续把 4/8 问题误判成 world-state 主链或 inventory panel 缓存问题，先把最小刀口收窄。
- 修复后恢复点：如果后续进入真实施工，优先回到 `ToolbarUI.Build()` 的绑定策略收口，而不是继续扩大到 `InventoryPanelUI / BoxPanelUI / SaveManager`。

## 本轮已完成

- 重新核读：
  - `Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs`
  - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
  - `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
  - `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`
  - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
  - `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
  - `Assets/YYY_Scripts/Service/Inventory/InventoryService.cs`
  - `Assets/YYY_Scripts/UI/Utility/UIItemIconScaler.cs`
- 静态核对：
  - `Assets/222_Prefabs/UI/0_Main/ToolBar.prefab`
  - `Assets/000_Scenes/Home.unity`
  - `Assets/000_Scenes/Primary.unity`
  - `Assets/000_Scenes/Town.unity`
- 已证实：
  - `InventoryPanelUI` 和 `BoxPanelUI` 都是按循环索引直接 `Bind(..., i, ...)`，不是名字驱动。
  - `ToolbarUI.Build()` 是这条链里唯一仍靠 `ResolveToolbarSlotIndex()` 用名字推断槽位索引的地方。
  - `PersistentPlayerSceneBridge.RebindPersistentCoreUi()` 每次切场都会重新执行 `toolbarUi.Build(); toolbarUi.ForceRefresh();`
  - `ToolBar/Grid` 在 `Home / Primary / Town` 三个主场景里的 authored 子节点顺序一致，都是从 `Bar_00_TG` 到 `Bar_00_TG (11)` 的标准 0..11 顺序。

## 新关键判断

- 最核心判断：固定槽位 `4/8` 的问题，当前最高概率根因已经从“整个 inventory/bridge 链都坏了”收窄成“toolbar 自己的重绑策略太脆弱”。
- 我为什么这样判断：
  - `InventoryPanelUI` / `BoxPanelUI` 走的是直接索引绑定，切场时还有显式 `ConfigureRuntimeContext(...) / EnsureBuilt() / Refresh...()`
  - 只有 `ToolbarUI.Build()` 还在做：
    - 子物体过滤
    - 名字解析索引
    - 排序
    - `boundIndices` 去重
  - 这条链一旦在 runtime 命名或时序上有一点漂移，就更容易留下“少数格子显示错，但底层服务真源已经重绑”的坏相。

## 对 4/8 的判断

- 代码里没有把 `4/8` 硬编码成特殊槽位。
- scene / prefab authored 顺序也没发现 `4/8` 是错位或缺件。
- 所以 `4/8` 更像“稳定暴露位”，不是“逻辑特殊位”：
  - toolbar 这条通病存在
  - 但当前玩家最容易在第 `5/9` 格看见它
  - 很可能因为这两个槽位上常驻更容易观察异常的工具/运行态道具

## 为什么进出 Home 会恢复

- `PersistentPlayerSceneBridge` 的 persistent UI 根通常来自 `Home` 这套 authored UI。
- 回到 `Home` 时，toolbar hierarchy 与这套 persistent baseline 最一致；bridge 还会再次执行：
  - `toolbarUi.Build()`
  - `toolbarUi.ForceRefresh()`
  - `sceneInventory.RefreshAll()`
  - `sceneHotbarSelection.ReassertCurrentSelection(...)`
- 所以“进出 Home 会恢复”更像是：
  - 回到了 persistent UI 的原始 authored 基线
  - 把普通切场后留下的 toolbar 局部脏态重新刷正

## 当前最安全的最小修复建议

- 最小修复应优先只收 `ToolbarUI.Build()`：
  - 不再依赖 `ResolveToolbarSlotIndex()` 的名字推断
  - 直接按 `gridParent` 子物体 sibling 顺序 `0..11` 绑定
  - 只保留一个 childCount / 命名异常校验日志
- 我认为这是打包前最安全的原因：
  - 三个主场景和 prefab 的 authored 顺序已经静态证实一致
  - inventory panel / box panel 早就在用“按顺序直接绑”的策略
  - 改 toolbar 去和它们统一，改动面最小、语义最直接、回归风险也最低

## 验证状态

- 验证方式：静态代码证据链 + prefab/scene authored 顺序核对
- 结论状态：
  - `ToolbarUI` 名字绑定脆弱：高概率
  - `4/8` 是 authored 顺序写错：已排除
  - `Home` 作为 persistent UI authored baseline 触发恢复：高概率
  - “最小安全修法应只收 ToolbarUI.Build” ：高概率

## 当前遗留

- 尚未做：
  - Unity live 复现
  - 实机证明异常到底“跟槽位走”还是“跟物品走”
- 如果后续继续，只读阶段之后的第一刀应优先：
  - `ToolbarUI.Build()` 绑定策略收口
  - 而不是再次扩大到 `InventoryPanelUI / BoxPanelUI / world-state` 主链

## 2026-04-18 只读续记｜Primary 丢物 -> Home 正常 -> 回 Primary 背包回弹 / 掉落残留 / 疑似重复 的静态根因链

- 用户目标：只读审查 Sunset 当前背包 / toolbar / runtime rebind 链，解释为什么在 `Primary` 丢物后进 `Home` 一切正常，但再回 `Primary` 时会出现“背包像回弹、地上掉落还在、甚至像刷出重复物品”；不要改代码；要回答最可能根因链、哪些组件可能还拿着旧 `InventoryService / HotbarSelectionService`、最小最安全修法、以及按风险排序的 suspect files / methods。
- 当前主线：把“world-state 掉落恢复”和“inventory / hotbar 双索引 rebind”拆开，判断这更像真数据回滚，还是 UI / 运行态重投影造成的假重复观感。
- 本轮子任务：补读并交叉比对 `PersistentPlayerSceneBridge`、`HotbarSelectionService`、`GameInputManager`、`InventoryPanelUI`、`BoxPanelUI`、`InventoryInteractionManager`、`InventorySlotUI`、`ToolbarUI`，并补 `WorldItemPickup / WorldItemPool / DynamicObjectFactory / ItemDropHelper` 这一小段来确认掉落物恢复合同。
- 服务于什么：在真正施工前先确认应该先收“bridge 选择态快照”还是“旧服务引用 / 旧订阅”，避免把真正的一刀做散。
- 修复后恢复点：如果后续进入真实施工，第一刀应优先回到 `PersistentPlayerSceneBridge + HotbarSelectionService` 的双索引恢复合同；只有这一刀收完仍有症状，才继续追 `InventoryPanelUI / BoxPanelUI` 的选中态投影和旧订阅硬化。

## 本轮已完成

- 重新核读：
  - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
  - `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
  - `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`
  - `Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs`
  - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
  - `Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs`
  - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
  - `Assets/YYY_Scripts/Service/Inventory/InventoryService.cs`
  - `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
- 为了判断“掉落残留 / 疑似重复”是否是真重复，补读：
  - `Assets/YYY_Scripts/World/WorldItemPickup.cs`
  - `Assets/YYY_Scripts/World/WorldItemPool.cs`
  - `Assets/YYY_Scripts/Data/Core/DynamicObjectFactory.cs`
  - `Assets/YYY_Scripts/UI/Utility/ItemDropHelper.cs`
  - `Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
  - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
  - `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`

## 新关键判断

- 当前最高概率主因已经从“旧 `InventoryService` 被广泛拿错”收窄成“`PersistentPlayerSceneBridge` 在切场时把 `HotbarSelectionService` 的双索引状态压扁成了单一 hotbar 索引”：
  - `CaptureSceneRuntimeState()` 只保存 `selectedIndex`，不保存 `selectedInventoryIndex`
  - `RebindHotbarSelection()` 又把 `RestoreSelectionState(hotbarIndex, hotbarIndex)` 写死成同一个值
  - 这样一来，跨场景后本来应该保留的“面板/放置偏好来源槽位”会被强行折叠回快捷栏第一行
- `InventoryPanelUI` 和 `BoxPanelUI` 又都把“面板当前选槽”重新同步为 `selection.selectedIndex`，而不是 `selection.selectedInventoryIndex`：
  - `InventoryPanelUI.SyncSelectionFromHotbar()`
  - `BoxPanelUI.SyncInventorySelectionFromHotbar()`
  - 所以只要面板重新激活、重新打开、或收到 hotbar 变化，就会把视觉选中态重新投影回快捷栏
- `PersistentPlayerSceneBridge` 对掉落物 world-state 的恢复合同本身是成立的：
  - 离场时 `CaptureSceneWorldRuntimeState()` 会把 `WorldItemPickup` 快照进 `sceneWorldSnapshotsByScene`
  - 回场时 `RestoreSceneWorldRuntimeState()` 会按 snapshot 恢复 / 重建 / 反向修剪
  - `ItemDropHelper` 生成玩家丢弃物时还会显式 `SetSourceNodeGuid(null)`，因此它会被当作独立掉落物保存和恢复
- 这就形成了当前最像用户描述的坏相组合：
  - `Primary` 的地上掉落在回场时被“正确恢复”
  - 但背包 / 箱子 / toolbar 的选槽和运行态被“错误折叠回快捷栏”
  - 用户看到的是“地上掉落还在，同时包里/手上看起来又像回来了”，从体感上就是“回弹 / 重复”

## 对“旧 InventoryService / HotbarSelection”风险的判断

- 在你点名的主链里，真正强绑定旧服务风险最高的不是 `ToolbarUI`，而是“有缓存字段但不在切换对象时立即重订阅”的几个组件：
  - `InventoryPanelUI.ConfigureRuntimeContext()` 只改字段并 `EnsureBuilt()`，但组件级 `selection.OnSelectedChanged` 订阅只在 `OnEnable/OnDisable` 管
  - `BoxPanelUI.ConfigureRuntimeContext()` 也会直接换 `_hotbarSelection` / `_inventoryService`，但旧 hotbar 订阅只在 `UnsubscribeFromHotbarSelection()` 里清；如果对象真换了而面板没先关，旧对象订阅有残留风险
  - `InventoryInteractionManager` 持有 `inventory/equipment/database` 缓存，依赖 bridge 或 `BoxPanelUI.SyncInventoryInteractionManagerRuntimeContext()` 主动刷新
- 相邻链路里还有两个次级缓存点：
  - `PackagePanelTabsUI` 持有 `runtimeInventoryService/runtimeHotbarSelection`
  - `InventorySlotInteraction.CachedPackagePanel` 只在缓存为 null 时重新抓一次，不主动失效
- 相反，`ToolbarUI` / `ToolbarSlotUI` 这版已经有显式 `SyncInventorySubscription()/SyncSelectionSubscription()`，而且 `ToolbarUI.Build()` 也已经改成按 sibling 顺序 `0..11` 绑定；它们现在更像“会把上游错误状态老老实实显示出来”，不是第一根真源。

## 验证状态

- 验证方式：静态代码证据链。
- 已证实：
  - bridge 切场时只保存 `selectedIndex`，不保存 `selectedInventoryIndex`
  - bridge 回绑时把 `hotbarIndex` 和 `inventoryIndex` 都恢复成同一值
  - `InventoryPanelUI` / `BoxPanelUI` 同步面板选槽时只读 `selectedIndex`
  - `Primary` 掉落物会被 off-scene world snapshot 正常恢复
- 仍属推断：
  - “用户看到的重复”更像“正确恢复的 world drop + 错误折叠的面板/运行态”叠出的假重复，而不是 inventory 数组真的被回滚两份
  - 若要证明是真数据回滚，还需要 Unity live 复现或日志证据

## 如果后续继续

- 第一刀最小安全修法应优先只收：
  - `PersistentPlayerSceneBridge.CaptureSceneRuntimeState()`
  - `PersistentPlayerSceneBridge.RebindHotbarSelection()`
  - 让它保存并恢复完整的 `selectedIndex + selectedInventoryIndex`
- 如果第一刀后仍有“开包即回弹”的观感，再补第二刀：
  - `InventoryPanelUI.SyncSelectionFromHotbar()`
  - `BoxPanelUI.SyncInventorySelectionFromHotbar()`
  - 让面板同步优先读取 `selectedInventoryIndex`，而不是无条件折叠到 hotbar
- 当前不建议第一刀就去碰：
  - `InventoryService.Save/Load`
  - `ToolbarUI.Build()`
  - `WorldItemPickup / DynamicObjectFactory`
  因为这三条在这次静态证据里都不像主导根因

## 2026-04-18 只读续记｜限定文件范围内剩余“背包选槽被压回 hotbar”读点复核

- 用户目标：只读审查 `Assets/YYY_Scripts/UI/Inventory`、`Assets/YYY_Scripts/UI/Box`、`Assets/YYY_Scripts/UI/Toolbar`、`Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`、`Assets/YYY_Scripts/Service/Player/PlayerInteraction.cs`、`Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`，专门找还会把真实背包选槽压回 hotbar 选槽的剩余读点；不要改代码；只要高风险读点、已安全点、最小修复顺序。
- 当前主线：继续背包 / toolbar / 箱子跨场景与双索引问题的只读收口，把“面板层已修到哪、运行态入口还剩哪”讲清楚。
- 本轮子任务：限定在用户点名的文件范围内，用 `selectedIndex / selectedInventoryIndex / SelectInventoryIndex / SetPanelSelectionIndex` 这组符号重新逐点核读。
- 服务于什么：确认下一刀如果真要动，是否还要回 UI 面板链，还是只该收 `GameInputManager` / `HotbarSelectionService` 的运行态入口。
- 修复后恢复点：如果后续进入真实施工，这条线应先从限定范围里剩余的运行态高风险点下手，而不是回头重改 `InventoryPanelUI / BoxPanelUI / ToolbarUI`。

## 本轮已完成

- 复核并逐段核读：
  - `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
  - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
  - `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
  - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
  - `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`
  - `Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs`
  - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
  - `Assets/YYY_Scripts/Service/Player/PlayerInteraction.cs`

## 新关键判断

- 在用户限定的这些文件里，`InventoryPanelUI` 和 `BoxPanelUI` 这条“面板选槽同步链”目前已经改成读 `selectedInventoryIndex`，不再是本轮的剩余高风险源。
- `ToolbarUI` / `ToolbarSlotUI` / `InventorySlotUI` 当前对 `selectedIndex` 的读取主要用于快捷栏高亮、toggle 恢复和 hover/近期使用可见性，属于 hotbar UI 自身状态显示，当前不构成“把真实背包选槽压回 hotbar”的风险源。
- `PlayerInteraction.cs` 在这组关键词下无命中，当前不在这条问题链内。
- 当前限定范围里仍需要优先盯的读点几乎都集中在 `GameInputManager.cs`：
  - placement/held-item 之外的大多数农业与手持判断入口仍默认从 `hotbarSelection.selectedIndex` 取当前槽；
  - 其中用于真实背包非 hotbar 选槽场景最值得先防守的是“未走 `TryGetCurrentPlacementSelection()` 的 fallback 入口”，因为它们仍把当前运行态来源假定为 hotbar。
- `HotbarSelectionService.cs` 本体当前已经具备 `selectedIndex + selectedInventoryIndex` 双索引和 `GetResolvedPlacementSlotIndex()`；它更像“已经提供了安全读法，但调用方还没完全统一吃进去”，而不是本轮主要残留 bug 点。

## 验证状态

- 验证方式：静态代码核读 + 定点符号搜索。
- 结论状态：
  - `InventoryPanelUI / BoxPanelUI` 面板同步已安全：静态证实
  - `ToolbarUI / ToolbarSlotUI / InventorySlotUI` 的 `selectedIndex` 使用当前主要是 hotbar UI 自身显示：静态证实
  - 剩余风险主要集中在 `GameInputManager` 仍以 `selectedIndex` 作为运行态来源的入口：静态证实
  - 这些入口是否都会在玩家当前坏相里实际触发：仍待 live 复现验证

## 如果后续继续

- 最小修复优先顺序应先看：
  - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
  - `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
- 当前不建议先回头重改：
  - `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
  - `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`
  - `Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs`
  因为在这次限定范围复核里，它们已经不再是剩余主风险点。

## 2026-04-18 只读续记｜restart 后 toolbar 固定槽位 4/8 异常的当前最高概率链

- 用户目标：只读回答一个限定问题，不改代码：
  - 为什么 `重新开始游戏` 后 toolbar 固定槽位 `4/8` 显示异常；
  - 为什么 NPC 对话后或进出 `Home` 又恢复。
- 当前主线：继续背包 / toolbar / restart / scene-rebind 这条只读收口，把“到底是 save data 坏了，还是 restart 顺序和 toolbar 刷新合同有缝”讲清楚。
- 本轮子任务：交叉复核 `SaveManager`、`PersistentPlayerSceneBridge`、`HotbarSelectionService`、`ToolbarUI / ToolbarSlotUI`、`InventoryPanelUI`、`InventoryInteractionManager`、`BoxPanelUI`、`PackagePanelTabsUI` 的 restart / rebind / refresh 触点。
- 服务于什么：避免下一刀继续把 4/8 坏相误判成 `InventoryPanelUI / BoxPanelUI` 或 save data 真损坏。
- 修复后恢复点：如果后续进入真实施工，优先回到 `SaveManager.NativeFreshRestartRoutine()` 与 `ToolbarUI` 的刷新/订阅合同，而不是回头扩大到 save DTO 或 world-state 主链。

### 本轮已完成

- 重新核读：
  - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
  - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
  - `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
  - `Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs`
  - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
  - `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
  - `Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs`
  - `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`
  - `Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
- 新钉实：
  - 当前代码里没有 `4/8` 特判，也没有旧的 `ResolveToolbarSlotIndex()` 名字绑定链。
  - `SaveManager.NativeFreshRestartRoutine()` 会先 `LoadSceneAsync(Town)`，而 sceneLoaded 期间 bridge 已经先执行 `RebindScene()` / `RebindPersistentCoreUi()`。
  - 之后 `ApplyNativeFreshRuntimeDefaults()` 才 `ResetPersistentRuntimeForFreshStart()`，把 persistent inventory / hotbar 清到 fresh 状态。
  - restart 因而天然形成：
    - `旧态先重绑`
    - `新态后清空`
  - `ToolbarUI` 当前仍有一条真实脆弱点：
    - `OnDisable()` 只解绑事件，不清 `subscribedInventory / subscribedSelection`
    - 后续若对象走过 disable/enable 且引用没换，`Sync*Subscription()` 会早退，toolbar 更依赖单次 `Build()/ForceRefresh()`。

### 新关键判断

- 当前最高概率根因已经收敛成：
  - restart 顺序先把旧 runtime snapshot 打回 toolbar
  - 再做 fresh reset
  - toolbar 自己的刷新/重订阅合同又不够硬
  - 于是少数槽位会残留旧显示
- 为什么 NPC 对话或进出 `Home` 会恢复：
  - `Home` 往返会再走一次 bridge 的全量 scene rebind，是更强的 UI 重绑链
  - dialogue lock 会统一关闭 modal UI、收尾 input / placement / held 壳，并再做 hotbar selection reassert
  - 所以它们更像“把 restart 留下的脏显示重新洗正”，不是 save data 自己又变回来了

### 当前最小修法建议

- 第一刀优先收：
  - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
  - 目标是把 fresh reset 与 UI rebind 顺序改成“先清 fresh runtime，再做一次 bridge 级 UI 重绑”
- 第二刀才看：
  - `Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs`
  - 补硬 toolbar 的重订阅合同，避免 disable/enable 后同引用 early-return

### 验证状态

- 验证方式：静态代码证据链。
- 结论状态：
  - `restart 先旧态 rebind、后 fresh reset`：静态证实
  - `4/8` 没有代码特判：静态证实
  - `ToolbarUI` 订阅恢复存在脆弱点：静态证实
  - `对话/Home 是更强的状态重整入口，所以会恢复`：高概率

---

## 2026-04-18 背包/toolbar/箱子/跨场景回弹链只读并行复盘

- 用户目标：只读并行分析“重开游戏/读取存档后，背包与 toolbar 4/8 槽位、背包交互、sort、垃圾桶、跨场景回弹”这条链，快速钉死最小根因和最安全修法，不改代码。
- 当前主线：继续围绕 inventory save/load + persistent bridge + persistent UI 这条链，判断到底是显示错、数据错，还是跨场景快照回写顺序错。
- 本轮子任务：并行核读 `SaveManager`、`PersistentPlayerSceneBridge`、`InventoryPanelUI`、`InventoryInteractionManager`、`ToolbarUI`、`ToolbarSlotUI`、`InventorySortService`、`InventorySlotUI`、`InventorySlotInteraction`，补看 `HotbarSelectionService`、`PackagePanelTabsUI`、`InventoryService`、`PlayerInventoryData` 的事件与重绑语义。
- 服务于什么：给下一刀真实施工缩成最小安全落点，避免同时去改 save DTO、UI 绑定、箱子交互和跨场景桥接。
- 修复后恢复点：如果下一轮允许施工，优先回到 `SaveManager.LoadGameInternal/ApplyLoadedSaveData` 与 `PersistentPlayerSceneBridge` 的快照同步合同，只把 UI 层改动限制在必要的 rebind/reset。

### 本轮已完成

- 静态确认 `SaveManager.ApplyLoadedSaveData()` 会：
  - 先 `RestorePlayerData / RestoreInventoryData / RestoreAllFromSaveData`
  - 再 `PersistentPlayerSceneBridge.ImportOffSceneWorldSnapshotsFromSave()`
  - 再 `PersistentPlayerSceneBridge.RefreshActiveSceneRuntimeBindings()`
  - 最后 `RefreshAllUI()`
- 静态确认 `PersistentPlayerSceneBridge` 仍单独维护：
  - `inventorySnapshot`
  - `hotbarSelectionSnapshot`
  - `hotbarInventorySelectionSnapshot`
  - 它们只在 `CaptureSceneRuntimeState()` / `ResetPersistentRuntimeForFreshStartInternal()` 更新，没有在普通 `LoadGame` 成功后同步到新读入的数据。
- 静态确认场景重绑时 `RebindScene()` 会无条件：
  - `RestoreSceneInventoryState(sceneInventory)`，直接把 `inventorySnapshot` 打回 runtime inventory
  - `RebindHotbarSelection(...)`，把 `hotbarSelectionSnapshot / hotbarInventorySelectionSnapshot` 打回 selection
- 静态确认 UI/交互层大多会优先取 bridge 的 preferred runtime service，但它们只是消费当前 runtime，不负责纠正 bridge 内部快照是否过时。
- 静态确认 `PlayerInventoryData.LoadFromSaveData()` 只发 `OnInventoryChanged`，不逐槽发 `OnSlotChanged`；因此读档后的很多刷新依赖后续整包 `Build/Refresh/Reassert`，更放大了“谁最后写回 runtime”这个顺序问题。

### 新关键判断

- 当前最小根因不是 `4/8` 特判，也不是 sort/垃圾桶各自独立坏掉，而是同一条状态源冲突：
  - `SaveManager` 已把读档内容恢复到当前 runtime inventory / selection / world-state
  - 但 `PersistentPlayerSceneBridge` 手里的跨场景快照仍可能是读档前旧值
  - 后续只要再触发一次 scene rebind，就会把这份旧 snapshot 重新打回去
- 因此会出现两类表象：
  - `显示异常但 Home/对话后恢复`
    - 本质是后来又触发了更强的 `runtime rebind / selection reset / panel rebuild`
    - 修的是显示壳，不是根因
  - `数据、显示、切场结果像回弹/复制`
    - 本质是 load 后的新 runtime 状态，与 bridge 持有的旧 snapshot 在不同时间点轮流成为“最后一次写入”
- `sort / 垃圾桶 / 背包拖拽 / 箱子交互` 之所以都像会受影响，是因为它们都直接操作当前 runtime inventory：
  - 当场看可能成功
  - 但一旦切场重绑，bridge 旧 snapshot 又能把旧布局打回来，形成“像复制了一份旧背包再盖回来”的体感
- `4/8` 更像症状位，不是代码里写死的特殊槽位：
  - 本轮核读的目标文件内没有针对 `4/8` 的独立逻辑
  - 更符合“旧 selection / 旧 inventory snapshot 恰好在这些槽位最容易被肉眼看见”

### 当前最小最安全修法建议

- 第一优先级只动 `SaveManager` 与 `PersistentPlayerSceneBridge` 的合同，不先碰具体槽位 UI：
  - 在 `SaveManager.ApplyLoadedSaveData()` 成功恢复 runtime 后，立刻把 bridge 的 `inventorySnapshot + hotbarSelectionSnapshot + hotbarInventorySelectionSnapshot` 同步成“刚读进来的新状态”，再做 UI refresh。
  - 最安全做法是新增一个 bridge 侧的显式同步入口，例如“从当前 active runtime 重新捕获快照”或“用读档后的 runtime 覆盖内部 snapshot”，不要让 `SaveManager` 直接反射改私有字段。
- 第二优先级只补一个 UI 壳层兜底：
  - 在 `RefreshAllUI()` 里保留现有 `ConfigureRuntimeContext/Build/ForceRefresh/Reassert` 顺序，但确保 selection reset 发生在 bridge snapshot 已同步之后。
  - 不建议先从 `ToolbarUI / InventoryPanelUI` 里硬写“读档后额外刷新两次”这种表层补丁。
- 第三优先级才考虑 interaction 壳层 reset：
  - `InventoryInteractionManager.ClearHeldState()/HideHeldIcon()`
  - `InventorySlotInteraction.ResetActiveChestHeldState()`
  - 这类只负责防止 held/selection 残影，不负责解决回弹根因。

### 下一轮施工顺序建议

- 第 1 步：`D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PersistentPlayerSceneBridge.cs`
  - 补一个“load 成功后重建 bridge 内部 inventory/hotbar snapshot”的最小公开入口。
- 第 2 步：`D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
  - 在 `ApplyLoadedSaveData()` 里，放在 `PersistentPlayerSceneBridge.RefreshActiveSceneRuntimeBindings()` 前后择一，统一成“先恢复 runtime -> 同步 bridge snapshot -> 再 RefreshAllUI()` 的单方向写入链。
- 第 3 步：仅在第 1/2 步后仍有残留 UI 壳问题时，再看：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Toolbar\ToolbarUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryPanelUI.cs`
  - 只补 rebind/selection reassert，不改业务数据写入。

### 验证状态

- 验证方式：只读静态代码证据链。
- 结论状态：
  - `Load 后 bridge snapshot 未同步，scene rebind 会把旧 inventory/selection 打回 runtime`：静态证实
  - `Home/对话恢复更像二次 rebind / selection reset，而非 save data 自愈`：高概率
  - `sort / 垃圾桶 / 拖拽 / 箱子交互与回弹共根`：静态证据强
  - `4/8 是症状位不是硬编码位`：静态证实

---

## 2026-04-19 toolbar 失联与 Town 空气墙补查

- 用户目标：继续只读排查 `Home` 里 toolbar 空、`Primary` 又恢复的问题，同时把 `Town` 的“空气墙”只读查清并汇报，不先漂去改空气墙。
- 当前主线：把 toolbar 的场景切换失联修到最小安全态；空气墙只做证据归因，不修改 scene。
- 本轮子任务：
  - 核 `ToolbarUI / ToolbarSlotUI / HotbarSelectionService / PersistentPlayerSceneBridge` 的重绑与刷新链
  - 核 `Town.unity` 的真实 collider / tilemap collider / traversal blocker 证据
- 服务于什么：先把 toolbar 的“看起来像没东西”修回去，再把空气墙的原因讲清楚，避免两个问题互相混淆。
- 修复后恢复点：toolbar 只保留 scene 切换后的 runtime 重绑，不再被边界 HUD 淡出误伤；空气墙继续按场景真实 collider 真值排查。

### 本轮已完成

- 已确认 toolbar 问题更像“被边界 HUD 淡出链误伤”，不是背包数据丢失：
  - `PersistentPlayerSceneBridge` 会在 `LateUpdate()` 里跑 `UpdatePersistentUiBoundaryFocus()`
  - 这条链会给名为 `ToolBar` 的 UI 根动态加 `CanvasGroup` 并按玩家 viewport 调 alpha
  - 这会让 Home / Primary 之间出现“toolbar 像空了、但功能还在”的假象
- 已做最小修复：
  - 从 `ResolveNamedBoundaryFocusEdges()` 里移除了 `ToolBar` 的边界淡出映射
  - toolbar 不再进入 boundary focus 的动态淡出列表
- 已做最小验证：
  - `validate_script Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
  - 结果：`assessment=no_red`，`owned_errors=0`，`external_errors=0`
  - `git diff --check` 通过
- 已确认空气墙方向的只读结论：
  - `TraversalBlockManager2D` 本身不生成新 collider，只是收集现有 blocking source 并同步给 nav/grid 约束
  - `Town.unity` 里更像是现实存在的 `TilemapCollider2D / CompositeCollider2D` 在挡人，尤其可疑的是 `基础设施/-9970/-9960/-9975` 和 `轨道/Props_*`

### 现在还没做成什么

- 没做 live / Play 验证，所以 toolbar 修复还停在“代码与编译层已过”，没有手动实机证据。
- 空气墙没有改 scene，只完成了只读归因。

### 当前阶段

- toolbar：已做最小修复，进入等待实机确认。
- 空气墙：只读排查完成，等待后续是否要单独开 scene 验证或针对具体 collider 再缩小范围。

### 下一步只做什么

- 优先做 toolbar 的最小 live 确认：`Home` 进入后 toolbar 不再像空的，切到 `Primary` 也不回弹异常。
- 空气墙如果继续追，只从 `Town.unity` 的真实 collider 对位，不先改 `TraversalBlockManager2D`。

### 需要用户现在做什么

- 暂时只需要你实机看一次 `Home -> Primary`，确认 toolbar 是否还会“看起来空掉”。

### changed_paths

- `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
- `.codex/threads/Sunset/2026-04-17_背包toolbar箱子跨场景只读排查/memory_0.md`

### code_self_check

- toolbar 修复点只动了边界 HUD 淡出映射，没有碰背包数据、hotbar selection 或 save contract。
- 编译自检已过，未引入 owned red。

### pre_sync_validation

- `validate_script`：通过
- `git diff --check`：通过

### 当前是否可直接提交到 main

- 代码层：可以
- 体验层：还差一次 live 确认

### blocker_or_checkpoint

- `checkpoint`: toolbar 边界淡出误伤已切断，等待实机确认

### 一句话摘要

- 这轮把 `ToolBar` 从边界 HUD 淡出链里摘掉了，toolbar 的“Home 里像空了、Primary 又回来”的最像根因已经处理；空气墙则确认更像 Town 里真实 collider 在挡，不是我先前怀疑的那条导航自动收集本身。

## 2026-04-19 追加｜toolbar 图标缺失与 NPC 位置恢复再审

### 当前主线
- 用户目标：继续只读确认 `Home` 场景 toolbar 图标内容缺失的根因，同时补查“默认存档/普通存档里 NPC 位置到底有没有恢复”，并确认语义是否应当只是“恢复坐标、不激活”。
- 本轮子任务：把 toolbar 的 runtime refresh / scene rebind 链和 NPC resident 的保存链彻底分开，不再把两件事混成一个锅。
- 子任务服务于：让后续施工能直接判断是修 UI 刷新、修存档格式，还是补一个只恢复坐标的 NPC 位置回放口。
- 恢复点：如果后续继续，toolbar 先回 `PersistentPlayerSceneBridge.RebindPersistentCoreUi()` / `ToolbarUI` 的订阅时序；NPC 先回 `NpcResidentRuntimeContract` 与正式存档 DTO 的接线缺口。

### 本轮已完成
1. 重新核对 toolbar 相关链：
   - `ToolbarUI.Build()`
   - `ToolbarUI.SyncInventorySubscription()`
   - `ToolbarUI.SyncSelectionSubscription()`
   - `PersistentPlayerSceneBridge.RebindPersistentCoreUi()`
   - `PersistentPlayerSceneBridge.ForceRuntimeUiRefreshAfterSceneRebind()`
2. 重新核对 NPC resident 相关链：
   - `NpcResidentRuntimeSnapshot.cs`
   - `NpcResidentRuntimeContract.cs`
   - `NPCAutoRoamController.cs`
   - `PersistentPlayerSceneBridge` 中的 `CaptureNativeResidentRuntimeState()` / `RestoreNativeResidentRuntimeState()`
   - `SaveDataDTOs.cs`
   - `SaveManager.cs`
3. 钉实两条关键事实：
   - toolbar 图标缺失更像切场后的 runtime refresh / 订阅链时序问题，不像背包数据被真的清空
   - NPC resident 位置快照现在只存在于 bridge 的运行态缓存里，没有进入正式 `GameSaveData` DTO

### 关键判断
1. toolbar 问题与 NPC 存档恢复不是同一个问题：
   - toolbar 是 UI 刷新 / 订阅 / 切场时序
   - NPC 是正式存档格式里有没有这份 resident 位置语义
2. 当前 NPC 这条线的真相更接近：
   - 场景切换时 bridge 记得 resident 快照
   - 真正读写存档时并没有把 resident 快照写进 `GameSaveData`
   - 所以默认存档和普通存档都不能算已经完成“NPC 位置保留”
3. 另外，当前 bridge 的恢复口是 `resumeResidentLogic: true`：
   - 这意味着它不是单纯“把 NPC 摆回坐标”
   - 它还可能让 NPC 继续漫游或恢复脚本控制
   - 这和用户想要的“只恢复坐标，不激活”不是一回事

### 验证状态
- 已完成：静态代码审查。
- 未完成：Unity live / packaged 复现。
- 当前口径：`静态推断成立`。

### 下一步
- 如果后续要修 toolbar，优先继续收 `RebindPersistentCoreUi -> ToolbarUI` 的 refresh / subscription 真值链。
- 如果后续要补 NPC 存档恢复，第一刀应先把 resident 快照纳入正式存档格式，再单独做“只恢复坐标、不恢复 roam”的恢复口，而不是直接复用现在这条会续跑的 bridge 恢复口。

## 2026-04-19 追加｜箱子放置真值与推墙阻挡最小修复

- 用户目标：直接修掉箱子两条打包前硬 bug：
  1. 放置预览位置与实际落点不一致
  2. 推动时箱子不像玩家那样被墙挡住
- 当前主线：把箱子显示真值、放置真值、碰撞真值统一到同一套 collider 合同上，不再继续漂去 toolbar、存档或空气墙。
- 本轮子任务：
  - 查清 `ChestController` 的真实碰撞链
  - 查清 `PlacementGridCalculator` 的真实落点链
  - 用最小代码改动收住这两条
- 服务于什么：把箱子线从“看起来已经修过很多次，但实际还会错位/穿墙”收回到可直接做打包前实测的状态。
- 修复后恢复点：如果下一轮继续，只需要实机验证箱子 `预览=落点`、`贴墙不穿` 两件事，不需要再回到根因收缩阶段。

### 本轮已完成
1. `Assets/YYY_Scripts/Service/Placement/PlacementGridCalculator.cs`
   - 新增 `GetPlacementColliderLocalCenter()`
   - `GetPlacementPosition()` 改为按真实 collider 几何中心算落点
   - `TryGetPlacementReachEnvelopeBounds()` 改为按真实 collider footprint 算导航 envelope
   - `GetPreviewSpriteLocalPosition()` 也改成共用同一套 collider 中心合同
2. `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
   - `TryPush()` 从 `Physics2D.OverlapCircleAll` 改为 `Collider2D.Cast`
   - 加 `0.02f` 薄 skin，减少贴边漏判
3. `Assets/YYY_Tests/Editor/ChestPlacementGridTests.cs`
   - 新增箱子 placement/reach/preview 合同测试
   - 新增推墙逻辑文本护栏，防止退回旧的小圆探测
4. `thread-state`
   - 已跑 `Begin-Slice`
   - 已跑 `Park-Slice`
   - 当前 live 状态：`PARKED`

### 关键判断
1. 放置错位主因不是 prefab 摆歪，而是 `GetPlacementPosition()` 之前把视觉层“假想底部对齐偏移”错误地算进了真实落点。
2. 推墙穿模主因不是箱子没有 collider，而是阻挡检测只在目标中心点做了一个小圆探测，对宽箱子和贴墙情况不成立。
3. 这轮最安全的修法不是去大改 prefab，也不是回退 `__ChestSpriteVisual` 整条链，而是让放置/预览/碰撞都只跟真实 collider 中心走。

### 验证结果
1. `git diff --check -- Assets/YYY_Scripts/World/Placeable/ChestController.cs Assets/YYY_Scripts/Service/Placement/PlacementGridCalculator.cs Assets/YYY_Tests/Editor/ChestPlacementGridTests.cs`
   - 通过
2. `validate_script` / `compile --skip-mcp`
   - 被 `CodexCodeGuard timeout` 和 `CLI/Unity bridge` 阻塞
   - 没拿到 live compile 绿票
3. 当前能诚实落的验证状态：
   - `代码改动已落地`
   - `结构自检成立`
   - `Unity live / packaged 终验待补`

### blocker_or_checkpoint
- `checkpoint`: 箱子放置真值与推墙阻挡的最小修复已落地
- `blocker`:
  - `CLI/Unity bridge` 当前阻塞 fresh compile/live 绿票
  - 仍待用户实机重点复测：`预览=落点`、`贴墙推动不穿`

## 2026-04-19 追加｜ChestPlacementGridTests 编译尾账修复

- 用户反馈：`Assets/YYY_Tests/Editor/ChestPlacementGridTests.cs` 出现 6 个 `CS0103`，都指向 `PlacementGridCalculator`。
- 当前主线：不扩散去改 asmdef 结构，只把这份新测试的编译尾账收干净。
- 根因：
  1. `PlacementGridCalculator` 本体存在于运行时代码中。
  2. 但 `Tests.Editor.asmdef` 本身没有直接引用运行时代码程序集。
  3. 所以测试里用强类型直接点 `PlacementGridCalculator.xxx()` 会在编译期报 `CS0103`。
- 本轮已做：
  1. `ChestPlacementGridTests.cs` 改为 `AppDomain + Reflection` 调用运行时静态方法
  2. 保留原来 4 组护栏语义不变
  3. 未去改 `Tests.Editor.asmdef`
- 当前结论：
  1. 这组 `CS0103` 属于测试程序集边界问题，不是放置修复本身失效
  2. 最小安全修法已经落在测试文件内部，不会继续把修改面扩散到整个测试编译结构
