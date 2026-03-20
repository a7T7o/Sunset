# NPC - phase2 交付面收口

```text
【真实业务开发批次 03｜NPC｜phase2 交付面收口】

你的 continuation branch：
- codex/npc-roam-phase2-003

这轮不是 smoke，不是准入演习，也不是重复确认“main 缺 NPC 内容”。
这轮的唯一目标是：把 `codex/npc-roam-phase2-003` 收束成一个可交付的 NPC phase2 carrier。

你已知的 live 业务事实：
- 相对 `main`，当前 branch 的真实业务面主要集中在：
  - Assets/100_Anim/NPC/
  - Assets/111_Data/NPC/
  - Assets/222_Prefabs/NPC/
  - Assets/Sprites/NPC/*.meta
  - Assets/YYY_Scripts/Controller/NPC/
  - Assets/YYY_Scripts/Data/NPCRoamProfile.cs
  - Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs
  - Assets/Editor/NPCPrefabGeneratorTool.cs
  - Assets/Editor/NPCAutoRoamControllerEditor.cs
- 但 branch 里还混有不该进入最终交付面的噪音：
  - AGENTS.md
  - scripts/git-safe-sync.ps1
  - TD 镜像文档
  - 与 NPC 无关的治理 / workspace 漂移

你的第一动作 MUST 是：
1. 执行：
   powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action request-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003" -CheckpointHint "npc-phase2-deliverable-03" -QueueNote "dev-batch-03"

分流规则：

- 如果返回 GRANTED 或 ALREADY_GRANTED：
  1. 执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action ensure-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003"
  2. 进入 branch 后先执行：
     git diff --name-status main...HEAD
  3. 这轮允许保留 / 修改 / 提交的路径范围只有：
     - Assets/100_Anim/NPC/
     - Assets/111_Data/NPC/
     - Assets/222_Prefabs/NPC/
     - Assets/Sprites/NPC/
     - Assets/YYY_Scripts/Controller/NPC/
     - Assets/YYY_Scripts/Data/NPCRoamProfile.cs
     - Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs
     - Assets/Editor/NPCPrefabGeneratorTool.cs
     - Assets/Editor/NPCAutoRoamControllerEditor.cs
     - .kiro/specs/NPC/
     - .codex/threads/Sunset/NPC/memory_0.md
  4. 这轮必须完成的真实工作是：
     - 明确把 diff 分成“NPC 交付面”与“治理噪音”
     - 从当前 carrier 中清掉不属于 NPC phase2 最终交付面的噪音
     - 如果在允许范围内存在最小 glue fix 需求，顺手补齐
  5. 这轮禁止保留到最终交付面中的文件包括但不限于：
     - AGENTS.md
     - scripts/git-safe-sync.ps1
     - .kiro/specs/000_代办/
     - 非 NPC 的治理文档 / memory 漂移
     - 农田 / 导航 / 遮挡 / spring-day1 相关文件
  6. 这轮不允许再用“branch 里本来就有内容”作为空回执；你必须二选一：
     - 做出真实 carrier 清洗 / glue fix checkpoint
     - 或基于 live diff 明确证明“噪音已清空，当前 carrier 已可直接交付”
  7. 如果形成了最小可交付 checkpoint，执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action sync -Mode task -OwnerThread "NPC" -ScopeRoots "Assets/100_Anim/NPC","Assets/111_Data/NPC","Assets/222_Prefabs/NPC","Assets/Sprites/NPC","Assets/YYY_Scripts/Controller/NPC","Assets/YYY_Scripts/Data/NPCRoamProfile.cs","Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs","Assets/Editor/NPCPrefabGeneratorTool.cs","Assets/Editor/NPCAutoRoamControllerEditor.cs",".kiro/specs/NPC",".codex/threads/Sunset/NPC" -IncludePaths "<按实际改动逐条填写>"
  8. sync 成功后立刻执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action return-main -OwnerThread "NPC"
  9. 如果 live diff 证明你只需要给出“already-clean-carrier”结论且无需新增改动，也允许无 sync 直接 return-main，但结论必须是基于 diff 分类后的写实判断，不能偷懒复述旧结论。

- 如果返回 LOCKED_PLEASE_YIELD：
  - 不准 ensure-branch
  - 不准在 main 写 tracked 文档
  - 读取 DRAFT_FILE_HINT
  - 在 Draft 写下这轮的最小交付面分类和待清噪音列表后等待唤醒

本轮聊天只回复：
- 已读 prompt 路径
- request-branch: <结果>
- ensure-branch: <成功 / 未执行 / 失败>
- hotfile_lock: not-needed
- sync: <成功 / 未执行 / 失败>
- return-main: <成功 / 未执行 / 失败>
- changed_paths: <路径列表 / none>
- blocker_or_checkpoint: <一句话>
- 一句话摘要

本轮禁止：
- Primary.unity
- Unity / MCP / Play Mode
- 在 main 写 tracked 文档
- 长篇复述过程
```
