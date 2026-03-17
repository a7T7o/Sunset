# 阶段记忆：强制skills闸门与执行规范重构

## 阶段目标
- 把 `Sunset` 当前“有 rules、有 skills、但不稳定命中”的状态，收束成“每次进入实质性任务前都会先触发正确启动闸门”的稳定机制。
- 把 `sunset-startup-guard`、项目 `AGENTS.md`、线程最小回复结构、shared root 占用判断和 `worktree` 例外口径收成同一套执行面。

## 当前状态
- **完成度**：第一轮落地已完成，第二轮验证与收紧待继续推进
- **最后更新**：2026-03-17
- **状态**：进行中

## 当前稳定结论
- `sunset-startup-guard` 已创建并可读，路径为 `C:\Users\aTo\.codex\skills\sunset-startup-guard\SKILL.md`。
- `D:\Unity\Unity_learning\Sunset\AGENTS.md` 已把“Sunset 实质性任务先过启动闸门”写成项目级硬规则。
- Unity / MCP 单实例层已独立落盘：
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-occupancy.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-hot-zones.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-log.md`
- `11_main-branch-only回归与worktree退役收口` 已真实完成；Sunset 当前现场已恢复为 `D:\Unity\Unity_learning\Sunset @ main`。
- 当前 `git worktree list --porcelain` 只剩 shared root 本身，`worktree` 已重新收束为高风险隔离/事故 cleanroom/特殊实验的例外机制。
- 因此，后续治理主线不再继续扩写 `11`，而是回到本阶段继续压实：启动闸门、skills 命中、最小回复结构、以及对 `worktree` 红线的稳定执行。

## 会话记录

### 会话 1 - 2026-03-17（承接 11 封板后的回切核验）

**用户需求**：
> 基于上一轮已完成的 `11` 终局状态，核对 Sunset 现场是否真的已经回到 `main + branch-only`，并明确现在该进入哪条后续治理主线。

**完成事项**：
1. 读取并核验 `sunset-startup-guard`、`skills-governor`、`sunset-workspace-router`、项目 `AGENTS.md` 与现行状态入口，确认当前治理主线应由启动闸门继续接管。
2. 实测核验 Git 现场：
   - `git status --short --branch` 为 clean 的 `main...origin/main`
   - `git worktree list --porcelain` 只剩 `D:/Unity/Unity_learning/Sunset`
   - 当前最新提交为 `dd83ff74`（`2026.03.17-07`）
3. 发现本阶段目录此前缺少 `memory.md`，本轮补齐该工作区记忆，避免 `11` 封板后再次出现“下一阶段入口有 tasks 但无记忆承接”的断档。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\09_强制skills闸门与执行规范重构\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地\memory_0.md`

**解决方案**：
- 把本阶段正式确认为 `11` 之后的治理恢复点。
- 以后凡是继续推进 skills 强制命中、启动闸门、四件套/AGENTS 执行约束的任务，默认都从这里续写，而不是回头再向 `11` 塞尾巴。

**遗留问题**：
- [ ] 验证 `sunset-startup-guard` 在新 session/新线程里的稳定可见性和首条 `commentary` 命中率。
- [ ] 收紧 shared root 占用口径，决定是否需要独立的 `root-workdir lease / branch occupancy` 记录面。
- [ ] 把“禁止把 `worktree` 常态化”继续内化到启动闸门输出模板与后续 AGENTS / skills 规范中。

### 会话 2 - 2026-03-17（治理总盘点与未完项分层）

**用户需求**：
> 现在项目进度到底在哪一步、哪些事情已经做完、哪些事情还没做完、每一步做完后的验收结果应该是什么。

**完成事项**：
1. 逐个复核 `000_代办/codex/01-11` 的 `tasks.md`，区分“真未完”“阶段遗留但已被后续阶段覆盖”“纯文档勾选未收口”三类状态。
2. 确认当前真实 Git 现场仍为：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `git worktree list --porcelain` 只剩 shared root
   - 最新提交 `8478077a`（`2026.03.17-08`）
