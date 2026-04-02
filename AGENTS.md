# Sunset 项目 Codex 工作规则

## 0A. 2026-03-21 极简并发开发临时口径（高于下文旧流程，直到用户撤销）
- 当前最高目标不是流程完美，而是尽快恢复真实开发、缩短等待、减少治理摩擦。
- 当前唯一 live 规范快照文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前规范快照_2026-03-22.md`
- 当前 live 执行优先级固定为：
  1. 用户当前裁定
  2. `D:\Unity\Unity_learning\Sunset\AGENTS.md`
  3. 当前规范快照
  4. `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\` 下当前命中的治理规范正文
  5. `.kiro/steering/` 中的通用结构、领域细则与历史通用基线
- 自 2026-03-23 起，以下补强项视为当前 live 口径的一部分：
  1. 规则变更必须同轮同步到 `AGENTS.md`、当前规范快照、治理规范正文（如 `治理线程批次分发与回执规范.md` / `典狱长模式_治理总闸与分发规范.md`）以及相关 skill；少一处都不算真正生效。
  2. 只要线程碰过 `Scene`、`Prefab`、`Primary.unity` 或其他热 Unity 资源，回执里必须显式说明“我留了什么、保留什么、清掉什么、当前是否已对我这条线 clean”。
  3. 只要白名单路径包含 `.unity`、`.prefab`、`.asset`，收口前必须把本轮改动分类为“有效内容 / 自动副产物 / 调试残留 / 他线脏改”；自动副产物默认不得混入 checkpoint。
  4. branch / worktree 只允许作为例外载体存在；一旦例外成果已可迁回 `main`，线程必须先迁最小 checkpoint，再移除旧 carrier blocker，再回到 `main` 语义继续。
  5. 任何 Unity / MCP live 写或 live 验证，都必须先说清 4 件事：需要什么实例、最多占多久、只做什么、做完退回什么状态。
  6. 线程回执最小字段固定为：`当前在改什么 / changed_paths / 是否触碰高危目标 / 是否需要 Unity-MCP live 写 / code_self_check / pre_sync_validation / 当前是否可直接提交到 main / 提交 SHA / 当前 git status 是否 clean / 当前 own 路径是否 clean / blocker_or_checkpoint / 一句话摘要`。
  7. 默认执行纪律是“一刀一收”；除非同一逻辑尚未闭环、同一 live 窗口内必须连跑、或用户明确要求更大 checkpoint，否则不要顺手叠第二刀。
  8. 普通线程的 own dirty / untracked 默认必须在当前这一刀内自己收口；不能再把自己的尾账拖到下一轮，再等治理线程专门开 cleanup 批次兜底。
  9. `git-safe-sync.ps1` 在 `main-only` 的 `task` 模式下，虽然允许 unrelated dirty 留在 shared root，但如果本轮白名单所属 `own roots` 下仍有未纳入本轮的 remaining dirty / untracked，就必须直接阻断。
  10. `Primary.unity`、`Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`、`ProjectSettings/TagManager.asset`、`Assets/Editor/StaticObjectOrderAutoCalibrator.cs` 当前统一按“强制报实的 hot / mixed 目标”处理；owner 未明确前，任何线程都不得借 cleanup 名义静默吞并。
  11. 两类 hot / mixed 目标从现在起按不同实战口径执行：
      - `Primary.unity` = 单 scene writer 接盘面；`dirty + unlocked + ownerless` 表示“无人接盘的 stale mixed scene”，不等于永久不可写。当前批准的接盘顺序为：`NPC -> spring-day1`，直到用户改写。
      - `GameInputManager.cs` = 可并发但必须记触点的共享热点；允许多个线程并行改，但必须显式报出自己触碰的入口/方法/行为链；若撞到同一触点或同一语义顺序，才升级治理 / integrator。
  12. 如果 `changed_paths` 包含 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`，回执除固定最小字段外还必须额外补：
      - `touched_touchpoints`
      - 用来明确本轮到底改了哪些方法、入口链或行为判定点
  12A. 从 2026-03-31 起，普通线程只要从“只读分析”进入“开始做一刀真实施工”，就必须先登记一次 `thread-state`：
      - 入口脚本：`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
      - 这一步不是可选提示，而是当前 live 并发规则的一部分
      - 不需要在纯只读分析阶段就登记；一旦准备真正改 tracked 内容、接盘 hot target、或开始形成本轮白名单切片，就必须登记
  12B. `Begin-Slice` 至少要报实 4 件事：
      - `ThreadName`
      - `CurrentSlice`
      - `TargetPaths`
      - 如果命中共享热点文件，再额外报 `SharedTouchpoints`
  12C. 从 2026-03-31 起，普通线程在准备执行白名单 `sync` 前，必须先跑：
      - `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
      只有它通过后，才算进入真正可收口状态；如果它报 blocker，就先停在 blocker，不得跳过
  12D. 线程如果中途暂停、卡住、让出现场、或本轮不再继续收口，必须跑：
      - `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
      不再允许“人已经停了，但 live 状态层还显示 ACTIVE”
  12E. 治理位、典狱长位和 integrator 需要看当前谁在施工、谁卡住、谁 ready 时，默认先看：
      - `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Show-Active-Ownership.ps1`
      不再只靠 memory、回执和人脑追问还原现场
  12F. 过渡规则：
      - 已经在施工中的旧线程不要求废弃重开
      - 但从现在起，最晚必须在“下一次真实继续施工前”补一次 `Begin-Slice`
      - 最晚必须在“第一次准备 sync 前”补一次 `Ready-To-Sync`
      - 如果当前这轮决定先停，则直接补 `Park-Slice`
  12G. 从 2026-04-01 起，凡是 Sunset 治理位、典狱长位、唤醒位、或续工 prompt 生成位，且这条 prompt 会让线程从只读进入真实施工、或继续一个已经在跑的施工切片：
      - 默认必须在业务正文末尾追加统一 `thread-state` 接线尾巴
      - 统一尾巴文件：
        `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\线程续工统一尾巴_thread-state补登记_2026-04-01.md`
      - 这不是“有空就加”的提醒，而是 Sunset 当前 live 治理 prompt 的默认硬要求
      - 只有当该线程本轮始终停留在纯只读分析，才允许不追加这段尾巴
  12H. 上面的 `thread-state` 规则不只约束治理位发出的 prompt，也同样约束 Sunset 业务线程在“用户直接和我对话”的高频场景：
      - 只要用户这轮要求我继续真实施工、开始修、直接改、继续推进当前切片，线程自己就必须主动遵守 `12A ~ 12F`
      - 不允许把“这次不是治理位发 prompt”当成不登记的理由
      - 不允许等用户或治理位额外提醒我补 `Begin-Slice`
      - 当前正确口径是：`thread-state` 由线程自己自发执行，不以治理 prompt 作为触发前提
  12I. 因此，Sunset 业务线程在直聊场景的最低动作固定为：
      - 如果这轮仍停留在只读分析：第一条 `commentary` 直接说明“本轮只读，暂不跑 `Begin-Slice`”
      - 如果这轮准备进入真实施工：第一条 `commentary` 直接说明“现在先跑 `Begin-Slice` 再继续施工”
      - 第一次准备 `sync` 前：直接自己跑 `Ready-To-Sync`
      - 如果这轮先停、卡住、让位或本轮不再继续：直接自己跑 `Park-Slice`
  12J. Sunset 业务线程本轮收尾或回执时，除原有字段外，还必须额外报实：
      - 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
      - 如果没跑，原因是什么
      - 当前 live 状态是 `ACTIVE / READY / PARKED` 还是被 blocker 卡住
  13. 从现在起，必须明确区分“给治理看的最小回执”和“给用户看的用户可读汇报”；两者不再视为同一种文本。
  14. 只要汇报对象是用户，或用户问的是“现在做到了什么 / 还剩什么 / 下一步做什么 / 到哪个阶段了”，回复必须先交 `用户可读汇报层`，并把下面 6 项作为 `保底六点卡` 逐项显式输出，顺序不得打乱、不得合并、不得省略：`当前主线 / 这轮实际做成了什么（按功能点） / 现在还没做成什么 / 当前阶段 / 下一步只做什么 / 需要用户现在做什么`。
  15. `保底六点卡` 的空项也不允许跳过；如果这轮某项没有内容，必须直接写明 `无 / 尚未 / 不需要 / 仍待验证` 之类的明确答案，不能留空、不能写“同上”、不能让用户自己从后文猜。
  16. 用户可读回复的固定顺序改为：`A1 保底六点卡 -> A2 用户补充层（可选，只有线程认为对用户理解有帮助时才补） -> B 技术审计层`；也就是说，首先是给用户看懂，其次才是给治理看清。
  17. `changed_paths`、参数名、checkpoint、日志、dirty/lock/owner 报实等技术信息，只能放在最后的 `技术审计层`；在 `保底六点卡` 没写齐之前，不得提前插入，也不得拿它们顶替前面的用户层。
  18. 如果回复里使用了 `best-known stable checkpoint`、`detour`、`release / recover`、`blockOnsetEdgeDistance`、`HardStop`、`stuck cancel`、`blocked-hotfile` 这类工程术语，必须在同一段里立刻翻译成玩家可感知结果，而且这份翻译应优先出现在 `保底六点卡` 或 `用户补充层`，不能只藏在技术审计层。
  19. 对用户宣称“已完成 / 已修好 / 已过线”时，必须显式标明验证状态：`用户已测 / 线程自测已过 / 静态推断成立 / 尚未验证`；不能把这几类状态混写成一句模糊结论。
  20. 以后线程若出现以下任一情况，即使工程事实没错，也视为汇报不合格：缺任一项 `保底六点卡`、把 2 项以上揉成同一段、先贴技术 dump 再补人话、只给“做了很多”却不说“还没做成什么 / 下一步只做什么”、或完全不写“需要用户现在做什么”；治理线程和当前执行线程都应主动拒绝这种回复。
  21. 治理线程和当前执行线程审用户向回复时，必须先按 `保底六点卡` 判是否说清楚，再看技术审计层；技术信息再充分，也不能反向弥补前面 6 项缺失。
- 即日起 Sunset 普通开发的真实基线默认只有一个：`main`。
- 对普通开发，暂停把 `request-branch`、`grant-branch`、`ensure-branch`、`wake-next`、`return-main` 当成必经前置。
- 不再为普通开发继续新增 `codex/*` 分支或新增 worktree。
- 治理线程的角色改成“收烂摊子 + 高危打断 + 极简口径维护”，不再做重排队、重放行、重闸门。
- 治理线程默认不再为“常规 own dirty 尾账”重复开 cleanup 批次；只有命中 cross-thread mixed / hot-file incident、owner 不清或 shared root 物理现场被卡死时，才升级治理收口。
- 所有线程都可以直接开始开发；只有命中下面这些硬打断条件时才必须停下：
  1. 正在改同一个高危目标；但 `GameInputManager.cs` 例外按“触点并发”处理，只有撞到同一触点 / 同一行为链时才硬打断。
  2. 需要 Unity / MCP live 写入，但已经有别的线程在写。
  3. 准备做破坏性 Git 动作，例如 `reset --hard`、`checkout --`、`clean`、强推、危险 rebase。
  4. 已经造成编译坏、场景坏、引用坏，需要先收烂摊子。
- “高危目标”默认包括：
  - 同一个 Scene
  - 同一个 Prefab
  - `Primary.unity`
  - `Assets/000_Scenes/SceneBuild_01.unity`
  - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
  - `ProjectSettings/TagManager.asset`
  - `Assets/Editor/StaticObjectOrderAutoCalibrator.cs`
  - 任何当前正被另一个线程 live 写入的 Unity 资源
- 其中当前补充口径固定为：
  - `Primary.unity` 继续按单 writer 处理；没有 active lock 但文件仍 dirty 时，默认理解为“待接盘 scene”，不是“假活跃占用”
  - `GameInputManager.cs` 虽仍属强制报实热点，但不再默认按单 writer 停工；优先按触点拆分，并接受最终 integrator 收盘
- 等待态不再强制先写 tracked memory、占用文档或治理回执；能用最小聊天回执解决的，就不要先制造治理脏改。
- 已经存在的 branch / worktree 只视为过渡现场，不再扩张成长期模型；其有价值成果应尽快回到 `main`，然后继续只以 `main` 为准。
- 如果本节、当前规范快照与下文旧的 branch-only / grant-only / queue-only 规则冲突，以本节和当前规范快照为准。

## 0. 例外场景下的分支 / 租约闸门
- 本节只在你明确进入 `branch carrier / worktree / request-branch / ensure-branch / return-main` 例外流程时生效。
- 对普通 `main-only` 线程，本节默认不适用；普通线程直接遵守 `0A` 与当前规范快照，完成 startup preflight 后就在 `main` 语义下开发，并按白名单 `sync` 收口。
- 【强制最高优先级】只要任务发生在 `D:\Unity\Unity_learning\Sunset` 且属于实质性任务，你的第一次动作 MUST 是调用 `sunset-startup-guard`；如果当前会话未显式暴露该 skill，你 MUST 立即调用 `skills-governor` 并按 Sunset 等价流程手工完成同级前置核查。
- 在第一次闸门返回“允许继续”之前，你 NEVER 可以：
  - 执行 `git-safe-sync.ps1 -Action ensure-branch`
  - 切离 `main`
  - 写任何实现代码、改场景、改 Prefab、改 Inspector、改资源
  - 进入 Unity Play Mode
  - 假设 shared root 仍然是 neutral
- 第一次唤醒阶段 MUST 是纯只读阶段。没有显式 Lease / Grant 的线程，NEVER 允许执行 `ensure-branch`。
- shared root 上的 `ensure-branch` 不是局部动作，而是全局写态。没有租约就切分支，一律视为严重违规。
- 如果你试图绕过这条顺序，必须先阻断、先汇报，再等待显式准入；NEVER 允许“先切过去再说”。
- 没拿到 `GRANTED / ALREADY_GRANTED` 的 waiting 线程，NEVER 允许在 `main` 上写 tracked `memory`、回执卡或治理文档；恢复点优先放进 `CheckpointHint / QueueNote` 或最小聊天回执。
- waiting / 挂起阶段如需写代码草稿或复盘草稿，唯一允许的落点是 gitignored Draft 沙盒：`D:\Unity\Unity_learning\Sunset\.codex\drafts\<OwnerThread>\`；NEVER 用 tracked 文档顶替它。
- 一旦进入 shared root 的任务分支，你 MUST 把这次占用视为“最小写事务窗口”：先做 checkpoint 或最小代码写入，长时间只读分析、治理回执和 memory 补记一律放到 `return-main` 之后。
- `return-main` 之后也不等于可以无脑立刻写脏 `main`；如果队列仍有 waiting 线程，事后复盘优先继续放进 Draft 沙盒或最小聊天回执，tracked 证据应延后到治理窗口或最小白名单同步时再落。
- 对任何 Sunset 实质性任务，首次 `commentary` MUST 显式点名本轮正在使用的 skill；如果 `sunset-startup-guard` 当前会话未显式暴露而改走 `skills-governor + 手工等价闸门`，也必须明说。
- 对 shared root 的 live Git 准入命令：
  - `request-branch`
  - `grant-branch`
  - `ensure-branch`
  - `wake-next`
  - `return-main`
  默认 MUST 通过稳定 launcher：
  - `C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1`
  而不是直接调用仓库内 `scripts\git-safe-sync.ps1`。
- 唯一例外是治理线程正在修改 `scripts\git-safe-sync.ps1` 本体并需要验证 working tree 版本；此时才允许：
  - 直接运行仓库内脚本
  - 或运行稳定 launcher 但显式带 `-SourceRef HEAD`

## 1. 文件定位
- 本文件是 Sunset 项目的 live 路由与执行约束层，不重复抄写 `.kiro/steering` 的通用正文。
- 当前 Sunset 的 live 规则层按以下顺序理解：
  1. 用户当前裁定
  2. 本文件
  3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前规范快照_2026-03-22.md`
  4. `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\` 下当前命中的治理规范正文
  5. `.kiro/steering/` 中的通用结构、领域细则与历史通用基线
- `.kiro/steering/` 仍然重要，但它不再单独定义当前 shared-root / main-only / dispatch / startup-guard 的 live 默认。
- `C:\Users\aTo\.codex\AGENTS.md` 是 Codex 的全局规则文件位置，不是 Sunset 线程记忆文件的存储位置。
- Sunset 项目全部 Codex 线程的记忆总根路径固定为 `D:\Unity\Unity_learning\Sunset\.codex\threads\`。
- `CLAUDE.md`、Claude 迁移文档、History 交接稿可以吸收经验，但当前真正的唯一有效依据仍是 `.kiro/steering` 与活跃工作区文档。
- 除 `Codex`、`Claude`、`Sunset`、`AGENTS.md`、`memory_0.md`、路径、命令、代码标识、Unity 专有类型名等特殊情况外，所有说明文字一律使用中文。

## 2. 默认工作方式
- 默认使用中文输出、中文文档、中文总结。
- 只要任务发生在 `D:\Unity\Unity_learning\Sunset` 且属于实质性任务，必须先使用 `sunset-startup-guard` 做项目级启动闸门核查，再进入其他 skills、工作区文档和具体修改。
- 先判断当前任务归属哪个 `.kiro/specs` 工作区；用户给了明确路径时直接使用，不另造工作区。
- 线程记忆文件一律写入 `D:\Unity\Unity_learning\Sunset\.codex\threads\<分组>\<线程名>\memory_0.md` 这一项目内结构，不写到 `C:\Users\aTo\.codex\`。
- 如果当前任务属于长期线程治理或线程进入前核验，先读 `D:\Unity\Unity_learning\Sunset\.codex\threads\线程分支对照表.md` 与当前治理工作区中的用户使用说明。
- `sunset-startup-guard` 的职责优先级高于普通工作区路由：它先核主线、`cwd/branch/HEAD`、共享根目录占用与模式判定，再决定是否继续调用 `sunset-workspace-router`、`sunset-lock-steward` 等同伴 skills。
- 如果任务涉及 Unity、MCP、Play Mode、Compile、Domain Reload、Console、Scene / Prefab 验证或 Inspector 读写，除 shared root 占用外，还必须额外核：
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-occupancy.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-live-baseline.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-hot-zones.md`
  - 必要时回看 `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-log.md`
- 如果任务会使用 MCP / Play Mode 做 live 取证，进入前必须先说明：需要什么证据、最多跑几轮、拿到什么信号就 Pause / Stop、最后退回什么状态；禁止先长时间跑着再回头想怎么收。
- 如果任务会进入 Play Mode 做取证、调试或验收，完成当前步骤后必须主动退回 Edit Mode，再允许继续后续操作、汇报或把现场交给其他线程；禁止把运行中的 Editor 留给别人收尾。
- 如果任务涉及 DialogueUI、NPC 气泡、字体、UI 样式、布局、材质或其他直接影响观感的表现层资源，除相关业务文档外，还必须补读 `D:\Unity\Unity_learning\Sunset\.kiro\steering\ui.md`，并把“好看、合理、专业、可读”当成硬验收项，而不是只看功能是否可用。
- 如果任务涉及工作区、记忆、交接、治理、报告、Claude 或 Codex 迁移、历史接手，优先使用 `sunset-workspace-router`。
- 如果任务涉及 Sunset 线程续工 prompt、回执后下一轮 prompt、验收失败后的 prompt 回拉、或需要把多轮返工收成“单轮可验硬切片”，优先使用 `sunset-prompt-slice-guard`。
- 如果用户明确说出 `典狱长`、`典狱长上货`、`上货`，或当前任务本质上是在做治理总闸的“审回执 / 判停发 / 按需分发”，优先使用 `sunset-warden-mode`；如果当前会话未显式暴露该 skill，必须改读 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\典狱长模式_治理总闸与分发规范.md` 并执行手工等价流程。
- 如果用户明确说出 `看守长`、`看守长上场`、`喊看守长`，或当前任务本质上是在要求“这条线程把自己这一刀交成用户可直接验的验收包 + 回执单”，优先组合：
  - `acceptance-warden-mode`
  - `sunset-acceptance-handoff`
  如果当前会话未显式暴露其中任一 skill，也必须按两者的手工等价流程执行。
- 在 Sunset 里，`看守长` 和 `典狱长` 严格分流：
  - `看守长` = 业务线程自己的验收交接
  - `典狱长` = 治理线程的审回执 / 判停发 / 再分发
  - 不允许把用户说的 `看守长` 偷换成 `sunset-warden-mode`
- 如果当前是在 Sunset 业务线程自己的直聊场景里触发 `看守长`，且用户没有明确要求“审别的线程 / 审别人的回执 / 审别人的 prompt”，默认就审当前线程自己最近完成或准备交接的这一刀：
  - 主动回看当前工作区、线程 memory、当前 slice、最新验证与最新回执
  - 直接交验收包
  - 不允许先反问用户“把线程回执 / prompt 文件 / 线程名发来”
- 这里的默认验收对象，优先固定为：
  - 上一轮刚完成
  - 且已经向用户汇报过
  的那一刀
- 如果用户是在一轮完成汇报之后紧接着喊 `看守长`，线程必须直接把刚才那轮完成项收成验收包，不要把验收对象漂成“这条线整体”或重新问用户“要验哪一轮”
- 如果用户这次只是单独喊了 `看守长`，没有把上一轮完成汇报贴在眼前，但上下文仍明显是当前 Sunset 业务线程自己的验收场景，线程也必须自己回看当前工作区、线程 memory、当前 slice 与最近完成汇报，选最近一次“已完成且已向用户交代过结果”的切片来交接；最多只允许用一句话报明“这次我按 XXX 这一刀给你收验收包”，仍不允许先向用户索要线程名、回执或 prompt 文件
- `看守长` 回复的首屏不应先讲治理、skill 或模式切换：
  - 不要先说 `我先切到看守长模式`
  - 不要先说 `我先按当前治理口径处理`
  - 不要先说 `我先读某某 skill`
  - 不要只回 `在 / 看守长到了 / 看守长到位` 这种空占位再等用户补材料
  - 应直接进入上一轮完成项的验收交接正文
- 如果当前任务是“给我可直接发给线程的 prompt / 续工话术”，详细正文默认必须写进文件；聊天里固定改用复制友好格式：
  1. `对应文件在：`
  2. 平铺列出最新 prompt 文件路径
  3. `你可以直接这样发给 XXX：`
  4. 一个 `text` 代码块，里面直接写出可复制话术，并包含 `请先完整读取 [最新 prompt 路径]`
- 不要再退回“文件路径 + 极短转发语”的旧格式，也不要让用户自己从长聊天里拼出到底该发哪段。
- 如果任务涉及 Sunset 代码修改、编译可用性、checkpoint / sync / handoff 前的红错清理、warning 与 error 区分、或“不能把 Unity 留到不能用”的执行纪律，优先使用 `sunset-no-red-handoff`。
- 如果任务涉及 `ScreenSpaceOverlay` UI、GameView 最终合成屏、Prompt / Workbench 的最终观感验收，或 Main Camera 截图天然无法证明 UI 是否正确，优先使用 `sunset-ui-evidence-capture`；如果当前会话未显式暴露该 skill，也必须手工沿用 `.codex/artifacts/ui-captures/spring-ui` 与 `scripts/SpringUiEvidence.ps1` 的同等流程。
- 如果任务进入“线程已完成自己能做的验证，接下来需要用户终验”的阶段，优先使用 `sunset-acceptance-handoff`，要求线程先交专业验收指南、预期结果、失败判读和回执单；禁止只说“等用户复测”。
- 如果任务属于 `Codex规则落地` 治理线程，且涉及多线程 prompt 分发、统一领取入口、本轮批次文件、固定回收卡收件或最小回复格式约束，优先使用 `sunset-governance-dispatch-protocol`；典狱长模式下还必须先做“是否该停给用户”的总闸裁定；如果当前会话未显式暴露相关 skill，必须同时改读：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\治理线程批次分发与回执规范.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\典狱长模式_治理总闸与分发规范.md`
  再执行手工等价流程。
- 如果任务涉及场景、预制体、检查器、ScriptableObject、序列化引用或验证场景，优先使用 `sunset-scene-audit`。
- 如果任务涉及锐评、审视报告、差异、纠正、是否采纳评审，优先使用 `sunset-review-router`。
- 如果任务涉及 `.kiro/locks/`、A 类热文件、`Primary.unity`、`GameInputManager.cs`、`Acquire-Lock.ps1`、`Release-Lock.ps1`、或 `Committed/Parked/Abandoned` 交接判断，优先使用 `sunset-lock-steward`。
- 如果任务涉及文档乱码、`SKILL.md` / `agents/openai.yaml` / `memory.md` / `tasks.md` 编码健康、ANSI / GBK / UTF-8 漂移判断，优先使用 `sunset-doc-encoding-auditor`。
- 如果任务涉及阶段收口、版本快照、冻结归档、解冻后基线摘要、或需要把当前锁池 / Console / 活文档入口 / 线程状态打包成一份快照，优先使用 `sunset-release-snapshot`。
- 如果任务属于当前治理续办、L5 后入口重构、skills/AGENTS 重构、四件套规范或线程解冻收口，优先从：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前规范快照_2026-03-22.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地`
  进入，而不是回到 `Codex迁移与规划`、旧全局 `tasks.md` 或把 `000_代办/codex` 误当成正式工作区。

## 3. Sunset 任务连续性与主线恢复
- 先判断当前这句用户输入是在继续现有主线、处理中途阻塞，还是明确切换到新主线。
- 只要用户没有明确换线，就默认仍服务于当前活跃工作区的主线目标。
- 明确换线的常见表达包括：`换个问题`、`先不做这个`、`另起一个任务`、`重新开始`、`这个先放下`。
- 类似 `顺便问一句`、`先修一下这个`、`你先看看为什么不行`、`先查下报错`，默认仍服务当前工作区主线，属于阻塞处理或插入式子任务。
- 修工具、修 MCP、修脚本、修规则、查报错、补验证，在 Sunset 项目里默认都属于“阻塞处理”，不是新的业务主线。
- 处理阻塞时，要始终明确“我现在在修什么、它服务于原来的哪个目标、修完回到哪一步”。
- 阻塞处理完成后，优先回到原工作区、原主线、原完成标准，而不是只围绕最后一句输入漂移。
- 如果用户一句话只说“修一下”“看看这个为什么不行”，默认理解为服务当前主线，除非用户明确改题。
- 在每次准备收尾回复时，快速自检四问：
  1. 当前活跃工作区的主线目标是什么？
  2. 本轮是在推进主线，还是只是在清理阻塞？
  3. 我的回复里有没有明确写出主线当前进度，而不是只写阻塞处理结果？
  4. 如果阻塞已解除，我有没有把下一步主线动作说清楚？

## 4. 进入任务后的读取顺序
- 第一步：先运行 `sunset-startup-guard`，核定主线、`cwd/branch/HEAD`、共享根目录占用、模式判定和首个同伴 skill。
- 第二步：定位当前活跃工作区。
- 第三步：如果是当前治理续办或活文档任务，先读 `Sunset当前唯一状态说明_2026-03-17.md` 与对应现行活文档，再读工作区 `memory.md`。
- 第四步：其余任务先读该工作区的 `memory.md`，必要时继续读直接相关的子工作区或父工作区 `memory.md`。
- 第五步：读 `.kiro/steering/README.md` 和 `.kiro/steering/rules.md`，确认当前任务应走的规则入口。
- 第六步：按主题补读对应 steering 文件，再读工作区内的 `requirements.md`、设计稿、任务拆分、评审、报告、指引等文档。
- 第七步：最后再改代码、改场景、改配置、改文档。

## 5. Steering 路由表
- 全局项目规则、中文口径、输出方式：`rules.md`
- 记忆、追加式记录、分卷、继承、同步顺序：`workspace-memory.md`
- 评审、锐评、审视报告、路径 A/B/C：`code-reaper-review.md`
- 场景、预制体、检查器、ScriptableObject、序列化引用：`scene-modification-rule.md`
- 编码风格、region、EventBus、分层约束：`coding-standards.md`
- 存档、GUID、持久化、重建：`save-system.md`
- Git、分支、提交、回退、checkpoint、preflight、`.gitattributes`：`git-safety-baseline.md`
- 文档、规则落地、文档同步：`documentation.md`
- 上下文恢复、长链任务接手：`000-context-recovery.md`
- steering 自身维护、避免双源复制：`maintenance-guidelines.md`
- 领域细则按关键词读取：`items.md`、`so-design.md`、`layers.md`、`systems.md`、`trees.md`、`ui.md`、`animation.md`、`placeable-items.md`、`chest-interaction.md`、`debug-logging-standards.md`、`scene-hierarchy-sync.md`。

## 6. Sunset 的记忆纪律
- 本项目同时存在两层记忆：
  - Codex 线程记忆：`D:\Unity\Unity_learning\Sunset\.codex\threads\<分组>\<线程名>\memory_0.md`
  - 工作区记忆：`.kiro/specs/.../memory.md`
- 任何产生可复用结论的 Sunset 实质性工作，都不能只写在线程记忆里；对应工作区记忆也要落盘。
- 任何 Sunset 实质性工作，除了项目 / 工作区记忆与线程记忆，还必须补记 `C:\Users\aTo\.codex\memories\skill-trigger-log.md`，记录本轮技能触发、原因、可见性与结果。
- 更新顺序固定为：当前子工作区 → 受影响的父工作区 → 当前线程记忆。
- 如果本轮同时命中了全局治理层写回，则更新顺序收紧为：当前子工作区 → 受影响的父工作区 → 必要的全局层（如 `global-learnings.md` / `skill-trigger-log.md`）→ 当前线程记忆。
- 只读分析如果产生了稳定的治理结论、路由结论、风险判断，也要更新记忆。
- 线程记忆和工作区记忆的每次追加，都应写清：当前主线目标、本轮阻塞或子任务、恢复点或下一步主线动作。
- 分卷命名固定为：
  - 工作区活跃卷：`memory.md`
  - 工作区历史卷：`memory_0.md`、`memory_1.md`...
  - 线程活跃卷：`memory_0.md`
  - 线程历史卷：`memory_1.md`、`memory_2.md`...
- 如果本轮只完成了阻塞处理而主线尚未完成，收尾时必须同时汇报“阻塞已解除”和“主线下一步”，不能只汇报修复动作本身。
- 任何直接对用户的阶段汇报，都不能只报 `changed_paths / checkpoint / blocker`；至少要先把“做到什么 / 没做到什么 / 当前阶段 / 下一步”说成人话，再补技术层。
- `900_开篇/spring-day1-implementation` 是落地参考样板：其 `memory.md` 体现追加式记录，其 `requirements.md` 与 `OUT_tasks.md` 体现需求驱动和任务拆解。新工作区可以借鉴这种信息密度，但不必机械复制文件数量。
- `Codex规则落地` 是 Sunset 当前正式治理工作区样板：其根层 `memory.md` + 分阶段 `tasks.md` 体现“根层记忆、阶段承接、按需 design”的新治理结构。
- `000_代办/codex` 只保留 TD 镜像与读取入口，不再承载工作区正文。
- 记忆文件中的说明文字默认使用中文，不要把普通叙述写成英文；只有文件名、路径、命令、专有名词或特殊技术名词可以保留原文。

## 7. 风险任务处理
- 本项目的 Git 收尾顺序固定为：先按工作区规则更新工作区记忆；如果本轮命中了全局治理层，再补写必要的全局层（如 `global-learnings.md`、`skill-trigger-log.md`）；再更新线程记忆；最后执行 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`。
- 任何 Sunset 实质性工作在完成记忆更新后，只要当前改动已经达到可提交状态，Codex 就必须继续执行 Git 安全同步，而不是停在“本地已改完但未提交/未推送”。
- 长期治理 / 总览 / 审计线程默认停留在根目录 `D:\Unity\Unity_learning\Sunset` 的 `main`；长期功能线程当前也默认以 `main` 语义推进并按白名单收口；`codex/` 分支与 `worktree` 只保留给高风险隔离、branch carrier 迁入和特殊实验。
- 如果历史 `worktree` 仍暂时存在，不得默认信任其中的脚本副本、规则副本和入口文档副本与 shared root 同步；凡是执行闸机脚本或读取 live 治理入口时，优先以 shared root 的现行版本为准。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\` 属于治理正文工作区；`D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\` 只属于 TD 镜像区。不要把二者再混成一个入口。
- 进入任一长期线程前，先核验两件事：当前工作目录、`git branch --show-current`；UI 中残留的分支提示不能替代真实 Git 状态。
- 如果共享根目录 `D:\Unity\Unity_learning\Sunset` 当前 checkout 不在 `main`，则必须先由 `sunset-startup-guard` 判断它是否已被其他线程占用；只有在 branch carrier / worktree 例外场景下，才继续按非 `main` 现场处理。
- `shared-root-branch-occupancy.md` 只回答 Git/目录层是否中性；它不替代 Unity/MCP 单实例占用判断。
- 只要任务会触发 Unity/MCP 读写，就必须把 `mcp-single-instance-occupancy.md` 与 `mcp-hot-zones.md` 当成第二层现场闸门；出现 Play/Compile/Domain Reload/对象失效/端口占用等冲突信号时，先降级只读，再决定是否继续。
- 自 2026-03-23 起，只要任务涉及 Unity / MCP live 验证、Console 读取、Scene / Prefab live 回读，除占用层外还必须通过 `mcp-live-baseline.md` 的四项基线核查：
  - `config.toml` 中只保留 `unityMCP`
  - `127.0.0.1:8888` 正在监听且目标 pidfile 存在
  - 当前会话 resources / templates 暴露的 server 确实是 `unityMCP`
  - 当前实例与本轮目标实例一致
- 任何线程、prompt、draft、memory、回执若仍把 `http://127.0.0.1:8080/mcp`、`localhost:8080/mcp`、`mcp-unity` 当成当前 live 口径，统一视为旧口径残留，不能直接当成本轮事实继续外推。
- 如果 `config.toml`、`127.0.0.1:8888`、pidfile 与 `check-unity-mcp-baseline.ps1` 都已通过，但某条旧线程 / 旧会话仍报旧端口、旧桥名或 `resources/templates` 为空，优先判定为“旧会话 MCP 路由缓存未刷新”，不要直接外推成 shared root 服务端已回滚；应改走新线程 / 新会话复核，或先手工对 `http://127.0.0.1:8888/mcp` 做 initialize + `tools/list` 证明服务端可用。
- 凡是为了验证进入 Play Mode 的任务，完成当前取证后都必须先确认已经回到 Edit Mode；未退回前，不算完成现场清理，也不允许把 Unity 让给其他线程。
- 涉及 UI、对话框、气泡、字体、布局、样式的任务，不得以“能显示”为完成标准；必须额外核可读性、锚点、留白、层级遮挡、字体协调性和整体专业感。
- 治理任务若留在 `main`，只允许使用 `git-safe-sync.ps1 -Action sync -Mode governance`，并通过 `-IncludePaths` 明确带上本轮受影响的业务记忆或线程记忆。
- 调用 `git-safe-sync.ps1` 时，必须显式传入 `-OwnerThread <线程名>`；脚本会按线程身份校验当前分支语义，不匹配直接阻断。
- 只要动作属于 shared root 的 live 准入 / 排队 / 唤醒，默认命令入口改为：
  - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 ...`
  这样即使 shared root 已切到旧任务分支，也不会退回执行该分支自带的旧版仓库脚本。
- 如果 shared root 占用文档已经声明 `is_neutral = false`，则 `git-safe-sync.ps1` 的 `task` 模式仍会额外核对 `owner_thread + current_branch`；但在 `main-only` 白名单 sync 下，脚本不再因 unrelated remaining dirty 一刀切阻断。
- 同样从现在起，`git-safe-sync.ps1` 在 `main-only` 白名单 sync 下还会额外核：当前白名单所属 `own roots` 是否已经 clean；如果同根还有自己未纳入本轮的 remaining dirty / untracked，脚本会直接拒绝继续 sync。
- 如果当前线程已经位于某个 `codex/` 分支，且本轮只是顺手修治理规则或治理文档，不必为了同步治理文件强行切回 `main`；此时改用 `git-safe-sync.ps1 -Action sync -Mode task -OwnerThread <线程名> -IncludePaths ...`，只白名单提交本轮治理文件。
- 普通真实实现任务当前默认不再要求先 `ensure-branch`；完成一刀后，默认直接在 `main` 上执行 `git-safe-sync.ps1 -Action sync -Mode task -OwnerThread <线程名> -IncludePaths ...` 做白名单提交。
- 自 2026-03-23 起，只要 `task` 模式白名单中包含 `.cs` 文件，`git-safe-sync.ps1` 会在收口前自动触发代码闸门：
  - 目标文件 UTF-8 检查
  - `git diff --check`
  - 程序集级编译检查
- 代码闸门未通过时，禁止继续收口；不要再把“等用户贴编译报错后再修”当成默认流程。
- 只有命中 branch carrier / worktree 例外场景时，才继续使用 `ensure-branch`、`return-main` 和 `task-active` 这套旧事务模型。
- 无论在 `main` 还是 `codex/` 分支，真实实现任务完成后都必须使用显式白名单同步，不得使用无边界 `git add -A`。
- 如果 `git-safe-sync.ps1` 判断当前不能安全同步，必须明确汇报阻塞点、当前分支、基线 hash 和剩余 dirty 分类；禁止硬提交、硬推送或偷偷跳过 Git 收尾。
- 只要线程回执中的 `当前 own 路径是否 clean` 不是 `yes`，这条回执就不能当作“已完成当前这一刀”或“可进入下一轮 feature prompt”；最多只能当 blocker 报实或 cleanup 未闭环。
- 旧的 `Codex迁移与规划`、旧全局 `tasks.md`、旧 `TD_*` 代办文件都只作为历史兼容入口，不再承接当前治理续办的新任务。
- 评审任务先按 `code-reaper-review.md` 判路由；若落到需要出审视报告的路径，先出报告，不要抢先改文件。
- 场景、预制体、检查器、ScriptableObject 任务先审视再修改；当序列化引用、挂载关系或场景影响范围不清晰时，先给证据和判断，再做改动。
- 历史交接文档和 Claude 迁移文档只作为背景与经验库，不替代当前活跃工作区。
- 修改 steering、规则、治理文档时，遵守唯一规则源原则，避免把同一条规则复制到多处导致双源漂移。

## 8. 本文件边界
- 本文件不是第二套 steering。遇到细节判断，以 `.kiro/steering` 正文与活跃工作区文档为准。
- 当规则冲突时，优先级按“用户当前指令 → 更近目录的 `AGENTS.md` → 本文件 → 历史与迁移参考文档”理解。

