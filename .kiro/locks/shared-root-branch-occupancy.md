# shared-root-branch-occupancy

## 文件身份
- 本文件用于记录 shared root `D:\Unity\Unity_learning\Sunset` 的占用语义。
- 它不是 A 类热文件锁的替代品。
- 它只回答 shared root 当前是否处于中性可进入状态。

## 当前状态
- root_path: `D:\Unity\Unity_learning\Sunset`
- owner_mode: `main-dirty-shared-root`
- current_branch: `main`
- last_verified_head: `ece0c0ea`
- is_neutral: `false`
- blocking_dirty_scope: `other-thread dirty is still present in shared root, currently dominated by NPC business files plus non-governance memories`
- daily_policy: `main + branch-only`
- worktree_policy: `exception-only`
- last_updated: `2026-03-17`

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
  - 未裁定前不得把它当中性现场继续写业务。

## 一句话口径
- 当前 shared root 虽已回到 `main`，但仍带其他线程的 dirty，因此默认入口模型是 `main + branch-only`，不代表此刻可以无前置核查地直接切分支。
