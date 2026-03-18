# shared-root-queue

## 文件身份
- 本文件用于记录 shared root `D:\Unity\Unity_learning\Sunset` 的排队 / 等待 / 唤醒状态。
- 它不替代 `shared-root-branch-occupancy.md`。
- `occupancy` 仍是 shared root 当前 live 分支、租约状态与中性判断的唯一事实源。
- 真正的 runtime 机器状态写入：
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\active\shared-root-queue.lock.json`
- 本文件只回答：
  - 哪些线程正在等待 shared root 的下一次准入机会
  - 哪个 ticket 正在被服务
  - 线程挂起前最后声明的 checkpoint / note 是什么

## 当前口径
- 业务线程如需进入真实写入，不应直接假设自己现在就能拿到租约。
- queue-aware 流程的第一入口是：
  - `git-safe-sync.ps1 -Action request-branch -OwnerThread <线程名> -BranchName codex/...`
- 当 shared root 暂时不能发租约时：
  - 脚本返回 `LOCKED_PLEASE_YIELD`
  - 线程必须保存当前上下文到自己的 `memory_0.md`
  - 线程进入等待态，而不是继续乱试 `ensure-branch`
- 当 shared root 回到 `main + neutral` 且无未消费租约时：
  - 治理线程读取本文件的队首 `waiting` 条目
  - 决定下一次唤醒或发放准入

## Runtime State Template
<!-- queue-state:begin -->
```json
{
  "version": 1,
  "queue_enabled": true,
  "policy": "request-branch-yield-then-grant",
  "next_ticket": 1,
  "current_serving_ticket": null,
  "last_updated": "2026-03-19 00:00:00 +08:00",
  "entries": []
}
```
<!-- queue-state:end -->

## 状态说明
- `waiting`：已登记排队，尚未拿到租约。
- `granted`：已拿到租约，但尚未切入任务分支。
- `task-active`：已切入任务分支并占用 shared root。
- `completed`：本轮 checkpoint 已完成，并已 `return-main` 归还 shared root。
- `cancelled`：本轮请求撤销，不再参与排队。

## 一句话口径
- `occupancy` 管 live 现场；runtime queue json 管等待与唤醒；本文件提供人类可读说明与模板。
