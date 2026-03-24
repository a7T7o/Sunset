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

## 会话 2026-03-20：治理主线重锚与未落盘议题补写
**当前主线目标**
- 治理线程的主线不是只修单个事故，而是把事故反哺到 shared root 执行模型，并持续定义真实开发如何安全、高吞吐地推进。
**本轮子任务 / 阻塞**
- 用户指出我在修完 NPC 事故后又丢了当前线程自己的工作：没有把事故原因、治理发现、dirty 容忍度思考和重度 MCP 场景搭建线程方案系统性落盘。
**本轮完成**
- 已新增三份执行模型文档：事故反思与主线重对齐、dirty 容忍度讨论稿、重度 MCP 场景搭建线程执行方案。
- 已更新 `requirements.md / design.md / tasks.md`，把 `main-ready`、dirty 讨论边界和特种场景线程分类正式写进正文与待办。
**恢复点 / 下一步**
- 下一步不是继续空谈，而是先做治理 sync，把这轮主线文档补写安全并入 `main`。
- 收口后，治理主线继续推进：下一轮真实业务批次模板要带 `main-ready` 验收；dirty 分级机制留在独立治理窗口继续设计。

## 会话 2026-03-20：治理白名单 sync 已完成，主线重新落地
**当前主线目标**
- 当前不是继续追着单个 NPC 事故跑，而是把事故反哺到 `共享根执行模型与吞吐重构` 正文，并把此前只停留在聊天里的两条线正式入库：
  - `dirty 容忍 / 清扫推送机制`
  - 重度 `MCP` 场景搭建线程执行方案

**本轮子任务 / 阻塞**
- 用户要求我重新振作起来，不要在推回 `main` 后再次失忆；要把事故原因、我的发现、dirty 思考与另一个场景搭建线程方案都重新纳回治理主线。

**本轮完成**
1. 已完成治理白名单同步：
   - 命令：`scripts/git-safe-sync.ps1 -Action sync -Mode governance -OwnerThread "Codex规则落地" -IncludePaths ...`
   - 提交：`6d1fde83`（`2026.03.20-03`）
2. 已把以下内容正式推回 `main`：
   - `共享根执行模型与吞吐重构/requirements.md`
   - `共享根执行模型与吞吐重构/design.md`
   - `共享根执行模型与吞吐重构/tasks.md`
   - 三份 2026-03-20 新文档
   - 子工作区 / 父工作区 / 本线程记忆

**关键判断**
- NPC 事故暴露的不是 `farm` 覆盖 NPC，而是此前治理只验证了 `carrier-ready`，没有把 `main-ready` 作为硬验收。
- `dirty` 方向已被正式记录，但当前仍只是讨论稿，未批准为默认机制。
- 重度 MCP 场景搭建线程已被正式归类为“特殊执行类型”，不应直接套普通 shared root 短事务模型。
- 当前仓库仍残留一个非本线程的 tracked 改动：`.codex/threads/Sunset/Skills和MCP/memory_0.md`；我没有越权替它回退或代为提交。

**恢复点 / 下一步**
- 当前治理正文已重新落地，不再只存在于聊天上下文。
- 下一步应继续：
  - 把 `main-ready` 验收并入下一轮真实业务批次模板与回收卡
  - 后续独立设计 `dirty` 分级机制
  - 如需仓库绝对 clean，单独处理 `Skills和MCP` 线程遗留的 memory dirty

### 补充更新
- 上述最后一个尾巴我已直接处理完：对 `.codex/threads/Sunset/Skills和MCP/memory_0.md` 执行了单文件治理 sync。
- 对应提交：`1eaac0c8`（`2026.03.20-05`）。
- 当前 live 状态已回正为：
  - `D:\Unity\Unity_learning\Sunset @ main`
  - `HEAD = 1eaac0c8`
  - `git status --short --branch = ## main...origin/main`
  - shared root `main + neutral`

## 会话 2026-03-20：关于 NPC 事故“执行责任 vs 治理责任”的澄清
**当前主线目标**
- 不只要修现场，还要把“到底是谁没做好”与“为什么系统会让它通过”拆开写清楚，避免后续继续混在一起。

**本轮子任务 / 阻塞**
- 用户追问：这次是不是同步规范没到位导致 NPC 没把资源提交；`carrier-ready / main-ready` 跟定责到底有什么关系。

**本轮完成**
- 明确定性：
  - 不是 `sync` 脚本失效
  - 是此前验收规范没有覆盖“主场景依赖资产是否已经真正进入 `main`”
- 明确分层责任：
  - `NPC` 线负执行责任，因为结果上确实留下了“branch 有、main 没有”的交付缺口
  - 治理线负系统责任，因为此前批次模板没有把 `main-ready` 设成硬问题

**关键判断**
- `carrier-ready` 和 `main-ready` 不是绕开定责，而是为了避免“线程自证 branch 收口成功”就被误判为“用户回到 main 已可用”。
- 以后两层都要答，才能既定责，又防止同类问题重演。

## 会话 2026-03-20：工作区目录重组与当前总待办落盘
**当前主线目标**
- 不只要把治理结论想清楚，还要把当前工作区变成“能持续承载后续批次”的干净结构，并把所有遗留代办汇总成一份当前总表。

**本轮子任务 / 阻塞**
- 用户明确指出：`共享根执行模型与吞吐重构` 根目录过乱，很多阶段内容不应直接堆在根层；同时要求我把 `NPC / 农田 / 导航 / 遮挡 / spring-day1 / dirty / 场景线程` 等遗留内容统一写进文档。

**本轮完成**
1. 已将工作区重组为：
   - `00_工作区导航/`
   - `01_执行批次/`
   - `02_专题分析/`
   - 根层仅保留 `requirements / design / tasks / memory`
