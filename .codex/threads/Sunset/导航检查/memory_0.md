# 导航检查线程记忆

## 线程概述

- 线程目标：对 Sunset 导航系统做只读审计、核实现状、沉淀可接续的判断基线。
- 默认工作目录：`D:\Unity\Unity_learning\Sunset`
- 默认分支：`main`

## 当前状态

- 最近更新：2026-03-15
- 当前主线目标：把线程旧分析、Sunset 规则、现状代码、场景挂载、Prefab 分布和 Editor 配置统一核对，形成我方审计结论。
- 当前恢复点：若继续推进，直接从 `1.0.0初步检查/03_现状核实与差异分析.md` 的问题清单进入下一轮整改或方案拆解。

## 会话记录

### 会话 1 - 2026-03-15

- 用户显式指定线程目录 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查` 与工作区 `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查`，要求彻底阅读线程目录全部内容，并核实现状代码与工作区文档后输出我的分析。
- 已完成事项：读取线程旧稿 `01_导航代码索引.md`、`02_导航全盘分析报告.md`；确认子工作区和父工作区当前无现成文档；读取 `.kiro/steering/README.md`、`rules.md`、`workspace-memory.md`、`systems.md` 与 `.codex/threads/线程分支对照表.md`；核实导航主链代码、Primary 场景挂载、相关 Prefab 与 Editor 代码。
- 关键决策：本轮不覆盖线程旧稿，而是在 `1.0.0初步检查` 目录新增 `03_现状核实与差异分析.md`；按“子工作区 memory → 父工作区 memory → 线程 memory”顺序首次建立记忆链。
- 关键结论：`GameInputManager -> PlayerAutoNavigator -> NavGrid2D` 仍是核心主链；`PlacementNavigator` 为运行时创建；`CloudShadowManager` 反射读取 NavGrid 私有字段但主场景当前未启用该路径；`NavGrid2D` 当前并非真正 Zero GC，因为 `Physics2D.OverlapCircleAll` / `Physics2D.OverlapPointAll` 官方文档明确说明会分配返回数组。
- 主线恢复点：后续如继续本线程，应先围绕 GC 审计、刷新节流、文档漂移三件事展开，而不是回到旧报告的排序热点单点判断。

### 会话 1 - Git 收尾补记 - 2026-03-15

- 已按项目规则执行 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1 -Action sync -Mode governance -IncludePaths ...`，创建并推送提交 `2026.03.15-01`（`3d7d65d5`）。
- 需要特别记住：`governance` 模式在本次预检里把两份既有治理线 memory 改动也一并纳入允许同步范围，并已随同提交进入 `main`：
  - `.kiro/specs/Steering规则区优化/memory.md`
  - `.kiro/specs/Steering规则区优化/当前运行基线与开发规则/memory.md`
- 当前仓库仍有大量与本线程无关的残留脏改，尤其是 NPC 线资产、其他线程 memory、以及本线程旧稿的删除/未跟踪状态；如果下轮继续推进，应继续维持“只白名单当前线程文件”的策略。

### 会话 2 - 2026-03-16（冻结汇总落盘）

- 用户要求把本线程的冻结现场快照写入 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\2026.03.16冻结文档汇总\`，文件名使用线程名，并只需简短回报。
- 当前主线目标已从“继续导航审计”切换为“把已验证的冻结现场正式落盘到治理汇总区”，这轮属于治理支撑动作，不继续推进导航实现或整改。
- 已完成事项：读取目标汇总目录及现有样例 `项目文档总览.md`，确认沿用“线程名.md”命名与 12 段冻结快照结构；将冻结快照写入 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\2026.03.16冻结文档汇总\导航检查.md`；按“子工作区 memory → 父工作区 memory → 线程 memory”顺序同步追加记忆。
- 关键决策：本轮只转写已验证的冻结快照，不重开导航代码审计，不新增代码、场景、Prefab、资源或规则正文修改。
- 关键结论：当前共享热文件冲突风险仍集中在 `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity` 与 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs` 的未持锁 dirty；`D:\Unity\Unity_learning\Sunset\.kiro\locks\` 目录仍不存在；`D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs` 当前未 dirty，但仍是导航入口观察对象。
- 主线恢复点：冻结解除后，如继续本线程，应先根据统一裁决确认 A 类锁目录、owner 与现有 diff 归属，再回到 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\1.0.0初步检查\03_现状核实与差异分析.md` 继续导航审计。

### 会话 3 - 2026-03-16（审计阶段收口）

