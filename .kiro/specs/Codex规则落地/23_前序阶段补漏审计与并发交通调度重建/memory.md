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

## 2026-03-19｜queue 闭环演习完成，并补入 runtime 自愈

**用户目标**
- 不是只要设计草案，而是要求我一步到位把 queue 的 wake / cancel / requeue 补成真正可用的闭环，并且在推进过程中持续客观审视自身。

**本轮完成**
- 在 `git-safe-sync.ps1` 中新增：
  - `cancel-branch-request`
  - `requeue-branch-request`
  - `wake-next`
- 为 `return-main` 新增 `NEXT_IN_LINE_*` 输出。
- 为 queue runtime 新增自愈逻辑：
  - `Repair-SharedRootQueueRecord`
  - 依据 `occupancy` 修补 stale `task-active / granted`
- 新增文档：
  - `queue-rollout-matrix.md`
- 完成一轮 Git 层实盘演习，验证：
  - 首次 `request-branch -> GRANTED`
  - 后续线程 `LOCKED_PLEASE_YIELD`
  - `requeue-branch-request` 重新排到队尾
  - `wake-next` 唤醒队首
  - 再次 `request-branch -> ALREADY_GRANTED`
  - `cancel-branch-request` 释放 grant
  - `wake-next` 在已有未消费 grant 时返回 `WAKE_BLOCKED`
- 演习结束后已恢复：
  - `D:\Unity\Unity_learning\Sunset @ main`
  - `git status --short --branch = ## main...origin/main`
  - runtime queue 为空基线

**关键判断**
- 现在可以更有把握地说：
  - Git 层的 queue / wait / wake / cancel / requeue 闭环已经真实存在
  - 不再只是文档口径
- 但我也必须保留一个清醒判断：
  - 老任务分支里可能仍是旧版 `git-safe-sync.ps1`
  - 所以“所有任务分支天然携带最新闸机”这件事还不能吹成已完成
- 本轮的自愈逻辑已经显著缓解这个问题：
  - 即使旧分支脚本留下 stale open entry
  - 回到 `main` 后，新版脚本也会按 `occupancy` 修回 queue runtime

**当前恢复点**
- `23` 现在可以从“写出 queue 机制”进入“评估是否需要更强的脚本分发/launcher 方案”。
- 若继续补强，下一轮重点不再是基础 queue，而是：
  - 旧分支脚本漂移的更彻底解法
  - Unity / MCP 层的高风险联动调度

## 2026-03-19｜稳定 launcher 落地，旧分支脚本漂移风险继续收口

**用户目标**
- 在 Git 层 queue 已经跑通后，不再停在“旧分支脚本漂移先靠 runtime 自愈兜底”的状态，而是继续把 live 入口做稳。
- 同时要求我保持客观，不把现状夸大成“已经全自动调度完成”。

**本轮完成**
- 新增仓库外稳定 launcher：
  - `C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1`
- launcher 的执行策略已固定为：
  - 在 `D:\Unity\Unity_learning\Sunset` 上读取 `main:scripts/git-safe-sync.ps1`
  - 写到临时文件
  - 再以该 canonical 版本执行 live 调度命令
- 已同步更新当前现行文档口径：
  - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-queue.md`
  - `dispatch-protocol.md`
  - `queue-rollout-matrix.md`
  - `tasks.md`
  - `analysis.md`
- 已完成一轮最小烟雾验证：
  - 从仓库外 `D:\迅雷下载\开始` 执行 launcher 的 `preflight`
  - launcher 正确输出：
    - `LAUNCHER_MODE`
    - `LAUNCHER_REPO_ROOT`
    - `LAUNCHER_SOURCE_REF`
    - `LAUNCHER_LIVE_BRANCH`
  - canonical 主脚本成功执行，shared root queue runtime 未被污染

**本轮暴露并修复的真实 bug**
- launcher 初版自身存在 PowerShell 语法拼接错误
- Windows PowerShell 5.1 在 `-File` 执行临时 UTF-8 无 BOM 且含中文脚本时，会出现解析异常
- launcher 初版参数转发把整串参数误当成一个参数
- 当前收口方式：
  - launcher 自身改成 ASCII-only
  - 临时 canonical 脚本改为 UTF-8 with BOM
  - 参数改为真正的数组转发

**关键判断**
- 现在更客观的状态是：
  - Git 层 queue / wait / wake / cancel / requeue 闭环成立
  - 旧分支脚本漂移不再直接卡住 live 调度入口
  - 但这仍不是“全项目所有层都自动化调度完成”
- 当前剩余重点仍是：
  - 多 checkpoint 持续推进模型
  - Unity / MCP 单实例调度边界
  - 治理线程的批次发放 / 回收规范继续收口

**当前恢复点**
- 之后若继续发放业务线程的 live 调度命令，应默认改用：
  - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 ...`
