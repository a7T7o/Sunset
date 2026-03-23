# 2.0.0 整改设计 memory

## 2026-03-22｜首个真实整改 checkpoint
- 当前主线目标：不再停在审计态或旧分支 blocker，而是在 `main` 上落下第一刀真实遮挡整改。
- 本轮子任务：处理 `295e8138` 的历史遗留，并把“树 parent/child 双 `OcclusionTransparency`”的首个真实修正落回 `main`。
- 已完成：
  - 将 `2.0.0整改设计` 四件套迁回 `main` 并改写为当前口径
  - 让 `TreeController` 不再只控制当前节点上一份遮挡组件
  - 为 `OcclusionTransparency` 增加同树识别能力
  - 让 `OcclusionManager` 在树林缓存 / 恢复 / preview 恢复中按物理树去重
  - 把 `BatchAddOcclusionComponents.cs` 对齐到当前组件字段
  - 补一条最小 EditMode 测试，证明父/子双组件属于同一物理树
- 关键判断：
  - `295e8138` 中仍有价值的部分是 `BatchAdd` 工具收口和 `2.0.0` 设计入口，已迁入并更新。
  - 旧分支里的“只先做 docs-first 收口”口径现在判废，不再作为继续阻塞的理由。
- 恢复点 / 下一步：
  - 优先做 Unity 编译 / EditMode 验证。
  - 如果验证通过，下一刀在“补更完整测试”与“继续压缩树林边缘误判”之间二选一。

## 2026-03-22｜自审补丁与验证补齐
- 本轮自审发现的遗漏：
  - `OcclusionManager.SetChoppingTree()` 原先仍只给单个组件设置砍伐高亮，没有同步到同一物理树的另一份 `OcclusionTransparency`。
  - `OcclusionSystemTests.cs` 新增测试最初直接引用运行时代码类型，和 `Tests.Editor.asmdef` 的装配边界不兼容。
- 已补的修正：
  - `OcclusionManager` 新增同树级别的 `SetPhysicalTreeChopping(...)`，`SetChoppingTree()` 与 `ClearChoppingHighlight()` 现在都会同步处理同树全部相关组件。
  - `OcclusionSystemTests.cs` 改为反射方式取运行时类型与方法，不再依赖测试程序集直接引用 `TreeController` / `OcclusionTransparency`。
  - `OcclusionTransparency.cs` 曾被错误解码写坏；已先从 Git 健康版本恢复，再只追加本轮需要的方法补丁。
- 最新验证结果：
  - `validate_script`：
    - `OcclusionTransparency.cs` = 0 error
    - `OcclusionManager.cs` = 0 error
    - `TreeController.cs` = 0 error
    - `OcclusionSystemTests.cs` = 0 error
  - `read_console`：0 error / 0 warning
  - `OcclusionSystemTests` EditMode：12/12 passed
- 当前恢复点：
  - 本轮首个真实遮挡整改 checkpoint 已完成代码、文档、自审与最小测试验证。
  - 下一刀可以直接进入“补更完整测试覆盖”或“继续压树林边缘误判”的真实业务推进。

## 2026-03-22｜第二刀已启动
- 第二刀当前锁定的问题：
  - `OcclusionManager` 里树相关距离过滤、树林缓存边界和边缘方向判断，仍混用组件节点位置与物理树根位置。
- 本轮已落下的第二刀改动：
  - 新增 `GetOccluderReferencePosition(...)`，把树类遮挡物的参考位置统一到物理树根。
  - 预览距离过滤、运行时距离过滤、`FindConnectedForest()` 初始边界 / 搜索半径 / min-max 边界更新、以及边缘方向日志全部改用统一参考位置。
  - 新增 `ChoppingHighlight_ParentAndChildComponents_AreUpdatedTogether` 测试，锁住同树砍伐高亮同步行为。
- 本轮验证结果：
  - `OcclusionManager.cs` `validate_script = 0 error`
  - `OcclusionSystemTests` EditMode = 13/13 passed
- 第二刀下一步清单：
  - 继续补树林边缘透明 / preview / 恢复链路的针对性测试。
  - 如果测试暴露问题，再决定是否继续收口 `currentForestBounds` / `currentlyOccluding` 的组件级缓存残留。

## 2026-03-22｜第二刀第一段完成
- 本段完成内容：
  - `OcclusionManager` 的树相关参考位置已统一到物理树根：
    - 预览距离过滤
    - 运行时距离过滤
    - `FindConnectedForest()` 起点与边界更新
    - 边缘方向判断
  - 自审时继续修掉了恢复路径里残留的一处 `currentlyOccluding.Contains(occluder)` 组件级判断，改为 `ContainsPhysicalTree(...)`
  - 新增两条回归测试：
    - `ChoppingHighlight_ParentAndChildComponents_AreUpdatedTogether`
    - `ContainsPhysicalTree_ParentAndChildComponents_ReturnsTrue`
- 本段验证结果：
  - `OcclusionSystemTests` 已扩到 14 条
  - `OcclusionSystemTests` EditMode = 14/14 passed
  - 清空 Console 后，`OcclusionTransparency` 的“未找到 OcclusionManager（等待超时）”没有再次复现
- 当前判断：
  - 第二刀第一段已经形成稳定 checkpoint。
  - 目前最值得继续推进的是“树林边缘透明 / preview / 恢复链路”的行为测试，而不是继续盲改主链。

## 2026-03-23｜第二刀继续落地
- 本轮继续补的业务点：
  - `IsBoundaryTree()` 改为基于物理树根参考点而不是树冠中心判边界
  - 边界阈值收紧到 `0.75f`
  - `currentForestBounds` 改为基于 `ColliderBounds` 联合包围盒
  - `GetTreeGrowthStageIndex()` 现在会从 child / parent 方向补找 `TreeController`
- 本轮新增测试：
  - `TreeGrowthStage_ParentOcclusion_UsesChildTreeController`
  - `BoundaryClassification_CenterTree_IsNotBoundaryTree`
  - `ForestOcclusion_BoundaryTreeOutsideForest_OnlyTargetTreeTransparent`
  - `ForestOcclusion_InteriorTreeTrigger_MakesEntireForestTransparent`
  - `ForestOcclusion_PlayerInsideForest_MakesEntireForestTransparent`
  - `PreviewOcclusion_ClearPreviewBounds_RestoresTransparency`
- 当前判断：
  - 这批测试把“边界树 / 内侧树 / 林内 / preview 恢复 / 父组件成长阶段读取”五条链路都锁进来了。
  - 受当前 MCP 通道波动影响，本轮还需要再补一次 Unity 侧全量重跑确认；但代码层与测试层已经完整落地。
