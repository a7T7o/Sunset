# Sunset 当前工作树 dirty 与 WIP 归属表（2026-03-16）

## 1. 快照锚点
- 当前实际工作树分支：
  - `codex/npc-main-recover-001`
- 当前实际 `HEAD`：
  - `8aed637f`
- `main` 相对 `origin/main`：
  - `ahead=0, behind=0`
- 当前锁池：
  - 空，仅剩 `.kiro/locks/active/.gitkeep`
- 本表只回答一件事：
  - 当前工作树里的 dirty / untracked 属于谁，能不能混入默认基线或当前线程提交。

## 2. 当前 tracked dirty 分组

| 路径组 | owner 线程 | 当前归类 | 能否混入默认基线提交 | 说明 |
|---|---|---|---|---|
| `.kiro/specs/Steering规则区优化/当前运行基线与开发规则/` | `Codex规则落地` | 治理续办 WIP | 否 | 本轮现行入口与 Git 规则重构仍未治理提交 |
| `.kiro/specs/Steering规则区优化/memory.md` | `Codex规则落地` | 父治理记忆 WIP | 否 | 属于本轮治理记忆追加 |
| `.codex/threads/Sunset/Codex规则落地/memory_0.md` | `Codex规则落地` | 线程记忆 WIP | 否 | 属于本轮治理线程收尾 |
| `.codex/threads/Sunset/spring-day1/memory_0.md`、`.kiro/specs/900_开篇/spring-day1-implementation/memory.md` | `spring-day1` | 线程 / 工作区记忆 WIP | 否 | 属于 `spring-day1` 自己的记忆尾账 |
| `.codex/threads/Sunset/农田交互修复V2/memory_0.md` | `农田交互修复V2` | 线程记忆 WIP | 否 | 属于 `farm` 的记忆尾账 |
| `.codex/threads/Sunset/项目文档总览/memory_0.md` | `项目文档总览` | 线程记忆 WIP | 否 | 属于项目文档总览线程尾账 |
| `.codex/threads/Sunset/导航检查/` 删除项、`.codex/threads/Sunset/遮挡检查/` 删除项、`.kiro/specs/999_全面重构_26.03.15/memory.md` | `导航检查` / `遮挡检查` | 审计文档重组 WIP | 否 | 当前仍处于文档迁移 / 重组状态 |
| `.kiro/specs/001_BeFore_26.1.21/导航系统重构/memory.md` | `NPC` | 旧工作区尾账 | 否 | NPC 线遗留的旧工作区记忆变动 |
| `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab` | `NPC` | Prefab WIP | 否 | 当前 NPC 资产实现线正在推进 |
| `Assets/Editor/NPCPrefabGeneratorTool.cs` | `NPC` | 工具脚本 WIP | 否 | 当前 NPC / 工具治理线共同关注的实现文件 |
| `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese*.asset` | `spring-day1` | 字体资产尾账 | 否 | 对话字体链仍留有资产脏改 |

## 3. 当前 untracked 分组

| 路径组 | owner 线程 | 当前归类 | 能否混入默认基线提交 | 说明 |
|---|---|---|---|---|
| `.kiro/specs/000_代办/codex/` | `Codex规则落地` | 治理续办新工作区 | 否 | 本轮新建的代办总入口尚未固化 |
| `.kiro/specs/000_代办/kiro/`、旧 `TD_*` 删除链 | 旧代办体系尾账 | 历史代办体系重组现场 | 否 | 当前属于历史代办目录迁移 / 收束尾账 |
| `.codex/threads/Sunset/Skills和MCP/` | `Skills和MCP`（已归档） | 归档线程遗留文档 | 否 | 用户已明确该线程直接归档；后续只按历史资料处理，不再作为活跃线程复工 |
| `.codex/threads/Sunset/导航检查/1.0.0初步检查/`、`.codex/threads/Sunset/遮挡检查/1.0.0初步检查/` | `导航检查` / `遮挡检查` | 线程文档 WIP | 否 | 当前审计线程的新目录结构尚未收尾 |
| `.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/` | `spring-day1` | 子工作区 WIP | 否 | `spring-day1` 新阶段工作区 |
| `.kiro/specs/999_全面重构_26.03.15/遮挡检查/` | `遮挡检查` | 子工作区 WIP | 否 | 遮挡审计线程新子工作区 |
| `Assets/Screenshots/` | `farm` / 验收样本 | 验收截图样本 | 否 | 暂不默认混入治理或业务基线 |
| `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`、`NPCBubblePresenter.cs` | `NPC` | 新业务脚本 WIP | 否 | 当前 NPC 实现线新增未跟踪脚本 |

## 4. 当前归属判断
- 当前工作树不是默认 `main` 现场的干净共享基线，而是处于 `codex/npc-main-recover-001` 上的多线程混合现场。
- 这不代表 Git 规则坏了，反而说明：
  - 默认基线 `main` 仍是干净同步的；
  - 当前本地工作树里存在多个线程尚未各自收口的 WIP。
- 因此不能把：
  - “`main` 与 `origin/main` 同步”
  - 和
  - “当前工作树适合无边界继续提交任何内容”
  混为一谈。

## 5. 当前执行口径
- 治理线程继续推进时，只能带本轮显式白名单治理文档。
- `NPC` 线程继续推进时，只能带自己的资产 / Prefab / NPC 脚本与对应记忆，不得顺手混入治理续办文件。
- `spring-day1`、`farm` 当前更适合先收自己的记忆与尾账，再决定是否进入新需求。
- 审计线程当前优先做文档结构收口，不宜把文档尾账混进别人的任务分支。

## 6. 当前一句话口径
- 2026-03-16 当前工作树的核心事实不是“113 未上传”，而是“默认基线 `main` 已同步，但本地当前处在 `NPC` 任务分支上，且混有多个线程尚未收口的 dirty / untracked WIP”。
