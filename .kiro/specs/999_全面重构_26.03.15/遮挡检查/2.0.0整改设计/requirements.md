# 2.0.0 整改需求

## 目标
- 让 `遮挡检查` 从“审计基线”进入“可执行整改设计”阶段。
- 围绕“树 Prefab 父/子双 `OcclusionTransparency`”建立第一个最小 checkpoint。
- 优先清理会误导后续整改的过时工具或测试，而不是直接扩大修改 `OcclusionManager` 主链。

## 已知事实
- `Tree/M1.prefab`、`M2.prefab`、`M3.prefab` 都存在父/子双 `OcclusionTransparency`。
- `TreeController` 只控制当前节点上的 `OcclusionTransparency`，父节点那份可能绕过树苗/树桩阶段的禁用逻辑。
- `OcclusionManager` 以 `OcclusionTransparency` 组件而不是“物理树”做注册和树林判定。
- `BatchAddOcclusionComponents.cs` 仍在写已经删除的 `affectChildren` 与 `occlusionTags` 字段。

## 本轮最小需求
- 创建 `2.0.0整改设计` 四件套，固定整改入口。
- 在允许路径内完成一个最小 checkpoint。
- 本轮选择的 checkpoint：
  - 对齐 `Assets/Editor/BatchAddOcclusionComponents.cs` 与当前 `OcclusionTransparency` 定义，停止继续写旧字段。

## 本轮禁止
- 不碰 `Assets/000_Scenes/Primary.unity`
- 不碰 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
- 不进入 Unity / MCP / Play Mode
- 不扩大到大范围重写 `OcclusionManager`

## 完成标准
- `2.0.0整改设计` 四件套存在且能承接后续迭代。
- `BatchAddOcclusionComponents.cs` 不再访问当前组件里不存在的序列化字段。
- `carrier_ready` 能回答本轮 checkpoint 是否已收敛。
- `main_ready` 能回答 return-main 后 `main` 是否仍可继续承接遮挡主入口。
