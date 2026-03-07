# 2026.03.02_Cursor规则迁移 - 开发记忆

## 模块概述

在不破坏现有 `.kiro/steering` 规则体系的前提下，为 Cursor 的 Rules / Skills / Subagents / Commands 设计一套与 Steering 规则协同工作的迁移方案，并用文档固化下来，为后续自动化和子智能体建设打基础。

## 当前状态

- **完成度**: 10%
- **最后更新**: 2026-03-02
- **状态**: 进行中

## 会话记录

### 会话 1 - 2026-03-02

**用户需求**:
> 在 `Steering规则区优化` 工作区下创建一个新子工作区，为 Cursor 的 Rules / Skills / Subagents 等功能编写迁移计划；要求完全遵守 `.kiro/steering` 的规则与 Hook 流程。

**完成任务**:
1. 阅读并整理 `.kiro/steering` 全部规则文件与 `hook事件触发README.md`，确认当前 Steering 规则区的最终形态与 Hook 事件模型。
2. 在主工作区下创建 Cursor 教程文档 `Cursor规则_Skills_Subagents教程.md`，用中文系统说明 Cursor 的 Rules / Skills / Subagents / Commands，并给出与 Steering/Hook 的职责边界。
3. 新建子工作区 `2026.03.02_Cursor规则迁移/` 并创建本 `memory.md` 作为迁移工作的索引。

**修改文件**:
- `Cursor规则_Skills_Subagents教程.md` - 新建：Cursor Rules/Skills/Subagents/Commands 教程与与 Steering/Hook 协作说明。
- `2026.03.02_Cursor规则迁移/memory.md` - 新建：记录 Cursor 迁移子工作区的首次会话。

**解决方案**:
- 将本子工作区定位为“迁移规划与落地执行”的专用空间，只在这里讨论和记录将 `.kiro/steering` 规则映射到 Cursor 配置体系（Rules/Skills/Subagents/Commands）的策略；具体对各系统的实现细节仍留在各自领域工作区。

**遗留问题**:
- [ ] 需要补充 `requirements.md`、`design.md`、`tasks.md` 与专门的《迁移计划》文档。
- [ ] 需要设计分阶段迁移方案（先文档与规则对齐，再逐步引入 Skills/Subagents，最后评估哪些 Hook 可以被替代或精简）。

---

## 关键决策

| 决策 | 原因 | 日期 |
|------|------|------|
| 将 Cursor 配置视为 Steering 的“运行时镜像”而非新的权威规则源 | 避免出现“双重真理”，保证 `.kiro/steering` 继续作为唯一规则来源 | 2026-03-02 |

## 相关文件

| 文件 | 说明 |
|------|------|
| `Cursor规则_Skills_Subagents教程.md` | Cursor 规则/技能/子代理/命令的整体说明与与 Steering 的协作关系 |
| `2026.03.02_Cursor规则迁移/memory.md` | 本子工作区的开发记忆（当前文件） |

---

### 会话 2 - 2026-03-02（续）

**用户需求**:
> 阅读两份聊天记录后，明确核心差异：Kiro 有自动继承对话机制（>80%压缩+Hook生成快照），Cursor 没有这个机制，不需要融入。需求是全面完成 Kiro 规则与 Hook 的 Cursor 化适配。

**完成任务**:
1. 阅读 `9_Kiro-Cursor迁移聊天记录001.md` 和 `9_Kiro-Claude迁移聊天记录001.md`，理解用户核心需求
2. 明确 Kiro 与 Cursor 的核心差异：自动继承对话机制
3. 修正 `Cursor规则_Skills_Subagents教程.md`，明确：
   - 不需要融入：自动继承对话、context-recovery、memory 自动更新检查
   - 需要融入：Steering 规则体系、Hook 可手动执行部分、Memory 手动更新
4. 修正《迁移计划.md》，明确融入原则和具体方案

**修改文件**:
- `Cursor规则_Skills_Subagents教程.md` - 修正：明确融入范围，区分 Kiro 自动特性和 Cursor 需要的手动特性
- `迁移计划.md` - 修正 v2.0：明确不需要融入的特性，聚焦规则适配和手动操作流程

**关键理解**:
- Kiro 的自动化机制（自动继承、自动 Memory 更新）不需要融入 Cursor
- 用户已自行复制 `.kiro/steering` 和 `.kiro/hooks` 到 `.cursor/` 目录
- 目标是让 Cursor 也能像 Kiro 一样自动执行规则和工作流，但通过 Cursor 的方式（Rules 配置、手动触发）


### 会话 3 - 2026-03-06

**用户需求**：
> 继续推进“全库范围工具术语一致性清扫”，重点修复 `.cursor/rules` 中残留的旧工具术语（`readFile/fsWrite/fsAppend` 等），避免规则文本继续误导不可执行的工具调用。

**完成任务**：
- 修正 `.cursor/rules` 文档中的旧工具名：`readFile/fsWrite/fsAppend` → `Read/Edit/Write/Bash` 对齐为 Claude Code 可执行语义。
- 将 `rules.md` 的“文档写入规范”改写为 Claude Code 工具模型（新建/修改/大段追加的分块策略）。
- 复扫确认：`.cursor/rules/**/*.md` 不再出现 `readFile/fsWrite/fsAppend/read_file` 等残留。

**修改文件**：
- `.cursor/rules/README.md` - 修改：`readFile` → `Read`（智能加载机制表述对齐）。
- `.cursor/rules/rules.md` - 修改：文档写入规范从 `fsWrite/fsAppend` 迁移为 `Write/Edit/Bash`。
- `.cursor/rules/000-context-recovery.md` - 修改：`readFile` → `Read`（继承恢复规则表述对齐）。
- `.cursor/rules/maintenance-guidelines.md` - 修改：`readFile` → `Read`（keywords/智能加载说明对齐）。

**解决方案**：
- 统一将“规则文本中的工具术语”与 Claude Code 的实际工具集合绑定，避免文档继续产出不可执行指令。

**遗留问题**：
- [ ] 扩展同类清扫范围：对 `.kiro/specs/**`（尤其是历史 Hook/迁移文档）中残留的旧工具术语做分层治理（必要处标注“历史语境”，其余对齐 Claude Code 工具名）。

