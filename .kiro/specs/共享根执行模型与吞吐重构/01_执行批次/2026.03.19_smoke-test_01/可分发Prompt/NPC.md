# NPC｜执行层 smoke test 01

```text
【执行层 smoke test 01｜NPC】

生成本 prompt 时的 live 快照：
- D:\Unity\Unity_learning\Sunset
- main
- clean
- HEAD = 912adfca

但你执行时必须以命令返回的 live preflight 为准。

你的 continuation branch：
- codex/npc-roam-phase2-003

你本轮只允许做的事：
- 用 stable launcher 发起 request-branch
- 如果拿到 GRANTED / ALREADY_GRANTED，就 ensure-branch 进入 continuation branch
- 进入后只做只读核对：
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
  - `Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset`
- 不写业务代码
- 不写 tracked 回执
- 核对完成后立即 return-main

先执行：
1. powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action request-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003" -CheckpointHint "smoke-npc-readonly" -QueueNote "smoke-test-01"

分流规则：
- 如果返回 GRANTED 或 ALREADY_GRANTED：
  1. 执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action ensure-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003"
  2. 只做上面的只读核对，不做任何 tracked 写入
  3. 执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action return-main -OwnerThread "NPC"
  4. 读取 return-main 输出里的：
     - POST_RETURN_EVIDENCE_MODE
     - POST_RETURN_NEXT_ACTION
     本轮只在聊天里汇报，不要写 tracked 文件

- 如果返回 LOCKED_PLEASE_YIELD：
  - 不准执行 ensure-branch
  - 不准在 Sunset 仓库里写任何 tracked 文件
  - 读取脚本输出里的 `DRAFT_FILE_HINT`
  - 用 apply_patch 在该路径写一个最小 draft note，内容只保留：
    - owner_thread
    - target_branch
    - ticket
    - queue_position
    - next_checkpoint
  - 写完后停下等待，不要继续乱试

本轮聊天只回复：
- 已读取 prompt 路径
- request-branch: <结果>
- ensure-branch: <成功/未执行/失败>
- return-main: <成功/未执行/失败>
- 如果是 waiting，再补 ticket / queue_position / draft_file
- 如果 return-main 成功，再补 post_return_evidence_mode
- 一句话摘要

本轮禁止：
- Unity / MCP / Play Mode
- Primary.unity
- GameInputManager.cs
- 任何 tracked 写入
- 在聊天里长篇复述本文件
```
