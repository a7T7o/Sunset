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
  说明：明确如何验证一人持槽、多人只读准备、等待线程不污染 `main`、以及 wake / cancel / requeue 的边界行为。

- [x] 9. 补前序阶段收口映射
  说明：回看 `Codex规则落地` 里会误导后续读者的旧摘要、旧入口、旧“当前主线”描述，并逐步写明它们已被本工作区接管的问题域。

- [x] 10. 补 Waiting Draft 沙盒与 post-return 证据策略
  说明：为等待态和 `return-main` 后但队列未清空的线程提供 gitignored 草稿承载面，并明确 tracked 证据何时允许最小落盘。
- [x] 11. 补旧分支 Draft 忽略兜底与 smoke-test 恢复闭环
  说明：为仍停留在旧规则基线上的 continuation branch 增加 repo-local Draft 忽略兜底，避免 waiting 线程的 `.codex/drafts/**` 反向阻断持槽线程 `return-main`；并据此完成 `smoke-test_01` 的 shared root 恢复与回收闭环。
- [x] 12. 完成 `smoke-test_01` 四线程真实闭环验收
  说明：按队列顺序实际跑通 `导航检查 -> NPC -> 农田交互修复V2 -> 遮挡检查` 的 `wake-next / ALREADY_GRANTED / ensure-branch / return-main` 续跑链路，并完成最终回收与基线回正。
