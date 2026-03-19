# Codex规则落地 Thread Memory Continuation 2

## 2026-03-18｜shared root 恢复验证收口

**用户目标**
- 当前主线不是再写新规范，而是把 Sunset 真正恢复到“全部健康、可以继续开发”的状态。
- 本轮阻塞处理服务于这条主线：先核清 `farm` 二阶段阻断，再把还没自愈完的 continuation branch 补齐。

**本轮完成**
- 现场核查发现 shared root 实际卡在 `codex/spring-day1-story-progression-001 @ a9c952b7`，不是外部汇报里的 `main`。
- 已把 shared root 直接回正到 `main`。
- 已亲自完成 `farm` 的闭环复测：
  - `grant-branch`
  - `ensure-branch`
  - `return-main`
  全部通过。
- 已确认此前 `farm` 的 FATAL 不再是当前主线故障。
- 已处理 `spring-day1` 的 branch-local drift，并在其分支上推送治理热修：
  - `27dc06a1`
- 已亲自完成 `spring-day1` 的闭环复测：
  - `grant-branch`
  - `ensure-branch`
  - `return-main`
  全部通过。

**关键判断**
- 当前 shared root 主闸机已经能支撑真实恢复开发。
- 当前主要 continuation branch 的健康状态：
  - `codex/farm-1.0.2-cleanroom001`：健康
  - `codex/npc-roam-phase2-003`：健康
  - `codex/spring-day1-story-progression-001`：已自愈完成，健康
- `导航` / `遮挡` 目前没有现成 continuation branch 需要做同样 graft。

**当前现场**
- `D:\Unity\Unity_learning\Sunset`
- `main @ 1add175b`
- `git status --short --branch` clean
- `.kiro/locks/shared-root-branch-occupancy.md` 为 `main + neutral`

**恢复点**
- 阻塞处理已完成，主线回到“继续发放线程准入 prompt，并恢复业务开发”。
- 这轮不新增阶段 22 正文；若后续要做自动化 hook，只留在 TD/后续补强，不抢占当前恢复窗口。

## 2026-03-18｜5 线程唤醒 / 准入 prompt 已成包

**本轮完成**
- 已产出 5 个线程当前版唤醒 / 准入 prompt，并落盘：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\恢复开发唤醒与准入Prompt_2026-03-18.md`
- 已把剩余未完成项同步写入同一文件，避免后续继续在对话里口头漂移。

**当前剩余真正待办**
- 用户分发 prompt 并收回阶段一结果。
- 按 shared root 单写者模型，串行发放阶段二 grant。
- 历史治理回读主线仍在暂停中，后续若恢复治理深挖，再从暂停点继续。

## 2026-03-18｜阶段22工作区与收件箱已补齐

**本轮完成**
- 已正式把“恢复开发发放与回收”独立成阶段 22：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收`
- 已在该阶段内建立：
  - 5 个线程的可分发 prompt 文件
  - 5 个线程的固定回收卡
  - 一份总入口说明
- 旧根层 prompt 文件已降级成路由页，不再承载正文。

**这意味着**
- 用户后续不需要再手工补“回写到哪、按什么格式写”。
- 我后续优先读取阶段 22 的 `线程回收/`，不再从聊天里人工摘证据。

## 2026-03-18｜阶段一全量收件后，治理裁定完成

**用户目标**
- 用户要求我去阶段 22 的“收件箱”正式接收全部阶段一结果，并把“先群发统一入口、线程自己领取专属 prompt”的模式写成正式流程，以后类似场景都复用。

**本轮完成**
- 复核当前 live Git 现场为 `D:\Unity\Unity_learning\Sunset @ main @ d0c6bb72`，当前 dirty 属于阶段 22 治理回写与线程 memory 回写，不是 shared root 失控。
- 正式审阅并接收 5 张阶段一回收卡：
  - `NPC`
  - `农田交互修复V2`
  - `spring-day1`
  - `导航检查`
  - `遮挡检查`
