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
