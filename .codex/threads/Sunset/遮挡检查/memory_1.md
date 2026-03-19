# 遮挡检查 memory_1

## 2026-03-19｜queue-aware准入异常回收
- 用户目标：领取 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\可分发Prompt\遮挡检查.md`，按 prompt 申请 continuation branch `codex/occlusion-audit-001`，并开始写固定回收卡。
- 当前主线目标：在不触碰 Unity / 热区文件的前提下，完成 `遮挡检查` 线程的 queue-aware 准入分流，只有拿到 grant 才进入首个根因 checkpoint。
- 本轮子任务 / 阻塞：执行 stable launcher 的 `request-branch`，但遇到 `request-branch 必须提供 -BranchName` 异常；随后 live shared root 被 `NPC` 消费 grant 并切入 `codex/npc-roam-phase2-003`，导致本线程只能做治理回收，不能继续 branch 流程。
- 已完成事项：
  - 复核 live preflight：初始为 `D:\Unity\Unity_learning\Sunset @ main @ 9b5279a58128e902770df92f706c6796378de7fc`，初始 status 为 clean。
  - 两次调用稳定 launcher 的 `request-branch`，均返回相同参数异常，未形成 `遮挡检查` 自身的 queue ticket / grant。
  - 复核 queue / occupancy / reflog，确认 shared root 于 `2026-03-19 13:44:44 +08:00` 被切到 `codex/npc-roam-phase2-003 @ 7385d1236d0b85c191caff5c5c19b08678d1cf80`，owner 为 `NPC`。
  - 写回固定回收卡：`D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\线程回收\遮挡检查.md`
- 关键决策：
  - 本轮不执行 `ensure-branch`、不进入代码整改、不同步、不执行 `return-main`。
  - 把本轮结论固定为“异常准入回收”，等待 shared root 重新回到 neutral `main` 后再发起下一轮准入。
- 验证结果：
  - 未进入 Unity / MCP / Play Mode。
  - 未触碰 `Primary.unity`、`GameInputManager.cs`。
- 恢复点 / 下一步：后续若继续 `遮挡检查`，先重新核对 shared root 是否已经从 `NPC` 分支归还到 neutral `main`，再重新运行 `request-branch` 申请 `codex/occlusion-audit-001`。
