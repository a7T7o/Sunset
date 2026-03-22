# 导航检查 2.0.0 整改设计记忆

## 会话 1 - 2026-03-21

- 当前主线目标：把导航线程从 `1.0.0初步检查` 过渡到 `2.0.0整改设计` 的可继续整改状态。
- 本轮子任务：按 batch04 的最小 docs-first checkpoint，在 `codex/navigation-audit-001` 上固化 `2.0.0整改设计` 目录。
- 已完成：
  - 在 shared root 上完成 live preflight。
  - 执行 `request-branch` 并获批 `codex/navigation-audit-001`。
  - 执行 `ensure-branch` 进入任务分支。
  - 新建 `2.0.0整改设计` 四件套。
- 已确认事实：
  - 高风险热文件仍是 `GameInputManager.cs` 与 `Primary.unity`。
  - 首个整改 checkpoint 仍应优先从 `NavGrid2D`、`PlayerAutoNavigator` 这类非热文件推进。
  - 本轮未进入 Unity / MCP / Play Mode。
- 恢复点：
  - 下一轮若继续整改，应基于本目录四件套进入首个非热文件代码 checkpoint。

## 会话 2 - 2026-03-22

- 当前主线目标：停止继续停留在 docs-first，把 `导航检查` 推进到 `main` 上的首个真实代码 checkpoint。
- 本轮子任务：
  - 先处理 `codex/navigation-audit-001` 的旧分支遗留；
  - 再直接进入 `NavGrid2D.cs` / `PlayerAutoNavigator.cs` 的非热文件整改。
- 已完成：
  - 仅保留旧分支里仍有价值的 `2.0.0整改设计` 四件套，并迁回 `main`。
  - 将旧分支上的根层 docs-first 垫片判定为已作废口径，不再继续作为 blocker。
  - 在 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavGrid2D.cs` 中，把 `IsPointBlocked()` 的分配式 `OverlapCircleAll(...)` 改为复用缓冲区查询。
  - 在 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs` 中：
    - 让玩家导航实现 `INavigationUnit`
    - 增加动态导航障碍识别
    - 增加移动 NPC / 动态导航单元的侧向绕行
    - 增加持续阻挡时的网格刷新与路径重建入口
    - 把前瞻碰撞检测改为复用缓冲区查询
- 关键结论：
  - 旧口径“`codex/navigation-audit-001` 里还有成果，所以现在不能继续”已作废。
  - 本轮已经形成 `main` 上的首个真实导航代码 checkpoint，而不是继续停在纯设计阶段。
- 风险与恢复点：
  - 本轮未触碰 `GameInputManager.cs`、`Primary.unity`，也未进入 Unity / MCP live 写。
  - 下一轮优先做 Unity / MCP 或人工运行验证，确认“玩家自动导航绕移动 NPC”在真实场景里成立。
