# 遮挡检查工作区记忆

## 会话1
- 用户要求彻查项目内与遮挡功能挂钩的全部代码、引用、导航联动与性能风险，并允许自由组织输出文件。
- 已完成：全链路定位（OcclusionManager/OcclusionTransparency/TreeController/PlacementPreview/NavGrid2D/命中链）。
- 已产出文档：`遮挡系统代码索引与调用链.md`、`遮挡-导航-命中一致性与性能风险报告.md`。
- 关键结论：命中检测在工具链（Collider bounds）与点击交互链（Sprite bounds）存在策略分裂；性能热点为遮挡全量遍历、像素采样、导航全图重建叠加。
