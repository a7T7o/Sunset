# spring-day1 - 当前任务回执与向 scene-build 交接

## 停用说明（2026-03-22）
- 本文件正文可继续作为交接字段参考，但其中关于“scene-build 后续还会迁到新路径”的假设已经失效。
- 当前统一口径：`scene-build` 正式现场仍是 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`。
- 如果后续继续使用本文件，请按上面这个正式路径理解，不要再引用错误副本路径。

```text
这不是一份“再开一轮新开发”的 prompt。
你现在只做一件事：把你手上的 `spring-day1` 理解，收成一份 `scene-build` 可直接执行的空间 brief，然后停下来交件。

重点不是证明你能不能继续开始，而是把 `scene-build` 真正需要吃的内容讲清楚。
如果你回来的只是状态汇报，而没有形成正式交付口径，这轮就不算完成。

你回执时必须按真实现场回答下面这些字段：
- 已读 prompt 路径
- 当前 branch / HEAD
- 当前 git status：clean 还是 dirty
- 正式交付文件路径
- 当前这一刀已经完成了什么
- `Day1 场景模块清单` 的一句话结论
- `SceneBuild_01 的正式身份`
- `SceneBuild_01 的强制承载动作`
- `SceneBuild_01 的禁止误扩边界`
- `next_scene_build_focus`
- changed_paths
- 是否触碰 Unity / MCP live 写
- 是否撞到高危目标
- handoff_ready: yes / no
- blocker_or_checkpoint
- 一句话摘要

如果 `handoff_ready = no`，请明确写清：
- 还差哪一个最小尾项
- 这个尾项收完后，正式交付文件会落在哪个路径

如果 `handoff_ready = yes`，请确保你的交付件已经满足：
- `scene-build` 看完就能继续施工
- 不是泛剧情复述
- 不是 UI / 字幕 / 对话实现代办
- 已经把“该做什么”和“先别扩什么”分开写清
- 即使后续 Codex 映射再调整，这份交付件也仍然可以直接读、直接用
```
