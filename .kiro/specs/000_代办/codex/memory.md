# codex 代办主线记忆

> 历史长卷已保留在 [memory_0.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/memory_0.md)。本卷只保留当前治理续办的活跃摘要、恢复点和下一步。

## 当前主线目标
- 让 `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex` 成为 L5 之后唯一有效的治理续办总入口。
- 当前治理主线不再承担“项目恢复”本身，而是负责恢复之后遗留的规则收口、skills/AGENTS 强约束、shared root 纪律与 memory 体系重构；其中 `11` 已完成，后续如继续治理应回到 `09` 或另行立项。

## 当前状态
- **用户视角完成度（11阶段）**：100%
- **治理续办总盘状态**：Git/现场层终局已完成，后续如继续治理应回到 `09`
- **最后更新**：2026-03-17
- **状态**：`11` 已完成并封板

## 当前阶段目录
- `09_强制skills闸门与执行规范重构`
- `10_共享根仓库分支漂移与现场占用治理`
- `11_main-branch-only回归与worktree退役收口`

## 当前稳定结论
- 旧全局 `tasks.md` 不再承接新治理任务；当前治理续办统一进入 `000_代办/codex` 的阶段目录。
- `03-17` 起，现行入口已经切到新的状态说明与总索引。
- `09` 已完成“项目级启动闸门落地”的第一轮：
  - `sunset-startup-guard` 已创建
  - `Sunset/AGENTS.md` 已写入强制入口
- `10` 已完成事故事实厘清与承载面找回：
  - 污染分支历史已锁定为：`codex/farm-1.0.2-correct001 @ 11e0b7b4`
  - `farm` 合法 carrier：`codex/farm-1.0.2-cleanroom001 @ 66c19fa1`
  - `NPC` 合法 carrier：`codex/npc-roam-phase2-002 @ 6e2af71b`
  - `spring-day1` clean checkpoint：`codex/spring-day1-story-progression-001 @ a9c952b7`
- `11` 已完成最终收口目标：
  - 临时 `worktree` 只能是事故容器
  - shared root 已回到 `D:\Unity\Unity_learning\Sunset @ main`
  - `git worktree list --porcelain` 只剩共享根目录
  - `spring-day1 / NPC / farm` 已完成根目录 branch-only 检出验证
  - `npc_restore.zip` 与 `Assets/Screenshots*` 已移出仓库工作树

## 当前最高优先级
1. 若继续治理，回到 `09_强制skills闸门与执行规范重构`，继续压实 skills、AGENTS 与四件套的强约束。
2. `10` 与 `11` 保留为历史结论层，不再继续承接新的过程尾巴。
3. 继续遵守“shared root `main` + 业务分支”的默认模型，除非未来再次出现确有必要的高风险隔离例外。

## 当前恢复点
- `09` 下一步：
  - 继续验证 `sunset-startup-guard` 的 session 稳定可见性
  - 把“禁止 worktree 常态化”进一步吸收到启动闸门口径
- `10` 下一步：
  - 不再新增尾巴
  - 保留为事故治理前置结论层
- `11` 下一步：
  - 无阶段内剩余动作
  - 后续只保留终局快照、任务清单与过程证据，不再继续承接新任务

## 最近会话

### 会话 1 - 2026-03-17（11 阶段正式接管最终收口）
**用户目标**：
> 不接受 worktree 变成常态；要求彻底回到 `main-branch-only`，并把剩余所有代办、情况和 prompt 统一收进新阶段，不再让 `09/10` 继续承担收尾杂项。

**已完成事项**：
1. 创建并重写 `11_main-branch-only回归与worktree退役收口`：
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
4. 已真实退役第一批纯历史 worktree：
   - `main-reflow-carrier`
   - `NPC`
   - `farm-10.2.2-patch002`
5. 已新增：
   - [所有线程回归誓言.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/所有线程回归誓言.md)（线程回包索引）
   - [共享根目录dirty归属初版_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/共享根目录dirty归属初版_2026-03-17.md)

**关键决策**：
- 当前用户视角完成度仍按 `0%` 计，直到共享根目录回到 `main` 且 worktree 被真正退役。
- 分支是长期 carrier，worktree 只是事故容器。
- `导航检查 / 遮挡检查 / 项目文档总览` 当前都不需要为了这轮 branch-only 回归新建 worktree。

**恢复点 / 下一步**：
- 现在应优先把 branch-only 回归 prompt 发给 `spring-day1 / NPC / farm`。
- 收到三方答复后，继续推进 shared root 清理和第二批 worktree 退役。

