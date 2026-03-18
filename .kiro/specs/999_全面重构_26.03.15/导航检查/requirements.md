# 导航检查 2.0.0 整改需求

## 简介

本阶段不是继续扩写 `1.0.0` 审计结论，而是把已经确认的 live 问题收束成一份可执行的整改需求。  
当前导航系统已经具备稳定可用的业务闭环，但仍存在三类会持续放大成本的问题：

1. 高频物理查询仍在产生数组分配，`Zero GC` 结论不成立。
2. 动态障碍物刷新仍缺少跨模块的统一调度。
3. 输入桥接、执行器和网格层的边界还不够清晰，后续很容易在热文件里越改越重。

本阶段的目标是为后续真实整改建立统一边界，而不是在 `main` 上直接进入代码修补。

## 当前 live 前提

- shared root 默认入口：`D:\Unity\Unity_learning\Sunset @ main`
- 本线程阶段二 continuation branch：`codex/navigation-audit-001`
- 当前审计基线来源：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\1.0.0初步检查\03_现状核实与差异分析.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\1.0.0初步检查\04_审计阶段结案与移交建议.md`

## 术语表

- `导航主链`：`GameInputManager -> PlayerAutoNavigator -> NavGrid2D`
- `放置链`：`PlacementManager -> PlacementNavigator -> PlayerAutoNavigator`
- `刷新链`：树、箱子等动态障碍通过 `NavGrid2D.OnRequestGridRefresh` 触发网格重建
- `ClosestPoint 合同`：当前所有交互接近判定以 Collider/Bounds 的最近点为准
- `A 类热文件`：至少包括 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs` 与 `Assets/000_Scenes/Primary.unity`

## 范围

### 本阶段纳入

1. `NavGrid2D` 的高频阻挡查询与网格重建路径。
2. `PlayerAutoNavigator` 的接近判定、视线优化和路径可达查询。
3. `GameInputManager` 的右键命中筛选与交互桥接逻辑。
4. `ChestController`、`TreeController`、`PlacementNavigator` 与导航主链之间的接口约束。
5. 与导航语义直接冲突的文档漂移，例如 `chest-interaction.md`。

### 本阶段不纳入

1. 多层级导航与桥上桥下系统。
2. 真实 Unity 场景 / Prefab / Play Mode 验收。
3. `CloudShadowManager` 的表现层优化。
4. NPC 自动移动新能力。
5. 与导航无直接关系的全局治理扩写。

## 需求

### 需求 1：高频查询必须从“会分配”收敛到“可控非分配”

**用户故事**：作为开发者，我希望导航主链在高频路径上不再依赖 `OverlapCircleAll` / `OverlapPointAll` 的分配式查询，以避免移动、寻路和点击交互中的持续 GC 抖动。

#### 验收标准

1. WHEN 导航主链执行高频阻挡探测时 THEN 系统 SHALL 提供基于缓存数组的非分配查询路径。
2. WHEN `NavGrid2D` 检查某个格点是否阻挡时 THEN 系统 SHALL 不再把 `OverlapCircleAll` 当作默认路径。
3. WHEN `PlayerAutoNavigator` 做近身探测或局部可达性判断时 THEN 系统 SHALL 优先复用统一的查询辅助逻辑，而不是各处散落独立分配调用。
4. WHEN `GameInputManager` 做右键命中筛选时 THEN 系统 SHALL 明确哪些查询保留分配式，哪些必须迁到非分配路径，并在代码结构上可辨识。
5. WHEN 完成首轮整改后 THEN 文档与实现 SHALL 不再继续宣称当前 live 是“Zero GC 已完成”，除非已有实证支持。

### 需求 2：刷新链必须从“模块内自保”升级为“系统级调度”

**用户故事**：作为开发者，我希望树木、箱子和后续动态障碍对导航网格的刷新请求能够被统一合并，而不是各模块各自延迟、各自触发。

#### 验收标准

1. WHEN 多个动态障碍在短时间内变化时 THEN 系统 SHALL 提供统一的刷新调度入口。
2. WHEN `ChestController` 与 `TreeController` 请求网格刷新时 THEN 系统 SHALL 可以被调度层合并，而不是直接形成重建风暴。
3. WHEN 刷新策略被设计完成时 THEN 文档 SHALL 说清“谁发请求、谁合并、谁真正执行重建”。
4. WHEN 后续新增动态障碍类型时 THEN 系统 SHALL 不要求每个模块都重复发明自己的延迟节流方案。

### 需求 3：当前交互语义必须保持稳定，不允许整改时回退

**用户故事**：作为玩家，我希望整改后仍然保持现在已经成立的交互体验，而不是把 `ClosestPoint`、`IInteractable` 等现有稳定语义在重构中改坏。

#### 验收标准

1. WHEN 导航接近可交互目标时 THEN 系统 SHALL 继续以 `ClosestPoint` / 最近边缘距离作为接近判定主语义。
2. WHEN `IInteractable` 已经承接统一交互契约时 THEN 系统 SHALL 不把业务类型判断重新塞回输入层硬编码。
3. WHEN `PlacementNavigator` 仍以运行时创建为前提时 THEN 新设计 SHALL 尊重这个事实，不把它误当成场景静态节点。
4. WHEN 文档与实现存在漂移时 THEN 后续整改 SHALL 优先把文档拉回 live 语义，而不是把代码回退到旧文档。

### 需求 4：整改顺序必须优先避开不必要的热文件冲突

**用户故事**：作为协作者，我希望导航整改先走低冲突路径，把热文件和 Unity 热区留到确有必要时再碰，以降低并发风险。

#### 验收标准

1. WHEN 阶段 2.0.0 刚开始时 THEN 首个 checkpoint SHALL 先固化文档，不直接进入 Unity / Play Mode。
2. WHEN 首轮代码整改可以先落在 `NavGrid2D` / `PlayerAutoNavigator` 时 THEN 不应无必要抢先改 `GameInputManager.cs`。
3. WHEN 确实需要修改 `GameInputManager.cs` 或 `Primary.unity` 时 THEN 方案文档 SHALL 显式标注热文件风险与准入前置条件。
4. WHEN 阶段 checkpoint 完成时 THEN shared root SHALL 能被归还，不把本线程长期留在共享现场。

## 非目标

1. 本阶段不承诺“一次性把导航系统彻底重写”。
2. 本阶段不承诺立即消灭所有分配点。
3. 本阶段不承诺同时解决遮挡、NPC、自定义表现层的全部历史问题。
4. 本阶段不承诺在没有额外验证前宣布性能提升已经被 Unity Profiler 证实。

## 当前判断

`2.0.0` 的第一步应该是“立约束、定顺序、压边界”，而不是直接扑向 `GameInputManager` 大拆分。  
先把高频查询、刷新调度、当前稳定语义和热文件策略钉清，后续代码整改才有可持续的落点。
