# 当前运行基线与开发规则 - 开发记忆（分卷）

> 本卷起始于 `memory_0.md` 之后。旧长卷已完整归档到 `memory_0.md`。

## 模块概述
- 本工作区负责维护 Sunset 当前唯一有效的活文档入口，包括状态说明、Git 规则、L5 开工总览、共享表、四件套规范与总索引。
- 它不是迁移期历史堆场，也不是新的总治理代办池；当前治理续办已转入 `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex`。

## 当前状态
- **完成度**: 90%
- **最后更新**: 2026-03-16
- **状态**: 常态维护中

## 分卷索引
- `memory_0.md`：2026-03-14 ~ 2026-03-16 的完整长卷，覆盖 L5 收口、锁治理、热文件固化审核、现行入口重建、治理 skills 落地、冻结汇总归档、旧首版退役与活目录瘦身。

## 承接摘要

### 最近归档长卷的稳定结论
- 当前唯一有效活文档根目录仍是：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则`
- 当前默认入口已经稳定为：
  - `Sunset当前唯一状态说明_2026-03-16.md`
  - `Sunset Git系统现行规则与场景示例_2026-03-16.md`
  - `Sunset L5开工清单与治理续办总览_2026-03-16.md`
  - `Sunset工作区四件套与代办规范_2026-03-16.md`
- `03-13` 历史状态正文与 `03-15` 首版共享表已退役出活目录，历史去向已写入 `06_L5解冻与阶段快照_2026-03-16`。
- 当前治理型 skills 已形成四件套：
  - `sunset-thread-wakeup-coordinator`
  - `sunset-lock-steward`
  - `sunset-doc-encoding-auditor`
  - `sunset-release-snapshot`

### 当前恢复点
- 本工作区后续不再继续承接大规模历史叙事，重点变为“保持现行入口准确、精简、可接手”。
- 当前优先治理债已收紧为：
  - 超长 `memory` 分卷治理
  - 剩余历史错码 / 编码健康巡检
  - 四件套规范在新阶段工作区中的持续落地
- `sunset-todo-router` 当前结论仍是“暂不立即新建”，继续由 `skills-governor + sunset-workspace-router + 现行四件套规范` 共同承担代办路由。

## 会话记录

### 会话 1 - 2026-03-16（活文档工作区 memory 分卷重建）

**用户需求**:
> 继续做，不要停；在不打扰 `farm` 业务现场的前提下，把你已经发现的治理续办继续推进，尤其是当前超长 `memory`、代办结构和规则补强问题。

**完成任务**:
1. 将旧长卷 `memory.md` 完整归档为 `memory_0.md`。
2. 新建本精简活跃卷，补齐分卷索引、承接摘要与当前恢复点。
3. 将本工作区后续角色收紧为“现行入口维护”，不再继续把历史叙事和阶段尾账无限叠加到主卷。

**修改文件**:
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\memory.md`

**关键决策**:
- 活文档工作区的长卷历史应完整保留，但活跃卷必须恢复成可直接接手的摘要入口。
- 当前活文档层的下一步工作不再是继续扩写旧过程，而是维护当前入口准确性与轻量度。

**验证结果**:
- 旧长卷已从活跃入口安全迁为 `memory_0.md`。
- 当前卷已具备分卷索引、承接摘要与恢复点，后续可以继续在本卷追加。

**遗留问题**:
- [ ] 仍需把线程记忆与父治理工作区记忆同步分卷完成。
- [ ] 仍需将本轮分卷约定补入现行规则文档。

### 会话 2 - 2026-03-16（分卷约定已补入现行规则）

**完成任务**:
1. 将“工作区活跃卷固定为 `memory.md`、线程活跃卷固定为 `memory_0.md`”写入：
   - `Sunset工作区四件套与代办规范_2026-03-16.md`
   - `基础规则与执行口径.md`
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
2. 同步完成父治理工作区和线程记忆的分卷，关闭本卷会话 1 中的遗留项。

