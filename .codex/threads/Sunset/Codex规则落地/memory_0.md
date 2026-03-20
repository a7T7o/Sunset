# Codex规则落地 - 线程记忆

> 历史长卷保留在 [memory_1.md](/D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/Codex规则落地/memory_1.md)。本卷只保留当前活跃治理主线、稳定结论与恢复点。

## 线程概述
- 线程名称：Codex规则落地
- 线程分组：Sunset
- 线程定位：负责 Sunset 中与 Codex 治理相关的全局规则、入口、skills、AGENTS、线程执行纪律、共享根目录占用语义和事故收口；不代替业务线程写业务代码。

## 当前主线目标
- 把 Sunset 的治理体系从“规则很多但经常被跳过”，推进到“进入实质性任务前会被强制触发正确入口与现场核查”。
- 当前 live 主线已切换到：
  - `21_第一次唤醒复盘与shared-root分支租约闸门`
- 当前重点是修掉第一次唤醒暴露出的“shared root 提前切分支”缺口，并把 superpower 的认知钢印真正打进本地入口。

## 当前状态
- **用户视角完成度（恢复正常开发）**：主入口在 `main`，但第一次唤醒协议仍需先修正
- **治理推进度（当前子主线 21）**：进行中
- **最后更新**：2026-03-18
- **状态**：阶段 21 已接管新增缺口

## 当前稳定结论
- Sunset 当前治理续办总入口固定为：
  - [D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地)
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex` 已降级为 TD-only 镜像区，不再承载治理正文。
- [11_main-branch-only回归与worktree退役收口](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/tasks.md) 与 [12_治理工作区归位与彻底清盘](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/12_治理工作区归位与彻底清盘/tasks.md) 仍保留历史完成记录，但不再代表当前 live 现场。
- 当前 live 事实已更新为：
  - `D:\Unity\Unity_learning\Sunset @ main`
  - `shared-root-branch-occupancy.md` 当前为 `main + neutral`
  - `git worktree list --porcelain` 只剩 shared root
- `obra/superpowers` 已完成复审并维持 `rejected-as-is`。
- 为了让强制技能命中更贴近原生环境，已把下列本地治理技能暴露到 `C:\Users\aTo\.agents\skills\`：
  - `skill-vetter`
  - `skills-governor`
  - `sunset-startup-guard`
- 当前新暴露的治理缺口是：
  - `ensure-branch` 还没有 shared root 独占租约闸门
  - 第一次唤醒与第二次准入 prompt 还没有正式拆开
- 当前三条业务线的合法 carrier 历史判断仍可作为参考；shared root 常态入口已经恢复，但真实写入仍必须先过 preflight、再按分支闸机进入。
- `worktree` 仍然是 Sunset 红线例外；本轮新增的回正工作不是重新常态化它，而是把 live 现场重新拉回 branch-only 模型。
- Unity / MCP 单实例层三件套已落盘：
  - `mcp-single-instance-occupancy.md`
  - `mcp-hot-zones.md`
  - `mcp-single-instance-log.md`
- `D:\Unity\Unity_learning\Sunset_external_archives` 与 `D:\Unity\Unity_learning\Sunset_backups` 的历史退役结论仍然有效。
- 当前唯一剩余物理尾巴是：
  - `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
  - 已无 Git 身份，只剩被外部进程占用的空目录残壳

## 最近会话

### 会话 1 - 2026-03-17（11 阶段执行面已落盘并开始真实退役）
**用户目标**：
> 不接受“已经立项”式半成品，而是要求把 `11` 做成真正可执行的 branch-only 收口面，并且现场要开始真实清理，不只是继续写文档。

**已完成事项**：
1. 重写 `11` 阶段核心文档：
   - `analysis.md`
   - `tasks.md`
   - `执行方案.md`
   - `memory.md`
2. 新增：
   - [总进度与收口清单_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/总进度与收口清单_2026-03-17.md)
   - [分发Prompt_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/分发Prompt_2026-03-17.md)
3. 重写现行入口文档：
   - [Sunset当前唯一状态说明_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset当前唯一状态说明_2026-03-17.md)
   - [Sunset现行入口总索引_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset现行入口总索引_2026-03-17.md)
4. 已真实执行第一批纯历史 worktree 退役，`git worktree list` 已缩减。
5. 已新增：
   - [所有线程回归誓言.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/所有线程回归誓言.md)（线程回包索引）
   - [共享根目录dirty归属初版_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/共享根目录dirty归属初版_2026-03-17.md)

**关键决策**：
- `09/10` 不再继续承接新增收尾尾巴；`11` 是当前最终收口主线。
- 分支是长期 carrier，worktree 只是事故容器。
- `导航检查 / 遮挡检查 / 项目文档总览` 当前不需要 worktree。
- 用户视角完成度仍按 `0%` 计，直到 shared root 回 `main` 且 `git worktree list` 被清到只剩共享根目录。

**恢复点 / 下一步**：
- 立即向 `spring-day1 / NPC / farm` 分发 branch-only 回归 prompt。
- 收到三方答复后，继续：
  1. 共享根目录归属表
  2. shared root 回 `main`
  3. 第二批 worktree 退役

### 会话 2 - 2026-03-17（纠正“所有线程回归誓言”读取对象并锁定真实阻塞）
**用户目标**：
> `所有线程回归誓言` 文件夹里放的是线程真实回包，而不是统一誓言正文；要求我基于真实回包继续推进，不要再围着占位索引文件分析。

**已完成事项**：
1. 纠正 `11` 阶段里的读取对象误判：
   - `所有线程回归誓言.md` 是索引/提炼位
   - `所有线程回归誓言\*.md` 才是真实线程回包
2. 读取并提炼：
   - `spring-day1`
   - `NPC`
   - `农田交互修复V2`
   - `导航检查`
   - `遮挡检查`
   - `项目文档总览`
   六份回包
3. 锁定当前真实阻塞已缩成：
   - shared root 与 `NPC_roam_phase2_rescue` 的 `spring-day1` 字体 dirty 双现场分裂
   - shared root 治理/归档 dirty 尚未形成可执行收口
   - `Assets/111_Data/NPC 1.meta`、`Assets/Screenshots*`、`npc_restore.zip` 尚未裁定
