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