# 阶段任务：main-branch-only 回归与 worktree 退役收口

## 阶段目标
- 彻底结束 Sunset 当前事故期对临时 `worktree` 的依赖。
- 让项目回到“共享根目录 + 分支”的默认开发模型。
- 达到“用户不需要再为了 `farm / NPC / spring-day1` 的日常开发和验收打开多个 Unity 工程”的可用状态。

## 2026-03-18 迁移说明
- 本阶段保留“历史上曾完成过一次 branch-only 回归与 worktree 退役”的结论。
- 但后续 live 并发现场再次失真，说明该结论没有被持续守住。
- 因此新的“shared root 现场回正 + 防止再次回退”的任务，已迁移到：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\20_shared-root现场回正与物理闸机落地`
- `11` 不再继续承接这轮 live 回正过程，只保留历史收口记录与证据。

## 当前裁定
- `11` 阶段已经完成，不再继续向本阶段追加新尾巴。
- shared root 已恢复为：
  - `D:\Unity\Unity_learning\Sunset @ main`
- 当前 `git worktree list --porcelain` 只剩共享根目录。
- `NPC / farm / spring-day1` 的 branch-only 验证已经真实完成，不再需要继续下发本阶段 prompt。

## 已完成
- [x] 正式立项 `11`，承接 `09/10` 之后全部“回归 branch-only 常态”的剩余工作。
- [x] 锁定用户红线：`worktree` 只允许作为事故隔离例外，不能升级为 Sunset 默认开发流。
- [x] 盘点当前全部已注册 worktree 与其 `branch / HEAD / clean-dirty` 状态。
- [x] 确认 `farm` 合法 branch carrier：
  - `codex/farm-1.0.2-cleanroom001 @ 66c19fa1`
- [x] 确认 `NPC` 合法 continuation branch carrier：
  - `codex/npc-roam-phase2-002 @ 6e2af71b`
- [x] 确认 `spring-day1` clean checkpoint：
  - `codex/spring-day1-story-progression-001 @ a9c952b7`
- [x] 确认 `NPC_roam_phase2_rescue` 中 4 个 TMP 资源 dirty 归属 `spring-day1`，不归属 NPC。
- [x] 明确 `导航检查 / 遮挡检查 / 项目文档总览` 当前都不是 worktree 问题核心，不需要为了这轮 branch-only 回归再新建 worktree。
- [x] 产出本阶段执行方案文档。
- [x] 产出本阶段总进度与收口清单文档。
- [x] 产出可直接下发的 branch-only 回归 prompt 成品。
- [x] 建立线程回包统一汇总位：
  - `所有线程回归誓言.md`（索引）
  - `所有线程回归誓言\*.md`（真实线程回包）
- [x] 产出 shared root 第一版归属图：
  - `共享根目录dirty归属初版_2026-03-17.md`
- [x] 完成第二批待退役容器的最新核验：
  - `第二批worktree核验表_2026-03-17.md`
- [x] 收齐第一轮 branch-only 回包：
  - `spring-day1`
  - `NPC`
  - `农田交互修复V2`
  - `导航检查`
  - `遮挡检查`
  - `项目文档总览`
- [x] 已真实退役第一批纯历史 worktree：
  - `D:\Unity\Unity_learning\Sunset_worktrees\main-reflow-carrier`
  - `D:\Unity\Unity_learning\Sunset_worktrees\NPC`
  - `D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002`
- [x] 已完成 `spring-day1` 二轮裁定与执行：
  - 5 个相关字体资产继续归 `spring-day1`
  - 两处字体 dirty 的证据已导出
  - shared root 与 rescue 的字体 dirty 已全部丢弃，保留已提交版本
- [x] 已裁定并移出：
  - `Assets/Screenshots*`
  - `npc_restore.zip`
- [x] 已将治理收口提交安全落盘到：
  - shared root 首个回 `main` 的干净提交：`763ba4a1`
- [x] 已在 shared root 对以下分支完成 root + branch 检出验证：
  - `codex/spring-day1-story-progression-001`
  - `codex/npc-roam-phase2-002`
  - `codex/farm-1.0.2-cleanroom001`
- [x] 已真实退役第二批 worktree：
  - `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue`
  - `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_continue`
  - `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
  - `D:\Unity\Unity_learning\Sunset_worktrees\spring-day1-story-progression-001`
- [x] 当前 `git worktree list --porcelain` 只剩共享根目录。
- [x] 已生成终局快照：
  - `终局快照_2026-03-17.md`
- [x] 已同步更新本阶段文档、父级 memory、线程 memory 与现行状态入口。

## 当前剩余待办
- 无。

## 阶段外后续
- 如果继续推进 Codex 治理续办，应回到：
  - `09_强制skills闸门与执行规范重构`
  - 或者针对新的治理问题另行立项
- `11` 不再继续承接新的过程尾巴，也不再继续承担 prompt 分发职能。

## 完成标准
- [x] 用户后续不再需要为 `farm / NPC / spring-day1` 的正常开发或验收打开多个 Unity 工程。
- [x] `farm / NPC / spring-day1` 的后续承载面都明确为“分支”，不是“某个 worktree 目录”。
- [x] 共享根目录恢复到 `D:\Unity\Unity_learning\Sunset @ main`，重新成为默认中性现场。
- [x] `git worktree list` 最终只剩共享根目录。
- [x] `worktree` 被重新收束为事故隔离例外，不再是 Sunset 的新常态。
