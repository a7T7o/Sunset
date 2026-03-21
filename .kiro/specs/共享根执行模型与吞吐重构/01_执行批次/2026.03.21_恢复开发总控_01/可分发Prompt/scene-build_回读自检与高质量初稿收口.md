# scene-build - 回读自检与高质量初稿收口
```text
【恢复开发总控 01｜scene-build｜回读自检与高质量初稿收口】
你继续使用自己的专属 worktree：
- 工作目录：D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001
- 分支：codex/scene-build-5.0.0-001
- 当前稳定基线：a62b557d

你上一轮已经完成：
- SceneBuild_01 的逻辑层最小版本
- 锚点、围栏阻挡、触发区和必要挂点落地
- 最小 Git checkpoint 收口

这轮不要再回头补“逻辑层有没有做”，也不要顺手去处理 shared root 残留。
你这轮的目标是：
- 把当前 SceneBuild_01 做一次真正的回读自检
- 在不失控扩面的前提下，把它收成“可继续精修的高质量初稿”

本轮优先关注：
- 当前场景层级是否清楚、命名是否稳定、挂点是否自洽
- 围栏阻挡、触发区、锚点这些逻辑对象是否和场景组织一致
- 地表 / 结构 / 装饰 / 逻辑之间是否存在明显穿帮、遮挡怪异、布局失衡
- 能否形成一份“现在交给项目经理看也不会太糙”的初稿状态

本轮允许：
- 在自己的 worktree 内继续修改 scene、文档、线程记忆
- 继续使用 Unity / MCP 或 Scene YAML；优先走你当前最稳定的路径
- 做只读 MCP 探测、层级回读、必要的自检证据采集
- 形成新的最小 Git checkpoint

本轮必须保持：
- 不处理 shared root 那边的任何残留
- 不把“本地 YAML 落盘”直接说成“Unity live 验收通过”
- 不为了赶进度继续无边界堆物件
- 如果本会话 `unityMCP` 仍读到 `Sub2API` HTML，就明确保留这个阻塞，不准假装闭环已恢复

聊天只回复：
- 已读 prompt 路径
- 当前 branch / HEAD
- 当前 git status
- 本轮 checkpoint
- 是否触碰 Unity / MCP
- 是否已经 clean
- blocker_or_checkpoint
- 一句话摘要
```