4. 更新或新增：
   - [所有线程回归誓言.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/所有线程回归誓言.md)
   - [共享根目录dirty归属可执行版_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/共享根目录dirty归属可执行版_2026-03-17.md)
   - [第二批worktree核验表_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/第二批worktree核验表_2026-03-17.md)
   - [分发Prompt_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/分发Prompt_2026-03-17.md)

**关键决策**：
- 现在不再缺三大业务线的 branch-only carrier 判断。
- 当前二轮 prompt 只需要发给 `spring-day1`，专门裁定字体双现场差异。
- `NPC` 与 `farm` 后续只需等待 shared root 回 `main` 后，在根目录做一次 clean checkout 验证。

**恢复点 / 下一步**：
- 先发 `spring-day1` 二轮 prompt。
- 然后继续治理 shared root 自身 dirty，准备最终回 `main`。

### 会话 3 - 2026-03-17（`spring-day1` 字体双现场已实清）
**用户目标**：
> 审核 `spring-day1` 的二轮回复并继续推进，不停在“回复已收到”。

**已完成事项**：
1. 复核二轮回复成立，并确认 5 个字体资产继续归 `spring-day1`。
2. 导出 shared root / rescue 两处字体 dirty 的证据补丁与 diffstat。
3. 已实际清空：
   - shared root 的 `BitmapSong / Pixel / SoftPixel / V2` dirty
   - rescue 的 `BitmapSong / Pixel / SDF / V2` dirty
4. `NPC_roam_phase2_rescue` 已恢复为 `CLEAN`。
5. 已删除误复制残留 `Assets/111_Data/NPC 1.meta`。

**关键决策**：
- 当前不再需要继续围绕 `spring-day1` 发 prompt。
- 当前剩余阻塞已收缩成：
  - shared root 自身治理/归档 dirty
  - `Assets/Screenshots*`
  - `npc_restore.zip`

**恢复点 / 下一步**：
- 继续 shared root 自身收口。
- 裁定截图与 zip。
- 然后推进 shared root 回 `main` 与第二批 worktree 退役。

## 当前下一步
1. 把 `第一次唤醒/` 目录纳入 Git 证据。
2. 新建并推进 `21`：
   - shared root 分支租约闸门
   - 双阶段唤醒协议
   - `AGENTS.md` / `openai.yaml` 认知钢印补强
3. 完成受控回归后，再重发线程唤醒。

### 会话 4 - 2026-03-17（shared root 已回 `main`，线程同步改写为终局口径）
**用户目标**：
> 在 `11` 的真实 Git/现场动作已经做完后，把当前线程记忆、父级 memory 和现行状态入口一起改成终局口径，不要再保留“shared root 仍是事故现场”的说法。

**已完成事项**：
1. 复核 shared root 终局状态：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `git worktree list --porcelain` 只剩共享根目录
2. 确认 `spring-day1 / NPC / farm` 的根目录 branch-only 检出验证都已真实完成。
3. 同步重写：
   - `11` 阶段核心文档
   - 父级 `000_代办/codex/memory.md`
   - 当前线程 `memory_0.md`
   - 现行状态说明与现行入口总索引
4. 为 `11` 新增终局快照：
   - [终局快照_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/终局快照_2026-03-17.md)

**关键决策**：
- 本线程关于 `11` 的子主线已经完成，不再继续把新的 shared root 尾巴往里塞。
- 如果用户接下来继续强调 skills、AGENTS、四件套或强制闸门，这些都应重新落到 `09` 或新的治理阶段，而不是回头再扩写 `11`。

**恢复点 / 下一步**：
- `11` 已封板。
- 当前线程若继续治理，应回到“让规则被稳定触发与遵守”的主命题，而不是继续处理已经收口的 worktree 事故。

### 会话 5 - 2026-03-17（核验 11 完成事实并补齐 09 工作区记忆）
**用户目标**：基于上一轮终局摘要，确认 Sunset 现场是否真的已经回到 `main + branch-only`，并明确当前线程接下来该回到哪条治理主线。  
**完成事项**：只读复核 shared root 当前为 `D:\Unity\Unity_learning\Sunset @ main`、`git worktree list --porcelain` 仅剩根目录、`git status --short --branch` clean、最新提交为 `dd83ff74`；同时发现 `09_强制skills闸门与执行规范重构` 目录缺少 `memory.md`，本轮已补建并同步父层记忆。  
**关键决策**：把 `09` 明确恢复为 `11` 封板后的唯一治理续办点；`11` 继续只保留终局快照与过程证据，不再承接新尾巴。  
**涉及文件**：`C:\Users\aTo\.codex\skills\sunset-startup-guard\SKILL.md`、[09_强制skills闸门与执行规范重构/memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/09_强制skills闸门与执行规范重构/memory.md)、[memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory.md)、[memory_0.md](/D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/Codex规则落地/memory_0.md)。  
**恢复点 / 下一步**：后续若继续推进本线程，优先进入 `09` 去压实 `sunset-startup-guard` 的稳定命中、最小回复结构和 `worktree` 红线执行。

### 会话 6 - 2026-03-17（总进度盘点与未完项重新分层）
**用户目标**：要一份能够直接回答“现在项目到底到哪一步、还剩什么、哪些是真的没做完”的总盘点。  
**完成事项**：通读 `000_代办/codex/01-11` 的 `tasks.md`，并结合当前 Git 现场重新分层：`11` 已真实完成，`04/05/06/08` 也已达到阶段完成；`03/10` 有旧勾选未收口，但多数已被 `11` 的终局动作覆盖；当前真正还活跃的治理主线只剩 `09`，此外还有 `01/02/07` 的少量收尾债。  
**关键决策**：业务线程警戒线已经解除，不需要再分发 branch-only prompt；当前剩余的“警戒线”主要是治理层的，即 skills 强制命中、启动闸门稳定执行、旧待办显式封板这几项。  
**涉及文件**：[09_强制skills闸门与执行规范重构/tasks.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/09_强制skills闸门与执行规范重构/tasks.md)、[memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/09_强制skills闸门与执行规范重构/memory.md)、[memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory.md)。  
**恢复点 / 下一步**：从 `09` 继续做强制 skills 闸门落地；同时择机清掉 `03/10` 的文档残余未勾选项，防止以后再被误读成系统尚未收口。

