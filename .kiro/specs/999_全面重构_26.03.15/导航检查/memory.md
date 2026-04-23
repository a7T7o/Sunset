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

### 会话 2 - 2026-03-16（审计阶段文档固化）

**用户需求**：
> 全局已解冻，项目当前已恢复到可全面继续开发状态。你当前首先处理的是“文档重组与阶段状态收口”，不是直接进入代码整改。请你现在只做以下事情：1. 核对当前工作目录 / 分支 / HEAD / dirty。2. 明确你现在的文档删改、迁移、未跟踪项分别是什么。3. 先把本轮审计阶段文档固化。4. 再告诉我：你下一步是继续做代码整改，还是先以审计基线阶段结案。

**当前主线目标**：
- 在不进入代码整改的前提下，把 `导航检查` 线程的 `1.0.0初步检查` 正式收口为一个可交接的审计阶段。

**本轮子任务 / 阻塞**：
- 当前阻塞不是导航代码本身，而是线程文档仍处于“旧稿删除态 + 新阶段目录承接态”的迁移现场，需要先把阶段边界钉清。

**完成任务**：
1. 核对当前现场：工作目录 `D:\Unity\Unity_learning\Sunset`、分支 `codex/npc-main-recover-001`、`HEAD = b9b6ac48`，并读取当前 repo dirty 清单。
2. 区分当前导航线程相关的文档状态：
   - 删除态：线程根目录旧稿 `01_导航代码索引.md`、`02_导航全盘分析报告.md`
   - 迁移承接态：`1.0.0初步检查/01_导航代码索引.md`、`1.0.0初步检查/02_导航全盘分析报告.md`
   - 修改态：`memory_0.md`
   - 已存在基线稿：`1.0.0初步检查/03_现状核实与差异分析.md`
3. 新增阶段结案文档：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\1.0.0初步检查\04_审计阶段结案与移交建议.md`
4. 将本阶段结论回写到子工作区、父工作区与线程记忆，明确“当前更适合先以审计基线阶段结案，而不是直接进入代码整改”。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\1.0.0初步检查\04_审计阶段结案与移交建议.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\memory_0.md`

**关键结论**：
- `1.0.0初步检查` 现已具备独立结案条件：旧稿已迁入阶段目录，现状核实稿已存在，阶段结案文档已补齐，记忆链已同步。
- 当前更适合把本线程停在“审计基线阶段结案、等待是否立项整改”的状态，而不是直接滑入代码修改。
- 如果后续要继续导航整改，应该新开下一阶段并补 `requirements.md` / `design.md` / `tasks.md`，而不是继续在 `1.0.0初步检查` 内混做。

**遗留问题 / 下一步**：
- [ ] 如果用户确认继续导航整改，下一步先新开整改阶段文档，再进入方案和代码。
- [ ] 如果用户确认先阶段结案，则本子工作区当前可以作为只读审计基线保留。

### 会话 3 - 2026-03-18（阶段22只读唤醒回包）

**用户需求**：
> `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\可分发Prompt`，领取你的专属 prompt 并完成对第一阶段 prompt 读取和回应。

**当前主线目标**：
- 当前导航主线没有切换，仍是“从 `1.0.0` 审计基线过渡到下一阶段是否立项整改”的判断。
- 本轮子任务是：按阶段 22 的线程唤醒协议，只读回包当前 live 是否允许进入阶段二。

**完成任务**：
1. 在 `D:\Unity\Unity_learning\Sunset @ main` 上重新做只读 preflight：
   - `HEAD = 14838753b4ae9b09b2146b92fb3bfdc9ac82b2a0`
   - `git status --short --branch = ## main...origin/main`
2. 回读当前治理入口、shared root 占用文档、线程记忆、子工作区记忆，以及 `1.0.0` 的 `03/04` 审计文档。
3. 重新抽查 live 代码现状：
   - `GameInputManager.cs` 仍使用 `OverlapPointAll`
   - `PlayerAutoNavigator.cs` / `NavGrid2D.cs` 仍使用 `OverlapCircleAll`
   - `PlacementNavigator.cs` 仍走 `ClosestPoint`
   - `ChestController.cs` / `TreeController.cs` 仍直接触发 `OnRequestGridRefresh`
