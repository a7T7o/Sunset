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
