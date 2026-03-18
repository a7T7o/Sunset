# 阶段记忆：main-branch-only 回归与 worktree 退役收口

## 2026-03-18 迁移说明
- 本阶段保留“历史上曾完成一次 branch-only 回归”的事实记录。
- 但 `2026-03-18` 的 live 现场已再次失真，因此新的 shared root 回正与防回退工作，不再由本阶段承接。
- 当前请改读：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\20_shared-root现场回正与物理闸机落地`

## 当前状态
- **用户视角完成度**：100%
- **治理推进度**：100%
- **最后更新**：2026-03-17
- **状态**：shared root 已回 `main`，三条业务线根目录 branch-only 验证已完成，第二批 worktree 已退役，`11` 阶段已封板。

## 会话记录

### 会话 1 - 2026-03-17：11 阶段立项
**用户目标**：
> 彻底删除所有 worktree 相关内容，最终进入 `main-branch-only` 阶段；`worktree` 只能是事故隔离例外，不能成为新常态。

**本轮子任务**：
- 新建独立 `11` 阶段，承接 `09/10` 之后全部“回归 branch-only 常态”的剩余问题。
- 盘点所有已注册 worktree 及其真实 Git 状态。
- 明确三大业务线与历史容器的初步分类。

**已完成事项**：
1. 创建 `11` 阶段目录：
   - `analysis.md`
   - `tasks.md`
   - `执行方案.md`
   - `memory.md`
2. 实核当前全部已注册 worktree 与 `branch / HEAD / clean-dirty` 状态。
3. 把“最终必须回到 branch-only 常态”的目标写入本阶段文档。

**关键决策**：
- 当前不是“worktree 模型合理化”，而是“事故隔离期尚未结束”。
- `farm / NPC / spring-day1` 当前都还没有完成从临时容器回到 branch-only 常态的最后迁回。

**恢复点 / 下一步**：
- 向 `spring-day1 / NPC / farm` 分发 branch-only 回归 prompt。
- 补齐共享根目录恢复 `main` 的前置条件与 worktree 退役顺序。

### 会话 2 - 2026-03-17：branch-only 口径补实
**用户目标**：
> 不接受半成品；不仅要说清楚，还要把总进度、剩余任务、可转发 prompt 和最终退役顺序都落到文档里。

**本轮子任务**：
- 把 `11` 从“已立项”推进到“有执行面”的状态。
- 重写本阶段核心文档。
- 产出可直接转发给线程的 prompt 成品。
- 将 `11` 提升为当前治理收口主线，而不是继续让 `09/10` 承担尾巴。

**已完成事项**：
1. 重写 `11` 阶段的：
   - `analysis.md`
   - `tasks.md`
   - `执行方案.md`
2. 新增：
   - `总进度与收口清单_2026-03-17.md`
   - `分发Prompt_2026-03-17.md`
3. 更新现行状态说明与现行入口总索引，把 `11` 纳入当前活跃收口主线。
4. 明确写死：
   - 用户视角完成度仍按 `0%` 计
   - `worktree` 只承认事故容器身份
   - `导航检查 / 遮挡检查 / 项目文档总览` 当前不需要新建 worktree

**关键决策**：
- 以后不能再把 `NPC_roam_phase2_continue`、`farm-1.0.2-cleanroom001`、`spring-day1-story-progression-001` 当成长住开发现场表述。
- 分支是长期 carrier，worktree 只是临时容器。
- 在 shared root 还没恢复 `main` 前，物理删除 worktree 不能抢跑，但退役顺序和 prompt 可以先锁定。

**恢复点 / 下一步**：
- 等 `spring-day1 / NPC / farm` 返回 branch-only 迁回方案。
- 补共享根目录 dirty 归属表。

### 会话 3 - 2026-03-17：第一批历史 worktree 已真实退役
**用户目标**：
> 不要再停留在“文档层”，要开始真实清理现场。

**本轮子任务**：
- 对第一批纯历史、已 clean、且不再承载当前主线的 worktree 做真实退役。

**已完成事项**：
1. 实核以下三处 worktree 都是 clean，且对应分支对象仍在仓库内：
   - `D:\Unity\Unity_learning\Sunset_worktrees\main-reflow-carrier`
   - `D:\Unity\Unity_learning\Sunset_worktrees\NPC`
   - `D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002`
2. 已真实执行 `git worktree remove` 并完成删除。
3. 当前 `git worktree list` 已缩减为：
   - 共享根目录
   - `farm-1.0.2-cleanroom001`
   - `NPC_roam_phase2_continue`
   - `NPC_roam_phase2_rescue`
   - `spring-day1-story-progression-001`

**关键决策**：
- 现在已经可以把“历史容器退役”从计划项改成已执行项。
- 但第二批仍不能抢跑删除，因为它们还与三大业务线的 branch-only 迁回方案直接相关。

**恢复点 / 下一步**：
- 继续收 `spring-day1 / NPC / farm` 的 branch-only 回归答复。
- 再推进第二批临时容器退役与 shared root 回 `main`。

### 会话 4 - 2026-03-17：统一誓言与 shared root 初版归属图已落盘
**用户目标**：
> 在继续推进未完成事项的同时，把“所有线程必须回归 branch-only”的共同约束写成正式文本，并继续把 shared root 回 `main` 的前置准备做实。

**本轮子任务**：
- 创建所有线程统一约束文档。
- 创建 shared root 的第一版 dirty 归属图。

**已完成事项**：
1. 新增：
   - `所有线程回归誓言.md`（线程回包汇总位，文件名沿用用户指定）
   - `共享根目录dirty归属初版_2026-03-17.md`
   - `第二批worktree核验表_2026-03-17.md`
2. 将 shared root 当前脏内容分为：
   - 治理线程活跃改动
   - `spring-day1` 明确认领
   - `NPC` 当前认领
   - 治理归档/重组残留
   - 待核验杂项垃圾
3. 核验第二批剩余 worktree 真实现场：
   - `farm-1.0.2-cleanroom001` clean
   - `NPC_roam_phase2_continue` clean
   - `NPC_roam_phase2_rescue` dirty，但 dirty 已知归属 `spring-day1`
   - `spring-day1-story-progression-001` clean
4. 将这些新文档写回 `tasks.md` 与总进度清单，作为已完成项。

**关键决策**：
- 现在 shared root 还不能回 `main`，但理由已经不再模糊，而是有了第一版归属图支撑。
- “所有线程回归誓言.md” 不再承担规范正文职责，而是专门用于汇总后续线程回包。

**恢复点 / 下一步**：
- 发出 `spring-day1 / NPC / farm` 的 branch-only prompt。
- 等它们回完 branch-only 方案后，把 shared root 第一版归属图升级成最终可执行归属表。

### 会话 5 - 2026-03-17：纠正“线程回包目录”误读并进入执行态
**用户目标**：
> `所有线程回归誓言` 目录里放的不是规范正文，而是发完 prompt 后各线程给回来的真实回应；不要再把占位索引文件当成真实回包本体。

**本轮子任务**：
- 纠正我对 `所有线程回归誓言` 的读取对象误判。
- 读取真实线程回包目录中的 `spring-day1 / NPC / 农田交互修复V2 / 导航检查 / 遮挡检查 / 项目文档总览` 六份回包。
- 把 `11` 从“等待回包”推进到“基于回包的可执行收口”。

**已完成事项**：
1. 确认 `11` 目录里同时存在：
   - `所有线程回归誓言.md`（索引/提炼位）
   - `所有线程回归誓言\*.md`（真实线程回包）
2. 读取并提炼六份真实回包，锁定：
   - `spring-day1 -> D:\Unity\Unity_learning\Sunset @ codex/spring-day1-story-progression-001 @ a9c952b7`
   - `NPC -> D:\Unity\Unity_learning\Sunset @ codex/npc-roam-phase2-002 @ 6e2af71b`
   - `farm -> D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-cleanroom001 @ 66c19fa1`
3. 识别出当前真正阻塞点已缩成：
   - shared root 与 `NPC_roam_phase2_rescue` 的 `spring-day1` 字体 dirty 双现场分裂
   - shared root 治理/归档 dirty 尚未形成可执行收口
   - shared root 杂项 `untracked` 尚未裁定
4. 新增并更新：
   - `所有线程回归誓言.md`
   - `共享根目录dirty归属可执行版_2026-03-17.md`
   - `第二批worktree核验表_2026-03-17.md`
   - `总进度与收口清单_2026-03-17.md`
   - `tasks.md`
   - `分发Prompt_2026-03-17.md`

**关键决策**：
- `所有线程回归誓言` 的真实回包载体是目录，不是索引 `.md` 本体。
- 当前已经不是“等三大业务线回包”的阶段，而是“执行 shared root 回 `main` 前的最后几项裁定”的阶段。
- `NPC` 与 `farm` 当前都不需要二轮 carrier 澄清；二轮 prompt 只需要发给 `spring-day1` 去裁定字体双现场差异。

**恢复点 / 下一步**：
- 先发 `spring-day1` 二轮 prompt，裁定 shared root 与 rescue 的字体 dirty 分裂。
- 随后治理线程收口 shared root 自身治理/归档 dirty 与杂项 `untracked`。
- 再推进 shared root 回 `main`、三条分支根目录 branch-only 验证与第二批 worktree 退役。

### 会话 6 - 2026-03-17：`spring-day1` 二轮裁定已执行为真实清尾
**用户目标**：
> 审核 `spring-day1` 的二轮回复，并直接推进下一步，而不是停在“回复已收到”。

**本轮子任务**：
- 核对二轮回复与仓库现场是否一致。
- 导出 shared root 与 `NPC_roam_phase2_rescue` 两处字体 dirty 的证据。
- 真正清掉两处字体 dirty，并把 `11` 阶段文档推进到新的执行态。

**已完成事项**：
1. 实核二轮回复成立：
   - shared root 字体 dirty：`BitmapSong / Pixel / SoftPixel / V2`
   - rescue 字体 dirty：`BitmapSong / Pixel / SDF / V2`
   - 字体库、主场景、NPC Prefab 对 5 个相关资产的引用关系成立
2. 导出证据到：
   - [证据导出_2026-03-17_spring-day1字体双现场](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/证据导出_2026-03-17_spring-day1字体双现场)
3. 已实际丢弃两处字体 dirty，保留已提交版本：
   - shared root 4 个字体 dirty 已清空
   - `NPC_roam_phase2_rescue` 已恢复为 `CLEAN`
4. 已删除误复制残留：
   - `Assets/111_Data/NPC 1.meta`
5. 新增并更新：
   - [spring-day1字体双现场证据与清尾_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/spring-day1字体双现场证据与清尾_2026-03-17.md)
   - [所有线程回归誓言.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/所有线程回归誓言.md)
   - [第二批worktree核验表_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/第二批worktree核验表_2026-03-17.md)
   - [共享根目录dirty归属可执行版_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/共享根目录dirty归属可执行版_2026-03-17.md)

**关键决策**：
- `spring-day1` 不再是 shared root 回 `main` 的主要阻塞项。
- `NPC_roam_phase2_rescue` 当前已 clean，但仍按既定顺序保留到 shared root 回 `main` 且根目录 branch-only 验证完成后再退役。
- 当前剩余阻塞收缩成：
  - shared root 自身治理/归档 dirty
  - `Assets/Screenshots*`
  - `npc_restore.zip`

**恢复点 / 下一步**：
- 继续处理 shared root 自身治理尾巴。
- 裁定 `Assets/Screenshots*` 与 `npc_restore.zip` 的最终去留。
- 然后推进 shared root 回 `main`。

### 会话 7 - 2026-03-17：shared root 已回 `main`，`11` 阶段封板
**用户目标**：
> 在 Git/现场层面的真实收口已经完成后，把 `11` 阶段文档、现行状态入口、父级 memory 和线程 memory 全部同步到终局口径，不要再保留“65%”“shared root 仍冻结”这类旧说法。

**本轮子任务**：
- 用真实 Git 现场重新核对 `11` 的终局事实。
- 把 `11` 阶段从“执行中”改写为“已完成”。
- 把终局状态同步回父级与线程级记忆。

**已完成事项**：
1. 重新核对 shared root 终局现场：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `git status --short --branch` clean
   - `git worktree list --porcelain` 只剩共享根目录
2. 复核根目录 branch-only 检出验证证据：
   - `spring-day1 -> a9c952b7`
   - `NPC -> 6e2af71b`
   - `farm -> 66c19fa1`
3. 新增：
   - [终局快照_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/终局快照_2026-03-17.md)
4. 重写并同步：
   - [tasks.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/tasks.md)
   - [analysis.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/analysis.md)
   - [总进度与收口清单_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/总进度与收口清单_2026-03-17.md)
   - [所有线程回归誓言.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/所有线程回归誓言.md)
   - [第二批worktree核验表_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/第二批worktree核验表_2026-03-17.md)
   - [Sunset当前唯一状态说明_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset当前唯一状态说明_2026-03-17.md)
   - [Sunset现行入口总索引_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset现行入口总索引_2026-03-17.md)
5. 同步更新父级 memory 与线程 memory，使 `11` 的完成状态在治理总入口和当前线程里都可恢复。

**关键决策**：
- `11` 阶段已经完成，不再继续承接新 prompt、新尾巴或新的 shared root 事故。
- `分发Prompt_2026-03-17.md` 现在只保留为过程留档，不再作为现行动作入口。
- 如果后续继续推进 Codex 治理续办，应回到 `09_强制skills闸门与执行规范重构` 或另行立项。

**恢复点 / 下一步**：
- `11` 内无剩余待办。
- 若用户继续推进治理主线，下一步应回到 `09` 的 skills/AGENTS/四件套强约束深化，而不是回头继续往 `11` 塞过程尾巴。

### 会话 8 - 2026-03-17：farm continuation 现状复核与 branch-only 切换口径确认
**用户目标**：
> 回来后先确认当前现场是否已经全部就位、是否还有代办残留，以及我现在对“何时切分支、何时回归 main”的理解是否已经正确。

**本轮子任务**：
- 只读核对 shared root 当前真实状态。
- 只读核对 farm continuation branch 是否仍存在、物理 cleanroom worktree 是否已经退役。
- 明确 branch-only 常态下的切换/回归口径。

**已完成事项**：
1. 复核 shared root 当前实时现场：
   - 工作目录仍为 `D:\Unity\Unity_learning\Sunset`
   - 当前分支为 `main`
   - 当前 HEAD 为 `c525ba12f78988397e4145467445d8f467fe7b2b`
   - `git status --short --branch` 为 clean
2. 复核 farm continuation branch：
   - `codex/farm-1.0.2-cleanroom001` 仍存在，当前指向 `66c19fa17a55afec7bf2e0a2a1c695aa0c7f75d0`
   - `codex/farm-1.0.2-correct001` 仍存在，但只保留事故取证身份
3. 复核 `git worktree list --porcelain`：
   - 当前只剩共享根目录 worktree
   - `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001` 已不再作为挂载中的物理 worktree 现场

**关键决策**：
- `farm-1.0.2-cleanroom001` 已完成“事故 cleanroom -> 普通 continuation branch carrier”的转换，不再被描述为长期工作现场。
- 后续若继续 farm 实现，应在共享根目录 `main` 上先做治理/总览，只在真正进入 farm 代码或场景修改前，检出 `codex/farm-1.0.2-cleanroom001`。
- 完成该分支阶段性目标并同步/合回后，应把共享根目录恢复到 `main`，而不是把 root 长期停在 farm 分支。

**恢复点 / 下一步**：
- 当前从治理角度看，farm continuation 现场已就位，无新增阻断。
- 若用户下一步要继续 farm 开发，最小动作是在 `D:\Unity\Unity_learning\Sunset` 检出 `codex/farm-1.0.2-cleanroom001` 后再进入实现。
