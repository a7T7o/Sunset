# 23_前序阶段补漏审计与并发交通调度重建

## 2026-03-19｜阶段建立

**用户目标**
- 用户明确要求：
  - 不要直接沿着旧阶段继续滑行
  - 先对前面所有阶段做排查、总结、补漏
  - 防止旧阶段基础没有真正收尾，新阶段却误以为前提已经成立
- 同时用户再次纠偏目标方向：
  - 不是把 shared root 做成“单人设施”
  - 而是最终做成“有交规、可排队、可等待、能消化中断”的并发交通系统

**本阶段为何建立**
- `22` 证明了恢复开发运营能跑起来，但还不是并发调度终局。
- 旧阶段任务板与 live 现实已经出现分叉，尤其是 `21/22`。
- 因此必须新开 `23`，专门承接：
  - 前序阶段补漏审计
  - 旧任务板回填 / 迁移说明
  - 并发交通调度模型重建

**当前已知基线**
- `D:\Unity\Unity_learning\Sunset @ main @ 1f4bc6bb`
- `git status --short --branch = ## main...origin/main`
- `shared-root-branch-occupancy.md = main + neutral`
- `导航检查` 已完成一次 docs-first 阶段二闭环

**当前恢复点**
- 先完成 `01-12、20-22` 的全面审计与分级。
- 再决定哪些旧阶段需要补写审计备注、哪些内容应正式迁入 `23`。
- 在此之前，不再把“single-writer 串行”误当成最终模型。

## 2026-03-19｜审核 `gemini.md` 后的纠偏吸收

**审核目标**
- 用户要求我完整阅读：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\gemini.md`
- 并结合 `23` 当前正文，重新审视我自己的阶段判断。

**审核结论**
- 路径判断：`Path B`
- 也就是：
  - Gemini 指出的核心问题真实存在
  - 但它给出的立即执行方案，当前不能原样落地

**我采纳的部分**
- “物理单写”不等于“逻辑单线程”这个判断是对的。
- `23` 的确不该继续只写“单槽位串行”，而是应构建：
  - 排队
  - 等待态
  - 中断消化
  - 多 checkpoint 推进

**我拒绝原样照抄的部分**
- 当前还没有真正的等待队列 / 挂起状态载体 / 自动唤醒逻辑。
- `git-safe-sync.ps1` 目前也没有真正的 `yield / wait / queue` 自动行为。
- 所以不能现在就直接群发“所有业务线程一起去抢租约”的 prompt。

**当前恢复点**
- `23` 接下来应先把调度模型写成正式正文，再决定是否进入“并发唤醒实盘”。
- 在此之前，Gemini 的内容作为方向参考有效，但不能直接当执行脚本。

## 2026-03-19｜queue 脚本兼容性修补与最小实测

**用户目标**
- 继续沿着 `23` 的方向推进，不把并发交通系统停留在纸面。
- 优先解决 queue-aware 脚本入口的真实运行 bug，再判断是否具备后续 rollout 条件。

**本轮完成**
- 修复了 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 中 queue runtime 读取的 PowerShell 兼容性问题：
  - 去掉 `ConvertFrom-Json -Depth`
  - 为空 runtime 增加回退
  - 为坏 JSON 增加显式 `FATAL`
- 保留 `request-branch` 作为非致命入口，不去破坏旧 `grant-branch` 的既有阻断语义。
- 在当前治理 dirty 场景下完成一次最小实测：
  - `git-safe-sync.ps1 -Action request-branch -OwnerThread "导航检查" -BranchName "codex/navigation-audit-001" -CheckpointHint "docs-ready" -QueueNote "stage23-validation"`
  - 返回 `STATUS: LOCKED_PLEASE_YIELD`
- 确认 queue runtime 会写入：
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\active\shared-root-queue.lock.json`
- 已将本轮验证票据清空为基线空队列，避免误导后续真实调度。

**关键判断**
- 这轮验证说明 Sunset 终于拥有了“抢不到槽位时，不是原地 `FATAL` 死掉，而是能进入等待态”的物理入口。
- 但这还不是完整调度层：
  - 还没有自动唤醒
  - 还没有 cancel / requeue / 越序审批
  - 也还没有完整负例矩阵
- 所以当前仍不能把 `23` 误说成“已经可以全员并发实盘抢租约”。

**当前恢复点**
- 下一步继续补：
  - waiting/granted/task-active/completed/cancelled 的完整状态流
  - queue 消费 / 取消 / 唤醒 / 回执协议
  - 真正适合 rollout 的实盘验证矩阵