**验证结果**:
- 工作区与线程的分卷命名差异已经有明文规则，不再需要临场判断。
- 当前活文档工作区 `memory.md` 保持 `53` 行，已恢复为可直接接手的健康入口。

**恢复点**:
- 本工作区后续继续只保留高密度摘要与少量新记录。
- 下一组治理债优先转向历史错码对象巡检与新活文档入口的持续校准。

### 会话 3 - 2026-03-16（现行总索引独立化与 Git 边界纠偏）

**完成任务**:
1. 新建 `Sunset现行入口总索引_2026-03-16.md`，把当前总索引从旧 `03-13` 文件里独立出来。
2. 将 `文档重组总索引_2026-03-13.md` 改写为历史路由页，不再冒充现行总索引。
3. 将 `git-safe-sync.ps1` 当前真实边界补回现行 Git 文档：
   - `governance` 默认自动白名单只剩 `.gitattributes`、`.gitignore`、`AGENTS.md`、`scripts/`、`.kiro/steering/`、`.kiro/hooks/`
   - 活文档、代办工作区、线程记忆同步时仍必须显式传 `IncludePaths` / `ScopeRoots`
4. 把编码巡检结论补入状态说明和 L5 总览，明确当前大量乱码感属于 `Default` 读法噪音。
5. 从 L5 总览中移除 `Skills和MCP` 的复工 Prompt，只保留其归档身份说明。

