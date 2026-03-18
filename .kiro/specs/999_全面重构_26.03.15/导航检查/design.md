# 导航检查 2.0.0 整改设计

## 设计目标

把 `1.0.0` 审计里已经确认的事实转成可执行整改方案，并明确第一批实现该从哪里下手、暂时不该碰什么。

本设计不追求“一步到位重写导航”，而是追求三件事：

1. 先把最会持续制造成本的点收敛。
2. 保住当前已经成立的交互语义。
3. 把热文件和 Unity 热区的接触时机后移。

## 设计输入

### live 现状

- `NavGrid2D.cs` 当前 723 行，`IsPointBlocked()` 仍用 `Physics2D.OverlapCircleAll(...)`
- `PlayerAutoNavigator.cs` 当前 1014 行，局部探测和近障分析仍有 `OverlapCircleAll(...)`
- `GameInputManager.cs` 当前 2574 行，右键命中与部分交互检测仍用 `OverlapPointAll(...)`
- `PlacementNavigator.cs` 已统一到 `ClosestPoint`
- `ChestController.cs`、`TreeController.cs` 仍直接发 `NavGrid2D.OnRequestGridRefresh?.Invoke()`

### 原始需求输入

从 `001_BeFore_26.1.21/导航系统重构/requirements.md` 回收的长期目标里，有四条仍对当前 live 直接有效：

1. 高频导航查询应向非分配路径收敛。
2. 视线与局部可达性判断应统一，不要散落多套判定。
3. 输入层与交互层应该继续解耦，而不是重新硬编码。
4. 动态障碍刷新需要系统级合并，而不是模块内补丁。

从 `001_BeFore_26.1.21/遮挡与导航/memory.md` 回收的经验里，有两条应作为反例保留：

1. 不要为了短期修感受而继续堆更多“局部补丁式导航技巧”。
2. 不要在没有重新核实 live 代码的前提下，沿用旧版“已经 Zero GC / 已经最简最优”的表述。

## 问题分层

### 层 1：查询分配问题

这是当前最确定、最可直接度量的问题。

- `NavGrid2D` 的格点阻挡探测仍在分配。
- `PlayerAutoNavigator` 的局部探测仍在分配。
- `GameInputManager` 的右键命中筛选仍在分配。

这里的问题不是“完全没有缓存”，而是：

- 查询 API 本身仍返回新数组。
- 调用点分散在三层代码里。
- 现有文档和旧工作区里仍留有“Zero GC 已成立”的历史叙述。

### 层 2：刷新调度问题

当前刷新链缺的是“系统调度层”，而不是“单模块再多加一个延迟”。

现状：

- `TreeController` 有自己的延迟刷新节流。
- `ChestController` 在自身状态变化时直接请求刷新。
- `NavGrid2D` 只有静态事件入口，没有统一合并器。

结果：

- 单对象内部问题被局部缓解。
- 多对象并发变化时仍可能形成重建风暴。

### 层 3：桥接层膨胀问题

`GameInputManager` 现在并不只是“代码长”，而是承担了太多不同层级的责任：

- 点击命中筛选
- 交互桥接
- 农田状态机
- 一部分导航进入逻辑

这里不应该直接做全面拆分，而应先收一个更小的目标：

- 把导航相关查询与接近判定从输入逻辑里抽成可替换的桥接层。

### 层 4：语义保护问题

当前 live 已有一些不该被回退的稳定语义：

- `ClosestPoint` 接近判定
- `IInteractable` 契约
- `PlacementNavigator` 运行时创建
- `CloudShadowManager` 风险是潜在态，不是当前整改主线

2.0.0 必须把这些写进方案，否则后续实现容易越改越偏。

## 设计策略

## 方案总原则

1. 先收高频查询，再碰桥接层。
2. 先补统一调度设计，再决定是否拆模块。
3. 先做 docs-first checkpoint，再做 code-first checkpoint。
4. 尽量把首批代码改动压在非热文件上。

### 方案 A：查询辅助层先行

