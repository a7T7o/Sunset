# Codex规则落地 - 活跃入口记忆

> 2026-04-10 起，旧超长母卷已归档到 [memory_7.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_7.md)。本卷只保留当前治理定位、活跃阶段入口和恢复点，不再承接长流水。

## 当前定位
- 本工作区只负责 `Sunset` 的安全治理、shared-root 事故恢复、live 规则入口和历史审计。
- 不再充当业务执行总线、运行时调度总线或所有线程回执的总堆场。

## 当前状态
- **最后校正**：2026-04-10
- **状态**：活跃卷已重建
- **当前活跃阶段**：
  - `25_vibecoding场景规范与main收口`
  - `26_memory主卷退场与活跃卷重建`

## 历史卷索引
- [memory_index.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_index.md)
- [memory_0.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_0.md)
- [memory_1.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_1.md)
- [memory_2.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_2.md)
- [memory_3.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_3.md)
- [memory_4.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_4.md)
- [memory_5.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_5.md)
- [memory_6.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_6.md)
- [memory_7.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_7.md)

## 当前稳定结论
- 当前唯一 live 规范层级始终以：用户裁定 -> `AGENTS.md` -> 当前规范快照 -> 命中的治理正文 为准。
- `CLI first, direct MCP last-resort`、`thread-state`、`用户可读汇报`、`No-Red 证据卡 v2` 仍是当前有效治理基线。
- 后续若出现新的 shared-root、分发、线程续工、停发裁定问题，应优先进入对应阶段目录，不再把细节倒回根主卷。
- 当 shared root 进入“大量 parked 线程 + 数百 dirty/untracked + 热文件仍脏”的收口期时，第一动作必须先做只读分层盘点：区分 `零功能风险垃圾`、`docs/memory`、`Editor/Test only`、`运行时/Scene/ProjectSettings`，不能直接为了清 status 盲删或盲提交通用大包。
- 如果当前 `sunset_mcp.py status` 落 `listener_missing`，就不能对运行时切片 claim `可安全提交/可打包`；此时最多只允许先推进不影响正常功能的垃圾清扫与分批提交规划。
- 2026-04-12 已把 shared-root 紧急清扫再收窄到最保守的一刀：
  - 只提交 `.gitignore` 对 `Save/`、`.codex` 临时日志、`Assets/__CodexEditModeScenes*` 和本地 `.codex/archives/` 的忽略；
  - 只删除当前运行生成的 `Save/`、两份 `.codex` 临时日志、`Assets/__CodexEditModeScenes*`；
  - `Assets/Screenshots/`、`Assets/Sprites/Temp000/`、`.codex/archives/` 当前一律不删，避免误伤验收证据、草图或人工归档。
  - 上述最小 safe cleanup 已单独提交为 `1a138a92`，没有吞并其他线程的 memory、scene 或业务改动。
- 2026-04-12 已把当前 shared-root 最大头 `Assets/000_Scenes` 安全收成独立 checkpoint：
  - 实际提交为 `3ee66fb4`；
  - 白名单只包含 `Artist.unity / Home.unity / Primary.unity / SampleScene.unity / SampleScene.unity.meta / Town.unity / 矿洞口.unity` 这 7 项；
  - 证明当前 scene 大头可以按“5 个现存 scene target + 2 个删除项 expected sync paths”独立收口，不必为了解决大头而顺手吞并其他业务根、memory 或 docs。
- `Ready-To-Sync` 对带删除项的 scene 批次会额外依赖 `expected_sync_paths` 是否包含删除目标；只把现存文件放进 `TargetPaths` 还不够，像 `SampleScene.unity(.meta)` 这种已删除项必须一并写进 `ExpectedSyncPaths`，否则会被 own-roots remaining dirty 规则拦下。
- 2026-04-12 已确认“Codex 仍爆卡”的第一真元凶不是 scene，而是 `.kiro/xmind-pipeline/node_modules` 这坨未跟踪依赖海：
  - `git status --porcelain=v1 -uall` 原先是 `2617` 条，其中 `.kiro/xmind-pipeline` 单根就占 `1861` 条，且 `node_modules` 占 `1825` 条；
  - 已把 `.kiro/xmind-pipeline/node_modules/` 加入 `.gitignore` 并清掉本地目录，最小提交为 `19e31c4c`；
  - 清完后总量已直接降到 `792` 条，说明这坨依赖海才是当前“显示没变、Codex 卡爆”的最大噪音源。
- 当前 remaining untracked 的新顺序已变成：
  - `.kiro/specs` = `108`
  - `Assets/100_Anim` = `86`
  - `Assets/Editor` = `59`
  - `Assets/Screenshots` = `58`
  - `Assets/Sprites` = `54`
  - `.kiro/xmind-pipeline` 仅剩 `36` 条源码/输出/测试文件，不再是第一大头。

## 当前恢复点
- 继续做 memory 治理、治理入口收口或规范修订时，优先进入：
  - [26_memory主卷退场与活跃卷重建](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/26_memory主卷退场与活跃卷重建/memory.md)
- 如果只是查旧治理事实、批次分发、典狱长裁定或历史事故过程，直接查 `memory_0..7.md` 与对应阶段目录。
- 如果后续继续 shared-root 清账，优先沿“先拿最大同根大头做独立 checkpoint”推进；`Assets/000_Scenes` 这一根已经单独收口，不要再把 scene 与 docs/memory 混成通用大包。
- 当前下一刀若继续以“让 Codex/VS Code 不再爆卡”为优先目标，应先处理 `.kiro/specs` 这一批 `108` 个未跟踪正文，再决定是否要碰 `Assets/Screenshots / Assets/Sprites / Assets/Editor`。

## 本卷纪律
- 根卷只保留：
  - 工作区定位
  - 当前活跃阶段
  - 稳定结论
  - 恢复点
- 任何新的详细治理过程、批次执行、分卷动作或事故复盘，都不得再回灌本卷。
