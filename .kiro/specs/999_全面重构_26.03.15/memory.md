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

### 会话 5 - 2026-03-18

- 子工作区 `导航检查` 本轮以阶段 22 的线程唤醒协议做了一次只读回包，不进入实现。
- 当前父层重新确认的 stable 事实：
  - shared root live 现场为 `D:\Unity\Unity_learning\Sunset @ main`
  - 当前 `HEAD = 14838753b4ae9b09b2146b92fb3bfdc9ac82b2a0`
  - `git status --short --branch = ## main...origin/main`
  - `shared-root-branch-occupancy.md` 当前为 `main + neutral`
- `导航检查` 子线现状仍停在 `1.0.0` 审计基线，`2.0.0整改设计` 与 `requirements/design/tasks` 仍未建立。
- 父层本轮新增稳定结论：
  - 如果继续导航主线，下一轮必须进入 branch-only 真实写入，而不能继续把 shared root 当长期现场；
  - 推荐 continuation branch 仍为 `codex/navigation-audit-001`；
  - 最小 checkpoint 先固化 `2.0.0` 设计文档，再决定是否扩到 `GameInputManager / PlayerAutoNavigator / NavGrid2D` 的代码整改。

### 会话 4 - 2026-03-19（导航检查进入 queue waiting）

- 子工作区 `导航检查` 本轮不再只是“可继续 branch-only”的静态判断，而是已经实测进入 `queue-aware` waiting。
- live Git 在执行准入时不是 `main`，而是 `codex/npc-roam-phase2-003 @ 7385d1236d0b85c191caff5c5c19b08678d1cf80`；canonical `request-branch` 返回 `LOCKED_PLEASE_YIELD`，`ticket = 3`，`queue_position = 2`。
- `导航检查` 未进入 `codex/navigation-audit-001`，未做代码 checkpoint，固定回收卡已写回：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\线程回收\导航检查.md`
- 稳定 launcher 本轮暴露出 optional 参数未转发缺口，会直接吃掉 `request-branch` 所需的 `-BranchName`。
- 后续恢复点：等 shared root 回到 `main + neutral` 后，再让 `导航检查` 继续 `codex/navigation-audit-001` 上的首个非热文件 checkpoint。

### 会话 5 - 2026-03-22（遮挡检查进入 main 上真实整改）

- 子工作区 `遮挡检查` 本轮不再停在“旧分支遗留是否并回”的判断，而是已经在 `main` 上落下首个真实遮挡整改 checkpoint。
- 父层新增稳定事实：
  - `codex/occlusion-audit-001 @ 295e8138` 中仍有价值的部分已按“部分保留”迁回 `main`：
    - `BatchAddOcclusionComponents.cs` 工具对齐
    - `2.0.0整改设计` 四件套
  - 旧口径“因为 `295e8138` 还在分支上，所以当前不能继续”已作废，不再作为默认 blocker。
- 本轮已完成的真实整改落点：
  - `TreeController` 改为联动同树范围内全部相关 `OcclusionTransparency`
  - `OcclusionTransparency` 新增物理树根识别
  - `OcclusionManager` 改为按物理树去重处理树林缓存、恢复与 preview 恢复
  - `OcclusionSystemTests` 新增父/子双组件同树归属的最小证据
- 当前父层残留风险：
  - 早先 `recompile_scripts` 与旧工具路径下的测试调用曾返回 `Connection failed: Unknown error`
  - 但当前通过 `unityMCP` 的脚本验证、Console 读取和 `OcclusionSystemTests` EditMode 运行，已经拿到了最小可用验证闭环
- 父层新增稳定结论：
  - `遮挡检查` 这一轮可以视为“首个真实整改 checkpoint 已完成并通过最小专项验证”
  - 后续推进不需要再回到“是否先把旧分支并回”的阶段

### 会话 6 - 2026-03-22（遮挡系统源头溯源）

- 子工作区 `遮挡检查` 本轮暂停继续扩写实现，转为回顾历史工作区 `001_BeFore_26.1.21/遮挡与导航` 与相关 Docx。
- 父层新增稳定结论：
  - 最初“树林”语义指向的是“物理树构成的连通区域”，不是组件集合。
  - 历史设计中的林判断核心来自“树根距离 OR 树冠显著重叠”，不是简单的组件数量或组件 Bounds 统计。
  - 当前真正偏离源头设计的点，不在“有没有树林逻辑”，而在“物理树语义在后续实现里被双组件结构冲散了”。
- 当前父层判断：
  - `遮挡检查` 后续应优先继续围绕“边界树 / 内侧树 / 多树同时遮挡 / preview 恢复”四类行为做测试和收口，而不是回到抽象审计层。

### 会话 7 - 2026-03-23（遮挡检查主工程单实例验证完成）

- 子工作区 `遮挡检查` 已按批准窗口在主工程单实例里完成验证，不再停留在“验证入口阻塞”状态。
- 父层新增稳定事实：
  - 手动直连 `http://127.0.0.1:8888/mcp` 可正常驱动主工程验证。
  - 新增 6 条行为测试已全部通过。
  - `OcclusionSystemTests` 已扩到 20 条，并全部通过。
