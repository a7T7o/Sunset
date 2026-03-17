# 历史编码巡检与治理边界收紧 - 开发记忆

## 模块概述
- 本阶段负责处理 Sunset 当前还残留的乱码焦虑、编码疑点和治理执行边界模糊问题。
- 核心原则不是“看到乱码就重写”，而是先做真伪分类，再决定修复、重建或降级。

## 当前状态
- **完成度**: 5%
- **最后更新**: 2026-03-16
- **状态**: 进行中

## 会话记录

### 会话 1 - 2026-03-16

**用户需求**:
> 历史乱码全面开始；现在 `memory`、历史乱码、治理执行边界这几件事都可以直接干到底，只要不动 `npc` 和 `farm` 就行。

**完成任务**:
1. 新建 `07_历史编码巡检与治理边界收紧/` 阶段目录。
2. 固定本阶段范围：编码巡检、历史错码分类、治理白名单边界复核。

**修改文件**:
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\07_历史编码巡检与治理边界收紧\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\07_历史编码巡检与治理边界收紧\memory.md`

**关键决策**:
- 历史乱码处理与治理边界收紧属于新的治理阶段，不与 `memory` 分卷阶段混写。
- 本阶段继续避开 `NPC` / `farm` 的业务现场，只处理治理层与文档层对象。

**遗留问题**:
- [ ] 仍需完成全量可疑对象扫描。
- [ ] 仍需形成第一版“真乱码 / 假乱码 / 历史忽略”分类表。

### 会话 2 - 2026-03-16（第一版编码分类表与执行边界口径落盘）

**用户需求**:
> 继续做，不要停；你自己把历史乱码、规则边界和现行入口收短这几条线直接往下做，不要碰 `NPC` 和 `farm` 的业务现场。

**完成任务**:
1. 对治理层与现行入口层做轻量编码证据审计，比较了 `UTF8` / `Default` 两种读法，并抽查关键文件字节。
2. 确认 `skills-governor`、`sunset-workspace-router`、`.kiro/steering/workspace-memory.md`、`农田系统/memory.md` 当前都属于 `healthy-terminal-noise`，不是文件本体损坏。
3. 新建 `Sunset编码审计分类表_2026-03-16.md`，把 `healthy-terminal-noise`、`convertible-in-place`、`historical-ignore` 的第一版分类写死。
4. 复核 `git-safe-sync.ps1` 当前真实边界，确认 `governance` 默认自动白名单已经收紧到 `.gitattributes`、`.gitignore`、`AGENTS.md`、`scripts/`、`.kiro/steering/`、`.kiro/hooks/`。
5. 将以上结论同步进现行状态说明、Git 规则、L5 总览、基础规则与兼容路由页，并把旧 `03-13` 总索引降级为历史路由页，新增 `Sunset现行入口总索引_2026-03-16.md` 作为新的当前总索引。

**修改文件**:
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\07_历史编码巡检与治理边界收紧\Sunset编码审计分类表_2026-03-16.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\07_历史编码巡检与治理边界收紧\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前唯一状态说明_2026-03-16.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset Git系统现行规则与场景示例_2026-03-16.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset L5开工清单与治理续办总览_2026-03-16.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset现行入口总索引_2026-03-16.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\文档重组总索引_2026-03-13.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\基础规则与执行口径.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\tasks.md`

**关键决策**:
- 当前活文档层不再把 `03-13` 命名文件继续当作现行总索引；旧索引改成历史路由页，新的当前索引单独承接。
- 编码治理的当前重点不是“全面重写活文档”，而是“维持 UTF-8、修实锤对象、把历史归档降级忽略”。
- `git-safe-sync.ps1` 当前的真实风险已经从“白名单过宽”转为“有人仍误以为 governance 会自动兜住 `.kiro/specs/...` 与 `.codex/threads/...`”。

**验证结果**:
- `skills-governor\SKILL.md` 已通过 `UTF8` / `Default` 对照和 `Format-Hex` 抽查，确认文件本体健康。
- 现行状态说明、Git 规则、L5 总览和基础规则口径已对齐脚本真实行为。
- `Skills和MCP` 已从 L5 总览的唤醒 Prompt 区彻底移除，只保留归档身份说明。

**遗留问题**:
- [ ] 若后续发现新的 `agents/openai.yaml` 或小型路由文件出现实锤错码，仍需按 `convertible-in-place` 路线单独修复。
- [ ] 历史归档对象仍可继续做抽样巡检，但不应打扰当前活跃业务线程的 owner-held 现场。
