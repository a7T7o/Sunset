# Cursor 配置体系总览（Rules / Skills / Subagents / Commands）

> **目标**：给未来的自己一个完整的 Cursor 配置教程，让 Cursor 能够像 Kiro 一样自动执行规则和工作流，同时明确 Kiro 自动化特性哪些需要融入、哪些不需要融入。
>
> **核心区别**：Kiro 有自动继承对话机制（上下文超过80%自动压缩，Hook 生成继承快照），**Cursor 没有这个机制**，因此继承恢复相关功能不需要融入。

---

## 一、Kiro 与 Cursor 的核心差异

| 维度 | Kiro | Cursor |
|------|------|--------|
| **自动继承对话** | ✅ 有（>80%自动压缩 + Hook 生成快照 + context-recovery） | ❌ 无（需要手动处理） |
| **规则加载** | ✅ Hook 自动智能加载 | 需要通过 Rules 配置 |
| **Memory 更新** | ✅ 自动（Hook 触发） | 需手动（在回复末尾提醒或用户触发） |
| **文件事件 Hook** | ✅ 自动触发 | 需通过 Cursor 的 Auto Attach 或手动 |
| **任务结束 Hook** | ✅ 自动触发 | 无此机制 |

**不需要融入 Cursor 的 Kiro 特性**：
- `000-context-recovery.md` 的继承恢复流程
- `memory-update-check.kiro.hook` 的自动压缩检测
- 自动生成的继承会话快照机制

**需要融入 Cursor 的 Kiro 特性**：
- Steering 规则体系（禁止事项、编码规范、工作流）
- Hook 的"可手动执行的部分"（如智能规则加载思路，转化为 Rules 配置）
- Memory 手动更新流程（通过规则约束或用户指令触发）

---

## 二、Cursor Rules（规则）详解

### 2.1 规则文件位置

用户已直接将 `.kiro/steering/` 下的文件复制到 `.cursor/rules/`，包括：
- `rules.md` - 核心开发规则
- `communication.md` - 沟通规范
- `workspace-memory.md` - 工作区规范
- `000-context-recovery.md` - 继承恢复（参考，但 Cursor 无需自动执行）
- 各类系统规则：`farming.md`、`chest-interaction.md`、`save-system.md` 等

### 2.2 规则加载机制

Cursor 的 Rules 支持多种加载方式：

| 加载方式 | 说明 | 对应 Kiro 特性 |
|---------|------|---------------|
| `alwaysApply: true` | 始终加载 | Kiro 的 P0 核心规则 |
| `globs` 匹配 | 编辑匹配文件时自动加载 | Kiro 的 Hook 智能加载 |
| `Agent Requested` | Agent 判断是否需要 | Kiro 的手动加载 |

### 2.3 融入策略

由于 Cursor 没有自动继承对话机制：

1. **继承恢复相关规则**：仅作为参考文档，不需要自动执行
2. **Memory 更新**：通过规则约束，要求每次任务结束后提醒用户手动更新，或用户输入特定指令时执行
3. **智能规则加载**：通过 Cursor 的 `globs` 配置实现类似 Hook 的自动加载效果

---

## 三、Cursor Hook 对应方案

### 3.1 Kiro Hook → Cursor 映射

| Kiro Hook | 事件类型 | Cursor 对应方案 | 融入状态 |
|-----------|---------|----------------|---------|
| `smart-assistant.kiro.hook` | promptSubmit | 通过 `globs` 自动加载领域规则 | ✅ 需要融入 |
| `memory-update-check.kiro.hook` | agentStop | 手动更新 + 规则约束提醒 | ⚠️ 手动执行 |
| 文件创建/保存 Hook | fileCreated/fileEdited | Cursor 无此机制 | ❌ 不需要 |

### 3.2 手动执行的操作

由于 Cursor 没有自动任务结束触发器，以下操作需要手动执行：

1. **Memory 更新**：在完成任务后，提醒用户或根据用户指令更新 memory
2. **继承会话快照**：用户输入"执行对话快照操作"时才执行

---

## 四、Cursor Skills 与 Subagents

### 4.1 Skills（技能）

Skills 是可复用的能力模块，在 Cursor 中可以：

- 通过规则约束要求在特定场景下使用
- 作为 SOP 文档供 Agent 调用

### 4.2 Subagents（子代理）

Cursor 支持创建专门的子代理执行特定任务，可以类比 Kiro 的长期工作区。

---

## 五、在本项目中如何使用

### 5.1 日常开发流程

1. **启动时**：Cursor 自动加载 `alwaysApply: true` 的规则
2. **开发过程中**：通过 `globs` 自动加载相关领域规则
3. **任务完成后**：
   - 提醒用户更新 memory（因为没有自动 Hook）
   - 如需快照，等待用户输入"执行对话快照操作"

### 5.2 与 Kiro 的差异应对

| Kiro 自动行为 | Cursor 应对方式 |
|--------------|---------------|
| 自动继承对话 | 无需处理（Cursor 不支持） |
| 自动 Memory 更新 | 任务结束后提醒用户或用户手动触发 |
| 自动规则加载 | 通过 Cursor Rules 的 `globs` 实现 |
| 自动快照生成 | 用户输入特定指令时执行 |

---

## 六、后续工作

1. **确认哪些 Hook 需要转化为 Cursor 规则或手动操作**
2. **设计 Memory 更新的手动触发流程**
3. **确定 Skills/Subagents 的具体使用场景