4. 将阶段一结论回写到：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\线程回收\导航检查.md`

**关键结论**：
- 当前 live 仍支持原有 `1.0.0` 审计结论，不存在“导航线程主要成果已丢失”的情况。
- 当前 live 仍未进入 `2.0.0整改设计`，所以如果继续导航主线，下一轮必须进入真实写入。
- 但这次真实写入必须是 branch-only：
  - 推荐分支仍为 `codex/navigation-audit-001`
  - 最小 checkpoint 先补 `requirements.md / design.md / tasks.md`
  - 不应在 `main` 上直接开始导航整改

**遗留问题 / 下一步**：
- [ ] 若治理批准阶段二，先申请 `grant-branch`，再执行 `ensure-branch`。
- [ ] 阶段二的第一步应先固化 `2.0.0整改设计`，而不是立刻扩大到 Unity / 热文件写入。

### 会话 4 - 2026-03-19（queue-aware业务准入 01 回收）

**用户需求**
> 从 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\可分发Prompt\导航检查.md` 领取本线程 prompt，并按 prompt 申请 continuation branch、开始写固定回收卡。

**当前主线目标**
- 从 `1.0.0` 审计基线进入 `2.0.0` 的最小 branch-only 整改 checkpoint，但前提是先拿到 `codex/navigation-audit-001` 的租约。

**本轮子任务 / 阻塞**
- 子任务是执行 `request-branch -> ensure-branch` 的 queue-aware 准入；阻塞是 shared root live 分支已切到 `codex/npc-roam-phase2-003`，不再是可发租约的 `main` 大厅。

**完成任务**
1. 读取 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\可分发Prompt\导航检查.md`，确认允许的 continuation branch 仍是 `codex/navigation-audit-001`，且本轮不得进入 Unity / MCP / Play Mode，不得碰 `GameInputManager.cs`。
2. 复核 live 现场并确认当前真实 Git 已变为：
   - `D:\Unity\Unity_learning\Sunset`
   - `codex/npc-roam-phase2-003`
   - `7385d1236d0b85c191caff5c5c19b08678d1cf80`
3. 先按 prompt 原文用稳定 launcher 执行 `request-branch`，实测命中 launcher optional 参数未转发缺口，`-BranchName` 在转发过程中丢失，返回 `request-branch 必须提供 -BranchName。`
4. 为避免退回仓库 working tree 脚本，改用 `main:scripts/git-safe-sync.ps1` 的手工等价 canonical 执行 `request-branch`，得到：
   - `STATUS: LOCKED_PLEASE_YIELD`
   - `TICKET: 3`
   - `QUEUE_POSITION: 2`
   - `REASON: 当前 live 分支是 'codex/npc-roam-phase2-003'，只有 main 大厅才能发放分支租约。`
5. 未继续 `ensure-branch`，未进入 Unity / MCP / Play Mode，未碰 `GameInputManager.cs` / `Primary.unity`，并已回写固定回收卡：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_queue-aware业务准入_01\线程回收\导航检查.md`

**关键结论**
- `导航检查` 本轮已正式进入 waiting，而不是继续停留在“可随时开工”的只读判断态。
- continuation branch 没变，仍是 `codex/navigation-audit-001`；只是当前 shared root 不可发租约。
- 稳定 launcher 对 optional 参数的转发现有缺口，会直接影响 queue-aware 准入命令。

**恢复点 / 下一步**
- 等 shared root 回到 `main + neutral` 且治理层重新唤醒 `导航检查`。
- 获得 `GRANTED / ALREADY_GRANTED` 后，再执行 `ensure-branch` 并只做首个非热文件 checkpoint：优先 `NavGrid2D / PlayerAutoNavigator`，不扩到 `GameInputManager.cs`、`Primary.unity`，也不进入 Unity / MCP。

### 会话 5 - 2026-03-22（main-only 旧分支收口 + 首个真实代码 checkpoint）

