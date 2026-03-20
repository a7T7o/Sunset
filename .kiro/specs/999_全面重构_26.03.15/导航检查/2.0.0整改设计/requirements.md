# 导航检查 2.0.0 整改设计需求

## 目标

把 `1.0.0初步检查` 已确认的问题，收束成一个可继续推进的整改阶段，并明确首个 checkpoint 仍然优先走非热文件、非 Unity 的低冲突路径。

## 已确认的 live 事实

1. 当前导航主链仍是 `GameInputManager -> PlayerAutoNavigator -> NavGrid2D`。
2. `PlacementNavigator` 由 `PlacementManager.Start()` 在运行时创建，不是场景静态挂载。
3. `NavGrid2D.IsPointBlocked()` 仍使用 `Physics2D.OverlapCircleAll(...)`。
4. `PlayerAutoNavigator` 仍存在 `Physics2D.OverlapCircleAll(...)` 查询路径。
5. `GameInputManager` 仍存在 `Physics2D.OverlapPointAll(...)` 命中筛选路径。
6. `ChestController`、`TreeController` 仍直接触发 `NavGrid2D.OnRequestGridRefresh`。
7. 当前稳定语义仍应保留 `ClosestPoint` 接近判定与 `IInteractable` 契约。

## 整改要求

### 要求 1：先把高频查询问题收束为可控整改面

1. 导航相关高频物理查询必须先从“分散调用”收束成可审计的整改面。
2. 首个代码 checkpoint 只能优先落在非热文件：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavGrid2D.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
3. 在没有明确热文件准入前，不直接进入：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`

### 要求 2：刷新链整改必须先明确调度边界

1. `ChestController`、`TreeController` 直接发刷新请求的现状需要先被文档化。
2. 后续整改应优先设计统一刷新调度入口，而不是继续在业务模块内各自补延迟逻辑。

### 要求 3：整改不得破坏当前已成立语义

1. `ClosestPoint` 仍是接近判定的稳定基线。
2. `IInteractable` 仍是交互主契约。
3. 任何整改都不能把已成立的 live 语义回退为旧文档中的过时描述。

### 要求 4：首个 checkpoint 必须是低冲突、可快速归还 shared root 的事务

1. 本轮先固化 `2.0.0整改设计` 目录。
2. 本轮不进入 Unity / MCP / Play Mode。
3. 本轮不触碰 `Primary.unity` 与 `GameInputManager.cs`。

## 非目标

1. 本阶段不是一次性重写整个导航系统。
2. 本阶段不宣称已经完成 Zero GC。
3. 本阶段不在没有独立准入的前提下碰热文件。
