# shared-root-branch-occupancy

## 文件身份
- 本文件用于记录 shared root `D:\Unity\Unity_learning\Sunset` 的占用语义。
- 它不是 A 类热文件锁的替代品。
- 它只回答 shared root 当前是否处于中性可进入状态。

## 当前状态
- root_path: `D:\Unity\Unity_learning\Sunset`
- owner_mode: `governance-main-finalizing`
- owner_thread: `Codex规则落地`
- current_branch: `main`
- last_verified_head: `64ff9816`
- is_neutral: `false`
- lease_state: `governance-finalizing`
- blocking_dirty_scope: `A 阶段 Git 外科已执行完毕，shared root 已回到 main；当前仅剩阶段 20 治理文档与脚本改动尚未同步`
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
- 当 shared root 已回到 `main`，但 `owner_mode = governance-main-finalizing` 时：
  - 说明业务分支错位已剥离，shared root 已进入治理收口阶段。
  - 此时只允许治理线程在 `main + governance` 上做最后同步；业务线程仍不应抢入。

## 一句话口径
- 当前 shared root 已经从错位的 NPC 分支回到了 `main`，但阶段 20 的治理同步尚未完成；现在是“治理收口中”，还不是最终 neutral。
