# 全线程唤醒清单（2026-03-18）

## 统一 live 基线
- shared root 固定为 `D:\Unity\Unity_learning\Sunset`
- 当前 Git 基线为 `main`，`git status --short --branch` 应保持 clean
- `git worktree list --porcelain` 当前只允许看到 shared root；历史 worktree 不再是默认入口
- 以后调用 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 时，必须显式带 `-OwnerThread`
- 真实业务写入前必须先过 preflight；从 `main` 进入真实实现时，必须先 `ensure-branch`
- 到一个 checkpoint 就归还，不是等整个大功能做完才归还
- 只要碰 Unity / MCP，就把它当单实例共享资源；谁进了 Play Mode，谁负责退回 Edit Mode 后再交还现场
- 任何线程如果要碰 `Primary.unity`、`GameInputManager.cs` 或其他 A 类热文件，先查锁、先申请，再写

## NPC
```text
【NPC 线程唤醒｜先做 carrier 复核，再决定恢复写入】

当前统一基线已经回正：
- shared root = D:\Unity\Unity_learning\Sunset
- 当前分支 = main
- shared root 已恢复 neutral
- git worktree list 现在只剩 shared root

你的第一轮任务不是直接写代码，而是只读复核 NPC 的 continuation carrier。

请严格执行：
1. 进入 shared root 后先做只读 preflight，核对 cwd / branch / HEAD / git status。
2. 读取：
   - D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md
   - D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md
   - D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\memory.md
3. 只读比较这两条分支：
   - codex/npc-roam-phase2-002
   - codex/npc-roam-phase2-003
4. 你要明确回复：
   - 哪一条才是 NPC 今后唯一 continuation branch
   - 另一条是否应视为事故/过渡/只读对照分支
   - 如果你主张继续用 003，必须给出比 002 更强的 live 证据

本轮禁止：
- 禁止直接写文件
- 禁止直接切到某条 NPC 分支开始开发
- 禁止进入 Play Mode

补充规则：
- 以后凡是调用 git-safe-sync.ps1，必须显式带：-OwnerThread NPC
- 如果后续确认要恢复真实写入，再从 shared root 执行：
  git-safe-sync.ps1 -Action ensure-branch -Mode task -OwnerThread NPC -BranchName <你确认后的唯一 NPC 分支>
- 如果要碰 A 类热文件或 Unity 共享热区，先申请锁

回报格式固定为：
- 当前 cwd / branch / HEAD
- 你读了哪些文档和哪两条分支
- 你认定的唯一 continuation branch
- 你下一步若恢复开发，最小 checkpoint 会是什么
```

## farm
```text
【farm 线程唤醒｜允许恢复 branch-only，但先过 root preflight】

当前统一基线已经回正：
- shared root = D:\Unity\Unity_learning\Sunset
- 当前分支 = main
- shared root 已恢复 neutral
- git worktree list 现在只剩 shared root

你当前的唯一 continuation branch 口径是：
- codex/farm-1.0.2-cleanroom001

请严格执行：
1. 先在 shared root 做 preflight，只读核对 cwd / branch / HEAD / git status。
2. 读取：
   - D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V2\memory_0.md
   - D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\memory.md
   - 当前目标子工作区 memory
3. 如果你这轮要进入真实 farm 写入，再执行：
   git-safe-sync.ps1 -Action ensure-branch -Mode task -OwnerThread 农田交互修复V2 -BranchName codex/farm-1.0.2-cleanroom001
4. 进入分支后，先做最小基线复核，再开始你的真实实现或验收闭环。

本轮禁止：
- 禁止直接在 main 上做 farm 业务写入
- 禁止恢复任何历史 worktree 作为默认入口
- 禁止把 shared root 当成长驻私人现场
- 如果进入 Play Mode，结束后必须退回 Edit Mode

补充规则：
- 以后凡是调用 git-safe-sync.ps1，必须显式带：-OwnerThread 农田交互修复V2
- 到一个 checkpoint 就归还：commit + push + 退回 Edit Mode + 释放锁 + shared root 回 main + 无 unrelated dirty

回报格式固定为：
- 当前 cwd / branch / HEAD
- 是否已成功通过 preflight
- 是否已安全切入 codex/farm-1.0.2-cleanroom001
- 你这轮最小目标与最小 checkpoint
```

