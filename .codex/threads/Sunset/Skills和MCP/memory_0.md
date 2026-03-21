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
- 已确认 `农田交互修复V2` 线程里“用不了 MCP”的根因是旧 `mcp-unity` 误用，而不是 Unity Server 没起。
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

### 会话 5 - 2026-03-20（主线迁移至场景搭建）

**用户需求**：
> 用户正式指定当前线程后续主线迁移为“场景搭建”，固定父工作区为 `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建`、当前子工作区为 `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划`；要求保留线程名称 `Skills和MCP` 不变，直接建立后续开发可遵守的文档基石、任务清单，并先把资产普查、场景骨架、Tilemap 分层和执行顺序落地。

**完成任务**：
1. 按 `skills-governor + sunset-workspace-router + sunset-scene-audit` 做手工等价启动闸门，确认本轮命中新的 900 开篇场景搭建工作区。
2. 只读复核当前现场：`D:\Unity\Unity_learning\Sunset @ main @ bdb8cb4c`，写文档前工作树干净；同时确认项目已具备 `Scenes / Prefabs / Tilemap / Farm 脚本` 等基础资产入口。
3. 新建父工作区 `5.0.0场景搭建` 的 `requirements.md`、`design.md`、`tasks.md`、`memory.md`，把长期主线目标、场景骨架、Tilemap 分层原则、总执行模型与长期任务路线固定下来。
4. 新建子工作区 `1.0.1初步规划` 的 `requirements.md`、`design.md`、`tasks.md`、`资产普查.md`、`memory.md`，把当前阶段的目标、已验证事实、首轮资产普查结果与下一阶段准入条件落盘。
5. 明确线程主线迁移结论：从现在起，`Skills和MCP` 线程名保留不变，但主目标不再是 Skills/MCP 专题本身，而是“独立场景搭建”；Skills/MCP 退回支撑层。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\requirements.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\design.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\requirements.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\design.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\资产普查.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Skills和MCP\memory_0.md`

**关键结论**：
- 当前线程不改名、不换线程目录，但主线正式迁移为场景搭建。
- 当前阶段不是立即搭 scene，而是把后续施工的规则、骨架、分层和任务顺序定稳。
- 已验证当前项目具备真实场景搭建基础：存在 `Assets/000_Scenes`、`Assets/222_Prefabs`、`Assets/223_Prefabs_TIlemaps`、`Assets/Sprites`、`Assets/111_Data`、`Assets/YYY_Scripts`、`Assets/ZZZ_999_Package` 等关键入口；并已读到 `387` 个 prefab、`3756` 个 png、`4717` 个 asset 的粗量级规模。

**恢复点 / 下一步**：
- 下一步优先收口“新 scene 的命名、路径与最小创建方案”。
- 进入真实 `Assets/` 施工前，仍需按 live Git / scene 审视规则重新做准入确认。

### 会话 6 - 2026-03-20（父子工作区结构修正）

**用户需求**：
> 用户明确纠正：`5.0.0场景搭建` 是父工作区，不应该在根目录堆 `requirements/design/tasks`；当前真正的正文承载区是 `1.0.1初步规划`，要求立即修正。

**完成任务**：
1. 复核 `workspace-memory.md` 与用户当前口径后，确认我此前把“父工作区”机械理解成“当前也需要完整三件套”的做法偏重了规则模板，轻了用户这条主线的实际组织方式。
2. 把父层三件套中的有效内容整合下沉到 `1.0.1初步规划`。
3. 删除父层误建的 `requirements.md`、`design.md`、`tasks.md`。
4. 保留父层 `memory.md` 作为父工作区承接层，并在父子 memory 中写清这次结构纠偏。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\requirements.md` - [删除]
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\design.md` - [删除]
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\tasks.md` - [删除]
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\memory.md` - [追加]：写入父层职责收窄结论。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\requirements.md` - [修改]
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\design.md` - [修改]
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md` - [修改]
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md` - [追加]：写入子层正文承载结论。

**关键结论**：
- 当前线程后续继续沿“场景搭建”主线推进不变。
- 但文档结构从现在起改为：父层只保留 `memory.md`，子层 `1.0.1初步规划` 才是当前正文承载区。

**恢复点 / 下一步**：
- 下一步仍然回到 `1.0.1初步规划`，继续做新 scene 命名、路径与最小创建方案。

### 会话 7 - 2026-03-20（新 scene 命名与最小创建方案）

**用户需求**：
> 请你开始。

**完成任务**：
1. 按 `skills-governor + sunset-workspace-router + sunset-scene-audit` 做只读前置核查，继续在 `1.0.1初步规划` 内推进。
2. 只读确认当前 scene 命名现场：
   - `Assets/000_Scenes` 为扁平目录
   - Build Settings 当前只有 `Primary.unity`
   - `DoorTrigger.cs` 存在按字符串加载 scene 的入口
3. 在子工作区正文中正式收口推荐方案：
   - 新 scene 路径：`Assets/000_Scenes/SceneBuild_01.unity`
   - scene 名：`SceneBuild_01`
   - 初建时不加入 Build Settings
   - 使用 Empty Scene + `SceneRoot / Systems / Tilemaps / PrefabSetDress / GameplayAnchors / LightingFX / DebugPreview` 起步
4. 更新子工作区任务状态，将“确认新 scene 命名、路径与最小创建方案”标记为完成。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\design.md` - [修改]
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md` - [修改]
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md` - [追加]
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\memory.md` - [追加]
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Skills和MCP\memory_0.md` - [追加]

