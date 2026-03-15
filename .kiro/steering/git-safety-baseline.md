---
inclusion: manual
priority: P1
isCanonical: true
canonicalDomain: [Git安全工作流, 分支策略, Git preflight, checkpoint提交, rollback口径, .gitattributes策略]
keywords: [git, 分支, 提交, 回退, rollback, checkpoint, preflight, gitattributes]
lastUpdated: 2026-03-11
---

# Git 安全基线规范

## 适用范围

本规范适用于 `Sunset` 仓库内所有真实任务进入实现前的 Git 安全检查，目标是确保后续改动具备：

- 可回退
- 可审计
- 可恢复
- 可与当前主线任务清晰对应

## 1. 分支策略

### 默认规则

- 任何真实业务实现、代码修改、场景修改、Prefab 修改、ScriptableObject 修改、`Packages/` 修改、`ProjectSettings/` 修改，默认都必须在独立任务分支进行。
- 任务分支统一使用 `codex/` 前缀。
- 推荐命名格式：`codex/<系统>-<任务>-<补丁号>`。

示例：

- `codex/farm-10.2.2-patch002`
- `codex/day1-dialogue-fix`
- `codex/save-prefab-database-cleanup`

### 允许留在 `main` 的情况

只有以下轻量治理类改动允许直接在 `main` 上操作：

- `.kiro/steering/` 规则文档
- 项目根 `AGENTS.md`
- `.kiro/hooks/` 中的治理型 Hook
- `.gitattributes`
- `.gitignore`
- 当前治理工作区的 `memory.md`、`tasks.md`、规范文档
- 线程记忆与纯治理型总结文档

### 绝对不允许直接在 `main` 上做的事

- 任何 `Assets/` 下的脚本、场景、Prefab、材质、SO、字体、图片等运行时或资源改动
- 任何 `Packages/`、`ProjectSettings/` 的实现性改动
- 任何业务工作区的实现推进
- 当前 `main` 已经 dirty，且脏改动不属于本轮治理主线时

## 2. Git preflight

进入真实任务实现前，至少执行一次以下预检：

1. `git branch --show-current`
2. `git status --porcelain=v2`
3. `git rev-parse --short HEAD`
4. `git rev-list --left-right --count @{upstream}...HEAD`
   - 如果当前分支没有 upstream，再检查 `origin/main...main`
5. 识别 dirty 文件属于哪一类：
   - 当前任务相关
   - 必须先收口
   - 必须先隔离
   - 已知本地噪音

### preflight 输出必须明确回答的 4 个问题

1. 当前是否允许继续在现有分支工作？
2. 当前 dirty 状态里，哪些是本轮可以带着走的？
3. 哪些必须先提交、转移或拆分，不能混入本轮？
4. 当前 HEAD 短 hash 是什么？它是不是本轮可回退基线？

## 3. dirty 状态分类

### 可以保留

- 当前治理任务本身的规则文档、任务看板、工作区记忆改动
- 当前任务分支内、且明确属于同一任务的连续小步实现改动

### 必须先收口

- 与当前任务无关、但仍被 Git 跟踪的业务代码改动
- 与当前任务无关的场景 / Prefab / SO / 资源改动
- 会导致一次提交混入多条主线的 tracked 改动

### 必须隔离

- 本地工具残留
- 本地特殊工作树
- 本地用户设置
- 不准备进入仓库历史的本机文件

## 4. 当前仓库的已知特殊噪音

### `.claude/worktrees/agent-a2df3da0`

- 当前应视为本地特殊工作树残留，不应再作为 `Sunset` 根仓库的正式受控结构长期保留。
- 它在根仓库中被记录为 `gitlink`，但仓库没有 `.gitmodules`，因此不满足正式子模块管理条件。
- 后续策略应为：
  1. 从根仓库跟踪中移除该 `gitlink`
  2. 在 `.gitignore` 中忽略 `.claude/worktrees/`
  3. 保留本地工作树目录本身，不把其内容继续计入根仓库正常状态判断