- 用户明确要求当前优先处理“文档重组与阶段状态收口”，不要直接进入代码整改；并要求先核对工作目录 / 分支 / HEAD / dirty，再明确当前文档删改、迁移、未跟踪项，最后判断下一步方向。
- 已核对现场：
  - 工作目录：`D:\Unity\Unity_learning\Sunset`
  - 当前分支：`codex/npc-main-recover-001`
  - 当前 `HEAD`：`b9b6ac48`
  - 当前 repo dirty 很重，但与本线程直接相关的文档现场主要是：
    - 删除态：`D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\01_导航代码索引.md`、`D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\02_导航全盘分析报告.md`
    - 迁移承接态：`D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\1.0.0初步检查\01_导航代码索引.md`、`D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\1.0.0初步检查\02_导航全盘分析报告.md`
    - 修改态：`D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\memory_0.md`
    - 已存在阶段基线稿：`D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\1.0.0初步检查\03_现状核实与差异分析.md`
- 本轮新增阶段文档：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\1.0.0初步检查\04_审计阶段结案与移交建议.md`
- 关键决策：将 `1.0.0初步检查` 先作为“审计基线阶段”正式收口，不在当前阶段内直接滑入代码整改。
- 当前主线恢复点：如果用户后续确认继续导航整改，下一步应新开整改阶段并先补 `requirements.md` / `design.md` / `tasks.md`；若不继续，本线程当前即可停在“审计基线阶段结案”状态。

### 会话 4 - 2026-03-18（阶段22第一阶段 prompt 已回包）

- 用户要求从 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\可分发Prompt` 领取本线程专属 prompt，并完成第一阶段 prompt 的读取和回应。
- 本轮显式使用了 `skills-governor`、`sunset-workspace-router`、`sunset-thread-wakeup-coordinator`，并手工执行了 `sunset-startup-guard` 的等价 preflight。
- 已核对当前 live 现场：
  - `D:\Unity\Unity_learning\Sunset`
  - `main`
  - `14838753b4ae9b09b2146b92fb3bfdc9ac82b2a0`
  - `git status --short --branch = ## main...origin/main`
- 已回读当前治理入口、shared root 占用文档、线程记忆、工作区记忆，以及 `1.0.0初步检查` 的 `03/04` 文档；并重新抽查导航核心代码，确认以下事实仍成立：
  - `GameInputManager.cs` 仍使用 `OverlapPointAll`
  - `PlayerAutoNavigator.cs` / `NavGrid2D.cs` 仍使用 `OverlapCircleAll`
  - `PlacementNavigator.cs` 仍走 `ClosestPoint`
  - `ChestController.cs` / `TreeController.cs` 仍直接触发 `NavGrid2D.OnRequestGridRefresh`
- 已把阶段一回包写入：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\线程回收\导航检查.md`
- 关键决策：
  - 当前 live 仍支持原有 `1.0.0` 审计基线，不需要回退到更早线程旧稿；
  - 若继续本线程主线，下一轮确实需要进入真实写入，但必须先走阶段二 grant 准入；
  - 推荐 continuation branch 仍是 `codex/navigation-audit-001`；
  - 最小 checkpoint 先固化 `2.0.0整改设计` 的 `requirements.md / design.md / tasks.md`，不直接在 `main` 上改导航代码，也不直接进入 Unity / Play Mode。
- 当前主线恢复点：
  - 若用户批准阶段二，先申请 `grant-branch`，再执行 `ensure-branch`；
  - 若未批准，本线程继续保持只读，不自行切分支。

### 会话 5 - 2026-03-19（queue-aware业务准入 01 回执）

- 用户要求从 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\可分发Prompt` 领取 `导航检查` 专属 prompt 并开始写回执单。
- 当前主线没有变化：仍是把导航线程从 `1.0.0` 审计基线推进到 `2.0.0` 的最小 code checkpoint；本轮子任务是先拿 continuation branch 租约。
- 读取 prompt 后先执行 `request-branch`。稳定 launcher 实测存在 optional 参数未转发缺口，`-BranchName` 被吃掉，返回 `request-branch 必须提供 -BranchName。`
- 随后用 `main:scripts/git-safe-sync.ps1` 的手工等价 canonical 方式重试，得到：
  - `STATUS: LOCKED_PLEASE_YIELD`
  - `TICKET: 3`
  - `QUEUE_POSITION: 2`
  - `REASON: 当前 live 分支是 'codex/npc-roam-phase2-003'，只有 main 大厅才能发放分支租约。`
