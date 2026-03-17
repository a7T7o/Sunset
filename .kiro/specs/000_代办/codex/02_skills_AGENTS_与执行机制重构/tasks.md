# 阶段任务：skills、AGENTS 与执行机制重构

## 目标
- 修复治理型 skills 的错码与失真。
- 让规范不只写在文档里，而是真正被工具与 skill 约束执行。
- 明确 AGENTS、skills、工作区规则三者的责任边界。

## 待办
- [x] 审核并重写 `skills-governor`。
- [x] 审核 `global-learnings`，确认当前可读、暂不重写。
- [x] 重新检查 `sunset-workspace-router`、`sunset-unity-validation-loop` 是否仍与最新规则一致。
- [x] 审核当前 `AGENTS.md` 是否还存在旧口径残留。
- [x] 清理 `agent-a2df3da0` 一类历史 worktree 噪声在规则、AGENTS 与治理文档中的残留口径。
- [x] 设计并落地 `sunset-thread-wakeup-coordinator`。
- [x] 修复 `sunset-thread-wakeup-coordinator/agents/openai.yaml` 的编码与隐式调用元数据。
- [x] 设计并落地 `sunset-lock-steward`。
- [x] 设计并落地 `sunset-doc-encoding-auditor`。
- [x] 设计并落地 `sunset-release-snapshot`。
- [ ] 评估是否需要把“每次对话强制先走治理型 skill”写成更强约束。
- [ ] 评估 `git-safe-sync.ps1` 是否需要在治理模式下更细的白名单能力。

## 完成标准
- 关键治理型 skills 恢复可读、可依赖。
- 至少新增一项真正能减少人工转述与手工遵守成本的 Sunset 治理 skill。
- 与线程唤醒、物理锁、文档编码有关的关键治理 skill 均有可读、可触发、可继续迭代的健康版本。
- 与阶段快照和收口摘要有关的治理 skill 也有统一入口，不再只靠手工拼接状态汇报。
