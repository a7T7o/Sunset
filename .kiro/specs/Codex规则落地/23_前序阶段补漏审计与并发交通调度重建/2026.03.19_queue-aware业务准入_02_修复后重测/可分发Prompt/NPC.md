# NPC｜queue-aware 业务准入 02｜修复后重测

```text
【queue-aware 业务准入｜NPC｜修复后重测】

生成本 prompt 时的 live 基线是：
- D:\Unity\Unity_learning\Sunset
- main
- clean
- occupancy = neutral
- HEAD = 659109c1

但你执行时必须以命令返回的 live preflight 为准，不得假设大厅仍空。

你的 continuation branch：
- codex/npc-roam-phase2-003

你本轮只允许做的事：
- 用修复后的 stable launcher 发起 request-branch
- 如果拿到 GRANTED / ALREADY_GRANTED，就 ensure-branch 进入 continuation branch
- 进入后只做只读核对：
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
  - `Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset`
- 不写业务代码
- 不写 tracked 回收卡
- 核对完成后立即 return-main

先执行：
1. powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action request-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003" -CheckpointHint "repair-retest-npc" -QueueNote "repair-retest-batch03"

分流规则：
- 如果返回 GRANTED 或 ALREADY_GRANTED：
  1. 执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action ensure-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003"
  2. 只做上面的只读核对，不做任何 tracked 写入
  3. 执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action return-main -OwnerThread "NPC"

- 如果返回 LOCKED_PLEASE_YIELD：
  - 不准执行 ensure-branch
  - 不准在 Sunset 仓库里写任何 tracked 文件
  - 不准修改收件卡或 memory_0.md
  - 只在聊天里按最小格式回执，然后停下等待

本轮聊天只回复：
- 已读取 prompt 路径
- request-branch: <结果>
- ensure-branch: <成功/未执行/失败>
- return-main: <成功/未执行/失败>
- 如果是 waiting，再补 ticket / queue_position
- 一句话摘要

本轮禁止：
- Unity / MCP / Play Mode
- Primary.unity
- GameInputManager.cs
- 在聊天里长篇复述本文件
```
