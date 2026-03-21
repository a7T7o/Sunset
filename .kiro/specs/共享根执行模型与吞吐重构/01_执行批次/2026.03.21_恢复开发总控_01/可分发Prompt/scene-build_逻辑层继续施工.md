# scene-build - 逻辑层继续施工

```text
【恢复开发总控 01｜scene-build｜逻辑层继续施工】
你当前继续使用自己的专属 worktree：
- 工作目录：D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001
- 分支：codex/scene-build-5.0.0-001
- 当前稳定基线：44afab3f

你上一轮已经完成：
- 装饰层纠偏
- 对 `Primary.unity` 的参考吸收
- 最小 Git checkpoint 收口

这轮不要回头再解释装饰层，也不要处理 shared root 残留。你的目标是：
- 继续把 `SceneBuild_01` 从“结构 + 装饰”推进到“逻辑层最小版本”

本轮优先关注：
- GameplayAnchors 的真实用途落地
- 关键交互 / 出入口 / 可站位 / 触发点的最小逻辑框架
- 逻辑层与现有建筑、路径、装饰的对应关系
- DebugPreview / LightingFX / Systems 下真正需要为后续施工留下的最小挂点

本轮允许：
- 在自己的 worktree 内继续修改 scene、文档、线程记忆
- 继续使用 Unity / MCP 或 Scene YAML；优先选你当前最稳定的路径
- 形成新的最小 Git checkpoint

本轮必须保持：
- 不碰 shared root 残留的 `SceneBuild_01.unity`
- 不把“逻辑层施工推进”写成“Unity live 验收通过”
- 不为了赶进度乱塞无验证价值的空 GameObject

本轮回执只回复：
- 已读 prompt 路径
- 当前 branch / HEAD
- 当前 git status
- 本轮 checkpoint
- 是否触碰 Unity / MCP
- 是否已经 clean
- blocker_or_checkpoint
- 一句话摘要
```