- 本轮真实现场因此更新为：
  - `cwd = D:\Unity\Unity_learning\Sunset`
  - `branch = codex/npc-roam-phase2-003`
  - `HEAD = 7385d1236d0b85c191caff5c5c19b08678d1cf80`
  - `git status --short --branch = ## codex/npc-roam-phase2-003...origin/codex/npc-roam-phase2-003` + `M .kiro/locks/shared-root-branch-occupancy.md`
- 已停止继续 `ensure-branch`，未进入 Unity / MCP / Play Mode，未碰 `GameInputManager.cs` / `Primary.unity`，并已把回执写入：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\线程回收\导航检查.md`
- 关键决策：不绕过 queue 直接切分支；保持 `codex/navigation-audit-001` 作为 continuation branch，等待 shared root 回到 `main + neutral` 后再继续。
- 主线恢复点：下次被唤醒时，直接从 `ensure-branch -> 首个 NavGrid2D / PlayerAutoNavigator 非热文件 checkpoint` 继续，而不是重做 `1.0.0` 审计。

### 会话 6 - 2026-03-22（main-only 首个真实导航代码 checkpoint）

- 用户明确要求停止继续停在 docs-first 或旧 branch waiting 口径上，先处理 `codex/navigation-audit-001` 的遗留，再在同一轮进入真实导航开发。
- 本轮显式继续使用 `skills-governor`、`sunset-workspace-router`，并按 Sunset 启动闸门做了等价 preflight。
- 当前 live 现场：
  - `D:\Unity\Unity_learning\Sunset`
  - `main`
  - `9e6a94ca539e895b3a8398b2e23746cefc70bf08`
- 本轮处理旧分支遗留的结论：
  - 保留并迁回 `main`：`2.0.0整改设计` 四件套
  - 判废：旧分支根层 docs-first 垫片，不再继续作为 blocker
- 本轮真实代码改动：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavGrid2D.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
- 本轮真实代码结论：
  - `NavGrid2D` 的 `IsPointBlocked()` 已从 `OverlapCircleAll(...)` 改到复用缓冲区查询
  - `PlayerAutoNavigator` 已开始支持移动 NPC / 动态导航单元的识别、侧向绕行和持续阻挡时的重规划入口
  - 玩家导航本体已实现 `INavigationUnit`，为后续 NPC/NPC 局部规避接线留出兼容位
- 当前恢复点：
  - 下一轮先验证真实场景里的“玩家自动导航绕移动 NPC”；
  - 如果验证成立，再继续扩到 NPC/NPC 的真实接线，而不是回头重做文档整理。

### 会话 7 - 2026-03-22（是否需要统一导航系统的线程结论）

- 用户明确追问：当前这些修改是否已经变成“在屎山上雕花”，以及是否应该为玩家、NPC、怪物、牲畜、宠物等所有移动体建立一个彻底共享的导航系统。
- 本轮只读复盘后的线程级判断：
  - 是，需要一个共享的导航核心；
  - 但不应该做成“所有对象共用一个超大万能脚本”，而应该做成“共享导航基础设施 + 不同对象各自的行为脑与运动适配器”。
- 促成这个判断的已验证事实：
  - `PlayerAutoNavigator` 和 `NPCAutoRoamController` 现在各自维护一套寻路、卡住恢复和移动循环；
  - `NavGrid2D` 目前只足够承担静态/准静态网格职责；
  - `INavigationUnit` 已存在，但还没有真正成为全系统共享导航主链的落地骨架；
  - 玩家和 NPC 当前连最终运动语义都不一致。
- 线程级结论：
  - 现有补丁不是完全没价值，但如果继续沿着“玩家侧补一点、NPC 侧补一点、卡住再重建一点”的方式推进，会越来越像高级雕花。
  - 后续重构应该先把导航问题重新定义为：静态路径规划、动态代理避让、最终运动/接触解析三层，再据此统一所有移动体。

### 会话 8 - 2026-03-22（屎山修复工作区建立与单文档主表）

- 用户明确指定新工作区 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查`，要求不要再写 requirements/design/tasks 五件套，而是只维护一个可长期执行、可勾选、可修订的阶段设计主文档。
- 本轮显式使用 `skills-governor`、`sunset-workspace-router`，并按 Sunset 启动闸门做了等价 preflight。
- 当前 live 现场：
  - `D:\Unity\Unity_learning\Sunset`
  - `main`
  - `a8d2e357aa2fbff8b905cbe3388dd5f5275db351`
- 本轮完成：
  - 创建父工作区记忆：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\memory.md`
  - 创建子工作区主文档：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\统一导航重构阶段设计与执行主表.md`
  - 创建子工作区记忆：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
