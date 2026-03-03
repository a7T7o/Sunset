# 2026.03.02_Cursor规则迁移 - 设计文档

## 1. 当前架构与问题定位

### 1.1 现有规则体系

- **Steering 规则区**（`.kiro/steering`）：
  - P-1：`000-context-recovery.md`（继承恢复，最高优先级）。
  - P0：`rules.md`、`layers.md`、`workspace-memory.md`、`communication.md`（核心禁止事项、层级设计、工作区规范、沟通规范）。
  - P1/P2：系统与开发规范（`animation.md`、`ui.md`、`so-design.md`、`items.md`、`systems.md`、`trees.md`、`coding-standards.md` 等）。
  - 归档：`archive/*.md`（产品、技术栈、结构、进度、农田）。
- **Hook 体系**（`.kiro/hooks/`）：
  - `smart-assistant.kiro.hook`：promptSubmit 智能规则加载（Phase 4.0/4.1）。
  - `memory-update-check.kiro.hook`：agentStop memory 更新检查 + 压缩检测（迭代到 v8）。
- **Specs 工作区**：
  - 大量以 `{日期/编号}_{功能}` 命名的工作区，各自有 `requirements/design/tasks/memory` 与子工作区。

### 1.2 Cursor 侧现状

- `.cursor/rules/rules.md` 已经镜像 Steering 的核心规则，并设置为 `inclusion: always`。
- 尚未使用：
  - `.cursor/skills/`：没有任何 Skill 定义。
  - `.cursor/subagents/`：没有自定义子智能体配置（仅有内置 Task 子代理）。
  - `.cursor/commands/`：没有 `/xxx` 命令。
- 截止目前，Cursor 配置只承担“把 Steering 核心规则注入上下文”的职责，没有进一步的自动化封装。

### 1.3 问题与机会

- **问题**：
  - Steering 规则与 Hook 演化历史复杂，记忆成本高，新建自动化（Skills/Subagents/Commands）容易踩老坑。
  - Cursor 新能力尚未系统性使用，很多可以自动化的步骤（例如三件套生成、TD 维护）仍通过临时对话完成。
- **机会**：
  - 利用 Skills 把已经成熟的 SOP 固定下来，减轻记忆负担。
  - 利用 Subagents 把某些长线工作区（如存档系统、导航系统）拆给专职子智能体。
  - 利用 Commands 和 userTriggered Hook 提供一键式工具，而不是每轮对话手动敲指令。

---

## 2. 迁移总体思路

### 2.1 单一真理源原则

- 所有“规则内容”仍然以 `.kiro/steering` 中标记 `isCanonical: true` 的文件为唯一真理来源：
  - `rules.md` / `layers.md` / `workspace-memory.md` / `communication.md`。
- Cursor Rules/Skills/Subagents/Commands 只做两件事：
  1. **把规则带进上下文**（镜像 + 引用）。
  2. **按规则执行步骤**（SOP 自动化）。

### 2.2 两条迁移主线

1. **文档与规则镜像线**：
   - 梳理哪些 steering 规则需要在 Cursor 侧有镜像（例如语言规范、memory 规则、代办管理）。
   - 在 `.cursor/rules/` 内补充或拆分必要的规则文件，但内容必须从 steering 复制/摘要而来。
2. **自动化与执行线**：
   - 为成熟的工作流设计 Skills（例如“三件套生成”“TD 管理”“审视报告模板化”）。
   - 为重型/跨会话任务设计 Subagents（例如“存档系统审查子代理”），并定义其使用范围与 memory 责任。
   - 用 Commands 和 demo Hook 把这些能力挂到“用户一键触发”的入口上。

---

## 3. 分阶段设计

### Phase 1：现状固化与风险收敛（本工作区）

**目标**：把现有 Steering 规则区 + Hook 体系的现状、优先级和风险点通过文档彻底固化，为后续迁移提供稳定基线。

- 任务要点：
  - 读取并确认 `.kiro/steering/README.md` 与 `规则优先级分析.md` 的最终版本。
  - 在主工作区下写好《Cursor规则_Skills_Subagents教程》，统一解释 Cursor 配置与 Steering/Hook 的职责划分。
  - 在本子工作区下补齐 `requirements/design/tasks/memory` 与《迁移计划》文档骨架。