- 父层当前判断：
  - `遮挡检查` 这条线在 EditMode 专项验证层面已经通过当前整改范围的主要验收。
  - 后续如果继续推进，应转入更高一层的 PlayMode / 场景体感验证，或进入点击命中 / 工具命中双标准整改。

### 会话 8 - 2026-03-23（遮挡检查范围收尾）

- 子工作区 `遮挡检查` 已完成当前定义范围内的代码收口与主工程单实例验证。
- 父层新增稳定结论：
  - 当前这条线已不再残留“必须继续修”的核心遮挡 / 林判断问题。
  - 后续若继续开新刀，应视为下一阶段新业务，而不是本轮尾项。

### 会话 6 - 2026-03-22（导航检查进入 main 上真实整改）

- 子工作区 `导航检查` 已不再停留在 `codex/navigation-audit-001` 的 docs-first 挂起状态，而是进入 `main` 上的真实整改。
- 本轮父层新增稳定事实：
  - `codex/navigation-audit-001` 中仍有价值的遗留已按“部分保留”迁回 `main`：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\2.0.0整改设计\requirements.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\2.0.0整改设计\design.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\2.0.0整改设计\tasks.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\2.0.0整改设计\memory.md`
  - 旧口径“因为成果还在分支上，所以当前不能继续”已作废，不再作为默认 blocker。
- 本轮已落下的真实代码 checkpoint：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavGrid2D.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
- 本轮导航侧真实推进内容：
  - `NavGrid2D` 改为复用缓冲区做阻挡检测，避免继续走 `OverlapCircleAll(...)` 分配路径
  - `PlayerAutoNavigator` 开始承担动态导航障碍识别、移动 NPC 局部绕行、持续阻挡后的刷新重规划
  - 未来 `INavigationUnit` 接线入口已在玩家导航侧预留
- 当前父层残留风险：
  - 尚未做 Unity / MCP live 验证，不能把“代码已落下”冒充成“场景里已经确认稳定”

### 会话 7 - 2026-03-22（导航统一架构判断已前移）

- `导航检查` 子工作区本轮新增了一个比“首个代码 checkpoint”更上层的稳定判断：
  - Sunset 后续若继续处理玩家/NPC/未来怪物与宠物的移动问题，不应再把 `PlayerAutoNavigator` 和 `NPCAutoRoamController` 当成两条长期并行演化的导航主线。
- 父层当前认定的系统级问题是：
  - 现状属于“静态网格寻路 + 玩家侧补丁 + NPC 漫游侧补丁”的分裂架构；
  - 若目标是多种移动体共存、互让、绕行、追逐和跟随，后续需要的是共享导航核心，而不是继续对两个控制器分别雕花。
- 父层推荐的新边界：
  - 静态路径规划层
  - 动态代理避让层
  - 最终运动/接触解析层
- 所有移动体应共享前两层，行为差异只保留在各自 brain / controller 层。
