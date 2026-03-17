# Codex规则落地 - 线程记忆

> 历史长卷保留在 [memory_1.md](/D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/Codex规则落地/memory_1.md)。本卷只保留当前活跃治理主线、稳定结论与恢复点。

## 线程概述
- 线程名称：Codex规则落地
- 线程分组：Sunset
- 线程定位：负责 Sunset 中与 Codex 治理相关的全局规则、入口、skills、AGENTS、线程执行纪律、共享根目录占用语义和事故收口；不代替业务线程写业务代码。

## 当前主线目标
- 把 Sunset 的治理体系从“规则很多但经常被跳过”，推进到“进入实质性任务前会被强制触发正确入口与现场核查”。
- 同时把本轮 `NPC / farm / spring-day1` 带出来的事故期临时 `worktree` 全部收回，最终回到 `main + branch-only` 常态。

## 当前状态
- **用户视角完成度**：0%
- **治理推进度**：65%
- **最后更新**：2026-03-17
- **状态**：持续推进中

## 当前稳定结论
- Sunset 当前治理续办总入口固定为：
  - [D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex)
- 当前最终收口主线已经升级为：
  - [11_main-branch-only回归与worktree退役收口](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/tasks.md)
- 当前共享根目录 `D:\Unity\Unity_learning\Sunset` 仍是事故取证现场，不是可写业务现场。
- `worktree` 当前只能被承认为：
  - 事故隔离容器
  - cleanroom / rescue / 历史取证点
  - 不是 Sunset 默认开发方式
- 当前三条业务线的合法 carrier 已基本明确：
  - `farm -> codex/farm-1.0.2-cleanroom001 @ 66c19fa1`
  - `NPC -> codex/npc-roam-phase2-002 @ 6e2af71b`
  - `spring-day1 -> codex/spring-day1-story-progression-001 @ a9c952b7`
- 第一批纯历史 worktree 已真实退役：
  - `main-reflow-carrier`
  - `NPC`
  - `farm-10.2.2-patch002`

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
1. 等三条业务线返回 branch-only 迁回方案。
2. 补 shared root 归属表。
3. 开始执行第二批临时 worktree 退役，而不是继续容忍它们作为长期现场存在。
