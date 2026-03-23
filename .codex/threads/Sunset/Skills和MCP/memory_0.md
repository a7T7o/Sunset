# Skills和MCP - 线程记忆

## 线程概述
- 线程名称：Skills和MCP
- 线程分组：Sunset
- 线程定位：用于沉淀 Sunset 项目内与 Skills、MCP、Unity 工具链验证闭环直接相关的主线，而不是继续混写在综合治理线程中。
- 来源说明：本线程于 2026-03-16 从 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地\memory_0.md` 中抽离建立；源线程历史保留不删，本线程负责后续独立接手。

## 当前状态
- **完成度**：85%
- **最后更新**：2026-03-16
- **状态**：已完成独立迁移；冻结期只保留只读复核、路径承接与文档落盘，不继续扩展实现。

## 当前有效入口
- 线程记忆入口：`D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Skills和MCP\memory_0.md`
- 现行规则入口：`D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\memory.md`
- 当前唯一状态说明：`D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前唯一状态说明_2026-03-13.md`
- 活跃 Hook / MCP 使用说明：`D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\hook事件触发README.md`
- 历史归档入口：`D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\文档归档\2026-03-Codex恢复与迁移收口`
- 全局交叉材料：`D:\迅雷下载\开始\.codex\threads\系统全局\000_git治理交叉内容\Skills和mcp.md`

## 已迁入主线摘要

### 2026-03-07：项目专用 Skills 首轮落地
- 已落地 `sunset-workspace-router`、`sunset-scene-audit`、`sunset-review-router` 三个项目专用 Skill。
- 已补建 `D:\Unity\Unity_learning\Sunset\History\2026.03.07-Claude-Cli-历史会话交接\总索引.md`，作为历史交接统一入口。
- 相关历史产物已归档到：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\文档归档\2026-03-Codex恢复与迁移收口\01_迁移准备与候选比较_2026-03-07_2026-03-10\已落地项目专用Skills清单_2026-03-07.md`

### 2026-03-10：Unity MCP 接入与验证闭环补齐
- 已新增 `sunset-unity-validation-loop`，补齐 Sunset 的 Unity 验证闭环 Skill。
- 已形成两份关键方案文档：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\文档归档\2026-03-Codex恢复与迁移收口\01_迁移准备与候选比较_2026-03-07_2026-03-10\Unity-MCP候选对比_2026-03-07.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\文档归档\2026-03-Codex恢复与迁移收口\01_迁移准备与候选比较_2026-03-07_2026-03-10\Unity-MCP迁移试装方案_2026-03-10.md`
- 已明确 `Install Skills` 与 `Install Roslyn DLLs` 不是当前必需项，Kiro 显示的“51 个 MCP 工具”主要是能力数量展示，不等于 51 个后台进程。
- 旧结论保留：2026-03-10 曾成功读取 94 个 EditMode tests，并跑通一次 `94/94 succeeded`；这是历史已记录结果，本轮迁移未重跑，只保留为旧结论。

### 2026-03-13：旧桥误用排查、弹窗定位与旧桥清退
- 已确认 `农田交互修复V2` 线程里“用不了 MCP”的根因是旧 `旧 MCP 桥口径（已失效）` 误用，而不是 Unity Server 没起。
- 已确认在当前 Windows 环境里，主动对比探测旧桥可能触发前台系统弹窗，因此后续验证默认只测新桥 `unityMCP`。
- 已将旧桥从活配置、活文档示例和活进程层面清退，相关处理背景可追溯至：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\文档归档\2026-03-Codex恢复与迁移收口\00_迁移总纲与过程规则_2026-03-07_2026-03-13\OpenClaw终端闪窗排查与止血报告_2026-03-10.md`

### 2026-03-15：新桥单线收口
- 旧结论保留：用户重启客户端后，会话已确认只走 `unityMCP`；当时已成功完成活动场景读取、Console 读取与 `refresh_unity` 最小验证闭环。
- 本轮迁移已复核到两条仍然成立的当前事实：
  - `C:\Users\aTo\.codex\config.toml` 当前只读到 `[mcp_servers.unityMCP]`
  - Unity live 仍可成功读取活动场景 `Primary` 与最新 Console
