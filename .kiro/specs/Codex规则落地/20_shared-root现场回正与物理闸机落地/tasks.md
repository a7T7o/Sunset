# 阶段任务：shared root 现场回正与物理闸机落地

## 阶段目标
- 停止 Sunset 再次落入“大规模 dirty -> 冻结 -> 清理 -> 再次 dirty”的循环。
- 先把 shared root 回正到用户认可的 clean `main`。
- 再把现有规则从“说明书”升级为“会拦人的物理闸机”。

## 当前状态
- **阶段状态**：已完成
- **工作性质**：A 阶段 Git 外科、B 阶段最小闸机、D 阶段模型与治理同步均已完成
- **现场前提**：shared root 已回到 `main + neutral`

## 已完成
- [x] 重新实核 live 现场：
  - `D:\Unity\Unity_learning\Sunset @ codex/npc-roam-phase2-003`
  - `HEAD = 2ecc2b753ea711557baca09432d0c7e3760cb3f7`
- [x] 确认 shared root 仍有 mixed dirty / untracked：
  - farm 两份 memory dirty
  - `2026.03.18线程并发讨论` 未跟踪目录
- [x] 确认 farm cleanroom worktree 仍真实存在
- [x] 通读并吸收 `NPC / spring-day1 / 农田交互修复V2 / 导航检测 / 遮挡检查` 回包中的并发现场信息
- [x] 完成第一轮问题定性：
  - 问题核心不是“并发不允许”
  - 而是“没有把规则变成物理闸机”
- [x] 评估 Gemini 建议：
  - 高层诊断可吸收
  - 具体 Git 命令列为待审核 runbook，不直接执行
- [x] 完成 `git-safe-sync.ps1` 第一波物理闸机落地：
  - 新增 `-OwnerThread`
  - `main` 禁止 `task` 模式
  - `task` 分支语义与线程身份不匹配即阻断
- [x] 同步更新当前活入口文档中的脚本调用口径：
  - `AGENTS.md`
  - `git-safety-baseline.md`
  - `基础规则与执行口径.md`
- [x] 发现并确认一个新的 live 风险：
  - 历史 farm cleanroom worktree 内的 `scripts/git-safe-sync.ps1` 仍是旧版副本
  - 这进一步证明 shared root 仍应作为治理脚本的唯一 live 来源，历史 worktree 只能视作例外现场
- [x] 执行 A 阶段 Git 外科：
  - 将治理脏改与 farm dirty 分组 park
  - 将 `codex/npc-roam-phase2-003` 从 `2ecc2b75` 回正到 `c81d1f99`
  - 使用 `--force-with-lease` 回正远端
  - 将 shared root 切回 `main`
- [x] 清理 Git worktree 挂载：
  - `git worktree list --porcelain` 当前只剩 shared root
- [x] 完成阶段 20 治理同步：
  - 使用新版 `git-safe-sync.ps1 -Mode governance -OwnerThread Codex规则落地`
  - 已在 `main` 上形成并推送治理 checkpoint `2966daa5`

## 待审核任务清单
### A. shared root 现场回正 runbook
- [x] 锁定当前 owner、blocker、blocked 线程清单
- [x] 复核 `2ecc2b75` 之前的目标回退点、远端状态与本地未推送风险
- [x] 形成用户可手工执行的 Git 外科 runbook：
  - 回退方式
  - staged/unstaged/untracked 处理方式
  - 错提交内容隔离方式
  - 强推风险提示
  - shared root 回 `main` 的最终验收
- [x] 明确哪些步骤必须由用户手工执行，哪些步骤后续可以交给脚本

### B. 物理闸机设计与落地方案
- [x] 读取并审计 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
- [x] 明确脚本需要新增的输入语义：
  - 线程身份 / owner 身份
  - 允许分支模式
  - `task` / `governance` / `ensure-branch` 的分支边界
- [x] 形成最小硬阻断清单，并落下第一波实现：
  - 分支语义不匹配即阻断
  - `main` 禁止 `-Mode task`
  - `task` 分支必须带 `codex/` 前缀
- [x] 落下第二波最小 shared root 闸机：
  - 若 shared root 占用文档声明 `is_neutral = false`，则 `task` 模式必须匹配 `owner_thread + current_branch`
  - shared root 上只要仍有未纳入白名单的 remaining dirty，`task` 模式直接阻断
  - shared root 从 `main` 执行 `ensure-branch` 前，occupancy 文档必须先回到 neutral
- [ ] 第二波扩展闸机待补：
  - shared root 自动 claim / release wrapper
  - dirty transfer 的自动隔离或自动归属辅助
- [ ] 评估是否需要独立新增：
  - branch guard skill
  - shared root lease 文件
  - preflight wrapper

### C. 旧阶段迁移与封板标记
- [x] 在 `09` 标记：第一轮闸门落地已完成，但 live 回正与物理闸机第二轮改由 `20` 接管
- [x] 在 `11` 标记：历史收口结论保留，但 live 回退后的 shared root 回正不再由 `11` 承接
- [x] 在 `12` 标记：工作区归位结论保留，但后续 live 修复不再由 `12` 承接
- [x] 更新根层 memory / 线程 memory，避免后续再把 `09/11/12` 误读为现行主线

### D. 恢复正常开发模型
- [x] 写出 shared root 恢复后的唯一口径：
  - `main-common + branch-task + checkpoint-first + merge-last`
- [x] 写出“可以并发的事 / 不可并发的事”
- [x] 写出 checkpoint 归还标准：
  - 提交
  - 推送
  - 退回 Edit Mode
  - 释放锁
  - 切回 `main`
  - 不留 unrelated dirty

## 当前裁定
- `20` 是当前新的治理主线。
- `09/11/12` 只保留历史作用，不再继续承接这轮 live 回正。
- 阶段 `20` 当前已完成；后续若继续补自动 claim / release wrapper 或 superpower 化 skills 约束，应另立后续阶段承接。

## 完成标准
- [x] shared root 真正回到 clean `main`
- [x] `git worktree list` 达到用户认可的最终结构
- [x] `20` 输出一份经审核可执行的现场回正 runbook
- [x] `20` 输出一份经审核可实施的物理闸机改造方案（第一波与第二波最小闸机已落地，扩展 wrapper 仍待定）
- [x] 旧阶段已全部写明“未解决问题已迁移至 20”，不再制造双入口