**关键结论**：
- 现在“新 scene 该叫什么、放哪、怎么最小起步”已经收口，不需要再反复讨论。
- 下一个真正的阻塞不再是命名，而是“首版 prefab 候选池整理到什么粒度才足够开工”。

**恢复点 / 下一步**：
- 下一步继续在 `1.0.1初步规划` 内处理 prefab 候选池粒度与首版施工入口。

### 会话 8 - 2026-03-20（场景搭建线已迁入专属 branch / worktree）

**用户需求**：
> 用户要求把当前这条重度 MCP 场景搭建线从 shared root `main` 剥离，给它一个独立 branch，让它继续写文档和后续工作。

**完成任务**：
1. 已将当前线程 WIP 从 shared root 转移到：
   - branch：`codex/scene-build-5.0.0-001`
   - worktree：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
2. 当前已有的线程记忆与 `5.0.0场景搭建 / 1.0.1初步规划` 文档 WIP 已保留在该 worktree 中。

**关键结论**：
- 当前线程名仍是 `Skills和MCP`，但其真实工作面已经不再是 shared root 的 `main`。
- 这一步的目的不是立刻开 Unity，而是先把长寿命 docs / 规划 WIP 从 shared root 清出去，给其他业务波次让路。
- 当前仍应保持 docs / 规划优先，不直接进入 Unity / MCP 高频写链路。

**恢复点 / 下一步**：
- 当前线程默认从：
  - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
  - `codex/scene-build-5.0.0-001`
  继续。

### 会话 9 - 2026-03-20（专属 worktree 现场复核与 prefab 粒度收口）

**用户需求**：
> 用户明确通知本线程已从 shared root 剥离，后续默认在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001 @ codex/scene-build-5.0.0-001` 继续；本轮只做现场复核、prefab 候选池粒度整理、文档/资产普查/scene 规划，禁止回 shared root、禁止创建真实 scene、禁止进入 Unity / MCP 高频施工。

**完成任务**：
1. 按 `skills-governor + sunset-workspace-router + sunset-scene-audit` 做手工等价前置核查，并在新 worktree 内只读确认：
   - `cwd`：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `branch`：`codex/scene-build-5.0.0-001`
   - 初始 `git status --short` 含：
     - `M .codex/threads/Sunset/Skills和MCP/memory_0.md`
     - `?? .kiro/specs/900_开篇/5.0.0场景搭建/`
2. 回读 worktree 内已有正文，确认当前 scene 规划结论仍成立：
   - 新 scene 仍推荐 `Assets/000_Scenes/SceneBuild_01.unity`
   - Tilemap 仍为 8 层结构
3. 继续做只读资产普查，把首版 prefab 候选池整理到足够开工的粒度：
   - `A 全收录`：`House / Tree / Rock / House_Tilemap`
   - `B 按桶收录`：`Farm -> Food / T`
   - `C 按需补入`：`Dungeon props / UI / WorldItems / NPC / UI_Tilemap`
4. 更新当前 worktree 内的子工作区正文：
   - `design.md`
   - `资产普查.md`
   - `tasks.md`

**关键结论**：
- 当前线程虽然仍叫 `Skills和MCP`，但在执行层面已经稳定切到“场景搭建 worktree 主线”。
- 当前 docs/WIP 现场没有漂移，且 prefab 候选池粒度已经足够支撑下一步 create-only 级别的真实骨架施工准备。
- 本轮没有进入 Unity / MCP 高频施工，没有创建真实 scene，也没有碰 `Primary.unity`。

**恢复点 / 下一步**：
- 下一步继续在当前 worktree 内做 `SceneBuild_01` 的 create-only 准入复核；准入通过后，再开始最小骨架施工。

### 会话 10 - 2026-03-21（场景搭建主线全量状态盘点）

**用户需求**：
> 回到之前进度，并完整说明当前哪些内容已完成、哪些做到一半、哪些还没完成。

**完成任务**：
1. 在专属 worktree 内只读复核当前身份：
   - `cwd = D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `branch = codex/scene-build-5.0.0-001`
   - `HEAD = f1507f59`
