# Codex规则落地 - 线程记忆

> 历史长卷保留在 [memory_1.md](/D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/Codex规则落地/memory_1.md)。本卷只保留当前活跃治理主线、稳定结论与恢复点。

## 线程概述
- 线程名称：Codex规则落地
- 线程分组：Sunset
- 线程定位：负责 Sunset 中与 Codex 治理相关的全局规则、入口、skills、AGENTS、线程执行纪律、共享根目录占用语义和事故收口；不代替业务线程写业务代码。

## 当前主线目标
- 把 Sunset 的治理体系从“规则很多但经常被跳过”，推进到“进入实质性任务前会被强制触发正确入口与现场核查”。
- 同时把本轮 `NPC / farm / spring-day1` 带出来的事故期临时 `worktree` 全部收回；这一步已经完成，当前若继续治理应回到启动闸门、skills 与规则执行面的深化。

## 当前状态
- **用户视角完成度（11阶段）**：100%
- **治理推进度（当前子主线 11）**：100%
- **最后更新**：2026-03-17
- **状态**：`11` 已完成并封板

## 当前稳定结论
- Sunset 当前治理续办总入口固定为：
  - [D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex)
- 当前最终收口主线已经升级为：
  - [11_main-branch-only回归与worktree退役收口](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/tasks.md)
- 当前共享根目录已经恢复为：
  - `D:\Unity\Unity_learning\Sunset @ main`
- `worktree` 在当前 Sunset 口径中已重新收束为例外：
  - 只用于高风险隔离、事故 cleanroom 或特殊实验
  - 不再是默认开发方式
- 当前三条业务线的合法 carrier 已完成 branch-only 回归：
  - `farm -> codex/farm-1.0.2-cleanroom001 @ 66c19fa1`
  - `NPC -> codex/npc-roam-phase2-002 @ 6e2af71b`
  - `spring-day1 -> codex/spring-day1-story-progression-001 @ a9c952b7`
- 当前 `git worktree list --porcelain` 只剩共享根目录。
- `npc_restore.zip` 与 `Assets/Screenshots*` 已移出仓库工作树。

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
   - [总进度与收口清单_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/总进度与收口清单_2026-03-17.md)
   - [分发Prompt_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/分发Prompt_2026-03-17.md)
3. 重写现行入口文档：
   - [Sunset当前唯一状态说明_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset当前唯一状态说明_2026-03-17.md)
   - [Sunset现行入口总索引_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset现行入口总索引_2026-03-17.md)
4. 已真实执行第一批纯历史 worktree 退役，`git worktree list` 已缩减。
5. 已新增：
   - [所有线程回归誓言.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/所有线程回归誓言.md)（线程回包索引）
   - [共享根目录dirty归属初版_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/共享根目录dirty归属初版_2026-03-17.md)

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
   - [所有线程回归誓言.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/所有线程回归誓言.md)
   - [共享根目录dirty归属可执行版_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/共享根目录dirty归属可执行版_2026-03-17.md)
   - [第二批worktree核验表_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/第二批worktree核验表_2026-03-17.md)
   - [分发Prompt_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/分发Prompt_2026-03-17.md)

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
1. `11` 不再追加新任务。
2. 若继续推进治理，回到 `09_强制skills闸门与执行规范重构`，继续压实启动闸门、skills 与规则执行面。
3. 保持 shared root `main` + 业务分支 的默认模型，不再让 `worktree` 回潮成常态。

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
   - [终局快照_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/终局快照_2026-03-17.md)

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
**涉及文件**：`C:\Users\aTo\.codex\skills\sunset-startup-guard\SKILL.md`、[09_强制skills闸门与执行规范重构/memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/09_强制skills闸门与执行规范重构/memory.md)、[memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/memory.md)、[memory_0.md](/D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/Codex规则落地/memory_0.md)。  
**恢复点 / 下一步**：后续若继续推进本线程，优先进入 `09` 去压实 `sunset-startup-guard` 的稳定命中、最小回复结构和 `worktree` 红线执行。

### 会话 6 - 2026-03-17（总进度盘点与未完项重新分层）
**用户目标**：要一份能够直接回答“现在项目到底到哪一步、还剩什么、哪些是真的没做完”的总盘点。  
**完成事项**：通读 `000_代办/codex/01-11` 的 `tasks.md`，并结合当前 Git 现场重新分层：`11` 已真实完成，`04/05/06/08` 也已达到阶段完成；`03/10` 有旧勾选未收口，但多数已被 `11` 的终局动作覆盖；当前真正还活跃的治理主线只剩 `09`，此外还有 `01/02/07` 的少量收尾债。  
**关键决策**：业务线程警戒线已经解除，不需要再分发 branch-only prompt；当前剩余的“警戒线”主要是治理层的，即 skills 强制命中、启动闸门稳定执行、旧待办显式封板这几项。  
**涉及文件**：[09_强制skills闸门与执行规范重构/tasks.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/09_强制skills闸门与执行规范重构/tasks.md)、[memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/09_强制skills闸门与执行规范重构/memory.md)、[memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/memory.md)。  
**恢复点 / 下一步**：从 `09` 继续做强制 skills 闸门落地；同时择机清掉 `03/10` 的文档残余未勾选项，防止以后再被误读成系统尚未收口。
