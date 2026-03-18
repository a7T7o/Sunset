# 阶段任务：skills、AGENTS 与执行机制重构

## 阶段目标
- 让规则不只停留在文档层，而是真正进入 skills 与 AGENTS 的执行约束层。

## 已完成
- [x] 审核并重构 `skills-governor`
- [x] 审核 `sunset-workspace-router`
- [x] 建立并重构 `sunset-startup-guard`
- [x] 补齐 `sunset-startup-guard/references/checklist.md`
- [x] 更新 Sunset 项目 `AGENTS.md`
- [x] 审核并落地：
  - `sunset-thread-wakeup-coordinator`
  - `sunset-lock-steward`
  - `sunset-doc-encoding-auditor`
  - `sunset-release-snapshot`
- [x] 对“是否需要每次 Sunset 实质性任务都先走治理型 skill”做最终裁定：
  - 需要
  - 当前以 `sunset-startup-guard + AGENTS.md` 落地
- [x] 对 `git-safe-sync.ps1` governance 白名单能力做最终裁定：
  - 继续保持显式 `-IncludePaths` / `-ScopeRoots`
  - 不扩大默认治理白名单

## 当前裁定
- 本阶段已完成。

## 完成标准
- [x] 关键治理 skills 可读可用
- [x] AGENTS 与启动闸门已形成明确约束
- [x] 关键执行机制已有最终口径