- 当前主线目标：不再把 `2.0.0整改设计` 停留在分支文档层，而是把它推进成 `main` 上的真实导航代码整改入口。
- 本轮已完成：
  - 处理旧分支 `codex/navigation-audit-001` 遗留：仅保留并迁回 `main` 的是 `2.0.0整改设计` 四件套；旧分支根层 docs-first 垫片直接判废，不再继续挂作 blocker。
  - 在 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavGrid2D.cs` 中，将 `IsPointBlocked()` 改为复用缓冲区查询，收掉已确认的一处分配式热点。
  - 在 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs` 中，补上移动 NPC / 动态导航单元的识别、局部绕行、持续阻挡触发重规划，以及未来 `INavigationUnit` 接线的兼容入口。
- 关键决策：
  - 这轮不继续扩写设计稿，也不进入热文件 `GameInputManager.cs`。
  - 首个真实代码 checkpoint 以 `NavGrid2D + PlayerAutoNavigator` 为边界，不越界碰 NPC 气泡、场景搭建或共享热区。
- 当前恢复点：
  - 下一轮优先做真实验证，确认“玩家自动导航绕移动 NPC”是否稳定；
  - 若验证通过，再决定是否继续扩到 NPC/NPC 局部规避的真实接线。

### 会话 6 - 2026-03-22（统一导航架构复判）

- 当前主线目标：判断 Sunset 是否应该从“玩家导航补丁 + NPC 漫游补丁”升级为“所有移动体共享的统一导航核心”。
- 本轮只读复盘后确认的关键事实：
  - `PlayerAutoNavigator` 与 `NPCAutoRoamController` 目前各自维护独立的寻路、卡住恢复和移动执行循环。
  - `NavGrid2D` 仍主要承担静态/准静态障碍网格职责，并不等于动态多代理导航系统。
  - 当前仓库中虽然已有 `INavigationUnit` / `NavigationUnitBase`，但真正接到共享导航主链上的实现还非常少。
  - `PlayerMovement` 与 `NPCAutoRoamController` 的最终运动语义也不一致：前者主要走 `rb.linearVelocity`，后者主要走 `rb.MovePosition(...)`。
- 当前判断：
  - 是，后续应重构为“共享导航核心 + 各自行为脑 + 各自运动适配器”，而不是继续在玩家导航和 NPC 漫游上分别堆补丁。
  - 当前继续只在 `PlayerAutoNavigator` / `NPCAutoRoamController` 上追加局部补丁，已经开始接近“在屎山上雕花”。
- 推荐重构方向：
  - 将问题拆成三层：静态路径规划层、动态代理避让层、最终运动/接触解析层。
  - 所有移动体共享前两层，不再各自维护一套独立的寻路与卡住恢复逻辑。
  - `GameInputManager`、`NPCAutoRoamController`、未来怪物/牲畜/宠物控制器，只保留为“行为脑/意图发起层”。

### 会话 7 - 2026-03-23（玩家阻挡态执行层修复）

**用户需求**：
> 用户指出实测 300 条日志后，玩家自动导航遇移动 NPC 依旧像推土机一样顶着走；要求彻查，不再回到 tag / 截图 / prefab 猜测，并明确要求把 NPC 组件事实和 MCP 现场一起查清。

**当前主线目标**：
- 把“共享避让已经在算，但玩家最终运动语义还在推人”这条根因收敛并落下最小真实修复。

**本轮子任务 / 阻塞**：
- 子任务是把玩家执行层补上真正的 blocked-state 运动语义。
- 当前阻塞是：`unityMCP` 服务基线虽然正常，但本会话 `list_mcp_resources` / `list_mcp_resource_templates` 仍为空，只能做到基线分类与序列化取证，无法直接做 live resource 读写。

**完成任务**：
1. 复跑 `list_mcp_resources` / `list_mcp_resource_templates`，确认当前会话依旧为空枚举，并结合 `mcp-live-baseline` 与 `unity-mcp-orchestrator` 技能口径，把现场明确分类为“服务正常、会话暴露异常”。
2. 直接回读 `Assets/222_Prefabs/NPC/001.prefab`、`Assets/222_Prefabs/NPC/003.prefab` 与 `Assets/000_Scenes/Primary.unity`，确认 NPC 真实组件链与刚体参数稳定存在：
   - NPC 仍是 `BoxCollider2D + Rigidbody2D + NPCMotionController + NPCAutoRoamController ...`
   - NPC 刚体仍是 `m_Mass = 6 / m_LinearDamping = 8 / m_CollisionDetection = 1`
   - 玩家主场景刚体仍是 `m_Mass = 1 / m_LinearDamping = 0 / m_CollisionDetection = 0`
