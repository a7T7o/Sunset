# scene-build - 当前任务回执与迁移前冻结

## 已废弃（2026-03-22）
- 这份 prompt 基于错误前提“scene-build 将迁到 `D:\Unity\Unity_learning\scene-build-5.0.0-001`”。
- 当前统一口径：正式 worktree 仍是 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`。
- `D:\Unity\Unity_learning\scene-build-5.0.0-001` 只是误复制副本，`.git` 已失活，不可作为正式现场。
- 本文件只保留历史，不再分发。

```text
这不是新的施工 prompt。
你先把手里“当前这一刀”收口到可描述状态，然后停下来给我一份迁移前冻结回执。

这轮要求：
- 不再新开下一层施工
- 如果你正在写一刀中的最后几步，可以先把这一刀收口
- 收口后不要自己搬目录
- 收口后不要回 shared root 改东西

你要按真实现场回答下面这些字段：
- 已读 prompt 路径
- project_root
- 当前 branch / HEAD
- 当前 git status：clean 还是 dirty
- 当前这一刀已经完成了什么
- 当前这一刀如果现在就冻结，还差什么
- changed_paths
- 是否触碰 Unity / MCP live 写
- can_freeze_now: yes / no
- migration_ready: yes / no
- migration_blockers
- move_target: `D:\Unity\Unity_learning\scene-build-5.0.0-001`
- expected_move_method: `git worktree move`
- blocker_or_checkpoint
- 一句话摘要

如果 `can_freeze_now = no`，请明确写清：
- 还差哪一个最小尾项
- 预计冻结点落在哪个 checkpoint 或哪组文件
```
