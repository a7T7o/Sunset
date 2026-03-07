# Steering规则区优化 - 开发记忆（分卷）

## 模块概述

对项目中的 `.kiro/steering` 规则文件进行分析、分类和优化，减少上下文消耗，提高规则的可维护性与使用效率。

## 当前状态

- 完成度：100%
- 状态：已完成，可归档
- 备注：原 `memory.md` 已迁移为 `memory_1.md`（1480 行），此文件为新卷。

## 分卷索引

- `memory_1.md`：历史主卷（2026-01-08 ~ 2026-03-02 等）

## 会话记录
### 会话 2026-03-06
**用户需求**：
- 回顾“这段时间到底改了什么”，生成可追溯修改清单并落盘到 `Steering规则区优化/Claude迁移与规划/`；同时希望制度化为固定工作流。

**完成任务**：
- 生成并落盘《修改清单（可追溯）》：基于 `git status --short`、`git diff --name-status`、`git diff --stat` 与关键文件 diff，按 M/D/?? 分类列出所有变更，并标注高风险污染源。

**修改文件**：
- `.kiro/specs/Steering规则区优化/memory.md` - 新建：分卷（原 `memory.md`→`memory_1.md`）。
- `.kiro/specs/Steering规则区优化/Claude迁移与规划/修改清单_2026-03-06.md` - 新建：本轮修改清单落盘。

**解决方案**：
- 用 Git 取证做“事实层”闭环：先全量枚举变更，再抽取关键 diff 片段形成审计记录；最后把结果落盘到指定工作区，确保可追溯。

**遗留问题**：
- [ ] 高风险变更是否回滚需人工决策：`Assets/000_Scenes/Primary.unity`、`Assets/Saves/slot1.json`、以及 900_开篇工作区的删除/未跟踪导出污染。
- [ ] `.claude/settings.json` 放开 `Bash(*)` 的安全边界需确认是否保留。

### 会话 2026-03-06（续）
**用户需求**：
- 继续就“memory 覆盖而非追加”的执行错误进行反思，要求在 `Claude迁移与规划` 子工作区落盘分析与整改方案，并按 `workspace-memory.md` 创建/更新该子工作区 memory。

**完成任务**：
- 落盘《工作流反思与整改方案_2026-03-06.md》，明确根因、冲突矩阵、SOP 纠偏与 append-only 防复发机制。
- 在 `Claude迁移与规划` 子工作区新建 `memory.md`，记录会话 1 的用户需求、完成任务、修改文件、方案与遗留问题。
- 按“先子后主”规则，将本次摘要同步到父工作区 memory。

**修改文件**：
- `.kiro/specs/Steering规则区优化/Claude迁移与规划/工作流反思与整改方案_2026-03-06.md` - [新增]：流程复盘与整改方案报告。
- `.kiro/specs/Steering规则区优化/Claude迁移与规划/memory.md` - [新增]：子工作区开发记忆。
- `.kiro/specs/Steering规则区优化/memory.md` - [追加]：父工作区摘要同步。

**解决方案**：
- 采用“证据先行 + 最小增量整改”策略：先固化事实，再定义 P0/P1 修复清单；并将 append-only 防线前移到写前自检与 hook 阻断建议层。

**遗留问题**：
### 会话 2026-03-06（续2）
**用户需求**：
- 继续“边读边写”推进 `全面理解报告`，补齐 steering 全域审计，复核 `.claude/hooks`，并形成 `CLAUDE.md` 优先级重排与 hooks 改造清单。

**完成任务**：
- 完成第4批文档补读与结论固化（items/so-design/placeable-items/archive系）。
- 在 `全面理解报告.md` 新增“第4批结论（已完成）”。
- 在同报告新增 `CLAUDE.md` 最小改动重排草案与 `.claude/hooks` P0/P1/P2 改造优先级清单。
- 按“先子后主”规则完成本次父工作区摘要同步。

**修改文件**：
- `.kiro/specs/Steering规则区优化/Claude迁移与规划/全面理解报告.md` - [追加]：第4批结论 + 治理草案。
- `.kiro/specs/Steering规则区优化/Claude迁移与规划/memory.md` - [追加]：会话2详细记录。
- `.kiro/specs/Steering规则区优化/memory.md` - [追加]：父工作区摘要同步（本条）。

**解决方案**：
- 通过“规则补齐→风险分级→最小改动草案”将冲突从概念争议收敛为可执行清单，且保持批次化落盘防中断。

**遗留问题**：
### 会话 2026-03-06（续3）
**用户需求**：
- 对照 `讨论003.md` 重新审视整改方案，解释 `.claude/hooks` 实际功能与 Kiro Hook 的差距，并把《工作流反思与整改方案》修订为最终版。