- 本轮未重新复跑 `refresh_unity` 与 EditMode tests，因此“完整最小闭环仍然稳定”目前只能继续保留为旧结论，尚未升级为本轮已复核事实。

## 关键文件索引

### Skills 目录
- `C:\Users\aTo\.codex\skills\sunset-workspace-router`
- `C:\Users\aTo\.codex\skills\sunset-scene-audit`
- `C:\Users\aTo\.codex\skills\sunset-review-router`
- `C:\Users\aTo\.codex\skills\sunset-unity-validation-loop`

### 文档产物
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\文档归档\2026-03-Codex恢复与迁移收口\00_迁移总纲与过程规则_2026-03-07_2026-03-13\Codex工作流与技巧手册.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\文档归档\2026-03-Codex恢复与迁移收口\00_迁移总纲与过程规则_2026-03-07_2026-03-13\OpenClaw终端闪窗排查与止血报告_2026-03-10.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\文档归档\2026-03-Codex恢复与迁移收口\01_迁移准备与候选比较_2026-03-07_2026-03-10\Unity-MCP候选对比_2026-03-07.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\文档归档\2026-03-Codex恢复与迁移收口\01_迁移准备与候选比较_2026-03-07_2026-03-10\Unity-MCP迁移试装方案_2026-03-10.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\文档归档\2026-03-Codex恢复与迁移收口\01_迁移准备与候选比较_2026-03-07_2026-03-10\已落地项目专用Skills清单_2026-03-07.md`

### 源线程与冻结汇总
- 源线程记忆：`D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地\memory_0.md`
- 当前冻结快照：`D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\2026.03.16冻结文档汇总\Skills和MCP.md`

## 会话记录

### 会话 1 - 2026-03-16

**用户需求**:
> 用户明确更正：`D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Skills和MCP` 才是当前 Codex 线程的正确工作区；要求先把既有内容找到并安全迁移，再把当前线程内容写入 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\2026.03.16冻结文档汇总\Skills和MCP.md`。

**完成任务**:
1. 确认 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Skills和MCP` 在本轮写入前为空目录。
2. 回读 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地\memory_0.md`，抽离 2026-03-07 至 2026-03-15 间与 Skills / MCP 直接相关的主线。
3. 复核四个项目专用 Skill 目录、相关归档文档、当前活规则入口、`C:\Users\aTo\.codex\config.toml`、Unity 活动场景与 Console 读链状态。
4. 新建本线程 `memory_0.md` 作为独立接手入口，同时在冻结汇总目录落盘 `Skills和MCP.md`。
5. 在源线程 `Codex规则落地` 中追加迁移说明，明确后续 Skills / MCP 主线应优先从本线程继续。

**修改文件**:
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Skills和MCP\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\2026.03.16冻结文档汇总\Skills和MCP.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\2026.03.16冻结文档汇总\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地\memory_0.md`

**关键决策**:
- 安全迁移不采用“剪切旧线程历史”，而采用“保留源线程原记录 + 新线程重建可接手索引 + 冻结汇总补快照”的方式。
- 今后凡是继续处理项目专用 Skills、Unity MCP 候选、接入、验证闭环、旧桥清退与当前唯一口径，优先从 `Skills和MCP` 线程进入，而不是再回到 `Codex规则落地` 混读。

**遗留问题**:
- [ ] 冻结解除后，如本线程继续推进，应先在新线程下补一轮状态对齐：复核 `refresh_unity` 与 EditMode tests 是否仍稳定，再决定是否继续扩展文档或工作流。

### 会话 2 - 2026-03-19

**用户需求**:
> 先测试现在的 MCP 链接是否正常；然后评估“给一张图片 + prefab 索引/目录，能否用 MCP 和初始化手段自动搭一个高精度基础场景，为后续微调节省工作量”，并给出可行度。

**完成任务**:
1. 读取 `sunset-unity-validation-loop` 与 `sunset-scene-audit`，确认本轮应先做最小只读验证，再给出场景搭建边界判断。
2. 通过 Unity MCP 成功执行三次当前时点取证：
   - `manage_scene(action=get_active)`：活动场景返回 `Primary`
   - `read_console(action=get)`：成功读到最新 Console
   - `manage_scene(action=get_hierarchy)`：成功读到当前根层级
3. 从 Console 读到 `MCP-FOR-UNITY` 在 `8080` 端口停止后重新启动本地 HTTP server 的日志，确认当前链路确实打到了 Unity 侧服务。
4. 基于当前 MCP 能力与 Sunset 的场景修改规则，形成“图驱动搭基础场景”的能力评估与精度边界。

**修改文件**:
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Skills和MCP\memory_0.md`

