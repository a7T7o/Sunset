# 2.0.0 整改需求

## 目标
- 把 `codex/occlusion-audit-001 @ 295e8138` 中仍然有效的整改入口迁回 `main`。
- 先解决“同一物理树被父/子双 `OcclusionTransparency` 按两个组件处理”的首个真实主链问题。
- 同步收掉会继续制造旧数据的 `BatchAddOcclusionComponents.cs` 历史入口。

## 已确认事实
- `Tree/M1.prefab`、`M2.prefab`、`M3.prefab` 的根节点和 `Tree` 子节点都带 `OcclusionTransparency`，且根节点也带 `Tree` 标签。
- `TreeController` 之前只缓存当前节点上的一份 `OcclusionTransparency`，树苗 / 树桩阶段无法同步关闭同树的另一份组件。
- `OcclusionManager` 之前按组件粒度扫描、建树林和做 `overlappingTreeCount >= 2` 保底，同树双组件会被误当成两棵树。
- `BatchAddOcclusionComponents.cs` 仍在写已删除的 `affectChildren` / `occlusionTags` 字段。

## 本轮最小需求
- 在 `main` 上固化 `2.0.0整改设计` 四件套。
- 让 `TreeController` 能联动同一物理树的全部相关遮挡组件。
- 让 `OcclusionManager` 以“物理树”而不是“组件实例”处理树林去重和恢复逻辑。
- 把 `BatchAddOcclusionComponents.cs` 对齐到当前 `OcclusionTransparency` 定义。
- 补一条能证明父/子双组件属于同一物理树的最小 EditMode 证据。

## 本轮禁止
- 不碰 `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
- 不碰 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- 不扩到 NPC / 导航 / 场景手调
- 不把旧分支残留继续当默认 blocker

## 完成标准
- `main` 上已经存在可继续承接的 `2.0.0整改设计` 文档。
- `TreeController` 不再只控制当前节点上的单份遮挡组件。
- `OcclusionManager` 的树林缓存 / 恢复逻辑不再把同树双组件视作两棵树。
- `BatchAddOcclusionComponents.cs` 不再写入过时字段。