- 治理线程如需继续补阶段 23，不必再重复造 queue 本体，而应把注意力放在：
  - 并发 checkpoint 模型
  - Unity / MCP 层
  - 调度运营协议

## 2026-03-19｜live 准入前先清场：已为 `Skills和MCP` 落一轮根层批次分发

**用户目标**
- 在 stable launcher 已经落地后，继续主线，不停在“建议下一步”层。
- 直接进入恢复开发的实际运营动作，而不是再写一轮空分析。

**本轮完成**
- 再次复核 live shared root：
  - `branch = main`
  - `occupancy = neutral`
  - `queue runtime = empty`
- 但同时发现一个新的 live 阻断：
  - Git working tree 当前仍有 3 个 dirty
  - 且都落在 `Skills和MCP / Steering规则区优化` 这一线
- 这意味着：
  - 现在还不应直接向业务线程发放 `request-branch / ensure-branch`
  - 否则线程大概率会被 shared root 闸机拦下
- 已按根层协议落一轮新的批次运营文件：
  - 根层批次分发文件：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_01_稳定launcher复工前清场.md`
  - 线程专属 prompt：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_稳定launcher复工前清场\可分发Prompt\Skills和MCP.md`
  - 固定回收卡：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_稳定launcher复工前清场\线程回收\Skills和MCP.md`

**关键判断**
- 这轮“直接开始”的最正确动作不是硬发业务准入，而是先把当前真实 blocker 纳入治理流程。
- 目前阻断业务 live 准入的不是 queue 本身，也不是 launcher 本身，而是 shared root 还有别线 dirty 未收口。
- 因此当前裁定为：
  - 先给 `Skills和MCP` 发清场专属 prompt
  - 收件并确认 `main + clean` 后
  - 再进入下一轮业务线程 live 准入分发

**当前恢复点**
- 用户现在可以直接群发：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_01_稳定launcher复工前清场.md`
- 治理线程下一步应：
  - 等 `Skills和MCP` 回收卡
  - 读取回收卡并确认 shared root 是否 clean
  - 若 clean，再生成下一轮业务线程 live 准入批次

## 2026-03-19｜Skills和MCP 清场回收已完成
**用户目标**
- 领取 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_稳定launcher复工前清场\可分发Prompt\Skills和MCP.md`，核对 live Git 现场并在一致时执行稳定 launcher 的治理收口。

**本轮完成**
- 在 `D:\Unity\Unity_learning\Sunset` 只读复核到：`branch = main`、`HEAD = cfeedf33`、`git status --short --branch = ## main...origin/main`。
- 确认 dirty 恰好只有 3 个预期文件：
  - `.codex/threads/Sunset/Skills和MCP/memory_0.md`
  - `.kiro/specs/Steering规则区优化/memory.md`
  - `.kiro/specs/Steering规则区优化/当前运行基线与开发规则/memory.md`
- 已执行稳定 launcher 的治理同步并成功创建 / 推送提交：`main @ 0b41d4ed`。
- 已把结果写回固定回收卡：`D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_稳定launcher复工前清场\线程回收\Skills和MCP.md`。

**关键判断**
- 本轮 live blocker 不是 occupancy、queue 或 launcher 故障，而是 `Skills和MCP` 线程遗留的 3 个治理 / 记忆 dirty。
- 该 blocker 现已收口，shared root 当前回到 `main + clean`。

**恢复点**
- 治理线程现在可继续读取固定回收卡，并决定是否发放下一轮业务线程 live 准入。