**修改文件**:
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset现行入口总索引_2026-03-16.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\文档重组总索引_2026-03-13.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前唯一状态说明_2026-03-16.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset Git系统现行规则与场景示例_2026-03-16.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset L5开工清单与治理续办总览_2026-03-16.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\基础规则与执行口径.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\tasks.md`

**验证结果**:
- 当前活文档层已经不再把旧 `03-13` 索引文件继续当现行总索引。
- Git 文档、状态说明、L5 总览和基础规则对脚本真实行为已经重新对齐。

**恢复点**:
- 本工作区后续继续承担“现行入口维护”，不再承接旧命名文件的历史包袱。
- 下一步可以继续做 07 阶段抽样巡检，或转入下一批超长活跃 `memory` 主卷治理。

### 会话 4 - 2026-03-16（共享层快照 branch/HEAD 已按 preflight 纠正）

**完成任务**:
1. 运行 `git-safe-sync.ps1 -Action preflight -Mode governance` 复核执行层。
2. 发现当前物理工作树已从旧的 farm 分支切换到：
   - `codex/npc-main-recover-001 @ 8aed637f`
3. 立即修正共享层快照：
   - `Sunset活跃线程总表_2026-03-16.md`
   - `Sunset当前工作树dirty与WIP归属表_2026-03-16.md`

**验证结果**:
- 文档中的“当前实际工作树分支 / HEAD”已与脚本 preflight 输出重新一致。
- 当前共享层关于 `NPC` 优先收口的判断，已从“推理”升级为“也与当前物理 worktree 落点一致”。

**恢复点**:
- 本工作区后续若再写共享层快照，必须先以脚本 / CLI 当前输出为准，不再沿用旧会话印象。

### 会话 5 - 2026-03-17（只读总结当前 skills 与规则理解）

**用户目标**:
- 不做任何新建或规则修改，只要求基于当前现行入口、AGENTS、skills 与 Git 现场，明确回答“现在 Sunset 的 skills、Git 和治理规则到底怎么理解”。

**本轮处理**:
1. 只读复核现行活文档入口：
   - `Sunset当前唯一状态说明_2026-03-16.md`
   - `Sunset Git系统现行规则与场景示例_2026-03-16.md`
   - `Sunset现行入口总索引_2026-03-16.md`
   - `Sunset工作区四件套与代办规范_2026-03-16.md`
2. 只读复核 `D:\Unity\Unity_learning\Sunset\AGENTS.md` 当前 Sunset 项目路由口径。
3. 只读复核当前已安装治理型 skills 与 `02_skills_AGENTS_与执行机制重构/tasks.md`。
4. 只读运行 `git status --short --branch`，重新确认当前物理 worktree 仍在：
   - `codex/npc-main-recover-001...origin/codex/npc-main-recover-001`

**关键结论**:
- 当前现行入口已经明确切到 `2026-03-16` 这一组文件；`03-13` 与 `03-15` 旧文件只剩历史或退役身份。
- 当前 Git 规则的真实核心是：
  - 读取现场默认从主项目目录进入；
  - 治理类走 `main + governance`；
  - 业务实现走 `codex/... + task`；
  - A 类热文件先查锁再写；
  - 一切以 CLI / `git-safe-sync.ps1` 真实结果为准。
- 当前已健康可用、且直接服务治理主线的 skills 至少包括：
  - `skills-governor`
  - `global-learnings`
  - `sunset-workspace-router`
  - `sunset-thread-wakeup-coordinator`
  - `sunset-lock-steward`
  - `sunset-doc-encoding-auditor`
  - `sunset-release-snapshot`
  - `sunset-unity-validation-loop`
- 当前 skills 体系的主要缺口已不再是“完全没有 skill”，而是“是否要把 Sunset 启动前置闸门做成更强约束”的那一步尚未落地。
- 当前默认基线文档写的是 `main@9b9a6bd...` 已稳定，但这不等于当前物理 worktree 也在 `main`；本轮再次确认真实 worktree 仍然是 `codex/npc-main-recover-001`，且仓库内仍混有多线程 dirty / untracked。

**恢复点**:
- 本工作区当前已经有足够清晰的现行规则入口，可直接用于向用户解释当前 skills、Git 与治理规则体系。
- 下一步若用户恢复“继续建设必要 skill / 继续治理续办”，优先进入 `000_代办/codex/02_skills_AGENTS_与执行机制重构`，而不是回头改旧活文档文件名或旧全局 `tasks.md`。

### 会话 6 - 2026-03-17（下一阶段状态文件与技能闸门专项已建档）

**用户目标**:
- 先快速完成当前活文档层的初步记录和入口补建；
- 然后把剩余大项全部转入代办，好让治理线程马上切回 `NPC` 修复。

**完成任务**:
1. 新建 `Sunset当前唯一状态说明_2026-03-17.md`，正式记录：
   - 治理主线已从 L5 入口梳理，转入“强制 skills 闸门与执行规范重构”
   - 当前因为 `NPC` 问题，只先完成建档与收口，不继续扩写
2. 新建 `Sunset现行入口总索引_2026-03-17.md`，把 `09` 阶段和新专项文档接入现行入口。
3. 新建 `Sunset强制技能闸门与线程回复规范_2026-03-17.md`，把下一阶段要补的执行闸门问题正式写明。
4. 同步更新：
   - `基础规则与执行口径.md`
   - 兼容 `tasks.md`

**关键结论**:
- 当前活文档层已经不再只围绕 `03-16` 这一组文件运转；
- `03-17` 起，现行入口已经正式承认“强制 skills 闸门缺失”是下一阶段治理重心。

**恢复点**:
- 本工作区后续如果继续治理，不再从“解释旧规则”开始，而是直接进入 `09` 阶段承接强制 skills 闸门与线程回复规范重构。
- 当前则允许优先切回 `NPC` 修复主线。

### 会话 7 - 2026-03-17（共享根仓库分支漂移已进入现行入口）

**用户目标**:
- 彻查 `NPC` / `farm` 当前这轮混线到底怎么发生；
- 判断“分支是否也该成为锁的一部分”；
- 如有必要，单独立项。

**完成任务**:
1. 取证确认当前共享根目录真实现场：
   - `D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-correct001 @ 11e0b7b4`
2. 取证确认旧 NPC worktree 仍存在：
   - `D:\Unity\Unity_learning\Sunset_worktrees\NPC @ codex/npc-generator-pipeline`
3. 新建阶段：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\10_共享根仓库分支漂移与现场占用治理`
4. 在当前状态说明和当前总索引中接入 `10` 阶段。