**关键结论**:
- 当前 Unity MCP **读链正常**；至少场景、Console、层级三类读取都已在本轮成功返回。
- 本轮**没有**复测 `refresh_unity`、Prefab 写入、EditMode tests，因此不能把当前结论夸大成“所有链路都已重新验证”。
- 如果用户提供：图片、prefab 索引/目录、prefab 与画面元素的映射规则、尺度参考（像素 / 单位 / tile / 人物身高）、目标场景路径与允许自动改动边界，我可以用 MCP 先搭出一个**高质量基础场景初稿**，再通过场景回读和截图对比继续收口。

**可行度判断**:
- 2D / 正交 / prefab 对照清晰：`85%`
- UI 布局或平面式摆放：`80%~90%`
- 3D 单张透视图基础还原：`55%~70%`
- 当前给用户的总体承诺值：`75%~85% 可把基础场景初稿搭到明显省时的程度`

**遗留问题**:
- [ ] 真正进入落地前，仍需先做一次“原有配置 / 问题原因 / 建议修改 / 风险影响”的场景审视，不应直接在生产场景盲写。
### 会话 3 - 2026-03-19（按固定 prompt 完成 Skills和MCP 清场回收）

**用户需求**:
> 领取 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_稳定launcher复工前清场\可分发Prompt\Skills和MCP.md`，按其中要求核对 live Git 现场、必要时执行稳定 launcher 的治理同步，并把结果写回固定回收卡。

**完成任务**:
1. 只读核对 `D:\Unity\Unity_learning\Sunset`、`git branch --show-current`、`git rev-parse --short HEAD`、`git status --short --branch`。
2. 确认 pre-sync 现场为 `main @ cfeedf33`，且 dirty 恰好只有 3 个预期文件：
   - `.codex/threads/Sunset/Skills和MCP/memory_0.md`
   - `.kiro/specs/Steering规则区优化/memory.md`
   - `.kiro/specs/Steering规则区优化/当前运行基线与开发规则/memory.md`
3. 执行稳定 launcher：`powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action sync -Mode governance -OwnerThread "Skills和MCP" -IncludePaths ".codex/threads/Sunset/Skills和MCP/memory_0.md;.kiro/specs/Steering规则区优化/memory.md;.kiro/specs/Steering规则区优化/当前运行基线与开发规则/memory.md"`。
4. launcher 成功创建并推送提交：`0b41d4ed`；sync 后现场恢复为 `main + clean`。
5. 已把回收结果写入：`D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_稳定launcher复工前清场\线程回收\Skills和MCP.md`。

**关键结论**:
- 本轮阻断业务 live 准入的根因不是 Unity / MCP、occupancy 或 queue 本身，而是本线程残留的 3 个治理 / 记忆 dirty 未收口。
- 该 blocker 已完成治理同步并推送；shared root 当前回到 `main + clean`。

**恢复点 / 下一步**:
- 本线程当前已完成 prompt 要求的清场回收；后续由治理线程读取固定回收卡并决定下一轮业务线程准入。

### 会话 4 - 2026-03-20（线程身份证与现场快照复核）
**用户需求**：
> 在继续讨论新场景任务前，先完整汇报我当前线程、Kiro 工作区、分支、shared root、Unity/MCP 与队列现场，明确我对自身处境的理解。
**本轮完成**：
1. 只读复核当前 live 身份：`D:\Unity\Unity_learning\Sunset @ main @ 6d85c50e`。
2. 确认本线程记忆文件仍位于：`D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Skills和MCP\memory_0.md`，且本线程当前没有独立 `codex/` 业务分支在用。
3. 确认与本线程直接相关的 Kiro 落点主要有两类：
   - 历史沉淀 / 归档：`Steering规则区优化` 下的 Skills和MCP 冻结文档
   - 当前治理 prompt 挂点：`Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_稳定launcher复工前清场\...\Skills和MCP.md`
4. 复核 shared root / queue / Unity：
   - 占用文档仍写 `main + neutral`
   - queue 当前无 active serving，历史票据已推进到 `next_ticket = 14`
   - Unity 当前活动场景仍为 `Assets/000_Scenes/Primary.unity`
   - 本轮抽样 Console 只读到 MCP 注册日志，未据此宣称全局无错
5. 复核 live Git 后发现：当前 shared root 实际并非 clean，而是挂有 `Codex规则落地` 与 `共享根执行模型与吞吐重构` 的治理 dirty；这说明文档口径与 live Git 现场暂时不同步，后续应以 live Git 为准。
**关键结论**：
- `Skills和MCP` 是一个项目级专题线程，有线程记忆，有历史 Kiro 落点，但当前没有单独挂在某个 `codex/...` 业务分支上。
- 如果后续要让我接管新场景执行，我不能直接在当前 shared root 上开工；必须先等 live Git 收口，再按准入切到独立任务分支。
**恢复点 / 下一步**：
- 本线程已完成身份快照复核；后续继续讨论或执行新场景任务时，优先以 `live Git + live Unity + queue runtime` 为准，而不是只看占用文档的静态描述。

### 会话 44 - 2026-03-23（MCP 根因治理从 scene-build 子问题升级为 main 基线治理）
**用户需求**：不再满足于单次端口纠偏，要求把更深层的“配置 / 文档 / 会话 / 规范漂移”根因直接治理掉，而且这轮应落在 `main` 而不是 `scene-build` 分支。
**当前主线目标**：将 MCP live 事实统一成 shared root `main` 的硬基线，防止 future threads 再拿 `8080` / `旧 MCP 桥口径（已失效）` / 会话旧结论继续误判。
**已完成事项**：
1. 重新划清现场边界：
   - `scene-build` worktree 上的场景改动仍然有效，但这轮 MCP 根因治理必须落到 `D:\Unity\Unity_learning\Sunset @ main`。
2. 只读核查后确认根病不在“休眠”，而在四层漂移：
   - 配置漂移
   - 文档漂移
   - 会话漂移
   - 规范未硬闸门化
3. 点名发现当前仍在误导的 active 残留：
   - `D:\Unity\Unity_learning\Sunset\.codex\drafts\遮挡检查\main-live-validation_2026-03-23.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\001部分回执.md`
   等文件中仍有 `8080` 旧口径。
4. 在 shared root `main` 直接新增 / 修改：
   - `AGENTS.md`
   - `.kiro/locks/mcp-live-baseline.md`
   - `.kiro/locks/mcp-single-instance-occupancy.md`
   - `.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset当前规范快照_2026-03-22.md`
   - `scripts/check-unity-mcp-baseline.ps1`
5. 实测新脚本通过，当前 live 现场输出 `baseline_status: pass`。
**关键决策**：
- 这轮不去直接篡改别的线程旧回执正文，而是先把“以后谁都必须服从的硬基线”立起来。
- 今后 MCP live 判断必须同时满足：`unityMCP + 8888 + pidfile + 当前会话 resources server 正确 + 目标实例正确`。
**恢复点 / 下一步**：
- 立即对白名单把这批治理文件 sync 到 `main`。
- 后续再按治理需要，决定是否给 active drafts / 回执追加“旧口径失效说明”。

### 会话 45 - 2026-03-23（MCP 第二阶段：active 旧口径残留清理）
**用户需求**：继续把“第二阶段清理”直接落地，把当前还在误导人的 active `8080` 残留逐个点名、分批清掉。
**当前主线目标**：继续在 `main` 做 MCP 根因治理，不回到 scene-build 场景施工。
**已完成事项**：
1. 只看 active 层重新排查，确认这轮真正要处理的不是历史归档，而是仍可能被当前线程读成 live 事实的 memory / 回执 / draft。
2. 已处理中和的文件包括：
   - `.kiro/specs/999_全面重构_26.03.15/遮挡检查/memory.md`
   - `.kiro/specs/NPC/memory.md`
   - `.kiro/specs/NPC/1.0.0初步规划/memory.md`
   - `.codex/threads/Sunset/NPC/memory_0.md`
   - `.codex/threads/Sunset/农田交互修复V2/memory_0.md`
   - `.kiro/specs/共享根执行模型与吞吐重构/01_执行批次/2026.03.21_main-only极简并发开发_01/001部分回执.md`
   - `.codex/drafts/遮挡检查/main-live-validation_2026-03-23.md`
3. 已把这些文件中的精确旧地址替换为“旧 MCP 端口口径（已失效）”，旧桥名替换为“旧 MCP 桥口径（已失效）”，并追加统一纠偏说明。
4. 已新增 `scripts/find-legacy-mcp-references.ps1` 作为第二把治理脚本，用来专扫 active `.kiro/.codex` 层的旧口径残留。
**关键决策**：
- 这轮清理不是篡改历史事实，而是阻断“旧事实继续被读成当前事实”。
- 活文档能清的先清；历史归档与旧阶段卷继续保留历史价值，不在本轮强改。
**恢复点 / 下一步**：
- 下一步对白名单做一次 `main` 收口；如果 owner / 热区策略阻断个别文件，就优先把可安全纳入的治理文件与脚本先进 `main`。

## 2026-03-23 MCP 桥口径纠偏
- 本文件中若出现“旧 MCP 桥口径（已失效）”，均表示历史阶段使用过的旧桥结论，不再代表当前 live 入口。
- 当前唯一有效 live 基线以 D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-live-baseline.md 为准：unityMCP + http://127.0.0.1:8888/mcp。
### 会话 46 - 2026-03-23（MCP 第二阶段收尾：active 扫描已 clean）
**用户需求**：继续把第二阶段清理做完，不停在半套治理层。
**当前主线目标**：彻底清掉 active 层中仍会误导线程判断的旧桥 / 旧端口口径。
**已完成事项**：
1. 跑 `scripts/find-legacy-mcp-references.ps1` 后，发现 active 层剩余的是旧 `旧桥` 桥口径，而不再是 `8080`。
2. 已继续处理中和：
   - `.codex/threads/Sunset/Skills和MCP/memory_0.md`
   - `.kiro/specs/共享根执行模型与吞吐重构/memory.md`
   - `.kiro/specs/农田系统/memory.md`
   - `.kiro/specs/农田系统/2026.03.01/10.2.1补丁001/memory.md`
   - `.kiro/specs/农田系统/2026.03.01/10.2.2补丁002/memory.md`
3. 所有这些文件里的 `旧桥` 已统一改写为“旧 MCP 桥口径（已失效）”，并追加统一纠偏说明。
4. 再次运行扫描脚本，结果已变为：`legacy_mcp_reference_status: clean`。
**关键决策**：
- 第二阶段现在不只是“建立了规则”，而是已经把当前 active 层里会误导人的旧字面口径清到了可接受状态。
- 以后再出现旧口径，可以直接由扫描脚本作为治理告警入口。
**恢复点 / 下一步**：
- 下一步只剩这批第二阶段收尾文件同步进 `main`。
### 会话 47 - 2026-03-23（遮挡检查 preflight 风险复核）
**用户目标**：
- 查看“遮挡检查”线程的中间思考，判断当前是否已经出现会导致 `Primary.unity` 误写的危险信号。

**当前主线目标**：
- 继续把 Sunset 的 MCP live 基线压实到 shared root `main`，不要让别的线程因为旧口径 / 漂移口径重新误判。

**本轮子任务 / 阻塞**：
- 只读检查 `遮挡检查` 线程 preflight 里的 MCP 证据链是否成立。
- 这一步服务于主线里的“根因治理”，不是切走去做遮挡业务实现。

**已完成事项**：
1. 读到 `C:\Users\aTo\.codex\config.toml` 当前又漂回旧端口口径（已失效）。
2. 读到 shared root 唯一有效基线文档仍坚持 `unityMCP + http://127.0.0.1:8888/mcp`。
3. 读到 `D:\Unity\Unity_learning\Sunset\Library\MCPForUnity\RunState\mcp_http_8888.pid` 存在，且 `127.0.0.1:8888` 正在监听。
4. 运行 `D:\Unity\Unity_learning\Sunset\scripts\check-unity-mcp-baseline.ps1` 后明确得到：
   - `baseline_status: fail`
   - `issues: unexpected_endpoint`
