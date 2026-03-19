# 2026.03.19 queue-aware业务准入 01 memory

## 2026-03-19｜NPC 线程 batch02 准入执行
- 当前主线目标：在 shared root `main + clean + neutral` 的前提下，按 queue-aware 准入恢复业务线程进入 continuation branch 的最小 checkpoint。
- 本轮对象：`NPC`，目标分支 `codex/npc-roam-phase2-003`，准入目标是“最小 live 基线复核与下一 checkpoint 边界固化”，不进入 Unity / MCP / Play Mode，不碰 `Primary.unity` 与 `GameInputManager.cs`。
- 本轮执行：launcher 两次调用都因 `request-branch 必须提供 -BranchName` 失败；随后改用 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 直接执行，成功得到 `STATUS: GRANTED` 并完成 `ensure-branch` 进入 `codex/npc-roam-phase2-003 @ 7385d1236d0b85c191caff5c5c19b08678d1cf80`。
- 已证实事实：NPC 关键基线文件仍在位，包括 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`、`Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`、`Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset`、`Assets/222_Prefabs/NPC/001.prefab~003.prefab` 与 `Assets/Editor/NPCPrefabGeneratorTool.cs`。
- 本轮恢复点：完成记忆与回执固化后做一次显式白名单 task sync，再执行 `return-main` 归还 shared root。

## 2026-03-19｜NPC batch02 收口前固化
- live 现场已再次复核：当前 shared root 为 `D:\Unity\Unity_learning\Sunset @ codex/npc-roam-phase2-003 @ 7385d1236d0b85c191caff5c5c19b08678d1cf80`，`git status --short --branch` 仅见 occupancy runtime dirty 与本批次治理目录未跟踪文件。
- launcher `C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1` 在本轮 `request-branch` 两次都报 `request-branch 必须提供 -BranchName。`，已证实是参数转发异常；实际准入改由 canonical `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 完成。
- canonical `request-branch` 已成功得到 `STATUS: GRANTED`，随后 `ensure-branch` 成功消费 shared root 租约并进入 `codex/npc-roam-phase2-003`；本轮仍严格停留在“最小 live 基线复核”，未进入 Unity / MCP / Play Mode，也未碰 `Primary.unity` 与 `GameInputManager.cs`。
- 已证实 NPC 关键基线仍在位：`Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`、`Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`、`Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset`、`Assets/222_Prefabs/NPC/001.prefab`、`Assets/222_Prefabs/NPC/002.prefab`、`Assets/222_Prefabs/NPC/003.prefab`、`Assets/Editor/NPCPrefabGeneratorTool.cs`。
- queue runtime 当前存在一处需要写实记录的异常：`active/shared-root-queue.lock.json` 中 NPC 的 `ticket=1` 被记为 `cancelled / reconciled-missing-grant`，但 `shared-root-branch-occupancy.md` 仍登记 `owner_thread=NPC`、`lease_state=task-active`；本轮只记录，不擅自修 queue runtime。
- 本轮恢复点：先按显式白名单完成 task sync，再补回执卡并执行 `return-main` 归还 shared root。

## 2026-03-19｜NPC batch02 sync 阻断写实
- 按本轮白名单对 `.kiro/specs/Codex规则落地/23_前序阶段补漏审计与并发交通调度重建/2026.03.19_queue-aware业务准入_01/memory.md`、`.kiro/specs/NPC/1.0.0初步规划/memory.md`、`.kiro/specs/NPC/memory.md`、`.codex/threads/Sunset/NPC/memory_0.md` 进行 task preflight 后，脚本明确阻断继续同步。
- 阻断原因不是 NPC 白名单本身，而是 shared root 当前仍存在未纳入本轮白名单的 remaining dirty：`M .kiro/specs/Codex规则落地/memory.md`、`?? .codex/threads/Sunset/遮挡检查/memory_1.md`、`?? .kiro/specs/Codex规则落地/23_前序阶段补漏审计与并发交通调度重建/2026.03.19_queue-aware业务准入_01/memory_1.md`、`?? .kiro/specs/Codex规则落地/23_前序阶段补漏审计与并发交通调度重建/2026.03.19_queue-aware业务准入_01/线程回收/遮挡检查.md`、`?? .kiro/specs/Codex规则落地/memory_1.md`。
- 结论：本轮 request-branch 与 ensure-branch 已成功，但 task sync 与 return-main 需要等待 shared root 的无关 remaining dirty 先被清尾或隔离，当前不能伪装成 NPC 已完成归还。
## 2026-03-19｜queue-aware 业务准入 01：导航检查进入 waiting

**用户目标**
- 领取 `导航检查` 专属 prompt，并按本批次要求执行 `request-branch -> ensure-branch -> 固定回收`。

**本轮现场**
- live cwd：`D:\Unity\Unity_learning\Sunset`
- live branch：`codex/npc-roam-phase2-003`
- live HEAD：`7385d1236d0b85c191caff5c5c19b08678d1cf80`
- `git status --short --branch`：`## codex/npc-roam-phase2-003...origin/codex/npc-roam-phase2-003`，并带 `M .kiro/locks/shared-root-branch-occupancy.md`

**已完成**
- 读取 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\可分发Prompt\导航检查.md`
- 先用稳定 launcher 按 prompt 原文执行 `request-branch`，实测暴露 optional 参数未转发：`-BranchName` 在 launcher 到 canonical script 之间丢失，返回 `request-branch 必须提供 -BranchName。`
- 为避免退回仓库 working tree 脚本，改用 `main:scripts/git-safe-sync.ps1` 的手工等价 canonical 执行再次申请。
- canonical `request-branch` 返回：
  - `STATUS: LOCKED_PLEASE_YIELD`
  - `TICKET: 3`
  - `QUEUE_POSITION: 2`
  - `REASON: 当前 live 分支是 'codex/npc-roam-phase2-003'，只有 main 大厅才能发放分支租约。`
- 已按 prompt 停止后续动作，未执行 `ensure-branch` / `task sync` / `return-main`，并把结果写入固定回收卡：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\线程回收\导航检查.md`

**关键判断**
- 这次阻塞不是 `导航检查` 自身代码问题，而是 shared root 已被 `NPC` 线程占用，当前不具备 `main + neutral` 发租约前提。
- 稳定 launcher 的 optional 参数转发存在真实缺口，至少影响 `request-branch` 这类需要 `-BranchName` 的 live 准入动作。

**恢复点 / 下一步**
- 等 shared root 回到 `main + neutral` 且治理层重新唤醒 `导航检查`。
- 下一次继续时仍以 `codex/navigation-audit-001` 为 continuation branch，先做 `NavGrid2D / PlayerAutoNavigator` 的首个非热文件 checkpoint，不碰 `GameInputManager.cs`、`Primary.unity`，也不进入 Unity / MCP / Play Mode。