### 会话 7 - 2026-03-17（12 阶段真实清盘与 MCP 单实例层并入执行面）
**用户目标**：在不漂移主线的前提下，把“代办区误当工作区”的结构错位、仓库外历史容器退役，以及 Unity / MCP 单实例冲突一起补成真实可执行口径。  
**已完成事项**：
1. 新建：
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-occupancy.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-hot-zones.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-log.md`
2. 更新：
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - 当前运行基线与开发规则下的 live 入口文档
3. 新建 [外部历史容器退役说明_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/12_治理工作区归位与彻底清盘/外部历史容器退役说明_2026-03-17.md)。
4. 真实删除：
   - `D:\Unity\Unity_learning\Sunset_external_archives`
   - `D:\Unity\Unity_learning\Sunset_backups`
5. 实测当前 shared root `HEAD = 952a1f23`。
**关键决策**：
- shared root 中性和 Unity / MCP 单实例安全必须分层判断。
- 后续只要继续治理，统一回到 `09`，不再重开 “worktree 退役” 主线。

### 会话 8 - 2026-03-17（治理收官复核与两条补强规则正式落盘）
**用户目标**：
> 不要只停在“worktree 退役已经完成”的描述上，还要继续把 shared root 当前真实状态、Play Mode 离场纪律和表现层审美要求一起补成强规则。

**已完成事项**：
1. 复核当前仓库现场：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - 最新治理提交：`663af03c`
   - `main...origin/main` 无 ahead / behind
   - shared root 当前仍带其他线程 dirty，主体是 NPC 业务文件并夹杂少量非治理记忆，因此实时上不是中性现场
2. 更新 repo 内规则与 live 文档：
   - `AGENTS.md`
   - `shared-root-branch-occupancy.md`
   - `mcp-single-instance-occupancy.md`
   - `mcp-hot-zones.md`
   - `基础规则与执行口径.md`
   - `Sunset强制技能闸门与线程回复规范_2026-03-17.md`
   - `Sunset当前唯一状态说明_2026-03-17.md`
   - `ui.md`
3. 更新本机 skill：
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\SKILL.md`
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\references\checklist.md`
4. 回写：
   - `09_强制skills闸门与执行规范重构/tasks.md`
   - `09_强制skills闸门与执行规范重构/design.md`

**关键决策**：
- `main` 只是默认入口模型，不代表 shared root 实时已干净。
- 进入 Play Mode 做验证后，回到 Edit Mode 才算离场完成。
- UI / 气泡 / 字体 / 样式的审美与专业度现在是正式验收项，不再只是提醒。

**恢复点 / 下一步**：
- 治理主线已基本闭环。
- 当前未完成项不在治理文档，而在 shared root 仍残留的 NPC 业务 dirty；这不是本轮治理提交的白名单范围。

### 会话 9 - 2026-03-18（对 dirty 循环的彻底反思与恢复路线重定）
**用户目标**：
> 用户明确要求我停止“半成品也算完成”的治理叙事，正面回答为什么 skills / superpower 落了这么多轮仍没有真正拦住错误现场写入，并给出恢复正常开发进度的现实路线；本轮不要先照 Gemini 的建议动 Git，而是先把问题想透。

**已完成事项**：
1. 重新实核 live Git：
   - `D:\Unity\Unity_learning\Sunset`
   - `codex/npc-roam-phase2-003`
   - `HEAD = 2ecc2b753ea711557baca09432d0c7e3760cb3f7`
   - 仍有 farm 两份 memory dirty 与 `2026.03.18线程并发讨论` 未跟踪目录
2. 复核 worktree 现场，确认 farm cleanroom 仍存在，说明“worktree 已彻底退役”的旧结论已经失效。
3. 结合 `NPC / 导航检测 / farm / spring-day1 / 遮挡检查` 的回包和 reflog，确认真实事故链是：
   - shared root 先被切到 NPC 分支且未归还
   - 导航检测继续在这条 NPC 分支上提交 `2ecc2b75`
   - farm 再留下 unrelated dirty
   - 其他线程因此无法把 shared root 当成中性入口重入
4. 明确这次失败的核心责任不在“并发目标”，而在“治理执行面没有变成物理约束”：
   - live 分支不匹配时没有被硬停
   - owner 占用后没有强制归还
   - worktree 退役没有持续验收
   - skills/AGENTS 仍可被绕过，只是说明，不是闸机

**关键决策**：
- 当前用户的真正目标是：`main-common + branch-task + checkpoint-first + merge-last`，而不是“所有线程永远不能并发”。
- 在这个模型下：
  - 读、预检、设计、只读取证可以并发
  - 真实写入必须单 shared root 单写者串行
  - 归还不是等整个功能做完，而是到 checkpoint 就归还
- 恢复正常开发的顺序必须是：
  1. 冻结当前 shared root 新写入
  2. 让当前 owner 先归还现场
  3. shared root 回 clean `main`
  4. 再恢复 branch-only 开发
- 后续治理应继续回到 `09`，但重点必须转成“分支不对即停、现场不对即停、Play Mode 未退即不许交还”的硬闸门。

**恢复点 / 下一步**：
- 先完成一次“现场回正”，再谈正常并发开发。
- 在硬闸门没有代码级落地前，不能再宣称系统已经稳定恢复。

### 会话 10 - 2026-03-18（阶段 20 接管与旧阶段迁移标记落盘）
**用户目标**：
> 先做文档工作，把新的认知和代办落入新的 `20` 阶段；旧阶段只保留历史作用，但必须标记“未解决问题已迁移”，避免以后再读乱。

**已完成事项**：
1. 新建 `20_shared-root现场回正与物理闸机落地` 阶段，并写入：
   - `analysis.md`
   - `tasks.md`
   - `memory.md`
2. 在新阶段中落盘：
   - live 现场真相
   - 事故根因
   - 对 Gemini 建议的采用边界
   - 待审核的现场回正清单与物理闸机任务清单
3. 在旧阶段显式补写迁移说明：
   - `09/tasks.md`
   - `11/tasks.md`
   - `12/tasks.md`
   - `11/memory.md`
   - `12/memory.md`
4. 把父级与线程级摘要切换到 `20` 主线，停止继续把 `09/11/12` 当作 live 入口。

**关键决策**：
- `20` 现在是治理线程新的唯一活跃主线。
- 旧阶段继续保留其历史事实，但后续再读必须被导向 `20`。
- 这一步仍然是文档先行，不代表 Git 手术已经获批执行。

**恢复点 / 下一步**：
- 等用户审核 `20` 阶段清单。
- 审核后再进入实操方案输出，不提前越权改 Git。

### 会话 11 - 2026-03-18（第一波脚本闸机已落地）
**用户目标**：
> 在文档修正后直接执行，不要继续停留在空设计；先把最核心的 `git-safe-sync.ps1` 闸机写进去，再标记任务清单。

**已完成事项**：
1. 先重新实核 live 现场，确认当前仍不是 `main + clean`，因此没有把 Gemini 的人工清场假设误写进文档。
2. 直接修改 `scripts/git-safe-sync.ps1`：
   - 新增强制参数 `-OwnerThread`
   - `task` 模式位于 `main` 时直接阻断
   - `task` 分支必须具备 `codex/` 前缀
   - `task` 分支必须匹配 `OwnerThread`
   - `ensure-branch` 目标分支也必须匹配 `OwnerThread`
3. 同步修正文档：
   - `AGENTS.md`
   - `git-safety-baseline.md`
   - `基础规则与执行口径.md`
   - `shared-root-branch-occupancy.md`
   - `Sunset当前唯一状态说明_2026-03-17.md`
4. 更新 `20/tasks.md`，把第一波脚本闸机标记为已完成。
5. 完成三条自测，确认脚本能拦住“错线程 / 错分支 / 错目标分支”。

**关键决策**：
- 这轮真正落下去的是“脚本闸机第一波”，不是 shared root 现场回正。
- 所以当前可以说“第一波防呆已开始落地”，但还不能说“系统已经恢复正常开发”。

**恢复点 / 下一步**：
- 继续输出 shared root 回正 runbook。
- 审核后再决定是否补第二波 owner/lease 闸机。

### 会话 12 - 2026-03-18（第二波 shared root 最小闸机补齐）
**用户目标**：
> 不要只停在第一波 `OwnerThread` 闸机；继续把 shared root 的 owner/lease 与 remaining dirty 阻断也补到最小可用，并把文档口径统一掉。

**已完成事项**：
1. 在 `scripts/git-safe-sync.ps1` 中继续落下第二波最小闸机：
   - occupied shared root 必须匹配 `owner_thread + current_branch`
   - shared root 上如果仍有未纳入白名单的 remaining dirty，`task` 模式直接阻断
   - shared root 的 `main` 在 `ensure-branch` 前必须先恢复 neutral
2. 同步修正文档与阶段清单：
   - `shared-root-branch-occupancy.md`
   - `git-safety-baseline.md`
   - `基础规则与执行口径.md`
   - `AGENTS.md`
   - `20/tasks.md`
3. 重新自测：
   - 当前 `codex/npc-roam-phase2-003` shared root 上，`OwnerThread = NPC` 也会被 remaining dirty 拦住
   - `OwnerThread = 农田交互修复V2` 会被 branch mismatch + shared root owner mismatch 双重拦截
   - 在干净的 farm cleanroom 工作目录里，调用 shared root 最新脚本时，`task` preflight 可以正常通过
4. 新发现：
   - farm cleanroom 自带的 `scripts/git-safe-sync.ps1` 还是旧副本，没有 `-OwnerThread`
   - 这说明历史 worktree 会发生脚本与规则漂移，进一步支持“shared root 才是 live 治理入口，worktree 只保留例外”的判断

**关键决策**：
- 第二波最小闸机现在已落盘，但 shared root 现场仍未回正，所以这轮仍属于“治理推进中”，不是“开发已恢复正常”。
- 当前 owner/lease 校验只覆盖“共享根目录已声明 occupied 时的硬阻断”；自动 claim/release wrapper 仍待后续判断，不冒充已完成。

**恢复点 / 下一步**：
- 继续产出 shared root 人工回正 runbook。
- 等 runbook 经过用户审阅后，再决定是否进入 Git 外科执行或继续补自动 claim/release。

### 会话 13 - 2026-03-18（shared root 人工回正 runbook 已产出）
**用户目标**：
> 在补完第二波最小闸机后，继续把 shared root 人工回正 runbook 真正写出来，并把当前 owner / blocker / blocked、回退点和风险写成可审核执行的清单。

**已完成事项**：
1. 重新核实当前 live Git 与目标回退点：
   - 当前 shared root：`codex/npc-roam-phase2-003 @ 2ecc2b75`
   - 目标回退点：`c81d1f99`
   - 当前 `origin/codex/npc-roam-phase2-003` 也在 `2ecc2b75`
   - `main / origin/main @ 64ff9816`
2. 在阶段 `20` 新增：
   - `runbook.md`
   - 内容包括：
     - 当前 owner / blocker / blocked 分层裁定
     - stage 20 治理脏改与 farm dirty 的分组 park
     - 推荐 `reset --hard c81d1f99 + push --force-with-lease` 路径
     - 保守 `revert 2ecc2b75` 的替代路径
     - shared root 切回 `main` 后只恢复治理 stash、不恢复 farm stash 的步骤
     - neutral 回填与最终验收标准
3. 同步更新 `20/tasks.md`，把 A 阶段 runbook 四项任务全部标记为完成。

**关键决策**：
- 现在阶段 `20` 已具备“手术前准备层”的三件核心物：
  - 第一波脚本闸机
  - 第二波 shared root 最小闸机
  - A 阶段人工回正 runbook
- 但 Git 外科本身仍未执行，shared root 仍未回正，所以当前状态仍不是“恢复正常开发”。

**恢复点 / 下一步**：
- 等用户审核 `runbook.md`。
- 审核通过后，再由用户人工执行危险步骤，或继续让我补自动 claim / release wrapper。

### 会话 14 - 2026-03-18（A 阶段已执行，D 阶段模型已补）
**用户目标**：
> 既然已经批准执行，就直接完成 shared root 回正、继续补 D 阶段文档，并把治理主线推进到可最终同步的状态。

**已完成事项**：
1. 执行 A 阶段 Git 外科：
   - 分组 park `stage20-governance-parking`
   - 分组 park `farm-root-dirty-parking`
   - 将 `codex/npc-roam-phase2-003` 从 `2ecc2b75` 回正到 `c81d1f99`
   - 使用 `push --force-with-lease` 回正远端
   - 将 shared root 切回 `main`
2. 恢复治理 stash，不恢复 farm stash。
3. `git worktree list --porcelain` 现在只剩 shared root，farm cleanroom 已在 Git 层退役。
4. 新增 D 阶段文档：
   - `operating-model.md`
   - 正式写入 `main-common + branch-task + checkpoint-first + merge-last`
5. 更新当前活入口与占用文档，使当前口径变成：
   - shared root 已回到 `main`
   - 当前仍处于 `governance-main-finalizing`
   - 做完最后一次治理同步后再回到最终 neutral

**关键决策**：
- 当前阶段 `20` 已经从“准备手术”推进到“手术已做完，正在收口”。
- 当前剩余工作是治理线程在 `main + governance` 上做最后同步，不再是业务错位继续扩散。
- farm cleanroom 如果磁盘上还有空目录残壳，也已经不再拥有 Git worktree 身份。

**恢复点 / 下一步**：
- 用新版 `git-safe-sync.ps1 -Mode governance` 完成治理同步。
- 同步完成后，把 shared root 占用文档收成最终 neutral，并输出全线程唤醒清单。

### 会话 15 - 2026-03-18（治理同步 checkpoint 已推到 main）
**用户目标**：
> 在 shared root 回正后，不要停在本地脏改；继续把阶段 20 的文档、脚本、任务板和记忆一起同步到 `main`。

**已完成事项**：
1. 用新版 `git-safe-sync.ps1 -Action sync -Mode governance -OwnerThread Codex规则落地` 在 `main` 上完成治理同步。
2. 新增并推送治理 checkpoint：
   - `2966daa5`
3. 当前阶段 20 的核心成果已全部进入 `main`：
   - 第一波分支语义闸机
   - 第二波 shared root 最小闸机
   - A 阶段回正 runbook
   - D 阶段 operating model
4. 当前正在做最后的 neutral 回填与收尾说明，不再涉及新的危险 Git 手术。

**关键决策**：
- 阶段 20 现在已经从“本地治理准备”推进到“已有主干 checkpoint 的 live 治理成果”。
- 接下来只需要再做一次很小的 finalizing commit，就能把 shared root 明确收成 `main + neutral`。

**恢复点 / 下一步**：
- 完成 neutral 回填。
- 输出全线程唤醒清单。
- 再审视是否需要另立后续阶段承接 superpower / wrapper 化约束。

### 会话 16 - 2026-03-18（阶段 20 封板与本地原生技能暴露）
**用户目标**：
> 把阶段 20 真正做成“会留在系统里的完成态”，把 superpower 复审、本地技能命中、唤醒 prompt、全局技能注册和 learnings 都补齐，再给出最终验收与下一步判断。

**已完成事项**：
1. 复审 `obra/superpowers`，明确：
   - 原版继续 `rejected-as-is`
   - 只吸收其“先过技能闸门再行动”的方法论
2. 在本机创建并核验：
   - `C:\Users\aTo\.agents\skills\skill-vetter`
   - `C:\Users\aTo\.agents\skills\skills-governor`
   - `C:\Users\aTo\.agents\skills\sunset-startup-guard`
3. 在阶段 `20` 新增：
   - `wakeup-prompts.md`
4. 同步更新：
   - `20/tasks.md`
   - `20/memory.md`
   - `Codex规则落地/memory.md`
   - 当前线程 `memory_0.md`
   - 全局技能注册与全局 learnings
5. 保留唯一剩余物理尾巴记录：
   - `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
   - 当前只是被占用的空目录残壳

