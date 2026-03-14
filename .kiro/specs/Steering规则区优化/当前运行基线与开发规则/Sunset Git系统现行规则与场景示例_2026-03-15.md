# Sunset Git系统现行规则与场景示例（2026-03-15）

## 1. 一句话先说清
- 当前 `Sunset` 的默认开发现场是 `D:\Unity\Unity_learning\Sunset`。
- 当前默认开发分支是 `main`，默认推送分支是 `origin/main`。
- 但 **不是所有改动都能直接在 `main` 上提交**。
- 当前 Git 系统的真实执行规则，要同时看三层：
  1. `当前运行基线与开发规则` 里的现行口径；
  2. `.kiro/steering/git-safety-baseline.md` 的 Git 安全规则；
  3. `scripts/git-safe-sync.ps1` 的实际拦截与放行逻辑。

---

## 2. 当前真实规则总表

| 场景 | 是否允许留在 `main` | 应用模式 | 是否必须先开 `codex/` 分支 | 典型例子 |
|---|---|---|---|---|
| 治理文档 / 规则 / memory / 索引 / Hook 文档 | 允许 | `governance` | 否 | 更新 `Sunset当前唯一状态说明`、补线程记忆、整理文档结构 |
| 业务代码 / 场景 / 资源 / Prefab / SO / `Packages` / `ProjectSettings` | 不允许直接在 `main` 收尾 | `task` | 是 | 改 `DialogueUI.cs`、改 NPC 工具、改农田放置逻辑 |
| 只做只读核查、不改文件 | 可留在当前分支 | 无提交 / `preflight` | 否 | 盘点 dirty、看分支状态、看脚本规则 |
| 高风险隔离 / 特殊实验 / 故障修复 | 默认不在 `main` 长做 | `task` + 可选 worktree | 通常是 | 实验性 MCP 切换、危险 Prefab 清洗、隔离性大重构 |

---

## 3. 规则源分别管什么

### 3.1 `当前运行基线与开发规则`
- 负责回答“现在默认在哪开发、默认先看什么文档、当前项目总体口径是什么”。
- 它解决的是**入口问题**，不是所有 Git 细节。

### 3.2 `.kiro/steering/git-safety-baseline.md`
- 负责回答“什么情况下必须分支、什么时候能提、什么时候不能提、dirty 怎么分类、checkpoint 怎么做、rollback 怎么说”。
- 它解决的是**Git 安全规则问题**。

### 3.3 `scripts/git-safe-sync.ps1`
- 负责把上面规则落成**可执行拦截**。
- 真正会拦你的不是“口头规则”，而是这个脚本：
  - `task` 模式下，如果你在 `main`，它会直接拒绝继续；
  - `task` 模式下，如果你没给 `ScopeRoots` 或 `IncludePaths`，它也会直接拒绝；
  - `ensure-branch` 时，如果工作树不干净，或者 `main` 没和远端同步，也会直接拒绝。

---

## 4. 现在最容易混淆的点

### 4.1 “默认开发分支是 `main`” 不等于 “所有东西都直接提交到 `main`”
- 现在的意思是：
  - 默认先从 `main` 进入主项目现场；
  - 但如果你要做的是**真实业务实现**，收尾时还是要先上 `codex/` 分支。

**场景例子**
- 你只是更新 `memory.md`、整理规则说明：
  - 可以直接留在 `main`
  - 用 `git-safe-sync.ps1 -Mode governance`
- 你要改 `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`：
  - 这属于真实业务实现
  - 不能直接在 `main` 用 `task` 模式提交
  - 要先 `ensure-branch` 到 `codex/...`

### 4.2 “小改动可以直接进 `main`” 的真实边界
- 当前真正成立的是：
  - **治理类小改动** 可以直接在 `main`
  - **业务类小改动** 目前仍按 `task` 规则走 `codex/` 分支
- 也就是说，“小改动”这个说法，不能脱离“改的是哪一类文件”单独理解。

**场景例子**
- 改一行 `Sunset当前唯一状态说明_2026-03-13.md`：可以在 `main`
- 改一行 `NPCPrefabGeneratorTool.cs`：仍然应该先上 `codex/xxx`

---

## 5. `git-safe-sync.ps1` 到底怎么工作

## 5.1 三个动作