3. 继续核对 `PlayerMovement.cs / PlayerAutoNavigator.cs / NavigationLocalAvoidanceSolver.cs / NPCAutoRoamController.cs`，确认问题已收敛到“玩家缺少 blocked-state 执行层”。
4. 修改 `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`：
   - 新增导航阻挡态约束
   - 近距时剥离继续冲向 blocker 的前向速度
   - 给阻挡态加低速上限与临时高阻尼
   - 在退出阻挡态时恢复默认刚体参数
5. 修改 `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`：
   - 在 `closeRangeConstraint.Applied` / `avoidance.ShouldRepath` / detour 等近距阻挡场景下改走 `SetBlockedNavigationInput(...)`
   - 普通导航输入改为显式 `SetNavigationInput(...)`
6. 运行验证：
   - `git diff --check`
   - `powershell -ExecutionPolicy Bypass -File D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1 -Action preflight -Mode task -OwnerThread 导航检查 -IncludePaths ...`

**修改文件**：
- `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs` - 新增导航阻挡态速度语义、去前冲与阻尼收束
- `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs` - 在近距阻挡 / 重规划 / detour 场景切换到阻挡态输入

**关键结论**：
1. 这轮再次确认：不是 tag 问题，也不是“NPC 没被识别到”；NPC prefab 本身和碰撞刚体配置都在。
2. 推土机感最像的根因是玩家执行层：`PlayerMovement.Update()` 仍在零阻尼玩家刚体上每帧直写 `linearVelocity`，把 solver 的减前冲 / 减速意图重新放大。
3. 这刀不再试图“让 solver 更聪明”，而是直接让 AutoNav 在近距阻挡态切换到低速、去前冲、临时高阻尼的执行语义。
4. 当前 `unityMCP` 服务基线依旧是好的，但本会话资源暴露仍异常；因此本轮 MCP 只用于分类和基线核查，不用于 live 资源操作。

**验证结果**：
- `list_mcp_resources` / `list_mcp_resource_templates`：空
- `git diff --check`：通过
- `git-safe-sync.ps1 -Action preflight ...`：通过
  - 代码闸门适用：`True`
  - 代码闸门通过：`True`
  - 程序集：`Assembly-CSharp`
- `Editor.log` 尾部未出现本轮新增的 Unity 编译报错，但新代码下的运行态 `NavAvoid` 仍待重新取证。

**遗留问题 / 下一步**：
- [ ] 重新进入 Unity 运行态，用新一轮 `[NavAvoid]` 日志确认 `rbVelocity` 是否已经被压回低速，且近距时不再继续朝 blocker 壳层内推进。
- [ ] 重点复验两个场景：玩家绕行移动 NPC；NPC/NPC 会车。
- [ ] 如果新日志仍显示被阻挡态存在异常尖峰，则继续收紧玩家刚体语义，而不是回退到 tag / prefab / MCP 挂载排查。

### 会话 8 - 2026-03-23（unityMCP 直连 live 对象取证）

**用户需求**：
> 用户追问“那你没做完吗？你没获取到 MCP 内容吗？”，并贴出当前 Unity Console 中持续刷新的 `[NavAvoid]` 截图，要求我明确说明当前到底有没有拿到 MCP 现场。

**当前主线目标**：
- 不再模糊描述“没拿到 MCP”，而是把当前会话里 MCP 哪些链路通、哪些链路还异常，直接用 live 场景对象取证说清楚。

**本轮完成**：
1. 直接通过 `unityMCP` 工具读取当前 active scene，确认当前就是 `Assets/000_Scenes/Primary.unity`，且场景处于 `isDirty = true`。
2. 用 `find_gameobjects` + `read_mcp_resource` 直接读取 live 玩家对象与 3 个 live NPC：
   - 玩家 instance id：`285878`
   - NPC instance id：`-356184 / -356316 / -353858`
