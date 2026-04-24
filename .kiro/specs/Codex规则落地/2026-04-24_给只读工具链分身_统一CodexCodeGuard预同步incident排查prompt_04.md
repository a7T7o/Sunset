请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-24_shared-root工具incident与资源根同步分发批次_04.md]

这轮不要替任何业务线程继续跑 shared-root 上传。

你当前唯一主线固定为：
把 `spring-day1 / UI / 存档系统 / 导航检查` 这 `4` 条线已经拿到的 `CodexCodeGuard / pre-sync` 异常，收成 `1` 份统一工具 incident 结论；回答清楚它们是不是同一个根因、最靠近根因的是哪一层、下一刀真正该修哪几个文件/函数。

你必须先继承并且不要推翻的当前真状态：
1. `spring-day1`
   - `Assets/Editor/Story` 的 `23` 文件根内整合批已经真实尝试过
   - 当前不是 same-root remaining dirty，而是 stable preflight 超时，残留 `CodexCodeGuard.dll --phase pre-sync`
2. `UI`
   - `UI/Tabs` 这 `7` 文件当前已覆盖全部同根脏改
   - 新第一 blocker 已从 same-root 升级成 `CodexCodeGuard returned no JSON / baseline fail`
   - [PackagePanelTabsUI.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs) 已应视作这批核心件
3. `导航检查`
   - `Service/Navigation` 根内整合批已经真实尝试过
   - 新第一 blocker 不再是同根 remaining dirty，而是 `CodexCodeGuard incident during Ready-To-Sync (no JSON result)`
4. `存档系统`
   - `Data/Core` 三文件当前不再按业务上传推进
   - 已有证据指向 [Program.cs](/D:/Unity/Unity_learning/Sunset/scripts/CodexCodeGuard/Program.cs) 的
     - `GitDirtyState.Load(repoRoot)`
     - `RunGit(repoRoot, "diff", "--name-status", "HEAD", "--")`
     - `RunProcess(...)`
   - 且普通 `git diff --name-status HEAD --` 对这 `3` 个文件本身是瞬间返回的
5. 上面这 `4` 条线当前都已经是 `PARKED`
6. 这轮不再让原业务线程自己继续撞同一个 blocker

这轮唯一允许的范围固定为：
1. 这 `4` 条线对应的 `prompt_03`、线程回执、thread-state 状态文件
2. 与 `preflight / Ready-To-Sync / CodexCodeGuard` 直接相关的工具代码：
   - [Program.cs](/D:/Unity/Unity_learning/Sunset/scripts/CodexCodeGuard/Program.cs)
   - [git-safe-sync.ps1](/D:/Unity/Unity_learning/Sunset/scripts/git-safe-sync.ps1)
   - `C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1`
   - [Ready-To-Sync.ps1](/D:/Unity/Unity_learning/Sunset/.kiro/scripts/thread-state/Ready-To-Sync.ps1)
   - [StateCommon.ps1](/D:/Unity/Unity_learning/Sunset/.kiro/scripts/thread-state/StateCommon.ps1)
3. 与这次 incident 直接相关的只读日志、进程证据、memory

这轮明确不准做的事：
1. 不准代 `spring-day1 / UI / 存档系统 / 导航检查` 继续跑第二次业务上传尝试
2. 不准改任何业务文件
3. 不准扩到 Unity runtime、scene、Prefab、资源内容线
4. 不准把这轮做成“顺手修工具代码”；先把 incident 归因收清

这轮必须按顺序执行：
1. 先判断：现有证据是否已经足够把这 `4` 条线收成 `1` 个统一 incident，还是其实已经分裂成 `2+` 个不同 incident。
2. 默认不重跑。
3. 只有当现有证据不足以判断“到底是同一个根因还是多个根因”时，才允许做最多 `1` 次代表性最小复核。
4. 这次代表性复核也必须只读，不准把 `4` 组都重新跑一遍。
5. 最终必须明确回答：
   - 这 `4` 条线到底是 `1` 个共因 incident，还是 `2+` 个 incident
   - 最靠近根因的层，是：
     - `Ready-To-Sync/StateCommon`
     - 稳定 launcher
     - `git-safe-sync.ps1`
     - `CodexCodeGuard Program.cs`
     - 还是 CLI baseline 自身
   - 下一刀真正该修的最小边界是什么
   - 这下一刀应交给谁：
     - `Codex规则落地` 工具修复线
     - 还是仍可回交业务线程各自处理

这轮完成定义只有两种：
1. 你已经把这 `4` 条 incident 收成统一结论，并把下一刀修复边界说死
2. 或者你做了最多 `1` 次代表性只读复核后，把它们明确分成 `2+` 个 incident，并把各自下一刀边界说死

最终回执必须额外明确：
1. 这 `4` 条线最终是 `1` 个 incident 还是 `2+` 个 incident
2. 你有没有重跑；如果没有，为什么现有证据已经足够
3. 最靠近根因的是哪一层
4. 下一刀真正该修哪几个文件 / 函数
5. 原 `spring-day1 / UI / 存档系统 / 导航检查` 这 `4` 条业务线程此刻是否都应继续保持 `PARKED`

这轮默认是只读 incident 审计：
- 默认不补 `Begin-Slice`
- 默认不写业务 tracked 文件
- 如果你认为必须做 `1` 次代表性最小复核，也仍然保持只读，并在回执里把命令与证据写清
