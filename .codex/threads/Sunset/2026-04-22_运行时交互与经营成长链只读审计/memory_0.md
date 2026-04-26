# 2026-04-22 运行时交互与经营成长链只读审计

- 用户目标：在 `D:\Unity\Unity_learning\Sunset` 内做一轮只读审计，围绕经营成长与运行时交互相关 live 文档和真实代码，回答哪些系统已经接成可运行链、共享真值和状态承接点在哪里、哪些事实适合转写成技术策划简历、哪些旧文档表述已经过时或容易误写。
- 当前主线：不是改代码或修规则，而是把 `Sunset` 当前“运行时交互 + 经营成长链”的真实接线情况压成一份可继续迭代的技术底稿。
- 本轮子任务：读取 `02_经营成长.md`、`03_交互系统.md`、`08_进度总表.md`、`.kiro\about\01_系统架构与代码全景.md`，并核对 `GameInputManager`、`InventoryService`、`PlacementManager`、`FarmTileManager`、`CropController`、`CraftingService`、`InventoryInteractionManager`、`BoxPanelUI`、`ChestController` 等真实运行时代码；为确认制作链边界，额外补读了 `WorkstationData`、`CraftingStationInteractable`、`SpringDay1WorkbenchCraftingOverlay`、`PackagePanelTabsUI`。
- 服务于什么：为后续技术策划草稿本、简历事实句、系统总览或进一步只读拆解提供可靠母本，避免把旧设计口径、历史命名和局部链误写成“全链已落地”。
- 修复后恢复点：如果用户下一轮继续深挖，应优先补 `PersistentPlayerSceneBridge`、`HotbarSelectionService`、工作台/配方资源资产与 `Resources/Story/SpringDay1Workbench` 的实际配方资产，进一步确认跨场景承接与 Workbench 之外各站点的闭环程度。

## 本轮已完成

- 已确认真正已经接成运行时主链的高置信部分：
  - `GameInputManager` 主导的输入优先级总调度链：面板/剧情锁 -> 农田链 -> 放置链 -> 世界 `IInteractable` -> 通用工具。
  - `InventoryService + HotbarSelection + InventoryInteractionManager + PackagePanelTabsUI + BoxPanelUI` 组成的玩家库存/面板/箱子共享视图链。
  - `PlacementManager` 的 `Preview -> Locked -> Navigating -> Executing` 放置状态机，以及种子专用分支。
  - `FarmTileManager + CropController + TimeManager + GameInputManager` 组成的农田播种/生长/收获链。
  - `ChestController + PackagePanelTabsUI + BoxPanelUI + InventoryInteractionManager` 组成的世界箱子交互链。
- 已确认“局部接通但不宜写成全站点完整闭环”的部分：
  - `CraftingStationInteractable -> CraftingService -> SpringDay1WorkbenchCraftingOverlay` 的 `Workbench` 特化制作链已经存在，且支持材料预扣、队列完成通知和运行时 overlay。
  - 但 `WorkstationData.OnPlaced()` 仍是 TODO，通用工作台落地后的世界设施闭环证据不足，不宜写成“所有站点都已完整落地”。
- 已确认关键共享真值：
  - 玩家库存真值：`InventoryService` 内部的 `PlayerInventoryData / InventoryItem`。
  - 农田占位和湿润真值：`FarmTileManager` 中按楼层分组的 `FarmTileData`。
  - 作物运行态真值：`CropController.state + CropInstanceData`，并回写/绑定 `FarmTileData.cropController` 与 `cropData`。
  - 放置承诺真值：`PlacementSnapshot`。
  - 箱子运行时真值：`ChestController.RuntimeInventory`（`_inventoryV2` 优先，旧 `_inventory` 为兼容镜像）。
  - Workbench 队列真值：`SpringDay1WorkbenchCraftingOverlay` 中的 `_queueEntries / _activeQueueEntry`，材料真值仍回到 `InventoryService`。

## 关键判断

- 当前最应该被视为“已经不是功能并列，而是真链路”的，是：
  - 输入优先级总调度链。
  - 背包/Hotbar/箱子同源库存链。
  - 农田主循环链。
  - 放置状态机及其种子分支。
  - 箱子世界对象到 UI/库存/持久化的完整交互链。
- 当前最容易被文档写满、但实际仍需克制表述的，是制作/工作台/配方：
  - `Workbench` 特化链可写成“已有运行时闭环样例”。
  - 不能直接写成“全站点经营制作链已完整落地”。
- 旧口径里最危险的误写点：
  - 把 `CropManager` 继续写成农田主循环核心。
  - 把收获写成 `CropManager.TryHarvest` 或“成熟后直接入背包”。
  - 把库存结构写回 `20 + 8`。
  - 把 `BoxPanelUI` 的 Down 区写成“去掉 Hotbar 的背包剩余区”。
  - 把当前 `PlacementManager` 继续统称成现役 `PlacementManagerV3`。

## 验证状态

- 验证方式：只读核对指定 live 文档与真实代码；未进入 Unity/MCP live 验证，未改任何业务代码。
- 结论状态：
  - 输入/库存/箱子/农田/放置主链：代码级证据较强，属高置信静态判断。
  - `Workbench` 制作链：代码级局部闭环存在，但资产/场景覆盖范围仍待补证。
  - “全站点制作经营闭环”：当前不能成立，只能写成方向与局部样例已接通。

## 遗留与下一步

- 尚未做：
  - 未核对 `PersistentPlayerSceneBridge.cs`、`HotbarSelectionService.cs`、`RecipeData` 资源资产本体、`MasterItemDatabase.asset` 与 `Resources/Story/SpringDay1Workbench` 的实际配置差异。
  - 未做 Unity 运行态验证，因此没有把静态链路进一步提升为 live 终验结论。
- 如果继续：
  - 第一步应补读跨场景桥接与 Hotbar 选中承接。
  - 第二步应补核配方资源与各站点 prefab / scene 落点，分清 `Workbench 特化链` 与 `通用工作台链` 的边界。
  - 第三步再把这轮底稿压成简历版项目事实句或技术策划草稿本正文。
