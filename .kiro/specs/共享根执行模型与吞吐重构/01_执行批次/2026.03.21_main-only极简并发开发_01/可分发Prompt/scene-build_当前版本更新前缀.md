# scene-build｜当前版本更新前缀

```text
当前版本更新，你这条线不是 shared root 普通线程，而是独立 worktree 场景施工线。
不要再按 shared root 的旧 branch 口径理解自己，也不要再等治理线程替你收口。

当前唯一正式现场：
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
- 当前正式分支语义：`codex/scene-build-5.0.0-001`
- 你所有场景施工、memory、checkpoint 都只在这个 worktree 内完成
- 不回 shared root 改 `Assets/000_Scenes/SceneBuild_01.unity`
- 你每完成一刀，就直接在这个 worktree 分支里提交你自己的 checkpoint
- 只有未来需要把成果挑选迁回 `main` 时，才升级治理介入

你先按新制度做两件事：
1. 完整回忆你上次做到哪里：
   - 已落盘了哪些场景施工
   - 当前 dirty 分别是什么
   - 哪些是本轮仍要继续的有效任务
2. 明确你这轮准备一次性推进到哪里，并直接在 worktree 内做到那个 checkpoint

当前精修方向仍然以 `spring-day1` handoff 为准：
- 东侧主入口抵达感
- 院落留白与站位焦点
- 工作台闪回焦点
- 农田 / 砍树教学落点
- 室内外衔接
- 禁止扩成整村大图
- 禁止只做纯美术摆场

只有命中下面情况你才停：
1. 有别的线程也在 live 写同一个 `SceneBuild_01` 或同批场景资源。
2. 你要做 Unity / MCP live 写，但已经有人占用。
3. 你把场景、引用或资源关系写坏了，需要先收口。

聊天只回这些字段：
- 当前在改什么
- 上次 checkpoint / 当前恢复点
- 当前 worktree dirty 分类
- 剩余精修任务清单
- 本轮下一步最多推进到哪里
- changed_paths
- 是否触碰 Unity / MCP live 写
- 是否撞到高危目标
- 当前是否已做 worktree checkpoint
- blocker_or_checkpoint
- 一句话摘要
```
