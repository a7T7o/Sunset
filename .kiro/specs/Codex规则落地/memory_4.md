# Codex规则落地 memory_4

## 2026-03-19：修复后重测批次已生成
- 当前主线目标：从“把事故修好”过渡到“验证修好的系统能否稳定用于下一轮恢复开发”。
- 本轮子任务：在 `main @ 659109c1` clean 基线上，生成 `queue-aware业务准入_02_修复后重测` 批次。
- 关键决策：
  - 采纳 `Gemini` 关于“可以重启下一轮并发准入”的大方向。
  - 不采纳“直接照旧让线程写 tracked 回收卡”的默认执行方式。
  - 新补的硬约束是：
    - 等待态线程不得在 `main` 上写 tracked 文件
    - 固定收件箱仍保留，但由治理线程根据最小聊天回执统一回填
  - 这条约束已同步写入：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\治理线程批次分发与回执规范.md`
    - `C:\Users\aTo\.codex\skills\sunset-governance-dispatch-protocol\SKILL.md`
- 恢复点 / 下一步：
  1. 等 4 条线程回执本轮重测结果。
  2. 若本轮通过，再进入真正的恢复开发调度。