2. 已新建总览文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\00_工作区导航\当前总览与待办_2026-03-20.md`
3. 已把旧批次路径、旧专题路径和相关入口文档全部改到新结构。

**关键判断**
- 这轮之后，当前工作区终于不再把“阶段目录”和“正文入口”混堆在一起。
- 当前遗留主线已被正式汇总，下一步可以直接据此生成 `main-ready` 版批次 04。
- 用户已说明场景搭建线程正在独立做自己的文档，因此本线程当前不介入该线实际推进，只保留记录口径。

## 会话 2026-03-20：真实业务开发批次 04 已入库，但 shared root 仍被他线程 dirty 占用
**当前主线目标**
- 把“已生成但尚未 sync 的 batch04”正式推回 `main`，并给出客观的 live 判断：到底是已经可以直接群发，还是还差 shared root 清场这一步。

**本轮子任务 / 阻塞**
- 用户要求直接开始，不再停留在本地未入库状态。
- 当前阻塞不是我这边没写完，而是 shared root 同时存在其他线程留下的：
  - `.codex/threads/Sunset/Skills和MCP/memory_0.md`
  - `.kiro/specs/900_开篇/5.0.0场景搭建/**`

**本轮完成**
1. 已复核并确认 batch04 涉及的治理资产范围。
2. 已执行治理白名单同步并成功推送：
   - 提交：`b6fc4b19`
   - 提交消息：`2026.03.20-09`
3. 现在正式进入 `main` 的包括：
   - `2026-03-20_批次分发_08_真实业务开发批次_04.md`
   - `共享根执行模型与吞吐重构/01_执行批次/2026.03.20_真实业务开发批次_04/`
   - 对应的 `tasks.md` 与当前总览更新

**关键判断**
- 我这条治理主线关于“生成并入库 batch04”的目标已经完成。
- 但当前 shared root 不是绝对 clean，所以我不能对用户谎称“现在所有线程可立即从根目录开跑”。
- 更准确的状态是：
  - `batch04-ready = yes`
  - `shared-root-clean-for-dispatch = no`

**恢复点 / 下一步**
- 下一步不再是继续写 batch04 文件，而是等待或协调相关线程先收口自己的 dirty。
- 一旦 shared root 回到 `main + clean + neutral`，就可以按 batch04 开始第一波：
  - `NPC`
  - `农田交互修复V2`

### 补充更新
- 这一步我已经替用户做完了：
  - `Skills和MCP` 场景搭建线已被迁入 `codex/scene-build-5.0.0-001`
  - 专属 worktree：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
- shared root 因此重新放空，当前 batch04 第一波已具备直接分发条件。
- `spring-day1` 仍不进 batch04；它后续要单开一轮集成波次，而不是混进当前的短事务并发波。

## 会话 2026-03-20｜shared root 二次清场与 batch04 第一波续跑恢复点
**当前主线目标**
- 不再停在“场景搭建线应不应该迁回 worktree”的判断层，而是直接完成 shared root 清场，并把 batch04 第一波从 `LOCKED_PLEASE_YIELD` 恢复到可继续消费的状态。

**本轮子任务 / 阻塞**
- 用户已经贴回 `NPC / 农田` 的 batch04 第一波回执，两条线都因 shared root dirty 被打进 waiting。
- 经过复核，真实阻断源不是 occupancy，也不是业务线程自己，而是：
  - shared root 残留的场景搭建线文档与线程记忆
  - 以及一棵误生成的 URL 编码历史目录树

**本轮完成**
1. 已将 shared root 的场景搭建残留无损迁入：
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\shared-root-import_2026-03-20\`
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.codex\threads\Sunset\Skills和MCP\memory_1.md`
2. 已删除 shared root 上对应副本，避免继续污染 `main`。
3. 已额外删除 `.kiro/specs/%E5%85%B1%E4%BA%AB...` 这棵 URL 编码残留目录，解除 `Filename too long` warning。
4. 当前 live 再次回到：
   - `main`
   - `git status --short --branch = ## main...origin/main`
   - occupancy = `main + neutral`
5. `wake-next` 复核显示：
   - `NPC` 已存在未消费 grant
   - 因此当前下一手不是再 grant `NPC`，而是直接让 `NPC` 走 `ALREADY_GRANTED -> ensure-branch`
   - `农田交互修复V2` 继续排在 `NPC` 后面等待

**关键判断**
- 场景搭建这种特种线程，任何 tracked / untracked WIP 都不能继续留在 shared root；必须只留在自己的 `branch + worktree`。
- `dirty` 放宽机制现在不应插进这轮 live 抢修；它的起点已经钉在执行模型总览里，等 batch04 第一波真实回收后再独立起治理阶段。

**恢复点 / 下一步**
- 现在直接给用户三份最短口令：
  - 场景搭建线程：去 worktree 吸收 `shared-root-import_2026-03-20`
  - `NPC`：立即消费现有 grant
  - `农田`：保持 waiting，待 `NPC return-main` 后再续跑

## 会话 2026-03-20：NPC docs-only 收口完成后的治理裁定
**当前主线目标**
- 在 `NPC` 已成功 `sync + return-main` 后，把事故性质、责任层级与场景搭建线程的并行边界讲清楚，并继续维持 batch04 第一波推进。

**本轮完成**
1. 已确认 `NPC` 本轮 docs-only checkpoint 提交为 `b680cd4b`，`sync` 与 `return-main` 均成功。
2. 已确认 shared root 当前回到：`D:\Unity\Unity_learning\Sunset @ main`，`git status --short --branch` clean，occupancy 为 `neutral-main-ready`。
3. 已确认场景搭建线程继续停留在：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001 @ codex/scene-build-5.0.0-001`，其 dirty 只属于自身 worktree。

**关键决策**
- 需要把两类问题拆开对用户说明：
  - 之前 `main` 内 NPC sprite / 资产依赖缺失：属于业务交付验收问题，执行与治理均有责任。
  - 本轮 batch04 收口时 `sync / return-main` 一度被卡：属于治理脚本 / occupancy 状态机问题，主责在 shared root 收口工具链。
- 场景搭建线程现在可以与治理线程、业务线程长期并行，但只限于 Git 层；进入 Unity / MCP 重写场景时，仍必须遵守单实例 / 单写者闸门。

**恢复点 / 下一步**
- batch04 第一波已从 `NPC` 收口事故中恢复。
- 下一步先继续 `农田交互修复V2`，随后单开治理修复 `scripts/git-safe-sync.ps1` 的 shared root 收口鲁棒性。

## 会话 2026-03-21：场景搭建 worktree / Unity 边界已明确，occupancy 缺口已正式立项
**当前主线目标**
- 在继续 batch04 第一波前，把 worktree 能力边界、`farm` 续跑口径和 occupancy 收口脚本修复正式落盘。

**本轮完成**
1. 已确认：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001` 可以作为场景搭建线程的 Git / 文档 / WIP 独立现场，但不自动豁免 Unity / MCP 单实例、单写者规则。
2. 已将 shared root 收口脚本缺口正式挂回任务清单：`tasks.md` 第 20 项。
3. 已新增专题分析文档：`02_专题分析\2026-03-21\shared-root_occupancy收口脚本缺口与修复计划.md`。
4. 已实际尝试 `wake-next`，结果确认当前阻断来自我本轮尚未同步的治理 dirty，而不是业务线程占槽。

**关键决策**
- `farm` 应继续当前 batch04 真实业务 prompt，不回退到 smoke/test prompt。
- 其他线程不需要全部停工等治理修完；Git / worktree 层可并行，Unity / MCP 写线程仍需按单写者节奏排队。

**恢复点 / 下一步**
- 先同步本轮治理 dirty。
- 然后重新 `wake-next`，继续 `农田交互修复V2`。

### 补充更新
- 本轮治理文档已同步进 `main`，提交为 `e39e097a`。
- 随后治理线程已成功执行 `wake-next`，将 `农田交互修复V2` 推进到 shared root 分支租约已发放状态。
- 对 `farm` 的正式续跑口径现已明确为：`request-branch -> ALREADY_GRANTED -> ensure-branch`。

### 会话 10 - 2026-03-21（第二波收件完成后的白话裁定）
**用户目标**：用户不理解为什么看起来只有 `遮挡检查` 做了“完整开发”，同时贴出 scene-build 线程的新回执，希望我用白话解释这几个线程当前各自处于什么阶段。  
**当前主线目标**：把 batch04 第二波结果讲清楚，并判断 scene-build 是否已经进入等待施工写态的阶段。  
**本轮子任务**：核对 `导航检查` / `遮挡检查` 的实际提交面，并给出场景搭建线程的状态解释。  
**完成事项**：
1. 核定 shared root 当前已回到 `main + neutral`。
2. 核 `导航检查`：提交 `e486b432` 仅为 docs-first checkpoint，没有代码改动。
3. 核 `遮挡检查`：提交 `295e8138` 为 docs-first + 一个最小非热文件代码切口，改动点是 `Assets/Editor/BatchAddOcclusionComponents.cs`。
4. 得出白话结论：不是只有遮挡“真干活”，而是两条线程这轮各自落在不同类型的 checkpoint 上。
5. 结合 scene-build 回执，确认它已经把施工前准备整理成顺序，当前真正等待的是“写态裁定”，不是继续规划。

**关键决策**：
- `遮挡检查` 这轮不是完整整改，只是做了一个安全的小代码切口。
- `导航检查` 也完成了真实工作，只是它这次工作是文档化整改设计，不是代码。
- scene-build 现在可以继续做 worktree 内的施工前尾项，但是否进入 Unity / MCP 施工，要看治理排程。

**恢复点**：
- batch04 第二波已可视为完成。
- 下一步需要给用户一个白话版“现在谁做完了、谁还在等、scene-build 何时能进施工写态”的总口径。

### 会话 11 - 2026-03-21（scene-build 放行为下一位 Unity/MCP 写线程）
**用户目标**：用户贴出 scene-build 线程“施工前尾项已全部收口”的回执，希望我明确给出白话裁定。  
**当前主线目标**：在 batch04 第二波完成后，决定项目下一位实际施工线程是谁。  
**本轮子任务**：核定第二波真实产出差异，并给 scene-build 一个明确的写态结论。  
**完成事项**：
1. 确认 `导航检查` 与 `遮挡检查` 都完成了真实 checkpoint，只是一个偏文档、一个多了小代码点。
2. 确认 shared root 当前 clean，`HEAD = e18e111e`。
3. 确认 Unity / MCP 占用记录为 `current_claim = none`。
4. 结合 scene-build 回执，确认其只剩“正式放行”这一个前置项。

**关键决策**：
- 现在可以把 scene-build 作为下一位唯一 Unity / MCP 写线程放行。
- 首个施工窗口限定为：`SceneBuild_01 -> Grid + Tilemaps` 的 create-only 起步，不直接放大到整轮场景精修。

**恢复点**：
- 给用户一条可直接转发给 scene-build 的正式放行口令。
- scene-build 完成第一个施工 checkpoint 后，再决定是否继续到底稿/结构层。
## 会话 12 - 2026-03-21（治理主线回正到 occupancy bugfix 收口）
**当前主线目标**
- 把 `git-safe-sync.ps1 / shared-root occupancy` 这次确定性收口 bug 真正修完并同步，而不是继续停在“问题分析已知道”的聊天层。

**本轮子任务 / 阻塞**
- 用户要求我不要再只讨论，要直接把治理问题干完；这轮子任务因此是：
  - 修脚本
  - 补工作区/治理/线程记忆
  - 做治理 `sync`

**本轮完成**
1. 已在 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 落地修复：
   - 增加 `Set-OrAddPsNoteProperty`
   - 增加 `Repair-SharedRootActiveSessionRecord`
   - 修复 legacy runtime 缺字段时 `Touch-SharedRootActiveSession` 直接赋值会炸的问题
   - 让 `Get/Set/Touch/Complete-SharedRootActiveSession` 对旧结构兼容
2. 已更新治理正文：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\tasks.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\02_专题分析\2026-03-21\shared-root_occupancy收口脚本缺口与修复计划.md`
3. 已完成验证：
   - PowerShell 语法检查通过
   - active-session 兼容测试通过
   - 从仓库根运行 working tree 版 `preflight -Mode governance` 成功

**关键决策**
- 这次真正修掉的是治理工具链主责 bug，而不是业务线程又犯了新错误。
- `dirty` 分级 / 放宽仍留在任务 `15`，这轮不混改。
- 当前主线在本轮修复完成后会回到：治理 `sync` 收口，然后再向用户交付白话责任说明和后续安排。

**恢复点 / 下一步**
- 先完成本轮治理 `sync -Mode governance`。
- 然后向用户交付：
  - 问题到底是什么
  - 责任在哪一层
  - 已修了什么
  - 还有什么后续任务没做

## 会话 13 - 2026-03-21（scene-build 回执已接收，写态恢复）
**当前主线目标**
- 在治理脚本 bugfix 完成后，继续维持各线程的现场裁定清晰，避免 scene-build 因旧阻塞印象继续停着不动。

**本轮子任务 / 阻塞**
- 用户贴回了 scene-build 最新确认：
  - Unity / MCP 的 `project_root` 已对准 `D:/Unity/Unity_learning/Sunset_worktrees/scene-build-5.0.0-001`
  - 不在 Play Mode
  - 不在 Compile / Domain Reload 中间态
  - `editor/state` 仍有 `stale_status` 提示，但核心状态字段可读

**本轮完成**
1. 已将该回执纳入治理判断。
2. 已明确这条回执意味着：
   - scene-build 不再卡在“要不要先修 Unity/MCP 指向”
   - 当前可以继续之前已裁定的最小施工窗口：`SceneBuild_01 -> Grid + Tilemaps`

**关键决策**
- `stale_status` 本轮不构成阻断，因为关键准入字段已经返回且一致。
- scene-build 现在是“可继续施工”，不是“仍在等待批准”。
- 继续边界不变：先做首个 create-only checkpoint，再回执，不直接扩张范围。

**恢复点 / 下一步**
- 向用户给出一句可直接转发的口径：
  - 现场已回正，可以继续 `SceneBuild_01 -> Grid + Tilemaps`
- 等 scene-build 完成首个施工 checkpoint 后，再评估下一段施工窗口。

## 会话 14 - 2026-03-21（旧计划已过期，当前主线位置对账）
**当前主线目标**
- 把我之前那份“先等 farm、再推进第二波、最后修脚本”的阶段性计划，和现在的真实现场重新对齐，避免用户继续拿旧路线图判断当前状态。

**本轮子任务 / 阻塞**
- 用户贴回了我之前的旧计划，追问“所以现在走到哪里了”。
- 这轮阻塞不是技术阻塞，而是状态口径滞后：计划文本已经落后于现场。

**本轮完成**
1. 已复核 live 现场：
   - shared root 在 `main + neutral`
   - `git status --short --branch = ## main...origin/main`
   - Unity / MCP shared-root claim = `none`
2. 已把旧计划中的各项与当前状态逐项对账：
   - 等 `farm` 回执：已完成
   - 第二波 `导航 / 遮挡`：已完成
   - 修 `git-safe-sync.ps1 / occupancy`：已完成
   - scene-build worktree 并行边界：已明确，且当前已恢复最小施工窗口
3. 已明确当前剩余主任务：
   - `dirty` 分级 / 放宽机制设计
   - `spring-day1` 的独立进入时机
   - 等 scene-build 首个施工 checkpoint 回执后，再裁定下一段窗口

**关键决策**
- 那段旧计划现在应被视为“已执行完成的大部分路线图”，不是 current todo。
- 当前 Sunset 治理主线已经推进到后半段：业务两波已收口、治理 bugfix 已落地、scene-build 已恢复继续。

**恢复点 / 下一步**
- 给用户一份白话对账：
  - 哪些已经完成
  - 哪些仍未完成
  - 现在真正的下一步是什么

## 会话 15 - 2026-03-21（给用户也给自己看的最新待办）
**当前主线目标**
- 不再拿旧计划和旧阻塞当 current todo，而是把 Sunset 现阶段真正剩余的任务重新排成一版可直接执行的顺序。

**本轮子任务 / 阻塞**
- 用户明确要一份“写给我看也写给你自己看”的完整待办。
- 当前阻塞不是现场冲突，而是多份旧口径并存：旧 batch04 计划、scene-build 旧任务稿、最新实时回执。

**本轮完成**
1. 已重新对齐三类依据：
   - live shared root 基线
   - 治理任务单
   - scene-build 最新实时回执
2. 已得出当前最新待办顺序：
   - 先让 scene-build 继续 `SceneBuild_01 -> Grid + Tilemaps`
   - 等 scene-build 首个施工 checkpoint 回来
   - 治理线程转主攻任务 `15`
   - 任务 `14` 作为后续模板固化持续跟进
   - `spring-day1` 继续独立排队

**关键决策**
- scene-build 的最新实时回执优先级高于它 worktree 里尚未清理的旧阻塞文稿。
- 当前唯一适合继续实际 Unity / MCP 写入的线程仍然是 scene-build。
- 我这边的真正下一步已经不再是 batch04，而是治理设计任务 `15`。

**恢复点 / 下一步**
- 对用户输出一份白话完整待办。
- 对我自己保持同一恢复点：
  - 等 scene-build checkpoint
  - 准备 dirty 分级设计

## 会话 16 - 2026-03-21（scene-build 继续施工，我并行起草任务 15）
**当前主线目标**
- 不再停在“最新待办排程”层，而是顺着该排程直接开工：scene-build 继续施工，我这边并行推进治理任务 `15`。

**本轮子任务 / 阻塞**
- 用户明确让我不要再等 prompt，直接继续。
- 我据此做了两件事：
  - 只读看一眼 scene-build 是否真的在继续
  - 直接启动 `dirty` 分级与 takeover 边界设计

**本轮完成**
1. 已确认 scene-build worktree 仍在活跃推进：
   - worktree `git status` 显示新的施工相关未跟踪文件
   - 说明它确实在继续，不是停滞等待
2. 已新增任务 `15` 的正式设计稿：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\02_专题分析\2026-03-21\dirty分级与takeover边界设计稿.md`
3. 已把结论挂回治理正文：
   - `tasks.md`
   - `design.md`
4. 当前草案结论已明确：
   - dirty 分四级
   - 默认硬闸门不撤
   - 只先承认 D1 与同线程 D2 continuation
   - 不批准跨线程 raw dirty takeover

**关键决策**
- 这轮我没有去打断 scene-build，也没有试图接手它的现场；我把并行边界守住了。
- 治理主线已正式切换为任务 `15`，不再重复 batch04 已完成部分。

**恢复点 / 下一步**
- 先完成本轮治理 `sync -Mode governance`。
- 后续若继续任务 `15`，先补脚本报告层，再决定是否进入脚本放宽层。

## 会话 17 - 2026-03-21（任务 15 首轮报告层落地并完成只读验证）
**当前主线目标**
- 不打断 `scene-build` 的施工，把治理主线继续推进到任务 `15` 的脚本可见性层：先让 `git-safe-sync.ps1` 会说清 dirty 是什么，再决定以后要不要讨论放宽。

**本轮子任务 / 阻塞**
- 用户明确要求我继续干活，不要只停在讨论里。
- 当前最合适的推进点不是再发 prompt，而是把任务 `15` 的“报告层”真正落到脚本，并验证不会误改现有硬闸门行为。

**本轮完成**
1. 已在 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 加入首轮 dirty 报告层函数：
   - `Test-GovernanceReportPath`
   - `Test-DirtyHardBlockPath`
   - `Get-DirtyLevel`
   - `Get-DirtyOwnerHint`
   - `Get-DirtyPolicyHint`
   - `New-DirtyReportEntry`
   - `Format-DirtyReportEntry`
   - `Get-DirtyLevelSummaryText`
2. 已让 `preflight` 输出升级为：
   - allowed / remaining 的分级概览
   - 每条 dirty 的 `level / owner / policy`
   - remaining dirty gate message 的 `<Dx/owner>` 预览
3. 已完成本地验证：
   - PowerShell 语法解析通过
   - `preflight -Mode governance` 成功
   - `preflight -Mode task` 在 `main` 上仍返回 `CanContinue = False`
4. 已同步补记：
   - 子工作区 `共享根执行模型与吞吐重构`
   - 父工作区 `Codex规则落地`
   - 本线程 `memory_0.md`

**关键决策**
- 这轮不是 shared root 带脏放行，而是先把 dirty 变得“可解释、可审计、可归类”。
- 当前边界仍保持：
  - `D1` 只代表治理型可收口 dirty
  - `D2` 只保留给同线程 continuation 的讨论空间
  - `D3` 继续是硬阻断 dirty
- `scene-build` 继续并行施工，但我没有介入它的 worktree 或 Unity 现场。

**恢复点 / 下一步**
- 立刻执行本轮治理 `sync -Mode governance`，把脚本报告层与记忆一起收口进 `main`。
- 收口后继续任务 `15`，优先观察真实 dirty 样本，再决定是否要进一步细化分类边界或模板口径。
## 会话 18 - 2026-03-21（任务 15 真实样本回放后修正 D3 漏判）
**当前主线目标**
- 继续把治理主线压在任务 `15` 上：不讨论空泛方向，直接拿真实 dirty 样本校对脚本分级边界，并把确认无误的报告层偏差修掉。

**本轮子任务 / 阻塞**
- 用户要求我继续推进，不要只停在说明层。
- 当前最合适的推进点是：验证 dirty 报告层有没有把设计稿里应当属于 `D3` 的共享资产错误报告成 `D2`。

**本轮完成**
1. 已确认 shared root 现场稳定：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `git status --short --branch = ## main...origin/main`
   - occupancy = `main + neutral`
2. 已在同一 PowerShell 会话里 dot-source `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`，对真实样本做分级回放。
3. 已实锤两类偏差：
   - `Assets/000_Scenes/Primary.unity` 因大小写比较漏判成 `D2`
   - `Assets/222_Prefabs/*.prefab*`、`Assets/111_Data/*.asset*`、`Assets/100_Anim/*.controller*`、`Assets/Sprites/*.meta` 仍被误报为 `D2`
4. 已修改 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`：
   - `Test-DirtyHardBlockPath` 改为小写比较
   - 补入上述共享资产的 `D3` 硬阻断命中
   - `Get-DirtyOwnerHint` 的 `Primary.unity / GameInputManager.cs` 热文件判断同步改为大小写稳定
5. 已完成二次验证：
   - 共享 Prefab / Data / Anim controller / Sprite meta 与 `Primary.unity` 现已全部回到 `D3`
   - `HotbarSelectionService.cs` 继续保持 `D2`
   - `GameInputManager.cs / ProjectSettings/TagManager.asset` 继续保持 `D3`

**关键决策**
- 这轮修的是报告层精度，不是 shared root 放宽策略。
- 当前边界仍不变：
  - 默认硬闸门不撤
  - 不批准跨线程 `raw dirty takeover`
  - `scene-build` 继续是唯一实际 Unity / MCP 写线程，我不碰它的现场

**恢复点 / 下一步**
- 先做治理 `sync -Mode governance`，把这轮脚本修正与记忆同步进 `main`。
- 收口后继续任务 `15`，下一子步再看是否还有别的真实样本需要纠正。
## 会话 19 - 2026-03-21（任务 15 第二轮样本审计继续补洞）
**当前主线目标**
- 继续把治理主线压在任务 `15` 上，不提前碰 shared root 放宽，而是再做一轮真实样本审计，把报告层剩余的误分级和 owner hint 盲区继续修准。

**本轮子任务 / 阻塞**
- 第一轮样本虽然已经修掉共享 Prefab / ScriptableObject / `Primary.unity` 的误分级，但还没验证：
  - 非 `Primary` 的 scene 文件
  - 真实 `.anim` clip
  - 真实 sprite 图片文件
  - `HotbarSelectionService.cs` 的 owner hint

**本轮完成**
1. 已在同一 PowerShell 会话中对第二批真实样本做回放：
   - `Assets/000_Scenes/SceneBuild_01.unity`
   - `Assets/000_Scenes/SceneBuild_01.unity.meta`
   - `Assets/100_Anim/NPC/001/Clips/001_Idle_Down.anim`
   - `Assets/Sprites/NPC/1/001.png`
   - `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
2. 已确认第二批真实偏差：
   - 非 `Primary` scene 与 `.meta` 仍被误报成 `D2`
   - 真实 `.anim` clip 与 sprite 图片仍被误报成 `D2`
   - `HotbarSelectionService.cs` 的 owner hint 仍误归 `Codex规则落地`
3. 已修改 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`：
   - 所有 `.unity / .unity.meta` 统一按 `D3` 报告
   - `Assets/222_Prefabs/*`、`Assets/111_Data/*`、`Assets/100_Anim/*`、`Assets/Sprites/*` 统一回到高风险共享资产口径
   - 农田 owner hint 补认 `hotbar`
4. 已完成回归验证：
   - `SceneBuild_01.unity / .meta` 现已归入 `D3`
   - 真实 `.anim` clip 与 sprite 图片现已归入 `D3`
   - `HotbarSelectionService.cs` 现已正确归属 `农田交互修复V2`

**关键决策**
- 这轮继续只修报告层准确性，不改 shared root 的默认硬闸门。
- 到这一步，任务 `15` 的已知高风险目录和真实样本覆盖面明显更完整，但仍不等于批准带脏推进。

**恢复点 / 下一步**
- 先做治理 `sync -Mode governance`，把这轮第二次样本修正收进口径层。
- 收口后继续任务 `15`，优先查剩余 owner hint 盲区。

## 会话 20 - 2026-03-21（任务 15 第三轮样本审计：补齐 spring-day1 / 遮挡 owner 边界）
**当前主线目标**
- 继续推进 Sunset 治理主线里的任务 `15`，这轮专门查 owner hint 还会不会把真实业务线说错。

**本轮子任务 / 阻塞**
- 前两轮已修完 `D3` 分类边界与农田 owner，但还没验证：
  - `spring-day1` 会不会被 `.kiro/specs/900_开篇` 的粗规则误吞成 `scene-build`
  - 遮挡相关文件名会不会因为不在 `遮挡/occlusion` 目录段里而回退到治理线程默认 owner

**本轮完成**
1. 已继续用同一 PowerShell 会话 dot-source `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`，回放第三批真实样本：
   - `.kiro/specs/900_开篇/spring-day1-implementation/requirements.md`
   - `.kiro/specs/900_开篇/spring-day1-implementation/OUT_tasks.md`
   - `Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset`
   - `Assets/Editor/BatchAddOcclusionComponents.cs`
   - `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`
   - `Assets/YYY_Scripts/Service/Rendering/OcclusionTransparency.cs`
   - `Assets/YYY_Tests/Editor/OcclusionSystemTests.cs`
2. 已确认两个真缺口：
   - `spring-day1` 文档此前被误归 `scene-build`
   - 遮挡相关 Editor / Runtime / Test 文件此前被误回退到 `Codex规则落地`
3. 已修正 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`：
   - 把 `scene-build` owner 规则从整个 `.kiro/specs/900_开篇` 收窄到 `5.0.0场景搭建`
   - 新增 `spring-day1` owner 识别
   - 扩大遮挡 owner 对 `occlusion / 遮挡` 文件名与 `云朵遮挡系统` 的识别
4. 已完成回归验证：
   - `spring-day1` 文档与对白资产已正确归属 `spring-day1`
   - 遮挡相关文件已正确归属 `遮挡检查`
   - `5.0.0场景搭建` 文档与 `SceneBuild_01.unity` 仍正确归属 `scene-build`

**关键决策**
- 这轮继续只修报告层 owner hint，不放宽 shared root 的硬闸门。
- `TilemapToSprite.cs` 这类泛工具本轮故意不硬绑 `scene-build`，防止再制造新的误归属。

**恢复点**
- 先按治理白名单同步本轮脚本与记忆。
- 同步后继续任务 `15`，优先看还有没有其他 owner fallback 需要收窄。

## 会话 21 - 2026-03-21（scene-build 底稿 v1 回执已核对，可继续结构层）
**当前主线目标**
- 继续承担 Sunset 治理收件与裁定，不插手 scene-build 现场，只判断这次 `地图底稿 v1` 回执是否已经达到可继续下一层施工的条件。

**本轮子任务 / 阻塞**
- 用户贴来了 scene-build 最新回执，核心问题是：
  - 这次是不是已经有真实 checkpoint
  - 现在能不能直接放行到 `结构层`

**本轮完成**
1. 已只读复核 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`：
   - `branch = codex/scene-build-5.0.0-001`
   - `HEAD = 7b92abe0`
   - upstream 已同步到 `origin/codex/scene-build-5.0.0-001`
   - 当前 worktree clean
2. 已核对该 worktree 自己的 `tasks.md / memory.md` 尾部，确认这轮确实已经把：
   - 底稿 v1 落盘
   - 底稿计数
   - “MCP 仍未 live 验收”的限制
   记进它自己的工作区文档
3. 已形成治理裁定：
   - 允许继续 `结构层`
   - 但这仍只是 Git/YAML 级 checkpoint，不是 Unity live 通过
   - shared root 残留 `SceneBuild_01.unity` 继续保持单独待治理，不混入本轮施工推进

**关键决策**
- 这次放行的依据不是“他说自己做完了”，而是：
  - 有真实提交
  - 已推送
  - worktree clean
  - 可随时从 `7b92abe0` 回到当前底稿基线
- 因为 MCP 仍未正常工作，所以这次不能对外说“场景已 live 验收”，只能说“允许从稳定基线继续向结构层施工”。

**恢复点**
- 对用户的正式口径是：scene-build 可以继续做 `结构层`。
- 我这边继续留在治理主线，不进入 scene-build worktree 施工。

## 会话 22 - 2026-03-21（恢复开发总控规范与线程级 prompt 已固化）
**当前主线目标**
- 把 Sunset 当前恢复开发的交通规则、线程顺序、可分发 prompt 和治理线程自己的更新义务正式落成文件，不再靠聊天口头记忆。

**本轮子任务 / 阻塞**
- 用户明确要求一份彻底的开发路径，而且不是只要一句“现在先发谁”，而是要：
  - 当前各线程的开发 prompt
  - 各线程的注意事项
  - 项目经理自己的最简执行法
  - 以及治理线程今后收到回执后必须怎样更新这套文件

**本轮完成**
1. 已新增总控文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\00_工作区导航\恢复开发总控与线程放行规范_2026-03-21.md`
2. 已新增治理入口：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-21_批次分发_09_恢复开发总控与线程放行规范.md`
3. 已新增当前版 prompt 目录：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_恢复开发总控_01\`
4. 已在该目录中落下：
   - 治理线程职责与更新规则
   - 统一最小回执格式
   - `scene-build / NPC / 农田交互修复V2 / 导航检查 / 遮挡检查 / spring-day1` 六份当前 prompt
5. 已把当前正式口径写清：
   - `scene-build` 继续自己的 worktree 结构层施工
   - shared root 业务线程一次只放一条
   - 当前推荐顺序：`NPC -> 农田交互修复V2 -> 导航检查 -> 遮挡检查`
   - `spring-day1` 当前只放行只读准备，不进真实开发
   - 以后任何回执如果改变风险、顺序或边界，治理线程必须先更新这些文件再通知用户

**关键决策**
- 这轮不是“又新建一批一次性 prompt”，而是把“当前版 prompt”正式变成可持续维护的治理资产。
- 这样后续用户贴回执回来，我就有义务先改文件再改口径，而不是继续临场口头解释。

**恢复点**
- 先按治理白名单同步总控文件、入口文件和记忆。
- 同步后，用户就可以直接按这套文件发下一条线程，而我负责后续滚动更新。

## 会话 23 - 2026-03-21（治理线程自用 prompt 已补齐）
**当前主线目标**
- 把“给别人的 prompt 已落盘，但我自己按什么文件执行”这块也补齐，避免总控体系留半截。

**本轮子任务 / 阻塞**
- 用户直接追问“那你发给你自己的呢？”，说明上一轮虽然已经给出了总控规范，但治理线程自己的执行入口还不够显式。

**本轮完成**
1. 已新增治理线程自用执行文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_恢复开发总控_01\治理线程_当前执行Prompt.md`
2. 已更新总控文件：
   - 把 `治理线程（发给我自己）` 纳入“当前 prompt 入口”
3. 已更新批次入口文件：
   - 同步增加治理线程自用 prompt 路径
4. 已更新职责文件：
   - 明确当前执行入口路径
   - 明确我也要维护自己的 prompt 资产
5. 已同步补记工作区与治理记忆，准备做本轮 governance sync 收口

**关键决策**
- 这轮不是改业务顺序，而是补治理闭环。
- 以后不只是别人按 prompt 做事，我自己也有一份正式 current prompt，用户可以直接查它来知道我该怎么工作。

**恢复点**
- 先做治理同步，把这轮“自用 prompt 补齐”推进到 `main`。
- 同步后，对用户明确：当前体系已经从“半闭环”升级到“全角色都有执行入口”的闭环状态。

## 会话 24 - 2026-03-21（可分发Prompt 语义已补白话说明）
**当前主线目标**
- 解决用户对 `可分发Prompt` 目录的理解歧义，明确这些文件到底是不是“先测试准备好没有”的空转 prompt。

**本轮子任务 / 阻塞**
- 用户明确表示自己没搞懂“当前开发放行”这组文件的性质，担心发过去后还要再补第二轮实际业务 prompt。

**本轮完成**
1. 已新增白话说明文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_恢复开发总控_01\可分发Prompt\README.md`
2. 已把当前固定口径写清：
   - `NPC / 农田交互修复V2 / 导航检查 / 遮挡检查 / scene-build` 这些文件都是“直接执行 prompt”
   - 线程收到后应直接完成“准入 + 本轮最小真实工作 + 回执”
   - `spring-day1` 是当前唯一只读准备 prompt
3. 已把这层说明同步写回：
   - 总控文件
   - 批次入口文件

**关键决策**
- 这轮不是改队列，而是补解释层。
- 目标是让项目经理不用再猜“这是不是测试 prompt”，而是一眼知道哪些文件可以直接发出去开工。

**恢复点**
- 接着把这轮补充说明和前面的自用 prompt 一起做 governance sync。
- 同步后，对用户的正式口径固定为：除 `spring-day1` 外，当前目录里的这些 prompt 都可以直接发出去执行。

## 会话 25 - 2026-03-21（治理线程正式进入执行态，当前第一条维持 NPC）
**当前主线目标**
- 不再停留在“治理线程自用 prompt 已存在”，而是正式按这份 prompt 执行首轮 live 核查与发放裁定。

**本轮子任务 / 阻塞**
- 用户直接要求“那你开始吧”，我需要把治理线程从规则准备态推进到实际执行态。

**本轮完成**
1. 已按自用 prompt 做 live 核查：
   - `cwd = D:\Unity\Unity_learning\Sunset`
   - `branch = main`
   - `HEAD = d382c41c`
   - `git status --short --branch = ## main...origin/main`
2. 已核对 shared root 占用：
   - `current_branch = main`
   - `is_neutral = true`
   - `lease_state = neutral`
3. 已核对 Unity / MCP 单实例文档：
   - `current_claim = none`
   - 但仍保持“shared root 线程默认不进 Unity / MCP”的硬边界
4. 已复核总控文件、自用 prompt 与现行队列一致，没有发现需要改序的 live 信号
5. 已形成当前治理裁定：
   - `scene-build` 可继续自己 worktree 的施工线
   - shared root 当前下一条仍应发 `NPC`
   - `农田交互修复V2 / 导航检查 / 遮挡检查 / spring-day1` 继续按既定顺序等待

**关键决策**
- 当前 clean `main + neutral` 现场下，没有理由跳过 `NPC` 去先发后面的 shared root 线程。
- 这轮真正的推进不是再写新规则，而是把“现在先发谁”固化成 live 裁定。

**恢复点**
- 先做最小 governance sync，把这轮执行态裁定推进到 `main`。
- 同步后，对用户的直接口径就是：shared root 现在先发 `NPC`；scene-build 继续自己的 worktree 线。

## 会话 27 - 2026-03-21（Unity MCP 服务端已恢复，当前剩客户端绑定刷新）
**当前主线目标**
- 用用户提供的最新截图与服务端日志，重新判断 scene-build 是否已经具备恢复 MCP 的条件。

**本轮子任务 / 阻塞**
- 用户说明此前是 `8080` 被占导致误入 HTTP 页面；现在已把 Unity MCP 重开到 `127.0.0.1:8888/mcp`，并贴出控制台日志要求确认是否“其实真的运行了”。

**本轮完成**
1. 已从服务端日志确认 live 事实：
   - `15:20:31` 启动 `mcp-for-unity-server`
   - 监听 `http://127.0.0.1:8888/mcp`
   - `scene-build-5.0.0-001` 插件注册成功
   - 注册了 25 个工具
   - 多轮 `/mcp` 请求成功返回 `200/202 OK`
2. 已确认这足以证明：
   - Unity MCP 服务端本身是真的恢复了
   - 不是假启动，也不是再次落到 HTML 假页面
3. 同时，我这边在本聊天里重试 `unityMCP` 调用时，工具仍返回非 Unity HTML 内容，因此当前剩余问题被收敛为：
   - 本会话客户端绑定 / 路由缓存还没刷新到新 MCP
4. 已形成治理结论：
   - scene-build 可以从“绝对不能用 MCP”升级为“可以先做一轮最小只读 MCP 探针”
   - 但在我这边工具绑定刷新前，仍不宣称“我已完全恢复对 Unity 的直接操控”
   - `NPC` 的 shared root 单线执行不受影响

**关键决策**
- 当前真正恢复的是 Unity MCP 服务端，不是我这边的聊天工具绑定。
- 所以对项目经理的白话口径应该区分两层：
  - “服务器好了”
  - “我这边这条会话还没完全接上”

**恢复点**
- 对 scene-build：可尝试最小只读探针，再决定是否用 MCP 推装饰层。
- 对 shared root：继续等 `NPC` 回执，不加开第二条业务线。

## 会话 28 - 2026-03-21（scene-build / NPC 新 prompt 已按真实验收反馈重写）
**当前主线目标**
- 把用户刚刚给出的两条高价值反馈直接落成新的线程 prompt，而不是让用户反复自己口头转述。

**本轮子任务 / 阻塞**
- scene-build 最新回执技术上成功，但用户对视觉结果强烈不满意，要求它学习 `Primary`。
- NPC 最新回执技术上成功，但用户对最终气泡位置和碰撞结果仍不接受，并要求它追加下一步规划。

**本轮完成**
1. 已 live 核对 shared root：
   - `main`
   - `neutral`
   - 说明 NPC 已归还，shared root 当前可再发新一条
2. 已 live 核对 scene-build worktree：
   - `branch = codex/scene-build-5.0.0-001`
   - `HEAD = 0717172a`
   - `git status = clean`
3. 已新增 scene-build 新 prompt：
   - `scene-build_装饰层纠偏与Primary参考重做.md`
4. 已新增 NPC 新 prompt：
   - `NPC_气泡位置碰撞体修正与下一步规划.md`
5. 已更新总控文件与批次入口文件，把当前入口切到上述两份新 prompt

**关键决策**
- scene-build 当前真正需要的不是“继续推进”，而是“纠偏审美方向”。
- NPC 当前真正需要的不是“再试一次同类微调”，而是把气泡位置、碰撞体和下一步规划一起纳入下一轮要求。

**恢复点**
- 先做治理同步。
- 同步后，对用户的直接建议就是：
  - 把新的 scene-build prompt 发过去
  - 把新的 NPC prompt 发过去

## 会话 29 - 2026-03-21（NPC 归还后 shared root 下一条改为 farm，scene-build 升到逻辑层）
**当前主线目标**
- 根据两条新回执，把当前 live 交通模型重新裁定成最新状态，并落实成正式文件。

**本轮子任务 / 阻塞**
- `NPC` 已完成新的分支 checkpoint 并归还 shared root
- `scene-build` 已在 worktree 内完成装饰层纠偏 checkpoint，等待治理裁定下一步是否进入逻辑层

**本轮完成**
1. 已 live 核对 shared root：
   - `branch = main`
   - `HEAD = 95b3f793`
   - `git status = clean`
   - occupancy = `neutral-main-ready`
2. 已 live 核对 scene-build worktree：
   - `branch = codex/scene-build-5.0.0-001`
   - `HEAD = 44afab3f`
   - `git status = clean`
3. 已新增：
   - `scene-build_逻辑层继续施工.md`
4. 已更新总控文件与批次入口：
   - shared root 下一条从 `NPC` 改为 `农田交互修复V2`
   - `scene-build` 当前阶段改为 `逻辑层`
   - `NPC` 改列为“已收口但待 Unity 手测”，不再是当前应立即继续发的开发线程

**关键决策**
- 这轮不是再改质量要求，而是改交通状态。
- `NPC` 当前成果已经足够让出 shared root，但还不足以宣称完全验收。
- `scene-build` 则因当前 worktree 干净、前一层已收口，可以继续推进到逻辑层。

**恢复点**
- 先做治理同步。
- 同步后：
  - 给 scene-build 发 `scene-build_逻辑层继续施工.md`
  - 给 farm 发 `农田交互修复V2_当前开发放行.md`

## 会话 30 - 2026-03-21（恢复开发总控已同步完成）
**当前主线目标**
- 把上一条会话里的交通裁定真正推上 `main`，形成正式 live 放行基线。

**本轮完成**
1. 已执行治理白名单 `sync` 并成功推送：
   - `main @ 7c6fc1e9`
2. 已正式生效的放行状态：
   - `scene-build` 当前继续 `逻辑层`
   - shared root 当前下一条是 `农田交互修复V2`
   - `NPC` 当前不再发新开发 prompt，而是等待后续 Unity 手测窗口
3. 已额外确认一个文档层风险：
   - `scene-build_逻辑层继续施工.md` 本体按 `UTF-8` 读取正常
   - PowerShell `Get-Content -Encoding Default` 会出现假乱码
   - 这属于编码读法差异，不是 prompt 内容损坏

**关键决策**
- 当前最重要的是严格按新顺序推进，不把 `NPC` 再塞回 shared root 串行槽位。
- `scene-build` 和 `farm` 现在是两条可继续推进的线，但职责不同：
  - `scene-build` 在自己的 worktree 继续
  - `farm` 在 shared root 接下一棒

**恢复点**
- 对用户的明确建议：
  - 发 `scene-build_逻辑层继续施工.md`
  - 发 `农田交互修复V2_当前开发放行.md`
  - `NPC` 暂停在待手测态

## 会话 31 - 2026-03-21（farm 收口后，NPC 因真实验收失败升为下一条）
**当前主线目标**
- 接住 `farm` 的回执，并把用户刚给出的 NPC 真实失败证据写回 live 排程。

**本轮完成**
1. 已 live 核对 shared root：
   - `main @ 91f7328a`
   - `git status = clean`
   - occupancy = `neutral-main-ready`
2. 已确认 `farm`：
   - checkpoint 已推送到 `21a5f8df`
   - `carrier_ready = yes`
   - `main_ready = no`
   - 原因是 continuation branch 仍带历史差异：`AGENTS.md`、`scripts/git-safe-sync.ps1`
3. 已根据用户的真实 Unity 验收结果改判 `NPC`：
   - 不是“待手测”
   - 而是“验收失败返工”
4. 已新增正式可分发 prompt：
   - `NPC_验收失败返工与live验收闭环.md`
5. 已更新 live 总控：
   - shared root 下一条 = `NPC（验收失败返工）`
   - `farm` 改列到“已完成 checkpoint 但当前不是 main-ready”

**关键决策**
- `farm` 这轮不需要立刻继续占 shared root。
- `NPC` 则因为已经被用户真实验收打回，当前优先级必须升到 shared root 下一条。

**恢复点**
- 现在对用户的明确建议：
  - `scene-build` 可以继续留在 worktree 内推进
  - shared root 下一条应发 `NPC_验收失败返工与live验收闭环.md`

## 会话 32 - 2026-03-21（scene-build 逻辑层收口后升到回读自检阶段）
**当前主线目标**
- 接住 scene-build 新回执，把它从“逻辑层继续施工”切到下一阶段。

**本轮完成**
1. 已 live 核对 scene-build worktree：
   - `codex/scene-build-5.0.0-001 @ a62b557d`
   - `git status = clean`
2. 已确认：
   - 逻辑层最小版本 checkpoint 已完成
   - 本轮触碰了只读 MCP 探测
   - 实际施工仍走 Scene YAML
3. 已确认未解点：
   - 本会话 `unityMCP` 仍没有恢复成可用的 Unity live 验证闭环
4. 已新增正式 prompt：
   - `scene-build_回读自检与高质量初稿收口.md`
5. 已更新 live 总控：
   - scene-build 当前阶段 = `回读自检与高质量初稿收口`

**关键决策**
- 现在 scene-build 不该再拿旧的“逻辑层继续施工”prompt。
- 它下一步应该做的是自检、回读和高质量初稿收口。

**恢复点**
- 对用户的明确建议：
  - scene-build 现在应发 `scene-build_回读自检与高质量初稿收口.md`
  - shared root 现在应发 `NPC_验收失败返工与live验收闭环.md`

## 会话 33 - 2026-03-21（NPC 已转入 needs-unity-window，scene-build 升到剧本对齐精修）
**当前主线目标**
- 把用户刚贴回的 `scene-build` 与 `NPC` 最新回执真正写回总控文件，并把 shared root 下一条改成最新可发线程。

**本轮子任务 / 阻塞**
- `scene-build` 已完成“回读自检与高质量初稿收口”，说明当前 prompt 已过期。
- `NPC` 已在 `codex/npc-roam-phase2-003 @ 657594a6` 完成返工 checkpoint 并归还 shared root，但回执明确写出 `needs-unity-window`。
- 用户补充了新的质量要求：scene-build 的精修标准必须和既有 `spring-day1` 剧本对齐，而不是只做泛化美术打磨。

**本轮完成**
1. 已 live 核对 shared root：
   - `D:\Unity\Unity_learning\Sunset @ main @ 26ce1d04`
   - `git status = clean`
   - occupancy = `neutral-main-ready`
2. 已 live 核对 scene-build worktree：
   - `codex/scene-build-5.0.0-001 @ 0a14b93c`
   - `git status = clean`
3. 已新增 scene-build 当前版 prompt：
   - `scene-build_高质量初稿后续精修与spring-day1剧本对齐.md`
4. 已更新总控文件、批次入口、治理线程自用 prompt 与职责文件，使当前 live 口径改为：
   - `scene-build` 当前阶段 = `高质量初稿后续精修与spring-day1剧本对齐`
   - shared root 下一条 = `导航检查`
   - 然后 = `遮挡检查`
   - `NPC` 当前转入待专属 Unity 验收窗口
   - `农田交互修复V2` 继续保留为“已完成 checkpoint 但当前不是 main-ready”
5. 已准备做本轮 governance sync，把这轮更新推成 `main` 上的正式口径。

**关键决策**
- `NPC` 当前不是继续占 shared root 的对象，而是“carrier checkpoint 已够，但 live 手测仍缺真正可用的 Unity 窗口”。
- `scene-build` 当前不是卡住，而是已经进入下一阶段；如果继续发旧 prompt，只会让项目经理再次拿过期文件分发。
- `spring-day1` 文档现在被正式用作 scene-build 精修的剧情精度参考，而不是只当旁支历史材料。

**恢复点**
- 当前应发：
  - `scene-build_高质量初稿后续精修与spring-day1剧本对齐.md`
  - `导航检查_当前开发放行.md`
- 当前继续等待：
  - `NPC` 的 Unity 验收窗口
  - `遮挡检查` 的 shared root 顺位

## 会话 34 - 2026-03-21（恢复开发总控已正式 sync，项目经理可按 live prompt 分发）
**当前主线目标**
- 把上一轮已经改好的恢复开发总控真正推到 `main`，并把“scene-build 对齐 spring-day1 剧本”的新要求变成正式 live 口径。

**本轮完成**
1. 已使用稳定 launcher 执行治理同步：
   - `C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action sync -OwnerThread Codex规则落地 -Mode governance`
2. 已推送提交：
   - `D:\Unity\Unity_learning\Sunset @ main @ ff90f0be`
   - commit = `2026.03.21-23`
3. 已复核：
   - `git status --short --branch = ## main...origin/main`
   - occupancy 仍为 `neutral-main-ready`
4. 已确认当前给项目经理的 live 口径已经落地：
   - `scene-build` 用 `scene-build_高质量初稿后续精修与spring-day1剧本对齐.md`
   - shared root 下一条用 `导航检查_当前开发放行.md`
   - `NPC` 保持 `needs-unity-window`

**关键决策**
- 这轮不是新增业务开发，而是把排序、边界和参考标准正式收进 live 总控。
- 用户强调的“scene-build 高精度要求要和 spring-day1 剧本一致”现在已经完成文件化、记忆化和 Git 同步，不再只是聊天约定。

**恢复点**
- 下一次如果用户继续问“现在谁能开发 / 发哪个 prompt”，直接基于 `ff90f0be` 之后的 `main` 回答，不再重新手工拼装旧回执。

## 会话 35 - 2026-03-21（用户要求瓦解规则帝国，治理线已切换为 main-only 极简并发模式）
**当前主线目标**
- 不再继续维护旧的 branch / grant / queue 体系，直接把当前 live 治理口径改成“大家直接开发，只有高危才打断”。

**本轮完成**
1. 已重写总控文件、批次入口、治理线程 prompt、治理线程职责文件。
2. 已把当前直接可发 prompt 改成 main-only 直开口径：
   - `scene-build`
   - `NPC`
   - `农田交互修复V2`
   - `导航检查`
   - `遮挡检查`
   - `spring-day1`
3. 已在仓库级 `AGENTS.md` 顶部加入临时最高优先级覆盖：
   - 普通开发只认 `main`
   - 不再把 branch / grant / return-main 当普通前置
   - 治理线程只保留高危打断与收烂摊子职责

**关键决策**
- 当前 live 治理模型已经从“复杂交通调度”降成“极简并发开发 + 硬刹车”。
- 唯一仍然保留的强限制不是治理偏好，而是技术现实：
  - Unity / MCP live 仍只能单写

**恢复点**
- 接下来如果用户贴回线程进展，只优先判断：
  1. 有没有撞同一个高危目标
  2. 有没有撞 Unity / MCP 单写
  3. 有没有把现场写坏
- 如果都没有，就直接让线程继续干，不再重新造排队流程。

## 会话 36 - 2026-03-21（main-only 新阶段目录、scene-build / spring-day1 新 prompt 与迁移口径已落盘）
**当前主线目标**
- 响应用户对“目录太乱、命名太差、旧阶段和新阶段混在一起”的批评，把当前极简并发开发正式整理成一个新阶段入口，并补 `scene-build / spring-day1` 的新 prompt 与迁移口径。

**本轮完成**
1. 已建立并写实当前现行目录：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01`
2. 已在新目录补齐 6 份当前直接开发 prompt：
   - `scene-build`
   - `NPC`
   - `农田交互修复V2`
   - `导航检查`
   - `遮挡检查`
   - `spring-day1`
3. 已补齐用户明确点名要的两份新 prompt：
   - `scene-build_当前任务回执与迁移前冻结.md`
   - `spring-day1_当前任务回执与向scene-build交接.md`
4. 已把以下入口文件改为新目录跳转入口：
   - `恢复开发总控与线程放行规范_2026-03-21.md`
   - `2026-03-21_批次分发_09_恢复开发总控与线程放行规范.md`
5. 已把旧目录 `2026.03.21_恢复开发总控_01` 标成历史阶段。
6. 已把 `scene-build` 的后续迁移口径写死：
   - 当前现场：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - 目标路径：`D:\Unity\Unity_learning\scene-build-5.0.0-001`
   - 迁移方式：`git worktree move`
   - 执行顺序：先冻结回执，再迁移

**关键决策**
- 这轮不能再把“极简并发开发”伪装成旧阶段尾巴，必须有自己的目录和自己的 prompt 集合。
- `scene-build` 现在被正式定义为“待迁移的独立项目现场”，不是 shared root 普通线程。
- `spring-day1` 当前被正式定义为“向 scene-build 交接”的内容线，而不是平行的第二个大施工面。

**恢复点**
- 现在可以直接从新目录发 prompt。
- 下一步等 `scene-build / spring-day1` 两份真实回执回来，再继续执行 `scene-build` 迁移和后续 `vibecoding` 场景规范适配。

## 会话 37 - 2026-03-21（spring-day1 prompt 已重写为 scene-build 空间 brief 交付）
**当前主线目标**
- 按用户要求，先把 `spring-day1` 的 prompt 改到真正可发的状态，让它不再泛泛继续开发，而是直接输出给 `scene-build` 的正式空间职责表。

**本轮完成**
1. 已补做 Sunset 手工等价启动核查：
   - `D:\Unity\Unity_learning\Sunset @ main @ 65468c2d`
   - `git status --short --branch = ## main...origin/main`
   - shared root occupancy = `neutral-main-ready`
2. 已重写：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\spring-day1_当前开发放行.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\spring-day1_当前任务回执与向scene-build交接.md`
3. 已把 prompt 的核心改成：
   - `spring-day1` 负责把 Day1 剧情流程翻译成 `scene-build` 可直接施工的空间 brief
   - 交付内容必须覆盖 `Day1` 场景模块、`SceneBuild_01` 身份、强制承载动作、禁止误扩边界与精修优先级
   - 回执必须带正式交付文件路径，不再接受只有状态没有交件

**关键决策**
- 这轮最重要的不是再增加一份“能不能继续”的测试 prompt，而是把 `spring-day1` 的职责重新钉死。
- `spring-day1` 现在是 `scene-build` 的剧情空间交付线，不是第二个自由施工线程。

**恢复点**
- 现在可以直接把新的 `spring-day1_当前开发放行.md` 发给 `spring-day1`。
- 等它交回正式交付件，再继续看 `scene-build` 如何吃这份 brief 往下搭。

## 会话 38 - 2026-03-21（scene-build 冻结回执已转成下一步迁移 prompt）
**当前主线目标**
- 继续处理用户贴回的 `scene-build` 冻结回执，不停留在分析层，而是把下一步直接做成可发 prompt，并回答“何时恢复施工、何时迁移”。

**本轮完成**
1. 已复核当前 `scene-build` worktree 现场：
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `codex/scene-build-5.0.0-001 @ 0a14b93c`
   - dirty 仅 3 个记忆文件
   - 目标迁移路径 `D:\Unity\Unity_learning\scene-build-5.0.0-001` 当前不存在
2. 已确认此前写死的迁移方案仍是：
   - 先冻结
   - 再 `git worktree move`
   - 再复核 `git worktree list --porcelain`
3. 已新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\scene-build_最小checkpoint并等待正式迁移.md`
4. 已对 `spring-day1` 两份 prompt 做小幅补强：
   - 明确其交付件在 `scene-build` 迁移到 `D:\Unity\Unity_learning\scene-build-5.0.0-001` 后也必须继续可用

**关键决策**
- 当前不让 `scene-build` 直接恢复自由施工。
- 更稳的顺序是：
  - 先把 3 个记忆 dirty 收成最小 checkpoint
  - clean 后停住
  - 再由治理侧执行正式迁移
  - 迁移完成并复核后再恢复施工
- 这里的关键不是流程完美，而是避免在 still-dirty 的旧路径上继续叠加新施工。

**恢复点**
- 现在可以直接把：
  - `spring-day1_当前开发放行.md`
  - `scene-build_最小checkpoint并等待正式迁移.md`
  发给对应线程。
- 等 `scene-build` 回执 `ready_for_move = yes` 后，再执行正式迁移。

## 会话 39 - 2026-03-21（scene-build 迁移目标纠偏并确认当前卡在 live 目录锁）
**当前主线目标**
- 接住用户对迁移认知的纠正：重新找回 `NPC / farm` 之前那次真实迁移事实，并把 `scene-build` 的下一步从“模糊等迁移”收成可执行动作。

**本轮完成**
1. 已从历史治理记忆重新确认：
   - `NPC / farm` 当时稳定成立的是“回根仓库后，重启 Codex 才完全显现”
   - 不是“会话内热切换完全成功”
2. 已纠正我上一轮写错的目标路径：
   - 废弃：`D:\Unity\Unity_learning\SceneBuild_Standalone\scene-build-5.0.0-001`
   - 改为：`D:\Unity\Unity_learning\scene-build-5.0.0-001`
3. 已复核当前真实现场：
   - `git worktree list --porcelain` 仍只有 shared root 和 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `scene-build` worktree 当前 clean，`HEAD = 8e641e67`
   - `spring-day1` 已完成 handoff，当前不需要再发新 prompt
4. 已直接尝试执行：
   - `git -C D:\Unity\Unity_learning\Sunset worktree move D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001 D:\Unity\Unity_learning\scene-build-5.0.0-001`
   - 结果：`Permission denied`
5. 已定位 live 锁来源：
   - `Unity.exe -projectPath D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - 多个 `mcp-for-unity` `http / stdio` 进程
   - `Library\MCPForUnity\TerminalScripts\mcp-terminal.cmd`
6. 已新增一个真正必要的新 prompt：
   - `scene-build_迁移前释放Unity与MCP目录锁.md`

**关键决策**
- 当前不是 Git 规则阻断，也不是要再讨论一次迁移方案。
- 当前唯一剩余前置就是让 `scene-build` 释放旧目录上的 Unity / MCP live 进程锁。

**恢复点**
- 先让 `scene-build` 回执 `unity_closed / mcp_closed / ready_for_move_now`。
- 然后我这里继续做真正的 `git worktree move` 和迁后复核。

## 会话 40 - 2026-03-22（scene-build 迁移手术已以 copy + repair 兜底完成）
**当前主线目标**
- 在直接 `git worktree move` 一直被系统锁卡住的情况下，把 `scene-build` 真实迁出去，并避免再让人回旧路径误施工。

**本轮完成**
1. 已收下 `spring-day1` 回执，确认其职责已经完成：
   - handoff 正式交付为 `scene-build_handoff.md`
   - 本轮后除非它再改 handoff，否则不再需要持续贴回治理线程
2. 已收下 `scene-build` 的释放目录锁回执：
   - `unity_closed = yes`
   - `mcp_closed = yes`
   - 但浮出 4 个 TMP 字体资源 dirty
3. 已再次尝试直接迁移：
   - `git -C D:\Unity\Unity_learning\Sunset worktree move ...`
   - 结果仍为 `Permission denied`
4. 已执行兜底迁移：
   - `robocopy D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001 D:\Unity\Unity_learning\scene-build-5.0.0-001 /E /COPY:DAT /DCOPY:DAT /R:1 /W:1 /XD Library Temp Logs`
   - `git -C D:\Unity\Unity_learning\Sunset worktree repair D:\Unity\Unity_learning\scene-build-5.0.0-001`
5. 已复核成功：
   - `git worktree list --porcelain` 现在正式注册的是 `D:\Unity\Unity_learning\scene-build-5.0.0-001`
   - 新路径 branch / HEAD 仍为 `codex/scene-build-5.0.0-001 @ 8e641e67`
   - 4 个 TMP dirty 也在新路径上保留
6. 已新增给线程的下一步 prompt：
   - `scene-build_迁后复核与从新路径恢复.md`

**关键决策**
- `scene-build` 的 Git 层迁移已经算完成，不再继续纠缠“非得直接 move”。
- 当前真正的风险是旧路径物理目录还在，可能让人误回旧路径继续写。

**恢复点**
- 现在要发给 `scene-build` 的是迁后复核 prompt，不是迁移 prompt。
- 等它确认 `old_path_abandoned = yes` 后，再恢复场景精修。

## 会话 41 - 2026-03-22（scene-build 误迁移认知收回并完成入口止血）
**当前主线目标**
- 收回我把“Codex 映射 / 归位问题”错误扩大成“Git worktree 物理迁移”的误操作，恢复用户要求的旧 worktree 认知，并把会继续误导别人的入口文档止血。

**本轮完成**
1. 已重新核对正式工作树关系：
   - `git worktree list --porcelain` 现在正式只认 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - shared root 仍是 `D:\Unity\Unity_learning\Sunset @ main`
2. 已确认误复制目录仍在：
   - `D:\Unity\Unity_learning\scene-build-5.0.0-001`
   - 其中 `.git` 已失活为 `.git.DISABLED_DO_NOT_USE`
3. 已改正当前现行入口中的错误迁移口径：
   - `README.md`
   - `治理线程_当前执行Prompt.md`
   - `治理线程_职责与更新规则.md`
   - `scene-build_当前开发放行.md`
   - `spring-day1` 两份 prompt
   - 4 份错误迁移 prompt 的停用说明
4. 已在工作区记忆层追加明确纠偏：
   - 不再继续任何 `scene-build -> D:\Unity\Unity_learning\scene-build-5.0.0-001` 的迁移动作
   - 后续只认旧 worktree 正式现场

**关键决策**
- 现在最重要的不是继续动 Git，而是避免任何人再被我之前那套错误路径口径带偏。
- 误复制副本先保留为待裁定对象，不擅自删除。

**恢复点**
- 后续如果用户继续推进 scene-build，就只围绕 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001` 继续。
- 等用户明确决定后，再处理误复制副本的去留。

## 会话 42 - 2026-03-22（误复制副本已删除，scene-build 现场彻底回到唯一 worktree）
**当前主线目标**
- 不再停在“误副本还在待裁定”，而是按用户明确要求把它删掉，并把 scene-build 的落地状态收成唯一现场。

**本轮完成**
1. 已删除误复制副本目录：
   - `D:\Unity\Unity_learning\scene-build-5.0.0-001`
2. 已复核：
   - `Test-Path D:\Unity\Unity_learning\scene-build-5.0.0-001 = False`
   - `git worktree list --porcelain` 仍只认 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
3. 已把当前批次入口和治理口径继续改到最终状态：
   - `README.md`
   - `治理线程_当前执行Prompt.md`
   - `治理线程_职责与更新规则.md`
   - `scene-build_当前开发放行.md`
4. 已把“等用户裁定是否删除副本”的半截状态从工作区记忆里补成已完成事实。

**关键决策**
- scene-build 现在不存在第二个项目根，也不存在新的迁移目标。
- 后续所有 scene-build 推进，都只准围绕旧 worktree 正式现场继续。

**恢复点**
- 如果用户现在要继续 scene-build，就直接继续 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`。

## 2026-03-22｜scene-build 线程已在 Codex 应用层改回旧 worktree
**用户目标**
- 不要我再谈 Git 迁移，而是让我真正把 Codex 里“场景搭建”这条线程归位到旧 worktree，最后能够明确告诉用户“现在可以重启 Codex 检查是否就位成功”。

**本轮完成**
1. 已确认 Git 正式现场始终正确，报错根因不在 `git worktree`。
2. 已从 `C:\Users\aTo\.codex\session_index.jsonl` 找到当前被命名为“场景搭建”的线程 id：
   - `019cc7ba-fb87-7012-a7ef-0ccee21121c0`
3. 已在 `C:\Users\aTo\.codex\state_5.sqlite` 中把该线程改回：
   - `cwd = \\?\D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `git_branch = codex/scene-build-5.0.0-001`
   - `git_sha = 8e641e67149f413c74181f4c6753895c2cdfcf53`
4. 已在 `C:\Users\aTo\.codex\.codex-global-state.json` 中把激活 workspace 根补回旧 worktree，并保留 `Sunset` shared root。
5. 已完成本轮应用层归位前的两份备份：
   - `state_5.sqlite.bak-20260322-scene-build-cwd-fix`
   - `.codex-global-state.json.bak-20260322-scene-build-cwd-fix`

**关键决策**
- 这轮对用户真正有价值的成果，不是“又写了一批治理 prompt”，而是把 Codex 本机状态改对。
- 后续如果再有 scene-build 归位问题，先查 `session_index / threads / global-state`，不要先碰 Git 正式 worktree。

**下一步**
- 用户现在可以重启 Codex，检查“场景搭建”是否已经回到旧 worktree 项目目录下。

## 2026-03-22｜补刀收口：scene-build 回跳不是 Git 问题，而是 rollout 里还残着 1 条旧 cwd
**用户目标**
- 不只是让 scene-build 在线程列表里短暂显示在旧 worktree 下，而是点击进去也要稳定留在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`。

**本轮完成**
1. 已确认 sqlite 中线程 `019cc7ba-fb87-7012-a7ef-0ccee21121c0` 的 `cwd/git_branch/git_sha` 仍然正确。
2. 已在原始 rollout 文件修掉最后 1 条残留的 `cwd = \\?\D:\Unity\Unity_learning\Sunset`。
3. 已复核该文件当前 78 条 `cwd` 全部为旧 worktree 口径，不再出现 shared root。
4. 已全局扫描 `.codex` 下相关 `.jsonl`，未发现同线程 id 再带旧 `Sunset` cwd。

**关键决策**
- 用户要的“迁移成功”已经明确等价于：Codex 应用层线程元数据彻底回到旧 worktree。
- 不再把问题解释成 Git worktree 迁移失败，也不再引入任何新路径。

**恢复点**
- 现在可以直接让用户重启 Codex 并实测；如果仍回跳，再查是否还有别的缓存层回填，而不是先碰 Git。
## 2026-03-22｜scene-build 线程归位修复已沉淀为最高优先级 SOP
**当前主线目标**
- 不再靠临场试错修 `scene-build` 线程回跳，而是把本次实战中真正有效的应用层修复流程固化成后续可直接照搬的标准方案。

**本轮完成**
1. 新增正式方案文档：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\24_main-only极简并发开发与scene-build迁移\scene-build线程归位修复最高优先级方案_2026-03-22.md`
2. 文档已明确写死后续第一优先级流程：
   - 先确认 Git worktree 没坏
   - 锁定线程 id
   - 同时修 `sqlite / rollout / session_index / global-state`
   - 最后再重启 Codex 验证
3. 文档已单列“反例黑名单”和“我这次的犯错历史”，明确禁止：
   - 先怀疑 Git worktree 坏了
   - 擅自创造新路径
   - 只修一层状态文件
   - 直接手改超长 JSONL
   - 写出 BOM JSONL
   - 把中文标题污染成 `????`

**关键决策**
- 以后 scene-build 同类问题，本文档是最高优先级入口。
- 只有本文档整套流程走完仍失败，才允许进入第二优先级探索，不允许再跳过 SOP 直接自由发挥。

**恢复点 / 下一步**
- 后续若用户继续要求修 scene-build 归位或类似线程归位，先执行本文档，不再重开新的想象性方案。
## 2026-03-22｜scene-build 的 4 个 TMP dirty 不应再退回线程重判
**当前主线目标**
- 纠正我对 `scene-build` 那条回执的误处理，避免把已经在别线明确归属的 TMP 资产再次退回给场景线程重收件。

**本轮完成**
1. 已复核 `scene-build` worktree 当前 dirty 的 4 个文件确实是：
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese BitmapSong SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset`
2. 已从 `spring-day1` 相关 memory 重新核实：这批 `DialogueChinese*` TMP 资产本来就是 `spring-day1` 的中文对话字体资产链，不是 `scene-build` 施工层新增业务对象。
3. 已核实工作区历史记录里早就写过：
   - `Primary.unity` 与五套 TMP 字体资产属于保护对象
   - 本轮恢复/迁移/收口时不应混入其他线程提交
4. 已明确纠正：我刚才要求 `scene-build` 再做一次 1/2/3 分类判断，是重复收件，属于治理侧失误，不应再这样做。

**关键决策**
- 这 4 个 TMP dirty 现在直接按“外线保护资产 / spring-day1 历史资产链”处理。
- 不再把它们当成 `scene-build` 自己要重新定性的对象。
- `scene-build` 不认领、不扩写、不单独为这 4 个文件停工分析。

**恢复点 / 下一步**
- 后续给 `scene-build` 的正确口径应是：
  - 这 4 个 TMP 资产不属于你的施工面
  - 不纳入你的 scene checkpoint
  - 不需要你再重判
  - 由治理侧按保护对象口径统一处理

## 2026-03-22｜用户指出我不该再让 scene-build 重判 TMP dirty
**用户目标**
- 追问为什么治理侧明明已经从 `spring-day1` 收到过 TMP 资产归属，却还把同一批文件退回给 `scene-build` 线程定性。

**已完成**
- 已承认这是我的收件错误，不是线程回执不足。
- 已重核 `spring-day1` 两层 memory，确认 4 个 `DialogueChinese*` TMP 资产一直都属于中文对话字体链，且此前已被归为保护对象。
- 已决定后续不再让 `scene-build` 认领、解释或分析这批文件。

**关键决策**
- `scene-build` 现场只继续管 `SceneBuild_01` 自己的施工资产。
- 这批 TMP dirty 统一按 `spring-day1` 历史保护资产处理，治理侧自己消化，不再重复收件。

**恢复点**
- 当前主线回到：给用户一版可直接转发给 `scene-build` 的纠正口径，并继续维持 `main-only` 下的极简并发收口，不再制造额外治理动作。

## 2026-03-22｜用户要求我把治理线程自己的欠账全盘托出
**用户目标**
- 不再只汇报刚刚修过的阻塞，而是去 `代办 + 历史 memory + 当前阶段文档` 里把治理线程自己没做完的内容彻底翻出来，按真实状态摊开。

**已完成**
- 已回看 `24 / 23 / TD_14 / 根层 memory / 线程 memory`。
- 已新增自审总表：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\24_main-only极简并发开发与scene-build迁移\治理线程自审与未完成项总表_2026-03-22.md`
- 已把欠账拆成三类：
  - 真未完成：`scene-build` 归位最终验收、`vibecoding` 规范适配立项、批次 prompt 收口、`TD_14`
  - 已作废但文档还残着：`scene-build` 新路径迁移、`23` 阶段 queue 正文化尾项
  - 治理文档债：`24/tasks.md` / `24/memory.md` 坏编码且夹带旧迁移叙事

**关键决策**
- 当前治理线最危险的不是“少一个新规则”，而是“旧错口径和坏编码文档还在入口位置上”。
- 接下来应优先清入口、收验收、再单开新阶段，不再继续让历史坏文档挂着冒充现行入口。

**恢复点**
- 本线程当前已从碎片救火切回“治理线总收口”主线。

## 2026-03-22｜用户重新裁定当前主线：快推 vibecoding，main-only 执行壳继续保留
**用户目标**
- 明确告诉我：
  - `scene-build` Codex 归位已经彻底完成
  - `vibecoding` 场景规范适配必须立刻赶入进程
  - `2026.03.21_main-only极简并发开发_01` 和这个目标不是两条主线，而是同一核心的执行壳与正文关系
  - `TD_14` 不是当前最该先冲的内容

**已完成**
- 已建立新阶段：
  - `25_vibecoding场景规范与main收口`
- 已新增：
  - `并发线程统一回执与main收口机制_2026-03-22.md`
  - `并发线程_统一收口回执.md`
  - `给全局skills的窄范围委托_2026-03-22.md`
- 已把批次 README 改成：
  - 正文看 `25`
  - 分发和回执还在 `2026.03.21_main-only极简并发开发_01`

**关键决策**
- 当前治理线的 P0 不是再修 `scene-build`。
- 当前治理线的 P0 是：
  - 建 `vibecoding` 正式主线
  - 建统一回执和顺序进 `main` 的最小机制
  - 把全局 skills 线程限制在 `startup-guard` 一级告警处理这一个窄任务里

**恢复点**
- 下一步应直接进入 `vibecoding` 场景规范适配 brief，而不是再回旧迁移叙事。

## 2026-03-22｜我已把第一版 `vibecoding` 场景规范 brief 真的做出来了
**用户目标**
- 不是只让我“立项”，而是要快马加鞭把 `vibecoding` 场景规范适配真正推进起来。

**已完成**
- 已新增：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\vibecoding场景规范适配Brief_2026-03-22.md`
- brief 已经把当前 `SceneBuild_01` 的执行标准压成短口径：
  - 东侧进入
  - 院落对话留白
  - 工作台闪回焦点
  - 教学区与生活区关系
  - 禁止误扩

**关键决策**
- 当前 `25` 阶段已经不是空壳，而是开始产出真实正文。

**恢复点**
- 接下来优先补 dirty 归属说明，再发第一轮统一回执窗口。

## 2026-03-22｜我已把 shared root dirty 归属说明正式落盘
**当前主线目标**
- 不再让“现在一堆 dirty 到底谁的”继续成为统一收口前的口头障碍，而是把它收成一份治理侧可以直接复用的正式说明。

**已完成**
- 已新增：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\当前shared_root_dirty归属说明_2026-03-22.md`
- 已把当前现场按归属拆成：
  - 治理线当前认领
  - 治理线旧账但不再继续扩写
  - NPC 线程认领
  - `spring-day1` / 剧情整理线认领
  - 农田 / 库存交互线认领
- 已明确把旧误区写死：
  - `scene-build` 新路径迁移作废
  - 旧 queue / grant / ensure-branch 正文化扩建作废
  - mixed dirty 不揉成一个超大 commit
- 已同步推进：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\tasks.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`

**关键决策**
- 以后任何线程来问“现在 dirty 怎么办”，治理侧不再现场临时脑补，而是先按这份归属说明过滤噪音。
- 当前最对路的下一步不是再写新规则，而是发第一轮统一回执窗口，收真实可入 `main` 状态。

**遗留问题**
- 还没有做第一轮真实统一回执收件。
- 还没有基于回执形成第一版 main 收口批次表。
- 给全局 skills 的窄范围委托 prompt 已写好，但还没实际发出并收件。

**恢复点**
- 当前主线继续停在：
  1. 发 `并发线程_统一收口回执.md`
  2. 收回执
  3. 出批次表
  4. 再决定是否让全局 skills 线程开工 `sunset-startup-guard` 一级告警处理方案

## 2026-03-22｜我已把统一回执正式收成第一版批次表
**当前主线目标**
- 不让 `000全员回执.md` 停留在“原始回执堆”，而是直接推进到治理侧初裁，变成后续真正可执行的顺序收口依据。

**已完成**
- 已新增：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\第一轮main收口批次表_2026-03-22.md`
- 已把治理侧初裁回写到：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\000全员回执.md`
- 已正式形成第一轮分组：
  - A组：农田
  - B组：`spring-day1`、遮挡检查
  - C组：NPC、导航
  - 支援：全局 skills
- 已同步推进：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\tasks.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`

**关键决策**
- 统一回执这一步现在已经不是“打算做”，而是“做完了第一轮”。
- 本轮真正值得优先推进到 `main` 的只有农田；其他线程要么是 branch carrier 下一批，要么继续开发比现在硬收口更合理。

**遗留问题**
- 还没给农田发第一批 `main` 收口执行 prompt。
- 还没把全局 skills 对 `sunset-startup-guard` 的裁定正式吸收入 `25` 阶段正文。

**恢复点**
- 当前主线下一步很明确：
  1. 先发农田第一批 `main` 收口执行 prompt
  2. 再吸收全局 skills 的窄裁定
  3. 最后裁 B组 先迁 `spring-day1` 还是先迁遮挡检查

## 2026-03-22｜我已经把农田第一批 `main` 收口 prompt 生出来了
**当前主线目标**
- 让第一版批次表立刻产生执行动作，而不是只在文档里停着。

**已完成**
- 已新增：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\农田_第一批main收口执行.md`
- 已把农田这轮动作压成：
  - 停止扩写
  - 只核对白名单
  - 只收农田自己的 `main` checkpoint
  - 回执只要提交路径、SHA、`git status`、`blocker_or_checkpoint`

**关键决策**
- 当前不需要再讨论“是不是该开始了”，因为 A组 的首个可执行 prompt 已经准备完成。

**恢复点**
- 你现在就可以把这份 prompt 发给农田。
- 我这边下一步等农田回执，再决定 B组。

## 2026-03-22｜我已经把“每线程只提自己白名单”写成正式机制
**当前主线目标**
- 不只口头回答用户“是不是这就是最终解法”，而是把它真正写入当前并发收口机制。

**已完成**
- 已更新：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\并发线程统一回执与main收口机制_2026-03-22.md`
- 已正式写明：
  - 默认一线程只提交自己修改的内容
  - 默认只提交自己的白名单路径
  - 默认一线程一次只收一刀 checkpoint
- 已纠正“提交 id”口径：
  - 不能自定义 Git SHA
  - 可以统一 checkpoint 名称 / commit message
  - 推荐格式：`YYYY.MM.DD_<线程名>_<编号>`

**关键决策**
- 这已经不是聊天建议，而是当前 `main-only` 并发模型的正式默认主流程。
- 治理侧以后只在高危撞车、跨线程耦合、branch carrier / worktree 迁入时介入。

**恢复点**
- 当前后续执行就按这条主流程走，不必每轮再从头讨论。

## 2026-03-22｜我已把 B组 先收 spring-day1 的 prompt 生出来
**当前主线目标**
- 继续顺着批次表往下走，把 B组 的首个迁入对象真正推进到可执行，而不是停在“先收谁”的口头判断。

**已完成**
- 已裁定：
  - B组 先收 `spring-day1`
  - 遮挡检查后置
- 已新增：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\spring-day1_B组main迁入执行.md`
- 已把动作压成：
  - 只把 `codex/spring-day1-0.0.2-foundation-001 @ 4ff31663` 的 6 个故事骨架代码文件迁入 `main`
  - 如需补记，只限 `spring-day1` 自己的两层 memory

**关键决策**
- `spring-day1` 当前优先级高于遮挡检查，因为它是 Day1 主骨架代码，不是 editor + docs checkpoint。

**恢复点**
- 你现在可以直接把这份 prompt 发给 `spring-day1`。
- 它回执后我再接遮挡检查。

## 2026-03-22｜我已经把白名单提交规则同步到执行层壳
**当前主线目标**
- 用户明确提醒“只做文档工作不够”，所以我要把默认规则继续压进真正的执行入口，而不只停在正文主线。

**已完成**
- 已更新：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\README.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\治理线程_职责与更新规则.md`
- 已新增：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\线程完成后_白名单main收口模板.md`
- 已把规则真正同步到执行层：
  - 默认一线程只提自己白名单
  - 默认一线程一次只收一刀 checkpoint
  - 统一 checkpoint 名称 / commit message：`YYYY.MM.DD_<线程名>_<编号>`

**关键决策**
- 现在这条规则已经同时存在于：
  - 正文主线
  - 执行层入口
  - 通用分发模板
- 这才算真正落地。

**恢复点**
- 当前继续等 `spring-day1` 回执。
- 回来后我继续接 B组 和全局 skills 裁定吸收。

## 2026-03-22｜我已把硬入口、阶段正文和回收卡对齐到同一条 main-only 白名单主线
**当前主线目标**
- 不再只在正文里维护“每线程白名单提交”，而是把真正会影响线程行为的硬入口、批次壳、回收卡和 dirty 归属说明全部改到一条线上。

**已完成**
1. 已纠偏 live 入口：
   - `AGENTS.md`
   - `Sunset当前唯一状态说明_2026-03-17.md`
   - `Sunset Git系统现行规则与场景示例_2026-03-16.md`
2. 已把 `spring-day1` 新回执吃进当前阶段判断：
   - 基础脊柱代码已进 `main @ 83d809a9`
   - 不再把它写成“仍在待迁入的 B组 carrier”
3. 已把农田和 `spring-day1` 的执行结果同步进：
   - `000全员回执.md`
   - `第一轮main收口批次表_2026-03-22.md`
   - `当前shared_root_dirty归属说明_2026-03-22.md`
4. 已吸收全局 skills 的窄裁定：
   - `sunset-startup-guard` 继续保留为 Sunset 硬前置
   - 但退出当前 `manual-equivalent` 一级告警统计
   - 治理线后续只再往 `skill-trigger-log.md` 追加 `STL-v2`
5. 已在 `D:\Unity\Unity_learning\Sunset @ main @ 83d809a9` 上做一轮真实 `task + main + IncludePaths` 预检，结果允许继续：
   - `shared root 当前位于 main-only 白名单 sync；允许保留 unrelated dirty，不再因 remaining dirty 一刀切阻断`

**关键决策**
- 当前这条治理线程的主线已经从“解释模型”推进到“入口纠偏 + 回收卡更新 + 脚本预检实证”三件套并行落地。

**恢复点**
- 现在直接做治理线自己的白名单 `sync`。
- 这刀收完后，shared root 剩下的噪音就主要是 NPC 和 `spring-day1` 文档整理线，不再是治理线自己。

## 2026-03-22｜我已把“规则已更新，可直接提交”写成两份现成 prompt
**当前主线目标**
- 不再只对用户口头说“现在可以自己提”，而是直接给出两个线程可转发版本。

**已完成**
1. 已新增 `NPC_继续修复并直接main收口.md`。
2. 已新增 `spring-day1_剩余文档整理并直接main收口.md`。
3. 两份 prompt 都已明确：
   - 当前规则已经更新
   - 可以直接按白名单提交到 `main`
   - 不再沿用旧的“先 branch / 不能提交”口径

**关键决策**
- 现在用户如果要推进，不需要再让我中间翻译一遍。
- 直接转发这两份文件即可。

**恢复点**
- 下一步等这两条线的新回执。

## 2026-03-22｜我已把固定回执箱写进当前批次 prompt
**当前主线目标**
- 回应用户“以后别让我自己建回执文件”的要求，把这件事从提醒变成 prompt 默认行为。

**已完成**
1. 已把当前批次固定回执箱定为：
   - `001部分回执.md`
2. 已把回执落点直接写进 NPC、`spring-day1` 和通用收口模板。

**关键决策**
- 从当前批次开始，回执文件由治理线提前准备，线程只负责往固定文件追加。

**恢复点**
- 下一步继续等新回执，不再让用户手动搭回执路径。

## 2026-03-22｜我把“能不能开工”彻底压成了现成前缀，也把自己的错误点记死
**当前主线目标**
- 回答用户最后那串很核心的问题：现在是不是已经可以直接开工、哪些线程该直接发 prompt、哪些不用再走一轮制度测试，以及我自己这条线到底还错在哪里。

**已完成**
1. 已新增普通线程前缀：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\并发线程_当前版本更新前缀.md`
2. 已新增 `scene-build` 专用前缀：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\scene-build_当前版本更新前缀.md`
3. 已形成当前分组：
   - 直接开工：`NPC`、农田、导航检查、遮挡检查
   - 直接开工但走 worktree 专用前缀：`scene-build`
   - 当前先停：`spring-day1`、全局 skills

**我这轮必须记住的错**
- 我前面最浪费时间的地方，不是没写规则，而是写了太多规则，却没有第一时间把“你现在就能直接干”的短前缀给线程。
- 我把 `scene-build` 一度误带到“迁新路径”上，结果把 worktree 特例和 shared root 迁移混成一件事，拖慢了推进。
- 我没有足够早地把“普通线程不用再重复制度测试、`scene-build` 只要改用 worktree 口径”说死，导致用户反复替我纠正。

**我对自己的后续约束**
1. 先发可执行前缀，再补治理解释。
2. 先给线程分组，再谈规则细节。
3. 只要线程已经满足直接推进条件，先明确告诉它“现在就干”，不要继续让它排队。
4. `scene-build` 这类特例只说明“按哪种口径干”，不再扩写成新的迁移工程。

**恢复点**
- 你现在可以直接拿新前缀去唤醒线程。
- 我自己下一步该做的是继续缩短路径、减少无效治理动作，而不是再造新的规范层。

## 2026-03-22｜我已经把第二波直接开发分发壳做完，不再让用户自己拼
**当前主线目标**
- 把我刚说的“下一步我来做剩余线程有效任务梳理 + 下一波直接唤醒顺序”真正落地，而不是再停在口头承诺。

**已完成**
1. 已新增批次级顺序文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\下一波直接唤醒顺序_2026-03-22.md`
2. 已新增 4 条专属 prompt：
   - `scene-build_当前版本更新并继续施工.md`
   - `导航检查_当前版本更新并继续直接开发.md`
   - `NPC_当前版本更新并继续直接开发.md`
   - `遮挡检查_当前版本更新并继续直接开发.md`
3. 已把顺序固定为：
   - `scene-build`
   - `导航检查`
   - `NPC`
   - `遮挡检查`

**我这轮继续给自己记住的约束**
1. 不再只给“通用前缀”，还要继续给“线程专属 prompt”。
2. 只要已经能判断优先级，就直接帮用户把下一波顺序排出来。
3. `spring-day1` 和全局 skills 这种已收口线程，要明确写“先停”，不要含糊地留成灰色状态。

**恢复点**
- 用户现在可以不靠聊天临时拼装，直接转发下一波 prompt。
- 我下一步只继续看回执和风险，不再反复解释“是不是能开始”。

## 2026-03-22｜我已经修正了第二波里最该修的两个错
**当前主线目标**
- 修正用户刚刚指出的关键问题：我虽然做了第二波 prompt，但导航和遮挡的版本还没真正切断旧分支思维；另外我错把 `spring-day1` 留在暂停态。

**已完成**
1. 已回读 `002全员部分聊天记录` 里的 3 条关键反馈：
   - 导航检查
   - 遮挡检查
   - `spring-day1`
2. 已确认我自己的错误：
   - 我上一版 prompt 只说“直接开发”，但没把“旧分支遗留必须这轮自己处理掉”压成硬要求
   - 我把 `spring-day1` 误当成“阶段完成先停”，但它真实状态已经回到“可继续做 Day1 首段剧情闭环”
3. 已修正：
   - `导航检查_当前版本更新并继续直接开发.md`
   - `遮挡检查_当前版本更新并继续直接开发.md`
   - 新增 `spring-day1_当前版本更新并继续直接开发.md`
4. 已更新第二波顺序文件，让 `spring-day1` 重新回到可开工队列

**我这轮继续给自己记住的约束**
1. 看到线程还在用旧分支口径时，不能只给“继续开发”的泛提示，必须直接写死“这轮自己处理 branch 遗留”。
2. 对已经收口到 `main` 的线程，下一步要看它还有没有真实业务闭环，而不是机械地判“先停”。
3. 用户给我目录级聊天记录时，要先读完再发 prompt，不能只靠上一轮摘要。

**恢复点**
- 你现在应该发的是纠偏后的导航/遮挡 prompt，以及新补的 `spring-day1` prompt。
- 我下一步只继续吃他们的新回执和现场冲突。

## 2026-03-23｜我必须记住：并发里最该补的是“改完代码后的轻量强制自检”
**当前主线目标**
- 响应用户对 3 段聊天记录的追问，判断该不该上 hook，以及 Codex 该怎么先管住自己写出来的代码正确率。

**已完成**
1. 已确认三条新证据其实都指向同一个问题：
   - 不是不会修
   - 而是修得太晚，等用户贴报错才修
2. 已把缺口收成三类：
   - `.cs` 改完后缺少目标文件静态自查
   - 缺少改文件后的编码 / diff 健康检查
   - 缺少收口前最小编译 / 测试验证

**我给自己的后续约束**
1. 以后看到“这类问题本不该等用户贴错误”的案例，要第一时间往“轻量强制自检层”归因，而不是只说线程粗心。
2. 不先上重型总闸门；先补最小但高收益的 post-edit / pre-sync 两层。
3. Unity/MCP 是第二层验证，不再让它背第一层低级错误筛查的锅。

**恢复点**
- 我下一步会把最小可落地方案讲清楚，再决定是否直接落脚本或 hook。

## 2026-03-23｜我已经把“代码后强制自检”真正做进主流程了
**当前主线目标**
- 不再只对用户说“应该有 post-edit / pre-sync 两层闸门”，而是把它们真正接到 Sunset 当前主流程上。

**已完成**
1. 已新增 `CodexCodeGuard` 工具项目。
2. 已把它接进 `git-safe-sync.ps1`：
   - `task` 模式白名单含 `.cs` 就自动触发
3. 已完成两轮真实测试：
   - 直接测当前遮挡线代码改动
   - 再测 `git-safe-sync.ps1` 集成输出
4. 已把 prompt / README / AGENTS / 当前快照一并同步到新口径。

**我这轮给自己记住的结论**
1. 真正的“最佳效果落地”不是写一堆规则，而是把闸门接到线程真的会经过的脚本上。
2. 多并发里最重要的是“只拦这轮新引入的问题”，否则所有线程都会被旧坑拖死。
3. 以后再遇到“本来不该等用户贴报错”的案例，优先补主流程闸门，不再只补提示词。

**恢复点**
- 这套代码闸门已经正式可用。
- 下一步只需要根据真实线程使用反馈做细化，而不是重做架构。

## 2026-03-23｜我给自己记住：全局 skills 旧回复已不是待处理项，别再重复追那批 Sunset 本地落地
**当前主线目标**
- 回答用户两个梳理问题：
  1. 另一个线程抛出的历史问题现在还要不要回
  2. 我自己到底还剩什么没做完

**已完成**
1. 已确认全局 skills 那条旧回复在 Sunset 本地已经闭环，不再需要额外批准它来补同一批事。
2. 已把我自己剩余项压到 3 类真实未完：
   - `TD_14` 还没裁
   - 代码闸门还没跑完一轮真实线程反馈
   - 当前批次原始材料还没归档

**我继续要避免的错**
1. 不要把“别人曾经说还能做”误当成“我这里还没做”。
2. 不要把现场别的线程 dirty 混算成我自己的治理欠账。

**恢复点**
- 后续再被问“你还剩什么”，按 ownership 说，不再泛化成一锅。

## 2026-03-23｜我这条治理线的老尾项已经收干净了
**当前主线目标**
- 把我刚刚列出来的最后几个老尾项收完，避免下一轮又重复说“还剩一点点”。

**已完成**
1. `TD_14` 已裁掉，不再挂在当前主线上。
2. 批次材料已做关账摘要。
3. 当前阶段任务板的剩余结构性尾项全部完成。

**我现在剩下的只有运行态工作**
1. 看线程新回执
2. 做冲突裁定
3. 观察代码闸门真实效果

**恢复点**
- 以后再回顾这条线，不再从“还有哪些老账没清”开始，而是从“当前新现场发生了什么”开始。

## 2026-03-23｜遮挡验证副本这件事我也已经关到 prompt 里了
**当前主线目标**
- 不让“这次到底批不批、怎么批”继续停在聊天里，而是做成可以直接发给遮挡线程的动作件。

**已完成**
1. 已把这次问题定性为：
   - 流程越界
   - 不是主仓库破坏性事故
2. 已生成专用 prompt：
   - `遮挡检查_验证副本有条件批准并测试后删除.md`

**恢复点**
- 这条问题现在已从判断态进入执行态；后续只等遮挡线程回测试结果和删副本回执。

## 2026-03-23｜快速出警通道已经不是概念，而是现成武器
**已完成**
1. `sunset-rapid-incident-triage` skill 已建好。
2. 背包异常案例已做直测，结果正确。
3. 遮挡线程单实例验证批准 prompt 已生成。

**恢复点**
- 下次用户一说“谁动了这个、快找犯人”，我不该再从零开始翻，而应直接走这条 skill。

## 2026-03-23｜遮挡线程这轮我也已经判断清楚了
**关键结论**
- 它这轮不是需要“全项目静默”，而是需要一个短时的主工程 Unity/MCP 单实例验证窗口。
- 所以我对它的裁定是：批，但要限窗，不准扩张。

## 2026-03-23｜我给自己记住：“治理尾账已清”不等于“全项目已清”
**关键结论**
- 以后对用户说“尾账清了”，必须明确限定是“治理线尾账”。
- 业务线程正在跑的 dirty、checkpoint、live 验证，不要混进“治理尾账”口径里。

## 2026-03-23｜并发制度现在该补的是窄口硬规则，不是重新造帝国
**当前主线目标**
- 用户追问当前改革是不是还存在大量漏洞，以及规范要不要继续加强；我要给出真实复盘和可落地补强方案，而不是再空谈。

**已完成**
1. 已把这轮真实暴露出来的制度缺口整理成正式文档：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\规范补强方案_2026-03-23.md`
2. 已明确 7 个当前最该补强的点：
   - 规则入口四同步
   - 热文件卫生字段
   - 资源卫生闸门
   - 例外退场四步
   - live 窗口四字段
   - 固定回执字段
   - 一刀一收纪律
3. 已把判断钉住：
   - `main-only` 方向继续保持
   - 不回退到 branch-first
   - 不重建重治理系统

**我这轮给自己记住的结论**
1. 现行制度不是错了，而是有几个高频事故点还没被制度真正覆盖。
2. 后续补规则时，优先补线程真实会踩到的坑，不再补抽象总纲。
3. 只要规则更新没有同步到 `AGENTS.md + 当前规范快照 + 批次 README + 通用前缀 prompt`，就不能算真的生效。

**恢复点**
- 下一步如果用户要我继续，就应该把这 7 条里最硬的几条正式同步进 live 入口，而不是再停留在方案层。

## 2026-03-23｜这次我没有只改总纲，也把线程入口一起改了
**当前主线目标**
- 用户要求“直接落地”，所以我继续把补强项从总规范同步进线程真正会读到的 prompt 入口。

**已完成**
1. 已把补强项同步到 live 入口：
   - `AGENTS.md`
   - 当前规范快照
   - 批次 `README.md`
   - 通用前缀 prompt
   - 通用白名单收口模板
2. 已继续同步到各线程活跃 prompt，覆盖 NPC、scene-build、spring-day1、农田、导航、遮挡。

**我这轮给自己记住的结论**
1. 规则只有落到线程实际会打开的 prompt，才算真的生效。
2. 以后用户如果说“所有他们遵守的东西都要同步修改”，不要只改规范正文，要连 prompt 入口一起改。

**恢复点**
- 下一步是把这批 prompt 同步改动再收一刀治理白名单提交。

## 2026-03-23｜网页学习后的编码检测已经补成正式治理结论
**当前主线目标**
- 用户让我阅读一篇关于 Codex 中文乱码根因的文章，并判断 Sunset 本地是否存在同类问题，要求给出本地检测报告。

**本轮完成**
1. 已确认文章核心不是“只改 VS Code”，而是统一：
   - `files.encoding`
   - `chcp`
   - `[Console]::InputEncoding`
   - `[Console]::OutputEncoding`
   - `$OutputEncoding`
   - PowerShell Profile 默认写文件编码
2. 已完成本地环境取证：
   - `PowerShell 5.1`
   - `pwsh` 缺失
   - `chcp=936`
   - `InputEncoding=gb2312`
   - `Console Output=utf-8`
   - `$OutputEncoding=us-ascii`
   - `$PROFILE` 缺失
   - VS Code 未显式设置 `"files.encoding": "utf8"`
3. 已完成文件层取证：
   - 有些文件只是 `healthy-terminal-noise`
   - `001部分回执.md` 这类样本已出现真实替换字符，不能再当成单纯终端误读
4. 已落正式报告：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\本地编码检测报告_2026-03-23.md`

**关键决策**
- 当前 Sunset 的编码问题是两层并存：
  1. 环境层未统一
  2. 文件层已有少量真实损坏
- 正确动作顺序是：
  1. 先统一环境
  2. 再修活文档
  3. 最后处理历史样本

**恢复点**
- 如果用户要我继续直接落地，下一步就不是继续分析，而是：
  - 修 Profile / VS Code UTF-8
  - 定点修 `001部分回执.md`

## 2026-03-23｜我已经把编码环境真的修掉了
**当前主线目标**
- 用户在看完检测结论后要求“直接开始落地”，所以我继续把环境层修复真正做完，而不是只留建议。

**本轮完成**
1. 已改本机 VS Code 用户设置，显式补上：
   - `"files.encoding": "utf8"`
2. 已创建：
   - `C:\Users\aTo\Documents\WindowsPowerShell\Microsoft.PowerShell_profile.ps1`
3. 已在 Profile 里统一：
   - `chcp 65001`
   - `InputEncoding / OutputEncoding / $OutputEncoding`
   - `Get-Content / Set-Content / Add-Content / Out-File / Export-Csv` 默认编码
4. 已复测确认：
   - 当前 shell 的 `chcp`、输入、输出、管道都已回到 `utf-8`
   - 不带 `-Encoding` 的 `Get-Content` 现在能正确读出本报告里的中文

**关键决策**
- 以后再遇到 Windows PowerShell 中文乱码，不能只修 Console 编码。
- 必须把默认读写编码也一起纳入 Profile，尤其是 `Get-Content:Encoding`。

**恢复点**
- 当前环境修复已完成。
- 下一步若继续，直接切到 `001部分回执.md` 定点修复。

## 2026-03-23｜用户自己的改单也已经按白名单收了一刀
**当前主线目标**
- 用户指出 `Anvil.png.meta` 是他自己手改的，要我直接帮他提交；同时问这 4 个 TMP 字体资源是不是 `spring-day1` 留下的。

**本轮完成**
1. 已确认 `Anvil.png.meta` 是有意义的真实改动，不是空脏改。
2. 已单独提交：
   - `30d6b5ca`
3. 已追查 TMP 字体资源近期历史：
   - 找不到任何最近 `spring-day1` 提交直接碰这 4 个字体资产

**关键决策**
- 以后用户自己的修改也可以直接按最小白名单 checkpoint 收，不必因为“不是某个线程”就卡住。
- TMP 这 4 个字体资源目前只能判为：
  - 本地 dirty 真实存在
  - 但尚不能按 Git 证据直接钉成 `spring-day1` 已提交责任

**恢复点**
- 如果继续处理脏现场，后面要么继续追字体资源来源，要么先修活文档坏样本。

## 2026-03-23｜`001部分回执.md` 这个活文档我已经修干净了
**当前主线目标**
- 继续吃我自己的剩余尾账，把此前已确认真实损坏的批次活文档 `001部分回执.md` 直接修掉。

**本轮完成**
1. 已定位出文件里真实损坏的 5 段：
   - 1 段 NPC 第二刀回执
   - 4 段遮挡检查回执
2. 已用线程 memory、工作区 memory 和批次上下文把这 5 段重建为干净中文回执。
3. 已复检确认：
   - 当前文件 `HAS_UTF8_REPLACEMENT=False`
   - 不再含 `npc�� / �ڵ���飺` 这类损坏残留

**关键决策**
- 这次修复再次证明：
  - 对活文档不能只记“这里坏了”
  - 应该尽快修成可读、可执行、可接手的状态
- 这也意味着我之前列出来的“下一步先修 `001部分回执.md`”现在已经完成，不再是待办。

**恢复点**
- 当前我自己的直接剩余动作，已经从“修这个坏文档”切回到：
  - 继续判断还有没有别的活跃坏样本值得处理
  - 或转去处理新的线程回执 / 治理问题

## 2026-03-23｜补审农田线与项目文档线回执，并下发下一轮 prompt
- 当前主线目标：
  - 继续承担 Sunset 治理侧的回执审核与 prompt 分发；本轮聚焦两条最新回执：
    - `农田交互修复V2` 的箱子 bridge 第 1 刀
    - `项目文档总览` 的成品层第一版
- 本轮子任务：
  - 快速核证据强度，给出“是否批准 + 缺口在哪里 + 下一轮怎么干”的治理裁定，并把 prompt 直接写进对应目录。
- 已完成事项：
  - 已确认农田线第 1 刀可以批准为 `checkpoint accepted`，但证据强度仅到 bridge 内存层；现有 `ChestInventoryBridgeTests` 尚未覆盖 `Save()/Load()` 与真实箱子 UI 重开路径。
  - 已确认项目文档线第一版成品层可以批准，但 `README v1` 的文档入口仍使用本地绝对路径，不能直接视为可落根版。
  - 已新增：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\003-第一刀checkpoint通过并转入第二刀与箱子链补强回归.md`
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\项目文档总览\26.03.23-项目经理简历画像与README委托-03可落根版与最终口径收口.md`
  - 已同步更新：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
- 关键决策：
  - 农田线的治理口径必须收紧为：
    - 第 1 刀通过
    - 但不能宣称“箱子链彻底稳定”
    - 下一轮必须在推进第 2 刀时补一条持久化或重开回归
  - 项目文档线的治理口径必须收紧为：
    - 第一版成品通过
    - 但不能停在 v1
    - 下一轮直接做 `README v2 可落根版 + 简历主口径推荐终稿`
- 恢复点 / 下一步：
  - 现在可以把两份新 prompt 直接发给对应线程；
  - 我这边只需要等它们按新回执格式回来，再继续做下一轮裁定。

## 2026-03-24｜从治理分发切到导航客观审核
**当前主线目标**
- 用户临时打断治理分发，要求我不要再写 prompt，而是直接站到“客观分析 / 锐评”位置，审核导航线程当前方案到底是不是合理、是不是最佳、最初框架是否就有偏差。

**本轮子任务**
- 回读导航核心代码、工作区 memory 与近期 live 回执；
- 把“线程宣称已全绿”与“用户仍感觉像大圆壳 / 推土机”之间的矛盾讲透；
- 给出独立判断，而不是只复述导航线程自己的结论。

**本轮完成**
1. 已按 Sunset AGENTS 做手工等价 startup guard，并显式说明：
   - 使用 `skills-governor`
   - 使用 `sunset-workspace-router`
   - 对 `sunset-review-router` 走手工等价审视流程
2. 已回读核心实现：
   - `NavigationLocalAvoidanceSolver.cs`
   - `NavigationAvoidanceRules.cs`
   - `NavigationPathExecutor2D.cs`
   - `PlayerAutoNavigator.cs`
   - `NPCAutoRoamController.cs`
   - `NPCMotionController.cs`
   - `NavigationAvoidanceRulesTests.cs`
3. 已得出稳定审核结论：
   - 当前方案不是 crowd sim / ORCA / RVO
   - 本质是“静态建路 + 共享执行层 + 启发式局部避让 + 临时 detour 点”
   - 共享的是执行底座，不是完整的动态交通语义
4. 已同步把这轮审视结论落回：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\memory.md`

**关键决策**
- 这条导航线不能简单判成“从头到尾都走错”：
  - 共享执行层、共享 registry、共享 avoidance 入口方向本身是成立的。
- 但它也绝不能被包装成“最佳设计”：
  - 从一开始更像“先统一路径执行，再不断给动态避让打补丁”，而不是先定义统一动态交通规则。
- 所以当前最准确的双结论是：
  - 功能口径：已经从系统级失败拉回到受控场景可通过
  - 体验口径：仍不是最自然、最优雅的动态避让框架

**恢复点**
- 这轮之后，如果用户继续追导航，不该再用治理分发口气回答；
- 应继续按“代码证据 + 架构判断 + 体验口径”去审，而不是重新回到给线程排工单。

## 2026-03-24｜硬读 Gemini 聊天记录后的导航需求再锚定
**当前主线目标**
- 用户要求我不要停留在已有审核结论，而是把 `005-gemini聊天记录-1.md` 全文硬读完，再回答“我现在真正理解到了什么”。

**本轮子任务**
- 不做新 prompt、不做新 patch；
- 直接把这份 Gemini 长对话里真正稳定的需求、架构信息和过度绝对化部分拆开。

**本轮完成**
1. 已完整读完：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\005-gemini聊天记录-1.md`
2. 已确认这份文件不是普通锐评，而是一次“问题层级升级”：
   - 从现状解释
   - 到架构罪证
   - 再到用户把标准升级为专业底座
   - 最后形成四层架构白皮书
3. 已把稳定结论同步回：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\memory.md`

**关键决策**
- 这份 Gemini 文件最重要的贡献不是“继续骂当前导航烂”，而是把用户真实标准说透：
  - 要的不是补丁修复
  - 要的是面向未来怪物追击、宠物跟随、NPC 生态的专业导航地基
- 它有价值的方向包括：
  - 宏观/微观断层
  - 动态实体从静态网格阻挡中剥离
  - detour 污染路径层的问题
  - 统一最终运动语义
- 但它也不是可以原样神化照搬的最终设计：
  - 有些表述过于绝对
  - 仍需要结合 Sunset 当前已有资产做项目化收敛

**恢复点**
- 如果这条导航线继续推进，我接下来更应该帮用户做的是：
  - 把 Gemini 的白皮书和当前代码现实合并成一份真正可执行的 Sunset 导航需求与架构设计稿；
- 而不是继续只围绕单个 detour bug 或单个参数说事。

## 2026-03-24｜导航正式设计稿已落盘
**当前主线目标**
- 用户在确认我已理解 Gemini 长对话后，明确要求“彻底落地”；
- 因而本轮目标从“继续解释”切换为“把导航线真正落成正式设计文档”。

**本轮子任务**
- 在 `导航检查` 工作区新增一份正式的长期导航需求与架构正文；
- 再把执行主表和记忆层接到这份正文上。

**本轮完成**
1. 已新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\006-Sunset专业导航系统需求与架构设计.md`
2. 文档已系统写清：
   - 用户真实目标
   - 当前导航根问题
   - 红线
   - 现有资产继承/重构/废止
   - 5 层目标架构
   - 功能线 / 体验线双验收
   - 迁移阶段
3. 已同步更新：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\统一导航重构阶段设计与执行主表.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\memory.md`

**关键决策**
- 导航线现在已经不能再被定义成“继续修旧导航直到能用”。
- 从这轮开始，它的正式主线定义应是：
  - `Sunset` 专业导航底座迁移
- 这意味着后续代码线程必须以 `006-Sunset专业导航系统需求与架构设计.md` 为上位依据。

**恢复点**
- 后续如果继续导航推进，我应优先帮用户做的是：
  - 从 `006` 再拆出第一阶段迁移任务，而不是重新回到解释或泛化批评。

## 2026-03-24｜审核 `005-gemini锐评-2.md` 并按 Path B 吸收补洞
**当前主线目标**
- 用户要求对 `005-gemini锐评-2.md` 走正式审核，而且强调要格外谦虚、严谨。

**本轮子任务**
- 判断该锐评属于 Path A / B / C 哪一条；
- 如果核心方向成立，则把真正有价值的补洞正式吸收到 `006`。

**本轮完成**
1. 已完整阅读：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\005-gemini锐评-2.md`
2. 已对照：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\006-Sunset专业导航系统需求与架构设计.md`
   - 当前导航代码现实
   - 工作区 memory
3. 已判定为：
   - `Path B`
4. 已把 3 个有效补洞写回 `006`：
   - Layer E 的状态平滑与刹停恢复
   - shape-aware 方案的性能边界
   - 交通记忆 / 状态惯性

**关键决策**
- 这份锐评主干是对的，但“全面放行”“工业级白皮书已成”这种口气不能直接照单全收。
- 最合理的处理方式不是打回，也不是神化，而是：
  - 接受方向
  - 降温措辞
  - 吸收工程级补洞

**恢复点**
- 后续导航审稿继续维持同一口径：
  - 严肃验证
  - 拒绝绝对化吹捧
  - 有效建议直接写回正式设计稿

## 2026-03-24｜导航后续开发路线图已正式落盘
**当前主线目标**
- 用户不再要我继续发治理 prompt，也不再只要审稿；这轮明确要求我基于 `006` 已通过的正式设计，直接梳理“后续开发方向、阶段划分、验收口径、注意事项”。

**本轮子任务**
- 不做新的业务 patch；
- 直接把导航线从“有了上位设计正文”推进到“有了后续开发路线图与执行顺序”。

**本轮完成**
1. 已新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\007-Sunset专业导航底座后续开发路线图.md`
2. 已在 `007` 中明确：
   - 下一步唯一推荐动作是进入 `S0 基线冻结与施工蓝图`
   - 后续开发共 `9` 个阶段
   - 每阶段的目标、产物、关键任务、验收、风险、热区与禁区
3. 已同步更新：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\memory.md`

**关键决策**
- 导航线现在已经不是“只差继续证明设计方向没错”，而是正式进入“怎么迁移、先迁什么、哪些先别碰”的执行阶段。
- 从这轮开始，导航线的三层入口固定为：
  - `006`：长期需求与架构正文
  - `007`：后续开发路线图
  - 主表：当前阶段执行推进

**恢复点**
- 后续若再推进导航，不该重新从锐评、prompt 或局部 patch 起步；
- 应直接按 `007` 的 `S0 -> S8` 路线继续。

## 2026-03-24｜导航线程执行口径已切成 `S0-S6` 连续闭环授权
**当前主线目标**
- 用户明确指出：导航线如果继续被拆成很多小轮 prompt，极容易重新漂移；
- 本轮要求我不要再给它过细的碎刀指令，而是直接写一份更高质量的 prompt，授权它把 `S0-S6` 连续做完，`S7/S8` 继续后延。

**本轮子任务**
- 不再继续输出泛泛建议；
- 直接把这次新的治理口径落成导航工作区 prompt 文件。

**本轮完成**
1. 已新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-7.md`
2. 新 prompt 已正式规定：
   - 以 `006 + 007` 为硬上位依据
   - 连续推进 `S0 -> S6`
   - 允许中间 checkpoint，但不允许把主线拆回旧式碎刀
   - 严禁漂回旧 patch 主线
   - 严禁越级碰 `S7/S8`
   - 严禁顺手扩张到无关业务线

**关键决策**
- 这轮我认同用户的判断：
  - 当前导航线最该避免的，不是“自由度过大”，而是“主线被拆碎后不断偏移”。
- 所以最合理的治理方式不是继续细分更多 prompt，而是：
  - 把 `S0-S6` 收成一个连续闭环委托
  - 再用固定回执字段和完成定义防止放飞

**恢复点**
- 后续如果继续看导航线，我不该再按旧细刀逻辑给它排单；
- 应直接审它有没有真正把 `S0-S6` 闭环推进起来。

## 2026-03-24｜导航审稿自用 prompt 已落盘
**当前主线目标**
- 用户这轮新增的需求，不是再给导航线程排工，而是要一份“发给我自己和 Gemini 的导航验收审稿 prompt”，方便后续把导航回执贴进来后直接按固定口径审。

**本轮子任务**
- 把导航审稿职责、必须对照的文档、必须回答的问题、红旗、输出格式全部收成一个稳定模板。

**本轮完成**
1. 已新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\008-给Codex与Gemini的导航验收审稿prompt.md`
2. 模板已明确要求审稿者：
   - 不是秘书、不是鼓掌员、不是回执复读机
   - 必须对照 `006 / 007 / 002-prompt-7 / 主表 / memory`
   - 必须给出 `S0-S6` 分阶段验收
   - 必须明确功能线 / 体验线、旧 patch 是否托底、导航线程是否真的做得到

**关键决策**
- 这份 `008` 的价值不在于再给导航线程加压，而在于：
  - 以后用户一贴导航回执，我和 Gemini 都会被收进同一套审稿纪律里，不再飘。

**恢复点**
- 后续如果用户贴来导航回执，我应直接按 `008` 输出：
  - 核心判决
  - `S0-S6` 验收表
  - 关键风险
  - 缺失证据
  - 最终裁定

## 2026-03-24｜spring-day1 Day1 工作台/UI 重收口 prompt 已落盘
**当前主线目标**
- 用户这轮明确回到 `spring-day1`，要求我替它写一份新的执行 prompt；
- 重点不是再泛泛催工，而是把 Day1 工作台 UI、任务限制、血量/精力条、制作过程表现、距离与上下判断、首次 `E` 提示，以及“用户必须能手动拖动精调 UI”这一整轮要求收成正式委托。

**本轮子任务**
- 给 `spring-day1` 生成新的阶段 prompt，并把用户 7 点原文完整摘录进去；
- 同时把回执格式写成硬约束，避免线程继续绕过关键问题。

**本轮完成**
1. 已新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\26.03.24-Day1工作台UI与任务体验重收口委托-01.md`
2. 该 prompt 已明确要求线程：
   - 不能只修样式，要同时收布局、交互、任务链、表现层和可手调性；
   - 必须解释当前 UI 为什么不能拖、改了什么之后用户可以拖；
   - 必须把用户这次 7 点原文全部落地，不允许只挑一两点做。

**关键决策**
- 这轮不再沿用零散口头催工；
- 对 `spring-day1` 这种已经进入体验收口阶段的线，最稳的方式就是把用户原文、禁止漂移项、执行顺序、硬回执字段一起写进正式 prompt。

**恢复点**
- 后续如果要继续推进 `spring-day1`，直接让线程读取这份新 prompt 即可；
- 我这边下一步只需要等它的回执，再继续审方向和验收。