**关键结论**:
- 当前新暴露的问题不是 A 类文件锁本身，而是“共享根工作目录的分支上下文没有被当作显式占用资源管理”。
- 后续需要的不是简单把“分支”塞进文件锁，而是补一层：
  - 根工作目录 / 当前分支 / 当前 `HEAD` 占用校验。

**恢复点**:
- 本工作区后续若继续治理，除 `09` 阶段外，还应同时参考 `10` 阶段分析。
- 当前已允许结束本轮调查，切回 `NPC` 修复主线。

### 会话 8 - 2026-03-17（10 阶段执行方案已形成）

**用户目标**:
- 在拿到 farm 证词后，不再停留在事故分析，而是给出明确的处置方案。

**完成任务**:
1. 在 `10_共享根仓库分支漂移与现场占用治理/` 下新增：
   - `执行方案.md`
2. 明确裁定：
   - 共享根目录进入业务写入冻结
   - 当前共享根目录视为 farm 的取证占用现场
   - 旧 NPC worktree 先保留为历史检出点
   - NPC 走 `codex/npc-roam-phase2-001` 救援落点
   - farm 停止沿污染分支追加开发，转入 cleanroom 重建思路
3. 在当前状态说明和当前总索引中同步接入 `10` 阶段。

**关键结论**:
- 当前现行入口已经不只是“知道问题”，而是已经包含了临时冻结和分流收口的明确方向。

**恢复点**:
- 本工作区后续若继续治理，应以 `10/执行方案.md` 作为当前这次事故的第一执行入口。

### 会话 10 - 2026-03-19（Unity MCP 当前读链复核通过；图驱动基础场景搭建具备执行条件）
**用户目标**:
> 先测试现在的 MCP 链接是否正常；再评估“给一张图片 + prefab 索引/目录，是否能用 MCP 和初始化手段自动搭一个高精度基础场景，为后续微调节省工作量”。
**本轮处理**:
1. 按 `sunset-unity-validation-loop` 的“最小有用检查优先”原则，只做当前只读验证，不主动触发代码/场景写入。
2. 通过 Unity MCP 成功执行：
   - `manage_scene(action=get_active)`：返回活动场景 `Primary`
   - `read_console(action=get)`：成功返回最新 Console 日志
   - `manage_scene(action=get_hierarchy)`：成功返回 `Primary` 根层级
3. 当前 Console 中可见 `MCP-FOR-UNITY` 在 `8080` 端口停止后重新启动本地 HTTP server 的日志，说明这轮请求链确实打到了当前 Unity 侧服务。
4. 结合现有 MCP 工具能力与 `scene-modification-rule.md`，形成“看图搭基础场景”的当前能力评估：
   - 如果是 **2D / 正交 / prefab 映射清晰** 的基础场景，可行度高；
   - 如果是 **单张透视图驱动的复杂 3D 场景**，可行度明显下降；
   - 高精度前提不是只给图片，还需要 prefab 索引、命名规则、尺度参考、目标场景路径与允许的自动化边界。
