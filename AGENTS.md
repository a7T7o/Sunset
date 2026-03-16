# Sunset 项目 Codex 工作规则

## 1. 文件定位
- 本文件是 Sunset 项目的 Codex 路由层，不重复抄写 `.kiro/steering` 正文。
- Sunset 的唯一正文规则源在 `.kiro/steering/`；本文件只负责告诉 Codex 先读什么、何时更新哪层记忆、哪些任务必须走哪条规则。
- `C:\Users\aTo\.codex\AGENTS.md` 是 Codex 的全局规则文件位置，不是 Sunset 线程记忆文件的存储位置。
- Sunset 项目全部 Codex 线程的记忆总根路径固定为 `D:\Unity\Unity_learning\Sunset\.codex\threads\`。
- `CLAUDE.md`、Claude 迁移文档、History 交接稿可以吸收经验，但当前真正的唯一有效依据仍是 `.kiro/steering` 与活跃工作区文档。
- 除 `Codex`、`Claude`、`Sunset`、`AGENTS.md`、`memory_0.md`、路径、命令、代码标识、Unity 专有类型名等特殊情况外，所有说明文字一律使用中文。

## 2. 默认工作方式
- 默认使用中文输出、中文文档、中文总结。
- 先判断当前任务归属哪个 `.kiro/specs` 工作区；用户给了明确路径时直接使用，不另造工作区。
- 线程记忆文件一律写入 `D:\Unity\Unity_learning\Sunset\.codex\threads\<分组>\<线程名>\memory_0.md` 这一项目内结构，不写到 `C:\Users\aTo\.codex\`。
- 如果当前任务属于长期线程治理或线程进入前核验，先读 `D:\Unity\Unity_learning\Sunset\.codex\threads\线程分支对照表.md` 与当前治理工作区中的用户使用说明。
- 如果任务涉及工作区、记忆、交接、治理、报告、Claude 或 Codex 迁移、历史接手，优先使用 `sunset-workspace-router`。
- 如果任务涉及场景、预制体、检查器、ScriptableObject、序列化引用或验证场景，优先使用 `sunset-scene-audit`。
- 如果任务涉及锐评、审视报告、差异、纠正、是否采纳评审，优先使用 `sunset-review-router`。

## 3. Sunset 任务连续性与主线恢复
- 先判断当前这句用户输入是在继续现有主线、处理中途阻塞，还是明确切换到新主线。
- 只要用户没有明确换线，就默认仍服务于当前活跃工作区的主线目标。
- 明确换线的常见表达包括：`换个问题`、`先不做这个`、`另起一个任务`、`重新开始`、`这个先放下`。
- 类似 `顺便问一句`、`先修一下这个`、`你先看看为什么不行`、`先查下报错`，默认仍服务当前工作区主线，属于阻塞处理或插入式子任务。
- 修工具、修 MCP、修脚本、修规则、查报错、补验证，在 Sunset 项目里默认都属于“阻塞处理”，不是新的业务主线。
- 处理阻塞时，要始终明确“我现在在修什么、它服务于原来的哪个目标、修完回到哪一步”。
- 阻塞处理完成后，优先回到原工作区、原主线、原完成标准，而不是只围绕最后一句输入漂移。
- 如果用户一句话只说“修一下”“看看这个为什么不行”，默认理解为服务当前主线，除非用户明确改题。
- 在每次准备收尾回复时，快速自检四问：
  1. 当前活跃工作区的主线目标是什么？
  2. 本轮是在推进主线，还是只是在清理阻塞？
  3. 我的回复里有没有明确写出主线当前进度，而不是只写阻塞处理结果？
  4. 如果阻塞已解除，我有没有把下一步主线动作说清楚？

## 4. 进入任务后的读取顺序
- 第一步：定位当前活跃工作区。
- 第二步：先读该工作区的 `memory.md`，必要时继续读直接相关的子工作区或父工作区 `memory.md`。
- 第三步：读 `.kiro/steering/README.md` 和 `.kiro/steering/rules.md`，确认当前任务应走的规则入口。
- 第四步：按主题补读对应 steering 文件，再读工作区内的 `requirements.md`、设计稿、任务拆分、评审、报告、指引等文档。
- 第五步：最后再改代码、改场景、改配置、改文档。

## 5. Steering 路由表
- 全局项目规则、中文口径、输出方式：`rules.md`
- 记忆、追加式记录、分卷、继承、同步顺序：`workspace-memory.md`
- 评审、锐评、审视报告、路径 A/B/C：`code-reaper-review.md`
- 场景、预制体、检查器、ScriptableObject、序列化引用：`scene-modification-rule.md`
- 编码风格、region、EventBus、分层约束：`coding-standards.md`
- 存档、GUID、持久化、重建：`save-system.md`
- Git、分支、提交、回退、checkpoint、preflight、`.gitattributes`：`git-safety-baseline.md`
- 文档、规则落地、文档同步：`documentation.md`
- 上下文恢复、长链任务接手：`000-context-recovery.md`
- steering 自身维护、避免双源复制：`maintenance-guidelines.md`
- 领域细则按关键词读取：`items.md`、`so-design.md`、`layers.md`、`systems.md`、`trees.md`、`ui.md`、`animation.md`、`placeable-items.md`、`chest-interaction.md`、`debug-logging-standards.md`、`scene-hierarchy-sync.md`。

## 6. Sunset 的记忆纪律
- 本项目同时存在两层记忆：
  - Codex 线程记忆：`D:\Unity\Unity_learning\Sunset\.codex\threads\<分组>\<线程名>\memory_0.md`
  - 工作区记忆：`.kiro/specs/.../memory.md`
- 任何产生可复用结论的 Sunset 实质性工作，都不能只写在线程记忆里；对应工作区记忆也要落盘。
- 更新顺序固定为：当前子工作区 → 受影响的父工作区 → 当前线程记忆。
- 只读分析如果产生了稳定的治理结论、路由结论、风险判断，也要更新记忆。
- 线程记忆和工作区记忆的每次追加，都应写清：当前主线目标、本轮阻塞或子任务、恢复点或下一步主线动作。
- 如果本轮只完成了阻塞处理而主线尚未完成，收尾时必须同时汇报“阻塞已解除”和“主线下一步”，不能只汇报修复动作本身。
- `900_开篇/spring-day1-implementation` 是落地参考样板：其 `memory.md` 体现追加式记录，其 `requirements.md` 与 `OUT_tasks.md` 体现需求驱动和任务拆解。新工作区可以借鉴这种信息密度，但不必机械复制文件数量。
- 记忆文件中的说明文字默认使用中文，不要把普通叙述写成英文；只有文件名、路径、命令、专有名词或特殊技术名词可以保留原文。

## 7. 风险任务处理
- 本项目的 Git 收尾顺序固定为：先按工作区规则更新工作区记忆，再更新线程记忆，最后执行 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`。
- 任何 Sunset 实质性工作在完成记忆更新后，只要当前改动已经达到可提交状态，Codex 就必须继续执行 Git 安全同步，而不是停在“本地已改完但未提交/未推送”。
- 长期治理 / 总览 / 审计线程默认停留在根目录 `D:\Unity\Unity_learning\Sunset` 的 `main`；长期功能线程也默认先从根目录进入，再按真实任务切到对应 `codex/` 分支；`worktree` 只保留给高风险隔离、故障修复与特殊实验。
- 进入任一长期线程前，先核验两件事：当前工作目录、`git branch --show-current`；UI 中残留的分支提示不能替代真实 Git 状态。
- 治理任务若留在 `main`，只允许使用 `git-safe-sync.ps1 -Action sync -Mode governance`，并通过 `-IncludePaths` 明确带上本轮受影响的业务记忆或线程记忆。
- 如果当前线程已经位于某个 `codex/` 分支，且本轮只是顺手修治理规则或治理文档，不必为了同步治理文件强行切回 `main`；此时改用 `git-safe-sync.ps1 -Action sync -Mode task -IncludePaths ...`，只白名单提交本轮治理文件。
- 真实实现任务若准备从 `main` 进入代码或场景修改，必须先执行 `git-safe-sync.ps1 -Action ensure-branch -BranchName codex/...`；只有在工作树干净、基线同步时才允许创建任务分支。
- 真实实现任务完成后，必须在对应 `codex/` 分支上执行 `git-safe-sync.ps1 -Action sync -Mode task -ScopeRoots ... -IncludePaths ...`，用显式白名单提交当前任务，不得使用无边界 `git add -A`。
- 如果 `git-safe-sync.ps1` 判断当前不能安全同步，必须明确汇报阻塞点、当前分支、基线 hash 和剩余 dirty 分类；禁止硬提交、硬推送或偷偷跳过 Git 收尾。
- 评审任务先按 `code-reaper-review.md` 判路由；若落到需要出审视报告的路径，先出报告，不要抢先改文件。
- 场景、预制体、检查器、ScriptableObject 任务先审视再修改；当序列化引用、挂载关系或场景影响范围不清晰时，先给证据和判断，再做改动。
- 历史交接文档和 Claude 迁移文档只作为背景与经验库，不替代当前活跃工作区。
- 修改 steering、规则、治理文档时，遵守唯一规则源原则，避免把同一条规则复制到多处导致双源漂移。

## 8. 本文件边界
- 本文件不是第二套 steering。遇到细节判断，以 `.kiro/steering` 正文与活跃工作区文档为准。
- 当规则冲突时，优先级按“用户当前指令 → 更近目录的 `AGENTS.md` → 本文件 → 历史与迁移参考文档”理解。
