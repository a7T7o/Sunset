# A 阶段 runbook：shared root 人工回正

## 1. 文档身份
- 本文是 `20_shared-root现场回正与物理闸机落地` 的 A 阶段人工执行版 runbook。
- 它只描述“如何把当前 shared root 从错位任务分支 + mixed dirty 回正到可继续治理的 `main`”。
- 它不是“已经执行完毕”的记录。
- 本文默认由用户人工执行危险 Git 步骤；Codex 只负责给出经 live 核对过的清单、顺序、风险与验收标准。

## 2. 当前 live 基线
- shared root：
  - `D:\Unity\Unity_learning\Sunset`
- 当前分支：
  - `codex/npc-roam-phase2-003`
- 当前 `HEAD`：
  - `2ecc2b753ea711557baca09432d0c7e3760cb3f7`
- 当前 `origin/codex/npc-roam-phase2-003`：
  - `2ecc2b753ea711557baca09432d0c7e3760cb3f7`
- 目标回退点：
  - `c81d1f99e4cb3a53f054ab445fc972de09b6ab97`
- 当前 `main` / `origin/main`：
  - `64ff98161f3fe0b900d532c652241af5e34d6d22`

## 3. 当前 owner / blocker / blocked 清单
### 当前 owner 分层裁定
- shared root 当前 checkout 的业务承载分支：
  - `codex/npc-roam-phase2-003`
  - 语义 owner 仍是 `NPC`
- shared root 当前顶部错位提交：
  - `2ecc2b75`
  - 内容 owner 是 `导航检测`
- shared root 当前 live 治理 / runbook 脏改：
  - 当前这轮 `Codex规则落地`
  - 这些改动还没有形成独立 checkpoint，不能在本 runbook 执行前直接丢弃

### 当前 blocker
- blocker 1：
  - `2ecc2b75` 已落在 `codex/npc-roam-phase2-003` 本地与远端
  - 它把导航文档提交进了 NPC 分支
- blocker 2：
  - shared root 当前 working tree 里还挂着两份 farm 残留 dirty
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V2\memory_0.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\memory.md`
- blocker 3：
  - 当前治理线程已在 shared root 上新增一批未提交治理文档和脚本改动
  - 其中还包含未跟踪目录：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\20_shared-root现场回正与物理闸机落地`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\2026.03.18线程并发讨论`

### 当前 blocked
- `spring-day1`
- `NPC` 的正常 branch-only 重入
- `农田交互修复V2`
- 后续任何想从 shared root 安全切入 `codex/...` 的线程

## 4. 推荐手术路径
- 推荐路径：
  - 先分组 park 当前 mixed dirty
  - 再把 `codex/npc-roam-phase2-003` 从 `2ecc2b75` 回退到 `c81d1f99`
  - 然后用 `--force-with-lease` 回正远端
  - 再把 shared root 切回 `main`
  - 最后只恢复治理线程自己的 stage 20 文档脏改，继续治理，不恢复 farm 脏改
- 不推荐直接“无脑 reset + stash 全仓”：
  - 因为当前 working tree 里混着三类东西：
    - 当前治理线程的新文档 / 新脚本
    - farm 的 tracked dirty
    - 两个未跟踪讨论 / 阶段目录
  - 如果只做一次全仓 stash，后续恢复时会重新把 farm dirty 和治理 dirty 搅在一起

## 5. 推荐执行顺序
### Step 0. 再做一次最终只读确认
在 `D:\Unity\Unity_learning\Sunset` 中执行：

```powershell
git status --short --branch
git log --oneline --decorate -n 3
git reflog -n 10 --date=iso
git stash list
```

预期：
- 当前 branch 仍是 `codex/npc-roam-phase2-003`
- 顶部仍是：
  - `2ecc2b75`
  - `c81d1f99`
- 先确认没有新增未知 dirty
- 先确认不会误伤已有 stash

### Step 1. 单独 park 当前治理线程的 stage 20 脏改
在 `D:\Unity\Unity_learning\Sunset` 中执行：

```powershell
git stash push -u -m "2026-03-18 stage20-governance-parking" -- `
  .codex/threads/Sunset/Codex规则落地 `
  .kiro/locks/shared-root-branch-occupancy.md `
  .kiro/specs/Codex规则落地 `
  .kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset当前唯一状态说明_2026-03-17.md `
  .kiro/specs/Steering规则区优化/当前运行基线与开发规则/基础规则与执行口径.md `
  .kiro/specs/Steering规则区优化/2026.03.18线程并发讨论 `
  .kiro/steering/git-safety-baseline.md `
  AGENTS.md `
  scripts/git-safe-sync.ps1
```

预期：
- 当前 stage 20 治理文件被单独 park
- 这一步会把 `runbook.md`、阶段 `20` 新目录、讨论目录和脚本改动一起装进专用 stash

### Step 2. 单独 park farm 的 shared root 残留 dirty
在 `D:\Unity\Unity_learning\Sunset` 中执行：

```powershell
git stash push -m "2026-03-18 farm-root-dirty-parking" -- `
  .codex/threads/Sunset/农田交互修复V2/memory_0.md `
  .kiro/specs/农田系统/memory.md
```

