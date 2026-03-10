# 10.2.1补丁001 任务清单

版本: v1.0  
日期: 2026-03-10  
状态: 已完成实现，独立 Roslyn 编译通过，待用户在 Unity 编辑器内做手动回归验证

## 0. 文档建档

- [x] 1. 在 `10.2.1补丁001` 下创建 `requirements.md`、`analysis.md`、`design.md`、`tasks.md`、`memory.md`
- [x] 2. 将五个主问题和三个补充风险全部落入审查文档
- [x] 3. 将本轮接手结论同步到子工作区 `memory.md`
- [x] 4. 将本轮摘要同步到父工作区 `农田系统/memory.md`
- [x] 5. 将本轮摘要同步到线程记忆 `D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/农田交互修复V2/memory_0.md`

## 1. 放置成功语义统一

- [x] 6. 为当前 placeable 验证建立统一入口，避免初始锁定、导航重达、导航中改点位分别走不同判定
- [x] 7. 修复 `PlacementManager` 在 `Navigating` 状态点击新位置时退回通用 `ValidateCells()` 的问题
- [x] 8. 让 `OnNavigationReached()` 在执行放置前做同源重验，而不是直接 `ExecutePlacement()`
- [x] 9. 校准 `PlacementNavigator` 与 `PlayerAutoNavigator` 的“到达即可执行”语义，确保导航停止与放置成功使用同一套边界口径

## 2. 作物占位统一接入

- [x] 10. 在 `FarmTileManager` 侧补齐作物占位查询接口，统一暴露 `FarmTileData.cropController` 事实
- [x] 11. 让 `PlacementValidator.HasObstacle()` 把作物占位纳入 placeable 阻挡
- [x] 12. 评估并补齐 `PlacementValidator.HasFarmingObstacle()` 对作物占位的读取需求
- [x] 13. 实现 `PlacementValidator.IsOnFarmland()` 的真实判断，不再保留 `TODO + false`

## 3. `V` 施工模式语义隔离

- [x] 14. 修复 `GameInputManager.TryEnqueueFarmTool()` 中 `Hoe` 因 `farmPreview.HasCrop` 自动转成 `RemoveCrop` 的逻辑
- [x] 15. 让施工模式下的 `Hoe` 对作物格给出明确无效反馈，而不是继续走破坏链
- [x] 16. 修复施工模式下锄头仍可触发 `TreeController.HandleSaplingDigOut()` 的问题
- [x] 17. 回归检查 `V` 对 placeable 与农田工具的统一授权语义，确保不破坏 10.2.0 已确认的正确方向

## 4. “1+8 + 0.75/1/1.5” 口径统一

- [x] 18. 清理相关注释与命名，统一“1+8 视觉结构下的检测尺度统一”表述
- [x] 19. 校对 `PlacementValidator` 中对 `OverlapBoxAll` 尺度的注释，避免把完整尺寸误写成 half extents
- [x] 20. 评估是否需要把 `0.75`、`1`、`1.5` 提炼为带语义的命名常量

## 5. 树成长并入作物占位

- [x] 21. 在 `TreeController` 的成长空间检测链中接入作物占位查询
- [x] 22. 让成长受阻反馈能够区分“作物阻挡”与现有 `Tree / Rock / Building` 阻挡
- [x] 23. 回归检查树苗成长、树苗放置、作物占位三者之间的边界是否一致

## 6. 补充风险收口

- [x] 24. 评估并收口 `GetCurrentLayerIndex()` 恒返回 `0` 对本轮修复链路的影响
- [x] 25. 优先在已知上下文里传递真实 `layerIndex`，减少对假楼层值的依赖
- [x] 26. 完成一轮针对箱子、树苗、种子、Hoe 的手动回归验证方案整理

## 7. 审核与实现切换条件

- [x] 27. 等待用户审核本轮审查文档
- [x] 28. 用户确认后再进入代码实现阶段

## 8. 待执行手动回归清单

以下清单仅表示本轮补丁必须在 Unity 编辑器内逐项回归的验收项，当前尚未逐条现场确认：

- [ ] 1. 箱子放置到已有作物的耕地上时，应保持红色无效预览，且导航到位后也不能落地。
- [ ] 2. `V` 开启后，`Hoe` 指向已有作物格时，应显示明确无效反馈，不能转成 `RemoveCrop`，也不能挖掉树苗。
- [ ] 3. `V` 开启后，`WateringCan` 指向已有作物耕地时，应允许浇水，不应被作物占位误判成障碍物。
- [ ] 4. 种子播种、树苗放置与 placeable 导航重达时，都应围绕同一格心坐标和 `1.5 x 1.5` footprint 重验。
- [ ] 5. 树苗成长检测时，四方向若命中作物占位，应输出“作物阻挡”诊断并阻止成长。