- 已把治理裁定直接回写到 5 张固定回收卡的“治理裁定区”。
- 已一度在阶段 22 内生成过统一群发领取入口文件，但该做法后来被用户纠偏。
- 已新增阶段一审核汇总：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\阶段一审核结论与阶段二分发建议.md`
- 已修正 5 份线程专属 prompt 里的静态 `HEAD`，改为“所有 branch / HEAD / status 以 live preflight 为准”。

**关键判断**
- 阶段一 5 条线程全部通过，没有发现越级写入。
- 阶段二仍必须保持单写者串行。
- 推荐 `导航检查` 作为首个低风险阶段二准入对象，因为它的第一 checkpoint 只做 docs 固化，不需要先碰 Unity / Play Mode / 热文件。
- `农田交互修复V2 / spring-day1 / NPC` 共享下一个业务写入槽位，需按用户业务优先级三选一。
- `遮挡检查` 可准入，但更适合放在导航之后。

**恢复点**
- 阶段 22 现在已经从“分发准备态”进入“收件、裁定、串行放行”态。
- 后续类似群发，不再手工复制每个线程的专属 prompt，但批次分发文件由治理线程按轮次在根层生成。

## 2026-03-18｜批次分发文件归属纠偏：上提到治理根层并改成专用 skill

**用户纠偏**
- 用户明确指出：这类批次分发文件不属于阶段 22 资产，也不是应被维护的固定 prompt。
- 正确口径应是：治理线程在需要分发 / 收件时，按轮次在 `Codex规则落地` 根层生成一份本轮批次文件。

**本轮修正**
- 已删除阶段 22 下的 `00_统一群发领取入口.md`。
- 已新增根层规则：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\治理线程批次分发与回执规范.md`
- 已新增治理线程专用 skill：
  - `C:\Users\aTo\.codex\skills\sunset-governance-dispatch-protocol\`
- 已更新 `D:\Unity\Unity_learning\Sunset\AGENTS.md`，把这类任务接入新的强制路由点。

**恢复点**
- 后续若再次组织新一轮群发，先按根层规则生成本轮批次文件，再让线程领取各自专属 prompt。
- 当前 shared root 继续保持串行写入模型，不因导航阶段二成功就放开为“所有线程并行开工”。

## 2026-03-19｜用户对阶段目标的关键纠偏已确认

**用户目标**
- 用户明确指出：他要的不是“始终依次进入”的单人设施模型，而是“完善交规后的并发交通系统”。
- 线程应能在各自准备就绪时申请切分支；切不进去时应等待或转入后续可做内容，而不是把整体开发理解成永久单线程。

**本轮完成**
- 重新基于 live 现场确认：
  - `D:\Unity\Unity_learning\Sunset @ main @ 1f4bc6bb`
  - `git status --short --branch = ## main...origin/main`
  - occupancy 仍为 `main + neutral`
- 明确给当前治理主线重新定性：
  - 现阶段已完成：强制入口、分支准入、return-main、最小闭环、批次分发 / 回执机制
  - 现阶段未完成：排队层、等待态模型、中断消化、可继续推进的多 checkpoint 并发模型

**关键判断**
- 用户的质疑是对的：我之前如果把“single-writer 串行”继续表述成终局，就是把阶段 22 的当前约束误说成最终目标。
- 更准确的说法应该是：
  - 当前已经修到“有闸机、有红绿灯、能受控通行”
  - 但还没修到“有完整路网、有排队规则、有等待态治理”

**恢复点**
- 当前主线先回到：对用户明确说明“现在不是全面修正完毕，而是入口层完成、调度层待补”。
- 后续治理若继续推进，应单独立项补“并发排队调度与等待态治理”，而不是继续污染阶段 22。

## 2026-03-19｜按用户要求正式新立 23，并启动前序阶段全面补漏审计

**用户目标**
- 用户明确补充：不能一边开新阶段，一边让旧阶段维持误导性的未收口状态。
- 因此需要：
  - 先对前序阶段做排查、总结、补漏
  - 再用新阶段承接方向变化后的主线

**本轮完成**
- 全量扫描 `Codex规则落地` 阶段目录，确认现有阶段为：
  - `01-12`
  - `20-22`
