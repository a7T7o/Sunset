# Sunset 当前工作树 dirty 与 WIP 归属表（2026-03-15）

## 1. 快照锚点
- Git 基线：`main@71e4a3b8b99d6d216336e7fb42045d7a22b3df7a`
- 当前 `main` 与 `origin/main`：同步
- 本表只回答一件事：当前工作树里的每一类 dirty / untracked 属于谁，能不能混入默认基线提交。

## 2. 当前 tracked dirty / deleted

| 路径 | Git 状态 | owner 线程 | 归类 | 能否混入默认基线提交 | 说明 |
|---|---|---|---|---|---|
| `.codex/threads/Sunset/spring-day1/memory_0.md` | `M` | `spring-day1` | 线程记忆 WIP | 否 | 属于 spring-day1 自己的线程记忆更新 |
| `.codex/threads/Sunset/导航检查/01_导航代码索引.md` | `D` | `导航检查` | 文档重组 WIP | 否 | 正迁入 `1.0.0初步检查/` 子目录 |
| `.codex/threads/Sunset/导航检查/02_导航全盘分析报告.md` | `D` | `导航检查` | 文档重组 WIP | 否 | 正迁入 `1.0.0初步检查/` 子目录 |
| `.codex/threads/Sunset/遮挡检查/memory.md` | `D` | `遮挡检查` | 文档重组 WIP | 否 | 正改为新的线程记忆结构 |
| `.codex/threads/Sunset/遮挡检查/遮挡-导航-命中一致性与性能风险报告.md` | `D` | `遮挡检查` | 文档重组 WIP | 否 | 正迁入 `1.0.0初步检查/` 子目录 |
| `.codex/threads/Sunset/遮挡检查/遮挡系统代码索引与调用链.md` | `D` | `遮挡检查` | 文档重组 WIP | 否 | 正迁入 `1.0.0初步检查/` 子目录 |
| `.kiro/specs/001_BeFore_26.1.21/导航系统重构/memory.md` | `M` | `NPC` | 旧工作区迁移尾账 | 否 | NPC 规划从导航工作区迁出后留下的尾账 |
| `.kiro/specs/900_开篇/spring-day1-implementation/memory.md` | `M` | `spring-day1` | 工作区记忆 WIP | 否 | spring-day1 当前子工作区记忆更新 |
| `.kiro/specs/999_全面重构_26.03.15/memory.md` | `M` | `导航检查` / `遮挡检查` | 父工作区共享记忆 WIP | 否 | 两条审计线程共享的父层记忆 |
| `Assets/000_Scenes/Primary.unity` | `M` | `spring-day1` | 业务场景 WIP | 否 | 当前主场景集成收口尚未完成 |
| `Assets/Sprites/NPC/001.png.meta` | `M` | `NPC` | 资源 WIP | 否 | NPC 资源链的一部分 |
| `Assets/Sprites/NPC/002.png.meta` | `M` | `NPC` | 资源 WIP | 否 | NPC 资源链的一部分 |
| `Assets/Sprites/NPC/003.png.meta` | `M` | `NPC` | 资源 WIP | 否 | NPC 资源链的一部分 |
| `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset` | `M` | `spring-day1` | 字体资产 WIP | 否 | 当前对话字体链尚未完成主场景收口 |
| `Assets/YYY_Scripts/Story/UI/DialogueUI.cs` | `M` | `spring-day1` | 业务脚本 WIP | 否 | 当前仍属于对话 live 收尾链 |

## 3. 当前 untracked

| 路径 | owner 线程 | 归类 | 能否混入默认基线提交 | 说明 |
|---|---|---|---|---|
| `.codex/threads/Sunset/NPC/memory_0.md` | `NPC` | 线程记忆 WIP | 否 | NPC 线程记忆尚未进入基线 |
| `.codex/threads/Sunset/导航检查/1.0.0初步检查/` | `导航检查` | 线程文档 WIP | 否 | 导航审计新目录尚未收尾 |
| `.codex/threads/Sunset/遮挡检查/1.0.0初步检查/` | `遮挡检查` | 线程文档 WIP | 否 | 遮挡审计新目录尚未收尾 |
| `.codex/threads/Sunset/遮挡检查/memory_0.md` | `遮挡检查` | 线程记忆 WIP | 否 | 遮挡线程新记忆文件 |
| `.kiro/specs/001_BeFore_26.1.21/导航系统重构/npc规划001.md` | `NPC` | 旧工作区迁移尾账 | 否 | 应由 NPC 线程决定保留说明、迁移或归档 |
| `.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/` | `spring-day1` | 子工作区 WIP | 否 | spring-day1 新阶段工作区尚未收尾 |
| `.kiro/specs/999_全面重构_26.03.15/遮挡检查/` | `遮挡检查` | 子工作区 WIP | 否 | 遮挡线程新工作区尚未收尾 |
| `.kiro/specs/NPC/` | `NPC` | 主工作区 / 子工作区 WIP | 否 | NPC 工作区结构与任务文档尚未同步入基线 |
| `Assets/100_Anim/NPC/` | `NPC` | 资源 WIP | 否 | 当前未跟踪的 NPC 动画资源 |
| `Assets/222_Prefabs/NPC/` | `NPC` | 资源 WIP | 否 | 当前未跟踪的 NPC Prefab 资源 |

## 4. 当前归属判断
- 当前工作树不是“完全干净”，但它已经不是“归属不明”的混乱态。
- 当前最主要的未收口来源有三类：
  - `spring-day1` 的场景 / UI / 字体 live 集成 WIP
  - `NPC` 的资源与规划工作区 WIP
  - `导航检查` / `遮挡检查` 的线程文档重组 WIP
- 因此现在不能把“`main` 与 `origin/main` 同步”误读成“当前工作树适合无边界继续做任何真实任务”。

## 5. 当前执行口径
- 治理线程可以继续只对白名单治理文档收口。
- 业务线程若要继续推进真实任务，应先以本表确认为自己的 WIP 边界，再决定是否进入 `codex/xxx` 分支。
- 本表下一次更新时，优先检查：
  - `Primary.unity`
  - `DialogueUI.cs`
  - `Assets/100_Anim/NPC/`
  - `Assets/222_Prefabs/NPC/`
  - `.kiro/specs/NPC/`
  - `.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/`
