# NPC - 真实开发准入批次 01

```text
【真实开发准入批次 01｜NPC】
生成本 prompt 时的 live 快照：
- D:\Unity\Unity_learning\Sunset
- main
- clean
- HEAD = 24dbb37c

但你执行时必须以命令返回的 live preflight 为准。

你的 continuation branch：
- codex/npc-roam-phase2-003

你的本轮目标：
- 处理用户最新明确点名的 NPC 主线问题：
  - 场景 NPC 报错
  - meta 丢失 / 资源链异常疑点
- 本轮优先级是“先恢复当前阻断，再谈扩写”，不要顺手扩展到新功能。

你的本轮第一动作：
1. 先执行：
   powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action request-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003" -CheckpointHint "npc-real-repair-01" -QueueNote "dev-batch-01"

分流规则：

- 如果返回 GRANTED 或 ALREADY_GRANTED：
  1. 执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action ensure-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003"
  2. 进入 branch 后，本轮只做最小真实修复闭环：
     - 先定位当前 NPC 报错 / meta 缺失的最短根因
     - 如果根因明确且落在 NPC 自身作用域内，就做最小修复
     - 形成一个最小 checkpoint 后立刻同步并归还 shared root
  3. 本轮允许优先触碰的作用域：
     - Assets/YYY_Scripts/Controller/NPC/
     - Assets/111_Data/NPC/
     - Assets/222_Prefabs/NPC/
     - Assets/Sprites/NPC/
     - Assets/Animations/NPC/
     - Assets/Editor/NPCPrefabGeneratorTool.cs
     - 以及对应 .meta
  4. 本轮禁止直接写入这些高风险对象：
     - Assets/000_Scenes/Primary.unity
     - A 类热文件
     - 农田 / 导航 / 遮挡 无关作用域
  5. 如果你的最小修复只落在允许作用域内，执行 task sync：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action sync -Mode task -OwnerThread "NPC" -ScopeRoots "Assets/YYY_Scripts/Controller/NPC","Assets/111_Data/NPC","Assets/222_Prefabs/NPC","Assets/Sprites/NPC","Assets/Animations/NPC","Assets/Editor/NPCPrefabGeneratorTool.cs" -IncludePaths "<按实际改动逐条填写>"
  6. sync 成功后立刻执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action return-main -OwnerThread "NPC"
  7. 如果你最后没有形成任何 tracked 改动，只允许：
     - 汇报根因 / 阻断
     - 直接 return-main

- 如果返回 LOCKED_PLEASE_YIELD：
  - 不准执行 ensure-branch
  - 不准在 Sunset 仓库里写任何 tracked 文件
  - 读取脚本输出里的 DRAFT_FILE_HINT
  - 用 apply_patch 在该路径写最小 Draft，内容只保留：
    - owner_thread
    - target_branch
    - ticket
    - queue_position
    - next_checkpoint
    - suspected_root_cause
  - 写完后停止等待，不要继续乱试

- 如果你发现真正需要动：
  - Primary.unity
  - GameInputManager.cs
  - 其他 A 类热文件
  那么本轮不要硬写。请把本轮结论定性为 hot-file blocked，保留证据后 return-main。

持槽纪律：
- shared root 持槽窗口按“短事务”执行，目标是快进快出
- 不要把长时间只读分析、复盘、文档补写塞进持槽期

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
- Unity / MCP / Play Mode 的无边界扩张使用
- 修改 Primary.unity
- 修改 A 类热文件
- 修改农田 / 导航 / 遮挡 作用域
- 在 main 上写 tracked memory / 回收卡 / 治理文档
- 在聊天里长篇复述过程
```
