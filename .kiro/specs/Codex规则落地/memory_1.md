# Codex规则落地 Memory Continuation 1

## 2026-03-18｜shared root 恢复验证与 spring branch 自愈

**当前主线目标**
- 把 Sunset 从“闸机刚落地但线程恢复仍有残留漂移”的状态，推进到“shared root 健康、主要业务 continuation branch 已具备新闸机、可以继续按 `main-common + branch-task + checkpoint-first + merge-last` 恢复开发”的状态。

**本轮子任务 / 阻塞**
- 复核 `farm` 二阶段汇报中的 `grant-branch` FATAL 是否仍是 live 主线问题。
- 处理 `codex/spring-day1-story-progression-001` 的 branch-local drift。

**已完成事项**
- live 复核发现 shared root 当时真实停在 `codex/spring-day1-story-progression-001 @ a9c952b7`，而不是外部汇报中的 `main`。
- 已把 shared root 从干净的 `spring` 现场直接切回 `main`，恢复统一入口。
- 已在当前 live `main + neutral + clean` 现场下做受控 `farm` 闭环验证：
  - `grant-branch` 成功
  - `ensure-branch` 成功
  - `return-main` 成功
- 结论已收紧：`farm` 之前那份“grant 时报 shared root 当前不干净”的汇报，不再构成当前 `main` 新闸机的现行阻断，更像旧现场残影或切换期间的漂移证据。
- 已定位 `spring-day1` 的真实剩余问题是 branch-local drift：
  - 分支内 `scripts/git-safe-sync.ps1` 仍是旧版
  - 分支内缺少 `.kiro/locks/shared-root-branch-occupancy.md`
- 已在 `codex/spring-day1-story-progression-001` 上 graft `main` 的以下治理基础设施：
  - `scripts/git-safe-sync.ps1`
  - `AGENTS.md`
  - `.kiro/locks/shared-root-branch-occupancy.md`
- 已在 `spring` 分支上完成治理热修提交并推送：
  - `27dc06a1`
- 已再次从 `main` 做受控 `spring-day1` 闭环验证：
  - `grant-branch` 成功
  - `ensure-branch` 成功
  - `return-main` 成功

**关键决策**
- 当前真正需要恢复的不是再补一轮 shared root 主脚本，而是把老 continuation branch 的 branch-local 治理漂移补齐。
- `farm`、`npc`、`spring-day1` 现在都已经具备最新闸机所需的 branch-local 基础设施。
- `导航`、`遮挡` 当前没有现成 continuation branch 需要 graft；后续若它们从只读转真实写入，应直接从 clean `main` 按双阶段协议进入新 `codex/...` 分支。

**当前现场**
- shared root：`D:\Unity\Unity_learning\Sunset`
- 当前分支：`main`
- 当前 HEAD：`1add175b`
- 当前 `git status --short --branch`：clean
- occupancy：`main + neutral + branch_grant_state = none`

**恢复点 / 下一步**
- 这轮治理阻塞已经解除，主线可以回到“发放双阶段唤醒 / 准入 prompt，让业务线程继续真实开发”。
- 若继续恢复线程，优先口径：
  - `farm` / `NPC` / `spring-day1` 可直接按新闸机进入
  - `导航` / `遮挡` 默认仍先只读，转写入时再申请自己的 `codex/...` 分支