5. 当前会话 `list_mcp_resources` / `list_mcp_resource_templates` 为空，所以不能把“服务在”直接等同于“这条会话已拿到安全可写的 Unity live 入口”。

**关键决策**：
- 这不是“语气有点危险”，而是确实存在 preflight 失败却准备继续逼近 `Primary.unity` 的风险。
- 根因是三层再次分叉：
  - 配置层：旧端口口径（已失效）
  - 服务层：`8888`
  - 会话层：资源暴露为空
- 在这种状态下，任何面向 `Primary.unity` 的 live 写入都应先停。

**验证结果**：
- 已验证事实：配置与基线不一致；8888 监听正常；基线脚本失败。
- 当前判断：造成遮挡线程阻塞的核心不是业务逻辑本身，而是 MCP preflight 现场没有统一回正。

**恢复点 / 下一步**：
- 先把 `config.toml` 再次纠回 `8888`，再做基线脚本 + 会话资源暴露复核。
- 在复核通过前，主线仍停留在“MCP 根因治理未彻底完成”，不能宣布 `Primary.unity` 可安全进入 live 写态。
### 会话 48 - 2026-03-23（用户称已改 config，但现场仍落在旧端口口径）
**用户目标**：
- 用户说自己已经改完配置，要求我确认现在是否还有别的问题。