**完成任务**：
- 完成 `讨论003.md`、Kiro Hook 说明文档、`.kiro/hooks/*` 与 `.claude/hooks/*` / `.claude/settings*.json` 的交叉核对。
- 将《工作流反思与整改方案_2026-03-06.md` 重写为最终版：撤销“Hook 主治理幻想”，明确 Claude CLI 迁移主轴是“结束前必记忆 + 先子后父 + append-only 工具手法”。
- 按“先子后主”规则完成本次父工作区摘要同步。

**修改文件**：
- `.kiro/specs/Steering规则区优化/Claude迁移与规划/工作流反思与整改方案_2026-03-06.md` - [重写]：最终版整改方案。
- `.kiro/specs/Steering规则区优化/Claude迁移与规划/memory.md` - [追加]：会话3详细记录。
- `.kiro/specs/Steering规则区优化/memory.md` - [追加]：父工作区摘要同步（本条）。

**解决方案**：
- 通过“打碎 Hook 幻觉 → 回归文档调度与工具纪律 → 锁定每次结束前必记忆”的思路，把 Kiro 灵魂迁移到 Claude CLI 的现实边界内。

**遗留问题**：
- [ ] 尚未将最终版方案正式落实到 `CLAUDE.md`。
- [ ] 尚未去神化 `.claude/hooks` 的命名/注释。
- [ ] 尚需复查本父 memory 的历史格式连续性。

### 会话 2026-03-06（续4）
**用户需求**：
- 认可“适配之外还要升级”的方向，要求直接落地：正式改写 `CLAUDE.md`，并把 `.claude/hooks` 去神化为符合 Claude CLI 现实能力的辅助层。

**完成任务**：
- 完成 `CLAUDE.md` 重写：收敛为“优先级排序 + 规则路由 + 普通会话 SOP + memory 收尾清单 + Unity/MCP 现实边界 + hooks 定位”。
- 完成 `.claude/hooks/*.sh` 改写：统一为 marker / reminder / audit 语义，移除误导性自动化表述。
- 用 `bash -n` 完成 5 个 hook 脚本语法校验。

**修改文件**：
- `CLAUDE.md` - [重写]：正式落地为 Claude CLI 调度协议。
- `.claude/hooks/domain-rule-injector.sh` - [改写]：prompt-submit marker。
- `.claude/hooks/pre-bash-block.sh` - [改写]：best-effort danger audit。
- `.claude/hooks/post-edit-format.sh` - [改写]：post-edit audit marker。
- `.claude/hooks/pre-compact-snapshot.sh` - [改写]：pre-compact reminder marker。
- `.claude/hooks/stop-update-memory.sh` - [改写]：stop reminder marker。

**解决方案**：
- 通过“CLAUDE 调度化 + hooks 审计化 + memory 收尾硬规则化”把治理主轴从幻想中的 hook 语义执行层，切回可落地、可追踪、可验证的 CLI 现实工作流。

**遗留问题**：
- [ ] `.claude/settings*.json` 的外围说明尚未同步去神化。
- [ ] `pre-bash-block.sh` 的 runner 实际阻断语义仍待后续实测。
- [ ] 本父 memory 的历史格式连续性仍需后续单独自检。

### 会话 2026-03-06（续5）
**用户需求**：
- 继续推进配置层治理，把 `.claude/settings.json` / `.claude/settings.local.json` 的外围说明也去神化，并与 `CLAUDE.md` / hooks 新定位保持一致。

**完成任务**：
- 在 `CLAUDE.md` 增补 `.claude/settings*.json` 的现实职责说明：权限表 + hook 接线板，而非 Kiro 式语义执行层。
- 在 `Claude迁移与规划/工作流反思与整改方案_2026-03-06.md`、`全面理解报告.md`、`修改清单_2026-03-06.md` 同步补齐 settings 层能力边界与语义风险说明。
- 完成“hooks 去神化”向“settings 配置层去神化”的延伸闭环。

**修改文件**：
- `CLAUDE.md` - [补充]：增加 `.claude/settings*.json` 定位说明。
- `.kiro/specs/Steering规则区优化/Claude迁移与规划/工作流反思与整改方案_2026-03-06.md` - [补充]：纳入 settings + hooks 的整体能力边界描述。
- `.kiro/specs/Steering规则区优化/Claude迁移与规划/全面理解报告.md` - [补充]：新增 settings 层定位结论。
- `.kiro/specs/Steering规则区优化/Claude迁移与规划/修改清单_2026-03-06.md` - [补充]：新增 settings 层职责与风险说明。

**解决方案**：
- 通过“脚本层诚实化 + 配置层诚实化”双步收口，把 Claude CLI 工作流治理从‘看起来像 Kiro’进一步收敛为‘现实可执行且表述一致’。

**遗留问题**：
- [ ] `pre-bash-block.sh` 的 runner 实际阻断语义仍待后续实测。
- [ ] `stop-update-memory.sh` 的 append-only 守卫仍未落地实现。
- [ ] 本父 memory 的历史格式连续性仍需后续单独自检。

### 会话 2026-03-06（续6）
**用户需求**：
- 继续，做 `pre-bash-block.sh` 的实测报告。

**完成任务**：
- 读取 `.claude/hooks/pre-bash-block.sh` 与 `.claude/settings.json`，确认脚本逻辑与 `PreToolUse` wiring 的现状。
- 对 `pre-bash-block.sh` 做 standalone 安全实测：验证空输入、普通命令、`rm -rf /`、`git reset --hard`、`git push -f`、`git push --force` 的输出与退出码。
- 通过 Claude Code Bash 工具做 runner 侧安全探针：执行 `rm --version`、`git push -f -h`、`git push --force -h`，观察是否出现可见阻断或 hook 输出。
- 落盘《pre-bash-block实测报告_2026-03-06.md》，固化“当前实现仅为 best-effort 审计层，不是可靠 blocker”的实测结论。

**修改文件**：
- `.kiro/specs/Steering规则区优化/Claude迁移与规划/pre-bash-block实测报告_2026-03-06.md` - [新增]：记录 standalone + runner 安全探针证据、结论与剩余不确定点。
- `.kiro/specs/Steering规则区优化/Claude迁移与规划/memory.md` - [追加]：记录本次实测过程与报告落盘。
- `.kiro/specs/Steering规则区优化/memory.md` - [追加]：同步父工作区摘要（本条）。

**解决方案**：
- 采用“脚本单测 + runner 安全探针 + settings 对照”三层取证法，在不执行真实破坏操作的前提下确认 `pre-bash-block.sh` 只具备危险模式识别/提示能力，且当前 wiring 与脚本覆盖面并不完全一致。

**遗留问题**：
- [ ] 仍需在受控隔离环境中验证：若 `PreToolUse` 命中后返回非零退出码，Claude runner 是否真正阻断 Bash 调用。
- [ ] `Bash(rm*|git reset --hard*|git push -f*)` 的 matcher 精确语义仍待进一步官方文档或对照实验补锚，尤其是 `git push --force` 是否会命中 wiring。
- [ ] `stop-update-memory.sh` 的 append-only 守卫仍未实现。

### 会话 2026-03-06（续7）
**用户需求**：
- 纠正另一终端先把规划写到 `C:\Users\aTo\.claude\plans\...` 的错误流程，要求将该问题先按代办规则入账，再分析如何防止工作区先行制度再次失守。

**完成任务**：
- 复核 `CLAUDE.md`、`rules.md`、`workspace-memory.md` 与 `000_代办` 现有样例，确认当前框架缺少“禁止把全局 plans 作为正式项目产物”的硬门槛。
- 新建子代办 `TD_Claude迁移与规划.md`，并在主代办 `TD_000_Steering规则区优化.md` 汇总该项，正式记录“工作区先行缺口 / 代办先行缺口 / PlanMode 替代策略”。
- 更新 `CLAUDE.md` 与《工作流反思与整改方案_2026-03-06.md`，将“先工作区 / 先代办 → 再规划落盘”固化为新增硬规则。

**修改文件**：
- `CLAUDE.md` - [追加]：新增工作区先行硬门槛。
- `.kiro/specs/000_代办/Steering规则区优化/TD_Claude迁移与规划.md` - [新增]：记录本次流程漏洞。
- `.kiro/specs/000_代办/Steering规则区优化/TD_000_Steering规则区优化.md` - [追加]：补入子代办索引与 P0 汇总。
- `.kiro/specs/Steering规则区优化/Claude迁移与规划/工作流反思与整改方案_2026-03-06.md` - [追加]：补强工作区先行 / 代办先行 SOP。
- `.kiro/specs/Steering规则区优化/Claude迁移与规划/memory.md` - [追加]：记录会话7详细过程。

**解决方案**：
- 通过“工作区先行硬门槛 + 代办先行漏斗”双重约束，把全局 PlanMode 产物降级为临时机制，项目正式规划统一回落到 `.kiro/specs/当前工作区/` 内。

**遗留问题**：
- [ ] 仍需继续观察：后续是否还会出现先全局计划、后补工作区的流程漂移。
- [ ] 若再出现新的项目外正式产物路径，还需继续补强治理文本。
- [ ] `stop-update-memory.sh` 的 append-only 守卫仍未实现。

### 会话 2026-03-06（续8）
**用户需求**：
- 禁止自动进入 PlanMode，保持 acceptEdits，并继续完成 hooks / matcher / append-only 治理闭环。

**完成任务**：
- 移除 `EnterPlanMode`、补齐 `git push --force*` matcher、落实 `stop-update-memory.sh` append-only 检查，并把 acceptEdits 常驻 / 二次实测结论同步到 `CLAUDE.md` 与治理文档。

**修改文件**：
- `CLAUDE.md`、`.claude/settings*.json`、`.claude/hooks/pre-bash-block.sh`、`.claude/hooks/stop-update-memory.sh`、`Claude迁移与规划/*`、`000_代办/*`

**解决方案**：
- 用“acceptEdits 常驻 + 工作区文档规划 + hooks 仅辅助 + append-only 检查器”四层收口，避免自发 PlanMode 漂移。

**遗留问题**：
- [ ] 仍需继续观察 `PreToolUse` / `Stop` 在真实 runner 生命周期中的阻断语义。
