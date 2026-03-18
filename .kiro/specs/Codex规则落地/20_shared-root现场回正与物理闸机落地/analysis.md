# 阶段分析：shared root 现场回正与物理闸机落地

## 1. 立项原因
- `09/11/12` 在文档层分别完成过：
  - 启动闸门第一轮落地
  - `main-branch-only` 回归与 `worktree` 退役
  - 治理工作区归位
- 但 `2026-03-18` 的 live 现场证明，这些阶段的结论没有持续转化为“会拦人的物理约束”。
- 结果是 shared root 再次发生了：
  - 非 owner 占用未归还
  - 错分支继续写入
  - unrelated dirty 混入
  - 旧文档口径继续被沿用

## 2. 当前 live 事实
- shared root 当前真实现场不是 `main`，而是：
  - `D:\Unity\Unity_learning\Sunset @ codex/npc-roam-phase2-003 @ 2ecc2b753ea711557baca09432d0c7e3760cb3f7`
- 当前 shared root 仍可见的 blocker：
  - farm 两份 memory dirty
  - `2026.03.18线程并发讨论` 未跟踪目录
- `git worktree list --porcelain` 表明 farm cleanroom worktree 仍真实存在。
- 用户已人工冻结新的实质性写入；在阶段 20 完成前，不再允许把 shared root 当作可继续开发的正常入口。

## 3. 事故最短责任链
1. NPC 把 shared root 切到自己的 `codex/npc-roam-phase2-003` 后未归还。
2. 导航检测继续在这条 NPC 分支上提交了 `2ecc2b75`。
3. farm 在 shared root 留下 unrelated dirty。
4. `spring-day1` 与其他线程因此失去安全重入条件。

## 4. 问题本质
- 问题不在“并发”这个目标本身。
- 问题在于目前的 rules / AGENTS / skills / occupancy 文档，主要还是说明书，不是闸机。
- 具体表现为：
  - 分支不匹配时，没有硬阻断 `commit/sync`
  - shared root 被占用后，没有强制归还机制
  - `main` 上缺少 task-mode 禁写闸门
  - `worktree` 退役没有持续验收
  - Play Mode 离场、MCP 单实例、shared root 单写者，这三层约束仍未真正并成统一执行面

## 5. 对 Gemini 建议的吸收与保留意见
### 可以吸收的部分
- Gemini 对问题本质的定性是成立的：
  - “Markdown 治国”不够
  - 真正缺的是代码级、会 `exit 1` 的强制闸机
- Gemini 对目标模型的理解也与用户诉求一致：
  - `main-common + branch-task + checkpoint-first + merge-last`

### 不能直接无脑执行的部分
- Gemini 给出的 `reset --soft / stash / push --force / checkout main` 是候选手术路径，但当前阶段不能把它视为“已批准执行的唯一方案”。
- 原因不是它一定错，而是它涉及：
  - 改写已推送分支历史
  - 暂存 mixed dirty / untracked 的方式选择
  - 远端与本地分支状态确认
  - 错提交流水的最终归档方式
- 这些都必须在阶段 20 里形成一份经用户审核的“人工执行版 runbook”后，才能进入实操。

## 6. 阶段 20 的目标
- 先把 shared root 从混线现场回正为 clean `main`
- 再把“规则存在”升级成“规则不满足就直接停”
- 最终恢复到用户要的开发模型：
  - shared root 常驻 `main`
  - 业务线程只在自己的 `codex/...` 分支真实写入
  - 读、预检、设计可以并发
  - 真写入必须单写者串行，到 checkpoint 就归还

## 7. 本阶段边界
- 当前阶段以文档、设计、审计、执行清单为主。
- 本阶段不直接执行 Git 手术；先形成：
  - 现场回正 runbook
  - `git-safe-sync.ps1` / preflight 的物理闸机改造方案
  - 旧阶段迁移与封板标记

## 8. 完成判据
- shared root 回到 clean `main`
- `git worktree list` 满足用户认可的最终结构
- 新增一套物理闸机，至少覆盖：
  - 分支语义不匹配即阻断
  - `main` 禁止 task-mode 写入
  - shared root owner/lease 未满足即阻断
  - Play Mode 未退回 Edit Mode 不允许交还
- `09/11/12` 不再被误读为“当前仍在执行中的 live 阶段”

## 9. 最终执行结果（2026-03-18）
- A 阶段 Git 外科已经执行并完成：
  - `codex/npc-roam-phase2-003` 已从错位提交 `2ecc2b75` 回正到 `c81d1f99`
  - 远端回正使用了 `push --force-with-lease`
  - shared root 已切回 `main`
- 阶段 20 的治理同步已经完成并推上 `main`
- 当前 live Git 口径是：
  - `D:\Unity\Unity_learning\Sunset @ main`
  - `git status --short --branch` clean
  - `git stash list` empty
  - `git worktree list --porcelain` 只剩 shared root

## 10. superpowers 复审结论
- 这轮已重新审查 `obra/superpowers` 的 Codex 适配入口，证据包括：
  - `D:\Temp\superpowers-vet\.codex\INSTALL.md`
  - `D:\Temp\superpowers-vet\skills\using-superpowers\SKILL.md`
  - `D:\Temp\superpowers-vet\skills\using-git-worktrees\SKILL.md`
- 可吸收部分：
  - “先过技能闸门再行动”的纪律
  - 通过原生 skill discovery 让技能在新会话里自动可见
- 不可原样接管部分：
  - `using-git-worktrees` 把 worktree 当默认 feature isolation，这与 Sunset 的 `worktree = exception-only` 正面冲突
  - `using-superpowers` 假定平台有统一 Skill 工具和它自己的强制技能总入口，不适合作为 Sunset 当前环境的原样接管包
- 最终裁定：
  - `obra/superpowers` 继续保持 `rejected-as-is`
  - Sunset 采用“本地化吸收”而不是“原版整包安装”
  - 已把本地核心闸门 skill 暴露到 `C:\Users\aTo\.agents\skills\`：
    - `skills-governor`
    - `skill-vetter`
    - `sunset-startup-guard`
  - 这意味着后续新会话重启后，可以同时依赖：
    - repo / 全局 AGENTS 的显式规则
    - 原生 skill discovery 对核心闸门技能的发现

## 11. 剩余唯一物理尾巴
- `Sunset_external_archives` 与 `Sunset_backups` 当前都不存在，不构成尾巴
- 当前唯一剩余物理尾巴是：
  - `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
- 它已经不再是 Git worktree，只是一个空目录残壳
- 当前尝试物理删除时仍被外部进程占用，因此不在本阶段强删；应在占用释放后直接删空目录即可
