# D 阶段：恢复正常开发模型

## 1. 最终口径
- Sunset 的常态开发模型是：
  - `main-common + branch-task + checkpoint-first + merge-last`
- 它不再等同于：
  - “所有线程长期挂在自己的 worktree 上开发”
- shared root 的唯一常态职责是：
  - 作为默认入口
  - 常驻 `main`
  - 承载治理线程在 `main + governance` 上的短时收口

## 2. 并发允许与禁止
### 可以并发的事
- 只读核查
- 文档阅读
- 设计与方案思考
- 线程汇报与只读取证
- 进入前 preflight
- 非写入型的历史 diff / reflog / log 审计

### 不可以并发的事
- 在同一个 shared root 上并发真实写入
- 在 shared root 未归还时，别的业务线程继续切自己的 `codex/...` 分支
- 在 Unity 单实例中并发写 Scene / Prefab / Inspector / Play Mode / Compile / Domain Reload 相关状态
- 在 `main` 上做真实业务实现并直接收口
- 把 unrelated dirty 留在 shared root 里继续做下一条业务线

## 3. 进入真实开发的标准动作
1. 先从 `D:\Unity\Unity_learning\Sunset @ main` 进入。
2. 先过启动闸门与 preflight。
3. 先确认：
   - shared root 当前是 neutral
   - Unity / MCP 单实例层没有被别的线程占用
4. 真正开始业务写入前，执行：
   - `git-safe-sync.ps1 -Action ensure-branch -OwnerThread <线程名> -BranchName codex/...`
5. 在自己的 `codex/...` 分支上完成一轮最小闭环。
6. 到 checkpoint 就收口，不把 shared root 当成长驻私人现场。

## 4. checkpoint 归还标准
- 一个线程想宣称“我已归还 shared root / 可交给别人”时，最低必须同时满足：
  - 当前任务相关改动已经形成 checkpoint
  - checkpoint 已 push
  - 如果进入过 Play Mode，已经退回 Edit Mode
  - 如果拿过锁，已经释放锁
  - shared root 已切回 `main`
  - shared root working tree 不留 unrelated dirty
  - shared root 占用文档已回到 neutral

## 5. Git / 脚本硬闸机
- 以后所有线程执行 `git-safe-sync.ps1` 都必须显式传：
  - `-OwnerThread <线程名>`
- 当前已落地的硬阻断包括：
  - `main` 禁止 `task` 模式
  - `task` 分支必须带 `codex/` 前缀
  - `task` 分支必须与 `OwnerThread` 语义匹配
  - occupied shared root 必须匹配 `owner_thread + current_branch`
  - shared root 若还留着未纳入白名单的 remaining dirty，`task` 模式直接阻断
  - shared root 的 `main` 在 `ensure-branch` 前必须先回到 neutral

## 6. worktree 最终口径
- `worktree` 只保留例外身份：
  - 高风险隔离
  - 故障修复
  - 特殊实验
- 只要异常阶段结束，就应回到：
  - shared root `main`
  - branch-only
- 历史 worktree 即便暂时残留空目录，也不再拥有 live 入口资格。

## 7. Unity / MCP 补充纪律
- Git 现场 neutral，不等于 Unity 现场 neutral。
- 只要任务碰 Unity / MCP，就还必须额外核：
  - `mcp-single-instance-occupancy.md`
  - `mcp-hot-zones.md`
  - 必要时回看 `mcp-single-instance-log.md`
- 谁进入 Play Mode，谁负责退回 Edit Mode 后再交还现场。

## 8. 一句话版
- 以后 Sunset 的正常模式就是：
  - shared root 常驻 `main`
  - 业务线程只在自己的 `codex/...` 分支上真写
  - 到 checkpoint 就归还
  - 并发允许读，不允许在同一 shared root 上无序并发写