**关键决策**：
- 阶段 20 已封板，不再继续接新尾巴。
- 现在的系统已经恢复到用户要求的 `main-common + branch-task + checkpoint-first + merge-last` 常态。
- 后续若还要更强的自动 wrapper 或 session 级强闸机，必须另立新阶段承接。

**恢复点 / 下一步**：
- 当前可直接唤醒业务线程恢复工作。
- 若用户继续要更强的强制命中，再新开治理阶段。

### 会话 17 - 2026-03-18（第一次唤醒复盘后确认还缺“分支切换租约闸门”）
**用户目标**：
> 通读第一次唤醒的全部回包，判断真正的问题出在哪，并明确我是否建议新开阶段继续治理。

**已完成事项**：
1. 读取 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\20_shared-root现场回正与物理闸机落地\第一次唤醒\` 下全部 5 份回包。
2. 复盘出最短事故链：
   - `farm` 在第一次唤醒阶段提前执行 `ensure-branch`
   - shared root 被切到 `codex/farm-1.0.2-cleanroom001`
   - `导航检查` 的只读审计因此跑在错误 live 分支上
   - 用户察觉 VS 文档视图异常后要求切回 `main`
3. 判断这轮真正暴露的新缺口是：
   - `ensure-branch` 仍缺少“必须先拿到 shared root 独占租约”的强制条件
   - 第一次唤醒 prompt 仍把“只读复核”和“条件式进入真实分支”混在一轮里
4. 复核当前 live 状态：
   - shared root 已回 `main`
   - 当前新增 dirty 只有证据目录 `第一次唤醒/`

**关键决策**：
- 阶段 20 不是失败，而是“完成了第一段硬阻断”；新问题属于下一段治理目标。
- 我建议立项 `21`，专做：
  - 第一次唤醒复盘
  - shared root 分支租约闸门
  - 两阶段唤醒协议

**恢复点 / 下一步**：
- 若用户同意，下一步就该把 `第一次唤醒/` 证据纳入治理工作区，并以 `21` 继续推进。

### 会话 18 - 2026-03-18（阶段 21 进入物理落地）
**用户目标**：
> 不再反复讨论原理，要求直接产出并尽量落地：`git-safe-sync.ps1` 的 shared root 租约闸机、`AGENTS.md` 钢印、两个 skill 的 `default_prompt`、以及双阶段唤醒模板。

**已完成事项**：
1. 在 shared root 仓库内完成：
   - `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md`
   的物理改造。
2. 在本机全局 skill 目录完成：
   - `C:\Users\aTo\.codex\skills\skills-governor\agents\openai.yaml`
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\agents\openai.yaml`
   的强制钢印配置。
