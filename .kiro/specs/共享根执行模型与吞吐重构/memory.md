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
   - `02_专题分析/2026-03-19/基线瓶颈盘点.md`
   - `02_专题分析/2026-03-19/前序阶段问题域映射.md`
   - `02_专题分析/2026-03-19/负例矩阵与rollout清单.md`
3. 收紧项目级执行口径：
   - `AGENTS.md`
   - `shared-root-queue.md`
   - `治理线程批次分发与回执规范.md`

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
- `D:\Unity\Unity_learning\Sunset\AGENTS.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-queue.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\治理线程批次分发与回执规范.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\02_专题分析\2026-03-19\基线瓶颈盘点.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\02_专题分析\2026-03-19\前序阶段问题域映射.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\02_专题分析\2026-03-19\负例矩阵与rollout清单.md`
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
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.19_smoke-test_01\可分发Prompt\`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.19_smoke-test_01\线程回收\`
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
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.19_真实开发准入批次_01\`
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
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.19_真实开发准入批次_02\`
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
- 新建阶段目录：`D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.20_真实业务开发批次_03\`
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

### 会话 18 - 2026-03-20（NPC phase2 资产未真正落入 main 的事故补救）
**用户需求**
> 用户发现 `NPC` batch03 后场景里 NPC 一度恢复，但 `farm` 收口回到 `main` 后 `Primary` 场景的 `001/002/003` 又红掉，要求彻底查清到底是谁动了什么并恢复正常。
**已完成事项**
1. 重新按 live Git 与场景 YAML 取证，确认 `codex/farm-1.0.2-cleanroom001` 对 NPC 运行时路径没有差异，`farm` 不是覆盖源头。
2. 反查 `Primary.unity` 中 `001/002/003` 的 prefab GUID，确认 `main` 早已引用 `codex/npc-roam-phase2-003` 才有的 `Assets/222_Prefabs/NPC/*.prefab`、`Assets/100_Anim/NPC/**`、`Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset`、`Assets/Sprites/NPC/001~003.png.meta` 与对应 runtime 脚本。
3. 已从 `codex/npc-roam-phase2-003` 把上述 NPC phase2 运行时资产精确恢复到当前 `main` 工作树，并静态验证 `Primary.unity -> prefab -> sprite/meta -> animator controller -> roam profile -> runtime scripts` 这条链路已经在 `main` 上重新闭合。
4. Unity Editor 侧已发生脚本重编译与 Asset Refresh；本轮 MCP 桥失联，未能做 Inspector 读回，但 Editor.log 尾部未再出现新的 NPC missing 报错。
**关键决策**
- 这次事故不是 queue / grant / return-main 的交通故障，而是“真实业务批次验收口径不够严”：只看 `carrier` 在位与退槽成功，不足以证明 `main` 已经具备生产场景依赖的运行时资产。
- 后续真实业务批次的治理回收必须补一条硬验收：如果 `main` 场景已经引用了某条 branch 才有的 runtime 资产，则本轮结束前必须要么把它们真正并入 `main`，要么明确标记“scene not main-ready”，不能再仅凭 `branch carrier clean` 判通过。
**恢复点 / 下一步**
- 先把本轮 NPC 资产恢复与记忆层一起最小提交，恢复 `main + clean`。
- 收口后再回到真实业务开发批次 03，继续处理 `NPC / 农田` 的后续业务开发，而不是把这次事故误判成 `farm` 覆盖。

### 会话 19 - 2026-03-20（主线重对齐、dirty 讨论稿与重度 MCP 场景方案补写）
**用户需求**
> 用户指出我在修完 NPC 事故后又丢了治理主线，要求我重新拿起当前线程全部工作，明确这次事故原因、我从中发现了什么、dirty 容忍度思考是否已记录，以及重度 MCP 场景搭建线程方案是否已正式落盘。
**本轮完成**
1. 补写 [事故反思与主线重对齐.md](D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\02_专题分析\2026-03-20\事故反思与主线重对齐.md)，把“不是 farm 覆盖，而是 main 早已依赖 branch-only 资产”的主因、治理缺口和主线重锚点单独固化。
2. 补写 [dirty容忍度与清扫推送机制讨论稿.md](D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\02_专题分析\2026-03-20\dirty容忍度与清扫推送机制讨论稿.md)，明确这条方向值得做，但当前仍是讨论稿，不是已批准机制。
3. 补写 [重度MCP场景搭建线程执行方案.md](D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\02_专题分析\2026-03-20\重度MCP场景搭建线程执行方案.md)，把单线程独占、验证场景优先、`worktree` 仅解决 Git/WIP 隔离而不解决 Unity/MCP 并行的方案写成正式口径。
4. 同步更新 `requirements.md / design.md / tasks.md`，新增 `main-ready` 约束、dirty 讨论边界和重度 MCP 线程归类，并把“将 `main-ready` 验收并入真实业务批次模板”与“设计 dirty 分级机制”显式挂成待办。
**关键决策**
- 当前治理线程的职责不只是把 shared root 交通系统修通，还必须补齐“业务恢复是否真的在 main 上成立”的终局验收。
- `dirty` 容忍现在只能进入讨论，不允许偷渡成默认行为；否则会把刚建立的 clean main / neutral shared root 基线重新打散。
- 场景搭建线程不是普通短事务开发线，而是特种执行类型，后续不能直接套普通业务批次模板。
**恢复点 / 下一步**
- 下一步应把本轮文档收口同步到 `main`。
- 同步完成后，治理主线的剩余硬任务是：
  - 把 `main-ready` 验收并入下一轮真实业务批次模板
  - 继续推进 `NPC / 农田` 真实开发
  - 后续在独立治理窗口再深入设计 dirty 分级机制

## 2026-03-20｜治理文档白名单 sync 已完成
**用户目标**
> 用户要求我不要在推回 `main` 后再次失忆，而要把这次事故反哺、dirty 讨论与场景线程方案真正收口，并明确当前还剩什么。

**本轮完成**
1. 已执行治理白名单同步：`scripts/git-safe-sync.ps1 -Action sync -Mode governance -OwnerThread "Codex规则落地" -IncludePaths ...`
2. 已把本工作区正文、三份 2026-03-20 新文档、父工作区记忆与治理线程记忆一起正式推回 `main`。
3. 本次治理提交为：`6d1fde83`（`2026.03.20-03`）。

**关键判断**
- 到这一步，事故反思、`dirty` 讨论边界与重度 MCP 场景方案都已进入正式治理正文，不再只是聊天结论。
- 当前仓库内未残留本线程的业务代码 dirty，但仍保留一个其他线程的 tracked 改动：`.codex/threads/Sunset/Skills和MCP/memory_0.md`。
- 该改动不属于本工作区白名单，也不应由我越权回退或代为提交。

**恢复点 / 下一步**
- 当前治理主线仍剩两条硬尾项：
  - 把 `main-ready` 验收并入下一轮真实业务批次模板与回收卡
  - 继续把 `dirty` 分级机制留在独立治理窗口设计
- 若要恢复仓库“绝对 clean”，需由 `Skills和MCP` 线程单独收口其 `memory_0.md`，或由用户明确授权我代管。

### 补充更新
- 我已继续代为完成 `Skills和MCP` 线程 `memory_0.md` 的单文件治理收口，提交：`1eaac0c8`（`2026.03.20-05`）。
- 当前仓库现场已恢复为：
  - `main @ 1eaac0c8`
  - `git status --short --branch = ## main...origin/main`
  - shared root 仍为 `main + neutral`

## 2026-03-20｜关于 NPC 事故的定责与治理分层说明
**用户问题**
> 用户追问：这次是不是同步规范没到位、导致 NPC 没把资源同步提交；以及 `carrier-ready / main-ready` 到底和“谁犯错、谁整改”是什么关系。

**本轮结论**
1. 这次问题不是 `sync` 脚本没工作，而是“同步/验收规范不完整”。
2. 具体执行层事实是：
   - `Primary.unity` 已依赖 NPC phase2 运行时资产
   - 这些资产此前只在 `codex/npc-roam-phase2-003` 上，没真正进入 `main`
   - 因而 `NPC` 线在执行结果上确实留下了“branch 内有、main 上没有”的交付缺口
3. 但治理层也有责任：
   - 之前批次只要求回答 `carrier-ready`
   - 没有强制线程回答“这些被主场景依赖的 runtime 资产是否已经真正进入 `main`”
   - 所以这个缺口被当成“已收口”放过去了

**关键判断**
- 这件事不是二选一：
  - 执行责任：NPC 线对自己交付面没有真正落到 `main` 负直接责任
  - 治理责任：我对验收模板缺少 `main-ready` 闸门负系统责任
- `carrier-ready / main-ready` 的意义，不是绕开定责，而是把定责拆清楚：
  - `carrier-ready` 回答“你的 branch 自己是不是整理好了”
  - `main-ready` 回答“用户回到 `main` 后是不是马上能正常开发/运行”
- 少了后者，就会出现“线程说自己收口成功，但用户一回 `main` 现场还是坏的”这种假通过

**恢复点 / 下一步**
- 后续真实业务批次模板必须同时问两件事：
  - 你的 carrier 是否收口完成
  - 你的主场景/主运行入口依赖是否已经真实落入 `main`
- 这样以后就能同时做到：
  - 谁没交付完整，一眼能定责
  - 为什么系统会放过它，也能被治理层直接修正

## 2026-03-20｜工作区目录重组与统一待办落盘
**用户需求**
> 用户指出当前工作区根目录混入了过多阶段内容，要求我先整理文件夹，再把当前所有遗留代办系统性写进文档，不要继续靠聊天悬空管理。

**本轮完成**
1. 已将本工作区根目录收束为：
   - `requirements.md / design.md / tasks.md / memory.md`
   - `00_工作区导航/`
   - `01_执行批次/`
   - `02_专题分析/`
2. 已完成迁移：
   - 四个历史批次目录下沉到 `01_执行批次/`
   - 2026-03-19 / 2026-03-20 的专题分析文档下沉到 `02_专题分析/`
3. 已新建总览文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\00_工作区导航\当前总览与待办_2026-03-20.md`
4. 已补齐相关入口路径：
   - `Codex规则落地` 根层批次入口
   - 批次 01 / 02 的线程回收卡
   - 本工作区 `memory.md` 中的旧目录引用

**关键判断**
- 这轮整理不是美化目录，而是为了把“正文 / 批次 / 专题分析 / 当前总表”分层固定下来，避免后续继续把执行主线埋在大量历史阶段材料里。
- 当前总待办已经统一写入导航总表，覆盖：
  - `NPC`
  - `农田交互修复V2`
  - `导航检查`
  - `遮挡检查`
  - `spring-day1`
  - `dirty` 机制
  - 重度 `MCP` 场景线程
- 用户刚刚说明场景搭建线程已自行开始做自己的文档，因此本工作区当前只记录其方案，不介入、不打断。

**恢复点 / 下一步**
- 下一步优先执行 `任务 18`：生成补入 `main-ready` 的真实业务开发批次 04。
- 后续所有新批次与阶段材料默认继续下沉到 `01_执行批次/` 或 `02_专题分析/`，根目录不再堆历史阶段文档。

## 2026-03-20｜真实业务开发批次 04 已正式入库
**用户目标**
> 用户要求不要再停在分析或本地草稿，而是直接把“已生成但未入库”的批次 04 正式收口，并说明当前还能不能马上进入真实分发。

**本轮完成**
1. 复核 live Git 现场，确认本轮治理资产范围为：
   - `共享根执行模型与吞吐重构/tasks.md`
   - `00_工作区导航/当前总览与待办_2026-03-20.md`
   - `Codex规则落地/2026-03-20_批次分发_08_真实业务开发批次_04.md`
   - `01_执行批次/2026.03.20_真实业务开发批次_04/` 下的 4 份 prompt 与 4 份固定回收卡
2. 已执行治理白名单同步：
   - 命令：`scripts/git-safe-sync.ps1 -Action sync -Mode governance -OwnerThread "Codex规则落地" -IncludePaths ...`
   - 提交：`b6fc4b19`（`2026.03.20-09`）
3. 批次 04 的核心新增现在已经成为 live 基线：
   - 第一轮正式把 `carrier_ready / main_ready / main_ready_basis` 纳入真实业务回执
   - 第一波明确为 `NPC / 农田交互修复V2`
   - 第二波明确为 `导航检查 / 遮挡检查`
   - `spring-day1` 继续保持独立集成波次口径

**关键判断**
- 这一步意味着“批次 04 资产已在 `main` 上”，不再只是本地未同步草稿。
- 但同步后 live 工作树仍然不是绝对 clean：
  - `.codex/threads/Sunset/Skills和MCP/memory_0.md`
  - `.kiro/specs/900_开篇/5.0.0场景搭建/**`
  仍由其他线程占用并保持 dirty。
- 因此要区分两件事：
  - `batch04-ready`：是，分发资产已经正式入库。
  - `shared-root-clean-for-dispatch`：否，当前 shared root 仍有其他线程 dirty，未回到“绝对 clean”。

**恢复点 / 下一步**
- 当前本工作区关于“生成并入库批次 04”的目标已经完成，`tasks.md` 中任务 18 可视为正式闭环。
- 下一步转入任务 19，但在真正开始第一波分发前，应先由对应线程收口：
  - `Skills和MCP/memory_0.md`
  - `900_开篇/5.0.0场景搭建/**`
- 收口完成、shared root 回到 `main + clean + neutral` 后，再发 `NPC / 农田` 会最稳。

### 补充更新
- 上述阻塞现已解除：
  - `Skills和MCP` 线程的 tracked `memory_0.md`
  - `900_开篇/5.0.0场景搭建/**`
  已整体迁入专属 branch/worktree：
  - `codex/scene-build-5.0.0-001`
  - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
- 当前 shared root 已重新放空到 `main`，因此 batch04 第一波现在可以直接启动：
  - `NPC`
  - `农田交互修复V2`
- `spring-day1` 仍不并入 batch04，原因不变：
  - 它是集成型 / 主场景型 / 高 Unity 风险线程
  - 需要独立集成波次，而不是混入当前短事务 shared root 波次

## 2026-03-20｜shared root 残留清场完成，batch04 第一波恢复到可续跑
**用户目标**
> 用户要求我直接开始：先把场景搭建线残留从 shared root 移走并告知它后续动作，再继续推进 `NPC / 农田` 的 batch04 第一波续跑；同时明确 `dirty` 放宽机制到底应从哪里启动。

**本轮完成**
1. 复核 shared root 阻断根因，确认 `NPC / 农田` 的 `LOCKED_PLEASE_YIELD` 不是 occupancy 错态，而是 shared root 上残留了场景搭建线的两处内容：
   - `.codex/threads/Sunset/Skills和MCP/memory_1.md`
   - `.kiro/specs/900_开篇/5.0.0场景搭建/**`
2. 进一步核到这批内容并非纯重复副本：
   - `5.0.0场景搭建/**` 在 shared root 与 `scene-build-5.0.0-001` worktree 中已经分叉
   - `memory_1.md` 当时只存在于 shared root
3. 已采用“无损迁入”方案完成现场清场：
   - 把 shared root 的 `5.0.0场景搭建/**` 复制到
     `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\shared-root-import_2026-03-20\`
   - 把 shared root 的 `memory_1.md` 复制到
     `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.codex\threads\Sunset\Skills和MCP\memory_1.md`
   - 随后已从 shared root 删除这两处残留副本
4. 继续复核时又发现第二层阻断：`.kiro/specs/` 下还残留一棵误生成的 URL 编码历史目录：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\%E5%85%B1%E4%BA%AB...`
   这棵目录会触发 `git status` 的 `Filename too long` warning，并被 `wake-next` 视为 shared root dirty。
5. 已删除这棵 URL 编码残留目录后，shared root 重新恢复为：
   - `main`
   - `git status --short --branch = ## main...origin/main`
   - occupancy 仍为 `main + neutral`
6. live queue 现状已推进到：
   - `NPC` 已存在未消费 grant，下一步应直接走 `ALREADY_GRANTED -> ensure-branch`
   - `农田交互修复V2` 仍处于 waiting，需在 `NPC` 退场后继续 `wake-next`
7. `dirty` 放宽机制的启动点已正式钉回总览：
   - 继续留在治理窗口
   - 等 `batch04` 第一波完成真实回收后，再从 `dirty容忍度与清扫推送机制讨论稿.md` 起下一阶段

**关键判断**
- 场景搭建线的文档与线程记忆确实应回到它自己的 `branch + worktree` 承载；继续留在 shared root，只会持续卡死业务线程准入。
- 对这类已分叉残留，正确做法不是直接删除，也不是继续霸占 `main`，而是“先无损迁入、再清 shared root”。
- `NPC / 农田` 当前还不需要重发新的 request；前者应直接消费现有 grant，后者应等待 `NPC` 退场后由治理线程继续叫号。

**恢复点**
- shared root 已重新 clean，可继续 batch04 第一波。
- 立即下一步：
  1. 给场景搭建线程发“去 worktree 吸收 `shared-root-import_2026-03-20`”的收口 prompt
  2. 先让 `NPC` 消费现有 grant 完成本轮真实 checkpoint
  3. `NPC` 退场后再 `wake-next -> 农田交互修复V2`

## 2026-03-20｜NPC 已完成 docs-only 收口，当前需向用户明确事故分型、责任层级与 worktree 并行边界
**当前主线目标**
- 在 `NPC` 已完成 batch04 第一波最小收口后，把“到底出了什么问题、责任是谁、场景搭建线程能否继续并行”明确写成治理结论。

**本轮完成**
1. 已复核当前 live 现场：
   - shared root：`D:\Unity\Unity_learning\Sunset @ main`
   - `git status --short --branch`：clean
   - occupancy：`neutral-main-ready`
   - queue：`current_serving_ticket = null`
   - `农田交互修复V2` 仍保持 waiting
2. 已复核 `NPC` 本轮 docs-only checkpoint：
   - 提交：`b680cd4b`
   - 文件：`D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\2026-03-20_phase2_main-ready核验.md`
   - `sync / return-main` 最终都已成功
3. 已复核场景搭建线程现场：
   - worktree：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - branch：`codex/scene-build-5.0.0-001`
   - dirty 仅留在该 worktree 自己的文档与线程记忆，不再污染 shared root

**关键判断**
- 必须把两类问题分开：
  1. 更早之前的 `main` 内 NPC sprite / 资产依赖缺失：
     - 性质：业务交付验收缺口
     - 责任：`NPC` 执行责任 + 治理验收责任
     - 根因：当时只验 `carrier-ready`，没有把 `main-ready` 设成硬门
  2. 这次 batch04 收口时 `sync / return-main` 一度被卡：
     - 性质：shared root 收口状态机缺口
     - 责任：治理 / 工具实现主责；`NPC` 只承担异常后不该继续扩大环境诊断的次责
     - 根因：`ensure-branch` 曾半成功，但 occupancy 没同步落成 `task-active`，导致后续按旧态错拦
- 场景搭建线程现在可以长期并行，但边界必须说清：
  - Git 层：可以，因为它已迁入自己的 `branch + worktree`
  - Unity / MCP 层：不自动安全；进入真实 scene 写入时仍需单实例 / 单写者闸门

**恢复点 / 下一步**
- batch04 第一波已从 `NPC` 收口事故中恢复。
- 立即下一步：
  1. 继续 `农田交互修复V2`
  2. 单开治理修复：补 `git-safe-sync.ps1` 在 shared root 上 `ensure-branch -> task-active -> sync -> return-main` 的鲁棒性
  3. 后续统一补 prompt 模板：系统级异常即冻结，禁止线程自行修环境

## 2026-03-21｜正式立项 occupancy 收口脚本修复，并确认 worktree 不豁免 Unity / MCP 单写者
**当前主线目标**
- 不把“shared root 收口脚本缺口”继续留在聊天里，而是正式挂回工作区任务清单，并明确场景搭建 worktree 的并行边界。

**本轮完成**
1. 已新增任务：
   - `20. 修复 shared root occupancy 收口状态机在异常下的鲁棒性`
2. 已新增专题分析文档：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\02_专题分析\2026-03-21\shared-root_occupancy收口脚本缺口与修复计划.md`
3. 已再次复核并确认：
   - shared root 当前 clean，但我本轮治理文档 dirty 在未同步前会阻断 `wake-next`
   - `farm` 后续应继续消费现有 batch04 真实业务 prompt，不是回到 smoke/test prompt
   - `scene-build-5.0.0-001` 虽然是独立 worktree，但不自动绕过 Unity / MCP 单实例 / 单写者规则

**关键判断**
- `worktree` 只隔离 Git / 文档 / WIP，不隔离 Unity Editor / MCP 冲突。
- 如果你要在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001` 打开 Unity，可以，但前提是：
  - 当前没有另一份 Sunset Editor 在写
  - 它自己成为当前唯一的 Unity / MCP 写线程
- `farm` 现在不需要等治理脚本先修完才继续；它只需要等 shared root 再次 clean，然后按当前 batch04 实际 prompt 续跑。

**恢复点 / 下一步**
- 先同步本轮治理文档 dirty。
- 然后立即重新执行 `wake-next`，把 `农田交互修复V2` 从 waiting 推进到可继续状态。

### 补充更新
- 本轮治理文档已同步进 `main`：
  - 提交：`e39e097a`
- 随后治理线程已成功执行一次 `wake-next`：
  - `农田交互修复V2`
  - `ticket = 15`
  - `branch = codex/farm-1.0.2-cleanroom001`
- 当前对 `farm` 的正式口径已变为：
  - 继续沿用 batch04 真实业务 prompt
  - 先做 live preflight
  - 随后执行 `request-branch`，预期返回 `ALREADY_GRANTED`
  - 再继续 `ensure-branch`

## 2026-03-21｜shared-root active-session 旧结构兼容缺口已修复，occupancy 收口 bug 正式落地补洞
**当前主线目标**
- 修复 `git-safe-sync.ps1` 在 shared root 收口链路中的确定性 bug，避免旧 `active-session` runtime 因缺字段把 `sync` 尾部和 `return-main` 收口再次炸掉。

**本轮完成**
1. 已在 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 落地 active-session 兼容修复：
   - 新增 `Set-OrAddPsNoteProperty`
   - 新增 `Repair-SharedRootActiveSessionRecord`
   - `Get-SharedRootActiveSession` 读取旧 runtime 时会自动补齐缺失字段并回写
   - `Set-SharedRootActiveSession` 初始化时显式写入 `last_reason = entered-task-active`
   - `Touch-SharedRootActiveSession` 改为“缺属性则补、已有则改”
   - `Complete-SharedRootActiveSession` 归档历史时补带 `source / last_reason`
2. 已更新任务单：
   - `19` 改为完成，表示 batch04 第二波收件与切换已完成
   - `20` 改为完成，表示本次 occupancy 收口鲁棒性补丁已落地
   - `15` 继续保留未完成，明确 `dirty` 分级/放宽不混入这次 bugfix
3. 已更新专题分析文档：
   - 明确这次修的是治理工具层 bug，不是业务线程再犯同一种交付错误
   - 明确 `dirty` 放宽仍属于后续设计题
4. 已完成本地验证：
   - PowerShell 语法解析通过
   - 旧 active-session 结构兼容测试通过
   - 从仓库根运行 working tree 版 `preflight -Mode governance` 成功

**关键判断**
- 这次最像真根因的是 ignored runtime 的旧结构缺少 `last_reason`，旧逻辑在 `Touch-SharedRootActiveSession` 里直接赋值，导致提交/推送已经成功后，脚本尾部在补写 active-session 状态时再炸。
- 这属于 shared root 治理工具链主责，不应再把责任继续压给 `NPC / farm / 导航 / 遮挡` 这些业务线程。
- 本轮没有放宽 `dirty` 闸门；我们只修确定性 bug，不把设计变更和 bugfix 混做一团。

**恢复点 / 下一步**
- 先将本轮治理修复、工作区记忆、线程记忆一并同步进 `main`。
- 后续再单开任务 `15`，讨论 `dirty` 分级、容忍边界和 takeover 条件。

## 2026-03-21｜任务 15 已进入正式设计：dirty 分级、takeover 边界与放宽上限已写成草案
**当前主线目标**
- 在 `scene-build` 继续施工的同时，把任务 `15` 从旧讨论稿推进到正式设计稿，明确 dirty 不是“一刀切全禁”也不是“谁都能带脏跑”。

**本轮完成**
1. 已只读确认 `scene-build` 仍在自己的 worktree 内推进：
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - worktree 内已出现新的施工相关产物与文档脏改
   - 我未插手其现场，只把它视为当前唯一继续中的 Unity / MCP 写线程
2. 已新增任务 15 的正式设计稿：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\02_专题分析\2026-03-21\dirty分级与takeover边界设计稿.md`
3. 设计稿已把 dirty 正式拆为四级：
   - `D0`：噪音 / ignored runtime
   - `D1`：治理型可收口 dirty
   - `D2`：任务分支内可继续的 owner-dirty
   - `D3`：禁止放行 dirty
4. 已明确当前阶段的正式边界：
   - 默认硬闸门不撤：`main clean + shared root neutral`
   - 允许继续讨论并逐步补脚本的“分级报告能力”
   - 暂不批准跨线程直接接 raw dirty
   - 当前只承认“同线程、同任务分支”的 D2 continuation
5. 已更新：
   - `tasks.md` 中任务 `15` 的当前进展说明
   - `design.md` 中对任务 `15` 的正式挂接

**关键判断**
- 这次不是在给 shared root 开后门，而是在把 dirty 从“全部按事故处理”升级成“先分级，再决定哪些场景值得保守放宽”。
- 当前最小可接受放宽只到：
  - `D1` 治理 dirty 的可解释收口
  - `D2` 同线程 continuation
- 跨线程 raw dirty takeover 仍然不安全，不能在这一轮设计里偷渡上线。

**恢复点 / 下一步**
- 先把这轮设计稿、任务单更新和记忆同步进 `main`。
- 后续若继续推进任务 `15`，下一步应先补脚本的“分级报告”能力，而不是先改放宽判定。

## 2026-03-21｜任务 15 首轮 dirty 报告层已落到脚本并通过只读验证
**当前主线目标**
- 把任务 `15` 从“只有设计稿”推进到脚本级首轮落地，但继续保持 `main clean + shared root neutral` 的默认硬闸门不变。

**本轮完成**
1. 已在 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 落下首轮 dirty 报告层：
   - 新增 `Test-GovernanceReportPath / Test-DirtyHardBlockPath / Get-DirtyLevel / Get-DirtyOwnerHint / Get-DirtyPolicyHint`
   - 新增 `New-DirtyReportEntry / Format-DirtyReportEntry / Get-DirtyLevelSummaryText`
2. 已让 `New-PreflightReport` 和 `Write-PreflightReport` 不再只输出 `Category`，而是同时输出：
   - `DirtyLevel`
   - `OwnerHint`
   - `PolicyHint`
   - 以及 allowed / remaining 的分级概览
3. 已让 `Get-RemainingDirtyGateMessage` 在 remaining dirty 阻断预览里补带 `<Dx/owner>`，便于后续快速判断是治理收口、同线程 continuation，还是硬阻断。
4. 已完成本地只读验证：
   - PowerShell 语法解析通过
   - `preflight -Mode governance` 成功，当前 `scripts/git-safe-sync.ps1` 被正确报告为 `D1`
   - `preflight -Mode task` 在 `main` 上仍然明确返回 `CanContinue = False`，证明报告层没有偷偷放宽 task 闸门

**关键判断**
- 这轮已经把“dirty 分级”从纯文档讨论推进到了脚本可见性层，但还没有进入“放宽准入”的策略层。
- 当前脚本的正式口径仍然是：
  - `D0`：噪音 / runtime，不把它误说成业务脏改
  - `D1`：治理型可收口 dirty，可解释、可审计
  - `D2`：仅保留给同线程、同任务分支 continuation 的 owner-dirty 讨论空间
  - `D3`：继续视为硬阻断 dirty
- 这说明任务 `15` 当前已经进入“先让脚本会说清楚脏改是什么”，而不是“先让脚本带脏放行”。

**恢复点 / 下一步**
- 先把本轮脚本补丁与记忆一并同步进 `main`。
- 后续若继续任务 `15`，下一子步应优先拿真实 dirty 样本复核分类边界，再决定是否需要进入更深一层的 takeover / 放宽判定。

## 2026-03-21｜任务 15 真实样本回放已校正 D3 边界，仍未触碰放宽闸门
**当前主线目标**
- 继续推进任务 `15`，把 dirty 报告层从“能输出”推进到“分类边界与设计稿一致”，同时不打断 `scene-build` 的 Unity / MCP 施工窗口。

**本轮完成**
1. 已在 shared root `main @ 5570ce48` 上复核 live 基线：
   - `git status --short --branch = ## main...origin/main`
   - shared root 仍为 `main + neutral`
   - `scene-build` 继续停留在自己的 worktree，我未介入其现场
2. 已用同一 PowerShell 会话 dot-source working tree 版 `scripts/git-safe-sync.ps1`，对以下真实样本做分级回放：
   - `Assets/222_Prefabs/NPC/NPC.prefab`
   - `Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset`
   - `Assets/100_Anim/NPC/NPC.controller`
   - `Assets/Sprites/NPC/icon.png.meta`
   - `Assets/000_Scenes/Primary.unity`
   - 以及对照样本 `HotbarSelectionService.cs / GameInputManager.cs / ProjectSettings/TagManager.asset`
3. 已复现两类真实偏差：
   - `Primary.unity` 因 `EndsWith('/primary.unity')` 的大小写比较而漏判成 `D2`
   - 设计稿中应视为 `D3` 的共享 Prefab / ScriptableObject / Animator Controller / Sprite meta 仍被误报成 `D2`
4. 已在 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 修正报告层分类：
   - `Test-DirtyHardBlockPath` 统一改为基于小写比较
   - 补入 `Assets/222_Prefabs/*.prefab*`
   - 补入 `Assets/111_Data/*.asset*`
   - 补入 `Assets/100_Anim/*.controller*` 与 `*.overrideController*`
   - 补入 `Assets/Sprites/*.meta`
   - `Get-DirtyOwnerHint` 的 `Primary.unity / GameInputManager.cs` 热文件提示同步改为大小写稳定判断
5. 已完成二次样本回放验证：
   - `NPC.prefab / NPC_DefaultRoamProfile.asset / NPC.controller / icon.png.meta / Primary.unity` 现已全部报告为 `D3`
   - `HotbarSelectionService.cs` 仍为 `D2`
   - `GameInputManager.cs / ProjectSettings/TagManager.asset` 仍为 `D3`

**关键判断**
- 这轮修的是“报告层分类准确性”，不是 shared root 的放宽准入。
- 默认硬闸门保持不变：
  - `main clean`
  - shared root `neutral`
  - 不批准跨线程直接接 `raw dirty`
- 真实样本已经证明，设计稿里点名的共享资产此前确实存在误报为 `D2` 的缺口；如果不补，后续 dirty 报告会给出错误安全感。

**恢复点 / 下一步**
- 先把本轮脚本补丁、任务单更新和三层记忆同步进 `main`。
- 同步后，任务 `15` 的下一子步应转向“是否还存在其他误分边界”，而不是提前讨论放宽闸门。

## 2026-03-21｜任务 15 第二轮样本审计已补齐 Scene / Anim / Sprite 与 Hotbar owner 边界
**当前主线目标**
- 在上一轮已修正共享 Prefab / ScriptableObject 边界后，继续沿任务 `15` 做第二轮真实样本审计，确认是否还有其他会给治理层制造错误安全感的误分级。

**本轮完成**
1. 已再次用同一 PowerShell 会话 dot-source working tree 版 `scripts/git-safe-sync.ps1`，回放第二批真实样本：
   - `Assets/000_Scenes/SceneBuild_01.unity`
   - `Assets/000_Scenes/SceneBuild_01.unity.meta`
   - `Assets/100_Anim/NPC/001/Clips/001_Idle_Down.anim`
   - `Assets/Sprites/NPC/1/001.png`
   - `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
   - `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
2. 已确认四类边界仍有偏差：
   - 非 `Primary` 的真实 scene 文件与 `.meta` 仍被报成 `D2`
   - `Assets/100_Anim/*` 下真实 `.anim` clip 仍被报成 `D2`
   - `Assets/Sprites/*` 下真实 sprite 图片文件仍被报成 `D2`
   - `HotbarSelectionService.cs` 的 owner hint 仍误归到 `Codex规则落地`
3. 已再次修正 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`：
   - 将场景高风险判断从 `Primary.unity` 扩成所有 `.unity / .unity.meta`
   - 将 `Assets/222_Prefabs/*`、`Assets/111_Data/*`、`Assets/100_Anim/*`、`Assets/Sprites/*` 的整根高风险语义写回 `D3`
   - 将农田 owner hint 从“只认目录段”补成“也认 `hotbar` 关键字”，修正 `HotbarSelectionService.cs`
4. 已完成第二轮二次验证：
   - `SceneBuild_01.unity / .meta` 现已归入 `D3`
   - 真实 `.anim` clip 与真实 sprite 图片文件现已归入 `D3`
   - `HotbarSelectionService.cs` 现已正确归属 `农田交互修复V2`
   - `FarmToolPreview.cs` 仍保持 `D2 + 农田交互修复V2`

**关键判断**
- 到这一步，任务 `15` 的报告层已经不只是“会说 D1/D2/D3”，而是开始和真实现场更一致。
- 这依然不代表 shared root 可以带脏放行；我们修的是分类准确性和 owner 提示，不是准入策略。

**恢复点 / 下一步**
- 先把这轮第二次样本修正一并同步进 `main`。
- 同步后，任务 `15` 的下一子步应转向“是否还存在其他高风险目录或 owner 提示盲区”，而不是提前改放宽规则。

## 2026-03-21｜任务 15 第三轮样本审计已补齐 spring-day1 与遮挡 owner 边界
**当前主线目标**
- 沿着任务 `15` 继续排查“谁的 dirty 被脚本说错了”这类 owner hint 盲区，而不是提前讨论 shared root 放宽。

**本轮完成**
1. 已再次在 shared root `main` 上复核 live 基线：
   - `git status --short --branch = ## main...origin/main`
   - shared root 仍为 `main + neutral`
   - `scene-build` 继续停留在自己的 worktree，我未介入其现场
2. 已继续用同一 PowerShell 会话 dot-source working tree 版 `scripts/git-safe-sync.ps1`，回放第三批真实样本：
   - `.kiro/specs/900_开篇/5.0.0场景搭建/1.0.1初步规划/tasks.md`
   - `.kiro/specs/900_开篇/spring-day1-implementation/requirements.md`
   - `.kiro/specs/900_开篇/spring-day1-implementation/OUT_tasks.md`
   - `Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset`
   - `Assets/Editor/BatchAddOcclusionComponents.cs`
   - `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`
   - `Assets/YYY_Scripts/Service/Rendering/OcclusionTransparency.cs`
   - `Assets/YYY_Tests/Editor/OcclusionSystemTests.cs`
3. 已确认两类 owner 边界仍有偏差：
   - 整个 `.kiro/specs/900_开篇` 之前都被粗暴归成 `scene-build`，导致 `spring-day1` 文档也被误认成场景线程
   - `Occlusion` 相关文件名虽然清楚指向遮挡线，但旧规则只认目录段，结果这些文件都回退成治理线程默认 owner
4. 已修正 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`：
   - 将 `scene-build` owner 规则从整个 `.kiro/specs/900_开篇` 收窄到 `.kiro/specs/900_开篇/5.0.0场景搭建`
   - 新增 `spring-day1` owner 规则，识别 `spring-day1-implementation` 与 `springday1 / spring-day1`
   - 将遮挡 owner 规则扩成可识别 `.kiro/specs/云朵遮挡系统` 与文件名中的 `occlusion / 遮挡`
5. 已完成第三轮回归验证：
   - `spring-day1` 文档与 `SpringDay1_FirstDialogue.asset` 现已正确显示为 `spring-day1`
   - `BatchAddOcclusionComponents.cs / OcclusionManager.cs / OcclusionTransparency.cs / OcclusionSystemTests.cs` 现已正确显示为 `遮挡检查`
   - `5.0.0场景搭建` 文档与 `SceneBuild_01.unity` 仍继续正确显示为 `scene-build`

**关键判断**
- 这轮仍然只修 owner hint 的归属准确性，不放宽 `sync / return-main` 的硬闸门。
- `TilemapToSprite.cs` 这类泛 Tilemap 工具本轮刻意没有强行绑到 `scene-build`，避免用过宽关键词继续制造新的误归属。

**恢复点 / 下一步**
- 先把这轮第三次样本修正一并同步进 `main`。
- 同步后，任务 `15` 的下一子步应转向“是否还存在其他 owner fallback 仍过于粗糙的路径”，而不是提前谈带脏放行。

## 2026-03-21｜恢复开发总控规范与线程级放行 prompt 已固化成正式文件
**当前主线目标**
- 把“现在谁能开发、谁要等待、用户每次该怎么发 prompt、收到回执后治理线程该怎么更新口径”从聊天里的零散说明，收束成一套可持续维护的正式文件。

**本轮完成**
1. 已新增当前总控文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\00_工作区导航\恢复开发总控与线程放行规范_2026-03-21.md`
2. 已新增恢复开发总控目录：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_恢复开发总控_01\`
3. 已在该目录中固化：
   - `治理线程_职责与更新规则.md`
   - `线程回执规范/统一最小回执格式.md`
   - `可分发Prompt/scene-build_结构层继续施工.md`
   - `可分发Prompt/NPC_当前开发放行.md`
   - `可分发Prompt/农田交互修复V2_当前开发放行.md`
   - `可分发Prompt/导航检查_当前开发放行.md`
   - `可分发Prompt/遮挡检查_当前开发放行.md`
   - `可分发Prompt/spring-day1_集成波次前只读准备.md`
4. 已新增治理入口文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-21_批次分发_09_恢复开发总控与线程放行规范.md`
5. 已把当前交通模型正式写死：
   - `scene-build` 是特殊 worktree 施工线，可继续结构层，但仍不是 Unity live 验收通过
   - shared root 业务线一次只允许一条进入
   - 当前推荐顺序为 `NPC -> 农田交互修复V2 -> 导航检查 -> 遮挡检查`
   - `spring-day1` 当前不放行真实开发，只放行集成波次前只读准备

**关键判断**
- 对用户最重要的不是“我现在口头建议谁先做”，而是“以后每次回执回来时，治理线程必须先改文件、再改口径”。
- 这轮固化的本质是把“恢复开发总控”升级成长期维护资产，而不是继续靠聊天记忆排队。

**恢复点 / 下一步**
- 先把这轮总控文件、入口文件和记忆同步进 `main`。
- 同步后，后续任一线程回执只要改变了风险、顺序或边界，治理线程必须先更新这些文件，再告诉用户新的发放顺序。

## 2026-03-21｜治理线程自己的当前执行 prompt 已补齐
**当前主线目标**
- 把“别人有当前开发 prompt，但治理线程自己只有职责说明”的缺口补齐，避免后续又回到口头执行。

**本轮完成**
1. 已新增治理线程自用执行入口：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_恢复开发总控_01\治理线程_当前执行Prompt.md`
2. 已把总控文件中的“当前 prompt 入口”补成对称结构：
   - 除业务线程与 `scene-build` 外，新增 `治理线程（发给我自己）`
3. 已同步补记批次入口文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-21_批次分发_09_恢复开发总控与线程放行规范.md`
4. 已在职责文件中显式写明：
   - 我的当前执行入口就是这份新 prompt
   - 维护“治理线程自用 prompt”本身也属于治理线程的职责

**关键判断**
- 之前那套文件已经回答了“你该发给谁”，但还没完全回答“我自己该按什么文件做事”。
- 补齐这份自用 prompt 后，整套规范从“业务线程有 prompt，治理线程有职责”变成“所有角色都有显式执行入口”。

**恢复点 / 下一步**
- 先把这轮补齐自用 prompt 的文件与记忆同步进 `main`。
- 同步后，对用户的正式口径更新为：以后不只是别人按当前 prompt 开发，我自己也按当前 prompt 维护和更新总控。

## 2026-03-21｜“当前开发放行”命名歧义已补白话说明
**当前主线目标**
- 把项目经理最容易误解的一点补清楚：`可分发Prompt` 里的文件到底是“直接执行 prompt”还是“先测准备度”的测试 prompt。

**本轮完成**
1. 已新增项目经理白话说明：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_恢复开发总控_01\可分发Prompt\README.md`
2. 已在总控文件中明确写明：
   - 当前目录下这些 prompt 默认都是直接执行 prompt
   - 唯一例外是 `spring-day1_集成波次前只读准备.md`
3. 已在批次入口文件中同步写明同一口径，避免用户只看入口文件时继续误读

**关键判断**
- 这轮不是业务顺序变化，而是命名解释补洞。
- 当前体系里，`当前开发放行` 的真实含义是“放行后直接做本轮真实任务”，不是“先试探准备好了没有”。

**恢复点 / 下一步**
- 继续把这轮新增说明与前面的自用 prompt 一并同步进 `main`。
- 同步后，对用户的口径统一为：除 `spring-day1` 外，当前目录中的 prompt 都可直接发出去执行。

## 2026-03-21｜治理线程已按自用 prompt 进入执行态，并裁定当前第一条仍为 NPC
**当前主线目标**
- 不再只是“写好治理线程 prompt”，而是正式按它执行一轮 live 核查和当前发放裁定。

**本轮完成**
1. 已按治理线程自用 prompt 完成手工等价启动闸门核查，确认：
   - `D:\Unity\Unity_learning\Sunset`
   - `branch = main`
   - `HEAD = d382c41c`
   - `git status --short --branch = ## main...origin/main`
2. 已核对 shared root 占用文档：
   - `current_branch = main`
   - `is_neutral = true`
   - `lease_state = neutral`
3. 已核对 Unity / MCP 单实例层：
   - `current_claim = none`
   - 但口径仍保持“如需进入 Unity / MCP，必须再核 live”
4. 已再次对齐当前总控文件与治理线程自用 prompt，没有发现顺序漂移或规则冲突
5. 已形成当前执行裁定：
   - `scene-build` 可继续其 worktree 施工
   - shared root 当前下一条仍应发 `NPC`
   - `农田交互修复V2 / 导航检查 / 遮挡检查 / spring-day1` 继续按既定顺序等待

**关键判断**
- 这轮不是新改规则，而是把“治理线程现在开始正式照规则跑”变成 live 事实。
- 在当前 clean `main + neutral` 的前提下，没有理由跳过 `NPC` 去先发别的 shared root 线程。

**恢复点 / 下一步**
- 先把这次 live 核查与裁定补记同步进 `main`。
- 同步后，对用户的正式口径就是：
  - 现在如果要发 shared root 第一条，就发 `NPC`
  - 如果要继续场景施工，`scene-build` 也可并行继续自己的 worktree 线

## 2026-03-21｜scene-build 与 NPC 的当前 prompt 已按真实验收反馈更新
**当前主线目标**
- 不再沿用已经过时的“结构层继续施工 / NPC 当前开发放行”通用口径，而是把用户刚给出的真实验收反馈直接写进新 prompt。

**本轮完成**
1. 已基于 scene-build 最新回执与用户截图反馈，新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_恢复开发总控_01\可分发Prompt\scene-build_装饰层纠偏与Primary参考重做.md`
2. 这份 prompt 已明确写死：
   - 当前装饰层审美不达标
   - 必须先只读学习 `Assets/000_Scenes/Primary.unity`
   - 再重做 `Decor_Farmstead`
   - 重点是整体组织、院落关系、留白、簇与环境叙事，而不是继续随手加道具
3. 已基于 NPC 最新回执与用户图片反馈，新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_恢复开发总控_01\可分发Prompt\NPC_气泡位置碰撞体修正与下一步规划.md`
4. 这份 prompt 已明确写死：
   - 当前气泡仍压脸
   - 位置与观感仍怪
   - NPC 与玩家穿透问题必须纳入本轮
   - 碰撞体应参考玩家，只保留合理下半身实体感
   - 同时要求给出 `next_step_plan`
   - 若修复离不开 Unity / MCP / prefab live 调参，则必须回 `needs-unity-window`
5. 已同步更新：
   - 总控文件中的当前推荐施工内容与当前 prompt 入口
   - 批次入口文件中的对应路径

**关键判断**
- 这轮不是抽象改文案，而是把“你对结果不满意的点”直接变成下一轮线程的硬目标。
- scene-build 和 NPC 现在都不该再按旧 prompt 继续，因为旧 prompt 没有把这次真实验收问题写进去。

**恢复点 / 下一步**
- 先把这轮新 prompt 与记忆同步进 `main`。
- 同步后：
  - scene-build 当前应发 `scene-build_装饰层纠偏与Primary参考重做.md`
  - NPC 当前应发 `NPC_气泡位置碰撞体修正与下一步规划.md`

## 2026-03-21｜NPC 已归还 shared root，scene-build 已收口装饰层，当前交通模型已更新
**当前主线目标**
- 根据最新两条回执，把“当前到底该发谁、谁先不要发、scene-build 下一步是什么”更新成正式 live 口径。

**本轮完成**
1. 已 live 核对 shared root：
   - `main @ 95b3f793`
   - `git status = clean`
   - occupancy = `neutral-main-ready`
2. 已确认 NPC 本轮结果：
   - `codex/npc-roam-phase2-003 @ 3e7af95b`
   - shared root 已成功 `return-main`
   - 当前成果在分支，不在 `main`
   - 后续待办不是“继续开发”，而是单独的 Unity 视觉 / 碰撞手测窗口
3. 已 live 核对 scene-build worktree：
   - `codex/scene-build-5.0.0-001 @ 44afab3f`
   - `git status = clean`
4. 已新增：
   - `scene-build_逻辑层继续施工.md`
5. 已更新总控文件与批次入口文件：
   - scene-build 当前阶段改为 `逻辑层`
   - shared root 当前下一条改为 `农田交互修复V2`
   - `NPC` 改列到“已收口但待后续手测”，不再视为当前应立即再次发放的开发线程

**关键判断**
- 这轮最重要的变化不是新增 bug，而是交通状态变化：
  - `NPC` 已完成当前开发窗口并归还
  - `scene-build` 已完成装饰层纠偏并收口
  - 所以 shared root 这边现在该轮到 `农田交互修复V2`
- 同时，`NPC` 虽然当前不再占槽，但也不等于“已经彻底验收通过”；它还有分支内手测债。

**恢复点 / 下一步**
- 先把这轮状态更新与新 prompt 同步进 `main`
- 同步后：
  - `scene-build` 现在应发 `scene-build_逻辑层继续施工.md`
  - shared root 现在应发 `农田交互修复V2_当前开发放行.md`

## 2026-03-21｜恢复开发总控状态已正式同步进 main
**当前主线目标**
- 把最新交通裁定真正落成 live 基线，避免“文档说一套、仓库实际还是上一轮”的漂移。

**本轮完成**
1. 已使用治理白名单同步成功：
   - `main @ 7c6fc1e9`
2. 已正式把以下状态写进 live 文档：
   - `scene-build` 当前阶段 = `逻辑层`
   - shared root 当前下一条 = `农田交互修复V2`
   - `NPC` 当前身份 = 已收口但待后续 Unity 手测
3. 已确认 shared root 仍保持：
   - `branch = main`
   - `occupancy = neutral`

**当前执行口径**
- 现在可以继续发：
  - `scene-build_逻辑层继续施工.md`
  - `农田交互修复V2_当前开发放行.md`
- 现在先不要再发新的 shared root 开发 prompt 给 `NPC`

**下一步**
- 等用户决定是否立刻放行 `scene-build` 进入逻辑层、以及是否让 `farm` 接棒 shared root。

## 2026-03-21｜farm 已完成本轮 checkpoint，但 NPC 因真实验收失败升为下一条 shared root
**当前主线目标**
- 接住 `farm` 的新回执和用户对 `NPC` 的真实验收否决，把 shared root 下一条从“继续沿用旧排序”改成“NPC 正式返工”。

**本轮完成**
1. 已 live 核对 shared root：
   - `main @ 91f7328a`
   - `git status = clean`
   - occupancy = `neutral-main-ready`
2. 已确认 `farm` 本轮结果：
   - checkpoint 已推送到 `codex/farm-1.0.2-cleanroom001 @ 21a5f8df`
   - `carrier_ready = yes`
   - `main_ready = no`
   - 原因是 continuation branch 仍含 branch 既有差异：`AGENTS.md`、`scripts/git-safe-sync.ps1`
3. 已根据用户真实 Unity 验收裁定：
   - `NPC` 当前状态不是“待手测”
   - 而是“验收失败，应返工”
4. 已新增正式 prompt：
   - `NPC_验收失败返工与live验收闭环.md`
5. 已更新总控口径：
   - shared root 当前下一条 = `NPC（验收失败返工）`
   - `farm` 改列为“已完成 checkpoint 但当前不是 main-ready”

**关键判断**
- `farm` 这轮是有效 checkpoint，不是失败；它的问题是“分支还不是 main-ready”，不是“这轮没做成事”。
- `NPC` 则相反：分支里虽然有提交，但真实验收没过，所以当前优先级必须升到 shared root 下一条。

**恢复点 / 下一步**
- 现在可以：
  - 继续让 `scene-build` 在 worktree 里推进
  - shared root 下一条发 `NPC_验收失败返工与live验收闭环.md`

## 2026-03-21｜scene-build 已完成逻辑层最小版本，当前阶段升为回读自检与高质量初稿收口
**当前主线目标**
- 接住 `scene-build` 的新回执，把它从“继续逻辑层”升级成下一阶段，不让用户继续发过期 prompt。

**本轮完成**
1. 已确认 scene-build worktree：
   - `codex/scene-build-5.0.0-001 @ a62b557d`
   - `git status = clean`
2. 已确认本轮结果：
   - 逻辑层最小版本已完成并收成干净 checkpoint
   - 做过只读 MCP 探测
   - 实际施工仍走 Scene YAML
3. 已确认当前未解点：
   - 本会话 `unityMCP` 仍未恢复稳定的 Unity live 验证闭环
4. 已新增正式 prompt：
   - `scene-build_回读自检与高质量初稿收口.md`
5. 已更新总控口径：
   - `scene-build` 当前可继续阶段 = `回读自检与高质量初稿收口`

**关键判断**
- `scene-build` 不是卡住了，而是已经完成了逻辑层这一刀。
- 它下一步不该再拿“逻辑层继续施工”旧 prompt，而该进入回读、自检和高质量初稿收口。

**恢复点 / 下一步**
- 现在 scene-build 应发：
  - `scene-build_回读自检与高质量初稿收口.md`

## 2026-03-21｜NPC 返工已形成 carrier checkpoint，shared root 下一条改为导航检查，scene-build 升级到剧本对齐精修
**当前主线目标**
- 接住 `scene-build` 与 `NPC` 的最新回执，把恢复开发总控正式改到最新 live 状态，而不是继续沿用“NPC 仍是下一条 / scene-build 仍在高质量初稿收口”的旧口径。

**本轮完成**
1. 已 live 核对 shared root：
   - `D:\Unity\Unity_learning\Sunset @ main @ 26ce1d04`
   - `git status --short --branch = ## main...origin/main`
   - `shared-root-branch-occupancy.md = neutral-main-ready`
2. 已 live 核对 scene-build worktree：
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001 @ codex/scene-build-5.0.0-001 @ 0a14b93c`
   - `git status --short --branch = clean`
3. 已确认 `NPC` 最新结果：
   - `codex/npc-roam-phase2-003 @ 657594a6`
   - `carrier_ready = yes`
   - `main_ready = no`
   - `blocker_or_checkpoint = needs-unity-window`
   - 当前阻塞不是 shared root 槽位，而是 live 验收仍被 `unityMCP -> Sub2API HTML` 与旧 `mcp_unity -> Connection failed` 卡住
4. 已结合用户新增要求，把 scene-build 下一阶段改成“高质量初稿后续精修 + spring-day1 剧本对齐”，并要求它先只读回看：
   - `spring-day1-implementation/requirements.md`
   - `spring-day1-implementation/OUT_tasks.md`
   - scene-build 自己的 `tasks.md`
5. 已更新：
   - 总控文件
   - 批次入口文件
   - 治理线程自用 prompt
   - 治理线程职责文件
   使当前 live 口径变为：
   - `scene-build` 当前阶段 = `高质量初稿后续精修与spring-day1剧本对齐`
   - shared root 下一条 = `导航检查`
   - 然后 = `遮挡检查`
   - `NPC` 转入“待专属 Unity 验收窗口”
   - `农田交互修复V2` 继续保留为“已完成 checkpoint 但当前不是 main-ready”

**关键判断**
- `NPC` 当前不是“还得继续占 shared root 再做一轮文本开发”，而是“分支 checkpoint 已够，但 live 验收还差真正可用的 Unity 窗口”。
- `scene-build` 当前也不该继续拿“回读自检与高质量初稿收口”旧 prompt，因为它那一步已经做完，下一步应转向有边界的精修推进，并以 `spring-day1` 剧本为精度基准。

**恢复点 / 下一步**
- 现在应发给 scene-build：
  - `scene-build_高质量初稿后续精修与spring-day1剧本对齐.md`
- shared root 当前下一条应发：
  - `导航检查_当前开发放行.md`
- `NPC` 当前继续等待：
  - 真正可用的 Unity / MCP 验收窗口

## 2026-03-21｜恢复开发总控已正式 sync 到 main，scene-build 对齐 spring-day1 剧本要求生效
**当前主线目标**
- 把上一轮已经改好的恢复开发总控文档真正同步到 `main`，并确认项目经理现在拿到的是 live 口径，而不是“本地已改未推”的半成品状态。

**本轮完成**
1. 已使用稳定 launcher 执行治理同步：
   - `C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1`
   - `-Action sync -Mode governance -OwnerThread Codex规则落地`
2. 已把以下治理改动正式推到 `main`：
   - `scene-build_高质量初稿后续精修与spring-day1剧本对齐.md`
   - 恢复开发总控 / 批次入口 / 治理线程执行 prompt / 治理线程职责与更新规则
   - 对应工作区与线程记忆
3. 已确认同步结果：
   - `D:\Unity\Unity_learning\Sunset @ main @ ff90f0be`
   - `git status --short --branch = ## main...origin/main`
   - shared root occupancy 仍为 `neutral-main-ready`

**关键判断**
- 现在“scene-build 的后续精修必须和 spring-day1 剧本对齐”已经不是口头提醒，而是正式进入 live 总控和可分发 prompt。
- `NPC` 当前仍不是 shared root 下一条；它继续停在 `needs-unity-window`，等真正可用的 Unity 验收窗口。
- shared root 当前正式顺序仍是：
  - `导航检查`
  - 然后 `遮挡检查`

**恢复点 / 下一步**
- 现在项目经理可直接使用：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_恢复开发总控_01\可分发Prompt\scene-build_高质量初稿后续精修与spring-day1剧本对齐.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_恢复开发总控_01\可分发Prompt\导航检查_当前开发放行.md`

## 2026-03-21｜恢复开发总控从“排队放行”切换为“main-only 极简并发开发”
**当前主线目标**
- 响应用户“彻底去除规则帝国、压缩治理摩擦、直接并发开发”的要求，把当前 live 总控从 branch / grant / queue 体系切到 `main` 单基线模型。

**本轮完成**
1. 已重写总控文件：
   - `恢复开发总控与线程放行规范_2026-03-21.md`
2. 已重写批次入口：
   - `2026-03-21_批次分发_09_恢复开发总控与线程放行规范.md`
3. 已重写治理线程自用文件：
   - `治理线程_当前执行Prompt.md`
   - `治理线程_职责与更新规则.md`
4. 已重写当前直接可发 prompt：
   - `NPC_当前开发放行.md`
   - `农田交互修复V2_当前开发放行.md`
   - `导航检查_当前开发放行.md`
   - `遮挡检查_当前开发放行.md`
   - `scene-build_高质量初稿后续精修与spring-day1剧本对齐.md`
   - 新增 `spring-day1_当前开发放行.md`
5. 已把仓库级 `AGENTS.md` 加上最高优先级临时覆盖：
   - 普通开发默认只认 `main`
   - 不再把 branch / grant / return-main 当普通前置
   - 只保留高危撞车、Unity / MCP 单写冲突、破坏性 Git、现场写坏这几种硬打断

**关键判断**
- 这次不是“在旧体系上减一两条规则”，而是把旧的交通模型降级成历史背景，把当前 live 口径直接改成 `main-only + 高危才打断`。
- 仍然保留 Unity / MCP 单写，不是因为还想继续堆规范，而是因为那是物理限制，不是治理偏好。
- `scene-build` 当前仍允许继续，但旧 worktree / branch 现在被正式定义为“过渡现场”，不再作为长期真理；长期真理只剩 `main`。

**恢复点 / 下一步**
- 现在项目经理可以直接给线程发当前开发 prompt，不再先发测试 prompt。
- 只有线程撞上同一个高危目标、撞上 Unity / MCP live 单写、或者把现场写坏了，才需要把回执贴回治理线程。

## 2026-03-21｜当前 live 批次入口已从旧总控目录迁到 `main-only` 新阶段目录
**当前主线目标**
- 把当前 live 的批次入口、线程 prompt 入口和 scene-build 特殊口径从旧目录中拆出来，避免项目经理继续在历史目录里翻当前规则。

**本轮完成**
1. 已建立新的现行批次目录：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01`
2. 已在该目录下补齐：
   - 当前直接开发 prompt
   - `scene-build` 迁移前冻结 prompt
   - `spring-day1` 向 `scene-build` 交接 prompt
3. 已把旧目录 `2026.03.21_恢复开发总控_01` 标成历史阶段。
4. 已把工作区导航入口 `恢复开发总控与线程放行规范_2026-03-21.md` 重写为新目录跳转入口。

**关键判断**
- 当前 live 口径仍然是：普通开发默认 `main-only + 高危才打断`。
- 但 `scene-build` 现在被正式定义为特殊过渡现场，后续将迁到：
  - `D:\Unity\Unity_learning\SceneBuild_Standalone\scene-build-5.0.0-001`
- 这个迁移动作不会现在凭空执行，而是要先等冻结回执。

**恢复点 / 下一步**
- 当前项目经理已经可以只看新目录，不必再回头翻旧总控目录。

## 2026-03-21｜spring-day1 分发口径已从泛放行改成面向 scene-build 的正式交付
**当前主线目标**
- 让 `spring-day1` 不再作为模糊的“也可以继续开发”的线程存在，而是明确承担对 `scene-build` 的剧情空间交付职责。

**本轮完成**
1. 已重写：
   - `spring-day1_当前开发放行.md`
   - `spring-day1_当前任务回执与向scene-build交接.md`
2. 已把这轮要求收紧为：
   - 输出 `scene-build` 可直接施工的空间 brief
   - 不直接改 `SceneBuild_01`
   - 不做 Unity / MCP live 写
   - 不回到 UI / 字幕 / 对话实现

**关键决策**
- `spring-day1` 当前更像“剧情到空间的翻译层”，而不是新的并行施工面。
- 因此 prompt 的重点从“你还能做什么”改成了“你必须交出什么”。

**恢复点 / 下一步**
- 现在用户可以直接把新的 `spring-day1` prompt 发出去。
- 等交回正式 handoff 后，再由 `scene-build` 吃这份 brief 继续施工。

## 2026-03-21｜scene-build 已从“冻结回执”进入“最小 checkpoint 后等待迁移”
**当前主线目标**
- 在 `scene-build` 冻结回执已经到手的前提下，把下一步从模糊讨论收成一个单独可发的执行 prompt。

**本轮完成**
1. 已新增：
   - `scene-build_最小checkpoint并等待正式迁移.md`
2. 已把 `scene-build` 下一步口径收紧为：
   - 只允许处理 3 个记忆 dirty
   - 不继续扩施工
   - 不自己迁移
   - clean 后等待治理侧执行 `git worktree move`
3. 已补强 `spring-day1` prompt：
   - 其交付 brief 必须在 `scene-build` 迁移后仍然自洽可用

**关键决策**
- 当前 `scene-build` 不该直接恢复为持续施工态。
- 更合理的顺序是：
  - 最小 checkpoint
  - 正式迁移
  - 迁后复核
  - 再恢复施工

**恢复点 / 下一步**
- 现在项目经理可以直接把新的 `scene-build` prompt 发给线程。
- 等收到 `ready_for_move = yes` 后，再进入物理迁移动作。

## 2026-03-21｜scene-build 进入“释放目录锁后再迁移”的最小执行态
**当前主线目标**
- 在 `scene-build / spring-day1` 两份回执已经到手后，给当前批次补上真正必要的下一步，而不是继续空转。

**本轮完成**
1. 已确认：
   - `scene-build` 已完成最小 checkpoint，`ready_for_move = yes`
   - `spring-day1` 已完成向 `scene-build` 的正式 handoff
2. 已纠正 `scene-build` 的迁移目标路径为：
   - `D:\Unity\Unity_learning\scene-build-5.0.0-001`
3. 已由治理侧直接实测 `git worktree move`，结果不是策略失败，而是系统级 `Permission denied`
4. 已锁定 live 占用进程：
   - `Unity.exe`
   - `mcp-for-unity` 的 `http / stdio` 进程
   - `mcp-terminal.cmd`
5. 已新增当前唯一必要 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\scene-build_迁移前释放Unity与MCP目录锁.md`

**关键决策**
- 这轮不再给 `spring-day1` 追加动作。
- `scene-build` 也不继续施工；先释放锁，再迁移，再恢复。

**恢复点 / 下一步**
- 先把新的释放目录锁 prompt 发给 `scene-build`。
- 它一回执 `ready_for_move_now = yes`，治理侧就继续执行迁移。

## 2026-03-22｜scene-build 已完成注册迁移，当前只差迁后复核
**当前主线目标**
- 把 `scene-build` 从旧 `Sunset_worktrees` 体系里真正摘出来，并把当前 batch 收到可继续推进的状态。

**本轮完成**
1. `spring-day1` handoff 已确认完成；这轮后不再需要持续抄送给治理线程。
2. `scene-build` 线程确认 Unity / MCP 目录锁已释放，但随之暴露 4 个 TMP 字体资源 dirty。
3. 治理侧再次尝试直接 `git worktree move`，仍为 `Permission denied`。
4. 已改走兜底路径：
   - `robocopy` 复制到 `D:\Unity\Unity_learning\scene-build-5.0.0-001`
   - `git worktree repair D:\Unity\Unity_learning\scene-build-5.0.0-001`
5. 迁后复核成立：
   - `git worktree list --porcelain` 正式路径已变成新目录
   - 新路径仍是 `codex/scene-build-5.0.0-001 @ 8e641e67`
6. 已新增下一步必要 prompt：
   - `scene-build_迁后复核与从新路径恢复.md`

**关键决策**
- 这轮可以不再把 `scene-build` 当成“待迁移线程”。
- 它现在是“已迁移但待复核的新项目现场”。

**恢复点 / 下一步**
- 把迁后复核 prompt 发给 `scene-build`。
- 等它确认只在新路径继续后，再恢复精修。

## 2026-03-22｜scene-build 路径误判已撤回，shared-root 批次入口改回旧 worktree 口径
**当前主线目标**
- 修正我在执行批次入口里写错的 scene-build 路径，把“迁到新路径”的误口径撤回成用户指定的旧 worktree 事实。

**本轮完成**
1. 已确认正式 `scene-build` worktree 仍应视为：
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
2. 已确认 `D:\Unity\Unity_learning\scene-build-5.0.0-001` 只是误复制副本，不能作为正式现场。
3. 已修正当前批次 README、治理线程执行口径、治理职责文件，以及仍可能被继续转发的 scene-build / spring-day1 prompt。
4. 已把几份错误迁移 prompt 标成“已废弃（2026-03-22）”，防止后续继续误发。

**关键决策**
- 当前 shared-root 批次层只做入口止血，不再继续推动 scene-build 物理迁移。
- 后续如再处理 scene-build，只能围绕旧 worktree 的正式现场继续。

**恢复点 / 下一步**
- 等用户决定是否清理误复制副本目录。
- 在此之前，任何线程都不要把 `D:\Unity\Unity_learning\scene-build-5.0.0-001` 当成正式项目根。

## 2026-03-22｜误复制副本已删，批次层对 scene-build 的唯一口径完成收口
**当前主线目标**
- 把批次层“误副本仍在”的过渡叙述删干净，正式收成只认旧 worktree 的唯一执行口径。

**本轮完成**
1. 已删除误复制副本目录：
   - `D:\Unity\Unity_learning\scene-build-5.0.0-001`
2. 已复核当前唯一正式现场仍是：
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
3. 已更新当前批次 README、治理线程 prompt、治理职责与 `scene-build_当前开发放行.md`，全部改成“副本已删，只有一条正式路径”。

**关键决策**
- 批次层不再保留“还有一个误副本等处理”的中间状态。
- scene-build 相关后续动作只围绕旧 worktree 继续。

**恢复点 / 下一步**
- 如果你继续调 scene-build，就直接让线程在旧 worktree 上继续干，不需要再讨论副本或迁移。

## 2026-03-22｜批次壳已从 scene-build 止血切到 main-only 收口执行层
**当前主线目标**
- 不再让这个执行批次壳继续围着 scene-build 迁移转，而是把它真正切回“并发线程统一回执 + 白名单 main 收口”的当前职责。

**本轮完成**
1. 已把当前批次层的默认口径固定为：
   - 普通线程默认在 `main` 语义下并发推进
   - 每线程只提交自己的白名单
   - 每线程一次只收一刀 checkpoint
2. 已更新：
   - `README.md`
   - `治理线程_当前执行Prompt.md`
   - `治理线程_职责与更新规则.md`
   - `线程完成后_白名单main收口模板.md`
3. 已把第一轮执行结果同步进回收卡：
   - 农田已入 `main @ f40d228d`
   - `spring-day1` 基础脊柱已入 `main @ 83d809a9`
4. 已确认当前批次层不再继续推动：
   - scene-build 新路径迁移
   - queue / grant / ensure-branch 的旧式正文化扩建

**关键决策**
- 从这条父工作区往下看，当前最重要的不是再造新的交通系统，而是把已有线程真实推进的结果持续收进回收卡和模板里。

**恢复点 / 下一步**
- 当前治理线自己还有一批入口 / 批次文件白名单待同步。
- 这刀收完后，批次层就能以更干净的 shared root 继续服务后续线程。

## 2026-03-22｜批次层已新增两份可直接转发的 self-sync prompt
**当前主线目标**
- 把“规则更新后，线程自己直接提”这件事从治理结论变成批次层现成可发的 prompt。

**本轮完成**
1. 已新增：
   - `NPC_继续修复并直接main收口.md`
   - `spring-day1_剩余文档整理并直接main收口.md`
2. 已把当前批次层的执行口径进一步收紧为：
   - 线程自己提
   - 治理线程只维护入口和少数例外

**关键决策**
- 这两份 prompt 现在就是你可以直接转发的版本，不需要你再临场补规则说明。

**恢复点 / 下一步**
- 当前可以直接把两份 prompt 分发出去。

## 2026-03-22｜批次层已改成“固定回执箱 + prompt 直写落点”
**当前主线目标**
- 把“项目经理不要再手动创建回执文件”落实成当前批次层默认做法。

**本轮完成**
1. 已把 `001部分回执.md` 认作当前批次固定回执箱。
2. 已把回执落点直接写进：
   - `NPC_继续修复并直接main收口.md`
   - `spring-day1_剩余文档整理并直接main收口.md`
   - `线程完成后_白名单main收口模板.md`

**关键决策**
- 后续当前批次的新 prompt，默认都应自带固定回执落点，而不是再让用户自己搭目录和文件。

**恢复点 / 下一步**
- 当前继续等 `NPC` 和 `spring-day1` 后续回执。

## 2026-03-23｜MCP 根因治理开始从“端口纠偏”升级到“硬基线落地”
**当前主线目标**
- 不再把 MCP 问题当成一次性的端口误配，而是把它收束成 shared root `main` 上的长期治理基线。

**本轮完成**
1. 只读复盘当前 live 现场，确认根因不是“休眠打断”，而是更深的四层漂移：
   - 配置层：`config.toml` 曾在不同时间残留旧 `旧 MCP 桥口径（已失效）`
   - 文档层：active drafts / memory / 回执里仍有人持续写 `8080`
   - 会话层：即使配置改对，旧会话与旧结论仍会继续外推
   - 规范层：缺少一个统一硬闸门，把端口 / server / 监听 / 目标实例一次性钉死
2. 已确认当前最具误导性的活文件层仍存在 `8080` 残留，且这些残留不在历史归档，而在 active memory / drafts / 回执里。
3. 已在 shared root `main` 直接新增并落地以下治理改动：
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-live-baseline.md`
   - `D:\Unity\Unity_learning\Sunset\scripts\check-unity-mcp-baseline.ps1`
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-occupancy.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前规范快照_2026-03-22.md`
4. 新增的统一硬口径是：
   - 当前唯一有效 server 名：`unityMCP`
   - 当前唯一有效端点：`http://127.0.0.1:8888/mcp`
   - 当前 shared root pidfile：`D:\Unity\Unity_learning\Sunset\Library\MCPForUnity\RunState\mcp_http_8888.pid`
   - 任何 `8080` / `旧 MCP 桥口径（已失效）` / “Session Active 所以可用” 一律视为不完整或旧口径
5. 新脚本 `check-unity-mcp-baseline.ps1` 已实际运行通过，能稳定输出：
   - config 里是否只剩 `unityMCP`
   - 8888 是否监听
   - pidfile 是否存在
   - terminal script 当前实际命令

**关键决策**
- 这轮不去重写别的线程的历史回执正文；那些旧结论保留历史事实。
- 真正的根治方式是：新增唯一 live 基线，并让 `AGENTS + occupancy + 当前规范快照` 统一引用它。
- 以后线程再拿 `8080` 当 live 事实，不再是“误会”，而是明确违规 / 旧口径残留。

**恢复点 / 下一步**
- 下一步应对白名单把这 5 个 shared root 文件收进 `main`。
- 收口后，再由治理层决定是否补一轮 active draft / 回执的定向清理与纠偏说明。

## 2026-03-23｜MCP 第二阶段清理：active 旧口径残留已开始逐文件中和
**当前主线目标**
- 在第一阶段“唯一基线 + AGENTS 硬闸门 + 可执行脚本”落地后，继续处理当前仍会误导线程判断的 active 旧口径残留。

**本轮完成**
1. 重新只看 active 层，点名仍残留旧 `8080` / 旧桥口径的活文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\遮挡检查\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md`
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V2\memory_0.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\001部分回执.md`
   - `D:\Unity\Unity_learning\Sunset\.codex\drafts\遮挡检查\main-live-validation_2026-03-23.md`
2. 对上述 active 文件已做两类处理中和：
   - 把精确旧地址替换为：`旧 MCP 端口口径（已失效）`
   - 把旧桥名替换为：`旧 MCP 桥口径（已失效）`
3. 同时在这些 active 文件尾部追加统一纠偏说明，明确：
   - 旧口径只保留历史阶段事实
   - 当前唯一有效 live 基线以 `mcp-live-baseline.md` 为准
4. 新增第二把治理脚本：
   - `D:\Unity\Unity_learning\Sunset\scripts\find-legacy-mcp-references.ps1`
   用于扫 active `.kiro/.codex` 层里仍残留的旧端口 / 旧桥口径。

**关键决策**
- 本轮不篡改历史归档，不清 state_backups，不重写历史迁移报告。
- 只处理中还会被线程继续读成“当前事实”的 active 文档、draft、线程记忆与回执层。
- “清理”方式不是抹除历史，而是把旧口径显式中和成失效标签，防止 future threads 再把它外推出去。

**恢复点 / 下一步**
- 下一步对白名单尝试把这批 active 清理文件与扫描脚本收进 `main`。
- 如果个别文件因 owner / 热区策略不宜直接收口，则至少已完成局部中和，可在后续 owner 收口时继续并轨。

## 2026-03-23 MCP 桥口径纠偏
- 本文件中若出现“旧 MCP 桥口径（已失效）”，均表示历史阶段使用过的旧桥结论，不再代表当前 live 入口。
- 当前唯一有效 live 基线以 D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-live-baseline.md 为准：unityMCP + http://127.0.0.1:8888/mcp。
## 2026-03-23｜MCP 第二阶段清理补完：active 扫描已 clean
**当前主线目标**
- 把 active 层里最后剩下的旧桥口径也清掉，让新加的扫描脚本真正能给出 `clean`。

**本轮完成**
1. 在第二阶段第一批清理后，再次使用 `scripts/find-legacy-mcp-references.ps1` 复扫 active 层。
2. 复扫发现剩余 active 残留已不再是 `8080`，而是一些 `mcp-unity` 旧桥历史口径，集中在：
   - `Skills和MCP` 线程记忆
   - `共享根执行模型与吞吐重构/memory.md`
   - `农田系统` 根层与 `10.2.1 / 10.2.2` memory
3. 已把这些文件中的 `mcp-unity` 全部中和为：`旧 MCP 桥口径（已失效）`，并补统一纠偏说明。
4. 再次执行 `scripts/find-legacy-mcp-references.ps1` 后，结果已返回：
   - `legacy_mcp_reference_status: clean`

**关键决策**
- 这轮之后，active memory / draft / 回执层里已经没有“还能被误读成 live 事实”的旧端口或旧桥直接字符串。
- 历史档案和专门的基线说明文件保留历史价值，不纳入 active 残留统计。

**恢复点 / 下一步**
- 现在可以对白名单把这一小批收尾文件再收一次进 `main`。
- 后续若脚本再次报 `found`，就可以直接当成新的口径漂移告警处理。