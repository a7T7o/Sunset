# Codex规则落地线程记忆补卷 6

## 会话 43 - 2026-03-19（跳出整个 Codex规则落地 工作区后的方向重审）
**用户目标**：
> 不满足于“阶段 23 里继续反思”，而是要求我彻底跳出 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地` 整个工作区，重审它的总体方向是不是本身就偏了。

**已完成事项**：
1. 回读 `Codex规则落地` 根层结构、`memory.md`、`design.md`、批次分发规范和现有阶段分布。
2. 明确认定：
   - 这条工作区前半段方向是对的，解决了 Sunset 的失控问题。
   - 但它后半段开始把治理工作区变成运行时调度总线，这一方向正在偏。
3. 抓到三个工作区级证据：
   - 根层 `memory.md` 仍停留在 `21` 口径，说明整工作区的统一活入口已经失真。
   - tracked Markdown 被过度当成运行时介质。
   - 当前优化目标已经从“恢复开发”漂移到“继续优化治理流程自身”。

**关键决策**：
- `Codex规则落地` 现在应被重新定性为：
  - 安全治理与事故恢复工作区
  - 而不是高吞吐执行工作区
- 下一步不应继续沿着 queue / receipt / batch 文档扩写。
- 应改开新的后续阶段，专门做执行模型与吞吐架构重构。

**恢复点 / 下一步**：
- 这次重判需要同步回治理主线，并修正根层 `memory.md` 仍停在阶段 21 的失真摘要。
- 回答用户时，必须明确承认：
  - 方向前半段对
  - 后半段已经开始偏
  - 现在应该换题，而不是继续在当前工作区内部越修越厚。

## 会话 44 - 2026-03-19（执行层新主线已正式立项）
**用户目标**：
> 既然方向已经重判成立，就不要停在聊天里，直接开始执行，把新的执行层主线真正建起来。

**已完成事项**：
1. 新建 `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构`。
2. 落盘第一版 `requirements.md / design.md / tasks.md / memory.md`。
3. 在 `Codex规则落地` 根层补写迁移说明，明确旧工作区今后只保留安全治理与事故恢复职责。

**关键决策**：
- 不再把新主线继续记成 `Codex规则落地` 的后续阶段。
- 执行层问题从现在起单独承接、单独推进。

**恢复点 / 下一步**：
- 继续在新工作区内完成瓶颈盘点、线程生命周期和持槽边界设计。

## 会话 45 - 2026-03-19（执行层第一轮物理落地完成）
**用户目标**：
> 交接同步后，不再分轮审核，要求我把新工作区的任务一路做到可落地版本，只看最终结果。

**已完成事项**：
1. 为 `git-safe-sync.ps1` 增加 ignored active session runtime、持槽窗口输出、return-main 持槽统计和 governance preflight 纠偏。
2. 完成新工作区的支撑文档：
   - 瓶颈盘点
   - 旧阶段问题域映射
   - rollout / 负例矩阵
3. 同步收紧 `AGENTS.md`、`shared-root-queue.md` 和治理分发规范。

**关键决策**：
- 新主线第一轮不再停在概念文档，而是已经开始物理改变执行层协议。
- 当前剩余工作以实盘验证为主，而不是继续补职责边界。

**恢复点 / 下一步**：
- 完成记忆与技能日志同步后，执行 governance sync，把这轮改造收进 `main`。

## 会话 46 - 2026-03-19（执行层收口前的活入口纠偏）
**用户目标**：
> 不要只停在“已经改了脚本和文档”，而要把仍会误导后续线程的旧口径一起修掉，再一次性收口。

**已完成事项**：
1. 复核新工作区、旧治理工作区和 queue 说明的 live 口径一致性。
2. 发现并修正 `shared-root-queue.md` 中“waiting 线程仍需回写 tracked memory_0.md”的旧说法，使其与 `AGENTS.md` 和执行层新设计一致。
3. 把 `共享根执行模型与吞吐重构/memory.md` 的状态从“刚立项”更新为“第一轮物理协议已落地”，避免后续读者误判当前成熟度。
4. 把 `Codex规则落地/memory.md` 的根层活入口改成“安全治理层已收口，执行层已迁出”的真实状态。

**关键决策**：
- 当前可以客观宣称：新主线已经从概念设计推进到第一轮物理落地。
- 但不能夸大成“并发吞吐问题已全部解决”；后续仍需 rollout 和持槽压缩验证。

**恢复点 / 下一步**：
- 立即执行 governance sync，把本轮最终收口版同步进 `main`。

## 会话 47 - 2026-03-19（第二轮执行层补丁：Draft 沙盒与 post-return 策略）
**用户目标**：
> 审核 Gemini 对新工作区的鼓励性建议，但不要被夸大带偏；要求我查漏补缺后直接进入下一轮物理改造。

**已完成事项**：
1. 采纳并落地 Draft 沙盒：
   - `.codex/drafts/README.md`
   - `.gitignore` 对 `.codex/drafts/**` 的忽略规则
2. 为 `git-safe-sync.ps1` 增加 Draft 提示输出：
   - waiting / granted / wake-next / return-main 均可直接给出草稿目录和文件 hint
3. 纠正 `return-main` 后的误导性想象：
   - 不再默认暗示“归还槽位后立刻安全写 tracked memory”
   - 改为根据队列是否仍有人等待，输出不同的 post-return 证据策略

**关键决策**：
- Gemini 的大方向可采纳，但“return-main 后就安全写 tracked 复盘”对本项目并不严谨；真实可用版本必须把队列状态也算进去。
- 这轮补丁的核心不是增加更多规则，而是减少 shared root 释放后的二次污染。

**恢复点 / 下一步**：
- 先做 parser、ignore 规则和 preflight 验证，再执行治理同步。

## 会话 48 - 2026-03-19（第二轮执行层补丁已收口为 live 基线）
**用户目标**：
> 要求我把本轮第二次执行层补丁真正收口成最终版，不留“本地已改但还没生效”的尾巴。

**已完成事项**：
1. 验证：
   - `git-safe-sync.ps1` parser 正常
   - Draft 沙盒 ignore 规则正常
   - governance preflight 正常
2. 治理同步：
   - 提交 `2026.03.19-16`
   - `main @ 306b29ee109cc696234f73416a625eb318a131c1`
3. stable launcher 复核：
   - 已读取 `main` 上的新 canonical script
   - 当前 `git status` clean

**关键决策**：
- 这轮关于 Draft 沙盒和 post-return 分流的补丁已经正式生效，不再只是设计稿。
- 当前可以基于新口径进入小批次、低风险开发准入；但还不能夸大成“高吞吐并发体系已经终局完成”。

**恢复点 / 下一步**：
- 下一步应生成新一轮 smoke test / 开发准入批次，而不是继续补旧 prompt。

## 会话 49 - 2026-03-19（新版 smoke test 批次已生成）
**用户目标**：
> 对 Gemini 的“现在直接发旧 4 份 prompt 开跑”做锐评审核；除非 Path C，否则直接按更稳的判断继续做。

**已完成事项**：
1. 路径判断为 `Path B`：
   - Gemini 对方向的赞许可采纳
   - 但“旧 prompt 可直接复用”不采纳
2. 已生成新版分发资产：
   - 根层批次文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_04_执行层smoke-test_01.md`
   - 新工作区专属 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\2026.03.19_smoke-test_01\可分发Prompt\*.md`
   - 新工作区治理镜像回收卡：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\2026.03.19_smoke-test_01\线程回收\*.md`
3. 批次内容已包含：
   - Draft 沙盒
   - `POST_RETURN_EVIDENCE_MODE`
   - waiting / granted 全部禁止 tracked 写入

**关键决策**：
- 这轮继续推进，但不是盲从 Gemini 的“直接复用旧批次”。
- 当前真正可群发的是 smoke test 01，而不是阶段 23 里的旧 prompt。

**恢复点 / 下一步**：
- 同步这批新分发文件进 `main`，然后即可对外发车。

## 会话 50 - 2026-03-19（smoke-test_01 事故回收与 shared root 回正）
**用户目标**：
> 四条 smoke test 线程都已回复，要求我直接开始回收、恢复现场，并把当前治理主线继续推进到可再次调度的状态。

**已完成事项**：
1. 复核 live 现场，确认 shared root 并未正常完成本轮 smoke test，而是卡在：
   - `codex/navigation-audit-001 @ 71905387`
   - `owner_thread = 导航检查`
   - 另有 `NPC / 农田交互修复V2 / 遮挡检查` 三条 waiting 条目
2. 用 stable launcher `preflight` 明确验证：
   - 阻断 `return-main` 的 remaining dirty 正是其他 waiting 线程写入的 `.codex/drafts/**`
3. 在本机仓库 `D:\Unity\Unity_learning\Sunset\.git\info\exclude` 增加 `.codex/drafts/**` 作为旧分支兼容兜底
4. 随后重新执行：
   - `sunset-git-safe-sync.ps1 -Action return-main -OwnerThread '导航检查'`
   成功把 shared root 恢复到：
   - `main @ c09ac560`
   - `git status --short --branch = ## main...origin/main`
   - `shared-root-branch-occupancy.md = main + neutral`
   - active session 清空
5. 补写执行层工作区 `smoke-test_01` 的四张治理镜像回收卡，并把“旧分支 Draft 忽略兜底”写回执行层工作区任务 / 记忆。

**关键决策**：
- 本轮失败的核心不是 Draft 沙盒方向错误，而是旧 continuation branch 尚未带上新版忽略规则，导致 waiting Draft 会反向阻断持槽线程退场。
- `导航检查` 这轮虽然最终被救回，但也暴露出持槽 11.25 分钟、显著超出 `docs-fast-lane` 推荐 3 分钟的吞吐问题；后续 rollout 仍要继续压缩持槽窗口。
- 当前我不在 shared root 仍有 waiting 时直接写更多 tracked 治理杂项，而是先做最小回收、最小同步，再决定是否唤醒队首。

**恢复点 / 下一步**：
- 立即对白名单治理文件执行 `governance sync`，把这轮回收与恢复结论安全收进 `main`。
- 同步完成后，shared root 继续保持 `main + neutral`，再根据 clean 现场决定是否 `wake-next -> NPC`。

## 会话 51 - 2026-03-19（NPC 续跑与第二次真实闭环）
**用户目标**：
> 不再停在“可以继续”的解释层，而是继续原计划，直接沿队列推进 `NPC`：先 `wake-next`，再让它消费 grant 并完成这一轮 smoke test 只读闭环。

**已完成事项**：
1. 在 shared root `main + neutral + clean` 的前提下执行：
   - `sunset-git-safe-sync.ps1 -Action wake-next -OwnerThread 'Codex规则落地'`
   脚本返回 `STATUS: WOKE_NEXT`，成功向 `NPC` 发放 `codex/npc-roam-phase2-003` 的 grant
2. 收到 `NPC` 最小回执，确认其完成：
   - `request-branch = ALREADY_GRANTED`
   - `ensure-branch = 成功`
   - `return-main = 成功`
   - `post_return_evidence_mode = defer-tracked-while-queue-waiting`
3. live 再次复核后确认：
   - `main + neutral`
   - queue 中 `ticket 3 / NPC = completed`
   - 下一位队首 waiting 已变成 `农田交互修复V2`
4. 按执行层规则，本轮 tracked 回收仍由治理线程统一补写，不让 `NPC` 在线程侧把 `main` 写脏。

**关键决策**：
- 现在已经连续有两条线完成真实闭环：`导航检查` 与 `NPC`。
- 当前最合理的节奏仍是：
  - 先最小 tracked 回收
  - 立刻治理同步回 `main`
  - 再继续 `wake-next -> 农田交互修复V2`

**恢复点 / 下一步**：
- 先把 `NPC` 这轮成功闭环补入执行层 / 治理层记忆与回收卡
- 然后执行最小 `governance sync`
- 同步完成后直接继续到 `农田交互修复V2`

## 会话 52 - 2026-03-19（农田续跑与第三次真实闭环）
**用户目标**：
> 继续沿 `smoke-test_01` 队列推进 `农田交互修复V2`，不要因为说明或审核中断节奏。

**已完成事项**：
1. 在 `NPC` 回收后确认 shared root 再次处于 `main + neutral + clean`
2. 执行：
   - `sunset-git-safe-sync.ps1 -Action wake-next -OwnerThread 'Codex规则落地'`
   成功向 `农田交互修复V2` 发放 `codex/farm-1.0.2-cleanroom001` 的 grant
3. 收到 `农田交互修复V2` 最小回执，确认其完成：
   - `request-branch = ALREADY_GRANTED`
   - `ensure-branch = 成功`
   - `return-main = 成功`
   - `post_return_evidence_mode = defer-tracked-while-queue-waiting`
4. 补充保留了一条对业务判断有价值的只读事实：
   - `FarmManager.cs` 当前不存在
5. live 再次复核后确认：
   - `main + neutral`
   - queue 中 `ticket 4 / 农田交互修复V2 = completed`
   - 当前只剩 `遮挡检查` 一条 waiting

**关键决策**：
- 到这里已经连续有三条线程完成真实闭环：`导航检查`、`NPC`、`农田交互修复V2`
- 当前最合理的节奏不变：
  - 先最小 tracked 回收
  - 治理同步回 `main`
  - 再继续 `wake-next -> 遮挡检查`

**恢复点 / 下一步**：
- 先把农田这轮成功闭环补入执行层 / 治理层记忆与回收卡
- 然后执行最小 `governance sync`
- 同步完成后直接继续到 `遮挡检查`