3. 已完成两次低风险验证：
   - `preflight` 正常读取 grant 字段
   - 无 grant 的 `ensure-branch` 被脚本 FATAL 阻断

**关键决策**：
- 这轮以后，shared root 上的 `ensure-branch` 不再允许在第一次唤醒阶段出现。
- 第一次唤醒只读、第二次 grant 准入的协议已成为新的恢复入口模型。

**恢复点 / 下一步**：
- 将仓库内治理文件白名单同步到 `main`。
- 输出最终标准版双阶段唤醒 Prompt，供后续重新唤醒业务线程。

### 会话 19 - 2026-03-18（回顾 Kiro 规则正文与治理历史）
**用户目标**：
> 在继续恢复线程前，要求重新回顾 Kiro 的所有核心规则与 `Codex规则落地` 历史，深度理解最初规范与后续补强，不要再漂移。

**已完成事项**：
1. 回读当前 live 入口、核心 steering 正文与 `Codex规则落地` 根层设计。
2. 回看 `01/02/05/09/10/11/12/20/21` 的阶段任务和阶段记忆，重新串起治理演化链。
3. 重新确认用户一直在强调的 6 条主轴：
   - 代办区不是工作区
   - shared root 是 `main-common`，真实写入先进 `codex/...`
   - `worktree` 只作例外
   - Git / shared root / Unity-MCP / 热文件锁是分层闸门
   - PlayMode 必须退回 EditMode；UI/样式审美是硬验收
   - 规则必须尽量物理化，不能只停在文档

