# 共享根并发交通调度协议 v1

## 1. 目标
- 把 Sunset 从“只有闸机、没有调度”的状态，推进到“可以等待、可以排队、可以唤醒”的状态。
- 维持 shared root 的单写者安全前提，同时允许业务线程在逻辑层并发准备。

## 1.1 稳定入口
- shared root 的 live Git 调度命令默认不再直接调用仓库内：
  - `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
- 新的默认入口固定为：
  - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1`
- 它会始终从 `main:scripts/git-safe-sync.ps1` 取 canonical 版本再执行，所以：
  - 当前 shared root 即使临时切到了旧任务分支
  - live 调度命令也不会跟着吃到旧分支自带的旧脚本
- 唯一例外：
  - 治理线程正在改 `scripts/git-safe-sync.ps1` 本体并需要验证 working tree 版本
  - 此时可显式使用仓库内脚本，或使用 launcher 但带 `-SourceRef HEAD`

## 2. 三层分工

### 2.1 shared root occupancy
- 文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md`
- 职责：
  - 记录当前 live 分支
  - 记录是否 `main + neutral`
  - 记录当前 grant / task-active 的实际占用状态

### 2.2 shared root queue
- 文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-queue.md`
- 职责：
  - 记录谁在等待
  - 记录等待顺序
  - 记录挂起前的 checkpoint hint / note
  - 记录当前被服务的 ticket
  - 在新版脚本读取时，对照 `occupancy` 做 runtime 自愈，修补旧分支脚本留下的陈旧 open entry

### 2.3 业务线程 memory
- 文件：
  - 各线程自己的 `memory_0.md`
- 职责：
  - 保存挂起前的真实恢复点
  - 说明当前已经准备到哪一步
  - 在 `LOCKED_PLEASE_YIELD` 后进入等待态

## 3. 标准状态机

### 3.1 shared root 写入槽位
- `neutral`：
  - shared root 在 `main + neutral`
  - 当前可尝试服务下一个排队请求
- `branch-granted`：
  - 已向某线程发放租约
  - 但尚未切入任务分支
- `task-active`：
  - 某线程已切入 `codex/...` 分支并正在真实写入
- `returned-main`：
  - 线程已完成 checkpoint 并归还 shared root

### 3.2 queue entry
- `waiting`
- `granted`
- `task-active`
- `completed`
- `cancelled`

## 4. 线程申请流程

### 4.1 业务线程
1. 完成只读 preflight。
2. 如准备进入真实写入，执行：
   - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action request-branch -OwnerThread <线程名> -BranchName codex/...`
3. 根据返回状态分流：
   - `GRANTED`
     - 允许继续执行 `ensure-branch`
   - `ALREADY_GRANTED`
     - 说明当前租约已经发给自己，可直接继续
   - `LOCKED_PLEASE_YIELD`
     - 不继续乱试 `ensure-branch`
     - 保存当前恢复点到 `memory_0.md`
     - 标记自己处于等待态
4. 如果本轮不想继续排队：
   - 执行：
     - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action cancel-branch-request -OwnerThread <线程名> -BranchName codex/...`
5. 如果希望重新排到队尾：
   - 执行：
     - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action requeue-branch-request -OwnerThread <线程名> -BranchName codex/...`

### 4.2 等待态允许做什么
- 继续只读核查
- 补文档
- 准备下一个 checkpoint 的设计 / 任务拆分
- 整理待执行清单

### 4.3 等待态禁止做什么
- 不得继续反复重试 `ensure-branch`
- 不得越过租约直接切分支
- 不得把 shared root 当成自己的长期现场

## 5. 治理线程的唤醒协议

### 5.1 何时检查队列
- shared root 已回到：
  - `main`
  - `occupancy = neutral`
  - 无未消费 grant
- returning 线程执行 `return-main` 后，会直接打印：
  - `NEXT_IN_LINE_TICKET`
  - `NEXT_IN_LINE_OWNER_THREAD`
  - `NEXT_IN_LINE_BRANCH`
  这只是唤醒提示，不等于已经自动发放 grant。

### 5.2 如何选择下一个线程
- 从 `shared-root-queue.md` 中读取最早的 `waiting` ticket。
- 默认按 ticket 顺序服务，不人为插队。
- 只有在明确业务优先级压过当前队首时，治理线程才可越序，但必须在文档中写明原因。

### 5.3 唤醒后线程该做什么
- 治理线程执行：
  - `powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action wake-next -OwnerThread Codex规则落地`
- 该动作会把队首 `waiting` 提升为 `granted`
- 先重做 live preflight
- 再执行：
  - `request-branch`
  - 预期返回 `ALREADY_GRANTED`
  - 然后继续 `ensure-branch`
- 不因为自己曾在队首，就跳过 live 核查

## 6. 当前版本的边界
- v1 还不是全自动调度器。
- 当前已落地的是：
  - queue 文件载体
  - queue-aware 的请求入口
  - `cancel-branch-request / requeue-branch-request / wake-next`
  - `ensure-branch / return-main` 对 queue 状态的回写与 `NEXT_IN_LINE` 提示
  - 回到 `main` 后依据 `occupancy` 自愈 runtime queue 漂移
- 当前仍需后续补强的是：
  - 更完整的负例矩阵
  - 公平性 / 优先级 / 越序审批规则
  - 更高级的自动化守护进程或 Hook
- 当前仍需显式记住的风险：
  - 长期业务分支上的仓库内 `git-safe-sync.ps1` 仍可能是旧版本
  - 所以业务线程与治理线程的 live 调度动作，必须改走稳定 launcher
  - 老分支上的仓库内脚本只保留给治理线程做脚本本体调试或历史取证

## 7. 一句话口径
- 不是“谁先喊得大声谁进”，而是“谁先排到、现场又允许，谁就拿到下一次 shared root 写入机会”。
