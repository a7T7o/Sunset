# scene-build_迁后复核与从新路径恢复

## 已废弃（2026-03-22）
- 这份 prompt 建立在错误结果“正式 worktree 已切到 `D:\Unity\Unity_learning\scene-build-5.0.0-001`”之上。
- 当前统一口径：正式 worktree 仍是 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`。
- `D:\Unity\Unity_learning\scene-build-5.0.0-001` 只是误复制副本，`.git` 已失活，不可作为正式现场。
- 本文件只保留误操作历史，不再分发。

```text
迁移手术已经做完一半以上了。
治理侧没有再等旧目录完全解锁，而是走了兜底迁移：
- 先把 worktree 内容复制到：`D:\Unity\Unity_learning\scene-build-5.0.0-001`
- 再用 Git 的 `worktree repair` 把正式登记切到了新路径

治理侧当前看到的事实是：
- `git worktree list --porcelain` 里，正式注册路径已经是：`D:\Unity\Unity_learning\scene-build-5.0.0-001`
- 新路径当前 branch / HEAD 仍是：`codex/scene-build-5.0.0-001 @ 8e641e67`
- 4 个 TMP 字体资源 dirty 也已经一起带到了新路径
- 旧路径 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001` 物理上还在，但它现在只是遗留旧壳，不再是你应该继续工作的正式现场

你这轮只做下面 4 步：
1. 明确切换你的工作现场到：`D:\Unity\Unity_learning\scene-build-5.0.0-001`
2. 只读复核新路径的：
   - `git branch --show-current`
   - `git rev-parse HEAD`
   - `git status --short --branch`
3. 确认你后续只在新路径继续，不再回旧路径写任何内容
4. 结合 `spring-day1` 已交付的 `scene-build_handoff.md`，判断你下一刀是否可以继续回到 `SceneBuild_01` 精修

这轮不要做的事：
- 不要回旧路径继续施工
- 不要自己删旧路径
- 不要自己处理那 4 个 TMP dirty，先只做事实回执
- 不要现在就开 Unity / MCP live 写

聊天只回这些字段：
- 已读 prompt 路径
- new_project_root
- 当前 branch / HEAD
- 当前 git status
- old_path_abandoned: yes / no
- handoff_doc_read: yes / no
- can_resume_scene_work: yes / no
- blocker_or_checkpoint
- 一句话摘要
```
