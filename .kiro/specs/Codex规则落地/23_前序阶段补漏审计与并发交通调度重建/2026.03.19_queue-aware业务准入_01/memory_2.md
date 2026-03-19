# 2026.03.19 queue-aware业务准入 01 memory_2

## 2026-03-19：治理线程事故复盘补卷
- 当前主线目标：复盘 `2026.03.19_queue-aware业务准入_01` 为何没有形成可持续的并发准入闭环，判断真实阻断点，并给出恢复前提。
- 本轮子任务：只读核对 live Git / occupancy / queue runtime、三份线程回收卡、stable launcher、`main:scripts/git-safe-sync.ps1` 与当前 live branch 脚本。
- 已核实事实：
  1. 当前 shared root 真实处于 `codex/npc-roam-phase2-003 @ 7385d123`，occupancy 登记 `owner_thread = NPC / lease_state = task-active`。
  2. queue runtime 却把 `NPC ticket=1` 记成 `cancelled`，同时保留 `农田 ticket=2 waiting` 与 `导航 ticket=3 waiting`，说明 queue runtime 与 occupancy 已分裂。
  3. stable launcher `C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1` 的 `New-ForwardArgumentList` 在函数内误用本地 `$PSBoundParameters`，导致 `-BranchName / -CheckpointHint / -QueueNote` 没有稳定转发；这与 `NPC`、`遮挡检查` 回执里的 `request-branch 必须提供 -BranchName` 完全一致。
  4. `main:scripts/git-safe-sync.ps1` 的 queue repair 逻辑会在 `grant -> ensure-branch -> task-active` 过渡期把 granted 条目误降成 `cancelled`；`NPC ticket=1` 当前就是这类状态机错误样本。
  5. `NPC` 线程本轮并未违规写业务，它只完成了最小 continuation checkpoint；真正挡住 `task sync / return-main` 的是 shared root 上后来新增的治理 dirty，不是 NPC 自己的白名单文件。
- 关键结论：
  - 这轮失败不是“线程胡闹”，而是“launcher 参数转发 bug + queue 状态机 bug + slot holder 收口期被 foreign governance dirty 污染”的叠加事故。
  - 当前 shared root 仍被 `NPC` 合法占住，且 queue runtime / occupancy 未回正；在修复前不应继续发新准入。
- 涉及文件：
  - `C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1`
  - `main:scripts/git-safe-sync.ps1`（只读比对）
  - `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\active\shared-root-queue.lock.json`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\线程回收\NPC.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\线程回收\导航检查.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\线程回收\遮挡检查.md`
- 恢复点 / 下一步：
  1. 先修 launcher 参数透传。
  2. 再修 canonical queue repair / task-active handoff。
  3. 再补 slot holder 收口期的治理写入隔离或 rescue runbook。
  4. occupancy 与 queue runtime 回正后，才能重发 batch。