**当前主线目标**：
- 继续完成 Sunset MCP 基线回正，不让线程在“自以为改好了”的状态下继续误判。

**本轮子任务 / 阻塞**：
- 只读复核当前 config 与基线脚本结果。

**已完成事项**：
1. 读取 `C:\Users\aTo\.codex\config.toml`，发现其 `url` 仍落在旧端口口径（已失效）。
2. 运行 `D:\Unity\Unity_learning\Sunset\scripts\check-unity-mcp-baseline.ps1`，结果仍为：
   - `baseline_status: fail`
   - `issues: unexpected_endpoint`
3. 读取 terminal script，确认 Unity 侧服务口径依旧是 `8888`。

**关键决策**：
- 这轮不能宣布“已经回正，剩下再看别的问题”。
- 当前仍然是同一个核心问题：配置层没有真正改到与 live 基线一致。

**恢复点 / 下一步**：
- 先把 `config.toml` 真改成 `http://127.0.0.1:8888/mcp`，再让我复跑一次。
### 会话 48 - 2026-03-23（8888 已回正后的再次复核）
**用户目标**：
- 用户手动把 `config.toml` 改回 `8888` 后，确认现在是否还有别的问题。

**当前主线目标**：
- 继续 MCP 根因治理，确认问题是否已经从“配置层错误”收敛到“仅剩会话层 / 现场治理层尾项”。

