# Kiro-Claude 调度协议（Claude CLI 现实版）

> 本文件只负责：优先级、规则路由、执行前后 SOP、危险动作红线、memory 收尾纪律。
>
> 本文件**不是**第二套业务规则正文。具体领域事实与细则，统一回落到 `.kiro/steering` canonical 文档。

---

## 1. 项目内优先级与冲突仲裁

### 1.1 项目内优先级顺序
1. 用户当轮明确指令
2. `.kiro/steering/rules.md`
3. 对应 canonical 领域文档
4. 场景触发文档
5. archive / 背景文档
6. `CLAUDE.md`

### 1.2 `CLAUDE.md` 的定位
- 负责排序，不负责重写所有规则。
- 负责说明“什么时候该读什么文档”。
- 负责定义普通会话主流程、收尾前检查清单、不可越过的红线。
- 若 `CLAUDE.md` 与 steering 细节冲突：**执行细节以对应 steering canonical 文件为准**。

---

## 2. 普通会话启动 SOP

### 2.1 先判断任务类型
- **纯问答 / 纯解释**：直接回答。
- **代码修改 / 文档落盘 / 调查问题**：先读证据，再执行。
- **多文件改动 / 架构调整 / 多种实现路径**：先给实现方案与步骤，再动手。
- **场景 / Prefab / Inspector / ScriptableObject 配置修改**：先读取和分析当前配置，再征求同意。

### 2.2 规则路由表

| 任务关键词 | 必读 steering |
|---|---|
| 工作区、memory、分卷、继承快照 | `workspace-memory.md` |
| 锐评、审查、审视报告 | `code-reaper-review.md` |
| 场景、组件、Prefab、Inspector、SO配置 | `scene-modification-rule.md` |
| 物品、背包、堆叠 | `items.md` + `so-design.md` |
| 放置、预览、底部对齐 | `placeable-items.md` |
| 农田、作物、浇水、耕地 | `archive/farming.md` + 当前活跃工作区文档 |
| 树木、遮挡、碰撞 | `trees.md` + `layers.md` |
| UI、面板、快捷键 | `ui.md` |
| 存档、保存、加载、GUID | `save-system.md` |
| 动画、工具帧同步 | `animation.md` |
| 系统、时间、季节、导航 | `systems.md` |
| 编码、命名、Region | `coding-standards.md` |
| 调试、日志 | `debug-logging-standards.md` |

### 2.3 工作区先行硬门槛
- 任何计划、报告、审视、设计、任务拆解、代办记录，必须先锚定到 `.kiro/specs/` 内的明确工作区，再允许落盘。
- 禁止把 `C:\Users\aTo\.claude\plans\...` 这类全局 plans 路径当成项目正式产物目录。
- 若系统 PlanMode 只能先写全局 plan 文件，则本仓库默认不用它承载正式规划；改为直接在当前工作区的 `design.md`、`tasks.md` 或专项分析文档内落盘。
- 若当前话题还没有工作区：先创建或确认工作区，再继续规划、报告或大段文档输出。
- 本仓库默认保持 `acceptEdits` 工作方式，不主动调用 `EnterPlanMode`；如需规划，直接在当前工作区文档内规划。只有用户明确要求进入 PlanMode，且确认正式产物仍回写工作区时，才允许讨论该路径。

### 2.4 工作区读取顺序
1. 先定位当前最相关工作区。
2. 若工作区存在：先读该工作区 `memory.md`。
3. 若当前任务依赖三件套：再读 `requirements.md`、`design.md`、`tasks.md`。
4. 若用户提到锐评：再读锐评文件和 `code-reaper-review.md`。
5. 若检测到明确继承/压缩信号：才进入继承恢复分支；**普通 CLI 对话不默认走这条线**。

---

## 3. 执行中纪律

### 3.1 证据先行
- 不对未读取的代码、文档、配置做结论。
- 所有关键判断都应有 `Read` / `Grep` / 实际配置证据支撑。
- 不把 `.claude/hooks` 的 marker 当成“自动完成”的事实证据。

### 3.2 修改边界
- 补丁只改当前任务需要的最小范围。
- 不顺手扩写无关模块。
- 新功能遵循“新增并兼容”；bug 修复不顺手做架构重写。
- 修改后必须记录修改文件清单。

### 3.3 锐评处理
- 锐评必须先核查，再决定执行。
- 路径 A/B：可直接执行。
- 路径 C：**必须**创建审视报告并中断等待用户确认。
- 对话中的锐评处理细则，统一以 `.kiro/steering/code-reaper-review.md` 为准。

---

## 4. Memory 收尾 SOP（硬规则）

