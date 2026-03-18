# 阶段记忆：shared root 现场回正与物理闸机落地

## 阶段目标
- 接管 `2026-03-18` live 事故复盘后的新治理主线。
- 先把 shared root 从混线现场回正，再把 rules / skills / AGENTS 升级成真正的物理闸机。

## 当前状态
- **阶段状态**：进行中，待用户审核
- **最后更新**：2026-03-18
- **阶段定位**：文档先行；暂不直接执行 Git 手术

## 当前稳定结论
- 当前 live 现场已推翻旧的“shared root 已稳定回到 `main`、`worktree` 已彻底退役”的口径。
- 问题根本原因不是“并发这个目标错了”，而是：
  - 错分支继续写入没有被物理阻断
  - owner 未归还没有被强制拦停
  - 文档型规则无法替代脚本级闸机
- Gemini 的高层诊断可以吸收，但它给出的 Git 手术命令序列仍需进入本阶段 runbook 审核，不能直接视为已批准执行。
- 用户已经人为冻结新的实质性写入，因此本阶段可以先专注文档、清单与方案，不急于继续扩散现场。

## 会话记录

### 会话 1 - 2026-03-18（新阶段立项与认知同步）
**用户目标**：
> 把这轮反思正式落入新的 `20` 阶段，不再继续污染旧阶段；新的内容和代办必须在新位置承接，旧阶段要有迁移标记，避免以后再误读。当前先做文档工作，不直接动 Git。

**完成事项**：
1. 新建 `20_shared-root现场回正与物理闸机落地` 阶段目录。
2. 写入本阶段 `analysis.md / tasks.md / memory.md`，统一记录：
   - live 事故事实
   - 问题本质
   - 对 Gemini 建议的吸收与保留意见
   - 待审核 runbook 与物理闸机任务清单
3. 明确本阶段边界：
   - 文档先行
   - 现场回正与脚本改造方案先行
   - Git 手术后续待用户审核
4. 在旧阶段显式写入迁移标记：
   - `09_强制skills闸门与执行规范重构/tasks.md`
   - `11_main-branch-only回归与worktree退役收口/tasks.md`
   - `12_治理工作区归位与彻底清盘/tasks.md`
   - `11_main-branch-only回归与worktree退役收口/memory.md`
   - `12_治理工作区归位与彻底清盘/memory.md`
5. 同步改写父级与线程级摘要，让 `20` 成为新的可读入口。

**关键决策**：
- `20` 从现在开始是新的治理主线。
- `09/11/12` 只保留历史完成层，不再继续承接这轮 live 回正。
- 在 shared root 真正 clean 之前，不再宣称系统已恢复正常开发。

**恢复点 / 下一步**：
- 回写 `09/11/12` 的迁移标记。
- 同步父级 memory 与线程 memory。
- 等用户审核 `20` 的任务清单后，再进入下一轮实操设计。

### 会话 2 - 2026-03-18（第一波物理闸机已写入脚本并完成负例自测）
**用户目标**：
> 在文档纠偏之后，不再停留在空方案；先把 `git-safe-sync.ps1` 的第一波物理闸机真正写进去，并同步修正现行文档与阶段清单。

**完成事项**：
1. 实核 live 现场，确认 Gemini 所称“shared root 已被人工清回 `main + clean`”当前并不成立：
   - `D:\Unity\Unity_learning\Sunset @ codex/npc-roam-phase2-003 @ 2ecc2b75`
   - shared root 仍有 mixed dirty
   - farm cleanroom worktree 仍存在
