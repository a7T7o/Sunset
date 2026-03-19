# 2026.03.19 queue-aware业务准入 01 memory_1

## 2026-03-19｜遮挡检查｜异常准入回收补卷
- 当前主线目标：按 queue-aware 准入规则，为 `遮挡检查` 线程申请 continuation branch `codex/occlusion-audit-001`，仅在获 grant 后进入首个低风险 checkpoint。
- 本轮子任务：领取 `可分发Prompt\遮挡检查.md`，执行 `request-branch` 分流，并把结果写回固定回收卡。
- 已验证事实：
  - live preflight 起点为 `D:\Unity\Unity_learning\Sunset @ main @ 9b5279a58128e902770df92f706c6796378de7fc`，`git status --short --branch` 初始为 `## main...origin/main`。
  - stable launcher 两次 `request-branch` 都返回 `request-branch 必须提供 -BranchName`，因此本线程没有成功入队，也没有拿到 `codex/occlusion-audit-001` 的 grant。
  - live 现场在执行期间发生变化：`shared-root-branch-occupancy.md` 于 `2026-03-19 13:44:18 +08:00` 显示 grant 已发给 `NPC -> codex/npc-roam-phase2-003`，`git reflog` 显示 shared root 于 `2026-03-19 13:44:44 +08:00` 被切到该分支。
  - 收尾现场为 `codex/npc-roam-phase2-003 @ 7385d1236d0b85c191caff5c5c19b08678d1cf80`，工作树可见 `M .kiro/locks/shared-root-branch-occupancy.md` 与未跟踪的本子工作区 `memory.md`。
  - 本线程本轮未进入 Unity / MCP / Play Mode，未触碰 `Primary.unity` 与 `GameInputManager.cs`。
- 关键决策：
  - 不执行 `ensure-branch`、不做代码整改 checkpoint、不同步、不执行 `return-main`。
  - 仅落固定回收卡与补卷记忆，等待 shared root 回到 neutral `main` 后再重新申请下一轮准入。
- 恢复点 / 下一步：后续若继续 `遮挡检查`，先重新核 `cwd / branch / HEAD / occupancy / queue`，确认 shared root 已回到 neutral `main` 后，再重新执行 `request-branch`。
