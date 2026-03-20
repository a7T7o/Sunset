# NPC - phase2 真实业务推进与 main-ready 核验

```text
【真实业务开发批次 04｜NPC｜phase2 真实业务推进与 main-ready 核验】

你的 continuation branch：
- codex/npc-roam-phase2-003

这轮不是 smoke，不是 carrier 清洗复述，也不是再次确认“资源在不在”。
这轮的唯一目标是：在 NPC phase2 允许范围内做出一个真实 checkpoint，并明确它对 `main` 的真实影响。

你已知的 live 业务事实：
- `main` 已恢复 `Primary.unity` 当前依赖的 NPC prefab / anim / profile / runtime 基础链路
- 上一次事故不是 `farm` 覆盖 NPC，而是“只看 carrier，没有追问 main-ready”
- 所以这轮你必须明确回答：你这次 checkpoint 会不会再次制造 `main` 对 branch-only 资产的依赖缺口

你的第一动作 MUST 是：
1. 执行：
   powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action request-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003" -CheckpointHint "npc-phase2-real-dev-04" -QueueNote "dev-batch-04"

分流规则：

- 如果返回 GRANTED 或 ALREADY_GRANTED：
  1. 执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action ensure-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003"
  2. 进入 branch 后先执行：
     git diff --name-status main...HEAD
  3. 本轮允许保留 / 修改 / 提交的路径范围只有：
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
  4. 这轮必须完成的真实工作是：
     - 先把 live diff 分成“NPC 交付面”与“噪音”
     - 然后在允许范围内至少做出一个真实 checkpoint，优先二选一：
       - runtime 行为链：`NPCAutoRoamController` / `NPCBubblePresenter` / `NPCRoamProfile` / `NPCAnimController`
       - editor / 生成链：`NPCPrefabGeneratorTool` / `NPCAutoRoamControllerEditor`
     - 不准只回“branch 里本来就有内容”
  5. 本轮禁止保留到最终交付面中的文件包括但不限于：
     - AGENTS.md
     - scripts/git-safe-sync.ps1
     - 非 NPC 的治理文档 / memory 漂移
     - 农田 / 导航 / 遮挡 / spring-day1 相关文件
  6. 本轮必须显式回答两件事：
     - `carrier_ready`
     - `main_ready`
     其中：
     - `carrier_ready = yes` 只表示 branch 内这轮 checkpoint 已收束
     - `main_ready = yes` 只允许在“你本轮 return-main 后，当前 main 不会因为 NPC branch-only 资产而断链”时填写
     - 如果 `main_ready = no`，必须把 `main_ready_basis` 写成具体依赖链，而不是抽象话
  7. 如果形成了真实 checkpoint，执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action sync -Mode task -OwnerThread "NPC" -ScopeRoots "Assets/100_Anim/NPC","Assets/111_Data/NPC","Assets/222_Prefabs/NPC","Assets/Sprites/NPC","Assets/YYY_Scripts/Anim/NPC","Assets/YYY_Scripts/Controller/NPC","Assets/YYY_Scripts/Data/NPCRoamProfile.cs","Assets/Editor/NPCPrefabGeneratorTool.cs","Assets/Editor/NPCAutoRoamControllerEditor.cs",".kiro/specs/NPC",".codex/threads/Sunset/NPC" -IncludePaths "<按实际改动逐条填写>"
  8. sync 成功后立刻执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action return-main -OwnerThread "NPC"
  9. 只有在 live diff 证明“当前 branch 在允许范围内已经没有剩余业务差量，且本轮唯一新增成果只是 main-ready 结论文档”时，才允许 docs-only checkpoint；否则 `changed_paths` 不得为 `none`

- 如果返回 LOCKED_PLEASE_YIELD：
  - 不准 ensure-branch
  - 不准在 main 写 tracked 文档
  - 读取 DRAFT_FILE_HINT
  - 在 Draft 里写下“本轮目标 checkpoint + 你对 carrier/main-ready 的初判”后等待唤醒

本轮聊天只回复：
- 已读 prompt 路径
- request-branch: <结果>
- ensure-branch: <成功 / 未执行 / 失败>
- hotfile_lock: not-needed
- sync: <成功 / 未执行 / 失败>
- return-main: <成功 / 未执行 / 失败>
- carrier_ready: <yes / no>
- main_ready: <yes / no>
- main_ready_basis: <一句话>
- changed_paths: <路径列表 / none>
- blocker_or_checkpoint: <一句话>
- 一句话摘要

本轮禁止：
- Primary.unity
- Unity / MCP / Play Mode
- 在 main 写 tracked 文档
- 长篇复述过程
```