2. 直接修改 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`，落下第一波硬阻断：
   - 新增强制参数 `-OwnerThread`
   - `task` 模式位于 `main` 时直接阻断
   - `task` 分支不是 `codex/` 前缀时直接阻断
   - `task` 分支与 `OwnerThread` 语义不匹配时直接阻断
   - `ensure-branch` 的目标分支也必须与 `OwnerThread` 匹配
3. 同步修正现行文档：
   - `AGENTS.md`
   - `.kiro/steering/git-safety-baseline.md`
   - `基础规则与执行口径.md`
   - `shared-root-branch-occupancy.md`
   - `Sunset当前唯一状态说明_2026-03-17.md`
4. 更新本阶段任务清单，明确：
   - B 阶段第一波脚本闸机已完成
   - A 阶段现场回正仍未完成
5. 完成脚本负例自测：
   - `OwnerThread = NPC` + 当前 `codex/npc-roam-phase2-003`：preflight 允许继续
   - `OwnerThread = 农田交互修复V2` + 当前 `codex/npc-roam-phase2-003`：preflight 正确拒绝
   - `OwnerThread = NPC` + `ensure-branch codex/farm-guard-test`：正确直接抛出 FATAL

**关键决策**：
- 这轮只推进“脚本闸机第一波”，不把 Gemini 的 Git 手术命令冒充成已执行事实。
- 当前仍不能把 A 阶段标记为已完成；live 现场依旧需要单独回正。
- 第一波闸机已经能拦住最常见的“错线程在错分支上继续写”。

**恢复点 / 下一步**：
- 继续细化 A 阶段的人工 runbook。
- 再决定第二波闸机是否补 shared root owner/lease 和 dirty transfer gate。

### 会话 3 - 2026-03-18（第二波最小 shared root 闸机落地，并确认历史 worktree 脚本漂移）
**用户目标**：
> 不要停在“已经有第一波闸机”的半完成状态；继续把第二波 shared root 闸机补到最小可用，并先把文档、任务清单和 live 事实同步好。

**已完成事项**：
1. 直接继续改写 `scripts/git-safe-sync.ps1`，补上第二波最小 shared root 闸机：
   - 若 `shared-root-branch-occupancy.md` 已声明 `is_neutral = false`，则 `task` 模式必须同时匹配：
     - `owner_thread`
     - `current_branch`
   - shared root 上只要仍存在未纳入本轮白名单的 remaining dirty，`task` 模式就直接阻断，不再允许“留着 unrelated dirty 继续同步”
   - 从 shared root 的 `main` 执行 `ensure-branch` 前，占用文档必须先回到 neutral
2. 同步修正文档口径：
   - `shared-root-branch-occupancy.md`
   - `git-safety-baseline.md`
   - `基础规则与执行口径.md`
   - `AGENTS.md`
   - `20/tasks.md`
3. 完成第二波脚本自测：
   - 在当前 shared root 上，`OwnerThread = NPC` 也会因为 remaining dirty 被 `task` preflight 正确阻断
   - 在当前 shared root 上，`OwnerThread = 农田交互修复V2` 会同时触发分支语义不匹配与 shared root owner 不匹配
   - 在 `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001` 这个干净 cleanroom 现场里，调用 shared root 最新脚本做 `task` preflight，可正常通过
4. 新发现一个 live 风险：
   - farm cleanroom worktree 内自带的 `scripts/git-safe-sync.ps1` 仍是旧版副本，连 `-OwnerThread` 参数都没有
   - 这证明历史 worktree 一旦长期保留，脚本 / 规则副本会发生版本漂移，因此 shared root 仍应被视为治理脚本与入口文档的唯一 live 来源

**关键决策**：
- 第二波最小闸机已经开始落地，但 shared root 现场回正仍未完成，所以系统状态仍然不是“已恢复正常开发”。
- 当前新增的 owner/lease 校验，是“在 shared root 已声明 occupied 时会拦人”的最小闸机；自动 claim / release 仍留在后续扩展项，不伪装成已经完成。
- “历史 worktree 副本会漂移”现在已经不再只是偏好判断，而是有 live 证据支持的治理风险。

**恢复点 / 下一步**：
- 继续输出 A 阶段的 shared root 人工回正 runbook。
- 再判断是否要追加 shared root 自动 claim / release wrapper，还是继续坚持“先人工 runbook，后脚本化”。

### 会话 4 - 2026-03-18（A 阶段 shared root 人工回正 runbook 已落盘）
**用户目标**：
> 既然第二波最小闸机已经补了，就不要停在“脚本更严格了”；继续把 A 阶段 shared root 回正的手工 runbook 写出来，并把当前 owner / blocker / blocked、回退点和风险都核清。

**已完成事项**：
1. 重新核对当前 live 分支历史与远端状态：
   - `codex/npc-roam-phase2-003 @ 2ecc2b75`
   - 目标回退点是 `c81d1f99`
   - 当前 `origin/codex/npc-roam-phase2-003` 也在 `2ecc2b75`
   - `main / origin/main` 在 `64ff9816`
2. 结合 `导航检测 / NPC / 农田交互修复V2 / spring-day1` 的并发讨论回包，写明当前分层裁定：
   - 业务承载分支 owner：`NPC`
   - 顶部错位提交 owner：`导航检测`
   - shared root 当前 tracked dirty blocker：`农田交互修复V2`
   - 当前治理线程 blocker：`Codex规则落地` 这轮未提交的 stage 20 文档与脚本改动
3. 新建：
   - `runbook.md`
   - 内容包含：
     - staged / unstaged / untracked 的分组 park 方式
     - 推荐的 `reset --hard c81d1f99 + push --force-with-lease` 路径
     - 保守替代的 `git revert 2ecc2b75` 路径
     - shared root 切回 `main` 后如何只恢复治理 stash、不恢复 farm stash
     - shared root 占用文档如何回填为 neutral
     - 哪些步骤必须人工执行，哪些可以留给后续 wrapper
4. `20/tasks.md` 中 A 阶段的四项 runbook 任务已全部改为完成。

**关键决策**：
- 当前 runbook 默认推荐“分组 park + 改写 NPC 分支历史 + force-with-lease + shared root 回 `main`”。
- runbook 明确把“恢复治理 stash”和“恢复 farm stash”拆开，避免再次把 shared root 搅脏。
- 当前仍然没有执行 Git 外科；runbook 只是进入“可审核、可手工执行”的状态。

**恢复点 / 下一步**：
- 等用户审核 `runbook.md`。
- 审核通过后，再决定是否由用户人工执行 runbook，还是先继续补自动 claim / release wrapper。

### 会话 5 - 2026-03-18（A 阶段已执行，shared root 已回到 main）
**用户目标**：
> 既然已经正式授权，就不要再停留在“待审核 runbook”；直接执行 A 阶段 Git 外科，把 shared root 回正，然后继续做 D 阶段文档收口与治理同步。

**已完成事项**：
1. 按 runbook 的安全路径分组 park：
   - `stage20-governance-parking`
   - `farm-root-dirty-parking`
2. 将 `codex/npc-roam-phase2-003` 从 `2ecc2b75` 回正到 `c81d1f99`：
   - 本地 `reset --hard c81d1f99`
   - 远端 `push --force-with-lease`
3. 将 shared root 切回 `main`，确认当前 `git status --short --branch` 已回到 clean。
4. 恢复治理 stash，不恢复 farm stash。
5. 清掉 Git worktree 挂载，使 `git worktree list --porcelain` 当前只剩 shared root。
6. 新增 D 阶段正式文档：
   - `operating-model.md`
   - 把最终模型明确写成 `main-common + branch-task + checkpoint-first + merge-last`
7. 更新活入口文档与占用文档，使当前口径变成：
   - shared root 已回到 `main`
   - 当前仍处于 `governance-main-finalizing`
   - 完成最后一次治理同步后才回到最终 neutral

**关键决策**：
- A 阶段不再是“待执行 runbook”，而是已经完成的 live 事实。
- 当前阻塞已经从“业务错位 + shared root 脏乱”收缩成“治理线程还在 main 上做最后一次同步”。
- farm cleanroom 在 Git 层已经不再是活跃 worktree；若磁盘上还有空目录残壳，也不再拥有 Git 身份。

**恢复点 / 下一步**：
- 继续完成治理同步。
- 同步完成后，把 shared root 占用文档收成最终 neutral，并输出全线程唤醒清单。

### 会话 6 - 2026-03-18（阶段 20 治理同步已形成 checkpoint，正在做最后 neutral 回填）
**用户目标**：
> 既然 A 阶段和 D 阶段都已经推进到位，就继续把治理同步真正推上 `main`，再把 shared root 占用文档和 live 入口收成最终 neutral。

**已完成事项**：
1. 使用新版 `git-safe-sync.ps1 -Action sync -Mode governance -OwnerThread Codex规则落地` 在 `main` 上完成一次治理同步。
2. 已创建并推送治理 checkpoint：
   - `2966daa5`
3. 当前阶段 20 的核心成果已经全部进入 `main`：
   - 第一波分支语义闸机
   - 第二波 shared root 最小闸机
   - A 阶段 runbook
   - D 阶段 `operating-model.md`
4. 当前正在做最后一轮文档回填：
   - `shared-root-branch-occupancy.md` 收成 `main + neutral`
   - `Sunset当前唯一状态说明_2026-03-17.md` 改写为“当前已恢复常态”
   - `20/tasks.md` 改为已完成状态

**关键决策**：
- 阶段 20 的“治理同步”已经不是待办，而是已有主干 checkpoint 的 live 事实。
- 现在剩下的是很小的一次 neutral 收尾提交，而不是新的治理主任务。

**恢复点 / 下一步**：
- 再做最后一次 tiny governance sync。
- 收尾后输出全线程唤醒清单与 superpower / skills 审视结论。
