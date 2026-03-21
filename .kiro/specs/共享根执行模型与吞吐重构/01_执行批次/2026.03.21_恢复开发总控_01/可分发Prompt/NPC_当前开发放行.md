# NPC - 当前开发放行

```text
【恢复开发总控 01｜NPC｜当前开发放行】
你的 continuation branch：
- codex/npc-roam-phase2-003

你当前属于 shared root 串行开发位。你的目标是：
- 在 NPC 允许域内继续推进一个真实业务 checkpoint
- 明确回答本轮 carrier_ready / main_ready

你的第一动作 MUST 是：
1. 执行：
   powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action request-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003" -CheckpointHint "npc-current-dev" -QueueNote "recovery-control-01"

分流规则：
- 如果返回 GRANTED 或 ALREADY_GRANTED：
  1. 执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action ensure-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003"
  2. 进入 branch 后先执行：
     git diff --name-status main...HEAD
  3. 本轮允许域只限：
     - Assets/100_Anim/NPC/
     - Assets/111_Data/NPC/
     - Assets/222_Prefabs/NPC/
     - Assets/Sprites/NPC/
     - Assets/YYY_Scripts/Anim/NPC/
     - Assets/YYY_Scripts/Controller/NPC/
     - Assets/YYY_Scripts/Data/NPCRoamProfile.cs
     - Assets/Editor/NPCPrefabGeneratorTool.cs
     - Assets/Editor/NPCAutoRoamControllerEditor.cs
     - .kiro/specs/NPC/
     - .codex/threads/Sunset/NPC/memory_0.md
  4. 本轮必须做出一个真实 checkpoint，不准只回答“branch 里本来就有内容”
  5. 如果你发现这轮必须进入 Unity / MCP、Primary.unity 或其他主场景 live 验证：
     - 立刻停下
     - 不准自行升级范围
     - 把 blocker_or_checkpoint 写成 needs-unity-window
  6. 如果形成 checkpoint，执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action sync -Mode task -OwnerThread "NPC" -ScopeRoots "Assets/100_Anim/NPC","Assets/111_Data/NPC","Assets/222_Prefabs/NPC","Assets/Sprites/NPC","Assets/YYY_Scripts/Anim/NPC","Assets/YYY_Scripts/Controller/NPC","Assets/YYY_Scripts/Data/NPCRoamProfile.cs","Assets/Editor/NPCPrefabGeneratorTool.cs","Assets/Editor/NPCAutoRoamControllerEditor.cs",".kiro/specs/NPC",".codex/threads/Sunset/NPC" -IncludePaths "<按实际改动逐条填写>"
  7. sync 成功后立刻执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action return-main -OwnerThread "NPC"

- 如果返回 LOCKED_PLEASE_YIELD：
  - 不准 ensure-branch
  - 不准在 main 写 tracked 文件
  - 草稿只准写进 Draft

聊天只回复：
- 已读 prompt 路径
- request-branch
- ensure-branch
- hotfile_lock: not-needed
- sync
- return-main
- carrier_ready
- main_ready
- main_ready_basis
- changed_paths
- blocker_or_checkpoint
- 一句话摘要

本轮禁止：
- Primary.unity
- Unity / MCP / Play Mode
- 在 main 写 tracked 文件
- 长篇复述过程
```