- 新建立项：
  - `23_前序阶段补漏审计与并发交通调度重建`
- 已在 `23` 中落下：
  - `tasks.md`
  - `analysis.md`
  - `memory.md`
- 已对最容易误导后续判断的旧阶段补写审计备注：
  - `20`
  - `21`
  - `22`

**关键判断**
- 现在的正确推进方式不是“直接在 22 上继续扩”，而是：
  - `22` 继续负责恢复开发运营
  - `23` 负责前序阶段补漏审计与并发交通模型重建
- 这样可以避免新阶段误以为旧基础已经全部做完，也能避免旧阶段任务板继续制造假未完成或假已完成。

**恢复点**
- 接下来继续在 `23` 内细化阶段分类与迁移表。
- 完成审计后，再进入并发排队 / 等待态 / 中断消化模型的正式正文设计。

## 2026-03-19｜对 `23/gemini.md` 的审核已吸收进主线判断

**用户目标**
- 用户要求我不要只是看一眼 Gemini 的内容，而要真正消化后再重新审视自己的判断。

**本轮完成**
- 按 review 路由核查了：
  - `gemini.md`
  - `code-reaper-review.md`
  - 当前 `23` 正文
  - `git-safe-sync.ps1` 现状
- 结论为：
  - 路径 `B`
  - 方向正确，执行时机过早

**关键判断**
- Gemini 对“逻辑并发 + 物理单写”的区分是对的，这也再次证明 `23` 立项方向正确。
- 但在当前没有实际 queue / wait / wake 机制时，直接群发“全员去抢租约”的 prompt 会越过实现基础。
- 所以当前最合理的吸收方式是：
  - 把它的方向收进 `23`
  - 不把它的立即执行方案直接发出去

**恢复点**
- 继续把 `23` 做成真正的调度层正文。
- 等实现和运行口径准备好后，再把 Gemini 那套“并发挂起与等待”升级成可执行方案。

### 会话 34 - 2026-03-19（修复 queue runtime 兼容性并跑通 `request-branch` 最小验证）
**用户目标**：
> 基于当前 `23` 主线继续做真正的物理调度器，不停留在文档层；优先修复 queue 入口脚本的运行时 bug，并完成安全验证。

**已完成事项**：
1. 读取并复核：
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\tasks.md`
   - `analysis.md`
   - `dispatch-protocol.md`
   - `shared-root-queue.md`
2. 确认 queue runtime 首个真实阻断点是脚本兼容性，而不是调度模型本身：
   - `ConvertFrom-Json -Depth 10`
   - 当前 live PowerShell 不支持该参数
3. 在 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 中完成最小修补：
   - 去掉 `ConvertFrom-Json -Depth`
   - 增加空 runtime 回退
   - 增加坏 JSON 的显式 `FATAL`
4. 运行最小验证命令：
   - `git-safe-sync.ps1 -Action request-branch -OwnerThread "导航检查" -BranchName "codex/navigation-audit-001" -CheckpointHint "docs-ready" -QueueNote "stage23-validation"`
   - 成功返回 `STATUS: LOCKED_PLEASE_YIELD`
5. 已把验证用 runtime queue 回写为空基线，避免把假票据留作后续真实等待条目。

**关键决策**：
- 当前应保留 `request-branch` 作为 queue-aware 的非致命入口，不直接修改旧 `grant-branch` 的语义。
- 现在可以证明 Sunset 已有“活着等待”的物理入口，但还没有完整自动唤醒和消费队列能力。
- 因此仍不能把当前状态宣传成“已可全员并发实盘抢租约”。

**恢复点**
- 继续在 `23` 内完成：
  - 等待态完整状态流
  - cancel / requeue / wake 协议
  - 负例矩阵与 rollout 条件
- 若本轮治理改动收口完毕，按 `governance` 模式安全同步到 `main`。

### 会话 35 - 2026-03-19（补齐 wake/cancel/requeue，并完成 Git 层 queue 实盘演习）
**用户目标**：
> 不再停在草案层，直接把 queue 系统一步到位补成可用闭环，同时持续自我审视，不把外部夸奖当作完工证明。

**已完成事项**：
1. 在 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 中新增：
   - `cancel-branch-request`
   - `requeue-branch-request`
   - `wake-next`
   - `return-main` 的 `NEXT_IN_LINE_*` 输出
2. 新增 queue runtime 自愈逻辑：
   - `Repair-SharedRootQueueRecord`
   - 用于修补旧任务分支脚本留下的 stale `task-active / granted`
3. 更新正文与协议：
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-queue.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\dispatch-protocol.md`
   - `queue-rollout-matrix.md`
