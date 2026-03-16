# 1.0.0 图层与浇水修正 - 任务清单

- [x] 1. 回读 `PlacementManager.ExecuteSeedPlacement(...)`、`CropController` 与 `DynamicSortingOrder`，确定作物排序补点。
- [x] 2. 在种植成功链上补齐作物的 sorting layer / sorting order / dynamic sorting 接入。
- [x] 3. 回读 `FarmToolPreview` 与 `GameInputManager` 的浇水预览状态字段，确认当前随机样式触发路径。
- [x] 4. 将浇水样式刷新时机修正为“成功入队后，移出当前格时才生成下一次随机样式”。
- [x] 5. 运行本地编译验证并记录 1.0.0 的结果。
- [x] 6. 更新 1.0.0 子工作区 memory。