**关键决策**：
- 阶段 21 之后，新的治理重点不再是补更多解释，而是继续审计哪些旧规则还只是说明书、尚未真正具备拦截力。

**恢复点 / 下一步**：
- 待用户唤醒线程期间，继续整理“纸面规则 vs 物理闸机”的残余差距清单。

### 会话 20 - 2026-03-18（按文件来源向用户做精简汇报）
**用户目标**：
> 简略汇报本轮实际查询过的文件。

**已完成事项**：
1. 整理出本轮回顾所读取的文件来源清单。
2. 将汇报结构定为：路径分组 + 一句话用途。

**关键决策**：
- 这轮仅汇报来源，不新增新的治理判断。

**恢复点 / 下一步**：
- 如用户继续追问，再从这些来源文件中展开具体规则差距。

### 会话 21 - 2026-03-18（紧急暂停历史回读，转入租约死锁修复）
**用户目标**：
> 暂停当前“历史回读 / 查漏补缺”主线，先把任务和进度迅速存入代办，然后立刻处理 `farm` 的在线测试暴露出的 shared root 租约死锁。

**已完成事项**：
1. 先把原主线暂停点与恢复点写入 TD：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\TD_13_阶段21历史回读暂停与分支租约死锁修复.md`
2. 定位并修复脚本逻辑漏洞：
   - `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
   - 新增 shared root occupancy 的“合法租约运行态脏改”识别
   - `grant-branch / ensure-branch / return-main / preflight` 同步接入新过滤逻辑
3. 安全补强：
   - 阻止其他线程在 `main` 上越权清空不属于自己的 `branch-granted`
4. 低风险验证：
   - PowerShell 语法检查 `OK`
   - live `preflight` 已显示 occupancy 被归为 `shared root 租约运行态脏改`，不再被当作普通 blocking dirty

**关键决策**：
- 这次 `farm` 的失败被正式定性为脚本 Bug，不是线程失误。
- occupancy 不能被永久降级成噪音；只能在“合法租约运行态”下做精确豁免。
- 原主线并未取消，只是被 `P0` Bug 暂停。

**恢复点 / 下一步**：
- 当前最高优先级是让用户审核本轮脚本修补，然后清掉 `main + branch-granted` 中间态并重测 Farm。
- 修补完成后，再回到 `TD_13` 记录的历史回读恢复点，继续阶段 21 的查漏补缺。

### 会话 22 - 2026-03-18（用户要求显式展示已触发 skills，并建立统一 trigger log）
**用户目标**：
> 在 `B` 路线完成后，继续补上“skills 触发不可见、没有统一日志”的治理缺口，并说明当前 global learnings 已经沉淀了什么。

**已完成事项**：
1. 新增 `C:\Users\aTo\.codex\memories\skill-trigger-log.md`，定义固定模板并写入首条记录。
2. 在全局规则源、治理 skill 和 `Sunset/AGENTS.md` 中接入“首条 commentary 显式点名 skill + 收尾补记 trigger log”的硬要求。
3. 追加新的全局 learning：
   - `GL-20260318-002 skill-policy.explicit-skill-callout-and-trigger-log`

**关键决策**：
- 以后“用了 skill 但用户没看见、事后也查不到”被正式视为治理缺口，而不是可忽略的体验问题。
- trigger log 是执行审计层，不替代线程 / 项目记忆，也不替代 global learnings。

**恢复点 / 下一步**：
- 当前子任务完成后，可以向用户直接汇总：
  1. 当前已有的 global learnings 清单
  2. 本轮新增的 trigger log 机制
  3. 之后每个实质性任务会如何显式展示 skill 触发
- 若用户把主线切回阶段 21 的历史回读，则从 `TD_13` 的恢复点继续。

### 会话 23 - 2026-03-18（锐评审核 Gemini 建议并拉回开发恢复）
**用户目标**：
> 对 Gemini 关于“阶段 21 已封板、现在优先重测 farm、自动化 hook 先列代办”的建议走锐评审核，并以“尽快恢复可开发状态”为最终目标收口。

**已完成事项**：
1. 按 `code-reaper-review.md` 核查后，将 Gemini 建议判为 `路径 B`。
2. 已确认其核心判断成立：
   - 不应现在继续扩展新基建
   - 应先用当前 clean 现场做 farm 的实盘重测
3. 已做本地化修正：
   - “自动化 skill trigger hook”只先记入 `TD_14`
   - 不直接开启新的正文阶段

