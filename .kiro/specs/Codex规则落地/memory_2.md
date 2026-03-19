# Codex规则落地 memory_2

## 2026-03-19：阶段 23 批次 `queue-aware业务准入_01` 事故裁定
- 当前主线目标：在阶段 23 内把 shared root 的并发交通从“会撞死的准入”修到“可排队、可收口、可恢复”的真实工程状态。
- 本轮子任务：对 `2026.03.19_queue-aware业务准入_01` 的回执与 live 现场做只读审计，判断这轮失败到底来自哪里。
- 已核实结论：
  1. 这轮不是业务线程违规，而是治理基建先出故障。
  2. stable launcher 存在 `-BranchName` 等参数未稳定转发的问题，直接导致 `request-branch` 在 `NPC`、`遮挡检查` 两条线上先报假性阻断。
  3. canonical queue 的 repair 逻辑会在 `grant -> task-active` 交接期误把 granted 条目改成 `cancelled`，这正是当前 `NPC ticket=1` 与 occupancy 分裂的根因。
  4. `NPC` 持锁后没有违规写业务，但 shared root 上随后叠加的治理 dirty 不在 NPC 的 task sync 白名单里，导致它无法 `sync / return-main`，shared root 被合法占住却无法收口。
  5. 当前 live branch 上的仓库脚本还是旧版，而 launcher 转发的是 `main` 上的新 queue-aware 脚本，版本漂移显著放大了现场理解成本。
- 关键决策：
  - 阶段 23 不能直接宣布“并发交通已闭环”。
  - 现阶段必须先把 launcher、queue handoff、slot holder 收口隔离三件事修完，再考虑继续分发新 batch。
- 关联记录：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\事故复盘.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\memory_2.md`
- 恢复点 / 下一步：
  - 下一步若进入修复模式，优先顺序固定为：
    1. 修 `C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1`
    2. 修 `main:scripts/git-safe-sync.ps1` 的 queue handoff
    3. 设计并落实 slot holder 收口隔离 / rescue runbook
    4. 回正 shared root 与 queue runtime 后，才允许重开业务准入
