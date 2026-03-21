# NPC - 气泡位置碰撞体修正与下一步规划

```text
【恢复开发总控 01｜NPC｜气泡位置碰撞体修正与下一步规划】
你的 continuation branch：
- codex/npc-roam-phase2-003

这轮来自真实验收的反馈不是抽象意见，而是明确问题：
- 现在的 NPC 气泡仍然压在人物脸上，位置没有真正离开头顶
- 观感仍然怪，不够自然
- NPC 之间会互相穿透，玩家也可以直接穿过 NPC
- 碰撞体不能占满整个角色，应参考玩家的碰撞体思路，只保留下半身 / 透视合理的实体感

你这轮的目标是两件事：
- 优先修正“气泡位置 / 观感”和“NPC 碰撞体”问题
- 同时给出下一步最小推进规划，而不是只做眼前一刀

你的第一动作 MUST 是：
1. 执行：
   powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action request-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003" -CheckpointHint "npc-bubble-collider-followup" -QueueNote "recovery-control-01"

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
  4. 本轮先围绕两个问题推进：
     - 气泡必须真正离开脸部，形成“头顶上方 + 有留白 + 观感更自然”的位置与显隐
     - NPC 碰撞体必须变成“像玩家一样只保留合理实体感”的方案，不能继续整个人可穿透
  5. 如果你能在允许域内只靠脚本 / prefab / 配置改动完成真实 checkpoint，就直接做，不准只写分析。
  6. 如果你确认这轮修复离不开 Unity / MCP / 场景 live 观察 / prefab 现场调参：
     - 立刻停下，不准自行进入 Unity / MCP
     - 把 blocker_or_checkpoint 写成 `needs-unity-window`
     - 同时把 `next_step_plan` 写具体，至少写清：
       - 你要动哪些 prefab / controller / collider
       - 你要验证什么视觉或碰撞结果
       - 下一轮最小执行顺序
  7. 如果形成 checkpoint，执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action sync -Mode task -OwnerThread "NPC" -ScopeRoots "Assets/100_Anim/NPC","Assets/111_Data/NPC","Assets/222_Prefabs/NPC","Assets/Sprites/NPC","Assets/YYY_Scripts/Anim/NPC","Assets/YYY_Scripts/Controller/NPC","Assets/YYY_Scripts/Data/NPCRoamProfile.cs","Assets/Editor/NPCPrefabGeneratorTool.cs","Assets/Editor/NPCAutoRoamControllerEditor.cs",".kiro/specs/NPC",".codex/threads/Sunset/NPC" -IncludePaths "<按实际改动逐条填写>"
  8. sync 成功后立刻执行：
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
- next_step_plan
- blocker_or_checkpoint
- 一句话摘要

本轮禁止：
- Primary.unity
- 在未获明确窗口前自行进入 Unity / MCP / Play Mode
- 在 main 写 tracked 文件
- 长篇复述过程
```