**关键决策**：
- 当前主线正式拉回“恢复开发能力”，不是“继续做补强二期工程”。
- farm 的阶段二重测是现在最关键的验证动作。

**恢复点 / 下一步**：
- 立即向用户交付可直接发给 farm 的阶段二 Lease / Grant 与准入 Prompt。

### 会话 24 - 2026-03-18（最终收口：当前现场已可进入 farm 重测）
**用户目标**：
> 把“TD_14 只记代办、当前优先恢复开发”的结论彻底固化，并确认当前现场已经健康到可以开始下一轮线程验证。

**已完成事项**：
1. 已通过 governance sync 将 `TD_14` 和本轮记忆更新同步到 `main`。
2. 最新治理提交：
   - `98fc19e6 / 2026.03.18-10`
3. 当前 shared root 现场：
   - `main`
   - clean
   - neutral
   - 无未消费 grant

**关键决策**：
- 当前已达到“可开始 farm 阶段二重测”的恢复状态。
- 自动化 hook 留待后续，不再干扰当前主线。

**恢复点 / 下一步**：
- 向用户输出 `路径 B` 锐评结论与可直接分发给 farm 的 Prompt。

### 会话 25 - 2026-03-18（farm 重测暴露第二个 shared root 闸机 bug）
**用户目标**：
> farm 已按 Prompt 执行阶段二，但 `grant-branch` 在 clean `main` 上仍然报 shared root 不干净；要求直接排查并修掉，让系统恢复可开发状态。

**已完成事项**：
1. 已在本地复现报错。
2. 已用脚本内部取值证明根因：
   - `Get-StatusEntries` 没有任何真实 dirty
   - `Get-BlockingStatusEntries` 却因为 `@($null)` 产生了 1 个幽灵条目
3. 已完成脚本最小修补：
   - 修 `Get-BlockingStatusEntries`
   - 修 `Get-RemainingDirtyEntries`
4. 已继续补上分支切换链路修补：
   - 唯一 runtime dirty 为 occupancy 时，允许受控 force checkout
   - `return-main` 改为恢复 occupancy 到 `HEAD` 基线

**关键决策**：
- 当前主线继续保持“恢复开发能力”，所以这次先修 shared root 闸机，不转去别的补强。
- 只有把 `return-main` 改成真正回到 `HEAD` 基线，shared root 才可能在闭环验证后重新 clean。

**恢复点 / 下一步**：
- 先把脚本和记忆同步到 `main`，再在 clean 现场跑 `grant -> ensure-branch -> return-main` 完整验证。

### 会话 26 - 2026-03-20（承认批次 01/02 仍属护航测试，并纠偏测试覆盖面）
**用户目标**：
> 用户明确指出：我最近发出的 prompt 并不是他真正要的开发内容，而是测试 / 护航型内容；要求我诚实承认，并说清楚为什么只有 `NPC / 农田交互修复V2` 在继续测试、还有哪些线程应该一起测。

**已完成事项**：
1. 重新只读核查 Sunset live 现场，确认当前为：
   - `main`
   - `neutral`
   - `clean`
   - queue runtime 历史条目均已收口，没有活跃 grant / waiting
2. 回读 `共享根执行模型与吞吐重构`、批次 01 / 02 入口与线程 prompt，确认用户判断成立：
   - 这些 prompt 的主内容仍是准入、只读核对、carrier 收束、merge-noise 判断、热文件通道验证
   - 还不是高密度的真实业务开发指令
3. 正式给出更准确的定性：
   - `smoke-test_01` 是交通系统实盘测试
   - 批次 01 / 02 是“开发前护航式准入 / 收束”
   - 不能再描述成“你要的开发内容已经在稳定分发”
4. 明确为什么后续只有 `NPC / 农田交互修复V2` 在继续：
   - 它们是用户当时反复点名的最高优先级业务线
   - 我把 smoke-test 通过后的第一批护航资源集中投给了它们
   - 但这样做也导致测试覆盖面偏窄，没把 `导航检查 / 遮挡检查 / spring-day1` 一起带入下一轮
5. 明确后续如果目标仍是“交通系统测试”，至少还应覆盖：
   - `导航检查`
   - `遮挡检查`
   - `spring-day1`

**关键决策**：
- 用户这次纠偏必须被视为有效治理结论：
  - 我不能再把护航式 prompt 包装成真正的业务开发 prompt。
- 当前阶段应诚实描述为：
  - 交通系统已完成一轮 smoke-test 闭环
  - 两条高优先级业务线又完成了两轮开发前收束
  - 但还没进入用户真正要的“明确业务目标 + 明确交付面”的 prompt 阶段
- 如果继续测交通系统，样本集应扩到：
  - `NPC`
  - `农田交互修复V2`
  - `导航检查`
  - `遮挡检查`
  - `spring-day1`
- 如果切到真正开发，就应停止继续发“探测 / 收束型” prompt，改发明确业务交付 prompt。

**涉及文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_05_真实开发准入批次_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_06_真实开发准入批次_02.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\线程分支对照表.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\locks\active\shared-root-queue.lock.json`

**恢复点 / 下一步**：
- 对用户的下一轮回复必须直说：
  1. 你说得对，我最近发的仍主要是测试 / 护航 prompt。
  2. 只推进 `NPC / 农田` 是因为我把资源集中给了最高优先级业务线，不代表覆盖面已经合理。
  3. 后续若继续做测试，应把 `导航 / 遮挡 / spring-day1` 一起纳入。
  4. 后续若切真正开发，应改发具体业务开发 prompt，而不是继续探测。
### 会话 27 - 2026-03-20（主线再锚定：当前仍是“交通系统可用 + 业务开发未全面发车”）
**用户目标**：
> 继续当前主线，不要漂移；直接进入复核并告诉我现在究竟做到哪、哪些内容只是测试、哪些线程还没真正进入开发。

**本轮完成事项**
1. 重新执行前置核查并显式命中：
   - `skills-governor`
   - `sunset-workspace-router`
   以手工等价方式完成 Sunset 启动闸门与工作区定位。
2. 复核 live 现场：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `git status --short --branch = ## main...origin/main`
   - occupancy = `main + neutral`
   - queue runtime 当前无活跃条目