3. 形成新的总判断：
   - `11` 已真完成
   - 业务线程不再需要继续分发 branch-only prompt
   - 当前真正的治理主线只剩 `09`
   - `01/02/07` 各有少量收尾项，但本质上都应服务于 `09` 的“强制执行面”而不是重新开一轮事故治理

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\09_强制skills闸门与执行规范重构\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地\memory_0.md`

**解决方案**：
- 以后对总进度的解释采用三层口径：
  - 已完成并封板：`04/05/06/08/11`
  - 历史已完成但文档可能仍有旧勾选：`03/10`
  - 当前真正活跃未完主线：`09`
  - 次级收尾债：`01` 的旧错码文档最终处置、`02` 的 skills 强制约束评估、`07` 的小文件编码修复

**遗留问题**：
- [ ] 把 `03/10` 中已被 `11` 覆盖的旧待办显式降级或封板，避免以后再被误读成“系统仍未收口”。
- [ ] 继续推进 `09` 的剩余 5 项，把“规则存在”变成“规则稳定命中”。

### 会话 3 - 2026-03-17（MCP 单实例层补建并并入启动闸门）

**用户需求**：
> 在继续完成整体清盘的同时，把 Unity / MCP 单实例冲突也正式纳入现行治理结构，不要只散落在线程回复里。

**完成事项**：
1. 吸收 `NPC / farm / spring-day1` 对 Unity / MCP 并行风险的共同判断，确认：
   - shared root 中性
   - Unity / MCP 单实例安全
   必须分层裁定。
2. 新建三份 live 文档：
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-occupancy.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-hot-zones.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-log.md`
3. 同步更新：
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `基础规则与执行口径.md`
   - `Sunset强制技能闸门与线程回复规范_2026-03-17.md`
   - `Sunset Git系统现行规则与场景示例_2026-03-16.md`
   - `Sunset当前唯一状态说明_2026-03-17.md`
   - `Sunset现行入口总索引_2026-03-17.md`
4. 复核当前 shared root 真实 `HEAD` 为 `952a1f23`。

**关键决策**：
- 今后凡是会碰 Unity / MCP 的 Sunset 实质性任务，启动闸门除了核 Git / shared root 外，还必须额外核单实例层三件套。
- 这不是另开主线，而是 `09` 当前“强制执行面”里的一个补强动作。

**遗留问题**：
- [ ] 继续验证新 session 是否稳定命中“Git 中性 + MCP 单实例层”双重前置核查。

## 相关文件
- `C:\Users\aTo\.codex\skills\sunset-startup-guard\SKILL.md` - Sunset 项目级启动闸门 skill
- `D:\Unity\Unity_learning\Sunset\AGENTS.md` - 已写入强制触发启动闸门的项目规则
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\09_强制skills闸门与执行规范重构\tasks.md` - 当前阶段任务清单
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\09_强制skills闸门与执行规范重构\design.md` - 当前阶段设计说明
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前唯一状态说明_2026-03-17.md` - 当前现行状态入口

### 会话 4 - 2026-03-17（补强 Play Mode 离场纪律、表现层验收与 shared root 实时占用口径）
**用户目标**：
> 在继续彻底清盘的同时，把“做完 Play Mode 验证后必须回到 Edit Mode”与“UI / 气泡 / 字体 / 样式要好看且专业”补成真正会命中的强约束，并把当前 shared root 的实时状态写成真相而不是旧快照。

**完成事项**：
1. 实核当前仓库现场：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - 最新治理提交：`663af03c`
   - 当前 shared root 仍带其他线程 dirty，主体是 NPC 业务文件并夹杂少量非治理记忆，因此不是实时中性现场。
2. 更新本阶段执行面：
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\SKILL.md`
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\references\checklist.md`
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-occupancy.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-hot-zones.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\steering\ui.md`
3. 补强 live 规则：
   - `基础规则与执行口径.md`
   - `Sunset强制技能闸门与线程回复规范_2026-03-17.md`
   - `Sunset当前唯一状态说明_2026-03-17.md`
   - `shared-root-branch-occupancy.md`
4. 回写本阶段 `tasks.md / design.md`，把新增两条硬约束纳入正式记录。

**关键决策**：
- `main + branch-only` 是默认入口模型，不等于 shared root 实时中性；实时能不能切分支，仍要看 `shared-root-branch-occupancy.md`。
- 任何为了取证、调试或验收进入 Play Mode 的线程，离场前都必须先 Stop 回 Edit Mode。
- UI / 对话框 / 气泡 / 字体 / 样式类任务，不再允许只以“功能通了”为完成标准；必须把观感、可读性和专业度当成硬验收。

**恢复点 / 下一步**：
- `09` 的强制执行面已继续加厚。
- 后续若还要深化治理，应继续围绕“启动闸门稳定命中”和“shared root 实时占用真相”展开，而不是回退到 `11/12`。