在 `NavGrid2D` 附近建立统一的非分配查询辅助能力，供网格层与执行器层共用。

目标：

- 把“阻挡探测”“局部障碍采样”“近距离可达性判断”的共性接口收束。
- 减少 `PlayerAutoNavigator` 自己直接做 `OverlapCircleAll(...)` 的需求。

首批收益：

- `NavGrid2D` 与 `PlayerAutoNavigator` 可先整改，不必立即碰 A 类热文件 `GameInputManager.cs`。

### 方案 B：刷新入口改为调度器

不在 `TreeController`、`ChestController` 里继续发明各自节流，而是在导航域里增加一个统一调度器。

建议职责：

- 接收刷新请求
- 合并相邻时间窗内的请求
- 决定本帧 / 下一帧 / 延迟窗口谁真正执行 `RebuildGrid`

建议先做文档设计，再决定实现落点究竟在 `NavGrid2D` 内部还是独立服务对象。

### 方案 C：输入层只保留桥接，不再保留查询细节

`GameInputManager` 的整改不应从“大拆分”起步，而应从“导航相关查询细节下沉”起步。

目标：

- 输入层继续判断“玩家点了什么、当前要走交互还是纯导航”
- 具体的距离计算、最近点语义、命中筛选细节，尽量收成专门辅助层

这样做的好处：

- 可以在不改变当前交互行为的前提下，把后续改动面压缩。
- 即便未来确实要碰 `GameInputManager.cs`，改动也更聚焦。

## 推荐实现顺序

### checkpoint 1：docs-first

本次已完成或将完成：

1. `requirements.md`
2. `design.md`
3. `tasks.md`

目的：

- 把整改边界钉清。
- 让后续代码修改有明确顺序和非目标。

### checkpoint 2：non-alloc 基础层

优先改：

1. `NavGrid2D`
2. `PlayerAutoNavigator`

暂不优先改：

1. `GameInputManager`
2. `Primary.unity`

理由：

- 可以先避开 A 类热文件。
- 可以先验证“导航主执行链”能否用统一查询辅助层收敛分配问题。

### checkpoint 3：输入桥接层

在 checkpoint 2 稳住之后，再处理：

1. `GameInputManager` 的点击命中筛选
2. 交互桥接的查询抽取

此时需要显式评估：

- 是否触发 A 类锁
- 是否需要并发排期

### checkpoint 4：刷新调度器 + 文档回写

最后处理：

1. 刷新调度器真实落地
2. `chest-interaction.md` 等文档回写
3. 如需 Unity 验证，再单独申请热区操作

## 热文件与风险

### 当前高风险对象

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`

### 当前低风险对象

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavGrid2D.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementNavigator.cs`

### 本阶段处理策略

1. 本 checkpoint 只写文档，不碰任何热文件。
2. 下个 code checkpoint 尽量先从低风险对象开始。
3. 只有当输入层桥接确实成为阻断点时，才申请 `GameInputManager.cs` 的热文件准入。

## 不采纳的路线

### 不采纳 1：直接全面拆 `GameInputManager`

原因：

- 热文件冲突高。
- 现阶段缺少更细的桥接边界。
- 容易把当前已经稳定的交互语义一起打散。

### 不采纳 2：直接宣布“重新做完整导航系统”

原因：

- 当前 live 系统已经有大量稳定业务接入。
- 这是整改，不是重开新系统。
- 风险与收益严重失衡。

### 不采纳 3：先碰 Unity 场景验证

原因：

- 当前最小 checkpoint 是方案固化，不是验收。
- Unity / Play Mode / 场景改动会把本轮从低风险文档 checkpoint 变成高风险现场。

## 结论

`2.0.0` 的第一步不是“写更多技术幻想”，而是：

- 把高频查询、刷新调度、桥接层边界和热文件策略写清楚；
- 然后按 `NavGrid2D -> PlayerAutoNavigator -> GameInputManager` 的顺序进入真实代码整改；
- 始终保住 `ClosestPoint` 与 `IInteractable` 这两条当前 live 的稳定合同。
