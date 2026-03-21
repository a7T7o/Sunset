# 共享根执行模型与吞吐重构 - 任务清单

- [x] 1. 固化新工作区边界
  说明：明确哪些职责继续留在 `Codex规则落地`，哪些职责迁移到本工作区，避免再次双源漂移。
- [x] 2. 盘点当前执行层的真实瓶颈
  说明：基于既有实盘记录，整理 shared root 长时间持槽、tracked 回执污染、等待态落盘位置错误等证据。
- [x] 3. 定义线程生命周期
  说明：形成“只读准备 -> 申请槽位 -> 等待挂起 -> 最小写事务 -> return-main -> 事后落盘”的统一模型。
- [x] 4. 定义持槽时间边界
  说明：区分哪些动作允许发生在 shared root 持槽期，哪些动作必须放到持槽前或释放后。
- [x] 5. 重构运行态承载面
  说明：把 queue / wake / cancel / requeue 这类高频状态尽量迁移到 ignored runtime，而不是 tracked Markdown。
- [x] 6. 重构 stable launcher / canonical script 协议
  说明：在不破坏安全闸机的前提下，补齐执行层需要的新状态码、等待语义和唤醒语义。
- [x] 7. 收紧治理线程分发协议
  说明：让治理线程回到“批次入口 + 事后审计”，不再承担高频人工运行态消息总线。
- [x] 8. 制定 rollout 与负例矩阵
  说明：明确如何验证一人持槽、多线程只读准备、等待线程不污染 `main`、以及 wake / cancel / requeue 的边界行为。
- [x] 9. 补前序阶段收口映射
  说明：回看 `Codex规则落地` 里会误导后续读者的旧摘要、旧入口、旧“当前主线”描述，并逐步写明它们已被本工作区接管的问题域。
- [x] 10. 补 Waiting Draft 沙盒与 post-return 证据策略
  说明：为等待态和 `return-main` 后但队列未清空的线程提供 gitignored 草稿承载面，并明确 tracked 证据何时允许最小落盘。
- [x] 11. 补旧分支 Draft 忽略兜底与 smoke-test 恢复闭环
  说明：为仍停留在旧规则基线上的 continuation branch 增加 repo-local Draft 忽略兜底，避免 waiting 线程的 `.codex/drafts/**` 反向阻断持槽线程 `return-main`，并据此完成 `smoke-test_01` 的 shared root 恢复与回收闭环。
- [x] 12. 完成 `smoke-test_01` 四线程真实闭环验收
  说明：按队列顺序实际跑通 `导航检查 -> NPC -> 农田交互修复V2 -> 遮挡检查` 的 `wake-next / ALREADY_GRANTED / ensure-branch / return-main` 续跑链路，并完成最终回收与基线回正。
- [x] 13. 固化 `carrier-ready / main-ready` 事故补洞口径
  说明：把 NPC 主场景断链事故中暴露出的验收缺口写回执行模型，明确后续不能再只凭 branch 收口就宣称生产场景已恢复。
- [ ] 14. 将 `main-ready` 验收并入真实业务批次模板与回收卡
  说明：后续持续把“是否已真正落入 `main`”作为治理必答项，而不再只收 `changed_paths / sync / return-main`。
- [ ] 15. 设计 `dirty` 分级与清扫推送机制
  说明：把“能跑且可接手的 dirty 是否允许直接推进”做成正式讨论稿，明确哪些可容忍、哪些绝对禁止、哪些必须先满足 takeover 条件。
  当前进展：已形成 `02_专题分析/2026-03-21/dirty分级与takeover边界设计稿.md`，并已在 `scripts/git-safe-sync.ps1` 落下首轮 dirty 分级报告层（`DirtyLevel / OwnerHint / PolicyHint`）；当前结论仍为“先分级、先报告、默认硬闸门不撤，且暂不批准跨线程 raw dirty takeover”。
- [x] 16. 固化重度 MCP 场景搭建线程执行方案
  说明：明确这类线程的单线程独占、验证场景优先、`worktree` 边界和交付方式，避免后续把它误塞进普通 shared root 短事务模型。
- [x] 17. 整理工作区目录并建立总览待办
  说明：把根目录收束为“正文 + 导航 + 批次 + 专题分析”，并生成一份覆盖 `NPC / 农田 / 导航 / 遮挡 / spring-day1 / dirty / 场景线程` 的统一待办总表。
- [x] 18. 生成真实业务开发批次 04（补入 `main-ready`）
  说明：真实业务 prompt 与回收口径显式回答 `carrier-ready` 和 `main-ready`，并把 `导航 / 遮挡 / spring-day1` 的排位和类型一起纳入波次计划。
- [x] 19. 执行批次 04 的收件、裁定与下一波切换
  说明：基于 `NPC / 农田` 第一波回执完成裁定，并继续切到 `导航 / 遮挡`，同时保持 `spring-day1` 的独立集成波次口径。
- [x] 20. 修复 shared root occupancy 收口状态机在异常下的鲁棒性
  说明：针对 2026-03-20 `NPC batch04` 中暴露出的“`ensure-branch` 半成功后 occupancy 未完整落为 `task-active`，进而让 `sync / return-main` 尾部误炸”问题，补齐 `git-safe-sync.ps1` 的旧 runtime 字段兼容、active session 幂等修复与异常后的安全收口。