**本轮子任务 / 阻塞**：
- 只读复核 MCP 基线是否已经转绿。
- 再补看当前这条会话是否已经拿到可见的 MCP 暴露。

**已完成事项**：
1. 读到 `C:\Users\aTo\.codex\config.toml` 已正确写为 `http://127.0.0.1:8888/mcp`。
2. 读到 `127.0.0.1:8888` 正在监听，且 shared root pidfile 仍存在。
3. 运行 `D:\Unity\Unity_learning\Sunset\scripts\check-unity-mcp-baseline.ps1` 已得到 `baseline_status: pass`。
4. 当前会话 `list_mcp_resources` / `list_mcp_resource_templates` 仍为空，因此我只能确认“配置层与服务层已回正”，还不能从这条会话直接证明“会话层 MCP 暴露已回正”。
5. 同时看到 shared root 仍存在其他线程业务 dirty，因此 MCP 回正不等于 shared root 立即变成可自由写入现场。

**关键决策**：
- 现在最大的 MCP 基线问题已经解决。
- 剩余风险不再是“旧端口配置错误”，而是：
  - 会话层是否真的挂上正确 live MCP
  - shared root 是否具备写 `Primary.unity` 的准入条件

**验证结果**：
- 已验证事实：配置正确、8888 监听正常、pidfile 存在、基线脚本通过。
- 当前判断：如果只问 MCP 基线，已经大体恢复；如果问“是否完全无风险”，答案仍然是否。

