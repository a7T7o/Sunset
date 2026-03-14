# 导航检查 - 工作区记忆

## 模块概述

本工作区用于对 Sunset 当前导航系统做只读审计与证据化核实，目标不是立刻改代码，而是先建立一份可继续接手的现状基线。

## 当前状态

- 完成度：20%
- 最后更新：2026-03-15
- 状态：初步核实完成，尚未进入整改设计

## 会话记录

### 会话 1 - 2026-03-15

**用户需求**：
> `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查，这里是你的线程工作区，D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查，这里是你的sunset工作区，请你显彻底阅读线程文件夹下的所有内容然后进行核实，核实当然是理解所有内容后去进行阅读与当前系统的所有相关的代码和sunset相关工作区的所有文档，我要的是你最准确的理解然后生成一份从你的角度你的思考的分析文件在D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\1.0.0初步检查，并且要记得按照规则创建并更新memory，请你开始`

**当前主线目标**：
- 对 Sunset 导航系统做一次从线程旧稿到现状代码/场景/Prefab/Editor 的彻底核实，并建立后续整改可继续接手的基线。

**本轮子任务 / 阻塞**：
- 子工作区与父工作区当前都没有现成 `memory.md` 或其他工作文档，需要先确认“工作区文档缺失”是否属实，再补建 memory。

**完成任务**：
1. 读取线程目录既有文档 `01_导航代码索引.md`、`02_导航全盘分析报告.md`，提取旧稿结论。
2. 确认 `.kiro/specs/999_全面重构_26.03.15/导航检查` 与其父目录当前无现成文档，当前工作区侧没有导航检查基线。
3. 读取并核对 `.kiro/steering/README.md`、`rules.md`、`workspace-memory.md`、`systems.md` 与 `.codex/threads/线程分支对照表.md`。
4. 逐段核实导航主链代码：
   - `NavGrid2D`
   - `PlayerAutoNavigator`
   - `GameInputManager`
   - `IInteractable`
5. 核实直接依赖导航的业务代码：
   - `PlacementManager`
   - `PlacementNavigator`
   - `CropController`
   - `CropManager`
   - `ChestController`
   - `TreeController`
   - `NPCDialogueInteractable`
   - `CloudShadowManager`
   - `NavGrid2DStressTest`
   - `INavigationUnit`
6. 核实 Editor 和配置侧证据：
   - `PlayerAutoNavigatorEditor`
   - `CloudShadowManagerEditor`
   - `TagMaskEditors`
   - `Primary.unity`
   - `Physics2DSettings.asset`
   - 代表性 Prefab：箱子/作物/树/NPC
7. 在线程目录新增分析文件：
   - `.codex/threads/Sunset/导航检查/1.0.0初步检查/03_现状核实与差异分析.md`

**关键结论**：
1. 现状主链仍然是 `GameInputManager -> PlayerAutoNavigator -> NavGrid2D`，放置链是 `PlacementManager -> PlacementNavigator -> PlayerAutoNavigator`。
2. `PlacementNavigator` 不是场景静态挂载，而是 `PlacementManager.Start()` 运行时创建。
3. `Primary.unity` 中确实挂有 `GameInputManager`、`NavGrid2D`、`PlayerAutoNavigator`、`PlacementManager`、`CloudShadowManager`；`NavGrid2DStressTest` 存在但对象处于禁用态。
4. `CloudShadowManager` 确实存在通过反射读取 `NavGrid2D` 私有字段的代码，但 `Primary.unity` 当前 `enableCloudShadows = 0` 且 `areaSizeMode = Manual`，所以这条风险是潜在态，不是主场景当前活跃路径。
5. `NavGrid2D` 当前并不是真正 Zero GC：`IsPointBlocked()` 使用 `Physics2D.OverlapCircleAll()`，`GameInputManager` 多处使用 `Physics2D.OverlapPointAll()`；官方文档明确说明这两个 API 都会为返回数组分配内存。
6. 旧线程报告抓住了主链、刷新链和耦合点，但漏掉了“Zero GC 结论不成立”“CloudShadow 风险当前未激活”“PlacementNavigator 为运行时创建”这几个会影响整改优先级的事实。
7. `chest-interaction.md` 仍把箱子导航描述成“底部中心偏移目标点”，而现代码已经统一到 `ClosestPoint`，存在文档漂移。

**验证结果**：
- 工作区文档现状：子工作区为空目录，父工作区也暂无 `memory.md` / `requirements.md` / `design.md` / `tasks.md`。
- 路由与现场：`导航检查` 线程按对照表属于 A 类审计线程，当前目录 `D:\Unity\Unity_learning\Sunset`、当前分支 `main` 正确。
- 场景证据：`Primary.unity` 中 `NavGrid2D` / `GameInputManager` / `PlayerAutoNavigator` / `PlacementManager` 为真实挂载，`PlacementManager.navigator` 序列化为空，符合运行时创建逻辑。
- Prefab 证据：`ChestController` 分布于 `Assets/222_Prefabs/Box/*.prefab`，`TreeController` 分布于 `Assets/222_Prefabs/Tree/*.prefab`，`CropController` 分布于 `Assets/222_Prefabs/Crops/*.prefab`，`NPCDialogueInteractable` 分布于 `Assets/222_Prefabs/NPC/001.prefab`。
- 官方文档证据：
  - `Physics2D.OverlapCircleAll` 会分配返回数组
  - `Physics2D.OverlapPointAll` 会分配返回数组

