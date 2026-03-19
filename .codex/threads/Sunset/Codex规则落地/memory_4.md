# Codex规则落地线程记忆补卷 4

## 会话 40 - 2026-03-19（修复后重测批次生成）
**用户目标**：
> 轻微走审核路径后继续推进，不只停留在“事故已修好”的口头结论，而是直接开始下一轮可执行的恢复开发调度内容。

**已完成事项**：
1. 轻审核 `Gemini` 新建议后裁定：
   - “P1 bug 已修复、可以重启并发准入”这一方向可采纳
   - 但“沿用旧回收模式直接群发 4 线程”还差一道运营层防呆
2. 识别出新的 live 风险：
   - 等待态线程如果在 `main` 上写 tracked 回收卡或 `memory_0.md`，会再次把 shared root 写脏，直接污染 queue 实盘
3. 因此新建修复后重测批次：
   - 根层批次文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-19_批次分发_03_queue-aware业务准入_02_修复后重测.md`
   - 子目录：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_02_修复后重测\可分发Prompt\`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_02_修复后重测\线程回收\`
4. 为 `导航检查 / NPC / 农田交互修复V2 / 遮挡检查` 生成新一轮专属 prompt，并把固定收件箱改成“治理线程代填”模式。
5. 把这条新增运营规则同步写回：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\治理线程批次分发与回执规范.md`
   - `C:\Users\aTo\.codex\skills\sunset-governance-dispatch-protocol\SKILL.md`

**关键决策**：
- 这轮先做“修复后重测”，不是直接把系统宣传成已经可无限并发写入。
- 线程若拿到 `LOCKED_PLEASE_YIELD`，本轮只允许最小聊天回执，不允许在 `main` 上写 tracked 文件。

**恢复点 / 下一步**：
- 把本轮批次文件同步到 `main`。
- 然后就可以把根层批次入口发给 4 条线程，开始二次并发实盘。
