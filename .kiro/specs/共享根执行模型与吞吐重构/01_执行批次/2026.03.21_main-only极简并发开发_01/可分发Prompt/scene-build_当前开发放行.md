# scene-build - 当前开发放行

```text
你继续真实开发，但你这条线不是 shared root 普通线程，而是独立场景施工线。

当前工作现场：
- 只在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001` 内工作
- 不回 shared root 改场景
- 当前过渡目录后续要迁到 `D:\Unity\Unity_learning\SceneBuild_Standalone\scene-build-5.0.0-001`
- 在治理线程发“迁移前冻结”prompt 之前，不自行搬目录

你这轮直接做真实推进，不要再做“先证明我能不能开始”：
- 继续把 `SceneBuild_01` 做成能承载 `spring-day1` 的场景
- 优先补真正影响剧本承载、玩家动线和教学落点的部分
- 优先回读：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\requirements.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\OUT_tasks.md`

这轮不要做的事：
- 不要回 shared root 写 `Assets/000_Scenes/SceneBuild_01.unity`
- 不要把“YAML 已落盘”写成“Unity live 已验收”
- 不要顺手处理治理目录、Git 规则或别的线程问题
- 不要自己决定迁目录

只有命中下面情况你才停：
1. 有人也在 live 写同一个 `SceneBuild_01` 或同批场景资源
2. 你要做 Unity / MCP live 写入，但已经有人在写
3. 你把当前场景现场写坏了，需要先收口

聊天只回：
- 当前在改什么
- changed_paths
- 是否触碰 Unity / MCP live 写
- 是否撞到高危目标
- blocker_or_checkpoint
- 一句话摘要
```