4. 完成 Git 层实盘演习：
   - `navigation` 首次拿到 grant
   - `farm / npc` 进入 waiting
   - `farm` 被重排到队尾
   - 治理线程 `wake-next` 唤醒 `npc`，随后取消 `npc`
   - 再次 `wake-next` 唤醒 `farm`，随后取消 `farm`
   - 额外验证 `wake-next` 在已有未消费 grant 时会返回 `WAKE_BLOCKED`
5. 演习结束后已把 runtime queue 回写为空基线，当前 live 为：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `git status --short --branch = ## main...origin/main`

**关键决策**：
- 当前可以确认 Git 层 queue 闭环已经成立。
- 但我也明确保留一个残余风险判断：
  - 旧任务分支里的脚本版本仍可能落后
  - 所以“所有分支天然携带最新闸机”仍不能被当成已完成事实
- 自愈逻辑已经显著降低这个风险，但没有把它神话成完全消失。

**恢复点**：
- 后续若继续阶段 `23`，重点应从“基础 queue”切到：
  - 脚本漂移的更彻底解决方案
  - Unity / MCP 高风险联动调度

### 会话 36 - 2026-03-19（落地 stable launcher，切断 shared root live 调度对当前 checkout 脚本版本的依赖）
**用户目标**：
> 在 Git 层 queue 已经真实成立后，继续推进阶段 23，但不要被“已经像 OS 调度器”这类夸奖带偏；真正补掉旧任务分支脚本漂移这个物理风险，并保持持续自审。

**已完成事项**：
1. 重新执行 Sunset 前置核查：
   - 显式按 `skills-governor + sunset-workspace-router + sunset-startup-guard(manual-equivalent)` 路由
   - 复核 `AGENTS.md`
   - 复核阶段 `23` 的 `tasks.md / analysis.md / dispatch-protocol.md`
   - 复核 live 基线：
     - `D:\Unity\Unity_learning\Sunset @ main @ 57b68e66`
     - runtime queue 空基线
2. 新增仓库外稳定 launcher：
   - `C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1`
   - 默认从 `main:scripts/git-safe-sync.ps1` 读取 canonical 脚本，再落临时文件执行
3. 把现行入口文档改口：
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-queue.md`
   - `dispatch-protocol.md`
   - `queue-rollout-matrix.md`
   - `tasks.md`
   - `analysis.md`
4. 完成一轮最小烟雾验证：
   - 从仓库外 `D:\迅雷下载\开始` 执行：
     - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action preflight -Mode governance -OwnerThread Codex规则落地`
   - launcher 成功输出：
     - `LAUNCHER_MODE`
     - `LAUNCHER_REPO_ROOT`
     - `LAUNCHER_SOURCE_REF`
     - `LAUNCHER_LIVE_BRANCH`
   - canonical 主脚本成功执行，shared root queue runtime 未被污染
5. 过程中暴露并修复了 launcher 自身的三个真实 bug：
   - PowerShell 参数拼接错误
   - 路径分隔符归一化误判
   - Windows PowerShell 5.1 执行临时 UTF-8 无 BOM 含中文脚本时的解析异常

**关键决策**：
- 现在更客观的结论是：
  - Git 层 queue / wait / wake / cancel / requeue 闭环成立
  - shared root live 调度入口已经不再直接受当前 checkout 的仓库内脚本版本摆布
  - 但这仍不等于 Unity / MCP 层也拥有同等级调度器
