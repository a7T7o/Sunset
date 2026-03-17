# 遮挡检查工作区记忆

## 会话1
- 用户要求彻查项目内与遮挡功能挂钩的全部代码、引用、导航联动与性能风险，并允许自由组织输出文件。
- 已完成：全链路定位（OcclusionManager/OcclusionTransparency/TreeController/PlacementPreview/NavGrid2D/命中链）。
- 已产出文档：`遮挡系统代码索引与调用链.md`、`遮挡-导航-命中一致性与性能风险报告.md`。
- 关键结论：命中检测在工具链（Collider bounds）与点击交互链（Sprite bounds）存在策略分裂；性能热点为遮挡全量遍历、像素采样、导航全图重建叠加。

## 会话2 - 2026-03-15

- 当前主线目标：把线程旧稿与当前真实遮挡实现重新对齐，形成可继续接手的审计基线。
- 本轮子任务：补建缺失的 Sunset 子工作区记忆、父工作区记忆与线程根 `memory_0.md`，并新增一份 Codex 视角核实分析。
- 已完成核实：
  - 纠正旧稿里的脚本 GUID 映射错误。
  - 确认 `Primary.unity` 当前 `OcclusionManager` 参数与旧稿漂移。
  - 确认 `Tree/M1~M3.prefab` 都存在父/子双 `OcclusionTransparency`。
  - 确认 `TreeController` 只 `GetComponent<OcclusionTransparency>()`，阶段切换只能控制当前节点那一份组件。
  - 确认命中判定在 `PlayerToolHitEmitter` 与 `GameInputManager` 之间仍然是双标准。
  - 确认树上像素采样真实开启，编辑器批处理工具与测试覆盖已经落后于当前实现。
- Unity 侧验证：
  - `Primary` 当前已加载且 `isDirty = false`
  - Console 有 1 条与 `NPCPrefabGeneratorTool` 相关的 warning
  - `OcclusionSystemTests` EditMode 11/11 通过
- 本轮新增文档：
  - `03_遮挡现状核实与差异分析（Codex视角）.md`
- 当前恢复点：
  - 阻塞已解除，工作区 / 线程记忆已补齐，主线已回到“基线建立完成，等待后续整改设计”的阶段。
