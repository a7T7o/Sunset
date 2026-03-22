# 导航检查 2.0.0 整改设计

## 设计定位

`2.0.0整改设计` 不是继续扩写 `1.0.0` 审计，而是把已经确认的 live 问题转成一个可执行整改入口，并约束首个实现 checkpoint 的风险边界。

## 设计结论

### 1. 整改顺序

推荐顺序固定为：

1. `NavGrid2D`
2. `PlayerAutoNavigator`
3. `GameInputManager`

这样做的原因是：

1. 先收束主执行链里的高频查询问题。
2. 尽量把首个代码 checkpoint 压在非热文件。
3. 在没有必要前，不抢占共享热文件。

### 2. 查询问题先收“入口”，不先做大拆分

当前问题不是单一 API，而是高频查询分散在多个层级里：

1. 网格阻挡检测在 `NavGrid2D`。
2. 执行器局部检测在 `PlayerAutoNavigator`。
3. 输入命中筛选在 `GameInputManager`。

因此首轮整改应该先把导航查询能力收束成更清晰的边界，再决定是否继续把输入层细节下沉。

### 3. 刷新问题先收“调度”，不先堆局部补丁

`ChestController` 与 `TreeController` 目前都能直接触发网格刷新请求。  
这说明问题核心不是“某个模块没延迟”，而是缺统一调度层。

因此本阶段把“统一刷新调度”确定为后续整改主线之一，但不在本次 docs-first checkpoint 里直接落代码。

### 4. 语义保护必须写进方案

后续实现必须持续保护以下事实：

1. `ClosestPoint` 是当前接近判定基线。
2. `IInteractable` 是当前交互契约基线。
3. `PlacementNavigator` 的运行时创建事实不能被误写成静态场景结构。

### 5. 热文件策略

当前明确视为本线整改热区的文件：

1. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
2. `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`

本轮 checkpoint 不触碰以上文件。  
如果后续必须进入，需在独立准入下单独申请。

## 本轮 checkpoint 定义

本轮完成条件是：

1. 建立 `2.0.0整改设计` 目录。
2. 固化 `requirements.md`、`design.md`、`tasks.md`、`memory.md`。
3. 把本次 batch04 的持槽动作收口到文档基线，不扩展到 Unity 或热文件。