2. 回读当前正文工作区、shared-root 迁入目录、`memory_1.md` 与当前线程记忆，确认主线没有丢内容，但存在“主正文 / 迁入快照 / 线程续卷”三层并行承载。
3. 给出线程级收口判断：
   - **已完成**：主线迁移、父子结构修正、scene 命名、Tilemap 分层、资产普查、prefab 候选池粒度、worktree 剥离。
   - **半完成**：迁入快照已经吸收，但还没做最终归档清理；`memory_1.md` 已落入当前 worktree，但尚未决定后续是继续滚动还是仅作为迁入续卷保留。
   - **未完成**：create-only 准入、真实 scene 骨架、后续底稿/结构/装饰/逻辑/MCP 回读/交付链。

**关键结论**：
- 这条线程目前已经不缺“规划”，缺的是“从规划切到真实 scene 写态”的那一步。
- 因此当前真正的主线阻塞点已经非常明确：不是继续补文档，而是后续写态准入与施工开始。

**恢复点 / 下一步**：
- 下一步继续在当前专属 worktree 内推进 `SceneBuild_01` 的 create-only 准入复核与最小骨架开工。

### 会话 11 - 2026-03-21（施工前准备清单已重排）

**用户需求**：
> 用户明确要求：当前可以继续收口施工前准备，但先别正式开工写场景；要先把未完成项按执行顺序整理出来，并把 imported 快照、`memory_1.md`、create-only 准入复核整理成我自己的开工前清单。

**完成任务**：
1. 回读当前正文、迁入快照与线程续卷后，确认当前线程已经不缺规划蓝图，真正缺的是施工前尾项收口与写态裁定。
2. 已在子工作区 `tasks.md` 中新增：
   - “当前按执行顺序排好的未完成清单”
   - “开工前三个尾项”
3. 明确当前线程进入下一步前的顺序应为：
   - imported 快照角色收口
   - `memory_1.md` 角色收口
   - create-only 准入口径复核
   - 等待你裁定是否成为唯一 Unity / MCP 写线程
   - 然后才进入真实 scene 施工链

**关键结论**：
- 当前线程已经从“施工规划阶段”切换到“施工前尾项整理阶段”。
- 在你下达下一次写态准入前，我会继续停留在 docs / 规划 / 普查层，不进入 Unity / MCP 高频施工。

**恢复点 / 下一步**：
- 下一步按新任务顺序继续处理开工前三个尾项。

### 会话 12 - 2026-03-21（施工前三个尾项已全部完成）

**用户需求**：
> 用户明确要求我先完成 imported 快照收口、`memory_1.md` 角色收口、create-only 准入复核；完成前不要进入 Unity / MCP / create-only 写 scene。

**完成任务**：
1. 已将 imported 目录正式定性为“迁入证据快照层”，不再与主正文并行滚动。
2. 已将 `memory_1.md` 正式定性为“迁入续卷 / 历史快照卷”，后续线程活跃记忆继续写 `memory_0.md`。
3. 已按当前 `codex/scene-build-5.0.0-001 @ f1507f59` 现场重做 create-only 准入复核：
   - 本地 `shared-root-branch-occupancy.md` 仍显示 shared root `main + neutral`
   - 本地 `mcp-single-instance-occupancy.md` 仍显示 `current_claim = none`
   - 本地 `.kiro/locks/active/` 当前只有 `.gitkeep`
   - 当前未见 Unity / MCP 占用冲突证据
4. 已将这三项结果写回子工作区 `tasks.md` 与相关 memory。

**关键结论**：
- 施工前尾项现在已经收干净。
- 当前真正剩下的唯一前置，是等待你正式裁定我成为下一位 Unity / MCP 写线程。

