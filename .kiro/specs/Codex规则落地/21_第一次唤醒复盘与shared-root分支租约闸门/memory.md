# 阶段记忆：第一次唤醒复盘与 shared-root 分支租约闸门

## 阶段目标
- 承接阶段 `20` 之后暴露出的新缺口。
- 把“切分支也是 shared root 共享写态”升级成正式闸门。
- 用双阶段唤醒协议和认知钢印，补齐 `superpowers` 灵魂在 Sunset 的本地化落地。

## 当前状态
- **阶段状态**：进行中
- **最后更新**：2026-03-18
- **阶段定位**：证据收口、shared root 分支租约、认知闸门补强

## 当前稳定结论
- 阶段 `20` 已完成，但第一次唤醒证明：
  - 现有闸机能拦住错提交
  - 还不能拦住第一次唤醒阶段的提前切分支
- 第一次唤醒里，真正越线的主要是 `farm`：
  - 提前执行 `ensure-branch`
  - 把 shared root 切到 farm 分支
- `导航检查` 的只读审计结论因此被污染，不应直接视为长期基线
- 这轮后续治理的正确方向不是重做 `20`，而是立 `21` 补：
  - shared root 分支租约闸门
  - 两阶段唤醒协议
  - `AGENTS.md` / `openai.yaml` 级认知钢印

## 会话记录

### 会话 1 - 2026-03-18（阶段 21 立项）
**用户目标**：
> 结合第一次唤醒的真实回包、Gemini 对 superpower 的分析，以及 shared root 再次暴露出的流程漏洞，给出彻底分析，并判断是否需要新阶段继续推进。

**已完成事项**：
1. 通读 `20/第一次唤醒/` 下五份真实回包。
2. 锁定第一次唤醒的最短事故链：
   - `farm` 提前 `ensure-branch`
   - shared root 被切到 farm 分支
   - `导航检查` 在错误 live 分支上做了只读审计
3. 确认当前讨论的 `superpowers` 与此前审查对象一致：
   - `obra/superpowers`
   - 核心 skill：`using-superpowers`
4. 确认本地已有可承接“认知钢印”的入口：
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `C:\Users\aTo\.codex\skills\skills-governor\agents\openai.yaml`
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\agents\openai.yaml`

**关键决策**：
- `21` 不是重做 `20`，而是补 `20` 之后第一次唤醒才显现的新缺口。
- `21` 的核心不在业务代码，而在 shared root 的分支租约与认知入口闸门。
- 原版 `superpowers` 继续不装；本地化吸收它的灵魂才是正路。

**恢复点 / 下一步**：
- 先把 `第一次唤醒/` 证据正式纳入 Git。
- 然后按 `tasks.md` 依次推进：
  - 分支租约模型
  - 脚本补强
  - `AGENTS.md` / `openai.yaml` 认知钢印
  - 双阶段唤醒 prompt

### 会话 2 - 2026-03-18（阶段 21 物理闸机与钢印配置落地）
**用户目标**：
> 不再停留在规划或原理解说，直接把 shared root 分支租约闸机、`AGENTS.md` 钢印、skill `default_prompt` 和双阶段唤醒模板落到物理文件，并给出可验收成品。

**已完成事项**：
1. 在 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 落地 shared root 分支租约机制：
   - 新增 `grant-branch`
   - 新增 `return-main`
   - `ensure-branch` 从 shared root 的 `main` 进入前必须先通过 `Assert-SharedRootBranchGrant`
2. 在 `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md` 增补 grant 字段与解释口径：
   - `branch_grant_state`
   - `branch_grant_owner_thread`
   - `branch_grant_branch`
   - `branch_grant_updated`
3. 把强制钢印块插到 `D:\Unity\Unity_learning\Sunset\AGENTS.md` 最顶部，明确：
   - 第一次回复先过 `sunset-startup-guard` / `skills-governor`
   - 没有显式 Lease / Grant 前，禁止 `ensure-branch`
   - 第一次唤醒阶段必须纯只读
4. 在本机 live skill 配置中落地强制 `default_prompt`：
   - `C:\Users\aTo\.codex\skills\skills-governor\agents\openai.yaml`
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\agents\openai.yaml`
5. 低风险验证通过：
   - `preflight` 可正常读取并输出 grant 字段
   - 用匹配线程名的假分支执行 `ensure-branch` 时，会被“未拿到独占分支租约”硬阻断