### 4.1 触发原则
以下情况都必须记忆，不允许以“没改代码”为理由跳过：
- 代码修改
- 文档落盘
- 架构决策
- 规则调整
- 锐评核查
- 纯问答但形成了可追踪结论

### 4.2 更新顺序
- 有明确子工作区：**先子后父**。
- 多层嵌套：**从最内层开始逐级上报**。
- 无明确子工作区：更新主 `memory.md`。
- 继承/压缩场景不是普通主流程；只有明确出现压缩/继承信号时，才按 `workspace-memory.md` 的例外规则处理。

### 4.3 Append-only 手法（强制）
1. 写前先读取目标 `memory.md` 当前尾部。
2. 确认旧会话块仍完整存在。
3. 本次只能新增会话块，**只允许在 EOF 之后追加**。
4. 禁止用替换旧块的方式伪装追加。
5. 写后重新读取尾部，确认：
   - 旧记录仍在
   - 新记录只出现在文件尾部
   - 父/子顺序正确

### 4.4 分卷纪律
- 活跃卷始终是 `memory.md`。
- 主 memory 单卷上限 200 行；子 memory 单卷上限 300 行。
- 写入前若预计超限：将当前 `memory.md` 归档为 `memory_N.md`，新建前情提要后继续写入。
- 分卷允许，改写历史会话块不允许。

---

## 5. 文档与工具调用纪律

### 5.1 文档写入
- 已存在文件：优先 `Edit`，避免整文件覆写。
- 新文件：用 `Write` 创建。
- 长文分块写入，避免超大 payload。
- 连续失败 2 次：停止重试，先重新 `Read` 当前文件，再继续。

### 5.2 Memory 特别规则
- `memory.md` 只做追加，不改历史会话。
- 归档重命名允许；历史块改写不允许。
- 不能把 `replace` 当 `append` 用。

### 5.3 输出风格
- 中文、直接、少废话。
- 结论要有证据，不做空想式建议。
- 需要列文件时，优先给出可追踪路径。

---

## 6. Unity / MCP 现实边界

### 6.1 允许自动执行的范围
- 已获授权且属于“创建类”的 Unity 基础操作，可通过 MCP 直接执行。
- 执行前必须在相关工作区文档中记录将要创建的资产或操作。

### 6.2 需要先分析再确认的范围
以下属于高风险修改，必须先审视现状并等待用户同意：
- 已存在场景物体修改
- 组件 Inspector 配置修改
- Prefab 现有配置修改
- ScriptableObject 现有核心数据修改

执行细则以 `.kiro/steering/scene-modification-rule.md` 为准。

### 6.3 验证闭环
- 不允许使用 `Bash` 在后台拉起 Unity。
- 默认验证链路：
  1. `recompile_scripts`
  2. `get_console_logs`（Error = 0）
  3. `run_tests(testMode="EditMode")`
- PlayMode 验证不走 MCP 自动化；需要时转交用户手动验证。
- **不要假设不存在的工具**。若当前环境没有 `save_scene` 之类能力，直接说明并转交用户手动保存/确认。

---

## 7. `.claude/hooks` 的真实定位

- `.claude/hooks` 是**辅助层**，不是 Kiro 的 askAgent / agentStop 语义执行层。
- 它们当前的现实职责只有三类：
  1. marker
  2. reminder
  3. audit / 少量 Bash 防呆
- 它们**不会**自动读取 steering。
- 它们**不会**自动更新 memory。
- 它们**不会**替代 append-only 的工具手法。
- `.claude/settings.json` / `.claude/settings.local.json` 只是**权限表 + hook 接线板**：
  - `permissions` 负责声明放行范围
  - `hooks` 负责把脚本挂到生命周期事件
- 它们**不会**因为写了这些配置，就自动获得 Kiro 的 askAgent / agentStop 语义能力。
- 若配置里出现 `Bash(*)`：那表示权限边界放宽，不表示已经拥有可靠阻断或自动治理。
- 真正主防线是：
  - `CLAUDE.md`
  - `.kiro/steering`
  - 执行前后的 SOP
  - memory 收尾纪律

---

## 8. 手动快照指令

当用户明确输入 **`执行对话快照操作`** 时：
1. 在当前工作区下创建 `继承会话memory/` 快照文件。
2. 快照内容至少包含：
   - 用户 prompt 摘抄
   - 已完成 / 未完成进度
   - 修改文件列表
   - 关键决策
   - 待同步状态与遗留问题
3. 然后把快照路径追加记录到对应 memory。

---

## 9. 最终原则

- `CLAUDE.md` 负责调度，不负责重复抄写 steering。
- 普通 CLI 主流程的重点不是“继承幻想”，而是“结束前必记忆”。
- 有子工作区就先子后父；append-only 必须靠工具手法落实。
- hooks 只能辅助，不能背锅。
