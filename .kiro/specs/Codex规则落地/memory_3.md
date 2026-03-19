# Codex规则落地 memory_3

## 2026-03-19：阶段 23 修复模式已完成物理回正
- 当前主线目标：让阶段 `23` 不只会审计 `queue-aware` 事故，还要把 shared root 重新拉回可继续开发的 live 基线。
- 本轮子任务：直接进入修复模式，完成 launcher 参数透传修复、canonical queue handoff 修复、`NPC` 营救与 shared root 回正。
- 已完成事项：
  1. 稳定 launcher `C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1` 已修复 optional 参数透传缺口，`-BranchName` 不再被吞。
  2. `main` 上的 canonical `scripts\git-safe-sync.ps1` 已修复 queue `granted -> task-active` 交接误杀问题，并形成提交：
     - `98cbb88b fix: preserve queue ticket on task handoff`
  3. `NPC` 卡死现场已被安全营救：
     - `codex/npc-roam-phase2-003` 已完成 continuation checkpoint：`86bc5f38`
     - shared root 已成功 `return-main`
  4. shared root 基线已补正为：
     - `D:\Unity\Unity_learning\Sunset @ main`
     - occupancy 为 `main + neutral`
     - queue runtime 为空队列
- 关键决策：
  - 阶段 `23` 当前不再卡在“是否需要修”，而是已经进入“修完后如何治理收口”的最后一段。
  - 当前不应重新发起业务线程准入，先完成治理文档冲突收口与主线同步。
- 恢复点 / 下一步：
  1. 收口 stash 回放后的治理 memory 冲突。
  2. 用 `governance sync` 把本轮修复模式文档、回正事实和新增补卷提交到 `main`。
  3. 提交完成后再基于 live clean 事实决定下一轮业务分发。
