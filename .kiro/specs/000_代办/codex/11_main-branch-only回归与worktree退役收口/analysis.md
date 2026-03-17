# 分析：main-branch-only 回归与 worktree 退役收口

## 1. 阶段定位
- `09` 解决“先过启动闸门再做事”。
- `10` 解决“共享根目录分支漂移事故的止血、取证与承载面找回”。
- `11` 解决最后一公里：把事故期临时 `worktree` 全部退役，恢复 `D:\Unity\Unity_learning\Sunset @ main` 的 `main + branch-only` 常态。

## 2. 最终裁定
- `11` 阶段目标已经真实完成，不再继续向本阶段追加新尾巴。
- shared root 当前真实状态是：
  - `D:\Unity\Unity_learning\Sunset @ main @ 763ba4a1`
  - `git status --short --branch` clean
  - `origin/main` 已同步到同一提交
- 当前 `git worktree list --porcelain` 只剩：
  - `D:/Unity/Unity_learning/Sunset`
- 三条业务线现在都只保留“根目录 + 分支”入口：
  - `spring-day1 -> D:\Unity\Unity_learning\Sunset @ codex/spring-day1-story-progression-001 @ a9c952b7`
  - `NPC -> D:\Unity\Unity_learning\Sunset @ codex/npc-roam-phase2-002 @ 6e2af71b`
  - `farm -> D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-cleanroom001 @ 66c19fa1`
- `npc_restore.zip` 与 `Assets/Screenshots*` 已移出仓库工作树，不再阻塞 shared root 清场。

## 3. 本轮实际执行
- 先在 `codex/governance-11-shared-root-return001` 上完成治理收口提交 `af3060f9`。
- 因该治理分支基于污染链 `11e0b7b4`，没有整支并回 `main`，而是只将治理提交 `cherry-pick` 到 `main`，形成干净终局提交 `763ba4a1`。
- 已真实完成并推送：
  - `main @ 763ba4a1`
  - `origin/main @ 763ba4a1`
- 第二批 worktree 已真实退役并删除物理目录：
  - `D:\Unity\Unity_learning\Sunset_worktrees\spring-day1-story-progression-001`
  - `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_continue`
  - `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue`
  - `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
- 根目录 branch-only 检出验证已真实完成，`git reflog` 保留了三次切换证据：
  - `2026-03-17 19:38:29`：`main -> codex/spring-day1-story-progression-001`
  - `2026-03-17 19:38:44`：`main -> codex/npc-roam-phase2-002`
  - `2026-03-17 19:39:45`：`main -> codex/farm-1.0.2-cleanroom001`
  - 每次都已返回 `main`，且验证时工作树保持 clean

## 4. 为什么现在可以判定为真正的 branch-only 常态
- 长期承载面已经全部落回 Git 分支，而不是某个物理目录。
- shared root 已重新成为默认中性现场，用户后续正常开发或验收不再需要额外打开多个 Unity 工程副本。
- `worktree` 已被收束回例外语义：
  - 只用于未来确有必要的高风险隔离、事故 cleanroom 或特殊实验
  - 不再是 Sunset 的日常入口

## 5. 11 阶段完成后的分层理解
- `09` 继续承担强制 skills、启动闸门和规则执行面的后续深化。
- `10` 保留为事故链路、污染分支和 carrier 裁定的历史结论层。
- `11` 已完成 Git/现场终局收口，今后不再继续承接新的过程尾巴；若再出现新的 shared root 事故，应另行立项。

## 6. 一句话结论
- `11_main-branch-only回归与worktree退役收口` 已经从“执行方案”变成“已完成事实”：Sunset 现已恢复到 `D:\Unity\Unity_learning\Sunset @ main` 的 `main + branch-only` 常态，`worktree` 不再是当前工作流的一部分。
