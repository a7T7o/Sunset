# 2026-03-21 dirty 分级与 takeover 边界设计稿

## 1. 这份设计稿解决什么问题
- 把任务 `15` 从“值得讨论”推进到“有明确边界的正式草案”。
- 回答三件事：
  - 哪些 dirty 只是噪音或治理尾项，不该一刀切当成事故。
  - 哪些 dirty 只允许在线程自己的任务分支里继续，不允许跨线程乱接。
  - 哪些 dirty 仍然必须视为硬阻断，绝不能因为“看起来还能跑”就放行。

## 2. 当前裁定先说在前面
- 这份文档现在是设计稿，不是已全面生效的新制度。
- 在脚本、模板和 runbook 全部补齐前，默认硬闸门仍然保持不变：
  - `main` 必须 clean
  - shared root 必须 `neutral`
  - `task` 模式仍不允许带着未归属的 remaining dirty 继续写
- 这次草案真正准备放宽的，不是“shared root 可以带脏跑”，而是：
  - 把 dirty 分出层级
  - 先允许少量、可归属、可回滚的 dirty 只在受控场景内继续

## 3. 分级模型

### D0：噪音 / ignored runtime
- 定义：
  - gitignored Draft
  - ignored runtime JSON
  - 已知本地工具噪音
  - 不进入仓库历史的本机残留
- 特征：
  - 不承载业务交付语义
  - 不影响下一位线程理解 `main`
  - 不需要 takeover
- 裁定：
  - 不作为 shared root 阻断项
  - 继续留在 ignored 层，不升级为 tracked 治理动作

### D1：治理型可收口 dirty
- 定义：
  - 当前治理线程自己的 tracked 文档、tasks、memory
  - 明确 owner、明确路径、且不改变运行时语义的治理改动
- 典型例子：
  - `Codex规则落地` 的 memory 补记
  - `共享根执行模型与吞吐重构` 的专题分析文档
  - `scripts/git-safe-sync.ps1` 这类治理工具修复
- 特征：
  - 只影响规则、说明、审计与工具链
  - 可以通过显式白名单 `governance sync` 收口
  - 不应该混入业务线程 checkpoint
- 裁定：
  - 允许在 `main` 上通过显式 `IncludePaths` 收口
  - 不得借这个名义把业务 dirty 伪装成治理 dirty

### D2：任务分支内可继续的 owner-dirty
- 定义：
  - 位于任务分支内
  - owner 明确
  - scope 明确
  - rollback 基线明确
  - 不命中热文件 / 禁止域
  - 当前仍由同一线程继续推进
- 典型例子：
  - 某功能线程在自己的 `codex/...` 分支上，完成了一个可回滚的半成品，但还没到最终 checkpoint
  - 该半成品已有明确 `changed_paths`、恢复点、下一步和风险说明
- 必要前提：
  - 当前分支不是 `main`
  - dirty 全部落在本线程白名单业务范围内
  - 已形成最小 checkpoint 计划或最小 handoff 说明
  - 没有把 unrelated dirty 混进来
- 裁定：
  - 允许同一线程在同一任务分支上继续
  - 不允许把它带回 shared root `main`
  - 当前阶段不批准“原样交给别的线程直接接未提交 dirty”

### D3：禁止放行 dirty
- 定义：
  - 任何一类一旦放行就会造成 ownership 混乱、运行时断链或回滚失真
- 典型例子：
  - `main` 上的业务 dirty
  - shared root 上未归属的 mixed dirty
  - `Primary.unity`
  - `GameInputManager.cs`
  - 共享 Prefab / ScriptableObject / Sprite meta 的未验收改动
  - 仍需 Unity / MCP 连续验证的 Scene / Inspector 脏改
  - 命中热文件但未走锁的改动
  - remaining dirty 里混有本线程白名单外文件
- 裁定：
  - 继续作为硬阻断
  - 不进入放宽机制
  - 必须先清尾、收口、隔离或显式回滚