**恢复点 / 下一步**：
- 一旦你放行，我就直接进入 `SceneBuild_01 -> Grid + Tilemaps` 的最小施工准备，而不再回头补规划。

### 会话 13 - 2026-03-21（写线程已放行，但 Unity 目标根目录错误）

**用户需求**：
> 用户正式放行，允许进入 Unity / MCP / create-only 写态，并要求首个 checkpoint 只做 `SceneBuild_01 -> Grid + Tilemaps`。

**完成任务**：
1. 完成进入前复核：
   - 不在 Play Mode
   - 不在 Compile / Domain Reload 中间态
   - Console 仅见 1 条 `UnityEditor.Graphs` 内部空引用噪音
2. 读取 `project_info` 后识别到关键事实：
   - 当前 Unity / MCP 连接的项目根是 `D:/Unity/Unity_learning/Sunset`
   - 并非当前 worktree `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
3. 在发现这一点前，`manage_scene create/save` 已误在 shared root 写出：
   - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\SceneBuild_01.unity`
   - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\SceneBuild_01.unity\SceneBuild_01.unity`
4. 发现后已立即停止，没有继续写 `Grid + Tilemaps`。

**关键结论**：
- 当前真正 blocker 已从“等用户放行”切换成“Unity 工具链仍连 shared root，不能继续在错误项目根上写”。
- 这次 checkpoint 只完成了进入前复核，没有完成合法的 `SceneBuild_01 -> Grid + Tilemaps` 施工。

**恢复点 / 下一步**：
- 下一步先处理 Unity 根目录错连与 shared root 误写资产，再恢复合法施工。

### 会话 14 - 2026-03-21（Unity / MCP 已重新绑定到场景搭建 worktree）

**用户需求**：
> 用户要求我先不要继续 scene 施工，而是先把 Unity / MCP 指到 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`，并只回报 `project_root`、Play Mode 状态、Compile / Domain Reload 状态。

**完成任务**：
1. 按 `skills-governor + sunset-unity-validation-loop` 做只读阻塞处理，先验证 `unityMCP` 资源是否重新出现。
2. 读到唯一在线实例：`scene-build-5.0.0-001@b4abdcc2b4706d2c`。
3. 通过 `set_active_instance` 将当前客户端会话切到该实例。
4. 成功读回：
   - `projectRoot = D:/Unity/Unity_learning/Sunset_worktrees/scene-build-5.0.0-001`
   - `is_playing = false`
   - `is_compiling = false`
   - `is_domain_reload_pending = false`
5. 额外抽样 Console，当前未见项目级红编译；仅看到 warning 与配置缺失提示。

**关键结论**：
- 本线程当前服务的主线仍然是“场景搭建”，本轮只是清理一个工具阻塞：把 Unity / MCP 从 shared root 纠回到专属 worktree。
- 阻塞已解除，但我遵守用户口径，本轮先停在状态确认，不继续执行 `SceneBuild_01 -> Grid + Tilemaps` 写态。

**恢复点 / 下一步**：
- 下一步在用户确认后，直接恢复 `SceneBuild_01 -> Grid + Tilemaps` 的最小施工窗口。

### 会话 15 - 2026-03-21（首个 worktree checkpoint 收口完成）

**用户需求**：
> 继续当前主线，并把 `SceneBuild_01 -> Grid + Tilemaps` 的首个 checkpoint 收口完成。

**当前主线目标**：
- 场景搭建主线继续绑定在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`，线程名仍为 `Skills和MCP`，但线程实际职责已稳定迁移为“场景搭建”。

**本轮子任务 / 阻塞**：
- 本轮不是扩施工面，而是把首个 create-only checkpoint 做成可交接状态：证据回读、临时脚本清理、文档同步。

**完成任务**：
1. 只读复核当前现场：
   - `cwd = D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `branch = codex/scene-build-5.0.0-001`
   - `HEAD = f1507f59fa9142a900f6451b43623d14f154bf0c`
2. 回读 checkpoint 证据：
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.codex\drafts\scene-build-5.0.0-001\scene_build_01_checkpoint_result.json`
   - 其中 `success = true`
3. 回读场景 YAML，确认 `SceneBuild_01.unity` 已包含：
   - 根层级：`SceneRoot / Systems / Tilemaps / PrefabSetDress / GameplayAnchors / LightingFX / DebugPreview`
   - `Grid`
   - 8 个 Tilemap
   - `MainCamera`
4. 删除临时施工脚本与 `.meta`：
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\Editor\CodexSceneBuild01Checkpoint.cs`
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\Editor\CodexSceneBuild01Checkpoint.cs.meta`
5. 只读复核 shared root 历史残留：
   - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\SceneBuild_01.unity` 仍存在
   - 嵌套错位路径已不存在

