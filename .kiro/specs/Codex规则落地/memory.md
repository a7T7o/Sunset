# codex 代办主线记忆

> 历史长卷已保留在 [memory_0.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_0.md)。本卷只保留当前治理续办的活跃摘要、恢复点和下一步。

## 当前主线目标
- 维持 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地` 作为“安全治理与事故恢复层”的单一定位。
- 保护已经完成的 shared root 硬闸机、runbook、分发规范与历史审计口径，避免执行层问题再次回流到本工作区膨胀。
- 与 `共享根执行模型与吞吐重构` 保持清晰边界：本工作区只做安全治理收口，不再承接吞吐与运行时编排主线。

## 当前状态
- **用户视角完成度（治理安全层）**：shared root 失控、错分支写入、无硬闸机的旧问题已基本被压住；执行层问题已正式迁出到新工作区处理。
- **治理续办总盘状态**：`09/10/11/12/20/21/22/23` 的安全治理资产已经堆齐，本工作区当前处于“维持安全基线 + 为新执行层提供边界与事故恢复支撑”的状态。
- **最后更新**：2026-03-19
- **状态**：整工作区方向已重判且已迁出执行层主线；当前工作重点是保持单一事实入口，防止旧摘要继续误导后续线程。

## 当前阶段目录
- `09_强制skills闸门与执行规范重构`
- `10_共享根仓库分支漂移与现场占用治理`
- `11_main-branch-only回归与worktree退役收口`
- `12_治理工作区归位与彻底清盘`
- `20_shared-root现场回正与物理闸机落地`
- `21_第一次唤醒复盘与shared-root分支租约闸门`
- `22_恢复开发分发与回收`
- `23_前序阶段补漏审计与并发交通调度重建`

## 当前稳定结论
- 旧全局 `tasks.md` 不再承接新治理任务；当前治理正文统一进入 `Codex规则落地`，`000_代办/codex` 只保留 TD 镜像与读取入口。
- `03-17` 起，现行入口已经切到新的状态说明与总索引。
- `09` 已完成“项目级启动闸门落地”的第一轮：
  - `sunset-startup-guard` 已创建
  - `Sunset/AGENTS.md` 已写入强制入口
  - Unity / MCP 单实例层三件套已落盘
- `10` 已完成事故事实厘清与承载面找回：
  - 污染分支历史已锁定为：`codex/farm-1.0.2-correct001 @ 11e0b7b4`
  - `farm` 合法 carrier：`codex/farm-1.0.2-cleanroom001 @ 66c19fa1`
  - `NPC` 合法 carrier：`codex/npc-roam-phase2-002 @ 6e2af71b`
  - `spring-day1` clean checkpoint：`codex/spring-day1-story-progression-001 @ a9c952b7`
- `11` 与 `12` 的历史文档结论仍保留，但 `2026-03-18` 的 live 现场已证明：
  - 旧口径无法阻止现场再次失真
  - 因此新的 live 回正与硬闸机工作迁移到 `20`
- 当前 live 事实已经变为：
  - `D:\Unity\Unity_learning\Sunset @ main`
  - `shared-root-branch-occupancy.md` 当前为 `main + neutral`
  - `git worktree list --porcelain` 只剩 shared root
  - `obra/superpowers` 已复审并维持 `rejected-as-is`
  - `C:\Users\aTo\.agents\skills\` 已暴露 `skill-vetter / skills-governor / sunset-startup-guard`
- 第一次唤醒复盘已经证明：
  - `ensure-branch` 还缺 shared root 独占租约闸门
  - 第一次唤醒与第二次准入 prompt 需要拆开
- `21/22/23` 的补漏与重测又证明了另一个更深层的事实：
  - 前半段方向是对的，而且必要；它确实把 Sunset 从 shared root 失控和错位写入里拉了出来
  - 但后半段开始偏航；治理工作区正在被拿来充当运行时调度总线
  - tracked Markdown 被过度用作 queue / receipt / memory / runtime state 介质
  - shared root 单槽位里混入了只读核对、治理写回、回执补填、memory 更新等非必要持槽动作
- `遮挡检查` 一次持槽 11 分钟是关键证据：
  - 这不是单个线程不听话
  - 而是当前模型把“安全治理”与“执行调度”混成了一层，导致安全性上来了、吞吐掉下去了
- 因此本工作区现在的正确定位应改写为：
  - `Codex规则落地` 是“安全治理与事故恢复层”
  - 不是“高吞吐多线程执行层”
- 当前真正需要新解的问题已经变成：
  - 如何缩短 shared root 持槽时间
  - 如何把运行时等待/排队/唤醒状态从 tracked repo 写入里抽离
  - 如何让线程在未拿到 shared root 槽位前，依然能持续做只读准备、方案整理和 checkpoint 规划

## 当前最高优先级
1. 保持 `Codex规则落地` 只承担安全治理、事故恢复与历史审计，不再吸收执行层吞吐需求。
2. 以当前工作区为边界，收紧并固化已经完成的治理安全基线与事故 runbook。
3. 确保所有活入口都能把读者正确导向 `共享根执行模型与吞吐重构`，而不是继续在本工作区内追逐 queue 细节。
4. 继续复核前序阶段中仍会误导读者的旧入口、旧摘要、旧“当前主线”口径，逐步收口到统一事实。

## 当前恢复点
- `20/21/22/23` 的安全治理成果继续有效，但它们已经不能再被误解为“并发执行模型已经成型”。
- 当前恢复点应改成：
  - 本工作区继续维护 hard gate、baseline、事故 runbook
  - 执行层吞吐、持槽压缩、等待态承载面问题统一转入新工作区
  - 后续若只是继续提速开发，不再回到这里追加“新阶段”

## 最近会话

### 会话 10 - 2026-03-19（跳出整个工作区后的方向重判）
**用户目标**：
> 不是继续在 `23` 里补洞，而是彻底跳出 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地` 整个工作区，重新审视“我们现在做的方向到底对不对”。

**已完成事项**：
1. 复核 live Git 现场，确认当前 shared root 仍在 `main @ a5e00520`，但工作树残留 3 份未同步的方向重判记忆。
2. 回读工作区根层 `memory.md / design.md`、`23` 的补漏与重测材料、治理分发规范，以及用户提供的 Gemini 深度对话材料。
3. 明确重判：
   - 前半段方向是对的，解决了 shared root 失控、错分支写入、无法回正、无审计等问题。
   - 后半段开始偏航，把治理工作区越做越像运行时协调总线。
4. 抓到三个结构性证据：
   - 根层 `memory.md` 仍停留在 `21` 口径，说明统一活入口已经失真。
   - tracked Markdown 被过度当成 queue / receipt / runtime state 介质。
   - `遮挡检查` 的长时间持槽证明当前瓶颈已经从“安全”转成“吞吐”。

**关键决策**：
- `Codex规则落地` 应被重新定性为“安全治理与事故恢复层”，不再继续承担高频运行时编排。
- 后续真正的新主线应改题为“执行模型与吞吐重构”，重点解决 shared root 持槽时长、等待状态承载面和执行舱解耦问题。
- 继续在本工作区里叠加 queue / receipt / batch 文档，只会让治理更厚，不会让开发更快。

**涉及文件**：
- [memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory.md)
- [memory_6.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_6.md)
- [memory_3.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/23_前序阶段补漏审计与并发交通调度重建/memory_3.md)
- [memory_6.md](/D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/Codex规则落地/memory_6.md)

**恢复点 / 下一步**：
- 先把这轮方向重判同步进治理根层与线程记忆。
- 然后再决定是否新开执行层重构工作区；在此之前，不再继续把 `23` 当成无限续命的 queue 修补场。

### 会话 11 - 2026-03-19（执行层新主线已正式迁出）
**用户目标**：
> 接受“旧治理工作区不该继续膨胀成执行层”的判断，并要求我不要停在分析，而是直接开始执行，正式落成新的主线工作区。

**已完成事项**：
1. 新建工作区：
   - [共享根执行模型与吞吐重构](/D:/Unity/Unity_learning/Sunset/.kiro/specs/共享根执行模型与吞吐重构)
2. 新建该工作区第一版正文：
   - [requirements.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/共享根执行模型与吞吐重构/requirements.md)
   - [design.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/共享根执行模型与吞吐重构/design.md)
   - [tasks.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/共享根执行模型与吞吐重构/tasks.md)
   - [memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/共享根执行模型与吞吐重构/memory.md)
3. 在旧治理工作区新增迁移说明：
   - [主线迁移说明_执行层改题_2026-03-19.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/主线迁移说明_执行层改题_2026-03-19.md)

**关键决策**：
- 从这一刻起，“如何提升多线程持续推进能力”不再继续记在 `Codex规则落地` 的后续阶段里。
- `Codex规则落地` 保留为安全治理与事故恢复层；执行层问题改由新工作区单独承接。

**恢复点 / 下一步**：
- 接下来应在新工作区内继续完成：
  - 执行层真实瓶颈盘点
  - 线程生命周期定义
  - 持槽时间边界定义

### 会话 12 - 2026-03-19（旧治理工作区活入口已按新边界收口）
**用户目标**：
> 不要只新建执行层工作区，还要把旧治理工作区本身的活入口改到不会继续误导后续线程为止。

**已完成事项**：
1. 回写本工作区根层状态摘要，明确：
   - 执行层问题已经迁出
   - 当前这里只保留安全治理与事故恢复职责
2. 与新工作区同步口径：
   - waiting 线程不再在 `main` 上写 tracked 运行态
   - 治理线程回到“批次入口 + 事后审计”
3. 保留旧阶段资产，但不再把它们叙述成当前主线的继续正文。

**关键决策**：
- `Codex规则落地` 以后可以继续有维护动作，但这些动作只能是安全层维护，不再是吞吐层建设。
- 如果后续再出现“怎么更快开发”的问题，默认先去 `共享根执行模型与吞吐重构`，而不是回到这里续号。

**恢复点 / 下一步**：
- 继续通过治理同步收进 `main`，把新旧边界一起固化为 live 入口事实。

### 会话 13 - 2026-03-19（治理层补入 Draft 沙盒与 post-return 约束）
**用户目标**：
> 不满足于“新执行层有理论框架”，要求继续补治理层入口中仍会把 `main` 再次写脏的真实缺口。

**已完成事项**：
1. 在治理入口层补入 Draft 沙盒规则：
   - waiting / 挂起线程的草稿只允许写入 gitignored `.codex/drafts/<OwnerThread>/`
2. 收紧 `return-main` 后的治理口径：
   - 如果队列仍有 waiting 条目，不再暗示线程可立刻在 `main` 上补写 tracked 复盘
   - 改为先写 Draft 或最小聊天回执，延后到治理窗口再做最小白名单同步
3. 同步修正文档：
   - `AGENTS.md`
   - `shared-root-queue.md`

**关键决策**：
- 这轮不是放松规则，而是避免“shared root 刚释放就被事后复盘再次写脏”的吞吐回退。
- 治理层继续只负责边界和约束，不接回运行时总线职责。

**恢复点 / 下一步**：
- 待验证通过后，将这轮治理边界补丁同步进 `main`。

### 会话 14 - 2026-03-19（治理层第二轮边界补丁已进 main）
**用户目标**：
> 确保 Draft 沙盒与 `return-main` 后的分流规则不只停在本地修改，而是成为 live 治理入口事实。

**已完成事项**：
1. 已把以下治理层文件同步进 `main`：
   - `AGENTS.md`
   - `shared-root-queue.md`
   - `Codex规则落地/memory.md`
2. 已确认 stable launcher 从 `main` 读取到新 canonical script，且当前 preflight 无待同步改动。

**关键决策**：
- 从现在起，治理层对 post-return 的口径正式变成：
  - 队列仍有人等待时，不默认允许立刻在 `main` 上补写 tracked 复盘
  - 优先 Draft / 最小聊天回执，再择机做最小白名单同步

**恢复点 / 下一步**：
- 后续治理主线应直接进入新批次设计与低风险实盘，而不是再回头补旧阶段 prompt。

### 会话 15 - 2026-03-19（新版 smoke test 批次已替代旧 prompt 入口）
**用户目标**：
> 不继续补发旧工作区里没发完的 prompt，而是在当前 live 规则下给出真正可用的新分发批次。

**已完成事项**：
1. 以审稿方式裁定 Gemini 建议为 `Path B`：
   - 方向认可
   - “直接复用旧 4 份 prompt”不采纳
2. 新建根层批次入口：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_04_执行层smoke-test_01.md`
3. 批次现在指向新执行层工作区内的线程专属 prompt 与治理镜像回收卡，不再继续依赖 `23` 下旧 prompt。

**关键决策**：
- 从现在起，旧批次 prompt 只保留历史证据身份；当前 live 分发入口切到 smoke test 01。
- 治理线程仍负责根层分发文件，但线程专属 prompt 已迁到新执行层工作区。

**恢复点 / 下一步**：
- 将本轮新批次同步进 `main`，然后就可以按新入口群发。

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
   - [总进度与收口清单_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/总进度与收口清单_2026-03-17.md)
   - [分发Prompt_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/分发Prompt_2026-03-17.md)
3. 重写现行入口文档：
   - [Sunset当前唯一状态说明_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset当前唯一状态说明_2026-03-17.md)
   - [Sunset现行入口总索引_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset现行入口总索引_2026-03-17.md)
4. 已真实退役第一批纯历史 worktree：
   - `main-reflow-carrier`
   - `NPC`
   - `farm-10.2.2-patch002`
5. 已新增：
   - [所有线程回归誓言.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/所有线程回归誓言.md)（线程回包索引）
   - [共享根目录dirty归属初版_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/共享根目录dirty归属初版_2026-03-17.md)

**关键决策**：
- 当前用户视角完成度仍按 `0%` 计，直到共享根目录回到 `main` 且 worktree 被真正退役。
- 分支是长期 carrier，worktree 只是事故容器。
- `导航检查 / 遮挡检查 / 项目文档总览` 当前都不需要为了这轮 branch-only 回归新建 worktree。

**恢复点 / 下一步**：
- 现在应优先把 branch-only 回归 prompt 发给 `spring-day1 / NPC / farm`。
- 收到三方答复后，继续推进 shared root 清理和第二批 worktree 退役。

### 会话 2 - 2026-03-17（11 封板后的治理恢复点核验）
**用户目标**：在 `11` 已完成的前提下，复核 Sunset 当前真实现场，并确认后续治理应该回到哪条主线继续推进。  
**完成事项**：只读核验 `D:\Unity\Unity_learning\Sunset @ main`、`git worktree list --porcelain` 仅剩 shared root、最新提交为 `dd83ff74`；同时补建 [09_强制skills闸门与执行规范重构/memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/09_强制skills闸门与执行规范重构/memory.md)，避免 `11` 封板后下一阶段入口缺少工作区记忆承接。  
**关键决策**：`11` 继续保持“终局快照层”，不再追加新治理尾巴；后续若继续推进 skills 强制命中、启动闸门、AGENTS/四件套执行约束，统一回到 `09` 续办。  
**涉及文件**：`C:\Users\aTo\.codex\skills\sunset-startup-guard\SKILL.md`、[09_强制skills闸门与执行规范重构/memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/09_强制skills闸门与执行规范重构/memory.md)、[memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory.md)、[memory_0.md](/D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/Codex规则落地/memory_0.md)。  
**恢复点 / 下一步**：继续在 `09` 压实 `sunset-startup-guard` 的 session 可见性、首条 `commentary` 结构命中率，以及“禁止把 worktree 常态化”的稳定闸门口径。

### 会话 3 - 2026-03-17（治理总盘点与未完项分层）
**用户目标**：要一份当前 Sunset 治理与项目进度总盘点，明确哪些阶段已真完成、哪些只是旧勾选未收口、哪些是现在真正还要做的。  
**完成事项**：逐个复核 `000_代办/codex/01-11` 的 `tasks.md`，并结合当前 Git 现场确认：`11` 已真完成、branch-only prompt 不再需要继续分发、当前真正活跃未完主线只剩 `09`；`01/02/07` 各有少量收尾债，`03/10` 则属于被 `11` 覆盖后尚未文档封板的旧待办。  
**关键决策**：后续汇报统一采用三层口径：
1. 已完成并封板：`04/05/06/08/11`
2. 历史已完成但文档待显式降级：`03/10`
3. 当前唯一活跃主线：`09`
**涉及文件**：[09_强制skills闸门与执行规范重构/tasks.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/09_强制skills闸门与执行规范重构/tasks.md)、[memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/09_强制skills闸门与执行规范重构/memory.md)、[memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory.md)、[memory_0.md](/D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/Codex规则落地/memory_0.md)。  
**恢复点 / 下一步**：继续推进 `09` 的剩余 5 项，并择机把 `03/10` 的旧未勾选项做显式封板处理，避免未来再次误判。

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
   - [共享根目录dirty归属可执行版_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/共享根目录dirty归属可执行版_2026-03-17.md)
   - [所有线程回归誓言.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/所有线程回归誓言.md)
   - [第二批worktree核验表_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/第二批worktree核验表_2026-03-17.md)
   - [分发Prompt_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/分发Prompt_2026-03-17.md)

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
2. 为 `11` 新增 [终局快照_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/11_main-branch-only回归与worktree退役收口/终局快照_2026-03-17.md)。
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
- `000_代办/codex` 只保留 TD 镜像与读取入口，不再承接治理正文。
- 当前活跃推进点已经从 `11` 切回到后续治理深化；若用户继续推进规则/skills/四件套，应直接回到 `09` 或新阶段开工。

### 会话 7 - 2026-03-17（12 阶段真实清盘与 MCP 单实例层并入 live 规则）
**用户目标**：在继续彻底清盘的同时，把“代办区误当工作区”的结构性错误彻底纠正，并把 Unity / MCP 单实例冲突补成正式规则层。  
**完成事项**：
1. 新建：
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-occupancy.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-hot-zones.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-log.md`
2. 更新项目与 live 规则入口：
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `基础规则与执行口径.md`
   - `Sunset强制技能闸门与线程回复规范_2026-03-17.md`
   - `Sunset Git系统现行规则与场景示例_2026-03-16.md`
   - `Sunset当前唯一状态说明_2026-03-17.md`
   - `Sunset现行入口总索引_2026-03-17.md`
3. 新建 [外部历史容器退役说明_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/12_治理工作区归位与彻底清盘/外部历史容器退役说明_2026-03-17.md)。
4. 真实删除：
   - `D:\Unity\Unity_learning\Sunset_external_archives`
   - `D:\Unity\Unity_learning\Sunset_backups`
5. 复核当前 shared root 真实 `HEAD` 为 `952a1f23`。
**关键决策**：
- shared root 中性与 Unity / MCP 单实例安全是两层不同闸门。
- `000_代办/codex` 不再承担任何治理正文职责，只保留 TD。
**恢复点 / 下一步**：
- `12` 到此彻底完成。
- 如继续治理，只回到 `09` 深化强制执行面。

### 会话 8 - 2026-03-17（补强 shared root 实时占用、Play Mode 离场纪律与表现层验收）
**用户目标**：
> 继续完成治理主线，同时把“Play Mode 用完必须退回 Edit Mode”和“UI / 气泡 / 字体 / 样式必须兼顾审美与专业度”写进真正会被命中的闸门层。

**完成事项**：
1. 复核当前 Git 事实：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - 最新治理提交：`663af03c`
   - shared root 当前仍带其他线程 dirty，主体是 NPC 业务文件并夹杂少量非治理记忆
2. 更新 repo 内规则：
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-occupancy.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-hot-zones.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\基础规则与执行口径.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset强制技能闸门与线程回复规范_2026-03-17.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前唯一状态说明_2026-03-17.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\steering\ui.md`
3. 更新本机 skill（不进入 repo 提交）：
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\SKILL.md`
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\references\checklist.md`
4. 同步 `09` 阶段设计与任务清单，纳入新增两条硬约束。

**关键决策**：
- `main + branch-only` 是默认工作模型，不是 shared root 实时中性的保证；实时能否切分支必须再看占用文档与 `git status`。
- 进入 Play Mode 做验证的线程，离场前必须先回到 Edit Mode。
- 表现层任务必须按“好看、合理、专业、可读”验收，而不是只看功能通过。

**恢复点 / 下一步**：
- 治理主线仍落在 `09`，但结构性清盘和规则层补强都已基本闭环。
- 当前剩余现场阻塞不在治理正文，而在 shared root 上仍保留的 NPC 相关业务 dirty。

### 会话 9 - 2026-03-18（live 事故复盘推翻旧“已收口”口径）
**用户目标**：
> 不要继续拿旧快照和旧完成度说事，而是基于 live Git 现场、线程回包和并发事故本身做彻底反思，告诉用户为什么又掉回 dirty 循环，以及现在到底该怎样恢复正常开发进度。

**已完成事项**：
1. 重新实核 shared root live 现场：
   - `D:\Unity\Unity_learning\Sunset`
   - `codex/npc-roam-phase2-003`
   - `HEAD = 2ecc2b753ea711557baca09432d0c7e3760cb3f7`
   - 当前仍有 farm 两份 memory dirty，以及 `2026.03.18线程并发讨论` 未跟踪目录
2. 复核 `git worktree list --porcelain`，确认 farm cleanroom worktree 仍存在，说明之前“只剩 shared root、已彻底进入 main-branch-only”的口径并不成立于 live 现场。
3. 基于 `NPC / 导航检测 / 农田交互修复V2 / spring-day1 / 遮挡检查` 回包，锁定这次混线的最短责任链：
   - NPC 占住 shared root 分支后未归还
   - 导航检测继续在 NPC 分支上写入并提交 `2ecc2b75`
   - farm 留下 unrelated dirty
   - spring-day1 因现场不再中性而无法安全重入
4. 明确当前问题本质：不是“并发这个目标本身错了”，而是“我们把规范写成了文档和提醒，却没有把它变成真正会拦人的物理闸门”。

**关键决策**：
- 当前不能再宣称 `11/12` 已完成到可长期依赖的程度；它们在文档层完成过，但 live 现场后来重新失真了。
- 真正的恢复路线应改为：
  1. 立即停止 shared root 的新增写入
  2. 先让当前 owner 归还 shared root
  3. shared root 回到 clean `main`
  4. 再恢复用户要的 `main-common + branch-task + checkpoint-first + merge-last`
- 之后允许并发的，是读、预检、设计和只读取证；真实写入仍必须单写者串行，且到 checkpoint 就归还。
- 治理主线继续落在 `09`，但目标必须收紧成“把规则变成硬阻断”，而不是再堆一轮解释性文档。

**恢复点 / 下一步**：
- 下一阶段优先做“现场回正”，不是继续扩写“已收口”叙事。
- 当 shared root 真正回到 clean `main` 后，再恢复各业务线程的 branch-only 正常推进。

### 会话 10 - 2026-03-18（阶段 20 立项并接管 live 回正主线）
**用户目标**：
> 既然这轮问题已经超出旧阶段边界，就不要再继续往 `09/11/12` 里补洞；要新开一个 `20` 阶段，把新的认知、Gemini 评估、待审核清单和旧阶段迁移标记都落实进去。

**已完成事项**：
1. 新建：
   - [20_shared-root现场回正与物理闸机落地/analysis.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/20_shared-root现场回正与物理闸机落地/analysis.md)
   - [20_shared-root现场回正与物理闸机落地/tasks.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/20_shared-root现场回正与物理闸机落地/tasks.md)
   - [20_shared-root现场回正与物理闸机落地/memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/20_shared-root现场回正与物理闸机落地/memory.md)
2. 在 `20` 中明确写入：
   - live 事故事实
   - 对 Gemini 建议的吸收与保留意见
   - “先文档、后审核、再执行”的当前边界
   - shared root 回正 runbook 与物理闸机改造的待审核任务清单
3. 回写旧阶段迁移标记：
   - `09/tasks.md`
   - `11/tasks.md`
   - `12/tasks.md`
   - `11/memory.md`
   - `12/memory.md`
4. 改写当前根层摘要，把新的治理主线切到 `20`，并停止继续沿用旧的“100% 已收口”叙事。

**关键决策**：
- 从现在起，`20` 是唯一现行治理主线。
- `09/11/12` 继续保留历史结论，但只作基础与证据层，不再承接 live 回正。
- Gemini 的高层判断被吸收入 `20`；具体 Git 手术步骤仍保持“待审核 runbook”，不直接执行。

**恢复点 / 下一步**：
- 等用户审核 `20/tasks.md`。
- 审核通过后，再进入：
  - shared root 回正 runbook 细化
  - `git-safe-sync.ps1` 物理闸机方案输出

### 会话 11 - 2026-03-18（第一波物理闸机落地，现行入口同步纠偏）
**用户目标**：
> 在不越权执行 Git 手术的前提下，先把文档纠偏，然后直接把 `git-safe-sync.ps1` 改成真正会拦人的脚本，并把任务清单标记到位。

**已完成事项**：
1. 实核后确认：Gemini 所假设的“shared root 已人工清回 `main + clean`”并未发生；live 现场仍是 `codex/npc-roam-phase2-003 @ 2ecc2b75`，且 farm cleanroom worktree 仍存在。
2. 在 `scripts/git-safe-sync.ps1` 中落下第一波物理闸机：
   - 强制 `-OwnerThread`
   - `main` 禁止 `task`
   - `task` 必须在 `codex/` 分支
   - `task` 分支必须与 `OwnerThread` 语义匹配
   - `ensure-branch` 目标分支也必须匹配 `OwnerThread`
3. 同步修正活入口文档与规则正文：
   - `AGENTS.md`
   - `git-safety-baseline.md`
   - `基础规则与执行口径.md`
   - `shared-root-branch-occupancy.md`
   - `Sunset当前唯一状态说明_2026-03-17.md`
4. 回写 `20/tasks.md`，明确 B 阶段第一波已完成、A 阶段仍待 runbook。
5. 完成三次脚本自测，验证：
   - 正常 owner + 正常分支可通过 preflight
   - 错 owner + 当前分支会被 preflight 拒绝
   - 错 owner + 错目标分支会被 ensure-branch 直接抛出 FATAL

**关键决策**：
- 现在可以确认：我们已经不再只是“写规则说明”，而是开始把规则写成脚本闸机。
- 但 shared root 现场回正仍然没做，因此当前恢复正常开发的完成度仍不能上调。
- 阶段 `20` 继续保持为唯一现行主线。

**恢复点 / 下一步**：
- 优先输出 A 阶段的人工回正 runbook。
- 然后再决定是否补第二波 shared root owner/lease 闸机。

### 会话 12 - 2026-03-18（第二波最小 shared root 闸机已落地）
**用户目标**：
> 在 shared root 还没回正前，继续把第二波最小闸机和文档入口补齐，不要停在“只有第一波 OwnerThread 校验”的状态。

**已完成事项**：
1. 继续改写 `scripts/git-safe-sync.ps1`，补上 shared root 第二波最小闸机：
   - occupied shared root 必须匹配 `owner_thread + current_branch`
   - shared root 上若仍有 remaining dirty，则 `task` 模式直接阻断
   - shared root 的 `main` 进入 `ensure-branch` 前，占用文档必须先回到 neutral
2. 同步修正文档：
   - `shared-root-branch-occupancy.md`
   - `git-safety-baseline.md`
   - `基础规则与执行口径.md`
   - `AGENTS.md`
   - `20/tasks.md`
3. 自测确认：
   - 当前 shared root 上，正确 owner 也会因 remaining dirty 被拦下
   - 错 owner 会同时触发 branch mismatch 与 shared root owner mismatch
   - 干净 cleanroom 里调用 shared root 最新脚本时不会被误伤
4. 额外识别到一条新治理风险：
   - 历史 worktree 内的 `git-safe-sync.ps1` 仍是旧版副本，已经出现脚本漂移

**关键决策**：
- `20` 现在不再只是“runbook 待写”，而是已经承接了第一波 + 第二波最小闸机。
- 但 shared root 仍未回到 clean `main`，所以当前恢复状态仍不能上调为“正常开发已恢复”。
- 历史 worktree 副本漂移被正式记录为当前反 worktree 常态化的 live 证据之一。

**恢复点 / 下一步**：
- 继续推进 A 阶段人工回正 runbook。
- 若后续还要补自动 claim/release，再留在 `20` 承接，不回流旧阶段。

### 会话 13 - 2026-03-18（A 阶段人工回正 runbook 已就位）
**用户目标**：
> 在第二波最小闸机之后，继续把 A 阶段 runbook 做成可审核、可手工执行的版本，不要只停留在“以后再说”的任务项。

**已完成事项**：
1. 重新核对 live Git：
   - `codex/npc-roam-phase2-003 @ 2ecc2b75`
   - 目标回退点 `c81d1f99`
   - `origin/codex/npc-roam-phase2-003` 当前同样在 `2ecc2b75`
   - `main / origin/main @ 64ff9816`
2. 在阶段 `20` 新增：
   - `runbook.md`
   - 把 staged / unstaged / untracked 的分组 park、`reset --hard + force-with-lease` 推荐路径、保守 `revert` 替代路径、neutral 回填与验收标准全部写进去
3. 在 `20/tasks.md` 中把 A 阶段四项 runbook 任务改成完成。

**关键决策**：
- 当前阶段 `20` 现在已经同时具备：
  - 第一波分支语义闸机
  - 第二波 shared root 最小闸机
  - A 阶段人工回正 runbook
- 但这些都仍属于“准备好恢复手术”，不是“手术已执行完毕”。

**恢复点 / 下一步**：
- 等用户审核 `runbook.md`。
- 审核通过后，再进入用户人工执行的 Git 外科阶段，或继续补自动 claim / release wrapper。

### 会话 14 - 2026-03-18（A 阶段已执行，D 阶段模型已补）
**用户目标**：
> 既然已经批准执行，就直接完成 shared root 回正、继续补 D 阶段文档，并把治理主线推进到可最终同步的状态。

**已完成事项**：
1. 按安全路径执行 A 阶段 Git 外科：
   - 分组 park `stage20-governance-parking`
   - 分组 park `farm-root-dirty-parking`
   - 将 `codex/npc-roam-phase2-003` 从 `2ecc2b75` 回正到 `c81d1f99`
   - 使用 `push --force-with-lease` 回正远端
   - 将 shared root 切回 `main`
2. 恢复治理 stash，不恢复 farm stash。
3. 现在 `git worktree list --porcelain` 只剩 shared root，farm cleanroom 已经在 Git 层退役。
4. 新增 D 阶段文档：
   - `20_shared-root现场回正与物理闸机落地/operating-model.md`
   - 正式写入 `main-common + branch-task + checkpoint-first + merge-last`
5. 更新 live 入口文档，使当前口径变成：
   - shared root 已回到 `main`
   - 当前仍处于 `governance-main-finalizing`
   - 完成最后一次治理同步后才回到最终 neutral

**关键决策**：
- A 阶段已经从“待审核 runbook”升级为“已执行 live 事实”。
- 当前剩余工作不再是业务错位修复，而是治理线程在 `main + governance` 上做最后同步。
- farm cleanroom 若磁盘上仍残留空目录壳体，也不再拥有 Git worktree 身份。

**恢复点 / 下一步**：
- 用新版 `git-safe-sync.ps1 -Mode governance` 完成治理同步。
- 同步后再把 shared root 占用文档收成最终 neutral，并输出全线程唤醒清单。

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
4. 当前正在做最后的 neutral 回填与收尾说明，不再涉及危险 Git 手术。

**关键决策**：
- 阶段 20 现在已经不是“本地准备状态”，而是已经有主干 checkpoint 的 live 治理成果。
- 接下来只需要再做一次很小的 finalizing commit，就能把 shared root 明确收成 `main + neutral`。

**恢复点 / 下一步**：
- 完成 neutral 回填。
- 输出全线程唤醒清单。
- 再审视是否需要另立后续阶段承接 superpower / wrapper 化约束。

### 会话 16 - 2026-03-18（阶段 20 封板与 superpower 本地适配收口）
**用户目标**：
> 把阶段 20 做到真正可验收，不只保留脚本和 runbook，还要把 superpower 复审、本地技能原生暴露、五线程唤醒 prompt 和全局技能层回写一并做完。

**已完成事项**：
1. 复审 `obra/superpowers`，确认：
   - 可吸收“先过技能闸门再行动”的方法论
   - 不接受其 `worktree-first` 与私有 Skill 工具前提
   - 继续维持 `rejected-as-is`
2. 把本地闸门技能暴露到 `C:\Users\aTo\.agents\skills\`：
   - `skill-vetter`
   - `skills-governor`
   - `sunset-startup-guard`
3. 在 `20` 内新增五线程复工清单：
   - `wakeup-prompts.md`
4. 同步更新：
   - 阶段 `20` 记忆
   - 根层 `Codex规则落地/memory.md`
   - 线程 `memory_0.md`
   - 全局 `global-skill-registry.md`
   - 全局 `global-learnings.md`
5. 核查仓库外历史容器：
   - `Sunset_external_archives` 不存在
   - `Sunset_backups` 不存在
6. 保留并记录唯一剩余物理尾巴：
   - `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
   - 当前只是被占用的空目录残壳，不再拥有 Git 身份

**关键决策**：
- 阶段 20 到此正式封板。
- 现在的治理基线已经能支撑恢复正常 branch-only 开发。
- 如果后续继续追求自动 claim/release wrapper 或更强的 session 级强制技能命中，应新开阶段承接，不再污染已完成的 `20`。

**恢复点 / 下一步**：
- 当前可直接分发 `wakeup-prompts.md`。
- 业务线程恢复时统一按 `main-common + branch-task + checkpoint-first + merge-last` 执行。

### 会话 17 - 2026-03-18（第一次唤醒复盘后，建议转入 21）
**用户目标**：
> 复盘第一次唤醒的真实回包，判断第一次唤醒为什么没有稳定工作，并明确是否需要新的治理阶段继续补闸机。

**已完成事项**：
1. 读取并复盘 `20/第一次唤醒/` 下五份真实回包。
2. 明确第一次唤醒并非“所有线程都失控”：
   - `NPC` 与 `spring-day1` 基本遵守只读
   - `farm` 提前执行 `ensure-branch`，把 shared root 从 `main` 切到 farm 分支
   - `导航检查` 的只读审计因此被污染
   - `遮挡检查` 结果相对可信
3. 锁定当前剩余缺口不是 `sync` 闸机，而是：
   - shared root 分支切换租约
   - 第一次唤醒与第二次“正式进入 branch-task”的协议拆分
4. 当前 live 仓库状态变为：
   - `main`
   - tracked clean
   - 但新增了未跟踪证据目录：
     - `20_shared-root现场回正与物理闸机落地/第一次唤醒/`

**关键决策**：
- 阶段 20 不回滚，它已经解决了“错分支提交 / 错 owner sync”的硬阻断。
- 但阶段 20 不足以覆盖“branch switch 也是共享写态”的新暴露问题。
- 建议正式立项 `21`，主题聚焦：
  - 第一次唤醒复盘
  - shared root 租约闸门
  - 两阶段唤醒协议

**恢复点 / 下一步**：
- 若用户认可，下一阶段应先把 `第一次唤醒/` 目录纳入治理证据，再在 `21` 内实现新闸机与新 prompt 模型。

### 会话 18 - 2026-03-18（阶段 21 物理闸机与钢印配置已经落地）
**用户目标**：
> 不再只看分析文档，而是把阶段 21 的 shared root 分支租约、`AGENTS.md` 钢印、skill 级默认提示词和可直接分发的双阶段唤醒模板做成真实成品。

**已完成事项**：
1. `git-safe-sync.ps1` 已扩展为：
   - `grant-branch`
   - `return-main`
   - `ensure-branch` 进入前强制核验 shared root grant
2. `shared-root-branch-occupancy.md` 已加入 grant 状态字段与口径。
3. `Sunset/AGENTS.md` 已把“第一次回复先过启动闸门、无 grant 禁止 ensure-branch”提升为文件顶层规则。
4. 本机全局 skill 配置已落地强制 `default_prompt`：
   - `skills-governor`
   - `sunset-startup-guard`
5. 低风险验证通过：
   - `preflight` 能输出 grant 字段
   - 无租约 `ensure-branch` 会被脚本直接 FATAL 阻断

**关键决策**：
- 阶段 21 的重点不是再写新规则说明，而是把 shared root 的“切分支租约”做成物理闸机。
- 后续线程恢复必须改成“两阶段唤醒”：先只读，再 grant 准入。

**恢复点 / 下一步**：
- 用新的双阶段唤醒模板重发线程。
- 若这轮仓库治理改动成功同步，可把阶段 21 视为进入可验收状态。

### 会话 19 - 2026-03-18（回顾 Kiro 规则正文与治理历史，重新压实原始规范）
**用户目标**：
> 在重新唤醒线程期间，先回顾 Kiro 的核心规则与 `Codex规则落地` 历史，重新抓稳用户从一开始就在强调的那些规范，不要再靠“我大概记得”推进。

**已完成事项**：
1. 回读 live 入口与核心规则源：
   - `Sunset当前唯一状态说明_2026-03-17.md`
   - `Sunset现行入口总索引_2026-03-17.md`
   - `基础规则与执行口径.md`
   - `.kiro/steering/README.md`
   - `.kiro/steering/rules.md`
   - `.kiro/steering/workspace-memory.md`
   - `.kiro/steering/git-safety-baseline.md`
   - `.kiro/steering/documentation.md`
   - `.kiro/steering/communication.md`
   - `.kiro/steering/scene-modification-rule.md`
   - `.kiro/steering/code-reaper-review.md`
   - `.kiro/steering/maintenance-guidelines.md`
   - `.kiro/steering/ui.md`
2. 复盘 `Codex规则落地` 的关键阶段演化：
   - `01` 活入口重构
   - `02` skills / AGENTS / 启动闸门
   - `05` 四件套与代办区边界
   - `09` 强制闸门、MCP、PlayMode 离场、样式验收
   - `10/11/12` shared root 与 worktree 退役、工作区归位
   - `20/21` 物理闸机与 shared root 分支租约

**关键决策**：
- 这条治理主线的真实核心不是“多立阶段”，而是不断把用户反复强调的红线从纸面规则改造成物理入口。
- 目前最该继续盯住的残余风险是：仍有部分历史规则只存在于 steering / memory 里，还没完全变成强制闸机。

**恢复点 / 下一步**：
- 后续继续输出“仍是纸面约束的规则差距清单”。
- 业务线程恢复时继续以 `21` 的两阶段唤醒和 grant 准入为唯一口径。

### 会话 20 - 2026-03-18（本轮回顾来源文件已整理为精简清单）
**用户目标**：
> 要求简略汇报“刚刚到底查了哪些文件”。

**已完成事项**：
1. 将本轮查询来源收束为四组：
   - 启动闸门与 companion skill
   - live 入口层
   - steering 正文层
   - `Codex规则落地` 根层与关键阶段
2. 准备以“文件路径 + 一句话用途”形式直接汇报。

**关键决策**：
- 这轮不新增新阶段，也不扩写分析，只保留来源清单和最小用途说明。

**恢复点 / 下一步**：
- 若用户继续追问，再基于这份来源清单展开某组规则的详细复盘。

### 会话 21 - 2026-03-18（紧急插单：shared root 分支租约死锁修复）
**用户目标**：
> 先把当前任务和进度存入代办，再立刻处理 `farm` 实盘测试暴露出的脚本死锁：`grant-branch` 自己写脏 occupancy，随后 `ensure-branch` 又把它当阻断项。

**已完成事项**：
1. 在 TD 镜像层新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\TD_13_阶段21历史回读暂停与分支租约死锁修复.md`
   用于记录原主线暂停点、恢复点和本轮紧急插单。
2. 在正式正文里完成 `git-safe-sync.ps1` 修补：
   - 为 occupancy 增加“合法租约运行态脏改”识别
   - `grant-branch` / `ensure-branch` / `return-main` 改用新的 blocking dirty 过滤逻辑
   - `preflight` 与 remaining dirty 口径同步收敛
3. 顺手补了一个越权口子：
   - 其他线程不能在 `main` 上直接 `return-main` 清掉不属于自己的未消费租约
4. 低风险验证通过：
   - PowerShell 语法无错误
   - live `preflight` 已能把 occupancy 标成 `shared root 租约运行态脏改`，不再误当普通 blocking dirty

**关键决策**：
- 这次 Farm 的 `FATAL` 已正式判定为脚本 Bug，不是线程执行不规范。
- occupancy 不能粗暴地整体降级成 noise；必须是“仅在合法租约运行态下精确豁免”。

**恢复点 / 下一步**：
- 下一步优先不是继续历史回读，而是：
  1. 审核本轮脚本修补
  2. 解除当前 `main + branch-granted` 中间态
  3. 重新实盘验证 Farm 的 `grant -> ensure-branch`
- 原本的阶段 21 历史回读主线已写入 `TD_13`，后续可从那里继续恢复。

### 会话 22 - 2026-03-18（skill 触发显式可见与 trigger log 落地）
**用户目标**：
> 不只要有 skill，还要让 skill 触发对用户显式可见，并建立统一日志，便于后续继续迭代规范。

**已完成事项**：
1. 新增全局 `skill-trigger-log.md` 审计层，并写入首条记录。
2. 在全局 `AGENTS.md`、`global-learning-system.md`、`global-learnings.md`、`global-skill-registry.md` 中补齐“首条 commentary 显式点名 + 收尾补记 trigger log”的统一口径。
3. 收紧 `skills-governor`、`global-learnings`、`sunset-startup-guard` 的 skill 正文与 `agents/openai.yaml`。
4. 更新 `D:\Unity\Unity_learning\Sunset\AGENTS.md`，把这条要求接进 Sunset 项目入口。

**关键决策**：
- 以后不能再把“用了 skill 但用户看不见、事后也查不到”当成可接受状态。
- `skill-trigger-log.md` 被正式定为执行审计层；`global-learnings.md` 继续只承接稳定结论。

**恢复点 / 下一步**：
- 这轮治理子任务收口后，若用户继续当前主线，可继续回到阶段 21 的“纸面规则 vs 物理闸机”差距清单整理。

### 会话 23 - 2026-03-18（Gemini 锐评审核后把主线拉回开发恢复）
**用户目标**：
> 现在最优先的是“全部健康恢复并能正常运作”，因此要求对 Gemini 建议做锐评审核后，把主线拉回 farm 的实盘重测，而不是继续发散新基建。

**已完成事项**：
1. 按 Sunset 锐评规范将 Gemini 建议判为 `路径 B`：
   - 核心方向正确
   - 但“自动化 skill hook”只能先落 `000_代办`，不能立即开新正文阶段
2. 已新增 TD 镜像：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\TD_14_自动化skill触发日志Hook待办.md`
3. 已把当前治理主线重新锚定为：
   - 先验证阶段 21 的 shared root / 租约闸机是否真的可以恢复开发

**关键决策**：
- 当前 clean 现场应该优先用于 `farm` 的 `grant -> ensure-branch` 实盘重测。
- 自动化 hook 属于后续补强，不允许抢占当前恢复窗口。

**恢复点 / 下一步**：
- 向用户直接交付 farm 的阶段二重测 Prompt；若重测成功，再继续按同一模型恢复其他线程。

### 会话 24 - 2026-03-18（治理裁决与 TD_14 已推送，主线回到 farm 重测）
**用户目标**：
> 确保“自动化 hook 先列代办、当前优先恢复开发”不只是对话承诺，而是已经真正固化到仓库，随后再恢复线程开发。

**已完成事项**：
1. `TD_14` 已进入 `main`，但仍只属于 `000_代办` 记录层。
2. 相关工作区记忆与线程记忆已同步更新并推送。
3. 当前仓库 `HEAD` 已到：
   - `98fc19e6`
4. 当前 shared root 仍是 clean `main`，没有残留 grant。

**关键决策**：
- 从现在开始，恢复开发的第一验证动作就是 farm 的 `grant -> ensure-branch`。
- 这说明阶段 21 的收口已经从“规则落地”真正进入“可实盘验证”的状态。

**恢复点 / 下一步**：
- 直接把 farm 阶段二重测 Prompt 交给用户分发。

### 会话 25 - 2026-03-18（修复 grant-branch 的幽灵 dirty 判定）
**用户目标**：
> farm 二阶段重测时，`grant-branch` 在 live `main + neutral + clean` 上仍然报“shared root 当前不干净”；要求立刻查明并修复。

**已完成事项**：
1. 已本地复现 farm 的报错，不再把它视为 farm 线程个例。
2. 已证明真实根因：
   - `Get-StatusEntries = 0`
   - `Get-BlockingStatusEntries = 1 个空对象`
   - 属于 PowerShell 空数组包装缺陷
3. 已在 `git-safe-sync.ps1` 中完成最小补丁：
   - `Get-BlockingStatusEntries`
   - `Get-RemainingDirtyEntries`
   都改为真正返回空数组，不再把空结果包成伪 dirty。
4. 已继续修补第二个链路问题：
   - 当唯一脏改是 occupancy 运行态脏改时，`ensure-branch` / `return-main` 允许受控 `checkout -f`
   - `return-main` 现在恢复 occupancy 到 `HEAD` 基线，不再写出新的 neutral dirty

**关键决策**：
- 这次属于 shared root 物理闸机的第二个脚本 bug。
- 当前先修复并验证，不扩写别的治理功能。
- 如果不把 neutral 收口改成恢复 `HEAD` 基线，就算切分支成功，shared root 最终仍不能 clean 归还。

**恢复点 / 下一步**：
- 先同步脚本补丁到 `main`，再在 clean shared root 上跑 farm 的完整闭环验证。

### 会话 26 - 2026-03-18（阶段22收到导航检查阶段一回包）
**用户目标**：
> 从 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\可分发Prompt` 领取 `导航检查` 的专属 prompt，并完成对第一阶段 prompt 的读取和回应。

**已完成事项**：
1. 对 `导航检查` 线程手工执行了 `sunset-startup-guard` 等价 preflight，并显式按要求点名 `skills-governor`、`sunset-workspace-router`。
2. 读取并执行：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\可分发Prompt\导航检查.md`
3. 只读复核导航线程现场：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `HEAD = 14838753b4ae9b09b2146b92fb3bfdc9ac82b2a0`
   - `git status --short --branch = ## main...origin/main`
4. 将阶段一回包写入：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\线程回收\导航检查.md`
5. 复核 `导航检查` 当前 live 结论仍停在 `1.0.0` 审计基线：
   - `2.0.0整改设计` 不存在
   - `requirements.md / design.md / tasks.md` 不存在
   - 导航核心代码仍保留 `OverlapPointAll / OverlapCircleAll / ClosestPoint / OnRequestGridRefresh` 这些审计关键事实

**关键决策**：
- `导航检查` 阶段一已完成，且已给出“建议进入阶段二”的结论。
- 但阶段二只能按 `grant -> ensure-branch -> branch-only` 进入，不能把 shared root 直接改成导航线程长期现场。
- `导航检查` 的最小阶段二 checkpoint 应先固化 `2.0.0` 设计文档，不直接扩大到 Unity / Play Mode 或热文件写入。

**恢复点 / 下一步**：
- 当前阶段 22 已开始收回真实线程回包，不再停留在 prompt 分发态。
- 若治理批准，下一步为 `导航检查` 发放 `codex/navigation-audit-001` 的阶段二准入。
### 会话 26 - 2026-03-18（阶段22分发执行：NPC阶段一回收）
**用户目标**：要求进入 `22_恢复开发分发与回收/可分发Prompt` 领取 NPC 专属 prompt，并完成第一阶段 prompt 的读取与回应。
**完成事项**：只读核对 shared root live Git、NPC 线程记忆 / 工作区记忆、`shared-root-branch-occupancy.md`、`mcp-single-instance-occupancy.md`、`mcp-hot-zones.md`，并将结果写入 [NPC.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/22_恢复开发分发与回收/线程回收/NPC.md)。
**关键决策**：本轮以 live 事实为准，确认 NPC continuation branch 当前仍为 `codex/npc-roam-phase2-003 @ 7385d123`；阶段一可通过，但因 `branch_grant_state = none` 且 Unity/MCP 仍需 live verify，不能越级进入阶段二写入。
**恢复点 / 下一步**：等待治理线程依据线程回收单裁定 NPC 是否准入阶段二。

### 会话 27 - 2026-03-18（阶段22分发执行：遮挡检查阶段一回收）
**用户目标**：要求进入 `22_恢复开发分发与回收/可分发Prompt` 领取 `遮挡检查` 专属 prompt，并完成第一阶段 prompt 的读取与回应。  
**完成事项**：只读核对 shared root live Git、`shared-root-branch-occupancy.md`、`mcp-single-instance-occupancy.md`、`mcp-hot-zones.md`、遮挡线程记忆、遮挡工作区记忆和当前治理入口；重新核对遮挡主链 live 代码 / 场景 / Prefab 证据；将阶段一结果写入 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\线程回收\遮挡检查.md`。  
**关键决策**：本轮以 live 事实为准，确认 shared root 当前是 `main + neutral + clean`；`遮挡检查` 的旧审计结论在首次 clean preflight 的 `HEAD = 14838753b4ae9b09b2146b92fb3bfdc9ac82b2a0` 与回写收口前 latest live `HEAD = d0c6bb72ae1100b0ef5626685c6cfe1ee6a9d958` 上都仍成立；若进入阶段二，默认 continuation branch 为 `codex/occlusion-audit-001`，但 grant 未发放前不能进入真实写入。  
**恢复点 / 下一步**：等待治理线程依据 `线程回收\遮挡检查.md` 裁定是否准入阶段二；本线程继续保持只读。

### 会话 28 - 2026-03-18（阶段22阶段一全量收件，补治理裁定与统一领取入口）
**用户目标**：阶段一已经全部回收，要求治理线程去“收件箱”接收全部结果，并把“统一群发先去领取专属 prompt”的模式固化成正式流程规范，之后类似群发都按这个模式走。  
**完成事项**：
1. 复核当前 live 现场为 `D:\Unity\Unity_learning\Sunset @ main @ d0c6bb72`，当前 dirty 来自阶段 22 治理回写和线程 memory 回写，不属于 shared root 失控。
2. 正式审阅 5 张阶段一回收卡，并把治理裁定回写进：
   - `线程回收/NPC.md`
   - `线程回收/农田交互修复V2.md`
   - `线程回收/spring-day1.md`
   - `线程回收/导航检查.md`
   - `线程回收/遮挡检查.md`
3. 新增统一群发领取入口：
   - `22_恢复开发分发与回收/可分发Prompt/00_统一群发领取入口.md`
4. 新增阶段一审核汇总：
   - `22_恢复开发分发与回收/阶段一审核结论与阶段二分发建议.md`
5. 修正 5 份专属 prompt 中写死的 `HEAD = 9b14814b`，统一改为“以 live preflight 为准”。
6. 更新阶段 22 `总入口_如何分发与回收.md`、`tasks.md`、`memory.md`，把“统一群发入口 + 固定收件箱 + 串行阶段二”固化为当前标准流程。

**关键决策**：
- 现在的标准群发方式已经不是手工复制每个线程的专属 prompt，而是先群发统一入口，让线程自己去 `可分发Prompt/` 领取同名文件。
- 阶段一 5 条线程全部通过，但阶段二不能一起开。
- 推荐 `导航检查` 作为低风险首个阶段二准入对象；`农田交互修复V2 / spring-day1 / NPC` 共享下一个业务写入槽位；`遮挡检查` 放在导航之后更稳妥。

**恢复点 / 下一步**：
- 如要立即恢复真实开发，先单发 `导航检查` 的阶段二 prompt，做一次 docs-only 的低风险闭环验证。
- 导航闭环通过后，再按用户业务优先级从 `农田交互修复V2 / spring-day1 / NPC` 三者中选一条进入下一槽位。

### 会话 29 - 2026-03-18（导航检查阶段二 docs-only 闭环已完成）
**用户目标**：
> 读取阶段二并开始完成阶段二 prompt 的内容。

**已完成事项**：
1. 在 shared root 上再次核对阶段二前提：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `HEAD = 6b32ac248a67ad9bab37f68d7fab2d8757a28b21`
   - `git status --short --branch = ## main...origin/main`
   - `shared-root-branch-occupancy.md = neutral`
2. 为 `导航检查` 执行：
   - `grant-branch`
   - `ensure-branch`
   到 `codex/navigation-audit-001`
3. 在任务分支上完成 docs-first checkpoint，并推送提交：
   - `71905387`
4. checkpoint 产物：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\requirements.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\design.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\tasks.md`
5. 已执行 `return-main`，shared root 已回到 `main`，且本轮未进入 Unity / MCP / Play Mode。
6. 已在阶段 22 回收卡中补齐阶段二区块：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\线程回收\导航检查.md`

**关键决策**：
- 阶段 22 的“低风险先行”策略已被 live 验证成立。
- `导航检查` 现在不再只是候选线程，而是已经拥有真实 continuation branch：
  - `codex/navigation-audit-001 @ 71905387`
- 后续若继续导航主线，应该回到该分支继续 `T1-T3`，而不是重新在 `main` 上准备一轮 docs checkpoint。

**恢复点 / 下一步**：
- 当前 shared root 已归还，可供下一条线程再次申请阶段二槽位。
- 若用户决定继续让 `导航检查` 向代码整改推进，应直接在 `codex/navigation-audit-001` 上继续，而非重新 grant 新分支。

### 会话 30 - 2026-03-18（用户纠偏批次分发归属，新增治理线程专用协议与 skill）
**用户目标**：
> 明确纠正“统一群发 prompt”不应被当成阶段资产或固定维护文案；它应是治理线程在需要分发 / 收件时按轮次临时生成的批次文件，同时希望把这条要求上提成专门规范和 skill 来约束治理线程自身。

**已完成事项**：
1. 删除阶段 22 中不应长期存在的批次文件：
   - `22_恢复开发分发与回收/可分发Prompt/00_统一群发领取入口.md`
2. 在 `Codex规则落地` 根层新增：
   - `治理线程批次分发与回执规范.md`
3. 更新阶段 22 正文，明确：
   - 阶段目录只保留线程专属 prompt 与固定回收卡
   - 批次分发文件必须由治理线程按轮次在根层临时生成
4. 新建治理线程专用 skill：
   - `C:\Users\aTo\.codex\skills\sunset-governance-dispatch-protocol\SKILL.md`
   - `C:\Users\aTo\.codex\skills\sunset-governance-dispatch-protocol\agents\openai.yaml`
5. 更新 Sunset 入口规则：
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   把“Codex规则落地治理线程的多线程分发 / 收件 / 最小回复格式约束”接成新的强制路由点。

**关键决策**：
- 这类批次分发文件属于治理线程操作协议，不属于阶段内容资产。
- 以后再做多线程分发，我必须先在 `Codex规则落地` 根层生成本轮批次文件，再让线程去对应阶段目录领取专属 prompt。
- 线程聊天回复应继续压缩成最小格式，治理判断以固定回收卡为准。
- 即使导航阶段二 docs-first 已经完成，也仍不能“并行开始所有线程”；真实写入仍保持单写者串行。

**恢复点 / 下一步**：
- 当前 shared root 已归还且 clean，可继续发放下一条阶段二槽位。
- 下一步应按业务优先级在 `农田交互修复V2 / spring-day1 / NPC / 遮挡检查 / 导航继续整改` 中只选一条进入真实写入。

### 会话 31 - 2026-03-19（阶段定位纠偏：当前完成的是入口层，不是并发调度层）
**用户目标**：
> 明确追问“我们现在到底算什么情况”，并指出目标不应是把 shared root 变成单人设施，而应是做成可排队、可等待、可中断恢复、支持多 checkpoint 推进的并发交通系统。

**已完成事项**：
1. 重新按 live 事实复核当前现场：
   - `D:\Unity\Unity_learning\Sunset @ main @ 1f4bc6bb`
   - `git status --short --branch = ## main...origin/main`
   - `shared-root-branch-occupancy.md = main + neutral`
2. 重新对齐阶段 22 的真实产出边界：
   - 已有 `grant-branch / ensure-branch / return-main`
   - 已有 shared root neutral gate、分支语义校验、批次分发 / 回执 / 收件流程
   - `导航检查` 已跑通一次 docs-first 阶段二闭环
3. 正式纠偏口径：当前完成的是“强制入口 + 基本交规 + 最小闭环”，不是用户目标中的“完整并发调度系统”。

**关键决策**：
- 不再把“真实写入当前仍按 single-writer 串行”表述成终局模型。
- 用户要的是“交规 + 排队 + 等候态治理 + 中断消化”，而不是“所有线程永远依次进入”。
- 当前真正缺失的一层应独立成后续治理阶段承接，不继续塞进阶段 22。

**恢复点 / 下一步**：
- 保持阶段 22 已有能力可继续用于当前线程准入与低风险闭环。
- 后续应新开阶段，专门补“并发排队调度与等待态治理”。

### 会话 32 - 2026-03-19（用户要求前序阶段全面审计并新立 23）
**用户目标**：
> 不要直接在旧阶段上继续滑行；必须先对前面所有阶段做排查、总结、补漏，避免旧阶段基础没有收尾、新阶段却误以为前提已成立。同时正式新立项，承接方向变化后的治理主线。

**已完成事项**：
1. 全量扫描 `Codex规则落地` 现有阶段目录，确认当前阶段序号已到 `22`，新阶段可安全接 `23`。
2. 回读 `01-12、20-22` 的任务板与关键记忆摘要，完成第一轮阶段分类：
   - 已完成封板
   - 已完成但带非阻塞 backlog
   - 任务板与 live 现实已分叉、必须补记
3. 新建阶段：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建`
   并落下：
   - `tasks.md`
   - `analysis.md`
   - `memory.md`
4. 给旧阶段补写审计说明，避免误导：
   - `20_shared-root现场回正与物理闸机落地/tasks.md`
   - `21_第一次唤醒复盘与shared-root分支租约闸门/tasks.md`
   - `22_恢复开发分发与回收/tasks.md`
5. 同步修正 `22` 的完成标准勾选，至少回填已被 live 证明完成的项目：
   - 阶段一五线程回收
   - 治理裁定落盘
   - 固定回写路径 / 格式边界
   - 工作区已成为可复用正文，而非一次性对话产物

**关键决策**：
- `23` 现在是新的治理主线。
- `22` 继续保留“恢复开发运营工作区”的职责，不再独自承担“并发交通调度重建”。
- `21` 的大量未勾选框不能再机械理解为“这些基础能力还不存在”；其历史目标大部分已在 `21 -> 22` 之间落地，剩余增强项迁入 `23`。

**恢复点 / 下一步**：
- 继续在 `23` 里完成更细的前序阶段审计结论。
- 再基于审计结果正式设计“排队、等待态、中断消化、多 checkpoint 推进”的并发交通模型。

### 会话 33 - 2026-03-19（审阅 `23/gemini.md`，采纳方向但拒绝立即群发并发抢租约）
**用户目标**：
> 要求我完整阅读 `23_前序阶段补漏审计与并发交通调度重建/gemini.md`，结合当前 `23` 正文重新审视自己的内容，并按审核路径给出结论。

**已完成事项**：
1. 读取并审核：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\gemini.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\steering\code-reaper-review.md`
   - 当前 `23` 的 `tasks.md / analysis.md / memory.md`
2. 结合脚本现状再次核实：
   - `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 当前仍以 `FATAL` 拒绝为主
   - 当前仓库尚不存在真正的 `yield / wait / queue` 自动调度实现
3. 给出审核路径判断：
   - `Path B`

**关键决策**：
- 采纳 Gemini 的方向性纠偏：
  - `single-writer` 不是终局
  - `23` 应承接并发交通调度模型
- 不采纳它“现在就群发所有业务线程并发抢租约”的立即执行建议。
- 原因不是方向错，而是物理调度器、等待态载体、自动唤醒路径还没落地；现在直接群发会让规则先于实现，重新制造混乱。

**恢复点 / 下一步**：
- 继续在 `23` 内把调度层做成正式正文。
- 待队列 / 等待态 / 回执 / 唤醒机制成型后，再决定是否进入真正的并发唤醒实盘。

## 2026-03-19｜阶段 23 进入 queue 入口可运行态

**本轮完成**
- `git-safe-sync.ps1` 已补入 queue runtime 读取兼容性修复，解决了当前 PowerShell 对 `ConvertFrom-Json -Depth` 不支持而导致的运行时崩溃。
- 已完成一次 `request-branch` 最小实测，并稳定得到：
  - `STATUS: LOCKED_PLEASE_YIELD`
- 这说明 `23` 不再只是“写 queue 文档”，而是开始拥有可运行的等待态入口。

**关键判断**
- 当前 Sunset 的治理层状态应更新为：
  - `22` 负责恢复开发运营与批次分发 / 回收
  - `23` 已开始承担 queue / wait / dispatch 的物理落地
- 但当前仅能宣称“等待态入口成立”，不能宣称“完整并发交通调度已完成”。

**恢复点**
- 继续在 `23` 内补完整的唤醒、取消、队首消费与负例矩阵。
- 待这些落地后，再决定是否进入真正的多线程并发唤醒实盘。

## 2026-03-19｜Git 层 queue 闭环已实测完成

**本轮完成**
- `23` 已不再停留在“可以等待”的半成品阶段，而是补齐了：
  - `wake-next`
  - `cancel-branch-request`
  - `requeue-branch-request`
- 已完成一轮 Git 层实盘演习，并在演习后把 shared root 恢复为：
  - `main + clean`
  - runtime queue 空基线
- 已将演习结果和 rollout 矩阵写入 `23`。

**关键判断**
- 现在可以确认：
  - shared root 的 Git 层并发交通基础设施已经具备“申请、等待、重排、唤醒、取消、归还”的闭环
- 但还不能夸大为“全项目所有层都已自动化调度完成”。
- 当前仍保留的关键风险是：
  - 旧任务分支上的脚本漂移
  - Unity / MCP 单实例层的高风险联动调度

**恢复点**
- 下一步若继续推进，不该再重复造基础 queue。
- 真正值得继续补强的是：
  - 旧分支脚本漂移的更彻底解法
  - Unity / MCP 调度层

## 2026-03-19｜阶段 23 已补入稳定 launcher

**本轮完成**
- `Codex规则落地/23_前序阶段补漏审计与并发交通调度重建` 已新增并落地：
  - 仓库外稳定 launcher
  - 路径：`C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1`
- 现行 live 口径已从“默认直接调用仓库内 `scripts/git-safe-sync.ps1`”收紧为：
  - shared root 的 `request-branch / grant-branch / ensure-branch / wake-next / return-main`
  - 默认走稳定 launcher
- 已完成一轮最小烟雾验证：
  - 从仓库外 `cwd` 执行 launcher 的 `preflight`
  - launcher 能正确抓取 `main` 上的 canonical 脚本并执行

**关键判断**
- `23` 的焦点已经从“有没有 queue”转成“入口是否稳定、剩余边界还有哪些”。
- 现在不能再说“旧分支脚本漂移会直接把 live 调度拉回旧版本”，因为默认入口已换成稳定 launcher。
- 但仍不能夸大成：
  - 所有历史分支都已经自动升级
  - Unity / MCP 层已经拥有同等级调度器

**恢复点**
- `23` 后续继续补：
  - 多 checkpoint 持续推进模型
  - Unity / MCP 单实例边界
  - 治理线程批次发放 / 回收协议

## 2026-03-19｜恢复开发前的新 live 阻断已转成根层批次文件

**本轮完成**
- 在准备直接进入“业务线程 live 准入”前，再次复核 shared root 现场。
- 确认当前 live 事实不是 `main + clean`，而是：
  - `main + occupancy neutral`
  - 但 Git working tree 仍有 `Skills和MCP / Steering规则区优化` 线留下的 3 个 dirty
- 因此没有误发业务阶段二 prompt，而是先把这个 blocker 转成正式治理动作：
  - 新增根层批次分发文件：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_01_稳定launcher复工前清场.md`
  - 新增 `Skills和MCP` 专属 prompt 与固定回收卡

**关键判断**
- 现在的 shared root 还不适合直接恢复业务线程 live 准入。
- 这次真正的下一步不是“让业务线程去抢租约”，而是：
  - 先把当前 dirty owner 收口
  - 再进入下一轮业务线程发放

**恢复点**
- 当前用户如果要继续分发，先发根层批次分发文件给 `Skills和MCP`。
- 治理线程收到回收卡并确认 clean 后，再生成下一轮业务线程 batch。

## 2026-03-19｜queue-aware 业务准入批次 01 已落根层

**本轮完成**
- `Skills和MCP` 清场回收完成后，治理线程继续读取：
  - live shared root
  - queue runtime
  - 阶段 22 既有线程回收卡
- 在此基础上，新建并落盘：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_02_queue-aware业务准入_01.md`
- 并为以下线程生成专属 prompt / 回收卡：
  - `NPC`
  - `农田交互修复V2`
  - `导航检查`
  - `遮挡检查`

**关键判断**
- 现在 shared root 已经可以恢复业务线程 live 准入，但本轮仍要克制地分层：
  - 先放 4 条适合先拿 Git 槽位、先做低风险 checkpoint 的线
  - `spring-day1` 暂缓到下一轮 `Unity/MCP-aware` 准入
- 这轮正式把业务线程准入模型切成：
  - `request-branch -> GRANTED/ALREADY_GRANTED/LOCKED_PLEASE_YIELD -> ensure-branch -> 最小 checkpoint -> return-main`

**恢复点**
- 用户现在可直接群发新的根层批次分发文件。
- 治理线程收到 4 条回收卡后，再做下一轮调度裁定。

## 2026-03-19｜Skills和MCP 固定回收卡已完成
**用户目标**
- 按 `23_前序阶段补漏审计与并发交通调度重建\2026.03.19_稳定launcher复工前清场` 批次要求，完成 `Skills和MCP` 线程的 live 清场、治理同步与固定回收卡写回。

**本轮完成**
- 只读确认 shared root 当时处于 `main @ cfeedf33`，且 dirty 仅有 `Skills和MCP` 线程遗留的 3 个治理 / 记忆文件。
- 已通过稳定 launcher 执行 `governance sync`，创建并推送提交：`0b41d4ed`。
- 清场后 `git status --short --branch = ## main...origin/main`。
- 已写回回收卡：`D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_稳定launcher复工前清场\线程回收\Skills和MCP.md`。

**关键判断**
- `Skills和MCP` 线程对应的 live blocker 已解除；当前 shared root 可继续作为下一轮业务线程 live 准入的基础现场。

**恢复点**
- 后续治理动作应回到阶段 23 / 当前批次调度视角，继续决定下一轮谁领取准入 prompt。

## 2026-03-19｜queue-aware 业务准入 01：导航检查进入 waiting

**用户目标**
- 领取 `导航检查` 专属 prompt，并按 `2026.03.19_queue-aware业务准入_01` 执行准入与固定回收。

**本轮现场**
- live cwd：`D:\Unity\Unity_learning\Sunset`
- live branch：`codex/npc-roam-phase2-003`
- live HEAD：`7385d1236d0b85c191caff5c5c19b08678d1cf80`
- `git status --short --branch`：`## codex/npc-roam-phase2-003...origin/codex/npc-roam-phase2-003`，并带 `M .kiro/locks/shared-root-branch-occupancy.md`

**已完成**
- 读取 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\可分发Prompt\导航检查.md`
- 先用稳定 launcher 按 prompt 原文执行 `request-branch`，实测暴露 optional 参数未转发：`-BranchName` 在 launcher 到 canonical script 之间丢失，返回 `request-branch 必须提供 -BranchName。`
- 为避免退回仓库 working tree 脚本，改用 `main:scripts/git-safe-sync.ps1` 的手工等价 canonical 执行再次申请。
- canonical `request-branch` 返回：
  - `STATUS: LOCKED_PLEASE_YIELD`
  - `TICKET: 3`
  - `QUEUE_POSITION: 2`
  - `REASON: 当前 live 分支是 'codex/npc-roam-phase2-003'，只有 main 大厅才能发放分支租约。`
- 已按 prompt 停止后续动作，未执行 `ensure-branch` / `task sync` / `return-main`，并把结果写入固定回收卡：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\线程回收\导航检查.md`

**关键判断**
- 这次阻塞不是 `导航检查` 自身代码问题，而是 shared root 已被 `NPC` 线程占用，当前不具备 `main + neutral` 发租约前提。
- 稳定 launcher 的 optional 参数转发存在真实缺口，至少影响 `request-branch` 这类需要 `-BranchName` 的 live 准入动作。

**恢复点 / 下一步**
- 等 shared root 回到 `main + neutral` 且治理层重新唤醒 `导航检查`。
- 下一次继续时仍以 `codex/navigation-audit-001` 为 continuation branch，先做 `NavGrid2D / PlayerAutoNavigator` 的首个非热文件 checkpoint，不碰 `GameInputManager.cs`、`Primary.unity`，也不进入 Unity / MCP / Play Mode。

## 2026-03-19｜smoke-test_01 卡死恢复完成，shared root 已回正
**用户目标**
- 四条执行层 smoke test 都已给出回执，要求治理线程立即开始回收、修正当前卡死现场，并恢复可继续调度的 shared root 基线。

**本轮完成**
- 只读确认本轮真正卡死点不是“线程都完成了”，而是：
  - `导航检查` 已进入 `codex/navigation-audit-001`
  - `NPC / 农田交互修复V2 / 遮挡检查` 正确进入 waiting，并写入 Draft
  - `导航检查` 的 `return-main` 被其他 waiting 线程的 Draft 反向阻断
- 根因已锁定为：旧 continuation branch 对 `.codex/drafts/**` 缺少忽略规则；这不是 Draft 沙盒概念本身失败，而是跨旧分支兼容层缺口。
- 已采用最小恢复动作：
  - 在 repo-local `D:\Unity\Unity_learning\Sunset\.git\info\exclude` 增加 `.codex/drafts/**`
  - 重新执行 `导航检查` 的 `return-main`
- 当前 live 已恢复为：
  - `main @ c09ac560`
  - `git status --short --branch = ## main...origin/main`
  - `shared-root-branch-occupancy.md = main + neutral`
  - queue 中 `导航检查 = completed`，`NPC / 农田交互修复V2 / 遮挡检查 = waiting`
- 已补写执行层工作区下 `smoke-test_01` 的四张治理镜像回收卡，避免回收结论继续只停留在聊天态。

**关键判断**
- `Codex规则落地` 当前对这轮执行层事件的职责仍然是：事故恢复、治理裁定、根层口径同步；不重新夺回执行层主线。
- 现在可以客观地说：shared root 已从这轮 smoke test 的卡死态恢复，但队列仍未消费完，下一步仍要先做治理收口再继续叫号。

**恢复点**
- 先对白名单治理文件执行一次 `governance sync`。
- 同步完成后，再决定是否立刻 `wake-next -> NPC`，或先把专属唤醒 prompt 发出去。

## 2026-03-19｜NPC smoke-test_01 续跑成功，队列继续前推
**用户目标**
- 在 `导航检查` 退场恢复后，继续沿本轮 `smoke-test_01` 队列消费 `NPC`，不要停留在说明层。

**本轮完成**
- 治理线程已执行 `wake-next`，成功向 `NPC` 发放 `codex/npc-roam-phase2-003` 的 shared root grant。
- `NPC` 回执确认：
  - `request-branch = ALREADY_GRANTED`
  - `ensure-branch = 成功`
  - `return-main = 成功`
  - `post_return_evidence_mode = defer-tracked-while-queue-waiting`
- live 复核显示：
  - shared root 已再次回到 `main + neutral`
  - queue 中 `ticket 3 / NPC = completed`
  - 下一位 waiting 变为 `农田交互修复V2`

**关键判断**
- 到这一步，执行层这轮已经连续完成了两次真实闭环：
  - `导航检查`
  - `NPC`
- 这说明当前脚本和队列模型已具备继续消费后续 waiting 条目的可信度，但仍要保持“先最小回收，再继续叫号”的节奏，不把治理脏改夹进业务槽位。

**恢复点**
- 先对白名单治理文件执行一次最小 `governance sync`
- 同步完成后继续 `wake-next -> 农田交互修复V2`

## 2026-03-19｜农田 smoke-test_01 续跑成功，队列只剩遮挡
**用户目标**
- 在 `NPC` 闭环后继续消费本轮队列，不中断，直接推进 `农田交互修复V2`。

**本轮完成**
- 治理线程已执行 `wake-next`，成功向 `农田交互修复V2` 发放 `codex/farm-1.0.2-cleanroom001` 的 shared root grant。
- `农田交互修复V2` 回执确认：
  - `request-branch = ALREADY_GRANTED`
  - `ensure-branch = 成功`
  - `return-main = 成功`
  - `post_return_evidence_mode = defer-tracked-while-queue-waiting`
- 只读核对附加事实：
  - `FarmToolPreview.cs` 与 `PlacementManager.cs` 在位
  - `FarmManager.cs` 当前不存在
- live 复核显示：
  - shared root 已再次回到 `main + neutral`
  - queue 中 `ticket 4 / 农田交互修复V2 = completed`
  - 仅剩 `遮挡检查` 一条 waiting

**关键判断**
- 这说明当前脚本与队列模型已连续支撑三次真实闭环，没有再次出现 `return-main` 阻断。
- 当前最合理的继续动作已经非常明确：先最小 tracked 回收，再推进最后一个 waiting 条目 `遮挡检查`。

**恢复点**
- 先对白名单治理文件执行一次最小 `governance sync`
- 同步完成后继续 `wake-next -> 遮挡检查`

## 2026-03-19｜遮挡 smoke-test_01 续跑成功，整轮执行层 smoke test 收口
**用户目标**
- 在 `农田交互修复V2` 之后继续推进最后一个 waiting 条目 `遮挡检查`，并在其完成后给出整轮 `smoke-test_01` 的真实收口状态。

**本轮完成**
- 治理线程已执行 `wake-next`，成功向 `遮挡检查` 发放 `codex/occlusion-audit-001` 的 shared root grant。
- `遮挡检查` 回执确认：
  - `request-branch = ALREADY_GRANTED`
  - `ensure-branch = 成功`
  - `return-main = 成功`
  - `post_return_evidence_mode = minimal-tracked-allowed`
- live 复核显示：
  - shared root 已再次回到 `main + neutral + clean`
  - queue 中 `ticket 5 / 遮挡检查 = completed`
  - 当前队列 waiting 条目已清空

**关键判断**
- 这意味着本轮执行层 `smoke-test_01` 已经完成完整实盘闭环，而不是停在局部验证：
  - `导航检查`
  - `NPC`
  - `农田交互修复V2`
  - `遮挡检查`
  全部都真实完成了从 waiting / grant 到 `return-main` 的退场链路
- `Codex规则落地` 当前对这轮的职责已经转为：
  - 做最终治理同步
  - 给用户汇报“这套 shared root 执行交通系统在 smoke-test 层面已通过”
  - 然后再裁定下一批是否进入真实开发准入

**恢复点**
- 先对白名单治理文件执行最后一次最小 `governance sync`
- 同步完成后向用户汇报本轮最终验收结果与下一步建议

## 2026-03-19｜真实开发运行口径已从 smoke test 结论中抽出
**用户目标**
- 用户不想只知道“smoke test 跑通了”，而是要知道后续真实开发到底该如何操作、什么时候还需要治理线程、什么时候业务线程可直接跑协议。

**本轮补充结论**
- 治理线程以后主要负责：
  - 批次规划
  - 排队冲突调度
  - 事故恢复
  - tracked 回收与治理同步
- 业务线程以后真正的日常口径是：
  - 并行准备
  - 串行进槽
- 如果 shared root 当前就是 `main + neutral + clean`，单一业务线程并不一定每次都要先等治理线程手工点名；它可以直接按既有协议：
  - `request-branch -> ensure-branch -> checkpoint -> return-main`
- 当前用户给出的业务优先级被治理层接受为合理基线：
  - `NPC` 优先于 `农田`
  - `导航` 优先于 `遮挡`
  - `spring-day1` 可与导航并行准备，但真实入槽要看 Unity / MCP / 热文件风险

**恢复点**
- 后续如果继续推进，不再生成 smoke prompt
- 应转入“真实开发准入批次 01”

## 2026-03-19｜真实开发准入批次 01 已落地为可分发资产
**用户目标**
- 用户正式批准开始给 prompt，并追问“以后是不是所有事都还要先找治理线程”“NPC 和 farm 并行时到底会出现哪些具体情况、如何处理 interrupted / yielded / resumed”。

**本轮完成**
- 已生成本轮治理入口文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_05_真实开发准入批次_01.md`
- 已在执行层工作区生成：
  - `NPC` 真实修复 prompt
  - `农田交互修复V2` 准备态 prompt
  - `农田交互修复V2` 进槽续跑 prompt
  - 固定回收卡

**关键判断**
- 治理线程不是以后每一步都要先审批的人工中转站，但在“多线程同时触碰 shared root”时仍然是最稳的调度层。
- 对当前用户的这组业务优先级，最优第一批不是“双线程同时抢槽”，而是：
  - `NPC` 立即进槽
  - `农田` 并行准备
- 如果用户确实让两个线程同时 request-branch，也只是退化成：
  - 一个 `GRANTED`
  - 一个 `LOCKED_PLEASE_YIELD`
  - 前者退场后再续跑后者
  不是事故，只是吞吐较差。
- 以后真正需要治理线程强介入的高频场景可以具体化为：
  - 多线程并发抢槽
  - 某线程 `return-main` 失败
  - 某线程持槽中断但尚未 sync / 收口
  - 需要统一补写 tracked 回收与治理同步

**恢复点**
- 当前可直接对外分发：
  - `NPC`
  - `农田交互修复V2（准备态）`
- 等 `NPC` 回执成功后，再切到 `农田交互修复V2_进槽续跑`

## 2026-03-19｜真实开发准入批次 01 已收件并转化为下一裁定点
**用户目标**
- 用户已贴回本轮 `NPC` 与 `农田交互修复V2` 的最小回执，希望治理线程正式收件，并告诉他“这一轮到底算推进了什么”。

**本轮完成**
- 已复核 live：`main + neutral + clean`
- 已复核 queue runtime：
  - `ticket 6 / NPC = completed`
  - `ticket 7 / 农田交互修复V2 = completed`
- 已回填两张固定回收卡

**关键判断**
- 批次 01 的真实产出不是新代码，而是两个高价值裁定：
  - `NPC`：问题已从“场景 NPC 报错 / meta 丢失”收敛为“main 还没带上 phase2 载体内容”
  - `农田交互修复V2`：第一检查点已在 continuation carrier 内完成，下一刀将进入 `GameInputManager.cs` 热文件阶段
- 这意味着后续不该重复本轮 prompt，而应各自升级到新的批次类型：
  - `NPC` -> phase2 集成 / 合入批次
  - `农田` -> 热文件专项批次

**恢复点**
- 当前 shared root 仍可继续调度
- 下一步应由治理线程决定：
  - 先发 `NPC` 的集成批次
  - 还是先发 `农田` 的热文件申请批次

## 2026-03-19｜真实开发准入批次 02 已落地
**用户目标**
- 用户明确要求“直接开始”，不要只停留在‘下一步应该做什么’的口头裁定，而是直接把下一轮批次做成可分发资产。

**本轮完成**
- 已生成治理入口：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_06_真实开发准入批次_02.md`
- 已生成执行层批次资产：
  - `NPC_phase2集成清洗.md`
  - `农田交互修复V2_GameInputManager热文件专项.md`
- 已同时生成固定回收卡
- 已用真实锁脚本确认：
  - `GameInputManager.cs` 当前未被锁定
  - `Primary.unity` 当前未被锁定

**关键判断**
- 这轮的正确调度不是两个线程同时发车，而是：
  - `NPC` 先做 phase2 集成清洗
  - `农田` 热文件专项 prompt 先准备好，等 `NPC` 退场再发
- 这样做可以同时满足：
  - shared root 单槽位纪律
  - `GameInputManager.cs` 热文件锁不被过早空占

**恢复点**
- 现在直接给用户发 `NPC` 的批次 02 prompt
- 收到 `NPC` 回执后，再发 `农田` 的热文件专项 prompt

## 2026-03-19｜真实开发准入批次 02 已正式收件
**用户目标**
- 用户贴回批次 02 的 `NPC` 与 `农田` 回执，希望治理线程正式入账并给出后续裁定。

**本轮完成**
- 已复核 live：`main + neutral + clean`
- 已复核 queue runtime：
  - `ticket 8 / NPC = completed`
  - `ticket 9 / 农田交互修复V2 = completed`
- 已复核 `GameInputManager.cs` 当前为 unlocked
- 已回填批次 02 的两张固定回收卡

**关键判断**
- 批次 02 的真实产出仍然不是新代码，而是两条更明确的治理结论：
  - `NPC`：phase2 carrier 完整，但需要 merge-noise 清洗
  - `农田`：第二检查点已在 continuation carrier 中齐备，且热文件通道已验证可通，但也需要 merge-noise 清洗
- 因此两条线现在都不该再继续发“探测型 prompt”，而应直接升级到：
  - carrier 去噪 / 合流批次

**恢复点**
- 当前 shared root 仍可继续调度
- 下一轮应优先生成：
  - `NPC` carrier 去噪 / 合流批次
  - `农田` carrier 去噪 / 合流批次

## 2026-03-20｜真实业务开发批次 03 已建立
**用户目标**
- 用户要求治理线程不要再停留在 smoke / 准入 / 护航 prompt，而是正式开始恢复真实业务开发分发。

**本轮完成**
- 在 `Codex规则落地` 根层新增：`2026-03-20_批次分发_07_真实业务开发批次_03.md`
- 新批次明确只承担“本轮入口路由”职责，不做永久模板。
- 本轮入口已把两条业务线切换为真实开发口径：
  - `NPC`：phase2 交付面收口与 carrier 清洗
  - `农田交互修复V2`：1.0.2 交付面收口与热文件前后两段式推进
- 本轮入口同时明确：
  - waiting 线程仍不得在 `main` 写 tracked 文档
  - `NPC` 不准再只回“branch 里已有内容”
  - `农田` 不准再只回“上轮 checkpoint 已存在”
  - 真正执行前仍以 live preflight 与 queue/grant 结果为准

**关键判断**
- 这次不是继续补“交通测试覆盖面”，而是第一次把治理分发入口改成真实业务开发批次。
- `导航检查`、`spring-day1`、`遮挡检查` 未被写进本轮，不是忘记，而是按用户优先级先恢复 `NPC / 农田` 两条主业务线。
- 由于治理线程本轮在 `main` 上追加了文档和记忆，本轮还差最后一步治理 sync；没做完这一步之前，prompt 虽已就位，但现场还不算可立即发车。

**恢复点**
- 当前下一步不是继续写新 prompt，而是：
  - 同步工作区记忆
  - 执行治理收口
  - 验证 `main + clean`
- 完成后，这份批次入口就可以直接交给用户群发。

## 2026-03-20｜真实业务批次 03 的 NPC 主场景断链事故补洞
**用户目标**
- 用户要求查清 `NPC` 在 batch03 后一度恢复、但 `farm` 收口回到 `main` 后 `Primary` 场景再次出现 `Missing Prefab / Missing Sprite / Missing Animator Controller` 的根因，并彻底恢复正常。

**本轮完成**
- 按 live Git 差异重新复核，确认 `codex/farm-1.0.2-cleanroom001` 对 NPC phase2 运行时路径没有改动，`farm` 不是覆盖源。
- 反查 [Primary.unity](D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity) 中 `001/002/003` 的 prefab GUID，确认 `main` 场景已经依赖 `codex/npc-roam-phase2-003` 才有的 prefab / anim / profile / meta / runtime 代码，但这些资产此前从未真正进入 `main`。
- 已将上述 NPC 运行时资产从 `codex/npc-roam-phase2-003` 精确恢复到当前 `main` 工作树，并静态验证 GUID / fileID 引用链已经重新闭合；Unity Editor 也已完成重编译与 Asset Refresh。

**关键判断**
- `NPC` batch03 不是“写坏了场景”，而是“只做了 carrier 清洗与 branch 提交，没有把 `main` 生产场景实际依赖的运行时资产真正提升到 `main`”。
- 因此治理层后续不能再只看 `changed_paths`、`sync`、`return-main` 和 `carrier` 是否更干净；还必须加一条面向生产场景的主线验收：`main` 若已依赖该 branch 的运行时资产，则必须显式检查 `main-ready`。

**恢复点**
- 下一步先把这次 NPC 主场景修复最小提交到 `main` 并恢复 clean 现场。
- 后续再补真实业务批次 prompt / 回收卡口径，避免再次出现“branch 好了，但 main 场景仍断链”的假通过。

## 2026-03-20｜治理主线重锚：事故反思、dirty 讨论稿与重度 MCP 场景方案已落盘
**用户目标**
- 用户明确指出：治理线程不能修完 NPC 事故就丢掉自己的主线，还要求把事故原因、治理发现、dirty 容忍度思考和重度 MCP 场景搭建线程方案都重新拿起来并落盘。

**本轮完成**
- 在 `共享根执行模型与吞吐重构` 工作区新增三份正式文档：
  - `2026-03-20_事故反思与主线重对齐.md`
  - `2026-03-20_dirty容忍度与清扫推送机制讨论稿.md`
  - `2026-03-20_重度MCP场景搭建线程执行方案.md`
- 同步更新 `requirements.md / design.md / tasks.md`：
  - 把 `carrier-ready / main-ready` 分离写成正式口径
  - 明确 dirty 容忍仍未批准
  - 把重度 MCP 场景搭建线程归类为特殊执行类型
  - 新增待办：后续把 `main-ready` 验收并入真实业务批次模板，以及继续设计 dirty 分级机制

**关键判断**
- 到这一步，治理线程的主线已经重新锚定，不再只是“修一个具体事故”，而是把事故反哺到执行模型正文。
- 当前最重要的新增结论是：后续真实业务批次不能再只验 `carrier-ready`，必须显式回答 `main-ready`。

**恢复点**
- 下一步先把这轮文档同步收口到 `main`。
- 收口完成后，再继续真实业务批次和后续 dirty 机制深挖，而不是继续靠聊天记忆悬空保存这些方向。

## 2026-03-20｜共享根执行模型反哺已正式入库
**用户目标**
> 用户要求我重新拿起治理线程自己的主线，不要只修单个事故，要把原因、发现、dirty 思考和重度场景线程方案都真正落盘。

**本轮完成**
1. 已将 `共享根执行模型与吞吐重构` 工作区的事故反思、dirty 讨论稿、重度 MCP 场景方案及其正文改动，统一走 governance sync 正式推回 `main`。
2. 本轮正式入库提交为：`6d1fde83`（`2026.03.20-03`）。
3. 已确认治理白名单之外仍存在一个其他线程的 tracked memory 改动：`.codex/threads/Sunset/Skills和MCP/memory_0.md`。

**关键决策**
- 治理线程的职责已经明确扩大为两层：
  - 共享根交通与吞吐规则
  - 真实业务是否已经 `main-ready` 的终局验收
- `dirty 容忍 / 清扫推送` 目前只有讨论稿，没有批准成默认机制。
- `Skills和MCP` 的线程记忆 dirty 不应被我静默回退，也不应被我越权打包进本轮治理提交。

**恢复点**
- 当前治理主线已重新落地，不再悬空。
- 还剩两项硬尾项待继续：
  - 下一轮真实业务批次模板补入 `main-ready`
  - 后续独立窗口设计 `dirty` 分级与 takeover 机制

### 补充更新
- 为把 live 基线彻底恢复到可直接开发的状态，我已继续对白名单外唯一残留的 `Skills和MCP/memory_0.md` 做单文件治理收口。
- 收口提交：`1eaac0c8`（`2026.03.20-05`）。
- 当前 `Sunset` live 现场已恢复为 `main + clean + neutral shared root`。

## 2026-03-20｜NPC 事故不是只靠“定责”就能闭环
**用户问题**
> 用户明确追问：这次到底是不是同步规范没到位，`main-ready` 和 `carrier-ready` 为什么不是一句“谁错谁改”就结束。

**关键决策**
- 是，同步规范在“验收口径”上没到位，但不是 `sync` 工具坏了。
- 直接执行责任在 `NPC` 线：它的 phase2 资产此前没有真正落入 `main`，却让主场景已经形成依赖。
- 系统责任在治理线：我此前的回收卡和批次模板没有强制回答 `main-ready`，导致这个执行缺口被误判成“已收口”。

**治理结论**
- 后续必须同时保留两种判断：
  - `carrier-ready`：线程自己的 branch 是否整理完成
  - `main-ready`：用户回到 `main` 后是否立即可用
- 这样不是为了模糊责任，而是为了把“执行没交付完整”和“治理没把闸门问到位”分层看清。

## 2026-03-20｜共享根执行模型工作区已完成目录重组
**用户需求**
> 用户要求我先把 `共享根执行模型与吞吐重构` 工作区从“根目录堆满阶段材料”的状态整理干净，再把当前阶段遗留待办统一落入文档。

**本轮完成**
1. 已将该工作区根目录重组为：
   - `00_工作区导航/`
   - `01_执行批次/`
   - `02_专题分析/`
   - 加四件正文：`requirements / design / tasks / memory`
2. 已新增：
   - `00_工作区导航/当前总览与待办_2026-03-20.md`
3. 已同步修正：
   - `Codex规则落地` 根层批次入口中的路径
   - 历史批次回收卡中的 prompt 路径
   - 执行模型工作区 `memory.md` 中的旧入口引用

**关键决策**
- 当前共享根执行模型工作区正式采用“正文常驻、批次下沉、专题分析下沉、总览单独导航”的结构。
- 当前阶段遗留代办不再散落在聊天和 memory 各处，而是由 `当前总览与待办_2026-03-20.md` 统一承接。

**恢复点**
- 治理主线下一步继续回到：补 `main-ready` 的真实业务批次 04，而不是继续扩散式补文档。

## 2026-03-20｜批次 04 根入口已推回 `main`
**当前主线目标**
- 把 `main-ready` 版真实业务开发批次 04 从“本地已生成”推进到“根入口已正式入库”，并明确当前是否已经恢复到可直接群发的 shared root 基线。

**本轮子任务 / 阻塞**
- 用户要求我直接开始，不要再让批次 04 停留在未同步状态。
- 当前真实阻塞不是批次文件缺失，而是 shared root 上同时存在：
  - 其他线程的 tracked memory dirty
  - 场景搭建线程的未跟踪工作区文档

**本轮完成**
1. 已对白名单治理文件执行一次正式 `governance sync`。
2. 本轮提交：
   - `b6fc4b19`（`2026.03.20-09`）
3. 已正式入库的资产包括：
   - `2026-03-20_批次分发_08_真实业务开发批次_04.md`
   - `共享根执行模型与吞吐重构/01_执行批次/2026.03.20_真实业务开发批次_04/`
   - `共享根执行模型与吞吐重构/tasks.md`
   - `共享根执行模型与吞吐重构/00_工作区导航/当前总览与待办_2026-03-20.md`

**关键判断**
- 从治理层视角看，批次 04 现在已经是 live 入口，不再只是聊天承诺。
- 但 shared root 仍然不是“绝对 clean 可立即发车”：
  - `.codex/threads/Sunset/Skills和MCP/memory_0.md` 仍为 tracked dirty
  - `.kiro/specs/900_开篇/5.0.0场景搭建/**` 仍为 untracked dirty
- 所以当前不能把“批次文件已入库”误说成“shared root 已重新清空”。

**恢复点 / 下一步**
- 批次 04 的治理生成与根入口入库已完成。
- 真正开始第一波分发前，需要由相应线程先把上述 dirty 自行收口，或显式转入它们自己的隔离执行面。
- 之后再继续本线程的下一步：
  - 收件 `NPC / 农田`
  - 做批次 04 第一波裁定
  - 再决定是否切到 `导航 / 遮挡`

### 补充更新
- 当前上述 dirty 已不再留在 shared root：
  - 已为 `Skills和MCP（场景搭建）` 分配：
    - branch：`codex/scene-build-5.0.0-001`
    - worktree：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
- 因此 shared root 现已回到可用的 `main` 入口，batch04 第一波可以开始。
- `spring-day1` 继续排除在 batch04 之外，因为它不是短事务功能线，而是独立集成波次候选。

## 2026-03-20｜场景搭建残留已迁回 worktree，shared root 再次回正
**用户目标**
> 用户要求我不要停在判断上，而是直接把场景搭建线留在 shared root 的残留迁回它自己的执行面，并继续为 `NPC / 农田` 恢复真实分发。

**本轮完成**
1. 已确认阻断 batch04 第一波的 shared root 脏态来自场景搭建线残留：
   - `.codex/threads/Sunset/Skills和MCP/memory_1.md`
   - `.kiro/specs/900_开篇/5.0.0场景搭建/**`
2. 已采用“无损迁入”方式把上述残留移入：
   - `codex/scene-build-5.0.0-001`
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   其中：
   - `5.0.0场景搭建/**` 进入 `shared-root-import_2026-03-20/`
   - `memory_1.md` 已复制到该线程自己的 `.codex/threads/...`
3. 已从 shared root 清除对应残留副本。
4. 额外还清掉了一棵误生成的 URL 编码历史目录：
   - `.kiro/specs/%E5%85%B1%E4%BA%AB...`
   它此前会制造 `Filename too long` warning，并让 `wake-next` 误判 shared root dirty。
5. 当前 live 已回正到：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `git status --short --branch = ## main...origin/main`
   - occupancy 仍为 `main + neutral`

**关键判断**
- 从治理视角，这轮已经证明：场景搭建这种特种线程不能再把任何 tracked / untracked WIP 留在 shared root；它必须彻底依附自己的 `branch + worktree`。
- `NPC` 现在已有未消费 grant，因此不该重发 request；下一步是直接让它消费 grant 进入续跑。
- `农田` 继续排在 `NPC` 之后，等 `NPC return-main` 后再继续 `wake-next`。

**恢复点**
- shared root 已重新可调度。
- 当前主线已切回：`NPC -> 农田` 的 batch04 第一波真实业务续跑。

## 2026-03-20｜batch04-NPC 收口问题复盘：shared root 已归还，但需要向用户拆清“业务事故”与“脚本事故”
**当前主线目标**
- 在 `NPC` docs-only checkpoint 已成功提交并 return-main 后，给出一份面向项目经理的清晰定性：到底是业务线没做好，还是治理脚本没做好。

**本轮完成**
1. 已复核 `NPC` 最终回执：
   - `sync = 成功`
   - `return-main = 成功`
   - checkpoint 提交：`b680cd4b`
2. 已复核当前 shared root：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - occupancy：`neutral-main-ready`
   - queue 当前无人占槽
3. 已复核场景搭建 worktree：
   - `codex/scene-build-5.0.0-001`
   - 保留自己的文档 dirty，不需要 shared root 代管

**关键判断**
- 现在必须明确区分两起问题：
  1. “之前 main 里 NPC 丢 sprite / 资产缺依赖”：
     - 是业务交付验收事故
     - `NPC` 线程与治理验收都负有责任
  2. “本轮 batch04 中 NPC docs-only checkpoint 一度无法 sync / return-main”：
     - 是治理脚本 / occupancy 状态机事故
     - 主责在 `scripts/git-safe-sync.ps1` 对 shared root 收口异常恢复不够鲁棒
- 当前最可信的证据链是：
  - `ensure-branch` 过程中出现系统异常
  - 仓库实际上已经切入 `codex/npc-roam-phase2-003`
  - 但 occupancy 没及时写成 `task-active`
  - 所以后续 `sync` 和 `return-main` 一度按旧态误拦
- 这说明后续治理不仅要补 prompt，还要补脚本层的幂等恢复和异常冻结。

**恢复点 / 下一步**
- 当前 shared root 已恢复，可继续 batch04 第一波后继线程。
- 下一步：
  1. 继续 `农田交互修复V2`
  2. 单开治理修复 `git-safe-sync.ps1`
  3. 再决定第二波 `导航 / 遮挡`

## 2026-03-21｜治理线程已把 occupancy 缺口正式挂回任务清单，且确认 `farm` 应继续真实业务 prompt
**当前主线目标**
- 继续 batch04 第一波，不再把 `farm` 卡在“要不要先等治理修完”这种不必要等待里。

**本轮完成**
1. 已将 occupancy 收口缺口正式立项到：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\tasks.md`
2. 已新增分析稿：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\02_专题分析\2026-03-21\shared-root_occupancy收口脚本缺口与修复计划.md`
3. 已实际尝试执行一次：
   - `wake-next`
   结果确认：shared root 不是被业务线程占用，而是被我本轮尚未同步的治理 dirty 阻断

**关键判断**
- `farm` 继续时应沿用当前 batch04 真实业务 prompt，不回退到 smoke/test prompt。
- 其他线程也不是都必须等治理脚本先修完：
  - Git / 文档 / worktree 内自治推进，可以并行
  - Unity / MCP 写入线程，仍然必须按单写者节奏排队
- 因此治理修复和业务推进是“分层并行”，不是“全部停工等治理”。

**恢复点 / 下一步**
- 先把本轮治理 dirty 同步进 `main`。
- 然后重新 `wake-next`，继续 `农田交互修复V2`。

### 补充更新
- 治理线程随后已完成：
  - 治理 sync：`e39e097a`
  - `wake-next`：成功
- 当前 `farm` 不再是 waiting-unknown，而是已拿到 shared root 分支租约：
  - `OwnerThread = 农田交互修复V2`
  - `Branch = codex/farm-1.0.2-cleanroom001`
- 对它的下一句正式续跑口径已经明确为：
  - `request-branch -> ALREADY_GRANTED -> ensure-branch`

### 会话 10 - 2026-03-21（第二波结果裁定与 scene-build 施工前状态解释）
**用户目标**：理解为什么第二波里看起来只有 `遮挡检查` 真正做了代码改动，并结合场景搭建线程的新回执，判断它现在要的到底是什么。  
**完成事项**：
1. 复核 live 现场，确认 shared root 当前为 `main + neutral`，没有业务线程继续占用。
2. 复核 `导航检查` 提交 `e486b432`：
   - 只包含 `2.0.0整改设计` 四件套与记忆更新
   - 没有触碰业务代码
3. 复核 `遮挡检查` 提交 `295e8138`：
   - 在 `2.0.0整改设计` 四件套之外
   - 额外改了 `Assets/Editor/BatchAddOcclusionComponents.cs`
4. 裁定：`遮挡检查` 不是“做了完整开发”，而是完成了 prompt 允许的首个最小非热文件 code checkpoint；`导航检查` 则落在 docs-first checkpoint。
5. 结合 scene-build worktree 回执，确认其当前状态为：
   - 规划层基本完成
   - 施工前准备已整理成顺序
   - 尚未进入 Unity / MCP / create-only 写态

**关键决策**：
- 第二波两条线程都算“干了活”，只是产物形态不同：
  - `导航检查`：文档型 checkpoint
  - `遮挡检查`：文档 + 一个低风险编辑器脚本 checkpoint
- `遮挡检查` 之所以能带一段代码，是因为它本轮选中的切入点是非热文件、低耦合、可快速验证的 `BatchAddOcclusionComponents.cs`，不等于它已经做完整轮整改。
- scene-build 线程当前不是继续要规划，而是在等待“是否轮到它成为下一位唯一 Unity / MCP 写线程”的裁定。

**恢复点 / 下一步**：
- 当前 batch04 第二波已经实质收口。
- 下一步需要在 `spring-day1`、scene-build 施工写态、以及治理脚本修复之间排优先级。

### 会话 11 - 2026-03-21（scene-build 写态放行前裁定）
**用户目标**：在 `导航检查` 与 `遮挡检查` 都结束后，搞清楚为什么看起来只有 `遮挡检查` 做了代码开发，并判断 scene-build 线程是否可以正式成为下一位 Unity / MCP 写线程。  
**完成事项**：
1. 复核第二波提交差异：
   - `导航检查` 提交 `e486b432`：docs-first checkpoint
   - `遮挡检查` 提交 `295e8138`：docs-first + `Assets/Editor/BatchAddOcclusionComponents.cs`
2. 裁定两条线程都完成了真实工作，只是 checkpoint 类型不同，不存在“只有遮挡干活”的事实。
3. 复核 live 现场：
   - shared root `D:\Unity\Unity_learning\Sunset` 为 `main + neutral`
   - `git status --short --branch = ## main...origin/main`
   - Unity / MCP 记录 `current_claim = none`
4. 结合 scene-build 回执，确认其三个施工前尾项已全部收口，只剩写态裁定。

**关键决策**：
- 当前可以把 scene-build 视为下一位候选 Unity / MCP 写线程。
- 放行口径应当是“允许进入 `SceneBuild_01 -> Grid + Tilemaps` 的 create-only 施工起步”，而不是无限制自由施工。
- 进入前仍需现场复核 Play Mode / Compile / Console，但不再缺治理层批准。

**恢复点 / 下一步**：
- 直接向 scene-build 下发正式写态放行口令。
- 要求其以 `SceneBuild_01 -> Grid + Tilemaps` 为首个施工 checkpoint，完成后先回执再扩展到底稿/结构层。

### 会话 12 - 2026-03-21（shared-root occupancy 收口 bugfix 落地）
**用户目标**：不要再停留在讨论层，而是把 `git-safe-sync.ps1 / shared-root occupancy` 这条治理缺口直接修完并收口。  
**当前主线目标**：修复 batch04 中多次出现的“提交/推送已成功，但 `sync` 尾部或 `return-main` 因 runtime 字段缺失再次报错”的治理 bug。  
**本轮子任务**：在 shared root `main` 上完成脚本补丁、更新治理任务单和分析稿，并准备治理同步。  
**完成事项**：
1. 在 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 补上 active-session 旧结构兼容：
   - 读取旧 runtime 时自动补齐 `source / last_reason / entered_at / last_updated` 等字段
   - 初始化 runtime 时显式写入 `last_reason`
   - `touch` 阶段不再对缺失属性直接赋值
   - 归档历史时补带 `source / last_reason`
2. 在 `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\tasks.md` 将：
   - `19` 标为完成
   - `20` 标为完成
   - `15` 明确保留未完成，不混入本次 bugfix
3. 在专题分析文档中明确：
   - 本次主责在治理工具链
   - 业务线程不应再在系统异常后自行深挖 PATH / ACL / restore
   - `dirty` 放宽仍是后续设计题
4. 已完成本地验证：
   - PowerShell 语法通过
   - legacy active-session 兼容测试通过
   - 从仓库根运行 working tree 版 `preflight -Mode governance` 成功

**关键决策**：
- 这次修的是确定性 bug，不是“顺手放宽 dirty 闸门”。
- `scene-build worktree` 只隔离 Git/WIP，不豁免 Unity / MCP 单写者规则；这个判断继续有效，不因本轮脚本修复发生变化。
- 责任划分保持清晰：
  - 旧 `main-ready` 验收缺口：业务执行责任 + 治理验收责任
  - 本轮 `sync / return-main` 尾部误炸：治理脚本实现主责

**恢复点**：
- 继续完成治理记忆回写与 `sync -Mode governance`。
- 收口后再回到任务 `15`，单独讨论 `dirty` 分级与容忍边界。

### 会话 13 - 2026-03-21（scene-build 已对准 worktree，可继续最小施工窗口）
**用户目标**：接收场景搭建线程最新回执，并确认它现在是否已经具备继续 `SceneBuild_01 -> Grid + Tilemaps` 的现场条件。  
**当前主线目标**：在治理 bugfix 已收口后，明确 scene-build 的 Unity / MCP 写态是否已经真正回到专属 worktree。  
**本轮子任务**：把 scene-build 的最新现场确认纳入治理口径，并给用户一个可直接转发的白话结论。  
**完成事项**：
1. 已接收 scene-build 最新回执：
   - `project_root = D:/Unity/Unity_learning/Sunset_worktrees/scene-build-5.0.0-001`
   - 不在 `Play Mode`
   - 不在 `Compile / Domain Reload` 中间态
2. 已确认本轮 `editor/state` 虽然仍带 `stale_status` 提示，但核心状态字段已经成功返回，且不影响这三项准入判断。
3. 已把治理口径收束为：
   - Unity / MCP 已重新对准 scene-build 专属 worktree
   - 当前可以继续 `SceneBuild_01 -> Grid + Tilemaps`

**关键决策**：
- 这次是“写态已恢复”，不是“仍在等待治理批准”。
- `stale_status` 在这轮只是提示噪音，不构成阻断，因为真正需要的 `project_root / Play Mode / Compile 状态` 都已经明确返回。
- 继续范围仍然限定在之前放行的最小施工窗口，不自动扩展到整轮场景精修。

**恢复点**：
- 现在可以把口径直接转发给 scene-build：按既定窗口继续 `SceneBuild_01 -> Grid + Tilemaps`。
- 等它完成首个施工 checkpoint 后，再决定是否扩展到底稿 / 结构层。

### 会话 14 - 2026-03-21（把旧执行计划与当前实际进度对账）
**用户目标**：对照我之前写过的“先等 farm、再跑第二波、最后修治理脚本”的计划，弄清楚我们现在到底走到哪一步了。  
**当前主线目标**：把 Sunset 治理主线的实际推进位置讲清楚，避免继续拿旧计划当当前状态。  
**本轮子任务**：复核 live 现场、最近治理提交、shared root 状态与 Unity / MCP 占用口径，然后把“哪些已经完成、哪些还没做”整理成当前账本。  
**完成事项**：
1. live 现场已再次确认：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `git status --short --branch = ## main...origin/main`
   - shared root occupancy 为 `main + neutral`
   - `current_claim = none`
2. 已确认我之前那段计划现在已有多项完成，不再是未来时：
   - `farm` 本轮 batch04 已完成并归还 shared root
   - 第二波 `导航检查 / 遮挡检查` 已完成并归还
   - `git-safe-sync.ps1 / occupancy` 收口 bugfix 已完成并进 `main`
   - scene-build 的 Unity / MCP 已对准专属 worktree，已恢复到可继续最小施工窗口
3. 已确认当前最新治理提交链：
   - `601b07db`：occupancy bugfix 收口
   - `f05c8e4d`：scene-build ready-to-continue 结论回写

**关键决策**：
- 你引用的那段计划是“当时的待执行路线图”，现在已经不是当前现场。
- 当前真正还没做完的，不是 batch04 第一、二波，也不是 occupancy bugfix，而是：
  - `dirty` 分级 / 放宽机制设计（任务 `15`）
  - `spring-day1` 的独立进入时机
  - scene-build 首个施工 checkpoint 之后的下一段施工窗口裁定
- 所以主线状态已经从“等 farm 回执”推进到了“业务两波已收口、治理 bugfix 已修完、scene-build 已恢复施工、只剩后续治理设计和施工回执”。

**恢复点**：
- 现在给用户的正式口径应更新为：
  - batch04 第一波：完成
  - batch04 第二波：完成
  - occupancy bugfix：完成
  - scene-build：可继续 `SceneBuild_01 -> Grid + Tilemaps`
  - 后续治理主任务：任务 `15`

### 会话 15 - 2026-03-21（最新待办顺序重排）
**用户目标**：要一份“写给你也写给我”的最新待办顺序，不再混用旧计划、旧阻塞和已经完成的事项。  
**当前主线目标**：把 Sunset 当前真正剩余的任务按执行优先级排出来，让后续协作不再靠聊天回忆。  
**本轮子任务**：结合 live 现场、治理任务单、scene-build 最新回执和 worktree 内旧任务稿，给出一版最新执行顺序。  
**完成事项**：
1. 已再次确认当前 live 基线：
   - shared root = `main + neutral`
   - `git status --short --branch = ## main...origin/main`
   - shared-root Unity / MCP claim = `none`
2. 已确认治理任务单当前真正未完成的核心项只剩：
   - 任务 `14`：持续把 `main-ready` 验收并入后续真实业务批次模板
   - 任务 `15`：`dirty` 分级 / 放宽机制设计
3. 已确认 scene-build 的实时现场比其 `tasks.md` 更靠前：
   - worktree 内旧任务稿仍保留“修正 Unity / MCP 根目录错连”“处理 shared root 误写资产”等旧阻塞
   - 但最新线程回执已经说明：
     - `project_root` 已对准 worktree
     - 不在 `Play Mode`
     - 不在 `Compile / Domain Reload`
     - 当前可以继续 `SceneBuild_01 -> Grid + Tilemaps`
   - 因此后续排程以最新实时回执为准，不再把旧阻塞项当成当前卡点

**关键决策**：
- 当前最新待办顺序应改为：
  1. scene-build 继续首个施工窗口：`SceneBuild_01 -> Grid + Tilemaps`
  2. 等 scene-build 回来首个施工 checkpoint，再裁定是否扩展到底稿 / 结构层
  3. 治理线程单开任务 `15`，设计 `dirty` 分级 / 放宽与 takeover 边界
  4. 继续把任务 `14` 的 `main-ready` 验收问答固化进后续真实业务批次模板
  5. `spring-day1` 继续保持独立波次，暂不混入当前施工节奏
- 当前不是“继续等 batch04 回执”，而是“场景施工继续 + 治理设计并行准备”。

**恢复点**：
- 用户现在可以把 scene-build 视为当前唯一正在推进的 Unity / MCP 写线程。
- 我这边的治理主线则切换为：以任务 `15` 为主，准备下一轮 `dirty` 管控设计。

## 2026-03-21｜任务 15 的 dirty 报告层已进入脚本验证阶段
**用户目标**
> 用户要求我不要只停在讨论或排程，而是继续把治理主线往前推进；在不打断 `scene-build` 的前提下，直接把任务 `15` 做到更接近 live 的层级。

**完成事项**
1. 已在 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 补入首轮 dirty 报告层：
   - `preflight` 现在会为每个 dirty 项输出 `DirtyLevel / OwnerHint / PolicyHint`
   - allowed / remaining 两侧都会给出分级概览
   - remaining dirty 的阻断预览会补带 `<Dx/owner>`
2. 已把该结果回写到：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\tasks.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\design.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\memory.md`
3. 已完成本地验证：
   - 脚本语法通过
   - `preflight -Mode governance` 输出符合预期
   - `preflight -Mode task` 在 `main` 上仍被硬闸门阻断，说明这轮没有偷偷放宽策略

**关键决策**
- 任务 `15` 现在已经从“纯设计稿”推进到了“脚本会报告，但不会放宽”的阶段。
- 这属于治理工具层的可见性增强，不是 shared root 放行策略变更。
- 因此当前对用户和各线程的对外口径仍保持不变：
  - 默认硬闸门不撤
  - 不批准跨线程直接接 raw dirty
  - scene-build 继续独立施工，我不插手它的现场

**恢复点 / 下一步**
- 继续按治理顺序执行一次 `sync -Mode governance`，把本轮脚本报告层补丁正式收进口径层。
- 收口后，治理主线继续留在任务 `15`，下一步优先观察真实 dirty 样本是否支持进一步细化分类边界。

### 会话 16 - 2026-03-21（任务 15 正式起草）
**用户目标**：不再等我发 prompt，而是让我自己接着推进主线；用户特别点明 scene-build 一直在干活，要我看一眼后直接继续。  
**当前主线目标**：在不干扰 scene-build 施工现场的前提下，正式启动治理任务 `15`，把 `dirty` 分级 / 放宽 / takeover 边界写成可执行草案。  
**本轮子任务**：只读确认 scene-build 仍在推进，然后并行产出任务 `15` 的正式设计稿。  
**完成事项**：
1. 已确认 scene-build 仍在其专属 worktree 内推进，且 worktree 内出现新的未跟踪施工产物与文档更新；我未介入它的现场。
2. 已新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\02_专题分析\2026-03-21\dirty分级与takeover边界设计稿.md`
3. 设计稿已明确四级 dirty 模型：
   - `D0`：噪音 / ignored runtime
   - `D1`：治理型可收口 dirty
   - `D2`：任务分支内可继续的 owner-dirty
   - `D3`：禁止放行 dirty
4. 设计稿已明确本轮不批准：
   - shared root 带任务 dirty 继续跑
   - 跨线程直接接 raw dirty
5. 已同步更新：
   - `tasks.md` 中任务 `15` 的当前进展
   - `design.md` 对任务 `15` 的正式挂接说明

**关键决策**：
- 当前治理主线正式从“解释旧计划”切换到“推进任务 `15`”。
- 这轮设计的目标是增加分级和报告能力，不是直接撤销硬闸门。
- scene-build 继续是当前唯一实际写 Unity / MCP 的线程；我这边只做治理并行推进。

**恢复点**：
- 下一步优先把这轮任务 `15` 设计稿同步进 `main`。
- 若继续推进任务 `15`，下一子步应先设计脚本如何输出 dirty 等级与 owner 信息。

## 2026-03-21｜任务 15 的真实样本回放已纠正 shared asset / Primary 的误分级
**用户目标**
> 继续推进 Sunset 当前治理主线，不停在“报告层已经做出来”这一步，而是拿真实 dirty 样本去核对分类边界，并把能确认的误差直接修掉。

**完成事项**
1. 已复核 shared root 当前现场仍为：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `git status --short --branch = ## main...origin/main`
   - occupancy = `main + neutral`
2. 已在同一 PowerShell 会话里直接 dot-source working tree 版 `scripts/git-safe-sync.ps1`，完成任务 `15` 的真实样本回放。
3. 已实锤两处报告层偏差：
   - `Primary.unity` 因大小写比较问题被误判成 `D2`
   - 设计稿里应算 `D3` 的共享 Prefab / ScriptableObject / Animator Controller / Sprite meta 仍被误报成 `D2`
4. 已修正 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`：
   - `Test-DirtyHardBlockPath` 改为大小写稳定比较
   - 新增共享资产高风险根与扩展名的 `D3` 命中
   - `Get-DirtyOwnerHint` 对 `Primary.unity / GameInputManager.cs` 的热文件提示同步修正
5. 已完成二次样本回放验证：
   - `NPC.prefab / NPC_DefaultRoamProfile.asset / NPC.controller / icon.png.meta / Primary.unity` 现已全部归入 `D3`
   - `HotbarSelectionService.cs` 继续为 `D2`
   - `GameInputManager.cs / ProjectSettings/TagManager.asset` 继续为 `D3`

**关键决策**
- 这轮仍然只修“报告层准确性”，不触碰 shared root 放宽机制本身。
- 对外口径保持不变：
  - 默认硬闸门不撤
  - 不批准跨线程直接接 `raw dirty`
  - `scene-build` 继续独立施工，我不插手它的 worktree / Unity 现场

**恢复点 / 下一步**
- 下一步先完成治理 `sync -Mode governance`。
- 收口后，任务 `15` 继续沿“真实样本 -> 分类边界 -> 是否还需补洞”的顺序推进。

## 2026-03-21｜任务 15 第二轮样本审计已补齐 Scene / Anim / Sprite 与 Hotbar owner 提示
**用户目标**
> 不只做一轮样本回放就停下，而是继续把任务 `15` 的真实边界再扫一遍，看看还有没有别的误分级或 owner 提示盲区。

**完成事项**
1. 已继续回放第二批真实样本：
   - `SceneBuild_01.unity / .meta`
   - 真实 `.anim` clip
   - 真实 sprite 图片文件
   - `HotbarSelectionService.cs`
2. 已再次确认并修正四个盲区：
   - 非 `Primary` scene 文件此前仍误报 `D2`
   - `Assets/100_Anim/*` 下真实 clip 仍误报 `D2`
   - `Assets/Sprites/*` 下真实 sprite 文件仍误报 `D2`
   - `HotbarSelectionService.cs` 的 owner hint 仍误归治理线程
3. 已更新 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`：
   - 所有 `.unity / .unity.meta` 统一报告为 `D3`
   - `Assets/222_Prefabs/*`、`Assets/111_Data/*`、`Assets/100_Anim/*`、`Assets/Sprites/*` 统一按整根高风险资产处理
   - 农田 owner hint 补认 `hotbar` 关键字
4. 已完成二次验证：
   - `SceneBuild_01.unity / .meta` 现已进入 `D3`
   - 真实 `.anim` clip 与 sprite 图片现已进入 `D3`
   - `HotbarSelectionService.cs` 现已正确归属 `农田交互修复V2`

**关键决策**
- 这轮仍然只修报告层，不放宽 shared root 准入。
- 到这一步，任务 `15` 的脚本报告层已经从“第一版能用”推进到“至少对当前已知高风险目录与真实样本基本对齐”。

**恢复点 / 下一步**
- 先完成本轮治理同步。
- 后续继续任务 `15` 时，优先查剩余 owner hint 盲区和目录覆盖盲区。

## 2026-03-21｜任务 15 第三轮样本审计已补齐 spring-day1 与遮挡 owner 误归属
**当前主线目标**
- 继续把治理主线压在任务 `15` 上，专门修 dirty 报告层里还会误导排障判断的 owner hint 盲区。

**本轮子任务 / 阻塞**
- 第二轮样本虽然已经把 `Scene / Anim / Sprite / Hotbar` 修准，但还没核对：
  - `spring-day1` 是否会被 `scene-build` 误吞
  - `Occlusion` 文件名驱动的文件是否会掉回治理线程默认 owner

**本轮完成**
1. 已继续回放第三批真实样本：
   - `.kiro/specs/900_开篇/spring-day1-implementation/*`
   - `Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset`
   - `Assets/Editor/BatchAddOcclusionComponents.cs`
   - `Assets/YYY_Scripts/Service/Rendering/Occlusion*.cs`
   - `Assets/YYY_Tests/Editor/OcclusionSystemTests.cs`
2. 已确认两个真实偏差：
   - `.kiro/specs/900_开篇/*` 旧规则过宽，导致 `spring-day1` 被误报为 `scene-build`
   - `occlusion` 只按目录段匹配，导致遮挡相关文件名被误回退到 `Codex规则落地`
3. 已修正 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`：
   - `scene-build` 只再认 `.kiro/specs/900_开篇/5.0.0场景搭建` 与 `scenebuild`
   - 新增 `spring-day1` owner 识别
   - 遮挡 owner 扩到 `云朵遮挡系统` 与文件名 `occlusion / 遮挡`
4. 已完成回归验证：
   - `spring-day1` 文档与故事对白资产现已正确归属 `spring-day1`
   - 遮挡相关 Editor / Runtime / Test 文件现已正确归属 `遮挡检查`
   - `scene-build` 文档与 `SceneBuild_01.unity` 仍保持正确归属

**关键决策**
- 这轮仍只修报告层的归属提示，不动 shared root 的准入硬闸门。
- 对 `TilemapToSprite.cs` 这类泛工具，本轮选择保持保守，不为了一次修正再引入新的 owner 误报。

**恢复点 / 下一步**
- 先做治理 `sync -Mode governance`，把第三轮样本修正收进口径层。
- 收口后继续任务 `15`，优先找剩余 owner fallback 仍偏粗的路径。

## 2026-03-21｜scene-build 底稿 v1 已形成干净 checkpoint，可继续结构层但仍未拿到 Unity live 验收
**当前主线目标**
- 在不介入 scene-build worktree 现场的前提下，只读裁定它这次 `地图底稿 v1` 回执是否已经达到“允许继续下一施工层”的条件。

**本轮子任务 / 阻塞**
- 用户贴来 scene-build 最新回执，要求我判断：
  - 这次是不是已经形成了可信 checkpoint
  - 下一步能不能从地图底稿继续进入 `结构层`

**本轮完成**
1. 已只读复核 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001` 的 live Git 现场：
   - `branch = codex/scene-build-5.0.0-001`
   - `HEAD = 7b92abe0`
   - `origin/codex/scene-build-5.0.0-001` 已同步到同一提交
   - `git status --short --branch` 为 clean
2. 已核对其工作区文档落盘，确认这轮不是口头完成，而是已把：
   - `SceneBuild_01` 的地图底稿 v1
   - 底稿计数
   - “MCP 仍未 live 验收，只能算 scene YAML 已落盘”的限制
   写回到该 worktree 自己的 `tasks.md / memory.md / thread memory`
3. 已明确保持边界：
   - 我未进入该 worktree 改文件
   - 我未处理 shared root 中残留的 `SceneBuild_01.unity`
   - 我未把这轮判定升级成“Unity / Console 已通过”

**关键决策**
- 这次回执已经满足“可继续下一施工层”的最低治理条件，因为：
  - worktree 内已有真实 Git checkpoint
  - checkpoint 已推到远端
  - 当前工作树 clean，可随时回到该基线
- 但这仍然只是“场景 YAML / 资产层施工 checkpoint”，不是 Unity live 验收通过。
- 因此我给出的正式口径是：
  - 允许 scene-build 继续进入 `结构层`
  - 但后续仍要把 `MCP / Console / Unity live` 缺失当成独立验证债，不得假装已经验收完成

**恢复点 / 下一步**
- scene-build 下一步可继续在同一 worktree 内做 `结构层`。
- shared root 残留 `SceneBuild_01.unity` 仍保持单独待裁定，不混进这轮施工推进。

## 2026-03-21｜恢复开发总控文件与线程级 prompt 已正式落地
**当前主线目标**
- 不再只给用户口头排程，而是把“谁现在能开发、谁必须等待、收到回执后该如何更新”正式落到治理文件里，作为后续发放和更新的唯一入口。

**本轮子任务 / 阻塞**
- 用户明确要求一份彻底的开发路径，覆盖：
  - `NPC`
  - `农田交互修复V2`
  - `spring-day1`
  - `导航检查`
  - `遮挡检查`
  - 以及治理线程自己
- 用户同时要求：后续如果线程回执改变了风险或顺序，治理线程必须直接更新这轮输出文件并告知。

**本轮完成**
1. 已新增当前总控文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\00_工作区导航\恢复开发总控与线程放行规范_2026-03-21.md`
2. 已新增治理入口文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-21_批次分发_09_恢复开发总控与线程放行规范.md`
3. 已新增 `2026.03.21_恢复开发总控_01` 目录，落下：
   - 治理线程职责
   - 统一最小回执格式
   - `scene-build / NPC / 农田交互修复V2 / 导航检查 / 遮挡检查 / spring-day1` 六份当前版 prompt
4. 已把当前治理口径正式固化为：
   - `scene-build` 继续结构层，但仍不是 Unity live 验收通过
   - shared root 业务线程按单槽位串行推进
   - 当前 shared root 推荐顺序：`NPC -> 农田交互修复V2 -> 导航检查 -> 遮挡检查`
   - `spring-day1` 当前只放行集成波次前只读准备，不并入这轮真实开发串行队列

**关键决策**
- 这轮最重要的变化不是“我替用户选了谁先开发”，而是把“以后每次回执回来时我必须更新哪些文件”也一并写进制度。
- 这样后续不会再回到“旧 prompt 还能不能发、现在到底该听哪句话”的聊天漂移状态。

**恢复点 / 下一步**
- 先做治理 `sync -Mode governance`，把总控文件、入口文件和记忆同步进 `main`。
- 同步后，后续任一线程回执如果改变顺序、风险或放行边界，先更新这些文件，再告诉用户新口径。

## 2026-03-21｜治理线程自用 prompt 已补齐，当前总控体系闭环
**当前主线目标**
- 把“治理线程自己靠哪份文件执行”也显式落盘，避免总控体系里只剩别人有 prompt、治理线程只有职责说明。

**本轮子任务 / 阻塞**
- 用户追问“那你发给你自己的呢？”，本质是在指出当前体系还缺治理线程自用执行入口。

**本轮完成**
1. 已新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_恢复开发总控_01\治理线程_当前执行Prompt.md`
2. 已更新总控入口文件与批次入口文件，把治理线程也列入“当前 prompt 入口”。
3. 已更新职责文件，明确：
   - 当前执行入口路径
   - “治理线程_当前执行Prompt.md”也是需要持续维护的 prompt 资产

**关键决策**
- 这不是多建一份文档凑数，而是把“我也要按文件执行”写进制度。
- 以后如果我对队列、风险或边界做了调整，也必须先更新自己的这份 prompt，再更新对外口径。

**恢复点 / 下一步**
- 按治理白名单把这轮补齐动作同步进 `main`。
- 同步后，对用户的口径升级为：所有角色都已有正式执行入口，后续只需要滚动维护，不再补洞。

## 2026-03-21｜当前开发放行 prompt 的语义已补白话说明
**当前主线目标**
- 回应用户对 `可分发Prompt` 目录的真实疑问：这些文件是不是只用来测准备度，还是发过去就直接进入本轮真实工作。

**本轮子任务 / 阻塞**
- 用户明确表示“我没有搞太懂”，说明光有文件还不够，需要给项目经理一个一眼能懂的解释层。

**本轮完成**
1. 已新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_恢复开发总控_01\可分发Prompt\README.md`
2. 已把口径明确写死：
   - `NPC / 农田交互修复V2 / 导航检查 / 遮挡检查 / scene-build` 这几份都属于“直接执行 prompt”
   - `spring-day1` 是唯一例外，当前仍是只读准备 prompt
3. 已同步把这层解释写回总控文件与批次入口文件，避免用户只看入口时继续误会

**关键决策**
- 这次不是调整开发顺序，而是补“项目经理解释层”。
- 当前这些 prompt 的设计就是把“准入 + 最小真实工作 + 收口”合并在一轮里，减少二次派发成本。

**恢复点 / 下一步**
- 连同前面的治理线程自用 prompt，一起做本轮 governance sync。
- 同步后，对用户的白话口径固定为：除了 `spring-day1`，其他这些 prompt 都是发过去就开干。

## 2026-03-21｜治理线程已正式进入执行态，当前发放裁定保持为 NPC 优先
**当前主线目标**
- 把“治理线程 prompt 已写好”推进到“治理线程已按 prompt 实际执行”，并给出当前第一条可发 shared root 线程的正式裁定。

**本轮子任务 / 阻塞**
- 用户明确要求“那你开始吧”，意味着治理线程不能再停在文档准备态，而要正式进入 live 执行。

**本轮完成**
1. 已按治理线程自用 prompt 完成 live 核查：
   - `main`
   - `HEAD = d382c41c`
   - `git status = clean`
2. 已核对 occupancy：
   - shared root 当前为 `neutral-main-ready`
   - `lease_state = neutral`
3. 已核对 Unity / MCP 单实例口径：
   - 当前无人 claim
   - 但 shared root 线程默认仍不因此自动获得 Unity / MCP 权限
4. 已复核总控文件与治理线程 prompt 一致，未见需要调整队列顺序
5. 已形成当前治理裁定：
   - `scene-build` 继续 worktree 施工线
   - shared root 当前下一条仍发 `NPC`
   - 其余 shared root 线程继续等待当前队列顺序

**关键决策**
- 当前没有新的现场信号支持改序。
- 所以这轮真正的治理动作不是“再解释一次规则”，而是正式把当前第一条锁定为 `NPC`。

**恢复点 / 下一步**
- 先做本轮最小 governance sync。
- 同步后，用户可以直接发 `NPC_当前开发放行.md`，而我继续承担收件与后续裁定。

## 2026-03-21｜Unity MCP 服务端已真实恢复，但本会话工具绑定仍未完全刷新
**当前主线目标**
- 判断 scene-build 这条 worktree 线能否把口径从“YAML 兜底”升级为“可尝试恢复 MCP”，并区分“服务端是否真的在跑”和“我这个会话是否已经能直接调用”。

**本轮子任务 / 阻塞**
- 用户重开了 MCP，并提供了 `MCP For Unity` 面板截图与服务端日志，希望确认“是不是其实真的运行了”。

**本轮完成**
1. 已核对用户提供的服务端日志，确认以下 live 事实已经成立：
   - `2026-03-21 15:20:31` 启动 `mcp-for-unity-server`
   - 地址为 `http://127.0.0.1:8888/mcp`
   - `scene-build-5.0.0-001` 插件已注册
   - 已注册 25 个工具
   - 多轮 `POST /mcp`、`GET /mcp`、`DELETE /mcp` 返回 `200/202 OK`
2. 已据此确认：
   - Unity MCP 服务端本身确实在运行
   - Unity 端插件与 HTTP `/mcp` 路径也确实在工作
3. 同时，我这边再次直接调用 `unityMCP` 工具时，仍收到非 Unity 的 HTML 响应，因此当前问题已收敛为：
   - 服务端是真的好着
   - 但我这个聊天会话里的工具绑定 / 路由刷新仍未完全生效
4. 已形成当前治理口径：
   - `scene-build` 可以准备恢复使用 MCP，但应先做一轮最小只读 sanity probe
   - 在我这边工具绑定刷新前，我仍不把它宣称成“本会话已完全恢复 MCP 控制”
   - shared root 仍由 `NPC` 单线占用，不受这条恢复结论影响

**关键决策**
- 这次不能再说“可能是 8080 问题没修好”，因为日志已经证明 `8888/mcp` 正在正常提供服务。
- 当前真正残留的问题不是 Unity 服务端，而是本聊天会话的客户端绑定刷新。

**恢复点 / 下一步**
- 对 scene-build 的操作建议升级为：
  - 可以尝试恢复 MCP
  - 但先做最小只读探针，再决定是否把装饰层升级成 MCP 主流程
- 我这边继续把当前口径保持为：
  - 服务端已恢复
  - 本会话工具直连仍待刷新

## 2026-03-21｜scene-build 与 NPC 的当前 prompt 已按用户真实不满意点重写
**当前主线目标**
- 把用户刚刚给出的强反馈直接变成两条线程的下一轮正式 prompt，而不是继续让用户自己口头转述。

**本轮子任务 / 阻塞**
- scene-build 最新回执虽然技术上 clean，但用户对画面结果明确不接受，认为当前场景缺少对 `Primary` 的学习。
- NPC 最新回执虽然成功收口，但用户验收后仍明确指出：
  - 气泡还是糊脸
  - 位置还是怪
  - NPC 碰撞体 / 穿透完全没到位
  - 还要求 NPC 给出下一步规划

**本轮完成**
1. 已新增 scene-build 新 prompt：
   - `scene-build_装饰层纠偏与Primary参考重做.md`
2. 已新增 NPC 新 prompt：
   - `NPC_气泡位置碰撞体修正与下一步规划.md`
3. 已把总控文件与批次入口中的当前入口改到这两份新 prompt
4. 已明确区分两条线的下一轮要求：
   - scene-build：不是“继续往下堆”，而是先学 `Primary` 再重做装饰层
   - NPC：不是“继续微调气泡代码”，而是把气泡位置、碰撞体和下一步计划一起纳入硬目标

**关键决策**
- 这轮要改的不是队列，而是质量门槛。
- 如果不把“用户真实不满意的点”写进 prompt，线程很容易继续沿着自己以为对的方向空转。

**恢复点 / 下一步**
- 先按治理白名单同步这轮 prompt 更新。
- 同步后，把新 prompt 发给：
  - scene-build
  - NPC

## 2026-03-21｜收件后裁定更新：shared root 下一条改为 farm，scene-build 进入逻辑层
**当前主线目标**
- 接住 NPC 与 scene-build 的最新回执，把这轮真正的 live 状态变化写回总控，而不是继续沿用旧排序。

**本轮子任务 / 阻塞**
- `NPC` 已完成一轮分支 checkpoint 并成功归还 `main`
- `scene-build` 已完成装饰层纠偏 checkpoint，并询问是否进入逻辑层

**本轮完成**
1. 已 live 复核 shared root：
   - `main @ 95b3f793`
   - `git status = clean`
   - occupancy = neutral
2. 已 live 复核 scene-build worktree：
   - `codex/scene-build-5.0.0-001 @ 44afab3f`
   - `git status = clean`
3. 已新增 scene-build 新 prompt：
   - `scene-build_逻辑层继续施工.md`
4. 已更新总控与批次入口：
   - `scene-build` 当前阶段切到 `逻辑层`
   - shared root 当前下一条切到 `农田交互修复V2`
   - `NPC` 不再列为当前开发发放对象，而是列为“已收口但待分支手测”

**关键决策**
- 这轮的交通裁定已经变了：不是继续发 `NPC`，而是该切到 `farm`
- 但 `NPC` 也不是完全结束，它只是从“开发线程”切换成了“待分支验收线程”

**恢复点 / 下一步**
- 先做治理同步。
- 同步后，对用户的直接建议就是：
  - 把 `scene-build_逻辑层继续施工.md` 发给 scene-build
  - 把 `农田交互修复V2_当前开发放行.md` 发给 farm

## 2026-03-21｜恢复开发总控同步完成，当前 live 放行顺序生效
**当前主线目标**
- 把刚更新好的总控排序正式推上 `main`，让后续放行不再依赖口头裁定。

**本轮完成**
1. 已完成治理白名单同步：
   - `main @ 7c6fc1e9`
2. 已正式生效的当前顺序：
   - `scene-build`：继续 `逻辑层`
   - shared root 下一条：`农田交互修复V2`
   - `导航检查`、`遮挡检查`：继续排在 shared root 后续顺位
   - `NPC`：已转入“待分支手测”，不是本轮继续开发对象

**关键决策**
- 这轮的关键不是新增一个 prompt，而是把顺序变更真正推成 live 基线。
- 从现在开始，如果用户要继续推进，治理线程给出的首选动作应是：
  - `scene-build` 进逻辑层
  - `farm` 进入当前开发窗口

**下一步**
- 接用户裁定后，继续收 shared root 下一个线程回执。

## 2026-03-21｜farm 回执入账，NPC 因验收失败改列 shared root 下一条
**当前主线目标**
- 把 `farm` 的真实回执和用户对 `NPC` 的否决一起写回 live 总控，更新下一条该发谁。

**本轮完成**
1. 已确认 `farm`：
   - 本轮 checkpoint 已推送到 `21a5f8df`
   - shared root 已归还到 `main`
   - `carrier_ready = yes`
   - `main_ready = no`
2. 已确认 `farm` 当前不 main-ready 的性质：
   - 不是本轮 checkpoint 失败
   - 而是 continuation branch 自带历史差异 `AGENTS.md`、`scripts/git-safe-sync.ps1`
3. 已根据用户真实验收反馈改判 `NPC`：
   - 不再维持“待手测”
   - 改成“验收失败返工”
4. 已新增正式返工 prompt：
   - `NPC_验收失败返工与live验收闭环.md`
5. 已更新当前 live 顺序：
   - `scene-build` 继续 `逻辑层`
   - shared root 下一条改为 `NPC（验收失败返工）`
   - `导航检查`、`遮挡检查` 顺延

**关键决策**
- 现在不能再把 `NPC` 当成“后续再说”的尾项，因为用户已经给出明确的真实失败证据。
- `farm` 这轮则应视为已收口一刀，不需要立刻接着占 shared root。

**恢复点 / 下一步**
- 现在应发：
  - `scene-build_回读自检与高质量初稿收口.md`
  - `NPC_验收失败返工与live验收闭环.md`

## 2026-03-21｜scene-build 回执入账，逻辑层已收口
**当前主线目标**
- 把 scene-build 的逻辑层完成状态写回 live 总控，避免继续发旧 prompt。

**本轮完成**
1. 已确认 scene-build：
   - `codex/scene-build-5.0.0-001 @ a62b557d`
   - `git status = clean`
   - 逻辑层最小版本 checkpoint 已完成
2. 已确认当前剩余阻塞：
   - 本会话 `unityMCP` 仍读到 `Sub2API` HTML
   - 所以它还不能声称 Unity live 闭环恢复
3. 已新增 scene-build 新 prompt：
   - `scene-build_回读自检与高质量初稿收口.md`
4. 已更新当前口径：
   - scene-build 当前下一阶段 = `回读自检与高质量初稿收口`

**关键决策**
- 这轮不用再让它继续“补逻辑层”。
- 现在该让它做回读、自检、整理和高质量初稿收口。

**恢复点 / 下一步**
- 当前应发给 scene-build 的是：
  - `scene-build_回读自检与高质量初稿收口.md`

## 2026-03-21｜最新回执已入账：NPC 转入 needs-unity-window，scene-build 升级到剧本对齐精修
**当前主线目标**
- 把 `scene-build` 与 `NPC` 的最新回执写回 live 总控，让项目经理下一条该发谁、谁该等待，不再停留在上一轮旧排序。

**本轮完成**
1. 已再次核对 shared root：
   - `D:\Unity\Unity_learning\Sunset @ main @ 26ce1d04`
   - `git status = clean`
   - occupancy = `neutral-main-ready`
2. 已核对 scene-build worktree：
   - `codex/scene-build-5.0.0-001 @ 0a14b93c`
   - `git status = clean`
3. 已确认 `NPC`：
   - 返工 checkpoint 已推送到 `codex/npc-roam-phase2-003 @ 657594a6`
   - shared root 已归还
   - `carrier_ready = yes`
   - `main_ready = no`
   - 当前 blocker = `needs-unity-window`
4. 已新增并切换 scene-build 当前可分发 prompt：
   - `scene-build_高质量初稿后续精修与spring-day1剧本对齐.md`
5. 已同步更新：
   - `恢复开发总控与线程放行规范_2026-03-21.md`
   - `2026-03-21_批次分发_09_恢复开发总控与线程放行规范.md`
   - `治理线程_当前执行Prompt.md`
   - `治理线程_职责与更新规则.md`

**关键决策**
- `NPC` 当前不再是 shared root 下一条；它的问题已经从“需不需要再写分支”切到“缺真正可用的 Unity 验收窗口”。
- `scene-build` 当前不该再发“回读自检与高质量初稿收口”旧 prompt；那一步已经完成，下一步是有边界的后续精修，并要和 `spring-day1` 剧本对齐。
- 当前 shared root 下一条正式改为：
  - `导航检查`
  - 然后 `遮挡检查`

**恢复点 / 下一步**
- 现在给用户的正式口径应是：
  - `scene-build` 发 `scene-build_高质量初稿后续精修与spring-day1剧本对齐.md`
  - shared root 当前发 `导航检查_当前开发放行.md`
  - `NPC` 等 Unity 验收窗口

## 2026-03-21｜恢复开发总控已完成治理 sync，当前放行顺序正式写入 main
**当前主线目标**
- 把上一轮已经改好的恢复开发总控正式推上 `main`，避免项目经理继续依据本地未同步状态分发旧 prompt。

**本轮完成**
1. 已用稳定 launcher 执行治理 `sync`：
   - `OwnerThread = Codex规则落地`
   - `Mode = governance`
2. 已创建并推送提交：
   - `main @ ff90f0be`
   - commit message = `2026.03.21-23`
3. 已复核 live 状态：
   - shared root = `D:\Unity\Unity_learning\Sunset @ main`
   - `git status = clean`
   - occupancy = `neutral-main-ready`

**关键决策**
- `scene-build` 当前生效 prompt 已正式切到：
  - `scene-build_高质量初稿后续精修与spring-day1剧本对齐.md`
- shared root 当前正式下一条是：
  - `导航检查`
  - 然后 `遮挡检查`
- `NPC` 继续停在：
  - `needs-unity-window`

**恢复点 / 下一步**
- 后续如果用户继续问“现在到底该发谁”，直接按已经 sync 到 `main` 的这套口径回答，不再回退到旧排序。

## 2026-03-21｜治理模型正式降级：从重流程放行改为 main-only 并发开发
**当前主线目标**
- 把治理线程从“复杂交通调度层”降级成“高危打断器 + 收烂摊子的人”，用最小代价换真实开发速度。

**本轮完成**
1. 已重写当前总控和批次入口，使其不再要求：
   - branch 放行
   - grant 排队
   - return-main 收口
2. 已重写治理线程自用 prompt 与职责文件，使治理线程只保留：
   - 高危撞车打断
   - Unity / MCP 单写冲突打断
   - 现场写坏后的收口
3. 已把当前可分发 prompt 改成“直接开发 prompt”：
   - `scene-build`
   - `NPC`
   - `农田交互修复V2`
   - `导航检查`
   - `遮挡检查`
   - `spring-day1`
4. 已把 `AGENTS.md` 顶部加上临时最高优先级覆盖：
   - 当前普通开发只认 `main`
   - 旧 branch-only 规则退到下层历史背景

**关键决策**
- 当前治理线不再追求“每一步都被格式化”，而是追求“除非真的危险，否则让开发直接发生”。
- 当前只保留 4 个硬刹车：
  - 同一高危目标撞车
  - Unity / MCP live 单写冲突
  - 破坏性 Git 动作
  - 已经把现场写坏

**恢复点 / 下一步**
- 之后项目经理如果继续贴线程回执，优先判断是否命中这 4 个硬刹车；如果没有，就直接让线程继续，而不是重新拉回放行体系。

## 2026-03-21｜main-only 新阶段目录与 scene-build 迁移口径已正式建立
**当前主线目标**
- 把“main-only 极简并发开发”从旧阶段目录里剥离出来，建立新的现行目录、现行 prompt 集合，以及 `scene-build / spring-day1` 的迁移与交接口径。

**本轮完成**
1. 已建立新的现行目录：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01`
2. 已在新目录下补齐当前直接开发 prompt 与回执型 prompt。
3. 已把旧目录 `2026.03.21_恢复开发总控_01` 标成历史阶段。
4. 已把两个现行入口文件重写为新目录跳转入口。
5. 已明确 `scene-build` 的迁移口径：
   - 当前现场：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - 目标路径：`D:\Unity\Unity_learning\scene-build-5.0.0-001`
   - 迁移方式：`git worktree move`
   - 执行时机：先冻结，再迁移

**关键判断**
- 用户这轮要的不是“继续堆更复杂的治理”，而是“先把当前入口、命名、分区理顺，再为后续轻治理和 vibecoding 场景规范留出干净起点”。
- 因此这轮先收口目录与入口，不把 `vibecoding` 规范适配和 `scene-build` 正式迁移混成一步。

**恢复点 / 下一步**
- 先等 `scene-build / spring-day1` 两份真实回执。
- 回执回来后，再执行 `scene-build` 的正式迁移动作。

## 2026-03-21｜spring-day1 的 live prompt 已切到“给 scene-build 交正式 brief”
**当前主线目标**
- 避免 `spring-day1` 继续以泛开发口径跑偏，改成直接服务 `scene-build` 的剧情到空间翻译线程。

**本轮完成**
1. 已重写 `spring-day1` 的当前开发 prompt，明确其唯一职责是输出给 `scene-build` 的空间职责表。
2. 已重写 `spring-day1` 的交接 / 回执 prompt，要求它回来的必须是正式交付口径，而不是空泛状态汇报。
3. 已把固定交付块写清：
   - `Day1` 场景模块清单
   - `SceneBuild_01` 的正式身份
   - 强制承载动作
   - 禁止误扩边界
   - `next_scene_build_focus`

**关键决策**
- `spring-day1` 当前不是第二个施工线程，而是 `scene-build` 的剧情约束与空间职责提供者。
- 因此它这轮的完成标准不再是“做了点什么”，而是“交出了一份场景线程能直接执行的 brief”。

**恢复点 / 下一步**
- 项目经理现在可以直接发送新的 `spring-day1` prompt。
- 等 `spring-day1` 交回 brief 后，再决定是否需要进一步改 `scene-build` 的后续口径。

## 2026-03-21｜scene-build 当前不直接恢复施工，先做最小 checkpoint 再迁移
**当前主线目标**
- 回答 `scene-build` 冻结回执之后，到底该不该立刻恢复施工，以及正式迁移的先后顺序。

**本轮完成**
1. 已裁定：
   - 当前不建议 `scene-build` 立刻恢复自由施工
   - 先做 3 个记忆文件的最小 checkpoint
   - 之后再执行正式迁移
2. 已新增专用 prompt：
   - `scene-build_最小checkpoint并等待正式迁移.md`
3. 已对 `spring-day1` prompt 做一处补强：
   - 其交付件必须在 `scene-build` 迁到 `D:\Unity\Unity_learning\scene-build-5.0.0-001` 后仍然可直接使用

**关键决策**
- 当前阻塞 `scene-build` 的不是场景内容，而是迁移前的最后一层收口。
- 与其带着 dirty 硬搬，不如先用最小成本把 memory 收成 checkpoint，再迁移；这一步风险更低，后续也更清楚。

**恢复点 / 下一步**
- 项目经理现在有两份最新可发文件：
  - `spring-day1_当前开发放行.md`
  - `scene-build_最小checkpoint并等待正式迁移.md`
- 等 `scene-build` clean 并回执 ready 后，再执行 `git worktree move`。

## 2026-03-21｜scene-build 迁移口径纠偏：不是热切换，不是 `SceneBuild_Standalone`，当前卡在 live 目录锁
**当前主线目标**
- 继续服务 `24_main-only极简并发开发与scene-build迁移`：把 `scene-build` 的真实迁移动作从错误认知中拉回现场事实。

**本轮完成**
1. 已纠正一条关键历史认知：
   - `NPC / farm` 当时真正稳定成立的是“回根仓库后，重启 Codex 才完全显现”
   - 不是“会话内热切换完全成功”
2. 已纠正 `scene-build` 目标路径：
   - 从 `D:\Unity\Unity_learning\SceneBuild_Standalone\scene-build-5.0.0-001`
   - 改为 `D:\Unity\Unity_learning\scene-build-5.0.0-001`
3. 已实测 `git worktree move`，确认当前不是规则阻断，而是 OS 级目录占用阻断。
4. 已定位 live 占用来源为 `Unity.exe + mcp-for-unity + mcp-terminal.cmd` 全部仍指向旧目录。

**关键决策**
- 当前治理线程不再给 `spring-day1` 发新 prompt。
- 当前唯一必要的新 prompt 是让 `scene-build` 释放 Unity / MCP 目录锁。

**恢复点 / 下一步**
- 等 `scene-build` 释放旧目录锁后，立刻继续迁移，不再回头讨论目标路径。

## 2026-03-22｜scene-build 已完成 Git 层迁移，当前转入“旧路径废弃 + 新路径复核”
**当前主线目标**
- 把 `scene-build` 从“等待迁移”推进到“迁移完成后的可继续施工态”。

**本轮完成**
1. 已收下 `spring-day1` 的最终 handoff 回执；它本轮已经完成职责，不再需要持续贴给治理线程。
2. 已收下 `scene-build` 的释放目录锁回执，但同时确认关 Unity 后浮出 4 个 TMP 字体资源 dirty。
3. 已再次验证：直接 `git worktree move` 仍然被 OS 级 `Permission denied` 阻断。
4. 已改走 `copy + repair` 兜底方案，并验证成功：
   - 新正式路径：`D:\Unity\Unity_learning\scene-build-5.0.0-001`
   - `git worktree list --porcelain` 已只认新路径
5. 已新增迁后 prompt：
   - `scene-build_迁后复核与从新路径恢复.md`

**关键决策**
- 当前可以把“迁移本身”视为已完成，不再继续纠缠直接 `move`。
- 旧 `Sunset_worktrees` 路径当前只剩危险旧壳，不应再作为 live 现场使用。

**恢复点 / 下一步**
- 等 `scene-build` 按迁后 prompt 回执“只认新路径”后，再切回施工推进。

## 2026-03-22｜scene-build 迁移结论作废，正式 worktree 已收回旧路径
**当前主线目标**
- 收住我前一轮对 scene-build 路径的误判，恢复到用户明确指定的旧 worktree 认知，并把误导性入口先止血。

**本轮完成**
1. 已确认正式 `scene-build` worktree 重新回到：
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
2. 已确认 `D:\Unity\Unity_learning\scene-build-5.0.0-001` 只是误复制副本：
   - `.git` 已失活为 `.git.DISABLED_DO_NOT_USE`
3. 已改正当前 live 批次目录里的现行口径：
   - README
   - 治理线程执行 prompt
   - 治理线程职责文件
   - scene-build / spring-day1 相关 prompt
4. 已把几份错误迁移 prompt 标成“已废弃”，避免后续再被分发。

**关键决策**
- 这轮不再继续任何 scene-build 物理迁移动作。
- 当前治理线程的职责只剩：收误操作、修入口、阻止别人误入错误副本路径。

**恢复点 / 下一步**
- 如果用户后续要继续 scene-build，只按旧 worktree 路径继续。
- 错误副本是否删除，等待用户明确裁定。

## 2026-03-22｜误复制副本已删除，scene-build 当前只剩旧 worktree 正式现场
**当前主线目标**
- 把上一轮“副本仍待裁定”的状态收成最终事实，避免现场继续保持半清理状态。

**本轮完成**
1. 已按用户明确要求删除误复制副本：
   - `D:\Unity\Unity_learning\scene-build-5.0.0-001`
2. 已复核：
   - `Test-Path` 返回 `False`
   - `git worktree list --porcelain` 仍只认 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
3. 已把当前批次入口与治理口径改成“副本已删除，现场唯一”。

**关键决策**
- scene-build 的“迁移落地”到这里已经结束：结果不是产生一个新项目根，而是把误副本清掉，只保留唯一正式 worktree。
- 后续再动 scene-build，只能继续旧 worktree，不再讨论新路径。

**恢复点 / 下一步**
- 如果继续做，就直接回 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001` 干活。

## 会话 43 - 2026-03-22（scene-build 真正完成的是 Codex 应用层归位，不是 Git 再迁移）
**当前主线目标**
- 把 `scene-build` 的剩余问题从“Git / 目录层”彻底收口到“Codex 桌面端线程 cwd 绑定错误”，并完成真正对用户有用的归位修复。

**本轮完成**
1. 已确认用户看到的报错：
   - `fatal: 'codex/scene-build-5.0.0-001' is already used by worktree ...`
   说明 Git 本体仍然正常，错误来自 UI 还在错误 cwd 下尝试切那个分支。
2. 已定位到 Codex 本地状态层：
   - `C:\Users\aTo\.codex\state_5.sqlite`
   - `C:\Users\aTo\.codex\session_index.jsonl`
   - `C:\Users\aTo\.codex\.codex-global-state.json`
3. 已确认当前被命名为“场景搭建”的线程 id 为：
   - `019cc7ba-fb87-7012-a7ef-0ccee21121c0`
4. 已把它的 `cwd` / `git_branch` / `git_sha` 改回旧 worktree 现场。
5. 已把全局激活 workspace 根改成优先包含：
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `D:\Unity\Unity_learning\Sunset`

**关键决策**
- 从现在开始，scene-build 的“迁移成功”口径应理解为：
  - Codex 应用层线程已归位到旧 worktree
  - 用户重启 Codex 后检查是否显示/切换正常
- 不再把“新建路径 / 复制路径 / 再 repair Git”当成后续默认动作。

**恢复点**
- 用户现在可以直接重启 Codex 做归位验证。
- 如果验证通过，scene-build 这条线就回到正常开发，不再停留在迁移话题。

## 2026-03-22｜治理线程自审：把本线未完成项、作废项和文档债集中摊开
**当前主线目标**
- 不再只围绕单个阻塞点救火，而是把治理线程自己在 `23 / 24 / TD` 层留下的欠账一次性对齐，明确哪些是真的没做完，哪些已经被用户口头推翻但文档还没清干净。

**本轮完成**
1. 已对账以下入口：
   - `24_main-only极简并发开发与scene-build迁移/tasks.md`
   - `24_main-only极简并发开发与scene-build迁移/memory.md`
   - 根层 `Codex规则落地/memory.md`
   - `23_前序阶段补漏审计与并发交通调度重建/tasks.md`
   - `000_代办/codex/TD_14_自动化skill触发日志Hook待办.md`
   - 线程记忆 `memory_0.md`
2. 已形成一份集中总表：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\24_main-only极简并发开发与scene-build迁移\治理线程自审与未完成项总表_2026-03-22.md`
3. 已明确区分三类对象：
   - 真未完成：`scene-build` 归位验收回执、`vibecoding` 场景规范适配立项、批次目录收口、`TD_14`
   - 已作废但文档仍残留：`scene-build` 正式迁移到新路径、`23` 阶段 queue/多 checkpoint 正文化
   - 文档债：`24/tasks.md` 与 `24/memory.md` 坏编码且仍夹带旧迁移叙事

**关键决策**
- 当前治理线最该补的不是再造新规则，而是先把自己留下的坏入口和假待办清掉。
- `24` 这组文件已经不能再被默认当作健康活文档继续叠写，后续应以新的健康文档承接。

**恢复点 / 下一步**
- 后续优先顺序应是：
  1. 收 `scene-build` 归位最终验收回执
  2. 清理/替换 `24` 阶段坏入口
  3. 单开 `vibecoding` 场景规范适配阶段
  4. 再决定 `TD_14` 是否升级为正式阶段

## 2026-03-22｜当前治理主线正式切到“vibecoding 场景规范与 main 收口”
**当前主线目标**
- 接受用户最新裁定，把治理线从“继续清 scene-build 尾巴”切到新的正式主线：
  - `vibecoding` 场景规范适配
  - `main-only` 并发开发的统一收口机制

**本轮完成**
1. 已新建正式阶段工作区：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口`
2. 已明确层级关系：
   - `25_vibecoding场景规范与main收口` 是正文主线
   - `2026.03.21_main-only极简并发开发_01` 是执行批次壳
3. 已新增机制文档：
   - `并发线程统一回执与main收口机制_2026-03-22.md`
4. 已新增固定群发 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\并发线程_统一收口回执.md`
5. 已新增给全局 skills 的窄范围委托：
   - `给全局skills的窄范围委托_2026-03-22.md`
   - 范围只锁定 `sunset-startup-guard` 一级告警、trigger-log 格式统一、`Missed-Skills`、`Promote-To-Learning`

**关键决策**
- `TD_14` 当前不作为优先级最高项，它先降级到后手。
- 对 `main-only` 的正确补强不是回到旧 queue，而是“一次广播、一次收件、一个收口窗口、顺序进 main”。

**恢复点 / 下一步**
- 先按新阶段继续出第一版 `vibecoding` 场景规范适配 brief。
- 需要收口时，直接发 `并发线程_统一收口回执.md`。

## 2026-03-22｜第一版 `vibecoding` 场景规范适配 brief 已进入正式主线
**当前主线目标**
- 把 `25` 阶段从“只有治理壳”推进到“已经有第一份场景规范正文”。

**本轮完成**
1. 已结合 `spring-day1` handoff、requirements 和 `scene-build` 现有定位，新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\vibecoding场景规范适配Brief_2026-03-22.md`
2. 已把当前 `SceneBuild_01` 的 `vibecoding` 标准收成短口径：
   - 住处安置
   - 院落对话留白
   - 工作台焦点
   - 教学区与生活区相邻
   - 禁止误扩成整村总图

**关键决策**
- 当前 `vibecoding` 主线已经正式进入“有正文可执行”的状态，不再只是口头要求。

**恢复点 / 下一步**
- 继续补 dirty 归属说明。
- 然后进入第一轮统一回执窗口。

## 2026-03-22｜`25` 阶段已补齐 shared root dirty 归属说明
**当前主线目标**
- 让 `main-only` 并发现场不再靠脑内记忆区分 dirty，而是有一份可直接引用的归属口径，服务后续统一回执和顺序进 `main`。

**本轮完成**
- 已新增 `25_vibecoding场景规范与main收口/当前shared_root_dirty归属说明_2026-03-22.md`。
- 已把当前 mixed dirty 正式拆成：治理线当前认领、治理线旧账停用区、NPC、`spring-day1` / 剧情整理、农田 / 库存交互。
- 已把 `25/tasks.md` 推进到“dirty 归属说明完成，下一步发统一回执窗口”。

**关键决策**
- 当前 shared root 脏，不等于所有线程都不能继续；线程只认自己的 `changed_paths`，治理侧再用统一回执和批次表做顺序收口。

**恢复点 / 下一步**
- 下一步发第一轮统一回执窗口。
- 再基于回执结果形成第一版 main 收口批次表。

## 2026-03-22｜第一轮统一回执与第一版 main 收口批次表已完成
**当前主线目标**
- 把 `main-only` 从“已经开始收件”推进到“已经做出首轮批次裁定”，让后续执行不再停留在原始回执堆里。

**本轮完成**
- 已基于 `000全员回执.md` 收到农田、`spring-day1`、NPC、导航、遮挡检查、全局 skills 的正式回执。
- 已新增 `第一轮main收口批次表_2026-03-22.md`。
- 已形成首轮分组：
  - A组：农田
  - B组：`spring-day1`、遮挡检查
  - C组：NPC、导航
  - 支援：全局 skills

**关键决策**
- 本轮最适合先入 `main` 的只有农田。
- `spring-day1` 与遮挡检查作为 branch carrier 下一批处理，NPC 与导航继续开发更划算。

**恢复点 / 下一步**
- 下一步优先发农田第一批 `main` 收口执行 prompt。
- 然后再决定 B组 先迁哪条线。

## 2026-03-22｜农田第一批 `main` 收口 prompt 已准备完成
**当前主线目标**
- 在第一版批次表形成后，立刻把 A组 的第一刀执行 prompt 准备好，避免治理线停在“有表无动作”。

**本轮完成**
- 已新增 `农田_第一批main收口执行.md`。
- 已把农田动作压成最小执行面：只核对白名单、只收农田自己的 checkpoint、不再扩写。

**关键决策**
- 当前真正开始执行的第一刀就是农田，不再继续停在抽象裁定层。

**恢复点 / 下一步**
- 现在可以直接把农田 prompt 发出去。
- 农田回执后再接 B组。

## 2026-03-22｜每线程白名单提交已被收成正式默认规则
**当前主线目标**
- 把用户提出的“每个线程只提交自己修改的内容”从聊天判断推进成治理正文里的正式默认原则。

**本轮完成**
- 已在 `并发线程统一回执与main收口机制_2026-03-22.md` 中补入：
  - 默认一线程一白名单提交
  - 默认一线程一刀 checkpoint
  - 仅在高危撞车、跨线程耦合、branch carrier / worktree 迁入时升级处理
- 已补清：
  - Git SHA 不能自定义
  - 可统一的是 checkpoint 名称 / commit message
  - 推荐格式：`YYYY.MM.DD_<线程名>_<编号>`

**关键决策**
- 当前并发治理最接近最终方案的，不是“大一统提交”，而是“线程各收各的白名单提交，治理侧只处理少数例外”。

**恢复点 / 下一步**
- 以后线程完成一刀后，默认直接按这条规则执行即可。

## 2026-03-22｜B组 先收 spring-day1 的执行 prompt 已准备完成
**当前主线目标**
- 在 A组 农田之后，继续把 B组 的下一个执行对象推进到“可立刻发 prompt”的状态。

**本轮完成**
- 已裁定 B组 先收 `spring-day1`。
- 已新增 `spring-day1_B组main迁入执行.md`。
- 已把 `spring-day1` 动作压成 branch carrier `4ff31663` 到 `main` 的最小迁入窗口。

**关键决策**
- 当前 B组 更值得先迁的是 `spring-day1`，不是遮挡检查。

**恢复点 / 下一步**
- 现在可以直接把 `spring-day1` prompt 发出去。
- 回执后再处理遮挡检查。

## 2026-03-22｜默认白名单提交规则已落到执行壳入口
**当前主线目标**
- 不让“每线程白名单提交”只停在正文规则里，而是把它同步到真正会被反复打开的执行层入口。

**本轮完成**
- 已更新批次壳 `README.md` 与 `治理线程_职责与更新规则.md`。
- 已新增通用模板 `线程完成后_白名单main收口模板.md`。
- 已把默认提交规则和命名规则同步到执行层。

**关键决策**
- 当前这条规则已经算真正落地，不再只是治理正文里的抽象结论。

**恢复点 / 下一步**
- 后续线程的正常收口，默认直接套这个模板即可。

## 2026-03-22｜当前 live 入口、阶段正文和执行壳已对齐到同一条 main-only 收口线
**当前主线目标**
- 把本轮用户最强调的一点真正落下去：不仅正文说“每线程白名单提交”，而且线程实际会读到的入口、批次表、回收卡和脏改归属说明也必须同步改成同一条口径。

**已完成**
1. 已纠偏 live 入口：
   - `AGENTS.md`
   - `Sunset当前唯一状态说明_2026-03-17.md`
   - `Sunset Git系统现行规则与场景示例_2026-03-16.md`
2. 已把 `spring-day1` 新回执并回阶段正文与执行层：
   - `83d809a9` 已进入 `main`
   - 不再继续把它当“待 branch carrier 迁入”
3. 已把农田与 `spring-day1` 的最新状态同步到：
   - `000全员回执.md`
   - `第一轮main收口批次表_2026-03-22.md`
   - `当前shared_root_dirty归属说明_2026-03-22.md`
4. 已吸收全局 skills 的窄裁定：
   - `sunset-startup-guard` 保留硬前置
   - 但退出当前 `manual-equivalent` 一级告警统计
   - Sunset 治理线程后续只再写 `STL-v2`
5. 已完成一轮真实 `task + main + IncludePaths` 预检验证，结果允许继续。

**关键决策**
- 当前 `25_vibecoding场景规范与main收口` 已经不只是“机制阶段”，而是把入口、正文、执行壳、回收卡和预检证据接成了闭环。

**恢复点 / 下一步**
- 当前下一步不是再补外围解释，而是直接把治理线自己的白名单文件同步掉。

## 2026-03-22｜已把“规则更新后可直接 self-sync”变成可转发 prompt
**当前主线目标**
- 回应用户“那就直接告诉他们规则已经更新，让他们自己提”的要求，把当前规则翻译成线程可直接执行的话术。

**已完成**
1. 已为 `NPC` 生成“继续修复并直接 main 收口” prompt。
2. 已为 `spring-day1` 生成“剩余文档整理并直接 main 收口” prompt。
3. 两份 prompt 都已明确说明：
   - 当前规则更新已经真正落进 `main`
   - 线程现在可以自己按白名单提交
   - 不要再按旧口径回“不能提交”

**关键决策**
- 当前不再需要我替这两条线代提交。
- 现在最符合主流程的就是：线程自己白名单提自己的那一刀。

**恢复点 / 下一步**
- 把这两份 prompt 发给对应线程。
- 然后继续收它们的回执。

## 2026-03-22｜我已把“现在可以直接开工”压成两种现成前缀
**当前主线目标**
- 不再只对用户口头说“dirty 基本清掉、规则已经落地，所以可以直接开工”，而是把它落实成可以直接转发给线程的当前版本更新前缀。

**已完成**
1. 已新增普通线程通用前缀：
   - `并发线程_当前版本更新前缀.md`
2. 已新增 `scene-build` 专用前缀：
   - `scene-build_当前版本更新前缀.md`
3. 已把当前线程状态收成三类：
   - 直接开工：`NPC`、农田、导航检查、遮挡检查
   - 直接开工但走 worktree 专用口径：`scene-build`
   - 当前先停：`spring-day1`、全局 skills
4. 已明确：
   - `NPC` / `spring-day1` 已替普通 `main-only` 线程走完一轮制度验证
   - 后续普通线程不需要再重复做这轮制度测试

**关键决策**
- 当前要减少的不是“开发速度”，而是“线程再次误以为自己还要排队、还不能提、还得等治理线代提”的认知噪音。
- `scene-build` 的例外不在“能不能干”，而在“它不是 shared root main-only，而是 worktree 内直接 checkpoint”。

**恢复点 / 下一步**
- 你现在可以直接把新前缀转发给对应线程。
- 我这条治理线下一步要继续收的是剩余线程的实际有效任务，而不是再造一层抽象规则。

## 2026-03-22｜第二波直接开发分发壳已经备好
**当前主线目标**
- 不只停在“可以直接开工”的原则层，而是把下一波最值得唤醒的线程、顺序和专属 prompt 全部准备好。

**已完成**
1. 已新增：
   - `下一波直接唤醒顺序_2026-03-22.md`
2. 已新增 4 条线程专属 prompt：
   - `scene-build_当前版本更新并继续施工.md`
   - `导航检查_当前版本更新并继续直接开发.md`
   - `NPC_当前版本更新并继续直接开发.md`
   - `遮挡检查_当前版本更新并继续直接开发.md`
3. 已把当前唤醒顺序固定为：
   - `scene-build -> 导航检查 -> NPC -> 遮挡检查`
4. 已把当前不必继续唤醒的线程固定为：
   - `spring-day1`
   - 全局 skills

**关键决策**
- 现在项目经理已经不需要临时从聊天里拼“下一步到底发给谁”。
- 当前治理线程的价值变成：准备现成壳、吃固定回执、只在少数高危冲突时打断。

**恢复点 / 下一步**
- 现在可以直接把第二波 prompt 发出去。
- 我这边接下来只继续处理回执、风险和冲突，不再重复造规则。

## 2026-03-22｜我已纠正第二波里的两个关键误判
**当前主线目标**
- 修正我刚刚第二波分发里最危险的两个误判：导航和遮挡的 prompt 还不够硬，没把“旧分支遗留不再算 blocker”压死；另外 `spring-day1` 不该继续停。

**已完成**
1. 已回读 `002全员部分聊天记录` 中的：
   - 导航检查
   - 遮挡检查
   - `spring-day1`
2. 已确认：
   - 导航与遮挡都仍把 branch carrier 视为当前阻塞
   - `spring-day1` 其实已经明确表态可以继续做真实 Day1 闭环
3. 已重写两份关键 prompt：
   - 导航检查：必须在同一轮里处理 `codex/navigation-audit-001` 遗留并进入 `main` 首个真实代码 checkpoint
   - 遮挡检查：必须在同一轮里处理 `295e8138` 遗留并进入 `main` 首个真实整改 checkpoint
4. 已新增：
   - `spring-day1_当前版本更新并继续直接开发.md`
5. 已更新第二波顺序：
   - `spring-day1` 重新回到可开工队列

**关键决策**
- 当前这不是小修辞问题，而是主流程纠偏：
  - 不能再容忍线程继续拿旧分支残留当本轮不动手的借口
  - 不能把已经完成收口的 `spring-day1` 继续误放在暂停态

**恢复点 / 下一步**
- 你现在可直接分发的不是我上一版导航/遮挡 prompt，而是这次纠偏后的新版。
- 我下一步继续只处理新回执，不再重复这一轮认知修正。

## 2026-03-23｜我已把“并发代码正确率”问题正式定性
**当前主线目标**
- 回答用户关于“是不是该加强每次改代码后的强制检查 / hook”的问题，不再停在抽象担心层。

**已完成**
1. 已根据 `spring-day1`、遮挡线程两次编译级修正的聊天记录确认：
   - 当前线程们仍会把本地可提前发现的问题拖到用户贴错误后才补
2. 已把当前缺口定性为 3 类：
   - 目标文件静态自查没有默认执行
   - 改文件后的最小 diff/编码健康检查没有默认执行
   - 收 checkpoint 前的最小编译/测试验证没有默认执行

**关键决策**
- 当前最优解不是“把 MCP 再抬高一层”，而是补一个轻量强制的“双层正确率闸门”：
  - 第一层：改完代码立刻做本地静态检查
  - 第二层：准备收口时做最小编译/测试验证

**恢复点 / 下一步**
- 我接下来会直接把这个最小方案讲清楚，供你决定是只写规则，还是顺手让我把脚本 / hook 一起落地。

## 2026-03-23｜我已经把代码闸门做成真实脚本，不再停在方案层
**当前主线目标**
- 回应用户“直接一步到位落地，然后再测试”的要求，把并发正确率治理从聊天方案变成真实可执行脚本。

**已完成**
1. 已新增 `scripts/CodexCodeGuard/` 检查器项目。
2. 已把 `.cs` 代码闸门接入 `scripts/git-safe-sync.ps1` 的 `task preflight / sync` 主流程。
3. 已把硬入口和线程 prompt 同步到“代码闸门”新口径。
4. 已完成真实测试：
   - `CodexCodeGuard.dll` 直测通过
   - `git-safe-sync.ps1 -Action preflight -Mode task` 集成测试通过，输出已包含代码闸门结果

**关键决策**
- 当前最有价值的不是 Git hook，而是把闸门挂在你们已经真实统一经过的 `git-safe-sync.ps1` 上。
- 这样线程就算不自觉，也过不了收口口。

**恢复点 / 下一步**
- 现在这套东西已经可以正式投入并发现场使用。
- 下一步只需要观察真实线程回执，再决定要不要补更重的 targeted tests。

## 2026-03-23｜我已把“全局 skills 旧历史是否还要回”与“我自己还剩什么”正式摊平
**当前主线目标**
- 不再让全局 skills 的旧历史回复和治理线自己的历史欠账继续混在一起，导致用户还要反复追问“这件事到底做没做完”。

**已完成**
1. 已确认全局 skills 那条旧历史现在不需要再回，也不需要再批准补做：
   - 它当时列出的 Sunset 本地落地项，治理线程已经完成
2. 已把我这条治理线剩余事项重新收成 3 个真实未完：
   - `TD_14` 裁定
   - 代码闸门观察期后的二次细化
   - 当前批次原始回执 / 聊天记录材料归档
3. 已明确：
   - 现场虽然还有很多 dirty，但大部分不属于治理线，是各业务线程正在跑的实现或记忆更新

**关键决策**
- 以后再遇到“另一个线程过去说还能做”这种情况，要先按当前 live 事实复核，不能直接把旧回复当未完成待办。

**恢复点 / 下一步**
- 我接下来只需要盯三件事：
  - `TD_14`
  - 代码闸门真实使用反馈
  - 批次原始材料归档

## 2026-03-23｜治理线剩余结构性尾项已全部做完
**当前主线目标**
- 把我自己刚刚盘出来的最后几件尾项真正关账，而不是继续留在“已识别但待做”的状态。

**已完成**
1. `TD_14` 已正式裁定为冻结后备项，不再算当前未完成。
2. 当前批次材料已形成归档与关账摘要。
3. 当前阶段任务板里的剩余结构性项已全部勾完。

**关键决策**
- 从现在开始，这条治理线已经没有“历史账务类未完成项”。
- 剩下的只会是运行中的新问题，而不是老账。

**恢复点 / 下一步**
- 如果后续继续推进，默认就是：
  - 吃线程回执
  - 处理冲突
  - 观察代码闸门效果

## 2026-03-23｜遮挡验证副本裁定已正式落地
**当前主线目标**
- 把“是否批准 `Sunset_occlusion_validation` 这个副本”从聊天口径升级成正式可转发裁定。

**已完成**
1. 已新增：
   - `遮挡检查_验证副本有条件批准并测试后删除.md`
2. 裁定是：
   - 有条件批准
   - 只允许作为一次性测试沙盒
   - 本轮测试跑完就删
   - 不得变成长期第二现场

**恢复点 / 下一步**
- 这件事现在已经不是待判断事项，而是可直接执行的已定裁定。

## 2026-03-23｜快速出警通道已具备“skill + prompt + 脚本”三件套
**当前主线目标**
- 把用户提出的“以后出问题就让你快速找犯人”真正变成可执行能力，而不是继续靠我临场手搓。

**已完成**
1. 已创建本地 skill：
   - `sunset-rapid-incident-triage`
2. 已创建项目 prompt：
   - `快速出警_责任线程速查.md`
3. 已做背包异常案例验证，命中 `农田交互修复V2`
4. 已补一份遮挡线程专用批准 prompt：
   - `遮挡检查_主工程单实例验证窗口批准.md`

**关键决策**
- 现在这条“快速出警”能力已经可以正式投入使用。
- 后续不需要每次重新想责任线程定位流程。

**恢复点 / 下一步**
- 你现在既可以直接叫我走这条 skill，也可以把 prompt 发给我当快捷入口。

## 2026-03-23｜遮挡线程审批结论：批，但要给单实例验证窗口
**当前主线目标**
- 给用户一个明确的可执行裁定，而不是模糊的“看起来可以试试”。

**已完成**
1. 已核共享根与 Unity/MCP 单实例层状态。
2. 已裁定：
   - 遮挡线程当前方案可批
   - 但它确实需要一个短时清净窗口
3. 已把这条裁定落进专用 prompt：
   - `遮挡检查_主工程单实例验证窗口批准.md`

**关键决策**
- 这里的“清净时刻”只针对主工程 Unity/MCP live 验证，不等于全项目冻结。

**恢复点 / 下一步**
- 现在可以直接把批准 prompt 发给遮挡线程执行。

## 2026-03-23｜治理尾账与业务现场边界再确认
**当前主线目标**
- 防止“治理尾账已清”被误解成“所有线程都已经 clean / 所有业务都已完工”。

**关键结论**
- 当前已清的是治理尾账。
- 当前未清、但不属于治理欠账的，是各业务线程自己的实现现场与验证现场。

**恢复点 / 下一步**
- 这条线后续默认只处理新回执、新冲突和新验证，不再反复回头清治理旧账。

## 2026-03-23｜编码问题学习与本地检测已形成新治理结论
**当前主线目标**
- 回答用户对本地文档乱码根因的追问，并把这次“文章学习 + 本地取证”的结论收进治理链。

**已完成**
1. 已基于外部文章完成本地环境与文件双层取证。
2. 已新增正式报告：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\本地编码检测报告_2026-03-23.md`
3. 已把当前判断钉住：
   - Sunset 确实命中了同类编码问题
   - 但不是只有“终端假乱码”，而是“环境未统一 + 少量文件真损坏”并存

**关键决策**
- 后续治理顺序固定为：
  1. 先统一 PowerShell / VS Code 的 UTF-8 环境口径
  2. 再修仍在使用的活文档坏样本
  3. 历史归档样本按 `historical-ignore / targeted-rewrite` 分级处理
- 禁止把所有乱码都当成同一类问题，更不能直接做无差别批量转码。

**恢复点 / 下一步**
- 如果继续推进，本治理线下一步应进入：
  - 编码环境统一
  - `001部分回执.md` 定点修复

## 2026-03-23｜编码环境统一已从分析态进入落地态
**当前主线目标**
- 把“本地编码问题”从检测和建议推进到实际环境修复。

**已完成**
1. 已在本机 VS Code 用户设置中显式补上 `"files.encoding": "utf8"`。
2. 已创建 `WindowsPowerShell` 的 Profile 启动入口。
3. 已把 PowerShell 5.1 的控制台编码与默认文件读写编码统一到 UTF-8。
4. 已复测确认：
   - 新 shell 中 `chcp=65001`
   - 输入 / 输出 / 管道编码全部为 `utf-8`
   - 默认 `Get-Content` 已不再把 UTF-8 无 BOM 文档读花

**关键决策**
- 本次修复的关键新增点不是只有 `chcp 65001`，而是把 `Get-Content:Encoding` 也纳入默认口径。
- 这意味着后续判断“终端假乱码”与“文件真损坏”时，误判概率会显著下降。

**恢复点 / 下一步**
- 本治理线下一个最自然动作已切换为：
  - 修活文档坏样本
  - 首选 `001部分回执.md`
## 2026-03-23｜MCP 根因治理新增“旧线程缓存未刷新”口径
**当前主线目标**
- 继续把治理线保持在“安全基线 + 误判阻断”职责上，不让旧线程再把 MCP 现场误判成服务端回滚。

**本轮完成**
1. 已确认当前 shared root `main` 上：
   - `config.toml` 回正为 `unityMCP + 8888`
   - 基线脚本返回 `pass`
2. 已从 Codex 会话日志中补到更深层根因：
   - 旧线程即使在读到 `8888` 配置后，也可能继续沿用旧 MCP 路由缓存发请求
3. 已把这条口径同步进 live 规则、批次 README、通用前缀 prompt 与 `scene-build` 前缀 prompt。
4. 已继续清 active memory 中会误导人的旧端口字面，把它们统一改写成“旧端口口径（已失效）”。

**关键决策**
- 以后凡是 `config + 8888 + pidfile + baseline` 已通过，但旧线程仍报旧端口 / 旧桥名 / `resources/templates` 为空，默认先按“旧会话 MCP 路由缓存未刷新”处理。
- 这类问题优先换新线程 / 新会话，或先手工对 `8888` 做 initialize + `tools/list`；不再直接把锅甩给 shared root 服务端。

**恢复点 / 下一步**
- 当前治理线已把 MCP 根因从“只纠端口”推进到“纠会话缓存误判”。
- 下一步只剩白名单 sync，以及继续看真实线程回执是否还会复发。

## 2026-03-23｜用户单文件提交与 TMP 字体归属核对
**当前主线目标**
- 帮用户把自己手改的 `Anvil.png.meta` 单独收口，同时判断当前 4 个 TMP 字体资产是否真能归到 `spring-day1`。

**已完成**
1. 已将 `Anvil.png.meta` 以单文件白名单方式提交到 `main`：
   - `30d6b5ca`
2. 已核当前 4 个 TMP 字体资源的 Git 历史：
   - 未发现任何最近的 `spring-day1` 提交直接修改它们

**关键决策**
- 当前制度下，用户自己的修改也完全可以像线程一样独立提交：
  - 一次只收自己明确指定的路径
  - 不必等整个仓库 clean
- 当前 4 个 TMP 字体资源不能直接按 `spring-day1` 追责；证据还不够硬。

**恢复点 / 下一步**
- 这批字体资源如果要继续清责，下一步应看：
  - 哪个线程最近在 live Unity 里触碰过 TMP / DialogueUI / 中文字体资源

## 2026-03-23｜两条线程回执已完成快审并补发第三轮执行令
**当前主线目标**
- 审核 `农田交互修复V2` 与 `项目文档总览` 两条线程的新回执，给出可执行裁定，并直接补发下一轮 prompt。

**本轮完成**
1. 已核 `农田交互修复V2` 第 1 刀箱子 bridge 回执与关键代码 / 测试：
   - `ChestInventory.cs`
   - `ChestInventoryV2.cs`
   - `ChestController.cs`
   - `ChestInventoryBridgeTests.cs`
2. 已确认这一刀的真实裁定：
   - `StackOverflowException` 主阻断已清掉
   - `第 1 刀 checkpoint 可接受`
   - 但现有测试只覆盖 bridge 内存层，尚未覆盖 `Save()/Load()` 与真实箱子 UI 重开路径
3. 已核 `项目文档总览` 的四份成品：
   - `26.03.23-README草案-v1.md`
   - `26.03.23-简历项目介绍三档文案.md`
   - `26.03.23-面试口述版项目讲稿.md`
   - `26.03.23-角色证据矩阵与可写边界.md`
4. 已确认项目文档线的真实裁定：
   - 第一版成品层已完成
   - 但 `README v1` 的文档入口仍使用本地绝对路径，不适合直接落到仓库根 `README.md`
5. 已新增两份下一轮执行令：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\003-第一刀checkpoint通过并转入第二刀与箱子链补强回归.md`
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\项目文档总览\26.03.23-项目经理简历画像与README委托-03可落根版与最终口径收口.md`

**关键决策**
- 农田线这轮不能把“bridge 最小测试通过”外推成“整条箱子链彻底稳定”；下一轮必须在推进第 2 刀放置问题的同时，补一条 `save/load` 或 `reopen chest UI` 回归。
- 项目文档线这轮不能停在“第一版成品已经有了”；下一轮必须把 `README v1` 推进成 `README v2 可落根版`，并对保守版 / 强化版给出明确推荐主口径。

**恢复点 / 下一步**
- 现在可以直接把这两份 prompt 发给对应线程执行。
- 我这边已完成本轮治理审计与补发，下一步等待它们按新格式回执即可。

## 2026-03-24｜项目文档总览线程目录改为批次化归档
**当前主线目标**
- 处理用户指出的线程目录散乱问题，把 `项目文档总览` 从“按日期平铺”整理成“按批次分文件夹”的结构。

**本轮完成**
1. 已确认 `项目文档总览` 线程现有文件天然分成 3 个阶段：
   - 首轮委托与分析
   - 第二轮成品初稿
   - 第三轮可落根收口
2. 已在 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\项目文档总览\` 下建立：
   - `01_第一批次_委托与分析`
   - `02_第二批次_成品初稿`
   - `03_第三批次_可落根收口`
3. 已将对应 prompt 与产物迁移到各自批次目录。
4. 已在根目录新增 `README.md`，明确：
   - 当前批次结构
   - 每批包含什么
   - 后续新增内容如何继续归档

**关键决策**
- 这条线后续不再沿用“同一天文件平铺根目录”的方式。
- 后续如再有新一轮 prompt 与产物，默认继续新增 `04_...` 批次目录，而不是再把根目录写满。

**恢复点 / 下一步**
- `项目文档总览` 当前已经完成批次化整理；
- 如果用户后续要求整理别的长期线程目录，可以直接按这套模式复用。

## 2026-03-24｜三条线程已收件并补发新一轮执行令
**当前主线目标**
- 根据 `项目文档总览`、`导航检查`、`农田交互修复V2` 的新回执继续做治理裁定，判断谁该继续打、谁该转最终总结 / 验收报告。

**本轮完成**
1. 已确认 `项目文档总览` 线程当前已进入可收口状态：
   - `README v2` 完成
   - 简历推荐终稿完成
   - 最终主口径已收紧为 `混合版`
   - 仓库根 `README.md` 当前实际不存在
2. 已为 `项目文档总览` 新增第 4 批次 prompt：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\项目文档总览\04_第四批次_验收总结与落根决策\26.03.24-项目经理简历画像与README委托-04最终总结与验收报告.md`
3. 已确认 `导航检查` 当前结构层 checkpoint 可接受，但真实 blocker 已收敛为：
   - active / moving 成立
   - 实际位移没有打到 `Rigidbody2D / Transform`
4. 已为 `导航检查` 新增下一轮 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\003-prompt-3.md`
5. 已确认 `农田交互修复V2` 当前状态为：
   - 第 1 刀补强回归完成
   - 第 2 刀代码层第一轮闭环完成
   - 当前不能停在“等用户自己 live 复测”
6. 已为 `农田交互修复V2` 新增下一轮 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\004-第二刀进入live终验与验收分流.md`

**关键决策**
- `项目文档总览`：不再继续写新文案，转入“全面总结 + 验收报告 + 根 README 创建决策”。
- `导航检查`：不再继续折腾结构层，下一轮唯一主线是锁“位移未推进”的第一责任点。
- `农田交互修复V2`：不再接受“等用户自己试”，下一轮必须自己把第二刀跑到 live 终验结果，并按通过 / 失败分流。

**恢复点 / 下一步**
- 现在可以直接把这三份新 prompt 发给对应线程；
- 我这边下一步等待它们的新回执，再决定谁可以正式转验收报告、谁还要继续打下一刀。

## 2026-03-24｜项目文档总览线程已完成内容生产，进入用户决策门
**当前主线目标**
- 继续审 `项目文档总览` 的新回执，判断这条线是否还能继续扩写，还是已经可以视为内容生产完成。

**本轮完成**
1. 已核读：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\项目文档总览\04_第四批次_验收总结与落根决策\26.03.24-项目文档总览最终总结与验收报告.md`
2. 已确认该线程本轮交付物完整覆盖了：
   - 三批次回顾
   - 最终交付物总表
   - 最终推荐主口径
   - 正式验收结论
   - 用户待决策项
3. 已再次核实仓库根：
   - `D:\Unity\Unity_learning\Sunset\README.md` 当前确实不存在

**关键决策**
- 这条线当前没有新的阻塞发现。
- 真实状态应裁定为：
   - `内容生产完成`
   - `线程可关单`
   - 后续只剩用户执行决策，不再需要线程继续补文案或补分析
- 下一步只有两个合法方向：
  1. 立即落根创建仓库根 `README.md`
  2. 暂不落根，线程封板停车

**恢复点 / 下一步**
- `项目文档总览` 现在不需要新的研究型 prompt；
- 如果用户决定现在就落根，可以直接给它发“根 README 创建执行 prompt”；
- 如果用户暂不落根，就只需要给它发“线程封板 / 停车 prompt”。

## 2026-03-24｜向用户口头汇报导航线与农田线当前真实进度
**当前主线目标**
- 用户要求我直接说明另外两条线程目前各自做到哪里、做了什么，以及我在分发 prompt 时是否按它们的真实进度、真实偏移和真实完成度做治理判断。

**本轮完成**
1. 已明确当前两条未收口线程的真实状态：
   - `导航检查`：结构层已完成，当前 blocker 已收敛到“位移执行没真正打到 `Rigidbody2D / Transform`”
   - `农田交互修复V2`：第 1 刀已完成，第 2 刀代码层第一轮闭环已完成，当前只差可信的 live 终验结果
2. 已把我的治理判断口径固定为：
   - `导航检查` 下一轮不再允许继续折腾结构层
   - `农田交互修复V2` 下一轮不再允许停在“等用户手动测”
3. 已确认我前面给它们的 prompt 都是基于：
   - 已完成内容
   - 当前真实 blocker
   - 与最初需求 / 设计之间的偏移点
   - 以及“下一轮最该打哪一个责任点”

**关键决策**
- `导航检查` 的偏移已经从“统一导航没落地”切换为“共享执行层落地了，但位移根本没推进”，所以治理口径也随之切到 movement execution root-cause。
- `农田交互修复V2` 的偏移已经从“箱子爆栈 / 放置系统完全没底”切换为“代码层闭环有了，但 live 验收还没拿到”，所以治理口径随之切到 second-blade live 终验，而不是继续补代码想象。

**恢复点 / 下一步**
- 现在我可以继续按这个口径收这两条线的后续回执；
- 只要它们再回来，我会继续按“实际进度 / 实际 blocker / 实际偏移”裁定，而不是按旧 prompt 机械追。

## 2026-03-24｜spring-day1 与农田线两条新回执已完成分流裁定
**当前主线目标**
- 接住 `spring-day1` 与 `农田交互修复V2` 的两条新回执，继续按治理口径判断：谁还不能停、谁已经完成并该转阶段总结。

**本轮完成**
1. 已确认 `spring-day1` 新回执的真实状态：
   - `DialogueUI` 连续剧情时序 bug 已锁根因并落补丁
   - 静态断言已补齐
   - 但仍停在“请用户复测这 2 件事”
2. 已为 `spring-day1` 新增下一轮 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\26.03.24-连续剧情UI恢复live终验与验收收口.md`
3. 已确认 `农田交互修复V2` 新回执的真实状态：
   - 第二刀 live 终验 `4 / 4 pass`
   - 第二刀验收报告已落盘
   - `fa129567` 已提交推送
4. 已为 `农田交互修复V2` 新增下一轮 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\005-第二刀完成后转阶段总结与后续刀次移交.md`

**关键决策**
- `spring-day1`：还不能算完成；下一轮不许再停在“让用户复测”，必须自己拿 live 结果，或者锁到首个未恢复 UI 根物体。
- `农田交互修复V2`：第二刀已经完成；下一轮不再继续修代码，而是转“阶段总结 + 后续刀次移交”。

**恢复点 / 下一步**
- 现在可以把这两份新 prompt 直接发给对应线程；
- 我这边下一步等它们的新回执，再继续判断 `spring-day1` 是否能转 focused 验收报告，以及 `农田` 是否该正式进入第三刀。

## 2026-03-24｜按用户撤回要求取消 spring-day1 管控，并只处理农田与导航
**当前主线目标**
- 用户明确要求撤回此前所有对 `spring-day1` 的治理内容，本轮只处理真正的两条线程：`农田交互修复V2` 与 `导航检查`。

**本轮完成**
1. 已按用户撤回要求，删除此前误发的：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\26.03.24-连续剧情UI恢复live终验与验收收口.md`
2. 已确认本轮用户真正要处理的两条回执是：
   - `农田交互修复V2`：第二刀完成后的阶段总结回执
   - `导航检查`：长时运行后的详细中间汇报
3. 已为 `农田交互修复V2` 新增停车 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\006-第二刀封板停车并等待第三刀放行.md`
4. 已为 `导航检查` 新增下一轮 focused prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\004-prompt-4.md`

**关键决策**
- `spring-day1`：本轮不再纳入治理，不再继续给它发 prompt。
- `农田交互修复V2`：第二刀与阶段总结都已完成，当前正确状态是“封板停车，等待第三刀放行”。
- `导航检查`：当前不能再 full-run 乱飞，下一轮只保留单场 `PlayerAvoidsMovingNpc`，并把第一责任点压到 `NavigationLocalAvoidanceSolver` 或更具体的条件分支。

**恢复点 / 下一步**
- 现在可以只把 `006` 和 `004-prompt-4` 发给农田与导航两条线；
- 我这边下一步继续按这个新口径收它们的回执，不再碰 `spring-day1`。

## 2026-03-24｜农田线阶段总结已完成，当前切入“第三刀待放行停车”
**当前主线目标**
- 接住 `农田交互修复V2` 的阶段总结回执，判断它现在是否还要继续推进，还是应该进入停车待放行。

**本轮完成**
1. 已核读：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\阶段总结与后续刀次移交报告_2026-03-24.md`
2. 已确认该报告已经完整覆盖：
   - 当前阶段边界
   - 第 1 / 第 2 刀成果总表
   - 后续刀次建议排序
   - 用户待裁决项
3. 已新增停车待放行 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\006-第三刀待放行停车.md`

**关键决策**
- `农田交互修复V2` 当前不再继续写代码，也不再继续扩写文档。
- 它的真实状态应裁定为：
   - 第 2 刀已完成
   - 第 3 刀入口已明确
   - 当前等待用户是否放行第 3 刀

**恢复点 / 下一步**
- 现在这条线如果要继续推进，应只发“停车待放行” prompt；
- 只有用户明确放行第 3 刀后，才再进入新的实现 prompt。

## 2026-03-24｜治理续办恢复核查已完成，当前只保留农田与导航两条 live 线程
**当前主线目标**
- 在新的治理续办会话里先核准“哪些线程还在治理面上、哪些 prompt 现在算 live”，避免沿着旧记忆继续漂移。

**本轮完成**
1. 已按当前 Sunset 治理口径完成只读 preflight：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `HEAD = 574dcee6`
   - `@{upstream}...HEAD = 0 / 0`
2. 已再次确认 `spring-day1` 不再纳入本轮治理：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\26.03.24-连续剧情UI恢复live终验与验收收口.md` 当前确实不存在
3. 已确认当前仍在位的两份治理 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\004-prompt-4.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\006-第二刀封板停车并等待第三刀放行.md`
4. 已额外核到农田目录当前还存在更新后的停车版：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\006-第三刀待放行停车.md`
   - 这说明农田线的停车口径后来又被收紧了一次；当前更收口的 live 口径应以 `006-第三刀待放行停车.md` 为准，旧 `006-第二刀封板停车并等待第三刀放行.md` 保留历史批次证据身份

**关键决策**
- 当前治理面只保留两条线程：
  - `农田交互修复V2`
  - `导航检查`
- `spring-day1` 已撤回，不再继续给它发治理 prompt。
- 当前这轮不新增新的业务 prompt：
  - 农田线维持“停车待放行第三刀”
  - 导航线维持“只保留单场 `PlayerAvoidsMovingNpc` 并压 solver / 条件分支”

**恢复点 / 下一步**
- 下一步继续等这两条线程回执，再按真实完成度裁定：
  - 农田如果仍在继续写代码，要拦停；
  - 导航如果已跑通最后一条，就转阶段验收；若未跑通，就继续把责任点压到更具体方法 / 条件分支。

## 2026-03-24｜用户正式驳回农田两份 006 停车 prompt，当前先回到“无新 prompt”状态
**当前主线目标**
- 响应用户对农田线治理口径的直接驳回，撤掉我此前自行生成的两份 `006` 停车 prompt，避免继续把不合适的治理话术留在工作区里。

**本轮完成**
1. 已按用户明确要求删除：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\006-第二刀封板停车并等待第三刀放行.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\006-第三刀待放行停车.md`
2. 已把当前农田线的治理事实收回到更保守的状态：
   - 第二刀验收报告仍有效
   - 阶段总结与后续刀次移交报告仍有效
   - 但当前不再保留新的 `006` prompt 作为 live 指令

**关键决策**
- 用户已明确驳回这两份 `006` 的治理表达；后续如果农田线需要新 prompt，必须重新思考后再单独生成，不能继续沿用这次停车口径。
- 当前农田线的 live 事实仍然是：
  - 第二刀已完成
  - 第三刀未放行
  - 现阶段以阶段总结 / 验收文档为准，不追加新的治理 prompt

**恢复点 / 下一步**
- 暂不再给农田线追加新的 `006`；
- 等用户后续若要我重新设计农田下一轮治理 prompt，再从零生成新的版本。

## 2026-03-24｜农田新 006 已重写为“续工裁决入口 + 用户补充区”
**当前主线目标**
- 按用户新的引导重做农田续工 prompt：不再写“停车”，而是显式给用户留下继续补充、继续裁决、继续改口径的位置。

**本轮完成**
1. 已新增新的农田 `006`：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\006-续工裁决入口与用户补充区.md`
2. 新版 `006` 已明确纠正此前两份停车 prompt 的误判：
   - 不再把当前状态定义成“已彻底落地，只差放行”
   - 明确要求线程先回读第二刀验收、阶段总结、以及“用户补充区”
   - 明确为用户保留继续发言的位置
3. 新版 `006` 当前承担的角色改为：
   - 续工入口
   - 用户补充入口
   - 后续裁决入口

**关键决策**
- 当前农田线的治理表达应从“停车待放行”切换为“保留 checkpoint，但继续接受用户补充，再决定怎么续工”。
- 后续如果农田线继续推进，应优先以这份新版 `006` 为准，而不是回到被驳回的两份停车 prompt。

**恢复点 / 下一步**
- 现在农田线已有新的续工入口文件；
- 若用户继续补充农田交互未落地的问题，后续应直接写入或映射到这份 `006` 的“用户补充区”语义下，再让线程继续推进。

## 2026-03-24｜导航新 005 已补成“续工裁决入口 + 用户补充区”
**当前主线目标**
- 把导航线也从“只剩最后一条就继续跑”改成更受控的续工入口，避免线程再次长时间 live 验证、让用户体感成死循环。

**本轮完成**
1. 已新增新的导航 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\005-prompt-5-续工裁决入口与用户补充区.md`
2. 新版导航 `005` 已明确写死：
   - 当前不再回结构层
   - 当前只保留 `PlayerAvoidsMovingNpc`
   - 必须给用户保留继续补充和继续纠偏的位置
   - 必须收短 live 窗口，不再长时间 full-run / 多场来回摆位
3. 当前导航线新的治理表达变为：
   - 续工入口
   - 用户补充入口
   - 后续裁决入口

**关键决策**
- 当前导航线的真实治理要求，不只是“继续打最后一条”，而是“继续打最后一条，但必须明显收紧推进方式与 live 取证边界”。
- 后续如果导航线程继续推进，应优先以这份新 `005` 为准，而不是只沿用旧的 `004-prompt-4`。

**恢复点 / 下一步**
- 现在导航和农田两条线都已经有新版续工入口；
- 下一步可以按新的双文件入口继续发给线程执行，并按最小回执格式继续收件。

## 2026-03-24｜导航工作区 prompt 命名已纠偏回 `002-prompt-*`
**当前主线目标**
- 响应用户对导航工作区命名规则的直接纠偏：该目录下的 prompt 应统一使用 `002-prompt-编号` 口径，而不是我后来漂移出来的 `003/004/005-prompt-*`。

**本轮完成**
1. 已将导航工作区 3 份现行 prompt 文件名统一纠正为：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-3.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-4.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-5.md`
2. 已移除旧命名：
   - `003-prompt-3.md`
   - `004-prompt-4.md`
   - `005-prompt-5-续工裁决入口与用户补充区.md`

**关键决策**
- 当前导航工作区的 prompt 命名规则恢复为：
  - 固定前缀 `002-prompt-`
  - 后缀只用编号
- 后续如再新增导航 prompt，也应继续沿用：
  - `002-prompt-6.md`
  - `002-prompt-7.md`
  而不是再自造新的前缀体系。

**恢复点 / 下一步**
- 现在对用户交付导航 prompt 路径时，应只使用新的 `002-prompt-3 / 4 / 5`；
- 农田线命名本轮保持不变，不在这次纠偏范围内。

## 2026-03-24｜导航线已接到 `002-prompt-5` 回执，下一轮正式切到 NPC detour 恢复链
**当前主线目标**
- 接住导航线程对 `002-prompt-5` 的回执，判断它是继续发散，还是已经把问题压到足够具体的下一责任点。

**本轮完成**
1. 已确认这轮回执满足上一轮治理要求：
   - 已完整吸收用户补充区
   - 已保持单场 `PlayerAvoidsMovingNpc`
   - live 窗口已显著收短
   - `Primary.unity isDirty` 来源已查清
2. 已确认当前第一责任点继续前移，不再停在 solver 参数本身：
   - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
   - `TryHandleSharedAvoidance()`
   - `shouldAttemptDetour && TryCreateDynamicDetour(...)`
   - 以及其后 `OverrideWaypoint` 清理 / 恢复原目标路径的收尾链
3. 已新增下一轮导航 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-6.md`

**关键决策**
- 这轮不是“又卡住了”，而是有效 checkpoint：
  - 玩家已到位
  - 净空已转正
  - 当前失败形态已压成“detour 后没有恢复原目标收尾”
- 下一轮不再允许继续泛泛调 solver 参数；
- 下一轮应直接打 NPC detour 恢复链 root-cause。

**恢复点 / 下一步**
- 现在可直接把 `002-prompt-6.md` 发给导航线程；
- 若下一轮修通最后一条，还必须补跑另外两条，拿 fresh 同轮证据后才能转阶段验收报告。

## 2026-03-24｜NPC 线程现状已回顾，当前不宜进入热场景落点与最终联调
**当前主线目标**
- 回答用户关于 `NPC` 现状的追问：在导航已经被连续鞭策这么多轮之后，`NPC` 这条线现在到底能不能继续，以及应继续到什么程度。

**本轮完成**
1. 已回读 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md` 中的近期收口记录，确认 NPC 线程自己的口径并没有变：
   - 纯 NPC 工具链、气泡样本化、生成器与场景化入口已经基本收拢
   - 剩余三件主事仍是：
     - `Primary.unity` 里的正式场景化落点
     - 导航交付后的 NPC 侧接入
     - 最终联调验收
2. 已对照当前导航最新进度确认：
   - 导航不再卡在“位移不动”
   - 也不再主要卡在 solver 参数本身
   - 当前最新 blocker 已压到：
     - `NPCAutoRoamController.TryHandleSharedAvoidance()`
     - detour 清理 / `OverrideWaypoint` 恢复原目标路径的收尾链
3. 已确认 `NPC` 相关现场当前仍是热状态：
   - `Assets/000_Scenes/Primary.unity` 仍 dirty
   - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
   - `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
   都还在 live 变动面里

**关键决策**
- `NPC` 当前**不能**继续做的事：
  - 进入 `Primary.unity` 做正式 `001/002/003` 的 Home Anchor / 活动范围落点
  - 宣称进入最终联调验收
  - 以当前导航结果为基线做正式场景收口
- `NPC` 当前**可以**继续做的事：
  - 只读回顾导航交付面
  - 整理 NPC 侧接入清单 / 适配清单
  - 明确 `NPCAutoRoamController / NPCMotionController` 在导航最终交付后要核哪些接口与语义
  - 保持 `003` 验证样本与 NPC 工具链可用，但不把它们升级成“最终验收”
- 换句话说：
  - `NPC` 现在不是“完全不能动”
  - 但它现在也还不该进入“热场景落点 + 最终联调”阶段
  - 它最合理的状态是：
    - `低风险准备态`
    - 等导航把 detour 恢复链真正收口后，再做热场景与最终联调

**恢复点 / 下一步**
- 当前对 NPC 最合理的治理口径应改成：
  - `可做低风险接入准备`
  - `暂不放行热场景落点与最终联调`
- 等导航至少满足下面两种条件之一后，再放 NPC 往前走：
  1. `PlayerAvoidsMovingNpc` 真通过，且三条场景补齐 fresh 同轮证据
  2. detour 恢复链虽然未彻底通过，但已锁到稳定可接的最终行为语义，足以让 NPC 侧做适配与联调前准备

## 2026-03-24｜shared root 脏改已分桶，spring 与 farm 清扫令已下发
**当前主线目标**
- 响应用户“先做警匪操作”的要求，先把当前 shared root dirty 盘清，再把清扫任务分给真正需要扫地的线程。

**本轮完成**
1. 已重新核当前 shared root dirty。
2. 已明确当前 dirty **不是只有** `spring` 和 `farm` 两条线：
   - 导航主堆自己仍有活跃 dirty
   - `Primary.unity` 仍属热场景
   - 但本轮按用户要求，先只给 `spring-day1` 与 `农田交互修复V2` 下清扫令
3. 已新增批次分发入口：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-24_批次分发_10_shared-root脏改清扫_spring_farm_01.md`
4. 已新增 spring 清扫 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\26.03.24-shared-root脏改清扫与白名单收口.md`
5. 已新增 farm 清扫 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\007-shared-root脏改清扫与归属核对.md`

**关键决策**
- 本轮不让 `spring` 和 `farm` 代替导航 / NPC / 场景热面扫地。
- spring 当前优先清扫口径是：
  - `DialogueUI.cs`
  - `SpringDay1WorkbenchCraftingOverlay.cs`
  - 4 个 `DialogueChinese * SDF.asset`
- farm 当前优先清扫口径是：
  - 先核准自己当前是否真的只剩 `006-续工裁决入口与用户补充区.md` 这一项 shared root 尾账
- 导航 / NPC / `Primary.unity` 当前不在这轮清扫令范围内。

**恢复点 / 下一步**
- 现在可以直接把批次入口或两份线程专属 prompt 发给 spring 与 farm；
- 下一步等它们各自回执，再判断 shared root 里还剩多少“可扫的他线 dirty”和多少“仍在活跃开发中的 dirty”。

## 2026-03-24｜spring-day1 与导航两份续工 prompt 已核对落盘，可直接分发
**当前主线目标**
- 用户这一轮的治理主线是：先把 `spring-day1` 的新 prompt 做对，再把导航线程的督促 prompt 一并纳入，并确保最终给出的不是“口头说已写”，而是已经真实落盘、可直接发送的版本。

**本轮完成**
1. 已核对 `spring-day1` 新委托存在且可用：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\26.03.24-Day1工作台UI与任务体验重收口委托-02.md`
2. 已核对导航新委托存在且可用：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-10.md`
3. 已复核两侧工作区记忆尾部，确认这轮 prompt 升级原因、约束收紧点与恢复点都已追加，不是只落文件没补 memory。

**关键决策**
- 这轮对用户最重要的稳定事实不是“又生成了两份文档”，而是：
  - `spring-day1` 新 prompt 已把“最初 7 点原文完整摘录 + 本轮新增复测反馈 + 审美硬要求 + 禁止自建平行 crafting 规范”一起收口；
  - 导航新 prompt 已把主刀固定在 `CheckAndHandleStuck()` 往共享执行层退壳，不允许再靠 external blocker 停车，也不允许继续泛讲架构。

**恢复点 / 下一步**
- 现在已经可以直接把这两份 prompt 的最简聊天口径发给对应线程；
- 后续治理只需要等它们回执，再继续做方向审稿或验收裁定，不需要再补造一轮中间文案。

## 2026-03-24｜对当前 Sunset 开发瓶颈的治理诊断
**当前主线目标**
- 用户开始反问“现在的开发是不是已经步入正轨、高效开发”，因此需要对这段时间的多线程推进方式做一次系统性诊断，而不是只盯单个线程的快慢。

**本轮核心结论**
1. 当前开发**还没有进入真正高效的正轨**；它比最乱的时候更有治理，但仍处于“治理在补执行缺口”的阶段。
2. 现在最大的瓶颈不是单点技术，而是 4 个问题叠加：
   - `用户可感知验收` 与 `线程自报 checkpoint` 长期脱节；
   - 架构迁移中间态过长，内部退壳很多、外部体感改善太少；
   - shared root + Unity live + 多线程并发让 compile / scene / hot file 互相卡脖子；
   - 美术 / UI / 交互体验要求很高，但早期缺少足够具体、可验证、可微调的承载方式。
3. 用户当前这套开发方式**不是方向错**，但确实存在明显毛病：
   - prompt 轮次很多；
   - 线程容易用“部分完成”“结构 checkpoint”替代真实交付；
   - 如果没有治理线程持续拽着，它们会自然漂向“讲方向、讲骨架、讲 blocker”，而不是直接交出你肉眼能验到的结果。

**关键判断**
- 真正的问题不是“分阶段开发”本身，而是过去一段时间的阶段定义更多按线程内部结构来切，而不是按用户最终可见结果来切。
- 用户这几轮持续逼它们回到：
  - 原始需求全文
  - 真实 scene 体感
  - 同轮 fresh live
  - 不能拿 external blocker 停车
  实际上是在把开发重新拉回正轨。

**恢复点 / 下一步**
- 后续治理应继续收紧为：
  1. 每轮只认一两个用户可见目标；
  2. 线程回执必须把“结构进展”和“体验进展”分开；
  3. 不再接受“内部退壳很多，但用户眼里没变化”的假完成；
  4. 对需要 fresh compile / live 的线，尽量缩短并发面，减少被他线 blocker 拖住的机会。

## 2026-03-24｜“为什么一个初始 prompt 很难一次做完所有任务”的补充结论
**当前主线目标**
- 用户进一步指出：线程总在反复修，不能从一个初始 prompt 直接完成全部任务；需要明确这是不是执行者能力问题，还是任务建模方式本身就有结构性缺陷。

**本轮补充判断**
1. 这不只是“线程笨”，更是当前任务类型决定了**很难靠单个初始 prompt 一次闭环**：
   - 需求里混着功能、体验、审美、架构、数据、运行态验证；
   - 很多标准只有进 Unity live、看真实画面、摸真实手感后才会暴露；
   - 用户自己也是边看结果边补精细要求，这很正常，但会天然打破“首 prompt 一次全中”的幻想。
2. 真问题不在“不能一刀完成所有内容”，而在于：
   - 线程没有把每一轮收成**真正可见、可验、可冻结**的一刀；
   - 经常用“部分结构进展”冒充“整体完成”。
3. 所以后续最合理的治理方向不是追求“一个 prompt 打穿全部”，而是：
   - 一条主线拆成少数几个真正垂直切片；
   - 每一刀都必须交出用户肉眼能验的结果；
   - 未完成项要老老实实留在下一刀，不准口头吞并。

**恢复点 / 下一步**
- 后续 prompt 设计应继续避免“大而全一次打穿”的幻觉；
- 但也不能退回无限碎片化，而应控制成“少轮次、强垂直、强验收”的开发节奏。

## 2026-03-24｜关于“每次写入都不该把 Unity 改到不能用”与“Play 很慢改善”的治理判断
**当前主线目标**
- 用户继续追问两个实际痛点：
  1. 为什么线程一改文件就容易把 Unity 带进编译报错、暂时不可用的状态；
  2. 另一个线程对 Unity Play 前卡顿的改善，到底算不算抓到了真问题。

**本轮判断**
1. “每次写入都完全零报错”如果理解成**编辑过程一秒都不允许红**，不现实；
   但如果理解成**任何 checkpoint / sync / 留场给别人之前都必须恢复到可编译可用**，这是完全合理、而且应该成为硬纪律。
2. 当前出现的 `TreeController` 红错就属于**本不该长时间存在的低级可避免错误**：
   - `TreeController.cs:1059` 使用了 `energyCost`，但当前方法作用域里并没有这个局部变量；
   - 这类错误说明线程在共享现场里留下了未闭合的改动事务，而不是复杂技术难题。
3. `PlayerThoughtBubblePresenter.cs:220` 的 `CS0618` 只是 obsolete warning，不该阻断使用；
   - 它应列入“顺手清理”的体验债；
   - 不应和真正让 Unity 不可编译的红错混在一起。
4. 对 Play 很慢这条支线，我认可“抓到了第一刀真凶”：
   - `StaticObjectOrderAutoCalibrator.cs` 原先在 `ExitingEditMode` 每次进 Play 前都自动全场景扫描；
   - 现在改成默认关闭自动执行、保留手动菜单和开关，这是正确的降噪与减负。
5. 但这还不能宣布“慢启动问题彻底解决”：
   - `EditorSettings.asset` 显示当前仍是完整 Enter Play Mode 重载配置；
   - `Assets/Editor` 里仍有其他编辑器期钩子，如 `PersistentIdAutomator.cs`；
   - 日志里的 `Asset Pipeline Refresh` / `Domain Reload` 仍说明后面还有第二层瓶颈。

**关键决策**
- 后续治理对“编译可用性”的口径要改成三层：
  1. 编辑中短暂红：允许，但应尽量缩短；
  2. 离开本轮前仍红：不允许；
  3. 白名单 sync / 交接 / 让别人接 Unity 时仍红：严重违规。
- 对“Unity 很慢”这条线的口径则应改成：
  - 当前第一刀有效；
  - 但只算定位并削掉了一个明显的 Enter Play 前重活，不算总问题收完。

**恢复点 / 下一步**
- 后续线程若继续写共享现场代码，应更严格执行“最小事务窗口 + 最小编译闸门”；
- 如果接着追 Play 慢，应优先继续审：
  1. 完整 Domain Reload 是否可进一步收短；
  2. 其他 `InitializeOnLoad` / `sceneSaving` 编辑器脚本；
  3. `Asset Pipeline Refresh` 的资产刷新来源；
  4. asmref / 原生扩展噪音是否在放大重载成本。

## 2026-03-24｜两条口头支线已落成真实 skill，并已接入 Sunset 规则
**当前主线目标**
- 用户要求把前面我口头提出却未落盘的两条治理支线真正固化：
  1. “单轮 prompt 必须形成可验硬切片”
  2. “任何代码任务停手 / sync / handoff 前必须零 owned red”

**本轮完成**
1. 已新建本地 skill：
   - `C:\Users\aTo\.codex\skills\sunset-prompt-slice-guard\`
   - `C:\Users\aTo\.codex\skills\sunset-no-red-handoff\`
2. 两个 skill 都已带上：
   - `SKILL.md`
   - `agents/openai.yaml`
3. 已把它们接进：
   - [AGENTS.md](/D:/Unity/Unity_learning/Sunset/AGENTS.md)
   - [global-skill-registry.md](/C:/Users/aTo/.codex/memories/global-skill-registry.md)

**关键决策**
- `sunset-prompt-slice-guard`：
  - 用来约束 Sunset 的续工 prompt、回执后回拉 prompt、验收失败后的单轮切片；
  - 核心是“每轮只打一刀，且这刀必须用户可见、可验、可冻结”。
- `sunset-no-red-handoff`：
  - 用来约束 Sunset 代码任务；
  - 允许编辑中短暂红，但不允许在停手、sync、交接 Unity 使用权时仍留 owned red。

**恢复点 / 下一步**
- 后续在 Sunset 里写续工 prompt，应默认按 `sunset-prompt-slice-guard` 口径执行；
- 后续在 Sunset 里做代码修改，应默认把 `sunset-no-red-handoff` 当作常驻纪律，而不是等用户再提醒。

## 2026-03-24｜Unity Play Busy 支线 prompt 与导航 `002-prompt-11` 已落盘
**当前主线目标**
- 用户要求在完成上面两条 skill 落地后，再补：
  1. Unity Play Busy / 启动优化支线的下一轮 prompt
  2. 导航基于最新回执的下一轮 prompt

**本轮完成**
1. 已在全局排障线程下新增：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\2026-03-24-UnityPlayBusy续查prompt-01.md`
   - 口径已收紧为：先验证第一刀真实收益，再继续查下一组 Editor-only 真责任点；严禁无证据乱动全局设置与业务代码。
2. 已在导航工作区新增：
   - [002-prompt-11.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/002-prompt-11.md)
   - 口径已收紧为：接受第二个退壳 checkpoint，但下一刀唯一主刀固定为 `detour lifecycle` 继续退壳。

**关键决策**
- 性能线这次不准再“自由散查”，而是必须：
  - 先测第一刀有效性
  - 再锁第二刀
  - 且只允许发生在 Editor-only、可回退、可验证边界内
- 导航线这次不准再在两个候选方向之间摇摆；
  - 我已经替它把下一刀钉到 `detour create / clear / recover`

**恢复点 / 下一步**
- 性能线现在已经有可直接发送的受控 prompt；
- 导航线现在已经有可直接发送的 `002-prompt-11`；
- 农田这轮用户说“农田也回复了”，但实际贴出的仍是同一份导航回执，我没有伪造农田 prompt，等待真实农田回执再处理。

## 2026-03-25｜基于用户重贴的真实 spring / farm 回执，已补齐两份新 prompt
**当前主线目标**
- 用户明确提醒我不要漂移，要按他重新完整粘贴的 spring / farm 回执继续当前 prompt 生成主线，而不是沿用我之前脑内摘要。

**本轮完成**
1. 已基于用户重贴后的完整 `spring-day1` 回执、旧委托链和图片验收，新增：
   - [26.03.25-Day1工作台UI与任务体验重收口委托-03.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/26.03.25-Day1工作台UI与任务体验重收口委托-03.md)
2. 已基于用户重贴后的真实农田回执、`001 / 006 / 当前续工计划与日志`，新增：
   - [008-新增工具运行时与玩家反馈链进入live验收与补口.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/农田系统/2026.03.16/1.0.4交互全面检查/008-新增工具运行时与玩家反馈链进入live验收与补口.md)

**关键决策**
- `spring-day1` 这轮不再接受“代码侧收口，等用户复测”：
  - 新 prompt 已把用户贴图、工作台 `0.5m` 交互包络线语义、任务逻辑死锁、任务页翻页体验、提示气泡双语义一起收进更硬的一刀。
- 农田这轮不再接受“日志和脚本级闭环已完成”：
  - 新 prompt 已把下一轮唯一主刀固定为 4 组 Unity / MCP live 行为验收；
  - 若 live 不过，就同轮继续补口，不准再把第一轮逻辑验收甩给用户。

**恢复点 / 下一步**
- 现在 spring 与 farm 都已有可直接发送的新 prompt；
- 后续如果用户继续贴回执，我应按 `sunset-prompt-slice-guard` 的口径继续审：
  1. 这轮到底是不是单刀
  2. 线程有没有把第一轮真实验收继续甩给用户
  3. 结构线和体验线有没有继续混说

## 2026-03-25｜验收交接 skill 与 Unity stop-early 纪律已真正落盘，并补齐性能/导航下一轮 prompt
**当前主线目标**
- 用户要求把前面讨论过却还没真正落地的两条治理结论做成真实规则：
  1. 线程到“需要用户终验”时，不能只说“等用户复测”，必须交专业验收指南与回执单。
  2. Unity / MCP live 一旦拿到当前步骤所需证据，就必须立刻 `Pause / Stop`，不能放任日志洪水继续刷。
- 同时，用户补充了第五个任务：基于导航线程最新回执继续下达下一轮 prompt。

**本轮完成**
1. 已新建本地 skill：
   - `C:\Users\aTo\.codex\skills\sunset-acceptance-handoff\`
2. 已更新现有 skill：
   - `C:\Users\aTo\.codex\skills\sunset-unity-validation-loop\SKILL.md`
3. 已同步接线到：
   - [AGENTS.md](/D:/Unity/Unity_learning/Sunset/AGENTS.md)
   - [global-skill-registry.md](/C:/Users/aTo/.codex/memories/global-skill-registry.md)
   - [Sunset当前规范快照_2026-03-22.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset当前规范快照_2026-03-22.md)
   - [并发线程_当前版本更新前缀.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/共享根执行模型与吞吐重构/01_执行批次/2026.03.21_main-only极简并发开发_01/可分发Prompt/并发线程_当前版本更新前缀.md)
   - [README.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/22_恢复开发分发与回收/线程回收/README.md)
4. 已新增性能线下一轮 prompt：
   - [2026-03-25-UnityPlayBusy续查prompt-02.md](/D:/迅雷下载/开始/.codex/threads/系统全局/谁是卧底/2026-03-25-UnityPlayBusy续查prompt-02.md)
5. 已新增导航线下一轮 prompt：
   - [002-prompt-12.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/002-prompt-12.md)

**关键决策**
- `sunset-acceptance-handoff` 把“线程先自验，再交用户终验包”正式收成 skill；
- `sunset-unity-validation-loop` 现在明确区分：
  - MCP 擅长逻辑自验与短窗证据
  - 人工终验擅长观感、手感、节奏与最终裁决
- Sunset live 规则现在正式新增两条硬纪律：
  - 取够证据立刻 `Pause / Stop`
  - 进入用户终验阶段必须交验收指南 + 回执单
- 性能线这轮被收紧成：
  - 只验证 `SpringDay1BedSceneBinder.cs / SpringDay1WorkbenchSceneBinder.cs` 是否真拖慢 `ProcessInitializeOnLoadAttributes`
- 导航线这轮被收紧成：
  - 只打 shared detour `clear / recover` 过密问题，不再横跳别的责任簇

**恢复点 / 下一步**
- 后续所有 Sunset 线程若进入“等用户终验”阶段，应默认按 `sunset-acceptance-handoff` 组织回执；
- 后续所有 Unity / MCP live 取证，都应默认按 stop-early 纪律先定义停机条件、拿到证据后立即退回 Edit Mode；
- 性能线下一轮直接发 `prompt-02`；
- 导航线下一轮直接发 `002-prompt-12`。

## 2026-03-25｜性能线 `prompt-02` 回执裁定：只接受取证补丁 checkpoint，不接受任务完成
**当前主线目标**
- 用户贴回 `谁是卧底` 对 `prompt-02` 的最新回执，要求我继续给出可直接转发的回应 prompt，而不是泛泛汇报。

**本轮新增稳定事实**
1. 我接受它这轮只做到一个中间 checkpoint：
   - `SpringDay1BedSceneBinder.cs / SpringDay1WorkbenchSceneBinder.cs`
   - 默认关闭、可回退的 diagnostics 与 auto-bind 短路开关已落地
2. 但我不接受它把这轮表述成已完成：
   - 因为 `Play -> Stop = 0 次`
   - 当前新的第一责任点还没有继续从“两个 Binder 候选”收窄到更硬证据
3. 这轮还出现了 scope 漂移信号：
   - `C:\Users\aTo\.codex\AGENTS.md` 被顺手纳入 changed_paths
   - 这不属于当前 Unity Play Busy 主刀
4. 当前 external blocker 是真实的，但它只能解释“为什么这轮没跑成 live”，不能被包装成“本轮目标已完成”。

**恢复点 / 下一步**
- 下一轮给性能线的回应 prompt 应继续钉死：
  1. 当前只接受“Binder 取证补丁已就位”这个 checkpoint
  2. 不接受继续做治理 / 工具侧支线
  3. 一旦共享现场 compile red 清掉，立刻按既定方案跑最小 `Play -> Stop` 取证

## 2026-03-25｜Unity Play Busy 续查：第一刀收益已复测，但总瓶颈未下降
**当前主线目标**
- 按 `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\2026-03-24-UnityPlayBusy续查prompt-01.md` 的约束，先验证 `StaticObjectOrderAutoCalibrator.cs` 这一刀的真实收益；如果仍慢，只锁下一组 Editor-only 责任点，不越过高风险边界。

**本轮完成**
1. 手工等价执行了 Sunset 启动闸门：
   - 复核 `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - 复核 `shared-root-branch-occupancy.md`、`mcp-single-instance-occupancy.md`、`mcp-live-baseline.md`、`mcp-hot-zones.md`
   - 确认 shared root 仍在 `main`，MCP 基线是 `unityMCP @ http://127.0.0.1:8888/mcp`
2. 对实例 `Sunset@21935cd3ad733705` 做了一次最小受控复测：
   - `Play -> 等待进入 -> Stop -> 回到 Edit Mode`
   - 没有改全局 Play 设置、没碰业务运行时代码、没开后台 watcher
3. 从 `Editor.log` 新增片段中抽出本轮样本：
   - 取证文件：`D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\unity-play-retest-2026-03-24.log`
4. 完成了第一刀前后对比：
   - 新样本 `Domain Reload Profiling = 19053ms`
   - 新样本 `ProcessInitializeOnLoadAttributes = 6120ms`
   - 新样本 `AwakeInstancesAfterBackupRestoration = 7277ms`
   - 新样本 `Asset Pipeline Refresh = 19.599s`
   - 对比上一轮已接受基线（约 `18.6~18.8s / 5.8~5.9s / 7.24s / 19.3~25.1s`），关键重段几乎没有下降
5. 复核本轮新样本中已不再出现 `StaticObjectOrderAutoCalibrator` / `[AutoCalibrator]` 的进 Play 前扫描日志，说明第一刀确实把那条 Editor-only 前置扫描链打掉了；但这不足以解释当前整体慢的问题。
6. 收缩了下一组项目自有 Editor-only 候选：
   - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1BedSceneBinder.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1WorkbenchSceneBinder.cs`
   - 它们都在 `[InitializeOnLoad]` 下注册，并且在 `Primary` 场景里存在全层级 `Transform` 扫描路径，最贴近当前仍然很重的 `ProcessInitializeOnLoadAttributes` 段

**关键结论**
- 第一刀“有效”，但有效性只体现在“移除了一个真实存在的进 Play 前扫描源”，没有换来可感知的整体耗时下降。
- 当前不能把 Unity Play Busy 宣布为已解决。
- 当前新的第一责任点已经从 `StaticObjectOrderAutoCalibrator.cs` 转移到：
  - `Editor.log` 的 `Domain Reload Profiling -> SetupLoadedEditorAssemblies -> ProcessInitializeOnLoadAttributes (6120ms)` 段
  - 在项目自有脚本里，最值得继续收缩的下一组是两个 `SpringDay1*SceneBinder` 的 `[InitializeOnLoad]` 绑定链
- 本轮没有做第二刀：
  - 因为“还有项目自有候选”这件事已经成立，但还没硬到足以安全改动单一文件并宣布它就是下一刀真凶
  - 按 prompt 边界，本轮先停在“真实复测 + 锁下一组责任点”

**恢复点 / 下一步**
- 如果继续沿这条支线推进，下一轮最小动作应是：
  1. 只针对 `SpringDay1BedSceneBinder.cs` / `SpringDay1WorkbenchSceneBinder.cs` 增加可回退、默认关闭的取证或开关；
  2. 再做一次同规格最小复测；
  3. 只要新证据仍指向 `ProcessInitializeOnLoadAttributes`，再决定是否做第二刀。

## 2026-03-25｜Unity Play Busy prompt-02：Binder 取证补丁已落地，但 live 复测被无关 compile red 阻断
**当前主线目标**
- 按 `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\2026-03-25-UnityPlayBusy续查prompt-02.md`，只验证：
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1BedSceneBinder.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1WorkbenchSceneBinder.cs`
  是否真的是 `ProcessInitializeOnLoadAttributes` 里的下一组责任点。

**本轮完成**
1. 先把用户上条工具支线真正落成硬规则：
   - `C:\Users\aTo\.codex\AGENTS.md`
   - 新增“搜索默认口径”：搜索类任务默认优先 `rg` / `rg --files`，系统对象查询继续优先 PowerShell。
2. 在两个 Binder 内只加入了默认安全、可回退、Editor-only 的最小取证能力：
   - 新增诊断日志开关（默认关闭）
   - 新增 Auto Bind 短路开关（默认关闭，即默认行为不变）
   - 新增日志字段：场景名、Transform 总数、候选数、实际创建数、单次执行耗时
3. 单文件校验结果：
   - 两个 Binder 通过 `validate_script`
   - 仅有通用 warning：`Consider null checking GetComponent results`
4. 现场 blocker 已明确：
   - 当前 Unity Console 已有无关 compile red，来自：
     - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
     - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
     - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
   - 这些都不在本轮允许改动范围内
   - 因此本轮尚未进入任何一次 `Play -> Stop` 复测

**关键结论**
- 这轮没有漂移到别的 Editor 脚本，也没有碰场景、Prefab、全局 Play 设置或业务运行时代码。
- 两个 Binder 的取证补丁已经就位，但当前还不能给出“它们是不是下一真责任点”的 live 结论，因为共享现场被无关 compile red 先行阻断。
- 当前准确口径不是“Binder 被排除”或“Binder 已坐实”，而是：
  - “这轮主刀仍只锁这两个 Binder”
  - “取证手段已经就位”
  - “下一步先清掉无关 compile red，才能合法执行 2~3 次最小复测”

**恢复点 / 下一步**
- 一旦无关 compile red 被别的责任线程清掉，后续可以直接继续：
  1. 开启两个 Binder 的 diagnostics；
  2. 先跑一轮默认行为样本；
  3. 再用各自的短路开关做对照样本；
  4. 每次取够证据立刻 Stop，并退回 Edit Mode。

## 2026-03-25｜已按相同分发口径补齐农田与 spring-day1 的新文件版 prompt

**当前主线目标**
- 用户继续贴回农田与 `spring-day1` 的最新回执，并要求我完全按同样方式处理：
  - 详细 prompt 写进文件
  - 聊天里只保留一条极短的分发话术

**本轮完成**
1. 已新增农田详细文件：
   - [009-hover遮挡链闭环与live收口.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/农田系统/2026.03.16/1.0.4交互全面检查/009-hover遮挡链闭环与live收口.md)
2. 已新增 `spring-day1` 详细文件：
   - [26.03.25-Day1工作台UI与任务体验重收口委托-04.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/26.03.25-Day1工作台UI与任务体验重收口委托-04.md)

**关键决策**
- 农田线当前不再泛跑 4 组 live，而是只锁：
  - `hover-occlusion-chain`
  - `previewBounds=null`
  这一处精确剩余点
- `spring-day1` 当前不再继续堆代码，而是：
  - 等 shared root 的 farm 编译红字解除
  - 立刻做 Play 自验
  - 然后交正式验收指南与回执单

**恢复点 / 下一步**
- 后续如果用户要直接分发：
  - 农田发 `009`
  - `spring-day1` 发 `委托-04`
- 聊天层继续只保留文件路径与一句短消息。

## 2026-03-25｜Unity Play Busy prompt-03：已完成 3 组 Binder 对照样本，结论是“两个 Binder 不是这一刀真核心”
**当前主线目标**
- 按 `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\2026-03-25-UnityPlayBusy续查prompt-03.md`，在 compile red 解除后立刻完成 3 组最小 `Play -> Stop` 对照样本，只验证：
  - `SpringDay1BedSceneBinder.cs`
  - `SpringDay1WorkbenchSceneBinder.cs`
  是否真对 `ProcessInitializeOnLoadAttributes` 有关键影响。

**本轮完成**
1. 复核 compile red 已解除，MCP 基线仍为 `unityMCP @ 127.0.0.1:8888`，实例仍是 `Sunset@21935cd3ad733705`。
2. 只使用已经存在的 Binder 取证补丁，未再新增实现，完成了 3 组最小样本：
   - baseline：两个 Binder 正常启用
   - 只短路 BedBinder
   - 只短路 WorkbenchBinder
3. 每组进入 Play 成功后都立即 Stop，并在结束后确认回到 Edit Mode。
4. 保存的取证样本：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\binder-sample-1-baseline.log`
   - `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\binder-sample-2-bed-disabled.log`
   - `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\binder-sample-3-workbench-disabled.log`
5. 取证结果统一采用各样本中明确 `Reloading assemblies for play mode` 后的那组 Domain Reload 数据做对照：
   - baseline：
     - `Domain Reload Profiling = 24058ms`
     - `ProcessInitializeOnLoadAttributes = 7913ms`
     - `AwakeInstancesAfterBackupRestoration = 9055ms`
     - `Asset Pipeline Refresh = 24.705s`
   - 只短路 BedBinder：
     - `Domain Reload Profiling = 23901ms`
     - `ProcessInitializeOnLoadAttributes = 7928ms`
     - `AwakeInstancesAfterBackupRestoration = 9109ms`
     - `Asset Pipeline Refresh = 24.568s`
   - 只短路 WorkbenchBinder：
     - `Domain Reload Profiling = 24124ms`
     - `ProcessInitializeOnLoadAttributes = 7963ms`
     - `AwakeInstancesAfterBackupRestoration = 9181ms`
     - `Asset Pipeline Refresh = 24.797s`
6. Binder diagnostics 关键信息：
   - baseline 非 busy 样本：
     - Bed：`scene=Primary transforms=471 candidates=0 created=0 elapsedMs=4`
     - Workbench：`scene=Primary transforms=471 candidates=1 created=0 elapsedMs=0`
   - 只短路 BedBinder：
     - Bed：`skipped=disabled scene=Primary`
     - Workbench：`scene=Primary transforms=855 candidates=1 created=0 elapsedMs=4`
   - 多个实际 Play 进入窗口内，两者都重复出现：
     - `skipped=editor-busy`
7. 收口前已把现场恢复到默认安全状态：
   - Bed / Workbench Auto Bind 都恢复为 enabled
   - 两个 diagnostics 都恢复为 disabled

**关键结论**
- 这两个 Binder 不是这一刀的真核心，至少不足以解释当前 `ProcessInitializeOnLoadAttributes` 约 `7.9s` 的重段。
- 证据链有两层：
  1. 三组样本里，短路 Bed 或 Workbench 后，`ProcessInitializeOnLoadAttributes` 不降反升（`7913ms -> 7928ms / 7963ms`），没有出现明显下降；
  2. Binder 自身 diagnostics 显示：
     - 真正跑到其执行体时，扫描代价只有 `0~4ms`
     - 实际 Play 进入窗口里更常见的是 `skipped=editor-busy`
- 因此当前新的第一责任点已经从“两处 Binder 候选”继续收窄为：
  - `ProcessInitializeOnLoadAttributes` 里的非 Binder 初始化链
  - 也就是：这两个 Binder 的 auto-bind 执行体已基本被排除，不应再继续把下一刀押在它们身上

**恢复点 / 下一步**
- 这轮对 Binder 的证据已经闭环，后续不应再继续围绕这两个文件做同类对照；
- 下一轮如果继续推进，应从 `ProcessInitializeOnLoadAttributes` 段里的非 Binder 初始化链重新收窄，而不是回头重复这两个 Binder。

## 2026-03-25｜Unity Play Busy prompt-04：双线并查后，`SaveManager` 被排除，单一第一责任点继续收窄到 `unity-mcp` 包层 `InitializeOnLoad`

**当前主线目标**
- 按 `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\2026-03-25-UnityPlayBusy续查prompt-04.md`，不再围着 Binder 打转，而是通过：
  - `SaveManager` / `AwakeInstancesAfterBackupRestoration`
  - 非 Binder `ProcessInitializeOnLoadAttributes`
  两条证据线并查，继续收窄出单一第一责任点。

**本轮完成**
1. 已给 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs` 加入默认关闭、可回退、editor-only 的最小取证能力：
   - Diagnostics 开关
   - Disable Editor AssetDatabase Fallbacks 开关
2. 已完成 2 次最小 `Play -> Stop`：
   - baseline
   - 禁用 `SaveManager` editor fallback
   每次都在拿到 `ProcessInitializeOnLoadAttributes / AwakeInstancesAfterBackupRestoration / Asset Pipeline Refresh` 与 `[SaveManager][Diag]` 后立刻 `Stop`，最后退回 `Edit Mode`。
3. 已保存样本：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\prompt04-sample-1-baseline.log`
   - `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\prompt04-sample-2-saveManager-fallback-disabled.log`
4. baseline 样本：
   - `ProcessInitializeOnLoadAttributes = 8906ms`
   - `AwakeInstancesAfterBackupRestoration = 10281ms`
   - `Asset Pipeline Refresh = 28.037s`
   - `[SaveManager][Diag] ... elapsedMs=15~16 ... prefabDbAssetDbFallbackHit=True ... dynamicFactoryInitialized=True`
5. 禁用 `SaveManager` editor fallback 后：
   - `ProcessInitializeOnLoadAttributes = 9063ms`
   - `AwakeInstancesAfterBackupRestoration = 10209ms`
   - `Asset Pipeline Refresh = 28.012s`
   - `[SaveManager][Diag] ... elapsedMs=0 ... prefabDbAssetDbFallbackHit=False ... dynamicFactoryInitialized=False`

**关键结论**
- `SaveManager` 确实稳定命中 `AssetDatabase` fallback，但它自身的取证耗时只有 `15~16ms`，禁用 fallback 后也没有让重段出现能切下一刀的明显下降：
  - `AwakeInstancesAfterBackupRestoration` 只下降约 `72ms`
  - `Asset Pipeline Refresh` 只下降约 `25ms`
  - `ProcessInitializeOnLoadAttributes` 反而上升约 `157ms`
- 因此 `SaveManager` 已被排除为这一刀的主真凶。
- 同时，项目内剩余非 Binder `InitializeOnLoad` 候选也明显偏弱：
  - `PersistentIdAutomator.cs` 只在静态构造中注册 `sceneSaving`
  - `DialogueChineseFontAssetCreator.cs` 属于 `InitializeOnLoadMethod`，且更贴近 `ProcessInitializeOnLoadMethodAttributes`
- 当前新的单一第一责任点已经继续收窄到：
  - `D:\Unity\Unity_learning\Sunset\Library\PackageCache\com.coplaydev.unity-mcp@e9de4c0341cf\Editor\Services\Transport\Transports\StdioBridgeHost.cs`
  - 它代表的 `unity-mcp` 包层 `InitializeOnLoad` 集群

**恢复点 / 下一步**
- 这轮之后不应再继续把下一刀押在 `SaveManager` 或项目内剩余的轻量非 Binder 入口上；
- 下一刀若继续推进，应优先围绕 `unity-mcp` 包层 `InitializeOnLoad` 集群，先从 `StdioBridgeHost.cs` 领跑取证，而不是回头重做 `SaveManager` 对照。

## 2026-03-25｜Unity Play Busy 线补记：已补做“我到底临时禁用了什么、设计初衷是什么、哪些不能乱动”的安全性复盘

**当前主线目标**
- 用户在 `prompt-04` 收口后，明确要求我先停下，不继续下一刀，先回答：
  - 我临时禁用过的东西到底来自哪里
  - 它们分别做什么
  - 设计初衷是什么
  - 我是否真的做过足够深入的理解与边界判断

**本轮补到的稳定事实**
1. 我这条线里真正“临时禁用过”的内容，只有三类，而且都已恢复默认状态：
   - `SpringDay1BedSceneBinder` 的 Auto Bind
   - `SpringDay1WorkbenchSceneBinder` 的 Auto Bind
   - `SaveManager` 的 editor `AssetDatabase` fallback
2. 两个 Binder 都来自：
   - `Assets/Editor/Story/*.cs`
   - 都是 Editor-only 的 `[InitializeOnLoad]` 自动补挂器
   - 设计初衷是：当 `Primary` 场景里出现目标对象但缺少交互组件时，自动补挂 `SpringDay1BedInteractable` / `CraftingStationInteractable`
   - 所以它们本质是“编辑器便利与内容修复器”，不是运行时核心逻辑
3. `SaveManager` 的 `AssetDatabase` fallback 来自：
   - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - 设计初衷是：编辑器里如果 `PrefabDatabase.asset` 不在 `Resources`，仍能用 `AssetDatabase.FindAssets("t:PrefabDatabase")` 找到数据库并初始化 `DynamicObjectFactory`
   - 因为真实资产在：
     - `Assets/111_Data/Database/PrefabDatabase.asset`
     - 而不在 `Assets/**/Resources`
4. 同时项目内还存在第二条初始化链：
   - `Assets/YYY_Scripts/Service/PersistentManagers.cs`
   - 它通过序列化字段 `prefabDatabase` 再次初始化 `DynamicObjectFactory`
   - 所以禁用 `SaveManager` fallback 不是“把整个动态对象工厂完全禁掉”，而只是切掉了 `SaveManager` 自己那条 editor 兜底路径
5. 包层方面：
   - 我没有禁用 `com.coplaydev.unity-mcp` 的任何 `InitializeOnLoad` 入口
   - 目前只做到了读码、排序和 suspect 升级，还没有动包代码或包开关

**关键纠偏**
- 我前一轮对“能不能临时关掉”做到了受控取证级理解，但还没有做到“上游设计全景审查级”理解；用户这次追问是合理的。
- 现在补完复盘后，当前准确口径应是：
  - Binder 的 Auto Bind：可以短时关闭做 Editor-only 取证，因为它们是内容补挂便利器，关掉不会直接改运行时架构；但不能长期遗留 disabled
  - `SaveManager` 的 editor fallback：只能作为短时测量开关，不能被误当成正式修法，因为它承担的是编辑器兜底职责，而且项目里还有 `PersistentManagers` 这条并行初始化链
  - `unity-mcp` 包层：在没有更深入的设计级审查前，不应直接做“禁用式”试验；因为 `StdioBridgeHost / TransportCommandDispatcher` 的设计初衷就是编辑器启动桥接与命令泵，它们是工具基础设施本体，不是随手可关的小功能

**恢复点 / 下一步**
- 当前已把“临时取证开关”和“真实系统职责”区分清楚；
- 后续若继续这条支线，在动 `unity-mcp` 包层前，必须先补一轮更深的设计级理解与更窄的安全边界评估，不能直接照搬 `SaveManager` 这类本地 editor fallback 的试法。

## 2026-03-25｜四线程最新回执治理裁定：只判完成度，不误把 checkpoint 当总完成

**当前主线目标**
- 用户要求我不要再长篇转发 prompt，而是先正确判断 4 条线程最新回执各自处于：
  - 本轮完成
  - 仅 checkpoint
  - 是否已经到用户落地验收
  - 是否还需要继续下一轮 prompt

**本轮完成**
1. 已对照原文件完成定义，逐条复核：
   - `spring-day1`：`26.03.25-Day1工作台UI与任务体验重收口委托-04.md`
   - 农田：`009-hover遮挡链闭环与live收口.md`
   - 导航：`002-prompt-13.md`
   - `谁是卧底`：`2026-03-25-UnityPlayBusy续查prompt-03.md`
2. 已形成治理裁定：
   - `spring-day1`：仍只是 checkpoint，不可交用户终验；因为 `委托-04` 要求的是完整 Play 自验或正式验收包，它当前只拿到首段 live 证据，后半链路未跑穿，也未交验收指南/回执单。
   - 农田：本轮完成；`009` 的 hover 单点已闭环，且线程明确给出 `1.0.4` 四组 live 现已全绿，不需要继续发 prompt。
   - 导航：`002-prompt-13` 本轮完成；当前完成的是 `detour clear / recover` 节制簇“三场同轮 fresh 无回归”，但这不等于导航 `S0-S6` 全完成。
   - `谁是卧底`：`prompt-03` 本轮完成；已用 3 组样本把两个 Binder 排除为这一刀真核心，但 Unity Play Busy 整体问题仍未解决。

**关键决策**
- 这轮不新增 prompt 文件，除非用户明确要求继续推进下一刀。
- 当前没有哪条线程已经正式进入“可直接交用户终验包”的状态：
  - `spring-day1` 还没到，因为线程自己都没完成 `委托-04` 要求的完整自验与验收包。
  - 农田虽然可以让用户选做 spot-check，但按 `009` 完成定义已可收口，不需要我继续催线程。

**恢复点 / 下一步**
- 如果用户下一步只想做验收：
  - 优先看农田是否需要做个人体验抽查；
  - `spring-day1` 先不该交给用户终验，应先让线程把 `WorkbenchFlashback / FarmingTutorial` 自验跑穿并交正式验收包。
- 如果用户下一步要继续压线程：
  - `spring-day1` 需要新一轮文件版 prompt；
  - `谁是卧底` 若继续，应进入非 Binder 初始化链的新 prompt；
  - 导航是否继续，要先由用户决定是否现在就开下一责任簇。

## 2026-03-25｜治理纠偏：导航这类强逻辑线，不能再把 synthetic probe 绿灯当成真实落地

**当前主线目标**
- 用户在真实手测里仍看到“玩家推着 NPC 走”，要求我停下来复盘：为什么导航线程说三场同轮 fresh 全绿，但实际体验仍然失败。

**本轮完成**
1. 已回读导航工作区主表、`002-prompt-10~13`、以及 runner / 输入 / 运动控制相关代码。
2. 已确认前一轮治理裁定需要纠偏：
   - 结构 checkpoint 仍成立；
   - 但不能再把它外推成“导航现在不需要继续验收”。

**关键决策**
- 对导航这类“强逻辑 + 强体验耦合”的线，后续验收至少拆三层：
  1. 结构 owner 迁移
  2. synthetic probe 无回归
  3. 真实入口驱动的 in-scene 行为验收
- 只有前两层都过，不代表第三层自动过。
- 对导航线程，后续默认由治理线程继续压验收，不再先交给用户背第一轮逻辑验收。

**新增稳定事实**
- 当前 `NavigationLiveValidationRunner` 的 probe 主要是：
  - 摆位
  - 直接 `SetDestination(...)`
  - 直接 `DebugMoveTo(...)`
  它没有完整覆盖 `GameInputManager` 的真实点击入口。
- 当前 probe 的 pass 条件也偏宽，核心仍是：
  - 到达
  - `minClearance > -0.08f`
  并未把“持续推挤 / 顶着走 / 体感违和”作为硬失败。
- 导航主表中 `S6` 的“最终运动语义显式收口”仍是未完成项，不能再被当成旁支问题切走。

**恢复点 / 下一步**
- 导航线程如果继续，不该再只交“结构迁移 + 三场 probe 绿灯”；
- 下一轮应直接升级为：
  - 真实入口链路 probe
  - push / overlap / displacement 更硬指标
  - `S6` 最终运动语义收口审查

## 2026-03-25｜三份新 prompt 已实际落盘：导航、`谁是卧底`、NPC

**当前主线目标**
- 用户在要求我先阅读并反思锐评后，重新回到治理主线，明确指出：
  - 前面少了真正落地的回复 prompt
  - 这轮必须给出 3 份文件版 prompt
  - 不能只看线程自述，要由我作为总闸重新裁定后再写

**本轮完成**
1. 已新增导航新 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-14.md`
   - 新唯一主刀固定为：
     - 真实点击入口下，把“玩家推着 NPC 走”压成硬验收，只围绕最终运动语义收一刀
2. 已新增 `谁是卧底` 新 prompt：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\2026-03-25-UnityPlayBusy续查prompt-04.md`
   - 新唯一主刀固定为：
     - 双线并查，但目标只有一个：继续收窄出单一第一责任点
3. 已新增 NPC 新开工 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\2026-03-25_NPC导航接入契约与联调验收规范起稿.md`
   - 新唯一主刀固定为：
     - 先做 `NPC-导航接入契约与联调验收规范`

**关键裁定**
1. 导航这轮不准再拿：
   - 结构 owner 迁移
   - synthetic probe 绿灯
   冒充真实动态导航落地。
2. `谁是卧底` 这轮不准再围绕两个 Binder 打转，也不准只盯一层 `InitializeOnLoad`；必须同时面对：
   - 非 Binder 初始化链
   - `SaveManager` 这类 editor fallback runtime 重活
3. NPC 这轮不准误入导航代码主战场，只允许做 docs-only 的接入契约与验收规范。

**恢复点 / 下一步**
- 当前三份 prompt 都已具备可直接转发状态；
- 聊天层后续只应给路径与极短转发语，不再把详细正文贴回聊天。

## 2026-03-25｜治理总闸再纠偏：已新增导航 `002-prompt-15`、农田 `010`、`谁是卧底 prompt-05`

**当前主线目标**
- 用户在最新现场验收里同时否定了导航与农田的前一轮口径，并要求我不要沿用已经过时的 prompt，而是重新以总闸身份裁定后再落文件。

**本轮完成**
1. 已新增导航新 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-15.md`
2. 已新增农田新 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\010-放置链事故回退与全局自治重建.md`
3. 已新增 `谁是卧底` 新 prompt：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\2026-03-25-UnityPlayBusy续查prompt-05.md`

**关键裁定**
1. 导航：
   - 当前不再是“真实入口体验续收口”，而是“动态导航体感已比旧基线更差的回归事故处理”；
   - 线程被明确允许在导航线内自主选择 `selective rollback / selective restore / forward fix`，但必须先恢复到至少不比旧基线更差。
2. 农田：
   - 当前不再是 `hover-only` 或“4 组 live 全绿即可总完成”；
   - 主线被正式改判为“整条放置链事故回退与自治重建”。
3. `谁是卧底`：
   - 当前不再允许直接对 `unity-mcp` suspect 做 disable 类试验；
   - 下一轮唯一主刀固定为包层启动链的设计级 / 风险级审查。

**恢复点 / 下一步**
- 当前这轮治理产物已经全部落成文件，不再依赖聊天长文；
- 后续对线程的聊天分发仍继续只给：
  - 文件路径
  - 极短转发语
- 不再把旧 prompt 或已经失效的口头结论继续外推。

## 2026-03-25｜典狱长模式已正式落入规则源、治理规范与 skill 层

**当前主线目标**
- 用户要求把“审回执、判停发、按需分发”的治理总闸正式命名为“典狱长模式”，并强调这次不能只写进记忆，必须真正烙进规章和 skills。

**本轮完成**
1. 已新增治理规范正文：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\典狱长模式_治理总闸与分发规范.md`
2. 已把典狱长模式同步进 live 规则源：
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前规范快照_2026-03-22.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\治理线程批次分发与回执规范.md`
3. 已新增 project skill：
   - `C:\Users\aTo\.codex\skills\sunset-warden-mode\SKILL.md`
   - `C:\Users\aTo\.codex\skills\sunset-warden-mode\agents\openai.yaml`
4. 已同步改造相关治理 skill：
   - `C:\Users\aTo\.codex\skills\sunset-governance-dispatch-protocol\SKILL.md`
   - `C:\Users\aTo\.codex\skills\sunset-prompt-slice-guard\SKILL.md`
   - `C:\Users\aTo\.codex\skills\skills-governor\SKILL.md`

**关键决策**
1. 典狱长模式的第一动作固定为：
   - 先审回执
   - 再把线程判入四类：
     - `继续发 prompt`
     - `停给用户验收`
     - `停给用户分析 / 审核`
     - `无需继续发`
2. 只有第一类线程才继续写新的 prompt 文件；其余三类必须停下并把原因交代清楚。
3. 给用户的转发格式正式固定为：
   - `对应文件在：`
   - 文件路径列表
   - `你可以直接这样发给 XXX：`
   - 一个直接包含 `请先完整读取 [最新 prompt 路径]` 的 `text` 代码块
4. 旧的“路径 + 极短短句”格式已被正式废止，不再作为治理默认口径。

**验证与阻塞**
- 已通过全文检索确认：
  - `典狱长模式`
  - `对应文件在：`
  - `sunset-warden-mode`
  已同时出现在全局规则、Sunset 规则、治理规范、新 skill 与全局治理库存中。
- 新 skill 的 `openai.yaml` 已手工补齐。
- `skill-creator` 自带脚本验证存在环境阻塞：
  - `generate_openai_yaml.py` / `quick_validate.py` 依赖本机缺失 `yaml` 模块，无法完成官方脚本式校验；
  - 因此本轮改用手工补齐 `agents/openai.yaml` + 规则关键字全文检索做替代验证。

**恢复点 / 下一步**
- 后续用户只要说出 `典狱长`、`典狱长上货`、`上货`，治理线程就应默认进入这套先判后发的流程；
- 当前下一步只剩把这轮结果同步到线程记忆、全局审计层，并给用户列出现行规范与 skills 清单。

## 2026-03-26｜V1 交接文档统一写作 prompt 已落入 `.codex/threads` 根层

**当前主线目标**
- 用户要求为 Sunset 多个即将进入 `V2` 阶段的线程建立一套统一的“重型交接仪式”写作 prompt，不是快速摘要，而是面向未来接班线程的彻底就位式 handoff。

**本轮完成**
1. 已在 Sunset 项目 `.codex/threads` 根层新增统一写作 prompt：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\2026-03-26_V1交接文档统一写作Prompt.md`
2. 已在 Sunset 项目 `.codex/threads` 根层新增极简导入 prompt：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\2026-03-26_V1交接文档导入Prompt.md`
3. 已创建交接产物统一落点目录：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\V1交接文档\`

**关键裁定**
1. 交接文档的正式落点固定为：
   - `.codex/threads/V1交接文档/`
   而不是 `.kiro/`
2. 当前线程自己的 `memory_0.md` 功能不变：
   - 仍然是线程记忆
   - 只是要求未来接班人把 `memory` 也一并阅读和消化
3. 未来继任线程的物理命名规则，按用户新规统一固定为：
   - `当前线程名 + V2`
4. 统一写作 prompt 已把交接包最少文件数固定为 7 份，并把以下内容列为强制项：
   - 线程身份与职责
   - 主线 / 支线迁移编年
   - 关键节点、分叉路与判废记录
   - 用户习惯、长期偏好与协作禁忌
   - 当前高权重事项、风险与未竟问题
   - 证据索引与接手建议
5. 极简导入 prompt 的职责是：
   - 只负责把线程引到统一写作 prompt
   - 不在聊天层重复整套正文
   - 保持治理分发时的复制友好与统一口径

**恢复点 / 下一步**
- 后续凡是需要进入 `V1 -> V2` 交接阶段的 Sunset 线程，治理层都可以直接分发：
  - 根层统一写作 prompt
  - 根层极简导入 prompt
- 线程执行时，按统一 prompt 将交接包写入：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\V1交接文档\<当前线程名>\`
- 下一步如果用户开始点名具体线程进入交接，治理层就应改做：
  - 逐线程发导入 prompt
  - 回收其交接包路径
  - 检查是否命中了统一完成定义

## 2026-03-26｜V1/V2 交接路径口径改判：统一 prompt 留根层，交接正文改回线程工作区内 `V2交接文档`

**当前主线目标**
- 用户明确纠正刚刚落盘的交接规范路径：统一写作 prompt 可以继续放在 `D:\Unity\Unity_learning\Sunset\.codex\threads\` 根层，但真正的交接正文不应集中写到根层共享目录，而应落在各自线程工作区下面。

**本轮完成**
1. 已修正统一写作 prompt：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\2026-03-26_V1交接文档统一写作Prompt.md`
2. 已修正极简导入 prompt：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\2026-03-26_V1交接文档导入Prompt.md`
3. 已把两份 prompt 的正式落点统一改成：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\<分组>\<当前线程名>\V2交接文档\`
4. 已在 prompt 中补明：根层文件只负责“统一规范入口”，不再承担交接正文目录语义。

**关键裁定**
1. `D:\Unity\Unity_learning\Sunset\.codex\threads\` 根层现在只保留：
   - 统一写作 prompt
   - 极简导入 prompt
   - 其他治理入口文件
2. 每条线程真正的交接正文，固定写到当前线程工作区内：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\<分组>\<当前线程名>\V2交接文档\`
3. 用户给出的示例路径：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V2\V2交接文档\`
   已作为这轮正式口径写回 prompt。
4. 旧的根层共享目录口径：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\V1交接文档\`
   本轮明确改判为错误口径残留；当前没有删除物理目录，但后续不再把它当成交接正文落点继续分发。
5. 未来继任线程命名规则不变，仍按用户新规保留为：
   - `当前线程名 + V2`

**恢复点 / 下一步**
- 当前这条治理支线已经从“交接内容模板搭建”推进到“路径语义纠偏完成”。
- 后续如果用户开始点名具体线程进入交接，治理层应直接分发根层导入 prompt，并要求各线程把交接包写回各自工作区下的：
  - `V2交接文档`
- 这轮无需再重写交接内容结构，也无需删除旧目录；下一步只需按新口径执行具体线程交接。

## 2026-03-26｜交接 Prompt 最终版已按 Gemini 锐评完成二次收口

**当前主线目标**
- 继续完善 Sunset 的线程重型交接机制，把刚落盘的统一交接 prompt 收成可直接发放的最终版。

**本轮完成**
1. 已审核 Gemini 对两份交接 prompt 的锐评，并完成事实核查。
2. 已再次回读 `谁是卧底` 线程记忆，确认它这轮实际完成的是：
   - 两份根层 prompt 落盘
   - 交接正文路径从根层共享目录纠偏回线程工作区内 `V2交接文档`
3. 已进一步修改：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\2026-03-26_V1交接文档统一写作Prompt.md`
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\2026-03-26_V1交接文档导入Prompt.md`

**关键裁定**
1. Gemini 锐评整体走 `路径 B`：
   - 问题基本成立；
   - 但修法要按 Sunset 本地治理口径收紧，不原样照抄。
2. 这轮实际采纳的 3 点：
   - 补“单轮写不完时的分批续写 / 防截断机制”
   - 把继任线程命名从 `当前线程名 + V2` 升级为“无版本号则补 `V2`，有 `V数字` 则数字递增”
   - 把 `memory_0.md` 的模糊“回写”改成明确的“追加记录至”
3. 这轮未原样采纳的 1 点：
   - 不把“让用户手动去资源管理器删旧目录”当成最终方案
   - 先由治理线程核残留身份，再决定是否清理
4. 当前已钉死的新规则：
   - `导航检查 -> 导航检查V2`
   - `谁是卧底 -> 谁是卧底V2`
   - `农田交互修复V2 -> 农田交互修复V3`
   - 明确禁止 `V2V2 / V3V2`
5. 防截断机制已写进最终版 prompt：
   - 若一轮写不完 7 份交接文档，必须分批落盘
   - 每次停下前先把已完成文件和剩余文件追加记进 `memory_0.md`
   - 聊天里明确提示用户回复“继续”
6. 现场残留复核结果：
   - 根层 `D:\Unity\Unity_learning\Sunset\.codex\threads\V1交接文档\` 实际并不存在
   - 当前只发现 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V2\V1交接文档\` 这一处空目录残留
   - 它属于旧口径残留，不再作为后续分发依据

**恢复点 / 下一步**
- 这套交接 prompt 现在已经完成二次审校，可视为当前可直接发放的最终版。
- 后续如果用户开始点名具体线程进入代际交接，治理层默认直接使用这两份最终版 prompt，不再回退到第一版口径。

## 2026-03-26｜第一波交接分发已细化为：`谁是卧底` 直接进，`NPC / spring-day1` 先做交接前确认

**当前主线目标**
- 用户已接受交接 prompt 最终版，下一步不再停留在治理讨论，而是开始第一波实际分发。

**本轮完成**
1. 已新增 `谁是卧底` 的正式交接委托：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\2026-03-26_谁是卧底进入下一代重型交接委托-01.md`
2. 已新增 `NPC` 的交接前状态确认委托：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPC进入下一代交接前状态确认委托-01.md`
3. 已新增 `spring-day1` 的交接前状态确认委托：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\26.03.26-Day1进入下一代交接前状态确认委托-08.md`

**关键裁定**
1. `谁是卧底`：
   - 当前适合直接进入代际交接；
   - 原因是它这条线当前最稳定，且目标是把诊断链、证据链、prompt 链整体交给新线程，不依赖当前再补业务代码。
2. `NPC`：
   - 当前不直接替它判“能交接”或“不能交接”；
   - 改为先发“交接前状态确认委托”，要求它自己基于最新 live 现场判断：
     - 若可进入，就立即进入交接；
     - 若不可进入，就交清晰 blocker 回执。
3. `spring-day1`：
   - 按最新纠偏回执，治理层当前初判它大概率已经满足进入交接条件；
   - 但仍保留线程自己最终确认权，因此先发“交接前状态确认委托”，并明确：
     - 若可进入，就直接进入交接；
     - 若不可进入，就只回最小 blocker。

**恢复点 / 下一步**
- 当前第一波交接分发已经具备可直接转发的 3 份文件。
- 后续若用户现在就开始发放：
  - `谁是卧底` 直接按正式交接委托进入 handoff；
  - `NPC / spring-day1` 先按确认委托回一轮，再决定是否立即进入交接。

## 2026-03-26｜第二波交接分发已补齐：`导航 / 农田 / 项目文档总览` 先做状态确认，不抢先替它们判定 handoff

**当前主线目标**
- 用户继续推进“老线程交给新线程”的治理主线，这一轮要求我补齐：
  - `导航检查`
  - `农田交互修复V2`
  - `项目文档总览`
  这 3 条线的可转发文件。

**本轮完成**
1. 已回读并对齐：
   - `导航检查` 最新线程记忆与工作区记忆
   - `农田交互修复V2` 最新线程记忆与 `1.0.4` 工作区记忆
   - `项目文档总览` 最新线程记忆与其“归档辅助线”身份判断
2. 已新增 `导航` 交接前状态确认委托：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航进入下一代交接前状态确认委托-01.md`
3. 已新增 `农田` 交接前状态确认委托：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\2026-03-26-农田进入下一代交接前状态确认委托-01.md`
4. 已新增 `项目文档总览` 交接前状态确认委托：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\项目文档总览\2026-03-26_项目文档总览进入下一代交接前状态确认委托-01.md`

**关键裁定**
1. `导航检查` 当前不直接替它判“进交接”或“继续施工”：
   - 因为它虽然已经把责任点收窄到执行层 `detour owner` 不落地，
   - 但仍需线程自己确认：这个叙事是否已经稳定到足以无失真交给 `导航检查V2`。
2. `农田交互修复V2` 当前也不直接替它判“立即交接”：
   - 因为它已经把 placeable 主链拉回至少不比 `124caccc` 更差，
   - 但仍需线程自己确认：当前 handoff 口径是否应固定为“主链已恢复 + sapling runner 稳定性剩余 + 新增 6 条需求并入总入口”。
3. `项目文档总览` 当前更不能机械替它造一个 `V2`：
   - 因为这条线的最新身份更像“归档终态 + 历史辅助线”，
   - 所以本轮要求它先回答“是否真的需要继任线程”，再决定要不要交接。

**恢复点 / 下一步**
- 当前第二波交接分发文件已经补齐。
- 下一步如果用户开始发放：
  - `导航 / 农田` 先回一轮“是否可进入交接”的确认回执；
  - `项目文档总览` 先回一轮“是否真的需要继任线程”的确认回执；
  - 再基于它们各自的回答决定是否进入正式 handoff。

## 2026-03-26｜第一波交接回执已验收：`谁是卧底 / spring-day1 / NPC` 均可从当前批次收件

**当前主线目标**
- 用户开始回传第一波交接确认结果；
- 这轮目标不是再发新 prompt，而是验收这些回执是否真的命中“交接包已完整落盘”的完成定义。

**本轮完成**
1. 已核验 `谁是卧底`：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\V2交接文档\` 已存在且 7 份文件齐全；
   - `memory_0.md` 已追加交接目录、继任线程名与推荐阅读顺序；
   - 当前可判定该线程本轮交接完成，后续归 `无需继续发`。
2. 已核验 `spring-day1`：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\V2交接文档\` 已存在且 7 份文件齐全；
   - `memory_0.md`、父工作区 `memory.md` 与子工作区 `003-进一步搭建\memory.md` 均有新补记；
   - 当前可判定该线程本轮交接完成，后续归 `无需继续发`。
3. 已核验 `NPC`：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\V2交接文档\` 已存在且 7 份文件齐全；
   - `memory_0.md` 已明确写入“当前满足进入下一代交接条件”与 blocker 边界；
   - 当前可判定该线程本轮交接完成，后续归 `无需继续发`。

**关键裁定**
1. `谁是卧底`：
   - 收件通过；
   - 当前状态转为“已完成交接，等待未来 `谁是卧底V2` 视需要唤醒”。
2. `spring-day1`：
   - 收件通过；
   - 但仍保留一个审计提醒：
     - `003-进一步搭建\memory.md` 历史编码污染并未根治；
     - 后续真正接班时，主读入口仍应优先 `V2交接文档 + 线程 memory + 父工作区 memory`，不要回退依赖那段旧污染内容。
3. `NPC`：
   - 收件通过；
   - 当前 blocker 口径稳定保留为：
     - `Primary.unity` dirty 且无独占写归属；
   - 这已被正常写入交接，而不是继续留给 V1 扩刀。

**恢复点 / 下一步**
- 当前第一波 3 条线都已经可以从本轮治理清单里划出，不再继续发 prompt。
- 现在真正还在等待回执的是：
  - `导航检查`
  - `农田交互修复V2`
  - `项目文档总览`

## 2026-03-26｜已补第一波 3 条 V2 线程的首轮启动委托：`谁是卧底V2 / spring-day1V2 / NPCV2`

**当前主线目标**
- 用户指出：交接包收下之后，还需要给新线程一份真正能开工的“首轮开场 prompt”；
- 这轮目标不是再做 handoff，而是给已收件的 3 条 V2 线程各补一份首轮启动委托。

**本轮完成**
1. 已新增 `谁是卧底V2` 首轮启动委托：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\2026-03-26_谁是卧底V2首轮启动委托-02.md`
2. 已新增 `spring-day1V2` 首轮启动委托：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\26.03.26-Day1V2首轮启动委托-09.md`
3. 已新增 `NPCV2` 首轮启动委托：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2首轮启动委托-02.md`

**关键裁定**
1. `谁是卧底V2`：
   - 首轮唯一主刀固定为：
     - `TransportCommandDispatcher` 的下一刀开工前复核与 default-off diagnostics 最小实施切口定界
   - 明确禁止回头扩到 `lag_monitor / popup / Binder / SaveManager / Sunset` 业务现场。
2. `spring-day1V2`：
   - 首轮唯一主刀固定为：
     - `live preflight + 非热正式面首刀裁定`
   - 先重新钉实 `ee318757` 基线与 `Primary.unity` mixed blocker，再决定 V2 第一实现切口。
3. `NPCV2`：
   - 首轮唯一主刀固定为：
     - `Primary.unity` 安全写窗口只读复核 + `HomeAnchor` 最小 scene 切片准入裁定
   - 不再让它回头微调气泡或抢导航核心。

**恢复点 / 下一步**
- 当前第一波 3 条 V2 线程都已经有可直接转发的首轮启动 prompt。
- 后续如果 `导航 / 农田 / 项目文档总览` 也完成 handoff 或确认结果，按同样模式继续补它们的 V2 首轮启动 prompt。

## 2026-03-26｜第二波回执已验收：`导航 / 农田` 进入 handoff，`项目文档总览` 正式停发；并补 `导航检查V2 / 农田交互修复V3` 首轮启动委托

**当前主线目标**
- 用户继续回传第二波交接确认结果；
- 这轮目标是先收件、再决定是否继续发 V2 首轮启动 prompt。

**本轮完成**
1. 已核验 `导航检查`：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\V2交接文档\` 已存在且 7 份文件齐全；
   - 线程 `memory_0.md` 与导航工作区 `memory.md` 已追加；
   - 当前可判定该线程 handoff 完成。
2. 已核验 `农田交互修复V2`：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V2\V2交接文档\` 已存在且 7 份文件齐全；
   - 线程记忆、子工作区、父层、根层 `memory.md` 均已追加；
   - 当前可判定该线程 handoff 完成。
3. 已核验 `项目文档总览`：
   - 当前明确回执为 `no`；
   - 理由成立：它已是“归档终态 + 历史辅助线”，不存在需要继任线程继续承接的高权重事项；
   - 当前正式裁定为 `无需继续发`，不生成 `项目文档总览V2`。
4. 已新增 `导航检查V2` 首轮启动委托：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2首轮启动委托-02.md`
5. 已新增 `农田交互修复V3` 首轮启动委托：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\2026-03-26-农田交互修复V3首轮启动委托-02.md`

**关键裁定**
1. `导航检查V2`：
   - 首轮唯一主刀固定为：
     - detour owner 保活最小闭环
   - 只允许锁 `PlayerAutoNavigator / NPCAutoRoamController / NavigationPathExecutor2D`，不再回漂 solver、仲裁器或场景。
2. `农田交互修复V3`：
   - 首轮唯一主刀固定为：
     - `013` 的 sapling-only runner 稳定性收尾
   - 先把验证层尾差收掉，再进入新增 6 条需求。
3. `项目文档总览`：
   - 当前正式停发；
   - 未来若重启，直接按现有 `README + memory + 最终总结与验收报告` 作为最小阅读入口，不生成 V2 启动 prompt。

**恢复点 / 下一步**
- 当前 6 条线程裁定已全部落地：
  - `谁是卧底 / spring-day1 / NPC / 导航 / 农田` 已收件并完成对应新线程首轮 prompt 分发
  - `项目文档总览` 正式归入“无需继续发”
- 下一步只需等待这些新线程的首轮回执。

## 2026-03-26｜已为当前治理线程自身补交接前状态确认入口

**当前主线目标**
- 用户进一步要求：不仅要给其他线程发 handoff / V2 启动 prompt，也要给当前治理线程自己补一份“交接前状态确认委托”。

**本轮完成**
1. 已新增：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\对话丢失修复\2026-03-26_治理线程进入下一代交接前状态确认委托-01.md`

**关键裁定**
1. 当前治理线程的初判仍是：
   - 还不适合立即 handoff；
   - 因为它当前仍承担：
     - 新线程首轮回执收件
     - 典狱长四类裁定
     - 视情况回拉后续 prompt
2. 但后续如果真要把治理线程本身交给下一代：
   - 现在已经有了固定确认入口；
   - 不必再临时拼 prompt。

**恢复点 / 下一步**
- 这份自用确认委托当前只作为后续治理 handoff 的入口储备；
- 眼下这条治理线程仍保持活跃，继续等待新线程首轮回执。

## 2026-03-26｜`导航检查` 最小回执已正式收件，V1 维持停发

**当前主线目标**
- 用户贴回 `导航检查` 的最小聊天回执；
- 本轮只做治理收件确认，不把已经完成 handoff 的旧线程机械转成下一刀施工。

**本轮完成**
1. 复核用户回执与现有交接事实一致：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\V2交接文档\` 已回写；
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\memory_0.md` 已追加；
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md` 已追加。
2. 复核线程一句话摘要与现行裁定一致：
   - 当前导航线稳定叙事仍是：
     - `ShouldRepath` 已出现；
     - detour owner 没有稳定接管执行层。
3. 维持治理裁定不变：
   - `导航检查` V1 继续归类为 `无需继续发`；
   - 不再给旧线程补新的施工 prompt。

**恢复点 / 下一步**
- 这条回执只起到正式收件确认作用，不改变既有分发结果。
- 治理线程后续只等待 `导航检查V2` 的首轮回执；
- 当前真正活跃的施工入口仍是：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2首轮启动委托-02.md`

## 2026-03-26｜`典狱长-V2` 进入前状态确认结果：5 条新线程首轮证据已齐，但当前治理线程仍不能 handoff

**当前主线目标**
- 用户要求先不要机械 handoff，而是只判断这条治理线程现在能不能安全交给 `典狱长-V2`。

**本轮完成**
1. 已按 `2026-03-26_典狱长-V2进入前状态确认委托-02.md` 完整回读：
   - 当前治理线程 `memory_0.md`
   - `Codex规则落地/memory.md`
   - `skill-trigger-log.md`
   - 统一交接写作 prompt / 导入 prompt
   - 5 份新线程首轮启动委托
2. 已直接核对 5 条新线程自己的 `memory_0.md`，确认首轮结果都已真实落盘：
   - `谁是卧底V2`：已把下一刀收紧到 `TransportCommandDispatcher` 的 `default-off diagnostics`
   - `spring-day1V2`：已完成 live preflight，第一刀仍固定为 non-hot 正式面
   - `NPCV2`：已确认 `Primary.unity` scene 写窗口仍不成立
   - `导航检查V2`：已把 detour owner 最小保活闭环接上，并拿到 1 组有效执行窗口
   - `农田交互修复V3`：已拿到至少 1 轮无 `NavValidation` 并发的 sapling-only 窗口，但 patched runner 仍未在新的干净窗口里稳定转绿
3. 已确认“5 条新线程首轮回执是否收齐”这一项现在可以回答为：
   - `yes`

**关键裁定**
1. 当前这条治理线程仍不能 handoff。
2. 单一第一阻塞点不是“还有线程没回执”，而是：
   - 这 5 条首轮结果还没有被当前治理线程统一收件成正式四类裁定，也还没完成必要的第二轮 `prompt` 回拉 / 停发。
3. 因此现在如果直接交给 `典狱长-V2`，会把本波治理闭环切断，导致新线程后续去向悬空。

**恢复点 / 下一步**
- 下一步不是立刻生成 `典狱长-V2` 交接包；
- 而是先把本波 5 条新线程首轮结果做完统一裁定闭环，再重新判断是否到 handoff 停点。

## 2026-03-26｜5 条新线程首轮回执已全部收齐，治理线程已拿到第二轮裁定输入

**当前主线目标**
- 用户追问“齐了没有”，因此这轮只核“5 条新线程首轮回执是否全部齐全且可信”，不抢先写第二轮 prompt。

**本轮完成**
1. 已逐条核对以下 5 条新线程的首轮回执与对应记忆：
   - `谁是卧底V2`
   - `spring-day1V2`
   - `NPCV2`
   - `农田交互修复V3`
   - `导航检查V2`
2. 已确认它们的最小回执与线程 `memory_0.md` / 工作区 `memory.md` / `skill-trigger-log.md` 一致，不存在“聊天说有、文件没写”的假收件。
3. 已确认导航的 `519d51bd` 真实存在，导航首轮“已白名单 sync 到 main”的说法成立。

**关键裁定**
1. 当前 5 条首轮回执现在可以正式判为：
   - `已齐`
2. 当前 5 条线的四类裁定预判为：
   - `谁是卧底V2`：继续发 prompt
   - `spring-day1V2`：继续发 prompt
   - `NPCV2`：停给用户分析 / 审核（hot-file blocker 未解）
   - `农田交互修复V3`：继续发 prompt
   - `导航检查V2`：继续发 prompt
3. 因此，上一个“治理线程还不能 handoff”的单一阻塞点已经被解除到下一阶段：
   - 现在不再是“等回执”
   - 而是“是否要继续由本代治理线程发第二轮 prompt，还是已到 `典狱长-V2` 的 handoff 停点”

**恢复点 / 下一步**
- 当前治理线程已经拿到完整第二轮裁定输入；
- 如果用户要继续典狱长分发，下一步就按上面 5 条预判进入第二轮 prompt / 停发裁定；
- 如果用户要直接评估 handoff，现在也已经比上一轮更接近可交接停点。

## 2026-03-26｜第二轮典狱长分发已落稿：`谁是卧底V2 / spring-day1V2 / 导航检查V2 / 农田交互修复V3` 继续，`NPCV2` 停发

**当前主线目标**
- 用户明确要求“开始吧”，因此这轮不再只做审计，而是正式进入第二轮典狱长 prompt 分发。

**本轮完成**
1. 已按 `sunset-prompt-slice-guard` 重新压缩 4 条继续施工线程的第二轮硬切片：
   - `谁是卧底V2`：`TransportCommandDispatcher` 初始化链 `default-off diagnostics` 落地
   - `spring-day1V2`：两个 Day1 UI prefab 的 formal-face 基线审面与单刀对齐
   - `导航检查V2`：同一 detour owner 闭环上的 1 组 NPC 侧 fresh
   - `农田交互修复V3`：新的干净 sapling-only 窗口下的 patched runner 转绿验证
2. 已新增对应 prompt 文件：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\2026-03-26_谁是卧底V2第二轮续工委托-03.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\26.03.26-Day1V2第二轮续工委托-10.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2第二轮续工委托-03.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\2026-03-26-农田交互修复V3第二轮续工委托-03.md`
3. 已明确维持 `NPCV2` 停发，不生成第二轮施工 prompt。

**关键裁定**
1. 当前 second-wave 的治理结果是：
   - `继续发 prompt`：`谁是卧底V2 / spring-day1V2 / 导航检查V2 / 农田交互修复V3`
   - `停给用户分析 / 审核`：`NPCV2`
2. `spring-day1V2` 这轮被进一步压成 prefab formal-face 切口，不再允许继续停在“non-hot 正式面”这类宽泛口号里。
3. `导航检查V2` 这轮被限制为 NPC 侧 1 组 fresh，不允许把首轮 player 侧通过冒充成整条导航已过线。
4. `农田交互修复V3` 这轮明确区分“干净窗口失败”和“并发污染窗口”，不允许再把两者混讲。

**恢复点 / 下一步**
- 当前 4 份第二轮 prompt 已可直接转发；
- 后续按回执再判断是否继续第三轮，或是否已接近 `典狱长-V2` handoff 停点。

## 2026-03-26｜用户裁定“后续治理属于 `典狱长-V2`”，当前治理线程已正式转入自身 handoff

**当前主线目标**
- 用户明确纠偏：当前这条治理线程不该再继续向下把控，后面的治理属于 `典狱长-V2`；
- 因此这轮不再继续做典狱长分发，而是切到当前线程自己的重型交接。

**本轮完成**
1. 已回读：
   - 当前线程 `memory_0.md`
   - `Codex规则落地/memory.md`
   - `2026-03-26_V1交接文档统一写作Prompt.md`
   - `2026-03-26_典狱长-V2进入前状态确认委托-02.md`
   - `skill-trigger-log.md`
2. 已在：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\对话丢失修复\V2交接文档\`
   下正式生成 7 份交接文件。
3. 已把未来继任线程名固定写为：
   - `典狱长-V2`

**关键裁定**
1. 当前治理线程的最新停点已经成立：
   - second-wave prompt 已落稿
   - 继续治理从这里开始属于 `典狱长-V2`
2. 因此旧结论“还不能 handoff”被用户最新裁定覆盖，不再继续沿用。

**恢复点 / 下一步**
- 当前 `V1` 到此收口；
- 后续典狱长治理默认由 `典狱长-V2` 接班，不再继续由本线程往下做第三轮分发。

## 2026-03-26｜已补 `典狱长-V2` 首轮启动委托，当前 handoff 交付物已完整

**当前主线目标**
- 用户补充要求：仅有重型交接包还不够，必须再给 `典狱长-V2` 一份真正能开工的“开场白”。

**本轮完成**
1. 已新增：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\对话丢失修复\2026-03-26_典狱长-V2首轮启动委托-03.md`
2. 已把新线程首轮动作压成：
   - 接班后的 current-wave 基线复核与 inbox 建立
3. 已明确禁止新线程在首轮就：
   - 重写 second-wave prompt
   - 抢先写 third-wave prompt
   - 继续争论 `V1` 是否应该留场

**关键裁定**
1. 当前 handoff 交付物现在才算完整：
   - 7 份重型交接文档
   - 1 份 `典狱长-V2` 首轮启动委托
2. 这份启动委托的作用不是继续治理，而是让 `V2` 能从稳定现场直接接班，不会开局尬住。

**恢复点 / 下一步**
- 当前 `V1` 的交接包与开场 prompt 都已具备；
- 后续只需把首轮启动委托直接发给 `典狱长-V2`，即可开始新线程治理。

## 2026-03-26｜`典狱长-V2` 已完成 current-wave 基线复核与 inbox 建立

**当前主线目标**
- 接住 `V1` 已交出的治理现场，只做 current-wave 基线复核与 inbox 建立，不抢写第三轮 prompt。

**本轮完成**
1. 已按 `2026-03-26_典狱长-V2首轮启动委托-03.md` 回读交接包核心、当前治理记忆、`STL-20260326-018/019/020` 与 4 份 second-wave prompt，确认 `V1` handoff 停点与 second-wave 事实一致。
2. 已把当前 5 条线治理图重新钉死为：
   - `谁是卧底V2`：继续施工，当前等待 second-wave 最小回执
   - `spring-day1V2`：继续施工，当前等待 second-wave 最小回执
   - `导航检查V2`：继续施工，当前等待 second-wave 最小回执
   - `农田交互修复V3`：继续施工，当前等待 second-wave 最小回执
   - `NPCV2`：停给用户分析 / 审核，不进入 second-wave 施工收件队列
3. 已把接班后的 current-wave inbox 固定成两层：
   - 施工 inbox：`谁是卧底V2 / spring-day1V2 / 导航检查V2 / 农田交互修复V3` 的 second-wave 最小回执
   - 分析位：`NPCV2` 等待用户分析 / 审核结论，而不是等待第三轮施工 prompt
4. 已明确从这一轮开始，后续典狱长治理由 `典狱长-V2` 承接；`V1` 不再继续留场复判、重写 second-wave 或抢先写 third-wave。

**关键裁定**
1. 当前是否已完成接班基线复核：
   - `yes`
2. 当前真正等待的治理输入应拆成两类：
   - 若只看施工收件：当前只等 4 条继续施工线的 second-wave 回执
   - 若看全量治理：另有 `NPCV2` 继续停在用户分析 / 审核等待位
3. 因此当前 inbox 的最准口径不是“继续发 prompt”，而是“先收 4 条施工回执 + 保持 1 条用户分析位”。

**恢复点 / 下一步**
- 下一步只需继续收 `谁是卧底V2 / spring-day1V2 / 导航检查V2 / 农田交互修复V3` 的 second-wave 最小回执；
- `NPCV2` 继续等待用户分析 / 审核结论；
- 在这些输入回来之前，不重写 second-wave，不抢写 third-wave。

## 2026-03-26｜shared-root 大扫除批次已发：6 条线只做 own dirty / untracked 认领与白名单收口

**当前主线目标**
- 用户基于 `NPCV2` 的最新只读阻塞反馈，明确改题：
  - 当前第一任务不是继续业务续工；
  - 而是先发一轮 shared-root cleanup，让所有相关线程各扫各的地、认领 own dirty、能白名单提交就提交、异常就汇报。

**本轮完成**
1. 已复核 cleanup 批次现场仍成立：
   - `D:\Unity\Unity_learning\Sunset @ main @ 519d51bd20d98e662eafb94cea0c5bbbeb314cec`
   - `Primary.unity` 仍为 `M`，且 `git diff --stat -- Assets/000_Scenes/Primary.unity` 仍是 `76 insertions / 4 deletions`
   - `shared-root-branch-occupancy = neutral-main-ready` 仍只表示 Git 入口中性，不等于热 scene 可写
   - `GameInputManager.cs` 仍是 mixed hot-file dirty
   - `TagManager.asset` owner 仍未明确
2. 已在本工作区根层新增本轮批次入口：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-26_批次分发_11_shared-root大扫除认领与白名单收口_01.md`
3. 已为 6 条线分别落 cleanup prompt：
   - `spring-day1V2`
   - `NPCV2`
   - `导航检查V2`
   - `农田交互修复V3`
   - `谁是卧底V2`
   - `项目文档总览`
4. 已把本轮统一口径钉死为：
   - 只认领 own dirty / untracked
   - 能白名单 sync 到 `main` 的直接收口
   - `Primary.unity` / `GameInputManager.cs` / `TagManager.asset` 等 mixed / hot 目标只报实，不乱吞
   - 不准借 cleanup 名义恢复业务续工或扩成 third-wave

**关键裁定**
1. 当前 cleanup 分发不是只为 `NPCV2` 开路，而是一次 shared-root 全线收尾动作。
2. `NPCV2` 的业务态仍保持“停给用户分析 / 审核”，但 cleanup 可以继续只收 own docs tail 与 owner 报实，不构成业务复工。
3. `项目文档总览` 虽然不需要 `V2` 业务继任线程，但当前仍有 own 文档尾账，因此纳入本轮 cleanup。
4. 治理线程自己仍保留一层 docs tail：
   - `Codex规则落地 / Steering规则区优化 / 共享根执行模型与吞吐重构` 的治理文档 dirty
   - 这些未外发给业务线程，后续由 `典狱长-V2` 自己单独收口

**涉及文件**
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-26_批次分发_11_shared-root大扫除认领与白名单收口_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\26.03.26-Day1V2共享根大扫除与白名单收口-11.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2共享根大扫除与owner报实-03.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2共享根大扫除与白名单收口-04.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\2026-03-26-农田交互修复V3共享根大扫除与白名单收口-04.md`
- `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\2026-03-26_谁是卧底V2共享根大扫除与AutoCalibrator收口-04.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\项目文档总览\2026-03-26_项目文档总览共享根大扫除与文档尾账收口-02.md`

**恢复点 / 下一步**
- 下一步不再继续争论 second-wave / third-wave；
- 先等这 6 条线回 cleanup 最小回执；
- 收件后再判断：
  - 哪些已经对自己 clean
  - 哪些已完成白名单 sync
  - 哪些仍有 foreign dirty / mixed hot-file blocker 需要典狱长继续裁定。

## 2026-03-26｜cleanup 批次收件复核：实际 active set 为 5/5 已回执通过，`谁是卧底V2` 不计入本轮

**当前主线目标**
- 用户贴回 5 条 cleanup 回执，要求先核票、再重新把 shared-root 当前盘面钉死；
- 同时用户明确纠偏：
  - `谁是卧底V2` 仍在执行原任务；
  - 并未被实际投入本轮 cleanup 批次执行。

**本轮完成**
1. 已逐条核对 5 条已回执线程的提交与路径 clean 状态：
   - `spring-day1V2`：`3b2c0f1e8b8da53b64e8e6545610d51238cafb31` 存在；其 own 路径当前 clean
   - `NPCV2`：`eb6284fa / ddef0564 / 29e8db17` 存在；其 own 路径当前 clean
   - `导航检查V2`：`12ce08149716c72e4c76c5fac805fcaa7fd315f7 / caa8bde3d706fe1234abc84062130d3e8eab236d` 存在；其 own 路径当前 clean
   - `项目文档总览`：`1452bebb1171235b454d1d4fd961639caabdc930` 存在；其 own 路径当前 clean
   - `农田交互修复V3`：`f5a2bf50 / 533974de` 存在；其 own 路径当前 clean
2. 已复核这 5 条线当前都满足：
   - own dirty / untracked 已认领
   - 已完成白名单 sync 到 `main`
   - 当前 shared root 不 clean 的原因都来自 foreign dirty / hot-file dirty，而不是它们自己的白名单残留
3. 已根据用户纠偏重钉本轮统计口径：
   - 本轮实际 active cleanup set = 5
   - `谁是卧底V2` 不纳入本轮 cleanup 完成率与收件统计
4. 已复核 shared root 剩余 dirty 主要集中在两组：
   - mixed / hot-file：
     - `Assets/000_Scenes/Primary.unity`
     - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
     - `ProjectSettings/TagManager.asset`
     - `Assets/Editor/StaticObjectOrderAutoCalibrator.cs`
   - 治理 / 文档尾账：
     - `Codex规则落地`
     - `Steering规则区优化`
     - `共享根执行模型与吞吐重构`
     - 若干 `V1` 提示文档与 `tmp/`

**关键裁定**
1. 当前 5 条实际参批 cleanup 线程可全部判为：
   - `cleanup 已完成，对自己 clean`
2. 但这不等于 shared root 已 clean：
   - 当前仓库仍被 4 个 unresolved hot / mixed target 与治理 docs tail 占着
3. `NPCV2` 需要拆成双态理解：
   - cleanup 线：已完成
   - 业务线：仍停给用户分析 / 审核
4. `导航检查V2` 与 `农田交互修复V3` 本轮虽然带了最小代码/ warning-cleanup，但仍在 own path 内，且代码闸门 / 编译闸门已通过，可接受为 cleanup 范围内的受控收口。
5. `谁是卧底V2` 当前不应被误报成“cleanup 未回执”：
   - 正确口径是“未进入本轮 cleanup 执行集合”

**恢复点 / 下一步**
- 当前 cleanup 队列已经收完应收的 5/5；
- 下一步不要再继续催这 5 条线补 cleanup；
- 现在真正要处理的是：
  1. `Primary.unity / GameInputManager.cs / TagManager.asset / StaticObjectOrderAutoCalibrator.cs` 的 owner / 去向
  2. 治理线程自己手上的 docs tail
  3. `谁是卧底V2` 继续按用户原任务线推进，不并入本轮 cleanup 统计。

## 2026-03-26｜治理收口已改成硬闸门：own-root 清尾责任下沉到脚本与 live 规则

**当前主线目标**
- 用户明确要求：不能以后每次都等典狱长临时发 cleanup prompt 才收垃圾；
- 本轮要把“线程自己一刀一收、own dirty 不得拖到下一轮”的纪律真正压进 live 脚本与 live 规则。

**本轮完成**
1. 已验证 `scripts/git-safe-sync.ps1` 新增的 `own roots` 硬闸门：
   - 正向样本：`项目文档总览` 只带自己一条路径做 `preflight` 时允许继续；
   - 反向样本：治理线只带 `Codex规则落地` 中一小块 include path，但同根还有剩余 docs tail 时，被脚本直接阻断。
2. 已把本轮新纪律同步到 live 规则层：
   - `AGENTS.md`
   - `Sunset当前规范快照_2026-03-22.md`
   - `治理线程批次分发与回执规范.md`
   - `典狱长模式_治理总闸与分发规范.md`
   - 相关治理 skill 正文
3. 已把当前统一口径钉死为：
   - `git-safe-sync` 不只允许保留 unrelated dirty，也会阻断 `own roots` 下未纳入本轮的 remaining dirty / untracked
   - 回执必须带 `当前 own 路径是否 clean`
   - 只要 `当前 own 路径是否 clean != yes`，回执就不能算闭环，不能直接进入下一轮 feature prompt
   - 治理线程默认不再为常规 own dirty 尾账重复开 cleanup 批次
4. 已把当前强制报实的 unresolved hot / mixed 目标固定为：
   - `Primary.unity`
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
   - `ProjectSettings/TagManager.asset`
   - `Assets/Editor/StaticObjectOrderAutoCalibrator.cs`

**关键裁定**
1. cleanup 这轮的 5/5 完成并不代表 shared root clean，但它证明了“线程对白名单 clean”和“仓库整体 clean”已经能被分开治理。
2. 今后常规尾账不能再外包给典狱长；线程必须在自己的 wave 内把 own dirty / untracked 自己收干净。
3. 典狱长治理只保留给真正的 cross-thread mixed / hot-file incident，不再承担普通线程的默认扫尾。

**恢复点 / 下一步**
- 只要本轮治理 docs tail 白名单 sync 完成，接下来治理焦点就只剩：
  1. 4 个 unresolved hot / mixed 目标的 owner 裁定
  2. 恢复开发时对高危线程的陪跑与防串线检查

## 2026-03-26｜4 个 unresolved hot / mixed 目标已完成收口，shared root 只剩 foreign 文档脏改

**当前主线目标**
- 用户明确要求：在 own-root 硬闸门落地后，不要再停在“还有 4 个 unresolved target”的分析层；
- 如果 target 无法继续完美拆纯，但当前 Unity 现场没有明显异常，就优先按可接受基线提交，先恢复开发吞吐。

**本轮完成**
1. 已按稳定 launcher + `main-only` 白名单口径，对 3 个窄 diff 先做正式 `preflight -> sync`：
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
   - `ProjectSettings/TagManager.asset`
   - `Assets/Editor/StaticObjectOrderAutoCalibrator.cs`
   - 已提交并推送：`70d9c20c`（`2026.03.26_典狱长-V2_02`）
2. 已对 `Primary.unity` 先做一刀最小 scene hygiene：
   - 明确剥掉导航 residue：NPC 的 `showDebugLog / drawDebugPath` scene override
   - 保留其余 Day1 正文与当前玩家 / 相机 integration 位移
3. 已对 `Assets/000_Scenes/Primary.unity` 单独做 `preflight -> sync`：
   - 已提交并推送：`3fc6c3a2`（`2026.03.26_典狱长-V2_03`）
4. 当前 shared root 最新 `HEAD` 已为：
   - `3fc6c3a2e6421722150f063ab6c29d33030163a8`
5. 当前 `git status` 已只剩 1 条 foreign 文档脏改：
   - `.codex/threads/Sunset/项目文档总览/memory_0.md`

**关键裁定**
1. 此前固定报实的 4 个 unresolved hot / mixed 目标，现已全部从 shared root 工作树中清空：
   - `Primary.unity`
   - `GameInputManager.cs`
   - `TagManager.asset`
   - `StaticObjectOrderAutoCalibrator.cs`
2. `Primary.unity` 这次不是继续追求 owner 纯度到 100% 才允许落地，而是在用户明确授权“进度优先、可接受就先提”的前提下，按当前 integration baseline 收口。
3. 这不等于以后这些目标可以自由混写：
   - 只是说明这轮 shared root 的历史阻塞已经被治理线程吃掉；
   - 后续任何线程再碰这 4 个目标，仍要按高危热区陪跑口径处理。
4. 当前 shared root 不 clean 的原因，已不再是 hot / mixed target，而只剩一条明确 foreign 的文档脏改；它不属于本轮治理白名单，不应被误吞。

**恢复点 / 下一步**
- 当前治理焦点已从“先清 4 个 hot / mixed 目标”切到：
  1. 恢复普通开发时，对 `Primary.unity / GameInputManager.cs / TagManager.asset / StaticObjectOrderAutoCalibrator.cs` 的高危陪跑
  2. 继续执行“线程自己一刀一收”的 live 纪律，不再回退到常规 cleanup 批次
  3. 对剩余 foreign 文档脏改保持 owner 隔离，不把它误判成 shared root 仍被热区卡死

## 2026-03-26｜恢复开发与 docs-only 开工批次已落盘，三条线改用“最小回执 + 用户可读详细汇报”双层交付

**当前主线目标**
- 用户已经不再要继续治理分析，而是要直接放行：
  - `NPCV2`
  - `农田交互修复V3`
  - `导航检查V2`
- 同时用户明确要求：
  - 后续回执不能只写给治理线程看
  - 必须补一份用户也能直接读懂、能按功能点验收的详细汇报

**本轮完成**
1. 已在治理根层新建本轮批次入口：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-26_批次分发_12_恢复开发与文档开工_01.md`
2. 已为 `NPCV2` 落下新一轮专属委托：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2恢复开工委托-04.md`
   - 唯一主刀收紧为：`Primary.unity` 中 `001 / 002 / 003` 的 `HomeAnchor` 最小 scene 集成
   - 同时强制它交出 scene audit 五段式分析和用户可验收的详细汇报
3. 已为 `农田交互修复V3` 落下并行专属委托：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\2026-03-26-农田交互修复V3恢复开工委托-05.md`
   - 唯一主刀收紧为：`工具运行时资源链 -> 玩家反馈 -> 树木 / hover 遮挡口径`
   - 明确禁止回开 `013`、placeable 主链、`Primary.unity` 和 `GameInputManager.cs`
4. 已为 `导航检查V2` 落下 docs-only 专属委托：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2框架总审视委托-05.md`
   - 唯一主刀收紧为：全面框架总审视 + 重构蓝图收束
   - 明确禁止任何代码、live、solver patch、`GameInputManager.cs`、`Primary.unity`
5. 已把本轮统一回执协议钉死为“双层交付”：
   - 聊天只回最小字段
   - 详细汇报必须单独落文件，且默认按功能点组织，写清用户怎么验 / 怎么读

**关键裁定**
1. `NPCV2` 现在正式从“只读 blocker 态”切回真实业务施工，但仍按高危热 scene 陪跑口径执行。
2. `农田交互修复V3` 可以并行继续，但只能走 non-hot 代码线，不能再把自己拉回 placeable / runner / 热文件主战场。
3. `导航检查V2` 这轮不再继续代码推进，先把蓝图和后续阶段顺序钉死给用户看。
4. 从这轮开始，治理线程对外分发的默认交付已不再只是 prompt，而是：
   - prompt 文件
   - 用户可读详细汇报路径
   二者一起固定下来。

**恢复点 / 下一步**
- 当前治理线程下一步不再自己继续扩业务；
- 只需把这 3 条 prompt 交给用户转发；
- 等它们回执后，再按：
  - 是否命中本轮完成定义
  - 当前 own 路径是否 clean
  - 详细汇报是否足够用户验收 / 审读
  继续做下一轮典狱长裁定。

**补记**
- 上述批次文件与 3 份 prompt 已完成治理白名单 `sync` 到 `main`：
  - 提交：`3f149b4d`（`2026.03.26_典狱长-V2_05`）
- 当前 shared root 仍保留但未纳入本轮的 dirty 只剩：
  - `.codex/threads/Sunset/项目文档总览/memory_0.md`
  - `.kiro/specs/屎山修复/导航V2/000-gemini锐评-1.0.md`
  - `.kiro/specs/屎山修复/导航V2/000-gemini锐评-1.1.md`
- 它们都不是本轮 prompt 分发白名单的一部分，不应被误吞。

## 2026-03-26｜关于“dirty 互相传染”的治理判断：不能彻底解除 Git 规约，应改成分层并发模型

**当前主线目标**
- 用户在恢复开发放行后继续指出一个真实痛点：
  - 多线程并行时，线程容易把 shared root 的整体 dirty 错当成“我自己的现场”
  - 于是经常出现“我明明只做了 A，回看却像 A+B，干脆先停”的心理与流程阻塞
- 用户进一步追问：
  - 是否要彻底放弃当前 Git 规约，默认接受冲突，最后再做一次大收盘

**本轮分析结论**
1. 当前主要问题不是“规约太多”，而是“规约粒度错了”：
   - 把 `repo overall dirty` 当成了线程判断依据
   - 没有把“普通并行代码”和“热文件 / scene / asset”分层处理
2. 不建议彻底解除 Git 规约：
   - 对普通代码文件，晚一点集成可以承受
   - 但对 `Primary.unity`、Prefab、`.asset`、`TagManager.asset` 这类 Unity 序列化热文件，放任多人先写后冲突，最终成本通常更大
3. 也不需要退回“全仓一次只能一个人写”：
   - 正确模型应是“按文件类型分层并发”，而不是“全局单写”

**建议口径**
1. 普通代码 / 文档路径：
   - 允许并行
   - 线程不要再看全仓 `git status` 做归因
   - 只看自己的 `own roots / changed_paths / include paths`
2. 共享热文件：
   - 只对热目标启用短窗独占
   - 当前至少包括：
     - `Primary.unity`
     - `GameInputManager.cs`
     - `TagManager.asset`
     - `StaticObjectOrderAutoCalibrator.cs`
3. 如果未来想提高吞吐，不该改成“shared root 放任混写后再收盘”，而应改成：
   - 普通线程在 disjoint 路径并行
   - 命中同一热文件时，用独占写窗口或 branch / worktree carrier
   - 最后由单一 integrator 做明确收盘

**关键裁定**
1. `dirty 传染` 的根因是“归因视图”错了，不是 Git 本身不该存在。
2. 下一步应该放宽的是：
   - 线程对 foreign dirty 的心理负担
   - 普通路径的并发准入
3. 下一步不能放宽的是：
   - 同一热 scene / prefab / asset 的多人同时 tracked 写入
4. 之后若继续完善工具，最有价值的方向不是“取消所有闸门”，而是：
   - 让脚本默认输出 `own diff / foreign diff / same-file contamination` 三层结果
   - 避免线程再把 `A + B` 误读成“都是我的锅”

**恢复点 / 下一步**
- 当前先把这个判断作为治理分析结论保留；
- 如用户确认要落规则，下一轮应优先做的是：
  1. 收紧“只看 own roots，不看 overall dirty”的线程口径
  2. 把热文件与普通文件彻底分层
  3. 为未来的“统一收盘”单独设计 integrator 流程，而不是让 shared root 先长期混写

## 2026-03-26｜复核导航 / NPC 多段自述后，确认当前更深的问题不是 dirty 本身，而是“主线权威漂移 + 文件切片归属错位”

**当前主线目标**
- 用户贴回了父线程、`导航检查V2`、`NPCV2` 的多段自述与互相纠偏，希望确认：
  - 当前问题是否真的只是 dirty 互相传染
  - 还是现行规范本身已经让线程对“谁有权定义主线、谁该认领哪个文件切片”产生系统性错乱

**本轮复核事实**
1. 当前 live working tree 里真正还在 active dirty 的实现热面，主要是：
   - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
   - `Assets/000_Scenes/Primary.unity`
   - 3 份 `DialogueChinese*` 字体
2. `Assets/Editor/NPCAutoRoamControllerEditor.cs` 当前并不在 active dirty 中；
   - 与 `NPCV2` 记忆里的 `24886aad` 一致，说明 Editor 补口至少不是当前 live 主冲突面。
3. `NPCV2` 记忆已明确写出：
   - `65e1ee35`：`Primary.unity` 的 `001 / 002 / 003 HomeAnchor` 最小集成已做
   - `24886aad`：`NPCAutoRoamControllerEditor.cs` 的 Play Mode 报错已修
   - 当前更合理的恢复点是：等用户终验 / 做边界报实，而不是继续主动扩成 runtime 主线
4. `导航检查V2` 记忆已明确写出：
   - 当前唯一仍未过线的是 `NpcAvoidsPlayer`
   - 下一步应先只读钉死 `TickMoving / TryHandleSharedAvoidance / TryReleaseSharedAvoidanceDetour` 里的第一责任点
   - 下下步才是最小 runtime 补口 + 1 条 fresh + own 收尾
5. 因此，`dirty 互相传染` 不是唯一问题；
   - 当前更深的问题是：线程可以根据本地可见症状，自行把“我眼前最卡的点”升格成“当前总主线”

**关键裁定**
1. 我之前关于“按热文件分层并发”的判断仍然成立，但只说对了一半。
2. 当前真正需要优先改的，不只是 Git 并发模型，而是两层更上游的治理口径：
   - `主线权威`
   - `文件切片归属`
3. 当前更准确的现场层级应该是：
   - 总主线：真实右键导航里“玩家不再推着 NPC 走，NPC 侧避让过线”
   - 主实现线程：`导航检查V2`
   - 支持线程：`NPCV2`，仅在 `HomeAnchor / Inspector` 重新成为当前验收 blocker 时被短窗唤醒
   - 卫生 / owner 报实：后置层，不得再和 runtime 主刀并列抢主线
4. 这意味着：
   - `NPCV2` 可以有自己的“当前最卡的用户可见点”
   - 但它没有权力单独把这个点升级成与导航 runtime 并列的总主线
5. 同时，当前归属模型也有错位：
   - `NPCAutoRoamController.cs` 虽然路径在 `NPC` 下，但当前 active slice 明显属于导航 runtime
   - `NPCAutoRoamControllerEditor.cs` 才属于 NPC Editor / Inspector 支持 slice
   - `Primary.unity` 与 `DialogueChinese*` 字体则属于 shared-hot / mixed 目标，不能再被任一线程按“同目录 / 同主题”自动吞并

**规范调整方向**
1. 先新增“总主线权威文件”：
   - 只允许治理线程维护
   - 固定写清：
     - 当前唯一总主线
     - 当前主实现线程
     - 当前支持线程
     - 当前后置卫生项
     - 当前验收目标
     - 当前非目标
2. prompt 必须强制引用这份权威文件；
   - 子线程不再允许在自述里重写总主线，只能写“我服务于哪条总主线、我是主实现/支持/卫生中的哪一类”
3. 归属规则从“按文件路径 / 线程名猜”改成“按切片归属”：
   - runtime slice
   - editor / inspector slice
   - scene / asset slice
   - hygiene / report slice
4. 其中只有 shared serialized 热目标继续按整文件独占；
   - 普通代码允许按切片并行
   - shared scene / prefab / asset 不允许“切片口头化后多人同写”
5. 支持线程要增加“晋升条件”：
   - 只有当它处理的症状被治理线程明确判定为“当前总验收 blocker”，它才允许升到 active 支持窗口
   - 否则默认停在支持位，不主动扩刀
6. hygiene / owner 报实要正式降级为 Phase 4：
   - 不能再和 runtime 主刀并列发 prompt
   - 只能在主刀 checkpoint 后或明确停工窗口再做
7. 回执格式要增加两列硬字段：
   - `我当前服务的总主线：`
   - `我当前所属角色：主实现 / 支持 / 卫生`
   - 这样一眼就能看出线程是不是在偷偷改题
8. 对热文件并发的 Git 规则仍要保留，但要服务于上面的层级，而不是单独成为全部治理中心

**恢复点 / 下一步**
- 当前最值得落地的规范升级，不是继续争论“是不是全局单写”，而是：
  1. 先把“谁能定义总主线”钉死
  2. 再把“文件按什么切片归属”钉死
  3. 最后才是按热文件 / 普通文件做并发分层
- 若用户认可，下一轮应正式把这三层一起下压进：
  - `AGENTS.md`
  - 当前规范快照
  - 治理分发规范
  - prompt 模板

## 2026-03-26｜已对 `典狱长-V2` 的首轮越界回复做 Path C 审视，纠偏为“只回答并发 dirty 归因问题”

**当前主线目标**
- 用户贴回了 `典狱长-V2` 的首轮回复，并明确指出其中“导航唯一总主线 / 导航唯一主实现 / NPCV2 支持线程 / cleanup 整体后置”属于胡言乱语，希望我直接替他审掉并纠偏。

**本轮核查事实**
1. `典狱长-V2` 的首轮启动委托 `2026-03-26_典狱长-V2首轮启动委托-03.md` 明确只允许它做：
   - current-wave 基线复核
   - inbox 建立
   - 钉死 5 条线治理图
   - 明确后续治理从这里开始归它承接
2. 同一份启动委托还明确禁止：
   - 重写 second-wave prompt
   - 抢先写 third-wave prompt
   - 重新争论 `V1` 是否继续治理
   - 把 `NPCV2` 从停发态又硬拉回施工态
3. 用户这轮原问题本身也不是“请重排当前总主线”，而是：
   - `dirty` 互相传染怎么避免
   - 是否要彻底解除 Git 规约
4. 因此，它前半段“`own diff / foreign diff / same-file contamination` 三层归因 + hot target 独占 + integrator 收盘”的方向可以保留；
   - 但后半段把问题偷换成“导航唯一总主线 / NPC 支持位 / 新治理权威文件体系”属于明显越界。

**已完成事项**
1. 已在治理工作区新增审视报告：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-26_典狱长-V2首轮越界判断审视报告-01.md`
2. 已在当前治理线程目录新增纠偏委托：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\对话丢失修复\2026-03-26_典狱长-V2并发dirty问题纠偏委托-04.md`
3. 已把纠偏刀口锁死为：
   - 只回答用户的并发 dirty 原题
   - 显式撤回 5 条错误判断
   - 不准再改写 current-wave 治理图
   - 不准再发明新的治理基础设施

**关键判断**
- 这次问题的优先级不是“交接不够”，而是：
  - `典狱长-V2` 首轮没有 obey 自己的启动委托；
  - 它把一个并发 dirty 归因问题偷换成了更大的治理改题问题。
- 当前正确保留面只有：
  - 普通路径并行
  - hot target 独占
  - `own / foreign / same-file contamination`
  - integrator 收盘
- 当前必须撤回面包括：
  - 导航唯一总主线
  - 导航唯一主实现
  - `NPCV2` 支持线程化
  - `Primary.unity` / 字体 / owner / cleanup blanket 后置
  - 一整套新增治理文件体系

**恢复点 / 下一步**
- 当前先把 `典狱长-V2` 拉回“只回答并发 dirty 归因”的正确作用域；
- 等它交回纠偏回执后，再决定是否需要把其中真正有效的并发口径同步进 live 规则。

## 2026-03-26｜已核清“讨论性更新是否需要收口”的规范边界，并确认导航回执的阻断理由写偏了

**当前主线目标**
- 用户继续追问：导航相关线程把“这轮只是讨论和记忆回写，所以没有 git sync”当成正常口径，这是否符合 Sunset 规范，还是规范落地又漂了。

**本轮核查事实**
1. `Sunset/AGENTS.md:161-165` 已明确：
   - 任何产生可复用结论的 Sunset 实质性工作，都不能只写在线程记忆里；
   - 只读分析如果产生稳定治理结论、路由结论、风险判断，也要更新记忆。
2. `workspace-memory.md:355-371` 也明确把：
   - 架构决策
   - 问题分析
   - 设计变更
   - 技术债务识别
   - 锐评执行
   统统列为“即使没有代码修改也必须触发 memory 记录”的讨论类场景。
3. `Sunset/AGENTS.md:179-205` 已明确：
   - 实质性工作在完成记忆更新后，只要达到可提交状态，就应继续 `git-safe-sync`
   - `main-only` 白名单 sync 不再因 unrelated dirty 一刀切阻断
   - 真正会阻断的是本轮白名单所属 `own roots` 下仍有未纳入的 remaining dirty / untracked。
4. 当前导航 owner roots 现场并不干净，至少仍包含：
   - 已修改：线程 / 父工作区 / 子工作区的多份 `memory.md`
   - 未跟踪：`导航V2/002-导航V2开发宪法与阶段推进纲领.md`、`导航检查/...-08.md`、`导航检查/...-09.md` 等

**关键判断**
- “讨论性内容天然不用收口”不是现行规范，也不是我认可的落地形态。
- 更准确的说法应是：
  - 讨论性内容如果形成稳定结论，必须记忆；
  - 若已达到可提交状态，应继续白名单 sync；
  - 若不能 sync，必须把真实阻断写成：
    - own roots 还有剩余 dirty / untracked
    - 或 hot / mixed 目标阻断
  - 不能笼统写成“shared root 里还有别的线程 dirty，所以这轮讨论不收口”。

**恢复点 / 下一步**
- 后续再审类似回执时，典狱长应直接追问：
  1. 这轮是否已达到可提交状态
  2. 若未 sync，阻断来自 own roots 还是 hot / mixed blocker
  3. 当前 own 路径是否 clean
- 不再接受“讨论性更新所以先不收口”的空泛说法。

## 2026-03-26｜已为 `典狱长-V2` 重写整合版纠偏委托，统一吸收“并发 dirty + 讨论收口边界”两类误判

**当前主线目标**
- 用户尚未发出前一版纠偏委托，希望我先吸收最新新增的问题，再整理成最新可直接转发版本给 `典狱长-V2`。

**本轮已完成**
1. 新增整合版审视报告：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-26_典狱长-V2越界与讨论收口误判审视报告-02.md`
2. 新增整合版纠偏委托：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\对话丢失修复\2026-03-26_典狱长-V2纠偏委托-05.md`
3. 新版委托已把两组问题同时钉死：
   - 并发 dirty 原题被偷换成 current-wave 改题
   - 讨论性更新与 Git 收口条件被混成一句模糊挡箭牌

**关键判断**
- 当前最新可发送版本已经不是 `...并发dirty问题纠偏委托-04.md`，而是 `...纠偏委托-05.md`。
- 这轮仍然不是让 `V1` 代替 `V2` 去改 live 规则；
  - 只是先逼 `V2` 把自己的理解和口径纠正到位。

**恢复点 / 下一步**
- 现在可直接向用户提供：
  - 新的审视报告路径
  - 新的 `V2` 可转发话术
- 等 `V2` 回执后，再决定是否值得进入规范层同步。

## 2026-03-27｜`典狱长-V2` 已完成纠偏，下一步转入首个真实治理实操

**当前主线目标**
- `V2` 已交回合格纠偏回执；当前不应再停留在“认错”和“抽象讲解”层，而应立刻验证它能否把纠偏后的口径应用到真实治理对象。

**本轮已完成**
1. 已确认 `V2` 纠偏回执通过：
   - 已撤回 7 条错误判断
   - 已把并发 dirty 与讨论收口边界答回用户原题
2. 已新增它的首个真实治理委托：
   - `D:\迅雷下载\开始\.codex\threads\系统全局\对话丢失修复\2026-03-27_典狱长-V2首个实操治理委托-06.md`
3. 首考对象已固定为：
   - 一条真实导航回执
   - 核心是审“讨论性更新不收口 / shared root 有别人 dirty 所以不 sync”是否成立
   - 并要求它给出真实阻断与四类裁定

**关键判断**
- 从治理接班角度看，`V2` 现在已经过了“纠偏”阶段，下一步最该做的是实操验证，而不是继续让 `V1` 代劳。
- 这轮仍然不该让它改 live 规范；只该让它在真实个案上做一次窄治理。

**恢复点 / 下一步**
- 现在可直接把 `...首个实操治理委托-06.md` 发给 `典狱长-V2`；
- 等它交回第一份真实个案裁定后，再决定是否继续放权到更大的治理批次。

## 2026-03-27｜`典狱长-V2` 首个真实治理实操已通过，当前不再需要 `V1` 继续考试式放题

**当前主线目标**
- 用户要求我以父线程身份验收 `典狱长-V2` 的首个真实治理实操是否合格，并判断下一步是继续考试，还是正式放权。

**本轮核查事实**
1. `典狱长-V2` 已把首个实操对象正确压成：
   - 一条导航线程“讨论性更新不收口 / shared root 里还有别人 dirty 所以不 sync”的真实回执
2. 它已正确给出：
   - 四类裁定：`继续发 prompt`
   - 核心审结论：讨论 / 文档新增仍属实质性工作，应记忆、应判断收口
   - 真实阻断：`own roots` 未 clean，而非 foreign dirty 或 hot/mixed blocker
3. 这份结论与当前导航 owner roots 现场一致：
   - `.kiro/specs/屎山修复` 这条根下确实仍有 sibling dirty / untracked
4. `skill-trigger-log.md` 已新增 `STL-20260327-051`，证明这不是口头补课，而是一次真实个案裁定

**关键判断**
- 这次 `V2` 已经通过“首个真实治理实操”：
  - 不只是会讲规范
  - 而是能把规范用在真实个案上
- 当前没有必要继续让 `V1` 用考试式 prompt 代劳；
  - 后续典狱长治理应优先交回 `V2`

**恢复点 / 下一步**
- 现在可以把 `V2` 视为“可放权但继续观察”的接班状态；
- 下一步不是再给它出题，而是让它继续处理真实治理现场；
- 只有当它再次出现：
  - 改题
  - 乱重排线程角色
  - 把真实阻断写成空泛挡箭牌
  时，`V1` 再重新介入纠偏。

## 2026-03-26｜复核 `NPCV2 / 导航检查V2` 自述后，当前总主线与子线优先级重新钉死

**当前主线目标**
- 用户贴回了父线程、`NPCV2`、`导航检查V2` 的多段自述，希望确认：
  - 当前到底是不是“新分支”
  - 谁才是现在真正的主线线程
  - `NPCV2` 到底是继续并行主干，还是已经退到支持位

**本轮复核事实**
1. 当前 live dirty 现场里仍明确存在：
   - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
   - `Assets/000_Scenes/Primary.unity`
   - 3 份 `DialogueChinese*` 字体
2. 当前 live dirty 里不包含：
   - `Assets/Editor/NPCAutoRoamControllerEditor.cs`
   说明 editor 热修已不再是当前 active collision face。
3. `NPCV2` 工作区记忆已明确记录：
   - `65e1ee35`：`Primary.unity` 中 `001 / 002 / 003` 的最小 `HomeAnchor` 集成已落地
   - `24886aad`：`NPCAutoRoamControllerEditor.cs` 的 Play Mode `MarkSceneDirty` 报错已修复
   - 当前恢复点已经切到：
     - 守住 own / non-own 边界
     - 等用户终验或做 owner 报实
   - 不再是“继续沿 editor 链无上限往前修”
4. `导航检查V2` 工作区与线程记忆已明确记录：
   - 当前唯一有效实现主线仍是 `NpcAvoidsPlayer`
   - 下一步先做只读责任点压缩：
     - `TickMoving()`
     - `TryHandleSharedAvoidance()`
     - `TryReleaseSharedAvoidanceDetour()`
   - 下下步才是 1 个最小 runtime 补口 + 1 条 fresh + own 收尾

**关键裁定**
1. 总主线没有变：
   - 仍然只有一个：真实右键导航里“玩家不再推着 NPC 走，NPC 侧避让过线”
2. 当前唯一主实现线程是：
   - `导航检查V2`
3. `NPCV2` 当前不是与导航并列的第二条主实现线：
   - 它已经完成两刀有效工作；
   - 现在更接近支持 / 验收 / owner 报实位；
   - 只有当用户再次明确看到 `HomeAnchor` 仍为空或 Inspector 补口仍断时，它才重新回到 editor-only 修复位
4. 因此“父线程分派执行阶段”这个说法只对一半：
   - 对的是：总主线确实仍是同一条，且父线程在裁边界
   - 不对的是：当前两条子线并不是同级并行主推进
   - 更准确的层级应是：
     - 总主线：导航 runtime 过线
     - 主子线：`导航检查V2` 继续修 release / recover 执行链
     - 辅子线：`NPCV2` 暂停主动扩刀，只保留验收 / owner 报实 / editor 复发时的窄补口
5. cleanup、字体、`Primary.unity` mixed hot 面、Gemini 大架构讨论，当前都不是主线

**恢复点 / 下一步**
- 当前最重要的是先让 `导航检查V2` 给出：
  - 责任点只读钉死结果
  - 然后 1 条最小 runtime 结果
- `NPCV2` 当前不应继续主动找新刀口；
- 除非用户现场再次复现 `HomeAnchor` 空或 Inspector 补口失效，否则它应停在支持位，不再和导航线程并列抢主线。

## 2026-03-27｜全盘复核后，当前 Sunset 已进入“规则实盘期”，但规范栈仍是半收敛状态

**当前主线目标**
- 用户要求我不要再只围着单条线程或单份 prompt 讲话，而是从全局回看：
  - 我当前到底扮演什么角色
  - Sunset 全局现在处于什么阶段
  - 当前实际生效的规范到底是什么
  - 这些规范是否已经符合最初设计预期
  - 各线程、skills、文档规约与 shared root 现场现在各自是什么状态

**本轮复核事实**
1. 当前 live 基线已推进到：
   - `D:\Unity\Unity_learning\Sunset @ main @ ee7ba4c1`
2. 当前 shared root 仍存在真实 active dirty，且不再只是单一线程：
   - hot / mixed：`Assets/000_Scenes/Primary.unity`、`Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
   - mixed shared resource：3 份 `DialogueChinese*.asset`
   - 导航 runtime：`Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
   - 农田 / 交互实现面：`TreeController.cs`、`FarmToolPreview.cs`、`PlacementManager.cs`、`PlayerInteraction.cs`、`PlayerThoughtBubblePresenter.cs`、`InventoryPanelUI.cs`、`ChestController.cs`
   - 多条治理 / 工作区 / 线程 `memory.md`
3. 当前 Unity MCP 资源列表已恢复可见，但运行态仍是：
   - `mcpforunity://instances -> instance_count = 0`
   - `mcpforunity://editor/state -> no_unity_session`
   说明“工具层可见”不等于“当前有可用 Unity live 会话”。
4. 当前真正 live 生效的规范栈，已经不是单文件，而是分层覆盖：
   - 用户当前裁定
   - `Sunset/AGENTS.md`
   - `Sunset当前规范快照_2026-03-22.md`
   - `典狱长模式_治理总闸与分发规范.md`
   - `治理线程批次分发与回执规范.md`
5. 同时也确认了当前规范栈仍存在两类明显漂移：
   - `git-safety-baseline.md` 前半段仍保留 branch-first 旧口径，不能再被直接当成 live 默认开发模型
   - `workspace-memory.md` / `documentation.md` 仍是老一代通用规范，当前在 Sunset 中实际要受 `AGENTS.md + 当前规范快照` 重新约束
6. 当前 skills 实际使用也与设计有一处稳定偏差：
   - `skills-governor`、`sunset-workspace-router`、`sunset-warden-mode` 已进入真实执行链
   - 但注册表中的 Sunset 主干 skill `sunset-startup-guard` 仍未在当前会话显式暴露，实盘上长期由 `skills-governor + 手工等价前置核查` 顶替

**关键判断**
1. 我当前的真实身份，不再是“继续替每条线程写 prompt 的普通治理线程”，而是：
   - 全局治理总闸
   - shared root 现场审计者
   - 线程 owner / prompt / 回执 / 规范一致性的复核者
2. Sunset 当前所处阶段，也不再是“设计治理体系”的阶段，而是：
   - `main-only + whitelist-sync + exception-escalation` 已经进入实盘运行
   - 现在真正的问题变成：这套规则有没有被 threads 和 shared root 现场真正执行到位
3. 当前规范方向基本对，但还不能说“完全符合设计预期”：
   - 对的部分是：并发普通路径已放开、hot target 独占、`own-path-clean` 已成硬闸、典狱长四类裁定已落地、skill 审计已真实运行
   - 不对的部分是：规范源还没有完全收敛为单层清晰体系，旧 steering 文档与新 live 规则仍有双源；`sunset-startup-guard` 仍处于“设计要求存在、实盘长期手工等价”的状态；治理工作区自己也还带着未同步文档尾账
4. 当前各线程真实分层已很清楚：
   - 业务主刀：`导航检查V2`、`农田交互修复V3`
   - 支持 / 基线 / 外部阻塞位：`NPCV2`
   - 规划 / parked：`spring-day1V2`
   - owner / 调度文档位：`导航V2`
   - 归档辅助线：`项目文档总览`
   - 治理本体：`Codex规则落地`

**恢复点 / 下一步**
- 当前最该做的不是再发明新治理体系，而是把已存在的 live 规范真正收敛成现场可执行现实：
  1. 把仍处于 active dirty 的 hot / mixed 面继续压缩到可清楚归属的 owner 面
  2. 停止让旧 steering 文档继续与 `AGENTS.md / 当前规范快照` 并列制造双源
  3. 继续把 `sunset-startup-guard` 从长期手工等价，补回显式可触发状态
  4. 让治理工作区自己先做到“文档层 / 审计层 clean”，再去要求业务线程进一步守规

## 2026-03-27｜已把 live 规则源、startup-guard discovery gap 与治理线 own tail 一次性压回正文层

**当前主线目标**
- 用户明确要求不要再停在分析，而是直接把三件“不符合预期”的内容落地：
  - 规范源仍有双源
  - `sunset-startup-guard` 仍是设计在前、实盘缺位
  - 治理线自己还有尾账

**本轮完成事项**
1. 直接回写 Sunset 的 live 入口层：
   - `AGENTS.md`
   - `Sunset当前规范快照_2026-03-22.md`
   - `Sunset当前唯一状态说明_2026-03-17.md`
   - `Sunset现行入口总索引_2026-03-17.md`
   - `基础规则与执行口径.md`
2. 在上述文件中写死当前 live 层级：
   - `用户裁定 -> AGENTS.md -> 当前规范快照 -> 命中的治理规范正文 -> .kiro/steering`
3. 给通用 steering 正文补边界说明：
   - `git-safety-baseline.md`
   - `workspace-memory.md`
   - `documentation.md`
   统一说明：它们提供通用结构 / 通用基线，不再单独定义当前 Sunset 的 live 默认开发模型。
4. 回正 shared root 占用文档：
   - `shared-root-branch-occupancy.md`
   - `daily_policy` 改回 `main-only + whitelist-sync + exception-escalation`
   - `last_verified_head` 更新为本轮实核的 `1401ae8c`
5. 补强 `sunset-startup-guard` 的两套 skill 副本与注册表：
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\SKILL.md`
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\references\checklist.md`
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\agents\openai.yaml`
   - `C:\Users\aTo\.agents\skills\sunset-startup-guard\...`
   - `C:\Users\aTo\.codex\memories\global-skill-registry.md`
6. 其中当前固定化的新口径是：
   - `sunset-startup-guard` 未显式暴露 = discovery gap
   - discovery gap != 闸门失效
   - 仍必须 manual-equivalent
   - 仍必须写 trigger log

**关键判断**
- 这轮没有去追所有历史 memory / design 的旧句子，而是只修“现在还会继续误导线程进入现场的 live 入口”。
- 治理线自己的 own tail 也不再只是“知道有尾账”，而是已经进入：
  - repo 内 live 文档已改
  - repo 内治理 memory 已补
  - repo 外 skill / 注册表已补
  的实收状态。

**恢复点 / 下一步**
- 现在治理线剩下的不是“还没动”，而是：
  1. 再把本轮 repo 内治理白名单正式 sync 到 `main`
  2. 再向用户交代：mixed dirty 一锅端只能是最终收官选项，不应成为日常 live 模型

## 2026-03-27｜repo 内治理白名单已正式同步到 `main`

**本轮完成事项**
1. 已通过 stable launcher 执行：
   - `git-safe-sync.ps1 -Action sync -Mode governance -OwnerThread Codex规则落地`
2. 本轮治理提交：
   - `101e160c`
   - commit message：`2026.03.27_Codex规则落地_01`
3. 已确认本轮白名单 own roots 为：
   - `AGENTS.md`
   - `.kiro/locks`
   - `.kiro/steering`
   - `.kiro/specs/Codex规则落地`
   - `.kiro/specs/Steering规则区优化/当前运行基线与开发规则`
   当前 `remaining dirty = 0`

**关键结论**
- “规范源双源收敛 + startup-guard discovery gap 口径补齐 + 治理线 own tail 收口”这三件事，repo 内部分已经正式落入仓库历史。

**恢复点 / 下一步**
- 后续只剩两层：
  1. 向用户明确解释为什么 mixed dirty 一锅端不能升格为日常 live 模型
  2. 继续观察业务线程与 hot / mixed 现场是否按新口径真正执行

## 2026-03-27｜高速并发实盘复核：当前四线程可继续，但只按“自由切片 / 热点避让 / 最终收盘桶”三层执行

**当前主线目标**
- 用户已明确把 `导航 / NPC / 农田 / Day1` 四条线切到高速模式，并要求治理线程不要再空谈，而是直接按真实现场说明：
  1. 谁现在还能继续做什么
  2. 哪些面必须避让
  3. 哪些 dirty 这轮不该硬管，而应留给最终 integrator 收盘

**本轮实盘核查**
1. 当前 live 基线：
   - `D:\Unity\Unity_learning\Sunset @ main @ 477b3e7e`
2. shared root 占用文档当前仍显示：
   - `owner_mode = neutral-main-ready`
   - `is_neutral = true`
   - 但 `last_verified_head = 1401ae8c`
   - 说明 shared root 入口中性成立，但占用文档 head 记账已经落后于真实现场
3. 当前 working tree 已确认四条高速线程同时在写：
   - `导航检查 / 导航检查V2`
   - `NPC`
   - `农田交互修复V3`
   - `spring-day1V2`
4. Unity MCP 当前事实：
   - `mcpforunity://instances -> instance_count = 1`
   - 但 `mcpforunity://editor/state` 返回 stale `playmode_transition`
   - 因此“当前有 Unity 实例”成立，但“现在确实已回到 Edit Mode”不能靠这次 stale 快照外推

**当前三层裁定**
1. 自由切片，可继续并行：
   - `导航`：
     - 当前 own dirty 落在：
       - `NavigationLiveValidationMenu.cs`
       - `NavigationLiveValidationRunner.cs`
       - 导航工作区 / 线程记忆
     - 当前应继续只打 `NpcAvoidsPlayer` 的 detour `release / recover` 同链
     - 不应借机转去 `Primary.unity`
   - `NPC`：
     - 当前 own dirty 已扩到非 scene 内容 / 反馈层：
       - `Assets/111_Data/NPC/*.asset`
       - `NPCDialogueContentProfile.cs`
       - `NPCRoamProfile.cs`
       - `PlayerNpcNearbyFeedbackService.cs`
       - `NPCToolchainRegularizationTests.cs`
       - 对应工作区 / 线程记忆
     - 当前可继续推进 `P2 / P3` 这类不撞热 scene 的内容与玩家侧反馈切片
   - `spring-day1`：
     - 当前 own dirty 集中在：
       - `NPCDialogueInteractable.cs`
       - `DialogueManager.cs`
       - `SpringDay1Director.cs`
       - `SpringDay1PromptOverlay.cs`
       - `SpringDay1WorkbenchCraftingOverlay.cs`
       - `SpringDay1DialogueProgressionTests.cs`
       - 对应工作区 / 线程记忆
     - 当前仍是可继续的 non-hot story/UI 切片
   - `农田`：
     - 当前 own dirty 集中在：
       - `TreeController.cs`
       - `FarmToolPreview.cs`
       - `PlacementManager.cs`
       - `InventoryPanelUI.cs`
       - `ChestController.cs`
       - 对应工作区 / 线程记忆
     - 这些文件仍可继续按当前大清盘任务书推进
2. 热点避让，当前只允许单线持有：
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
     - 当前是 `M`
     - 且当前只有 `农田` 线正在围绕它做交互收口
     - 因而这轮应视为“农田暂时持有的热代码面”
     - 其他线程此时不应再顺手进去
   - `Assets/000_Scenes/Primary.unity`
     - 当前仍是 `M`
     - 没有明确 owner
     - 仍按 mixed hot 面处理
     - 这轮四条高速线都不应把“shared root neutral”误读成“scene 可写”
3. 最终 integrator 收盘桶候选：
   - `Primary.unity`
   - `DialogueChinese*.asset`
   - 后续若继续证明确实跨线程缠死、又不适合再逐线清责的 Unity 自动副产物
   - 这些面可以留给最终收官统一处理，但不应倒灌成当前每条线程的日常停工理由

**关键结论**
1. 当前最正确的治理动作不是再发 cleanup 批次，也不是重新排工所有线程。
2. 更合适的做法是：
   - 放行四条线继续各自自由切片
   - 只把 `GameInputManager.cs` 和 `Primary.unity` 当作显式避让面
   - 把 `Primary.unity + DialogueChinese*` 保留在最终 integrator 收盘桶
3. 我当前的位置因此进一步收紧为：
   - 不再替大家打扫 every dirty
   - 只在以下三种情况出手：
     - 有第二条线程试图进入 `GameInputManager.cs`
     - 有线程想重新进入 `Primary.unity`
     - 用户决定打开最终 integrator 收盘窗口

**恢复点 / 下一步**
- 现在可以直接按下面口径继续：
  1. `导航` 继续 `release / recover`
  2. `NPC` 继续非 scene 的内容 / 反馈 / 关系切片
  3. `农田` 继续当前交互大清盘，但不再扩新的 hot target
  4. `Day1` 继续 non-hot story/UI 正式面
- 治理线程下一步默认只做：
  - 热点冲突仲裁
  - 最终 integrator 收盘前的总表维护
  - 不再主动打断大家做常规卫生

## 2026-03-27｜用户批准新的 hot target 实战口径：`GameInputManager.cs` 放开触点并发，`Primary.unity` 改为 NPC 接盘后再转 spring-day1

**当前主线目标**
- 用户明确批准一条新的实际路线，并要求我先做路线审核：
  1. `GameInputManager.cs` 不再按“谁先 dirty 谁独占”理解，而是改成可并发但必须记触点，最后可由 integrator 收盘
  2. `Primary.unity` 不放成多人随便写，继续保持单 writer
  3. 当前 `Primary.unity` 的 scene writer 顺序应改成：`NPC -> spring-day1`

**本轮实盘核查**
1. `GameInputManager.cs`
   - 当前确实在 dirty
   - 实际改动集中在输入主分发入口：
     - `HandleUseCurrentTool`
     - `HandleRightClickAutoNav`
     - `ShouldPreservePlacementModeForCurrentRightClick`
     - `TryBlockAxeActionAgainstHighTierTree`
     - `TryGetTreeInteractableAtWorld`
     - `TryGetChestInteractableAtWorld`
   - 这再次证明它不是“不能碰”，而是“文件共享、语义容易串锅”
2. `Primary.unity`
   - 当前仍是 `M`
   - `Check-Lock.ps1` 在发锁前返回 `unlocked`
   - `shared-root-branch-occupancy.md` 当前 `owner_thread = none`
   - 因此它的真实状态更准确是：
     - `ownerless stale mixed scene`
     - 不是“有人正在占着写”
3. 路线兼容性
   - `NPC` 当前确实仍有 scene 级主线未闭
   - `spring-day1` 当前主要仍在 non-hot story/UI 面，可排在下一个 scene writer 窗口
   - 因此用户提出的 `NPC -> spring-day1` 顺序与现现场一致，可执行

**本轮已执行动作**
1. 已把新口径同步进 live 规则层：
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前规范快照_2026-03-22.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\典狱长模式_治理总闸与分发规范.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\治理线程批次分发与回执规范.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-hot-zones.md`
2. 已同步修改相关 skill：
   - `C:\Users\aTo\.codex\skills\sunset-lock-steward\SKILL.md`
3. 已正式为 `NPC` 发出 `Primary.unity` 的物理写锁：
   - 目标：`Assets/000_Scenes/Primary.unity`
   - owner：`NPC`
   - task：`primary-scene-takeover`
   - checkpoint：`接盘当前 dirty scene；先完成 NPC scene 主线，再释放给 spring-day1`
   - granted_by：`user-2026-03-27-route-review`

**当前稳定结论**
1. `GameInputManager.cs`
   - 现在继续属于强制报实热点
   - 但 live 口径已经从“默认单 writer 停工”改成“触点并发 + touched_touchpoints + integrator 收盘”
2. `Primary.unity`
   - 现在继续属于单 writer 场景面
   - 但它不再被口头误判成“假活跃占用”
   - 当前已正式进入 `NPC` 的接盘窗口
3. 这轮不是只做文档判断，而是已经把项目现场状态一起推进到了：
   - `GameInputManager.cs = unlocked`
   - `Primary.unity = locked by NPC`

**恢复点 / 下一步**
- 接下来如果 `NPC` 完成本轮 scene checkpoint，应通过 `Release-Lock.ps1` 释放 `Primary.unity`
- 下一个 scene writer 默认按本轮用户裁定交给 `spring-day1`
- 治理线程之后对这两类文件的判断口径固定为：
  - `GameInputManager.cs`：先问触点
  - `Primary.unity`：先问当前 writer 是谁

## 2026-03-27｜按用户最新裁定只向 `NPC` 下发一条 `Primary.unity` scene writer prompt，暂不生成 `spring-day1` prompt

**当前主线目标**
- 用户已明确裁定：这轮先不要给 `spring-day1` 写 prompt，只给 `NPC` 写一条新的 scene writer 委托；等 `NPC` 做完并由我复核后，再决定是否发下一棒。

**本轮只读核查**
1. `Primary.unity`
   - 当前仍为 `M`
   - 但物理锁已在：
     - `D:\Unity\Unity_learning\Sunset\.kiro\locks\active\A__Assets__000_Scenes__Primary.unity.lock.json`
     里明确发给 `NPC`
2. `NPC` 当前正式任务底账
   - `P0 = done`
   - `T-P1-01 = done`
   - `T-P1-02` 在旧文档里仍写作 `blocked-hotfile`
   - 但在用户已授予写窗之后，这个旧 blocker 口径不应继续阻止它进入 scene 最小切片
3. 当前应发的不是泛泛“去接盘 Primary”，而是一条硬切片：
   - 只做 `T-P1-02 -> T-P1-03 / T-P1-04 / T-P1-05`
   - 只落三名 NPC 的最小点位对象
   - 不进入 `T-P1-06`
   - 不放锁给 `spring-day1`

**本轮已落动作**
1. 已新增 prompt 文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC-P1-02-Primary场景接盘与最小点位落地委托-01.md`
2. prompt 内已明确钉死：
   - 当前写窗已授予，不再反复论证旧 blocker
   - 这轮只允许落以下最小对象集：
     - `NPC_001_Stay_SquareWatch`
     - `NPC_002_Stay_YardSun`
     - `NPC_003_Stay_StudyBench`
     - `NPC_SHARED_Meet_MainCrossing`
     - `NPC_001_Solo_PathShoulder`
     - `NPC_002_Solo_GardenEdge`
     - `NPC_003_Solo_ObservationEdge`
   - 不碰字体、不碰 `GameInputManager.cs`、不碰导航 runtime、不做 mixed cleanup
   - 做完后只停在 `ready-for-warden-review`

**恢复点 / 下一步**
- 当前治理线程下一步不是继续群发，而是等 `NPC` 回执：
  1. scene 最小切片是否真实落地
  2. 当前 own 路径是否 clean
  3. 是否已经到达 `ready_for_release`
- 只有我复核通过后，才会决定是否给 `spring-day1` 写下一条 scene writer prompt

## 2026-03-27｜根据用户补充现场，撤回“此刻先发 Primary scene prompt”的优先级，改为给 `NPC` 一条 `0.0.2` 中继续推提示

**当前主线目标**
- 用户补充了 `NPC` 当前真实现场截图与上一条长 prompt 后，要求我重新判断：
  - 现在是不是应该继续让它沿 `0.0.2清盘002` 往前干
  - 并只给一个中间引导提示，而不是把它打断去执行我刚写的 `Primary.unity` scene writer prompt

**本轮复核结论**
1. `NPC` 当前没有跑偏
   - 它已经落出了：
     - `2026-03-27-NPC-0.0.2清盘002-现状排查与验收回退矩阵.md`
     - `2026-03-27-NPC-0.0.2清盘002-详细落地任务列表.md`
   - 这说明它已经把用户刚刚的“非正式聊天并未成立、0.0.1 有误判完成”这一层重新钉死
2. 当前更适合继续的，不是 `Primary.unity`
   - 而是 `0.0.2` 主线里的：
     - `002 / 003` 按 `E` 发起的 NPC 非正式聊天首个闭环
3. 当前仓库现场也支持这个判断
   - `NPC` 相关 active dirty 已经落在：
     - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
     - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
   - 说明它已经进入真实聊天入口与表现层，不适合此刻硬切回 scene 写窗

**本轮已落动作**
1. 已新增中继提示文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-03-27-NPC-0.0.2清盘002-中继续推提示-非正式聊天首个闭环切片-01.md`
2. 新提示已明确要求：
   - 保留 `0.0.2` 两份主文档，不再继续扩解释文档
   - 直接进入：
     - `T-002-C01`
     - `T-002-C02`
     - `T-002-C03`
     - `T-002-C04`
   - 主刀固定为：
     - `002 / 003` 的按 `E` 发起、玩家先说、NPC 延迟回复、可跳过动效、可中断的首个非正式聊天闭环
   - 明确禁止：
     - `Primary.unity`
     - `GameInputManager.cs`
     - 拆烂 `001` 的 `spring-day1` 正式剧情链
     - 把工作台提示 UI 一起吞掉

**恢复点 / 下一步**
- 当前正确动作不是打断 `NPC` 切去 scene，而是继续让它沿 `0.0.2` 主线把第一个真实聊天闭环做出来
- 等它把这刀做完，我再基于回执和现场决定：
  - 是继续 `0.0.2` 下一刀
  - 还是再回到 `Primary.unity` 的 writer 窗口

## 2026-03-28｜已把“用户可读汇报层”升级成 Sunset live 硬规则

**当前主线目标**
- 用户明确指出：现在线程并不是都没做事，而是越来越不会把“实际产出 / 当前阶段 / 剩余内容 / 下一步”说成人话；现有规范虽然管住了治理回执，却没有管住“必须让用户一眼看懂”。

**本轮分析结论**
1. 当前 Sunset 现有规则主要解决的是：
   - 治理可审计
   - 白名单可收口
   - hot target 可报实
2. 但缺了一层真正强制的：
   - `用户可读汇报契约`
3. 因此现场会出现：
   - 线程写的内容对治理有用
   - 但用户仍然不知道：
     - 这轮实际做成了什么
     - 还剩什么
     - 到哪个阶段了
     - 下一步该怎么发令

**本轮已落动作**
1. 已新增分析与方案文档：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-28_线程用户可读汇报失真分析与强制规约方案.md`
2. 已把新规则同步进 live 规则层：
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前规范快照_2026-03-22.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\治理线程批次分发与回执规范.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\典狱长模式_治理总闸与分发规范.md`
3. 已同步到相关 skill：
   - `C:\Users\aTo\.codex\skills\sunset-governance-dispatch-protocol\SKILL.md`
   - `C:\Users\aTo\.codex\skills\sunset-prompt-slice-guard\SKILL.md`

**当前固定新口径**
1. 以后必须明确区分两种文本：
   - `给治理看的最小回执`
   - `给用户看的用户可读汇报`
2. 只要对象是用户，或用户在问：
   - 现在做到哪了
   - 实际做了什么
   - 还剩什么
   - 下一步做什么
   - 到哪个阶段了
   线程就必须先回答：
   - `当前主线`
   - `这轮实际做成了什么`
   - `现在还没做成什么`
   - `当前阶段`
   - `下一步只做什么`
   - `需要用户现在做什么`
3. `changed_paths / checkpoint / 参数名 / best-known stable / dirty / lock / owner 报实` 等技术信息，只能放在后面的技术审计层
4. 如果线程只交技术 dump、不交用户可读层，治理线程应视为“汇报不合格”

**恢复点 / 下一步**
- 从这轮开始，治理线程后续写 prompt 和审回执时，都要把“用户可读汇报层”当成硬闸门
- 下一步进入实盘观察：
  - 谁继续交技术 dump
  - 谁已经开始按新口径汇报

## 2026-03-28｜把“用户可读汇报层”进一步收紧成六点保底卡，明确“先给用户看，再给治理看”

**当前主线目标**
- 用户要求我不要只停在“至少回答 6 项”的软口径，而是把这 6 项彻底写成强制格式：
  - 每轮都必须有
  - 先给用户看懂
  - 然后线程再补自己认为必要的内容
  - 最后才给治理看技术审计

**本轮已落动作**
1. 已把 Sunset live 规则进一步收紧到：
   - `A1 保底六点卡`
   - `A2 用户补充层（可选）`
   - `B 技术审计层`
2. 已明确写死：
   - `当前主线`
   - `这轮实际做成了什么`
   - `现在还没做成什么`
   - `当前阶段`
   - `下一步只做什么`
   - `需要用户现在做什么`
   这 6 项必须逐项显式输出、顺序固定、不能合并、不能省略。
3. 已补空项规则：
   - 没有内容也必须写 `无 / 尚未 / 不需要 / 仍待验证`
4. 已补拒收规则：
   - 缺任一项
   - 先贴技术 dump 再补人话
   - 不写 `需要用户现在做什么`
   - 只说做了什么，不说还没做成什么
   这些都直接视为汇报不合格。
5. 已同步到：
   - `AGENTS.md`
   - 当前规范快照
   - 治理正文
   - 典狱长模式正文
   - 相关治理 skill

**恢复点 / 下一步**
- 后续治理线程审用户向回复时，默认先看 `保底六点卡` 是否完整，再看技术审计层。
- 之后如果再有线程继续交“参数 + checkpoint + changed_paths”式回复，就不再只口头提醒，而是直接按“不合格汇报”处理。

## 2026-03-28｜典狱长分发：共享字体止血 owner 已明确回到 spring-day1，允许继续发 prompt

**当前主线目标**
- 用户要求把“共享字体止血”里属于 Day1 owner 的那一半正式交回 `spring-day1`，并明确要求典狱长模式继续分发，不再由治理线程顺手深做。

**本轮裁定**
1. 当前这条不是“停给用户验收”，也不是“停给用户分析 / 审核”。
2. 当前应判为：`继续发 prompt`
3. 原因已经站稳：
   - `DialogueChinese V2 SDF.asset` 属于共享 TMP 动态字体稳定性风险；
   - 但当前相关现场里另外 6 个 Day1-facing 文件是 Day1 正式产品面，不是无关缓存；
   - 这 6 个文件里的 owner-side keep / rework / revert，确实应由 `spring-day1` 线程继续施工。

**本轮已落动作**
1. 新建单条可分发 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-28_典狱长_spring-day1_共享字体止血owner接盘_01.md`
2. Prompt 已固定：
   - 当前唯一主刀 = 只接盘 6 个 Day1-facing 文件
   - 禁止漂移到共享字体底座重建
   - 强制最终回执按：
     - `A1 保底六点卡`
     - `A2 用户补充层（功能点验收 + 停步自省）`
     - `B 技术审计层`

**恢复点 / 下一步**
- 这轮典狱长位已经完成“先审回执、再做继续发 prompt 裁定、再落单切片 prompt”。
- 下一步就是由用户把这份 prompt 转给 `spring-day1` 开工，不再由治理线程代做这 6 个 Day1-facing 文件。

## 2026-03-28｜典狱长审核：spring-day1 共享字体止血 owner 接盘回执已达 checkpoint 预期，但未到收盘态

**当前主线目标**
- 用户要求我审核 `spring-day1` 刚回来的这份 owner 接盘回执，到底有没有完成上一轮 prompt 预期。

**本轮审结论**
1. 结论不是“未达标”，而是：
   - `已达标`
   - 但只达到了 `owner checkpoint`
   - 还没有达到 `收盘完成`
2. 现场核对后确认：
   - 3 个脚本当前 diff 只剩默认字体候选链收窄到 `DialogueChinese SDF`
   - `DialogueFontLibrary_Default.asset` 的 6 个 key 已全部统一到 `DialogueChinese SDF`
   - 两个 Day1 prefab 当前引用也已统一到 `DialogueChinese SDF`
   - 上一轮 prompt 明确要求剥掉的同文件行为续写，当前确实已被回退，没有继续混留在这刀里
3. 同时也确认：
   - `DialogueChinese V2 SDF.asset` 仍留在共享风险线，没有被它这轮顺手吞进 owner 成果
   - `Assets/YYY_Scripts/Story/UI` 同根仍有 5 个 remaining dirty/untracked，故当前不能 claim `own path clean = yes`

**治理裁定**
- 这轮应认定为：
  - `核心预期已命中`
  - `允许继续下一刀`
- 不应认定为：
  - `整条任务已完工`
  - `可直接当最终提交态`

**恢复点 / 下一步**
- 当前下一刀应继续只做：
  - `Assets/YYY_Scripts/Story/UI` 同根 hygiene / own-tail 收口
- 不应回漂到底座线：
  - `DialogueChinese V2 SDF.asset`
  - `DialogueChineseFontAssetCreator.cs`
  - `DialogueChinese*` 整批稳定化

## 2026-03-28｜典狱长纠偏：共享字体止血这刀误发给老 `spring-day1` 后，正式改由 `spring-day1V2` 接棒

**当前主线目标**
- 用户反馈上一条典狱长 prompt 实际发给了老 `spring-day1`，要求我重新判断后续到底该继续跟老线程对话，还是把前情提要与后续工作正式交给 `spring-day1V2`。

**本轮裁定**
1. 当前最稳口径不是继续让老 `spring-day1` 往前滚。
2. 当前也不是让 `spring-day1V2` 重做老线程已经完成的 owner checkpoint。
3. 正确分工应改成：
   - 老 `spring-day1`：这刀到此为止，保留其已产出的 owner checkpoint
   - `spring-day1V2`：继承这个已审 checkpoint，继续做后半刀 same-root hygiene / 收盘

**为什么这样判**
1. 老 `spring-day1` 已经真实完成并交回了“6 个 Day1-facing 文件的字体止血 checkpoint”，且治理审结论为：
   - checkpoint 达标
   - 但尚未到 sync-ready
2. 继续让老 `spring-day1` 往前推，只会把老线程和 `V2` 的 owner 边界重新搅混。
3. 直接让 `V2` 重打一遍，也会制造重复施工和 same-file 并发风险。
4. 因此最稳的接棒方式是：
   - `继承，不重打`
   - `只继续后半刀`

**本轮已落动作**
1. 已新增 `spring-day1V2` 接棒 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-28_典狱长_spring-day1V2_继承v1字体止血checkpoint并接棒_01.md`
2. Prompt 已明确压死：
   - 先读老 `v1` 的 prompt + 线程记忆 + 父工作区记忆
   - 把老 `v1` 的 6 文件 checkpoint 当继承基线
   - 不重做旧刀
   - 当前唯一新增主刀 = `Assets/YYY_Scripts/Story/UI` 同根 hygiene

**恢复点 / 下一步**
- 现在最合理的用户动作是：
  1. 不再给老 `spring-day1` 新任务
  2. 把接棒 prompt 发给 `spring-day1V2`
- 后续治理位只按 `spring-day1V2` 的回执继续裁定，不再双线并行推进这条 Day1 字体止血链。

## 2026-03-28｜审 `spring-day1V2` 最新回复：技术事实并非全错，但对当前委托属于明显跑题

**当前主线目标**
- 用户贴出 `spring-day1V2` 的最新回复，要求我判断“它到底在说什么、为什么会这样回复、这条回复算不算完成当前委托”。

**本轮审结论**
1. 这条回复里有一部分技术事实是成立的，不是纯胡说：
   - `DialogueChineseFontAssetCreator.cs` 当前确实按 `AtlasPopulationMode.Dynamic + isMultiAtlasTexturesEnabled = true` 生成字体；
   - `Editor.log` 里也确实反复出现：
     - `DialogueChinese V2 SDF.asset`
     - `DialogueChinese Pixel SDF.asset`
     - `DialogueChinese BitmapSong SDF.asset`
     - 以及更晚出现的 `DialogueChinese SDF.asset`
     的 `Importer(NativeFormatImporter) generated inconsistent result`
   - `Ellipsis` 缺字也确实存在。
2. 但对当前这轮 `spring-day1V2` 接棒 prompt 来说，这条回复是明显跑题的：
   - Prompt 明确要求它：
     - 继承老 `v1` 已审 checkpoint
     - 不重打 6 文件 owner 裁决
     - 只继续 `Assets/YYY_Scripts/Story/UI` 同根 hygiene
   - 它却回到了“共享动态中文字体底座稳定性分析”这条更大题上。
3. 所以这条回复不能算完成当前委托，准确裁定应是：
   - `技术判断层部分有效`
   - `但任务执行层未命中`
4. 用户看不懂它在说什么的根因也已经清楚：
   - 它回答的是“更大的真实根因是什么”
   - 而不是“我这轮接棒后实际做了什么、还剩什么、下一步为什么只做 same-root hygiene”

**恢复点 / 下一步**
- 现在不该跟着它的回复重新回到底座线讨论；
- 正确动作仍是把 `spring-day1V2` 拉回接棒 prompt 的唯一主刀：
  - 继承 `v1` checkpoint
  - 只做 `Assets/YYY_Scripts/Story/UI` 同根 hygiene
- 这条回复最多只能算“补充背景判断”，不能算当前委托的完成回执。

**本轮治理同步结果**
- 已用 `git-safe-sync.ps1 -Action sync -Mode governance -OwnerThread Codex规则落地` 完成白名单同步。
- 提交 SHA：`1fbf64d5`
- 提交信息：`2026.03.27_Codex规则落地_06`
- 当前这批 live 规则补强已正式进入 `main`。

## 2026-03-28｜复审 `spring-day1V2` 跑题回复：用户向审结论进一步压成人话

**当前主线目标**
- 用户继续追问 `spring-day1V2` 为什么会那样回复，并要求我直接审核这段话到底算什么、问题出在哪。

**本轮稳定结论**
1. 这段回复翻成人话，核心是在说：
   - “现在炸的不是 Day1 剧情逻辑，而是共享 TMP 中文字体资产自己不稳定，导入一次一个样，还会触发 domain reload。”
2. 这层技术判断本身大体成立，所以它不是在胡编。
3. 但它这轮真正错的地方不是技术，而是答题范围：
   - 当前委托要它做的是 `继承 v1 已审 checkpoint + 只继续 Assets/YYY_Scripts/Story/UI 同根 hygiene`
   - 它却把题目重新抬回“共享字体底座到底怎么治”
4. 所以这条回复的准确裁定应当固定为：
   - `背景分析可参考`
   - `当前委托不合格`
   - `不能据此改写当前主刀`
5. 用户之所以会觉得“完全不知道它在说什么”，根因不是用户没看懂技术，而是它没有回答用户当前最需要的 4 件事：
   - 它这轮实际做了什么
   - 还剩什么
   - 当前阶段到哪
   - 下一步为什么只该做 same-root hygiene

**恢复点 / 下一步**
- 这条回复不能作为 `spring-day1V2` 当前接棒任务的完工回执；
- 后续如果继续推进，必须把它硬拉回：
  - `继承，不重打`
  - `只做 same-root hygiene`
  - `按 A1/A2/B 用户可读结构回执`

## 2026-03-28｜已生成 `spring-day1V2` 二次纠偏 prompt：从“背景分析”硬拉回“same-root hygiene 续工”

**当前主线目标**
- 用户要求我直接给出下一条要发给 `spring-day1V2` 的 prompt，并说明为什么要这样写。

**本轮已落动作**
1. 已新增 prompt 文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-28_典狱长_spring-day1V2_拉回same-root-hygiene纠偏续工_02.md`
2. 这条 prompt 明确做了 4 个收紧：
   - 先显式判死上一条长回复：`不是乱说，但不算当前委托完成`
   - 强制把主刀重新锁回：`继承 v1 checkpoint + 只做 Assets/YYY_Scripts/Story/UI 同根 hygiene`
   - 要求它把 5 条 same-root remaining dirty 逐项判成 `own / same-file contamination / foreign`
   - 要求最终回执必须同时带 `A1 保底六点卡 + A2 继承说明/停步自省 + B 技术审计层`

**为什么这样写**
1. 上一条 prompt 其实已经写了“不要重打、只继续后半刀”，但它仍然被更大的共享字体根因吸走，说明这次不是信息不够，而是纠偏力度还不够。
2. 所以下一条不能再只是重复“别漂移”，而必须先把上一条回复的法律地位说死：
   - 可参考
   - 但不算交卷
3. 同时用户现在最需要的是：
   - 它这轮实际新增做了什么
   - 还剩什么
   - same-root 到底能不能收
   - 为什么下一步还是这一刀
4. 因此这条 prompt 没有让它少思考，反而是把思考位置摆正：
   - 不是继续想“更大的根因”
   - 而是先思考“我这轮到底该交什么、为什么只该交这个”

**恢复点 / 下一步**
- 现在可以直接把这条新 prompt 发给 `spring-day1V2`；
- 后续再审时，只看它是否真的命中 same-root hygiene 与用户可读回执，不再接受“共享字体底座分析”顶替当前委托。

## 2026-03-28｜复审 `spring-day1V2` same-root hygiene 回执：已从跑题进入合格 B，但还差最终 blocker 停表

**当前主线目标**
- 用户贴出 `spring-day1V2` 第二次 same-root hygiene 回执，要求我一步到位完成详细审核、判断和下一条 prompt。

**本轮审结论**
1. 这次回执已经第一次真正命中当前委托主刀：
   - 继承了老 `v1` 已审 checkpoint
   - 没再回到底座线
   - 对 prompt 指定的 5 个 same-root 项做了判类
2. 所以这次不能再判它“答非所问”；当前应判为 `合格 B`。
3. 但这条线仍未收盘，还差两个精度缺口：
   - preflight 命令口径不精确；`task` 模式下若无 `ScopeRoots / IncludePaths`，launcher 会先因“无边界自动提交被禁止”拦下；
   - 最终 blocker stop-list 尚未补全；当前现场还应继续纳入：
     - `Assets/YYY_Scripts/Story/UI/SpringDay1UiPrefabRegistry.cs(.meta)`
     - `Assets/Resources/Story/SpringDay1UiPrefabRegistry.asset(.meta)`
     - `.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/memory.md`
4. 因此下一条 prompt 不应再写成“继续做一点 hygiene”，而应升级成：
   - `最终 blocker 矩阵`
   - `准确 preflight 口径`
   - `收盘还是停表`

**本轮已落动作**
1. 已新增 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-28_典狱长_spring-day1V2_字体止血链最终blocker矩阵与收盘裁定_03.md`
2. 这条 prompt 的唯一主刀是：
   - 把这条字体止血链到底还能不能在它这里收盘，一次裁死

**恢复点 / 下一步**
- 这条线当前仍属于 `继续发 prompt`；
- 但下一轮必须是“最终 blocker 停表 prompt”，不再允许模糊续工。

## 2026-03-28｜复审 `spring-day1V2` 最新 04 完整回执：这条线应正式停发，后续改走三拆

**当前主线目标**
- 用户贴来 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1V2\2026-03-28_给典狱长的最新完整回执_04.md`，要求我基于最新并行事实判断：
  - `spring-day1V2` 是否应正式停发
  - 后续是否拆成 `NPC foreign 剥离 / UI-V1 registry 链路归并 / 文档 blocker 拆账`

**本轮审结论**
1. `04` 已经真正补齐 `03` 委托要求的收盘条件：
   - 准确 `preflight` 边界已给出
   - 第一真实阻断原因已钉死
   - 最终 blocker 矩阵与 stop-list 已补全
   - `spring-day1V2` 的唯一裁定保持为 `B｜最终停表`
2. 这条线现在应正式停发，不再继续给 `spring-day1V2` 发“再做一点 hygiene / 字体止血链续工”的 prompt。
3. 这里的“停发”不是因为它已经 `sync-ready`，而是因为：
   - `当前 own 路径是否 clean = no`
   - 但剩余阻断已不再属于它的 `own current-slice`
   - 继续让它往下磨，只会继续把字体止血、UI 正式施工、NPC foreign 与文档账本混成一团
4. `UI-V1 / SpringUI` 已经足够站稳为 Day1 UI 正式 owner：
   - `SpringDay1PromptOverlay.cs` 与 `SpringDay1WorkbenchCraftingOverlay.cs` 当前都显式调用 `SpringDay1UiPrefabRegistry.Load*Prefab()`
   - `SpringUI` 工作区 memory 已把 `SpringDay1UiPrefabRegistry.cs` 和 `Assets/Resources/Story/SpringDay1UiPrefabRegistry.asset` 记入 `Phase 1 prefab-first` 主链
5. `NpcWorldHintBubble.cs(.meta)` 继续判为 `foreign` 的依据仍成立：
   - NPC 工作区 memory 明确把它记为 `001 / 002 / 003` 共用的 NPC 线文件
6. 因此后续最合理的不是继续单线自转，而是改走三拆：
   - `NPC foreign 剥离`
   - `UI-V1 / SpringUI registry 链路归并`
   - `003-进一步搭建/memory.md` 文档 blocker 拆账

**路由细化**
1. `NpcWorldHintBubble.cs(.meta)`
   - 继续按 `foreign` 路由给 NPC 线，不回投 `spring-day1V2`
2. `SpringDay1UiPrefabRegistry.cs(.meta/.asset/.meta)`
   - 对 `spring-day1V2` 来说仍是 `other-slice contamination`
   - 但对治理位下一步路由，应直接视为 `UI-V1 / SpringUI` 活跃主链，或在 UI 线当前处于用户终验窗口时交给 integrator 收盘
3. `.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/memory.md`
   - 独立作为文档 / 治理 blocker 处理，不再和字体止血或 UI owner 混成一刀

**恢复点 / 下一步**
- `spring-day1V2` 这条线当前应从典狱长模式下的“继续发 prompt”改判为：
  - `无需继续发`
- 后续治理若继续推进 Day1，只应围绕三拆对象发新 prompt，而不是再对 `spring-day1V2` 本体续工。

**补充校验**
- 我在这轮审结论后，直接按 `04` 里给出的 `IncludePaths` 重新跑了 stable launcher 的 `preflight`。
- 复核结果没有推翻“正式停发 + 三拆”，但新增暴露出一个比 `04` 更晚的 same-root 现场项：
  - `Assets/222_Prefabs/UI/Spring-day1/Primary.unity`
  - `Assets/222_Prefabs/UI/Spring-day1/Primary.unity.meta`
- 因此现在最准确的话不该再说成“`04` 已把 stop-list 永久补全”，而应说：
  - `04` 的停发表述方向成立
  - 但 live 现场又新长出一个 UI / scene contamination 子项
  - 它不属于 `spring-day1V2` 该继续自转解决的理由，反而更强化了“别再给它续工”的裁定

## 2026-03-28｜按用户要求完成 Sunset 仓库总盘点：当前不宜一锅提交，但也不是整体失控

**当前主线目标**
- 用户要求我不要被“几百个更改、二十多万增删”这种表象带节奏，而是实际调研工作区 memory、线程 memory 与真实 git 现场，判断当前是否适合直接整体提交。

**本轮盘点事实**
1. 当前 `git status` 可见条目约 `178`：
   - `modified = 75`
   - `deleted = 53`
   - `untracked = 50`
2. 当前 `git diff --stat`（只算已跟踪差异）为：
   - `128 files changed`
   - `+32772 / -101123`
3. 当前体量被 3 类东西显著放大：
   - `Assets/000_Scenes/Primary.unity` 删除单文件就占 `-85162`
   - `UI系统` 旧目录批量删除，同时 `Before03.28` / `0.0.1 SpringUI` 新目录仍未跟踪，形成迁移型大体量
   - `DialogueChinese*` 字体资产本身是大 YAML 资源，单文件 diff 就是几千行级
4. 当前活跃线程 / 工作区 memory 指向的并行主线至少有：
   - `UI / Day1`
   - `NPC`
   - `导航`
   - `农田 / 背包 / Tooltip`
   - `治理 / 文档 / 记忆`
5. 所以现在不是“一坨内容全都在无序互踩”，而是“多条主线并行 + 文档迁移与大资源把数字抬很高”。

**最值得警惕的真实风险**
1. `Assets/000_Scenes/Primary.unity`
   - 当前不是普通修改，而是老路径真实消失
   - 同时在 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 冒出一个未跟踪同名 scene
   - 这是当前最危险的异常面
2. `DialogueChinese*` 字体资产
   - 仍属于高体量、高不确定性的共享资源 diff
3. `GameInputManager.cs`
   - 仍是共享热点文件，不适合无触点对账就混进整仓提交

**当前治理建议**
1. 现在不建议直接“一锅端提交”
2. 但也不需要把整个仓库理解成“已经失控到不能动”
3. 更合理的顺序是：
   - 先核清 `Primary.unity` 异常迁移面
   - 再把 `UI系统` 目录迁移 / 归档和业务代码提交拆开
   - 再按 `UI-Day1 / NPC / 导航 / 农田-背包 / 治理文档` 各自切 checkpoint

**恢复点 / 下一步**
- 如果后续继续推进仓库收口，当前最值钱的一步不是提交，而是先把 `Primary.unity` 删除与新位置同名 scene 的关系判清。

## 2026-03-28｜继续只读深挖仓库总审：`Primary.unity` 不是“小 UI scene 误放”，而是 owner 不明的高风险迁移异常

- 当前主线目标：
  - 用户要求继续做“只读全盘清扫”，不要改代码、不要提交，而是把仓库现场再钉细一层，尤其要判断 `Primary.unity` 异常迁移和 `untracked` 孤儿项到底是什么结构。
- 本轮新增核到的硬事实：
  1. `Assets/000_Scenes/Primary.unity` 与 `.meta` 当前仍是 tracked delete，而 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity(.meta)` 仍是 untracked。
  2. 新旧 `Primary.unity.meta` 的 GUID 完全相同，都是：
     - `a84e2b409be801a498002965a6093c05`
  3. `ProjectSettings/EditorBuildSettings.asset` 仍只指向旧路径：
     - `Assets/000_Scenes/Primary.unity`
  4. 新 scene 不是一个“小 UI 专用 scene 壳”：
     - 行数仍有 `83705`
     - 且仍直接包含 `001_HomeAnchor / 002_HomeAnchor / 003_HomeAnchor`
     - `SaveManager`
     - `PersistentManagers`
     - `NavGrid2DStressTest`
  5. 目前在工作区 memory / 线程 memory 里，除了我这条治理审计记忆外，没有找到任何线程明确解释“为什么 `Primary.unity` 会出现在 `Assets/222_Prefabs/UI/Spring-day1/`”。
- 这轮站稳的判断：
  1. `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 不能被理解成“Day1/UI 合理生成的一个独立子 scene”。
  2. 当前更准确的口径是：
     - 旧主场景路径被删
     - 同 GUID scene 出现在错误目录
     - 内容又不是与旧 `HEAD` 完全一致
     - 且没有线程正式认领
  3. 所以它仍是当前仓库里最危险的 ownerless mixed 异常面，不适合混进任何业务 checkpoint，更不适合整仓一锅提交。
- 对 untracked 的进一步裁定：
  1. 大部分 untracked 并不是孤儿：
     - `UI系统/0.0.1 SpringUI`、`Before03.28`
     - `NPC/0.0.2清盘002`
     - `场景搭建（外包）` 线程文档
     - `SpringDay1UiPrefabRegistry.cs/.asset`
     - `PlayerNpcChatSessionService.cs`
     - `NPCInformalChatInteractable.cs`
     - `NpcWorldHintBubble.cs`
     - `NPCInformalChatInterruptMatrixTests.cs`
     - `SpringDay1LateDayRuntimeTests.cs`
     这些都能在对应工作区或线程 memory 里找到活跃主线解释。
  2. 但有 3 类 under-documented 项需要单独盯：
     - `Assets/222_Prefabs/UI/Spring-day1/Primary.unity(.meta)`：高风险、owner 不明、不能按 UI 正常副产物处理
     - `Assets/Screenshots/*`：当前命名明显是 `spring-day1` / UI 现场截图，但没有对应 memory 解释，而且与新写下的 `.codex/artifacts/ui-captures/` 证据口径冲突
     - `NPCInformalChatExitModel.cs`：代码与测试已真实使用，但当前工作区记忆里还没显式点名，属于文档补记滞后，不是纯孤儿
  3. `Assets/Editor/NPC.meta` 目前更像低风险目录副产物：
     - 同目录下已有 `PlayerNpcRelationshipDebugMenu.cs`
     - 且该调试菜单已在 `NPCV2` memory 里出现
- 当前恢复点：
  - 这轮总审已经从“不要一锅提交”推进到“哪些东西真的危险、哪些东西只是有主线但还没收口”。
  - 若继续只读推进，最值钱的下一步不再是泛看数字，而是：
    1. 先把 `Primary.unity` 异常 scene 的 owner / 去留判死
    2. 再把 `Assets/Screenshots/*` 按“仓库运行时资产 vs UI 证据泄漏”单独裁定
    3. 之后才谈各主线 checkpoint 切分

## 2026-03-29｜治理基础设施补一刀：`next-skill-trigger-id.ps1` 已从“规则先行、执行断链”推进到可运行

- 当前主线目标：
  - 用户要求我继续把全局治理支线里还没做完的内容往前推，重点是优化真实执行质量，而不是继续堆规则文本。
- 本轮子任务：
  - 修复 `C:\Users\aTo\.codex\tools\next-skill-trigger-id.ps1`；
  - 验证 `skill-trigger-log` 活跃卷的当前健康度；
  - 收紧 live 文案里仍会把 helper 说成“只扫描最大值 + 1”的残留表述。
- 本轮已站稳的事实：
  1. helper 之前的真实坏点是 PowerShell 语法层断裂：`foreach` / `ForEach-Object` 块缺少闭括号，导致 parser error。
  2. 修完后，`powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\next-skill-trigger-id.ps1` 已成功返回：
     - `STL-20260329-002`
  3. 原子保留状态文件已经真实落出：
     - `C:\Users\aTo\.codex\memories\skill-trigger-id-state.txt`
     - 当前内容：`20260329=2`
  4. `C:\Users\aTo\.codex\memories\skill-trigger-log.md` 活跃卷已确认没有重复 `STL-ID`。
  5. `sunset-ui-evidence-capture` 仍然挂在正式入口链上，不是“只有 skill 文件但没接线”的假落地。
- 当前判断：
  - 这轮真正被拔掉的是执行层 blocker，因为现在不是只“规定要原子分配”，而是 helper 已经能返回并保留编号。
  - 审计层当前更准确的状态是：
    - 活跃链已止血
    - 历史 raw 归档卷仍保留旧重号
  - 所以不能把现状吹成“全部历史卷都已 canonical clean”，但也不再是“规则都写了、工具却不能跑”。
- 当前恢复点：
  - 如果后续继续这条治理支线，最自然的下一步不是新增规则，而是决定是否把 `legacy_pre / pre_cleanup` raw 归档与 canonical 审计口径进一步分层。

## 2026-03-29｜治理支线再推进一刀：`skill-trigger-log` 已完成 raw / canonical 分层

- 当前主线目标：
  - 用户明确要求我继续执行上一轮说好的下一步，只做“把历史 raw 归档卷和 canonical 审计口径分层”，不再扩别的治理话题。
- 本轮子任务：
  - 调整 `next-skill-trigger-id.ps1` 的扫描范围；
  - 把全局规则文案统一改成 canonical 口径；
  - 明确 grandfathered raw 历史卷的边界。
- 本轮已站稳的事实：
  1. helper 现在只扫描 canonical 审计卷：
     - 活跃卷 `skill-trigger-log.md`
     - 未来 canonical 历史卷 `skill-trigger-log_canonical_*.md`
  2. grandfathered raw 历史卷已正式降为“只保留原始历史证据”：
     - `skill-trigger-log_legacy_pre_20260325.md`
     - `skill-trigger-log_20260325_to_20260328_pre_cleanup.md`
  3. 这条分层已同步进：
     - `C:\Users\aTo\.codex\AGENTS.md`
     - `C:\Users\aTo\.codex\memories\global-learning-system.md`
     - `C:\Users\aTo\.codex\skills\skills-governor\SKILL.md`
     - `C:\Users\aTo\.codex\memories\skill-trigger-log.md`
     - 两个 grandfathered raw 历史卷头部说明
  4. helper 实测仍可运行，当前已返回：
     - `STL-20260329-014`
  5. 分层后的口径差异已经钉死：
     - 若把所有匹配 `skill-trigger-log*.md` 的卷一起混扫，重复组为 `84`
     - 若只按 canonical 审计卷统计，重复组为 `3`
- 当前判断：
  - 这刀已经把“历史 raw 噪音一直污染当前判断”的问题收窄了。
  - 现在剩下的 duplicate 已不再是 old raw 噪音，而是当前 canonical 面自己的真实问题。
  - 所以这轮之后，治理判断应该从“历史和当前混成一团”改成“raw 已分层，当前 canonical 还剩 3 组活跃重号待清”。
- 当前恢复点：
  - 如果后续继续这条治理支线，下一步最自然的就不再是“继续谈 raw 分层”，而是只处理当前 canonical 活跃卷自己的 3 组重号。

## 2026-03-29｜canonical 活跃卷重号已清零：实际不是 3 组，而是顺藤摸出了 5 组

- 当前主线目标：
  - 用户要求我继续自己的治理主线，把当前 canonical 活跃卷自己的重号清掉，不再回头谈 raw 分层。
- 本轮子任务：
  - 只清 `C:\Users\aTo\.codex\memories\skill-trigger-log.md` 的 canonical 重号；
  - 保留先出现的原号，把后出现的晚条目重编号到新的预留号位；
  - 不改 raw 历史卷，不扩别的治理主题。
- 本轮已站稳的事实：
  1. 原先按第一层扫描看见的是 `3` 组重号：
     - `007`
     - `009`
     - `010`
  2. 实际继续往下清时，又顺藤摸出了更深一层的 `2` 组：
     - `011`
     - `018`
  3. 这轮最终共清了 `5` 组重号，并把后写入的晚条目重编号为：
     - `007 -> 019`
     - `009 -> 020`
     - `010 -> 021`
     - `011 -> 022`
     - `018 -> 023`
  4. helper 为这次 canonicalization 真实预留过：
     - `019`
     - `020`
     - `021`
     - `022`
     - `023`
  5. 当前 canonical 审计卷复核结果已经变成：
     - `CANONICAL_DUP_GROUPS=0`
- 当前判断：
  - 这条治理主线已经从“raw / canonical 分层”推进到了“canonical 活跃卷自身 clean”。
  - 也就是说，这轮之后 `skill-trigger-log` 当前 canonical 面已经不再被历史 raw 噪音拖，也不再被活跃卷重号拖。
- 当前恢复点：
  - 如果后续还要继续这条治理线，下一步就不该再围着 duplicate cleanup 打转，而应转去看更高一层的问题，例如会话级显式 skill 暴露一致性，或后续新增记录是否会再次撞号。

## 2026-03-29｜审计闭环补齐：`check-skill-trigger-log-health.ps1` 已接到收尾口径

- 当前主线目标：
  - 继续这条全局治理支线，但这轮不再新增规则，只把 canonical 活跃卷清零后的“防再发”真正接成执行闭环。
- 本轮子任务：
  - 复核 `check-skill-trigger-log-health.ps1` 的当前输出；
  - 用新的预留 `STL-ID` 补齐这轮审计落盘；
  - 把“追加后立刻验 canonical health”正式补进工作区记忆和线程记忆。
- 本轮已站稳的事实：
  1. `C:\Users\aTo\.codex\tools\check-skill-trigger-log-health.ps1` 已在盘上且可运行，不是只停在规则文本里。
  2. 本轮先预留到了 `STL-20260329-028`，但离场复核时又发现另一条活跃记录已经占用了 `028`；因此按“保留先出现原号、晚条目改到新预留号位”的 canonical 口径，把这轮条目最终收到了：
     - `STL-20260329-029`
  3. 当前状态文件也已同步推进到：
     - `C:\Users\aTo\.codex\memories\skill-trigger-id-state.txt`
     - 当前值：`20260329=29`
  4. health helper 当前输出已经明确：
     - `Health: ok`
     - `Canonical-Duplicate-Groups: 0`
     - `All-Matching-Duplicate-Groups: 82`
     - `Raw-Only-File-Count: 2`
  5. 这说明当前 canonical 面已经把这次新冒出的 `028` 撞号也收回到了 clean；`82` 组重复仍然只是 grandfathered raw 历史卷旧账，不应再被误判成当前活跃卷回退。
- 当前判断：
  - 这轮真正往前推的一步，不是继续清 duplicate，也不是再发明一层治理口号，而是把“追加后必须自验 canonical clean”这件事补成了执行闭环。
  - 到这里为止，这条治理主线已经从“把活跃卷清干净”推进到“离场前必须再证明确实没被自己写脏”。
- 当前恢复点：
  - 如果后续还要继续这条治理线，下一步不该再写新规则，而应只抽检后续真实新增记录是否都走了：
    1. 先原子取号
    2. 再追加活跃卷
    3. 最后立刻跑 health helper

## 2026-03-29｜典狱长续发 `全局skills`：只证明 `preference-preflight-gate` 的自动前置半步

- 当前主线目标：
  - 用户要求我主线优先，不再继续 `skill-trigger-log` 支线施工，而是直接给 `全局skills` 线程发下一轮 prompt。
- 本轮子任务：
  - 先按最新本地事实重判这条线是否还该继续发 prompt；
  - 再把下一轮唯一主刀压成单刀文件，不让它回漂去讲旧 B、旧 canonical 争论或继续堆偏好材料。
- 本轮已站稳的事实：
  1. 当前 canonical 审计面仍是 clean：
     - `Canonical-Duplicate-Groups = 0`
  2. `preference-preflight-gate` 在我当前这条 fresh session 里已经显式暴露，所以“fresh session 会不会显式暴露它”不再是未知。
  3. 当前唯一还没证明的，只剩 `A` 的后一半：
     - 面对真实 UI / 场景 / 体验判断任务时，它会不会不靠手工 helper 自动前置命中。
  4. 我已把下一轮 prompt 正文落到：
     - `D:\迅雷下载\开始\.codex\threads\系统全局\全局skills\2026-03-29_preference-preflight-gate_auto-frontload-proof_01.md`
- 当前判断：
  - 这条线还值得继续，但只值得再做这一刀。
  - 继续发 prompt 是合理的，因为主线还差最后半步证据；不合理的是让它再回去补 profile、补 learning、补 helper 或回讲旧 B。
- 支线代办：
  - `skill-trigger-log` health helper 这条线先停在代办，只保留后续抽检新增记录的健康度，不再继续扩主题。
- 当前恢复点：
  - 如果用户转发这份 prompt，`全局skills` 下一轮就只该回答：
    1. 当前 fresh session 是否显式暴露
    2. 真实任务是否自动前置命中
    3. 因此现在到底还是 `B`，还是能升到 `A`

## 2026-03-29｜执行层抽检完成：强回执已出现，但默认输出还没天然切到用户模式

- 当前主线目标：
  - 用户让我继续我的治理主线；这轮不新增规则，只抽检最近真实线程产出，判断“用户可读汇报 + 停步自省”到底落到什么程度。
- 本轮子任务：
  - 深审 4 条近期样本：
    1. `spring-day1V2` 正式回执 04
    2. UI 线程 `2026-03-29_进度快照.md`
    3. `导航检查V2` 最近 `memory_0.md` 条目
    4. `NPCV2` 最近 `memory_0.md` 条目
  - 并把结果收成一份不加规则的执行层抽检文档。
- 本轮已站稳的事实：
  1. `spring-day1V2` 的 `04` 是当前最强落地样本：
     - `A1 保底六点卡`
     - `A2 用户补充层`
     - `停步自省`
     - `B 技术审计层`
     都已经出现，而且顺序正确。
  2. UI 线程 `2026-03-29_进度快照.md` 是强内部快照：
     - 主线、支线、优先级、恢复顺序都很清楚
     - 但它不是用户回执，不能拿它冒充“已经对用户说明白”
  3. 导航与 NPC 最近 memory 都显示：
     - 工程 continuity 已明显变强
     - 但如果直接端给用户，仍会回到“技术层偏重、自评与用户动作缺失”的老问题
  4. 本轮抽检结论已落盘到：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-29_执行层落地抽检_01.md`
- 当前判断：
  - 现在已经不是“规范完全没落地”
  - 更准确的现实是：
    - 正式回执层开始像样
    - 内部快照层已经很强
    - 但线程还没有稳定做到“只要面对用户，就自动切到用户回执模式”
  - 当前最大的执行层问题不再是规则缺位，而是：
    - channel mismatch
    - 自评仍然稀缺
- 当前恢复点：
  - 如果继续这条治理主线，下一步不该加规则；
  - 只该继续抽检真实样本，盯两件事：
    1. 面向用户时，线程会不会真的切到正式回执模式
    2. 自评 / 薄弱点 / 最可能看错处，是否开始变成常规输出

## 2026-03-29｜审核 `全局skills` 的 A 回执：当前不予放行，原因是 proof 被 prompt 本身污染

- 当前主线目标：
  - 用户把 `全局skills` 新回执贴回来了，要求我审核并给出下一步 prompt。
- 本轮子任务：
  - 不是再争论结论，而是核这次 `A` 证明的方法本身是否成立；
  - 如果不成立，就把不通过原因讲死，并改发一份不污染证据的 blind proof 纠偏 prompt。
- 本轮已站稳的事实：
  1. `全局skills` 这次 claim `A` 的核心证据，来自：
     - `D:\迅雷下载\开始\.codex\threads\系统全局\全局skills\2026-03-29_preference-preflight-gate_auto-frontload-proof_01.md`
     - 以及它后续在 `memory_0.md` / `skill-trigger-log.md` 里的自述
  2. 但旧 proof 文件本身在最前面就要求先读：
     - `C:\Users\aTo\.codex\skills\preference-preflight-gate\SKILL.md`
  3. 同一个 proof 文件后面又要求去证明：
     - “面对真实任务时，它会不会不靠手工 helper 自动前置命中”
  4. 这会直接污染因果链：
     - 因为 session 已被 skill 名称、skill 正文和 proof 目标预先 priming
     - 后面再说“我自动做出了这些前置动作”，已经无法区分到底是自动触发，还是照 proof 委托执行
  5. 我已据此生成新的纠偏 prompt：
     - `D:\迅雷下载\开始\.codex\threads\系统全局\全局skills\2026-03-29_preference-preflight-gate_blind-proof_retry_02.md`
- 当前判断：
  - 这次 `A` 结论当前不予放行。
  - 不是因为它完全没价值，而是因为 proof 设计本身已经污染了证据，所以只能回退成：
    - `B`
    - 或更细口径下的 `B+（显式暴露已有，但自动命中的干净证明仍缺）`
  - 下一步也不该继续争辩“这次其实算不算 A”，而应改做真正不污染证据的 blind proof。
- 当前恢复点：
  - 如果用户转发新的 blind proof prompt，`全局skills` 下一轮只该回答：
    1. 为什么这次 proof 不能算干净的 `A`
    2. 现在最诚实等级是什么
    3. 真正能证明 `A` 的下一次实验应怎样设计

## 2026-03-29｜假合规案例已沉淀：这次不是“内容不够”，而是“证明方法已坏”

- 当前主线目标：
  - 继续治理主线，但这轮不扩规则，只把刚抓到的 `A` 误判收成一份可复用的假合规案例。
- 本轮子任务：
  - 不是重复说“这次不通过”，而是把“为什么它看起来很像成功、实际上却不能放行”写成一套可复用的识别法。
- 本轮已站稳的事实：
  1. 这次问题的危险点，不在输出太差，而在：
     - 输出很完整
     - 术语都对
     - 审计层也像回事
     - 但 proof 文件本身已经预埋 skill 与 proof 目标
  2. 我已把这次案例收成：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-29_执行层假合规案例_01.md`
  3. 这份案例已经明确整理出 4 个后续复用检查点：
     - proof 文件是否先点名 skill
     - 任务是否自然来活
     - 当前 session 是否已被 priming
     - 回执是在证明结果，还是在证明方法
- 当前判断：
  - 这一步的价值不在于再打一条规则，而在于把“看起来像合规、其实证据链已坏”的模式钉出来。
  - 这会直接提高后续审回执的效率，因为以后遇到类似 `A` claim，不需要再从头猜它哪里怪。
- 当前恢复点：
  - 如果继续治理主线，下一步仍然不该加规则；
  - 只该继续积累这类高价值的真实审计案例，尤其是：
    - 假合规
    - channel mismatch
    - 自评缺席

## 2026-03-29｜`全局skills` 最新回执纠偏成立，但不允许停在“等 fresh session”

- 当前主线目标：
  - 用户指出 `全局skills` 又停住了，要我审这份最新回执，并直接给出能继续开工的 prompt。
- 本轮子任务：
  - 只审两件事：
    1. 这份最新回执是否已经达到“可以停发”的程度；
    2. 如果不能停，下一刀到底该让它做什么。
- 本轮已站稳的事实：
  1. 这份回执在纠偏判断上是成立的：
     - 它已经老实撤回旧 `A`；
     - 也已经把旧 proof 被 prompt / skill 名称 / proof 目标污染的原因拆清。
  2. 但它当前停表方式不通过：
     - 它把结论停在“需要用户提供 fresh session / 新入口”；
     - 这等于只完成了判错，没有把主线继续推进。
  3. 这条线现在真正还能继续做的一刀，不是再争 `A / B`，而是把“下一次真正能证明 A 的 blind proof”准备成可直接执行的资产包。
  4. 对应的续工 prompt 已经有可用正文：
     - `D:\迅雷下载\开始\.codex\threads\系统全局\全局skills\2026-03-29_preference-preflight-gate_blind-proof-package_03.md`
- 当前判断：
  - 这份回执应判为：
    - `继续发 prompt`
  - 不应判成：
    - `停给用户分析`
    - 也不应继续让它停在“等条件成熟”
  - 原因很简单：
    - 它已经完成“为什么这次不能算 A”的纠偏；
    - 但还没完成“把下一次干净实验准备到可直接开跑”。
- 当前恢复点：
  - 如果继续这条治理主线，唯一正确下一步就是把 `blind-proof-package_03` 发给 `全局skills`；
  - 让它这轮只交 3 个执行资产：
    1. `blind-proof_raw_task_seed_01.md`
    2. `blind-proof_governance_rubric_01.md`
    3. `blind-proof_execution_protocol_01.md`
  - 在这 3 个文件落出来前，这条线不算真的恢复推进。

## 2026-03-29｜当前治理主线的未完项盘点：除等 `全局skills` 回执外，无新的必须立刻执行项

- 当前主线目标：
  - 用户追问：除了等待 `全局skills` 的新回执，这边还有没有别的未完成内容。
- 本轮子任务：
  - 只做当前主线的收口盘点，不扩新规则，也不切去别的支线。
- 已完成判断：
  1. 当前主线上的必须动作已经做完：
     - 最新回执已审；
     - 下一条续工 prompt 已给；
     - 项目记忆、线程记忆、审计日志也已补齐。
  2. 因此就这条 `preference-preflight-gate / blind proof` 主线来说，当前没有我这边还欠着但没做的立即执行项。
  3. 现在剩下的真正下一步，已经完全落到对方回执：
     - 等 `全局skills` 交出 blind proof 三件套；
     - 我再回来审它是否真的把主线从“等条件”推进成“可直接开跑”。
  4. 仍然存在的东西只是 parked backlog，不属于这条主线当前必须继续做的内容：
     - 后续继续做执行层样本抽检；
     - 后续继续积累假合规 / channel mismatch 案例；
     - Codex 本地双源线程标题不同步的系统性病灶。
- 当前判断：
  - 这轮最诚实结论是：
    - `当前主线已收口到等待回执`
    - `无新的必须立刻执行项`
- 当前恢复点：
  - 现在不该为了“看起来在动”而再补别的治理动作；
  - 正确恢复点就是等 `全局skills` 回来，再按新口径审它的 blind proof 执行包。

## 2026-03-29｜已切回 Sunset 原主线，并落地“全局警匪定责清扫”第一轮摸底与认定书分发

- 当前主线目标：
  - 用户要求回到 Sunset 最初主线，不再围着 `全局skills` 的 blind proof 支线停留；
  - 先对当前整个 shared root 的新内容做一次全局警匪定责清扫；
  - 第一轮先只读摸底并发出认定书；
  - 第二轮再正式定责、分配清扫、处理剩余项。
- 本轮子任务：
  - 不直接拍最终责任；
  - 只完成第一轮治理资产：
    - 摸底报告
    - 批次分发入口
    - 各线程认定书
- 本轮已完成事项：
  1. 重新只读摸底了当前 Sunset 现场：
     - `git status --short`
     - `git diff --stat`
     - 活跃线程 `memory_0.md`
     - 关键工作区 `memory.md`
     - 关键归属 `rg` 搜索
  2. 已落盘治理总报告：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-29_全局警匪定责清扫摸底_01.md`
  3. 已落盘第一轮批次分发入口：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-29_批次分发_13_全局警匪定责清扫第一轮_01.md`
  4. 已给以下 8 条活跃线程各落一份第一轮认定书：
     - `spring-day1`
     - `场景搭建（外包）`
     - `spring-day1V2`
     - `NPCV2`
     - `导航检查V2`
     - `导航检查`
     - `农田交互修复V3`
     - `项目文档总览`
  5. 已明确把 `NPC`、`Skills和MCP`、`遮挡检查` 暂时排除在第一轮单独发书集合之外：
     - 先并入 successor / 主责任线核对；
     - 若第一轮回执仍显示它们有独立 repo 责任，再第二轮补发。
- 本轮站稳的关键判断：
  1. 当前 working tree 不是“几条线程各自留点小脏改”，而是明显进入 `mixed incident` 状态。
  2. `Assets/000_Scenes/Primary.unity -> Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 这组 delete / untracked 不能直接拍给单一线程；
     - 更像 same-GUID 主场景搬家或现场分裂事故面。
  3. UI 侧的 `旧目录删除 + 0.0.1 SpringUI / Before03.28 新目录建立` 说明父线程和实施线程之间至少发生过一次真实迁移，必须各自举证。
  4. 第一轮最值钱的推进不是继续改代码，而是先逼线程把：
     - 聊天叙事
     - memory / 工作区文档
     - working tree 真实路径
     重新对齐。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-29_全局警匪定责清扫摸底_01.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-29_批次分发_13_全局警匪定责清扫第一轮_01.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\2026-03-29_全局警匪定责清扫第一轮认定书_01.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\场景搭建（外包）\2026-03-29_全局警匪定责清扫第一轮认定书_01.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1V2\2026-03-29_全局警匪定责清扫第一轮认定书_01.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\2026-03-29_全局警匪定责清扫第一轮认定书_01.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\2026-03-29_全局警匪定责清扫第一轮认定书_01.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\2026-03-29_全局警匪定责清扫第一轮认定书_01.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-03-29_全局警匪定责清扫第一轮认定书_01.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\项目文档总览\2026-03-29_全局警匪定责清扫第一轮认定书_01.md`
- 验证结果：
  - 已回读摸底报告与批次分发文件，结构正确、路径可用；
  - `git status --short` 已确认新增治理文件全部进入 working tree；
  - 本轮未继续业务实现、未触碰 runtime 代码。
- 当前恢复点：
  - 当前治理主线已从“只读摸底”推进到“第一轮认定书已就位”；
  - 下一步不再自行扩题；
  - 只等各线程按这批认定书回第一轮自查，再进入第二轮正式定责与清扫分配。

## 2026-03-29｜第一轮补丁：已为用户侧 `UI-V1` 聊天线程补发自查认定书，不推翻已发批次

- 当前主线目标：
  - 用户补充现场事实：前一轮 prompt 里，只有 `spring-day1` 这条没有实际发出；
  - 且用户当前手上的实施线程标题不是 `spring-day1`，而是 `UI-V1`；
  - 因此需要补一份能直接发给 `UI-V1` 的自查 prompt，并判断第一轮整体是否需要重排。
- 本轮子任务：
  - 不重发整个第一轮；
  - 只补 `UI-V1` 这一条缺口；
  - 同时把 `UI-V1` 与 repo 内真实实施线之间的映射关系重新说清。
- 本轮已完成事项：
  1. 已重新核实当前 repo 里的真实映射：
     - 没有独立名为 `UI-V1` 的线程目录；
     - Day1 UI 的实施线仍落在
       - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1`
       - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI`
     - `场景搭建（外包）` 仍是父线程 / 审核线，而不是实施线本体。
  2. 已补发专供用户当前聊天线程标题使用的认定书：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\2026-03-29_UI-V1_全局警匪定责清扫第一轮认定书_01.md`
  3. 这份补发认定书已显式写死：
     - `UI-V1 = spring-day1 / SpringUI`
     - 本轮不是继续实现，而是替这条实施线做 owner / foreign / mixed / stale narrative 的第一轮自查。
- 当前关键判断：
  1. 第一轮整体不需要重排；
     - 已发出去的其他线程 prompt 仍然成立。
  2. 当前只需要把 `UI-V1` 当成“用户侧线程标题”，而不是再额外创造一条新的 repo 责任线。
  3. 因此治理上的正确补法是：
     - 保留 `场景搭建（外包）` 作为已发出的 UI 父线程自查；
     - 再补 `UI-V1` 这一条实施线自查；
     - 不撤回、不改写其他第一轮线程入口。
- 当前恢复点：
  - 第一轮现在不是“漏了一个大块”，而是“已补齐 UI 实施线缺口”；
  - 下一步继续等第一轮回执，不额外扩发新线程。

## 2026-03-29｜第一轮回执已审，第二轮只放行 4 条清扫线，其余停发或继续等 `UI-V1`

- 当前主线目标：
  - 用户要求我先看一轮当前已回来的“全局警匪定责清扫”回执；
  - 判断当前这一轮是否已经可以给率先完成的线程下达下一步指令；
  - 要求我不能只听线程自报，还要结合 working tree 和 memory 现场做典狱长裁定。
- 本轮子任务：
  - 回读已回执线程：
    - `项目文档总览`
    - `导航检查`
    - `导航检查V2`
    - `农田交互修复V3`
    - `场景搭建（外包）`
    - `NPCV2`
    - `spring-day1V2` 的 `04` 完整回执
  - 结合 shared root 现状和关键混线路径，判断谁能先进入第二轮“清扫认领”；
  - 只给真正边界已经站稳的线程落第二轮执行书。
- 本轮已完成：
  1. 已重新核对当前 shared root 的关键现场：
     - `GameInputManager.cs` 仍为 `M`
     - `PlayerAutoNavigator.cs` 仍为 `M`
     - `NPCDialogueInteractable.cs` / `DialogueManager.cs` 仍在 mixed 现场中
     - `NpcWorldHintBubble.cs`、`SpringDay1UiPrefabRegistry.cs/.asset`、`NavigationStaticPointValidationRunner.cs/.Menu.cs` 都仍在 working tree 中
  2. 已形成第一份正式裁定总表：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-29_全局警匪定责清扫第一轮裁定与第二轮放行_01.md`
  3. 已裁定这轮可以先放行第二轮执行书的线程只有 4 条：
     - `导航检查`
     - `导航检查V2`
     - `NPCV2`
     - `农田交互修复V3`
  4. 已给上述 4 条线程各落一份第二轮执行书：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\2026-03-29_全局警匪定责清扫第二轮执行书_01.md`
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\2026-03-29_全局警匪定责清扫第二轮执行书_01.md`
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\2026-03-29_全局警匪定责清扫第二轮执行书_01.md`
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-03-29_全局警匪定责清扫第二轮执行书_01.md`
  5. 已明确这轮不继续放行的线程：
     - `项目文档总览` = `无需继续发`
     - `spring-day1V2` = `无需继续发 / 最终停表`
     - `场景搭建（外包）` = 继续等 `UI-V1` 第一轮回执后再裁
- 当前关键判断：
  1. 现在已经可以发第二轮，但只能发“清自己、认自己、退 foreign、报 mixed”的治理执行书，不能恢复业务实现。
  2. `场景搭建（外包）` 现在不能往下推，不是因为它没回，而是因为 `UI-V1` 第一轮回执还没回来，UI 父线程 / 实施线程还不能单边定责。
  3. `项目文档总览` 和 `spring-day1V2` 这轮都已经不值得再继续发执行 prompt；前者 repo 内只剩最小 docs 尾账，后者已到 `B｜最终停表`。
- 当前恢复点：
  - 现在治理主线已从“等第一轮回执”推进到“第二轮首批执行书已发给 4 条线程”；
  - 下一次恢复时：
    - 若这 4 条线程回第二轮回执，就继续审“谁已清掉 own / 谁仍卡 mixed”
    - 若 `UI-V1` 第一轮回执先回来，就优先补裁 UI 线
    - `项目文档总览`、`spring-day1V2` 默认不再进入新一轮 prompt 分发

## 2026-03-29｜`UI-V1` 第一轮已审，UI 实施线进入第二轮，父线程继续停在治理位

- 当前主线目标：
  - 用户反馈“其他的也写完了”，要求我继续做全局把控；
  - 当前新增的关键缺口就是审 `UI-V1` 第一轮回执，并决定 UI 线现在能不能进入第二轮。
- 本轮子任务：
  - 审 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\2026-03-29_UI-V1_全局警匪定责清扫第一轮回执_01.md`
  - 结合 `SpringUI / 场景搭建（外包） / spring-day1-implementation` memory 与 shared root UI 现场，判断 UI 实施线与父线程治理线的分工
  - 若边界足够稳，则给 `UI-V1` 单独落第二轮执行书
- 本轮已完成：
  1. 已确认 `UI-V1 = spring-day1 / SpringUI` 的映射成立，且不是新线程，只是用户侧标题与 repo 内实施线的映射补齐。
  2. 已确认 `UI-V1` 第一轮边界基本成立：
     - still-own = `Prompt / Workbench + prefab-first + registry + evidence + runtime test`
     - mixed = 两个手调 prefab、`SpringDay1WorldHintBubble.cs`、`Assets/222_Prefabs/UI/Spring-day1/Primary.unity`
     - foreign = `NpcWorldHintBubble.cs`、same-GUID `Primary.unity` 事故整面、父线程治理 / 审核 / 分发面、目录迁移本身
  3. 已新增 `UI-V1` 第二轮执行书：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\2026-03-29_UI-V1_全局警匪定责清扫第二轮执行书_01.md`
  4. 已把治理总裁定补更新：
     - `UI-V1` 现在加入“可继续发 prompt”集合
     - `场景搭建（外包）` 继续停在治理位，不进入实施清扫第二轮
- 当前关键判断：
  1. `UI-V1` 现在已经够资格进入第二轮，但它拿到的也只能是 owner cleanup，不是继续自由推进 UI 体验。
  2. `场景搭建（外包）` 现在更应该停在父线程治理位；如果之后要做，也应是 docs / governance 收口，而不是继续吞实施清扫。
  3. 这样处理后，UI 线终于也从“等缺口补齐”进到了“实施线可清自己、父线程只保留治理”的稳定形态。
- 当前恢复点：
  - 现在第二轮在跑的实施线变成 5 条：
    - `导航检查`
    - `导航检查V2`
    - `NPCV2`
    - `农田交互修复V3`
    - `UI-V1`
  - 继续停发：
    - `项目文档总览`
    - `spring-day1V2`
    - `场景搭建（外包）` 的实施清扫

## 2026-03-29｜第三轮改题：从“清扫定责”切到“认领归仓与 git 上传”

- 当前主线目标：
  - 用户看到 shared root 总改动量继续升高，明确要求不要再停在“谁是谁”的回执层；
  - 用户要的是：各线程自己认领自己做的事情，并且自己去上传 git。
- 本轮子任务：
  - 回读四条已回的第二轮回执：
    - `导航检查`
    - `导航检查V2`
    - `NPCV2`
    - `农田交互修复V3`
  - 复核 `UI-V1` 当前 repo 侧状态；
  - 基于当前现场，把治理主线从“第二轮清扫认领”切到“第三轮认领归仓与 git 上传”。
- 本轮已完成：
  1. 已确认四条第二轮回执都还停在“边界更准了，但还没真的上 git”的阶段；它们大多只把 still-own 包收窄、把 remaining path 说清，没有进入真实归仓。
  2. 已确认 `UI-V1` 当前 repo 里只有第二轮执行书，没有第二轮回执文件；因此治理位不能把它按“已收件”处理。
  3. 已新增第三轮总分流文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-29_全局警匪定责清扫第三轮_认领归仓与git上传分流_01.md`
  4. 已给以下线程新增第三轮 prompt：
     - `导航检查`：`2026-03-29_全局警匪定责清扫第三轮_认领归仓与git上传_01.md`
     - `导航检查V2`：`2026-03-29_全局警匪定责清扫第三轮_认领归仓与git上传_01.md`
     - `NPCV2`：`2026-03-29_全局警匪定责清扫第三轮_认领归仓与git上传_01.md`
     - `农田交互修复V3`：`2026-03-29_全局警匪定责清扫第三轮_认领归仓与git上传_01.md`
     - `项目文档总览`：`2026-03-29_全局警匪定责清扫终轮_docs-tail认领归仓与git上传_01.md`
     - `UI-V1`：`2026-03-29_UI-V1_补第二轮回执并进入第三轮认领归仓与git上传_01.md`
  5. 已明确本轮暂不放行第三轮的线程：
     - `spring-day1V2`
     - `场景搭建（外包）`
- 当前关键判断：
  1. 第三轮不再接受“我这里还不 clean”的解释型回执；只认两类结果：
     - `已按 still-own 白名单真实 preflight + sync，并给出 SHA`
     - `已真实尝试归仓，但被第一真实 blocker 拦下，并给出 exact blocker`
  2. `项目文档总览` 现在可以被重新唤醒一次，但只允许做 terminal docs-tail 归仓；这不是恢复业务线程，而是让它自己认掉自己的 thread-doc 尾账。
  3. `spring-day1V2` 这轮仍不放行，因为它剩下的不是纯 thread-doc，而是 shared Day1 父 docs / blocker 语义；现在贸然下“自己去上传”只会再把 UI / NPC / docs 三条线搅混。
  4. `UI-V1` 当前缺的是固定回收链闭环，不是 owner 边界；所以这轮给它的是“先补第二轮回执，再进第三轮归仓”。
- 当前恢复点：
  - 治理主线现在已从 second-wave cleanup 进入 third-wave commit-readiness；
  - 下一次恢复时，优先看第三轮回执里的两类硬结果：
    - `A｜已上 git`
    - `B｜第一真实阻断已钉死`
  - `spring-day1V2` 与 `场景搭建（外包）` 暂不进入这一轮。

## 2026-03-29｜第四轮改题：clean subroot 先归仓，mixed-root 停给治理位

- 当前主线目标：
  - 用户要求我在全部回执后继续推进，不能再停在“谁是谁”的解释层；
  - 当前治理位必须把第三轮回执真正转成下一步动作，让线程自己去上传能独立归仓的部分，其余 mixed-root 由治理位接盘。
- 本轮子任务：
  - 回读 `项目文档总览`、`spring-day1V2`、`导航检查`、`导航检查V2`、`农田交互修复V3`、`NPCV2`、`UI-V1` 的最新落盘回执；
  - 对照 `D:\Unity\Unity_learning\Sunset` 当前 working tree；
  - 重新做第四轮四类裁定，并只给仍有 clean subroot 的线程发新 prompt。
- 本轮已完成：
  1. 已重新核实 `项目文档总览` 真完成、真停发：
     - `736028b0fb0c1bea1dbf348dda1c4550d1745a21`
     - `6aaf4e9327b020994058a597a46d86ea08b5eb69`
     - 当前 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\项目文档总览` 已 clean。
  2. 已重新核实 `spring-day1V2` 继续停发表成立，而且理由已经不是抽象 blocker，而是 owner-routing 已定型：
     - `NpcWorldHintBubble*` = NPC foreign
     - `SpringDay1UiPrefabRegistry*` = `UI-V1 / SpringUI` 活跃主链
     - `003-进一步搭建/memory.md` = docs / governance blocker
  3. 已新增第四轮总裁定文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-29_全局警匪定责清扫第四轮_可自归仓子根与mixed-root停发表_01.md`
  4. 已给 3 条线程落第四轮 prompt，只准冲 clean subroot：
     - `导航检查`
     - `农田交互修复V3`
     - `NPCV2`
  5. 已明确本轮不再继续自转的线程：
     - `项目文档总览` = `无需继续发`
     - `spring-day1V2` = `继续默认停发`
     - `导航检查V2` = `停给治理位接盘 mixed-root`
     - `UI-V1` = `停给治理位接盘 mixed-root`
     - `场景搭建（外包）` = `继续默认停发`
  6. 已把当前真正需要治理位接盘的 mixed-root backlog 压成 3 个核心根：
     - `Assets/Editor`
     - `Assets/YYY_Scripts/Service/Player`
     - `Assets/YYY_Scripts/Story/UI`
- 当前关键判断：
  1. 第三轮已经把“same-root blocker 存在”证明完了；继续让线程重复撞 still-own 整包，只会空转。
  2. 当前最有效的推进不是再发“解释为什么 blocked”，而是把能切出来的 clean subroot 先真的上 git。
  3. `导航检查V2` 与 `UI-V1` 现在都已经不适合继续按普通线程自转；它们剩下的是治理位 mixed-root backlog。
  4. 这轮之后，治理主线从“继续审 blocker 解释”升级成“线程先归仓 clean subroot，治理位自己处理 mixed-root 总账”。
- 当前恢复点：
  - 现在等待的不是新一轮 owner 说明，而是 3 条第四轮 clean-subroot 回执：
    - `导航检查`
    - `农田交互修复V3`
    - `NPCV2`
  - 如果这 3 条拿到 SHA，就继续剥离 shared root；
  - 如果仍失败，就按新的 first blocker 继续切更小子根；
  - `导航检查V2 / UI-V1 / spring-day1V2 / 场景搭建（外包） / 项目文档总览` 本轮都不再进入活跃继续施工池。

## 2026-03-29｜第五轮分流：`NPCV2` 退出活跃池，导航与农田按两种代码闸门分别推进

- 当前主线目标：
  - 用户确认“他们完全回复完毕了”，要求我基于最新回执继续往下裁定，而不是停在第四轮结果摘要；
  - 当前必须判断：`NPCV2` 是否彻底停表，`导航检查` 与 `农田交互修复V3` 该不该继续，以及继续时各自该走什么刀口。
- 本轮子任务：
  - 回读 3 条第四轮回执；
  - 结合 live Git 提交、working tree 和源码依赖，判断它们属于：
    - 已完成停表
    - 继续施工，但刀口不同
- 本轮已完成：
  1. 已确认 `NPCV2` 第四轮真实完成：
     - 回执自报 `preflight=yes / sync=yes / SHA=70fdd44f / own path=yes`
     - live Git 继续可见：
       - `70fdd44f 2026.03.29_NPCV2_01`
       - `a08a9b26 2026.03.29_NPCV2_02`
     - 当前 `HEAD` 下再看 `Assets/111_Data/NPC + own docs/thread` 已无剩余 diff
  2. 已确认 `导航检查` 第四轮没有再卡 same-root，而是卡在代码闸门：
     - `NavigationLiveValidationRunner.cs` 硬依赖 `PlayerAutoNavigator.Debug*`
     - `NavigationLiveValidationMenu.cs` 硬依赖 `NPCInformalChatValidationMenu.ExclusiveValidationLockKey`
     - 这说明第五轮该做的是“去 mixed 依赖”，不是再缩白名单
  3. 已确认 `农田交互修复V3` 第四轮也不再卡 same-root，而是卡在最小依赖缺口：
     - `PlacementManager.cs` 需要 `GameInputManager.ShouldPreservePlacementModeForCurrentRightClick(...)`
     - `InventorySlotUI.cs / ToolbarSlotUI.cs` 需要 `ToolRuntimeUtility.*`
     - 同时 live 仓库显示：
       - `Assets/YYY_Scripts/Controller/Input` 当前只脏 `GameInputManager.cs`
       - `Assets/YYY_Scripts/Data/Core` 当前只脏 `ToolRuntimeUtility.cs`
     - 所以第五轮对农田不该停发表，而应走“最小共享依赖扩包”
  4. 已新增第五轮总分流文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-29_全局警匪定责清扫第五轮_代码闸门分流与停发表_01.md`
  5. 已新增 2 条第五轮 prompt：
     - `导航检查`：`2026-03-29_全局警匪定责清扫第五轮_去除mixed依赖并重新归仓_01.md`
     - `农田交互修复V3`：`2026-03-29_全局警匪定责清扫第五轮_最小共享依赖扩包归仓_01.md`
  6. 已明确这轮不再继续发的线程：
     - `NPCV2` = `无需继续发 / 已退出活跃施工池`
- 当前关键判断：
  1. `NPCV2` 不该再继续发 cleanup prompt；它当前已经完成自己那块可自归仓子根，后续剩余 mixed-root 不是这条线再自转能解决的。
  2. `导航检查` 还能继续，但下一刀必须是“让 `Service/Navigation` 脱离 mixed-root 也能编译”，不是再去碰 `Service/Player` 或 `Assets/Editor`。
  3. `农田交互修复V3` 也还能继续，而且比导航更适合继续，因为它现在缺的是 2 个隔离得足够小的共享依赖，不是 broad mixed-root。
  4. 这轮之后，活跃继续施工线程从 `3` 条变成 `2` 条：
     - `导航检查`
     - `农田交互修复V3`
- 当前恢复点：
  - 先等 2 条第五轮回执：
    - `导航检查`
    - `农田交互修复V3`
  - `NPCV2` 默认退出活跃施工池，不再进入新一轮 prompt；
  - 后续治理位自己再处理 mixed-root backlog。

## 2026-03-29｜live 未提交现场复核：不是“什么都没提交”，但绝不能暴力全提

- 当前主线目标：
  - 用户在第五轮 prompt 已发、线程仍未回执时，直接追问“现在到底还有多少没提交、为什么这么难提、是不是已经解决不了了、要不要直接暴力全部提交”；
  - 我需要基于 `D:\Unity\Unity_learning\Sunset` 的 live Git 现场给出可下判断的人话结论，而不是继续复读前一轮裁定。
- 本轮子任务：
  - 复查仓库当前未提交总量、已提交的真实线程、最大脏改来源和当前活跃施工池；
  - 判断现在最该避免的误判是不是“其实什么都没提交”和“直接 `git add -A` 全提最快”。
- 本轮已完成：
  1. 已重查 live Git 现场：
     - `git status --porcelain=v1` = `231` 条
     - 其中 `Tracked = 122`
     - `Untracked = 109`
     - `Deleted = 51`
  2. 已重查当前总体 diff 量级：
     - `git diff --shortstat` = `122 files changed, 35733 insertions(+), 100920 deletions(-)`
  3. 已再次确认“不是一条都没提交”：
     - `NPCV2` 已在 `main` 上有：
       - `70fdd44f 2026.03.29_NPCV2_01`
       - `a08a9b26 2026.03.29_NPCV2_02`
     - `项目文档总览` 已在 `main` 上有：
       - `736028b0 2026.03.29_项目文档总览_01`
       - `6aaf4e93 2026.03.29_项目文档总览_02`
  4. 已确认“看起来像世界毁灭”的最大头不是单一线程，而是 4 类混线一起叠加：
     - `.kiro/specs/UI系统`：`50 files changed, 157 insertions(+), 14036 deletions(-)`，主要是旧 UI 文档树删改和迁移
     - `Assets/000_Scenes/Primary.unity`：`85162 deletions(-)`，属于高风险现场，绝不该跟文档尾账一起吞
     - `Assets/TextMesh Pro/Resources/Fonts & Materials`：`6 files changed, 15133 insertions(+), 410 deletions(-)`，属于共享字体资产 churn
     - `Assets/YYY_Scripts/Service / Story / UI`：仍有热代码 mixed-root 脏改
  5. 已确认当前活跃施工池仍只有两条，不是所有线程都还在乱跑：
     - `导航检查`
     - `农田交互修复V3`
- 当前关键判断：
  1. 现在不是“根本解决不了了”，而是“已经切掉一部分能独立归仓的线，但剩下的大头刚好都是最不适合粗暴吞并的混合现场”。
  2. `暴力全部提交` 当前不是最佳方案；它会把：
     - UI 文档迁移尾账
     - `Primary.unity` 巨量删除
     - 共享字体资产 churn
     - mixed-root 热代码
     一起固化进同一刀，等于把 stale / foreign / 未验证 / 潜在误删全部锁死。
  3. 真正拖慢速度的不是“大家都没干活”，而是剩下的现场已经从“有没有做”进入“怎么不误吞别人和高风险资产”的阶段。
  4. 当前最值得继续快推的路仍然是：等两条第五轮回执先把还能切出的代码子根提掉，再由治理位接盘剩余 mixed-root 总账。
- 当前恢复点：
  - 现在还在等待的活跃回执仍只有两条：
    - `导航检查`
    - `农田交互修复V3`
  - 若两条第五轮成功归仓，未提交总量会先明显下去；
  - 若仍失败，下一步治理位就不再等“自转奇迹”，而是开始单独清点：
    - `.kiro/specs/UI系统` 旧文档删改
    - `Primary.unity` 删除面
    - 字体资产 churn
    - `Assets/YYY_Scripts/Service/Player` / `Assets/YYY_Scripts/Story/UI` mixed-root 总账

## 2026-03-29｜用户新裁定更新：`UI系统` 旧文档迁移与 TMP 字体 churn 可视为可归仓基线，但 `Primary.unity` 仍需明确“是否接受删除”

- 当前主线目标：
  - 用户进一步说明：`.kiro/specs/UI系统` 大规模旧文档迁移/删除、`TMP 中文字体资产那一组 churn`，他现在都认可可以直接提交；
  - 但我必须先钉死 `Primary.unity` 当前现场到底是什么，再决定是否也能一起从 blocker 清单移除。
- 本轮子任务：
  - 核实 `Primary.unity` 在 `2026-03-29` 当前 live working tree 里的真实状态；
  - 判断用户这句“现在的场景和内容我认可了”能不能直接等价为“接受当前 `Primary.unity` 这一项随下次治理提交一起归仓”。
- 本轮已完成：
  1. 已确认 `.kiro/specs/UI系统` 与 `Assets/TextMesh Pro/Resources/Fonts & Materials` 用户裁定可以直接视为“已接受可归仓基线”。
  2. 已重新核实 `Primary.unity` 当前不是普通 scene dirty，而是：
     - `git status --short -- Assets/000_Scenes/Primary.unity` = `D`
     - `Test-Path D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity` = `False`
  3. 已确认这意味着当前现场里的 `Primary.unity` 是“整文件删除”，不是“场景内容改了一些待提交”。
- 当前关键判断：
  1. `UI系统` 旧文档迁移和 TMP 字体 churn 现在已经不该再作为“用户未拍板的风险项”卡在治理口径里。
  2. 但 `Primary.unity` 不能被我自动脑补成“用户接受当前场景改动”；因为当前 live 事实是“文件已不存在”。
  3. 所以 `Primary.unity` 这一项还需要用户再给一句更明确的裁定：
     - 是接受“当前这份 scene 文件直接作为删除提交”
     - 还是只接受“场景内容基线”，但不接受“删掉 `Primary.unity`”
- 当前恢复点：
  - 后续治理位可以先把：
    - `.kiro/specs/UI系统`
    - `Assets/TextMesh Pro/Resources/Fonts & Materials`
    从高风险未知项改判成“已获用户接受的接盘面”
  - `Primary.unity` 则在拿到用户对“删除是否接受”的一句明示前，继续单独保留，不跟前两类混成同一口径。

## 2026-03-29｜第六轮裁定：导航真停表，农田继续一刀，Primary 改判为迁移态而非裸删除

- 当前主线目标：
  - 用户提醒我不要被他的口头参考牵着走，而是要先看完最新回执、再独立判断下一步；
  - 我需要基于 live 回执和 working tree，重新做第六轮四类裁定。
- 本轮子任务：
  - 复审 `导航检查` 与 `农田交互修复V3` 的第五轮回执；
  - 核实 `Primary.unity` 相关现场是否是迁移，而不是单纯删除；
  - 只给仍该继续施工的线程生成下一步 prompt。
- 本轮已完成：
  1. 已确认 `导航检查` 当前应正式停表：
     - live Git 已有：
       - `acfc7f27 2026.03.29_导航检查_01`
       - `3f6e445b 2026.03.29_导航检查_02`
     - 当前再看：
       - `Assets/YYY_Scripts/Service/Navigation`
       - `.codex/threads/Sunset/导航检查`
       - `.kiro/specs/屎山修复/导航检查`
       已无相对 `HEAD` 的剩余 diff
  2. 已确认 `农田交互修复V3` 第五轮 blocker 继续成立，但下一步不该再扩包：
     - 当前 first blocker 仍是 `GameInputManager.cs`
     - exact lines：
       - `2666 -> playerInteraction.LastActionFailureReason`
       - `3249 -> targetTree.ShouldBlockAxeActionBeforeAnimation(...)`
     - 同时 live dirty 还显示：
       - `Assets/YYY_Scripts/Service/Player/PlayerInteraction.cs`
       - `Assets/YYY_Scripts/Controller/TreeController.cs`
       都处于 mixed 状态
     - 因而第六轮不允许继续把这两个文件带回白名单，只允许在 `GameInputManager.cs` 内做 compat 切口
  3. 已确认我上一轮对 `Primary.unity` 的口径需要纠正：
     - 旧位置：
       - `Assets/000_Scenes/Primary.unity` = deleted
     - 新位置：
       - `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` = untracked
       - 文件实际存在，约 `2.37 MB`
     - 这应改判成“迁移态”，不是“裸删除”
  4. 已新增第六轮总裁定：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-29_全局警匪定责清扫第六轮_导航停表农田继续与Primary迁移改判_01.md`
  5. 已新增唯一继续施工 prompt：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-03-29_全局警匪定责清扫第六轮_在GameInputManager内切断更深mixed依赖并重新归仓_01.md`
- 当前关键判断：
  1. 这轮最重要的变化是：`导航检查` 不再继续占用活跃施工位，它已经从“第五轮代码包归仓”走到“代码包 + 审计尾账也已提交”。
  2. `农田交互修复V3` 还能继续，但只能走“在 `GameInputManager.cs` 内本地兼容切口”这条路，不能再用扩包硬顶。
  3. `Primary.unity` 当前不该再被口头描述成单纯高风险删除；更准确的说法是“旧 scene 删除 + 新 scene 落到 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 的迁移态”。
  4. `UI系统` 旧文档迁移与 TMP 字体 churn 维持“已获用户接受的接盘面”判定，但 `UI-V1 / spring-day1` 这条线本轮仍不重新唤醒。
- 当前恢复点：
  - 现在活跃继续施工线程只剩：
    - `农田交互修复V3`
  - `导航检查` 退出活跃池；
  - 后续治理位处理 UI 相关 backlog 时，要把两个 `Primary.unity` 一起按迁移包审，不再把旧路径删除单独当成“是否丢文件”的结论。

## 2026-03-29｜非活跃线程再分层：不是都不用管 git，而是要区分“已完结”“零星尾账”和“治理接盘包”

- 当前主线目标：
  - 用户追问：非活跃施工现场是不是就都不用继续收 prompt、也不用管 git 提交了；
  - 我需要把“非活跃”再拆层，而不是用一句“都停了”糊过去。
- 本轮子任务：
  - 逐组回看非活跃线程在 live working tree 里的实际残留；
  - 判断它们分别属于：
    - 已完结且 git 已到位
    - 不该再发施工 prompt，但还有零星尾账
    - 不该再发旧 prompt，但必须由治理位 / integrator 接盘提交
- 本轮已完成：
  1. 已确认真完结、git 已到位的非活跃线程：
     - `NPCV2` = `0`
     - `导航检查` = `0`
     这里的 `0` 指我刚刚按其线程目录 + 工作区 + 代码根复查到的当前 `git status` 条目数
  2. 已确认“功能上完成，但还有极小尾账”的线程：
     - `项目文档总览` = `1`
     - `场景搭建（外包）` = `1`
     当前都不像需要继续发功能 prompt，更像单条 thread-doc 尾账，可由治理位顺手收掉
  3. 已确认“不应继续沿旧线程自转，但仍有成包 git backlog”的线程：
     - `spring-day1(UI-V1)` = `74`
     - `spring-day1V2` = `8`
     - `导航检查V2` = `18`
  4. 其中最重的一包仍是 `spring-day1(UI-V1)`：
     - `.kiro/specs/UI系统` 大量旧文档迁移 / 删除
     - `Assets/YYY_Scripts/Story/UI/*`
     - `Assets/222_Prefabs/UI/Spring-day1/*`
     - `Assets/111_Data/UI/Fonts/Dialogue/DialogueFontLibrary_Default.asset`
     - `Assets/000_Scenes/Primary.unity` 删除 + `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 新增
- 当前关键判断：
  1. “非活跃”不等于“全部不用再管 git”。
  2. 更准确的分法应该是：
     - 已完成且已提交：不再发 prompt，也不用再追 git
     - 只剩极小尾账：不发功能 prompt，但需要一个很小的治理收口动作
     - mixed-root / 迁移包 backlog：不能再沿旧线程继续施工 prompt，但仍然要提交，只是提交责任要转给治理位 / integrator，而不是让旧线程继续自转
  3. 所以当前最不该做的事，是把 `spring-day1(UI-V1)`、`spring-day1V2`、`导航检查V2` 这种 backlog 误判成“既然停发，就等于以后不用提交”。
- 当前恢复点：
  - 目前真正还需要新的施工 prompt 的，仍只有 `农田交互修复V3`
  - 非活跃线程里的 git 总账，下一阶段要按 3 类分别收：
    - `项目文档总览` / `场景搭建（外包）`：治理位小尾账收口
    - `spring-day1(UI-V1)`：迁移包 / mixed-root 接盘提交
    - `spring-day1V2` / `导航检查V2`：先判是治理位 absorb 其 docs tail，还是重新指定 one-shot owner 收掉

## 2026-03-29｜第七轮第一波已发：先清 NPC 老残包与小尾账，`UI-V1 / 导航检查V2` 暂缓到第二波

- 当前主线目标：
  - `农田交互修复V3` 已归仓后，活跃功能施工线程已清空；
  - 当前正式转入 `非活跃线程 git 收口波次`；
  - 这一波不再发功能 prompt，而是优先清掉最容易独立归仓、且能替后续 mixed-root 去 foreign 的几包尾账。
- 本轮子任务：
  - 复核非活跃 backlog 的 live working tree；
  - 判断哪些线程适合立刻做 one-shot git 收口，哪些必须延后到第二波；
  - 生成第七轮第一波总裁定与 thread-specific prompt。
- 本轮已完成：
  1. 已新增第七轮第一波总文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-29_全局警匪定责清扫第七轮_非活跃线程git收口第一波_01.md`
  2. 已新增 4 条 thread-specific prompt：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\2026-03-29_全局警匪定责清扫第一轮_NPC非正式聊天残包认领归仓_01.md`
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1V2\2026-03-29_全局警匪定责清扫第二轮_字体止血docs-tail与font库归仓_01.md`
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\项目文档总览\2026-03-29_全局警匪定责清扫终补轮_thread-memory尾账归仓_01.md`
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\场景搭建（外包）\2026-03-29_全局警匪定责清扫第二轮_thread-memory尾账归仓_01.md`
  3. 已明确本波四类裁定：
     - `继续发 prompt`：
       - `NPC`
       - `spring-day1V2`
       - `项目文档总览`
       - `场景搭建（外包）`
     - `暂缓到第二波`：
       - `spring-day1(UI-V1)`
       - `导航检查V2`
     - `无需继续发`：
       - `NPCV2`
       - `导航检查`
       - `农田交互修复V3`
  4. 已把暂缓理由钉死：
     - `UI-V1` 当前同根仍夹着 `NpcWorldHintBubble.cs`，现在强发只会继续卡在 NPC foreign / same-root gate
     - `导航检查V2` 当前同根仍夹着 `PlayerNpcChatSessionService.cs`，现在强发只会继续把 NPC 残包和玩家导航残包混成一坨
- 当前关键判断：
  1. 第七轮第一波最值钱的动作不是先碰 `UI-V1` 大迁移包，而是先让 `NPC` 把 `NpcWorldHintBubble.cs` 和 `PlayerNpcChatSessionService.cs` 这一组老残包认走；
  2. 这样第二波再打 `UI-V1` 和 `导航检查V2`，同根 foreign / mixed 噪音会先少一层；
  3. `项目文档总览` 和 `场景搭建（外包）` 当前都只剩 thread-memory 尾账，不值得再拖进大的治理轮次。
- 当前恢复点：
  - 先等第七轮第一波的 4 条回执；
  - 若 `NPC` 成功剥走老残包，下一波就优先唤醒：
    - `spring-day1(UI-V1)`
    - `导航检查V2`
  - 若第一波仍有人失败，则下一步继续按“第一真实 blocker”拆，而不回头发功能 prompt。

## 2026-03-29｜第七轮第一波复审：`spring-day1V2 / 项目文档总览` 已提掉，`场景搭建（外包）` 继续一刀，`NPC` 转治理 mixed-root 池

- 当前主线目标：
  - 审第七轮第一波 4 条回执；
  - 判断谁已经完成、谁还能再自提一刀、谁已经不适合继续在线程内自转。
- 本轮子任务：
  - 复核 `spring-day1V2`、`项目文档总览`、`NPC`、`场景搭建（外包）` 的最新回执与 live Git 现场；
  - 决定下一步还给不给线程 prompt。
- 本轮已完成：
  1. 已确认 `spring-day1V2` 已完成归仓：
     - live commit：`cd42f0bf 2026.03.29_spring-day1V2_01`
     - 当前：
       - `.codex/threads/Sunset/spring-day1V2 = 0`
       - `.kiro/specs/900_开篇/0.0阶段/0.0.3V2 = 0`
       - `.kiro/specs/900_开篇/spring-day1-implementation = 0`
  2. 已确认 `项目文档总览` 已完成 thread-tail 归仓：
     - live commit：`dc4d68ee 2026.03.29_项目文档总览_03`
     - 当前：
       - `.codex/threads/Sunset/项目文档总览 = 0`
  3. 已确认 `NPC` 第一波没有过：
     - 回执结果：`B`
     - first blocker：`Assets/Editor/Story/DialogueDebugMenu.cs`
     - 本质原因不是“还差一点”，而是它想归仓的残包横跨：
       - `Assets/Editor`
       - `Assets/YYY_Scripts/Service/Player`
       - `Assets/YYY_Scripts/Story/UI`
       这三根当前都已经是 mixed-root
  4. 已确认 `场景搭建（外包）` 第二轮也没有过，但性质不同：
     - 回执结果：`B`
     - 它卡的是“只带 `memory_0.md` 太窄”
     - 当前 live 对这条线的可见状态更像“整线程目录 docs-tail 还没一起收”
  5. 已新增 `场景搭建（外包）` 第三轮 prompt：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\场景搭建（外包）\2026-03-29_全局警匪定责清扫第三轮_整线程目录docs-tail归仓_01.md`
- 当前关键判断：
  1. `spring-day1V2` 和 `项目文档总览` 现在都应退出活跃治理池，不再继续发 prompt。
  2. `场景搭建（外包）` 还可以再给最后一刀，因为它当前仍是 docs-only thread-tail，不是 mixed-root 功能包。
  3. `NPC` 则不该继续在线程里自转；它这组残留已经证明自己是治理 mixed-root 池问题，不再是“线程自己再收一下”的问题。
  4. 因此在 `NPC` 这组 residual 还挂在：
     - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
     - `Assets/YYY_Scripts/Story/UI/NpcWorldHintBubble.cs`
     时，`UI-V1` 与 `导航检查V2` 仍然不适合直接开第二波施工 prompt。
- 当前恢复点：
  - 现在只继续等：
    - `场景搭建（外包）` 第三轮
  - `NPC` 及其 residual 改由治理 mixed-root 总账接盘；
  - 等 `场景搭建（外包）` 结果回来后，再决定是直接进入 mixed-root 治理接盘波，还是继续对 `UI-V1 / 导航检查V2` 做二次裁定。

## 2026-03-29｜第八轮已切入：`场景搭建（外包）` 验收通过，旧线程自提阶段正式结束，主线转入 mixed-root 治理接盘波

- 当前主线目标：
  - 验收 `场景搭建（外包）` 第三轮是否真实完成；
  - 若完成，则把“旧线程自提阶段结束、转入 mixed-root 治理接盘波”正式落盘。
- 本轮子任务：
  - 复核最新提交头与 live working tree；
  - 确认 `场景搭建（外包）` 已不再属于继续发 prompt 的对象；
  - 对剩余 `NPC / spring-day1(UI-V1) / 导航检查V2` 做阶段改判。
- 本轮已完成：
  1. 已确认 `场景搭建（外包）` 第三轮真实归仓：
     - live commits：
       - `58ebf240 2026.03.29_场景搭建（外包）_01`
       - `a300f331 2026.03.29_场景搭建（外包）_02`
  2. 已正式确认旧线程自提阶段到此结束：
     - 已真实归仓：
       - `spring-day1V2`
       - `项目文档总览`
       - `场景搭建（外包）`
     - 已无需继续发：
       - `NPCV2`
       - `导航检查`
       - `农田交互修复V3`
  3. 已把 3 条 mixed-root 线统一改判为“停止旧线程自提，转治理接盘池”：
     - `NPC`
     - `spring-day1(UI-V1)`
     - `导航检查V2`
  4. 已新增第八轮阶段文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-29_全局警匪定责清扫第八轮_转入mixed-root治理接盘波_01.md`
- 当前关键判断：
  1. 当前 live 剩余已经不是“再多等几个旧线程就会自然归仓”的结构，而是明显的交叉根 mixed-root 总账。
  2. 继续给 `NPC / UI-V1 / 导航检查V2` 发旧式 `preflight -> sync` prompt，只会重复 same-root / foreign / mixed blocker。
  3. 下一阶段应该由治理位先拆根、拆包、拆责任，再决定 one-shot owner 或 integrator 上传顺序。
- 当前恢复点：
  - 接下来不再继续审“旧线程还能不能自己提”；
  - 直接进入 mixed-root 治理接盘波，优先面对：
    - `Assets/Editor`
    - `Assets/YYY_Scripts/Service/Player`
    - `Assets/YYY_Scripts/Story/UI`
    - `Assets/222_Prefabs/UI/Spring-day1`
    - `Assets/000_Scenes`
    - `Assets/TextMesh Pro/Resources/Fonts & Materials`
    - `.kiro/specs/UI系统`

## 2026-03-30｜第九轮第一批已开：治理位先吸 `UI系统 + 屎山修复` docs 迁移包，再只放行 `导航检查V2 -> Service/Player` 首开线

- 当前主线目标：
  - 在 mixed-root 接盘阶段里，不再虚开多条线；
  - 先拿掉纯 docs 迁移噪音，再明确今天真正能开的第一条代码线。
- 本轮子任务：
  - 复核 `.kiro/specs/UI系统` 与 `.kiro/specs/屎山修复` 是否已经形成纯 docs 迁移包；
  - 若成立，由治理位直接吸收；
  - 然后对剩余代码根做第一批“开工 / 冻结”裁定。
- 本轮已完成：
  1. 已确认并吸收 docs 迁移大包：
     - roots：
       - `.kiro/specs/UI系统`
       - `.kiro/specs/屎山修复`
     - live commit：
       - `463d46ac 2026.03.30_对话丢失修复_01`
  2. docs 吸收后，live 总量已降到：
     - `TOTAL = 82`
     - `TRACKED = 39`
     - `UNTRACKED = 43`
     - `DELETED = 2`
  3. 已新增第九轮第一批裁定文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_全局警匪定责清扫第九轮_第一批开工线与冻结线_01.md`
  4. 已明确第一条真正能开的代码线：
     - `导航检查V2 -> Assets/YYY_Scripts/Service/Player` root-integrator 首开线
  5. 已新增 thread prompt 文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_导航检查V2_Service-Player根接盘开工_01.md`
  6. 已明确这轮不该虚开的两条：
     - `spring-day1(UI-V1)`
     - `NPC`
     因为它们当前物理上还缠在：
       - `Assets/YYY_Scripts/Story/Interaction`
       - `Assets/YYY_Scripts/Story/Managers`
       - `Assets/YYY_Scripts/Story/UI`
       - `Assets/YYY_Tests/Editor`
       - `Assets/Editor`
     这一批 shared story roots 里
- 当前关键判断：
  1. 现在真正能先开的，不是 `UI`，也不是 `NPC`，而是 `Service/Player` 这整根。
  2. `UI` 与 `NPC` 今天如果硬开，只会继续撞 same-root，属于假开工。
  3. 所以正确顺序是：
     - 治理位先吸纯 docs
     - 再只放行一条最稳的代码根
     - 同时继续拆 `Story` 大根
- 当前恢复点：
  - 现在等待 `导航检查V2` 按整根 `Service/Player` 做 integrator 口径回执；
  - 治理位下一刀继续拆 `Assets/YYY_Scripts/Story/**`，再决定 `spring-day1(UI-V1)` 与 `NPC` 的开工顺序。

## 2026-03-30｜第十轮已落：Story 大根已拆成语义 owner + 物理 root carrier，当前唯一首开线改判为 `spring-day1(UI-V1) -> Story/UI`

- 当前主线目标：
  - 用户明确要求我停止抽象分析，直接拆 `Story` 大根，把 `NPC / Spring / UI` 三条线在 `Story / Editor / Tests` 里的责任边界彻底钉死。
- 本轮子任务：
  - 只读复核 live dirty、线程回执、线程 memory 与实际代码内容；
  - 不能再只按线程自报认领，而要同时给出：
    - 语义 owner
    - 物理 root carrier
    - 当前到底哪条线能真开工
- 本轮已完成：
  1. 已新增第十轮总裁定文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_全局警匪定责清扫第十轮_Story大根拆包与NPC-Spring-UI分责_01.md`
  2. 已把 `Story / Editor / Tests` 当前 mixed 面拆成 3 层结论：
     - `NPC` 语义 own：
       - `NPCInformalChatValidationMenu.cs`
       - `PlayerNpcChatSessionService.cs`
       - `NPCInformalChatInteractable.cs`
       - `NpcWorldHintBubble.cs`
       - `NPCInformalChatInterruptMatrixTests.cs`
     - `UI-V1` 语义 own：
       - `Prompt / Workbench / UiLayerUtility`
       - `SpringDay1WorldHintBubble.cs`
       - `SpringDay1UiPrefabRegistry.*`
       - `SpringUiEvidenceCaptureRuntime.cs`
       - `SpringUiEvidenceMenu.cs`
       - `Prompt / Workbench` prefab
     - `Spring` 语义 own：
       - `DialogueDebugMenu.cs`
       - `CraftingStationInteractable.cs`
       - `NPCDialogueInteractable.cs`
       - `SpringDay1BedInteractable.cs`
       - `DialogueManager.cs`
       - `SpringDay1Director.cs`
       - `SpringDay1DialogueProgressionTests.cs`
  3. 已额外改判：
     - `SpringDay1LateDayRuntimeTests.cs` 不再按 UI-only 处理，而是 `Spring / UI` 交叉支撑测试
  4. 已明确物理 root carrier：
     - `Assets/YYY_Scripts/Story/UI` 这整根当前由 `spring-day1(UI-V1)` 接盘最合理
     - `Assets/YYY_Scripts/Story/Interaction` / `Assets/YYY_Scripts/Story/Managers` / `Assets/Editor/Story` / `Assets/YYY_Tests/Editor` 仍不该先放 `NPC`
     - `Assets/Editor` 直系根还混着 `NPCInformalChatValidationMenu.cs` 与 `NavigationStaticPointValidationMenu.cs`，这不是本轮 Story 首开线
  5. 已新增唯一继续施工 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_UI-V1_StoryUI整根接盘开工_01.md`
- 当前关键判断：
  1. 当前真正能先开的，不是 `NPC`，也不是 `spring-day1V2`，而是 `spring-day1(UI-V1)`。
  2. 原因不是 UI 更重要，而是 `Story/UI` 是这批 Story roots 里唯一一条：
     - foreign 最少
     - 物理边界最清楚
     - 可以通过“整根接盘 + 批准 carry 一个 NPC presentation leaf”形成真实 `preflight -> sync` 尝试
  3. `spring-day1V2` 现在仍保持停发，不是永久作废，而是本波若先重唤，会被迫吞 `Interaction / Editor/Story / Tests/Editor` 里过量 foreign，时机还不对
  4. `NPC` 现在也不能先开，因为：
     - `Story/Interaction` 还没从 Spring mixed 根里剥开
     - `Assets/Editor` 直系根还被导航菜单文件一起卡着
     - `Service/Player` 又已经先交给 `导航检查V2`
- 当前恢复点：
  - 现在等待用户把 `UI-V1 Story/UI` prompt 发出；
  - 等 `UI-V1` 回来后，再决定下一波是否重唤 `spring-day1V2` 吃剩余 Spring roots，或先接 `导航检查V2` 的回执再拆 `Assets/Editor` 直系根。

## 2026-03-30｜导航回执复核完成：继续发 prompt，但只准做 `Service/Player` 根内兼容回退，不准借 compile gate 扩到 `Data`

- 当前主线目标：
  - 在 mixed-root 治理接盘波里，继续处理 `导航检查V2 -> Assets/YYY_Scripts/Service/Player` 这一条已开工线；
  - 这轮不再重复 Story 拆包，而是把导航回执里的 compile gate 定性彻底钉死，并给出下一轮唯一施工切片。
- 本轮子任务：
  - 复核导航回执、stable launcher `preflight`、相关代码 diff 与 `HEAD` 基线；
  - 判断这两条 compile error 到底是“纯根内可修”，还是“根内文件正在依赖别的根尚未归仓的新 API”。
- 本轮已完成：
  1. 已真实复跑 `导航检查V2` 的 stable launcher `preflight`，结果与回执一致：
     - `CanContinue=False`
     - `own roots remaining dirty 数量: 0`
     - blocker 仍是 2 条代码闸门错误
  2. 已进一步钉死关键事实：
     - `PlayerNpcNearbyFeedbackService.cs` 当前新增了对 `NPCRoamProfile.HasInformalConversationContent` 的依赖
     - `PlayerNpcRelationshipService.cs` 当前新增了 `AdjustStage(...)`，内部依赖 `NPCRelationshipStageUtility.Shift(...)`
  3. 已对照 `HEAD` 基线确认：
     - `Assets/YYY_Scripts/Data/NPCRoamProfile.cs` 在 current working tree 里有 `HasInformalConversationContent`，但 `HEAD` 没有
     - `Assets/YYY_Scripts/Data/NPCRelationshipStage.cs` 在 current working tree 里有 `Shift`，但 `HEAD` 没有
  4. 已把导航这轮准确改判为：
     - 不是“纯根内两条普通 compile error”
     - 而是 `Service/Player -> Data` 的未归仓 API 依赖漂移
  5. 已新增下一轮专属 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_导航检查V2_Service-Player兼容回退清compilegate_02.md`
- 当前关键判断：
  1. `导航检查V2` 现在仍属于“继续发 prompt”，不是停发。
  2. 但不能按它回执里的旧话术继续发“留在根内清 compile gate”这种宽口径 prompt，因为那会默认接受 `Data` 根的 current dirty 当基线。
  3. 正确下一刀必须是：
     - 只在 `Assets/YYY_Scripts/Service/Player/**` 内做 compatibility rollback
     - 先证明这整根能不能不扩 `Assets/YYY_Scripts/Data/**` 就自愈
  4. 因为当前两条新增依赖都很薄，治理位优先判：
     - 先做根内兼容回退，比直接升级成 cross-root integrator 更合理
- 当前恢复点：
  - 现在等待用户把 `导航检查V2` 的 compat prompt 发出；
  - 等它回执后，再判断：
    - 是 `A｜根内兼容回退成功并 sync`
    - 还是 `B｜已证明不扩 Data 根就过不去`。

## 2026-03-30｜UI-V1 接盘已真 sync：当前应停发 UI-V1，并正式重唤 `spring-day1V2` 接剩余 Spring Story 包

- 当前主线目标：
  - 在 Story 大根 mixed-root 接盘波里，把已经 peeled 完的 UI roots 与尚未接盘的剩余 Spring-dominant roots 明确切开，并决定下一条真正该继续施工的 Story 线程。
- 本轮子任务：
  - 复核 `UI-V1` 的最新回执、对应提交与当前 own roots 状态；
  - 判断 `UI-V1` 这条线是否应停发；
  - 再根据 UI peel 后的现场，决定是否可以正式重唤 `spring-day1V2`。
- 本轮已完成：
  1. 已回读 `UI-V1` 回执文件并与 Git 现实交叉核对：
     - 回执正文本身停在 `preflight 已过、sync 待执行` 的中间态
     - 但用户转来的最新补充“已 sync、SHA=2c6276eb”与 Git 现实一致
  2. 已确认 `2c6276eb` 真正在 `main`，且提交内容命中原 prompt 允许范围：
     - `Assets/YYY_Scripts/Story/UI/**`
     - `Assets/222_Prefabs/UI/Spring-day1/**`
     - `Assets/Resources/Story/**`
     - `.codex/threads/Sunset/spring-day1/**`
     - `.kiro/specs/UI系统/0.0.1 SpringUI/memory.md`
  3. 已真实复核当前 own roots 状态：
     - `Assets/YYY_Scripts/Story/UI`
     - `Assets/222_Prefabs/UI/Spring-day1`
     - `Assets/Resources/Story`
     - `.codex/threads/Sunset/spring-day1`
     - `.kiro/specs/UI系统/0.0.1 SpringUI`
     当前都已 `remaining dirty = 0`
  4. 已据此把 `UI-V1` 裁定为：
     - 当前这条切片 `A｜已真实 sync`
     - 本波 Story 接盘里应 `无需继续发`，不是再补下一刀
  5. 已进一步复核 UI peel 后剩余 Story roots：
     - `Assets/YYY_Scripts/Story/Interaction`
     - `Assets/YYY_Scripts/Story/Managers`
     - `Assets/Editor/Story`
     - `Assets/YYY_Tests/Editor`
     现在已明显变成 `Spring-dominant` 剩余包
  6. 已新增下一条唯一继续施工 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_spring-day1V2_剩余SpringStory根接盘开工_01.md`
- 当前关键判断：
  1. `UI-V1` 这轮不是 checkpoint 冒充完成，而是真正完成了原本派给它的 `Story/UI` 整根接盘切片。
  2. 所以它现在应停发；继续给它发 prompt，只会重新把已经 peeled 完的 UI roots 拉回混战。
  3. `NPC` 现在仍不该先开，因为：
     - `NPCInformalChatInteractable.cs(.meta)` 还长在 `Assets/YYY_Scripts/Story/Interaction`
     - `NPCInformalChatInterruptMatrixTests.cs(.meta)` 还长在 `Assets/YYY_Tests/Editor`
     - `Assets/Editor` 直系根也仍混着 `NPCInformalChatValidationMenu.cs` 与 `NavigationStaticPointValidationMenu.cs`
  4. 当前最合理的下一条 Story 施工线，已经从“等 UI peel 完再看”推进成：
     - 正式重唤 `spring-day1V2`
     - 由它作为 `Spring integrator` 接剩余的 Spring-dominant Story 包
- 当前恢复点：
  - 现在等待用户把 `spring-day1V2` 新 prompt 发出；
  - 同时 `导航检查V2` 继续走它那条 `Service/Player` compat rollback；
  - 后续再根据这两条回执，决定 `NPC` 是否终于具备真实开工条件。

## 2026-03-30｜再复核当前可动面：导航这条已可停发，NPC 仍未到真实开工点；我当前还能主动做的是 `Assets/Editor` 直系根拆包

- 当前主线目标：
  - 用户明确追问“我是不是只能等回执”，因此这轮不再只说线程状态，而是要把当前哪些线已经可以停、哪些线还不能开、以及治理位现在还能主动做什么，讲成可直接下令的话。
- 本轮子任务：
  - 复核导航是否真的完成当前切片；
  - 重新判断 NPC 现在到底是否已具备真实开工条件；
  - 给出“除了等待之外，我当前还能主动推进的下一刀”。
- 本轮已完成：
  1. 已再次复核导航这条线：
     - `6a6db43e` 已在 `main`
     - 当前同一组 include paths 再跑 `preflight` 时，`own roots remaining dirty = 0`
     - 说明 `Service/Player` compat rollback 这刀已真实收住
  2. 已据此把 `导航检查V2` 当前切片裁定为：
     - `A｜已真实 sync`
     - 当前应停发，而不是继续顺手扩到 `Data/**` 或别的根
  3. 已重新审 `NPC` 当前现场：
     - `NPCInformalChatValidationMenu.cs(.meta)` 仍卡在 `Assets/Editor` 直系根
     - `NPCInformalChatInteractable.cs(.meta)` 仍卡在 `Assets/YYY_Scripts/Story/Interaction`
     - `NPCInformalChatInterruptMatrixTests.cs(.meta)` 仍卡在 `Assets/YYY_Tests/Editor`
     - 与此同时，这些根里仍混着 `Spring` 或 `导航` 的剩余文件
  4. 已据此确认：
     - `NPC` 现在仍不该先开
     - 原因不是 NPC 自己没活，而是物理 roots 还没剥到足够干净
  5. 已进一步把“除了等待还能做什么”收缩成一个明确候选：
     - 当前治理位还能主动做的下一刀，是拆 `Assets/Editor` 直系根
     - 因为现在这根里已经能清楚区分：
       - `NPCInformalChatValidationMenu.cs` = NPC validation 菜单
       - `NavigationStaticPointValidationMenu.cs` = 导航静态验证菜单
       - 这不是 owner 不清，而是物理落点还没被治理剥开
- 当前关键判断：
  1. 我现在不是“只能等”。
  2. 当前已经可以直接停掉两条已完成切片：
     - `UI-V1`
     - `导航检查V2`
  3. 当前 Story 主线仍应由 `spring-day1V2` 继续接剩余 Spring 包，这条不能被跳过。
  4. `NPC` 现在仍不该虚开；但治理位现在还能主动推进的，不是瞎等，而是下一步继续拆 `Assets/Editor` 直系根，为 NPC 后续开工减掉一个真实 blocker。
- 当前恢复点：
  - 现在一条在建主线仍是 `spring-day1V2 -> 剩余 Spring Story 包`
  - `导航检查V2` 当前切片可停发
  - `NPC` 仍待 `Story/Interaction / Tests/Editor / Assets/Editor` 进一步剥包后再开
  - 如果用户要我不等，我下一刀最值得主动做的是：`Assets/Editor` 直系根拆包治理

## 2026-03-30｜spring-day1V2 新 blocker 已复核：不是该退回治理位的大跨根问题，而是 `DialogueDebugMenu.cs` 根内可切的编译耦合

- 当前主线目标：
  - 继续推进 `spring-day1V2 -> 剩余 Spring Story 包`；
  - 用户转来最新回执后，这轮要判断它是该停给治理位重拆，还是还能继续在当前根内往前推。
- 本轮子任务：
  - 复核 `spring-day1V2` 最新 `preflight` blocker
  - 判断 `DialogueDebugMenu.cs -> NPCInformalChatValidationMenu` 这条依赖到底是什么级别的问题
  - 决定是否继续给 `spring-day1V2` 发下一轮 prompt
- 本轮已完成：
  1. 已真实复跑 `spring-day1V2` 当前整包 `preflight`，结果与回执一致：
     - `own roots remaining dirty = 0`
     - 第一真实 blocker = `Assets/Editor/Story/DialogueDebugMenu.cs:23`
     - `CS0103`：当前上下文不存在 `NPCInformalChatValidationMenu`
  2. 已直接回读代码，确认阻断点只是一条编译时常量引用：
     - `DialogueDebugMenu.IsSuppressedByNpcValidation()` 里读取 `NPCInformalChatValidationMenu.ExclusiveValidationLockKey`
     - `NPCInformalChatValidationMenu.cs` 本体里暴露的只是 `EditorPrefs` 锁 key 常量：`Sunset.NpcInformalChatValidation.Active`
  3. 已据此改判：
     - 这不是需要先交给 `NPC` 或治理位去搬 `Assets/Editor` 直系根的 blocker
     - 而是 `spring-day1V2` 当前仍可在 `Assets/Editor/Story/**` 根内完成的最小 decouple
  4. 已新增下一轮唯一施工 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_spring-day1V2_断开DialogueDebugMenu对NPC菜单编译耦合_02.md`
- 当前关键判断：
  1. `spring-day1V2` 现在不该停发。
  2. 但下一轮不该再让它讲“交给治理位拆”，而是继续留在当前白名单根里，先把 `DialogueDebugMenu.cs` 对 NPC 菜单常量的编译耦合切掉。
  3. 这比现在就升级成 `Assets/Editor` 根搬家更窄、更快，也更符合这一波 `Spring integrator` 的 slice。
  4. `NPC` 当前仍不该开；这条新 blocker 也不构成提前重唤 NPC 的理由。
- 当前恢复点：
  - 现在等待用户把 `spring-day1V2` 的 decouple prompt 发出；
  - 若它下一轮通过 `preflight -> sync`，剩余 Story 包会再往前收一大步；
  - `NPC` 仍等这包继续 peeled 后再看。

## 2026-03-30｜spring-day1V2 已完成剩余 Spring Story 包，当前唯一继续施工线改判为 `NPC -> 剩余NPC package`

- 当前主线目标：
  - 在 `UI-V1`、`导航检查V2`、`spring-day1V2` 三条切片都已各自收住后，把下一条真正还能继续推进的线明确切给 `NPC`。
- 本轮子任务：
  - 复核 `spring-day1V2` 最新提交与当前 clean 状态；
  - 判断它现在是否应停发；
  - 同时把 `NPC` 当前剩余包压成一个真实可跑的切片。
- 本轮已完成：
  1. 已确认 `spring-day1V2` 最新提交 `97dc123` 已在 `main`，且当前同组 include paths 再跑 `preflight` 时：
     - `CanContinue=True`
     - `own roots remaining dirty = 0`
  2. 已据此把 `spring-day1V2` 当前切片裁定为：
     - `A｜已真实 sync`
     - 当前应停发
  3. 已复核当前剩余 dirty 分布，确认 NPC 现在真正剩下的主包已经收缩为：
     - `Assets/YYY_Scripts/Controller/NPC/**`
     - `Assets/YYY_Scripts/Data/NPCDialogueContentProfile.cs`
     - `Assets/YYY_Scripts/Data/NPCRelationshipStage.cs`
     - `Assets/YYY_Scripts/Data/NPCRoamProfile.cs`
     - `Assets/YYY_Scripts/Data/NPCInformalChatExitModel.cs(.meta)`
     - `Assets/Editor/NPCInformalChatValidationMenu.cs(.meta)`
     - `Assets/Editor/NPC.meta`
     - `.codex/threads/Sunset/NPC/**`
  4. 已进一步确认：
     - `Assets/Editor` 直系根当前仍混着 `NavigationStaticPointValidationMenu.cs(.meta)`
     - 所以 NPC 这轮正确动作不是继续碰 `Assets/Editor` 直系根，而是先把自己的 validation 菜单迁入 `Assets/Editor/NPC/`
  5. 已新增唯一继续施工 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_NPC_剩余NPC包接盘开工_02.md`
- 当前关键判断：
  1. 现在不再是“我只能等回执”。
  2. 当前已应停发的线程切片有：
     - `UI-V1`
     - `导航检查V2`
     - `spring-day1V2`
  3. 当前唯一新的继续施工线是：
     - `NPC`
  4. 这条 `NPC` 新切片不是旧的“非正式聊天残包 exact 回补”，而是：
     - `NPC integrator`
     - 以“迁入 `Assets/Editor/NPC/` + 收 Controller/NPC + 收 Data/NPC* + 收 thread dir”为唯一主刀
- 当前恢复点：
  - 现在等待用户把 `NPC` 新 prompt 发出；
  - 若 `NPC` 这轮通过 `preflight -> sync`，当前 mixed-root 接盘波会再往前收一大步。

## 2026-03-30｜day1 最终 sync 回执已核实，当前 mixed-root 接盘波收束为“三停一继续”

- 当前主线目标：
  - 在 `spring-day1V2` 交出最终 `sync` 回执后，把这波 mixed-root 接盘的活线重新收束成最新可执行状态，避免继续对已闭环线程误发 prompt。
- 本轮子任务：
  - 复核 `spring-day1V2` 最终回执与 Git 现实是否一致
  - 复核现成 `NPC` prompt 是否仍与当前工作树相符
  - 补跑一次 `skill-trigger-log` 健康检查，确认治理审计层仍 clean
- 本轮已完成：
  1. 已确认 `spring-day1V2` 最终提交 `97dc123a` 已在 `main`，且用户转来的最新回执与 Git 现实一致：
     - `DialogueDebugMenu.cs` 根内最小 decouple 已归仓
     - 该包 own roots 当前已 clean
  2. 已再次交叉核对当前工作树，确认现在仍应继续施工的只剩 `NPC` 包；
     `UI-V1`、`导航检查V2`、`spring-day1V2` 三条都不应再被唤回
  3. 已复核 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_NPC_剩余NPC包接盘开工_02.md` 仍与最新现场一致，不需要改写：
     - `NPC` 这轮仍应以 `NPC integrator` 身份接 `Controller/NPC + Data/NPC* + Editor/NPC + thread dir`
     - 第一步仍是把 `NPCInformalChatValidationMenu.cs` 迁入 `Assets/Editor/NPC/`，避开 `Assets/Editor` 直系根里的导航菜单残面
  4. 已补跑 `check-skill-trigger-log-health.ps1`，结果：
     - `Canonical-Duplicate-Groups = 0`
     - 审计层当前健康
- 当前关键判断：
  1. 这波治理现在不是“很多线程都半开着”，而是已经收束成：
     - `无需继续发`：`UI-V1`、`导航检查V2`、`spring-day1V2`
     - `继续发 prompt`：`NPC`
  2. 当前仓库里仍可见的大脏面，例如 `Primary.unity` 删除面、TMP 中文字体资产 churn、`OcclusionManager.cs`，都不属于这轮 mixed-root 接盘波允许顺手吞并的范围
  3. 所以下一步不该再回头重开已 sync 的三条线，也不该临时扩题，而应只把现成 NPC prompt 发出去，等它交回 `A｜真实 sync` 或 `B｜第一真实 blocker`
- 当前恢复点：
  - 当前唯一继续施工入口保持为：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_NPC_剩余NPC包接盘开工_02.md`
  - 在 `NPC` 回执回来前，这波 mixed-root 接盘不再新增别的活线

## 2026-03-30｜NPC 新 blocker 已复核：不是停发，而是最小补带 `Navigation` 的 Editor leaf

- 当前主线目标：
  - 继续推进当前唯一活线 `NPC`；
  - 在它交回 `B｜第一真实 blocker 已钉死` 后，判断这是不是应停给别的线程，还是仍属于 NPC 当前 package 内可继续收口的最小补带。
- 本轮子任务：
  - 复核 `NPC` 最新回执、工作树与 `Assets/Editor` 现场
  - 判断 `NavigationStaticPointValidationMenu.cs(.meta)` 这对文件应该怎么处理
  - 给出下一轮唯一继续施工 prompt
- 本轮已完成：
  1. 已确认 `NPC` 上一轮做的迁根动作属实：
     - `NPCInformalChatValidationMenu.cs(.meta)` 已迁到 `Assets/Editor/NPC/`
  2. 已确认它这轮新的第一真实 blocker 也属实：
     - `Assets/Editor/NavigationStaticPointValidationMenu.cs(.meta)` 仍落在脚本聚合出来的 own root `Assets/Editor` 下
  3. 已只读核对 `Assets/Editor` 现场，当前与这次 blocker 直接相关的未归仓 Editor 残面已经收缩成：
     - `Assets/Editor/NPC.meta`
     - `Assets/Editor/NPC/NPCInformalChatValidationMenu.cs(.meta)`
     - `Assets/Editor/NavigationStaticPointValidationMenu.cs(.meta)`
  4. 已据此改判：
     - 这不是需要重唤 `导航检查V2` 的新施工线
     - 而是 `NPC` 当前 package 还差一个最小的 `carried foreign editor leaf`
  5. 已新增下一轮唯一 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_NPC_补带NavigationEditorLeaf完成剩余NPC包sync_03.md`
- 当前关键判断：
  1. 上一轮 prompt 的错误不在“让 NPC 迁根”，而在“把迁到子目录”误当成“足以躲开 `Assets/Editor` 父根”
  2. 当前最快、最窄、最不打断别人的做法，是让 `NPC` 在本包里补带：
     - `Assets/Editor/NavigationStaticPointValidationMenu.cs`
     - `Assets/Editor/NavigationStaticPointValidationMenu.cs.meta`
     作为 `carried foreign editor leaf`
  3. 这不等于把导航语义 owner 转给 NPC；只是为了把当前真实 own root `Assets/Editor` 收干净
  4. 所以下一步不是停发，也不是重开导航，而是继续给 `NPC` 发一轮更窄的 follow-up prompt
- 当前恢复点：
  - 当前唯一继续施工入口更新为：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_NPC_补带NavigationEditorLeaf完成剩余NPC包sync_03.md`
  - 若这轮再过不了，再按新的第一真实 blocker 重新裁定

## 2026-03-30｜清扫波阶段复盘：当前已从多线乱流收束成“NPC 收尾 + 热根另案”

- 当前主线目标：
  - 用户要求我从全局角度说明“这轮清扫还剩多少、做完后怎么办、后续还能不能继续这样开发”，所以这轮不再只审单条回执，而是把 mixed-root 清扫波与整个仓库健康度拆成两层总图。
- 本轮子任务：
  - 重新核当前 Git 现场
  - 重新判断当前剩余脏面的主次关系
  - 给出清扫波结束后的开发方式建议
- 本轮已完成：
  1. 已重新核当前工作树，当前可见剩余 path entry 主要收束为三组：
     - `NPC` 当前活线残面
     - `Primary.unity` 删除面
     - TMP 中文字体与少数 shared runtime 根残面
  2. 已把进度分成两层判断：
     - 如果只看这波 `mixed-root` 接盘清扫：已到最后一刀，核心只剩 `NPC`
     - 如果看整个仓库回到“可放心开工”的健康基线：还没有结束，至少还压着 `Primary.unity` 与 TMP 字体稳定性两类热根问题
  3. 已形成新的治理结论：
     - 当前这套“典狱长 + integrator peeling”模式还可以再跑完 `NPC` 这一刀
     - 但不应无限继续扩成长期默认开发模式
- 当前关键判断：
  1. 这波清扫波本身已接近尾声，继续收益最大的动作仍是先收掉 `NPC`
  2. 清扫波做完后，不应继续把所有后续开发都维持在“治理审回执 + 再派 integrator”模式，否则治理成本会开始大于开发收益
  3. `NPC` 收完之后，后续开发应切回“热根单 owner + 正常功能线程”的模式，并把真正还脏的 shared hot roots 单独立案：
     - `Primary.unity`
     - `DialogueChinese*` / TMP 中文字体资产
     - 其余少量 shared runtime 根（如 `OcclusionManager.cs`）按独立 owner 处理
- 当前恢复点：
  - 当前治理主线仍先等 `NPC` 这轮 03 prompt 回执
  - 若 `NPC` 过线，这波 mixed-root 清扫就应宣布阶段收口，并转入“热根另案 + 正常开发恢复”

## 2026-03-30｜NPC 已真实 sync：mixed-root 清扫主线正式收口，后续切回“热根单 owner + 正常开发”

- 当前主线目标：
  - 在 `NPC` 03 回执交回真实 `sync` 后，判断这波 mixed-root 清扫是否可以正式结束，并给出清扫后的开发模式切换建议。
- 本轮子任务：
  - 复核 `NPC` 最终回执、提交与当前工作树
  - 判断当前是否还存在需要继续按典狱长模式扩线的线程
  - 明确清扫结束后的下一阶段主矛盾
- 本轮已完成：
  1. 已确认 `NPC` 最终提交 `cc0bc7f8` 已在 `main`，且这次真实过线的口径成立：
     - `NavigationStaticPointValidationMenu.cs(.meta)` 只是 `carried foreign editor leaf`
     - 不代表导航 owner 语义转移
     - 当前这组 own paths 已 clean
  2. 已重新核当前工作树，确认 `NPC` 相关实现与文档残面已经退出活跃清扫主线；当前剩余 path entry 已压到 12 条，且不再属于这波 multi-thread peeling 的同一类问题
  3. 已据此把 mixed-root 清扫主线改判为：
     - `已完成`
     - 不再继续唤回 `UI-V1`、`导航检查V2`、`spring-day1V2`、`NPC`
  4. 已明确清扫后剩余问题改由“热根另案”承接，当前最值得单独立案的就是：
     - `Assets/000_Scenes/Primary.unity` 真实删除面
     - `DialogueChinese*` / TMP 中文字体资产 churn
     - 少量 shared runtime 根：`TreeController.cs`、`OcclusionManager.cs`
- 当前关键判断：
  1. 这波清扫模式已经完成它该做的事了，再继续沿用，只会让治理成本继续膨胀
  2. 现在真正的最大问题，已经从“多线程 mixed-root 互卡”转移成“少数共享热根仍未单独定责”
  3. 所以后续开发模式不应再是“继续四处发 integrator prompt”，而应切回：
     - 热根单 owner
     - 正常功能线程
     - 只有再次出现 mixed-root incident 时才重新启用典狱长模式
- 当前恢复点：
  - mixed-root 清扫主线到此收口
  - 下一阶段建议顺序：
    1. 先为 `Primary.unity` 真实删除面单独立案
    2. 再为 TMP 中文字体稳定性单独立案
    3. 最后把 `TreeController.cs` / `OcclusionManager.cs` 这类 shared runtime 残面分配 owner 后恢复正常开发

## 2026-03-30｜热根另案已落盘，当前剩余从“乱根清扫”收缩为“少数热根 + 轻量尾账”

- 当前主线目标：
  - 用户要求我直接把 mixed-root 清扫之后剩下的四类尾项收成明确裁定，不再继续发散：
    - `Primary.unity`
    - TMP 中文字体稳定性
    - shared runtime 残面
    - 文档尾账与 `.codex/artifacts`
- 本轮子任务：
  - 对上述 4 类剩余项做只读实查
  - 形成新的治理案文件
  - 把当前真正能直接收的轻量项压进本轮 docs-only 收口范围
- 本轮已完成：
  1. 已新增 `Primary.unity` 删除面立案：
     - `2026-03-30_单独立案_Primary.unity删除面_01.md`
  2. 已新增 TMP 中文字体稳定性立案：
     - `2026-03-30_单独立案_TMP中文字体稳定性_01.md`
  3. 已新增 shared runtime 残面定责：
     - `2026-03-30_shared-runtime残面定责_01.md`
  4. 已新增轻量尾账收口说明：
     - `2026-03-30_轻量尾账收口_01.md`
  5. 已把四类剩余项重新压成当前唯一可靠判断：
     - `Primary.unity` = 应恢复旧 canonical path，不是确认删除
     - TMP 中文字体 = 独立共享 importer 稳定性案，不再混进业务线
     - `OcclusionManager.cs` = 农田 preview 遮挡尾差
     - `TreeController.cs` = 农田/砍树表现包，不是 shared runtime 小尾账
     - 轻量可直接收的只剩：
       - `.codex/artifacts/README.md`
       - `.codex/threads/Sunset/项目文档总览/memory_0.md`
- 当前关键判断：
  1. mixed-root 清扫波已经结束，这轮不该再继续用 integrator / 典狱长模式扩面。
  2. 当前剩余问题已经收束成两种：
     - 需要单独 owner 的热根
     - 可以直接 docs-only 收掉的轻量尾账
  3. 其中最重要的新改判是：
     - `TreeController.cs` 不能再被说成“小范围 shared runtime 残面”，否则会再次低估实际工作量和验证面。
- 当前恢复点：
  - 这轮之后，仓库剩余问题的主叙事应改成：
    1. `Primary.unity` 单 writer 另案
    2. TMP 中文字体稳定化另案
    3. 农田方向按 `OcclusionManager` 小尾差 / `TreeController` 大包分刀
    4. 文档尾账与 artifact README 直接 docs-only 收口

## 2026-03-30｜热根定责口径再修正：历史触碰者与当前可提交 owner 不是一回事

- 当前主线目标：
  - 用户质疑我上一轮把“这些残面到底是谁干的”和“现在该由谁来提提交”混在了一起，要求我重新按线程记忆和实际材料把责任链理清，不要再糊涂下判断。
- 本轮子任务：
  - 回查 `spring-day1`、`spring-day1V2`、`农田交互修复V3` 等线程记忆
  - 重新区分：
    - 历史上谁留下了这些改动
    - 现在谁适合继续提交
- 本轮已完成：
  1. 已确认 `Primary.unity` 这案子里，`Spring/UI` 的历史触碰证据比我上一轮说得更强：
     - `UI-V1` 接盘 `Story/UI` 整根时，明确把 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity(.meta)` 当作迁移 sibling 一起带走
     - `spring-day1` / `spring-day1-implementation` 历史记忆里多次明确提到 `Primary.unity` 与 Day1 / Spring UI 现场绑定
  2. 已确认 `OcclusionManager.cs` 与 `TreeController.cs` 当前更像 `farm` 线残面，而不是漂浮 shared runtime：
     - `农田交互修复V3` 的记忆里直接把两者带入过真实 preflight 白名单
     - `OcclusionManager.cs` 的 preview `GetColliderBounds()` 证据链也直接落在农田工作区
  3. 已确认 TMP 这组字体资产虽然被 `spring-day1` / UI 线频繁提到，但那些回执更多是在“绕开 / 降权 / 不混入本轮提交”，而不是形成一个干净 owner：
     - 这组资产当前仍更像共享 importer / 动态字体 churn
     - 不是靠再问 `UI / Spring / NPC` 三家一遍，就会自然得到一个唯一 owner
- 当前关键判断：
  1. 我上一轮真正说糊涂的地方，不是“风险判断错了”，而是把“历史触碰者”直接说成了“当前提交 owner”。
  2. 修正后更准确的口径应该是：
     - `Primary.unity`：历史上大概率就是 `Spring/UI` 迁移出来的；但当前删除旧 canonical path 这一步，仍不能在未确认迁移意图前直接提交
     - `TreeController.cs / OcclusionManager.cs`：当前应优先回到 `farm` 线认领
     - TMP 字体：继续按共享稳定性案处理，不再伪装成某一条业务线的普通尾账
- 当前恢复点：
  - 后续若继续问线程，不该群发乱问；
  - 真正有价值的只剩两种问法：
    1. 向 `Spring/UI` 问 `Primary.unity` 迁移意图
    2. 向 `farm` 问 `TreeController.cs / OcclusionManager.cs` 是否按当前 working tree 直接收口

## 2026-03-30｜已生成两条精准追问 prompt：只问 `Primary` 迁移意图与 `farm` 认领边界

- 当前主线目标：
  - 用户已明确要求我不要再群发乱问，而是直接进入下一步；
  - 这轮主线因此收紧成“热根单 owner + 精准追问”。
- 本轮子任务：
  - 只生成两份新的典狱长 prompt 文件：
    1. 给 `UI-V1 / Spring UI`，只问 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 到底是不是最终 canonical path
    2. 给 `farm`，只问 `TreeController.cs / OcclusionManager.cs` 现在能不能直接回到 `farm` 线收口
- 本轮已完成：
  1. 已新增 `UI-V1` 窄刀 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_UI-V1_确认Primary迁移意图_01.md`
  2. 已新增 `farm` 窄刀 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_farm_确认TreeController与OcclusionManager认领_01.md`
  3. 两份 prompt 都已按最新纠偏口径写死：
     - `UI-V1` 这轮只做 `Primary` 迁移语义裁定，不准开改 scene
     - `farm` 这轮只做 `TreeController / OcclusionManager` 认领边界裁定，不准顺手改代码
- 当前关键判断：
  1. 这轮最重要的不是再做一次“所有剩余项总分析”，而是把真正有价值的两条追问打出去。
  2. 这两条 prompt 都是 `只读认定`，不是开工令：
     - 先把语义和认领边界问清
     - 再决定是否进入后续 single-writer / owner 收口
  3. TMP 中文字体稳定性案这轮继续不群问，因为目前仍没有一个干净单一 owner 候选。
- 当前恢复点：
  - 下一步不是我继续扩分析，而是把这两份 prompt 转给用户发出；
  - 等 `UI-V1` 和 `farm` 回执后，再决定：
    1. `Primary.unity` 是走恢复旧 canonical path，还是承认迁移目标并开成套迁移案
    2. `TreeController / OcclusionManager` 是一起回 `farm`，还是拆成轻尾差 + 大包两刀

## 2026-03-31｜两条追问回执已通过，现已切到 3 条开工 prompt

- 当前主线目标：
  - 用户要求我不要再停留在问责和分析，而是直接开始落地；
  - 这轮主线因此正式从“追问 owner”切到“发开工 prompt”。
- 本轮子任务：
  - 根据 `UI-V1` 与 `farm` 的已审回执，把下一阶段真正要开的 3 刀直接写成施工 prompt：
    1. `spring-day1`：`Primary.unity` single-writer 恢复旧 canonical path
    2. `farm`：`OcclusionManager.cs` 小尾差归仓
    3. `farm`：`TreeController.cs` 完整包归仓
- 本轮已完成：
  1. 已新增 `Primary` 开工 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-31_典狱长_spring-day1_Primary单writer恢复旧canonical_01.md`
  2. 已新增 `farm/OcclusionManager` 开工 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-31_典狱长_farm_OcclusionManager小尾差归仓_01.md`
  3. 已新增 `farm/TreeController` 开工 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-31_典狱长_farm_TreeController完整包归仓_01.md`
  4. 额外新发现的真实热区事实也已写进 `Primary` prompt：
     - 当前 `.kiro/locks/active/A__Assets__000_Scenes__Primary.unity.lock.json` 仍存在
     - `owner_thread = NPC`
     - 但旧路径 scene 已被删，所以 `Check-Lock.ps1` 对这个删除面会直接报“目标不存在”
     - 这意味着 `Primary` 目前不是简单的“无锁可写”，而是 `deleted canonical path + stale NPC lock`
- 当前关键判断：
  1. `UI-V1` 与 `farm` 的追问都已经问完了，不该再发解释题。
  2. `Primary` 这条线虽然可以开工，但它现在真实存在一个 stale hot-lock 现实，所以 prompt 已明确要求：
     - 不准装作无锁
     - 如果锁仍阻断，就老实回第一真实 blocker
  3. `farm` 这边现在可以并行开两刀：
     - `OcclusionManager` 小刀
     - `TreeController` 大刀
- 当前恢复点：
  - 下一步就是把这 3 份开工 prompt 发给用户转发；
  - 等回执回来后，再看：
    1. `Primary` 是直接恢复旧 canonical path 过线，还是先卡在 stale lock blocker
    2. `farm` 两刀能否分别真实 `preflight -> sync`

## 2026-03-31｜`spring-day1` 已撞到 stale NPC lock，现已补发 NPC 善后 prompt

- 当前主线目标：
  - 用户反馈两件新事实：
    1. `farm/OcclusionManager` 已真实归仓
    2. `spring-day1` 在 `Primary` single-writer 第一刀里，已把 first blocker 查穿成 stale NPC active lock
  - 主线因此不再是继续催 `spring-day1`，而是先把这把过期锁合法收口。
- 本轮子任务：
  - 评估用户误把 `Primary` 语义 prompt 发给 `UI` 是否会伤主线
  - 基于 `spring-day1` 的 blocker 回执，决定下一条应该发给谁
  - 把真正该发的补救 prompt 落成文件
- 本轮已完成：
  1. 已确认误发给 `UI` 这件事本身问题不大：
     - 因为 `UI` 这边做的是只读语义裁定
     - 它不会直接改 scene 或改变当前热根 owner
  2. 已确认现在不该继续催 `spring-day1`：
     - 因为它已经把 blocker 查穿
     - 再发给它，只会继续撞同一把 stale NPC lock
  3. 已新增真正该发的补救 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-31_典狱长_NPC_Primary过期锁善后与旧canonical恢复_01.md`
  4. 这份 prompt 已把唯一主刀写死为：
     - 由 `NPC` 以当前 lock owner 身份
     - 先把 `Assets/000_Scenes/Primary.unity(.meta)` 从 `HEAD` 恢复
     - 再合法释放这把 stale active lock
- 当前关键判断：
  1. 现在不能继续把 `Primary` 这条线挂在 `spring-day1` 身上空等。
  2. 真正该先做的是让 `NPC` 结束自己留下的 hot-lock 现场。
  3. `farm` 这边现在可以继续按顺序推进：
     - `OcclusionManager` 已闭环
     - 再继续 `TreeController` 即可
- 当前恢复点：
  - 下一步转发顺序应变成：
    1. 不管 `UI` 那条误发
    2. 立刻把 `NPC` 这份 stale lock 善后 prompt 发出去
    3. `farm` 继续跑 `TreeController` 那条第二刀
    4. 等 `NPC` 回来后，再决定是否还需要重发 `spring-day1` 的 `Primary` prompt

## 2026-03-31｜`Primary` 新路径 duplicate sibling 已按 `A` 删除并归仓

- 当前主线目标：
  - 不再处理旧 canonical path，也不再讨论迁移意图；
  - 当前唯一主线是按 `2026-03-31_典狱长_spring-day1_Primary新路径duplicate处置_02.md`，只把 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity(.meta)` 这份 same-GUID duplicate sibling 做最小处置。
- 本轮子任务：
  1. 复核治理 prompt、线程 memory、父工作区 memory 与 `Primary` 删除面立案
  2. 核实 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity.meta` 与 `Assets/000_Scenes/Primary.unity.meta` 当前确实同 GUID：`a84e2b409be801a498002965a6093c05`
  3. 在 `A 删除 duplicate sibling` 与 `B 保留但改独立 GUID` 之间做最小裁定，并真实跑同组 `preflight -> sync`
- 本轮已完成：
  1. 已确认新路径 `Primary` 当前没有 live canonical 身份，现行引用只剩历史文档 / memory 证据，不再被 `Build Settings`、`NPCAutoRoamControllerEditor.cs` 或其他 live 入口使用
  2. 已按 `A` 删除：
     - `Assets/222_Prefabs/UI/Spring-day1/Primary.unity`
     - `Assets/222_Prefabs/UI/Spring-day1/Primary.unity.meta`
  3. 删除后已复核 `Assets` 根内只剩旧 canonical 的 `Primary` GUID：
     - `Assets/000_Scenes/Primary.unity.meta`
  4. 已真实跑过最小白名单：
     - `preflight = pass`
     - `sync = pass`
     - 提交 `1e07d04039669a445b3697da05aefe43e48aca0a`
- 当前关键判断：
  1. 这轮应选 `A`，不是 `B`，因为 `UI-V1` 已经完成只读裁定：新路径 scene 不是最终 canonical path，只是迁移 sibling / 临时复制面
  2. 这轮成功的关键不在“重整 `Primary` 整案”，而在“只删 duplicate 身份本身”，因此没有顺手触碰旧 canonical path、`EditorBuildSettings.asset`、`NPCAutoRoamControllerEditor.cs` 或任何 scene 内容
  3. 当前 broader working tree 仍保留其他 unrelated dirty，但这刀自己的最小白名单已经独立归仓
- 当前恢复点：
  - `Primary` 这案子的 duplicate sibling 已经收掉；
  - 后续若再讨论 `Primary`，不该回头重问“新路径是不是 canonical”，而应只围绕剩余真实业务问题继续。

## 2026-03-31｜`Primary` 已彻底退场，下一刀正式切到共享 TMP 中文字体稳定性

- 当前主线目标：
  - 用户要求我不要停在“这刀认不认”的分析层，而是直接开始下一步；
  - 在 `Primary` 旧路恢复、stale lock 善后、新路径 duplicate 删除都已完成后，当前仓库里剩下最大的真实脏面已经切换成 `DialogueChinese* / LiberationSans Fallback` 这组共享 TMP 中文字体 churn。
- 本轮子任务：
  1. 回读 `2026-03-30_单独立案_TMP中文字体稳定性_01.md`
  2. 重新核当前 dirty 规模与相关硬证据
  3. 把下一轮真正可发的施工 prompt 压成单刀
- 本轮已完成：
  1. 已确认当前 TMP 脏面仍只落在 6 份资产：
     - `DialogueChinese BitmapSong SDF.asset`
     - `DialogueChinese Pixel SDF.asset`
     - `DialogueChinese SDF.asset`
     - `DialogueChinese SoftPixel SDF.asset`
     - `DialogueChinese V2 SDF.asset`
     - `LiberationSans SDF - Fallback.asset`
  2. 已确认当前 diff 量级没有变小：
     - `15133 insertions`
     - `410 deletions`
  3. 已确认当前生成器硬证仍成立：
     - `Assets/Editor/Story/DialogueChineseFontAssetCreator.cs`
     - `AtlasPopulationMode.Dynamic`
     - `isMultiAtlasTexturesEnabled = true`
  4. 已新增下一条正式施工 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-31_典狱长_spring-day1_TMP中文字体稳定性回到已提交基线判定_02.md`
- 当前关键判断：
  1. `spring-day1` 在 `Primary` 题上应正式停发，不再继续回头碰 scene。
  2. 这组字体 dirty 当前更像共享 importer / atlas / glyph churn，而不是业务成果；因此下一刀最合理的不是“继续重建字体方案”，而是先判清：
     - 它们能不能整根安全回到 `HEAD`
  3. 我这轮仍然把执行线程落在 `spring-day1`，不是因为这题又变回 Day1 业务，而是因为：
     - 它对这批字体的历史证据最多
     - 当前 working tree 脏面也仍与这条线纠缠最深
     - 但这只是临时 integrator 选择，不等于长期 owner 已经改判
- 当前恢复点：
  - 下一步不再给 `spring-day1` 发任何 `Primary` prompt；
  - 只发这条新的共享字体稳定性 prompt；
  - 等回执回来后，再判断：
    1. 能否整根回到 `HEAD`
    2. 还是必须单独继续做 importer 稳定化 deeper slice。

## 2026-03-31｜共享 TMP 字体 churn 已回到 `HEAD`，当前仓库只剩 `SpringUI` memory 尾账

- 当前主线目标：
  - 用户继续要求我“直接开始下一步”；
  - 在共享 TMP 中文字体 6 资产也已回到 `HEAD` 后，当前仓库现场只剩最后一条 dirty：`SpringUI` 工作区 memory。
- 本轮子任务：
  1. 核当前剩余 dirty 是否真的只剩 `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\memory.md`
  2. 把这最后一条 docs-only 尾账压成可直接发出的硬切片 prompt
- 本轮已完成：
  1. 已确认当前仓库 `git status --short` 只剩：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\memory.md`
  2. 已确认这条 dirty 的内容只是：
     - `2026-03-31` 那条 `Primary` 迁移意图只读裁定补记
     - 不是新的业务实现
  3. 已新增最后一条 docs-only prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-31_典狱长_UI-V1_SpringUI记忆尾账docs-only归仓_02.md`
- 当前关键判断：
  1. 这轮已经不是业务收口，而是最后一条工作区记忆尾账收口。
  2. 这条线应发给 `UI-V1 / SpringUI`，不是因为它又要继续做 UI，而是因为这份 memory 本来就属于它自己的工作区。
  3. 如果这条 docs-only 也过线，那么当前 Sunset 仓库现场就会进入真正的整仓 clean。
- 当前恢复点：
  - 下一步只发这条 `SpringUI` memory docs-only prompt；
  - 回执回来后，再做最后一次整仓 clean 复核。

## 2026-03-31｜共享 TMP 中文字体 6 资产已整根回到 `HEAD` 基线

- 当前主线目标：
  - 不再继续 `Day1 / UI / Primary`；
  - 当前唯一主线是按 `2026-03-31_典狱长_spring-day1_TMP中文字体稳定性回到已提交基线判定_02.md`，只判并处理 6 份共享 TMP 中文字体 dirty 能不能整根安全回到已提交基线。
- 本轮子任务：
  1. 复核 `TMP 中文字体稳定性` 立案、治理 memory、线程 memory、父工作区 memory
  2. 核对这 6 份资产当前 dirty 的结构是否符合 importer / atlas / glyph churn
  3. 若可安全回基线，则只把这 6 份资产恢复到 `HEAD`
- 本轮已完成：
  1. 已确认当前 churn 的 6 份资产正是：
     - `DialogueChinese BitmapSong SDF.asset`
     - `DialogueChinese Pixel SDF.asset`
     - `DialogueChinese SDF.asset`
     - `DialogueChinese SoftPixel SDF.asset`
     - `DialogueChinese V2 SDF.asset`
     - `LiberationSans SDF - Fallback.asset`
  2. 已确认当前 diff 仍是大规模 atlas / glyph / character 膨胀：
     - `15133 insertions`
     - `410 deletions`
  3. 已确认 `HEAD` 中这 6 个文件本身全部存在，且外部 live 引用仍指向这些同路径、同 GUID 的资产身份，而不是依赖未提交 churn 的新文件身份
  4. 已实际执行：
     - 只把这 6 份资产恢复到 `HEAD`
  5. 恢复后已复核：
     - 这 6 份文件 `git status` 为空
     - `Assets/TextMesh Pro/Resources/Fonts & Materials` 下这批 dirty 已清空
- 当前关键判断：
  1. 这轮结果应记为 `A`
  2. 当前可以安全回基线的根因，不是“这些字体不再被业务引用”，而是：
     - 业务仍然引用的是这 6 个文件本身
     - 恢复到 `HEAD` 不会改路径、不改 GUID、不改外部引用链
     - 当前 dirty 主要表现为 importer / atlas / glyph 副产物膨胀，而不是未提交的业务成果
  3. 这轮没有触碰：
     - `Primary`
     - `DialogueChineseFontAssetCreator.cs`
     - 业务代码
     - prefab
     - scene
- 当前恢复点：
  - 共享 TMP 中文字体这 6 资产当前已回到已提交基线；
  - 如果后续还要继续这个案子，不该再从“带着当前 churn 继续业务 sync”进入，而应在独立稳定化切片里重新判断 importer 风险。

## 2026-03-31｜高并发 live 状态层第一版已物理落地并通过 smoke

- 当前主线目标：
  - 用户不再接受“继续讨论方案”，要求把 `A 类热文件硬锁 + B 类共享触点声明 + C 类 own slice 自由施工 + 退出窗口 sync/park` 真正落成可执行脚本。
- 本轮子任务：
  1. 在仓库内新增 thread-state 脚本与 schema
  2. 让这套脚本至少完成第一轮真实 smoke，而不是停留在 Markdown 规则
  3. 把会导致假阳性/假 blocker 的脚本级问题一并修掉
- 本轮已完成：
  1. 已新增：
     - `D:\Unity\Unity_learning\Sunset\.kiro\state\active-threads.schema.json`
     - `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\StateCommon.ps1`
     - `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
     - `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
     - `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
     - `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Show-Active-Ownership.ps1`
  2. 已更新 `.gitignore`，忽略：
     - `.kiro/state/active-threads/`
     - `.kiro/state/*.lock`
  3. 已修掉首轮真实执行里暴露出的 3 类脚本问题：
     - `UTF-8 无 BOM` 导致 `powershell -File` 误解码、中文字符串触发假性 parser error
     - 数组返回值在严格模式下被塌缩成 `$null / 单字符串`，继续读 `.Count` 会炸
     - `Show-Active-Ownership -AsJson` 在零线程时静默空输出，而不是稳定返回空数组
  4. 已完成最小 smoke：
     - `Show-Active-Ownership.ps1`：空状态可正常输出
     - `Begin-Slice.ps1`：C 类切片可正常登记
     - `Ready-To-Sync.ps1`：对测试切片可稳定进入 `READY_TO_SYNC`
     - `Park-Slice.ps1`：可正常停车且不再写出 `[null] blockers`
     - B 类共享热点验证通过：
       - `GameInputManager.cs` 上不同 touchpoint 可并行登记
       - 同 touchpoint 会在 `Begin-Slice` 阶段直接阻断
  5. smoke 现场已清理：
     - `.kiro/state/active-threads/` 当前无测试残留
- 当前关键判断：
  1. 这轮已经不是“方案草案”，而是第一版可执行底座。
  2. 当前最重要的跃迁，不是脚本文件数增加，而是：
     - dirty 归属首次进入 live 状态层
     - 冲突从“7 天后治理清扫”前移到了 `Begin-Slice / Ready-To-Sync`
  3. 这轮还没有覆盖到 A 类真实锁烟测：
     - 原因不是脚本没接好，而是我刻意不在当前收口轮里制造新的 hot-file 锁历史噪音
     - 现阶段更重要的是先把状态层与 B 类并发判定跑稳
- 当前恢复点：
  - 下一步如果继续推进，不该再回到宏观争论；
  - 应只做两类落地：
    1. 把这套 thread-state 脚本接进线程开工 / 收口 prompt 与 runbook
    2. 再补一轮 A 类热文件真实锁链 smoke 或受控接线验证

## 2026-03-31｜thread-state 第一版已真实 sync 到 `main`

- 当前主线目标：
  - 在脚本与 smoke 通过后，不停在“本地实现”，而是判断这刀是否能作为独立白名单真实归仓。
- 本轮子任务：
  1. 对 `.gitignore + .kiro/state + .kiro/scripts/thread-state + Codex规则落地/memory.md` 跑最小 `preflight`
  2. 如果 own roots clean 且共享根允许，则直接 `sync`
- 本轮已完成：
  1. 已真实运行：
     - `preflight`
     - `sync`
  2. 已提交并推送：
     - `SHA = 5db71c57`
  3. 当前这刀 own roots 已 clean：
     - `.gitignore`
     - `.kiro/state`
     - `.kiro/scripts/thread-state`
     - `.kiro/specs/Codex规则落地`
- 当前关键判断：
  1. 这轮 claim 的不是“整仓 clean”，而是“并发状态层第一版已独立归仓成功”。
  2. 仓库里仍有剩余 dirty，但都不在这刀 own roots 内：
     - `NPC / 导航检查 / 导航检查V2 / 项目文档总览` 线程记忆
     - `.kiro/specs/屎山修复/**`
  3. 也就是说，这轮已经把“脚本本体能不能上基线”这件事单独做完了，不需要再等全仓清空才承认它成立。
- 当前恢复点：
  - 后续如果继续推进这套并发系统，应直接基于 `5db71c57` 这版已提交脚本继续；
  - 不再回到“还只是本地实验”的叙事。

## 2026-03-31｜thread-state 已是 live 规范，但 prompt 仍是过渡接线层

- 当前主线目标：
  - 用户继续追问一个关键边界：既然 `thread-state` 已经进了 `main`，那它到底只是“可选提示”，还是已经成为 Sunset 当前应该执行的 live 规范。
- 本轮子任务：
  1. 明确 `thread-state` 的规则层级
  2. 明确为什么现在还需要通过 prompt 去提醒线程使用
  3. 给出当前最准确的落地口径
- 本轮已确认：
  1. `thread-state` 现在已经不是“草稿”或“仓库里放着的工具”：
     - 它已真实进入 `main`
     - 因此应按 Sunset 当前 live 执行层的一部分看待
  2. 但它还没有自动强制接进每条线程的开工/收口动作：
     - 还没写进所有线程现成 prompt 模板
     - 还没并入现成 runbook / launcher / 固定回执流程
  3. 所以当前准确口径不是：
     - `只能靠 prompt 提醒`
     - 也不是：
     - `已经完全自动执行、谁都不用提醒`
  4. 当前真正的状态是：
     - `规范已经成立`
     - `执行接线还在过渡期`
- 当前关键判断：
  1. 从规则层说，它已经是全局 Sunset live 规范的一部分。
  2. 从执行层说，之所以现在还要发 prompt 提醒，是因为：
     - 线程不会自动知道“从这轮开始必须先 Begin-Slice”
     - 现有分发文本、开工话术和收口话术还没全部改写
  3. 也就是说，prompt 现在扮演的是“把新规范接到旧线程现场”的过渡层，而不是在替代规范本身。
- 当前恢复点：
  - 后续正确方向不是继续把它当“靠自觉提醒”的软建议；
  - 而是尽快把它写进：
    1. 线程开工 prompt 模板
    2. 收口 / sync 前固定动作
    3. 治理位的默认检查口径

## 2026-04-01｜thread-state 已正式写入 AGENTS / 规范快照 / 治理正文

- 当前主线目标：
  - 用户继续追问“为什么不是强制 skills”，并明确表示自己已经看不懂当前规范；
  - 这轮主线因此不是继续口头解释，而是把 `thread-state` 正式同步进 Sunset 的三层 live 规则源。
- 本轮子任务：
  1. 把 `Begin-Slice / Ready-To-Sync / Park-Slice / Show-Active-Ownership` 写入 `AGENTS.md`
  2. 同步写入 `Sunset当前规范快照_2026-03-22.md`
  3. 新增一份治理正文，专门说明这套并发登记与收口规范
  4. 明确解释“为什么不是只靠强制 skill”
- 本轮已完成：
  1. 已更新 [AGENTS.md](/D:/Unity/Unity_learning/Sunset/AGENTS.md)，把三步登记与过渡规则写成 live 硬动作
  2. 已更新 [Sunset当前规范快照_2026-03-22.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset当前规范快照_2026-03-22.md)，新增 `5D. 当前并发登记与退出口径`
  3. 已新增治理正文：
     - [并发thread-state开工与收口规范_2026-03-31.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/并发thread-state开工与收口规范_2026-03-31.md)
  4. 已在正文中明确钉死：
     - `skill` 是提示与协助层
     - `AGENTS / 规范快照 / 治理正文` 是 live 规则层
     - `thread-state` 脚本是现场执行层
- 当前关键判断：
  1. 用户这次看不懂，不是因为理解力问题，而是因为之前确实只把脚本落盘了，还没把“规则层级”写死。
  2. 当前最重要的修正，不是再造新机制，而是把这套已经落地的机制正式写成：
     - 规则源
     - 执行入口
     - 过渡接线方式
  3. 从这轮之后，`thread-state` 不应再被理解成“工具箱里一个可选脚本”。
- 当前恢复点：
  - 下一步不该再争论“是不是规范”；
  - 下一步只剩两件事：
    1. 把旧线程陆续补登记
    2. 把新线程 prompt 模板全部切到这套开工 / 收口口径