## 4. takeover 边界

### 4.1 当前阶段批准的 takeover 只有一种
- 同一线程、同一任务分支、同一 scope 的继续推进。
- 也就是说：
  - `NPC` 自己继续 `NPC`
  - `farm` 自己继续 `farm`
  - 允许 D2 在该线程自己的分支里短期存在

### 4.2 当前阶段明确不批准的 takeover
- 不批准“线程 A 留一堆未提交 dirty，线程 B 直接接着写”。
- 不批准“shared root 上带着任务 dirty 直接换人”。
- 不批准“没有 checkpoint、没有 baseline、没有 owner 说明就宣称可接手”。

### 4.3 将来如要开放跨线程 takeover，至少要满足
- 先有 branch checkpoint，而不是直接交接 raw dirty。
- 必须补齐以下字段：
  - `owner_thread`
  - `target_branch`
  - `baseline_head`
  - `changed_paths`
  - `dirty_level`
  - `validation_state`
  - `hotfile_state`
  - `next_step`
  - `rollback_anchor`
- 并且：
  - 禁止域为空
  - related memory 已更新
  - 下一位线程明确承接

## 5. 清扫推送机制的建议口径

### 5.1 D1 的收口方式
- 用 `sync -Mode governance`
- 只带显式 `IncludePaths`
- 目标是把治理尾项尽快推上 `main`

### 5.2 D2 的收口方式
- 用 `sync -Mode task`
- 只带当前任务 scope 和显式 `IncludePaths`
- 目标是形成一个最小 checkpoint 提交，而不是长期留 raw dirty

### 5.3 当前不允许的“清扫推送”
- 不允许把 unrelated dirty 顺手扫进同一个提交
- 不允许为了“先推上去再说”绕开白名单
- 不允许用 docs-only 提交替代业务 checkpoint
- 不允许把 shared root 上的 D3 dirty 包装成可推送尾项

## 6. 对 `git-safe-sync.ps1` 的后续要求
- 先补报告能力，再考虑放宽能力。
- 建议下一阶段脚本至少补这几项：
  - 在 preflight / sync 输出里显式给每类 dirty 打级别
  - 报出 `owner / scope / reason`
  - 将“remaining dirty”从单一阻断消息升级为“分级阻断消息”
- 但当前阶段仍不建议脚本直接自动放宽：
  - shared root `main`
  - 跨线程 raw dirty
  - 禁止域文件

## 7. 推荐 rollout 顺序
1. 先保持现有硬闸门不变，只新增“分级报告”。
2. 再允许 D1 治理 dirty 更稳定地收口。
3. 再允许 D2 作为“同线程、同分支”的继续态被明确承认。
4. 最后才评估是否要设计“跨线程 checkpoint takeover”。
- 不把“跨线程直接接 raw dirty”列入当前 rollout。

## 8. 对用户原问题的正式回答
- “能跑且可接手的 dirty 是否允许直接推进？”
- 当前我的正式答案是：
  - `可以部分推进，但只限 D1 和 D2 的受控场景。`
  - `不允许把 shared root 重新变成公共脏工作台。`
  - `不允许跨线程直接接 raw dirty。`
- 也就是说，这次设计的目标是“提高吞吐”，不是“撤销安全基线”。

## 9. 当前建议落地到任务单的结论
- 任务 `15` 现在应视为：已进入正式设计阶段。
- 当前阶段可立即执行的治理动作是：
  - 保持 `main clean + shared root neutral` 为默认硬规则
  - 把 D0 / D1 / D2 / D3 分级写进后续脚本设计与回执模板
  - 明确“同线程 continuation 可带 D2，跨线程 raw dirty 不批准”

## 10. 一句话结论
- 这次 `dirty` 机制不是要把 shared root 放宽成“谁都能带脏跑”，而是要把 dirty 从“全都算事故”升级成“先分级、再裁定、只对极少数受控场景放宽”。