3. 回读 `共享根执行模型与吞吐重构` 根层记忆、批次 05/06 分发入口、批次 02 回收卡，确认：
   - `smoke-test_01` 已完整闭环
   - `真实开发准入批次_01 / 02` 已全部收口
   - 但这些批次仍主要是准入、核验、收束，不是高密度业务实现
4. 明确补正此前口径：
   - 我最近确实还在做测试/护航主导，而不是你真正要的完整开发内容分发
   - 之所以只持续推进 `NPC / 农田交互修复V2`，是因为它们被优先级前置，不代表其他线程已被同轮覆盖

**关键决策**
- 当前主线仍是“把共享根执行模型从可测试推进到可用于真实开发分发”。
- 当前已经完成的是交通系统基础闭环与两条高优先级业务线的开发前收束。
- 当前还没完成的是：
  - 面向真实业务交付的 prompt 体系
  - `导航 / 遮挡 / spring-day1` 的同轮覆盖策略

**涉及文件**
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_05_真实开发准入批次_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_06_真实开发准入批次_02.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\2026.03.19_真实开发准入批次_02\线程回收\NPC_phase2集成清洗.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\2026.03.19_真实开发准入批次_02\线程回收\农田交互修复V2_GameInputManager热文件专项.md`

**验证结果**
- live 仓库 clean
- shared root neutral
- queue 无残留 waiting / granted / task-active

**遗留问题 / 下一步**
- 如果继续做“交通系统覆盖测试”，应把 `导航检查 / 遮挡检查 / spring-day1` 纳入下一轮。
- 如果切“真实开发”，下一步就不再发准入型 prompt，而是直接发业务交付型 prompt。

## 会话 2026-03-20：从测试型分发切到真实业务开发批次 03
**当前主线目标**
- 沿 `Codex规则落地` 治理主线，把共享根执行模型真正切换到“能服务真实业务开发恢复”的阶段，而不是继续停留在 smoke / 准入 / 护航。

**本轮子任务 / 阻塞**
- 用户明确纠偏：此前批次 01/02 仍然偏测试和护航，真实开发必须开始。
- 当前阻塞不是规则缺失，而是需要把分发内容改成真实业务开发口径，并在发出前把治理写脏的 `main` 收口回 clean。

**本轮完成**
- 已生成新的真实业务开发批次：
  - 根层入口：`D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-20_批次分发_07_真实业务开发批次_03.md`
  - 阶段目录：`D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\2026.03.20_真实业务开发批次_03\`
- 已生成两份真实业务开发 prompt：
  - `NPC_phase2交付面收口.md`
  - `农田交互修复V2_1.0.2交付面收口.md`
- 这两份 prompt 均已写明：
  - 具体业务目标
  - 允许修改 / 保留范围
  - 必须排除的治理噪音
  - `request-branch -> ensure-branch -> sync -> return-main` 收口路径
  - `农田` 的 `GameInputManager.cs` 热文件锁要求
- 已同时建立固定回收卡，便于后续治理镜像收件。

**关键决策**
- 这轮不再继续包装“准入 / 核验 / carrier 收束”成真实开发，而是第一次直接发真实业务开发 prompt。
- 当前仍优先恢复 `NPC / 农田交互修复V2`，其他线程不在本轮立刻发车。
- 在把这轮治理改动 sync 回 `main` 之前，不能对用户谎称“现在已经可以直接分发”。

**涉及文件 / 路径**
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-20_批次分发_07_真实业务开发批次_03.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\2026.03.20_真实业务开发批次_03\可分发Prompt\NPC_phase2交付面收口.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\2026.03.20_真实业务开发批次_03\可分发Prompt\农田交互修复V2_1.0.2交付面收口.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\2026.03.20_真实业务开发批次_03\线程回收\NPC_phase2交付面收口.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\2026.03.20_真实业务开发批次_03\线程回收\农田交互修复V2_1.0.2交付面收口.md`

**验证结果**
- 已复核 current HEAD 为 `9f1dbafc`
- 当前 still dirty 的来源是治理线程本轮新增的分发文件与记忆追加，不是业务线程残留占用

**恢复点 / 下一步**
- 下一步先执行治理 sync 收口，把 `main` 恢复到 clean。
- 收口完成后，再把这轮批次正式交付用户群发。

## 会话 2026-03-20：NPC 主场景断链事故复核与 main 资产恢复
**当前主线目标**
- 继续治理主线，但本轮插入的阻塞处理是：把 `Primary.unity` 中 `NPC 001/002/003` 再次红掉的事故查清并恢复，确保真实业务开发不会建立在“branch 正常、main 场景仍断链”的假基线上。
**本轮子任务 / 阻塞**
- 用户观察到 `NPC` batch03 后场景一度恢复，但 `farm` 收口回到 `main` 后又出现 `Missing Prefab / Missing Sprite / Missing RuntimeAnimatorController`；要求判断到底是谁动了什么。
**本轮完成**
- 已重新核对 `main...codex/farm-1.0.2-cleanroom001`，确认 `farm` 对 NPC phase2 路径无差异；真正根因是 `main` 的 `Primary.unity` 已引用 `codex/npc-roam-phase2-003` 才有的 prefab / meta / profile / runtime 资产，但这批资产此前未真正合回 `main`。
- 已从 `codex/npc-roam-phase2-003` 把 `Assets/100_Anim/NPC/`、`Assets/111_Data/NPC/`、`Assets/222_Prefabs/NPC/`、`Assets/Sprites/NPC/001~003.png.meta`、`Assets/YYY_Scripts/Controller/NPC/`、`Assets/YYY_Scripts/Data/NPCRoamProfile.cs`、`Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs`、`Assets/Editor/NPCPrefabGeneratorTool.cs` 精确恢复到当前 `main` 工作树，并静态验证引用链已经闭合。
- Unity Editor 本轮已发生脚本重编译与 Asset Refresh；MCP 桥失联，未能做 Inspector 二次读取，但 Editor.log 尾部未再出现新的 NPC missing 报错。
**恢复点 / 下一步**
- 下一步直接把这批 NPC 恢复文件连同治理记忆最小提交到 `main`，恢复 clean 基线。
- 这轮阻塞解除后，主线回到：补强真实业务批次的验收口径，明确 `main-ready` 检查，再继续 `NPC / 农田` 的真实开发推进。
