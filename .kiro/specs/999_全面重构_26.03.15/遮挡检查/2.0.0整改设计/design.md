# 2.0.0 整改设计

## 设计原则
- 先修对象粒度，再谈更大范围的遮挡体验优化。
- 先把历史分支里仍然有效的入口迁回 `main`，再继续真实整改。
- 不依赖场景手调，优先用脚本层和最小测试固化首刀。

## 本轮 checkpoint
- `TreeController` 改为缓存整棵树范围内的相关 `OcclusionTransparency`，阶段切换时统一开关遮挡。
- `OcclusionTransparency` 新增“物理树根”识别能力，用于把父/子双组件归到同一棵树。
- `OcclusionManager` 改为按物理树去重处理：
  - 树林 Flood Fill 不再把同树父/子组件都加入 `currentForest`
  - 树林命中 / 恢复 / preview 恢复按物理树判包含
  - 对同一物理树的透明切换统一下发到同树全部相关组件
- `BatchAddOcclusionComponents.cs` 迁入旧分支里已验证有效的工具对齐版。

## 为什么先这样做
- 这刀直接命中此前审计锁定的两个主问题：
  - `TreeController` 单组件联动导致阶段禁用可能失效
  - `OcclusionManager` 组件粒度导致树林保底误判
- 改动面只落在遮挡脚本、编辑器工具和专项测试，不碰共享场景和高危热文件。

## 仍未覆盖的点
- 还没处理点击命中 / 工具命中的双标准问题。
- 还没验证所有真实树 prefab / 场景实例在 Unity live 下的体感效果。
- 如果后续需要进一步收口，下一刀优先考虑：
  - 补更完整的 `OcclusionSystemTests`
  - 核查 `ContainsPointPrecise` 与树林边缘透明在 live 场景里的表现