**关键决策**：
- shared root 上的 `ensure-branch` 正式被定性为全局写态，不再允许把它当成线程局部动作。
- 第一次唤醒与第二次正式进入业务分支必须分成两阶段；第一次只读，第二次先发 grant 再准入。
- `superpowers` 继续只吸收“认知钢印 + 前置闸门”的灵魂，不引入其 worktree-first 实现。

**恢复点 / 下一步**：
- 产出并分发新的双阶段唤醒模板。
- 仓库内治理文件完成白名单同步后，可按新闸机重新组织业务线程恢复。

### 会话 3 - 2026-03-18（回读 Kiro 规则正文与治理历史，压实规范来源）
**用户目标**：
> 在重新唤醒业务线程前，回顾 Kiro 的全部核心规则与 `Codex规则落地` 历史，重新深度理解用户最初的规范到底是什么、后续每轮补强补到了哪里、现在还差哪些强制化收口。

**已完成事项**：
1. 回读当前 live 入口层：
   - `Sunset当前唯一状态说明_2026-03-17.md`
   - `Sunset现行入口总索引_2026-03-17.md`
   - `基础规则与执行口径.md`
2. 回读核心 steering 正文：
   - `000-context-recovery.md`
   - `rules.md`
   - `workspace-memory.md`
   - `git-safety-baseline.md`
   - `documentation.md`
   - `communication.md`
   - `scene-modification-rule.md`
   - `code-reaper-review.md`
   - `maintenance-guidelines.md`
   - `ui.md`
3. 回读 `Codex规则落地` 根层设计与 `01/02/05/09/10/11/12/20/21` 的阶段任务与记忆摘要，重新串起治理演化链。

**关键决策**：
- 用户最初的核心规范并不是“多写文档”，而是“职责分层 + 入口唯一 + 规则必须真的拦人”。
- 历史上最稳定的主轴一共有 6 条：
  1. `000_代办` 只能是记录/读取层，不能再冒充工作区。
  2. shared root 默认是 `main-common`，真实写入必须先进自己的 `codex/...`。
  3. `worktree` 是例外容器，不是日常开发模式。
  4. Git 中性现场、shared root 占用、Unity/MCP 单实例、A 类热文件锁是不同层的闸门，不能混成一个判断。
  5. Play Mode 取证后必须退回 Edit Mode；UI/气泡/字体/样式必须把审美和专业感当成硬验收。
  6. 规则不能停留在说明书，必须尽量改造成脚本闸机、入口钢印或强制流程。
- 阶段 21 现在已经把“shared root 切分支也算全局写态”正式接到这条主线上，但后续还要继续审计哪些规则仍只是纸面约束。

**恢复点 / 下一步**：
- 用户唤醒线程期间，我后续应继续把“仍是纸面、尚未物理化”的规则做差距清单。
- 新线程恢复时，统一按“两阶段唤醒 + grant 准入 + checkpoint 归还”执行。

### 会话 4 - 2026-03-18（回顾来源文件清单已向用户做精简汇报）
**用户目标**：
> 简略汇报本轮回顾过程中到底读取了哪些文件。

**已完成事项**：
1. 按类别整理并准备汇报本轮读取来源：
   - 启动闸门 skill
   - live 入口层
   - steering 正文
   - `Codex规则落地` 根层与关键阶段
2. 保持只读口径，不新增新的治理结论。

**关键决策**：
- 这轮对用户的汇报以“文件清单 + 一句话用途”形式输出，避免再次展开成长篇分析。