### `preflight`
- 只检查，不提交。
- 用来看：
  - 当前分支
  - 当前 `HEAD`
  - 与远端的 ahead/behind
  - 哪些改动会被当成本轮允许同步
  - 哪些改动只会被保留/报告

**例子**
```powershell
powershell -ExecutionPolicy Bypass -File scripts/git-safe-sync.ps1 -Action preflight -Mode governance
```

### `ensure-branch`
- 用来创建或切换到任务分支。
- 但它有两个前提：
  1. 当前工作树必须干净（除已知本地噪音外）
  2. 如果当前在 `main`，`main` 必须和远端同步

**例子**
```powershell
powershell -ExecutionPolicy Bypass -File scripts/git-safe-sync.ps1 -Action ensure-branch -BranchName codex/npc-tool-warning-fix
```

### `sync`
- 真正执行白名单暂存、提交、推送。
- 它不是 `git add -A`，而是先做 preflight，再只暂存白名单路径。

---

## 5.2 两种模式

### `governance`
- 适用于：
  - `.kiro/steering/`
  - `.kiro/specs/Steering规则区优化/`
  - `.kiro/hooks/`
  - `scripts/`
  - `.gitattributes`
  - `.gitignore`
  - `AGENTS.md`
  - 以及你显式加进来的治理文档 / 线程记忆

**场景例子**
- 你整理规则文档、更新记忆、补索引：
  - 用 `governance`
  - 可以留在 `main`

### `task`
- 适用于真实任务改动。
- 脚本会做三条硬拦截：
  1. 如果当前在 `main`，直接拒绝；
  2. 如果当前分支不叫 `codex/...`，直接拒绝；
  3. 如果没给 `ScopeRoots` / `IncludePaths`，直接拒绝。

**场景例子**
- 你在 `main` 上改了 `NPC` 的脚本，再跑：
```powershell
powershell -ExecutionPolicy Bypass -File scripts/git-safe-sync.ps1 -Action sync -Mode task -IncludePaths 'Assets/Editor/NPCPrefabGeneratorTool.cs'
```
- 结果会被挡住，因为它看到：
  - 你现在在 `main`
  - 这是 `task` 模式
  - 于是判定“真实任务前必须先创建 `codex/` 分支”

---

## 6. “白名单同步”到底是什么意思

- 白名单同步 = 只提交“这轮明确相关的文件”，不把整个仓库当前所有 dirty 一起端上去。
- 它的核心不是“我改得少”，而是“我明确声明这次要带哪些路径”。

**场景例子：NPC 那轮那句话是什么意思**
- 当时的意思不是“文档坏了”，也不是“Git 坏了”。
- 真正的意思是：
  1. 只想提交这轮 `NPC` 相关文件；
  2. 仓库里还有别的无关 dirty；
  3. 没有去碰那些无关 dirty；
  4. 但是因为当时使用的是 `task` 规则，且人在 `main`，所以脚本在提交前就拦住了；
  5. 结果就是：**改动在本地有，但那一轮没有完成提交 / 推送**。

这也是为什么“白名单同步”和“能不能直接在 `main` 提交”是两回事：
- 你可以白名单得很干净；
- 但如果模式/分支不对，脚本照样会挡。

---

## 7. dirty 怎么分类，脚本怎么看

脚本内部会把脏改分成几类：
- `治理线改动`
- `实现/资源改动`
- `农田线改动`
- `开篇线改动`
- `about治理线改动`
- `线程记忆改动`
- `已知本地噪音`
- `其他改动`

**场景例子**
- 你本轮只想提交治理文档：
  - `.kiro/specs/Steering规则区优化/...` 会被视作治理线改动
  - `Assets/100_Anim/NPC/`、`Assets/222_Prefabs/NPC/`、`Assets/Sprites/NPC/*.meta` 会被视作实现/资源改动
  - 它们会被列在“仍保留在工作树中的其他改动”，但不会自动混进这次提交

---

## 8. 现在到底什么时候能直接在 `main`

### 可以
- 规则文档
- 活文档目录
- 线程记忆
- 工作区记忆
- 路由页
- 索引文档
- Hook 文档 / Hook 规则
- 纯治理脚本说明

**例子**
- 整理 `当前运行基线与开发规则`
- 更新 `Codex规则落地` 线程 memory
- 重组归档目录

