# 2.0.0 整改设计

## 设计原则
- 先修“会污染后续判断的错误入口”，再修“真正的主链逻辑”。
- 先收敛对象粒度问题，再谈树林判定与命中统一。
- docs-first，但本轮必须附带一个最小代码 checkpoint。

## 本轮 checkpoint 选择
- 选择“过时批处理工具与当前组件定义对齐”。

## 选择原因
- 它位于允许路径内，改动面小，风险低。
- 它直接命中审计里已确认的真实漂移：工具仍在写旧字段。
- 如果不先修这个入口，后续即使设计清楚树双组件方案，也可能继续被旧工具重新写坏。

## 实施方案
1. 保留批处理工具现有职责：
   - 扫描选中父物体
   - 补齐 Rigidbody2D / CompositeCollider2D / OcclusionTransparency
   - 清理父节点 SpriteRenderer
2. 删除对旧字段的写入：
   - `affectChildren`
   - `occlusionTags`
3. 将序列化赋值缩到当前组件仍存在的字段：
   - `occludedAlpha`
   - `fadeSpeed`
   - `canBeOccluded`
4. 使用小型 helper 包住 `SerializedObject.FindProperty`，避免今后字段继续变动时再次出现空引用写入。

## 后续递进
- 下一步优先比较两个方向：
  - `TreeController.SetCanBeOccluded()` 是否应联动父/子两份组件
  - `OcclusionSystemTests` 是否先补一条“单树双组件误入森林逻辑”的最小测试

## 风险
- 本轮没有直接修复主链误判，只是先收紧错误入口。
- 工具对齐后，仍需要真正的树双组件粒度方案来闭环主问题。