- 输出：
  - 一套能让未来自己“3 分钟读懂当前架构”的文档组合。

### Phase 2：规则映射与分类（计划阶段）

**目标**：明确“哪些 steering 规则需要在 Cursor 侧有镜像，以及镜像到哪一层（Rules / Skills / Subagents / Commands）”。

- 动作设计：
  - 以 `规则优先级分析.md` 为基础，建立一张“Steering → Cursor”映射表：
    - P-1 / P0：在 Cursor Rules 中保持一份镜像（只做轻量摘要）。
    - P1/P2：只在 Skills/Subagents 中引用，不再重复定义。
    - P3/P4：保持 archive 形式，只在需要时由 Skill 加载。
  - 根据 `.kiro/steering/workspace-memory.md` 的“子工作区嵌套 + 代办管理”规则，限定 Cursor 侧可以修改哪些 specs 文件，以及 Memory 责任如何分层。

### Phase 3：Skills 与 Subagents 设计（不改正式 Hook）

**目标**：先用 Skills/Subagents 在“应用层”验证迁移路线，而不触碰任何现有 Hook。

- 典型 Skills 设计候选：
  - `specs-triple-file-generator`：根据 steering 规范，在任意 specs 工作区一键创建三件套。
  - `td-sync-from-memory`：扫描一个工作区的 memory/tasks，更新或生成对应的 TD 文件。
  - `steering-audit-skill`：自动读取 steering 规则与审查报告模板，生成新一轮审查草稿。
- 典型 Subagents 设计候选：
  - `save-system-auditor`：专门负责存档系统 specs/代码审查与 Phase 文档维护。
  - `hook-architecture-reviewer`：专门负责 Hook 相关实验与报告（避免主智能体上下文被 Hook 历史淹没）。

### Phase 4：Hook 精简与 demo 试点（仅新增 demo，不改正式 Hook）

**目标**：在保证安全的前提下，设计 1–2 个独立的 demo Hook，验证“由 Hook 触发 Skills/Subagents”的整体链路。

- 约束：
  - 只能新增带 `-demo` 或“测试”字样的 Hook 文件（如 `cursor-migration-demo.kiro.hook`）。
  - 不修改、不删除、不重命名现有 Hook 文件。
  - demo Hook 只能调用**无副作用或弱副作用**的 Skills（例如只读 + 生成报告，不直接修改项目资产）。

---

## 4. 关键设计决策

1. **Steering 优先**：任何规则内容的修改都必须先通过 `.kiro/steering` 对应文件，再决定是否同步 Cursor 侧镜像。
2. **Memory 驱动**：所有迁移行为（包括仅规划、不落地的分析）都必须在对应工作区 `memory.md` 中记录——本子工作区负责记录迁移视角，具体系统工作区记录对业务层的影响。
3. **渐进式验证**：先用文档与 Skills/Subagents 在“应用层”试跑迁移方案，在用户明确确认之前，不对正式 Hook 和核心规则做结构性改动。
4. **可回滚性**：任何新增的 Cursor Rules/Skills/Subagents/Hook demo 文件在命名和内容上都要清晰标注来源和用途，一旦方案被否定可以单向删除，不影响现有生产配置。

---

## 5. 与其它工作区的关系

- 与 `Steering规则区优化` 主工作区：
  - 主工作区继续记录所有关于 Steering/Hook 改造的全局决策与审查报告。
  - 本子工作区聚焦“Cursor 侧映射与迁移”，不重复叙述 Hook v4/v5/v6/v7 的细节，只在需要时引用相应分析文档。
- 与各功能系统工作区（农田/箱子/存档/物品放置等）：
  - 不直接修改业务代码或 specs，只在迁移方案中列出“未来可由 Skills/Subagents 接管”的工作流。
  - 若迁移方案对某个系统提出约束（例如“存档系统必须在某 Hook 前完成 memory 写入”），需要在该系统的工作区中同步记录。

