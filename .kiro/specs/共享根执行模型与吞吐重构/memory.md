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
