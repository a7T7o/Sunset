# 共享根执行模型与吞吐重构 - 开发记忆

## 模块概述
- 本工作区承接 Sunset 在完成安全治理层之后，暴露出的“执行层吞吐与调度模型错位”问题。
- 它不负责推翻现有安全基线，而是负责定义 shared root 在安全前提下怎样更高效地被多个线程使用。

## 当前状态
- **完成度**：65%
- **最后更新**：2026-03-19
- **状态**：第一轮执行层协议已落地，当前处于治理同步与低风险实盘验证前基线

## 会话记录

### 会话 1 - 2026-03-19
**用户需求**：
> 接受“`Codex规则落地` 前半段对、后半段偏”的重判，要求不要只停在结论上，而是直接开始执行，把真正的新主线落成新的工作区。

**完成任务**：
1. 新建本工作区，正式把主问题改题为“执行模型与吞吐重构”。
2. 形成第一版 `requirements.md / design.md / tasks.md`。
3. 确定本工作区与 `Codex规则落地` 的分工：
   - 旧工作区继续负责安全治理与事故恢复。
   - 新工作区开始承接执行层重构。
4. 明确当前第一轮设计重点：
   - shared root 只做最小写事务
   - waiting 线程不再实时污染 tracked repo
   - 运行态与证据层分离

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\requirements.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\design.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\memory.md`

**关键决策**：
- 不再把 `Codex规则落地` 继续扩写成高频运行时协调总线。
- 新主线单独立项，不再把阶段号硬接在旧治理工作区后面。
- 当前设计优先级不是“再加 queue 规则”，而是“先定义执行层分层和持槽边界”。

**遗留问题 / 下一步**：
- 继续完成任务 2、3、4：
  - 盘点真实吞吐瓶颈
  - 定义线程生命周期
  - 定义持槽时间边界

### 会话 2 - 2026-03-19
**用户需求**：
> 不要停在工作区搭建上，而是继续把新主线相关的问题直接修到可落地版本，中间过程不再逐轮审核，只看最终版。

**完成任务**：
1. 修改 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`：
   - 新增 ignored active session runtime
   - 为 `request-branch / wake-next / ensure-branch / return-main / sync` 增加执行层字段输出
   - 修正 governance preflight 的误导性 task lease 显示
2. 补齐执行层支撑文档：
   - `基线瓶颈盘点_2026-03-19.md`
   - `前序阶段问题域映射_2026-03-19.md`
   - `负例矩阵与rollout清单_2026-03-19.md`
3. 收紧项目级执行口径：
   - `AGENTS.md`
   - `shared-root-queue.md`
   - `治理线程批次分发与回执规范.md`

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
- `D:\Unity\Unity_learning\Sunset\AGENTS.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-queue.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\治理线程批次分发与回执规范.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\基线瓶颈盘点_2026-03-19.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\前序阶段问题域映射_2026-03-19.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\负例矩阵与rollout清单_2026-03-19.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\tasks.md`

**关键决策**：
- active session 与持槽窗口采用 ignored runtime 承载，不再污染 tracked repo。
- waiting 线程的恢复点优先进入 queue runtime 和最小聊天回执，不再默认回写 tracked memory。
- 治理线程继续负责批次入口与事后审计，不再承担高频人工消息总线。

**遗留问题 / 下一步**：
- 接下来主要剩实盘验证与后续按结果微调，不再是方向未定或职责未分的问题。

### 会话 3 - 2026-03-19
**用户需求**：
> 不再中途审核，要求我把这条新主线直接收口成可继续开发的最终版，并把仍会误导后续线程的旧口径一起修完。

**完成任务**：
1. 再次复核 live 基线与 working tree，确认当前待同步改动全部围绕：
   - 执行层 active session / 持槽窗口协议
   - waiting 线程不写 tracked 运行态
   - 治理线程回到“入口 + 审计”职责
2. 修正文档层残留冲突：
   - 修正 `shared-root-queue.md` 早段仍要求 waiting 线程回写 `memory_0.md` 的旧口径
   - 统一到 `AGENTS.md` 与新执行层设计的“CheckpointHint / QueueNote / 最小聊天回执”口径
