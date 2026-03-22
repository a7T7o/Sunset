# spring-day1｜剩余文档整理并直接 main 收口

```text
当前规则已经更新，而且这条更新已经真正进入 `main`：
- `main-only + whitelist-sync + exception-escalation`
- 不再因为 shared root 里有 unrelated dirty，就默认拦住你自己的白名单提交

先确认一个已成立事实：
- `spring-day1 0.0.2` 的基础脊柱代码已经进入 `main @ 83d809a9`
- 所以你这轮不要再去重复提交那批 `Story` 代码

当前 shared root：
- `D:\Unity\Unity_learning\Sunset @ main`

你这轮只做一件事：
- 把 `spring-day1` 剩余的文档整理面按白名单直接收口到 `main`

当前优先收口面：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\requirements.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\scene-build_handoff.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_0.md`

如果下面这批目录整理确实也是你这轮原本要交付的内容，就一并收掉：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\0.0.1剧情初稿\` 的删除/迁移
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\0.0阶段\` 的新增内容

如果你检查后发现：
- `0.0.1剧情初稿 -> 0.0阶段` 这批整理不是你这轮自己的交付
- 或里面混进了不属于 `spring-day1` 的内容
那你就不要硬提，只回执说明。

你这轮按下面顺序做：
1. 先确认 `83d809a9` 那批基础脊柱代码已经不在你的本轮白名单里
2. 只保留剩余文档整理面
3. 如果这批文档整理已经是最终交付面，就直接白名单提交到 `main`
4. 提交后立刻停下，不再顺手继续扩写

这轮不要做：
- 不要再改 `Assets/YYY_Scripts/Story/` 那批已进 `main` 的代码
- 不要进 Unity / MCP live 写
- 不要去改 `scene-build` worktree
- 不要再切 branch
- 不要顺手扩成新一轮剧情实现开发

只有命中下面情况你才停：
1. 你发现当前 dirty 里混进了不属于 `spring-day1` 的路径
2. 你发现自己又开始改 Story 代码，而不是收文档尾巴
3. 你需要改同一个高危 Scene / Prefab / 热脚本

回执不要再让项目经理手动建文件。
直接把最终回执追加到：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\001部分回执.md`

回执固定字段：
- 实际提交到 `main` 的路径
- 提交 SHA
- 当前 `git status` 是否 clean
- blocker_or_checkpoint
- 一句话摘要
```
