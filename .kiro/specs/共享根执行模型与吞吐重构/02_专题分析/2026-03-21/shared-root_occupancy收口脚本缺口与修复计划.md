# shared root occupancy 收口脚本缺口与修复计划

## 1. 这份文档解决什么问题
- 把 2026-03-20 `NPC batch04` 收口时暴露出的 shared root 收口故障正式定性。
- 区分两类问题：
  - 旧的业务交付验收问题
  - 新暴露的治理脚本 / 状态机问题
- 为后续修 `scripts/git-safe-sync.ps1` 提供明确目标，而不是继续让业务线程自己试错。

## 2. 已确认的 live 事实
- `NPC` 本轮最终已完成：
  - docs-only checkpoint 提交：`b680cd4b`
  - `sync = 成功`
  - `return-main = 成功`
- 当前 shared root 已回到：
  - `D:\Unity\Unity_learning\Sunset @ main`
  - `git status --short --branch = clean`
  - `shared-root-branch-occupancy.md = neutral-main-ready`
- 当前场景搭建线程已停留在：
  - worktree：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
  - branch：`codex/scene-build-5.0.0-001`

## 3. 这次到底出了什么问题
### 3.1 不是业务本身没做完
- `NPC` 在本轮已经完成了 `main-ready` 判断，并产出了正式文档：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\2026-03-20_phase2_main-ready核验.md`
- 问题不在“它有没有业务结论”，而在“它做完后没能顺利退场”。

### 3.2 真正故障点
- `ensure-branch` 执行过程中出现过系统级异常。
- 但从 live 现场可以确认：
  - 仓库实际上已经切进 `codex/npc-roam-phase2-003`
  - 也就是说，这一步是“半成功”
- 脚本设计上，切入任务分支后应立即把 occupancy 改成：
  - `owner_mode = task-branch-active`
  - `owner_thread = <线程>`
  - `current_branch = <任务分支>`
  - `lease_state = task-active`
- 但异常发生后，状态没有可靠落成，于是：
  - `sync` 还在按旧 occupancy 校验
  - `return-main` 也继续按旧 occupancy / 未提交脏改双重校验
- 结果形成“分支已进、checkpoint 已写、但无法正常提交并归还”的死锁。

## 4. 责任怎么拆
### 4.1 旧问题：NPC 资产缺失 / main 依赖 branch-only
- 性质：业务交付验收问题。
- 责任：
  - `NPC` 线程负执行责任。
  - 治理线程负验收责任。
- 根因：以前只验 `carrier-ready`，没有把 `main-ready` 设成硬闸门。

### 4.2 新问题：batch04 中一度无法 `sync / return-main`
- 性质：治理脚本 / shared root 收口状态机问题。
- 主责：治理 / 工具实现层。
- 次责：业务线程在系统异常后不应继续深挖 PATH / ACL / restore 等环境问题，而应立即冻结并交回治理线程。
- `farm` 对这次收口死锁无责，因为它始终处于 waiting，未进入执行面。

## 5. 风险等级
- 风险级别：高。
- 原因不是它破坏了当前 `main`，而是它暴露出一条危险链：
  - 分支已经切入
  - occupancy 却没同步
  - 后续所有收口动作都可能被旧态误拦
- 如果不修，会反复造成三层误判：
  - 线程误以为“业务没做对”
  - 用户误以为“又是某个线程乱搞”
  - 实际却是 shared root 状态机断链

## 6. 修复目标
### 6.1 脚本层
- 补齐 `ensure-branch` 异常后的状态一致性：
  - 要么完全失败且不切分支
  - 要么一旦切入成功，就必须可靠落成 `task-active`
- 给 `sync / return-main` 增加“半成功残留态”的识别与兜底。
- 禁止线程在系统异常后继续自行做环境手术。

### 6.2 流程层
- prompt 模板补入明确红线：
  - 系统级异常即冻结
  - 禁止自行排查 PATH / ACL / restore
  - 立即回报治理线程
- docs-only fallback 需要更早出现，而不是等线程自己绕很久才退回最小收口。

### 6.3 runbook 层
- 增加一份治理接管 runbook：
  - 如何识别“分支已进但 occupancy 未落”
  - 如何由治理线程最小修正 occupancy
  - 如何只允许继续 `sync -> return-main`

## 7. 当前并行边界结论
### 7.1 worktree 能解决什么
- 能解决 Git / 文档 / WIP 隔离问题。
- 场景搭建线程现在留在自己的 `branch + worktree`，不会再污染 shared root。

### 7.2 worktree 不能解决什么
- 不能自动解决 Unity / MCP 冲突。
- 当前 Sunset 规则仍然是：
  - `single-instance-shared-editor`
  - `single-writer-only`
- 所以即使打开的是：
  - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
  也仍然只能理解为：
  - Git 现场隔离了
  - Unity / MCP 写入层仍要单写者占用

## 8. 接下来的执行顺序
1. 业务推进优先：
   - 先继续 `农田交互修复V2`
2. 治理修复并行准备：
   - 以本文为入口修 `scripts/git-safe-sync.ps1`
3. 第二波业务继续前再判断：
   - `导航`
   - `遮挡`
4. `spring-day1` 继续保持独立集成波次，不并入当前短事务 shared root 波次。

## 9. 一句话结论
- 这次真正新增的问题不是“NPC 又没把业务做好”，而是“shared root 收口脚本在异常下没有把 occupancy 状态机收完整”；worktree 已经解决了场景搭建的 Git 污染问题，但没有豁免 Unity / MCP 单写者规则。

## 10. 2026-03-21 补充：本轮已落地的脚本修复
- 已在 [git-safe-sync.ps1](/D:/Unity/Unity_learning/Sunset/scripts/git-safe-sync.ps1) 补上 active session 的兼容修复：
  - `Set-SharedRootActiveSession` 新建 runtime 时显式写入 `last_reason`
  - `Get-SharedRootActiveSession` 读取旧 runtime 时会自动补齐缺失字段，并回写修复后的 JSON
  - `Touch-SharedRootActiveSession` 改为“缺属性则补、存在则改”，不再直接对旧结构赋值
- 这次修复的直接目标不是放宽闸门，而是让旧 runtime / 半旧 runtime 不会再把 `sync` 尾部和 `return-main` 收口炸掉。
- `dirty` 分级与放宽机制仍停留在任务 15；它是后续设计题，不在这次 bugfix 里顺手混改。