**关键结论**:
- 当前 Unity MCP **读链正常**；至少活动场景、Console、层级三类只读调用都已稳定返回，不能再把现状表述成“当前 MCP 不通”。
- 当前仍未在本轮复跑 `refresh_unity`、Prefab 写入、EditMode tests，因此更精确的现状口径应是：**当前读链正常，写链/验证链本轮未复测**。
- “图片 + prefab 索引/目录 -> 自动搭基础场景”当前可以做，但应定位为“高质量初稿 + 后续微调”，不是“单张图一次性 100% 还原”。
**恢复点**:
- 如果用户后续提供图片与 prefab 索引，本工作区可继续先出一份“原有配置 / 问题原因 / 建议修改 / 风险影响”的场景搭建方案，再决定是否在隔离场景或目标场景里落地。
### 会话 47 - 2026-03-23（遮挡检查 preflight 风险复核）
**用户需求**：检查“遮挡检查”线程的中间思考，判断是否已经出现危险信号。
**当前主线目标**：继续 MCP 根因治理，阻断线程在 `Primary.unity` 写入前因为错误端口口径而误判 live 现场。
**本轮子任务**：只读复核 `config.toml`、仓库基线文档、占用文档与基线自检脚本，判断这是不是一次真实的 preflight 失败。
**已完成事项**：
1. 只读确认当前 live `config.toml` 再次漂回旧端口口径（已失效）。
2. 只读确认仓库唯一有效基线仍然是：
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-live-baseline.md -> unityMCP + http://127.0.0.1:8888/mcp`
3. 只读确认服务层现场是“8888 活着，但配置层漂移了”：
   - `D:\Unity\Unity_learning\Sunset\Library\MCPForUnity\RunState\mcp_http_8888.pid` 存在
   - `Test-NetConnection 127.0.0.1 -Port 8888` 为 `True`
4. 运行 `D:\Unity\Unity_learning\Sunset\scripts\check-unity-mcp-baseline.ps1`，结果明确为：
   - `baseline_status: fail`
   - `issues: unexpected_endpoint`
5. 只读确认当前会话的 `list_mcp_resources` / `list_mcp_resource_templates` 为空，说明即便 8888 监听存在，也不能把本会话直接当作“已拿到可安全写入的 Unity live 入口”。

**关键结论**：
- 这股“危险气息”是真的，不是多心。
- 危险点不在于 8888 服务死了，而在于“配置层重新漂到旧端口口径（已失效）、服务层仍在 8888、会话层又没有拿到明确资源暴露”，三层事实再次分叉。
- 在这种现场下，如果线程继续把 `Primary.unity` 当作可直接写入的单实例现场，就是典型的 preflight 未完成即准备落刀。
- 因为 `Primary.unity` 同时命中 shared root 热区和 Unity/MCP 单实例热区，所以这里必须把结论定性为“应先阻断写入，再回正 MCP 基线”，而不是“边写边试”。

**恢复点 / 下一步**：
- 下一步优先回正 `C:\Users\aTo\.codex\config.toml` 到 `http://127.0.0.1:8888/mcp`，并重新做一次基线脚本 + 会话资源暴露复核。
- 在这两个检查重新通过前，不应允许任何线程把 `Primary.unity` 的 MCP preflight 视为通过。
### 会话 48 - 2026-03-23（用户称已改 config，但 live 复核仍未回正）
**用户需求**：用户表示已修正配置，要求确认当前是否还有别的问题。
**当前主线目标**：继续完成 MCP 基线回正，确认 shared root live 入口是否真正恢复一致。
**本轮子任务**：只读复核 `config.toml` 与基线脚本，不碰业务文件。
**已完成事项**：
1. 只读确认 `C:\Users\aTo\.codex\config.toml` 的 `url` 当时仍落在旧端口口径（已失效）。
2. 再次运行 `D:\Unity\Unity_learning\Sunset\scripts\check-unity-mcp-baseline.ps1`。
3. 脚本结果仍为：
   - `baseline_status: fail`
   - `issues: unexpected_endpoint`
4. 同时确认服务层仍是 8888：
   - `D:\Unity\Unity_learning\Sunset\Library\MCPForUnity\RunState\mcp_http_8888.pid` 存在
   - Terminal Script 仍写 `--http-url http://127.0.0.1:8888`

**关键结论**：
- 当前最主要的问题还没变：配置层仍未回正到 8888。
- 所以现在还不能说“别的问题都没有了”；眼下最大的唯一问题仍然是 `config.toml` 和 live 基线不一致。