- 本轮关键决策：
  - `屎山修复/导航检查` 不再继续沿用五件套文档模式，而改为“唯一主文档 + memory 追加”的长期工作方式。
  - 主文档将直接承载需求描述、阶段设计、执行清单、状态标记和修订记录，作为后续统一导航重构的唯一入口。
- 当前恢复点：
  - 后续再推进这条线时，默认先读 `统一导航重构阶段设计与执行主表.md`，再按阶段执行，不再回到旧的文档散铺模式。

### 会话 9 - 2026-03-23（锐评 001 审核结论）

- 用户要求对 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\锐评\001.md` 走审核锐评流程。
- 本轮显式使用 `skills-governor`、`sunset-workspace-router`、`sunset-review-router`，并按 Sunset 启动闸门做了等价 preflight。
- 已核对：
  - 锐评文件本体
  - `D:\Unity\Unity_learning\Sunset\.kiro\steering\code-reaper-review.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\统一导航重构阶段设计与执行主表.md`
  - 相关 live 代码：`NPCAutoRoamController.cs`、`NPCMotionController.cs`、`PlayerMovement.cs`、`INavigationUnit.cs`
- 路径判断：
  - 本次锐评走 **Path B**
- 关键结论：
  - 锐评指出“最终运动语义不一致”“S3 算法边界不足”“执行顺序过瀑布流”“缺 sleeping 机制”这些问题，大方向成立。
  - 但其“必须把 NPC 运动统一改成 `linearVelocity`”的建议过于绝对，当前不能直接作为项目定案。
  - 后续应把这份锐评作为主表的局部修订输入，而不是直接照单实现。

### 会话 10 - 2026-03-23（Path B 全面落地第一批共享导航核心）

- 用户明确要求：不要停在审核结论，直接按 Path B 把所有成立任务落地，并开始全面实现。
- 本轮显式继续使用 `skills-governor`、`sunset-workspace-router`、`sunset-review-router`，并按 Sunset 启动闸门做了等价 preflight。
- 当前 live 现场：
  - `D:\Unity\Unity_learning\Sunset`
  - `main`
  - `c661f669`
- 本轮已完成：
  - 修订 `屎山修复/导航检查/统一导航重构阶段设计与执行主表.md`
  - 扩展 `INavigationUnit.cs`
  - 新增共享导航核心第一批文件：
    - `NavigationAgentSnapshot.cs`
    - `NavigationAvoidanceRules.cs`
    - `NavigationAgentRegistry.cs`
    - `NavigationLocalAvoidanceSolver.cs`
  - 修改 `PlayerAutoNavigator.cs`，接入共享代理注册与共享局部避让
  - 修改 `NPCAutoRoamController.cs`，接入共享代理注册与共享局部避让
- 关键结论：
  - 这轮已经把“共享动态代理层”从纯设计推进到真实代码层。
  - 玩家与 NPC 开始共享动态代理真相，不再完全依赖各自私有补丁。
  - 但统一路径执行层和最终运动语义收口仍未完成，这轮不是终局，而是第一批核心基础设施已成立。
- 当前恢复点：
  - 下一轮优先做最小静态 / Unity 验证；
  - 验证通过后，继续推进共享路径执行层，而不是回头再补局部动态避让私货。

### 会话 11 - 2026-03-23（P0 失败后的规则层纠偏）

- 用户明确指出：当前 P0 不合格，右键导航路线上有 NPC 时，玩家仍然只是在推着 NPC 走。
- 本轮我重新做了代码、Prefab 搭载和场景脚本挂载层面的只读核查，并确认：
  - 玩家与 NPC 的导航脚本搭载真实存在
  - 当前共享入口不是“没接上”，而是“接上了但规则仍不对”
- 本轮关键排查结论：
  - 共享让行规则把自动导航玩家放在了过高优先级，导致玩家面对 moving NPC 时默认更偏向继续前冲，而不是主动避让
  - solver 近距离阻挡时的减前冲力度也不足
- 本轮已完成修正：
  - `NavigationAvoidanceRules.cs`：玩家自动导航遇到 moving NPC / Enemy 时默认让行
  - `NavigationLocalAvoidanceSolver.cs`：增强近距离动态阻挡时的减前冲与侧绕
  - `NavigationAvoidanceRulesTests.cs`：新增最小规则回归测试
- 当前恢复点：
  - 现在应优先复验这轮规则纠偏后的运行态行为；
  - 如果仍然失败，则说明主因已进入共享路径执行层缺失或最终运动语义冲突。

### 会话 12 - 2026-03-23（玩家推 NPC 的直接纠偏）

- 用户再次强调：当前 P0 仍不合格，玩家自动导航路线遇到 NPC 时仍然只是在推着走。
- 本轮我重新核对了：
  - 角色脚本搭载事实
  - NPC Prefab 上的 roam / motion / collider / rigidbody 搭载
  - `Primary.unity` 中玩家导航脚本挂载
  - 当前共享规则与 solver 实现
- 本轮关键发现：
  - 自动导航玩家在共享规则里没有被强制要求对 moving NPC 主动让行，这是当前最直接的一处规则层错误。
- 本轮已完成：
  - 修正 `NavigationAvoidanceRules.cs`
  - 修正 `NavigationLocalAvoidanceSolver.cs`
  - 新增 `NavigationAvoidanceRulesTests.cs`
- 当前恢复点：
  - 现在应优先看修正后是否仍然推着 NPC 走；
  - 如果仍失败，就不再继续补规则，而是直接推进共享路径执行层或最终运动语义收口。

### 会话 13 - 2026-03-23（NavigationAvoidanceRulesTests 编译修复）

- 用户要求我直接认领并修复 `NavigationAvoidanceRulesTests.cs` 带来的编译错误。
- 本轮已确认：
  - 问题根因不是共享导航核心代码缺失
  - 而是 `Tests.Editor.asmdef` 下的测试文件直接引用了主程序集里的全局类型，导致编译期不可见
- 本轮已完成：
  - 将 `Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs` 改写为反射式测试
  - 通过最小代码闸门确认 `Tests.Editor` 装配可编译
- 当前恢复点：
  - 这组测试装配层的“找不到类型”错误已经止血；
  - 后续继续回到“玩家为什么仍然推着 NPC 走”的运行态复验。

### 会话 14 - 2026-03-23（基于 NPC 正式回执继续打 P0）

- 用户补充了 NPC 线的正式回执，并要求不打断当前修复进程，继续直接完成任务。
- 本轮已吸收 NPC 回执中的关键前提：
  - `Moving / ShortPause / LongPause` 先稳定
  - `IsMoving` 先稳定
  - `rb.MovePosition(...)` 仍视为 NPC 当前基线
  - `001 / 002 / 003` 的刚体与碰撞配置当前视为稳定
- 这让我可以把当前排查责任继续锁在导航线，而不是再怀疑 NPC 线正在边联调边改地基。
- 本轮进一步发现并修正：
  - `PlayerMovement.UpdateMovement()` 会把输入再次 `normalized`，导致 solver 的减前冲 / 减速信息被最终运动层吞掉
  - 已修改 `PlayerMovement.cs`，改为 `Vector2.ClampMagnitude(...)`
  - 已修改 `PlayerAutoNavigator.cs`，增加临时动态绕行点
  - 已修改 `NavigationLocalAvoidanceSolver.cs`，增加 `SpeedScale`、阻挡代理位置 / 半径 / 建议侧绕方向
- 当前恢复点：
  - 现在需要优先复验“玩家是否仍在推着 NPC 走”；
  - 如果仍失败，则下一刀应直接进入共享路径执行层，而不是继续补局部 steering。

### 会话 15 - 2026-03-23（Primary.unity 导航热文件卫生处理）

- 当前主线目标：
  - 继续服务 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查` 这条“玩家右键导航遇 NPC 不能继续推着走”的主线。
- 本轮子任务：
  - 先处理我留在 `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity` 上的导航侧脏改卫生问题，避免继续堵住热场景。
- 本轮 live 结论：
  - `Primary.unity` 中属于导航线的新增字段只有 `dynamicObstaclePadding`、`dynamicObstacleRepathCooldown`、`dynamicObstacleVelocityThreshold`、`obstacleProbeBufferSize`、`sharedAvoidanceLookAhead`、`avoidancePriority`。
  - 这些值与 `PlayerAutoNavigator.cs` 当前代码默认值一致，不需要继续靠 scene 序列化保存。
  - 同一份 scene diff 里还混有 `StoryManager`、对话测试状态等非导航内容，不应整文件纳入导航 checkpoint。
- 本轮完成：
  - 已从 `Primary.unity` 移除上述 6 个导航字段级脏改。
  - 已把需要保留的导航侧 WIP 收口为最小代码 checkpoint 候选：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
- 恢复点：
  - `Primary.unity` 对我这条导航线已不再有上述字段级占用；
  - 下一步回到主线时，应继续围绕代码验证“玩家绕开 NPC”，而不是再把中间态参数压到热场景里。
