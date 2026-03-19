# NPC - phase2 集成清洗批次

```text
【真实开发准入批次 02｜NPC｜phase2 集成清洗】
生成本 prompt 时的 live 快照：
- D:\Unity\Unity_learning\Sunset
- main
- clean
- HEAD = 3247541f

但你执行时必须以 live preflight 为准。

你的 continuation branch：
- codex/npc-roam-phase2-003

本轮真实目标：
- 不再重复“main 缺 NPC phase2 内容”的根因排查
- 直接把 `codex/npc-roam-phase2-003` 收束成可交付的 phase2 carrier
- 优先清掉 branch 中与 NPC phase2 无关的漂移，只保留 NPC 可交付范围

你已知的 branch 漂移事实：
- 相对 main，当前 branch 除了 NPC phase2 相关资产，还混有：
  - AGENTS.md
  - scripts/git-safe-sync.ps1
  - 部分 thread/workspace 文档漂移
- 这些不应继续混在 NPC 最终交付里

你的第一动作：
1. 执行：
   powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action request-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003" -CheckpointHint "npc-phase2-integrate-02" -QueueNote "dev-batch-02"

分流规则：

- 如果返回 GRANTED 或 ALREADY_GRANTED：
  1. 执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action ensure-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003"
  2. 进入 branch 后，先只读执行：
     git diff --name-status main...HEAD
  3. 本轮只允许保留 / 修改 / 交付这些 NPC phase2 范围：
     - Assets/100_Anim/NPC/
     - Assets/111_Data/NPC/
     - Assets/222_Prefabs/NPC/
     - Assets/Sprites/NPC/
     - Assets/YYY_Scripts/Controller/NPC/
     - Assets/YYY_Scripts/Data/NPCRoamProfile.cs
     - Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs
     - Assets/Editor/NPCPrefabGeneratorTool.cs
     - Assets/Editor/NPCAutoRoamControllerEditor.cs
  4. 本轮应优先做的真实工作：
     - 把 branch 中与上述 NPC 范围无关的漂移移出最终交付面
     - 如有必要，在 NPC 允许域内补最小 glue fix，使 phase2 carrier 自洽
  5. 本轮禁止保留到最终交付中的文件：
     - AGENTS.md
     - scripts/git-safe-sync.ps1
     - 非 NPC 的治理文档 / memory 漂移
     - 农田 / 导航 / 遮挡 作用域
  6. 如果你形成了最小可交付 checkpoint，执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action sync -Mode task -OwnerThread "NPC" -ScopeRoots "Assets/100_Anim/NPC","Assets/111_Data/NPC","Assets/222_Prefabs/NPC","Assets/Sprites/NPC","Assets/YYY_Scripts/Controller/NPC","Assets/YYY_Scripts/Data/NPCRoamProfile.cs","Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs","Assets/Editor/NPCPrefabGeneratorTool.cs","Assets/Editor/NPCAutoRoamControllerEditor.cs" -IncludePaths "<按实际改动逐条填写>"
  7. sync 成功后立刻：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action return-main -OwnerThread "NPC"
  8. 如果你发现本轮无需新增 NPC 作用域 tracked 改动，但已能明确给出“哪些 branch 漂移必须在后续 merge 策略中排除”，也允许无 sync 退场，但要把 blocker_or_checkpoint 写成 merge-ready / merge-noise 结论

- 如果返回 LOCKED_PLEASE_YIELD：
  - 不准 ensure-branch
  - 不准写 tracked 文件
  - 读取 DRAFT_FILE_HINT
  - 在 Draft 写最小恢复点后等待

本轮聊天只回复：
- 已读 prompt 路径
- request-branch: <结果>
- ensure-branch: <成功 / 未执行 / 失败>
- sync: <成功 / 未执行 / 失败>
- return-main: <成功 / 未执行 / 失败>
- changed_paths: <路径列表 / none>
- blocker_or_checkpoint: <一句话>
- 一句话摘要

本轮禁止：
- Primary.unity
- GameInputManager.cs
- Unity / MCP / Play Mode
- 在 main 写 tracked 文档
- 长篇叙述过程
```