**恢复点 / 下一步**：
- 先把 `C:\Users\aTo\.codex\config.toml` 里的 `url` 真正改成 `http://127.0.0.1:8888/mcp` 并保存。
- 保存后再复跑一次基线脚本；如果脚本变为 `baseline_status: pass`，再继续看是否存在会话层问题。
### 会话 48 - 2026-03-23（MCP 基线已回正，但会话层仍待证明）
**用户需求**：用户已手动把 `config.toml` 改回 `8888`，要求确认现在是否还有别的问题。
**当前主线目标**：继续把 Sunset 的 MCP live 基线从“靠人记得住”推进到“配置层、服务层、会话层都一致”。
**本轮子任务**：只读复核配置、监听、pidfile、基线脚本，并补看当前会话层是否已经拿到 MCP 暴露。
**已完成事项**：
1. 只读确认：
   - `C:\Users\aTo\.codex\config.toml -> [mcp_servers.unityMCP].url = "http://127.0.0.1:8888/mcp"`
2. 只读确认：
   - `127.0.0.1:8888` 正在监听
   - `D:\Unity\Unity_learning\Sunset\Library\MCPForUnity\RunState\mcp_http_8888.pid` 存在，值为 `1768`
3. 运行 `D:\Unity\Unity_learning\Sunset\scripts\check-unity-mcp-baseline.ps1`，结果已转为：
   - `baseline_status: pass`
4. 继续检查当前会话层，`list_mcp_resources` 与 `list_mcp_resource_templates` 仍为空，说明“配置层 + 服务层”已经回正，但“会话层已证明挂上正确 MCP”这一条，本轮仍无法由当前会话直接证实。
5. 只读看到 shared root 仍有其他线程业务 dirty / 资产 dirty，说明即便 MCP 基线变绿，也不能直接把 shared root 解释为“可自由进入 `Primary.unity` 写态”。

**关键结论**：
- 好消息：MCP 基线脚本已经通过，最核心的 `8080/8888` 配置漂移问题此刻已被纠正。
- 剩余问题主要有两个：
  1. 当前会话层还没有拿到可见的 MCP resources/templates 暴露，不能把这条会话直接当成“已完全证明 live 读写链可用”。
  2. shared root 仍不是 clean neutral 现场，不能因为 MCP 绿了就跳过热区 / 占用 / 脏改核查。

**恢复点 / 下一步**：
- 如果下一步要做真正的 Unity/MCP live 写入，仍应补一轮“会话层实际可调用”验证，再决定是否放行 `Primary.unity` 写态。
- 如果只是回答“现在大问题还在不在”，答案是：最大的问题已经修正，但还不能把整个现场宣布为完全无风险。
### 会话 49 - 2026-03-23（旧线程缓存未刷新已进入现行口径）
**用户需求**：用户要求继续把 MCP 根因从“端口改对”推进到“彻底拔掉旧线程误判的根源”。
**当前主线目标**：让 Sunset 当前活入口不再把旧线程 / 旧会话报错直接外推成服务端回滚。
**本轮子任务**：只读复核会话日志，并把稳定结论同步进 live 规则、prompt 与脚本。
**已完成事项**：
1. 只读确认 shared root `main` 下：
   - `config.toml` 已回正到 `8888`
   - `check-unity-mcp-baseline.ps1` 已 `pass`
2. 只读确认旧线程会话日志里存在“已经读到 8888 配置，但后续请求仍继续打旧端口”的证据，说明旧线程误判不止来自配置层，也来自会话路由缓存未刷新。
3. 已把这条口径同步进：
   - `AGENTS.md`
   - `Sunset当前规范快照_2026-03-22.md`
   - `mcp-live-baseline.md`
   - 批次 `README.md`
   - 通用前缀 prompt
   - `scene-build` 前缀 prompt
   - `check-unity-mcp-baseline.ps1`
