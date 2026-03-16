# 1.0.0 图层与浇水修正 - 设计文档

## 设计目标

以最小侵入方式修复作物排序和浇水样式时机，不重写农田主流程。

## 方案一：作物排序修正

### 现状
- 普通 placeable 的放置流程会在实例化后调用：
  - `SyncLayerToPlacedObject(...)`
  - `SetSortingOrder(...)`
- 种子专用流程 `ExecuteSeedPlacement(...)` 只实例化 `cropPrefab` 并初始化 `CropController`，没有接入上述排序同步链。
- `CropController` 自身只有 `AlignSpriteBottom()`，没有动态排序。

### 设计
- 在 `ExecuteSeedPlacement(...)` 成功实例化作物后：
  - 先沿用放置链的楼层与 sorting layer 同步；
  - 再补齐静态初始 order；
  - 最后确保作物实际渲染节点上存在 `DynamicSortingOrder`，让其与玩家运动形成持续正确的前后遮挡。
- 如果 prefab 上已存在 `DynamicSortingOrder`，则只同步配置，不重复挂载。
- 优先把组件挂到 `CropController` 所在的渲染节点，而不是盲目挂在根节点。

## 方案二：浇水随机时机修正

### 现状
- `FarmToolPreview` 仍保留 `_lastWateringCellPos`、`_cachedPuddleVariant`、`_needsNewPuddleVariant`。
- 当前 `OnWaterExecuted(...)` 只记录位置，没有重新驱动“移出当前格才随机”的机制。

### 设计
- 保留三元状态，但重新定义语义：
  - `_cachedPuddleVariant`：当前正在显示的样式
  - `_lastWateringCellPos`：最近一次成功入队/执行的格子
  - `_needsNewPuddleVariant`：下一次离开 `_lastWateringCellPos` 时，才允许生成新样式
- `GameInputManager` 在浇水成功入队时明确设置 `_needsNewPuddleVariant = true`，并同步记录当前格。
- `FarmToolPreview.UpdateWateringPreview(...)` 中只在“`_needsNewPuddleVariant == true` 且当前 cell != _lastWateringCellPos`”时随机新样式。
- 真正随机后立即清掉 `_needsNewPuddleVariant`，避免同一次离格连续跳样式。

## 风险控制

- 不修改作物保存结构，不改 `CropController.Initialize(...)` 数据契约。
- 不改 `FarmTileManager.SetWatered(...)` 的真实落地逻辑，只修预览状态机与调用口径。
