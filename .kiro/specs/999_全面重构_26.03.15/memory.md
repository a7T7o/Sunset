# 999_全面重构_26.03.15 - 工作区记忆

## 模块概述

本工作区承接 2026-03-15 之后的全面重构审计类子工作区，目前已显式进入 `导航检查` 与 `存档检查` 两条子线。

## 当前状态

- 最后更新：2026-03-15
- 状态：导航检查子工作区已建立首轮审计基线

## 会话记录

### 会话 1 - 2026-03-15

- 子工作区 `导航检查` 已创建 `memory.md`，完成首轮只读审计与线程旧稿复核。
- 结论：主链为 `GameInputManager -> PlayerAutoNavigator -> NavGrid2D`，`PlacementManager/PlacementNavigator`、`TreeController`、`ChestController`、`CropController`、`NPCDialogueInteractable`、`CloudShadowManager` 为直接依赖路径。
- 风险重排：`NavGrid2D` 当前并不是真正 Zero GC，`CloudShadowManager` 的 NavGrid 反射路径在主场景当前为潜在态而非活跃态，`chest-interaction.md` 与现代码的 `ClosestPoint` 导航目标逻辑已出现文档漂移。
- 关键文件：`.kiro/specs/999_全面重构_26.03.15/导航检查/memory.md`，`.codex/threads/Sunset/导航检查/1.0.0初步检查/03_现状核实与差异分析.md`。

### 会话 1 - Git 收尾补记 - 2026-03-15

- 已执行 `git-safe-sync.ps1` 并创建/推送提交 `2026.03.15-01`（`3d7d65d5`）。
- 注意：`governance` 模式预检额外纳入了两份既有治理线 memory 改动：`.kiro/specs/Steering规则区优化/memory.md` 与 `.kiro/specs/Steering规则区优化/当前运行基线与开发规则/memory.md`。
- 这两份并非本子工作区新增文件，但已随本次同步进入 `main`；后续若要追溯本线程提交范围，需要把这点一并考虑。

### 会话 2 - 2026-03-15

- 新建子工作区 `遮挡检查`，补建 `.kiro/specs/999_全面重构_26.03.15/遮挡检查/memory.md`，开始建立遮挡系统只读审计基线。
- 当前主线目标：核实现有遮挡系统的真实实现，而不是沿用旧遮挡文档或线程旧稿的结论。
- 本轮阻塞：用户指定的子工作区原本不存在，父工作区也没有遮挡检查文档，需要先补工作区记忆承接。
- 子工作区首轮核实已确认：
  - 旧线程稿中的脚本 GUID 映射有误，需按当前 `.meta` 文件纠正。
  - `Primary.unity` 的 `OcclusionManager` 参数与旧稿有明显漂移，`rootConnectionDistance` 当前为 `1.5`。
  - 树 Prefab 普遍存在父/子双 `OcclusionTransparency` 结构，`TreeController` 只控制当前节点那一份组件。
  - 命中判定在点击链与工具链之间仍是双标准。
  - EditMode 测试虽然全绿，但对当前真实主链覆盖不足。
- 关键文件：`.kiro/specs/999_全面重构_26.03.15/遮挡检查/memory.md`，`.codex/threads/Sunset/遮挡检查/1.0.0初步检查/03_遮挡现状核实与差异分析（Codex视角）.md`。

### 会话 3 - 2026-03-16

- `导航检查` 子工作区本轮未进入代码整改，而是先完成“文档重组与阶段状态收口”。
- 已核对当前现场：工作目录 `D:\Unity\Unity_learning\Sunset`，当前分支 `codex/npc-main-recover-001`，当前 `HEAD` 为 `b9b6ac48`；仓库存在大量文档与代码脏改，但与 `导航检查` 直接相关的文档现场主要是旧稿删除、阶段目录承接和记忆补记。
- 子工作区已新增阶段结案文档：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\1.0.0初步检查\04_审计阶段结案与移交建议.md`
- 父层稳定结论：
  - `导航检查` 的 `1.0.0初步检查` 已可视为独立完成的审计基线阶段；
  - 当前更适合先阶段结案，再决定是否进入下一阶段整改设计；
  - 若后续继续整改，不应直接在现有审计阶段里混改，而应新开下一阶段并补齐 `requirements/design/tasks`。

### 会话 3 - 2026-03-16

- 子工作区 `遮挡检查` 本轮未进入代码整改，而是先完成“审计成果固化与阶段口径澄清”。
- 当前主线目标从“继续判断系统问题”收口为“把已经完成的审计成果固化成子工作区可承接基线”。
- 当前现场已发生重要变化：
  - 工作目录仍为 `D:\Unity\Unity_learning\Sunset`
  - 当前分支已变为 `codex/npc-main-recover-001`
  - 工作树混入大量与 `遮挡检查` 无关的治理重组 / 删除 / NPC 恢复内容
- 子工作区新增：
  - `.kiro/specs/999_全面重构_26.03.15/遮挡检查/审计成果固化与阶段口径.md`
- 本轮父层确认的稳定结论：
  - `遮挡检查` 当前应视为“审计基线已建立，待决定是否进入整改设计”，而不是“默认已进入代码整改”；
  - 后续如果开整改，最小目标应先锁定树 Prefab 双 `OcclusionTransparency` 粒度问题，再决定是否扩到命中标准统一、测试补强与工具修复。

### 会话 4 - 2026-03-17

- 子工作区 `遮挡检查` 本轮只做只读路由核查，不进入实现。
- 当前主线目标是明确：后续若从审计转整改，是否需要独立 worktree，以及 branch-only 应该如何进入。
- 父层已确认的稳定结论：
  - `遮挡检查` 当前不需要独立 worktree；
  - 现行默认姿势仍是“共享根目录 `D:\Unity\Unity_learning\Sunset @ main` 读取现场，再切 `codex/...` 分支进入真实整改”；
  - `worktree` 只保留为高风险隔离、故障修复、特殊实验例外，不是 `遮挡检查` 的默认入口。
- 推荐的 branch-only 开工入口：
  - 先回到共享根目录 `main`
  - 重新做 `preflight`
  - 若确认开整改，再切 `codex/occlusion-remediation-001`