3. 把本工作区的状态从“刚立项”更新为“第一轮物理落地已完成”，不再让读者误以为这里只停留在概念阶段。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-queue.md`

**关键决策**：
- 当前新工作区可以客观宣称“第一轮物理协议已落地”，但还不能夸大为“吞吐问题已彻底解决”。
- 下一步主线应是低风险 rollout 与持槽时长压缩验证，而不是回头继续扩写旧治理工作区。

**遗留问题 / 下一步**：
- 通过治理同步把本轮改动收进 `main`。
- 同步后以 launcher 复核 canonical script 生效，再进入执行层实盘验证。

### 会话 4 - 2026-03-19
**用户需求**：
> 带着审核态度消化 Gemini 对新工作区的判断，查漏补缺后直接开始下一轮物理改造，不再停在理论框架。

**完成任务**：
1. 审核后采纳 Gemini 的两个有效方向：
   - waiting 线程需要一个真正免疫 Git 脏检查的 Draft 沙盒
   - `return-main` 需要更明确的事后动作提示
2. 同时纠正 Gemini 被放大的部分：
   - `request-branch / wake-next` 的 runtime 化并不是“现在才要做”，而是此前已经落地
   - `return-main` 后也不能简单地说“现在可以安全写 tracked memory”，因为这会在队列未清空时再次把 `main` 写脏
3. 物理落地补丁：
   - `git-safe-sync.ps1` 新增 Draft 沙盒路径输出
   - `LOCKED_PLEASE_YIELD / GRANTED / ALREADY_GRANTED / wake-next / return-main` 输出 Draft 提示
   - `return-main` 新增 `POST_RETURN_EVIDENCE_MODE / POST_RETURN_NEXT_ACTION`
   - 新建 gitignored Draft 沙盒规范：`D:\Unity\Unity_learning\Sunset\.codex\drafts\README.md`
   - `.gitignore` 正式忽略 `.codex/drafts/**`

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
- `D:\Unity\Unity_learning\Sunset\.gitignore`
- `D:\Unity\Unity_learning\Sunset\.codex\drafts\README.md`
- `D:\Unity\Unity_learning\Sunset\AGENTS.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-queue.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\requirements.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\design.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\memory.md`

**关键决策**：
- Draft 沙盒是对“等待态不污染 Git”的补完，不是替代正式记忆或正式代码提交。
- `return-main` 后的 tracked 证据写入必须区分：
  - 队列仍有人等待：优先 Draft / 最小聊天回执，延后 tracked 落盘
  - 队列为空：允许最小 tracked 收口

**遗留问题 / 下一步**：
- 做 parser、`.gitignore` 与 live preflight 验证。
- 验证通过后执行治理同步，把第二轮执行层补丁并入 `main`。

### 会话 5 - 2026-03-19
**用户需求**：
> 不要停在“补丁已改完”，而要把本轮第二次执行层补丁真正同步成 live 基线。

**完成任务**：
1. 验证通过：
   - `git-safe-sync.ps1` parser 正常
   - `.codex/drafts/example.md` 被 `.gitignore` 正确忽略
   - `.codex/drafts/README.md` 保持可跟踪
   - governance `preflight` 允许继续
2. 通过治理白名单同步把第二轮补丁收进 `main`：
   - 提交：`2026.03.19-16`
   - HEAD：`306b29ee`
3. 用 stable launcher 复核 canonical script：
   - `LAUNCHER_MODE: stable-canonical-script`
   - `HEAD: 306b29ee`
   - 当前无待同步改动

**关键决策**：
- 这轮补丁现在不只是 working tree 改动，而是已经成为 shared root 的 live 规则。
- Draft 沙盒与 post-return 分流策略正式进入 Sunset 当前执行模型基线。

**遗留问题 / 下一步**：
- 下一步不再是补这轮脚本，而是基于新口径生成小批次、低风险 smoke test / 开发准入。

### 会话 6 - 2026-03-19
**用户需求**：
> 对 Gemini 的“直接复用旧 4 份 prompt 开跑”做审核，如果不是 Path C 就按更稳的判断继续推进。

**完成任务**：
1. 走 `Path B`：
   - 认可 Gemini 对方向的肯定
   - 但否定“旧 4 份 prompt 可直接复用”的做法
2. 新建第一轮真正匹配当前 live 规则的 smoke test 批次：
   - 根层批次入口：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_04_执行层smoke-test_01.md`
   - 新执行层工作区中的 prompt / 收件箱：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\2026.03.19_smoke-test_01\可分发Prompt\`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\2026.03.19_smoke-test_01\线程回收\`
3. 批次内容已升级为：
   - waiting 线程必须落 Draft note
   - granted 线程必须回报 `POST_RETURN_EVIDENCE_MODE`
   - 全部线程本轮都禁止 tracked 写入

**关键决策**：
- 旧批次 prompt 仍可作历史证据，但不再作为当前 smoke test 的 live 入口。
- 这轮 smoke test 测的是“Draft 挂起 + return-main 分流”，不是只重复旧版 queue 行为。

**遗留问题 / 下一步**：
- 将本轮 smoke test 批次同步进 `main`。
- 同步后即可直接群发新版批次入口。

## 关键决策

| 决策 | 原因 | 日期 |
|------|------|------|
| 新开独立工作区承接执行层重构 | `Codex规则落地` 已不适合继续膨胀为运行时调度层 | 2026-03-19 |
| 保留旧治理层，不推翻安全基线 | 现有硬闸机对安全问题已经有效 | 2026-03-19 |

## 相关文件

| 文件 | 说明 |
|------|------|
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\requirements.md` | 新主线需求边界 |
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\design.md` | 新执行层分层设计 |
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\tasks.md` | 新主线任务清单 |
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md` | 旧治理工作区的重判与交接背景 |

### 会话 7 - 2026-03-19（smoke-test_01 回收与 shared root 恢复）
**用户需求**：
> 四条 smoke test 线程都已回复，需要治理线程开始回收，并把当前卡死现场真正恢复成可继续调度的状态。

**已完成事项**：
1. 只读复核 live 现场，确认 shared root 并没有正常完成第一轮 smoke test，而是卡在：
   - `codex/navigation-audit-001 @ 71905387`
   - `owner_thread = 导航检查`
   - queue 中另有 `NPC / 农田交互修复V2 / 遮挡检查` 三条 waiting
2. 复核 `preflight` 与 Draft 证据，锁定本轮新暴露的 P1 根因：
   - waiting 线程写入的 `.codex/drafts/**` 在 `导航检查` 所处旧分支上未被忽略
   - 这些 Draft 被 `return-main` 误判成 remaining dirty，从而反向阻断退场
3. 采用最小恢复方案，在仓库本地 `D:\Unity\Unity_learning\Sunset\.git\info\exclude` 增加：
   - `.codex/drafts/**`
   以兼容仍未合入新版 `.gitignore` 的旧 continuation branch
4. 在该兜底生效后重新执行：
   - `sunset-git-safe-sync.ps1 -Action return-main -OwnerThread '导航检查'`
   并成功返回：
   - `STATUS: RETURNED_MAIN`
   - `NEXT_IN_LINE_OWNER_THREAD: NPC`
   - `POST_RETURN_EVIDENCE_MODE: defer-tracked-while-queue-waiting`
5. 恢复后现场已回正为：
   - `main @ c09ac560`
   - `git status --short --branch = ## main...origin/main`
   - `shared-root-branch-occupancy.md = main + neutral`
   - `shared-root-active-session.lock.json = absent`
   - queue 中 `导航检查` 已完成，`NPC / 农田交互修复V2 / 遮挡检查` 保持 waiting
6. 同步补完本轮 `smoke-test_01` 的四张治理镜像回收卡，确保“聊天事实”转成仓库内治理事实。

**关键决策**：
- 这次失败并不推翻 Draft 沙盒方案；真正的缺口是“旧 continuation branch 对 Draft 的忽略策略不完整”。
- shared root 的恢复动作必须先于下一位线程唤醒；否则队列会在错误基线上继续放大问题。
- `导航检查` 这轮暴露出的另一个有效信号是持槽过长：`docs-fast-lane` 推荐 3 分钟，但实际持槽达到 11.25 分钟，后续 rollout 必须继续压缩持槽窗口。

**恢复点 / 下一步**：
- 先对白名单治理文件做一次 `governance sync`，把本轮回收与恢复结论收进口径层。
- 同步完成后，shared root 应继续保持 `main + neutral`，再决定是否执行 `wake-next` 唤醒 `NPC`。

### 会话 8 - 2026-03-19（NPC smoke-test_01 续跑成功闭环）
**用户需求**：
> 在 `导航检查` 退场恢复完成后，不停留在解释层，继续沿队列消费下一位，也就是先 `wake-next -> NPC`，再准备发给 NPC 的续跑 prompt。

**已完成事项**：
1. 在确认 shared root 仍是 `main + neutral + clean` 后，治理线程执行：
   - `sunset-git-safe-sync.ps1 -Action wake-next -OwnerThread 'Codex规则落地'`
   并成功向 `NPC` 发放：
   - `codex/npc-roam-phase2-003`
2. `NPC` 按续跑 prompt 完成：
   - `request-branch = ALREADY_GRANTED`
   - `ensure-branch = 成功`
   - 对 `NPCAutoRoamController.cs / NPCBubblePresenter.cs / NPC_DefaultRoamProfile.asset` 的只读核对
   - `return-main = 成功`
3. 退场后现场再次恢复为：
   - `main + neutral`
   - queue 中 `ticket 3 / NPC = completed`
   - 下一位 waiting 已变成 `农田交互修复V2`
4. 按 `POST_RETURN_EVIDENCE_MODE = defer-tracked-while-queue-waiting` 口径，本轮由治理线程统一补写 NPC 的 tracked 回收卡，而不是让 NPC 在线程内直接把 `main` 写脏。

**关键决策**：
- 这说明 `wake-next -> ALREADY_GRANTED -> ensure-branch -> return-main` 这条续跑链路现在已经实盘闭环，不再只停留在设计层。
- 当前执行层的真实剩余队列已经缩减到两条：
  - `农田交互修复V2`
  - `遮挡检查`

**恢复点 / 下一步**：
- 先把 NPC 成功闭环的最小 tracked 回收与记忆同步进 `main`
- 同步完成后，继续 `wake-next -> 农田交互修复V2`

### 会话 9 - 2026-03-19（农田 smoke-test_01 续跑成功闭环）
**用户需求**：
> 在 `NPC` 完成后继续沿队列推进下一位，也就是 `农田交互修复V2`，不要中断主线。

**已完成事项**：
1. 在 `NPC` 回收完成并治理同步后，确认 shared root 再次处于 `main + neutral + clean`
2. 治理线程执行：
   - `sunset-git-safe-sync.ps1 -Action wake-next -OwnerThread 'Codex规则落地'`
   并成功向 `农田交互修复V2` 发放：
   - `codex/farm-1.0.2-cleanroom001`
3. `农田交互修复V2` 按续跑 prompt 完成：
   - `request-branch = ALREADY_GRANTED`
   - `ensure-branch = 成功`
   - 对 `FarmToolPreview.cs / PlacementManager.cs / FarmManager.cs` 的只读核对
   - `return-main = 成功`
4. 只读核对补充事实：
   - `FarmToolPreview.cs` 在位
   - `PlacementManager.cs` 在位
   - `FarmManager.cs` 当前不存在
5. 退场后现场再次恢复为：
   - `main + neutral`
   - queue 中 `ticket 4 / 农田交互修复V2 = completed`
   - 当前只剩 `遮挡检查` 一条 waiting

**关键决策**：
- 到这一步，`smoke-test_01` 已连续完成三条真实闭环：
  - `导航检查`
  - `NPC`
  - `农田交互修复V2`
- 当前执行层剩余的唯一 waiting 条目是 `遮挡检查`，说明这轮 smoke test 已经接近尾声。

**恢复点 / 下一步**：
- 先把农田成功闭环的最小 tracked 回收与记忆同步进 `main`
- 同步完成后，继续 `wake-next -> 遮挡检查`

### 会话 10 - 2026-03-19（遮挡 smoke-test_01 续跑成功，整轮 smoke test 完成）
**用户需求**：
> 在 `农田交互修复V2` 之后继续推进最后一个 waiting 条目 `遮挡检查`，并在其回执到位后完成整轮 `smoke-test_01` 的最终收口。

**已完成事项**：
1. 在 `农田` 回收完成并治理同步后，确认 shared root 再次处于 `main + neutral + clean`
2. 治理线程执行：
   - `sunset-git-safe-sync.ps1 -Action wake-next -OwnerThread 'Codex规则落地'`
   并成功向 `遮挡检查` 发放：
   - `codex/occlusion-audit-001`
3. `遮挡检查` 按续跑 prompt 完成：
   - `request-branch = ALREADY_GRANTED`
   - `ensure-branch = 成功`
   - `return-main = 成功`
   - `post_return_evidence_mode = minimal-tracked-allowed`
4. 只读核对结论：
   - 调查 carrier 在位
   - 关键文档与调查线索仍在位
5. 退场后现场再次恢复为：
   - `main + neutral + clean`
   - queue 中 `ticket 5 / 遮挡检查 = completed`
   - `shared-root-active-session.lock.json = absent`
   - 当前 waiting 条目已清空

**关键决策**：
- 到这一步，`smoke-test_01` 已完成四条真实闭环：
  - `导航检查`
  - `NPC`
  - `农田交互修复V2`
  - `遮挡检查`
- 这轮 smoke test 已经不再只是“验证排队会不会挂起”，而是完成了完整的实盘证明：
  - waiting Draft 不再污染 `main`
  - `wake-next -> ALREADY_GRANTED -> ensure-branch -> return-main` 可以连续多次成功闭环
  - 最后一条退场后，当队列清空时会正确返回 `minimal-tracked-allowed`

**恢复点 / 下一步**：
- 先把 `遮挡检查` 的成功闭环与“整轮 smoke-test_01 已完成”的结论同步进 `main`
- 同步完成后，shared root 应保持 `main + neutral + clean`，再决定是否进入下一批真实开发准入

### 会话 11 - 2026-03-19（smoke test 通过后的真实开发运行口径）
**用户需求**：
> 不再只关心 smoke test 本身，而是要把“这套交通系统测通以后，日常真实开发到底该怎么跑”说清楚，尤其是 NPC / 农田 / 导航 / spring-day1 / 遮挡 这些线该如何排。

**稳定结论**：
1. 治理线程不是以后每一行代码都要人工指挥的“交警”，而是批次边界、排队冲突、事故恢复和 tracked 回收的调度层。
2. 真实开发的工作方式应改成：
   - 多线程可以并行做只读分析、方案整理、Draft 草稿准备
   - 但 shared root 的真实写入 / 分支切换仍是单槽位串行
   - 也就是“并行准备，串行进槽”
3. 如果当前只有一条线程要工作，而且 shared root 已是 `main + neutral + clean`，该线程可以直接按协议执行：
   - `request-branch`
   - `ensure-branch`
   - 最小 checkpoint
   - `return-main`
   不一定每次都要先经过治理线程手工调度。
4. 治理线程真正必须介入的场景是：
   - 多条线程同时想进 shared root
   - 某条线程已经进入 waiting / granted / task-active 中间态
   - `return-main` 失败或 shared root 不再 neutral
   - 需要统一补 tracked 回收、阶段结论、治理同步
5. 对当前用户关心的业务优先级，推荐口径是：
   - 第一优先级：`NPC`
     - 因为它已明确存在场景报错 / meta 缺失疑点，且可能牵涉 scene / asset / Unity 层
     - 应作为单独高优先级真实开发批次先处理
   - 第二优先级：`农田交互修复V2`
     - 可以与 `NPC` 并行做只读准备与 Draft
     - 但真正占 shared root 写入时，排在 `NPC` 后
   - 第三优先级：`导航检查`
     - 应先于 `遮挡检查`
   - 第四优先级：`spring-day1`
     - 可以与导航并行做只读准备，但实际进槽顺序要看它是否涉及 Unity / MCP / 热文件
   - 第五优先级：`遮挡检查`
     - 放在导航之后，再进入真实整改
6. 当前最适合用户的实际操作模型不是“所有线程同时开写”，而是：
   - `NPC` 与 `农田` 并行准备
   - `NPC` 先拿 shared root 做真实修复
   - `NPC` 归还后，`农田` 进入
   - 同时让 `导航` 与 `spring-day1` 保持只读准备
   - `农田` 归还后，再根据 Unity / MCP 风险决定先 `导航` 还是 `spring-day1`
   - `遮挡检查` 保持在导航之后

**恢复点 / 下一步**：
- 下一步不再发 smoke prompt，而是生成“真实开发准入批次 01”
- 该批次应至少包含：
  - `NPC`：真实修复
  - `农田交互修复V2`：只读准备或排队等待

### 会话 12 - 2026-03-19（真实开发准入批次 01 已生成）
**用户需求**：
> 用户批准直接开始给 prompt，同时要求把“以后到底是不是一直找治理线程、NPC 和 farm 并行时具体会遇到什么情况、某条线程中断后怎么处理”说成具体场景，而不是停留在宽口径。

**本轮完成**：
1. 已新建真实开发批次目录：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\2026.03.19_真实开发准入批次_01\`
2. 已生成并可直接分发的 prompt 资产：
   - `NPC.md`
   - `农田交互修复V2_准备.md`
   - `农田交互修复V2_进槽续跑.md`
3. 已生成治理根层分发入口：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_05_真实开发准入批次_01.md`
4. 已生成固定回收卡：
   - `线程回收/NPC.md`
   - `线程回收/农田交互修复V2.md`

**关键判断**：
- 真正高效的第一批不是让 `NPC` 和 `农田` 同时抢 shared root，而是：
  - `NPC` 立即进槽做真实修复
  - `农田` 同时只读准备和写 Draft
- 这样做不是保守，而是因为 `NPC` 当前是明确的场景 / meta 阻断线，而 `农田` 可以在不占槽的情况下先把下一次最小 checkpoint 预写好。
- 如果用户强行同时让 `NPC` 和 `农田` 都发 `request-branch`，系统也不会坏：
  - 最多就是一个拿到 `GRANTED`
  - 另一个得到 `LOCKED_PLEASE_YIELD`
  - 等前者 `return-main` 后，再给后者发续跑 prompt 或 `wake-next`
  这属于“允许但低效”的模式。
- 线程“中断”要分两类：
  - 如果是在 waiting / prep 阶段中断：本质没有进槽，只要保住 Draft，后面直接续跑即可
  - 如果是在已 ensure-branch 的持槽阶段中断：必须先判断是否已经形成 checkpoint / sync；未收口前不能直接切下一位

**恢复点 / 下一步**：
- 当前最合理的真实开发顺序已经明确：
  - 先发 `NPC` 真修 prompt
  - 同时发 `农田` 准备态 prompt
  - `NPC` 完成后，再发 `农田交互修复V2_进槽续跑.md`

### 会话 13 - 2026-03-19（真实开发准入批次 01 回执已回收）
**用户需求**：
> 用户贴回 `NPC`、`农田交互修复V2_准备`、`农田交互修复V2_进槽续跑` 三张最小回执，要求治理线程正式收件并给出这一批真实开发到底推进到了哪一步。

**本轮完成**：
1. 已确认 live 现场仍为：
   - `main`
   - `neutral`
   - `clean`
2. 已依据回执与 queue runtime 确认：
   - `NPC` 本轮对应 `ticket 6 = completed`
   - `农田交互修复V2` 本轮对应 `ticket 7 = completed`
3. 已回填固定回收卡：
   - `线程回收/NPC.md`
   - `线程回收/农田交互修复V2.md`

**关键判断**：
- `NPC` 这轮不是“没做出结果”，而是把问题定性成了：
  - 当前 `main` 缺失 NPC phase2 的 Profile / Prefab / AutoRoam 运行时代码
  - continuation branch `codex/npc-roam-phase2-003` 内静态资源链已对齐
  - 所以下一步不应继续重复做同一类只读排查，而应转入“phase2 carrier 的集成 / 升格 / 合入策略”
- `农田交互修复V2` 这轮也不是“空转”，而是确认：
  - 首个无 A 类热文件 checkpoint 已经完整存在于 continuation carrier
  - 下一检查点才是真正的新工作面，而且会碰 `GameInputManager.cs`
  - 因此下一步不应再重复本轮 prompt，而应转入热文件阶段裁定
- 这说明真实开发准入批次 01 的价值在于“把两条线都推进到更准确的下一决策点”，而不是强行造出无意义的新补丁。

**恢复点 / 下一步**：
- `NPC` 后续应直接进入：
  - phase2 载体内容的真实集成批次
- `农田交互修复V2` 后续应直接进入：
  - `GameInputManager.cs` 热文件专项批次
- `导航检查` / `spring-day1` 仍可按原计划准备下一批，但不要与上述两条新决策点混写

### 会话 14 - 2026-03-19（真实开发准入批次 02 已生成）
**用户需求**：
> 用户批准不要再停在解释层，而是直接开始下一轮，让我把 `NPC` 集成批次和 `农田` 热文件批次做成可发车的实物 prompt。

**本轮完成**：
1. 已新建：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\2026.03.19_真实开发准入批次_02\`
2. 已生成可分发 prompt：
   - `NPC_phase2集成清洗.md`
   - `农田交互修复V2_GameInputManager热文件专项.md`
3. 已生成对应固定回收卡：
   - `NPC_phase2集成清洗.md`
   - `农田交互修复V2_GameInputManager热文件专项.md`
4. 已用锁脚本与 live 现场核实：
   - `GameInputManager.cs = unlocked`
   - `Primary.unity = unlocked`
   - shared root 仍是 `main + neutral + clean`

**关键判断**：
- `NPC` 这轮不再是“有没有 phase2”问题，而是“怎样把现有 phase2 carrier 收束为可交付交付面”。
- `农田` 这轮不再是“第一检查点是否存在”，而是“如何在拿到 `GameInputManager.cs` 热文件锁后推进第二检查点”。
- 当前最稳调度仍然是：
  - 先发 `NPC_phase2集成清洗`
  - `NPC` 退场后，再发 `农田交互修复V2_GameInputManager热文件专项`
- 农田这轮之所以不和 `NPC` 同时发车，不是因为锁现在被占用，而是因为：
  - shared root 仍是单槽位
  - 热文件锁不应该提前空占

**恢复点 / 下一步**：
- 现在可立即分发 `NPC_phase2集成清洗.md`
- 等 `NPC` 成功 `return-main` 后，再分发 `农田交互修复V2_GameInputManager热文件专项.md`

### 会话 15 - 2026-03-19（真实开发准入批次 02 回执已回收）
**用户需求**：
> 用户贴回 `NPC_phase2集成清洗` 与 `农田交互修复V2_GameInputManager热文件专项` 两张回执，要求治理线程正式收件。

**本轮完成**：
1. 已确认 live 仍为：
   - `main`
   - `neutral`
   - `clean`
2. 已结合 queue runtime 确认：
   - `ticket 8 / NPC = completed`
   - `ticket 9 / 农田交互修复V2 = completed`
3. 已确认 `GameInputManager.cs` 热文件锁在农田退场后已释放。
4. 已回填批次 02 的两张固定回收卡。

**关键判断**：
- `NPC` 本轮的真实结论是：
  - phase2 carrier 的业务交付面完整
  - 但 branch 相对 `main` 仍混有 merge-noise
  - 因而下一步应是 NPC carrier 的去噪 / 合流，而不是继续重复 phase2 集成清点
- `农田交互修复V2` 本轮的真实结论是：
  - `GameInputManager / Inventory / Farm / Placement` 的第二检查点接口已完整存在于 continuation carrier
  - 热文件锁已成功申请并释放，证明该阶段的物理通道畅通
  - 但下一步不该再重复热文件专项，而应转入农田 carrier 的去噪 / 合流
- 到这一步，两条线都从“继续探测 branch 里有没有东西”推进到了“branch 已有业务内容，但需要治理 merge-noise 才能安全落地”的新阶段。

**恢复点 / 下一步**：
- 后续若继续推进，不再复用批次 02
- 下一轮更合理的是：
  - `NPC` carrier 去噪 / 合流批次
  - `农田` carrier 去噪 / 合流批次

### 会话 16 - 2026-03-20（用户纠偏：批次 01/02 仍是护航测试，不是所需开发内容）
**用户需求**：
> 用户明确指出：当前 prompt 里的真实开发需求与业务指令几乎为 0，本质上仍是在做测试；要求我承认这一点，并说清为什么只有 `NPC / 农田交互修复V2` 在继续测试，以及还有哪些线程本该纳入一起测试。

**本轮完成**：
1. 只读复核 live 现场，确认当前不是卡死态，而是：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `git status --short --branch = ## main...origin/main`
   - `shared-root-branch-occupancy.md = main + neutral`
   - queue runtime 中历史 `ticket 1-10` 已全部收口，当前无 waiting / granted / task-active 残留
2. 回读批次 01 / 02 入口与线程 prompt，确认它们虽然打着“真实开发准入”口径，但实际内容仍以：
   - 准入
   - 只读核对
   - carrier 收束 / merge-noise 判断
   - 热文件通道验证
   为主，而不是“明确业务改什么、做到什么算完成”的开发指令。
3. 正式把当前阶段重新定性为：
   - `smoke-test_01` 是交通系统实盘测试
   - 批次 01 / 02 是“真实开发前的护航式准入 / 收束批次”
   - 还不能诚实地称为“你要的实际开发内容已经开始稳定分发”
4. 明确为什么后续只继续推了 `NPC / 农田交互修复V2`：
   - 它们是用户显式点名的最高优先级业务线
   - smoke / 批次 01 已把两条线推进到新的决策点
   - 所以后续批次集中继续裁定这两条线，而没有同步给 `导航检查 / 遮挡检查 / spring-day1`
5. 明确如果当前目标是“继续验证执行交通系统而不是只围绕两条业务线做护航”，后续测试样本至少应覆盖：
   - `导航检查`
   - `遮挡检查`
   - `spring-day1`
   其中：
   - `导航检查 / 遮挡检查` 代表审计型、只读型、短事务线程
   - `NPC / 农田交互修复V2` 代表高优先级功能线与热文件线
   - `spring-day1` 才能补上 Unity / MCP / 集成压力维度

**关键决策**：
- 用户这次纠偏是对的：批次 01 / 02 不能再被包装成“已经进入你真正要的业务开发”。
- 当前更准确的说法是：
  - 交通系统 smoke test 已通过
  - `NPC / 农田交互修复V2` 又被推进了两轮“开发前护航式准入 / 收束”
  - 但真正面向业务实现的 prompt 还没有成形
- 后续如果继续做“测试”，不能再只围着 `NPC / 农田` 转；至少要把 `导航检查 / 遮挡检查 / spring-day1` 一起纳入覆盖矩阵。
- 后续如果要切到“真正开发”，则新 prompt 必须同时包含：
  - 具体业务目标
  - 明确修改范围
  - 明确验收结果
  - 失败时的最小 checkpoint / blocker 口径

**恢复点 / 下一步**：
- 在对用户汇报时，必须显式承认：
  - 目前还处于护航 / 测试 / 准入主导阶段
  - 不是用户要的高密度业务开发 prompt 阶段
- 下一轮如果继续做交通测试，应按：
  - `NPC / 农田交互修复V2 / 导航检查 / 遮挡检查 / spring-day1`
  的组合重新设计覆盖。
- 下一轮如果切换到真正开发，则不再先发“探测 / 收束型” prompt，而是直接生成带业务交付面的开发 prompt。
### 会话 17 - 2026-03-20（现状复核：批次 01/02 已收口，但仍主要是护航准入，不是完整业务开发）
**用户目标**：
> 继续当前治理主线，直接给出现在到底做到哪一步、为什么此前只有 `NPC / 农田交互修复V2` 在继续、以及当前是否已进入真实业务开发阶段的客观判断。

**本轮完成**
1. 重新按 live 事实核对 shared root：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `git status --short --branch = ## main...origin/main`
   - `shared-root-branch-occupancy.md = main + neutral`
   - `shared-root-queue.lock.json` 当前无 `waiting / granted / task-active` 残留
2. 回读 `共享根执行模型与吞吐重构` 根层记忆、`Codex规则落地` 批次 05/06 入口、批次 02 回收卡，确认：
   - `smoke-test_01` 已完整通过
   - `真实开发准入批次_01 / 02` 也已全部回收
   - 但批次 01/02 的主内容仍是只读核验、carrier 收束、merge-noise 判断、热文件通道验证
3. 明确定性当前阶段：
   - 交通系统已经从“能否排队”推进到“能否稳定准入并退槽”
   - 但还没有进入用户真正要的“明确业务目标 + 明确修改范围 + 明确验收结果”的高密度开发批次
4. 明确为什么此前只有 `NPC / 农田交互修复V2`：
   - 这两条线是用户明确点名的最高优先级业务线
   - 我把护航式准入资源集中投给了它们
   - 但这不代表 `导航检查 / 遮挡检查 / spring-day1` 已被纳入同一轮真实开发覆盖

**关键判断**
- 当前可以诚实地说：
  - 共享根交通规则已具备“可持续使用”的基础能力
  - `NPC / 农田` 已完成两轮开发前收束
  - 但“真实业务开发 prompt 体系”还没有被系统性发出
- 因此后续不能再把“准入/护航 prompt”包装成“已经开始你要的开发内容”。

**恢复点 / 下一步**
- 如果下一步继续验证交通覆盖面，应补上：
  - `导航检查`
  - `遮挡检查`
  - `spring-day1`
- 如果下一步切到真实业务开发，应直接生成带以下要素的开发 prompt：
  - 具体业务目标
  - 明确修改范围
  - 明确验收标准
  - 失败时的最小 checkpoint / blocker 回执

## 2026-03-20｜真实业务开发批次 03 已生成
**用户目标**
- 用户明确要求停止继续发测试 / 护航型 prompt，正式开始恢复真实开发。

**本轮完成**
- 新建批次入口：`D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-20_批次分发_07_真实业务开发批次_03.md`
- 新建阶段目录：`D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\2026.03.20_真实业务开发批次_03\`
- 已为两条高优先级业务线生成真实业务开发 prompt：
  - `NPC_phase2交付面收口.md`
  - `农田交互修复V2_1.0.2交付面收口.md`
- 两份 prompt 都已从“只读核验 / 准入演习”切换为“明确业务目标 + 明确允许范围 + 明确噪音清理 + 明确 sync/return-main 收口”的真实开发口径。
- 同时补建了两张固定回收卡，供治理线程后续镜像收件。

**关键判断**
- 到这一轮为止，共享根执行模型已经不再只是“能排队、能退槽”的测试体系，而是第一次具备了给真实业务线程发开发指令的分发面。
- 但 shared root 此刻仍因治理线程本轮文档与记忆写入而 dirty，所以本轮还必须完成治理收口，才能把这批 prompt 变成“可立即执行”的 clean main 现场。
- 本轮仍只发 `NPC / 农田交互修复V2`，不代表其他线程完成；只是按用户当前优先级先恢复两条最核心业务线。

**恢复点 / 下一步**
- 先同步：`共享根执行模型与吞吐重构`、`Codex规则落地`、治理线程记忆与 skill-trigger-log。
- 再执行一次治理收口，把当前 `main` 恢复到可直接分发的 clean 状态。
- 收口完成后，再把批次 03 作为用户可直接群发的真实开发入口交付出去。