**涉及的代码文件**

### 核心文件（主链）
| 文件 | 关系 | 说明 |
|------|------|------|
| `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs` | 核心读取 | 右键导航、交互桥接、农田导航状态机 |
| `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs` | 核心读取 | 执行器、停距计算、视线优化、卡顿恢复 |
| `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs` | 核心读取 | 网格构建、阻挡探测、A*、刷新事件 |
| `Assets/YYY_Scripts/Interfaces/IInteractable.cs` | 核心读取 | 统一交互契约 |

### 相关文件（旁路依赖）
| 文件 | 关系 | 说明 |
|------|------|------|
| `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs` | 读取 | 放置导航入口、运行时创建 `PlacementNavigator` |
| `Assets/YYY_Scripts/Service/Placement/PlacementNavigator.cs` | 读取 | `ClosestPoint` 到达判定 |
| `Assets/YYY_Scripts/Farm/CropController.cs` | 读取 | 作物通过 `IInteractable` 接入导航交互 |
| `Assets/YYY_Scripts/Farm/CropManager.cs` | 读取 | 已废弃，但明确说明作物交互已迁移 |
| `Assets/YYY_Scripts/World/Placeable/ChestController.cs` | 读取 | 箱子刷新 NavGrid、箱子交互 |
| `Assets/YYY_Scripts/Controller/TreeController.cs` | 读取 | 树碰撞变化触发延迟刷新 |
| `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs` | 读取 | NPC 通过 `IInteractable` 接入 |
| `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs` | 读取 | 可选从 NavGrid 推导渲染区域 |
| `Assets/YYY_Scripts/Service/Navigation/NavGrid2DStressTest.cs` | 读取 | 压测目标与现实现状存在偏差 |
| `Assets/YYY_Scripts/Service/Navigation/INavigationUnit.cs` | 读取 | 未来动态避让扩展点 |

### Editor / 场景 / 配置
| 文件 | 关系 | 说明 |
|------|------|------|
| `Assets/Editor/PlayerAutoNavigatorEditor.cs` | 读取 | LOS Tag 与 NavGrid Tag 同步工具 |
| `Assets/Editor/CloudShadowManagerEditor.cs` | 读取 | `FromNavGrid` / `AutoDetect` 模式配置入口 |
| `Assets/YYY_Scripts/Utils/Editor/TagMaskEditors.cs` | 读取 | NavGrid/GameInput/Placement 的 Tag 多选编辑器 |
| `Assets/000_Scenes/Primary.unity` | 读取 | 主场景挂载与参数事实 |
| `ProjectSettings/Physics2DSettings.asset` | 读取 | `m_AutoSyncTransforms: 0` 配置事实 |

**遗留问题 / 下一步**：
- [ ] 对 `OverlapCircleAll` / `OverlapPointAll` 的调用点做一次完整分配审计，确认哪些点值得优先改造成非分配路径。
- [ ] 设计跨模块统一的 NavGrid 刷新合并策略，而不是只在树或箱子单模块里继续加延迟。
- [ ] 修正文档漂移，至少把 `chest-interaction.md` 对箱子导航目标点的表述改到与 `ClosestPoint` 现状一致。
- [ ] 如果进入整改阶段，需要在本子工作区继续补 `requirements.md` / `design.md` / `tasks.md`。

---

## 相关文件

| 文件 | 说明 |
|------|------|
| `.codex/threads/Sunset/导航检查/1.0.0初步检查/01_导航代码索引.md` | 线程旧索引 |
| `.codex/threads/Sunset/导航检查/1.0.0初步检查/02_导航全盘分析报告.md` | 线程旧报告 |
| `.codex/threads/Sunset/导航检查/1.0.0初步检查/03_现状核实与差异分析.md` | 本轮新增分析 |

### 会话 1 - Git 收尾补记 - 2026-03-15

**执行动作**：
- 运行 `scripts/git-safe-sync.ps1 -Action sync -Mode governance -IncludePaths ...`

**结果**：
- 已创建并推送提交：`2026.03.15-01`（`3d7d65d5`）
- 当前分支仍为 `main`
- 推送后 upstream 状态：`behind=0, ahead=0`

**异常/注意事项**：
- 本次脚本除了白名单中的 4 个目标文件，还自动纳入了两份既有治理线 memory 改动：
  - `.kiro/specs/Steering规则区优化/memory.md`
  - `.kiro/specs/Steering规则区优化/当前运行基线与开发规则/memory.md`
- 原因不是本轮手工扩大范围，而是 `governance` 模式默认允许治理线目录；脚本预检时将这两份已存在的脏改一起判定为可同步。
- 仓库里仍保留大量与本线程无关的脏改，尤其是 NPC 线资产、其他线程 memory、以及本线程旧稿的删除/未跟踪状态，本轮未继续处理。
