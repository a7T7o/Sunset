# 2026.03.02_Cursor规则迁移 - 需求文档

## 1. 背景

- `.kiro/steering` 已经过多轮优化（Phase 4.0/4.1/5.0/Hook v7-v8），形成完整的规则与 Hook 体系。
- Cursor 新版提供了 `Rules / Skills / Subagents / Commands` 等配置能力，目前只建立了一个全局 `rules.md` 镜像 steering 核心规则。
- 需要一套**可审查、可执行、可回滚**的迁移方案，把已有 Kiro 规则体系有序映射到 Cursor 侧配置，而不是重新造轮子或制造第二套真理来源。

## 2. 目标

1. **明确角色边界**：定义 `.kiro/steering`、Cursor Rules、Skills、Subagents、Commands、Hook 之间的职责与优先级。
2. **给出迁移蓝图**：形成一份详细的《迁移计划》文档，包含阶段划分、任务拆解、风险分析、回滚策略。
3. **保持单一真理源**：确保所有规范仍以 `.kiro/steering` 中 `isCanonical: true` 的文件为唯一真理来源，Cursor 侧仅做“运行时镜像”和自动化封装。
4. **设计可落地 POC**：在不触碰现有正式 Hook 的前提下，提出 1–2 个可以立即实现的 demo（例如 userTriggered Hook + Skill 组合），用于验证迁移思路。

## 3. 非目标

- 本轮不会大规模重写或删除现有 `.kiro/steering` 规则文件。
- 本轮不会直接修改现有正式 Hook（如 `memory-update-check.kiro.hook v8`），只允许新增**独立 demo Hook** 作为实验。
- 不在本子工作区内重构具体业务系统（农田/箱子/存档等），只负责“规则与自动化层”的映射规划。

## 4. 约束条件

- **严格遵守 steering 规则**：
  - 文档全部使用中文（代码片段除外）。
  - 不覆盖 `requirements/design/tasks/memory`，只追加或新建。
  - Memory 更新遵守“先子后主”“所有对话都要有记录”“不得跳过子 memory”。
  - 禁止幻想：任何关于代码/文件状态的结论必须有当前会话内的 Read 证据或已有文档证据。
- **Hook 变更限制**：
  - 仅在用户明确允许的前提下，新增 demo 级 Hook 文件，文件名和描述中需明显带有 `-demo` 或 `测试` 标记。
  - 不修改、不删除现有 `.kiro/hooks/*.kiro.hook` 正式文件。

## 5. 输出物

1. 本子工作区三件套：
   - `requirements.md`（当前文件）
   - `design.md`（迁移架构、阶段设计）
   - `tasks.md`（可执行任务列表）
2. 迁移规划文档：
   - `迁移计划.md`：包含背景、现状评估、阶段拆解、风险与回滚、POC 设计等。
3. 主工作区补充：
   - `Steering规则区优化/memory.md` 中追加本次迁移规划会话记录与子工作区索引。

## 6. 成功标准

- 有一份结构清晰、可直接执行的迁移计划，后续任何对 Cursor 配置的修改都可以挂接到该计划对应阶段。
- Cursor 侧新增的 Rules/Skills/Subagents/Commands/Hook demo 全部遵守 `.kiro/steering` 的规则与写入规范。
- 不增加新的“隐形规则来源”，所有规范变更都能在 `.kiro/steering` 或 specs 工作区文档中找到依据。

