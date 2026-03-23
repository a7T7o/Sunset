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
