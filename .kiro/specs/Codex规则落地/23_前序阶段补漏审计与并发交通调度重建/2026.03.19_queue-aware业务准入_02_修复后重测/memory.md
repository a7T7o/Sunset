# 2026.03.19 queue-aware业务准入 02 修复后重测 memory

## 2026-03-19：修复后重测批次已生成
- 当前主线目标：在 shared root 已回到 `main + clean + neutral` 后，对修复过的 queue-aware Git 准入做一次二次并发重测。
- 本轮子任务：生成本轮根层批次分发文件、4 份专属 prompt 与治理线程代填的固定收件卡模板。
- 关键新增约束：
  - 本轮等待态线程不得直接写 tracked 回收卡，也不得在 `main` 上更新 tracked `memory_0.md`。
  - 原因是任何等待态写入都会把 shared root 写脏，反向干扰 queue 实盘。
  - 因此本轮统一改为：线程只做最小聊天回执，治理线程根据回执回填固定收件箱。
- 本轮对象：
  - `导航检查`
  - `NPC`
  - `农田交互修复V2`
  - `遮挡检查`
- 本轮目标不是推进真实业务实现，而是验证：
  - stable launcher 的 `request-branch`
  - queue 的 `GRANTED / LOCKED_PLEASE_YIELD`
  - `ensure-branch`
  - `return-main`
  这一整条闭环在修复后是否稳定。
- 恢复点 / 下一步：
  1. 等 4 条线程按专属 prompt 返回最小聊天回执。
  2. 治理线程再统一回填 `线程回收` 固定卡。
  3. 基于结果裁定是否进入真正的下一轮恢复开发准入。