预期：
- shared root 上那两份 farm memory dirty 被单独 park
- 这份 stash 后续不要在 shared root `main` 上直接恢复

### Step 3. 确认 shared root 已经 clean
在 `D:\Unity\Unity_learning\Sunset` 中执行：

```powershell
git status --short --branch
git stash list
```

预期：
- working tree clean
- stash list 至少新增两条：
  - `stage20-governance-parking`
  - `farm-root-dirty-parking`

### Step 4. 回正 NPC 分支历史
在 `D:\Unity\Unity_learning\Sunset` 中执行：

```powershell
git switch codex/npc-roam-phase2-003
git log --oneline --decorate -n 3
git reset --hard c81d1f99e4cb3a53f054ab445fc972de09b6ab97
git log --oneline --decorate -n 3
git push origin codex/npc-roam-phase2-003 --force-with-lease
```

预期：
- reset 前顶部是：
  - `2ecc2b75`
  - `c81d1f99`
- reset 后顶部变为：
  - `c81d1f99`
- 远端 `origin/codex/npc-roam-phase2-003` 被一并回正到 `c81d1f99`

风险提示：
- 这是改写已推送分支历史的危险步骤
- 必须人工执行
- 必须使用 `--force-with-lease`，不要改成裸 `--force`

### Step 5. 把 shared root 切回 `main`
在 `D:\Unity\Unity_learning\Sunset` 中执行：

```powershell
git switch main
git status --short --branch
```

预期：
- 当前 branch 变为 `main`
- shared root 不再停在 NPC 业务分支上
- 如果此时 working tree 仍 clean，说明 Git 现场已经完成第一层回正

### Step 6. 只恢复治理 stash，不恢复 farm stash
先看 stash 内容：

```powershell
git stash list
git stash show --stat stash@{0}
git stash show --stat stash@{1}
```

确认哪一条是 `stage20-governance-parking` 后，再执行：

```powershell
git stash pop <stage20-governance-parking 对应的 stash 条目>
```

注意：
- 不要把 `farm-root-dirty-parking` 这条 stash 在 shared root 的 `main` 上直接 pop 回来
- 那条 stash 只保留给 farm 后续在自己的合法 carrier 中手工处理

### Step 7. 立即修正 shared root 占用文档到 neutral
恢复治理 stash 后，立刻把：
- `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md`

改成 neutral 口径，至少包含：
- `owner_mode: neutral-main-ready`
- `owner_thread: none`
- `current_branch: main`
- `last_verified_head: <当前 main 的 short hash>`
- `is_neutral: true`
- `lease_state: neutral`
- `blocking_dirty_scope: none`
- `last_updated: 2026-03-18`

### Step 8. shared root 回正后的最低验收
在 `D:\Unity\Unity_learning\Sunset` 中执行：

```powershell
git branch --show-current
git rev-parse --short HEAD
git status --short --branch
git worktree list --porcelain
```

最低通过标准：
- 当前 branch：
  - `main`
- 当前 working tree：
  - 不包含他线业务 dirty
- `shared-root-branch-occupancy.md`：
  - `is_neutral = true`
  - `current_branch = main`
- `git worktree list`：
  - 仍可暂时看到历史 cleanroom，但 shared root 本身已回正
- 当前治理线程可以继续在 `main + governance` 上处理 stage 20

## 6. 可选替代路径
如果用户明确不接受改写 NPC 分支历史，可以改用“保历史、加 revert”的保守路径：

```powershell
git switch codex/npc-roam-phase2-003
git revert --no-edit 2ecc2b753ea711557baca09432d0c7e3760cb3f7
git push origin codex/npc-roam-phase2-003
git switch main
```

但这条路径的缺点很明确：
- `2ecc2b75` 仍会留在 NPC 分支历史里
- 导航检测错位写入会作为一段显式历史永久保留
- 分支会更脏，不符合这轮“回正 shared root 与 branch 语义”的主目标

因此本 runbook 的默认推荐仍是：
- `reset --hard c81d1f99`
- `push --force-with-lease`

## 7. 哪些步骤必须人工做，哪些后续可脚本化
### 当前必须人工执行
- Step 1 / Step 2 的 stash 分组裁定
- Step 4 的 `reset --hard` + `push --force-with-lease`
- Step 6 的 stash 恢复选择
- Step 7 的 neutral 回填

### 后续可脚本化候选
- stage 20 治理脏改的定向 park / restore
- shared root auto claim / auto release wrapper
- 占用文档的 neutral / occupied 自动回填
- “脏改按线程分组提示”的辅助脚本

## 8. 当前 runbook 的边界
- 本 runbook 解决的是：
  - shared root 从错位业务分支回正到 `main`
  - 错位导航提交从 NPC 分支剥离
  - 当前治理脏改与 farm 脏改的分组 park
- 本 runbook 不直接解决：
  - farm cleanroom worktree 的最终删除
  - farm stash 的最终归宿
  - Unity / MCP 单实例的 live 验收
  - 自动 claim / release wrapper