### 会话 2 - 2026-03-17（`11` 阶段纠正“真实回包目录”并进入执行态）
**用户目标**：
> `所有线程回归誓言` 目录里存放的是线程真实回包，不是规范正文；要求基于这批真实回包继续推进，而不是继续围着占位索引打转。

**已完成事项**：
1. 纠正 `11` 阶段中对 `所有线程回归誓言` 的语义：
   - `.md` 是索引/提炼位
   - 同名目录 `所有线程回归誓言\*.md` 才是真实线程回包
2. 读取并提炼六份线程回包，确认：
   - `spring-day1`、`NPC`、`farm` 的 branch-only 唯一入口都已明确
   - `导航检查 / 遮挡检查 / 项目文档总览` 都已退出 worktree 阻塞清单
3. 识别出当前真正剩余阻塞：
   - `spring-day1` 字体 dirty 在 shared root 与 `NPC_roam_phase2_rescue` 双现场分裂
   - shared root 自身治理/归档 dirty 尚未形成可执行收口
   - 杂项 `untracked` 归属未定
4. 新增或更新：
   - [共享根目录dirty归属可执行版_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/共享根目录dirty归属可执行版_2026-03-17.md)
   - [所有线程回归誓言.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/所有线程回归誓言.md)
   - [第二批worktree核验表_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/第二批worktree核验表_2026-03-17.md)
   - [分发Prompt_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/分发Prompt_2026-03-17.md)

**关键决策**：
- `11` 当前已经从“等待回包”切换到“执行 shared root 回 `main` 前置动作”的状态。
- 二轮 prompt 当前只需要发给 `spring-day1`；`NPC` 与 `farm` 只需等待 shared root 回 `main` 后做 branch-only 验证。

**恢复点 / 下一步**：
- 发 `spring-day1` 二轮 prompt。
- 然后继续 shared root 的治理收口与最终回 `main` 执行准备。

### 会话 3 - 2026-03-17（`spring-day1` 二轮已执行，阻塞缩到治理层）
**用户目标**：
> 审核 `spring-day1` 二轮回复并直接推进下一步。

**已完成事项**：
1. 实核二轮回复的资源引用与 diff 结论成立。
2. 导出 shared root 与 rescue 的字体 dirty 证据补丁与 diffstat。
3. 已实际清掉两处字体 dirty，保留已提交版本。
4. `NPC_roam_phase2_rescue` 已恢复为 `CLEAN`。
5. 已删除 `Assets/111_Data/NPC 1.meta`。

**关键决策**：
- `spring-day1` 不再是 shared root 回 `main` 的主要阻塞。
- 当前剩余阻塞已收缩成：
  - shared root 治理/归档 dirty
  - `Assets/Screenshots*`
  - `npc_restore.zip`

**恢复点 / 下一步**：
- 继续 shared root 自身清尾。
- 处理截图与 zip 杂项。
- 然后推进 shared root 回 `main`。

### 会话 4 - 2026-03-17（`11` 已封板，治理总入口切回终局口径）
**用户目标**：
> 在 shared root 已回 `main`、worktree 已退役之后，把 `000_代办/codex` 总入口和相关 memory 全部改成终局口径，不要再保留“11 还在 65%”“还要继续发 prompt”的旧状态。

**已完成事项**：
1. 复核 `11` 终局事实：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `git worktree list --porcelain` 只剩共享根目录
2. 为 `11` 新增 [终局快照_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/11_main-branch-only回归与worktree退役收口/终局快照_2026-03-17.md)。
3. 重写 `11` 的 `tasks.md / analysis.md / 总进度与收口清单_2026-03-17.md`，把阶段状态从“执行中”改成“已完成”。
4. 重写当前状态入口：
   - [Sunset当前唯一状态说明_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset当前唯一状态说明_2026-03-17.md)
   - [Sunset现行入口总索引_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset现行入口总索引_2026-03-17.md)
5. 同步本父级 memory，明确：
   - `11` 已封板
   - 后续治理应回到 `09` 或新阶段

**关键决策**：
- `11` 完成后，不再继续用它承接新的治理过程尾巴。
- `10` 保留事故结论，`11` 保留终局快照；下一步若继续治理，应返回 `09` 的强制闸门与规则执行面。

**恢复点 / 下一步**：
- `000_代办/codex` 仍然是治理总入口。
- 但当前活跃推进点已经从 `11` 切回到后续治理深化；若用户继续推进规则/skills/四件套，应直接回到 `09` 或新阶段开工。
