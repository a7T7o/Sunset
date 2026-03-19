# Codex规则落地线程记忆补卷 3

## 会话 39 - 2026-03-19（修复模式执行完毕，进入治理文档收口）
**用户目标**：
> 不要只停留在事故复盘，要把这轮 `queue-aware` 事故真的修完，并把 shared root 拉回可继续开发的现场。

**已完成事项**：
1. 修复稳定 launcher：
   - 文件：`C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1`
   - 修掉 `New-ForwardArgumentList` 对 `-BranchName / -CheckpointHint / -QueueNote` 的转发缺口。
2. 修复 `main` 上 canonical queue handoff：
   - 文件：`D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
   - 修正 `Repair-SharedRootQueueRecord`，避免合法的 `granted -> task-active` 过渡被误杀成 `cancelled`
   - 修复提交：`98cbb88b fix: preserve queue ticket on task handoff`
3. 完成 `NPC` 营救：
   - 先把 foreign dirty 停放到 `stash@{0}`：`2026-03-19 queue-aware rescue foreign-dirty park`
   - 再用稳定 launcher从 `main` canonical 脚本对 `NPC` 执行白名单 `task sync`
   - `NPC` continuation checkpoint 已形成：`codex/npc-roam-phase2-003 @ 86bc5f38`
   - 随后成功 `return-main`
4. shared root 物理基线已回正：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - occupancy 已恢复 `main + neutral`
   - queue runtime 已重置为空队列

**关键决策**：
- 这轮阻塞已经从“物理闸机损坏”切换成“治理文档层尚未收口”。
- 不再继续发新业务线程；当前优先级是把 stash 回放后留下的 memory 冲突和新增补卷安全提交到 `main`。

**恢复点 / 下一步**：
- 先解决 4 个 `UU` memory 文件并补完修复模式记忆。
- 再执行一次 `governance sync`，把本轮事故修复与现场回正正式入库。
