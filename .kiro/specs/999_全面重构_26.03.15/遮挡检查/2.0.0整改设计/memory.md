# 2.0.0 整改设计 memory

## 2026-03-21｜首个 checkpoint 落地
- 当前主线目标：把 `遮挡检查` 从审计基线推进到可执行整改设计，并尽量完成一个围绕树双组件粒度的首个最小 checkpoint。
- 本轮子任务：创建 `2.0.0整改设计` 四件套，并在允许路径内完成低风险的代码侧收敛。
- 已完成：
  - 新建 `requirements.md`、`design.md`、`tasks.md`、`memory.md`
  - 完成 `BatchAddOcclusionComponents.cs` 对齐：
    - 不再写已删除的 `affectChildren`
    - 不再写已删除的 `occlusionTags`
    - 仅写当前仍存在的 `occludedAlpha`、`fadeSpeed`、`canBeOccluded`
    - 用 helper 包装 `FindProperty`
- 关键判断：
  - 该 checkpoint 已经把“过时工具继续写坏对象”的入口先收窄。
  - 但树双组件主问题本身仍未在主链里被真正修复。
- 恢复点 / 下一步：
  - 优先比较 `TreeController` 联动父/子组件 与 `OcclusionSystemTests` 补链 两个方向，决定下一轮最小切口。
