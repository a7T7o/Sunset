# NPC 回收单

## 固定信息
- owner_thread: `NPC`
- continuation_branch: `codex/npc-roam-phase2-003 @ 7385d123`
- 当前阶段工作区: `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收`

## 阶段一：线程回收区（由 NPC 填写）
- 回写时间: `2026-03-18 22:26:14 +08:00`
- cwd: `D:\Unity\Unity_learning\Sunset`
- branch: `main`
- HEAD: `14838753b4ae9b09b2146b92fb3bfdc9ac82b2a0`
- git status --short --branch: `## main...origin/main`
- 已读取文档:
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-occupancy.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-hot-zones.md`
- 是否确认 continuation branch 仍为 `codex/npc-roam-phase2-003`: `是，当前 live 指向 7385d1236d0b85c191caff5c5c19b08678d1cf80`
- 下一轮若恢复真实开发，是否会碰 Unity / MCP / Play Mode / A 类热文件:
  - `Unity / MCP`: `可能会碰，但当前 current_claim = none 仅代表无人声明占用，不代表已可直接写；恢复开发前仍需 live verify`
  - `Play Mode`: `本轮未进入；若阶段二需要验收，必须按单实例规则进入并在完成后先退回 Edit Mode`
  - `A 类热文件`: `当前 active 锁目录为空，但热区定义仍包含 Primary.unity 与 GameInputManager.cs；若后续要碰，必须先查锁/获锁`
- 本轮最小 checkpoint: `阶段二获批后，先 grant -> ensure-branch 到 codex/npc-roam-phase2-003，再做最小 live 基线复核，不直接扩写业务`
- 当前阻断点（若有）: `当前无硬阻断，但 branch_grant_state = none，且 Unity/MCP 仍是 must-verify-live，所以本轮只通过阶段一，不可直接越级进入真实写入`

## 阶段二：线程回收区（由 NPC 填写）
- 回写时间:
- grant 是否成功:
- ensure-branch 是否成功:
- 当前 branch:
- 当前 HEAD:
- 本轮最小 checkpoint:
- 是否进入 Unity / MCP / Play Mode:
- 若进入，是否已退回 Edit Mode:
- 是否碰热文件 / 热区:
- 若碰，是否已先查锁:
- 当前阻断点（若有）:

## 治理裁定区（由 Codex规则落地 填写）
- 阶段一裁定: `通过。shared root 只读 preflight 合规，continuation branch 仍与 live 事实一致，本轮无越级写入。`
- 阶段二裁定: `准入候选，需串行排队，不自动抢占下一写入槽位。若进入阶段二，只允许先做最小 live 基线复核，不直接扩写业务。`
- 后续动作: `继续保持只读，等待治理线程按顺序单发阶段二 prompt；若后续触及 Unity / MCP / Play Mode 或 A 类热文件，必须先做 live verify 并先查锁。`
