# scene-build_迁移前释放Unity与MCP目录锁

## 已废弃（2026-03-22）
- 这份 prompt 基于错误前提“scene-build 将迁到 `D:\Unity\Unity_learning\scene-build-5.0.0-001`”。
- 当前统一口径：正式 worktree 仍是 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`。
- `D:\Unity\Unity_learning\scene-build-5.0.0-001` 只是误复制副本，`.git` 已失活，不可作为正式现场。
- 本文件只保留历史，不再分发。

```text
这不是继续施工的 prompt。
你现在不要继续改 `SceneBuild_01`，不要新写 scene / prefab / script / asset，也不要自己执行 `git worktree move`。

治理侧已经把这轮事实收口到了下面这版：
- `spring-day1` 的 handoff 已经到位，当前不缺剧情 brief
- 你的 worktree 已经 clean，`ready_for_move` 这一步已经成立
- 迁移目标路径改正为：`D:\Unity\Unity_learning\scene-build-5.0.0-001`
- 治理侧已经实测过真正的迁移动作：
  - `git -C D:\Unity\Unity_learning\Sunset worktree move D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001 D:\Unity\Unity_learning\scene-build-5.0.0-001`
  - 当前失败原因不是 Git 规则，而是旧目录还被 live 进程占着

目前已确认仍咬住旧目录 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001` 的进程类型有：
- `Unity.exe`
- `mcp-for-unity` 的 `http / stdio` 进程
- `Library\MCPForUnity\TerminalScripts\mcp-terminal.cmd`

你这轮只做下面 5 步：
1. 先确认当前没有未保存的 scene / prefab / 文档修改；如果有，先保存，再继续
2. 关闭指向 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001` 的 Unity Editor
3. 关闭这条线对应的 MCP 进程、project-scoped HTTP server 和 `mcp-terminal` 窗口
4. 只读复核这些进程是否已经不再占用旧目录
5. 停下等待治理侧继续执行真正的 `git worktree move`

如果你发现：
- 还有没法关掉的残留进程
- Unity 里还有不敢丢的未保存改动
- 或者你关闭后旧目录仍然报占用
那就不要硬做，直接把剩余 blocker 原样回我。

聊天只回这些字段：
- 已读 prompt 路径
- project_root
- 当前 branch / HEAD
- 当前 git status
- unity_closed: yes / no
- mcp_closed: yes / no
- remaining_lock_processes
- ready_for_move_now: yes / no
- blocker_or_checkpoint
- 一句话摘要
```
