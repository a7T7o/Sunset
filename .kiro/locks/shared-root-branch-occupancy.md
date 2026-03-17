# shared-root-branch-occupancy

## 文件身份
- 本文件用于记录 shared root `D:\Unity\Unity_learning\Sunset` 的占用语义。
- 它不是 A 类热文件锁的替代品。
- 它只回答 shared root 当前是否处于中性可进入状态。

## 当前状态
- root_path: `D:\Unity\Unity_learning\Sunset`
- owner_mode: `neutral-shared-root`
- current_branch: `main`
- last_verified_head: `952a1f23`
- is_neutral: `true`
- daily_policy: `main + branch-only`
- worktree_policy: `exception-only`
- last_updated: `2026-03-17`

## 解释口径
- 当 `current_branch = main` 且 `is_neutral = true` 时：
  - shared root 可作为 Sunset 默认进入现场。
- 但这不替代 Unity / MCP 单实例层；进入 Editor / MCP 读写前仍要再核：
  - `mcp-single-instance-occupancy.md`
  - `mcp-hot-zones.md`
- 当 shared root checkout 不在 `main` 时：
  - 必须先判定是否已被特定线程合法占用。
  - 未裁定前不得把它当中性现场继续写业务。

## 一句话口径
- 当前 shared root 已回到中性 `main`，后续默认从这里进入，再按任务切到对应 `codex/...` 分支。