**恢复点 / 下一步**：
- 若用户继续追问，我再从这份来源清单里展开讲某一组规则的演化。

### 会话 5 - 2026-03-18（紧急插单：修复 grant 与 ensure-branch 的自锁）
**用户目标**：
> 先把当前主线任务和进度迅速存入代办，然后立刻处理 `farm` 在线验证暴露出的紧急 Bug：`grant-branch` 写脏 occupancy 后，`ensure-branch` 又把这份合法脏改当成阻断项，导致 shared root 卡死在 `main + branch-granted`。

**已完成事项**：
1. 新增 TD 镜像：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\TD_13_阶段21历史回读暂停与分支租约死锁修复.md`
   - 将原本“历史回读 / 查漏补缺”主线的当前任务、已完成进度、恢复点和本次紧急插单一起登记到记录层。
2. 修补 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`：
   - 新增 `Get-SharedRootOccupancyRelativePath`
   - 新增 `Test-SharedRootLeaseRuntimeDirtyPath`
   - 新增 `Get-BlockingStatusEntries`
   - 让 `grant-branch`、`ensure-branch`、`return-main` 在合法租约运行态下忽略 `.kiro/locks/shared-root-branch-occupancy.md` 这一份脚本自管态脏改
   - 让 `New-PreflightReport` 把该文件标记为 `shared root 租约运行态脏改`，不再误算成普通 blocking dirty
   - `Get-RemainingDirtyEntries` 也同步排除该类别
3. 顺手补了一条安全收口：
   - 当 shared root 仍在 `main` 且租约属于别的线程时，`return-main` 不再允许越权清空他人的未消费 grant
4. 完成低风险验证：
   - PowerShell 语法解析 `OK`
   - 在当前 live repo 上执行只读 `preflight` 时，occupancy 已被正确归类为 `shared root 租约运行态脏改`
   - `shared root lease` 不再因为 occupancy 本身报错，而是只会继续指出真正的其他 dirty（本轮是脚本与新 TD 自身）

**关键决策**：
- 这次不把 `shared-root-branch-occupancy.md` 整体降级为永久 noise。
- 只在“shared root 合法租约运行态”下对它做精确豁免，避免影响未来把 occupancy 当治理文件正常提交。
- 这次 Farm 的失败被正式定性为脚本逻辑漏洞，不是线程违规。

**恢复点 / 下一步**：
- 原本暂停的“历史回读 / 查漏补缺”主线仍保留，后续从 `TD_13` 记录的恢复点继续。
- 紧急 Bug 这条线下一步应先：
  1. 审核并同步本轮脚本修补
  2. 清掉当前 live 的 `main + branch-granted` 中间态
  3. 用 Farm 的第二阶段重新实盘验证 `grant -> ensure-branch`

### 会话 22 - 2026-03-18（补齐 skill 触发显式可见与 trigger log 审计层）
**用户目标**：
> 在 `B` 路线完成后，继续解决“skills 触发用户几乎看不见、也没有统一日志”的治理缺口；不仅要说明当前已经 learn 了什么，还要把 trigger log 真的落盘，避免继续停留在纸面规则。

**已完成事项**：
1. 在全局治理层新增统一审计日志：
   - `C:\Users\aTo\.codex\memories\skill-trigger-log.md`
   - 采用 append-only，专门记录实质性任务里的 skill 触发、触发原因、可见性与结果。
2. 收紧全局规则源：
   - `C:\Users\aTo\.codex\AGENTS.md`
   - `C:\Users\aTo\.codex\memories\global-learning-system.md`
   - `C:\Users\aTo\.codex\memories\global-skill-registry.md`
   - `C:\Users\aTo\.codex\memories\global-learnings.md`
   - 新增 `GL-20260318-002 skill-policy.explicit-skill-callout-and-trigger-log`
