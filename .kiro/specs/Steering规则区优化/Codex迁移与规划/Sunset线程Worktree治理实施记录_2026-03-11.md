# Sunset 线程 Worktree 治理实施记录（2026-03-11）

## 1. 本轮目的
- 固化 `Sunset` 的长期线程 - 分支 - worktree 映射。
- 保护根目录 `D:\Unity\Unity_learning\Sunset` 只承载稳定 `main`。
- 让 `NPC` 与农田补丁线程拥有各自独立文件视图。
- 在不触碰 NPC 合并到 `main` 的前提下，完成线程路由底盘。

## 2. 本轮实际动作

### 2.1 新建长期 worktree 根目录
- `D:\Unity\Unity_learning\Sunset_worktrees`

### 2.2 新建两个长期功能 worktree
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC`
  - 分支：`codex/npc-generator-pipeline`
  - HEAD：`404933466f5538475c0c898b21dc808a60b38ea2`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002`
  - 分支：`codex/farm-10.2.2-patch002`
  - HEAD：`a47da9e1af84f9479b33a9c3dbefabd1eff1d7f9`

### 2.3 保持根目录主路不变
- `D:\Unity\Unity_learning\Sunset`
  - 分支：`main`
  - HEAD：`8b7d630f47cea385912cf14fe91162c40c71a541`

### 2.4 备份并对齐 Codex 状态层
- 已备份：
  - `C:\Users\aTo\.codex\state_5.sqlite.bak-20260311-183352-sunset-worktree-routing`
- 已把相关活跃 `Sunset` 线程的默认 `cwd` / `git_branch` / `git_sha` 对齐到以下结果：
  - 治理 / 总览类活跃线程 -> `D:\Unity\Unity_learning\Sunset` / `main`
  - NPC 活跃线程（状态库历史标题线索：`--+++*`） -> `D:\Unity\Unity_learning\Sunset_worktrees\NPC` / `codex/npc-generator-pipeline`
  - 农田活跃线程 -> `D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002` / `codex/farm-10.2.2-patch002`

## 3. 当前 worktree 总状态

```text
D:\Unity\Unity_learning\Sunset                              -> main
D:\Unity\Unity_learning\Sunset\.claude\worktrees\agent-a2df3da0 -> worktree-agent-a2df3da0（历史本地特殊 worktree）
D:\Unity\Unity_learning\Sunset_worktrees\NPC               -> codex/npc-generator-pipeline
D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002 -> codex/farm-10.2.2-patch002
```

## 4. 本轮新增的项目内文档
- `D:\Unity\Unity_learning\Sunset\.codex\threads\线程分支对照表.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\Codex迁移与规划\Codex线程Worktree使用说明.md`

## 5. 本轮明确没有做的事
- 没有把 `codex/npc-generator-pipeline` 合并到 `main`
- 没有把农田分支合并到 `main`
- 没有删除任何业务分支
- 没有改动 NPC 或农田业务代码本身

## 6. 当前验收重点
- 看目录是否存在
- 看根目录是否仍为 `main`
- 看两个功能 worktree 是否分别落在正确分支
- 看 `线程分支对照表.md` 与 `Codex线程Worktree使用说明.md` 是否能直接指导后续使用
