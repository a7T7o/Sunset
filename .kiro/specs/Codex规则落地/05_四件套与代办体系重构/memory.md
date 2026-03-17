# 四件套与代办体系重构 - 开发记忆

## 模块概述
- 本阶段负责把 Sunset 的 `requirements / design / tasks / memory` 使用方式从“单工作区无限膨胀”收回到“根层记忆 + 分阶段 tasks + 按需 design/requirements”的轻量结构。

## 当前状态
- **完成度**: 100%
- **最后更新**: 2026-03-16
- **状态**: 本阶段已收口，后续按规则继续推广

## 会话记录

### 会话 1 - 2026-03-16

**用户需求**:
> 建立一个彻底的新工作区来完成这次发现的所有代办事项，并在里面建立干净的文档机制；非必要只保留必要的两件套，也就是 memory 和 tasks；同时彻底反思和重构 Requirement、design、tasks、memory 这四件套的规范，并判断 skills 是否需要进一步接管代办路由。

**完成任务**:
1. 回读 `2026.02.15_代办规则体系` 与现行 Sunset 活文档，提炼当前仍有效的四件套规则。
2. 在 `000_代办/codex/05_四件套与代办体系重构/` 建立本阶段 `tasks.md`，并补足本 `memory.md`，让阶段工作区符合“根层记忆 + 阶段 tasks”规范。
3. 将 `Sunset工作区四件套与代办规范_2026-03-16.md` 收紧为现行专项规则。
4. 将旧全局 `tasks.md` 缩退为兼容路由页，不再承接新的治理续办。
5. 完成 `sunset-todo-router` 评估，给出“暂不立即新建”的结论与升级条件。

**修改文件**:
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\05_四件套与代办体系重构\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\05_四件套与代办体系重构\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset工作区四件套与代办规范_2026-03-16.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\文档重组总索引_2026-03-13.md`

**关键决策**:
- Sunset 后续默认不再长期维护“一个工作区一本总 tasks”。
- 阶段任务应沉淀在阶段目录自己的 `tasks.md`，而不是堆回旧全局入口。
- `sunset-todo-router` 不是现在的第一优先级；当前先用 `skills-governor + sunset-workspace-router + 现行四件套规范` 组合承担代办路由。

**验证结果**:
- `000_代办/codex` 现已具备根层 `memory.md` + 分阶段 `tasks.md` 的治理续办结构。
- 旧全局 `tasks.md` 已缩退为兼容路由页。
- 四件套规范已经在活文档中正式落盘。

**遗留问题**:
- [ ] 旧超长 `memory.md` 仍需按线程和工作区逐步分卷。
- [ ] 需要在后续真实任务中继续验证“阶段 tasks + 根层 memory”是否足够稳。

## 关键决策

| 决策 | 原因 | 日期 |
|------|------|------|
| 治理续办统一承接到 `000_代办/codex` | 避免继续把历史尾账和现行入口混在一个活目录里 | 2026-03-16 |
| 旧全局 `tasks.md` 只保留兼容路由身份 | 避免新的治理续办重新堆回灾难级入口 | 2026-03-16 |
| 暂不立即新建 `sunset-todo-router` | 现有 skills 与专项规则已经能承担当前代办路由，先避免过早扩 skill 面 | 2026-03-16 |

## 相关文件

| 文件 | 说明 |
|------|------|
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\05_四件套与代办体系重构\tasks.md` | 本阶段执行清单 |
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset工作区四件套与代办规范_2026-03-16.md` | 当前现行四件套规范 |
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\tasks.md` | 已缩退为兼容路由页的旧入口 |
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md` | 治理续办总代办记忆 |
