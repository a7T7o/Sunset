# NPC｜当前版本更新并继续直接开发

```text
当前版本更新，你这条线不要再停在“上一刀已收口完成”的观望态，直接继续做 NPC 自己的下一步。

当前唯一真实基线：
- `D:\Unity\Unity_learning\Sunset @ main`
- 默认模型：`main-only + whitelist-sync + exception-escalation`
- 你完成一刀后，默认自己白名单提交自己的 checkpoint

你先完整回忆并汇报：
1. 你上一刀 main-only 已经真实收了哪些 NPC 文件。
2. 当前剩余的 NPC 自己的问题还有哪些。
3. 哪些问题应该继续由你做，哪些必须明确交给导航线程。

你这轮继续只认 NPC 自己的代码范围，不越界去做导航核心。
当前优先级：
1. 气泡表现继续收：
   - 尾巴必须明显朝下指向 NPC
   - 尾巴单独轻微跳动
   - 气泡和文字适度放大到清晰可读
2. NPC 自身体感继续收：
   - NPC 不应轻易被玩家顶开
   - 只从 NPC 自己的 collider / rigidbody / movement-consumer 范围内处理
3. 为后续导航成果预留干净接入口：
   - 不在这轮自己发明动态避障系统

明确边界：
- 你认领：
  - `NPCBubblePresenter`
  - NPC prefab 默认参数
  - NPC 自身 collider / rigidbody / movement-consumer
- 你不认领：
  - `NavGrid2D` 动态障碍
  - 玩家自动导航绕移动 NPC
  - NPC/NPC 会车礼让核心

只有命中下面情况你才停：
1. 撞到同一个高危目标。
2. 你要做 Unity / MCP live 写，但已经有人在写。
3. 你把 NPC 现场写坏了，需要先收口。

直接做到你这轮承诺的下一个 NPC checkpoint 再停。

回执直接追加到：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\001部分回执.md`

聊天只回这些字段：
- 当前在改什么
- 上次 checkpoint / 当前恢复点
- 剩余任务清单
- 本轮下一步最多推进到哪里
- changed_paths
- 是否触碰高危目标
- 是否需要 Unity / MCP live 写
- 当前是否可以直接提交到 `main`
- blocker_or_checkpoint
- 一句话摘要
```
