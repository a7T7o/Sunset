# scene-build｜当前版本更新并继续施工

```text
当前版本更新，你这条线现在直接继续施工，不再停在“我先证明能不能开始”。

你不是 shared root 普通线程，你是独立 worktree 场景施工线：
- 正式现场只认 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
- 正式分支语义只认 `codex/scene-build-5.0.0-001`
- 你每完成一刀，就直接在这个 worktree 分支里提交自己的 checkpoint
- 不回 shared root 改 `Assets/000_Scenes/SceneBuild_01.unity`
- 只有未来要把成果挑选迁回 `main` 时，治理线程才介入

你先完整回忆并汇报：
1. 你上次已经落了哪些批次的 `SceneBuild_01` 精修。
2. 你当前 dirty 里哪些是：
   - 有效施工内容
   - TMP 资源副产物
   - memory / 快照补记
3. 你当前还剩哪些最关键的精修任务。

你这轮不要再做“场景理解对齐文档”，直接做真实施工。
当前优先级按这个顺序推进：
1. 入口停驻点到院心的节奏
2. 院心介绍区到工作台的空间语义顺序
3. 工作台到农田 / 砍树教学落点的视线与走位
4. 室内外衔接的自然性

明确禁止：
- 不要扩成整村大图
- 不要回去做纯装饰堆砌
- 不要把“YAML 已改”写成“Unity live 已验收”
- 不要自己改治理目录、git 规则或别的线程问题

只有命中下面情况你才停：
1. 有别的线程也在 live 写同一个 `SceneBuild_01` 或同批场景资源。
2. 你要做 Unity / MCP live 写，但已经有人占用。
3. 你把场景、引用或资源关系写坏了，需要先收口。

直接做到你这轮承诺的下一个 checkpoint 再停。

回执直接追加到：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\001部分回执.md`

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
