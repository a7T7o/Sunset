# NPC｜queue-aware 业务准入 01

```text
【queue-aware 业务准入｜NPC｜批次 01】

当前 shared root live 基线：
- D:\Unity\Unity_learning\Sunset
- main
- clean
- occupancy = neutral
- queue runtime = empty

你本轮裁决状态：
- resume-new-feature

你的 continuation branch：
- codex/npc-roam-phase2-003

你本轮唯一允许的最小 checkpoint：
- 进入 continuation branch 后，只做 NPC 主线的最小 live 基线复核与下一 checkpoint 边界固化
- 本轮不默认进入 Unity / MCP / Play Mode
- 本轮不默认碰 A 类热文件

请先在 shared root 执行：
1. powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action request-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003" -CheckpointHint "npc-baseline-recheck" -QueueNote "batch02"

分流规则：
- 如果返回 `GRANTED` 或 `ALREADY_GRANTED`
  - 再执行：
    powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action ensure-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003"
- 如果返回 `LOCKED_PLEASE_YIELD`
  - 不准执行 ensure-branch
  - 立刻把等待态写进你的 memory_0.md
  - 再把结果写进回收卡后停止

如果成功进入分支：
- 只做本轮最小 checkpoint
- checkpoint 完成后，用显式白名单做 task sync
- 然后执行：
  powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action return-main -OwnerThread "NPC"

完整结果请回写到：
D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\线程回收\NPC.md

只填写：
- `## 本轮回收区（由 NPC 填写）`

本轮禁止：
- 禁止进入 Unity / MCP / Play Mode
- 禁止碰 Primary.unity
- 禁止碰 GameInputManager.cs
- 禁止在聊天里长篇复述回收卡正文

聊天里只回复：
- 已回写文件路径
- request-branch / ensure-branch / return-main 是否成功
- 一句话摘要
```