3. 收紧治理 skill：
   - `C:\Users\aTo\.codex\skills\skills-governor\SKILL.md`
   - `C:\Users\aTo\.codex\skills\skills-governor\agents\openai.yaml`
   - `C:\Users\aTo\.codex\skills\global-learnings\SKILL.md`
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\SKILL.md`
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\agents\openai.yaml`
4. 把这条要求接进 Sunset 项目入口：
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - 明确 Sunset 实质性任务也必须显式点名命中的 skill，并在收尾前补记 `skill-trigger-log.md`。
5. 写入首条 trigger log：
   - `STL-20260318-001 global-skill-trigger-visibility`

**关键决策**：
- `global-learnings` 与 `global-skill-registry` 解决的是“稳定结论”和“当前状态板”，但不能替代“这次任务到底有没有真的触发 skill”的执行审计层。
- 从本轮开始，skill 触发必须满足两件事：
  1. 首条 `commentary` 显式可见
  2. 收尾前追加 trigger log
- 如果某个应命中的 skill 在当前会话没有显式暴露，但执行了手工等价流程，也必须显式说明并记日志，不能继续留成暗箱动作。

**恢复点 / 下一步**：
- 本轮显式可见与日志化缺口已补上。
- 若用户继续追问，可直接汇总当前 global learnings 已有条目，并说明本轮新增的 trigger log 机制如何使用。
- 若用户把主线重新切回阶段 21 的“历史回读 / 纸面规则差距清单”，则从 `TD_13` 记录的恢复点继续。

### 会话 23 - 2026-03-18（Gemini 锐评审核：主线拉回开发恢复）
**用户目标**：
> 对 Gemini 最新建议走一次锐评审核，并把主线从“继续补强新基建”拉回到“先恢复健康开发、先验证阶段 21 产物是否真的可用”。

**已完成事项**：
1. 依据 `code-reaper-review.md` 对 Gemini 建议做 A/B/C 裁决：
   - 结论为 `路径 B`
2. 已核实其核心判断成立：
   - 阶段 21 可以视为已封板
   - 当前 shared root 处于 `main + neutral`
   - 此刻最值得优先做的是 farm 的 `grant -> ensure-branch` 实盘重测，而不是立即继续新基建
3. 已做本地化修正：
   - Gemini 提到的“后续自动化 hook”不直接开正文阶段
   - 仅先写入 `000_代办`：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\TD_14_自动化skill触发日志Hook待办.md`
4. 已准备把下一步收束为：
   - 发给 farm 的阶段二 Lease / Grant 与准入重测 Prompt

**关键决策**：
- 这次 Gemini 的方向判断是对的，但“阶段 22”只能先作为 TD 镜像待办，不得立即开新正文阶段。
- 当前主线不再是继续造新基建，而是先验证阶段 21 产物是否已经把 shared root 恢复到可正常开发。

**恢复点 / 下一步**：
- 下一步直接向用户交付：
  1. `路径 B` 的锐评裁决
  2. `TD_14` 已登记确认
  3. 可直接发给 farm 的阶段二重测 Prompt

### 会话 24 - 2026-03-18（TD_14 与锐评裁决已同步，现场重新回到可重测状态）
**用户目标**：
> 不只要结论，还要把“自动化 hook 先列代办、当前先恢复开发能力”真正固化到仓库里，让后续 farm 重测基于稳定现场进行。

**已完成事项**：
1. 已将以下文件同步到 `main`：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\TD_14_自动化skill触发日志Hook待办.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\21_第一次唤醒复盘与shared-root分支租约闸门\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地\memory_0.md`
2. governance sync 提交已创建并推送：
   - `98fc19e6 / 2026.03.18-10`
3. 当前 shared root 再次确认：
   - `main`
   - `is_neutral = true`
   - `branch_grant_state = none`
   - `git status clean`

**关键决策**：
- 当前现场已经重新回到“可发放 grant、可执行 farm 阶段二重测”的开发恢复点。
- 自动化 hook 仍停留在 TD 层，不占用当前恢复窗口。

**恢复点 / 下一步**：
- 下一步直接进入 farm 的阶段二 Lease / Grant 与准入重测。