4. 已将当前 active memory 中剩余的旧端口字面改写为“旧端口口径（已失效）”。

**关键结论**：
- 今后如果 `config + 8888 + pidfile + baseline` 都通过，但旧线程仍报旧端口 / 旧桥名 / `resources/templates` 为空，默认先判为“旧会话 MCP 路由缓存未刷新”。
- 这类情况要优先换新线程 / 新会话，或先手工对 `8888` 做 initialize + `tools/list`，而不是直接把服务端重新定性为坏了。

**恢复点 / 下一步**：
- 当前活入口已经补上这条根因口径；下一步只剩白名单收口与继续观察真实线程回执。

### 会话 50 - 2026-03-27（live 规则源收敛与 startup-guard discovery gap 口径补齐）

**用户需求**：
> 直接去落地三件当前不符合预期的内容：
> 1. 规范源仍有双源
> 2. `sunset-startup-guard` 仍是设计在前、实盘缺位
> 3. 治理线自己还有尾账

**当前主线目标**：
- 不再停留在“规则分析”，而是直接把 Sunset 当前 live 入口、通用 steering 边界与启动闸门 skill 的实盘口径改成单一事实入口。

**本轮完成事项**：
1. 重新收敛 live 入口层：
   - `AGENTS.md`
   - `Sunset当前规范快照_2026-03-22.md`
   - `Sunset当前唯一状态说明_2026-03-17.md`
   - `Sunset现行入口总索引_2026-03-17.md`
   - `基础规则与执行口径.md`
2. 在上述 live 入口中显式写死：
   - 当前 Sunset live 规则层级是：`用户裁定 -> AGENTS.md -> 当前规范快照 -> 命中的治理规范正文 -> .kiro/steering`
   - `.kiro/steering` 继续承担通用结构、领域细则与历史通用基线，但不再单独定义当前 shared-root / main-only / dispatch / startup-guard 的 live 默认。
3. 给通用 steering 明确边界说明：
   - `git-safety-baseline.md`
   - `workspace-memory.md`
   - `documentation.md`
4. 回正 shared root 占用文档的实时政策字段：
   - `shared-root-branch-occupancy.md`
   - 其中 `daily_policy` 已改回 `main-only + whitelist-sync + exception-escalation`
   - `last_verified_head` 已按本轮只读核验更新为 `1401ae8c`
5. 同步补强 `Sunset强制技能闸门与线程回复规范_2026-03-17.md`：
   - 明确 `sunset-startup-guard` 未显式暴露时属于 discovery gap
   - 继续要求 manual-equivalent + trigger log

**关键结论**：
- 这轮没有去追历史 memory / design 的所有旧表述，而是只修“后续线程现在还会继续读到的 live 入口”。
- 现在 live 口径不再允许出现：
  - `AGENTS.md` 说一套
  - `.kiro/steering` 被误当另一套 live 默认
  的双源状态。

**恢复点 / 下一步**：
- 现行活入口已经收敛到同一层级说法；
- 下一步转入：
  1. 补 `sunset-startup-guard` skill / 注册表 / trigger-log 的审计层记录
  2. 再对白名单治理路径尝试正式 sync

### 会话 51 - 2026-03-27（live 入口收敛白名单已正式 sync）

**完成事项**：
1. 已按 stable launcher + `governance` 白名单正式同步本工作区涉及的 live 入口改动。
2. 本轮治理提交已进入 `main`：
   - `101e160c`
   - commit message：`2026.03.27_Codex规则落地_01`

**关键结论**：
- 这轮“live 规则源收敛”不是停在本地编辑，而是已经成为 shared root 的仓库历史事实。

**恢复点**：
- 当前本工作区本轮 own 路径已 clean；
- 剩余 dirty 留给导航、农田、开篇与 hot / mixed 现场继续各自处理。
