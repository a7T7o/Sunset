# queue rollout matrix

## 目标
- 在不碰 Unity / MCP / Play Mode 的前提下，先把 shared root queue 的核心路径做成可重复演习的 Git 层闭环。
- 明确哪些场景已经实测，哪些仍只是协议层设计。

## 实测优先级

### P0
- `request-branch` 在 clean `main + neutral` 上可直接返回 `GRANTED`
- 第二、第三个线程在已有 grant 时会返回 `LOCKED_PLEASE_YIELD`
- `requeue-branch-request` 会撤销旧 ticket，并把线程重新放到队尾
- 已 `task-active` 的线程执行 `return-main` 后，会打印 `NEXT_IN_LINE_*`
- 治理线程执行 `wake-next` 后，队首线程再次 `request-branch` 会返回 `ALREADY_GRANTED`
- `cancel-branch-request` 能取消 waiting / granted，并在必要时释放 grant

### P1
- `wake-next` 在队列为空时返回 `NO_WAITING_REQUESTS`
- `wake-next` 在 shared root 不 clean / 不 neutral 时返回 `WAKE_BLOCKED`
- waiting 线程反复 `request-branch` 不会插队，也不会生成多张 open ticket

### P2
- 明确越序审批时的治理记录格式
- 验证多次 completed / cancelled 后是否需要定期裁剪 runtime 历史项

## 推荐演习角色
- 占锁者：`navigation -> codex/navigation-audit-001`
- 第一等待者：`npc -> codex/npc-roam-phase2-003`
- 重排者：`farm -> codex/farm-1.0.2-cleanroom001`
- 调度者：`Codex规则落地`

## 一句话口径
- rollout 先验证 Git 层调度闭环，Unity / MCP 独占调度放在下一轮更高风险验证里。

## 2026-03-19 实测结果
- 已实测通过：
  - `request-branch` 首次申请返回 `GRANTED`
  - 第二、第三线程在已有 grant 时返回 `LOCKED_PLEASE_YIELD`
  - `requeue-branch-request` 会撤销旧 ticket，并把线程重新放到队尾
  - `wake-next` 能把队首 `waiting` 提升为 `granted`
  - 被唤醒线程再次 `request-branch` 返回 `ALREADY_GRANTED`
  - `cancel-branch-request` 能取消 granted，并释放 grant
  - `wake-next` 在已有未消费 grant 时返回 `WAKE_BLOCKED`
  - 所有演习结束后，shared root 恢复到 `main + clean`
- 演习中暴露并已修补：
  - 切到旧任务分支后，仓库里的旧版 `git-safe-sync.ps1` 会带来旧输出或旧 queue 回写
  - 已新增 queue runtime 自愈逻辑；回到 `main` 后，新版脚本会按 `occupancy` 修正 stale open entry
- 本轮未覆盖：
  - Unity / MCP 层写态调度
  - 越序审批的治理记录格式
  - 长期运行后的 queue runtime 裁剪策略
