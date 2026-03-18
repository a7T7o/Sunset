# shared-root-branch-occupancy

## 文件身份
- 本文件用于记录 shared root `D:\Unity\Unity_learning\Sunset` 的占用语义。
- 它不是 A 类热文件锁的替代品。
- 它只回答 shared root 当前是否处于中性可进入状态。

## 当前状态
- root_path: `D:\Unity\Unity_learning\Sunset`
- owner_mode: `neutral-main-ready`
- owner_thread: `none`
- current_branch: `main`
- last_verified_head: `3d305036`
- is_neutral: `true`
- lease_state: `neutral`
- blocking_dirty_scope: `none`
- daily_policy: `main-common + branch-task + checkpoint-first + merge-last`
- worktree_policy: `exception-only`
- last_updated: `2026-03-18`

## 解释口径
- 当 `current_branch = main` 且 `is_neutral = true` 时：
  - shared root 可作为 Sunset 默认进入现场。
- 当 `current_branch = main` 但 `is_neutral = false` 时：
  - 说明默认入口模型仍是 `main + branch-only`，但 shared root 当下不是可直接切分支的干净现场。
  - 必须先做 preflight，核清当前 dirty 归属，再决定是否允许继续进入业务分支。
- 但这不替代 Unity / MCP 单实例层；进入 Editor / MCP 读写前仍要再核：
  - `mcp-single-instance-occupancy.md`
  - `mcp-hot-zones.md`
- 当 shared root checkout 不在 `main` 时：
  - 必须先判定是否已被特定线程合法占用。
  - 当前若 `is_neutral = false`，则 `git-safe-sync.ps1` 的 `task` 模式会额外核对：
    - `owner_thread`
    - `current_branch`
    - 是否仍残留未纳入白名单的 remaining dirty
  - 在回正到 clean `main` 之前，只允许只读核查、治理记录与经审核的恢复动作。
- 当 `current_branch = main` 且 `is_neutral = true` 时：
  - shared root 已恢复为默认进入现场。
  - 业务线程可以重新按 `main-common + branch-task + checkpoint-first + merge-last` 模型进入。

## 一句话口径
- 当前 shared root 已恢复为 `main + neutral`；后续线程应先从 shared root 进入，再按闸机切入各自的 `codex/...` 分支，到 checkpoint 即归还。
