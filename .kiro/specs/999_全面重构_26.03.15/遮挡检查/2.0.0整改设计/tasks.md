# 2.0.0 整改任务

- [x] 在 `main` 上恢复 `2.0.0整改设计` 四件套
- [x] 迁入旧分支 `295e8138` 中仍有效的 `BatchAddOcclusionComponents.cs` 对齐改动
- [x] 让 `TreeController` 联动同树范围内的全部 `OcclusionTransparency`
- [x] 让 `OcclusionManager` 按物理树去重处理树林缓存与恢复
- [x] 补一条“父/子双组件属于同一物理树”的最小 EditMode 证据
- [x] 运行关键脚本验证与 `OcclusionSystemTests` EditMode 验证
- [x] 修复同树双组件下的砍伐高亮遗漏
- [x] 统一 `OcclusionManager` 内树相关位置来源为“物理树根位置”
- [x] 补一条“同树砍伐高亮同步”的最小 EditMode 证据
- [x] 补“同树归属 / 同树高亮 / 同树 Contains”专项测试
- [x] 补“边界树单树透明 / 内侧树整林透明 / 林内整林透明 / preview 恢复 / 父组件成长阶段读取”行为测试
- [x] 将 `currentForestBounds` 从根位置粗包围盒改为 `ColliderBounds` 联合包围盒
- [x] 用 Unity 重新跑新增的边界/preview 行为测试并确认全部通过
- [x] 评估是否进入点击命中 / 工具命中双标准的下一条业务线
  - 结论：属于下一阶段新业务，不属于本轮遮挡/林判断整改收口
