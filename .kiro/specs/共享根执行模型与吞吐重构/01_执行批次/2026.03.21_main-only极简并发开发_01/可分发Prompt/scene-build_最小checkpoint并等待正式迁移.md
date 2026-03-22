# scene-build - 最小checkpoint并等待正式迁移

## 已废弃（2026-03-22）
- 这份 prompt 基于错误前提“scene-build 将迁到 `D:\Unity\Unity_learning\scene-build-5.0.0-001`”。
- 当前统一口径：正式 worktree 仍是 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`。
- `D:\Unity\Unity_learning\scene-build-5.0.0-001` 只是误复制副本，`.git` 已失活，不可作为正式现场。
- 本文件只保留历史，不再分发。

```text
这不是继续施工的 prompt。
你现在不要继续扩 `SceneBuild_01`，也不要自己搬目录。

治理侧对你刚才那份冻结回执的正式裁定已经给出：
- 不采用“带 3 个记忆 dirty 直接迁移”
- 先把当前 3 个记忆 dirty 收成一个最小 checkpoint
- checkpoint 完成并确认 clean 后，停下等待治理侧执行正式迁移

当前现场：
- project_root：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
- 当前工作分支应仍是：`codex/scene-build-5.0.0-001`
- 目标迁移路径固定为：`D:\Unity\Unity_learning\scene-build-5.0.0-001`
- 正式迁移方式固定为：`git worktree move`

这轮只允许纳入 checkpoint 的文件：
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md`
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\memory.md`
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.codex\threads\Sunset\Skills和MCP\memory_0.md`

你这轮要做的事只有 4 步：
1. 先复核当前 tracked dirty 是否仍然只有上面这 3 个记忆文件
2. 如果是，就把这 3 个文件收成一个最小 checkpoint
3. checkpoint 后确认当前 worktree `git status clean`
4. 停下，等待治理侧后续执行目录迁移

如果你发现除了这 3 个文件之外，还有别的 tracked dirty：
- 不要自己扩 scope
- 不要硬提交
- 立刻停下并回报真实 remaining dirty

这轮不要做：
- 不要继续施工 `SceneBuild_01`
- 不要新改 scene / prefab / script / asset
- 不要触碰 Unity / MCP live 写
- 不要自己执行 `git worktree move`
- 不要回 shared root 改东西

聊天只回：
- 已读 prompt 路径
- project_root
- 当前 branch / HEAD
- checkpoint_commit
- 当前 git status
- changed_paths
- 是否触碰 Unity / MCP live 写
- ready_for_move: yes / no
- move_target
- blocker_or_checkpoint
- 一句话摘要
```