**关键结论**：
- 首个 `SceneBuild_01 -> Grid + Tilemaps` checkpoint 已经真正完成，且交付物已从“临时施工脚本”收口为“场景资产 + 证据回读”。
- shared root 的同名 scene 仍需单独裁定；这属于历史残留治理，不属于当前 worktree checkpoint 的继续施工。

**恢复点 / 下一步**：
- 当前恢复点不再是“继续建 Grid + Tilemaps”，而是“等待用户裁定是否扩到地图底稿阶段”。

### ?? 16 - 2026-03-21????? v1 ?? scene-build worktree ???
**????**?
> ? `codex/scene-build-5.0.0-001` ????????????shared root ????????? `Grid + Tilemaps` checkpoint ???????????
**??????**?
- `Skills?MCP` ??? Sunset ?????????????????????? `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001` ??? `SceneBuild_01`??????????????????????
**????? / ??**?
- ??????????????? `SceneBuild_01.unity`????????????? Unity MCP ?????????? `CodexSceneBuild01MapDraft.cs` ?????? Unity ??????
**????**?
1. ??? `skills-governor + sunset-workspace-router + sunset-scene-audit + sunset-unity-validation-loop` ??????????? worktree / branch / HEAD ????
2. ?? MCP ?????`unityMCP` ???????? `Sub2API` HTML?`mcp_unity` ??? `Connection failed: Unknown error`?
3. ???? `TM_Ground_Base / TM_Ground_Detail / TM_Path_Water` ??? YAML ?????? `SceneBuild_01.unity` ??????? v1?
4. ???? tile ??? bounds?`Ground_Base=159`?`Ground_Detail=107`?`Path_Water=21`?
5. ???????????? `Assets/Editor/CodexSceneBuild01MapDraft.cs` / `.meta`???? gitignored ?????
**????**?
- ??????????????????????????? + ???? v1????? checkpoint?
- ???????? Unity live ????? YAML ??????????????????????Editor ??????????????????
**??? / ???**?
- ????????????????? worktree ??? `???` ???????????????????????MCP ???????? / Unity live ??????

### 会话 17 - 2026-03-21（结构层最小版本完成，主线恢复到装饰层）

**用户需求**：
> 在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001` 内继续推进 `结构层`，做完后按“最小 checkpoint + clean + 回执”回来。

**当前主线目标**：
- 线程职责已从早期的 Skills / MCP 治理线稳定迁移为“场景搭建”，当前主线是继续完成 `SceneBuild_01` 的场景初稿。

**本轮子任务 / 阻塞**：
- 子任务：完成结构层最小版本。
- 阻塞：`unityMCP` 仍返回 `Sub2API` HTML，无法做稳定 Unity live 写入与验证，所以改用 Scene YAML 兜底。

**完成任务**：
1. 在 worktree 现场确认：
   - `cwd = D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `branch = codex/scene-build-5.0.0-001`
   - `HEAD = 7b92abe0`
2. 完成结构层最小写入：
   - `Structure_Farmstead`
   - `Structure_House_Main`
   - `Fence_North_01 / Fence_North_02 / Fence_South_01 / Fence_South_02 / Fence_East_Lower / Fence_East_Upper`
3. 文件级回读确认：
   - 新对象全部挂在 `SceneRoot/PrefabSetDress`
   - `fileID` 唯一
   - 父子引用完整
4. 本轮没有触碰 shared root 残留，也没有宣称 Unity live 验收通过。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity`
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md`
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md`
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\memory.md`
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.codex\threads\Sunset\Skills和MCP\memory_0.md`

**关键决策**：
- 当前结构层不再强绑 `TM_Structure_Back / TM_Structure_Front`，而是优先使用 `PrefabSetDress` 承载主建筑与围栏院落，理由是：MCP 不稳时这条 YAML 路线更可控，也更容易在后续装饰层继续扩写。

**恢复点 / 下一步**：
- 下一步直接进入装饰层最小版本。
- 等装饰层和逻辑层完成后，再统一补 Unity live / Console / MCP 验证闭环。
