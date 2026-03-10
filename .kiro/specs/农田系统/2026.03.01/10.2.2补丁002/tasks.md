# 10.2.2补丁002 任务清单

版本: v1.0  
日期: 2026-03-10  
状态: 文档已建档，待用户审核；暂不进入代码实现

## 0. 文档建档

- [x] 1. 创建 `10.2.2补丁002` 工作区
- [x] 2. 创建 `requirements.md`
- [x] 3. 创建 `analysis.md`
- [x] 4. 创建 `design.md`
- [x] 5. 创建 `tasks.md`
- [x] 6. 创建 `memory.md`

## 1. 理解纠偏

- [x] 7. 重新核对 `PlacementPreview`、`PlacementValidator`、`PlacementManager` 的真实职责边界
- [x] 8. 重新核对 `10.1.5补丁005` 中种子并入放置系统后的原始设计
- [x] 9. 重新核对 `10.2.0改进001` 中 `V` 键统一授权的正式语义
- [x] 10. 重新核对 `SeedData`、`SaplingData`、普通 `PlaceableItemData` 的现有代码分流事实

## 2. 方案结论

- [x] 11. 明确“普通 placeable 的耕地禁放”必须走逐格红判定，不能只在执行层失败
- [x] 12. 明确“普通 placeable 的耕地禁放”不能挂在整物品级 `CanPlaceAt()` 上
- [x] 13. 明确 `SeedData` 继续走 `PlacementPreview`，但验证语义应收敛到播种本身
- [x] 14. 明确 `SaplingData` 继续保留树苗专用验证链，不并入普通 placeable 方案
- [x] 15. 明确 `FarmToolPreview` 的 `1.5 x 1.5 footprint` 不向普通 placeable 泛化

## 3. 待实现任务（等待用户审核后再进入）

- [ ] 16. 为普通 `PlaceableItemData` 增加“逐格耕地禁放”验证分支
- [ ] 17. 将普通 placeable 压耕地的无效结果回传到 `PlacementPreview.UpdateCellStates(...)`
- [ ] 18. 校准普通 placeable 的多格预览，确保“哪些格子踩耕地，哪些格子标红”
- [ ] 19. 收窄 `ValidateSeedPlacement(...)`，避免再被普通 placeable / 农具施工障碍规则误伤
- [ ] 20. 复核 `ValidateSaplingPlacement(...)` 与 `TreeController` 成长边距的边界口径，确保树苗继续走专用链
- [ ] 21. 做一轮专门针对“箱子 / 种子 / 树苗”的手动回归清单

## 4. 待验证场景

- [ ] 22. 箱子横跨草地与耕地时，只有压住耕地的格子标红
- [ ] 23. 箱子完全落在空草地上时，保持原始格子检测逻辑
- [ ] 24. 种子指向可种植耕地时，仍为标准绿色方框 + 作物预览
- [ ] 25. 种子不因普通 placeable 的禁耕地规则被一并挡掉
- [ ] 26. 树苗继续受冬季 / 耕地 / 树距等专用规则约束，不退化成普通 placeable