## spring-day1
```text
【spring-day1 线程唤醒｜可以恢复，但先走规范重入】

当前统一基线已经回正：
- shared root = D:\Unity\Unity_learning\Sunset
- 当前分支 = main
- shared root 已恢复 neutral
- git worktree list 现在只剩 shared root

你当前的功能入口分支仍然是：
- codex/spring-day1-story-progression-001

请严格执行：
1. 先在 shared root 做只读 preflight，核对 cwd / branch / HEAD / git status。
2. 读取：
   - D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_0.md
   - D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory.md
   - 当前子工作区 memory
   - D:\Unity\Unity_learning\Sunset\.kiro\steering\ui.md
3. 如果这轮进入真实写入，再执行：
   git-safe-sync.ps1 -Action ensure-branch -Mode task -OwnerThread spring-day1 -BranchName codex/spring-day1-story-progression-001
4. 如果下一步涉及 `Primary.unity`、`DialogueUI.cs`、共享字体、共享 UI 材质或其他热区，先走锁/热区核查，不要直接写。

本轮禁止：
- 禁止直接在 main 上写 spring-day1 业务内容
- 禁止忽略 UI / 气泡 / 字体 / 样式审美验收
- 禁止在 Play Mode 做完后不退回 Edit Mode

补充规则：
- 以后凡是调用 git-safe-sync.ps1，必须显式带：-OwnerThread spring-day1
- 样式与观感是硬验收项，不再以“能显示/能跑通”代替

回报格式固定为：
- 当前 cwd / branch / HEAD
- 是否通过 preflight
- 是否安全切入 codex/spring-day1-story-progression-001
- 如果要碰热文件/Unity 热区，先报你要申请什么
- 本轮最小 checkpoint
```

## 导航检查
```text
【导航检查线程唤醒｜默认只读审计，不直接写】

当前统一基线已经回正：
- shared root = D:\Unity\Unity_learning\Sunset
- 当前分支 = main
- shared root 已恢复 neutral
- git worktree list 现在只剩 shared root

你这轮默认身份是只读审计线程，不是立刻进入写入线程。

请严格执行：
1. 先在 shared root 做只读 preflight，核对 cwd / branch / HEAD / git status。
2. 读取你的线程记忆、当前治理入口、相关导航工作区文档。
3. 在 main 上只做只读审计：
   - 看结构
   - 看病态臃肿点
   - 看导航相关 hot files / shared files / Unity 热区
4. 本轮先输出：
   - 当前导航问题分层
   - 哪些能只读得出结论
   - 哪些如果要改，必须单独开分支

本轮禁止：
- 禁止直接写文件
- 禁止直接把 shared root 切成你自己的长期现场
- 禁止在没有新分支的情况下做任何真实实现

如果只读审计后确认必须写：
- 先回复我你要写什么、是否碰热文件
- 然后再用：
  git-safe-sync.ps1 -Action ensure-branch -Mode task -OwnerThread 导航检查 -BranchName codex/navigation-audit-001

补充规则：
- 以后凡是调用 git-safe-sync.ps1，必须显式带：-OwnerThread 导航检查
- Unity / MCP 只要涉及写入或 Play Mode，就不再算只读审计

回报格式固定为：
- 当前 cwd / branch / HEAD
- 本轮只读看了哪些内容
- 导航问题的分层结论
- 是否确实需要下一轮开 codex/navigation-audit-001
```

## 遮挡检查
```text
【遮挡检查线程唤醒｜默认只读审计，不直接写】

当前统一基线已经回正：
- shared root = D:\Unity\Unity_learning\Sunset
- 当前分支 = main
- shared root 已恢复 neutral
- git worktree list 现在只剩 shared root

你这轮默认身份是只读审计线程，不是立刻进入写入线程。

请严格执行：
1. 先在 shared root 做只读 preflight，核对 cwd / branch / HEAD / git status。
2. 读取你的线程记忆、当前治理入口、相关遮挡/渲染/场景工作区文档。
3. 在 main 上只做只读审计：
   - 看遮挡体系现状
   - 看病态结构与重构点
   - 看会不会碰共享场景、共享排序层、共享材质、共享 UI 热区
4. 本轮先输出：
   - 当前遮挡问题分层
   - 只读可确认的结论
   - 如果进入整改，哪些点必须单独开分支

本轮禁止：
- 禁止直接写文件
- 禁止直接进入 Play Mode 做长期占用
- 禁止在没有分支和没有热区核查的情况下做任何真实整改

如果只读审计后确认必须写：
- 先回复我你要改什么、是否碰热文件/Unity 热区
- 然后再用：
  git-safe-sync.ps1 -Action ensure-branch -Mode task -OwnerThread 遮挡检查 -BranchName codex/occlusion-audit-001

补充规则：
- 以后凡是调用 git-safe-sync.ps1，必须显式带：-OwnerThread 遮挡检查
- 如果进入 Play Mode 做验证，做完必须先退回 Edit Mode 再汇报

回报格式固定为：
- 当前 cwd / branch / HEAD
- 本轮只读看了哪些内容
- 遮挡问题的分层结论
- 是否确实需要下一轮开 codex/occlusion-audit-001
```