3. 确认 live 玩家对象已挂载：
   - `PlayerMovement`
   - `PlayerAutoNavigator`
   - `Rigidbody2D`
   - `BoxCollider2D`
4. 确认 live `PlayerMovement` 已经出现本轮新增字段：
   - `blockedNavigationDamping`
   - `blockedNavigationMaxSpeedFactor`
   - `blockedNavigationMinSpeed`
   - `blockedNavigationSpeedBuffer`
   - `blockedNavigationForwardClearanceSlack`
   这说明新脚本已经编进当前 Editor 现场，而不只是仓库里改了文件。
5. 确认 3 个 live NPC 当前全部是 `tag = NPC`，并且都真实挂有：
   - `BoxCollider2D`
   - `Rigidbody2D`
   - `NPCMotionController`
   - `NPCAutoRoamController`
   其 live 物理参数仍是：
   - `BoxCollider2D.offset.y = 0.46`
   - `Rigidbody2D.mass = 6`
   - `Rigidbody2D.linearDamping = 8`
   - `Rigidbody2D.collisionDetectionMode = 1`
6. 同时确认当前会话仍存在 MCP 局部异常：
   - `list_mcp_resources / list_mcp_resource_templates` 仍为空
   - `read_console` 当前返回 0 条，即使用户截图中 Console 明显还在刷 `[NavAvoid]`

**关键结论**：
1. 当前不是“完全拿不到 MCP 内容”，而是：
   - `unityMCP` 的 scene / gameobject / component 直连链路是通的
   - `resources` 枚举层和 `read_console` 在本会话里仍有局部异常
2. 这次已经用 live 对象而不是 prefab 猜测证实：
   - 玩家对象与 3 个 NPC 都真实在场
   - NPC 组件链和物理参数都在
   - 本轮新增的玩家阻挡态字段已经编进现场
3. 因而当前主线不该再回到“有没有拿到 NPC / NPC 有没组件”的方向，而应继续盯运行态行为本身。

**当前恢复点**：
- 现在可以明确对用户说：我已经拿到 MCP live 内容了，只是当前会话的 `resources` 和 `read_console` 这两条链路还不稳定。
- 下一步若继续终验，优先继续用当前可用的 `unityMCP` scene / component 直连路径，不再退回截图驱动排查。

### 会话 2 - 2026-04-03（桥仍过不去：只读审计收敛到水层实体碰撞 + 桥覆盖 contract gap）

**用户需求**：
> 在 `D:\Unity\Unity_learning\Sunset` 只读审计过桥问题，不改文件；重点检查 `Primary.unity`、`TraversalBlockManager2D.cs`、`NavGrid2D.cs`、`PlayerMovement.cs`，回答真实阻挡源、scene 还是脚本、以及在拿不到 `Primary.unity` 写锁时还能先修哪部分脚本。

**当前主线目标**：
- 把 `Primary traversal` 中“桥还是过不去”的当前真阻挡源，从 scene 配置、导航阻挡和玩家实体碰撞三层里拆清楚。

**本轮子任务 / 阻塞**：
- 子任务是只读核对当前磁盘保存态，不进入 `Primary.unity` 实写，也不改脚本。
- 当前阻塞是：没有 `Primary.unity` 写锁，不能先用 scene 改法验证桥面 override / collider 方案。

**完成任务**：
1. 读取 `Primary.unity` 当前保存态中唯一一份 `TraversalBlockManager2D`：
   - `blockingTilemaps = [Layer 1 - Water, Layer 1 - 桥_物品0]`
   - `walkableOverrideTilemaps = [Layer 1 - 桥_底座]`
   - `bindPlayerMovement = 1`
   - `enforcePlayerNavGridBounds = 1`
2. 读取 `NavGrid2D` 当前逻辑，确认 `IsPointBlocked(...)` 的判定顺序是：
   - 先判 `explicit walkable override`
   - 再判 `explicit obstacle`
   - 最后才回落到 tags / layer / collider
3. 读取 `PlayerMovement` 与场景中玩家真实组件，确认：
   - 玩家仍是 `Rigidbody2D (Dynamic) + BoxCollider2D (non-trigger)` 的实体移动
   - `navigationFootProbe` 仍是“中心 + 左右脚”三点硬判定