**恢复点 / 下一步**：
- 真要进 Unity/MCP live 写态，仍建议补一次会话层实际可调用验证。
- 在 shared root 仍有其他 dirty 的情况下，不应因为 MCP 脚本通过就省略热区/占用核查。
### 会话 49 - 2026-03-23（旧线程会话缓存未刷新已写成 live 硬口径）
**用户目标**：
- 不只修一次端口漂移，而是把更深层的“旧线程 / 旧会话仍沿用旧 MCP 路由缓存”根因也真正落地。

**当前主线目标**：
- 继续在 `D:\Unity\Unity_learning\Sunset @ main` 把 MCP live 真相压成不依赖人脑记忆的硬基线。

**本轮子任务 / 阻塞**：
- 只读复核 `main` 现场、Codex 会话日志与 live 规则入口。
- 把“旧会话缓存未刷新”同步进规则、prompt、脚本与 active memory。

**已完成事项**：
1. 在 shared root `main` 只读确认：
   - `C:\Users\aTo\.codex\config.toml` 已是 `http://127.0.0.1:8888/mcp`
   - `D:\Unity\Unity_learning\Sunset\scripts\check-unity-mcp-baseline.ps1` 已返回 `baseline_status: pass`
2. 回读 Codex 会话日志后确认：同一条旧线程在已经读到 `8888` 配置之后，后续请求仍继续打旧端口，说明问题不止是配置层，也包括旧会话 MCP 路由缓存未刷新。
3. 已把这条稳定口径同步到：
   - `AGENTS.md`
   - `Sunset当前规范快照_2026-03-22.md`
   - `mcp-live-baseline.md`
   - 批次 `README.md`
   - `并发线程_当前版本更新前缀.md`
   - `scene-build_当前版本更新前缀.md`
   - `check-unity-mcp-baseline.ps1`
4. 已把当前 active memory 中仍会误导人的旧端口字面改写成“旧端口口径（已失效）”，并重新执行 `scripts/find-legacy-mcp-references.ps1` 做复核。

**关键决策**：
- 以后只要 `config + 8888 + pidfile + baseline` 已通过，但旧线程仍报旧端口 / 旧桥名 / `resources/templates` 为空，优先判为“旧会话 MCP 路由缓存未刷新”，不要直接宣布 shared root 服务端回滚。
- 真要继续 live 验证，优先开新线程 / 新会话，或先手工对 `8888` 做 initialize + `tools/list`，先证明服务端可用。

**验证结果**：
- 当前 stable 结论已经从“只知道 8888 才是对的”推进到“能区分配置层错误 vs 旧会话缓存未刷新”。

**恢复点 / 下一步**：
- 下一步只剩按白名单把这轮治理文档 / 脚本 / 记忆同步进 `main`。