### 不可以直接在 `main` 收尾
- `Assets/` 下的业务脚本
- 场景
- Prefab
- ScriptableObject
- 资源
- `Packages/`
- `ProjectSettings/`

**例子**
- 改 `DialogueUI.cs`
- 改 `TimeManager.cs`
- 改农田放置逻辑
- 改 NPC 工具运行时代码

---

## 9. 什么时候必须先开 `codex/` 分支

- 只要你做的是**真实任务实现**，就应该先开。
- 不看“是不是只改了一行”，看“改的是不是业务实现层”。

**例子**
- 只改一行 `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
  - 也算真实实现
  - 仍应先开 `codex/...`
- 改十页治理文档
  - 还是治理
  - 可以留在 `main`

---

## 10. 什么时候才允许 worktree

- 高风险隔离
- 故障修复
- 特殊实验

**例子**
- 想测试一条可能破坏场景的大改链路：可以开独立 worktree
- 想做一次 MCP 兼容性试验，不想污染当前主现场：可以开独立 worktree
- 普通 NPC 文档整理、普通业务修补：不应该默认先开 worktree

---

## 11. preflight / checkpoint / rollback 该怎么理解

### preflight
- 先看分支、HEAD、upstream、dirty 分类，再决定要不要继续。

**例子**
- 你准备开始一轮 `NPC` 修复：
  - 先跑 `preflight`
  - 发现当前工作树里还有 `farm` 和 `spring-day1` 的脏改
  - 这时不能假装没看见，必须明确它们是“保留/隔离”，不是本轮自动带走

### checkpoint
- 能单独描述、能单独回退、能单独验收的一步，就值得形成一次提交。

**例子**
- “只补 `DialogueDebugMenu.cs:158` 的红编译” 可以是一次 checkpoint
- “只整理文档目录结构” 也可以是一次 checkpoint

### rollback
- 每轮都应能说清：
  - 本轮起始基线 hash
  - 本轮当前 checkpoint hash
  - 如果要回退，回到哪一个

---

## 12. 现在仓库里的特殊现实情况

- 当前 `main` / `origin/main` 已同步。
- 但当前工作树仍有不少**不属于本轮 Git 规则整理任务**的 dirty / untracked。
- 它们不代表 Git 系统坏了，只代表当前仓库不是空白洁净场。

**当前必须明确不混入默认基线的例子**
- `Assets/100_Anim/NPC/`
- `Assets/222_Prefabs/NPC/`
- `Assets/Sprites/NPC/*.meta`
- 其他线程自己的 `memory.md` / `memory_0.md`

这类内容要么留给原线程处理，要么在自己的任务分支收口，不能因为“顺手提交一下”就混入治理提交。

---

## 13. 你现在最该记住的执行口径

### 口径 A：默认先站在主项目根目录
- 先进入 `D:\Unity\Unity_learning\Sunset`
- 先看 `main`
- 先看活文档目录

### 口径 B：治理改动和业务改动分开看
- 治理改动：`main + governance`
- 业务改动：`codex/... + task`

### 口径 C：白名单不等于放宽规则
- 白名单只解决“这次带哪些文件”
- 不解决“你现在应不应该在 `main` 上提交真实任务”

### 口径 D：脚本执行层优先于口头想当然
- 如果脚本挡了，先看：
  - 你是不是在 `main`
  - 你是不是用了 `task`
  - 你有没有给 `ScopeRoots` / `IncludePaths`
  - 你的工作树是不是不干净

---

## 14. 最简决策树

### 我现在只改了规则 / memory / 文档
- 留在 `main`
- 用 `governance`
- 白名单提交

### 我现在改了任何 `Assets/` / `Packages/` / `ProjectSettings/`
- 先看是否真是业务实现
- 如果是：先 `ensure-branch` 到 `codex/...`
- 再用 `task`

### 我现在仓库里有别人的 dirty
- 不替别人收
- 不混进本轮
- 让脚本把它们列为 remaining

### 我现在想知道 NPC 那轮为什么没推上去
- 因为当时是在 `main`
- 但使用的是真实任务提交逻辑
- 所以脚本先拦住了
- 结果就是“本地有改动，但那一轮没有形成提交/推送”
