# shared-root-queue

## 文件身份
- 本文件用于记录 shared root `D:\Unity\Unity_learning\Sunset` 的排队 / 等待 / 唤醒状态。
- 它不替代 `shared-root-branch-occupancy.md`。
- `occupancy` 仍是 shared root 当前 live 分支、租约状态与中性判断的唯一事实源。
- shared root 的 live 准入命令默认入口固定为：
  - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1`
  - 不再默认直接调用仓库内 `scripts\git-safe-sync.ps1`
- 真正的 runtime 机器状态写入：
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\active\shared-root-queue.lock.json`
- 新版 `git-safe-sync.ps1` 会在读取 runtime queue 时，对照 `occupancy` 做一次自愈：
  - 如果旧分支脚本留下了陈旧的 `task-active / granted`
  - 回到 `main` 后，新脚本会把 runtime queue 修回与 live occupancy 一致的状态
- 本文件只回答：
  - 哪些线程正在等待 shared root 的下一次准入机会
  - 哪个 ticket 正在被服务
  - 线程挂起前最后声明的 checkpoint / note 是什么

## 当前口径
- 业务线程如需进入真实写入，不应直接假设自己现在就能拿到租约。
- queue-aware 流程的第一入口是：
  - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action request-branch -OwnerThread <线程名> -BranchName codex/...`
- 业务线程如需退出等待或重新排到队尾，使用：
  - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action cancel-branch-request -OwnerThread <线程名> -BranchName codex/...`
  - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action requeue-branch-request -OwnerThread <线程名> -BranchName codex/...`
- 当 shared root 暂时不能发租约时：
  - 脚本返回 `LOCKED_PLEASE_YIELD`
  - 线程进入等待态，而不是继续乱试 `ensure-branch`
  - 恢复点优先放进 `CheckpointHint / QueueNote / 最小聊天回执 / queue runtime`
- 当 shared root 回到 `main + neutral` 且无未消费租约时：
  - `return-main` 会打印当前 `NEXT_IN_LINE`
  - 治理线程可执行：
    - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action wake-next -OwnerThread Codex规则落地`
  - 将队首 `waiting` 条目提升为 `granted`

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

## 2026-03-19 执行层补充口径
- waiting 线程不再默认在 `main` 上写 tracked `memory_0.md` 或固定回执卡。
- `request-branch` 返回 `LOCKED_PLEASE_YIELD` 时，恢复点优先进入：
  - `CheckpointHint`
  - `QueueNote`
  - `.kiro/locks/active/shared-root-queue.lock.json`
- shared root 一旦进入 `task-active`，脚本会额外写入 ignored runtime：
  - `.kiro/locks/active/shared-root-active-session.lock.json`
- 该 runtime 记录：
  - 当前占用线程
  - 目标分支
  - checkpoint hint / note
  - 推荐持槽分钟数
- `return-main` 会打印实际持槽分钟数与下一位 waiting 条目的 hint；治理线程应优先读取这些字段，而不是重新手工翻 tracked 回执。

## 状态说明
- `waiting`：已登记排队，尚未拿到租约。
- `granted`：已拿到租约，但尚未切入任务分支。
- `task-active`：已切入任务分支并占用 shared root。
- `completed`：本轮 checkpoint 已完成，并已 `return-main` 归还 shared root。
- `cancelled`：本轮请求撤销，不再参与排队。

## 标准命令口径
- 首次申请：
  - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action request-branch -OwnerThread <线程名> -BranchName codex/...`
- 放弃当前 waiting / granted：
  - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action cancel-branch-request -OwnerThread <线程名> -BranchName codex/...`
- 重新排到队尾：
  - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action requeue-branch-request -OwnerThread <线程名> -BranchName codex/...`
- 治理线程唤醒队首：
  - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action wake-next -OwnerThread Codex规则落地`
- 当前版本不是自动守护进程：
  - `wake-next` 仍由治理线程显式触发
  - `return-main` 负责给出下一位提示，但不会替代治理裁定

## 一句话口径
- `occupancy` 管 live 现场；runtime queue json 管等待与唤醒；本文件提供人类可读说明与模板。