4. 核对桥相关 scene truth：
   - `Layer 1 - 桥_底座` 非空
   - `Layer 1 - 桥_地表` 当前是空 Tilemap
   - `Layer 1 - Water` 有非 trigger `TilemapCollider2D`
   - `Layer 1 - 桥_物品0` 当前没有 `TilemapCollider2D`，且 manager 关闭了该 tilemap 的 occupancy fallback，因此它不是当前首要真阻挡源

**关键结论**：
1. 当前最可能的真实阻挡源，不是 `桥_物品0`，而是 `Layer 1 - Water` 的实体 `TilemapCollider2D` 仍在桥下参与真实物理碰撞；玩家仍是动态刚体实体，所以“导航允许但实体水层仍把人顶住”在当前保存态下完全可能同时成立。
2. 这不是纯 scene 问题，也不是纯脚本问题，而是两者叠加：
   - scene 侧：桥可走覆盖目前只落在 `桥_底座`，`桥_地表` 为空，水层实体碰撞也仍然存在；
   - 脚本侧：当前 `TraversalBlockManager2D / NavGrid2D / PlayerMovement` 只能表达“桥面导航可走”，还不能表达“桥上允许穿过水层实体碰撞”。
3. 在拿不到 `Primary.unity` 写锁时，仍然值得先推进的不是 `PlayerAutoNavigator`，而是 `TraversalBlockManager2D + NavGrid2D + PlayerMovement` 这三处 contract：
   - 让 manager 把现有 blocking / walkable override 源继续绑定给玩家运行时
   - 让 navGrid 暴露“当前脚底是否命中 walkable override”的半径查询
   - 让 player 在命中 bridge walkable override 时，对配置来源里的水层实体碰撞做 ignore / soft-pass，而不是继续被水 collider 硬顶住

**验证结果**：
- `Primary.unity` 当前只剩 1 份 `TraversalBlockManager2D` 序列化挂载，不再是早前的双 manager 竞争态。
- `Primary.unity` 当前 `NavGrid2D` 仍保存着显式 obstacle colliders，独立 manager 则在 `Awake` 时会再写一轮当前配置；因此 fresh 运行态和 stale 运行态都可能让“水仍挡桥”表现得像 scene 问题。
- 本轮为静态推断成立，未做 fresh Play / live collider 复测。

**遗留问题 / 下一步**：
- [ ] 若用户允许在无 scene 锁前继续推进，优先新开一刀脚本 slice，只碰：
  - `Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
  - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
- [ ] 若用户后续拿到 `Primary.unity` 写锁，再做 scene 侧复核：
  - 桥面 override 实际覆盖范围
  - 水层 collider 在桥下是否应保留
  - current bridge source 是否仍只该用 `桥_底座`

---

## 2026-04-19 Town 空气墙补查（只读）

### 用户反馈
1. `Town` 有多处“看起来没东西但会撞墙”的区域。
2. 用户补充强调：从截图体感看，不像是 `TraversalBlockManager2D` 凭空造出来的墙。
3. 这轮要求仍是只读查找和汇报，不改 scene。

### 这轮查到的关键点
1. `TraversalBlockManager2D` 本身不会创建新的 `Collider2D`：
   - 它做的是收集已有 `blockingTilemaps / blockingColliders`
   - 再同步给 nav/grid 与玩家/NPC 约束
2. `Town.unity` 里既有：
   - 手动配置的 `blockingTilemaps / blockingColliders`
   - 也开着 `autoCollectSceneBlockingColliders = 1`
3. 当前更像主嫌的是 scene 真实 collider，而不是 manager 单独造墙：
   - `基础设施/-9970/-9960/-9975`
   - `轨道/Props_*`
   - 这些区域存在 `TilemapCollider2D / CompositeCollider2D`

### 当前结论
1. `Town` 的空气墙更像场景里本来就有真实碰撞体或 tilemap collider 在挡人。
2. `TraversalBlockManager2D` 会放大“不能走”的结果，但不像它自己凭空生成了一堵物理墙。
3. 如果下一轮继续追，应先对位具体撞点对应哪一组 scene collider，不要先盲改 `TraversalBlockManager2D.cs`。
