# Codex迁移与规划 - 设计文档

**创建日期**: 2026-03-07

---

## 1. 设计目标

为 Codex 在本项目中的使用方式建立一套“**不脱离 Steering、不过度幻想自动化、能直接执行**”的运行协议。

本设计重点解决三件事：

1. **规则归属**：哪些内容继续由 `.kiro/steering` 决定，哪些内容只在 Codex 文档中解释。
2. **流程迁移**：Kiro / Claude 已有工作流里，哪些可以迁移到 Codex，哪些必须舍弃或降级。
3. **操作技巧**：Codex 在这个仓库中怎样做，才能稳定、低漂移、可追踪。

---

## 2. 已确认现状

### 2.1 Kiro 的核心价值

- Kiro 的主防线是 `.kiro/steering` canonical 文档。
- Kiro Hook 是 IDE 内部事件系统，具备 `promptSubmit`、`agentStop`、`preToolUse` 等原生语义。
- 正式规划、分析、设计、审查，最终都应回写到 `.kiro/specs/` 工作区。

### 2.2 Claude 迁移的关键收敛

从 `CLAUDE.md` 与 `Claude迁移与规划` 工作区可归纳出三条已经被验证的治理结论：

1. **`CLAUDE.md` 应做排序器，不应重写 Steering 正文。**
2. **`.claude/hooks` 只能承担有限提醒 / 审计 / 防呆，不能假装等同于 Kiro Hook。**
3. **真正不会漂移的主线，是工作区先行 + 证据先行 + memory 收尾必做。**

### 2.3 Codex 的现实能力边界

当前 Codex 会话具备的核心能力是：

- `update_plan`：适合做对话内进度管理，不适合替代项目正式规划文档。
- `shell`：适合读取、搜索、补丁、执行验证命令。
- `web`：适合处理最新信息、需要外部事实验证的任务。
- Unity MCP：适合执行受支持的 Unity 编辑器操作与日志/测试链路。

但 Codex 没有本项目内建的、与 Kiro 同等级的：

- 自动 prompt 路由 Hook
- 自动 agentStop memory Hook
- 自动上下文继承恢复机制

因此 Codex 方案必须以 **人工执行 SOP + 工作区文档沉淀** 为主。

---

## 3. 迁移原则

### 原则 1：Steering 继续做唯一规则源

- 所有业务规则、项目规则、领域规则，仍以 `.kiro/steering/` 为准。
- Codex 文档只负责回答两类问题：
  1. 我在 Codex 中该怎么读、怎么做。
  2. 哪些原本依赖 Hook 的环节，现在要靠手法补回来。

### 原则 2：Codex 只描述真实能力

- 能自动做的，才写成自动步骤。
- 不能自动做的，要明确写成“人工执行”或“转交用户”。
- 禁止把平台外能力写成默认前提。

### 原则 3：正式产物回写工作区

- `update_plan` 只是会话态工具；正式规划仍回写 `requirements.md` / `design.md` / `tasks.md` / 专项文档。
- 这与 Claude 侧“默认 acceptEdits，不靠全局 plan 文件承载正式规划”的收敛结论一致。

### 原则 4：SOP 重于幻觉自动化

- Codex 最稳妥的迁移不是“发明不存在的 Hook”，而是把高频纪律写成简明、可重复执行的 SOP。

---

## 4. Codex 运行协议（五层模型）

### L0：系统 / 开发者 / 用户指令

- 这是 Codex 实际运行时的最高优先级层。
- 若与仓库文档冲突，按平台实际指令执行，并在文档中保持解释而非对抗。

### L1：`.kiro/steering/` canonical 规则层

- 负责项目规则事实。
- 包括：`rules.md`、`workspace-memory.md`、`scene-modification-rule.md`、`code-reaper-review.md` 等。

### L2：`.kiro/specs/` 工作区产物层

- 负责当前话题的计划、分析、记录、记忆与阶段性交付。
- Codex 的正式工作记录必须沉淀在这里。

### L3：Codex 会话执行层

- 负责当前回合的读证据、列计划、调用工具、落盘、验证、总结。
- `update_plan`、preamble、最终回复格式都属于这一层。

### L4：Codex 技巧与防漂移层

- 负责降低误判和误操作：
  - 先 `Read/Grep` 再结论
  - 先读 `memory.md` 尾部再追加
  - 工具按依赖关系分组
  - 只并行互不依赖的读操作
  - 对 Unity 高风险修改先审视再确认

---

## 5. Kiro / Claude / Codex 映射关系

| 主题 | Kiro | Claude 迁移经验 | Codex 设计口径 |
|------|------|----------------|---------------|
| 规则主源 | `.kiro/steering` | `CLAUDE.md` 只做排序器 | 继续以 `.kiro/steering` 为唯一主源 |
| Prompt 路由 | `promptSubmit` Hook | 通过 `CLAUDE.md` + marker 辅助 | 通过会话启动 SOP 手动路由 |
| Memory 收尾 | `agentStop` Hook 可参与 | hooks 不能代替人工纪律 | 明确“结束前必须手动更新 memory” |
| 工具阻断 | `preToolUse` 语义强 | Bash 防呆部分可用 | 以平台工具能力 + 自我检查为主 |
| 压缩继承 | Kiro 有专门机制 | Claude 已确认不是主迁移目标 | Codex 不做 1:1 迁移 |
| 正式规划 | 可配合 Hook / 工作区 | 不依赖全局 plans | 使用 `update_plan` 管过程，工作区文档管产物 |

---

## 6. 关键设计决策

### 决策 1：Codex 不另造“Codex 版 CLAUDE.md”

- 原因：容易再次形成双源规则漂移。
- 选择：先在 `Codex迁移与规划` 子工作区内形成手册与迁移文档，再视需要上提。

### 决策 2：Codex 的主补偿点是 SOP，不是 Hook 幻想

- 原因：当前可确定的 Codex优势在工具编排、计划追踪、终端执行和文档整理，而不是 repo 内生命周期 Hook。

### 决策 3：保留“工作区先行 + 记忆必做”作为最核心迁移成果

- 原因：这是 Kiro 与 Claude 迁移经验里最稳定、最不依赖平台细节的部分。

---

## 7. 输出物设计

本阶段输出以下文件：

1. `requirements.md`：说明目标与边界。
2. `design.md`：说明设计原则、映射与决策。
3. `tasks.md`：记录后续迁移拆解。
4. `memory.md`：记录本子工作区推进过程。
5. `Codex工作流与技巧手册.md`：作为未来 Codex 会话的直接操作蓝本。

---

## 8. 后续演进方向

### Phase A：先在工作区内部验证

- 继续补充手册中的路由表、验证链路、交付模板。
- 用后续真实 Codex 会话验证是否足够稳定。

### Phase B：评估是否上提项目级指南

- 若手册稳定，可再评估是否新增仓库级 Codex 说明文件。
- 上提时仍必须保持“解释层”定位，不替代 Steering。

### Phase C：与实际工程任务联动验证

- 在后续真实业务工作区中验证：
  - 是否能稳定做到“先工作区、后执行、再记忆”。
  - 是否能避免 Plan 工具与正式产物脱钩。