### `.claude/settings.local.json`

- 当前更适合作为本地机器 / 本地工作树设置，而不是仓库级共享文件。
- 后续策略应为：
  1. 从仓库跟踪中移除
  2. 在 `.gitignore` 中忽略
  3. 保留本地文件，不删除用户现有配置

## 5. checkpoint 提交规则

### 什么时候必须打 checkpoint

- 一个可独立描述、独立验证、独立回退的小任务完成后
- 即将开始高风险场景 / Prefab / SO 修改前
- 即将进行仓库治理性清理前
- 准备切题、关机、换机器或交接前

### 什么时候不该提交

- 当前改动还混着多条主线
- 当前改动还没过最小自检
- 当前在 `main` 上做业务实现

### checkpoint 的最低要求

- 提交前先看 `git diff --staged --stat`
- 只提交与本轮任务相关的文件
- 默认反对 `git add -A` 把跨工作区脏改一并扫入

## 6. rollback 口径

每轮需要给出至少两个可复原信息：

1. **起始基线**：本轮开始前的 `HEAD` 短 hash
2. **当前 checkpoint**：本轮最近一个可用提交 hash

对用户的汇报口径至少包含：

- 本轮开始基线：`<hash>`
- 当前最新 checkpoint：`<hash>`
- 如果要回退到本轮开始前，使用哪个 hash
- 如果只想撤回最后一步，使用哪个 hash

## 7. memory 记录要求

凡是形成了稳定的 Git 结论、preflight 结论、checkpoint 结论或 rollback 口径，都必须写入对应工作区记忆。

至少写清：

- 当前主线目标
- 当前分支
- 起始基线 hash
- 当前 checkpoint hash（如果有）
- 当前 dirty 状态是否已收口
- 是否仍有已知本地噪音未处理

## 8. `.gitattributes` 基线

仓库必须具备 `.gitattributes`，至少满足两类目标：

### 文本类

- Unity 文本资源统一为可稳定 diff 的文本行尾策略
- 规则文档、记忆文档、脚本、配置文件统一稳定文本属性

### 二进制类

- 图片、音频、字体、DLL、PSD 等标记为 binary
- 避免 Git 误把二进制当文本处理

## 9. 进入实现前的最终闸门

只有同时满足以下条件，才算达到“可安全进入实现”：

- 当前任务已在独立 `codex/` 分支上
- 当前分支 dirty 状态仅包含本任务相关改动
- `.claude/worktrees` 与本地设置噪音已被隔离，不再干扰根仓库判断
- `.gitattributes` 已存在并生效
- preflight 已做过并记录
- 当前起始基线与最近 checkpoint 已可明确说明

若任一项不满足，默认先补 Git 基线，再进入业务实现。

## 10. 显式白名单同步原则

- Git 自动同步只允许基于显式白名单路径执行，不能靠无边界 `git add -A` 扫全仓。
- 白名单由两部分组成：
  1. 当前模式的默认允许范围；
  2. 本轮通过 `ScopeRoots` / `IncludePaths` 明确声明的额外路径。
- 未落在白名单内的 dirty 改动，一律只做分类与汇报，不自动纳入提交。
- 这意味着：即使工作树里同时存在其他主线 dirty，只要本轮同步明确限定了自己的白名单，仍可安全形成当前任务的 checkpoint；但这些剩余 dirty 不等于已经安全，也不代表可以直接进入实现。

## 11. Codex 自动同步纪律

在 `Sunset` 仓库里，Codex 每轮实质性工作收尾时，固定执行顺序为：

1. 更新当前子工作区记忆
2. 更新受影响父工作区记忆
3. 更新当前线程记忆
4. 执行 `scripts/git-safe-sync.ps1`
5. 汇报当前分支、起始基线、最新 checkpoint、push 结果与剩余 dirty

### 模式要求

- 治理任务留在 `main` 时：
  - 使用 `git-safe-sync.ps1 -Action sync -Mode governance`
  - 允许默认治理白名单 + 本轮明确声明的记忆路径
