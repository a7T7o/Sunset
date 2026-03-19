# 2026.03.19 queue-aware业务准入 01 memory_3

## 2026-03-19：修复模式执行结果与现场回正
- 当前主线目标：把 `2026.03.19_queue-aware业务准入_01` 从事故审计推进到“物理修复完成、shared root 已回正、只剩治理文档收口”的可恢复状态。
- 本轮子任务：执行修复模式，修 stable launcher、修 canonical queue handoff、营救被卡在 `NPC` 分支的 shared root，并把 occupancy / queue runtime 拉回 neutral baseline。
- 已完成事项：
  1. 修复稳定 launcher：
     - 文件：`C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1`
     - 修掉 `New-ForwardArgumentList` 对 `-BranchName / -CheckpointHint / -QueueNote` 的透传缺口。
     - 修正 `ScopeRoots / IncludePaths` 转发方式，并完成语法校验。
  2. 修复 canonical queue handoff：
     - 文件：`D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
     - 修正 `Repair-SharedRootQueueRecord`，当 occupancy 已经是同 owner / 同 branch 的 `task-active` 时，不再把 queue 中的 `granted` 票据误杀成 `cancelled`。
     - 修复已提交到 `main`：`98cbb88b fix: preserve queue ticket on task handoff`
  3. 完成 `NPC` 营救：
     - 先将 foreign governance dirty 定向停放到 `stash@{0}`：`2026-03-19 queue-aware rescue foreign-dirty park`
     - 再用稳定 launcher 从 `main` 的 canonical 脚本对白名单文件执行 `task sync`
     - `NPC` continuation checkpoint 成功形成：`codex/npc-roam-phase2-003 @ 86bc5f38`
     - 随后成功执行 `return-main`
  4. 手工回正 runtime：
     - `shared-root-branch-occupancy.md` 已恢复 `main + neutral`
     - `last_verified_head = 98cbb88b`
     - `active/shared-root-queue.lock.json` 已重置为空队列，`next_ticket = 1`
- 关键结论：
  - 这轮事故的三处物理根因都已被真实修补，不再停留在“只会复盘”的阶段。
  - `NPC` 已完成最小收口并归还 shared root，不需要再次执行营救动作。
  - 当前剩余阻断已经从物理闸机故障切换为治理文档层的冲突和未入库材料收口。
- 涉及文件：
  - `C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1`
  - `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\active\shared-root-queue.lock.json`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\事故复盘.md`
- 恢复点 / 下一步：
  1. 先把 stash 回放后的 `UU` memory 文件正式标记解决。
  2. 再把本轮修复模式产生的治理文档、补卷与回正事实统一走一次 `governance sync`。
  3. 提交完成后再重新评估是否恢复下一轮 queue-aware 准入分发。
