# 负例矩阵与 Rollout 清单（2026-03-19）

## 1. Rollout 顺序
1. 先上线执行层文档边界与迁移说明。
2. 再上线 `git-safe-sync.ps1` 的 active session runtime、持槽窗口提示、waiting 输出收紧。
3. 再上线治理协议修订：
   - waiting 线程不再写 tracked 回执
   - 恢复点优先进入 runtime queue 的 `CheckpointHint / QueueNote`
4. 最后再做实盘批次验证。

## 2. 负例矩阵

| 场景 | 预期结果 | 验证点 |
|------|------|------|
| shared root 已被占用，第二个线程 request | 返回 `LOCKED_PLEASE_YIELD`，且提示不要在 `main` 上写 tracked memory / 回执 | 输出字段与 queue runtime |
| waiting 线程想退出排队 | `cancel-branch-request` 成功，不污染 `main` | queue runtime 状态改为 `cancelled` |
| 队首线程被 wake | `wake-next` 输出 ticket、branch、checkpoint hint、hold window | 治理线程无需手工翻 tracked 回执 |
| grant 后 ensure-branch 成功 | 生成 ignored active session runtime，输出推荐持槽窗口 | active session runtime |
| return-main 后归还成功 | 输出实际持槽分钟数和下一位 hint | return-main 输出 |
| governance sync on main | 不再显示误导性的 task lease 阻断结论 | preflight 输出 |

## 3. 实盘验收清单
- `request-branch` 在 dirty / occupied / queue-head / grant-ready 四种情形下输出符合预期。
- waiting 线程仅保留最小聊天回执，不再在 `main` 写 tracked memory。
- `ensure-branch` 成功后 active session runtime 正常生成。
- `return-main` 正常打印实际持槽时长与下一位信息。
- 现有安全基线不退化：
  - 无租约不能 ensure-branch
  - main 不能 task sync
  - 错线程不能 return-main

## 4. 一句话口径
- Rollout 的目标不是再做一轮“看起来更复杂”的治理，而是验证执行层重构以后，shared root 是否真的更快、更干净、更可轮转。