- 如果当前线程已经在某个 `codex/` 分支上，而本轮只是在该分支顺手修治理文件：
  - 不强制切回 `main`
  - 改用 `git-safe-sync.ps1 -Action sync -Mode task -IncludePaths ...`
  - 只提交本轮明确列出的治理文件，不顺手吃进该分支上的其他业务 dirty
- 真实实现任务进入代码 / 场景前：
  - 先用 `git-safe-sync.ps1 -Action ensure-branch -BranchName codex/...`
- 真实实现任务收尾时：
  - 使用 `git-safe-sync.ps1 -Action sync -Mode task -ScopeRoots ... -IncludePaths ...`

### 自动同步失败时的口径

如果脚本判断当前不能安全同步，必须至少汇报：

- 当前分支
- 当前 `HEAD` 短 hash
- 不允许继续的原因
- 剩余 dirty 中哪些是当前任务外改动
- 下一步最小动作是什么

## 12. 自动任务分支托管

- `git-safe-sync.ps1 -Action ensure-branch` 只允许在工作树干净时创建或切换任务分支。
- 若当前位于 `main`，且 `main` 相对远端存在 ahead / behind，默认先同步 `main`，再建 `codex/` 分支。
- 推荐命名仍为：`codex/<系统>-<任务>-<补丁号>`。
- 任何 `Assets/`、`Packages/`、`ProjectSettings/` 的真实实现，都不应先改完再想起建分支；分支必须先于实现动作存在。

## 13. 长期线程、任务分支与 worktree 例外原则

- `Sunset` 的长期线程分三类语义：
  1. **A 类：治理 / 总览 / 只读 / 审计线程**，固定使用仓库根目录 `D:\Unity\Unity_learning\Sunset`，默认分支为 `main`
  2. **B 类：功能推进 / 集成 / 实现线程**，默认也先从仓库根目录 `D:\Unity\Unity_learning\Sunset` 进入，入口基线为 `main`；一旦进入真实实现，再切到对应 `codex/*` 分支
  3. **C 类：高风险隔离 / 故障修复 / 特殊实验线程**，只在确有必要时才使用独立 `worktree`
- 固定映射表的唯一项目内入口为：`D:\Unity\Unity_learning\Sunset\.codex\threads\线程分支对照表.md`
- `worktree` 现在只承担高风险隔离、故障修复、特殊实验和历史对照职责，不再作为普通功能线程的默认现场
- `merge` 的职责是让功能分支成果回到 `main` 参与联调，不能拿线程切换或 `worktree` 视图替代
- 名称中带 `restored`、`snapshot`、`backup`、`mixed` 的分支，默认只用于取证、恢复、对比，不作为长期线程默认归属

## 14. 进入线程前的固定核验

- 进入任何长期线程前，至少核验两件事：
  1. 当前工作目录是否符合对照表
  2. `git branch --show-current` 是否符合对照表
- UI 中残留的分支提示、线程历史元数据、上次会话记忆，都不能替代这次实际核验
- 如果工作目录或分支与对照表不一致，默认先修正路由，不直接继续写代码、改场景或跑破坏性命令
- 若线程默认目录已通过 Codex 状态层写入，也仍要做这一步核验；状态层只是便利入口，不是唯一真相

## 15. `main` 与功能线程的边界

- 允许长期停留在 `main` 的线程，只应包括：
  - 规则治理
  - 项目总览
  - 历史交接
  - 审计 / 巡检 / 只读分析
  - 纯治理文档维护
- 以下线程或任务，不允许继续借根目录 `main` 做真实实现：
  - 农田补丁实现
  - NPC 工具链实现
  - 任何会改动 `Assets/`、`Packages/`、`ProjectSettings/` 的真实功能开发
- 如果功能线程需要在 `main` 中看见成果，正确动作是“在检查点后合并回 `main`”，而不是继续把实现长期堆在 `main`
