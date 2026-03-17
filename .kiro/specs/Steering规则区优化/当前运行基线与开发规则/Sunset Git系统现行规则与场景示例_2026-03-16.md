# Sunset Git系统现行规则与场景示例（2026-03-16）

## 1. 文件身份
- 本文件从 `2026-03-16` 起，承接当前现行 Git 规则入口。
- `Sunset Git系统现行规则与场景示例_2026-03-15.md` 已归档到：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\文档归档\2026-03-Codex恢复与迁移收口\06_L5解冻与阶段快照_2026-03-16\02_03-15首版共享表与模型退役_2026-03-16\Sunset Git系统现行规则与场景示例_2026-03-15.md`
- 后续 Git 规则如果发生明确阶段变化，应新建新的当前文件，不继续把最新口径叠写进旧日期文件名。

## 2. 先说结论
- 默认开发现场仍是 `D:\Unity\Unity_learning\Sunset`。
- 默认基线分支仍是 `main`，默认推送基线仍是 `origin/main`。
- 但“默认从 `main` 进入现场”不等于“所有改动都直接在 `main` 上提交”。
- 真正执行时要同时看四层：
  1. 当前状态入口：[Sunset当前唯一状态说明_2026-03-16.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset当前唯一状态说明_2026-03-16.md)
  2. 本文件
  3. `.kiro/steering/git-safety-baseline.md`
  4. `scripts/git-safe-sync.ps1`

## 3. 当前规则总表

| 场景 | 默认停留位置 | 收尾模式 | 是否必须先开 `codex/` 分支 | 典型例子 |
|---|---|---|---|---|
| 治理文档 / 规则 / 索引 / `memory` / Hook 文档 / 脚本说明 | `main` | `governance` | 否 | 更新状态说明、总索引、线程记忆、治理代办 |
| 真实任务实现 | `codex/...` | `task` | 是 | 改 `Assets/`、场景、Prefab、SO、资源、`Packages/`、`ProjectSettings/` |
| 只读核查 / preflight / MCP 现场读取 | 当前现场即可 | 不提交 | 否 | 看分支、看 dirty、看 Console、看场景 |
| 高风险隔离 / 故障修复 / 特殊实验 | 通常单独 `codex/...`，必要时 worktree | `task` | 通常是 | 大改场景、危险实验、隔离性试验 |

## 4. 当前最重要的边界

### 4.1 `main + governance`
- 只适用于治理类改动。
- 但脚本当前“自动放行”的默认范围已经收紧，只包括：
  - `.gitattributes`
  - `.gitignore`
  - `AGENTS.md`
  - `scripts/`
  - `.kiro/steering/`
  - `.kiro/hooks/`
- 这意味着下列对象虽然语义上属于治理类，但不会因为 `Mode=governance` 自动带入：
  - `.kiro/specs/Steering规则区优化/...`
  - `.kiro/specs/000_代办/codex/...`
  - `.codex/threads/...`
  - 工作区 / 线程 `memory`
  - 活文档目录中的状态、Git、总览、索引、共享表
- 收尾方式：
  - `git-safe-sync.ps1 -Action preflight -Mode governance -IncludePaths ...`
  - `git-safe-sync.ps1 -Action sync -Mode governance -IncludePaths ...`
  - 或使用 `-ScopeRoots ...`
- 核心理解：
  - `governance` 只给你“治理模式”，不再默认兜住整片治理文档树。

### 4.2 `codex/... + task`
- 只要碰到以下内容，就不要在 `main` 上收尾：
  - `Assets/`
  - `Packages/`
  - `ProjectSettings/`
  - 业务脚本
  - 场景
  - Prefab
  - ScriptableObject
  - 资源
- 收尾方式：
  1. 先从主项目目录读取现场
  2. 做 `preflight`
  3. 必要时查锁
  4. 再切到 `codex/...`
  5. 最后用 `git-safe-sync.ps1 -Action sync -Mode task` 白名单提交

### 4.3 白名单不是免责卡
- 白名单只回答“本轮带哪些文件”。
- 白名单不会放宽分支规则。
- 白名单不会允许你把业务实现直接在 `main` 上收尾。
- 依然禁止无边界 `git add -A`。

## 5. 当前标准工作流

### 5.1 治理类工作
1. 先确认当前主线和工作区。
2. 先读当前状态入口和需要的治理文档。
3. 带着本轮 `IncludePaths` / `ScopeRoots` 跑一次 `preflight`。
4. 只改白名单治理文件。
5. 更新记忆：
   - 当前子工作区
   - 父工作区
   - 当前线程
6. 用 `governance` 同步。

### 5.2 真实任务工作
1. 先在主项目根目录读取现场。
2. 先核实：
   - 当前分支
   - 当前 `HEAD`
   - dirty 分类
   - 是否涉及 A 类热文件
3. 如果要写 A 类热文件，先查锁。
4. 真正开工前切到 `codex/...`。
5. 只提交本轮白名单。
6. 如果拿过锁，完成 checkpoint 后释放。

## 6. A 类热文件规则
- 当前典型 A 类对象：
  - `Assets/000_Scenes/Primary.unity`
  - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
- 规则不是“整个工作树只能一个人动”，而是“共享热文件不能无序并发写”。
- 同文件跨 owner 转交前，上一 owner 必须已经是：
  - `Committed`
  - `Parked`
  - `Abandoned`
- 禁止 `Mixed-in-place`。
- 当前锁与认领入口应优先看后续健康版锁表；在健康版锁表尚未重建前，先看：
  - 当前状态说明
  - 当前 L5 总览
  - 当前线程汇报

## 7. `git-safe-sync.ps1` 的真实作用

### `preflight`
- 只检查，不提交。
- 至少回答四件事：
  - 当前在哪个分支
  - 当前 `HEAD` 是什么
  - 当前 dirty 哪些是本轮相关，哪些不是
  - 当前是否适合继续

### `ensure-branch`
- 用来安全创建 / 切换任务分支。
- 前提通常是：
  - 当前现场足够干净
  - `main` 和远端同步

### `sync`
- 执行白名单暂存、提交、推送。
- 不是 `git add -A` 的包装壳。
- 真正会拦你的，是模式、分支和白名单边界。
- 如果你没有显式给 `.kiro/specs/...`、`.codex/threads/...` 等路径传 `IncludePaths` / `ScopeRoots`，`governance` 也不会自动替你带上它们。

## 8. 最容易误判的 6 件事

### 误判 1
- “默认开发分支是 `main`” = “所有东西都在 `main` 上提交”
- 错。正确理解是：
  - 默认先从 `main` 读取现场；
  - 真实实现收尾仍回到 `codex/... + task`。

### 误判 2
- “我只改了一行，所以能直接在 `main`”
- 错。判断标准不是改几行，而是改的是什么类型的文件。

### 误判 3
- “白名单很干净，所以可以忽略分支规则”
- 错。白名单和分支边界是两套闸门。

### 误判 4
- “工作树里有别人的 dirty，说明 Git 规则不好用”
- 错。恰恰说明白名单隔离在起作用。

### 误判 5
- “worktree 才是多线程开发的默认姿势”
- 错。现在默认姿势是同一主项目目录 + 分支 + 热文件锁。

### 误判 6
- “VSCode 里的未上传数字就是真实提交数”
- 错。最终以 CLI 为准。

## 9. VSCode 里“113 未上传”该怎么理解
- 当前真实 Git 核验结果是：
  - `main` 相对 `origin/main` 为 `ahead=0, behind=0`
  - 当前真实所在分支可能并不是 `main`，而是某条 `codex/...`
  - 当前工作树里同时存在大量其他线程的 dirty / untracked
- 因此 VSCode 里看到的 `113`，不能直接解释成“当前主线还有 113 个未推送提交”。
- 更合理的理解是：IDE 把下列信息混在一起展示了：
  - 工作树脏改
  - 未跟踪文件
  - 其他本地分支状态
  - 自身缓存统计
- 真的要判断是否“未上传”，先看：
  - `git branch --show-current`
  - `git rev-list --left-right --count @{upstream}...HEAD`
  - `git status --short --branch`

## 10. 现在什么时候可以直接在 `main`
- 可以：
  - 状态说明
  - 总索引
  - 代办
  - 规则
  - 记忆
  - Hook 文档
  - 治理脚本说明
- 但要真正同步这些对象，仍需显式给出：
  - `-IncludePaths`
  - 或 `-ScopeRoots`
- 不可以直接在 `main` 收尾：
  - 任何业务脚本
  - 任何场景
  - 任何 Prefab
  - 任何资源
  - 任何 `Packages/` / `ProjectSettings/` 实现改动

## 11. 当前和 `000_代办/codex` 的关系
- L5 之后的治理续办，不再继续堆回旧的全局 `tasks.md`。
- 后续治理债应从：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex`
  进入并分阶段推进。
- 工作区结构、阶段 `tasks.md`、`memory` 分卷与代办新口径，统一看：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset工作区四件套与代办规范_2026-03-16.md`
- 这意味着：
  - Git 规则重写
  - 基础规则重写
  - skills / AGENTS 重构
  - 冻结汇总归档
  - 四件套规范重构
  都不再靠超长上下文硬扛。

## 12. 当前一句话执行口径
- 先从主项目根目录读取现场；
- 治理走 `main + governance`；
- 实现走 `codex/... + task`；
- A 类热文件先查锁；
- 一切以 CLI 和脚本拦截结果为准，不以 IDE 数字或口头印象为准。