- 这轮新增的全局 gotcha 值得跨项目复用：
  - Windows PowerShell 5.1 下做临时脚本桥接时，入口脚本应尽量 ASCII-only，临时目标脚本应写 BOM

**恢复点**：
- 阶段 `23` 下一步不再重复造 queue，而是优先补：
  - 多 checkpoint 持续推进模型
  - Unity / MCP 单实例调度边界
  - 治理线程的批次发放 / 回收协议
- 之后若继续唤醒业务线程进入 shared root live 调度，默认应改用：
  - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 ...`

### 会话 37 - 2026-03-19（恢复开发前先清场：为 `Skills和MCP` 落根层批次分发文件）
**用户目标**：
> 不要停在“建议下一步”，而是直接开始恢复开发的实际运营动作。

**已完成事项**：
1. 按 `skills-governor + sunset-workspace-router + sunset-thread-wakeup-coordinator + sunset-startup-guard(manual-equivalent)` 重新核 shared root live 现场。
2. 发现当前 live 事实并不是可直接发业务准入，而是：
   - `D:\Unity\Unity_learning\Sunset @ main @ bfa8ae47`
   - `occupancy = neutral`
   - `queue runtime = empty`
   - 但 Git working tree 仍有 3 个 dirty，归属 `Skills和MCP / Steering规则区优化`
3. 没有误发业务线程的 `request-branch / ensure-branch` prompt，而是先按治理线程根层协议落了这一轮批次运营文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_01_稳定launcher复工前清场.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_稳定launcher复工前清场\可分发Prompt\Skills和MCP.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_稳定launcher复工前清场\线程回收\Skills和MCP.md`

**关键决策**：
- 当前最正确的“直接开始”不是假装已经能发业务线程 live 准入，而是先把 shared root 上当前真实 blocker 转成正式清场批次。
- 当前裁定：
  - 先 `Skills和MCP` 清场
  - 后业务线程 live 准入

**恢复点**：
- 现在可以直接分发这轮根层批次文件给 `Skills和MCP`。
- 等它回写固定回收卡并确认 `main + clean` 后，再继续生成下一轮业务线程 live 准入批次。

### 会话 38 - 2026-03-19（shared root 清场完成后，生成 queue-aware 业务准入批次 01）
**用户目标**：
> 继续主线，不要停在“清场轮”汇报；在 shared root 真 clean 后，直接进入下一轮业务线程 live 准入运营。

**已完成事项**：
1. 读取 `Skills和MCP` 清场回收卡，确认：
   - 它已完成稳定 launcher 的治理同步
   - shared root 已恢复为 `main + clean`
2. 再次复核 live：
   - `D:\Unity\Unity_learning\Sunset @ main @ 00e3b734`
   - `occupancy = neutral`
   - `queue runtime = empty`
3. 回读阶段 22 既有回收卡，对 5 条业务线程做现阶段分层：
   - `NPC / 农田 / 导航 / 遮挡`：适合进入本轮 queue-aware Git 槽位准入
   - `spring-day1`：延后到下一轮 `Unity/MCP-aware` 准入
4. 新建根层批次文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_02_queue-aware业务准入_01.md`
5. 新建 4 条线程的专属 prompt 与固定回收卡：
   - `NPC`
   - `农田交互修复V2`
   - `导航检查`
   - `遮挡检查`

**关键决策**：
- 这轮不再沿用旧的 `grant-branch` 直推口径，而是统一切到：
  - `request-branch`
  - `GRANTED / ALREADY_GRANTED / LOCKED_PLEASE_YIELD`
  - `ensure-branch`
  - `最小 checkpoint`
  - `return-main`
- 这轮也没有把所有线程一股脑推进去：
  - `spring-day1` 单独延后，不是因为不健康，而是因为它的下一最小 checkpoint 天然带 Unity / MCP / Play Mode

**恢复点**：
- 用户现在可以直接群发这轮根层批次文件。
- 后续等 4 条线程回写后，再做下一轮调度，包括是否给 `spring-day1` 发单独的 `Unity/MCP-aware` 准入批次。
