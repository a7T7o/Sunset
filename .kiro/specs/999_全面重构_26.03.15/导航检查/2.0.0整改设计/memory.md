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
